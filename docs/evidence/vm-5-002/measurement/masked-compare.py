"""E5 and E6: compare two verbose check transcripts with what is allowed to differ masked.

Masked: elapsed times written as "<n> ms", hexadecimal addresses, and every number on the
recycled-runtime heap-plateau row. Anything else that differs is printed.

python masked-compare.py <left> <right>
"""
import re
import sys


def masked(path):
    lines = []
    for line in open(path, encoding="utf-8-sig"):
        line = line.rstrip("\n")
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
