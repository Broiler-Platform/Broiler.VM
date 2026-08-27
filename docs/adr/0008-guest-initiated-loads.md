# ADR 0008 - Guest-Initiated Loads And The Artifact-Provider Capability

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 6 requires this contract to be frozen before anything implements
it and states the reason: retrofitting re-entrant verification into an already
frozen lifecycle is a core contract amendment, whereas specifying it now is a
paragraph. Invariant 11 fixes the four properties the contract must have -
mediated, bounded, charged to the requesting operation, deterministically
refusable - and invariant 3 requires bytes a profile obtains while executing to
become their own verified handle before anything in them runs.

Section 9 is why this is a core contract rather than one language's need wearing
a core name. JavaScript needs guest-initiated loads for `eval`, the Function
constructor, dynamic `import()`, and specifier resolution; WebAssembly needs
none of them in its first version. Refusal, bounding, and charging are
language-free mechanism; what a specifier means is semantics, and stays with the
profile (section 8).

So the question this record settles is not whether guest-initiated loads exist.
It is where the audit question is answered - which profiles in this image can
ask for more code, who may answer them, and what a composition that answers
nothing returns - and it must be answered so that the answer is a property of
the composition rather than of the guest's input, its nesting depth, or how much
budget it has already spent.

**Existence.** Every artefact named below is a VM-0 decision on paper unless it
carries a different marker, and each carries its marker at first mention. This
record cites exactly two files as existing: the core contract version constant
(exists at VM-0: src/Broiler.VM.Abstractions/VmCoreContract.cs) and the
architecture-rule register (exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/rules.register.json). No mediator,
provider, bound, meter, or refusal path is implemented at VM-0, and no
architecture rule in the register asserts anything in this record.

## Decision

Core contract version 1 admits and fully specifies guest-initiated loads; the
constant it is bound to lives in src/Broiler.VM.Abstractions/VmCoreContract.cs
(exists at VM-0). ADR 0003 (`0003-core-contract-v1-and-amendments.md`) owns the
version assignment and the amendment procedure, and this record is one of its
contract-bearing ten.

### Declaration is necessary and is not sufficient

A profile declares that it may request loads in its catalog descriptor
`VmProfileDescriptor` (VM-0 decision on paper; no file at VM-0). ADR 0002
(`0002-profile-identity-and-static-catalog.md`) owns the one frozen descriptor
field table; this record contributes one field to it, a two-case discriminator
that is either *not declared* or *declared* with three mandatory parts and no
defaults:

| Part | What it states | Constraint |
|---|---|---|
| Minimum artifact-provider capability version | the lowest provider capability version this profile can work with | an ordinary capability-import minimum, bound by ADR 0011's rules; this record mints no separate version axis |
| Guest-load hard maxima | the profile's own maxima for the four bounds below | finite; for a declaring profile these four rows of ADR 0007's profile-maxima column may not be unconstrained |
| Verifier-work-to-fuel rate | what one unit of nested verifier work costs in the requesting operation's fuel | positive and finite |

A descriptor that declares loads with any part missing or unbounded is rejected
where every other descriptor defect is rejected: catalog construction throws
`VmCatalogValidationException` (VM-0 decision on paper; no file at VM-0) and
returns no result to inspect. This record amends the cluster ruling that named
an `invalid state` outcome here, because ADR 0002 places composition-time
descriptor failures outside the operation-result stages entirely.

Declaration is not sufficient. The composition must also register an
artifact-provider capability on the runtime. Nothing a profile declares can
create the power to load; declaration only makes the power *grantable*.

### The mediator is handed out, never looked up

The core hands a declaring profile's executor a core-owned mediator,
`IVmArtifactLoadMediator` (VM-0 decision on paper; no file at VM-0), at executor
creation. It hands a non-declaring profile nothing at all. An undeclared request
is therefore structurally unrepresentable rather than a runtime check that could
be reported, logged, or forgotten - which is what makes "can this profile ask
for more code?" a composition-time question with a compile-time answer.

