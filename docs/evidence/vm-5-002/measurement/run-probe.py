#!/usr/bin/env python3
"""E12: run the reflection probe over one build's Broiler.VM.Runtime, one mode per process.

python run-probe.py <variant>[,<variant>] <mode>[,<mode>]

Variants are named by build: base (f127d92), c2 (e117162, the pre-admission table without the
execution-context resolution cache) and c3 (the head, both). Each is that tree's Broiler.VM.Runtime
output directory, which carries Broiler.VM.Abstractions.dll beside it.
"""
import pathlib
import subprocess
import sys
import time

OUT = pathlib.Path(__file__).resolve().parent
PROBE = pathlib.Path(
    "C:/Users/proof/AppData/Local/Temp/claude/D--Broiler-VM/"
    "cbafeab9-f73a-434e-9481-38837a0bebe7/scratchpad/impl/measure/probe2/"
    "bin/Release/net10.0/Probe.exe")
BINS = {
    "base": "D:/Broiler.VM-base/src/Broiler.VM.Runtime/bin/Release/net10.0",
    "c2": "D:/Broiler.VM-c2/src/Broiler.VM.Runtime/bin/Release/net10.0",
    "c3": "D:/Broiler.VM/src/Broiler.VM.Runtime/bin/Release/net10.0",
}

for mode in sys.argv[2].split(","):
    for variant in sys.argv[1].split(","):
        started = time.perf_counter()
        p = subprocess.run([str(PROBE), BINS[variant], mode], capture_output=True, text=True)
        target = OUT / f"probe-{mode}-{variant}.txt"
        header = (f"# probe variant={variant} mode={mode} bin={BINS[variant]}\n"
                  f"# started={time.strftime('%Y-%m-%dT%H:%M:%S')}\n")
        target.write_text(header + p.stdout + p.stderr, encoding="utf-8", newline="\n")
        print(f"{variant} {mode} exit={p.returncode} "
              f"seconds={time.perf_counter() - started:.1f} -> {target.name}", flush=True)
