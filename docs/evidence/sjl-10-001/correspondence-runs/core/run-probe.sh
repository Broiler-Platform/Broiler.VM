#!/bin/bash
# Usage: run-probe.sh <probe name>
# SJL-8's harness (sjl-8-001/correspondence-runs/tz/run-probe.sh), for SJL-10.1's core half. Builds
# built/<name>.js (Broiler.JS, and the reference) and vm262/test/sjl10/<name>.js (Broiler.VM, a raw
# test262 test), runs each once, and writes <name>.bjs.txt, <name>.vm.raw.txt, <name>.vm.txt,
# <name>.ref.txt and <name>.compare.txt.
# The reference is polyfill/lib of tc39/proposal-temporal at e8cc03fc (JSD-0053's pin), run as the
# retained dataset's README (src/tests/temporal/README.md) runs it: Node 22.22.0, from POLYFILL_DIR, a
# directory holding that lib and its three runtime dependencies (here big-integer 1.6.52, call-bind
# 1.0.9 and es-abstract 1.24.2, from the npm registry), lib/shim.mjs imported first, the program
# evaluated with an indirect eval and print bound to standard output.
# BJS_ROOT and VM_ROOT name read-only checkouts at Broiler.JS d3464413 and Broiler.VM 14072b0, each
# built in Release; TEST262_ROOT a test262 checkout at the common pin ccaac100, whose harness/ is
# copied into vm262/ for the run (raw tests read none of it) and is not retained.
set -u
R=$(cd "$(dirname "$0")" && pwd)
V=$R/vm262
BJS_ROOT=${BJS_ROOT:-/home/user/js-at-d3464413}
VM_ROOT=${VM_ROOT:-/home/user/vm-at-14072b0}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
name=$1
mkdir -p $R/built $V/test/sjl10
[ -d $V/harness ] || cp -r $TEST262_ROOT/harness $V/harness
cat $R/probes/prelude.js $R/probes/$name.js $R/probes/epilogue.js > $R/built/$name.js
{ printf '/*---\ndescription: SJL-10.1 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat $R/built/$name.js; } > $V/test/sjl10/$name.js
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 600 dotnet $BJS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll --script-host $R/built/$name.js > $R/$name.bjs.txt 2>&1; echo "exit=$?" >> $R/$name.bjs.txt )
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 dotnet $VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll --test262 $V --test test/sjl10/$name.js --fuel 200000000000 --wall 600000 > $R/$name.vm.raw.txt 2>&1 )
python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
sys.stdout.write(t[a+5:b] + "\n" if a >= 0 and b > a else "NO OUTPUT; see raw transcript\n")
' $R/$name.vm.raw.txt > $R/$name.vm.txt
POLYFILL_DIR=${POLYFILL_DIR:?set POLYFILL_DIR to a directory holding lib/ and node_modules/}
( cd $POLYFILL_DIR && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 600 node --js-regexp-duplicate-named-groups --input-type=module -e "
  await import('./lib/shim.mjs');
  const fs = await import('fs');
  globalThis.print = (s) => process.stdout.write(s + '\\n');
  (0, eval)(fs.readFileSync('$R/built/$name.js', 'utf8'));" > $R/$name.ref.txt 2>&1; echo "exit=$?" >> $R/$name.ref.txt )
python3 -I $R/sidebyside.py $R/$name.vm.txt $R/$name.bjs.txt $R/$name.ref.txt > $R/$name.compare.txt
