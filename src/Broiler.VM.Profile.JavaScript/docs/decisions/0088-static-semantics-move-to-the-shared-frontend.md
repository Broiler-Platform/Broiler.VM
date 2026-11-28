<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0088 - Static semantics move to the shared frontend: SJL-15's area verdict, this family's half, decisions D57 to D66 as the documents quote them, and the owner's decisions of 2026-11-16

**Status:** Decided 2026-11-16, this family's half.
- **The area moves** (decision D-verdict): the facts SJL-15's milestone lists - scopes and their kinds, declarations
  and conflicts, Annex B.3.3 aliases, per-function flags, reference resolution with dynamic markers, labels and jump
  targets, private-name environments, module records, named-evaluation names, free names and captures, and every early
  error outside the parse - move to the semantics half of `Broiler.ECMAScript.Frontend`, in the Broiler.JS repository,
  under the contract [JSD-0080](0080-the-shared-frontend-contract-and-the-in-place-preparation.md) recorded and the
  design [`areas/static-semantics.md`](../../../../docs/javascript-shared-libraries/areas/static-semantics.md) sketches,
  grown from the declarations walk SJL-13.2 began. Each engine keeps its lowering decisions, as the milestone's table
  "What each engine keeps" lists them.
- **It is read under X1 to X7 below**, this family's half of each. It is an extraction for every fact but free names
  and captures, which this family does not compute: for this family that fact is an adoption (X1).
- **Broiler.JS's half of X2 and X6 is completed by a dated note on this record** (decision D-x2), filed with
  Broiler.JS's own change for SJL-15.1, before any code of SJL-15: its column of the correspondence, the class of each
  row across both engines, and its deletion list with the disposition of every public type it empties.
- **Decisions D57 to D66 are SJL-15's in SJL-13's plan, which is held in no repository.** They are recorded here as the
  documents quote them, and only so (section "Decisions D57 to D66"): D60 and D61 as JSD-0083 quotes them, D65 as
  JSD-0083 and JSD-0081 quote it, D64's second half applied as Broiler.JS's record of D64 leaves it to SJL-15, and D65 as
  "no edge" while SJL-11.4 and SJL-11.5 have not landed. D57, D58, D59, D62, D63 and D66 are quoted nowhere, and this record does not reconstruct them.
