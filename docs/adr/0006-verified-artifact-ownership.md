# ADR 0006 - Verified-Artifact Ownership And Immutability

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 6 fixes the property a verified artifact must have - opaque,
immutable, profile-bound, and the only thing instantiate and execute accept -
and invariant 3 makes that property the core's headline safety claim. Neither
says who chooses the representation, what identity a successful verification
binds, who may dispose the result, under what predicate a second runtime may
consume it, or what stops verification from quietly acquiring a dependency on
execution. Section 13's VM-0 exit gate requires this record to name
verified-artifact ownership and to record that verification is separable from
execution and must stay so.

This record settles those questions for core contract version 1. Everything it
decides is a decision on paper. At VM-0 the component holds three product
project shells with no behaviour, and the only checked-in product code is
`VmCoreContract` (exists at VM-0:
src/Broiler.VM.Abstractions/VmCoreContract.cs), whose `Version` constant is the
1 that identity component 5 below carries. No type, member, enum, predicate, or
check named in this record exists as code, and none is asserted by a test at
VM-0; the section "What VM-0 does not prove" says so precisely. Sibling records
are cited as ADR NNNN (`file-name.md`), and all twelve exist at VM-0 under
docs/adr/.

## Decision

| # | Question | Ruling |
|---|---|---|
| 1 | Snapshot of the caller's bytes, or a decoded form? | The profile declares one kind for every artifact it verifies. The core mandates neither and owns the input boundary instead: `ReadOnlySpan<byte>` is the only byte form on the verification path. |
| 2 | What does a successful verification bind? | Exactly seven identity components, compared by value, all seven compared when a second runtime is offered the handle. |
| 3 | Ordinary managed data, or a disposable resource? | Both kinds are admitted and declared per profile. Three states, the lease counter, idempotent disposal, and deterministic use-after-dispose are unconditional and identical for both. |
| 4 | When may a second runtime consume a handle? | An origin gate, then eleven ordered conditions, first failure wins. Only a missing catalog entry is `unsupported profile`; every other refusal is `invalid state` with a reason naming the failing clause. |
| 5 | How may verification fail? | Four profile-facing failure classes over a core-owned reason set. An escaping verifier exception is never translated into one of them. |
| 6 | Is verification separable from execution? | Requirement V-SEP: one entry point, no capability invocation, no guest-initiated load, and no executor until first instantiation. |
| 7 | What may be keyed on, and what may be persisted? | Two named input sets, and a persisted-envelope key that is a narrower closed set excluding every remainder-derived and process-local fact. |

The seven rulings are stated in the seven sections that follow. Where the
arbitration of VM-0's briefs amended the ruling that reached this record, the
amendment is recorded in "Rulings this record amends" rather than argued again.

## 1. The input boundary and the representation choice

**The representation is the profile's choice, declared statically.** The catalog
descriptor carries `VmArtifactRepresentationKind` in `{Snapshot, Decoded}` (VM-0
decision on paper; no file at VM-0). It is fixed per profile, never per artifact
and never per call. The core never branches on it semantically: it is
diagnostic, evidence, and persistence-eligibility metadata only. ADR 0002
(`0002-profile-identity-and-static-catalog.md`) owns the single frozen
descriptor field table; the fields this record requires are contributed to it
and are not restated here.

**The core guarantees invariant 3 by owning the input boundary instead.**

- *Sole entry shape.* One verification entry point takes an immutable descriptor
  value plus a verification input whose byte form is `ReadOnlySpan<byte>`. No
  public or profile-facing member on the verification path accepts `byte[]`,
  `ArraySegment<byte>`, `Memory<byte>`, `ReadOnlyMemory<byte>`, `Stream`,
  `PipeReader`, `IBufferWriter<byte>`, `IEnumerable<byte>`,
  `IAsyncEnumerable<T>`, or a pointer. Because a by-ref struct cannot be stored
  in a field, boxed, captured, or awaited, neither a verifier nor a handle can
  retain an alias to caller storage. Invariant 3 is then enforced by the type
  system on the ordinary path rather than by each profile's discipline.
- *Bytes are the only input.* ADR 0010 (`0010-embedding-decisions.md`) rules the
  byte round trip mandatory, so there is no compile-directly-to-handle path and
  no second input form. Representation kind therefore describes what the handle
  keeps, never how it was fed.
- *The core makes the copy.* Where the profile declares `Snapshot`, the core
  allocates a core-owned buffer only after the effective artifact-byte ceiling
  is materialised and the declared length has cleared it - ADR 0007
  (`0007-resource-authority-and-budgets.md`) owns that ordered computation - and
  hands the verifier a span over the core's copy. The handle's byte store is
  that buffer. Where the profile declares `Decoded`, no copy is made and the
  handle retains no bytes.
- *Deep immutability.* Everything reachable from a handle is immutable once
  verification returns and is safe for unsynchronised concurrent readers;
  publication is a single store after complete construction. Internal
  memoisation is legal only where it is pure, deterministic, race-benign (any
  racing computation yields an equal value), and observably inert. Core contract
  version 1 approves no use of it: the deferred-body case that the incoming
  ruling approved disappeared with lazy per-section verification (ADR 0010).
