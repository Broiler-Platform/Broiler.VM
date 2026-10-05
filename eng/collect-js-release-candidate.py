"""Take the JavaScript profile's release-candidate run of the pinned suite and retain it as one bundle.

    python eng/collect-js-release-candidate.py --bundle JS-10-003 \
        --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-003 \
        --suite <an unpacked checkout of the pinned test262 revision>

Phase F9's slice R3 (decision JSD-0061) takes release gate 9's facts:

  * A RUN FROM AN EXACT COMMIT. The manifest is written first and made read-only, naming the commit
    and whether the tree is clean, and the conformance root is built once from it and copied aside,
    so every run below is the same binary, whose digest the manifest records.
  * EVERY MANIFEST HAS ITS OWN TOTALS. One whole run per manifest and form a run can name: the wide
    manifest in the bytecode form and in the native form, the slice manifest, and the numeric
    manifest's native form. Four runs are four sets of totals; nothing here adds them.
  * THE EFFECTIVE LIMIT VECTOR each run was obtained under, read back by the conformance root's
    --effective-limits from a handle verified exactly as a variant's is, beside each run's totals.
  * THE FAILURE MANIFEST, generated from each run's merged report: every variant that failed or
    exhausted an allowance, with its reason. A construct outside a manifest is counted by family in
    the report beside it rather than listed, because under the slice manifest that is nearly every
    variant of the suite.
  * THE RATCHET: the two runs that have a floor in src/tests/conformance/floors are held to it by the
    runner's own --floor, and its verdict is retained.
  * NO AGGREGATE PERCENTAGE. This script computes no rate and adds no totals across runs.

Each run's merged report is retained compressed; its JSON summary and merge transcript beside it.
"""

import argparse
import datetime
import gzip
import hashlib
import os
import platform
import shutil
import stat
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PROJECT = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance",
                       "Broiler.VM.Composition.JavaScript.Conformance.csproj")
BUILT = os.path.join(ROOT, "src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance",
                     "bin", "Release", "net10.0")
BINARY = "Broiler.VM.Composition.JavaScript.Conformance"
PIN = os.path.join("src", "tests", "conformance", "pins", "test262.pin")
FLOORS = os.path.join(ROOT, "src", "tests", "conformance", "floors")

# The runner's own allowances, stated here so the limit vector is read under the same two.
FUEL = 100_000_000
WALL = 5_000

RUNS = [
    ("wide-bytecode", [], "test262-wide.floor"),
    ("wide-native", ["--form", "native"], "test262-wide-native.floor"),
    ("slice-bytecode", ["--manifest", "broiler.javascript.slice"], None),
    ("numeric-native", ["--manifest", "broiler.javascript.numeric", "--form", "native"], None),
]

# A failure-manifest line is every result that failed or spent an allowance. A construct outside the
# manifest is counted by family in the report and is not a failure of the manifest's own surface.
KEPT = ("Failed", "Exhausted")

# The runner's exit codes: 0 nothing failed, 1 cases failed - the ordinary outcome of a whole run -
# and 2, 3 and 4 a harness defect, an unretainable run and a crossed floor.
ACCEPTED = (0, 1)


