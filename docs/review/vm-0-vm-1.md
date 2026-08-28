# Reviewer worksheet: VM-0 and VM-1

This is a worksheet, not a record. It is where a reviewer reads item by item and
notes what they found. The review itself - the area verdicts, the decision, and any
conditions - is recorded in [HUMAN_REVIEW.md](../../HUMAN_REVIEW.md), not here.

**The code-facing criteria have moved into the source.** Twenty-two items here were about a
code unit and nothing else, and what each of them was really worth was its *Falsified if*
row - the observation that would show the unit wrong. That row now sits on the declaration
itself, as the third line of the assurance annotation, between the AI assessment and the
human line:

```csharp
// Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=630EF7
// Broiler-Falsified-If: new T[] is reached before TryReserve returns true, or a failed allocation keeps its reservation
// Broiler-Human:        PENDING
```

They moved because an annotation sits on the declaration and travels with it, and a line
number in a separate document does not. This document's had already rotted: its first item
cited *"the checked multiplication at `VmBoundedAllocator.cs:57`"*, which the annotations
themselves turned into a parameter declaration. Nothing caught it, because rule H3 checks
identifiers, areas and counts and never whether a cited line means anything. A criterion is
required on every unit assessed `Security=High` or `Critical`, permitted below that, and
counted in [CODE-ASSURANCE.md](../../CODE-ASSURANCE.md); rule J10 names every unit that owes
one and carries none. The line is a comment, so it is outside every fingerprint: rewording a
criterion invalidates no review, which is correct, because a criterion is an instruction to
the reviewer and not part of what the reviewer certifies.

