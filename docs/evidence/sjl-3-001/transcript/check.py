#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""The transcript check of bundle sjl-3-001's revised rule (decision-rule-revised-2026-10-20.md), for to-string.js.

    python3 docs/evidence/sjl-3-001/transcript/check.py --numref <Broiler.JS scripts/conformance/numref.py> \
        --form bytecode|native --control-answer <to-string.control.js at arm B's first check> \
        --a <arm A's transcript> --answer-a <to-string.js at arm A's first check> \
        --b <arm B's transcript> --answer-b <to-string.js at arm B's first check> \
        [--tsv <out.tsv>] [--txt <out.txt>]

A transcript is what the command-line root prints when it runs to-string.transcript.js beside this file: the texts
of to-string.js's conversions, one per line, in the workload's order (a file ending in .gz is read compressed). The
check holds, for one form, if and only if:

  (i)   each arm's transcript has as many lines as to-string.control.js answers at arm B's first check;
  (ii)  the lengths of each arm's lines, in UTF-16 code units as the workload counts them, sum to that arm's
        to-string.js answer at its first check;
  (iii) every line of arm A's transcript is one of the texts the specification admits for the value and the
        conversion that line converts, as Broiler.JS's scripts/conformance/numref.py computes them at commit
        966140ad694570467ad21975f23047d0d6f0c1ce (SHA-256 below; any other file is refused).

Each line's value is computed as the program computes it, in IEEE 754 double arithmetic, which Python's float is:
values[j] * (1 + i / 1024). Every line on which the two arms differ, and every line of arm A the specification does
not admit, is listed with its form, its value, its conversion, both texts and whether each is admitted.

It judges nothing beyond the check: exit 0 when the check holds, 1 when it does not, 2 when it refuses its inputs.
"""

import argparse
import collections
import gzip
import hashlib
import importlib.util
import re
import struct
import sys

NUMREF_COMMIT = "966140ad694570467ad21975f23047d0d6f0c1ce"
NUMREF_SHA256 = "424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7"

# to-string.js's values, loop bound and scale. The collector and the revision check also hold the workload's and
# the transcript program's own text to these (loop_of), so the three cannot drift apart unseen.
VALUES = (0.0, -0.0, 1.0, -1.0, 0.1, 0.5, 1 / 3, 2 / 3, 123.456, -987.654321, 1e21, 1.5e-7, 5e-324,
          1.7976931348623157e308, 9007199254740992.0, 9007199254740994.0, 123456789012345680000.0, 0.000001,
          1e-7, 255.0, 4294967295.0, 3.141592653589793, 2.718281828459045, 1e100, -1e-100)
OUTER = 400
SCALE = 1024
# The eight conversions, in the order the workload makes them for each value: (name, method, argument).
CONVERSIONS = (("String", "String", None), ("toString(2)", "toString", 2), ("toString(7)", "toString", 7),
               ("toString(16)", "toString", 16), ("toString(36)", "toString", 36), ("toFixed(4)", "toFixed", 4),
               ("toExponential(6)", "toExponential", 6), ("toPrecision(10)", "toPrecision", 10))


class Refused(Exception):
    """An input the check does not take: the reference is not the named file, or a loop is not the workload's."""


def sha256_of(path):
    with open(path, "rb") as stream:
        return hashlib.sha256(stream.read()).hexdigest()


def load_numref(path):
    """numref.py as a module, refused unless its bytes are the named file's."""
    digest = sha256_of(path)
    if digest != NUMREF_SHA256:
        raise Refused("the reference %s has SHA-256 %s, not %s (numref.py at Broiler.JS %s)"
                      % (path, digest, NUMREF_SHA256, NUMREF_COMMIT))
    spec = importlib.util.spec_from_file_location("sjl_3_001_numref", path)
    module = importlib.util.module_from_spec(spec)
    # Its dataclasses look their module up by name, and nothing is cached beside a file this check does not own.
    sys.modules[spec.name] = module
    keep, sys.dont_write_bytecode = sys.dont_write_bytecode, True
    try:
        spec.loader.exec_module(module)
    finally:
        sys.dont_write_bytecode = keep
    return module


