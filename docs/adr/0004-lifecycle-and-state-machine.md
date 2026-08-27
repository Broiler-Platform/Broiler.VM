# ADR 0004 - The Common Execution Lifecycle And State Machine

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 7 hands VM-0 the state model and names five of its questions by
hand: which calls may originate on another thread, whether cancellation may be
requested cross-thread, when reentrant execution is rejected, whether external
suspension may be requested and by whom, and how suspended state retains and
releases resources. Invariant 10 then makes every answer stable for the life of
a core contract version, and section 13's VM-0 exit gate requires this record to
name lifecycle states. None of it is decidable until the set of objects the
machine runs over is closed, which invariant 5 demands independently: mutable
state has an owner, and an owner nobody named cannot be tested.

This record closes that set, publishes five state tables with an initiator on
every transition, fixes the instance's state after each operation outcome, and
settles thread affinity, reentrancy, cancellation, disposal, the single
invalid-state outcome, and what a suspended operation retains and releases. It
is the longest VM-0 record because section 7 assigns it the most questions; it
is split by object and by rule, never by convenience.

**What this record does not settle**, and cites rather than restates:

| Subject | Owner |
|---|---|
| The outcome set, the seven envelope-bearing stages, the outcome precedence order, the reason registry, the closed exception set, and `VmDiagnostics` | ADR 0005 (`0005-operation-result-envelope.md`) |
| The frozen descriptor field table and catalog validation | ADR 0002 (`0002-profile-identity-and-static-catalog.md`) |
| `VmVerifiedArtifact` states, leases, and cross-runtime sharing | ADR 0006 (`0006-verified-artifact-ownership.md`) |
| The fifteen budget dimensions, their scopes, precedence, and `VmAggregateBudget` | ADR 0007 (`0007-resource-authority-and-budgets.md`) |
| Guest-load mediation, `IVmArtifactLoadMediator`, and `VmGuestLoadBounds` | ADR 0008 (`0008-guest-initiated-loads.md`) |
| External suspension's double gate, `VmSuspensionOrigin`, `VmOperationControlHandle`, `MaxSuspendedResidency`, `MaxLiveSuspendedOperations`, and asynchronous instantiation | ADR 0009 (`0009-external-suspension-and-async-instantiation.md`) |
| Host-capability descriptors and capability-declared affinity | ADR 0011 (`0011-source-level-profile-contract.md`) |
| The public-name table, the amendment procedure, the amendment register, and the admitted-versus-implemented table | ADR 0003 (`0003-core-contract-v1-and-amendments.md`) |
| The project graph, the architecture-rule register, and its exclusions | ADR 0001 (`0001-component-topology-and-dependency-graph.md`) |

### Everything Named Here Is Paper

Every type, member, enum member, state, transition, initiator, reason code,
runtime option, and descriptor field named anywhere in this record is a **VM-0
decision on paper; no file at VM-0** declares, implements, or asserts any of
them, and that marker is not repeated at each mention. Exactly three artefacts
this record names exist as checked-in files: `src/Broiler.VM.Abstractions/`
`VmCoreContract.cs` (exists at VM-0), which holds the two contract-version
constants and no lifecycle surface; `src/tests/`
`Broiler.VM.Architecture.Tests/rules.register.json` (exists at VM-0), which
holds every architecture rule the component asserts; and the twelve records
under `docs/adr/` (exists at VM-0). The fixture profile, the runtime, the
catalog, every executor and every meter are **deferred to VM-1**.

### Vocabulary This Record Binds

**The unit of execution is an operation.** Roadmap section 7 step 7 uses the
word "sessions" for a thing sections 1 through 6 never define, give states to,
or give an owner. It denotes `VmOperation`, and the word is retired from every
Broiler.VM document, public name, and test name from this record forward. Two
names for one object guarantee two state machines, and section 16 makes a second
core state machine a stop condition. ADR 0003's amendment register carries the
roadmap wording as a proposed, not applied row.

**A stage is one of ADR 0005's seven envelope-bearing stages.** `Dispose`,
`RequestCancel`, `RequestSuspend`, `TryTakeSuspension`, `PollDeadlines`, and
lease acquire and release are **control operations**: they return
`VmControlResult`, consume no untrusted input, and appear in no stage row.

**Unqualified "handle" means the verified-artifact handle** and nothing else.
The operation's control object is always written `VmOperationControlHandle` in
full, and the resumption object is always `VmSuspension`.

## Decision

| # | Ruling |
|---|---|
| 1 | Core contract version 1 defines six lifecycle types and one resumption object, and no others. A seventh is an amendment under ADR 0003's procedure. |
| 2 | Five state tables - builder, catalog, runtime, instance, operation - carry every legal transition, each tagged with exactly one of four initiators, and every state that is terminal. |
| 3 | An instance's state after an operation outcome is fixed by a mandatory mapping. An implementer has no freedom there, and a profile changes it only through one declared descriptor field. |
| 4 | The core owns no thread, work item, timer, or synchronization context. Every deadline is observed at a profile poll point, at the next core entry point, or at `VmRuntime.PollDeadlines()`, which any thread may call. |
| 5 | No core object has thread affinity by default: agility between steps, exclusivity during a step through the runtime's execution slot. A profile may only tighten, and never for cancellation, disposal, an external-suspension request, or a diagnostics read. |
| 6 | A guest-initiated load re-enters the runtime that requested it and no other, and is the only reentrant execution path in the core. |
| 7 | Cancellation may be requested by any holder of a live reference, from any thread, at any time. The latch is monotonic and is never cleared. Latency is bounded in profile work units and **never** in wall-clock time. |
| 8 | Disposal is idempotent, imposes no caller-visible order, is always accepted, and blocks boundedly: a drain budget, then `Orphaned`. Terminal unwind of a suspended operation runs on the disposing thread. |
| 9 | One `InvalidState` category carries every illegal transition and every use after disposal, on every object, at every stage. It never multiplies into per-object cases and never carries a profile payload. |
| 10 | A suspended operation retains everything it held, pauses only the wall clock, is bounded by lifecycle bounds rather than by a sixteenth budget dimension, and releases in one fixed order on every terminal path. |
| 11 | VM-0 freezes all of this on paper. No state-machine code exists at VM-0 and none may be written before VM-1. |

## The Lifecycle Object Set

Contract version 1 defines exactly these, and closes the set.

