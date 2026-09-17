#!/usr/bin/env bash
# E3 of VM-5-002's re-collection: every witness of witnesses.py applied alone to a worktree outside the
# repository at the remedy head. For each: the edit applied and its diff kept; the contract tests built;
# the fuel-exactness class, both per-thread leak tests and the restore-path test run under the defect;
# for a sampled witness (W4, W24) whose test did not fail, that test alone run again, up to ten runs in
# all, until it fails; the tree restored; the same tests run clean as a control; and the named tests
# run alone, clean, so each witness has a passing transcript of its own. One clean run before the first
# witness. Every transcript begins with identity lines written by identity.py after the build and before
# the test, and is kept whatever it shows.
#   run-witnesses.sh <worktree> <outdir> [witness ...]
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
WT=$1; OUT=$2; shift 2
PY="$HERE/witnesses.py"
export DOTNET_CLI_UI_LANGUAGE=en
mkdir -p "$OUT"
cd "$WT" || exit 1

FILTER="FullyQualifiedName~FuelChargeExactnessTests|FullyQualifiedName~A_Disposed_Runtime_Leaves_No_Per_Thread_State_Behind|FullyQualifiedName~A_Capability_That_Changes_Nothing_Returns_Its_Caller"
T6="FullyQualifiedName~FuelChargeExactnessTests.Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit"
T26="FullyQualifiedName~FuelChargeExactnessTests.A_Meter_Charged_From_Two_Threads_Is_Polled_On_Exactly_The_Work_Between_Polls"
TB=src/tests/Broiler.VM.Contract.Tests/bin/Release/net10.0

run_tests() { # $1 = log, $2 = filter, $3 = build label
  local tmp="$1.build.tmp"
  dotnet build src/tests/Broiler.VM.Contract.Tests -c Release > "$tmp" 2>&1
  local built=$?
  if ! python "$HERE/identity.py" --build "$3" --tree "$WT" \
        --bin "$WT/$TB/Broiler.VM.Runtime.dll" --bin "$WT/$TB/Broiler.VM.Contract.Tests.dll" > "$1"; then
    cat "$1"; rm -f "$tmp"; echo "STOPPED: branch check failed"; exit 97
  fi
  {
    echo "# build-command=dotnet build src/tests/Broiler.VM.Contract.Tests -c Release (exit $built)"
    cat "$tmp"
    echo "# command=dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --no-build --filter \"$2\" --logger console;verbosity=normal"
  } >> "$1"
  rm -f "$tmp"
  if [ $built -eq 0 ]; then
    dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --no-build --filter "$2" --logger "console;verbosity=normal" >> "$1" 2>&1
    echo "# exit=$?" >> "$1"
  else
    echo "# exit=build-failed" >> "$1"
  fi
  echo "# finished=$(date -Iseconds)" >> "$1"
}

summary() { # $1 = log
  local total failed passed
  total=$(grep -E "^Total tests:" "$1" | head -1 | awk '{print $3}')
  passed=$(grep -E "^\s+Passed: [0-9]+" "$1" | head -1 | awk '{print $2}')
  failed=$(grep -E "^\s+Failed: [0-9]+" "$1" | head -1 | awk '{print $2}')
  echo "total=${total:-?} passed=${passed:-0} failed=${failed:-0} compile-errors=$(grep -c "error CS" "$1")"
}

alone_filter() { # the named tests of a witness, by method, joined into one filter
  python "$PY" "$WT" mustfail "$1" | sed -E 's/\(.*$//' | sort -u | while IFS= read -r m; do
    case "$m" in
      A_Disposed_Runtime_*|A_Capability_That_Changes_Nothing_*) echo "FullyQualifiedName~ReviewRegressionTests.$m" ;;
      *) echo "FullyQualifiedName~FuelChargeExactnessTests.$m" ;;
    esac
  done | paste -sd'|'
}

WITNESSES="$*"
[ -z "$WITNESSES" ] && WITNESSES=$(python "$PY" "$WT" names)

git checkout -q -- src/Broiler.VM.Runtime
run_tests "$OUT/clean-before.log" "$FILTER" "wit2 clean, before the first witness"
echo "clean-before: $(summary "$OUT/clean-before.log")"

for w in $WITNESSES; do
  git checkout -q -- src/Broiler.VM.Runtime
  if ! python "$PY" "$WT" "$w" > "$OUT/$w.apply.txt" 2>&1; then
    echo "$w: REFUSED"; cat "$OUT/$w.apply.txt"; continue
  fi
  git diff -- src/Broiler.VM.Runtime >> "$OUT/$w.apply.txt"
  run_tests "$OUT/$w.log" "$FILTER" "wit2 with $w injected"
  echo "$w: $(summary "$OUT/$w.log")"
  if [ "$w" = W4 ] || [ "$w" = W24 ]; then
    if [ "$w" = W4 ]; then ALONE="$T6"; NAME=Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit
    else ALONE="$T26"; NAME=A_Meter_Charged_From_Two_Threads_Is_Polled_On_Exactly_The_Work_Between_Polls; fi
    if ! grep -E "^\s+Failed Broiler" "$OUT/$w.log" | grep -qF "$NAME"; then
      for i in 2 3 4 5 6 7 8 9 10; do
        run_tests "$OUT/$w-iteration-$i.log" "$ALONE" "wit2 with $w injected, sampled test alone, run $i"
        echo "  $w iteration $i: $(summary "$OUT/$w-iteration-$i.log")"
        grep -qE "^\s+Failed Broiler" "$OUT/$w-iteration-$i.log" && break
      done
    fi
  fi
  git checkout -q -- src/Broiler.VM.Runtime
  echo "# after revert: git status --porcelain --untracked-files=no -> [$(git status --porcelain --untracked-files=no)]" >> "$OUT/$w.log"
  run_tests "$OUT/$w.control.log" "$FILTER" "wit2 after $w reverted, control"
  echo "  $w control: $(summary "$OUT/$w.control.log")"
  run_tests "$OUT/$w.alone.log" "$(alone_filter "$w")" "wit2 after $w reverted, named tests alone"
  echo "  $w named tests alone: $(summary "$OUT/$w.alone.log")"
done

git checkout -q -- src/Broiler.VM.Runtime
echo "# final git status --porcelain --untracked-files=no -> [$(git status --porcelain --untracked-files=no)]" > "$OUT/final-status.txt"
echo "ALL WITNESSES DONE"
