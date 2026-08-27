# ADR 0009 - External Suspension And Asynchronous Instantiation

**Status:** Proposed
**Date:** 2026-08-27
**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 13 requires VM-0 to record two of its four "even where the first
release ships no implementation" decisions here: the external-suspension
transitions, and whether core contract version 1 admits asynchronous
instantiation. Section 7 prices the second one exactly - "VM-0 records whether
core contract version 1 admits it, and adding it afterwards is an amendment" -
and invariant 12 shapes the first: external control is a lifecycle state, not a
side channel, and a host may pause and resume "only through transitions the core
contract declares".

They are one record because they are one mechanism. Section 7 gives suspension a
single outcome category shared by a guest yield, a host pause, and an
instantiation park, so the three differ only in who caused the pause and who is
entitled to undo it. Settling them apart would have produced three parking
models, three resumption objects, and three resume paths.

Two constraints bound every ruling below and neither is satisfiable by a design
that holds a thread. Section 14's lifecycle row makes "an externally suspended
operation that cannot be resumed, cancelled or disposed" a release blocker, and
section 16 requires the contract to bound how long a paused operation may block
disposal.

Everything this record decides is paper. Milestone VM-0 checks in five project
shells and one pair of public constants (exists at VM-0:
`src/Broiler.VM.Abstractions/VmCoreContract.cs`); no type, member, state,
transition or bound named below exists in the checkout, and the rule register
(exists at VM-0: `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`)
asserts none of them. See Exclusion EX-21.

## Decision

| # | Ruling |
|---|---|
| 1 | Core contract version 1 admits external suspension and asynchronous instantiation. Both are declaration-gated; neither is reachable for a profile that does not declare it. |
| 2 | Suspension of every origin parks by unwinding. No thread is held anywhere for the duration of a pause. |
| 3 | Exactly one resumption object exists per suspended operation - `VmSuspension` - and its holder is determined by the suspension's origin. |
| 4 | Resume happens exactly one way: `VmRuntime.Resume(VmSuspension)` on the owning runtime. |
| 5 | External suspension is gated twice: the profile declares it, and the composition enables it per runtime, disabled by default. |
| 6 | Authority is possession, not identity. The core authenticates nobody and defines no principal, permission, or diagnostic client. |
| 7 | Two mandatory finite lifecycle bounds - `MaxSuspendedResidency` and `MaxLiveSuspendedOperations` - bound a paused operation. Neither is a budget dimension. |

Every type, member, enum member, option, state, reason code, and bound named
anywhere in this record is a VM-0 decision on paper; no file at VM-0 declares
any of them, and that marker is not repeated at each mention.
`VmSuspension`, `VmSuspensionOrigin`, `VmOperationControlHandle`,
`MaxSuspendedResidency` and `MaxLiveSuspendedOperations` are settled by this
record. `VmRuntime`, `VmControlResult` and the runtime-creation options object
are settled by ADR 0004 (`0004-lifecycle-and-state-machine.md`);
`VmProfileDescriptor` and `VmCatalogValidationException` by ADR 0002
(`0002-profile-identity-and-static-catalog.md`). The twelve ADR files
cross-referenced below exist at VM-0 under `docs/adr/`; every other artefact
they describe carries its own marker in its own record.

## Parking By Unwinding

Suspension of every origin parks by **unwinding**. The `suspension` outcome
returns out of the invoke or instantiate call to the caller that made it, and no
thread is blocked inside the core, inside the profile, or inside a host callback
while an operation is suspended. A suspended operation is data on the heap
behind a single-use resumption object; it is not a parked stack.

Three guarantees follow and are frozen at contract version 1.

