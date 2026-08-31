# ADR 0003 - Core Contract Version 1 And The Amendment Procedure

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** version 1 (contract-bearing)

**Core contract version:** 1

**Minimum supported version:** 1

## Context

Roadmap section 13 makes two of VM-0's exit conditions this record's business:
core contract version 1 is assigned, and its amendment procedure is published.
Three consumers need the number on day one and none of them can read prose - a
catalog entry must carry it (section 3), an evidence bundle must record it
(status ledger section 3, Identity), and the public support table must state it
(section 15 gate 1). Section 14 makes "an undeclared or forked core contract
version" a release blocker for the core/catalog area, which is unenforceable
unless membership in the contract is a mechanical property of a document set
rather than an argument.

This record therefore does four jobs. It assigns the version and closes its
boundary. It puts the number in one place that a build can read. It makes the
amendment procedure executable rather than aspirational, because section 2
states that at least one amendment should be planned for. And it owns the two
artefacts the other eleven records must share: the frozen vocabulary and the
frozen table of public names.

VM-0 is a decision-and-shell milestone. Except for the two constants named
below, every artefact this record names is a decision on paper. Nothing here
demonstrates a runtime, a catalog, a verifier or a budget, and no sentence in
this record may be read as evidence that one exists.

### Notation used by this record

Every artefact carries one of exactly three existence markers at its first
mention: `(exists at VM-0: <path>)`, `(VM-0 decision on paper; no file at
VM-0)`, or `(deferred to VM-n)`.

Two label sets are renamed from the analysis that produced them, because they
would otherwise collide inside this ADR set. The additive-classification clauses
are `AD1..AD5`, not `A1..A5`, because `A1..A11` are architecture-rule
identifiers in `rules.register.json` (exists at VM-0:
`src/tests/Broiler.VM.Architecture.Tests/rules.register.json`). The
version-admission rules are `AR1..AR5`, not `R1..R5`, because `R1..R6` are the
ownership roles ADR 0012 (`0012-security-ownership-and-support-matrix.md`)
defines. Nothing else about either set is changed.

## Decision

### 1. Version 1 is assigned, and it is one integer

Core contract version 1 is assigned, dated 2026-08-27. It is a single
monotonically increasing non-negative integer. There is no major/minor/patch
form, no per-artefact-class version, no version range, and no pre-release
suffix.

Section 2 asks for one integer; sections 3, 14 and 15 each need exactly one
answerable value per catalog entry, per evidence bundle and per support-table
row. Semantic versioning was rejected because a patch component invites a
semantics change smuggled in as a patch, and a minor component duplicates the
additive/breaking classification that section 7 of this record already owns
explicitly. Per-class versioning was rejected because it turns compatibility
into a matrix that no descriptor field can express and makes "the core contract
version" unanswerable.

The core contract version is stated by the runtime and never claimed by an
artifact. No artifact descriptor field carries one, and adding such a field is
forbidden (see section 5).

### 2. What version 1 covers, and what it does not

Version 1's normative content is exactly the seven artefact classes roadmap
section 2 enumerates, and no others.

| Class | Content | Roadmap anchor | Fixed by |
|---|---|---|---|
| 1 | Lifecycle states and legal transitions, including which steps may recur inside a guest-initiated load | invariant 10; section 7 steps 1-7 | ADR 0004 (`0004-lifecycle-and-state-machine.md`) |
| 2 | Operation-result categories, their stage-specific legality, and the rule that language outcomes are typed profile payloads adding no core case | section 7; invariant 4 | ADR 0005 (`0005-operation-result-envelope.md`) |
| 3 | Resource authority: precedence, host/profile/artifact intersection, monotonic tightening, re-verification to raise a ceiling, aggregate metering | invariant 9; section 7 run-time requirements | ADR 0007 (`0007-resource-authority-and-budgets.md`) |
| 4 | Verified-artifact ownership: the identities a handle is bound to, the snapshot-or-fully-decode obligation, shareability, leases, idempotent disposal | invariant 3; section 6 | ADR 0006 (`0006-verified-artifact-ownership.md`) |
| 5 | Guest-initiated loads: mediation, bounding, charging to the requesting operation, deterministic refusal | invariant 11; section 6 | ADR 0008 (`0008-guest-initiated-loads.md`) |
| 6 | External control: which transitions exist, who may request them, their distinctness from guest-initiated suspension and terminal cancellation | invariant 12; section 7 | ADR 0009 (`0009-external-suspension-and-async-instantiation.md`) |
| 7 | Host-capability shape: typed allowlisted versioned identity, declared reentrancy, thread affinity, exception translation, and the artifact-provider capability as a distinct kind never implied by a value capability | section 7 host capabilities | ADR 0011 (`0011-source-level-profile-contract.md`) |

Explicitly outside version 1, and changeable without an amendment:

- profile bytecode formats and their supported version ranges;
- feature manifests and their contents;
- profile IDs and their allocation;
- profile verifier, semantic and cache versions;
- NuGet package versions and the package graph;
- the public CLR API's names, shapes, overloads and assembly layout (section 3);
- the persisted outer-envelope schema, which carries its own envelope schema
  version per roadmap section 6. Only two clauses about the envelope are inside
  the contract: its lifecycle position (a bounded preprocessing step that never
  yields an executable handle and never bypasses profile verification) and its
  result mapping;
- diagnostics message text and diagnostic identifiers;
- RID matrices, composition names, trimming and Native AOT settings;
- performance and overhead figures; and
- the fixture profile.

Folding the envelope schema in was rejected because section 6 already gives the
envelope its own schema version; including it would mint a core contract
version, and therefore a section 15 recertification of Native AOT, packaging and
lifecycle evidence, whenever a header field is added, for a change with no
semantic content.

### 3. The public CLR API's shape is not contract content

The contract version governs meaning. The API baseline (deferred to VM-6)
governs shape. They are two independent freezes with two independent gates:
section 15 gates 1 and 4 turn on the contract version, gate 6 turns on API and
package baselines, and section 15's recertification list names "core contract
version" and "package graph" as separate triggers.

None of the following mints a version: renaming a type or member; adding a type,
member or overload; changing a parameter name or an optional-parameter default;
splitting, merging or renaming a promised assembly or package; moving a member
between promised assemblies; changing nullability annotations; adding or
changing analyzer diagnostics; changing XML documentation.

Every one of the following mints a version even if no identifier changes: making
a previously illegal lifecycle transition legal or the reverse; changing which
result category an outcome maps to; changing ceiling precedence, intersection or
charging; changing what a verified handle owns or when it may be shared;
changing when the artifact-provider capability is consulted, bounded or refused;
changing who may request external suspension or what a paused operation may
observe; changing the declared reentrancy, affinity or exception-translation
rules of the host-capability shape.

One coupling is mandatory. Version 1's semantics must be fully expressible
through the frozen public API. If an amendment cannot be implemented without an
API change, the amendment record names that change and the API baseline is
updated in the same change. Implementing an amendment by changing behaviour
behind an unchanged API is forbidden, and so is changing contract semantics
under cover of an API-baseline update.

Folding the API into the contract version was rejected because a rename would
then force an amendment and a full recertification for a change with no semantic
content, and because VM-6 is chartered to freeze the API separately. Leaving the
question to VM-6 was rejected because VM-1 through VM-5 will make API changes
long before VM-6, and silence means each is argued individually.

### 4. The contract-bearing set, and where the number lives

**The set is closed.** Ten records are contract-bearing: 0002 through 0011. Two
are not: ADR 0001 (`0001-component-topology-and-dependency-graph.md`), which
governs component shape, and ADR 0012, which governs roles, RIDs and support
truth. Neither carries contract surface, and section 3 above already places
shape outside the version.

