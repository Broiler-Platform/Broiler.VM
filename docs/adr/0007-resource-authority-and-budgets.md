# ADR 0007 - Resource Authority, Precedence, And Aggregate Budgets

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

## Context

Roadmap invariant 9 is the only invariant that names three authorities - host,
profile, artifact - orders them, and then adds a composition clause, without
ever saying what the arithmetic is:

> **Resource authority is trusted and monotonic.** At runtime creation the host
> supplies explicit ceilings or explicitly adopts bounded profile defaults;
> omission never means unbounded. Each profile may impose a stricter hard
> maximum, and artifact declarations may only request lower limits.
> Verification fixes their intersection as the handle's
> verification/instantiation ceilings before an untrusted allocation. Instance
> or invocation budgets may tighten those ceilings or allocate a remaining
> fuel/time allowance; they never raise them without producing a newly verified
> handle. Ceilings also compose: a host may create runtimes under one shared
> aggregate budget, a per-runtime ceiling may never exceed the parent's
> remaining allowance, and creating more runtimes may not multiply a host
> maximum.

Two implementers reading that sentence produce two different effective policies
from the same inputs, because it fixes directions and not an order. Section 7's
load-time and run-time requirements and section 6's guest-initiated-load bullets
each name further bounds without saying whether they are the same kind of thing.
Section 13's VM-0 next action requires a recorded decision on "whether aggregate
budgets are a core object or a host responsibility". This record settles the
closed dimension set, the ordered computation, what tighten-only means, and the
aggregate object.

**Existence of everything named here.** Two checked-in files are named: the
contract-version constants (exists at VM-0:
`src/Broiler.VM.Abstractions/VmCoreContract.cs`) and the architecture-rule
register (exists at VM-0:
`src/tests/Broiler.VM.Architecture.Tests/rules.register.json`). Every type,
enumeration, member, table, matrix, reason identifier and diagnostic identifier
below is (VM-0 decision on paper; no file at VM-0); the milestone that builds
each is named where it is decided. The three product shells export exactly one
public type between them, `VmCoreContract`, so at VM-0 there is no budget
surface for an architecture rule to inspect, and the register contains no rule
owned by this record. The public names below are the frozen ones published by
ADR 0003 (`0003-core-contract-v1-and-amendments.md`); this record invents none.

## The fifteen budgeted dimensions

Core contract version 1 defines exactly fifteen budgeted dimensions. The set is
closed: adding, removing, splitting, renaming or reclassifying one is a numbered
core contract amendment under section 2, never a profile-specific extension. A
profile declares nothing but whether a dimension applies to it.

An **allowance** is a non-negative counter allocated when its scope opens and
decremented as work is charged; reaching zero fails the charging operation with
`ResourceExhaustion`. An allowance is never replenished, refunded or reset
inside its scope. A **ceiling** is a scalar maximum frozen by the scope that
sets it and compared against an instantaneous or per-artifact measure; it is not
consumed, and it falls only when the measured quantity itself falls - a frame
returns, a nested load unwinds, a runtime is disposed.

The enumeration below is the frozen order. It is both the declaration order and
the tie-break order used when several dimensions are exhausted at one
observation point. `Declared at` lists the scopes that may carry a value for the
dimension; `Aggregate` is decided in the next section.

| # | Dimension | Class | What it measures | Declared at | Aggregate |
|---|---|---|---|---|---|
| 1 | `Fuel` | Allowance | profile-charged abstract execution work units | Runtime, Instance, Invocation, Aggregate | yes |
| 2 | `WallClock` | Allowance | attributed execution time | Runtime, Instance, Invocation, Aggregate | yes |
| 3 | `AllocatedBytes` | Allowance | cumulative attributed bytes allocated | Runtime, Instance, Invocation, Aggregate | yes |
| 4 | `HostCalls` | Allowance | host-capability invocations | Runtime, Instance, Invocation, Aggregate | yes |
| 5 | `NestedLoadFanOut` | Allowance | artifact-provider requests admitted for one operation | Runtime, Instance, Invocation, Aggregate | yes |
| 6 | `NestedLoadBytes` | Allowance | provider-returned bytes for one operation | Runtime, Instance, Invocation, Aggregate | yes |
| 7 | `VerifierWork` | Allowance | verifier work units for one verification, including every nested verification charged to the same operation | Runtime, Artifact, Aggregate | yes |
| 8 | `LiveBytes` | Ceiling | attributed live (retained) bytes | Runtime, Instance, Aggregate | yes |
| 9 | `CallDepth` | Ceiling | call/frame depth | Runtime, Instance, Invocation, Aggregate | yes |
| 10 | `NestedLoadDepth` | Ceiling | provider-mediated nesting depth | Runtime, Instance, Invocation, Aggregate | yes |
| 11 | `ArtifactBytes` | Ceiling | bytes of one artifact presented to one verification | Runtime, Artifact | no |
| 12 | `SectionCount` | Ceiling | top-level framed units one artifact may declare | Runtime, Artifact | no |
| 13 | `DeclaredCount` | Ceiling | the greatest value any single untrusted declared count, length, index or offset may hold before it may size an allocation | Runtime, Artifact | no |
| 14 | `StructuralDepth` | Ceiling | framing nesting depth inside one artifact | Runtime, Artifact | no |
| 15 | `LiveRuntimes` | Ceiling | concurrently live runtimes under one aggregate budget | Aggregate | yes |