def bits_of(value):
    return struct.unpack("<Q", struct.pack("<d", value))[0]


def loop_of(text):
    """(the values' bit patterns, the loop bound, the scale, the conversions in order) of to-string.js's loop as a
    JavaScript text spells it, so the workload, the transcript program and this checker can be compared."""
    found = re.search(r"var values = \[(.*?)\];", text, re.S)
    bound = re.search(r"for \(var i = 0; i < (\d+); i\+\+\)", text)
    scale = re.search(r"var x = values\[j\] \* \(1 \+ i / (\d+)\);", text)
    if not found or not bound or not scale:
        return None
    values = []
    for element in found.group(1).split(","):
        element = element.strip()
        if "/" in element:
            numerator, denominator = element.split("/")
            values.append(bits_of(float(numerator) / float(denominator)))
        else:
            values.append(bits_of(float(element)))
    body = text[scale.end():]
    conversions = tuple("String" if call.startswith("String") else "%s(%s)" % (method, argument)
                        for call, method, argument in re.findall(
                            r"(String\(x\)|x\.(toString|toFixed|toExponential|toPrecision)\((\d+)\))", body))
    return tuple(values), int(bound.group(1)), int(scale.group(1)), conversions


def expected_loop():
    return (tuple(bits_of(value) for value in VALUES), OUTER, SCALE, tuple(name for name, _, _ in CONVERSIONS))


def lines_of(text):
    """The transcript's lines: the program's answer split at each line feed, without the one line end the command-line
    root prints after it."""
    if text.endswith("\n"):
        text = text[:-1]
    return text.split("\n") if text else []


def read_transcript(path):
    opener = gzip.open if path.endswith(".gz") else open
    with opener(path, "rt", encoding="utf-8", newline="") as stream:
        return stream.read()


def utf16_length(text):
    return len(text.encode("utf-16-le")) // 2


class Reference:
    """The admissible texts of every line the program prints, in order, computed once."""

    def __init__(self, numref):
        self.numref = numref
        self.lines = []  # (i, j, bits, conversion index)
        for i in range(OUTER):
            for j, value in enumerate(VALUES):
                x = value * (1 + i / SCALE)
                for m in range(len(CONVERSIONS)):
                    self.lines.append((i, j, bits_of(x), m))
        self._cache = {}

    def admitted(self, bits, m):
        key = (bits, m)
        if key not in self._cache:
            numref = self.numref
            _, method, argument = CONVERSIONS[m]
            if method == "String":
                texts = numref.number_to_string(bits).values
            elif method == "toString":
                texts = numref.number_to_string(bits, argument).values
            elif method == "toFixed":
                texts = (numref.to_fixed(bits, argument),)
            elif method == "toExponential":
                texts = numref.to_exponential(bits, argument).values
            else:
                texts = (numref.to_precision(bits, argument),)
            self._cache[key] = (frozenset(texts), numref.number_to_string(bits).recommended)
        return self._cache[key]


TSV_HEADER = ("form\tline\ti\tj\tvalue-bits\tvalue\tconversion\tarm A's text\tarm B's text\tdiffer\t"
              "arm A admitted\tarm B admitted")


