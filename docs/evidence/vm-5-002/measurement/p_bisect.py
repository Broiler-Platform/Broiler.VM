"""Smallest --fuel that completes a program, per variant and form (binary search), then the CLI output one below it,
once plain and once with BROILER_MEASURE_BUDGET=1 (runtime budget snapshot, exhaustion dimension and scope).
A program item may join several files with '+', which the CLI runs as several scripts in one realm.
python p_bisect.py base,core shapes/a_1000.js,shapes/c_small.js+shapes/a_1000.js"""
import os, pathlib, subprocess, sys

ROOT = pathlib.Path(__file__).resolve().parent
variants, programs = sys.argv[1].split(","), sys.argv[2].split(",")


def run(variant, form, files, fuel, budget=False):
    cmd = [str(ROOT / "bin" / variant / "Broiler.VM.Composition.JavaScript.Cli.exe"), "--wall", "600000", "--fuel", str(fuel)]
    if form == "native":
        cmd += ["--native", "x86-64-win64"]
    cmd += [str(ROOT / f) for f in files]
    env = dict(os.environ)
    env.pop("BROILER_MEASURE", None)
    if budget:
        env["BROILER_MEASURE_BUDGET"] = "1"
    else:
        env.pop("BROILER_MEASURE_BUDGET", None)
    p = subprocess.run(cmd, capture_output=True, text=True, env=env)
    text = p.stdout + p.stderr
    for f in files:
        text = text.replace(str(ROOT / f).replace("\\", "/"), "<" + pathlib.Path(f).name + ">").replace(str(ROOT / f), "<" + pathlib.Path(f).name + ">")
    return p.returncode, text.strip().replace("\n", " | ")


for program in programs:
    files = program.split("+")
    for variant in variants:
        for form in ("bytecode", "native"):
            lo, hi = 1, 4_000_000
            code, text = run(variant, form, files, hi)
            assert code == 0, (variant, form, program, text)
            while lo < hi:
                mid = (lo + hi) // 2
                if run(variant, form, files, mid)[0] == 0:
                    hi = mid
                else:
                    lo = mid + 1
            _, at = run(variant, form, files, lo, budget=True)
            code, plain = run(variant, form, files, lo - 1)
            _, budget = run(variant, form, files, lo - 1, budget=True)
            print(f"{program} {variant} {form} min_fuel={lo}", flush=True)
            print(f"  at_min(budget): {at}", flush=True)
            print(f"  below exit={code} plain: {plain}", flush=True)
            print(f"  below budget: {budget}", flush=True)