Three rulings elsewhere in this record depend on definitions that must be fixed
here rather than left to VM-1:

- `WallClock` accrues only as monotonic elapsed time between entry into a
  core-owned operation call - verify, instantiate, invoke, resume - and its
  return on that thread. It is **not** a real-time deadline and it does not
  accrue while an operation is suspended, under any suspension origin. A host
  that needs a real-time deadline gets no core support and enforces it in its
  own event loop (section 11). This is a deliberate non-goal, not an omission.
- `LiveBytes` is enforced only over attributed allocation: bytes allocated
  through the core's bounded-allocation primitive plus bytes a profile
  explicitly reports retained and released. It is not a process, GC-heap or
  resident-set limit, and the public support table (deferred to VM-6) must say
  so.
- Section 7's load-time phrase "constants, metadata" names no further core
  dimensions. Both are profile-format applications of `DeclaredCount`, checked
  through the bounded-allocation guard in `Broiler.VM.Binary` (shell exists at
  VM-0: `src/Broiler.VM.Binary/Broiler.VM.Binary.csproj`; the primitives are
  deferred to VM-1). The core owns the guard; the profile owns which counts pass
  through it. A core dimension whose definition named a language concept -
  constants, strings, objects, opcodes - would put language structure into a
  semantics-neutral contract, against invariant 4.

`Allocation`, `HostCallCount` and `LiveRuntimeCount` are struck as names; they
were an earlier eleven-member spelling of this set and are never used.

## Which dimensions carry aggregate scope, and why

The governing test is stated once and is the reason, not a list:

> A dimension carries aggregate scope if and only if its measure is summable
> across concurrently live runtimes under one parent - cumulatively for an
> allowance, instantaneously for a ceiling.

Eleven dimensions pass it: all seven allowances, plus the four live-sum ceilings
`LiveBytes`, `CallDepth`, `NestedLoadDepth` and `LiveRuntimes`. For an aggregate
ceiling the parent's measure is the instantaneous **sum** across its children,
never the maximum: aggregate `CallDepth` bounds total live frame depth under the
parent, which is what makes it a bound on stack memory rather than a per-runtime
style rule.

Four dimensions do not carry aggregate scope, and need none. Each bounds the
shape of one untrusted input at one observation point, so a second runtime
raises no maximum:

- `ArtifactBytes` bounds one artifact presented to one verification; a second
  runtime does not make any single artifact larger.
- `SectionCount` bounds the sections one artifact declares; a second runtime
  does not add a section to any artifact.
- `DeclaredCount` bounds the value one untrusted count may hold before it may
  size an allocation; a second runtime does not raise that value.
- `StructuralDepth` bounds framing depth inside one artifact; a second runtime
  does not deepen any artifact's framing.

Their only summable consequence is the memory held while N verifications run
concurrently, and that is metered by aggregate `AllocatedBytes` and `LiveBytes`,
which are aggregate-scoped. Invariant 9's clause that creating more runtimes may
not multiply a host maximum therefore holds unqualified: for every dimension
whose measure can be multiplied by creating runtimes, a parent meter exists.

`LiveRuntimes` is the one dimension with no runtime, instance, invocation or
artifact scope. Because the runtime-creation options carry a slot for each of
the fifteen (ADR 0004, `0004-lifecycle-and-state-machine.md`), the slot exists
and must be filled: the only legal runtime-scope entry for `LiveRuntimes` is the
`AdoptParentRemaining` marker, and any other entry fails runtime creation with
reason `BudgetDimensionNotRuntimeScoped`. Where no parent is present that marker
resolves to TOP, because a single unparented runtime cannot multiply anything.
This is the only dimension for which TOP is a legal runtime-scope resolution,
and it is still not an omission: the host writes the marker explicitly.

## Scope, and what a resource-exhaustion result names

`VmBudgetScope` is closed at five members.

