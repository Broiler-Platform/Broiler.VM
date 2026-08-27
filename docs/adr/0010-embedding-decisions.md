# ADR 0010 - Embedding: Byte Round-Trip, Lazy Sections, And Incremental Verification

**Status:** Proposed
**Date:** 2026-08-27
**Core contract:** version 1 (contract-bearing)

## Context

Section 11 hands VM-0 three questions, and section 13's exit gate requires each
to be recorded with the reasoning that settled it. Section 6 leaves a fourth
open with the words "If persistence is approved". All four are invisible until a
host with a latency budget arrives, and all four are expensive to retrofit:
each one either changes what a verified artifact means or changes the load
stage's input contract.

Nothing exists to measure. There is no profile, no compiler, no host, and no
verification-throughput baseline (deferred to VM-5). This record therefore
settles all four on the direction of the amendment each would later need rather
than on numbers, and says so wherever a number would have been the better
argument.

Everything decided here is paper. The product graph at VM-0 exports exactly one
public type, `VmCoreContract` (exists at VM-0:
src/Broiler.VM.Abstractions/VmCoreContract.cs), whose only members are the two
contract-version constants. There is no verification entry point, no artifact
handle, no envelope member and no input type in the checkout. Every type,
member, parameter set, table and field list below is therefore (VM-0 decision on
paper; no file at VM-0) unless its own marker says otherwise.

## Decision

Four rulings, all conservative, all in the same direction: version 1 admits the
narrowest surface that can be widened additively afterwards.

| # | Question | Ruling in core contract version 1 | Pre-recorded amendment |
|---|---|---|---|
| 1 | May locally produced bytecode skip serialization (section 11, decision 1)? | No. The byte round trip is mandatory; bytes are the only input from which a verified artifact may be produced. | Candidate amendment 3 - in-process producer input form |
| 2 | May verification be lazy per section (section 11, decision 2)? | No. Verification is whole-artifact and eager; a handle means the whole artifact was verified. | Candidate amendment 4 - lazy per-section verification |
| 3 | May an artifact be verified incrementally as it arrives (section 11, decision 3)? | No. Excluded from version 1; the input is whole and complete when verification is called. | Candidate amendment 2 - streaming verification |
| 4 | Is the optional persisted envelope approved (section 6)? | Approved as contract, not as a release feature. Release 1 exposes no envelope member. | None. It needs a gate, not an amendment. |

The candidate-amendment register that carries shapes 2, 3 and 4 belongs to
ADR 0003 (`0003-core-contract-v1-and-amendments.md`) (VM-0 decision on paper; no
file at VM-0). Registering a shape records what an amendment would have to
supply. It is not approval, not a schedule, and not a claim that anyone intends
to mint one.

## Decision 1 - The byte round trip is mandatory

Core contract version 1 accepts exactly one verification input form: a complete,
caller-owned `ReadOnlySpan<byte>`. The verification entry point's parameter set
is the descriptor, that byte range, and a cancellation token, and nothing else
(ADR 0006, `0006-verified-artifact-ownership.md`, owns the entry point and
requirement V-SEP). There is no compile-directly-to-verified-handle path in the
core, in `Broiler.VM.Abstractions` (exists at VM-0:
src/Broiler.VM.Abstractions/Broiler.VM.Abstractions.csproj), or in any profile's
public surface.
`VmVerifiedArtifact` is sealed, has no public constructor, and its only
construction site is inside the core's verification type.

The handle carries no producer discriminator. The record field an earlier
proposal called `ProducedBy`, ranging over bytes and in-process lowering, is
struck and does not exist; a discriminator is the seam along which a second
producer grows, and ADR 0006's non-compared field list no longer carries it.

Two consequences are part of the ruling rather than commentary. Decoding in
place stays legal: invariant 3 permits full decoding, the representation is the
profile's declared choice (ADR 0006), and what is forbidden is producing a
handle from anything other than a byte sequence the verifier read. And
guest-driven loads are bytes-only by section 6's own wording - the
artifact-provider capability answers with a descriptor and bytes exactly as a
caller would, and ADR 0008 (`0008-guest-initiated-loads.md`) owns that mediator.