Membership is mechanical, not editorial. Every ADR file (exists at VM-0:
`docs/adr/0001..0012` and `docs/adr/README.md`) carries a mandatory header field
immediately after `**Date:**`, reading either `**Core contract:** version 1
(contract-bearing)` or `**Core contract:** not contract-bearing`. Altering the
normative content of a contract-bearing record is an amendment; altering a
non-contract-bearing record is not.

**The number lives in exactly one place in code.** `VmCoreContract` (exists at
VM-0: `src/Broiler.VM.Abstractions/VmCoreContract.cs`) is a public static class
in `namespace Broiler.VM` whose only members are `public const int Version = 1`
and `public const int MinimumSupportedVersion = 1`. It carries no other member:
no version string, no history array, no formatting helper, no instance, so it
cannot accrete policy.

`const int` rather than `static readonly int`, because the value must be usable
as an optional-parameter default - the mechanism section 5 relies on to capture
"built against" - because it folds away entirely under trimming and Native AOT,
and because a test can compare it with a documented literal. A `static readonly`
field cannot be an optional-parameter default, so the built-against fact could
only be supplied by hand, and a profile assembly would read the loaded core's
value at run time, turning a compile-time fact into a run-time echo that always
agrees with itself. An assembly-level attribute was rejected because it must be
read reflectively, which invariant 2 keeps out of the runtime path, and cannot
be used in a constant context. A file generated by MSBuild from this ADR was
rejected because generation makes the ADR a build input; a hand-written constant
plus a failing test is reviewable and cannot silently regenerate a wrong value.

**Three registered rules bind the documents to the code.** All are owned by this
record and all run at VM-0 against subjects that exist.

Rule E1: VmCoreContract.Version and VmCoreContract.MinimumSupportedVersion equal
the values in ADR 0003's header fields. Status: Active; witness: the assertion
fails if either value is edited alone.

Rule E2: Every ADR file declares a Core contract header field, and the
contract-bearing set is exactly 0002 through 0011. Status: Active; witness: the
assertion fails if an ADR omits or mistypes the field.

Rule E3: Every contract-bearing ADR declares the current core contract version.
Status: Active; witness: the assertion fails if an ADR names a version other
than VmCoreContract.Version.

Rule E4 and Rule E5, both owned by ADR 0001, close the remaining halves: E4
binds the index and the rule identifiers, E5 pins the product graph's public
surface to exactly `Broiler.VM.VmCoreContract` and its two constants. This
record relies on E5 and does not restate it.

Keeping the version in prose only was rejected: catalog entries, evidence
bundles and support tables would each transcribe it, section 14's forked-version
release blocker would have nothing to diff, and drift would be found by a reader
rather than by a test. Placing the constant in `Broiler.VM.Runtime` was rejected
because section 5 assigns the core contract version to `Broiler.VM.Abstractions`
and a profile must read it without referencing the runtime.

An evidence bundle satisfies the ledger's "core contract version" Identity field
by recording the constant as read from a build output, never by transcribing
this record or the roadmap.

### 5. Admitting a declared version

A profile descriptor carries two distinct integers and they may not be collapsed
into one. Both are fields of the single descriptor field table ADR 0002
(`0002-profile-identity-and-static-catalog.md`) owns (VM-0 decision on paper; no
file at VM-0).

| Field | Kind of fact | How it is populated |
|---|---|---|
| `BuiltAgainstCoreContractVersion` | machine-derived | the descriptor factory declares it as an optional parameter defaulted to `VmCoreContract.Version`, so C# bakes the profile assembly's own compile-time core version in at the call site |
| `AuthoredCoreContractVersion` | human assertion | a required integer literal written by the profile author, recording the version whose semantics the profile was reviewed against; populating it from `VmCoreContract.Version` is forbidden |

A single field was rejected because it conflates a compile-time fact with a
review assertion: section 2 step 4 requires the support table to name "the
profiles and packages that require" a version, which needs the author's
assertion, while the no-binary-plug-in-ABI rule needs the compile-time fact.
Populating the authored value from the constant was rejected because it
auto-echoes on every recompile, making section 3's rejection of unsupported core
contract versions unreachable and turning the catalog's version record into a
tautology.

**Admission runs at catalog `Build()`** in this fixed precedence, reporting the
first failing rule so that a descriptor failing several has one deterministic
reason (deferred to VM-1; the catalog admission validator is the implementing
artefact).

| Order | Condition | Reason identifier |
|---|---|---|
| AR1 | `BuiltAgainstCoreContractVersion != VmCoreContract.Version` | `CoreContractBuiltAgainstMismatch` |
| AR2 | `AuthoredCoreContractVersion > VmCoreContract.Version` | `CoreContractVersionNotYetSupported` |
| AR3 | `AuthoredCoreContractVersion < VmCoreContract.MinimumSupportedVersion` | `CoreContractVersionRetired` |
| AR4 | `AuthoredCoreContractVersion > BuiltAgainstCoreContractVersion` | `CoreContractAuthoredExceedsBuiltAgainst` |
| AR5 | otherwise | accept, and record both integers in the catalog entry |

Rejection is a `VmCatalogValidationException`, never an `invalid artifact`
outcome: this is composition-time input, not artifact input. AR1 is the
concrete, testable meaning of "no binary plug-in ABI": binary compatibility
across core contract versions is not promised, so a profile assembly compiled
against a different core is an unsupported composition and is refused rather
than tolerated by assembly unification.

**The three-way ruling**, for a descriptor declaring N against a runtime
implementing M:

- **N = M.** Accepted. The normal, and the only, configuration in which both
  descriptor integers equal the runtime's.
- **N < M.** Accepted for `AuthoredCoreContractVersion` only, and only while `N
  >= MinimumSupportedVersion`. Because section 7 raises
  `MinimumSupportedVersion` to V whenever a breaking amendment mints V, the
  accepted window is exactly the additive-only tail, so an accepted N<M profile
  needs no compatibility behaviour anywhere. It runs under the current
  contract's single state machine, unshimmed: what versions N+1..M added is
  opt-in and is simply not requested by a descriptor that does not declare it.
  The catalog records N and the support table publishes "authored at N, running
  under M". `BuiltAgainstCoreContractVersion < M` is never accepted (AR1).
- **N > M.** Rejected deterministically, for either integer, always. There is no
  opt-in flag, no best-effort mode and no partial acceptance; a runtime cannot
  honour a contract it does not implement (invariant 8).

Accepting any declared version and adapting behaviour to it was rejected as a
second core state machine, which section 16 makes a stop condition and section
14 a release blocker. Accepting N>M optimistically and failing on first use was
rejected because invariant 8 requires unsupported surface to fail
deterministically and early, and a late failure reports a composition mistake as
a run-time fault - the exact diagnostic error section 7 warns about.

**Two further placements.** A verified artifact records the executing runtime's
own `VmCoreContract.Version` and never a caller-supplied claim. The artifact
descriptor supplied to verification carries no core contract version and may
never gain one: untrusted artifact input does not assert trusted contract
identity, exactly as it may only request lower resource limits (invariant 9).

**The persisted envelope header** (deferred to VM-2 for the header validator)
records the writing runtime's version. On load, a recorded version other than
`VmCoreContract.Version` yields `invalid artifact` with the stable reason
`CoreContractVersionMismatch`. There is no accepted window here and no
migration: an envelope is a cache, a cache miss costs a recompile, and
reinterpreting old bytes under new semantics is prohibited by section 6. A host
implementing a code cache treats the rejection as a miss.

### 6. The amendment procedure

Roadmap section 2's four steps are frozen as this checklist.

