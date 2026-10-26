#!/bin/bash
# WP7.2 step 4 (bundle sjl-12-001 part D): dotnet test of Broiler.VM.Architecture.Tests in Release, with
# BROILER_RULE_MESSAGES naming a directory of the outputs, at the after commit (its clean checkout, the one steps 2
# and 3 built in) and at the rule commit (a clean detached checkout of its own, made here). The test log (console,
# normal verbosity, and a TRX file written outside the tree) and the rule-message files are retained. The gate
# before each run; dotnet test builds what it runs.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
AC=1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f
RC=889ea1568f59bf37aa21c0035b423d0d030eb87f
VMW=$W/vm-after
RW=$W/vm-rule
O=$W/vm
L=$O/after-commit/logs
T=src/tests/Broiler.VM.Architecture.Tests
echo "$(utc) s4 start" >> $GATELOG

partd() {   # NAME TREE OUTDIR
  local name=$1 tree=$2 out=$3
  mkdir -p $out/rule-messages $R/$name-results
  { echo "# before dotnet test"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-$name-before.txt
  run $name $tree env BROILER_RULE_MESSAGES=$out/rule-messages dotnet test $tree/$T -c Release --nologo --disable-build-servers \
    --results-directory $R/$name-results --logger "console;verbosity=normal" --logger "trx;LogFileName=$name.trx"
  { echo "# after dotnet test"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-$name-after.txt
  cp $R/$name.out $out/test.log
  cp $R/$name.err $out/test.stderr.txt
  tail -1 $R/status.tsv > $out/test.exit.txt
  for f in $R/$name-results/*.trx; do [ -f "$f" ] && gzip -n -9 -c "$f" > $out/$(basename "$f").gz; done
  grep -E "^\s*(Passed|Failed|Skipped)\s" $R/$name.out | grep -E "DiagnosticRegistryRuleTests|CompilationStackRuleTests" > $out/rule-tests-n5-n9-n12-n19.txt
  grep -E "^(Passed!|Failed!)|Total tests|Passed:|Failed:|Skipped:" $R/$name.out > $out/test-summary.txt
}

# ---- at the after commit
partd partd-after $VMW $O/rules-after

# ---- at the rule commit: its clean detached checkout
git -C /home/user/Broiler.VM worktree add --detach $RW $RC > $R/worktree-add-rule.log 2>&1
{ echo "# the rule commit's checkout, as made"; git -C $RW rev-parse HEAD; git -C $RW status --porcelain --ignored; } > $L/checkout-rule-commit-made.txt
partd partd-rule $RW $O/rules-rule-commit
echo "$(utc) s4 done" >> $GATELOG
echo "s4 done"