The mediator is valid only for the dynamic extent of the invocation that
supplied it. Retaining it and using it later returns `InvalidState`, reason
`MediatorOutOfScope`. Reason codes are contract surface at version 1; ADR 0005
(`0005-operation-result-envelope.md`) owns the reason registry, and this record
contributes the guest-load reasons named throughout.

### The only re-entrant path in the core

ADR 0004 (`0004-lifecycle-and-state-machine.md`) owns the reentrancy rules.
Section 7 requires them to state explicitly whether a guest-initiated load may
re-enter the runtime that requested it. It may, and it is the only execution
path in the core that may:

- it re-enters the runtime that requested it and no other runtime;
- on the requesting operation's own thread;
- inside the parent's already-held execution slot, taking no second slot;
- only through the mediator, and only as a child verification, plus a child
  instantiation where the profile's declaration permits one;
- charged to the parent operation's remaining allowances; and
- bounded by the four bounds below.

Everything else that could re-enter is refused. In particular the artifact
provider itself may not re-enter (see below), and no guest-initiated load may
occur during verification at all - ADR 0006
(`0006-verified-artifact-ownership.md`) fixes provider isolation as part of
verification being separable from execution, so a composition can verify
artifacts with no provider present and get identical results.

## The Artifact-Provider Capability

The artifact-provider capability is a distinct capability **kind**, not a
value-returning import whose return type happens to be bytes. ADR 0011
(`0011-source-level-profile-contract.md`) owns the frozen capability descriptor
and the capability-kind enumeration (VM-0 decision on paper; no file at VM-0);
this record fixes what the artifact-provider kind means.

| Property | Ruling |
|---|---|
| Kind | A distinct capability kind. Registering any number of value capabilities, or sharing a host registry, never implies one. |
| Cardinality | At most one per runtime. A second binding of the kind is refused at runtime creation, `HostFailure` / `DuplicateArtifactProvider`. |
| Identity | The registered capability's `CapabilityId` **is** the provider identity, and it is a mandatory cache-key input. No separate identity field is minted: ADR 0011 already requires a stable, non-localized, namespaced capability ID, so a provider without an identity cannot be constructed. |
| Version | The ordinary capability version. An import names one exact version; the profile's declared minimum is checked as an ordinary capability-import binding at runtime creation. |
| Invocation | Synchronous, on the requesting operation's own thread, inside the requesting invocation's frame. |
| Request | A core-owned envelope carrying the requesting runtime and profile identity, the current nesting depth, the operation's remaining-allowance snapshot, the cancellation token, and an opaque profile-owned request payload. The core never interprets the payload. |
| Answer | A closed set of exactly three: *provided* (descriptor plus bytes), *refused* (reason), *not found* (reason). Adding a fourth is a core contract amendment. |
| Refusal | A typed answer, never an exception. A thrown exception is a host fault translated by ADR 0011's translation rule and is not a refusal. |
| Reentrancy | Forbidden. Any call to verify, instantiate, invoke, resume, or dispose on any runtime from inside the provider returns `InvalidState`, reason `ReentrantRuntimeCallFromCapability`. The provider is a byte source, not a runtime client; the core performs the nested verification itself after the provider returns. |
| Same profile | The returned descriptor must name the profile the requesting runtime is executing. A different profile is refused (see the classification table). |
| Cost | One unit of the requesting operation's `HostCalls` allowance, plus the wall-clock elapsed during the call. |

**Why the shape is synchronous and byte-returning.** Section 11 gives the host
the event loop and forbids the core from fetching anything. A `Task`-returning
provider would put a scheduler, a synchronization context, and an async state
machine into the core's public surface, and would add AOT-visible machinery for
a case the contract already covers: asynchronous acquisition is expressed as
profile suspension, a host fetch on the host's own loop, host resume, and a
then-satisfiable synchronous request. ADR 0009
(`0009-external-suspension-and-async-instantiation.md`) owns suspension and
`VmSuspension` (VM-0 decision on paper; no file at VM-0).

**Why refusal is a value.** For a content-policy-constrained composition,
refusal is the steady state, not the error path. An exception on the common path
is slow where latency is measured and is indistinguishable from a provider
defect, which section 7 requires the core to translate rather than interpret.

