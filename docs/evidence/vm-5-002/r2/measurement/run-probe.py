#!/usr/bin/env python3
"""E12 of VM-5-002's re-collection: run the first collection's reflection probe over one build's
Broiler.VM.Runtime, one mode per process.

python run-probe.py <outdir> <variant>[,<variant>] <mode>[,<mode>]

The first collection's measurement/run-probe.py, changed in three ways: each file begins with identity
lines written before the probe starts (so "started" is a start time, where the first collection's
header, written after the probe returned, gave a finish time), including the digests of the probe and of
the runtime it loads; the output directory is an argument; and the variants are the re-collection's:
base (f127d92), c2r (C2', the remedy head with VmExecutionScope.cs taken from e117162) and credit (the
remedy head, in the main checkout). The probe is the first collection's measurement/probe/Program.cs,
built unchanged outside the repository.
"""
import pathlib
import subprocess
import sys
import time

HERE = pathlib.Path(__file__).resolve().parent
PROBE_DIR = pathlib.Path(
    "C:/Users/proof/AppData/Local/Temp/claude/D--Broiler-VM/"
    "cbafeab9-f73a-434e-9481-38837a0bebe7/scratchpad/impl/measure/probe2/bin/Release/net10.0")
PROBE = PROBE_DIR / "Probe.exe"
TREES = {
    "base": "D:/Broiler.VM-base",
    "c2r": "D:/broiler-arms/c2r",
    "credit": "D:/Broiler.VM",
}
out = pathlib.Path(sys.argv[1])
out.mkdir(parents=True, exist_ok=True)

for mode in sys.argv[3].split(","):
    for variant in sys.argv[2].split(","):
        tree = TREES[variant]
        bin_dir = tree + "/src/Broiler.VM.Runtime/bin/Release/net10.0"
        ident = subprocess.run(
            [sys.executable, str(HERE / "identity.py"), "--build", "%s, probe %s" % (variant, mode), "--tree", tree,
             "--bin", bin_dir + "/Broiler.VM.Runtime.dll", "--bin", bin_dir + "/Broiler.VM.Abstractions.dll",
             "--bin", str(PROBE), "--bin", str(PROBE_DIR / "Probe.dll")],
            capture_output=True, text=True)
        if ident.returncode != 0:
            print(ident.stdout)
            sys.exit(97)
        started = time.perf_counter()
        p = subprocess.run([str(PROBE), bin_dir, mode], capture_output=True, text=True)
        target = out / f"probe-{mode}-{variant}.txt"
        header = ident.stdout + f"# probe variant={variant} mode={mode} bin={bin_dir}\n# command={PROBE} {bin_dir} {mode}\n"
        footer = f"# exit={p.returncode}\n# finished={time.strftime('%Y-%m-%dT%H:%M:%S')}\n"
        target.write_text(header + p.stdout + p.stderr + footer, encoding="utf-8", newline="\n")
        print(f"{variant} {mode} exit={p.returncode} seconds={time.perf_counter() - started:.1f} -> {target.name}", flush=True)
