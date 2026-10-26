#!/bin/bash
# Usage: run-js-invariant.sh <rows-dir> <probe name>...
# run-js.sh's command (built/<name>.js from the same prelude, probe and epilogue) with one variable added,
# DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1, so .NET has only the invariant culture; writes
# <rows-dir>/runs/<name>.bjs.invariant.txt in run-js.sh's form (command, revision, probe hash, output, exit).
set -u
JS_ROOT=/home/user/s9-js
# ASSEMBLE, 2026-10-26: the run named the scratch harness directory here by its absolute path;
# the bundle holds that directory's files in probes/ and runs/{vm,js}/, so H is a variable.
H=${H:?the harness directory: lib/prelude.js, lib/epilogue.js and the run-*.sh scripts}
ROWS=$(cd "$1" && pwd); shift
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
for name in "$@"; do
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  out=$ROWS/runs/$name.bjs.invariant.txt
  {
    echo "# command (from <rows>/, JS_ROOT=$JS_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 timeout 900 dotnet $DLL --script-host built/$name.js"
    echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD)"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$out"
  ( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 timeout 900 dotnet $DLL --script-host built/$name.js >> "$out" 2>&1; echo "exit=$?" >> "$out" )
  sed -i "s#$ROWS/#<rows>/#g" "$out"
done
