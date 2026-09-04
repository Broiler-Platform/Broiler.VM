#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE DIFFERENTIAL PROBES, AND THE TWO DIFFERENT QUESTIONS THEY ANSWER.
#
# `src/tests/differential/` holds probes over the surface this profile admits. Each probe prints one
# numbered line per case, and each has a retained answer file beside it. This script runs both
# comparisons:
#
#   * ALWAYS, against the retained answers. That is a regression check and it needs nothing but the
#     built host, so it belongs in any lane. What it asserts is that this build answers what the
#     build that retained the file answered.
#
#   * WHEN a comparison engine is named, against that engine as well. That is the question the
#     retained file cannot answer: whether the answers are RIGHT. A retained file agrees with
#     itself by construction, and a component whose only oracle is its own previous output is
#     exactly the arrangement bundle JS-4-001 records as producing this component's claims about
#     JavaScript rather than conformance.
#
# THE DECLARED DIVERGENCES ARE DATA AND NOT A README PARAGRAPH. A probe's answer file may carry
# `#diverges <case> <reason>` lines. A case named there is expected to differ from the comparison
# engine and is reported as a declared divergence; a case NOT named there that differs is a finding,
# and the exit code says so. A divergence that has gone away is also reported, because a declaration
# nobody removed is a claim about the code that has stopped being true.
#
# THIS SCRIPT PUBLISHES NO SCORE. A count of agreeing cases is a measurement of the probe, and
# roadmap section 17 governs any figure that is ever retained.
#
#   python3 eng/run-differential.py [--binary-directory <dir>] [--against <node path>]
#                                   [--write] [--only <name>]

import argparse
import os
import pathlib
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
PROBES = ROOT / "src/tests/differential"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# The shell global the host gives a program, written for the comparison engine, which has no such
# thing. It is prepended to a COPY of the probe; the probe on disk is the one the host runs, so
# nothing here can make the two engines run different source in the part that matters.
SHIM = (
    "globalThis.print = (...a) => console.log("
    "a.map(x => typeof x === \"string\" ? x : String(x)).join(\" \"));\n"
)


def answers(path):
    """The retained answers, and the declared divergences, read from one file."""
    lines = []
    declared = {}

    if not path.exists():
        return None, declared

    for line in path.read_text(encoding="utf-8").splitlines():
        if line.startswith("#diverges "):
            _, case, reason = line.split(" ", 2)
            declared[case.strip()] = reason.strip()
            continue

        if line.startswith("#"):
            continue

        lines.append(line)

    return lines, declared


def host(binary, probe):
    """What the host answered, with its trailing completion value dropped."""
    done = subprocess.run(
        [str(binary), str(probe), "--quiet"],
        capture_output=True, text=True, encoding="utf-8", errors="replace")

    return done.returncode, (done.stdout + done.stderr).splitlines()


def comparison(engine, probe, scratch):
    """What the comparison engine answered."""
    wrapped = scratch / (probe.stem + ".wrapped.js")
    wrapped.write_text(SHIM + probe.read_text(encoding="utf-8"), encoding="utf-8")

    done = subprocess.run(
        [engine, str(wrapped)],
        capture_output=True, text=True, encoding="utf-8", errors="replace")

    return done.returncode, (done.stdout + done.stderr).splitlines()


def numbered(lines):
    """A case number to its answer. A line that does not begin with one is not a case."""
    cases = {}

    for line in lines:
        head, _, rest = line.partition(" ")

        if head.isdigit():
            cases[head] = rest

    return cases


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--against", default=None, help="a comparison engine to run the probes under")
    parser.add_argument("--write", action="store_true", help="retain what this build answered")
    parser.add_argument("--only", default=None)
    arguments = parser.parse_args()

    binary = pathlib.Path(arguments.binary_directory) / "Broiler.VM.Composition.JavaScript.Cli"

    if not binary.exists():
        raise SystemExit(f"# no binary at {binary}")

    probes = sorted(PROBES.glob("*.js"))

    if arguments.only:
        probes = [p for p in probes if p.stem == arguments.only]

    if not probes:
        raise SystemExit(f"# no probes under {PROBES}")

    scratch = pathlib.Path(os.environ.get("TMPDIR", "/tmp"))
    ok = True

    for probe in probes:
        expected = probe.with_suffix(".expected.txt")
        code, produced = host(binary, probe)
        print(f"--- {probe.name} (exit {code})")

        if code != 0:
            print(f"    the host did not answer: {' '.join(produced[-1:])}")
            ok = False
            continue

        retained, declared = answers(expected)

        if arguments.write:
            body = [
                "# The answers this build produced, retained so a later build can be compared with it.",
                "# GENERATED by eng/run-differential.py --write. A `#diverges` line is authored: it",
                "# names a case that is EXPECTED to differ from the comparison engine, and the reason.",
            ]

            body += [f"#diverges {case} {reason}" for case, reason in sorted(declared.items())]
            body += produced
            expected.write_text("\n".join(body) + "\n", encoding="utf-8")
            print(f"    retained {len(produced)} answers in {expected.name}")
            continue

        if retained is None:
            print(f"    no retained answers at {expected.name}; run with --write to make them")
            ok = False
            continue

        if retained != produced:
            ok = False
            print("    THIS BUILD DISAGREES WITH THE RETAINED ANSWERS:")

            for case, answer in sorted(numbered(produced).items(), key=lambda kv: int(kv[0])):
                was = numbered(retained).get(case)

                if was != answer:
                    print(f"      case {case}: retained `{was}`, now `{answer}`")

        if not arguments.against:
            continue

        _, other = comparison(arguments.against, probe, scratch)
        mine = numbered(produced)
        theirs = numbered(other)
        undeclared = []
        stale = []

        for case in sorted(mine, key=int):
            if case not in theirs:
                undeclared.append((case, mine[case], "(the comparison engine printed no such case)"))
                continue

            if mine[case] == theirs[case]:
                if case in declared:
                    stale.append(case)

                continue

            if case in declared:
                print(f"    case {case} diverges as declared: {declared[case]}")
                print(f"      here `{mine[case]}`, there `{theirs[case]}`")
                continue

            undeclared.append((case, mine[case], theirs[case]))

        for case, here, there in undeclared:
            ok = False
            print(f"    UNDECLARED DIVERGENCE at case {case}: here `{here}`, there `{there}`")

        for case in stale:
            ok = False
            print(
                f"    case {case} is declared as a divergence and does not diverge; "
                "the declaration is stale")

    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
