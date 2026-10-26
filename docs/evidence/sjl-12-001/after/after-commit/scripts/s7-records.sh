#!/bin/bash
# WP7.2 records: after-commit.txt from git reads alone (as after/ordering/ordering-commit.txt records the ordering
# commit); the raw run logs copied into after-commit/logs/ under the names the ordering run's logs use; runs.tsv and
# pre-run.txt as base/logs/ holds them; the drivers copied into after-commit/scripts/. No run.
. /tmp/claude-0/-home-user/0a6f6b61-1da7-5ce6-9020-88003835d4eb/scratchpad/sjl12/wp7/scripts/lib.sh
set -u
G="git -C /home/user/Broiler.VM"
RC=889ea1568f59bf37aa21c0035b423d0d030eb87f
CC=865b69b9ede430d5a548e5b97b85e5a7b5e1aed4
OC=066abefea4351fb49bd8bf8e823fbdc7aac560de
AC=1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f
DR=76ace8692c1e51a95c3615b253bd2d31438718e3   # the ref part A's after run was dispatched from (run.txt)
O=$W/vm
A=$O/after-commit
L=$A/logs
x() { echo "\$ $*"; "$@"; echo "(exit $?)"; }
{
  echo "# bundle sjl-12-001: the after commit's identity, and the paths and blobs the after runs rest on. Written"
  echo "# $(utc) from git reads alone, after the runs, in /home/user/Broiler.VM, whose origin/main, the default branch as"
  echo "# last fetched, is $($G rev-parse origin/main). Every command as it was run, with its output and exit code."
  echo "rule-commit     $RC"
  echo "corpus-commit   $CC"
  echo "ordering-commit $OC"
  echo "after-commit    $AC"
  echo
  echo "## the after commit"
  x $G log -1 --format='commit %H%nparents %P%ncommitted %cI%nsubject %s' $AC
  x $G merge-base --is-ancestor $AC origin/main
  x $G merge-base --is-ancestor $OC $AC
  x $G merge-base --is-ancestor $CC $AC
  x $G merge-base --is-ancestor $RC $AC
  echo
  echo "## from the corpus commit to the after commit, every path outside docs/"
  x $G diff --name-only $CC $AC -- . ':(exclude)docs'
  echo
  echo "## the rule, the mode's source file and the comparison tools: the blob at each commit"
  for p in docs/evidence/sjl-12-001/decision-rule-area.md eng/compare-test262-forms.py; do x $G rev-parse $RC:$p $CC:$p $AC:$p; done
  echo "# the next three are not at the rule commit; the corpus commit adds them"
  for p in src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/ParseCorpus.cs eng/compare-parse-corpus.py eng/check-frontend-mapping.py; do
    x $G rev-parse $CC:$p $AC:$p
  done
  echo
  echo "## part A's configuration: the workflow and its driver at the after run's dispatched ref ($DR) and at the rule commit"
  for p in .github/workflows/test262-evidence.yml eng/test262-evidence.py; do x $G rev-parse $RC:$p $DR:$p $AC:$p; done
  echo
  echo "## the manifest at the after commit"
  echo "\$ git show $AC:src/tests/corpus/parse-1/corpus.manifest | sha256sum"
  $G show $AC:src/tests/corpus/parse-1/corpus.manifest | sha256sum; echo "(exit $?)"
  echo
  echo "## the corpus commit's third condition, read at the after commit: every commit after the rule commit, up to the"
  echo "## after commit, that changes a path under src/Broiler.VM.Profile.JavaScript*/ outside src/Broiler.VM.Profile.JavaScript/docs/,"
  echo "## or under src/compositions/Broiler.VM.Composition.JavaScript.Conformance/ (git log's TREESAME reading, README section 3),"
  echo "## each with whether the corpus commit is its ancestor"
  PS=(':(glob)src/Broiler.VM.Profile.JavaScript*/**' ':(exclude,glob)src/Broiler.VM.Profile.JavaScript/docs/**' 'src/compositions/Broiler.VM.Composition.JavaScript.Conformance')
  echo "\$ git log --format=%H $RC..$AC -- ${PS[*]}"
  for c in $($G log --format=%H $RC..$AC -- "${PS[@]}"); do
    $G merge-base --is-ancestor $CC $c; echo "$c  corpus commit an ancestor: exit $?  $($G log -1 --format=%s $c)"
  done
  echo "(end of list)"
} > $A/after-commit.txt 2>&1

# ---- the logs, under the ordering run's names where it has one
cp $R/build-after.out $L/build.log
[ -s $R/build-after.err ] && cp $R/build-after.err $L/build.stderr.txt
for n in census-inputs-after census-test-after parse-corpus-after-run1 parse-corpus-after-run2 parse-corpus-after-run3 checks-after; do
  if [ $n = checks-after ]; then :; else cp $R/$n.out $L/$n.stdout.txt; fi
  cp $R/$n.err $L/$n.stderr.txt
  awk -F'\t' -v k=$n '$1 == k { print "exit " substr($2, 4) " " $3 }' $R/status.tsv > $L/$n.exit.txt
done
cp $R/journal-check.txt $L/journal-check.txt
cp $R/gate.log $L/gate.log
cp $R/commands.log $L/runs-commands.log
{ printf 'run\texit\twall\tmaxrss\tstarted\tended\tloadavg-before\tloadavg-after\n'
  while IFS=$'\t' read -r name rc wall rss st en; do
    b=$(awk -F'\t' -v k=$name '$1 == k && $2 == "before" { print $4 }' $R/load.tsv | tail -1)
    a=$(awk -F'\t' -v k=$name '$1 == k && $2 == "after" { print $4 }' $R/load.tsv | tail -1)
    printf '%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\n' "$name" "${rc#rc=}" "${wall#wall=}" "${rss#maxrss=}" "${st#started=}" "${en#ended=}" "$b" "$a"
  done < $R/status.tsv; } > $L/runs.tsv
: > $L/pre-run.txt; for f in $(ls -tr $R/*.pre.txt); do cat $f >> $L/pre-run.txt; done
CL=$O/controls/logs
for n in k1-build k1-checks k1-parse-corpus k2-build k2-checks k2-parse-corpus-run1 k2-parse-corpus-run2 k2-parse-corpus-run3 \
         k-reverted-build k-reverted-checks parte-mapping parte-control-1 parte-control-2; do
  case $n in *-checks) ;; *-build) cp $R/$n.out $CL/$n.log ;; *) cp $R/$n.out $CL/$n.stdout.txt ;; esac
  cp $R/$n.err $CL/$n.stderr.txt
  awk -F'\t' -v k=$n '$1 == k { print "exit " substr($2, 4) " " $3 }' $R/status.tsv > $CL/$n.exit.txt
done
mkdir -p $A/scripts; cp -p $W/scripts/*.sh $W/scripts/*.py $A/scripts/
echo "s7 done"
