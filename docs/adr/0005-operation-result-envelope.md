# ADR 0005 - The Operation-Result Envelope And Payload Ownership

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 7 says that all public stages use a profile-neutral
operation-result envelope "but their legal categories are stage-specific", and
then names categories for three stages. Invariant 10 forbids a profile adding a
case to a core result enum, invariant 4 keeps language semantics out of the
core, and section 16 lists "the core result enum grows one case per language" as
a standing risk with a stop condition behind it. None of that is a contract
until the exact category set, the exact per-stage rows, the mechanism by which
an outcome is reported, and the ownership of the language-shaped part of a
result are all written down and closed.

This record closes them, and only them. Profile identity, catalog entries and
composition-time validation are ADR 0002
(`0002-profile-identity-and-static-catalog.md`). The lifecycle objects, their
states and their legal transitions are ADR 0004
(`0004-lifecycle-and-state-machine.md`). The verified handle is ADR 0006
(`0006-verified-artifact-ownership.md`), the budget dimensions are ADR 0007
(`0007-resource-authority-and-budgets.md`), guest-load mediation is ADR 0008
(`0008-guest-initiated-loads.md`), and suspension origin and resume authority
are ADR 0009 (`0009-external-suspension-and-async-instantiation.md`). This
record is what those six cite when they say "the result".

Nothing decided here executes at VM-0. Milestone VM-0 builds five project
shells and exactly one piece of product code, and roadmap section 13's exit gate
for VM-0 asks for a recorded decision, not an implementation. Invariant 8 and
the status ledger's update rule 4 both forbid presenting a shell as a proven
capability, so every artefact this record names carries its existence marker
here once and is used freely afterwards.

| Artefact | Exists at |
|---|---|
| `VmOutcome`, `VmReason` | (VM-0 decision on paper; no file at VM-0) |
| The seven stage result structs and `IVmOperationResult` | (VM-0 decision on paper; no file at VM-0) |
| `IVmProfilePayload`, `VmPayloadIdentity` | (VM-0 decision on paper; no file at VM-0) |
| `VmDiagnostics`, `VmSourcePosition`, `VmCallerIdentity` | (VM-0 decision on paper; no file at VM-0) |
| `VmHostCallOutcome`, the host exception observer | (VM-0 decision on paper; no file at VM-0) |
| `VmControlResult` | (VM-0 decision on paper; no file at VM-0) |
| `VmCoreDefectException` | (VM-0 decision on paper; no file at VM-0) |
| `VmCatalogValidationException` (owned by ADR 0002) | (VM-0 decision on paper; no file at VM-0) |
| The stage-matrix manifest and the reason-registry manifest | (deferred to VM-1) |
| `src/Broiler.VM.Abstractions/VmCoreContract.cs` | (exists at VM-0: src/Broiler.VM.Abstractions/VmCoreContract.cs) |
| `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` | (exists at VM-0: src/tests/Broiler.VM.Architecture.Tests/rules.register.json) |

The two rules in the register that bear on this record are the ones proving the
result surface is absent rather than present:

Rule E5: The product graph exports exactly one public type,
`Broiler.VM.VmCoreContract`, whose only members are the two contract-version
constants. Status: Active; witness recorded in `rules.register.json` as the
assertion itself, which fails if any product assembly exports a second type.

Rule B4: No exported member of a product assembly names a type outside
`System.*` and `Broiler.VM`. Status: Vacuous at VM-0 - it runs, and nothing in
the VM-0 graph can violate it; it becomes non-vacuous at VM-1 when a product
assembly exports a member with a parameter or return type.

## Decision

Core contract version 1 fixes five things, and an implementer who reads only
this section still cannot choose differently on any of them.

1. **The contract is a return value, never an exception.** Every
   envelope-bearing stage returns its own `readonly struct` by value. No outcome
   is ever observable as a CLR exception at the core's public surface, and the
   exception set that may escape a public member is closed at three types.
2. **There is exactly one outcome enum**, `VmOutcome`, with ten members at
   fixed numeric values, one of which is reserved and never returned.
   Stage-specificity is expressed by which members a stage may return, never by
   a second enum.
3. **There are exactly seven envelope-bearing stages**, each with its own result
   struct whose construction is `internal` and which exposes exactly one factory
   per category legal for that stage. The matrix is therefore enforced by the
   compiler inside the core assembly, not by review.
4. **A language outcome rides as a typed profile payload** implementing
   `IVmProfilePayload` in a nullable slot on the three payload-bearing results.
   The core inspects exactly one thing about a payload - its identity - and
   calls no other member of it, ever.
5. **Control operations are not stages.** `Dispose`, cancellation requests,
   `RequestSuspend`, `PollDeadlines` and lease acquire/release return
   `VmControlResult` with exactly four members - `Accepted`, `NoOp`,
   `InvalidState`, `Unsupported` - carry no `VmOutcome`, no profile payload, and
   appear in no row of the stage matrix.

Point 5 is a new ruling of this record. A control operation consumes no
untrusted input and produces no stage value, so an envelope would be
nine-tenths empty and would place `Suspension`, `ProfileFault` and
`ResourceExhaustion` in rows where they can never occur, which weakens the
matrix as a specification. Four states are exactly what a caller of dispose or
cancel must distinguish. A cancellation's effect is observed as `Cancellation`
on the affected operation; a later use of a disposed object is observed as
`InvalidState`; neither is reported by the control operation itself.

## The outcome set

| Member | Value | Meaning under core contract version 1 |
|---|---|---|
| `None` | 0 | Reserved. Never returned by any stage. It exists so that `default(...)` of any result struct cannot be read as success. |
| `Normal` | 1 | The stage completed and produced its stage value. |
| `UnsupportedProfile` | 2 | A well-formed profile identity was resolved against a composition that cannot host it. |
| `InvalidArtifact` | 3 | Offered bytes are not a well-formed artifact of the identified profile and format version, or a persisted envelope's outer schema, bounds, checksum, atomic-replacement state or migration is not acceptable. |
| `InvalidState` | 4 | The operation is not legal against the target object in its current state. |
| `ProfileFault` | 5 | The profile completed the operation by producing a language-defined fault, or violated the profile contract. |
| `Suspension` | 6 | The operation paused at a declared transition and is resumable through a core-owned single-use `VmSuspension`. |
| `Cancellation` | 7 | A host-requested cancellation was observed at a declared polling point. |
| `ResourceExhaustion` | 8 | One named budget dimension in one named scope had no remaining allowance. |
| `HostFailure` | 9 | A host capability could not be reached, refused, or faulted. |