| # | Guarantee | Why it holds unconditionally |
|---|---|---|
| G1 | Every suspended operation can be cancelled and disposed without ever being resumed. | Neither path needs the continuation to make progress: cancellation latches monotonically, disposal unwinds. |
| G2 | Disposal never waits on a resume, an event loop, a host reply, or another thread. It runs the profile's terminal-unwind entry point on the disposing thread under the bounded unwind budget ADR 0004 defines, releases core-owned resources, and returns. | The only work disposal must do is done by the thread that asked for it. |
| G3 | An abandoned suspended operation is reclaimed by ownership, not by a clock: it belongs to its runtime and dies with it. The core mints no watchdog and never resumes, cancels, or terminates an operation on a timer of its own. | A core timer would make a breakpoint a race against machine speed and would misreport a slow host as a fault. |

This forbids, at contract version 1:

- blocking any thread inside the core or a profile for the duration of a
  suspension, including inside a host capability callback;
- a disposal path that waits on a resume, a host reply, an event loop, or an
  unbounded profile callback;
- a core-owned timer, watchdog, or finalizer that resumes, cancels, or
  terminates a suspended operation; and
- unbounded accumulation of live suspended operations.

Unwinding is the only model that delivers G1 through G3 without qualification.
Any design that pauses in place holds a thread, and a held thread means disposal
either waits for a resume that may never come or aborts a thread, which .NET
does not safely offer. Invariant 12's distinction is the same one in different
words: a lifecycle state is data, a blocked call stack is a side channel.

## One Resumption Object, One Resume Path, And Who Holds It

`VmSuspension` is a sealed class with no public constructor. It is minted only
when an operation has actually parked, is single-use, and is bound to exactly
one runtime, one instance, and one operation. It exposes identity, state,
origin, and the profile's opaque projection, and nothing else.

`VmSuspensionOrigin` has exactly three members. The origin decides who receives
the resumption object, what the caller's result carries, and therefore who can
resume.

| Origin | Who caused the pause | Where the `VmSuspension` is delivered | The caller's suspension result carries | Typed profile payload |
|---|---|---|---|---|
| `Guest` | the profile, on the executing guest's behalf | the caller's suspension result | the resumption object and the profile's projection | permitted |
| `Instantiation` | the profile, parking a partially built instance | the caller's suspension result | the resumption object and the profile's projection | permitted |
| `External` | a holder of the operation's `VmOperationControlHandle` | the requesting control handle, once, through `TryTakeSuspension` | the origin and the operation identity only - no resumption object, no profile payload | forbidden |

Exactly one `VmSuspension` exists per suspended operation in every row. One
object means one admission check, one resume, and no race. ADR 0005
(`0005-operation-result-envelope.md`) owns the payload-carriage table; this
record fixes only which origins may fill the slot.

Resume happens exactly one way: `VmRuntime.Resume(VmSuspension)` on the owning
runtime, which consumes the object. A second resume, a resume after
cancellation, disposal, or abandonment, and a resume presented to a runtime that
does not own the object each complete `invalid state`. Resume admission checks
the aggregate parent before any profile continuation runs, so an exhausted
`VmAggregateBudget` refuses the resume rather than failing mid-continuation
(ADR 0007, `0007-resource-authority-and-budgets.md`).

`VmOperationControlHandle` exposes exactly four members. It is minted at
operation start and handed to the invoking caller.

| Member | Returns | Semantics |
|---|---|---|
| `RequestSuspend` | `VmControlResult` | arms a monotonic pending-suspend latch; never blocks the requester |
| `RequestCancel` | `VmControlResult` | arms the cancellation latch; free-threaded and never tightenable by a profile (ADR 0004) |
| `QueryState` | a state snapshot | operation state and suspension origin; non-blocking, never mutating |
| `TryTakeSuspension(out VmSuspension)` | `VmControlResult` | single-use; hands over the External-origin resumption object exactly once |

`TryTakeSuspension` maps onto the closed four-member result as follows:
`Accepted` when the operation has parked with `External` origin at this handle's
request and the object is handed over; `NoOp` when the pending suspend has not
yet been observed, so nothing is taken; `InvalidState` when the object was
already taken or the operation is terminal; `Unsupported` for `Guest`- and
`Instantiation`-origin suspensions, whose object rides the caller's result.

