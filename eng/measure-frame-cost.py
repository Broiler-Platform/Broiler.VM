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
# So the measurement is made from OUTSIDE, by bisection over the published binary, one child process
# per trial, with the question "did it answer, or did it die".
#
# THERE ARE TWO DEPTHS AND THEY ARE NOT THE SAME NUMBER. That is the finding this script exists to
# keep visible:
#
#   * how deep a recursion can go and RETURN; and
#   * how deep a recursion can go and THROW, with the exception unwinding to a handler above it.
#
# The second was an eighth of the first until 2026-09-04. A frame with a `catch` that rethrows is
# entered during the runtime's second pass, so a throw crossing a thousand interpreter frames
# accumulated a thousand funclets and their dispatchers and the process died - on a stack that holds
# eight thousand ordinary calls. The executor catches by FILTER now, which runs in the first pass and
# does not unwind per frame, and the two depths agree *(JSC-97)*. A build where they diverge again
# has the same defect back, and this script is how that is noticed.
#
# WHAT THE NUMBERS ARE NOT. They are bounded ABOVE by the engine's own `MaximumCallDepth`, which
# answers with a catchable `RangeError` rather than dying, and by the profile's declared call-depth
# maximum, which the core holds a caller to. So what this reports is the smaller of the real capacity
# and the declared bound - which is the right thing to report for a released build, and is NOT a
# measurement of the stack. Measuring the raw capacity means lifting both bounds in a build of your
# own; the figures that arrangement produced on 2026-09-04 are recorded in `JsEngine.MaximumCallDepth`
# beside the bound derived from them.
#
#   python3 eng/measure-frame-cost.py [--binary-directory <dir>] [--stack-bytes <n>] [--ceiling <n>]

import argparse
import pathlib
import subprocess
import sys
import tempfile

ROOT = pathlib.Path(__file__).resolve().parent.parent
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# The stack `JsExecution.GuestStackBytes` declares for one guest invocation. It is stated here
# rather than read, because a script that read it from the source would agree with the source by
# construction and would stop being able to notice the two disagreeing.
DEFAULT_STACK_BYTES = 96 * 1024 * 1024

RETURNING = """function down(n) { return n === 0 ? 0 : down(n - 1); }
print("answered " + down(%d));
"""

# THE THROWING SHAPE HAS TO TELL ITS OWN EXCEPTION FROM THE BOUND'S. Both arrive at the same
# `catch`, and a fixture that printed either as an answer would report that every depth completes -
# which is what the engine's bound firing looks like from inside the program.
THROWING = """function down(n) { if (n === 0) { throw new Error("here"); } down(n - 1); }
try { down(%d); print("bounded no-throw"); }
catch (failure) {
  print(failure.message === "here" ? "answered " + failure.name : "bounded " + failure.name);
}
"""


# THE THREE OUTCOMES, AND WHY THEY ARE THREE RATHER THAN TWO. A run that COMPLETED reached the
# depth it was asked for; a run the engine's bound or the host's ceiling REFUSED did not reach it and
# is not a failure; a run that DIED is the defect this whole exercise exists to keep out. Folding
# the middle one into either of the others is what made an earlier version of this script report a
# per-frame cost derived from a bound rather than from the stack.
COMPLETED = "completed"
BOUNDED = "bounded"
DIED = "died"


def outcome(binary, scratch, shape, depth, ceiling, timeout):
    """What the host did at this recursion depth."""
    source = scratch / "depth.js"
    source.write_text(shape % depth, encoding="utf-8")

    try:
        done = subprocess.run(
            [
                str(binary), str(source), "--quiet",
                "--call-depth", str(ceiling),
                "--fuel", "100000000000",
                "--wall", "600000",
            ],
            capture_output=True, text=True, timeout=timeout,
        )
    except subprocess.TimeoutExpired:
        return DIED, "timed out"

    both = done.stdout + done.stderr

    if both.startswith("answered"):
        return COMPLETED, "answered"

    if both.startswith("bounded"):
        return BOUNDED, both.strip().splitlines()[0]

    if "Maximum call stack size exceeded" in both:
        return BOUNDED, "the engine's own bound"

    if "CeilingReached on CallDepth" in both:
        return BOUNDED, "the budget ceiling"

    if "Stack overflow" in both or done.returncode < 0:
        return DIED, "the process terminated"

    return DIED, f"exit {done.returncode}: {both.strip().splitlines()[-1] if both.strip() else ''}"


def deepest(binary, scratch, shape, ceiling, timeout, label):
    """The deepest recursion of this shape that COMPLETES, and what stopped it going deeper."""
    low, high = 1, ceiling
    verdict, why = outcome(binary, scratch, shape, low, ceiling, timeout)

    if verdict != COMPLETED:
        print(f"# {label}: the shallowest recursion did not complete: {why}", file=sys.stderr)
        return None, verdict

    verdict, why = outcome(binary, scratch, shape, high, ceiling, timeout)

    if verdict == COMPLETED:
        print(f"# {label}: every depth up to {high} completed")
        return high, COMPLETED

    # INVARIANT: `low` completed and `high` did not. Every step keeps it, so the loop ends with
    # `low` the deepest recursion that completes and `high` the shallowest that does not.
    stopped = verdict

    while high - low > 1:
        middle = (low + high) // 2
        verdict, why = outcome(binary, scratch, shape, middle, ceiling, timeout)
        print(f"#   {label} {middle}: {verdict} ({why})")

        if verdict == COMPLETED:
            low = middle
        else:
            high = middle
            stopped = verdict

    return low, stopped


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
    print(f"# declared guest stack {arguments.stack_bytes} bytes")

    with tempfile.TemporaryDirectory(prefix="broiler-depth-") as directory:
        scratch = pathlib.Path(directory)

        returning, why_returning = deepest(
            binary, scratch, RETURNING, arguments.ceiling, arguments.timeout, "returning")

        throwing, why_throwing = deepest(
            binary, scratch, THROWING, arguments.ceiling, arguments.timeout, "throwing")

    if returning is None or throwing is None:
        return 1

    print(f"deepest-returning-recursion {returning}")
    print(f"stopped-by-returning {why_returning}")
    print(f"deepest-throwing-recursion {throwing}")
    print(f"stopped-by-throwing {why_throwing}")
    print(f"declared-guest-stack-bytes {arguments.stack_bytes}")

    if DIED in (why_returning, why_throwing):
        print("# A RECURSION TERMINATED THE PROCESS, which is the outcome this bound exists against")
        return 1

    if why_returning == BOUNDED and why_throwing == BOUNDED:
        print(
            "# both were stopped by a declared bound and not by the stack, so this run reports what\n"
            "# the build PROMISES rather than what the stack holds. Lift `MaximumCallDepth` and the\n"
            "# profile's declared call-depth maximum in a build of your own to measure the capacity.")

        return 0

    print(f"bytes-per-frame {arguments.stack_bytes / returning:.0f}")

    # THE TWO MUST AGREE, or a throw is costing stack a call is not. Reporting it rather than
    # asserting it is deliberate: this script measures and the acceptance table judges.
    if abs(returning - throwing) > max(64, returning // 20):
        print("# THE TWO DEPTHS DISAGREE, so an exception is costing stack a call is not")
        return 1

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