def check(reference, form, transcripts, control_answer, answers):
    """One form's check. transcripts and answers map "A" and "B" to text; control_answer is text. Returns (holds,
    summary lines, TSV rows without the header)."""
    lines = {arm: lines_of(transcripts[arm]) if transcripts[arm] is not None else None for arm in ("A", "B")}
    summary = ["## form %s" % form]

    def number(text):
        return int(text) if text is not None and re.fullmatch(r"-?\d+", text.strip()) else None

    control = number(control_answer)
    first = {}
    for arm in ("A", "B"):
        count = len(lines[arm]) if lines[arm] is not None else None
        first[arm] = count is not None and control is not None and count == control
    summary.append("(i)   lines: arm A %s, arm B %s; to-string.control.js at arm B's first check: %s; %s" % (
        len(lines["A"]) if lines["A"] is not None else "(no transcript)",
        len(lines["B"]) if lines["B"] is not None else "(no transcript)", control_answer,
        "holds" if first["A"] and first["B"] else "FAILS"))
    second = {}
    parts = []
    for arm in ("A", "B"):
        total = sum(utf16_length(line) for line in lines[arm]) if lines[arm] is not None else None
        answer = number(answers.get(arm))
        second[arm] = total is not None and answer is not None and total == answer
        parts.append("arm %s %s against to-string.js at arm %s's first check %s" % (
            arm, total if total is not None else "(no transcript)", arm, answers.get(arm)))
    summary.append("(ii)  sum of the lines' lengths: %s; %s" % (
        "; ".join(parts), "holds" if second["A"] and second["B"] else "FAILS"))

    rows = []
    counts = collections.OrderedDict((name, collections.Counter()) for name, _, _ in CONVERSIONS)
    a_lines = lines["A"] or []
    b_lines = lines["B"] or []
    expected = len(reference.lines)
    a_admitted_all = lines["A"] is not None and len(a_lines) == expected
    for index in range(max(expected, len(a_lines), len(b_lines))):
        a = a_lines[index] if index < len(a_lines) else None
        b = b_lines[index] if index < len(b_lines) else None
        if index < expected:
            i, j, bits, m = reference.lines[index]
            admitted, shown = reference.admitted(bits, m)
            name = CONVERSIONS[m][0]
        else:
            i = j = bits = None
            admitted, shown, name = frozenset(), "(beyond the program's lines)", "(none)"
        a_ok = a is not None and a in admitted
        b_ok = b is not None and b in admitted
        a_admitted_all = a_admitted_all and a_ok
        tally = counts.setdefault(name, collections.Counter())
        tally["lines"] += 1
        tally["A admitted" if a_ok else "A not admitted"] += 1
        differ = a != b
        if differ:
            tally["differ"] += 1
            tally["differ, B admitted" if b_ok else "differ, B not admitted"] += 1
        if differ or not a_ok:
            rows.append("\t".join([
                form, str(index + 1), "" if i is None else str(i), "" if j is None else str(j),
                "" if bits is None else "%016x" % bits, shown, name,
                "(absent)" if a is None else a, "(absent)" if b is None else b, "yes" if differ else "no",
                "yes" if a_ok else "no", "yes" if b_ok else "no"]))
    summary.append("(iii) every line of arm A is one the specification admits: %s (%d of the program's %d lines)" % (
        "holds" if a_admitted_all else "FAILS",
        sum(tally["A admitted"] for tally in counts.values()), expected))
    summary.append("conversion | lines | arm A admitted | arm A not admitted | lines the arms differ on |"
                   " of those, arm B admitted | of those, arm B not admitted")
    for name, tally in counts.items():
        summary.append("%s | %d | %d | %d | %d | %d | %d" % (
            name, tally["lines"], tally["A admitted"], tally["A not admitted"], tally["differ"],
            tally["differ, B admitted"], tally["differ, B not admitted"]))
    holds = first["A"] and first["B"] and second["A"] and second["B"] and a_admitted_all
    summary.append("transcript check, %s form: %s" % (form, "HOLDS" if holds else "DOES NOT HOLD"))
    return holds, summary, rows


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--numref", required=True)
    parser.add_argument("--form", required=True, choices=("bytecode", "native"))
    parser.add_argument("--control-answer", required=True)
    parser.add_argument("--a", required=True)
    parser.add_argument("--answer-a", required=True)
    parser.add_argument("--b", required=True)
    parser.add_argument("--answer-b", required=True)
    parser.add_argument("--tsv")
    parser.add_argument("--txt")
    arguments = parser.parse_args()
    try:
        numref = load_numref(arguments.numref)
    except Refused as refusal:
        print("transcript check: REFUSED: %s" % refusal)
        return 2
    holds, summary, rows = check(Reference(numref), arguments.form,
                                 {"A": read_transcript(arguments.a), "B": read_transcript(arguments.b)},
                                 arguments.control_answer, {"A": arguments.answer_a, "B": arguments.answer_b})
    print("\n".join(summary))
    if arguments.txt:
        with open(arguments.txt, "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(summary) + "\n")
    if arguments.tsv:
        with open(arguments.tsv, "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join([TSV_HEADER] + rows) + "\n")
    return 0 if holds else 1


if __name__ == "__main__":
    sys.exit(main())