**Authority is possession, stated positively.**

| Authority | Held by | Granted by |
|---|---|---|
| Pause | possession of the operation's `VmOperationControlHandle` | the invoking caller, who is minted the handle and alone decides who receives it |
| Resume | possession of both the owning `VmRuntime` and the `VmSuspension` | `TryTakeSuspension` for `External` origin; the caller's suspension result for `Guest` and `Instantiation` |
| Cancel and dispose | possession of any live reference to the operation, its instance, or its runtime | ADR 0004 |

The core authenticates nobody. Contract version 1 defines no principal,
permission, claim, or policy, and knows no "diagnostic client". A host that
grants the pause authority without the resume authority has created a
composition-level limitation, not a core defect. Any richer model would require
the core to own an identity and an audit trail it cannot define without a host's
security model, and a permission check the core cannot verify is precisely the
shape-only stub invariant 8 forbids from satisfying a capability gate.

The unresumable case is therefore closed deterministically rather than by a
watchdog. Disposing a control handle that still holds an untaken External-origin
`VmSuspension` latches the operation cancelled with reason
`ExternalSuspensionAbandoned`; a resumption object that is taken and never used
is bounded by `MaxSuspendedResidency`. Cancel and dispose remain available to
the runtime owner throughout, by G1.

**Retired names.** These are struck and appear in no contract surface.

| Struck name | Replaced by |
|---|---|
| `VmSuspensionToken` | `VmSuspension` - a copyable value type cannot enforce single use, and the object carries the profile's projection, which is reference-shaped state |
| `VmSuspensionCause` | `VmSuspensionOrigin` - a two-member cause enum cannot name the third origin once asynchronous instantiation is admitted |
| `RequestResume` | `TryTakeSuspension` plus `VmRuntime.Resume(VmSuspension)` - one admission check instead of two |
| `WallClockPausesWhileSuspended` | nothing; the wall clock pauses under every origin unconditionally (ADR 0007) |

## The Double Gate On External Suspension

| Gate | Declared by | Default | Result when closed |
|---|---|---|---|
| `ExternalSuspension` on `VmProfileDescriptor` (ADR 0002) | the profile author, at composition time | none - the field is mandatory and explicit | `VmControlResult.Unsupported`, core diagnostic reason `ExternalSuspensionNotDeclared` |
| `ExternalSuspension` in the runtime-creation options (ADR 0004) | the composition root, per runtime | `Disabled` | `VmControlResult.Unsupported`, core diagnostic reason `ExternalSuspensionNotEnabled` |

Both gates are declaration gates, so a refusal is `Unsupported` and never
`invalid state`: nothing about the operation's state is wrong, the capability
was simply never claimed. The two reasons are kept distinct because they name
different owners - one is a profile that cannot pause, the other a composition
that will not allow it - and a host debugging a silent refusal needs to know
which. Default-`Disabled` follows invariant 8: the powerful surface is off until
a composition claims it, and the claim then belongs in that composition's
support statement.

**`RequestSuspend` is a request, not a transition.** It arms a monotonic latch
that the executing profile observes at the same safepoints as cancellation and
returns immediately without blocking the requester; the operation transitions by
unwinding at the next safepoint and returns `suspension` with origin `External`
to its invoking caller. Making suspend synchronous was rejected: it would
reintroduce cross-thread blocking and let a runaway guest hang whoever asked for
the pause.

**Cancellation outranks a pending suspend at every safepoint.** ADR 0005's
outcome precedence order places `Cancellation` second and `Suspension` second to
last, so an operation with both latches armed completes `cancellation`. This is
what gives a deterministic answer when one party pauses an operation another
party is simultaneously tearing down.

**Pause latency is bounded in profile work units, not in wall clock.** Both
latches are read at the same declared poll points, so the descriptor's mandatory
finite cancellation-poll bound (ADR 0004) governs them together. Contract
version 1 makes no wall-clock promise about how long a pause request takes to
park an operation - Exclusion EX-23.

