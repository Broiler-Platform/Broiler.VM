#!/usr/bin/env bash
# The evidence collector of VM-5-002's re-collection, at the remedy head, into r2/. The collector writes
# its own logs and no identity lines, and it is a hashed driver, so this wrapper writes the identity
# fields to r2/collector-identity.txt immediately before and immediately after it runs, with the digests
# of the runtime assembly and the bench host the checkout holds at each point (the collector's own build
# step rebuilds them). The collector's console goes to <console log>. It is run with this README at
# d5014f8's text, so the tree it collects is the commit's; the wrapper does not restore anything itself.
#   run-collector.sh <console log>
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
CONSOLE=$1
TREE=D:/Broiler.VM
R2=$TREE/docs/evidence/vm-5-002/r2
BINS=(--bin $TREE/src/Broiler.VM.Runtime/bin/Release/net10.0/Broiler.VM.Runtime.dll
      --bin $TREE/src/tests/Broiler.VM.Bench.Host/bin/Release/net10.0/Broiler.VM.Bench.Host.exe
      --bin $TREE/src/tests/Broiler.VM.Bench.Host/bin/Release/net10.0/Broiler.VM.Runtime.dll
      --bin $TREE/eng/collect-evidence.py)
export DOTNET_CLI_UI_LANGUAGE=en
{ echo "# before the collector"; python "$HERE/identity.py" --build "collector, before" --tree "$TREE" "${BINS[@]}"; } > "$R2/collector-identity.txt" || { cat "$R2/collector-identity.txt"; exit 97; }
echo "# command=python eng/collect-evidence.py --bundle VM-5-002 --out docs/evidence/vm-5-002/r2 --rebench --skip-controls" >> "$R2/collector-identity.txt"
( cd "$TREE" && python eng/collect-evidence.py --bundle VM-5-002 --out docs/evidence/vm-5-002/r2 --rebench --skip-controls ) > "$CONSOLE" 2>&1
code=$?
{ echo "# exit=$code"; echo "# after the collector"; python "$HERE/identity.py" --build "collector, after" --tree "$TREE" "${BINS[@]}"; } >> "$R2/collector-identity.txt"
echo "collector exit=$code"