| Scope | Meaning |
|---|---|
| `Invocation` | one invoke or resume of one operation |
| `Instance` | one instantiated instance and every operation on it |
| `Artifact` | the verified handle's effective verification/instantiation ceilings |
| `Runtime` | one runtime |
| `Aggregate` | the shared parent |

Every `ResourceExhaustion` result names exactly one dimension and exactly one
scope. When several are exhausted at one observation point the result is
deterministic by two rules: report the **outermost** scope first - `Aggregate`,
then `Runtime`, then `Artifact`, then `Instance`, then `Invocation` - and within
a scope the first dimension in the frozen order above. Reporting the outermost
exhausted scope is what makes the result actionable: a host whose shared
aggregate allowance is spent must not be told that its invocation ran out of
fuel, because no retry of that invocation can succeed.

A guest-initiated load adds no scope. Work it performs is charged to the
requesting operation, so exhaustion inside a nested load reports scope
`Invocation` and carries the requesting operation identity and the nesting depth
in diagnostics - which is what makes section 6's "charged to the requesting
operation" auditable from the result alone. ADR 0008
(`0008-guest-initiated-loads.md`) owns the nested bound set and its charging
rule; this record owns the dimensions it charges into.

`ResourceExhaustion` is one category among the closed outcome set owned by ADR
0005 (`0005-operation-result-envelope.md`), which also owns the outcome
precedence order. Two consequences of that order belong here: `Cancellation`
outranks `ResourceExhaustion`, so an operation that is cancelled while over
budget reports cancellation; and `WallClock` exhaustion is a resource exhaustion
and never a cancellation, because a limit the host configured and an intent the
host expressed are different facts and a test that cannot tell them apart cannot
prove a bound was enforced.

**Ceilings are not disclosed downward.** The host-facing result carries the
dimension, the scope, the consumption and the effective ceiling for that scope -
disclosing nothing the host did not itself set or cannot read from the verified
handle. The profile-facing signal is the dimension and the scope alone, with no
quantities. The profile-facing metering surface is exactly two members,
`TryCharge(dimension, amount)` returning a boolean and `Poll()`; there is no
non-consuming remaining-allowance reader on it, so a profile - and through it
guest code - cannot binary-search a host ceiling without spending it. Remaining
values are readable on the host-facing budget snapshot only. This narrows the
earlier proposal of profile-facing remaining readers, which is recorded here as
amended rather than re-litigated.

## The precedence algorithm

The effective policy is computed per dimension by the ordered algorithm below.
`TOP` means "this layer contributes no constraint", and `min(x, TOP) = x`. All
values are non-negative finite integers.

| Step | Layer | Produces | Inputs it may read | Meaning of omission | Failure |
|---|---|---|---|---|---|
| P0 | Catalog | `ProfileMax(d)`, `ProfileDefault(d)` | the immutable profile descriptor | `ProfileMax` may be TOP; a profile need not constrain a dimension | a descriptor with `ProfileDefault(d) > ProfileMax(d)` is rejected at catalog construction (ADR 0002, `0002-profile-identity-and-static-catalog.md`) |
| P1 | Runtime creation | `RuntimeCeiling(d)` | the host's explicit value, or the `AdoptProfileDefault(d)` marker, or the `AdoptParentRemaining(d)` marker | illegal; there is no core default | `HostFailure`, reason `BudgetDimensionUnresolved`, naming d |
| P2 | Verification | `HandleCeiling(d) = min(RuntimeCeiling(d), ProfileMax(d), ArtifactRequest(d))` | the immutable artifact descriptor only | an omitted artifact request is TOP: it adds no restriction and removes none | a request above the intersection is clamped, with diagnostic `LimitRequestClamped(d, requested, effective)` |
| P3 | Instantiation | `InstanceCeiling(d)`, `InstanceAllowance(d)` | the instance override | an omitted override is TOP: inherit | a raising override is refused, `HostFailure`, reason `BudgetRaiseRefused` |
| P4 | Invocation and resume | the operation's values | the invocation override | an omitted override is TOP: inherit | as P3; a resume under an exhausted parent is `ResourceExhaustion`, reason `ParentExhausted` |
| P5 | Charging | decremented meters | `TryCharge(d, n)` | not applicable | `ResourceExhaustion` naming d, the innermost failing link and that scope's identity |

At P1, if the host supplied neither a value nor a marker, or named a profile
default the descriptor does not carry, runtime creation fails and no runtime
exists. **The core supplies no default of its own**, for any dimension, ever.
That is the operative reading of "omission never means unbounded": an ambient
core default would make silence mean something the host did not write, and would
hide a composition mistake behind a value nobody chose. `RuntimeCeiling` is then
frozen for the life of the runtime and is never recomputed.