### Why this beat admitting the direct path

1. **Direction of amendment.** Forbidding and later admitting is additive under
   ADR 0003's test; admitting and later withdrawing is not. With no consumer to
   validate a design against, the branch that can be reversed cheaply is the
   only defensible one.
2. **No consumer.** Section 8's extraction gate - open a fast or shared path
   when real code demands it, from measurement rather than anticipation -
   applies to the core's own surface as much as to shared profile code.
3. **The saving is smaller than it looks, and this is what actually decides
   it.** Section 11 makes the code cache the persisted envelope, keyed by source
   identity, compiler version and format version, so a browser serializes the
   artifact anyway to populate its cache. A direct path would save only the
   re-decode on the first, cache-missing run of each script - the same run that
   pays the compile cost, where a decode is a small fraction.
4. **A second producer of executable handles is the drift vector section 16
   names.** It is not literally a second verifier, but the historical failure is
   that a fast path and a slow path diverge under optimization pressure
   precisely because the fast path exists to be fast. The core also cannot fuzz
   or differentially test an input form it may not inspect.

| Alternative | Why rejected |
|---|---|
| Admit a compile-directly-to-verified-handle path in version 1 | Buys unmeasured latency at the cost of a second handle producer inside the security boundary, with no consumer and no numbers. If it is right, VM-5 shows it and an additive amendment adds it. |
| Admit it only for producers inside the same trust boundary, flagged trusted | A trust flag is the defect's exact shape: it creates a mode the byte path does not take, so the two paths stop running the same checks, and "trusted" becomes a property somebody eventually sets wrongly. Section 7 opens by stating that bytecode is untrusted input even when a local tool produced it. |
| Require literal re-serialization even where a profile could decode in place | Over-reads invariant 3, which explicitly permits full decoding. Real cost, no security benefit. |
| Defer the decision to VM-5 so numbers can settle it | Section 11 and VM-0's exit gate require the decision now. VM-5 measures whether a profile should use such a path, not whether the contract admits one. |

### No invariant was amended, and that is the evidence

This decision required **no amendment to invariant 3**, and no VM-0 record
amends any section 2 invariant. Invariant 3's own final two sentences already
place guest-obtained bytes and nested handles in scope: bytes a profile obtains
while executing become their own verified handle before anything in them runs.
The absence of a required amendment is itself the evidence that the conservative
branch was the compatible one - the alternative could not even be stated without
first rewriting invariant 3's scope. The in-process producer path, with its
same-verifier and differential-corpus conditions, is recorded unimplemented as
candidate amendment 3 in ADR 0003, carrying the note that minting it would be
the amendment invariant 3 does not currently need.

### The reopening trigger, predeclared so it is a measurement

Reopening is a numbered core contract amendment, and its trigger is fixed now so
that it is a number and not an argument: VM-5's verification-throughput baseline
(deferred to VM-5), taken on a browser-shaped workload of many small artifacts
on both JIT and Native AOT, showing that serialize plus verify plus decode
consumes more than a predeclared share of end-to-end source-to-first-execution
time for an in-process producer. VM-5 must include that lane and predeclare the
share, or the trigger is unusable. No such measurement exists today; see
Exclusion EX-26.

Should the amendment ever be minted, its obligations are fixed here so the later
work is mechanical rather than a renegotiation.

| Obligation | Content |
|---|---|
| Same entry point | The direct path calls the same profile verifier through the same core entry point, with no mode, flag or trust hint the byte path cannot also take. |
| No unverified path | Every structural, type, stack, control-flow and bound check runs identically; the producer hands the executor nothing the verifier has not accepted. |
| Indistinguishable identity | The handle binds the seven identity components ADR 0006 freezes, unchanged. The producer is not one of them and never enters the sharing predicate. |
| Differential evidence | Over a source corpus and the malformed corpus: equal handles under the declared identity comparison, identical execution results, identical failure taxonomies. |
| Deterministic lowering | Section 10's property. Without it the differential obligation cannot even be stated. |

