# ADR 0011 - The Source-Level Profile Contract, Profile Checklist, And Sharing Rule

**Status:** Proposed
**Date:** 2026-08-27
**Core contract:** version 1 (contract-bearing)

## Context

Broiler.VM hosts language profiles it does not know. Everything a profile
author can rely on is therefore contract, and VM-0 either writes it down or
leaves VM-1 to invent it one commit at a time. This record settles the
profile-facing half of core contract version 1: what a profile may reference
and reach, what roadmap section 4's ten rows oblige the core to provide, the
frozen shape of a typed host capability, the closed set of types that may cross
that boundary, how an exception escaping a host capability is translated, why a
shared host registry cannot bridge two profiles, and the standing rule that
governs every future proposal to move code out of a profile into the core.

None of it is implemented. Broiler.VM.Abstractions
(exists at VM-0: src/Broiler.VM.Abstractions/Broiler.VM.Abstractions.csproj)
exports exactly one public type, `VmCoreContract`
(exists at VM-0: src/Broiler.VM.Abstractions/VmCoreContract.cs), whose only
members are two integer constants; Broiler.VM.Binary
(exists at VM-0: src/Broiler.VM.Binary/Broiler.VM.Binary.csproj) and
Broiler.VM.Runtime
(exists at VM-0: src/Broiler.VM.Runtime/Broiler.VM.Runtime.csproj) export none.
Every type, field, enumeration and table named below is
(VM-0 decision on paper; no file at VM-0). Naming one does not mean a shell
holds it, and no decision in this record is proven by the VM-0 build.

## The source-level profile contract

The supported public contract is a **source-level composition contract**. Five
promises, frozen at core contract version 1.

| # | Promise |
|---|---|
| P1 | A profile is an ordinary `net10.0` project whose Broiler.VM reference set is exactly {Broiler.VM.Abstractions, Broiler.VM.Binary}, by project or package reference, compiled into the application. Its descriptor and factory are named directly by a composition root (deferred to VM-3 for the first advertised root). |
| P2 | The promised surface is exactly the public API of those two assemblies, as captured by the public API baseline (deferred to VM-6). There is no privileged surface: nothing profile-facing is reachable through `InternalsVisibleTo`, a friend assembly, an internal partial, or an unlisted type. If a profile needs it, it is public or it does not exist. |
| P3 | Within one core contract version, a profile that compiles and passes its contract tests against a core package version compiles unchanged, with the same semantics, against any later core package version carrying that same contract version. |
| P4 | Every unsupported case a profile can reach has a named deterministic failure, and the profile may rely on that name (invariant 8). Which of the two discharge forms applies to each artefact is ADR 0003's (`0003-core-contract-v1-and-amendments.md`) admitted-versus-implemented table. |
| P5 | Adding a profile requires no change to the core runtime, the execution loop, or any Broiler-owned package. |

P1 is narrower than the reference set proposed during drafting, which read
"Abstractions, Binary, and the runtime's contract surface", and the narrowing
is deliberate. **No profile package ever references
Broiler.VM.Runtime.** Every profile-facing contract - the verifier and executor
interfaces, the metering surface, `IVmArtifactLoadMediator`, the capability
transfer types, the payload interface, `VmProfileDescriptor` and its factory
delegate - is declared in Broiler.VM.Abstractions; Broiler.VM.Runtime holds
implementations and exposes no profile-facing type. The alternative drags the
runtime into every profile package's dependency closure and makes "the
runtime's contract surface" an unbounded phrase no test can hold.

Two registered rules already hold the shape of P1 and P2 against the shells:

Rule A8: No profile project references Broiler.VM.Runtime. Status: Active;
witness src/tests/Broiler.VM.Architecture.Tests/witnesses/A8-profile-references-runtime.csproj.witness.

Rule A10: No product project declares InternalsVisibleTo. VM-3 must prove a
profile is writable through the public source contract alone. Status: Active;
witness src/tests/Broiler.VM.Architecture.Tests/witnesses/A10-product-internals-visible-to.csproj.witness.

P2 is the load-bearing promise and the easiest to lose. A surface reachable
only through `InternalsVisibleTo` would make the fixture profile
(exists at VM-0: src/tests/Broiler.VM.Fixtures/Broiler.VM.Fixtures.csproj) a
privileged consumer and would silently invalidate VM-3's whole demonstration,
because the consumer profile would be proving a contract the core's own
profiles do not use. That project is a shell at VM-0 and holds no profile. The
cost of P2 is that every profile-facing type is public from the first commit;
the API surface is larger and the VM-6 baseline correspondingly more valuable.

### What "no binary plug-in ABI" means concretely

