"""Independent completeness check of a VM --parse-corpus report against the manifest."""
import gzip, sys, collections, hashlib
man, rep = sys.argv[1], sys.argv[2]
mb = open(man, "rb").read(); digest = hashlib.sha256(mb).hexdigest()
ml = mb.decode().split("\n")[:-1]
evals = [l.split("\t")[1] for l in ml if l.startswith("#% eval-flags\t")]
i = next(k for k, l in enumerate(ml) if not l.startswith("#"))
rows = [l.split("\t") for l in ml[i + 1:]]
exp = []; admitted = {}
for pop in ("B1", "B2", "B3", "B4"):
    for r in rows:
        rid, origin, path, goal, variants, ef = r[0], r[1], r[2], r[4], r[5].split(","), r[7]
        if pop == "B1":
            for v in variants: exp += [(rid, pop, v, "bytecode"), (rid, pop, v, "native")]
        elif pop == "B2" and origin == "test262":
            for v in variants:
                exp.append((rid, pop, v, "bytecode"))
                if "_FIXTURE" in path: admitted[exp[-1]] = "fixture"
        elif pop == "B3" and ef == "yes":
            for e in evals: exp.append((rid, pop, e, "bytecode"))
        elif pop == "B4":
            exp.append((rid, pop, goal, "slice"))
            if goal == "json": admitted[exp[-1]] = "no-json-goal"
data = open(rep, "rb").read()
if data[:2] == b"\x1f\x8b": data = gzip.decompress(data)
text = data.decode()
assert text.endswith("\n") and "\r" not in text
lines = text[:-1].split("\n")
hdr = {}; k = 0
while lines[k].startswith("#"):
    if lines[k].startswith("#% "):
        a, _, b = lines[k][3:].partition("\t"); hdr[a] = b
    k += 1
cols = lines[k].split("\t"); body = [l.split("\t") for l in lines[k + 1:]]
twice = hdr.get("twice-concurrent") == "yes"
keys = [tuple(r[:4]) for r in body]
print("manifest digest ok:", hdr["manifest-sha256"] == digest, "| compilations header", hdr["compilations"], "| body", len(body),
      "| expected", len(exp), "| keys == expected in plan order:", keys == exp, "| workers", hdr["workers"], "| twice", twice,
      "| populations", hdr["populations"], "| selection", hdr["selection"])
c = collections.Counter(); bad = []
disagree = 0
for r in body:
    key = tuple(r[:4]); out, note = r[4], r[8]
    c[(key[1], out)] += 1
    if out == "crashed" or (out == "not-compiled" and admitted.get(key) != note) or (key in admitted and out != "not-compiled"):
        bad.append(r)
    if twice and r[4:9] != r[9:14]: disagree += 1
for x in sorted(c): print(" ", x, c[x])
print("rows outside the rule's outcomes or reading 2:", len(bad), bad[:3])
if twice: print("rows whose two concurrent compilations differ:", disagree)
