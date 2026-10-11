# Milestone SJL-0 — Records, decisions and baselines

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Decisions](../decisions.md)

- **Objective.** Before any code exists, four things are true:
  - in every repository involved, each record the programme would make false is revised, or is
    filed for revision with its superseded text quoted;
  - the programme decisions, all taken on 2026-10-11, are recorded where their records belong;
  - the baselines every later comparison needs are retained;
  - the pilot's comparison rules are committed.
- **Owner roles.**
  - **Broiler.VM:** the architecture owner, the core-contract owner (R5) for the ADR 0011 reading,
    the JavaScript profile owner, and the release owner (R6).
  - **Broiler.JS:** its maintainer.
  - **Broiler.Unicode and Broiler.RegEx:** their maintainers.
  - All one person (EX-30).
- **Repositories.** Broiler.VM, Broiler.JS, Broiler.Unicode, Broiler.RegEx.
- **Waits on.** Nothing.
- **Deadline inside the milestone.** Decision [SJL-D-7](../decisions.md#sjl-d-7) was taken on
  2026-10-11: re-scope JS-2. Its JSD is filed **before 2026-11-30**, or JSD-0005's stop condition
  takes the seed snapshot by default. Taking the decision does not disarm the stop condition; only
  the filed JSD does.

---

## Work packages

### SJL-0.1 — The responsibility inventory, at named revisions

[inventory.md](../inventory.md) was derived from the working trees on 2026-10-11. This work package
re-derives every row at **named commits** of Broiler.VM, Broiler.JS, Broiler.Unicode, Broiler.RegEx
and Broiler.DateTime, and records it in the bundle. Each row records:

- the responsibility;
- the Broiler.VM members;
- the Broiler.JS members;
- the component members, if any;
- the coupling class;
- the proposed disposition: shared, adoption, retained in engines, or candidate for refusal;
- the area milestone.

*Done when* the retained inventory names a commit for every repository and a disposition for every
row.

### SJL-0.2 — ADR 0014, the shared-libraries admission record

Filed as `docs/adr/0014-shared-javascript-libraries.md` (the title is a working name), header
`**Core contract:** not contract-bearing`, status `Proposed`. It records:

1. **Context.** The feasibility report; the review's corrections; the inventory of SJL-0.1; the
   one-person non-independence (EX-30).
2. **The scope reading of ADR 0011** (decision [SJL-D-8](../decisions.md#sjl-d-8)):
   - the extraction gate and the standing refusals govern sharing between Broiler.VM profiles and
     placement in the core;
   - a JavaScript-family dependency on an engine-neutral library is neither;
   - the route taken, the alternative not taken (an amendment, unexecutable under EX-15), and who
     would rule otherwise.
3. **The admission policy.** What may be admitted: a library that references neither engine and
   nothing in Broiler.VM, follows the conventions of
   [architecture.md](../architecture.md#4-contract-conventions), is pinned by content hash, and is
   named on the allow-list. Where it may be admitted: JavaScript-family projects only, never the
   core, `Ubc`, an emitter, the WebAssembly profile or a composition root directly. How the
   admission is enforced: rules `I1` to `I7` and the revised A2, B3, N1 and N37, all minted at
   SJL-2.
4. **The fork policy** (decision [SJL-D-9](../decisions.md#sjl-d-9)): fixes flow through the shared
   libraries; engine code does not flow between engines; evidence does not transfer; no local
   patches.
5. **Homes and package identity** (decisions [SJL-D-1](../decisions.md#sjl-d-1) and
   [SJL-D-2](../decisions.md#sjl-d-2)): the component-aligned homes, with locale and time data in the
   Broiler.JS repository, and the `Broiler.ECMAScript.*` packages, one per boundary.
6. **Versioning, co-hosting and the consumption source** (decisions
   [SJL-D-3](../decisions.md#sjl-d-3) and [SJL-D-4](../decisions.md#sjl-d-4)): a separate version
   stream, minimum ranges with lock files, and the committed feed in development with nuget.org for
   anything a release depends on.
7. **Assurance** (decision [SJL-D-5](../decisions.md#sjl-d-5)): full parity, and a Broiler.VM
   release that refuses an admitted package whose upstream revision lacks a completed review.
8. **The pilot decision** ([SJL-D-23](../decisions.md#sjl-d-23)): the programme continues past the
   pilot, a choice taken before the pilot ran. The record says so in those words.
9. **What is not planned**: whole-runtime unification ([SJL-D-18](../decisions.md#sjl-d-18)) and
   Broiler.JS's non-standard extensions ([SJL-D-17](../decisions.md#sjl-d-17)).
10. **Consequences**: every record SJL-0.3 to SJL-0.6 moves, by name. Nothing is reviewed,
    accepted, advertised, packed or published by this record.

*(Revised 2026-10-11, when every decision was taken. The old items 5 and 6, now 6 and 7, read:
"**Versioning, co-hosting and the consumption source**, as decisions SJL-D-3 and SJL-D-4 stand when
the record is filed. Each is either recorded as taken or named as open with its holder." and
"**Assurance** (decision SJL-D-5), in the same way." The new items are 5 and 8; the old items 7 and
8 are now 9 and 10.)*

The ADR index gains the row. The sentence "All thirteen are `Proposed`" is corrected with the
superseded text quoted. Rules E2 and E4 pass.

*Done when* the record is filed and indexed, and rules E2 and E4 pass.

### SJL-0.3 — The Broiler.VM core documents

| Record | Change |
|---|---|
| ADR 0003 §11 roadmap-amendment register | One row per roadmap sentence in [governance.md](../governance.md#21-broilervm-core-documents) items V1 to V7: the old text verbatim (ASCII, line breaks normalised), the new text, requiring ADR 0014, status **`Proposed`**. The rows become `Applied` at SJL-2, when the admission they describe exists. |
| `docs/roadmap.md` §8 and ADR 0011's standing-refusals table | An editorial pointer paragraph beside each, naming ADR 0014 and the scope reading, **without editing the decision text**. This follows the precedent ADR 0013 set. |
| `docs/roadmap.status.md` §1 | A dated correction of "Broiler.VM has no dependency on that component and none of its milestones wait for it", saying that this programme's milestones wait on libraries built in the Broiler.JS repository and that no core milestone does |
| `README.md`, "Relationship to Broiler.JS" | A dated correction with a pointer to this programme |
| `docs/mvp.md` §5 | A route row for the ADR 0011 scope reading, in the register's four-column shape |

*Done when* every row exists with its old text quoted, and each correction quotes what it replaces.

### SJL-0.4 — The JavaScript profile's records

1. **A JSD** (the next free number), in the shape `docs/decisions/README.md` requires: status,
   date, owner and co-signer as one person, decision, what it rejects and why, settling condition,
   owning milestone. It decides:
   - **admission:** the family may depend on admitted shared libraries;
   - **the revision of invariant 11:** shared libraries are upstream of both engines, and neither
     engine is the other's upstream;
   - **the withdrawal of the seed copy rows** in roadmap §4.3: tokenizer, parser, syntax tree,
     static analysis, storage and registration generator. The row "Acquire as this checkout's own
     dependencies" is replaced by admitted packages;
   - **decision [SJL-D-7](../decisions.md#sjl-d-7), taken as (a):** the seed copy is not taken, and
     JS-2's gate is re-scoped to "the general front end is the shared frontend (SJL-13)";
   - **decision [SJL-D-23](../decisions.md#sjl-d-23):** the programme continues past the pilot.

   **If this JSD cannot be filed before 2026-11-30**, a JSD that only revises JSD-0005's stop
   condition, to a programme milestone, is filed before that date instead. That is the fallback
   SJL-D-7's recommendation names.
2. **JSC entries** (the next free numbers, one per changed reading) for:
   - invariant 11;
   - §4 and §5's "no edge in either direction reaches any legacy Broiler component";
   - the §4.3 copy rows;
   - `roadmap.gates.md` gate 2 and its stop row;
   - `roadmap.delivery.md`'s JS-2 gate text.

   The plan carries a bare *(corrected: JSC-nn)* pointer at each.
3. **Invariant 12 ("No evidence transfers") is not touched.** The JSD says so in its own words.

*Done when* the JSD is indexed, every JSC entry exists in the five-field shape, and the JS-2 row of
the profile's ledger names the JSD as the record its settling condition asked for.

### SJL-0.5 — Broiler.JS's records

| Record | Change |
|---|---|
| `docs/architecture/dependencies.md` | A dated revision. "Broiler.VM is a separate component with no dependency edge to this one in either direction" becomes: no edge between the engines in either direction; the shared libraries in this repository are consumed by Broiler.VM and reference neither engine. The `FrontEnd/Semantics` row notes that it becomes the shared frontend, consumed by both of Broiler.JS's back ends and by Broiler.VM. The stale "Nested DateTime, Regex, and Unicode components" is corrected. |
| `docs/public-api.md` | A dated revision of "No Broiler.VM integration is planned for this component, and none changes that statement". The shared packages' compatibility promise is added at SJL-2. |
| `docs/roadmap/README.md` | "The bytecode/VM plan is retired from this component" **stays**. A pointer to the new plan pair is added. |
| `docs/roadmap/SharedLibraries.md` and `SharedLibraries.status.md` (new) | Broiler.JS's half of the programme, in its own plan/status convention. One item per milestone with Broiler.JS clauses, each with an owner, current evidence, a next action and an objective exit criterion. No changing count. |
| `Assemblies.md`, `Roadmap.md`, `ModernizationDelivery.md`, MOD-M2-7 | The residual text that still plans "Separate IL and Broiler.VM JavaScript integration shells" is reconciled: rewritten as a pointer to the shared-libraries plan, or withdrawn with that pointer |
| `Modernization.md` invariant 3 | Extended to the Broiler.VM consumer. MOD-M2-3, MOD-M2-4 and MOD-M2-5 are cross-referenced to SJL-12. |
| `docs/architecture/builtins-vs-clr-types.md` §6 | Its stale "vendored `Broiler.DateTime`" is corrected |

*Done when* each document carries a dated revision, and the plan pair exists with an item per
milestone.

### SJL-0.6 — The component repositories' records

- **Broiler.Unicode** gains a `docs/roadmap.md`; it has none today. Its items are the SJL-7
  requirements in [areas/unicode.md](../areas/unicode.md):
  - exact-name lookup, immutable results, code-point lookups;
  - pinned and hashed generator inputs, and the generator in the repository;
  - **tables reached through a provider**, so that each engine can embed the tables the generator
    produces ([SJL-D-6](../decisions.md#sjl-d-6) (c));
  - public data versions;
  - normalization, casing, folding and segmentation, if Broiler.Unicode meets SJL-7.1's owner
    conditions;
  - tests and an assurance gate;
  - the emoji-version correction in its README.

  Locale data is not among its items: [SJL-D-1](../decisions.md#sjl-d-1) placed it in the
  Broiler.JS repository.
- **Broiler.RegEx**'s `docs/roadmap.md` gains the SJL-11 requirements from
  [areas/regexp.md](../areas/regexp.md), as decision [SJL-D-14](../decisions.md#sjl-d-14) (a) took
  them: every Broiler.VM requirement, duplicate named groups, and the removal of the per-start
  budget reset.

*Done when* both roadmaps carry the items with an owner.

### SJL-0.7 — The programme decisions

Every decision was taken on 2026-10-11 ([decisions.md](../decisions.md)). This work package checks
that each is recorded where its section says.

**Recorded at SJL-0**, by SJL-0.2 to SJL-0.6:

- [SJL-D-7](../decisions.md#sjl-d-7), before 2026-11-30;
- [SJL-D-1](../decisions.md#sjl-d-1) to [SJL-D-5](../decisions.md#sjl-d-5);
- [SJL-D-8](../decisions.md#sjl-d-8) and [SJL-D-9](../decisions.md#sjl-d-9);
- [SJL-D-17](../decisions.md#sjl-d-17), [SJL-D-18](../decisions.md#sjl-d-18) and
  [SJL-D-23](../decisions.md#sjl-d-23);
- the parts of [SJL-D-6](../decisions.md#sjl-d-6), [SJL-D-14](../decisions.md#sjl-d-14) and
  [SJL-D-16](../decisions.md#sjl-d-16) that the Broiler.JS plan and the component roadmaps carry.

**Recorded later, by the milestone that applies each:** `SJL-D-3`'s `docs/publishing.md`
(SJL-2.13), `SJL-D-10` (SJL-2 and SJL-3), `SJL-D-16`'s pilot JSD (SJL-3.1) and each area's changelog,
`SJL-D-25` (SJL-3, then each area), `SJL-D-15` (SJL-4), `SJL-D-19` (SJL-6), `SJL-D-6`'s JSD (SJL-7),
`SJL-D-20` and `SJL-D-21` (SJL-8), `SJL-D-22` and `SJL-D-24` (SJL-9), `SJL-D-14`'s JSD (SJL-11),
`SJL-D-11` and `SJL-D-12` (SJL-12, with `SJL-D-12`'s package disposition at SJL-14), and `SJL-D-13`
(SJL-15).

*Done when* the ledger's decision rows say `recorded` for each decision of the first list, naming
its records, and name the milestone for each of the second.

*(Revised 2026-10-11, when every decision was taken. The work package recorded `SJL-D-7`, `SJL-D-8`,
`SJL-D-9`, `SJL-D-17` and `SJL-D-18` at SJL-0, and read: "**Recorded here or scheduled for SJL-2**,
each with its holder named: SJL-D-1 to SJL-D-5. *Done when* the ledger's decision rows say which.")*

### SJL-0.8 — The baselines

Every comparison a cut-over is later judged by needs its base taken **before** any engine changes,
at a named commit, in the repository whose engine it describes.

**Broiler.VM (bundle `sjl-0-001`):**

1. **Whole-suite test262** at the pinned revision (`ccaac100…`), under the wide manifest, in the
   bytecode form and the native form, through `eng/run-test262.py`. The verdict files and the merge
   logs are kept. The release-candidate collector's procedure is the model.
2. **Closures** of every JavaScript composition root under JIT, trimming and Native AOT, read off
   the published output.
3. **The API baselines** as committed: the family's `docs/api/public-api.txt` and the core's
   `docs/api/public-api.txt`, with their hashes.
4. **Packages.** The family's produced nuspecs and contents, from the collector of bundle
   JS-10-002.
5. **The differential harness** over its retained answers, and against Broiler.JS at the base
   commit of SJL-0.8's Broiler.JS half. This is a cross-check; its declared divergences are
   refreshed and adjudicated, or listed as unadjudicated.
6. **The fuel-parity twins** and the retained exhaustion verdicts.
7. **Measurements for the pilot's operations** under the rule of SJL-0.9: number to string, string
   to number, and the literal values in the compiler. These are Broiler.VM's own measurements; the
   figures stay in the bundle.

**Broiler.JS (bundle `sjl-0-001`, the first bundle in Broiler.JS's new `docs/evidence/`):**

1. **Whole-suite test262** at the same pinned revision, through `scripts/compliance/run_test262.py`,
   in the configuration its canonical CI run uses. Per-path results are kept. (The two runners'
   totals are not comparable to each other, and no document compares them.)
2. **A public API listing** of every packable assembly. Broiler.JS has no API baseline today; this
   is its first, produced by the tool SJL-2 adopts, or by an interim generated listing kept in the
   bundle.
3. **Package contents and nuspecs** of every packable project.
4. **Measurements for the pilot's operations** with Broiler.JS's own benchmark project under the
   rule of SJL-0.9.

*Done when* both bundles exist, each naming its base commit, and each README states what the
bundle excludes.

### SJL-0.9 — The predeclared comparison rules

`decision-rule.md` files are committed **before any code they judge exists**, which a later bundle
shows with `git merge-base --is-ancestor`. They hold no figure, and each names:

- the evidence class;
- the population;
- the configuration;
- the comparison;
- the admitted difference classes;
- the one decision it produces.

They follow the templates in [validation.md](../validation.md#4-predeclared-comparison-rules):

| File | Judges |
|---|---|
| `docs/evidence/sjl-1-001/decision-rule.md` (Broiler.VM) and the same path in Broiler.JS | Vector classification: every difference between a vector and an engine is classified, and none takes its expected value from either engine |
| `docs/evidence/sjl-3-001/decision-rule.md` (Broiler.VM) | The pilot's test262 parity per variant in both forms; the fuel-parity twins; Broiler.VM's measurement |
| `docs/evidence/sjl-3-001/decision-rule.md` (Broiler.JS) | The pilot's test262 parity per path; Broiler.JS's measurement |
| `docs/evidence/sjl-3-001/reference-arms.md` (home) | The library-level comparison against both engines' pre-cut-over implementations over the stated input population |

Later areas commit their own rules as their first act, under the same discipline.

*Done when* every file exists at a commit earlier than the first code commit of the milestone it
judges.

---

## Deliverables

- ADR 0014 and its index row.
- The ADR 0003 §11 register rows (`Proposed`).
- The pointer paragraphs beside roadmap §8 and ADR 0011's table.
- The core-ledger and README corrections.
- A route row in `docs/mvp.md`.
- The profile JSD and its JSC entries.
- Broiler.JS's revised documents and its new plan pair.
- The component roadmap items.
- The decision records.
- Both baseline bundles.
- The decision-rule files.
- The programme ledger's SJL-0 row.

---

## Exit gate

**Broiler.VM clauses**

1. ADR 0014 exists, is not contract-bearing, and carries the record items of SJL-0.2. Rules E2 and
   E4 pass over the index.
2. Every sentence of governance items V1 to V7 has a register row with its old text verbatim and
   status `Proposed`. Items V8 and V9 are corrected in place with their superseded text quoted.
3. The route row for [SJL-D-8](../decisions.md#sjl-d-8) exists. ADR 0011's and roadmap §8's
   decision text is unchanged, which a diff of each shows, and each carries the pointer paragraph.
4. The profile JSD is filed and indexed. JSC entries exist for profile items P1, P3, P4 and P5 of
   [governance.md](../governance.md#22-javascript-profile-documents-srcbroilervmprofilejavascriptdocs).
   [SJL-D-7](../decisions.md#sjl-d-7) is recorded before 2026-11-30, or a JSD revising JSD-0005's
   stop condition to a programme milestone is filed before that date. *(Revised 2026-10-11, when
   SJL-D-7 was taken: the sentence read "SJL-D-7 is recorded on a date no later than 2026-11-30, or
   JSD-0005's stop condition is revised in the same record.")*
5. Bundle `sjl-0-001` retains every base of SJL-0.8's Broiler.VM half at a named commit.
6. The Broiler.VM decision-rule files of SJL-0.9 exist, hold no figure, and name the six elements.
7. The ledger rows say `In progress`, naming the unmet clauses, until every clause is met.

**Broiler.JS clauses**

8. Every document of SJL-0.5 carries a dated revision, and the `SharedLibraries` plan and status
   pair exists.
9. Broiler.JS's bundle `sjl-0-001` retains every base of SJL-0.8's Broiler.JS half at a named
   commit.
10. The Broiler.JS decision-rule files exist, under the same conditions as clause 6.
11. [SJL-D-17](../decisions.md#sjl-d-17) and [SJL-D-18](../decisions.md#sjl-d-18) are recorded, and
    the shared-libraries plan carries Broiler.JS's part of every decision SJL-0.7 lists as recorded
    at SJL-0. *(Revised 2026-10-11, when every decision was taken: the clause read "SJL-D-17 and
    SJL-D-18 are recorded.")*

**Component clauses**

12. Broiler.Unicode's new roadmap and Broiler.RegEx's roadmap carry the programme's items, with an
    owner.

**The refusal branch.** Both decisions it turns on were taken on 2026-10-11 as recommended, so the
branch now opens only on a later ruling that reverses one of them. If
[SJL-D-8](../decisions.md#sjl-d-8) is ruled to need an amendment, or
[SJL-D-9](../decisions.md#sjl-d-9) is declined:

- the records say so;
- every later row in the ledger is `Not started`, with the reason;
- **the programme ends with SJL-0's gate met.**

The feasibility report then stands as an assessment, with the review's corrections.

---

## Evidence bundles

- **Broiler.VM `docs/evidence/sjl-0-001/`:**
  - `README.md` and `manifest.json` (the commits compared and every input by hash);
  - the base test262 verdict files and merge logs in both forms;
  - the closures in three modes;
  - the API baseline hashes, nuspecs and contents;
  - the differential report;
  - the fuel-parity logs;
  - the measurement logs with their rule;
  - the retained inventory;
  - the rule E2 and E4 run.
- **Broiler.JS `docs/evidence/sjl-0-001/`:**
  - `README.md` and `manifest.json`;
  - the test262 per-path results and merge logs;
  - the public API listing;
  - nuspecs and contents;
  - the measurement logs with their rule.

---

## Does not do

- **It writes no code** under any repository's product directories, mints no architecture rule,
  admits no package and moves no algorithm.
- **It does not make the scope reading or the fork policy succeed.** A refusal is a met gate.
- **It does not file the records of decisions that later milestones apply.** For example,
  [SJL-D-14](../decisions.md#sjl-d-14)'s area-verdict JSD belongs to SJL-11.
- **It does not edit decision text in any contract-bearing record.**