## Decision 2 - Verification is whole-artifact and eager

A verified handle means every part of the artifact has been verified. Version 1
declares no verification-mode parameter and no `VerificationMode` enum; the
`Complete`/`DeferBodies` pair an earlier proposal introduced is struck, as is
the `EffectiveSectionVerificationMode` member it would have recorded on the
handle. The handle exposes no partial-verification, pending-section or
continuation member.

The mechanically testable consequence, which is the part an implementer must not
reopen: `InvalidArtifact` is legal only at the three input-consuming stages -
persisted-envelope preprocessing, caller-driven load/verification, and the
guest-initiated load - and is illegal at instantiation, invocation and resume.
ADR 0005 (`0005-operation-result-envelope.md`) owns that matrix and its negative
rules; this record supplies the reason they hold.

| Work a profile defers | Admitted | Why |
|---|---|---|
| Dispatch tables, quickening, lowering to a faster internal form, other representation building | Yes | It cannot fail a verification check, and the core owns no profile's internal representation (invariant 4, section 4). Deferring work that cannot fail costs the contract nothing. |
| Any structural, type, stack, control-flow, index or bound check | No | Its failure is a verification failure, and a verification failure after the load stage has no legal category. |
| Reporting a late check failure as `ProfileFault` | No | It misreports a malformed artifact as a language fault, so a malformed artifact becomes indistinguishable from a language error and VM-2's malformed corpus silently stops testing what it claims. |

Three roadmap commitments point the same way. Invariant 3 makes the handle the
proof - only that handle may be instantiated or executed - and the cheapest
proof that nothing unverified ever runs is that the handle cannot exist until
everything is verified. Section 11's own closing subsection requires
verification to stay separable from execution: a lazily verified artifact cannot
be fully verified without executing it, so the embedder that validates a cached
artifact before trusting it would have no way to do so, and the feature would
destroy the property recorded in the same section (ADR 0006's requirement
V-SEP). And invariant 9 with section 7's ceiling model computes effective limits
and charges verifier work at the load stage; laziness moves an unbounded
fraction of that work into invocation budgets, making per-invocation budgets
depend on which operation happens to enter a body first, and it interacts with
guest-load charging in a way nothing has specified (ADR 0007,
`0007-resource-authority-and-budgets.md`, owns the dimensions such work would be
charged against).

| Alternative | Why rejected |
|---|---|
| Admit lazy per-section verification in version 1 | Breaks the separability property recorded in the same roadmap section, moves verifier work into execution budgets with no charging rule, and adds a partially verified handle kind for a consumer that does not exist. |
| Admit a `DeferBodies` hint with an eager skeleton, recording the effective mode on the handle | Release 1 would implement complete verification only, so the mode would be contract surface with no implementer, no gate demanding it and no named deterministic failure, which ADR 0003's admitted-versus-implemented rule forbids. A deferred body failure also necessarily arrives after a handle exists, where no stage row admits `InvalidArtifact`. |
| Admit it as a profile-private behaviour with no core contract change | The dangerous version: the profile could report a late verification failure only as a language fault, which is exactly the misreporting the table above forbids. |
| Forbid all deferred work inside a profile, including representation building | Over-reaches. The core does not own a profile's internal representation, and work that cannot fail a check is not the core's business. |

If a profile later needs lazy sections, the amendment must supply all of the
following, so that it cannot be smuggled in as an implementation detail. It is
not additive: it changes what a handle means and what a stage may return.

| The amendment must supply | Content |
|---|---|
| A definition of an independently verified section | A unit whose verification result depends on nothing outside its own bytes plus already eagerly verified whole-artifact declarations - section table, type, import and export declarations, cross-references and all bounds - such that no property established for the whole artifact can be invalidated by an unverified section. |
| An explicit partially verified handle kind | Version 1 forbids one, so the amendment adds it in the open or not at all. |
| A per-section verification transition | Declared in the state machine ADR 0004 (`0004-lifecycle-and-state-machine.md`) freezes. |
| A result distinguishable from a language fault | A late section failure may never be reported as a profile fault. |
| A charging rule | Deferred verifier work charged to the invocation that triggers it, against ADR 0007's dimensions. |