**Why one provider.** Provider identity is a cache-key input (section 6).
Several providers make the key ambiguous and turn "this composition registers no
provider" from a property of the composition into a property of each request,
which is exactly the determinism sections 6 and 16 wrote the rule for.

**Rejected.** An asynchronous provider; the provider as an ordinary typed import
(section 1 and section 7 both forbid a value import implying the power to
introduce code, and it would make the allowlist unfalsifiable, since every
byte-returning import would need review); a provider allowed to call back into
the runtime (it defeats the charging model, makes cancellation and disposal
ordering unanalysable, and would let a provider produce a handle outside the
core's verification path); refusal by a well-known exception type; several
providers selected by the profile.

## Classification At The Mediator Boundary

No core result category is minted for any part of this contract. ADR 0005 owns
the closed category set, the seven envelope-bearing stages - the guest-initiated
load is one of them - and the single precedence order used at every stage. This
record fixes the mediator's **observation points**: which facts are looked at,
and in what order. Precedence resolves facts that are observed together;
observation order decides which facts are observed at all. The two are not
competing rules.

| Order | Observation point | Outcome and reason |
|---|---|---|
| 1 | Legality: the mediator is in scope, the operation is Running, the call is not from inside a host capability | `InvalidState`; `MediatorOutOfScope`, `ReentrantRuntimeCallFromCapability`, or ADR 0004's state reason |
| 2 | The cancellation latch of the requesting operation | `Cancellation` |
| 3 | Whether the composition registered an artifact-provider capability | `HostFailure` / `ProviderNotRegistered` |
| 4 | Admission bounds: nesting depth, then fan-out, then an already-exhausted allowance | `ResourceExhaustion`, naming the exhausted dimension and scope per ADR 0007 |
| 5 | The provider is invoked and answers | `HostFailure` / `ProviderRefused`, `ProviderArtifactNotFound`, `ProviderContractViolation`, or `ProviderProfileMismatch` |
| 6 | The returned byte length against the cumulative nested-bytes bound | `ResourceExhaustion`; the artifact is dropped unverified |
| 7 | Ordinary verification of the returned bytes | the caller-driven load outcomes, and `UnsupportedProfile` where the descriptor names a profile absent from the catalog |

Step 3 is the load-bearing one. It precedes every bound **and** is taken before
the request payload is inspected, so a composition that registers no provider
gives one answer to every request, forever. The answer cannot depend on what the
guest asked for, how deep it is, or how much budget it has left. Only two facts
can precede it, and both are properties of the operation rather than of the
request or of the policy: an illegal call and an observed cancellation. Checking
bounds first would make the refusal in a provider-less composition depend on
guest-controlled state, and section 6's "refuses every request
deterministically" would be false in the one case it was written for.

Step 7 shows precedence doing its work. A provider that returns a descriptor
naming a profile that is *in* the catalog but is not the requesting profile is a
provider contract failure, `HostFailure` / `ProviderProfileMismatch`. One naming
a profile *absent* from the catalog is `UnsupportedProfile`, because that fact
outranks `HostFailure` in the single precedence order and both are observed
together. A nested `UnsupportedProfile` reports the requested ID only: the
catalog's contents appear on the host-facing surface and never in a result
observed by guest code. ADR 0002 owns that split.

### Mapping a nested outcome onto the requesting invocation

The nested load is its own stage. Its result is observed by the profile that
requested it and is never returned to the caller. What the caller sees is the
requesting invocation's own result, and convertibility - not the profile's taste
- decides what that may be.

| Nested outcome | Conversion | The requesting invocation reports |
|---|---|---|
| `InvalidArtifact` | mandatory | `ProfileFault`, once the language-defined fault propagates out of guest code |
| `UnsupportedProfile` | mandatory | `ProfileFault`, likewise |
| `HostFailure` | optional | `ProfileFault` if converted; otherwise `HostFailure`, which is legal at invocation |
| `ResourceExhaustion` | forbidden | `ResourceExhaustion` |
| `Cancellation` | forbidden | `Cancellation` |
| `InvalidState` | forbidden | `InvalidState` |

A profile that surfaces `InvalidArtifact` or `UnsupportedProfile` to the caller
unconverted has violated the profile contract, reported as `ProfileFault` with
core reason `NestedFailureNotConverted`. Mandatory conversion is what keeps
`InvalidArtifact` a load-stage-only category and keeps a nested artifact's
malformedness a language event inside the guest rather than a claim about the
caller's artifact. Conversion means conversion *into the language*: whether the
resulting fault is caught by guest code, so that the invocation completes
`Normal`, is a language question the core must not decide - invariant 4, and the
content-policy case in section 11 where a page's own catch handler must run.

The three terminal outcomes are not convertible into anything. The mediator
returns a terminal signal, and the profile must abandon the operation with
bounded unwinding that runs no further guest code able to request a load or to
suspend. A catchable `ResourceExhaustion` would let a guest spin on exhaustion
and would defeat bounded cancellation; that is section 14's lifecycle blocker
and section 16's malicious-input row.

**Every refusal is recorded whether or not the guest swallows it.** The core
emits a `GuestLoadRefused` diagnostic carrying the reason, the nesting depth,
and the provider's capability identity where one exists, and increments a
per-operation counter. ADR 0005 owns `VmDiagnostics` (VM-0 decision on paper; no
file at VM-0) and its requesting-operation and nesting-depth fields. So a
`Normal` result still carries the evidence that a refusal happened, and a
composition can prove "no dynamic code ever ran here" from diagnostics alone.

## Bounds, Defaulting, And Charging

Four bounds, and exactly four. They are not a new budget system: they are four
of the fifteen dimensions in ADR 0007's frozen dimension table
(`0007-resource-authority-and-budgets.md`), which this record cites and does not
restate.

| Dimension | What it bounds |
|---|---|
| `NestedLoadDepth` | concurrently active nested loads under one invocation |
| `NestedLoadFanOut` | total requests admitted by one invocation, refused ones included |
| `NestedLoadBytes` | the sum of provider-returned byte lengths per invocation |
| `VerifierWork` | verification effort, shared with the caller-driven path |

`VmGuestLoadBounds` (VM-0 decision on paper; no file at VM-0) is the name of
that group of four. The cluster ruling's separate `MaxDepth`,
`MaxFanOutPerOperation`, `MaxCumulativeNestedBytes` and
`MaxCumulativeNestedVerifierWork` names are struck as duplicates of dimensions
that already have frozen names.

`VerifierWork` deliberately is not a private nested meter. It is the same
allowance the caller-driven verification path draws on, which is precisely why a
nested verification competes with, and can never enlarge, the requesting
operation's own verification budget. A separate nested-only meter would let a
guest convert unlimited fuel into unlimited verification.

**Defaulting: adopt all four or specify all four.** ADR 0007 already requires
every one of the fifteen dimensions to carry an explicit value or an explicit
adoption marker at runtime creation, and omission never means unbounded
(invariant 9). This record adds one rule on top: the four move together. A
composition may adopt the profile's guest-load maxima for all four or supply all
four explicitly; a mixture is refused at runtime creation, `HostFailure` /
`GuestLoadBoundsNotConfigured`, and a host value above the profile's maximum is
refused as `GuestLoadBoundExceedsProfileMaximum`. The reason is that the four
are not independent safety properties - depth without a byte bound, or fan-out
without depth, each leaves an axis open - so a half-configured quadruple looks
configured and is not. This record amends the cluster ruling that reported these
as `invalid state`: ADR 0005 reserves that category for illegal transitions and
use-after-dispose, and a misconfigured composition is neither.

A profile that does not declare guest-initiated loads has the bound set
(0, 0, 0, 0), and marks the same four rows `NotApplicable` in ADR 0007's
per-dimension declaration matrix - one fact stated in two vocabularies, which is
why ADR 0007 can say that a `NestedLoadDepth` marked `NotApplicable` forbids
binding an artifact provider at all. The zeros are what the meters read, so a
diagnostic or an aggregate sum over a mixed composition is always well-defined;
the profile still has no mediator, so the zeros are never reached.

**Charging order.** Enforcement order is not a detail - it is what makes a
hostile artifact unable to spend a bound before the bound is observed.

| When | What is charged or checked |
|---|---|
| At admission, before the provider is called | `NestedLoadDepth`, then `NestedLoadFanOut`, then whether the operation's `Fuel`, `WallClock` or `AllocatedBytes` are already exhausted |
| During the provider call | one `HostCalls` unit and the elapsed wall-clock, both against the requesting operation |
| On return, before verification begins | the returned length against `NestedLoadBytes`; an over-bound artifact is dropped unverified |
| As verification proceeds | `VerifierWork` incrementally, and `Fuel` at the descriptor's declared verifier-work-to-fuel rate |
| After a handle exists | nested instantiation, charged the ordinary way |

Fuel conversion exists because section 6 requires nested verification to draw on
the invoking operation's fuel, and fuel is a profile-owned unit the core cannot
invent. The rate is declared by the profile because only the profile can defend
it; VM-5's verification-throughput baseline is the natural evidence for it.

**Aggregate scope.** All four of these dimensions carry `Aggregate` scope under
ADR 0007's rule that a dimension is aggregate-scoped exactly when its measure is
summable across concurrently live runtimes under one parent. A nested load
therefore decrements the parent meter of a `VmAggregateBudget` (VM-0 decision on
paper; no file at VM-0) as well as the requesting operation's, and a guest load
in one runtime can exhaust a sibling runtime's ability to make one. That is
invariant 9 working as written, not a surprise: creating more runtimes must not
multiply a host maximum.

**Rejected.** Core-wide numeric defaults (the core cannot know what a language's
module graph costs; a number chosen here would be either a de facto semantic
limit on every profile or routinely overridden and therefore meaningless - the
fixture profile may carry such numbers as test data); treating an omitted bound
as inherited from the operation's fuel (a cheap, deeply recursive load graph
passes when each step is individually affordable); charging nested verification
only to a separate verification budget; letting a host or provider raise a bound
mid-operation, which is invariant 9's forbidden direction and section 16's
"artifact weakens host policy" risk.

