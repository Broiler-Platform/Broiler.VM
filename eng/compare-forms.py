#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COMPARE THE TWO OUTPUT FORMS OVER THE SAME SOURCE, AND WRITE NOTHING DOWN.
#
# ------------------------------------------------------------------------------------------------
# WHAT THIS IS NOT, FIRST, BECAUSE IT IS THE PART A READER WILL GET WRONG
# ------------------------------------------------------------------------------------------------
#
# **This script produces no baseline and no figure that any document in this repository may
# retain.** `docs/baselines.md` section 1 and rule `L1` say what a retained figure is: a
# predeclared measurement in a register, a control that differs from the candidate in exactly one
# thing, an A/A lane that shows the machine's noise floor, every repetition retained, and a
# condition checked before and after every lane. This harness has the second, the third, the
# fourth and the fifth. It deliberately does NOT have the first, and the first is the one that
# matters: **there is no register row for either output form**, because minting one is a
# bundle-collection act that `docs/mvp.md` defers until a bundle exists and a human has read it.
#
# So the discipline is here and the register is not, and that is the intended state. The harness
# prints; the repository records nothing. A figure from this script that appears in a roadmap, a
# ledger, a support table or a README is a defect in the commit that put it there, not a finding
# this script made. `VM-7` states the same rule from the other side - "No claim about speed" - and
# adds that any figure about what a native form buys belongs to the profile that emits it,
# measured against that profile's own baseline. That baseline does not exist yet.
#
# ------------------------------------------------------------------------------------------------
# THE PREDECLARED RULE, STATED BEFORE ANY NUMBER EXISTS
# ------------------------------------------------------------------------------------------------
#
# Declared here, in the source, so that it is fixed before a run rather than chosen after one:
#
#   1. THE QUANTITY. For one kernel and one form, the quantity is the wall-clock difference
#      between running the kernel and CHECKING it - `--check` compiles, verifies and stops. The
#      difference is what instantiating and executing cost, with process start, JIT warm-up,
#      reading the file, parsing, lowering, emitting and verifying subtracted out because they are
#      in both lanes. The raw wall clocks are printed too, because a reader is entitled to see how
#      much of the process the subtraction removed.
#
#   2. THE CONTROL. The control lane is the SAME source, under the SAME form, through the SAME
#      binary, with `--check` and nothing else changed. It is not the other form and it is not
#      another program.
#
#   3. THE A/A LANE. The bytecode form is measured a SECOND time, identically, in the same
#      interleave. The difference between those two medians is this machine's noise floor for this
#      kernel. It is not an estimate of the noise floor; it is one observation of it.
#
#   4. WHAT COUNTS AS A DIFFERENCE. A form difference is reported as a difference only when
#      |median(bytecode) - median(native)| is GREATER than |median(bytecode) - median(bytecode
#      again)|. When it is not, the harness prints `below the noise floor` and reports no ratio.
#      A ratio is printed only beside a difference that cleared the floor.
#
#   5. REPETITIONS AND WARM-UP. Three warm-up repetitions, discarded and named as discarded, then
#      seven retained - both counts are `--warmup` and `--repetitions`, and both are printed at the
#      top of a run so a transcript says which it was. Every retained repetition is printed. There
#      is no outlier policy, no mean and no statistical model; the reported figure is the median of
#      the retained repetitions, and the spread between them is most of what a single figure hides.
#
#   6. INTERLEAVED LANES. All six processes of one repetition - two forms times run and check,
#      plus the A/A pair - run inside that repetition in a fixed order rather than as blocks. A
#      machine that gets slower during a run then slows every lane by about the same amount.
#
#   7. THE CONDITION, CHECKED BEFORE ANY TIMING AND AGAIN AFTER ALL OF IT. Both forms must exit
#      zero and print the SAME answer for every kernel. A faster wrong answer is the failure this
#      exercise is most exposed to, so equivalence is not a result of the run: it is the gate the
#      run does not start without, and it is re-checked at the end in case a lane changed
#      something. A kernel whose forms disagree stops the whole harness; it is not excluded and
#      timed around.
#
# ------------------------------------------------------------------------------------------------
# WHAT IS NOT CONTROLLED FOR, NAMED RATHER THAN LEFT FOR A READER TO FIND
# ------------------------------------------------------------------------------------------------
#
#   * THE TWO FORMS ARE NOT CHARGED ALIKE. Both are metered - a native run under `--fuel 1000` is
#     refused with `AllowanceExhausted on Fuel` exactly as a bytecode run is - but they are not
#     charged at the same GRANULARITY. Measured on 2026-09-07: `loop-integer.js` with its literal
#     raised from one million to three million exhausts the host's DEFAULT fuel under the bytecode
#     form and finishes under the native one, from the same source in the same process shape. So the
#     allowance is stated explicitly on both lanes, generously, and every lane must exit zero;
#     but the metering is part of what is being compared and cannot be subtracted out.
#
#   * THE MEASUREMENT IS A PROCESS. There is no in-process timer in either lane, so scheduling,
#     page faults and the operating system's process creation are inside every figure. The control
#     removes what is common; it does not remove variance.
#
#   * ONE MACHINE, ONE RUN, ONE BUILD. Nothing here is a claim about another machine, another
#     runtime configuration, or a published Native AOT image. The bench host under `src/tests`
#     turns its GC and tiered compilation off for exactly this reason; this harness drives the
#     ordinary end-user CLI as a user would get it, and says so.
#
#   * ONLY ONE NATIVE BACKEND CAN BE A LANE. `arm64-aapcs64` is emitting-only: it compiles and
#     refuses to instantiate, so it cannot be run and is not timed. Which x86-64 convention is
#     available is the host's, not this script's - the harness asks the binary and uses the one
#     that runs.
#
# ------------------------------------------------------------------------------------------------
# WHY OCTANE IS NOT THE WORKLOAD HERE
# ------------------------------------------------------------------------------------------------
#
# Because it cannot be compiled. `broiler.javascript.numeric` admits Number values, the arithmetic
# and comparison and unary operators, `let` and `const` bindings of numbers, the ordinary
# statements, and functions of numbers called by name; it refuses everything else BY NAME at
# compile time. Every Octane benchmark is written in the whole language. This is not a matter of
# degree - there is no slower version of `richards.js` under this manifest, there is a refusal -
# and the harness demonstrates it rather than asserting it: with `--octane <checkout>` it compiles
# a named benchmark under the numeric manifest and prints the host's own complaint.
#
#   python3 eng/compare-forms.py [--binary-directory <dir>] [--kernels <dir>] [--only <stem>]...
#                                [--repetitions N] [--warmup N] [--fuel N] [--wall MS]
#                                [--backend <name>] [--octane <checkout>]