Where a parent is present, P1 also performs the aggregate admission checks over
the eleven aggregate dimensions: creation fails `ResourceExhaustion` with
reason `ParentExhausted` when any aggregate allowance is already spent,
`LiveRuntimeCeilingReached` when the parent is at its `LiveRuntimes` ceiling,
and `ExceedsParentRemaining` - naming the dimension, the requested value and
the remainder - when `RuntimeCeiling(d) > ParentRemaining(d)` and the host did
not pass `AdoptParentRemaining(d)`. `ParentRemaining(d)` is the unspent
allowance for an aggregate allowance and the headroom between the parent's
ceiling and its current live measure for an aggregate ceiling.

At P2, `ArtifactRequest(d)` is read **only** from the immutable artifact
descriptor and never from the payload, because a limit read out of the payload
would require reading untrusted bytes before a policy exists. Limits encoded
inside a payload are profile-owned self-restrictions applied under the
already-frozen policy; they are not inputs to this computation. Materialization
completes before (a) the first read of any payload byte, (b) any allocation
sized by payload data, and (c) entry into the profile verifier. The frozen
policy is passed to the verifier as an immutable value and is a **required
parameter** of every bounded reader and bounded-allocation primitive in
`Broiler.VM.Binary`, so "allocate before materializing" fails to compile rather
than being asserted at run time. That is the strongest available form of
section 7's rule that effective limits are computed before reading or allocating
from an untrusted declared count.

For a nested (guest-initiated) verification, `RuntimeCeiling(d)` in P2 is
replaced by the requesting operation's remaining allowance for allowance
dimensions and by its effective ceiling for ceiling dimensions. A nested load
can exhaust an invocation; it can never enlarge one. ADR 0008 owns the resulting
handle's origin flag and its non-shareability.

At P3 each allowance's mode is fixed and never changes afterwards: `Pooled` -
one monotonically decreasing counter shared by every operation in the scope - or
`Replenishing` - a fresh allowance per operation. An override on a `Pooled`
dimension creates a per-operation sub-cap drawn from the pool, and each charge
decrements both.

At P5 the charge walks the meter chain innermost to outermost - operation,
instance, runtime, aggregate parent - at most four links, because aggregate
budgets do not nest. Every link is checked, then every link is applied. If any
link would go below zero, **none** is applied and the operation returns
`ResourceExhaustion`. Partial application would debit a sibling runtime for work
that never happened, so the parent meter would drift and the failure would not
reproduce. Control operations - dispose, cancel, request-suspend, poll-deadlines
and lease acquire and release, which ADR 0005 rules are not envelope-bearing
stages - charge nothing and are never refused for exhaustion; an exhausted
budget that made cleanup impossible would produce exactly the unbounded
retention section 14 blocks.

## Monotonicity, and what raising a ceiling costs

Monotonicity is stated as two checkable properties, and they are the oracle
VM-2's limit suite is written against:

- **M1.** For every dimension d and every scope s with enclosing scope p,
  `Effective(s, d) <= Effective(p, d)` at the moment s is opened.
- **M2.** Every live meter is non-increasing over its lifetime, except that a
  ceiling measure falls when the measured quantity falls. No core operation ever
  increases any live meter.

Tighten-only is precise for both classes. For a ceiling, an override sets
`min(inherited, override)`. For an allowance, an override sets the operation's
initial allowance to `min(inherited-or-pool-remaining, override)`; it can never
top up a partially consumed pooled meter.

The asymmetry between the two kinds of input is the rule to remember:

| Input | Speaker | Treatment when it asks for more |
|---|---|---|
| An artifact-requested limit (P2) | outside the trust boundary | clamped to the intersection, with a diagnostic; the artifact is not rejected |
| A host instance or invocation override (P3, P4) | inside the trust boundary | refused, `HostFailure`, reason `BudgetRaiseRefused`, naming the dimension, the inherited value, the requested value and the remedy |

Rejecting an over-requesting artifact would convert a request into a
requirement, so the same safe bytes would fail on a tighter host even when
nothing in them needs the larger limit - which contradicts section 6 and makes
the descriptor an unintended compatibility surface. Silently clamping a host
override would discard an instruction from trusted code, producing a runtime
that dies mysteriously under load with the blame attached to the artifact.
`BudgetRaiseRefused` is emphatically **not** `ResourceExhaustion`: nothing was
exhausted, and misreporting a composition defect as exhaustion is the same
diagnostic error section 7 rejects when it separates an unsupported profile from
an invalid artifact. It must never be mapped into an exhaustion metric.

