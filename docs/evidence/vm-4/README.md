# Evidence bundle VM-4-001

**Milestone:** VM-4 — lifecycle, concurrency, diagnostics, and host integration
**Collected:** 2026-08-29
**Core contract version:** 1
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition. Under update
rule 8 the work could be built and landed without that decision; it could not be released, and
nothing here is a release.

**VM-4 is the first milestone since VM-1 to change the product assemblies, and it changed them
because a suite that could reach a second thread found four things a single-threaded suite could
not.** Each was proven by a test that failed before the fix and passes after it, and each is a rule
that was written down at VM-0, implemented at VM-1, and enforced nowhere:

| What was wrong | What the record said | Where it was found |
|---|---|---|
| A running host capability refused **every other thread's** call into the runtime | ADR 0011 F5: a capability that re-enters *the invoking runtime* is refused | A concurrent verification, refused while a capability ran on another thread |
| Instance and runtime disposal returned **while the profile was still executing**, then released the artifact lease and the retained bytes under it | ADR 0004: disposal is bounded and there is no use-after-dispose | A disposal raced against a step held inside the profile |
| `VmThreadAffinity.OperationThreadPinned` was carried in every descriptor and **read by nothing** | ADR 0002 row 8, ADR 0004: every call on a pinned operation arrives on the thread that started it | A pinned operation resumed from another thread, and answered `Normal` |
| An instantiation in flight when the runtime began disposing **registered its instance anyway** | ADR 0004: no instance outlives the runtime that made it | A disposal raced against an instantiation held inside the profile |

The first three are the same shape as the three VM-2 found: a bound that was declared, carried and
read nowhere. `VmReason.ThreadAffinityViolation` and the `DisposeDrainBudget` option were both in
the surface from VM-1 and neither had a code path.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry.

| Field | Where |
|---|---|
| Identity | Section 1 |
| Source | Section 2 |
| Dependencies and corpus | Section 3 |
| Environment | Section 4 |
| Procedure | Section 5 |
| Outputs | Section 6 |
| Decision | Section 7 |
| Validity | Section 8 |

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-4-001 |
| Milestone | VM-4 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-4 gate |
| Core contract version | 1, unchanged. VM-4 adds no public member and mints no amendment. |
| Reason-registry revision | 2, unchanged. No reason was added; one that existed and could not be produced now can be. |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

---

## 2. Source

Collected with a **dirty working tree**: this bundle's own record set is uncommitted at collection
time.

What VM-4 changed in the product assemblies, in full:

| File | Change |
|---|---|
| `VmRuntime.cs` | The in-capability depth becomes call-stack scoped; instance registration is refusable; runtime disposal shares one drain deadline across its instances; resume checks the declared thread affinity before consuming the token |
| `VmInstanceImplementation.cs` | Steps in flight are counted; disposal drains them under the host's `DisposeDrainBudget`; a disposal whose drain expires hands the lease release to the departing step |
| `VmOperation.cs` | An operation records the thread it started on and answers whether the current thread satisfies the declared affinity |
| `VmInstantiation.cs` | An instantiation refused by a disposing runtime disposes the instance it built, on both the synchronous and the asynchronous path |

What VM-4 added outside them: one test-only project (`Broiler.VM.Soak.Host`, authorised by ADR 0001
revision 2), four behavioural test files, and one execution gate in the fixtures.

---

## 3. Dependencies and corpus

The VM-2 corpus is unchanged and is replayed here in all three publish modes. VM-4 adds no corpus:
its subject is not what the core does with bytes but what it does with threads, and the instrument
for that is the execution gate rather than a file of inputs.

**The execution gate is worth naming as an instrument.** Every concurrency claim in this bundle is
*arranged* rather than raced: the fixture profile can be held inside a step - an invocation, an
instantiation, an unwind, or a host capability - while the test does the thing under test, and then
released. A test that started a thread and slept would be asserting that the collecting machine is
slow enough, which is the shape of a test that passes on one machine and fails in a lane. The gate
belongs to one descriptor rather than being a static, because xunit runs test classes in parallel.

---

## 4. Environment