## The Nested Handle

A handle produced by a guest-initiated load is `VmVerifiedArtifact` (VM-0
decision on paper; no file at VM-0) like any other, produced by the same
verification entry point. Two things about it are different, and both follow
from one fact.

**Its ceilings intersect four inputs, not three.** Computed before any untrusted
allocation: the host's runtime ceilings, the selected profile's maxima, the
nested descriptor's requested limits (which may only request less), and a
snapshot of the requesting operation's remaining `Fuel`, `WallClock` and
`AllocatedBytes` at the moment verification begins. Section 6 requires the
remainder in the intersection. Excluding it would let a guest manufacture a
handle carrying full host ceilings from inside a nearly exhausted operation.

**It is runtime-scoped and is never shareable.** Those ceilings encode one
operation's transient remainder, so identity equality between two runtimes is
not sufficient for the handle to be safe in a second one. `VmArtifactOrigin`
(VM-0 decision on paper; no file at VM-0) is a non-compared field on the handle
with the two members `Caller` and `GuestInitiated`, and a `GuestInitiated`
handle presented to any runtime other than its producer is refused with reason
`NestedHandleNotShareable`, checked **before** the identity comparison rather
than as part of it. ADR 0006 owns the sharing predicate and inserts this as its
condition 0. The refusal has to be structural, because a comparison that
happened to succeed would be a comparison against a third party's spent budget.

