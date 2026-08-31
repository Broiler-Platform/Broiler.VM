# Broiler.VM roadmap

**Status:** Proposed component roadmap for the generic execution core. [The evidence
ledger](roadmap.status.md) is the authority for what has been accepted; at the time of writing it
records VM-0 as in progress and unaccepted, and VM-1 through VM-6 as not started. No milestone is
complete merely because this document exists.

Broiler.VM is a new, statically composed, NativeAOT-compatible component that executes verified
bytecode artifacts. It is a **host for language profiles, not a language.** It owns profile
selection, bounded loading, the verification boundary, the execution lifecycle, resource
authority, diagnostics, and composition evidence. It owns no opcode set, no value representation,
and no language semantics of its own.

This roadmap plans the core only. **JavaScript** and **WebAssembly** are the two intended first
profiles, each a separate component with its own roadmap written after the core contract is
accepted. Section 9 records what those profiles are expected to require, so the contract is
designed for them rather than retrofitted to them. Sections 10 and 11 record where compilation
lives and how a host gets from source to an artifact without any tooling, and section 12 records
the boundary against the legacy `Broiler.JS` component, which Broiler.VM does not depend on.

---

## 1. Terminology and support claims

The repository already uses the word *profile* for several different boundaries. Documents, APIs,
test names, and release manifests must qualify the term when ambiguity is possible.

| Term | Meaning in this roadmap |
|---|---|
| **VM profile** | A bytecode language plus its format, feature/version manifest, verifier, value/frame model, and executor. It is a separate component that references the core; the core never references it. |
| **Core contract version** | The numbered revision of the profile-neutral lifecycle, operation-result, resource-authority, guest-initiated-load, external-control, and host-capability contract frozen by VM-0 and implemented by VM-1. Profiles and artifacts version independently of it. |
| **Feature manifest** | The exact language surface accepted by one version of a profile. The core fixes the manifest's shape and identity rules; a profile fixes its content. A profile name alone is never a conformance claim. |
| **Built-in profile** | A profile whose factory and dependencies are directly referenced at build time and rooted in a static catalog. A built-in may be Broiler-provided or application-local; it is never discovered at run time. |
| **Fixture profile** | A deliberately trivial profile owned by the core's own tests. It proves contracts, closures, and failure paths without waiting for a product profile, and it never ships in a product package. |
| **Verified artifact** | The opaque, immutable, profile-bound output of successful verification. Execution and instantiation consume this handle, never caller-owned raw bytes. |
| **Guest-initiated load** | A verification requested by executing guest code rather than by the caller. It produces an ordinary verified artifact through the ordinary verification path. |
| **Artifact-provider capability** | The typed, allowlisted host capability that answers a guest-initiated load with a descriptor and bytes. It is a distinct capability kind from a value-returning import and is never implied by one. |
| **External suspension** | A pause requested by the host or a diagnostic client rather than by guest code, from which execution may resume. It is distinct from guest-initiated suspension and from terminal cancellation. |
| **Deployment composition** | The exact set of profiles, host capabilities, and tools statically linked into one product image. Compositions are named, and each claimed one carries its own evidence. |

A core release claims the core: its contract version, its lifecycle and safety guarantees, and the
compositions it can publish and run. It never claims a language. An unknown profile, unsupported
feature manifest, or incompatible format version is a deterministic load failure, never a
best-effort fallback to another profile.

### Scope

Broiler.VM owns:

- explicit profile selection and an immutable built-in catalog;
- bounded artifact loading and profile/version matching, including bounded mediation of
  guest-initiated loads;
- the common verify/load/instantiate/invoke/suspend/resume/cancel/dispose lifecycle, per-runtime
  and shared aggregate resource-budget authority, diagnostics, and profile-neutral
  operation-result envelopes;
- typed host-capability registration, including artifact-provider capabilities, and per-runtime
  ownership;
- the bounded binary-reading and bounded-allocation primitives every profile needs, so that no
  profile writes its own unchecked reader;
- the numbered core contract version and the amendment procedure that changes it;
- the public source-level contract by which a profile is written and statically composed; and
- composition, trimming, Native AOT, and package evidence for the core boundary.

Each VM profile owns:

- its bytecode payload format and feature manifest;
- decoding, validation, and profile-specific resource checks;
- its value, frame, call, control-flow, trap/exception, and suspension model;
- imports, exports, and conversions at its host boundary;
- the language meaning of any guest-initiated load it declares, including specifier resolution,
  linking, lexical context, evaluation ordering, and what it exposes while externally suspended;
- its typed normal-result and fault payloads and the projection API that exposes them without
  adding language cases to the core;
- conformance fixtures, its own oracle, and profile-specific optimizations; and
- any compatibility promise for its persisted profile payload.

### Non-goals

- one universal opcode set, tagged value, or frame ABI shared across languages;
- reflection-based or unloadable runtime plug-ins, and any binary plug-in ABI;
- automatic artifact/profile detection by trying multiple decoders;
- an implied invocation bridge between two profiles hosted in the same process;
- source compilers, parsers, or text formats in the core;
- a debug wire protocol, a cross-profile inspection API, or a profile-neutral breakpoint model.
  VM-0 freezes only the external-suspension transitions that a profile-owned debug surface needs;
- delivering the JavaScript or WebAssembly profile. Those are separate components with their own
  roadmaps, and no gate in this document depends on either existing; and
- performance claims about any language. The core measures only its own overhead.

---

## 2. Engineering invariants

1. **A profile is selected explicitly.** The caller supplies a stable profile identity, or a
   checked Broiler.VM envelope supplies it and the caller confirms it. The runtime never guesses
   by probing every registered decoder.
2. **Registration is static and typed.** Composition roots directly reference their profile
   factories; the generic runtime references no concrete profile. There is no `Assembly.Load`,
   `Type.GetType`, assembly scan, `Activator.CreateInstance`, magic type name, or
   module-initializer ordering dependency.
3. **Verification produces the only executable input.** Every external artifact is parsed with
   checked lengths and budgets. Verification snapshots or fully decodes caller-owned bytes into an
   opaque immutable handle, and only that handle may be instantiated or executed. Later mutation,
   disposal, or concurrent reuse of the caller's buffer cannot affect verified instructions. Bytes
   a profile obtains while executing take the same path: they become their own verified handle
   before anything in them runs, and no profile may execute source or bytes it acquired without
   one.
4. **The core is semantics-neutral.** It provides lifecycle and safety contracts, not a
   lowest-common-denominator ISA. Two profiles may share a primitive only after section 8's
   evidence rule is satisfied.
5. **Mutable state has an owner.** Frames, feedback, inline caches, quickening, host handles, and
   compiled artifacts belong to a runtime, realm/module, program, or function. Canonical bytecode
   and persisted artifacts contain no warmed state or process-local identities.
6. **Tool closures are explicit.** A product image contains a compiler, parser, or text-format
   reader only when its composition declares one, and that larger closure must publish and run
   under Native AOT before it is supported.
7. **Native AOT is demonstrated, not inferred.** Analyzer success and a trimmed build are inputs.
   Each claimed composition must publish and run its representative workload on every declared RID
   with trim/AOT warnings treated as errors.
8. **Unsupported surface is truthful.** A missing capability, host capability, or deployment mode
   has a documented deterministic failure. A shape-only stub cannot satisfy a capability gate.
9. **Resource authority is trusted and monotonic.** At runtime creation the host supplies explicit
   ceilings or explicitly adopts bounded profile defaults; omission never means unbounded. Each
   profile may impose a stricter hard maximum, and artifact declarations may only request lower
   limits. Verification fixes their intersection as the handle's verification/instantiation
   ceilings before an untrusted allocation. Instance or invocation budgets may tighten those
   ceilings or allocate a remaining fuel/time allowance; they never raise them without producing a
   newly verified handle. Ceilings also compose: a host may create runtimes under one shared
   aggregate budget, a per-runtime ceiling may never exceed the parent's remaining allowance, and
   creating more runtimes may not multiply a host maximum.
10. **The common lifecycle is stable within a core contract version.** VM-0/VM-1 define ownership,
   state transitions, thread affinity, reentrancy, cancellation, suspension, resumption, and
   idempotent disposal. Profiles refine observable language behavior without changing the core
   state machine or adding language cases to a core result enum. A capability the frozen contract
   cannot express is added by a numbered amendment, never by a profile-specific special case.
11. **Guest-initiated loads are mediated, bounded, and refusable.** A profile may obtain further
   artifacts while executing only through a declared artifact-provider capability. Each request is
   charged to the operation that made it, bounded in depth, fan-out, and cumulative bytes, and
   deterministically refused when the composition registers no provider. A profile never reaches
   the filesystem, the network, or a source compiler on its own.
12. **External control is a lifecycle state, not a side channel.** A host or diagnostic client may
   pause and resume execution only through transitions the core contract declares. Suspension
   requested from outside is distinct from guest-initiated suspension and from terminal
   cancellation, and what a paused profile exposes remains the profile's own surface.
13. **The core is provable without a product profile.** Every core gate closes against the fixture
   profile and an application-local consumer profile. A gate that cannot be demonstrated without
   shipping a language is a core-design defect, not a scheduling problem.

