#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-0-001: the extraction record's scans and the ADR rules' run.
#
# Run from the repository root at the commit the bundle names, with a Release build of the solution:
#
#   python docs/evidence/ubc-0-001/collect.py
#
# It writes, into this directory:
#   correspondence-table.md   the table section of ADR 0013, copied as it stands at the commit
#   proposed-surface.txt      the proposed public surface's names, one per line, as ADR 0013 lists them
#   vocabulary-scan.log       eng/ubc-vocabulary-scan.py over the shared-part column and over the names,
#                             then two negative controls: a language identifier injected into a
#                             shared-part cell of the real record and into a copy of the names, each
#                             watched failing, the record restored byte for byte and watched passing
#   architecture-tests.log    rules E1 to E4 in detail, then the whole architecture suite's summary
# and then eng/ubc-bundle-manifest.py writes manifest.json. It judges nothing: the README says what the
# files show.

import os
import re
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/adr/0013-the-universal-bytecode-extraction-record.md"
SCAN = [sys.executable, "eng/ubc-vocabulary-scan.py"]
INJECTED = "the JsValue array of each activation"


def run(command, env=None):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, env=env)
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text)


def main():
    with open(os.path.join(ROOT, RECORD), encoding="utf-8") as handle:
        record = handle.read()

    start = record.index("## The correspondence table")
    end = record.index("## The proposed public surface")
    write("correspondence-table.md", record[start:end].rstrip("\n") + "\n")

    surface = record[end:record.index("## The resulting graph edges")]
    listed = surface[:surface.index("**What the list leaves out")]
    names = []
    for match in re.finditer(r"`([A-Za-z][A-Za-z0-9]*)`", listed):
        if match.group(1) not in names:
            names.append(match.group(1))
    write("proposed-surface.txt", "\n".join(names) + "\n")

    log = []
    table = SCAN + ["--table", RECORD, "--heading", "The correspondence table", "--column", "Shared part"]
    code, out = run(table)
    log.append(f"$ python eng/ubc-vocabulary-scan.py --table {RECORD} --heading \"The correspondence table\" --column \"Shared part\"\n{out}exit {code}\n")
    surface_path = "docs/evidence/ubc-0-001/proposed-surface.txt"
    code, out = run(SCAN + ["--identifiers", surface_path])
    log.append(f"$ python eng/ubc-vocabulary-scan.py --identifiers {surface_path}\n{out}exit {code}\n")

    # Negative control 1: a language identifier in a shared-part cell of the real record.
    original = open(os.path.join(ROOT, RECORD), "rb").read()
    row_c = record.index("| c | The dispatch loop |")
    cell = record.index("A verified unit is executed by a loop", row_c)
    injected = record[:cell] + INJECTED + ", and " + record[cell:]
    with open(os.path.join(ROOT, RECORD), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(injected)
    try:
        code, out = run(table)
        log.append(f"--- negative control 1: '{INJECTED}' injected into row c's shared-part cell of the record\n{out}exit {code} (a match must fail the scan)\n")
    finally:
        with open(os.path.join(ROOT, RECORD), "wb") as handle:
            handle.write(original)
    restored = open(os.path.join(ROOT, RECORD), "rb").read() == original
    code, out = run(table)
    log.append(f"--- after revert: record restored byte for byte: {restored}\n{out}exit {code}\n")

    # Negative control 2: a language's type name among the surface's names, in a copy.
    copy = os.path.join(HERE, "proposed-surface.injected.tmp")
    with open(copy, "w", encoding="utf-8", newline="\n") as handle:
        handle.write("\n".join(names + ["WasmModule"]) + "\n")
    try:
        code, out = run(SCAN + ["--identifiers", os.path.relpath(copy, ROOT).replace(os.sep, "/")])
        log.append(f"--- negative control 2: 'WasmModule' added to a copy of the names\n{out}exit {code} (a match must fail the scan)\n")
    finally:
        os.remove(copy)
    write("vocabulary-scan.log", "\n".join(log))

    env = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en")
    tests = ["dotnet", "test", "src/tests/Broiler.VM.Architecture.Tests", "-c", "Release", "--no-build", "--nologo"]
    code1, out1 = run(tests + ["--filter", "FullyQualifiedName~CoreContractVersionTests", "--logger", "console;verbosity=normal"], env)
    code2, out2 = run(tests, env)
    lines = [line for line in out2.split("\n") if line.startswith(("Passed!", "Failed!"))]
    write("architecture-tests.log",
          "$ dotnet test src/tests/Broiler.VM.Architecture.Tests -c Release --no-build --nologo "
          "--filter FullyQualifiedName~CoreContractVersionTests --logger \"console;verbosity=normal\"\n"
          + out1 + f"exit {code1}\n\n$ dotnet test src/tests/Broiler.VM.Architecture.Tests -c Release --no-build --nologo\n"
          + "\n".join(lines) + f"\nexit {code2}\n")
    print("collected; now write README.md, then run eng/ubc-bundle-manifest.py")
    return 0


if __name__ == "__main__":
    sys.exit(main())
