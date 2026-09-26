#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# READ EXIT-GATE CLAUSE 3'S FIRST HALF OFF THE SOURCE: every WasmOpcode member has a row of the family's
# table or a common mapping, and every byte of the table's access and numeric ranges names a member.
#
# Run from the repository root at the commit the bundle names:
#
#   python docs/evidence/ubc-4-002/opcode-coverage.py
#
# It writes opcode-coverage.log into this directory. It is a reading of three source files by pattern, not
# a check over the compiled assembly: it finds the members of `WasmOpcode`, the `case WasmOpcode.X:` labels
# of `WasmLowering.Step` (the one method that lowers an instruction), and the constants `WasmFamilyTable`
# declares for its access and numeric ranges and its named rows, and it says for each member which of them
# answers it. It reads the constants, not the table's rows, so the eight global rows at 0xE0 to 0xE7, which
# name no member by design, are outside what it checks. It is retained as a script's reading rather than
# written as a rule of the architecture suite, which could read the same files; that is not done here. Clause 3's second half - every Primitive row has a reference handler
# the E2 corpus reaches - is the harness's --primitives lane, which fails a row with no input line.

import io
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
PROFILE = os.path.join(ROOT, "src", "Broiler.VM.Profile.WebAssembly")


def read(name):
    return io.open(os.path.join(PROFILE, name), encoding="utf-8").read()


def main():
    opcode_source = read("WasmOpcode.cs")
    lowering = read("WasmLowering.cs")
    table = read("WasmFamilyTable.cs")

    members = [(name, int(value, 16)) for name, value in
               re.findall(r"^\s{4}([A-Z][A-Za-z0-9]*) = (0x[0-9A-Fa-f]{2}),", opcode_source, re.M)]

    # The body of Step: from its declaration to the next method declared at the same indentation.
    start = lowering.index("private bool Step(ref VmBoundedReader reader, byte opcode, int at)")
    end = re.compile(r"^    (?:private|internal|public) ", re.M).search(lowering, start + 1).start()
    step = lowering[start:end]
    cases = set(re.findall(r"case WasmOpcode\.([A-Za-z0-9]+):", step))

    constants = dict((name, int(value, 16)) for name, value in
                     re.findall(r"internal const byte ([A-Za-z]+) = (0x[0-9A-Fa-f]{2});", table))
    ranges = {
        "family access row": (constants["FirstAccess"], constants["LastAccess"]),
        "family numeric row": (constants["FirstNumeric"], constants["LastNumeric"]),
    }
    # The rows Step lowers by name that are the family's own rather than common rows.
    family_named = {"CallIndirect": constants["CallIndirect"], "MemorySize": constants["MemorySize"],
                    "MemoryGrow": constants["MemoryGrow"]}

    lines = ["# WasmOpcode members against WasmLowering.Step and WasmFamilyTable, read from the source", ""]
    counts = {}
    unanswered = []
    for name, value in members:
        if name in cases:
            how = "family row, lowered by name" if name in family_named else (
                "family global rows 0x%02X-0x%02X, lowered by name" % (constants["FirstGlobal"], constants["LastGlobal"])
                if name in ("GlobalGet", "GlobalSet") else "lowered by name to common rows")
        else:
            how = next((label for label, (low, high) in ranges.items() if low <= value <= high), None)
        if how is None:
            unanswered.append(name)
            how = "NO ROW AND NO COMMON MAPPING"
        counts[how] = counts.get(how, 0) + 1
        lines.append("0x%02X %-24s %s" % (value, name, how))

    by_value = dict((value, name) for name, value in members)
    extra = []
    for label, (low, high) in ranges.items():
        extra.extend("0x%02X in the %s range names no member" % (b, label) for b in range(low, high + 1)
                     if b not in by_value)
    for name, value in family_named.items():
        if by_value.get(value) != name:
            extra.append("WasmFamilyTable.%s = 0x%02X is not WasmOpcode.%s" % (name, value, name))
    stray = sorted(cases - set(name for name, _ in members))

    lines.append("")
    lines.append("# %d members" % len(members))
    for how, count in sorted(counts.items()):
        lines.append("#   %3d %s" % (count, how))
    lines.append("# members with no row and no common mapping: %d%s" % (
        len(unanswered), "".join(" " + n for n in unanswered)))
    lines.append("# family rows in the ranges naming no member, or named rows at another byte: %d%s" % (
        len(extra), "".join("\n#   " + e for e in extra)))
    lines.append("# case labels in Step naming no member: %d%s" % (len(stray), "".join(" " + s for s in stray)))
    failed = bool(unanswered or extra or stray)
    lines.append("# %s" % ("FAILED" if failed else
                            "every member is answered, and every byte of the access and numeric ranges names a member"))

    with io.open(os.path.join(HERE, "opcode-coverage.log"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("$ python docs/evidence/ubc-4-002/opcode-coverage.py\n" + "\n".join(lines) + "\n\nexit %d\n" % int(failed))
    print(lines[-1])
    return int(failed)


if __name__ == "__main__":
    sys.exit(main())
