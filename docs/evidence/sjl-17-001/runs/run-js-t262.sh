#!/bin/bash
# Usage: run-js-t262.sh [--invariant] <rows-dir> <label> <file listing test262 paths>
# SJL-9.1's run-js-t262.sh (bundle sjl-9-001) at the copy at 61cb15b5: the listed files through
# scripts/compliance/run_test262.py on the Release shell, with --include-negative; no LANG, LC_ALL or LANGUAGE;
# TZ=UTC; --invariant adds DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1. Writes <rows-dir>/runs/<label>.t262-bjs[.invariant].raw.txt:
# the command, the revision, the runner's log, its exit, and one JSON line per variant (its duration and size left out).
set -u
H=$(cd "$(dirname "$0")" && pwd)
JS_ROOT=${JS_ROOT:-/home/user/js-s17p}
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
tag=bjs inv=
[ "$1" = --invariant ] && { inv="DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1"; tag=bjs.invariant; shift; }
ROWS=$(cd "$1" && pwd); label=$2; list=$3
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
T=$H/tmp/$label.$$
mkdir -p "$ROWS/runs" "$T"
out=$ROWS/runs/$label.t262-$tag.raw.txt
json=$T/$label.json
paths=$(tr '\n' ' ' < "$list")
{ echo "# command (from JS_ROOT=$JS_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC $inv TMPDIR=<tmp> timeout 1800 python3 scripts/compliance/run_test262.py --suite-root $TEST262_ROOT --broiler-dll $DLL --include-negative --output <json> --no-summary-stdout <each path of the listing>"
  echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD); test262 $(git -C $TEST262_ROOT rev-parse HEAD)"; } > "$out"
( cd "$JS_ROOT" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC $inv TMPDIR=$T timeout 1800 python3 scripts/compliance/run_test262.py --suite-root "$TEST262_ROOT" --broiler-dll "$DLL" --include-negative --output "$json" --no-summary-stdout $paths >> "$out" 2>&1; echo "exit=$?" >> "$out" )
python3 -I -c '
import json, sys
d = json.load(open(sys.argv[1], encoding="utf-8"))
for r in d.get("results", []):
    r = {k: v for k, v in r.items() if k not in ("durationMs", "sourceSizeBytes")}
    print(json.dumps(r, ensure_ascii=True, sort_keys=True))
' "$json" >> "$out" 2>&1
rm -rf "$T"
sed -i "s#$T/#<tmp>/#g; s#$T#<tmp>#g; s#$ROWS/#<rows>/#g" "$out"
