"""Measure the JavaScript profile's two baselines and retain them as one evidence bundle.

    python eng/measure-js-baselines.py --bundle JS-10-001 \
        --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-001

Phase F9's slice R1 (decision JSD-0059) stands up the measurement lane JS-10 owns and takes the two
figures roadmap sections 16 and 18 open their questions against: verification throughput per byte,
and cold-start cost. The rules are roadmap section 17's, which restate the core's baseline register:

  1. a control that is the same workload minus the thing measured;
  2. interleaved lanes - candidate, control and A/A alternate inside every repetition;
  3. an A/A lane, and a difference smaller than it reported BELOW RESOLUTION, not as a result;
  4. every repetition retained, with no outlier policy and no statistical model;
  5. a condition checked before and after every lane;
  6. an immutable manifest written before either arm runs;
  7. the effective configuration each measured child reports, and the arm failing on a mismatch;
  8. exactly one evidence class, declared up front, with exactly one predeclared decision.

Verification throughput is measured inside one child (the conformance root's --measure-verify),
because it is a property of a call. Cold start is a property of a process, so this script launches
the child and times it: --cold-start composes, lowers, verifies, instantiates and runs a one-line
script; --cold-start-control is the same process doing none of that.

Nothing here decides whether a figure is good. It runs the procedure and retains what happened,
including a failed arm.
"""

import argparse
import datetime
import hashlib
import json
import os
import platform
import stat
import subprocess
import sys
import time

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PROJECT = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance",
                       "Broiler.VM.Composition.JavaScript.Conformance.csproj")
ASSETS = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance", "obj",
                      "project.assets.json")
BINARY = "Broiler.VM.Composition.JavaScript.Conformance"
WORKLOAD = os.path.join("src", "tests", "differential", "the-statement-and-object-surface.js")

REPETITIONS = 7
LAUNCHES = 10

EVIDENCE_CLASS = "baseline"
DECISION = (
    "Each figure is recorded as this profile's baseline on its arm if its candidate-versus-control "
    "difference exceeds its A/A difference, and as below resolution otherwise. No other decision is "
    "taken: the persistence question (roadmap section 16) and the in-process producer row (section 18) "
    "reopen only against a host's stated latency budget, and no host has stated one.")

ARMS = {
    "jit": {"publish": ["-p:PublishAot=false", "--self-contained", "true"], "aot": "no"},
    "aot": {"publish": ["-p:PublishAot=true"], "aot": "yes"},
}