| Field | Value |
|---|---|
| OS | Linux 6.18.44, glibc 2.39 |
| Architecture | x86_64 |
| RID | `linux-x64` |
| Processors | 4 |
| Configuration | Release |
| Publish modes | JIT, trimmed self-contained, Native AOT |
| Lane | None. One machine, one RID, no CI. |

**Four processors is a limit on what any concurrency evidence here is worth**, and it is recorded as
exclusion EX-88 rather than left for a reader to infer. Interleavings that need genuine parallelism
across many cores are not reached by four, and no test here can distinguish "correct" from "did not
happen to race" on a machine this size.

---

## 5. Procedure

Everything below was produced by `python eng/collect-evidence.py --bundle VM-4-001 --out
docs/evidence/vm-4`, run from the component root.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4-6 | Publish and run the fixtures host under JIT, trimming and Native AOT | `publish-jit-and-trimmed.log`, `publish-aot.log` |
| 7 | Replay the corpus from each of the three modes and compare the tables | `corpus-replay.log` |
| 8 | Eight seeded fuzz sessions of 250,000 iterations each | `fuzz.log` |
| 8b | Publish and run each composition in three modes; compare catalogs; list closures | `composition-*.log`, `catalog-*.txt`, `closure-*.txt` |
| 8c | **The soak run: 400,000 lifecycle cycles across four workers, sampled** | `soak.log` |
| 9 | Fourteen negative controls, each injected, run, reverted, and run again | `negative-control.log` |
| 10 | Environment and hashes | `environment.txt`, `hashes.txt` |

---

## 6. Outputs

**Build.** Thirteen projects build Release with 0 warnings and 0 errors.

**Tests.** 293 tests pass, 0 failed, 0 skipped — 99 architecture and 194 behavioural. VM-3 retained
262; VM-4 adds twenty-nine behavioural tests and two architecture rules, which is the shape of a
milestone whose subject is behaviour under concurrency.

**Pack.** Exactly three `.nupkg` and three `.snupkg`. None of the eight test-only projects packs and
neither composition root does.

**Publish and run.** The fixtures host publishes and runs in all three modes. The trimmed image is
78256 bytes, unchanged; the Native AOT image is 1565592 bytes, **8,304 bytes larger than VM-3's**.
That growth is the four corrections and it is reported rather than smoothed: this is the first
milestone since VM-1 whose product assemblies are not byte-identical to the previous one's.

**Both compositions** still publish and run in all three modes and pass their twelve checks, and
their closures are unchanged: five non-framework assemblies for the single-profile composition and
six for the two-profile one.

**The soak.** 400,000 lifecycle cycles across four workers, two profiles and 100 runtimes — create,
verify, instantiate, invoke, suspend, resume, cancel, abandon a pause, expire it, dispose the
instance, dispose the runtime — with the managed heap, the total allocated bytes and the working set
sampled throughout. 100,000 suspensions, 90,908 resumptions, 42,856 cancellations, no refusal and no
fault. `soak.log` retains every sample from both the JIT run and the trimmed self-contained image.

**The plateau.** The trimmed self-contained run allocated 1,923,955,560 bytes over its life and
settled at a managed heap of 66,216 bytes — against 127,096 at the first sample, before any cycle had run. The
working set plateaus around 170 MB and stops climbing well before the run ends, which is the GC
holding a heap it has already emptied rather than the component holding anything. The samples in
between rise and fall between collections, which is what a sampled heap does; the claim rests on the
settled figure, and on its being smaller than the first sample after nearly two gigabytes of
allocation.

**Recycling runtimes is part of the workload, and the first soak run is why.** It asked for 400,000
cycles and completed 161,616: a runtime's ceilings are a total allowance rather than a per-operation
one, so one long-lived runtime spends its fuel and refuses everything afterwards. That run was
measuring the moment its budget ran out. The host now recycles a runtime every four thousand cycles,
counts refusals rather than skipping them, and refuses to report a run that had any.

The host judges one thing about itself and nothing else: whether the run actually exercised what it
claims. A run that completed no cycles, never suspended, never resumed, never cancelled, faulted, or
was refused anything exits non-zero, because a beautifully flat line from a host that stopped
working is the most misleading output it could produce.

