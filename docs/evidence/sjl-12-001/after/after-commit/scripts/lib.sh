# Shared by the WP7.2 drivers (a copy of WP4.7's lib.sh with W changed, nothing else): the idle-machine gate and the run wrapper.
W=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7
R=$W/runs
ST=$R/status.tsv
LOADLOG=$R/load.tsv
GATELOG=$R/gate.log
CMDLOG=$R/commands.log
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet

utc() { date -u +%Y-%m-%dT%H:%M:%SZ; }

# Every process whose command is dotnet or python, one per line (the gate runs in bash, so none
# of these is the gate itself).
others() {
  ps -eo pid=,comm=,args= | awk '$2 ~ /^(dotnet|python[0-9.]*)$/ { print }'
}

# The brief: before each run record the load average, nproc, the SDK and the OS; if the load
# average (1-minute) is above 1.0 or any other dotnet/python process is running, poll every
# 2 minutes, up to 30 minutes, and record it; never kill another process.
gate() {
  local name=$1 waited=0 load procs
  # let the previous run's load decay before the first look
  sleep 60
  while :; do
    load=$(cut -d' ' -f1 /proc/loadavg)
    procs=$(others)
    if awk -v l="$load" 'BEGIN { exit !(l <= 1.0) }' && [ -z "$procs" ]; then
      echo "$(utc) gate $name: idle (load $(cat /proc/loadavg); no dotnet/python process) after ${waited}s of waiting" >> $GATELOG
      break
    fi
    echo "$(utc) gate $name: NOT idle: load $(cat /proc/loadavg); processes: ${procs:-none}" >> $GATELOG
    if [ $waited -ge 1800 ]; then
      echo "$(utc) gate $name: waited 30 minutes; proceeding NOT idle" >> $GATELOG
      break
    fi
    sleep 120; waited=$((waited + 120))
  done
  {
    echo "=== $name"
    echo "time: $(utc)"
    echo "loadavg: $(cat /proc/loadavg)"
    echo "nproc: $(nproc)"
    echo "os: $(. /etc/os-release; echo "$PRETTY_NAME"); $(uname -srm)"
    echo "sdk (dotnet --info, head):"
    dotnet --info 2>&1 | head -12 | sed 's/^/  /'
  } > $R/$name.pre.txt
}

# run NAME CWD CMD... : gate, then time the command with timed.py (LANG and LC_* stripped,
# TZ=UTC), recording the load just before and just after.
run() {
  local name=$1 cwd=$2; shift 2
  gate $name
  echo "$(utc) $name: cd $cwd && env -u LANG TZ=UTC $*" >> $CMDLOG
  printf '%s\tbefore\t%s\t%s\n' "$name" "$(utc)" "$(cat /proc/loadavg)" >> $LOADLOG
  (cd "$cwd" && env -u LANG TZ=UTC python3 -I $W/scripts/timed.py $ST $name $R/$name.out $R/$name.err "$@")
  printf '%s\tafter\t%s\t%s\n' "$name" "$(utc)" "$(cat /proc/loadavg)" >> $LOADLOG
  echo "$(utc) $name: done: $(tail -1 $ST)" >> $GATELOG
}
