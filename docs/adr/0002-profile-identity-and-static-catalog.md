# ADR 0002 - Profile Identity, Version Semantics, And The Static Catalog

**Status:** Proposed
**Date:** 2026-08-27
**Core contract:** version 1 (contract-bearing)

## Context

Roadmap section 3 fixes the shape of static registration and then defers "the
exact public names ... to VM-0". Invariant 1 makes an explicitly supplied
profile identity the entire selection mechanism, and invariant 2 removes every
fallback a name-based system would otherwise have: no probing, no scanning, no
second guess. That combination only works when two questions have byte-exact
answers - what a profile is called, and when two names are the same name.

Section 3 also enumerates what a catalog entry must provide without fixing
types, optionality, or a failure surface, and section 14's core/catalog row
requires tests for "duplicate, alias, unknown and reserved IDs", for
order independence, and for factory identity. None of those is implementable
while the words remain prose.

This record settles the identity layer and the catalog that holds it: the ID
grammar and its two distinct comparison rules; the reserved namespace and the
honest limit of enforcing it at run time; whether aliases exist at all; the
four version axes and where each is checked; the feature manifest's shape; the
single frozen descriptor field table; the catalog's failure surface; the order
of identity and version checks on the load path; what an `UnsupportedProfile`
outcome may disclose and to whom; what "catalog order has no semantic effect"
means at the byte level; and the six conditions under which a source generator
may replace a hand-written catalog.

Two things this record does not own. The result categories and their precedence
belong to ADR 0005 (`0005-operation-result-envelope.md`), which this record
cites and never restates. The fifteen budget dimensions belong to ADR 0007
(`0007-resource-authority-and-budgets.md`), likewise.

## What exists at VM-0, and what does not

Nothing in this record is implemented. The VM-0 product graph exports exactly
one public type and it is not one of these.

Rule E5: the product graph exports exactly one public type,
`Broiler.VM.VmCoreContract`, whose only members are the two contract-version
constants. Status: Active; the assertion fails if any product assembly exports
a second type.

The only artefact this record names that is a checked-in file is
`VmCoreContract` (exists at VM-0:
`src/Broiler.VM.Abstractions/VmCoreContract.cs`), whose two constants ADR 0003
(`0003-core-contract-v1-and-amendments.md`) assigns. Every name below is a
VM-0 decision on paper; VM-1 implements them verbatim.

| Name frozen here | Kind | Exists at |
|---|---|---|
| `VmProfileId` | validated value type over the ID grammar | VM-0 decision on paper; no file at VM-0 |
| `VmFeatureManifestId` | validated value type over the manifest grammar | VM-0 decision on paper; no file at VM-0 |
| `VmFormatVersionRange` | inclusive `(Min, Max)` pair of `uint` | VM-0 decision on paper; no file at VM-0 |
| `VmProfileDescriptor` | sealed immutable class, full-arity constructor | VM-0 decision on paper; no file at VM-0 |
| `VmCatalog` | immutable, order-normalized entry set | VM-0 decision on paper; no file at VM-0 |
| `VmCatalogBuilder` | obtained from `VmCatalog.CreateBuilder()` | VM-0 decision on paper; no file at VM-0 |
| `VmCatalogValidationException` | the composition-time failure | VM-0 decision on paper; no file at VM-0 |
| `VmCatalogValidationReason` | its named, open reason set | VM-0 decision on paper; no file at VM-0 |
| `VmProfileCatalogListing` | the host-facing enumeration | VM-0 decision on paper; no file at VM-0 |
| the canonical catalog encoding | the identity oracle of a built catalog | VM-0 decision on paper; no file at VM-0 |
| the descriptor field table | the frozen entry contract, printed below | VM-0 decision on paper; no file at VM-0 |

A drift test that asserts an implementation against the descriptor field table
is deferred to VM-1; it has no rule identifier at VM-0 and no row in
`src/tests/Broiler.VM.Architecture.Tests/rules.register.json` (exists at VM-0).

## Decision

### 1. The profile-ID grammar and its two comparison rules

A profile ID is an ASCII, dot-separated, case-preserved token. The grammar is
frozen at core contract version 1:

```text
id    := label ('.' label){1,7}
label := alnum ('-' alnum)*
alnum := [A-Za-z0-9]+
```

with the additional rule that the first character of the first label is an
ASCII letter `[A-Za-z]`.

| Bound | Value |
|---|---|
| Labels per ID | 2 to 8 |
| Characters per label | 1 to 64 |
| Characters per ID | 3 to 128 |
| Alphabet | ASCII letters, digits, `-` as an interior separator, `.` between labels |

The bounds are the decision, not decoration: an ID therefore contains no
whitespace, no path separator, no `:`, `*`, `?`, `"`, `<`, `>`, `|`, no
leading, trailing, or doubled hyphen, no empty label, and no `..` sequence. A
host may use an ID verbatim as a file-name component, a cache-key segment, a
log field, or an evidence-bundle key without escaping. Section 6 puts the
profile ID inside the persisted envelope and inside the verified-artifact
handle's identity, and section 14 requires every accepted evidence bundle to
record identity; both need exactly this property.

Two comparison rules, and they are deliberately different:

| Operation | Rule | Why |
|---|---|---|
| MATCHING - artifact descriptor to catalog entry, handle-sharing identity match, envelope profile dispatch, capability lookup by profile | ordinal, case-sensitive (`StringComparison.Ordinal`, or `SequenceEqual` over the raw bytes) | invariant 1 permits one entry per request; the ID recorded in a handle, an envelope, and an evidence bundle must be the ID the caller supplied |
| UNIQUENESS - collision detection at registration | ASCII lowercase fold (`c \| 0x20` applied only to `A`..`Z`) | catches the confusable pair at composition time instead of letting two entries shadow each other at run time |

The ASCII-only grammar makes the fold a pure byte operation: no ICU, no
`CultureInfo`, no dependence on `InvariantGlobalization`, and identical
behaviour in JIT, trimmed, and Native AOT hosts, which section 14 requires of
every failure class and section 15 gate 5 requires of every published
composition. The core never lower-cases, upper-cases, trims, or otherwise
rewrites an ID it stores or echoes: the roadmap's own reserved namespace is
spelled `Broiler.*`, and a canonicalizing core would make its diagnostics
disagree with the composition root's source.

