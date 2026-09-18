#!/usr/bin/env bash
# E7 of VM-5-002's re-collection, credit side: two whole-suite test262 runs of the pinned checkout from
# the main checkout's conformance host at the remedy's head, bytecode then native, under the wide
# manifest at the default fuel and 60,000 ms, driven by this branch's eng/run-test262.py; then the four
# comparisons and the hand rule of section 5.2 against the base reports the first collection retained.
#   run-e7.sh <outdir> [bytecode|native|compare ...]
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1; shift
STEPS="${*:-bytecode native compare}"
TREE=D:/Broiler.VM
SUITE=artifacts/t262/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
BIN=src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0
REL=$(python -c "import os,sys; print(os.path.relpath(sys.argv[1], sys.argv[2]).replace(os.sep,'/'))" "$OUT" "$TREE")
mkdir -p "$OUT/e7"
for step in $STEPS; do
  case $step in
    bytecode|native)
      rm -rf "$OUT/t262-credit-$step"
      "$HERE/run-logged.sh" "$OUT/e7/t262-credit-$step-driver.log" "credit, test262 $step" "$TREE" \
        $BIN/Broiler.VM.Composition.JavaScript.Conformance.exe $BIN/Broiler.VM.Composition.JavaScript.Conformance.dll $BIN/Broiler.VM.Runtime.dll eng/run-test262.py -- \
        python eng/run-test262.py --suite $SUITE --binary-directory $BIN --form $step --wall 60000 \
          --digest-cache artifacts/fc/t262-digest.cache --out "$REL/t262-credit-$step"
      echo "e7 $step exit=$?"
      cp "$OUT/t262-credit-$step/merge.log" "$OUT/e7/t262-credit-$step-merge.log"
      ;;
    compare)
      python "$HERE/base-reports.py" "$OUT/base-reports" > "$OUT/e7/base-reports-check.log" 2>&1 || { cat "$OUT/e7/base-reports-check.log"; exit 1; }
      B=$OUT/base-reports/e7
      cd "$TREE"
      for form in bytecode native; do
        { python "$HERE/identity.py" --build "e7 comparison" --tree "$TREE" --bin eng/compare-test262-forms.py
          echo "# command=python eng/compare-test262-forms.py --same-form --suite $SUITE <retained base $form report> <re-collection credit $form report>"
          python eng/compare-test262-forms.py --same-form --suite $SUITE "$B/t262-base-$form.report" "$OUT/t262-credit-$form/test262.report" 2>&1
          echo "# exit=$?"; } > "$OUT/e7/compare-sameform-$form.log"
        { python "$HERE/identity.py" --build "e7 hand rule" --tree "$TREE"
          echo "# command=python docs/evidence/vm-5-002/measurement/e7-rule.py <retained base $form report> <re-collection credit $form report> $form"
          python docs/evidence/vm-5-002/measurement/e7-rule.py "$B/t262-base-$form.report" "$OUT/t262-credit-$form/test262.report" $form 2>&1
          echo "# exit=$?"; } > "$OUT/e7/e7-rule-$form.log"
      done
      { python "$HERE/identity.py" --build "e7 comparison" --tree "$TREE" --bin eng/compare-test262-forms.py
        echo "# command=python eng/compare-test262-forms.py --exempt-guest-loads --suite $SUITE <re-collection credit bytecode report> <re-collection credit native report>"
        python eng/compare-test262-forms.py --exempt-guest-loads --suite $SUITE "$OUT/t262-credit-bytecode/test262.report" "$OUT/t262-credit-native/test262.report" 2>&1
        echo "# exit=$?"; } > "$OUT/e7/compare-forms-credit.log"
      { python "$HERE/identity.py" --build "e7 comparison" --tree "$TREE" --bin eng/compare-test262-forms.py
        echo "# command=python eng/compare-test262-forms.py --exempt-guest-loads --suite $SUITE <retained base bytecode report> <retained base native report>"
        python eng/compare-test262-forms.py --exempt-guest-loads --suite $SUITE "$B/t262-base-bytecode.report" "$B/t262-base-native.report" 2>&1
        echo "# exit=$?"; } > "$OUT/e7/compare-forms-base.log"
      for form in bytecode native; do
        python - "$OUT/t262-credit-$form/test262.report" "$OUT/e7/t262-credit-$form.report.gz" <<'PY'
import gzip, hashlib, sys
data = open(sys.argv[1], "rb").read()
with open(sys.argv[2], "wb") as raw:
    with gzip.GzipFile(filename="", mode="wb", fileobj=raw, mtime=0) as gz:
        gz.write(data)
print("%s  %d  %s" % (hashlib.sha256(data).hexdigest(), len(data), sys.argv[1]))
PY
      done > "$OUT/e7/credit-report-digests.txt"
      tail -n 3 "$OUT"/e7/compare-*.log; cat "$OUT"/e7/e7-rule-*.log | grep -v "^# "; cat "$OUT/e7/credit-report-digests.txt"
      ;;
  esac
done
echo "E7 DONE: $STEPS"
