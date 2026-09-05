<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle JSW-10-001 — the two workloads, whole, and the runs that say so

**Collected:** 2026-09-05. **Stage:** JSW-10. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** The [workload roadmap](../../roadmap.workloads.md)'s last stage asks for
three things: a whole-suite conformance run retained per manifest with every `unsupported` family
named, the Octane checkout pinned and archived so a benchmark result has an identity, and a lane
that runs both workloads on every claimed runtime identifier. This is the collection for the first
and the record of the other two. **It is not an acceptance** and nothing in it has been reviewed:
`assurance-release.log` is the release gate refusing because every relevant unit is
`HUMAN_PENDING`, and that refusal is the correct answer rather than a defect.

**It states figures, and every one is read off a retained file in this directory.** The ledger's
update rule 10 says a number with no retained record behind it is not a number this document family
may state; each below is a line of `test262-wide.report.gz`, `test262-slice.report.gz`,
`octane.log`, `frame-cost.log` or one of the control logs, and a reader can recompute it with no
network.

**`identity.txt` records the working tree as DIRTY, and it lists what.** All twenty-three entries
are this bundle's own files, marked `D` because the directory was removed before the collection that
replaced it. **No product or test source was modified**, which the list is there to let a reader
check rather than take on trust.

---

## 1. Octane runs whole

`octane.log` is `python3 eng/run-octane.py` with no arguments, on an otherwise idle machine. The
driver checks the archive at `src/tests/octane/pins/` against `octane.pin` before extracting a byte,
runs each benchmark in a process of its own through the ordinary command line of the end-user host,
and retains every line each printed.

