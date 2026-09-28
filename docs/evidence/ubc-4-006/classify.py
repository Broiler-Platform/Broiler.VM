#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE CLASSIFICATION OF BUNDLE UBC-4-006: every difference comparison.log lists, named with the rule's class it is
# in, or with NONE. It also writes the verdict floor re-based to the run after, each command that moved named with
# its class.
#
#   python3 docs/evidence/ubc-4-006/classify.py <the extracted test/core directory> docs/evidence/ubc-4-006
#
# It writes classes.log and floor-rebased.txt into the bundle. The classes are the rule's:
#   (f) a command whose module's code contains one of the twelve float comparisons, whose base answer is the retired
#       interpreter's defect (ProfileFault/ProfileContractViolation, no results) and whose answer after passes;
#   (r) a refusal at both commits of the same outcome with a universal code in place of the profile's;
#   (g) a growth's -1 at the base answered as a resource exhaustion after;
#   (v) a verdict failing at the base and passing after, outside the three above - named only with a cause;
#   (n) one NaN value of one width at both commits, the verdict passing at both, the answer after exactly the
#       positive canonical NaN of that width - the class this bundle's rule adds to UBC-4-005's.
# A difference in none of them is written as NONE, with the reason it is in none. This script does not
# admit anything: the rule admits, and a NONE row is a regression under it.
#
# Which module a command acts on is read from the script as the reader reads it: the module a named action names,
# otherwise the last module command before it. Whether that module's code contains a float comparison is read off
# its text, by the twelve instruction names; a binary module is never the module an (f) candidate acts on here.

import collections
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(HERE, "reader-check"))
COMPARISON = re.compile(r"^A (\S+) (\S+) \| base: (.*) \[(\w+)\] \| after: (.*) \[(\w+)\]$")
FLOAT_COMPARISON = re.compile(rb"\bf(32|64)\.(eq|ne|lt|gt|le|ge)\b")
CANONICAL = {"f32": 0x7FC00000, "f64": 0x7FF8000000000000}


def lists():
    """The extractor's reader of a script's lists, shared so that both number commands alike."""
    source = open(os.path.join(HERE, "reader-check", "extract.py"), encoding="utf-8").read()
    namespace = {}
    exec(source.split("items=[]")[0].split("root, out = sys.argv[1], sys.argv[2]")[1], namespace)
    return namespace["forms"], namespace["text"], namespace["head"]


def targets(root):
    """For every action and action assertion: whether the module it acts on has a float comparison in its code."""
    forms, text, head = lists()
    answer = {}
    for name in sorted(os.listdir(root)):
        if not name.endswith(".wast"):
            continue
        source = open(os.path.join(root, name), "rb").read()
        current = None
        named = {}
        for ordinal, form in enumerate(forms(source), 1):
            keyword = head(source, form)
            if keyword == "module":
                words = [text(source, child) for child in form[2] if child[2] is None]
                current = None if b"binary" in words else bool(FLOAT_COMPARISON.search(text(source, form)))
                if len(form[2]) > 1 and form[2][1][2] is None and text(source, form[2][1]).startswith(b"$"):
                    named[text(source, form[2][1])] = current
                continue
            action = None
            if keyword in ("invoke", "get"):
                action = form
            elif keyword and (keyword.startswith("assert_return") or keyword in ("assert_trap", "assert_exhaustion")):
                if len(form[2]) > 1 and form[2][1][2] is not None and head(source, form[2][1]) in ("invoke", "get"):
                    action = form[2][1]
            if action is not None:
                target = current
                if len(action[2]) > 1 and action[2][1][2] is None and text(source, action[2][1]).startswith(b"$"):
                    target = named.get(text(source, action[2][1]))
                answer["%s:%d" % (name, ordinal)] = target
    return answer


def nan(answer):
    """The one NaN value an answer holds, as (kind, bits), or None."""
    parts = answer.split(" ")
    if len(parts) != 2 or parts[0] != "values":
        return None
    kind, bits = parts[1].split(":")
    if kind not in CANONICAL:
        return None
    value = int(bits, 16)
    exponent = 0x7F800000 if kind == "f32" else 0x7FF0000000000000
    fraction = 0x007FFFFF if kind == "f32" else 0x000FFFFFFFFFFFFF
    return (kind, value) if value & exponent == exponent and value & fraction else None


def nan_class(base, after, base_verdict, after_verdict):
    """Class (n): one NaN value of one width at both commits, both passing, the answer after the positive canonical NaN."""
    was, now = nan(base), nan(after)
    return (was is not None and now is not None and was[0] == now[0] and base_verdict == after_verdict == "pass"
            and now[1] == CANONICAL[now[0]])


def classify(identity, command, base, base_verdict, after, after_verdict, float_modules):
    if base.startswith("ProfileFault/ProfileContractViolation") and after_verdict == "pass":
        if float_modules.get(identity):
            return "(f)", "the module's code has a float comparison; the base's routing defect, the specification's value after"
        return "NONE", "the base's contract violation, in a module with no float comparison"
    if base.startswith("refused ") and after.startswith("refused "):
        return "NONE", "a refusal at both, which (r) admits only with a universal code after"
    if nan_class(base, after, base_verdict, after_verdict):
        return "(n)", "one NaN value both runs pass with, the base's sign or payload carried over, the positive canonical NaN after"
    if base_verdict == "fail" and after_verdict == "pass":
        return "NONE", "a verdict corrected, which (v) admits only with its cause written; none is written here"
    return "NONE", "a difference no class describes"


def main():
    root, bundle = sys.argv[1], sys.argv[2]
    float_modules = targets(root)
    rows = []
    counts = collections.Counter()
    moved = {}
    with open(os.path.join(bundle, "comparison.log"), encoding="utf-8") as handle:
        for line in handle:
            match = COMPARISON.match(line.rstrip("\n"))
            if not match:
                continue
            identity, command, base, base_verdict, after, after_verdict = match.groups()
            label, reason = classify(identity, command, base, base_verdict, after, after_verdict, float_modules)
            counts[(label, reason)] += 1
            rows.append("%s %s %s | %s" % (label, identity, command, reason))
            if base_verdict != after_verdict:
                moved[identity] = label
    out = ["# classes of every difference in comparison.log, by the rule of decision-rule.md"]
    for (label, reason), count in sorted(counts.items()):
        out.append("# %s: %d: %s" % (label, count, reason))
    out.extend(rows)
    with open(os.path.join(bundle, "classes.log"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("\n".join(out) + "\n")

    floor = ["# revision 977f97014c962f7bd1291fcc6d28b41a924882bf",
             "# The verdict floor of population A, set from the base run (base-verdicts.txt) and re-based by hand to the run",
             "# after (after-verdicts.txt). A command whose verdict moved carries its class; every other line is the base's."]
    with open(os.path.join(bundle, "after-verdicts.txt"), encoding="utf-8") as handle:
        for line in handle:
            line = line.rstrip("\n")
            if not line or line.startswith("#"):
                continue
            identity = line.split(" ")[0]
            floor.append(line + (" " + moved[identity] if identity in moved else ""))
    with open(os.path.join(bundle, "floor-rebased.txt"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("\n".join(floor) + "\n")
    print("\n".join(out[:1 + len(counts)]))
    return 0


if __name__ == "__main__":
    sys.exit(main())