- **The owner's decisions of 2026-11-16 are recorded**, each taken as recommended under the owner's direction of
  2026-10-18, each with its alternative: D-verdict, D-x2, D-windows, D-pin, D-slicetree, D-handoffs, D-precision,
  D-measure, D-roots and D-tables (section "The owner's decisions of 2026-11-16"). They carry names, not numbers, because the plan's
  numbers D1 to D74 are all assigned (JSD-0083's note on D-asi and D-t24).
- **Decision D-census is put to the owner**, and not taken: what clause 5's "byte-identical before and after" means for
  the slice corpus's inputs that move when the slice path parses with the library, in three classes: those hand-offs H23
  and H25 correct (class H), those whose refusal becomes the library's (class R), and the census lines the library's
  parse moves (class C). SJL-D-13 (a), which clause 5 applies, was the owner's own choice.
  *(Note of 2026-11-16, after SJL-15.1 merged: "put to the owner, and not taken" no longer holds. **The owner answered
  on 2026-11-16, choosing "Admit listed rows"**, alternative (ii) of section "Put to the owner: decision D-census",
  which records the answer verbatim at its end. D-census is the owner's own decision, not one taken under the owner's
  direction of 2026-10-18. SJL-15's clause 5 is revised before any evidence, and bundle `sjl-15-001`'s rule is revised
  the same day by a new dated file, so that its part S admits classes H, R and C, each listed by row; every other byte
  difference still fails part S.)*
- *(Added 2026-11-23.)* **Decision D-ab1 of 2026-11-23 is recorded**, taken as recommended under the owner's direction
  of 2026-10-18, with its alternatives, in section "The owner's decisions of 2026-11-23": row AB1 is listed at window 1,
  in part A of bundle `sjl-15-001`'s rule by a class of its own, ED, and in its part B's class F, by that rule's
  revision of 2026-11-23, filed before any run. It is the session's decision, which the owner may overturn.
- **SJL-15.0, the start, is a reading of the owner's words of 2026-10-25**, recorded as a reading and not as a decision
  taken under the direction (the programme ledger's SJL-15.0 entry).
- Nothing in this profile changes with this record. The model is SJL-15.2's, in Broiler.JS; the cut-overs are SJL-15.3's
  and SJL-15.4's own changes, and this record is what they apply. It accepts no milestone, meets no clause and moves no
  gate.

**Date:** 2026-11-16.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the owner of the home and of the other engine;
- the core architecture owner, for rule I1's allow-list and rule I3's closures, which admit the model's stream version;
- the verification-boundary owner, as the holder of
  [JSD-0014](0014-the-source-front-end-and-the-verification-boundary.md)'s decision 1, which keeps one producer for
  each check.

**All four roles are held by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the programme's SJL-15, work package SJL-15.1, with SJL-15.0's start.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, with that
  repository's half of the decisions that bind it, by these names and numbers. It is filed in Broiler.JS's own change,
  and is observed until it merges; the programme ledger names its merge commit when it has.
- **SJL-15 started before SJL-13 and SJL-14, which it waits on, met their gates**, at the owner's words of 2026-10-25,
  which the programme ledger's SJL-15.0 entry records and reads. SJL-15 may not meet its gate before SJL-13 and SJL-14
  meet theirs. This record moves no gate.

**The owner's words of 2026-10-25.** The owner wrote: "Continue with the remaining SJL milestones". JSD-0083 reads them
as the choice to start SJL-13, and records that "the same words, read the same way, start SJL-14 and SJL-15 when each
files its own start entry" (JSD-0081's decision D2). SJL-15's first work package files its records here, so this is
SJL-15's start. Broiler.JS's status row for SJL-15 names the same condition: "its records once SJL-13.1's and SJL-14.1's
rules are filed", and both are. That is a reading of words, recorded as one.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." This record reads it in two parts, as JSD-0080 to JSD-0084 did: each decision SJL-15.1 needs is taken as
recommended and recorded below as the owner's decision, dated 2026-11-16, the day it is filed, with the alternative not
taken; and the programme proceeds through SJL-15's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-15 is accepted by it.
- SJL-15 still may not meet its gate before SJL-13 and SJL-14 meet theirs.
- Rules still come before runs: bundle [`sjl-15-001`](../../../../docs/evidence/sjl-15-001/decision-rule-area.md)'s rule
  is committed with this record, before any code it judges, with its revision of the same day
  ([`decision-rule-area-revised-2026-11-16.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-16.md)),
  filed before any run.
- **Three kinds of entry are not taken under it**: the start, which is a reading of the owner's words; decision
  D-census, which would revise what the owner's own choice SJL-D-13 (a) keeps; and each publication of the shared stream
  (JSD-0083's decision D30).

**Names.**
- **Decisions.** "Decision Dn" with a number is SJL-13's plan's numbering, which JSD-0083 keeps: D1 to D39 are
  JSD-0083's, D40 to D56 and D64 SJL-14's (Broiler.JS's `docs/roadmap/SharedLibraries.md`), D57 to D66 SJL-15's and D67
  to D74 SJL-16's. "Decision D-name" is this record's.
- **Rows.** The correspondence's rows, in bundle
  [`sjl-15-001`](../../../../docs/evidence/sjl-15-001/correspondence.md), are named by their letters: SC, DE, AB, FF, RS,
  LB, PN, MR, NE, FC, EE and SL. Its findings are F1 to F7. A row of JSD-0083's correspondence is named with its bundle,
  as "`sjl-13-001`'s B16".
- **Windows.** SJL-15.3's merge is window 1 and SJL-15.4's window 2 (decision D-windows).
- **Paths.** A line number is the line at Broiler.VM `a86b6f5d` and at Broiler.JS `6fda6ca4`, as the correspondence gives
  them.

---

## Context

The area's design is the programme's
[`areas/static-semantics.md`](../../../../docs/javascript-shared-libraries/areas/static-semantics.md), and its contract is
JSD-0080's, filed at SJL-12.1, whose section "The semantic model's design" the home's `design.md` restates. SJL-13's
syntax half added the declarations walk (JSD-0083's decision D10): it reports every early error either parser reported
at parse time and exposes each scope's kind and declarations and each program's and function's strictness and parameter
names as side tables in source order. SJL-15.2 grows the model from that walk and does not start a second one (the
milestone's note of 2026-10-26 under SJL-15.2).

This family has two producers of the facts today (the correspondence, "The model surface today"): the wide path's
binder pass, `JsBinder`, separated from emission and made deterministic at SJL-12.6 and moved onto the shared tree at
SJL-13's step 2, which reads only strictness from the library and computes every other declaration fact again; and the
slice path's `SliceStaticSemantics`, behind its own `SliceTokenizer` and `SliceParser`, which allocates frame slots and
refuses unresolvable identifiers. SJL-D-13 (a), the owner's choice of 2026-10-11, rebuilds the slice path on the shared
tree and model, keeping its census counts, slot order and refusals byte for byte.

**Roadmap section 5 asks for this verdict.** The programme's own extraction gate applies "at the start of every area
milestone (SJL-3 to SJL-11, SJL-13 to SJL-15, and SJL-17). The area's first work package files the verdict." SJL-15's
milestone file has no verdict work package, and roadmap section 11's SJL-15 row names no verdict JSD. JSD-0083 is the
frontend area's verdict for the syntax half's five sub-areas, and it says it does not decide "The semantic model:
SJL-15". The note that narrowed section 5 for SJL-14 reads SJL-14 as "the second engine's cut-over of the same area",
which SJL-15 is not. So SJL-15.1 files the verdict (decision D-verdict).

---

## The verdict, condition by condition

| Condition | This family's half |
|---|---|
| **X1. Two implementations** | **This family's half: two producers in merged code at `a86b6f5d`, for every fact but free names and captures**: `JsBinder` (`src/Broiler.VM.Profile.JavaScript.Compiler/JsBinder.cs`, `JsBinder.Walks.cs`, `JsBinder.EarlyErrors.cs`) and the lowering's resolution (`JsCompiler.Names.cs`) on the wide path, and `SliceStaticSemantics` on the slice path. **Broiler.JS's producers are named, not read here**: `FastScope`, `FastFunctionScope`, `FreeNameScan`, `SyntaxValidation`, `DirectEvalSupport` and its compiler's checks, as `areas/static-semantics.md` section 2 names them; Broiler.JS's half reads them at a stated revision (decision D-x2), and until its dated note fills the correspondence's column this record does not say X1 holds over both engines. **Free names and captures** (row FC1) have no producer in this family: every binding lives in an environment record; the model's fact is an adoption for this family, which reads no capture. |
| **X2. Correspondence from code** | **This family's half is written**, in bundle `sjl-15-001`'s `correspondence.md`, read from code at `a86b6f5d` against the model surface at `6fda6ca4`, each row classed against the specification for this family. **Broiler.JS's half completes it** by a dated note on this record and on the correspondence, filed with Broiler.JS's SJL-15.1 change before any SJL-15 code, which gives each row its class across both engines; until then no row's cross-engine class is recorded, and none is unknown in this family's column (decision D-x2). |
| **X3. Neutral contract** | **Held by the design and checked by the home.** The model is JSD-0080's design: source-ordered collections, no hash order, failure as diagnostics, no engine type; the declarations walk it grows from already meets it (`DeclarationTables`, `DeclarationsWalk`). The home's clauses 1 to 3 judge the implementation (Broiler.JS's bundle `sjl-15-001`). |
| **X4. No edge between engines, none into Broiler.VM** | **Held.** The library references neither engine. **Decision D65: no package edge to Broiler.Regex** while SJL-11.4 and SJL-11.5 have not landed (section "Decisions D57 to D66"). |
| **X5. Boundaries hold** | **Held, by rule I3.** The model is in `Broiler.ECMAScript.Frontend`, which this family admits for `Broiler.VM.Profile.JavaScript.Compiler` alone; no compiler-free root reaches it. Bundle `sjl-15-001`'s part D runs rule I3, and its part I lists each root's closure. |
| **X6. Deletion list** | **This family's half, by type and by member, below** (section "The deletion lists"). Broiler.JS's half, with the disposition of every public type SJL-15.5 empties (SJL-D-25), is completed by the dated note decision D-x2 names. |
| **X7. Owners accept** | **The owners' decision D-verdict**, under the direction; one person holds every role (EX-30). This is the area verdict roadmap section 5 asks for; it moves no milestone toward `Accepted` (update rule 7). |

**On refusal** the verdict would have named the failed condition and the source-level pointers. No condition fails.

*(Noted 2026-11-17, at Broiler.JS's SJL-15.1, by decision D-x2, completing X2 and X6.)* X2's cell above reads "**Broiler.JS's half completes it** by a dated note on this record and on the correspondence, filed with Broiler.JS's SJL-15.1 change before any SJL-15 code, which gives each row its class across both engines; until then no row's cross-engine class is recorded", and X6's reads "Broiler.JS's half, with the disposition of every public type SJL-15.5 empties (SJL-D-25), is completed by the dated note decision D-x2 names." The Broiler.JS paragraph under "The deletion lists (X6)" below says the same ("completed member by member, with the disposition of every public type it empties, by the dated note decision D-x2 names"), and this note is that one. Broiler.JS's SJL-15.1 change has merged as `e4e45d38` (observed), before any SJL-15 code. **X2:** its column of the correspondence, read from code at Broiler.JS `6fda6ca4` with the probes of both engines, is its bundle `sjl-15-001`'s `correspondence-broiler-js.md`, and bundle `sjl-15-001`'s `correspondence.md` here carries it, each row with its class across both engines (rows SL2 to SL5, which are Broiler.VM's only, with the class Broiler.JS's column gives them), by its note of the same date. No row's class is unknown. The rows that differ: corrections of Broiler.JS in rows DE3, FF4, FF5, NE1 and EE1 (in eval code, the super and `arguments` rows its compiler leaves unchecked, each in its rule's class T, and super-call rows refused with an engine error or not answered; as Scripts, `arguments` in a static block and `this` as a for-in target; the other rows of its class T, listed by site before any run, show no difference); corrections of Broiler.VM in rows AB1 (the inputs of probes V028 to V030, one of them the pinned edition against test262 at `ccaac100`, the fact implementation-defined), LB1 (H20), MR2 (H19) and SL4 (H23, H25); every other row the same, or each engine's policy. Free names and captures (row FC1) are an extraction from Broiler.JS's `FreeNameScan` and an adoption here, as X1 says. **X6:** Broiler.JS's deletion list, member by member, and the disposition of every public type SJL-15.5 empties are its bundle `sjl-15-001`'s `dispositions.md`: `FastFunctionScope`, its nested `VariableScope` and `FreeNameScan` obsolete with the removal version `1.0.0`, SJL-D-25 (b), by Broiler.JS's decision D-js-dispositions; `FastScope` and `FastScopeItem` facade-obsolete since SJL-14's W2 (decision D50); `SyntaxValidation` and `FastCompiler.EarlyErrors` deleted whole, internal and private; the checks in `Visit*` and `DirectEvalSupport.Validate` deleted by member, `DirectEvalSupport` itself kept. Both engines' halves of X2 and X6 are then filed; this note meets no clause and moves no gate.

---

## Decisions D57 to D66, as the documents quote them

SJL-13's plan numbers D57 to D66 as SJL-15's, "filed by their first work packages" (JSD-0083, section "Names"), and "is
held in no repository" (JSD-0083, the same section). **This record does not have the plan's text.** Each number below is
recorded from what the documents quote of it, with where; where nothing quotes a number, the number is recorded as
unquoted and nothing is put in its place. The decisions SJL-15.1 needs beyond these are taken under names of their own
(next section), so that if the plan's text is found, its D57 to D66 can be compared with this record and each difference
filed by a dated note.

| Decision | What the documents quote | Where | Recorded here as |
|---|---|---|---|
| D57 | Nothing | (none) | **Unquoted**; not reconstructed |
| D58 | Nothing | (none) | **Unquoted**; not reconstructed |
| D59 | Nothing | (none) | **Unquoted**; not reconstructed. JSD-0083's "D59" in its correspondence (`sjl-13-001`'s row D59, EG0007) is a row, not this decision |
| D60 | "SJL-15.4, which removes `SliceTokenizer` (D60)"; "SJL-15.4, which rebuilds the slice path on the shared library and deletes `SliceParser` and `SliceTokenizer` (D60)"; "the slice path's `SliceTokenizer.IsWhiteSpace` at SJL-15.4 (H3, D60)" | JSD-0083's hand-offs H23, H25 and H13 | **At SJL-15.4 the slice path parses with the shared library, and `SliceParser` and `SliceTokenizer` are deleted.** Applied in X6 and in bundle `sjl-15-001`'s rule. A reading: the bare "D60" in that table is read as the plan's decision, not as `sjl-13-001`'s row D60 (EX0010, an import or export in a Script), which has nothing to do with the slice tokenizer; JSD-0083's own convention would read a bare "Dn" as a row, so the reading is recorded |
| D61 | "SJL-15's `semantics` vectors follow the same route (decision D61)"; the route itself is D25's text about the `syntax` vectors, in schema 1, with module-goal and switch-dependent rows left to the parse corpus, and "A revision after each engine's cut-over retires the divergence fields that no longer hold", with this family's pin move | JSD-0083, decision D25 | **The `semantics` vectors follow the `syntax` vectors' route.** Only those words are D61's. A reading, drawn from D25's text about the `syntax` vectors and not quoted as D61's: added to Broiler.JS's corpus in schema 1 before the shared code lands (SJL-1.6), each engine's divergence fields citing its records, retired by a corpus revision after each engine's cut-over with this family's pin move. Applied by bundle `sjl-15-001`'s pin commit and its part E |
| D62 | Nothing | (none) | **Unquoted**; not reconstructed. `sjl-13-001`'s row D62 (EP0015) is a row |
| D63 | Nothing | (none) | **Unquoted**; not reconstructed. `sjl-13-001`'s row D63 (`StrictModeValidator`'s `case` clauses) is a row |
| D64 | "Class T (an error raised before evaluation that the base raised later, or not at all), listed, at SJL-14 for the rows D10 moves into the parse ... and at SJL-15 for the rest. SJL-13's plan numbers it with SJL-15's decisions; it is taken here, with D40 to D56, because its SJL-14 half binds this milestone's rule, and SJL-15's records read its other half" | Broiler.JS's `docs/roadmap/SharedLibraries.md`, SJL-14's decisions table, the owner's decision of 2026-11-03; JSD-0083's decision D45 and its SJL-14.1 note | **Already filed by SJL-14.1; not reconstructed.** Its second half is applied here: **for this family**, class T lists the row whose early error moves before evaluation with the model, eval code's undeclared private name (row PN1, finding F6), admitted in bundle `sjl-15-001`'s rule's parts A and B; **for Broiler.JS**, its rule for SJL-15 lists by row the early errors SJL-15.5 moves from its compiler into the model |
| D65 | "The library's package edge to Broiler.Regex: decision D65's, at SJL-15.1"; "which is decided at SJL-15.1, and only if SJL-11.4 and SJL-11.5 have both landed" | JSD-0083, "What this record does not decide" and "What this rejects, and why" and decision D7; JSD-0081's note of 2026-10-26 under decision D18 | **No edge.** Neither SJL-11.4 nor SJL-11.5 has landed (the programme ledger's SJL-11 row, every clause unmet), so the condition D65 is decided under does not hold: `Broiler.ECMAScript.Frontend` gains no package edge to Broiler.Regex, and the regular-expression seam stays the request's validator provider (JSD-0083's decision D7). **When both have landed**, D65 is taken again by a dated note on this record, before any change that would add the edge |
| D66 | Nothing | (none) | **Unquoted**; not reconstructed |

---

## The owner's decisions of 2026-11-16

Each is taken as recommended under the owner's direction of 2026-10-18, and recorded as the owner's decision of
2026-11-16. Each names the alternative not taken.

**Decision D-verdict - the area verdict.** **A verdict JSD under X1 to X7, this record**, at the next free number, in
JSD-0083's and JSD-0084's form. *Not taken:* a dated narrowing note on roadmap section 5, like SJL-14's of 2026-10-26,
reading SJL-15's verdict as JSD-0083's or JSD-0080's. The reason: section 5 names SJL-15 among the area milestones whose
first work package files the verdict; JSD-0083 covers the syntax half's five sub-areas and says it does not decide the
semantic model; JSD-0080 is SJL-12's contract, which roadmap section 5's note of 2026-10-19 says is "not an area
milestone owing a verdict"; and SJL-14's narrowing rested on SJL-14 being the second engine's cut-over of an area
already judged, which SJL-15 is not. Roadmap section 12's first stop condition is "An area moved without its
extraction-gate record". A dated note on roadmap sections 5 and 11 names this record.

**Decision D-x2 - Broiler.JS's half of X2 and X6.** **Completed by a dated note on this record and on the
correspondence**, filed with Broiler.JS's own SJL-15.1 change, before any SJL-15 code, as JSD-0083's X6 was completed at
SJL-14.1. Until it is filed no row has a class across both engines, and no SJL-15.2 code merges. *Not taken:* holding
this verdict until both halves exist and filing it once. The reason: each repository files its half in its own change,
this family's first; the rules must be committed before any code, and nothing of SJL-15's code can land before
Broiler.JS's half in any case, since SJL-15.2's code is Broiler.JS's.

**Decision D-windows - this family's windows.** **Two windows, one merge each**: window 1, SJL-15.3, the lowering on the
model with `JsBinder` deleted; window 2, SJL-15.4, the slice path rebuilt. Each window's base is its merge's first parent;
either may land first, and the later window's base descends from the earlier's after commit; neither lies inside
SJL-11.4's window, whose bases bundle `sjl-11-001`'s rule voids if another merge lands there. *Not taken:* one merge for
both. The reason: window 1 moves artifact bytes by class P and window 2 must move nothing in the slice corpus; one merge
would mix two attributions in one comparison, as SJL-13 kept its steps apart (JSD-0083's decisions D19 and D35).

**Decision D-pin - the model reaches this family by a published, admitted version.** **The pin commit**, a merge of its
own before window 1's base, admits the stream version the owner publishes with the model (JSD-0083's decision D30) and
moves this family's vectors pin to the corpus revision carrying the `semantics` vectors (decision D61), as this family
moved to `0.1.0-preview.11` before SJL-13's step 2. *Not taken:* the version move inside the window's merge. The reason:
the move would change what the base compiles against inside the comparison that judges the window.

**Decision D-slicetree - the slice path's tree.** **At SJL-15.4 the slice static semantics and the slice lowering read
the shared tree**, and `SliceSyntax.cs`'s records go with `SliceParser`; the census reads its construct kinds
(`SliceConstructKind`, `SliceConstructs.cs`, which stays) from the shared tree by a fixed map. *Not taken:* an adapter
building `SliceSyntax` from the shared tree, as SJL-13's step 1 built `JsSyntax`. The reason: an adapter keeps a second
tree with one consumer, which SJL-13's step 2 removed on the wide path; the slice manifest's constructs are few and
statement-level; bundle `sjl-15-001`'s part S judges the census, the slot order and the refusals byte for byte either
way.

**Decision D-handoffs - JSD-0083's hand-offs to SJL-15.** **H19** (row MR2, the top-level-`await` reading) and **H20**
(row LB1, `continue` to an outer label of a label set) are corrections of window 1, from the model's module record and
jump-target fact; **H18** (the dynamic function's parameters and body parsed under their own goals) is a correction of
window 1, as JSD-0083's table recommends ("SJL-15's Broiler.VM window, which re-pins the frontend"); **H23** and **H25**
(row SL4) are window 2's, which parses the slice path with the library. Each is a class F correction of bundle
`sjl-15-001`'s rule, listed by row before any run. *Not taken:* H18 in a change of its own with its own window, which
JSD-0083's table also allows ("a later Broiler.VM change with its own row and window"). The reason: window 1 already
re-pins the frontend and runs every part over the wide path; a window of its own would repeat them for one row.

**Decision D-precision - the binder's approximations.** **Window 1 reads the model's precise facts** for `arguments`, a
separate variable environment and direct-`eval` contamination (rows FF3, FF4 and FF5), and the artifacts that move for
those reasons only are bundle `sjl-15-001`'s part B class P, each listed with its row and its decoded difference. *Not
taken:* keeping the binder's approximations in the lowering, or-ed with the model's facts, so that no byte moves. The
reason: the milestone asks for "`arguments` need, which is precise instead of an over-approximation"; keeping the
approximation would keep a second computation of a model fact in the lowering, which is what SJL-15 removes; clause 6
admits a difference that is classified.

**Decision D-measure - the measurement.** **SJL-15.6's "each compiler's own measurement" is a rule part of each bundle
(template T5), not a gate clause**, as SJL-13 read its own (the milestone's note of 2026-10-26 under SJL-13.6, JSD-0083's
decision D24). SJL-15's exit gate has no measurement clause, and none is added. A dated note on the milestone records
the reading. *Not taken:* a gate revision adding a measurement clause. The reason: no controlled lane exists, so a
measurement clause would be unmet until one does, as SJL-3's clause 7, SJL-4's clause 9 and SJL-14's clause 6 are, and
it would join the clauses SJL-D-23's trigger (b) waits on; the milestone never asked for one.

**Decision D-roots - clause 9's "every root".** **Every JavaScript root this machine can publish, each root that carries
the JavaScript profile, the Android root being EX-126's**, as the readings of SJL-3, SJL-8, SJL-9, SJL-10, SJL-13 and
SJL-17 took the same words; bundle `sjl-15-001`'s part I judges it. *Not taken:* every root the composition register
lists, the roots that carry no JavaScript profile among them.

**Decision D-tables - how the tools read the slice path's binding table.** *(Taken 2026-11-16, at SJL-15.1, after a
review of bundle `sjl-15-001`'s rule before any run; filed by that rule's revision of the same day.)* **The tools commit
adds one public read-only member to `SliceCompilation`**, in `SliceSourceCompiler.cs`, that answers each binding's name,
its kind as text and its slot, in slot order, filled from the binding table the slice static semantics answers, as
strings and integers and no type either window deletes; it changes no artifact byte, diagnostic or census line, and the
tools commit's path condition names that file for that member alone. SJL-15.4 keeps the member and its meaning and fills
it from its own allocation over the model, so the report code stays unchanged at window 2. *Not taken:* the window-2
merge ports the report code, its output held byte-identical by a control. The reason: at `a86b6f5d` the table is
`SliceStaticSemantics.Validate`'s answer over a `SliceProgram` that only `SliceTokenizer` and `SliceParser` build, inside
the front end, so tools code that built it itself would name three types window 2 deletes, and bundle `sjl-15-001`'s
parts B, S and C, which run the tools "unchanged at each after commit", could not be configured at window 2; a port would
be judged by a comparison it writes itself.
*(Note of 2026-11-28, before the tools commit exists; the decision above is not edited.)* It reads "the tools commit's
path condition names that file for that member alone". The member also changes the profile family's API baseline,
`src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`, which rule N10 holds to the family's public surface, and
which that condition, as bundle `sjl-15-001`'s rule reads it with its revision of 2026-11-16, does not name: the
baseline is neither under the repository's top-level `docs/` nor an assurance artefact. That rule is revised a fourth
time, before any run, by
[`decision-rule-area-revised-2026-11-28.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-28.md),
which quotes the rule whole and leaves it and its three revisions unedited: the tools commit's second condition also
names that baseline, "for the lines of that member alone (rule N10)", and nothing else in the rule changes. The rule
commit is from then on the commit that carries all five files. The revision was taken on 2026-11-28 as recommended,
under the owner's direction of 2026-10-18 quoted above, "Please do all recommended decisions and skip waiting for
review.", and is recorded here, as the section "The owner's decisions of 2026-11-23" below records decision D-ab1,
under the owner's direction, with the alternative not taken. It is not one of the owner's own answers, and it is none
of the three kinds of entry this record names as not taken under the direction. **It is the session's decision, and
the owner may overturn it**, at any time, by a further dated rule file in bundle `sjl-15-001`; filed before any run of
the part it revises, that file is not weaker evidence, and filed after one, it is, as the rule's section "Revisions"
reads. *Not taken:* asking the owner first, as bundle `sjl-13-001`'s revision of 2026-11-08, which admitted the same
baseline to its registry commit's list "for the members `SliceSourceDiagnosticCode` gains, as rule N10 requires, and
nothing else", followed the owner's own choice ("Revise rule + rebuild"). The member, its meaning and the alternative
not taken above are unchanged. This choice is flagged for the owner.

## The owner's decisions of 2026-11-23

*(Added 2026-11-23.)* The programme ledger's entry of 2026-11-17 records what the `semantics` vectors' first runs show
of this family, among them row AB1, and ends "where row AB1 stands in the rule is not decided here". The decision below
was taken on 2026-11-23 as recommended, under the owner's direction of 2026-10-18 quoted above, "Please do all
recommended decisions and skip waiting for review.", and is recorded here, as the decisions of 2026-11-16 were, under
the owner's direction, with the alternatives not taken. It follows the pattern of decision D-tables above, a matter of
bundle `sjl-15-001`'s rule found before any run and filed by a revision of that rule. It is not one of the owner's own
answers, and it is none of the three kinds of entry this record names as not taken under the direction. **It is the
session's decision, and the owner may overturn it**, at any time, by a further dated rule file in bundle `sjl-15-001`;
filed before any run of part A or part B, that file is not weaker evidence, and filed after one, it is, as the rule's
section "Revisions" reads.

**Decision D-ab1 - where row AB1 stands in bundle `sjl-15-001`'s rule.** **(a) Row AB1 is listed at window 1, by a dated
revision of the rule filed before any run**,
[`decision-rule-area-revised-2026-11-23.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-23.md),
which quotes the rule whole and leaves it and both revisions of 2026-11-16 unedited. **Part A gains class ED**, window 1
only, which admits (i) the variants of test262's `annexB/language/function-code/block-decl-func-skip-arguments.js` whose
verdict moves from pass to fail because SJL-15.3 follows the pinned edition, the file at `ccaac100` asserting this
family's answer, an earlier edition's reading, and (ii) any other variant whose verdict moves for row AB1's reason, a
move from pass to fail only where the variant asserts the base's answer against the pinned edition's text, each listed
with the row. **Part B's class F gains row AB1**, window 1 only, beside LB1, MR2 and the dynamic function's goals,
bounded by the decoded difference as class P is, because row AB1 moves an accepted input's bytes and no outcome or
diagnostic. Class ED is not class C1 widened: C1's correction half (LB1, MR2 and the dynamic function's goals) admits
only a move from fail to pass, and its (T) half is row PN1's timing and is unchanged. Class ED departs from template T1
of the programme's validation document twice, for its listed variants alone, both named in the revision before any run:
its part (i) is a move from pass to fail, which T1 admits only under C2 ("no variant passing at base fails, is exhausted
or is unsupported after, except under C2"), and class ED is none of the classes C1 to C5 of that document's section 4
("The difference classes they admit come from this vocabulary:"), outside T1's "every changed verdict falls in C1 to C5
and is listed". Every other line of the rule and of its revisions stands, parts G and J among them: part G's twins and
part J's workloads name part B's class P alone, so a twin or a workload whose artifact moves for row AB1 is read under
those parts as written. The rule commit is from then on the commit that carries all four files; the words of section
"Put to the owner: decision D-census" below, on the owner's answer, "the rule commit is from then on the commit that
carries all three files", read as of 2026-11-16 until 2026-11-23, and are left as written.
- *Not taken:* (b) **SJL-15.3 keeps Broiler.VM's current answer for `semantics/annex-b/ab1/0005`**, row AB1 staying a
  known divergence and the rule unrevised. The reason: the correspondence classes row AB1 a correction of Broiler.VM
  across both engines, and its last cell has SJL-15.3 read the model's alias decision, `ScanAnnexB` gone; keeping the
  base's answer would keep in the lowering a computation of the alias against the model's fact, which decision
  D-precision rejects for the binder's approximations.
- *Not taken:* (c) **the rule left as it is.** The reason: a move of row AB1 at window 1 is then a regression of part A,
  the named file's verdict moving from pass to fail, and of part B, an accepted input's bytes moving outside class P, so
  either part reads NOT MET for the correction the correspondence names, and this record's falsifier, "An artifact of
  window 1 that moves for a reason class P, F, T or S7 does not name", holds; a revision written after the runs were
  seen would be weaker evidence.
- *Not taken:* class C1 stretched to row AB1. C1's correction half admits a verdict that improves, and its (T) half row
  PN1's timing alone; reading it to admit one that worsens would change what it means for the rows it already lists.
- *Not taken:* class C3, "Implementation-defined", the nearest of the validation document's classes, as the
  correspondence's row AB1 reads "the whole fact implementation-defined (Annex B), on in both engines' requests". The
  reason: with Annex B on in both engines' requests, which block function it aliases is the pinned edition's text, the
  row's class "Specification"; the rule reads "no C2, no C3 and no C5", and C3 admitted would be admitted for every row;
  and T1 admits a move from pass to fail only under C2, so C3 would depart from T1 as class ED does.

**What it changes:** bundle `sjl-15-001`'s rule, by that revision; the bundle's README, by dated notes; this record's
falsifier, by a dated note; the programme ledger. No condition of the verdict, no deletion list, no hand-off and no
code changes.

---

## Put to the owner: decision D-census

**The question.** Clause 5 reads "the slice corpus's census counts, slot order and refusals are byte-identical before and
after", applying SJL-D-13 (a), "keeping its census counts, slot order and refusals byte for byte", which was the owner's
own choice of 2026-10-11 ("Taken 2026-10-11: (a), chosen by the owner"). Parsing the slice path with the library, as
SJL-15.4 does (decision D60), moves three classes of the slice corpus's outputs, which bundle `sjl-15-001`'s rule, as
its revision of 2026-11-16 reads, names before any run:
- **Class H, the corrections** (row SL4). JSD-0083 hands to SJL-15.4 two defects of the slice path, H23 and H25, and the
  owner's decision "Exempt and record" of 2026-11-09 recorded H25's: the slice tokenizer's escape digits, its
  regular-expression body ending in a backslash, and `SliceParser`'s acceptance of a strict directive with `\8` or
  `\08`. The library corrects each. An input of the slice corpus that reaches one, the parse corpus's `J000262` and
  `J001009` among them, cannot be both corrected and byte-identical (finding F4).
- **Class R, the refusals the library makes** (rows SL3, DE3 and EE1). `SliceStaticSemantics` reports every refusal it
  finds, in walk order, under its own codes; the library reports its first refusal, by place, under its own rows, before
  any tree reaches the slice static semantics (row EE2). The window-2 deletion list below removes the slice path's checks
  of rows the library makes, so an input refused at the base by one of them (`ReservedWordAsBinding`,
  `DuplicateLexicalDeclaration`, `VarAndLexicalCollision`, `LegacyOctalInStrictCode`, `ConstWithoutInitialiser`,
  `InvalidAssignmentTarget`, and `IllegalBreak` and `IllegalContinue` once the model reports jumps), or refused with more
  than one diagnostic, can change its code, message, place or list, and so can a program of `SliceSourcePrograms` that
  pins one.
- **Class C, the census's parse half** (row SL5, finding F5). The census counts files parsed and parse failures by the
  first slice diagnostic's code; a file `SliceParser` parsed that the library refuses, a file the library parses that
  `SliceParser` refused, or a failure counted under the library's code moves a line of it.

**The alternatives.**
- **(i) Clause 5 stands as written.** Bundle `sjl-15-001`'s part S admits no difference; any input of class H, R or C
  that differs makes it NOT MET, and clause 5 stays unmet until the owner chooses otherwise. SJL-15.4 cannot keep the
  defects, or the slice path's own refusals of rows the library makes, while parsing with the library.
- **(ii) Clause 5 is revised before any evidence, under update rule 5**, to "The slice path is rebuilt on the shared
  model: `SliceParser` is gone, and the slice corpus's census counts, slot order and refusals are byte-identical before
  and after, but for the corrections the correspondence lists by row before any run (hand-offs H23 and H25), the
  refusals the library makes in place of the slice path's own, and the census lines the library's parse moves, each
  input listed with its row and class", with a new dated rule file in bundle `sjl-15-001` that admits classes H, R and C,
  each bounded as that file says, before any run of part S; SJL-D-13 gains a dated note quoting it. The owner may admit
  some of the classes and not others; a class not admitted is judged as in (i).

*Not offered:* SJL-15.4 keeps the slice path's own codes, messages and full walk-order lists, the checks of class R
taken off the deletion list below. The library refuses before any tree reaches the slice static semantics, so a full
list would need a tree the library has refused or a parse of the slice path's own, which decision D60 deletes; and the
kept checks would be a second producer of the library's rows, which JSD-0014's decision 1 does not allow.

**Recommended:** (ii), for all three classes, because the corrections are handed to SJL-15.4 by the owner's own decision
of 2026-11-09, the refusals of class R and the census lines of class C follow from decision D60, which parses the slice
path with the library, and the byte-identity the owner chose on 2026-10-11 is the rebuild's acceptance check for
everything else: the slot order, the binding tables, the accepted inputs' bytes and the admission refusals of rows RS3
and RS4. **It is not taken
here**: it would revise what the owner's own choice keeps, which the direction of 2026-10-18 does not reach. **Until the
owner chooses, clause 5 stands as written**, and part S judges it so.

**The owner's answer, 2026-11-16** *(added 2026-11-16, after SJL-15.1 merged, closing "put to the owner")*. The question
put was: "SJL-15 clause 5 requires the slice corpus's outputs to be byte-identical before and after the slice path moves
onto the shared library. Moving it necessarily changes three kinds of output: the corrections of hand-offs H23 and H25,
the refusals the library now makes in place of the slice path's own, and census lines its parse moves. How should clause
5 read?" The options were "Admit listed rows" (recommended) and "Keep as written". **The owner chose "Admit listed
rows"**, described as: "A dated revision now, before any evidence, admits those three classes, each listed by row.
Every other byte difference still fails part S." That is alternative (ii) above, for all three classes. It is the
owner's own decision, not one taken under the owner's direction of 2026-10-18, which this record said does not reach
it. "Until the owner chooses, clause 5 stands as written" therefore ends, and it is applied as follows, each by a dated
record that quotes what it replaces:
- **SJL-15's clause 5** is revised before any evidence, under the ledger's update rule 5, to the words of alternative
  (ii): "The slice path is rebuilt on the shared model: `SliceParser` is gone, and the slice corpus's census counts,
  slot order and refusals are byte-identical before and after, but for the corrections the correspondence lists by row
  before any run (hand-offs H23 and H25), the refusals the library makes in place of the slice path's own, and the
  census lines the library's parse moves, each input listed with its row and class." The milestone file quotes the
  clause as it stood.
- **Bundle `sjl-15-001`'s rule** is revised by a new dated file,
  [`decision-rule-area-revised-2026-11-16b.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-16b.md),
  filed before any run of part S, which quotes the rule it revises whole and leaves both earlier files unedited; the
  rule commit is from then on the commit that carries all three files. Part S admits class H (row SL4, hand-offs H23
  and H25), class R (rows SL2, SL3, DE3 and EE1: the static-semantics refusals, and a single refusal of the slice
  tokenizer or parser, but not the manifest's) and class C (row SL5, any census line that moves only by the files whose
  parse is the library's), each listed by row: class H by its rows now, from the correspondence, with its inputs
  completed at the base run; classes R and C, whose inputs and lines only a run can show, from the window-2 base run's
  reports, before any after run of window 2 is taken, the listing committed in the bundle, or retained there with the
  time it was written, before the after run, from the correspondence's rows. A difference outside a listed row, or
  beyond its class, still fails part S. The revision records its choices with their alternatives: class H's inputs
  completed at the base run, not limited to those named now, and class R counting the slice parser's and tokenizer's
  single refusals, not bounded to the static-semantics codes.
- **SJL-D-13** gains a dated note in `decisions.md` quoting the revised clause, as alternative (ii) said.
- No evidence of SJL-15 exists, so nothing is re-evaluated.

---

## The deletion lists (X6)

**This family, at window 1** (SJL-15.3): the binder pass, `JsBinder`, whole: `JsBinder.cs`, `JsBinder.Walks.cs` and
`JsBinder.EarlyErrors.cs`, with its nested types (`Declarations`, `Declaration`, `HoistingScope`, `FunctionFacts`,
`ModuleDeclaration`, and the walks' `Exit`, `PrivateScope`, `Context` and pending records), registered in rule I5 by type,
`JsBinder` being deleted whole with its nested types and members, which its row refuses with it; the constructors,
positional record parameters and enumeration members rule I5's reader does not read are named by kind in I5's register
paragraph and in SJL-15.3's cut-over record (as bundle `sjl-13-001`'s rule revision of 2026-11-10 and bundle
`sjl-13-002`'s of 2026-11-13 read the same clause). The lowering's readers of it (`JsCompiler.Hoisting.cs`, `JsCompiler.Statements.cs`,
`JsCompiler.Functions.cs`, `JsCompiler.Eval.cs`, `JsCompiler.Modules.cs`, `JsCompiler.Classes.cs`,
`JsCompiler.ControlFlow.cs`, `JsCompiler.Exceptions.cs` and `JsCompiler.cs`) read the model instead. **Kept**: the
lowering's slot and record allocation (`Scope.Declare`), its exit stack for emission, its scope-map rows and its
`BinderAdmitted` guards, which become guards on the model's answers.

**This family, at window 2** (SJL-15.4): `SliceParser.cs` and `SliceTokenizer.cs` whole (decision D60), and
`SliceSyntax.cs`'s records (decision D-slicetree), each type deleted whole registered in rule I5 by type, its positional
parameters named by kind and not registered; in `SliceStaticSemantics`, its own scope
analysis (`Scope`, `HoistVarBindings`, `VarDeclaratorsWithin`, `DeclareVar`, `DeclareLexicalNames`, `DeclareLexical`,
`CheckVarLexicalIntersection`, `EnterScope`, `LeaveScope`, `HoistingScope`, `Resolve`'s scope walk) and its checks of
rows the library makes (`CheckBindingName`, the duplicate and collision checks, the legacy-octal and constant-initialiser
checks, the assignment-target check), each member of a type that stays registered by member, one row per distinct
name, or, where it is lines of a member that stays, named by row in SJL-15.4's records. **Kept**: `SliceStaticSemantics` itself, rebuilt over the model, with its admission
refusals (the manifest's constructs, unresolvable identifiers and assignment to a constant, rows RS3 and RS4) and the
slice lowering's slot allocation; `SliceConstructs.cs` and the census; rule N16's slice manifest on its own path.

**Broiler.JS, at SJL-15.5**: the milestone's list - the compiler's `FastFunctionScope`, `FreeNameScan`, the scope half of
`FastScope`, `SyntaxValidation`, `FastCompiler.EarlyErrors`, and the early-error checks in `Visit*` methods and
`DirectEvalSupport.Validate` - completed member by member, with the disposition of every public type it empties, by the
dated note decision D-x2 names.

---

## The hand-offs

| Hand-off | From | Where it goes |
|---|---|---|
| H18 | JSD-0083 (rows P26, D61 of `sjl-13-001`) | Window 1, class F (decision D-handoffs) |
| H19 | JSD-0083 (row P32, item 10) | The model's module record (SJL-15.2); window 1, class F (row MR2) |
| H20 | JSD-0083 (row D30) | The model's jump-target fact (SJL-15.2); window 1, class F (row LB1) |
| H23 | JSD-0083 (rows T13, T12) | Window 2 (row SL4); decision D-census |
| H25 | JSD-0083 (row T33, EE0004) | Window 2 (row SL4); decision D-census |
| H-eval-private | This record (row PN1, finding F6) | The model's inherited context; window 1, class T (decision D64's second half) |

---

## What this rejects, and why

- **A second walk for the model**: the model grows from the declarations walk (the milestone's note under SJL-15.2).
- **A package edge from the library to Broiler.Regex now**: decision D65's condition does not hold.
- **The binder's approximations kept beside the model's facts**: a second computation of a model fact (decision
  D-precision).
- **One merge for both of this family's windows**: two attributions in one comparison (decision D-windows).
- **An adapter tree for the slice path**: a second tree with one consumer (decision D-slicetree).
- **A measurement gate clause**: unmet until a controlled lane exists (decision D-measure).
- **Reconstructing D57, D58, D59, D62, D63 and D66 from their likely subjects**: the plan's text is not held, and a
  recorded decision would claim a text nobody has.

## Settling condition

This record is settled when Broiler.JS's half of X2 and X6 is filed by its dated note, and SJL-15's clauses 4 to 9 have
been judged on bundle `sjl-15-001`, and clauses 1 to 3, 10 and 11 on Broiler.JS's bundle `sjl-15-001`, whatever each
decides.
*(Note of 2026-11-17, after Broiler.JS's SJL-15.1 merged: of the condition, "Broiler.JS's half of X2 and X6 is filed
by its dated note" is filed by the note after the verdict table, with bundle `sjl-15-001`'s correspondence's note of
the same day, Broiler.JS's change having merged as `e4e45d38` (Broiler-Platform/Broiler.JS#1120, observed). No clause
of SJL-15 has been judged on either bundle, so the rest of the condition stands, and the record is not settled.)*

## Dated notes filed with this record

Each beside the text it reads, quoting it: SJL-15's milestone file (the verdict and the rules at SJL-15.1; SJL-15.2's
model in Broiler.JS before SJL-14's cut-over; SJL-15.6's measurement; clause 5, unchanged, with decision D-census put to
the owner; the records list); the programme roadmap's sections 4, 5 and 11; JSD-0083's "What this record does not
decide", in one note under that section, which also reads what its section "Names" says of D57 to D66 and leaves nothing
beside that section; `areas/static-semantics.md`'s status; and the programme ledger's SJL-15.0 and SJL-15.1 entries,
its SJL-15 row, its Next list and "Last updated".
*(Note of 2026-11-16, after SJL-15.1 merged, on the owner's answer to decision D-census: the milestone file's note
above, "clause 5, unchanged, with decision D-census put to the owner", is followed by clause 5's dated revision and a
note recording the answer; bundle `sjl-15-001` gains its rule's revision of the same day, with notes in its README and
correspondence; SJL-D-13 in `decisions.md` gains a note; the decisions index and the programme ledger record it.)*
*(Note of 2026-11-17, after Broiler.JS's SJL-15.1 merged, on Broiler.JS's half of X2 and X6: this record gains the
note after its verdict table completing X2 and X6, and a note on its settling condition; bundle `sjl-15-001`'s
correspondence gains its note filling the column "Broiler.JS", and its README notes on its "Correspondence" paragraph
and on "What waits, in order"; the programme ledger records it in its entry of that day, its SJL-15 row, its Next list
and "Last updated".)*
*(Note of 2026-11-23, decision D-ab1: this record gains a bullet under its status, section "The owner's decisions of
2026-11-23" and a note on its falsifier; bundle `sjl-15-001` gains its rule's revision of that day, with dated notes in
its README; the programme ledger records it in its entry of that day, with a note on its entry of 2026-11-17's
sentence on row AB1, its Next list and "Last updated".)*
*(Note of 2026-11-28, the rule's revision of that day on the tools commit's path condition: this record gains a note
on decision D-tables; bundle `sjl-15-001` gains its rule's revision of that day, with a dated note in its README after
"What waits, in order"; the programme ledger records it in its entry of that day, later, with a note on its Next list's
item on the tools commit and the pin commit, and "Last updated".)*

## What this record does not decide

- **Broiler.JS's half**: its column of the correspondence, its deletion list and dispositions, its rule and its
  closure-sensitive suites, filed in its own change.
- **The model's design in detail**: SJL-15.2's, under JSD-0080's contract.
- **SJL-D-13's record**: the profile's JSD of SJL-15.4, with its JSC entries, as the milestone's records say.
- **Decision D-census**: the owner's. *(Note of 2026-11-16, after SJL-15.1 merged: decided by the owner that day, "Admit
  listed rows", as section "Put to the owner: decision D-census" records at its end.)*
- **Each publication** of the shared stream: the owner's (JSD-0083's decision D30).
- **A speed claim of any kind.**

## Falsified if

- A fact SJL-15.3's lowering reads from the model that the correspondence does not name, or a fact it names that the
  lowering still computes for itself after window 1.
- An artifact of window 1 that moves for a reason class P, F, T or S7 does not name.
  *(Note of 2026-11-23, decision D-ab1: class F of bundle `sjl-15-001`'s rule names row AB1 at window 1 from that day,
  by the rule's revision
  [`decision-rule-area-revised-2026-11-23.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-23.md),
  filed before any run. An artifact of window 1 that moves only because the lowering reads the model's Annex B.3.3
  alias decision as the pinned edition gives it, listed as class F's row AB1 lists it (the row, the function's place,
  the decoded difference, and the input's verdict in part A unchanged or in part A's class ED), is now a move for a
  reason class F names, and does not falsify this record; one that moves for row AB1 unlisted, or beyond what its
  decoded difference shows, still does. A test262 verdict that moves for row AB1 is that rule's part A's, read under
  its class ED, and this item does not read it. This reading is the session's decision, which the owner may
  overturn.)*
- The slice corpus's binding table, refusals or census differing at window 2 outside what the owner's answer to decision
  D-census admits.
  *(Note of 2026-11-16, after SJL-15.1 merged: the owner's answer admits classes H, R and C, each listed by row, as
  bundle `sjl-15-001`'s rule revision of that day,
  [`decision-rule-area-revised-2026-11-16b.md`](../../../../docs/evidence/sjl-15-001/decision-rule-area-revised-2026-11-16b.md),
  lists them.)*
- A shared-library edge to Broiler.Regex before SJL-11.4 and SJL-11.5 have both landed.
- The plan's text, if found, giving D57 to D66 a content this record contradicts, which a dated note then records.
