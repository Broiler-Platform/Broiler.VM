# Governance: repositories, records, rules, change flow, review, security and licensing

**Date:** 2026-10-11. **Status:** plan. This document lists every record and rule each repository
must change, how a change to a shared library flows between repositories, and the obligations on
review, security and licensing. It files nothing: each record named here is a deliverable of the
milestone in its row. Quoted sentences are short extracts of the current text, quoted so a reader
can find them. Where a quoted sentence is long, only its operative part is quoted.

---

## 1. Repositories and owners

| Repository | Role in the programme | Owner roles on this date |
|---|---|---|
| **Broiler.VM** (`D:\Broiler.VM`) | Consumer: its JavaScript profile family adopts the shared libraries. Hosts this programme's roadmap and ledger, and collects Broiler.VM-side evidence. | The six roles of ADR 0012 (R1 verification boundary, R2 host-capability allowlist, R3 fuzz corpus, R4 vulnerability response, R5 core-contract version owner, R6 release and recertification), the core architecture owner and the JavaScript profile owner. **All held by one person**, which ADR 0012 records as EX-30. |
| **Broiler.JS** (`D:\Broiler.JS`) | Home of the language-algorithms and Intl libraries, the frontend, the conformance vectors, and the time-data and locale-data generators with their pins (decision `SJL-D-1`). Also a consumer: its own engine adopts them and stays the public reference engine. | The component maintainer and reviewer; the same person. |
| **Broiler.Unicode** (`D:\Broiler.Unicode`) | Home of Unicode character data and algorithms, under SJL-7.1's owner conditions. Published today as `Broiler.Unicode.Properties`, `Broiler.Unicode.Emoji.StringProperties` and `Broiler.Unicode.Cldr.LocaleData`, all at preview.3. Its CLDR package is not the programme's locale data (decision `SJL-D-1` (b-ii)). | The component maintainer. The repository has no `docs/`, no roadmap, no identifier series and no assurance annotations. |
| **Broiler.RegEx** (`D:\Broiler.RegEx`, package `Broiler.Regex`) | Home of the regular-expression engine, fixed to Broiler.VM's requirements (decision `SJL-D-14` (a)). Published at preview.4; Broiler.JS already consumes it. | The component maintainer. |
| **Broiler.DateTime** (`D:\Broiler.DateTime`) | Not a home in this programme: decision `SJL-D-1` placed time data in the Broiler.JS repository. No consumer anywhere today, and no milestone changes it. | The component maintainer. |
| **Broiler.JSeal** (`D:\Broiler.JSeal`) | Downstream. Its providers consume both engines' packages, and its package gate constrains shared package names; the neutral prefix of decision `SJL-D-2` passes it. Its cross-engine embedding tests are a cross-check. | The component maintainer. |
| **Broiler.HtmlBridge**, **Broiler.Browser** | Downstream co-hosts of both engines. They are where NuGet's single-version resolution matters (see [architecture.md](architecture.md#63-a-process-that-hosts-both-engines)). | Their maintainers. |
| **Aggregate** (`D:\Broiler`) | Pins every component as a submodule. Its release gate requires the submodule pointers to "contain every required commit", with no out-of-tree patches. | The repository owner. |

**One person holds every role named above.** Every record this programme files therefore states
that the owner and the co-signer, or the owner and the reviewer, are the same person. This is the
non-independence ADR 0012 records as EX-30. No gate here is independently confirmed until a second
holder exists.

---

## 2. Positions this programme reverses or narrows

The programme contradicts written positions in four repositories. **None of them is changed by
implication.** Each is changed by the record named in its row, in the milestone named in its row,
with the superseded text quoted beside the new text.

### 2.1 Broiler.VM core documents

