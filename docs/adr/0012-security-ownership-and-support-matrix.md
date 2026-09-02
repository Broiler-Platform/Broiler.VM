# ADR 0012 - Security Ownership, Support Matrix, And Pinned Platform References

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** not contract-bearing

## Context

Two questions decide whether anything else in the VM-0 record can be believed:
who is allowed to say yes, and what may be said in public. Section 13 makes
named ownership VM-0's only dependency. Section 15 gate 1 requires a public
table that names the core contract version, the compositions, the host
capabilities, the guest-load and external-control support, the RIDs, and the
deterministic exclusions separately. Section 16 makes an untruthful support
claim the one stop condition that a difficult or slow milestone does not
excuse, and invariant 7 makes Native AOT support demonstrated rather than
inferred.

No person is assigned to Broiler.VM. The component holds five project shells,
one public type with two integer constants, and an architecture-test suite;
it has run nothing, published nothing, and advertises no composition.

This record therefore settles five things: ownership expressed as roles with
vetoes rather than as people; the consequence of VM-0's dependency line being
unmet; what VM-0 may and may not publish about support; the declared RID
matrix and the discipline that governs when a RID becomes a claim; and the
contents of the VM-0 evidence bundle. It is not contract-bearing - it names no
lifecycle transition, no result category, and no public type of the core
contract, and nothing here can be implemented into a shipping core.

## Decision - ownership is six roles with vetoes

Ownership is defined as roles, each a named responsibility with a defined veto
and a milestone by which it must be filled. A role is not a person and is not
a job title; it is the smallest unit that can hold a veto. The role set is
closed at six.

> **Ownership update: 2026-08-27.** All six roles are held by MaiRat / Maik Ratzmer, the
> component's sole maintainer. The Holder column below and Exclusion EX-30 are written against
> that fact; the role structure, the vetoes and the must-be-filled-by milestones are the decision
> this record makes and are unchanged. One person holding every role means owner and reviewer are
> not independent, which `HUMAN_REVIEW.md` records as a residual risk rather than resolving.

As originally written this record named no person and recorded all six roles as vacant; the
update above supersedes that, and the general rules about what a vacant role does and does not
permit remain in force for any role that falls vacant later.

