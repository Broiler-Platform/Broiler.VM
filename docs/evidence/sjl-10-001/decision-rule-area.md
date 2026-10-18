<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-10-001's area rule: the Temporal core's test262 parity on hosted runners, its reference datasets, and the fuel-parity twins

**Written:** 2026-10-18, at milestone SJL-10 (work package SJL-10.1), with the area verdict
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
before any shared member or cut-over of the area exists. **Milestone judged:** SJL-10 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-10-temporal.md): its
Broiler.VM clause "test262 in both forms and the polyfill dataset meet the area rule. For Broiler.VM that is
unchanged." **Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:** none. Every role
this rule names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class.
- **The templates.** Part A is in template T1 of
  [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules)
  and part C in T7. Part B is a vector agreement in T3's sense, over the programs this family retains rather than the
  shared corpus.
- **Each part decides alone**, and no part's runs are read by another.
- **There is no measurement part.** SJL-10's exit gate has no measurement clause, as SJL-8's had none.
- **Other rules.** The Broiler.JS repository commits its own rule for its own engine. It is not read here.
- **It holds no figure.** Where a count or an allowance matters, it names the file that fixes it, or states it as a
  parameter of the rule.

**What earlier bundles taught this rule, before any run of SJL-10.**
- **Part A follows bundle `sjl-8-001`'s part A:** the hosted workflow for both runs. A row whose exhausted dimension
  moves on the wall clock is run again on the programme's local machine. That machine is the one bundles `sjl-5-001`
  to `sjl-8-001` were taken on, named in the bundle by its processor count, its SDK and its operating system,
  otherwise idle.
- **This family has one correction in this area**, row R6 of the correspondence, which SJL-10.4 makes (JSD-0078
  decision D9). No test262 case at the pin is known to reach it: this family passes every scored case under
  `test/built-ins/Temporal` (JSD-0054 section 6). So part A names R6 as its only C1 row before the run, and a C1 row
  R6 does not explain is also a finding against the record.
- **This family's pins do not move.** It follows `tc39/proposal-temporal@e8cc03fc` (JSD-0053) and test262
  `ccaac100`, and JSD-0078's decisions D1 and D2 keep both; its tz release and calendar sources are SJL-8's, which
  SJL-10 does not touch.
- **Part B follows bundle `sjl-8-001`'s part B**, over the Temporal programs this family retains. Part C is that
  bundle's part C unchanged: the command-line root carries no Intl data, and Temporal is admitted only with Intl and
  BigInt (JSD-0054), so the twins reach none of the area's code.

**The base.** SJL-10's base is this family after SJL-8, **the commit that carries this rule**. Its product sources
are the default branch's after SJL-8's merges, `047dcec`'s.
- Part A's base runs are taken by the workflow at that commit, named by all forty digits, and retained in
  `sjl-10-001/base/`.
- Parts B and C are taken locally from a clean checkout of the same commit.