**Admissibility.** An amendment is admissible only when an already merged or
approved profile capability cannot be expressed by the frozen contract. A
speculative or anticipated need is not admissible. Anyone may propose; a
proposal naming a capability from a profile roadmap that has not been accepted
is filed and held, not minted.

**Minting.** Only the core-contract owner - role R5 in ADR 0012, vacant at VM-0
- mints a version. The verification-boundary owner co-signs whenever artefact
class 3, 4, 5 or 7 changes. The release and recertification owner co-signs
always, because the support table and section 15 recertification are affected.

**Record.** Two artefacts, in one change: a new ADR
`NNNN-core-contract-version-n+1.md` in the house format, marked contract-bearing
at version n+1 (deferred to whichever milestone first mints one); and a dated
revision block appended to the version-n record pointing at it. The version-n
record is retained and never edited retroactively, because ledger update rule 1
requires earlier decisions to be preserved as dated history and evidence bundles
cite a version whose text must stay readable after it is superseded.

The new record carries these rows, and a row may not be omitted:

| # | Required row | Why it is required |
|---|---|---|
| 1 | The driving capability, the profile that needs it, and the profile-owned design that was tried and rejected | section 2 step 1; asserting that no profile-owned design exists is not sufficient |
| 2 | The diff, per artefact class, each item marked added / changed / removed | section 2 step 2 |
| 3 | The counterweight check: whether the other intended profile could use the capability, is unaffected, **or records a refusal - a refusal being recorded and not blocking** | section 9 designates WebAssembly the counterweight for judging whether a proposed core feature is general or one language's need in disguise. Widened 2026-08-31: the first live counterweight answer was neither of the two the row admitted. A profile holding a veto over a core amendment would be a profile-to-profile dependency established by governance rather than by reference, which is what the extraction gate's fourth condition exists to prevent, so a refusal is an answer and not a bar. Editorial: section 2's closed list of seven artefact classes does not contain this procedure |
| 4 | The classification, additive or breaking, by the test in section 7 below | section 2 |
| 5 | The new `MinimumSupportedVersion`: unchanged for additive, raised to n+1 for breaking | section 7 below |
| 6 | The public API impact, which may be "none" | section 3 above |
| 7 | The evidence re-evaluation table | ledger update rule 5 |
| 8 | The support-table rows to be published | section 2 step 4; section 15 gate 1 |
| 9 | Which contract-bearing records change revision | keeps the closed set of section 4 above true |

A capability that only one language can ever use, and that cannot be expressed
without naming that language's concepts, is refused at row 3.

**Evidence triage.** Under ledger update rule 5 every accepted bundle is placed
in exactly one of three states, by a mechanical rule rather than by judgement. A
bundle must be re-collected if its gate covers any artefact class the diff
touched, or if it is Native AOT, packaging or release-gate evidence, because
section 15 lists a core contract version change as a recertification trigger
without qualification. Every other bundle recertifies unchanged and is annotated
with the amendment identifier. Earlier records are preserved and never
rewritten.

Re-collecting everything on every amendment was rejected: rule 5 asks for
per-record classification, and a blanket rule makes even an additive amendment
prohibitively expensive, which encourages hiding changes as editorial.
Recertifying nothing on an additive amendment was rejected because section 15's
trigger list names the core contract version unconditionally.

**Support table.** An amendment publishes the new version number and date, the
classification, the new `MinimumSupportedVersion`, the profiles and packages
that require the new version, and the deterministic failure returned for a
descriptor outside the accepted window. A release continues to name the version
it actually implements: minting n+1 does not retroactively change what a shipped
release claims. The contract version cannot express both what a release
implements and what it accepts, which is why gate 1 publishes two integers and
the out-of-window failure rather than one number.

**Standing rule and stop condition.** Exactly one core state machine and exactly
one core contract version exist in the product graph at any time. There is no
fork: no per-version runtime assembly, no per-version namespace or type, no
version-conditional branching in the runtime, and no per-profile contract. A
profile that can only be hosted by a second state machine is refused; its need
is either amended into the one contract or recorded as unsupported.

**Not an amendment.** Implementing something version 1 already admits (section
8) is not an amendment. A purely editorial revision of a contract-bearing record
is not an amendment, but it is recorded as a dated editorial revision that
states the behaviour which provably did not change. An editorial claim that
turns out to change behaviour is a defect, corrected by minting the next
version.

### 7. Additive versus breaking

An amendment is **additive** if and only if every clause holds.

| Clause | Test |
|---|---|
| AD1 | No existing lifecycle state or legal transition is removed, and no previously legal transition becomes illegal. |
| AD2 | No existing operation-result category is removed, renamed or re-scoped, and no outcome that previously mapped to category X now maps to category Y. |
| AD3 | No rule about resource-authority precedence, verified-artifact ownership, budget charging, guest-load bounding, external-control eligibility, or host-capability declaration is weakened, reversed or made optional. |
| AD4 | Every newly added transition, capability or obligation is opt-in through a descriptor declaration, so a profile that does not declare it observes version-n behaviour unchanged. |
| AD5 | A profile package whose source declares version n compiles unchanged against core n+1 and passes its existing contract tests without source edits. |

Anything else is **breaking**. Three consequences are stated here so they are
not re-argued later:

- **Adding an operation-result category is breaking.** Hosts and profiles handle
  categories exhaustively and the core cannot guarantee an unhandled new case is
  safe. This is deliberate pressure against section 16's risk that the core
  result enum grows one case per language, and it is consistent with section 7
  requiring both external suspension and guest-initiated loads to add no
  category.
- **Adding an opt-in transition that reuses an existing category is additive.**
- **Turning a named deterministic failure into a success is breaking**, even
  though nothing is removed. Hosts rely on refusals: section 11's worked example
  is a content policy expressed by registering no artifact-provider capability,
  so making a previously refused request succeed changes a security posture.

The classification drives exactly one mechanism. `MinimumSupportedVersion` is
unchanged by an additive amendment and raised to n+1 by a breaking one. That
single rule is what makes the accepted authored-version window
`[MinimumSupportedVersion, Version]` equal to the additive-only tail, and
therefore what makes accepting an N<M profile safe with no compatibility
behaviour in the runtime. The safety of section 5's N<M acceptance is a theorem
of this rule, not an assumption; letting `MinimumSupportedVersion` move
independently of the classification was rejected for exactly that reason.

Classifying by intent or by size of change was rejected: intent-based
classification resolves to "additive" under schedule pressure, and an
implementer must not be able to reasonably choose differently.

**No unspecified cells.** Version 1 contains no cell whose behaviour is
unspecified: every cell either states a behaviour or names a deterministic
failure (invariant 8). The classification test therefore never has to reason
about whether a profile could have relied on an unspecified reading, and
specifying a previously unspecified behaviour is not an available move.

### 8. Admitted versus implemented

Invariant 8 requires unsupported surface to be truthful, and roadmap section 13
requires each of VM-0's four open questions to be recorded "even where the first
release ships no implementation". The record of that is the table below, not a
published support table: `docs/support.md` is VM-6's and does not exist at VM-0.
ADR 0012 states that no public support table exists at VM-0 and cites this
table.

**An artefact admitted by version 1 discharges invariant 8 in exactly one of two
ways, and no third.** This narrows the rule as originally written, which
demanded a returned failure in every case:

- **Form (a): a named deterministic failure identifier** the shipping core
  returns when the artefact is requested. The failure is itself part of version
  1.
- **Form (b): absence from the public API baseline**, recorded as a named
  deterministic exclusion. Form (b) is legitimate only when no public member can
  express the request, so there is nothing for a caller to call and nothing for
  the core to refuse.

