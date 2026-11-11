#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Writes comparison-revision-38.txt (bundle sjl-9-001, vectors/): this family's run on revision 38 on its pin against its run on revision 32 on the
pin before, every verdict that moves attributed, and the negative control against the run.
Usage: compare.py <base.tsv> <after.tsv> <negative-control.tsv> <retired.tsv> <old vectors dir> <new vectors dir>"""
import collections, hashlib, os, sys
base_p, after_p, nc_p, retired_p, old_dir, new_dir = sys.argv[1:7]
def sha(p): return hashlib.sha256(open(p, "rb").read()).hexdigest()
def load(p):
    head, d = [], {}
    for l in open(p, encoding="utf-8"):
        if l.startswith("#"): head.append(l.rstrip("\n")); continue
        if l.startswith("id\t"): continue
        x = l.rstrip("\n").split("\t"); d[x[0]] = x
    return head, d
def records(root):
    d = {}
    for dp, _, fs in os.walk(root):
        for f in sorted(fs):
            if not f.endswith(".vectors"): continue
            rec = None
            for line in open(os.path.join(dp, f), encoding="utf-8"):
                line = line.rstrip("\n")
                if not line.strip():
                    if rec is not None: d[rec["id"][0]] = rec; rec = None
                    continue
                if line.startswith("#"): continue
                k, _, v = line.partition(": ")
                if k == "id": rec = collections.defaultdict(list)
                if rec is not None: rec[k].append(v)
            if rec is not None: d[rec["id"][0]] = rec
    return d
# The vectors revision 36's and revision 37's rows of REVISIONS.md (Broiler.JS 3493462a) name as gaining a field.
REV = {f"intl/numbers-and-text/families/{n}": "revision 36 adds (the pinned CLDR 48.2.0 contradicts the reference)" for n in ("0121", "0309", "0454")}
REV.update({i: "revision 37 adds (a host that lists aa or pap, decision D7)" for i in
    ["intl/date-times/families/" + n for n in ("0001", "0004", "0307", "0310")] + ["intl/durations/families/0053"] +
    ["intl/numbers-and-text/families/" + n for n in ("0003", "0509", "0006", "0512", "0008", "0514", "0009", "0515")]})
hb, a = load(base_p); ha, b = load(after_p); hn, n = load(nc_p)
old, new = records(old_dir), records(new_dir)
retired = {}
for l in open(retired_p, encoding="utf-8"):
    x = l.rstrip("\n").split("\t")
    if len(x) == 3 and x[0] == "broiler-vm": retired[x[2]] = x[1]
out = []
w = out.append
w("Comparison of this family's run on corpus revision 38, on the committed pin, with its run on corpus revision 32, on the")
w("pin before it moved, and of the run's negative control with the run. Written by compare-revision-38.py here from the files below.")
w("")
w(f"base:     run-base-revision-32.tsv.gz here (decompressed {sha(base_p)})")
w(f"after:    run-revision-38.tsv.gz here (decompressed {sha(after_p)})")
w(f"control:  run-negative-control-revision-38.tsv.gz here (decompressed {sha(nc_p)})")
w(f"retired:  Broiler.JS 3493462a docs/evidence/corpus-revision-38/retired-divergences-revision-38.tsv (sha256 {sha(retired_p)}), its broiler-vm lines")
w("vectors:  the extracted archives of b2c2e224 (revision 32) and 3493462a (revision 38), read record by record")
w("")
w("######## the summary lines (# ...)")
w("base:"); [w("  " + l) for l in hb]
w("after:"); [w("  " + l) for l in ha]
w("")
w("######## the vectors")
added = sorted(set(b) - set(a)); removed = sorted(set(a) - set(b))
w(f"rows: base {len(a)}, after {len(b)}; in both {len(set(a) & set(b))}; only after {len(added)}; only in the base {len(removed)}")
g = collections.Counter(("/".join(i.split("/")[:3]), b[i][1], b[i][4]) for i in added)
w("rows only after, by file group, class and verdict (revision 33's collation vectors; their since field):")
for k, c in sorted(g.items()): w(f"  {c:6d}  {k[0]}  {k[1]}  {k[2]}")
w("  since: " + ", ".join(f"{s} {c}" for s, c in sorted(collections.Counter(new[i]["since"][0] for i in added).items())))
w("")
w("######## the records of the vectors in both runs, field by field (revision 32 against revision 38)")
fc = collections.Counter()
for i in set(old) & set(new):
    for k in set(old[i]) | set(new[i]):
        if old[i].get(k) != new[i].get(k): fc[("/".join(i.split("/")[:2]), k)] += 1
for (grp, k), c in sorted(fc.items()): w(f"  {c:6d}  {grp}  {k}")
w("  (no out, script, in, op or prov field of any other group changes; a field this list does not name is unchanged)")
w("")
w("######## every verdict that moves, attributed")
moves = collections.Counter(); unattributed = []
for i in sorted(set(a) & set(b)):
    if a[i][4] == b[i][4]: continue
    grp = "/".join(i.split("/")[:2])
    if i in retired and a[i][4] == "match (divergence no longer holds)" and b[i][4] == "match" and "divergence.broiler-vm" in old[i] and "divergence.broiler-vm" not in new[i]:
        why = f"revision 38 retires its divergence.broiler-vm field ({retired[i]}), which SJL-9.4 corrected"
    elif "per-engine.broiler-vm" in old[i] and "per-engine.broiler-vm" not in new[i] and a[i][4] == "difference" and b[i][4] == "match" and b[i][2] == "value: " + old[i]["out"][0][len("value: "):] and a[i][3] == b[i][3] == b[i][2]:
        why = "row Z8: revision 38 drops its per-engine.broiler-vm expectation (the primary), so the expectation is out, the identifier as given, which the run renders at both pins (decision D15)"
    else:
        why = None; unattributed.append(i)
    moves[(grp, a[i][1], b[i][1], a[i][4], b[i][4], why)] += 1
for k, c in sorted(moves.items(), key=str):
    w(f"  {c:6d}  {k[0]}  class {k[1]} -> {k[2]}  {k[3]} -> {k[4]}")
    w(f"          {k[5]}")
w(f"unattributed moves: {len(unattributed)} {unattributed}")
w(f"the retired fields' vectors: {len(retired)}; after: " + ", ".join(f"{v} {c}" for v, c in collections.Counter(b[i][4] for i in retired).items()))
w("")
w("######## the vectors in both runs whose verdict does not move, other columns")
oc = collections.Counter(); exp_other = []
for i in sorted(set(a) & set(b)):
    if a[i][4] != b[i][4]: continue
    grp = "/".join(i.split("/")[:2])
    if a[i][1] != b[i][1]:
        dv = lambda r: sorted(k for k in r if k.startswith("divergence."))
        if b[i][1] == "divergence" and dv(old[i]) == [] and dv(new[i]) == ["divergence.broiler-js"]:
            why = "a divergence.broiler-js field, Broiler.JS's alone, that " + REV.get(i, "NO ROW NAMES")
        elif b[i][1] == "normative" and dv(old[i]) and dv(new[i]) == []:
            why = "revision 38 retires its last divergence field, " + " and ".join(dv(old[i]))
        else:
            why = "UNATTRIBUTED"

        oc[(grp, f"class {a[i][1]} -> {b[i][1]}", why)] += 1
    if a[i][2] != b[i][2]: exp_other.append(i)
    if a[i][3] != b[i][3]: exp_other.append(i)
for k, c in sorted(oc.items()): w(f"  {c:6d}  {k[0]}  {k[1]}  ({k[2]})")
w(f"expected or rendered column changed with the verdict unchanged: {len(exp_other)}")
w(f"rendered column changed for any vector in both: {sum(1 for i in set(a) & set(b) if a[i][3] != b[i][3])}")
w("")
w("######## the negative control against the run")
diff = [i for i in b if b[i] != n.get(i)] + [i for i in n if i not in b]
w(f"rows that differ: {len(diff)}")
for i in diff: w(f"  run:     {chr(9).join(b[i])}"); w(f"  control: {chr(9).join(n[i])}")
w("summary lines that differ:")
for x, y in zip(ha, hn):
    if x != y: w(f"  run: {x}  | control: {y}")
print("\n".join(out))
