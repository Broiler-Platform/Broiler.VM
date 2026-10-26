#!/bin/bash
# Usage: bisect-vm-fuel.sh <rows-dir> <probe name> [bytecode|native]
# The smallest fuel allowance under which <rows-dir>/probes/<name>.js completes in Broiler.VM's conformance root
# (the base copy at cd1928d), by bisection; each step is one process of harness/run-vm.sh's command with the probe's
# first line set to "// vm-fuel: N" (a copy of the probe in the scratch rows directory ../tmp/u-bisect). A step
# "completes" when the root reports the probe's lines, and is "exhausted" when it reports the fuel spent. Every
# step and the result go to <rows-dir>/runs/<name>.fuel-bisect[-native].txt. (rows-MP's script, its scratch
# directory renamed.)
set -u
H=$(cd "$(dirname "$0")/../../harness" && pwd)
ROWS=$(cd "$1" && pwd); name=$2; form=${3:-bytecode}
B=$(dirname "$ROWS")/tmp/u-bisect
mkdir -p "$B/probes"
out=$ROWS/runs/$name.fuel-bisect$([ "$form" = native ] && echo -native).txt
echo "# bisect-vm-fuel.sh over probes/$name.js (sha256 $(sha256sum "$ROWS/probes/$name.js" | cut -c1-64)), form $form, Broiler.VM $(git -C /home/user/s9-vm rev-parse HEAD); each step: run-vm.sh's command with --fuel N" > "$out"
step() {
  { echo "// vm-fuel: $1"; tail -n +2 "$ROWS/probes/$name.js"; } > "$B/probes/$name.js"
  bash "$H/run-vm.sh" --form "$form" "$B" "$name" > /dev/null 2>&1
  local tag=vm; [ "$form" = native ] && tag=vm-native
  if grep -q 'OUT<<' "$B/runs/$name.$tag.raw.txt"; then echo completes; elif grep -q '^exhausted' "$B/runs/$name.$tag.raw.txt"; then echo exhausted; else echo other; fi
}
lo=1; hi=200000000000
r=$(step $lo); echo "fuel $lo: $r" >> "$out"; [ "$r" = exhausted ] || { echo "result: the low end does not exhaust" >> "$out"; exit 1; }
r=$(step $hi); echo "fuel $hi: $r" >> "$out"; [ "$r" = completes ] || { echo "result: the high end does not complete" >> "$out"; exit 1; }
while [ $((hi - lo)) -gt 1 ]; do
  mid=$(( (lo + hi) / 2 )); r=$(step $mid); echo "fuel $mid: $r" >> "$out"
  if [ "$r" = completes ]; then hi=$mid; elif [ "$r" = exhausted ]; then lo=$mid; else echo "result: an unexpected verdict" >> "$out"; exit 1; fi
done
echo "result: smallest completing fuel $hi (exhausted at $lo)" >> "$out"