## Decision 3 - Incremental verification is excluded

The verification input is whole and complete at the moment verification is
called. Version 1 has no streaming, chunked, incremental or resumable
verification and no asynchronous verification entry point. No member on the
verification path returns `Task`, `ValueTask` or `IAsyncEnumerable<T>`, declares
an async modifier, or accepts a `System.IO.Stream`, a `PipeReader`, a chunk
sequence or a continuation token. Version 1 declares no byte-source capability
kind: the artifact provider is the only non-value capability kind, and ADR 0008
owns it.

Section 11 states the position and its cost, and nothing in the analysis
overturns it. Invariant 3 requires the handle to own a snapshot or a full decode
of its input, and a stream has no snapshot at verification start - which is
exactly the caller-buffer-mutation class whose mutation, disposal and concurrent
overwrite tests are release blockers. Section 7's load-time requirement that
effective limits be computed before reading or allocating from an untrusted
declared count cannot be met from a declared total length that arrives inside
the stream. A streaming API would also make the core a consumer of a host-driven
byte source, which is new host-capability surface, and section 11's division of
labour keeps the core out of fetching entirely. Finally the decisions are
coupled: the span-only input shape chosen in decision 1 to hold invariant 3
cannot cross an await, and recording that dependency now stops the two
decisions drifting apart.

| Alternative | Why rejected |
|---|---|
| Admit streaming verification in version 1 | It needs a byte-source capability kind, a state machine for a partly fed input, an incremental ceiling model and abandonment semantics, all designed against no profile and no measurement - the largest speculative surface on the table. |
| Admit it but ship no implementation, as the envelope does | The envelope is already embedded in section 7's lifecycle and categories, so freezing it prevents a later change to an existing stage. Streaming is embedded nowhere, so freezing it would add surface with no consumer and no retrofit hazard to avoid. |
| Admit a hidden streaming path used only internally by a profile | A profile cannot construct a handle at all (decision 1), and a hidden path is an unverifiable second entry into the security boundary. |
| Declare streaming permanently out of scope | Untruthful about a real need: section 9's WebAssembly profile lives in an ecosystem where streaming compilation is standard practice. Recording it as the expected amendment is honest; foreclosing it is not. |
| Say nothing and leave it open | VM-0's exit gate requires the decision recorded with its reasoning, and silence is what section 16's critical-path row exists to prevent. |

Four things are frozen now so that the amendment stays as small as possible.

1. The signature is closed and stated - descriptor, complete byte range,
   cancellation token - so an incremental form cannot arrive as a quiet
   widening of an existing parameter.
2. Every invariant a streaming implementation would have to satisfy is already
   frozen: effective limits materialised before anything is read; no allocation
   from an untrusted declared count before it clears its bound; identity bound
   only at completion; the failure taxonomy of ADR 0006; and no partial state
   escaping a failed verification.
3. That taxonomy already contains the invalid-artifact reason `Truncated`, which
   is the class a stream ending early must return, so the amendment needs no new
   reason.
4. The handle exists only on success, so no partially verified handle can ever
   be observable. That is prohibited in advance, not merely unimplemented.

| Element of the pre-recorded shape | Frozen content |
|---|---|
| Opening | An incremental verification operation opened from the same descriptor, with effective ceilings fixed at open from the host/profile/artifact intersection exactly as today. |
| Feeding | Bounded chunk append, with verifier work charged as each chunk arrives against ADR 0007's `VerifierWork` allowance. |
| Abandonment | Ordinary disposal. No partial state escapes and no handle is produced. |
| Byte source | A host capability of a declared kind, so the core still fetches nothing itself. |
| Completion | A finish step yielding the same handle type, with identity indistinguishable from the whole-bytes path, proven by a differential test over the same corpora. |