Values 10 and above are reserved for numbered amendments under roadmap section
2's amendment procedure. Renumbering, renaming, removing or re-scoping any of
the ten is forbidden outright, because persisted envelopes, evidence bundles,
support tables and malformed-corpus expectations all record numeric outcomes and
would become silently wrong.

Roadmap section 7 names a success category only for invocation. This record
extends `Normal` to every stage, so success is one category rather than a
per-stage special case, and the matrix below is representable as a table at all.

Two further rules make the enum load-bearing rather than decorative:

- **A stage's success value is never a bare property.** The catalog-free stage
  values - the runtime, the extracted profile bytes and descriptor, the
  `VmVerifiedArtifact`, the instance, the typed payload - are reachable only
  through a `TryGet...` accessor that returns false unless `Outcome == Normal`.
  A caller that ignores the outcome cannot obtain the value.
- **Every result also carries a `VmReason`**, a core-owned, profile-neutral code
  drawn from a registry that is closed per category. Reason codes are the
  pressure valve that keeps the category set closed; see the amendment section.

An open or extensible outcome type - stable string codes, or an abstract outcome
class a profile may subclass - was rejected: it is section 16's "one case per
language" risk wearing a different shape, section 15 gate 4 could never be
tested against a closed set, and no caller switch could ever be exhaustive even
in principle. Merging `UnsupportedProfile` into `InvalidArtifact` was rejected
because section 7 rejects it explicitly - it misreports a composition mistake as
a corrupt file, the most likely diagnostic error for single-profile products.
Per-stage enums with no shared type were rejected because invariant 10 speaks of
one core result enum, and a shared enum is what makes a cross-stage precedence
rule, one drift test, and one support-table claim possible.

## The seven envelope-bearing stages

Catalog construction is not a stage. A catalog is authored by a composition root
from trusted compile-time data, so a defect there is a wiring bug that must be
loud and unrecoverable: `VmCatalogBuilder.Add` and `VmCatalog.Build` throw
`VmCatalogValidationException`, and no `VmCatalogResult` type exists. That is
ADR 0002's ruling and this record adopts it; the consequence here is that the
matrix names seven stages, not eight, and that `UnsupportedProfile` keeps one
meaning instead of two.

| # | Stage | Normal | UnsupportedProfile | InvalidArtifact | InvalidState | ProfileFault | Suspension | Cancellation | ResourceExhaustion | HostFailure |
|---|---|---|---|---|---|---|---|---|---|---|
| S1 | Runtime creation | yes | - | - | yes | - | - | - | yes | yes |
| S2 | Persisted-envelope preprocessing (reserved) | yes | yes | yes | yes | - | - | yes | yes | - |
| S3 | Caller-driven load and verification | yes | yes | yes | yes | - | - | yes | yes | - |
| S4 | Guest-initiated load (profile-facing) | yes | yes | yes | yes | - | - | yes | yes | yes |
| S5 | Instantiation | yes | yes | - | yes | yes | yes | yes | yes | yes |
| S6 | Invocation | yes | - | - | yes | yes | yes | yes | yes | yes |
| S7 | Resume | yes | - | - | yes | yes | yes | yes | yes | yes |

The negative rules are the part an implementer must not reopen. They are frozen
with their reasons, because a matrix without reasons is re-litigated the first
time a stage is inconvenient.

| Category | Illegal at | Why |
|---|---|---|
| `InvalidArtifact` | S5, S6, S7 | Invariant 3 makes verification complete: a verified handle cannot later become invalid. Admitting it would create a second, later verification point, which is section 16's second-verifier stop condition in miniature. |
| `ProfileFault` | S1, S2, S3, S4 | A verification failure is `InvalidArtifact`, and section 4 requires that no partial state escape a failed verification. Before instantiation there is no profile instance to own a fault. |
| `Suspension` | S1, S2, S3, S4 | A resumable nested verification would let a half-verified artifact outlive its requesting operation, which VM-4's gate ("leaves no partially verified state", "never blocks disposal indefinitely") forbids. A provider that must await suspends the requesting invocation instead. |
| `UnsupportedProfile` | S1, S6, S7 | No profile is resolved at runtime creation, and the profile is already bound once an instance exists. |
| `HostFailure` | S2, S3 | No host capability is invoked on the caller-driven verification path. |
| `Cancellation` | S1 | Runtime creation allocates bounded core structures over trusted host input and reaches no cancellation polling point. |
| `None` | every stage | Reserved; it exists only to poison `default(...)`. |

S7's row is the row of the stage that suspended - S5 or S6 - plus
`InvalidState`, minus `UnsupportedProfile`. The subtraction is deliberate and
corrects an inconsistency in the underlying rulings: `UnsupportedProfile` at
instantiation is an entry check on a handle shared from another runtime, and
that check has already passed by the time an instantiation suspends, so resume
cannot reach it.

**S2 is reserved.** Core contract version 1 freezes the persisted envelope's
stage, ownership split, outer-header field list, failure mapping and
re-verification rule, and core release 1 exposes no envelope member in its
public API baseline. The row is therefore admitted and not implemented, and its
invariant 8 discharge is a deterministic exclusion - absence from the API
baseline - not a returned failure. ADR 0010 (`0010-embedding-decisions.md`) owns
that status and its exclusion identifier; ADR 0003
(`0003-core-contract-v1-and-amendments.md`) records the discharge form. This
record must not be read as promising an envelope reader in release 1.

