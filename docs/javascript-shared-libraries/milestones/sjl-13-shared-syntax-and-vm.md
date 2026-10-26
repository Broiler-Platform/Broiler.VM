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

### SJL-13.4 — Admission and closures

- The allow-list admits the frontend for `Broiler.VM.Profile.JavaScript.Compiler` only.
- Rule `I3` proves that `JavaScript.ExecutionOnly` and `JavaScript.Android` do not carry it.
- The composition register declares it for the compiler-bearing roots.
- Notices are updated.

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
