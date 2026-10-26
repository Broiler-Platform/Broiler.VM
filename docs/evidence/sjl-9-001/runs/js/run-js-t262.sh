#!/bin/bash
# Usage: run-js-t262.sh [--tz ZONE] <rows-dir> <label> <test262 path>...
# Runs test262 files (paths relative to the checkout) of the read-only checkout at ccaac100 through
# Broiler.JS's own test262 runner (scripts/compliance/run_test262.py of the copy at f918b040) on the
# Release shell, and writes <rows-dir>/runs/<label>.t262-bjs[.<zone>].raw.txt: the command, the revision,
# the runner's log, its exit, and one JSON line per variant (path, variant, status and every field the
# runner records but its duration and size). No LANG, LC_ALL or LANGUAGE; TZ=UTC unless --tz. The runner's
# temporary files go under ../tmp/<label>.<pid> (TMPDIR, one per invocation, removed after), never
# /tmp; the checkout is read only.
set -u
H=$(cd "$(dirname "$0")" && pwd)
JS_ROOT=/home/user/s9-js
TEST262_ROOT=${TEST262_ROOT:-/home/user/tc39/test262}
tz=UTC tag=bjs
while [ $# -gt 0 ]; do
  case "$1" in
    --tz) tz=$2; shift 2 ;;
    *) break ;;
  esac
done
[ $# -ge 3 ] || { echo "usage: run-js-t262.sh [--tz ZONE] <rows-dir> <label> <test262 path>..." >&2; exit 2; }
[ "$tz" = UTC ] || tag=$tag.${tz//\//_}
ROWS=$(cd "$1" && pwd); label=$2; shift 2
export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
DLL=$JS_ROOT/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
T=$(cd "$H/.." && pwd)/tmp/$label.$$
mkdir -p "$ROWS/runs" "$T"
out=$ROWS/runs/$label.t262-$tag.raw.txt
json=$T/$label.$tag.json
{
  echo "# command (from JS_ROOT=$JS_ROOT): env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz TMPDIR=<tmp> timeout 1800 python3 scripts/compliance/run_test262.py --suite-root $TEST262_ROOT --broiler-dll $DLL --output <json> --no-summary-stdout $*"
  echo "# revision: Broiler.JS $(git -C $JS_ROOT rev-parse HEAD); test262 $(git -C $TEST262_ROOT rev-parse HEAD)"
} > "$out"
( cd "$JS_ROOT" && env -u LANG -u LC_ALL -u LANGUAGE TZ=$tz TMPDIR=$T timeout 1800 python3 scripts/compliance/run_test262.py --suite-root "$TEST262_ROOT" --broiler-dll "$DLL" --output "$json" --no-summary-stdout "$@" >> "$out" 2>&1; echo "exit=$?" >> "$out" )
python3 -I -c '
import json, sys
d = json.load(open(sys.argv[1], encoding="utf-8"))
for r in d.get("results", []):
    r = {k: v for k, v in r.items() if k not in ("durationMs", "sourceSizeBytes")}
    print(json.dumps(r, ensure_ascii=True, sort_keys=True))
' "$json" >> "$out" 2>&1
rm -rf "$T"
sed -i "s#$T/#<tmp>/#g; s#$T#<tmp>#g; s#$ROWS/#<rows>/#g" "$out"
