#!/usr/bin/env bash
# E8 of VM-5-002's re-collection, credit side: the low-fuel runs over the four subtrees at fuel 1,000,
# 2,500, 10,000, 30,000 and 100,000, both forms, from the main checkout's conformance host at the
# remedy's head, driven by this branch's eng/run-test262.py; then, per (form, fuel), the sorted result
# rows and the form-comparison script against the base report the first collection retained, and a
# count of fuel-exhaustion lines on both sides.
#   run-e8.sh <outdir> [runs|compare] [fuel ...]
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1; STEP=${2:-all}; shift; [ $# -gt 0 ] && shift
FUELS="${*:-1000 2500 10000 30000 100000}"
TREE=D:/Broiler.VM
SUITE=artifacts/t262/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
BIN=src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0
REL=$(python -c "import os,sys; print(os.path.relpath(sys.argv[1], sys.argv[2]).replace(os.sep,'/'))" "$OUT" "$TREE")
mkdir -p "$OUT/e8"
if [ "$STEP" = all ] || [ "$STEP" = runs ]; then
  for f in $FUELS; do
    for form in bytecode native; do
      rm -rf "$OUT/low-credit-$form-$f"
      "$HERE/run-logged.sh" "$OUT/e8/low-credit-$form-$f-driver.log" "credit, test262 low fuel $form $f" "$TREE" \
        $BIN/Broiler.VM.Composition.JavaScript.Conformance.exe $BIN/Broiler.VM.Composition.JavaScript.Conformance.dll $BIN/Broiler.VM.Runtime.dll eng/run-test262.py -- \
        python eng/run-test262.py --suite $SUITE --binary-directory $BIN --form $form --fuel $f --wall 600000 \
          --dir test/built-ins/Promise --dir test/language/statements/class --dir test/built-ins/Array \
          --dir test/language/expressions/object \
          --digest-cache artifacts/fc/t262-digest.cache --out "$REL/low-credit-$form-$f"
      echo "=== EXIT credit $form fuel=$f -> $? ($(date -Iseconds)) ==="
    done
  done
fi
if [ "$STEP" = all ] || [ "$STEP" = compare ]; then
  python "$HERE/base-reports.py" "$OUT/base-reports" > "$OUT/e8/base-reports-check.log" 2>&1 || { cat "$OUT/e8/base-reports-check.log"; exit 1; }
  cd "$TREE"
  : > "$OUT/e8/e8-compare-summary.log"
  : > "$OUT/e8/credit-report-digests.txt"
  python "$HERE/identity.py" --build "e8 comparison" --tree "$TREE" --bin eng/compare-test262-forms.py >> "$OUT/e8/e8-compare-summary.log"
  for f in $FUELS; do
    for form in bytecode native; do
      B="$OUT/base-reports/e8/low-base-$form-$f.report"; C="$OUT/low-credit-$form-$f/test262.report"
      if [ ! -f "$C" ]; then printf '%-9s %-7s NOT RE-COLLECTED\n' $form $f >> "$OUT/e8/e8-compare-summary.log"; continue; fi
      { python "$HERE/identity.py" --build "e8 comparison" --tree "$TREE" --bin eng/compare-test262-forms.py
        echo "# command=python eng/compare-test262-forms.py --same-form --suite $SUITE <retained base report> <re-collection credit report>"
        python eng/compare-test262-forms.py --same-form --suite $SUITE "$B" "$C" 2>&1
        echo "# exit=$?"; } > "$OUT/e8/compare-e8-$form-$f.log"
      code=$(grep "^# exit=" "$OUT/e8/compare-e8-$form-$f.log" | tail -1 | cut -d= -f2)
      diffs=$(diff <(grep "^result|" "$B" | sort) <(grep "^result|" "$C" | sort) | grep -c "^[<>]")
      fb=$(grep -c "Fuel" "$B"); fc=$(grep -c "Fuel" "$C")
      printf '%-9s %-7s sorted-row-diff=%s compare-exit=%s Fuel-lines base=%s credit=%s | %s\n' $form $f "$diffs" "$code" "$fb" "$fc" \
        "$(grep "variants in both" "$OUT/e8/compare-e8-$form-$f.log" | sed 's/^# //')" >> "$OUT/e8/e8-compare-summary.log"
      python - "$C" "$OUT/e8/low-credit-$form-$f.report.gz" >> "$OUT/e8/credit-report-digests.txt" <<'PY'
import gzip, hashlib, sys
data = open(sys.argv[1], "rb").read()
with open(sys.argv[2], "wb") as raw:
    with gzip.GzipFile(filename="", mode="wb", fileobj=raw, mtime=0) as gz:
        gz.write(data)
print("%s  %d  %s" % (hashlib.sha256(data).hexdigest(), len(data), sys.argv[1]))
PY
    done
  done
  cat "$OUT/e8/e8-compare-summary.log"
fi
echo "E8 DONE: $STEP $FUELS"
