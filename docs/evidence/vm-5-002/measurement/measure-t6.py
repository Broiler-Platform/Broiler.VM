#!/usr/bin/env python3
"""Rule item 3c: the duration of the concurrent exactness test alone, five runs, alternating builds.

The test is the one that spends a shared runtime ceiling to the unit from two instances on two
threads. It is run on its own, once per build per repetition, base first, so that a drift in the
machine moves both builds together. The duration read is the test's own, taken from the TRX result
rather than from the process, so neither the host's startup nor the test discovery is in it.

python measure-t6.py --reps 5
"""
import argparse
import json
import pathlib
import re
import statistics
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

OUT = pathlib.Path(__file__).resolve().parent
TEST = "Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit"
TREES = {
    "base": "D:/Broiler.VM-base/src/tests/Broiler.VM.Contract.Tests",
    "credit": "D:/Broiler.VM/src/tests/Broiler.VM.Contract.Tests",
}

ap = argparse.ArgumentParser()
ap.add_argument("--reps", type=int, default=5)
ap.add_argument("--out", default="t6")
args = ap.parse_args()

results_dir = OUT / "t6-trx"
results_dir.mkdir(exist_ok=True)
rows = []

for rep in range(args.reps):
    for build in ("base", "credit"):
        name = f"{build}-r{rep}.trx"
        started = time.perf_counter()
        p = subprocess.run(
            ["dotnet", "test", TREES[build], "-c", "Release", "--no-build",
             "--filter", f"FullyQualifiedName~{TEST}",
             "--logger", f"trx;LogFileName={name}",
             "--results-directory", str(results_dir)],
            capture_output=True, text=True)
        process_ms = (time.perf_counter() - started) * 1000
        tree = ET.parse(results_dir / name)
        ns = "{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}"
        results = tree.getroot().findall(f".//{ns}UnitTestResult")
        assert len(results) == 1, f"{build} r{rep}: {len(results)} results"
        outcome = results[0].get("outcome")
        hours, minutes, seconds = results[0].get("duration").split(":")
        test_ms = ((int(hours) * 60 + int(minutes)) * 60 + float(seconds)) * 1000
        row = {"rep": rep, "build": build, "outcome": outcome,
               "test_ms": round(test_ms, 1), "process_ms": round(process_ms, 1),
               "exit": p.returncode}
        rows.append(row)
        print(json.dumps(row), flush=True)

summary = {}
for build in ("base", "credit"):
    ms = [r["test_ms"] for r in rows if r["build"] == build]
    summary[build] = {"median_ms": round(statistics.median(ms), 1),
                      "min_ms": round(min(ms), 1), "max_ms": round(max(ms), 1), "n": len(ms)}

verdict = {
    "base_median_ms": summary["base"]["median_ms"],
    "credit_median_ms": summary["credit"]["median_ms"],
    "credit_at_or_below_base": summary["credit"]["median_ms"] <= summary["base"]["median_ms"],
    "outcomes": sorted({r["outcome"] for r in rows}),
}
(OUT / f"{args.out}.json").write_text(
    json.dumps({"cmd": " ".join(sys.argv), "test": TEST, "rows": rows,
                "summary": summary, "verdict": verdict}, indent=1),
    encoding="utf-8", newline="\n")
print("SUMMARY " + json.dumps(summary))
print("VERDICT " + json.dumps(verdict))