import argparse
import os
import pathlib
import platform
import statistics
import subprocess
import sys
import time
from datetime import datetime, timezone

ROOT = pathlib.Path(__file__).resolve().parent.parent
KERNELS = ROOT / "src/tests/forms"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# THE BACKENDS THIS HARNESS WILL TRY, IN ORDER, AND WHY THE LIST IS NOT THE HOST'S. The host names
# three; one of them emits and is never armed. Rather than encode which is which - a copy of
# somebody else's decision that goes stale - the harness tries each in turn on a one-line probe and
# uses the first that RUNS. A backend that refuses to instantiate is reported as emitting-only and
# is not timed.
CANDIDATE_BACKENDS = ["x86-64-win64", "x86-64-sysv", "arm64-aapcs64"]

# The Octane file the refusal demonstration compiles. `richards.js` is the suite's oldest
# benchmark, and its first statement - `var Richards = new BenchmarkSuite(...)` at line 38 - is
# already a `new` expression the numeric manifest refuses by name, which is the shortest honest
# way to show what the manifest does with the whole language.
#
# CORRECTED 2026-09-08. This comment read "`richards.js` is the suite's smallest and oldest
# benchmark and it declares a constructor in its first twenty lines". Neither half held against
# the pinned checkout: `splay.js` and `navier-stokes.js` are smaller self-contained benchmarks,
# and the file's first twenty lines are the V8 licence header, with the first constructor at line
# 99. The sentence is anchored to 38:16 instead, because that is the refusal the run prints.
OCTANE_WITNESS = "richards.js"