| # | Clause |
|---|---|
| ABI-1 | The core never loads a profile it was not compiled against: no `Assembly.Load`, `Type.GetType`, assembly scan, `Activator.CreateInstance`, magic type name, extension directory, or module-initializer ordering dependency (invariant 2). |
| ABI-2 | The supported delivery unit is a recompiled composition, not a profile binary. A profile assembly compiled against one core contract version is not supported against another; it is refused at catalog admission by ADR 0002's (`0002-profile-identity-and-static-catalog.md`) version-axis check rather than silently rebound by assembly unification. |
| ABI-3 | The core exposes no member that identifies a profile by assembly path, assembly name, or type-name string, and offers no versioned native or COM entry point, no interface GUID, and no stable vtable. No package implies one. |
| ABI-4 | Consumer compatibility is obtained by rebuilding. An additive amendment promises source compatibility - recompile and republish, no source edit - and never binary compatibility. |
| ABI-5 | Because there is no ABI, the core may freely change internals, generic instantiations, assembly internals and layout across package versions, provided the promised public surface still compiles. |

Rule B5: No assembly in the product graph references a dynamic-loading,
reflection-invocation or IL-emit API. Invariant 2 requires registration to be
static and typed. Status: Active; witness DynamicLoadingWitness in
Broiler.VM.Architecture.Tests.dll.

What may change without minting a core contract version, and what may not:

| Change | Amendment? | Governed instead by |
|---|---|---|
| Public API names, shapes, overloads, defaults, package layout | No | the API baseline (deferred to VM-6) and package versioning |
| Internals, implementation strategy, generic instantiations | No | ABI-5 |
| Adding a reason code inside an existing outcome category | No | ADR 0005's (`0005-operation-result-envelope.md`) reason-registry revision |
| Diagnostic text, performance, the fixture profile, documentation | No | ordinary change control |
| The semantic content of the seven artefact classes of contract version 1 | **Yes** | ADR 0003's amendment procedure |

### How VM-3 demonstrates the promise

The demonstration (deferred to VM-3) is an application-local consumer profile
in a separate project, given a reverse-domain identity outside the reserved
`Broiler.*` namespace, composed by direct typed registration. It is accepted
only with all five of: the consumer project has no `InternalsVisibleTo` grant,
no reflection, and no reference to a fixture or test project; it references only
the promised assemblies; the change that adds it modifies no Broiler-owned
package and leaves the core API baseline byte-identical; single-profile and
two-profile compositions each publish and run under trimming and Native AOT
with warnings as errors, and each closure report contains exactly the declared
profiles and no fixture or test assembly; and a negative demonstration records
that dropping a prebuilt profile assembly beside the application is not a
supported delivery path, evidenced by the absence of any core API that accepts
an assembly, an assembly name, a type name, or a path.

**Rejected.** Promising binary compatibility for profile assemblies across core
package versions: that is a binary plug-in ABI in all but name, excluded by
section 1's non-goals, section 3 and section 15 gate 2, and impossible to
honour while the core is free to change generic instantiations for Native AOT
rooting.
Exposing an internal fast path through `InternalsVisibleTo`: it makes the
promise unprovable, as above. Supporting a dropped-in profile assembly: it
requires discovery or unification, both excluded by invariant 2, and defeats
the exact-closure gates. Deferring the whole promise to VM-6, where the API is
frozen: VM-1 through VM-3 make daily public-versus-internal decisions, so the
surface VM-6 freezes would already contain accidents.

## The profile checklist, frozen as ten obligations

Roadmap section 4's ten rows are frozen as VM-0 obligations. Each row states
what the core must provide and what the fixture profile must demonstrate; VM-1
owns both columns. Nothing language-specific is added: no row names an opcode,
value kind, syntax construct, module system, or language feature. All twenty
cells are (VM-0 decision on paper; no file at VM-0).

