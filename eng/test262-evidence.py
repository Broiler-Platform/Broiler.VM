#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# TAKE AN EVIDENCE BUNDLE'S TEST262 RUNS ON HOSTED RUNNERS, BY THE PROCEDURE THE BUNDLES' RULES NAME.
#
# The bundles of the shared JavaScript libraries programme take test262 in both forms through bundle
# sjl-0-001's collector, which is eng/collect-js-release-candidate.py's procedure: eng/run-test262.py
# at the pin, the wide manifest, the bytecode form and the native form, 100,000,000 fuel and 5,000 ms
# per variant, each form held to its floor, every file of the run retained. On one machine an area's
# after arm holds that machine for the length of its runs. .github/workflows/test262-evidence.yml
# takes the same runs on GitHub's hosted runners, and this script is what it runs (milestone SJL-6,
# work package SJL-6.0).
#
# IT RUNS THE TESTED COMMIT'S OWN PROCEDURE, NOT A COPY OF IT. `--tree` is a checkout of the commit
# under test, and this script loads that checkout's eng/run-test262.py and calls it: `pinned()` and
# `identify()` verify the extracted suite against that checkout's pin before anything runs, exactly
# as run-test262.py's main() does, and `shard()` builds and runs every shard's command line, so each
# shard is the command a local run of that commit gives. What run-test262.py cannot do is spread its
# shards over machines; that, and the merge's bookkeeping, is all this script adds:
#
#   shards  one runner's slice of one form: the shards [first, first + count) of `--shards`, one
#           process each, their reports, transcripts and exit codes, the digest of every assembly of
#           the conformance root the runner built, and the runner's configuration;
#   merge   one form's whole run from every runner's slice: the shards' exit codes read as
#           run-test262.py reads them, the merge, the retention verdict and the floor taken by the
#           harness with the same commands and ranked by the same rule, and the files bundle
#           sjl-0-001's collector retains, under the names it gives them.
#
# THE PARTITION IS THE LOCAL ONE. A local run on four processors cuts each form into sixteen shards
# (run-test262.py's default, four per process), and the workflow cuts it into the same sixteen, four
# runners of four, so every shard holds the files it holds locally and runs with the same command.
#
# IT JUDGES NOTHING. Every verdict is the harness's or run-test262.py's rule applied to the harness's
# output; this script computes no rate and compares nothing with another run. A bundle imports the
# workflow's artifact unchanged and its own rule reads it.
#
#   python3 eng/test262-evidence.py shards --tree <checkout> --suite <suite> --binary-directory <dir>
#                                          --form bytecode|native --shards 16 --first <n> --count 4
#                                          --out <dir>
#   python3 eng/test262-evidence.py merge  --tree <checkout> --suite <suite> --binary-directory <dir>
#                                          --form bytecode|native --shards 16 --slices <dir> --out <dir>

import argparse
import concurrent.futures
import gzip
import hashlib
import importlib.util
import json
import os
import pathlib
import platform
import shutil
import subprocess
import sys

# The allowances eng/collect-js-release-candidate.py and bundle sjl-0-001's collector state, and the
# runs they take: one per form under the wide manifest, each with its floor.
FUEL = 100_000_000
WALL = 5_000

RUNS = {
    "bytecode": ("wide-bytecode", [], "test262-wide.floor"),
    "native": ("wide-native", ["--form", "native"], "test262-wide-native.floor"),
}

FLOORS = pathlib.Path("src", "tests", "conformance", "floors")

# A failure-manifest line is every result that failed or spent an allowance, as the collectors write it.
KEPT = ("Failed", "Exhausted")