A type that exists and throws, or a method that silently no-ops, satisfies
neither form and is a shape-only stub, which invariant 8 rejects outright.

The **Implemented in core release 1** column is a VM-0 decision about what
release 1 must do. It is not evidence: no release exists, and the column is
recertified by the release and recertification owner at VM-6 (Exclusion EX-17).

| Artefact | Admitted by core contract version 1 | Implemented in core release 1 | Invariant 8 discharge | Owning ADR |
|---|---|---|---|---|
| Guest-initiated load mediation and bounding | Yes | Yes | (a) the composition-registers-no-provider refusal identifier | 0008 |
| A registered artifact-provider capability | Yes | No - no Broiler-advertised composition registers one | (a) the same refusal; registering none IS the content policy | 0008 |
| External suspension | Yes | Yes, behind the descriptor-declares plus runtime-enables double gate | (a) `ExternalSuspensionNotDeclared` / `ExternalSuspensionNotEnabled` | 0009 |
| Asynchronous instantiation | Yes | Yes, where the descriptor declares it | (a) `UndeclaredAsynchronousInstantiation` under the invalid-state outcome | 0009 |
| Aggregate budgets | Yes | Yes | not applicable; admitted and implemented | 0007 |
| The persisted envelope | Yes | No | (b) Exclusion EX-25; no envelope member appears in the public API baseline | 0010 |
| Streaming / incremental verification | No; adding it is a numbered amendment | No | (b) a deterministic exclusion in ADR 0010's block | 0010 |
| Lazy per-section verification | No; version 1 fixes whole-artifact eager verification | No | (b) a deterministic exclusion in ADR 0010's block | 0010 |
| In-process producer input (compile straight to a verified handle) | No; version 1 mandates the byte round trip | No | (b) a deterministic exclusion in ADR 0010's block | 0010 |

The table has no "Provided by this composition" column, and its absence must not
be read as a claim that every composition provides every admitted artefact; see
Exclusion EX-10 in section 13.

### 9. The frozen vocabulary

**Section 1's TEN terms are the frozen vocabulary**: VM profile, core contract
version, feature manifest, built-in profile, fixture profile, verified artifact,
guest-initiated load, artifact-provider capability, external suspension, and
deployment composition. Using a synonym for any of them in a document, an API
identifier, a test name, or a release manifest is a defect. The set is defined
by equality with the rows of roadmap section 1's table, never by a literal
count, so a future term added to the roadmap fails loudly rather than being
silently dropped to keep a number true.

| # | Rule |
|---|---|
| T1 | In the public API every type that names the concept carries the qualifier: `VmProfileId`, `VmProfileDescriptor`, and so on. No exported type is named exactly `Profile`, `IProfile`, `ProfileId`, `ProfileDescriptor`, `ProfileCatalog` or `ProfileFactory`, because those names appear unqualified in closure reports, support tables and consumer code that has already imported the namespace. Members of an already-qualified type may use the short form (`descriptor.Id`). |
| T2 | In test names the qualified term appears: `FixtureVmProfile`, `ApplicationLocalVmProfile`. A test name never says "profile" alone where VM profile, feature manifest or deployment composition is meant. |
| T3 | In release manifests, support tables and closure reports each row names the qualified term, and a row that mentions a language names the feature manifest identity and version, because a profile name alone is never a conformance claim. |
| T4 | Banned as synonyms for a VM profile everywhere: plug-in, add-in, extension, engine, backend, provider (reserved for the artifact-provider capability), module (reserved for profile-owned language concepts), and implementation. Plug-in and extension are banned specifically because they imply discovery and a binary ABI, which section 1's non-goals and section 15 gate 2 exclude - an implication can be created by a name alone. |
| T5 | "Runtime" unqualified means `VmRuntime`, the per-composition execution host; the .NET runtime is always written as ".NET runtime". "Version" unqualified is never used: write core contract version, profile-format version, feature-manifest identity, verifier semantic version, envelope schema version, or package version. |
| T6 | A core release claims the core - its contract version, its lifecycle and safety guarantees, and the compositions it can publish and run. It never claims a language, and no support row may be phrased so that it appears to. |

Two consequences of the qualifier rule are settled here rather than left to each
author. The unit of work is an **operation**; the word "session" is not used in
any record (it survives in this document only inside the amendment register's
quotation of the roadmap sentence that retires it). And unqualified **handle**
always means the verified-artifact handle: the per-runtime opaque transfer
reference is `VmOpaqueRef`, never `VmHandle`, because `VmHandle` beside the
verified-artifact handle is the exact ambiguity T1 exists to prevent.

Relying on the namespace to qualify short names was rejected because support
tables, closure reports, evidence bundles and test output carry no namespace:
the unqualified name is what a reader and a report actually see. Treating
terminology as review-enforced style was rejected because section 1 names APIs,
tests and release manifests explicitly and section 15 gate 1 makes the support
table's wording a release gate.

### 10. The frozen public-name table

Every name below is used verbatim by every record that mentions the concept, and
this record fixes it centrally for the whole ADR set. **The table is not the
whole frozen public surface, though, and absence from it never means a name is
free.** A public name that exactly one contract-bearing record introduces and
uses is frozen by that record's own normative text instead, no less firmly for
having no row here. Names frozen that way include ADR 0005's result vocabulary -
`VmOutcome`, `VmReason`, `IVmOperationResult`, `VmVerificationResult`,
`VmInstantiationResult`, `VmInvocationResult`, `VmSourcePosition`,
`VmCallerIdentity` and `VmHostCallOutcome`; ADR 0004's lifecycle objects
`VmInstance`, `VmOperation`, `VmObjectId` and `VmThreadAffinity`; and ADR 0002's
identity types `VmProfileId`, `VmFeatureManifestId` and `VmFormatVersionRange`.
No author invents a public name in either place: introducing a name a second
record must repeat is an edit to this table in the same change, and introducing
a single-record name is an edit to the record that owns it.

**The namespace is recorded once, here, and not per row: every exported type in
the product graph is declared `namespace Broiler.VM;`, written explicitly in its
file, in all three product assemblies.** ADR 0001 owns that rule and the
`RootNamespace` property that follows from it.

Except for `VmCoreContract`, every row is a VM-0 decision on paper; no file at
VM-0.

| Name | Kind | Owning ADR |
|---|---|---|
| `VmCoreContract` | static class, `Version` and `MinimumSupportedVersion` (exists at VM-0) | 0003 |
| `VmCatalog` | class | 0002 |
| `VmCatalogBuilder` | class | 0002 |
| `VmProfileDescriptor` | sealed immutable class, full-arity constructor | 0002 |
| `VmCatalogValidationException` | exception | 0002 |
| `VmRuntime` | class; `Resume(VmSuspension)` is the single resume entry point | 0004 |
| `VmRuntimeCreationResult` | result type returned by `VmRuntime.Create(catalog, options)` | 0004 |
| `VmControlResult` | enum `{Accepted, NoOp, InvalidState, Unsupported}` | 0004 |
| `VmDiagnostics` | readonly struct carried by every stage result | 0005 |
| `VmCoreDefectException` | exception | 0005 |
| `VmVerifiedArtifact` | class; the verified-artifact handle | 0006 |
| `VmVerifiedArtifactState` | enum `{Ready, Draining, Disposed}` | 0006 |
| `VmArtifactLifetimeKind` | enum `{Managed, Disposable}` | 0006 |
| `VmArtifactRepresentationKind` | enum; members owned by 0006 | 0006 |
| `VmAggregateBudget` | class | 0007 |
| `VmBudgetScope` | enum `{Invocation, Instance, Artifact, Runtime, Aggregate}` | 0007 |
| `IVmArtifactLoadMediator` | interface | 0008 |
| `VmGuestLoadBounds` | bounds group | 0008 |
| `VmArtifactOrigin` | enum `{Caller, GuestInitiated}` | 0008 |
| `VmOperationControlHandle` | class; exactly `RequestSuspend`, `RequestCancel`, `QueryState`, `TryTakeSuspension` | 0009 |
| `VmSuspension` | the single per-suspended-operation token | 0009 |
| `VmSuspensionOrigin` | enum `{Guest, External, Instantiation}` | 0009 |
| `VmOpaqueRef` | readonly struct; the per-runtime opaque transfer reference | 0011 |

