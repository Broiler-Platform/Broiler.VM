#!/bin/bash
# Waits for the machine to be otherwise idle (no process of the CI-optimisation workflow, 1-minute load
# average below 1.0, twice in a row 30 s apart), stops leftover build servers, records the machine's state,
# then runs bundle sjl-10-001's rerun and wall-race steps, sampling the process table throughout.
S=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl106/vm-rerun
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet TMPDIR=$S/tmp
T=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl106/vm-local/tree-96876432
B=/home/user/vm-base-99c9c9f
SUITE=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl106/preimport-vm/suite/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
LOG=$S/logs/wait-and-run.log
other() { ps -eo args | grep -E "ci-opt|/home/user/js-ci|/home/user/vm-ci" | grep -v grep; }
idle() { [ -z "$(other)" ] && awk '{exit !($1<1.0)}' /proc/loadavg; }
deadline=$(( $(date +%s) + ${WAIT_SECONDS:-6000} ))
wait_idle() {
  while true; do
    if idle; then sleep 30; idle && return 0; fi
    [ "$(date +%s)" -ge "$deadline" ] && { echo "== no idle machine within the wait, $(date -u +%FT%TZ)" >> $LOG; exit 3; }
    sleep 30
  done
}
echo "== waiting from $(date -u +%FT%TZ), loadavg $(cat /proc/loadavg)" >> $LOG
wait_idle
echo "== idle at $(date -u +%FT%TZ), loadavg $(cat /proc/loadavg); dotnet build-server shutdown" >> $LOG
dotnet build-server shutdown >> $LOG 2>&1
wait_idle
stamp=$(date -u +%FT%TZ)
echo "== idle again at $stamp, loadavg $(cat /proc/loadavg); starting" >> $LOG
{ echo "# $stamp"; cat /proc/loadavg; } > $S/idle/start-loadavg.txt
{ echo "# $stamp"; ps -eo pid,ppid,etime,pcpu,args; } > $S/idle/start-ps.txt
( while true; do echo "## $(date -u +%FT%TZ) $(cat /proc/loadavg)"; ps -eo pid,ppid,etime,pcpu,args --sort=-pcpu | head -15; other | sed 's/^/OTHER: /'; sleep 5; done ) > $S/idle/samples.txt 2>&1 &
sampler=$!
cd /home/user/vm-sjl106
python3 docs/evidence/sjl-10-001/collect-area.py --tree $T --base-tree $B --suite $SUITE --out docs/evidence/sjl-10-001 \
  --only rerun --only wall-race > $S/logs/collector-rerun.log 2>&1
code=$?
kill $sampler
stamp=$(date -u +%FT%TZ)
{ echo "# $stamp"; cat /proc/loadavg; } > $S/idle/end-loadavg.txt
{ echo "# $stamp"; ps -eo pid,ppid,etime,pcpu,args; } > $S/idle/end-ps.txt
echo "== collector exit $code at $stamp, loadavg $(cat /proc/loadavg)" >> $LOG
