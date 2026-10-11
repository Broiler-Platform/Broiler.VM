<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-3-001's decision rule: the pilot's test262 parity, the fuel-parity twins, and Broiler.VM's measurement

**Written:** 2026-10-11, at milestone SJL-0 (work package SJL-0.9), before any shared library, adapter
or cut-over of the pilot exists, and with the workloads of part C committed in the same change.
**Milestone judged:** SJL-3 of [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md),
its Broiler.VM clauses on conformance, resource accounting and measurement. **Owner:** the JavaScript
profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule names is
held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class, in the
templates T1, T7 and T5 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
**Each part decides alone**, and no part's runs are read by another. Part C is the only measurement:
[roadmap.gates.md section 17](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
rule 8 asks for one evidence class per bundle that carries a figure, so SJL-3 retains part C's runs in
a sub-bundle of their own, `sjl-3-001/measurement/`, with its own README and manifest. The Broiler.JS
repository commits its own rule for its own engine, and the library-level reference-arm rule lives in
the home; neither is read here. **It holds no figure.** Where a count or an allowance matters, it
names the file that fixes it.

**The base.** Every part compares an after arm with the base that bundle
[`sjl-0-001`](../sjl-0-001/README.md) names: its commit, taken before any engine changed. A part
whose base is not retained in that bundle, or whose base cannot be rebuilt from that commit, is void
and is not judged.

## Part A - test262 parity, in both forms (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are bundle sjl-0-001's, at its base commit; the after runs are taken at the commit that lands SJL-3's Broiler.VM cut-over, named in bundle sjl-3-001, on the same RID
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector or specification clause that names the correction
  C2 charge model by decision - an exhaustion verdict moves because a dated decision changed the charging units of an operation the pilot moved; each one listed with the decision
  C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation, and the area's vectors classify it; each one listed with its vector
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits and the re-run is retained, and a row that still differs outside C1 to C3 is a regression
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after other than under C2, and an internal defect on either side
MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1 to C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** The bundle retains each comparison's output, which lists every differing row with
its class or as unclassified, and a table naming each C1, C2 and C3 row with its reason. The floor
verdicts the runner prints are retained and do not decide part A: a floor is a ratchet over totals,
and this part judges variants. A row the comparison cannot classify is a regression even when the
totals improve.

## Part B - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide as committed at bundle sjl-0-001's base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release; at the base, each twin's smallest completing fuel allowance bisected under the bytecode form, as bundle sjl-0-001's fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin, its base figure and one less
comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after
admitted difference classes:
  C2 charge model by decision - a twin's smallest completing allowance moves because a dated decision changed the charging units of an operation the pilot moved; the twin is then bisected again after, both figures listed with the decision, and it must complete at its new figure and refuse with AllowanceExhausted on Fuel at one less in both forms
MET if and only if every twin, in both forms, after as at the base, completes at its base figure and refuses with AllowanceExhausted on Fuel at one less, or moved under C2 and meets C2's own condition.
NOT MET otherwise.
```

**How it is read.** One verdict at every ceiling tried, at the base and after, in both forms: a twin
that completes one unit sooner or later, in either form, is a charge that moved, and only a dated
decision admits that.

## Part C - Broiler.VM's measurement (template T5)

```rule
evidence class: measurement
population: the three workloads of src/tests/forms/numbers as committed with this rule - to-string.js (number to string), to-number.js (string to number) and literals.js (the compiler's literal values) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; one machine and one RID, both named in the sub-bundle's manifest
arms: B the base, rebuilt from bundle sjl-0-001's base commit; A the commit that lands SJL-3's Broiler.VM cut-over; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane
condition: each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; a lane whose answer differs fails the condition
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

**The base reading at SJL-0.** Bundle `sjl-0-001` measures arm B alone, under this part's harness:
for each workload and form, B, its control and an A/A lane B-again, interleaved. **Its one decision
is not this part's**: each workload's attribution E(B) is recorded as the base's figure when |E(B)|
exceeds |Q(B) - Q(B-again)|, and as below resolution otherwise. It exists so the workloads, the
harness and the base's noise floor exist before any code moves, and it decides nothing about SJL-3,
which rebuilds B and runs it interleaved with A.

**What part C does not decide.** A speed claim of any kind, in either direction. A comparison with
Broiler.JS, whose own measurement is judged by its own rule in its own repository; Broiler.VM's
release gate 10 forbids that comparison. Anything about a RID or a machine other than the one named.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
this one whole and saying it was written afterwards; its decision is then weaker evidence, as the
core's bundle `ubc-4-006` records for its own. **SJL-3.2 confirms this file unchanged before any code
of SJL-3.3 to SJL-3.5 exists**, and a later bundle shows the ancestry with
`git merge-base --is-ancestor`.

## What this file does not decide

- **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home.
- **Broiler.JS's test262 parity or measurement.** Each is judged by Broiler.JS's own rule.
- **The differential cross-check.** Template T8 cannot meet a conformance clause on its own, and the
  milestone retains its report beside these parts without a rule here.
- **Vectors, closures, packages and the `I` rules.** Each is a clause of SJL-3 judged by its own
  mechanism, not by a comparison this file states.
