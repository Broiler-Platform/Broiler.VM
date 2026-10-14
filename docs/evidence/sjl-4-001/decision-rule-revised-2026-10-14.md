<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-4-001's decision rule, revised after part A's runs were seen: an exhaustion race

**Written:** 2026-10-14, at milestone SJL-4.6, **after** part A's base runs, both after takes, their
comparisons, their re-runs of class C4 (c') and a diagnostic of two rows had all been seen, so the
decision it gives is **weaker evidence** than the one [`decision-rule.md`](decision-rule.md) gives, as
that rule's own "Revisions" paragraph says. It was written at the owner's direction, given the same
day after the bundle had reported part A NOT MET under the original rule. **Owner:** the JavaScript
profile owner, with the core architecture owner. **Reviewer:** none; every role is one person (EX-30),
and this revision does not claim that anyone independent has read it. The bundle's README states part
A's decision under both rules.

## What changed, and why

1. **Class C4 gains (d), an exhaustion race.** Under the original rule a (c') row - a variant one of
   whose runs exhausted the wall clock - is run again alone at both commits, and "a row that still
   differs outside C1 and C3 is a regression". The native pair of the after take at `f617962` has two
   such rows, `test/built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` [strict] and
   `test/staging/sm/Date/dst-offset-caching-6-of-8.js` [strict]. Each is exhausted in every run at
   both commits. What differs is the allowance it reaches first: the fuel allowance, 100,000,000, or
   the wall clock, 5,000 ms, which a row that needs about both reaches in either order from one run
   to the next. A diagnostic, [`race-diagnostic.py`](race-diagnostic.py), ran the two paths alone ten
   times at each commit and found both commits reaching both allowances on both rows. The original
   rule has no class for that, so it reads a race the cut-over did not cause as a regression.
   **Class (d) admits a (c') row whose re-runs differ only when ten more runs alone at each commit
   show it is that race**: exhausted in every run at both commits, and each commit exhausting both
   allowances at least once. A row that completes in any run is not admitted, and neither is a row
   that exhausts one allowance only, in every run at either commit: a commit that always reaches fuel
   first, against one that always reaches the wall clock first, is a charge or a speed that moved, and
   remains a regression.
2. **The ten repetitions are a step of the bundle's collector, taken afresh.** The diagnostic's runs
   are what prompted this revision, so (d) does not read them. The collector's `races` step takes the
   ten runs per commit after this file is committed, on an otherwise idle machine, and the README
   reads that step's output alone.
3. **How the exhaustion verdicts are read.** The milestone asks that a row exhausted on fuel at the
   base be exhausted on fuel after. A row admitted under (d) is exhausted on both allowances at both
   commits, so its exhaustion verdict does not move, and the paragraph "How it is read" of the
   original rule applies to it in that sense.

**Nothing else changed:** part A's population, configuration, comparison, classes C1 and C3, the
other classes of C4 and its MET line, and the whole of part B, are the original's. Part A's block,
restated with class C4 as revised:

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are taken at the base commit as this file's header says and retained in sjl-4-001/base/; the after runs are taken at the commit that lands SJL-4's Broiler.VM cut-over, named in bundle sjl-4-001; both on the same RID, each on an otherwise idle machine
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector, the specification clause or the row of JSD-0067's X2 that names the correction
  C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation (an implementation-approximated Math function), and the area's vectors classify it; each one listed with its vector
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits and the re-run is retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (d)
  C4 (d) an exhaustion race - a (c') row whose re-runs differ is run alone ten more times at each commit, alternating which commit goes first, each run one process of the conformance root over the row's path, under the part's manifest, form and allowances, on an otherwise idle machine, and every run is retained; the row is admitted if and only if, in every run of it at both commits - the whole run, the re-run and the ten repetitions - it is exhausted, on fuel or on the wall clock, and each commit's runs include at least one exhaustion on fuel and at least one on the wall clock; a row that passes, fails, is unsupported or is skipped in any of those runs, or that is exhausted on one dimension only in every run at either commit, is a regression
no C2: JSD-0067's decision 3 keeps this family's charging units, so an exhaustion verdict that moves on fuel is a regression, not a class
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1, C3 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

## The rule it revises, quoted whole

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-4-001's decision rule: the numerics area's test262 parity, and its fuel-parity twins and exhaustion verdicts
>
> **Written:** 2026-10-13, at milestone SJL-4 (work package SJL-4.1), with the area verdict
> [JSD-0067](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0067-numerics-move-to-the-shared-algorithms-library.md),
> before any shared kernel, adapter or cut-over of the area exists, and with part B's probes
> (`src/tests/forms/numerics`) committed in the same change. **Milestone judged:** SJL-4 of
> [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-4-numerics.md),
> its Broiler.VM clauses 9 and 10. **Owner:** the JavaScript profile owner, with the core architecture
> owner. **Reviewer:** none. Every role this rule names is held by one person (EX-30), and this rule
> does not claim that anyone independent has read it.
>
> **What this file is.** Two predeclared decisions, one per part, each of one evidence class, in the
> templates T1 and T7 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
> **Each part decides alone**, and neither part's runs are read by the other. **There is no
> measurement part**: SJL-4's exit gate has no measurement clause, unlike SJL-3's. The Broiler.JS
> repository commits its own rule for its own engine, and the library-level reference-arm rule lives in
> the home; neither is read here. **It holds no figure.** Where a count or an allowance matters, it
> names the file that fixes it.
>
> **The base.** Unlike SJL-3's, SJL-4's base is not bundle `sjl-0-001`'s commit: it is this family after
> SJL-3, **the commit that carries this rule**, whose product sources are the default branch's after
> SJL-3's merge (`a04b2e5`). Its runs are taken by this bundle's collector **before any code of SJL-4
> lands**, from a clean checkout of that commit on an otherwise idle machine, and retained in
> `sjl-4-001/base/`. A part whose base runs are not retained there, or were taken after any code of
> SJL-4 landed, is void and is not judged.
>
> ## Part A - test262 parity, in both forms (template T1)
>
> ```rule
> evidence class: conformance parity (not a measurement)
> population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
> configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are taken at the base commit as this file's header says and retained in sjl-4-001/base/; the after runs are taken at the commit that lands SJL-4's Broiler.VM cut-over, named in bundle sjl-4-001; both on the same RID, each on an otherwise idle machine
> comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
> admitted difference classes:
>   C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector, the specification clause or the row of JSD-0067's X2 that names the correction
>   C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation (an implementation-approximated Math function), and the area's vectors classify it; each one listed with its vector
>   C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits and the re-run is retained, and a row that still differs outside C1 and C3 is a regression
> no C2: JSD-0067's decision 3 keeps this family's charging units, so an exhaustion verdict that moves on fuel is a regression, not a class
> every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
> MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1, C3 or C4 and is listed, and the native pair meets the same conditions.
> NOT MET otherwise.
> ```
>
> **How it is read.** The bundle retains each comparison's output, which lists every differing row with
> its class or as unclassified, and a table naming each C1 and C3 row with its reason. The floor
> verdicts the runner prints are retained and do not decide part A: a floor is a ratchet over totals,
> and this part judges variants. A row the comparison cannot classify is a regression even when the
> totals improve. **The exhaustion verdicts** milestone SJL-4.6 requires are this part's exhausted
> dimensions: a row exhausted on fuel at the base must be exhausted on fuel after, and a row that
> completes at the base must complete after, or be a listed C1.
>
> ## Part B - the fuel-parity twins and probes (template T7)
>
> ```rule
> evidence class: resource accounting (not a measurement)
> population: every *.small.js twin in src/tests/forms/wide, and every *.small.js probe in src/tests/forms/numerics, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
> configuration: the command-line composition root built in Release; at the base commit, each twin's and probe's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-4-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
> ceilings tried: for each twin and probe, its base figure and one less
> comparison: per twin or probe and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
> admitted difference classes: none; JSD-0067's decision 3 keeps this family's charging units, so a moved figure is a defect of the kernel or of an adapter, not a class
> MET if and only if every twin and probe, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
> NOT MET otherwise.
> ```
>
> **How it is read.** One verdict at every ceiling tried, at the base and after, in both forms: a twin or
> probe that completes one unit sooner or later, in either form, is a charge that moved, and nothing
> admits that here. The probes' answers are chosen so that no correction of SJL-4 changes them (their
> README says how); an answer that changes is read as a moved verdict. The wide twins are SJL-3's
> population again: SJL-4 moves none of their operations, and they show that the cut-over moved no
> charge outside the area either.
>
> ## Revisions
>
> A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
> this one whole and saying it was written afterwards; its decision is then weaker evidence. **Before
> SJL-4.3's code lands, a later bundle shows this file unchanged**, as SJL-3.2 did for SJL-3's rules.
>
> ## What this file does not decide
>
> - **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home,
>   in bundle `sjl-4-001` there.
> - **Broiler.JS's test262 parity.** Broiler.JS's own rule judges it.
> - **The vectors, the closures, the packages and the `I` rules.** Each is a clause of SJL-4 judged by
>   its own mechanism (clauses 8, 11 and 12), not by a comparison this file states.
> - **A speed claim of any kind.** SJL-4 measures nothing.
