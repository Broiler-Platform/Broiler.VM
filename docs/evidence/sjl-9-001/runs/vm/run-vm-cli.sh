#!/bin/bash
# Usage: run-vm-cli.sh [--native] [--tz ZONE] <rows-dir> <probe name>...
# SJL-9.1's check through Broiler.VM's command-line composition root (sjl-11-001's run-vm-cli.sh form):
# builds <rows-dir>/built/<name>.js as run-vm.sh does, runs it once through
# Broiler.VM.Composition.JavaScript.Cli of the copy at cd1928d as committed, under run-vm.sh's fuel and
# wall-clock allowances, and writes <rows-dir>/runs/<name>.cli[-native][.<zone>].raw.txt (the command,
# the revision, the root's whole output and its exit). The admitted copy's patch touches the conformance
# root only, so this root has no admitted variant. No LANG, LC_ALL or LANGUAGE; TZ=UTC unless --tz.
# --native emits with x86-64-sysv, the backend this host arms (the root's --native takes a backend name).
# AT cd1928d THIS ROOT DOES NOT COMPOSE THE INTL SURFACE: its project references no
# Broiler.VM.Profile.JavaScript.Intl, `typeof Intl` is "undefined" there, and a program naming any Intl
# member is refused by the verifier (1608 InvalidArtifact/UnsupportedFeatureManifest, exit 4) before it
# runs. So no Intl answer of Broiler.VM's comes from this root; the conformance root (run-vm.sh) gives
# them, in both forms. This script stays for Intl-free control lines only.
set -u
H=$(cd "$(dirname "$0")" && pwd)
VM_ROOT=/home/user/s9-vm
tz=UTC tag=cli native=
while [ $# -gt 0 ]; do
  case "$1" in
    --native) native="--native x86-64-sysv"; tag=cli-native; shift ;;
    --tz) tz=$2; shift 2 ;;
    *) break ;;
  esac
done
[ $# -ge 2 ] || { echo "usage: run-vm-cli.sh [--native] [--tz ZONE] <rows-dir> <probe name>..." >&2; exit 2; }
[ "$tz" = UTC ] || tag=$tag.${tz//\//_}
ROWS=$(cd "$1" && pwd); shift
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Cli.dll
mkdir -p "$ROWS/built" "$ROWS/runs"
for name in "$@"; do
  fuel=$(sed -n '1s#^// vm-fuel: \([0-9]*\)$#\1#p' "$ROWS/probes/$name.js")
  fuel=${fuel:-200000000000}
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  cmd="dotnet $DLL --fuel $fuel --wall 600000 $native built/$name.js"
  out=$ROWS/runs/$name.$tag.raw.txt
  {
    echo "# command (from <rows>/, VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 900 ${cmd//  / }"
    echo "# revision: Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$out"
  ( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz timeout 900 $cmd >> "$out" 2>&1; echo "exit=$?" >> "$out" )
  sed -i "s#$ROWS/#<rows>/#g" "$out"
done