**Resume is semantically transparent.** A guest resumed from an external
suspension observes exactly the behaviour it would have observed without the
pause, except for observations derived from wall-clock time. This is why an
External-origin suspension carries no payload and why the wall clock is paused
under every origin (ADR 0007): a pause the host imposed must not change the
guest's result or its failure class.

**What the core exposes while an operation is paused is closed.**

| The core always exposes | The core never exposes |
|---|---|
| operation state and `VmSuspensionOrigin` | frames, stacks, or activation records |
| the budget snapshot, consumed and remaining per dimension (ADR 0007) | values, variables, or scopes |
| core diagnostic identity: runtime, profile ID, artifact identity, guest-load depth (ADR 0005) | source positions or any position table |
| the declared thread affinity (ADR 0004) | a breakpoint, stepping, or expression-evaluation surface |

A profile that exposes nothing at all while paused is fully conformant. Anything
a debugger sees beyond the left column comes from the profile's own surface,
through the opaque projection on the `VmSuspension`, and the core is not its
transport. A second `Invoke` against an instance whose operation is suspended is
`invalid state` - the execution slot is held - and is not an inspection path.
Section 1's non-goals forbid a debug wire protocol, a cross-profile inspection
API, and a profile-neutral breakpoint model, and state that VM-0 freezes "only
the external-suspension transitions that a profile-owned debug surface needs".
Four members and no inspection is that minimum; a core-neutral frame or variable
API would be the archetypal lowest-common-denominator core invariant 4 exists to
prevent.

## Asynchronous Instantiation

Core contract version 1 **admits** asynchronous instantiation. The instantiation
stage may complete `suspension` only where the selected profile's
`VmProfileDescriptor` declares `AsynchronousInstantiation` (ADR 0002).
ADR 0005 owns the stage-outcome matrix; this record fixes the declaration gate,
the undeclared path, and what a suspended instantiation may hold.

**A suspended instantiation publishes no instance.** The verified artifact, the
profile's partial state, and the `VmSuspension` are retained. No instance handle
exists and no API may return one until a resume completes with `normal`. A
resume yields any outcome legal at the instantiation stage, `suspension` among
them.

**No separate lifecycle state is minted.** ADR 0004 owns the operation state
set; an instantiation park is a profile-caused pause and occupies the same
suspended state as a guest yield, with `VmSuspensionOrigin.Instantiation` as the
discriminator. The distinct `InstantiationSuspended` state proposed in this
record's source ruling is not created: it would duplicate every row of ADR
0004's transition table to carry a distinction the origin field already carries,
and a second suspended state is a second thing to keep correct.

**The undeclared path.** A profile that completes instantiation with
`suspension` without the declaration has violated the profile contract. The core
abandons the pending instantiation through the profile's bounded abandon entry
point and completes the operation `invalid state` with reason
`UndeclaredAsynchronousInstantiation`. It is `invalid state` and not `profile
fault` because the condition is an illegal transition against a declared
contract rather than a language-defined fault the profile chose to report, and
because ADR 0005's precedence order puts `InvalidState` first, which makes the
classification unambiguous when the abandon path also exhausts a budget. No
instance is published and the operation is not resumable.

**A host that cannot pump an event loop keeps two deterministic outs**, and they
are the load-bearing consequence of admitting the transition at all.

| Out | Mechanism | Where the refusal lands |
|---|---|---|
| Never compose the capability | section 3 requires a composition root to name every descriptor it adds directly, and the declaration is a mandatory field on that descriptor, so whether any composed profile can park during instantiation is a build-time fact the composition root already controls | build time, by reading the descriptors it names |
| Cancel and dispose a suspended instantiation | always legal, never requires a resume, bounded by G1 and G2 | `cancellation` |

No `AllowAsynchronousInstantiation` catalog option is minted. It would express a
fact the composition root already decides when it chooses its descriptors, and
paying for it would mean adding a member to ADR 0002's closed
catalog-validation reason registry from outside that record.