### Core contract version and amendment

The profile-neutral contract frozen by VM-0 and implemented by VM-1 — lifecycle states and legal
transitions, operation-result categories, resource authority, verified-artifact ownership,
guest-initiated loads, external control, and the host-capability shape — carries one integer
**core contract version**, starting at 1. It is versioned separately from any profile format,
feature manifest, or package version, and every support table, catalog entry, and evidence bundle
names it.

Amendment is an expected event, not a failure. One is required whenever an approved profile
capability cannot be expressed by the frozen contract. Because this roadmap freezes the contract
before either intended profile is designed, at least one amendment should be planned for. The
procedure is:

1. record the driving capability, the profile that needs it, and why no profile-owned design
   satisfies it;
2. mint core contract version *n+1* as a dated revision of the VM-0 ADR, stating which
   transitions, categories, or contracts changed and whether the change is additive;
3. re-evaluate accepted evidence against the new version, recording what recertifies unchanged,
   what must be re-collected, and what is superseded, under the
   [status ledger](roadmap.status.md) update rules; and
4. publish the new version in the support table beside the profiles and packages that require it.

An additive amendment may leave existing profile packages source-compatible; changing an existing
transition or category may not. Neither is a reason to fork the contract: a second core state
machine maintained for one language is a stop condition under section 16.

---

## 3. Static profile registration

The exact public names are deferred to VM-0, but the required shape is a generic builder/catalog
whose entries contain immutable descriptors and direct factory delegates supplied by a composition
root:

```csharp
var catalog = VmCatalog.CreateBuilder()
    .Add(FixtureVmProfile.Descriptor)
    .Build();

var vm = VmRuntime.Create(catalog);
```

Each profile exposes its own descriptor accessor from its own package. There is no aggregate
`BuiltInProfiles` type that names several profiles at once: such a type would reference every
profile assembly and defeat the exact-closure gates in VM-3. A composition that wants two profiles
names both descriptors; a composition that wants one names one and links only that one.

Every catalog entry must provide:

- a stable, non-localized profile ID and display name;
- a supported profile-format range and feature-manifest IDs;
- an AOT-rooted verifier and per-runtime executor factory;
- bounded profile limit defaults that a host must explicitly adopt or override, plus
  host-capability descriptors;
- the core contract version it was built against, and whether it declares guest-initiated loads,
  asynchronous instantiation, or external suspension;
- a conformance manifest/version and diagnostics identity; and
- package and ownership metadata used by architecture and release checks.

Registration rejects duplicate IDs, alias collisions, missing factories, unsupported versions,
unsupported core contract versions, and descriptors whose declared identity differs from the
produced executor. Catalog order has no semantic effect. A hand-maintained catalog is the initial
preference; a source generator may replace it later only if it emits direct calls, produces a
reviewable manifest, and has a test proving that generated and documented catalogs agree. Runtime
reflection is not an allowed substitute.

Broiler-owned IDs use the reserved `Broiler.*` namespace; application-local IDs use a documented
reverse-domain namespace. The public contract is an AOT-safe source-level composition API, not a
binary plug-in ABI: a profile is compiled with the application, its generic instantiations and
factories are rooted directly, and its compatibility is checked at build and test time as well as
at catalog construction.

---

## 4. What a profile must be able to do

The core is only as generic as the hardest profile it can host. VM-0 states the profile-facing
contract as a checklist, and VM-1 proves each item against the fixture profile:

| Capability | Core obligation |
|---|---|
| Decode a binary payload | Bounded readers, checked arithmetic, section/segment framing, and no allocation from an untrusted count before it clears its bound. |
| Validate before execution | A verification entry point that produces an immutable handle and a stable failure taxonomy, with no partial state escaping a failed verification. |
| Own a value and frame model | The core stores no values and inspects no frames. Handles are opaque to the core and typed to the profile. |
| Report language outcomes | Typed profile payloads behind profile-neutral operation-result envelopes, with a projection API that adds no language case to the core. |
| Suspend and resume | Guest-initiated suspension, and external suspension where the composition allows it. |
| Acquire code while running | Guest-initiated loads through an artifact-provider capability, bounded and charged to the requesting operation. |
| Call the host | Typed, allowlisted, versioned capabilities with declared reentrancy, affinity, and exception translation. |
| Be budgeted | Fuel, wall-clock, allocation, depth, and host-call accounting that a profile charges into and cannot enlarge. |
| Persist and invalidate | An opaque profile section inside a core-owned envelope, with the profile owning its own cache key and migration. |
| Be composed exactly | A direct descriptor and factory, rooted for trimming and Native AOT, with a reviewable closure. |

A profile that cannot be expressed through this checklist is either misdesigned or evidence that
the contract needs an amendment. Both outcomes are recorded; neither is worked around inside the
core's execution loop.

---

## 5. Package boundaries and the dependency graph

These names are hypotheses, not authorization to create assemblies. VM-0 must prove the graph with
project shells and an explicit assembly/package budget.

| Logical boundary | Candidate package | Responsibility and dependency rule |
|---|---|---|
| Contracts | `Broiler.VM.Abstractions` | Profile IDs/descriptors, execution options/results, budgets, diagnostics, typed host contracts, and the core contract version. References no concrete profile. |
| Bounded input primitives | `Broiler.VM.Binary` | Checked readers, variable-length integer decoding, bounded framing, and allocation guards used by the core envelope and by every profile verifier. Contains no format, no schema, and no semantics. |
| Core runtime | `Broiler.VM.Runtime` | Builder, immutable catalog, bounded load/execute lifecycle, resource authority, guest-initiated-load mediation, cancellation, diagnostics, and ownership. References abstractions and binary primitives only. |
| Fixture profile | `Broiler.VM.Fixtures` *(test-only)* | The trivial profile and application-local consumer profile used to prove contracts and closures. Never referenced by a product package. |
| Profile (future) | `Broiler.VM.Profile.<Language>` | One language: format, verifier, value/frame model, executor, imports, conformance. References the core; never references another profile. **One product assembly is the default**; section 10 splits the format out only where Broiler compiles the language itself, and a profile that consumes a format an external toolchain produces has nothing for a format assembly to hold apart. The frozen "exactly two core assemblies" reference set is a set of **Broiler.VM-owned** assemblies: a profile component's own siblings are not members of it, which ADR 0011 P1 and ADR 0001 now state rather than leave to be inferred from a rationale that is entirely about excluding the runtime. |

Single-profile and multi-profile composition roots are explicit packages or samples, never a
runtime option that removes an already rooted profile. No new assembly is accepted merely to
shorten a file: it must enforce a dependency, AOT, deployment, ownership, test, or package
boundary.

The target direction is below; arrows mean **depends on**:

```text
Broiler.VM.Binary      ──→ (nothing)
Broiler.VM.Abstractions ─→ (nothing)
Broiler.VM.Runtime     ──→ Abstractions + Binary
Broiler.VM.Profile.X   ──→ Abstractions + Binary (+ Runtime contracts)
Broiler.VM.Profile.Y   ──→ Abstractions + Binary (+ Runtime contracts)
composition root       ──→ Runtime + the profiles it names
```

The verified graph may adjust names and split points, but it must retain these rules: the core
knows no concrete profile; no profile references another profile; nothing in the product graph
references a fixture or test project; and only a composition root knows which profiles it
includes. Broiler.VM references no legacy Broiler component, in either direction.

---

## 6. Artifact, verification, and versioning model

### Explicit descriptor, immutable verification result, and profile-owned payload

The verification API receives an immutable artifact descriptor plus caller-owned bytes. The
descriptor identifies the profile, profile-format version, feature-manifest ID, and any
artifact-requested limits. Those requests can only tighten the host/profile ceilings in section 7.
If an artifact omits a limit, it adds no restriction; it does not remove the materialized ceiling.
The selected profile owns decoding of the payload, and a caller that mislabels bytes receives that
profile's deterministic validation failure rather than a search for a decoder that accepts them.

Successful verification returns an opaque handle bound to the exact profile descriptor, feature
manifest, verifier/semantic version, effective verification/instantiation ceilings, core contract
version, and host-signature assumptions used during validation. The handle owns a byte snapshot or
fully decoded immutable representation; it never aliases mutable caller storage. Instantiate and
execute APIs accept only that handle. Sharing a handle across runtimes is allowed only when those
identities match and the profile declares the representation shareable; mutable instances,
memories, realms, feedback, imports, and host handles are never part of it.

VM-0 also fixes handle lifetime. A handle is either ordinary managed immutable data or owns
explicitly disposable resources; it cannot be ambiguously borrowed from a runtime. Where sharing
and disposal are both supported, explicit leases, idempotent disposal, and deterministic
use-after-dispose behavior prevent one runtime from invalidating another's input.

### Optional persisted envelope

If persistence is approved, ownership is split explicitly. The core owns a small bounded outer
header, profile dispatch, byte ownership, atomic storage and replacement, corruption reporting,
and compatibility of the outer schema, while treating the profile section as opaque. The profile
owns its payload, semantic cache-key contribution, compiler/debug metadata, migration,
invalidation, and composition-specific fallback.

