"""E9 of VM-5-002's re-collection: the smallest --fuel that completes a program, per form, by binary
search through the command-line host, then the host's output one below it, once plain and once with
BROILER_MEASURE_BUDGET=1 (the runtime budget snapshot, exhaustion dimension and scope).

The first collection's measurement/p_bisect.py, changed in one way: a variant is named with the
directory its host runs from (label=directory) instead of a bin/<label> directory beside the script,
so the binary the identity lines hash is the binary that runs. Programs are read from the first
collection's measurement/shapes, and a program item may join several files with '+', which the host
runs as several scripts in one realm. Output lines are the first collection's, program names included.

python p_bisect.py credit=<cli bin dir> shapes/a_1000.js,shapes/c_small.js+shapes/a_1000.js
"""
import os
import pathlib
import subprocess
import sys

SHAPES_ROOT = pathlib.Path(__file__).resolve().parents[2] / "measurement"
variants = [v.split("=", 1) for v in sys.argv[1].split(",")]
programs = sys.argv[2].split(",")


def run(bindir, form, files, fuel, budget=False):
    cmd = [str(pathlib.Path(bindir) / "Broiler.VM.Composition.JavaScript.Cli.exe"), "--wall", "600000", "--fuel", str(fuel)]
    if form == "native":
        cmd += ["--native", "x86-64-win64"]
    cmd += [str(SHAPES_ROOT / f) for f in files]
    env = dict(os.environ)
    env.pop("BROILER_MEASURE", None)
    if budget:
        env["BROILER_MEASURE_BUDGET"] = "1"
    else:
        env.pop("BROILER_MEASURE_BUDGET", None)
    p = subprocess.run(cmd, capture_output=True, text=True, env=env)
    text = p.stdout + p.stderr
    for f in files:
        text = text.replace(str(SHAPES_ROOT / f).replace("\\", "/"), "<" + pathlib.Path(f).name + ">").replace(str(SHAPES_ROOT / f), "<" + pathlib.Path(f).name + ">")
    return p.returncode, text.strip().replace("\n", " | ")


for program in programs:
    files = program.split("+")
    for variant, bindir in variants:
        for form in ("bytecode", "native"):
            lo, hi = 1, 4_000_000
            code, text = run(bindir, form, files, hi)
            assert code == 0, (variant, form, program, text)
            while lo < hi:
                mid = (lo + hi) // 2
                if run(bindir, form, files, mid)[0] == 0:
                    hi = mid
                else:
                    lo = mid + 1
            _, at = run(bindir, form, files, lo, budget=True)
            code, plain = run(bindir, form, files, lo - 1)
            _, budget = run(bindir, form, files, lo - 1, budget=True)
            print(f"{program} {variant} {form} min_fuel={lo}", flush=True)
            print(f"  at_min(budget): {at}", flush=True)
            print(f"  below exit={code} plain: {plain}", flush=True)
            print(f"  below budget: {budget}", flush=True)
