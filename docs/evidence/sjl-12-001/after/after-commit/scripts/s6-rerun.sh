#!/bin/bash
# WP7.2 step 6 (bundle sjl-12-001 part A, classes C4 (c') and (e)): the conformance root built in Release at the rule
# commit (part A's base) and at the after commit, each in its clean checkout; then every (c') row of step 1's two
# comparisons run again alone at both commits, each run one process of the conformance root over the row's path,
# under the part's manifest (broiler.javascript.wide), the pair's form and the part's allowances (fuel 100000000,
# wall 5000 ms), with the suite extracted from the pinned archive and checked through --expect; a path that holds two
# (c') rows (its sloppy and strict variants) is one process, which runs both. Then, for each row whose two re-runs
# differ, (e): one process over the row's path once more at each commit with the wall-clock allowance ten times the
# part's (50000 ms) and nothing else on the machine. The gate before every build and every run.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
VMW=$W/vm-after
RW=$W/vm-rule
O=$W/vm
L=$O/after-commit/logs
RR=$O/rerun
CF=src/compositions/Broiler.VM.Composition.JavaScript.Conformance
SUITE=$W/suite-a/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
FUEL=100000000
WALL=5000
RACE_WALL=50000
mkdir -p $RR/reports $RR/logs
echo "$(utc) s6 start" >> $GATELOG

for side in rule after; do
  if [ $side = rule ]; then tree=$RW; else tree=$VMW; fi
  { echo "# before the conformance root's build"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-conformance-$side-before-build.txt
  run build-conformance-$side $tree dotnet build -c Release -nologo --disable-build-servers $tree/$CF/Broiler.VM.Composition.JavaScript.Conformance.csproj
  { echo "# after the conformance root's build"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-conformance-$side-after-build.txt
  (cd $tree/$CF/bin/Release/net10.0 && sha256sum *.dll | sort -k2) > $RR/binaries-conformance-$side.sha256
  cp $R/build-conformance-$side.out $RR/logs/build-conformance-$side.log
done

one() {   # NAME TREE FORM WALL PATH REPORT
  local name=$1 tree=$2 form=$3 wall=$4 path=$5 report=$6
  run $name $tree $tree/$CF/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance --test262 $SUITE \
    --manifest broiler.javascript.wide --expect $tree/src/tests/conformance/pins/test262.pin --report $report \
    --fuel $FUEL --wall $wall --form $form --test $path
  cp $R/$name.out $RR/logs/$name.stdout.txt; cp $R/$name.err $RR/logs/$name.stderr.txt
}

for run_name in wide-bytecode wide-native; do
  form=${run_name#wide-}
  python3 -I $W/scripts/rerun_table.py rows $O/compare-$run_name.log > $RR/rows-$run_name.tsv
  cut -f1 $RR/rows-$run_name.tsv | LC_ALL=C sort -u > $RR/paths-$run_name.txt
  for side in rule after; do
    if [ $side = rule ]; then tree=$RW; else tree=$VMW; fi
    mkdir -p $R/rerun-$run_name-$side
    i=0
    while read -r path; do
      one rerun-$run_name-$side-$i $tree $form $WALL $path $R/rerun-$run_name-$side/rerun-$i.report
      i=$((i + 1))
    done < $RR/paths-$run_name.txt
  done
  python3 -I $W/scripts/rerun_table.py table $O/compare-$run_name.log $R/rerun-$run_name-rule $R/rerun-$run_name-after rerun \
    $RR/rerun-$run_name.tsv > $RR/differ-$run_name.tsv
  # each path's two re-runs compared with the after commit's eng/compare-test262-forms.py --same-form, as bundle
  # sjl-4-001's step compares its two re-runs
  i=0
  while read -r path; do
    extra=""; [ $form = native ] && extra="--exempt-guest-loads"
    {
      echo "\$ compare-test262-forms.py <rule-commit re-run $i> <after-commit re-run $i> --same-form $extra --suite <suite>   # $path"
      (cd $VMW && python3 -I eng/compare-test262-forms.py $R/rerun-$run_name-rule/rerun-$i.report $R/rerun-$run_name-after/rerun-$i.report \
        --same-form $extra --suite $SUITE 2>&1)
      echo "exit code: $?"
      echo
    } >> $RR/rerun-$run_name-compare.log
    i=$((i + 1))
  done < $RR/paths-$run_name.txt
  for side in rule after; do
    for f in $R/rerun-$run_name-$side/*.report; do gzip -n -9 -c $f > $RR/reports/$run_name-$side-$(basename $f).gz; done
  done
done

# ---- (e): each row whose re-runs differ, once more at each commit, the wall clock ten times the part's
for run_name in wide-bytecode wide-native; do
  form=${run_name#wide-}
  if [ ! -s $RR/differ-$run_name.tsv ]; then echo "$run_name: no (c') row whose re-runs differ" >> $RR/wall-race-none.txt; continue; fi
  cut -f1 $RR/differ-$run_name.tsv | LC_ALL=C sort -u > $RR/race-paths-$run_name.txt
  for side in rule after; do
    if [ $side = rule ]; then tree=$RW; else tree=$VMW; fi
    mkdir -p $R/race-$run_name-$side
    # the race report index is the path's index among the differing rows' paths, sorted
    i=0
    while read -r path; do
      one wall-race-$run_name-$side-$i $tree $form $RACE_WALL $path $R/race-$run_name-$side/race-$i.report
      i=$((i + 1))
    done < $RR/race-paths-$run_name.txt
  done
  python3 -I $W/scripts/rerun_table.py race $RR/differ-$run_name.tsv $R/race-$run_name-rule $R/race-$run_name-after race $RR/wall-race-$run_name.tsv
  for side in rule after; do
    for f in $R/race-$run_name-$side/*.report; do gzip -n -9 -c $f > $RR/reports/wall-race-$run_name-$side-$(basename $f).gz; done
  done
done
echo "$(utc) s6 done" >> $GATELOG
echo "s6 done"
