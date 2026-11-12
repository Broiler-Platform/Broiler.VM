# Milestone SJL-13 — The shared syntax library and Broiler.VM's wide path

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/frontend.md)

- **Objective.** `Broiler.ECMAScript.Frontend` exists with its tokenizer, parser, canonical tree,
  positions and diagnostics, under the contract of SJL-12. Broiler.VM's wide path parses with it,
  its own wide parser is gone, and its artifacts are byte-identical to the base, or every difference
  is classified. **Profile milestone JS-2 closes under its re-scoped gate.**
- **Owner roles.** The Broiler.JS maintainer (home) and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS (home), Broiler.VM.
- **Waits on.** SJL-12, plus:
  - SJL-3, for literal values;
  - SJL-7, for identifier classes.
  - **Regular-expression literal validation comes from SJL-11 if it has landed.** Otherwise it comes
    through the validation provider seam, which Broiler.VM implements over its current matcher until
    SJL-11. The seam is part of the contract either way.

---

## Work packages

### SJL-13.1 — Area verdict and comparison rules

1. The extraction gate is applied. X1 is met by both engines' merged front ends.
2. The rules are committed, as the area design's templates set out:
   - **Broiler.VM:** the artifact bytes for the parse corpus; test262 per variant in both forms; the
     registry rules; source fuzzing.
   - **Home:** determinism and the corpus.

*(Added 2026-10-26, at SJL-13.1, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md).)*
The area verdict is JSD-0083, decided on 2026-10-26: all five sub-areas accepted, the tokenizer, the parser and its
tree, positions, the diagnostics and the parse-time early errors, and the request with its switches, limits and
providers, each under X1 to X7. It records the owner's decisions D2 to D39, taken under the owner's direction of
2026-10-18, which changes no rule, and decision D1, a reading of the owner's words of 2026-10-25 that started SJL-13.
Its input is the correspondence of bundle [`sjl-13-001`](../../evidence/sjl-13-001/correspondence.md), read from code
in both engines and probed in both. The rules are committed: this family's
[`sjl-13-001`](../../evidence/sjl-13-001/decision-rule-area.md) for step 1 (parts A to J) and
[`sjl-13-002`](../../evidence/sjl-13-002/decision-rule-area.md) for step 2 (parts A to G), and the home's in
Broiler.JS's bundle `sjl-13-001`, which also holds the stage table's classification of the parse corpus.

### SJL-13.2 — The tokenizer and the parser

The design is set out in [areas/frontend.md](../areas/frontend.md):

- **Seeded from Broiler.VM's design:**
  - an immutable tree and identity-keyed side tables;
  - no rewinding;
  - cover grammars resolved by conversion;
  - structured templates;
  - limits checked before the stack is.
- **Literal values** come from the algorithms library.
- **Identifier classes** come from the Unicode owner.
- **Regular-expression literals** go to the validation provider.
- **Diagnostics** carry codes and message identifiers.
- **Determinism** is held by rule SL9, over two runs and two threads.
- **No ambient state** (rule SL2).
- **Native AOT** compatibility.

It is developed **against the parse corpus of SJL-12**, the test262 syntax tests and fuzzing. Before
any engine uses it, the parser accepts and refuses every corpus input as both engines' baselines
classify it.

*Done when* the corpus and the syntax tests pass at library level, under the rules.

*(Read 2026-10-26, at SJL-13.1, before any evidence of SJL-13 existed, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
decisions D10 and D26.)*
- **The library's scope.** "Diagnostics carry codes and message identifiers" is read with the parse-time early
  errors: the syntax half reports the grammar and every early error either engine's parser reports at parse time
  today, through a declarations walk in `Broiler.ECMAScript.Semantics` that also exposes side tables ordered by source
  position (scopes and their declarations, parameter names, strictness); it applies the one `request` row, EX0023, by
  the top-level-`await` switch, and reports no `engine` row. Broiler.JS's stage table
  (`Broiler.ECMAScript/docs/frontend/stages.tsv`) assigns every catalogue row its stage (decision D10).
