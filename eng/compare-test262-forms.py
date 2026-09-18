#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# HOLD A NATIVE-FORM TEST262 RUN TO THE BYTECODE RUN OF THE SAME CHECKOUT, VARIANT BY VARIANT.
#
# The baseline native form over `broiler.javascript.wide` runs every instruction through the
# interpreter's own dispatch for that instruction, so its verdicts are expected to be the bytecode
# form's verdicts. Expected is not checked, and a total is not a check at all: two runs can share a
# pass count while disagreeing about hundreds of variants in both directions. So this script reads
# the `result|` rows of two reports, joins them on (path, variant), and names every row whose verdict
# or exhausted dimension differs.
#
# EVERY DIFFERENCE IS SORTED INTO A NAMED CLASS OR IT IS A DEFECT. The classes are the costs the form
# declares, and nothing else:
#
#   (a) the native run answered `Unsupported` with the refusal the baseline emitter gives for an
#       artifact whose emitted code would exceed the format's native-code ceiling. That is a limit of
#       the form, stated by the compiler, and the bytecode run has no such limit.
#   (b) the native run spent `Fuel` - or `NestedLoadBytes` - on a GUEST-LOAD variant, one whose test
#       source contains `eval(`, `Function(` or `import(`. A guest-loaded program is verified under
#       the requesting operation's allowance and its native payload is larger, so fuel and the
#       nested-load byte count are spent at a different point. This class is admitted only with
#       `--exempt-guest-loads`, because it is an exemption and a caller should have to ask for it.
#   (c) a wall-clock exhaustion, admitted only when the two runs were not both taken at the
#       deterministic lane's allowance of 60,000 ms or more, and only in three shapes:
#         - the NATIVE run exhausted the wall where the bytecode run passed, failed, or exhausted
#           another allowance. Each native instruction costs more wall time, so a realistic wall
#           bites the native run first, and the native run gave no answer to disagree with.
#         - the BYTECODE run exhausted the wall and the native run PASSED. Where the bytecode run
#           was taken at a shorter wall than the native run, that exhaustion is a statement about
#           the shorter wall. A native failure, refusal or defect against it is NOT admitted, since
#           it can be a wrong answer only the native form gives.
#         - the BYTECODE run exhausted the wall and the native run exhausted ANOTHER allowance.
#           Neither run gave an answer, and which allowance trips first is a race between the wall
#           clock and a counter that the machine decides: two BYTECODE runs of one checkout on two
#           machines split the same variants between `WallClock` and `Fuel` (added 2026-09-15, after
#           the first CI pair of the two forms showed it and a bytecode-against-bytecode comparison
#           across a workstation and a hosted runner showed the same split with no native run at all).
#       Under the deterministic lane none is admitted, because at sixty seconds a wall-clock
#       exhaustion is not noise.
#
# Anything else - a pass that became a failure, a failure that became a pass, a refusal to
# instantiate, an internal defect - is UNCLASSIFIED and fails the comparison.
#
# WHAT IT DOES NOT DO. It ignores paths present in only one report, so two partial runs of different
# selections compare over what they share; it prints how many it ignored so a comparison over nothing
# cannot read as a clean one. It reads the manifest, form and allowance rows and prints them, and
# says when the two reports were not a bytecode run and a native run under one manifest, but it does
# not refuse: the rows are still the rows. It computes no rate and publishes no figure.
#
# EXIT CODES: 0 when every difference is in an admitted class, 1 when any is not, 2 when the input
# could not be read (argparse's own code for a bad command line).
#
#   python3 eng/compare-test262-forms.py <bytecode.report> <native.report>
#                                        [--exempt-guest-loads] [--suite <root>] [--limit <n>]

import argparse
import collections
import pathlib
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
HEADER_PREFIX = "# broiler-js-conformance test262 report "

# The allowance at or above which a wall-clock exhaustion is not an admitted difference. It is the
# harness's own default and the deterministic lane's `--wall`.
DETERMINISTIC_WALL_MS = 60_000

# The refusal text the baseline emitter gives for an artifact over the native-code ceiling.
CEILING_REFUSAL = "would exceed the format's native-code ceiling"

GUEST_LOADS = ("eval(", "Function(", "import(")
LOADING_DIMENSIONS = ("Fuel", "NestedLoadBytes")

# The bytecode verdicts against which a native wall-clock exhaustion is class (c): an answer, or an
# exhaustion of another allowance. A bytecode refusal or defect against a native run that got as far
# as running out of wall is a divergence of the two forms, not the cost of one.
WALL_REFERENCES = ("Passed", "Failed", "Exhausted")