The ID is a validated value type, `VmProfileId`, with `TryParse` and `Parse`
over `ReadOnlySpan<char>`. `default(VmProfileId)` is empty and every core API
rejects it as malformed, so an unvalidated string cannot reach a catalog entry
or a lookup. No public core member that stores or matches an identity accepts a
raw `string`.

Rejected: `OrdinalIgnoreCase` matching, because two spellings would select one
entry and the recorded identity would depend on which spelling arrived first.
Rejected: canonicalizing to lower case on storage, for the reason above.
Rejected: Unicode IDs, which import normalization, confusable, and casing rules
that need ICU and make the collision rule culture-dependent and untestable
across RIDs. Rejected: a GUID or integer ID, which cannot express the
ownership section 3 requires and produces unreadable diagnostics for exactly
the single-profile products section 7 calls out.

This forbids: any culture-sensitive comparison applied to a profile ID;
`OrdinalIgnoreCase` anywhere on the matching path; normalizing, trimming, or
re-casing a declared ID; single-label IDs; and accepting an identity as a raw
string at any public core member that stores or matches it.

### 2. The reserved first label `broiler`, and the honest limit

An ID is reserved when its first label equals `broiler` under the ASCII
lowercase fold. Reservation is exactly that one label; core contract version 1
reserves no other prefix. Reserved IDs may be declared only by Broiler-owned
profile packages. The naming scheme under the reserved label is frozen here:
core-test fixture profiles use `Broiler.VM.Fixture.<Name>` (deferred to VM-1)
and product profiles use `Broiler.VM.Profile.<Language>`, of which this roadmap
plans none - section 1 puts each in its own component with its own roadmap, so
no such identity exists anywhere at VM-0. An application-local profile uses a
reverse-domain ID
under a DNS domain the application controls: at least two labels, a first label
that does not fold to `broiler`, and a first pair forming a prefix its owner
can demonstrate. VM-3's consumer profile (deferred to VM-3) uses the
documentation-reserved `com.example.*`.

Enforcement is split across three levels, and the split is the decision:

| Level | What it checks | What it cannot do |
|---|---|---|
| (a) catalog construction, at `Add` | self-consistency only: a reserved ID is accepted only if the entry's `PackageIdentity.PackageId` also begins with `Broiler.` under the same fold. Otherwise `VmCatalogValidationReason.ProfileIdReservedNamespace` is thrown | it catches accident, not forgery - exactly as section 6 says a checksum detects corruption but does not authenticate code |
| (b) CI, at release | the authoritative reservation check: VM-3's ID-governance job (deferred to VM-3) compares every ID declared in the repository and in every published package manifest against the owned-package list | nothing at run time |
| (c) invariant 2 | composition is static, so only code the application deliberately linked can register anything at all | it is a property of the composition, not a check the core performs |

The reserved namespace is a GOVERNANCE rule protecting diagnostics, support
tables, and evidence identity from ID squatting. It is not a security boundary
and no security property may be derived from it. Stating it any other way would
be the shape-only claim invariant 8 rejects.

Rejected: a hard-coded allowlist of Broiler package names inside the core,
because the core would then name concrete profiles, which invariant 2 and
section 5 both forbid, and the list would rot with every new profile. Rejected:
a strong-name or assembly-identity check, which needs reflection over assembly
identity on the composition path, is hostile to Native AOT, and still proves
nothing about a statically linked in-image forger. Rejected: relying on CI
alone, which loses the cheapest signal for the most common accident - an
application copying a Broiler descriptor as a template and leaving the ID.

### 3. Core contract version 1 admits no alias mechanism

A catalog entry has exactly one identity: one `VmProfileId`. The descriptor
declares no `Aliases`, `AlternateIds`, `LegacyIds`, `PreviousIds`, or
`Deprecated` member; the catalog exposes no redirect, no fallback map, and no
"also known as" lookup. No amendment may add one without a recorded driving
capability under section 2's amendment procedure.

The only aliasing the core recognises is the one it rejects. This is the
concrete content of section 3's "alias collisions" and of section 14's required
alias test:

| Condition | Reason |
|---|---|
| two entries whose IDs are ordinally equal | `DuplicateProfileId` |
| two entries whose IDs are equal under the ASCII lowercase fold but not ordinally equal | `ProfileIdAliasCollision` |

Renaming or superseding a profile is not an alias; it is a new identity. A
profile that must migrate ships two descriptors, each with its own ID, its own
verifier, and its own executor factory reporting its own ID. The core does not
know they are related, and the composition root decides whether to register one
or both. A retired ID that a composition does not register produces
`UnsupportedProfile` - the same outcome as any absent profile. The core defines
no deprecation warning, no soft redirect, and no grace period.

Rejected: an `Aliases` set that is empty in release 1, which freezes a member
with no consumer - the shape-only surface invariant 8 rejects - and forces the
canonical encoding, every listing, every diagnostic, and every drift check to
carry a field that is always empty. Rejected: deprecated-ID redirects, which
make the core own a deprecation policy and make the recorded identity differ
from the requested one.

### 4. `DisplayName` is required, non-localized, and mechanically inert

`DisplayName` is a required human-readable label of 1 to 64 UTF-16 code units,
with no `U+0000`, no C0 or C1 control characters, no `U+2028` or `U+2029`, and
no leading or trailing space. Non-ASCII characters are permitted. No Unicode
normalization, casing, or collation is required or performed, because
normalization behaviour under `InvariantGlobalization` plus Native AOT is a
runtime property this record will not assume - and since the field is never
compared, normalization would buy nothing.

`DisplayName` participates in nothing mechanical. It is never compared, folded,
sorted, or used for lookup, uniqueness, ordering, cache keys, handle-identity
matching, or envelope dispatch. Two entries may carry the same display name;
only IDs must be unique. It appears in the host-facing listing, in diagnostics
beside the ID and never instead of it, and in support tables.