**Why admit it now.** The asymmetry of costs decides it. Admitting costs one
origin member, one declaration gate, and one arm in a host's switch, all of them
statically unreachable in core release 1. Deferring changes an existing stage's
legal category set after hosts have written exhaustive switches, which section 2
classes as the amendment that may not leave existing packages source-compatible,
and it strands the descriptor field section 3 already mandates. Section 9
records top-level await as the JavaScript profile's expected requirement and
records WebAssembly as needing none: the counterweight confirms the transition
is optional per profile rather than imposed on every profile, which is what
makes admitting it cheap. Section 2's note that at least one amendment should be
planned for is not an argument for spending it here; ADR 0010
(`0010-embedding-decisions.md`) spends it where the contract genuinely cannot be
written blind.

## Bounds On A Paused Operation

Two bounds are mandatory at runtime creation. Both are **lifecycle** bounds and
neither is a sixteenth budget dimension: ADR 0007 closes the dimension set at
fifteen and requires every `resource exhaustion` result to name exactly one
dimension in exactly one scope, so expressing either of these as a dimension
would break that closure or misreport a lifecycle fact as a budget.

| Bound | Shape | Omission | Observed at | On expiry or refusal |
|---|---|---|---|---|
| `MaxSuspendedResidency` | monotonic duration, per operation, host-supplied, finite; there is no infinite value | runtime creation fails, reason `SuspendedResidencyUnbounded` | `VmRuntime.PollDeadlines()` and the next core entry point on any thread | the operation is latched cancelled and completes `cancellation`, reason `SuspendedResidencyExpired` |
| `MaxLiveSuspendedOperations` | count, per runtime, host-supplied, finite | runtime creation fails, reason `SuspendedOperationLimitUnbounded` | the moment an operation would park | the operation that would next suspend instead completes `invalid state`, reason `SuspendedOperationLimitReached` |

`MaxSuspendedResidency` exists because the wall clock is paused. ADR 0007 rules
that wall clock accrues only between entry into a core-owned call and its return
on that thread, under every suspension origin, so no clock is running to bound a
paused operation's retention. Charging wall clock while suspended was rejected
there and is not reopened here: it would make host inaction consume the guest's
allowance, and a parked sibling would drain a shared aggregate parent while
doing no work, making which sibling observes exhaustion depend on host
scheduling - a nondeterministic failure class section 14 blocks. A residency
bound attaches the obligation to the lifecycle, which is where the retention
actually lives, and the passive core can enforce it because it already owns
`PollDeadlines`.

`MaxLiveSuspendedOperations` refuses with `invalid state` rather than `resource
exhaustion` for the same closure reason: parking is not legal against a runtime
whose suspended set is already full, and reporting it as exhaustion would
require naming a dimension that does not exist. Unbounded accumulation is not an
option - invariant 9 forbids unbounded-by-omission and section 14 lists
unbounded retention as a blocking failure.

What a suspended operation retains, and the order in which a terminal transition
releases it, is ADR 0004's table and is not restated here. Two consequences of
it are load-bearing for this record: retained allocation and the live-runtime
count continue to be metered against the aggregate parent while an operation is
suspended, because the memory is genuinely held; and fuel, allocation, and
host-call meters are frozen, because no work runs.

## Rejected Alternatives

