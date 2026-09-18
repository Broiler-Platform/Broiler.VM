#!/usr/bin/env bash
# E13 of VM-5-002's re-collection: the fixture bench host in both trees, two pairs - the first base
# first, the second credit first - each run through run-logged.sh with --no-build, so the host and the
# runtime the identity lines hash are the ones that run. The trees were built before the timing runs
# began; nothing is built here.
#   run-bench.sh <outdir> <pair: 1|2>
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1; PAIR=$2
P=src/tests/Broiler.VM.Bench.Host; B=$P/bin/Release/net10.0
mkdir -p "$OUT/e13"
run() { # $1 build, $2 tree, $3 log
  "$HERE/run-logged.sh" "$OUT/e13/$3" "$1, bench host pair $PAIR" "$2" $B/Broiler.VM.Bench.Host.exe $B/Broiler.VM.Bench.Host.dll $B/Broiler.VM.Runtime.dll -- \
    dotnet run --project $P -c Release --no-build
  echo "$3 exit=$?"
}
if [ "$PAIR" = 1 ]; then
  run base D:/Broiler.VM-base bench-base.log
  run credit D:/Broiler.VM bench-credit.log
else
  run credit D:/Broiler.VM bench-credit-run2.log
  run base D:/Broiler.VM-base bench-base-run2.log
fi