Recording that shape is not an admission: version 1 creates no API, no state and
no capability kind for it. Three things are named non-additive in advance and
are refused even inside an amendment - a streaming path that yields a usable
handle before the whole artifact is verified, one that lets any instruction
execute before completion, and one that changes the load stage's categories.
Because the amendment changes the load stage's input contract it is not additive
in the source-compatible sense, so minting it triggers the status ledger's
update rule 5: accepted evidence is re-evaluated and each record is marked
recertified, re-collected or superseded.

## Decision 4 - The persisted envelope is contract, not a release feature

Core contract version 1 **approves the envelope as contract and as nothing
else**. It freezes the stage, the ownership split, the outer-header field list,
the re-verification rule and the failure mapping. Release 1 exposes **no
envelope member**: no opening member, no envelope type and no schema constant
appears in the public API baseline, and version 1 mints no public name for the
opening operation because no member exists to name. No release before a named
persistence gate ships an envelope reader or writer, publishes an outer schema
version as stable, or offers byte compatibility.

That absence is the invariant 8 discharge. ADR 0003 narrows the
admitted-versus-implemented rule to two forms - a named deterministic failure
the shipping core returns, or absence from the public API baseline recorded as a
named deterministic exclusion - and the envelope takes the second. A type that
exists and throws is the shape-only stub invariant 8 rejects; absence is the
honest and testable form of "not implemented". The exclusion is EX-25 below.

Excluding the envelope from the contract was the alternative, and it fails:
section 7's lifecycle step 3 and its category mapping already mention envelope
loading, and section 4's checklist already lists "Persist and invalidate" as a
core obligation. Removing it and adding it back later would change an existing
stage and the meaning of an existing category, which section 2 classifies as the
kind of amendment that may not stay source-compatible. Freezing the contract and
shipping nothing is the same pattern section 6 applies to guest-initiated loads,
and section 11 makes the eventual consumer concrete by naming the code cache as
the persisted envelope, so this is not speculative surface. Freezing only the
ownership split and deferring the field list also fails: the field list is what
determines whether later addition is additive, and section 6 already enumerates
it, so freezing costs one paragraph.

**Stage.** Envelope loading is a bounded preprocessing step of section 7's
lifecycle step 3 - never a stage of its own in the sense of an independent
lifecycle position, and never an alternative to verification. Its paper shape is
an opening operation taking a complete byte range and yielding an artifact
descriptor plus a payload span, followed by the ordinary verification call.
There is and will be no member that returns a verified handle from envelope
bytes. ADR 0005 carries the preprocessing row in its stage matrix as reserved:
admitted, not implemented.

**Ownership split.**

| Owner | Owns |
|---|---|
| Core | The bounded outer header, profile dispatch, byte ownership, atomic storage and replacement, corruption reporting, and outer-schema compatibility. It treats the profile section as opaque. |
| Profile | Its payload, its semantic cache-key contribution, compiler and debug metadata, migration, invalidation, and composition-specific fallback. |

**Outer header, frozen in this order.** The rightmost column binds this list to
the persisted-envelope key set that ADR 0006 defines; this record does not
restate that set.

| # | Field | In ADR 0006's persisted-envelope key set |
|---|---|---|
| 1 | Envelope magic, fixed 8 bytes | No - framing |
| 2 | Outer schema version. 0 is reserved, never emitted and always invalid, so zero-filled or truncated storage is rejected; the first published schema is 1 | No - core-owned outer compatibility |
| 3 | Profile descriptor identity: the profile ID and the descriptor revision | Yes |
| 4 | Profile format version, as the verifier accepted it | Yes |
| 5 | Feature-manifest identity | Yes |
| 6 | Core contract version | Yes |
| 7 | Verifier semantic version, which is section 6's engine semantic and cache version | Yes |
| 8 | The profile's declared hard maxima | Yes |
| 9 | Artifact content hash | Yes |
| 10 | Per-import capability tuples | Yes |
| 11 | Payload length and per-section lengths, each with a configured upper bound | No - bounded framing |
| 12 | Canonical source or module identity, supplied by the host | No - the host's own lookup key; the core records and echoes it and compares it never |
| 13 | Checksum algorithm identifier, corruption checksum, and the byte range it covers | No - corruption detection |
| 14 | Atomic replacement state | No - storage state |
| 15 | Optional debug-metadata descriptor, whose positions the profile validates | No |
| 16 | The opaque profile section begins here | Not a header field |

