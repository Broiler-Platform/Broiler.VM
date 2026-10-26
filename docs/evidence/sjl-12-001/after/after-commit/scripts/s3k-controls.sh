#!/bin/bash
# WP7.2 step 3, controls (bundle sjl-12-001 part C's K1 and K2): each a patch to a scratch copy of the after commit
# (a clean detached worktree of its own, never committed), built in Release, run, then reverted and the revert
# checked against the after commit. K1 reverses the global lexical declarations: the source-order checks row must
# fail, and part B's comparison over the patched build must report at least one byte difference. K2 shuffles them
# per compilation from an unseeded source: run 1 against run 2, or run 3's two compilations, must differ, and the
# determinism row must fail. After both reverts the copy is built again and its checks run again. The gate before
# every build and run; comparisons are not gated.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
AC=1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f
VMW=$W/vm-after                                   # the after commit's checkout (comparisons run from its root)
KW=$W/vm-k                                        # the scratch copy
O=$W/vm
C=$O/controls
CL=$C/logs
SC=src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler
F=src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.Hoisting.cs
KV=$KW/$SC/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.dll
KM=$KW/src/tests/corpus/parse-1/corpus.manifest
mkdir -p $CL
echo "$(utc) s3k start" >> $GATELOG

journal() {   # WP4.7's stage-b.sh journal check, unchanged
  local n=$1 j=$R/$1.journal
  if [ -f $j ]; then
    {
      echo "=== $n journal: $(wc -l < $j) lines, $(stat -c %s $j) bytes"
      awk -F'\t' '{ c[$1]++ } END { printf "begin %d, end %d\n", c["begin"], c["end"] }' $j
      awk -F'\t' '{ k = $2 "\t" $3 "\t" $4 "\t" $5 "\t" $6 } $1 == "begin" { open[k]++ } $1 == "end" { open[k]-- }
        END { n = 0; for (k in open) if (open[k] != 0) { n++; if (n <= 5) print "unbalanced: " k } print "keys begun and not ended: " n }' $j
    } >> $R/journal-check.txt
    rm -f $j
  else
    echo "=== $n journal: missing" >> $R/journal-check.txt
  fi
}
named() { grep -E "^[A-Za-z]+ +" $1 | grep -E "deterministic|source order|tree-depth bound|nesting bound"; tail -1 $1; }
revert_check() {   # NAME: the revert, then the copy against the after commit
  git -C $KW checkout -- $F
  {
    echo "# $1 reverted: git checkout -- $F"
    git -C $KW rev-parse HEAD
    git -C $KW diff --exit-code; echo "git diff --exit-code (worktree against HEAD): exit $?"
    git -C $KW diff --exit-code $AC; echo "git diff --exit-code $AC: exit $?"
    echo "git status --porcelain:"; git -C $KW status --porcelain
  } > $C/$1-revert-check.txt 2>&1
}

git -C /home/user/Broiler.VM worktree add --detach $KW $AC > $R/worktree-add-k.log 2>&1
{ echo "# the scratch copy, as made"; git -C $KW rev-parse HEAD; git -C $KW status --porcelain --ignored; } > $CL/checkout-k-made.txt

B=docs/evidence/sjl-12-001
A=$B/after
CC=$B/controls
cp $O/parse-corpus-after-run1.tsv.gz $VMW/$A/

# ---- K1
cp $W/kpatch/k1/JsCompiler.Hoisting.cs $KW/$F
git -C $KW diff > $C/k1.patch
run k1-build $KW dotnet build -c Release -nologo --disable-build-servers $KW/$SC/Broiler.VM.Composition.JavaScript.SliceCompiler.csproj
run k1-checks $KW dotnet $KV --checks --verbose
run k1-parse-corpus $KW dotnet $KV --parse-corpus $KM $R/k1-parse-corpus.tsv.gz --workers 1 --journal $R/k1-parse-corpus.journal
journal k1-parse-corpus
revert_check k1
mv $R/k1-parse-corpus.tsv.gz $C/
gzip -n -9 -c $R/k1-checks.out > $C/k1-checks.log.gz
named $R/k1-checks.out > $C/k1-checks-named-rows.txt
cp $C/k1-parse-corpus.tsv.gz $VMW/$CC/
for side in base after; do
  if [ $side = base ]; then BR=$B/base/parse-corpus-run1.tsv.gz; else BR=$A/parse-corpus-after-run1.tsv.gz; fi
  (cd $VMW && python3 -I eng/compare-parse-corpus.py identity $BR $CC/k1-parse-corpus.tsv.gz \
    --manifest src/tests/corpus/parse-1/corpus.manifest --no-census --out $CC/k1-identity-$side.tsv \
    > $CL/k1-identity-$side.stdout.txt 2> $CL/k1-identity-$side.stderr.txt)
  echo "identity exit $?" > $CL/k1-identity-$side.exit.txt
  gzip -n -9 -c $VMW/$CC/k1-identity-$side.tsv > $C/k1-identity-$side.tsv.gz
  rm -f $VMW/$CC/k1-identity-$side.tsv
