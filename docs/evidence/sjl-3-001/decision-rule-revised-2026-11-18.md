<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-3-001's decision rule, revised again after part C's second run was seen: one more measurement, the third and last, on the controlled lane `win-x64-r7-5800x`

**Written:** 2026-11-18, **after** part C's first two measurements had been seen: [`measurement/`](measurement/), NOT
MET at `49ea00e` on its clauses 1 and 3 under [`decision-rule.md`](decision-rule.md), and
[`measurement-revised/`](measurement-revised/README.md), NOT MET on its clause 3 alone under
[`decision-rule-revised-2026-10-20.md`](decision-rule-revised-2026-10-20.md). The decision it gives is therefore
**weaker evidence** than the one `decision-rule.md` gives, as that rule's own "Revisions" paragraph says, and it is
no stronger than the one the revision of 2026-10-20 gives, which it revises. **Owner:** the JavaScript profile owner,
with the core architecture owner. **Reviewer:** none; every role is one person (EX-30), and this revision does not
claim that anyone independent has read it. Neither earlier rule file is edited; this file quotes the revision of
2026-10-20 whole below, and that revision quotes `decision-rule.md` whole.

**Why it was written.** The revision of 2026-10-20 made `measurement-revised/` part C's last measurement. Its last
reading says: "this is the second and last measurement of part C: a NOT MET here is not followed by a third under a
further revision, and leaves clause 13 open until the owner's next dated choice under decision SJL-D-23." That
decision's note of 2026-10-20 says the next dated choice "is taken when either (a) a controlled lane exists,
recorded as `Measurement.md` requires; or (b)" the clauses it lists are the only unmet ones, and that "That choice is
the owner's own when it comes, not one taken under the owner's direction of 2026-10-18." Trigger (a) was reached on
2026-11-17: Broiler.JS recorded the controlled lane `win-x64-r7-5800x` in its
`docs/evidence/lanes/win-x64-r7-5800x/`, merged in Broiler-Platform/Broiler.JS#1121 as
`dcab5079d5b0696d6fcc593c848a654d0ab2a107` (observed, and not re-read here).

