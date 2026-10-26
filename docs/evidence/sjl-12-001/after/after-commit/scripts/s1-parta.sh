#!/bin/bash
# WP7.2 step 1 (bundle sjl-12-001 part A): the after commit's clean checkout is made; the base and after hosted
# artifacts are compared through eng/compare-test262-forms.py --same-form, the bytecode pair as it is and the
# native pair with --exempt-guest-loads over the suite extracted from the pinned archive. Comparisons only: no
# build and no run, so no gate (as WP4.7's stage C ran its comparisons). Each output is retained.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
AC=1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f      # the after commit
RC=889ea1568f59bf37aa21c0035b423d0d030eb87f      # the rule commit (part A's base)
VMW=$W/vm-after                                   # the after commit's clean checkout
O=$W/vm                                           # the outputs (laid out as the bundle's after/)
L=$O/after-commit/logs
P=$W/parta                                        # scratch: the reports, decompressed
X=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7-vm-hosted/x
ZIP=/tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7-vm-hosted/dl/test262-evidence.zip
mkdir -p $L $P
echo "$(utc) s1 start" >> $GATELOG

# ---- the after commit is on the default branch; its clean detached checkout, named by all forty digits
git -C /home/user/Broiler.VM fetch -q origin main
git -C /home/user/Broiler.VM merge-base --is-ancestor $AC origin/main; echo "merge-base --is-ancestor $AC origin/main: exit $?" > $L/after-commit-ancestry.txt
git -C /home/user/Broiler.VM merge-base --is-ancestor $RC $AC; echo "merge-base --is-ancestor $RC $AC: exit $?" >> $L/after-commit-ancestry.txt
git -C /home/user/Broiler.VM rev-parse origin/main >> $L/after-commit-ancestry.txt
git -C /home/user/Broiler.VM worktree add --detach $VMW $AC > $R/worktree-add-after.log 2>&1
{ echo "# after the checkout was made, before anything ran in it"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain --ignored; } > $L/checkout-after-commit-made.txt

# ---- the base's hosted folder, as the default branch retains it (docs/evidence/sjl-12-001/base/hosted/), its
#      hashes.txt checked; the after artifact's zip digest and its extraction checked
mkdir -p $P/base-src
git -C /home/user/Broiler.VM archive origin/main docs/evidence/sjl-12-001/base/hosted | tar -x -C $P/base-src
BH=$P/base-src/docs/evidence/sjl-12-001/base/hosted
{ echo "## base: (cd docs/evidence/sjl-12-001/base/hosted at origin/main $(git -C /home/user/Broiler.VM rev-parse origin/main) && sha256sum -c hashes.txt)"
  (cd $BH && sha256sum -c hashes.txt; echo "exit $?")
  echo "## after: sha256sum test262-evidence.zip (run 40231328560, artifact 12613262582; GitHub's digest sha256:9ac186958365b1e926f8cb59adeecf10260c1138afab24314642f981bb46b7fb)"
  sha256sum $ZIP
  echo "## after: the zip extracted afresh and compared with the extraction read below (diff -r)"
  rm -rf $P/zipx; mkdir -p $P/zipx; (cd $P/zipx && unzip -q $ZIP); diff -r $P/zipx $X; echo "diff exit $?"; unzip -l $ZIP | tail -1
  rm -rf $P/zipx
  echo "## run.txt of each"
  echo "# base"; cat $BH/run.txt; echo "# after"; cat $X/run.txt
} > $O/hosted-check.txt 2>&1

# ---- completeness, read from each folder (scripts/parta_complete.py prints, decides nothing)
{ for side in base after; do
    if [ $side = base ]; then F=$BH; else F=$X; fi
    for run in wide-bytecode wide-native; do echo "# $side"; python3 -I $W/scripts/parta_complete.py $F $run; done
  done
} > $O/hosted-completeness.txt 2>&1

# ---- the suite extracted from the pinned archive the after commit holds (its SHA-256 checked against test262.pin)
A=$VMW/src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz
{ sha256sum $A; grep '^archive-sha256' $VMW/src/tests/conformance/pins/test262.pin; } > $L/suite-archive.txt
mkdir -p $W/suite-a
tar xzf $A -C $W/suite-a
SA=$W/suite-a/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e

# ---- the reports, decompressed for the script, which reads plain text
for run in wide-bytecode wide-native; do
  gunzip -c $BH/$run.report.gz > $P/base-$run.report
  gunzip -c $X/$run.report.gz > $P/after-$run.report
done
sha256sum $P/*.report > $L/parta-reports.sha256

# ---- the comparisons, eng/compare-test262-forms.py as the after commit holds it (the rule commit's blob,
#      48e3091a872921849bab1f903b46698be0c69e33, at both), run from the checkout's root
cmp_one() {
  local run=$1; shift
  {
    echo "\$ compare-test262-forms.py <base $run> <after $run> $*"
    (cd $VMW && python3 -I eng/compare-test262-forms.py $P/base-$run.report $P/after-$run.report "$@" 2> $L/compare-$run.stderr.txt)
    echo "exit code: $?"
  } > $O/compare-$run.log
}
cmp_one wide-bytecode --same-form --suite $SA
cmp_one wide-native --same-form --exempt-guest-loads --suite $SA
{ echo "# after step 1"; git -C $VMW rev-parse HEAD; git -C $VMW status --porcelain --ignored; } > $L/checkout-after-commit-after-step1.txt
echo "$(utc) s1 done" >> $GATELOG
echo "s1 done"
tail -1 $O/compare-wide-bytecode.log $O/compare-wide-native.log