done
rm -f $VMW/$CC/k1-parse-corpus.tsv.gz

# ---- K2
cp $W/kpatch/k2/JsCompiler.Hoisting.cs $KW/$F
git -C $KW diff > $C/k2.patch
run k2-build $KW dotnet build -c Release -nologo --disable-build-servers $KW/$SC/Broiler.VM.Composition.JavaScript.SliceCompiler.csproj
run k2-checks $KW dotnet $KV --checks --verbose
run k2-parse-corpus-run1 $KW dotnet $KV --parse-corpus $KM $R/k2-parse-corpus-run1.tsv.gz --workers 1 --journal $R/k2-parse-corpus-run1.journal
journal k2-parse-corpus-run1
run k2-parse-corpus-run2 $KW dotnet $KV --parse-corpus $KM $R/k2-parse-corpus-run2.tsv.gz --workers 1 --journal $R/k2-parse-corpus-run2.journal
journal k2-parse-corpus-run2
run k2-parse-corpus-run3 $KW dotnet $KV --parse-corpus $KM $R/k2-parse-corpus-run3.tsv.gz --twice-concurrent --journal $R/k2-parse-corpus-run3.journal
journal k2-parse-corpus-run3
revert_check k2
mv $R/k2-parse-corpus-run1.tsv.gz $R/k2-parse-corpus-run2.tsv.gz $R/k2-parse-corpus-run3.tsv.gz $C/
gzip -n -9 -c $R/k2-checks.out > $C/k2-checks.log.gz
named $R/k2-checks.out > $C/k2-checks-named-rows.txt
for n in 1 2 3; do cp $C/k2-parse-corpus-run$n.tsv.gz $VMW/$CC/; done
(cd $VMW && python3 -I eng/compare-parse-corpus.py determinism $CC/k2-parse-corpus-run1.tsv.gz $CC/k2-parse-corpus-run2.tsv.gz \
  $CC/k2-parse-corpus-run3.tsv.gz --population $A/parse-corpus-after-run1.tsv.gz --manifest src/tests/corpus/parse-1/corpus.manifest \
  --out $CC/k2-determinism.tsv > $CL/k2-determinism.stdout.txt 2> $CL/k2-determinism.stderr.txt)
echo "determinism exit $?" > $CL/k2-determinism.exit.txt
gzip -n -9 -c $VMW/$CC/k2-determinism.tsv > $C/k2-determinism.tsv.gz
rm -f $VMW/$CC/k2-determinism.tsv $VMW/$CC/k2-parse-corpus-run1.tsv.gz $VMW/$CC/k2-parse-corpus-run2.tsv.gz $VMW/$CC/k2-parse-corpus-run3.tsv.gz
rm -f $VMW/$A/parse-corpus-after-run1.tsv.gz

# ---- after both reverts: the copy built again and its checks run again
run k-reverted-build $KW dotnet build -c Release -nologo --disable-build-servers $KW/$SC/Broiler.VM.Composition.JavaScript.SliceCompiler.csproj
run k-reverted-checks $KW dotnet $KV --checks --verbose
gzip -n -9 -c $R/k-reverted-checks.out > $C/k-reverted-checks.log.gz
named $R/k-reverted-checks.out > $C/k-reverted-checks-named-rows.txt
{ echo "# the scratch copy, after both reverts and the rebuild"; git -C $KW rev-parse HEAD; git -C $KW status --porcelain; } > $CL/checkout-k-end.txt
{ echo "# the after checkout, after the controls' comparisons"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain; } > $O/after-commit/logs/checkout-after-controls.txt
echo "$(utc) s3k done" >> $GATELOG
echo "s3k done"
