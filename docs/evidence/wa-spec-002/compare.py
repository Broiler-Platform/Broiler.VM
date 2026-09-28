#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE COMPARISON OF RECORD WA-SPEC-002: the specification's core scripts through the harness root's --spec lane
# before the change and after it, per command, with every command whose answer or verdict moved named with the
# change that moved it.
#
#   python3 docs/evidence/wa-spec-002/compare.py docs/evidence/wa-spec-002
#
# It reads before-run.log and after-run.log and prints the per-family totals of each and one line per command that
# moved. A command is named with the first class whose bounds it meets, read off its two answers alone:
#   (l) limits              refused before with code 2305 or 2306 as not decoded, refused after with code 2712 or 2713
#                           as invalid
#   (k) constant expression refused before with code 2601 or 2602, refused after with code 2714 or 2709 as invalid
# A command in none of them is written NONE, and the last line counts them. The script judges nothing else.

import collections
import os
import re
import sys

REFUSAL = re.compile(r"^refused (\w+)/(\w+)/(\d+)@")


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
    if was == now and was_verdict == now_verdict:
        return None
    old, new = refusal(was), refusal(now)
    invalid_after = new and new[0] == "InvalidArtifact" and new[1] == "SemanticValidationFailed"
    if old and old[2] in ("2305", "2306") and invalid_after and new[2] in ("2712", "2713"):
        return "(l)"
    if old and old[2] in ("2601", "2602") and invalid_after and new[2] in ("2714", "2709"):
        return "(k)"
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
