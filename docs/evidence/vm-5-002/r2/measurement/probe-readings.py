"""Rule item 3's first two parts, read from the probe files of VM-5-002's re-collection.

    python probe-readings.py <e12 dir>

concurrent-bench: the credit build's nanoseconds per admitted charge at or below base in every cell with
two or more threads. concurrent-ambient: the credit build at or below C2' (the build carrying only the
pre-admission table) in every cell with two or more threads; base is printed beside it. Every figure is
copied from the CONC line of the named file; each cell's exactness, runtime-consumption and fault fields
are checked and printed when any is not the passing value. bench's two lines are printed for each build
as they stand; no rule item reads them.
"""
import pathlib
import re
import sys

d = pathlib.Path(sys.argv[1])
LINE = re.compile(r"^CONC mode=(\S+) threads=(\d+) ceiling=(\S+) ns_per_admitted_charge_min_of_3=(\S+) "
                  r"admitted_per_rep=(\d+) rounds=(\d+) exact=(\S+) runtime_consumed_matches=(\S+) faults=(\d+)", re.M)


def cells(name):
    text = (d / name).read_text(encoding="utf-8")
    out = {}
    for m in LINE.finditer(text):
        key = (int(m.group(2)), m.group(3))
        out[key] = m.group(4)
        if m.group(7) != "True" or m.group(8) != "True" or m.group(9) != "0":
            print("  NOT CLEAN in %s: %s" % (name, m.group(0)))
    return out


def table(mode, columns, subject, comparator):
    data = {label: cells("probe-%s-%s.txt" % (mode, variant)) for label, variant in columns}
    print("%s (files %s)" % (mode, ", ".join("probe-%s-%s.txt" % (mode, v) for _, v in columns)))
    print("  threads | ceiling | " + " | ".join(label for label, _ in columns) + " | %s at or below %s" % (subject, comparator))
    held = failed = 0
    keys = sorted(set().union(*[set(v) for v in data.values()]), key=lambda k: (k[0], ["unbounded", "2^20", "4x2^20"].index(k[1])))
    for key in keys:
        row = [data[label].get(key, "absent") for label, _ in columns]
        verdict = ""
        if key[0] >= 2:
            s, c = data[subject].get(key), data[comparator].get(key)
            ok = s is not None and c is not None and float(s) <= float(c)
            held += ok
            failed += not ok
            verdict = "holds" if ok else "FAILS"
        print("  %s | %s | %s | %s" % (key[0], key[1], " | ".join(row), verdict or "(one thread: not read)"))
    print("  cells with two or more threads: %d hold, %d fail" % (held, failed))
    return failed == 0


bench_ok = table("concurrent-bench", [("base", "base"), ("credit", "credit")], "credit", "base")
ambient_ok = table("concurrent-ambient", [("base", "base"), ("c2r", "c2r"), ("credit", "credit")], "credit", "c2r")
for variant in ("base", "credit"):
    path = d / ("probe-bench-%s.txt" % variant)
    if path.exists():
        print("bench %s: %s" % (variant, "; ".join(re.findall(r"^BENCH (.*)$", path.read_text(encoding="utf-8"), re.M))))
print("item 3, concurrent-bench part: %s" % ("holds in every cell" if bench_ok else "FAILS"))
print("item 3, concurrent-ambient part: %s" % ("holds in every cell" if ambient_ok else "FAILS"))