Members frozen outside a type's own record, because more than one record names
them: `BuiltAgainstCoreContractVersion` and `AuthoredCoreContractVersion` on the
descriptor (0002, ruled here in section 5); `MaxSuspendedResidency` and
`MaxLiveSuspendedOperations` on the runtime-creation options (0009, cited by
0004).

Reason identifiers this record owns, all registered in ADR 0005's reason
registry: `CoreContractBuiltAgainstMismatch`,
`CoreContractVersionNotYetSupported`, `CoreContractVersionRetired`,
`CoreContractAuthoredExceedsBuiltAgainst`, and `CoreContractVersionMismatch`.

**Retired names.** These were proposed during VM-0's analysis and are struck.
They appear nowhere except in this row, and a record that uses one is wrong:
`VmSuspensionToken`, `VmSuspensionCause`, `VmCompositionException`,
`VmProfileIdentityMismatchException`, `VmCatalogResult`, `VmHandle`,
`VmArtifactOwnershipKind`, `DisposeRequested`, `RequestResume`,
`VerificationMode`, `EffectiveSectionVerificationMode`, `ProducedBy`,
`Allocation`, `HostCallCount`, `LiveRuntimeCount`,
`WallClockPausesWhileSuspended`.

### 11. The roadmap-amendment register

VM-0 does not re-author `docs/roadmap.md` (exists at VM-0: `docs/roadmap.md`).
The roadmap is the authority document and VM-0 was not asked to rewrite it. But
an ADR set that anchors to sentences its own analysis found contradictory is not
a freeze either, so the divergence is made auditable instead of tacit: every
sentence a VM-0 decision supersedes is listed below with its verbatim current
text and the text that would replace it. **Every row is `Proposed` and unapplied
except rows 1 and 13.**

Quoting convention. The old-text column is character-exact, with two departures
forced by the ASCII-only house rule and by table layout: a line break inside a
quoted sentence is normalised to a single space, and the box-drawing arrow in
section 5's diagram is written `[arrow]`. No other glyph in any quoted row is
outside ASCII.

| # | Section | Subsection | Old text (verbatim) | Proposed new text | Requiring ADR | Requiring ruling | Status |
|---|---|---|---|---|---|---|---|
| 1 | header | document header | `[The evidence ledger](roadmap.status.md) records VM-0 through VM-6 as not started.` | `[The evidence ledger](roadmap.status.md) is the authority for what has been accepted; at the time of writing it records VM-0 as in progress and unaccepted, and VM-1 through VM-6 as not started.` | 0012 | ledger update rule 1; VM-0 scope ruling item 7 | Applied at VM-0 |
| 2 | 2 | Core contract version and amendment | `It is versioned separately from any profile format, feature manifest, or package version, and every support table, catalog entry, and evidence bundle names it.` | `It is versioned separately from any profile format, feature manifest, package version, and persisted envelope schema version, and every support table, catalog entry, and evidence bundle names it.` | 0003 | core-contract-v1-scope-and-assignment | Proposed |
| 3 | 3 | registration example | `var vm = VmRuntime.Create(catalog);` | `var created = VmRuntime.Create(catalog, options);` | 0004 | gate-audit gap 4 | Proposed |
| 4 | 3 | catalog governance paragraph | `Registration rejects duplicate IDs, alias collisions, missing factories, unsupported versions, unsupported core contract versions, and descriptors whose declared identity differs from the produced executor.` | `Registration rejects duplicate IDs, confusable-ID collisions, missing factories, unsupported profile-format versions, unsupported core contract versions, and descriptors whose declared identity differs from the verifier and executor its factory produces; rejection is thrown, not returned.` | 0002 | no-profile-id-aliases; catalog-construction-failures-throw | Proposed |
| 5 | 5 | target-direction diagram | `Broiler.VM.Profile.X   [arrow] Abstractions + Binary (+ Runtime contracts)` | `Broiler.VM.Profile.X   [arrow] Abstractions + Binary` | 0001 | vm0-assembly-and-package-set | Proposed |
| 6 | 6 | Explicit descriptor, immutable verification result | `Where sharing and disposal are both supported, explicit leases, idempotent disposal, and deterministic use-after-dispose behavior prevent one runtime from invalidating another's input.` | `Explicit leases, idempotent disposal, and deterministic use-after-dispose behavior are unconditional, and prevent one runtime from invalidating another's input whether or not the representation is shareable.` | 0006 | verified-handle-lifetime-and-lease | Proposed |
| 7 | 7 | Lifecycle and result boundary, step 2 | `a runtime is created with typed host capabilities, authoritative resource ceilings, and declared affinity/reentrancy rules;` | `a runtime is created with typed host capabilities, authoritative resource ceilings, and declared affinity/reentrancy rules, and runtime creation returns an operation-result envelope like every other stage;` | 0004 | gate-audit gap 4; stage-outcome-matrix-v1 | Proposed |
| 8 | 7 | Lifecycle and result boundary, step 7 | `cancellation and idempotent disposal transition sessions, instances, and any explicitly disposable verified handles to documented terminal states and reject later use deterministically.` | `cancellation and idempotent disposal transition operations, instances, and any explicitly disposable verified handles to documented terminal states and reject later use deterministically.` | 0004 | lifecycle-object-set-and-ownership | Proposed |
| 9 | 7 | stage-specific categories, load/verification bullet | ``Optional envelope loading is a bounded preprocessing step whose outer-schema, corruption, migration, profile, and version failures use `invalid artifact`; it never yields an executable handle or bypasses profile verification`` | ``Optional envelope loading is a bounded preprocessing step whose outer-schema, corruption, migration, and version failures use `invalid artifact`, while an envelope naming a profile the catalog does not contain is `unsupported profile`; it never yields an executable handle or bypasses profile verification`` | 0005 | unsupported-profile-and-invalid-state-placement | Proposed |
| 10 | 7 | unsupported-profile paragraph | ``Selecting a profile the composition does not contain is not an invalid artifact: it is a distinct `unsupported profile` outcome naming the requested ID and the catalog's contents.`` | ``Selecting a profile the composition does not contain is not an invalid artifact: it is a distinct `unsupported profile` outcome naming the requested ID and, to the host alone, the catalog's contents; a result crossing to guest code or returned from a guest-initiated load names the requested ID only.`` | 0002 | unsupported-profile-outcome-and-catalog-disclosure | Proposed |
| 11 | 7 | Run-time requirements, aggregate bullet | `Where a host creates several runtimes under one shared aggregate budget, fuel, wall-clock, allocation, and live-runtime counts are metered against the parent as well as each runtime.` | `Where a host creates several runtimes under one shared aggregate budget, every dimension whose measure is summable across concurrently live runtimes under one parent is metered against the parent as well as each runtime, as ADR 0007's dimension table records.` | 0007 | aggregate-budget-core-object | Proposed |
| 12 | 7 | Run-time requirements, aggregate bullet | ``Exhausting the parent is reported as `resource exhaustion` to whichever operation observes it, and no runtime may be created or resumed once the parent has no remaining allowance.`` | ``Exhausting the parent is reported as `resource exhaustion` to whichever operation observes it, and once the parent has no remaining allowance no runtime may be created and no operation may be resumed.`` | 0007 | aggregate-budgets-contract-level-constraints | Proposed |
| 13 | 8 | The extraction gate | `A new shared component is opened only when all four hold: two or more profiles already implement the behavior; the implementations are compared and the shared part is identified from real code rather than anticipated; the shared part is expressible without naming any language concept; and extracting it does not create a profile-to-profile dependency.` | `A new shared component is opened only when all four hold: two or more product VM profiles already implement the behavior, the fixture profile and the application-local consumer profile counting toward neither; the implementations are compared and the shared part is identified from real code rather than anticipated; the shared part is expressible without naming any language concept; and extracting it does not create a profile-to-profile dependency.` | 0011 | sharing-and-extraction-gate-governance | Applied 2026-08-31, wording differs |
| 14 | 14 | Artifact safety and policy, blocking-failure column | `unbounded allocation, crash, hang, or nondeterministic failure class` | `unbounded allocation, crash, hang, or a failure whose category, resource dimension and budget scope are not deterministic` | 0007 | resource-exhaustion-detail | Proposed |
| 15 | 14 | Lifecycle/concurrency, blocking-failure column | `or unbounded cancellation latency` | `or a cancellation latency not bounded in declared work units` | 0004 | cancellation-contract | Proposed |
| 16 | 15 | gate 1 | `**Support truth:** the public table names the core contract version, the compositions, host capabilities, guest-initiated-load and external-control support, RIDs, and deterministic exclusions separately, and states that no language profile ships with the core.` | `**Support truth:** the public table names the core contract version the release implements, the minimum core contract version it accepts, the deterministic failure returned for a descriptor outside that window, the compositions, host capabilities, guest-initiated-load and external-control support, RIDs, and deterministic exclusions separately, and states that no language profile ships with the core.` | 0003 | descriptor-and-envelope-contract-version-compatibility | Proposed |
| 17 | 16 | risk: concurrent runtimes multiply a host ceiling | `Meter fuel, wall-clock, allocation, and live-runtime counts against a shared aggregate budget as well as each runtime, and refuse creation and resumption once the parent allowance is spent.` | `Meter every summable dimension in ADR 0007's table against a shared aggregate budget as well as each runtime, and refuse runtime creation and operation resumption once the parent allowance is spent.` | 0007 | aggregate-budget-core-object | Proposed |
| 18 | 16 | risk: external pause becomes an unbounded or privileged side channel | `Declare who may request external suspension, keep it distinct from guest suspension and terminal cancellation, bound how long a paused operation may block disposal, and leave what a paused profile exposes to the profile.` | `Declare who may request external suspension, keep it distinct from guest suspension and terminal cancellation, bound how long a paused operation may block disposal with a mandatory finite MaxSuspendedResidency, latch an abandoned external suspension as cancelled, and leave what a paused profile exposes to the profile.` | 0009 | external-suspension-transitions-and-authority | Proposed |