| # | Where | What it says now | What the programme needs | Record | Milestone |
|---|---|---|---|---|---|
| V1 | `docs/roadmap.md`, introduction | "the legacy `Broiler.JS` component, which Broiler.VM does not depend on" | Broiler.VM depends on no Broiler.JS **engine** package. Its JavaScript profile family may depend on the shared libraries ADR 0014 admits, some of which are built in the Broiler.JS repository. | ADR 0003 §11 register row; dated inline revision | Filed `Proposed` at SJL-0; `Applied` at SJL-2 |
| V2 | `docs/roadmap.md` §5 | "Broiler.VM references no legacy Broiler component, in either direction." | No edge to a legacy Broiler **engine** component in either direction. Admitted shared libraries are the only edges allowed outside the component, and only from the JavaScript family. | Register row | SJL-0 / SJL-2 |
| V3 | `docs/roadmap.md` §8, "Share mechanism. Never share semantics." and the candidate rows on lexing and on "A shared syntax tree or parser" | The extraction gate and its refusals govern sharing **between Broiler.VM profiles and into the core**. | A **scope reading**: an admitted external dependency of one profile family is not an extraction between profiles. No profile shares anything with another profile, and the core gains nothing. | ADR 0014 records the reading. Route row `MVP-` in `docs/mvp.md` §5. An editorial pointer paragraph beside the §8 table, as ADR 0013 placed beside ADR 0011's table. | SJL-0 |
| V4 | `docs/roadmap.md` §9, "Seeding" | "started from a copy of the legacy Broiler.JS component"; "No project reference, package reference, or shared source link runs between Broiler.VM and any legacy Broiler component in either direction"; "fixes do not flow across the fork in either direction after the snapshot" | The seed copy is not taken (decision `SJL-D-7`). Package references to admitted shared libraries are allowed. **Fixes to a shared area flow through the shared library.** Engine code still does not flow between the engines. | Register rows | SJL-0 / SJL-2 |
| V5 | `docs/roadmap.md` §12 | "Broiler.VM does not depend on it, extend it, wrap it, or replace it"; "No evidence transfers."; "The legacy IL/bytecode differential is not a core oracle."; "Copying is allowed; depending is not." | The first and last sentences narrow to **the engine**. The two middle sentences stay exactly as they are: evidence still does not transfer, and a differential is still a cross-check. | Register rows | SJL-0 / SJL-2 |
| V6 | `docs/roadmap.md` §14, dependency-architecture row; §15 gate 2 | "no edge to a legacy Broiler component in either direction"; "no edge reaches a legacy Broiler component" | "no edge to a legacy Broiler engine component; every edge to an admitted shared library is declared, pinned and witnessed" | Register rows | SJL-0 / SJL-2 |
| V7 | `docs/roadmap.md` §16, risk rows on legacy code | "Legacy code is copied into a profile and quietly becomes a dependency … state that fixes do not flow across the fork."; "Section 12 is a gate, not a preference." | The first row is rewritten for admitted libraries. Section 12 stays a gate. New rows are added: upstream defects, version drift, co-hosting (see [risks.md](risks.md)). | Register rows | SJL-0 / SJL-2 |
| V8 | `docs/roadmap.status.md` §1 | "Broiler.VM has no dependency on that component and none of its milestones wait for it." | Dated correction: this programme's milestones wait on libraries built in the Broiler.JS repository. **No core milestone waits on them.** | Dated correction in the ledger | SJL-0 |
| V9 | `README.md`, "Relationship to Broiler.JS" | "Broiler.VM does not depend on it … The JavaScript profile is expected to begin from a snapshot copy of that component" | Dated correction, with a pointer to this programme | Dated correction | SJL-0 |
| V10 | `docs/adr/0001-component-topology-and-dependency-graph.md`, legacy boundary | "OUTBOUND - Broiler.VM references no legacy Broiler component. Enforced … by Rule A1, Rule A2, Rule A3 and Rule B3, and at pack level by Rule C2" | The boundary keeps every Broiler **engine** package out. An allow-list, held by rule `I1` (proposed), admits the named shared packages into named projects. | Dated ADR 0001 revision stating what the section said before | SJL-2 |
| V11 | ADR 0001, the NuGet.config rationale | "Adding a Broiler-serving package feed would make a PackageReference to a legacy Broiler package RESOLVABLE. Omitting it makes that forbidden edge unreachable rather than merely detected" | **Already false** on this date: Broiler.JS publishes to nuget.org, which is Broiler.VM's one package source. The revision says so and replaces the claim with what actually holds the boundary: rules A2 and `I1`, and package source mapping (decision `SJL-D-4`). | ADR 0001 revision | SJL-2 |
| V12 | ADR 0001, why the repository does not chain to a parent `Directory.Build.props` | "it references no other Broiler component" | Still no chaining. The reason becomes "it references no other Broiler component's build" (packages are not builds). | ADR 0001 revision | SJL-2 |
| V13 | `docs/adr/0011-source-level-profile-contract.md`, standing refusals, including "A shared syntax tree or parser — Refused permanently." and "Lexing, source positions, diagnostic formatting — Waits for a second text front end." | Contract-bearing record. Editing its decision text would be read as an amendment, and the amendment procedure is unexecutable while one person holds R5 and both co-signing roles (EX-15). | **Not edited.** ADR 0014 records the scope reading: the refusals concern a component shared between Broiler.VM profiles or placed in the core namespace, which this programme never creates. ADR 0011 gains an editorial pointer paragraph beside the table, as it did for ADR 0013, without touching the table's text. If the core-contract owner rules the reading wrong, the programme is `Blocked` at SJL-0, naming the core as holder. | ADR 0014; pointer paragraph; route row | SJL-0 |
| V14 | `docs/adr/0012-security-ownership-and-support-matrix.md`, roles | R4 (vulnerability response) and R6 (release and recertification) cover Broiler.VM's own packages. | Both roles cover **admitted upstream packages** too: a vulnerability in one is an R4 case, and bumping its pin is an R6 recertification trigger. | Dated ADR 0012 revision (not contract-bearing) | SJL-2 |
| V15 | `docs/adr/README.md` | "All thirteen are `Proposed`" | Index row for ADR 0014; the count sentence corrected, with the superseded text quoted | Index edit, under rules E2 and E4 | SJL-0 |
| V16 | `docs/mvp.md` §5, route register | No route covers this programme | A route row for ADR 0011's scope reading. Columns: route taken / alternative not taken (an amendment, unexecutable under EX-15) / what would settle it / who decides (R5). | Route row | SJL-0 |
| V17 | `docs/compositions.md` §2 schema, "Sibling assemblies" | "a profile's own siblings, of which a lowering is the first" | A new column for the shared assemblies each root carries. The shared libraries are not siblings, so they cannot go in that column. | Register edit, under rules K2 and K4 | SJL-3 (first product consumption) |
| V18 | `THIRD_PARTY_NOTICES.md` | Lists Broiler.VM's own third-party data | A row for each admitted package, naming its licence. The data rows stay, because Broiler.VM still embeds its tables (decision `SJL-D-6` (c)). ADR 0001's component-file table says notices are "Required … on the day a runtime dependency first lands". | Notices edit in the same change as the dependency | SJL-3 onward |
| V19 | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, `graph.manifest.json`, `LegacyBoundaryTests.cs` remarks | A2 "No PackageReference names a Broiler.* package."; B3 `Vacuous`, with a permanence reason that relies on A1 and A2; the manifest has no field for package edges | See section 3 | Rule changes with witnesses | SJL-2 |
| V20 | `samples/NuGet.config`, `samples/Directory.Packages.props`, `eng/verify-feed.ps1`, `eng/collect-js-packages.py` | The JavaScript consumer restores only from `../artifacts/feed`, and the collection proxies the upstream to an unreachable address | The shared packages are placed on the local feed before restore. `verify-feed.ps1` maps the shared identifiers to their source. | Tooling change | SJL-2, exercised at SJL-3 |