| # | Core obligation (VM-1 provides) | Fixture demonstration (VM-1 must show) |
|---|---|---|
| 1 | Broiler.VM.Binary supplies checked-arithmetic readers, variable-length integer decoding, section and segment framing, and an allocation guard that refuses before allocating from an untrusted declared count. | A fixture artifact declaring a count above its configured bound fails before any allocation proportional to that count. |
| 2 | One verification entry point returning either an immutable handle or a failure from a stable taxonomy, with no partial state escaping a failed verification (ADR 0006, `0006-verified-artifact-ownership.md`). | Truncated and structurally corrupt fixture artifacts yield `InvalidArtifact`, and no handle, instance, or observable partial state is produced. |
| 3 | The core stores no values and inspects no frames; profile state is reachable only as profile-typed opaque payloads. | No exported core member is typed in a fixture value or frame type, and the fixture retains every value across the boundary. |
| 4 | Profile-neutral operation-result envelopes carrying typed profile payloads, plus a projection API that adds no language case to the core (ADR 0005). | A fixture-defined fault round-trips as `ProfileFault` plus a typed payload the core never names or interprets. |
| 5 | Guest-initiated suspension always; external suspension only where the descriptor declares it and the runtime enables it, as a transition distinct from cancellation (ADR 0009, `0009-external-suspension-and-async-instantiation.md`). | Guest suspension and resume; external suspension and resume where declared; and a deterministic named refusal where it is not. |
| 6 | Guest-initiated loads only through a declared artifact-provider capability, bounded in depth, fan-out, cumulative bytes and verifier work, and charged to the requesting operation (ADR 0008, `0008-guest-initiated-loads.md`). | A nested load through a fixture provider succeeds within bounds; the same load with no provider registered is refused deterministically; a nested load can neither enlarge nor escape its requesting operation's budget. |
| 7 | Typed, allowlisted, versioned capabilities with declared reentrancy, thread affinity, and exception translation - the descriptor frozen below. | Signature mismatch and unregistered-capability paths fail deterministically; a host exception is translated per the declared mode and cannot tear down or corrupt another runtime. |
| 8 | Accounting a profile charges into and cannot enlarge - at least `Fuel`, `WallClock`, `AllocatedBytes`, `CallDepth` and `HostCalls` from the dimension table ADR 0007 (`0007-resource-authority-and-budgets.md`) owns - with ceilings materialized before untrusted allocation. | An attempt to raise a ceiling fails, an omitted invocation override inherits, an explicit override only tightens, and exhaustion reports `ResourceExhaustion`. |
| 9 | An opaque profile section inside a core-owned bounded outer envelope: the core owns outer schema, dispatch, byte ownership, atomicity and corruption reporting; the profile owns payload, cache-key contribution and migration (ADR 0010, `0010-embedding-decisions.md`). | A corrupt outer header yields `InvalidArtifact`, outer compatibility does not imply payload compatibility, and loading always re-verifies. |
| 10 | A direct descriptor and factory delegate, rooted for trimming and Native AOT, producing a reviewable closure. | Trimmed and Native AOT test hosts construct the fixture profile through the generic contract, and no product composition's closure report contains a fixture or test assembly. |

Row 9's obligation is the **ownership split**, not the shipping of storage.
Whether release 1 implements the envelope is recorded in ADR 0003's
admitted-versus-implemented table; dropping the row because release 1 ships no
storage would leave the split renegotiable, which section 14's
persistence-ownership blocker forbids.

**Cross-cutting condition on all ten rows.** The fixture profile's adapter is
shaped after a non-trivial existing runtime rather than a contract-shaped toy,
so the contract is reachable by code that was not written for it (section 9's
seeding condition; section 16's risk that a core designed with no real profile
fits no real profile).

A profile requirement that maps onto no row is a contract defect. It produces
an amendment proposal or a recorded refusal under ADR 0003's procedure. It is
never worked around inside the core's execution loop, and it is never proven
against a product profile in place of the fixture and application-local ones
(invariant 13).

Rule A4: No product project has a ProjectReference resolving under src/tests/.
Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A4-product-references-test.csproj.witness.

**Rejected.** Leaving the checklist as descriptive prose: section 4 says VM-1
proves each item, and a row without a named demonstration is unfalsifiable, so
VM-1's gate would be argued rather than checked. Adding rows for what the
intended profiles are expected to need - module graphs, generators, linkers:
those are section 9 expectations, not core obligations, and importing them puts
language concepts into the core's own checklist.

## The host-capability descriptor

A host capability is described by a fixed seven-field descriptor
(VM-0 decision on paper; no file at VM-0) in contract version 1. Adding a
field, or a member to any closed set below, is a numbered amendment.

| # | Field | Domain | Frozen rule |
|---|---|---|---|
| F1 | `CapabilityId` | stable, non-localized, namespaced string | `Broiler.*` is reserved; application-local capabilities use a documented reverse-domain namespace, mirroring the profile-ID policy of ADR 0002 exactly. |
| F2 | `Version` | single integer | An import names ONE EXACT version. No range, no "or later", no negotiation. A host supporting two versions registers two capabilities. |
| F3 | `SignatureId` | stable identifier of the parameter and return shape | Derived from a canonical description that both the host registration and the profile import declare. A mismatch is refused at binding, never at first call. |
| F4 | `Kind` | closed: `Value`, `ArtifactProvider` | Registering `Value` capabilities never implies an `ArtifactProvider`. ADR 0008 owns the provider's own shape. |
| F5 | `Reentrancy` | closed: `NonReentrant`, `ReentrantIntoInvokingRuntime` | Enforced, not merely declared: the core holds a per-runtime in-capability flag for the duration of the call and refuses a re-entrant public call with `InvalidState` / `ReentrancyRefused` where the capability declared `NonReentrant`. |
| F6 | `ThreadAffinity` | closed: `CallerThread` (the only legal value in version 1) | The capability executes synchronously on the calling thread. Marshalling elsewhere is the host implementation's own business; its blocking time is charged to the operation's `WallClock` allowance and it counts as one unit of `HostCalls`. |
| F7 | `ExceptionTranslation` | closed: `TerminateOperation`, `ObservableFault` | No default. The descriptor declares one; a `Value` capability and an `ArtifactProvider` capability may declare different modes. |

