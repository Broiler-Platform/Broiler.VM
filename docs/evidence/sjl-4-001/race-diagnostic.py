#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""A diagnostic, not a step of the rule: the two native (c') rows whose re-runs disagree, run again.

    python3 docs/evidence/sjl-4-001/race-diagnostic.py --base-tree <base checkout> --tree <after checkout> \
        --suite <the pinned test262> --repetitions 10 > docs/evidence/sjl-4-001/after/race-diagnostic.tsv

Part A's re-run of class C4 (c') is one run alone at each commit, and decision-rule.md reads its
result; this script decides nothing. It runs the paths of the two native rows whose re-runs gave
different exhaustion dimensions, alone, the given number of times at each commit, alternating which
commit goes first, with the conformance roots the re-run used (built in each checkout), the wide
manifest, the native form and part A's fuel and wall allowance, and prints each row's verdict per
repetition, so that a reader can see whether either commit's dimension is stable.
"""

import argparse
import os
import subprocess
import tempfile

PATHS = ["test/built-ins/decodeURI/S15.1.3.1_A2.5_T1.js", "test/staging/sm/Date/dst-offset-caching-6-of-8.js"]
ROWS = [("test/built-ins/decodeURI/S15.1.3.1_A2.5_T1.js", "strict"),
        ("test/staging/sm/Date/dst-offset-caching-6-of-8.js", "strict")]
ROOT = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance", "bin", "Release", "net10.0",
                    "Broiler.VM.Composition.JavaScript.Conformance")
PIN = os.path.join("src", "tests", "conformance", "pins", "test262.pin")


def run(tree, suite):
    with tempfile.TemporaryDirectory() as scratch:
        report = os.path.join(scratch, "report")
        command = [os.path.join(tree, ROOT), "--test262", suite, "--manifest", "broiler.javascript.wide",
                   "--expect", os.path.join(tree, PIN), "--report", report, "--fuel", "100000000", "--wall", "5000",
                   "--form", "native"]
        for path in PATHS:
            command += ["--test", path]
        subprocess.run(command, cwd=tree, capture_output=True)
        verdicts = {}
        for line in open(report, encoding="utf-8"):
            if line.startswith("result|"):
                parts = line.rstrip("\n").split("|")
                verdicts[(parts[1], parts[2])] = parts[3] + ("/" + parts[5] if parts[3] == "Exhausted" else "")
        return verdicts


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base-tree", required=True)
    parser.add_argument("--tree", required=True)
    parser.add_argument("--suite", required=True)
    parser.add_argument("--repetitions", type=int, default=10)
    arguments = parser.parse_args()
    print("repetition\tside\torder\t" + "\t".join("%s [%s]" % row for row in ROWS))
    for repetition in range(1, arguments.repetitions + 1):
        sides = [("base", arguments.base_tree), ("after", arguments.tree)]
        if repetition % 2 == 0:
            sides.reverse()
        for order, (side, tree) in enumerate(sides, 1):
            verdicts = run(os.path.abspath(tree), os.path.abspath(arguments.suite))
            print("%d\t%s\t%d\t%s" % (repetition, side, order, "\t".join(verdicts.get(row, "(absent)") for row in ROWS)),
                  flush=True)


if __name__ == "__main__":
    main()
