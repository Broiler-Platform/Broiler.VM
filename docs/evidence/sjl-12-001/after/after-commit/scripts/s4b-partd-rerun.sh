#!/bin/bash
# WP7.2 step 4, run again once (bundle sjl-12-001 part D). The first runs (s4-partd.sh) ran dotnet test of the
# architecture test project alone. At both commits it failed tests that read the built image of every shipping
# assembly - rules B3, B5, N10, U9 and W2 - because a test project's own build does not produce those images: a
# cause outside the change (bundle sjl-3-001's collector says so in its rules step, and builds the solution first, as
# the CI lane does). So each commit's checkout first builds the solution, Broiler.VM.slnx, in Release, and the same
# dotnet test is run again once, with BROILER_RULE_MESSAGES naming a fresh directory. The first runs are retained.
# The gate before each build and run.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
VMW=$W/vm-after
RW=$W/vm-rule
O=$W/vm
L=$O/after-commit/logs
T=src/tests/Broiler.VM.Architecture.Tests
echo "$(utc) s4b start" >> $GATELOG

partd2() {   # NAME TREE OUTDIR
  local name=$1 tree=$2 out=$3
  mkdir -p $out/rule-messages $R/$name-results
  { echo "# before the solution's build"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-$name-before-solution-build.txt
  run $name-solution-build $tree dotnet build $tree/Broiler.VM.slnx -c Release -nologo --disable-build-servers
  cp $R/$name-solution-build.out $out/solution-build.log
  tail -1 $R/status.tsv > $out/solution-build.exit.txt
  { echo "# after the solution's build"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-$name-after-solution-build.txt
  run $name $tree env BROILER_RULE_MESSAGES=$out/rule-messages dotnet test $tree/$T -c Release --nologo --disable-build-servers \
    --results-directory $R/$name-results --logger "console;verbosity=normal" --logger "trx;LogFileName=$name.trx"
  { echo "# after dotnet test"; git -C $tree rev-parse HEAD; git -C $tree status --porcelain; } > $L/checkout-$name-after.txt
  cp $R/$name.out $out/test.log
  cp $R/$name.err $out/test.stderr.txt
  tail -1 $R/status.tsv > $out/test.exit.txt
  for f in $R/$name-results/*.trx; do [ -f "$f" ] && gzip -n -9 -c "$f" > $out/$(basename "$f").gz; done
  grep -E "^\s*(Passed|Failed|Skipped)\s" $R/$name.out | grep -E "DiagnosticRegistryRuleTests|CompilationStackRuleTests" > $out/rule-tests-n5-n9-n12-n19.txt
  grep -E "^\s*Failed\s" $R/$name.out > $out/failed-tests.txt
  grep -E "Total tests|^\s+Passed:|^\s+Failed:|^\s+Skipped:|Test Run" $R/$name.out > $out/test-summary.txt
}

partd2 partd-after-2 $VMW $O/rules-after/run-2
partd2 partd-rule-2 $RW $O/rules-rule-commit/run-2
echo "$(utc) s4b done" >> $GATELOG
echo "s4b done"
