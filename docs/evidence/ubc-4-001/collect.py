#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-4-001: the base run of population B, beside the predeclared rule.
#
# Run from the repository root at the base commit the bundle names, after
# `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-4-001/collect.py
#
# It writes, into this directory:
#   base-run.log         the WebAssembly harness root run once, verbose, with the retained corpus:
#                        every corpus entry, every execution check and every differential check, each
#                        on one line as the root prints it, and the root's exit code
#   base-population.txt  the same lines reduced to one row per member of population B -
#                        kind|name|verdict|answer - sorted, which is what ubc-4-002 compares against
#   corpus-integrity.log eng/wasm-corpus-integrity.py over the retained corpus, manifest shape and
#                        hashes, so the corpus the base run read is shown intact
# and then eng/ubc-bundle-manifest.py writes manifest.json. It judges nothing: the README says what
# the files show, and the decision rule beside them says what a later run is judged by.

import os
import re
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness/bin/Release/net10.0/Broiler.VM.Composition.WebAssembly.Harness.dll"
CORPUS = "src/tests/wasm/corpus"

# The retained logs are read by people who did not run them: the command line's own messages in English.
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", VSLANG="1033", DOTNET_NOLOGO="1")

# A member line as the root prints it: a verdict, a name, a colon, what it answered. A corpus entry's
# name has no colon; a check's name is its lane label, a colon, and the entry it invoked.
MEMBER = re.compile(r"^(ok  |FAIL) (.*?): (.*)$")


def run(command):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, env=ENV,
                            encoding="utf-8", errors="replace")
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def population(log):
    """One row per member of population B: the lane it belongs to, its name, its verdict and answer."""
    rows = []
    lane = "canonical"
    for line in log.splitlines():
        if line.startswith("# retained corpus"):
            lane = "corpus"
        elif line.startswith("# execution"):
            lane = "execution"
        elif line.startswith("# differential"):
            lane = "differential"
        match = MEMBER.match(line)
        if match and lane in ("corpus", "execution", "differential"):
            verdict, name, answer = match.groups()
            rows.append("%s|%s|%s|%s" % (lane, name, verdict.strip(), answer))
    return sorted(rows)


def main():
    command = ["dotnet", HARNESS, "--verbose", "--corpus", CORPUS]
    code, out = run(command)
    write("base-run.log", "$ %s\n%s\nexit %d" % (" ".join(command), out, code))

    rows = population(out)
    write("base-population.txt",
          "# population B at the base: lane|name|verdict|answer, one row per member, sorted\n" + "\n".join(rows))

    # Hashes and shape only: the mutation pass needs a replay that exits 0 on the intact corpus, and at
    # the base the root exits 1 because of the float-comparison defect the rule names.
    integrity = [sys.executable, "eng/wasm-corpus-integrity.py", "--corpus", CORPUS, "--hashes-only"]
    code, out = run(integrity)
    write("corpus-integrity.log", "$ %s\n%s\nexit %d" % (" ".join(integrity), out, code))

    print("collected; now write README.md, then run eng/ubc-bundle-manifest.py")
    return 0


if __name__ == "__main__":
    sys.exit(main())
