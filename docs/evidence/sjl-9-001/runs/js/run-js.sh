#!/bin/bash
# Usage: run-js.sh [--tz ZONE] [--lang LOCALE] <rows-dir> <probe name>...
# SJL-9.1's Broiler.JS harness, SJL-11.1's form (sjl-11-001/correspondence-runs/js/run-js.sh): builds
# <rows-dir>/built/<name>.js (lib/prelude.js + <rows-dir>/probes/<name>.js + lib/epilogue.js) and runs it
# once through the shell, BroilerJS.dll --script-host, of the copy at f918b040 built in Release, writing
# <rows-dir>/runs/<name>.bjs[.<zone>][.<locale>].txt (the command, the revision, the shell's output and
# its exit). With no --lang the run has no LANG, LC_ALL or LANGUAGE (the .NET runtime's current culture is
# the invariant culture); --lang tr_TR.UTF-8 sets LANG and LC_ALL to it (the host has no locale
# definitions beyond C and C.utf8; .NET takes the culture name from the variable, and ICU supplies its
# data). TZ=UTC unless --tz. Each run under timeout 900.
set -u
H=$(cd "$(dirname "$0")" && pwd)
JS_ROOT=/home/user/s9-js
tz=UTC lang= tag=bjs
while [ $# -gt 0 ]; do
  case "$1" in
    --tz) tz=$2; shift 2 ;;
    --lang) lang=$2; shift 2 ;;
    *) break ;;
  esac
done
[ $# -ge 2 ] || { echo "usage: run-js.sh [--tz ZONE] [--lang LOCALE] <rows-dir> <probe name>..." >&2; exit 2; }
[ "$tz" = UTC ] || tag=$tag.${tz//\//_}
if [ -n "$lang" ]; then tag=$tag.${lang%%.*}; envspec="-u LANGUAGE LANG=$lang LC_ALL=$lang"; else envspec="-u LANG -u LC_ALL -u LANGUAGE"; fi
ROWS=$(cd "$1" && pwd); shift
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
mkdir -p "$ROWS/built" "$ROWS/runs"
for name in "$@"; do
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  out=$ROWS/runs/$name.$tag.txt
  {
    echo "# command (from <rows>/, JS_ROOT=$JS_ROOT): env $envspec TZ=$tz timeout 900 dotnet $DLL --script-host built/$name.js"
    echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD); Broiler.Regex $(sed -n 's/.*Include="Broiler.Regex" Version="\([^"]*\)".*/\1/p' $JS_ROOT/Directory.Packages.props); Broiler.Unicode.Algorithms $(sed -n 's/.*Include="Broiler.Unicode.Algorithms" Version="\([^"]*\)".*/\1/p' $JS_ROOT/Directory.Packages.props)"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$out"
  ( cd "$ROWS" && env $envspec TZ=$tz timeout 900 dotnet $DLL --script-host built/$name.js >> "$out" 2>&1; echo "exit=$?" >> "$out" )
  # The shell names the program by its absolute path in a stack trace; that path is this machine's.
  sed -i "s#$ROWS/#<rows>/#g" "$out"
done
