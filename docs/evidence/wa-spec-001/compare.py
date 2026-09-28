#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE COMPARISON OF RECORD WA-SPEC-001: the specification's core scripts through the harness root's --spec lane
# before the change and after it, per command, with every command whose answer or verdict moved named with the
# change that moved it.
#
#   python3 docs/evidence/wa-spec-001/compare.py docs/evidence/wa-spec-001
#
# It reads before-run.log and after-run.log and prints the per-family totals of each and one line per command that
# moved. A command is named with the first class whose bounds it meets, read off its two answers and its command
# alone:
#   (s) scoring       the answer is the same at both, and the verdict moved: the lane's scoring changed
#   (c) custom name   an assert_malformed module the profile admitted before, refused after with a reason that says
#                     it did not decode
#   (i) import read   a module refused before as an unadmitted import (code 2403), refused after with a reason that
#                     says it did not decode
#   (r) two results   a module the profile admitted before, refused after with code 2711
#   (t) indirect type an action that trapped before as an indirect call type mismatch and returned values after
# A command in none of them is written NONE, and the last line counts them. The script judges nothing else.

import collections
import os
import re
import sys

REFUSAL = re.compile(r"^refused (\w+)/(\w+)/(\d+)@")
MALFORMED = {"Truncated", "MalformedEncoding", "UnknownFormatVersion", "InconsistentStructure"}


def load(path):
    rows = collections.OrderedDict()
    with open(path, encoding="utf-8") as handle:
        for line in handle:
            line = line.rstrip("\n")
            if line.startswith(("#", "$")) or " | " not in line:
                continue
            head, answer, verdict = line.split(" | ")
            identity, _, command = head.split(" ", 2)
            rows[identity] = (command, answer, verdict)
    return rows


def refusal(answer):
    match = REFUSAL.match(answer)
    return match.groups() if match else None


def classify(command, before, after):
    _, was, was_verdict = before
    _, now, now_verdict = after
    if was == now:
        return "(s)" if was_verdict != now_verdict else None
    old, new = refusal(was), refusal(now)
    if command == "assert_malformed" and was == "instance" and new and new[0] == "InvalidArtifact" and new[1] in MALFORMED:
        return "(c)"
    if old and old[2] == "2403" and new and new[0] == "InvalidArtifact" and new[1] in MALFORMED:
        return "(i)"
    if was == "instance" and new and new[2] == "2711":
        return "(r)"
    if was == "trap IndirectCallTypeMismatch" and now.startswith("values"):
        return "(t)"
    return "NONE"


def totals(rows):
    counts = collections.defaultdict(collections.Counter)
    for command, _, verdict in rows.values():
        counts[command][verdict] += 1
        counts["all"][verdict] += 1
    return counts


def main():
    here = sys.argv[1]
    before = load(os.path.join(here, "before-run.log"))
    after = load(os.path.join(here, "after-run.log"))
    out = []

    if list(before) != list(after):
        out.append("# the two runs do not name the same commands in the same order")
        print("\n".join(out))
        return 1

    before_totals, after_totals = totals(before), totals(after)
    out.append("# per command family: pass / fail / excluded, before -> after")
    for family in sorted(set(before_totals) | set(after_totals)):
        b, a = before_totals[family], after_totals[family]
        out.append("# %-30s %5d / %4d / %3d -> %5d / %4d / %3d" % (
            family, b["pass"], b["fail"], b["excluded"], a["pass"], a["fail"], a["excluded"]))

    classes = collections.Counter()
    moves = collections.Counter()
    rows = []
    for identity, was in before.items():
        now = after[identity]
        label = classify(was[0], was, now)
        if label is None:
            continue
        classes[label] += 1
        moves[(label, was[0], was[2], now[2])] += 1
        rows.append("%s %s %s | before: %s [%s] | after: %s [%s]" % (label, identity, was[0], was[1], was[2], now[1], now[2]))

    out.append("# commands whose answer or verdict moved: %d" % len(rows))
    for (label, command, was, now), count in sorted(moves.items()):
        out.append("# %s %s %s -> %s: %d" % (label, command, was, now, count))
    out.extend(rows)
    out.append("# NONE: %d" % classes["NONE"])
    print("\n".join(out))
    return 0 if classes["NONE"] == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