**S4 is profile-facing only.** A guest-initiated load result is observed by the
profile that requested it through the mediator ADR 0008 owns, and is never
returned to the caller of an invocation. Folding it into S3 and widening S3 to
admit `HostFailure` was rejected: it would make a caller-driven verification
appear able to fail for a host-capability reason that cannot occur on that path,
weakening the matrix as a test oracle and hiding the nesting depth and
requesting-operation identity that section 6's charging rule must keep
auditable.

**Two lists, not one, for verification.** The S3 row has six categories. The
categories a *profile verifier* may map its own failures onto are exactly four -
`UnsupportedProfile`, `InvalidArtifact`, `ResourceExhaustion`, `Cancellation`.
`Normal` and `InvalidState` are core-owned and unreachable from a profile
verifier: verify called on a disposed runtime returns `InvalidState` like every
other stage, and `Normal` is the universal success category. Stating one list
where two are meant is how a profile author ends up believing the core will
accept an outcome it cannot represent.

Treating runtime creation as a throwing composition-time operation outside the
envelope was rejected because section 7 requires that a runtime cannot be
created once a shared aggregate parent has no remaining allowance, which is only
expressible if runtime creation can return `ResourceExhaustion`; two error
mechanisms at one surface guarantee that one of them is untested.

## Where `UnsupportedProfile` and `InvalidState` sit

`UnsupportedProfile` is returned exactly when a syntactically valid profile
identity has been resolved and the composition cannot host it.

| Stage | The condition |
|---|---|
| S2, S3 | The requested profile ID - caller-supplied, or supplied by a checked envelope and confirmed by the caller under invariant 1 - has no catalog entry, or the entry's supported format range or feature-manifest IDs exclude the requested ones. |
| S4 | A guest-requested descriptor names a profile the catalog does not host. |
| S5 | A verified handle shared from another runtime is instantiated in a runtime whose catalog does not host its profile, or hosts it at an excluded version. |

Section 7 contains an internal collision: envelope profile failures are assigned
to `invalid artifact` in one sentence and an absent profile is explicitly not an
invalid artifact in another. This record resolves it **by ownership of the
failing fact, not by which stage observed it**. A persisted-envelope failure is
`InvalidArtifact` when the failing fact is a property of the bytes - magic,
outer schema version, declared lengths against configured bounds, checksum, torn
atomic-replacement state, unsupported migration, or an inner profile ID or
format version that contradicts the descriptor the caller confirmed. It is
`UnsupportedProfile` when the envelope is structurally sound and internally
consistent and the named profile is simply absent from this composition. Reading
section 7's envelope sentence as covering only envelope-internal inconsistency
is the only reading under which both of its sentences are true.

Precedence between those two checks is fixed and not negotiable: all structural,
bound and checksum checks run before profile resolution. A profile ID read out
of unauthenticated corrupt bytes is not trustworthy enough to name in a
diagnostic, and naming it would invert section 7's load-time rule that bounds
are checked before anything is read or allocated from an untrusted declared
value.

`InvalidState` is the one universal category, legal at every stage. It covers
illegal lifecycle transitions, use after disposal or after a terminal fault, a
call on a thread the declared affinity forbids, refused reentrancy, a
`VmSuspension` already consumed or abandoned or belonging to another runtime, a
verified handle presented to a runtime that does not own it, and a profile
payload presented to a runtime that does not own it. Section 7's "one stable
core invalid state outcome" means one category: the specific condition is
carried by a `VmReason`, never by a second category. The registry carries at
least `ObjectDisposed`, `TerminalFault`, `ThreadAffinityViolation`,
`ReentrancyRefused`, `ResumeTokenConsumed`, `ForeignHandle` and
`ForeignPayload`.

Two amendments to the rulings behind this section are recorded rather than
silently applied. First, builder misuse - duplicate profile ID, alias collision,
reserved-namespace violation, a call on a consumed builder - is **not** an
`InvalidState` reason, because catalog construction is not a stage; those are
`VmCatalogValidationException` reasons owned by ADR 0002. Second, an
`UnsupportedProfile` result carries the **requested** identity only, echoed
verbatim, truncated to the 128-character ID bound and escaped in any text
rendering. It never carries a listing of what the catalog does contain. Section
6 routes guest-initiated load failures back through the requesting operation,
where the result is read by untrusted guest code, and a composition's profile
inventory is exactly the reconnaissance a guest should not be handed. The
listing is reachable only through an explicit member on `VmCatalog`, an object
no profile and no guest is ever given, and the containment is a type shape
rather than a policy because a policy cannot be architecture-tested. Section 7's
phrase "naming the requested ID and the catalog's contents" therefore needs the
qualifier "on the host-facing surface only"; that amendment is proposed, not
applied, in ADR 0003's amendment register.

Adding an `InvalidComposition` category for registration failures was rejected:
eleven categories where ten suffice, for a condition already expressible, and
each added category permanently widens what every caller switch and every
support-table claim must cover. Distinguishing use-after-dispose with a
dedicated category or with `ObjectDisposedException` was rejected because
section 7 assigns it to the one stable invalid-state outcome and an exception
would fork the reporting mechanism.

## Precedence and observation order

When two or more conditions hold at one observation point, the reported category
is the first of:

| Order | Category |
|---|---|
| 1 | `InvalidState` |
| 2 | `Cancellation` |
| 3 | `UnsupportedProfile` |
| 4 | `InvalidArtifact` |
| 5 | `ResourceExhaustion` |
| 6 | `HostFailure` |
| 7 | `ProfileFault` |
| 8 | `Suspension` |
| 9 | `Normal` |

This is one order for every stage, including the mediated guest load.
`InvalidState` is first because an illegal call must never be interpreted - the
target may be disposed and its state unreadable. `Cancellation` is second
because the cancellation latch is monotonic and is observed before any input is
examined, which is what makes a cancelled request deterministic; under any order
that ranks `UnsupportedProfile` above it, cancelling a request that names an
absent profile reports one category or the other depending on thread timing,
and section 14 lists a nondeterministic failure class as a release blocker.