Section 7 also names *permissions* and *cancellation* in the same breath as
these fields, and neither is a field. **Registration is the permission.** A
capability the composition did not register into that runtime is not reachable
from it at all, so permission denial is `CapabilityNotRegistered` at binding
rather than a per-call check the core would have to evaluate on the hot path.
Cancellation observability at a capability boundary is a runtime-identity
input, owned by ADR 0004 (`0004-lifecycle-and-state-machine.md`) and ADR 0006,
not a property of the capability.

F6 is a deliberate placeholder and must be read as one: it is written now, with
one legal value, so that an amendment can add affinity kinds without changing
the descriptor's shape or the identity that records it.

### Allowlist and binding

A capability is invocable by a runtime only if the composition root registered
it INTO THAT RUNTIME at runtime creation AND the profile's descriptor declared
a matching import naming exactly (`CapabilityId`, `Version`, `SignatureId`,
`Kind`). Binding happens once, at runtime creation, and produces an immutable
binding table (VM-0 decision on paper; no file at VM-0). There is no
post-creation registration, no unregistration, no by-name lookup at call time,
no fallback and no default resolution: calls dispatch through an index fixed at
binding.

| Situation | Outcome |
|---|---|
| Required import unsatisfied | runtime creation returns `HostFailure` / `CapabilityNotRegistered`, naming the exact triple |
| Optional import unsatisfied | binds as absent; the profile may ask only whether index k is bound |
| Invocation of an absent optional binding | `HostFailure` / `CapabilityNotRegistered` |
| Import present, `SignatureId`, `Version` or `Kind` mismatched | runtime creation returns `HostFailure` / `CapabilitySignatureMismatch`, naming both sides |
| Capability registered but not imported | inert |
| Any required binding fails | NO PARTIAL BINDING: the runtime is not created and no capability is bound |

Six of the seven fields, plus the boundness of an optional import, form the
per-import tuple that ADR 0006 makes a cache-key input; `ThreadAffinity` is a
runtime-identity input only. That is why F2 admits no range: a range would make
the key depend on which version happened to be present at bind time, so the
same bytes would carry different semantics on two hosts with the same declared
support.

### Affinity composes by intersection and may only tighten

The core default is no thread affinity anywhere (ADR 0004). A profile's
declared affinity may tighten it; a capability's declared affinity may tighten
it further for calls that cross that capability. No layer may relax an affinity
a lower layer declared, and no layer may turn the core's absence of affinity
into a requirement on a caller that never touches the declaring capability. A
violation returns `InvalidState` / `ThreadAffinityViolation` on the operation
that crossed the boundary.

In contract version 1 the capability layer contributes no tightening, because
`CallerThread` is F6's only legal value. The rule is written now anyway: two
independently declared affinities with no composition rule is exactly the kind
of unstated interaction VM-1 would otherwise resolve silently, and intersection
with tighten-only is the rule the budget model already uses, so it introduces
no new concept.

**Rejected.** Version ranges or minimum-version imports: they make the cache
key depend on ambient registration rather than on the artifact. A single kind
with a "returns bytes" flag: a host could then flip a value capability into a
provider, reopening invariant 11's back door. Post-creation registration or a
mutable per-runtime registry: it makes runtime identity mutable, so a verified
handle admitted under matching identity could have its assumptions invalidated
afterwards, and it defeats index dispatch and AOT rooting. Name-based lookup
with a default fallback: it violates invariant 2 directly, hides reachability
from the trimmer, and turns a missing capability into a silent substitution
instead of a deterministic refusal. Host-affinitized capabilities in version 1:
they require the core to own a thread or a marshalling queue, which is the
scheduler line the passive core does not cross.

## The closed set of capability transfer types

Section 7 requires that a capability cannot enumerate arbitrary CLR members.
That is enforced by giving profile code no CLR reference to enumerate, not by a
policy. Contract version 1 defines a closed set of transfer types
(VM-0 decision on paper; no file at VM-0). A signature whose parameters or
return type name anything outside that set is not a valid signature: it fails
to compile against the core-defined contract types, and any signature that
nevertheless reaches descriptor validation is rejected at catalog construction.

| Admitted | Notes |
|---|---|
| The CLR primitive value types: `bool`, the signed and unsigned integer widths, the floating-point widths, `char` | by value only |
| `VmBytes`, a core-owned bounded read-only byte view | lifetime is the call; may not be retained past return |
| A core-owned bounded UTF-8 text view | same call-scoped, non-retainable lifetime |
| `VmOpaqueRef`, the core-owned opaque per-runtime transfer reference | see below |
| The core-owned artifact-descriptor-plus-bytes result type | `ArtifactProvider` capabilities only |

Nothing else. In particular `System.Object`, `System.Type`, `System.Delegate`,
any `System.Reflection` type, any interface, any array of reference type, any
other framework value type, and any host-defined class or struct are not
transfer types.