| Object | What it is | Owner | Disposable | Caller-visible surface |
|---|---|---|---|---|
| `VmCatalogBuilder` | mutable, single-use, composition-root only | the composition root | no | `Add`, `Build` |
| `VmCatalog` | immutable, free-threaded, one state, never disposed | nobody; it outlives every runtime built from it | no | the entries it was built with |
| `VmRuntime` | the execution domain and the unit of isolation | whoever created it | yes | `Verify`, `Instantiate`, `Resume`, `PollDeadlines`, `Dispose` |
| `VmVerifiedArtifact` | the opaque immutable verified artifact (ADR 0006) | nobody; it is lease-counted and may outlive its producing runtime | yes | lease acquire and release, `Dispose` |
| `VmInstance` | profile-owned mutable state instantiated from one `VmVerifiedArtifact` | its runtime | yes | `Invoke`, `Dispose` |
| `VmOperation` | the unit of execution; kind is `Verify`, `Instantiate`, or `Invoke` | its runtime | not caller-addressable | `VmOperationControlHandle` (ADR 0009) and the stage result the call returns |
| `VmSuspension` | the single resumption object of one suspended operation (ADR 0009) | the party the origin determines | no | identity, state, origin, and the profile's opaque projection |

`VmSuspension` is a resumption object, not a seventh lifecycle type: it carries
no state of its own, has no state table, and is valid only while the operation
it addresses is suspended.

**Ownership.** A catalog outlives every runtime built from it and is never owned
by one. A runtime owns its instances and its operations. A `VmVerifiedArtifact`
is **not** owned by the runtime that produced it - section 6 permits sharing one
across runtimes and warns that one runtime must never invalidate another's
input, so ownership by the producer would make every share a use-after-dispose
hazard. An instance holds a lease on its source artifact from `Instantiating`
until `Disposed`. An `Instantiate` or `Invoke` operation is associated with
exactly one instance; a `Verify` operation with none. Nested operations produced
by a guest-initiated load are owned by their parent, are never returned to the
caller, and are not separately addressable.

**Identity.** Every lifecycle object carries an opaque process-local
`VmObjectId` that is stable for its lifetime, never reused within the process,
and never derived from an address, a secret, or host state. ADR 0005's
`VmDiagnostics` surfaces it as `RuntimeId` and `OperationId`; the core exposes
no other identity and no pointer.

**Operation kinds against ADR 0005's stages.** Three kinds cover seven stages,
because two stages are steps inside an operation and one continues one.

| Stage | Operation |
|---|---|
| S1 runtime creation | none; no operation exists yet |
| S2 persisted-envelope preprocessing | a bounded step inside a `Verify` operation |
| S3 caller-driven verification | a top-level `Verify` |
| S4 nested, mediated load | a nested `Verify`, child of the requesting operation |
| S5 instantiation | an `Instantiate` |
| S6 invocation | an `Invoke` |
| S7 resume | no new operation; the operation that suspended continues, keeping its identity, its budget remainder, and its nested-load counters |

`Resume` is deliberately not a fourth kind. A new identity would restart budget
accounting and the nested-load depth and fan-out counters, which section 6
forbids for continued work, and it would make `Suspension` a completion rather
than a state, contradicting invariant 12.

**What the core does not define.** There is no core `Realm`, `Module`, `Frame`,
`Value`, `Scope`, `Scheduler`, `Dispatcher`, `Debugger`, or `Thread` object:
each is profile-owned or host-owned, and a core type named after a language
concept is the lowest-common-denominator failure invariant 4 and section 16
exist to prevent. There is likewise no core `Session` type.

`VmAggregateBudget` (ADR 0007) is a resource-authority object and **not** a
seventh lifecycle object: the host creates it before any runtime exists, it has
no state table here, and the only lifecycle transitions it participates in are
the two admission refusals recorded below.

## The Catalog Builder And The Catalog

`VmCatalogBuilder` states: `Building`, `Consumed` (terminal).

| From | Trigger | Initiator | To | Failure |
|---|---|---|---|---|
| (none) | `VmCatalog.CreateBuilder()` | caller | Building | - |
| Building | `Add(descriptor)` accepted | caller | Building | - |
| Building | `Add(descriptor)` rejected | core | Building, unchanged | throws `VmCatalogValidationException` naming the offending entry and field (ADR 0002) |
| Building | `Build()` accepted | caller | Consumed, producing a `VmCatalog` in `Built` | - |
| Building | `Build()` rejected | core | Building; no catalog is produced | throws `VmCatalogValidationException` |
| Consumed | any call | caller | Consumed, unchanged | throws `VmCatalogValidationException`, reason `BuilderConsumed` |

`VmCatalog` has exactly one state, `Built`. It is immutable, free-threaded, has
no terminal state, and is not disposable.

Composition is the one place the core throws rather than returning a result, and
this record adopts that ruling rather than competing with it: a catalog is
authored by a composition root from trusted compile-time data, so a defect there
is a wiring bug that must be loud and unrecoverable, while result envelopes
exist for stages that consume untrusted input. Catalog construction is therefore
not a stage, `VmCatalogResult` does not exist, and builder misuse is not an
invalid-state reason. The closed exception set of contract version 1 is ADR
0005's three items and this record adds none.

A mutable catalog was rejected: section 3 requires an immutable catalog and
order-independent semantics, and late registration would make catalog identity
time-dependent and defeat VM-3's catalog-drift check. Reusing a consumed builder
is refused for the same reason.

## The Runtime

`VmRuntime` states: `Ready`, `Poisoned`, `Disposing`, `Disposed` (terminal).

| From | Trigger | Initiator | To |
|---|---|---|---|
| (none) | `VmRuntime.Create(catalog, options)` accepted | caller | Ready |
| (none) | `Create` refused: the aggregate parent has no remaining allowance | core | no object; `ResourceExhaustion` naming the dimension at scope `Aggregate` (ADR 0007) |
| (none) | `Create` refused: a dimension carries neither a value nor an adopt marker, or a mandatory lifecycle bound is missing | core | no object; the reason ADR 0007 or ADR 0009 names for that field |
| (none) | `Create` refused: the aggregate parent is already disposed | core | no object; `InvalidState` |
| Ready | a profile breaks the metering contract (ADR 0007); the operation completes `ProfileFault`, reason `ProfileContractViolation` (ADR 0005), and never `InvalidState` | core | Poisoned |
| Ready or Poisoned | `Dispose()` | caller or host | Disposing |
| Poisoned | any call other than `Dispose` or a diagnostics read | any | unchanged; `InvalidState` |
| Disposing | the last in-flight operation completes, or the drain budget expires | core | Disposed |
| Disposing or Disposed | `Dispose()` again | caller or host | unchanged; `VmControlResult.NoOp` |
| Disposing or Disposed | any other call | caller or host | unchanged; `InvalidState` |

Runtime creation is an envelope-bearing stage: it returns
`VmRuntimeCreationResult` (ADR 0005) and never throws. It is **synchronous**;
contract version 1 admits no asynchronous or suspended runtime creation.

Two orthogonal attributes are not states and must be encoded separately:

| Attribute | Values | Rule |
|---|---|---|
| Execution slot | `Free`, or `Held` by exactly one operation | An `Instantiate`, `Invoke`, or resumed operation takes it; verification does not. |
| Concurrent-verification counter | 0 .. `MaxConcurrentVerifications` | A runtime option with a bounded default of 1, raisable by the host, never unbounded. |

**There is no runtime `Suspended` state.** Suspension is a property of executing
work (invariant 12); with one slot holder at a time a runtime state would
duplicate the operation's state and create two truths about one pause. Section
7's run-time sentence "no runtime may be created or resumed once the parent has
no remaining allowance" therefore reads as *no runtime may be created and no
operation resumed*: creation is refused above, and resume admission is refused
in the operation table below.

Keeping the runtime to four states with concurrency as an attribute is what
stops the table multiplying combinatorially, which is the precondition for
invariant 10's stability claim being reviewable. `Poisoned` exists because ADR
0007 requires a runtime whose profile broke the metering contract to accept no
further operation and only disposal; naming the state here is what makes that
requirement a transition rather than an implementation habit.

Hosts that want parallelism create parallel runtimes. One runtime executes one
operation at a time, and that is the only supported parallelism model in
contract version 1.

## The Instance And The Outcome Mapping

`VmInstance` states: `Instantiating`, `Live`, `Executing`, `Suspended`,
`Faulted`, `Disposing`, `Disposed` (terminal).

| From | Trigger | Initiator | To |
|---|---|---|---|
| (none) | an `Instantiate` operation starts | caller | Instantiating; the object is **not** published to the caller |
| Instantiating | instantiation completes `Normal` | core | Live, and the instance is published in the result |
| Instantiating | instantiation suspends, where the descriptor declares asynchronous instantiation (ADR 0009) | guest | Suspended |
| Instantiating | instantiation completes non-`Normal`, or is abandoned while suspended | core | Faulted, then Disposed; no instance is ever published |
| Live | `Invoke` accepted | caller | Executing |
| Executing | the operation suspends | guest or host | Suspended |
| Suspended | `VmRuntime.Resume(VmSuspension)` accepted | whoever holds the runtime and the resumption object | Executing |
| Executing or Suspended | the operation reaches a terminal outcome | core | per the mapping below |
| Live or Faulted | `Dispose()` | caller or host | Disposing, then Disposed |
| Executing or Suspended | `Dispose()` | caller or host | Disposing, with cancellation latched on the operation, then Disposed |
| Disposing or Disposed | any call other than `Dispose` | any | unchanged; `InvalidState` |

**Outcome to instance state. Mandatory; no implementation freedom.**

| Operation outcome | Instance next state |
|---|---|
| `Normal` | Live |
| `Suspension` | Suspended |
| `ProfileFault` | Live where the descriptor declares `FaultRecovery = InstanceRecoverable`; Faulted where it declares `InstanceFatal`. The field is mandatory and has no default. |
| `ResourceExhaustion` | Faulted, always |
| `Cancellation` | Faulted, always |
| `HostFailure` | Faulted, always |
| `InvalidState` | unchanged; the call never entered the profile |
| `UnsupportedProfile` | unchanged; no instance exists |

From `Faulted` only `Dispose` and a diagnostics read are legal; everything else
is `InvalidState` with reason `TerminalFault`. An instance is entered by at most
one operation at a time, and an `Invoke` on an instance that is `Executing` or
`Suspended` is `InvalidState` with reason `WrongState`.

Three consequences are load-bearing and each was chosen against a real
alternative. Publishing the instance only on `Normal` removes the half-built
instance from the machine entirely, so an abandoned asynchronous instantiation
leaves the caller holding nothing rather than an object with no defined state.
Faulting on exhaustion, cancellation, and host failure is the truthful record
that invariant 8 requires: the profile stack was abandoned at an arbitrary
point, so it has no owner-visible state, and declaring it usable would make
VM-4's isolation and use-after-dispose evidence meaningless. Making fault
recovery a declared descriptor field, with no default, keeps the core
semantics-neutral: recoverability is a language property - a WebAssembly trap
and a caught JavaScript exception differ - and a core-wide answer would silently
pick one language's.

**Inspection of a paused instance is the profile's own typed projection on the
`VmSuspension`, never a second core `Invoke`.** Section 7 says external
suspension cannot be used to observe state the profile does not expose, and
section 1 gives the profile ownership of what a paused profile exposes; a second
core entry into a paused stack would contradict invariant 12 and section 1's
non-goal of a cross-profile inspection API.

## The Operation

`VmOperation` states: `Running`, `SuspendedByGuest`, `SuspendedByHost`,
`Completing` (core-owned transient), `Completed` (terminal outcome recorded),
`Orphaned` (core-owned; see disposal), `Disposed` (terminal). Two orthogonal
monotonic latches, `CancellationRequested` and `ExternalSuspendRequested`,
neither ever cleared.

| From | Trigger | Initiator | To |
|---|---|---|---|
| (none) | the caller calls `Verify`, `Instantiate`, or `Invoke` and the preconditions pass | caller | Running |
| (none) | the profile requests a guest-initiated load through the mediator (ADR 0008) | guest | Running, as a nested `Verify` child of the requesting operation |
| Running | the profile yields a guest suspension | guest | SuspendedByGuest |
| Running | the `ExternalSuspendRequested` latch is observed at a profile-declared safepoint | host request, profile acknowledgement, core mediation | SuspendedByHost |
| SuspendedByGuest or SuspendedByHost | `VmRuntime.Resume(VmSuspension)` admitted | the holder of the runtime and the resumption object | Running |
| SuspendedByGuest | an external suspension is requested | host | unchanged; the latch is armed and takes effect at the next safepoint after the operation is next Running |
| Running | the executor returns, faults, exhausts a budget, or observes cancellation | core | Completing |
| SuspendedByGuest or SuspendedByHost | cancellation latched, abandonment through disposal, or a lifecycle bound expires | caller, host, or core | Completing, by bounded terminal unwind |
| Completing | the result is materialized | core | Completed, carrying exactly one outcome |
| Running or suspended | the owning object is disposed and the drain budget expires | core | Orphaned |
| Completed | the control handle is disposed | caller | Disposed |
| Completed or Disposed | resume, suspend, or re-invoke | any | unchanged; `InvalidState`, reason `AlreadyCompleted` or `ObjectDisposed` |

**Two suspended states, three origins.** `VmSuspensionOrigin` (ADR 0009) has
three members and this table has two suspended states; they compose as follows.

| Origin | Operation state | Who receives the `VmSuspension` |
|---|---|---|
| `Guest` | SuspendedByGuest | it rides the caller's suspension result |
| `Instantiation` | SuspendedByGuest | it rides the caller's suspension result |
| `External` | SuspendedByHost | the requesting `VmOperationControlHandle`, once, through `TryTakeSuspension` |

