#!/bin/bash
# Usage: run-vm.sh [--admitted] [--form bytecode|native] [--tz ZONE] [--lang LOCALE] <rows-dir> <probe name>...
# SJL-9.1's Broiler.VM harness, SJL-11.1's form (sjl-11-001/correspondence-runs/vm/run-vm.sh): builds
# <rows-dir>/built/<name>.js (lib/prelude.js + <rows-dir>/probes/<name>.js + lib/epilogue.js) and
# vm262/test/sjl9/<rows>/<name>.js beside this script (the same program as a raw test262 test), runs it
# once through the conformance composition root, and writes <rows-dir>/runs/<name>.<tag>.raw.txt (the
# command, the revision, the root's whole transcript and its exit) and <name>.<tag>.txt (the probe's
# lines, or the verdict line when the program ended before printing them).
# <tag> is vm (bytecode, the copy at cd1928d as committed), vm-native (--form native), vm-admitted
# (the copy with canonical-tz, Intl.Locale-info and Intl.NumberFormat-v3 admitted by
# ../admitted-proposals.patch; the patch touches the suite-features list only, so a raw probe's answer
# is the same in both copies, and the admitted copy matters for test262 files that claim a proposal:
# run-vm-t262.sh), vm-admitted-native; a zone other than UTC adds .<zone, "/" as "_">.
# The fuel allowance is 200,000,000,000 unless the probe's first line says "// vm-fuel: N"; the
# wall-clock allowance is 600,000 ms. No LANG, LC_ALL or LANGUAGE unless --lang (which sets LANG and
# LC_ALL, and adds .<locale> to the tag); TZ=UTC unless --tz. The conformance root's realm answers UTC
# and en-US whatever TZ and LANG say (smoke/runs/host-defaults.*), so --tz and --lang are controls.
set -u
H=$(cd "$(dirname "$0")" && pwd)
VM_ROOT=/home/user/s9-vm
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
form=bytecode tz=UTC tag=vm lang=
while [ $# -gt 0 ]; do
  case "$1" in
    --admitted) VM_ROOT=/home/user/s9-vm-admitted; tag=vm-admitted; shift ;;
    --form) form=$2; shift 2 ;;
    --tz) tz=$2; shift 2 ;;
    --lang) lang=$2; shift 2 ;;
    *) break ;;
  esac
done
[ $# -ge 2 ] || { echo "usage: run-vm.sh [--admitted] [--form bytecode|native] [--tz ZONE] <rows-dir> <probe name>..." >&2; exit 2; }
case "$form" in bytecode) ;; native) tag=$tag-native ;; *) echo "form must be bytecode or native" >&2; exit 2 ;; esac
[ "$tz" = UTC ] || tag=$tag.${tz//\//_}
if [ -n "$lang" ]; then tag=$tag.${lang%%.*}; envspec="-u LANGUAGE LANG=$lang LC_ALL=$lang"; else envspec="-u LANG -u LC_ALL -u LANGUAGE"; fi
ROWS=$(cd "$1" && pwd); shift
rows=$(basename "$ROWS")
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$VM_ROOT/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
V=$H/vm262
mkdir -p "$ROWS/built" "$ROWS/runs" "$V/test/sjl9/$rows"
[ -d "$V/harness" ] || cp -r "$TEST262_ROOT/harness" "$V/harness"
[ -f "$V/features.txt" ] || cp "$TEST262_ROOT/features.txt" "$V/features.txt"
revision="Broiler.VM $(git -C $VM_ROOT rev-parse HEAD)"
if [ "$VM_ROOT" = /home/user/s9-vm-admitted ]; then
  revision="$revision, working tree = that commit plus admitted-proposals.patch (sha256 $(git -C $VM_ROOT diff | sha256sum | cut -c1-64))"
fi
for name in "$@"; do
  fuel=$(sed -n '1s#^// vm-fuel: \([0-9]*\)$#\1#p' "$ROWS/probes/$name.js")
  fuel=${fuel:-200000000000}
  cat "$H/lib/prelude.js" "$ROWS/probes/$name.js" "$H/lib/epilogue.js" > "$ROWS/built/$name.js"
  { printf '/*---\ndescription: SJL-9.1 probe %s\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"; cat "$ROWS/built/$name.js"; } > "$V/test/sjl9/$rows/$name.js"
  cmd="dotnet $DLL --test262 vm262 --test test/sjl9/$rows/$name.js --form $form --fuel $fuel --wall 600000"
  raw=$ROWS/runs/$name.$tag.raw.txt
  {
    echo "# command (from harness/, VM_ROOT=$VM_ROOT): env $envspec TZ=$tz timeout 900 $cmd"
    echo "# revision: $revision"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$raw"
  ( cd "$H" && env $envspec TZ=$tz timeout 900 $cmd >> "$raw" 2>&1; echo "exit=$?" >> "$raw" )
  sed -i "s#$H/#<harness>/#g; s#$ROWS/#<rows>/#g" "$raw"
  python3 -I -c '
import sys
t = open(sys.argv[1], encoding="utf-8", errors="replace").read()
a = t.find("OUT<<"); b = t.rfind(">>OUT")
if a >= 0 and b > a:
    sys.stdout.write(t[a+5:b] + "\n")
else:
    lines = [l for l in t.splitlines() if "test/sjl9/" in l and not l.startswith("#") and not l.startswith("coverage")]
    sys.stdout.write("NO OUTPUT; the verdict line: " + (lines[0] if lines else "(none)") + "\n")
' "$raw" > "$ROWS/runs/$name.$tag.txt"
done
