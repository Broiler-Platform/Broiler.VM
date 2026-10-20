<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-3-001's decision rule, revised after part C's run was seen: arm A at this family's later sources, and a condition a correcting cut-over can meet

**Written:** 2026-10-20, **after** part C's run, [`measurement/`](measurement/), had been seen and read NOT MET on its
clauses 1 and 3, and after the workloads' answers and `to-string.js`'s conversion texts had been checked at both arms
([`revision-check/`](revision-check/README.md)). The decision it gives is therefore **weaker evidence** than the one
[`decision-rule.md`](decision-rule.md) gives, as that rule's own "Revisions" paragraph says. **Owner:** the
JavaScript profile owner, with the core architecture owner. **Reviewer:** none; every role is one person (EX-30), and
this revision does not claim that anyone independent has read it. The bundle's README states part C's decision under
both rules.

**Why it was written.** Part C's first run read NOT MET on its clause 1 before any timing was read. `to-string.js`
answers with the total length of the texts it converts, and the cut-over corrects radix digits
([JSD-0066](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md)
section 4), so its answer at arm A cannot equal the base arm's. The bundle recorded this before the run, the owner
chose on 2026-10-13 to measure under the rule as written, and the README's section 7 says clause 13 stays open "until
a dated rule judges a condition that a correcting cut-over can meet". On 2026-10-20 the owner decided, under the
direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review."), which changes no
programme rule, to take that rule now, once. This file is it. It does not run part C's arm A at `49ea00e` again,
which the rule forbids: "a failed condition is a NOT MET, not a re-run".

**The owner's decisions of 2026-10-20 that this file records.** Both were taken as recommended, under the same
direction.
- **Write this revision now, and take its run once,** on the local machine part C's first run was taken on, with no
  other build, test or run on it while the run is taken. The record says plainly what that run can show. Under this
  tolerance, a change that makes no difference at all reads NOT MET more often than MET: each workload and form must
  have D at most N, and N is only the distance between two medians of the same binaries. **A NOT MET is therefore
  not a finding of a slowdown, and a MET is not a speed claim.**
- **The revision changes only what the next section lists.** The noise floor, the quantity, the comparison, arm B,
  the workloads, the repetitions, the allowances and the MET line are kept. The tolerance is not touched: the core's
  rule 3 makes the A/A lane "the one judgement the harness makes" ([`docs/baselines.md`](../../baselines.md)
  section 1), and a tolerance chosen after a run had been seen would be chosen to pass it.

**Which sources arm A's binaries are built from, and what may land before the run.**
- **The root.** Part C builds one root, the command-line composition root,
  `src/compositions/Broiler.VM.Composition.JavaScript.Cli/`. Its project references, followed transitively, are
  `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Runtime`, `Broiler.VM.Profile.JavaScript`,
  `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` and
  `Broiler.VM.Profile.MachineCode`. It does not reference the slice compiler's composition root,
  `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`.
- **The comparison.** The collector holds arm A to `aaa39fb` on every path outside `docs/` and outside any `docs/`
  directory, but four, and refuses the run before any build if any other such path differs. The four are the paths
  beyond the documents that bundle [`sjl-12-001`](../sjl-12-001/decision-rule-area.md)'s rule lets change before its
  corpus commit: `src/tests/corpus/parse-1/`, `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`,
  `eng/compare-parse-corpus.py` and `eng/check-frontend-mapping.py`. None of them is read by a build of the root:
  - the slice compiler is a sibling root that the root's closure does not reference;
  - the corpus is inputs, not code;
  - the two scripts are Python that no build runs.

  So the comparison covers every path the root's build reads. Before any build, the collector also checks this at
  arm A:
  - no project of the root's closure lies under the four paths;
  - neither a project file of the closure nor a build file of the repository names one of them;
  - no differing path under a `docs/` directory is one a build compiles or embeds (C# source, a resource, a project
    or an MSBuild file).