`Instantiation` is a guest-raised pause during instantiation rather than during
invocation, so it parks in the same state; the origin, not the state, is what
distinguishes them on the resumption object. One merged `Suspended` state with a
reason field was rejected: invariant 12 makes the guest-versus-host distinction
normative and section 16 requires the two to stay separate, and a distinction
that lives only in data is one the resume path can ignore.

**Resume admission**, evaluated before any profile continuation runs, in ADR
0005's precedence order: the resumption object belongs to this runtime and has
not been consumed; the operation is neither cancelled nor terminal; the runtime
is `Ready` and its execution slot is free; and the aggregate parent has
remaining allowance, failing which the resume is `ResourceExhaustion` at scope
`Aggregate` rather than a failure discovered mid-continuation.

**Nested operations** complete before their parent leaves `Running`, are never
suspended independently of their parent - `Suspension` is not a legal category
of either load stage (ADR 0005) - and their artifacts are published to the
parent only on `Normal`. They never become top-level operations, never allocate
a fresh budget, and are never separately cancellable, resumable, or disposable:
section 6 gives nested work no independent lifetime, and an addressable nested
operation would let a caller leave a parent mid-verification with partially
verified state, which VM-4's gate forbids.

Three shapes were rejected here and are worth recording. **Preemptive external
suspension** was rejected because .NET offers no safe managed interruption; a
profile stopped at an arbitrary point could promise nothing about what it
exposes, and resume would be undefined. **Nesting an external suspension inside
a guest suspension** was rejected because it creates a suspension stack with an
ordering question at every resume; latching the request and applying it at the
next safepoint after resume is deterministic and costs no state.
**Expressing suspension as an awaited task** was rejected because it imports a
scheduler and a synchronization context into a core that owns no threads, and
would let a continuation run on a pool thread outside the runtime's slot.
Abandonment likewise gets no new outcome category: it is `Cancellation` with a
reason, because the category set is closed and a new category is an amendment
purchased for nothing.

## Initiators And Authority

Every transition above is tagged with exactly one initiator from a closed set of
four. The tag is part of the frozen contract because diagnostics, audit, and
tests all key on it.

| Initiator | Who it is | Transitions it owns |
|---|---|---|
| Caller | the embedding application, through a public member on an object it holds | builder `Add` and `Build`; `VmRuntime.Create`; `Verify`; `Instantiate`; `Invoke`; `Resume` of a `Guest`- or `Instantiation`-origin suspension; lease acquire and release; `Dispose`; `PollDeadlines` |
| Guest | executing profile code, expressed by the profile's executor on the guest's behalf; the guest never calls a core member | `Running` to `SuspendedByGuest`; creation of a nested load through the mediator; completion with `ProfileFault`; a host-capability call |
| Host or external control | a supervisor, watchdog, or diagnostic client, typically off-thread | requesting an external suspension; `TryTakeSuspension` and the resume that follows it; cancellation requested by someone other than the invoking caller; disposal by a supervisor; withdrawal of an aggregate allowance |
| Core | the runtime itself | every transition to `Completing`, `Completed`, `Orphaned`, and `Poisoned`; the instance outcome mapping; artifact `Draining` to `Disposed` on the last lease release; every `InvalidState` rejection |

**Authority is object-capability, never principal.** Possession of a live
reference is the authority to act on it. Contract version 1 defines no
permission, principal, claim, policy, or owner-thread check for cancellation,
disposal, or a diagnostics read, and a profile may not add one. A role or
principal check would need configuration the core does not have and would create
a security surface the core does not own; more decisively, section 14 makes "an
externally suspended operation that cannot be resumed, cancelled or disposed" a
release blocker, and an owner check is exactly what makes a wedged operation
unrecoverable by a supervisor.

**Two gates are declaration gates, not principal gates.** External suspension
requires both the descriptor's `ExternalSuspension` declaration and the runtime
option `ExternalSuspension = Enabled`; a guest-initiated load requires both the
descriptor's `GuestInitiatedLoads` declaration and a registered artifact
provider. A closed declaration gate on a control operation returns
`VmControlResult.Unsupported` naming the missing declaration (ADR 0009 fixes the
two reasons); it is never `InvalidState` and is never silently ignored, because
invariant 8 requires a missing capability to be a truthful "not available here"
rather than a report that something about the caller's state was wrong.

**Control operations return `VmControlResult`**, closed at `Accepted`, `NoOp`,
`InvalidState`, and `Unsupported`. It is a distinct type from the operation
result and adds no category to it: the stage categories are stage-specific and
closed, and a control operation belongs to no stage, so folding it in would
either add categories or misreport. A control result carries exactly one reason
code - a lifecycle reason when the outcome is `InvalidState`, and the missing
declaration when it is `Unsupported`.

## Threads And The Passive Core

**The core owns no threads.** It creates no thread, queues no work item, starts
no timer, and captures no synchronization context. Every unit of core and
profile work executes on the caller's thread, and every deadline - a wall-clock
allowance, the suspended-residency bound, a dispose drain - is observed at a
profile poll point on the executing thread, at the next core entry point, or at
`VmRuntime.PollDeadlines()`, which any thread may call and which never blocks.

A core-owned timer thread was rejected: it is machinery no composition declared,
an interrupt the core cannot deliver safely into managed profile code, and an
AOT and trimming surface with no consumer. `PollDeadlines` puts the same
capability inside a loop the host already runs.

**Default: no thread affinity anywhere. Agility between steps, exclusivity
during a step.** No core object is bound to its creating thread; exclusivity is
enforced by the runtime's execution slot, not by thread identity.

| Call | May originate on another thread | Concurrency rule in contract version 1 |
|---|---|---|
| Catalog `Add` and `Build` | yes | not safe for concurrent use; the composition root must not call it concurrently |
| `VmRuntime.Create` | yes | fully concurrent; the creating thread gains no status |
| `Verify`, top level | yes | may run concurrently with a slot-holding operation and with other verifications up to `MaxConcurrentVerifications`; it does not take the slot |
| `Verify`, nested | no | runs on the requesting operation's thread, inside its slot |
| `Instantiate` | yes | takes the execution slot; `InvalidState` while the slot is held |
| `Invoke` | yes, and need not be the thread that instantiated | takes the execution slot |
| `Resume` | yes, and need not be the thread that suspended | takes the execution slot |
| Cancellation request | yes, always, including during execution | free-threaded and non-blocking |
| External-suspension request | yes, always | free-threaded and non-blocking |
| `Dispose` | yes, always, except from a thread currently executing inside that object | free-threaded; bounded-blocking |
| Diagnostics read | yes, always, concurrently with execution | snapshot-based, non-blocking, never mutating |
| `PollDeadlines` | yes, always | fully concurrent |

Verification deliberately takes no slot. It produces only the immutable artifact
of invariant 3 and touches no per-runtime mutable state, so requiring the slot
would forbid section 11's browser case of verifying a fetched script while
another script runs, forcing a second runtime and duplicate budget bookkeeping
for no safety gain.