- **The done-when.** "The parser accepts and refuses every corpus input as both engines' baselines classify it" and
  "the corpus and the syntax tests pass at library level, under the rules" are read as the classification Broiler.JS's
  `scripts/frontend/classify_parse_corpus.py` writes into its bundle `sjl-13-001`, from test262's metadata, the stage
  table and both engines' SJL-12 after baselines, in two configurations, this family's request (V) and Broiler.JS's
  (J). The home's rule judges the library against it (decision D26).
- **The work package is taken in six stages**, one merge each: the skeleton and the request; the tokenizer; the tree
  and the parser; the diagnostics and the parse-time early errors; the limits, determinism and fuzzing; and the
  done-when, whose merge is the library commit.

### SJL-13.3 — Broiler.VM's wide path on the shared library

The cut-over takes two steps:

1. **An adapter** from the shared tree to `JsSyntax`, so the lowering is unchanged and the artifact
   comparison isolates the parser.
2. **The lowering reads the shared tree directly**, and the adapter and `JsSyntax`'s parse-only
   parts go.

Each step lands with its evidence. Then:

- **Deleted:** `JsParser`.
- **`CompilationStack`** still wraps every entry point (rule N19).
- **Diagnostics.** Shared codes map onto the registry. `SliceSourceDiagnosticCode` remains the
  vocabulary rules N5 to N9 bind. The registry takes a new revision for any meaning change.