**Raising a ceiling requires a newly verified handle.** The only way to obtain
a higher `HandleCeiling(d)` is a fresh verification of the artifact under a
runtime whose `RuntimeCeiling(d)` permits it. There is no re-policy, re-bind,
re-open, upgrade, widen or clone-with-larger-limits operation on an existing
handle, and none may be added without a numbered amendment. A derived handle
would carry a ceiling its verification never established - a larger structure
that the old policy rejected mid-parse was never validated - which is precisely
the "policy raises a verified ceiling" failure section 14 makes a release
blocker. Because effective ceilings are part of verified-handle identity (ADR
0006, `0006-verified-artifact-ownership.md`), the re-verified handle has a
different identity from the old one: the two cannot be confused, and a handle
may be presented to a second runtime only where identities match.

No member on any budget, handle, instance or runtime type is named `Grant`,
`Refund`, `Reset`, `Extend`, `Increase`, `Raise`, `TopUp`, `Widen`, `Reopen`,
`WithLimits`, `Withdraw` or `Credit`. Invariant 9's "cannot enlarge" is thereby
a property of the API surface rather than of profile discipline. The charge
amount is an unsigned integral type, so a negative charge is not expressible.

**Effective ceilings are in-process identity only.** They are part of the
in-process verified-handle identity and of the in-process cache key; they are
not part of the persisted-envelope key, whose closed content ADR 0006 defines
and which contains no remainder-derived quantity. A handle whose artifact
origin is `GuestInitiated` is ineligible for any persisted envelope. Loading a
persisted artifact re-verifies it and recomputes effective ceilings against the
loading runtime, so a persisted artifact never carries a ceiling decision
forward, and invariant 5's rule that persisted artifacts contain no
process-local identities holds by construction.

## The shared aggregate budget is a core object

`VmAggregateBudget` (VM-0 decision on paper; no file at VM-0) is a first-class,
host-created, host-owned, explicitly disposed core type passed to runtime
creation. It cannot be a host responsibility, because the quantities it meters -
fuel charged inside an execution loop, attributed allocation, attributed
execution time - are observable only inside the core, so a host has no hook to
see them and therefore no way to honour invariant 9's composition clause. No
host-supplied allowance accessor is consulted on any admission path: a host
callback on the runtime-creation or resume path would be a capability invocation
on a lifecycle path, which the passive-core rule forbids.

It declares the eleven aggregate dimensions of the table above and no others.
**Aggregate budgets do not nest in contract version 1**: a runtime has zero or
one parent and a parent has no parent, so the meter chain is at most four links
deep. Deeper trees - nested worker-style agents, section 9 - are a named
amendment driver, recorded here so the deferral is a decision rather than a
discovery.

It is **pay-as-you-go, not reservation**. Creating a runtime reserves nothing;
every charge decrements the operation, instance, runtime and parent meters
together. That is exactly what stops N runtimes from multiplying a host ceiling:
N children under a 100-unit parent can together spend 100 units and no more,
whatever their individual ceilings say. Reservation was rejected because ten
idle runtimes with generous ceilings would exhaust a parent that has spent
nothing, and because section 7 phrases the creation refusal in terms of the
parent having no remaining allowance, which is spending language.

Contract version 1 guarantees four properties of it, and any mechanism that
cannot deliver all four is out of scope for version 1:

| # | Guarantee |
|---|---|
| G1 | A per-runtime ceiling may never exceed the parent's remaining allowance at creation, computed as part of the ordinary intersection before any untrusted allocation. |
| G2 | Every aggregate dimension is metered against the parent as well as against each runtime, and a suspended operation's retained allocation and live count continue to be metered. |
| G3 | Exhausting the parent is reported as `ResourceExhaustion` to whichever operation observes it, adding no category. |
| G4 | Once the parent has no remaining allowance, no runtime may be created and no operation may be resumed. |

G4 fixes a reading section 7 leaves open: only operations suspend, so "no
runtime may be created or resumed" is read as **no runtime created and no
operation resumed**. Resume admission is a core lifecycle transition, so the
refusal is checked at admission, before any profile continuation runs, and is
therefore deterministic. ADR 0003's amendment register carries the corresponding
roadmap sentence as a proposed, not applied row.

**Exhaustion does not kill siblings.** The parent refuses new work; it never
terminates a runtime. Live operations fail at their next charge or poll;
completed work is unaffected. Terminating children would give a budget a
cross-runtime blast radius and a shutdown protocol, which is scheduler
behaviour. Disposing a parent that still has live child runtimes is
`InvalidState`; the host disposes children first, because force-termination
would require the core to unwind profile frames it does not own. A host that
wants drain-then-teardown calls the explicit non-blocking `Seal()`, after which
no runtime may be created under the parent and no operation may be resumed,
while in-flight operations run to completion.

