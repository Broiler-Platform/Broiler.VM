#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# SCAN TEXT FOR THE UNIVERSAL BYTECODE'S BANNED VOCABULARY.
#
# The universal bytecode programme's first milestone (UBC-0, docs/universal-bytecode.roadmap.md
# section 5, exit gate clause 2) requires that the correspondence table in the extraction record
# names the shared part of every mechanism "without a language identifier", checked by the same
# banned-vocabulary scan rule U2 will use. This script is that scan. It reads the vocabulary from
# docs/ubc/banned-vocabulary.txt, which rule U2 reads too, so the record and the rule cannot hold two
# different lists.
#
# THREE WAYS TO POINT IT:
#
#   --table FILE --heading TEXT --column NAME
#       the first Markdown table after the first heading whose text contains TEXT; every cell of the
#       column whose header is NAME is scanned, one cell per row. A heading, table or column that is
#       not found, and a column with no non-empty cell, is an input error: a scan of nothing would
#       pass, and a passing scan of nothing is the one answer this script must never give.
#   --text FILE
#       every line of FILE, as prose.
#   --identifiers FILE
#       one identifier per line; each is split at its case boundaries before the word terms are
#       matched, which is how rule U2 reads an exported name.
#
# Exit codes: 0 when nothing matched, 1 when anything matched, 2 when the input could not be read.
# The report names every scanned item and every match, so a retained log is its own evidence.

import argparse
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_VOCABULARY = os.path.join(ROOT, "docs", "ubc", "banned-vocabulary.txt")
KINDS = ("word", "substring", "prefix")


def fail(message):
    print(f"ubc-vocabulary-scan: {message}", file=sys.stderr)
    sys.exit(2)


def read_vocabulary(path):
    terms = []
    with open(path, encoding="utf-8") as handle:
        for number, line in enumerate(handle, start=1):
            line = line.rstrip("\n")
            if not line.strip() or line.lstrip().startswith("#"):
                continue
            parts = line.split("|")
            if len(parts) != 3 or parts[1] not in KINDS or not parts[0]:
                fail(f"{path}:{number}: a vocabulary row is term|kind|why with kind one of {KINDS}")
            terms.append((parts[0].lower(), parts[1]))
    if not terms:
        fail(f"{path}: the vocabulary is empty, so every scan would pass")
    return terms


SEGMENT = re.compile(r"[A-Z]+(?=[A-Z][a-z])|[A-Z]?[a-z]+|[A-Z]+|[0-9]+")
WORD = re.compile(r"[A-Za-z0-9]+")


def segments(text):
    for word in WORD.findall(text):
        for segment in SEGMENT.findall(word):
            yield segment.lower()


def matches(text, terms):
    found = []
    lowered = text.lower()
    words = set(segments(text))
    for term, kind in terms:
        if kind == "word" and term in words:
            found.append(f"{term} ({kind})")
        elif kind == "substring" and term in lowered:
            found.append(f"{term} ({kind})")
        elif kind == "prefix" and re.search(r"(?<![A-Za-z0-9])" + re.escape(term) + r"[A-Za-z0-9]", lowered):
            found.append(f"{term} ({kind})")
    return found


def split_row(line):
    cells = re.split(r"(?<!\\)\|", line.strip())
    if cells and cells[0] == "":
        cells = cells[1:]
    if cells and cells[-1] == "":
        cells = cells[:-1]
    return [cell.strip() for cell in cells]


def table_column(path, heading, column):
    with open(path, encoding="utf-8") as handle:
        lines = handle.read().split("\n")
    start = None
    for index, line in enumerate(lines):
        if line.startswith("#") and heading in line:
            start = index
            break
    if start is None:
        fail(f"{path}: no heading contains {heading!r}")
    index = start + 1
    while index < len(lines) and not lines[index].startswith("|"):
        if lines[index].startswith("#"):
            fail(f"{path}: the heading {heading!r} is followed by another heading before any table")
        index += 1
    if index >= len(lines):
        fail(f"{path}: no table follows the heading {heading!r}")
    header = split_row(lines[index])
    if column not in header:
        fail(f"{path}: the table under {heading!r} has no column {column!r}; it has {header}")
    position = header.index(column)
    cells = []
    index += 2
    while index < len(lines) and lines[index].startswith("|"):
        row = split_row(lines[index])
        first = row[0] if row else ""
        cell = row[position] if position < len(row) else ""
        cells.append((first, cell))
        index += 1
    if not [cell for _, cell in cells if cell]:
        fail(f"{path}: the column {column!r} under {heading!r} has no non-empty cell")
    return cells


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--vocabulary", default=DEFAULT_VOCABULARY)
    parser.add_argument("--table")
    parser.add_argument("--heading")
    parser.add_argument("--column")
    parser.add_argument("--text")
    parser.add_argument("--identifiers")
    arguments = parser.parse_args()

    terms = read_vocabulary(arguments.vocabulary)
    items = []
    if arguments.table:
        if not arguments.heading or not arguments.column:
            parser.error("--table needs --heading and --column")
        for first, cell in table_column(arguments.table, arguments.heading, arguments.column):
            items.append((f"row {first}", cell))
        source = f"{arguments.table}, heading containing {arguments.heading!r}, column {arguments.column!r}"
    elif arguments.text:
        with open(arguments.text, encoding="utf-8") as handle:
            for number, line in enumerate(handle, start=1):
                if line.strip():
                    items.append((f"line {number}", line.rstrip("\n")))
        source = arguments.text
    elif arguments.identifiers:
        with open(arguments.identifiers, encoding="utf-8") as handle:
            for line in handle:
                if line.strip():
                    items.append(("identifier", line.strip()))
        source = arguments.identifiers
    else:
        parser.error("name one of --table, --text or --identifiers")
        return 2

    print(f"# ubc-vocabulary-scan: {source}")
    print(f"# vocabulary: {os.path.relpath(arguments.vocabulary, ROOT).replace(os.sep, '/')}, terms: {', '.join(t for t, _ in terms)}")
    matched = 0
    for label, text in items:
        found = matches(text, terms)
        if found:
            matched += 1
            print(f"MATCH {label}: {', '.join(found)}")
            print(f"      {text}")
        else:
            print(f"ok    {label}")
    print(f"# scanned {len(items)} item(s); {matched} matched")
    return 1 if matched else 0


if __name__ == "__main__":
    sys.exit(main())
