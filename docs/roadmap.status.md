# Broiler.VM roadmap status

**Last updated:** 2026-09-01

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[Broiler.VM roadmap](roadmap.md). The roadmap defines planned work and objective exit gates; this
ledger records whether those gates have accepted evidence.

No Broiler.VM milestone is complete merely because its design appears in the roadmap. At this
snapshot, **every milestone VM-0 through VM-6 is in progress and unaccepted.** The roadmap has
no further milestone: what remains is not more work but the human review that gates a release.

The repository now contains the Broiler.VM [component overview](../README.md), the roadmap
documents, the twelve boundary records in [docs/adr](adr/README.md), the composition register in
[docs/compositions.md](compositions.md), the baseline register in
[docs/baselines.md](baselines.md), the public support table in [docs/support.md](support.md),
the frozen public API in [docs/api/public-api.txt](api/public-api.txt), a pristine feed consumer
that reaches the component only through packages, and a fourteen-project graph that implements core contract
version 1: the profile-neutral contracts, the bounded binary primitives, the immutable catalog, the
runtime and its lifecycle, resource authority including aggregate budgets and the full precedence
algorithm, guest-initiated-load mediation with its four bounds, external suspension, two fixture
profiles, a composition-root host that publishes and runs under trimming and Native AOT, a fuzz
target host, a retained malformed-input corpus of eighty-seven artifacts, two application-local
consumer profiles written against the public contract alone, two named composition roots that
publish and run under trimming and Native AOT, a soak host whose long lifecycle run is sampled for
a memory plateau, an uninstrumented benchmark host that publishes what the core costs a profile on
the JIT and Native AOT lanes, and 318 tests passed: 121 architecture and 197 behavioural.

**No milestone is accepted, and none can be until a human reads the work.** VM-0 through VM-6 are
all unaccepted because no relevant code unit carries a decision on its `// Broiler-Human:` line, so
the generated [`HUMAN_REVIEW.md`](../HUMAN_REVIEW.md) states `PENDING`. At VM-6 that stops being a
formality: the milestone's own gate asks that reviews be complete, so the last milestone in the
roadmap cannot be met by anyone who wrote it.

VM-1 was nonetheless built against VM-0's frozen-but-unapproved records, and update rule 8 now
records why that was legitimate: **human review gates a release, not a development step.** The
owner ruled on 2026-08-28 that implementing, landing and evidencing a milestone may proceed without
a review decision, and that publishing a package, claiming a RID, issuing a support table or moving
any milestone to `Accepted` may not. That resolves what Exclusion EX-43 previously recorded as an
open reading.

Nothing about the component's actual state changes with it. No release capability is claimed, and
the work that has accumulated unreviewed is not paper: it is a parser over untrusted bytes, a
budget enforcer, and now the corpus and the fuzz target that are supposed to keep both honest -
which are themselves one person's reading of what the parser ought to answer.

---

## 1. Reading this ledger

The following categories must remain distinct:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation or validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout, such as the
  absence of Broiler.VM projects. It can explain a status but cannot by itself satisfy a future
  implementation, contract, performance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable evidence bundle that identifies the exact
  sources and gate, records the executed commands and environment, retains their outputs, and
  demonstrates every part of the objective exit gate. Only accepted evidence may advance a
  milestone to `Accepted`.

Work in other components is not Broiler.VM evidence. In particular, the legacy `Broiler.JS`
component's implementation, conformance results, measurements, and Native AOT samples establish
nothing here, and no core gate may cite them. Broiler.VM has no dependency on that component and
none of its milestones wait for it.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, owner, and unblock condition must be recorded; lack of scheduling is not a blocker. |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and owner/reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and decision record must be linked; evidence history is retained. |