def load(tree):
    """The tested checkout's eng/run-test262.py, as a module: its pin, its checks and its shard."""
    path = pathlib.Path(tree, "eng", "run-test262.py")
    spec = importlib.util.spec_from_file_location("run_test262_at_the_tested_commit", path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def binary_of(module, directory):
    # Absolute, because the harness runs with the tested checkout as its working directory.
    binary = pathlib.Path(directory).resolve() / module.BINARY_NAME
    if not binary.exists() and binary.with_name(binary.name + ".exe").exists():
        binary = binary.with_name(binary.name + ".exe")
    if not binary.exists():
        raise SystemExit(f"# no binary at {binary}")
    return binary


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for block in iter(lambda: stream.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def assemblies(directory):
    """The digest of every assembly the conformance root's build wrote, in the form `sha256sum` prints."""
    return "".join(f"{sha256(path)}  {path.name}\n" for path in sorted(pathlib.Path(directory).glob("*.dll")))


class Hider:
    """Writes paths as the collectors do: <tree>, <suite> and <scratch> in place of this machine's."""

    def __init__(self, **paths):
        self.paths = [(str(pathlib.Path(path).resolve()), f"<{name}>") for name, path in paths.items() if path]

    def __call__(self, text):
        for path, name in self.paths:
            text = text.replace(path, name)
        return text


def write(path, text):
    path = pathlib.Path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="\n") as stream:
        stream.write(text)


def run(command, cwd):
    environment = dict(os.environ)
    environment.setdefault("DOTNET_CLI_TELEMETRY_OPTOUT", "1")
    environment["DOTNET_CLI_UI_LANGUAGE"] = "en"
    done = subprocess.run(command, cwd=str(cwd), capture_output=True, encoding="utf-8", errors="replace",
                          env=environment)
    return done.returncode, (done.stdout or ""), (done.stderr or "")


def runner_configuration(module, binary, tree):
    """What the bundle records about the machine: the image, the processors, the SDK, the backend."""
    code, out, err = run(["dotnet", "--version"], tree)
    limits = run([str(binary), "--effective-limits", "--fuel", str(FUEL), "--wall", str(WALL),
                  "--form", "native"], tree)
    backend = next((line for line in limits[1].splitlines() if " form native " in f" {line} "), "(not reported)")
    lines = [
        f"image {os.environ.get('ImageOS', '(unknown)')} {os.environ.get('ImageVersion', '(unknown)')}",
        f"runner {os.environ.get('RUNNER_NAME', '(unknown)')} {os.environ.get('RUNNER_OS', '(unknown)')} "
        f"{os.environ.get('RUNNER_ARCH', '(unknown)')}",
        f"machine {platform.platform()} {platform.machine()} cpus={os.cpu_count()}",
        f"sdk {out.strip() or err.strip()}",
        f"native backend, as the harness reports it: {backend.rsplit(';', 1)[-1].strip()}",
    ]
    return "\n".join(lines) + "\n"


def shards(arguments):
    module = load(arguments.tree)
    tree = pathlib.Path(arguments.tree).resolve()
    suite = pathlib.Path(arguments.suite).resolve()
    binary = binary_of(module, arguments.binary_directory)
    name, extra, _ = RUNS[arguments.form]
    out = pathlib.Path(arguments.out).resolve()
    reports, logs = out / "shards", out / "logs"
    reports.mkdir(parents=True, exist_ok=True)
    logs.mkdir(parents=True, exist_ok=True)

    # The verification run-test262.py's main() takes before it starts a single shard; every shard
    # then verifies the checkout again through --expect.
    fields = module.pinned()
    digest, _ = module.identify(suite, fields)

    # The namespace run-test262.py's main() hands shard(), with the bundles' allowances and its own
    # defaults for everything they do not set.
    namespace = argparse.Namespace(manifest=module.WIDE_MANIFEST, form=arguments.form, backend=None,
                                   decline=[], dir=[], fuel=FUEL, wall=WALL, digest_cache=None)
    indices = list(range(arguments.first, arguments.first + arguments.count))
    print(f"# {name}: shards {indices[0]}..{indices[-1]} of {arguments.shards}, suite content {digest}", flush=True)
    with concurrent.futures.ThreadPoolExecutor(max_workers=len(indices)) as pool:
        done = list(pool.map(
            lambda index: module.shard(binary, suite, namespace, index, arguments.shards, reports, logs),
            indices))

    hide = Hider(tree=tree, suite=suite)
    broken = 0
    for index, code, transcript in done:
        write(logs / f"shard-{index:04d}.exit", f"{code}\n")
        write(transcript, hide(transcript.read_text(encoding="utf-8", errors="replace")))
        print(f"--- shard {index} of {arguments.shards} exited {code}", flush=True)
        if code not in (0, 1):
            broken = 1

    write(out / "binaries.sha256", assemblies(binary.parent))
    write(out / "runner.txt", runner_configuration(module, binary, tree))
    return broken


def merge(arguments):
    module = load(arguments.tree)
    tree = pathlib.Path(arguments.tree).resolve()
    suite = pathlib.Path(arguments.suite).resolve()
    binary = binary_of(module, arguments.binary_directory)
    name, extra, floor = RUNS[arguments.form]
    slices = pathlib.Path(arguments.slices).resolve()
    out = pathlib.Path(arguments.out).resolve()
    scratch = out.parent / f".merge-{name}"
    shutil.rmtree(scratch, ignore_errors=True)
    reports, logs = scratch / "shards", out / "shards" / name
    reports.mkdir(parents=True)
    logs.mkdir(parents=True, exist_ok=True)
    hide = Hider(tree=tree, suite=suite, scratch=scratch)

    lines = []

    def say(text):
        lines.append(text)
        print(text, flush=True)

    fields = module.pinned()
    digest, _ = module.identify(suite, fields)
    say(f"# test262 {fields['upstream']} at {fields['revision']}")
    say(f"# {fields['files']} files, content {digest} - the checkout answers to {module.PIN.name}")
    say(f"# manifest {module.WIDE_MANIFEST}; form {arguments.form}; declined (none)")

    # Every runner's slice of this form, one folder each, named slice-<form>-<runner> as the workflow
    # uploads it: its reports, its transcripts gzipped, its exit codes, its assemblies' digests and its
    # configuration.
    codes, runners = {}, []
    for runner in sorted(path for path in slices.iterdir()
                         if path.is_dir() and path.name.startswith(f"slice-{arguments.form}-")):
        for report in sorted((runner / "shards").glob("*.report")):
            shutil.copy(report, reports / report.name)
        for transcript in sorted((runner / "logs").glob("shard-*.log")):
            with open(transcript, "rb") as source, gzip.GzipFile(logs / (transcript.name + ".gz"), "wb",
                                                                 compresslevel=9, mtime=0) as target:
                shutil.copyfileobj(source, target)
        for exit_file in sorted((runner / "logs").glob("shard-*.exit")):
            codes[int(exit_file.stem.split("-")[1])] = exit_file.read_text().strip()
        runners.append(runner)
    say(f"# {arguments.shards} shards across {len(runners)} hosted runners, {FUEL} fuel and {WALL} ms per variant")
    say(f"# every shard's transcript is retained under shards/{name}/")

    for index in range(arguments.shards):
        say(f"--- shard {index} of {arguments.shards} exited {codes.get(index, '(no report: the shard did not run)')}; "
            f"shards/{name}/shard-{index:04d}.log.gz")

    code = 0
    # run-test262.py's rule: a shard that reported the harness's defect code measured nothing, and no
    # merge is taken. A shard that never ran is left to the merge, whose coverage check names it.
    failed = sorted(index for index, value in codes.items() if value == "3")
    if failed:
        say(f"# {len(failed)} shard(s) reported a harness defect and measured nothing: {failed}")
        code = 2
    else:
        merged = scratch / "test262.report"
        document = scratch / "test262.json"
        status, stdout, stderr = run([str(binary), "--merge", str(reports), "--report", str(merged),
                                      "--json", str(document)], tree)
        write(scratch / "merge.log", stdout + stderr)
        say(stdout.rstrip())
        if stderr.strip():
            say(stderr.rstrip())
        say(f"# the whole-run report is {name}.report.gz")
        say(f"# the machine-readable report is {name}.test262.json")
        code = status

        if document.exists():
            retention = json.loads(document.read_text(encoding="utf-8"))["retention"]
            if retention["retainable"]:
                say("# this run may be retained: pinned, whole, and its verdicts account for it")
            else:
                say("# THIS RUN MAY NOT BE RETAINED AS A WHOLE-SUITE FIGURE:")
                for reason in retention["reasons"]:
                    say(f"#   {reason}")
                if any("--expect" in reason or "account for" in reason for reason in retention["reasons"]):
                    code = 3

        status, stdout, stderr = run([str(binary), "--floor", str(tree / FLOORS / floor), "--report", str(merged)],
                                     tree)
        write(scratch / "floor.log", stdout + stderr)
        say(stdout.rstrip())
        if stderr.strip():
            say(stderr.rstrip())
        if status != 0 and code in (0, 1):
            code = 4

        if merged.is_file():
            with open(merged, "rb") as source, gzip.GzipFile(out / f"{name}.report.gz", "wb", compresslevel=9,
                                                             mtime=0) as target:
                shutil.copyfileobj(source, target)
            kept = []
            with open(merged, encoding="utf-8") as stream:
                for line in stream:
                    if line.startswith("total|"):
                        kept.insert(0, "# " + line.strip())
                    if line.startswith("result|"):
                        parts = line.rstrip("\n").split("|")
                        if len(parts) > 3 and parts[3] in KEPT:
                            kept.append(line.rstrip("\n"))
            write(out / f"{name}.failures.txt",
                  f"# failure manifest of hosted run {name}, generated from its merged report\n"
                  "# result|path|variant|verdict|family|dimension|kind|features|detail\n"
                  + "\n".join(kept) + "\n")
        for retained in ("test262.json", "merge.log", "floor.log"):
            path = scratch / retained
            if path.is_file():
                write(out / f"{name}.{retained}", hide(path.read_text(encoding="utf-8", errors="replace")))

    write(out / f"{name}.run.log", hide("\n".join(lines).strip() + f"\nexit code: {code}\n"))

    status, stdout, stderr = run([str(binary), "--effective-limits", "--fuel", str(FUEL), "--wall", str(WALL)]
                                 + extra, tree)
    write(out / f"limits-{name}.log", hide((stdout + stderr).strip() + f"\nexit code: {status}\n"))

    # One build: every runner's assemblies and this job's must hash alike, or the run mixed builds.
    mine = assemblies(binary.parent)
    listing = [f"# the SHA-256 of each list of the conformance root's assemblies, {name}; one distinct list is one build"]
    lists = {"merge": mine}
    for runner in runners:
        path = runner / "binaries.sha256"
        lists[runner.name] = path.read_text(encoding="utf-8") if path.is_file() else "(absent)\n"
    for label, text in lists.items():
        listing.append(f"{hashlib.sha256(text.encode()).hexdigest()}  {label}")
    distinct = len(set(lists.values()))
    listing.append(f"runners: {len(runners)}; lists: {len(lists)}; distinct lists: {distinct}")
    write(out / f"{name}.binaries.log", "\n".join(listing) + "\n")
    write(out / "binaries.sha256", mine)
    write(out / "test262-binary.txt", f"{module.BINARY_NAME}.dll sha256={sha256(binary.parent / (module.BINARY_NAME + '.dll'))}\n")
    configurations = [f"# {runner.name}\n{(runner / 'runner.txt').read_text(encoding='utf-8')}"
                      if (runner / "runner.txt").is_file() else f"# {runner.name}\n(absent)\n" for runner in runners]
    write(out / f"{name}.runners.txt", "".join(configurations))

    shutil.rmtree(scratch, ignore_errors=True)
    print(f"{name}: run-test262 exit {code}; {distinct} distinct assembly list(s) across {len(lists)}", flush=True)
    # 0, 1 and 4 are measurements, as the collectors read them; 2 and 3 are runs that could not be taken
    # or may not be retained, and a run that mixed builds is not one run.
    return 0 if code in (0, 1, 4) and distinct == 1 and len(runners) > 0 else 1


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    for command in ("shards", "merge"):
        sub = commands.add_parser(command)
        sub.add_argument("--tree", required=True, help="a checkout of the commit under test")
        sub.add_argument("--suite", required=True, help="the pinned suite, extracted")
        sub.add_argument("--binary-directory", required=True, help="the conformance root this job built")
        sub.add_argument("--form", required=True, choices=sorted(RUNS))
        sub.add_argument("--shards", type=int, required=True, help="the shards of the whole run")
        sub.add_argument("--out", required=True)
        if command == "shards":
            sub.add_argument("--first", type=int, required=True)
            sub.add_argument("--count", type=int, required=True)
        else:
            sub.add_argument("--slices", required=True, help="every runner's slice, one folder each")
    arguments = parser.parse_args()
    return shards(arguments) if arguments.command == "shards" else merge(arguments)


if __name__ == "__main__":
    sys.exit(main())