| Role | Owns | Veto | Must be non-vacant before | Holder |
|---|---|---|---|---|
| **R1 Verification-boundary owner** (section 13's core security owner) | Section 7's load-time requirements, the verification failure taxonomy, the rule that verification produces the only executable input, and the rule that verification stays separable from execution | Any change to a verification entry point, to a failure category, or to the one-verifier property of section 10 | VM-2 is accepted | MaiRat |
| **R2 Host-capability allowlist owner** (R1 jointly with section 13's host-integration owner) | The typed capability surface, the artifact-provider capability as a distinct kind, capability versioning and signature matching, exception translation, and the rule that registering value capabilities never implies a provider | Any addition to the capability shape or to the allowlist | VM-4 is accepted | MaiRat |
| **R3 Fuzz-corpus owner** (R1 unless separately named) | Corpus identity and hashes, seed provenance, minimization, permanent retention of minimized regressions, and the budgets and iteration counts every security bundle records | Retiring a corpus entry | VM-2 is accepted | MaiRat |
| **R4 Vulnerability-response owner** (section 13's release owner, escalating to R1) | Intake, embargo, triage, fix, disclosure, and firing the section 15 recertification trigger | Disclosure timing and the content of a security advisory | Any package or preview is published | MaiRat |
| **R5 Core-contract-version owner** (section 13's architecture owner) | Core contract version 1 and section 2's amendment procedure | Any change to a declared transition or result category - such a change is an amendment by definition - and the minting of any version | VM-0 is accepted | MaiRat |
| **R6 Release and recertification owner** | Section 15's gates, the truthfulness of the public support table when one exists, and section 15's recertification triggers | Any release or preview publish, and any change to a claimed support column | VM-6 is accepted, or the first publish, whichever comes first | MaiRat |

R1 and R2 are separate because the threats differ and are gated at different
milestones: R1 defends against malformed input and is proved at VM-2; R2
defends against ambient authority and is proved at VM-4. Section 7 already
separates the artifact-provider capability from an ordinary import for exactly
that reason, and ownership that mirrors the contract is cheaper to audit than
ownership that flattens it.

Standing rules on the role set:

- **No silent default.** A vacant role does not pass to the architecture
  owner, to the author of a change, or to whoever is available. A role held by
  nobody at the point that requires it stops that point.
- **Separation at a release gate.** One person may not hold both R5 and R1 at
  a release gate: the amendment authority may not also be the sole signer of
  the boundary an amendment would move.
- **No new exit condition.** This record adds nothing to any milestone's
  objective exit gate and proposes no roadmap change. Ledger update rule 7
  already requires an owner and a reviewer to confirm every exit condition;
  this record only fixes *which role* must be the confirming owner, and the
  reviewer who accepts a milestone honours that map.
- **Vacancy is not `Blocked`.** The ledger's vocabulary reserves `Blocked` for
  a named external dependency, and an unfilled internal role is not external.
  A milestone whose work is done and whose required role is vacant is
  `In progress` with the vacancy listed as an open gate condition.

When a person is named, the holder row is added to the ownership table this
record specifies (VM-0 decision on paper; no file at VM-0) in
docs/roadmap.status.md (exists at VM-0: docs/roadmap.status.md). The row
carries every field below; a row missing any of them is not a naming.

| Field | Required value |
|---|---|
| Role identifier | Exactly one of R1..R6 |
| Holder | The person's name |
| Attributable identity | Reviewer alias plus a signature or an attributable commit |
| Date | The date the holder took the role |
| ADR revision | The revision of this record in force at that moment |
| Core contract version | The value of `VmCoreContract.Version` at that moment |
| Covered evidence | The evidence-bundle identifiers the holder's sign-off covers |
| Contact channel | Mandatory for R4, a reachable channel; omitted for other roles |

A change of holder is appended as dated history under ledger update rule 1. It
does not by itself invalidate accepted evidence, but the incoming holder must
record acceptance of every open item attributed to the role, and the ledger
row names those items.

Rejected: a single "security owner" covering R1 through R4, because
malformed-input defence and ambient-authority defence fail differently and are
gated two milestones apart. Rejected: naming ownership at VM-1, because
section 13 lists it as VM-0's dependency and section 16 makes its absence a
stop condition, so deferring moves the stop condition rather than resolving
it. Rejected: recording holders only in this record, because an ADR is a dated
decision and holders change, while the ledger is the authority for current
state.

## Decision - VM-0's dependency line is unmet, and nothing downstream may start

Section 13's VM-0 Dependencies line reads "Named ownership for the core
contract and its amendments". R5 is held by MaiRat, so that line is satisfied; the
milestone is still not accepted, because acceptance additionally requires a
recorded review decision, which no one has made.
This record states the consequences in terms, because the tempting failure
here is to redefine "named ownership" downward:

1. VM-0's ledger state is `In progress`. It is not `Accepted`, and it is not
   `Blocked`.
2. `In progress` is not acceptance for any purpose. It does not satisfy VM-1's
   "After VM-0 acceptance" precondition, and it satisfies no other milestone's
   dependency on VM-0.
3. No dependent milestone may leave `Not started` while VM-0 is not
   `Accepted`. Ledger update rule 3 forbids inferring completion transitively,
   and starting VM-1 against an unaccepted VM-0 is exactly that inference.
4. No VM-0 result may be cited as evidence in another milestone's row.
5. The VM-0 ledger row lists, as open gate conditions: the unrecorded review
   decision and the absence of an independent reviewer;
   the unmet Dependencies line; the count and identifiers of every rule in
   rules.register.json (exists at VM-0:
   src/tests/Broiler.VM.Architecture.Tests/rules.register.json) that is
   Vacuous or Deferred; the conditionality of the inbound half of the legacy
   boundary recorded by ADR 0001
   (`0001-component-topology-and-dependency-graph.md`); the roadmap-amendment
   register that ADR 0003 (`0003-core-contract-v1-and-amendments.md`) proposes
   and does not apply; and the absent SDK pin that ADR 0001 records.

A third ledger state - `Awaiting ownership`, for a milestone whose objective
exit conditions all have accepted evidence and whose required role is vacant -
was proposed and is **not** introduced. Adding a state changes the ledger's own
contract, and its definition would assert that every objective exit condition
has accepted evidence, which VM-0 cannot claim: several exit-gate clauses close
on paper decisions rather than on collected evidence. `In progress` already
requires the row to link its working evidence and list every open gate
condition, which is precisely what is true here. The proposal is surfaced to
the user as a recommendation and is written into no document.

Nothing mechanical enforces the role-to-gate map. The component has no
gate-check script and no CI workflow, and this record does not imply one
exists; see EX-38.

## Decision - VM-0 publishes no support table, and says so

No public support table exists at VM-0. Section 15 gate 1 is a release gate,
Broiler.VM has published nothing, and the future public table, docs/support.md
(deferred to VM-6), has no subject: the advertised composition set is empty, no
RID is claimed, and no capability is implemented. Writing a table now would
produce rows whose only honest content is "none", and a table of "none" is
read as a support claim.

The record VM-0 genuinely owes - each admitted-but-unimplemented artefact
decided explicitly, "even where the first release ships no implementation" -
lives as the five-column admitted-versus-implemented table in ADR 0003
(`0003-core-contract-v1-and-amendments.md`), which also fixes the two forms in
which invariant 8 may be discharged. This record does not restate that table
or that rule; it cites them, and adds the two things the table cannot carry.

**First: gate 1 publishes two integers, not one.** A release states what it
*implements* and what it *accepts*, and collapsing them hides a rejection. The
public table names `VmCoreContract.Version` (exists at VM-0:
src/Broiler.VM.Abstractions/VmCoreContract.cs), the version the release
implements, and `VmCoreContract.MinimumSupportedVersion`, the floor of the
authored window the release accepts from a profile descriptor. At contract
version 1 both are 1 and the window is a single point, which is exactly when
one integer looks sufficient and is not. A descriptor whose authored version
falls outside the window is refused deterministically at catalog construction
by the failure ADR 0002 (`0002-profile-identity-and-static-catalog.md`) names
`UnsupportedCoreContractVersion`, carried by `VmCatalogValidationException`;
the table names that failure beside the two integers.

**Second: the standing exclusions a first release publishes.** These are the
artefacts a caller might expect and will not get. Each is decided by the
record named beside it; this table is a directory, not a second decision, and
every row below is a paper decision at VM-0 with no implementation anywhere.

| Artefact | Decided by | What a first release does when it is requested |
|---|---|---|
| Persisted envelope | ADR 0010 (`0010-embedding-decisions.md`) | No envelope member exists in the public API baseline; recorded there as EX-25 |
| Incremental (streaming) verification | ADR 0010 | No member exists by which it can be requested |
| Lazy per-section verification | ADR 0010 | No member exists by which it can be requested |
| In-process producer input (compile straight to a verified handle) | ADR 0010 | No member exists by which it can be requested; the mandatory byte round trip is the shipped behaviour |
| Guest-initiated load with no registered provider | ADR 0008 (`0008-guest-initiated-loads.md`) | Deterministic refusal, `ArtifactProviderNotRegistered` |
| External suspension not declared, or not enabled per runtime | ADR 0009 (`0009-external-suspension-and-async-instantiation.md`) | `ExternalSuspensionNotDeclared` / `ExternalSuspensionNotEnabled` |
| Asynchronous instantiation the descriptor does not declare | ADR 0009 | `UndeclaredAsynchronousInstantiation` |
| A wall-clock bound on cancellation latency | ADR 0004 (`0004-lifecycle-and-state-machine.md`) | Contract version 1 promises none; the bound is stated in profile work units |
| Deployment on a RID this record excludes | This record | Nothing is published for it. A RID is not something a caller can request at run time, so there is no outcome to return |

The last row corrects a tempting shortcut. An excluded RID is **not** an
"unsupported deployment mode" outcome: the core's outcome set is closed by ADR
0005 (`0005-operation-result-envelope.md`), a RID is a property of a published
artifact rather than of an operation, and minting a tenth outcome to describe
a packaging fact would put a language-free lifecycle enum in the business of
describing shipping decisions. The truthful discharge of invariant 8 for an
excluded RID is that no package exists for it and the support table names the
exclusion.

Recommended schema for the public table when VM-6 writes it (deferred to
VM-6): one row per artefact, with columns *artefact identifier*, *admitted by
core contract version n*, *implemented in core release n*, *provided by this
composition* (one column per advertised composition), and - mandatory whenever
implemented is false - the deterministic failure identifier or the exclusion
identifier. The per-composition column is absent from ADR 0003's VM-0 table
because there is no advertised composition to fill it, and that record carries
the exclusion for its absence.

Rejected: creating docs/support.md at VM-0 with empty rows. An empty published
table is a support claim of a particularly bad kind - it looks maintained.
Rejected: one "supported" column with footnotes for the unimplemented rows,
because gate 1 requires exclusions stated separately and a footnote is the
untruthful support claim section 16 stops for.

## Decision - the declared RID matrix, and what "claimed" costs

The declared RID matrix for Broiler.VM core compositions is
{ win-x64, linux-x64, linux-arm64 } - two at VM-0 (a decision on paper; no file
at VM-0) and the third added on 2026-09-01 by the revision at the end of this
record. Declaring a RID means the component intends to collect publish-and-run
evidence for it and will accept a recertification trigger when it changes. It is
not a claim.

| RID | Status at core contract version 1 | Basis |
|---|---|---|
| win-x64 | Declared, not claimed | A publish lane for it already exists in the surrounding repository, so the evidence is collectible rather than hypothetical |
| linux-x64 | Declared, not claimed | As above |
| linux-arm64 | Declared, not claimed - **added 2026-09-01** | **Declared for the architecture and not for a consumer**, which is what makes it different from the two rows above and is why it is spelled out. The surrounding repository publishes no arm64 head, and its Android head - the one that ships `android-arm64` - runs Mono and cannot be reached from here at all. This row is the only way the component exercises arm64 code generation and arm64 Native AOT, and hosted arm64 Linux runners make that evidence collectible rather than hypothetical, which is the same test the two rows above pass. **It stands for no other RID**: a green result here says nothing about Mono, about the Android runtime, about a head that turns trimming off, or about macOS |
| win-arm64, osx-x64, osx-arm64 | Reserved - no evidence | The name may appear in a composition register marked reserved. It may never appear in a support table |
| android-arm64, android-x64 | Excluded pending pinned Native AOT platform references | Broiler.VM holds no evidence that ILCompiler Native AOT targets an Android RID, and the three Microsoft pages section 17 requires are unpinned: docs/platform-references.md (VM-0 decision on paper; no file at VM-0) is not created. See EX-32 |
| browser-wasm and every wasm RID | Excluded | Terminology freeze, below |

The grounding for choosing the first two is that the surrounding repository
already publishes those two RIDs and no other on a lane that runs on both host
operating systems; the grounding for the third is in its own row and is a
different kind of reason. Both are observed state in another component, not
Broiler.VM evidence - the ledger's section 1 forbids treating another
component's work as evidence here, and nothing above is offered as such. They
explain only why declaring four or six RIDs would create a matrix with no
lane on which any future claim could ever be collected.

**Terminology freeze.** In Broiler.VM, "WebAssembly" always names a guest VM
profile - a bytecode language the core may host - and never a host RID or a
deployment target. No core project, package, namespace, type, or RID string
may use the token to mean a host target. The freeze exists because a
`Broiler.VM.Profile.WebAssembly` in a support table beside a RID column is one
copy-edit away from reading as a wasm deployment claim.

**Claim discipline.** A RID becomes *claimed* only when an evidence bundle
exists that published and ran the named subject on it, with trim and AOT
warnings treated as errors. The earliest milestone at which each claim is
collectible follows from the gates section 13 already states; the table adds
no gate and changes none.

| Milestone | What may become claimed | Kind of claim |
|---|---|---|
| VM-0 | Nothing | No RID is claimed and no composition is advertised |
| VM-1 | win-x64, linux-x64 for the trimmed and Native AOT fixture construction host | A test-host claim, recorded as such, never as a composition claim |
| VM-2 | Nothing new | Its gate asks that failure categories stay stable on the already-claimed hosts |
| VM-3 | win-x64, linux-x64 for each named composition, publish-and-run, with its exact-closure report | A composition claim |
| VM-6 | Every advertised composition on every declared RID | The full advertised matrix, with the suppressions inventory |

At VM-0 the component sets **no** AOT or trimming property at all: no project
declares `PublishAot`, `IsAotCompatible`, or `IsTrimmable`, so no trim or AOT
analyzer runs and not even the analyzer *input* that invariant 7 classifies as
insufficient exists. Setting `IsAotCompatible` on the three product projects
is recommended at VM-1, when there is code for an analyzer to look at; it is
recorded here so no reader mistakes its absence for an oversight and no future
reader mistakes its presence for a claim.

Where the per-composition register eventually lives - one row per composition
with its profile descriptor set, declared RIDs, claimed RIDs, evidence-bundle
identifier, and milestone - is docs/compositions.md (deferred to VM-3), the
same file ADR 0001 names as the future source of the composition-root
allow-list. VM-0 creates no such file and no composition root, so the matrix
above lives only in this record.

Rejected: declaring the Android RIDs to match the surrounding repository's
full publish surface, because that head is built and packaged through the
Android workload rather than through ILCompiler and no Broiler.VM evidence
exists that Native AOT targets it - a support claim with no lane is what
section 16 stops for. Rejected: declaring no matrix and letting VM-3 discover
one, because section 13's VM-0 exit gate names RIDs among the things this
record must fix and section 15 treats a RID-matrix change as a recertification
trigger, which presupposes a recorded matrix. Rejected: claiming the two
declared RIDs at VM-0 on a clean build, because invariant 7 names analyzer
success and a trimmed build as inputs and ledger update rule 4 forbids
promoting them.

## Decision - the contents of the VM-0 evidence bundle

The ledger requires every status beyond `Not started` to point at a retained
bundle. This record fixes what the VM-0 bundle (exists at VM-0:
docs/evidence/vm-0/) records, field by field, so that "an acyclic shell graph
builds" is a reviewable fact rather than an assertion.

| Field | What the VM-0 bundle records |
|---|---|
| **Identity** | Milestone VM-0; the roadmap revision; core contract version 1; the bundle identifier; the collection timestamp; the owner; the reviewer, who at VM-0 is the same person |
| **Source** | The component commit, the dirty-tree state and patch identity, and the five project paths ADR 0001 prints (exists at VM-0: the three project files under src/ and the two under src/tests/) |
| **Dependencies and corpus** | The SDK and runtime versions actually used (SDK 10.0.400 at this collection) with the explicit note that no pin enforces them - ADR 0001 owns that exclusion - and the SHA-256 of eng/Broiler.Packaging.props (exists at VM-0: eng/Broiler.Packaging.props) as vendored, `82b186ff0d5c54ca6951eb519344970c53d7b4b880445591852885911261db03`, with its source path. No fixture corpus exists |
| **Environment** | OS, architecture, and configuration of each machine used, and the operating systems actually built on. No RID, no trimming mode, and no Native AOT mode is recorded, because none was exercised |
| **Procedure** | The exact commands and working directories: `dotnet build Broiler.VM.slnx -c Release`, `dotnet test Broiler.VM.slnx -c Release`, and `dotnet pack Broiler.VM.slnx -c Release`, run from the component root (exists at VM-0: Broiler.VM.slnx) |
| **Outputs** | The retained build, test and pack logs; the pack log showing exactly three .nupkg and three .snupkg; and the architecture-test result with its per-rule status |
| **Decision** | The expected gate, quoted from section 13 as the *planned* gate and never as the result; the actual result; and the exclusions by identifier, with the subset that most limits the bundle spelled out (below) |
| **Validity** | Reproduction instructions, and the recertification triggers that expire this bundle: a change to the SDK, to the core contract version, to the public API, or to the package graph |

The **claim** the Decision field states, and the only claim the bundle makes,
is this sentence: *an acyclic five-project shell graph builds; every forbidden
edge in the VM-0 shell graph is expressed and witnessed; nine rules await
their subject and are registered in rules.register.json.* Those nine are the
six Vacuous rows (B2, B3, B4, B5b, B6, B7) and the three Deferred rows (C1,
C2, C3) of the register; ADR 0001 owns the exclusion that names them and the
status vocabulary they use.

The Decision field additionally carries, by identifier, the exclusions minted
across the twelve records - EX-01..EX-08 from ADR 0001, EX-10..EX-17 from ADR
0003, EX-20..EX-24 from ADR 0009, EX-25..EX-28 from ADR 0010, and EX-30..EX-38
from this record, thirty-four in all. The identifier space is sparse: EX-09,
EX-18, EX-19 and EX-29 are minted nowhere and must not be cited. The sentence
that defines each exclusion lives in the record that mints it; the Decision
field reproduces none of them in full, spelling out in summary form only the
subset that most limits the bundle. The field also states in words that no
capability is proven, no composition is claimed, and no RID is claimed.

## Exclusions

Each exclusion below is defined here and only here; everywhere else it is
cited by its identifier. The VM-0 bundle's Decision field and the VM-0 ledger
row carry exclusions by identifier, each spelling out in summary form only the
subset that bears on it; neither reproduces all thirty-four sentences.

- Exclusion EX-30: VM-0 satisfies section 13's dependency line - all six roles
  are held - but it is not accepted, and the owner and reviewer required by
  ledger update rule 7 are the same person, so the confirmation is not
  independent. Reason: Broiler.VM has a single maintainer. Closed by: a signed
  decision in HUMAN_REVIEW.md and its ledger row; the independence gap closes
  only if a second maintainer is named.
- Exclusion EX-31: no public support table exists at VM-0; section 15 gate 1
  is unmet. Reason: the public table is VM-6's deliverable and VM-0 advertises
  no composition, claims no RID, and implements no capability for it to
  describe. Closed by: VM-6.
- Exclusion EX-32: docs/platform-references.md does not exist at VM-0; the
  three Microsoft Native AOT pages required by section 17 are unpinned and the
  Android exclusion is therefore provisional, not final. Reason: retrieving,
  hashing and archiving a third-party page is a human action, and section 17's
  own requirement that VM-0 record immutable revisions is not met by a
  retrieval nobody performed. Closed by: a human retrieving and pinning them.
- Exclusion EX-33: no RID is claimed at VM-0 and no trimmed or Native AOT
  publish-and-run evidence exists; no project in the component sets
  `PublishAot`, `IsAotCompatible` or `IsTrimmable`, so not even an analyzer
  input exists. Reason: invariant 7 makes Native AOT demonstrated rather than
  inferred, and a project shell demonstrates nothing. Closed by: VM-1 for the
  trimmed and Native AOT fixture construction host, and VM-3 for each named
  composition.
- Exclusion EX-34: VM-0 evidence is collected on the operating systems its
  Environment field names and on no others; where only one was run, no
  cross-platform result follows from it. Reason: VM-0 wires no CI, so
  collection happens on the machines the author had. Closed by: a second
  collection on the other declared host, recommended at VM-1.
- Exclusion EX-35: the component publishes no security contact and no
  vulnerability-intake channel at VM-0. Reason: R4 is held but no channel is
  published, and an intake
  channel with no owner is worse than none. Closed by: naming R4 with a
  reachable contact before the first publish, as section 15 gate 7 requires.
- Exclusion EX-36: no composition is advertised at VM-0 and no per-composition
  register exists, so the declared RID matrix carries no evidence-bundle
  identifier and no closure report. Reason: VM-0 creates no composition root.
  Closed by: VM-3.
- Exclusion EX-37: nothing tests the RID sets or the WebAssembly terminology
  freeze at VM-0; both are prose, and rules.register.json carries no row for
  either. Reason: their subjects - a support table, a composition register,
  package metadata - do not exist yet, and a rule may not be registered
  against an absent subject with an activation milestone at or before VM-0.
  Closed by: VM-6, when the packaging and documentation checks are wired and
  the register gains the rows.
- Exclusion EX-38: nothing enforces the role-to-gate map at VM-0; the
  component has no gate-check script and no CI workflow, so the map binds the
  reviewer who accepts a milestone rather than a machine. Reason: a component
  CI workflow is VM-6's. Closed by: VM-6.

## Consequences

- VM-0 can be authored, built and proved now, and its acceptance waits on
  exactly one named person: R5. Nothing else about ownership blocks work, and
  nothing about the vacancy is hidden - it is EX-30 in the ledger row, in the
  bundle, and here.
- Every security bundle at VM-2 and VM-4 has a pre-identified signer, so the
  sign-off is not invented on acceptance day; and a publish is blocked while
  R4 is named, which is what section 15 gate 7 asks for.
- The honest answer to "does Broiler.VM run on Android" is that no Broiler.VM
  package exists for any Android RID and the exclusion is provisional on
  references nobody has pinned. The answer to "is it Native AOT ready" is that
  the component has never been published or run under Native AOT and sets no
  AOT property.
- Adding a RID later is one composition-register row plus an evidence bundle,
  and it automatically fires a section 15 recertification trigger.
- This record supersedes no illustrative roadmap snippet and proposes no
  roadmap edit of its own. It does leave one visible discrepancy: section 17
  says VM-0 records immutable revisions for the platform references, and VM-0
  does not. That is recorded as EX-32 rather than repaired, because SCOPE
  forbids re-authoring the roadmap; if ADR 0003's amendment register carries a
  section 17 row, it carries it as `Proposed`, not applied.
- Three items are surfaced to the user as recommendations and are written into
  no document: retrieve and pin the three Native AOT pages, then create
  docs/platform-references.md and lift or confirm the Android exclusion; name
  R5 so VM-0 can be accepted, and R4 before any publish; and consider adding
  an `Awaiting ownership` state to the ledger vocabulary if a later milestone
  ever reaches "all evidence collected, owner vacant" - VM-0 has not.
- What this record forbids, in one place: naming a RID as supported anywhere
  without an evidence bundle that published and ran it; deriving an AOT claim
  from a property, an analyzer, or a non-AOT publish; using "wasm" or
  "WebAssembly" for a host target; publishing a support table before there is
  something true to put in it; recording a role holder without the ADR
  revision, contract version, and covered bundle identifiers; treating a
  vacant role as delegated to whoever is available; and treating VM-0's
  `In progress` state as acceptance for any downstream purpose.

---

### 2026-09-01 - the lane's RID matrix is brought back to this one, and what the consuming repository's Android head leaves open

**What the record said.** That the declared RID matrix is exactly
{ win-x64, linux-x64 }; that win-arm64, linux-arm64, osx-x64 and osx-arm64 are
*Reserved - no evidence*, whose own rule is that the name "may never appear in a
support table"; and that android-arm64 and android-x64 are excluded because
"Broiler.VM holds no evidence that ILCompiler Native AOT targets an Android RID"
and the platform references are unpinned.

**What was actually true.** The matrix above was right and nothing enforced it.
`.github/workflows/broiler-vm.yml` published and ran every composition root on a
third RID - `osx-arm64` - and `docs/support.md` gave that RID a row of its own in
section 3. Neither is a claim, and both were drift in the direction of one: a
reserved RID with a lane behind it and a table row in front of it is one edit
away from being read as supported, which is the reading this record's own
forbidding list exists to prevent.

**What replaced it.** The lane's matrix is the declared matrix, and the support
table's section 3 carries a row per declared RID and none for a reserved one.
**The grounding sentence for choosing two did not have to change**, which is the
part worth stating: it says the surrounding repository publishes those two RIDs
and no other, and it still does - `Broiler.Browser.Windows` publishes `win-x64`
under `Release-Windows` and `Broiler.Browser.Linux` publishes `linux-x64` under
`Release-Linux`, and there is no macOS head. The matrix was already aligned with
the consumer; the lane was not.

**What the Android head changes, and it is the reason above the exclusion.**
The consuming repository has a third head - `net10.0-android36.0`, ABIs
`android-arm64;android-x64`, `PublishTrimmed=false` - which its CI builds with
the Android workload. That head does not use Native AOT and does not run
CoreCLR: it runs Mono. So the reason this record gives for excluding the Android
RIDs - no evidence that ILCompiler Native AOT targets one - is a true sentence
about a question the consumer does not ask. **The real gap is narrower and
harder**: this component has no Android-targeted project and no device or
emulator harness, so "publish and run" on an Android RID means an application
package and a device, not a matrix row. The RIDs stay **Excluded**; the reason
is now the one that would have to be answered to lift it, and EX-32's pinned
references are a second condition rather than the only one.

**And one gap this leaves named rather than covered.** The product ships arm64 -
it is the Android head's primary ABI - and after this change no lane in this
component exercises arm64 at all. The withdrawn `osx-arm64` job was the only one
that ever did, and it covered the architecture and nothing else: not Mono, not
the Android runtime, not the head. **Substituting one reserved RID for another -
`linux-arm64` for `osx-arm64` - was considered and refused**, because a proxy
that lets a reader conclude "arm64 is covered" is the same substitution this
component corrects elsewhere, and because it would put the lane back outside the
declared matrix on the day it was brought inside it. Widening the matrix to
arm64 is a decision with a grounding of its own, and it is not taken here.

**What it does not settle.** No RID claim moves in either direction, because no
claim ever rested on the lane: a lane collects no bundle. `linux-x64` remains
the one RID with retained collections behind it, `win-x64` remains attempted and
unclaimed, and everything else remains unattempted. Withdrawing a lane entry
removes a publish nobody consumed; it removes no evidence, because there was
none to remove.

**What is not edited.** The decision section above stands as written, matrix and
rejections intact. This entry is where a reader learns that the lane spent two
days wider than the matrix it was meant to implement, and what the Android head
does to the exclusion's reasoning.

---

### 2026-09-01 - linux-arm64 is declared, and what a green job on it may not be read as

**What the record said.** The revision above, written earlier the same day: that
the declared matrix is { win-x64, linux-x64 }, that `linux-arm64` is *Reserved -
no evidence*, and - in its closing paragraph - that substituting `linux-arm64`
for the withdrawn `osx-arm64` "was considered and refused", leaving the arm64 gap
"named rather than covered", with the note that widening the matrix to arm64
"is a decision with a grounding of its own, and it is not taken here."

**What replaced it.** The decision is taken here. `linux-arm64` moves out of the
reserved row and into the declared matrix, which is three RIDs, and the lane
publishes and runs every composition root on it. The earlier entry was right that
this needs a grounding rather than a swap, and wrong to leave the gap open once
one exists: **the product ships arm64 and nothing in this component compiled a
line for it.**

**The grounding, and it is not the one the other two rows have.** Those two are
declared because a consumer publishes them. Nothing publishes `linux-arm64` - the
surrounding repository has no arm64 head, and the head that does ship arm64 ships
it as `android-arm64` on Mono, which this component cannot target. So this row is
declared for **the architecture**: it is the only way the component exercises
arm64 code generation and arm64 Native AOT, and hosted arm64 Linux runners make
that evidence collectible rather than hypothetical - the same test the other two
rows pass, met by a different route.

**What a green job on it may not be read as, stated because the objection that
delayed this decision was a good one.** It is not Android coverage. It shares an
instruction set with `android-arm64` and nothing else: not the runtime - Mono,
not CoreCLR - not the trimming configuration, which that head turns off, not the
packaging, and not the head. A reader who concludes from this row that the
Android head is exercised has made exactly the substitution the earlier revision
refused, and the reason that refusal was right is that a proxy nobody labelled
would have carried the same false reading with none of this paragraph attached to
it. The row is declared **with its limit written into the matrix**, which is the
difference between a stated scope and a silent proxy.

**What it does not settle.** No RID claim moves. Declaring is not claiming, and
this row is the definition's own case: the component now intends to collect
publish-and-run evidence for `linux-arm64` and accepts a recertification trigger
when it changes. `linux-x64` remains the one RID with retained collections behind
it; `win-x64` and now `linux-arm64` are attempted in a lane that retains nothing.
The Android exclusion is untouched and its reason is the one the revision above
corrected: no Android-targeted project and no device or emulator harness.

**What is not edited.** Both revisions above stand as written. The first recorded
the withdrawal of a reserved RID from the lane; the second recorded that widening
to arm64 was not taken that day. This entry is where a reader learns it was taken
the same day, on a decision made after the objection was heard, and what the
objection bought - the limit above, which the matrix now carries.