Precedence resolves genuine ties. It is not the reason most classifications come
out as they do; **observation order** is. Observation points run in section 7's
load-time order: legality of the call, then profile resolution, then
whole-artifact bound checks, then per-field structural checks, then per-field
bound checks, then allocation. That ordering, not the precedence list, is why an
artifact that is merely too large reports `ResourceExhaustion` against the
`ArtifactBytes` dimension while a field that is both structurally impossible and
over-bound reports `InvalidArtifact`. The mediator adds one observation-point
rule of its own, owned by ADR 0008: provider-not-registered is checked before
every bound and before the request payload is inspected. That is a step order,
not a competing precedence.

The malformed-versus-bounded rule follows: a value that contradicts the format
is `InvalidArtifact`; a value that is well formed but exceeds a configured bound
is `ResourceExhaustion`; where both apply at one point, `InvalidArtifact` wins.
The reason is evidence portability. A classification that does not depend on
host configuration keeps a malformed corpus's expected results identical across
hosts, RIDs and execution modes, which is exactly what VM-2's gate ("the same
failure categories are stable in JIT, trimmed, and Native AOT hosts") requires.
Classifying an over-bound field by the host's configured bound would make the
same bytes classify differently on two hosts, and the corpus would stop being
shared evidence.

`HostFailure` versus `ProfileFault` is not a precedence question but a
control-flow fact: `HostFailure` is reported only where the profile did not
convert the host outcome. A profile that converts it reports `ProfileFault`, and
the core preserves the originating host correlation token in diagnostics so the
causal chain survives the conversion.

Reporting whichever condition the implementation detects first was rejected: it
makes the failure class an artifact of code order, so a refactor changes
recorded evidence and a minimized fuzz case stops being reproducible. A result
carrying a secondary category was rejected: it doubles the assertion surface of
every test and invites callers to branch on the secondary, which reintroduces
category growth pressure by another route.

## Return values, not exceptions

Core contract version 1 is a return-value contract. The core never throws to
report an outcome. Cancellation does not throw: the core accepts a
`CancellationToken` as input, never calls `ThrowIfCancellationRequested` on a
public path, and reports `Cancellation`. Use-after-dispose does not throw
`ObjectDisposedException`; it returns `InvalidState`. The failure path is
allocation-free, because outcome, reason, versions, identities, budget kind and
scope, profile diagnostic code and position are all value types or references
the core already held - so verifying a hostile corpus produces no per-failure
allocation.

The set of exceptions that may escape a public member is closed at exactly
three, and this is the whole list for contract version 1:

| Exception | When | Effect |
|---|---|---|
| `System.ArgumentException` and its derivatives | A pure guard clause, evaluated before any lifecycle object is touched, any state transition, any budget charge, or any byte read | A caller defect, not an outcome. No state changes. |
| `VmCatalogValidationException` | Composition time only: `VmCatalogBuilder.Add` and `VmCatalog.Build` (ADR 0002) | A wiring bug in the composition root. Loud and unrecoverable by design. |
| `VmCoreDefectException` | A core invariant violation the core detected in itself | The affected runtime transitions to the terminal faulted state and the exception is rethrown; every later call on that runtime returns `InvalidState`. |

`OutOfMemoryException` and `StackOverflowException` are process conditions, not
core-raised exceptions, and are outside that set because the core neither
produces nor translates them. The core will not misreport an
`OutOfMemoryException` as `ResourceExhaustion` - that would let a process
condition masquerade as a budget decision, breaking the honesty of invariant 9's
accounting and making VM-4's aggregate-budget evidence uninterpretable. That
rule holds at every boundary, the host-capability catch described below
included: no budget dimension, no `ResourceExhaustion` reason code and no
carve-out anywhere in the contract permits a caught `OutOfMemoryException` to be
reported as a budget outcome. A `StackOverflowException` is uncatchable by
construction, and the call-depth ceiling exists to keep it unreachable through
guest work.

**Host boundary.** Deliberate host behaviour is a return value, not an
exception: a host capability delegate returns `VmHostCallOutcome` of
`Completed`, `Refused` or `Unavailable`, so a policy refusal costs no throw. The
core wraps each host capability invocation in the narrowest possible try/catch,
placed immediately outside the host delegate, so no host exception unwinds
through profile or core frames. Section 7's requirement that "host exceptions
cannot tear down or corrupt another runtime" is only enforceable at that
placement. Translation is fixed: an `OperationCanceledException` carrying the
operation's own token becomes `Cancellation`; anything else - an
`OutOfMemoryException` thrown by the host delegate included - becomes
`HostFailure` with reason `HostCapabilityFaulted`. The exception object is
handed to an observer the host registers at runtime creation and is never placed
in a result.

**Profile boundary.** A language fault is a returned typed payload, never a CLR
exception. The core wraps every call into a profile **executor**; an escaping
exception there is a profile contract violation reported as `ProfileFault`
with reason `ProfileContractViolation`, an empty payload slot, and a terminal
non-resumable instance.

**A profile verifier is the exception, and this record previously said the
opposite.** An escaping exception from a verifier is **not** translated into any
category: the core releases its own buffers, leaves the budget already charged,
leaves the runtime usable, and lets the exception propagate unchanged - so a
verifier bug can never masquerade as a malicious artifact or hide from the
malformed corpus. No handle and no partial state escape either way. ADR 0006
section 5 clause 2 owns that ruling, `VmVerification` implements it, and this
paragraph is corrected to match both rather than to compete with them. A profile whose internals use exceptions, such as an engine adapted
under roadmap section 9's seeding conditions, catches at its own adapter. That
is the concrete discharge of section 9's requirement that the contract be
reachable by code that was not written for it.

**Asynchrony.** No stage returns `Task`, `Task<T>`, `ValueTask<T>` or
`IAsyncEnumerable<T>` in contract version 1. Asynchronous instantiation and
asynchronous host work are expressed by `Suspension` plus an explicit resume, so
the host keeps its scheduler and its event loop, which section 11 assigns to it.

