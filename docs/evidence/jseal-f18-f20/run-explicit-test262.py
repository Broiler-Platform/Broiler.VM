"""Explicit runner for Test262 files the harness skips as a proposed feature.

Concatenates assert.js + sta.js (+ doneprintHandle.js for async) + includes + the test, runs the
Broiler CLI (--quiet, script goal), and scores non-strict and strict variants the way the harness
does (onlyStrict / noStrict / raw respected). Negative tests expect a failure of the stated type.
"""
import os, re, subprocess, sys, json

SUITE = "D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e"
CLI = sys.argv[1]
OUT = sys.argv[2]
DIRS = sys.argv[3:]

def front(text):
    m = re.search(r"/\*---(.*?)---\*/", text, re.S)
    return m.group(1) if m else ""

def listfield(fm, name):
    m = re.search(r"^\s*" + name + r":\s*\[(.*?)\]", fm, re.M)
    if m:
        return [x.strip() for x in m.group(1).split(",") if x.strip()]
    m = re.search(r"^\s*" + name + r":\s*\n((?:\s+-\s*.*\n)+)", fm, re.M)
    if m:
        return [l.strip()[1:].strip() for l in m.group(1).splitlines() if l.strip()]
    return []

def harness(name):
    with open(os.path.join(SUITE, "harness", name), encoding="utf-8") as f:
        return f.read()

files = []
for d in DIRS:
    for root, _, names in os.walk(os.path.join(SUITE, d)):
        for n in sorted(names):
            if n.endswith(".js") and "_FIXTURE" not in n:
                files.append(os.path.join(root, n))
files.sort()

os.makedirs(OUT, exist_ok=True)
tmp = os.path.join(OUT, "_case.js")
results = []
for path in files:
    rel = os.path.relpath(path, SUITE).replace("\\", "/")
    text = open(path, encoding="utf-8").read()
    fm = front(text)
    flags = listfield(fm, "flags")
    includes = listfield(fm, "includes")
    negative = re.search(r"negative:\s*\n\s*phase:\s*(\w+)\s*\n\s*type:\s*(\w+)", fm)
    if "module" in flags:
        variants = ["module"]
    elif "raw" in flags:
        variants = ["raw"]
    elif "onlyStrict" in flags:
        variants = ["strict"]
    elif "noStrict" in flags:
        variants = ["sloppy"]
    else:
        variants = ["sloppy", "strict"]
    for v in variants:
        parts = []
        if v == "strict":
            parts.append('"use strict";\n')
        if v != "raw":
            parts += [harness("assert.js"), harness("sta.js")]
            if "async" in flags:
                parts.append(harness("doneprintHandle.js"))
            for inc in includes:
                parts.append(harness(inc))
        parts.append(text)
        if v != "module":
            # The CLI renders a script's completion value and a Symbol completion throws while
            # rendering (pre-existing CLI behaviour, not a test outcome), so the completion is
            # pinned to undefined.
            parts.append(";void 0;")
        with open(tmp, "w", encoding="utf-8", newline="\n") as f:
            f.write("\n".join(parts))
        args = [CLI, "--quiet"] + (["--module"] if v == "module" else []) + [tmp]
        try:
            p = subprocess.run(args, capture_output=True, timeout=20)
            out = (p.stdout + p.stderr).decode("utf-8", "replace")
            code = p.returncode
        except subprocess.TimeoutExpired:
            out, code = "TIMEOUT", -1
        if negative:
            ok = code != 0 and negative.group(2) in out
        elif "async" in flags:
            ok = code == 0 and "Test262:AsyncTestComplete" in out and "Test262:AsyncTestFailure" not in out
        else:
            ok = code == 0
        results.append({"file": rel, "variant": v, "pass": ok, "out": out.strip()[-400:]})

passed = sum(r["pass"] for r in results)
with open(os.path.join(OUT, "results.json"), "w", encoding="utf-8") as f:
    json.dump(results, f, indent=1)
with open(os.path.join(OUT, "summary.txt"), "w", encoding="utf-8") as f:
    f.write(f"files {len(files)} variants {len(results)} pass {passed} fail {len(results)-passed}\n")
    for d in DIRS:
        sub = [r for r in results if r["file"].startswith(d + "/")]
        f.write(f"{d}: {sum(r['pass'] for r in sub)}/{len(sub)}\n")
    for r in results:
        if not r["pass"]:
            f.write(f"FAIL {r['file']} [{r['variant']}] :: {r['out'][-200:]!r}\n")
os.remove(tmp)
print(open(os.path.join(OUT, "summary.txt"), encoding="utf-8").read())
