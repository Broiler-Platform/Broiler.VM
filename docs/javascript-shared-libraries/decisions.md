# Decision points

**Date:** 2026-10-11. **State, later on 2026-10-11: every decision is taken, and Broiler.VM's records
of the decisions milestone SJL-0 records are filed; [the ledger](status.md) says which.** *(Revised
2026-10-11, at SJL-0's first records: this read "every decision is taken, and no record is
filed.")* A decision point is a place where two or more routes are admissible under the published
rules and a milestone clause cannot be met until one is chosen. This document states each question,
its alternatives, a recommendation and its holder, and ends each section with the decision taken.

**How the decisions were taken.** The owner, who holds every role named below, took the
recommendation of every decision that has one. Three decisions had no recommendation that chose one
alternative: SJL-D-13 and SJL-D-14 left (a) or (b) to a comparison, and SJL-D-23 left its answer to
the pilot's evidence. The owner chose (a) for all three, and (i), as recommended, for SJL-D-14's
duplicate named groups. Each section keeps its question, alternatives and recommendation as the
record of what was weighed.

- A decision is **taken** when its holder chooses. The choice is entered at the end of its section,
  dated, and the plan follows it from that date.
- A decision is **recorded** when the record its section names is filed in the repository that
  holds that record. A gate clause that needs a decision is met only by the filed record, never by
  this document. [The ledger](status.md) shows both states, and moves the second in the change that
  files the record.
- A taken decision is reopened only by a new dated choice, entered beside the first.
- **Every holder named below is one person on this date**, recorded in Broiler.VM as EX-30, and
  every record states that non-independence in its own words.

*(Superseded 2026-10-11, when every decision was taken. The opening read "**State of every decision
on this date: open.**" and "**It takes no decision.**" The first two rules read "A decision is taken
by filing the record named in its row. [The ledger](status.md) then moves the decision's state, in
the same change." and "Until a decision is taken, the clause that needs it is `Blocked` and names the
holder. The rest of the milestone proceeds.")*

---

## Summary

"Record filed by" names the work packages that file each decision's records. The record is due
before the milestone that first needs the decision meets its gate.

| # | Decision | Taken 2026-10-11 | Holder | Record filed by | Deadline |
|---|---|---|---|---|---|
| [SJL-D-1](#sjl-d-1) | Where each library lives | (b) component-aligned; locale data (b-ii) and time data (b-iii) in the Broiler.JS repository | Broiler.VM architecture owner, Broiler.JS maintainer, component maintainers | SJL-0.2, SJL-0.5, SJL-0.6 | (none) |
| [SJL-D-2](#sjl-d-2) | Package identity and granularity | (a) `Broiler.ECMAScript.*`, one package per boundary | Release owners of the home repositories, with the Broiler.VM release owner (R6) | SJL-0.2, SJL-0.5 | (none) |
| [SJL-D-3](#sjl-d-3) | Versioning, compatibility and co-hosting | A separate stream; minimum ranges with lock files; the compatibility minimum | Same | SJL-0.2, SJL-0.5, SJL-2.13 | (none) |
| [SJL-D-4](#sjl-d-4) | How Broiler.VM's CI obtains shared packages | (c) a committed feed in development; nuget.org for anything a release depends on | Broiler.VM release owner (R6) | SJL-0.2 | (none) |
| [SJL-D-5](#sjl-d-5) | Review and assurance regime for shared code | (a) full parity, with the release link | Broiler.VM release owner (R6), Broiler.JS maintainer | SJL-0.2, SJL-0.5 | (none) |
| [SJL-D-6](#sjl-d-6) | Data versions and data packaging | (c) shared generators and pins; each engine embeds its tables | JavaScript profile owner, Broiler.JS maintainer, Broiler.Unicode maintainer | SJL-0.5, SJL-0.6, SJL-7.1 | (none) |
| [SJL-D-7](#sjl-d-7) | Re-scope of JS-2: the seed copy is not taken | (a) re-scope | JavaScript profile owner | SJL-0.4 | **2026-11-30** for the record (JSD-0005's stop condition) |
| [SJL-D-8](#sjl-d-8) | Scope reading of ADR 0011's extraction gate and standing refusals | (a) a scope reading, as a route | Core-contract owner (R5) with the architecture owner | SJL-0.2, SJL-0.3 | (none) |
| [SJL-D-9](#sjl-d-9) | Fork policy: fixes flow through the libraries; evidence stays where it is collected | The four-part policy | Architecture owner, JavaScript profile owner, Broiler.JS maintainer | SJL-0.2, SJL-0.4, SJL-0.5 | (none) |
| [SJL-D-10](#sjl-d-10) | Shape of the metering contract | (a) the `WorkCharge` delegate | JavaScript profile owner, Broiler.JS maintainer | SJL-2.10, SJL-3.1 | (none) |
| [SJL-D-11](#sjl-d-11) | Diagnostic codes and message text | Codes and message identifiers; each engine renders its own text | JavaScript profile owner, Broiler.JS maintainer | SJL-12.2 | (none) |
| [SJL-D-12](#sjl-d-12) | Canonical syntax tree, and the compatibility of Broiler.JS's public `Ast` and `Parser` | (a) a new immutable tree, with (i) adapter packages | Broiler.JS maintainer, JavaScript profile owner | SJL-12.2, SJL-14.4 | (none) |
| [SJL-D-13](#sjl-d-13) | Broiler.VM's slice front-end path: migrate or retire | (a) rebuilt on the shared tree and model | JavaScript profile owner | SJL-15.4 | (none) |
| [SJL-D-14](#sjl-d-14) | Implementation base of the shared regular-expression engine, and duplicate named groups | (a) Broiler.Regex is fixed, then both engines move onto it; (i) duplicate named groups in it | Broiler.RegEx maintainer, JavaScript profile owner, Broiler.JS maintainer | SJL-0.6, SJL-11.1 | (none) |
| [SJL-D-15](#sjl-d-15) | BigInt width policy for Broiler.JS | (a) unbounded | Broiler.JS maintainer | SJL-4.1 | (none) |
| [SJL-D-16](#sjl-d-16) | Observable corrections at cut-over | (a) corrections are fixes; (c) only with a known dependent consumer | Broiler.JS maintainer, JavaScript profile owner | SJL-0.5, SJL-3.1, each area | (none) |
| [SJL-D-17](#sjl-d-17) | Broiler.JS's non-standard extensions stay in Broiler.JS | They stay | Broiler.JS maintainer | SJL-0.2, SJL-0.5 | (none) |
| [SJL-D-18](#sjl-d-18) | Whole-runtime unification is not planned | Not planned | Architecture owner, Broiler.JS maintainer | SJL-0.2; confirmed at SJL-16 | (none) |
| [SJL-D-19](#sjl-d-19) | Host policy as provider interfaces: clock, local time zone, default locale, available locales | Provider interfaces; each engine keeps its policy | JavaScript profile owner, Broiler.JS maintainer | SJL-6.1 | (none) |
| [SJL-D-20](#sjl-d-20) | Time-zone dataset policy | Broiler.VM's policy, at the newest release both engines can adopt | JavaScript profile owner, Broiler.JS maintainer | SJL-8.1 | (none) |
| [SJL-D-21](#sjl-d-21) | Calendar sources outside published years | Broiler.VM's ICU4X sources; no .NET calendar class | JavaScript profile owner, Broiler.JS maintainer | SJL-8.1 | (none) |
| [SJL-D-22](#sjl-d-22) | Locale coverage and the owner of CLDR data | (a) Broiler.VM's generator in the Broiler.JS repository, with (c) Broiler.JS's host provider | JavaScript profile owner, Broiler.JS maintainer, Broiler.Unicode maintainer | SJL-9.1 | (none) |
| [SJL-D-23](#sjl-d-23) | Continue past the pilot | (a) continue, taken before the pilot | Architecture owner, Broiler.JS maintainer | SJL-0.2, SJL-0.4 | (none) |
| [SJL-D-24](#sjl-d-24) | Collation | (c) shared later, by SJL-17 after SJL-9 | JavaScript profile owner, Broiler.JS maintainer | SJL-9.1 | (none) |
| [SJL-D-25](#sjl-d-25) | Disposition of Broiler.JS public types an area empties | Forwarders by default | Broiler.JS maintainer | SJL-3.5, then each area | (none) |

---

## Programme decisions

<a id="sjl-d-1"></a>
### SJL-D-1 — Where each library lives

**Question.** For each logical library in [architecture.md](architecture.md#2-the-library-catalogue),
which repository owns its source, tests, data pins and releases?

**Why it matters.** The home sets several things at once:

- who may change the library, and under which review and release rules;
- how many repositories a fix must cross before both engines have it;
- whether an existing published component is extended or duplicated.

The feasibility report asks that Broiler.JS become the home. It also says existing libraries
"should be evaluated as possible owners", and that "Repository consolidation should not create
another version of an already reusable library."

**Alternatives.**

- **(a) Everything in the Broiler.JS repository.** One home and one release stream for all
  libraries. This duplicates what Broiler.Unicode and Broiler.Regex already publish, and Broiler.JS
  already consumes both.
- **(b) Component-aligned.** Generic Unicode and regular-expression work goes in Broiler.Unicode
  and Broiler.Regex. ECMAScript-specific algorithms, the frontend, Intl algorithms, time data and
  vectors go in the Broiler.JS repository.
- **(c) A new neutral repository** for every shared library. This gives the cleanest ownership
  boundary, but adds a repository, a release stream and a review regime, and still duplicates the
  component libraries unless they move into it.

**Recommendation: (b).** Three areas stay open inside it:

- **Locale data.**
  - Option (b-i): extend `Broiler.Unicode.Cldr.LocaleData` until it carries the tables the Intl
    library needs, in that library's provider format, with a public CLDR version and hashed inputs.
  - Option (b-ii): move Broiler.VM's CLDR generator and pins into the Broiler.JS repository as a
    new data package.

  Settled at SJL-9 together with [SJL-D-22](#sjl-d-22).
- **Time data.**
  - Option (b-iii): the Broiler.JS repository, recommended.
  - Option (b-iv): Broiler.DateTime, which has no consumer today and no time-zone support.

  Settled at SJL-8.
- **Normalization, casing and segmentation.** Broiler.Unicode has none of these today. They go
  there only if [SJL-7](milestones/sjl-7-unicode.md)'s owner conditions are met. Otherwise they
  go into the Broiler.JS repository, and the record says why.

**Holder.** The Broiler.VM architecture owner, the Broiler.JS maintainer, and the maintainer of each
component repository for its own area. **Needed before** SJL-2 for the algorithms, the frontend and
the vectors; before SJL-7, SJL-8 and SJL-9 for data. **Record:** ADR 0014's consequences section,
and an entry in each home repository's roadmap.

**Taken 2026-10-11: (b), as recommended, with its open areas settled.**

- **Locale data: (b-ii).** Broiler.VM's CLDR generator and its pins move into the Broiler.JS
  repository. This follows from [SJL-D-22](#sjl-d-22) (a), taken the same day.
  `Broiler.Unicode.Cldr.LocaleData` is not extended for this programme.
- **Time data: (b-iii).** The Broiler.JS repository. Broiler.DateTime is not a home in this
  programme, and no milestone changes it.
- **Normalization, casing and segmentation:** the rule above stands. SJL-7.1 places them with
  Broiler.Unicode if it meets that work package's owner conditions, and otherwise in the Broiler.JS
  repository, with the record saying why.

Under [SJL-D-6](#sjl-d-6) (c), a data home holds the generator and the pins, and each engine embeds
the tables. **Recorded by** ADR 0014 (SJL-0.2) and the home repositories' roadmaps (SJL-0.5,
SJL-0.6).

<a id="sjl-d-2"></a>
### SJL-D-2 — Package identity and granularity

**Question.** What are the shared packages called, and how many are there?

**Constraints found on this date:**

1. **Broiler.JSeal's package gate** (`eng/package-gate.mjs`) treats `Broiler.JavaScript.*` and
   `Broiler.VM.*` as engine families: "one version per family within and across graphs; neither
   provider may depend on the other family". A shared package named into one family and consumed by
   the other engine would fail it.
2. **Broiler.JS's `eng/pack.ps1`** requires one version for every package it packs ("Package
   versions must agree").
3. **Broiler.VM's rule A2** tests the `Broiler.` prefix, and rule N37 tests produced dependencies
   against `Broiler.VM.`.
4. **Broiler.VM's roadmap section 5** accepts an assembly only when it enforces a boundary.

**Alternatives.**

- **(a) A neutral prefix, one package per boundary.** `Broiler.ECMAScript.Algorithms`, `.Intl`,
  `.Frontend`, `.TimeData`, and `.Conformance` (test-only).
- **(b) A neutral prefix, one package per feature.** `.Numerics`, `.Text`, `.Time`, `.Temporal`,
  `.Syntax`, `.Semantics` and others. This creates more packages without more boundaries.
- **(c) `Broiler.JavaScript.Shared.*`.** This keeps the Broiler.JS family name, so JSeal's gate
  must be revised, and the packages fall under Broiler.JS's single-version rule.

**Recommendation: (a).** Reserve the prefix on nuget.org as a supply-chain measure (see
[governance.md](governance.md#8-security)). The prefix name itself, `Broiler.ECMAScript`, is
negotiable; the neutrality is not.

**Holder.** The home repositories' release owners, with Broiler.VM's R6. **Needed before** SJL-2.
**Record:** ADR 0014, and the Broiler.JS shared-libraries plan.

**Taken 2026-10-11: (a), as recommended, with the prefix `Broiler.ECMAScript`.** The packages are
`Broiler.ECMAScript.Algorithms`, `.Intl`, `.Frontend` and `.Conformance` (test-only).
`Broiler.ECMAScript.TimeData` and `Broiler.ECMAScript.IntlData` are the identities reserved for
data packages, which [SJL-D-6](#sjl-d-6) (c) does not publish. The prefix is reserved on nuget.org
before anything under it is published (SJL-2.13). Broiler.JSeal's gate needs no revision.
**Recorded by** ADR 0014 (SJL-0.2) and the Broiler.JS shared-libraries plan (SJL-0.5).

**Revised 2026-10-12, at SJL-2, by the owner: the prefix reservation is withdrawn.** No reservation of
`Broiler.ECMAScript` on nuget.org is required, and nothing under the prefix waits for one before it is
published. The rest of (a) stands: the prefix, the package identities and Broiler.JSeal's unrevised
gate. The supply-chain measures that remain are those of
[governance.md](governance.md#8-security): package source mapping, lock files with content hashes
held to rule `I1`'s pins, nuget.org repository signatures, Source Link and deterministic builds, and
the upstream commit rule `I7` records. The taken choice's sentence this supersedes read "The prefix
is reserved on nuget.org before anything under it is published (SJL-2.13)." The holder is the same
one person (EX-30). **Recorded by** ADR 0014's revision of this date and the Broiler.JS
shared-libraries plan.

<a id="sjl-d-3"></a>
### SJL-D-3 — Versioning, compatibility and co-hosting

**Question.** How are shared packages versioned and kept compatible? How do the two engines depend
on them, so that one process can host both?

**Why it matters.** NuGet resolves one version of each package for a whole application. A process
hosting both engines therefore runs both on one copy of each shared assembly: Broiler.JSeal's tests,
Broiler.HtmlBridge, and the browser through HtmlBridge. See
[architecture.md](architecture.md#63-a-process-that-hosts-both-engines).

**Alternatives.**

- **Version stream.**
  - (a) A separate stream for the shared packages, with its own solution filter, pack invocation
    and tag prefix in the home repository.
  - (b) Lockstep with Broiler.JS's engine packages, as Broiler.JS's pack rule does today. Every
    engine release would then move Broiler.VM's dependencies, whether or not a library changed.
- **Engine dependency ranges.**
  - (a) Minimum versions in nuspecs, exact versions in each engine's lock files.
  - (b) Exact versions in nuspecs. This forces lockstep releases of both engines and makes
    co-hosting fail whenever they differ.
- **Compatibility.** Semantic versioning, an API baseline, package validation against the last
  published version, and a changelog with an "Observable changes" heading. These are not
  alternatives; they are the minimum.

**Recommendation:** a separate stream, minimum ranges plus lock files, and the full compatibility
minimum. Binary compatibility inside a major version is mandatory. A spec correction is a minor
version with its own changelog entry. A data-version change is a minor version with its own entry,
unless [SJL-D-6](#sjl-d-6) puts the data version into the package identity.

**Holder.** The home repositories' release owners, with Broiler.VM's R6. **Needed before** SJL-2.
**Record:** ADR 0014; Broiler.JS `docs/publishing.md`; the shared-libraries plan.

**Taken 2026-10-11, as recommended:** a separate version stream; minimum versions in nuspecs and
exact versions in each engine's lock files; and the full compatibility minimum. Under
[SJL-D-6](#sjl-d-6) (c) no data version enters a package identity, so a data-version change is a
minor version with its own changelog entry. **Recorded by** ADR 0014 (SJL-0.2), the shared-libraries
plan (SJL-0.5) and Broiler.JS's `docs/publishing.md` (SJL-2.13).

<a id="sjl-d-4"></a>
### SJL-D-4 — How Broiler.VM's CI obtains shared packages

**Question.** Where does Broiler.VM restore a shared package from?

**Facts.**

- Broiler.VM's `NuGet.config` clears every source and maps everything to nuget.org.
- The pristine JavaScript consumer restores from `../artifacts/feed` only. Its collection makes the
  upstream unreachable on purpose.
- `verify-feed.ps1` requires every dependency outside the release to be on nuget.org already.
- No repository uses an extra restore source. The `D:\local-packages` folder is ad hoc and
  referenced by no configuration.

**Alternatives.**

- **(a) Published previews only.** A shared change reaches Broiler.VM only after it is published
  to nuget.org. Broiler.VM's development waits on the home repository's release cadence.
- **(b) A committed, hash-pinned local feed** inside Broiler.VM. Package source mapping sends the
  shared prefix to it; lock files run in locked mode. This matches how Broiler.VM already archives
  test262, the tz database and the calendar crates. It adds binary files to the repository.
- **(c) Both:** (b) while an area is in development, and (a) for anything a Broiler.VM package
  release depends on.

**Recommendation: (c).** Rule `I1` reads the lock files' content hashes in every case.

**Holder.** Broiler.VM's R6. **Needed before** SJL-2. **Record:** ADR 0014, plus a route row in
`docs/mvp.md` if the route is taken before the owner records it.

**Taken 2026-10-11: (c), as recommended.** A committed, hash-pinned feed in Broiler.VM serves an
area while it is in development. Anything a Broiler.VM package release depends on is on nuget.org
first. Rule `I1` reads the lock files' content hashes in both cases. **Recorded by** ADR 0014
(SJL-0.2). No route row is needed, because ADR 0014 records the decision before SJL-2 takes the
route.

<a id="sjl-d-5"></a>
### SJL-D-5 — Review and assurance regime for shared code

**Question.** Which assurance scheme governs shared code? Does a Broiler.VM release that ships a
shared package require that package's revision to carry a completed review?

**Facts.**

- Broiler.VM's rules J1 to J12 cover only Broiler.VM's own sources.
- Broiler.JS opts into the same annotation scheme, but its annotations are partial, its units file
  is not generated, and no CI lane enforces it.
- Broiler.RegEx has generated artefacts and no enforcement. Broiler.Unicode has none.
- No repository involved has a human review current at its head.

**Alternatives.**

- **(a) Full parity.** The home repositories annotate every relevant unit, generate the artefacts
  and gate on them in CI, from a library's first commit. A Broiler.VM release reads the upstream
  revision's review state through rule `I7`'s recorded commit and refuses on an incomplete review.
- **(b) Annotation without the release link.** As (a), but Broiler.VM's release gate does not read
  upstream review state. A shared library then ships in Broiler.VM unreviewed.
- **(c) Defer.** Shared code carries no assurance until a later decision. Broiler.VM's assurance
  coverage shrinks with every area moved, and nothing replaces it.

**Recommendation: (a).** Whichever is chosen, the bundle that moves code out of Broiler.VM defines
an exclusion, numbered at filing time (EX-125 on this date), saying what left and where it is
covered.

**Holder.** Broiler.VM's R6 and the Broiler.JS maintainer. **Needed before** SJL-2 for new projects
and SJL-3 for the first product cut-over. **Record:** ADR 0014; the Broiler.JS shared-libraries
plan.

**Taken 2026-10-11: (a), as recommended.** Each home repository annotates every relevant unit,
generates the artefacts and gates on them in CI, from a library's first commit. A Broiler.VM release
reads the upstream review state through rule `I7`'s recorded commit, and refuses an admitted package
whose revision lacks a completed review. **Recorded by** ADR 0014 (SJL-0.2) and the Broiler.JS
shared-libraries plan (SJL-0.5).

<a id="sjl-d-6"></a>
### SJL-D-6 — Data versions and data packaging

**Question.** Which Unicode, CLDR, tz database and calendar versions do the shared libraries carry?
How are they packaged, and how does a version move?

**Facts.**

| Data | Broiler.VM | Broiler.JS |
|---|---|---|
| Unicode | 17.0.0, pinned, hashed, generated, held byte for byte by rule N22 | Host ICU or .NET runtime data for normalization, segmentation and identifiers; 17.0 tables from the Broiler.Unicode packages for regular-expression properties and one-to-many case mappings |
| CLDR | 48.2.0, pinned, generated, rules N27 and N28 | 48.2.0 through `Broiler.Unicode.Cldr.LocaleData`; the version appears only in internal classes, and its inputs are downloaded unhashed |
| tz database | 2026e, compiled in the repository, rules N29 and N30 | 2025b, copied from a build host's compiled zoneinfo, unpinned |

**Alternatives for packaging.**

- **(a) One package per data set, latest data.** Both engines move together, and a host that
  resolves a newer version changes both engines' behaviour.
- **(b) The data version in the package identity**, for example a Unicode 17 data package. Hosts
  can load two data versions side by side, at the cost of more package identities over time.
- **(c) Share the generator and the pins, not the bytes.** Each engine embeds tables generated by
  the shared generator from shared pins. This shares the logic, not the payload, and keeps
  Broiler.VM's current byte-for-byte rules unchanged.
- **(d) Any of the above, plus a runtime assertion** that turns a data-version mismatch into a
  deterministic refusal at composition time.

**Alternatives for moving a version.** In Broiler.VM, a re-pin "is a recorded decision, not an
update". Its Unicode pin moves with its test262 pin, and its CLDR pin moves with Unicode when the
collation algorithm's version moves. The shared libraries adopt the same rule. A version move is a
dated record in the home repository, followed by one change per engine.

**Recommendation.** Use (c) for the first data area. It changes the least and shares the most
error-prone part, which is generation. Moving to (a) or (b) is a later decision once both engines
have consumed shared tables for one release. **For Broiler.JS, adopting pinned data in place of
host data is an observable change**, decided under [SJL-D-16](#sjl-d-16).

**Holder.** The JavaScript profile owner, the Broiler.JS maintainer and the Broiler.Unicode
maintainer. **Needed before** SJL-7. **Record:** a JSD, the Broiler.JS plan, and each data home's
roadmap.

**Taken 2026-10-11: (c), as recommended, from the first data area, SJL-7, onward.**

- **The data home holds the pins and the generators.** Each engine embeds the tables they generate,
  in assemblies of its own, and references no package that carries them.
- **Shared code reads the tables through a provider** that hands it the generated bytes, in the
  shape of Broiler.VM's `IJsIntlData` today: a version, and one byte span per table. Decoding stays
  in shared code.
- **Broiler.VM's byte-for-byte rules stay in Broiler.VM** (N22, N28, N30, N32). They regenerate its
  tables with the home's generator from Broiler.VM's own pins, and rule `I4` holds those pins equal
  to the home's. The generator reaches Broiler.VM's tests pinned like an admitted package, and never
  enters a product closure.
- **A process hosting both engines runs each on its own tables**, so a host's package resolution
  cannot move either engine's data.
- **Moving to (a) or (b) is a later dated decision**, admissible once both engines have consumed
  shared tables for one release.
- **For Broiler.JS**, every behaviour that changes because pinned data replaces host data is a
  correction under [SJL-D-16](#sjl-d-16).

**Recorded by** the Broiler.JS plan and the data homes' roadmaps (SJL-0.5, SJL-0.6), and the
area-verdict JSD of SJL-7 (SJL-7.1).

<a id="sjl-d-7"></a>
### SJL-D-7 — Re-scope of JS-2: the seed copy is not taken

**Question.** Does Broiler.VM's JavaScript profile still take a snapshot copy of Broiler.JS, as its
milestone JS-2 plans, or does it consume the shared libraries instead?

**Facts.**

- JS-2 is `Blocked`, and its ledger says "No snapshot has been taken". The candidate seed is
  Broiler.JS commit `0341e5c…`.
- JSD-0005's stop condition takes the snapshot as-is on 2026-11-30, or earlier after a set number
  of further seed commits.
- JS-2 settles when "either the core contract is accepted, or a record says the seed copy will not
  be taken and JS-2's gate is re-scoped".
- Both of Broiler.VM's front ends were written in its own checkout. The ledger says "The general
  front end is still the ingest's and still waits."
- The seed's submodule layout, which JS-2's identity records, no longer exists at Broiler.JS's
  head.

**Alternatives.**

- **(a) Re-scope.** A JSD records that the seed copy will not be taken. JS-2's gate is re-scoped to
  consuming the shared frontend (SJL-13), and its other copy rows are withdrawn: storage,
  registration generator, static analysis. The "acquire as this checkout's own dependencies" row is
  fulfilled by admitted component packages.
- **(b) Keep the copy.** The snapshot is taken at the stop condition. The programme then shares
  code with a fork of itself, and every shared area has a third copy until the fork's copy is
  deleted.
- **(c) Defer.** A JSD revises JSD-0005's stop condition to a later date or a programme milestone,
  so the snapshot is not taken by default while the programme decides.

**Recommendation: (a)**, or (c) if SJL-0 cannot finish before the deadline. **Taking no decision is
not an option**: the stop condition then takes the snapshot on 2026-11-30 by default.

**Holder.** The JavaScript profile owner. **Needed before** SJL-0's exit, and **before
2026-11-30**. **Record:** a new JSD superseding JSD-0005's stop condition, and JSC entries for the
roadmap sentences it changes.

**Taken 2026-10-11: (a), as recommended.** The seed copy will not be taken, and JS-2's gate is
re-scoped to consuming the shared frontend (SJL-13). **Taking the decision here does not disarm
JSD-0005's stop condition; only a filed JSD does.** SJL-0.4 files that JSD before 2026-11-30. If it
cannot, a JSD revising JSD-0005's stop condition to a programme milestone, the recommendation's
fallback (c), is filed before that date instead. **Recorded by** the profile JSD and its JSC
entries (SJL-0.4).

<a id="sjl-d-8"></a>
### SJL-D-8 — Scope reading of ADR 0011's extraction gate and standing refusals

**Question.** Do ADR 0011's extraction gate (G1 to G4) and its standing refusals apply to a
JavaScript-only library shared with **another engine**? The refusals include "A shared syntax tree
or parser — Refused permanently" and "Lexing, source positions, diagnostic formatting — Waits for a
second text front end".

**Facts.**

- ADR 0011 is contract-bearing. Editing its decision text would be read as an amendment, as the
  MVP-12 precedent shows.
- The amendment procedure is unexecutable while one person holds R5 and both co-signing roles
  (EX-15).
- The gate's conditions concern Broiler.VM **product profiles**: G1 asks for two profiles
  implementing the behaviour, and G4 for no profile-to-profile edge. Its record is "filed in this
  ADR set because it changes the core graph".
- Broiler.VM's roadmap section 8 explains the refused shared parser as one that "would encode one
  language's grammar into the core's namespace".

**Alternatives.**

- **(a) Scope reading, as a route.** The gate and the refusals govern sharing between Broiler.VM
  profiles and placement in the core. An external dependency admitted to one profile family is
  neither. ADR 0014 records the reading. A route row in `docs/mvp.md` names the alternative not
  taken and the ruling that would settle it. ADR 0011 gains an editorial pointer paragraph and no
  edit to its table.
- **(b) Amendment.** Blocked under EX-15. The programme would wait for the amendment procedure to
  become executable.

**Recommendation: (a).** If the core-contract owner rules that the reading is wrong, SJL-0 is
`Blocked` naming the core as holder, and the programme stops there.

**Holder.** The core-contract owner (R5) with the architecture owner. **Needed before** SJL-0's
exit. **Record:** ADR 0014; a route row; the ADR 0011 pointer paragraph.

**Taken 2026-10-11: (a), as recommended,** by the core-contract owner with the architecture owner,
one person under EX-30. ADR 0011's decision text is not edited. A later ruling that the reading is
wrong, for example by a second holder once one exists, would still block SJL-0 and stop the
programme, as the recommendation says. **Recorded by** ADR 0014 (SJL-0.2), and the route row and
ADR 0011's pointer paragraph (SJL-0.3).

<a id="sjl-d-9"></a>
### SJL-D-9 — Fork policy: fixes flow through the libraries; evidence stays where it is collected

**Question.** Two current positions conflict with the programme:

- Broiler.VM's roadmap: "fixes do not flow across the fork in either direction".
- The profile's invariant 11: "neither side is the other's upstream".

What replaces them?

**Recommendation.** The replacement has four parts:

- **Fixes to a shared area flow through the shared library**, in both directions. The shared
  library is upstream of both engines.
- **Engine code still does not flow between the engines.** Neither engine is the other's upstream.
  A defect found in one engine's own code is that engine's to fix.
- **Evidence still does not transfer.** Each repository's gate is met by evidence that repository
  collected. Agreement between engines is a cross-check and never an oracle.
- **No local patches.** A defect in a shared library is never patched inside an engine. An engine
  may pin back to the previous release. A temporary adapter workaround needs a dated correction
  entry naming the upstream defect and the release that removes it.

**Alternatives.** Keeping the current text is not admissible once a shared library exists: it would
forbid fixing the library. A weaker policy that lets engines patch locally recreates the
duplication the programme removes.

**Holder.** The architecture owner, the JavaScript profile owner and the Broiler.JS maintainer.
**Needed before** SJL-0's exit. **Record:** ADR 0014; the profile JSD; the Broiler.JS plan.

**Taken 2026-10-11, as recommended:** the four parts above replace both positions. **Recorded by**
ADR 0014 (SJL-0.2), the profile JSD (SJL-0.4) and the Broiler.JS plan (SJL-0.5).

---

## Pilot-time decisions

<a id="sjl-d-10"></a>
### SJL-D-10 — Shape of the metering contract

**Question.** How does a shared operation charge work to its caller's budget?

**Facts.**

- Broiler.VM charges through `Action<ulong>`, as in `JsBigInt`, whose remark reads "The charge goes
  to a delegate so that the arithmetic holds no engine and can be judged on its own", and through
  `JsRegExpCharge` in the matcher. Its charge polls cancellation and the wall clock and may throw `JsAbort`. The matcher
  charges in blocks.
- Broiler.JS meters nothing in these areas.

**Alternatives.**

- **(a) A delegate**, `public delegate void WorkCharge(ulong units)`, nullable for "no metering".
  It is simple and familiar, with an indirect call per charge. Operations charge in blocks where
  steps are fine-grained.
- **(b) A generic struct meter.** `TMeter : struct, IWorkMeter`, with the meter passed by
  reference. A no-op meter costs nothing after specialisation. It complicates every public
  signature.
- **(c) An interface instance.** Similar to (a), with a virtual call.

**Recommendation: (a), with block charging documented per operation.** Move to (b) only for an
operation whose candidate-versus-control measurement, under a predeclared rule, shows the delegate
costs too much. In every alternative, the charge happens before the work, and the operation is
safe if the charge throws.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-3.
**Record:** the shared library's conventions document, and the pilot's JSD.

**Taken 2026-10-11: (a), as recommended.** `WorkCharge` is a nullable delegate, charged before the
work, and each operation documents its units and block size. Moving one operation to (b) is a later
decision, on that operation's candidate-versus-control measurement under a predeclared rule.
**Recorded by** the conventions document (SJL-2.10) and the pilot's JSD (SJL-3.1).

<a id="sjl-d-23"></a>
### SJL-D-23 — Continue past the pilot

**Question.** After SJL-3, did one shared algorithm make both engines simpler? Does the programme
continue?

**Criteria.** These are the feasibility report's own words, made decidable:

- every member on the pilot's deletion list is gone from both engines;
- each adapter holds no algorithm;
- no intra-engine duplicate of the area survives;
- both engines' conformance evidence meets the predeclared rule;
- each engine's own candidate-versus-control measurement meets its predeclared rule;
- no closure gained a forbidden assembly.

**Alternatives.**

- **(a) Continue** with the other area milestones.
- **(b) Continue with conditions:** a revised convention, a revised gate, or a narrower catalogue.
- **(c) Stop.** The pilot library stays, with both engines on it. The other areas are recorded as
  refused, with the reason, and their duplications carry source-level pointers to the record.

**Holder.** The architecture owner and the Broiler.JS maintainer. **Needed at** SJL-3's exit.
**Record:** ADR 0014 revision; the profile JSD.

**Taken 2026-10-11, before the pilot: (a), continue**, chosen by the owner. No alternative was
recommended, because the decision was planned for SJL-3's exit, on the pilot's evidence. Taken in
advance, it changes the pilot's role:

- **SJL-3 is no longer a point at which the programme can stop.** The six criteria above stay, as
  SJL-3's exit clauses. A criterion that fails keeps SJL-3 `In progress`, and stops nothing else.
- **A refusal of the pilot area** at SJL-3.1 ends that area only, as any area's refusal does.
- **Reopening the decision**, for example on the pilot's evidence, takes a new dated choice here.

**Recorded by** ADR 0014 (SJL-0.2), which carries it from the start, so no revision is needed, and
the profile JSD (SJL-0.4).

*(Noted 2026-10-20, which reopens nothing. SJL-3's clause 7, Broiler.JS's measurement, waits on a
controlled lane, which neither the programme's machine nor any GitHub-hosted runner is
(Broiler.JS `docs/roadmap/Measurement.md`, "Controlled lanes and effective configuration").
Clause 13, Broiler.VM's measurement, is read under a dated revision of its rule of the same date,
as weaker evidence, in the second and last measurement that revision allows
([bundle `sjl-3-001`](../evidence/sjl-3-001/README.md)); if that measurement does not meet it,
clause 13 stays open. SJL-4's clause 9 stays unmet by the owner's decision of the same date
([bundle `sjl-4-001`](../evidence/sjl-4-001/README.md), section 4). The next dated choice here is
taken when either (a) a controlled lane exists, recorded as `Measurement.md` requires; or (b)
SJL-3's clause 7, its clause 13 if still open, and SJL-4's clause 9 are the only unmet clauses of
SJL-3 to SJL-15 and SJL-17, every other clause being met on its bundles. At (b) the choice is
between waiting for a lane and a gate revision under alternative (b), which would be the first
exit-gate revision made after the evidence it concerns. Such a revision would take dated edits
here, in the milestone file with the superseded text quoted, in [the ledger](status.md) under its
update rule 5, and in Broiler.JS's plan pair. That choice is the owner's own when it comes, not one
taken under the owner's direction of 2026-10-18. This note records the owner's decision of
2026-10-20, taken under that direction, "Please do all recommended decisions and skip waiting for
review.", which changes no programme rule.)*

<a id="sjl-d-16"></a>
### SJL-D-16 — Observable corrections at cut-over

**Question.** The shared implementation often differs from an engine's current behaviour where that
engine is wrong. What happens to those differences?

**Facts.** The survey found, among others:

- in Broiler.JS: radix 0 and null treated as 10, `toPrecision` argument order, `parseInt(null, 36)`,
  culture-dependent `BigInt` strings, CRLF in indented JSON on Windows, a JSON nesting limit
  inherited from System.Text.Json, forgeable raw JSON, date-only ISO strings read as local time,
  `TimeClip` keeping −0, `1 ** NaN`, `selectRange` always `"other"`;
- in Broiler.VM: `parseInt` and hexadecimal-literal double rounding, `Math.sumPrecise` flushing a
  tiny term, possibly `String(2**-25)`.

**Alternatives.**

- **(a) Corrections are fixes.** A difference where the shared library follows the specification is
  accepted as a correction in the engine that was wrong. It is listed in that engine's changelog or
  ledger, and admitted by the area's comparison rule as its own class.
- **(b) Compatibility switches.** The old behaviour is kept behind an option. This keeps a second
  implementation, which is the duplication the programme exists to remove.
- **(c) Per-difference decisions.**

**Recommendation: (a)**, with (c) only for a difference with a known dependent consumer. Every
correction is listed by name in the cut-over's bundle. **A difference where the shared library is
wrong is a defect in the library**, fixed before the cut-over lands.

**Holder.** The Broiler.JS maintainer and the JavaScript profile owner. **Needed before** SJL-3.
**Record:** the pilot JSD; the Broiler.JS plan; each area's changelog.

**Taken 2026-10-11: (a), as recommended, with (c) only for a difference with a known dependent
consumer.** **Recorded by** the pilot JSD (SJL-3.1), the Broiler.JS plan (SJL-0.5) and each area's
changelog.

<a id="sjl-d-25"></a>
### SJL-D-25 — Disposition of Broiler.JS public types an area empties

**Question.** When an area moves an algorithm out of a public Broiler.JS type, what happens to that
type? Examples are `NumberParser`, `JSNumericOperators.ToInt32`, `JSNumberPrototype.DecimalToBase`
and `DateParser`.

**Facts.**

- Broiler.JS has no type forwarding anywhere and no API baseline.
- Its README calls the API unstable.
- Its Modernization invariant 7 gates assembly identity, type forwarding, package contents and
  pristine consumers.
- Its MOD-M0-4 leaves the type-forwarding question open.
- Downstream consumers are Broiler.JSeal and Broiler.HtmlBridge.

**Alternatives** per type: (a) a **forwarder** that keeps the member and calls the library; (b) an
**obsolete forwarder** with a removal version; (c) **removal** in a major version.

**Recommendation.** Default to (a) for every public member that downstream consumers or tests use.
Use (b) for the rest. Use (c) only with a recorded consumer check. The disposition of each member
is listed in the area's deletion list.

**Holder.** The Broiler.JS maintainer. **Needed before** SJL-3 for the first area, then for each
area. **Record:** Broiler.JS `docs/public-api.md`.

**Taken 2026-10-11, as recommended:** (a) for every public member that downstream consumers or tests
use, (b) for the rest, and (c) only with a recorded consumer check. **Recorded by** Broiler.JS's
`docs/public-api.md`, at SJL-3.5 and then at each area's cut-over.

<a id="sjl-d-17"></a>
### SJL-D-17 — Broiler.JS's non-standard extensions stay in Broiler.JS

**Question.** Do Broiler.JS's extensions enter the shared libraries? The extensions are:

- the `Decimal` type and `m` literals;
- `Number.prototype.clz` and `String.prototype.contains`;
- the .NET format-string extension to `toLocaleString`;
- the SpiderMonkey-style `N/N/N` date heuristics and the culture-dependent legacy `Date.parse`.

**Recommendation.** No. They stay in Broiler.JS's adapters or built-ins. Where one needs a shared
primitive, a parameter of a shared operation provides it, such as a lexer flag that admits the `m`
suffix. The shared libraries implement the specification only.

**Holder.** The Broiler.JS maintainer. **Needed before** SJL-0's exit. **Record:** the Broiler.JS
plan.

**Taken 2026-10-11, as recommended: no.** The extensions stay in Broiler.JS. **Recorded by** the
Broiler.JS plan (SJL-0.5), and listed in ADR 0014 (SJL-0.2).

<a id="sjl-d-18"></a>
### SJL-D-18 — Whole-runtime unification is not planned

**Question.** Does the programme lead to one shared value model, object model or runtime?

**Recommendation.** No, and it is recorded as **not planned**, not as deferred. The feasibility
report: "A universal value interface would affect allocation, dispatch, identity, exceptions and
performance. It would not automatically make the existing built-ins reusable." Broiler.JS's public
`JSValue` class hierarchy and Broiler.VM's internal tagged `JsValue` struct serve different engines
on purpose. Broiler.VM's JSD-0011 already rejects "Adopting the seed's boxed value hierarchy".
SJL-16 confirms the record, or reopens it only on new evidence.

**Holder.** The architecture owner and the Broiler.JS maintainer. **Needed before** SJL-0's exit.
**Record:** ADR 0014.

**Taken 2026-10-11, as recommended: not planned.** **Recorded by** ADR 0014 (SJL-0.2). SJL-16
confirms it, or reopens it only on new evidence.

---

## Area decisions

<a id="sjl-d-15"></a>
### SJL-D-15 — BigInt width policy for Broiler.JS

**Question.** Broiler.VM caps BigInt at a fixed bit width and reports over-width results as
`RangeError`, and it charges every operation. Broiler.JS has no cap and no metering, and some of
its BigInt paths raise .NET exceptions. The shared kernel takes a policy. Which policy does
Broiler.JS pass?

**Alternatives.**

- **(a) Unbounded**, meaning a ceiling as large as the representation allows, with a no-op meter.
  This keeps Broiler.JS's current width.
- **(b) The same ceiling as Broiler.VM.** Observable: very large BigInts become `RangeError`s.
- **(c) A Broiler.JS-specific ceiling.**

**Recommendation: (a)**, with the kernel's over-width status mapped to `RangeError` at the
representation's limit rather than to a .NET exception.

**Holder.** The Broiler.JS maintainer. **Needed before** SJL-4.

**Taken 2026-10-11: (a), as recommended.** **Recorded by** Broiler.JS's area record for SJL-4
(SJL-4.1).

<a id="sjl-d-19"></a>
### SJL-D-19 — Host policy as provider interfaces

**Question.** The engines differ on purpose:

| Policy | Broiler.VM | Broiler.JS |
|---|---|---|
| `Date` local time zone | UTC (JSD-0053 §7) | The host's `TimeZoneInfo.Local` |
| Clock resolution | Millisecond for `Temporal.Now` | 100-nanosecond |
| Default locale | Fixed `en-US` | The host's culture |
| Available locales | Its data | The host's cultures |

Do the shared libraries choose?

**Recommendation.** No. The libraries define provider interfaces (local time zone, clock, default
locale, available locales), and each engine supplies its own policy. Broiler.JS may keep its host
providers. **Its `Date` and its `Temporal` then stop using different time-zone data**, because both
read the provider: today `Date` uses the host's data and `Temporal` the bundled 2025b data.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-6.

**Taken 2026-10-11, as recommended:** provider interfaces, with each engine supplying its own policy.
**Recorded by** SJL-6's records (SJL-6.1).

<a id="sjl-d-20"></a>
### SJL-D-20 — Time-zone dataset policy

**Question.** What does the one shared time-zone dataset contain?

| Sub-question | Broiler.VM today | Broiler.JS today |
|---|---|---|
| Release | 2026e | 2025b |
| Backzone | Excluded (the default build) | Mostly included |
| Beyond the last explicit transition | Recurring rules extrapolated without limit | The last offset frozen, which is wrong after 2037 |
| Primary identifiers | From CLDR's `bcp47/timezone.json` | Link targets plus hard-coded aliases |
| `Factory` | Absent | Present |
| `Intl.DateTimeFormat`'s `timeZone` | Resolved to the primary; the `canonical-tz` test262 cases are skipped | Keeps the link name |
| Identifier lookup | ASCII case-insensitive | Invariant upper-casing |

**Recommendation.**

- Broiler.VM's policy: the default build without backzone, recurring rules, CLDR primaries, no
  `Factory`, ASCII lookup.
- Release: the newest release both engines can adopt together.
- The `canonical-tz` behaviour of `DateTimeFormat` is a separate decision. It follows the
  specification edition both engines pin.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-8.

**Taken 2026-10-11, as recommended:** Broiler.VM's policy; the newest release both engines can
adopt together, which SJL-8.1 pins by a dated record; and `canonical-tz` following the
specification edition both engines pin. **Recorded by** SJL-8's records (SJL-8.1).

<a id="sjl-d-21"></a>
### SJL-D-21 — Calendar sources outside published years

**Question.** Which sources do the calendars use?

| Calendar | Broiler.VM today | Broiler.JS today |
|---|---|---|
| Chinese and Korean | ICU4X's published years, with ICU4X's approximation outside them | .NET's lunisolar calendars in their range, and an astronomical computation outside |
| Persian | A 33-year rule with corrections from `calendrical_calculations` | .NET's `PersianCalendar` |
| Umm al-Qura | Its table | Its table (the two tables are identical) |

**Recommendation.** Broiler.VM's pinned ICU4X sources, which the Temporal reference datasets
Broiler.VM retains already check, and **no** .NET calendar classes, which vary with the runtime.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-8.

**Taken 2026-10-11, as recommended:** Broiler.VM's pinned ICU4X sources, and no .NET calendar
class. **Recorded by** SJL-8's records (SJL-8.1).

<a id="sjl-d-22"></a>
### SJL-D-22 — Locale coverage and the owner of CLDR data

**Question.** Which locales do the shared Intl data carry, and who generates them?

**Facts.**

- Broiler.VM covers `de`, `de-DE`, `en` and `en-US`, with complete data for each. Every other
  locale resolves deterministically to one of them.
- Broiler.JS mixes sources:
  - plural rules for a wide set of locales, through the CLDR package;
  - list, relative-time and day-period data for the languages the package carries;
  - English-only date-format and zone-name data;
  - host cultures for availability, separators and date order.
- Broiler.JS's date-time formatting in German produces English month names.
- Broiler.VM's JSD-0027 recorded `UnicodeCldr.LocaleData` as "Candidate input, not accepted".

**Alternatives.**

- **(a)** Grow Broiler.VM's generated subset to the locales Broiler.JS needs, in the shared data
  package.
- **(b)** Extend `Broiler.Unicode.Cldr.LocaleData` to Broiler.VM's table completeness and format.
- **(c)** Keep Broiler.JS's host-culture fallback as an optional provider for locales the shared
  data lacks.

**Recommendation: (a) with (c)** for Broiler.JS only, behind its own provider. **Each locale added
to the shared data is a data change** under [SJL-D-6](#sjl-d-6), with a size budget in Broiler.VM's
evidence.

**Holder.** The JavaScript profile owner, the Broiler.JS maintainer and the Broiler.Unicode
maintainer. **Needed before** SJL-9.

**Taken 2026-10-11: (a) with (c), as recommended.** Broiler.VM's generator, moved with its pins to
the Broiler.JS repository ([SJL-D-1](#sjl-d-1) (b-ii)), generates the agreed locale set. Broiler.JS
keeps a host-culture provider for the locales the shared data lacks, and Broiler.VM registers none.
**Recorded by** SJL-9's records (SJL-9.1), which revisit JSD-0027 §4.

<a id="sjl-d-24"></a>
### SJL-D-24 — Collation

**Question.** Broiler.VM implements UCA collation over CLDR root plus a few tailorings for its
locales. Broiler.JS uses the host's `CompareInfo`, which supports every host locale and ignores
`numeric` and `caseFirst`. Is collation shared?

**Alternatives.**

- **(a)** Share Broiler.VM's algorithm. Broiler.JS adopts it for the locales the shared data covers
  and keeps a host fallback provider for the rest.
- **(b)** Leave collation engine-specific.
- **(c)** Share later, after SJL-9.

**Recommendation: (c).** Collation is the last Intl component, as both surveys ranked it.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-9's
collation work package.

**Taken 2026-10-11: (c), as recommended.** Collation leaves SJL-9 and is shared after it, by its own
milestone, [SJL-17](milestones/sjl-17-collation.md). Until SJL-17's cut-overs, each engine keeps its
own. **Recorded by** SJL-9's records (SJL-9.1). SJL-17.1 files the area verdict.

<a id="sjl-d-14"></a>
### SJL-D-14 — Implementation base of the shared regular-expression engine, and duplicate named groups

**Question.** Broiler.Regex is the published component, and Broiler.JS already routes some patterns
through it. Broiler.VM's `JsRegExpMatcher` has what Broiler.Regex lacks:

- a caller-owned charge;
- frame and trail ceilings;
- bounded nesting without CLR recursion;
- exact property names and validated group names;
- an anchored match argument;
- one parser shared with the compiler's early errors.

Broiler.Regex today gives its budget a fresh allowance at each start position, can return a
shorter match when the budget runs out in its greedy fast path, and has no cancellation. Neither
engine's shared candidate supports ES2025 duplicate named groups. Only Broiler.JS's .NET translator
does.

**Alternatives.**

- **(a)** Fix Broiler.Regex to meet every Broiler.VM requirement, then move Broiler.VM onto it.
- **(b)** Make Broiler.VM's matcher the implementation inside the `Broiler.Regex` package identity,
  keeping Broiler.Regex's public syntax tree and set algebra where consumers use them.
- **(c)** Leave regular expressions engine-specific.

**Duplicate named groups.**

- (i) Implement them in the shared engine before Broiler.JS retires its translator.
- (ii) Record the shared engine's refusal as a divergence both engines share.

**Recommendation.** Decide between (a) and (b) on a predeclared comparison:

- conformance on the pinned test262 regular-expression directories;
- resource behaviour under adversarial patterns;
- each engine's own candidate-versus-control measurement.

On duplicate named groups, take (i). Broiler.JS must not lose a feature it has.

**Holder.** The Broiler.RegEx maintainer, the JavaScript profile owner and the Broiler.JS
maintainer. **Needed before** SJL-11.

**Taken 2026-10-11: (a), chosen by the owner, and (i), as recommended.** The recommendation left (a)
and (b) to a comparison.

- **Broiler.Regex is fixed until it meets every requirement listed above**, and its defects listed
  above go with them. Then both engines move onto it, and Broiler.VM's matcher is deleted.
- **The three-part comparison** planned to choose between (a) and (b) becomes the acceptance check
  of each engine's cut-over.
- **Duplicate named groups** are implemented in Broiler.Regex before Broiler.JS retires its
  translator. Broiler.VM, which refuses them today as a declared divergence, gains them at its
  cut-over as a listed correction ([SJL-D-16](#sjl-d-16)).

**Recorded by** Broiler.RegEx's roadmap (SJL-0.6) and SJL-11's area-verdict JSD (SJL-11.1).

<a id="sjl-d-11"></a>
### SJL-D-11 — Diagnostic codes and message text

**Question.** What do the shared frontend's diagnostics carry, and who owns the text?

**Facts.**

- Broiler.VM binds its source diagnostics to registry codes 2001 to 2406 (rules N5 to N9). Its
  conformance runner grades on the code. Its message text is "Varying". Its guests see a generic
  `SyntaxError`.
- Broiler.JS throws `FastParseException` with V8-style English messages, which its guests see.

**Recommendation.** The shared frontend emits:

- a stable code;
- a phase;
- a span;
- a source name;
- a **message identifier with arguments**;
- an optional default English rendering.

Broiler.VM maps codes onto its registry (a registry revision for every meaning change, as JSD-0009
requires). Broiler.JS renders its V8-style text from the identifier and arguments. Neither engine's
guest-visible text is part of the shared contract.

**Holder.** The JavaScript profile owner and the Broiler.JS maintainer. **Needed before** SJL-12's
exit.

**Taken 2026-10-11, as recommended.** **Recorded by** SJL-12.2.

<a id="sjl-d-12"></a>
### SJL-D-12 — Canonical syntax tree, and the compatibility of Broiler.JS's public `Ast` and `Parser`

**Question.** Which tree does the shared frontend produce? What happens to Broiler.JS's public `Ast`
and `Parser` packages?

**Facts.**

- **Broiler.VM's wide tree:**
  - immutable records with full positions;
  - side facts in identity-keyed tables;
  - exact literal values;
  - no ambient state, under rule N12;
  - deterministic in principle. In practice its hoisting order relies on dictionary insertion
    order, which backend stage JSB-1, not started, is meant to fix.
- **Broiler.JS's tree:**
  - mutable, public classes;
  - partly desugared at parse time;
  - built using ambient `CoreScript` state and a process-wide temporary counter;
  - early errors split across the parser, a validation pass and the compiler.
- **Broiler.VM's JSD-0014** rejects the latter two properties explicitly.

**Alternatives.**

- **Tree.**
  - (a) A new immutable tree, seeded from Broiler.VM's design, with an adapter into each consumer's
    current tree during the transition.
  - (b) Evolve Broiler.JS's tree, which has to drop mutability, desugaring and ambient state to
    qualify.
- **Broiler.JS packages.**
  - (i) Keep `Broiler.JavaScript.Ast` and `.Parser` as adapter packages that build the old tree from
    the new one, obsolete them, and remove them in a later major version.
  - (ii) Break them in one major version.

**Recommendation: (a) with (i).**

**Holder.** The Broiler.JS maintainer and the JavaScript profile owner. **Needed before** SJL-12's
exit.

**Taken 2026-10-11: (a) with (i), as recommended.** **Recorded by** SJL-12.2, and in Broiler.JS's
`docs/public-api.md` at SJL-14.4.

<a id="sjl-d-13"></a>
### SJL-D-13 — Broiler.VM's slice front-end path: migrate or retire

**Question.** Broiler.VM has two front-end paths sharing one tokenizer:

- the **slice** path: its own loose parser, a single static-semantics stage that already allocates
  frame slots, and a deliberate refusal of unresolvable identifiers;
- the **wide** path.

Does the slice path move to the shared frontend, or retire?

**Alternatives.**

- **(a) Rebuild it on the shared tree and semantic model**, keeping its census counts, slot order
  and refusals byte for byte.
- **(b) Retire it**, with the slice manifest compiled by the wide path under the slice manifest's
  admission. This needs the slice's refusals reproduced by admission, and a JSD.
- **(c) Keep it** on its own parser and the shared tokenizer, accepting a permanent second parser.

**Recommendation.** (a) or (b), on a predeclared comparison of the slice corpus and the retained
source corpus. **(c) is admissible only as a recorded refusal**, because it keeps a duplicate.

**Holder.** The JavaScript profile owner. **Needed before** SJL-15.

**Taken 2026-10-11: (a), chosen by the owner.** The recommendation left (a) and (b) to a comparison.

- **The slice path is rebuilt on the shared tree and semantic model.** Its census counts, slot order
  and refusals stay byte for byte, which the slice corpus shows before and after. The comparison
  planned to choose between (a) and (b) becomes that acceptance check.
- **`SliceParser` is deleted.** The slice lowering, with its slot allocation, stays.
- **Rule N16 is not revised**, because no manifest moves to the wide path.

**Recorded by** the profile JSD of SJL-15 (SJL-15.4).
