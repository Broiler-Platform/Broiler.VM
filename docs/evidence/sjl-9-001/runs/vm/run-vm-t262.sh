#!/bin/bash
# Usage: run-vm-t262.sh [--admitted] [--form bytecode|native] [--tz ZONE] <rows-dir> <label> <test262 path>...
# Runs test262 files (paths relative to the checkout, e.g. test/intl402/Locale/prototype/getCalendars/name.js)
# of the read-only checkout at ccaac100 through Broiler.VM's conformance root, every variant each file
# has, and writes <rows-dir>/runs/<label>.t262-<tag>.raw.txt (the command, the revision, the transcript,
# the exit). <tag> as run-vm.sh's. --admitted runs the copy whose suite-features list admits canonical-tz,
# Intl.Locale-info and Intl.NumberFormat-v3, so a file claiming one of them is scored, not skipped.
# The allowances are run-vm.sh's: fuel 200,000,000,000, wall 600,000 ms per variant. The conformance
# root reads the checkout and writes nothing into it. No LANG, LC_ALL or LANGUAGE; TZ=UTC unless --tz.
set -u
VM_ROOT=/home/user/s9-vm
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
form=bytecode tz=UTC tag=vm
while [ $# -gt 0 ]; do
  case "$1" in
    --admitted) VM_ROOT=/home/user/s9-vm-admitted; tag=vm-admitted; shift ;;
    --form) form=$2; shift 2 ;;
    --tz) tz=$2; shift 2 ;;
    *) break ;;
  esac
done
[ $# -ge 3 ] || { echo "usage: run-vm-t262.sh [--admitted] [--form bytecode|native] [--tz ZONE] <rows-dir> <label> <test262 path>..." >&2; exit 2; }
case "$form" in bytecode) ;; native) tag=$tag-native ;; *) echo "form must be bytecode or native" >&2; exit 2 ;; esac
[ "$tz" = UTC ] || tag=$tag.${tz//\//_}
ROWS=$(cd "$1" && pwd); label=$2; shift 2
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
mkdir -p "$ROWS/runs"
tests=
for p in "$@"; do tests="$tests --test $p"; done
cmd="dotnet $DLL --test262 $TEST262_ROOT$tests --form $form --fuel 200000000000 --wall 600000"
revision="Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
if [ "$VM_ROOT" = /home/user/s9-vm-admitted ]; then
  revision="$revision, working tree = that commit plus admitted-proposals.patch (sha256 $(git -C $VM_ROOT diff | sha256sum | cut -c1-64))"
fi
out=$ROWS/runs/$label.t262-$tag.raw.txt
{
  echo "# command (VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 1800 $cmd"
  echo "# revision: $revision; test262 $(git -C $TEST262_ROOT rev-parse HEAD)"
} > "$out"
( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 1800 $cmd >> "$out" 2>&1; echo "exit=$?" >> "$out" )