def run(command, cwd=ROOT):
    result = subprocess.run(command, cwd=cwd, capture_output=True, text=True)
    return result.returncode, (result.stdout + result.stderr)


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for block in iter(lambda: stream.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def write(path, text):
    with open(path, "w", encoding="utf-8", newline="\n") as stream:
        stream.write(text)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--suite", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, (os.cpu_count() or 3) - 2))
    parser.add_argument("--only", action="append", default=[], help="take only the named runs")
    arguments = parser.parse_args()

    out = os.path.join(ROOT, arguments.out)
    os.makedirs(out, exist_ok=True)

    code, output = run(["dotnet", "build", PROJECT, "-c", "Release"])
    if code != 0:
        write(os.path.join(out, "build.log"), output)
        print("the conformance root did not build; see build.log")
        return 1
    binaries = tempfile.mkdtemp(prefix="broiler-js-rc-bin-")
    shutil.copytree(BUILT, binaries, dirs_exist_ok=True)

    runs = [entry for entry in RUNS if not arguments.only or entry[0] in arguments.only]
    commit = run(["git", "rev-parse", "HEAD"])[1].strip()
    status = run(["git", "status", "--porcelain"])[1]
    lines = [
        "bundle " + arguments.bundle,
        "evidence-class conformance",
        "decision Each run is recorded as this commit's release-candidate totals for its manifest and "
        "form; a run with a floor is recorded as not regressing it if the runner's --floor accepts "
        "it, and as regressing it otherwise. No other decision is taken, and no figure is combined "
        "across runs.",
        "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "commit " + commit,
        "tree " + ("clean" if status.strip() == "" else "dirty"),
        "sdk " + run(["dotnet", "--version"])[1].strip(),
        "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
        "binary %s sha256=%s" % (BINARY + ".dll", sha256(os.path.join(binaries, BINARY + ".dll"))),
        "suite-pin " + PIN + " sha256=" + sha256(os.path.join(ROOT, PIN)),
        "allowance fuel=%d wallClockMs=%d" % (FUEL, WALL),
        "jobs %d" % arguments.jobs,
    ]
    for name, extra, floor in runs:
        lines.append("run %s %s floor=%s" % (name, " ".join(extra) or "(defaults)", floor or "none"))
    if status.strip():
        lines.append("dirty:")
        lines.extend("  " + line for line in status.splitlines())
    manifest = os.path.join(out, "manifest.txt")
    write(manifest, "\n".join(lines) + "\n")
    os.chmod(manifest, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
    manifest_digest = sha256(manifest)
    print("manifest " + manifest + (" (clean tree)" if not status.strip() else " (DIRTY tree)"))

    verdicts = []
    for name, extra, floor in runs:
        code, output = run([os.path.join(binaries, BINARY), "--effective-limits",
                            "--fuel", str(FUEL), "--wall", str(WALL)] + extra)
        write(os.path.join(out, "limits-%s.log" % name), output.strip() + "\nexit code: %d\n" % code)

        transcript = tempfile.mkdtemp(prefix="broiler-js-rc-%s-" % name)
        command = [sys.executable, os.path.join(ROOT, "eng", "run-test262.py"),
                   "--suite", arguments.suite, "--binary-directory", binaries,
                   "--jobs", str(arguments.jobs), "--fuel", str(FUEL), "--wall", str(WALL),
                   "--out", transcript] + extra
        if floor:
            command += ["--floor", os.path.join(FLOORS, floor)]
        print("run %s ..." % name, flush=True)
        code, output = run(command)
        write(os.path.join(out, "%s.run.log" % name), output.strip() + "\nexit code: %d\n" % code)
        verdicts.append((name, code))

        report = os.path.join(transcript, "test262.report")
        if os.path.isfile(report):
            with open(report, "rb") as source, gzip.GzipFile(
                    os.path.join(out, "%s.report.gz" % name), "wb", compresslevel=9, mtime=0) as target:
                shutil.copyfileobj(source, target)
            failures = []
            with open(report, encoding="utf-8") as stream:
                for line in stream:
                    if line.startswith("total|"):
                        failures.insert(0, "# " + line.strip())
                    if line.startswith("result|"):
                        parts = line.rstrip("\n").split("|")
                        if len(parts) > 3 and parts[3] in KEPT:
                            failures.append(line.rstrip("\n"))
            write(os.path.join(out, "%s.failures.txt" % name),
                  "# failure manifest of run %s, generated from its merged report\n"
                  "# result|path|variant|verdict|family|dimension|kind|features|detail\n" % name +
                  "\n".join(failures) + "\n")
        for retained in ("test262.json", "merge.log", "floor.log"):
            path = os.path.join(transcript, retained)
            if os.path.isfile(path):
                shutil.copy(path, os.path.join(out, "%s.%s" % (name, retained)))
        shutil.rmtree(transcript, ignore_errors=True)

    if sha256(manifest) != manifest_digest:
        print("THE MANIFEST CHANGED WHILE THE RUNS WERE TAKEN")
        return 1

    hashes = []
    for entry in sorted(os.listdir(out)):
        if entry != "hashes.txt" and os.path.isfile(os.path.join(out, entry)):
            hashes.append("%s  %s" % (sha256(os.path.join(out, entry)), entry))
    write(os.path.join(out, "hashes.txt"), "\n".join(hashes) + "\n")
    shutil.rmtree(binaries, ignore_errors=True)

    for name, code in verdicts:
        print("%s: runner exit %d%s" % (name, code, "" if code in ACCEPTED else " - NOT RETAINABLE AS TAKEN"))
    return 0 if all(code in ACCEPTED for _, code in verdicts) else 1


if __name__ == "__main__":
    sys.exit(main())
