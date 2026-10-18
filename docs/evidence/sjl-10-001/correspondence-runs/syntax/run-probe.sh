#!/bin/bash
# Usage: run-probe.sh <probe name>
# SJL-8.1's harness (docs/evidence/sjl-8-001/correspondence-runs/tz/run-probe.sh), for SJL-10.1's syntax half.
# Builds built/<name>.js (Broiler.JS) and vm262/test/sjl10/<name>.js (Broiler.VM, a raw test262 test),
# runs each engine once, and writes <name>.bjs.txt, <name>.vm.raw.txt, <name>.vm.txt and <name>.compare.txt.
# BJS and VM name the read-only checkouts the correspondence reads (Broiler.JS d3464413, Broiler.VM 14072b0),
# each built in Release; neither is built by this script.
set -u
R=$(cd "$(dirname "$0")" && pwd)
V=$R/vm262
BJS=${BJS:-/home/user/js-at-d3464413}
VM=${VM:-/home/user/vm-at-14072b0}
# The pinned suite (test262 ccaac100): its harness/ makes vm262/ a test262 root; it is copied for the run, not retained.
T262=${T262:-/home/user/tc39/test262}
[ -d $V/harness ] || cp -r $T262/harness $V/harness
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
name=$1
cat $R/probes/prelude.js $R/probes/$name.js $R/probes/epilogue.js > $R/built/$name.js
{ printf '/*---\ndescription: SJL-10.1 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat $R/built/$name.js; } > $V/test/sjl10/$name.js
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 600 dotnet $BJS/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll --script-host $R/built/$name.js > $R/$name.bjs.txt 2>&1; echo "exit=$?" >> $R/$name.bjs.txt )
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 dotnet $VM/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll --test262 $V --test test/sjl10/$name.js --fuel 200000000000 --wall 600000 > $R/$name.vm.raw.txt 2>&1 )
python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
sys.stdout.write(t[a+5:b] + "\n" if a >= 0 and b > a else "NO OUTPUT; see raw transcript\n")
' $R/$name.vm.raw.txt > $R/$name.vm.txt
python3 -I $R/sidebyside.py $R/$name.vm.txt $R/$name.bjs.txt > $R/$name.compare.txt