- *No borrowed identity.* The caller's descriptor is copied as a value at entry,
  so mutating the caller's descriptor object afterwards cannot change what the
  handle is bound to. Nothing reachable from a handle references a runtime, an
  instance, a realm, a capability delegate, a host object, a cancellation token,
  or a diagnostics sink.
- *Truthfulness.* A profile that defeats the span rule with unsafe code or a
  pinned pointer commits a defect, not an alternative design. The detection
  mechanism is VM-2's corpus that mutates, disposes, and concurrently overwrites
  the caller's original buffer after verification (deferred to VM-2).

**Why the core does not mandate a representation.** Section 6 and invariant 3
already admit both forms, so VM-0's job is to say who chooses and what the core
enforces regardless.

| Rejected | Why |
|---|---|
| Core mandates a byte snapshot for every profile | Forces a profile that decodes into an immutable graph to keep bytes it never reads again and doubles peak memory. Section 9's WebAssembly shape - an immutable module distinct from each instance - is exactly that profile. |
| Core mandates a fully decoded representation | The core would have to know what "decoded" means, which is the semantics invariant 4 keeps out of it, and it forecloses a profile whose safest verified form is validated bytes plus an index. |
| Profile chooses, with no core input rule | Invariant 3 would then rest on profile discipline; one profile retaining a `ReadOnlyMemory<byte>` over caller storage breaks the core's headline claim with nothing mechanical to catch it. |
| Accept `ReadOnlyMemory<byte>` or `Stream` for ergonomics, document a no-retention rule | Documentation is not enforcement, and the failure it permits is the one section 16 and section 14 both name as a release blocker. |

**Forbidden by this ruling:** any verification parameter whose buffer type the
callee can retain; a handle that aliases, wraps, pins, or holds a pointer into
caller storage; a per-artifact or run-time choice of representation; observable
lazily initialised state inside a handle; any reference from a handle to
runtime-, instance-, realm-, capability-, or host-owned objects.

## 2. What a successful verification binds

Seven identity components, frozen for core contract version 1, all compared by
value and all seven compared in the sharing predicate of section 4.

| # | Component | Content | Supplied by |
|---|---|---|---|
| 1 | `ProfileDescriptorIdentity` | (ProfileId, DescriptorRevision), where DescriptorRevision is a profile-declared integer the profile increments whenever any descriptor content that can affect verification changes: supported format range, manifest set, verifier version, profile hard maxima, host-capability descriptors | profile declares, core reads |
| 2 | `AcceptedProfileFormatVersion` | the format version the verifier actually accepted, not the range it supports | core |
| 3 | `FeatureManifestIdentity` | (ManifestId, ManifestVersion) | core |
| 4 | `VerifierSemanticVersion` | the identity of the rules actually applied | profile declares, core reads |
| 5 | `CoreContractVersion` | 1, from `VmCoreContract.Version` (exists at VM-0: src/Broiler.VM.Abstractions/VmCoreContract.cs) | core |
| 6 | `EffectiveCeilings` | (VerificationCeilings, InstantiationCeilings) - the materialised intersection of host ceilings, profile hard maxima, and artifact requests, as two immutable vectors over ADR 0007's dimension table | core |
| 7 | `HostSignatureAssumptions` | the canonically ordered set of seven-field capability tuples the verifier relied on | core |