**The remainder flag is struck.** The cluster ruling put a
`RemainderDerivedCeilings` flag on the handle. There is no such field. The
remainder snapshot is taken on the nested verification path and nowhere else,
so `Origin == GuestInitiated` is exactly the set of remainder-derived handles;
a second field would be a second way to ask the same question and a second way
to get it wrong. ADR 0006's pruned non-compared field set stands unchanged.

**One choke point carries both rules.** The origin gate that refuses
cross-runtime use is also the persistence gate: a handle whose origin is
`GuestInitiated` is ineligible for any persisted envelope and may not contribute
to any persisted cache key. Without that rule a remainder-derived ceiling - a
timing-dependent, process-local quantity - would reach a persisted artifact
through the effective-ceilings key input, which invariant 5's final sentence
forbids, and would produce a cache key that never recurs. ADR 0006 owns the two
key sets and states that the persisted-envelope key is the narrower closed set;
this record states the equivalence that makes origin the sufficient
discriminator, and puts the check where a handle is already resolved for a
runtime rather than adding a second enforcement site.

Correctness does not depend on ceilings being in the persisted key: loading
always re-verifies, and effective ceilings are recomputed at load time against
the loading runtime, so a persisted artifact never carries a ceiling decision
forward.

**Rejected.** Excluding the remainder so that nested handles are ordinary and
shareable; allowing sharing whenever both runtimes' ceilings dominate the
handle's, which makes shareability a runtime-pair property computed at use time
instead of a handle property fixed at verification - the ambiguous borrowing
section 6's handle-lifetime paragraph exists to forbid.

