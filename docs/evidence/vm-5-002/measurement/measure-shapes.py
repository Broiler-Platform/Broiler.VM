#!/usr/bin/env python3
"""E10: long shapes through the command-line host, three lanes, two forms.

Lanes are snapshotted binary directories, not trees: `base` is the base build's CLI output
directory, `credit` is the changed build's, and `aa` is a byte copy of `credit`, which makes the
A/A lane an identical build measured in the same interleaving. Both `credit` and `aa` carry the
measurement-only CLI patch (the one E9 uses), applied to both trees, built, snapshotted here and
reverted immediately afterwards, so `invoke_ms` is the invocation alone rather than the process.

Order per repetition is shape -> lane -> form, so every cell is interleaved with every other and a
drift in the machine moves all of them together. Two warm-up repetitions are run and recorded with
negative repetition numbers; only the seven counted repetitions enter a median.

python measure-shapes.py --out <prefix>
"""
import argparse
import json
import os
import pathlib
import re
import statistics
import subprocess
import sys
import time

HERE = pathlib.Path(__file__).resolve().parent
SHAPES_DIR = pathlib.Path(
    "C:/Users/proof/AppData/Local/Temp/claude/D--Broiler-VM/"
    "cbafeab9-f73a-434e-9481-38837a0bebe7/scratchpad/impl/measure/shapes")

ap = argparse.ArgumentParser()
ap.add_argument("--lanes", default="base,credit,aa")
ap.add_argument("--shapes", default="a_numeric,b_property,c_call,d_fib")
ap.add_argument("--forms", default="bytecode,native")
ap.add_argument("--warmups", type=int, default=2)
ap.add_argument("--reps", type=int, default=7)
ap.add_argument("--wall", default="600000")
ap.add_argument("--fuel", default="1000000000000")
ap.add_argument("--out", required=True)
args = ap.parse_args()

lanes = args.lanes.split(",")
shapes = args.shapes.split(",")
forms = args.forms.split(",")
env = dict(os.environ, BROILER_MEASURE="1")

log = open(args.out + ".log", "w", encoding="utf-8", newline="\n")


def say(line):
    print(line, flush=True)
    log.write(line + "\n")
    log.flush()


say("cmd " + " ".join(sys.argv))
say("started " + time.strftime("%Y-%m-%dT%H:%M:%S"))

rows = []
for rep in range(-args.warmups, args.reps):
    for shape in shapes:
        for lane in lanes:
            for form in forms:
                exe = HERE / "lanes" / lane / "Broiler.VM.Composition.JavaScript.Cli.exe"
                cmd = [str(exe), "--wall", args.wall, "--fuel", args.fuel]
                if form == "native":
                    cmd += ["--native", "x86-64-win64"]
                cmd.append(str(SHAPES_DIR / f"{shape}.js"))
                t0 = time.perf_counter()
                p = subprocess.run(cmd, capture_output=True, text=True, env=env)
                wall = (time.perf_counter() - t0) * 1000
                row = {"rep": rep, "counted": rep >= 0, "shape": shape, "lane": lane,
                       "form": form, "exit": p.returncode, "value": p.stdout.strip(),
                       "process_ms": round(wall, 1)}
                for key, value in re.findall(r"(\w+)=([0-9.]+)", p.stderr):
                    row[key] = float(value)
                if p.returncode != 0:
                    row["stderr"] = p.stderr[-400:]
                rows.append(row)
                say(json.dumps(row))

say("finished " + time.strftime("%Y-%m-%dT%H:%M:%S"))

cells = {}
for row in rows:
    if row["counted"]:
        cells.setdefault((row["shape"], row["form"], row["lane"]), []).append(row)

summary = {}
for (shape, form, lane), rs in cells.items():
    inv = [r["invoke_ms"] for r in rs]
    proc = [r["process_ms"] for r in rs]
    summary[f"{shape}|{form}|{lane}"] = {
        "n": len(rs),
        "invoke_median": round(statistics.median(inv), 1),
        "invoke_min": round(min(inv), 1),
        "invoke_max": round(max(inv), 1),
        "invoke_spread": round(max(inv) - min(inv), 1),
        "process_median": round(statistics.median(proc), 1),
        "process_min": round(min(proc), 1),
        "process_max": round(max(proc), 1),
        "process_spread": round(max(proc) - min(proc), 1),
        "values": sorted({r["value"] for r in rs}),
        "exits": sorted({r["exit"] for r in rs}),
    }

verdict = {}
for shape in shapes:
    for form in forms:
        base = summary.get(f"{shape}|{form}|base")
        credit = summary.get(f"{shape}|{form}|credit")
        aa = summary.get(f"{shape}|{form}|aa")
        if not (base and credit and aa):
            continue
        for metric in ("invoke", "process"):
            gap = base[metric + "_median"] - credit[metric + "_median"]
            # The A/A lane's spread: credit and aa are the same build, so the difference between
            # their medians and the width of each of their own samples is what the machine alone
            # moves a cell by.
            aa_spread = max(
                abs(credit[metric + "_median"] - aa[metric + "_median"]),
                credit[metric + "_spread"],
                aa[metric + "_spread"])
            verdict[f"{shape}|{form}|{metric}"] = {
                "base_median": base[metric + "_median"],
                "credit_median": credit[metric + "_median"],
                "aa_median": aa[metric + "_median"],
                "ratio_base_over_credit": round(
                    base[metric + "_median"] / credit[metric + "_median"], 3),
                "gap_ms": round(gap, 1),
                "aa_spread_ms": round(aa_spread, 1),
                "credit_at_or_below_base": credit[metric + "_median"] <= base[metric + "_median"],
                "gap_exceeds_aa_spread": gap > aa_spread,
            }

pathlib.Path(args.out + ".json").write_text(
    json.dumps({"cmd": " ".join(sys.argv), "rows": rows, "summary": summary,
                "verdict": verdict}, indent=1),
    encoding="utf-8", newline="\n")

for key in sorted(verdict):
    v = verdict[key]
    say(f"VERDICT {key} base={v['base_median']} credit={v['credit_median']} aa={v['aa_median']} "
        f"ratio={v['ratio_base_over_credit']} gap={v['gap_ms']} aa_spread={v['aa_spread_ms']} "
        f"below={v['credit_at_or_below_base']} gap_exceeds_aa_spread={v['gap_exceeds_aa_spread']}")
log.close()