It is non-localized for four reasons, each of which is a gate rather than a
preference: it appears in evidence bundles, closure reports, and support tables
that section 14 requires to be comparable across machines, lanes, and RIDs;
localization means resource lookup and satellite assemblies inside every
product closure, changing the exact composition closure invariant 7 and section
15 gate 5 require to publish and run with warnings as errors; a support case
must be able to quote a diagnostic that means the same thing to the reporter
and the maintainer; and it would introduce a globalization dependency into a
component that otherwise needs none. A host that wants a localized label maps
`VmProfileId` to its own resource string, outside the core.

This forbids: an optional or defaulted display name; localizing, translating,
or resource-loading one inside the core; and using it in any cache key, handle
identity, envelope field, or drift check other than as an echoed value.

### 5. Four version axes, never derived from one another

| # | Axis | Owner | Type | Checked | Failure |
|---|---|---|---|---|---|
| 1 | Core contract version | the core | `int` >= 1; no structure, no dotted parts | at catalog construction | thrown `VmCatalogValidationException` |
| 2 | Profile-format version | the profile | `uint` >= 1; `0` is reserved as unset and rejected | at verification | `InvalidArtifact` / `UnsupportedProfileFormatVersion` |
| 3 | Feature-manifest ID | the profile (content), the core (shape) | opaque namespaced token, ordinal only | at verification | `InvalidArtifact` / `UnsupportedFeatureManifest` |
| 4 | Package version | release engineering | the NuGet or assembly version string | never | none - it is inert data |

Axis 1. `VmCoreContract.Version` and `VmCoreContract.MinimumSupportedVersion`
are both `1` at this release. A descriptor carries two distinct contract-version
integers whose admission predicate ADR 0003 owns; this record only fixes where
they are checked, which is at catalog construction and nowhere else. A
descriptor declaring a version the core does not implement is rejected, never
optimistically accepted: the core cannot honour transitions it has not
implemented, and accepting would surface later as an undefined lifecycle
transition instead of a named composition failure. A caller-supplied artifact
descriptor carries no core contract version at all, because it describes an
artifact rather than a composition. A persisted envelope does record one, and
an envelope whose recorded version is outside the supported range fails the
bounded envelope preprocessing step; that stage and its reason belong to ADR
0006 (`0006-verified-artifact-ownership.md`).

Axis 2. The core knows no structure - not semantic versioning, not
major/minor - and performs only ordered comparison. A descriptor declares an
inclusive `VmFormatVersionRange` with `1 <= Min <= Max`; an artifact descriptor
declares exactly one version. At catalog construction only the range's
well-formedness is checked.

Axis 3. A descriptor declares a set of 1 to 64 accepted manifest IDs; an
artifact declares exactly one. Membership is exact set membership under ordinal
equality.

Axis 4. Package version is recorded in `PackageIdentity` for diagnostics,
support tables, and evidence bundles, and is excluded from the canonical
catalog encoding, so a package bump alone cannot change catalog identity. No
core code path compares, orders, parses, or branches on it.

**The cross-product rule.** The core checks axes 2 and 3 independently. A
descriptor declaring range `[1,3]` and manifests `{A,B}` makes the core accept
all six pairs. If a profile's legal `(format version, manifest)` combinations
are not the full cross product, the profile's own verifier rejects the illegal
pair as `InvalidArtifact` with core reason `UnknownFeature`, disambiguated by
the profile's stable 32-bit diagnostic code. **The core's acceptance of a pair
is necessary but not sufficient**, and no descriptor member may hold a pair
table, a matrix, or a combination predicate. Modelling the legal pairs would
put language-version structure into the core, which invariant 4 forbids, and
would duplicate authority the profile verifier holds anyway.

**What a profile name alone claims: nothing.** A profile ID by itself claims no
language version, no specification revision, no conformance result, no feature
set, no format version, no optimization tier, no performance property, and no
compatibility with any artifact. A support claim is the tuple `(ProfileId,
accepted format-version range, accepted manifest set, core contract version,
conformance manifest identity and version)` published in a support table and
backed by a retained evidence bundle. This is section 1's "a profile name alone
is never a conformance claim", made printable.

Rejected: semantic-version strings for axis 2 with core-side parsing, which
puts allocation, culture, and a failure taxonomy inside the core for a value
only the profile understands, and invites the core to reason about
compatibility that is language-owned. Rejected: requiring the descriptor's
contract version to equal the core's exactly and forever, which makes every
additive amendment a hard break and contradicts section 2. Rejected: letting
package version participate in catalog identity or a cache key, which would
make a no-op version bump invalidate caches and churn drift baselines.

### 6. What the core fixes about a feature manifest

The core fixes identity, cardinality, ordering, comparison, and immutability.
It never stores, reads, enumerates, parses, or compares manifest CONTENT. There
is no feature list, no feature flag, no capability bit vector, and no member
that could hold one.

| Property | Ruling |
|---|---|
| Identity | a `VmFeatureManifestId` and nothing else. Grammar as in section 1 above, with two additions: it must begin with its own profile's ID followed by `.` and at least one further label; bounds are 256 characters and at most 12 labels |
| Structural operations | exactly one, ever: the namespace prefix check at catalog construction. Every other use is opaque ordinal equality |
| Cardinality | a descriptor declares 1 to 64; empty is `FeatureManifestSetEmpty`, over 64 is `FeatureManifestSetTooLarge`. An artifact declares exactly one |
| Ordering | the accepted set is normalized to ascending ordinal order at catalog construction. Declaration order is not retained and has no observable effect anywhere |
| Comparison | ordinal, case-sensitive, exact. No wildcard, no prefix match, no range, no `any`, no ordering relation, and no notion that one manifest supersedes or implies another |
| Immutability | the set is materialized into a frozen, defensively copied collection. Any caller-supplied array, span, or builder buffer is copied; the catalog never aliases caller-owned mutable storage |

Namespacing under the owning profile's ID makes manifest IDs globally unique by
construction, so no cross-entry uniqueness rule is needed and a manifest can
never be claimed by a profile that does not own it. Normalizing rather than
preserving declaration order is what makes order independence a byte-level,
testable property instead of a promise. Copying rather than aliasing is
invariant 3's snapshot rule applied to composition data, and it is testable by
mutating the caller's array after `Add` and after `Build`.