Within its own runtime the profile may retain the handle for as long as the
runtime lives; a module map is the obvious consumer, and runtime disposal
disposes it.

## Content Policy By Omission, And The Browser Path

A content policy forbidding dynamic evaluation is expressed by registering
**no** artifact-provider capability. That refusal is total,
content-independent, and deterministic, and this record can claim so only
because of the three rulings above:

1. the mediator is the only route by which a profile can obtain executable bytes
   - invariant 11 plus the handed-out mediator, with no filesystem, socket,
   compiler, embedded resource, or byte-returning host object as an alternative;
2. the provider is a distinct capability kind that no number of value
   capabilities and no shared registry implies; and
3. the registration check precedes every bound and the request payload, so the
   answer is one answer for every request.

A policy with exceptions is a registered provider that answers *refused* per
request, which is `HostFailure` / `ProviderRefused` and is convertible into a
catchable language fault like any other host failure. A per-request flag on the
runtime or the descriptor is rejected: it is the ad-hoc engine-internal check
that section 11 says the capability model replaces, and it leaves the code path
that fetches bytes present in the image.

The worked embedding path, which section 11 asks VM-0 to record:

| Direction | Path |
|---|---|
| Caller-driven | The host found the script and decides when it runs. The adapter lowers it to bytes, verifies, instantiates, invokes. No provider is involved and no bound in this record applies. |
| Guest-driven | Code is already running and asks for more. The request goes through the mediator to the provider, which answers synchronously from the host's module map. |
| Asynchronous | The profile suspends, the host fetches on its own event loop, the host resumes the operation, the profile re-requests, and the provider now answers synchronously. The core never fetches, schedules, or awaits anything. |

Two consequences of that path are load-bearing rather than incidental. First,
the browser's composition links parser, front end, and lowering, so its Native
AOT gate must cover the compiler-bearing closure, which is larger than the
execution-only closure the core's own gates prove (invariant 6, section 11).
Second, because nested handles are remainder-derived and runtime-scoped, a host
that wants one verification shared by two realms pre-verifies the dependency
through the caller-driven path - which it can do, because it owns the module
map. The compile cost is still paid once: the code cache is the host-keyed
persisted envelope (VM-0 decision on paper; no file at VM-0 - ADR 0010,
`0010-embedding-decisions.md`, owns its status), and only verification
repeats.

**A blocking network fetch inside the provider is not the sanctioned design.**
The core cannot preempt a host call, so a blocking fetch overshoots the
operation's wall-clock budget and stalls the host's own loop. Suspension exists
so that it need not.

**No architecture rule asserts any of this at VM-0.** The cluster ruling
proposed an architecture test over two composition-root projects, one
registering a provider and one not. VM-0 creates no composition-root project and
no provider implementation, the project set is closed at five, and the rule
register contains no row for such a test - the subject does not exist before
VM-3. An implementer must not create those projects to satisfy this record:
Rule A11: No project
outside the composition-root allow-list references an assembly matching
Broiler.VM.Profile.*. The allow-list is empty at VM-0. Status: Active; witness
`src/tests/Broiler.VM.Architecture.Tests/witnesses/A11-profile-reference-outside-composition-root.csproj.witness`.

## What VM-0 Does Not Prove

