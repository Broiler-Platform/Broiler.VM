<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-0-001

**Milestone:** UBC-0 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the extraction record, the records that move, the predeclared parity rules.
**Collected:** 2026-09-25, at commit `3042855`, from a clean tree, by `collect.py` in this directory.
**Core contract version:** 1, unchanged.
**Status of the milestone after this collection:** `In progress`, every exit-gate clause met on the
evidence below, and unaccepted: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while
review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-0-001 |
| Milestone | UBC-0, exit gate clauses 1 to 8 |
| Record | [ADR 0013](../../adr/0013-the-universal-bytecode-extraction-record.md), not contract-bearing |
| Source revision compared | `51e0d60ee43ce34e8d21b1d9b10d086cdb723501` - every JavaScript and WebAssembly file the record cites, named in `manifest.json` by its git blob identifier at that revision |
| Commits of the milestone | `6486add` the three decision rules alone; `61cb43b` the record and the records that move; `3042855` the corrections an adversarial review of those records found |
| Owner | the core architecture owner, with the JavaScript and WebAssembly profile owners |
| Reviewer | none |
| Evidence class | records and rule runs; no measurement of any kind |

## 2. What this bundle demonstrates, clause by clause

| Clause | What it asks | Where it is shown |
|---|---|---|
| 1 | ADR 0013 exists, is not contract-bearing, carries the six record items and a per-condition verdict, and rules E2 and E4 pass | The record itself; `architecture-tests.log` shows rules E1 to E4 passing in detail and the whole architecture suite passing at this commit |
| 2 | The correspondence table names both implementations by file, member and revision, and no shared-part cell holds a language identifier | `correspondence-table.md` is the table as it stands at this commit; `vocabulary-scan.log` runs the banned-vocabulary scan rule U2 will read over its shared-part column and over the proposed surface's names (`proposed-surface.txt`), both clean, and two negative controls, each watched failing and then passing with the record restored byte for byte |
| 3 | Every core sentence UBC-0.3 names is revised with the superseded text quoted, and the amendment register carries each as applied | ADR 0003 section 11, rows 19 to 22, and the four revised sentences of `docs/roadmap.md`, each carrying its superseded text |
| 4 | JSD-0036 is filed and indexed, and JSB-3's State bullet names it | The decision, its index row, and the State bullet of 2026-09-25 in the backend roadmap, which states both readings of JSB-3's clause |
| 5 | Both profile plans carry the "stands beside this plan" paragraph, and the WebAssembly corrections file carries no entry for this milestone | The two plans' non-goals; the WebAssembly corrections file is unchanged by every commit above |
| 6 | `docs/mvp.md` section 5 carries rows for UBC-R1 and UBC-R2 | MVP-10 and MVP-11, beside MVP-12, the route the review found the record had taken without saying so |
| 7 | The three decision-rule files exist, hold no figure, and each names its evidence class, population, admitted classes and one decision | `docs/evidence/ubc-3-001/`, `ubc-4-001/` and `ubc-6b-001/`, first added by `6486add` before any code of the milestones they judge, and revised the same day by `3042855`, still before any such code, where the review found two of them ambiguous |
| 8 | The ledger's UBC-0 row says what it is, and a refusal is recorded as such | The programme ledger, committed with this bundle |

**What the verdict is, stated here because the ledger cites this bundle for it.** ADR 0013 accepts its
candidate A - the universal bytecode and its one loop - on G1 to G4. For candidate B - the native-form
mechanism the concept places beside it - G1 is unsatisfied, so under ADR 0011 the gate cannot be invoked
for it, and the record carries the dated note ADR 0011's exclusion item 5 prescribes, which carries no
verdict and refuses nothing. The programme's work package UBC-0.6 therefore reads the outcome as an
acceptance. The note's consequence is not small, and this bundle does not soften it: through the
roadmap's own "Waits on" fields, milestones UBC-3 and UBC-5 to UBC-10 cannot meet their gates while it
stands, and only UBC-1, UBC-2 and UBC-4 can.

## 3. What was run

```text
dotnet build Broiler.VM.slnx -c Release
python docs/evidence/ubc-0-001/collect.py
python eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-0-001 --milestone UBC-0 --evidence-class records ...
```

`collect.py` copies the table and the names out of the record, runs the scan over both, runs the two
negative controls - a language identifier injected into row c's shared-part cell of the real record, and
a language's type name added to a copy of the names - and then the architecture tests. Its own header
says what it writes. The manifest command's full argument list is in `manifest.json`'s `inputs` and
`sources`, which name every file the bundle depends on.

## 4. Environment

Windows 11 Enterprise, the .NET SDK `10.0.401`, Python `3.11.9`, the checkout at the commit above with
nothing outside this directory changed. No composition was published and nothing was executed but the
two scripts and the architecture tests.

## 5. What is NOT here, named rather than left as an absence

- **No profile's supply of its half.** The roadmap's work package UBC-0.1 has each profile supply its
  file paths, revision and correspondence rows. Here the record's author compiled both halves from the
  checkout, and neither profile recorded a supply of its own; ADR 0013 says so.
- **No run of any language.** The correspondence table is a reading of source at a named revision.
  Nothing in it was executed, and no row claims a behaviour a run showed.
- **No check that the table's quoted members still exist at a later revision.** The manifest pins the
  source revision; a reader at a later commit reads the sources at that revision, not the tree.
- **No review.** Every record here is unreviewed, and the author and the owner are one person.

## 6. What a reader should take from it

That the extraction gate was invoked with real merged code on both sides, that the shared part of the
universal bytecode can be written without a language's name, that the bytecode half was accepted and
the native half could not be put to the gate at all, and what that costs the programme. It is not an
acceptance of anything, and it is not a claim that the programme can finish as planned.

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