**Row 13 was applied on 2026-08-31 and its wording differs from the proposal.**
The roadmap now restricts the gate to product profiles and excludes the fixture
and consumer profiles by name, which is the substance this row proposed, but it
says so in two sentences rather than one and adds the reason - that those
profiles are core-owned and shaped to fit the contract, so agreement between
them is evidence about the core's own tests. It also says "real merged code"
where this row said "real code". The row is marked applied rather than
re-proposed because no clause of it is unmet; a reader comparing the two should
expect a paraphrase, not a quotation. The same edit added three things this row
did not propose and that no row here covers: that a failed gate obliges a dated
record and a source-level pointer rather than silence, that invocation belongs
to the core architecture owner with a profile supplying only its half, and four
further candidate rows in the sharing table. Those are recorded as Exclusion
EX-104 below rather than as register rows, because this register runs from the
ADR set to the roadmap and they run the other way.

No row proposes a change to an engineering invariant, to a milestone gate, to
section 13's delivery order, or to sections 14, 15 and 16 beyond the four
blocking-failure and mitigation cells listed above. In particular **invariant 3
is not amended and no section 2 invariant is amended.** ADR 0010
(`0010-embedding-decisions.md`) records that the mandatory byte round trip
needed no amendment to invariant 3, because invariant 3's own final sentences
already place bytes a profile obtains while executing in scope; the absence of a
required amendment is itself the evidence that the conservative branch was the
compatible one.

Two consequences of leaving the register unapplied are recorded in section 13
below as Exclusion EX-11 and Exclusion EX-12, where each is stated once. This
section cites those identifiers and does not restate their text.

### 12. The candidate-amendment register

Section 2 states that at least one amendment should be planned for. Nine
candidates are recorded so that the sentence has named content and so that the
shape of each is fixed while it is cheap. **The register records shape, not
intent: no candidate is proposed, approved or scheduled, and none is admissible
until it names a merged or approved profile capability.**

Rows 5 to 9 were added on 2026-08-31, after both intended profiles had written
roadmaps naming what they would ask for. Adding them changes no clause of this
record and mints no version: a shape in this register is not a proposal, which
is what the paragraph above says and what makes the register safe to extend
under Exclusion EX-15. Two of the five are worth reading together rather than in
order. **Row 5 is the only candidate both intended profiles independently rate
general**, which is the strongest evidence this register carries about any of
them, so it is listed first among the additions. And **row 6 was the one the two
profiles graded in opposite directions** - the profile with no parser, no text
format and no dynamic loads called it the strongest ask in its document while the
profile with all three called it weak, which is precisely the disagreement section
6's counterweight step exists to surface.

*Corrected 2026-08-31: that disagreement is resolved and this paragraph described
it as live for longer than it was.* Both roadmaps now grade the argument channel
the same way and both fix its scope at **arguments only**, the result channel
being adequate as it stands; the profile that had graded it weak did so reasoning
from a host that compiles a program rather than a call, which stops holding the
moment it hosts another profile. Row 6 is recorded at that scope. The lesson the
paragraph is kept for is that a register entry describing a disagreement outlives
the disagreement unless someone re-reads it.

| # | Candidate | What would drive it | Classes touched | Clause it must clear | Owning ADR |
|---|---|---|---|---|---|
| 1 | Relax the ceiling component of the cross-runtime sharing predicate from exact equality to element-wise subsumption (a handle no looser than the receiving runtime) | a host sharing one compiled artifact across realms whose runtimes carry different ceilings | 4 | breaking under the third stated consequence of section 7: a refusal becomes a success | 0006 |
| 2 | Incremental or streaming verification of an artifact as it arrives | a latency budget that cannot wait for whole bytes; funded by VM-5's verification-throughput measurements | 4, 2 | AD5: it changes the load stage's input contract | 0010 |
| 3 | An in-process producer input form that compiles straight to a verified handle | a runtime-compiling composition paying serialization on its critical path | 4 | AD3: minting it would be the amendment invariant 3 does not currently need | 0010 |
| 4 | Lazy per-section verification, each section verified before its own first execution | a host that compiles function bodies on first call and will not verify a whole bundle to run one entry point | 4, 3 | AD3 and AD4: it must stay opt-in and must not weaken the pre-execution guarantee | 0010 |
| 5 | A charging hook for work a host capability performs on a profile's behalf | wall clock bounds a slow capability and bounds nothing about one that allocates; **both intended profiles rate this general and neither has a profile-owned alternative** | 3, 7 | AD3: it adds a charging obligation and must not make any existing one optional | 0007 |
| 6 | A typed argument channel on the invocation request | a module that is nothing but exported functions with typed signatures, whose conformance suite invokes them with arguments end to end; and a profile hosting another one, where an export call is a typed call whose arguments originate on the other side | 2 | AD2: the entry-point name is a request field, so widening it must not re-scope any existing category | 0005 |
| 7 | Multiple results on a host capability | a calling convention admitting more than one result, whose second result has nowhere to go and is refused rather than truncated | 7 | AD3: a capability signature is a declared shape and widening it must not weaken the declaration rules | 0011 |
| 8 | A wider value slot on the capability channel | one value type in one instruction family that does not fit the current slot; splitting works and needs a published encoding | 7 | AD5: a profile compiled against the narrow slot must still compile | 0011 |
| 10 | Nested instantiation of a guest-loaded handle | a module system whose dependency must become its own instance rather than run in the requesting frame; the JavaScript roadmap names it and grades it moderate, and the WebAssembly roadmap declines it outright, having no instruction that asks for code while running | 5, 1 | AD4 - it must be opt-in through a descriptor declaration - and AD1, because it makes a nesting-depth bound live that is presently unreachable | 0008 |
| 9 | A refusable retention member on the metering surface | a language whose guest must observe a refused growth and continue, where the retention report returns nothing and the refusal is latched for the next charge or poll | 3 | AD3: it must not let a profile learn a remaining value, which is the asymmetry the four-member surface exists to hold | 0007 |

