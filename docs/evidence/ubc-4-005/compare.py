#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE COMPARISON OF BUNDLE UBC-4-005: the base run against the run after, per command of population A and per
# member of population B, over the members present at both. It lists every difference and classifies none: the
# classes are the README's to name, row by row, as the rule asks.
#
#   python3 docs/evidence/ubc-4-005/compare.py docs/evidence/ubc-4-005
#
# Population A is joined by command identity - the script and the command's ordinal - and compares the answer.
# Population B is joined by lane and name. The name of a corpus entry is its own name; the name of an execution
# or differential check is its label and its entry, counted by occurrence, because one check group may call an
# entry more than once.

import collections
import os
import re
import sys

MEMBER = re.compile(r"^(ok  |FAIL) (.*)$")


def population_a(path):
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


def population_b(path):
    rows = collections.OrderedDict()
    seen = collections.Counter()
    lane = None
    with open(path, encoding="utf-8") as handle:
        for line in handle:
            line = line.rstrip("\n")
            if line.startswith("# retained corpus"):
                lane = "corpus"
            elif line.startswith("# execution"):
                lane = "execution"
            elif line.startswith("# differential"):
                lane = "differential"
            elif line.startswith("# "):
                lane = None
            match = MEMBER.match(line)
            if not match or lane is None:
                continue
            verdict, rest = match.groups()
            parts = rest.split(": ")
            if lane == "corpus":
                name, answer = parts[0], ": ".join(parts[1:])
            else:
                name, answer = ": ".join(parts[:2]), ": ".join(parts[2:])
            seen[(lane, name)] += 1
            rows["%s|%s#%d" % (lane, name, seen[(lane, name)])] = (lane, answer, verdict.strip())
    return rows


def compare(label, base, after, out):
    shared = [key for key in base if key in after]
    differing = [key for key in shared if base[key][1] != after[key][1]]
    moves = collections.Counter((base[key][2], after[key][2]) for key in shared)
    out.append("# population %s: %d at the base, %d after, %d present at both, %d only at the base, %d only after" % (
        label, len(base), len(after), len(shared), len(base) - len(shared), len(after) - len(shared)))
    out.append("# population %s: %d answers identical, %d differ" % (label, len(shared) - len(differing), len(differing)))
    for (was, now), count in sorted(moves.items()):
        out.append("# population %s: verdict %s -> %s: %d" % (label, was, now, count))
    for key in differing:
        kind, was, was_verdict = base[key]
        _, now, now_verdict = after[key]
        out.append("%s %s %s | base: %s [%s] | after: %s [%s]" % (label, key, kind, was, was_verdict, now, now_verdict))
    return len(differing)


def main():
    here = sys.argv[1]
    out = []
    compare("A", population_a(os.path.join(here, "base-run.log")), population_a(os.path.join(here, "after-run.log")), out)
    out.append("")
    compare("B", population_b(os.path.join(here, "base-population-b.log")), population_b(os.path.join(here, "after-population-b.log")), out)
    print("\n".join(out))
    return 0


if __name__ == "__main__":
    sys.exit(main())
