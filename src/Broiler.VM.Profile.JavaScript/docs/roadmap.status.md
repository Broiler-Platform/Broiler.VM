# Broiler.VM.Profile.JavaScript roadmap status

**Last updated:** 2026-09-01

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[JavaScript profile roadmap](roadmap.md). The roadmap defines planned work and objective exit
gates; this ledger records whether those gates have accepted evidence. Where implementation or a
dated decision replaced something the roadmap used to say, the plan carries the new reading and
[the corrections and rejections](roadmap.corrections.md) carry what it replaced — **that file
records no status and advances nothing here**.

**At this snapshot, JS-0, JS-1, JS-3a and JS-9 are `In progress`, JS-2 is blocked, and the
remaining rows are `Not started`.** What exists is one feature manifest, one format version, a
verifier, an executor, a descriptor admitted by a catalog, a hand-written lowering, two composition
roots that publish on one RID under JIT, trimming and Native AOT — and whose **one retained Native
AOT run exits 1**, which the next paragraph is about — a retained corpus, a published diagnostic-code
registry, a frozen public-API baseline, a decision series, this profile's own group in the rule
register, retained evidence bundles, and a fuzz target with its negative controls — each counted
by section 2 below and by the record that holds it, not here. There is
**no tokenizer, no static-semantic stage, no object model, no standard library, no suspension, no
guest-initiated load, no snapshot and no conformance harness**. No milestone is complete because
its design appears in the roadmap, **nothing here has been reviewed by a human**, and nothing in
this component may be described as validated, accepted or supported.

**One retained claim was withdrawn on 2026-09-01, and it is a correction to a summary rather than a
change of state.** Both JS-9 bundles reported their publish-and-run as *six runs, all exit 0*, and
both bundles' own `publish-and-run.log` records the execution-only root's **Native AOT run exiting
1** on the soak's plateau check — five of six. Each bundle now carries a dated correction beside
that row, no retained log was edited, and the JS-9 row below carries what is open because of it.
**Nothing moved**: JS-9 was `In progress` before and is `In progress` now, with one more clause
named. It is recorded up here rather than only in a row because the failure mode is this
component's own — a summary that reports the passing half of a run it retained in full — and
update rule 2 is the rule it broke.

**The check that produced that exit has since been corrected, and the distinction between the two
sentences is the whole of what this ledger is for.** The working tree's soak passes in every publish
mode today; **no retained bundle shows it doing so**, because none has been collected since the
correction. A reader must not read the first fact as the second: what a row here may cite is a
bundle, and the newest one retains an exit 1. The JS-9 row carries both halves.

