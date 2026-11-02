#!/bin/bash
# Usage: run-vm.sh [--form bytecode|native] <rows-dir> <probe name>...
# SJL-17.1's Broiler.VM harness, SJL-9.1's form (bundle sjl-9-001, runs/vm/run-vm.sh): builds
# <rows-dir>/built/<name>.js (lib/prelude.js + <rows-dir>/probes/<name>.js + lib/epilogue.js) and
# vm262/test/sjl17/<name>.js beside this script (the same program as a raw test262 test), runs it once
# through the conformance composition root of the copy at 83290f9 built in Release, and writes
# <rows-dir>/runs/<name>.<tag>.raw.txt (the command, the revision, the root's whole transcript and its
# exit) and <name>.<tag>.txt (the probe's lines, or the verdict line when the program ended before
# printing them). <tag> is vm (bytecode) or vm-native (--form native, x86-64-sysv). Fuel 200,000,000,000
# unless the probe's first line says "// vm-fuel: N"; wall clock 600,000 ms. No LANG, LC_ALL or
# LANGUAGE; TZ=UTC. The test262 harness files are copied in from the read-only checkout and not retained.
set -u
H=$(cd "$(dirname "$0")" && pwd)
VM_ROOT=${VM_ROOT:-/home/user/vm-s17p}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
form=bytecode tag=vm
while [ $# -gt 0 ]; do
  case "$1" in
    --form) form=$2; shift 2 ;;
    *) break ;;
  esac
done
case "$form" in bytecode) ;; native) tag=vm-native ;; *) echo "form must be bytecode or native" >&2; exit 2 ;; esac
ROWS=$(cd "$1" && pwd); shift
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
V=$H/vm262
mkdir -p "$ROWS/built" "$ROWS/runs" "$V/test/sjl17"
[ -d "$V/harness" ] || cp -r "$TEST262_ROOT/harness" "$V/harness"
[ -f "$V/features.txt" ] || cp "$TEST262_ROOT/features.txt" "$V/features.txt"
revision="Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
for name in "$@"; do
  fuel=$(sed -n '1s#^// vm-fuel: \([0-9]*\)$#\1#p' "$ROWS/probes/$name.js")
  fuel=${fuel:-200000000000}
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  { printf '/*---\ndescription: SJL-17.1 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat "$ROWS/built/$name.js"; } > "$V/test/sjl17/$name.js"
  cmd="dotnet $DLL --test262 vm262 --test test/sjl17/$name.js --form $form --fuel $fuel --wall 600000"
  raw=$ROWS/runs/$name.$tag.raw.txt
  {
    echo "# command (from harness/, VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd"
    echo "# revision: $revision"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$raw"
  ( cd "$H" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd >> "$raw" 2>&1; echo "exit=$?" >> "$raw" )
  sed -i "s#$H/#<harness>/#g; s#$ROWS/#<rows>/#g" "$raw"
  python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
if a >= 0 and b > a:
    sys.stdout.write(t[a+5:b] + "\n")
else:
    lines = [l for l in t.splitlines() if "test/sjl17/" in l and not l.startswith("#") and not l.startswith("coverage")]
    sys.stdout.write("NO OUTPUT; the verdict line: " + (lines[0] if lines else "(none)") + "\n")
' "$raw" > "$ROWS/runs/$name.$tag.txt"
done
