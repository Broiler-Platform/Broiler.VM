<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-002 - the shared syntax library and the wide path, step 2: Broiler.VM's rule and evidence

**Milestone:** SJL-13 of the shared JavaScript libraries programme,
[`sjl-13-shared-syntax-and-vm.md`](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md), at
the second of SJL-13.3's two cut-over steps, in which the lowering, the numeric-manifest admission and the binder read
the shared tree directly and the adapter and `JsSyntax` go. Bundle [`sjl-13-001`](../sjl-13-001/README.md) judges the
first step and holds the correspondence both rest on.

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-13.1 with bundle `sjl-13-001`'s rule,
before any change either judges existed.

**Records:** [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
the area verdict; its decision D19 reads step 2.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules, with the readings taken before any run; nothing taken. The rules are of 2026-10-26, at SJL-13.1,
and the readings of 2026-11-13, at SJL-13.3's step 2 (section 1). **Nothing here is a verdict, and no run of the rule
has been taken.** A collector joins this bundle when there is a run to take. *(Revised 2026-11-13, when section 1's
readings taken before any run were filed. It read "rules only, 2026-10-26, at SJL-13.1. **Rules only. Nothing here is
a verdict, and nothing has been taken.**")*

*(Added 2026-11-14, at step 2's records, before any run of the rule.)* **Step 2 is built, and its runs are not
taken.** Its branch, `sjl-13-3-step2`, is cut from the default branch's `389b55c9ec9bb77409bb2487067fbebc0868849b`
(Broiler-Platform/Broiler.VM#205, the merge that moved this family to the shared stream's `0.1.0-preview.11` and
carried section 1's readings and the rule's revision of 2026-11-13), and lands as one merge, which is the after commit;
the merge's first parent is the base. `389b55c9ec9bb77409bb2487067fbebc0868849b` descends from the rule commit and from
bundle `sjl-13-001`'s after commit `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`, and the branch's first commit,
`db7a6f0eaf516d6113c2d11c986fe3232be9f6df`, is directly on it. The base is named here by all forty digits as the
branch holds it; the after commit is named when the merge lands. *(Revised 2026-11-14, later, at the branch's review:
it read "The base and the after commit are named here, by all forty digits, when the merge lands.")* This holds only
while the default branch is still `389b55c9` when the branch merges: if anything lands on it first, the merge's
first parent is not `389b55c9`, and the branch is built again on the new tip, with these records, before it merges;
reading 1 holds the default branch meanwhile. The branch's commits
from `db7a6f0eaf516d6113c2d11c986fe3232be9f6df` to `c452294b3ca1fefa9aa5e3db41ab5e3264903d25` move the admission walk,
the numeric admission, the binder and the lowering onto the shared tree, delete `JsSyntax.cs` and `JsFrontendAdapter.cs`
with its parts, and register what they deleted in rule I5; step 2's records follow them and change no product source.
[JSD-0087](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0087-the-wide-path-parses-with-the-shared-syntax-library.md)'s
dated note of 2026-11-14, the cut-over record's note at step 2, lists them. The development checks the branch was built
against, at the base and at its commits, are not evidence: they are kept outside this repository, are no run of the
rule and decide nothing (reading 6), and no figure of them is quoted here. Of them this README says only that the
development checks' test262 comparison found only Exhausted rows whose allowance moves between the wall clock and fuel,
as the base build's own re-runs do. Every part is taken after the merge, at the base and the after commit: part A by
the hosted workflow, dispatched for both, and parts B to G locally. Nothing here is a verdict yet.

*(Added 2026-11-15, after the merge, at task 7.1 of step 2's evidence, before any part is judged.)* **Step 2 merged**
as Broiler-Platform/Broiler.VM#206, `467ad7c395234b335c816b8f49ae5b505740e32c`, the after commit, a true merge; its
first parent is the base named above, `389b55c9ec9bb77409bb2487067fbebc0868849b`, and the after commit is the only
first-parent commit after the base. Part A's two runs were dispatched from the default branch at the after commit, with
`forms both`: the base run, workflow run 43651721567, tested `389b55c9`, and the after run, workflow run 43652416341,
tested `467ad7c3`. Each run's artifact `test262-evidence` is imported unchanged, with the digest of its zip as
downloaded, into [`base/hosted/`](base/hosted/imported.txt) and [`after/hosted/`](after/hosted/imported.txt); each
`imported.txt` names the run, the tested commit and the commit input, the workflow's and the driver's blobs at the
dispatched ref, which are the base commit's, and the images. Nothing here reads or judges them yet.

*(Added 2026-11-15, at task 7.2 of step 2's evidence, after the merge and before any part is judged.)* **Parts B to G
are taken locally, with part A's comparisons and one-process re-runs, and are retained here without being read.**
Every run was taken on 2026-11-15 on this family's local machine (four logical processors, .NET SDK 10.0.404, Ubuntu
24.04.5 LTS; section 1's reading 4 carries bundle `sjl-13-001`'s reading 10, the machine as replaced, each run
recording its SDK), from a clean `git archive`
of the commit it names, `docs/` included, extracted to a tmpfs, built in Release with `--disable-build-servers`: the
base `389b55c9ec9bb77409bb2487067fbebc0868849b` and the after commit `467ad7c395234b335c816b8f49ae5b505740e32c`. Every
run, build and comparison waited behind a gate that waits, with no fallback, for a one-minute load of at most 1.0 and no
dotnet, testhost, VBCSCompiler, MSBuild, ilc, crossgen or Broiler.* process, under an exclusive lock so that one run is
taken at a time, and no compiler server was alive at any run's start (section 1's reading 4: bundle `sjl-13-001`'s reading 9 does not
carry). The drivers
are kept outside this repository. Every folder that holds a run keeps a `logs/` with each run's command line, exit, wall
time, peak memory, the machine record before and after it with the whole process list at its start, and the gate's
record; `after/parta-race/` holds no run and no `logs/`, as its note says, and `measurement/logs/` joined that
sub-bundle after its collector had finished (part F below). Each run's `logs/<run>.machine.txt` is the part's machine
record, one per run in place of the single `machine.txt` a part of bundle `sjl-13-001` keeps (its
`after/parti/machine.txt`, for one): the precedent's form, with the whole process list at the run's start added on
purpose, so that what else ran then, a python or dotnet process among it, can be read from it. Each folder's
`hashes.txt` holds the SHA-256 of every other file in it, its `logs/` included. *(Revised 2026-11-15, later, at the
review of these records: it read "Every folder's `logs/` holds each run's command line, exit, wall time, peak memory,
the machine record before and after it with the whole process list at its start, and the gate's record; each folder's
`hashes.txt` holds the SHA-256 of every other file in it.", which `measurement/` and `after/parta-race/` did not bear
out.)* Where each part's output is:
- **Part A:** the comparisons of the imported reports, through `eng/compare-test262-forms.py` as the base commit holds
  it (`after/parta-comparisons/`; first taken without the gate, and taken again behind it with the same output, which
  is what that folder retains); each (c′) row's three one-process re-runs at each commit, with `--expect` against
  the commit's retained pin (`after/parta-rerun/`), whose row lists (`rows-cprime-wide-*.tsv`) were read from the
  first comparison, before the gated one was taken, the two comparisons' logs being the same bytes; and the (e) race,
  which runs only the rows whose re-runs differ (`after/parta-race/`, with its note).
- **Part B:** run 1 of `--parse-corpus` (B1 to B5, one worker) at each commit (`base/local/`, `after/local/`); the
  census over the parse corpus's inputs and over the suite's `test/` at both (`census-*.txt.gz`); the identity of the
  two runs with both census pairs (`after/local/partb-keys/`); the control in `controls/partb/`.
- **Part C:** runs 2 and 3 at the after commit (`after/local/`) and their determinism comparison
  (`after/local/determinism/determinism-after-runs-1-2-3.tsv`, the file bundle `sjl-13-001` keeps in
  `after/judgement/partc/`); `--checks --verbose` at both commits (`local/checks.log.gz`); the controls K1 to K4,
  re-made against step 2's code on copies of the after commit, their patches, outputs and revert checks, in
  `controls/`. K4 as configured: `controls/k4-checks.log.gz` holds ten `ok` lines, the last "the lowering is
  deterministic", and stops; the process ended with exit 134 (`controls/logs/k4-checks.exit.txt`) and a stack overflow
  in this component's `JsAdmissionWalk.Expression`, under `JsFrontend.Parse` (`controls/k4-checks.stderr.txt`), and
  `controls/k4-checks-named-rows.txt` is empty. The output does not name the check that overflowed; in the after
  commit's own `--checks` the next check is "the wide lowering is deterministic", the first of part C's five rows, so
  by the order of the checks the overflow came in that row, before the fifth row ran. That is the input to section 1's
  reading 2, under which the adjunct `controls/k4-row-*` was run; whether it meets K4's "must" is the judgement's.
- **Part D:** the architecture tests at both commits after a Release build of the solution, from a clean archive of
  each, with `BROILER_RULE_MESSAGES` set to a scratch folder whose files are copied unchanged into
  `<side>/rules/rule-messages/` (`base/rules/`, `after/rules/`); rule I5's two controls, pinned by the revision of
  2026-11-13, taken in the after commit's tree of that run (`controls/i5/`).
- **Part E:** the bisect at the base, `base/fuel-parity.log`, and the after run at the base's figures,
  `after/fuel-parity.log`, each run's record and attempts in `base/parte/` and `after/parte/`, where bundle
  `sjl-13-001` keeps its part I's, the same runs, in `after/parti/` (its `controls/parte/` holds its own part E's
  controls, a different part).
- **Part F:** the sub-bundle [`measurement/`](measurement/README.md), whose README was written and made read-only
  before the collector ran, collected last by bundle `sjl-13-001`'s
  [`collect-measurement.py`](../sjl-13-001/collect-measurement.py), unchanged; the collector wrote its own `hashes.txt`,
  and the three records copied in after it (`measurement-machine.txt`, `measurement-collector.log` and
  `measurement-collector.stderr.txt`) are appended to it, its lines unchanged. The README was written first, and part
  F's driver made it read-only (`chmod a-w`) at its own start, just before the gate and the collector: the listing it
  took then, `measurement/logs/before-ls.txt`, holds the README alone, read-only, and `logs/after-ls.txt`, taken after
  the three records were copied in, shows the manifest and the arms' binary lists read-only, as the collector makes
  them before its first lane. git keeps no read-only mode, so those two listings are the record of it. Part F's own
  run record, which its driver wrote outside this repository, was copied in unchanged into `measurement/logs/` at the
  review of these records (its command line, the gate's record, exit, wall time and peak memory, the machine records,
  the two listings, and the collector's `hashes.txt` as it wrote it, `logs/hashes-as-collected.txt`), and its lines are
  appended to `measurement/hashes.txt` in the same way; the sub-bundle's README, written before the run, does not list
  them. Part F started one second after the last control
  run ended (`controls/logs/runs.tsv`, `measurement/logs/runs.tsv`); its start record in
  `measurement/measurement-machine.txt` gives the one-, five- and fifteen-minute loads at that start.
- **Part G:** `--fuzz-wide` and the four `--fuzz` sessions at both commits (`local/fuzz-*`); the planted throws, the
  wide one in the binder's walk over the shared tree (`controls/partg-*`).

[`controls/note.txt`](controls/note.txt) says how each control was made and reverted. Nothing here is a verdict: the
next task reads these outputs under the rule.

*(Added 2026-11-15, at task 7.3 of step 2's evidence.)* **The parts are judged.** Section 4 judges each part of the
rule, on the rule as its revision of 2026-11-13 reads it, from the files named with it: parts A, B, C, D, E and G are
**MET**, part C on the reading of control K4 that section 1's reading 4 carries from bundle `sjl-13-001`, part D on
what shows rule I6 and part A on the hosted runs' SDK (section 6's readings 1, 2 and 3); part F, the measurement, is
read and given no verdict, since it is no gate clause (decision D24, section 1's reading 5). Section 3 says what task
7.3 took itself, section 5 reads clause 8's publication, section 6 lists every reading taken with the alternative not
taken, section 7 the findings and section 8 what comes next. The "State" line above reads "rules, with the readings
taken before any run; nothing taken" and "**Nothing here is a verdict, and no run of the rule has been taken.**", and
the paragraphs of tasks 7.1 and 7.2 end "Nothing here reads or judges them yet." and "Nothing here is a verdict: the
next task reads these outputs under the rule."; each reads as of the day it was written, and is left as written.

## 1. SJL-13's area rule for step 2 (SJL-13.1)

**Step 2 changes no outcome, no byte and no diagnostic** (JSD-0083 decision D19). The rule therefore admits no class
but part A's harness classes.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged. *(Note of 2026-11-13, late, at SJL-13.3's step 2: the
  sentence "It is named in this section, by all forty digits, when the change that carries it has merged." is
  fulfilled here. That change, the SJL-13.1 records, merged in Broiler-Platform/Broiler.VM#172 as
  `faa8af78cddc32d460a7299f088b18b9e4407d7a`, the rule commit. The commit of that change's branch that added the file
  is `ef3f0567a5eba0dca7fa4fc584bd53e94ce81281`, and `af2420b292c3cbcd5476943ce92c8798f6aeb847`, on the same branch
  and before the merge, revised it (part B's sub-population B5). The file's blob is
  `a6b2a086c5775b3e4ca7450da530bb9510bc5095` at the rule commit, at bundle `sjl-13-001`'s after commit
  `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`, and at `686533fad10a17600ce67e5a774892f62cb08006`, the default branch's
  head from which step 2's branch was cut. This section did not name the rule commit when #172 merged, as it should
  have; the note names it on the branch whose merge is to be the after commit, and section 1's readings taken before
  any run record that choice.)*
- **The base commit** and **the after commit** do not exist yet. The after commit is the default-branch merge that
  lands step 2, and the base commit is its first parent, which descends from the commit bundle `sjl-13-001` names as
  its after commit. This README names each, by all forty digits, when it lands.

*(Noted 2026-11-15, at task 7.3 of step 2's evidence.)* The bullet above reads "The base commit and the after commit
do not exist yet. [...] This README names each, by all forty digits, when it lands." Step 2 landed in one merge,
Broiler-Platform/Broiler.VM#206: the after commit is `467ad7c395234b335c816b8f49ae5b505740e32c`, and the base, its
first parent, is `389b55c9ec9bb77409bb2487067fbebc0868849b`, which the dated paragraphs above the section already name.
Section 4's table gives each condition the rule's "The commits" sets on them, with the git read that shows it. The
bullet is left as written.

**The rule has seven parts**, each deciding alone: part A, test262 in both forms on hosted runners (template T1), with
class C4 only; part B, artifact identity over the parse corpus's sub-populations B1 to B5, B5's ordered module
specifiers among them, with no class (template T6); part C, determinism and the five checks rows of bundle
`sjl-13-001`'s part C (layer L13); part D, the front end's rules, with rule I5 registering every member of `JsSyntax`
and of the adapter; part E, the fuel-parity twins (template T7); part F, this family's compile measurement over bundle
`sjl-13-001`'s workloads (template T5), not a gate clause; part G, source fuzzing over bundle `sjl-13-001`'s seeds,
both sessions (layer L9).

**The pins** are bundle `sjl-13-001`'s.

**Part B's comparison reads B5 since 2026-11-01.** `eng/compare-parse-corpus.py`, which part B runs as the base commit
holds it, refused every report holding sub-population B5 until the change of 2026-11-01 before step 1's base (bundle
`sjl-13-001`'s README, section 1, names it). Since that change it compares an accepted B5 row on its ordered
specifiers, its note, with its hash and length, and refuses a report whose B5 note does not have the hash and length
its row states. Its `control` mode makes this part's control as filed with `--column --specifier with-hash`: one
artifact hash changed, one diagnostic's column moved, in place of the outcome flip bundle `sjl-12-001`'s control makes,
and one specifier of a B5 input changed with its hash and length, which the comparison reports as three differences,
the third a `specifiers` one. With `--specifier note-only` the specifier changes alone and the report is refused.
*(Added 2026-11-01.)*

### Readings taken before any run (2026-11-13)

*(Added 2026-11-13, at SJL-13.3's step 2.)* These readings are filed on the branch whose merge is to be the after
commit, before any run of step 2 that the rule judges and before the development reference that reading 6 names was
taken. Each is recorded as the owner's dated decision under the owner's standing direction of 2026-10-18 ("Please do
all recommended decisions and skip waiting for review"), taken as the recommended choice, with the alternative not
taken; none is the owner's own dated answer, asked and answered, and the owner may rule otherwise. They change no part,
class, control or condition of the rule. Part D's clause on rule I5, and its control, are revised by
[`decision-rule-area-revised-2026-11-13.md`](decision-rule-area-revised-2026-11-13.md), filed on the same branch before
any run, on the reading of decision D20 that JSD-0083's dated note of 2026-11-13 records.

1. **Every run of this bundle is taken after step 2's merge.** The base's runs and the after commit's, part A's
   dispatches among them, are taken once step 2 has merged. Nothing else merges into Broiler.VM's default branch until
   this bundle's evidence merges, so that no other change lands between the base and the after commit, or after the
   after commit while this bundle's runs are taken, and part A's dispatched ref holds the base's workflow and driver. If step 2 has to land in a second
   merge, both merges are named in this bundle, the base stays the first merge's first parent, and every after run is
   taken again at the last, as the rule's "The commits" says. Not taken: "take the base column now", at
   `686533fad10a17600ce67e5a774892f62cb08006`, before step 2's merge.
2. **The rule commit is named on this branch**, by the note above: `faa8af78cddc32d460a7299f088b18b9e4407d7a`, the
   merge of #172, which carries `ef3f0567a5eba0dca7fa4fc584bd53e94ce81281`, with the rule file's blob
   `a6b2a086c5775b3e4ca7450da530bb9510bc5095`. Not taken: "a separate docs merge before the base", which would have
   named it on the default branch before step 2's branch.
3. **Part D's clause on rule I5 is revised**, by the revision named above. Its control is pinned: a scratch copy of
   the after commit declares the type `JsMemberExpression` again, with its member `NameSpan`, and must be refused; a
   second control declares `JsNumberLiteral` again. The revision records its alternatives.
4. **Which of bundle `sjl-13-001`'s readings carry.** That bundle's section 5 took readings 1 to 11, each now the
   owner's own dated answer by the rulings of 2026-11-11 and 2026-11-12. For this bundle:
   - **Reading 10 carries**: the rule's local machine is this family's machine as replaced, each run recording its
     SDK.
   - **Reading 2 carries**: part C's control K4 is applied as configured, and the adjunct, the same patch under a
     harness narrowed to the fifth row, is used only if K4 as configured fails to make the fifth row fail.
   - **Readings 1, 3, 4, 5, 6, 7, 8 and 11 have no object at step 2.** They read part A's kind (F) of class C1, part
     B's classes R, S and N, part H and the registry commit. This rule's part A admits only class C4, its part B
     admits no class, and it has no part H and no registry commit.
   - **Reading 9 does not carry.** Every local part of this bundle is taken with no compiler server alive, as the idle
     gate reads it.
   - Not taken: "rule each again", which would have asked the owner on each before step 2's runs; and "carry them
     all", which would have carried reading 9's idle servers into this bundle.
5. **Part F, a reading; there is nothing to decide.** Part F is no gate clause (decision D24). The owner's words of
   2026-11-03, "Please defer the measurement", answered decision D48 of SJL-13's plan, on SJL-14's clause 6, for
   Broiler.JS (`docs/javascript-shared-libraries/decisions.md`, decision SJL-D-23's dated note of 2026-11-03; the
   milestone `sjl-14-broiler-js-syntax.md`, its note of that day), not this rule's part F; and the owner's ruling of
   2026-11-12 on bundle `sjl-13-001`'s reading 9 let that bundle's part J, taken after 2026-11-03, stand. Part F is
   taken after step 2's merge, as the rule's B and A arms name the base's and the after commit's builds, and is read as
   no gate clause.
6. **Development checks before the merge** are taken from scratch archives, kept outside this bundle and never cited
   by it, and decide nothing. They are not runs the rule judges. The development reference taken at
   `686533fad10a17600ce67e5a774892f62cb08006` after these readings and the revision above were filed is such a check:
   it is not a judged run, nothing of it is under `docs/evidence/`, and it is not this bundle's base. Any revision of
   the rule filed after that reference states that it was written after the reference was seen. Not taken: "no checks
   before the merge".
7. **Where the facts are computed.** At step 2, the top-level-`await` fact, the BigInt values and strictness are
   computed in the compiler assembly's post-parse code, by `JsParser`'s rule: the admission walk (`JsAdmissionWalk`)
   computes the flag and the BigInt values, and a side table keyed by the shared tree's nodes holds strictness. This
   reads JSD-0083's "the lowering's at step 2" (X6, decision D15 and decision D20), and the rule's "The lowering
   computes from the shared tree, by `JsParser`'s rules, every fact the adapter computed at step 1", as that code;
   JSD-0083's dated notes of 2026-11-13 on X6, D15 and D20 record it. Not taken: "compute them in `JsCompiler`'s own
   files", as JSD-0083's text reads.

*(Noted 2026-11-13, later, when the shared stream's `0.1.0-preview.11` was taken before step 2.)* The passage above
reads "These readings are filed on the branch whose merge is to be the after commit, before any run of step 2 that the
rule judges and before the development reference that reading 6 names was taken"; reading 1 reads "Nothing else merges
into Broiler.VM's default branch until this bundle's evidence merges"; reading 6 reads "The development reference taken
at `686533fad10a17600ce67e5a774892f62cb08006` after these readings and the revision above were filed is such a check";
section 1's note on the rule commit reads "the note names it on the branch whose merge is to be the after commit"; and
the revision's "Written" line reads "It is filed on the branch whose merge is to be this rule's after commit, before
that merge", with "Step 2 is built on the branch `sjl-13-3-step2`, cut from
`686533fad10a17600ce67e5a774892f62cb08006`". The owner published the shared stream's `0.1.0-preview.11` on 2026-11-13,
and its move was taken first, in its own merge, before step 2's base, as the owner's dated decision under the standing
direction of 2026-10-18, taken as the recommended choice (not taken: "step 2 on preview.10, the stream move after step
2's evidence merges"), because step 2's code reads the shared syntax library's tree directly and the stream's new
version changes that library's sources (in comments only, so its code is `0.1.0-preview.10`'s; the ledger's entry of
2026-11-13 records the move's own local checks). *(Revised 2026-11-13, later still: the parenthetical read "(in
comments only, so its code is `0.1.0-preview.10`'s, and the parse corpus's report over B1 to B5 and both census
outputs are byte for byte the same before and after the move, in local runs from clean archives that are no part of
this rule)". Those words are withdrawn: reading 6 keeps development checks outside this bundle and never cited by it,
and this bundle cites none of the move's runs.)*
- **Where these readings land.** The two commits that filed this section's readings and the revision,
  `233b6629c5bac0ca921dd82c9ccbc8aa1ac01d11` and `90c4d07cfb38d916bcc804c17b2f5726a01c1933`, land in that change's
  merge, with the move, before step 2's base and before any run of this rule. They are not on the branch whose merge is
  the after commit, and the sentences quoted above that say so read as of the day they were written.
- **The base.** Step 2's base is the first parent of step 2's own merge, as the rule's "The commits" says, and that
  merge descends from the move's. The base therefore holds the move, the readings and the revision; the after commit is
  still the only first-parent commit that carries any part of step 2. The move is no part of step 2, and changes none of
  this rule's pins, which are bundle `sjl-13-001`'s: test262, ECMA-262, the parse corpus's manifest, the early-error
  catalogue and the registry.
- **Reading 1** reads from step 2's branch point: the move's merge, which carries the readings themselves, lands before
  it, and the reading's purpose, that no other change lands between the base and the after commit, or after the after
  commit while this bundle's runs are taken, is unchanged.
- **Reading 6's reference.** No development reference was taken at `686533fad10a17600ce67e5a774892f62cb08006`: its
  builds were stopped when the owner's publication arrived, and nothing of them is seen or cited. Step 2's stage 1 takes
  its development reference at step 2's branch point instead, the default branch's head once the move has merged, under
  reading 6 as written otherwise.
- **The revision file is not edited.** Rule files, revisions included, are never edited once filed, so its "Written"
  line keeps its words, and this note is its dated record.

No part, class, control or condition of the rule changes, and no part is voided or judged otherwise by the move landing
before the base. The readings above are left as written.

*(Noted 2026-11-13, later still, on review of the note above, which is read with these corrections and additions.)*
- **Whose choice it is.** The order above, and the reading of reading 1 below, are each taken under the owner's
  standing direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review") as the
  recommended choice, and recorded as the owner's dated decision under that direction, with the alternative not taken;
  neither is the owner's own dated answer, asked and answered, and the owner may rule otherwise, as for the readings
  above.
- **Reading 1.** It reads "Nothing else merges into Broiler.VM's default branch until this bundle's evidence merges".
  It is read from step 2's branch point, the default branch's head once the move has merged: the move's merge, which
  carries the readings themselves, lands before that point and is no part of step 2, and the reading's purpose, that
  no other change lands between the base and the after commit, or after the after commit while this bundle's runs are
  taken, is unchanged. Not taken: "reading 1 as written when `233b6629c5bac0ca921dd82c9ccbc8aa1ac01d11` filed it",
  which would forbid the move's merge and hold the move until this bundle's evidence merges.
- **Reading 2.** It reads "The rule commit is named on this branch" and "Not taken: "a separate docs merge before the
  base", which would have named it on the default branch before step 2's branch". Section 1's note on the rule commit
  also reads "at `686533fad10a17600ce67e5a774892f62cb08006`, the default branch's head from which step 2's branch was
  cut". Under the order above, the note that names the rule commit lands on the default branch in the move's merge,
  before step 2's branch is cut, which is in substance the alternative reading 2 did not take. That follows from the
  order decision, not from a new reading of reading 2. The rule commit is as the note names it, and the rule file's
  blob, `a6b2a086c5775b3e4ca7450da530bb9510bc5095`, is the same at this change's head, which does not touch the file.
  `686533fad10a17600ce67e5a774892f62cb08006` is the head from which the branch carrying the readings was cut; step 2's
  branch is cut from the default branch's head once the move has merged. The note above's "read as of the day they
  were written" covers these sentences too.
- **No run of step 2 was taken**, and no development reference: nothing of either is seen or cited here.
- **What the revision read at `686533fad10a17600ce67e5a774892f62cb08006`.** The move changes no source file of this
  repository: no `.cs` or `.csproj` file changes between that commit and this change's head. The files the revision's
  "What was seen, and where" reads, `I5RetiredDuplicatesRuleTests`, `JsSyntax.cs` and `JsFrontendAdapter*.cs`, are
  the same at this change's head, so what it read holds at step 2's branch point once this change has merged with
  nothing between.

The note above and the readings are left as written, but for the parenthetical revised in place.

## 2. What this bundle will hold

- **`base/`** and **`after/`**: part A's dispatches, imported unchanged with the digest of each zip; parts B to E and
  G at each commit; part A's comparisons and C4 re-runs.
- **`controls/`**: part B's planted differences, part C's K1 to K4, part D's I5 control and part G's planted throws,
  each with its output.
- **`measurement/`**: part F's sub-bundle, with its own README and manifest.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted.

*(Noted 2026-11-15, at task 7.3 of step 2's evidence.)* The list above names no folder for what task 7.3 reads and
takes itself, nor for clause 8's publication. They are [`after/judgement/`](after/judgement/) (the git reads of the
commits' conditions, part A's completeness and table of changed verdicts, part C's probe of control K4 and part D's
reads, each folder with a `hashes.txt`) and [`after/publication/`](after/publication/README.md) (every JavaScript root
published and run in three modes at the after commit). `manifest.json` lists every file of the bundle with its SHA-256.
The list is left as written.

## 3. What task 7.3 took itself (2026-11-15)

**Written 2026-11-15, at task 7.3 of step 2's evidence.** Everything task 7.3 read or wrote is in
[`after/judgement/`](after/judgement/) and [`after/publication/`](after/publication/README.md), each folder with a
`hashes.txt`. Its local runs were taken on 2026-11-15 on this family's local machine (four logical processors, .NET
SDK 10.0.404, Ubuntu 24.04.5 LTS; section 1's reading 4 carries bundle `sjl-13-001`'s reading 10), each behind the
gate and under the exclusive lock tasks 7.1 and 7.2 used, which wait, with no fallback, for a one-minute load of at
most 1.0 and no dotnet, testhost, VBCSCompiler, MSBuild, ilc, crossgen or `Broiler.*` process; each run's folder keeps
its command line, exit, wall time, peak memory, the machine record before and after it with the whole process list at
its start, and the gate's record. The drivers are kept outside this repository; a driver retained here carries a
`.txt` suffix.

- **The commits' conditions** ([`after/judgement/commits/`](after/judgement/commits/)): git reads alone, by
  `commit-conditions.sh.txt`, whose output is `commit-conditions.txt`; section 4's table reads them.
- **Part A's completeness and its changed verdicts** ([`after/judgement/parta/`](after/judgement/parta/)):
  `part-a-tables.py`, which reads the imported artifacts, the retained comparisons and re-runs and runs nothing,
  writes `completeness.txt` and `changed-verdicts.tsv`.
- **Control K4's overflow** ([`after/judgement/partc/k4-probe/`](after/judgement/partc/k4-probe/note.txt)): the K4
  library made again as task 7.2 made it (`controls/k4.patch` on a `git archive` of Broiler.JS `29061d65`, packed as
  `0.1.0-preview.11` into the scratch feed `controls/k4-nuget-config.patch` names), a copy of the after commit
  restoring it, and a copy of the after commit with the published library; each copy's slice compiler, built in
  Release with `--disable-build-servers`, ran `--parse-corpus` over B1 and B2 with `--select`, one process per input,
  over every compilation the after commit's run 1 refuses with code 2103, the contract's nesting refusal, with a
  journal.
- **Part D's reads** ([`after/judgement/partd/`](after/judgement/partd/)): `partd-reads.sh.txt`, git reads of the
  family baseline (rule I6), of the files the after commit adds to and deletes from the compiler assembly (rule N12),
  and of rule I5's register against the units both commits' generated `assurance.manifest.json` list, by
  `i5-units.py`, with a coarse line read of the declarations of the compiler's files that stay (`fields.py`); the
  output is `partd-reads.txt`. It runs no build and no test.
- **Clause 8's publication** ([`after/publication/`](after/publication/README.md)): bundle `sjl-7-001`'s collector,
  `--only cutover-closures`, at the after commit, as step 1's was taken (section 5).

Nothing else was run again. Tasks 7.1 and 7.2's retained outputs are read as they are, and none is edited.

## 4. The judgement of step 2 (2026-11-15, task 7.3)

**Written 2026-11-15, at task 7.3 of step 2's evidence** (JSD-0087 places it in SJL-13.3's work package 7). Each part
decides alone, on the rule as its revision of 2026-11-13 reads it, from the files named with it, and no part reads
another's verdict. Where a part turns on words the rule does not settle, the part says which reading it took, and
section 6 lists every such reading with the alternative not taken, for the owner to rule on. No test262 suite total is
quoted: part A is decided per variant. The rule admits no class but part A's C4 (decision D19).

### The commits, and the conditions the rule's header sets

[`after/judgement/commits/commit-conditions.txt`](after/judgement/commits/commit-conditions.txt) is the output of
[`commit-conditions.sh.txt`](after/judgement/commits/commit-conditions.sh.txt), git reads alone, over this repository,
taken at task 7.3 against this branch's head before task 7.3's commits. Every condition holds on these reads, so no
part is void. *(Added 2026-11-15, later, at the review of task 7.3.)* Where the table says "this branch's head", it is
that head, `a2387f186e2c1a989448cbffdaa56c012039b12a`, as the read's first line says. Neither task 7.3's commits nor
the review's fixes after them change any `decision-rule-area*.md` of this bundle, so the rule's blob and the
revision's are the same at this branch's final head.

| Commit | Condition (the rule's "The commits") | The git read, and what it shows |
|---|---|---|
| Rule commit `faa8af78cddc32d460a7299f088b18b9e4407d7a` (Broiler.VM#172) | carries `decision-rule-area.md` | `git log --diff-filter=A` names `ef3f0567a5eba0dca7fa4fc584bd53e94ce81281`, an ancestor of `faa8af7`, the merge of #172; the rule's blob is `a6b2a086c5775b3e4ca7450da530bb9510bc5095` at the rule commit, the base, the after commit and this branch's head |
| | the revision of 2026-11-13 | filed by `90c4d07cfb38d916bcc804c17b2f5726a01c1933`, before the base; its blob is `1ecc4c712e26768344b9214f8f68b21e38528973` at the base, the after commit and this branch's head; no `decision-rule-area*.md` of this bundle changes between the after commit and this branch's head |
| Base `389b55c9ec9bb77409bb2487067fbebc0868849b` (Broiler.VM#205) | the after commit's first parent | `git rev-parse 467ad7c^1` is `389b55c` |
| | descends from the rule commit | `git merge-base --is-ancestor faa8af7 389b55c`: yes |
| | descends from the commit bundle `sjl-13-001` names as its after commit, `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4` | `git merge-base --is-ancestor a8d8398 389b55c`: yes |
| | the after commit is the only first-parent commit that carries any part of step 2 | `git rev-list --first-parent 389b55c..467ad7c` is `467ad7c` alone, and it is the one merge on that line; at the base `JsSyntax.cs` and `JsFrontendAdapter.cs` are present and `JsTreeFacts.cs` is not; the branch the after commit merges begins with `db7a6f0eaf516d6113c2d11c986fe3232be9f6df`, whose parent is the base |
| After commit `467ad7c395234b335c816b8f49ae5b505740e32c` (Broiler.VM#206) | the default-branch merge that lands step 2, one merge | parents `389b55c` and `bc12ab2c37c425b579db20b86ed7648cccaea482`, a true merge; `JsSyntax.cs` and `JsFrontendAdapter.cs` with its three parts are deleted and `JsTreeFacts.cs` added; there was one merge, so the rule's provision for more than one does not apply |
| Part A's runs | dispatched at the base and the after commit, the workflow and the driver at the dispatched ref the base's | each `imported.txt` names its tested commit and the dispatched ref, the default branch at the after commit; the workflow's blob `740a70f8d919ced6cd4bdf31f8bb2408f12fdcf7` and the driver's `55f71ee3da6844dd3e3aeb23ea98d24143222a88` are the same at the base, the after commit and this branch's head ([`completeness.txt`](after/judgement/parta/completeness.txt)) |
| Parts B to G | taken locally from a clean checkout of each commit they name | task 7.2's paragraph above: clean `git archive`s of the base and the after commit, after the merge; and section 3 for task 7.3's own runs |
| The default branch | nothing else merges into it until this bundle's evidence merges (section 1's reading 1) | `origin/main` reads `467ad7c` at every read of task 7.3, before each push |

### The verdicts

| Part | Verdict | What it reads |
|---|---|---|
| A, test262 in both forms | **MET**, on reading 3 of section 6, the hosted runs' SDK and images | [`after/judgement/parta/`](after/judgement/parta/): `completeness.txt`, `changed-verdicts.tsv`; [`after/parta-comparisons/`](after/parta-comparisons/), [`after/parta-rerun/`](after/parta-rerun/), [`after/parta-race/`](after/parta-race/note.txt) |
| B, artifact identity | **MET** | [`after/local/partb-keys/`](after/local/partb-keys/); part B's run 1 and census at each commit (`base/local/`, `after/local/`); [`controls/partb/`](controls/partb/) |
| C, determinism | **MET**, on reading 1 of section 6, the reading of control K4 section 1's reading 4 carries | [`after/local/determinism/`](after/local/determinism/); `base/local/checks.log.gz`, `after/local/checks.log.gz`; [`controls/`](controls/note.txt) K1 to K4; [`after/judgement/partc/k4-probe/`](after/judgement/partc/k4-probe/note.txt) |
| D, the front end's rules | **MET**, on reading 2 of section 6, what shows rule I6 | [`after/rules/`](after/rules/), [`controls/i5/`](controls/i5/), [`after/judgement/partd/`](after/judgement/partd/); the base, read: [`base/rules/`](base/rules/) |
| E, the fuel-parity twins | **MET** | [`base/fuel-parity.log`](base/fuel-parity.log), [`after/fuel-parity.log`](after/fuel-parity.log), `base/parte/`, `after/parte/` |
| F, the compile measurement (no gate clause, decision D24) | no verdict (section 1's reading 5; section 6's reading 4) | [`measurement/summary.txt`](measurement/summary.txt), `measurement/conditions.tsv`, `measurement/runs.tsv` |
| G, source fuzzing | **MET** | `after/local/fuzz-*.log`, `controls/partg-control-wide.log`, `controls/partg-control-slice-*.log`; the base, read: `base/local/fuzz-*.log` |

### Part A: MET, on reading 3 of section 6, the hosted runs' SDK and images

"MET if and only if all four runs are complete, and every changed verdict of either pair is in C4 and is listed."

- **All four runs are complete** ([`completeness.txt`](after/judgement/parta/completeness.txt)), as bundle
  `sjl-13-001`'s part A states completeness: in each form at each commit, sixteen shard logs of sixteen, and the merge
  took sixteen shard reports; the run log's exit code is 1 in the bytecode form and 4 in the native form, both of
  which completeness admits; the merged report's retention object says retainable, its coverage is whole and it
  accounts for every variant; the binaries log has one distinct list of assemblies. Each comparison ignored no variant
  present in one report only.
- **The configuration.** The base run tested `389b55c` and the after run `467ad7c`, each dispatched from the default
  branch at the after commit with `forms both`, where the workflow and the driver are the base commit's blobs. Every
  runner and the merging job of both runs report .NET SDK 11.0.100; the images differ between runners of one run, as
  `imported.txt` records. The rule names neither, and section 6's reading 3 takes the runs as configured.
- **The comparisons** (`after/parta-comparisons/`), through `eng/compare-test262-forms.py` as the base commit holds it
  (the same blob at the after commit), list 11 changed variants in the bytecode pair and 13 in the native pair, with
  `--exempt-guest-loads` over the pin extracted whole. Each exits 0, every row in the script's class (c′), a
  wall-clock exhaustion on one side, and none unclassified;
  [`changed-verdicts.tsv`](after/judgement/parta/changed-verdicts.tsv) lists every row with its class and its re-runs.
- **C4 (c′): every one of the 24 rows** is a test the earlier bundles saw race the wall clock: `decodeURI` and
  `decodeURIComponent`'s `A2.5_T1`, `staging/sm/Date/dst-offset-caching-2-of-8`, `-4-of-8` and `-8-of-8`, the counting
  sort, and the two `RegExp-*-escape-BMP` tests. Each row's three one-process re-runs at each commit
  (`after/parta-rerun/`), with `--expect` against each commit's retained pin, every log naming the pin, give one
  verdict and one dimension in all six: Exhausted on fuel, or Exhausted on `NestedLoadFanOut` for the two `RegExp`
  rows, never on the wall clock (`agreement-wide-*.tsv`). The re-runs agree, which (c′) admits (section 6's reading
  5). Each re-run exits 1, the conformance root's answer when the selection executes no test to completion, and its
  report gives the verdict.
- **C4 (e)** has no row: no row's re-runs differ (`differing-wide-*.tsv` are empty), and the race runs only rows whose
  re-runs differ (`after/parta-race/note.txt`; section 6's reading 6).
- **No C1, C2, C3 or C5 row**, as the rule expects: step 2 changes no outcome. No variant is a regression.

### Part B: MET

"MET if and only if both runs are complete, the control is reported as stated, and no difference of any kind is
reported."

- **Both runs are complete.** Run 1 at each commit reports every compilation the manifest owes, under the manifest's
  digest, which is the same file at both commits (blob `2419aa2a`), none incomplete
  (`after/local/partb-keys/identity-base-after.txt.gz`); each run exits 0 (`<side>/local/logs/runs.tsv`) and each
  journal balances, every compilation begun ended (`<side>/local/journal-check.txt`). The census ran at both commits
  over the parse corpus's inputs and over the suite's `test/`. The configuration holds: the slice compiler's folder,
  `eng/compare-parse-corpus.py`, the corpus and the suite pin are unchanged between the base and the after commit.
- **No difference of any kind.** `eng/compare-parse-corpus.py identity`, as the base commit holds it, over the two run
  1 reports with both census pairs, reports no difference in outcome, artifact hash, B5 specifiers or diagnostic list
  in any sub-population, B1 to B5, and both census pairs identical byte for byte; the differing keys it writes
  (`keys-base-after.tsv`) are none. The two run 1 reports are the same file (`<side>/local/reports.sha256`).
- **The control** ([`controls/partb/`](controls/partb/)), a copy of the base's run 1 with `T000001`'s artifact hash
  changed, `T000741`'s first diagnostic's column moved and `T013133`'s first B5 specifier changed with its note's hash
  and length, is reported as three differences, one artifact, one diagnostics and one specifiers difference, as
  stated.

### Part C: MET, on reading 1 of section 6, the reading of control K4 section 1's reading 4 carries

"MET if and only if every accepted input gives one hash in all four compilations, all five rows pass at both commits,
and the controls fail as stated and pass after their revert." Its population, configuration and controls are bundle
`sjl-13-001`'s part C's, the controls applied to a scratch copy of the after commit; K4 reads "removes the library's
height charge of a chain node ... the fifth row must fail" (that rule's revision of 2026-11-02, item 5).

- **One hash in all four compilations.** `eng/compare-parse-corpus.py determinism` (the base commit's) over the after
  commit's runs 1 to 3, run 1 the population report, puts every B1 and B3 compilation run 1 accepted in the population
  and finds none whose four results are not one accepted hash
  ([`determinism-after-runs-1-2-3.tsv`](after/local/determinism/determinism-after-runs-1-2-3.tsv)).
- **All five rows pass at both commits**: "the wide lowering is deterministic", "the wide lowering declares lexical
  names in source order", "two goals on the wide path in one process do not reach each other", "a deep-nesting corpus
  is refused on the wide path rather than survived" and "a left operand's spine is charged to the chain it begins",
  each `ok` in `base/local/checks.log.gz` and in `after/local/checks.log.gz`, every row of both runs passing or not
  run on this machine and claimed by nothing.
- **K1**, the global lexical declarations reversed, makes the source-order row fail
  (`controls/k1-checks-named-rows.txt`), and part B's comparison of its build's report against the after commit's run
  1, the same bytes as the base's, reports artifact differences, every one a byte difference
  (`controls/k1-identity-after.txt.gz`). **K2**, the declarations shuffled per compilation from an unseeded source,
  makes the determinism row fail, and the four-hash comparison over its three runs finds compilations that are not one
  hash (`controls/k2-*`). **K3**, one parse request shared between the goals, makes the two-goals row fail
  (`controls/k3-*`). K2 and K3 each also fail a second row, which the rule does not forbid.
- **K4 as configured** (`controls/k4-checks.*`): the whole `--checks` run stopped with exit 134 and a stack overflow
  in this component's admission walk, `JsAdmissionWalk.Expression` under `JsFrontend.Parse`, after ten `ok` lines, the
  last "the lowering is deterministic", so no row of this part printed and the fifth row did not report FAIL. Task
  7.3's probe ([`after/judgement/partc/k4-probe/`](after/judgement/partc/k4-probe/note.txt)) names the input:
  `J000451`, a stored input of the parse corpus whose function declares a variable initialised by one binary chain of
  `+` some twenty thousand operands long. The after commit with the published library refuses it at the parse with
  2103, the contract's nesting refusal, as run 1 does; under K4 the library no longer charges the chain's height,
  accepts it, and the admission walk, recursing down the chain's left spine, overflows. The frame is repeated as often
  as in the configured run, which shows the same call shape, a chain in a variable's initialiser inside a function, at
  the compile stack's capacity, not the input. What names the input is the selection: K4 changes what the library
  charges, not the tree it builds, so only an input run 1 refuses with 2103 can reach a deeper tree under K4; run 1
  refuses with 2103 inputs of B1, B2 and B4 only, none of B3 or B5, and B4, the slice front end, reads no library tree;
  of B1's and B2's, `J000451` alone overflows under K4 (the probe note's dated addition). *(Revised 2026-11-15, later,
  at the review of task 7.3: it read "overflows, with the frame repeated as often as in the configured run. No other
  input the after commit refuses with 2103 overflows under K4.", which offered the frame count as tying the configured
  overflow to `J000451`; the count is the stack's capacity for that call shape.)* The
  determinism row is the first that compiles the parse corpus's stored inputs. **Under the harness narrowed to the
  fifth row**, the adjunct the carried reading names, the same library patch makes the fifth row fail, every one of
  its sources past the bound accepted (`controls/k4-row-*`). Section 6's reading 1 takes that as K4 making the fifth
  row fail; **part C's verdict turns on it**.
- **Each revert** equals the made copy's digest (`controls/k-digests.txt`, `controls/k4-digests.txt`), and **each
  reverted copy passes**: every `--checks` row of the copy whose digest every revert of K1 to K3 equals
  (`controls/k-reverted-checks-named-rows.txt`), and the whole `--checks` run of K4's reverted copy with the published
  library restored, its library the after build's (`controls/k4-reverted-*`).

### Part D: MET, on reading 2 of section 6, what shows rule I6

"MET if and only if every test passes at the after commit, every witness is refused, N12 counts the compiler
assembly's files with a comment naming each file step 2 deleted and added, I5 registers every type and member step 2
deletes that the after commit no longer declares [...] (decision D20, as JSD-0083's note of 2026-11-13 on it reads
it), I6 finds no shared type in this family's public API baseline, and the control is refused" (the revision of
2026-11-13).

- **Every test passes at the after commit** (`after/rules/test-summary.txt`, `failed-tests.txt` empty, exit 0), after
  a Release build of the solution from a clean archive, with `BROILER_RULE_MESSAGES` set; the tests of rules N5 to N9,
  N12, N19, I5, I6 and A10 among them (`after/rules/partd-after.trx.gz`). The tree's digest is unchanged by the run
  (`after/rules/tree-digest.txt`).
- **Every witness is refused**: each of these rules' tests reads the witness its register row names and asserts that
  the rule refuses it, and every one passes.
- **N12** asserts the compiler assembly's 58 files, with dated comments naming the one file step 2 added,
  `JsTreeFacts.cs` ("The count read 62 before"), and the five it deleted, `JsSyntax.cs`, `JsFrontendAdapter.cs`,
  `JsFrontendAdapter.Expressions.cs`, `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs` ("The count
  read 63 before"): exactly the files `git diff --name-status` adds to and deletes from the assembly between the base
  and the after commit ([`partd-reads.txt`](after/judgement/partd/partd-reads.txt)).
- **I5** registers, with milestone SJL-13.3, each of the 85 types the five deleted files declare directly in a
  namespace by type, none of which the after commit declares, and, by member, the two members types that stay lost,
  `JsCompiler.FindConstructor` and `JsCompiler.Unwrapped`, which moved to `JsTreeFacts`; its register paragraph for
  SJL-13.3's step 2 names by kind, per type, the positional record parameters, enumeration members and constructors
  its reader does not read. The read against both commits' generated assurance manifests finds no unit of the deleted
  files outside a registered type and no unit of a file that stays, listed at the base and not after, unregistered
  ([`partd-reads.txt`](after/judgement/partd/partd-reads.txt)); its coarse read of the files that stay, `fields.py`,
  which prints a `gone` line for each field- or property-like name a file declares at the base and not after, prints
  none there, so it finds no such name lost. *(Revised 2026-11-15, later, at the review of task 7.3: it read "its
  coarse read of the files that stay finds no field or property name lost.", while the output shows that only by the
  `gone` lines it does not hold.)* That read is the generator's units and a line read, not rule I5's reader; the
  rule's own test, which passes, refuses any registered name declared again.
- **I6** finds no shared type in the family's public API baseline: its test, which passes at the after commit, asserts
  that rule I6 reports nothing over `src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt` against every public
  type of every admitted assembly as restored, and that its witness, a family member returning an admitted type, is
  refused. The run writes no rule-message file for group I, since the test project writes none for that group at
  either commit; the git read beside it finds the baseline the same blob at both commits, with no line naming a
  `Broiler.ECMAScript` type ([`partd-reads.txt`](after/judgement/partd/partd-reads.txt)). Section 6's reading 2 takes
  the test's pass as what shows I6.
- **The control is refused**: rule I5's control pinned by the revision, a scratch copy of the after commit declaring
  `JsMemberExpression` again with its member `NameSpan`, fails `I5_No_Retired_Member_Is_Declared_Again` (exit 1), and
  so does the second, declaring `JsNumberLiteral` again; each is refused by its type's row, the register holding no
  row for either member (`controls/i5/register-rows.txt`); the reverted tree passes and its digest equals the made one
  (`controls/i5/`).
- **The base, read and not judged** (`base/rules/`): every test passes, and the rule messages differ from the after
  commit's only in group J's file.

### Part E: MET

"MET if and only if every twin, in both forms, gives after the verdict it gave at the base at each ceiling tried, and
completes with the base's answer where it completes."

- For each of the ten twins in both forms, at its base figure and one less (`base/fuel-parity.log`, the bisect taken
  at the base, and `after/fuel-parity.log`): it completes at the figure and is refused with `AllowanceExhausted` on
  fuel at one less, at both commits, and completes after with the base's answer.

### Part F: read, no verdict (no gate clause, decision D24)

"MET if and only if every lane's condition passes, every effective-configuration line is identical, and for every
workload D <= N." Part F is no gate clause (decision D24, and the rule's "Not a gate clause"), and section 1's reading
5 takes it after step 2's merge and reads it as no gate clause; task 7.3 reads it and writes no verdict (section 6's
reading 4).

- **The condition passes**: every lane's outcome and artifact hash equal arm B's, before and after every lane
  (`measurement/conditions.tsv`: none differs), no child exited other than 0, and **one effective-configuration line**
  is seen across arms, lanes and repetitions (`measurement/summary.txt`).
- **D and N** (`measurement/summary.txt`, Q the median of the retained repetitions, in ms): classes (D −34.4, N 11.8),
  declarations (−13.2, 1.2), expressions (12.6, 21.1), literals (14.2, 1.9), module (−24.0, 4.5) and statements (8.4,
  3.0). D exceeds N for literals and statements, so the MET line's third clause does not hold for two workloads; read
  under it, part F would be NOT MET, as bundle `sjl-13-001`'s part J was on the same clause.
- **How it is read**: it decides no speed claim and nothing about another machine. Part F's collector run started one
  second after the last control run, behind the gate at a one-minute load of 0.78 (`measurement/logs/gate.log`), with
  no compiler server alive.

### Part G: MET

"MET if and only if neither of the after commit's sessions reports a finding and both controls are reported as
stated."

- The wide session over bundle `sjl-13-001`'s seeds and the four slice sessions at the after commit each end "no
  finding" (`after/local/fuzz-*.log`), each exits 0, its stderr is empty, and no findings directory was left
  (`after/local/fuzz-findings.txt`).
- **The wide control** (`controls/partg-control-wide.patch`), a throw in the binder's walk over the shared tree on an
  `ExpressionStatement`, a node the wide seeds produce, is reported as findings by the wide session
  (`controls/partg-control-wide.log`), and **the slice control**, `SliceTokenizer` throwing on a semicolon, by each
  slice session (`controls/partg-control-slice-*.log`). Each revert equals the made digest, the slice control's once
  the findings directory its own sessions wrote inside the tree is left out (`controls/k-digests.txt`).
- The base's sessions are retained and not judged; they too report no finding.

## 5. Clause 8's publication, and the closures (2026-11-15, task 7.3)

The rule leaves clause 8's publication to "the collector's runs at this file's after commit" ("What this file does not
decide"). They are read, not judged. [`after/publication/`](after/publication/README.md), taken at task 7.3 (section
6's reading 13), shows every JavaScript root (ExecutionOnly, SliceCompiler, Conformance, Cli, PolyglotCli; Android
under EX-126) published and run under the JIT, trimming and Native AOT at the after commit, the collector's last line
reading "every root published and run in three modes: True". The compiler-bearing roots' JIT and trimmed closures
carry `Broiler.ECMAScript.Frontend` and ExecutionOnly's does not, so rule I3's boundary holds at step 2 as at step 1;
every root's closure and catalog lists the same assemblies as step 1's (`sjl-13-001/after/publication/closures/`),
since step 2 changes files within the compiler assembly only, and the admitted packages are those of the shared
stream's `0.1.0-preview.11`, the after commit's (`closures/admitted-packages.json`). The Native AOT images' mstat
records are retained by digest only, as in the earlier bundles. The composition register's four rows already name the
library for the compiler-bearing roots, by step 1's closures, and step 2 moves no row; rule K4 holds them at this
branch's head, where the architecture tests pass.

## 6. Readings the rule does not settle (2026-11-15, task 7.3)

**Each reading below was taken by task 7.3 under the owner's standing direction of 2026-10-18 ("Please do all
recommended decisions and skip waiting for review"), as the recommended choice, and recorded as the owner's dated
decision under that direction; none is the owner's own dated answer, asked and answered, and the owner may rule
otherwise.** Where a verdict turns on one, it says so.

1. **K4's "must still make the fifth row fail" (part C).** As configured, `--checks` under K4 died of a stack overflow
   in this component's admission walk before any of part C's five rows printed; the probe shows the input, `J000451`,
   which the published library refuses at the parse and the patched library accepts. Section 1's reading 4 carries
   bundle `sjl-13-001`'s reading 2, which the owner ruled on 2026-11-11 ("Narrowed run counts": the harness-narrowed
   adjunct, under the same library patch, meets "must still make the fifth row fail", and the configured run's stack
   overflow is a second defect of the same patch). Taken: the carried reading applies as ruled. The adjunct makes the
   fifth row fail, and the configured run's overflow is a second defect of the same patch, in step 2's admission walk
   where step 1's was in the adapter: the walk meets a tree past the bound only because the patched library no longer
   refuses it. Not taken: the ruling covered the adapter's overflow alone, and an overflow in step 2's own walk needs
   the owner's ruling again or a dated revision naming the adjunct; then K4 did not make the fifth row fail and **part
   C is NOT MET** until either. **Part C's verdict turns on this reading.** The carried reading is itself section 1's
   reading 4, taken under the same direction, not the owner's ruling for this bundle.
2. **What shows rule I6 (part D).** The plan of step 2's evidence said to read I6's rule-message file for "finds no
   shared type in this family's public API baseline", but the test project writes no file for group I at either
   commit, so the run retains none. Taken: the I6 test's pass at the after commit shows it, since its assertion is
   that rule I6 reports nothing over the family baseline against every admitted assembly's public types, and that its
   witness is refused; the git read of the baseline, the same blob at both commits and naming no `Broiler.ECMAScript`
   type, stands beside it. Not taken: only a rule-message file shows I6; then I6 is not shown, and **part D is NOT
   MET**, until the test project writes group I's messages, a change of a test this rule runs. **Part D's verdict
   turns on this reading.**
3. **The hosted SDK and images (part A).** Every runner and the merging job of both hosted runs report .NET SDK
   11.0.100, while this family's local machine has 10.0.404 and step 1's hosted runs had 10.0.403; the images differ
   between runners of one run. The workflow asks `actions/setup-dotnet` for `10.0.x` and the repository pins no SDK,
   so the image's newer SDK built the conformance root (section 7's second finding). The rule's configuration is
   bundle `sjl-13-001`'s part A's: the workflow and the driver at the dispatched ref byte-identical to the base
   commit's, which hold, and "the conformance composition root that commit builds in Release on every runner", which
   it is; it names no SDK and no image, and the re-runs' local machine is section 1's reading 4's (reading 10). Taken:
   the hosted runs are as configured, both built with the same SDK, and `imported.txt` records the SDK and the images
   *(revised 2026-11-15, later, at the review of task 7.3: it read "both built alike", which the images, different
   between runners, do not bear out)*. Not taken:
   hold the hosted runs to the local machine's SDK or to step 1's, which would void both runs and ask for dispatches
   again with a workflow the rule holds to the base's. **Part A's verdict turns on this reading**, which no difference
   of the comparisons does.
4. **Part F gets no verdict.** The rule writes a MET line for part F and calls it no gate clause; section 1's reading
   5 takes it after the merge and reads it as no gate clause; this stage's brief and the plan's question 3 say it gets
   no verdict. Taken: part F is read, its clauses set against `measurement/summary.txt` in section 4, and no verdict
   is written. Not taken: judge it under its MET line, as bundle `sjl-13-001`'s README judged its part J, on the
   rule's own words "Seven predeclared decisions, one per part", which count part F's among them; it would then be NOT
   MET on its third clause, for literals and statements. No gate clause turns on either. *(Revised 2026-11-15, later,
   at the review of task 7.3: it read "Not taken: judge it under its MET line, as bundle `sjl-13-001`'s README judged
   its part J; it would", and did not quote the rule's words the alternative rests on.)*
5. **Task 7.2's reading 1, "re-runs agree" (part A)**, confirmed: all six re-runs of a row, three at each commit, give
   one verdict and one exhausted dimension. Not taken: agreement within each commit only. Either gives the same rows
   here.
6. **Task 7.2's reading 2, the (e) race (part A)**, confirmed: the rule runs it for "a (c′) row whose re-runs differ",
   and none does, so nothing is raced. Not taken: race every (c′) row anyway, as the plan's "the (e) race on an idle
   machine" could be read; the rule's words decide.
7. **Task 7.2's reading 3, K4's library (part C)**, confirmed, with a correction of its reason: the rule asks for "a
   scratch copy of the library", which at the after commit is `0.1.0-preview.11`, built from Broiler.JS `e4b6b49e`
   (its tag `ecmascript-v0.1.0-preview.11`); task 7.2 packed `29061d65`, the preview.10 tag's commit, as preview.11.
   Between the two, the Frontend's and the Algorithms' sources differ in comment and empty lines only
   (`controls/note.txt`), so the scratch copy is the after commit's library's code with the patch. Not taken: take K4
   again from `e4b6b49e`, or from preview.10 with the copy's version pins changed. No verdict turns on it.
8. **Task 7.2's reading 4, part E's folder named `parte/`**, confirmed: it names this rule's part, and the README says
   where bundle `sjl-13-001` kept the same runs. A name only. Not taken: the precedent's `parti/`. No verdict turns on
   it. *(Added 2026-11-15, later, at the review of task 7.3: the alternative not taken, which the reading lacked.)*
9. **Task 7.2's reading 5, one reverted copy for K1 to K3**, confirmed: the rule asks that each control "pass after
   their revert", and each revert's digest equals the made copy's, which is the after commit's tree, so one copy of
   that tree, built afresh, carries each check, as bundle `sjl-13-001` did; K4's revert, which restores the published
   library, has its own copy. Not taken: build each reverted copy separately. No verdict turns on it.
10. **Task 7.2's reading 6, rule I5's controls in part D's tree (part D)**, confirmed: the revision asks for "a
    scratch copy of the after commit"; part D's tree is a clean archive of the after commit, and its digest after each
    control equals the made one. Not taken: a separate scratch copy. No verdict turns on it.
11. **Task 7.2's reading 7, part G's wide throw in the binder's walk**, confirmed: the rule places it "in a lowering
    or binder walk over the shared tree that the seeds reach", and the binder's walk is one, over the shared tree's
    `ExpressionStatement`. Not taken: the lowering's statement walk. No verdict turns on it.
12. **Task 7.2's reading 8, part F's collector unchanged**, confirmed: the rule takes bundle `sjl-13-001`'s part J
    whole, so that bundle's `collect-measurement.py` ran unchanged and its outputs name "sjl-13-001 part J"; the
    README says so. Not taken: edit the collector. No verdict turns on it.
13. **Clause 8's publication taken at task 7.3.** The rule leaves clause 8 to "the collector's runs at this file's
    after commit", and no earlier task of this bundle took them. Taken: task 7.3 takes them as step 1's were taken,
    from a clean clone of the repository detached at the after commit (step 1 used a detached worktree), and retains
    them in `after/publication/` unchanged, the identity record's first line naming bundle `sjl-7-001`'s cut-over as
    the collector writes it. Not taken: leave clause 8's publication at step 2 untaken, for SJL-13's exit reading. No
    part turns on it.

## 7. Findings (2026-11-15, task 7.3)

- **K4's overflow is the control's, not the product's.** The input that overflows step 2's admission walk under K4 is
  `J000451`, a stored input of the parse corpus, a binary chain past the tree-depth bound. Unpatched, the library
  refuses it at the parse with 2103 before the admission walk sees it, at both commits, in every run that compiles it:
  part B's runs, part C's runs and checks, and the probe's copy with the published library. No normal run of this
  bundle lost its process: part B's and part C's runs balance their journals with none crashed, every `--checks` row
  passes, the fuzz sessions report no finding, and part A's four hosted runs are complete; the row "the binder walks a
  tree at the tree-depth bound" compiles a binary chain exactly at the bound through `JsCompiler.Compile`, and so
  through the admission walk, with the process alive, and the rows "a deep-nesting corpus is refused on the wide path
  rather than survived" and "a left operand's spine is charged to the chain it begins" compile chains within the bound
  with the process alive. *(Revised 2026-11-15, later, at the review of task 7.3: it read "the rows "a deep-nesting
  corpus is refused on the wide path rather than survived" and "a left operand's spine is charged to the chain it
  begins" compile the chains within the bound with the process alive.", which did not name the row that compiles a
  chain at the bound.)* The walk is recursive, and it relies, as rule
  N19 says the compiler does, on the bound the library enforces against the declared stack; K4 removes that bound for
  chains. This is not a product finding, and no part's verdict turns on it but part C's, through section 6's reading
  1.
- **The hosted runs built with .NET SDK 11.0.100.** `.github/workflows/test262-evidence.yml` asks
  `actions/setup-dotnet` for `10.0.x`, and with no `global.json` in the repository the newer SDK the runner image
  holds builds the conformance root, as both runs' `run.txt` and `*.runners.txt` record. Both runs built with the same
  SDK, on images that differ between runners of one run, and no verdict moves *(revised 2026-11-15, later, at the
  review of task 7.3: it read "Both runs built alike", which the images do not bear out)*; the workflow is frozen
  under this rule, and whether it should pin the SDK is for a later change, after this bundle's evidence merges.
- **No figure of the base answers or of the development checks** is quoted here; this branch's own checks, a clean
  build of the solution and the architecture and contract tests at its head, are no run of the rule.

## 8. What comes next (2026-11-15, task 7.3)

Nothing here moves a pin or starts a later step. Before the evidence merges: the review of task 7.3 and its fixes. The
owner may rule on the readings of section 6, of which parts C, D and A turn on readings 1, 2 and 3. The hold on the
default branch (section 1's reading 1) lifts when this bundle's evidence merges. SJL-13's exit reading reads both
bundles; no clause is met here, and nothing is accepted.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition. *(Added 2026-11-14: EX-135, for the code step 2 moved out of this component's
assurance coverage, joins the table, and the row of SJL-13.1 is kept as written.)*

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-13.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
| EX-135 | Open | **The adapted tree and its adapter, which left this component's assurance coverage** *(added 2026-11-14, at step 2's records, as JSD-0087's dated note of that day records)*. The types rule I5 registers for SJL-13.3's step 2 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at step 2: `JsSyntax.cs`, `JsFrontendAdapter.cs`, `JsFrontendAdapter.Expressions.cs`, `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs` whole, with every type in them. What the admission walk, the numeric admission, the binder and the lowering read in the adapted tree's place is `Broiler.ECMAScript.Frontend`'s own tree, namespace `Broiler.ECMAScript.Syntax`, which the wide path has parsed with since step 1. The home's own assurance covers it in the Broiler.JS repository: its generated manifest, `Broiler.ECMAScript/hairs.manifest.json`, lists every unit of the library's assembly with an annotation on each relevant declaration, checked in CI (rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count the adapted tree's or the adapter's units. The request and the side tables it hands on (`JsFrontend.cs`), the readings of the shared tree (`JsTreeFacts.cs`, which holds `FindConstructor` and `Unwrapped`, members rule I5 registers as `JsCompiler`'s), the admission walk (`JsAdmissionWalk.cs`), the diagnostics and their texts, the providers, the numeric admission, the binder and the lowering stay in this component and stay covered. EX-125 and EX-127 to EX-134 say the same of the earlier areas |

## 10. SJL-13's exit reading, Broiler.VM's half, 2026-11-15

*(Added 2026-11-15, after both steps' evidence had merged, bundle [`sjl-13-001`](../sjl-13-001/README.md)'s in
Broiler-Platform/Broiler.VM#197 and this bundle's in Broiler-Platform/Broiler.VM#207, and after the records that observe
them merged in Broiler-Platform/Broiler.VM#208.)* It reads the milestone's clauses 4 to 10, this family's, against both
bundles' evidence at the commit each piece was taken. It changes no rule, widens none, revises no clause and declares
nothing of SJL-13 met. The home's clauses 1 to 3 are Broiler.JS's, read in its bundle `sjl-13-001` (observed). The
record over both repositories, each clause 1 to 10 with its reading and where it is recorded, JSD-0083's settling
condition read and the open items, is the programme ledger's entry of this date, as that ledger placed SJL-9's (its
entry of 2026-11-12), each bundle reading its own repository's half; SJL-13's Broiler.VM half has two bundles, one per
step, and this section, in the bundle of SJL-13's final state, reads both.

**What it reads.** The milestone's words are
[`sjl-13-shared-syntax-and-vm.md`](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md) at the
default branch's `6efc06315d40bafd68048c5e55fa5064010369fe` (the merge of Broiler-Platform/Broiler.VM#208), the head
when this reading was taken, with the readings of 2026-10-26 noted under its work packages and its exit gate. The
evidence:
- **step 1**, bundle `sjl-13-001`: its rule's run at the base `df75f8cea79aa2452ca22892d47d8afa5a14c93f` and the after
  commit `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4` (Broiler-Platform/Broiler.VM#196), judged in its section 4, with
  its section 5's readings, every one of them the owner's own dated answer by the rulings of 2026-11-11 and
  2026-11-12, and the publication at the after commit (its `after/publication/`);
- **step 2**, this bundle: its rule's run at the base `389b55c9ec9bb77409bb2487067fbebc0868849b` and the after commit
  `467ad7c395234b335c816b8f49ae5b505740e32c` (Broiler-Platform/Broiler.VM#206), judged in section 4, with section 6's
  readings, each taken under the owner's standing direction of 2026-10-18 and none the owner's own answer, and the
  publication at the after commit (section 5, [`after/publication/`](after/publication/README.md));
- **the head's rule checks**, [`exit-reading/`](exit-reading/), taken for this reading before any change of it, at
  `6efc0631`: a clean `git archive` on a tmpfs ([`local/archive.txt`](exit-reading/local/archive.txt)), whose scratch
  folder is named `base` in the retained records because the checks are this reading's answers before it changed
  anything, though they hold the head and not either rule's base; the solution built there in Release with
  `--disable-build-servers`, with no warning ([`build-solution.out.txt`](exit-reading/local/build-solution.out.txt),
  [`build-warnings.txt`](exit-reading/local/build-warnings.txt), empty), then the architecture suite and the contract
  tests with `--no-build`, each passing ([`test-architecture.out.txt`](exit-reading/local/test-architecture.out.txt),
  [`test-contract.out.txt`](exit-reading/local/test-contract.out.txt)). Each wrote a TRX of every test, which is not
  retained ([`trx.sha256`](exit-reading/trx.sha256)); the architecture suite's rows for rules I1 to I7, N5 to N9, N12
  and N19 are [`rules.tsv`](exit-reading/rules.tsv), each `Passed`, by
  [`rules-from-trx.py.txt`](exit-reading/scripts/rules-from-trx.py.txt). Each step ran under the exclusive lock and
  behind the hard idle gate this bundle's runs used, which wait, with no fallback, for a one-minute load of at most 1.0
  and no dotnet, testhost, VBCSCompiler, MSBuild, ilc, crossgen or `Broiler.*` process
  ([`gate.log`](exit-reading/local/gate.log), [`checks.sh.txt`](exit-reading/scripts/checks.sh.txt) with
  [`run.sh.txt`](exit-reading/scripts/run.sh.txt) and [`gate.sh.txt`](exit-reading/scripts/gate.sh.txt)), whose
  `stop-servers` lines name no leftover process, on this family's local machine (four logical processors, .NET SDK
  10.0.404, Ubuntu 24.04.5 LTS, each run's `machine.txt`);
- [`exit-reading/deletion-search.txt`](exit-reading/deletion-search.txt), git reads alone at `6efc0631`
  ([`deletion-search.sh.txt`](exit-reading/scripts/deletion-search.sh.txt)): the files the two steps deleted searched by
  name, every name rule I5's register holds for SJL-13.3 that names a type searched as a declaration in the family's
  product sources, and the shared library's parser and package reference where the wide path parses;
- [`exit-reading/verify-hashes.txt`](exit-reading/verify-hashes.txt): every `hashes.txt` of both bundles checked with
  `sha256sum -c` in a clean archive of `6efc0631`, and both bundles' `manifest.json` checked against the tree of the
  commit each was collected at, all of them verifying
  ([`verify-hashes.sh.txt`](exit-reading/scripts/verify-hashes.sh.txt),
  [`verify-manifest.py.txt`](exit-reading/scripts/verify-manifest.py.txt));
- **for clause 10**, the profile's JSC entries
  [JSC-329](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-329) and
  [JSC-330](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-330) and its ledger's JS-2 and
  JSB-1 rows, filed by SJL-13.5 at `dfc818e29133210bd4fe44ec94ce16b83c542f3b`, this reading's first commit, and JSC-329
  as later commits of the same change extend it before it merges: a dated addition, made at `3cc0d2d2` and its date line
  corrected at `04121034`, which reads JS-2's closure on the delivery plan's statement of its gate as well, and, at the
  review of this change, each of JSD-0063's requirements on the shared frontend mapped to what holds it, with the reading
  that takes the home's rules as observed and its alternative.

No path outside `docs/` and the profile's `docs/` differs between the after commit `467ad7c3` and the head `6efc0631`,
and the commits that carry this reading change documents and `exit-reading/` alone, so the code the head's checks read
is step 2's. The registry's last change is the registry commit `cbee7c4`, revision 19, at both after commits and the
head. [`exit-reading/hashes.txt`](exit-reading/hashes.txt) holds the SHA-256 of every other file of that folder, which
`sha256sum -c` verifies. The architecture suite also reads documents (rule I7 reads bundles), so the same checks were
taken again at this change's later head `04121034c0bc102f953254e8596b0c478c75492a`, which carries the reading's
documents, from a clean `git archive` by the same scripts, behind the same lock and gate: the solution built with no
warning, both suites passed, and the rows of rules I1 to I7, N5 to N9, N12 and N19 are those of
[`rules.tsv`](exit-reading/rules.tsv), each `Passed`. That run is not retained; this folder keeps the run at
`6efc0631`.

**Whose readings these are.** A clause below rests on a reading where the documents did not settle its words. Step 1's
are the owner's own rulings: bundle `sjl-13-001`'s readings 2, 3, 6, 7 and 11 ruled on 2026-11-11 ("Narrowed run
counts", "Not undone", "Accept both", "Counts as match") and readings 1, 4, 5, 8, 9 and 10 on 2026-11-12 ("Accept all
five", "Accept as taken"). Step 2's readings 1 to 3 of section 6, on which parts C, D and A turn, are taken under the
owner's standing direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review") as the
recommended choice; the owner may rule otherwise. So are JSD-0083's decisions of 2026-10-26, its readings of the
milestone's words noted under SJL-13.3 and the exit gate, and its notes of 2026-11-10 and 2026-11-13 on decisions D20
and D15; none is the owner's own dated answer.

| Clause | Its words | Read from, at the commit | Reading, and what it rests on |
|---|---|---|---|
| 4 | "`JsParser` is gone, and the wide path parses with the frontend." | bundle `sjl-13-001`, section 4's commit conditions and part D, at `a8d8398`; this bundle, section 4's commit conditions and part D, at `467ad7c3`; [`deletion-search.txt`](exit-reading/deletion-search.txt) and [`rules.tsv`](exit-reading/rules.tsv) at `6efc0631` | **Read met** at `a8d8398`, at `467ad7c3` and at the head. At step 1's after commit `JsParser.cs` is absent and the wide path parses through `Broiler.ECMAScript.Frontend`, rule I5 registering `JsParser` and the rest of step 1's deletion list (bundle `sjl-13-001`, part D, MET); at step 2's, `JsSyntax.cs` and `JsFrontendAdapter.cs` with its parts are deleted and the admission walk, the numeric admission, the binder and the lowering read the shared tree, rule I5 registering what they declared (part D, MET). At the head no file either step deleted is in the tree, no type the register names for SJL-13.3 is declared again, `JsFrontend.Parse` calls the library's `SyntaxParser`, and rules I5 and N19, which counts the library's namespace as a parser that must run on the declared stack, pass. It rests on readings, on a revision of each step's rule and, for step 1's commit conditions, on an owner's ruling: the milestone's "Deleted: `JsParser`" read with step 1's deletion list (decision D20) and "the adapter and `JsSyntax`'s parse-only parts go" read as `JsSyntax` whole (decision D19), both of 2026-10-26; JSD-0083's note of 2026-11-10 on decision D20, which names the binder's checks of `syntax` rows by row where rule I5 cannot register them, written into bundle `sjl-13-001`'s part D by that rule's revision of 2026-11-10, and its note of 2026-11-13 on the same decision, written into this bundle's part D by this rule's revision of 2026-11-13, each revision filed before any run it judges; and section 1's reading 7 on where the facts are computed at step 2. Each of these was taken under the owner's standing direction of 2026-10-18 and is not the owner's own answer. Step 1's commit conditions, on which no part of bundle `sjl-13-001` is void, rest for the registry commit's "in each path only what the list says" on that bundle's reading 11, which the owner ruled on 2026-11-11 ("Accept both"). Part D's verdict here turns on section 6's reading 2, rule I6, which clause 4 does not name |
| 5 | "Artifacts are byte-identical over the parse corpus, or every difference is classified under the rule." | part B of bundle `sjl-13-001` at `df75f8c`, the registry commit `cbee7c4` and `a8d8398`; part B of this bundle at `389b55c9` and `467ad7c3` | **Read met.** Step 1's every difference between the base and the after commit is in a class its rule admits and is listed (R, R2, S, F for rows P37, D64 and P15b, and N), and B4 and the census show none but R (bundle `sjl-13-001`, part B, MET); step 2's runs show no difference of any kind in any sub-population, B5's ordered specifiers among them, and both census pairs are identical (part B, MET). It rests on the owner's rulings of 2026-11-11 on bundle `sjl-13-001`'s readings 3 ("Not undone"), 6 and 11 ("Accept both"), on which step 1's part B turns, and on that rule's revisions of 2026-11-02 to 2026-11-10, each filed before any run of step 1: the revision of 2026-11-02 on the owner's decision taken as recommended under the standing direction of 2026-10-18; those of 2026-11-04, 2026-11-08, 2026-11-09 and 2026-11-09, later that day, on the owner's own dated decisions; and that of 2026-11-10 on a reading taken under the direction. Two of them were written after a development pre-check of step 1, on a local branch never pushed, had seen the differences they admit: the revision of 2026-11-09 (class N's bounded class for the places and codes of refused inputs, "Revise rule: bounded class", and two corrections in class F) after the pre-check on `sjl-13-3-step1b`, and the revision of 2026-11-09, later that day (rows T33, P38 and P39 in class F, "Rows + class F", and class N widened, "Widen N narrowly"), after the pre-check on `sjl-13-3-step1c`. Each says no run it judges had been taken; what was seen was a pre-check, not a run the rule judges, but a rule revised after the differences it admits were seen is weaker evidence for part B, the same kind of fact as the home's clause 3 records. Step 2's part B rests on no reading |
| 6 | "test262 in both forms meets the rule." | part A of bundle `sjl-13-001`, hosted runs at `df75f8c` and `a8d8398`, with its re-runs; part A of this bundle, hosted runs at `389b55c9` and `467ad7c3`, with its re-runs | **Read met.** Step 1: all four runs complete, every changed verdict of either pair in class C1 kind (F), each a row of this family's corrections, or C4, and listed (bundle `sjl-13-001`, part A, MET), on readings 1 and 10 that the owner ruled on 2026-11-12, on which no verdict turns. Step 1's part A rests on two revisions of its rule: its MET line as the revision of 2026-11-02 reads it, the owner's decision taken as recommended under the standing direction of 2026-10-18; and kind (F)'s rows T33, P38 and P39, which the revision of 2026-11-09, later that day, adds on the owner's own decision of that day ("Rows + class F"). Without that revision the variants of `test/staging/sm/RegExp/7.8.5-01.js`, `test/staging/sm/statements/arrow-function-in-for-statement-head.js` and `test/staging/sm/class/compPropNames.js` would each read as a regression, and part A would not be met. It was written after a development pre-check of step 1 had seen those three files fail at the base and pass after, and filed before any run part A judges; it is weaker evidence for part A for that reason. Step 2: all four runs complete, every changed verdict in C4 (c′), a wall-clock exhaustion whose re-runs agree, and none a regression (part A, MET). Step 2's rests on section 6's reading 3, taken under the owner's standing direction of 2026-10-18: the hosted runs as configured, built with .NET SDK 11.0.100 on images that differ between runners, which the rule does not name; the alternative would void both runs |
| 7 | "Rules N5 to N9, N12 and N19 hold. The registry revision is filed, if any." | part D of bundle `sjl-13-001` at `a8d8398` and its part E (the registry and its mapping); part D of this bundle at `467ad7c3`; [`rules.tsv`](exit-reading/rules.tsv) at `6efc0631` | **Read met** at both after commits and at the head: every test of rules N5 to N9, N12 and N19 passes, in a suite that passes whole, each rule refusing the witness its register row names; N12 counts the compiler assembly's files with a dated comment naming each file each step added and deleted. Registry revision 19 is filed at step 1's registry commit (decision D12), and the mapping checker passes at step 1's after commit and refuses its three controls (bundle `sjl-13-001`, part E, MET); step 2 takes no revision, since it changes no meaning, and the registry's last change is the registry commit. It rests on no reading of clause 7's own words, but on an owner's ruling and on revisions of step 1's rule, the owner's own: registry revision 19's content, part D's population (rules N5 to N9, N12 and N19) and part E's checker are as the revision of 2026-11-04 sets them (its items 1, 7 and 8), on the owner's decision of that day ("Mixed fix"); the registry commit's file list, which step 1's commit conditions check, is the revision of 2026-11-08's (its item 1), on the owner's decision of that day ("Revise rule + rebuild"); and that the registry commit changes "in each path only what the list says" rests, for its code-swap sites and the mapping's lines, on bundle `sjl-13-001`'s reading 11, which the owner ruled on 2026-11-11 ("Accept both"). Step 2's part D is judged on this rule's revision of 2026-11-13, taken under the standing direction, and turns on section 6's reading 2, which reads rule I6, not one of the rules clause 7 names |
| 8 | "Rule `I3` keeps the frontend out of the compiler-free roots. Every root is published and run in three modes." | bundle `sjl-13-001`'s part D and [`after/publication/`](../sjl-13-001/after/publication/README.md) at `a8d8398`; this bundle's section 5 and [`after/publication/`](after/publication/README.md) at `467ad7c3`, and the I3 test in part D's suite there; [`rules.tsv`](exit-reading/rules.tsv) at `6efc0631` | **Read met** at `a8d8398` and `467ad7c3` for every JavaScript root this machine can publish, ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, each published and run under the JIT, trimming and Native AOT, the collector's last line reading "every root published and run in three modes: True" at both commits. In both closures the compiler-bearing roots carry `Broiler.ECMAScript.Frontend` and ExecutionOnly does not, and rule I3, whose register row names the frontend clause's subject from step 1, passes at both after commits and at the head. It rests on an exclusion, EX-126, for the Android root (bundle `sjl-3-001`), as SJL-3's, SJL-8's, SJL-10's and SJL-9's readings of the clause did, and on section 6's reading 13, taken under the owner's standing direction of 2026-10-18, that step 2's publication was taken at task 7.3; no verdict turns on that reading |
| 9 | "Fuzzing runs clean under the rule." | part G of bundle `sjl-13-001` at `a8d8398`; part G of this bundle at `467ad7c3`, SJL-13's final state | **Read met.** At each after commit the wide session over bundle `sjl-13-001`'s seeds and the four slice sessions end "no finding", and each control is reported as stated (bundle `sjl-13-001`, part G, MET; this bundle, part G, MET), the wide control at step 2 in the binder's walk over the shared tree. It rests on no ruling and no revision; section 6's reading 11, which places step 2's wide control, is taken under the owner's standing direction of 2026-10-18, and no verdict turns on it |
| 10 | "JS-2 closes, with its JSC entries." | JSC-329 and JSC-330, and the profile ledger's JS-2 and JSB-1 rows, filed at `dfc818e2`; JSC-329 as later commits of this change extend it, at this change's head | **Read met** at `dfc818e2`, and on JSC-329 as later commits of this change extend it before it merges (its dated addition, made at `3cc0d2d2`, reading the delivery plan's statement of JS-2's gate as well, and its requirements mapped at the review of this change), as JSD-0083's decision D33 reads the clause: "the record of a closure that clauses 4 to 6 settle". JS-2's re-scoped gate reads met on both bundles, clauses 4 to 6 above with JSD-0063's requirements, and JSC-329 records it; the profile ledger's JS-2 row reads `[FULL]` and `In progress`, closed on the bundles and not accepted, and its blocker on the shared frontend is closed. JSC-330 and the JSB-1 row decision D34 calls for are filed in the same commit. It rests on readings taken under the owner's standing direction of 2026-10-18: decision D33 itself, and the reading that "closes" is the row read closed on the bundles in the ledger's own vocabulary, `In progress` and not `Accepted`, which needs review (EX-30), and not the row kept `Blocked` until SJL-13's gate is met; and the reading JSC-329 records of decision D33's "JSD-0063's requirements on bundles `sjl-13-001` and `sjl-13-002`": met by the programme's rules, this family's on its own bundles (part C's two-goals and deep-nesting rows; rule I2 for rules B5's and B5b's lists; the publications with warnings as errors for the code a root reaches) and the home's, read as observed, for what no rule of this family reads (rule SL2 for thread-static and async-local state in the library; SL10 and SL3 for conditional compilation and run-time generic construction; SL5 and the Native AOT probe for the code no root reaches; SL1 for the delivery plan's "no edge to an engine"). The alternative, that under the profile's invariant 12 those requirements are not shown until a bundle of this family shows them, would hold the row at `[PARTIAL]` and leave clause 10 unmet until then |

**So, of this family's clauses 4 to 10:** each reads met on bundles `sjl-13-001` and `sjl-13-002`. Clause 4 rests on
JSD-0083's readings of the milestone's words and on part D's revisions of 2026-11-10 and 2026-11-13, taken under the
owner's standing direction, and step 1's commit conditions on the owner's ruling on reading 11; clause 5 on the owner's
rulings of 2026-11-11 and on step 1's rule revisions, each filed before any run, two of them written after a
development pre-check had seen what they admit; clause 6 at step 1 on the revisions of 2026-11-02, taken under the
direction, and of 2026-11-09, later that day, the owner's own, written after the pre-check had seen the three files it
admits, and at step 2 on section 6's reading 3, taken under the direction; clause 7 on the owner's ruling on step 1's
reading 11 and on the owner's revisions of 2026-11-04 and 2026-11-08; clause 8 on EX-126 for the Android root; clause
10 on decision D33, the reading of "closes" and the reading of JSD-0063's requirements that JSC-329 records; clause 9
on no ruling and no revision. Every clause read met on the bundles; the gate waits on SJL-12's, SJL-3's and SJL-7's
gates and on review (EX-30). The programme ledger's entry of this date gives the record over both repositories,
JSD-0083's settling condition read, and the open items.