The marks that appear in section 2 - evidence verdicts set by the author about evidence,
and review verdicts set by a reviewer about their own reading - are defined once, in
[HUMAN_REVIEW.md section 1](../HUMAN_REVIEW.md#1-how-to-use-this-file), which is their
canonical legend.

---

## 2. Current milestone status

The leading column is an evidence verdict - the author's mark about what the row's
retained evidence shows, not a reviewer's finding and not a change of state. An
`In progress` milestone with a retained bundle is partly demonstrated; a `Not started`
milestone claims nothing at this snapshot.

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [PART] | **VM-0 — ownership, terminology, core contract version, and graph** | **In progress** | [Evidence bundle VM-0-001](evidence/vm-0/README.md), collected 2026-08-27 against component commit `1bba027` with a clean source tree. Twelve boundary records in [docs/adr](adr/README.md) freeze the graph, ID policy, lifecycle, result envelope, verified-artifact ownership, resource authority and precedence, the guest-initiated-load, asynchronous-instantiation, external-suspension and aggregate-budget decisions, the three embedding decisions, the profile-facing checklist and sharing rule, and core contract version 1 with its amendment procedure. A five-project acyclic shell graph built Release with 0 warnings; 35 architecture tests passed; a negative control showed the containment and edge rules rejecting an injected forbidden edge; pack produced exactly three packages and did not pack the fixture profile. **Superseded in part by VM-1:** the shell graph and its counts are historical, and VM-1's implementation now exercises what those records describe. The *decisions* VM-0 froze are unchanged and still unapproved. | **Open gate conditions, unchanged.** (1) No review decision is recorded. All six ownership roles in ADR 0012 are held by MaiRat, so section 13's dependency line is satisfied, but `HUMAN_REVIEW.md` is unsigned and `PENDING`, and owner and reviewer are the same person, so update rule 7's confirmation would not be independent (EX-30). (2) The twelve records are `Proposed`, not approved. (3) The inbound half of the legacy-boundary rule is environment-conditional (EX-01). (4) No SDK pin exists (EX-03). (5) Seventeen roadmap amendments are proposed and unapplied (EX-11). Next: review and sign, or reject, the twelve records — now with an implementation to read them against, which is a better position to review from than paper alone. |
| [PART] | **VM-1 — semantics-neutral runtime, catalog, and fixture profile** | **In progress** | [Evidence bundle VM-1-003](evidence/vm-1/README.md), collected 2026-08-28, superseding VM-1-002 and VM-1-001. Re-collected because the component gained the group H review-record rules and the Broiler Code Assurance system, which changed the hash of every product source file and moved the architecture suite from 44 tests to 89. Core contract version 1 is implemented: the profile-neutral contracts and thirty-row descriptor, the bounded readers and allocation guard, the immutable catalog with its canonical encoding, the runtime with its lifecycle, execution slot, latches and idempotent disposal, the fifteen-dimension meter chain with aggregate budgets, guest-initiated-load mediation with its bounds and its deterministic no-provider refusal, external suspension behind the double gate, typed payload projection, and two fixture profiles built as a bytecode stack machine with a real framed format. Seven projects build Release with **0 warnings**; **221 tests pass** (90 architecture, 131 behavioural); pack still produces exactly three packages; **the composition-root host publishes and runs under JIT, trimming and Native AOT on `win-x64`**, composing two profiles through the generic contract. Four negative controls each fail when injected and pass after revert. Fourteen of the gate's sixteen clauses are demonstrated, a fifteenth in part (G-08: reentrancy is enforced on the execution path, thread affinity is never exercised across threads), and the sixteenth is not met. **The implementation was adversarially reviewed against the frozen records: 45 findings survived independent refutation, sixteen of them blockers, several confirmed by execution. Every blocker is corrected and regression-tested; the first bundle, VM-1-001, is superseded because it reported a green suite over a tree that contained all sixteen.** | **Open gate conditions.** (1) No review decision is recorded, and no reviewer has read this work — the gate's own last clause, "the accepted contract is recorded with its version", is therefore **not met**: the contract is implemented and versioned, not accepted. (2) VM-1's dependency on VM-0 acceptance was not satisfied when the work was done (EX-43). (3) Declared thread affinity is carried but never exercised across threads; concurrency is VM-4's (EX-44). (4) One RID, one machine, no CI (EX-45). (5) The Native AOT publish needs a `vcvars64` shell because the ILCompiler package's own toolchain discovery fails here, so no automation reproduces it (EX-42). (6) Three deviations from the frozen records are filed as errata rather than amendments — the control-result shape, the result-construction gate, and the unexported `VmOperation` (EX-41). (7) Twenty-nine review findings of major and minor severity are recorded and unaddressed (EX-52): the runtime is never poisoned by a broken metering contract, the non-reentrancy gate is absent on the control operations, the disposable artifact lifetime releases nothing, and several diagnostics groups are thinner than the records require. Next: review the contract surface and the three errata, then sign or reject — and read the review section of the bundle first, because one pass found sixteen blockers behind a suite that was passing. |
| [PART] | **VM-2 — bounded artifacts, verification, and resources** | **In progress** | [Evidence bundle VM-2-001](evidence/vm-2/README.md), collected 2026-08-29 on `linux-x64`. The precedence algorithm is complete: the artifact-requested limit is clamped to the host and profile intersection and the clamp is recorded on the handle, and instance and invocation overrides inherit when omitted, tighten when stated, and are refused as a host failure when they would raise — which is what `BudgetRaiseRefused` was for, a reason VM-1 declared and no code path could produce. **A malformed-input corpus is retained**: eighty-seven artifacts under `src/tests/corpus/vm-2`, each with its SHA-256 and its expected answer, across prefixes, magic, format versions, section framing, the constant pool, the code section, artifact-bytes ceilings, and two systematic sweeps — the canonical artifact truncated at every offset and every one of its bytes inverted. Forty entries pin outcome, reason and profile diagnostic code by hand; the sweeps pin the closed set. **A fuzz target host** — the one test-only project ADR 0001's budget permits this milestone — ran eight seeded sessions of 250,000 iterations each, two million in total, and found no counterexample. Eight projects build Release with **0 warnings**; **255 tests pass** (90 architecture, 165 behavioural); pack still produces exactly three packages; **the published host replays the whole corpus under JIT, trimming and Native AOT and the three failure-class tables are byte-identical**. Eight negative controls each fail when injected and pass after revert. The materialization ordering is asserted mechanically for every corpus artifact including every failing one, and the effective policy each verification received is recomputed from the three layers independently. **Its own corpus and suite found three defects nobody had read**: the uncharged-work counter summed every budget dimension, so one in-bounds allocation breached a poll bound and a core unit conflation was billed to the profile; the cumulative nested verifier-work bound was validated at runtime creation and read nowhere; and the artifact-limit clamp was computed and discarded. | **Open gate conditions.** (1) No review decision is recorded and no reviewer has read this work. (2) Two gate clauses are `[PART]`: no fuzz session has found a regression to retain, so the clause asking that the suites retain minimized regressions is satisfied only in the sense that there is nothing to retain (EX-79); and recursive provider requests do not terminate at a configured depth bound because they cannot happen — contract version 1 gives an executing profile no way to instantiate the handle a load returns, so nesting is bounded at one by construction (EX-78). (3) Bounded outer-envelope parsing is not implemented and no milestone approves it: ADR 0010 records that VM-0 through VM-6 contain no persistence gate, so the gate's own "where approved" has no referent (EX-25). (4) One RID, one machine, no CI — and now a second single machine rather than a matrix (EX-45). (5) The fuzz session varies the payload and never the descriptor, so the whole descriptor-facing surface is exercised by the corpus and by no fuzz iteration (EX-80). (6) Two deviations from the frozen records are filed as errata: the clamp is carried on the handle rather than in the frozen diagnostics field set, so a failed verification records none (EX-82), and a refused override names its dimension in the diagnostics group ADR 0005 annotates for exhaustion (EX-83). (7) VM-2's dependency on VM-1 acceptance was not satisfied when the work was done; update rule 8 is what makes that legitimate. Next: review VM-0's records, VM-1's contract surface and VM-2's boundary together, and read each bundle's own exclusion table first. |
| [PART] | **VM-3 — public profile contract and exact closures** | **In progress** | [Evidence bundle VM-3-001](evidence/vm-3/README.md), collected 2026-08-29 on `linux-x64`. **Two application-local consumer profiles were written against the public source contract alone** - `com.example.calculator`, a flat token stream with one fixed entry point and no host import, and `com.example.ledger`, a record format of two length-framed sections whose entry-point bytes are an account name and which imports one optional host capability. Each references exactly `Broiler.VM.Abstractions` and `Broiler.VM.Binary`, holds no `InternalsVisibleTo` in either direction, declares no package reference, and reaches nothing in the runtime. **Two named composition roots** under `src/compositions/` compose them by direct typed registration - one profile and two - and each **publishes and runs under JIT, trimming and Native AOT**, passing twelve checks between them in every mode. **The closure of each published image is read off the published output**: five non-framework assemblies for the single-profile composition and six for the two-profile one, differing by exactly the second profile, with no fixture assembly, no testing framework and no reflection-emit assembly in either. **No file under any of the three product project directories changed**, and both published image sizes are byte-identical to VM-2's, which is the numeric form of the gate's central clause. Twelve projects build Release with **0 warnings**; **262 tests pass** (97 architecture, 165 behavioural); pack still produces exactly three packages. Six architecture rules were minted - A12 and A13 over project files, and a new group K holding the composition register, the reference sets, the catalog baselines and the published closures to each other - each with its own violating input. `docs/compositions.md` exists and **closes Exclusion EX-08**. Eleven negative controls each fail when injected and pass after revert, three of them new: a composition root linking the fixture profile, a composition deleted from the register, and a catalog baseline that gains a profile nothing composes. **Building the second profile found a behaviour nobody had written down**: a runtime ceiling was clamped to the tightest profile hard maximum in the catalog, so a profile that declared its own usage as its maximum capped every profile composed beside it - which is why the two-profile composition could not verify a ledger artifact until both profiles' maxima were corrected. **Superseded 2026-08-31, after this bundle was collected: that clamp was an implementation defect and not a property of the contract**, ADR 0007 puts `ProfileMax` at P2 against the profile an artifact names, and it has been removed - so a hard maximum now binds only the artifacts of the profile that declared it, and the one catalog-wide term left is the adopted **default**. The bundle is unedited and the finding stands as history under update rule 1: VM-3 did observe that behaviour and did loosen both profiles' maxima because of it. What changed is that the observation no longer describes the implementation, so no later row may cite this sentence as a contract property. The loosened maxima were left alone - nothing about them is wrong - and the comments that explained them were corrected on the same date. | **Open gate conditions.** (1) No review decision is recorded and no reviewer has read this work. (2) The two consumer profiles are exercised by published composition roots and by no test project, because rule A11 forbids a test project to reference a profile assembly and that rule is one of the things this milestone demonstrates; a reader checks behaviour in `composition-*.log` rather than in the suite. (3) Neither consumer profile is fuzzed at all - EX-80 now understates what it covers. (4) Rules K3 and K4 compare against the last collection rather than against the working tree, because the bundle is retained by a script a person runs (EX-86). (5) The closure report excludes framework assemblies by name prefix (EX-87). (6) One RID, one machine, no CI (EX-45). (7) VM-3's dependency on VM-2 acceptance was not satisfied when the work was done; update rule 8 is what makes that legitimate. Next: review the composition register and the two closure reports against ADR 0001 revision 1. The bundle's section 6.1 finding is retained but is **no longer an action**: it was answered on 2026-08-31 by removing the clamp rather than by profiles working around it, and `docs/compositions.md` section 5 carries what a profile author must declare now - which is a default chosen with its neighbours in mind, not a loosened maximum. |
| [PART] | **VM-4 — lifecycle, concurrency, diagnostics, and hosts** | **In progress** | [Evidence bundle VM-4-001](evidence/vm-4/README.md), collected 2026-08-29 on `linux-x64`. **This is the first milestone since VM-1 to change the product assemblies, and it changed them because a suite that could reach a second thread found four rules that were written down at VM-0, implemented at VM-1 and enforced nowhere.** A running host capability refused every OTHER thread's call into the runtime, because non-reentrancy - a property of a call stack - was held as a runtime-wide flag; disposal of an instance and of a runtime returned while the profile was still inside a step and then released the artifact lease and the retained bytes under it, because the `DisposeDrainBudget` option was declared, defaulted and read by nothing; `VmThreadAffinity.OperationThreadPinned` was carried in every descriptor and read by nothing, so `VmReason.ThreadAffinityViolation` had no code path; and an instantiation in flight when a runtime began disposing registered its instance anyway, stranding a lease and an allowance in a runtime nobody could reach. Each was proven by a test that failed before the fix. Disposal now drains in-flight steps under the host's own wall-clock bound and, when that bound expires, hands the lease release to the departing step rather than disposing the verified state under a profile that is still reading it. Thirteen projects build Release with **0 warnings**; **293 tests pass** (99 architecture, 194 behavioural); pack still produces exactly three packages; both compositions still publish and run in three modes with unchanged closures. **A soak host** - the one test-only project ADR 0001 revision 2 authorises - ran **400,000 lifecycle cycles across four workers and 100 recycled runtimes**, sampling the managed heap and working set throughout: the trimmed self-contained run settles at a managed heap of 66,216 bytes after allocating 1,923,955,560, below its own first sample, which is the declared memory plateau as a measurement rather than as a metered counter. The first soak run completed 161,616 of the 400,000 cycles it asked for and was reported invalid rather than published: a runtime's ceilings are a total allowance, so one long-lived runtime spends its fuel and refuses everything afterwards, and the run was measuring the moment its budget ran out. Two rules are minted: V11 holds the diagnostics record to a shape with nowhere to put free text, and V12 holds every profile-facing contract to trafficking in the contract's own types. Fourteen negative controls each fail when injected and pass after revert, three of them new and each pinning one of this milestone's corrections. | **Open gate conditions.** (1) No review decision is recorded and no reviewer has read this work. (2) Every concurrency result was collected on **four processors** (EX-88): the execution gate makes each arranged race deterministic, which is a narrower claim than correctness under arbitrary parallelism. (3) Thread affinity is enforced where the core can see a thread and nowhere else (EX-89); a profile that starts its own threads is invisible to it. (4) Two errata against ADR 0011 F5 are filed rather than amended: the in-capability flag is call-stack scoped where the record says per-runtime (EX-90), and a refused re-entry answers a more specific reason than the record names (EX-91). (5) `UnwindTimedOut` remains unreachable, because the allowance it would be measured against is in the profile's own work units and the unwind entry point takes no meter (EX-92); the bound that IS enforced is the core's own wall-clock wait. (6) The two opaque-reference identity reasons remain unreachable, with a test asserting the unreachability (EX-93). (7) One RID, one machine, no CI (EX-45). Next: review the four corrections against ADR 0004 and ADR 0011 - each is a place where a frozen record and the implementation disagreed for three milestones without anything noticing. |
| [PART] | **VM-5 — core overhead baselines** | **In progress** | [Evidence bundle VM-5-001](evidence/vm-5/README.md), collected 2026-08-29 on `linux-x64`. **A milestone whose subject was measurement changed the product assemblies twice, because a baseline asks a question no test asks: how much does this cost, and does the answer depend on anything it should not?** The guest-load mediator never reset its per-operation counters - `EnterScope` had an overload that omitted the operation's identity and every call site used it, so every step compared equal to the last and the reset never ran once. Fan-out, cumulative nested bytes and nested verifier work were therefore lifetime bounds on a mediator shared by every instance of one profile in one runtime: a probe asked how many mediated loads a runtime admits and printed **8**. Three existing fan-out tests passed throughout, because all three invoke exactly once. Separately, a runtime's capability depth lived in an `AsyncLocal<int>`, whose entry can never be released - returning the depth to zero stores a boxed zero, which is a present value - so one entry per runtime stayed on the thread for the life of the process, released by nothing and not by disposal: the same instantiate-and-invoke allocated 9,960 bytes early in a run and **1,188,872 bytes after seventy thousand runtimes**, and the benchmark took 528 seconds where it now takes 43. Nothing observable failed in either case. Fourteen projects build Release with **0 warnings**; **304 tests pass** (107 architecture, 197 behavioural); pack still produces exactly three packages; both compositions still publish and run in three modes with unchanged closures. **A benchmark host** - the one test-only project ADR 0001 revision 3 authorises, referencing no benchmarking package - produced **ten measurements on the JIT lane and on the Native AOT lane, every one with its A/A lane inside its effect**. `docs/baselines.md` is the register and rule **L1** binds it to the retained log in both directions. Seventeen negative controls each fail when injected and pass after revert, three of them new and each pinning one of this milestone's corrections. | **Open gate conditions.** (1) No review decision is recorded and no reviewer has read this work. (2) Every figure is an absolute time on **one four-processor machine** (EX-45, EX-88): the ratios travel, the absolute values do not, and a benchmark expires faster than a test result. (3) The gate's **envelope read and write** is answered for the operation-result envelope, not the persisted one, which is admitted by contract and implemented by no milestone (EX-96). (4) Only the register's measurement table is bound to the log; its recorded figures - startup, image sizes, the plateau and independence verdicts - are checked by nobody (EX-95). (5) The guest-load lane rebuilds a runtime inside its timed region because fan-out is a runtime-scope lifetime total whose profile maximum no host may raise (EX-97). (6) ADR 0001 owns rule group L nominally and names no rule of it (EX-94). (7) Package sizes are absent: packages are produced at VM-6. Next: review the two corrections against ADR 0008 and ADR 0004 - both are places where a bound the records state was not the bound the code enforced - and read the fan-out series, which is the one thing here that funds future optimisation work. |
| [PART] | **VM-6 — package, release, and recertification** | **In progress** | [Evidence bundle VM-6-001](evidence/vm-6/README.md), collected 2026-08-30 on `linux-x64`. **The first milestone since VM-3 that changes no product assembly**, which is what a packaging milestone ought to be able to say. The public API is frozen by ENUMERATION rather than by assertion: `docs/api/public-api.txt` lists every exported type and every public and protected member of the three packable assemblies with constants carrying their values, 1,251 lines, and rule **M1** compares it against the built assemblies in both directions - group V had fixed named properties of the surface since VM-1 and none of them was a claim about what IS there, so a member added or removed was a change nothing noticed. A **pristine feed consumer** under `samples/` has three `PackageReference`s, no `ProjectReference`, and a `NuGet.config` that clears every source and adds back one directory of `.nupkg` files with nuget.org unreachable; it defines a whole profile against the public contracts - format, bounded-reader verifier, metering adapter, Fuel-charging executor, payload projection - and passes four checks including a host ceiling bounding the work, then publishes with trim and AOT warnings as errors into a single self-contained file of **2,048,520 bytes** that passes them again. **Rollback is exercised rather than described**: two package sets on one feed, the consumer restored against the newer, rolled back to the older, printing the informational version it actually loaded each time. Rules **C1, C2 and C3** are promoted from Deferred - their subject, a produced package, exists for the first time - and the collection now retains every `.nuspec` so they have something to read. **C2 caught a false claim in this milestone's own support table**: the first draft said the packages declare no package dependency at all, and `Broiler.VM.Runtime` declares the other two. Group A's subject is narrowed to the solution rather than the directory tree, because the sample deliberately carries the Broiler package reference rule A2 forbids; rule **A14** is the complement, so a project in neither the solution nor `samples/` is reported rather than invisible. Fourteen projects build Release with **0 warnings**; **318 tests pass** (121 architecture, 197 behavioural); the trimmed and Native AOT images are byte-identical to VM-5's, which follows from changing no product code. `docs/support.md` and `THIRD_PARTY_NOTICES.md` are published. | **Open gate conditions.** (1) **The gate asks that reviews be complete and no review can be completed here**: `HUMAN_REVIEW.md` is unsigned and `PENDING`, all eight area verdicts are unset, and the person who would sign is the person who wrote the work. No package may be published without it. (2) **CI exists and has never run** (EX-102): a workflow file is not a required check, there is no branch protection, and its RID matrix is aspirational for every entry but `linux-x64` - so the drift checks are wired but not *required*, which the gate asks for and this milestone does not have. (3) One RID is claimed because one has ever been published on (EX-45); the support table says so rather than implying breadth. (4) Rule M1 compares text and not semantics, so a default value, an attribute or a nullability change that leaves signatures identical is invisible to it (EX-100), and the baseline is regenerated by an environment variable so a reviewer who regenerates without reading has defeated it (EX-99). (5) The pack rules read the last collection rather than the working tree (EX-101). (6) The checkout carries one project file the frozen graph does not, and cannot not (EX-103). (7) **The gate gained four publication clauses on 2026-09-01 and none of them is published**: the candidate-amendment register's state, the extraction-gate register's state, the persistence position, and the two bounds that are declared and not demonstrated — the depth half of the cross-runtime `CallDepth` bound and the reconciliation of two profiles' declared defaults. Each is a question no milestone here can answer, which is why the last milestone publishes the state rather than the answer; the support table this bundle published predates all four. Update rule 5 applies: the gate changed, and this row records what that leaves open rather than carrying the bundle forward against a different gate. Next: this is the last milestone in the roadmap, so what follows is not VM-7 but the review itself - read the API baseline diff, the support table's claims against the evidence, and the four gate clauses above that are `[PART]` or `[UNMET]`. |

### Where the review stands

No area has been reviewed, so no verdict below is set. **This table is a reader's summary and is
not the record.** The record is per code unit, in the `// Broiler-Human:` line on each declaration,
and [HUMAN_REVIEW.md](../HUMAN_REVIEW.md) is generated from those lines; the areas below are a
risk ordering a reader may find useful and are not bound to anything mechanical.

| Verdict | ID | Area |
|---|---|---|
| [ ] | RA-1 | Bounded reading of untrusted bytes |
| [ ] | RA-2 | Resource authority and budgets |
| [ ] | RA-3 | Lifecycle and state machine |
| [ ] | RA-4 | Verified-artifact ownership |
| [ ] | RA-5 | Guest-initiated loads and external suspension |
| [ ] | RA-6 | The public contract surface |
| [ ] | RA-7 | The records themselves |
| [ ] | RA-8 | The evidence and the rule register |

The immediate programme action is still **the review decision itself**, and it now covers seven
milestones rather than one. VM-0's records, VM-1's implementation, VM-2's boundary, VM-3's
compositions, VM-4's lifecycle hardening, VM-5's baselines and VM-6's packaging are all written,
all have retained evidence, and all six roles recorded in
[ADR 0012](adr/0012-security-ownership-and-support-matrix.md) are held. What remains is a human
reading the twelve records *and* the contract surface that implements them *and* the corpus and
fuzz target that bound it, and recording a decision on the declarations themselves, which
regenerates [HUMAN_REVIEW.md](../HUMAN_REVIEW.md). With a single maintainer that confirmation is
not independent, which EX-30 records rather than resolves.

Reviewing them together is easier than reviewing VM-0 alone was: a decision that was paper in
August can now be read against code that either honours it or does not, and the five places where
the implementation could not honour a record verbatim are named as errata rather than left for a
reader to discover.

It is also more necessary than it was, and for a second reason now. The first implementation passed
150 tests, built with no warnings, and published and ran a Native AOT binary while containing
sixteen blocking contract violations - among them an aggregate budget that could be driven to zero
while memory was live, and a capability translation mode that translated nothing. An adversarial
pass found them. VM-2's corpus and fuzz target then found three more that no pass had read, which
argues both that the mechanisms work and that reading alone was not enough - and those mechanisms
are themselves unreviewed: the corpus's expectations are one person's reading of what the verifier
ought to answer, and two million iterations finding nothing is worth exactly what the invariants
are worth. A second adversarial pass would be a reasonable thing to fund before anyone signs.

The review worksheet is a further gap rather than a further reason. `docs/review/vm-0-vm-1.md`
covers VM-0 and VM-1 and has no item for anything VM-2 added; HUMAN_REVIEW.md records that as
AT-11.

### Profiles

The JavaScript and WebAssembly profiles are separate components with their own roadmaps and their
own ledgers. They are **not planned or tracked here**, no row above depends on either, and no
profile result closes a core gate. Section 9 of the roadmap records only what they are expected to
require of the core contract.

A profile roadmap may open once VM-1's contract is accepted. It is implemented but not accepted, so
no profile roadmap may open yet. The JavaScript profile additionally
carries a seeding precondition recorded in its own roadmap: it starts from a named snapshot copy
of the legacy component taken after that component's in-flight fix programme lands, as a fork with
no dependency edge in either direction.

---

## 3. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle with all applicable fields
below. A command written in a plan is not evidence that the command ran.

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap/gate revision, core contract version, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Repository commit, dirty-tree state and patch identity, and exact paths and projects under test. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, fixture hashes, and applicable provenance or license decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets must be redacted without hiding semantically relevant configuration. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Outputs** | Durable logs, machine-readable results, binaries or packages or hashes, analyzer and trim/AOT warnings, crash dumps or minimized fuzz cases where applicable, and storage locations with retention policy. |
| **Decision** | Expected gate, actual result, unexplained failures, exclusions, deviations, the claim justified by the result, reviewer verdict, and follow-up owner. |
| **Validity** | Reproduction instructions, expiry or review date where evidence can age, and recertification triggers such as source, dependency, SDK, RID, contract-version, API, or composition changes. |

Performance evidence must additionally follow the repository's decision-grade measurement rules,
retain every repetition and resource metric, identify candidate and control exactly, and report
negative or inconclusive results. Security and fuzz evidence must retain corpus identity, budgets,
duration or iteration count, sanitizer and runtime settings, failures, and minimized regressions.
Native AOT evidence must come from publishing and running the declared composition on every
claimed RID; analyzer success alone is not a publish-and-run result.

---

## 4. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, including failures and exclusions.
3. Do not infer completion transitively. VM-0 acceptance does not accept VM-1; a fixture result
   does not accept an application-local one; and JIT, trimmed, or one-RID success does not accept
   an untested Native AOT or RID claim.
4. Do not promote seed, shell, smoke, analyzer-only, or shape-only results beyond what they prove.
   A failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence
   gathered for an older or different population is not silently carried forward. A core contract
   amendment is such a change: record the new version and state, per affected record, what
   recertifies unchanged, what must be re-collected, and what is superseded.
6. Do not record profile work here. A profile's status belongs to that profile's own ledger, and a
   profile result never advances a row in this file.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and evidence-bundle ID in
   the affected row.
8. **Human review gates a release, not a development step.** Ruled 2026-08-28 by the architecture
   and release owner. Development work - implementing a milestone, landing it, and collecting its
   evidence - may proceed and merge without a review decision. A **release** may not: no package is
   published, no RID is claimed, no support table is issued, and no milestone moves to `Accepted`
   until a named human has read the work and recorded a decision on every relevant code unit, which
   the publish lane checks and [HUMAN_REVIEW.md](../HUMAN_REVIEW.md) reports. The decision is bound
   to the fingerprint of each declaration rather than to a revision, so a unit that changes
   afterwards is reported as `STALE` rather than silently carried.

   This settles the question Exclusion EX-43 recorded rather than answering. Roadmap section 13
   sequences each milestone "after VM-*n* acceptance"; under this ruling that sequencing binds the
   release train, and a milestone may be *built* against records that are frozen but unapproved. It
   does not weaken anything else in this file: rule 7 is unchanged, an unreviewed milestone stays
   `In progress` however complete its evidence is, and the roadmap is not re-authored - the
   divergence is recorded here, as ADR 0003's amendment register records the others.

   Two consequences worth stating plainly, because the alternative is discovering them at release
   time. Unreviewed work accumulates: the longer a release is deferred, the more there is to read
   in one sitting, and the VM-1 review found sixteen blockers in one milestone's worth. And a
   development step that lands unreviewed carries its risk forward rather than dissolving it - the
   component currently holds an unreviewed parser and an unreviewed budget enforcer, and that is
   true no matter how many green suites are collected over them.

Until such updates are recorded, the table in section 2 remains the complete Broiler.VM status:
**VM-0 through VM-6 are all in progress and unaccepted**, each with a retained evidence bundle, and
no release capability is claimed. Core contract version 1 is implemented and runs; it is not
accepted, no milestone has a review decision, and no language profile ships. `linux-x64` and
`win-x64` have each published and run, on one machine each and with no CI lane that has ever
executed, so neither is a claimed RID.

*Corrected 2026-08-31.* This paragraph previously read "VM-0 and VM-1 are in progress and
unaccepted, VM-2 through VM-6 are not started" and "no RID other than `win-x64` is claimed, no
concurrency, corpus or performance result exists". It was written when that was true and was not
revised as VM-2 through VM-6 collected bundles VM-2-001 through VM-6-001, so it contradicted the
table it claims to summarise — an eighty-seven-artifact corpus, a four-hundred-thousand-cycle soak,
and ten measurements per lane all existed while this paragraph said they did not. It is recorded as
a correction rather than silently rewritten, because a summary that drifts from its own table is
exactly the failure this ledger exists to prevent, and **a reader who skips the table quotes this
paragraph.**
