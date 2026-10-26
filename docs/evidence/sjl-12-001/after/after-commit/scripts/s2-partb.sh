#!/bin/bash
# WP7.2 step 2 (bundle sjl-12-001 part B at the after commit): the slice compiler built in Release from the clean
# checkout; --census over parse-1/inputs and over the extracted suite's test/; --parse-corpus over the whole manifest
# with one worker and a journal (part B's after run, and part C's run 1); the journal checked and deleted; the report
# checked against the manifest; then eng/compare-parse-corpus.py identity, base run 1 against the after run, with both
# census pairs. The gate (lib.sh, WP4.7's) before the build and before every run. Comparisons are not gated.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
AC=1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f
VMW=$W/vm-after
O=$W/vm
L=$O/after-commit/logs
SC=src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler
V=$VMW/$SC/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.dll
M=$VMW/src/tests/corpus/parse-1/corpus.manifest
SA=$W/suite-a/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e/test
echo "$(utc) s2 start" >> $GATELOG

journal() {   # WP4.7's stage-b.sh journal check, unchanged but for the output file
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

{ echo "# before the build"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain --ignored; } > $L/checkout-before-build.txt
run build-after $VMW dotnet build -c Release -nologo --disable-build-servers $VMW/$SC/Broiler.VM.Composition.JavaScript.SliceCompiler.csproj
{ echo "# after the build"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain; } > $L/checkout-after-build.txt
(cd $VMW/$SC/bin/Release/net10.0 && sha256sum *.dll | sort -k2) > $O/after-commit/binaries.sha256

run census-inputs-after $VMW dotnet $V --census $VMW/src/tests/corpus/parse-1/inputs
run census-test-after $VMW dotnet $V --census $SA
run parse-corpus-after-run1 $VMW dotnet $V --parse-corpus $M $R/parse-corpus-after-run1.tsv.gz --workers 1 --journal $R/parse-corpus-after-run1.journal
journal parse-corpus-after-run1

# ---- staging: the report and the census where the bundle's after/ will hold them
mv $R/parse-corpus-after-run1.tsv.gz $O/parse-corpus-after-run1.tsv.gz
for n in inputs test; do gzip -n -9 -c $R/census-$n-after.out > $O/census-$n-after.txt.gz; done
python3 -I $W/scripts/vm_complete.py $M $O/parse-corpus-after-run1.tsv.gz > $L/complete-parse-corpus-after-run1.txt 2>&1

# ---- the comparison, eng/compare-parse-corpus.py as the corpus commit holds it (the after commit's blob is the
#      same, c1d3b53d957c1a23f409b466f667b0733772863d), run from the checkout's root with the after files copied to the
#      paths the bundle will commit them at (removed afterwards), as the ordering run did
B=docs/evidence/sjl-12-001
A=$B/after
for f in parse-corpus-after-run1.tsv.gz census-inputs-after.txt.gz census-test-after.txt.gz; do cp $O/$f $VMW/$A/$f; done
(cd $VMW && python3 -I eng/compare-parse-corpus.py identity $B/base/parse-corpus-run1.tsv.gz $A/parse-corpus-after-run1.tsv.gz \
  --manifest src/tests/corpus/parse-1/corpus.manifest \
  --census $B/base/census-inputs-1.txt.gz $A/census-inputs-after.txt.gz --census $B/base/census-test-1.txt.gz $A/census-test-after.txt.gz \
  --out $A/identity-base-after.tsv --differing-keys $A/differing-keys-base-after.txt \
  > $L/identity-base-after.stdout.txt 2> $L/identity-base-after.stderr.txt)
echo "identity exit $?" > $L/identity-base-after.exit.txt
cp $VMW/$A/identity-base-after.tsv $VMW/$A/differing-keys-base-after.txt $O/
rm -f $VMW/$A/parse-corpus-after-run1.tsv.gz $VMW/$A/census-inputs-after.txt.gz $VMW/$A/census-test-after.txt.gz \
  $VMW/$A/identity-base-after.tsv $VMW/$A/differing-keys-base-after.txt
# the census, base against after, byte for byte, also read outside the script
{ for n in inputs test; do
    echo "census-$n: base $(zcat $VMW/$B/base/census-$n-1.txt.gz | sha256sum | cut -d' ' -f1) after $(zcat $O/census-$n-after.txt.gz | sha256sum | cut -d' ' -f1)"
    cmp <(zcat $VMW/$B/base/census-$n-1.txt.gz) <(zcat $O/census-$n-after.txt.gz); echo "cmp exit $?"
  done; } > $L/census-base-after-cmp.txt 2>&1
{ echo "# after step 2"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain; } > $L/checkout-after-step2.txt
echo "$(utc) s2 done" >> $GATELOG
echo "s2 done"