**A profile may only tighten, through two declared descriptor fields.**
`ThreadAffinity`, closed at `Agile` and `OperationThreadPinned`, where pinned
requires every resume and every continuation of that operation on the exact
thread that started it; and `SupportsConcurrentVerification`, where false forces
`MaxConcurrentVerifications` to 1 for that profile. A violation is
`InvalidState` with reason `ThreadAffinityViolation`. Affinity composes by
intersection and may only tighten, and a capability-declared affinity may
tighten it further for calls that cross that capability (ADR 0011).

**Four calls can never be tightened by any layer:** the cancellation request,
the external-suspension request, `Dispose`, and a diagnostics read. Section 14's
release blocker and section 16's requirement to bound how long a paused
operation may block disposal both depend on a supervisor being able to act from
outside a wedged thread. A profile may never relax the slot rule, never declare
itself free-threaded within one operation, and never require the core to
marshal.

## Reentrancy

Reentrancy means beginning a call on a core object while a call on the same
runtime is still on the current thread's stack. Contract version 1 states four
rules.

**1. A guest-initiated load re-enters the runtime that requested it, and that is
the only reentrant execution path in the core.** Section 7 requires this record
to answer that question explicitly; the answer is yes, with every qualifier
fixed. It re-enters that runtime and no other; on the requesting operation's own
thread; inside the parent's already-held execution slot, taking no second slot;
only through a registered artifact provider; only as a nested `Verify`, or a
nested `Instantiate` where the descriptor declares it; charged to the parent's
remaining allowances; and bounded in depth, fan-out, cumulative nested bytes,
and cumulative nested verifier work (ADR 0008). Requiring a fresh runtime per
nested load was rejected: a nested load needs the requesting realm's identity
and the parent's remaining budget, and a fresh runtime has neither.

**2. From inside a host-capability callback, calls on the same runtime are
refused.** While a host capability runs, the thread is inside the runtime.
`Instantiate`, `Invoke`, `Resume`, and `Dispose` on that runtime return
`InvalidState` with reason `ReentrancyRefused`, because a second entry would
mutate profile-owned state whose owner is mid-flight, defeating invariant 5.
Permitted from a callback: a cancellation request, an external-suspension
request, a diagnostics read, `PollDeadlines`, and `Verify` on that runtime where
`MaxConcurrentVerifications` allows it, since it takes no slot. The artifact
provider is stricter still: ADR 0008 forbids it any runtime call at all, because
it is a byte source and not a runtime client.

**3. Cross-runtime reentry is legal and depth-bounded.** A host capability may
enter a different runtime, taking that runtime's slot normally. The chain is
bounded by `CallDepth` at scope `Aggregate` (ADR 0007), whose aggregate meter is
the instantaneous sum of live frame depth under one parent and is therefore
exactly the bound a cross-runtime chain needs; exceeding it is
`ResourceExhaustion`, not `InvalidState`, because the transition was legal and
the budget was not. Contract version 1 adds no separate runtime-entry dimension;
the per-thread entry-depth counter is a VM-1 mechanism, not a sixteenth bound.
Forbidding cross-runtime calls outright was rejected: it breaks a host object
bridging two independent runtimes for no safety gain, since the second runtime's
slot and budget already isolate it.

**4. Re-entering a paused stack is refused.** An `Invoke` on an instance that is
`Suspended` is `InvalidState` with reason `WrongState`; inspection while paused
is the profile's projection on the `VmSuspension`.

A profile may tighten these rules, declaring that it accepts no reentrancy at
all. It may never relax them.

## Cancellation

**Who.** Any holder of a live `VmRuntime`, `VmInstance`, or
`VmOperationControlHandle` reference, plus the core itself on budget
exhaustion. No principal and no thread check. Cancelling a runtime latches
cancellation on every operation it owns; cancelling an instance latches it on
the operation entered into that instance.

**Cross-thread.** Always, unconditionally, from any thread, at any time,
including while the operation is inside profile code or a host capability. The
request never blocks, never allocates unboundedly, and returns `Accepted` or
`NoOp` when the latch is already set or the operation is already terminal. No
profile may tighten this.

**Mechanism.** `CancellationRequested` is a monotonic latch on the operation;
setting it latches every descendant nested operation synchronously at request
time. It is never cleared and cannot be revoked. A clearable latch would make
the terminal outcome racy and would let a profile ignore a supervisor.

**Latency.** Core contract version 1 makes **no wall-clock latency guarantee**,
and any support statement about it must say so. What it guarantees is bounded
observability in profile work units: every descriptor declares a mandatory
`CancellationPollBound`, the maximum work a profile may charge between two
cancellation polls, with no default; the core enforces it at ADR 0007's meter,
and an operation that exceeds it completes `ProfileFault` with reason
`CancellationPollBoundExceeded`. Cancellation latched while the operation is
inside a host capability is not observed until that call returns, unless the
capability declares itself cancellable. **A composition whose capabilities may
block indefinitely has unbounded cancellation latency, and that is a property of
that composition, recorded in its own support claim, never a promise the core
can make away.**

Promising a wall-clock deadline was rejected as an untruthful support claim
under invariant 8: the core owns no threads and can preempt neither managed
profile code nor a blocking host capability, so the promise would fail against
the first blocking-capability composition. Work units are what the core can
actually observe, at a meter it already owns.

**Nested work.** Descendants are cancelled with their parent, complete
`Cancellation` first, publish nothing, and leave no partially verified state. A
descendant never cancels its parent by itself: the parent observes the failure
and completes `ProfileFault`, or `HostFailure` or `ResourceExhaustion` where the
provider or the budget failed rather than the artifact.

**Terminality.** Cancellation is terminal for the operation and faults the
instance it entered. It is **not** terminal for the runtime, the catalog, or the
artifact: the runtime stays `Ready` and may instantiate the same artifact again,
and a cancelled `Verify` faults nothing. Making it terminal for the runtime
would force hosts to rebuild a reusable execution domain on every timeout, and
nothing in section 7 asks for that.

## Disposal, Draining, And Orphaning

**Idempotence.** `Dispose` is never rejected for state; the second and later
calls return `NoOp`. The single exception is reentrant self-disposal: calling
`Dispose` on an object from a thread currently executing inside it returns
`InvalidState` with reason `ReentrancyRefused`, and the callback must request
cancellation and let the stack unwind instead. Making a repeated `Dispose` an
error was rejected: section 7 step 7 requires idempotence, and it breaks
`using` and finally-block patterns that legitimately dispose twice.