`VmOpaqueRef` is the mechanism that replaces references. A host that must hand
rich state across mints one from a per-runtime, host-side table. It is an
opaque, non-forgeable value carrying a runtime-scoped identity and a generation
stamp; profile code cannot dereference it, enumerate it, or compare it to
anything meaningful, and can only pass it back to a capability bound to the
same runtime. Presenting one to a capability of another runtime is
`HostFailure` / `ForeignOpaqueRef`; presenting one whose generation has been
invalidated is `HostFailure` / `StaleOpaqueRef`. An earlier draft named this
type `VmHandle`, and those reasons `ForeignHandle` and `StaleHandle`; that name
is struck, because unqualified "handle" means the verified-artifact handle and
nothing else. Neither reason is ADR 0005's `InvalidState` / `ForeignHandle`,
which is a verified handle presented to a runtime that does not own it: this
one is a host-side reference misused across the capability boundary, so it is
reported where the fault is, on the host boundary.

Three enforcement layers, strongest first:

| Layer | Mechanism | Where it acts |
|---|---|---|
| Type system | The closed set means there is no expressible signature through which a CLR reference crosses. | the profile author's own build, at compile time |
| Closure | Invariant 2 bans dynamic loading and reflective invocation; the closure scans of VM-3 and VM-6 assert that no published product closure references reflection, expression-tree, IL-emit or assembly-loading surface; invariant 7's Native AOT publish removes the metadata that would make enumeration meaningful. | deferred to VM-3 and VM-6 |
| Dispatch | Invocation is by binding index into an immutable table. No API returns the registered capability set, resolves a capability by name, or returns a CLR type or member. A profile may ask only whether index k is bound. | VM-1 |

Rule B4: No exported member of a product assembly names a type outside System.*
and Broiler.VM. Status: Vacuous at VM-0 - it runs, and nothing in the VM-0
graph can violate it; it becomes non-vacuous at VM-1 when a product assembly
exports a member with a parameter or return type.

**What this does not claim.** The capability boundary is not a profile sandbox.
A profile is trusted, statically composed code that can call any CLR API its own
assembly references. The claim, stated exactly as narrowly as section 7 makes
it, is that THE HOST-CAPABILITY BOUNDARY GRANTS NO CLR SURFACE: a profile
obtains nothing from the host beyond the values its declared imports name, and
section 14's blocking failure "arbitrary CLR discovery or access" is about that
boundary. Whether a profile assembly is itself constrained is a composition and
review property, held by the closure reports of VM-3 and VM-6, not by this
contract. Describing it otherwise in any support claim would be the untruthful
capability claim section 16 makes a stop condition.

**Rejected.** Admitting `System.Object` with a documented rule against
reflecting over it: a documented rule is precisely what section 7 declines to
settle for, and one `object` parameter reintroduces the whole CLR surface.
A call-time permission check on what a capability returns: it requires the core
to inspect values it is forbidden to store or interpret, and it is a run-time
check where a compile-time impossibility is available. Host-defined structs by
convention: a struct can carry a reference field, and each host type would
enter `SignatureId`, making it depend on assembly metadata the core cannot
canonicalize. Analyzer-only enforcement: an analyzer is suppressible and does
not constrain a profile compiled outside this repository, which is invariant
7's posture on analyzers applied by analogy.

## Exception translation across the capability boundary

**No CLR exception may unwind through profile frames.** The core brackets every
capability invocation with a catch boundary at the call site. An exception
escaping the host delegate is caught there and converted; it never propagates
into profile code, because a profile's frame and stack invariants are not
CLR-unwind-safe and the core, which inspects no frames, cannot restore what it
would break.

Translation precedence is ordered and exhaustive. Evaluate in order; stop at
the first match.

| # | Condition at the catch boundary | Result |
|---|---|---|
| X1 | The exception is an `OperationCanceledException` carrying the operation's own token, or cancellation was already requested | `Cancellation` |
| X2 | Any core meter in the operation's chain is exhausted at the moment of the catch | `ResourceExhaustion`, naming the dimension and the exhausted scope |
| X3 | The exception is an `OutOfMemoryException` | `ResourceExhaustion`, dimension `AllocatedBytes`, reason `HostAllocationFailed` |
| X4 | Anything else | `HostFailure`, reason `HostCapabilityFaulted`, carrying the capability's `CapabilityId` and `Version` and the opaque host correlation token of ADR 0005's diagnostics field for `HostFailure` |

X1 and X2 precede X4 so that a capability which observes cancellation or
exhaustion and throws is reported as what actually happened rather than as a
host defect. Misclassifying either would corrupt every cancellation and
exhaustion metric built on those categories, and section 14 blocks a
nondeterministic failure class. This precedence is the capability boundary's
own; it does not replace the stage outcome precedence ADR 0005 owns.

**One seam to reconcile.** ADR 0005 states the same translation in two rules -
an `OperationCanceledException` carrying the operation's own token becomes
`Cancellation`, anything else becomes `HostFailure` / `HostCapabilityFaulted` -
which are X1 and X4. X2 and X3 are this record's refinement, and they are
strictly additive: they reclassify only throws where a meter is already
exhausted or the CLR itself failed an allocation, both of which ADR 0005's
default would report as a host defect. Read together, the four-rule order
governs. Reconciling the two sentences is a one-line edit to ADR 0005 that the
core-contract owner must make before either record is accepted; VM-0 records
the divergence rather than silently resolving it in one direction.