def binary_at(directory):
    """The CLI, with the suffix APPENDED and not substituted (this assembly's name is dotted)."""
    binary = pathlib.Path(directory) / "Broiler.VM.Composition.JavaScript.Cli"

    if not binary.exists() and binary.with_name(binary.name + ".exe").exists():
        binary = binary.with_name(binary.name + ".exe")

    if not binary.exists():
        raise SystemExit(
            f"# no binary at {binary}\n# and none at {binary.with_name(binary.name + '.exe')}")

    return binary


def invoke(binary, form, path, allowances, check):
    """One process. Returns (exit code, stdout+stderr, seconds)."""
    # `--all` AND NOT `--quiet`. The completion value IS the equivalence check, so it must be
    # printed; and a refusal that named only its first diagnostic is how a kernel that is outside
    # the manifest in three places reads as one that is outside it in one.
    command = [str(binary), "--numeric"] + form + ["--all"] + allowances

    if check:
        command.append("--check")

    command.append(str(path))
    started = time.perf_counter()
    done = subprocess.run(command, cwd=str(ROOT), capture_output=True, text=True)
    return done.returncode, (done.stdout + done.stderr).strip(), time.perf_counter() - started


def answer(binary, form, path, allowances):
    """What one form prints for one kernel, with its exit code."""
    code, output, _ = invoke(binary, form, path, allowances, check=False)
    return code, output


def backend(binary, allowances):
    """The first named backend that both EMITS and RUNS, with what the others said.

    A backend that compiles and then refuses to instantiate is emitting-only. That is a fact about
    this build on this architecture and it is reported rather than worked around: an emitting-only
    backend has no execution to time, and timing its compilation against the other form's execution
    would be a comparison between two different questions.
    """
    probe = KERNELS / "loop-integer.js"
    notes = []

    for name in CANDIDATE_BACKENDS:
        code, output = answer(binary, ["--native", name], probe, allowances)

        if code == 0:
            notes.append(f"{name}: emits and runs")
            return name, notes

        first = output.splitlines()[0] if output else "(no output)"
        notes.append(f"{name}: does not run here - {first}")

    return None, notes


def equivalent(binary, kernels, allowances, backend_name):
    """THE GATE. Both forms, every kernel, same exit code and identical printed answer.

    Nothing is timed until this returns clean, and it is run again after every timing. A kernel
    that disagrees stops the harness rather than being excluded from it, because an excluded kernel
    is exactly the one a reader would want to know about.
    """
    rows = []
    disagreements = []

    for path in kernels:
        bytecode_code, bytecode_answer = answer(binary, [], path, allowances)
        native_code, native_answer = answer(
            binary, ["--native", backend_name], path, allowances)
        agrees = (bytecode_code == native_code == 0) and bytecode_answer == native_answer
        rows.append((path.stem, bytecode_code, bytecode_answer, native_code, native_answer, agrees))

        if not agrees:
            disagreements.append(path.stem)

    return rows, disagreements


def measure(binary, kernels, allowances, backend_name, repetitions, warmup):
    """The interleaved lanes. Six processes per kernel per repetition, in a fixed order.

    THE LANE NAMES: `bytecode` and `native` are the two candidates; `bytecode-again` is the A/A
    lane, which is the bytecode lane run a second time under a different name and nothing else.
    Each has a `-check` partner that is the same command with `--check`, which is the control.
    """
    lanes = [
        ("bytecode", []),
        ("native", ["--native", backend_name]),
        ("bytecode-again", []),
    ]

    samples = {path.stem: {name: {"run": [], "check": []} for name, _ in lanes} for path in kernels}

    for repetition in range(warmup + repetitions):
        retained = repetition >= warmup
        label = "retained" if retained else "warm-up, discarded"
        print(f"# repetition {repetition + 1} of {warmup + repetitions} ({label})")

        for path in kernels:
            for name, form in lanes:
                run_code, run_output, run_seconds = invoke(
                    binary, form, path, allowances, check=False)
                check_code, check_output, check_seconds = invoke(
                    binary, form, path, allowances, check=True)

                # THE CONDITION, CHECKED INSIDE THE TIMED LOOP AND NOT ONLY AROUND IT. A lane whose
                # operation quietly started failing is the most dangerous output a harness can
                # produce: it is fast, it is stable, and it is a number for the refusal path.
                if run_code != 0 or check_code != 0:
                    raise SystemExit(
                        f"# lane {name} on {path.stem} stopped doing what it is named for:\n"
                        f"#   run exit {run_code}: {run_output.splitlines()[:1]}\n"
                        f"#   check exit {check_code}: {check_output.splitlines()[:1]}")

                if retained:
                    samples[path.stem][name]["run"].append(run_seconds * 1000.0)
                    samples[path.stem][name]["check"].append(check_seconds * 1000.0)

    return samples