*(Corrected 2026-10-12, at SJL-2. Two rows misdescribed the tree as it stood. Row V19 names the
`LegacyBoundaryTests.cs` remarks among the places that carry the single-source NuGet.config claim; the
claim was in `AssemblyMetadataRuleTests.cs`'s remarks, and that is where SJL-2.3 dropped it, while
`LegacyBoundaryTests.cs` never made it. Row V20's current state read "The JavaScript consumer restores
only from `../artifacts/feed`"; since 2026-10-11 `samples/NuGet.config` lists nuget.org alone, so the
consumer's isolated local-only configuration is now written by `eng/collect-js-packages.py` for its
runs, which also places every admitted package on that feed.)*

### 2.2 JavaScript profile documents (`src/Broiler.VM.Profile.JavaScript/docs/`)

| # | Where | What it says now | What the programme needs | Record | Milestone |
|---|---|---|---|---|---|
| P1 | `roadmap.md`, invariant 11 | "**The fork is one-way and mechanical.** No project reference, package reference, or shared source item runs between this component and any legacy Broiler component in either direction … Fixes do not flow across the fork after the snapshot, and neither side is the other's upstream." | Rewritten for admitted shared libraries. The shared library is upstream of both engines, and neither engine is upstream of the other. | New profile decision (JSD) plus correction entries (JSC) | SJL-0 |
| P2 | `roadmap.md`, invariant 12 | "**No evidence transfers.**" | **Unchanged.** Every profile gate is met by evidence this repository collects. | (none) | (none) |
| P3 | `roadmap.md` §4 and §5, the seed conditions and "no edge in either direction reaches any legacy Broiler component" | The copy table says to copy the tokenizer, parser, syntax tree and static analysis, and to "Acquire as this checkout's own dependencies: the regular-expression engine, the Unicode property tables, and the locale data". | The copy rows are withdrawn and replaced by shared-library rows. The "acquire" row is fulfilled by admitting the component packages. | JSD plus JSC entries | SJL-0 |
| P4 | `roadmap.gates.md`, gate 2 and the risk row | "no edge reaches a legacy component"; "**Stop: a build edge in either direction, or a fix ported across the fork, stops the milestone.**" | Revised in the same terms as V6 and P1. The stop condition becomes "an edge to an engine, or an engine fix copied across". | JSC entries | SJL-0 |
| P5 | `roadmap.delivery.md`, JS-2, and **JSD-0005**, the snapshot stop condition | JS-2 is `Blocked`. The snapshot is taken as-is on 2026-11-30, or earlier after a stated number of further seed commits. | **Decision `SJL-D-7`:** the seed copy is not taken, and JS-2's gate is re-scoped to consuming the shared frontend. This uses the settling route JS-2 already names: "a record says the seed copy will not be taken and JS-2's gate is re-scoped". | New JSD superseding JSD-0005's stop condition; JSC entries | **SJL-0, before 2026-11-30** |
| P6 | **JSD-0014**, "The source front end and the verification boundary" | Rejects the seed's four-place early-error split and its ambient parse state. | **Adopted as a requirement on the shared frontend**, not reversed: the frontend has one early-error stage and no ambient state. SJL-12 removes both from Broiler.JS. | Cited by the SJL-12 JSD | SJL-12 |
| P7 | **JSD-0031**, Unicode data source, §4(b) | Rejects `Broiler.Unicode.Properties` for five reasons: rules A2 and N1, non-conforming loose name resolution, no decomposition or case-folding data, unreproducible provenance, and runtime cost. | Revisited at SJL-7, **answering each of the five reasons on evidence**. If any is not answered, the area is refused. | New JSD (area verdict) | SJL-7 |
| P8 | **JSD-0027**, Intl scope and data strategy, §4 | Host ICU rejected. `UnicodeCldr.LocaleData` is "Candidate input, not accepted". | Revisited at SJL-9 under the conditions JSD-0027 implies: a published CLDR version, pinned generation, and review. | New JSD | SJL-9 |
| P9 | **JSD-0011**, the value, frame and call ABI | Rejects "Adopting the seed's boxed value hierarchy" and "A mechanical re-typing of the standard library". | **Unchanged.** The programme shares no value model and no library objects. | (none) | (none) |
| P10 | **JSD-0036**, "The universal bytecode is the back-end-neutral form" | "the front end returns a validated tree; the lowering consumes it and emits universal bytecode" | Compatible. The shared frontend **is** that front end. SJL-13 coordinates with UBC-3. | Cited | SJL-13 |
| P11 | `support.md` §2, fixed surfaces | "Number to string and string to number at the edges — Fixed — The specification's algorithms, implemented here rather than delegated to the platform" | "implemented in an admitted shared library, pinned by the shared vectors and rule N11's corpus" | Support-table edit (drafted, not issued) | SJL-3 |
| P12 | `packages.md` | "**No foreign dependency.**" Rule N37 holds every produced `.nuspec` to dependencies starting `Broiler.VM.` | The family's packages declare the admitted shared packages as dependencies. N37 is revised to allow exactly the allow-list. | JSD plus packages.md edit | SJL-2 / SJL-3 |
| P13 | `release-gate.md`, gates 8 and 12 | Gate 8: "produced metadata declares no foreign dependency". Gate 12: notices. | Gate 8 reads the allow-list. Gate 12 gains a notice row per admitted package. | JSD-0062 successor or JSC entries | SJL-3 |
| P14 | `api/public-api.txt` | `JsRegExpMatcher` and `JsRegExpCharge` are public in Format. | They leave the baseline when SJL-11 removes them (rule N10, both directions). | Baseline regeneration | SJL-11 |
| P15 | `diagnostics/registry.txt` (embedder-seam codes 2001–2406) | Codes are bound to `SliceSourceDiagnosticCode` by rules N5 to N9. | The registry stays this profile's. Shared frontend codes map onto it, with a new registry revision for every code whose meaning changes, as JSD-0009 requires. | Registry revision; JSD | SJL-13 |

### 2.3 Broiler.JS documents

