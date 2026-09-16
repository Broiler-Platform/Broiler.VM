# Evidence bundle VM-5-002

**Milestone:** VM-5 - baseline the core's own overhead
**Collected:** 2026-09-16, and incomplete. This commit retains the predeclared rule, the conformance
parity rule and the two base test262 wall-clock lists, and nothing else. Every other section below
says what it is waiting for.
**Core contract version:** 1, unchanged. Fuel pre-admission adds no public member, mints no
amendment and adds no member to the meter interface.
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition.

> **No language performance claim follows from any figure in this bundle.** Every measurement is of
> the **core's own overhead** around a fixture profile whose executor is a toy stack machine, or of
> one JavaScript shape on one workstation. A real language profile's cost is its own, and nothing
> here predicts it.

**Why this commit exists before the changed build was measured.** A rule written after the numbers
are in is not a rule, it is a description. The subject of this bundle is a change made for speed, so
the reading of its evidence is fixed here, in the repository, before the changed build is measured:
what must hold, cell by cell, for the change to be described as faster, and what is named as failing
when a cell does not hold. The two base test262 lists in section 5.3 are part of the same
predeclaration - they are the rows the parity rule will later admit a difference on, they were taken
from the unchanged build, and they are committed before the changed build runs the suite once.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry. This collection is partial and
the table says so per field.

| Field | Where | State at this commit |
|---|---|---|
| Identity | Section 1 | Written |
| Source | Section 2 | To be collected |
| Dependencies and corpus | Section 3 | To be collected |
| Environment | Section 4 | To be collected |
| Procedure | Section 5 | The predeclared rule, the parity rule and the two base wall-clock lists are written; every command's transcript is to be collected |
| Outputs | Section 6 | To be collected |
| Decision | Section 7 | To be collected |
| Validity | Section 8 | To be collected |

Section 9 carries the exclusions and is likewise to be collected.

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-5-002 |
| Milestone | VM-5 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-5 gate |
| Core contract version | 1, unchanged |
| Reason-registry revision | 2, unchanged |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

**The commits this bundle is about**, on branch `claude/fuel-credit-block-steps`:

| Commit | Subject | What it carries |
|---|---|---|
| `cd362ac` | Pin fuel charging to the unit before changing how charges are applied | The fixture profile's windowed-polling variant and its environment observer, the fixture executor's windowed poll and its guest-load fuel ceiling, five fixture artifact writers, and the sixteen fuel-exactness contract tests. The meter is unchanged at this commit, which is what makes those tests an oracle rather than a description |
| `ba760d9` | Make the fuel-exactness tests fail on the claims they guard rather than on their timing | An amendment to the same tests, still on the unchanged meter |
| `e117162` | Admit fuel without the meter's lock inside a block checked at every level | `VmFuelPreAdmissions`, the `VmMeter` fast path and its split from the locked charge, the pre-admissions property on `VmBudgetLevel`, and the settle points in `VmRuntime`, `VmInstanceImplementation` and `VmInstantiation` |
| `a416c7a` | Resolve the ambient meter once per execution context | The resolution cache in `VmExecutionScope`, keyed by execution context |
| `4490eda` | Request a guest load from a profile that polls on a window, so the remainder it is verified under is read with fuel outstanding | A further test |
| `738d9c7` | Answer a zero-unit fuel charge without a locked write, and make each settle say what it alone guarantees | A zero-unit fast path, and the remark on each settle point saying what that point alone guarantees |
| `c98011a` | Hold five steps of one runtime open, and suppress a flow on a thread that is in no step | A further test |
| this commit | Predeclare the rule the fuel pre-admission evidence is read against | Section 5 of this README |

**The base for every comparison is commit `f127d92`**, "Pin every clause of the rule the granularity
harness applies, and refuse a shape with no twin" - the last commit on this branch before the meter
changed. It already carries the fuel-exactness tests and their amendment, so those tests are present
and passing in both builds, and it carries the per-block-steps harness commits, which touch no
product assembly.

The design this work follows names an earlier commit as the base, because it was written before
those test and harness commits landed. That commit's tree differs from `f127d92` only by them, and
`f127d92` is the commit that was actually built and run, so it is the base named here.

The base build is a worktree **outside** the repository, at `D:/Broiler.VM-base`, because a worktree
inside the checkout fails architecture rule A14 for tests run in the main checkout.

---

## 2. Source

To be collected. It will record what the change touched in the product assemblies, and the state of
the working tree when the evidence was retained.

---

## 3. Dependencies and corpus

To be collected. It will name the test262 pin, the Octane pin and the JavaScript corpus. The suite
revision the two base runs were taken against is stated with those runs in section 5.3.

---

## 4. Environment

To be collected.

---

## 5. Procedure

Every command of the collection, with its transcript, is to be collected. What is written here now
is what had to be written **before** the changed build was measured: the rule the timing evidence is
read against, the rule the conformance parity is read against, and the two lists the parity rule
admits a difference on.

Throughout this section members and types are named rather than source lines, per rule H3.

### 5.1 The predeclared rule, committed 2026-09-16

This is the rule. It is committed before any timing run of the changed build, and before the changed
build has run the conformance suite at all.

1. **Every correctness result holds.** The build is clean with warnings as errors; every test project
   passes; the generated assurance and review records are regenerated and the public API file is
   unchanged; the fuel-exactness tests pass unchanged on both builds; every injected-defect witness
   fails the test it names and passes again when reverted; both profile assemblies are byte-identical
   between the two builds under a deterministic build; the JavaScript checks, the corpus replay and
   the host-lifetime checks pass in both builds with identical verdicts; the conformance parity of
   section 5.2 holds; the low-fuel runs over the named subtrees agree row for row in both forms; and
   the fuel minima measured through the command-line host are identical in both builds and both
   forms.
