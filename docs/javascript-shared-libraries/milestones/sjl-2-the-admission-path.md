# Milestone SJL-2 — The admission path

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Governance](../governance.md) · [Architecture](../architecture.md)

- **Objective.** Two capabilities exist, and both are proven end to end with a probe package before
  any algorithm moves:
  - **Broiler.VM can admit a shared package.** A JavaScript-family project can consume a named,
    content-pinned package that references nothing in Broiler.VM or in either engine. Rules with
    witnesses refuse every other package, every other project, and every closure that breaks a
    deployment boundary.
  - **The home repository can produce one.** It can build, check and package a shared library under
    conventions and rules of its own.
- **Owner roles.**
  - **Broiler.VM:** the architecture owner (rules A2, B3, A7 and group `I`), the JavaScript profile
    owner (N1, N37), and the release owner R6 (tooling, sources, ADR 0012).
  - **Broiler.JS:** its maintainer, for the home foundation.
  - All one person.
- **Repositories.** Broiler.VM and Broiler.JS. Broiler.JSeal is not changed: the neutral prefix
  [SJL-D-2](../decisions.md#sjl-d-2) took passes its package gate.
- **Waits on.** SJL-0, which records the decisions this milestone applies, all taken on 2026-10-11:
  - [SJL-D-1](../decisions.md#sjl-d-1): the algorithms, frontend and vectors live in the Broiler.JS
    repository;
  - [SJL-D-2](../decisions.md#sjl-d-2) to [SJL-D-5](../decisions.md#sjl-d-5): the
    `Broiler.ECMAScript.*` packages, a separate version stream with minimum ranges and lock files,
    the committed feed in development with nuget.org for releases, and full assurance parity;
  - [SJL-D-8](../decisions.md#sjl-d-8) and [SJL-D-9](../decisions.md#sjl-d-9): the scope reading
    and the fork policy.

---

## Work packages: Broiler.VM

### SJL-2.1 — The allow-list and rule `I1`

The allow-list is one machine-readable file in the architecture-test project. A field of
`graph.manifest.json` would do equally well; the record says which. Each entry names:

- the package identifier;
- the pinned version;
- the projects that may reference it, all in the JavaScript family;
- whether it is product or test-only;
- its sources under [SJL-D-4](../decisions.md#sjl-d-4) (c): the committed feed, and nuget.org once a
  Broiler.VM package release depends on it.

**Rule `I1`.** Every `PackageReference` in the product graph is on the list, from a listed project,
and the content hash its lock file records equals the hash the list pins. Nothing outside the
JavaScript family holds an admitted reference.

Lock files (`RestorePackagesWithLockFile`) are enabled for every project that references a shared
package, and CI restores in locked mode.

**Witnesses** (each must fail rule `I1`):

- an unlisted package;
- a listed package referenced from `Broiler.VM.Runtime`;
- a listed package referenced from Format;
- a listed package whose lock-file hash differs from the pin.

*Done when* `I1` has a register row, a test and its witnesses, each watched failing and then
passing on revert.

### SJL-2.2 — Rule A2 revised

The rule reads "No PackageReference names a Broiler package outside rule `I1`'s allow-list". It also
covers identifiers spelled `BroilerJS.*`, which the current `StartsWith("Broiler.")` test misses.

The existing witness `A2-broiler-package-reference.csproj.witness` (`Broiler.JS 0.1.0-preview.1`) is
kept. New witnesses:

- a non-admitted `Broiler.JavaScript.Parser` reference;
- a `BroilerJS.Network` reference.

*Done when* the row, the test and every witness hold.

### SJL-2.3 — Rule B3 rewritten and activated

Today B3 is `Vacuous`, and its permanence reason relies on A1 and A2 making a foreign Broiler
assembly reference impossible. Its tests sweep only the core assemblies and the fixtures.

Rewritten, it sweeps every **shipping** assembly against the component's own assembly set plus the
admitted assemblies, and moves to `Active`. Its remarks, and those of `LegacyBoundaryTests`, drop
the claim that a single-source `NuGet.config` makes a foreign Broiler package unresolvable. That
claim is false on this date: Broiler.JS publishes to nuget.org, which is Broiler.VM's one source.

*(Corrected 2026-10-12, when the work package was done: the claim was in the remarks of
`AssemblyMetadataRuleTests`, and in B3's register row, not in those of `LegacyBoundaryTests`, which
never made it. It was dropped from both places it was.)*

**Witness:** an assembly referencing a non-admitted Broiler assembly.

*Done when* B3 is `Active` with its witness.

### SJL-2.4 — Rules N1 and N37 revised

- **N1:** "declares no PackageReference" becomes "declares no PackageReference outside the set rule
  `I1` admits for this project". The `System.Text.Json` witness is kept, so a non-Broiler package is
  still refused. A witness for a non-admitted engine package is added.
- **N37:** a produced `.nuspec` may declare a dependency outside `Broiler.VM.` only on an admitted
  package, at the pinned minimum version or later. `PackableFamilyProjects` reads package
  references, not only project references. `src/Broiler.VM.Profile.JavaScript/docs/packages.md`'s
  "No foreign dependency" paragraph is revised through a JSD and its JSC entries.

*Done when* both rows and their witnesses hold.

### SJL-2.5 — The graph, ADR 0001 and ADR 0012

- **`graph.manifest.json`** gains a per-project package-edge field. Rule A7 compares it with the
  project files in both directions, with a witness for a missing package edge.
- **ADR 0001** receives a dated revision stating what each changed section said before:
  - the outbound legacy boundary: what the rules now enforce;
  - the NuGet.config rationale: the claim is already false, and what holds the boundary instead;
  - the no-chaining reason: "references no other Broiler component's build";
  - the package-edge budget sentence, by citation of the catalogue figure, never typed.
- **ADR 0012** receives a dated revision: R4 (vulnerability response) and R6 (release and
  recertification) cover admitted packages, and a pin move is a recertification trigger. Under
  decision [SJL-D-5](../decisions.md#sjl-d-5) (a), a release refuses an admitted package whose
  upstream revision, the commit rule `I7` records, lacks a completed review.

*Done when* A7 holds with its witness and both revisions are filed.

### SJL-2.6 — Rules `I2`, `I3`, `I6` and `I7`, and rule E4

| Rule | What it checks | Witness |
|---|---|---|
| **`I2`**, imported-assembly hygiene | It reads every admitted assembly in a published closure, or in the restored package. It applies the member lists of B5, B5b and B5c, which read only the solution's own projects, and refuses member references into `System.Text.RegularExpressions`, the platform normalizer, culture-sensitive casing and comparison, or `TimeZoneInfo` (the metadata equivalents of N18 and N23). | A witness assembly referencing `System.Text.RegularExpressions` |
| **`I3`**, deployment boundary | The closure of `JavaScript.ExecutionOnly` and `JavaScript.Android` contains no frontend assembly. No root without an Intl or zoned-Temporal surface carries optional data. A Native AOT publish directory holds no managed assembly, so for that mode the collectors request ILC's mstat file (`IlcGenerateMstatFile`), retain it, and `I3` reads the assemblies it records. | A closure listing that puts the frontend in `ExecutionOnly` |
| **`I6`**, no leak | No admitted type appears in the JavaScript family's API baseline unless a decision baselines it. `ProfileApiSurface`'s resolver learns to resolve admitted assemblies, so a public signature naming one cannot silently break the baseline run instead. | A public member returning an admitted type |
| **`I7`**, upstream identity | A bundle manifest that runs a composition carrying an admitted package records its identifier, version, content hash and nuspec source commit | A manifest missing the commit |

Rule E4's identifier pattern gains group `I`, and the groups it misses today (K, L, M, N, W, X).

*Done when* every row holds with its witness.

### SJL-2.7 — Sources and tooling

- **Sources, under [SJL-D-4](../decisions.md#sjl-d-4) (c).** `NuGet.config` gains package source
  mapping that sends the shared prefix to two sources, and everything else to nuget.org only:
  - **a committed, hash-pinned feed** for areas in development. It lives in a pinned directory with
    a `.pin` file per package, named for example `eng/packages/`;
  - **nuget.org**, for anything a Broiler.VM package release depends on.

  Lock files pin the content hash whichever source supplies a package, and rule `I1` reads it.
  **No other repository's local folder is ever a source**, `D:\local-packages` included.
- **`eng/verify-feed.ps1`** requires every admitted package that a release depends on to be on
  nuget.org already, as it requires of every other dependency outside the release.
- **`samples/NuGet.config`** and **`eng/collect-js-packages.py`** place every admitted package on
  `artifacts/feed` before the pristine consumer restores with the upstream unreachable. The consumer
  restore therefore proves that the shared packages travel with the family's packages.

  *(Corrected 2026-10-12, when the work package was done: `samples/NuGet.config` lists nuget.org
  alone since 2026-10-11, when the samples moved to the published packages, so it is
  `eng/collect-js-packages.py` that writes the consumer's local-only configuration for its isolated
  runs, beside placing every admitted package on `artifacts/feed`. `samples/NuGet.config` is not
  edited.)*
- **The collectors** (`eng/collect-evidence.py`, `eng/collect-js-evidence.py`, the bundle-manifest
  script) record `I7` identity. They treat admitted assemblies as declared rather than filtering
  them by name: the current closure filter excludes only `System.` and `Microsoft.` names (EX-87).

*Done when* a scratch restore through each configured source succeeds, and a restore of an unmapped
identifier fails.

### SJL-2.8 — The register rows become `Applied`

The roadmap sentences filed `Proposed` at SJL-0 are revised in place in `docs/roadmap.md`. Each
revised sentence carries its superseded text, dated. The register rows move to `Applied`.

*Done when* every row is `Applied` and every sentence carries its superseded text.

### SJL-2.9 — The admission probe

A **probe package** comes from the home's SJL-2.13. It contains only the contract types of
[architecture.md section 4](../architecture.md#4-contract-conventions): the meter delegate, status
enumerations and limit records. It is admitted as **test-only**, and an architecture test restores
it into a scratch JavaScript-family-shaped project.

- **Positive:** `I1`, `I2` and `I6` pass on it.
- **Negative controls**, each of which must fail: the probe referenced from a core project; the
  probe with a changed hash; a probe build that references `System.Text.RegularExpressions`.

**No product project references any shared package at the end of this milestone.**

*Done when* the probe's positive and negative runs are retained.

---

## Work packages: the home repository (Broiler.JS)

### SJL-2.10 — Layout and conventions

- **Layout.** A solution folder for the shared libraries, with its own solution filter, separate
  from the engine's projects. Its `Directory.Build.props`:
  - `net10.0`;
  - `Nullable` enabled and `ImplicitUsings` disabled;
  - warnings as errors;
  - `IsAotCompatible` and `IsTrimmable`, which turn on the trim, AOT and single-file analyzers, with
    no warning suppressed (rule SL5);
  - deterministic builds and Source Link;
  - **no** `AllowUnsafeBlocks` unless a library's record justifies it.
- **Line endings.** A `.gitattributes` gives the folder LF line endings. Broiler.JS has none today,
  and some of its files are CRLF or mixed.
- **Conventions.** A conventions document makes
  [architecture.md section 4](../architecture.md#4-contract-conventions) normative. An adapter-guide
  template comes with it.

*Done when* the folder builds empty and the conventions document is filed.

### SJL-2.11 — The `SL` rules

An architecture-test project with a rules register in Broiler.VM's shape: statement, test, witness,
status. It holds SL1 to SL10, from [governance.md](../governance.md#32-shared-libraries-group-sl-in-their-home-repository),
and each rule has a witness watched failing and then passing.

*Done when* every rule is `Active` with its witness.

### SJL-2.12 — API baseline, package validation and assurance

- **API baseline.** A generated public-API listing, compared in both directions. It is the first
  API baseline in Broiler.JS's repository. Package validation is enabled once a first version is
  published, against that version.
- **Assurance.** Every relevant unit carries its annotation. The generated artefacts are produced
  by the Broiler Code Assurance tool, and a CI job fails on drift, from the shared folder's first
  commit. This is decision [SJL-D-5](../decisions.md#sjl-d-5) (a). In Broiler.JS today the scheme is
  configured, partial and unenforced.

*Done when* both checks run in CI and are watched failing on an injected drift.

### SJL-2.13 — Pack, version and publish

- **A separate version stream** ([SJL-D-3](../decisions.md#sjl-d-3)). The pack script is
  parameterised by project set and version, and the version resolver uses its own tag prefix.
  Broiler.JS's `eng/pack.ps1` and `eng/resolve-preview-version.mjs` assume one version and one tag
  series for the whole repository today.
- **The prefix reservation.** The home's release owner applies to reserve the `Broiler.ECMAScript`
  prefix on nuget.org before any package under it is published
  ([SJL-D-2](../decisions.md#sjl-d-2); [governance.md](../governance.md#8-security)).
- **The publish workflow** runs manually or on that tag. Its CI dry run packs and restores in an
  isolated cache, as Broiler.JS's `verify-feed.ps1` does.
- **The probe pre-release** of the algorithms package, holding contract types only, is packed. It
  reaches Broiler.VM through the committed feed ([SJL-D-4](../decisions.md#sjl-d-4) (c)), and is
  not published to nuget.org, because no Broiler.VM release depends on it.
- **Documentation and repairs:**
  - `docs/publishing.md` and `docs/public-api.md` describe the stream and the shared packages'
    compatibility promise: semantic versioning, the baseline, package validation, and the changelog
    headings.
  - `scripts/packaging/test_pristine_consumer.py`'s stale dependency expectations are repaired, and
    it gains the shared packages.
  - `THIRD_PARTY_NOTICES.md` gains the shared libraries' section.

*Done when* the probe packs, restores in a pristine consumer from a local feed, and passes package
validation's first-version run.

### SJL-2.14 — The Native AOT probe

An executable in the shared solution, published with Native AOT using the settings of Broiler.VM's
roots: full trimming, `InvariantGlobalization`, `UseSystemResourceKeys` and warnings as errors. It
runs the library-level vector runner over the whole corpus, with the data-backed libraries over
tables embedded in the probe. It runs in the home's CI on every change to a shared library.
Reference arms stay in the JIT test project and are never part of the probe.

*Done when* it publishes with zero warnings and runs on the home's CI runners.

### SJL-2.15 — Broiler.JSeal: not needed

[SJL-D-2](../decisions.md#sjl-d-2) took the neutral prefix `Broiler.ECMAScript`, outside both of
the families Broiler.JSeal's `eng/package-gate.mjs` knows, so the gate needs no revision.

*Done when* the bundle records that it was not needed.

*(Revised 2026-10-11, when SJL-D-2 was taken. The work package revised the gate "Only if SJL-D-2
chose a prefix inside the `Broiler.JavaScript.*` or `Broiler.VM.*` family".)*

---

## Deliverables

**Broiler.VM:**

- the allow-list;
- rules `I1`, `I2`, `I3`, `I6` and `I7`, minted;
- rules A2, B3 (activated), N1, N37, A7 with `graph.manifest.json`, and E4, revised;
- the ADR 0001 and ADR 0012 revisions;
- the `Applied` register rows and the revised roadmap sentences;
- the source mapping and lock files;
- the tooling changes;
- the probe runs;
- the JSD on `packages.md`.

**Broiler.JS:** the shared folder, the conventions, the `SL` rules, the API baseline, the assurance
gate, the version stream, the prefix reservation, the probe pre-release, the Native AOT probe, and
the publishing and public-API documents.

---

## Exit gate

**Broiler.VM clauses**

1. Every rule of SJL-2.1 to SJL-2.6 is in the register, with its test and witnesses, each watched
   failing on injection and passing on revert. Rule J12 holds over the new rows.
2. The admission probe passes `I1`, `I2` and `I6`. Every negative control of SJL-2.9 fails.
3. The ADR 0001 and ADR 0012 revisions are filed with what each section said before.
4. Every register row filed at SJL-0 is `Applied`, and its sentence is revised in place with the
   superseded text.
5. A restore through each configured source succeeds, and an unmapped identifier's restore fails.
   The collectors record `I7` identity.
6. **No product project references a shared package.**

**Broiler.JS clauses**

7. The shared folder builds under the conventions. SL1 to SL10 are `Active`, with witnesses.
8. The API baseline and the assurance gate run in CI, and each is watched failing on an injected
   drift.
9. The probe pre-release packs and restores in a pristine consumer, and passes package validation's
   first-version run.
10. The Native AOT probe publishes with zero warnings and runs.

**Component clauses**

None. *(Revised 2026-10-11, when SJL-D-2 was taken: the conditional clause 11 read "If needed,
Broiler.JSeal's gate passes with the shared probe in both providers' graphs." The neutral prefix
makes it unneeded.)*

---

## Evidence bundles

- **Broiler.VM `docs/evidence/sjl-2-001/`:** the witness runs, failing and passing; the probe's
  positive and negative runs; the restore logs per source; the register diff; the revisions.
- **Broiler.JS `docs/evidence/sjl-2-001/`:** the `SL` witness runs; the baseline and assurance drift
  controls; the pack, restore and package-validation logs; the Native AOT publish and run logs.

---

## Does not do

- **It moves no algorithm** and puts no shared package into any product project.
- **It publishes nothing** the home's own release rules do not allow.
- **It changes no Broiler.VM composition's closure**, so it publishes and runs nothing beyond the
  probe.
- **It does not admit the frontend, or anything a data area brings.** Those are added to the
  allow-list by the milestones that cut over to them.