Two rules bind that table. Every member of ADR 0006's key set appears in the
header, and every header field that is not a key-set member is framing,
host-owned lookup data or storage state and contributes to no key. And the
header carries **no effective ceilings, no handle instance identity, no runtime
or aggregate-budget identity, and no remainder-derived quantity** - which is how
invariant 5's closing sentence, that persisted artifacts contain no warmed state
or process-local identities, stays true by construction rather than by review.
The envelope also never persists object references, delegates, intern-table
indexes, warmed caches, quickened authoritative opcodes, or host handles.

**Eligibility.** Persistence is available only where the profile declares itself
persistable and supplies a payload writer; the core never synthesises a payload
from a handle. A handle whose `VmArtifactOrigin` is `GuestInitiated` is
ineligible for any persisted envelope and may contribute to no persisted cache
key, because its ceilings snapshot the requesting operation's remainder. That
rule is asserted at the single origin gate ADR 0008 owns, extended to a
persistence gate, and not at a second place.

**Re-verification.** Loading always re-verifies both the envelope and the
profile payload through the ordinary verifier. Outer-schema compatibility never
implies payload compatibility, and interpreting old bytes under new semantics is
prohibited, so a mismatch is a deterministic refusal rather than a best-effort
read; the host's response, to recompile, is a host decision and never a silent
core fallback. There is no trusted-cache mode and no skip-verification
parameter, and adding one is refused under section 16's stop conditions.
Effective ceilings are recomputed at load against the loading runtime, which is
the other half of why they are not in the key: a persisted artifact never
carries a ceiling decision forward.

**The checksum is not authenticity.** It detects accidental corruption only. The
core performs no signature check and makes no trust decision about provenance. A
host accepting artifacts from outside its trust boundary binds an approved hash,
signature or distribution identity before calling the core, and verification
remains mandatory regardless.

**Failure mapping, by ownership of the failing fact.** The rule is stated once:
classify by who owns the fact that failed, not by which step observed it. This
is what resolves section 7's own tension between its envelope sentence, which
sends every envelope failure to `InvalidArtifact`, and its unsupported-profile
sentence, which insists that a profile the composition lacks is never a corrupt
file. Reasons are ADR 0006's; this record mints none.

| Failing fact | Owner | Category | Reason |
|---|---|---|---|
| Envelope magic absent or wrong | The bytes | Invalid artifact | `MalformedEncoding` |
| Outer schema version 0, unknown, or with no migration path | The bytes | Invalid artifact | `UnknownFormatVersion` |
| Header declares a length outside its configured header bound | The bytes | Invalid artifact | `InconsistentStructure` |
| Fewer bytes present than the header declares | The bytes | Invalid artifact | `Truncated` |
| Corruption checksum disagrees over its recorded range | The bytes | Invalid artifact | `MalformedEncoding` |
| Replacement state records an incomplete, torn write | The bytes | Invalid artifact | `InconsistentStructure` |
| Recorded identity contradicts the caller's descriptor or the payload's own identity, including a stale contract or verifier version | The bytes, against the composition | Invalid artifact | `DescriptorMismatch` |
| The envelope names a profile the composition does not contain | The composition | Unsupported profile | `ProfileNotInCatalog` |
| The extracted payload exceeds the artifact-bytes ceiling | The bytes, against the runtime's resource authority | Resource exhaustion | The ordinary ceiling failure at verification, against ADR 0007's `ArtifactBytes` |