- **The slice path keeps** `SliceParser` and the tokenizer **until SJL-15 rebuilds it** on the
  shared tree and model (decision [SJL-D-13](../decisions.md#sjl-d-13) (a)). Until then, the shared
  tokenizer's behaviour must match the slice path's, which the corpus shows.

*(Read 2026-10-26, at SJL-13.1, before any evidence of SJL-13 existed, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
decisions D19, D20, D21 and D12.)*
- **"The adapter and `JsSyntax`'s parse-only parts go"** is read as `JsSyntax` whole: at step 2 the lowering,
  `JsNumericAdmission` and the binder read the shared tree, the binder's facts and their order unchanged, and the
  adapter and `JsSyntax` are deleted (decision D19).
- **"Deleted: `JsParser`"** is read with the rest of step 1's deletion list: `SliceTokenizer`'s wide-only members, the
  binder's `CheckRegExp`, and every binder check of a row the stage table assigns `syntax`, member by member, each
  registered in rule I5 (decision D20).
- **"The shared tokenizer's behaviour must match the slice path's, which the corpus shows"** is read as a checks row
  added at step 1 that compares the two tokenizers' streams over every input the slice path accepts, fact by fact
  through a field map bundle `sjl-13-001`'s part F fixes, with no class (decision D21).
- **"The registry takes a new revision for any meaning change"** is revision 19, at step 1's first commit (decision
  D12).

*(Noted 2026-11-10, at step 1's records, by [JSD-0087](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0087-the-wide-path-parses-with-the-shared-syntax-library.md).)* Step 1 is built on the branch `sjl-13-3-step1d`,
which lands as one merge, bundle `sjl-13-001`'s after commit, its first parent the base: the registry commit, revision
19, first; then the admission, the adapter with the deletions, the two `--checks` rows and one text per shared code.
`JsParser`, the slice tokenizer's wide-only members, the binder's `CheckRegExp` and its checks of `syntax` rows are
deleted, and the deleted types and members are registered in rule I5; bundle `sjl-13-001` defines EX-133 for the code
that left this component's assurance coverage. Its runs are taken after the merge. The reading above of "Deleted:
`JsParser`" says the binder's checks of `syntax` rows are deleted "member by member, each registered in rule I5
(decision D20)". Most of those checks were lines of members that stay, which rule I5, reading declared names, cannot
register: they are named by row in JSD-0087 and in rule I5's register paragraph, as JSD-0083's note of 2026-11-10 on
decision D20 reads it, and bundle `sjl-13-001`'s area rule says the same for its part D by its revision of that day.
The text above is left as written.

*(Noted 2026-11-10, later, at task 7.3 of step 1's evidence.)* Step 1 merged as `a8d8398` (Broiler-Platform/Broiler.VM#196),
bundle `sjl-13-001`'s after commit, its first parent `df75f8c` the base and the branch's first commit `cbee7c4` the
registry commit. Its evidence, tasks 7.1 to 7.3 of work package 7 on the branch `sjl-13-evidence`, is taken, and the
bundle's README judges the rule's ten parts in its section 4: parts A to I met, part J, the measurement, which is no gate
clause, not met. Section 5 names each reading the rule does not settle, with the alternative not taken, for the owner;
parts B, C and H each turn on one of them. Step 1's half of clauses 4 to 6 is judged; the clauses wait on step 2 as
well, under bundle `sjl-13-002`'s rule. The text above is left as written.

*(Noted 2026-11-10, later still, at the review of task 7.3.)* The note above says "parts B, C and H each turn on one of
them" and "Step 1's half of clauses 4 to 6 is judged". Part B turns on three readings, and the bundle judges, at step 1,
every clause its rule names: clauses 4 to 7 and 9 and clause 8's rule `I3`, of which clauses 4 to 6 wait on step 2 as
well. The readings section 5 of the bundle's README leaves to the owner, each with the reading taken and the
alternative not taken, are: kind (F)'s "first diagnostic", read as the one-process run's report detail; control K4's
"must still make the fifth row fail", met only by a harness narrowed to that row, on which part C turns; class R's
"unchanged between the registry commit and the after commit", read as a renumbering not undone, on which part B turns;
the stages and the library's messages, taken from stage-tagged scratch runs whose untagged reports equal part B's own;
the one process class N names; the code pairs class N reads between the registry and the after commit, R2's alone, on
which part B turns; part H's "match (divergence no longer holds)", read as a match, on which part H turns; one binder
message that several sites share; the idle compiler servers during parts I and J; the machine, replaced and its SDK
installed again, taken as the rule's local machine; and the registry commit's "in each path only what the list says",
shown by git reads for all but the mapping's and the stage table's lines and the sites of its code swaps, which rest on
JSD-0087, on which part B turns. No verdict moved at the review. The text above is left as written.

*(Noted 2026-11-11, at the owner's rulings of that day.)* The note above names the readings the bundle's README leaves
to the owner. Step 1's evidence merged as `acfc986b` (Broiler-Platform/Broiler.VM#197, observed). The owner ruled on the
five readings that parts B, C and H turn on, each time taking the reading the bundle took: control K4's "must still make
the fifth row fail", met by the harness narrowed to that row ("Narrowed run counts"); class R's "unchanged between the
registry commit and the after commit", read as a renumbering not undone ("Not undone"); the code pairs class N reads,
R2's alone, and the registry commit's "in each path only what the list says", resting on JSD-0087 ("Accept both", which
the README reads as taking that reading whole, the lines of the mapping and of the stage table's copy included); and
part H's "match (divergence no longer holds)", counted as a match ("Counts as match"). The README records each ruling
against its reading, and parts B, C and H's verdicts now rest on the owner's rulings, not on open readings; no verdict
moves. The readings that stay open are kind (F)'s "first diagnostic", the stages and the library's messages, the one
process class N names, the binder message several sites share, the idle compiler servers during parts I and J, and the
machine taken as the rule's local machine; the README marks none of them with the words that a part's verdict turns on
it, but the alternative of the reading on the idle compiler servers would void parts I and J. The owner's ruling on
these stays owed before bundle `sjl-13-002`'s runs. The text above is left as written.

*(Noted 2026-11-12, at the owner's rulings of that day.)* The note above reads "The readings that stay open are kind
(F)'s "first diagnostic", the stages and the library's messages, the one process class N names, the binder message
several sites share, the idle compiler servers during parts I and J, and the machine taken as the rule's local machine"
and "The owner's ruling on these stays owed before bundle `sjl-13-002`'s runs." On 2026-11-12 the owner ruled on all
six, each time taking the reading the bundle took: the five no verdict turns on, "Accept all five" (not taken: "Leave
them open"), and the idle compiler servers during parts I and J, which do not void them, "Accept as taken" (not taken:
"Void I and J, retake"). Every reading of the bundle is now the owner's own dated answer, which the README's section 5
records against each reading; parts I and J's verdicts rest on the second ruling, and no verdict moves. The native
floor's `atMost unsupported` row, which the bundle records as not re-set in the after commit, is re-set by hand to the
bytecode floor's row, late, in `c0bf379` on the branch `vm-s13r`, a change that is not part of step 2 and is to land
before step 2's base, by that branch merging before step 2's merge. Next: step 2 (WP8), under bundle `sjl-13-002`'s
rule, whose base is the first parent of step 2's merge, and WP9.4's corpus revision retiring the `syntax` divergence
fields. No clause is met, and nothing is accepted. The text above is left as written.

*(Noted 2026-11-12, later, at work package 9.4, this repository's half.)* The note above ends "Next: step 2 (WP8),
under bundle `sjl-13-002`'s rule, whose base is the first parent of step 2's merge, and WP9.4's corpus revision
retiring the `syntax` divergence fields." WP9.4's corpus revision is Broiler.JS's corpus revision 39, merged in
Broiler-Platform/Broiler.JS#1116 as `e4b6b49e` (observed). It retires every `divergence.broiler-vm` field of the
`syntax` area and those of the regular-expression area's row S9b, which step 1 corrects, on bundle `sjl-13-001`'s
part H and its section 5's finding and the owner's ruling of 2026-11-11, "Counts as match". Its row 39 also records
the owner's answers of 2026-11-12 to the questions it raised, none of which concerns this family's fields. This
family's vectors pin moved to it on the branch `vm-pin39` (`b781ebb`), not yet merged, where the conformance root
matches every vector of those fields with no divergence field and moves no other verdict, which ends WP9.4 in this
repository. The order: the pin moves before step 2's base, taken under the owner's standing direction of 2026-10-18 as
the recommended choice (not taken: "move the pin after step 2's merge"). A pin move merged before step 2 lands in
step 2's base and its after commit alike. No part of bundle `sjl-13-002`'s rule judges the vectors pin or the vector
runner's report; its part D runs the architecture tests, which read the pin, at both commits. Next: step 2 (WP8),
under bundle `sjl-13-002`'s rule. No clause is met, and nothing is accepted. The text above is left as written.

### SJL-13.4 — Admission and closures

- The allow-list admits the frontend for `Broiler.VM.Profile.JavaScript.Compiler` only.
- Rule `I3` proves that `JavaScript.ExecutionOnly` and `JavaScript.Android` do not carry it.
- The composition register declares it for the compiler-bearing roots.
- Notices are updated.

*(Noted 2026-11-10, at step 1's records, by [JSD-0087](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0087-the-wide-path-parses-with-the-shared-syntax-library.md).)* The allow-list admits `Broiler.ECMAScript.Frontend`
`0.1.0-preview.10` for `Broiler.VM.Profile.JavaScript.Compiler` alone, and the notices fold it into the shared stream's
row (rule N38); rule I3's frontend clause has its subject from the admission. The bullet "The composition register
declares it for the compiler-bearing roots." does not yet hold: no commit of step 1's branch moves
`docs/compositions.md`'s rows for those roots, and no closure of a root carrying the library is retained, which rule K4
holds the rows to. They move after the merge, in the records commit that retains the closures the collector publishes
at the after commit. JSD-0087 records it. The text above is left as written.

*(Noted 2026-11-10, later, at task 7.3 of step 1's evidence.)* The note above reads "They move after the merge, in the
records commit that retains the closures the collector publishes at the after commit." They moved at task 7.2 instead,
in the evidence branch's commit `2c087c4`, which retains in bundle `sjl-13-001` the closures of every root published in
three modes at the after commit and moves the four rows in the same commit, as JSD-0087's dated note records. From that
commit the bullet "The composition register declares it for the compiler-bearing roots." holds; the after commit
itself does not hold it. The text above is left as written.

### SJL-13.5 — JS-2 closes

The profile's ledger row JS-2 was `Blocked` on the seed copy. It closes under the gate that decision
[SJL-D-7](../decisions.md#sjl-d-7) re-scoped: "the general front end is the shared frontend". This
milestone's bundle is the evidence. JSC entries record the change.

### SJL-13.6 — Evidence

**In Broiler.VM:**

- the artifact comparison over the parse corpus, at each of the two cut-over steps;
- whole-suite test262 in both forms, with early-error verdicts compared per variant;
- the registry rules;
- the concurrency gate of the source front end;
- the source fuzz seeds and the fuzz host's source campaigns;
- every root published and run in three modes;
- rules `I1` to `I7`;
- the compiler's own measurement under its rule.

*(Read 2026-10-26, at SJL-13.1, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md), decisions D22 and D24.)* "The source fuzz seeds and the
fuzz host's source campaigns" are read as two sessions of the slice compiler: the existing slice session, which
reaches `SliceTokenizer` and the slice front end, and a wide-path session, `--fuzz-wide`, which the tools commit adds,
over the seeds bundle `sjl-13-001` commits with its rule; both run at both steps. No session reached the wide parser
before. "The compiler's own measurement under its rule" is a rule part of each bundle (template T5), not a gate
clause.

*(Noted 2026-11-10, later, at task 7.3 of step 1's evidence.)* At step 1, bundle `sjl-13-001` holds each item above:
the artifact comparison over the parse corpus (part B), whole-suite test262 in both forms compared per variant (part A),
the registry rules (parts D and E), the source front end's concurrency gate (part C), the two fuzz sessions (part G),
every root published and run in three modes (the closures, read beside the parts), rules I1 to I7 (part D), and the
compiler's own measurement (part J, not met, no gate clause). Its README judges each part; step 2's are bundle
`sjl-13-002`'s. The text above is left as written.

**In the home:** the library's runs.

---

## Exit gate

**Home clauses**

1. The frontend's syntax half is implemented under the contract and the conventions.
2. The parse corpus and test262's syntax tests pass at library level, as classified.
3. Rule SL9 (determinism) and the Native AOT probe pass.

**Broiler.VM clauses**

4. `JsParser` is gone, and the wide path parses with the frontend.
5. Artifacts are byte-identical over the parse corpus, or every difference is classified under the
   rule.
6. test262 in both forms meets the rule.
7. Rules N5 to N9, N12 and N19 hold. The registry revision is filed, if any.
8. Rule `I3` keeps the frontend out of the compiler-free roots. Every root is published and run in
   three modes.
9. Fuzzing runs clean under the rule.
10. JS-2 closes, with its JSC entries.

*(Read 2026-10-26, at SJL-13.1, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md), decision D33.)* Clause 10 is read as the record of
a closure that clauses 4 to 6 settle: JS-2's re-scoped gate is met by clauses 4 to 6 and JSD-0063's requirements on
bundles `sjl-13-001` and `sjl-13-002`, and clause 10 by the JSC entries and the profile ledger's closed JS-2 row that
SJL-13.5 files. There is no circularity.

---

## Evidence bundles

`docs/evidence/sjl-13-001/` and `-002/` (one per cut-over step) in Broiler.VM, and
`docs/evidence/sjl-13-001/` in the home.

---

## Does not do

- **It does not change Broiler.JS's front end.** That is SJL-14.
- **It does not share the static semantics.** That is SJL-15. Broiler.VM's binder pass from SJL-12
  stays in Broiler.VM until then.
- **It does not rebuild the slice path.** SJL-15 does, under decision `SJL-D-13` (a).

*(Read 2026-10-26, at SJL-13.1, before any evidence of SJL-13 existed, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
decisions D10, D20 and D39.)* "**It does not share the static semantics.** That is SJL-15. Broiler.VM's binder pass
from SJL-12 stays in Broiler.VM until then." is read with SJL-13.2's scope above. SJL-13 shares the parse-time early
errors, every one either engine's parser reports at parse time today, through a declarations walk in
`Broiler.ECMAScript.Semantics` that exposes scopes, declarations, parameter names and strictness as side tables
(decision D10). Step 1 deletes the binder's checks of every row the stage table assigns `syntax`, so the binder pass
keeps only its checks of `semantics` rows and its facts for the lowering (decision D20). The semantic model, and the
binder's remaining checks, stay SJL-15's.