An exception-based contract was rejected on evidence grounds, not taste:
exception type identity, messages and stack traces degrade under trimming and
Native AOT in ways an enum does not, so VM-2's cross-mode stability gate could
not be met without pinning the metadata the trimmer exists to remove; and an
exception allocates and captures a trace on the highest-volume failure path,
which section 16's malicious-input row and VM-2's "fail before execution without
out-of-budget allocation" make a direct cost. A hybrid - exceptions for
programmer errors, results for guest outcomes - was rejected because the two are
not separable at this boundary: section 7 assigns use-after-dispose, a caller
defect, to a returned category, so a hybrid forces every caller to write both a
switch and a try/catch, and under fuzz one of them is always wrong.
`Task`-returning stages were rejected because they import a scheduler and a
synchronization context into a semantics-neutral component, allocate a state
machine per operation on the AOT path, and turn cancellation back into an
exception - all to express what `Suspension` plus resume already expresses.

## The result types

One `readonly struct` per stage, seven in total:

| Stage | Type | Success accessor | Payload-bearing |
|---|---|---|---|
| S1 | `VmRuntimeCreationResult` | the created `VmRuntime` | no |
| S2 | `VmEnvelopeReadResult` | extracted profile bytes plus a descriptor, never a handle | no |
| S3 | `VmVerificationResult` | the `VmVerifiedArtifact` | no |
| S4 | `VmGuestLoadResult` | the nested `VmVerifiedArtifact` | no |
| S5 | `VmInstantiationResult` | the instance | yes |
| S6 | `VmInvocationResult` | the typed payload | yes |
| S7 | `VmResumeResult` | the suspended stage's success value | yes |

All seven share `VmOutcome`, `VmReason` and `VmDiagnostics`, and all satisfy the
constraint-only interface `IVmOperationResult`. Each exposes `Outcome`,
`Reason`, `Diagnostics`, `IsSuccess`, `IsSuspended` and one `TryGet...`
accessor; the three payload-bearing types additionally expose `PayloadIdentity`
and `TryGetPayload<T>`. Construction is `internal`, and each type exposes
exactly one factory per category legal for that stage and no others, so the
matrix is a compile-time fact inside the core assembly: `VmVerificationResult`
has no way to be constructed as `ProfileFault`, and a VM-1 implementer who has
not read this record still cannot violate a negative rule. Results are forwarded
by `in` wherever they are passed on; no core API returns a result by reference,
and no core field, collection or cache holds one.

`VmResumeResult` is one type, not two. It carries a `SuspendedStage` field and
re-exposes the suspended stage's success accessors, so a host loop holding a
`VmSuspension` can resume it without statically knowing which stage produced it
- and VM-4 must test exactly the case where a diagnostic client abandons a
paused operation, where that knowledge is not available.

One universal `VmResult` was rejected: it turns every negative matrix rule into
an unenforced convention and forces every result to carry every stage's success
slot. Class-based results were rejected: they allocate per operation including
on the failure path, and invite null.

## Typed profile payloads

A profile payload is a **reference type** implementing `IVmProfilePayload`,
whose single member is `VmPayloadIdentity Identity { get; }`, where
`VmPayloadIdentity` is `{ VmProfileId ProfileId, int PayloadKindId, int
PayloadSchemaVersion }`. `PayloadKindId` is profile-defined and opaque; the
profile descriptor declares the closed range of kind IDs it may use, and the
core validates membership in that range and attaches no meaning to any value.
Range-checking is mechanism; interpreting a value would be semantics, which
roadmap section 8 forbids the core to share.

The slot is occupied only as follows:

| Result category | Payload slot |
|---|---|
| `Normal` at S5, S6, S7 | may carry a payload |
| `ProfileFault` at S5, S6, S7 | may carry a payload |
| `Suspension` of origin `Guest` or `Instantiation` | may carry a payload, and carries the `VmSuspension` |
| `Suspension` of origin `External` | empty; carries the origin and the operation identity, and no `VmSuspension` (ADR 0009) |
| `UnsupportedProfile`, `InvalidArtifact`, `InvalidState`, `Cancellation`, `ResourceExhaustion`, `HostFailure` | always empty |
| Every S1-S4 result, at every category | no slot exists on the type at all |

An instantiation-origin suspension carries a payload because it is the profile
parking its own partially built instance; forbidding it would force the profile
to hide the projection in mutable state the core cannot account for, which
invariant 5 forbids. An external-origin suspension carries none because the
profile did not choose the yield, and section 7 says external suspension "cannot
be used to observe state the profile does not expose" - a result with no slot is
structurally incapable of leaking it.

The core may inspect exactly one thing about a payload, `Identity`, and only for
three purposes: rejecting a payload whose `ProfileId` is not the runtime's bound
profile or whose `PayloadKindId` is outside the descriptor's declared range
(reported as `ProfileFault` with reason `ProfileContractViolation`, payload
discarded), recording the identity in diagnostics, and enforcing the empty-slot
rules above. The core never invokes any other member of a payload, never calls
`ToString`, `GetHashCode` or `Equals` on it, never pattern-matches or switches
on its concrete type, never stores it in any collection that outlives the
returning call, never clones or pools it, never serialises it, and never
converts a payload of one profile into a value of another. The `ProfileId` check
is the mechanical enforcement of section 1's non-goal "an implied invocation
bridge between two profiles hosted in the same process": that non-goal becomes
testable in VM-1 with two fixture profiles rather than remaining an intention.

Ownership: the payload belongs to the instance that produced it. The core does
not retain it. Disposing the producing instance leaves the result struct fully
readable - outcome, reason, identity, diagnostics - while the meaning of the
payload's contents after disposal is profile-owned and must be documented per
profile. Presenting a payload to a core API belonging to a different runtime
returns `InvalidState` with reason `ForeignPayload`.

Carrying the payload as `object?` was rejected: it allows any object, including
host state, to be smuggled through a core result, and loses the identity check.
Struct payloads were rejected: they box on storage anyway and force the
projection generic to be instantiated per profile value type - code the core
cannot statically root, because it does not reference the profile. Letting the
core cache or pool payloads was rejected: it would require the core to reason
about payload lifetime and equality, which is profile semantics, and would let a
payload outlive its owning instance. A partial payload on `ResourceExhaustion`
or `Cancellation` was rejected: the profile did not complete a result, and a
partial one would give a profile a place to launder a budget outcome.

