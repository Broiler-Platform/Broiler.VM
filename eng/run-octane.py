#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# RUN THE RETAINED OCTANE WORKLOAD, ONE BENCHMARK PER PROCESS, AND RETAIN WHAT EVERY ONE OF THEM DID.
#
# The workload roadmap's JSW-10 asks for the Octane checkout to be pinned and archived so that a
# benchmark result has an identity, and section 1 states the target as behaviour: every benchmark in
# the checkout reports a score through the ordinary command line of the end-user host, driven one
# benchmark per process by `src/tests/octane/run-one.js`, with the process exit code agreeing with
# whether a score was produced.
#
# This script is that command line, run from the pin rather than from a directory somebody happens
# to have:
#
#   * it extracts `src/tests/octane/pins/octane-<revision>.tar.gz` into a scratch directory;
#   * it CHECKS the archive against `octane.pin` before extracting a byte of it, so a run is against
#     the revision the repository decided rather than against whatever is on the disk;
#   * it runs each benchmark with the files that benchmark needs, in the order the suite's own
#     `index.html` loads them;
#   * and it retains every line, failures included, rather than the passing half.
#
# IT COMPARES NOTHING AND RETAINS NOTHING. A score printed here is a number about this
# configuration; there is no measurement lane and no baseline register, both of which are `JS-10`'s,
# and roadmap section 17 governs any figure that is ever retained. What this answers is the question
# section 1 asks: does the benchmark REPORT a score, or does it meet something.
#
# IT DOES NOW PUBLISH THE SUITE'S OWN AGGREGATE, added 2026-09-07, and the distinction between
# publishing an aggregate and retaining a figure is the whole of why that is allowed. Octane's final
# score is defined by its own harness - `100 * GeometricMean(BenchmarkSuite.scores)` over every
# ratio the suites pushed - and this repository runs one benchmark per process, so until now the
# number the workload exists to produce was the one number nobody computed. Computing it is reading
# the suite's definition back off the run. Writing it into a document is not, and this script writes
# it into none: it prints it, and a `--report` a caller asks for writes a machine-readable file
# OUTSIDE the checkout's documents, for a lane to diff and a reader to read.
#
#   python3 eng/run-octane.py [--binary-directory <dir>] [--only <name>]... [--skip <name>]...
#                             [--fuel <n>] [--wall <ms>] [--live-bytes <n>] [--max-depth <n>]
#                             [--report <path>]
#
# `--only` may be repeated and names a SELECTION; with none of them the fifteen in the pin all run.
# `--skip` removes a benchmark from whatever that selection came to, and every run that uses one
# NAMES it - once before anything runs and once in the summary - because a ratio over a shrunken
# denominator reads exactly like a complete run. Skipping everything is refused.
# Each benchmark's wall-clock duration is printed beside its exit code, because `--wall` is an
# allowance a caller has to state and there was nothing to state one from.
#
# `--wall` IS A REQUEST AND THE PROFILE BOUNDS IT. `JavaScriptProfile.Maxima()` sets the
# `WallClock` maximum at 3,600,000 ms and calls it a maximum a host may tighten and may never
# loosen, so an hour is the most any composition of this profile can be granted: a run asking for
# ninety minutes is bounded at sixty and reports the shorter one met. The figure is not repeated
# here as a constant, because a copy of it in another language is a copy that goes stale; what the
# summary line says is what was REQUESTED, and what actually bound the run is in the run's own
# answer beside the duration.
#
# THE HOST IS ASKED FOR `--quiet` AND THAT IS A CORRECTION, dated 2026-09-07. Every benchmark block
# in every transcript this script has ever written ended with a bare `undefined` line, which was the
# host printing the completion value of `run-one.js` - the driver's last statement is a `throw`
# guard whose value is nothing, and the host prints the completion value of a run of named files
# sharing one realm. It was never a benchmark's output and it was never an error; it was noise in
# the middle of a transcript that a reader has to learn to ignore, which is the kind of thing that
# teaches a reader to ignore the next line too. `--quiet` suppresses the completion value and
# nothing else: the driver's own `print` calls are the write capability and are untouched.