2. **Shapes.** For each shape and form, the credit build's median is at or below the base build's,
   and the gap is larger than the A/A lane's spread.
3. **Concurrency**, three parts, all of which must hold:
   - in the probe's `concurrent-bench` mode, the credit build's nanoseconds per admitted charge is
     at or below base in every cell with two or more threads - threads 2, 4, 5 and 8, at each
     runtime ceiling measured;
   - in the probe's `concurrent-ambient` mode, the build carrying the execution-context resolution
     cache is at or below the build carrying only the pre-admission table, at every thread count of
     two or more;
   - the median duration over five runs of the concurrent exactness test alone, alternating builds,
     is at or below base on the credit build.
4. **The per-instruction meter row.** In the bench host, `meter-per-instruction` for credit is at or
   below base plus the A/A spread. That row polls after every charge, so each admitted block covers
   exactly one charge: it measures the settle and the re-admission that moved into the poll, not the
   saving the change was made for. Passing it shows only that a profile polling after every
   instruction does not regress.

**What a failure does.** A cell that fails item 2, 3 or 4 is named as failing in this bundle, and
nothing in this repository may describe the change as faster for that cell. A `concurrent-ambient`
failure triggers the resolution cache's documented fallback. A `concurrent-bench` or concurrent-test
failure triggers a change to the pre-admission table's capacity, its block cap or its share divisor.
Either way the change is made, the measurement is taken again, and **both** runs are retained - the
failing one is not dropped.

**No registered baseline funds this change.** VM-5's gate funds optimisation only against one of its
baselines, and the only baseline touching this path is the per-instruction meter row, which by item
4 cannot show the saving. The shape measurements and the core-charge measurements are bundle-local.
No new baseline is registered for this work and the figures in `docs/baselines.md` are not edited;
that gap is recorded as an exclusion in section 9 rather than repaired.

### 5.2 The conformance parity rule

Four whole-suite runs of the pinned test262 checkout: the base build and the credit build, each in
the bytecode form and in the emitted-machine-code form, every one under the wide manifest at the
default fuel allowance and a 60,000 ms wall clock.

**The rule, per form.** The sorted `result` rows of the base report and the credit report are
identical, **except** on rows in that form's predeclared list in section 5.3, where the credit row
may be `Passed` or an exhaustion of another allowance. Every other difference fails this rule and is
recorded as failing - including a credit row exhausted on `WallClock` that is not in that form's
list, and a listed row that became `Failed`.

**The comparison script is retained, not the gate.** The form-comparison script is run and its
output retained, but every row it names is checked against the list by hand. That script is written
to hold a bytecode run against a machine-code run and admits no wall-clock difference at 60,000 ms
or more, so a listed row that legitimately changed makes it exit non-zero; that exit is not itself a
failure of this rule.

**Within each build**, the bytecode-against-machine-code comparison names the same set of rows in
the base build as in the credit build, apart from rows in either form's list.

**Why the lists exist at all.** A wall-clock exhaustion is the one verdict in the report that is a
property of the machine rather than of the engine: the same variant on the same build can spend the
allowance on one run and finish on the next. Fixing the lists before the changed build runs is what
stops that verdict from being read backwards later - as a row the change repaired, or as a
regression the change caused.

### 5.3 The two base wall-clock lists

Both lists are read off the merged whole-run report of a base run. Both runs were taken from the
build in `D:/Broiler.VM-base` at commit `f127d92`, against the checkout the retained suite pin
admits - `tc39/test262` at revision `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, whose 56,560 files
hash to the content digest that pin declares - under manifest `broiler.javascript.wide` with no
surface declined, at an allowance of 100,000,000 fuel and 60,000 ms per variant, in 56 shards across
14 processes. Each run's merged report states that it may be retained: pinned, whole, and its five
verdicts account for its variants. Every shard transcript of both runs is retained outside this
bundle until the bundle is completed.

The two forms exhausted the wall clock on the same variants, and on no others.

**Bytecode form.** Two rows.

| Path | Variant |
|---|---|
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | sloppy |
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | strict |

**Emitted-machine-code form.** Two rows.

| Path | Variant |
|---|---|
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | sloppy |
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | strict |

Nothing else in either run spent the wall clock. Every other exhaustion in both runs was on a
deterministic allowance - fuel, the nested-load fan-out or the nested-load byte count - and those
rows are held to exact equality by the rule in section 5.2 like any pass or failure, with no
tolerance at all.

---

## 6. Outputs

To be collected. The core and fixture figures may be quoted here, each equal to its retained log
value. The JavaScript shape measurements, the Octane reports and the test262 reports are retained
and **not quoted**, as the core release gates require and as the JavaScript bundle before this one
did.

---

## 7. Decision

To be collected. It will carry the VM-5 gate clause by clause, the exactness argument in condensed
form with every part resting on argument or on sampled runs alone named as such, and the evidence
verdict for each item of the rule in section 5.1. Reviewer: none.

---

## 8. Validity

To be collected. The recertification triggers will be any change to a hashed file, any change to the
metering path, and any change of machine or SDK.

---

## 9. Exclusions

To be collected. It will carry forward the exclusions of the earlier VM-5 bundle that still apply,
and add the new ones this work needs, numbered after the highest exclusion identifier in the
repository at the time the bundle is completed. Among the new ones will be the VM-5 funding clause
recorded in section 5.1 and the timing signal a shared ceiling leaves between concurrent operations
of one runtime. No exclusion identifier is cited in this commit, because an identifier this bundle
has not yet defined would be a citation of nothing.
