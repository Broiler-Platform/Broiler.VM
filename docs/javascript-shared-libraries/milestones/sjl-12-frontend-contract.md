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
- It references `Broiler.JavaScript.Runtime` for six types: `CoreScript`, `CancellableDisposableAction`,
  `RegExpUnicodeValidator`, `JSUnicode`, `JSUnicodePropertyNames` and `RegExpGroupNames`. *(Corrected
  2026-10-19, at SJL-12.1, by
  [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
  from the Parser assembly's metadata at Broiler.JS `24510b0e`. The first three are the three the area design
  named, read at `44a214dc`. `JSUnicode` and `JSUnicodePropertyNames` came with SJL-7.5, Broiler.JS #1040 of
  2026-10-17, and `RegExpGroupNames` with the regular-expression literal's group-name check, Broiler.JS #1041 of
  the same day. The bullet read "It references `Broiler.JavaScript.Runtime` for exactly three types.")*
- It desugars `for` loops while parsing.
- Its early errors are spread across the parser, a separate validation pass and the compiler.

**Broiler.VM's wide path is also not ready to share.**

- Its declaration analysis and its instruction emission share methods: `HoistProgram`,
  `HoistFunction` and `HoistEval` collect declarations and emit in the same body.
- Its early errors are split between the parser and the lowering.
- Its hoisting order relies on dictionary insertion order. Backend stage JSB-1, not started, is
  meant to fix that.

*(Noted 2026-10-25, at SJL-12.6's records, and the three bullets kept as written.)* They describe the wide path
before SJL-12.6. At bundle `sjl-12-001`'s after commit, `1de3af6`:
- "Its declaration analysis and its instruction emission share methods": the binder pass, `JsBinder`, collects
  the declarations, and `HoistProgram`, `HoistFunction` and `HoistEval` read its facts and emit (item 1, `3fa4b39`).
- "Its early errors are split between the parser and the lowering": the lowering's early errors are the binder
  pass's, after the parse, and the parser keeps its own checks (item 2, `daa92eb`; JSD-0080 decision D10).
- "Its hoisting order relies on dictionary insertion order. Backend stage JSB-1, not started, is meant to fix
  that": the order is the source's, and the two checks rows JSB-1's gate names exist (item 3, `066abef`).
  JSD-0080 leaves JSB-1's judgement to the profile's ledger, which holds no JSB-1 row at this date, so no record
  has judged it ([JSC-319](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-319)).

None of this is judged here. Section "SJL-12.6" below says where it is.

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

*(Added 2026-10-19, at SJL-12.1, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
decisions D5 and D41.)* The three places are JSD-0080, Broiler.JS's plan section "The frontend contract (SJL-12)",
and the home's design document, `Broiler.ECMAScript/docs/frontend/design.md` in the Broiler.JS repository. No
`Broiler.ECMAScript.Frontend/` project folder is created at SJL-12, because SJL-12 writes no library code. The design
document is owed by this done-when, not by an exit clause: clause 3 names Broiler.JS's plan, and clause 6 the JSD.
JSD-0080 also files the semantic model's design.

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
   - `RegExpUnicodeValidator`, `JSUnicode`, `JSUnicodePropertyNames`, `RegExpGroupNames` and the Unicode tables move
     to `Broiler.JavaScript.Ast`, the public validator keeping a forwarder in `Runtime`. The provider seam the
     options carry is typed by Broiler.Unicode's `IUnicodeTables`, and its default is the table `Ast` holds.

   *(Revised 2026-10-19, at SJL-12.1, before any evidence of SJL-12 existed, by
   [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
   decision D20. The reference is to six types, not three. A seam with no default, or one that throws, would break
   the portable compiler, which has no `Runtime` reference, and the parser's Unicode and regular-expression tests.
   `CancellableDisposableAction` moves to `Ast` with a forwarder, and `UndoMark` is marked obsolete (SJL-D-25 (b)).
   The third bullet read "`RegExpUnicodeValidator` is replaced by the provider seam.")*
5. **`for`-loop desugaring moves** out of the parser into a preparation pass of the compiler, so the
   parser produces an undesugared tree.
6. **An architecture test** in Broiler.JS: the parser assembly declares no mutable static and no
   `AsyncLocal`. This is the shape of Broiler.VM's rule N12.

*Done when* every item has landed and Broiler.JS's test262, per path and with negative tests, shows no
regression against a base taken at the commit that carries Broiler.JS's rule (bundle `sjl-12-001`), under the
predeclared rule.

*(Revised 2026-10-19, at SJL-12.1, before any evidence of SJL-12 existed, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
decision D7. This is a work package's done-when, corrected in place under roadmap section 13, and not a gate
revision: exit clause 2, "test262 per path meets the predeclared rule", names no base. SJL-0's base predates every
cut-over since SJL-3, those of SJL-3 to SJL-8 and SJL-10, and excludes negative tests, and Broiler.JS's comparison
script refuses two runs whose `includeNegative` differs. The sentence read "*Done when* every item has landed and
Broiler.JS's test262, per path, shows no regression against SJL-0's base under the predeclared rule.")*

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

   *(Added 2026-10-19, at SJL-12.1, by
   [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
   decision D15.)* Its listed corrections:
   - `${yield}` and `${await …}` in formal parameters are refused with 2101;
   - template nesting counts toward the nesting bound, and past it is refused with 2103;
   - the shared tokenizer's template scan becomes iterative, so a deeply nested template is refused rather than
     ending the process.

   Positions inside substitutions may only become correct. No public member changes.

*Done when* every item has landed and the following hold:

- Broiler.VM's whole-suite test262 in both forms meets the predeclared rule;
- every input of the parse corpus gives the same artifact bytes as at base, or a listed, classified
  difference;
- two compilations of the same source give identical bytes in two runs and on two threads;
- rules N5 to N9, N12 and N19 hold.

*(Added 2026-10-25, at SJL-12.6's records, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md);
the work package's text is unchanged.)* **The four items have landed**, item 3 first and alone (decision D14), each
in a default-branch merge of its own:
- **Item 3**, `066abef` (Broiler.VM#163), bundle `sjl-12-001`'s ordering commit: the global lexical declarations
  and an evaluated program's lexical names in source order, and the checks rows "the wide lowering is
  deterministic" and "the wide lowering declares lexical names in source order". The item read "This is the
  relationship to stage JSB-1, which is recorded": it is recorded in
  [JSC-319](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-319), which corrects the
  backend roadmap's JSB-1 text. JSD-0080 leaves the stage's judgement to the profile's ledger, which holds no
  JSB-1 row at this date, so no record has judged it.
- **Item 1**, `3fa4b39` (Broiler.VM#165): `JsBinder`'s facts, read by `HoistProgram`, `HoistFunction` and
  `HoistEval`, which keep allocation, interning and emission order (decision D11). Item 1 reads that the binder
  collects "private-name, label, `super`, `return` and `new.target` validity" and "the `const`-initialiser check":
  the facts landed with item 1, and the checks with item 2, as decisions D11 and D12 divide them.
- **Item 2**, `daa92eb` (Broiler.VM#166): every language early error the lowering reported is the binder pass's,
  with its code, and a unit the binder refuses is not lowered. What decision D12 keeps in the lowering stays there
  (decisions D10 to D13).
- **Item 4**, `1de3af6` (Broiler.VM#167), bundle `sjl-12-001`'s after commit: structured templates, whose
  substitutions the parser reads in place, carrying the nesting depth, the tree depth and the parameter context
  into them, with decision D15's listed corrections; and `JsCompiler.Requests` on `CompilationStack`'s thread
  (decision D16). That carry has two consequences D15 does not list: an input already refused whose diagnostic
  list changes (`964b460`), and an input accepted before that the tree-depth bound refuses (`964b460`,
  `3902f6f`). The bundle names both as findings against JSD-0080, before the after runs, in its
  [readings named before the after runs](../../evidence/sjl-12-001/README.md#readings-named-before-the-after-runs-sjl-126-2026-10-25).
  Item 4's pre-checks found neither in the parse corpus or in the pinned suite's negative tests.

**Nothing here says the done-when holds.** Its four conditions are read on bundle `sjl-12-001`'s parts A to D,
whose after runs had not been taken when this note was written. The bundle's section 3 names, before those runs,
how the rule's classes read the edges item 4's review found. SJL-12 cannot meet its gate before SJL-3 meets
SJL-3's.

---

## Exit gate

**Broiler.JS clauses**

1. The parser reads no ambient state, holds no mutable static, references no `Runtime` type, and
   does not desugar. A Broiler.JS architecture test enforces this, with a witness.
2. test262 per path meets the predeclared rule.
3. The contract record is in Broiler.JS's plan.

*(Added 2026-10-19, at SJL-12.1, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
decisions D23 and D24. The clause's text is unchanged.)* Clause 1 is read this way:
- **"Does not desugar"** is SJL-12.5 item 5's `for`-loop desugaring. The `using`-in-`for` block wrap moves with it.
- **The parser's two other rewrites stay**, listed for SJL-14, whose shared tree keeps raw facts: the Annex B.3.4
  block it synthesizes for a function declaration that shadows a parameter, and the regular-expression literal's
  `\u{…}` decoding. Neither introduces a temporary or restructures control flow.
  *(Noted 2026-10-25, at SJL-11.1, by
  [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md)'s
  decision D22. The item reads "The parser's two other rewrites stay, listed for SJL-14", the second of them being
  "the regular-expression literal's `\u{…}` decoding". SJL-11.5 deletes that one, with the scanner's
  parenthesis-balance patch, or SJL-14 does if SJL-11 has not; it stays at SJL-12, as decided, and this clause's
  reading does not change.)*
- **"Reads no ambient state"** is read over the Parser assembly's own declarations and references. The stack
  segmentation's flag and environment variable, the compile thread's statics, `Ast`'s empty instances and the
  compiler's read of the current engine for direct `eval` are outside it.

**Broiler.VM clauses**

4. The binder pass is separate from emission. The wide path's early errors run in one stage.
   Ordering is deterministic.
5. test262 in both forms meets the predeclared rule. The parse corpus's artifacts are byte-identical,
   or every difference is classified. Rules N5 to N9, N12 and N19 hold.
6. The contract JSD is filed. Decisions `SJL-D-11` and `SJL-D-12` are recorded.

*(Added 2026-10-19, at SJL-12.1, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
decision D10. The clause's text is unchanged.)* Clause 4's "The wide path's early errors run in one stage" is read as
SJL-12.6 item 2 states it:
- **After parsing, the wide path has one early-error stage, the binder pass.** Every early error the lowering raises
  today moves there, with its code.
- **The wide parser keeps its own static-semantic checks** until SJL-13 deletes `JsParser` and SJL-15's model takes
  every early error that is not purely grammatical.
- **JSD-0014's decision 1 is a requirement on the shared front end**, which SJL-13 and SJL-15 meet.

Moving the parser's checks now would be work SJL-13 deletes, and would change the first code of refused inputs with
no class to admit it.

**Shared clauses**

7. The early-error catalogue and the diagnostic mapping are retained.
8. The parse corpus and its baselines are retained in both repositories.

---

## Evidence bundles

- **Broiler.JS** `docs/evidence/sjl-12-001/`: the test262 comparison, the architecture-test witness
  and the parse-corpus baseline.
- **Broiler.VM** `docs/evidence/sjl-12-001/`: the test262 comparison, the artifact comparison, the
  determinism runs, the rule runs and the parse-corpus baseline.

*(Added 2026-10-19, at SJL-12.1, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
decisions D8, D37 and D38.)* **The rules are committed with SJL-12.1**, before any code they judge. No work package
above names that step, and SJL-12.5 and SJL-12.6 are judged "against the predeclared rule" (validation principle 3;
roadmap section 6 item 10).
- **Broiler.VM's rule** is
  [`docs/evidence/sjl-12-001/decision-rule-area.md`](../../evidence/sjl-12-001/decision-rule-area.md). Its part E also
  checks the diagnostic mapping, validation section 7's control for this milestone.
- **Broiler.JS's rule** is `docs/evidence/sjl-12-001/decision-rule.md`. It adds the determinism of its parser and of
  its compiler's `for`-loop pass.

*(Added 2026-10-26, at SJL-12.7, when Broiler.VM's after evidence was recorded.)* **Broiler.VM's after runs are
taken, and its bundle judges each part of its rule MET as written, with no revision**
([`docs/evidence/sjl-12-001/README.md`](../../evidence/sjl-12-001/README.md), sections 6 to 8). The after commit is
`1de3af6`, the merge of SJL-12.6's last item.
- **The runs.** Part A's after dispatch of the hosted workflow, in both forms, at the after commit, on 2026-10-25;
  part B's and part C's runs at the after commit, part C's two controls in a scratch copy of it, part D's rule runs at
  the after commit and at the rule commit, part E's checker, and part A's re-runs of every wall-clock row at both
  commits, all on the programme's local machine, idle, on 2026-10-25 and 2026-10-26 (the bundle's section 6).
- **Part A, test262 in both forms: MET** (section 7). Every changed verdict is a race between the wall clock and
  another allowance, class C4 (c'), whose re-runs agree, or, for one row, whose (e) runs give the same verdict at both
  commits. No verdict moved between a pass, a failure and a refusal.
- **Part B, artifact identity over the parse corpus: MET** (section 7). No artifact's bytes and no outcome differ, and
  the slice front end and the census show no difference. Every difference is in a refused input's diagnostic list, and
  each is classified: an early error the binder pass reports where the lowering had stopped at its first refusal (S4),
  or a diagnostic the lowering repeated now reported once (S2). Both are SJL-12.6 item 2's (JSD-0080 decision D12).
- **Part C, determinism: MET** (section 7). Every accepted compilation gives one hash in two runs and on two
  threads, both checks rows pass, and controls K1 and K2 fail as stated and pass after their revert.
- **Part D, rules N5 to N9, N12 and N19: MET** (section 7), on the run after a build of the solution, as bundle
  `sjl-3-001`'s collector runs the rules; the seven rules' tests pass on the test project's own build too.
- **Part E, the diagnostic mapping: MET** (section 7). The checker passes on the mapping and refuses both controls.
- **The findings against JSD-0080 stand and decide no part** (section 8): no row of the after runs is of edge 1 or
  edge 2 (ii) of the bundle's section 3, and no record judges stage JSB-1.

Whether those parts meet this family's exit clauses 4 to 6 and its half of clauses 7 and 8 is read in the programme's
ledger. Broiler.JS's clauses are judged in its own bundle. **SJL-12 cannot meet its gate before SJL-3 meets SJL-3's**
(JSD-0080), so the gate waits on SJL-3's exit whatever both bundles show.

---

## Does not do

- **It writes no shared library code.** The shared syntax library is SJL-13.
- **It changes no language surface** either engine accepts, beyond listed corrections.
- **It does not change Broiler.VM's slice path.** SJL-15 rebuilds it on the shared tree and model,
  under decision [SJL-D-13](../decisions.md#sjl-d-13) (a).

  *(Added 2026-10-19, at SJL-12.1, by
  [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
  decision D15.)* SJL-12.6 item 4 changes one thing both paths share: `SliceTokenizer`'s template scan becomes
  iterative. It is a listed slice correction in which only a crash becomes a refusal. **No slice outcome changes for
  any input that completes today**, and bundle `sjl-12-001`'s rule holds the slice front end and its construct census
  to no difference at all.
