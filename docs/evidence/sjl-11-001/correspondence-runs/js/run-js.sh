#!/bin/bash
# Usage: run-js.sh [--cultures] <probe name>...
# SJL-11.1's Broiler.JS harness, SJL-10.1's form: builds built/<name>.js (probes/prelude.js +
# probes/<name>.js + probes/epilogue.js) and runs it once through the shell, BroilerJS.dll
# --script-host, writing <name>.bjs.txt (the command, the revision, the shell's output and exit).
# With --cultures the probe is also run with LANG and LC_ALL set to tr_TR.UTF-8 and lt_LT.UTF-8,
# writing <name>.tr-TR.bjs.txt and <name>.lt-LT.bjs.txt; the shell's .NET runtime takes its current
# culture from them (shown by each probe's first line, Intl's default locale). The invariant run has
# no LANG, LC_ALL or LANGUAGE. TZ=UTC throughout; each run under timeout 900. A file over 100 KB is
# then compressed by hand (gzip -n -9) and kept as <file>.gz.
# JS_ROOT names a read-only checkout at Broiler.JS 1f91b8f1 built in Release with
# --disable-build-servers; its Broiler.Regex is the published 0.1.0-preview.4 (Directory.Packages.props).
set -u
R=$(cd "$(dirname "$0")" && pwd)
JS_ROOT=${JS_ROOT:-/home/user/js-s11corr}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
cultures=0
if [ "${1:-}" = "--cultures" ]; then cultures=1; shift; fi
mkdir -p $R/built
run() { # name suffix envspec
  local name=$1 out=$R/$1$2.bjs.txt
  shift 2
  {
    echo "# command (from correspondence-runs/js/, JS_ROOT=$JS_ROOT): env $* TZ=UTC timeout 900 dotnet $DLL --script-host built/$name.js"
    echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD); Broiler.Regex $(sed -n 's/.*Include="Broiler.Regex" Version="\([^"]*\)".*/\1/p' $JS_ROOT/Directory.Packages.props)"
  } > $out
  ( cd $R && env "$@" TZ=UTC timeout 900 dotnet $DLL --script-host built/$name.js >> $out 2>&1; echo "exit=$?" >> $out )
  # The shell names the program by its absolute path in a stack trace; that path is this machine's,
  # so it is written as <runs>/ (the folder this script is in), the file's only rewrite.
  sed -i "s#$R/#<runs>/#g" $out
}
for name in "$@"; do
  cat $R/probes/prelude.js $R/probes/$name.js $R/probes/epilogue.js > $R/built/$name.js
  run $name "" -u LANG -u LC_ALL -u LANGUAGE
  if [ $cultures = 1 ]; then
    run $name .tr-TR -u LANGUAGE LANG=tr_TR.UTF-8 LC_ALL=tr_TR.UTF-8
    run $name .lt-LT -u LANGUAGE LANG=lt_LT.UTF-8 LC_ALL=lt_LT.UTF-8
  fi
done
