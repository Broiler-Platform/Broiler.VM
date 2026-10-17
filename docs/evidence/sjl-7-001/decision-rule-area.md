<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-7-001's area rule: the Unicode area's test262 parity on hosted runners, its fuel-parity twins and probes, and this family's measurement

**Written:** 2026-10-16, at milestone SJL-7 (work package SJL-7.1), with the area verdict
[JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md).
It was written before any owner member, adapter or cut-over of the area exists. Part B's probes and part C's
workloads (`src/tests/forms/unicode`) are committed in the same change.

**Milestone judged:** SJL-7 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-7-unicode.md): its
Broiler.VM clause 11, and its evidence item "each engine's measurement for `normalize`, casing and identifier
scanning under predeclared rules", which answers JSD-0031's fifth reason.

**Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:** none. Every role this
rule names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class, in the templates
T1, T7 and T5 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
- **Each part decides alone**, and no part's runs are read by another.
- **Part C is the only measurement.** The profile's
  [roadmap.gates.md section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
  rule 8 asks for one evidence class per bundle that carries a figure, so part C's runs are retained in a
  sub-bundle of their own, `sjl-7-001/measurement/`, with its own README and manifest, as bundle `sjl-3-001`
  did.
- The Broiler.JS repository commits its own rule for its own engine, and the owner's rules (its test files,
  its generation and the library-level reference arms) live in Broiler.Unicode's bundle of the same name.
  Neither is read here.
- **It holds no figure.** Where a count or an allowance matters, it names the file that fixes it, or states it
  as a parameter of the rule.

**What earlier bundles taught this rule, before any run of SJL-7.**
- **Part A follows bundle `sjl-6-001`'s area rule:** the hosted workflow for both runs, and a row whose
  exhausted dimension moves on the wall clock re-run on the programme's local machine. That machine is the one
  bundles `sjl-5-001` and `sjl-6-001` were taken on, named in the bundle by its processor count, its SDK and its
  operating system, otherwise idle.
- **Part C follows bundle `sjl-3-001`'s part C, with one change.** That part failed its condition because a
  correction moved a workload's answer, so **each workload here answers with the count of the operations that
  completed**, which no correction changes.

**The base.** SJL-7's base is this family after SJL-6, **the commit that carries this rule**. Its product
sources are the default branch's after SJL-6's merges, `24632b1`'s.
- Part A's base runs are taken by the workflow at that commit, named by all forty digits, and retained in
  `sjl-7-001/base/`.
- Part B's base figures are taken locally from a clean checkout of the same commit.
- Part C's base arm is built from that commit.

A part whose base was taken at any other commit is void and is not judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to this file's commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the commit that carries this file, and the after runs at the commit that lands SJL-7.4's Broiler.VM cut-over, named in bundle sjl-7-001; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the owner's implementation follows the specification where this family did not; each one listed with its path, its variant and the vector, the specification clause or the row of JSD-0074's correspondence that names the correction (C12, N6)
  C2 charge model by decision - a variant under test/intl402/Segmenter/ whose exhaustion verdict moves because JSD-0074's decision 4 changed segmentation's charging units; each one listed with the decision; nowhere else
  C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation (the locale-sensitive result of toLocaleUpperCase and toLocaleLowerCase without Intl, a segmenter's tailoring), and the area's vectors classify it; each one listed with its vector
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's, the machine otherwise idle, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2 outside test/intl402/Segmenter/: JSD-0074 keeps this family's charging units for normalization and casing, and the lookups and folding charge nothing, so an exhaustion verdict that moves on fuel elsewhere is a regression, not a class
no C5: the pin does not move, and the owner's tables are regenerated from this family's own pin
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1 to C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-6-001`'s part A.
- The bundle retains both dispatches' artifacts and each comparison's output, which lists every differing row
  with its class or as unclassified.
- A table names each C1, C2 and C3 row with its reason, and each (c') and (e) row with its re-runs.
- The floor verdicts the harness prints are retained and do not decide part A.
- The native floor was set in the `x86-64-win64` form and the hosted runners are `x86-64-sysv`, so each native
  run exits 4, which completeness admits.
- A row the comparison cannot classify is a regression even when the totals improve.
- **The hosted runners' image may differ between the two dispatches.** The bundle records each runner's image,
  and a difference it causes is judged like any other.

## Part B - the fuel-parity twins and probes (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, and every *.small.js probe in src/tests/forms/unicode, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine this file's header names; at the base commit, each twin's and probe's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-7-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin and probe, its base figure and one less
comparison: per twin or probe and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; JSD-0074 keeps this family's charging units for normalization and casing, the lookups and folding charge nothing, and segmentation, whose units move, is outside the command-line root's manifest, so a moved figure is a defect of an owner member or of an adapter, not a class
MET if and only if every twin and probe, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-6-001`'s part B: one verdict at every ceiling tried, at the base and after,
in both forms, and nothing admits a charge that moved.
- The probes' answers are chosen so that no correction of SJL-7 changes them (their README says how). An answer
  that changes is read as a moved verdict.
- The wide twins show that the cut-over moved no charge outside the area.
- `Intl`, and with it `Intl.Segmenter` and the locale's effect on casing, is outside the command-line root's
  manifest, so their exhaustion verdicts are part A's.

## Part C - this family's measurement (template T5)

```rule
evidence class: measurement
population: the three workloads of src/tests/forms/unicode as committed with this rule - normalize.js (normalize in its four forms), casing.js (the default and two language-sensitive case mappings) and identifiers.js (the compiler's identifier scanning outside ASCII, through the Function constructor) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; the local machine this file's header names, and one RID, both named in the sub-bundle's manifest
arms: B the base, built from the commit that carries this rule; A the commit that lands SJL-7.4's Broiler.VM cut-over; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane
condition: each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; a lane whose answer differs fails the condition; each answer counts the operations that completed, so no correction changes it
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
  - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped.
```

**What part C decides, and what it does not.**
- **What it decides:** whether this family's normalization, casing and identifier scanning are slower after
  the cut-over than before, beyond the A/A floor. That is JSD-0031's fifth reason, "its runtime cost", for this
  engine.
- **What it does not decide:** a speed claim of any kind, in either direction; a comparison with Broiler.JS,
  whose own measurement is judged by its own rule in its own repository, and which Broiler.VM's release gate 10
  forbids; anything about a RID or a machine other than the one named.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one
whole and says it was written afterwards; its decision is then weaker evidence. **Before SJL-7.2's code lands,
a later bundle shows this file unchanged**, as SJL-3.2, SJL-4, SJL-5 and SJL-6 did for theirs.

## What this file does not decide

- **The owner's clauses**: Unicode's test files, generation byte for byte, and the library-level reference
  arms. Their rules live in Broiler.Unicode's bundle `sjl-7-001`.
- **Broiler.JS's test262 parity and measurement.** Broiler.JS's own rule judges them.
- **The vectors, the closures, the packages, rule I4's data agreement and N22's and N28's regeneration.** Each
  is a clause of SJL-7 judged by its own mechanism (clauses 9, 10, 12 and 13), not by a comparison this file
  states.
