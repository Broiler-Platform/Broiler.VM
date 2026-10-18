#!/bin/bash
# Runs Broiler.VM's retained Temporal dataset program (src/tests/temporal/temporal.js at 14072b0) unchanged
# in both engines, wrapped only to hand its answer back: Broiler.JS prints it, Broiler.VM's raw test262
# test throws it inside OUT<<...>>OUT. Compares each engine's 1,629 lines with the retained reference
# answer (temporal.polyfill-e8cc03fc.txt). Writes d6-retained-dataset.{bjs,vm}.txt (gzip-compressed
# afterwards), .vm.raw.txt, and .bjs.diff and .vm.diff against the reference. Same roots as run-probe.sh.
set -u
R=$(cd "$(dirname "$0")" && pwd)
V=$R/vm262
BJS_ROOT=${BJS_ROOT:-/home/user/js-at-d3464413}
VM_ROOT=${VM_ROOT:-/home/user/vm-at-14072b0}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
D=$VM_ROOT/src/tests/temporal
n=d6-retained-dataset
mkdir -p $R/built $V/test/sjl10
[ -d $V/harness ] || cp -r $TEST262_ROOT/harness $V/harness
{ echo "var __r ="; cat $D/temporal.js; echo "print(__r);"; } > $R/built/$n.js
{ printf '/*---\ndescription: SJL-10.1 retained dataset\nflags: [raw]\n---*/\nvar __r =\n'; cat $D/temporal.js; echo "throw new Error('OUT<<' + __r + '>>OUT');"; } > $V/test/sjl10/$n.js
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 600 dotnet $BJS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll --script-host $R/built/$n.js > $R/$n.bjs.txt 2>&1 )
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 dotnet $VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll --test262 $V --test test/sjl10/$n.js --fuel 200000000000 --wall 600000 > $R/$n.vm.raw.txt 2>&1 )
python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
sys.stdout.write(t[a+5:b] + "\n" if a >= 0 and b > a else "NO OUTPUT; see raw transcript\n")
' $R/$n.vm.raw.txt > $R/$n.vm.txt
diff $R/$n.bjs.txt $D/temporal.polyfill-e8cc03fc.txt > $R/$n.bjs.diff
diff $R/$n.vm.txt $D/temporal.polyfill-e8cc03fc.txt > $R/$n.vm.diff
gzip -f $R/$n.bjs.txt $R/$n.vm.txt $R/$n.vm.raw.txt
