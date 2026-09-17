#!/usr/bin/env bash
# E5 and E6 of VM-5-002's re-collection, credit side, in the main checkout at the remedy's head: the
# JavaScript checks plain once and verbose twice, and the corpus replay with host lifetime plain once
# and verbose twice, each through run-logged.sh after one build, with --no-build so the binaries hashed
# are the ones run. Then the masked comparisons against the base transcripts the first collection
# retained, and between the two credit verbose runs.
#   run-e5-e6.sh <outdir>
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
OUT=$1
TREE=D:/Broiler.VM
OLD=D:/Broiler.VM/docs/evidence/vm-5-002
SC=src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler
EO=src/compositions/Broiler.VM.Composition.JavaScript.ExecutionOnly
export DOTNET_CLI_UI_LANGUAGE=en
mkdir -p "$OUT/e5" "$OUT/e6"
"$HERE/run-logged.sh" "$OUT/e5/build-credit.log" credit "$TREE" -- dotnet build $SC -c Release -warnaserror || exit 1
"$HERE/run-logged.sh" "$OUT/e6/build-credit.log" credit "$TREE" -- dotnet build $EO -c Release -warnaserror || exit 1
SCB="$SC/bin/Release/net10.0"; EOB="$EO/bin/Release/net10.0"
"$HERE/run-logged.sh" "$OUT/e5/e5-credit.log" credit "$TREE" $SCB/Broiler.VM.Composition.JavaScript.SliceCompiler.exe $SCB/Broiler.VM.Composition.JavaScript.SliceCompiler.dll $SCB/Broiler.VM.Runtime.dll -- dotnet run --project $SC -c Release --no-build -- --checks; echo "e5 plain exit=$?"
for r in "" "-run2"; do
  "$HERE/run-logged.sh" "$OUT/e5/e5-credit-verbose$r.log" credit "$TREE" $SCB/Broiler.VM.Composition.JavaScript.SliceCompiler.exe $SCB/Broiler.VM.Composition.JavaScript.SliceCompiler.dll $SCB/Broiler.VM.Runtime.dll -- dotnet run --project $SC -c Release --no-build -- --checks --verbose; echo "e5 verbose$r exit=$?"
done
"$HERE/run-logged.sh" "$OUT/e6/e6-credit.log" credit "$TREE" $EOB/Broiler.VM.Composition.JavaScript.ExecutionOnly.exe $EOB/Broiler.VM.Composition.JavaScript.ExecutionOnly.dll $EOB/Broiler.VM.Runtime.dll -- dotnet run --project $EO -c Release --no-build -- --corpus src/tests/corpus/js-1; echo "e6 plain exit=$?"
for r in "" "-run2"; do
  "$HERE/run-logged.sh" "$OUT/e6/e6-credit-verbose$r.log" credit "$TREE" $EOB/Broiler.VM.Composition.JavaScript.ExecutionOnly.exe $EOB/Broiler.VM.Composition.JavaScript.ExecutionOnly.dll $EOB/Broiler.VM.Runtime.dll -- dotnet run --project $EO -c Release --no-build -- --corpus src/tests/corpus/js-1 --verbose; echo "e6 verbose$r exit=$?"
done
{
  for pair in "e5-base-verbose.log e5-credit-verbose.log" "e5-base-verbose.log e5-credit-verbose-run2.log" "e5-base.log e5-credit.log"; do
    set -- $pair; python "$HERE/masked-compare.py" "$OLD/e5/$1" "$OUT/e5/$2"; echo
  done
  python "$HERE/masked-compare.py" "$OUT/e5/e5-credit-verbose.log" "$OUT/e5/e5-credit-verbose-run2.log"
} > "$OUT/e5/masked-compare.log" 2>&1
{
  for pair in "e6-base-verbose.log e6-credit-verbose.log" "e6-base-verbose.log e6-credit-verbose-run2.log" "e6-base.log e6-credit.log"; do
    set -- $pair; python "$HERE/masked-compare.py" "$OLD/e6/$1" "$OUT/e6/$2"; echo
  done
  python "$HERE/masked-compare.py" "$OUT/e6/e6-credit-verbose.log" "$OUT/e6/e6-credit-verbose-run2.log"
} > "$OUT/e6/masked-compare.log" 2>&1
grep -h "lines differing\|line counts" "$OUT/e5/masked-compare.log" "$OUT/e6/masked-compare.log"
echo "E5 E6 DONE"