**Fuzz.** Eight sessions, 250,000 iterations each, no counterexample.

### 6.1 What the concurrency suite establishes

| Claim | How |
|---|---|
| A capability running on one thread does not refuse another thread's call | A verification issued while the capability is held inside its handler, on another thread, succeeds |
| A capability that re-enters its own runtime is still refused | The other half, on the same call stack, answering `ReentrantRuntimeCallFromCapability` |
| Disposal waits for a step to leave the profile | A disposal raced against a held step does not complete until the gate is released |
| A drain that expires still returns, and does not dispose the handle under its reader | With a 150 ms budget the disposal returns; the artifact goes to `Draining`; the departing step completes the drain |
| A pinned operation is not resumed on another thread | `ThreadAffinityViolation`, checked before the resume token is consumed, so the operation stays resumable on its own thread |
| An agile operation is resumed on any thread | The control, so the check is not a blanket refusal |
| One instance admits one step at a time | A second invocation against a held instance is refused `ReentrancyRefused` |
| Independent runtimes charge only their own work | Eight runtimes, twenty-five cycles each, identical totals |
| A shared aggregate budget is not multiplied | Concurrent runtimes under one parent; the total spent never exceeds the parent's ceiling |
| Concurrent disposals dispose once | Eight threads, exactly one `Accepted`, the rest `NoOp` |
| Every public call on a disposed runtime answers invalid state | Verify, instantiate, invoke, cancel and poll, each answering `InvalidState` |
| A guest-initiated load in flight is cancelled with its operation | The provider cancels the requesting operation while inside its answer |
| An abandoned pause is ended by residency expiry | The freed slot is reusable, which is the observable half |

### 6.2 Diagnostics, and what they cannot carry

Rule V11 holds the diagnostics record to a shape with nowhere to put free text: every member is an
enum, a number, or one of four identity types. Three of the four are validated under a closed
grammar and name the profile, its manifest and a capability; the fourth is the caller identity,
whose content the caller itself supplied and can decline to supply. A host capability that throws an
exception carrying a connection string produces a host failure that names the capability and its
version and carries none of the string — not because the call sites are careful but because the
record has no field for it.

Rule V12 holds the five profile-facing contracts to trafficking in the contract's own types: no
member takes or returns `object`, a `Type`, a delegate, a reflection type, an assembly load context
or a raw pointer. The capability table answers exactly four questions — how many bindings, whether
one is bound, invoke with integers, invoke with bytes — which is what keeps it from becoming a
directory of what the host can do.

### 6.3 The negative controls

Fourteen, each injected, run, reverted and run again. Three are new at VM-4 and each pins one of
this milestone's own corrections: disposal that stops waiting for a step inside the profile, a
resume that no longer checks the declared affinity, and a disposing runtime that accepts an instance
registration again.

---

## 7. Decision

