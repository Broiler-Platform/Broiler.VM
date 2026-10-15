<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-6-001's area rule: the Date area's test262 parity on hosted runners, and its fuel-parity twins and probes

**Written:** 2026-10-15, at milestone SJL-6 (work package SJL-6.1), with the area verdict
[JSD-0072](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0072-date-algorithms-move-to-the-shared-algorithms-library.md),
before any shared member, adapter or cut-over of the area exists, and with part B's probes
(`src/tests/forms/date`) committed in the same change. **Milestone judged:** SJL-6 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-6-date.md),
its Broiler.VM clause 7. **Owner:** the JavaScript profile owner, with the core architecture owner.
**Reviewer:** none. Every role this rule names is held by one person (EX-30), and this rule does not
claim that anyone independent has read it.

**What this file is.** Two predeclared decisions, one per part, each of one evidence class, in the
templates T1 and T7 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
**Each part decides alone**, and no part's runs are read by another. **There is no measurement part**:
SJL-6's exit gate has no measurement clause, as SJL-5's had none. The Broiler.JS repository commits its own
rule for its own engine, and the library-level reference-arm rule lives in the home; neither is read here.
**It holds no figure.** Where a count or an allowance matters, it names the file that fixes it, or states
it as a parameter of the rule.

**Beside this bundle's other rule.** [`decision-rule.md`](decision-rule.md) is SJL-6.0's check, which
asked whether `.github/workflows/test262-evidence.yml` takes the runs bundle `sjl-0-001`'s procedure takes
on one machine; the [README](README.md) records it MET. **This file is the rule that names that workflow
for both SJL-6's base and its after runs**, which is the rest of SJL-6.0's done-when. It reads nothing of
the check but its verdict, and that check's comparison of a hosted run with a local one is not repeated
here: part A compares a hosted run only with another hosted run.

**What SJL-5 and SJL-6.0 taught this rule, before any run of SJL-6.** Bundle `sjl-5-001`'s part A met
every changed row as a wall-clock exhaustion at the base whose re-runs alone agreed at both commits, and
bundle `sjl-6-001`'s check found that the hosted runners and the local machine differ only in which
allowance a race exhausts first. Fuel is deterministic and the wall clock is not, so a row whose
exhausted dimension moves on the wall clock is judged as bundle `sjl-5-001`'s rule judged it: run alone
again at both commits on one machine, and once more with the wall clock out of the way if the re-runs
differ. The workflow takes none of these re-runs (its header says so); **this rule takes them on the
programme's local machine**, the one bundle `sjl-5-001`'s runs were taken on, named in the bundle by its
processor count, its SDK and its operating system, otherwise idle.

**The base.** SJL-6's base is this family after SJL-5, **the commit that carries this rule**, whose
product sources are the default branch's after SJL-5's merge, `72ad941`'s. Its runs are taken by the
workflow at that commit, named by all forty digits, and are retained in `sjl-6-001/base/`; a hosted run
names its tested commit in its own `run.txt`, so the base is the base whenever it is dispatched. Part B's
base figures are taken locally from a clean checkout of the same commit. A part whose base runs were taken
at any other commit is void and is not judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to this file's commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the commit that carries this file, and the after runs at the commit that lands SJL-6.4's Broiler.VM cut-over, named in bundle sjl-6-001; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector, the specification clause or the row of JSD-0072's correspondence that names the correction (T4, G3, P9)
  C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation (the fallback grammar of Date.parse, the time zone text of the renderings), and the area's vectors classify it; each one listed with its vector
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's, the machine otherwise idle, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: JSD-0072 keeps this family's charging units, so an exhaustion verdict that moves on fuel is a regression, not a class
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1, C3 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** The bundle retains both dispatches' artifacts, each comparison's output, which lists
every differing row with its class or as unclassified, and a table naming each C1 and C3 row with its
reason, and each (c') and (e) row with its re-runs. The floor verdicts the harness prints are retained and
do not decide part A: a floor is a ratchet over totals, and this part judges variants. The native floor
was set in the `x86-64-win64` form and the hosted runners are `x86-64-sysv`, so each native run exits 4,
which completeness admits as bundle `sjl-6-001`'s check admitted it. A row the comparison cannot classify
is a regression even when the totals improve. **The hosted runners' image may differ between the two
dispatches**, and within one, as it did in SJL-6.0's run; the bundle records each runner's image, and a
difference it causes is judged like any other: fuel is deterministic, and a wall-clock row is re-run on one
machine.

## Part B - the fuel-parity twins and probes (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, and every *.small.js probe in src/tests/forms/date, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine this file's header names; at the base commit, each twin's and probe's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-6-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin and probe, its base figure and one less
comparison: per twin or probe and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; JSD-0072 keeps this family's charging units, so a moved figure is a defect of a shared member or of an adapter, not a class
MET if and only if every twin and probe, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-5-001`'s part B: one verdict at every ceiling tried, at the base and
after, in both forms, and nothing admits a charge that moved. The probes' answers are chosen so that no
correction of SJL-6 changes them (their README says how); an answer that changes is read as a moved
verdict. The wide twins show that the cut-over moved no charge outside the area. `Temporal` and `Intl` are
outside the command-line root's manifest, so their exhaustion verdicts are part A's.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
this one whole and saying it was written afterwards; its decision is then weaker evidence. **Before
SJL-6.3's code lands, a later bundle shows this file unchanged**, as SJL-3.2, SJL-4 and SJL-5 did for
theirs.

## What this file does not decide

- **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home,
  in bundle `sjl-6-001` there.
- **Broiler.JS's test262 parity and its non-UTC host run.** Broiler.JS's own rule judges them.
- **The vectors, the closures, the packages and the `I` rules.** Each is a clause of SJL-6 judged by
  its own mechanism (clauses 6, 8 and 9, and the home clauses), not by a comparison this file states.
- **A speed claim of any kind.** SJL-6 measures nothing; a hosted run's wall clock is not compared with a
  local one, and part A's (e) raises the wall clock so that speed decides nothing.