- **Nothing here is implemented.** Every ruling in this record is a VM-0
  decision on paper. VM-1 implements the mediator, provider registration, and
  the refusal paths and proves them against the fixture profile and a test-only
  fixture provider (deferred to VM-1); VM-2 implements the bounds, the ordering,
  the charging, and the nested handle's ceilings (deferred to VM-2). The fixture
  profile needs a declaring and a non-declaring variant, so that both the
  mediated path and the structurally impossible one are demonstrated.
- **No product provider ships.** The first core release registers no
  artifact-provider capability in any product composition, and no product
  profile exists to declare guest-initiated loads. The fixture provider is
  test-only and is never referenced by a product package.
- **A guest-initiated load without a registered provider discharges invariant 8
  by a named deterministic failure** - `HostFailure` /
  `ProviderNotRegistered` - which is form (a) of the two discharge forms
  ADR 0003 defines. This record cites that narrowing and does not restate it;
  ADR 0003's admitted-versus-implemented table carries the row.
- **The core cannot bound a provider call from outside it.** The provider's
  duration is host-owned; the core detects an overrun only when the call
  returns, so a blocking provider can overshoot the requesting operation's
  wall-clock allowance by the length of one call. This is a permanent property
  of a synchronous host boundary, not an unimplemented feature. It must be
  stated in the host-capability documentation and in the future public support
  table that ADR 0012 (`0012-security-ownership-and-support-matrix.md`) governs,
  rather than papered over with a timer the core cannot enforce.
- **The core cannot tell two providers apart by behaviour.** Provider identity
  is the capability ID, so two compositions that register the same ID behind
  different policies will share cache entries. Choosing distinct IDs for
  distinct policies is a host obligation the core cannot check.

## Consequences

- **Reason codes are contract surface at version 1.** Adding one later is an
  additive amendment; renaming or removing one is not. The codes this record
  contributes to ADR 0005's registry are `MediatorOutOfScope`,
  `ReentrantRuntimeCallFromCapability`, `ProviderNotRegistered`,
  `DuplicateArtifactProvider`, `ProviderRefused`, `ProviderArtifactNotFound`,
  `ProviderContractViolation`, `ProviderProfileMismatch`,
  `GuestLoadBoundsNotConfigured`, `GuestLoadBoundExceedsProfileMaximum`,
  `NestedHandleNotShareable`, and `NestedFailureNotConverted`.
- **A profile's conformance obligation gains one core-owned rule: honour
  convertibility.** VM-1 must test a fixture profile that tries to swallow a
  terminal outcome and prove the core still terminates the operation.
- **Roadmap section 7's aggregate-metering sentence is narrower than this
  contract.** It names fuel, wall-clock, allocation, and live-runtime counts;
  eleven dimensions now carry `Aggregate` scope, including the four in this
  record. ADR 0003's roadmap-amendment register carries that sentence as a
  `Proposed`, not applied row, and until an owner lands the patch the roadmap
  and this record disagree there. VM-0 proposes no roadmap edit of its own.
- **The runtime-creation options object carries the guest-load bounds group.**
  ADR 0004 owns the corrected creation shape and records the supersession of
  section 3's illustrative runtime-creation snippet in its own Consequences;
  this record adds one member to that object and claims no supersession of its
  own.
- **Section 6's cache-key bullet is satisfied without a new key field.** The
  artifact-provider capability's tuple is one of the per-import capability
  tuples already in ADR 0006's closed persisted-envelope key set, so a cache
  entry produced under a permissive provider cannot be reused under a
  restrictive one. That is the correct security outcome and a real storage cost
  to state.
- **Allowlist and audit tooling has one question and one place to ask it.** "Can
  this image introduce code at run time?" is answered by looking for a single
  registration site, statically, without running anything.
- **VM-2's guest-load gate items map one-to-one onto this record**: depth,
  fan-out, cumulative bytes, cumulative verifier work, charging to the
  requesting operation, and the intersection of the nested handle's ceilings
  with the remaining allowance. VM-2's "a nested load cannot exceed, extend, or
  escape its requesting operation's budget" is satisfied by construction for the
  ceilings, and by the charging order for the allowances.
- **A future support table must distinguish three facts that are simultaneously
  true in release 1**: the contract admits mediation, the core implements it,
  and the shipped composition provides no provider. Collapsing them would be an
  untruthful support claim, which section 16 makes a stop condition.