The outer envelope plus the opaque profile section record at least: envelope magic and schema
version; profile ID, profile-format version, and feature-manifest ID; core contract version;
engine semantic/cache version; payload and section lengths with configured upper bounds; canonical
source/module identity and host-capability cache-key inputs; corruption-detection checksum data
and atomic replacement state; and optional debug metadata whose positions the profile validates.

It never persists object references, delegates, intern-table indexes, process-local identities,
warmed caches, quickened authoritative opcodes, or host handles. Loading always re-verifies the
envelope and the profile payload. Outer-envelope compatibility never implies profile payload
compatibility, and silently interpreting old bytes under new semantics is prohibited. A checksum
detects accidental corruption; it does not authenticate code. Hosts accepting artifacts from
outside their trust boundary must separately bind an approved hash, signature, or distribution
identity, and verification remains mandatory even then.

### Guest-initiated loads

A profile may need code the caller never supplied. The core treats that as an ordinary load
requested from an unusual place, not as a second execution path.

- The composition, not the guest, decides whether it is possible at all. A profile declares that
  it may request loads; the host either registers a typed artifact-provider capability or does
  not. A composition that registers none refuses every request deterministically.
- The provider returns a descriptor and bytes exactly as a caller would. The profile does not read
  files, open sockets, or invoke a compiler itself. A composition that includes a compiler
  supplies it behind the provider capability, which keeps invariant 6 intact and keeps the
  compiler inside the declared Native AOT closure.
- The returned bytes become their own immutable verified handle before anything in them runs.
  Nesting relaxes no bound, skips no descriptor match, and inherits no ceiling implicitly.
- Work is charged to the requesting operation. Nested verification and instantiation draw on the
  invoking operation's remaining fuel, time, and allocation allowance, and the nested handle's
  effective ceilings are the intersection of that remainder with the host and profile maxima. A
  nested load can exhaust an invocation; it can never enlarge one.
- Depth, fan-out, and cumulative nested bytes and verifier work have configured bounds. Detecting
  cycles in a dependency graph is the profile's problem; bounding recursion through the provider
  is the core's.
- Failures map onto existing categories rather than adding one. The nested load returns its own
  load/verification result, and the requesting operation reports the language-defined `profile
  fault`, or `host failure` and `resource exhaustion` when the provider or the budget failed
  rather than the artifact.
- The provider identity, capability version, and resolved artifact identity are cache-key inputs
  wherever a persisted envelope or semantic cache depends on them.

VM-0 freezes this contract even though the first core release ships no provider and no profile
that requests one. Retrofitting re-entrant verification into an already frozen lifecycle is a core
contract amendment; specifying it now is a paragraph.

---

## 7. Security, resources, and host boundary

Bytecode is untrusted input even when a local tool produced it. Verification and resource
accounting are part of correctness, not optional hardening.

### Lifecycle and result boundary

VM-0 freezes the state model and VM-1 implements it:

1. an immutable catalog is built by a composition root;
2. a runtime is created with typed host capabilities, authoritative resource ceilings, and
   declared affinity/reentrancy rules;
3. raw bytes, or profile bytes extracted from a bounded persisted envelope, are verified into an
   immutable profile-bound handle;
4. a verified artifact is instantiated into profile-owned mutable state;
5. execution or export invocation completes, suspends where the profile and host contract permit
   it, or returns a generic invocation outcome with a profile-owned typed payload;
6. a suspended operation resumes, is cancelled, or is disposed. Guest-initiated suspension resumes
   on the profile's own terms; external suspension resumes at the host's request and cannot be
   used to observe state the profile does not expose; and
7. cancellation and idempotent disposal transition sessions, instances, and any explicitly
   disposable verified handles to documented terminal states and reject later use
   deterministically.

Steps 3 and 4 may recur inside step 5 when a profile makes a guest-initiated load. The nested
operation is an ordinary instance of the same steps, runs under the requesting operation's
remaining budget, and may neither reorder nor skip them.

All public stages use a profile-neutral **operation-result envelope**, but their legal categories
are stage-specific:

- load/verification returns a verified handle or `invalid artifact`, `resource exhaustion`, or
  `cancellation`. Optional envelope loading is a bounded preprocessing step whose outer-schema,
  corruption, migration, profile, and version failures use `invalid artifact`; it never yields an
  executable handle or bypasses profile verification;
- instantiation returns an instance or `profile fault`, `resource exhaustion`, `cancellation`,
  `host failure`, or `suspension` where the profile's declared manifest permits asynchronous
  instantiation. VM-0 records whether core contract version 1 admits it, and adding it afterwards
  is an amendment; and
- invocation returns `normal`, `profile fault`, `suspension`, `cancellation`, `resource
  exhaustion`, or `host failure`. External suspension reuses `suspension` and adds no category,
  and neither does a guest-initiated load.

Selecting a profile the composition does not contain is not an invalid artifact: it is a distinct
`unsupported profile` outcome naming the requested ID and the catalog's contents. Conflating it
with a malformed payload misreports a composition mistake as a corrupt file, which is the most
likely diagnostic error for single-profile products.

Illegal lifecycle transitions and use-after-dispose return one stable core `invalid state`
outcome. Language faults are typed payloads owned and interpreted by their profiles; adding a
profile does not add a case to the common core. VM-0 also decides which calls may originate on
another thread, whether cancellation may be requested cross-thread, when reentrant execution is
rejected, whether external suspension may be requested and by whom, and how suspended state
retains and releases resources. The reentrancy rules must state explicitly whether a
guest-initiated load may re-enter the runtime that requested it.

### Ceilings are catalog-wide, and both halves of that are load-bearing

Effective ceilings are not resolved against the profile the caller selected. They are resolved
against **every profile in the catalog**, in two separate ways, and a profile author who knows only
the first will still write a defect:

1. **A runtime ceiling is clamped to the tightest profile hard maximum in the catalog**, per
   dimension, with no exemption for a profile that declares the dimension inapplicable; and
2. **adopting a profile default resolves to the tightest default in the catalog**, per dimension.

The consequence is the one VM-3 discovered by building a second profile rather than by reasoning:
a profile that declares its own usage as its maximum caps every profile composed beside it, and the
victim's verifier is refused for something it did nothing to cause. **A hard maximum is a statement
about your neighbours as much as about you.** What keeps a dimension unreachable is a profile's
import list and its budget declaration matrix, never a zero ceiling — and a dimension a profile
declares `NotApplicable` must still publish an unconstrained hard maximum, or it silently disables
that dimension for every profile beside it.

VM-0 records both terms in the resource-authority ADR that owns effective-ceiling materialization,
rather than leaving them in a topology finding, a composition register and an evidence bundle where
the algorithm that implements them does not cite any of the three. A profile roadmap states the
rule by citation and does not paraphrase it.

### Load-time requirements

- Checked arithmetic for every length, count, offset, index, and allocation calculation.
- Effective limits computed from profile hard maxima, host ceilings, and artifact requests before
  reading or allocating from an untrusted declared count; artifact metadata cannot raise a limit.
- **A bounded-read stop maps onto one outcome category, and which one is core-owned, not a matter
  of profile taste.** Each bounded-read status has exactly one correct answer: a framing or
  encoding failure is `invalid artifact`, and a breach of a configured ceiling that names a budget
  dimension is `resource exhaustion` naming that dimension and its scope. VM-0 rules the whole set
  in one place and VM-2 proves it, because the answer is what a retained corpus entry records: a
  miscategorised entry does not fail later, it passes and encodes the wrong answer, and every
  profile that copies the mapping copies the error. Conflating the two also misreports a host's
  declined ceiling as a malformed artifact, which is the diagnostic error most likely to be acted
  on wrongly.
- Bounds on artifact bytes, sections, constants, metadata, nesting, and aggregate verifier work.
- Deterministic rejection for unknown identifiers, sections, features, and versions.
- No allocation based on an untrusted declared count before the count passes its configured bound.
- Configured bounds on guest-initiated loads: nesting depth, fan-out per operation, cumulative
  nested bytes, and cumulative nested verifier work, each charged to the requesting operation.
- Successful verification owns or fully decodes its input. Unit and stress tests mutate, dispose,
  and concurrently overwrite the original caller buffer after verification and prove that the
  verified handle and execution result cannot change.

### Run-time requirements

- Per-instance or per-invocation fuel/cancellation polling, call/frame depth, allocation,
  host-call, and wall-clock budgets materialized from the verified handle and current host
  request. An omitted invocation override inherits the handle/runtime budget; an explicit override
  may only tighten it. Raising a verification/instantiation ceiling requires re-verification.
  Variable-work operations charge proportional work rather than one nominal instruction.
- Where a host creates several runtimes under one shared aggregate budget, fuel, wall-clock,
  allocation, and live-runtime counts are metered against the parent as well as each runtime.
  Exhausting the parent is reported as `resource exhaustion` to whichever operation observes it,
  and no runtime may be created or resumed once the parent has no remaining allowance.
- Host exceptions cannot tear down or corrupt another runtime; the core translates them according
  to the declared host contract.
- Runtime, artifact, and profile-owned state is reclaimed on dispose and reaches a measured memory
  plateau under repeated load/run/evict cycles.