A profile that needs finer granularity than a set of opaque IDs mints more IDs;
it does not ask the core for structure.

Rejected: a structured set of named feature flags the core stores, which makes
every new language feature a core data change - directly invariant 4 and
section 16's lowest-common-denominator risk. Rejected: ordered manifests where
a newer one implies an older one, because implication between language surfaces
is a language claim the core cannot evaluate. Rejected: a wildcard
"accept any manifest" token, which turns a support claim into a blanket one and
removes the deterministic `UnsupportedFeatureManifest` failure.

### 7. The frozen descriptor field table

`VmProfileDescriptor` is a sealed immutable class with a full-arity
constructor, deliberately not a struct: `default(T)` over a struct would
present an empty identity and a zero contract version as though they had been
declared, whereas a null reference is rejected loudly. Catalog construction
happens once per process, so the allocation saving a struct would buy is
irrelevant next to that hole.

**This table is the single frozen entry contract.** It is the union of every
field any contract-bearing ADR requires; each contributing record cites this
table rather than restating it. No ADR may add, remove, or retype a row without
editing this record in the same change, and doing so is a core contract
amendment classified by ADR 0003's additive-versus-breaking test. This record
asserts no field count: a count authored here could not survive the other
records adding mandatory declarations, and the amendment rule preserves what a
count was for - that a descriptor cannot grow silently.

Every field is required and none is defaulted, with two exceptions noted in the
table: `BuiltAgainstCoreContractVersion` (row 22), defaulted to a core
constant, and `ArtifactSharing` (row 27), defaulted to the restrictive
`RuntimeScoped`.

| # | Field | Type, all paper at VM-0 | Semantics fixed by |
|---|---|---|---|
| 1 | `ProfileId` | `VmProfileId` | this record, section 1 |
| 2 | `DisplayName` | `string`, 1..64 | this record, section 4 |
| 3 | `DescriptorRevision` | `int` >= 1, incremented whenever anything that can affect verification changes | ADR 0006 |
| 4 | `SupportedFormatVersions` | `VmFormatVersionRange` | this record, section 5 |
| 5 | `AcceptedFeatureManifests` | frozen ordinal-ascending set of 1..64 `VmFeatureManifestId` | this record, section 6 |
| 6 | `Verifier` | directly referenced instance or static delegate; declares its own `ProfileId` and contract versions | this record, section 8 |
| 7 | `ExecutorFactory` | per-runtime executor factory delegate, trim-rooted by direct reference | this record, section 8 |
| 8 | `ArtifactRepresentationKind` | `VmArtifactRepresentationKind` | ADR 0006 |
| 9 | `ArtifactLifetimeKind` | `VmArtifactLifetimeKind {Managed, Disposable}` | ADR 0006 |
| 10 | `SupportsConcurrentVerification` | declared, not defaulted | ADR 0004 (`0004-lifecycle-and-state-machine.md`) |
| 11 | `ThreadAffinity` | declared affinity | ADR 0004 |
| 12 | `CancellationPollBound` | bounded poll interval | ADR 0004 |
| 13 | `AbandonBudget` | bounded allowance for the abandon path | ADR 0009 (`0009-external-suspension-and-async-instantiation.md`) |
| 14 | `LimitDefaults` | per-dimension bounded defaults; no member may encode "unbounded" or "unset" | ADR 0007 |
| 15 | `ProfileHardMaxima` | per-dimension profile hard maxima | ADR 0007 |
| 16 | `BudgetDeclarationMatrix` | fixed-length `{Charged, NotApplicable}` over the dimension set | ADR 0007 |
| 17 | `HostCapabilityDescriptors` | frozen, defensively copied set, possibly empty; duplicates by capability ID rejected | ADR 0011 (`0011-source-level-profile-contract.md`) |
| 18 | `GuestInitiatedLoads` | `NotDeclared`, or `Declared` with its provider contract version, `VmGuestLoadBounds`, and verifier-work rate | ADR 0008 (`0008-guest-initiated-loads.md`) |
| 19 | `AsynchronousInstantiation` | declared or not declared | ADR 0009 |
| 20 | `ExternalSuspension` | declared or not declared | ADR 0009 |
| 21 | `PayloadKindIdRange` | the closed range of profile payload kind IDs | ADR 0005 |
| 22 | `BuiltAgainstCoreContractVersion` | `int`; factory-defaulted to the constant `VmCoreContract.Version` | ADR 0003 |
| 23 | `AuthoredCoreContractVersion` | `int` literal written by the profile author; never derived from a constant | ADR 0003 |
| 24 | `ConformanceManifestId`, `ConformanceManifestVersion` | opaque identity of the conformance corpus, ordinal; used for support tables and evidence only, never for matching | this record |
| 25 | `DiagnosticsIdentity` | opaque token under the profile's own ID namespace | this record |
| 26 | `PackageIdentity` | `PackageId` + `PackageVersion` + `OwnerTag`. `PackageId` participates only in the reserved-namespace self-consistency check; `PackageVersion` participates in nothing | this record |
| 27 | `ArtifactSharing` | `{Shareable, RuntimeScoped}`; defaulted to `RuntimeScoped` when a profile declares nothing. A verifier may narrow an artifact to `RuntimeScoped` and may never widen it | ADR 0006 |
| 28 | `FaultRecovery` | `{InstanceRecoverable, InstanceFatal}`; mandatory, no default | ADR 0004 |
| 29 | `MaxUnchargedWork` | `uint` >= 1; the bound, in the profile's own work units, on work performed between two `Poll()` calls | ADR 0007 |
| 30 | `ChargingGranularity` | `uint` >= 1; the charging granularity `g` of ADR 0007's obligation CO-1, in the same work units | ADR 0007 |

Empty `HostCapabilityDescriptors` means the profile imports nothing; it is a
legal and expected state, not an omission.

