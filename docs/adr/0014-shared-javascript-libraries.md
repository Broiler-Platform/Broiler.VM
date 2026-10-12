# ADR 0014 - Shared JavaScript Libraries: The Admission Record

**Status:** Proposed

**Date:** 2026-10-11

**Core contract:** not contract-bearing

## Context

**The feasibility report.** [The feasibility report](../javascript-shared-libraries-feasibility.md)
asks whether Broiler.VM's JavaScript profile family and the Broiler.JS engine can stop implementing
the same language algorithms, data and source front end twice. Its answer is a set of libraries that
name neither engine and that both engines consume. Each engine keeps its own value model, object
model, execution machinery, compiler back end, host integration and resource policy.

**The programme and its corrections.** [The shared JavaScript libraries programme](../javascript-shared-libraries/README.md)
turns that report into milestones SJL-0 to SJL-17. [Its review](../javascript-shared-libraries/review.md)
checks every claim the report makes and records the corrections. [Its decision register](../javascript-shared-libraries/decisions.md)
states every decision point, `SJL-D-1` to `SJL-D-25`, with its alternatives. All of them were taken
on 2026-10-11, and this record files the ones milestone SJL-0's work package SJL-0.2 names.

**The inventory.** The responsibilities the programme moves are listed in
[the programme's inventory](../javascript-shared-libraries/inventory.md). Work package SJL-0.1
re-derives every row at named commits of Broiler.VM, Broiler.JS, Broiler.Unicode, Broiler.RegEx and
Broiler.DateTime, and bundle [`sjl-0-001`](../evidence/sjl-0-001/README.md) retains that copy. The
retained copy, not the survey, is the one later gates read.

**Why this record is needed at all.** The programme contradicts written positions of this component.
[The roadmap](../roadmap.md) says Broiler.VM "references no legacy Broiler component, in either
direction", that the JavaScript profile starts from a copy of Broiler.JS, and that "fixes do not flow
across the fork". [ADR 0011](0011-source-level-profile-contract.md) refuses "A shared syntax tree or
parser" permanently. Rules A1, A2, A3 and N1 refuse every shared package today. None of these
positions is changed by implication. This record says what changes, why, and through which records.

**One person holds every role named here.** The core architecture owner, the core-contract owner
(R5), the release owner (R6), the JavaScript profile owner, the Broiler.JS maintainer and the
component maintainers are one person. ADR 0012 records this as EX-30. **Nothing below claims that a
second reader has checked any of it**, and no gate this record names is independently confirmed until
a second holder exists.

**It is not contract-bearing.** It names no lifecycle transition, no result category and no public
type of core contract version 1. It adds nothing to the core's three assemblies, to `Broiler.VM.Ubc`,
to an emitter or to the WebAssembly profile, and it never will. It changes no graph on its date:
**no package is admitted by this record.** Rule A2 still refuses every `Broiler.*` package reference
until milestone SJL-2 revises it.

## Decision 1 - The scope reading of ADR 0011 (decision SJL-D-8)

ADR 0011's extraction gate (G1 to G4) and its standing refusals are written about Broiler.VM
**product profiles**. G1 asks for two product profiles implementing the behaviour, and G4 for no
profile-to-profile edge. Its record clause files a verdict in this ADR set "because it changes the
core graph". The roadmap's section 8 explains the refused shared parser as one that "would encode one
language's grammar into the core's namespace".

**The reading this record takes:**

- **The gate and the standing refusals govern two things:** sharing between Broiler.VM profiles, and
  placement in the core.
- **A dependency of the JavaScript profile family on an engine-neutral library is neither.** No
  Broiler.VM profile shares anything with another profile. Nothing enters the core's namespace, the
  core's assemblies, `Broiler.VM.Ubc`, an emitter, or the WebAssembly profile. The library lives
  outside Broiler.VM, names neither engine, and references nothing in Broiler.VM.
- **So the standing refusals stand unchanged for what they refuse.** A syntax tree or parser shared
  between Broiler.VM profiles, or placed in the core, is still refused permanently. Lexing, source
  positions and diagnostic formatting shared between Broiler.VM profiles still wait for a second
  text front end. The programme creates none of these.

**The route taken, and the alternative not taken.** The reading is taken as a route, filed as
[MVP-16](../mvp.md#5-routes-taken-without-a-decision). The alternative is an amendment of ADR 0011's
decision text, which would mint core contract version 2. The amendment procedure is unexecutable
while one person holds R5 and both co-signing roles (EX-15), so that alternative would leave the
programme waiting on a mint nobody here can perform.

**Who would rule otherwise.** The core-contract owner (R5), with the core architecture owner. If they
rule that the reading is wrong, for example once a second holder exists, milestone SJL-0 is
`Blocked` naming the core as holder, and the programme ends with SJL-0's gate met. The feasibility
report then stands as an assessment, with the review's corrections.

**ADR 0011's decision text is not edited.** ADR 0011 gains an editorial pointer paragraph beside its
standing-refusals table, as it did for ADR 0013, and the table's text is unchanged. The roadmap's
section 8 gains the same kind of pointer paragraph beside its candidate table.

## Decision 2 - The admission policy

**What may be admitted.** A library that:

- references neither engine, and nothing in Broiler.VM;
- follows the contract conventions of
  [the programme's architecture](../javascript-shared-libraries/architecture.md#4-contract-conventions):
  no ambient state, bounded and metered work, no platform globalization or regular-expression API,
  Native AOT and trimming declared and probed, among others;
- is pinned by content hash, in a lock file;
- is named on the allow-list, with its version and the projects that may reference it.

**Where it may be admitted.** In the JavaScript family's projects only:
`Broiler.VM.Profile.JavaScript`, `.Compiler`, `.Format` and `.Intl`, each only as the allow-list
names. **Never** in the core (`Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Runtime`),
in `Broiler.VM.Ubc`, in an emitter, in the WebAssembly profile, or in a composition root directly.
Rules A12 and A13 keep composition roots and consumer profiles free of package references, so a
shared package reaches a root only transitively, and the root's row in the composition register
declares it.

**How the admission is enforced.** By architecture rules minted or revised at milestone SJL-2, each
with a witness watched failing on an injected defect and passing after its revert:

- **new rules `I1` to `I7`:** admission by allow-list and lock-file hash (`I1`), imported-assembly
  hygiene (`I2`), the deployment boundary (`I3`), data agreement (`I4`), retired duplicates (`I5`),
  no leak through the public surface (`I6`), and upstream identity in evidence (`I7`);
- **revised rules A2, B3, N1 and N37:** A2 refuses a `Broiler` package outside `I1`'s allow-list,
  rather than every one; B3 sweeps every shipping assembly against the component's own set plus the
  admitted assemblies; N1 allows the permitted set; N37 allows exactly the allow-list in produced
  nuspecs;
- **rule A7 and `graph.manifest.json`** gain a package-edge field, and rule E4's identifier pattern
  recognises group `I`.

The programme's [governance document](../javascript-shared-libraries/governance.md#31-broilervm)
lists each rule's change and its witnesses. **Until those rules exist, nothing is admitted**: rule A2
as it stands refuses every shared package, and this record does not weaken it.

**Unchanged.** Rules A1, A3 and D1 still forbid project references and linked source outside the
component, and any reference into Broiler.VM. A shared library may never reference Broiler.VM.

## Decision 3 - The fork policy (decision SJL-D-9)

Two current positions conflict with the programme: the roadmap's "fixes do not flow across the fork
in either direction", and the JavaScript profile's invariant 11, "neither side is the other's
upstream". Four parts replace them:

1. **Fixes to a shared area flow through the shared library**, in both directions. The shared library
   is upstream of both engines.
2. **Engine code still does not flow between the engines.** Neither engine is the other's upstream. A
   defect found in one engine's own code is that engine's to fix.
3. **Evidence still does not transfer.** Each repository's gate is met by evidence that repository
   collected. Agreement between the engines is a cross-check and never an oracle. The roadmap's
   section 12 sentences "No evidence transfers." and "The legacy IL/bytecode differential is not a
   core oracle." stay exactly as they are.
4. **No local patches.** A defect in a shared library is never patched inside an engine. An engine may
   pin back to the previous release. A temporary workaround in an adapter needs a dated correction
   entry naming the upstream defect and the release that removes it.

## Decision 4 - Homes and package identity (decisions SJL-D-1 and SJL-D-2)

**Homes (SJL-D-1, (b), component-aligned):**

| Library | Home |
|---|---|
| Language algorithms, Intl algorithms, the frontend, the conformance vectors | The Broiler.JS repository |
| Locale data: Broiler.VM's CLDR generator and its pins ((b-ii)) | The Broiler.JS repository |
| Time data: the time-zone and calendar generators and their pins ((b-iii)) | The Broiler.JS repository |
| Unicode character data and algorithms | Broiler.Unicode, under milestone SJL-7.1's owner conditions; otherwise the Broiler.JS repository, with the record saying why |
| Regular expressions | Broiler.RegEx (package `Broiler.Regex`), fixed to Broiler.VM's requirements (decision `SJL-D-14` (a)) |

`Broiler.Unicode.Cldr.LocaleData` is not extended for this programme. Broiler.DateTime is not a home,
and no milestone changes it. Under decision `SJL-D-6` (c) a data home holds the generators and the
pins, and each engine embeds the tables they generate, so no package carries data.

**Package identity (SJL-D-2, (a)):** a neutral prefix, one package per boundary:
`Broiler.ECMAScript.Algorithms`, `Broiler.ECMAScript.Intl`, `Broiler.ECMAScript.Frontend` and
`Broiler.ECMAScript.Conformance` (test-only). `Broiler.ECMAScript.TimeData` and
`Broiler.ECMAScript.IntlData` are reserved for data packages, which decision `SJL-D-6` (c) does not
publish. No reservation of the prefix on nuget.org is required (revised 2026-10-12; see the revision
at the end of this record). It passes Broiler.JSeal's package gate, which treats `Broiler.JavaScript.*` and
`Broiler.VM.*` as engine families, without a revision of that gate.

## Decision 5 - Versioning, co-hosting and the consumption source (decisions SJL-D-3 and SJL-D-4)

**Versioning (SJL-D-3):**

- **A separate version stream** for the shared packages, with its own solution filter, pack
  invocation and tag prefix in the home repository, apart from the Broiler.JS engine packages.
- **Minimum versions in nuspecs, exact versions in each engine's lock files.** A process hosting both
  engines resolves one version of each shared package, so neither engine may demand an exact one.
- **The compatibility minimum:** semantic versioning, an API baseline, package validation against the
  last published version, and a changelog with an "Observable changes" heading. Binary compatibility
  inside a major version is mandatory. A correction toward the specification is a minor version, and
  so is a data-version change, each with its own changelog entry.
- When a shared major version or data version moves, both engines move in coordinated releases.

**The consumption source (SJL-D-4, (c)):**

- **While an area is in development**, Broiler.VM restores the shared packages from a committed,
  hash-pinned local feed, with package source mapping sending the `Broiler.ECMAScript` prefix to it
  and lock files restored in locked mode.
- **Anything a Broiler.VM package release depends on is on nuget.org first.** `eng/verify-feed.ps1`
  already says so of every dependency outside the release.
- Rule `I1` reads the lock files' content hashes in both cases.

## Decision 6 - Assurance (decision SJL-D-5)

**Full parity, (a).** Each home repository annotates every relevant unit under the Broiler Code
Assurance scheme, generates the artefacts, and gates on them in CI, from a library's first commit and
before any area moves into it. **A Broiler.VM release refuses an admitted package whose upstream
revision lacks a completed review**, read through the source commit rule `I7` records in the bundle.

Broiler.VM's rules J1 to J12 still cover only Broiler.VM's own sources. Code that moves out of
Broiler.VM leaves that coverage, and the bundle of the milestone that moves it defines an exclusion,
numbered at filing time, saying what left and where it is now covered. A unit moved out keeps
`Origin=Ported`, and its human-review state starts again: a moved unit is not a reviewed unit.

## Decision 7 - The pilot decision (decision SJL-D-23)

**The programme continues past the pilot, a choice taken before the pilot ran.** The decision was
planned for the exit of milestone SJL-3, the number-conversion pilot, on the pilot's evidence. The
owner took it on 2026-10-11, before any of that evidence existed. So:

- **SJL-3 is not a point at which the programme can stop.** Its six criteria stay as SJL-3's exit
  clauses, and a criterion that fails keeps SJL-3 `In progress` and stops nothing else.
- **A refusal of the pilot area** ends that area only, as any area's refusal does.
- **Reopening the decision** takes a new dated choice, entered beside this one.

## Decision 8 - What is not planned (decisions SJL-D-18 and SJL-D-17)

- **Whole-runtime unification is not planned** (SJL-D-18). It is recorded as not planned, not as
  deferred. Broiler.JS's public `JSValue` class hierarchy and Broiler.VM's internal tagged `JsValue`
  struct serve different engines on purpose, and the JavaScript profile's JSD-0011 already rejects
  adopting the seed's boxed value hierarchy. The programme shares no value model, object model,
  frame, dispatch loop, compiler back end, emitted format, verifier, resource budget, suspension
  mechanism or host integration, and no built-in object implementation. Milestone SJL-16 confirms
  this, or reopens it only on new evidence.
- **Broiler.JS's non-standard extensions stay in Broiler.JS** (SJL-D-17): the `Decimal` type and `m`
  literals, `Number.prototype.clz`, `String.prototype.contains`, the .NET format-string extension to
  `toLocaleString`, and the SpiderMonkey-style date heuristics with the culture-dependent legacy
  `Date.parse`. The shared libraries implement the specification only. Where an extension needs a
  shared primitive, a parameter of a shared operation provides it.

## Routes this record takes, filed as rows of the MVP register

- **The scope reading of decision 1** is filed as
  [MVP-16](../mvp.md#5-routes-taken-without-a-decision). It names the alternative not taken, what
  would settle the question, and who would decide it.

No other route is taken: decision `SJL-D-4` is recorded here before milestone SJL-2 takes the route it
chooses, so it needs no row.

## Exclusions

This record defines no exclusion identifier.

## Consequences

**Records moved with this record, in Broiler.VM (work packages SJL-0.2 to SJL-0.4):**

- **The ADR index** gains this record. It is the fourth record that is not contract-bearing, beside
  ADR 0001, ADR 0012 and ADR 0013. The contract-bearing set, 0002 through 0011, is unchanged, which
  rule E2 asserts. The index's sentence "All thirteen are `Proposed`" is corrected, with the
  superseded text quoted.
- **[ADR 0003 section 11](0003-core-contract-v1-and-amendments.md)'s roadmap-amendment register**
  gains one row for every roadmap sentence of governance items V1 to V7: the introduction, sections 5,
  8, 9, 12, 14, 15 and 16. Every row is `Proposed`. The rows become `Applied` at milestone SJL-2, when
  the admission they describe exists, and the roadmap's sentences are not edited before then.
- **ADR 0011** and **the roadmap's section 8** each gain an editorial pointer paragraph beside their
  tables, naming this record and the scope reading. Neither table's text changes.
- **[The core ledger](../roadmap.status.md)** carries a dated correction of "Broiler.VM has no
  dependency on that component and none of its milestones wait for it".
- **[The README](../../README.md)**'s section "Relationship to Broiler.JS" carries a dated correction
  with a pointer to the programme.
- **[`docs/mvp.md`](../mvp.md)** gains route MVP-16.
- **The JavaScript profile** files decision JSD-0063 and correction entries JSC-291 to JSC-295
  (SJL-0.4): the admission, invariant 11's revision, the withdrawal of the seed copy rows, the re-scope
  of milestone JS-2 (`SJL-D-7`) and this record's pilot decision. Invariant 12, "No evidence
  transfers", is not touched.

**Records milestone SJL-0 moves in the other repositories, which this record does not file:**

- **Broiler.JS** (SJL-0.5): dated revisions of `docs/architecture/dependencies.md`,
  `docs/public-api.md`, `docs/roadmap/README.md` (whose "The bytecode/VM plan is retired from this
  component" stays), `Assemblies.md`, `Roadmap.md`, `ModernizationDelivery.md` with MOD-M2-7,
  `Modernization.md`'s invariant 3, and `docs/architecture/builtins-vs-clr-types.md` section 6; and the
  new plan and status pair `docs/roadmap/SharedLibraries.md` and `SharedLibraries.status.md`, which
  carry Broiler.JS's part of decisions `SJL-D-1` to `SJL-D-3`, `SJL-D-5`, `SJL-D-6`, `SJL-D-9`,
  `SJL-D-16`, `SJL-D-17` and `SJL-D-23`.
- **Broiler.Unicode** (SJL-0.6): a new `docs/roadmap.md` carrying the SJL-7 requirements and
  decision `SJL-D-6` (c)'s provider.
- **Broiler.RegEx** (SJL-0.6): its `docs/roadmap.md` gains the SJL-11 requirements, as decision
  `SJL-D-14` (a) took them.

**Records later milestones move, named here so none is moved by implication:** ADR 0001's legacy
boundary, its NuGet.config rationale and its parent-props reason (SJL-2); ADR 0012's roles R4 and R6,
which come to cover admitted upstream packages (SJL-2); the rules and register rows of decision 2
(SJL-2); the composition register's shared-assembly column (SJL-3); `THIRD_PARTY_NOTICES.md` (SJL-3
onward). An area's accept or refuse verdict is a JSD in the profile series, with a pointer added to
this section.

**Area verdicts, added as each is filed:**

- **Number conversion (SJL-3.1): accepted**, by the JavaScript profile's
  [JSD-0065](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0065-number-conversion-moves-to-the-shared-algorithms-library.md),
  2026-10-12.

**Nothing is reviewed, accepted, advertised, packed or published by this record**, under
[`docs/mvp.md`](../mvp.md). No package is admitted, no rule is minted and no algorithm moves.

---

### 2026-10-12 - the prefix reservation is withdrawn, SJL-2

**What the record said.** Decision 4's package-identity paragraph said: "The prefix is reserved on
nuget.org before anything under it is published (milestone SJL-2, work package SJL-2.13)."

**What replaced it.** The owner withdrew the reservation, and decision `SJL-D-2` carries the revised
choice beside the one taken on 2026-10-11. No reservation of `Broiler.ECMAScript` on nuget.org is
required, and Broiler.JS's `Publish shared libraries` workflow no longer waits for one. The prefix, the
package identities and every other decision of this record are unchanged. What guards against a
package substituted under the prefix is what already held at SJL-2: package source mapping, lock files
whose content hashes rule `I1` holds to the allow-list's pins, and the upstream commit rule `I7`
records.
