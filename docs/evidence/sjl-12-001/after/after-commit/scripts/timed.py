"""Run a command, write its stdout/stderr to files, and append one status line:
name, exit code (negative = signal), wall seconds, max RSS MB of the child, start and end (UTC).
The child gets this process's environment minus LANG and every LC_* variable: Python's
locale coercion (PEP 538) adds LC_CTYPE, which the calling shell did not set.
(WP4.7's copy of SJL-12.4's integration wrapper, integ-out/scripts/timed.py, with the start time added.)"""
import os, sys, time, subprocess
status, name, out, err = sys.argv[1:5]
cmd = sys.argv[5:]
env = {k: v for k, v in os.environ.items() if k != "LANG" and not k.startswith("LC_")}
began = time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime())
start = time.monotonic()
with open(out, "wb") as o, open(err, "wb") as e:
    p = subprocess.Popen(cmd, stdout=o, stderr=e, env=env)
    _, st, ru = os.wait4(p.pid, 0)
wall = time.monotonic() - start
rc = os.waitstatus_to_exitcode(st)
with open(status, "a") as s:
    s.write(f"{name}\trc={rc}\twall={wall:.1f}s\tmaxrss={ru.ru_maxrss // 1024}MB\tstarted={began}"
            f"\tended={time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime())}\n")
sys.exit(0)
