#!/bin/bash
# Usage: run.sh <probe-name>   (reads probes/<name>.js; writes runs/<name>.vm.txt, runs/<name>.js.txt and raw logs)
set -u
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
unset LANG LC_ALL LANGUAGE
export TZ=UTC
C=<work>/cal
N=$1
VMDLL=<Broiler.VM>/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
JSDLL=<Broiler.JS>/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
# Broiler.VM: a test262-shaped file; its lines leave through an uncaught Error's message.
{
  printf '/*---\ndescription: sjl8 calendar probe %s\nflags: [noStrict]\n---*/\n' "$N"
  printf 'var __o = []; function emit(s) { __o.push(String(s)); }\n'
  cat $C/probes/prelude.js $C/probes/$N.js
  printf "\nthrow new Error('@@OUT@@\\\\n' + __o.join('\\\\n') + '\\\\n@@END@@');\n"
} > $C/t262/test/probe/$N.js
# Broiler.JS: the shell's print.
{
  printf 'function emit(s) { print(String(s)); }\n'
  cat $C/probes/prelude.js $C/probes/$N.js
} > $C/runs/$N.broilerjs-input.js
cp $C/t262/test/probe/$N.js $C/runs/$N.broilervm-input.js
(cd $C && dotnet $VMDLL --test262 $C/t262 --test test/probe/$N.js) > $C/runs/$N.vm.raw 2>&1
awk '/@@OUT@@/{on=1; next} /^@@END@@/{on=0} on' $C/runs/$N.vm.raw > $C/runs/$N.vm.txt
(cd $C && dotnet $JSDLL --script-host $C/runs/$N.broilerjs-input.js) > $C/runs/$N.js.txt 2> $C/runs/$N.js.err
echo "vm lines: $(wc -l < $C/runs/$N.vm.txt)  js lines: $(wc -l < $C/runs/$N.js.txt)  js stderr: $(wc -c < $C/runs/$N.js.err) bytes"
diff $C/runs/$N.vm.txt $C/runs/$N.js.txt > $C/runs/$N.diff; echo "diff lines: $(grep -c '^[<>]' $C/runs/$N.diff)"