The header is core-owned bounded framing, so failing its own bounds is a
property of the bytes; the payload's size against the runtime's authority is the
ordinary verification ceiling and is not an envelope case at all. A composition
that has not declared persistence has no member to call, so its refusal is
structural in exactly the way content-policy-by-omission is structural in
ADR 0008. Version 1 mints no reason name for a not-composed case: ADR 0006's
reason set is closed, and adding to it is governed by ADR 0003's additive test.

**No milestone defines the persistence gate.** Section 13's VM-2 next action
says "bounded outer-envelope parsing where approved", and the word "approved"
has no referent, because VM-0 through VM-6 contain no persistence gate. VM-0
adds no milestone. The envelope reader and writer are therefore deferred to a
gate the roadmap does not contain (Exclusion EX-25), and Consequences carries
the recommendation that would create one.

## What this record forbids

| Forbidden | Because |
|---|---|
| Any public or internal member named or shaped as compile-to-handle, build-handle, trusted-handle, or `*FromSource`, that produces a verified artifact without the verifier reading bytes | Decision 1. Bytes are the only input, and the handle has one construction site. |
| A verifier mode, flag or trust hint reachable from one handle-producing path and not the other | It creates a mode the byte path does not take, so the paths stop running the same checks. |
| A profile type that can construct or mutate a verified artifact | The handle is the proof; a profile may not manufacture proof. |
| A producer discriminator on the handle, in identity, or in the sharing predicate | It is the seam a second producer grows along. |
| A verified handle representing a partially verified artifact, in this version or in any future amendment | Decision 2 and invariant 3. |
| Deferring any check whose failure would be a verification failure | Decision 2. |
| Reporting a verification failure after the load stage, under any category | It misreports a malformed artifact as a language fault and hollows out the malformed corpus. |
| A second entry point, or a distinct complete-verification API | Section 11 designs no second, tool-shaped API; verifying without running is an ordinary use of the one entry point. |
| A streaming, chunked, incremental, resumable or asynchronous verification entry point in version 1 | Decision 3. |
| A byte-source capability kind in version 1 | The core never fetches anything. |
| Executing any instruction from an artifact whose verification has not completed | Invariant 3. |
| Shipping an envelope reader or writer, publishing a stable outer schema version, or offering byte compatibility, before a named persistence gate | Decision 4 and section 16's internal-formats row. |
| Any member returning a verified handle from envelope bytes | The envelope is preprocessing, never a substitute for verification. |
| Any trusted, skip-verification or pre-verified load mode | Loading always re-verifies. |
| Treating outer-schema compatibility as payload compatibility, or the checksum as authenticity | Section 6, and section 14's persistence row names both as blocking failures. |
| Persisting object references, delegates, intern-table indexes, process-local identities, warmed caches, quickened opcodes or host handles | Invariant 5. |

## Exclusions

Each sentence below appears identically here, in the Decision field of the VM-0
evidence bundle (exists at VM-0: docs/evidence/vm-0/; ADR 0012,
`0012-security-ownership-and-support-matrix.md`, fixes its contents), and in the
VM-0 ledger row.

Exclusion EX-25: persistence is admitted by contract version 1 and implemented
by no release; no envelope member exists in the public API baseline; no
milestone currently defines the persistence gate. Reason: section 16 permits
promising persistence only after its explicit gate, and VM-0 through VM-6
contain no such gate. Closed by: a persistence gate the roadmap does not yet
contain.

Exclusion EX-26: VM-0 records no measurement for or against the byte round trip,
the eager-verification rule, or the streaming exclusion. Reason: VM-5 has not
run and no profile, compiler or host exists to measure, so all three decisions
rest on the direction of amendment rather than on numbers. Closed by: VM-5's
verification-throughput baseline, which must include the browser-shaped
many-small-artifacts lane and a predeclared threshold, or decision 1's reopening
trigger is unusable.

