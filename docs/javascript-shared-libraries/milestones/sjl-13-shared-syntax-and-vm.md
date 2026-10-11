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