**Fifteen of fifteen benchmarks reported a score and exited zero.** That is
[section 1](../../roadmap.workloads.md#1-the-target-stated-as-behaviour-rather-than-as-a-score)'s
target for this workload stated as behaviour, and it is the whole of what this bundle claims about
Octane: **the scores in the log are numbers about this configuration and authorise no comparison
with anything**, which the pin says of itself and roadmap section 17 governs. Octane is retired
upstream and the pinned commit is its retirement commit.

**Two of the fifteen are the rows this programme opened with.** `pdfjs` was refused by this
component's own verifier on bytes this component's own lowering produced, and `typescript` failed
with a type error against a value the program did not expect to be `undefined`; both are diagnosed
in [JSC-81](../../roadmap.corrections.md#jsc-81) and
[JSC-82](../../roadmap.corrections.md#jsc-82). **And `regexp` prints no checksum failure**, which
the ledger's `JS-6` row recorded as an approximation that neither refuses nor agrees for as long as
regular expressions were translated onto the platform's engine.

**The wall clock is a measurement hazard and this run was taken accordingly.** The host's bound is
real time, so a benchmark sharing a machine can meet a bound it would otherwise clear. An earlier
survey of the same fifteen, taken while three agents and a suite run shared four cores, is not in
this bundle for that reason.

---

## 2. The conformance suite, whole, per manifest

Both runs are `python3 eng/run-test262.py` against the pinned checkout, three shards at a time on an
idle machine, with `--expect` passed to every shard so no shard's report can name a revision the
shard did not itself read. The suite is `tc39/test262` at
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, digest
`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93` over 56,560 files, and each
run's header records that the checkout answers to `test262.pin`.

| | files | variants | pass | fail | **unsupported** | exhausted | skipped |
|---|---|---|---|---|---|---|---|
| `broiler.javascript.wide` | 53,469 | 94,545 | 71,153 | 14,987 | **0** | 59 | 8,346 |
| `broiler.javascript.slice` | 53,469 | 94,545 | 1,409 | 1,549 | **83,241** | 0 | 8,346 |

**The wide manifest's `unsupported` column is empty, and that is the claim this stage exists to
make.** Section 1 is precise about what it wanted and it is not that the column is hidden or
reinterpreted: *the manifest grows until the column empties honestly, one construct family at a
time, with every family that stays out named as an exclusion rather than absorbed into a verdict*.
There is no family left to name. The last four went in one change — a dynamic `import()`,
`import.meta`, an import attribute clause and an async generator as a module's default export
*(JSC-161 to JSC-166)* — and the one before them, `return` outside a function, was not a surface at
all but an early error being refused as though it were one *(JSC-158)*.

**The slice manifest's column is the same instrument pointed the other way, and it is why the wide
run's zero means something.** Forty-three families, 83,241 variants, each named in
`test262-slice.report.gz`: a call, an object literal, the class construct, a function, `new`, an
array literal, the generator construct, a property access, a string value, `try`. A manifest that
admits arithmetic over locals refuses nearly the whole of a language suite **by name**, which is
what an admitted-construct boundary looks like when it is doing its job.

### The verdicts that are neither pass nor fail

**Fifty-nine variants spent an allowance**, and the harness reports that as a fifth verdict rather
than folding it into the failures — 21 on the wall clock, 20 on fuel, 16 on nested-load fan-out and
2 on nested-load bytes, each named with an example in the report. **A failed column that silently
carried "we did not wait long enough" is a column nobody can act on**; this one can be acted on by
raising a number, and the transcript says which.

**Eight thousand three hundred and forty-six variants were skipped, and the accounting is not a
rounding.** 8,069 claim one of **eighteen distinct proposed features** the suite's own
`features.txt` marks as proposals rather than productions of the pinned edition — `Temporal` alone
is 6,674, then `explicit-resource-management`, `import-defer`, `source-phase-imports`,
`Intl.NumberFormat-v3`, `joint-iteration` and `ShadowRealm`. 275 are `_FIXTURE` files another test
loads and never runs on their own. **Two need an agent whose `[[CanBlock]]` is true**, which this
profile has no agent model for. A test about a language nothing here implements is not a test this
engine failed, and scoring one would be the false pass
[JSD-0018](../../decisions/0018-which-tests-are-about-this-language-and-who-decides.md) exists to
refuse.

---

## 3. What a declined surface does, which bundle JS-4-001 recorded as unmet

Three runs over named selections, each with one optional surface declined by the composition. They
are selections rather than whole runs because the property under test is a refusal at verification,
and the whole suite would buy a repetition rather than a reading.

| declined | over | variants | unsupported | pass | fail |
|---|---|---|---|---|---|
| `broiler.javascript.dynamic` | `test/language/eval-code` | 454 | 452 | 0 | 2 |
| `broiler.javascript.binary` | `test/built-ins/DataView` | 1,111 | 1,096 | 0 | 4 |
| `broiler.javascript.modules` | `test/language/module-code` | 750 | 424 | 170 | 3 |

**Every refused variant is refused at VERIFICATION with the surface named**, which is the property
[roadmap section 6](../../roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
describes and which bundle [JS-4-001](../js-4-001/README.md)'s exclusions record as unmet for
`eval`: the artifact declares a surface the composition declined, the core refuses it as an invalid
artifact, and the guest never sees a `ReferenceError` about a missing binding. **The modules row is
the one to read twice.** 170 variants still pass with the module surface declined, and they are the
script-goal variants in that tree — a script carries no module records, so it declares nothing, so
declining the surface says nothing about it. A decline that refused those too would be refusing
programs for a surface they do not use.

---

## 4. The call-depth margin, re-measured

`frame-cost.log` is the gate as it runs in an ordinary build: both recursions stop at the declared
bound of 5,999, which reports what the build **promises** rather than what the stack holds.

The capacity was then measured the way [JSC-139](../../roadmap.corrections.md#jsc-139) describes —
`JsEngine.MaximumCallDepth` and the profile's declared call-depth maximum lifted in a build of their
own, measured, and reverted:

- **19,694 calls** on the declared 96 MiB guest stack, about **5,111 bytes** of native executor
  frame each.
- **2.40 times** the 8,192-call ceiling a host may be granted, and 3.28 times the 6,000-call
  backstop.

**That margin is narrower than the 2.70 JSC-139 records, and the drop is not one change's.** It is
the accumulated cost of everything this programme merged — the module goal, the asynchronous
iteration family, the `Proxy`, the global lexical environment, the dynamic import. It is above the
factor of two below which a program granted the maximum could reach the stack before it reaches the
ceiling, which is the figure that would be a finding; **it is stated here because a margin that
narrows quietly is how the failure this bound exists against arrives.**

---

## 5. The component's own gates, collected here rather than asserted

- `build.log` — the whole solution in Release. Zero warnings, zero errors.
- `suite.log` — the whole test suite, **passing with no failures**. It did not until the morning
  this was collected: three assertions had been red since a second component was vendored under
  `src/Broiler.VM.HyperV/` without the rules that quantify over `src/` being told about it, and they
  were doing what they were written to do — the assurance scanner's own comment says a product
  project appearing in the tree fails there *until someone decides whether it is covered*.
  [JSC-175](../../roadmap.corrections.md#jsc-175) records the decision and whose it was.
- `assurance-gate.log` and `assurance-release.log` — the gate, and the release mode **refusing**,
  which is the correct answer while every relevant unit is `HUMAN_PENDING`.
- `publish-and-run.log`, `catalog-*.txt`, `closure-*.txt` — each composition root published as
  Native AOT and **run**, with its closure read off the published output rather than asserted.
- `fuzz.log` — the retained sessions, answer-guided and not coverage-guided.
- `hashes.txt` — the digests of the files a reader would have to trust.

### The control matrix, which reads as a matrix for the first time

**Forty-four controls ran and forty-four passed**: 29 judged by the whole suite, 13 by the corpus
replay, 2 by a fuzz session. Each is an injection into the real checkout, judged, and reverted, and
a control passes only when the judge **fails while the defect is present and passes after the
revert**.

**That second half could not hold until this morning.** A bundle collected a few hours earlier
reported twenty-nine controls run and none passed: every control judged by the suite needs the suite
to go from failing to passing, and it was failing for a reason none of them touch. **A control
matrix that cannot pass is not a weaker matrix; it is no matrix at all**, and it had been that way
for as long as the vendored component had been in the tree.

**Two controls are skipped, and that is a GAP rather than a smaller total**, stated in
`android-controls.log` in those words: there is no Android SDK on this machine, so neither was
injected. Nothing here may be read as covering the Android head.

**The corpus replay found a defect on its way to that matrix**, and it is the only gate that could
have: it replays retained BYTES rather than compiling source, and an opcode this branch renumbered
during a merge left one stored artifact meaning a different instruction from the one it was written
to test. Every other gate compiles through the current lowering and saw nothing.
[JSC-176](../../roadmap.corrections.md#jsc-176) records it, together with a second reading from the
same replay that is **not** a defect: the soak's plateau check read 1.79 against a band of 1.2 while
the machine was loaded and 0.96 at rest on the same commit, and it says of itself that it is a
plateau check and not a measurement.

`negative-controls.log.gz` is compressed for the reason the two whole-suite reports are: one rule
reports thousands of messages on each injection that makes it fail, and the uncompressed log is
fourteen megabytes of the same evidence.

---

## 6. What this bundle does NOT show

- **It shows no acceptance.** No reviewer decision exists, and the release gate refuses.
- **One machine, one runtime identifier.** Both whole-suite runs, the Octane run and the frame-cost
  measurement are `linux-x64` on one Linux host. The lane runs both workloads out of the published
  image on every claimed identifier, but **no identifier but this one has scored the whole suite**,
  and the lane's own step says so.
- **No Android control was injected**, for want of an SDK on this machine.
- **It shows no throughput claim and no baseline.** Roadmap section 17 governs any retained figure
  and JS-10 owns the measurement lane; the Octane scores in the log are numbers about this
  configuration.
- **The failing column is not diagnosed here.** 14,987 variants fail under the wide manifest and
  this bundle names none of them beyond what the report carries. What it claims is that they are
  failures rather than absences — the distinction the `unsupported` column exists to keep.
- **`broiler.javascript.regexp` is not declared**, a clause of JSW-4's gate left deliberately unmet
  and recorded as owed in [JSC-167](../../roadmap.corrections.md#jsc-167), together with the
  IL-emission metadata test that is also unasserted.
- **The `eval` surface's direct form stays refused inside a function**, a published exclusion of
  this manifest rather than a gap, and the largest single cluster of the failing column that is not
  a defect.
- **Two named divergences remain in the differential probes**, declared there rather than here:
  evaluated source gets this realm's lexical environment instead of one of its own, and a `var`
  created by eval code is not configurable — both the same seam, recorded in
  [JSC-142](../../roadmap.corrections.md#jsc-142).