**Allowances never refund**, including on dispose: spent is spent.
`LiveRuntimes` is a ceiling on a live count and decrements on dispose. That
contrast is the clearest illustration of the ceiling/allowance split and the
public support table must state it.

**It is an accounting object, not a scheduler.** The dividing line is: a budget
refuses; a scheduler defers. It has no queue, no fairness policy, no admission
ordering, no priority, no preemption, no thread, no timer and no waiting. Every
interaction is a non-blocking interlocked check-then-apply, or a non-blocking
check at creation and at resume admission. An exhausted meter fails an
operation; it never blocks, yields, queues or retries it.

Two narrowings are recorded rather than hidden, because both bend a rule
elsewhere in the roadmap:

- The aggregate budget is the **single named exception** to section 7's rule
  that concurrent runtimes share only immutable verified artifacts by default.
  It carries counters only and holds no profile-owned or host-owned values, and
  the sharing is *declared*, not inferred from a shared registry: each runtime
  records its parent's `AggregateBudgetId` in runtime identity, and the VM-4
  stress-evidence obligation that section 7 attaches to declared mutable sharing
  attaches to it. Two runtimes under one parent can observe each other through
  exhaustion and timing; a host placing mutually untrusted guests under one
  parent accepts that channel, and a host requiring isolation must not share a
  parent. The public support table must say so.
- For shared-parent exhaustion the failure **category**, the named **dimension**
  and the named exhausted **scope** are deterministic; the **victim operation is
  not**, and no test may assert it. Section 7 already says "to whichever
  operation observes it". Promising a deterministic victim would promise a
  scheduler.

Aggregate `WallClock` is summed attributed execution time across children -
additive and CPU-time-like, not real elapsed time. Summation is what makes the
anti-multiplication property hold; real elapsed time would let one slow child
kill unrelated siblings for reasons outside their control.

## Metering is mandatory, and applicability is declared

Metering is mandatory at the contract level. There is no opt-in, no unmetered
mode, no trusted-profile bypass and no configuration that disables a meter; an
opt-in meter is omission meaning unbounded with an extra step. What is
negotiable is only whether a dimension **applies** to a profile, and that must
be declared rather than defaulted.

Every profile descriptor declares, for each of the fifteen dimensions, exactly
one of `Charged` or `NotApplicable`. There is no third value and no default; a
descriptor that omits a row is rejected at catalog construction alongside the
rejections ADR 0002 already requires. `NotApplicable` is a claim with a
structural consequence the core enforces. `HostCalls` not applicable means no
host capability may be bound to that profile's runtimes and any invocation
attempt is `HostFailure`, reason `CapabilityNotRegistered`. `NestedLoadDepth`
not applicable means no artifact-provider capability may be bound and every
guest-initiated load is refused deterministically (ADR 0008). `VerifierWork`
not applicable is illegal, because verification always does work.

The core meters what it owns, whatever the profile does. The split must be
enumerated rather than left implicit, because it is exactly the blast radius of
a non-charging profile:

| Metering | Dimensions |
|---|---|
| Core-metered and unevadable | `WallClock`, `HostCalls`, `NestedLoadFanOut`, `NestedLoadBytes`, `NestedLoadDepth`, `ArtifactBytes`, `LiveRuntimes` |
| Core-metered when routed through `Broiler.VM.Binary`, which the profile is obliged to route through | `DeclaredCount`, `SectionCount`, `StructuralDepth`, and the guarded share of `AllocatedBytes` and `VerifierWork` |
| Profile-charged, and therefore only ever an obligation | `Fuel`, `CallDepth`, `LiveBytes`, and the share of `AllocatedBytes` and `VerifierWork` a profile allocates or consumes outside the core primitives |

**The trust frame is stated plainly.** A profile is statically composed into
the image and is therefore trusted code. Budget metering protects the host from
untrusted **artifacts**, not from profiles. A profile that does not charge is a
defect, not an attack, and the core's obligation is to make that defect
declared, detectable and named - not to sandbox the profile. Any claim that the
core contains or sandboxes a statically composed profile would be untruthful
under invariant 8 and must not appear in any support claim.

**Preemption is cooperative, and recorded as such.** The core cannot interrupt
a running profile loop; it can only refuse to start work and refuse to continue
at boundaries it owns. The poll contract is therefore part of contract version
1: the profile calls `Poll()` - one combined budget and cancellation check - at
least once per loop back-edge, per call entry, per host-capability return and
per provider return, and its descriptor declares `MaxUnchargedWork`, the bound
on work performed between two polls. At every boundary the core owns, it
compares charged work against attributed elapsed work; a profile exceeding its
declared `MaxUnchargedWork` is reported as `InvalidState` with reason
`ProfileContractViolation`, and the runtime is **poisoned** - it accepts no
further operation and only disposal - because once the metering contract is
broken the core's isolation assumptions no longer hold. This detection is
best-effort evidence of a defect and is **not** a security boundary.
Cancellation latency is bounded in profile work units, not in wall-clock time,
through the descriptor's `CancellationPollBound`, which ADR 0004 owns and which
is enforced at this meter.