**Excluded by construction at core contract version 1**: priority, precedence,
ordering hint, enabled flag, alias set, deprecation marker, feature content,
localized text, file path, assembly name, type name, and any string intended to
be resolved into a type. The last of these is the seed of reflection-based
composition and is forbidden by invariant 2 whether or not the core resolves it
today. Priority and enabled flags are forbidden because section 5 says a
composition root is an explicit package and never a run-time option that
removes an already rooted profile.

Each profile exposes its own descriptor through a static accessor on its own
type. There is no aggregate type naming several profiles - section 3 rejects
one by name, because it would reference every profile assembly and defeat
VM-3's exact-closure gates.

Rule B7: no product assembly exports a type named `BuiltInProfiles`,
`DefaultProfiles`, `AllProfiles` or `KnownProfiles`. Status: Vacuous at VM-0 -
it runs, and nothing in the VM-0 graph can violate it; it becomes non-vacuous
at VM-1 when the product graph exports a type other than `VmCoreContract`.

Rejected: optional fields with sensible defaults, because a default is a claim
nobody made and section 13 requires each of the guest-load, asynchronous-
instantiation, and external-suspension questions to carry a recorded decision
rather than silence; a defaulted `false` is silence in code form. Rejected: a
fluent per-field descriptor builder, which turns "forgot a field" from a
compile error into a run-time failure and multiplies the construction paths
every identity and drift check must then cover.

### 8. Catalog construction throws; it is not an envelope-bearing stage

`VmCatalogBuilder.Add` and `VmCatalog.Build` fail by throwing
`VmCatalogValidationException`, carrying a `VmCatalogValidationReason`, the
offending `VmProfileId` - or the ordinal position when the ID itself is
malformed - and the offending field name. A call on a consumed builder throws
the same exception with reason `BuilderConsumed`; the builder and catalog state
tables are ADR 0004's.

