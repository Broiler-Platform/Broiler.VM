"""E5 and E6 of VM-5-002's re-collection: compare two verbose check transcripts with what is allowed to
differ masked.

The first collection's measurement/masked-compare.py, extended in one way: the identity lines a
re-collection driver writes above and below a transcript (lines beginning "# " before the first line
of output, up to and including "# command=", and the "# exit=" and "# finished=" lines at its end)
are dropped before comparing, so a transcript of the re-collection can be held against one of the
first collection, which has none. Masked, as before: elapsed times written as "<n> ms", hexadecimal
addresses, and every number on the recycled-runtime heap-plateau row. Anything else that differs is
printed.

python masked-compare.py <left> <right>
"""
import re
import sys


def body(path):
    raw = [line.rstrip("\n") for line in open(path, encoding="utf-8-sig")]
    if any(line.startswith("# command=") for line in raw[:40]):
        start = next(i for i, line in enumerate(raw) if line.startswith("# command=")) + 1
        raw = raw[start:]
        while raw and (raw[-1].startswith("# exit=") or raw[-1].startswith("# finished=")):
            raw.pop()
    return raw


def masked(path):
    lines = []
    for line in body(path):
        line = re.sub(r"0x[0-9a-f]+", "0x<addr>", line)
        line = re.sub(r"\b\d+ ms\b", "<n> ms", line)
        if "recycled-runtimes-reach-a-heap-plateau" in line:
            line = re.sub(r"\d+(\.\d+)?", "<n>", line)
        lines.append(line)
    return lines


left, right = masked(sys.argv[1]), masked(sys.argv[2])
print("left  %s: %d lines" % (sys.argv[1], len(left)))
print("right %s: %d lines" % (sys.argv[2], len(right)))
differing = [(i + 1, a, b) for i, (a, b) in enumerate(zip(left, right)) if a != b]
for number, a, b in differing:
    print("line %d\n  < %s\n  > %s" % (number, a, b))
print("line counts equal: %s" % (len(left) == len(right)))
print("lines differing after masking: %d" % len(differing))
