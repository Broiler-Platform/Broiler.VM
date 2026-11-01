#!/bin/bash
# Usage: run-vm-t262.sh [--form bytecode|native] <rows-dir> <label> <file listing test262 paths>
# SJL-9.1's run-vm-t262.sh (bundle sjl-9-001) at the copy at 83290f9: the listed files of the read-only
# test262 checkout at ccaac100, every variant, through the conformance root; fuel 200,000,000,000 and
# wall 600,000 ms per variant; no LANG, LC_ALL or LANGUAGE; TZ=UTC. Writes <rows-dir>/runs/<label>.t262-<tag>.raw.txt.
set -u
VM_ROOT=${VM_ROOT:-/home/user/vm-s17p}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
form=bytecode tag=vm
[ "$1" = --form ] && { form=$2; shift 2; }
[ "$form" = native ] && tag=vm-native
ROWS=$(cd "$1" && pwd); label=$2; list=$3
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
tests=; while read -r p; do tests="$tests --test $p"; done < "$list"
cmd="dotnet $DLL --test262 $TEST262_ROOT$tests --form $form --fuel 200000000000 --wall 600000"
out=$ROWS/runs/$label.t262-$tag.raw.txt
{ echo "# command (VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 1800 dotnet <conformance root> --test262 $TEST262_ROOT --test <each path of the listing> --form $form --fuel 200000000000 --wall 600000"
  echo "# revision: Broiler.VM $(git -C $VM_ROOT rev-parse HEAD); test262 $(git -C $TEST262_ROOT rev-parse HEAD)"; } > "$out"
( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 1800 $cmd >> "$out" 2>&1; echo "exit=$?" >> "$out" )
