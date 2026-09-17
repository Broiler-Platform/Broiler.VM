#!/usr/bin/env bash
# E9 of VM-5-002's re-collection, credit side: p_bisect.py over the first collection's four programs,
# on the command-line host built in D:/broiler-arms/credit-measure (the remedy head with the E9
# measurement patch), then the credit blocks compared with the base blocks of the first collection's
# e9/e9-bisect.log.
#   run-e9.sh <outdir>
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1
TREE=D:/broiler-arms/credit-measure
BIN=$TREE/src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0
mkdir -p "$OUT/e9"
"$HERE/run-logged.sh" "$OUT/e9/e9-bisect.log" "credit-measure (remedy head with the E9 measurement patch)" "$TREE" \
  $BIN/Broiler.VM.Composition.JavaScript.Cli.exe $BIN/Broiler.VM.Composition.JavaScript.Cli.dll $BIN/Broiler.VM.Runtime.dll -- \
  python "$HERE/p_bisect.py" "credit=$BIN" "shapes/a_1000.js,shapes/a_2000.js,shapes/c_small.js+shapes/a_1000.js,shapes/d_small.js"
echo "e9 exit=$?"
python - "$OUT/e9/e9-bisect.log" > "$OUT/e9/e9-compare-with-base.log" <<'PY'
import sys
old = open("D:/Broiler.VM/docs/evidence/vm-5-002/e9/e9-bisect.log", encoding="utf-8").read().splitlines()
new = [l for l in open(sys.argv[1], encoding="utf-8").read().splitlines() if not l.startswith("# ")]
def blocks(lines, variant):
    out = {}
    for i, l in enumerate(lines):
        parts = l.split()
        if len(parts) == 4 and parts[1] == variant and parts[3].startswith("min_fuel="):
            out[(parts[0], parts[2])] = [parts[3]] + lines[i + 1:i + 4]
    return out
base, credit = blocks(old, "base"), blocks(new, "credit")
print("base blocks (first collection e9/e9-bisect.log): %d; credit blocks (re-collection): %d" % (len(base), len(credit)))
same = 0
for key in sorted(base):
    b, c = base[key], credit.get(key)
    if c == b:
        same += 1
        print("IDENTICAL %s %s %s" % (key[0], key[1], b[0]))
    else:
        print("DIFFERENT %s %s\n  base:   %s\n  credit: %s" % (key[0], key[1], b, c))
print("identical blocks: %d of %d" % (same, len(base)))
PY
cat "$OUT/e9/e9-compare-with-base.log"
echo "E9 DONE"
