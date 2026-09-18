#!/usr/bin/env python3
"""Rule item 3c in VM-5-002's re-collection: the duration of the concurrent exactness test alone, five
runs, alternating builds, base first in each repetition.

The first collection's measurement/measure-t6.py, changed in three ways: the output directory is an
argument; before each run the identity lines of that run (identity.py, with the digests of the
runtime and test assemblies the run loads) are written to <out>.log, followed by the run's own row; and
each TRX file is kept under <outdir>/t6-trx. The duration read is still the test's own, from its TRX
result, and the trees are built before the timing runs, so every run is --no-build.

python measure-t6.py --outdir <dir> --reps 5
"""
import argparse
import json
import pathlib
import statistics
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

HERE = pathlib.Path(__file__).resolve().parent
TEST = "Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit"
TREES = {"base": "D:/Broiler.VM-base", "credit": "D:/Broiler.VM"}
TB = "src/tests/Broiler.VM.Contract.Tests/bin/Release/net10.0"

ap = argparse.ArgumentParser()
ap.add_argument("--outdir", required=True)
ap.add_argument("--reps", type=int, default=5)
ap.add_argument("--out", default="t6")
args = ap.parse_args()

out = pathlib.Path(args.outdir)
results_dir = out / "t6-trx"
results_dir.mkdir(parents=True, exist_ok=True)
log = open(out / f"{args.out}.log", "w", encoding="utf-8", newline="\n")
rows = []

for rep in range(args.reps):
    for build in ("base", "credit"):
        tree = TREES[build]
        ident = subprocess.run(
            [sys.executable, str(HERE / "identity.py"), "--build", f"{build}, T6 alone, repetition {rep}", "--tree", tree,
             "--bin", f"{tree}/{TB}/Broiler.VM.Runtime.dll", "--bin", f"{tree}/{TB}/Broiler.VM.Contract.Tests.dll"],
            capture_output=True, text=True)
        log.write(ident.stdout)
        log.flush()
        if ident.returncode != 0:
            sys.exit(97)
        name = f"{build}-r{rep}.trx"
        started = time.perf_counter()
        cmd = ["dotnet", "test", f"{tree}/src/tests/Broiler.VM.Contract.Tests", "-c", "Release", "--no-build",
               "--filter", f"FullyQualifiedName~{TEST}",
               "--logger", f"trx;LogFileName={name}",
               "--results-directory", str(results_dir)]
        p = subprocess.run(cmd, capture_output=True, text=True)
        process_ms = (time.perf_counter() - started) * 1000
        tree_xml = ET.parse(results_dir / name)
        ns = "{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}"
        results = tree_xml.getroot().findall(f".//{ns}UnitTestResult")
        assert len(results) == 1, f"{build} r{rep}: {len(results)} results"
        outcome = results[0].get("outcome")
        hours, minutes, seconds = results[0].get("duration").split(":")
        test_ms = ((int(hours) * 60 + int(minutes)) * 60 + float(seconds)) * 1000
        row = {"rep": rep, "build": build, "outcome": outcome,
               "test_ms": round(test_ms, 1), "process_ms": round(process_ms, 1),
               "exit": p.returncode}
        rows.append(row)
        log.write("# command=%s\n%s\n" % (" ".join(cmd), json.dumps(row)))
        log.flush()
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
(out / f"{args.out}.json").write_text(
    json.dumps({"cmd": " ".join(sys.argv), "test": TEST, "rows": rows,
                "summary": summary, "verdict": verdict}, indent=1),
    encoding="utf-8", newline="\n")
log.write("SUMMARY " + json.dumps(summary) + "\nVERDICT " + json.dumps(verdict) + "\n")
log.close()
print("SUMMARY " + json.dumps(summary))
print("VERDICT " + json.dumps(verdict))