Component 7's tuple is `(CapabilityId, Version, SignatureId, Kind, Reentrancy,
ExceptionTranslation, OptionalImportBound)`, where `Kind` distinguishes a
value-returning import from the artifact-provider capability that ADR 0008
(`0008-guest-initiated-loads.md`) owns. It records the **shape the verifier
assumed, never a binding**: no delegate, registration object, host object, or
capability instance is reachable from it. Binding to actual implementations
happens at instantiation.

Instance budgets, invocation budgets, fuel and time allowances, cancellation
tokens, and diagnostics sinks are **not** identity: they may only tighten
(invariant 9), so including them would forbid the section 11 code-cache case
where two realms share one artifact and differ in per-invocation fuel, with no
safety gain.

**Non-compared record fields, exactly five.** The handle additionally carries,
and never compares: a process-local opaque `VerifiedArtifactInstanceId` for
diagnostics and lease bookkeeping; `VmArtifactRepresentationKind`;
`VmArtifactLifetimeKind`; `VmArtifactOrigin`; and the artifact diagnostics base.
`EffectiveSectionVerificationMode` and `ProducedBy` are struck: each named a
capability core contract version 1 excludes, and a field whose value set has one
member is not a discriminator but a promise that the second member will arrive.
The separate `RemainderDerivedCeilings` flag proposed alongside ADR 0008's
nested-handle ruling is struck for the same reason and is unnecessary:
`VmArtifactOrigin == GuestInitiated` is exactly the set of remainder-derived
handles, so no field is added.

Identity and the record fields stay readable for the whole life of the handle,
including after disposal, so diagnostics remain truthful about an object whose
resources are gone.

**Comparison is by value, never by reference.** A descriptor object reference is
process-local, cannot be recorded in evidence or in a future persisted envelope,
and breaks as soon as two catalogs are built from the same accessor; invariant 5
forbids process-local identity in a persisted artifact outright.

**Forbidden by this ruling:** comparing identity by object reference; placing
any mutable, process-local, or runtime-scoped value in identity; a profile
supplying its own identity comparison or overriding equality; carrying resolved
import bindings, host handles, delegates, realms, memories, tables, globals,
feedback, or quickened code in identity or anywhere reachable from the handle;
adding or removing a component without minting core contract version 2 under ADR
0003 (`0003-core-contract-v1-and-amendments.md`).

## 3. States, lifetime kinds, and the lease contract

Both kinds section 6 names are admitted in core contract version 1, and the
ambiguity section 6 forbids is removed by making the kind a static declared
property of the profile rather than a per-artifact or inferred one:
`VmArtifactLifetimeKind` in `{Managed, Disposable}`. `VmVerifiedArtifact` always
implements `IDisposable` and never `IAsyncDisposable`, so caller code is
identical for both kinds. `VmArtifactOwnershipKind` and the state name
`DisposeRequested` are retired names and appear nowhere else.

`VmVerifiedArtifactState` has three members and no others.

| From | Trigger | Initiator | To |
|---|---|---|---|
| (none) | a verification stage completes `normal` | core | `Ready` |
| `Ready` | lease acquired - explicitly by a caller, or by the core on behalf of a new instance | caller or core | `Ready`, lease count + 1 |
| `Ready` | lease released - explicitly, or because the holding instance reached its terminal state | caller or core | `Ready`, lease count - 1 |
| `Ready` | dispose with lease count 0 | caller | `Disposed` |
| `Ready` | dispose with lease count > 0 | caller | `Draining` |
| `Draining` | last lease released | core | `Disposed` |
| `Draining` | lease acquire, or instantiate | caller | unchanged; `invalid state`, reason `HandleDraining` |
| `Draining` or `Disposed` | dispose again | caller | unchanged; `VmControlResult.NoOp` |
| `Disposed` | any use | caller | unchanged; `invalid state`, reason `HandleDisposed` |

Every other transition is illegal. `Disposed` is terminal; there is no
resurrection and a `VerifiedArtifactInstanceId` is never reused. Dispose is a
control operation returning `VmControlResult`, so it is not a stage and appears
in no stage row of ADR 0005 (`0005-operation-result-envelope.md`).

**The lease contract is unconditional.** Section 6 states it conditionally
("where sharing and disposal are both supported"), while section 14 lists
verified-handle identity and lease lifetime as unconditional release blockers.
Because this record rules that sharing exists for every profile that declares
it, the three states, the lease counter, idempotent disposal, and deterministic
use-after-dispose apply to every profile and both kinds. A conditional contract
cannot be architecture-tested and cannot satisfy an unconditional blocker. The
wording change this implies for section 6 is carried as a proposed, not applied,
row in ADR 0003's roadmap-amendment register; VM-0 applies no such amendment.

**Ownership.** Verification returns the root reference to its caller. A handle
produced by a guest-initiated load is rooted in the requesting runtime and
released when that runtime is disposed, and no member in contract version 1
hands a nested handle to the host (ADR 0008). Any consuming use -
instantiation, execution from an instance, presentation to a second runtime, or
a future persist - requires an active lease; acquisition fails with a stable
reason when the handle is `Draining` or `Disposed`. The core acquires an
instance lease implicitly at instantiation and releases it when the instance is
disposed; a host that wants to pin a shared handle takes an explicit lease.
Disposing a runtime releases every lease that runtime holds and never disposes a
root it does not own.

**Drain, never revoke.** There is no force-dispose, no lease revocation, and no
timeout that invalidates another holder's lease. Root disposal on a leased
handle transitions to `Draining`, refuses all new acquisition deterministically,
and completes when the last lease is released. This is the only rule that
satisfies section 14's blocking failure "one runtime invalidates another's
handle" while keeping disposal idempotent.

**What the kind changes.** Nothing in the table. For `Managed`, the transition
into `Disposed` releases nothing but still happens, and an undisposed handle is
simply collected. For `Disposable`, the profile's
release runs exactly once at the transition to `Disposed`, on the thread that
released the last lease, and any unmanaged resource sits behind
`SafeHandle`-style finalizable ownership as a backstop that is never a
substitute for disposal. The kind therefore governs only when reclamation is
observable and whether VM-4's plateau measurement can attribute the memory.

| Rejected | Why |
|---|---|
| Managed-only handles in contract version 1 | Contradicts section 7 step 7, which already transitions "any explicitly disposable verified handles", and would make a memory-mapped or natively backed code cache - the section 11 browser case - an amendment rather than a profile choice. |
| Disposable-only handles | Imposes lifetime management and a leak class on profiles whose verified form is pure managed immutable data, for no safety gain. |
| Per-artifact kind chosen by the verifier | A caller cannot know from the descriptor whether it must dispose. That is precisely the ambiguous borrowing section 6 forbids. |
| Dispose is a no-op on `Managed`, leaving the handle usable | Callers hold both kinds behind one type; a kind-dependent contract makes correctness depend on a descriptor field the call site may not inspect. |
| Reference counting with immediate free on root disposal | Lets one runtime invalidate another's input, which section 14 lists as a release blocker. |
| Throw on use-after-dispose | Section 7 requires one stable `invalid state` outcome; an exception bypasses the profile-neutral envelope and makes the failure class implementation-defined across JIT, trimmed, and Native AOT hosts. |

**Forbidden by this ruling:** a distinct disposable subtype or a finalizer on
the handle type for a `Managed` profile; any revoke, force-dispose, or
lease-timeout member; freeing while a lease is outstanding; resurrecting a
disposed handle; reporting use-after-dispose as `invalid artifact`,
`unsupported profile`, `profile fault`, or an exception on the result surface; a
member that returns a handle the caller holds neither a reference nor a lease
to.

## 4. The cross-runtime sharing predicate

A handle `H` may be consumed by a runtime `R` only when all of the following
hold, evaluated by the core in this frozen order, first failure winning.

| Order | Condition | Refusal |
|---|---|---|
| 0 | `H.Origin` is `Caller`, or `R` is the runtime that produced `H` | `invalid state`, reason `NestedHandleNotShareable` |
| 1 | `H.State` is `Ready` and a lease is acquired for `R` | `invalid state`, reason `HandleDraining` or `HandleDisposed` |
| 2 | `R`'s catalog contains an entry for `H`'s ProfileId | `unsupported profile`, reason `ProfileNotInCatalog`, naming the requested ID only, never the catalog's contents (ADR 0005's containment rule) |
| 3 | `CoreContractVersion` is equal | `invalid state`, reason naming clause 3 |
| 4 | `DescriptorRevision` matches `R`'s catalog entry | `invalid state`, reason naming clause 4 |
| 5 | `AcceptedProfileFormatVersion` matches | `invalid state`, reason naming clause 5 |
| 6 | `FeatureManifestIdentity` matches | `invalid state`, reason naming clause 6 |
| 7 | `VerifierSemanticVersion` matches | `invalid state`, reason naming clause 7 |
| 8 | Both `EffectiveCeilings` vectors are exactly equal to the vectors `R` would materialise for the same descriptor | `invalid state`, reason naming clause 8 |
| 9 | Every recorded host-signature assumption has a capability registered on `R` with the same seven-field tuple; extra capabilities on `R` are permitted, a missing or differing one is not, and the comparison is never by delegate or registration instance | `invalid state`, reason naming clause 9 |
| 10 | The profile declares `ArtifactSharing = Shareable` and the verifier has not narrowed this artifact to `RuntimeScoped` | `invalid state`, reason naming clause 10 |
| 11 | `H`'s aggregate-budget root and `R`'s are the same object, or both are absent | `invalid state`, reason naming clause 11 |

The refusal-reason set is closed at one identifier per clause plus the two
handle-state reasons and `NestedHandleNotShareable`; this record freezes the
members, and ADR 0003's public-name table publishes the declaring enum name at
VM-1. `ArtifactSharing` defaults to `RuntimeScoped` when a profile declares
nothing, and narrowing is monotonic: an artifact narrowed by its verifier is
never widened. No new outcome category is introduced anywhere in the predicate.

**Only clause 2 is `unsupported profile`.** Section 7 keeps that outcome
distinct because misreporting a composition mistake as a corrupt file is the
likeliest diagnostic error for a single-profile product, and a missing catalog
entry is the only clause where the requested identity is by itself the whole
diagnosis. The result names that requested identity and nothing else. ADR 0005
owns that containment rule - no result carries a listing of what the catalog
does contain, which reaches the host only through `VmCatalog`'s listing member.
A descriptor-revision, manifest, verifier-version, or ceiling mismatch is a
lifecycle and policy mismatch between two runtimes that both have the profile,
so it takes `invalid state` with a reason. This narrows the incoming ruling that
reported every foreign-runtime identity mismatch as `unsupported profile`.

**What is never part of a shared handle**, restated as a frozen exclusion list:
instances; mutable memories, tables, and globals; realms, modules within a
realm, and lexical environments; feedback, inline caches, quickening, and any
warmed or profiled state; resolved import bindings, host handles, and host
object references; delegates and capability registrations; the artifact-provider
capability instance; budgets, fuel and time counters, cancellation tokens, and
diagnostics sinks; and any process-local identity. Reachability from a handle to
any of these is a defect, not a configuration.

**Equality, not subsumption, on ceilings.** A runtime with tighter verification
ceilings accepting a handle verified under looser ones would execute an artifact
its own policy would have refused, and section 16 requires the effective policy
recorded in the handle to be the policy applied. Equality also keeps sharing
cheap to reason about: every sharer is under identical ceilings.

| Rejected | Why |
|---|---|
| Element-wise subsumption (handle no looser than runtime) | Safe in principle, but the ceiling recorded in the handle is then not the ceiling applied, and the comparison is a vector whose direction is easy to invert. Recorded instead as the first candidate additive amendment, to be funded by VM-5 numbers if equality proves costly. |
| Share on profile ID alone and let `R` re-intersect ceilings | Silently re-materialises policy after verification and lets a permissively verified artifact run in a restrictive runtime, which invariant 9 forbids. |
| Default `ArtifactSharing` to `Shareable` | Omission meaning the permissive option is the exact failure invariant 9 forbids for limits; the same discipline applies to sharing. |
| A distinct core result category for a sharing refusal | Section 7 fixes the stage categories and section 15 gate 4 requires that composition situations add no core case. Reason codes carry the diagnostic without growing the enum. |
| Compare host-signature assumptions by registration instance | Makes the handle runtime-scoped by construction and captures a host handle into the shared object, which section 6 forbids outright. |

**Forbidden by this ruling:** sharing a handle that is `Draining` or `Disposed`;
any relaxation of ceiling equality in contract version 1; widening a narrowed
`ArtifactSharing`; inferring sharing from a common host registry; any reference
from a shared handle to an item on the exclusion list; adding a result category
for a sharing refusal.

## 5. The verification failure taxonomy

**Four profile-facing failure classes,** and no others, that a profile verifier
may map its own failures onto: `unsupported profile`, `invalid artifact`,
`resource exhaustion`, `cancellation`. A verifier never produces `profile
fault`, `host failure`, or `suspension` in core contract version 1. ADR 0005
fixes the verification stage's row at six categories; the two a verifier can
never produce are `normal`, which is the universal success category, and
`invalid state`, which a verification stage returns like any other stage when it
is called on a disposed runtime. The two lists are different things - a stage
row and a profile obligation - and are kept apart deliberately.

Under those classes the core owns a closed reason set. The profile supplies a
typed opaque detail payload and an artifact location; the core never interprets
either, and no profile may extend, alias, or supply the reasons.

| `invalid artifact` reason | Raised when |
|---|---|
| `Truncated` | the input ends inside a structure the format requires to be complete |
| `MalformedEncoding` | a byte sequence does not decode under the accepted format version |
| `UnknownFormatVersion` | the artifact's format version lies outside the profile's supported range |
| `UnknownFeature` | an identifier, section, opcode, or feature lies outside the accepted feature manifest |
| `InconsistentStructure` | sizes, counts, offsets, indexes, cross-references, or section extents disagree |
| `SemanticValidationFailed` | the artifact decodes and is structurally consistent but fails the profile's type, stack, or control-flow validation |
| `DescriptorMismatch` | the artifact's declared identity disagrees with the caller-supplied descriptor, or an artifact-requested limit is not a tightening |
| `UnsatisfiedHostAssumption` | a declared import has no registered capability descriptor with a matching ID, version, and signature |

A `resource exhaustion` result at the verification stage carries no separate
reason vocabulary: its reason is the exhausted dimension name from ADR 0007's
fifteen-dimension table together with the exhausted `VmBudgetScope`. This
replaces the eight ad-hoc reason identifiers the incoming ruling proposed, which
would have drifted from the dimension table the moment either was edited.
`cancellation` carries the single reason `Cancelled`. `unsupported profile`
carries `ProfileNotInCatalog`, whose message names the requested ID only, echoed
verbatim; ADR 0005's containment rule forbids the result to carry a listing of
what the catalog does contain.

**Determinism.** For the same descriptor, verification input, effective
ceilings, registered capability descriptors, and verifier version, the returned
category and reason are identical on every run and in JIT, trimmed, and Native
AOT hosts. Which failure is *determined* follows section 7's required check
order - effective limits are materialised and the declared length is
bounds-checked before anything is read or allocated, so a byte-ceiling failure
precedes a truncation failure. Which of two simultaneously determined outcomes
is *returned* is not decided here: ADR 0005 owns the single precedence order for
every stage, and this record's incoming ruling, which put `invalid artifact` and
`resource exhaustion` ahead of `cancellation` locally, is amended to that order.

**No partial state escapes a failed verification.**

1. A failed verification returns no handle and no out-parameter, and nothing the
   verifier built is observable to anyone. There is no partial handle, no
   handle-carrying-errors, and no retained partial decode on the runtime. This
   is also why `invalid artifact` is a load-stage-only category that can never
   appear in an instantiation or invocation result (ADR 0010).
2. Failure is by returned value. The core does **not** translate an escaping
   profile-verifier exception into any category: it releases core-owned buffers,
   leaves the budget already charged - work was genuinely done, so there is no
   refund-by-throwing - leaves the runtime usable, and lets the exception
   propagate unchanged, so a verifier bug can never masquerade as a malicious
   artifact or hide from the fuzz corpus. Where the core itself detects a
   verifier contract breach it throws `VmCoreDefectException`, which is one of
   the three members of the closed exception set ADR 0005 owns.
3. Repeating a failed verification returns the same result and leaves no
   residue. VM-2's repeated-malformed-corpus evidence and VM-4's plateau
   evidence must show it (deferred to VM-2 and VM-4).

**Forbidden by this ruling:** a profile defining its own verification categories
or reason codes; returning `profile fault`, `host failure`, or `suspension` from
verification; translating a verifier exception into any category; a partial,
error-carrying, or non-executable handle; reporting a missing catalog entry as
`invalid artifact` or a descriptor disagreement as `unsupported profile`; a
verifier loop that neither charges work nor polls cancellation.

## 6. Requirement V-SEP: verification is separable from execution

**V-SEP, a binding requirement of core contract version 1.** For every profile
and every composition, an artifact can be verified into a handle without
instantiating or executing anything, and the result is identical in identity and
failure class to the verification performed on the path to execution. V-SEP is a
release-gate condition under section 15 gate 3, not a design preference, and any
change to it is a numbered amendment.

Its mechanism is structural, and no second tool-shaped surface is created.

| Property | Content |
|---|---|
| One entry point | A single verification member is the only way a handle comes into existence anywhere in the core. Instantiate and invoke accept only a handle and contain no verification path of their own. Verifying without running is calling the one member and not calling instantiate. |
| Sufficiency | Verification requires only a runtime and its resource authority - never an instance, an event loop, a scheduler, a thread other than the caller's, or an artifact provider. |
| Totality | Verification is a total function of (descriptor, verification input, profile verifier version, effective ceilings, registered host-capability descriptors). It may not read executor state, instance state, or anything a runtime accumulates. |
| Capability isolation | A verifier may READ capability descriptors - IDs, versions, signatures - and record them as identity component 7. It may NEVER INVOKE a capability of any kind, so a composition can verify successfully with capability implementations that are absent or that throw. |
| Provider isolation | No guest-initiated load may occur during verification. While a profile verifier frame is on the stack the core refuses every provider request deterministically (ADR 0008), so re-entrant verification is structurally impossible independently of the configured depth bound. Import resolution, linking, and specifier resolution belong to instantiation and execution. |
| Lazy executor | A runtime creates a profile executor at first instantiation, never at runtime creation, so a verify-only host provably never materialises one. |

**A verify-only host still creates a runtime.** V-SEP means without
instantiating or executing, not without a runtime: ceilings and capability
descriptors come from the runtime. This is stated so that a static verification
member is not proposed later as the "obvious" simplification.

**Why it is worth this much structure.** Section 10 makes the payoff explicit -
a build-time reimplementation that is merely supposed to agree with the runtime
is a security defect with a schedule attached - and section 16 makes a second
verifier a stop condition. V-SEP holds "one verifier, ever" by removing the
motive rather than by policing: an embedder validating a cached artifact, a CI
checker, and any future out-of-host compile step all reach the same member, so
none of them has a reason to build a second verifier.

**Violations, each separately prohibited and separately testable once there is
code to test (deferred to VM-1 and VM-2):** a second verification or
validate-only entry point, however named or scoped; any build-time or offline
reimplementation of a profile's verifier; moving a check out of verification
into instantiation or into first execution - contract version 1 admits no
exception, because ADR 0010 excluded lazy per-section verification; a verifier
that requires an executor, an instance, a realm, or a capability invocation; an
instantiate or invoke overload that accepts bytes or a descriptor; and any
"trusted" path that produces an executable handle without running the profile
verifier.

| Rejected | Why |
|---|---|
| A dedicated validate-only or tool API beside verification | Section 11 states plainly that no second tool-shaped API is designed for this. Two entry points immediately raise the question of whether they agree, which is the one-verifier problem in miniature. |
| Record separability as a documented property with no enforcing structure | Invariant 8 and section 14 both reject documentation-only satisfaction of a capability claim; without the single-entry-point, lazy-executor, and no-provider rules nothing detects drift. |
| Let a verifier call capabilities to resolve imports or fold constants eagerly | It makes an artifact's verifiability depend on live host state, so the same bytes verify differently on two machines, and it lets a handle capture a host binding. |
| Create the executor at runtime creation for simplicity | A verify-only host would pay for and root an executor it never uses, and the cheapest observable proof that verification does not need execution would be gone. |

## 7. Cache-key inputs, runtime-identity inputs, and the persisted-envelope key

The two in-process sets are defined by what they affect, which is the assignment
section 7 leaves open when it says these properties are "part of the cache key
or runtime identity where they affect semantics".

- A **cache-key input** is anything a verifier or compiler may specialise on, so
  that bytes produced or verified under one value must not be reused under
  another.
- A **runtime-identity input** is anything that changes what a runtime may do or
  may share, so that a handle is admitted into a runtime only where it matches.

**Cache-key inputs:** for every import the artifact binds, the ordered tuple
`(CapabilityId, Version, SignatureId, Kind, Reentrancy, ExceptionTranslation,
OptionalImportBound)`; and, per section 6, for a guest-initiated load, the
provider identity, the provider capability version, and the resolved artifact
identity. Reentrancy and exception-translation mode are keys because both change
the legal control flow at a call site, and a profile may inline or elide a guard
depending on them. Optional-import boundness is a key because a profile may
compile a different path when an optional import is absent; without it both
variants collide on one key, which is a correctness bug in the cache rather than
a performance issue. Registered-but-unimported capabilities are excluded, so an
unrelated composition change does not invalidate every entry.

**Runtime-identity inputs:** the whole cache-key set, plus the effective budget
policy recorded in the handle, plus the aggregate-budget parent's identity where
there is one, plus the identity of a capability instance where the host bound
the same instance into more than one runtime, plus the profile's declared thread
affinity - `ThreadAffinity`, closed at `Agile` and `OperationThreadPinned` in
version 1 (ADR 0004, `0004-lifecycle-and-state-machine.md`) - which enters
runtime identity at its declared value, because that value governs which thread
may drive an operation. Cancellation observability at a capability boundary is a
runtime-identity input and not a cache-key input: it changes what a runtime may
do, not what the bytes mean.

**Matching is equality, not compatibility,** in both sets, for the reason given
in section 4.

**The persisted-envelope key is a separate, narrower, closed set.** It contains
only facts that are properties of the bytes and of the composition.

| Fact | In-process handle identity | Persisted-envelope key |
|---|---|---|
| `ProfileDescriptorIdentity` | yes | yes |
| `AcceptedProfileFormatVersion` | yes | yes |
| `FeatureManifestIdentity` | yes | yes |
| `VerifierSemanticVersion` | yes | yes |
| `CoreContractVersion` | yes | yes |
| Per-import capability tuples (component 7) | yes | yes |
| Artifact content hash | no - the bytes themselves are the input | yes |
| The profile's declared hard maxima | no - subsumed by `EffectiveCeilings` | yes |
| `EffectiveCeilings` | yes | **no** |
| `VerifiedArtifactInstanceId`, runtime identity, aggregate-budget root | no | **no** |

The exclusion of `EffectiveCeilings` is what makes invariant 5's final sentence
true by construction. A nested handle's ceilings snapshot the requesting
operation's remaining fuel, wall-clock, and allocation allowance, which is a
timing-dependent process-local quantity; persisting it would carry a
process-local identity into a persisted artifact and produce a key that never
recurs. The statement that cache-key inputs are "part of any persisted envelope
key" is therefore corrected here to: they are part of the **in-process** cache
key, and the persisted-envelope key is the narrower closed set above.

**A `GuestInitiated`-origin handle is ineligible for any persisted envelope**
and may not contribute to any persisted cache key. Origin is the sufficient
discriminator because the remainder snapshot is taken only on the nested
verification path; ADR 0008 states that equivalence and asserts both this rule
and the sharing gate of section 4 at the single choke point that resolves a
handle for a runtime, extended to a persistence gate. No handle field is added.

**Correctness does not depend on ceilings being in the key.** Loading always
re-verifies (section 6), and re-verification recomputes effective ceilings
against the loading runtime, so a persisted artifact never carries a ceiling
decision forward. No invariant is amended by any of this, and none is amended by
this record at all.

Persistence itself remains admitted by contract version 1 and implemented by no
release: no envelope member exists in the public API baseline. ADR 0010 owns
that exclusion as EX-25, and the envelope schema is deferred to VM-6.

## Rulings this record amends

The VM-0 arbitration narrowed or overturned several of the rulings assigned
here. Each is recorded once, not re-argued.

| Incoming ruling | As amended |
|---|---|
| Local outcome precedence at the verification stage | Struck; ADR 0005 owns one precedence order for every stage. |
| Any foreign-runtime identity mismatch is `unsupported profile` | Narrowed to clause 2 of the sharing predicate; every other refusal is `invalid state` with a reason. |
| Handle record fields include `EffectiveSectionVerificationMode` and `ProducedBy` | Both struck; the non-compared field list is exactly five members. |
| A `RemainderDerivedCeilings` flag on nested handles | Struck; `VmArtifactOrigin == GuestInitiated` is that set. |
| Deferred-body memoisation approved in version 1 | Struck with lazy per-section verification (ADR 0010); no memoisation use is approved. |
| Eight ad-hoc `resource exhaustion` reason identifiers | Replaced by the exhausted dimension name plus `VmBudgetScope` (ADR 0007). |
| Four-field host-signature assumption tuple | Widened to seven fields, adding Reentrancy, ExceptionTranslation, and optional-import boundness. |
| Cache-key inputs are also persisted-envelope key inputs | Corrected: the persisted-envelope key is the narrower closed set in section 7. |
| Names `VerifiedArtifact`, `ArtifactRepresentationKind`, `LifetimeKind` | Qualified to `VmVerifiedArtifact`, `VmArtifactRepresentationKind`, `VmArtifactLifetimeKind`, so the Vm/IVm rule holds on its first day. `VmArtifactOwnershipKind` and the state name `DisposeRequested` are retired outright, the latter in favour of `VmVerifiedArtifactState.Draining`. |
| The descriptor has "exactly fifteen fields" | The count is struck; ADR 0002 owns one descriptor field table that is the union of every contract-bearing record's requirements. |

## What VM-0 does not prove

Nothing in this record is enforced or proven at VM-0, and the ledger's update
rule 4 forbids reading it as more than it is. The architecture-rule register
(exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/rules.register.json) holds no row owned
by this record, and this record mints no rule identifier: every check below
needs a subject that VM-0 deliberately does not build. Exclusion identifiers are
allocated by ADR and none is allocated to this record, so the items below are
stated in words and, where an allocated identifier already covers the fact, that
identifier is cited rather than duplicated.

| Check this record implies | Earliest milestone with a subject |
|---|---|
| The verification path declares no retainable buffer type, and `ReadOnlySpan<byte>` is the only byte form | VM-1 (contract types) |
| The handle type is sealed, field-free, constructor-free, and reaches no runtime, instance, capability, delegate, cancellation-token, or diagnostics type | VM-1 |
| The identity type has exactly the seven components with value equality and no exclusion escape hatch | VM-1 |
| `VmVerifiedArtifactState`, `VmArtifactLifetimeKind`, `VmArtifactOrigin`, and the refusal-reason set declare exactly the members frozen here | VM-1 |
| Exactly one public member returns the handle type, and no instantiate or invoke member takes a descriptor or a byte span | VM-1 |
| The verification call graph reaches no capability-invocation member, and no executor is created at runtime creation | VM-1 |
| The sharing predicate refuses at each clause with its own reason, and the mutate, dispose, and concurrent-overwrite corpus cannot change a verified handle | VM-2 |
| Two concurrent runtimes share one handle with no cross-runtime leakage, `Draining` always resolves, and the memory plateau holds | VM-4 |

Three further facts are recorded here rather than tested anywhere. The count and
identifiers of every vacuous and deferred
architecture rule are recorded once, by ADR 0001
(`0001-component-topology-and-dependency-graph.md`), as EX-05; this record adds
no rule to either list. Persistence, including the envelope key frozen in
section 7, is admitted by contract and absent from the public API baseline,
which ADR 0010 records as EX-25. And no reviewer has accepted any of it: the
core-contract role that would approve this record is vacant, which is why its
status is `Proposed`, and why VM-0 stands as in progress rather than accepted in
ADR 0012 (`0012-security-ownership-and-support-matrix.md`).

## Consequences

**For the roadmap.** This record supersedes no illustrative snippet in
roadmap.md. It proposes one wording change - section 6's conditional lease
clause becomes unconditional - which is carried as a proposed, not applied, row
in ADR 0003's amendment register; VM-0 applies exactly one roadmap amendment and
this is not it. No invariant is amended. Invariant 3 needed no amendment for
these rulings: its own closing sentences already place guest-obtained bytes and
nested handles in scope, and the absence of a required amendment is itself the
evidence that the conservative branch was the compatible one.

**For sibling records.** ADR 0002's descriptor table is the single authority for
descriptor field names, and it carries the four fields whose semantics this
record owns: `DescriptorRevision`, `ArtifactRepresentationKind`,
`ArtifactLifetimeKind`, and `ArtifactSharing` (`{Shareable, RuntimeScoped}`,
default `RuntimeScoped`). This record cites that table and restates no row of
it. ADR 0003's public-name table gains `VmVerifiedArtifact`,
`VmVerifiedArtifactState`, `VmArtifactLifetimeKind`,
`VmArtifactRepresentationKind`, and `VmArtifactOrigin`, and records
`VmArtifactOwnershipKind`, `DisposeRequested`,
`EffectiveSectionVerificationMode`, `ProducedBy`, and `VmHandle` as struck
names. A profile that changes verification-affecting descriptor content without
incrementing `DescriptorRevision` commits a defect.

**For later milestones.** VM-1 must implement the three states and the lease
counter before VM-2 implements verification, because the contract fixes them,
and its fixture profile must include a verify-only test host that never
materialises an executor plus a test that verifies successfully against
capability implementations that throw. VM-2 owns the predicate, the reason sets,
the caller-buffer corpus, and fuzz results labelled by (category, reason) pairs
so cross-host stability is mechanically checkable. VM-4 must show two runtimes
sharing one handle, each predicate clause refusing with its own reason, a
`Draining` handle resolving without blocking disposal past its declared bound,
and the plateau. VM-5 must attribute the `Snapshot` copy to core overhead rather
than to the profile. VM-6 keeps the verify-only host in the closure reports and
freezes the envelope schema.

**For hosts.** A handle can outlive the runtime that verified it whenever
another runtime or the host holds a lease; that is intended, and it is possible
only because the handle carries no runtime reference. Callers holding `byte[]`
or `ReadOnlyMemory<byte>` convert at the boundary, because the core ships no
convenience overload. No asynchronous verification member can exist in contract
version 1, since `ReadOnlySpan<byte>` cannot cross an await - consistent with
excluding streaming verification, and no constraint at all on the asynchronous
instantiation admitted by ADR 0009
(`0009-external-suspension-and-async-instantiation.md`). The section 11
code-cache benefit survives, but with a real design constraint worth knowing
before integration rather than after: two realms sharing one artifact need
identical bound-import tuples, identical effective
ceilings, and the same aggregate-budget root, and they may differ only in
per-invocation budgets. A host module cache is therefore built by the host
verifying and caching for itself, because nested handles stay runtime-scoped and
are never handed out.

**Candidate amendments recorded, not minted.** Element-wise ceiling subsumption
in place of equality, to be funded by VM-5 numbers if equality proves costly;
exposing a nested handle to the host; and any eighth identity component. Each
would be a numbered core contract version under ADR 0003, and the last is not
additive.