What is left here is what the assurance system scans no source for: the records, the errata,
the invariant resolutions, the bundles, the register and the negative controls. Each of the
four code areas keeps one item, and it asks for the thing no rule can do - a judgement of
the criteria themselves, and of whether any is already unmet. That is the bridge between
this route and the source, and `EX-75` records why it is needed.

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
| [RA-1](#ra-1---bounded-reading-of-untrusted-bytes) | Bounded reading of untrusted bytes | 1 | |
| [RA-2](#ra-2---resource-authority-and-budgets) | Resource authority and budgets | 2 | |
| [RA-3](#ra-3---lifecycle-and-state-machine) | Lifecycle and state machine | 2 | |
| [RA-4](#ra-4---verified-artifact-ownership) | Verified-artifact ownership | 3 | |
| [RA-5](#ra-5---guest-initiated-loads-and-external-suspension) | Guest-initiated loads and external suspension | 7 | |
| [RA-6](#ra-6---the-public-contract-surface) | The public contract surface | 7 | |
| [RA-7](#ra-7---the-records-themselves) | The records themselves | 7 | |
| [RA-8](#ra-8---the-evidence-and-the-rule-register) | The evidence and the rule register | 6 | |
| | **Total** | **35** | |

## RA-1 - Bounded reading of untrusted bytes

### RC-01 - Judge the criteria the bounded-reading units now carry

| | |
|---|---|
| Area | RA-1 |
| Verdict | [ ] |
| Read | Every `// Broiler-Falsified-If:` line in `src/Broiler.VM.Binary/VmBoundedReader.cs`, `src/Broiler.VM.Binary/VmBoundedAllocator.cs` and `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - twenty-five of them - each read against the declaration it sits on and against the `Security` value on the line above it, and the four annotated units in those files that carry none; `CODE-ASSURANCE.md`, *Falsification criteria*; ADR 0007 section *The precedence algorithm*, steps P2 and P5; ADR 0006 section *5. The verification failure taxonomy* |
| Check | Decide whether each line names an observation somebody could go and make about that unit - the bound compared before the value it bounds, the reservation before the allocation, one encoding per value, the first cause retained - rather than a concern, a summary of the method, or a restatement of `Security=High`; and whether the four units here that carry none should. |
| Falsified if | A criterion in these files names no observation that can be made about the unit it sits on, or restates the assessment above it; or the code beneath a criterion already fails it, so the line records a defect rather than guarding against one; or the ordering these criteria name is not the ordering the reader and the allocator actually implement. |
| Prior finding | `EX-75` in `docs/evidence/vm-1/README.md`: rules J2 and J10 hold a criterion to being present, single-line and prose, and neither can read it. This item is the reading. |

## RA-2 - Resource authority and budgets

### RC-02 - Judge the criteria the budget units carry, and the units that carry none

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | The `// Broiler-Falsified-If:` lines in `src/Broiler.VM.Runtime/VmMeter.cs` - on the class, `TryCharge`, `ReportRetained`, `ReportReleased`, `PauseWallClock`, `AccrueWallClock` and `RemainingSnapshot` - and the line on `Resume` in `src/Broiler.VM.Runtime/VmRuntime.cs`; then `src/Broiler.VM.Runtime/VmAggregateBudget.cs`, `VmBudgetLevel.cs` and `VmCeilingResolution.cs`, none of whose units carries one; ADR 0007 sections *The precedence algorithm*, *Monotonicity, and what raising a ceiling costs* and *The shared aggregate budget is a core object* |
| Check | Decide whether the criteria on the meter name the observations that would show a budget failing to bound - a level committing while another refuses, a credit larger than the debit, a delta dropped rather than re-offered - and whether the aggregate budget and the ceiling resolution should carry criteria of their own, given that nothing in them is assessed above `Security=Medium` and so nothing requires one. |
| Falsified if | A criterion here names no observation that can be made about the unit it sits on, or restates the assessment above it; or the code beneath one already fails it; or the asymmetry that drove the aggregate live sum below the true sum would satisfy every criterion in this area, so none of them would have caught the blocker they were written from. |
| Prior finding | Asymmetric accounting - a retention the parent refused was still released from the parent. The criteria on `ReportRetained` and `ReportReleased` are that finding reduced to two observations; `EX-75` records that nothing judges whether the reduction is faithful. |

### RC-03 - Decide whether the budget regressions assert bounds tight enough to fail

| | |
|---|---|
| Area | RA-2 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/ReviewRegressionTests.cs` (`afterRelease >= 900`), 396-442 (`RequestCount <= 8`) and 522-545 (`Assert.NotEqual(VmOutcome.Normal, ...)`); `src/tests/Broiler.VM.Contract.Tests/ReclamationTests.cs` (`after < held`); `docs/evidence/vm-1/README.md`, section *The negative controls, in detail* |
| Check | Decide, for each of these four assertions, whether it excludes the wrong behaviour it is named for or merely a subset of it. |
| Falsified if | An implementation that never calls the provider at all still satisfies `RequestCount <= 8`; or one that reclaims a single byte still satisfies `after < held`; or one that over-charges the parent still satisfies `afterRelease >= 900`; or one that fails a provider call for a reason unrelated to `HostCalls` still satisfies `Assert.NotEqual(VmOutcome.Normal, ...)`. |
| Prior finding | Negative control 4 - the tests asserted the profile's reaction rather than the core's reason, and four assertions were one level too shallow. |

## RA-3 - Lifecycle and state machine

### RC-04 - Judge the criteria the lifecycle units carry

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | The `// Broiler-Falsified-If:` lines on `State`, `Settle` and `TryAdmit` in `src/Broiler.VM.Runtime/VmInstanceImplementation.cs`; on `Instantiate` and `PlaceholderState` in `src/Broiler.VM.Runtime/VmInstantiation.cs`; and on `Verify`, `Dispose`, `TryBeginCall` and `OperationKey` in `src/Broiler.VM.Runtime/VmRuntime.cs`; ADR 0004 section *The Instance And The Outcome Mapping*, the table headed "Outcome to instance state. Mandatory; no implementation freedom.", and section *Reentrancy*; ADR 0005 section *Precedence and observation order* |
| Check | Decide whether these lines reproduce the mandatory mapping and the frozen precedence order as things a reader can check at the declaration, and whether the units that can break those rules are the units carrying the criteria - `Settle` holds the mapping, but the stages that reach it are in three other files. |
| Falsified if | A criterion here names no observation that can be made about the unit it sits on, or restates the assessment above it; or the code beneath one already fails it; or `Settle`'s criterion is satisfied by a mapping the record's eight mandatory rows forbid, because it names four outcomes by class and the table names eight by row. |
| Prior finding | Mandatory mappings not implemented; inverted precedence; and `EX-75`, which records that no rule can tell a criterion that covers its unit from one that covers half of it. |

### RC-05 - Decide whether the lifecycle regressions exercise more than the invoke stage

| | |
|---|---|
| Area | RA-3 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/ReviewRegressionTests.cs` (the mapping cases) and 122-155 (`A_Terminate_Operation_Capability_Ends_The_Operation_As_A_Host_Failure`); `src/tests/Broiler.VM.Contract.Tests/LifecycleTests.cs` ; ADR 0004 section *The Instance And The Outcome Mapping*; ADR 0005 section *The seven envelope-bearing stages* |
| Check | Decide whether the mapping and the precedence order are tested at each stage that can produce them, or only where the fixture happens to make them easy to produce. |
| Falsified if | No test drives a terminating host failure, a poll-bound breach or an unconverted capability fault through the instantiation stage or through `Resume`, so a stage that omits those precedence steps entirely still passes every behavioural test. |
| Prior finding | Negative control 4 - a green suite is evidence about the contract only to the extent that the tests were written to catch the contract being broken. |

## RA-4 - Verified-artifact ownership

### RC-06 - Judge the criteria the verified-artifact units carry

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | The `// Broiler-Falsified-If:` lines on `Create`, `TryAcquireLease`, `Dispose` and `ReleaseLease` in `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs`; on the partial class, `VerifyCore` and `RunVerifier` in `src/Broiler.VM.Runtime/VmVerification.cs`; on the `VmRuntime` declaration in `src/Broiler.VM.Runtime/VmRuntime.cs`, which carries the one-entry-point criterion; and on `IVmVerifiedState`, `IVmVerificationContext`, `IVmProfileVerifier.Verify` and `VmExecutorFactory` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs`; ADR 0006 sections *1. The input boundary and the representation choice*, *2. What a successful verification binds*, *3. States, lifetime kinds, and the lease contract* and *6. Requirement V-SEP*, the property table |
| Check | Decide whether the six V-SEP properties and the ownership rules are each named by a criterion on a unit that could break them. No single unit holds V-SEP, so the question is whether the split across these files leaves a property named nowhere. |
| Falsified if | A criterion here names no observation that can be made about the unit it sits on, or restates the assessment above it; or the code beneath one already fails it; or a property in the V-SEP table is named by no criterion on any unit, so it rests on the prose of ADR 0006 alone. |
| Prior finding | `EX-51`, which records what rule V9 does not assert about the single construction site, and `EX-75`. |

### RC-07 - Confirm the single construction site is asserted at the granularity claimed

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` (the hidden factory and its remark); `src/Broiler.VM.Runtime/VmVerification.cs` (the one call); `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs` (rule V9); ADR 0006 sections *2. What a successful verification binds* and *6. Requirement V-SEP*; `docs/evidence/vm-1/README.md` exclusion EX-51 |
| Check | Decide whether V9 asserts what the one-construction-site rule claims, now that the exclusion states plainly that it does not count call sites. |
| Falsified if | V9's producer scan is restricted to public static declared-only methods, so a public *instance* member returning a `VmVerifiedArtifact` is not counted; or its caller check is per-assembly, so a second call to `VmVerifiedArtifact.Create` added anywhere inside `Broiler.VM.Runtime` leaves the rule green. |
| Prior finding | Rules weaker than their statements - V9 asserted return types rather than the construction site. |

### RC-08 - Decide whether the caller-buffer test proves anything about the core

| | |
|---|---|
| Area | RA-4 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` (`Mutating_The_Callers_Buffer_After_Verification_Changes_Nothing`); `src/tests/Broiler.VM.Fixtures/FixtureVmProfile.cs` ; `src/tests/Broiler.VM.Contract.Tests/ContractSurfaceTests.cs` and `src/tests/Broiler.VM.Contract.Tests/CatalogRegistrationTests.cs` ; ADR 0006 section *1. The input boundary and the representation choice*, the bullets *The core makes the copy* and *Truthfulness* |
| Check | Decide whether clearing the caller's array and re-reading the result discriminates between a core that owns the bytes and a fixture verifier that happened to decode eagerly. |
| Falsified if | Every descriptor in the test tree declares `VmArtifactRepresentationKind.Decoded`, so the test exercises the branch in which the record requires no copy at all and no test declares `Snapshot`; the ownership rule the test is named for is then unexercised, and the record itself defers detection to VM-2's mutating corpus. |
| Prior finding | Negative control 4 - four assertions were one level too shallow; and EX-52's `Snapshot` row, which this test does not reach. |

## RA-5 - Guest-initiated loads and external suspension

### RC-09 - Decide whether the no-provider refusal is taken before every bound

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, `RequestLoad`; `docs/adr/0008-guest-initiated-loads.md`, "Classification At The Mediator Boundary", observation steps 1 to 7 and the paragraph on step 3; `docs/evidence/vm-1/README.md`, gate clause "deterministic refusal where no provider is registered" |
| Check | Whether the registration check precedes every bound and is taken before the request payload is inspected, so a composition that registers no provider gives one answer to every request - independent of what the guest asked for, its nesting depth, and how much budget it has spent |
| Falsified if | Any bound check, meter charge, or read of the request payload runs before `runtime.ProviderFor(profile)` is consulted, so a provider-less composition can return two different refusals for two different guest inputs |
| Prior finding | Negative control 4 in `docs/evidence/vm-1/negative-control.log` and `docs/evidence/vm-1/README.md`, "The negative controls, in detail": removing this refusal did not fail the suite on its first run |

### RC-10 - Decide whether the mediator observes cancellation at its ordered position

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, `RequestLoad` and `Answer`; `docs/adr/0008-guest-initiated-loads.md`, "Classification At The Mediator Boundary", step 2 and the sentence naming the only two facts that may precede step 3; `docs/adr/0005-operation-result-envelope.md`, "Precedence and observation order" |
| Check | Whether the requesting operation's cancellation latch is an observation point of its own at position 2, or is only reachable through the `OperationCanceledException` catch around the provider call - and, if the latter, whether the reviewer accepts that as satisfying the record |
| Falsified if | With the cancellation latch armed and no provider registered, the nested load reports `HostFailure` / `ProviderNotRegistered` rather than `Cancellation`, because no cancellation check runs before the registration check |

### RC-11 - Decide whether nested fan-out and byte counters are scoped to one operation

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs`, both `EnterScope` overloads and the comment on why the counters are not reset per step; the call sites in `src/Broiler.VM.Runtime/VmInstantiation.cs` and `src/Broiler.VM.Runtime/VmInstanceImplementation.cs`; `docs/adr/0008-guest-initiated-loads.md`, "Bounds, Defaulting, And Charging" (`NestedLoadFanOut` is total requests admitted by one invocation, `NestedLoadBytes` the sum per invocation); `src/tests/Broiler.VM.Contract.Tests/GuestInitiatedLoadTests.cs` |
| Check | Whether the counters reset when a new operation begins and persist across a resume of the same operation, which is the distinction the record and the mediator's own comment both rest on |
| Falsified if | Every call site uses the one-argument `EnterScope(baseline)`, which passes a default operation id, so `currentOperation` never changes, `fanOut` and `bytes` accumulate for the life of the profile state, and a second invocation on one instance begins with the first invocation's fan-out already spent - and no test invokes twice and asserts the reset |

### RC-12 - Decide whether a provider may be registered where no profile declares loads

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs`, `TryResolveGuestLoadBounds` (the `registersProvider` local) and `TryBindCapabilities` (the at-most-one guard and `DuplicateArtifactProvider`); `docs/adr/0008-guest-initiated-loads.md`, "The Artifact-Provider Capability" cardinality row and the paragraph ruling that a `NestedLoadDepth` marked `NotApplicable` forbids binding an artifact provider at all |
| Check | Whether that ruling is enforced at runtime creation, or is a paper rule a later milestone owns - and if the latter, whether the reviewer requires it to be named as an exclusion rather than left as unreached code |
| Falsified if | Runtime creation succeeds for a catalog in which no descriptor declares guest-initiated loads while the options register an `ArtifactProvider` capability, because `registersProvider` is computed and then discarded |

### RC-13 - Decide whether the double gate on external suspension is closed in both halves

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs`, `RequestSuspend`; `src/Broiler.VM.Abstractions/VmDescriptorValues.cs`, `VmDeclaration`; `src/Broiler.VM.Runtime/VmRuntimeCreationOptions.cs`, `VmExternalSuspensionMode`; `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension"; `docs/adr/0004-lifecycle-and-state-machine.md`, "Initiators And Authority" (a closed declaration gate is `Unsupported` and never `InvalidState`) |
| Check | Whether both halves refuse with `Unsupported` under distinct reasons that name distinct owners, and whether the descriptor field is mandatory and explicit as the record requires |
| Falsified if | Either half answers `InvalidState`, the two reasons collapse into one, or an omitted descriptor field is indistinguishable from a deliberate refusal because `VmDeclaration.NotDeclared` is the zero value and nothing forces the author to state it |

### RC-14 - Decide whether every party entitled to resume has a path to resume

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs`, `TryPark`, `TryTakeSuspension` and `VmOperationControlHandleImplementation`; `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle.TryTakeSuspension` and its remarks; `docs/adr/0009-external-suspension-and-async-instantiation.md`, "One Resumption Object, One Resume Path, And Who Holds It", including the four-way mapping of `TryTakeSuspension` onto the control result |
| Check | Whether `TryTakeSuspension` gives the party entitled to resume a path to resume in every origin case without reintroducing the second admission check it was designed to remove, and whether collapsing two of its four answers is acceptable at a frozen surface or must be corrected first |
| Falsified if | A `Guest`- or `Instantiation`-origin suspension queried through the handle answers `InvalidState` rather than `Unsupported`, and a pending suspend not yet observed answers `InvalidState` rather than `NoOp`, so a caller cannot tell "not yet" from "never" |
| Prior finding | EX-52 in `docs/evidence/vm-1/README.md`: `TryTakeSuspension` never answers `Unsupported` or `NoOp`. Named in `HUMAN_REVIEW.md` under the four invariant resolutions |

### RC-15 - Decide whether resume admission under a spent parent is checked before any guest work

| | |
|---|---|
| Area | RA-5 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmRuntime.cs`, `Resume` (the parent admission check, the single-use consume, and their order); `src/Broiler.VM.Runtime/VmAggregateBudget.cs`, `AdmitsResumption`; `docs/adr/0007-resource-authority-and-budgets.md`, "The shared aggregate budget is a core object", ADR 0007 guarantee G4 (not a gate clause); `src/tests/Broiler.VM.Contract.Tests/SuspensionAndBudgetTests.cs` |
| Check | Whether the parent is asked before any profile continuation runs, and whether a refusal by an exhausted parent and a refusal by a disposed one are reported as different categories rather than folded together |
| Falsified if | The continuation runs before `AdmitsResumption` is consulted, or a resume under a spent parent completes `Normal` |
| Prior finding | The VM-1 review blocker recorded in `docs/evidence/vm-1/README.md`, "Missing admission checks": no resume admission check existed at all, and an operation resumed normally under a parent with no remaining allowance |

## RA-6 - The public contract surface

### RC-16 - Accept or reverse erratum 1: `VmControlResult` is a struct, not an enum

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmControlResult.cs`, `VmControlOutcome` and `VmControlResult`, including the note on shape; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the `VmControlResult` row; `docs/adr/0004-lifecycle-and-state-machine.md`, "Initiators And Authority" (a control result carries exactly one reason code); `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension" (the two reasons must stay distinct); `docs/evidence/vm-1/README.md`, Deviations 1 |
| Check | Whether the reviewer accepts the struct over the frozen four-member kind and an erratum against the name table, or reverses it - the alternatives being to amend the name-table row, or to drop one of the two requirements a bare enum cannot carry |
| Falsified if | The frozen four members are not preserved by name in `VmControlOutcome`, or a control result can be constructed carrying more than one reason or none where a reason is required |
| Prior finding | EX-41 and the Deviations section of `docs/evidence/vm-1/README.md` |

### RC-17 - Accept or reverse erratum 2: stage results are built by hidden public factories

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmStageResults.cs`, the `EditorBrowsable(Never)` factories and the types that carry no factory for an illegal cell; `docs/adr/0005-operation-result-envelope.md`, "The result types"; rule A10 and rule V9 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`; `docs/evidence/vm-1/README.md`, Deviations 2 |
| Check | Whether a public factory hidden from IntelliSense is an acceptable substitute for the internal construction ADR 0005 specifies, given that rule A10 forbids `InternalsVisibleTo` and a profile package must be able to name the result types - or whether the reviewer requires a different resolution before the surface is frozen |
| Falsified if | A factory exists for a cell the stage matrix marks illegal, or a profile-facing result type cannot be named from a package that references only Abstractions and Binary |
| Prior finding | EX-41; `docs/evidence/vm-1/README.md`, Deviations 2 |

### RC-18 - Accept or reverse erratum 3: `VmOperation` is a frozen name that is not exported

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Runtime/VmOperation.cs` (internal); `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle` and `VmOperationStateSnapshot`; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the paragraph freezing ADR 0004's lifecycle objects outside the table; `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs`, the frozen-name list and its note on `VmOperation`; `docs/evidence/vm-1/README.md`, Deviations 3 |
| Check | Whether addressing the operation publicly through a control handle and a state snapshot discharges the frozen name, or whether the name table must be amended so that rule V1 ranges over the real set |
| Falsified if | The name is absent from V1's baseline with only a source comment recording why, so V1 cannot fail for the deviation it documents |
| Prior finding | EX-41 |

### RC-19 - Decide whether the exported surface matches the frozen public-name table

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `docs/adr/0003-core-contract-v1-and-amendments.md` section 9 (the terminology rules T1 to T6) and section 10 (the table, the members frozen outside their own record, and the struck names); `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs`; rules V1 and V3 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`; the exported types in `src/Broiler.VM.Abstractions/` and `src/Broiler.VM.Runtime/` |
| Check | Whether every frozen name is exported under the frozen kind, whether every exported type is declared in namespace `Broiler.VM`, and whether the names a single record freezes in its own text are held as firmly as the table's rows |
| Falsified if | A struck name reappears on the exported surface, a frozen name is exported under a different kind without an erratum, or a name frozen by a record's own text is absent from both the baseline list and the errata |

### RC-20 - Decide whether the stage matrix is a compile-time fact

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmOutcome.cs`, `VmStage` and `VmStageMatrix`; `src/Broiler.VM.Abstractions/VmStageResults.cs`; `docs/adr/0005-operation-result-envelope.md`, "The seven envelope-bearing stages", the matrix and the table of negative rules with their reasons |
| Check | Whether the seven rows and every negative rule in the record are reproduced exactly, including S7 being the suspending stage's row plus `InvalidState` minus `UnsupportedProfile`, and whether an illegal cell is unrepresentable rather than merely unasserted |
| Falsified if | A cell legal in `VmStageMatrix` is illegal in the record's table or the reverse, or a factory exists that can mint a result for a cell the matrix denies |

### RC-21 - Decide whether a profile package can name everything it needs, and nothing more

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `docs/adr/0011-source-level-profile-contract.md`, "The source-level profile contract" (promises P1 to P5) and "The closed set of capability transfer types"; `src/Broiler.VM.Abstractions/VmProfileContracts.cs`, `src/Broiler.VM.Abstractions/VmGuestLoad.cs`, `src/Broiler.VM.Abstractions/VmTransferTypes.cs`; the public types in `src/Broiler.VM.Runtime/`; rules A8 and A10 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether every profile-facing contract is declared in Abstractions so that no profile package ever references Broiler.VM.Runtime, and whether the closed transfer-type set is the only expressible signature shape |
| Falsified if | A profile-facing interface, delegate or result type is declared in Broiler.VM.Runtime, or a capability signature can name a type outside the closed set |
| Prior finding | EX-50: the application-local consumer profile does not exist, so P1 and P2 are read against two fixture profiles in one test-only assembly |

### RC-22 - Decide whether the frozen member sets survive on the exported surface

| | |
|---|---|
| Area | RA-6 |
| Verdict | [ ] |
| Read | `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs`, `VmOperationControlHandle` and `VmSuspensionOrigin`; `docs/adr/0009-external-suspension-and-async-instantiation.md`, the `VmOperationControlHandle` member table, the paragraph on disposing a handle that holds an untaken suspension, and the Consequences bullet naming four drift assertions VM-1 gains; `docs/adr/0003-core-contract-v1-and-amendments.md` section 10, the `VmOperationControlHandle` row; `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether `Dispose` is a fifth member the record's "exactly four" must be amended for, or a disposal path the four-member count was never meant to exclude - and whether the promised drift assertions on this member set and on `VmSuspensionOrigin` exist as register rows yet |
| Falsified if | The exported handle carries a member outside `RequestSuspend`, `RequestCancel`, `QueryState` and `TryTakeSuspension` while no record row and no register rule accounts for it |
| Prior finding | EX-21, closed by the milestone at which each named surface exists; `docs/adr/0004-lifecycle-and-state-machine.md` Consequences records the same drift assertions as becoming writable at VM-1 |

## RA-7 - The records themselves

### RC-23 - Decide what happens to the seventeen proposed and unapplied roadmap amendments

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0003-core-contract-v1-and-amendments.md` section 11, all eighteen rows, the quoting convention, and the paragraph stating that no row touches an invariant, a milestone gate, the delivery order, or sections 14 to 16 beyond four named cells; `docs/roadmap.md`; EX-11 in `docs/evidence/vm-0/README.md` and `docs/evidence/vm-1/README.md` |
| Check | Whether the divergence may stand while VM-1 implements the records and the roadmap says something else, or whether the patch must land before either milestone is accepted; and whether each proposed new text narrows the sentence it replaces rather than widening it |
| Falsified if | A row's old text does not match `docs/roadmap.md` character for character under the record's own quoting convention, a row marked `Proposed` has in fact been applied, or a row changes an invariant or a gate while claiming not to |
| Prior finding | EX-11, carried in both bundles and in `docs/roadmap.status.md` as an open VM-0 gate condition |

### RC-24 - Decide whether ADR 0007's monotonicity rules resolve invariant 9's arithmetic

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0007-resource-authority-and-budgets.md`, Context (invariant 9 quoted in full, and the observation that it fixes directions and not an order), "The precedence algorithm", and "Monotonicity, and what raising a ceiling costs" (M1, M2, tighten-only for both classes, and the artifact-versus-host asymmetry); `src/Broiler.VM.Runtime/VmCeilingResolution.cs` and `src/Broiler.VM.Runtime/VmMeter.cs` |
| Check | Whether M1 and M2 plus the clamp-or-refuse asymmetry are a faithful reading of the invariant rather than a new policy, and whether `BudgetRaiseRefused` staying outside `ResourceExhaustion` is right |
| Falsified if | Two implementers can still derive two effective policies from the record's own text, a core operation increases a live meter, or a refused host override is reported as exhaustion |

### RC-25 - Decide whether the aggregate budget resolves invariant 9's composition clause

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0007-resource-authority-and-budgets.md`, "The shared aggregate budget is a core object", ADR 0007 guarantees G1 to G4 (not gate clauses), the two recorded narrowings, and the paragraph on exhaustion not killing siblings; `docs/adr/0009-external-suspension-and-async-instantiation.md` Consequences, the bullet on the roadmap sentence naming a transition that does not exist; `src/Broiler.VM.Runtime/VmAggregateBudget.cs` |
| Check | Whether G4's reading - no runtime created and no operation resumed - is the right resolution of a roadmap sentence that speaks of resuming runtimes, and whether the reviewer accepts the two narrowings, in particular that the victim operation of a shared-parent exhaustion is deterministic in category, dimension and scope but not in identity |
| Falsified if | The record's four guarantees are not jointly deliverable by a pay-as-you-go counter, or the resolution requires a scheduler property the record elsewhere forbids |
| Prior finding | The VM-1 blocker recorded in `docs/evidence/vm-1/README.md`, "Asymmetric accounting": the aggregate live sum could be driven to zero while memory was still live |

### RC-26 - Decide whether parking by unwinding resolves invariant 12

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0009-external-suspension-and-async-instantiation.md`, "Parking By Unwinding", ADR 0009 guarantees G1 to G3 (not gate clauses), the four things it forbids at contract version 1, and the Rejected Alternatives rows on suspending in place and on a core watchdog; `src/Broiler.VM.Runtime/VmOperation.cs`, `TryPark`, `Abandon` and `Expire`; `src/Broiler.VM.Runtime/VmRuntime.cs`, `PollDeadlines` |
| Check | Whether "external control is a lifecycle state, not a side channel" is genuinely discharged by parking as heap data behind a single-use object, and whether disposal is provably free of any wait on a resume, a host reply or an event loop |
| Falsified if | Any path holds a thread for the duration of a pause, disposal can block on a resume that never comes, or the core resumes, cancels or terminates a suspended operation on a timer of its own |

### RC-27 - Decide whether the suspension gates resolve invariant 8 without a shape-only stub

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0009-external-suspension-and-async-instantiation.md`, "The Double Gate On External Suspension", "Asynchronous Instantiation" (the undeclared path and its `invalid state` classification), the authority-is-possession table, and Consequences on ADR 0003 carrying two rows; `docs/adr/0003-core-contract-v1-and-amendments.md` section 8, the admitted-versus-implemented table and the two discharge forms |
| Check | Whether a declaration gate answering `Unsupported` with a naming reason is discharge form (a) rather than a type that exists and refuses everything, and whether "the core authenticates nobody" is an acceptable resolution of who may pause an operation |
| Falsified if | An admitted artefact discharges the invariant by neither form, or a gate returns a truthful-looking refusal from a path no composition can reach |

### RC-28 - Decide each of the twelve records, which are all still `Proposed`

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/README.md`, the index table, the errata paragraph, and the statement that all twelve are `Proposed` and not `Approved`; the `**Status:**` and `**Core contract:**` header fields of `docs/adr/0001-component-topology-and-dependency-graph.md` through `docs/adr/0012-security-ownership-and-support-matrix.md`; rules E1 to E4 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether each record is accepted, accepted with a condition, or rejected on its own - the index cannot be accepted as a block, and the contract-bearing ten cannot change afterwards without the amendment procedure |
| Falsified if | The index lists a record that does not exist or omits one that does, a contract-bearing record declares a version other than the one `VmCoreContract` carries, or a record's status is changed anywhere without a signature in `HUMAN_REVIEW.md` |
| Prior finding | EX-30: all six ownership roles are held by one person, so owner and reviewer confirmation is not independent |

### RC-29 - Decide the one seam ADR 0011 leaves open against ADR 0005

| | |
|---|---|
| Area | RA-7 |
| Verdict | [ ] |
| Read | `docs/adr/0011-source-level-profile-contract.md`, "Exception translation across the capability boundary", rules X1 to X3 and the paragraph headed "One seam to reconcile"; `docs/adr/0005-operation-result-envelope.md`, "Return values, not exceptions" and "Precedence and observation order"; `src/Broiler.VM.Runtime/VmCapabilityBinding.cs` |
| Check | Whether X2 - a core meter already exhausted at the catch boundary is reported as `ResourceExhaustion` rather than as a host defect - is a narrowing of ADR 0005's catch-all as the record claims, and whether the one-line edit the record says the core-contract owner must make before either record is accepted has been made |
| Falsified if | The implementation follows one record and not the other, or the edit is still outstanding while both records are presented for acceptance |

## RA-8 - The evidence and the rule register

### RC-30 - Decide whether the figures in the bundles match the retained logs

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/evidence/vm-1/README.md`, Outputs; `docs/evidence/vm-1/build.log`, `docs/evidence/vm-1/test.log`, `docs/evidence/vm-1/pack.log`, `docs/evidence/vm-1/publish-jit-and-trimmed.log`, `docs/evidence/vm-1/publish-aot.log`, `docs/evidence/vm-1/negative-control.log`, `docs/evidence/vm-1/d1-outcome.txt`, `docs/evidence/vm-1/hashes.txt`; `docs/evidence/vm-0/README.md` and the logs beside it; `HUMAN_REVIEW.md`, "Evidence Available To The Reviewer"; `docs/roadmap.status.md` section 2 |
| Check | Whether every figure quoted in a bundle, in the ledger and in the review file is the figure its log records: 7 projects Release with 0 warnings and 0 errors; 221 tests passing, 90 architecture and 131 behavioural; exactly 3 `.nupkg` and 3 `.snupkg`; a Native AOT binary of 1,279,488 bytes, recorded in the collecting machine's locale as `1.279.488`; a trimmed self-contained binary of 162,816 bytes; 5 checks passed and exit code 0 in each of the three modes; 4 negative controls |
| Falsified if | A document states a figure the log does not, in particular an AOT size other than 1,279,488 bytes or a suite total other than 221, or the SHA-256 list in `docs/evidence/vm-1/hashes.txt` does not recompute against the checkout the bundle claims to describe |
| Prior finding | Ledger update rule 4 forbids promoting a result beyond what it proves; `docs/evidence/vm-1/README.md` records that VM-1-001's figures were all true while sixteen blockers were present |

### RC-31 - Decide whether rule B3 is honestly `Vacuous`

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, the `$comment` block stating the one criterion and the three status values, and the B3 row with its `nonVacuousWhen` and its `witness` field; EX-40 in `docs/evidence/vm-1/README.md`; `HUMAN_REVIEW.md`, the attention item on one rule asserting nothing |
| Check | Whether a violation of B3 is genuinely unreachable by construction - A1 forbids the outbound project reference, A2 the package-shaped one, and the single-source `NuGet.config` makes a foreign `Broiler.*` package unresolvable - so that `Vacuous` with an activation milestone of VM-3 is the honest status rather than a rule quietly retired |
| Falsified if | A path exists by which a product assembly could name a foreign `Broiler.*` assembly in this checkout, which would make B3 `Active` and unwitnessed; or the register's own criterion that every rule carries a witness is not met by a row whose witness field is `none` or null |

### RC-32 - Decide whether the negative controls falsify anything

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/evidence/vm-1/negative-control.log`, all four controls with both runs; `docs/evidence/vm-1/README.md`, "The negative controls, in detail" and the paragraph on control 4; `docs/evidence/vm-0/negative-control.log` and the two discarded controls recorded under Procedure in `docs/evidence/vm-0/README.md` |
| Check | Whether each injected violation is the violation the rule is about rather than an adjacent one, whether the failure counts in each log are consistent with the suite totals, and whether the strengthening of four guest-load assertions after control 4 is enough - a negative control that passes is a finding about the suite, and the same question is owed to the assertions no control touched |
| Falsified if | A control fails for a reason other than the rule it targets, a revert run is absent from a log, or a control's injected edit does not correspond to the code path the bundle says it removes |
| Prior finding | Control 4 did not fail on its first run; four assertions were one level too shallow, testing the fixture's reaction rather than the core's reason |

### RC-33 - Decide whether each register row's witness falsifies that row's statement

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, every row's `statement`, `status` and `witness`; `src/tests/Broiler.VM.Architecture.Tests/witnesses/`; `src/tests/Broiler.VM.Architecture.Tests/RuleRegisterTests.cs`, which the register says enforces it against the test methods and witness inputs in both directions; the review note in the register's `$comment` on V4 and V9 |
| Check | Whether a witness exists that the rule actually rejects, and whether a shared witness input - A7 and A8 name the same `.csproj.witness` file, B1 and B2 the same test assembly - is enough to witness both statements |
| Falsified if | A row's statement is broader than what its rule executes, as V4 and V9 were before the review pass, or a witness is accepted by the rule it is filed under |
| Prior finding | Two register rows were among the surviving review findings: V4 checked a property count rather than the frozen rows, and V9 asserted return types rather than the construction site. EX-51 records what V9 still does not do |

### RC-34 - Decide whether the two bundles cover the eight required fields truthfully

| | |
|---|---|
| Area | RA-8 |
| Verdict | [ ] |
| Read | `docs/roadmap.status.md` section 3, the required-field list, and section 4, update rules 4, 5 and 7; `docs/evidence/vm-0/README.md` and `docs/evidence/vm-1/README.md` end to end, including VM-1-002's superseding banner and VM-0's re-collection note; `docs/evidence/vm-0/hashes.txt` against the current `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` |
| Check | Whether each bundle fills every applicable field, whether the superseded VM-1-001 result is retained rather than deleted, and how a reviewer should read VM-0's Outputs row for a register file that has since been rewritten - VM-0 pins its hash, and the file in the checkout is the VM-1 register with different counts |
| Falsified if | A field is empty without an exclusion naming why, a bundle claims a state the retained logs do not show, or a superseded result has been removed rather than kept as history |
| Prior finding | EX-01, EX-03, EX-42, EX-45: the D1 branch is environment-conditional, no SDK pin exists, the AOT publish is not reproducible by automation, and everything is one RID on one machine with no CI |

### RC-35 - Decide whether the claim each bundle justifies is as narrow as its evidence

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