What happens to the operation is F7's business: under `TerminateOperation` the
operation completes with the translated category, does not return to the
profile at the call site, and the instance is marked terminal; under
`ObservableFault` the profile receives a typed fault result at the call site
and decides what to do, which is what a language needs when a host call maps
onto a catchable language exception.

**Stack overflow is not translatable.** It is process-fatal and uncatchable, so
the core does not pretend to translate it. The `CallDepth` ceiling exists to
make it unreachable. `CallDepth` is therefore a SAFETY mechanism, not a
fairness one: its per-profile default must be set from measured native frame
cost per profile frame and justified in the descriptor's ownership metadata,
and a profile that recurses on the CLR stack without charging `CallDepth` can
crash the process - a profile contract violation the core cannot detect in
time.

**Isolation.** A host exception is scoped to the operation that made the call.
It never affects another runtime, never affects the aggregate budget beyond
charges already applied, and never prevents cancel or dispose of any runtime.

**Rejected.** Letting host exceptions propagate and asking profiles to guard
their own frames: it puts the core's isolation guarantee in every profile's
hands and makes it unprovable. Classifying everything as `HostFailure`: a
cancelled operation whose capability throws would be reported as a host defect.
A single translation mode: `TerminateOperation` alone makes any host error
unrecoverable to a language that models it as catchable, and `ObservableFault`
alone forces every profile to handle every host defect. Catching
`StackOverflowException`: it is not catchable, and claiming to handle it would
be an untruthful capability claim under invariant 8.

## A shared registry bridges nothing

Section 7's requirement is that a shared host registry does not ITSELF bridge
values between profiles or grant an ambient platform surface. The word "itself"
is load-bearing: a host may share its own state; the core's registry may not be
the thing that does it. Three structural facts, not a policy, make that
distinction real.

| # | Fact | Consequence |
|---|---|---|
| S1 | The registry is a composition-time template, not a live shared object. Registration produces per-runtime immutable bindings at runtime creation; the registry holds no per-runtime state, no values, and no mutable cross-runtime data. | There is nothing live to bridge through. |
| S2 | The core stores no values. | There is no core-owned place where one profile could park a value for another. The only core-owned mutable object shared across runtimes in contract version 1 is the aggregate budget, and it carries counters only (ADR 0007). |
| S3 | `VmOpaqueRef` is runtime-scoped and generation-stamped. | A reference minted for one runtime is `ForeignOpaqueRef` in another, so host-side tables do not become a channel. |

Together these mean that a bridge, where one exists, is host implementation
state the host deliberately shared - and that fact is recorded in runtime
identity as a shared capability instance, so it is declared rather than
inferred, and it carries section 7's VM-4 stress-evidence obligation for
mutable sharing.

**No ambient surface.** A runtime's reachable host surface is exactly its
immutable binding table. There is no ambient, static, thread-static or
`AsyncLocal` registry; no process-wide default capability set; no
environment-derived capability; and no capability a profile obtains other than
through a `Required` or `Optional` import its own descriptor declared and the
composition root satisfied.

Five evidence obligations follow. All are closable against fixture profiles
under invariant 13, and none is asserted at VM-0.

| # | Obligation | Owning milestone |
|---|---|---|
| T1 | Two runtimes of two DIFFERENT fixture profiles are bound from the same registration set; a reference minted for runtime A and presented by runtime B's guest returns `HostFailure` / `ForeignOpaqueRef`, and B observes no host state. | VM-4 |
| T2 | Where a host deliberately binds one capability instance into both runtimes and profile A stores state through it, any retrieval by B is attributable to host implementation state, AND both runtimes' recorded identities carry the shared-capability-instance marker. | VM-4 |
| T3 | A public-API test over the profile-facing surface shows no registry-enumeration member, no name-based lookup member, no `object`-typed parameter and no reflection type - so the absence of a bridge is a property of the surface, not of the implementation. | VM-3, held by VM-6's baseline |
| T4 | Each named composition's published dependency closure contains no reflection or dynamic-code assembly and exactly the capabilities its composition root names. | VM-3 and VM-6 |
| T5 | A composition registering value capabilities but no artifact provider refuses every guest-initiated load deterministically, with ADR 0008's refusal reason and not a value-capability one; a composition registering nothing returns `HostFailure` / `CapabilityNotRegistered` for every capability invocation. | VM-4 |

**Rejected.** Forbidding a host from binding one capability instance into two
runtimes: unenforceable, since the core cannot tell two instances apart from
two references to one, and it would forbid legitimate designs such as a shared
clock or logger. Declaring and recording the sharing is enforceable and honest;
forbidding it is neither. A core-mediated cross-profile value bridge: section 1
lists an implied invocation bridge between two hosted profiles as an explicit
non-goal, and any bridge requires the core to own a value representation.
Process-wide default capabilities for convenience: ambient surface by
definition, defeating the closure claim of section 15 gate 2. Documentation
plus code review: section 14 makes a cross-runtime capability leak a release
blocker, and a blocker needs a test.