import argparse
import hashlib
import json
import math
import pathlib
import platform
import shutil
import subprocess
import sys
import tarfile
import tempfile
import time
from datetime import datetime, timezone

# The report format's own version, bumped whenever a reader of an older file would misread a newer
# one. It is NOT the pin's version and not the suite's: it names the shape of the JSON below.
REPORT_SCHEMA = "broiler-js octane report"
REPORT_VERSION = 1

ROOT = pathlib.Path(__file__).resolve().parent.parent
PINS = ROOT / "src/tests/octane/pins"
PIN = PINS / "octane.pin"
DRIVER = "src/tests/octane/run-one.js"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# The files each benchmark needs beside `base.js`, in the order the suite's own index.html loads
# them. Every other benchmark is one file named after itself.
COMPANIONS = {
    "gbemu": ["gbemu-part1.js", "gbemu-part2.js"],
    "zlib": ["zlib.js", "zlib-data.js"],
    "typescript": ["typescript.js", "typescript-input.js", "typescript-compiler.js"],
}

BENCHMARKS = [
    "richards", "deltablue", "crypto", "raytrace", "earley-boyer", "regexp", "splay",
    "navier-stokes", "pdfjs", "mandreel", "gbemu", "code-load", "box2d", "zlib", "typescript",
]


def pinned():
    """The pin's fields, or a failure."""
    if not PIN.exists():
        raise SystemExit(f"# no pin at {PIN}")

    fields = {}

    for line in PIN.read_text().splitlines():
        line = line.strip()

        if not line or line.startswith("#"):
            continue

        key, _, value = line.partition(" ")
        fields[key] = value.strip()

    return fields


def extract(fields, into):
    """Checks the archive against the pin and extracts it. A disagreement is refused, never fixed."""
    archive = ROOT / fields["archived-at"]

    if not archive.exists():
        raise SystemExit(f"# the pin names an archive that is not here: {archive}")

    digest = hashlib.sha256(archive.read_bytes()).hexdigest()

    if digest != fields["archive-sha256"]:
        raise SystemExit(
            f"# the archive does not hash to what the pin says\n"
            f"#   pin  {fields['archive-sha256']}\n#   file {digest}"
        )

    with tarfile.open(archive) as opened:
        members = sorted((m for m in opened.getmembers() if m.isfile()), key=lambda m: m.name)
        content = hashlib.sha256()

        for member in members:
            path = member.name.split("/", 1)[1]
            body = opened.extractfile(member).read()
            content.update((f"{path}\n{hashlib.sha256(body).hexdigest()}\n").encode())

        if content.hexdigest() != fields["content-sha256"]:
            raise SystemExit("# the archive's contents do not hash to what the pin says")

        if len(members) != int(fields["files"]):
            raise SystemExit(
                f"# the archive holds {len(members)} files and the pin says {fields['files']}"
            )

        opened.extractall(into, filter="data")

    return into / f"octane-{fields['revision']}"


def read(output):
    """What the driver said, read back off its own lines and nothing else.

    THE THREE LINE KINDS ARE THE DRIVER'S, NOT THIS SCRIPT'S. `src/tests/octane/run-one.js` prints
    `result <name> <formatted>` per suite result, `score <formatted>` once, and `component <ratio>`
    once per entry of `BenchmarkSuite.scores`. The COMPONENTS are what an aggregate is computed
    from - they are the harness's own unrounded ratios in push order - and the other two are what a
    person reads. A line this script does not recognise is left alone; a benchmark that printed no
    component reports none, and the aggregate refuses rather than inventing one.
    """
    results = []
    components = []
    score = None

    for line in output.splitlines():
        parts = line.split()

        if len(parts) == 3 and parts[0] == "result":
            results.append({"name": parts[1], "score": parts[2]})
        elif len(parts) == 2 and parts[0] == "score":
            score = parts[1]
        elif len(parts) == 2 and parts[0] == "component":
            try:
                components.append(float(parts[1]))
            except ValueError:
                # A component that is not a number is a defect in the driver or in the host, and a
                # silently dropped one would shrink the aggregate's denominator. It is kept as the
                # unreadable thing it is so that `aggregate()` refuses on it.
                components.append(None)

    return results, score, components