Candidates 1 and 2 are funded by VM-5 measurements, per section 16's
critical-path risk row: a latency regression discovered after the contract is
frozen costs an amendment, so the choice rests on numbers rather than on
anticipation.

Candidate 10 is registered because ADR 0008 struck a clause that had described it
as already permitted - "plus a child instantiation where the profile's declaration
permits one", against a declaration with no such part and a path Exclusion EX-78
records as unreachable since VM-2. Striking the clause was editorial; restoring
the capability is not, and the shape is fixed here so that the difference is
visible. EX-78's own closing line names the same amendment: *"closed by an
amendment that lets a profile instantiate what it loaded, or a provider that may
re-enter."*

Candidates 3 and 4 acquired a counterweight answer on 2026-08-31 that this
register should carry, because it is the answer the procedure asks for and it
points the other way from the drivers above: the profile that consumes a format
an external toolchain produces **declines both**, having no in-process producer
to bypass serialization for and no wish to see a deferred check reported as a
trap. A refusal is recorded and is not blocking - a profile holding a veto over a
core amendment would be a profile-to-profile dependency established by governance
rather than by reference, which is what the extraction gate's fourth condition
exists to prevent - but section 6 row 3 admits only "could use it" or
"unaffected", and neither is what was said. Widening that row to admit a recorded
refusal is item 6 of Exclusion EX-104.

### 13. Exclusions

Each exclusion below is defined here, once, and is carried elsewhere by
identifier: the VM-0 evidence bundle (exists at VM-0: `docs/evidence/vm-0/`) and
the VM-0 ledger row cite these identifiers, and the bundle's Decision field
quotes in full only the subset that most limits it. The full sentence lives
here.

Exclusion EX-10: the admitted-versus-implemented table carries no "Provided by
this composition" column at VM-0. Reason: the advertised composition set is
empty and VM-0 creates no composition root. Closed by: VM-3.

Exclusion EX-11: VM-0 proposes but does not apply 17 roadmap amendments; the
roadmap and the ADRs therefore disagree in the listed places until an owner
lands the patch. Reason: the roadmap is the authority document and re-authoring
it is outside VM-0. Closed by: R5 sign-off on the register.

Exclusion EX-12: no test asserts that the roadmap-amendment register's quoted
old text still occurs verbatim in `docs/roadmap.md`. Reason:
`rules.register.json` is closed at 28 rules at VM-0 and contains no
documentation rule of this shape. Closed by: registering such a rule at VM-6.

Exclusion EX-13: no test asserts the frozen public-name table against the ADR
set, so a record naming an unlisted or retired public name fails no gate at
VM-0. Reason: the product graph exports one type, so there is no name surface to
check, and the documentation half is not registered. Closed by: VM-1, when the
first contract types exist.

Exclusion EX-14: the terminology rules T1 to T6 are enforced by review only at
VM-0. Reason: `rules.register.json` contains no naming rule, and Rule E5 pins
the public surface to a single type whose name already complies, so a naming
assertion would have no subject. Closed by: VM-1, when the public surface first
contains types a denylist can inspect.

Exclusion EX-15: the amendment procedure is unexercised and currently
unexecutable. Reason: no amendment has been minted, and the minting role R5 and
both co-signing roles are held by the same person, as ADR 0012 records, so a
  co-signature is not independent. Closed by: a second named maintainer.

Exclusion EX-16: the accepted authored-version window is the single point `[1,
1]`, so section 5's N<M acceptance path and section 7's
`MinimumSupportedVersion` mechanism have no reachable case at VM-0. Reason: no
amendment exists. Closed by: the first amendment; the first breaking one
exercises the window's lower bound.

Exclusion EX-17: the "Implemented in core release 1" column of the
admitted-versus-implemented table is a VM-0 decision and not evidence. Reason:
no release exists and no capability has been demonstrated. Closed by: VM-6.

Exclusion EX-104: the roadmap states obligations whose placement in this set was
audited on 2026-08-31. **Six of the seven items it originally listed were wrong
about the record, and they were wrong in the direction that costs most: they
claimed a ruling was missing when it existed.** The audit and its corrections are
below, because an exclusion that overstates a gap is worse than no exclusion - it
sends the next reader to mint an amendment nothing needed.

**All seven items are closed.** Six were wrong about the record - they claimed a
ruling was missing when it existed - and closed without an amendment. The seventh,
item 2, was a real contradiction between a record and its implementation, and was
closed by ruling that the record was right about the maximum and silent about the
default. No version was minted by any of them.

1. ~~the bounded-read status mapping~~ **- closed 2026-08-31.** The remarks
   attributed the mapping to ADR 0006 and ADR 0007, neither of which mentions
   either status, but **ADR 0005 rules it already** - "a value that contradicts
   the format is `InvalidArtifact`; a value that is well formed but exceeds a
   configured bound is `ResourceExhaustion`; where both apply at one point,
   `InvalidArtifact` wins." Nothing was unspecified, so section 7's bar never
   applied and no version was minted. Four verifier implementations were corrected
   to match the record.

2. ~~both catalog-wide ceiling terms in the ordered precedence algorithm~~ **-
   ruled and closed 2026-08-31.** This was the only item that reached a genuine
   contradiction rather than a false citation: ADR 0007's P1 row carries a closed
   inputs column excluding any descriptor and puts `ProfileMax` at P2, while the
   implementation clamped at P1 to the tightest maximum across the whole catalog.
   The two produced different numbers on a catalog of unlike profiles, so no
   editorial pass could close it - one of the two had to be wrong.

   **The record was ruled correct for the maximum and incomplete for the default,
   and the two halves resolved differently.** The P1 clamp is removed: a profile's
   hard maximum now constrains only its own artifacts, which is what P2 always
   said, and no version is minted because the row is unchanged. Nothing is
   loosened that a profile could exploit - P2 still intersects with the selected
   profile's maxima - and what is gone is a profile being refused for a neighbour's
   declaration. The `AdoptProfileDefault` marker is the opposite case: the record
   named it once and never said whose default it adopts, which is a genuinely
   unspecified cell against section 7's claim that version 1 has none. It resolves
   to the tightest default in the catalog, recorded in ADR 0007 as an erratum
   rather than changed, because at runtime creation a default has no owner and the
   conservative answer is the only safe one.

   **The asymmetry is the ruling**: a maximum has a correct owner one step later,
   so P1 must not guess at one; a default has no owner at all, so P1 must. The
   `Unconstrained`-on-inapplicable obligation falls away with the clamp that
   required it.

   *What made this expensive is worth keeping.* The clamp was enforced by code,
   described in two profiles' XML comments, a composition register and an evidence
   bundle, and asserted by **no test at all**, while the record that owns
   effective-ceiling materialization said something else. It was pinned by a test
   before it was ruled on, and that test is what turned the ruling into a
   one-line change with a visible failure rather than a search.