**Ordering.** The core imposes **no** disposal order on the caller. Disposing a
runtime cascades to its instances and latches cancellation on its operations;
disposing an instance cascades to its operation; disposing an operation releases
its instance and its artifact lease. Disposing a runtime does **not** dispose
the artifacts it produced - it releases only the leases it holds, because an
artifact is shareable and one runtime must never invalidate another's input
(ADR 0006). Any order the caller chooses is legal and deterministic. A required
order is the classic source of use-after-dispose and double-dispose defects, and
it is unnecessary once artifacts are lease-counted and cascading is defined.

**Use after dispose.** Every call other than `Dispose` returns `InvalidState`
with reason `ObjectDisposed` or `ObjectDisposing`, on every object, with no
exceptions and no partial functionality. One survivor is deliberate: the
core-neutral part of a result already returned - category, reason, identities,
accounting, diagnostics - remains readable forever, because the caller owns it.
Projecting the **profile payload** inside it after its instance is disposed is
`InvalidState`, reason `ObjectDisposed`, because the payload may reference
profile-owned mutable state.

**Racing an in-flight operation.** `Dispose` from another thread while an
operation is `Running`: it latches cancellation on that operation and every
descendant; it moves the object to `Disposing`, so every new call is
`InvalidState`; and it waits at most `DisposeDrainBudget` - a runtime option,
wall-clock, bounded default, host-configurable, and legitimately zero for a
non-blocking dispose - for the operation to reach `Completed`. On expiry
`Dispose` returns anyway, the object is `Disposed` for all callers, the
still-running operation becomes `Orphaned`, its result is discarded when the
executor finally returns, its resources are released at that moment, and the
core records a drain-expiry diagnostic. **Disposal therefore blocks boundedly
and never indefinitely.** Refusing to dispose an object with an in-flight
operation was rejected because it leaves a supervisor unable to tear down a
wedged runtime, which section 14 makes a release blocker; blocking without a
bound was rejected because section 16 requires the paused case to be bounded and
an unbounded wait turns a hostile artifact or a slow capability into a permanent
host hang.

**Racing a suspended operation.** No thread is held by a pause, so disposal
performs abandonment on the disposing caller's own thread: it latches
cancellation, then runs the profile's declared terminal-unwind entry point under
the effective unwind allowance - the tighter of the descriptor's `AbandonBudget`
(ADR 0002 carries the field; ADR 0009 fixes its semantics) and the runtime
option `UnwindBudget`, neither of which may raise the other. A profile that
declares no such entry point has its continuation dropped, deterministically.
Expiry completes the operation `Cancellation` with reason `UnwindTimedOut` and
faults the instance. A suspended operation can therefore never block disposal
beyond that allowance, and no other thread's cooperation is required.

**Finalizers.** No core object depends on a finalizer for correctness. One
consequence is recorded rather than hidden: a control handle that is merely
dropped rather than disposed is not observable to the core, and that case is
closed by `MaxSuspendedResidency` expiry instead (ADR 0009).

## The Single Invalid-State Outcome

There is exactly **one** core outcome for every illegal transition and every use
after disposal, on every object, at every stage: `InvalidState`. It is returned,
never thrown. Section 7's phrase "one stable core invalid state outcome" means
one **category**; the specific condition travels as a reason.

Per-object outcomes (`RuntimeDisposed`, `InstanceFaulted`,
`OperationAlreadyCompleted`, and the rest), per-stage variants, and
profile-specific subclasses are forbidden. The precision they offer is already
in the payload; enum growth is what invariant 10 and section 16's result-enum
risk forbid, and each addition would be a breaking amendment that every host
switch and every support claim must absorb. Throwing instead was rejected
because it splits the failure model in two and hides machine-readable state
detail in message strings.

**The lifecycle reasons.** ADR 0005 owns the reason registry; this record fixes
the members `InvalidState` may carry that originate in the lifecycle, and no
profile may extend the set.

| Reason | Raised when |
|---|---|
| `ObjectDisposed` | any call other than `Dispose` on a disposed object, or a payload projection after its instance is disposed |
| `ObjectDisposing` | any call other than `Dispose` on an object that is draining |
| `TerminalFault` | any call other than `Dispose` or a diagnostics read on a `Faulted` instance or a `Poisoned` runtime |
| `WrongState` | a legal call in the wrong state: a second `Invoke` on a busy or paused instance, a resume while the slot is held |
| `AlreadyCompleted` | resume, suspend, or re-invoke of an operation that has completed |
| `ReentrancyRefused` | rule 2 above, and reentrant self-disposal |
| `ThreadAffinityViolation` | a call on a thread a declared affinity forbids (ADR 0011) |
| `ResumeTokenConsumed` | a resumption object already consumed or abandoned, or presented to a runtime that does not own it |
| `HandleDraining`, `HandleDisposed`, `ForeignHandle` | the artifact-lease conditions ADR 0006 owns |

Two reasons from the underlying ruling are struck rather than silently dropped.
`BuilderConsumed` is not a lifecycle reason, because catalog construction is not
a stage; it is a `VmCatalogValidationException` reason owned by ADR 0002.
`SuspensionKindMismatch` is unreachable once resume has exactly one entry point:
a `Guest`- or `Instantiation`-origin resumption object is never handed to a
control handle, and `TryTakeSuspension` answers `Unsupported` rather than
crossing a resume path that no longer exists.

`ProfileContractViolation` is deliberately not on the list either, and no
reading of this record puts it there. A profile that breaks the metering
contract - by charging more than its declared `MaxUnchargedWork` between two
polls, for instance (ADR 0007) - has already run, so the breach completes the
operation `ProfileFault` with that reason, which ADR 0005 owns and registers
under that category, and poisons the runtime. Only the calls that arrive after
the poisoning are `InvalidState`, with reason `TerminalFault`. Reporting a
breach the profile committed as `InvalidState` would assert that something
about the caller's state was wrong when nothing was, which is exactly the
misreport the closed set above exists to prevent.

**What the result must answer.** An `InvalidState` result must, on its own,
answer which contract version produced it, which object was rejected and in
which state, which transition was attempted, and why. ADR 0005's `VmDiagnostics`
carries the contract version, the stage, the outcome, the reason, and the
runtime and operation identities; the object kind, the object's observed state,
and the attempted call are the three facts this record adds, in the same shape
as that record's per-category groups. It must **never** carry a profile-typed
payload: no profile code ran, so a payload would be fabricated, and fabricating
a language result for a core rejection is the semantics leak invariant 4
prevents. The core never asks a profile to interpret an invalid-state rejection
and never converts one into a `ProfileFault`.

## What A Suspended Operation Retains And Releases

**Retained.** A suspended operation retains its profile continuation, its
instance and the instance's lease on the artifact, its remaining fuel,
allocation, and host-call allowances, its wall-clock remainder, its nested-load
depth and fan-out counters, its `VmSuspension`, and its share of any aggregate
budget including the live-runtime count. Nothing is released early and nothing
is silently reclaimed: a paused operation must be found exactly as it was
paused. Releasing and re-acquiring leases or reservations across a pause was
rejected because re-acquisition can fail, which would make resume
nondeterministic and could let another runtime dispose an artifact underneath a
paused stack.

