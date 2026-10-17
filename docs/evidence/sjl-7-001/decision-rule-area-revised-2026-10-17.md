<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-7-001's area rule, revised after part C's run was seen: the after arm on Broiler.Unicode 0.1.0-preview.5

**Written:** 2026-10-17, after SJL-7.6, **after** part C's run, [`measurement/`](measurement/README.md), had been
seen and read NOT MET on its clause 3, for casing and identifier scanning in the native form. The decision it
gives is therefore **weaker evidence** than the one [`decision-rule-area.md`](decision-rule-area.md) gives, as that
rule's own "Revisions" paragraph says. **Owner:** the JavaScript profile owner, with the core architecture owner.
**Reviewer:** none; every role is one person (EX-30), and this revision does not claim that anyone independent has
read it. The bundle's README states part C's decision under both rules.

**Why it was written.** Part C's reading left the choice to the owner: take a second measurement under a dated
revision of the rule, or leave JSD-0031's fifth reason open for those two workloads in the native form. On
2026-10-17 the owner had the owner's normalization, case mapping and lexical classes made faster, in
Broiler.Unicode, rather than accept the cost. Those changes were published as `0.1.0-preview.5`, and this family
moved to them in [#138](https://github.com/Broiler-Platform/Broiler.VM/pull/138) (`9508bd5`), recorded by
[JSC-311](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-311) with the package bundle
JS-10-008. The owner then directed, the same day, that the programme continue. This file is the second
measurement that choice calls for. It does not re-run part C's arm A at `be0b610`, which the rule forbids.

## What changed, and why

1. **Arm A is a later commit.** Under the original rule arm A is "the commit that lands SJL-7.4's Broiler.VM
   cut-over", `be0b610`, on Broiler.Unicode `0.1.0-preview.4`. Here **arm A is the commit that carries this
   file**, whose product sources (every path outside `docs/` and outside any `docs/` directory) are those of
   `9508bd5`, the default branch after #138. Since `be0b610`, the product sources changed in two merges, and the
   arm carries both:
   - [#137](https://github.com/Broiler-Platform/Broiler.VM/pull/137) (`38f1579`), the defects the bundle named
     outside SJL-7's clauses: `Intl.Locale`'s `variants` option and Temporal's duration designators lowered with
     ASCII casing, not the platform's;
   - [#138](https://github.com/Broiler-Platform/Broiler.VM/pull/138) (`9508bd5`), the family on Broiler.Unicode
     `0.1.0-preview.5`, with rule N22's regenerated tables and budget.

   The sub-bundle's manifest names every commit since `be0b610` with its files outside the documents.
2. **Its runs are a sub-bundle of their own**, `sjl-7-001/measurement-revised/`, taken by the collector's
   `measure-revised` step after this file is committed. `measurement/` is not read, moved or run again.
3. **Arm B is unchanged**: built from `7b44dc7`, the commit that carries the original rule. So is the base the
   difference is taken against: SJL-7's base, this family after SJL-6.
4. **One reading is added, so that the revision cannot be repeated until a run meets**: this is the second and
   last measurement of part C, and a NOT MET here leaves JSD-0031's fifth reason open.

**Nothing else changed.** Part C's population, configuration, condition, configuration check, quantity, noise
floor, comparison, attribution and MET line are the original's. The workloads are those committed with the
original rule, and the collector refuses to run if either arm's tree holds a different byte of them. Parts A and
B are not revised, and this file does not read their runs. Part C's block, restated with arm A as revised:

```rule
evidence class: measurement
population: the three workloads of src/tests/forms/unicode as committed with decision-rule-area.md - normalize.js (normalize in its four forms), casing.js (the default and two language-sensitive case mappings) and identifiers.js (the compiler's identifier scanning outside ASCII, through the Function constructor) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; the local machine decision-rule-area.md's header names, and one RID, both named in the sub-bundle's manifest
arms: B the base, built from the commit that carries decision-rule-area.md (7b44dc7); A the commit that carries this file, whose product sources are those of 9508bd5, this family on Broiler.Unicode 0.1.0-preview.5 after SJL-7.4's cut-over; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
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
  - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped;
  - this is the second and last measurement of part C: a NOT MET here is not followed by a third under a further revision, and leaves JSD-0031's fifth reason open for the workloads and forms it names.
```

## What it decides, and what it does not

- **What it decides:** whether this family's normalization, casing and identifier scanning, on Broiler.Unicode
  `0.1.0-preview.5`, are slower than the base beyond the A/A floor. That is JSD-0031's fifth reason, "its runtime
  cost", for this engine, as part C asks it, **as weaker evidence**.
- **What it does not decide:** that part C's first run was wrong. `measurement/` stands as the reading of arm A at
  `be0b610`, NOT MET on clause 3. A MET here says the after arm on `0.1.0-preview.5` is within this machine's
  noise of the base, not that the cut-over on `0.1.0-preview.4` was.
- **Nor** parts A and B, the vectors, the closures, the packages, or any clause of SJL-7 that bundle `sjl-7-001`
  judged at `be0b610`. Those stand on `0.1.0-preview.4` as the README reads them. Whether they are taken again on
  `0.1.0-preview.5` is a separate choice, and this file does not make it.
- **Nor** Broiler.JS's measurement, which its own rule judges in its own repository, and which Broiler.VM's
  release gate 10 forbids comparing with this one.