A part whose base was taken at any other commit is void and is not judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it and JSD-0078's decision D2 keeps it - every variant, under the wide manifest, once in the bytecode form and once in the native form; test/built-ins/Temporal and test/intl402/Temporal are in it, and a regression on any other path fails this part too
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to this file's commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the commit that carries this file, and the after runs at the commit that lands SJL-10.4's Broiler.VM cut-over, named in bundle sjl-10-001; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; JSD-0078 names one such correction, row R6 (three constructors that end the invocation on a year above 2^53, decision D9); each one is listed with its path, its variant and the specification clause it follows, and a C1 row that R6 does not explain is recorded in the bundle as a finding that JSD-0078's correspondence missed
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's, the machine otherwise idle, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: JSD-0078 keeps this family's charging units and adds no charge (its X3), so an exhaustion verdict that moves on fuel is a regression, not a class
no C3 and no C5: this family's proposal revision and test262 pin do not change (JSD-0078 decisions D1 and D2), where the pinned text and its polyfill disagree it already follows the text (decision D7), and SJL-10 moves no data, so a verdict that moves because a revision, a policy or the data moved is a regression, not a class
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-8-001`'s part A. The bundle retains:
- both dispatches' artifacts;
- each comparison's output, which lists every differing row with its class or as unclassified;
- a table naming each C1 row with its clause and whether R6 explains it, and each (c') and (e) row with its re-runs.

The floor verdicts the harness prints are retained and do not decide part A. Each native run exits 4, which
completeness admits, as earlier bundles' did. A row the comparison cannot classify is a regression even when the
totals improve. The skipped `canonical-tz` and `Intl.Era-monthcode` cases stay skipped at both commits (JSD-0054), so
they move no verdict.

## Part B - the Temporal reference datasets unchanged (vector agreement, T3's sense)

```rule
evidence class: conformance parity (not a measurement)
population: the programs this family retains as reference datasets under src/tests/temporal/, as committed with this rule, each with its retained answer and its divergences file: src/tests/temporal/temporal.js; src/tests/temporal/intl/temporal-intl.js; src/tests/temporal/calendars/temporal-calendars.js, against both its polyfill answer and its ICU4X answer; src/tests/temporal/calendars-intl/calendar-formats.js; and src/tests/temporal/zone-names/zone-names.js - every Temporal program this family retains
configuration: the slice compiler's --checks, built in Release from a clean checkout, at the base commit and at the commit that lands SJL-10.4's cut-over, on the local machine this file's header names; the checks that run those programs and compare every line are temporal/t1/temporal-matches-the-reference-polyfill, temporal/t2/intl-over-temporal-matches-the-reference-polyfill, temporal/t3/calendars-match-the-reference-polyfill, temporal/t3/calendar-conversions-match-icu4x, temporal/t4/calendar-formats-match-the-reference-polyfill and temporal/t5/zone-names-match-the-reference-polyfill; each check's verdict and output are retained
comparison: per check, its verdict at the base against its verdict after; and per program, its text, its retained answer and its divergences file at the base against the same after, byte for byte (git diff between the two commits over src/tests/temporal/)
admitted difference classes: none; the reference datasets are this family's acceptance of its own Temporal, which JSD-0078 keeps as it is: no line of them reaches row R6, and no line of temporal.js is a case where the pinned text and its polyfill disagree (decision D7), so a line that moves, a divergence added or removed, or a check that fails is a regression
MET if and only if every check named passes at both commits, and no file under src/tests/temporal/ differs between them.
NOT MET otherwise.
```

**How it is read.** A check compares every line of its program with the retained answer, after its divergences are
applied, and fails on any line that differs. So a passing check at both commits, with the program, its answer and its
divergences unchanged, means every line of every program is the same at both commits. That is the milestone's "the
polyfill dataset meet[s] the area rule. For Broiler.VM that is unchanged", for the polyfill dataset and its
companions. Broiler.JS's runs of `temporal.js` and `temporal-calendars.js` are judged by its own rule.

## Part C - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine this file's header names; at the base commit, each twin's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-10-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin, its base figure and one less
comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; JSD-0078 adds no charge, so a moved figure is a defect of a shared member or of an adapter, not a class
MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-8-001`'s part C. The command-line root carries no Intl data, and so no Temporal,
which this family admits only with Intl and BigInt (JSD-0054). Its twins show that the cut-over moved no charge
outside the area. Inside the area, part A's exhaustion verdicts on fuel are the charge evidence.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
and says it was written afterwards, and its decision is then weaker evidence. **Before SJL-10.3's code lands, a later
bundle shows this file unchanged**, as SJL-3.2 and SJL-4 to SJL-8 did for theirs.

## What this file does not decide

- **The home clauses**: every sub-area implemented in the library, its vectors passing, and the polyfill dataset at
  library level wherever it can run there. They are judged in Broiler.JS, where the library lives.
- **Broiler.JS's test262 parity and its runs of the polyfill dataset.** Broiler.JS's own rule judges them.
- **The vectors.** SJL-10.2 adds them, and they may falsify the area record.
- **This family's other Broiler.VM clauses**: rules `I1` to `I7` hold, and every root is published and run in three
  modes. Each is judged by its own mechanism, the architecture tests and the release candidate's runs, not by a
  comparison this file states.
- **A speed claim of any kind.** SJL-10 measures nothing. A hosted run's wall clock is not compared with a local one,
  and part A's (e) raises the wall clock so that speed decides nothing.
