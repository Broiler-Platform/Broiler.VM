"""Read E3's transcripts in VM-5-002's re-collection and print the witness table.

    python witness-table.py <e3 dir> [witness ...]

For each witness: the counts under the defect, whether every test and row the table names failed, every
failing test, the iterations of a sampled witness, the control run after the revert, and the run of the
named tests alone. Everything is read from the transcripts; the identity lines of the clean run before
the first witness are repeated at the top.
"""
import importlib.util
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
spec = importlib.util.spec_from_file_location("witnesses", os.path.join(HERE, "witnesses.py"))
table = importlib.util.module_from_spec(spec)
spec.loader.exec_module(table)

out = sys.argv[1]
names = sys.argv[2:] or list(table.ORDER)


def read(path):
    if not os.path.exists(path):
        return None
    return open(path, encoding="utf-8", errors="replace").read()


def counts(text):
    total = re.search(r"^Total tests: (\d+)", text, re.M)
    passed = re.search(r"^\s+Passed: (\d+)", text, re.M)
    failed = re.search(r"^\s+Failed: (\d+)", text, re.M)
    return "total=%s passed=%s failed=%s compile-errors=%d" % (
        total.group(1) if total else "?", passed.group(1) if passed else 0,
        failed.group(1) if failed else 0, len(re.findall(r"error CS\d+", text)))


def failing(text):
    return [re.sub(r" \[[^\]]*\]$", "", m.group(1)).replace("Broiler.VM.Contract.Tests.", "")
            for m in re.finditer(r"^\s+Failed (Broiler\.\S.*)$", text, re.M)]


def head(text, key):
    m = re.search(r"^# %s=(.*)$" % key, text, re.M)
    return m.group(1).strip() if m else "?"


clean = read(os.path.join(out, "clean-before.log"))
if clean:
    print("clean-before: %s (branch=%s head=%s tree=%s tree-head=%s)" % (
        counts(clean), head(clean, "branch"), head(clean, "head")[:7], head(clean, "tree"), head(clean, "tree-head")[:7]))

caught = 0
for name in names:
    log = read(os.path.join(out, name + ".log"))
    if log is None:
        print("%s: not run" % name)
        continue
    fails = failing(log)
    extra = ""
    for i in range(2, 11):
        it = read(os.path.join(out, "%s-iteration-%d.log" % (name, i)))
        if it is None:
            break
        it_fails = failing(it)
        extra += " | sampled test alone, run %d: %s" % (i, "FAILED" if it_fails else "passed")
        fails += it_fails
    must = table.WITNESSES[name][2]
    verdicts = ["%s -> %s" % (m, "FAILED" if any(m in x for x in fails) else "PASSED (the defect is not caught)")
                for m in must]
    all_caught = all("-> FAILED" in v for v in verdicts)
    caught += all_caught
    print("%s: %s | injected %s%s" % (name, table.WITNESSES[name][0], counts(log), extra))
    for v in verdicts:
        print("    must fail: " + v)
    for x in failing(log):
        print("    failing: " + x)
    control = read(os.path.join(out, name + ".control.log"))
    alone = read(os.path.join(out, name + ".alone.log"))
    print("    control after revert: %s" % (counts(control) if control else "not run"))
    print("    named tests alone after revert: %s" % (counts(alone) if alone else "not run"))
print("witnesses whose every named test and row failed: %d of %d" % (caught, len(names)))