**No failed load carries a projectable payload.** `InvalidArtifact` and
`UnsupportedProfile` never carry one at any stage, and no S1-S4 result carries
one at any category. A failed verification carries only the profile's stable
32-bit diagnostic code and an opaque position token, both value types the core
does not interpret, alongside the core reason code. At verification time there
is no profile instance, realm or value world to own a language error value, so
constructing one would require the core to invent semantics that invariant 4
keeps out of it - and invariant 3 requires that no partial state escape a failed
verification. A host or profile that wants a language-shaped error constructs it
inside an instance, from the diagnostic code and position: a browser adapter
builds its `SyntaxError` in a realm, which is where the language's error
semantics already live. Allowing a payload only when the profile marks it "safe"
was rejected: a safety claim asserted by the component whose input is untrusted
is not evidence, it is the shape-only stub invariant 8 rejects.

## Projecting a payload

Projection has exactly two shapes and no others.

**(a) The core-side generic accessor**, declared once on each payload-bearing
result struct: `public bool TryGetPayload<TPayload>(out TPayload payload) where
TPayload : class, IVmProfilePayload`, implemented as an outcome check followed
by a plain `as` cast. The `class` constraint is mandatory and load-bearing: a
generic method instantiated only over reference types compiles to one canonical
shared body, so no per-profile generic code must be generated, rooted or
discovered, and the accessor is correct even for a profile type the core has
never referenced.

**(b) The profile-side static accessor**, shipped by the profile's own package -
for example `public static bool TryGetValue(in VmInvocationResult result, out
FixtureValue value)`. It is trivially rooted because the composition root
already references the profile package, and it checks the full
`VmPayloadIdentity` before casting, so a payload-kind or schema change across
profile versions fails closed rather than casting successfully into a stale
expectation. This is the shape profile documentation must recommend.

Native AOT rules, frozen for the whole result surface:

- No public core API declares a generic virtual method, or a generic interface
  method whose type argument can be a profile type.
- No core code path uses `MakeGenericType`, `MakeGenericMethod`,
  `Activator.CreateInstance`, `Type.GetType`, `Delegate.CreateDelegate` or
  `Unsafe.As` to reach or coerce a payload.
- Generic helpers over stage results are constrained `where T : struct,
  IVmOperationResult`, and every instantiation is over one of the seven
  core-owned structs declared in the same assembly, so all struct instantiations
  are statically reachable from the core itself.
- `IVmOperationResult` exists only as a generic constraint and is never a
  storage type, so no stage result is ever boxed by the core.
- The payload slot is typed `IVmProfilePayload?`, never `object?`, so a wrong
  cast fails deterministically rather than plausibly.

The consequence, stated plainly: **the core must never require an instantiation
it cannot see.** Reference-type projection needs no instantiation at all,
profile-side projection is rooted by the composition root, and the only
value-type generics in the surface are closed over core-owned types. Together
those mean the result surface imposes zero AOT rooting obligations on a profile
author; the only rooting obligations that remain are the descriptor and factory
section 3 already imposes.

A generic envelope `VmResult<TPayload>` threaded through the stage APIs was
rejected: it makes the stage signature depend on the profile type, forces
per-profile instantiation of core code the core cannot root, and breaks down the
moment one composition hosts two profiles or a stage returns a failure with no
payload type in scope. A generic virtual `Accept<TVisitor, TResult>` visitor was
rejected: generic virtual methods are the AOT shape most likely to need runtime
instantiation the trimmer cannot see, and it would make the core hold a dispatch
mechanism over profile types, which is a language case by another name. A source
generator emitting a per-profile projection into the core was rejected outright:
it would make the core reference a concrete profile, which section 5 and section
14's dependency-architecture row forbid.

## Diagnostics and secret safety

Every result carries a `VmDiagnostics` readonly struct. Its field set is frozen
and reserved explicitly, because adding a field later is additive for callers
reading fields but changes the struct's size and layout.

| Group | Fields |
|---|---|
| 1 | `CoreContractVersion`, `ReasonRegistryRevision` |
| 2 | `Stage`, `Outcome`, `Reason` |
| 3 | `RuntimeId` (process-unique, never a pointer), `OperationId` (monotonic per runtime), and `RequestingOperationId` plus `NestingDepth` for a guest-initiated load or any work performed under one |
| 4 | Profile identity where resolved: `ProfileId`, `ProfileFormatVersion`, `FeatureManifestId`, `VerifierSemanticVersion`. Where unresolved because the outcome is `UnsupportedProfile`: the requested identity only |
| 5 | Artifact identity where an artifact exists: the handle's core-assigned `ArtifactId`, stable for the life of the handle; the artifact byte length; and the caller-supplied canonical source/module identity, echoed verbatim and flagged `CallerSupplied` |
| 6 | `VmSourcePosition` where the profile supplies one: an opaque profile-owned value of section index, byte offset and two profile-defined 32-bit coordinates |
| 7 | For `ResourceExhaustion`: the budget dimension and the scope in which it was exhausted |
| 8 | For `HostFailure`: the capability ID, the capability version, and an opaque 128-bit host correlation token |
| 9 | For `InvalidArtifact`: the profile's stable 32-bit diagnostic code, alongside the core reason code |

Every field is a value type or a reference the core already held, so
`VmDiagnostics` allocates nothing. The core stores and returns a
`VmSourcePosition` and never parses, orders, formats or compares it: line and
column are language artifacts, and a core that ordered or formatted them would
be owning a semantic that invariant 4 keeps out. Group 7 names the dimension and
the scope from ADR 0007's table; it never crosses an absolute ceiling or an
absolute consumption figure to a profile.

The normative actionability rule: **a result must, on its own, answer which
contract version produced it, which stage, which profile and version, which
artifact, where in the artifact, whose budget and which host capability -
without the caller re-running the operation.**

Secret safety is stated as an allowlist, because a prohibition stated as an
instruction to be careful is not mechanically testable. Exactly four content
classes may appear in a result:

| Class | Content |
|---|---|
| Core-composed | Values drawn from closed core vocabularies: enums, numeric IDs, versions |
| Caller-echoed | The descriptor's canonical source/module identity, returned verbatim, never parsed and never logged by the core, flagged `CallerSupplied` so a diagnostics sink can redact it |
| Profile-supplied | The profile's 32-bit diagnostic code and its opaque position token |
| Host-supplied | An opaque 128-bit correlation token only, which the host resolves against its own log |

Nothing else. No result field, and no string the core produces, may contain a
filesystem path, a URI, a host name, a user or machine name, an environment
variable, a process or thread ID, a memory address, a managed or native stack
trace, a CLR type name outside `Broiler.VM.*`, an exception object, message or
`ToString()` produced outside the core, a host capability's argument or return
value, or any absolute resource ceiling or absolute consumption figure crossed
to a profile. `VmDiagnostics` exposes no `System.String` other than the profile
ID and the caller-echoed identity, and human-readable text is produced by a
caller-side formatter over the structured fields - which is also what makes
trimmed and Native AOT diagnostics identical to the JIT build.

Host exception detail is not lost; it takes a different road. The observer the
host registers at runtime creation is invoked inside the host's own trust domain
with the original exception object. The core does not format it, store it, or
place it in a result. That separation is what lets the rule be absolute without
making host capability failures undebuggable.

**A result is host-facing and is never handed to guest code.** A profile may not
surface a core result to guest code; where a profile wants a language-visible
error it constructs its own language value from the profile-owned parts. Even
correct fields - artifact identity, budget scopes, capability IDs - are host
configuration that guest code has no claim on.

Including the host exception's message "for convenience" was rejected: such
messages routinely contain paths, URIs, connection strings and user identifiers,
and are not stable across trimming, so it also breaks VM-2's cross-mode
stability requirement. Redacting on the way out with a scrubber was rejected: a
denylist over free-form text is unbounded and untestable, whereas an allowlist
over structured fields is a finite architecture test.

## How a guest-initiated load reaches the caller's result

A guest-initiated load is stage S4 with its own result, observed by the
requesting profile and never returned to the caller. The requesting invocation
then reports according to a fixed mapping, and no category is added by any of
it.

| Nested outcome | Conversion | Requesting operation reports |
|---|---|---|
| `InvalidArtifact` | mandatory | `ProfileFault`. A profile that returns the category unconverted is a contract violation, reported as `ProfileFault` with reason `NestedFailureNotConverted`. |
| `UnsupportedProfile` | mandatory | `ProfileFault`, same rule and same reason on failure to convert. |
| `HostFailure` | optional | `ProfileFault` if converted, otherwise `HostFailure` propagates - which is legal at invocation. Reason `ProviderNotRegistered` where the composition registers no artifact-provider capability. |
| `ResourceExhaustion` | forbidden | `ResourceExhaustion`, propagated unconverted. |
| `Cancellation` | forbidden | `Cancellation`, propagated unconverted. |
| `InvalidState` | forbidden | `InvalidState`, propagated unconverted. |

Conversion of the two artifact-shaped failures is mandatory because that is what
keeps `InvalidArtifact` load-stage-only and keeps a nested artifact's
malformedness a language event inside the guest rather than a claim about the
caller's artifact. `HostFailure` is already legal at invocation, so forcing its
conversion would hide a composition fact the caller owns.

The terminal set is non-convertible and the core enforces it rather than
trusting the profile: on observing `ResourceExhaustion`, `Cancellation` or
`InvalidState` the core **latches** the category on the operation, so whatever
the profile returns afterwards is discarded and the latched category is
reported. Without the latch a profile could catch a budget outcome and return
`Normal` or a catchable language fault, and guest code could swallow the signal
and keep running - which is a nested load enlarging its requesting operation's
budget by another route, and a release blocker under section 14's
artifact-safety row. In every case diagnostics carry `RequestingOperationId` and
`NestingDepth` of at least 1.

Reporting a missing provider as `UnsupportedProfile` was rejected: a missing
capability is not a missing profile, and `UnsupportedProfile` is defined by
resolving a requested profile ID against the catalog, which has no meaning
here. Adding a `NestedLoadFailure` category was rejected: section 7 says a
guest-initiated load adds no category, and the failure is already expressible.

## What an amendment costs

Adding a category is **source-additive and behaviourally breaking**. Existing
profile packages recompile unchanged, because no profile constructs a core
result; but any caller that switches over `VmOutcome` without a default arm
silently mishandles the new value. Roadmap section 2 states the
source-compatible half and not the behavioural half, so contract version 1
states it here, together with the two mitigations that make it survivable:

- The documented caller pattern is `if (!result.IsSuccess) { switch
  (result.Outcome) { ... default: treat as an unknown failure and do not proceed
  } }`, and every sample, test helper and documentation snippet the core ships
  must show the default arm, because samples are how a pattern actually
  propagates.
- Every result exposes `IsSuccess` and `IsSuspended`, so the two decisions a
  caller must always get right do not depend on enumerating categories at all.

Adding a category, adding a category to a stage's legal row, or changing an
existing category's meaning each mint core contract version n+1 under section
2's procedure and trigger the ledger's re-evaluation rules. Removing, renaming,
renumbering or re-scoping an existing category is forbidden outright: the
amendment mechanism is addition only.

Reason codes are what keep the category set closed. They are core-owned,
profile-neutral and closed per category; a profile may neither define nor
contribute one. Adding a reason code inside an existing category is additive,
does **not** bump the core contract version, and increments a separate monotonic
reason-registry revision published beside the contract version. Every category
has a generic reason, and callers must treat an unknown reason code as that
generic reason. Language-specific detail has exactly one destination and it is
not the core: the profile's typed payload for `ProfileFault`, and the profile's
opaque diagnostic code and position for `InvalidArtifact`. A proposal to add a
category for one language is rejected under section 16; if the capability is
genuinely inexpressible it becomes an amendment whose driving capability,
needing profile and rejected profile-owned designs are recorded per section 2.