- **So SJL-12's VM corpus commit does not void this run, whenever it merges.** That commit changes only those four
  paths and the documents (bundle `sjl-12-001`'s rule, "The commits, named before they exist").
  - If it merges before this file's commit, arm A carries it, and the sources arm A's binaries are built from are
    still `aaa39fb`'s.
  - If it merges after, arm A does not carry it. The run is taken at arm A itself: the collector refuses a tree
    whose first parent already carries this file.
- **Any other change** to a path outside the documents before this file's commit merges makes the collector refuse.
  SJL-12.6's ordering commit is one. This file must then be filed again, before its run, naming the new sources.

## What changed, and why

Two departures, each stated on its own, and one added reading.

1. **Arm A is a later commit.**
   - **What it is.** Under the original rule arm A is "the commit that lands SJL-3's Broiler.VM cut-over", `49ea00e`.
     Here **arm A is the commit that adds this file** to the default branch's history. Its product sources (every path
     outside `docs/` and outside any `docs/` directory) are those of `aaa39fb`, the default branch after the move to
     the shared stream's `0.1.0-preview.7` ([#156](https://github.com/Broiler-Platform/Broiler.VM/pull/156)), with
     the one exception the header states: the four paths that the command-line root does not build and that bundle
     `sjl-12-001`'s rule lets change before its corpus commit. Bundle `sjl-7-001`'s revision of 2026-10-17 moved its
     arm A the same way.
   - **It changes what part C judges.** Since `49ea00e` this family's product sources have changed in many merges:
     SJL-4's numerics (among them BigInt literal values, which `literals.js` exercises), SJL-5's text, SJL-6's Date,
     SJL-7's Unicode (among them the white-space classes `to-number.js`'s trims read), SJL-8's zones and calendars,
     SJL-10's Temporal core, and the shared stream's re-pins from `0.1.0-preview.1` to `0.1.0-preview.7`. The
     sub-bundle's manifest names every commit since `49ea00e` with its files outside the documents.
   - **How a result reads.** A MET here says this family, at those sources, is within this machine's noise of SJL-0's
     base on the three workloads; it does not say that SJL-3's cut-over alone was. A NOT MET cannot be laid on SJL-3's
     cut-over alone either.
2. **`to-string.js`'s condition is its own arm's answer, held to the specification by a transcript.**
   - **The new condition.** Under the original rule each lane's answer must equal the workload's answer at the base
     arm. For `to-string.js`, here, each lane's answer must equal its own arm's answer at that arm's first check;
     A-again has no binaries of its own, so its answers are compared with arm A's. A transcript check, taken before any
     lane, must also hold.
   - **The transcript and its check.** The transcript program,
     [`transcript/to-string.transcript.js`](transcript/to-string.transcript.js), is `to-string.js`'s loop with each
     conversion's text kept, in order, where the workload adds its length to a total. The check,
     [`transcript/check.py`](transcript/check.py):
     - holds every text of arm A to the answers the specification admits, computed in exact arithmetic by the shared
       corpus's own reference;
     - ties both transcripts to the workloads' answers;
     - lists every text the cut-over and the changes after it moved.
   - **When the first checks are taken.** So that the check is read before any lane, every arm's first check of every
     file, in each form, is taken before any lane runs, and not, as in the first run, just before its own workload's
     lanes. The lanes themselves, their order and their repetitions are the first run's, and every answer is checked
     again after its workload's lanes.
   - **Why the guard stands.** The transcript stands where the base-arm equality stood. The core's rule 5 asks that
     "the operation must still do what its name says". For this workload that is met by its texts being the
     specification's, not by their total length being the base's.
   - **The other workloads.** `to-number.js`, `literals.js` and every control keep the original condition, the base
     arm's answer. Each was checked at this family's later sources before this file was written, and each answers as
     at the base.
   - **Precedents.** A condition changed after a run has precedents: bundle `sjl-1-001`'s revision of 2026-10-12 (its
     item 1) and bundle `sjl-4-001`'s of 2026-10-14 (its item 1, class (d)). Bundle `sjl-7-001`'s revision changed no
     condition, because its answers already counted the operations that completed; this one departs further than
     that one did.
3. **One reading is added: this is the second and last measurement of part C.** A NOT MET here is not followed by a
   third measurement under a further revision. Clause 13 then stays open, and its next step is the owner's next dated
   choice under the programme's decision SJL-D-23.

**Nothing else changed.**
- **Part C:** its population, configuration, configuration check, quantity, noise floor, comparison, attribution,
  admitted classes and MET line are the original's.
