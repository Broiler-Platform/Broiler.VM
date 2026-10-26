<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-002's area rule: the wide path on the shared syntax library, step 2, judged by test262 parity in both forms on hosted runners, by artifact identity over the parse corpus, by determinism, by the front end's rules, by the fuel-parity twins, by this family's measurement, and by source fuzzing

**Written:** 2026-10-26, at milestone SJL-13 (work package SJL-13.1), with the area verdict JSD-0083 and bundle
`sjl-13-001`'s rule, before any change either judges exists. **Milestone judged:** SJL-13, at the second of its two
cut-over steps, in which the lowering, the numeric-manifest admission and the binder read the shared tree directly and
the adapter and `JsSyntax` go: its Broiler.VM clauses 4, 5 and 6 at that step, clause 7's rules N5 to N9, N12 and N19,
clause 9's fuzzing at that step, and SJL-13.6's measurement. **Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:**
none. Every role this rule names is held by one person (EX-30), and this rule does not claim that anyone independent
has read it.

**What this file is.** Seven predeclared decisions, one per part: part A in template T1, part B in T6, part E in T7,
part F in T5, part C layer L13, part D the rule runs and part G layer L9. Each part decides alone. Part F is not a gate clause
(decision D24). It holds no figure.

**Step 2 changes no outcome, no byte and no diagnostic** (decision D19). Every input's outcome, every accepted input's
artifact, every module-goal input's ordered specifiers from `JsCompiler.Requests`, every refused input's diagnostic
list and every test262 verdict is expected the same at both commits. The
lowering computes from the shared tree, by `JsParser`'s rules, every fact the adapter computed at step 1, the
top-level-`await` rule included (decision D15), and places every diagnostic as step 1 did. This rule therefore
admits no class but part A's harness classes.

**The commits, named before they exist.**
- **The rule commit** carries this file.
- **The after commit** is the default-branch merge that lands SJL-13.3's step 2, one merge, named in bundle
  `sjl-13-002`.
- **The base commit** is the after commit's first parent. It must meet three conditions: it descends from the rule
  commit; it descends from the commit bundle `sjl-13-001` names as its after commit; and the after commit is the only
  first-parent commit that carries any part of step 2. If step 2 had to land in more than one merge, each is named in
  bundle `sjl-13-002`, the base is the first one's first parent, and the after runs are taken at the last one.
- Part A's runs are dispatched at those commits at any time after each exists; the other parts are taken locally from
  clean checkouts.

A part whose base was taken at any other commit, or whose base commit fails those conditions, is void.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: bundle sjl-13-001's part A's
configuration: bundle sjl-13-001's part A's, the base runs taken at this file's base commit and the after runs at its after commit, the workflow and the driver at the dispatched ref byte-identical to the base commit's
completeness: bundle sjl-13-001's part A's
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C4 harness and environment - bundle sjl-13-001's part A's classes (a), (b), (c') and (e), with their re-runs on the local machine
no C1, no C2, no C3 and no C5: step 2 changes no outcome
every other difference is a regression
MET if and only if all four runs are complete, and every changed verdict of either pair is in C4 and is listed.
NOT MET otherwise.
```

## Part B - artifact identity over the parse corpus (template T6)

```rule
evidence class: artifact identity (not a measurement)
population: bundle sjl-13-001's part B's, sub-populations B1 to B5, B5 being each module-goal input through JsCompiler.Requests under the module options, as the base commit holds the manifest
configuration: the slice compiler's --parse-corpus mode, as the base commit holds it and unchanged at the after commit, built in Release from a clean checkout on the local machine, reporting for each input, sub-population, variant and form what bundle sjl-13-001's part B reports, in B5 the ordered specifiers of an accepted input; run at the base commit and at the after commit, and with --census at both
completeness: every manifest input reported in every sub-population, variant and form at both commits, the manifest's digest equal at both, and no run lost its process
controls: a copy of the base report with one artifact hash changed, one diagnostic's column changed and one specifier of a B5 input changed, which the comparison must report as three differences; retained
comparison: eng/compare-parse-corpus.py, as the base commit holds it: the outcome; the artifact hash of an input accepted at both, or in B5 its ordered specifiers; the ordered diagnostic list of an input refused at both; the census, byte for byte
admitted difference classes: none
every difference is a regression, a specifier list that differs for a B5 input accepted at both among them
MET if and only if both runs are complete, the control is reported as stated, and no difference of any kind is reported.
NOT MET otherwise.
```

## Part C - determinism (layer L13)

```rule
evidence class: determinism (not a measurement)
population and configuration: bundle sjl-13-001's part C's, at this file's after commit, its five checks rows at both commits
controls: bundle sjl-13-001's part C's K1 to K4, applied to a scratch copy of this file's after commit
admitted difference classes: none
MET if and only if every accepted input gives one hash in all four compilations, all five rows pass at both commits, and the controls fail as stated and pass after their revert.
NOT MET otherwise.
```

## Part D - the front end's rules (rule runs)

```rule
evidence class: rules (not a comparison)
population: rules N5 to N9, N12, N19, I5, I6 and A10 of the register, their tests and witnesses
configuration: dotnet test of Broiler.VM.Architecture.Tests in Release at the after commit, BROILER_RULE_MESSAGES set, logs retained; the same at the base commit, read, not judged; rule I5's control, a re-added deleted member of JsSyntax in a scratch copy, which must be refused
MET if and only if every test passes at the after commit, every witness is refused, N12 counts the compiler assembly's files with a comment naming each file step 2 deleted and added, I5 registers every member of JsSyntax and of the adapter, I6 finds no shared type in this family's public API baseline, and the control is refused.
NOT MET otherwise.
```

## Part E - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population, configuration and ceilings tried: bundle sjl-13-001's part I's, the bisect taken at this file's base commit and retained as sjl-13-002/base/fuel-parity.log
admitted difference classes: none
MET if and only if every twin, in both forms, gives after the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

## Part F - this family's compile measurement (template T5)

```rule
evidence class: measurement
population, arms, configuration, condition, quantity, noise floor, comparison and readings: bundle sjl-13-001's part J's, with its workloads, B this file's base commit's build and A this file's after commit's; retained in sjl-13-002/measurement/
admitted difference classes: none
MET if and only if every lane's condition passes, every effective-configuration line is identical, and for every workload D <= N.
NOT MET otherwise.
```

**What part F decides.** Whether reading the shared tree directly compiles more slowly than through the adapter on
this machine. Not a gate clause (decision D24).

## Part G - source fuzzing (layer L9)

```rule
evidence class: robustness (not a measurement)
population, configuration, reading and controls: bundle sjl-13-001's part G's, both sessions, with its seeds file, at this file's after commit, and the base's sessions at this file's base commit retained and read, not judged; the wide control's throw placed in a lowering or binder walk over the shared tree that the seeds reach, since the adapter is gone
MET if and only if neither of the after commit's sessions reports a finding and both controls are reported as stated.
NOT MET otherwise.
```

**How it is read.** Step 2 moves the lowering's and the binder's walks onto the shared tree, which no session reached
before this file's after commit (clause 9 judged on SJL-13's final state).

## Revisions

As bundle `sjl-13-001`'s.

## What this file does not decide

Step 1 (bundle `sjl-13-001`); the home clauses; clause 8's publication (the collector's runs at this file's after
commit); JS-2's closure, which the profile's ledger records on both bundles; a speed claim of any kind.