## Share mechanism, never share semantics

The rule is standing. It applies to every proposal to create a shared
component, to add a type to Broiler.VM.Binary, or to move code out of a profile
into the core. Mechanism is HOW something is done safely - reading an untrusted
length, running a worklist to fixpoint, charging a budget. Semantics is WHAT it
means - values, frames, types, opcodes, syntax trees. Mechanism generalizes
across languages because it is language-free; semantics does not, and sharing
it produces the lowest-common-denominator model invariant 4 exists to prevent.

**Invocation.** Any profile owner or the core architecture owner may invoke the
extraction gate, but only after two or more **product** profiles already
implement the behaviour in merged code. The fixture profile and the
application-local consumer profile do NOT count toward the two, and neither
does an anticipated profile, a profile roadmap, or a planned language.

| # | Gate condition, all four required |
|---|---|
| G1 | Two or more product profiles already implement the behaviour. |
| G2 | The implementations have been compared and the shared part identified from real merged code, not anticipated. |
| G3 | The shared part is expressible without naming any language concept. |
| G4 | Extraction creates no profile-to-profile dependency. |

**Record.** Every invocation produces an extraction record
(VM-0 decision on paper; no file at VM-0, and none can exist before the gate
first fires) in this ADR set - because it changes the core graph - naming: the
two implementations with file paths and source revisions; the measured
duplication, shown as a correspondence or a diff rather than asserted; the
proposed public surface, written with no
identifier drawn from any language's vocabulary; the resulting graph edges
demonstrating G4; the two named consumers; and the verdict with its date and
deciding owner. The record is filed whether the verdict is accept or refuse.

**When the gate fails.** The record states which condition failed and the
duplication is documented and kept. Each duplicated implementation carries a
source-level pointer to the record; the record is filed as a refused
extraction; and the duplication is NOT tracked as debt with a repayment
schedule, because a schedule converts a deliberate decision into a backlog item
that will eventually be executed without the gate. The record reopens only when
the failing condition changes - a third implementation appears, a language-free
formulation is found, or the dependency shape changes - and reopening produces
a new dated verdict rather than an edit of the old one.

**One pre-approved exception.** Broiler.VM.Binary is the single shared
mechanism component approved without the gate, because the core's own envelope
and every profile verifier are two consumers before any profile exists. Its
scope is frozen at mechanism: checked readers, variable-length integer
decoding, bounded framing, allocation guards. It contains no format, no schema
and no semantics, and any addition that names a language concept passes the
gate like anything else.

Standing refusals, restated so they are not re-litigated:

| Candidate | Standing verdict |
|---|---|
| A shared syntax tree or parser | Refused permanently. |
| A shared value representation, frame layout, or opcode set | Refused permanently; these are the semantics the core exists not to own. |
| A verification framework parameterized by a profile's abstract domain | Not predicted. Opened only when a second PRODUCT verifier exists and the duplication is measured. |
| Lexing, source positions, diagnostic formatting | Waits for a second text front end. |
| A command-line compiler, build integration, or packaged SDK | Not opened until a composition must ship precompiled artifacts with no compiler in its image. VM-0 chooses no name for one. |

Rule A11: No project outside the composition-root allow-list references an
assembly matching Broiler.VM.Profile.*. The allow-list is empty at VM-0.
Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A11-profile-reference-outside-composition-root.csproj.witness.

A11 is the mechanical form of G4's outbound half once profile projects exist:
only a composition root may reference a profile, so no profile can reference
another. ADR 0001 (`0001-component-topology-and-dependency-graph.md`) owns the
rule and the allow-list, which is declared inside the architecture-test project
(exists at VM-0: src/tests/Broiler.VM.Architecture.Tests/) and moves to a
composition register at VM-3.

**Rejected.** Letting the fixture and consumer profiles count toward G1: both
are core-owned and deliberately shaped to fit the contract, so agreement
between them is evidence about the core's own tests, and the core would be
extracting a shared component against itself. Tracking refused duplication as
scheduled debt: section 8 says duplicated mechanism is cheap and a wrong shared
abstraction is not. Pre-opening a verification framework because both intended
profiles will verify something: section 8 says extract later, do not predict -
two verifiers share the shape and share no domain, and the shape cannot be
validated before the second verifier exists. Allowing a large-benefit exception
to G4: the condition is absolute.

## What VM-0 does not enforce

Stated in plain words, because a rule that cannot be enforced must not be
written as a passing claim. ADR 0011 mints no exclusion identifier: the
identifier blocks allocate none to it, so each limit below is stated in full
and, where a sibling already owns the matching identifier, cited.

1. **Nothing in this record is asserted by an architecture rule over its own
   subject.** Five registered rules bear on it - A4, A8, A10, A11 and B5, all
   Active, plus B4, Vacuous - and they inspect project files and assembly
   metadata only. None of them can see a capability descriptor, a transfer
   type, a binding table, or a checklist demonstration, because none of those
   exists at VM-0. Closed by: the milestone that creates each subject adding
   its register row - VM-1 for the descriptor, transfer types and binding
   table, VM-3 and VM-6 for the closure and baseline obligations.