def aggregate(components):
    """Octane's own final score, over the ratios its own harness pushed.

    `BenchmarkSuite.RunSuites` computes `100 * GeometricMean(BenchmarkSuite.scores)` and
    `GeometricMean` is `exp(sum(log(x)) / n)`. This is that, over the components of every process in
    the selection, in the order they were produced.

    WHY IT IS NOT A MEAN OVER THE FIFTEEN `score` LINES. Two suites - `splay` and `mandreel` - carry
    a latency reference and push TWO ratios each, so the suite's own aggregate is a geometric mean
    over SEVENTEEN numbers and not fifteen. A mean over the per-process scores would weight those
    two suites once where Octane weights them twice, and would answer a number Octane does not
    define. The other reason is rounding: a `score` line has been through `FormatScore`, which keeps
    three significant digits.

    IT IS NOT A FIGURE ANY DOCUMENT MAY RETAIN. It is the run's own arithmetic over one run on one
    machine, with no control, no A/A lane and no repetitions - roadmap section 17 and rule L1 govern
    what a register may hold, and this is none of it.
    """
    if not components or any(value is None or value <= 0.0 for value in components):
        return None

    return 100.0 * math.exp(sum(math.log(value) for value in components) / len(components))


def whole_or_partial(wanted, skipped, missing):
    """`whole`, or `partial|<reasons>` - the conformance harness's vocabulary, not a new one.

    A run is whole when all fifteen were attempted and all fifteen scored. Every other run names
    what it is missing and why, in one field, so that a reader who sees only the aggregate line
    still meets the shortfall: a NARROWING by `--only`, an EXCLUSION by `--skip` and a REFUSAL by
    the profile are three different facts, and each of them on its own makes the aggregate a
    different quantity from the one Octane defines.
    """
    reasons = []
    unselected = [name for name in BENCHMARKS if name not in wanted and name not in skipped]

    if unselected:
        reasons.append("not selected: " + ", ".join(unselected))

    if skipped:
        reasons.append("excluded by the caller: " + ", ".join(skipped))

    if missing:
        reasons.append("did not exit zero: " + ", ".join(missing))

    return "whole" if not reasons else "partial|" + "; ".join(reasons)


def run(binary, checkout, name, fuel, wall, live_bytes, max_depth):
    """One benchmark, one process, through the ordinary command line."""
    files = [str(checkout / "base.js")]
    files += [str(checkout / f) for f in COMPANIONS.get(name, [f"{name}.js"])]

    # THE DRIVER MUST BE LAST AND THE HOST SORTS ITS PATHS ORDINALLY, so it is named relatively
    # from the repository root, where it sorts after an absolute path. That is a property of the
    # host's argument handling rather than of this script, and naming it here is cheaper than
    # discovering it from a run in which the harness had not been defined yet.
    command = [str(binary)] + files + [
        DRIVER, "--fuel", str(fuel), "--wall", str(wall), "--live-bytes", str(live_bytes),
        "--max-depth", str(max_depth), "--quiet"]

    # WHAT THE BENCHMARK COST, WHICH THIS SCRIPT DID NOT REPORT AND SHOULD HAVE. The `--wall`
    # above is an allowance a caller states in milliseconds, and a caller with no per-benchmark
    # duration in front of them has nothing to state it from: the lane's first bound was ten
    # minutes, chosen from nothing, and `zlib` walked through it on a hosted runner while
    # reporting a score on a workstation. A run that says how long each benchmark took turns the
    # next bound into a reading. It is a DURATION and not a score - roadmap section 17 governs a
    # figure a document retains, and this script retains none - so it is printed beside the exit
    # code where a score never goes.
    started = time.monotonic()
    done = subprocess.run(command, cwd=str(ROOT), capture_output=True, text=True)
    return done.returncode, (done.stdout + done.stderr).rstrip(), time.monotonic() - started