| Alternative | Why rejected |
|---|---|
| Suspend in place by blocking the executing thread inside a host callback | Blocks disposal on a resume that may never come, needs thread abort or a watchdog to be bounded, and turns external control into the privileged side channel invariant 12 and section 16 name as the risk. |
| A core watchdog that cancels a suspension after a configured timeout | Makes a breakpoint a race against a timer and results machine-dependent, and would have to be disabled for the debugging case it exists to serve. |
| An abandon or terminal-unwind callback with no budget | Reintroduces indefinite disposal blocking through the profile instead of through the host. |
| Keep `RequestResume` on the control handle | Two resume entry points mean two admission checks, a race, and two objects each claiming single use. `TryTakeSuspension` restores the transition invariant 12 requires while leaving exactly one resume path. |
| A `VmSuspension` that is reusable or transferable | Creates a use-after-resume class the lifecycle cannot bound and makes "never blocks disposal indefinitely" untestable. |
| A distinct diagnostic-client principal with its own permissions in the core | The core has no authentication, identity, or policy engine; a principal it cannot verify is a shape-only stub, which invariant 8 forbids from satisfying a capability gate. |
| Always-on external suspension for any profile that declares it | Grants every holder of a runtime reference the power to pause execution. Section 16 names the privileged side channel as the risk, and a composition must be able to say no. |
| A core-neutral frame, value, or source-position inspection API | Explicit section 1 non-goal, and the lowest-common-denominator core invariant 4 exists to prevent. |
| Two suspension categories, one guest and one external | Section 7 states that external suspension reuses `suspension` and adds no category; two categories would double every caller's handling for a property of the pause rather than of the outcome. |
| Excluding asynchronous instantiation from version 1 | Changes an existing stage's category set after hosts have shipped exhaustive switches, and strands the mandatory descriptor field section 3 already requires. It would also force the artifact-provider capability to become asynchronous, dragging an event loop into the core. |
| Modelling a suspended instantiation as an instance in a "not ready" sub-state | Hands out an instance handle whose every use is illegal, multiplying use-after-X cases behind a valid-looking object. |
| A `Task`-returning asynchronous instantiation entry point | Puts scheduling and async state machines into a Native AOT-static core and makes core behaviour depend on the host's synchronization context. |
| Letting a guest-initiated load suspend | A paused half-verified artifact could outlive its requesting operation, breaking the bounded-nesting guarantee in section 6 and VM-4's gate that an in-flight load leaves no partially verified state. ADR 0008 (`0008-guest-initiated-loads.md`) owns that exclusion. |

## Exclusions

Exclusion EX-20: core contract version 1 admits asynchronous instantiation and
no milestone gate requires anyone to demonstrate it. Reason: the two
demonstrations belong in VM-1's exit gate, and adding them there is a roadmap
amendment that VM-0 proposes and does not apply. Closed by: VM-1 discharging the
two obligations recorded in Consequences below.

Exclusion EX-21: no architecture rule asserts any transition, member, state or
category named in ADRs 0002 through 0011 - this record's transitions, members,
states, gates and bounds among them - beyond the two core contract version
constants ADR 0003 owns. Reason: the VM-0 product graph exports one static
class holding those two integer constants and nothing else, so no other decided
surface exists for a rule to range over; the register's remaining assertions
that reach the ADR files check their header fields and their index rows. Closed
by: the milestone at which each named surface exists and its drift assertions
become writable, beginning with VM-1 and the four this record lists in
Consequences.

Exclusion EX-22: `MaxSuspendedResidency` and `MaxLiveSuspendedOperations` are
mandatory at runtime creation on paper only. VM-0 fixes no value, no unit
granularity, and no enforcement, and nothing rejects an omitted bound. Reason:
runtime creation does not exist at VM-0. Closed by: VM-1.

Exclusion EX-23: core contract version 1 makes no wall-clock promise about how
long a pause request takes to park an operation. Reason: `RequestSuspend` is
observed at profile safepoints, so latency is a property of the profile's
safepoint density and of any host capability the operation is inside, neither of
which the core can bound. Closed by: a per-profile safepoint-density statement
in that profile's own documentation, and each composition's capability
blocking-behaviour claim at VM-6.

Exclusion EX-24: an untaken External-origin `VmSuspension` is latched abandoned
only when its control handle is explicitly disposed; a control handle merely
dropped is not observable to the core. Reason: no core object depends on a
finalizer for correctness (ADR 0004), so the core cannot see that a host stopped
holding a reference. Closed by: `MaxSuspendedResidency` expiry, which bounds the
dropped-handle case without the drop needing to be observed; no further work is
owed.

## Consequences