- Concurrent runtimes share only immutable verified artifacts by default. Any mutable sharing
  requires a manifest-declared ownership/lease contract and the applicable VM-4 stress evidence;
  it is never inferred from using the same host registry.

### Host capabilities

Hosts register narrow typed capabilities explicitly. A profile import names a stable capability
ID, version, and signature; it cannot enumerate arbitrary CLR members. Capability lookup,
permissions, reentrancy, thread affinity, cancellation, and exception translation are part of the
cache key or runtime identity where they affect semantics. A shared host registry does not itself
bridge values between profiles or grant an ambient platform surface.

An artifact-provider capability is a distinct capability kind rather than an ordinary import: it
answers a guest-initiated load with a descriptor and bytes instead of a value. It is declared,
allowlisted, versioned, and audited separately; registering value capabilities never implies one;
and a composition that omits it makes every guest-initiated load fail deterministically.

**Host-exception translation has a precedence and the order is the core's.** An exception crossing
back from a capability is classified in exactly this order: a cancellation exception carrying the
operation's own token is `cancellation`; an exhausted meter at the moment of the catch is `resource
exhaustion`; anything else is `host failure` naming the capability. **This is not a new statement:
ADR 0011 already rules it**, as X1/X2/X3, "ordered and exhaustive - evaluate in order; stop at the
first match", in the record section 2 names as the owner of the host-capability class. It is repeated
here because two independently written profile roadmaps restated it from the implementation instead
of citing it, which is a discoverability defect rather than a missing ruling. A profile cites ADR
0011.

---

## 8. Sharing between profiles without a lowest-common-denominator core

Two profiles written independently will otherwise duplicate real work: bounded binary reading,
control-flow validation, dispatch scaffolding, position tables. The rule that keeps that from
becoming either duplication or a mushy common core is:

> **Share mechanism. Never share semantics.**

Mechanism is *how* something is done safely — reading an untrusted length, running a worklist to
fixpoint, charging a budget. Semantics is *what* the thing means — values, frames, types, opcodes,
syntax trees. Mechanism generalizes across languages because it is language-free. Semantics does
not generalize; an attempt to share it produces a lowest-common-denominator model that fits no
language well, which invariant 4 exists to prevent.

Applied to the concrete candidates:

| Candidate | Verdict |
|---|---|
| Bounded binary reading: checked arithmetic, variable-length integers, framing, allocation guards | **Core-owned from day one** (`Broiler.VM.Binary`). The core's own envelope needs it and every profile verifier needs it, so it has two consumers before any profile exists. |
| Descriptor matching, verified handles, limit intersection, envelope, lifecycle, budgets, diagnostics identity, host registry | **Already core-owned.** No new component; the rule is simply that a profile never re-implements them. |
| A verification framework: worklist and fixpoint over a control-flow graph, parameterized by the profile's abstract domain | **Extract later, do not predict.** Two verifiers will share the shape and share no domain. Open it when the second verifier exists and the duplication is measured. |
| Lexing and diagnostic *formatting* | **Only when a second text front end exists.** Until then it is one profile's private code wearing a shared name. |
| Positioned diagnostics and a stable code registry: a versioned registry bound in both directions, each code mapping to exactly one core reason, each emission carrying the core's position record | **Named mechanism, extract later, trigger re-cut.** The original trigger — a second text front end — can never fire: a binary-format profile emits positioned refusals and will never have a front end. The trigger is instead **the second profile that emits positioned refusals**, front end or not. Until then both profiles use the core's position record, state which of its fields they populate, and neither invents a parallel one. |
| The conformance-harness method: a pinned corpus revision, content-independent sharding, a recorded selection pipeline, a self-check with a passing control before every shard, a closed set of configuration failures, a failure manifest that is a queue, the harness's own regression suite, and a ratchet | **Named mechanism, extract later, and test-only either way.** Two profiles have specified it in near-identical words against two unrelated corpora, which is design-level evidence and not merged code. The trigger is the second profile's harness merging. Note that a shared harness is never referenced by a product package, so its extraction can never enter a product closure — which removes the usual objection but not the gate. |
| Assurance annotation, fingerprinting, review-state generation, and the evidence-bundle contract and collection script | **Repository policy, not a core component.** The core has one implementation and both profile roadmaps specify a byte-identical floor. The correct first move is a repository-level policy document that the components cite, not a shared assembly; the trigger for a shared tool is a second component's implementation existing to compare against. |
| The projection between the contract meter and the bounded-reading meter, and between a limit vector and the four artifact-shaped read bounds | **Documented duplication, kept.** It is four method bodies and one projection, it names no language concept, and it is already written once per profile in this repository — but the canonical copy lives in a test-only assembly no product profile may reference, so every profile writes it again. The core's obligation is to publish the canonical form in the profile-facing contract so the copies agree, not to add a package for it. |
| A shared syntax tree or parser (`Broiler.VM.Parser.AST` and similar) | **No.** A syntax tree is the most language-specific artifact in a pipeline, and a binary-format profile has no parser and no tree at all. Such a component would have one consumer and would encode one language's grammar into the core's namespace. |
| Shared value representation, frame layout, or opcode set | **No.** These are the semantics the core exists not to own. |
| A cross-profile value channel, so one profile can call another in the same process | **No, and this is the row a browser will push hardest on.** See the cross-profile boundary below: the price is paid at the embedder's seam, where it is visible, rather than inside a core that would have to learn two languages to charge it. |

**The extraction gate.** A new shared component is opened only when all four hold: two or more
**product** profiles already implement the behavior; the implementations are compared and the
shared part is identified from real merged code rather than anticipated; the shared part is
expressible without naming any language concept; and extracting it does not create a
profile-to-profile dependency. The fixture profile and the application-local consumer profiles do
not count toward the two: they are core-owned and deliberately shaped to fit the contract, so
agreement between them is evidence about the core's own tests. Failing any one condition, the
duplication is documented and kept. Duplicated mechanism is cheap; a wrong shared abstraction is
not.

**Failing the gate is a filing obligation, not a silence.** The first condition is unsatisfiable
until two product profiles exist, so for the whole of VM-0 through VM-6 every candidate above fails
it. That does not make the candidates unrecorded work: a refused extraction produces a dated record
in the ADR set naming the condition that failed and the condition that would reopen it, and each
duplicated implementation carries a source-level pointer to that record. A duplication with no
record is the failure mode this rule exists to prevent, because it is indistinguishable from
nobody having noticed.

**Invocation is ADR 0011's to assign and it assigns it broadly: any profile owner or the core
architecture owner may invoke the gate**, once two product profiles implement the behaviour in
merged code. An earlier draft of this paragraph narrowed that to the core architecture owner alone,
which contradicted the record rather than refining it, and the record is the authority. What is
genuinely this roadmap's to add is the division of labour underneath: a profile milestone *supplies*
its half — file paths, source revision, and a correspondence table against the other implementation
— and records that it supplied it. **The verdict is filed in the core ADR set** whoever invoked,
because a verdict changes the core graph and because neither profile may cite an identifier
belonging to the other. A profile that finds the second implementation has not merged records that
the first condition is unsatisfied and names what would satisfy it - a state the gate's own
"when the gate fails" clause does not cover, because invocation is barred until the first condition
holds. **An unrecorded state is a stop
condition; an unsatisfied first condition is not.**

### The cross-profile boundary, and why it is not an extraction question

A browser hosts two profiles at once and reaches one of them through the other. That is a real
product requirement and the core answers it with two facts, both frozen and both easy to discover
too late:

- **A guest-initiated load may not name another profile.** The provider must answer with an
  artifact of the profile that asked; a different profile is a provider contract breach reported as
  a host failure. One profile can therefore never reach another through the mediator, and no
  amendment to the mediator is the right way to ask for it.
- **Cross-runtime reentry is legal and depth-bounded.** A host object bridging two independent
  runtimes is admitted deliberately, which is the route a browser's `WebAssembly` surface actually
  takes: the embedder receives a call from one profile, converts arguments into the core's transfer
  types, invokes the other profile's runtime, and converts results back.

So the seam is the embedder's, every call across it is two host-boundary transits, and a shared
mutable region — the case that matters most in practice — has no core representation and is not
getting one. **The core's obligation is to make that price visible before the component that pays
it exists.** Two further obligations follow and belong to whichever component composes both
profiles: a two-profile composition is a named composition with its own closure report, RID matrix
and evidence, and a call chain that crosses runtimes is bounded only where those runtimes were
created under one shared aggregate budget — so a two-profile composition root creates one.

---

## 9. The intended first profiles

Neither profile is planned by this roadmap and no core gate depends on either. They are recorded
here so the contract is designed against real requirements rather than an imagined average
language, and so that a later profile roadmap can be written without renegotiating the core.

### JavaScript

Expected to require, beyond the baseline in section 4:

- **guest-initiated loads** for `eval`, the Function constructor, dynamic `import()`, and module
  specifier resolution;
- **asynchronous instantiation** if module graphs with top-level await are in its approved scope;
- **external suspension** if breakpoints and stepping are in its approved scope;
- **shared aggregate budgets** if Worker-style agents place several runtimes under one host
  ceiling; and
- suspension and resumption for generators and async functions.

Each of those is why the corresponding core contract exists. Together they are the reason section
2's amendment procedure is written before the first profile rather than after it.

It will also need a **test-only conformance-suite ingestion path**, for the same reason the other
profile does: its oracle is an external corpus distributed as source files, and the harness that
reads them is the largest piece of code that must never appear in a product. The core's obligation
is the general rule in section 14 — test tooling and corpus readers stay out of every product
package and Native AOT closure — and the rule is stated for both profiles rather than for one,
because an asymmetric note is how one roadmap ends up with a discipline the other lacks.

**It is also the profile a browser reaches WebAssembly through**, which is a requirement that
belongs to neither profile alone. Section 8's cross-profile boundary records what the core does and
does not provide for it; the reciprocal obligation on this profile is to declare the `WebAssembly`
host-object surface as a named exclusion rather than let a reader infer it from the presence of a
second profile in the same image.

**Seeding.** The JavaScript profile will be started from a **copy** of the legacy `Broiler.JS`
component taken after its in-flight fix programme completes, used as a base and template rather
than as a dependency. That decision carries conditions:

- the snapshot is a named commit, recorded in the profile's own roadmap, and it is taken only once
  the legacy fix work has landed and the core contract is accepted, so the copy is adapted to a
  stable contract instead of chasing one;
- the copy is a fork with its own history. No project reference, package reference, or shared
  source link runs between Broiler.VM and any legacy Broiler component in either direction, and an
  architecture test enforces that;
- fixes do not flow across the fork in either direction after the snapshot. Each side owns its own
  defects from that point, and neither is described as the other's upstream; and
- because the seed is a large existing codebase rather than a greenfield interpreter, the core's
  profile-facing contract must be reachable by code that was not written for it. VM-1 checks this
  by shaping the fixture profile's adapter after a non-trivial existing runtime, not only after a
  contract-shaped toy.

### WebAssembly

Expected to require, beyond the baseline in section 4: binary decoding and type/stack validation
of structured control flow; an immutable verified module distinct from each mutable instance;
typed imports and exports resolved through an explicit linker; traps as profile results rather
than process failures; imported memories, tables, and globals with declared aliasing and lifetime
rules across instances; and a pinned specification and conformance-suite revision that its own
manifest fixes.

It is also expected to need **no** parser, **no** text format, and **no** guest-initiated loads in
its first version, which makes it the useful counterweight when judging whether a proposed core
feature is genuinely general or is one language's need in disguise.

Its conformance corpus is distributed as text scripts, so its roadmap will need a test-only
ingestion path. The core's obligation is only the general rule already in section 14: test tooling
and text parsing stay out of every product package and Native AOT closure.

---

## 10. Where compilation lives

The core executes verified artifacts and produces none. Section 1 keeps source compilers, parsers,
and text formats out of it. This section records where they do live, so the gap is owned rather
than merely excluded. It plans nothing and, importantly, it requires nothing: **no tooling is
mandatory for a host to run source.**

A host that compiles at run time references the lowering assembly directly, exactly as it would
any other dependency, and calls the core's ordinary verification entry point with the result.
There is no command-line tool, build integration, or packaged toolchain anywhere on that path. A
browser is the worked example in section 11.

Two concerns hide behind the word *compiler*, and they do not share a home:

| Concern | Language-specific | Home |
|---|---|---|
| **Format** — opcodes, schema, encoder and decoder | yes | `Broiler.VM.Profile.<Language>.Format`, **only where a Broiler compiler exists for the language**; otherwise inside the profile |
| **Lowering** — source to bytecode | yes, and only where Broiler compiles the language itself | `Broiler.VM.Profile.<Language>.Compiler` |

**The format package is the pivot, and it is a pivot only where there are two parties to pivot
between.** A compiler and an executor must agree on the bytecode, and neither may depend on the
other, so the format is one authority that both reference. The compiler is a **sibling** of the
profile rather than a part of it, which is what makes an execution-only image contain a format, a
verifier, and an interpreter and no compiler at all. Where Broiler writes no compiler for the
language — because an external toolchain already produces the artifacts — there is no second party,
the format stays inside the profile per section 5, and creating a format assembly would be creating
an assembly to shorten a file. **The split is a consequence of compiling the language, not a naming
convention for profiles**, and a profile roadmap that copies the three-package shape without a
compiler has copied a rationale it does not have.

There is no generic compiler host with pluggable language profiles, and section 8's extraction
gate is why. A generic compiler core would own no compilation: parsing, analysis, lowering, and
optimization are all language-specific, leaving only driver plumbing. Participation is also
lopsided in a way execution is not — a WebAssembly profile consumes artifacts that external
toolchains already produce, so it contributes no Broiler compiler at all. Execution is genuinely a
many-language problem; compilation is a one-language problem wearing the same shape.

### No toolchain component yet

A command-line compiler, build integration, or packaged SDK would today have no consumer, and
section 8's extraction gate applies to the core's own surroundings as much as to shared profile
code. None is planned, and VM-0 chooses no name for one.

The trigger for revisiting is a product that must ship precompiled artifacts with **no compiler in
its image** — a dynamic-code-prohibited or size-constrained composition. At that point compiling
somewhere other than the running host has a real consumer and a real closure to justify, and the
component is opened then against that requirement rather than against an anticipated one.

Two properties are worth holding from the start anyway, because both are cheap now and unpleasant
to retrofit:

1. **One verifier, ever.** Whatever eventually verifies an artifact calls the profile's verifier.
   A build-time reimplementation that is merely supposed to agree with the runtime is a security
   defect with a schedule attached. This costs nothing today, when the runtime is the only
   verifier, and it forecloses the tempting shortcut later.
2. **Deterministic lowering.** The same source, compiler version, and format version produce a
   byte-identical artifact. No consumer requires this yet — a host's own cache keys on source and
   versions rather than on output bytes — but retrofitting determinism means auditing every
   iteration order, timestamp, and identity-derived value in a finished compiler. Preserve it;
   do not build machinery for it.

A third rule follows from the composition model rather than from tooling: **one lowering, however
many hosts.** Where a composition compiles at run time, and where one is later added that compiles
ahead of time, both use the same lowering assembly. The composition decides which is present; the
code is not written twice.

---

## 11. Embedding: how source reaches the core

A host that starts from source rather than from an artifact needs a defined path to one. The
browser is the demanding case and the one to design against.

**A browser is always a runtime-compiler composition.** There is no ahead-of-time path for the
open web, because a page cannot be compiled before it is visited. Its composition links the
parser, front end, and lowering into the image, and its Native AOT gate proves that closure
publishes and runs — not the smaller execution-only one.

**The host keeps its own seam.** An embedder already talks to script through its own interface in
terms of source text, a document or resource identity, and a realm. That interface does not
change: an adapter behind it compiles to the profile's format, verifies, instantiates, and
invokes. The embedder never handles bytecode, and swapping the engine behind the seam stays a
bounded change.

**Source arrives in two directions, and each already has a contract:**

- **Caller-driven.** The host found the script, fetched it, and decides when it runs. Nothing is
  executing yet, so the adapter compiles, verifies, and invokes directly. Top-level and deferred
  scripts take this path.
- **Guest-driven.** Code is already running when it asks for more — `eval`, a function
  constructor, a dynamic import, a module dependency. This is the guest-initiated-load contract in
  section 6: the host registers an artifact-provider capability, and the core mediates, bounds,
  and charges each request to the operation that made it.

**The division of labour is strict.** The host owns identity resolution, transport, content
policy, integrity checks, the module map, and the event loop; the core never fetches anything. A
useful consequence for a browser is that a content policy forbidding dynamic evaluation is
expressed by registering no artifact-provider capability, so the refusal is a contract outcome
rather than an ad-hoc check inside an engine.

**The code cache is the persisted envelope.** Compile once, key by source identity, compiler
version, and format version, store the envelope, and skip compilation next time. Because verified
handles are immutable and shareable across runtimes with matching identity, two realms running the
same script can share one compiled artifact instead of compiling it twice.

### Three decisions this forces on VM-0

Each is cheap to settle now and expensive to retrofit, and each is invisible until a host with a
latency budget arrives.

1. **Whether locally produced bytecode must round-trip through bytes.** Invariant 3 is written for
   bytes that came from outside. When the compiler that produced them is in the same process and
   inside the same trust boundary, serializing and re-decoding on every load is pure critical-path
   cost. VM-0 decides whether the format may expose a compile-directly-to-verified-handle path
   that skips serialization while still running every type and control-flow check, or whether the
   round trip is mandatory. VM-5 measures verification throughput so the choice rests on numbers.
2. **Whether verification may be lazy per section.** Hosts that compile function bodies on first
   call do not want to verify an entire bundle to run one entry point. Either the profile verifies
   eagerly, or the format supports independently verified sections, each verified before its own
   first execution so that nothing unverified ever runs.
3. **Whether an artifact may be verified incrementally as it arrives.** The contract today is
   whole-bytes to handle. Streaming would be a core contract amendment; deciding it deliberately
   beats discovering it during a latency regression.

### Verification stays separable from execution

No second, tool-shaped API is designed for this. The lifecycle in section 7 already makes
verification its own step: an artifact becomes a handle before anything instantiates or executes
it, so verifying without running is an ordinary use of the host surface rather than a new
contract. An embedder that validates a cached artifact before trusting it, and any future
out-of-host compile step, both reach for the same entry point.

VM-0 records that separation as deliberate and required to survive, which is what keeps section
10's one-verifier property reachable without designing a surface for consumers that do not exist.

---

## 12. Relationship to the legacy Broiler.JS component

`Broiler.JS` is a **legacy component**. It keeps its own roadmap, its own status ledger, its own
release cadence, and its current consumers, and it is not part of Broiler.VM's graph, gates, or
evidence. Broiler.VM does not depend on it, extend it, wrap it, or replace it on any schedule
stated here.

Consequences worth stating plainly, because the alternative is discovering them later:

- **Two engines coexist.** The shipping browser stack keeps using the legacy component while
  Broiler.VM and its profiles are built. Retirement, migration, or indefinite coexistence is a
  product decision recorded outside this roadmap; nothing here assumes any of the three.
- **Legacy is not frozen.** It continues to gain language features and fixes on its own schedule.
  Two things follow. A seeding snapshot is a point-in-time fork that diverges further with every
  legacy release, so the later it is taken the more it carries and the more it costs to adapt.
  And the legacy component's performance characteristics are a property of that component, not an
  argument for this one: no core gate is justified by them, and section 16 keeps an unmeasured
  speed-up out of the case for Broiler.VM.
- **No evidence transfers.** Legacy conformance results, benchmarks, and Native AOT samples are
  not Broiler.VM evidence, and no core gate may cite them.
- **The legacy IL/bytecode differential is not a core oracle.** A future profile chooses its own
  oracle; the core's oracle is the fixture profile and its own contract tests.
- **Copying is allowed; depending is not.** Section 9's seeding conditions govern any code taken
  from the legacy component.

---

## 13. Milestones

Current state lives in [the status ledger](roadmap.status.md), which is the authority for what has
been accepted. This section states planned work and objective exit gates only; a milestone is
never complete because its design appears here.

### VM-0 — Freeze ownership, terminology, and the build-proven graph

- **Owner:** Broiler.VM architecture owner, with release/AOT review of the composition roots.
- **Next action:** Write the boundary ADR and project-shell spike. Pin terminology, dependency
  direction, package hypotheses, stable ID policy, the minimum lifecycle and profile-neutral
  operation-result contracts, trusted resource-limit precedence, and immutable
  raw-payload/envelope ownership. Assign core contract version 1 and publish its amendment
  procedure. Decide and record the guest-initiated-load contract, the artifact-provider capability
  shape, the external-suspension transitions, whether asynchronous instantiation is admitted, and
  whether aggregate budgets are a core object or a host responsibility — each explicitly, even
  where the first release ships no implementation. State the section 4 profile-facing checklist
  and the section 8 sharing rule and extraction gate. Record the legacy boundary in section 12 as
  an architecture-tested rule, not a convention. Record that verification is separable from
  execution and must stay so, and settle section 11's three embedding decisions: whether locally
  produced bytecode must round-trip through bytes, whether verification may be lazy per section,
  and whether an artifact may be verified incrementally as it arrives.

  Then settle seven further questions that are a sentence now and a defect in a shipped profile
  later. Each is here because it is reachable from the implementation and absent from the record
  that owns it, or because two independently written profile roadmaps answered it differently:

  1. **The bounded-read status mapping**, ruled once for the whole set, in the resource-authority
     and artifact-ownership records rather than in a source comment that attributes itself to
     records not carrying it. Every ceiling breach that names a budget dimension is `resource
     exhaustion` naming that dimension and scope, or it is not, but the four artifact-shaped
     ceilings are ruled together and not split arbitrarily.
  2. **Both catalog-wide ceiling terms** — the tightest hard maximum and the tightest adopted
     default — written into the ordered precedence algorithm that owns effective-ceiling
     materialization, with the "publish an unconstrained maximum on a dimension you declare
     inapplicable" rule stated as a profile obligation.
  3. **Host-exception translation precedence**, written down rather than left to be re-derived
     from the implementation.
  4. **Whether a profile's own sibling assembly counts inside the frozen two-core-assembly
     reference set**, and the reconciliation of section 5 with section 10 on where a format lives.
     No profile can take its placement decision until this is answered.
  5. **Who invokes the extraction gate, and where the record is filed** — including the case where
     the first condition is unsatisfied, and the fact that neither profile may cite the other, so
     the record can only live here.
  6. **The candidate-amendment register brought level with what the profiles have asked for**,
     including the charging hook for work done inside a host capability, which is the one candidate
     both intended profiles independently rate general, and the argument channel, whose scope and
     strength the two profiles currently record differently.
  7. **Whether a retained-state dimension can carry a guest-observable refusal.** It cannot today —
     the retention report returns nothing and the refusal is latched for the next charge or poll —
     and a profile whose language requires an observable refusal must gate on a charge instead.
     Recording it costs a sentence; discovering it costs a memory representation.
- **Dependencies:** Named ownership for the core contract and its amendments. No dependency on any
  profile, on the legacy component, or on the legacy component's in-flight work.
- **Objective exit gate:** An acyclic shell graph builds; architecture tests express every
  forbidden edge, including any edge to a legacy Broiler component; the ADR names package and
  composition roots, profile/version semantics, RIDs, security ownership, lifecycle states,
  result/payload ownership, resource authority, verified-artifact ownership, and the supported
  source-level profile contract; core contract version 1 is assigned and its amendment procedure
  is published; the guest-initiated-load, asynchronous-instantiation, external-suspension, and
  aggregate-budget questions each carry a recorded decision rather than silence; verification is
  separable from execution and recorded as required to stay so; and section 11's round-trip,
  lazy-section, and incremental-verification decisions are recorded with the reasoning that
  settled them. The seven questions above each carry a dated decision in the record that owns the
  subject, not in a source comment and not in an evidence bundle; the sharing table's triggers each
  name a condition that can actually fire; and the cross-profile boundary is recorded with its
  two frozen facts, its named unowned costs, and the statement that a two-profile composition is a
  composition with its own evidence rather than an emergent property of composing two roots.

### VM-1 — Build the semantics-neutral runtime, catalog, and fixture profile

- **Owner:** Broiler.VM core/runtime owner.
- **Next action:** Implement the contracts, bounded binary primitives, builder, descriptor
  validation, direct factory catalog, per-runtime executor creation, profile-neutral
  operation-result envelopes, typed profile payload boundary, limits, cancellation, diagnostics,
  lifecycle states, and thread-affinity and reentrancy rules. Implement whichever of
  guest-initiated-load mediation, artifact-provider registration, external-suspension transitions,
  and aggregate budget metering VM-0 assigned to the core, including their refusal paths. Build
  the fixture profile as the primary proof vehicle and shape its adapter after a non-trivial
  existing runtime, so the contract is not accidentally fitted to a toy.
- **Dependencies:** VM-0 graph and ADR.
- **Objective exit gate:** Core and catalog tests prove deterministic registration,
  duplicate/alias rejection, unknown-profile and unsupported-version failures, catalog-order
  independence, per-runtime state isolation, legal and illegal lifecycle transitions,
  cancellation and disposal behavior, declared thread affinity and reentrancy, typed
  profile-payload preservation, and the explicit absence of reflection or name-based discovery.
  The fixture profile exercises a guest-initiated load through a fixture provider, deterministic
  refusal where no provider is registered, external suspension and resume, and aggregate budget
  exhaustion across several runtimes. Trimmed and Native AOT test hosts construct the fixture
  profile through the generic contract. The accepted contract is recorded with its version.

### VM-2 — Establish bounded artifacts, verification, and resource enforcement

- **Owner:** Broiler.VM core security owner.
- **Next action:** Implement descriptor and profile matching, bounded outer-envelope parsing where
  approved, the opaque immutable verified-artifact handle, trusted host/profile/artifact limit
  intersection, explicit default and omission behavior, invocation-only tightening, deterministic
  failure classes, a fixture malformed corpus, and fuzz entry points. Bound guest-initiated loads:
  depth, fan-out, cumulative nested bytes and verifier work, charging to the requesting operation,
  and intersection of the nested handle's ceilings with the remaining allowance.
- **Dependencies:** VM-1 runtime and catalog; VM-0 artifact and resource ADR.
- **Objective exit gate:** Truncated, corrupt, oversized, mismatched, unknown-version, and
  resource-hostile fixture artifacts fail before execution without out-of-budget allocation.
  Effective policy is computed before allocation and never exceeds the host ceiling. Execution
  consumes only the verified handle; tests mutate, dispose, and concurrently overwrite the
  caller's original buffer after verification without changing behavior. Unit, property, and fuzz
  suites retain minimized regressions, and the same failure categories are stable in JIT, trimmed,
  and Native AOT hosts. Omitted limits inherit materialized bounded policy, invocation overrides
  only tighten it, and a raised ceiling requires a newly verified handle. A fixture guest-initiated
  load cannot exceed, extend, or escape its requesting operation's budget; recursive and fan-out
  provider requests terminate at their configured bounds; and a composition with no registered
  provider refuses every request deterministically.

### VM-3 — Prove the public profile contract and exact composition closures

- **Owner:** Broiler.VM architecture and developer-experience owner with release engineering.
- **Next action:** In a separate consumer project, implement an application-local profile using
  only the public source contract, and compose it by direct typed registration. Reserve the
  `Broiler.*` ID namespace and require an application-owned reverse-domain namespace for consumer
  profiles. Validate catalog and descriptor governance, direct factories, package roots, and the
  exact closure of each named composition. Prove that adding a second profile requires no change
  to the core runtime or execution loop.
- **Dependencies:** VM-1 and VM-2 with stable public-candidate descriptor, verified-artifact, and
  executor contracts. No dependency on a product profile.
- **Objective exit gate:** A consumer profile is added without changing the core runtime, the
  execution loop, or any Broiler-owned package, and without reflection, name-based loading, or an
  extension directory. Single-profile and two-profile compositions each publish and run under
  trimming and Native AOT, and each closure report contains exactly the declared profiles and no
  fixture or test assembly. CI detects duplicate or reserved IDs, undocumented entries, missing
  factories, forbidden edges, and catalog drift. The source-compatibility promise this exposes is
  frozen in VM-6; no binary plug-in ABI is implied.

### VM-4 — Harden lifecycle, concurrency, diagnostics, and host integration

- **Owner:** Broiler.VM runtime owner with host-integration and concurrency owners.
- **Next action:** Validate and harden the lifecycle, affinity, reentrancy, cancellation, result,
  and disposal rules frozen in VM-0/VM-1. Test independent runtimes and multiple fixture profiles
  under create/verify/instantiate/run/suspend/resume/cancel/dispose loops; enforce host capability
  allowlists and typed signatures; attach stable artifact and position diagnostics; and measure
  reclamation of frames, artifacts, interned data, and caches. Stress guest-initiated loads under
  cancellation and disposal, external suspension and resume including a client that abandons a
  paused operation, and aggregate budget exhaustion across concurrent runtimes.
- **Dependencies:** VM-1 through VM-3.
- **Objective exit gate:** Stress and soak suites show deterministic isolation, bounded
  cancellation, correct host-exception translation, no cross-runtime state leakage, no
  use-after-dispose, and a declared memory plateau. A guest-initiated load in flight is cancelled
  and disposed with its requesting operation and leaves no partially verified state; an externally
  suspended operation resumes, cancels, or disposes deterministically and never blocks disposal
  indefinitely; and a shared aggregate budget is honored by concurrent runtimes rather than
  multiplied by them. Diagnostics identify profile, version, and artifact locations without
  leaking host secrets. Host imports cannot reach undeclared CLR surface.

### VM-5 — Baseline the core's own overhead

- **Owner:** Performance owner with the core runtime owner.
- **Next action:** Take uninstrumented decision-grade baselines of everything the core costs a
  profile: verification throughput per byte, catalog construction and lookup, runtime creation and
  disposal, budget metering overhead per operation and per host call, guest-initiated-load
  mediation, envelope read and write, diagnostics capture, startup, image and package size, and
  resident-set plateau. Measure on JIT and Native AOT with the fixture profile.
- **Dependencies:** VM-2 for verification, VM-4 for lifecycle. No dependency on a product profile.
- **Objective exit gate:** Each measurement has a predeclared rule, a comparable control, an A/A
  lane validity check, and retained repetitions. The core publishes what it costs so a profile can
  budget against it, and states plainly that no language performance claim follows from any of it.
  Optimization is funded only against one of these baselines.

### VM-6 — Package, publish, and continuously recertify the core

- **Owner:** Broiler.VM release owner with package, security, API, and documentation owners.
- **Next action:** Finalize only the package boundaries justified by VM-0 evidence; create
  pristine feed consumers and samples that use public APIs only; freeze the public API, the
  source-level profile contract, the core contract version, and the artifact promises; publish
  support and exclusion tables; complete dependency, license, security, and human review; and wire
  graph, catalog, AOT, and contract drift checks into required CI and the status ledger.
- **Dependencies:** VM-0 through VM-4. VM-5 is required only where a product threshold says the
  measured core overhead is unshippable.
- **Objective exit gate:** Every advertised package restores from a feed without repository
  project references; the public API and package graph match the baseline; all malformed-input and
  contract suites pass; every claimed RID publishes and runs the declared compositions with
  warnings as errors; notices and reviews are complete; rollback is tested; and recertification
  triggers are documented. The support table states the core contract version and states that the
  core ships no language profile.

### Delivery order

```text
VM-0 graph, ownership, core contract version 1
  └→ VM-1 neutral runtime, catalog, fixture profile
       └→ VM-2 immutable artifact, verification, resource boundary
            ├→ VM-3 public profile contract and exact closures
            │    └→ VM-4 lifecycle, concurrency, diagnostics hardening
            │         ├→ VM-5 core overhead baselines
            │         └→ VM-6 package, publish, recertify
            └→ (profile roadmaps begin against the accepted contract)
