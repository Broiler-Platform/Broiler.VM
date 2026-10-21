#!/bin/bash
# Usage: bisect-vm-fuel.sh <probe name> <low> <high>
# SJL-11.1's fuel bisection (row B7): finds the smallest fuel allowance under which Broiler.VM's
# conformance root completes vm262/test/sjl11/<name>.js (which run-vm.sh wrote), by bisection
# between <low> (assumed exhausted) and <high> (checked to complete), each step one process with
# the same command as run-vm.sh but --fuel N. A step "completes" when the root's verdict line
# carries the probe's lines (OUT<<); it is "exhausted" when the verdict is exhausted on Fuel; any
# other verdict stops the bisection. Writes <name>.fuel-bisect.txt: the command, the revision,
# every step, and the result, then the run at the result and one below it.
set -u
R=$(cd "$(dirname "$0")" && pwd)
VM_ROOT=${VM_ROOT:-/home/user/vm-s11corr}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
name=$1 lo=$2 hi=$3
out=$R/$name.fuel-bisect.txt
step() { # fuel -> prints completed | exhausted | other:<line>
  local v
  v=$(cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 dotnet $DLL --test262 vm262 --test test/sjl11/$name.js --fuel $1 --wall 600000 2>&1 | grep "test/sjl11/$name.js \[raw\]" | grep -v '^#' | head -1)
  case "$v" in
    *'OUT<<'*) echo completed ;;
    exhausted*'Fuel:'*) echo exhausted ;;
    *) echo "other:$v" ;;
  esac
}
{
  echo "# command (from correspondence-runs/vm/, VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 dotnet $DLL --test262 vm262 --test test/sjl11/$name.js --fuel N --wall 600000"
  echo "# revision: Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
  echo "# bisection between $lo and $hi; each line: the allowance and the verdict"
} > $out
r=$(step $hi); echo "fuel $hi: $r" >> $out
[ "$r" = completed ] || { echo "result: none (the high bound did not complete)" >> $out; exit 1; }
r=$(step $lo); echo "fuel $lo: $r" >> $out
[ "$r" = exhausted ] || { echo "result: none (the low bound did not exhaust)" >> $out; exit 1; }
while [ $((hi - lo)) -gt 1 ]; do
  mid=$(( (lo + hi) / 2 ))
  r=$(step $mid); echo "fuel $mid: $r" >> $out
  case "$r" in
    completed) hi=$mid ;;
    exhausted) lo=$mid ;;
    *) echo "result: none (stopped at $mid)" >> $out; exit 1 ;;
  esac
done
echo "result: the smallest completing allowance is $hi ($lo exhausts)" >> $out