3. ~~the host-exception translation precedence, "which both profile roadmaps
   restated from the implementation because no record states it"~~ **- closed
   2026-08-31, and the quoted premise was false.** ADR 0011 states it, in the
   record section 2 names as the owner of class 7: *"Translation precedence is
   ordered and exhaustive. Evaluate in order; stop at the first match"*, with X1
   cancellation, X2 exhausted meter, X3 host failure. The roadmaps cited nothing
   because nobody looked, not because nothing existed.

4. Split in two, and the halves ended differently.
   - **4a, ~~sibling assemblies inside ADR 0011 P1's reference set~~ - closed
     2026-08-31.** P1 and ADR 0001's quoted sentence both now carry the qualifier:
     the set is of Broiler.VM-owned assemblies and a profile component's own
     siblings are not members of it. Filed as a dated editorial revision on both,
     which section 2 permits because the subject is package graph and assembly
     layout - explicitly outside version 1 - and because nothing changed: no
     product profile exists, and rule A13's subject test requires a test-only
     project, so no enforcement could have reached a sibling either way.
   - **4b, ~~the reconciliation of roadmap section 5 with section 10 on where a
     format lives~~ - closed, and it was already closed when this exclusion was
     written.** Commit `4cb815e` reconciled both sections twenty-four minutes
     before commit `462c746` recorded them here as unreconciled. A stale to-do,
     filed by the same author in the same session.

5. ~~who invokes the extraction gate and where the record is filed~~ **- closed
   2026-08-31.** Both named halves were false: ADR 0011 names who and names where.
   The two genuine residues are now recorded in ADR 0011 itself - the unsatisfied-G1
   filing, which the gate's own failure branch cannot cover because invocation is
   barred until G1 holds, and the reconciliation of the "first real invocation"
   clause, which was a statement about *when* and had been read as one about *who*.
   Neither was an amendment: the extraction gate is not among the seven artefact
   classes. The original text read:

   *OPEN, far narrower than described, and both named halves were false.* The
   original text said ADR 0011 *"delegates the first invocation to a profile
   roadmap without naming one"*. ADR 0011 names who - *"any profile owner or the
   core architecture owner may invoke"* - names where the record is filed - *"in
   this ADR set"* - and already carries the failed-gate filing and the
   source-level pointer from each duplicated implementation. The genuine residue is
   two things. The **unsatisfied-G1** state, which the roadmap now obliges a filing
   for and which no record carries. And a **live conflict this component
   introduced**: the roadmap says the first invocation *"belongs to the core
   architecture owner, not to a profile"*, contradicting ADR 0011's *"any profile
   owner or"*. The roadmap is the half that moved and is the half to correct.
   Neither is an amendment - the extraction gate is not among the seven artefact
   classes.

6. ~~this register's own section 12, brought level with what the profiles have
   asked for~~ **- closed 2026-08-31.** Section 12 gained a tenth row for nested
   instantiation, the stale paragraph about the two profiles grading the argument
   channel oppositely is corrected, and section 6 row 3 is widened to admit a
   recorded refusal. The original text read:

   *OPEN and mostly discharged.* Section 12 gained five rows on 2026-08-31 and
   is level with the profiles on all but one candidate: nested instantiation of a
   guest-loaded handle, which the JavaScript roadmap names and which the evidence
   side has carried open since VM-2 as EX-78. Two residues remain. This record's
   own section 12 prose about the two profiles grading the argument channel
   oppositely is **now stale**, both having since fixed the scope at arguments
   only. And section 6 row 3 still admits only *"could use the capability or is
   unaffected"* while section 12 already rules that a refusal is recorded and not
   blocking; widening row 3 is an editorial revision of the amendment *procedure*,
   which section 2's closed list of seven classes does not contain.

7. ~~that a retained-state dimension cannot carry a guest-observable refusal~~
   **- closed 2026-08-31, and this is the least creditable of the four.** The fact
   is stated in this record's own section 12 row 9, and the observation point is
   ADR 0007's *"live operations fail at their next charge or poll"*. It was never
   missing: **row 9 and this exclusion were added by the same commit.** One change
   wrote the fact into section 12 and called it unrecorded in section 13. Recorded
   as a reconciliation rather than a discovery, because the alternative is letting
   a bookkeeping error read as a contract gap.

**The generalisation, and the reason this exclusion is worth keeping now that most
of it is struck.** Every closed item closed the same way: a record already ruled
the matter, and the code, the roadmap or this exclusion cited a different record,
an older draft, or nothing at all. Not one needed the amendment procedure EX-15
records as unexecutable. Before any remaining item is treated as blocked, check
that the claim *"no record carries it"* was tested against all twelve records
rather than against the one the code happens to name - which is the check the
original filing of items 3, 5 and 7 skipped.

## Consequences

**Two supersessions of illustrative roadmap text are recorded here.** Section
3's canonical registration snippet `var vm = VmRuntime.Create(catalog);` is
superseded by the shape ADR 0004 owns; section 3 itself introduces the snippet
as a shape whose "exact public names are deferred to VM-0", so no amendment is
owed, and the supersession is recorded as register row 3 rather than applied to
the roadmap. Section 5's diagram arrow `Broiler.VM.Profile.X [arrow]
Abstractions + Binary (+ Runtime contracts)` is superseded by ADR 0001's strict
reading, recorded as register row 5. Neither edit is made; `docs/roadmap.md` is
unchanged by this record.

**The version cannot drift.** Any change to the number is a two-file change -
the constant and this record's header - and Rule E1 fails on either half alone.
Rule E2 and Rule E3 make the contract-bearing set and its declared version a
set-equality assertion over all twelve files rather than an editorial habit.

**`Broiler.VM.Abstractions` is non-empty at VM-0**, which is what lets the
architecture tests reflect over a real exported surface while proving the
assembly references nothing. The cost is two constants that fold away entirely
under trimming and Native AOT; nothing in this record gives any shell behaviour,
and the constants do not pre-empt VM-1's implementation.

**A profile package must be recompiled and republished for every core contract
version**, additive or not, because AR1 refuses a built-against mismatch
outright. The support table therefore has to list which profile package versions
run under which core contract version, and a mixed-package composition is never
a silently supported configuration.

**New capabilities are naturally shaped as opt-in descriptor declarations**,
because clause AD4 makes that the only route to an additive amendment. New
result categories become rare and expensive by design: a profile need that
appears to require one is first tested against carrying a typed profile payload
behind an existing category.

**VM-6 acquires new work.** The repository has no ApiCompat or public-API
baseline tooling today, so section 3's separation of shape from meaning depends
on a baseline mechanism VM-6 must build. The amendment record template gains a
required public-API-impact row, which may say "none".

**A profile author reads two coordinates**, not one: the package version they
compile against, and the core contract version whose semantics their profile was
reviewed against. That is the direct consequence of refusing to let one integer
carry both facts, and of gate 1 publishing two integers plus the out-of-window
failure rather than a single number.

**Nothing here is accepted.** Role R5, the only role that may accept this record
or mint its successor, is held but has recorded no decision, which is why the
status above reads
`Proposed`. VM-0 is `In progress` in the ledger and `In progress` is not
acceptance for any purpose, including VM-1's "After VM-0 acceptance"
precondition.