**The owner's choice of 2026-11-18 that this file records.** On 2026-11-18 the owner was asked: "SJL-3 clause 13
(Broiler.VM's measurement) is open after the last measurement its 2026-10-20 revision allows. What should happen now
that a controlled lane exists?" The owner chose "Third run on the lane": "You make a new dated revision of the rule
whose only change is one more measurement on win-x64-r7-5800x, with the tolerance unchanged. I draft it for your
words; the run happens in a local session on your workstation." The alternatives not taken were "Keep it open" ("No
revision and no run. Clause 13 stays open until trigger (b), where the choice is between waiting and a gate
revision.") and "Gate revision now" ("Revise SJL-3's exit gate under SJL-D-23 alternative (b) now, instead of
measuring. This would be the first exit-gate revision made after the evidence it concerns."). **This is the owner's
own dated choice, taken under decision SJL-D-23 at its trigger (a)**, as that decision's note of 2026-10-20 reserves
it, and **not one taken under the owner's direction of 2026-10-18** ("Please do all recommended decisions and skip
waiting for review."). This file is the revision it names. It revises no exit gate.
- **What this departs from.** The revision of 2026-10-20 said a NOT MET there "is not followed by a third under a
  further revision". This file is a third measurement under a further revision. It exists only because the owner's
  next dated choice under SJL-D-23, which that same reading names as clause 13's next step, chose it; that reading
  stands as the record of what the revision of 2026-10-20 allowed, and `measurement-revised/`'s reading under it is
  unchanged.
- **The tolerance is not touched.** As the revision of 2026-10-20 says of its own: the core's rule 3 makes the A/A
  lane "the one judgement the harness makes" ([`docs/baselines.md`](../../baselines.md) section 1), and a tolerance
  chosen after a run had been seen would be chosen to pass it. Both earlier runs have now been seen.
- **What a reading can show**, in the revision of 2026-10-20's own words, which hold on this lane as on the
  container: "Under this tolerance, a change that makes no difference at all reads NOT MET more often than MET: each
  workload and form must have D at most N, and N is only the distance between two medians of the same binaries.
  **A NOT MET is therefore not a finding of a slowdown, and a MET is not a speed claim.**"

## What changed, and why

**One change: the machine.** The revision of 2026-10-20 took its run "on the local machine part C's first run was
taken on, with no other build, test or run on it while the run is taken." This revision allows **one more
measurement, the third and last, on the controlled lane `win-x64-r7-5800x`**, under the conditions below, and that
sentence's machine is replaced by them. Nothing else of part C changes; the rest of this section says what one more
measurement on the lane needs, and nothing more.

1. **The lane.** One machine and one RID, `win-x64`, recorded as Broiler.JS's `docs/roadmap/Measurement.md` requires
   (its section "Controlled lanes and effective configuration") and as the lane's record has it at Broiler.JS
   `dcab5079d5b0696d6fcc593c848a654d0ab2a107`, in `docs/evidence/lanes/win-x64-r7-5800x/`. The record's files are
   named by the SHA-256 of the bytes that commit stores, as Broiler.JS's sub-bundle
   `docs/evidence/sjl-3-001/measurement-win-x64-r7-5800x/manifest.txt` names them:

   ```
   2f3facaf059322c1f7917c5ff1a67edba79bdc6deaeb90bbceb3022dcb00d1e3  docs/evidence/lanes/win-x64-r7-5800x/capture.ps1
   b6166404cdca51839ba87756aaf22c02b150c78e8a99f54167194dc464e74881  docs/evidence/lanes/win-x64-r7-5800x/machine.txt
   3d047ce2d68768766abd84048af2c4fcc7406323081e18d8594f8285b5bd4b80  docs/evidence/lanes/win-x64-r7-5800x/README.md
   ```

   The lane is Broiler.JS's record, read here and not re-read: this repository records no lane of its own, and it
   reads the lane's README's own words for what is not controlled on it. If the machine has changed since that record
   (its firmware, microcode, memory, OS build, SDK or runtime), the lane's README asks for "a new dated capture beside
   the first, never an edit of it"; this run is then taken only on the lane as that new capture records it, and the
   sub-bundle names the capture it was taken on.
2. **The quiet conditions**, those Broiler.JS's run on this lane recorded (its sub-bundle's README, "When, and beside
   what"), in place of "no other build, test or run on it while the run is taken", which they keep:
   - the build servers shut down (`dotnet build-server shutdown`) before the step starts, and the step's builds run
     with node reuse, the MSBuild server and the shared compiler off, as `collect.py`'s builds already do;
   - no other Claude session and no other build, test or run on the machine while the step runs, the desktop
     applications closed but any the owner leaves open and idle, each named in the sub-bundle;
   - the run window recorded at its start and its end (the lane's capture taken again, the load, the processes that
     used the CPU over a short sample, and the .NET build processes alive) and before every lane (its start time and
     the .NET build processes then alive), retained in the sub-bundle.
3. **The forms on this lane.** Part C's population takes the native form "under the backend the host arms"; on
   `win-x64` that is `x86-64-win64`, as `decision-rule.md`'s part B names it ("x86-64-sysv on linux-x64, x86-64-win64
   on win-x64"). This is the population's own wording on another RID, not a change of it. So absolute figures on this
   lane are comparable with neither earlier measurement; part C compares arms within one run only.
4. **The arms are the revision of 2026-10-20's, checked out on the lane.**
   - **Arm B**: bundle `sjl-0-001`'s base commit `0087e8d73ae3833200fa263b68f71db15b059a9c`, rebuilt, as in both
     earlier measurements.
   - **Arm A**: the arm the revision of 2026-10-20 defines, the commit on the default branch's first-parent history
     that adds `decision-rule-revised-2026-10-20.md`, the merge that lands it, whose first parent does not carry it:
     `ebf93cd3e8fba11002f3da563b13b32ca54afe32`, the merge of Broiler-Platform/Broiler.VM#160, held to
     `aaa39fbc2030e1a2554317bea75bcbb56041c696` on every path outside `docs/` and outside any `docs/` directory but
     the four paths that revision's header names, with every check that revision's header lists taken again before
     any build. **It is not the commit that adds this file**, which changes no path outside the documents.
   - **A-again**: arm A's binaries, measured a second time under another lane name, the A/A lane.
   - **How each is checked out.** Each arm is a clean worktree of its commit, detached, checked out with the
     repository's own line endings, as Broiler.JS's arms on this lane were: this repository's `.gitattributes`, at
     both arms, stores and checks out text with LF line endings on every platform (`* text=auto eol=lf`), so a
     checkout on `win-x64` carries the bytes the commit stores, whatever the machine's `core.autocrlf` says. Each tree
     holds no untracked or ignored file but `bin/` and `obj/` directories, removed before its build, and Python's
     caches, as the revision of 2026-10-20 asks. The workloads are checked byte for byte against the blobs the bundle's
     `manifest.json` names, and the transcript program, the checker and the reference by their blobs and digest, as
     there.
5. **The collector.** `collect.py`'s `measure-revised` step, at the blob arm A holds,
   `d52165dbe009212ea277cfe57b2e15806841be9f`, was written for the programme's container: it writes into
   `measurement-revised/`, which holds a completed measurement and is refused; it reads the running build processes
   from `/proc` and refuses where it cannot; and it reads a load average the `win-x64` platform does not give. A lane
   collector, committed before the run on the branch that retains it and named by blob in the sub-bundle's manifest
   beside `collect.py`'s, runs that step with `collect.py`'s collector and helpers, and departs from it only in these,
   each marked where it departs, as Broiler.JS's `docs/evidence/sjl-3-001/collect-lane.py` did on this lane:
   - **the folder**: the sub-bundle is `measurement-win-x64-r7-5800x/`, created new; the step stops if that folder
     holds a completed measurement, deletes nothing, and keeps a stopped collection renamed so its name says it
     stopped; arm A's cleanliness check admits that folder in place of `measurement-revised/`;
   - **the manifest's lane line**: the manifest names this file as the rule that judges the run, beside the rules it
     revises, and names the lane, its record at Broiler.JS `dcab5079d5b0696d6fcc593c848a654d0ab2a107` with the
     SHA-256 of each of the record's files, the machine and the RID;
   - **the build-server flags and check**: builds run with node reuse, the MSBuild server and the shared compiler off,
     and the check that no .NET build server or build is running reads the platform's own process list where `/proc`
     does not exist; the check itself is kept, and the step still refuses when no process list can be read;
   - **the encoding**: every child's output is decoded as UTF-8, with any byte that is not UTF-8 escaped rather than
     failing the collection;
   - **the run window**: recorded as item 2 says, in place of the load average where the platform gives none.

   **It departs in nothing else.** The arms and every refusal check of the step but the folder's; the builds and the
   binaries' hashes; the manifest written and made read-only before any check, transcript or lane; every arm's first
   check of every file, in each form, before any lane; the transcripts and their check; the warm-ups, the repetitions
   and the rotation of the arms; the allowances; the answers and what each is held to; the configuration line; the
   parsing; and the arithmetic of `summary.txt` are `collect.py`'s, line for line. The collector judges nothing.
6. **The sub-bundle** is a new folder, [`measurement-win-x64-r7-5800x/`](measurement-win-x64-r7-5800x/), with its own
   `manifest.txt` and `hashes.txt`, and a README written after the step. The bundle's `manifest.json` does not hash it,
   as it hashes neither earlier sub-bundle. **`measurement/` and `measurement-revised/` are never touched**: not read
   by the step, moved or run again.
7. **This is the third and last measurement of part C.** A NOT MET here leaves clause 13 open until trigger (b) of
   decision SJL-D-23's note of 2026-10-20, as that decision's note of 2026-11-18 reads it, and no fourth measurement
   follows, under this file or under any further revision. The revision of 2026-10-20's two other readings are kept:
   a failed condition is a NOT MET, not a re-run; and a build or restore failure before any lane is not a run.
8. **When it is taken.** Once, in a local session on the lane, after the merge that lands this file on the default
   branch, from that merge or a later default-branch commit's copy of this file. Arm A does not move with that merge.

**Nothing else changed.**
- **Part C:** its population (but for the lane, item 1, and the backend that lane's host arms, item 3),
  configuration, configuration check, quantity, noise floor, comparison, attribution, admitted classes and MET line are
  the revision of 2026-10-20's, which are the original's but for when the first checks are taken.
- **The arms and their inputs:** arm B, arm A and how its sources are fixed, A-again, the workloads as committed with
  the original rule, the transcript program, the checker and the reference, the repetitions and warm-ups, and the
  allowances.
- **The condition and the transcript check**, as the revision of 2026-10-20 states them.
- **The base:** the original's paragraph "The base" still applies, with the revision of 2026-10-20's item 4.
- **The tolerance.** No tolerance is widened: the A/A lane stays the only one, as the core's rule 3 makes it.
- **Parts A and B** are not revised, and this file does not read their runs.

**Where it is recorded.** The decision is this file's header and decision SJL-D-23's note of 2026-11-18. The result,
once taken, is `measurement-win-x64-r7-5800x/` with its README, and the bundle README's part C under the three rules.
The programme's ledger records clause 13's reading. There is no new decision record, no correction entry and no
milestone revision.

Part C's block, restated with the lane, the arms and the readings as revised; every other line is the revision of
2026-10-20's:

```rule
evidence class: measurement
population: the three workloads of src/tests/forms/numbers as committed with decision-rule.md - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest: the controlled lane win-x64-r7-5800x, RID win-x64, as Broiler.JS's docs/evidence/lanes/win-x64-r7-5800x/ records it at Broiler.JS dcab5079d5b0696d6fcc593c848a654d0ab2a107, its capture.ps1, machine.txt and README.md named by the SHA-256 this file's item 1 gives, or by a later dated capture beside them; the build servers shut down before the step, no other Claude session, build, test or run on the machine while it runs, and the run window recorded at its start, at its end and before every lane
arms: B the base, rebuilt from bundle sjl-0-001's base commit 0087e8d73ae3833200fa263b68f71db15b059a9c; A the commit on the default branch's first-parent history that adds decision-rule-revised-2026-10-20.md, the merge that lands it (its first parent does not carry that file), ebf93cd3e8fba11002f3da563b13b32ca54afe32, whose product sources are those of aaa39fbc2030e1a2554317bea75bcbb56041c696 but for the four paths that file's header names, none of which a build of the command-line root reads; A-again the A arm measured a second time under another lane name, the A/A lane; each arm a clean worktree of its commit with the repository's own line endings; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane; every arm's first check of every file, in each form, taken before any lane runs
transcript check, for to-string.js only, taken before any lane and retained with its inputs and its output: each arm's binary runs docs/evidence/sjl-3-001/transcript/to-string.transcript.js, as arm A holds it (blob 9a926b538541c7d51c089902507dc10ebd79c4b8), once in each form; the check holds if and only if, in each form, (i) each arm's transcript has as many lines as to-string.control.js answers at arm B's first check in that form; (ii) the lengths of each arm's lines sum to that arm's to-string.js answer at its first check in that form; and (iii) every line of arm A's transcript is one of the texts the specification admits for the value and the conversion that line converts, as Broiler.JS's scripts/conformance/numref.py computes them - number_to_string at the conversion's radix for String and toString, to_fixed, to_exponential and to_precision - the file as Broiler.JS's commit 966140ad694570467ad21975f23047d0d6f0c1ce carries it, with SHA-256 424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7, each line's value computed as the program computes it, in IEEE 754 double arithmetic; every line on which the two arms' transcripts differ is listed with its form, its value, its conversion, both texts, and whether arm B's text is one the specification admits
condition: for to-number.js, literals.js and every control, each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; for to-string.js, each lane's answer, checked before and after every lane, equals its own arm's answer at that arm's first check, A-again's compared with arm A's, and the transcript check holds; a lane whose answer differs fails the condition
configuration check: every timed child is run with --runtime, and its effective-configuration line is identical across arms, lanes and repetitions
quantity: Q(workload, arm, form) = median over retained repetitions of the workload's wall time
noise floor: N(workload, form) = |Q(workload, A, form) - Q(workload, A-again, form)|
comparison: D(workload, form) = Q(workload, A, form) - Q(workload, B, form)      (positive: the after arm slower), against N(workload, form); one engine's after arm against the same engine's base arm, never against another engine
attribution, retained beside D and deciding nothing: E(arm) = Q(workload, arm, form) - Q(control, arm, form)
admitted difference classes: none; a measurement admits no class, and a difference within the A/A floor is below resolution rather than admitted
MET if and only if all of:
  1. every lane's condition passes, before and after, for every workload and form;
  2. every effective-configuration line is identical across arms, lanes and repetitions;
  3. for every workload and form, D <= N.
NOT MET otherwise.
readings:
  - a workload and form whose |D| <= N is below resolution, and meets clause 3;
  - clause 3 has no tolerance but the A/A lane: this rule states no percentage, so the after arm is held to the base within this machine's own noise on that workload;
  - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped;
  - a build or restore failure before any lane is not a run: its cause is fixed outside the sources and the step is started again, the failed log retained; a base that cannot be rebuilt from its commit voids the part;
  - this is the third and last measurement of part C, taken once on the lane under the owner's own dated choice of 2026-11-18 under decision SJL-D-23 at its trigger (a): a NOT MET here leaves clause 13 open until that decision's trigger (b), and no fourth measurement follows under any further revision.
```

## What it decides, and what it does not

- **What it decides.** It answers SJL-3's clause 13, Broiler.VM's measurement, as weaker evidence. The question is
  the revision of 2026-10-20's, asked on the lane: whether this family at `aaa39fb`'s product sources is slower than
  SJL-0's base, beyond the A/A floor, on number to string, string to number and the compiler's literal values, in
  both forms, on this lane.
- **What a MET and a NOT MET show**, in the revision of 2026-10-20's own words: "A NOT MET is therefore not a finding
  of a slowdown, and a MET is not a speed claim." "A MET here says this family, at those sources, is within this
  machine's noise of SJL-0's base on the three workloads; it does not say that SJL-3's cut-over alone was. A NOT MET
  cannot be laid on SJL-3's cut-over alone either."
- **What a NOT MET does.** Clause 13 stays open until trigger (b), and nothing is measured again.
- **What it does not decide:**
  - **That either earlier run was wrong.** `measurement/` stands as the reading of arm A at `49ea00e`, NOT MET on
    clauses 1 and 3, and `measurement-revised/` as the reading under the revision of 2026-10-20, NOT MET on its
    clause 3 alone. Neither is read again.
  - **That SJL-3's cut-over alone is within noise of the base,** or that it is not.
  - **A speed claim of any kind**, in either direction.
  - **Any comparison with Broiler.JS**, whose own measurement is judged by its own rule in its own repository and which
    Broiler.VM's release gate 10 forbids, its part B on this same lane among them.
  - **Anything about another machine or RID**, linux-x64 and the programme's container among them.
  - **Parts A and B**, the vectors, the closures, the packages, or any other clause of SJL-3.

## The rule it revises, quoted whole

The quote below is [`decision-rule-revised-2026-10-20.md`](decision-rule-revised-2026-10-20.md) at blob
`531380f4f5045f8fdd3cf959dfe8be73ffd74ba3` (SHA-256 `92649a05cacfd50b128f4d978eb5980606ec4befc1a6120472c275b554c884e6`),
each line prefixed with `> ` (an empty line with `>`), as that revision quotes `decision-rule.md`; the programme's
validation principle 3 asks for the quote. That revision's own quote of `decision-rule.md` is inside it, so the rule
as written is quoted here too.

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-3-001's decision rule, revised after part C's run was seen: arm A at this family's later sources, and a condition a correcting cut-over can meet
>
> **Written:** 2026-10-20, **after** part C's run, [`measurement/`](measurement/), had been seen and read NOT MET on its
> clauses 1 and 3, and after the workloads' answers and `to-string.js`'s conversion texts had been checked at both arms
> ([`revision-check/`](revision-check/README.md)). The decision it gives is therefore **weaker evidence** than the one
> [`decision-rule.md`](decision-rule.md) gives, as that rule's own "Revisions" paragraph says. **Owner:** the
> JavaScript profile owner, with the core architecture owner. **Reviewer:** none; every role is one person (EX-30), and
> this revision does not claim that anyone independent has read it. The bundle's README states part C's decision under
> both rules.
>
> **Why it was written.** Part C's first run read NOT MET on its clause 1 before any timing was read. `to-string.js`
> answers with the total length of the texts it converts, and the cut-over corrects radix digits
> ([JSD-0066](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md)
> section 4), so its answer at arm A cannot equal the base arm's. The bundle recorded this before the run, the owner
> chose on 2026-10-13 to measure under the rule as written, and the README's section 7 says clause 13 stays open "until
> a dated rule judges a condition that a correcting cut-over can meet". On 2026-10-20 the owner decided, under the
> direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review."), which changes no
> programme rule, to take that rule now, once. This file is it. It does not run part C's arm A at `49ea00e` again,
> which the rule forbids: "a failed condition is a NOT MET, not a re-run".
>
> **The owner's decisions of 2026-10-20 that this file records.** Each was taken as recommended, under the same
> direction.
> - **Write this revision now, and take its run once,** on the local machine part C's first run was taken on, with no
>   other build, test or run on it while the run is taken. The record says plainly what that run can show. Under this
>   tolerance, a change that makes no difference at all reads NOT MET more often than MET: each workload and form must
>   have D at most N, and N is only the distance between two medians of the same binaries. **A NOT MET is therefore
>   not a finding of a slowdown, and a MET is not a speed claim.**
> - **The revision changes only what the next section lists.** The noise floor, the quantity, the comparison, arm B,
>   the workloads, the repetitions, the allowances and the MET line are kept. The tolerance is not touched: the core's
>   rule 3 makes the A/A lane "the one judgement the harness makes" ([`docs/baselines.md`](../../baselines.md)
>   section 1), and a tolerance chosen after a run had been seen would be chosen to pass it.
> - **A build or restore failure before any lane is not a run.** Its cause is fixed outside the sources and the step
>   is started again, with the failed log retained. A base that cannot be rebuilt from its commit at all voids the
>   part, as the original rule's paragraph "The base" says. The next section lists this as its item 4.
>
> **Which sources arm A's binaries are built from, and what may land before the run.**
> - **Arm A.** It is the commit on the default branch's first-parent history that adds this file: the merge that lands
>   it, whose first parent does not carry it. The collector refuses any other commit, this file's own commits on its
>   branch among them, since they are not on that history.
> - **The root.** Part C builds one root, the command-line composition root,
>   `src/compositions/Broiler.VM.Composition.JavaScript.Cli/`. Its project references, followed transitively, are
>   `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Runtime`, `Broiler.VM.Profile.JavaScript`,
>   `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` and
>   `Broiler.VM.Profile.MachineCode`. It does not reference the slice compiler's composition root,
>   `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`.
> - **The comparison.** The collector holds arm A to `aaa39fb` on every path outside `docs/` and outside any `docs/`
>   directory, but four, and refuses the run before any build if any other such path differs. It counts every path
>   added, changed or removed, and takes no rename: a file moved to one of the four paths, or into a `docs/` directory,
>   still differs at the path it left. The four are the paths
>   beyond the documents that bundle [`sjl-12-001`](../sjl-12-001/decision-rule-area.md)'s rule lets change before its
>   corpus commit: `src/tests/corpus/parse-1/`, `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`,
>   `eng/compare-parse-corpus.py` and `eng/check-frontend-mapping.py`. None of them is read by a build of the root:
>   - the slice compiler is a sibling root that the root's closure does not reference;
>   - the corpus is inputs, not code;
>   - the two scripts are Python that no build runs.
>
>   So the comparison covers every path in the repository that the root's build reads. Before any build, the
>   collector also checks this:
>   - no project of the root's closure, at arm A, lies under the four paths;
>   - neither a project file of the closure nor a build file of the repository, at arm A, names one of them;
>   - no differing path under a `docs/` directory is one a build compiles or embeds (C# source, a resource, a project
>     or an MSBuild file);
>   - neither arm's tree holds an untracked or ignored file but `bin/` and `obj/` directories and Python's caches,
>     and those `bin/` and `obj/` directories are removed before each build. So a file the comparison cannot see,
>     such as a `<project>.csproj.user` that MSBuild imports, cannot reach a build.
> - **So SJL-12's VM corpus commit does not void this run, whenever it merges.** That commit changes only those four
>   paths and the documents (bundle `sjl-12-001`'s rule, "The commits, named before they exist").
>   - If it merges before this file does, arm A, the merge that lands this file, carries it, and the sources arm A's
>     binaries are built from are still `aaa39fb`'s. It did merge first, on 2026-10-20, as `865b69b` (#159).
>   - If it merges after, arm A does not carry it. The run is taken at arm A itself: the collector refuses a tree
>     whose first parent already carries this file.
> - **Any other change** to a path outside the documents before this file's commit merges makes the collector refuse.
>   SJL-12.6's ordering commit is one. This file must then be filed again, before its run, naming the new sources.
>
> ## What changed, and why
>
> Two departures, each stated on its own, and two added readings.
>
> 1. **Arm A is a later commit.**
>    - **What it is.** Under the original rule arm A is "the commit that lands SJL-3's Broiler.VM cut-over", `49ea00e`.
>      Here **arm A is the commit on the default branch's first-parent history that adds this file**, the merge that
>      lands it. Its product sources (every path
>      outside `docs/` and outside any `docs/` directory) are those of `aaa39fb`, the default branch after the move to
>      the shared stream's `0.1.0-preview.7` ([#156](https://github.com/Broiler-Platform/Broiler.VM/pull/156)), with
>      the one exception the header states: the four paths that the command-line root does not build and that bundle
>      `sjl-12-001`'s rule lets change before its corpus commit. Bundle `sjl-7-001`'s revision of 2026-10-17 moved its
>      arm A the same way.
>    - **It changes what part C judges.** Since `49ea00e` this family's product sources have changed in many merges:
>      SJL-4's numerics (among them BigInt literal values, which `literals.js` exercises), SJL-5's text, SJL-6's Date,
>      SJL-7's Unicode (among them the white-space classes `to-number.js`'s trims read), SJL-8's zones and calendars,
>      SJL-10's Temporal core, and the shared stream's re-pins from `0.1.0-preview.1` to `0.1.0-preview.7`. The
>      sub-bundle's manifest names every commit since `49ea00e` with its files outside the documents.
>    - **How a result reads.** A MET here says this family, at those sources, is within this machine's noise of SJL-0's
>      base on the three workloads; it does not say that SJL-3's cut-over alone was. A NOT MET cannot be laid on SJL-3's
>      cut-over alone either.
> 2. **`to-string.js`'s condition is its own arm's answer, held to the specification by a transcript.**
>    - **The new condition.** Under the original rule each lane's answer must equal the workload's answer at the base
>      arm. For `to-string.js`, here, each lane's answer must equal its own arm's answer at that arm's first check;
>      A-again has no binaries of its own, so its answers are compared with arm A's. A transcript check, taken before any
>      lane, must also hold.
>    - **The transcript and its check.** The transcript program,
>      [`transcript/to-string.transcript.js`](transcript/to-string.transcript.js), is `to-string.js`'s loop with each
>      conversion's text kept, in order, where the workload adds its length to a total. The check,
>      [`transcript/check.py`](transcript/check.py):
>      - holds every text of arm A to the answers the specification admits, computed in exact arithmetic by the shared
>        corpus's own reference;
>      - ties both transcripts to the workloads' answers;
>      - lists every text the cut-over and the changes after it moved.
>    - **When the first checks are taken.** So that the check is read before any lane, every arm's first check of every
>      file, in each form, is taken before any lane runs, and not, as in the first run, just before its own workload's
>      lanes. The lanes themselves, their order and their repetitions are the first run's, and every answer is checked
>      again after its workload's lanes.
>    - **Why the guard stands.** The transcript stands where the base-arm equality stood. The core's rule 5 asks that
>      "the operation must still do what its name says". For this workload that is met by its texts being the
>      specification's, not by their total length being the base's.
>    - **The other workloads.** `to-number.js`, `literals.js` and every control keep the original condition, the base
>      arm's answer. Each was checked at this family's later sources before this file was written, and each answers as
>      at the base.
>    - **Precedents.** A condition changed after a run has precedents: bundle `sjl-1-001`'s revision of 2026-10-12 (its
>      item 1) and bundle `sjl-4-001`'s of 2026-10-14 (its item 1, class (d)). Bundle `sjl-7-001`'s revision changed no
>      condition, because its answers already counted the operations that completed; this one departs further than
>      that one did.
> 3. **One reading is added: this is the second and last measurement of part C.** A NOT MET here is not followed by a
>    third measurement under a further revision. Clause 13 then stays open, and its next step is the owner's next dated
>    choice under the programme's decision SJL-D-23.
> 4. **One reading is added from the owner's decision C13-5: a build or restore failure before any lane is not a
>    run.** Its cause is fixed outside the sources (a package cache, a feed, say), and the step is started again with
>    the failed log retained.
>    - **Why it is stated.** The original rule does not say this in so many words. Its paragraph "The base" voids a
>      part "whose base cannot be rebuilt from that commit", and a failure whose cause lies outside the sources does
>      not show that. Its last reading lets "an interrupted collection" be "run again from the start, with the stopped
>      run retained beside it", and a collection stopped before any lane is one.
>    - **What it does not change.** A base that cannot be rebuilt from its commit at all still voids the part. The
>      reading is in the block below beside that sentence.
>
> **Nothing else changed.**
> - **Part C:** its population, configuration (but for when the first checks are taken, item 2), configuration
>   check, quantity, noise floor, comparison, attribution, admitted classes and MET line are the original's.
> - **The arms and their inputs:** arm B, the workloads as committed with the original rule (checked byte for byte
>   against the blobs the bundle's `manifest.json` names), the repetitions and warm-ups, and the allowances.
> - **The base:** the original's paragraph "The base" still applies. A base that cannot be rebuilt from bundle
>   `sjl-0-001`'s commit voids the part; item 4 reads a build or restore failure whose cause lies outside the sources.
> - **The tolerance.** No tolerance is widened: the A/A lane stays the only one, as the core's rule 3 makes it.
> - **Parts A and B** are not revised, and this file does not read their runs.
> - **The runs** are a sub-bundle of their own, `sjl-3-001/measurement-revised/`, taken by the collector's
>   `measure-revised` step after this file is committed. `measurement/` is not read, moved or run again.
>
> **Where it is recorded.** The decision is this file's header. The result is `measurement-revised/` with its README,
> and the bundle README's part C under both rules. The programme's ledger and the dated note under decision SJL-D-23
> record clause 13's reading. There is no new decision record, no correction entry and no milestone revision.
>
> Part C's block, restated with arm A, the transcript check, the condition and the readings as revised:
>
> ```rule
> evidence class: measurement
> population: the three workloads of src/tests/forms/numbers as committed with decision-rule.md - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest
> arms: B the base, rebuilt from bundle sjl-0-001's base commit 0087e8d73ae3833200fa263b68f71db15b059a9c; A the commit on the default branch's first-parent history that adds this file, the merge that lands it (its first parent does not carry this file), whose product sources are those of aaa39fbc2030e1a2554317bea75bcbb56041c696 but for the four paths this file's header names, none of which a build of the command-line root reads; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
> configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane; every arm's first check of every file, in each form, taken before any lane runs
> transcript check, for to-string.js only, taken before any lane and retained with its inputs and its output: each arm's binary runs docs/evidence/sjl-3-001/transcript/to-string.transcript.js, as the commit that adds this file holds it, once in each form; the check holds if and only if, in each form, (i) each arm's transcript has as many lines as to-string.control.js answers at arm B's first check in that form; (ii) the lengths of each arm's lines sum to that arm's to-string.js answer at its first check in that form; and (iii) every line of arm A's transcript is one of the texts the specification admits for the value and the conversion that line converts, as Broiler.JS's scripts/conformance/numref.py computes them - number_to_string at the conversion's radix for String and toString, to_fixed, to_exponential and to_precision - the file as Broiler.JS's commit 966140ad694570467ad21975f23047d0d6f0c1ce carries it, with SHA-256 424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7, each line's value computed as the program computes it, in IEEE 754 double arithmetic; every line on which the two arms' transcripts differ is listed with its form, its value, its conversion, both texts, and whether arm B's text is one the specification admits
> condition: for to-number.js, literals.js and every control, each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; for to-string.js, each lane's answer, checked before and after every lane, equals its own arm's answer at that arm's first check, A-again's compared with arm A's, and the transcript check holds; a lane whose answer differs fails the condition
> configuration check: every timed child is run with --runtime, and its effective-configuration line is identical across arms, lanes and repetitions
> quantity: Q(workload, arm, form) = median over retained repetitions of the workload's wall time
> noise floor: N(workload, form) = |Q(workload, A, form) - Q(workload, A-again, form)|
> comparison: D(workload, form) = Q(workload, A, form) - Q(workload, B, form)      (positive: the after arm slower), against N(workload, form); one engine's after arm against the same engine's base arm, never against another engine
> attribution, retained beside D and deciding nothing: E(arm) = Q(workload, arm, form) - Q(control, arm, form)
> admitted difference classes: none; a measurement admits no class, and a difference within the A/A floor is below resolution rather than admitted
> MET if and only if all of:
>   1. every lane's condition passes, before and after, for every workload and form;
>   2. every effective-configuration line is identical across arms, lanes and repetitions;
>   3. for every workload and form, D <= N.
> NOT MET otherwise.
> readings:
>   - a workload and form whose |D| <= N is below resolution, and meets clause 3;
>   - clause 3 has no tolerance but the A/A lane: this rule states no percentage, so the after arm is held to the base within this machine's own noise on that workload;
>   - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped;
>   - a build or restore failure before any lane is not a run: its cause is fixed outside the sources and the step is started again, the failed log retained; a base that cannot be rebuilt from its commit voids the part;
>   - this is the second and last measurement of part C: a NOT MET here is not followed by a third under a further revision, and leaves clause 13 open until the owner's next dated choice under decision SJL-D-23.
> ```
>
> ## What it decides, and what it does not
>
> - **What it decides.** It answers SJL-3's clause 13, Broiler.VM's measurement, as weaker evidence. The question is
>   whether this family at `aaa39fb`'s product sources is slower than SJL-0's base, beyond the A/A floor, on number to
>   string, string to number and the compiler's literal values, in both forms, on this machine.
> - **What it does not decide:**
>   - **That part C's first run was wrong.** `measurement/` stands as the reading of arm A at `49ea00e`, NOT MET on
>     clauses 1 and 3.
>   - **That SJL-3's cut-over alone is within noise of the base,** or that it is not.
>   - **A speed claim of any kind**, in either direction.
>   - **Any comparison with Broiler.JS**, whose own measurement is judged by its own rule in its own repository and which
>     Broiler.VM's release gate 10 forbids.
>   - **Anything about another machine or RID.**
>   - **Parts A and B**, the vectors, the closures, the packages, or any other clause of SJL-3. Those stand at
>     `49ea00e` as the README reads them.
>
> ## The rule it revises, quoted whole
>
> The quote below is [`decision-rule.md`](decision-rule.md) at blob `feb4e1ceaa3c70371ae96419510210deaeff5833`, each
> line prefixed with `> ` (an empty line with `>`), as bundle `sjl-1-001`'s and bundle `sjl-4-001`'s revisions quote
> theirs; the programme's validation principle 3 asks for the quote.
>
> > <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> > <!-- SPDX-License-Identifier: Apache-2.0 -->
> >
> > # Bundle SJL-3-001's decision rule: the pilot's test262 parity, the fuel-parity twins, and Broiler.VM's measurement
> >
> > **Written:** 2026-10-11, at milestone SJL-0 (work package SJL-0.9), before any shared library, adapter
> > or cut-over of the pilot exists, and with the workloads of part C committed in the same change.
> > **Milestone judged:** SJL-3 of [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md),
> > its Broiler.VM clauses on conformance, resource accounting and measurement. **Owner:** the JavaScript
> > profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule names is
> > held by one person (EX-30), and this rule does not claim that anyone independent has read it.
> >
> > **What this file is.** Three predeclared decisions, one per part, each of one evidence class, in the
> > templates T1, T7 and T5 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
> > **Each part decides alone**, and no part's runs are read by another. Part C is the only measurement:
> > [roadmap.gates.md section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
> > rule 8 asks for one evidence class per bundle that carries a figure, so SJL-3 retains part C's runs in
> > a sub-bundle of their own, `sjl-3-001/measurement/`, with its own README and manifest. The Broiler.JS
> > repository commits its own rule for its own engine, and the library-level reference-arm rule lives in
> > the home; neither is read here. **It holds no figure.** Where a count or an allowance matters, it
> > names the file that fixes it.
> >
> > **The base.** Every part compares an after arm with the base that bundle
> > [`sjl-0-001`](../sjl-0-001/README.md) names: its commit, taken before any engine changed. A part
> > whose base is not retained in that bundle, or whose base cannot be rebuilt from that commit, is void
> > and is not judged.
> >
> > ## Part A - test262 parity, in both forms (template T1)
> >
> > ```rule
> > evidence class: conformance parity (not a measurement)
> > population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
> > configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are bundle sjl-0-001's, at its base commit; the after runs are taken at the commit that lands SJL-3's Broiler.VM cut-over, named in bundle sjl-3-001, on the same RID
> > comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
> > admitted difference classes:
> >   C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector or specification clause that names the correction
> >   C2 charge model by decision - an exhaustion verdict moves because a dated decision changed the charging units of an operation the pilot moved; each one listed with the decision
> >   C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation, and the area's vectors classify it; each one listed with its vector
> >   C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits and the re-run is retained, and a row that still differs outside C1 to C3 is a regression
> > every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after other than under C2, and an internal defect on either side
> > MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1 to C4 and is listed, and the native pair meets the same conditions.
> > NOT MET otherwise.
> > ```
> >
> > **How it is read.** The bundle retains each comparison's output, which lists every differing row with
> > its class or as unclassified, and a table naming each C1, C2 and C3 row with its reason. The floor
> > verdicts the runner prints are retained and do not decide part A: a floor is a ratchet over totals,
> > and this part judges variants. A row the comparison cannot classify is a regression even when the
> > totals improve.
> >
> > ## Part B - the fuel-parity twins (template T7)
> >
> > ```rule
> > evidence class: resource accounting (not a measurement)
> > population: every *.small.js twin in src/tests/forms/wide as committed at bundle sjl-0-001's base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
> > configuration: the command-line composition root built in Release; at the base, each twin's smallest completing fuel allowance bisected under the bytecode form, as bundle sjl-0-001's fuel-parity.log retains it; the wall allowance ample and the same at both commits
> > ceilings tried: for each twin, its base figure and one less
> > comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after
> > admitted difference classes:
> >   C2 charge model by decision - a twin's smallest completing allowance moves because a dated decision changed the charging units of an operation the pilot moved; the twin is then bisected again after, both figures listed with the decision, and it must complete at its new figure and refuse with AllowanceExhausted on Fuel at one less in both forms
> > MET if and only if every twin, in both forms, after as at the base, completes at its base figure and refuses with AllowanceExhausted on Fuel at one less, or moved under C2 and meets C2's own condition.
> > NOT MET otherwise.
> > ```
> >
> > **How it is read.** One verdict at every ceiling tried, at the base and after, in both forms: a twin
> > that completes one unit sooner or later, in either form, is a charge that moved, and only a dated
> > decision admits that.
> >
> > ## Part C - Broiler.VM's measurement (template T5)
> >
> > ```rule
> > evidence class: measurement
> > population: the three workloads of src/tests/forms/numbers as committed with this rule - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest
> > arms: B the base, rebuilt from bundle sjl-0-001's base commit; A the commit that lands SJL-3's Broiler.VM cut-over; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
> > configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane
> > condition: each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; a lane whose answer differs fails the condition
> > configuration check: every timed child is run with --runtime, and its effective-configuration line is identical across arms, lanes and repetitions
> > quantity: Q(workload, arm, form) = median over retained repetitions of the workload's wall time
> > noise floor: N(workload, form) = |Q(workload, A, form) - Q(workload, A-again, form)|
> > comparison: D(workload, form) = Q(workload, A, form) - Q(workload, B, form)      (positive: the after arm slower), against N(workload, form); one engine's after arm against the same engine's base arm, never against another engine
> > attribution, retained beside D and deciding nothing: E(arm) = Q(workload, arm, form) - Q(control, arm, form)
> > admitted difference classes: none; a measurement admits no class, and a difference within the A/A floor is below resolution rather than admitted
> > MET if and only if all of:
> >   1. every lane's condition passes, before and after, for every workload and form;
> >   2. every effective-configuration line is identical across arms, lanes and repetitions;
> >   3. for every workload and form, D <= N.
> > NOT MET otherwise.
> > readings:
> >   - a workload and form whose |D| <= N is below resolution, and meets clause 3;
> >   - clause 3 has no tolerance but the A/A lane: this rule states no percentage, so the after arm is held to the base within this machine's own noise on that workload;
> >   - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped.
> > ```
> >
> > **The base reading at SJL-0.** Bundle `sjl-0-001` measures arm B alone, under this part's harness:
> > for each workload and form, B, its control and an A/A lane B-again, interleaved. **Its one decision
> > is not this part's**: each workload's attribution E(B) is recorded as the base's figure when |E(B)|
> > exceeds |Q(B) - Q(B-again)|, and as below resolution otherwise. It exists so the workloads, the
> > harness and the base's noise floor exist before any code moves, and it decides nothing about SJL-3,
> > which rebuilds B and runs it interleaved with A.
> >
> > **What part C does not decide.** A speed claim of any kind, in either direction. A comparison with
> > Broiler.JS, whose own measurement is judged by its own rule in its own repository; Broiler.VM's
> > release gate 10 forbids that comparison. Anything about a RID or a machine other than the one named.
> >
> > ## Revisions
> >
> > A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
> > this one whole and saying it was written afterwards; its decision is then weaker evidence, as the
> > core's bundle `ubc-4-006` records for its own. **SJL-3.2 confirms this file unchanged before any code
> > of SJL-3.3 to SJL-3.5 exists**, and a later bundle shows the ancestry with
> > `git merge-base --is-ancestor`.
> >
> > ## What this file does not decide
> >
> > - **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home.
> > - **Broiler.JS's test262 parity or measurement.** Each is judged by Broiler.JS's own rule.
> > - **The differential cross-check.** Template T8 cannot meet a conformance clause on its own, and the
> >   milestone retains its report beside these parts without a rule here.
> > - **Vectors, closures, packages and the `I` rules.** Each is a clause of SJL-3 judged by its own
> >   mechanism, not by a comparison this file states.
