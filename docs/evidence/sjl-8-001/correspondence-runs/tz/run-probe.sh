#!/bin/bash
# Usage: run-probe.sh <probe name> [TZ for Broiler.JS, default UTC]
# Builds runs/built/<name>.js (Broiler.JS) and vm262/test/sjl8/<name>.js (Broiler.VM, a raw test262
# test), runs each engine once, and writes runs/<name>.bjs.txt, runs/<name>.vm.raw.txt, runs/<name>.vm.txt.
set -u
R=<work>/runs
V=<work>/vm262
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
name=$1; tz=${2:-UTC}; suffix=${3:-}
cat $R/probes/prelude.js $R/probes/$name.js $R/probes/epilogue.js > $R/built/$name.js
{ printf '/*---\ndescription: SJL-8 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat $R/built/$name.js; } > $V/test/sjl8/$name.js
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 600 dotnet <Broiler.JS>/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll --script-host $R/built/$name.js > $R/$name.bjs$suffix.txt 2>&1; echo "exit=$?" >> $R/$name.bjs$suffix.txt )
if [ -z "$suffix" ]; then
( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 900 dotnet <Broiler.VM>/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll --test262 $V --test test/sjl8/$name.js --fuel 200000000000 --wall 600000 > $R/$name.vm.raw.txt 2>&1 )
python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
sys.stdout.write(t[a+5:b] + "\n" if a >= 0 and b > a else "NO OUTPUT; see raw transcript\n")
' $R/$name.vm.raw.txt > $R/$name.vm.txt
fi