Making reason codes an open registry profiles may extend was rejected: it
reintroduces per-language growth one level down. Bumping the contract version
for every new reason code was rejected: it would force a full recertification
cycle for a purely additive diagnostic refinement, which in practice discourages
adding reasons and pushes detail back into categories.

## What VM-0 does not prove

Truthfulness beats completeness here. Roadmap section 16 makes an untruthful
support claim a stop condition, and the status ledger's update rule 4 forbids
promoting a shell result, so the limits of this record are stated rather than
implied. None of the following has an exclusion identifier of its own: ADR 0005
holds no identifier block, so where an exclusion is already owned elsewhere it
is cited, and where it is not it is stated in plain words.

| Not proven or enforced at VM-0 | Reason | Closed by |
|---|---|---|
| Every type in this record | The product graph exports exactly one public type and no result type exists; Rule E5 is what makes that checkable | VM-1, which implements the contracts and the result surface |
| The stage matrix as data | It is prose in this record. No manifest is checked in and no drift test runs at VM-0 | VM-1, which ships the matrix manifest and the factory-set drift test |
| The reason registry | Only the codes named here are fixed, and no registry file exists | VM-1, which ships the registry manifest and its monotonic revision |
| Every architecture test this record's rulings called for | The register in `rules.register.json` is closed at 28 rows at VM-0, and each of these rules has no subject to assert against; none carries a rule identifier | The milestone that creates the subject, registering the rule under ADR 0001's register discipline |
| That the failure path allocates nothing | No failure path exists to measure | VM-2 for the malformed corpus, VM-5 for the measured baseline |
| That failure categories are stable across JIT, trimmed and Native AOT hosts | Nothing has been published or run | VM-2's exit gate |
| The persisted-envelope stage (S2) | Admitted by contract version 1 and implemented by no release; no member exists in the public API baseline | ADR 0010, which owns the exclusion and the milestone question |

The test obligations this record places on later milestones, so that they are
not lost between ADR text and test code: exactly one public enum named
`VmOutcome` whose member/value pairs match the manifest; each stage result
struct's internal factory set equal to its manifest row; `IVmOperationResult`
used only as a generic constraint; `IVmProfilePayload` declaring exactly one
member; no public core type deriving from `System.Exception` except
`VmCoreDefectException`; no public stage method returning `Task`, `Task<T>`,
`ValueTask<T>` or `IAsyncEnumerable<T>`; no `throw` of
`OperationCanceledException` or `ObjectDisposedException` in
`Broiler.VM.Runtime`; no public member in `Broiler.VM.*` both virtual and
generic; no `System.Object`-typed public member; no `System.String` member in
the diagnostics surface beyond the profile ID and the caller-echoed identity;
and no S1-S4 result type declaring a payload member.

## Consequences

- **VM-1 cannot begin without this record.** Every stage signature, every
  profile adapter signature and every host capability signature is determined by
  the return-value ruling and the stage matrix. VM-1's fixture profile must
  exercise every legal cell of the matrix and assert that every illegal cell is
  unreachable; that is what makes VM-1's "the accepted contract is recorded with
  its version" a real gate rather than prose.
- **Host code must be written to return `VmHostCallOutcome`.** A host adapting
  existing throwing code writes one adapter, in its own assembly, where its
  exception detail stays. A profile adapted from a throwing legacy codebase
  catches at its own adapter.
- **A profile with a struct value model pays one allocation per non-void
  result.** That is a named, measurable cost for VM-5 to baseline rather than a
  hidden one.
- **A terminal faulted state is required** on the runtime and the instance, and
  every stage's `InvalidState` reason set must include `TerminalFault`. ADR 0004
  owns naming it.
- **The verified handle must expose a stable `ArtifactId`** from VM-2 onwards,
  and the descriptor must carry `VerifierSemanticVersion`. ADR 0006 and ADR 0002
  own those fields.
- **Profiles must publish and version their diagnostic-code registries**,
  because the profile diagnostic code is the sole profile-owned channel out of
  verification, and must document per profile how they convert a nested
  `InvalidArtifact` into their own fault.
- **The support table names two numbers**, the core contract version and the
  reason-registry revision. No such table exists at VM-0; ADR 0012
  (`0012-security-ownership-and-support-matrix.md`) records that and owns the
  exclusion.
- **Human-readable diagnostics are a caller-side concern.** The core ships
  structured fields and a reason vocabulary, never messages; guest-visible error
  text is unambiguously a profile responsibility, which is where section 4
  already puts it.

This record supersedes four illustrative or incomplete statements in the
roadmap. Each supersession is recorded in ADR 0003's amendment register as a
`Proposed`, not applied row, and **this record edits no roadmap text**; the
roadmap and the ADRs therefore disagree in these places until an owner lands the
patch.

| Superseded | By |
|---|---|
| Section 7's three stage bullets, which name categories for load/verification, instantiation and invocation only | The seven-stage matrix above, which adds runtime creation, envelope preprocessing, the guest-initiated load and resume, and extends `Normal` to every stage |
| Section 7's "naming the requested ID and the catalog's contents" | The same, qualified "on the host-facing surface only": no result ever carries a catalog listing |
| Section 7's envelope sentence read as assigning every envelope profile failure to `invalid artifact` | Classification by ownership of the failing fact: `InvalidArtifact` for a property of the bytes, `UnsupportedProfile` for a structurally sound envelope naming an absent profile |
| Section 7 step 7's "transition sessions, instances, and any explicitly disposable verified handles" | The unit is an operation, not a session; the quoted phrase above is the only place this record writes the older word |

Two of this record's decisions are load-bearing for other people's gates and are
worth naming as such. The `ProfileId` check on a payload is the mechanical
enforcement of section 1's two-profile non-goal, so that non-goal becomes
testable in VM-1 rather than remaining an intention. And the single precedence
order plus the fixed observation order are what let VM-2 record one expected
category and one expected reason per malformed-corpus case, portable across
hosts, RIDs and execution modes - which is what section 14's ban on a
nondeterministic failure class actually requires of an implementation.