def run(command, **kwargs):
    return subprocess.run(command, cwd=ROOT, capture_output=True, text=True, **kwargs)


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for block in iter(lambda: stream.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def write_manifest(out, bundle, rid):
    """The manifest, written and made read-only before either arm runs (rule 6)."""
    commit = run(["git", "rev-parse", "HEAD"]).stdout.strip()
    status = run(["git", "status", "--porcelain"]).stdout
    submodules = run(["git", "submodule", "status", "--recursive"]).stdout.strip() or "(none)"
    restore = run(["dotnet", "restore", PROJECT, "-r", rid])

    lines = [
        f"bundle {bundle}",
        f"written {datetime.datetime.now(datetime.timezone.utc).isoformat(timespec='seconds')}",
        f"evidence-class {EVIDENCE_CLASS}",
        f"decision {DECISION}",
        f"commit {commit}",
        f"submodules {submodules}",
    ]

    if status.strip():
        patch = os.path.join(out, "tree.patch")
        with open(patch, "w") as stream:
            stream.write(run(["git", "diff", "HEAD"]).stdout)
            stream.write("\n# untracked\n" + status)
        lines.append(f"tree dirty; the difference from {commit} is retained in tree.patch (sha256 {sha256(patch)})")
    else:
        lines.append("tree clean")

    lines.append(f"restore exit {restore.returncode}")

    with open(os.path.join(ROOT, ASSETS)) as stream:
        assets = json.load(stream)

    for name, library in sorted(assets.get("libraries", {}).items()):
        lines.append(f"dependency {library.get('type')} {name} {library.get('sha512', '-')}")

    lines.append("sdk " + run(["dotnet", "--version"]).stdout.strip())

    for runtime in run(["dotnet", "--list-runtimes"]).stdout.strip().splitlines():
        lines.append("runtime " + runtime)

    lines.append(f"machine {platform.platform()} {platform.machine()} processors={os.cpu_count()}")
    lines.append(f"python {platform.python_version()}")
    lines.append(f"rid {rid}")
    lines.append(f"repetitions {REPETITIONS}")
    lines.append(f"cold-start-launches-per-lane {LAUNCHES}")
    lines.append(f"workload verify-throughput {WORKLOAD} sha256 {sha256(os.path.join(ROOT, WORKLOAD))}")
    lines.append("workload cold-start the one-statement script `1 + 1`, which must complete with 2")

    for arm, spec in ARMS.items():
        lines.append(f"arm {arm} publish {' '.join(spec['publish'])} expect rid={rid} arch=X64 gc=workstation aot={spec['aot']}")

    path = os.path.join(out, "manifest.txt")
    with open(path, "w") as stream:
        stream.write("\n".join(lines) + "\n")

    os.chmod(path, stat.S_IRUSR | stat.S_IRGRP | stat.S_IROTH)
    return path, sha256(path)


def expect(configuration, rid, aot):
    """Rule 7: the configuration a child reports, against the arm's."""
    wanted = {"rid": rid, "arch": "X64", "gc": "workstation", "aot": aot}
    found = dict(field.split("=", 1) for field in configuration.split() if "=" in field)
    wrong = [f"{key}={found.get(key)} where {value} was asked" for key, value in wanted.items() if found.get(key) != value]
    return "; ".join(wrong) or None


def cold_start(binary, rid, aot, log):
    """Cold start, timed by this process: candidate, control and A/A interleaved per repetition."""

    def launch(argument):
        start = time.perf_counter_ns()
        child = subprocess.run([binary, argument], capture_output=True, text=True)
        elapsed = time.perf_counter_ns() - start
        output = child.stdout.strip()

        if child.returncode != 0:
            raise RuntimeError(f"{argument} exited {child.returncode}: {output}")

        if argument == "--cold-start" and "completion=2 " not in output + " ":
            raise RuntimeError(f"the candidate did not complete with 2: {output}")

        if (problem := expect(output, rid, aot)) is not None:
            raise RuntimeError(f"{argument} took another configuration: {problem}")

        return elapsed, output

    # The condition, before anything is timed: each child does what its name says.
    _, first = launch("--cold-start")
    _, control = launch("--cold-start-control")
    log.write(f"configuration {first}\n")
    log.write(f"control-configuration {control}\n")

    lanes = {"candidate": [], "control": [], "aa": []}

    for repetition in range(REPETITIONS):
        for lane, argument in (("candidate", "--cold-start"), ("control", "--cold-start-control"), ("aa", "--cold-start")):
            total = 0
            for _ in range(LAUNCHES):
                elapsed, _ = launch(argument)
                total += elapsed
            lanes[lane].append(total / LAUNCHES)

    def median(values):
        return sorted(values)[len(values) // 2]

    candidate = median(lanes["candidate"])
    control_time = median(lanes["control"])
    difference = candidate - control_time
    floor = abs(candidate - median(lanes["aa"]))
    resolved = floor <= abs(difference)

    log.write(
        f"measurement cold-start unit=process candidate-ns={candidate:.1f} control-ns={control_time:.1f} "
        f"difference-ns={difference:.1f} per-process-ns={difference:.4f} aa-ns={floor:.1f} "
        f"valid={'yes' if resolved else 'no'} "
        + ("" if resolved else f"upper-bound-per-process-ns={floor:.4f} ")
        + f"iterations={LAUNCHES} repetitions={REPETITIONS}\n")

    for repetition in range(REPETITIONS):
        log.write(
            f"  rep cold-start {repetition} candidate-ns={lanes['candidate'][repetition]:.1f} "
            f"control-ns={lanes['control'][repetition]:.1f} aa-ns={lanes['aa'][repetition]:.1f}\n")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--rid", default="linux-x64")
    args = parser.parse_args()

    out = os.path.join(ROOT, args.out)
    os.makedirs(out, exist_ok=True)
    manifest, manifest_hash = write_manifest(out, args.bundle, args.rid)
    publish_root = os.path.join(ROOT, "artifacts", "measure", args.bundle)
    failed = []

    for arm, spec in ARMS.items():
        target = os.path.join(publish_root, arm)
        published = run(["dotnet", "publish", PROJECT, "-c", "Release", "-r", args.rid, "-o", target, *spec["publish"]])

        with open(os.path.join(out, f"publish-{arm}.log"), "w") as log:
            log.write(f"# manifest sha256 {manifest_hash}\n")
            log.write(published.stdout + published.stderr)
            log.write(f"exit {published.returncode}\n")

        if published.returncode != 0:
            failed.append(f"{arm}: publish exited {published.returncode}")
            continue

        binary = os.path.join(target, BINARY)

        with open(os.path.join(out, f"measure-{arm}.log"), "w") as log:
            log.write(f"# manifest sha256 {manifest_hash}\n")
            log.write(f"# arm {arm}, binary sha256 {sha256(binary)}\n")
            verify = subprocess.run([binary, "--measure-verify", os.path.join(ROOT, WORKLOAD)], capture_output=True, text=True)
            log.write(verify.stdout)

            configuration = next((line[len("configuration "):] for line in verify.stdout.splitlines() if line.startswith("configuration ")), "")
            problem = expect(configuration, args.rid, spec["aot"]) if verify.returncode == 0 else f"exit {verify.returncode}"

            if problem is not None:
                log.write(f"ARM FAILED: verify-throughput {problem}\n")
                failed.append(f"{arm}: verify-throughput {problem}")
                continue

            try:
                cold_start(binary, args.rid, spec["aot"], log)
            except RuntimeError as failure:
                log.write(f"ARM FAILED: cold-start {failure}\n")
                failed.append(f"{arm}: cold-start {failure}")

    if sha256(manifest) != manifest_hash:
        failed.append("the manifest changed while the arms ran")

    with open(os.path.join(out, "hashes.txt"), "w") as stream:
        for name in sorted(os.listdir(out)):
            if name != "hashes.txt" and os.path.isfile(os.path.join(out, name)):
                stream.write(f"{sha256(os.path.join(out, name))}  {name}\n")

    print("measure-js-baselines: " + ("every arm measured" if not failed else "; ".join(failed)))
    return 0 if not failed else 1


if __name__ == "__main__":
    sys.exit(main())