Row = collections.namedtuple("Row", "verdict family dimension kind detail")

CLASSES = (
    ("a", "a code-size refusal of the baseline form", True),
    ("b", "fuel or nested-load bytes spent on a guest-load variant", True),
    ("c", "a wall-clock exhaustion under a realistic allowance", True),
    ("b-unexempted", "fuel or nested-load bytes spent on a guest-load variant, without --exempt-guest-loads",
     False),
    ("unclassified", "a difference in no admitted class", False),
)


def refuse(message):
    """Stops with the input-could-not-be-read code, which is not the comparison's own 1."""
    print(message, file=sys.stderr)
    raise SystemExit(2)


def read(path):
    """The identity rows and the result rows of one report, or a refusal."""
    header = {"manifest": "?", "harness": "?", "form": "bytecode", "fuel": 0, "wall": 0, "total": "?"}
    rows = {}

    if not pathlib.Path(path).is_file():
        refuse(f"# no report at {path}")

    with open(path, encoding="utf-8") as handle:
        first = handle.readline().rstrip("\n")

        if not first.startswith(HEADER_PREFIX):
            refuse(f"# {path} does not open with `{HEADER_PREFIX}...`, so it is not a test262 report")

        for number, line in enumerate(handle, start=2):
            line = line.rstrip("\n")

            if not line or line.startswith("#"):
                continue

            parts = line.split("|")
            kind = parts[0]

            if kind == "manifest" and len(parts) == 6:
                header["manifest"] = parts[1]
                header["harness"] = parts[3]
            elif kind == "form" and len(parts) == 3:
                header["form"] = f"{parts[1]} ({parts[2]})"
            elif kind == "allowance" and len(parts) == 3:
                header["fuel"] = int(parts[1])
                header["wall"] = int(parts[2])
            elif kind == "total":
                header["total"] = line
            elif kind == "result":
                # A cell never holds `|` - the harness writes it as `/` - so a row of any other arity
                # is a report this script does not understand, and it says so rather than guessing.
                if len(parts) != 9:
                    refuse(f"# {path}:{number}: a result row of {len(parts)} fields: {line}")

                rows[(parts[1], parts[2])] = Row(parts[3], parts[4], parts[5], parts[6], parts[8])

    return header, rows


def find_suite(named, required):
    """The checkout guest-load detection reads, named or found under artifacts/.

    Guest-load detection runs with or without --exempt-guest-loads, so a guest-load difference is
    named as class (b-unexempted) rather than lost among the unclassified rows. Only the exemption
    REQUIRES the checkout: without it a checkout that cannot be found is no refusal, and a
    guest-load difference is then reported unclassified, which fails the comparison all the same.
    """
    if named:
        suite = pathlib.Path(named)

        if not (suite / "test").is_dir():
            refuse(f"# {suite} has no test/ directory, so it is not a test262 checkout")

        return suite

    found = sorted(
        candidate.parent for candidate in (ROOT / "artifacts").glob("t262*/*/harness") if candidate.is_dir())

    if len(found) != 1 and not required:
        return None

    if len(found) != 1:
        refuse(
            "# --exempt-guest-loads reads each test's source and needs --suite <root>; "
            + (f"{len(found)} checkouts were found under artifacts/" if found else "none was found under artifacts/"))

    return found[0]


def guest_loads(suite, path, cache):
    """Whether this test's source reaches a guest-initiated load."""
    if path not in cache:
        try:
            text = (suite / path).read_text(encoding="utf-8", errors="replace")
        except OSError:
            # A source this script cannot read is not a guest-load variant it can vouch for.
            text = ""

        cache[path] = any(needle in text for needle in GUEST_LOADS)

    return cache[path]


def classify(reference, candidate, loads, exempt, realistic):
    """The class one differing row belongs to."""
    if candidate.verdict == "Unsupported" and (
            CEILING_REFUSAL in candidate.detail or CEILING_REFUSAL in candidate.family):
        return "a"

    if candidate.verdict == "Exhausted" and candidate.dimension in LOADING_DIMENSIONS and loads():
        return "b" if exempt else "b-unexempted"

    if realistic and is_wall(candidate) and not is_wall(reference) and reference.verdict in WALL_REFERENCES:
        # The native run ran out of wall where the bytecode run answered, or ran out of another
        # allowance: the native side gave no answer, so there is no answer to disagree with.
        return "c"

    if realistic and is_wall(reference) and candidate.verdict == "Passed":
        # The bytecode run ran out of its (shorter) wall and the native run passed. A native FAILURE
        # against a bytecode wall exhaustion is not admitted: it may be a wrong answer only the
        # native form gives, and nothing in this report can say otherwise.
        return "c"

    if realistic and is_wall(reference) and candidate.verdict == "Exhausted" and not is_wall(candidate):
        # Both runs ran out of an allowance and neither answered; the bytecode run's wall tripped
        # before its counters did and the native run's counter tripped before its wall. That order
        # is the machine's, not the form's - two bytecode runs on two machines split the same way -
        # and it is the mirror of the first shape above.
        return "c"

    return "unclassified"