## The five profile charging obligations

Section 7's sentence that variable-work operations charge proportional work
rather than one nominal instruction has no mechanism attached to it, and
because a profile is trusted, statically composed code, proportionality cannot
be enforced. The honest form is a numbered obligation with declared, reviewable
parameters, and an explicit statement of which obligations the core cannot
check.

| ID | Obligation | Core can verify it? |
|---|---|---|
| CO-1 | For any operation whose worst-case cost is a function of an input magnitude n - concatenation, comparison, sort, regular-expression match, memory copy, table growth, arbitrary-precision arithmetic, collection materialization, structural equality - charge `Fuel` as a monotone non-decreasing function of n, at least `ceil(f(n)/g)` units for the profile's declared granularity g, expressed in the profile's own work units and never in measured time. | No |
| CO-2 | Any single charge larger than the declared `MaxUnchargedWork` is charged **before** the work is performed, so exhaustion fails without doing the work. An over-estimate followed by no refund is permitted; an under-estimate with a later top-up is not. | No |
| CO-3 | Every allocation whose size derives from untrusted data is made through the core's bounded-allocation primitive, which charges `AllocatedBytes` and enforces `DeclaredCount`. | Partially: for allocations that go through it, and not for those that do not |
| CO-4 | At least one `Poll()` per loop back-edge, per call entry, per host-capability return and per provider return, with work between polls bounded by `MaxUnchargedWork`. | Not in general; detectable at core-owned boundaries |
| CO-5 | The profile-facing metering surface is exactly `TryCharge` and `Poll`. There is no grant, refund, reset, extend or withdraw, and a negative or zero-with-work charge is rejected. | Yes - structurally, from the member list and the parameter type |

Charging proportional to measured elapsed time was rejected outright: it makes
the same artifact fail differently on two machines and destroys the
deterministic failure classes section 14 requires. Deriving the charge in the
core was rejected because it would require the core to understand what the
arguments mean, which invariant 4 forbids. A core-imposed universal cost model
was rejected as the lowest-common-denominator semantics invariant 4 exists to
prevent.

Because the core is asking for obligations it cannot check, it owes five things
in return, all part of contract version 1: publish CO-1 to CO-5 as citable
clauses; require the descriptor to declare `MaxUnchargedWork`, the charging
granularity g and the fifteen-row applicability matrix, so the claims are
reviewable and drift-checked; detect and report `MaxUnchargedWork` violations at
core-owned boundaries; ship a proportionality fixture in the fixture profile
(deferred to VM-1) so profile authors are not left to invent the test shape and
invariant 13 holds; and state in the public support table that **fuel units are
profile-scoped and carry no cross-profile meaning** - a fuel figure from one
profile is not comparable with another's, and no core measurement implies a
language performance claim.

## Budget accounting across a suspension

| Meter | While an operation is suspended |
|---|---|
| `Fuel`, `AllocatedBytes`, `HostCalls`, nested-load counters | frozen; no work runs, so nothing is charged |
| `WallClock` | paused, under every suspension origin - `Guest`, `External` and `Instantiation` |
| `LiveBytes`, `LiveRuntimes`, and their aggregate sums | continue to be metered; the memory and the runtime are genuinely held |

`WallClock` pauses under every origin because it is metered against the shared
aggregate parent. If it accrued while suspended, a parked sibling would drain
the parent while doing no work, and which sibling observes exhaustion would
depend on host scheduling - a nondeterministic failure class section 14 blocks.
It would also make host inaction consume a guest's budget, when the host
decides when to resume, and would let a breakpoint deterministically kill the
operation being debugged, turning the lifecycle state invariant 12 declares
into a resource attack.

The earlier proposal of an origin-dependent clock and a
`WallClockPausesWhileSuspended` runtime option is struck: a composition-level
switch that changes an operation's failure class is precisely the second
execution mode the roadmap's stop conditions reject.

The parking risk that proposal was answering is real and is answered elsewhere:
`MaxSuspendedResidency` is a mandatory finite lifecycle bound on runtime
creation, owned by ADR 0009
(`0009-external-suspension-and-async-instantiation.md`). It is deliberately
**not** a sixteenth dimension - it bounds residency, not consumption, and
admitting it as a dimension would break the rule that an exhaustion result names
exactly one dimension and one scope.

