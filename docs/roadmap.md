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
profiles. Each is a family of product projects **in this component**, at
`src/Broiler.VM.Profile.<Language>*`, with its own roadmap, ledger, decisions and gates in the
project directory whose assembly they describe — not a component with a repository of its own.
Their plans are open and are not this document's; no row of this roadmap tracks one, and no profile
result closes a core gate. Section 9 records what those profiles are expected to require, so the
contract is designed for them rather than retrofitted to them. Sections 10 and 11 record where
compilation lives and how a host gets from source to an artifact without any tooling, and
section 12 records the boundary against the legacy `Broiler.JS` component, which Broiler.VM does
not depend on.

---

## 1. Terminology and support claims

The repository already uses the word *profile* for several different boundaries. Documents, APIs,
test names, and release manifests must qualify the term when ambiguity is possible.

| Term | Meaning in this roadmap |
|---|---|
| **VM profile** | A bytecode language plus its format, feature/version manifest, verifier, value/frame model, and executor. It is a **family of product projects in this component**, at `src/Broiler.VM.Profile.<Language>*`, that references the core; the core never references one, and no profile references another. *(Revised 2026-08-31: this row read "a separate component", written when no product profile existed. ADR 0001's dated revision rules that a language profile is a set of product projects in Broiler.VM rather than a component of its own, and rule A11 was changed in the same revision to admit a sibling in the same profile family. What is unchanged is every direction of dependency this row states.)* |
| **Core contract version** | The numbered revision of the profile-neutral lifecycle, operation-result, resource-authority, guest-initiated-load, external-control, and host-capability contract frozen by VM-0 and implemented by VM-1. Profiles and artifacts version independently of it. |
| **Feature manifest** | The exact language surface accepted by one version of a profile. The core fixes the manifest's shape and identity rules; a profile fixes its content. A profile name alone is never a conformance claim, and neither is a manifest name: a manifest claims only what its own retained oracle run shows. |
| **Built-in profile** | A profile whose factory and dependencies are directly referenced at build time and rooted in a static catalog. A built-in may be Broiler-provided or application-local; it is never discovered at run time. |
| **Fixture profile** | A deliberately trivial profile owned by the core's own tests. It proves contracts, closures, and failure paths without waiting for a product profile, and it never ships in a product package. |
| **Verified artifact** | The opaque, immutable, profile-bound output of successful verification. Execution and instantiation consume this handle, never caller-owned raw bytes. |
| **Artifact output form** | The kind of payload a verified artifact carries. **The form is a property of the artifact and not of the runtime**: the compiler chooses it, verification fixes it, and nothing the runtime does afterwards changes it. **What the core knows is that an opaque profile-owned payload has a declared form, and nothing beyond that.** VM-7 admits three forms — bytecode, `x86-64` machine code and `arm64` machine code — and that enumeration belongs to the profiles that offer them and to the support table that publishes them, never to a core interface: invariant 4 refuses the core an ISA it understands, and an ISA the core understood would be the lowest-common-denominator ISA that invariant exists to refuse. See [invariant 14](#2-engineering-invariants) for what the core does own, which is whether a composition may execute the result. *(added 2026-09-07 for VM-7)* |
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

- its artifact payload format and feature manifest. **A payload need not be bytecode**: VM-7
  admits a native machine-code form, and which forms a profile offers is that profile's choice
  and not the core's *(revised 2026-09-07)*;
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
- delivering the JavaScript or WebAssembly profile. Each is a product project family in this
  component with its own roadmap, ledger and gates, none of which this document plans or tracks,
  and no gate here depends on either existing. **The tree containing a profile and the packages
  shipping one are different claims**: every profile project declares no `PackageId` and carries
  `IsPackable=false`, which is what keeps
  [gate 1](#15-release-gates)'s "no language profile ships with the core" true while the profiles
  sit in `src/`; and
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
   one. **Where the payload is machine code the word *executable* stops being a metaphor, and the
   verifier's obligation grows to match** *(added 2026-09-07 for VM-7)*: a bytecode verifier that
   is wrong yields a wrong answer inside an interpreter that still owns the process, and a native
   verifier that is wrong yields whatever the bytes say. Nothing about the shape of this invariant
   changes; what changes is the cost of getting it wrong, and section 16 carries that as its own
   risk row rather than leaving it to be inferred from this sentence.
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
14. **Native execution is declared, never ambient.** A composition may map artifact bytes
   executable only where its register row declares it; the declaration is to be checked against the
   published closure the way every other closure claim already is — **the column landed on
   2026-09-07 and rule K5 has read it since**, holding every cell against the tree in both
   directions: a `none` over an image that links the arming path and an architecture over an image
   containing none both fail, and the assemblies that can map memory are read from the ImplMap
   tables and the checkout's source rather than from the register. Six of the register's ten rows
   declare `x86-64` and four declare `none`. So this invariant is enforced rather than declared;
   what remains outstanding for
   [VM-7](#vm-7--admit-a-native-artifact-form-and-in-process-native-execution) is reading the
   declaration against the retained closure reports the way rule K4 reads every other closure claim
   *(corrected 2026-09-08: this clause read "**the column exists as of 2026-09-07 with every row
   declaring `none`, and no rule reads it yet**, so this invariant's enforcement is VM-7's first
   next action and not a check this checkout runs", which went stale the day it was written — K5
   was minted, and the JavaScript and PolyglotCli rows moved to `x86-64`. **An invariant that
   reports itself unenforced when a rule enforces it understates the component in the direction
   this document treats as a defect**, so the superseded reading is quoted rather than dropped)*.
   Pages are armed **W^X** — written readable-writable, executed read-execute, never both at once —
   and a profile that maps nothing executable must be unable to, rather than merely not doing it.
   **The core generates no machine code and knows no encoding.** Invariant 4 is what forbids it: an
   ISA the core understood would be the lowest-common-denominator ISA that invariant exists to
   refuse, so instruction selection, register allocation and calling convention stay inside the
   profile that owns them, and the core owns only whether a composition may execute the result
   *(added 2026-09-07 for VM-7)*.

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

**The procedure is written and currently unexecutable, and that is this component's to say rather
than a profile's to discover.** No amendment has been minted, and the minting role and both
co-signing roles are held by one person, so no co-signature would be independent — the same
non-independence EX-30 records for review, applied to the one procedure a profile cannot route
around. Its consequence reaches outside this component: a profile that cannot express a capability
under version 1 waits on a mint nobody here can perform, which is a blocker this component holds
and section 16 records beside contract acceptance rather than leaving to be read as that profile's
own scheduling. **What VM-6 can do is publish the state**: the candidate register, per row,
with what each row would change, whether it is filed, held or opened, and the deterministic failure
or named exclusion the unamended contract leaves standing. A candidate a release names is not a
candidate a release has approved.

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
| Profile | `Broiler.VM.Profile.<Language>` *(product projects in this component, non-packable; ADR 0001's 2026-08-31 revision)* | One language: format, verifier, value/frame model, executor, imports, conformance. References the core; never references another profile. **One product assembly is the default**; section 10 splits the format out only where Broiler compiles the language itself, and a profile that consumes a format an external toolchain produces has nothing for a format assembly to hold apart. The frozen "exactly two core assemblies" reference set is a set of **Broiler.VM-owned** assemblies: a profile component's own siblings are not members of it, which ADR 0011 P1 and ADR 0001 now state rather than leave to be inferred from a rationale that is entirely about excluding the runtime. |

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

**The payload's declared form is part of that identity, and the core reads it the way it reads
every other profile-owned field** *(added 2026-09-07 for VM-7)*. Section 1's *artifact output form*
is carried on the descriptor and pinned into the handle beside the profile, the manifest and the
effective ceilings, so a composition that holds no executor for a form refuses it before any of the
payload is decoded, and so the refusal is the ordinary deterministic load failure this section
already requires of a mislabelled artifact rather than a new outcome category. **The form is fixed
when the artifact is verified and a verified handle's form never changes**, which is the same rule
invariant 9 applies to a ceiling: a different form needs a newly verified handle, and no run-time
observation selects one.

**What the core carries is a declaration and never a decoder**, and the distinction is invariant
4's rather than a convenience. The core does not know what `x86-64` means, holds no encoding table,
and cannot tell a well-formed instruction stream from a malformed one; what it can do is refuse a
form no composition in the image declares, and record which form a handle was minted against. The
verifier that reads the bytes is the profile's, and [invariant 14](#2-engineering-invariants) is
where the executable case's extra obligations are stated. **No milestone before VM-7 admits a form
other than bytecode**, and this passage describes the shape of a concept rather than a payload any
composition in this repository produces.

### Optional persisted envelope

**No milestone in this roadmap approves persistence, and the approval is not one this component can
take on its own.** The contract admits a bounded outer envelope and VM-0 through VM-6 contain no
persistence gate, so every clause below is a design kept reachable at no cost rather than work
anybody scheduled — and the trigger that would reopen it is a host's latency budget missed by a
stated margin, which is a measurement no host has produced because no host exists. Both intended
profiles plan against that reading and open the question against their own cold-start and
verification-throughput figures rather than against this section.

Two consequences follow and are stated here so a reader does not meet them as unmet gates.
[Section 14](#14-test-and-evidence-matrix)'s persistence row and
[gate 7](#15-release-gates)'s envelope-recovery clause are **conditional on approval**: until a
dated record approves persistence they have no subject, and a release does not fail for not
evidencing a feature the plan deliberately does not build. And **VM-6 publishes the position** —
admitted by contract, provided by no composition, with its deterministic behaviour and its
reopening trigger named in the support table — because a capability the contract admits and the
product does not provide is exactly what [gate 1](#15-release-gates) exists to keep from being
inferred. *Recorded 2026-09-01: the "where approved" wording below and in VM-2 had no referent, and
an evidence row with no subject reads as an unmet gate rather than an unopened one.*

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

### Which ceiling terms are catalog-wide, and which are not

Effective ceilings are resolved partly against the profile an artifact selects and partly against
the whole catalog, and the split is not arbitrary — it was ruled on 2026-08-31 after the record and
the implementation were found to disagree:

1. **A profile hard maximum constrains that profile's own artifacts and nobody else's.** It is
   applied at verification, against the profile the artifact names. The implementation used to
   clamp every runtime ceiling to the tightest maximum in the catalog, one step earlier; that was a
   defect and is corrected. A profile declaring a tight maximum no longer caps its neighbours, and
   the obligation to publish an unconstrained maximum on a dimension it declares inapplicable falls
   away with it.
2. **Adopting a profile default does resolve to the tightest default in the catalog**, per
   dimension, and that one is deliberate. At runtime creation no profile has been selected, so an
   adopted default has no owner to take a number from and the conservative answer is the only safe
   one. It costs nothing, because verification re-intersects with the selected profile's own maxima
   afterwards; a host that wants more states an explicit ceiling.

**The asymmetry is the rule**: a maximum has a correct owner one step later, so runtime creation
must not guess at one; a default has no owner at all, so it must. What keeps a dimension unreachable
is a profile's import list and its budget declaration matrix, never a zero ceiling.

The ruling cost more to reach than it should have, and the reason is worth keeping. The clamp was
enforced by code, described in two profiles' comments, a composition register and an evidence bundle
— and asserted by no test, while the record that owns effective-ceiling materialization said
something different. A behaviour with four prose witnesses and no executable one is a behaviour
nobody has to reconcile with the contract.

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
- Where a host creates several runtimes under one shared aggregate budget, **eleven of the fifteen
  dimensions** are metered against the parent as well as each runtime: the seven allowances plus
  `LiveBytes`, `CallDepth`, `NestedLoadDepth` and `LiveRuntimes`, per
  `VmBudgetDimensions.CarriesAggregateScope`. The four artifact-shaped ceilings — `ArtifactBytes`,
  `SectionCount`, `DeclaredCount`, `StructuralDepth` — do not, because summing a per-artifact
  ceiling across concurrent runtimes measures nothing. **`CallDepth` is in the first group, and
  that is the fact a cross-profile call chain depends on**: it is what bounds a chain that
  re-enters a second runtime through a host object.
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
| Bounded binary reading: checked arithmetic, variable-length integers, framing, allocation guards | **Core-owned from day one** (`Broiler.VM.Binary`), and pre-approved without the gate by ADR 0011. The core's own envelope needs it, so it has a consumer before any profile exists. **The original justification also said "every profile verifier needs it", and that half is now known to be false**: the intended WebAssembly profile declines `TryReadVarUInt32`, `TryReadVarUInt64` and `TryReadDeclaredCount`, because this package's integers are canonical-only and that format requires padded encodings to be accepted. The obligation to route a dimension through this package is therefore conditional on the members a profile actually calls, not absolute. |
| Descriptor matching, verified handles, limit intersection, envelope, lifecycle, budgets, diagnostics identity, host registry | **Already core-owned.** No new component; the rule is simply that a profile never re-implements them. |
| A verification framework: worklist and fixpoint over a control-flow graph, parameterized by the profile's abstract domain | **Extract later, do not predict.** Two verifiers will share the shape and share no domain. Open it when the second verifier exists and the duplication is measured. |
| Lexing and diagnostic *formatting* | **Only when a second text front end exists.** Until then it is one profile's private code wearing a shared name. |
| Positioned diagnostics and a stable code registry: a versioned registry bound in both directions, each code mapping to exactly one core reason, each emission carrying the core's position record | **Named mechanism, extract later, trigger re-cut.** The original trigger — a second text front end — can never fire: a binary-format profile emits positioned refusals and will never have a front end. The trigger is instead **the second profile that emits positioned refusals**, front end or not. Until then both profiles use the core's position record, state which of its fields they populate, and neither invents a parallel one. |
| The conformance-harness method: a pinned corpus revision, content-independent sharding, a recorded selection pipeline, a self-check with a passing control before every shard, a closed set of configuration failures, a failure manifest that is a queue, the harness's own regression suite, totals partitioned along a profile-named axis with each partition reporting the same six counters, and a ratchet bound to the pinned suite revision | **Named mechanism, extract later, and test-only either way.** Two profiles have specified it in near-identical words against two unrelated corpora, which is design-level evidence and not merged code. The trigger is the second profile's harness merging. Note that a shared harness is never referenced by a product package, so its extraction can never enter a product closure — which removes the usual objection but not the gate. |
| Assurance annotation, fingerprinting, review-state generation, and the evidence-bundle contract and collection script | **Repository policy, not a core component.** The core has one implementation and both profile roadmaps specify a byte-identical floor. The correct first move is a repository-level policy document that the components cite, not a shared assembly; the trigger for a shared tool is a second component's implementation existing to compare against. |
| The projection between the contract meter and the bounded-reading meter, and between a limit vector and the four artifact-shaped read bounds | **Documented duplication, kept.** It is four method bodies and one projection, it names no language concept, and it is already written once per profile in this repository — but the canonical copy lives in a test-only assembly no product profile may reference, so every profile writes it again. The core's obligation is to publish the canonical form in the profile-facing contract so the copies agree, not to add a package for it. |
| The retained malformed-corpus method: self-authored bytes with a hash and a pinned observed answer per entry, replay across three publish modes with byte-identical failure-class tables, mutated-entry detection, and control entries that verify successfully | **Named mechanism, extract later, and test-only either way.** Distinct from the conformance-harness row: that one ratchets a third-party suite, this one pins the core's own answers to bytes nobody else authored. The core already implements it — `CorpusRunner`, `MalformedCorpusTests`, and eighty-seven artifacts under `src/tests/corpus/vm-2` — and both intended profiles re-specify it in near-verbatim words. The trigger is the second profile's corpus merging. **The corpus-entry schema is published in ADR 0011 now**, so the copies agree on the record even while each writes its own runner. |
| The read-order recorder: a monotonic three-stamp sequence — policy handed over, first payload byte consumed, first allocation reserved — and the two ordering predicates over it | **Documented duplication, kept, with a published canonical form.** It is what makes the ordering clause both profiles write into their gates provable rather than asserted, it names no language concept, and it exists once already as `FixtureReadOrderRecorder`. But it lives in a test-only assembly no product profile may reference, so every profile writes it again. The core's obligation is to keep the canonical form discoverable, not to add a package. |
| The mapping from a bounded-read status onto an outcome category, a reason, and where applicable a budget dimension and scope | **Documented duplication, kept — and structurally unhomeable, which is worth stating rather than discovering.** `VmBoundedReadStatus` lives in `Broiler.VM.Binary` and the outcome vocabulary lives in `Broiler.VM.Abstractions`; both are dependency sinks with no reference between them, deliberately, so no core product assembly can host the mapping. It is already hand-written three times in this tree and has diverged once. **The canonical table is published in ADR 0011 now**; every profile still writes the method body, but they write the same one. |
| The two-profile hostile-neighbour catalog test: composing a profile beside a deliberately adverse neighbour and proving the neighbour's maxima do not reach it while its adopted defaults do | **A test shape, not a component.** Both intended profiles build it at their first milestone. It is recorded in the composition register as the way to prove a catalog-wide default has not been mis-declared, and no assembly is shared to do it. |
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

**And because no milestone here can satisfy the first condition, VM-6 publishes the register's
state.** Per candidate above: which condition failed, what would reopen it, and where the dated
record is. That is the one obligation this component can discharge whatever the profiles do —
section 16's stop condition is an *unrecorded* state, and a table of refusals nobody published is
unrecorded in every sense that matters. **The gate is over the publication and never over the
verdict**: a release that names every candidate as failing the first condition passes it, which is
the honest shape while two product profiles do not yet exist.

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
language, and so that a profile roadmap can be written without renegotiating the core.

**Both plans now exist, and neither is this document's.** They live at
`src/Broiler.VM.Profile.<Language>/docs/`, each with its own roadmap, ledger, decision series and
release gates, and the placement is ADR 0001's 2026-08-31 revision rather than a staging
convenience. Two consequences are this section's to state. What a profile plan says it *expects* of
the core belongs below and is this roadmap's design input; what it says about its own milestones,
evidence or state belongs to its own ledger, which
[update rule 6](roadmap.status.md#4-update-rules) keeps out of this component's. And the lists
below are kept current against those plans rather than against the expectations this section was
first written from — a requirement a profile has since dropped or sharpened is a requirement
the contract was designed against for no reason.

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

**This profile is planned to be brought up, and to be brought up as an MVP** *(added
2026-09-07)*. The owner's instruction of that date names a second input profile beside JavaScript.
**It has started, and nothing it has done is accepted** *(corrected 2026-09-08: this read "**None
of it has started**: that profile's ledger marks every milestone `Not started` and its directory
holds documentation and no code", which was true the day it was written and went stale within it)*:
that directory holds a project in the solution and source beside its documentation, and that
profile's ledger — not this section — is the authority for what the bring-up has demonstrated. It
marks WA-0, WA-1, WA-3 and WA-5 `In progress` and WA-2 `Blocked` on the strength of code that
exists and runs, with no milestone accepted, no evidence bundle retained and no human review of
anything. The work of building one belongs to
`src/Broiler.VM.Profile.WebAssembly/docs/` — that roadmap mints the stages, that ledger holds their
state, and [update rule 6](roadmap.status.md#4-update-rules) keeps every one of them out of this
component's ledger. **Nothing in this paragraph moves a core gate.** No row of section 15 gains a
subject because a second profile is planned, VM-0 through VM-7 wait on it exactly as much as they
waited before — which is not at all — and no WebAssembly result closes anything here.

**What a second product profile does change is which of this document's arguments have two
parties.** [Section 8](#8-sharing-between-profiles-without-a-lowest-common-denominator-core)'s
extraction gate asks first for two profiles that need the same mechanism, and through VM-6 that
condition is unsatisfiable rather than merely unmet: there has been one product profile and a
fixture. Section 14's two-profile catalog row is in the same position, and
[section 16](#16-risks-and-stop-conditions) already says so in its own words — a composition that
hosts two profiles does not close a gate until that test exists, and until now there has been no
second product profile to write it against. **A second one turns both from a design argument into
a thing with two real parties**, which is the one respect in which this component's own gates stand
to get better evidence out of work this document does not plan. **It does not make either pass
earlier.** A first condition that becomes satisfiable is not a gate that has been invoked, and a
catalog test with two product profiles in it is still a test somebody must write, run and retain;
both belong to the milestones that already own them, and neither is advanced by this paragraph.

Expected to require, beyond the baseline in section 4: binary decoding and type/stack validation
of structured control flow; an immutable verified module distinct from each mutable instance;
typed imports and exports resolved through an explicit linker; traps as profile results rather
than process failures; imported memories, tables, and globals with declared aliasing and lifetime
rules across instances; and a pinned specification and conformance-suite revision that its own
manifest fixes.

It also expects one thing version 1 cannot express, and it is the one candidate amendment with no
local workaround behind it: **a refusable retention member on the metering surface.** Its
specification requires `memory.grow` to answer the guest with a negative result when growth is
refused, VM-0's item 7 records that no retained-state dimension can carry a guest-observable
refusal, and gating on a charge does not recover one. Recorded here because
[section 2](#core-contract-version-and-amendment)'s procedure is currently unexecutable, so this is
the sharpest live example of a blocker this component holds rather than a profile's own delay.

**The MVP does not wait on that mint, and what it does instead is a published deviation rather
than a workaround nobody wrote down** *(added 2026-09-07)*. Growth is gated on the profile's **own**
declared memory maximum, which is not a core budget refusal at all, so the guest-observable
negative answer the specification requires exists, is exercised, and is correct for every refusal
the profile itself decides. **A refusal caused by a core budget stays non-guest-observable**,
because on the shipped contract there is no spelling of one — the amendment above is exactly the
absence of that spelling — and the deviation is named as a deviation in that profile's own support
surface rather than left for a reader to infer from a test that passes. **This is a route taken
without the decision that would have chosen it.** Choosing between deviating now and waiting for a
mint is a boundary approval [the MVP programme](mvp.md) defers, and a deferred decision is a
decision nobody took rather than one that went a particular way, so the route is recorded here —
where its consequence is — and named as taken-without-a-decision. Taking it answers nothing: the
candidate amendment stays **opened and unfiled** and unanswered, this component stays its holder,
and VM-6 still publishes the row's state.

It is also expected to need **no** parser, **no** text format, and **no** guest-initiated loads in
its first version, which makes it the useful counterweight when judging whether a proposed core
feature is genuinely general or is one language's need in disguise — and the row above is that
counterweight working: a profile with none of those three still meets the wall.

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
| **Lowering** — source to an artifact | yes, and only where Broiler compiles the language itself | `Broiler.VM.Profile.<Language>.Compiler` |
| **Output form** — which kind of artifact a lowering emits | yes | the same compiler. A backend is a **choice inside one lowering**, not a second lowering *(added 2026-09-07 for VM-7)* |

**A second output form is not a second compiler, and the distinction is the one section 10 already
draws for hosts.** "One lowering, however many hosts" below says a run-time-compiling and an
ahead-of-time-compiling composition share one lowering assembly. The same rule answers backends:
a bytecode backend and an x86 backend are two exits from one front end, sharing the parse, the
static semantics and the analyses, and a profile that forks its front end per target has written
the second lowering this section forbids. **What a backend does add is a second thing to keep
deterministic** — the determinism property below now ranges over machine code, where iteration
order reaches register allocation and instruction scheduling rather than only constant-pool
ordering.

**Both halves of that are rules rather than observations, and they are written as rules because a
backend arrives as a convenience and a rule is what survives one.** The first is that a backend is
added to the lowering that already exists, and its argument is not that two forks of a front end
would disagree: it is that the language's static semantics would then be decided in two places,
which is the defect whether or not the two agree today and is the defect a reader cannot check by
comparing outputs. The second is that determinism stops being cheap. The property below ranged over
a constant pool's ordering while bytecode was the only output; over machine code it also reaches
instruction selection, and an iteration whose order is a dictionary's rather than a declaration's
is enough to make two builds of one source differ in a byte of an epilogue — **two builds a reader
cannot compare, in a component whose evidence is made of comparisons.** Where that work is tracked
is not here: the JavaScript profile's backends are planned in
`src/Broiler.VM.Profile.JavaScript/docs/roadmap.backends.md` under identifiers of its own, and this
section fixes where a backend *lives* in the assembly graph rather than when anybody writes one.

**That determinism also makes a verification strategy available that a payload of code would
otherwise not have.** Where an image holds the front end and the backend both, a verifier can lower
the artifact's own bytecode again with the same backend and require the emitted bytes to be equal —
**re-emission equality**, which answers "are these the instructions this compiler produces for this
program" rather than only "are these bytes well formed". It is available only where the backend is
present, so an execution-only image cannot perform it and VM-7 does not treat it as the answer:
that milestone's gate pins the *verifier* with a retained corpus and states in its own words that
the code generator is pinned by nothing in it. **Re-emission equality is a consequence of
determinism and not a substitute for a differential oracle** — it proves that an emitter reproduces
itself, and an emitter that is wrong reproduces itself exactly.

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

**That trigger is now pulled, and by VM-7 rather than by a product** *(2026-09-07)*. A native
output form makes the execution-only composition the interesting one: it holds a format, a
verifier and an executor that maps a page, and no compiler and no code generator at all, which is
the "no compiler in its image" shape this paragraph was written against. **This does not by itself
open a toolchain component.** What VM-7 opens is a backend inside the existing lowering and a
mapping path inside the profile; a command-line compiler, a build integration and a packaged SDK
each still have no consumer, and section 8's extraction gate still governs whether the mapping
mechanism — how a page is allocated, armed and revoked — ever leaves the profile that first wrote
it. **It leaves on the second consumer and not on the first**, exactly as `Broiler.VM.Binary` did
for bounded reading; one profile with an executable page is a profile with an executable page, not
a shared primitive.

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

### Three decisions this forced on VM-0, and how ADR 0010 settled them

Each was cheap to settle then and expensive to retrofit, and each is invisible until a host with a
latency budget arrives. **All three are closed. They are kept here with their answers** because both
intended profiles reason from the closed answers, and a reader who met them as open questions would
mis-plan against them.

1. **Whether locally produced bytecode must round-trip through bytes.** Invariant 3 is written for
   bytes that came from outside. When the compiler that produced them is in the same process and
   inside the same trust boundary, serializing and re-decoding on every load is pure critical-path
   cost. **Settled: the round trip is mandatory** — bytes are the only input from which a verified
   artifact may be produced (ADR 0010, decision 1), with the alternative registered as candidate
   amendment 3, the in-process producer input form. VM-5 measures verification throughput so a
   later reopening rests on numbers.
2. **Whether verification may be lazy per section.** Hosts that compile function bodies on first
   call do not want to verify an entire bundle to run one entry point. **Settled: verification is
   whole-artifact and eager** — a handle means the whole artifact was verified (ADR 0010, decision
   2), with the alternative registered as candidate amendment 4.
3. **Whether an artifact may be verified incrementally as it arrives.** The contract is whole-bytes
   to handle. **Settled: excluded from version 1** — the input is whole and complete when
   verification is called (ADR 0010, decision 3), with the alternative registered as candidate
   amendment 2, streaming verification. ADR 0010 declined to foreclose it permanently, on the
   ground that section 9's WebAssembly profile lives in an ecosystem where streaming compilation is
   standard practice.

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
  2. **The catalog-wide ceiling term** — the tightest adopted default — written into the ordered
     precedence algorithm that owns effective-ceiling materialization, with the profile obligation
     it carries stated: a stingy default on a dimension you never use is what reaches a neighbour,
     and no default may be unconstrained.

     *Narrowed 2026-08-31, and the narrowing is the point rather than a tidy-up.* This item used
     to name **two** catalog-wide terms — the tightest hard maximum as well as the tightest
     adopted default — and to require the "publish an unconstrained maximum on a dimension you
     declare inapplicable" rule as a profile obligation. One term and the whole obligation were
     retracted: the hard-maximum clamp was an implementation defect rather than a property of the
     contract, ADR 0007 puts `ProfileMax` at P2 against the profile an artifact names, and
     removing it takes the obligation with it. The section
     [Which ceiling terms are catalog-wide, and which are not](#which-ceiling-terms-are-catalog-wide-and-which-are-not)
     is the ruling. An instruction to write down a property the contract does not have is worse
     than a missing instruction, which is why this reads as a correction rather than a deletion.
  3. **Host-exception translation precedence**, written down rather than left to be re-derived
     from the implementation.
  4. ~~**Whether a profile's own sibling assembly counts inside the frozen two-core-assembly
     reference set**, and the reconciliation of section 5 with section 10 on where a format
     lives. No profile can take its placement decision until this is answered.~~

     **Answered 2026-08-31: a profile's own siblings sit outside the set.** ADR 0011's obligation
     P1 carries an editorial revision stating that the set it bounds is of *Broiler.VM-owned*
     assemblies and that a profile component's own siblings — its format assembly, its lowering,
     its composition roots — are not members of it; ADR 0001 carries the same qualifier, with the
     reason that section 10's format pivot is incoherent unless a profile may reference its own
     format assembly. One rule had to move with it: **A11** forbade any reference to a
     `Broiler.VM.Profile.*` assembly from outside a composition root, and now exempts a sibling in
     the same profile family, keyed on the language segment so that an edge between two profile
     families is still a violation. The first profile has since taken its placement decision on
     that answer, at ADR 0001 revision 5.
  5. **Who invokes the extraction gate, and where the record is filed** — including the case where
     the first condition is unsatisfied, and the fact that neither profile may cite the other, so
     the record can only live here.
  6. **The candidate-amendment register brought level with what the profiles have asked for**,
     including the charging hook for work done inside a host capability, which is the one candidate
     both intended profiles independently rate general, and the argument channel, whose scope and
     strength the two profiles currently record differently.
  7. **Whether a retained-state dimension can carry a guest-observable refusal.** It cannot: the
     retention report returns nothing and the refusal is latched for the next charge or poll.
     Recording it costs a sentence; discovering it costs a memory representation.

     *Corrected 2026-09-01, and the correction removes an escape rather than adding one.* This item
     used to close "and a profile whose language requires an observable refusal must gate on a
     charge instead", which read as a profile-side workaround and is not one. Read off the shipped
     core rather than inferred from the contract: a refused `TryCharge` at any scope latches
     exhaustion on the meter, and the core rewrites the completed step as `ResourceExhaustion`
     whatever the profile does with the `false` it was handed. **There is no guest-observable budget
     refusal on core contract version 1, in any spelling.** So this is an amendment question and not
     a profile's design problem, which is the difference between a sentence in a profile's plan and
     a milestone it cannot start — an intended profile has met exactly that wall, and
     [section 2](#core-contract-version-and-amendment) records what an unexecutable procedure then
     costs.
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
- **Next action:** Implement descriptor and profile matching, bounded outer-envelope parsing **only
  where a dated record approves persistence, and none does** —
  [section 6](#optional-persisted-envelope) is why this clause has no subject rather than an unmet
  one — the opaque immutable verified-artifact handle, trusted host/profile/artifact limit
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

  **Then publish four states this component cannot resolve, each a state rather than a decision.**
  They are published here because no earlier milestone can answer them and this is the last one
  that gates a release. *(Revised 2026-09-07: this passage read "what follows it is the review, not
  a VM-7". A VM-7 now follows. **The conclusion is unchanged and its reason is not** — VM-7
  resolves none of the four: it mints no amendment, needs no second product profile, provides no
  persisted envelope, and closes neither declared-and-undemonstrated bound. So the four are still
  published at VM-6 rather than deferred into it, and a reader who met the old sentence and
  inferred that the register was complete at VM-6 should read gate 1 again.)*

  1. **The candidate-amendment register**, per row: what it would change, whether it is filed, held
     or opened, the deterministic failure or named exclusion the unamended contract leaves
     standing, and that the procedure is unexecutable and why
     ([section 2](#core-contract-version-and-amendment)). A row a profile is waiting on is a blocker
     this component holds, and the table is where a reader outside it can see that.
  2. **The extraction-gate register**, per candidate of
     [section 8](#8-sharing-between-profiles-without-a-lowest-common-denominator-core): the
     condition that failed, the condition that would reopen it, and the dated record. The first
     condition needs two product profiles and is unsatisfiable through this milestone, so what is
     published is a table of refusals rather than a verdict.
  3. **The persistence position** ([section 6](#optional-persisted-envelope)): admitted by
     contract, provided by no composition, with its deterministic behaviour and the measurement
     that would reopen it.
  4. **Two bounds that are declared and not demonstrated**: the depth half of the cross-runtime
     `CallDepth` bound, which closes only against a profile with calls, and the reconciliation of
     two profiles' declared defaults, which belongs to a component that does not exist. Both are
     named in the support table as unowned or unproven rather than left to be inferred from the
     dimensions that *are* metered.

  None of the four is answered by naming it. Each is published because a reader of the support
  table meets it there or meets it in a refusal.
- **Dependencies:** VM-0 through VM-4. VM-5 is required only where a product threshold says the
  measured core overhead is unshippable.
- **Objective exit gate:** Every advertised package restores from a feed without repository
  project references; the public API and package graph match the baseline; all malformed-input and
  contract suites pass; every claimed RID publishes and runs the declared compositions with
  warnings as errors; notices and reviews are complete; rollback is tested; and recertification
  triggers are documented. The support table states the core contract version and states that the
  core ships no language profile. **The four states above are each published, and each is asserted
  rather than asserted about**: a rule fails when a candidate-amendment row or a section 8
  extraction candidate has no corresponding published state; the persistence position names its
  deterministic behaviour and its reopening measurement; and the support table names the depth half
  of the cross-runtime bound as not demonstrated and the two-profile default reconciliation as
  unowned. **Each rule is over the publication and never over the answer** — a release that
  publishes every row truthfully and moves none of them passes.

### VM-7 — Admit a native artifact form and in-process native execution

- **Owner:** Core architecture owner with the security and release owners. **Three owners rather
  than one**, because this milestone spends the component's safety margin rather than adding a
  capability beside it, and an owner who can approve that alone is an owner reviewing themselves.
- **Why it is here at all, stated plainly.** Every milestone before this one narrowed what a
  product image may do. This one widens it, and the widening is the point rather than a side
  effect: a verified artifact may be machine code, and a composition may execute it. **The
  component's own records forbade this until today** — section 16 stopped a closure that reaches
  dynamic code, and the JavaScript profile's non-goals refused a second execution arm. Both are
  amended in the same change that opens this milestone rather than found to be in the way later,
  and neither amendment is a finding that the old rule was wrong. The old rule bought something
  real, this milestone spends it, and the exit gate below is the price.
- **What motivated it, and what that evidence is not.** A probe collected 2026-09-07 on one
  `win-x64` machine: emitted x86 executed correctly from a **published Native AOT image** on
  `win-x64` and on `win-x86` — `IsDynamicCodeSupported` is `False` in both and neither needs it —
  and was faster than the interpreted path on that machine. **No figure from it is stated here**
  *(revised 2026-09-07: this bullet printed per-call timings for the emitted path beside a
  per-instruction timing for a language profile's interpreter, and the pairing implied a language
  speedup; the figures are deleted and the qualitative fact kept)*. **This is a probe
  and not a baseline.** It was not collected by the benchmark host, it has no A/A lane, no
  predeclared rule and no retained repetitions, so rule L1 does not bind it and
  [the baseline register](baselines.md) does not carry it. It is recorded because a milestone that
  widens a trust boundary should say what it was opened against — and because that is not what
  closes it. **It is also not retained, and this record says so rather than letting a reader assume
  a bundle**: the probe was two throwaway programs, `docs/evidence/` holds no `vm-7` directory
  because there is no bundle to put in one, and adding the programs to the tree would put a project
  in neither the solution nor `samples/`, which rule A14 reports. So the probe is
  **unreproducible from this repository as it stands**, and a figure about a language profile's
  interpreter may not appear in a core record at all — [release gate 8](#15-release-gates) and
  [update rule 6](roadmap.status.md#4-update-rules). A reader who needs numbers re-derives them; a
  reader who needs to rely on them cannot, and that is the correct relationship between this
  paragraph and a gate.
- **One accident from that probe is worth more than its timings, and it is retained as a fixture
  rather than as an anecdote.** The 32-bit backend was written with the wrong calling convention:
  `ret` where the target ABI requires `ret 8`. Nothing refused it. The function returned the
  correct answer, every time, and leaked eight bytes of stack per call until the process died of
  stack exhaustion several million calls later — no exception, no diagnostic, and nothing a
  verifier of the *artifact* could have caught, because the artifact was well formed and the
  generator was wrong. **That is the shape of a native-form defect**: correct output, delayed
  death, and a failure that arrives nowhere near its cause. Every clause of the exit gate below
  that looks over-specified is answering it.
- **Next action:**
  1. **Make the enforcement say what the stop condition says.** Rule B5 forbids
     `System.Reflection.Emit.*`, `System.Runtime.Loader.*` and the reflection-invocation members;
     release gate 2 forbids "dynamic loading or IL emit". **Neither reached a hand-written
     machine-code path. Through VM-6 a profile that mapped a page executable passed every automated
     gate this component had, while tripping a stop condition this component publishes** — a hole
     that predates this milestone and was not created by it. **Done: it landed on 2026-09-07 and is
     recorded as done here on 2026-09-08, and this item is the one act a later document should name
     rather than number.** B5 gained the native-memory member half — the managed members by which a
     caller acquires, prepares or hands out a pointer to code — and its scope widened from the
     three core assemblies to every assembly a published image can contain, which is where an
     arming path would actually be; **B5c** was minted for the half no member reference names,
     reading the ImplMap and ModuleRef tables where `VirtualProtect` and `mprotect` are rows in the
     calling assembly's own metadata, with one named arming assembly allowlisted; **X1** pins the
     arming path to one place in the shipping source and every protection it passes to a named
     constant admitting a read and a write or a read and an execute and never both; and **K5**
     reads the composition register's native-execution column against those same ImplMap tables and
     the checkout's source, in both directions — a `none` over an image that links the arming path,
     an architecture over an image that can arm nothing, `none` beside an architecture, a value
     outside the vocabulary, and a row carrying no cell at all. The register now has **ten rows,
     six declaring `x86-64` and four declaring `none`**. Each rule carries the negative control
     item 4 below names, watched failing when injected and passing after revert. **What none of it
     accepts is the milestone**: no exit-gate clause is met by it, no bundle of this milestone's
     own is retained, and no person has read a line of it. *(Corrected 2026-09-08: this item read
     "**Neither reaches a hand-written machine-code path. Today a profile that maps a page
     executable passes every automated gate this component has, while tripping a stop condition
     this component publishes**", and then described its own enforcement in the future tense — "B5
     gains the native-memory surface ... it mints the rule that reads the composition register's
     native-execution column — that column exists as of 2026-09-07 and every one of its seven rows
     declares `none`, and **no rule reads it**, so the declaration exists and the enforcement does
     not". Every one of those clauses was true when it was written and none of them survived the
     day it was written on: B5, B5c, X1 and K5 are `Active` in
     `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, the register carries ten rows
     and not seven, and six of them declare an architecture —
     [the register](compositions.md#3-the-compositions) and
     [the support table](support.md) say the same. **A roadmap that reports its own enforcement as unwritten after it is written
     understates the component, which is the same defect as overstating it**, so the superseded
     reading is quoted here rather than deleted.)* *(Revised 2026-09-07: this item was numbered 2
     and the amendment falsification below was numbered 1. **The first thing a milestone that
     widens a boundary owes is the check that the boundary was ever enforced**, and this work
     depends on no backend existing and on no procedure this component cannot execute, while the
     item below depends on both. A document that referred to either of these by ordinal should
     refer to the act instead, so the reference cannot go stale on a renumber.)*
  2. **Attempt to falsify the claim that no core contract amendment is required.** It decides
     whether the milestone can *run*: section 2's amendment procedure is **unexecutable** while one
     person holds the minting role and both co-signing roles, so a VM-7 that needs an amendment is
     a VM-7 that cannot start — which is why it stands ahead of every item below that writes a
     backend, and behind the enforcement item above, which asks it nothing. The claim to attack is
     that contract version 1 already admits this — a verified artifact's payload is opaque and
     profile-owned, the executor is profile-owned, the lifecycle is untouched, and no result
     category is added — so a native payload is just a payload. **The claim is stated in order to
     be refuted, and what this item delivers is the attempt rather than the conclusion.** If it
     survives, record why in a dated ADR revision; if it falls, VM-7 is `Blocked` on a procedure
     this component cannot execute, and the ledger records that with this component named as the
     holder.
  3. **Define the declaration and prove the closure.** The composition register's native-execution
     column landed on 2026-09-07 and rule K5 has read it in both directions since the same date, so
     both the declaration and its enforcement exist: six of the register's ten rows declare
     `x86-64` and four declare `none` *(revised 2026-09-08: this item read that the column "landed
     on 2026-09-07 with every row declaring `none`, so what is left here is the rule above reading
     it", carrying a 2026-09-07 parenthetical that "only the enforcement is outstanding". The
     enforcement is rule K5 in `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`,
     witnessed by `CompositionRegisterTests` with five rejecting directions, and
     [the register](compositions.md#3-the-compositions) no longer declares `none` on every row.
     **What is still outstanding here is the closure half and not the declaration half**, which is
     the distinction the previous revision collapsed)*; the published closure is read off the published
     output as it already is; and an execution-only composition demonstrates the shape this
     milestone exists for — a format, a verifier and an executor that maps a page, and **no
     compiler and no code generator in the image at all**.
  4. **Fix W^X as a property of the mechanism rather than a habit of its callers.** Pages are
     written RW and executed RX and never both at once; the arming path is one place; and a
     negative control asserts that a page mapped RWX fails the suite. The probe found RWX permitted
     on the collection machine, which is a statement about that machine's policy and not a licence.
  5. **Publish the RID consequence before collecting it.** Invariant 7 wants publish-and-run per
     declared RID, and a native form multiplies that by one backend per architecture. This
     milestone opens **two backends and admits them on different evidence**: `x86-64`, whose
     emission and whose execution are both in scope, and `arm64`, whose emission is in scope and
     whose execution is not. `x86-32` is not one of them, and `ios-arm64` forbids the mechanism
     outright rather than merely lacking a runner. The support table says which backends exist, on
     which RIDs each has published **and run**, which are **emitting-only** and have therefore run
     nowhere, and — for every declared RID with no backend — that a native artifact is refused
     deterministically there rather than absent. *(Revised 2026-09-07: this item read "`x86-64` and
     `x86-32` are two, `arm64` is a third this milestone does **not** open". The owner's
     instruction of the same date names `arm64` as an output form this component is to have, and a
     plan that declined it here while the work was planned elsewhere would have put the plan and
     the instruction in two documents that disagree. **What changed is which architectures are in
     scope and not what evidence any of them owes** — `arm64` enters with its execution excluded by
     the rule below rather than admitted quietly, and `x86-32` leaves for a reason argued below
     rather than for a reason of priority.)*
- **What this milestone does not do. Each is a rule, not a scheduling note:**
  - **The core generates no machine code and learns no encoding.** Invariant 4 is what forbids it:
    an ISA the core understood would be the lowest-common-denominator ISA that invariant exists to
    refuse. Instruction selection, register allocation and calling convention belong to the profile
    that owns them; the core owns only whether a composition may execute the result.
  - **No tiering.** An artifact is bytecode or it is native, chosen when it is compiled and fixed
    when it is verified. There is no promotion from one to the other while running, no on-stack
    replacement, no deoptimization, and no profile-guided recompilation, because there is no second
    tier for any of them to reach. **This is what makes a native output form a different thing from
    the JIT the JavaScript profile's non-goals refused**, and the distinction is load-bearing
    rather than rhetorical: a tier is a code generator in the running image; a form is not.
  - **No code generator in an execution-only image.** Where a composition compiles, it declares a
    compiler; where it only executes, it may hold no backend at all.
  - **No claim about speed.** Section 1's non-goal stands: this component measures its own overhead
    and never a language's. A native form is admitted here as a **capability**. Any figure about
    what it buys belongs to the profile that emits it, measured against that profile's own
    baseline, and is not evidence for this milestone.
  - **`arm64` emits and does not execute, and the asymmetry is a rule rather than a schedule**
    *(added 2026-09-07)*. A backend that emits and has never executed is named in the support table
    as **emitting-only**; its encodings are pinned by golden-byte tests and by a corpus every entry
    of which is read back through a disassembler and compared against the instruction the encoder's
    own table names, rather than by a run; and **no figure and no capability claim attaches to
    it**. **This is weaker evidence than `x86-64`'s, and the difference is not cosmetic**: a golden
    byte is a claim about what an encoder wrote, and a run is a claim about what a processor did
    with it, and no quantity of the first becomes the second. It is nevertheless worth having, for
    two reasons this milestone states rather than assumes. **An encoder pinned by its own encodings
    is falsifiable** — a byte that changes fails a test, and a stream that does not read back as
    the instruction the table names fails another — which is strictly more than an unwritten
    backend offers and is the only kind of evidence available to a component with no machine to run
    on. **And an architecture added after the abstraction has hardened is an architecture added
    against a fixed target**: the second backend is what finds the places where the first one's
    shape was `x86-64`'s habits rather than the interface, and finding them while nothing depends
    on the answer costs less than finding them once something does. **The reason the execution half
    is excluded is the next rule and not a shortage of hardware.**
  - **On `arm64` a written page is not coherent with the instruction stream until maintenance runs,
    and this component has no way to run it** *(added 2026-09-07)*. The architecture requires a
    data-cache clean to the point of unification, a barrier, an instruction-cache invalidation, a
    second barrier and an instruction-synchronisation barrier between the write and the first
    execution. **There is no managed expression for that sequence and no dependable library export
    that performs it** — the usual C spelling is a compiler builtin rather than an exported symbol
    — so a backend that armed a page without it would be relying on whatever the machine that
    tested it happened to do about coherency. **That is the worst failure shape this milestone
    knows**: it works where it was written, and it fails elsewhere, on another core, under another
    scheduler, or on the same machine on a different day, with no diagnostic and nothing in the
    artifact to blame. So `arm64` execution is **excluded rather than unscheduled**, and what would
    close the exclusion is a maintenance path this component can name, call and test — not a runner
    and not a lane.
  - **`x86-32` leaves this milestone, and the reason is the accident above rather than a priority**
    *(recorded 2026-09-07; next action 5 named it beside `x86-64` until today)*. It is the only
    **callee-pops** ABI in the declared matrix, and it is the exact source of the `ret 8` defect
    this milestone keeps as a fixture: a wrong immediate on a return instruction that nothing
    refuses, that returns correct answers, and that kills the process a long way from its cause.
    **`arm64`'s `ret` takes no immediate at all, so that entire defect class is unrepresentable
    there**, and `x86-64` is caller-pops in both of its ABIs. Choosing `arm64` as the second
    backend therefore buys a second architecture without buying back the one defect class this
    milestone was opened by. **The rule is not that `x86-32` is unsafe.** It is that a milestone
    whose motivating accident belongs to one ABI does not add that ABI second, and a later
    milestone that wants it inherits both the fixture and the ABI table it will need.
- **What this MVP defers here, and what it does not.** This milestone runs under the programme
  [docs/mvp.md](mvp.md) records, and that record rather than this bullet is the authority for the
  terms. **Deferred, by the owner's instruction dated 2026-09-07**: approval of the boundary
  records, so every ADR this milestone touches stays `Proposed`; human review, so `HUMAN_REVIEW.md`
  stays unsigned and PENDING and every unit stays `HUMAN_PENDING`; evidence-bundle collection and
  milestone acceptance, so no row reaches `Accepted`; and section 2's amendment co-signing, which
  that section already records as unexecutable while one person holds every role. **Not deferred,
  and the list is short because each item is one whose deferral would make this record untruthful
  rather than merely unfinished**: the automated gates — the build, the suites, the rule register
  and the assurance generator's own consistency — where a rule that fails still fails; the status
  vocabulary, where a row says what its evidence shows and no more; the stop condition on an
  untruthful support claim, which is the one thing an MVP may not buy speed with; the
  non-advertisement of every composition and the three-package pack set; and the prohibition on
  publishing. **So a VM-7 bundle still owes every clause of the exit gate below that a machine
  checks, and the deferral discharges none of them.** What an MVP buys here is the right to build
  and merge unreviewed work, which [update rule 8](roadmap.status.md#4-update-rules) already
  grants. It buys nothing whatever about what may be claimed, and this milestone's whole subject is
  a claim.
- **Dependencies:** VM-2 for the verification boundary, VM-3 for the closure machinery this
  milestone extends, and VM-4 for lifecycle. **It does not depend on VM-5 or VM-6** and is not
  ordered behind them; the delivery-order note below says what "next" means with three milestones
  open at once.
- **Objective exit gate.** Every clause fails a build or a suite rather than being read:
  1. The amendment question is **answered in a dated record**: either the core contract version is
     unchanged with the reasoning retained, or a version is minted, or the milestone is `Blocked`
     naming its holder. An unanswered question is not a passing gate.
  2. **A negative control for each new rule** — a composition that maps executable memory with no
     register row; a page armed RWX; a backend claiming a RID with no publish-and-run record — each
     failing when injected and passing after revert.
  3. The **execution-only composition publishes and runs under JIT, trimming and Native AOT**, and
     its closure, read off the published output, contains a verifier and an executor and **no code
     generator**. The closure is the evidence; a linker annotation is not.
  4. **The verifier's answer for a native payload is pinned by a retained corpus**, in the same
     form as the eighty-seven artifacts VM-2 retains: each with its hash and its expected outcome,
     reason and diagnostic code. **A corpus of malformed input is not evidence about a code
     generator** — a wrong backend emits well-formed output, as the accident above did — so this
     clause pins the verifier, and the bundle states in its own words that the generator is pinned
     by nothing in this milestone and names what would pin it.
  5. The support table names, per declared RID, which native backends have published **and run**,
     names each **emitting-only** backend as one and states that it has run nowhere, and names the
     deterministic refusal everywhere else. **A backend with no run is a row that says so**, and a
     rule fails the release where an emitting-only backend appears in a support row without that
     word.
  6. **Nothing about a language is claimed**, and a rule fails the release if this milestone's
     bundle carries a figure about guest code.
  7. **The `arm64` backend is pinned without a run** *(added 2026-09-07)*: a golden-byte test per
     emitted instruction form, and a corpus whose every entry is decoded by a disassembler and
     compared against the instruction the encoder's own table names, both failing on a changed
     byte. **Neither is a claim that anything executed.** Clause 6 already forbids the bundle a
     figure; this clause additionally fails the milestone where the bundle, the support table or
     any release note describes the `arm64` backend as demonstrated, supported or working.

### Delivery order

```text
VM-0 graph, ownership, core contract version 1
  └→ VM-1 neutral runtime, catalog, fixture profile
       └→ VM-2 immutable artifact, verification, resource boundary
            ├→ VM-3 public profile contract and exact closures
            │    └→ VM-4 lifecycle, concurrency, diagnostics hardening
            │         ├→ VM-5 core overhead baselines
            │         ├→ VM-6 package, publish, recertify
            │         └→ VM-7 native artifact form, in-process native execution
            └→ (profile roadmaps build against the IMPLEMENTED contract;
                acceptance blocks the milestones their own ledgers name)
```

**VM-7 is the component's next primary objective, and that is a statement about attention rather
than about order** *(2026-09-07)*. Three milestones are open at once: VM-5 and VM-6 keep their
gates, VM-7 displaces neither, and none of the three waits on the other two. What "next" buys is
which one the owners are working on.

**One consequence runs backwards and is stated here rather than discovered at a release.** VM-7
changes release gate 2 and adds gate 11, and both are gates VM-6 is checked against — so under
[update rule 5](roadmap.status.md#4-update-rules) VM-6's retained bundle is evidence for an older
gate the moment VM-7's rules land. It is not carried forward silently: VM-6's row records what
recertifies unchanged and what must be re-collected, and a support table issued before VM-7's
native rows exist is a support table that predates its own gate. A component that widens what an
image may do owes its release train that arithmetic in advance.

**The order above is the order. What the MVP changes is what may be claimed at the end of it, and
not what runs inside it** *(added 2026-09-07)*. [The MVP programme](mvp.md) records what the
owner's instruction of that date defers — approval of the boundary records, human review,
evidence-bundle collection and milestone acceptance, and the amendment procedure's co-signing — and
records with equal force what it does not: the automated gates, the status vocabulary, the stop
condition on an untruthful support claim, the non-advertisement of every composition and the
three-package pack set, and the prohibition on publishing. Read against this diagram the
consequence is narrower than a reader might hope and is stated so the hope is not inferred: **no
milestone above moves faster because a decision was deferred, and none of them reaches
`Accepted`.** A deferred decision is a decision nobody took rather than a decision that went a
particular way, so wherever a milestone here takes a route that a deferred decision would have
chosen between, the route is recorded where its consequence is and named as
taken-without-a-decision — which is why section 9's memory-growth paragraph carries that phrase and
this one only points at it.

A profile roadmap's gates are its own, nothing in VM-0 through VM-7 waits for a profile, and no
profile result closes a core gate. **What a profile waits on is narrower than this note used to
say.** It read "a profile roadmap may begin as soon as VM-1's contract is accepted"; under
[update rule 8](roadmap.status.md#4-update-rules) — human review gates a release and not a
development step — a profile may open its plan and build against the contract **as implemented**,
and what waits on acceptance is each milestone its own ledger records as blocked, with this
component named as the holder. The distinction is the profiles' to draw and their ledgers draw it;
what this note owes is not to assert the stronger version *(revised 2026-09-01)*.

**One profile-owned document is named here, because VM-7 would otherwise read as the place its work
is tracked** *(added 2026-09-07)*. The JavaScript profile's output-form work — its backends, the
artifact section that carries their output, the path that arms a page, and anything it measures
about any of them — is planned by that profile itself, in
`src/Broiler.VM.Profile.JavaScript/docs/roadmap.backends.md`, whose stages are numbered JSB-1
through JSB-n in that profile's own identifier namespace. **This roadmap mints no identifier
there and no row of this component's ledger tracks one**:
[update rule 6](roadmap.status.md#4-update-rules) keeps
profile milestones out of the core's ledger, and a `VM-` number attached to a profile's backend
would be precisely the confusion that rule exists to prevent. What VM-7 owns is whether a
composition may execute a native payload at all and what it must declare in order to; what a JSB
stage owns is a compiler that produces one and a profile that runs it. **Naming the document is not
scheduling its stages**, and nothing in this section says that any of them has started.

---

## 14. Test and evidence matrix

| Area | Required tests/evidence | Failure that blocks release |
|---|---|---|
| Core/catalog | duplicate, alias, unknown and reserved IDs; version and core-contract-version mismatch; explicit selection; order independence; factory identity; application-local fixture; profile-neutral outcomes and typed payload preservation; **a two-profile catalog test proving that one profile's declared maxima reach the other's artifacts not at all, and that its adopted defaults do** *(corrected 2026-08-31 with the clamp: the maxima half used to ask that neither profile's maxima refuse the other, which is a property the contract no longer has and a test can no longer fail)* | reflection or name discovery, silent replacement, core reference to a concrete profile, an undeclared or forked core contract version, catalog drift, or **one profile's adopted default capping a dimension it never uses** |
| Dependency architecture | acyclic graph; core references no profile; no profile references another; no product package references a fixture or test project; no edge to a legacy Broiler component in either direction | any forbidden project or assembly edge, or undeclared dynamic loading |
| **Native artifact form and execution** *(added 2026-09-07, VM-7)* | a composition that maps executable memory carries a register row declaring it, and one that does not **cannot**; pages are armed W^X with a negative control over an RWX mapping; an execution-only composition publishes and runs in three modes with a closure containing no code generator; the verifier's answer for a native payload is pinned by a retained hashed corpus; the support table names each backend's publish-and-run RIDs and the deterministic refusal elsewhere, and names every emitting-only backend as one with no RID claimed and no figure attached | an undeclared executable mapping, an RWX page, a code generator in an execution-only closure, a native payload whose verifier answer is pinned by nothing, a backend claimed on a RID it has not run on, or an emitting-only backend described as supported, demonstrated or working. **Not blocked by, and not evidence of, a wrong code generator** — the corpus pins the verifier, and a wrong backend emits well-formed output |
| Artifact safety and policy | truncation, invalid sizes, indexes and framing, corrupt envelope, post-verification caller-buffer mutation, disposal and concurrent overwrite, verified-handle identity and lease lifetime, explicit default adoption, omitted-limit inheritance, host/profile/artifact intersection, invocation-only tightening, guest-initiated-load depth, fan-out and cumulative bounds, nested budget charging, missing-provider refusal, minimized fuzz corpus; **every bounded-read status produced by a named case and mapped onto its one ruled outcome category, asserted identically across every verifier in the graph** | invalid input executes, caller mutation changes execution, one runtime invalidates another's handle, omission becomes unbounded, policy raises a verified ceiling, a nested load enlarges or escapes its requesting operation's budget, a provider-less composition executes acquired bytes, **a ceiling breach reported as a malformed artifact or a framing failure reported as exhaustion**, **two verifiers in one graph answering one status differently**, unbounded allocation, crash, hang, or nondeterministic failure class |
| Persistence ownership *(conditional on approval; no milestone approves it — see [section 6](#optional-persisted-envelope))* | core outer-schema compatibility, rejection and migration; header and profile dispatch; atomic corruption handling; profile payload and cache-key boundaries; content authorization separate from checksum | ambiguous migration owner, outer compatibility mistaken for payload compatibility, torn update treated as valid, or checksum treated as authenticity |
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
   exclusions separately, and states that no language profile ships with the core. It additionally
   publishes **the state of the candidate-amendment register**, row by row, with the deterministic
   failure or named exclusion each unamended row leaves standing and with the procedure's
   unexecutability stated; **the state of section 8's extraction-gate register**, per candidate,
   with the failed condition and the condition that would reopen it; **the persistence position**,
   admitted by contract and provided by no composition, with its reopening measurement; and **the
   two bounds that are declared and not demonstrated** — the depth half of the cross-runtime
   `CallDepth` bound, and the reconciliation of two profiles' declared defaults, which belongs to a
   component that does not exist. A capability the contract admits and the product does not provide
   is named here or it is inferred from silence.
2. **Graph and registration:** the generated dependency closure matches VM-0, the catalog is
   static and documented, the generic runtime references no concrete profile, no product
   composition reaches dynamic loading, IL emit, **or an executable memory mapping its register row
   does not declare** *(widened 2026-09-07 for VM-7; the clause named only IL emit, which no
   hand-written machine-code path has ever gone near)*, and no edge reaches a legacy Broiler
   component.
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
   vulnerability response, and recertification owners are named. **Envelope recovery is conditional
   on approval**: no milestone approves persistence
   ([section 6](#optional-persisted-envelope)), so until a dated record does, this clause has no
   subject and a release does not fail for not evidencing it. What the release does owe is gate 1's
   published position.
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
11. **Native execution** *(added 2026-09-07, VM-7)*: every composition that maps artifact bytes
   executable declares it in the register and appears in the published closure with no code
   generator it does not also declare; pages are armed W^X; each native backend names the RIDs it
   has published **and run** on, or is named **emitting-only** and names none; and every declared
   RID without a backend carries a deterministic refusal rather than a silence. **A release may
   state that a native form exists and may not state what it is worth** — gate 8 is unchanged, and
   a language figure in a core bundle fails it. **An emitting-only backend is a backend a release
   may say exists and may not say works** *(added 2026-09-07 with `arm64`)*: its evidence is its
   own encodings, checked against a disassembler, and the word *supported* does not follow from a
   test that reads bytes back.

Recertification is required when the SDK or runtime, core contract version, package graph, host
capability surface, Native AOT settings, RID matrix, cache identity, resource defaults, or
representative workload changes.

---

## 16. Risks and stop conditions

| Risk | Mitigation / stop condition |
|---|---|
| The core becomes a lowest-common-denominator language runtime | Keep opcodes, values, frames, verifier rules, and semantics profile-owned. Apply section 8's extraction gate before sharing anything, and reject a shared primitive that introduces a profile-to-profile dependency or a semantic conversion tax. |
| One profile's **adopted default** silently caps another profile composed beside it, and the victim's verifier is refused for something it did not cause | The one catalog-wide term is stated in the record that owns effective-ceiling materialization, and a two-profile catalog test catches it rather than a reader. **Narrowed 2026-08-31 with the clamp**: this row used to name *both* terms and to require an unconstrained maximum on a dimension a profile declares inapplicable. A hard maximum is applied at P2 against the profile an artifact names and reaches no neighbour, so half this row described a defect rather than the contract, and the obligation went with it — see [Which ceiling terms are catalog-wide, and which are not](#which-ceiling-terms-are-catalog-wide-and-which-are-not). **Reconciling two profiles' declared defaults belongs to whichever component composes both, and no such component exists**; VM-6 names that as unowned in the support table rather than leaving a browser team to find it. **Stop: a default set to a profile's own usage is a composition defect, and a composition that hosts two profiles does not close a gate until that test exists.** |
| The extraction gate is never invoked, so duplicated mechanism accumulates with no record of why | A refused extraction produces a dated record naming the failed condition and the condition that reopens it, and each duplicated implementation points at it. Any profile owner or the core architecture owner may invoke, per ADR 0011; a profile supplies its half and records no verdict, and the verdict is filed in the core ADR set whoever invoked. **Stop: the verdict may be either, and an unsatisfied first condition is not a failure — but an unrecorded state is.** |
| **`CallDepth` is declared by every profile and charged by no code in this repository**, so the cross-profile seam's stated bound has never executed | Found 2026-08-31 and recorded rather than asserted away. Six descriptors declare a `CallDepth` ceiling; a search of the tree finds no charge site, because no fixture or consumer profile here has a call construct. What *is* now witnessed is the half that can be: a call chain crossing two runtimes under one shared parent is bounded by that parent, proved by a two-row case in which only the parent differs. The depth half stays unproven. **Closed by a profile with calls that charges the dimension, and a case in which an aggregate `CallDepth` ceiling refuses a cross-runtime chain.** Until then no document may describe the depth bound as demonstrated — and because no core milestone can close it, **VM-6 names the unproven half in the support table** rather than letting the aggregate bound read as wholly demonstrated. |
| Guest-controlled cost that grows with its input is charged flatly, so a bounded budget bounds nothing | Per-family declared monotone charging functions with a declared granularity and a ceiling floor, each with a retained fixture and an unsimplified control. The core cannot check this and says so; the compensating control is the evidence bundle. **Stop: an operation family without a proportionality fixture does not ship.** |
| Declared scalars — depth ceilings, uncharged-work bounds, charging granularity, poll bounds — are round figures rather than measurements | Each is derived from a retained, reproducible measurement on each claimed RID and recorded with it. **Stop: a stack overflow is not translatable into a result, so claiming to handle deep recursion without a measured bound is an untruthful capability claim.** |
| Owner and reviewer are the same person, so no gate here is independently confirmed | Roles are named per milestone and per record; where one person holds several, the non-independence is recorded as a residual limit on what these gates prove rather than resolved by assertion. **Stop: a vacant role stops the point that requires it; a role held by nobody does not pass to whoever is available.** |
| A profile's programme stalls on a precondition this component holds, and the blocker is described as scheduling | The core is the named holder of **two** such preconditions, and naming only the first is how the second gets read as a profile's own delay. **Contract acceptance**: every profile waits on it, and each blocked milestone is recorded blocked with its holder and its unblock condition. **The amendment procedure**: it is unexecutable while one person holds the minting role and both co-signing roles ([section 2](#core-contract-version-and-amendment)), so a profile that cannot express a capability under version 1 waits on a mint nobody here can perform. VM-6 publishes the candidate register's state so the wait is visible in the support table rather than only in that profile's ledger. **Stop: lack of scheduling is not a blocker; an unaccepted contract is, and so is an unexecutable amendment procedure — each is recorded with this component named as its holder.** |
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
| **A gate says less than the rule it enforces**, so a prohibition is published and not checked | Found 2026-09-07 while opening VM-7 and recorded rather than fixed quietly, because the hole predates the milestone that revealed it. Rule B5 forbids `System.Reflection.Emit.*`, `System.Runtime.Loader.*` and the reflection-invocation members; release gate 2 forbade "dynamic loading or IL emit". **Neither has ever reached a hand-written machine-code path**, so through VM-6 a composition could map a page executable, pass every automated gate this component has, and trip a stop condition this component published. Nothing did — but nothing would have said so. B5 gains the native-memory surface and a register-keyed allowlist at VM-7. **Stop: a rule whose only enforcement is a reader is not enforced, and a prohibition this component publishes without a check that fails is an untruthful support claim under the clause above.** |
| **A wrong code generator is not a wrong artifact**, so the whole verification apparatus answers a question nobody asked | The corpus, the fuzz target and the verifier all take *input* as their subject; a native backend's defect is in its *output*, and its output is well formed. VM-7's own probe produced a function that returned correct answers and killed the process by stack exhaustion several million calls later, from one wrong instruction in an epilogue. **No verifier of the artifact could have caught it, and no corpus of malformed input contains it.** The compensating control is a differential oracle — the same program lowered both ways, executed both ways, compared — owned by the profile that emits the code and not by this component. **Stop: a native backend that ships without one is shipping an unfalsifiable claim, and the core states in its support table that it holds no evidence about any generator.** |
| **An output form becomes an execution tier by increments** | Each step is small and locally reasonable: cache the mapped page, then keep it warm, then choose the form at instantiation, then choose it from a counter. The end of that path is the runtime code generator VM-7 declines and the JavaScript profile's non-goals refuse. The line is drawn at a property rather than at an intention: **the form is fixed when the artifact is verified, and a verified handle's form never changes.** Any promotion needs a newly verified handle, which is the same rule invariant 9 already applies to raising a ceiling. **Stop: a code path that selects a form from run-time observation, or that re-maps a handle's payload, is the second execution arm under another name.** |
| **Emitted code holds a managed reference the collector cannot see** | The core's promise that one runtime cannot corrupt another rests on the CLR's type and memory safety, which native frames leave. A profile whose value model carries a managed reference — the JavaScript profile's does, deliberately and on the record — cannot hand that reference to emitted code without a rooting scheme the collector understands. This is the profile's to solve and the core's to refuse: **the core admits a native form and does not thereby admit that any given value model may cross into it.** **Stop: a profile that cannot state where its emitted code's references are rooted has not earned the form, whatever its benchmarks say.** |
| **The RID matrix multiplies by backends**, and a green lane is read as a claim | One backend per architecture, each needing publish-and-run per invariant 7. VM-7 opens `x86-64`, whose emission and execution are both in scope, and `arm64`, whose emission is in scope and whose execution is not; `x86-32` is out of the milestone, and `ios-arm64` forbids the mechanism rather than lacking a runner *(revised 2026-09-07: this cell read "`x86-64` and `x86-32` are two, `arm64` a third VM-7 does not open"; the architectures changed and the obligation did not)*. `docs/support.md`'s existing rule applies unchanged — a RID whose evidence stops at a build is what publish-and-run refuses — and it now applies per backend as well as per RID. **Stop: a native backend named in the support table without a RID it has published and run on, unless the row says emitting-only and claims no RID at all.** |
| **An emitting-only backend is read as a supported one**, because a table that lists it lists it beside backends that run | `arm64` enters VM-7 with its emission in scope and its execution out of it, so its evidence is golden bytes and a disassembly-checked corpus rather than a run — and the difference between "the encoder wrote these bytes" and "a processor executed them" is invisible to a reader scanning a column of backend names. The compensating control is a word in the row rather than a footnote under it: the row says **emitting-only**, says it has run nowhere, and carries no figure. The underlying reason is not a missing runner and the table says that too — on `arm64` a written page is not coherent with the instruction stream until a maintenance sequence runs, and this component has no managed expression for that sequence. **Stop: an emitting-only backend described anywhere as supported, demonstrated or working, or carrying any figure at all, is an untruthful support claim under the clause below — and a backend that arms a page on `arm64` without the maintenance sequence is worse than one that refuses to, because it passes on the machine that wrote it.** |

Stop or re-scope a milestone when the graph is cyclic, a product closure reaches **undeclared**
dynamic code — a code generator, or an executable mapping, in an image whose register row declares
neither — test tooling, or a legacy component, a verifier cannot produce an immutable bounded
representation before execution, trusted policy can be weakened by artifact input, a second core
state machine is maintained for one language, the declared Native AOT composition cannot publish
and run, or the named ownership or maintenance ceiling is absent. A difficult or slow milestone is
not itself a stop condition; an untruthful support claim is.

***Narrowed 2026-09-07 for VM-7.** This clause read "a product closure reaches dynamic code",
unqualified, and it forbade VM-7 outright. One word carried the whole prohibition, and replacing it
with "undeclared" is the single largest widening of what a product image may do in this document.
It is recorded as a narrowing rather than as a correction: **the old clause was not wrong.** It
bought a property — that no image in this component could execute anything it had not been compiled
with — and VM-7 spends that property deliberately, in exchange for a declaration, a closure that
shows it, and gate 11. What replaces the old absolute is not a weaker rule but a rule with an
enforcement problem: **an absolute needs no allowlist and admits no mistake in one, and a
declaration needs both.** The register is now load-bearing where nothing was needed before, and the
six risk rows above are what guard it.*

---

## 17. Platform references

VM-0 records immutable revisions for implementation and release evidence; these moving links are
discovery entry points, not substitutes for the pinned manifests:

- [.NET Native AOT deployment and limitations](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [.NET Native AOT warning guidance](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/fixing-warnings)
- [.NET trimming options and analysis](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options)

A profile's own specification references belong in that profile's roadmap, not here.
