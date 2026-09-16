#!/usr/bin/env bash
# E3 at collection time: every witness applied to a worktree of the head, the fuel-exactness tests
# run, the tree restored. One transcript per run, kept whatever it shows.
set -u
WT=/d/broiler-arms/wit
OUT=/d/Broiler.VM/artifacts/fc/e3
PY=/c/Users/proof/AppData/Local/Temp/claude/D--Broiler-VM/cbafeab9-f73a-434e-9481-38837a0bebe7/scratchpad/c4b/wit/witnesses.py
export DOTNET_CLI_UI_LANGUAGE=en
cd "$WT" || exit 1

run_tests() { # $1 = log, $2 = filter
  {
    echo "# worktree $(git rev-parse HEAD)"
    echo "# started $(date -Iseconds)"
    echo "# dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --filter $2 --logger console;verbosity=normal"
    dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --filter "$2" --logger "console;verbosity=normal" 2>&1
    echo "# exit $?"
    echo "# finished $(date -Iseconds)"
  } > "$1"
}

CLASS="FullyQualifiedName~FuelChargeExactnessTests"
T6="FullyQualifiedName~FuelChargeExactnessTests.Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit"

git checkout -q -- src/Broiler.VM.Runtime
run_tests "$OUT/clean-before.log" "$CLASS"
echo "clean-before: $(grep -E '^(Passed!|Failed!)' "$OUT/clean-before.log")"

for w in W1 W2 W3 W4 W4b W5 W6 W7 W8 W9 W9b W10 W11 W12; do
  git checkout -q -- src/Broiler.VM.Runtime
  if ! python "$PY" "$WT" "$w" > "$OUT/$w.apply.txt" 2>&1; then
    echo "$w: REFUSED"; cat "$OUT/$w.apply.txt"; continue
  fi
  git diff -- src/Broiler.VM.Runtime >> "$OUT/$w.apply.txt"
  run_tests "$OUT/$w.log" "$CLASS"
  echo "$w: $(grep -E '^(Passed!|Failed!)|error CS' "$OUT/$w.log" | head -3 | tr '\n' ' ')"
  if [ "$w" = W4 ] && ! grep -q "Failed Broiler.VM.Contract.Tests.FuelChargeExactnessTests.Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit" "$OUT/$w.log"; then
    for i in 2 3 4 5 6 7 8 9 10; do
      run_tests "$OUT/W4-iteration-$i.log" "$T6"
      echo "W4 iteration $i: $(grep -E '^(Passed!|Failed!)' "$OUT/W4-iteration-$i.log")"
      grep -q '^Failed!' "$OUT/W4-iteration-$i.log" && break
    done
  fi
  git checkout -q -- src/Broiler.VM.Runtime
  echo "# after revert: git status --porcelain -> [$(git status --porcelain)]" >> "$OUT/$w.log"
done

git checkout -q -- src/Broiler.VM.Runtime
run_tests "$OUT/clean-after.log" "$CLASS"
echo "clean-after: $(grep -E '^(Passed!|Failed!)' "$OUT/clean-after.log")"
echo "# git status --porcelain -> [$(git status --porcelain)]" >> "$OUT/clean-after.log"
echo "ALL WITNESSES DONE"
