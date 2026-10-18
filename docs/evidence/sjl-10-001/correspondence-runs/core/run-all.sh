#!/bin/bash
# Runs every probe of probes/ through run-probe.sh, then the retained dataset through run-dataset.sh,
# and writes run-all.log. POLYFILL_DIR as run-probe.sh describes.
set -u
R=$(cd "$(dirname "$0")" && pwd)
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
{
  echo "run on $(date -u +%Y-%m-%d), $(. /etc/os-release; echo $PRETTY_NAME)"
  echo ".NET runtime: $(dotnet --list-runtimes | grep NETCore.App | cut -d' ' -f2); Node $(node --version)"
  for p in $(ls $R/probes | grep -v '^prelude\|^epilogue' | sed 's/\.js$//'); do
    $R/run-probe.sh $p
    echo "$p done"
  done
  $R/run-dataset.sh
  echo "d6-retained-dataset done"
} > $R/run-all.log 2>&1