**Metering.** Fuel, allocation, and host-call meters are frozen, since no work
occurs. **The wall clock is paused under every origin - `Guest`, `External`, and
`Instantiation` alike.** Charging it would make host inaction consume the
guest's budget, turning a breakpoint or a slow host fetch into a timeout, and it
would let a parked sibling drain a shared aggregate parent while doing no work,
which makes the failing sibling depend on host scheduling - a nondeterministic
failure class section 14 blocks. There is consequently no host option to keep
the clock running: such a knob would let a composition change an operation's
failure class. Aggregate accounting is otherwise unchanged: a suspended
operation still counts against its parent's allocation and live-runtime
allowances, because the memory is still held, so a host cannot multiply a
ceiling by parking work.

**Bounds.** Because the wall clock is paused, the pause needs its own bound.
Contract version 1 uses two lifecycle bounds rather than a sixteenth budget
dimension - `MaxSuspendedResidency` and `MaxLiveSuspendedOperations`, both
mandatory and finite at runtime creation (ADR 0009). Keeping them out of the
dimension set is what preserves invariant 9's rule that a `ResourceExhaustion`
result names exactly one dimension and one scope. Leaving residency unbounded
was rejected under invariant 9 - omission never means unbounded - and because an
abandoned pause would otherwise pin an instance, a lease, and an aggregate
allowance forever.

**Release order on any terminal transition. Fixed, mandatory, encodable.**

| # | Released |
|---|---|
| 1 | descendant operations, innermost first |
| 2 | the profile continuation, through the terminal-unwind entry point under the effective unwind allowance, or dropped where the profile declares none |
| 3 | the instance's lease on the artifact, and any host-handle registrations the operation owned |
| 4 | the operation's reservations, returned to the runtime meter |
| 5 | the runtime's reservations, returned to the aggregate parent |
| 6 | the `VmSuspension`, invalidated; every later use is `InvalidState`, reason `ResumeTokenConsumed` |

One order on every path is what lets VM-4's reclamation and memory-plateau
measurements attribute memory identically whether an operation completed, was
cancelled, expired, or was abandoned.

## What VM-0 Freezes And What VM-1 Implements

**VM-0 freezes, in this text and in no executable form:** the six lifecycle
types and the resumption object; the five state tables with every state,
transition, initiator, and terminal state; the outcome-to-instance-state
mapping; the initiator taxonomy and the object-capability authority model; the
thread-affinity table, the passive-core rule, and the two profile-tightening
descriptor fields; the four reentrancy rules; the cancellation contract
including the absence of a wall-clock promise; the disposal contract including
drain, orphan, and unwind; the single `InvalidState` outcome with its lifecycle
reasons and the three facts its result must add; the suspended retention,
bounds, and release order; and the closed `VmControlResult`.

**The runtime-creation options object.** `VmRuntime.Create(catalog, options)`
takes exactly one options object, and this record owns its field list.

| Group | Content | Fixed by |
|---|---|---|
| Aggregate budget | a `VmAggregateBudget`, or null for an unparented runtime | ADR 0007 |
| Ceilings | for each of the fifteen dimensions, an explicit value or an `AdoptProfileDefault` or `AdoptParentRemaining` marker. Omission is not a value and fails runtime creation | ADR 0007 |
| Lifecycle bounds | `MaxSuspendedResidency` and `MaxLiveSuspendedOperations`, both mandatory and finite (ADR 0009); `MaxConcurrentVerifications`, `DisposeDrainBudget`, and `UnwindBudget`, each with a bounded default the host may tighten or raise but never unbound | this record |
| Guest-load bounds | the `VmGuestLoadBounds` group | ADR 0008 |
| External suspension | the per-runtime `ExternalSuspension` enablement flag, `Disabled` by default | ADR 0009 |
| Capability bindings | the typed host capabilities this runtime registers, including any artifact provider | ADR 0011, ADR 0008 |

**Four descriptor fields this record requires.**
`SupportsConcurrentVerification`, `ThreadAffinity`, and `CancellationPollBound`
are fields 10, 11, and 12 of ADR 0002's frozen descriptor table, and
`FaultRecovery` is the fourth, carried by that same table with its semantics
fixed here. The table is the single authority for all four, including their
spelling. This record also relies on, and does not own, `ExternalSuspension`,
`GuestInitiatedLoads`, `AsynchronousInstantiation`, and `AbandonBudget`.

**VM-0 also freezes what contract version 1 admits and release 1 need not
implement.** The external-suspension transitions exist even though no release-1
profile declares them; the guest-load reentry path exists even though no
release-1 composition registers a provider; asynchronous instantiation exists as
the `Instantiating` to `SuspendedByGuest` transition even where no profile
declares it. In each case the state, the transition, the refusal, and the
failure outcome are frozen now, and only the provider, the declaration, and the
executor are absent. Section 6 states the reason for the guest-load case and it
generalizes: retrofitting a re-entrant lifecycle after the contract is frozen is
an amendment, while specifying it now is a paragraph. ADR 0003's
admitted-versus-implemented table carries the rows.

**VM-0 writes no state-machine code.** The shells contain no state enums with
behaviour, no transition logic, no executor, and no meters. A VM-0 shell that
carried a half-implemented lifecycle would create evidence the ledger's update
rule 4 forbids promoting, and would invite the second, divergent state machine
section 16 makes a stop condition.

**VM-1 implements** the tables as data-driven code, the execution slot, the
per-thread entry-depth counter, the latches, thread-safe meters and lease
counts, the bounded options, the refusal paths, and the invalid-state detail,
and proves them against the fixture profile. **VM-4 hardens** them under stress,
abandonment, and concurrency. The enum and member baselines the drift
assertions will compare against are created in VM-1 and frozen in VM-6.

## Amendments This Record Absorbs

The reconciliation of the eight analyst clusters changed thirteen things in the
rulings behind this record. They are listed rather than silently applied, so a
reader comparing this record with an earlier draft can see what moved and why.