- **VM-1 must discharge two obligations that no gate currently names.** (a) The
  fixture profile suspends during instantiation and resumes to a live instance,
  and separately is cancelled and disposed while suspended without ever being
  resumed, with no instance published on that path. (b) A catalog entry that
  does not declare asynchronous instantiation, whose profile nevertheless
  completes instantiation with `suspension`, is abandoned through the profile's
  bounded abandon entry point and reports `invalid state` with reason
  `UndeclaredAsynchronousInstantiation`. These are recorded here as obligations
  VM-1 must discharge. Writing them into VM-1's exit gate or into section 14
  requires a roadmap amendment that ADR 0003
  (`0003-core-contract-v1-and-amendments.md`) proposes and does not apply; this
  record does not state, and no reader may infer, that VM-1's gate, section 13's
  milestone list, section 13's delivery order, or section 14's rows have
  changed. Exclusion EX-20 records the gap in the meantime.
- **External suspension, by contrast, is already gated.** VM-1's exit gate as
  written requires the fixture profile to exercise external suspension and
  resume, and VM-4's requires an externally suspended operation to resume,
  cancel, or dispose deterministically and to never block disposal
  indefinitely, including a client that abandons a paused operation. This record
  adds no condition to either; it fixes what those conditions mean.
- **ADR 0003 carries two rows for this record in its admitted-versus-implemented
  table**, and both discharge invariant 8 by a named deterministic failure the
  shipping core returns rather than by absence from the public surface: external
  suspension that is not declared or not enabled returns
  `VmControlResult.Unsupported` with the naming diagnostic reason, and an
  undeclared asynchronous instantiation returns `invalid state` with reason
  `UndeclaredAsynchronousInstantiation`. ADR 0003 also carries
  `TryTakeSuspension` and `ExternalSuspensionAbandoned` in the public-name
  table.
- **The roadmap's section 7 run-time sentence "no runtime may be created or
  resumed once the parent has no remaining allowance" names a transition that
  does not exist**, because runtimes have no suspended state - only operations
  do. Resume admission against an exhausted aggregate parent is the transition
  that carries the obligation, and it is settled by ADR 0007. The corrected
  wording is a row in ADR 0003's amendment register, marked proposed and not
  applied; this record does not edit the roadmap.
- **A profile must be able to capture and reconstitute its own continuation** to
  declare guest suspension, external suspension, or asynchronous instantiation.
  A profile whose executor cannot unwind simply declares none of the three, and
  that refusal is truthful, named, and visible in its descriptor. ADR 0011
  (`0011-source-level-profile-contract.md`) carries it as an item on the
  profile-facing checklist; it is a real constraint on any future debugger
  design and is cheaper to state now than to discover in a profile port.
- **Because no thread is held, thread affinity constrains only resume, cancel
  with guest unwinding, and dispose** - not the mere existence of a suspended
  operation. ADR 0004 owns the affinity table.
- **External-suspension support is a property of a composition, not of a core
  release**, since the second gate is per runtime. A future public support table
  must therefore state it per composition. ADR 0012
  (`0012-security-ownership-and-support-matrix.md`) records that no public
  support table exists at VM-0 and that section 15 gate 1 is unmet.
- **VM-1 gains four drift assertions that cannot be written today** (EX-21):
  `VmSuspensionOrigin` has exactly the members `Guest`, `External`,
  `Instantiation`; `VmOperationControlHandle` has exactly the four members in
  this record's table; no public member in the product graph returns `Task`,
  `Task<T>`, `ValueTask`, `ValueTask<T>`, or a custom awaitable, and no product
  type implements `INotifyCompletion`; and no product assembly references a
  timer, delay, or thread-abort API. Each becomes a register row at its
  activation milestone, not before.
- **A host that switches over instantiation outcomes must handle `suspension`
  from version 1 onward**, even though no core release 1 composition can produce
  one. That is the deliberate price of admitting the transition: the arm is
  written once, cheaply, instead of being added under an amendment after hosts
  have shipped.
