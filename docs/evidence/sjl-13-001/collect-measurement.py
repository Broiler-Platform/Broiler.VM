#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect part J of bundle sjl-13-001's area rule, this family's compile measurement, into a sub-bundle.

    python3 -I docs/evidence/sjl-13-001/collect-measurement.py \
        --base-bin <the base commit's slice compiler, built in Release> --base-commit <forty digits> \
        --after-bin <the after commit's slice compiler, built in Release> --after-commit <forty digits> \
        --workloads <docs/evidence/sjl-13-001/inputs/workloads, as the rule commits it> --out <the sub-bundle>

WHY THIS FILE EXISTS. The rule's part J (decision-rule-area.md, as its revisions leave it, unchanged) names the slice
compiler's --measure-compile mode, which the tools commit adds, as the timed child, and says that the arms are
interleaved by the collector that runs it. No collector of this repository runs that mode: eng/collect-js-release-
candidate.py has no step for bundle sjl-13-001, and the measurement steps of the earlier bundles' collectors run the
command-line root over form workloads, not a compile. This is the smallest collector part J describes, written at
step 1's evidence (task 7.2), in the bundle's own folder, after the rule and before its runs, and it changes nothing
the rule fixes.

WHAT IT DOES, which is each line of part J and nothing else:
  - arms: B, the base commit's build; A, the after commit's; A-again, A's binaries run a second time under another
    lane name, the A/A lane. Each arm's binaries are named by hash in manifest.txt (binaries-b.sha256 and
    binaries-a.sha256, every file of each build's output), which is written and made read-only before any lane runs;
  - repetitions: three warm-up repetitions (eng/compare-forms.py's default), retained in runs.tsv and named as
    excluded from Q, then seven retained per lane (docs/baselines.md rule 4); in repetition r (from 0) the arms run in
    the order B, A, A-again rotated by r, each one child: `dotnet <arm> --measure-compile <workloads> --arm <lane>
    --runtime`, which compiles every workload and its control, a workload then its control, the bytecode form, warm
    (the mode's own untimed pass before its timed one);
  - the condition: every child's untimed check before its lane, its timed answers and its untimed check after its
    lane, file by file (outcome, diagnostics, artifact SHA-256 and length), against the first B child's, in
    conditions.tsv;
  - the configuration check: every child's effective-configuration line (`runtime-identifier=...`), numbered in
    runs.tsv and listed in summary.txt;
  - the quantities: Q(workload, arm), the median of the retained repetitions' compile wall time; N = |Q(A) -
    Q(A-again)|; D = Q(A) - Q(B); and, retained beside them, E(arm) = Q(workload, arm) - Q(control, arm), in
    summary.txt. It writes no verdict: part J's reading of D against N, and of the conditions, is the README's.
  - every child's whole output, in lanes.txt.gz, in the order the children ran; and hashes.txt over every file.

It judges nothing, kills nothing, and runs nothing but the children. Run it on an idle machine, as part J asks: it
records the load average before and after every child, and the machine, but does not wait for anything itself.
"""

import argparse
import datetime
import gzip
import hashlib
import os
import platform
import statistics
import stat
import subprocess
import sys
import time

ARMS = (("B", "base"), ("A", "after"), ("A-again", "after"))
WARMUP = 3
REPETITIONS = 7
DLL = "Broiler.VM.Composition.JavaScript.SliceCompiler.dll"


def utc():
    return datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for block in iter(lambda: stream.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def loadavg():
    with open("/proc/loadavg", encoding="ascii") as stream:
        return stream.read().strip()


def binaries(directory):
    lines = []
    for root, _, files in os.walk(directory):
        for name in files:
            full = os.path.join(root, name)
            lines.append("%s  %s" % (sha256(full), os.path.relpath(full, directory).replace(os.sep, "/")))
    return "".join(line + "\n" for line in sorted(lines, key=lambda line: line.split("  ", 1)[1]))


def write(out, name, text):
    with open(os.path.join(out, name), "w", encoding="utf-8", newline="\n") as stream:
        stream.write(text)


def main():
    parser = argparse.ArgumentParser()
    for name in ("--base-bin", "--after-bin", "--base-commit", "--after-commit", "--workloads", "--out"):
        parser.add_argument(name, required=True)
    arguments = parser.parse_args()
    out = os.path.abspath(arguments.out)
    os.makedirs(out, exist_ok=True)
    bins = {"base": os.path.abspath(arguments.base_bin), "after": os.path.abspath(arguments.after_bin)}
    workloads = os.path.abspath(arguments.workloads)
    env = {key: value for key, value in os.environ.items() if key != "LANG" and not key.startswith("LC_")}
    env["TZ"] = "UTC"
    env["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"

    # ---- the manifest, read-only before any lane runs
    write(out, "binaries-b.sha256", binaries(bins["base"]))
    write(out, "binaries-a.sha256", binaries(bins["after"]))
    sdk = subprocess.run(["dotnet", "--version"], capture_output=True, text=True, env=env).stdout.strip()
    files = sorted(name for name in os.listdir(workloads) if name.endswith(".js") or name == "README.md")
    lines = [
        "sub-bundle sjl-13-001/measurement",
        "evidence-class measurement",
        "decision docs/evidence/sjl-13-001/decision-rule-area.md part J, unchanged by its revisions of 2026-11-02, -04,"
        " -08, -09, -09b and -10",
        "collector docs/evidence/sjl-13-001/collect-measurement.py sha256=" + sha256(os.path.abspath(__file__)),
        "collected " + utc(),
        "arm-B-commit " + arguments.base_commit + " (the base commit)",
        "arm-A-commit " + arguments.after_commit + " (the after commit)",
        "arm-A-again the arm A binaries, measured a second time under another lane name",
        "binary arm B %s sha256=%s; every file of its output, binaries-b.sha256 sha256=%s" % (
            DLL, sha256(os.path.join(bins["base"], DLL)), sha256(os.path.join(out, "binaries-b.sha256"))),
        "binary arm A %s sha256=%s; every file of its output, binaries-a.sha256 sha256=%s" % (
            DLL, sha256(os.path.join(bins["after"], DLL)), sha256(os.path.join(out, "binaries-a.sha256"))),
        "root src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler, Release, under the JIT",
        "mode --measure-compile <workloads> --arm <lane> --runtime (the tools commit's)",
        "rid linux-x64",
        "sdk " + sdk,
        "machine %s %s cpus=%d" % (platform.platform(), platform.machine(), os.cpu_count()),
        "repetitions %d retained per lane (docs/baselines.md rule 4), %d warm-up excluded from Q (eng/compare-forms.py's"
        " default)" % (REPETITIONS, WARMUP),
        "order in each repetition r (from 0): the arms rotated by r from B, A, A-again; each arm one child, which"
        " compiles every workload and then its control, in the README's order",
        "form bytecode",
        "configuration-check every child is run with --runtime; its line starting runtime-identifier= is kept",
    ]
    lines += ["workload %s sha256=%s" % (name, sha256(os.path.join(workloads, name))) for name in files]
    manifest = os.path.join(out, "manifest.txt")
    write(out, "manifest.txt", "\n".join(lines) + "\n")
    for name in ("manifest.txt", "binaries-b.sha256", "binaries-a.sha256"):
        os.chmod(os.path.join(out, name), stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
    print("manifest written and made read-only: " + manifest, flush=True)

    # ---- the lanes
    runs = ["repetition\tretained\tposition\tarm\tfile\texit\tcondition-equals-B\tconfiguration\tcompile-ms"]
    conditions = ["repetition\tarm\tfile\twhen\toutcome\tdiagnostics\tsha256\tlength\tequals-B"]
    machine = ["child\trepetition\tarm\tstarted\tended\tloadavg-before\tloadavg-after\texit"]
    configurations = []
    reference = None
    times = {}
    raw = []
    child = 0
    for repetition in range(WARMUP + REPETITIONS):
        retained = repetition >= WARMUP
        order = ARMS[repetition % 3:] + ARMS[:repetition % 3]
        for position, (lane, side) in enumerate(order, 1):
            child += 1
            before_load, started = loadavg(), utc()
            result = subprocess.run(["dotnet", os.path.join(bins[side], DLL), "--measure-compile", workloads,
                                     "--arm", lane, "--runtime"], capture_output=True, text=True, env=env)
            after_load, ended = loadavg(), utc()
            machine.append("%d\t%d\t%s\t%s\t%s\t%s\t%s\t%d" % (child, repetition, lane, started, ended, before_load,
                                                              after_load, result.returncode))
            raw.append("=== child %d, repetition %d (%s), position %d, arm %s (%s binaries), exit %d\n%s%s" % (
                child, repetition, "retained" if retained else "warm-up, excluded from Q", position, lane, side,
                result.returncode, result.stdout, ("--- stderr ---\n" + result.stderr) if result.stderr else ""))
            config = next((line for line in result.stdout.splitlines() if line.startswith("runtime-identifier=")), "(none)")
            if config not in configurations:
                configurations.append(config)
            seen = {}
            for line in result.stdout.splitlines():
                fields = line.split("\t")
                if fields[0] in ("before", "after", "condition") and len(fields) == 7:
                    seen[(fields[0], fields[1])] = tuple(fields[2:])
                elif fields[0] == "time" and len(fields) == 4:
                    seen[("time", fields[2])] = fields[3]
            if reference is None and lane == "B":
                reference = {key[1]: value for key, value in seen.items() if key[0] == "before"}
            for (when, name), value in sorted(seen.items()):
                if when == "time":
                    continue
                equal = "yes" if reference is not None and reference.get(name) is not None and \
                    (value[1], value[2], value[3], value[4]) == tuple(reference[name][1:5]) else "NO"
                conditions.append("%d\t%s\t%s\t%s\t%s" % (repetition, lane, name, when, "\t".join(value[1:5])) +
                                  "\t" + equal)
            for (when, name), value in sorted(seen.items()):
                if when != "time":
                    continue
                cond = seen.get(("condition", name))
                equal = "yes" if cond is not None and reference is not None and name in reference and \
                    tuple(cond[1:5]) == tuple(reference[name][1:5]) else "NO"
                runs.append("%d\t%s\t%d\t%s\t%s\t%d\t%s\t%d\t%s" % (
                    repetition, "retained" if retained else "warm-up", position, lane, name, result.returncode, equal,
                    configurations.index(config) + 1, value))
                if retained:
                    times.setdefault((name, lane), []).append(float(value))
            print("child %d: repetition %d, arm %s, exit %d" % (child, repetition, lane, result.returncode), flush=True)

    write(out, "runs.tsv", "\n".join(runs) + "\n")
    write(out, "conditions.tsv", "\n".join(conditions) + "\n")
    write(out, "machine.tsv", "\n".join(machine) + "\n")
    with gzip.GzipFile(os.path.join(out, "lanes.txt.gz"), "wb", mtime=0) as stream:
        stream.write("".join(raw).encode("utf-8"))

    # ---- the quantities, without a verdict
    def q(name, lane):
        values = times.get((name, lane), [])
        return statistics.median(values) if values else float("nan")

    stems = sorted({name[:-3] for (name, _) in times if not name.endswith(".control.js")})
    summary = [
        "# part J of docs/evidence/sjl-13-001/decision-rule-area.md, collected by collect-measurement.py",
        "# Q is the median of the retained repetitions' compile wall time, in ms; N = |Q(A) - Q(A-again)|;"
        " D = Q(A) - Q(B); E(arm) = Q(workload, arm) - Q(control, arm), retained beside D and deciding nothing",
        "# no verdict is written here: part J's reading is the bundle README's",
        "# workload | Q(B) | Q(A) | Q(A-again) | N | D | Q(control, B) | Q(control, A) | Q(control, A-again) | E(B) |"
        " E(A) | E(A-again)",
    ]
    for stem in stems:
        w, c = stem + ".js", stem + ".control.js"
        qb, qa, qaa = q(w, "B"), q(w, "A"), q(w, "A-again")
        cb, ca, caa = q(c, "B"), q(c, "A"), q(c, "A-again")
        summary.append(" | ".join([stem] + ["%.3f" % value for value in (
            qb, qa, qaa, abs(qa - qaa), qa - qb, cb, ca, caa, qb - cb, qa - ca, qaa - caa)]))
    differing = sum(1 for line in conditions[1:] if line.endswith("\tNO"))
    summary.append("# condition lines (conditions.tsv) that differ from arm B's: %d; timed compilations whose"
                   " condition does: %d" % (differing, sum(1 for line in runs[1:] if line.split("\t")[6] == "NO")))
    summary.append("# children that exited other than 0: %d" % sum(
        1 for line in machine[1:] if line.split("\t")[7] != "0"))
    summary.append("# effective-configuration lines seen: %d (runs.tsv names each by its number)" % len(configurations))
    summary += ["#   %d: %s" % (index, line) for index, line in enumerate(configurations, 1)]
    write(out, "summary.txt", "\n".join(summary) + "\n")

    entries = []
    for name in sorted(os.listdir(out)):
        if name != "hashes.txt" and os.path.isfile(os.path.join(out, name)):
            entries.append("%s  %s" % (sha256(os.path.join(out, name)), name))
    write(out, "hashes.txt", "\n".join(entries) + "\n")
    print("done: %d children; summary.txt written" % child, flush=True)
    return 0


if __name__ == "__main__":
    sys.exit(main())