2. **This record registers no new rule identifier.** The rule register
   (exists at VM-0:
   src/tests/Broiler.VM.Architecture.Tests/rules.register.json) is the
   authority the evidence bundle quotes, and every rule identifier an ADR names
   must have exactly one row in it. The assertions the capability,
   transfer-type and registry decisions call for therefore appear above as
   obligations owned by a milestone, not as rules. Closed by: the activating
   milestone registering them with the types they assert over.
3. **The core cannot verify three of the five profile charging obligations.**
   CO-1 (proportionality), CO-2 (charge before work) and CO-4 (poll density)
   are stated obligations the core cannot check; CO-3 (allocation attribution)
   is enforced only for allocations that go through the core's bounded
   primitive; only CO-5 (no self-enlargement) is structural. ADR 0007 owns the
   obligations and the compensating controls. Closed by: VM-3 and VM-6 closure
   and review evidence, which are controls, not proofs.
4. **The capability boundary is not a sandbox**, as stated above. No support
   claim may describe it as one.
5. **The extraction gate cannot fire during VM-0 through VM-6**, because no
   product profile exists. G1 is unsatisfiable until two of them do, so the
   first real invocation belongs to a profile roadmap rather than to this
   component's. Closed by: the second product profile.
6. **The Vacuous and Deferred rule inventory** - including B4 above - is
   recorded once, with counts and identifiers, as ADR 0001's Exclusion EX-05
   and is repeated verbatim in the evidence bundle (exists at VM-0:
   docs/evidence/vm-0/) and the ledger row. This record does not restate it.

## Consequences

- Every profile-facing type is public from the first commit. The API surface is
  larger than it would otherwise be and the baseline VM-6 freezes is
  correspondingly more valuable; the cost is paid to keep P2 provable.
- Profile packages are versioned against core contract versions and republished
  per version. The future public support table lists capability identities as
  `(CapabilityId, Version)` pairs, never as names, because F2 admits no range.
- VM-1's exit gate becomes a checklist with ten named demonstrations. A partial
  demonstration cannot be promoted, under the status ledger's update rule 4.
- VM-1's fixture set must contain TWO distinct fixture profiles, or T1 and T5
  are not closable. The VM-1 gate's "multiple fixture profiles" wording already
  anticipates this, and the fixture profile must be substantial enough to
  exercise host calls, budgets, suspension, nested loads and persistence.
- The shared-capability-instance marker is a runtime-identity field VM-1 must
  populate; because it is also a cache-invalidation input, VM-2 and VM-6 carry
  it.
- X2 requires the core to read the operation's meter chain at the catch
  boundary without charging for the read. That is a core-internal path, not a
  profile-facing member: ADR 0007's rule that the profile-facing metering
  surface carries no non-consuming remaining reader is unaffected, and VM-1
  must implement the catch boundary without adding one.
- Hosts with rich state must redesign their capabilities around `VmOpaqueRef`
  tables. That is a real ergonomic cost, and it must be shown in a VM-3 sample
  so it is judged before VM-6 freezes the API. Adding a transfer type - a
  floating-point vector, say - is a numbered amendment, not a package release.
- A composition that forbids dynamic evaluation registers no artifact-provider
  capability, and the refusal is a contract outcome rather than an ad-hoc check
  inside an engine. That is exactly the browser content-policy consequence
  section 11 predicts; ADR 0008 owns it.
- Refusal records accumulate as durable rationale, so a refused extraction is
  not re-proposed from scratch, and Broiler.VM.Binary's scope becomes
  defensible by a name test rather than by argument.
- **Reason codes this record contributes to ADR 0005's registry**, which owns
  them: `CapabilityNotRegistered`, `CapabilitySignatureMismatch`,
  `ForeignOpaqueRef`, `StaleOpaqueRef` and `HostAllocationFailed`.
  `HostCapabilityFaulted`, `ReentrancyRefused` and `ThreadAffinityViolation`
  are already in it and are used here unchanged. Adding a code later is an
  additive amendment; renaming or removing one is not.
- **Supersedes an illustrative roadmap snippet.** P1 supersedes section 5's
  target-direction block, whose arrow reads
  `Broiler.VM.Profile.X --> Abstractions + Binary (+ Runtime contracts)`: a
  profile references Abstractions and Binary and never Broiler.VM.Runtime.
  G1 likewise supersedes section 8's first extraction condition, "two or more
  profiles already implement the behavior", which this record narrows to two or
  more PRODUCT profiles. Both are carried as `Proposed`, not applied, rows in
  ADR 0003's roadmap-amendment register, whose Exclusion EX-11 records that the
  roadmap and the ADRs disagree in those places until an owner lands the patch.
  VM-0 proposes and does not apply them; no invariant, milestone gate, delivery
  order, or section 14, 15 or 16 text is changed by this record.
