<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle JSV-4-001's decision rule: the value form, adopted or refused on one measurement

**Written:** 2026-09-24, before any code of stage JSV-2 existed. **Decision:**
[JSD-0035](../../decisions/0035-the-value-form-emitted-semantics-over-nan-boxed-values.md) section 10,
"speed is judged once, by a predeclared rule, against a retained measurement". **Thresholds:** fixed by
the repository owner on 2026-09-24, before any JSV-2 code existed, together with the ruling that the
form is judged once and whole, at the end of stage JSV-4, rather than after JSV-2. **Owner:** JavaScript
profile owner. **Reviewer:** none.

**What this file is.** The one predeclared decision of bundle `jsv-4-001`, as
[roadmap.gates.md section 17](../../roadmap.gates.md#17-measurement-discipline) rule 8 asks for: exactly
one evidence class, a measurement, and exactly one decision. It is committed before the first commit of
stage JSV-2, so no observation of an inline template, a direct call or a frame codec of any kind can have
shaped it. It holds no measured figure. The bundle's README, written when the measurement is collected,
is the only place a figure from the measurement goes.

**How it is read.** The harness that collects the bundle is written before the bundle's manifest, and it
reads the fenced `rule` block below and computes the verdict from that block and from nothing else, as
`eng/measure-baseline-granularity.py` does for bundle `jsb-11-002`: it refuses a block whose quantity,
noise floor or differences are not the ones it computes, a block that does not quantify its clauses or
say what happens otherwise, and a block with a threshold nobody has fixed. Clauses 1 and 5 are enforced
by stopping: a failed condition or a differing effective-configuration line ends the run, and no
summary is written.

**The arms.** All four timed arms are built from one commit, the candidate: the commit that completes
stage JSV-4. `B` is its bytecode form, the control. `L` is its baseline native form, which is timed and
reported and judged by no clause. `V` is its value form, the candidate. `V-again` is the value form run a
second time under another lane name, the A/A lane. `B-control` is the bytecode form built from the commit
that adds this file, which is the last commit before the first JSV-2 commit; it exists only for clause 4.
The bundle's manifest names both commits and every binary by hash.

```rule
population: every shape in src/tests/forms/wide as committed with this file, except the *.small.js fuel-parity twins and fresh-small.js; each in the bytecode form, the baseline native form and the value form, the two native forms under the x86-64 convention of the machine's runtime identifier; the JIT Release command-line host; one machine, named in the manifest
arms: B the control (bytecode), L the baseline form (reported, judged by no clause), V the candidate (value form), V-again the candidate's A/A lane, all built from the candidate commit; B-control the bytecode form built from the commit that adds this file; each named by commit and binary hashes in the bundle's manifest
repetitions: R=7 retained, W=3 warm-up repetitions retained in runs.jsonl and excluded from Q
quantity: Q(shape, arm) = median over retained repetitions of (run_ms - check_ms)
noise floor: N(shape) = |Q(shape, V) - Q(shape, V-again)|
difference: D(shape) = Q(shape, B) - Q(shape, V)      (positive: the value form faster)
bytecode drift: E(shape) = Q(shape, B) - Q(shape, B-control)
shapes: the number of shapes in the population
ADOPT the value form if and only if all of:
  1. condition-before, every lane's condition and condition-after pass for every shape, and every fuel-parity twin completes at one smallest allowance in the bytecode and value forms and refuses with AllowanceExhausted on Fuel at one less in both;
  2. value, at least ceil(1/2 * shapes) shapes: D > N and Q(V)/Q(B) <= 0.90;
  3. value, every shape: Q(V) <= 1.25 * Q(B) + N;
  4. bytecode, every shape: |E| <= 0.03 * Q(B-control)      (the interpreter is not moved);
  5. effective-configuration lines, reported by every timed child, identical across arms, lanes and repetitions.
REFUSE otherwise.
readings, ruled by the owner on 2026-09-24:
  - the form is judged once, at the commit that completes stage JSV-4, and not after JSV-2 or JSV-3;
  - a shape whose D is within its floor (D <= N) is a tie; a tie does not count toward clause 2;
  - the Octane benchmarks are timed in all three forms and reported in the bundle's README beside the verdict, and no clause judges them;
  - an interrupted collection (a machine restart, a killed process) may be re-run from the start, and the stopped run is retained beside the new one and named as stopped;
  - a failed answer condition or fuel-parity condition in the candidate is a refusal, not a re-run.
as the harness computes clause 2:
  - a shape whose Q(B) is not positive has no ratio, and does not count toward clause 2.
```

**Where the thresholds come from.** The owner fixed the four thresholds: at least half the shapes, a
value-to-bytecode ratio of 0.90 on them, no shape slower than 1.25 times bytecode beyond the A/A floor,
and a bytecode drift of 0.03. The owner also ruled that the form is judged whole at the end of JSV-4,
because at JSV-2 calls, property access and closure bindings still run through helpers, and that the
Octane benchmarks are reported rather than judged. That narrows the population JSD-0035 section 10 names
("the wide shapes and the Octane benchmarks") to the wide shapes for the verdict, and it is the owner's
ruling, recorded here before any candidate existed rather than after one was seen. The last line of the
block is not a reading the owner gave: it is how the harness computes clause 2, stated so that the file
says everything the verdict is computed from. `R` and `W` are not the owner's to choose: they are the
counts bundle `jsb-11-002` inherited from the core's baseline register and from `eng/compare-forms.py`.

**What the verdict does.**
- **ADOPT.** JSD-0035 is taken on this bundle, the value form stays, and MVP-8's alternative branch is
  settled as that record says.
- **REFUSE.** JSD-0035 is refused on this measurement. The commits of stages JSV-2, JSV-3 and JSV-4 are
  reverted together in one new commit; whether stages JSV-0 and JSV-1, which make no speed claim, stay in
  the tree is the owner's to rule then. The population is not narrowed until the rule passes.

**Disclosure: what had been seen when this rule was written.**
- The working-tree observations of 2026-09-23 that motivate JSD-0035: the baseline and numeric forms
  against bytecode on Octane and on the whole conformance suite. No figure from them is used, and none
  is recorded.
- Stage JSV-1's exploratory runs of 2026-09-24: the whole conformance suite in the value form, with and
  without handle-stress, the fifteen Octane benchmarks in the value form, and short timed `zlib` runs of
  the bytecode and value forms, one of them with the meter's charge batched as an experiment that was not
  kept. They timed the per-instruction value form, whose every instruction is a helper; no inline
  template existed. No figure from them is used, and none is recorded.
- No run of any wide shape in the value form had been timed.

**What this rule does not decide.** It says nothing about any stage's correctness gate, which JSD-0035
section 10 states and which each stage meets before this measurement is collected. It judges no Native
AOT image, no arm64 build and no machine but the one its manifest names, and the fresh-process shape
`fresh-small.js` is outside its population.
