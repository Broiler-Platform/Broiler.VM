#!/usr/bin/env bash
# E8: the low-fuel subtree runs. Five fuel values, both forms, both builds.
set -u
cd /d/Broiler.VM || exit 1

SUITE=artifacts/t262/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
CREDIT_BIN=src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0
BASE_BIN=/d/Broiler.VM-base/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0

for f in 1000 2500 10000 30000 100000; do
  for form in bytecode native; do
    for build in base credit; do
      if [ "$build" = base ]; then BIN="$BASE_BIN"; else BIN="$CREDIT_BIN"; fi
      OUT="artifacts/fc/low-$build-$form-$f"
      echo "=== RUN $build $form fuel=$f ==="
      python eng/run-test262.py \
        --suite "$SUITE" \
        --binary-directory "$BIN" \
        --form "$form" \
        --fuel "$f" \
        --wall 600000 \
        --dir test/built-ins/Promise \
        --dir test/language/statements/class \
        --dir test/built-ins/Array \
        --dir test/language/expressions/object \
        --digest-cache artifacts/fc/t262-digest.cache \
        --out "$OUT" > "$OUT-driver.log" 2>&1
      echo "=== EXIT $build $form fuel=$f -> $? ==="
    done
  done
done
echo "=== E8 ALL RUNS DONE ==="