The VM-4 gate, clause by clause. The marks are evidence verdicts set by the author about what the
retained evidence shows; no reviewer has set anything.

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[MET]` | Stress and soak suites show deterministic isolation | Eight independent runtimes over one catalog, twenty-five cycles each, produce identical consumption totals; a handle verified by one runtime serves another without carrying either one's state; two runtimes' budgets move independently. |
| `[MET]` | Bounded cancellation | Cancellation reaches a step already inside the profile and the operation ends `Cancellation`; a runtime-wide cancel reaches every live operation from another thread; a cancelled token is observed at the profile's declared poll bound. |
| `[MET]` | Correct host-exception translation | The two declared modes are exercised: a throwing capability declared `TerminateOperation` ends the operation as a host failure naming the capability, and one declared `ObservableFault` is reported to the profile. Neither carries anything the exception said. |
| `[MET]` | No cross-runtime state leakage | The isolation suite above, plus rule V12's structural half: nothing a profile holds can name a CLR type, so there is no route from one composition into another that does not go through the host's own registrations. |
| `[MET]` | No use-after-dispose | This is the clause VM-4 actually fixed. Disposal now drains in-flight steps under the host's own bound; a drain that expires hands the lease release to the departing step rather than disposing the verified state under a profile that is still reading it; an instantiation racing disposal is refused and gives its instance back. |
| `[MET]` | A declared memory plateau | 400,000 cycles across four workers and 100 runtimes, sampled throughout, and the trimmed self-contained run settling at a managed heap of 66,216 bytes after allocating 1,923,955,560 - below its own first sample. The figure is a measurement of a published image rather than a metered counter, and the host refuses to report a run that was refused anything. |
| `[MET]` | A guest-initiated load in flight is cancelled and disposed with its requesting operation and leaves no partially verified state | The provider cancels the requesting operation while inside its answer; the operation ends `Cancellation`, the instance faults, and no handle from the nested load reaches anyone. |
| `[MET]` | An externally suspended operation resumes, cancels, or disposes deterministically and never blocks disposal indefinitely | Disposal unwinds a paused operation on the disposing thread and returns; the resume token is then dead and says so; an abandoned pause is ended by residency expiry and its slot is reusable. |
| `[MET]` | A shared aggregate budget is honored by concurrent runtimes rather than multiplied by them | Concurrent runtimes under one parent; the parent's own total never exceeds its ceiling however the interleaving fell out, and a parent that can no longer cover a runtime's stated ceiling refuses to admit one. |
| `[MET]` | Diagnostics identify profile, version, and artifact locations without leaking host secrets | Section 6.2, both halves: the identifying half asserted against real failures at four stages, the leaking half asserted structurally by V11 and behaviourally against a capability that throws a secret. |
| `[MET]` | Host imports cannot reach undeclared CLR surface | Rule V12 over the five profile-facing contracts, with a witness interface that hands back a `Type` and an `object`. |
| `[PART]` | Thread affinity | `OperationThreadPinned` is now enforced on resume, which is the only place a second thread can enter an existing operation, and `ThreadAffinityViolation` is reachable for the first time. What is **not** enforced is affinity across a profile's own internal threading, which the core cannot see. Exclusion EX-44 is closed; EX-89 records what replaces it. |

### 7.1 Deviations recorded rather than amended

| Deviation | Why it is an erratum |
|---|---|
| The in-capability flag is call-stack scoped, where ADR 0011 F5 says "per-runtime" | The record's sentence is about a capability re-entering *the invoking runtime*, which is a property of a call stack. Implemented per-runtime it refused every other thread's unrelated call for the duration of any capability, so a host capability that took a lock stopped the runtime for every caller. The narrower scope enforces the sentence the record states; the wider one enforced something the record does not say. Exclusion EX-90. |
| A refused capability re-entry answers `ReentrantRuntimeCallFromCapability`, where ADR 0011 F5 names `ReentrancyRefused` | Carried over from VM-1 and left alone here. The implemented reason is strictly more specific and maps to the same category; `ReentrancyRefused` is produced for the instance-level case, so collapsing them would lose the distinction between "this instance is busy" and "you are inside a capability". Exclusion EX-91. |

---

## 8. Validity

**Reproduction.** `python eng/collect-evidence.py --bundle VM-4-001 --out docs/evidence/vm-4`.

**Expiry.** The figures here are true of the logs as retained. Rule H5 holds every quoted headline
figure to those logs and cannot hold the logs to the checkout. Exclusion EX-54 records that.

**Recertification triggers.** Any of these invalidates this bundle:

- a change to any file in `hashes.txt`;
- a change to the core contract version or the reason-registry revision;
- a change to the disposal drain, the affinity check or the capability-reentrancy scope, which are
  the three corrections the concurrency suite pins;
- a machine with a materially different processor count, since EX-88 records that four is what the
  concurrency evidence was collected on;
- an SDK change, since none is pinned.

---

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. `environment.txt` records what this machine resolved. |
| EX-25 | Open | **The persisted envelope is admitted by the contract and implemented by no milestone.** Unchanged. |
| EX-42 | Open | The Native AOT publish on `win-x64` needs a `vcvars64` environment. It did not apply to this collection. |
| EX-44 | Closed | **Closed 2026-08-29 by this bundle.** VM-1 recorded that the declared thread affinity was carried and never exercised across threads. `OperationThreadPinned` is now enforced on resume, `ThreadAffinityViolation` is reachable, and both the pinned refusal and the agile control are asserted from a second thread. What the closure does not cover is recorded as EX-89. |
| EX-45 | Open | **One RID, one machine, one lane.** |
| EX-52 | Open | **Twenty-nine review findings of major and minor severity remain unaddressed.** VM-4 did not work that list. |
| EX-54 | Open | Rule H5 checks document against log, not log against checkout. |
| EX-78 | Open | **The guest-load nesting-depth bound is unreachable at contract version 1.** Unchanged. |
| EX-79 | Open | **The corpus retains no minimized regression, because no session has found one.** Unchanged. |
| EX-80 | Open | **The fuzz session varies the payload and never the descriptor**, and fuzzes no consumer profile. Unchanged. |
| EX-81 | Open | **Rule H5 admits a figure from any bundle a document links.** Unchanged. |
| EX-82 | Open | **A verification that fails records no clamp.** Unchanged. |
| EX-83 | Open | **A refused budget override populates a diagnostics group ADR 0005 annotates for a different category.** Unchanged. |
| EX-84 | Open | **The cross-mode table is produced under one descriptor.** Unchanged. |
| EX-85 | Open | **`DescriptorMismatch` is unreachable through the public path** for all three profiles. Unchanged. |
| EX-86 | Open | **Rules K3 and K4 compare against the last collection, not against the working tree.** Unchanged. |
| EX-87 | Open | **The closure report excludes framework assemblies by name prefix.** Unchanged. |
| EX-88 | Open | **Every concurrency result here was collected on four processors.** Interleavings that need genuine parallelism across many cores are not reached, and no test can distinguish "correct" from "did not happen to race" on a machine this size. The execution gate makes each *arranged* race deterministic, which is a different and narrower claim than "this code is correct under arbitrary parallelism". Closed by: a lane with a machine class per RID, or a model checker over the lifecycle. |
| EX-89 | Open | **Thread affinity is enforced where the core can see a thread, and nowhere else.** `OperationThreadPinned` is checked on resume, which is the only place a second thread enters an existing operation through the public surface. A profile that starts its own threads and calls its own state from them is invisible to the core, and a profile that declares the pin while doing so is declaring something the core cannot enforce. Cancellation and disposal deliberately remain callable from any thread whatever the affinity says, because ADR 0009's guarantee G1 is that a parked operation can always be cancelled and disposed - a pinned profile whose thread has gone would otherwise be undisposable. Closed by: nothing in contract version 1; the core has no view of a profile's internal threads and acquiring one would mean owning its scheduler. |
| EX-90 | Open | **The in-capability flag is call-stack scoped, where ADR 0011 F5 says per-runtime.** Recorded as an erratum in section 7.1. The residual limit is worth stating: the flag flows with the execution context, so a capability that calls back through `Task.Run` or a new `Thread` is still caught, and one that suppresses execution-context flow is not. Closed by: an amendment to F5's wording, or a reviewer rejecting the reading. |
| EX-91 | Open | **A refused capability re-entry answers a more specific reason than ADR 0011 F5 names.** Recorded as an erratum in section 7.1. Closed by: an amendment, or a decision to collapse the two reasons. |
| EX-92 | Open | **`VmReason.UnwindTimedOut` cannot be produced at contract version 1.** ADR 0004 says expiry completes the operation with it, and the allowance it would be measured against is denominated in the profile's own work units - the terminal-unwind entry point takes an allowance and no meter, so the core has nothing to observe it against and cannot preempt a profile that ignores it. What IS bounded is the core's own waiting: `DisposeDrainBudget` is wall-clock, is now read, and is what makes disposal return. The reason is left unreachable and said so rather than being produced by a timer the records forbid. Closed by: an amendment giving the unwind entry point a meter, which would make the allowance chargeable. |
| EX-93 | Open | **`ForeignOpaqueRef` and `StaleOpaqueRef` cannot be produced at contract version 1.** The core mints no opaque reference and consumes none: a host handler produces one, the core carries it to the profile that asked, and no member takes one back - which a test asserts, so the day a consuming member appears the assertion fails and whoever added it must implement the check. Closed by: a member that accepts a reference, which is an amendment. |
