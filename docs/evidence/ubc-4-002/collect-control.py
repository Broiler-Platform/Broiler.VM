#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT THE NEGATIVE CONTROL OF UBC-4.6: obligation E2 over the WebAssembly family's rows, one run.
#
# Run from the repository root after `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-4-002/collect-control.py failing     (against the unmodified reference arms)
#   python docs/evidence/ubc-4-002/collect-control.py passing     (after the arms are corrected)
#
# It writes control-<label>.log into this directory: the commit it ran at, the WebAssembly harness root's
# `--primitives src/tests/corpus/ubc-2/primitives.txt` lane with its exit code, and nothing else. The rule
# (docs/evidence/ubc-4-001/decision-rule.md) asks for the check "retained failing with the rows named", then
# "retained passing" after the correction; this script takes each half at the commit it is run at.

import os
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness/bin/Release/net10.0/Broiler.VM.Composition.WebAssembly.Harness.dll"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", VSLANG="1033", DOTNET_NOLOGO="1")


def run(command):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, env=ENV, encoding="utf-8", errors="replace")
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def main():
    if len(sys.argv) != 2 or sys.argv[1] not in ("failing", "passing"):
        print("usage: collect-control.py failing|passing")
        return 2
    _, head = run(["git", "rev-parse", "HEAD"])
    _, dirty = run(["git", "status", "--porcelain", "--untracked-files=no"])
    command = ["dotnet", HARNESS, "--primitives", "src/tests/corpus/ubc-2/primitives.txt"]
    code, out = run(command)
    text = "# commit %s%s\n$ %s\n%s\nexit %d\n" % (
        head.strip(), " (tracked files modified)" if dirty.strip() else "", " ".join(command), out, code)
    with open(os.path.join(HERE, "control-%s.log" % sys.argv[1]), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text)
    print("wrote control-%s.log, exit %d" % (sys.argv[1], code))
    return 0


if __name__ == "__main__":
    sys.exit(main())
