"""Compare the probe's trace and stress output of VM-5-002's re-collection with the base files the first
collection retained.

    python trace-compare.py <first collection e12 dir> <re-collection e12 dir>

trace: the credit file's lines after the probe's own header lines (every line beginning "# ", and the
"variant=" line, which names the pre-admission table and so differs between builds by design) are
compared line by line with the retained probe-trace-base.txt read the same way. stress: each round's
PASS or FAIL, and the probe's failure count, are read from the credit file; stress output depends on
thread scheduling, so it is not compared with base line by line, as the first collection did not.
"""
import pathlib
import re
import sys

old = pathlib.Path(sys.argv[1])
new = pathlib.Path(sys.argv[2])


def body(path):
    return [l for l in path.read_text(encoding="utf-8").splitlines()
            if not l.startswith("# ") and not l.startswith("variant=")]


base = body(old / "probe-trace-base.txt")
credit = body(new / "probe-trace-credit.txt")
print("trace: retained base %s, %d lines after the header; credit %s, %d lines after the header"
      % ((old / "probe-trace-base.txt").as_posix(), len(base), (new / "probe-trace-credit.txt").as_posix(), len(credit)))
differing = sum(1 for a, b in zip(base, credit) if a != b) + abs(len(base) - len(credit))
for a, b in zip(base, credit):
    if a != b:
        print("  base:   " + a)
        print("  credit: " + b)
print("trace lines differing: %d" % differing)

stress = (new / "probe-stress-credit.txt").read_text(encoding="utf-8")
rounds = re.findall(r"^STRESS round=(\d+) maxAmount=(\d+) (PASS|FAIL)", stress, re.M)
failures = re.search(r"^stress_failures=(\d+)", stress, re.M)
print("stress: %d rounds, %d PASS, %d FAIL; stress_failures=%s"
      % (len(rounds), sum(r[2] == "PASS" for r in rounds), sum(r[2] == "FAIL" for r in rounds),
         failures.group(1) if failures else "(line absent)"))
