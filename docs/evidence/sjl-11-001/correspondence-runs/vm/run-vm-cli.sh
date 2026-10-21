#!/bin/bash
# Usage: run-vm-cli.sh <probe name>...
# SJL-11.1's check of how Broiler.VM's command-line composition root (the root plan 4.1's part D
# names) reports a run run-vm.sh reports through the conformance root: runs built/<name>.js (which
# run-vm.sh wrote) once through Broiler.VM.Composition.JavaScript.Cli, under the same fuel and
# wall-clock allowances as run-vm.sh, and writes cli/<name>.cli.raw.txt (the command, the revision,
# the root's whole output and its exit code). No LANG, LC_ALL or LANGUAGE; TZ=UTC.
set -u
R=$(cd "$(dirname "$0")" && pwd)
VM_ROOT=${VM_ROOT:-/home/user/vm-s11corr}
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Cli.dll
mkdir -p $R/cli
for name in "$@"; do
  fuel=$(sed -n '1s#^// vm-fuel: \([0-9]*\)$#\1#p' $R/probes/$name.js)
  fuel=${fuel:-200000000000}
  cmd="dotnet $DLL --fuel $fuel --wall 600000 built/$name.js"
  out=$R/cli/$name.cli.raw.txt
  {
    echo "# command (from correspondence-runs/vm/, VM_ROOT=$VM_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd"
    echo "# revision: Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
  } > $out
  ( cd $R && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 900 $cmd >> $out 2>&1; echo "exit=$?" >> $out )
done
