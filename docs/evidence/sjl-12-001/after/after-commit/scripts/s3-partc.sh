#!/bin/bash
# WP7.2 step 3 (bundle sjl-12-001 part C at the after commit): runs 2 (one worker) and 3 (--twice-concurrent), with
# journals checked and deleted; the slice compiler's --checks --verbose (part C's two rows and part D's two); then
# eng/compare-parse-corpus.py determinism over runs 1 to 3 with part B's after run (run 1) as the population report.
# The same build as step 2. The gate before every run; the comparison is not gated.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
VMW=$W/vm-after
O=$W/vm
L=$O/after-commit/logs
SC=src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler
V=$VMW/$SC/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.dll
M=$VMW/src/tests/corpus/parse-1/corpus.manifest
echo "$(utc) s3 start" >> $GATELOG

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

run parse-corpus-after-run2 $VMW dotnet $V --parse-corpus $M $R/parse-corpus-after-run2.tsv.gz --workers 1 --journal $R/parse-corpus-after-run2.journal
journal parse-corpus-after-run2
run parse-corpus-after-run3 $VMW dotnet $V --parse-corpus $M $R/parse-corpus-after-run3.tsv.gz --twice-concurrent --journal $R/parse-corpus-after-run3.journal
journal parse-corpus-after-run3
run checks-after $VMW dotnet $V --checks --verbose

mv $R/parse-corpus-after-run2.tsv.gz $R/parse-corpus-after-run3.tsv.gz $O/
for n in 2 3; do python3 -I $W/scripts/vm_complete.py $M $O/parse-corpus-after-run$n.tsv.gz > $L/complete-parse-corpus-after-run$n.txt 2>&1; done
gzip -n -9 -c $R/checks-after.out > $O/checks-after.log.gz
grep -E "^[A-Za-z]+ +" $R/checks-after.out | grep -E "deterministic|source order|tree-depth bound|nesting bound" > $O/checks-after-named-rows.txt
tail -1 $R/checks-after.out >> $O/checks-after-named-rows.txt

B=docs/evidence/sjl-12-001
A=$B/after
for n in 1 2 3; do cp $O/parse-corpus-after-run$n.tsv.gz $VMW/$A/; done
(cd $VMW && python3 -I eng/compare-parse-corpus.py determinism $A/parse-corpus-after-run1.tsv.gz $A/parse-corpus-after-run2.tsv.gz \
  $A/parse-corpus-after-run3.tsv.gz --population $A/parse-corpus-after-run1.tsv.gz --manifest src/tests/corpus/parse-1/corpus.manifest \
  --out $A/determinism-after-runs-1-2-3.tsv > $L/determinism-after.stdout.txt 2> $L/determinism-after.stderr.txt)
echo "determinism exit $?" > $L/determinism-after.exit.txt
cp $VMW/$A/determinism-after-runs-1-2-3.tsv $O/
rm -f $VMW/$A/parse-corpus-after-run1.tsv.gz $VMW/$A/parse-corpus-after-run2.tsv.gz $VMW/$A/parse-corpus-after-run3.tsv.gz \
  $VMW/$A/determinism-after-runs-1-2-3.tsv
{ echo "# after step 3"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain; } > $L/checkout-after-step3.txt
echo "$(utc) s3 done" >> $GATELOG
echo "s3 done"
