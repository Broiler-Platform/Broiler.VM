<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle JSB-11-002's decision rule: per-block baseline steps, adopted or refused on one measurement

**Written:** 2026-09-17, before any per-block code existed. **Route:** MVP-8, as to the baseline form's
step granularity. **Thresholds:** fixed by the repository owner before any per-block code existed; the
rule they belong to and its readings were ruled by the owner on 2026-09-17. **Owner:** profile
architecture owner. **Reviewer:** none.

**What this file is.** The one predeclared decision of bundle `jsb-11-002`, as
[roadmap.gates.md section 17](../../roadmap.gates.md#17-measurement-discipline) rule 8 asks for: exactly
one evidence class, a measurement, and exactly one decision. It is committed before the bundle's
manifest, before either arm is built and before any per-block code, so no observation of the candidate
of any kind can have shaped it. It holds no measured figure. The bundle's README, written when the
measurement is collected, is the only place a figure from the measurement goes.

**How it is read.** `eng/measure-baseline-granularity.py` reads the fenced `rule` block below and
computes the verdict from that block and from nothing else. Its `read_rule` refuses a block whose
quantity, noise floor or difference is not the one it computes, a block that does not quantify its
clauses as all of them or does not say what happens otherwise, and a block with a threshold nobody has
fixed. Its `summarise` computes clauses 2, 3 and 4. Clauses 1 and 5 are enforced by stopping: a failed
condition or a differing effective-configuration line ends the run, and no `summary.txt` is written.

**The arms.** `C` is the control, the last commit before the first per-block commit, whose baseline form
emits every instruction as one call into the interpreter's dispatch for it. `K` is the candidate, the
last per-block commit. `K-again` is the candidate run a second time under another lane name, the A/A
lane. The bundle's manifest names both commits and every binary by hash. Both arms carry the core
meter's fuel pre-admission (route MVP-9), which lands entirely before `C`.

```rule
population: every shape in src/tests/forms/wide as committed at d75499a, except the *.small.js fuel-parity twins and fresh-small.js; each in the bytecode form and in the native form under x86-64-win64; the JIT Release command-line host, win-x64, one machine
arms: C the control, K the candidate, K-again the candidate's A/A lane, each named by commit and binary hashes in the bundle's manifest
repetitions: R=7 retained, W=3 warm-up repetitions retained in runs.jsonl and excluded from Q
quantity: Q(shape, arm, form) = median over retained repetitions of (run_ms - check_ms)
noise floor: N(shape, form) = |Q(shape, K, form) - Q(shape, K-again, form)|
difference: D(shape, form) = Q(shape, C, form) - Q(shape, K, form)      (positive: candidate faster)
shapes: the number of shapes in the population
ADOPT per-block steps if and only if all of:
  1. condition-before, every lane's condition and condition-after pass for every shape;
  2. native, every shape: D >= -N                                  (the candidate is never slower beyond the floor);
  3. native, at least ceil(2/3 * shapes) shapes: D > N and Q(K)/Q(C) <= 0.90;
  4. bytecode, every shape: |D| <= N or |D| <= 0.03 * Q(C, bytecode)   (the interpreter is not moved);
  5. effective-configuration lines, reported by every timed child, identical across arms, lanes and repetitions.
REFUSE otherwise.
readings, ruled by the owner on 2026-09-17:
  - a native shape whose D is within its floor (D <= N) is a tie; a tie does not count toward clause 3, so it counts against adoption;
  - a bytecode shape meets clause 4 when it moved by no more than 0.03 of Q(C, bytecode), or when it stayed within its floor N;
  - clause 2 applies to every native shape as written;
  - an interrupted collection (a machine restart, a killed process) may be re-run from the start, and the stopped run is retained beside the new one and named as stopped;
  - a failed answer condition or output-hash condition in the candidate is a refusal, not a re-run.
as the harness computes clause 3:
  - a native shape whose Q(C, native) is not positive has no ratio, and does not count toward clause 3.
```

**Where the thresholds come from.** The owner fixed the three thresholds: at least two thirds of the
shapes, a native ratio of 0.90, and a bytecode fraction of 0.03. The owner also ruled that the rule is
this five-clause rule with those thresholds, so the harness stays as committed, and ruled the readings
in the block. The last line of the block is not a reading the owner gave: it is how the harness computes
clause 3, stated so that the file says everything the verdict is computed from. `R` and `W` are not the
owner's to choose: section 17 inherits the count of the core's baseline register, whose rule 4 fixes
seven per lane; no JavaScript milestone has fixed another; and three warm-up repetitions, retained and
named as discarded, follow `eng/compare-forms.py`.

**What the verdict does.**
- **ADOPT.** JSD-0026 is taken on this bundle and takes per-block steps, and a stage JSB-12 restates
  JSB-11's gate against them. Its bounds are fixed beside this file's commit, in
  [`jsb-12-001/bounds.md`](../jsb-12-001/bounds.md).
- **REFUSE.** JSD-0026 is still taken, and it refuses per-block steps. The per-block commits are
  reverted together in one new commit. No JSB-12 stage is added, and the baseline form keeps one call
  per instruction.
- **Either way**, MVP-8 ends as to granularity, and a new route row, with its id allocated when it is
  committed, carries the value-carrying tier (the owner's answer of 2026-09-17).

**Disclosure: what had been seen when this rule was written.**
- Working-tree figures of the prototype commits `525ea13` (per-block steps), `17747dc` (an engine-held
  fuel credit, since refused), `47fbba5` and `9ec81de` (their measurements). No figure from them is
  used, and none is recorded.
- Bundle VM-5-002's E10 (four JavaScript shapes, both forms) and E11 (a bytecode Octane subset), which
  timed the per-instruction form on the core meter with fuel pre-admission. No figure from them is used.
- Bundle JSB-11-001's retained reports of the per-instruction form. No figure from them is used.
- The harness was rehearsed at `d75499a` and driven again at `f127d92`, each time with both arms
  pointed at one per-instruction build and outside the repository. As those commits say, no figure from
  either was recorded.

**What this rule does not decide.** It says nothing about what per-block steps cost on the meter before
fuel pre-admission. It judges no `linux-x64` lane, no Native AOT image and no Octane run, and the
fresh-process shape `fresh-small.js` is outside its population. Those are bounds of stage JSB-12, fixed
in `bounds.md`.