def report(samples, kernels, repetitions):
    """Print every repetition, then the medians, then the one judgement the rule admits."""
    for path in kernels:
        stem = path.stem
        print()
        print(f"## {stem}")

        medians = {}

        for name in ("bytecode", "native", "bytecode-again"):
            run = samples[stem][name]["run"]
            check = samples[stem][name]["check"]
            attributed = [r - c for r, c in zip(run, check)]
            medians[name] = statistics.median(attributed)
            print(f"   {name:<15} run ms   " + " ".join(f"{value:8.1f}" for value in run))
            print(f"   {'':<15} check ms " + " ".join(f"{value:8.1f}" for value in check))
            print(f"   {'':<15} run-check" + " ".join(f"{value:8.1f}" for value in attributed)
                  + f"   median {medians[name]:.1f}")

        floor = abs(medians["bytecode"] - medians["bytecode-again"])
        difference = abs(medians["bytecode"] - medians["native"])
        print(f"   A/A noise floor (bytecode against itself): {floor:.1f} ms")
        print(f"   bytecode against native:                   {difference:.1f} ms")

        # THE ONE JUDGEMENT. Rule 4, applied without a threshold anybody chose after seeing a
        # number: greater than the floor, or nothing is reported.
        if difference > floor:
            faster = "native" if medians["native"] < medians["bytecode"] else "bytecode"
            slower = "bytecode" if faster == "native" else "native"

            if medians[faster] > 0.0:
                print(f"   ABOVE THE FLOOR: {faster} is faster on this kernel, "
                      f"{medians[slower] / medians[faster]:.1f}x by the median of "
                      f"{repetitions}")
            else:
                print(f"   ABOVE THE FLOOR: {faster} is faster on this kernel; no ratio is "
                      "printed, because the faster median is not positive and a ratio through it "
                      "would be arithmetic rather than a measurement")
        else:
            print("   BELOW THE NOISE FLOOR: this run measured no difference between the forms "
                  "on this kernel")