def is_wall(row):
    return row.verdict == "Exhausted" and row.dimension == "WallClock"


def describe(row):
    return row.verdict + (f"/{row.dimension}" if row.dimension else "")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("reference", help="the bytecode run's merged report")
    parser.add_argument("candidate", help="the native run's merged report")
    parser.add_argument(
        "--exempt-guest-loads", action="store_true",
        help="admit class (b): fuel or nested-load bytes spent differently on a variant loading a guest program")
    parser.add_argument("--suite", default=None, help="the test262 checkout; found under artifacts/ when omitted")
    parser.add_argument(
        "--limit", type=int, default=0, help="print at most this many rows per class; counts are always whole")
    arguments = parser.parse_args()

    # A detail is the harness's text and can hold anything; a console that cannot encode it must not
    # turn a comparison into a traceback.
    sys.stdout.reconfigure(errors="replace")

    reference_header, reference = read(arguments.reference)
    candidate_header, candidate = read(arguments.candidate)
    suite = find_suite(arguments.suite, required=arguments.exempt_guest_loads)

    for label, path, header in (
            ("reference", arguments.reference, reference_header),
            ("candidate", arguments.candidate, candidate_header)):
        print(f"# {label} {path}")
        print(f"#   manifest {header['manifest']}; harness {header['harness']}; form {header['form']}; "
              f"fuel {header['fuel']}; wall {header['wall']} ms")
        print(f"#   {header['total']}")

    if reference_header["manifest"] != candidate_header["manifest"]:
        print("# NOTE the two reports were taken under different manifests, so their verdicts answer two "
              "questions and most differences below are that and not the form")

    if not reference_header["form"].startswith("bytecode") or not candidate_header["form"].startswith("native"):
        print("# NOTE this is not a bytecode reference against a native candidate; the classes below "
              "describe that comparison")

    realistic = min(reference_header["wall"], candidate_header["wall"]) < DETERMINISTIC_WALL_MS
    print(f"# reading: {'realistic' if realistic else 'deterministic'} - class (c) is "
          + ("admitted, because a run was taken below " if realistic else "not admitted, because both runs were "
             "taken at or above ")
          + f"{DETERMINISTIC_WALL_MS} ms; class (b) is "
          + ("admitted" if arguments.exempt_guest_loads else "not admitted without --exempt-guest-loads"))

    shared = [key for key in reference if key in candidate]
    only = (len(reference) - len(shared)) + (len(candidate) - len(shared))
    cache = {}
    groups = collections.defaultdict(list)

    for key in sorted(shared):
        before, after = reference[key], candidate[key]

        if before.verdict == after.verdict and before.dimension == after.dimension:
            continue

        loads = (lambda path=key[0]: suite is not None and guest_loads(suite, path, cache))
        groups[classify(before, after, loads, arguments.exempt_guest_loads, realistic)].append((key, before, after))

    differing = sum(len(rows) for rows in groups.values())
    print(f"# {len(shared)} variants in both reports, {differing} differ; "
          f"{only} variant(s) present in only one report were ignored")

    if not shared:
        print("# NOTE the two reports share no variant, so this comparison compared nothing")

    for name, title, admitted in CLASSES:
        rows = groups.get(name, [])

        if not rows:
            continue

        print(f"\n## ({name}) {title}: {len(rows)} - {'admitted' if admitted else 'NOT ADMITTED'}")
        shown = rows if arguments.limit <= 0 else rows[:arguments.limit]

        for (path, variant), before, after in shown:
            note = after.kind or after.family or after.detail
            print(f"  {path} [{variant}]: {describe(before)} -> {describe(after)}"
                  + (f"  {note[:200]}" if note else ""))

        if len(shown) < len(rows):
            print(f"  ... {len(rows) - len(shown)} more")

    refused = sum(len(groups.get(name, [])) for name, _, admitted in CLASSES if not admitted)
    print()
    print(f"# {refused} difference(s) in no admitted class" if refused
          else "# every difference is in an admitted class")

    return 1 if refused else 0


if __name__ == "__main__":
    sys.exit(main())