def host(binary):
    """What the binary says it is, asked of the binary rather than assumed of the directory."""
    identity = {"path": str(binary), "bytes": binary.stat().st_size}

    try:
        answer = subprocess.run(
            [str(binary), "--version"], capture_output=True, text=True, timeout=120)
        identity["version"] = answer.stdout.strip().splitlines()
    except (OSError, subprocess.SubprocessError) as failure:
        # A HOST THAT CANNOT SAY WHAT IT IS STILL RUNS THE WORKLOAD, and the report says so rather
        # than carrying an absent field a reader would read as an unasked question.
        identity["version"] = None
        identity["version-refused"] = str(failure)

    return identity


def report(path, fields, binary, rows, components, total, coverage, skipped, spent, arguments):
    """The machine-readable answer, with a schema and a version, written where a caller asked.

    WHAT IT CARRIES AND WHY EACH PART IS THERE: the PIN, because a score without the revision it
    scored is a number about nothing; the HOST BINARY and what it says its manifest and format
    version are, because the same source under two manifests is two workloads; the PER-BENCHMARK
    score, the ratios that suite pushed and the process duration; the AGGREGATE, computed the way
    Octane computes it; the ALLOWANCES the caller stated, since `--wall` and `--live-bytes` change
    what a benchmark is allowed to finish; and the ENVIRONMENT, because every figure in the file is
    a property of the machine that produced it.

    IT IS NOT A BASELINE AND CARRIES A FIELD SAYING SO. A file that a lane diffs is one step from a
    file a lane ratchets against, and the step in between - a predeclared register row, a control, an
    A/A lane, retained repetitions - is `JS-10`'s and has not been taken.
    """
    document = {
        "schema": REPORT_SCHEMA,
        "version": REPORT_VERSION,
        "produced-at": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "retained": False,
        "not-a-baseline":
            "Produced by one run, with no predeclared rule, no control, no A/A lane and no "
            "repetitions. Roadmap section 17 and rule L1 govern any figure a document retains; no "
            "figure in this file may be written into one.",
        "pin": {
            "suite": fields.get("suite"),
            "upstream": fields.get("upstream"),
            "revision": fields.get("revision"),
            "archive-sha256": fields.get("archive-sha256"),
            "content-sha256": fields.get("content-sha256"),
            "files": int(fields["files"]) if "files" in fields else None,
        },
        "host": host(binary),
        "allowances": {
            "fuel": arguments.fuel,
            "wall-ms-requested": arguments.wall,
            "live-bytes": arguments.live_bytes,
            "max-depth": arguments.max_depth,
        },
        "selection": {
            "attempted": [row["benchmark"] for row in rows],
            "excluded": skipped,
            "of": len(BENCHMARKS),
        },
        "benchmarks": rows,
        "aggregate": {
            "score": round(total, 4) if total is not None else None,
            "definition":
                "100 * exp(sum(ln(r)) / n) over every ratio BenchmarkSuite.scores collected, which "
                "is what BenchmarkSuite.RunSuites computes for a whole-suite run.",
            "ratios": len(components),
            "coverage": coverage,
            "whole": coverage == "whole",
            "is-octane-score": coverage == "whole" and total is not None,
        },
        "seconds": round(spent, 3),
        "environment": {
            "platform": platform.platform(),
            "machine": platform.machine(),
            "processor": platform.processor(),
            "python": platform.python_version(),
        },
    }

    written = pathlib.Path(path)

    if written.parent and not written.parent.exists():
        written.parent.mkdir(parents=True, exist_ok=True)

    # NEWLINE IS FIXED AT LF, because this file is written on three platforms and a report whose
    # bytes depend on which one wrote it cannot be diffed across them.
    with open(written, "w", encoding="utf-8", newline="\n") as handle:
        json.dump(document, handle, indent=2)
        handle.write("\n")

    return written


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))

    # `--only` NAMES A SELECTION AND MAY BE REPEATED, which it could not before. One benchmark was
    # enough while this script was a workstation's; a lane is the caller that needs a few, and the
    # alternative - a caller that can only ask for one benchmark or for all fifteen - is what put
    # 23m28s of Octane on every publish cell of a lane that runs on every push. A name that is not
    # a benchmark is refused rather than silently running nothing.
    parser.add_argument("--only", action="append", default=None, metavar="NAME")

    # `--skip` IS THE OTHER HALF, AND IT EXISTS FOR AN EXCLUSION SOMEBODY DECIDED. `--only` says
    # what a caller wants to reach; this says what a caller has decided it cannot. They are not
    # interchangeable: a selection that silently omitted a benchmark would read as a smaller
    # question answered green, whereas a skip is NAMED in the summary line below every time it is
    # used, so a run always says what it did not run. The lane's one use is `zlib` on `osx-x64`,
    # where the benchmark meets the profile's own wall-clock ceiling - see JSC-183.
    parser.add_argument("--skip", action="append", default=None, metavar="NAME")
    parser.add_argument("--fuel", type=int, default=1_000_000_000_000)
    parser.add_argument("--wall", type=int, default=3_600_000)

    # THE MEMORY ALLOWANCE IS NAMED HERE RATHER THAN LEFT TO THE PROFILE, because the profile's
    # default is sized for a program a person types and two of these benchmarks hold working sets
    # far larger than that. A run under an allowance nobody chose reports a named exhaustion after
    # a benchmark has already printed its score, which is the least useful of the outcomes: it is
    # neither a score nor an absence. The figure is an allowance a caller states, not a
    # measurement, and the profile's hard maximum still bounds it.
    parser.add_argument("--live-bytes", type=int, default=1_000_000_000)

    # THE NESTING BOUND IS AN ALLOWANCE THIS SCRIPT NOW STATES, added 2026-09-07, and it is the
    # third of these that the host defaults for a program a person types rather than for generated
    # source. `SliceParseOptions.DefaultMaximumNestingDepth` is 64 and this workload is minifier
    # output: `earley-boyer.js` nests deeper than 64 and was refused before compilation began -
    # `2103:NestingTooDeep at 3466:35419` - so a benchmark the profile can run reported no score
    # for want of a flag nobody passed. 512 is `MaximumSupportedNestingDepth`, the largest value the
    # host's own `--max-depth` reader admits, and it is stated here rather than left defaulted for
    # exactly the reason `--live-bytes` is.
    #
    # IT RECOVERS ONE BENCHMARK AND NOT SIX. The other refusals in this workload are
    # `SliceParseOptions.MaximumTreeDepth`, ten thousand, which the component declares against the
    # stack it compiles on and which no command line reaches by design. Raising this option does not
    # move that bound and this script does not pretend it does.
    parser.add_argument("--max-depth", type=int, default=512)

    # `--report` IS OPT-IN AND WRITES OUTSIDE THE DOCUMENTS. Scores have only ever existed as
    # `score <n>` lines in a transcript, which means every reader of a run has been a person and
    # every comparison between two runs has been a person's eye. A file with a schema and a version
    # is what a lane diffs and what a bundle carries. It is deliberately not written unless a caller
    # names a path: a driver that always dropped a scored file into the tree would be one revision
    # away from a scored file the tree retains, and roadmap section 17 is about exactly that step.
    parser.add_argument("--report", default=None, metavar="PATH")
    arguments = parser.parse_args()

    # A NAME THAT IS NOT A BENCHMARK IS REFUSED HERE, before the archive is even read. A lane
    # passes this selection through a shell, and a typo in one that ran what it could and said
    # nothing would report "3 of 3 benchmarks reported a score" over a selection missing the
    # fourth - a green step for a smaller question than the one it was asked.
    named = (arguments.only or []) + (arguments.skip or [])
    unknown = [name for name in named if name not in BENCHMARKS]

    if unknown:
        raise SystemExit(
            "# not a benchmark in the pin: " + ", ".join(unknown)
            + "\n# the fifteen are: " + ", ".join(BENCHMARKS))

    # A SKIP THAT WOULD LEAVE NOTHING TO RUN IS REFUSED, because a run of no benchmarks reports
    # "0 of 0 benchmarks reported a score and exited zero" and exits ZERO - a green step over a
    # question nobody asked. An exclusion is a decision about one benchmark, never a way to turn
    # the workload off.
    if arguments.skip and not [
            name for name in (arguments.only or BENCHMARKS) if name not in arguments.skip]:
        raise SystemExit("# the skips leave no benchmark to run, which is not an exclusion")

    binary = pathlib.Path(arguments.binary_directory) / "Broiler.VM.Composition.JavaScript.Cli"

    # A PUBLISHED IMAGE CARRIES A SUFFIX ON ONE OF THE THREE CLAIMED PLATFORMS AND NOT ON THE OTHER
    # TWO, so the suffix is tried rather than assumed. Without this the workload lane would be
    # green on Linux and macOS and would report "no binary" on Windows - which is the shape of a
    # matrix that looks filled and is not.
    #
    # THE SUFFIX IS APPENDED AND NOT SUBSTITUTED, and the distinction is the whole of the defect
    # this line used to carry. `with_suffix` REPLACES the last dotted segment, and this assembly's
    # name is dotted five deep: it turned `Broiler.VM.Composition.JavaScript.Cli` into
    # `Broiler.VM.Composition.JavaScript.exe`, a path no publish has ever produced. So the fallback
    # never fired, on either Windows cell, and the comment above described an intent the code did
    # not carry out - which is exactly the matrix that looks filled and is not.
    if not binary.exists() and binary.with_name(binary.name + ".exe").exists():
        binary = binary.with_name(binary.name + ".exe")

    # BOTH CANDIDATES ARE NAMED WHEN NEITHER IS THERE. The message used to name one path, which is
    # what let a broken suffix fallback read as a missing publish rather than as a driver that had
    # looked in the wrong place - the reader cannot tell those apart from "no binary at <path>".
    if not binary.exists():
        raise SystemExit(
            f"# no binary at {binary}\n# and none at {binary.with_name(binary.name + '.exe')}")

    fields = pinned()
    scratch = pathlib.Path(tempfile.mkdtemp(prefix="broiler-octane-"))

    try:
        checkout = extract(fields, scratch)
        print(f"# octane {fields['upstream']} at {fields['revision']}")
        print(f"# {fields['files']} files, content {fields['content-sha256']}")
        print(f"# judging {binary}")

        # THE SKIPS ARE PRINTED BEFORE ANYTHING RUNS, and that placement is the point: a reader who
        # sees only the summary at the end still meets the exclusion at the top of the transcript,
        # and a run that excluded something never looks like a run that did not.
        skipped = [name for name in (arguments.only or BENCHMARKS) if name in (arguments.skip or [])]

        for name in skipped:
            print(f"# SKIPPED {name} - excluded by the caller, not attempted")

        wanted = [name for name in (arguments.only or BENCHMARKS) if name not in skipped]
        scored = 0
        spent = 0.0
        rows = []
        components = []

        for name in wanted:
            code, output, seconds = run(
                binary, checkout, name, arguments.fuel, arguments.wall, arguments.live_bytes,
                arguments.max_depth)
            spent += seconds
            print(f"--- {name} (exit {code}, {seconds:.0f}s)")

            for line in output.splitlines():
                print(f"    {line}")

            results, score, produced = read(output)
            # A ROW FOR A BENCHMARK THAT DID NOT SCORE CARRIES WHY, in the host's own words. A
            # machine-readable file whose failing rows are an exit code and nothing else asks its
            # reader to go and find the transcript, and the transcript is the thing the file exists
            # to replace. The detail is the LAST line the host printed, which is where its
            # complaint goes; a benchmark that exited zero carries none, because there is nothing
            # to explain.
            detail = None

            if code != 0:
                lines = [line for line in output.splitlines() if line.strip()]
                detail = lines[-1] if lines else None

            rows.append({
                "benchmark": name,
                "exit": code,
                "score": score,
                "results": results,
                "components": produced,
                "seconds": round(seconds, 3),
                "detail": detail,
            })

            # ONLY A BENCHMARK THAT EXITED ZERO CONTRIBUTES TO THE AGGREGATE, and one that did not
            # makes the aggregate refuse below rather than shrink. A geometric mean over the
            # subset that worked is a real number about a smaller run wearing the name of the
            # whole one.
            if code == 0:
                scored += 1
                components += produced

        # THE SUMMARY NAMES WHAT WAS NOT RUN, every time. `scored of len(wanted)` is a ratio over
        # what was attempted, and a ratio over a shrunken denominator reads exactly like a complete
        # run - "14 of 14" and "14 of 15" are one keystroke apart and mean different things. So the
        # excluded names are repeated here rather than left to the lines above.
        print(f"# {scored} of {len(wanted)} benchmarks reported a score and exited zero"
              + (f", {len(skipped)} excluded and not attempted: {', '.join(skipped)}"
                 if skipped else ""))
        print(f"# {spent:.0f}s over {len(wanted)} benchmarks, "
              f"under a REQUESTED wall of {arguments.wall // 1000}s each")

        # THE PER-BENCHMARK SCORES ARE REPRINTED TOGETHER, because they were fifteen numbers each
        # buried three lines deep in its own block and a reader comparing two runs had to find them
        # one at a time. The columns are the score the suite formatted, the number of ratios that
        # suite pushed, and what the process cost.
        print("#")
        print(f"# {'benchmark':<14} {'score':>8} {'ratios':>7} {'seconds':>8}")

        for row in rows:
            print(f"# {row['benchmark']:<14} {row['score'] or '-':>8} "
                  f"{len(row['components']):>7} {row['seconds']:>8.1f}")

        total = aggregate(components)
        missing = [row["benchmark"] for row in rows if row["exit"] != 0]
        coverage = whole_or_partial(wanted, skipped, missing)

        # AN AGGREGATE IS PRINTED WITH ITS COVERAGE ATTACHED, and the two words are the conformance
        # harness's rather than new ones: a run is `whole` or it is `partial|<reasons>`, and the
        # reasons are named. Octane's score is defined over the fifteen, so only a whole run may be
        # called Octane's score; a run over nine of them is a real geometric mean of a different
        # quantity, and printing it WITHOUT the denominator is how "14 of 14" and "14 of 15" become
        # the same sentence. The osx-x64 lane, which excludes `zlib` because the benchmark meets the
        # profile's own wall-clock maximum, is the run this distinction exists for.
        if total is None:
            print("# NO AGGREGATE: no benchmark in this run produced a score to take a mean over.")
        elif coverage == "whole":
            print(f"# AGGREGATE {total:.1f} - Octane's own score, "
                  f"100 x the geometric mean of {len(components)} ratios over all "
                  f"{len(BENCHMARKS)} benchmarks")
        else:
            print(f"# PARTIAL AGGREGATE {total:.1f} over {scored} of {len(BENCHMARKS)} benchmarks, "
                  f"{len(components)} ratios - THIS IS NOT OCTANE'S SCORE.")
            print(f"# coverage {coverage}")
            print("# A geometric mean over a subset is a real number about that subset and reads "
                  "exactly")
            print("# like the whole one. It may not be compared with a whole run or with another "
                  "subset.")

        if total is not None:
            print("# It is a number about this machine and this configuration. There is no "
                  "baseline register")
            print("# for it, no control and no repetition; no document may retain it "
                  "(roadmap section 17).")

        if arguments.report:
            written = report(
                arguments.report, fields, binary, rows, components, total, coverage, skipped,
                spent, arguments)
            print(f"# report {written}")

        return 0 if scored == len(wanted) else 1
    finally:
        shutil.rmtree(scratch, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