**The placement decision is taken.** This component is not a repository of its own and is not a
component of its own: it is a family of product projects inside `Broiler.VM`, at
`src/Broiler.VM.Profile.JavaScript*`, with its roadmap and decisions in the profile assembly's own
project directory. The profile's half is [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md);
the core's half is ADR 0001 revision 5, which authorises the three projects and revises rule A11
so that a profile may reference its own format sibling. **Three things the roadmap assumed would be
this component's own are the host component's** - the assurance system and the rule register in
which this profile holds group N, each recorded as a dated deviation in
[JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md) rather than dropped, and the
licence and notice files, which
[JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) records as the host's and
which no deviation covers because adopting them costs this profile nothing. **A fourth,
the API baseline, was adopted at JS-0 and became this family's own at JS-3a**, because the host's
describer cannot reach a profile assembly without a project reference rule A11 forbids;
[JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records it and
rule N10 holds it. **What is not shared is evidence:** a JS bundle is cited only by this ledger, a
core bundle only by the core's, and update rule 6 below is unchanged.

---

## 1. Reading this ledger

Four categories must remain distinct, and conflating any two of them is how an unfounded claim
gets recorded:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation evidence and not validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout — for instance
  that this component contains no project file. It can explain a status; it cannot satisfy a
  future implementation, contract, conformance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable bundle that identifies the exact sources and
  gate, records the executed commands and environment, retains their outputs, and demonstrates
  every part of the objective exit gate. Only accepted evidence may advance a milestone to
  `Accepted`.
- **Inherited material** is anything copied from the seed. It carries **no status of its own**.
  A copied file is unvalidated and unreviewed in this component on the day it lands, however long
  it has existed elsewhere.

A fifth thing is deliberately **not** a category here. **A correction to the plan is not evidence
and does not appear in this ledger's tables.** When implementation invalidates, rejects or
re-scopes something the roadmap said, the roadmap is edited and
[the corrections and rejections](roadmap.corrections.md) record what it said before — a change of
*plan*, never a change of *state*. A milestone whose scope was corrected has moved no row here,
and a row here moves only on evidence.

**Work in other components is not this component's evidence.** In particular, no conformance
result, benchmark, measurement, review decision, or Native AOT sample produced by the legacy
JavaScript engine component or by the Broiler.VM core establishes anything here, and no gate in
this ledger may cite one. That rule is not a courtesy to the fork; it is what makes a number in
this file mean something.

**This document is under the host component's review-document rules, and was not until JS-3a.**
Those rules built their corpus from the host's own `docs/` and this ledger lives elsewhere, so the
clauses that exist because a reviewer reads these documents — no citation of a source line number,
a closed mark vocabulary, every cited exclusion defined — governed a ledger a profile reviewer
never opens and not this one.
[JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md) closes that, and
records the two clauses that still do not reach this profile's bundles and what would close each.
**Section 2's mark table is this document family's legend**, read by the rule rather than restated
by it.

**One consequence for earlier evidence, stated rather than left to be inferred.** The architecture
suite now reads more documents than it did, so a suite total collected before JS-3a is a total over
a different test corpus. Bundles [JS-0-001](evidence/js-0/README.md) and
[JS-1-001](evidence/js-1/README.md) are unaffected in what they demonstrate — no rule either of
them cites changed — but each `suite.log` is a run of a suite that did not read this ledger.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, its holder, and its unblock condition must be recorded. **Lack of scheduling is not a blocker; an unaccepted upstream contract is.** |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and an owner and reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and the decision record must be linked; evidence history is retained. |

---

## 2. Current milestone status

The leading column is an **evidence verdict** — the author's mark about what a row's retained
evidence shows. It is not a reviewer's finding and not a change of state.

**This table is this document family's mark legend, and rule H1 reads it.** The vocabulary is
closed and has three members; a mark used anywhere in this profile's review documents that this
table does not publish is a rule violation, and so is a mark from the component's own nine-member
legend — the two vocabularies say different things and a reader must not have to guess which one
a mark came from. [JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md)
records the split.

| Mark | Meaning |
|---|---|
| `[NONE]` | The row has retained evidence of no kind. |
| `[PARTIAL]` | The row has a retained bundle that demonstrates some of its exit gate, with every unmet clause named in the bundle's own exclusions. **A `[PARTIAL]` row is not a qualified pass.** It is a row whose gate is open, and the named clauses are what is open. |
| `[FULL]` | The row's bundle demonstrates every exit-gate clause. It is still not `Accepted`: acceptance additionally needs an owner and a reviewer decision, which nothing here has. |

Four rows are `[PARTIAL]` and the remaining eight are `[NONE]`.

**The milestone set changed on 2026-08-31 and this table now carries the new shape.** What was one
`JS-3` is now `JS-3a` and `JS-3b`, split by dependency rather than by size: the conformance harness
needs a scoring target and not a copied front end, so leaving it fused put this component's only
external correctness signal behind both of the blockers in section 3 when it needed to be behind
neither. Twelve rows, not eleven. Nothing is accepted under either shape, so the split changes no
evidence claim — it changes what a reader is told is schedulable today. The split is recorded as
[JSC-15](roadmap.corrections.md#jsc-15) and the delivery file carries the same shape.

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [PARTIAL] | **JS-0 — boundary, placement, identity, assurance floor** | **In progress** | [Bundle JS-0-001](evidence/js-0/README.md): Release build of the whole solution with 0 warnings; the whole suite green; the assurance gate green and the assurance **release** mode refusing while naming each blocking declaration individually; **8 negative controls, each failing the suite when injected and passing after revert**; the candidate seed identity re-derived and matching on all four revisions. Seven decision records, [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) through [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md). Rules N1–N4 registered Active with nine witness inputs. | **One exit-gate clause is open; the other was discharged at JS-3a.** (1) **Open**: the two-profile catalog test's `eval`-refusal half needs guest loads and is carried to JS-8. Its descriptor half was discharged at JS-1 in both directions. (2) **Discharged 2026-08-31** by rule N10 and bundle JS-3A-002: the family's public surface is frozen in a baseline of its own, described from the build output without loading anything, and compared in both directions. [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records why the clause was re-homed to JS-3a rather than left at JS-3b. **Discharged is not accepted**: JS-0 still needs a reviewer decision that nobody has made. |
| [PARTIAL] | **JS-1 — the whole contract loop on a narrow slice** | **In progress** | [Bundle JS-1-001](evidence/js-1/README.md). `broiler.javascript.slice` is minted and format version 1 defined, carrying framed sections, a tagged constant pool, fixed instruction boundaries, **exception regions and suspension targets reserved and refused**, a canonical position table and declared maxima checked before use. All seven core-facing types are implemented. The descriptor is filled in one full-arity construction, admitted by a catalog, and **four named negative cases each provoke a refusal**. **All five verifier outcomes** are produced by named entries of a 51-entry retained corpus which replays twice with no residue, contains 16 passing controls, and on which the verifier throws nothing. **Four of the five execution-step kinds** are produced by named checks. **Two composition roots publish AND run on `win-x64` under JIT, trimmed self-contained and Native AOT**, warnings as errors, closures read off the published output: six managed assemblies for the execution-only image and seven for the compiler-bearing one, **differing by exactly the lowering**. JS-0's carried two-profile catalog clause is discharged in both directions. **Twelve negative controls**, four of them judged by the corpus rather than by the suite. **A third composition root landed on 2026-09-02 and it is an application rather than a console program**: an Android head composing exactly what the execution-only root composes, running that root's own check source on a booted Android system, retained as [Bundle JS-ANDROID-001](evidence/js-android-001/README.md) — 66 corpus entries replayed to their recorded answers, twice, plus the four ordering assertions, on **Mono rather than CoreCLR**. It is the first evidence in this profile from a runtime that is not CoreCLR and it claims nothing about Native AOT, about trimming or about a device: the collection is on an emulator and the bundle's exclusions say so first. That bundle recorded having **no negative control** and named it a gap; [Bundle JS-ANDROID-002](evidence/js-android-002/README.md) closes it with two, judged by a run on the device — one flipping a byte in the resource extraction the first bundle's own argument rests on, one making division by zero a fault in the profile, which the device replay catches by name on four entries. **Two controls are not a control matrix**: nothing else about that head is controlled, and its first run reported both as passing when the harness was the thing that was broken, which the second bundle records rather than smooths. Decision [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) records the entry-point answer and four corrections to earlier records. | **Two things, and the second was found rather than carried.** (1) **The exit-gate clause JS-1-001 carried is discharged**, at JS-3a rather than at JS-3b. The obstacle was real — `ApiSurface` describes a surface by loading an assembly, which needs a project reference, which rule A11 forbids a test project to have on a profile — and the first of the two routes the bundle named is now taken: rule N10 describes the family from its build output with `MetadataLoadContext`, which reflects **without running anything**, so it needs neither the reference A11 forbids nor the execution invariant 2 forbids. Bundle JS-3A-002 retains it. The clause moved because JS-3b is blocked on JS-2 and this needed neither; [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records that. (2) **Roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s third discipline was never implemented, and no bundle had said so.** The section names three orderings and asks that they be asserted mechanically *for every corpus entry, including every failing one*; JS-1-001 observed one ordering, and it is a different one. [Bundle JS-1-002](evidence/js-1-002/README.md) lands them, and grew the corpus by the entry the third discipline needed — which is the step from JS-3a's count to the one JS-9 is seeded from, recorded here because a reader tracing the corpus across three rows would otherwise meet an unexplained increment. **JS-1 is still not accepted**, because no reviewer decision exists.  `Suspended` is declared unreachable and produced at JS-7; five descriptor rows are provisional pending JS-5's measurements; one RID, one machine. |
| [NONE] | **JS-2 — seeding snapshot and front-end ingest** | **Blocked** (recorded as `Not started` above the blocker, because no work has begun either) | None. No snapshot has been taken. The candidate identity in roadmap [section 4.1](roadmap.md#41-the-snapshot-identity) is a recorded candidate, not a taken snapshot. | **Blocked on two named external dependencies.** See section 3. |
| [PARTIAL] | **JS-3a — diagnostic registry, position encoding, pinned suite, the oracle** | **In progress** | [Bundle JS-3A-001](evidence/js-3a/README.md), which is **the registry half of this milestone and not the oracle half**. [`docs/diagnostics/registry.txt`](diagnostics/registry.txt) is published at revision 1, one row per code, each naming the member that declares it, **the one core reason every emission carries**, the stage that refuses, **which half of the registry it belongs to**, the case that reaches it, and the revision its meaning dates from. **Five rules, N5 through N9, bind it to four independently written artefacts** — the code vocabulary, every emission site in the profile assembly, the retained corpus, and the composition's deliberately restated constants — plus the position factories, so no one edit can make it agree with everything. The corpus grew from 51 to **59 entries** to close the backward binding, and **37 of the 40 rows are reached by a named entry**. Decision [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) records the registry, its two halves and the position encoding; the encoding is pinned by four corpus rows through a new manifest column, and landing it **corrected a conflation in which every link- and walk-stage diagnostic reported a code-section offset under the artifact-relative marker**. `EntryStackNotEmpty` was declared at JS-1 and emitted by nothing; it is refused on the edge now, which also removes an order-dependence in which code an artifact provoked. **Twenty-two negative controls**, seven of them judged by the corpus rather than by the suite. **And one clause that was not this milestone's** — the public API baseline, open since JS-0 and parked at JS-3b behind two blockers it did not need — is discharged here by rule N10 and a baseline of the family's own, retained as [Bundle JS-3A-002](evidence/js-3a-002/README.md), per [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md). | **The oracle half of the exit gate is untouched, and it is the larger half.** No suite revision is pinned, no harness, self-check, sharding, merge, audit or scope tooling exists, no per-host-mode totals are published and no ratchet is set; the suite-revision dependency in section 3 is still open and a human has to retrieve, hash and archive the suite before it can close; **the ingested suite's attribution row and the core's standing-claim confirmation travel with that ingestion and are open with it** ([JSC-30](roadmap.corrections.md#jsc-30)). Within the registry half: **three rows are reachable from no artifact**, named and reasoned about in JSD-0009 with the admitting list held in rule N7 rather than in the registry; **no `embedder-seam` code exists**, because the front end that would mint one is JS-3b's, so that half of the split is declared and not exercised; four corpus rows pin a position and fifty-five pin none; one RID, one machine. |
| [NONE] | **JS-3b — static semantics as one verification stage, and the lowering** | **Not started** | None. No consolidated early-error stage, no strict-mode ruling, no lowering, no recorded answer for where the verification boundary falls. | After JS-2, and after JS-3a supplies the registry its diagnostics land in. Record the boundary decision of roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) before writing the stage that depends on it. |
| [NONE] | **JS-4 — value representation and object model** | **Not started** | No implementation of any kind: no object model, no property storage, no string, no frame object and no executor change. **What exists is the entry gate, taken on 2026-08-31**: decision [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) records all eight rows of roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model)'s ABI, each with what it buys, what it costs and what would falsify it. **A taken entry gate is not a started milestone** and this row does not move: roadmap section 8 makes the ABI a gate on entry rather than JS-4's first task, and JS-4 itself depends on JS-2, which is blocked. | **The entry gate is taken.** JSD-0011 answers the representation with *replace* — this profile keeps its own tagged struct rather than adopting the seed's boxed hierarchy, because it already has a struct value model with an executor written against it and a retained corpus pinning its semantics, so adopting the hierarchy would move the rewrite onto JS-1's executor rather than avoid one. What that answer does to JS-6's scope is a change of plan and is recorded as one, at [JSC-17](roadmap.corrections.md#jsc-17). Next: JS-4's own gate needs the object model and the copy, and waits on JS-2. |
| [NONE] | **JS-5 — executor, abrupt completion, budgets** | **Not started** | None. No interpreter, no charging model, no measured `CallDepth`. | After JS-4. |
| [NONE] | **JS-6 — the standard library** | **Not started** | None. The milestone's scope is a **rewrite against this profile's value struct, not a copy re-typed** — the plan says so, on [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md)'s answer and recorded at [JSC-17](roadmap.corrections.md#jsc-17), because the seed's library is typed against a boxed value base type this profile does not adopt. The storage half of the copy table is untouched — shapes, the transition table, element arrays and the named-property store are about storage keyed by a value rather than about the value's representation. The satellite-acquisition dependency was opened at JS-0 and has a named owner. | The re-scope is the whole of what has changed here, and it changes the milestone's size rather than its order. What it must now carry that a copy would not: its own scope estimate, its own review budget — a rewrite is unreviewed code written here rather than unreviewed code copied here, and both count — and an exclusion list published on the day it lands, because a rewritten library is smaller than a copied one and the difference is a support claim. |
| [NONE] | **JS-7 — suspension** | **Not started** | None. The continuation design needs no copied code and may be opened early. | After JS-5 and JS-6. |
| [NONE] | **JS-8 — guest-initiated loads and the three compositions** | **Not started** | None. No guest-load declaration, no mediator adapter, no composition registers a provider or declines to. | After JS-7. |
| [PARTIAL] | **JS-9 — adversarial input, agents, soak** | **In progress** | [Bundle JS-9-001](evidence/js-9/README.md): a **seeded mutation fuzz target over two of roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s four surfaces** — the verifier, and the executor over verified-but-adversarial artifacts — **which is not the guided target the section asks for**: it draws every mutant from the fixed retained corpus and takes no feedback from what a mutant reached, and this row and the bundle's own header called it coverage-guided until 2026-09-01, when both were corrected and no retained log was edited ([JSC-38](roadmap.corrections.md#jsc-38)). The target in the checkout takes feedback now and the retained sessions predate it, which the closing column states. Four retained sessions of 25,000 iterations each, seeded from the 60-entry manifest, with **no counterexample**: about four thousand mutants verified and were instantiated and invoked, and every fault the executor produced carried this profile's own typed payload. A session is a total function of its seed and its seed corpus — no wall clock, no thread count — and **a session that answers the same way every time, or that never reaches the executor, exits non-zero** rather than reporting clean iterations it did not earn. One fuzz control: the verifier's constant-index check removed, found at a named iteration, reverted. **[Bundle JS-9-002](evidence/js-9-002/README.md) adds the two host-level exercises**: two runtimes under one aggregate budget spend one total — 28 invocations completed and 100 refused, the parent spending exactly its allowance, and **which sibling was refused is deliberately not asserted** because the order is a race; disposing a parent with a live child is refused and accepted after; a sealed parent admits no further runtime; and a soak of **2,000 create-run-dispose cycles** reaches a heap plateau **under JIT and trimming and NOT under Native AOT**, where the same check on the same code grows by a factor of 2.30 against a band of 2.0 and the run exits 1. **Both bundles' READMEs summarised that run as “6 runs, all exit 0”, which their own `publish-and-run.log` contradicts**; each now carries a dated correction and the logs are unedited. **[Bundle JS-9-003](evidence/js-9-003/README.md)** adds the last clause of the gate that needs nothing unbuilt: a **mutated corpus entry** — one byte of a control entry and one of a malformed entry — is detected by the replay, which reports the changed triple *and* the hash mismatch, and is restored byte for byte. Every other control in this component injects into source; that one injects into the retained bytes, which is the direction that would otherwise be taken on trust. | **Two of the four surfaces are not fuzzed because they do not exist** — the source tokenizer and parser, and the regular-expression matcher — and a session may not be read as covering them. **The guidance clause is built in the checkout and is not closed.** No session was guided at all until 2026-09-01: the mutator drew from the retained corpus and nothing a mutant reached fed back into what it drew from next. It does now — a mutant whose published answer no seed artifact produces is kept as a further seed, the pool opens as the retained corpus and grows, and the declining host rotates through one vector per exhaustion dimension rather than tightening four at once, which is what lets a session reach the three arms it could not and attribute the four it could. **The signal is the answer this profile publishes and not an edge**, so two paths to one answer are one signal and a defect on a path that answers like its neighbour is invisible to the guidance; instrumenting for anything finer would put a coverage host in a published closure or change the assembly under test, and [JSD-0013](decisions/0013-the-fuzz-sessions-coverage-signal.md) records the refusal and what would falsify it ([JSC-42](roadmap.corrections.md#jsc-42)). **What a session judges about itself is its loop and not its growth**: how much a seed set grows is a fact about the corpus as much as about the mutator, so a session fails when it offered fewer mutants to the pool than it drew, and the composition asserts separately that the pool keeps a new answer and refuses a repeat. **No retained session is guided**: the four in [Bundle JS-9-001](evidence/js-9/README.md) were collected before any of this and their logs are unedited, so closing the clause needs a collection that has not happened. **The corpus is still slice-scope, not full-format**; there is no retained-bytes report over an object model that does not exist; no agents; and no session or soak budget — the seeds, the iteration counts and the cycle count are stated so a run is reproducible, not because any of them is a number something justifies. The soak's plateau is a band and not a measurement. **The Native AOT failure this row carried is diagnosed and the check is corrected**, on 2026-09-01: it was **not** a per-cycle retention. Running 2,000, 8,000 and 16,000 cycles produced a final heap identical to the byte — eight times the work, same heap — and sampling out to 20,000 cycles showed one step and then a heap that did not move for 19,500 consecutive cycles. The growth is **one-time warm-up**, and the baseline was read before it finished: under Native AOT the heap settles at about cycle 1,000, where the check sampled at cycle 99. Under JIT the runtime's own allocation front-loads and the heap is already settled by cycle 99, which is why the same code read 0.95 there and 2.30 here. **The band was not widened** — the baseline moved to the midpoint of the run, so both readings are after warm-up in every publish mode, and the band was **tightened** from 2.0 to 1.20 because the midpoint form makes 2.0 unreachable by any linear leak. **A negative control now injects a per-cycle retention**, which nothing did before and whose absence is why the defect survived. Every figure above is an **observed repository-state fact** under section 1's third category: it explains the status and satisfies no gate. **The clause is not closed here** — closing it needs a retained bundle, which is JS-9's, and none has been collected since the correction. **Both composition roots are now published and run on every declared RID by the component's own CI lane, which also runs the four fuzz sessions, the corpus-integrity mutation and — since the correction — the soak itself, against the published image** — none of which it did when this failure reached two bundles unnoticed. The soak was briefly excluded from that lane on the ground that a heap reading on a shared runner attributes to nothing; that was true of the old check's absolute reading and is not true of the corrected check's ratio, which repeated dispatched runs across three operating systems settled rather than argued — the lane runs two of those three since 2026-09-01, when it was brought back to the component's declared RID matrix, and the third reading stands as a record of what ran rather than as a claim about a platform. **Those readings are cited nowhere as a figure and this ledger states none of them**, because they were taken in lane runs that retain nothing: under update rule 10 a number with no retained record behind it is not a number this file may carry, and what the checkout holds is the check, its band and the control that makes it fail. **The lane advances nothing here either**: it collects no bundle, so it is a regression signal between collections and never a row in this table, and **a green lane is not evidence that the plateau passes**. The corpus grows from JS-1 onward, which is why this could start before JS-8. **The seven-dimension clause this row opened on 2026-09-01 is now built in the checkout and is not closed.** Roadmap section 7 names **seven** budget dimensions a verification can answer `ResourceExhaustion` on — it named four until that date, and the verifier's allocator, work-charge and poll arms name three more ([JSC-39](roadmap.corrections.md#jsc-39)) — and asks for a corpus entry per dimension because an exhaustion answer carries no diagnostic code and the registry's both-directions binding therefore reaches none of them. Where the row previously reported one entry, an ordering assertion, two categories buried in a fuzz histogram and three dimensions reached by nothing, the checkout now holds: a manifest with a **dimension and scope column**, **seven entries**, one per dimension, each presenting the same well-formed program to a host that declined it on exactly one ceiling; and **rule N11**, which reads every resource-exhaustion answer out of the verifier's own source and holds it to an entry that pins it, in both directions and against the core's two enumerations — the clause rule N7 could not reach, for the answers that carry no code. Landing it **found that artifact bytes is not this profile's answer at all**: the core compares the payload length one call before the verifier is entered, so the verifier's two artifact-bytes arms are unreachable through any host ceiling and are defensive ([JSC-41](roadmap.corrections.md#jsc-41)). The scope column earns itself on the same evidence: the reader's ceilings answer at `Artifact` and the three allowances answer at `Runtime`, because the meter reports the level that refused, and a row recording the dimension alone would have hidden it. **The corpus grows from 60 entries to 66**, which is where a reader tracing the count across three rows meets this step. **None of that closes the clause**: closing it needs a retained bundle and none has been collected since, and under update rule 5 the four retained fuzz sessions are now evidence over a different population — they were seeded from the 60-entry manifest and this checkout holds 66, so they recertify nothing about the corpus as it stands and the next collection re-runs them. This milestone owns the clause, and the delivery file's exit gate carries it. |
| [NONE] | **JS-10 — baselines, packaging, support table, release gate** | **Not started** | None. No measurement lane, no baseline register, no package, no support table, no human review decision on anything. Neither the language-specification edition nor the conformance-suite revision is pinned, and roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) requires a provisional pin to carry a named exclusion in this ledger until a human has retrieved, hashed, and archived it — see section 3. | After JS-9, and after a named human has read every relevant unit — which is the largest single-owner task in the programme and must be scheduled, not assumed. |

### What this component is not claiming

Stated positively, because a table of empty rows invites a reader to fill them in:

- **No language is supported.** One feature manifest exists and **none is accepted**: acceptance
  needs a retained oracle run, there is no oracle, and a manifest name would not be a conformance
  claim even if there were. `broiler.javascript.slice` admits numbers, arithmetic, comparison,
  local variables and structured control flow, and admits no object, no string, no function and no
  property access - which is deliberately not JavaScript anyone would ship.
- **No composition is advertised** and none is packable. Two composition roots exist, both
  registered as demonstrations. **One runtime identifier is recorded as published and run** -
  `win-x64` - which is a record of what happened on one machine and not a supported-RID claim;
  claiming a RID is a release act and JS-10 owns it. **And no retained bundle shows that run
  clean in every mode**: the newest one records the execution-only root's Native AOT run exiting 1
  on the soak's plateau check. That check has since been corrected and the working tree passes in
  every mode, which is a fact about the checkout and not about any bundle - so the clause stays
  open until a collection shows it, which the JS-9 row states and which no row here reads past.
- **A composition root carries more than the image its label describes.** The execution-only root
  holds the corpus replay, the ordering assertions, the fuzz mutator, the soak and the
  aggregate-budget exercises, and all of them are in the closure it publishes - because each has to
  drive this profile's own verifier and executor, and the rules leave nowhere else for them to be.
  The label is a claim about a reference set and not a file inventory
  ([JSC-34](roadmap.corrections.md#jsc-34)).
- **No conformance result exists.** The suite is not pinned and the harness is not built. A
  published diagnostic registry is not a conformance claim and neither is a retained corpus:
  both are this component's own record of what its verifier does.
- **No measurement exists**, and no figure from any other component stands in for one.
- **Nothing is reviewed.** No human has read anything here, and nothing that will be copied
  arrives reviewed.
- **The seed has not been taken.** Section 4.1 of the roadmap records a candidate identity so the
  record has a shape; JS-2 records what was actually taken, and may differ. Bundle JS-0-001
  re-derives that candidate from the checkout and matches on all four revisions, which says the
  record is reproducible and says nothing about a snapshot having happened.
- **A taken decision is not implemented code.** Two records decide things no line in this checkout
  does yet: [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md)
  fixes an eight-row ABI whose object model, frame object and string are all unwritten, and
  [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md) rules on a
  snapshot nobody has taken. Each says so in its own text; this bullet is here because a reader
  counting decision records would otherwise be counting the wrong thing.
- **The product code that exists is a slice and says so.** There is a verifier, an executor and a
  hand-written lowering over about two thousand readable lines. There is no tokenizer, no static
  semantics, no object model, no standard library, no suspension and no guest-initiated load, and
  the value representation is provisional until JS-4.
- **JS-1's hand-written encoder and lowering are scheduled for deletion at JS-4**, with a named
  owner and a gate clause, because a second handle-producing path and a second lowering are
  non-goals.
- **No milestone is accepted.** JS-0, JS-1 and JS-3a each have an open exit-gate clause, and
  acceptance would in any case need a reviewer decision that nobody has made.

---

## 3. Open external dependencies

A milestone blocked by a named external dependency records the blocker, its holder, and its
unblock condition. **One is open today**, and it belongs to JS-2. The second, the seed's
un-itemised waited-on set, was closed by JS-0 and is recorded below as closed rather than
deleted.

| Blocker | Holder | Unblock condition | Note |
|---|---|---|---|
| **The core contract is not accepted.** Every core milestone is in progress and unaccepted, and the core's review record is unsigned. The core roadmap's own seeding conditions require the copy to be adapted to an accepted contract rather than a moving one. | The Broiler.VM core's architecture and release owners | A recorded human review decision on the core's contract surface, at a named contract version | This blocks JS-2 onward. It does **not** block JS-0 or JS-1, which build against the contract as implemented — a distinction the roadmap's delivery order states and this ledger holds it to. |
| **The seed's waited-on set.** **Closed 2026-08-31** by [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md): a dated ruling on each of the five items — one `Wait`, four `Do not wait` — plus a stop condition, **2026-11-30 or 400 further commits on the seed's default branch, whichever comes first**, after which the snapshot is taken as-is and the remaining waited-on item is re-derived on this side of the fork. | This component's architecture owner | Met | The closure removes the open-ended postponement roadmap [section 23](roadmap.gates.md#23-risks-and-stop-conditions) names as a risk. **It does not unblock JS-2**, which still waits on the row above. |

Four further dependencies were **unopened rather than blocked** — an unopened dependency has no
holder and no unblock condition, which is a weaker position than a blocked one, not a stronger
one. **JS-0 opened two of them and left two unopened**, and the table says which is which:

| Unopened dependency | Opened at | If it has not landed |
|---|---|---|
| **OPENED 2026-08-31.** Acquisition of the regular-expression matcher and the Unicode and locale data as this checkout's own dependencies. **Owner: the profile built-ins owner**, named in [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md). Nothing is acquired yet; what changed is that the dependency now has a holder. | Opened at JS-0, consumed at JS-6 | JS-6 excludes every surface needing it and publishes the exclusions, rather than waiting. `broiler.javascript.regexp` is already a separate manifest identity, so the exclusion is a manifest not yet minted rather than a hole in one that is. |
| **The language-specification edition is not pinned, and JS-0 did not pin it.** Retrieving, hashing and archiving a third-party document is a human action; until someone performs it the pin is provisional, and roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) requires a provisional pin to carry a named exclusion here. This row is that exclusion, and it is **still open**: JS-0 was asked to record the intended edition and no decision record does, because recording an edition nobody has retrieved would be a pin in name only. | JS-3a records the pin actually taken | No manifest may be accepted against an unpinned edition, because a conformance total against a moving document is not a total. |
| **The conformance-suite revision is not pinned**, and its licence and attribution obligations are unlanded — though where they land is now stated: JS-3a carries the attribution row and the standing-claim confirmation, in the change that first ingests a suite file, because a notice cannot carry forward content this checkout does not hold ([JSC-30](roadmap.corrections.md#jsc-30)). The suite is third-party material this component ingests; roadmap [section 22](roadmap.gates.md#22-release-gates) gate 12 makes an attribution obligation discovered during a publish a stop. **Still open after JS-3a's registry half**, which was deliberately built to need nothing from it: retrieving, hashing and archiving third-party material is a human action and nobody has performed it. | JS-3a | The harness cannot start: a branch name is not a pin, and the method requires the revision resolved once before any shard. **This is what keeps JS-3a `In progress` rather than the registry work.** |
| **This profile's declared defaults are catalog-wide and unreconciled.** Roadmap section 3 records that a host adopting profile defaults gets the tightest in the catalog, so a neighbour's stingy default reaches this profile wherever ceilings are adopted rather than stated, and that reconciling two profiles' declarations belongs to whichever component composes them. That component does not exist and has no owner. **Narrowed 2026-08-31**: the maxima half of this row was retired when the core removed a catalog-wide maximum clamp its own record never authorised. A maximum now binds only the artifacts of the profile that declared it. **PARTLY OPENED 2026-08-31**: [JSD-0004](decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md) records the fifteen defaults and fifteen maxima with the split stated inside the decision, and chooses `NestedLoadDepth`'s default at 4 rather than the 1 this profile would need, precisely so a neighbour adopting defaults is not strangled. **The reconciliation itself is still unowned.** | JS-0 recorded the vectors; the composing component owns the reconciliation | A browser composition that adopts defaults discovers it as a resource exhaustion naming a dimension this profile did not breach, in a verifier that did nothing wrong. |

---

## 4. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle carrying all applicable
fields below. **A command written in a plan is not evidence that the command ran.**

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap and gate revision, core contract version, format version, feature manifest set, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Component commit, recursive submodule revisions, dirty-tree state and patch identity, and the exact paths and projects under test. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, corpus and fixture hashes, the pinned conformance suite revision, and applicable provenance or licence decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets redacted without hiding semantically relevant configuration. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Results** | Raw outputs retained, including failures. A bundle that retains only the passing half is not a bundle. |
| **Negative controls** | Each control, the injection that must make it fail, and the revert that must make it pass. The count is stated and grows across milestones. |
| **Closure** | For any Native AOT claim: the published output's dependency closure, read off the published image rather than asserted. |
| **Exclusions** | What the bundle does **not** show. Every open gate clause, every unexercised path, every single-machine or single-RID limitation, named. |

---

## 5. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, **including its failures and its exclusions**.
3. Do not infer completion transitively. JS-1 acceptance does not accept JS-2; a slice-manifest
   result does not accept a later manifest; and JIT, trimmed, or one-RID success does not accept
   an untested Native AOT or RID claim.
4. Do not promote seed, shell, smoke, analyzer-only, or shape-only results beyond what they prove.
   A failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence
   gathered against a different population is not silently carried forward. A core contract
   amendment is such a change: record the new version and state, per affected record, what
   recertifies unchanged, what must be re-collected, and what is superseded.
6. **Do not record core work here, and never record profile work in the core's ledger.** A core
   result never advances a row in this file, and no row here advances a row there.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and the evidence-bundle ID
   in the affected row. Where owner and reviewer are the same person, record the
   non-independence in the row rather than resolving it by assertion.
8. **Human review gates a release, not a development step.** Development work — implementing a
   milestone, landing it, collecting its evidence — may proceed and merge without a review
   decision. A **release** may not: no package is published, no RID is claimed, no support table
   is issued, and no milestone moves to `Accepted` until a named human has read the work and
   recorded a decision on every relevant code unit, bound to that declaration's fingerprint so a
   unit that changes afterwards reports stale rather than being silently carried.

   Two consequences are worth stating plainly, because this component will feel them harder than
   a greenfield one would. **Unreviewed work accumulates**, and this component starts with a large
   copied body of it: everything the snapshot brings in is unreviewed here on the day it lands,
   and the review debt is real from the first commit rather than from the first release. And **a
   development step that lands unreviewed carries its risk forward rather than dissolving it** —
   a passing conformance run over an unreviewed parser is a statement about the parser's outputs,
   not about the parser.
9. **A copied unit records its origin.** Every unit taken from the seed is annotated as ported,
   and the origin distribution is published in the generated assurance report. A component whose
   report cannot say how much of it was written here is a component whose review status cannot be
   read.
10. **No count, total, graph, commit, or score is copied into prose.** This ledger names the
    command or the retained record that reads it. A number transcribed into a sentence goes stale
    silently, and a ledger that goes stale silently is worse than one with a gap in it.

---

Until such updates are recorded, section 2 remains the complete status of this component: **no
milestone is accepted, no snapshot has been taken, no language surface is supported, no
composition is advertised, no runtime identifier is claimed, no measurement or conformance result
exists, and nothing has been reviewed.**

A closing summary that restates a table rather than pointing at it is a second copy of the status,
and the second copy is the one that goes stale. This one restates only what no milestone can
change without changing the table *(corrected: JSC-20)*.
