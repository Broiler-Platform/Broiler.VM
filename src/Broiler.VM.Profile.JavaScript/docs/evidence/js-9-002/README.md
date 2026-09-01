# Evidence bundle JS-9-002

> ### Correction, 2026-09-01 — this bundle's summary contradicted its own log
>
> **What this file said.** That `publish-and-run.log` shows *"6 publishes, 6 runs, all exit 0"*.
>
> **What the log shows.** `[executiononly/aot] run exit 1`, and
> `broiler-js-execution-only: 1 of 18 checks FAILED`. **Five of the six runs exit 0.** The failing
> check is `recycled-runtimes-reach-a-heap-plateau`, under **Native AOT only**: the heap goes from
> 68,608 bytes after 100 cycles to 157,720 after 2,000, a factor of **2.30 against a band of 2.0**.
> JIT and trimmed pass the same check on the same code, at 0.95 and 0.93.
>
> **No log byte was edited.** The logs were always right; this file's summary of them was not, and
> the summary is what a reader reads first. The rows below are corrected in place and an exclusion
> is added; what was retained is untouched.
>
> **Why it is recorded here rather than only in the ledger.** A bundle whose summary reports the
> passing half of its own run is the failure mode this component's whole method exists to prevent,
> and a correction filed somewhere else would leave the bundle still saying it.
> [Ledger update rule 2](../../roadmap.status.md#5-update-rules) is the rule that was broken:
> *state what it demonstrated, **including its failures and its exclusions***.

**Milestone:** JS-9. **What this bundle adds to [JS-9-001](../js-9/README.md):** the two
**host-level exercises** its exit gate names — sibling runtimes under one aggregate budget, and a
soak over recycled runtimes.

**Verdict this bundle supports:** JS-9 is **In progress**, not accepted. The corpus is still
slice-scope rather than full-format, two of the four fuzzing surfaces do not exist, there is no
retained-bytes report over an object model that does not exist, there are no agents, and **nothing
here has been reviewed**.

Produced by `eng/collect-js-evidence.py`. Every file beside this one is its output.

**No result from any other component is evidence here — and for these two exercises that rule is
load-bearing rather than ceremonial.** The core has contract tests of exactly this shape. They run
over a *fixture* profile, and what these exercise is whether **this** profile's verifier, executor
and instance state behave under a shared parent and across recycling. Ledger update rule 6 says a
core result never advances a row here; this is what that costs and what it buys.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-9-002` |
| Milestone | JS-9 |
| Adds | The aggregate-budget exercise and the soak |
| Corpus | 60 entries, unchanged from JS-9-001 |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

## Two siblings under one parent

| Check | What it observed |
|---|---|
| `two-siblings-under-one-parent-spend-one-total` | **28 invocations completed and 100 were refused** across two runtimes; the parent spent **exactly 4,000 of 4,000** fuel |
| `a-parent-with-a-live-child-refuses-disposal` | With one live child the parent answered `InvalidState`/`AggregateBudgetHasLiveRuntimes`; after the child was disposed, `Accepted` |
| `a-sealed-parent-admits-no-further-runtime` | Sealing answered `Accepted`; a runtime created afterwards was refused with `ResourceExhaustion`/`ParentExhausted` |

**One clause of the gate is about what is *not* asserted, and it is the sharpest thing here.** The
exit gate says *no test asserts which sibling observes a shared-parent exhaustion*. Both siblings
draw on one total and the order they reach it in is a race; a check that named a winner would be a
check that passes on one machine and reports a defect on another. What is asserted is the total,
that at least one sibling was refused, and that at least one completed.

### What the core refused, and why it is recorded

The first version of this exercise had each child ask for the **profile's defaults** under a parent
holding four thousand fuel, and **every child was refused at creation** with
`ExceedsParentRemaining`. That is the core doing its job: a child adopts the parent's *remaining*
on every dimension the parent bounds, and asking for more than the parent has is a creation-time
refusal rather than a runtime surprise. It is written into the check's own comment because it is
the mistake the next person writing one of these will make.

## The soak

| Mode | What it observed |
|---|---|
| JIT | **2,000 of 2,000** create-verify-instantiate-invoke-dispose cycles completed; the heap went from **81,776 bytes after 100 cycles to 77,808 after 2,000** — a factor of **0.95** against a band of 2.0. **PASS** |
| Trimmed | 2,000 of 2,000 completed; **56,400 → 52,448**, a factor of **0.93**. **PASS** |
| Native AOT | 2,000 of 2,000 completed; **68,608 → 157,720**, a factor of **2.30** against a band of 2.0. **FAIL**, and the run exits 1 |

**The third row is the one this file used to omit.** All three modes run the same check over the
same code and the same corpus; two settle below their starting point and one grows by a factor
of 2.3. Whether that is a per-cycle retention this profile causes under Native AOT or a band
that is wrong for an AOT heap is **not answered here and is not answerable from this bundle** —
it needs a measurement, and JS-5 owns measurement. What must not happen is the band being
widened to make the check pass: [section 17](../../roadmap.gates.md#17-measurement-discipline)
names *an envelope widened after seeing a candidate* as a measurement defect, and a plateau
band is an envelope.

**The band is loose on purpose and the check says so in its own text.** A managed heap does not
return to a number, it returns to a range, and a check comparing two byte counts for equality would
be a flake generator. What this is written to catch is unbounded growth across two thousand cycles
— which is what a per-cycle leak looks like — and not a regression of a few kilobytes. **It is a
plateau check and not a measurement**; JS-5 owns measurement and section 17 owns its rules.

The band is taken from a heap that has already seen a hundred cycles, so what is compared is steady
state against steady state rather than steady state against a process that has just started.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 138 architecture tests passed |
| Assurance gate mode | `assurance-gate.log` | Passed |
| Assurance release mode | `assurance-release.log` | **Refused, as it must** |
| Publish and run, both roots × three modes | `publish-and-run.log` | 6 publishes; **5 of 6 runs exit 0**, 18 checks per run — the Native AOT run of the execution-only root **exits 1 on one check**, see the correction above |
| Fuzz sessions | `fuzz.log` | 4 sessions, 100,000 iterations, 0 findings |
| Negative controls, suite-judged | `negative-controls.log` | 16 injected, 16 caught, 0 skipped |
| Negative controls, corpus-judged | `corpus-controls.log` | **9** injected, 9 caught, 0 skipped |
| Negative controls, fuzz-judged | `fuzz-controls.log` | 1 injected, 1 caught, 0 skipped |

**Both exercises run in every published mode** — which is how the plateau's Native AOT
failure was observable at all, and why it is above rather than absent. They are observed under
Native AOT as well as
under JIT.

### The control for this bundle

`the-executor-stops-charging-fuel-per-step` removes the interpreter loop's per-step fuel charge.

**Exactly one check failed, and it is not the corpus.** A counting loop still returns 55, every
replay row still agrees, and the ordering checks are unmoved — because nothing about any program's
*answer* changed. What broke is the claim that rests on a budget being spent:

> two-siblings-under-one-parent-spend-one-total: **128 invocations completed and 0 were refused**
> across two siblings; the parent spent **128 of 4000** fuel

The 128 is the instantiation charge; the steps are free. This is the same shape as the ordering
control in [JS-1-002](../js-1-002/README.md): a defect that leaves every recorded answer intact,
and that only an exercise built for it can see.

## Exclusions — what this bundle does not show

1. **The corpus is slice-scope, not full-format.** JS-9's gate asks for the malformed corpus grown
   to the whole format; the format has not grown, because the manifest that would grow it is JS-4's
   and JS-6's.
2. **Two of the four fuzzing surfaces do not exist** — the source tokenizer and parser, and the
   regular-expression matcher. Unchanged from JS-9-001.
3. **No retained-bytes reporting.** The gate asks for it over the object model, and there is no
   object model.
4. **No agents.** The realm and agent surface is JS-8's.
5. **No session or soak budget.** Four seeds, 25,000 iterations each, 2,000 cycles: stated so a run
   is reproducible, not because any of them is a number something justifies. JS-9 owns choosing.
6. **The plateau is a band, not a figure.** No number in it may be cited as a measurement.
7. **The plateau check FAILS under Native AOT and this bundle does not close it.** The run exits 1;
   JS-9's gate clause asking that a soak reach a stated heap plateau is **open on that mode**, and
   the ledger's JS-9 row carries it. Nothing here diagnoses it.
7. **One RID, one machine.** `win-x64`. A heap number on one machine is not a heap number.
8. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