## What VM-0 does not enforce

Recorded in plain words, because a shell that appears to prove one of these
would be the untruthful support claim section 16 makes a stop condition:

- **No architecture rule asserts any decision in this record.** The register at
  `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` holds 28 rules,
  all owned by ADR 0001 (`0001-component-topology-and-dependency-graph.md`) and
  ADR 0003; none of them has a budget subject, because at VM-0 the product
  graph exports only `VmCoreContract`. The drift tests this record's rulings
  imply have no subject until VM-1 and VM-2 create one: the fifteen-member
  enumeration, the five scopes, the applicability matrix, the metering member
  list and the forbidden member names. Closed by: those milestones registering
  the rules as they build their subjects, under the register's own status
  rules.
- **The core cannot verify CO-1, CO-2 or CO-4, and verifies CO-3 only for
  allocations routed through its own primitive.** Composition review and the
  exact-closure reports at VM-3 and VM-6 are the compensating control. Closed
  by: nothing at VM-0; the obligation is contractual, not enforced.
- **The victim operation of a shared-parent exhaustion is not deterministic.**
  Only the category, the dimension and the scope are. Closed by: nothing; a
  deterministic victim would require a scheduler.
- **Core contract version 1 makes no wall-clock cancellation-latency promise.**
  A composition whose host capabilities declare themselves non-cancellable and
  blocking truthfully has unbounded latency; that is a composition property and
  never a core claim.
- **An aggregate budget is by construction a cross-runtime information
  channel.** Nothing at VM-0 or later removes it; hosts requiring isolation
  must not share a parent.

## Consequences

- **Section 7's aggregate paragraph is superseded where it names four metered
  quantities.** Eleven dimensions carry aggregate scope, not four, because the
  four-dimension reading leaves ten dimensions in which two runtimes each
  receive the full host maximum, and invariant 9 says creating more runtimes
  may not multiply a host maximum. The same widening supersedes the matching
  mitigation row in section 16. Both are recorded as proposed, not applied rows
  in ADR 0003's amendment register; this record does not edit the roadmap.
- **Section 7's "no runtime may be created or resumed" is superseded by G4's
  reading** - no runtime created and no operation resumed - and carried as a
  further proposed, not applied register row in ADR 0003.
- **Section 7's load-time list is read, not amended**: "constants, metadata"
  are applications of `DeclaredCount`, and this record adds no dimension for
  them.
- VM-1 implements P1 and P3 to P5, the aggregate budget and its refusal paths,
  the applicability matrix and its structural consequences, the poll contract
  and its detection path, and the fixture variants that prove them - a
  deliberately non-charging fixture, a deliberately over-running one, and a
  proportionality fixture. The VM-1 exit gate already names aggregate budget
  exhaustion across several runtimes, so none of this is deferrable further.
- VM-2 implements P0 to P2 and the materialization ordering, and property-tests
  M1 and M2 over randomized layer values. The ordering is mechanically
  assertable: an instrumented byte source records the first payload read, a
  materialization event records completion, and the test asserts strict
  precedence for every artifact in the malformed corpus, including the ones
  that fail.
- VM-4 supplies the concurrent evidence: exhaustion under a shared parent
  asserted on category, dimension and scope only, and the stress evidence
  section 7 attaches to the one declared mutable cross-runtime object.
- VM-5 must baseline metering separately from execution: per-charge cost,
  per-host-call cost, `Poll()` at back-edge density, and the suspend/resume
  clock transitions. Fifteen meters and a per-back-edge poll are a hot-path
  cost the core imposes on every language, and a host that must verify one
  artifact under two budgets pays for two full verification passes.
- The public support table (deferred to VM-6) owes five statements this record
  fixes: `LiveBytes` is attributed allocation and not a process or GC-heap
  limit; `WallClock` is attributed execution time and not a real-time deadline;
  fuel is charged by the profile and the core cannot preempt one; fuel units
  are profile-scoped and carry no cross-profile meaning; and an aggregate
  budget is a cross-runtime information channel that allowances never refund.
- **Amendment drivers named now, so the deferral is a decision.** A sixteenth
  dimension - an I/O or descriptor budget, for instance - and nested aggregate
  budgets for nested worker-style agents each force core contract version 2
  under section 2's procedure. Section 2 already predicts at least one
  amendment; these are the two likeliest drivers from this record.
- Struck names, never to be reintroduced without an amendment: `Allocation`,
  `HostCallCount`, `LiveRuntimeCount` as dimension names, and
  `WallClockPausesWhileSuspended` as a runtime option.
