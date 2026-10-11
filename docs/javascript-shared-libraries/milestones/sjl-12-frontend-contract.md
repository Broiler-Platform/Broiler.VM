# Milestone SJL-12 — The frontend contract and in-place preparation

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/frontend.md) · [Static semantics](../areas/static-semantics.md)

- **Objective.** Three things are true at the end of this milestone:
  - **The contract is recorded.** The shared front end's contract is filed as a record: options,
    goals, inherited evaluation context, limits, providers, the tree, positions, diagnostics and the
    semantic facts. The canonical tree and the diagnostic policy, decisions `SJL-D-12` and `SJL-D-11`
    taken on 2026-10-11, are recorded.
  - **Both front ends are prepared in place.** Broiler.JS's parser has no ambient state, no
    process-wide counter and no runtime reference, and no longer desugars while it parses.
    Broiler.VM's declaration analysis is separated from its emission and orders its output
    deterministically.
  - **Nothing regressed.** Both engines' suites show no regression.
- **Owner roles.** The Broiler.JS maintainer and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS, Broiler.VM.
- **Waits on.** SJL-0 to start: its in-place work packages need no admission. SJL-3 to exit, because
  the contract cites the literal-value operations. It applies two decisions taken on 2026-10-11, and
  records them: [SJL-D-11](../decisions.md#sjl-d-11) (codes and message identifiers) and
  [SJL-D-12](../decisions.md#sjl-d-12) ((a) a new immutable tree, (i) adapter packages for
  Broiler.JS's `Ast` and `Parser`).

**Why preparation comes first.** The feasibility report: "Directly referencing the existing
Broiler.JS parser would be premature." Its reasons are confirmed by the review, with additions:

- Broiler.JS's parser reads the module goal and top-level-await permission from `AsyncLocal` state
  in `CoreScript`.
- It names desugaring temporaries from a process-wide counter, so its output depends on what was
  parsed earlier in the process.
- It writes to a shared static end-of-file token on every parse.
- It references `Broiler.JavaScript.Runtime` for exactly three types.
- It desugars `for` loops while parsing.
- Its early errors are spread across the parser, a separate validation pass and the compiler.

**Broiler.VM's wide path is also not ready to share.**

- Its declaration analysis and its instruction emission share methods: `HoistProgram`,
  `HoistFunction` and `HoistEval` collect declarations and emit in the same body.
- Its early errors are split between the parser and the lowering.
- Its hoisting order relies on dictionary insertion order. Backend stage JSB-1, not started, is
  meant to fix that.

**Broiler.VM's decision JSD-0014 already rejects** "the seed's four-place early-error split and its
ambient parse state". This milestone adopts that decision as a requirement on the shared front end.

---

## Work packages

### SJL-12.1 — The frontend contract record

Filed as a JSD in the profile series, in Broiler.JS's shared-libraries plan, and as the frontend
library's design document in the home. The full design is in
[areas/frontend.md](../areas/frontend.md#4-the-contract).

**Input: an immutable parse request.**

- **The source:** text, source name, base offset, line and column.
- **The goal**, one of Script, Module, Eval, FormalParameters or FunctionBody. The last two exist
  because neither engine parses `CreateDynamicFunction` as the specification says today: Broiler.VM
  concatenates the source, and Broiler.JS compiles several variants.
- **Switches:** forced strictness, top-level `await`, Annex B, HTML-like comments.
- **The inherited evaluation context:**
  - Broiler.VM's six evaluation flags;
  - the visible private names;
  - the caller's lexical names.
- **Proposal flags:** decorators, `accessor`, import phases. Broiler.JS's `m` suffix arrives as a
  lexer extension parameter (see [SJL-D-17](../decisions.md#sjl-d-17)).
- **Limits:** nesting depth and tree depth, as constants or bounded parameters, preserving rule
  N19's "not host-settable" property for Broiler.VM. A diagnostic cap. A first-error or all-errors
  policy.
- **Providers:** Unicode lexical classes, and regular-expression syntax validation.

**Output.**

- **An immutable tree** with full spans, the raw facts, and identity-keyed side tables.
- **Diagnostics** carrying a stable code, a phase, a span, a source name and a message identifier
  with arguments, in a deterministic order.
- **A semantic model**, defined in [areas/static-semantics.md](../areas/static-semantics.md).

**What each engine keeps** is in [areas/frontend.md](../areas/frontend.md#6-what-each-engine-keeps).

*Done when* the contract is filed in all three places.

### SJL-12.2 — Decisions SJL-D-11 and SJL-D-12

These decisions, taken on 2026-10-11, are recorded in the contract JSD and in Broiler.JS's plan:

- [SJL-D-11](../decisions.md#sjl-d-11): codes plus message identifiers, with each engine rendering
  its own text.
- [SJL-D-12](../decisions.md#sjl-d-12) (a) with (i): a new immutable tree seeded from Broiler.VM's
  wide design; Broiler.JS's public `Ast` and `Parser` packages become adapter packages, marked
  obsolete, and are removed in a later major version.

### SJL-12.3 — The early-error catalogue and the diagnostic mapping

**The catalogue** has one row per early error in ECMA-262 and its Annex B. Each row names:

- where Broiler.VM's slice path checks it today;
- where Broiler.VM's wide path checks it (parser or lowering);
- where Broiler.JS checks it (parser, `SyntaxValidation`, `FastCompiler.EarlyErrors`, a `Visit*`
  method, or `DirectEvalSupport`);
- the shared code it maps to.

**The mapping** goes from shared codes to Broiler.VM's registry codes (2001 to 2406). It records
every case where one shared code splits or merges registry rows. Under JSD-0009 that is a new
registry number, never a changed meaning.

*Done when* every early error has a row.

### SJL-12.4 — The parse corpus

A corpus retained for every later comparison, built from:

- test262's negative syntax tests and every positive parse test, at the pin;
- Broiler.VM's `SliceSourcePrograms` and the retained source corpus `src/tests/corpus/js-1/source`;
- Broiler.JS's parser and compiler test inputs.

**Baselines** are taken at a named commit:

- each engine's acceptance or refusal of each input;
- Broiler.VM's diagnostic code;
- Broiler.VM's artifact bytes for every accepted input, because Broiler.VM requires byte-identical
  lowering.

### SJL-12.5 — Broiler.JS: in-place preparation

1. **Explicit parse options.** They are threaded through a new `IJSCompiler` overload and the
   `FastParser` constructor. The `CoreScript` reads in `FastParser`, `FastScanner` and the `for` and
   expression productions go.
2. **One counter per parse.** It replaces the static `TempVarID`.
3. **The end-of-file race goes.** The parser no longer writes the shared static token's `Previous`.
4. **The `Parser` → `Runtime` reference goes.** This is Broiler.JS's own MOD-M2-4.
   - `CoreScript`'s goal state becomes an option.
   - `CancellableDisposableAction` moves to a lower assembly or is replaced.
   - `RegExpUnicodeValidator` is replaced by the provider seam.
5. **`for`-loop desugaring moves** out of the parser into a preparation pass of the compiler, so the
   parser produces an undesugared tree.
6. **An architecture test** in Broiler.JS: the parser assembly declares no mutable static and no
   `AsyncLocal`. This is the shape of Broiler.VM's rule N12.

*Done when* every item has landed and Broiler.JS's test262, per path, shows no regression against
SJL-0's base under the predeclared rule.

### SJL-12.6 — Broiler.VM: in-place preparation

1. **A binder pass**, separated from `JsCompiler`'s emission. It collects:
   - declarations and Annex B.3.3 aliases;
   - `arguments` and direct-`eval` analysis;
   - private-name, label, `super`, `return` and `new.target` validity;
   - the `const`-initialiser check.

   It produces facts that emission reads. `HoistProgram`, `HoistFunction` and `HoistEval` then emit
   only.
2. **Wide-path early errors move into the binder pass** from the lowering, so that the wide path
   has one early-error stage after parsing, as JSD-0014 requires of the slice path.
3. **Deterministic ordering.** Every output order that comes from dictionary enumeration is
   replaced by source order. This is the relationship to stage JSB-1, which is recorded.
4. **Structured template tokens**, so the wide parser stops re-tokenising substitutions.

*Done when* every item has landed and the following hold:

- Broiler.VM's whole-suite test262 in both forms meets the predeclared rule;
- every input of the parse corpus gives the same artifact bytes as at base, or a listed, classified
  difference;
- two compilations of the same source give identical bytes in two runs and on two threads;
- rules N5 to N9, N12 and N19 hold.

---

## Exit gate

**Broiler.JS clauses**

1. The parser reads no ambient state, holds no mutable static, references no `Runtime` type, and
   does not desugar. A Broiler.JS architecture test enforces this, with a witness.
2. test262 per path meets the predeclared rule.
3. The contract record is in Broiler.JS's plan.

**Broiler.VM clauses**

4. The binder pass is separate from emission. The wide path's early errors run in one stage.
   Ordering is deterministic.
5. test262 in both forms meets the predeclared rule. The parse corpus's artifacts are byte-identical,
   or every difference is classified. Rules N5 to N9, N12 and N19 hold.
6. The contract JSD is filed. Decisions `SJL-D-11` and `SJL-D-12` are recorded.

**Shared clauses**

7. The early-error catalogue and the diagnostic mapping are retained.
8. The parse corpus and its baselines are retained in both repositories.

---

## Evidence bundles

- **Broiler.JS** `docs/evidence/sjl-12-001/`: the test262 comparison, the architecture-test witness
  and the parse-corpus baseline.
- **Broiler.VM** `docs/evidence/sjl-12-001/`: the test262 comparison, the artifact comparison, the
  determinism runs, the rule runs and the parse-corpus baseline.

---

## Does not do

- **It writes no shared library code.** The shared syntax library is SJL-13.
- **It changes no language surface** either engine accepts, beyond listed corrections.
- **It does not change Broiler.VM's slice path.** SJL-15 rebuilds it on the shared tree and model,
  under decision [SJL-D-13](../decisions.md#sjl-d-13) (a).
