#!/bin/bash
# Usage: run-js.sh [--lang LOCALE] [--invariant] <rows-dir> <probe name>...
# SJL-17.1's Broiler.JS harness, SJL-9.1's form (bundle sjl-9-001, runs/js/run-js.sh and
# run-js-invariant.sh): builds <rows-dir>/built/<name>.js (lib/prelude.js + <rows-dir>/probes/<name>.js +
# lib/epilogue.js) and runs it once through the shell, BroilerJS.dll --script-host, of the copy at
# 61cb15b5 built in Release, writing <rows-dir>/runs/<name>.bjs[.<locale>][.invariant].txt (the command,
# the revision, the probe's hash, the shell's output and its exit). With no --lang the run has no LANG,
# LC_ALL or LANGUAGE (.NET's current culture is the invariant culture, and ICU is loaded); --lang sets
# LANG and LC_ALL; --invariant adds DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1, so .NET loads no ICU and has
# only the invariant culture. TZ=UTC. Each run under timeout 900.
set -u
H=$(cd "$(dirname "$0")" && pwd)
JS_ROOT=${JS_ROOT:-/home/user/js-s17p}
lang= tag=bjs inv=
while [ $# -gt 0 ]; do
  case "$1" in
    --lang) lang=$2; shift 2 ;;
    --invariant) inv=1; shift ;;
    *) break ;;
  esac
done
if [ -n "$lang" ]; then tag=$tag.${lang%%.*}; envspec="-u LANGUAGE LANG=$lang LC_ALL=$lang"; else envspec="-u LANG -u LC_ALL -u LANGUAGE"; fi
if [ -n "$inv" ]; then tag=$tag.invariant; envspec="$envspec DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1"; fi
ROWS=$(cd "$1" && pwd); shift
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
mkdir -p "$ROWS/built" "$ROWS/runs"
for name in "$@"; do
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  out=$ROWS/runs/$name.$tag.txt
  {
    echo "# command (from <rows>/, JS_ROOT=$JS_ROOT): env $envspec TZ=UTC timeout 900 dotnet $DLL --script-host built/$name.js"
    echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD); Broiler.Unicode.Algorithms $(sed -n 's/.*Include="Broiler.Unicode.Algorithms" Version="\([^"]*\)".*/\1/p' $JS_ROOT/Directory.Packages.props)"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$out"
  ( cd "$ROWS" && env $envspec TZ=UTC timeout 900 dotnet $DLL --script-host built/$name.js >> "$out" 2>&1; echo "exit=$?" >> "$out" )
  sed -i "s#$ROWS/#<rows>/#g" "$out"
done