Catalog construction is **not** one of the seven envelope-bearing stages ADR
0005 enumerates. Result envelopes exist for stages that consume untrusted
input; a catalog is authored by the composition root from trusted compile-time
data, so a defect there is a wiring bug that must be loud and unrecoverable
rather than a value to inspect. This also keeps the core's run-time category
set exactly what section 7 lists, which is what makes adding a category a
breaking amendment (invariant 10, and section 16's result-enum risk).

The closed exception set of core contract version 1 is ADR 0005's and is not
restated here; `VmCatalogValidationException` is its composition-time member.
`VmCompositionException`, `VmCatalogResult`, and
`VmProfileIdentityMismatchException` are struck names and are not minted.

**When each rule runs.** Single-descriptor rules are enforced eagerly at `Add`,
so the exception's stack names the offending registration call - the single
most useful datum for a wiring bug. Cross-entry rules run at `Add` against the
entries accepted so far and are re-validated at `Build`, which additionally
enforces set-level rules.

| Reason | Raised when |
|---|---|
| `ProfileIdMalformed` | a declared ID fails the frozen grammar |
| `ProfileIdReservedNamespace` | a reserved ID whose `PackageId` does not fold to a `Broiler.` prefix |
| `DuplicateProfileId` | two ordinally equal IDs |
| `ProfileIdAliasCollision` | two IDs equal under the fold but not ordinally |
| `DisplayNameMalformed` | display name outside the bounds in section 4 |
| `FormatVersionRangeInvalid` | `Min` is 0, or `Min > Max` |
| `FeatureManifestSetEmpty` | an empty accepted-manifest set |
| `FeatureManifestSetTooLarge` | more than 64 accepted manifests |
| `FeatureManifestIdMalformed` | a manifest ID failing the manifest grammar |
| `FeatureManifestIdOutOfNamespace` | a manifest ID not prefixed by its own profile's ID |
| `DuplicateFeatureManifestId` | a repeated manifest ID within one entry |
| `MissingVerifier` | no verifier supplied |
| `MissingExecutorFactory` | no executor factory supplied |
| `VerifierIdentityMismatch` | the verifier's declared `ProfileId` or contract version differs from the entry's |
| `LimitDefaultsInvalid` | a defaults or hard-maxima member outside ADR 0007's rules |
| `HostCapabilityDescriptorInvalid` | a capability descriptor failing ADR 0011's shape |
| `DuplicateHostCapabilityId` | two capability descriptors sharing a capability ID |
| `ConformanceIdentityMissing` | conformance identity absent |
| `DiagnosticsIdentityMalformed` | diagnostics identity outside the profile's ID namespace |
| `PackageIdentityMissing` | package identity absent |
| `CatalogTooLarge` | more than 64 entries |
| `BuilderConsumed` | any call on a builder whose `Build` has already succeeded |
| `CoreContractVersionNotYetSupported`, `CoreContractVersionRetired`, `CoreContractBuiltAgainstMismatch`, `CoreContractAuthoredExceedsBuiltAgainst` | the two contract-version integers fail ADR 0003's admission predicate |

**The reason set is open, and that is deliberate.** Adding a member is not a
core contract amendment: it is not one of the contract artefact classes, no
host handles it exhaustively at run time, and every member describes a defect
in first-party composition code. What IS an amendment, and a breaking one, is
routing any of these conditions into an operation-result category.

**An empty catalog is legal.** It is the honest expression of a product that
ships the core alone - which section 15 gate 1 requires such a product to state
- and every verification against it returns `UnsupportedProfile`. That makes
the outcome demonstrable with no profile linked at all, which is invariant 13's
requirement applied to this path. The 64-entry maximum is a shape bound on a
trusted input, not a security bound, and must not be described as one.

**Verifier identity versus executor identity.** Section 3 asks registration to
reject descriptors whose declared identity differs from the produced executor,
but an executor is produced per runtime, after registration, so registration
literally cannot check it. The obligation splits by what exists when:

| Subject | Exists at | Checked at | Outcome |
|---|---|---|---|
| Verifier | composition time | catalog construction | thrown, reason `VerifierIdentityMismatch` |
| Executor | executor creation, per runtime | executor creation | returned, never thrown: `ProfileFault` at the instantiation stage with core reason `ExecutorIdentityMismatch` |

A profile factory producing an executor that declares a different identity is a
profile contract violation observed on an untrusted-input path, which is what
`ProfileFault` is for; throwing there would put an exception on a stage whose
return-value contract says it never throws.

**One condition, one surface.** No condition may be reachable through both the
catalog path and the load path. A contract-version admission failure is only a
catalog failure. An unsupported format version or manifest is only a load
failure, because a descriptor cannot declare an artifact's version. A malformed
profile ID is a catalog failure when declared and a load failure when supplied
by a caller - two different inputs, not one condition on two surfaces.

Rejected: returning a result envelope from `Build`, which adds a core category
for a composition-time defect and invites hosts to continue with a partially
valid catalog. Rejected: collecting all errors and throwing once at `Build`,
which loses the offending call site. Rejected: logging a warning and skipping
an invalid entry - section 14's core/catalog row names silent replacement a
release blocker, and the composition's closure would then differ from its
declaration. Rejected: an option, flag, or environment variable that tolerates,
downgrades, or suppresses an admission failure. Rejected: invoking the executor
factory once at catalog construction to check identity, which creates
per-runtime mutable state at composition time and runs profile code before any
runtime with ceilings exists.

### 9. The identity and version check order on the load path

Verification performs these steps in this exact order. The first failure is
returned and no later step runs. No payload byte is read, hashed, sniffed, or
copied before step 4, and no allocation driven by an untrusted declared count
occurs before it.

| Step | Check | Failure |
|---|---|---|
| 1 | descriptor well-formedness: identity non-empty and grammatical, format version >= 1, exactly one grammatical manifest ID | `InvalidArtifact` / `MalformedArtifactDescriptor` |
| 2 | catalog lookup by ordinal identity equality. Terminal on a miss: no other entry is tried, no decoder is probed, no bytes are touched | `UnsupportedProfile` / `UnsupportedProfile` |
| 3a | format version within the matched entry's inclusive range | `InvalidArtifact` / `UnsupportedProfileFormatVersion` |
| 3b | manifest ID in the matched entry's accepted set, by ordinal equality | `InvalidArtifact` / `UnsupportedFeatureManifest` |
| 4 | effective-ceiling materialization (host ceiling, profile hard maximum, artifact request), owned by ADR 0007 | ADR 0007's |
| 5 | the profile verifier decodes and validates the payload | a verified handle, or `InvalidArtifact` with a profile-owned reason, or `ResourceExhaustion`, or `Cancellation` |

Step 1 precedes step 2 so that a malformed identity is not reported as an
absent profile: a caller passing garbage would otherwise be told the
composition lacks a profile, sending the fix to the wrong owner.

**A known ID with a bad format version or manifest is `InvalidArtifact`, not
`UnsupportedProfile`.** The composition contains the profile, so the mismatch
is a property of the artifact. Section 7 reserves `UnsupportedProfile` for
selecting a profile the composition does not contain; broadening it would tell
a host to link something it already has.

Two consequences are load-bearing. `UnsupportedProfile` and `InvalidArtifact`
can never both be true for one call, so a doubly-wrong input has one
predictable outcome and section 14's stable-failure-class requirement holds
across JIT, trimmed, and Native AOT hosts. And `UnsupportedProfile` is provably
reached without reading the payload, so it can be returned for a zero-length,
truncated, or entirely bogus buffer - exactly the single-profile
misconfiguration case section 7 calls out - and a diagnostic may state
truthfully that no payload byte was examined.

The categories are section 7's and ADR 0005's; the reason codes named here are
core-owned, profile-neutral refinements within a category. The core reason set
may never gain a member that names a language, a profile, or a language
construct - language detail travels in the typed profile payload.

Rejected: aggregating identity, version, and payload failures into one
multi-error result, which requires running later steps on input already known
to be wrong, including the profile decoder on a mismatched version, and makes
the failure class depend on how many things are wrong.

### 10. What `UnsupportedProfile` reports, and to whom

`UnsupportedProfile` means: the requested identity is not in this composition's
catalog. It differs from `InvalidArtifact` in owner (the composition root, not
the artifact producer), in fix (link or name the right profile, not regenerate
the file), and in evidence state (no payload byte was examined).

It reports the category; the reason; the requested identity echoed verbatim,
truncated to the 128-character ID bound and, in any text rendering, with every
byte outside the frozen grammar escaped as `\xNN`, because the requested
identity is untrusted caller or guest data and must never inject control
characters or line breaks into a log or an evidence bundle; the core contract
version; and the catalog identity derived from the canonical encoding, which
names the composition without enumerating it.

**Catalog disclosure is split by surface, and the split is enforced by type
rather than by a flag:**

| Surface | May carry a `VmProfileCatalogListing` |
|---|---|
| caller-initiated verification, where the host called the API | yes: the full listing, in normalized order, per entry - identity, display name, supported format range, accepted manifests, the contract versions, the guest-load, asynchronous-instantiation and external-suspension declarations, conformance identity, diagnostics identity, package ID. The caller authored the catalog, so this leaks nothing |
| guest-initiated loads, where the result travels back through the requesting operation | no. The result carries the category, the reason, and the sanitized requested identity only |

The guest-facing load result type declares no member whose type is a
`VmProfileCatalogListing` or any collection of catalog entries, so the
disclosure cannot be forgotten, mis-defaulted, or re-enabled by configuration.
`VmDiagnostics` (VM-0 decision on paper; no file at VM-0) never carries a
catalog listing on any surface; the listing is
reachable only through an explicit member on `VmCatalog`, an object no profile
and no guest is ever handed. Freeze the invariant: **catalog enumeration is a
host-facing diagnostic and is never a value the core places on a
guest-reachable path.**

It must not report: limit values; host-capability IDs, versions, or signatures;
package versions; file paths, assembly names, or type names; any host secret;
or any statement about why a profile is absent - there is no notion of "present
in another composition". It is terminal for that load: no fallback, no retry
against another entry.

Rejected: always including the listing, as section 7 reads literally, because
on the guest-initiated-load path the result is observed by executing untrusted
code and would hand a guest an enumeration of the image's profiles, versions,
and manifests - a composition-fingerprinting primitive with no benefit to a
host that already knows its own catalog. Rejected: never including it anywhere,
which removes the most useful datum for the misconfiguration case section 7
names; the host is not the adversary here. Rejected: a verbose-diagnostics
flag, which can be enabled in production, differs across JIT, trimmed, and AOT
configurations, and makes the failure class configuration-dependent.

### 11. Immutability, order independence, and the canonical encoding

Order independence means the built catalog and every observable consequence of
it are a pure function of the SET of entries. Mechanically:

| # | Ruling |
|---|---|
| (a) | `Build` normalizes entries to ascending ordinal identity order; declaration order is not retained |
| (b) | every core-produced enumeration - the host-facing listing, diagnostics, support-table rows, closure reports, drift manifests - emits that normalized order |
| (c) | lookup is by identity only. The catalog exposes no indexer, no index-based accessor, no `first`, `default`, `primary`, or `only` entry concept, and no member that behaves differently when the catalog holds one entry |
| (d) | a canonical catalog encoding is the identity oracle, defined below |
| (e) | `VmCatalog` has no mutating member: no `Add`, `Remove`, `Clear`, `Replace`, `AddOrReplace`, overwriting `TryAdd`, and no priority or enabled flag. Entry data is defensively copied at `Add`, so mutating a caller's array afterwards cannot change the catalog. Mutating the builder after `Build` does not affect an already-built catalog, and `Build` may be called more than once, returning catalogs with byte-identical encodings |

**The canonical encoding.** Walk entries in ascending ordinal identity order
and emit, per entry, the fields marked included below in the row order of the
descriptor field table: each string as its bytes preceded by a 4-byte
little-endian length, each integer as 4 bytes little-endian, each declared
boolean or two-valued declaration as one byte `0`/`1`, each set preceded by a
4-byte little-endian count. No field names, no separators, no whitespace, no
culture.

| Included | Excluded, and why |
|---|---|
| identity, display name, `DescriptorRevision`, format range, the normalized manifest set, the artifact representation and lifetime kinds, the guest-load, asynchronous-instantiation and external-suspension declarations, both contract-version integers, conformance identity and version, diagnostics identity, package ID | verifier instances and every delegate - the identity of code is proven by the closure report, not by a fingerprint; `LimitDefaults`, `ProfileHardMaxima`, `BudgetDeclarationMatrix`, `AbandonBudget`, `CancellationPollBound`, `HostCapabilityDescriptors` - host-overridable or tunable policy, whose change is already carried by `DescriptorRevision`, and whose inclusion would churn the drift baseline on tuning; `PackageVersion` - so a version bump alone cannot change catalog identity |

The encoding is an identity oracle and explicitly **not** a cache key. Anything
cache-key-shaped belongs to ADR 0006 and must include effective ceilings and
host-capability inputs itself. The core defines the encoding and its equality
and defines **no hash over it**, so it makes no collision-resistance claim it
cannot verify; a host that wants a short key hashes the encoding itself.

**The proof shape, deferred to VM-1**, is frozen here so the test cannot be
weakened later: a PERMUTATION test building a fixture catalog of at least three
entries, including identities adjacent under ordinal ordering, in all `N!`
`Add` orders for `N <= 6`, asserting byte-identical encodings and identical
categories, reasons, and rendered diagnostics from a fixed script of lookups,
admissions, and failing loads; a MUTATION test mutating the arrays and spans
passed to `Add`, after `Add` and after `Build`, asserting the encoding is
unchanged; a SHAPE test asserting no indexer, no mutator, and no
first/default accessor; and a HOST-STABILITY test asserting the encoding is
byte-identical under JIT, trimmed, and Native AOT hosts.

Rejected: preserving declaration order and merely promising nothing depends on
it, because order would stay observable in listings and encodings and the
property could then only be tested behaviourally. Rejected: defining catalog
identity as a cryptographic hash, which makes a claim this record cannot verify
and invites the fingerprint to be used as an authentication token. Rejected:
rebuilding a catalog with an entry removed or replaced as a composition switch,
because section 5 forbids a run-time option that removes an already rooted
profile - the removed profile is still in the closure, so the claim and the
image would disagree. Rejected: an indexer or an "only profile" shortcut for
single-profile products, which creates a code path whose behaviour changes with
catalog size and makes `UnsupportedProfile` avoidable by accident.

### 12. When a source generator may replace the hand-written catalog

A source generator may replace hand-written catalog construction only when all
six conditions hold, each verified in CI. Failing any one, the hand-maintained
catalog stays. All of this is deferred to VM-3.

| # | Condition |
|---|---|
| 1 | DIRECT CALLS ONLY. The generated code contains only direct constructor and `VmCatalogBuilder.Add` calls naming each descriptor accessor by its C# identifier. It emits no `Assembly.Load`, `Type.GetType`, `Activator.CreateInstance`, `MakeGenericType`, `GetMethod`, `GetType`, `Delegate.CreateDelegate`, `UnsafeAccessor`, or `[ModuleInitializer]`, and creates no ordering dependency between initializers |
| 2 | COMPOSITION-ROOT-SCOPED INPUT. Its input is declarations or a checked-in manifest inside the composition-root project. It must not discover profiles by enumerating referenced assemblies' metadata or scanning attributes across the reference graph |
| 3 | REVIEWABLE, DETERMINISTIC MANIFEST. Alongside the code it emits a text manifest listing every entry's frozen identity fields in normalized order, checked in and diffed in review. Regeneration is byte-deterministic for the same inputs, generator version, and core contract version |
| 4 | AGREEMENT TEST WITH A BYTE ORACLE. A test builds the documented and the generated catalog and asserts their canonical encodings are byte-identical. The checked-in manifest is the oracle; the hand-written catalog may be retired only after the manifest-oracle test is in required CI |
| 5 | SAME VALIDATION PATH. The generated code calls the same public builder API a human would write. It may not construct a frozen catalog directly, use internals, or bypass `Add`/`Build` validation |
| 6 | CLOSURE NEUTRALITY. It is referenced as an analyzer only (`PrivateAssets="all"`, `OutputItemType="Analyzer"`, `ExcludeAssets="runtime"`), appears in no product closure, and every named composition's closure report is byte-identical with and without it |

Condition 2 is the one section 3 leaves open and the one that matters most:
build-time discovery over the reference graph is implicit composition wearing a
compile-time costume. Linking a package would change the catalog without any
composition-root edit, so the closure and the declaration could silently
diverge - the exact failure VM-3's closure gate exists to catch.

**Reflection is never an admissible substitute, permanently.** No core contract
version may admit reflective, name-based, attribute-scanned, directory-scanned,
or module-initializer-ordered registration. Invariant 2 is unconditional and
section 16 lists replacing compile-time extensibility with reflection as a stop
condition. Section 2's amendment procedure may not be used to introduce runtime
discovery: that would change invariant 2, which is a roadmap change requiring
the same review as the roadmap itself, not a numbered contract amendment.

Rule B5: no assembly in the product graph references a dynamic-loading,
reflection-invocation or IL-emit API. Status: Active; witness the
`DynamicLoadingWitness` type compiled into `Broiler.VM.Architecture.Tests`.

Rule B5b: no assembly in the product graph applies `ModuleInitializerAttribute`.
Status: Vacuous at VM-0 - it runs, and nothing in the VM-0 graph can violate
it; it becomes non-vacuous at VM-1 when a product assembly declares a method
that could carry the attribute.

Rejected: a generator that scans referenced assemblies for a marker attribute,
per condition 2. Rejected: shipping the generator as an ordinary package
reference, which puts Roslyn on the product's dependency graph and inside its
closure audit. Rejected: proving agreement by review rather than by test -
review of generated code is the thing that stops happening after the third
regeneration. Rejected: deleting the hand-written catalog when the generator
lands, which removes the oracle before the manifest-oracle test exists, leaving
the agreement test comparing the generator with itself. Rejected: reflection
behind a development-only opt-in switch, which becomes a production path the
first time someone flips it.

## What VM-0 does not prove

Exclusion identifiers are allocated by ADR; this record owns no block, so each
item below is stated in plain words and cites the record that carries an
identifier where one exists.

- **None of this exists in code.** VM-0 freezes names and rules; VM-1
  implements them. No catalog, descriptor, identity type, validation, listing,
  or reason set is present in any product assembly at VM-0, and Rule E5 is what
  keeps that statement true.
- **The reserved namespace is not enforceable against forgery at run time.**
  The catalog check is self-consistency between a reserved ID and its declared
  package ID. Authoritative reservation is VM-3's CI job. No security property
  follows from an ID's namespace. Closed by: VM-3's ID-governance job.
- **The order-independence, mutation, shape, and host-stability proofs do not
  run at VM-0.** Their subject does not exist. They are frozen here as required
  test shapes and are deferred to VM-1.
- **The identity-related checks this record would like from the architecture
  suite are not registered rules at VM-0.** A globalization and
  resource-reference scan, a public-surface assertion against alias-shaped and
  feature-content-shaped member names, and a documentation-conformance test
  over every profile-ID and manifest-ID literal all have subjects that do not
  exist yet. None carries a rule identifier and none has a row in
  `rules.register.json`; registering them is the activating milestone's work
  under the register's own rule, which ADR 0001
  (`0001-component-topology-and-dependency-graph.md`) owns. The count and
  identifiers of every Vacuous and Deferred rule are recorded by ADR 0001 as
  Exclusion EX-05.
- **The four-column support tuple has no published table at VM-0.** ADR 0012
  (`0012-security-ownership-and-support-matrix.md`) records that no public
  support table exists at VM-0 and carries the identifier for it.

## Consequences

- **Section 3's builder sketch is confirmed in shape and renamed in detail.**
  `VmCatalog.CreateBuilder().Add(descriptor).Build()` stands as written. The
  second line of that illustrative snippet, `VmRuntime.Create(catalog)` (VM-0
  decision on paper; no file at VM-0), is superseded by ADR 0004, which owns
  the corrected creation shape; this record neither restates nor competes with
  it. ADR 0003's roadmap-amendment register
  carries the section 3 code block as a Proposed, not applied row. The roadmap
  is not edited.
- **Section 3's registration sentence is narrowed by this record.** "descriptors
  whose declared identity differs from the produced executor" becomes a
  verifier check at catalog construction plus an executor check at executor
  creation, because the executor does not exist at registration. ADR 0003's
  register carries the section 3 row; the roadmap text stands until an owner
  lands it.
- **Section 7's `UnsupportedProfile` sentence is narrowed by this record.**
  "naming the requested ID and the catalog's contents" holds on the host-facing
  surface only; the guest-facing load result type cannot express a listing. ADR
  0003's register carries the section 7 row.
- **Section 3's "alias collisions" now has one meaning.** It is a confusable-ID
  collision under the ASCII fold, not a redirect mechanism, and section 14's
  alias test becomes concrete at VM-1: register `com.example.P`, then
  `com.example.p`, assert `ProfileIdAliasCollision`.
- **The descriptor count "fifteen" is struck** and replaced by the table in
  section 7 plus its amendment rule. Any record that names a descriptor field
  cites that table.
- **A miswired composition fails at the `Add` call** with a named reason and
  field, before any runtime exists, and fails identically in the composition
  root's own tests and in a Native AOT publish-and-run gate.
- **Catalog drift becomes a byte diff** between the canonical encoding and a
  checked-in baseline, which is what section 15 gate 2's "the catalog is static
  and documented" needs to be mechanical. That baseline is deferred to VM-3.
- **A single-profile product exercises the same code path as a two-profile
  one**, so VM-3's "adding a second profile requires no core change" is
  structurally true rather than true by luck.
- **A core-only product is a first-class composition.** The empty catalog is
  legal and returns `UnsupportedProfile` for every load, which is the simplest
  proof of that path and needs no profile to exist - invariant 13 satisfied for
  this axis at the cost of one rule.
- **VM-1 inherits a finite, closed list of work here**: one comparison helper,
  one fold helper, one grammar validator shared by identities and manifest IDs,
  one exception type with one reason set, one canonical encoder, and the four
  frozen test shapes. There is no open-ended validation surface and no second
  string-comparison policy anywhere in the core.
