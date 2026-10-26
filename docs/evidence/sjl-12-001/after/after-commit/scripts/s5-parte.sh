#!/bin/bash
# WP7.2 step 5 (bundle sjl-12-001 part E): eng/check-frontend-mapping.py, as the corpus commit holds it (the after
# commit's blob is the same, 09e07cd68646bdc9e382e441a806956b35b745fd), run at the after commit's checkout over its
# mapping; then the rule's two controls, copies of that mapping made outside the tree with the one change each names
# (control 1: a shared code given a second registry code, JSE1401 to 2201;2202; control 2: a rule merged onto a
# registry row whose meaning does not cover it, not marked split, JSE1621 marked within-meaning of 2402), each run
# through the checker's --mapping. The copies are also compared with the bundle's retained controls/ copies. The gate
# before each run.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
VMW=$W/vm-after
O=$W/vm
C=$O/controls
MAP=src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv
C1=$C/part-e-control-1-second-registry-code.tsv
C2=$C/part-e-control-2-merge-not-split.tsv
mkdir -p $C
echo "$(utc) s5 start" >> $GATELOG

# ---- the two control copies, outside the tree
awk -F'\t' -v OFS='\t' '$1 == "code" && $2 == "JSE1401" { $7 = "2201;2202" } { print }' $VMW/$MAP > $C1
awk -F'\t' -v OFS='\t' '$1 == "code" && $2 == "JSE1621" { $8 = "within-meaning"; $9 = "2402 DuplicateExportName: One module publishes the same export name twice." } { print }' $VMW/$MAP > $C2
{ for n in 1-second-registry-code 2-merge-not-split; do
    echo "## controls/part-e-control-$n.tsv against the bundle's retained copy (origin/main $(git -C /home/user/Broiler.VM rev-parse origin/main))"
    cmp $C/part-e-control-$n.tsv <(git -C /home/user/Broiler.VM show origin/main:docs/evidence/sjl-12-001/controls/part-e-control-$n.tsv); echo "cmp exit $?"
  done; } > $C/part-e-controls-against-retained.txt 2>&1

run parte-mapping $VMW python3 -I eng/check-frontend-mapping.py
run parte-control-1 $VMW python3 -I eng/check-frontend-mapping.py --mapping $C1
run parte-control-2 $VMW python3 -I eng/check-frontend-mapping.py --mapping $C2

# ---- the record, in the shape of the bundle's controls/part-e-checker-runs.txt
{
  echo "Bundle sjl-12-001, part E at the after commit 1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f: the checker on the mapping and on"
  echo "its two control copies. eng/check-frontend-mapping.py as the after commit holds it (the corpus commit's blob),"
  echo "run from the after commit's clean checkout. Each control is the mapping with the one change its diff shows, made"
  echo "outside the tree. Python $(python3 --version 2>&1 | cut -d' ' -f2). Standard output and standard error together, in order of the files."
  echo
  echo "== The mapping"
  echo
  echo "\$ python3 -I eng/check-frontend-mapping.py"
  cat $R/parte-mapping.out $R/parte-mapping.err
  echo "exit status $(awk -F'\t' '$1 == "parte-mapping" { sub("rc=", "", $2); print $2 }' $R/status.tsv)"
  for n in 1 2; do
    if [ $n = 1 ]; then F=$C1; T="Control 1: a shared code given a second registry code (JSE1401, EE0040, mapped to 2201 and 2202)"
    else F=$C2; T="Control 2: a rule merged onto a registry row whose meaning does not cover it, not marked split (JSE1621, EE0180, marked within-meaning of 2402 DuplicateExportName)"; fi
    echo
    echo "== $T"
    echo
    echo "\$ diff $MAP controls/$(basename $F)"
    (cd $VMW && diff $MAP $F)
    echo
    echo "\$ python3 -I eng/check-frontend-mapping.py --mapping controls/$(basename $F)"
    cat $R/parte-control-$n.out $R/parte-control-$n.err
    echo "exit status $(awk -F'\t' -v k=parte-control-$n '$1 == k { sub("rc=", "", $2); print $2 }' $R/status.tsv)"
  done
} > $O/part-e-checker-runs.txt 2>&1
echo "$(utc) s5 done" >> $GATELOG
echo "s5 done"
