"""The machine's state immediately before and after the timing runs of VM-5-002's re-collection.

    python machine-state.py <before|after> <file>

Writes, LF: identity lines (identity.py), the processor, memory, clock and load, the power scheme, the
SDK and Python, and every process that has used at least 60 CPU-seconds, each with its process id, so
that a process named in both snapshots can be matched by id and its CPU time between them read exactly
(the first collection's snapshots listed eight names each without ids, and its load could only be
bounded from below).
"""
import json
import pathlib
import subprocess
import sys

HERE = pathlib.Path(__file__).resolve().parent
which, target = sys.argv[1], pathlib.Path(sys.argv[2])


def ps(script):
    p = subprocess.run(["powershell", "-NoProfile", "-NonInteractive", "-Command", script],
                       capture_output=True, text=True, encoding="utf-8", errors="replace")
    return p.stdout.strip()


ident = subprocess.run([sys.executable, str(HERE / "identity.py"), "--build", "machine state " + which,
                        "--tree", "D:/Broiler.VM"], capture_output=True, text=True)
if ident.returncode != 0:
    sys.stdout.write(ident.stdout)
    sys.exit(97)
cpu = json.loads(ps("Get-CimInstance Win32_Processor | Select-Object Name,NumberOfCores,NumberOfLogicalProcessors,"
                    "MaxClockSpeed,CurrentClockSpeed,LoadPercentage | ConvertTo-Json -Compress"))
os_ = json.loads(ps("Get-CimInstance Win32_OperatingSystem | Select-Object Caption,BuildNumber,"
                    "TotalVisibleMemorySize,FreePhysicalMemory,CSName | ConvertTo-Json -Compress"))
procs = json.loads(ps("[Console]::OutputEncoding=[Text.Encoding]::UTF8; Get-Process | Where-Object { $_.CPU -ge 60 } | "
                      "Sort-Object CPU -Descending | Select-Object Id,ProcessName,CPU,WorkingSet64,StartTime | "
                      "ConvertTo-Json -Compress") or "[]")
if isinstance(procs, dict):
    procs = [procs]
lines = [l[2:] for l in ident.stdout.splitlines()]
lines += [
    "host=%s" % os_["CSName"],
    "os=%s build %s" % (os_["Caption"], os_["BuildNumber"]),
    "cpu=%s" % cpu["Name"].strip(),
    "cores_physical=%s logical=%s maxclock_mhz=%s" % (cpu["NumberOfCores"], cpu["NumberOfLogicalProcessors"], cpu["MaxClockSpeed"]),
    "current_clock_mhz=%s load_pct=%s" % (cpu["CurrentClockSpeed"], cpu["LoadPercentage"]),
    "memory_total_kb=%s free_kb=%s" % (os_["TotalVisibleMemorySize"], os_["FreePhysicalMemory"]),
    "power_scheme=%s" % subprocess.run(["powercfg", "/getactivescheme"], capture_output=True, text=True,
                                       errors="replace").stdout.strip(),
    "dotnet=%s" % subprocess.run(["dotnet", "--version"], capture_output=True, text=True).stdout.strip(),
    "python=%s" % sys.version.split()[0],
    "processes_with_at_least_60_cpu_s_%s (id, name, cpu_s, working set MB, start time):" % which,
]
for p in procs:
    lines.append("  id=%s %s cpu_s=%.1f ws_mb=%.1f started=%s" % (
        p["Id"], p["ProcessName"], p["CPU"], p["WorkingSet64"] / 1048576, p.get("StartTime")))
target.parent.mkdir(parents=True, exist_ok=True)
target.write_bytes(("\n".join(lines) + "\n").encode("utf-8"))
print("\n".join(lines[:20]))