| # | Where | What it says now | What the programme needs | Milestone |
|---|---|---|---|---|
| J1 | `docs/architecture/dependencies.md`, opening | "Broiler.VM is a separate component with no dependency edge to this one in either direction, and nothing here plans an integration with it." | Still no edge between the engines. Shared libraries in this repository are consumed by Broiler.VM, and they reference nothing from either engine. | SJL-0 |
| J2 | `dependencies.md`, the `FrontEnd/Semantics` row | "Shared by IL and bytecode; references neither back end", pending MOD-M2 | Becomes the shared frontend, consumed by the IL back end, the portable back end and Broiler.VM's lowering. "Nested DateTime, Regex, and Unicode components" is corrected: they are packages now. | SJL-0 (record), SJL-12 (boundary) |
| J3 | `docs/public-api.md` | "No Broiler.VM integration is planned for this component, and none changes that statement." | Revised. The public package list gains the shared packages, with their compatibility promise (semantic versioning, an API baseline and package validation), which none of Broiler.JS's current packages has. | SJL-0 (text), SJL-2 (tooling) |
| J4 | `docs/roadmap/README.md` | "The bytecode/VM plan is retired from this component." | **Kept**: no Broiler.VM profile is planned here. A new plan/status pair is added for the shared libraries (`docs/roadmap/SharedLibraries.md` and `.status.md`, in Broiler.JS's own plan/status convention). | SJL-0 |
| J5 | `docs/roadmap/Assemblies.md`, `Roadmap.md`, `ModernizationDelivery.md`, MOD-M2-7 | Residual text still plans "Separate IL and Broiler.VM JavaScript integration shells that both consume shared semantics". This contradicts J1 and J3. | Reconciled: the residual text is rewritten as the shared-library plan, or withdrawn with a pointer to it | SJL-0 |
| J6 | `docs/roadmap/Modernization.md`, invariant 3 | "The semantic front end is shared … between IL and bytecode." | Extended to the Broiler.VM consumer. MOD-M2-4 is the Broiler.JS half of SJL-12, SJL-12.5's item 4, and closes there. MOD-M2-5 is superseded in part: its boundary specification by SJL-12.1's contract, and its extraction plan by SJL-13 to SJL-15. MOD-M2-3 stays open, outside SJL-12 *(corrected 2026-10-19, below)*. | SJL-0 |
| J7 | `docs/roadmap/Component.md` §3, regular-expression consolidation | "retire the translator only after the pinned RegExp corpus is clean" | Aligned with SJL-11: the translator retires when Broiler.Regex meets the programme's conditions | SJL-11 |
| J8 | `docs/architecture/builtins-vs-clr-types.md` §6 | Says BuiltIns "reaches its own vendored `Broiler.DateTime`" | Stale; corrected | SJL-0 |
| J9 | `HUMAN_REVIEW.md`, `assurance.config.json` | Annotations are partial, the units file does not exist yet, and no CI lane enforces assurance. | The shared-library projects are covered by generated artefacts and a CI gate from their first commit (decision `SJL-D-5`) | SJL-2 |
| J10 | `scripts/packaging/test_pristine_consumer.py` | Expects BuiltIns to depend on `Broiler.Regex`, `Broiler.DateTime` and `Broiler.Unicode.Properties`, which is stale | Repaired, then extended to the shared packages | SJL-2 |
| J11 | `THIRD_PARTY_NOTICES.md` | Names only Yantra JS | Notices for every shared library's provenance and for the tables Broiler.JS embeds (section 9) | SJL-2 onward |

*(Corrected 2026-10-19, at SJL-12.1, by
[JSD-0080](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md)
decision D28.)* Row J6's need cell read: "Extended to the Broiler.VM consumer. MOD-M2-3, MOD-M2-4 and MOD-M2-5 are
cross-referenced as the Broiler.JS half of SJL-12." The cross-references were made on 2026-10-12, and they met
roadmap section 11's SJL-12 cell for Broiler.JS. What they say of each item:
- **MOD-M2-3 is not part of SJL-12.** It removes Storage's `Ast` dependency and resolves the foundation cycle, and
  no SJL-12.5 item touches either. It stays open, and its cross-reference in `Modernization.md` is corrected in the
  same way at SJL-12.1.
- **MOD-M2-4 is SJL-12.5's item 4**, the parser's `Runtime` reference, and it closes at SJL-12.5.
- **MOD-M2-5 is superseded in part.** SJL-12.1's contract supersedes its boundary specification, and SJL-13 to
  SJL-15 supersede its priced extraction plan.

### 2.4 Component repositories

| # | Repository | Change | Milestone |
|---|---|---|---|
| C1 | Broiler.Unicode | Adopt the conventions of [architecture.md](architecture.md#4-contract-conventions): exact-name lookup, immutable or span results, pinned and hashed inputs, public data versions, tests. Declare `IsAotCompatible` and `IsTrimmable` and run a Native AOT probe in CI (convention 12); the packages already publish under Native AOT without a warning but declare neither. Tables reach its algorithms through a provider, so each engine can embed the tables its generator produces (decision `SJL-D-6` (c)). Add an identifier series and assurance with a CI gate; its roadmap is filed at SJL-0.6. Correct the README's emoji version: the code carries 17.0, the README says 16.0. | SJL-7 |
| C2 | Broiler.Unicode (CLDR) | Nothing: decision `SJL-D-22` (a) generates the locale data in the Broiler.JS repository, so `Broiler.Unicode.Cldr.LocaleData` is not extended. Broiler.JS stops using it at SJL-8 and SJL-9. | (none) |
| C3 | Broiler.RegEx | Under decision `SJL-D-14` (a), meet every Broiler.VM requirement: a caller-owned budget with a distinct exhausted result, cancellation through it, compile-time and nesting limits, exact property names and group-name validation, `Katakana_Or_Hiragana`, Unicode data through the Unicode owner's provider, AOT flags, and test262 in CI. Remove the per-start budget reset, and fix the greedy fast path that can return a shorter match when its budget runs out. Implement duplicate named groups before Broiler.JS retires its translator (`SJL-D-14` (i)). Declare `IsAotCompatible` and `IsTrimmable` and run a Native AOT probe in CI; the package already publishes under Native AOT without a warning, but its parser's unguarded recursion can terminate the process. | SJL-11 |
| C4 | Broiler.DateTime | Nothing: decision `SJL-D-1` placed time data in the Broiler.JS repository | (none) |
| C5 | Broiler.JSeal | Nothing: decision `SJL-D-2` took the neutral prefix `Broiler.ECMAScript`, which its package gate (`eng/package-gate.mjs`) admits | (none) |
| C6 | Aggregate | Submodule pointers advance together when a shared major version or data version moves | Every release that moves both engines |

---

## 3. Rules each repository mints or revises

### 3.1 Broiler.VM

Every minted or revised rule carries a witness. A witness is a test input that is watched failing
when the defect it describes is injected, and passing after the injection is reverted. Its
register row names it.

| Rule | Change | Witnesses (at least) | Milestone |
|---|---|---|---|
| **A2** | Revised from "No PackageReference names a Broiler.* package" to "No PackageReference names a Broiler package outside rule `I1`'s allow-list". The check also covers identifiers spelled `BroilerJS.*`. | The existing `A2-broiler-package-reference.csproj.witness`, kept; a non-admitted `Broiler.JavaScript.*` package; an admitted package referenced from a project the list does not name | SJL-2 |
| **B3** | Rewritten and promoted from `Vacuous` to `Active`. It sweeps every shipping assembly, not only the core and the fixtures, against the component's own set plus the admitted assemblies. Its permanence reason, which relied on A1 and A2 making such a reference impossible, is withdrawn. | An assembly referencing a non-admitted Broiler assembly | SJL-2 |
| **N1** | "declares no PackageReference" becomes "declares no PackageReference outside the permitted set" | Keep `N1-profile-package-reference.csproj.witness` (`System.Text.Json`); add an engine package | SJL-2 |
| **N37** | The produced `.nuspec` may declare dependencies outside `Broiler.VM.` only for admitted packages, at the version the allow-list pins or a later one. `PackableFamilyProjects` reads package references, not only project references. | A nuspec with a non-admitted dependency | SJL-2 |
| **A7** and `graph.manifest.json` | The manifest gains a package-edge field per project, and A7 compares it in both directions | A manifest missing a package edge the project file declares | SJL-2 |
| **K2 / K4** and `docs/compositions.md` | Register rows declare the shared assemblies per root, and K4 allows exactly those | A closure containing an undeclared shared assembly | SJL-3 |
| **E4** | Its rule-identifier pattern recognises group `I`. It also gains the groups that pattern misses today (K, L, M, N, W, X), which is a recorded gap. | A record citing `Rule I1` with no register row | SJL-2 |
| **I1** (new) | **Admission.** Every package reference in the product graph is on the allow-list: package identifier, version, the projects that may reference it, and its content hash as the lock file records it. Nothing outside the JavaScript family may hold an admitted reference, except a test project through a test-only entry that names it; a test-only entry never names a project that ships. *(Corrected 2026-10-16, at SJL-7.4, when decision `SJL-D-6` (c)'s generator reached the architecture tests "as a pinned, test-only package" and the rule was revised for it. The cell read "Nothing outside the JavaScript family may hold an admitted reference.")* | An unlisted package; a listed package in the core; a content hash that differs from the lock file; since SJL-7.4, a product entry naming a test project and a test-only entry naming a shipping project | SJL-2; revised SJL-7 |
| **I2** (new) | **Imported assembly hygiene.** Every admitted assembly in a published closure is held to the member lists of B5, B5b and B5c, which today read only the solution's own projects and never a package. It also references no platform globalization or regular-expression API: normalization, `System.Text.RegularExpressions`, culture casing or comparison, `TimeZoneInfo` (the metadata equivalents of N18 and N23). ILC, publishing with warnings as errors, already fails on a trim or AOT warning in package code a root reaches; `I2` also covers what no root reaches yet. | A witness assembly that references `System.Text.RegularExpressions` | SJL-2 |
| **I3** (new) | **Deployment boundary.** No compiler-free root's closure contains the frontend. No root without an Intl or zoned-Temporal surface carries optional data. A Native AOT publish directory holds no managed assembly, so for that mode the closure is read from ILC's own record of the image: the mstat file `IlcGenerateMstatFile` writes, which the collector retains. | ExecutionOnly with the frontend in its closure | SJL-2, active from SJL-3 |
| **I4** (new) | **Data agreement.** The pins Broiler.VM's embedded tables are generated from equal their home repository's pins, by hash, and every admitted library's declared data version equals the pin that governs it: Unicode, CLDR, tzdb or calendars. Otherwise a dated decision moved the pin in the same change. *(Minted 2026-10-16, at SJL-7.4, for the Unicode pin, the one with a home that holds a pin; CLDR, tzdb and the calendars join it when their areas move. Extended 2026-10-17, at SJL-8.5, to the tzdb and calendar pins and CLDR's time zone keys, whose home is Broiler.JS's `Broiler.ECMAScript/data/time/`: its generator package carries no pin file, so the rule holds Broiler.VM's pins to the home's by digests recorded with the Broiler.JS commit they were read at, and every time data version the package declares to the pins'.)* | A Broiler.VM pin differing from its home's by one hash; a library declaring another Unicode version; since SJL-8.5, a tzdb pin differing from the home's by one hash and a generator declaring another tzdb release | SJL-7; extended SJL-8 |
| **I5** (new) | **Retired duplicates.** A register of every member removed at a cut-over, by fully qualified name. None may reappear in Broiler.VM's sources. | A re-added `JsNumberFormat.ToJsString` | SJL-3 |
| **I6** (new) | **No leak through the public surface.** No admitted type appears in the JavaScript family's public API baseline unless a decision baselines it. This is rule B4 extended to the family, which B4 does not reach today. | A public member returning a shared type | SJL-2 |
| **I7** (new) | **Upstream identity in evidence.** Every bundle that runs a composition carrying an admitted package records that package's identifier, version and content hash, and the source commit from its nuspec | A bundle manifest missing the commit | SJL-2 |
| **N12, N18, N19, N23** | Kept for Broiler.VM's own sources. Their remarks gain a sentence saying that code which moved to a shared library is held by `I2` and by that library's own `SL` rules. N18's and N23's vacuity guards require named files to exist (`JsRegExpMatcher.cs`, `JsRealm.String.cs`), so they are re-pointed when those files are deleted or shrink. | Existing witnesses | SJL-7, SJL-11, SJL-13 |
| **N22, N27, N28, N29–N32** | Kept in Broiler.VM under decision `SJL-D-6` (c). Each regenerates Broiler.VM's embedded tables with the home's generator from Broiler.VM's pins, byte for byte, and `I4` holds those pins equal to the home's. N29 to N32 also gain the register rows they lack today. *(Done 2026-10-17, at SJL-8.5: N29 to N32 regenerate with the home's generator, and each has its register row.)* | The existing witnesses; a regenerated table with one byte changed | SJL-7, SJL-8, SJL-9 |
| **J group** | Unchanged. A bundle's exclusions table (rule H2) defines a new exclusion, numbered at filing time (EX-125 on this date), naming the code that leaves Broiler.VM's assurance coverage and where it is covered instead | (none) | SJL-3 |
| **A1, A3, D1** | **Unchanged.** Project references and linked source outside the component stay forbidden, and so does any reference into Broiler.VM. A shared library may never reference Broiler.VM. | (none) | (none) |
| **A12, A13** | **Unchanged.** Composition roots and consumer profiles declare no package references; shared packages reach roots transitively. | (none) | (none) |

### 3.2 Shared libraries (group `SL`, in their home repository)

The libraries carry a small architecture-test project and a rules register in Broiler.VM's shape:
statement, implementing test, witness and status. **Broiler.VM's gates rely on these rules being
active in the home repository, so a regression there is visible in an upstream ledger rather than
only in a downstream failure.**

| Rule | Statement |
|---|---|
| SL1 | No shared project references an engine project or package, anything in Broiler.VM, or a package outside the shared allow-list (Broiler.Unicode, Broiler.Regex, and each other in the direction of section 1 of [architecture.md](architecture.md)). |
| SL2 | No mutable static, settable static property, `[ThreadStatic]`, `AsyncLocal` or `ThreadLocal`. |
| SL3 | Nothing on the lists of Broiler.VM's rules B5, B5b and B5c (dynamic loading, reflection invocation, `Reflection.Emit`, `LambdaExpression.Compile`, native-library and function-pointer marshalling, method preparation, P/Invoke, module initializers). Also no generic instantiation over a run-time `Type`, no `Enum.GetValues(Type)`, no `dynamic`, no reflection-based serialization, no I/O and no environment read (convention 8). |
| SL4 | No platform globalization or regular-expression API (convention 9). |
| SL5 | `IsAotCompatible` and `IsTrimmable` hold for every shared library, the frontend included, and for the test code the probe runs, with no trim, AOT or single-file warning. No such warning is suppressed, and an inventory in the shape of Broiler.VM's N36 shows it. The Native AOT probe publishes with Broiler.VM's root settings and runs the whole vector corpus (convention 12). |
| SL6 | The public API baseline matches in both directions, and package validation passes against the last published version. |
| SL7 | Generated data is byte-identical to what its generator writes from the pinned, hashed inputs. |
| SL8 | Every relevant unit carries an assurance annotation, and the generated artefacts are current. |
| SL9 | Determinism: two runs, and two threads, over the vector corpus give identical output. |
| SL10 | No preprocessor directives; LF line endings. |

---

## 4. How each change is filed

| Kind of change | Procedure |
|---|---|
| A sentence in `docs/roadmap.md` | A row in ADR 0003 §11's roadmap-amendment register: the section, the old text verbatim (ASCII, line breaks normalised), the new text, the requiring ADR (ADR 0014), and the status `Proposed` until the change that makes it true, then `Applied`. The revised sentence in the roadmap carries its superseded text beside it, dated. |
| A sentence in a ledger, the README or a plan | A dated correction in place, quoting the superseded text |
| ADR 0001, ADR 0012 | A dated revision section stating what the record said before. ADR 0001's last budget sentence is updated under rule A15 if the project count changes. Packages are not projects, but the revision says how many package edges the graph gained, by citation. |
| ADR 0011 | **No edit to decision text.** An editorial pointer paragraph naming ADR 0014, as for ADR 0013. |
| A new core record | **ADR 0014**, the shared-libraries admission record, `**Core contract:** not contract-bearing`, status `Proposed`. It holds: the scope reading of ADR 0011 with its route; the admission policy and the allow-list mechanism; the homes and the package identity; the evidence policy ("no evidence transfers" kept); the ownership of upstream defects (the fork policy); the versioning and co-hosting policy; the consumption source; the assurance regime; the pilot decision; what is not planned; and the consequences, with every record it moves. |
| A route taken without a decision that could choose it | A row in `docs/mvp.md` §5 in its four-column shape. Taking a route without adding a row is the failure that section exists to prevent. |
| A profile plan sentence | A new JSD, which states what it rejects and why, with the settling condition and the one-person non-independence. Then one JSC entry per changed reading, in the five-field shape, with a bare *(corrected: JSC-nn)* pointer in the plan. |
| A Broiler.JS document | A dated revision in place. The new plan/status pair follows Broiler.JS's own convention: every item has an owner, current evidence, a next action and an objective exit criterion, and no changing count is written into a plan. |
| A component repository | An entry in that repository's roadmap. Broiler.Unicode has none, so SJL-0.6 creates a minimal `docs/roadmap.md` there. |
| An area verdict (accept or refuse) | A JSD in the profile series, because an area changes the JavaScript family's dependencies and not the core graph, plus a pointer in ADR 0014's consequences section. Broiler.JS records the same verdict in its shared-libraries status file. |

---

## 5. Release, versioning and compatibility

### 5.1 Versions

- **The shared packages version separately from the Broiler.JS engine packages** (decision
  `SJL-D-3`, taken on 2026-10-11). Today Broiler.JS's `eng/pack.ps1` requires one version for every
  package it packs, and `eng/resolve-preview-version.mjs` takes the next unused preview number
  across nuget.org and the repository's `v*` tags. The shared libraries get their own solution
  filter, pack invocation, tag prefix and version stream in the same repository. Lockstep versioning
  with the engine, the alternative not taken, would have bumped Broiler.VM's dependencies on every
  Broiler.JS release, whether or not a shared library changed.
- **Semantic versioning.** Package validation runs against the last published version.
  - A public API removal or a binary-breaking change is a major version.
  - A new API is a minor version.
  - A correction toward the specification is a minor version, with its own changelog entry under
    "Observable changes".
  - A data-version change is a minor version, with its own entry.
- **Previews.** `X.Y.Z-preview.N` while the engines are in preview, as every Broiler repository
  does today.

### 5.2 Publication and consumption

- **Publication** follows the home repository's own rules: nuget.org only, manually or on a tag,
  with a CI dry run that packs and restores from an isolated cache. Broiler.VM's own rule (no
  Broiler.VM package is published without a completed review naming the reviewed commit) governs
  Broiler.VM's packages only.
- **A Broiler.VM package can declare a shared dependency only once that dependency is published.**
  `verify-feed.ps1` says so: "Every dependency outside this release must already be on nuget.org."
- **How Broiler.VM's CI obtains a shared package** is decision `SJL-D-4`, taken on 2026-10-11 as
  (c), both sources:
  - a committed, hash-pinned local feed while an area is in development, with package source mapping
    sending the shared prefix to it and lock files in locked mode, consistent with how Broiler.VM
    already archives `test262`, tzdb and the calendar crates;
  - published packages from nuget.org for anything a Broiler.VM release depends on.
- **Lock files** (`RestorePackagesWithLockFile`, restore in locked mode in CI) are adopted by every
  project that references a shared package, so the content hash of what restored is part of the
  tree, which rule `I1` reads.

### 5.3 Compatibility for Broiler.JS's existing public types

Broiler.JS has no type forwarding anywhere, no API baseline, and a README that calls its API
unstable. Its Modernization invariant 7 nevertheless says "Namespace preservation alone is not
binary compatibility; assembly identity, type forwarding, package contents, and pristine consumers
are gated", and its MOD-M0-4 leaves the type-forwarding question open. For each public type an
area moves or empties, the area records one of three dispositions:

- **forwarder:** the public member stays and calls the shared library, for example
  `NumberParser.CoerceToNumber` and `JSNumericOperators.ToInt32`;
- **obsolete forwarder:** marked `[Obsolete]` with a removal version;
- **break:** removed in a major version, recorded in `public-api.md`.

Decision `SJL-D-25` sets the default: a forwarder for every public member that downstream consumers
or tests use, an obsolete forwarder for the rest, and a break only after a recorded consumer check.
The public `Ast` and `Parser` packages are the largest case. Decision `SJL-D-12` (i) makes them
adapter packages, marked obsolete and removed in a later major version.

### 5.4 Co-hosting

Two rules hold:

- **Engines declare minimum versions** of shared packages in their nuspecs.
- **Engines pin exact versions** in their own lock files and compositions.

When a shared major version or data version moves, both engines move in coordinated releases, and
the aggregate's submodule pointers move together. See
[architecture.md](architecture.md#63-a-process-that-hosts-both-engines).

---

## 6. The cross-repository change flow

1. **A defect or a gap is found:** by a vector, by test262 in either engine, by fuzzing or by a
   user.
2. **It is routed to the home repository.** It is never patched inside an engine's copy, because
   after cut-over there is no copy. An engine may add a failing vector that demonstrates the defect.
3. **The home repository fixes it:**
   - vector first, watched failing;
   - then the fix;
   - then the reference-arm comparison, unit tests, the `SL` rules and the Native AOT probe.
4. **A release** carries the changelog entry, and the "Observable changes" heading if a script can
   see the difference.
5. **Each engine moves its pin in its own change.** It runs its own evidence for the area (vectors,
   test262 under the area's comparison rule, closures, packages), updates its notices if data
   moved, and records the move in its own ledger. **Neither engine's gate cites the other's
   evidence.**
6. **Urgent cases.** If an engine cannot wait for a release, it may pin back to the previous version.
   It may not fork the library, and it may not add a local copy of the algorithm. A local
   workaround in an adapter is permitted only with a dated correction entry naming the upstream
   defect and the release that will remove it.
7. **An engine needs behaviour the other does not.** Examples are Broiler.VM's width ceilings and
   Broiler.JS's non-standard literal suffix. The behaviour becomes a **parameter of the shared
   operation**, such as a limit, a policy or a flag in an options record, or it stays in that
   engine's adapter. It never becomes a fork.

**Fixes flow through the shared libraries in both directions. Engine code still does not flow
between the engines.**

---

## 7. Review and assurance

- **Broiler.VM's assurance rules (J1 to J12) cover Broiler.VM's own source.** Code that moves into a
  shared library leaves that coverage. The bundle of the milestone that moves it defines an
  exclusion (EX-125 on this date) naming what left and where it is now covered.
- **The home repository covers it instead.** The shared projects use the Broiler Code Assurance
  scheme: one annotation per relevant declaration, generated artefacts, and a CI gate. Today
  Broiler.JS opts into the scheme in `assurance.config.json` but has partial annotations, no
  generated units file and no gate. Broiler.RegEx has generated artefacts but no CI enforcement.
  Broiler.Unicode has none. **Decision `SJL-D-5`**, taken on 2026-10-11 as (a), makes them
  prerequisites: from SJL-2 for new projects, and before cut-over for each moved area.
- **Ported code keeps its origin.** A unit moved out of Broiler.VM is annotated `Origin=Ported`, as
  the JavaScript ledger's update rule 9 requires of ported units. Its fingerprint is regenerated in
  its new home, and its human-review state starts again: a moved unit is not a reviewed unit.
- **Broiler.VM's release gate 9** requires a human decision on every relevant unit before
  publication. Under decision `SJL-D-5` (a), a Broiler.VM release that ships a shared package also
  requires that package's revision to carry a completed review record in its home repository, read
  through rule `I7`'s recorded upstream commit.
- **Human review is per revision** in every repository involved, and none of them has a review
  current at its head on this date. No record here claims a review.

---

## 8. Security

**Threat model.** The shared libraries process untrusted input:

- source text (the frontend);
- regular-expression patterns and subject strings (Regex);
- numeric, date, Temporal and JSON strings (algorithms);
- strings for normalization, casing and segmentation (Unicode).

Data is a trusted input: pinned, hashed, and embedded by each engine (decision `SJL-D-6` (c)).

**Obligations:**

1. **Bounded work.** Every operation that untrusted input can drive is metered and limited
   (conventions 4 and 5). Broiler.VM's denial-of-service posture rests on this: its fuel, its
   cancellation polling and its ceilings must reach inside the shared code.
2. **No unbounded recursion.** Parsers and matchers use explicit stacks or depth limits that are
   checked before the stack is. Broiler.Regex's parser and compiler recurse without a guard today.
   Broiler.VM's compilation runs on its own large thread, and its tree-depth bound is a constant the
   host cannot raise (rule N19). The frontend keeps both properties.
3. **Fuzzing.**
   - Broiler.VM's fuzz host and source fuzz seeds cover the compiler-bearing compositions and so
     reach the frontend. *(Read 2026-10-26, at SJL-13.1, by
     [JSD-0083](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md) decision D22. As written it did not hold of
     the wide path: Broiler.VM's source fuzz session reaches `SliceTokenizer` and the slice front end, and no
     session reached the wide parser. SJL-13's tools commit adds a wide-path session, `--fuzz-wide`, in the slice
     compiler root, over seeds bundle `sjl-13-001` commits with its rule, CI running the seeds the lane's seed file
     names from the Native AOT image; both of SJL-13's Broiler.VM bundles run both sessions.)*
   - The home repository adds fuzz targets for its parsers: numbers, Date strings, Temporal
     strings, JSON, regular-expression patterns, and source text.
   - Crashes become vectors.
4. **Supply chain:**
   - package source mapping that sends the shared prefix only to its intended source;
   - lock files with content hashes;
   - nuget.org repository signatures;
   - Source Link and deterministic builds;
   - the upstream commit recorded in Broiler.VM's bundles (rule `I7`).

   *(Corrected 2026-10-12, when the owner withdrew the prefix reservation and decision `SJL-D-2` was
   revised. The list's last entry read: "a reservation of the shared package prefix,
   `Broiler.ECMAScript` (decision `SJL-D-2`), on nuget.org before anything under it is published
   (SJL-2.13), which also guards against dependency confusion." A package substituted under the
   prefix is caught by the lock files' content hashes, which rule `I1` holds to its pins.)*
5. **Vulnerability response.** ADR 0012's R4 covers admitted packages (record V14). The home
   repositories need a security intake; Broiler.VM's release gate already lists a missing intake
   (`G13-no-security-intake`) as a blocker. A fix to a shared library that both engines ship
   releases in both engines together.

---

## 9. Licensing and provenance

- **Licences.** Broiler.VM, Broiler.JS and the component repositories are Apache-2.0.
- **Broiler.JS provenance must be checked unit by unit before anything moves.** Its notices name
  only Yantra JS, but:
  - its `assurance.config.json` records further origins: a Jint-derived `DateParser.cs` under
    BSD-2-Clause, an MIT-licensed vendored ILPack, and a CC-BY-SA-3.0 Stack Overflow snippet;
  - `UriHelper` and `JSMath.Clz32` cite Jurassic;
  - `NumberParser.RefineEstimate` has the same structure with no attribution;
  - `JSNumberPrototype.DecimalToArbitrarySystem` cites Stack Overflow.

  The rules:
  - **No share-alike code enters a shared library.** The cited radix routine is on the deletion
    list anyway.
  - Permissively licensed code may enter with its notice.
  - Code whose provenance is unclear is rewritten from the specification rather than moved.
- **Code moved from Broiler.VM** keeps its Apache-2.0 licence and records its origin.
- **Data licences travel with the data.** The licences involved are:
  - the Unicode licence for the UCD and CLDR;
  - public domain for the tz database;
  - the ICU4X crates' licences for calendar tables;
  - test262's licence for vectors derived from it.

  Under decision `SJL-D-6` (c) no shared package carries data. Each engine's notices cover the
  tables it embeds and name every admitted package, and the data home's notices cover its pinned
  inputs (V18, J11).

---

## 10. Documentation obligations per library

Each shared library carries:

- a README stating its scope, its specification and data pins, its conventions and its consumers;
- a changelog with "Observable changes" and "Data" headings;
- XML documentation citing the specification clause of every public member;
- a provenance file for every data set;
- an **adapter guide**: for each operation, what the caller must have done first (coercions,
  checks), what the caller must do with each status, and which errors the specification assigns
  to each status.

---

## 11. Cross-references between repositories

- **Broiler.VM hosts the programme roadmap and its ledger**, because the feasibility report lives
  here.
- **Broiler.JS hosts a pointer document and its own plan/status pair** for the work that lands in
  Broiler.JS (record J4). The component repositories host roadmap items.
- **Links between repositories** are relative paths that assume sibling checkouts, as the
  feasibility report's links do. They resolve in the local layout used on this date and are not
  guaranteed outside it.
- **No ledger advances another ledger's row.** This ledger may link to a Broiler.JS status row as
  observed state in another component, which can explain a status here and never satisfies a
  clause here.