```

A profile roadmap may begin as soon as VM-1's contract is accepted, and its own gates belong to
its own component. Nothing in VM-0 through VM-6 waits for a profile, and no profile result closes
a core gate.

---

## 14. Test and evidence matrix

| Area | Required tests/evidence | Failure that blocks release |
|---|---|---|
| Core/catalog | duplicate, alias, unknown and reserved IDs; version and core-contract-version mismatch; explicit selection; order independence; factory identity; application-local fixture; profile-neutral outcomes and typed payload preservation; **a two-profile catalog test proving that neither profile's declared maxima or adopted defaults refuse the other's ordinary artifact** | reflection or name discovery, silent replacement, core reference to a concrete profile, an undeclared or forked core contract version, catalog drift, or **one profile's declaration capping a dimension it declares inapplicable** |
| Dependency architecture | acyclic graph; core references no profile; no profile references another; no product package references a fixture or test project; no edge to a legacy Broiler component in either direction | any forbidden project or assembly edge, or undeclared dynamic loading |
| Artifact safety and policy | truncation, invalid sizes, indexes and framing, corrupt envelope, post-verification caller-buffer mutation, disposal and concurrent overwrite, verified-handle identity and lease lifetime, explicit default adoption, omitted-limit inheritance, host/profile/artifact intersection, invocation-only tightening, guest-initiated-load depth, fan-out and cumulative bounds, nested budget charging, missing-provider refusal, minimized fuzz corpus; **every bounded-read status produced by a named case and mapped onto its one ruled outcome category, asserted identically across every verifier in the graph** | invalid input executes, caller mutation changes execution, one runtime invalidates another's handle, omission becomes unbounded, policy raises a verified ceiling, a nested load enlarges or escapes its requesting operation's budget, a provider-less composition executes acquired bytes, **a ceiling breach reported as a malformed artifact or a framing failure reported as exhaustion**, **two verifiers in one graph answering one status differently**, unbounded allocation, crash, hang, or nondeterministic failure class |
| Persistence ownership | core outer-schema compatibility, rejection and migration; header and profile dispatch; atomic corruption handling; profile payload and cache-key boundaries; content authorization separate from checksum | ambiguous migration owner, outer compatibility mistaken for payload compatibility, torn update treated as valid, or checksum treated as authenticity |
| Lifecycle/concurrency | frozen state transitions; repeated verify, instantiate, run, suspend, resume, cancel and dispose; external suspension, resume and abandonment; guest-initiated load under cancellation and disposal; independent runtimes; multiple fixture profiles; thread affinity; reentrancy; shared aggregate budget exhaustion; memory plateau | profile-specific state leaks into the core result enum, shared mutable leakage, race, unbounded retention, use-after-dispose, an externally suspended operation that cannot be resumed, cancelled or disposed, concurrent runtimes multiplying a host ceiling, or unbounded cancellation latency |
| Host security | typed allowlist, signature mismatch, permission denial, thread affinity, host exception translation, artifact-provider allowlist and its absence, secret-safe diagnostics | arbitrary CLR discovery or access, a provider reachable without declaration, a tool reached outside the declared closure, or cross-runtime capability leak |
| Native AOT | every named composition, the application-local profile consumer, and each declared RID; warnings and suppressions inventory; shipped dependency-closure audit | a claimed composition fails publish or run, reaches forbidden dynamic code or test tooling, or loses a directly rooted profile or capability |
| Packaging | pristine feed restore, build and run; API and package baselines; dependency, license and notices; image and package sizes | repository-only success, undeclared dependency, missing notice, or a language capability implied by package or API |
| Core overhead | uninstrumented candidate and control identity, A/A lane validity, per-operation attribution, allocation, GC, RSS, startup, image and package size | a claim without a predeclared rule, a comparable control, or retained repetitions |

Generated results are evidence artifacts, not substitutes for pinned manifests and durable
summaries. Every accepted bundle records source revision, clean or dirty inputs, SDK and runtime,
publish properties, core contract version, RID and device, effective GC/JIT/AOT state, commands,
and raw outputs.

---

## 15. Release gates

A Broiler.VM core preview or stable release must satisfy all applicable gates:

1. **Support truth:** the public table names the core contract version, the compositions,
   host capabilities, guest-initiated-load and external-control support, RIDs, and deterministic
   exclusions separately, and states that no language profile ships with the core.
2. **Graph and registration:** the generated dependency closure matches VM-0, the catalog is
   static and documented, the generic runtime references no concrete profile, no product
   composition reaches dynamic loading or IL emit, and no edge reaches a legacy Broiler component.
   The public source-level profile contract and ID namespace pass VM-3; no binary plug-in ABI is
   implied.
3. **Correctness and safety:** the malformed corpus, fuzz regressions, immutable verified-artifact
   boundary, trusted limit intersection, guest-initiated-load bounds, lifecycle, and host-security
   suites pass against the fixture and application-local profiles.
4. **Lifecycle and results:** the frozen ownership, state-transition, affinity, reentrancy,
   suspension, resumption, external-control, guest-initiated-load, cancellation, and disposal
   rules pass at the declared core contract version. Language outcomes remain typed profile
   payloads behind profile-neutral envelopes, and a guest-initiated load adds no core result
   category and cannot exceed its requesting operation's budget.
5. **Native AOT:** each advertised composition publishes and runs on its declared matrix with
   trim and AOT warnings treated as errors. Suppressions are reviewed and scoped.
6. **Packages and consumers:** packages restore from a feed, samples use public APIs, API and
   package baselines and notices are current, and every closure matches its claim.
7. **Operations and persistence:** diagnostics, cancellation, rollback, format-version rejection,
   envelope recovery, vulnerability response, and recertification owners are named.
8. **Measurement honesty:** core overhead is published with its method, and no language
   performance is claimed or implied.
9. **Human review:** no package is published, no RID is claimed, no support table is issued, and
   no milestone moves to accepted until a named human has recorded a decision on every relevant
   code unit, bound to that declaration's fingerprint. This gate is stated here rather than only in
   the ledger's update rules, because a release gate that lives only in an update rule has to be
   discovered from a milestone row instead of read off the list a release is checked against.
10. **Licence and attribution:** notices are current and correctly scoped, and no standing
   third-party claim this component publishes is falsified by what any component that ingests or
   copies from it ships **or by what its tree contains**. A component that vendors or copies
   third-party source is named in the notice in the same change that introduces it.

Recertification is required when the SDK or runtime, core contract version, package graph, host
capability surface, Native AOT settings, RID matrix, cache identity, resource defaults, or
representative workload changes.

---

## 16. Risks and stop conditions

| Risk | Mitigation / stop condition |
|---|---|
| The core becomes a lowest-common-denominator language runtime | Keep opcodes, values, frames, verifier rules, and semantics profile-owned. Apply section 8's extraction gate before sharing anything, and reject a shared primitive that introduces a profile-to-profile dependency or a semantic conversion tax. |
| One profile's declared maxima or defaults silently cap another profile composed beside it, and the victim's verifier is refused for something it did not cause | Both catalog-wide terms are stated in the record that owns effective-ceiling materialization; a dimension declared inapplicable still publishes an unconstrained maximum; and a two-profile catalog test catches it rather than a reader. **Stop: a maximum or default set to a profile's own usage is a composition defect, and a composition that hosts two profiles does not close a gate until that test exists.** |
| The extraction gate is never invoked, so duplicated mechanism accumulates with no record of why | A refused extraction produces a dated record naming the failed condition and the condition that reopens it, and each duplicated implementation points at it. Invocation belongs to the core architecture owner; a profile supplies its half and records no verdict. **Stop: the verdict may be either, and an unsatisfied first condition is not a failure — but an unrecorded state is.** |
| Guest-controlled cost that grows with its input is charged flatly, so a bounded budget bounds nothing | Per-family declared monotone charging functions with a declared granularity and a ceiling floor, each with a retained fixture and an unsimplified control. The core cannot check this and says so; the compensating control is the evidence bundle. **Stop: an operation family without a proportionality fixture does not ship.** |
| Declared scalars — depth ceilings, uncharged-work bounds, charging granularity, poll bounds — are round figures rather than measurements | Each is derived from a retained, reproducible measurement on each claimed RID and recorded with it. **Stop: a stack overflow is not translatable into a result, so claiming to handle deep recursion without a measured bound is an untruthful capability claim.** |
| Owner and reviewer are the same person, so no gate here is independently confirmed | Roles are named per milestone and per record; where one person holds several, the non-independence is recorded as a residual limit on what these gates prove rather than resolved by assertion. **Stop: a vacant role stops the point that requires it; a role held by nobody does not pass to whoever is available.** |
| A profile's programme stalls on a precondition this component holds, and the blocker is described as scheduling | The core is the named holder of the contract-acceptance blocker for every profile. Each blocked milestone is recorded blocked with its holder and its unblock condition. **Stop: lack of scheduling is not a blocker; an unaccepted contract is.** |
| A core designed with no real profile fits no real profile | Prove every gate against a fixture profile shaped after a non-trivial existing runtime, keep section 9's requirement lists current, and treat a profile that cannot be expressed through section 4 as a contract defect rather than a profile problem. |
| An approved profile capability does not fit the frozen core contract | Amend it: mint the next core contract version, state what changed, and recertify affected evidence. Do not add a language-specific path to the core state machine, and do not maintain a second core contract per profile. |
| The core result enum grows one case per language | Keep only profile-neutral outcome categories in the core and carry language outcomes as typed profile payloads. Reject a profile that requires the core execution loop to learn its semantics. |
| Guest-initiated loading becomes an unverified or unbounded back door | Route every acquired byte through ordinary verification, reach the host only through a declared artifact-provider capability, charge nested work to the requesting operation, and bound depth, fan-out, and cumulative bytes. A composition with no provider refuses deterministically. |
| A compiler or tool is reached from inside a profile | Keep it behind the artifact-provider capability so it stays inside the declared composition and Native AOT closure. A composition that declares no tool has no path to one. |
| Concurrent runtimes multiply a host ceiling | Meter fuel, wall-clock, allocation, and live-runtime counts against a shared aggregate budget as well as each runtime, and refuse creation and resumption once the parent allowance is spent. |
| External pause becomes an unbounded or privileged side channel | Declare who may request external suspension, keep it distinct from guest suspension and terminal cancellation, bound how long a paused operation may block disposal, and leave what a paused profile exposes to the profile. |
| Static registration silently stops being extensible | Prove an application-local consumer profile through the public source contract, governed IDs, catalog drift tests, and direct composition roots. Do not replace compile-time extensibility with reflection or imply a binary plug-in ABI. |
| Trimming removes a profile or host path | Root factories and capabilities directly and publish and run every named composition. A linker annotation without execution is insufficient. |
| A second verifier appears at build time | Keep verification separable from execution on the ordinary surface so nothing needs its own, and hold section 10's one-verifier property. Two verifiers that must agree are a security defect with a schedule. |
| A host's critical path pays for contracts written for untrusted input | Settle section 11's round-trip, lazy-section, and incremental-verification decisions in VM-0 and measure verification throughput in VM-5. A latency regression discovered after the contract is frozen costs an amendment. |
| Legacy code is copied into a profile and quietly becomes a dependency | Enforce section 9's seeding conditions with an architecture test on the graph, record the snapshot commit, and state that fixes do not flow across the fork. |
| The legacy component is treated as Broiler.VM evidence | No core gate may cite legacy conformance, benchmarks, or AOT samples. Section 12 is a gate, not a preference. |
| Malicious input exhausts the verifier or runtime | Checked and bounded readers, pre-execution verification, fuel and cancellation, depth and allocation budgets, fuzzing, and stable resource failure results are release gates. |
| An artifact weakens host policy by declaring larger limits | Treat the host ceiling as authoritative, allow the profile to tighten it, allow the artifact only to request less, compute the intersection before allocation, and record the effective policy in the verified handle. |
| Caller-owned bytes change after verification | Snapshot or fully decode into an immutable profile-bound handle and execute only that handle. Mutation, disposal, and concurrent overwrite tests are release blockers. |
| Internal formats become accidental public contracts | Version from the first byte and promise persistence only after its explicit gate. Reject unsupported versions deterministically. |
| The core is justified by unmeasured performance | Capability and correctness come first. The core publishes only its own overhead and never a language claim. |

Stop or re-scope a milestone when the graph is cyclic, a product closure reaches dynamic code,
test tooling, or a legacy component, a verifier cannot produce an immutable bounded representation
before execution, trusted policy can be weakened by artifact input, a second core state machine is
maintained for one language, the declared Native AOT composition cannot publish and run, or the
named ownership or maintenance ceiling is absent. A difficult or slow milestone is not itself a
stop condition; an untruthful support claim is.

---

## 17. Platform references

VM-0 records immutable revisions for implementation and release evidence; these moving links are
discovery entry points, not substitutes for the pinned manifests:

- [.NET Native AOT deployment and limitations](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [.NET Native AOT warning guidance](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/fixing-warnings)
- [.NET trimming options and analysis](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options)

A profile's own specification references belong in that profile's roadmap, not here.