- **The arms and their inputs:** arm B, the workloads as committed with the original rule (checked byte for byte
  against the blobs the bundle's `manifest.json` names), the repetitions and warm-ups, and the allowances.
- **The base:** the original's paragraph "The base" still applies. A base that cannot be rebuilt from bundle
  `sjl-0-001`'s commit voids the part. A build or restore failure before any lane is not a run: its cause is fixed
  outside the sources, and the step is started again with the failed log retained.
- **The tolerance.** No tolerance is widened: the A/A lane stays the only one, as the core's rule 3 makes it.
- **Parts A and B** are not revised, and this file does not read their runs.
- **The runs** are a sub-bundle of their own, `sjl-3-001/measurement-revised/`, taken by the collector's
  `measure-revised` step after this file is committed. `measurement/` is not read, moved or run again.

**Where it is recorded.** The decision is this file's header. The result is `measurement-revised/` with its README,
and the bundle README's part C under both rules. The programme's ledger and the dated note under decision SJL-D-23
record clause 13's reading. There is no new decision record, no correction entry and no milestone revision.

Part C's block, restated with arm A, the transcript check, the condition and the readings as revised:

```rule
evidence class: measurement
population: the three workloads of src/tests/forms/numbers as committed with decision-rule.md - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest
arms: B the base, rebuilt from bundle sjl-0-001's base commit 0087e8d73ae3833200fa263b68f71db15b059a9c; A the commit that adds this file to the default branch's history, whose product sources are those of aaa39fbc2030e1a2554317bea75bcbb56041c696 but for the four paths this file's header names, none of which a build of the command-line root reads; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane; every arm's first check of every file, in each form, taken before any lane runs
transcript check, for to-string.js only, taken before any lane and retained with its inputs and its output: each arm's binary runs docs/evidence/sjl-3-001/transcript/to-string.transcript.js, as the commit that adds this file holds it, once in each form; the check holds if and only if, in each form, (i) each arm's transcript has as many lines as to-string.control.js answers at arm B's first check in that form; (ii) the lengths of each arm's lines sum to that arm's to-string.js answer at its first check in that form; and (iii) every line of arm A's transcript is one of the texts the specification admits for the value and the conversion that line converts, as Broiler.JS's scripts/conformance/numref.py computes them - number_to_string at the conversion's radix for String and toString, to_fixed, to_exponential and to_precision - the file as Broiler.JS's commit 966140ad694570467ad21975f23047d0d6f0c1ce carries it, with SHA-256 424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7, each line's value computed as the program computes it, in IEEE 754 double arithmetic; every line on which the two arms' transcripts differ is listed with its form, its value, its conversion, both texts, and whether arm B's text is one the specification admits
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
  - this is the second and last measurement of part C: a NOT MET here is not followed by a third under a further revision, and leaves clause 13 open until the owner's next dated choice under decision SJL-D-23.
```

## What it decides, and what it does not

- **What it decides.** It answers SJL-3's clause 13, Broiler.VM's measurement, as weaker evidence. The question is
  whether this family at `aaa39fb`'s product sources is slower than SJL-0's base, beyond the A/A floor, on number to
  string, string to number and the compiler's literal values, in both forms, on this machine.
- **What it does not decide:**
  - **That part C's first run was wrong.** `measurement/` stands as the reading of arm A at `49ea00e`, NOT MET on
    clauses 1 and 3.
  - **That SJL-3's cut-over alone is within noise of the base,** or that it is not.
  - **A speed claim of any kind**, in either direction.
  - **Any comparison with Broiler.JS**, whose own measurement is judged by its own rule in its own repository and which
    Broiler.VM's release gate 10 forbids.
  - **Anything about another machine or RID.**
  - **Parts A and B**, the vectors, the closures, the packages, or any other clause of SJL-3. Those stand at
    `49ea00e` as the README reads them.

## The rule it revises, quoted whole

The quote below is [`decision-rule.md`](decision-rule.md) at blob `feb4e1ceaa3c70371ae96419510210deaeff5833`, each
line prefixed with `> ` (an empty line with `>`), as bundle `sjl-1-001`'s and bundle `sjl-4-001`'s revisions quote
theirs; the programme's validation principle 3 asks for the quote.

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-3-001's decision rule: the pilot's test262 parity, the fuel-parity twins, and Broiler.VM's measurement
>
> **Written:** 2026-10-11, at milestone SJL-0 (work package SJL-0.9), before any shared library, adapter
> or cut-over of the pilot exists, and with the workloads of part C committed in the same change.
> **Milestone judged:** SJL-3 of [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md),
> its Broiler.VM clauses on conformance, resource accounting and measurement. **Owner:** the JavaScript
> profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule names is
> held by one person (EX-30), and this rule does not claim that anyone independent has read it.
>
> **What this file is.** Three predeclared decisions, one per part, each of one evidence class, in the
> templates T1, T7 and T5 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
> **Each part decides alone**, and no part's runs are read by another. Part C is the only measurement:
> [roadmap.gates.md section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
> rule 8 asks for one evidence class per bundle that carries a figure, so SJL-3 retains part C's runs in
> a sub-bundle of their own, `sjl-3-001/measurement/`, with its own README and manifest. The Broiler.JS
> repository commits its own rule for its own engine, and the library-level reference-arm rule lives in
> the home; neither is read here. **It holds no figure.** Where a count or an allowance matters, it
> names the file that fixes it.
>
> **The base.** Every part compares an after arm with the base that bundle
> [`sjl-0-001`](../sjl-0-001/README.md) names: its commit, taken before any engine changed. A part
> whose base is not retained in that bundle, or whose base cannot be rebuilt from that commit, is void
> and is not judged.
>
> ## Part A - test262 parity, in both forms (template T1)
>
> ```rule
> evidence class: conformance parity (not a measurement)
> population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
> configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are bundle sjl-0-001's, at its base commit; the after runs are taken at the commit that lands SJL-3's Broiler.VM cut-over, named in bundle sjl-3-001, on the same RID
> comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
> admitted difference classes:
>   C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector or specification clause that names the correction
>   C2 charge model by decision - an exhaustion verdict moves because a dated decision changed the charging units of an operation the pilot moved; each one listed with the decision
>   C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation, and the area's vectors classify it; each one listed with its vector
>   C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits and the re-run is retained, and a row that still differs outside C1 to C3 is a regression
> every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after other than under C2, and an internal defect on either side
> MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1 to C4 and is listed, and the native pair meets the same conditions.
> NOT MET otherwise.
> ```
>
> **How it is read.** The bundle retains each comparison's output, which lists every differing row with
> its class or as unclassified, and a table naming each C1, C2 and C3 row with its reason. The floor
> verdicts the runner prints are retained and do not decide part A: a floor is a ratchet over totals,
> and this part judges variants. A row the comparison cannot classify is a regression even when the
> totals improve.
>
> ## Part B - the fuel-parity twins (template T7)
>
> ```rule
> evidence class: resource accounting (not a measurement)
> population: every *.small.js twin in src/tests/forms/wide as committed at bundle sjl-0-001's base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
> configuration: the command-line composition root built in Release; at the base, each twin's smallest completing fuel allowance bisected under the bytecode form, as bundle sjl-0-001's fuel-parity.log retains it; the wall allowance ample and the same at both commits
> ceilings tried: for each twin, its base figure and one less
> comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after
> admitted difference classes:
>   C2 charge model by decision - a twin's smallest completing allowance moves because a dated decision changed the charging units of an operation the pilot moved; the twin is then bisected again after, both figures listed with the decision, and it must complete at its new figure and refuse with AllowanceExhausted on Fuel at one less in both forms
> MET if and only if every twin, in both forms, after as at the base, completes at its base figure and refuses with AllowanceExhausted on Fuel at one less, or moved under C2 and meets C2's own condition.
> NOT MET otherwise.
> ```
>
> **How it is read.** One verdict at every ceiling tried, at the base and after, in both forms: a twin
> that completes one unit sooner or later, in either form, is a charge that moved, and only a dated
> decision admits that.
>
> ## Part C - Broiler.VM's measurement (template T5)
>
> ```rule
> evidence class: measurement
> population: the three workloads of src/tests/forms/numbers as committed with this rule - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest
> arms: B the base, rebuilt from bundle sjl-0-001's base commit; A the commit that lands SJL-3's Broiler.VM cut-over; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
> configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane
> condition: each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; a lane whose answer differs fails the condition
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
>   - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped.
> ```
>
> **The base reading at SJL-0.** Bundle `sjl-0-001` measures arm B alone, under this part's harness:
> for each workload and form, B, its control and an A/A lane B-again, interleaved. **Its one decision
> is not this part's**: each workload's attribution E(B) is recorded as the base's figure when |E(B)|
> exceeds |Q(B) - Q(B-again)|, and as below resolution otherwise. It exists so the workloads, the
> harness and the base's noise floor exist before any code moves, and it decides nothing about SJL-3,
> which rebuilds B and runs it interleaved with A.
>
> **What part C does not decide.** A speed claim of any kind, in either direction. A comparison with
> Broiler.JS, whose own measurement is judged by its own rule in its own repository; Broiler.VM's
> release gate 10 forbids that comparison. Anything about a RID or a machine other than the one named.
>
> ## Revisions
>
> A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
> this one whole and saying it was written afterwards; its decision is then weaker evidence, as the
> core's bundle `ubc-4-006` records for its own. **SJL-3.2 confirms this file unchanged before any code
> of SJL-3.3 to SJL-3.5 exists**, and a later bundle shows the ancestry with
> `git merge-base --is-ancestor`.
>
> ## What this file does not decide
>
> - **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home.
> - **Broiler.JS's test262 parity or measurement.** Each is judged by Broiler.JS's own rule.
> - **The differential cross-check.** Template T8 cannot meet a conformance clause on its own, and the
>   milestone retains its report beside these parts without a rule here.
> - **Vectors, closures, packages and the `I` rules.** Each is a clause of SJL-3 judged by its own
>   mechanism, not by a comparison this file states.