Exclusion EX-27: none of the prohibitions in ADR 0010 is asserted by an
architecture test at VM-0. Reason: the product graph contains no verification
entry point, input type, verified-artifact handle or envelope member for a test
to inspect, so rules.register.json (exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/rules.register.json) holds no row for
them. Closed by: VM-1 and VM-2, which create the members these prohibitions
bound; the milestone that creates a subject mints its rule.

What holds the line meanwhile is a surface rule, not a semantic one. Rule E5:
The product graph exports exactly one public type, `Broiler.VM.VmCoreContract`,
whose only members are the two contract-version constants. Status: Active; its
witness is the assertion itself, which the register records as failing if any
product assembly exports a second type. E5 makes the prohibitions above
unviolatable at VM-0; it does not test them, and a passing suite is not evidence
that the contract they express has been implemented.

Exclusion EX-28: nothing checks that this record's outer-header field list stays
in step with the persisted-envelope key set of ADR 0006. Reason: both are paper
at VM-0, and a drift test needs two artefacts to compare. Closed by: the
persistence gate of EX-25, which owns that drift test.

## Consequences

- **Release 1 gives a browser no code cache.** Section 11's sentence that the
  code cache is the persisted envelope describes a benefit no release before the
  persistence gate delivers. A host that needs it caches source-to-artifact
  bytes itself and re-verifies on every load, which is the contract's intended
  behaviour rather than a workaround.
- **Every path pays serialize, verify and decode.** A browser's caller-driven
  top-level and deferred scripts lower to bytes and verify them; `eval`, the
  function constructor, dynamic import and module dependencies go through the
  artifact-provider capability, which returns a descriptor and bytes. This
  record states that cost plainly rather than leaving it to be discovered.
- **A host that wants one entry point of a large bundle pays for the bundle.**
  The mitigations need no contract change: split the bundle into several
  artifacts, or, after the persistence gate exists, cache the verified envelope
  so the cost is paid once.
- **A host receiving an artifact over the network buffers it fully before
  verifying.** ADR 0007's `ArtifactBytes` ceiling is therefore also that host's
  memory bound and must be sized with that in mind.
- **Illustrative roadmap text this record supersedes**, recorded here and not
  edited there: section 6's conditional "If persistence is approved" is now
  answered, as approved contract and not as a release feature; section 7's
  lifecycle step 3 clause about profile bytes extracted from a bounded
  persisted envelope describes admitted surface that release 1 does not expose;
  and section 11's code-cache sentence describes a benefit release 1 does not
  deliver. ADR 0003's
  roadmap-amendment register carries the corresponding rows. Every row there is
  proposed and not applied, so the roadmap and this record disagree in those
  places until an owner lands the patch - ADR 0003's Exclusion EX-11 states
  that. This record edits no roadmap text and amends no invariant, no milestone
  gate and no delivery order.
- **Obligations this record places on later milestones**, stated as obligations
  and not as gate text: VM-2 must show that a malformed artifact fails before
  execution and that no member produces a handle without the verifier reading
  bytes; VM-5 must include the many-small-artifacts lane and predeclare the
  reopening threshold. Adding either to a milestone's exit gate requires a
  roadmap amendment that VM-0 proposes and does not apply.
- **Recommendation to the user, not a roadmap change: an optional VM-2b
  Persistence milestone.** It would be entered only when a named consumer
  requires a persisted code cache. Proposed owners: R1, the
  verification-boundary owner, jointly with R6, the release and recertification
  owner - both roles are vacant at VM-0, and ADR 0012 assigns neither of them to
  this gate, because no such milestone exists. Proposed exit conditions:
  section 14's persistence-ownership row; a published outer envelope schema
  version;
  atomic-replacement and torn-write evidence; a re-verification-on-load proof;
  and a cache-key completeness proof against ADR 0006's key set. This
  recommendation is surfaced to the user and is never written into the roadmap.
- **Candidate amendments 2, 3 and 4 are registered in ADR 0003** with the shapes
  pre-recorded above. The register records shape, not intent: none of the three
  is approved, scheduled or owned, and the first amendment the procedure is
  likely to meet is number 2.