def octane(binary, checkout, allowances):
    """Compile one Octane benchmark under the numeric manifest and print what the host says.

    This is the deliverable's second half stated as evidence: the reason Octane is not the workload
    for a form comparison is that the manifest refuses it, and a refusal a reader can see is worth
    more than a sentence claiming one.
    """
    witness = pathlib.Path(checkout) / OCTANE_WITNESS

    if not witness.exists():
        print(f"# no {OCTANE_WITNESS} at {checkout}; the refusal is asserted and not shown")
        return

    code, output, _ = invoke(binary, [], witness, allowances, check=True)
    print(f"# {witness}")
    print(f"# compiled under broiler.javascript.numeric, --check only, exit {code}")

    for line in output.splitlines()[:8]:
        print(f"    {line}")

    if code == 0:
        print("# IT COMPILED, WHICH CONTRADICTS THIS HARNESS'S OWN HEADER. Either the manifest has "
              "widened or the wrong file was named; the header is now the thing to correct.")
    else:
        print("# That is the answer for every one of the fifteen. There is no slower Octane under "
              "this manifest; there is a refusal.")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--kernels", default=str(KERNELS))
    parser.add_argument("--only", action="append", default=None, metavar="STEM")
    parser.add_argument("--repetitions", type=int, default=7)
    parser.add_argument("--warmup", type=int, default=3)

    # THE ALLOWANCE IS STATED AND GENEROUS ON BOTH LANES, for the reason in the header: the forms
    # are charged at different granularities, and a default that bounds one lane and not the other
    # turns a comparison into a report about the meter.
    parser.add_argument("--fuel", type=int, default=100_000_000_000)
    parser.add_argument("--wall", type=int, default=600_000)
    parser.add_argument("--backend", default=None, metavar="NAME")
    parser.add_argument("--octane", default=None, metavar="CHECKOUT")
    arguments = parser.parse_args()

    if arguments.repetitions < 1:
        raise SystemExit("# a run of no retained repetitions measures nothing")

    binary = binary_at(arguments.binary_directory)
    allowances = ["--fuel", str(arguments.fuel), "--wall", str(arguments.wall)]
    directory = pathlib.Path(arguments.kernels)
    kernels = sorted(directory.glob("*.js"))

    if arguments.only:
        wanted = set(arguments.only)
        unknown = wanted - {path.stem for path in kernels}

        if unknown:
            raise SystemExit(
                "# not a kernel in " + str(directory) + ": " + ", ".join(sorted(unknown))
                + "\n# the kernels are: " + ", ".join(path.stem for path in kernels))

        kernels = [path for path in kernels if path.stem in wanted]

    if not kernels:
        raise SystemExit(f"# no kernels under {directory}")

    print("# broiler-js output-form comparison")
    print(f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}")
    print(f"# judging {binary}")
    print(f"# {platform.platform()}, {platform.machine()}, "
          f"{os.cpu_count()} logical processors")
    print(f"# allowances: fuel {arguments.fuel}, wall {arguments.wall} ms, stated on both lanes")
    print(f"# {len(kernels)} kernels, {arguments.warmup} warm-up + "
          f"{arguments.repetitions} retained repetitions, interleaved")
    print("#")
    print("# NO FIGURE BELOW IS RETAINED BY THIS REPOSITORY. There is no baseline register row for")
    print("# either output form; minting one is a bundle-collection act docs/mvp.md defers. What")
    # CORRECTED 2026-09-08. This line read "... an A/A lane and seven repetitions -" as a string
    # literal, so a run with `--repetitions 1` printed "seven repetitions" beside its own header
    # line saying one. The count is interpolated now; the declared default is still seven.
    print(f"# this run has is a predeclared rule, a control, an A/A lane and "
          f"{arguments.repetitions} retained repetition(s) -")
    print("# what it does not have is a register, and a figure without one is printed, not kept.")
    print("#")

    if arguments.backend:
        chosen, notes = arguments.backend, [f"{arguments.backend}: named by the caller"]
    else:
        chosen, notes = backend(binary, allowances)

    for note in notes:
        print(f"# backend {note}")

    if chosen is None:
        raise SystemExit("# no named backend both emits and runs here, so there is no second lane")

    print(f"# the native lane is {chosen}")
    print("#")
    print("# EQUIVALENCE FIRST. Nothing is timed until both forms answer the same thing.")

    rows, disagreements = equivalent(binary, kernels, allowances, chosen)

    for stem, bytecode_code, bytecode_answer, native_code, native_answer, agrees in rows:
        mark = "same" if agrees else "DIFFERENT"
        print(f"#   {stem:<16} bytecode({bytecode_code}) {bytecode_answer!r:<24} "
              f"native({native_code}) {native_answer!r:<24} {mark}")

    if disagreements:
        raise SystemExit(
            "# the forms disagree on: " + ", ".join(disagreements)
            + "\n# NOTHING IS TIMED. A faster wrong answer is the failure this harness exists to "
              "refuse, and an excluded kernel is the one a reader most needs.")

    print("#")
    samples = measure(
        binary, kernels, allowances, chosen, arguments.repetitions, arguments.warmup)
    report(samples, kernels, arguments.repetitions)

    print()
    print("# EQUIVALENCE AGAIN, after all the timing, in case a lane changed something.")
    rows, disagreements = equivalent(binary, kernels, allowances, chosen)

    if disagreements:
        raise SystemExit(
            "# the forms now disagree on: " + ", ".join(disagreements)
            + "\n# every figure printed above is void.")

    print(f"#   {len(rows)} kernels, both forms, same answer, same exit code")

    if arguments.octane:
        print()
        print("# WHY OCTANE IS NOT THE WORKLOAD FOR THIS COMPARISON")
        octane(binary, arguments.octane, allowances)

    print()
    print("# Nothing above was written to a file, a register, a ledger or a roadmap, and nothing")
    print("# above may be. Rule L1 governs a retained figure and there is no row for either form.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
