#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# WHAT THIS MEASURES, AND WHY IT IS A SCRIPT RATHER THAN A TEST.
#
# Roadmap section 8 says the call-depth bound is MEASURED and not chosen, and the workload roadmap's
# JSW-9 says the same thing in the other direction: the per-frame cost of this interpreter is to be
# measured rather than estimated, and the depth maximum derived from that measurement and recorded
# with it.
#
# The quantity is how much NATIVE stack one JavaScript call costs. This interpreter recurses on the
# CLR stack - one JavaScript call is `Call`, then `Invoke`, then `Execute` - so a frame is those
# three CLR frames plus whatever each holds. Nothing inside the process can read that number: a
# stack overflow is the one failure the CLR cannot turn into an exception, so a probe that asked
# "would one more frame fit" could only answer by dying.
#
# So the measurement is made from OUTSIDE, by bisection over the published binary:
#
#   * a recursion with no base case, `src/tests/cli/limits/an-unbounded-recursion.js`;
#   * the `--call-depth` ceiling raised one step at a time;
#   * for each ceiling, one child process, and the question "did it answer, or did it die".
#
# An answer is a named resource exhaustion on `CallDepth` - the outcome roadmap section 8 requires
# and which JSC-79 records the profile once failing to give. A death is a stack overflow: the .NET
# runtime prints `Stack overflow.` and terminates, and no exit code a caller reads means anything
# after that. The largest ceiling that still ANSWERS is the deepest recursion this build survives,
# and the declared guest stack divided by it is the per-frame cost.
#
# WHAT THE NUMBER IS NOT. It is not a per-frame cost for a different program: a frame's size depends
# on the operand stack the verifier computed for the unit, so a function with a wider expression
# costs more. The recursion here is the SMALLEST frame this interpreter has, so the figure is a
# LOWER bound on the cost and therefore an UPPER bound on the safe depth. A bound derived from it
# must leave margin, and the margin belongs in the record beside the figure rather than in a
# reader's head.
#
#   python3 eng/measure-frame-cost.py [--binary-directory <dir>] [--stack-bytes <n>] [--ceiling <n>]

import argparse
import os
import pathlib
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)
FIXTURE = "limits/an-unbounded-recursion.js"
CASES = ROOT / "src/tests/cli"

# The stack `JsExecution.GuestStackBytes` declares for one guest invocation. It is stated here
# rather than read, because a script that read it from the source would agree with the source by
# construction and would stop being able to notice the two disagreeing.
DEFAULT_STACK_BYTES = 16 * 1024 * 1024


def answered(binary, ceiling, timeout):
    """Whether the host ANSWERED at this ceiling, rather than dying."""
    try:
        done = subprocess.run(
            [str(binary), FIXTURE, "--call-depth", str(ceiling)],
            cwd=str(CASES),
            capture_output=True,
            text=True,
            timeout=timeout,
        )
    except subprocess.TimeoutExpired:
        return False, "timed out"

    both = done.stdout + done.stderr

    if "CeilingReached on CallDepth" in both:
        return True, "named exhaustion"

    if "Maximum call stack size exceeded" in both:
        return True, "the engine's own backstop"

    if "Stack overflow" in both or done.returncode < 0:
        return False, "the process terminated"

    return False, f"exit {done.returncode}: {both.strip().splitlines()[-1] if both.strip() else ''}"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--stack-bytes", type=int, default=DEFAULT_STACK_BYTES)
    parser.add_argument("--ceiling", type=int, default=100000)
    parser.add_argument("--timeout", type=int, default=300)
    arguments = parser.parse_args()

    binary = pathlib.Path(arguments.binary_directory) / "Broiler.VM.Composition.JavaScript.Cli"

    if not binary.exists():
        print(f"# no binary at {binary}", file=sys.stderr)
        return 2

    print(f"# measuring against {binary}")
    print(f"# fixture {FIXTURE}, declared guest stack {arguments.stack_bytes} bytes")

    low, high = 1, arguments.ceiling
    ok, why = answered(binary, low, arguments.timeout)

    if not ok:
        print(f"# the smallest ceiling already did not answer: {why}", file=sys.stderr)
        return 1

    ok, why = answered(binary, high, arguments.timeout)

    if ok:
        print(f"# every ceiling up to {high} answered ({why}); nothing to bisect")
        print(f"deepest-answering-ceiling {high}")
        return 0

    # INVARIANT: `low` answered and `high` did not. Every step keeps it, so the loop ends with
    # `low` the deepest ceiling that answers and `high` the shallowest that does not.
    while high - low > 1:
        middle = (low + high) // 2
        ok, why = answered(binary, middle, arguments.timeout)
        print(f"#   {middle}: {'answered' if ok else 'died'} ({why})")

        if ok:
            low = middle
        else:
            high = middle

    per_frame = arguments.stack_bytes / low

    print(f"deepest-answering-ceiling {low}")
    print(f"shallowest-dying-ceiling {high}")
    print(f"declared-guest-stack-bytes {arguments.stack_bytes}")
    print(f"bytes-per-frame {per_frame:.0f}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
