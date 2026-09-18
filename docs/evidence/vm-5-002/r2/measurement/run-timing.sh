#!/usr/bin/env bash
# The timing runs of VM-5-002's re-collection, in the order the remedy's design fixes, on a machine with
# no build running: the machine's state before; E10 over three lanes snapshotted first; E12 (trace and
# stress on credit, then bench on base and credit, concurrent-bench on base and credit, concurrent-ambient
# on base, C2' and credit); E13 pair 1 then pair 2; the T6 timing; the machine's state after. Nothing is
# built. Every step checks the branch through its own identity lines, and this driver checks it again,
# and that no build, test or conformance process is running, before each step, writing both to its log.
#   run-timing.sh <outdir> [step ...]      steps: before e10 e12 e13 t6 after (default: all, in order)
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1; shift
STEPS="${*:-before e10 e12 e13 t6 after}"
LOG=$OUT/timing-driver.log
mkdir -p "$OUT"
check() {
  local branch; branch=$(git -C D:/Broiler.VM branch --show-current)
  local busy; busy=$(powershell -NoProfile -NonInteractive -Command "Get-CimInstance Win32_Process | Where-Object { \$_.Name -match '^(dotnet|MSBuild|VBCSCompiler|testhost|vstest\.console|Broiler\.VM\..*)\.exe$' } | ForEach-Object { \$c = [string]\$_.CommandLine; \$n = [Math]::Min(90, \$c.Length); \$_.Name + '#' + \$_.ProcessId + ' cpu_s=' + [Math]::Round((\$_.KernelModeTime + \$_.UserModeTime) / 1e7, 1) + ' ' + \$c.Substring(0, \$n) }" | tr -d '\r' | paste -sd';')
  echo "# $(date -Iseconds) before $1: branch=$branch head=$(git -C D:/Broiler.VM rev-parse HEAD) build-test-host-processes=[$busy]" >> "$LOG"
  if [ "$branch" != claude/fuel-credit-block-steps ]; then echo "STOP: branch is $branch" | tee -a "$LOG"; exit 97; fi
}
for step in $STEPS; do
  check "$step"
  case $step in
    before) python "$HERE/machine-state.py" before "$OUT/machine-state-before.txt" >> "$LOG" 2>&1 ;;
    after) python "$HERE/machine-state.py" after "$OUT/machine-state-after.txt" >> "$LOG" 2>&1 ;;
    e10)
      mkdir -p "$OUT/e10"
      python "$HERE/make-lanes.py" D:/broiler-arms/e10-lanes "$OUT/e10/lanes.txt" >> "$LOG" 2>&1 || { echo "lanes failed" | tee -a "$LOG"; exit 1; }
      python "$HERE/measure-shapes.py" --lanes-dir D:/broiler-arms/e10-lanes --out "$OUT/e10/e10-shapes" > /dev/null 2>> "$LOG"
      echo "# e10 exit=$?" >> "$LOG"; grep '^VERDICT' "$OUT/e10/e10-shapes.log" >> "$LOG" ;;
    e12)
      python "$HERE/run-probe.py" "$OUT/e12" credit trace,stress >> "$LOG" 2>&1
      python "$HERE/run-probe.py" "$OUT/e12" base,credit bench >> "$LOG" 2>&1
      python "$HERE/run-probe.py" "$OUT/e12" base,credit concurrent-bench >> "$LOG" 2>&1
      python "$HERE/run-probe.py" "$OUT/e12" base,c2r,credit concurrent-ambient >> "$LOG" 2>&1 ;;
    e13)
      bash "$HERE/run-bench.sh" "$OUT" 1 >> "$LOG" 2>&1
      bash "$HERE/run-bench.sh" "$OUT" 2 >> "$LOG" 2>&1 ;;
    t6) python "$HERE/measure-t6.py" --outdir "$OUT/e13" --reps 5 >> "$LOG" 2>&1 ;;
  esac
  echo "# $(date -Iseconds) after $step" >> "$LOG"
done
echo "TIMING DONE: $STEPS" | tee -a "$LOG"