| As drafted | As decided here | Because |
|---|---|---|
| `VerifiedArtifact`, `VmCompositionException` | `VmVerifiedArtifact`, `VmCatalogValidationException` | ADR 0003's public-name table; the second name is retired |
| Builder misuse returns `InvalidState` | it throws, and `BuilderConsumed` is a catalog-validation reason | catalog construction is not a stage (ADR 0002, ADR 0005) |
| Catalog construction is stage S1 | there are seven envelope-bearing stages and no `VmCatalogResult` | ADR 0005 |
| `ResumeFromExternalSuspension` as a second resume path | one entry point, `VmRuntime.Resume(VmSuspension)` | two entry points mean two admission checks and a race (ADR 0009) |
| `SuspensionKindMismatch` | struck | unreachable once one resume path exists |
| `RequestResume` on the control handle | `TryTakeSuspension` plus the single resume | ADR 0009 |
| A runtime-level external-suspend arm latch | struck | it is a second control path with no token holder; the request belongs on the operation's control handle (ADR 0009) |
| `MaxRuntimeEntryDepth` as a new bound | `CallDepth` at scope `Aggregate` | a sixteenth dimension is not needed for a measure the aggregate ceiling already sums (ADR 0007) |
| `UnwindBudget` as the only unwind bound | the tighter of the descriptor's `AbandonBudget` and the runtime option | the profile declares its abandon allowance (ADR 0002, ADR 0009) |
| `ForeignThreadAffinityViolation` | `ThreadAffinityViolation` | one name for one condition (ADR 0011) |
| A closed external-suspension gate is `InvalidState` | `VmControlResult.Unsupported` | invariant 8: a missing capability is not an illegal transition (ADR 0009) |
| Wall clock continues under guest suspension | it pauses under every origin, with no host override | a parked sibling would drain a shared parent doing no work (ADR 0007) |
| Runtime states `Ready`, `Disposing`, `Disposed` | `Poisoned` added | ADR 0007 requires a runtime that broke the metering contract to accept only disposal |

## Consequences

- **Section 3's illustrative snippet is superseded in its second line, and the
  roadmap is not edited.** Section 3 introduces `var vm =
  VmRuntime.Create(catalog);` as a shape whose exact public names are deferred
  to VM-0. Under the frozen decisions runtime creation is an envelope-bearing
  stage that must also carry an aggregate budget, an explicit entry for every
  dimension, the mandatory lifecycle bounds, the guest-load bounds, the
  external-suspension flag, and the capability bindings, and it must return a
  result rather than a bare object. The corrected shape (VM-0 decision on paper;
  no file at VM-0) is:

```csharp
var catalog = VmCatalog.CreateBuilder()
    .Add(FixtureVmProfile.Descriptor)
    .Build();

var created = VmRuntime.Create(catalog, options);
if (!created.IsSuccess)
{
    // VmRuntimeCreationResult carries the outcome, the reason, and diagnostics.
}
```

  ADR 0003's amendment register carries the section 3 code block as row 3 and
  section 7 step 2's runtime-creation clause as row 7, both proposed and not
  applied. The builder lines stand as written (ADR 0002).

- **Two further roadmap sentences are superseded and neither is edited.**
  Section 7 step 7's word "sessions" denotes `VmOperation`, and section 7's
  run-time sentence and section 16's aggregate-budget risk row say "no runtime
  may be created or resumed", which names a transition that does not exist
  because runtimes have no suspended state. Both are rows in ADR 0003's
  amendment register, marked proposed and not applied. Nothing in this record
  states or implies that an invariant, a milestone gate, a delivery order, or
  section 14, 15, or 16 text has changed.

- **Section 14's cancellation blocker is met in the only terms the core can
  meet it.** "Unbounded cancellation latency" is enforced as a bounded poll
  interval in profile work units, declared per profile and enforced at ADR
  0007's meter. The residue - a host capability that blocks and declares itself
  non-cancellable - is truthfully a composition property, and ADR 0012
  (`0012-security-ownership-and-support-matrix.md`) records that no public
  support table exists at VM-0 to state it in. This record changes no gate text:
  the corrected blocking-failure wording is row 15 of ADR 0003's register,
  proposed and not applied.

- **Not enforced or provable at VM-0, stated plainly.** No architecture rule in
  `rules.register.json` (exists at VM-0) asserts any object, state, transition,
  initiator, reason, option, or bound decided here: the VM-0 product graph
  exports one static class holding two integer constants, so there is no
  lifecycle surface for a rule to range over. The drift assertions these tables
  will need - the state and reason member sets, the absence of any awaitable or
  thread-typed public member, the absence of a `Session`-named type, the
  `VmControlResult` member set - become writable at VM-1, and promoting them is
  part of that milestone's exit evidence. ADR 0001 owns the register and the
  count and identifiers of every Vacuous and Deferred rule.

- **Two facts this record needs from sibling records are not there yet.** ADR
  0005's frozen `VmDiagnostics` field set has no per-category group for
  `InvalidState` carrying the object kind, its observed state, and the
  attempted call. ADR 0003's frozen public-name table carries `VmRuntime`,
  `VmRuntimeCreationResult`, and `VmControlResult` for this record but not
  `VmInstance`, `VmOperation`, `VmObjectId`, or `VmThreadAffinity`. Each is a
  small edit to the record that owns it, made under that record's own amendment
  rule; until both land, this record and those two disagree by exactly one
  diagnostics group and four name-table rows. Recording the gaps is cheaper
  than assuming them closed, and each is visible to a reader comparing the
  three records.

- **VM-1 inherits named implementation obligations from this record**, and they
  are obligations rather than gate changes: budget meters, latches, lease
  counts, slot ownership, and state fields must be safe for concurrent access,
  because cancellation, disposal, diagnostics reads, and `PollDeadlines` are
  unconditionally free-threaded; the executor must maintain a per-thread
  runtime-entry depth counter and a slot owner-thread field; and the fixture
  profile needs a variant that deliberately exceeds its declared
  `CancellationPollBound`, so the violation is proved detected rather than
  tolerated. VM-1's test plan is generable mechanically from the five tables:
  every (state, call) pair is either a listed transition or an expected
  `InvalidState` with a named reason.

- **What this record forbids** is worth stating once as a list, because each
  item is a shape an implementer would otherwise reach for: a core `Session`
  type or any core object named after a language concept; a core-created thread,
  work item, timer, or captured synchronization context; default thread affinity
  on any core object; a profile relaxing affinity, the slot rule, or who may
  cancel and dispose; principal, role, or owner-thread checks anywhere; a
  revocable cancellation latch; a wall-clock cancellation promise; a rejected
  repeated `Dispose`; a required disposal order; an unbounded blocking
  `Dispose`; disposing a shared artifact as a side effect of disposing a
  runtime; finalizer-dependent correctness; per-object or per-stage
  invalid-state outcomes; throwing for an illegal transition; a profile payload
  on an invalid-state result; publishing an instance before its instantiation
  completes `Normal`; reusing an instance after exhaustion, cancellation, or
  host failure; a second concurrent `Invoke` on one instance; a core inspection
  API for a paused instance; a merged suspended state; preemptive interruption
  of a running executor; a task, awaitable, or callback in place of a suspension
  state; a nested load taking a second slot, a fresh budget, or a top-level
  identity; charging wall-clock time to a suspended operation; unbounded
  suspended residency; and any lifecycle behaviour in a VM-0 shell.
