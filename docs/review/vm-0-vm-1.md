# Reviewer worksheet: VM-0 and VM-1

This is a worksheet, not a record. It is where a reviewer reads item by item and
notes what they found. The review itself - the area verdicts, the decision, and any
conditions - is recorded in [HUMAN_REVIEW.md](../../HUMAN_REVIEW.md), not here.

The verdict cells below take the review vocabulary `[ ]`, `[A]`, `[C]`, `[R]` and `[?]`,
whose canonical legend is
[section 1 of HUMAN_REVIEW.md](../../HUMAN_REVIEW.md#1-how-to-use-this-file); this file
carries no second copy. Every verdict here is `[ ]`. None has been pre-filled, because
nothing in this repository has been read by a human reviewer and a filled mark would be
a false record.

Paths are relative to the component root `D:\Broiler.Browser\Broiler.VM`.

## Progress

One row per review area, in the risk order fixed by
[section 4 of HUMAN_REVIEW.md](../../HUMAN_REVIEW.md#4-review-route). Area verdicts are
not recorded here; they belong in
[section 8](../../HUMAN_REVIEW.md#8-area-verdicts).

| Area | Name | Items | Finished (date, initials) |
|---|---|---|---|
| [RA-1](#ra-1---bounded-reading-of-untrusted-bytes) | Bounded reading of untrusted bytes | 6 | |
| [RA-2](#ra-2---resource-authority-and-budgets) | Resource authority and budgets | 7 | |
| [RA-3](#ra-3---lifecycle-and-state-machine) | Lifecycle and state machine | 7 | |
| [RA-4](#ra-4---verified-artifact-ownership) | Verified-artifact ownership | 6 | |
| [RA-5](#ra-5---guest-initiated-loads-and-external-suspension) | Guest-initiated loads and external suspension | 7 | |
| [RA-6](#ra-6---the-public-contract-surface) | The public contract surface | 7 | |
| [RA-7](#ra-7---the-records-themselves) | The records themselves | 7 | |
| [RA-8](#ra-8---the-evidence-and-the-rule-register) | The evidence and the rule register | 6 | |
| | **Total** | **53** | |

## RA-1 - Bounded reading of untrusted bytes

### RC-01 - Confirm every bound is compared before the value it bounds is used

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Binary/VmBoundedReader.cs` 164-188 (`TryReadDeclaredCount`), `src/Broiler.VM.Binary/VmBoundedAllocator.cs` 34-68 and 80-137; ADR 0007 section *The precedence algorithm*, steps P2 and P5 |
| Check | Decide whether the order in each primitive is bound-check, then checked size arithmetic, then meter reservation, then allocation - so a hostile declared count costs nothing proportional to itself. |
| Falsified if | Any path returns a count, sizes a buffer, or reaches `new T[...]` before its bound comparison or before `meter.TryReserve` has returned true; or the `checked` multiplication at `VmBoundedAllocator.cs:57` can be bypassed for some `T`. |

### RC-02 - Confirm a spent reader stays spent and retains its first cause

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Binary/VmBoundedReader.cs` 21-27 (the claim), 409-421 (`Check`, `Fail`), and every public member that calls them; `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` 189-204; ADR 0006 section *5. The verification failure taxonomy* |
| Check | Decide whether one failure can be stepped past by a caller that ignored a return value, and whether the retained status is the first cause rather than its echo. |
| Falsified if | A public member on `VmBoundedReader` examines `bytes` or advances `position` when `Status` is not `Ok`, or a later `Fail` overwrites an earlier non-`Ok` status. |

### RC-03 - Confirm the framing arithmetic cannot wrap and no foreign frame can enter it

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Binary/VmBoundedReader.cs` 204-291 (`TryEnterSection`, `TryExitSection`, `TrySkipSectionBody`) and 307-345 (`TryTake`, `TryConsume`); `src/Broiler.VM.Binary/VmSectionFrame.cs` 13-20; ADR 0007 section *The fifteen budgeted dimensions*, rows 11-14 |
| Check | Decide whether `frame.Start + frame.DeclaredLength` at lines 258 and 277 is safe, given that the additions are unchecked while the additions in `TryConsume` are checked, and whether every frame reaching those members was minted by this reader. |
| Falsified if | A `VmSectionFrame` with values the reader never produced can reach `TryExitSection` or `TrySkipSectionBody` - for example a default frame, or one kept across two readers - and the sum wraps or the `(int)` casts at lines 96, 316 and 366 index outside the span. |

### RC-04 - Confirm the canonical variable-length encoding is the only accepted one

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Binary/VmBoundedReader.cs` 139-162 and 347-397 (`TryReadVarUInt64Core`); `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` 175-187; ADR 0006 section *5. The verification failure taxonomy* |
| Check | Decide whether the three guards - shift past `maxBits`, an overflowing final group, and a redundant zero continuation - together admit exactly one encoding per value at both 32 and 64 bits. |
| Falsified if | Two distinct byte sequences both return true from `TryReadVarUInt32` (or `TryReadVarUInt64`) with the same `value`; the 64-bit case at `shift = 63` is the one to try, since `1UL << (maxBits - shift)` is evaluated there. |

### RC-05 - Confirm every member of the read-status set is producible and says one thing

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Binary/VmBoundedReadStatus.cs` 16-44; `src/Broiler.VM.Binary/VmBoundedReader.cs` 399-407 (`ChargeWork`); `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` 36-46; `src/tests/Broiler.VM.Fixtures/FixtureVmVerifier.cs` around line 353; ADR 0007 section *Scope, and what a resource-exhaustion result names*, the paragraph ruling that a configured limit and an expressed intent are different facts |
| Check | Decide whether a status a reader can never latch belongs in a closed enum that profile verifiers map arm by arm, and whether a caller of the reader alone can tell a work exhaustion apart from a cancellation. |
| Falsified if | `VmBoundedReadStatus.AllocationRefused` is assigned nowhere in `src/Broiler.VM.Binary`, so the fixture's mapping arm for it is unreachable; or `ChargeWork` latches `WorkBudgetExhausted` for both a refused `TryChargeWork` and a `Poll` that returned false because cancellation was requested. |

### RC-06 - Decide whether the "refuses before allocating" test can fail at all

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` 147-173 (`The_Bounded_Reader_Refuses_A_Declared_Count_Before_Allocating`, `The_Bounded_Allocator_Refuses_Before_Allocating`) and 248-263 (`CountingMeter`); `src/Broiler.VM.Binary/VmBoundedReader.cs` 399-407 |
| Check | Decide whether `Assert.Equal(0ul, meter.Reserved)` in the reader test discriminates between an implementation that refuses before reserving and one that does not. |
| Falsified if | `VmBoundedReader` never calls `IVmBoundedAllocationMeter.TryReserve` on any path, so `meter.Reserved` is zero for every input the reader is given and the assertion holds independently of the behaviour it names. |
| Prior finding | Negative control 4 - four assertions were one level too shallow, asserting a reaction rather than the thing under test. |

## RA-2 - Resource authority and budgets

### RC-07 - Confirm a charge is all-or-none and names the outermost refusing scope

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmMeter.cs` 14-19 (the stated tie-break) and 110-166 (`TryCharge`); `src/Broiler.VM.Runtime/VmBudgetLevel.cs` 29-57; ADR 0007 sections *The precedence algorithm* (step P5) and *Scope, and what a resource-exhaustion result names* |
| Check | Decide whether every link is checked before any link is applied, and whether the reported `FailedScope` is the outermost link that would have refused rather than the first one examined. |
| Falsified if | Any level commits while another refuses - including the window between the parent's `RemainingFor` read at line 130 and the parent's `TryCharge` at line 152 - or a refusal reports `Invocation` where the runtime or the parent would also have refused. |

### RC-08 - Confirm the parent is charged before the local commit and credited only what it took

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmMeter.cs` 218-289 (`ReportRetained`, `ReportReleased`); `src/Broiler.VM.Runtime/VmAggregateBudget.cs` 372-418 (`TryCharge`, `Release`); `src/Broiler.VM.Runtime/VmBudgetLevel.cs` 44-57; ADR 0007 sections *The shared aggregate budget is a core object* and *Monotonicity, and what raising a ceiling costs* |
| Check | Decide whether the retain and release pair is symmetric at the parent - that a retention the parent refused can never afterwards be released from it, and that an allowance-class dimension never refunds at any level. |
| Falsified if | `ReportRetained` commits at the runtime, instance or invocation level on a path where `parent.TryCharge` returned false; or `ReportReleased` credits the parent an amount larger than the parent accepted; or the clamp at line 276 uses a level whose consumed value can diverge from the parent's debit. |
| Prior finding | Asymmetric accounting - a retention the parent refused was still released from the parent, driving the aggregate live sum below the true sum and then to zero. |

### RC-09 - Confirm wall-clock attribution survives a refusal and stops under a suspension

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmMeter.cs` 316-394 (`PauseWallClock`, `ResumeWallClock`, `AccrueWallClock`) and 169-216 (`Poll`); `src/Broiler.VM.Runtime/VmOperation.cs` 195-240 and 285-333; ADR 0007 sections *The fifteen budgeted dimensions* (the `WallClock` ruling) and *Budget accounting across a suspension* |
| Check | Decide whether a delta the parent refuses is re-offered rather than dropped, whether the clock is paused under all three suspension origins, and whether one elapsed delta can be attributed twice. |
| Falsified if | A refused delta is committed at the runtime, instance or invocation level; or the clock accrues while `pauseStartedAt >= 0` is not held for the whole parked interval; or two threads entering `AccrueWallClock` can both read the same `attributed - already` outside the lock at lines 357-386 and both commit it. |
| Prior finding | Asymmetric accounting - a refused wall-clock charge was silently dropped, permanently under-summing attributed time. |

### RC-10 - Confirm a spent parent admits neither a new runtime nor a resumption

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmAggregateBudget.cs` 275-359 (`TryAdmitRuntime`, `IsSpent`, `AdmitsResumption`); `src/Broiler.VM.Runtime/VmRuntime.cs` 126-141 and 282-315; ADR 0007 sections *The shared aggregate budget is a core object* and *The precedence algorithm* (step P4, `ParentExhausted`) |
| Check | Decide whether `IsSpent` covers exactly the allowance-class aggregate dimensions it claims, and whether the resume admission check runs before the operation is resumed rather than after. |
| Falsified if | The resume check at `VmRuntime.cs:287` runs after `operation.Resume`, or after `suspension.TryConsume` has already spent the token in a way that cannot be undone; or `IsSpent` returns false for a parent whose `Fuel`, `WallClock`, `AllocatedBytes`, `HostCalls`, `NestedLoadFanOut`, `NestedLoadBytes` or `VerifierWork` is fully consumed. |
| Prior finding | Missing admission checks - no resume admission check existed at all, and a parent whose allowance was fully spent still admitted new runtimes. |

### RC-11 - Confirm a nested verification inherits a remainder for allowances and a ceiling for ceilings

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmVerification.cs` 147-166; `src/Broiler.VM.Runtime/VmMeter.cs` 97-107 (`RemainingSnapshot`); `src/Broiler.VM.Runtime/VmBudgetLevel.cs` 64-77 (`AsRemainingVector`, `AsCeilingVector`); ADR 0007 section *The precedence algorithm*, the paragraph beginning "For a nested (guest-initiated) verification" |
| Check | Decide whether the substitution the record requires at P2 is performed per dimension class, so that a nested load can exhaust an invocation and can never enlarge one, without also shrinking a ceiling that is not an allowance. |
| Falsified if | `RemainingSnapshot` returns `AsRemainingVector()` for all fifteen dimensions, so a ceiling-class dimension such as `LiveBytes`, `CallDepth`, `NestedLoadDepth`, `ArtifactBytes`, `SectionCount`, `DeclaredCount` or `StructuralDepth` is supplied as `ceiling - consumed` rather than as the effective ceiling. |

### RC-12 - Confirm the `Artifact` scope is reachable in a resource-exhaustion result

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmMeter.cs` 14-19 (the documented five-level tie-break) and 22-34 (the four fields the chain actually holds); `src/Broiler.VM.Runtime/VmVerification.cs` 167-174; `src/Broiler.VM.Abstractions/VmBudgetVocabulary.cs` 78-104 and 217-238 (`IsDeclarableAt`); ADR 0007 section *Scope, and what a resource-exhaustion result names* |
| Check | Decide whether the five-member scope set is enforced or partly decorative - `VerifierWork` is declarable at `Runtime`, `Artifact` and `Aggregate` only, and `ArtifactBytes`, `SectionCount`, `DeclaredCount` and `StructuralDepth` at `Runtime` and `Artifact` only. |
| Falsified if | The meter chain contains no level whose `Scope` is `VmBudgetScope.Artifact`, so `VmBudgetScope.Artifact` reaches a result only from the single hard-coded site at `VmVerification.cs:173` and an artifact-scoped dimension charged through `TryCharge` is reported at `Invocation`, `Instance`, `Runtime` or `Aggregate`. |

### RC-13 - Decide whether the budget regressions assert bounds tight enough to fail

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/ReviewRegressionTests.cs` 177-214 (`afterRelease >= 900`), 396-442 (`RequestCount <= 8`) and 522-545 (`Assert.NotEqual(VmOutcome.Normal, ...)`); `src/tests/Broiler.VM.Contract.Tests/ReclamationTests.cs` 18-48 (`after < held`); `docs/evidence/vm-1/README.md`, section *The negative controls, in detail* |
| Check | Decide, for each of these four assertions, whether it excludes the wrong behaviour it is named for or merely a subset of it. |
| Falsified if | An implementation that never calls the provider at all still satisfies `RequestCount <= 8`; or one that reclaims a single byte still satisfies `after < held`; or one that over-charges the parent still satisfies `afterRelease >= 900`; or one that fails a provider call for a reason unrelated to `HostCalls` still satisfies `Assert.NotEqual(VmOutcome.Normal, ...)`. |
| Prior finding | Negative control 4 - the tests asserted the profile's reaction rather than the core's reason, and four assertions were one level too shallow. |

## RA-3 - Lifecycle and state machine

### RC-14 - Confirm all eight rows of the outcome-to-instance-state mapping

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` 559-616 (`Settle`) and 618-646 (`TryAdmit`); `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs` 9-31; ADR 0004 section *The Instance And The Outcome Mapping*, the table headed "Outcome to instance state. Mandatory; no implementation freedom." |
| Check | Decide whether each of the eight outcomes moves the instance exactly where the record says, and whether `Faulted` afterwards admits only disposal and a diagnostics read. |
| Falsified if | `Suspension` reaches the switch and changes the state; or `ResourceExhaustion`, `Cancellation` or `HostFailure` leaves the instance anything other than `Faulted`; or `ProfileFault` does not consult `profile.FaultRecovery`; or `InvalidState` or `UnsupportedProfile` mutates the state. |
| Prior finding | Mandatory mappings not implemented - the mapping collapsed cancellation, exhaustion and host failure to `Live`, leaving an instance re-invocable after its stack was abandoned mid-step. |

### RC-15 - Confirm the frozen precedence order is applied at every stage, not only at invoke

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` 329-374 and 431-464 (the two precedence heads); `src/Broiler.VM.Runtime/VmInstantiation.cs` 145-188 (the instantiation stage's checks) and 149 (`profileState.Scope.Enter(meter)`); `src/Broiler.VM.Runtime/VmExecutionScope.cs` 29-45 and 150-173 (`Latch`); ADR 0005 section *Precedence and observation order* |
| Check | Decide whether the instantiation stage applies the same nine-step order the invocation and resume stages do, given that the record states it is one order for every stage. |
| Falsified if | `VmInstantiation.Instantiate` enters the execution scope with no owning operation, so `VmAmbientCapabilityInvoker.Latch` has nothing to latch a terminating host failure onto; and the stage's result switch tests neither `HostFailure` nor `PollBoundExceeded`, so a capability declaring `TerminateOperation` that throws during instantiation is reported as the profile's own answer. |
| Prior finding | Inverted precedence, and *Declarations enforced nowhere* - a capability declaring `TerminateOperation` never terminated anything, so a host defect was billed to the guest. |

### RC-16 - Confirm the capability reentrancy gate covers the calls the record names and no others

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs` 462-472, 491-505 and 507-539 (`TryBeginCall`), plus `Dispose` 409-451, `RequestCancel` 373-390 and `PollDeadlines` 332-371; `src/Broiler.VM.Runtime/VmCapabilityBinding.cs` 167-199; ADR 0004 section *Reentrancy*, rules 2 and 4, and section *Disposal, Draining, And Orphaning*, the reentrant-self-disposal paragraph |
| Check | Decide whether the gate matches the record's permitted set - a cancellation request, an external-suspension request, a diagnostics read, `PollDeadlines`, and `Verify` where `MaxConcurrentVerifications` allows it - and its forbidden set. |
| Falsified if | `Verify` called from inside a bound non-reentrant capability returns `InvalidState` even though rule 2 permits it, or the reason returned is not the `ReentrancyRefused` the record names; or `Dispose`, `RequestCancel` and `PollDeadlines` reach their bodies from inside one, since none of them calls `TryBeginCall`. |
| Prior finding | EX-52 - the non-reentrancy gate is absent on `Dispose`, `RequestCancel` and `PollDeadlines`. |

### RC-17 - Confirm disposal drains boundedly and records what expiry produces

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs` 401-451; `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` 150-187 and 279-296 (`Unwind`); `src/Broiler.VM.Runtime/VmInstantiation.cs` 398-414; `src/Broiler.VM.Runtime/VmRuntimeCreationOptions.cs` 239-276; ADR 0004 section *Disposal, Draining, And Orphaning*, the paragraphs *Racing an in-flight operation* and *Racing a suspended operation* |
| Check | Decide whether the two bounds the record names are enforced: the wall-clock drain wait for an in-flight operation, and the unwind allowance whose expiry completes the operation `Cancellation` with reason `UnwindTimedOut` and faults the instance. |
| Falsified if | `VmRuntimeCreationOptions.DisposeDrainBudget` is read nowhere outside its own declaration, so `Dispose` never waits, never orphans an in-flight operation and never records a drain-expiry diagnostic; or `VmReason.UnwindTimedOut` is produced nowhere in the product graph while `Unwind` swallows every exception and enforces no time bound of its own. |

### RC-18 - Confirm the suspended set is keyed by identity and always released on abandonment

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs` 593-615 (`TryPark`, `Unpark`), 642-643 (`OperationKey`) and 262-315 (the resume lookup); `src/Broiler.VM.Runtime/VmOperation.cs` 56 (`Key`) and 339-395 (`Abandon`, `Complete`); ADR 0004 sections *The Operation* and *Disposal, Draining, And Orphaning* |
| Check | Decide whether a terminal operation always gives back its live-suspended slot, and whether the key the runtime files a parked operation under actually identifies it. |
| Falsified if | `OperationKey` reduces a `VmObjectId` to `unchecked((ulong)operationId.GetHashCode())`, so two live operations whose identities hash equal occupy one entry in `suspended`: parking the second evicts the first, and `Resume` can dispatch a suspension to an operation it does not belong to. |
| Prior finding | Leaks - abandonment never unparked, so a dead operation consumed a live-suspended slot for the life of the runtime. |

### RC-19 - Confirm no half-instantiated instance is published or misreports its state

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmInstantiation.cs` 189-263 (the suspended arm, including `PlaceholderState` and `runtime.RegisterInstance(pending)` at line 256); `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` 27 (`currentState = VmInstanceState.Live`) and 189-235 (`ResumeOperation`); `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs` 9-31; ADR 0004 section *The Instance And The Outcome Mapping*, the transition table rows for `Instantiating` |
| Check | Decide whether the record's `Instantiating` state exists in practice, and whether an undeclared asynchronous instantiation is answered as the record requires after a bounded abandon. |
| Falsified if | The placeholder instance registered while its instantiation is still parked reports `VmInstanceState.Live` rather than `Instantiating` or `Suspended`; or a runtime disposal that walks `instances` disposes a placeholder the caller was never given and the record says is never published. |
| Prior finding | A declared asynchronous instantiation was reported as a profile fault. |

### RC-20 - Decide whether the lifecycle regressions exercise more than the invoke stage

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/ReviewRegressionTests.cs` 19-74 (the mapping cases) and 122-155 (`A_Terminate_Operation_Capability_Ends_The_Operation_As_A_Host_Failure`); `src/tests/Broiler.VM.Contract.Tests/LifecycleTests.cs` 148-200; ADR 0004 section *The Instance And The Outcome Mapping*; ADR 0005 section *The seven envelope-bearing stages* |
| Check | Decide whether the mapping and the precedence order are tested at each stage that can produce them, or only where the fixture happens to make them easy to produce. |
| Falsified if | No test drives a terminating host failure, a poll-bound breach or an unconverted capability fault through the instantiation stage or through `Resume`, so a stage that omits those precedence steps entirely still passes every behavioural test. |
| Prior finding | Negative control 4 - a green suite is evidence about the contract only to the extent that the tests were written to catch the contract being broken. |

## RA-4 - Verified-artifact ownership

### RC-21 - Confirm the single construction site is asserted at the granularity claimed

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` 252-280 (the hidden factory and its remark); `src/Broiler.VM.Runtime/VmVerification.cs` 253-266 (the one call); `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs` 365-408 (rule V9); ADR 0006 sections *2. What a successful verification binds* and *6. Requirement V-SEP*; `docs/evidence/vm-1/README.md` exclusion EX-51 |
| Check | Decide whether V9 asserts what the one-construction-site rule claims, now that the exclusion states plainly that it does not count call sites. |
| Falsified if | V9's producer scan is restricted to public static declared-only methods, so a public *instance* member returning a `VmVerifiedArtifact` is not counted; or its caller check is per-assembly, so a second call to `VmVerifiedArtifact.Create` added anywhere inside `Broiler.VM.Runtime` leaves the rule green. |
| Prior finding | Rules weaker than their statements - V9 asserted return types rather than the construction site. |

### RC-22 - Confirm who makes the copy for a `Snapshot` profile

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmVerification.cs` 138-186 (the payload span handed to `profile.Verifier.Verify`) and 253-266 (what the handle is built from); `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` 225-247 (the handle's stated ownership) and 281-307; `src/Broiler.VM.Abstractions/VmProfileDescriptor.cs` 138-141; ADR 0006 section *1. The input boundary and the representation choice*, the bullet *The core makes the copy* |
| Check | Decide whether the record's rule is implemented or delegated: the core is required to allocate a core-owned buffer for a `Snapshot` profile and hand the verifier a span over the core's copy, with that buffer becoming the handle's byte store. |
| Falsified if | `RunVerifier` passes the caller's `payload` span directly to the verifier for every representation kind, and the handle retains only `outcome.State`, so a profile declaring `Snapshot` produces a handle holding no core-owned bytes and invariant 3 rests on the verifier's own discipline. |
| Prior finding | EX-52 - `VmArtifactRepresentationKind.Snapshot` never causes the core to retain bytes. |

### RC-23 - Confirm the lease contract, the drain, and what disposal releases

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` 186-223 (`VmArtifactLease`), 370-389 (`TryGetState`) and 391-461 (`TryAcquireLease`, `Dispose`, `ReleaseLease`); `src/Broiler.VM.Runtime/VmInstantiation.cs` 74-96; `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` 150-187; ADR 0006 section *3. States, lifetime kinds, and the lease contract*, including the paragraph *What the kind changes* |
| Check | Decide whether every transition in the record's state table is implemented, whether the core's implicit instance lease is taken and released exactly once, and whether the `Disposable` kind does what the record says it does. |
| Falsified if | `VmVerifiedArtifact.Dispose` and `ReleaseLease` run no profile release at the transition to `Disposed`, so `VmArtifactLifetimeKind.Disposable` and `Managed` are indistinguishable; or an instantiation that fails after `TryAcquireLease` leaves the lease held; or a lease is created anywhere other than `TryAcquireLease`. |
| Prior finding | Leaks - instantiation took no lease, so a handle backing a live instance went straight to `Disposed` instead of draining. EX-52 - `VmArtifactLifetimeKind.Disposable` releases nothing. |

### RC-24 - Confirm V-SEP holds structurally, not by convention

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs` 167-198 (`Verify`, and the remark on its closed parameter set); `src/Broiler.VM.Runtime/VmVerification.cs` 1-45 (`VmVerificationContext`) and 176-186; `src/Broiler.VM.Runtime/VmProfileRuntimeState.cs` 125-164 (the lazy executor); `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs` 411-434 (V9b); ADR 0006 section *6. Requirement V-SEP*, the property table |
| Check | Decide whether each of the six properties - one entry point, sufficiency, totality, capability isolation, provider isolation, lazy executor - is enforced by structure rather than asserted. |
| Falsified if | The verification context reaches anything a verifier could invoke rather than capability *descriptors* alone; or an executor is created on a path that does not instantiate; or a second member can produce a handle; or a guest-initiated load can be admitted while a profile verifier frame is on the stack. |

### RC-25 - Confirm identity component 6 carries two distinct ceiling vectors

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmVerification.cs` 155-163 (`new VmEffectiveCeilings(effective, effective)`); `src/Broiler.VM.Abstractions/VmLimitVector.cs` 300-332 (`VmEffectiveCeilings`); `src/Broiler.VM.Runtime/VmInstantiation.cs` 107-115 (the instance level built from `InstantiationCeilings`) and 312-383 (`TryAdmitSharing`); `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` 122-166 (`FirstMismatch`); ADR 0006 sections *2. What a successful verification binds* (component 6) and *4. The cross-runtime sharing predicate*; ADR 0007 section *The precedence algorithm*, step P3 |
| Check | Decide whether the verification and instantiation halves of component 6 can ever differ, and what clause 8's exact-equality comparison is therefore comparing. |
| Falsified if | Verification constructs the pair from one vector used twice, so `InstantiationCeilings` is never distinct from `VerificationCeilings` and P3's instance-override layer has no input to tighten; or `TryAdmitSharing` builds the receiving identity from a single vector at `VmInstantiation.cs:344-355` for the same reason. |

### RC-26 - Decide whether the caller-buffer test proves anything about the core

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` 111-129 (`Mutating_The_Callers_Buffer_After_Verification_Changes_Nothing`); `src/tests/Broiler.VM.Fixtures/FixtureVmProfile.cs` 187-188; `src/tests/Broiler.VM.Contract.Tests/ContractSurfaceTests.cs` 62-63 and `src/tests/Broiler.VM.Contract.Tests/CatalogRegistrationTests.cs` 178-179; ADR 0006 section *1. The input boundary and the representation choice*, the bullets *The core makes the copy* and *Truthfulness* |
| Check | Decide whether clearing the caller's array and re-reading the result discriminates between a core that owns the bytes and a fixture verifier that happened to decode eagerly. |
| Falsified if | Every descriptor in the test tree declares `VmArtifactRepresentationKind.Decoded`, so the test exercises the branch in which the record requires no copy at all and no test declares `Snapshot`; the ownership rule the test is named for is then unexercised, and the record itself defers detection to VM-2's mutating corpus. |
| Prior finding | Negative control 4 - four assertions were one level too shallow; and EX-52's `Snapshot` row, which this test does not reach. |

## RA-5 - Guest-initiated loads and external suspension

### RC-27 - Decide whether the no-provider refusal is taken before every bound

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, `RequestLoad`; `docs/adr/0008-guest-initiated-loads.md`, "Classification At The Mediator Boundary", observation steps 1 to 7 and the paragraph on step 3; `docs/evidence/vm-1/README.md`, gate clause "deterministic refusal where no provider is registered" |
| Check | Whether the registration check precedes every bound and is taken before the request payload is inspected, so a composition that registers no provider gives one answer to every request - independent of what the guest asked for, its nesting depth, and how much budget it has spent |
| Falsified if | Any bound check, meter charge, or read of the request payload runs before `runtime.ProviderFor(profile)` is consulted, so a provider-less composition can return two different refusals for two different guest inputs |
| Prior finding | Negative control 4 in `docs/evidence/vm-1/negative-control.log` and `docs/evidence/vm-1/README.md`, "The negative controls, in detail": removing this refusal did not fail the suite on its first run |

### RC-28 - Decide whether the mediator observes cancellation at its ordered position

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, `RequestLoad` and `Answer`; `docs/adr/0008-guest-initiated-loads.md`, "Classification At The Mediator Boundary", step 2 and the sentence naming the only two facts that may precede step 3; `docs/adr/0005-operation-result-envelope.md`, "Precedence and observation order" |
| Check | Whether the requesting operation's cancellation latch is an observation point of its own at position 2, or is only reachable through the `OperationCanceledException` catch around the provider call - and, if the latter, whether the reviewer accepts that as satisfying the record |
| Falsified if | With the cancellation latch armed and no provider registered, the nested load reports `HostFailure` / `ProviderNotRegistered` rather than `Cancellation`, because no cancellation check runs before the registration check |

### RC-29 - Decide whether nested fan-out and byte counters are scoped to one operation

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, both `EnterScope` overloads and the comment on why the counters are not reset per step; the call sites in `src/Broiler.VM.Runtime/VmInstantiation.cs` and `src/Broiler.VM.Runtime/VmInstanceImplementation.cs`; `docs/adr/0008-guest-initiated-loads.md`, "Bounds, Defaulting, And Charging" (`NestedLoadFanOut` is total requests admitted by one invocation, `NestedLoadBytes` the sum per invocation); `src/tests/Broiler.VM.Contract.Tests/GuestInitiatedLoadTests.cs` |
| Check | Whether the counters reset when a new operation begins and persist across a resume of the same operation, which is the distinction the record and the mediator's own comment both rest on |
| Falsified if | Every call site uses the one-argument `EnterScope(baseline)`, which passes a default operation id, so `currentOperation` never changes, `fanOut` and `bytes` accumulate for the life of the profile state, and a second invocation on one instance begins with the first invocation's fan-out already spent - and no test invokes twice and asserts the reset |

### RC-30 - Decide whether a provider may be registered where no profile declares loads

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs`, `TryResolveGuestLoadBounds` (the `registersProvider` local) and `TryBindCapabilities` (the at-most-one guard and `DuplicateArtifactProvider`); `docs/adr/0008-guest-initiated-loads.md`, "The Artifact-Provider Capability" cardinality row and the paragraph ruling that a `NestedLoadDepth` marked `NotApplicable` forbids binding an artifact provider at all |
| Check | Whether that ruling is enforced at runtime creation, or is a paper rule a later milestone owns - and if the latter, whether the reviewer requires it to be named as an exclusion rather than left as unreached code |
| Falsified if | Runtime creation succeeds for a catalog in which no descriptor declares guest-initiated loads while the options register an `ArtifactProvider` capability, because `registersProvider` is computed and then discarded |

### RC-31 - Decide whether the double gate on external suspension is closed in both halves

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs`, `RequestSuspend`; `src/Broiler.VM.Abstractions/VmDescriptorValues.cs`, `VmDeclaration`; `src/Broiler.VM.Runtime/VmRuntimeCreationOptions.cs`, `VmExternalSuspensionMode`; `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension"; `docs/adr/0004-lifecycle-and-state-machine.md`, "Initiators And Authority" (a closed declaration gate is `Unsupported` and never `InvalidState`) |
| Check | Whether both halves refuse with `Unsupported` under distinct reasons that name distinct owners, and whether the descriptor field is mandatory and explicit as the record requires |
| Falsified if | Either half answers `InvalidState`, the two reasons collapse into one, or an omitted descriptor field is indistinguishable from a deliberate refusal because `VmDeclaration.NotDeclared` is the zero value and nothing forces the author to state it |

### RC-32 - Decide whether every party entitled to resume has a path to resume

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs`, `TryPark`, `TryTakeSuspension` and `VmOperationControlHandleImplementation`; `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle.TryTakeSuspension` and its remarks; `docs/adr/0009-external-suspension-and-async-instantiation.md`, "One Resumption Object, One Resume Path, And Who Holds It", including the four-way mapping of `TryTakeSuspension` onto the control result |
| Check | Whether `TryTakeSuspension` gives the party entitled to resume a path to resume in every origin case without reintroducing the second admission check it was designed to remove, and whether collapsing two of its four answers is acceptable at a frozen surface or must be corrected first |
| Falsified if | A `Guest`- or `Instantiation`-origin suspension queried through the handle answers `InvalidState` rather than `Unsupported`, and a pending suspend not yet observed answers `InvalidState` rather than `NoOp`, so a caller cannot tell "not yet" from "never" |
| Prior finding | EX-52 in `docs/evidence/vm-1/README.md`: `TryTakeSuspension` never answers `Unsupported` or `NoOp`. Named in `HUMAN_REVIEW.md` under the four invariant resolutions |

### RC-33 - Decide whether resume admission under a spent parent is checked before any guest work

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs`, `Resume` (the parent admission check, the single-use consume, and their order); `src/Broiler.VM.Runtime/VmAggregateBudget.cs`, `AdmitsResumption`; `docs/adr/0007-resource-authority-and-budgets.md`, "The shared aggregate budget is a core object", ADR 0007 guarantee G4 (not a gate clause); `src/tests/Broiler.VM.Contract.Tests/SuspensionAndBudgetTests.cs` |
| Check | Whether the parent is asked before any profile continuation runs, and whether a refusal by an exhausted parent and a refusal by a disposed one are reported as different categories rather than folded together |
| Falsified if | The continuation runs before `AdmitsResumption` is consulted, or a resume under a spent parent completes `Normal` |
| Prior finding | The VM-1 review blocker recorded in `docs/evidence/vm-1/README.md`, "Missing admission checks": no resume admission check existed at all, and an operation resumed normally under a parent with no remaining allowance |

## RA-6 - The public contract surface

### RC-34 - Accept or reverse erratum 1: `VmControlResult` is a struct, not an enum

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmControlResult.cs`, `VmControlOutcome` and `VmControlResult`, including the note on shape; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the `VmControlResult` row; `docs/adr/0004-lifecycle-and-state-machine.md`, "Initiators And Authority" (a control result carries exactly one reason code); `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension" (the two reasons must stay distinct); `docs/evidence/vm-1/README.md`, Deviations 1 |
| Check | Whether the reviewer accepts the struct over the frozen four-member kind and an erratum against the name table, or reverses it - the alternatives being to amend the name-table row, or to drop one of the two requirements a bare enum cannot carry |
| Falsified if | The frozen four members are not preserved by name in `VmControlOutcome`, or a control result can be constructed carrying more than one reason or none where a reason is required |
| Prior finding | EX-41 and the Deviations section of `docs/evidence/vm-1/README.md` |

### RC-35 - Accept or reverse erratum 2: stage results are built by hidden public factories

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmStageResults.cs`, the `EditorBrowsable(Never)` factories and the types that carry no factory for an illegal cell; `docs/adr/0005-operation-result-envelope.md`, "The result types"; rule A10 and rule V9 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`; `docs/evidence/vm-1/README.md`, Deviations 2 |
| Check | Whether a public factory hidden from IntelliSense is an acceptable substitute for the internal construction ADR 0005 specifies, given that rule A10 forbids `InternalsVisibleTo` and a profile package must be able to name the result types - or whether the reviewer requires a different resolution before the surface is frozen |
| Falsified if | A factory exists for a cell the stage matrix marks illegal, or a profile-facing result type cannot be named from a package that references only Abstractions and Binary |
| Prior finding | EX-41; `docs/evidence/vm-1/README.md`, Deviations 2 |

### RC-36 - Accept or reverse erratum 3: `VmOperation` is a frozen name that is not exported

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs` (internal); `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle` and `VmOperationStateSnapshot`; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the paragraph freezing ADR 0004's lifecycle objects outside the table; `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs`, the frozen-name list and its note on `VmOperation`; `docs/evidence/vm-1/README.md`, Deviations 3 |
| Check | Whether addressing the operation publicly through a control handle and a state snapshot discharges the frozen name, or whether the name table must be amended so that rule V1 ranges over the real set |
| Falsified if | The name is absent from V1's baseline with only a source comment recording why, so V1 cannot fail for the deviation it documents |
| Prior finding | EX-41 |

### RC-37 - Decide whether the exported surface matches the frozen public-name table

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `docs/adr/0003-core-contract-v1-and-amendments.md` section 9 (the terminology rules T1 to T6) and section 10 (the table, the members frozen outside their own record, and the struck names); `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs`; rules V1 and V3 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`; the exported types in `src/Broiler.VM.Abstractions/` and `src/Broiler.VM.Runtime/` |
| Check | Whether every frozen name is exported under the frozen kind, whether every exported type is declared in namespace `Broiler.VM`, and whether the names a single record freezes in its own text are held as firmly as the table's rows |
| Falsified if | A struck name reappears on the exported surface, a frozen name is exported under a different kind without an erratum, or a name frozen by a record's own text is absent from both the baseline list and the errata |

### RC-38 - Decide whether the stage matrix is a compile-time fact

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmOutcome.cs`, `VmStage` and `VmStageMatrix`; `src/Broiler.VM.Abstractions/VmStageResults.cs`; `docs/adr/0005-operation-result-envelope.md`, "The seven envelope-bearing stages", the matrix and the table of negative rules with their reasons |
| Check | Whether the seven rows and every negative rule in the record are reproduced exactly, including S7 being the suspending stage's row plus `InvalidState` minus `UnsupportedProfile`, and whether an illegal cell is unrepresentable rather than merely unasserted |
| Falsified if | A cell legal in `VmStageMatrix` is illegal in the record's table or the reverse, or a factory exists that can mint a result for a cell the matrix denies |

### RC-39 - Decide whether a profile package can name everything it needs, and nothing more

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `docs/adr/0011-source-level-profile-contract.md`, "The source-level profile contract" (promises P1 to P5) and "The closed set of capability transfer types"; `src/Broiler.VM.Abstractions/VmProfileContracts.cs`, `src/Broiler.VM.Abstractions/VmGuestLoad.cs`, `src/Broiler.VM.Abstractions/VmTransferTypes.cs`; the public types in `src/Broiler.VM.Runtime/`; rules A8 and A10 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether every profile-facing contract is declared in Abstractions so that no profile package ever references Broiler.VM.Runtime, and whether the closed transfer-type set is the only expressible signature shape |
| Falsified if | A profile-facing interface, delegate or result type is declared in Broiler.VM.Runtime, or a capability signature can name a type outside the closed set |
| Prior finding | EX-50: the application-local consumer profile does not exist, so P1 and P2 are read against two fixture profiles in one test-only assembly |

### RC-40 - Decide whether the frozen member sets survive on the exported surface

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle` and `VmSuspensionOrigin`; `docs/adr/0009-external-suspension-and-async-instantiation.md`, the `VmOperationControlHandle` member table, the paragraph on disposing a handle that holds an untaken suspension, and the Consequences bullet naming four drift assertions VM-1 gains; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the `VmOperationControlHandle` row; `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether `Dispose` is a fifth member the record's "exactly four" must be amended for, or a disposal path the four-member count was never meant to exclude - and whether the promised drift assertions on this member set and on `VmSuspensionOrigin` exist as register rows yet |
| Falsified if | The exported handle carries a member outside `RequestSuspend`, `RequestCancel`, `QueryState` and `TryTakeSuspension` while no record row and no register rule accounts for it |
| Prior finding | EX-21, closed by the milestone at which each named surface exists; `docs/adr/0004-lifecycle-and-state-machine.md` Consequences records the same drift assertions as becoming writable at VM-1 |

## RA-7 - The records themselves

### RC-41 - Decide what happens to the seventeen proposed and unapplied roadmap amendments

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0003-core-contract-v1-and-amendments.md` section 11, all eighteen rows, the quoting convention, and the paragraph stating that no row touches an invariant, a milestone gate, the delivery order, or sections 14 to 16 beyond four named cells; `docs/roadmap.md`; EX-11 in `docs/evidence/vm-0/README.md` and `docs/evidence/vm-1/README.md` |
| Check | Whether the divergence may stand while VM-1 implements the records and the roadmap says something else, or whether the patch must land before either milestone is accepted; and whether each proposed new text narrows the sentence it replaces rather than widening it |
| Falsified if | A row's old text does not match `docs/roadmap.md` character for character under the record's own quoting convention, a row marked `Proposed` has in fact been applied, or a row changes an invariant or a gate while claiming not to |
| Prior finding | EX-11, carried in both bundles and in `docs/roadmap.status.md` as an open VM-0 gate condition |

### RC-42 - Decide whether ADR 0007's monotonicity rules resolve invariant 9's arithmetic

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0007-resource-authority-and-budgets.md`, Context (invariant 9 quoted in full, and the observation that it fixes directions and not an order), "The precedence algorithm", and "Monotonicity, and what raising a ceiling costs" (M1, M2, tighten-only for both classes, and the artifact-versus-host asymmetry); `src/Broiler.VM.Runtime/VmCeilingResolution.cs` and `src/Broiler.VM.Runtime/VmMeter.cs` |
| Check | Whether M1 and M2 plus the clamp-or-refuse asymmetry are a faithful reading of the invariant rather than a new policy, and whether `BudgetRaiseRefused` staying outside `ResourceExhaustion` is right |
| Falsified if | Two implementers can still derive two effective policies from the record's own text, a core operation increases a live meter, or a refused host override is reported as exhaustion |

### RC-43 - Decide whether the aggregate budget resolves invariant 9's composition clause

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0007-resource-authority-and-budgets.md`, "The shared aggregate budget is a core object", ADR 0007 guarantees G1 to G4 (not gate clauses), the two recorded narrowings, and the paragraph on exhaustion not killing siblings; `docs/adr/0009-external-suspension-and-async-instantiation.md` Consequences, the bullet on the roadmap sentence naming a transition that does not exist; `src/Broiler.VM.Runtime/VmAggregateBudget.cs` |
| Check | Whether G4's reading - no runtime created and no operation resumed - is the right resolution of a roadmap sentence that speaks of resuming runtimes, and whether the reviewer accepts the two narrowings, in particular that the victim operation of a shared-parent exhaustion is deterministic in category, dimension and scope but not in identity |
| Falsified if | The record's four guarantees are not jointly deliverable by a pay-as-you-go counter, or the resolution requires a scheduler property the record elsewhere forbids |
| Prior finding | The VM-1 blocker recorded in `docs/evidence/vm-1/README.md`, "Asymmetric accounting": the aggregate live sum could be driven to zero while memory was still live |

### RC-44 - Decide whether parking by unwinding resolves invariant 12

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0009-external-suspension-and-async-instantiation.md`, "Parking By Unwinding", ADR 0009 guarantees G1 to G3 (not gate clauses), the four things it forbids at contract version 1, and the Rejected Alternatives rows on suspending in place and on a core watchdog; `src/Broiler.VM.Runtime/VmOperation.cs`, `TryPark`, `Abandon` and `Expire`; `src/Broiler.VM.Runtime/VmRuntime.cs`, `PollDeadlines` |
| Check | Whether "external control is a lifecycle state, not a side channel" is genuinely discharged by parking as heap data behind a single-use object, and whether disposal is provably free of any wait on a resume, a host reply or an event loop |
| Falsified if | Any path holds a thread for the duration of a pause, disposal can block on a resume that never comes, or the core resumes, cancels or terminates a suspended operation on a timer of its own |

### RC-45 - Decide whether the suspension gates resolve invariant 8 without a shape-only stub

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension", "Asynchronous Instantiation" (the undeclared path and its `invalid state` classification), the authority-is-possession table, and Consequences on ADR 0003 carrying two rows; `docs/adr/0003-core-contract-v1-and-amendments.md` section 8, the admitted-versus-implemented table and the two discharge forms |
| Check | Whether a declaration gate answering `Unsupported` with a naming reason is discharge form (a) rather than a type that exists and refuses everything, and whether "the core authenticates nobody" is an acceptable resolution of who may pause an operation |
| Falsified if | An admitted artefact discharges the invariant by neither form, or a gate returns a truthful-looking refusal from a path no composition can reach |

### RC-46 - Decide each of the twelve records, which are all still `Proposed`

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/README.md`, the index table, the errata paragraph, and the statement that all twelve are `Proposed` and not `Approved`; the `**Status:**` and `**Core contract:**` header fields of `docs/adr/0001-component-topology-and-dependency-graph.md` through `docs/adr/0012-security-ownership-and-support-matrix.md`; rules E1 to E4 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether each record is accepted, accepted with a condition, or rejected on its own - the index cannot be accepted as a block, and the contract-bearing ten cannot change afterwards without the amendment procedure |
| Falsified if | The index lists a record that does not exist or omits one that does, a contract-bearing record declares a version other than the one `VmCoreContract` carries, or a record's status is changed anywhere without a signature in `HUMAN_REVIEW.md` |
| Prior finding | EX-30: all six ownership roles are held by one person, so owner and reviewer confirmation is not independent |

### RC-47 - Decide the one seam ADR 0011 leaves open against ADR 0005

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0011-source-level-profile-contract.md`, "Exception translation across the capability boundary", rules X1 to X3 and the paragraph headed "One seam to reconcile"; `docs/adr/0005-operation-result-envelope.md`, "Return values, not exceptions" and "Precedence and observation order"; `src/Broiler.VM.Runtime/VmCapabilityBinding.cs` |
| Check | Whether X2 - a core meter already exhausted at the catch boundary is reported as `ResourceExhaustion` rather than as a host defect - is a narrowing of ADR 0005's catch-all as the record claims, and whether the one-line edit the record says the core-contract owner must make before either record is accepted has been made |
| Falsified if | The implementation follows one record and not the other, or the edit is still outstanding while both records are presented for acceptance |

## RA-8 - The evidence and the rule register

### RC-48 - Decide whether the figures in the bundles match the retained logs

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/evidence/vm-1/README.md`, Outputs; `docs/evidence/vm-1/build.log`, `docs/evidence/vm-1/test.log`, `docs/evidence/vm-1/pack.log`, `docs/evidence/vm-1/publish-jit-and-trimmed.log`, `docs/evidence/vm-1/publish-aot.log`, `docs/evidence/vm-1/negative-control.log`, `docs/evidence/vm-1/d1-outcome.txt`, `docs/evidence/vm-1/hashes.txt`; `docs/evidence/vm-0/README.md` and the logs beside it; `HUMAN_REVIEW.md`, "Evidence Available To The Reviewer"; `docs/roadmap.status.md` section 2 |
| Check | Whether every figure quoted in a bundle, in the ledger and in the review file is the figure its log records: 7 projects Release with 0 warnings and 0 errors; 220 tests passing, 89 architecture and 131 behavioural; exactly 3 `.nupkg` and 3 `.snupkg`; a Native AOT binary of 1,279,488 bytes, recorded in the collecting machine's locale as `1.279.488`; a trimmed self-contained binary of 162,816 bytes; 5 checks passed and exit code 0 in each of the three modes; 4 negative controls |
| Falsified if | A document states a figure the log does not, in particular an AOT size other than 1,279,488 bytes or a suite total other than 220, or the SHA-256 list in `docs/evidence/vm-1/hashes.txt` does not recompute against the checkout the bundle claims to describe |
| Prior finding | Ledger update rule 4 forbids promoting a result beyond what it proves; `docs/evidence/vm-1/README.md` records that VM-1-001's figures were all true while sixteen blockers were present |

### RC-49 - Decide whether rule B3 is honestly `Vacuous`

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, the `$comment` block stating the one criterion and the three status values, and the B3 row with its `nonVacuousWhen` and its `witness` field; EX-40 in `docs/evidence/vm-1/README.md`; `HUMAN_REVIEW.md`, the attention item on one rule asserting nothing |
| Check | Whether a violation of B3 is genuinely unreachable by construction - A1 forbids the outbound project reference, A2 the package-shaped one, and the single-source `NuGet.config` makes a foreign `Broiler.*` package unresolvable - so that `Vacuous` with an activation milestone of VM-3 is the honest status rather than a rule quietly retired |
| Falsified if | A path exists by which a product assembly could name a foreign `Broiler.*` assembly in this checkout, which would make B3 `Active` and unwitnessed; or the register's own criterion that every rule carries a witness is not met by a row whose witness field is `none` or null |

### RC-50 - Decide whether the negative controls falsify anything

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/evidence/vm-1/negative-control.log`, all four controls with both runs; `docs/evidence/vm-1/README.md`, "The negative controls, in detail" and the paragraph on control 4; `docs/evidence/vm-0/negative-control.log` and the two discarded controls recorded under Procedure in `docs/evidence/vm-0/README.md` |
| Check | Whether each injected violation is the violation the rule is about rather than an adjacent one, whether the failure counts in each log are consistent with the suite totals, and whether the strengthening of four guest-load assertions after control 4 is enough - a negative control that passes is a finding about the suite, and the same question is owed to the assertions no control touched |
| Falsified if | A control fails for a reason other than the rule it targets, a revert run is absent from a log, or a control's injected edit does not correspond to the code path the bundle says it removes |
| Prior finding | Control 4 did not fail on its first run; four assertions were one level too shallow, testing the fixture's reaction rather than the core's reason |

### RC-51 - Decide whether each register row's witness falsifies that row's statement

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, every row's `statement`, `status` and `witness`; `src/tests/Broiler.VM.Architecture.Tests/witnesses/`; `src/tests/Broiler.VM.Architecture.Tests/RuleRegisterTests.cs`, which the register says enforces it against the test methods and witness inputs in both directions; the review note in the register's `$comment` on V4 and V9 |
| Check | Whether a witness exists that the rule actually rejects, and whether a shared witness input - A7 and A8 name the same `.csproj.witness` file, B1 and B2 the same test assembly - is enough to witness both statements |
| Falsified if | A row's statement is broader than what its rule executes, as V4 and V9 were before the review pass, or a witness is accepted by the rule it is filed under |
| Prior finding | Two register rows were among the surviving review findings: V4 checked a property count rather than the frozen rows, and V9 asserted return types rather than the construction site. EX-51 records what V9 still does not do |

### RC-52 - Decide whether the two bundles cover the eight required fields truthfully

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/roadmap.status.md` section 3, the required-field list, and section 4, update rules 4, 5 and 7; `docs/evidence/vm-0/README.md` and `docs/evidence/vm-1/README.md` end to end, including VM-1-002's superseding banner and VM-0's re-collection note; `docs/evidence/vm-0/hashes.txt` against the current `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether each bundle fills every applicable field, whether the superseded VM-1-001 result is retained rather than deleted, and how a reviewer should read VM-0's Outputs row for a register file that has since been rewritten - VM-0 pins its hash, and the file in the checkout is the VM-1 register with different counts |
| Falsified if | A field is empty without an exclusion naming why, a bundle claims a state the retained logs do not show, or a superseded result has been removed rather than kept as history |
| Prior finding | EX-01, EX-03, EX-42, EX-45: the D1 branch is environment-conditional, no SDK pin exists, the AOT publish is not reproducible by automation, and everything is one RID on one machine with no CI |

### RC-53 - Decide whether the claim each bundle justifies is as narrow as its evidence

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/evidence/vm-1/README.md`, Decision - the sixteen gate clauses one by one, the two that are not fully discharged, and the narrowly stated claim; the exclusion table, in particular EX-52's list of twenty-nine unaddressed findings; `docs/roadmap.status.md` section 2, the VM-1 row's open gate conditions |
| Check | Whether the bundle's claim stops where its evidence stops - fifteen of sixteen clauses demonstrated, the last one not met because no reviewer has read the contract - and whether the twenty-nine unaddressed findings are the next piece of work rather than noise a reader may skip |
| Falsified if | A clause marked shown rests on a test that asserts something weaker than the clause, or the ledger, the review file or the bundle describes VM-1 in terms an unaccepted milestone cannot carry |
| Prior finding | EX-52; and the gate's last clause, "the accepted contract is recorded with its version", recorded as not met |

## What to do with a finding

Three places, and all three are needed for the record to be complete.

1. **Record the item verdict here.** Edit the `Verdict` cell of the RC-nn row you have
   just read, using the review vocabulary from
   [section 1](../../HUMAN_REVIEW.md#1-how-to-use-this-file). Where the mark is `[R]` or
   `[?]`, name the defect or the missing thing on the row itself: `[?]` means the item
   could not be judged from what is here, which is a different answer from `[R]`.
2. **Record the area verdict in
   [HUMAN_REVIEW.md section 8](../../HUMAN_REVIEW.md#8-area-verdicts).** That table is
   the record. An area left at `[ ]` there means the area was not reviewed; it does not
   mean the area was accepted.
3. **Put any condition in
   [HUMAN_REVIEW.md section 10](../../HUMAN_REVIEW.md#10-conditions).** Every area marked
   `[C]` in section 8 records its condition there. A condition that exists only as a note
   on a worksheet row has not been recorded.

The decision in [section 9](../../HUMAN_REVIEW.md#9-decision) and the signature in
[section 11](../../HUMAN_REVIEW.md#11-human-attestation) are what make the review
binding. Nothing in this worksheet is a decision.
