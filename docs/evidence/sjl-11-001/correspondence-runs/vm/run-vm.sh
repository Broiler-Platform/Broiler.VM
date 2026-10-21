#!/bin/bash
# Usage: run-vm.sh <probe name>...
# SJL-11.1's Broiler.VM harness, SJL-10.1's form (sjl-10-001/correspondence-runs/core/run-probe.sh):
# builds built/<name>.js (probes/prelude.js + probes/<name>.js + probes/epilogue.js) and
# vm262/test/sjl11/<name>.js (the same program as a raw test262 test), runs it once through the
# conformance composition root, and writes <name>.vm.raw.txt (the command, the revision and the
# root's whole transcript) and <name>.vm.txt (the probe's lines, or the verdict line when the
# program ended before printing them).
# VM_ROOT names a read-only checkout at Broiler.VM aaa39fb built in Release with
# --disable-build-servers; TEST262_ROOT a test262 checkout at the common pin ccaac100, whose
# harness/ is copied into vm262/ for the run (a raw test reads none of it) and is not retained.
# The fuel allowance is 200,000,000,000 unless the probe's first line says "// vm-fuel: N";
# the wall-clock allowance is 600,000 ms. No LANG, LC_ALL or LANGUAGE; TZ=UTC.
set -u
R=$(cd "$(dirname "$0")" && pwd)
V=$R/vm262
VM_ROOT=${VM_ROOT:-/home/user/vm-s11corr}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
mkdir -p $R/built $V/test/sjl11
[ -d $V/harness ] || cp -r $TEST262_ROOT/harness $V/harness
[ -f $V/features.txt ] || cp $TEST262_ROOT/features.txt $V/features.txt
for name in "$@"; do
  fuel=$(sed -n '1s#^// vm-fuel: \([0-9]*\)$#\1#p' $R/probes/$name.js)
  fuel=${fuel:-200000000000}
  cat $R/probes/prelude.js $R/probes/$name.js $R/probes/epilogue.js > $R/built/$name.js
  { printf '/*---\ndescription: SJL-11.1 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat $R/built/$name.js; } > $V/test/sjl11/$name.js
  cmd="dotnet $DLL --test262 vm262 --test test/sjl11/$name.js --fuel $fuel --wall 600000"
  {
    echo "# command (from correspondence-runs/vm/, VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd"
    echo "# revision: Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
  } > $R/$name.vm.raw.txt
  ( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd >> $R/$name.vm.raw.txt 2>&1; echo "exit=$?" >> $R/$name.vm.raw.txt )
  python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
if a >= 0 and b > a:
    sys.stdout.write(t[a+5:b] + "\n")
else:
    lines = [l for l in t.splitlines() if "test/sjl11/" in l and not l.startswith("#") and not l.startswith("coverage")]
    sys.stdout.write("NO OUTPUT; the verdict line: " + (lines[0] if lines else "(none)") + "\n")
' $R/$name.vm.raw.txt > $R/$name.vm.txt
done
