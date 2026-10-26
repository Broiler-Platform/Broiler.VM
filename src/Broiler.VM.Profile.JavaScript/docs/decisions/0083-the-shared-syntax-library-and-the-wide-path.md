<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0083 - The shared syntax library and the wide path: SJL-13's area verdict, sub-area by sub-area, and the owner's decisions of 2026-10-26

**Status:** Decided 2026-10-26.
- **All five sub-areas are accepted**, each under X1 to X7 on its own (decision D4):
  - **(T)** the tokenizer (rows T);
  - **(P)** the parser and its tree (rows P);
  - **(N)** positions (rows N);
  - **(D)** diagnostics and the parse-time early errors (rows D and B);
  - **(R)** the request, its switches, limits and providers (rows R).
  They are one library, so a refusal of any of them would have been a refusal of SJL-13.
- **They move to a new library, `Broiler.ECMAScript.Frontend`**, in the Broiler.JS repository, the home (decision
  D6), under the contract [JSD-0080](0080-the-shared-frontend-contract-and-the-in-place-preparation.md) recorded. The
  syntax half reports the grammar and every early error either engine's parser reports at parse time today, sorted
  row by row by a stage table in the home (decision D10). The engines keep their admission, their ceilings, their
  rendering and their policies, as parameters of the request or in their own code.
- **This family's wide path parses with it in two steps** (decision D19): at step 1 an adapter builds `JsSyntax` from
  the shared tree, so the lowering is unchanged and `JsParser` is deleted, with the binder's copies of the checks the
  library now makes (decision D20); at step 2 the lowering, the numeric admission and the binder read the shared tree,
  and the adapter and `JsSyntax` go. Registry revision 19 is minted at step 1's first commit (decision D12).
- **ADR 0011's refusal of "a shared syntax tree or parser" does not reach the library**, by the scope reading that ADR
  0014 records (decision D5).
- **The owner's decisions D2 to D39 of 2026-10-26 are recorded**, under the owner's direction of 2026-10-18. Where the
  correspondence's findings F1 to F32, or the stage table's items S1 to S4, showed a recommendation's text to be wrong
  or incomplete, it was decided again the same day and is recorded as revised, with the text it replaces quoted: D7,
  D10 to D18, D20, D21, D23, D25, D26, D28 and D39. No new decision number was needed. D41 and D44 to D47, which bind
  SJL-14, were revised the same day in answer to findings, and are recorded here as those answers.
- **Decision D1 is a reading of the owner's words of 2026-10-25**, recorded as a reading and not as a decision taken
  under the direction.
- **This family's corrections are named before any run**: rows T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31,
  D32 and D40, made by step 1. The corrections of this family that SJL-13 does not make are handed on (hand-offs H18
  to H21), and every correction of Broiler.JS is SJL-14's (hand-off H22).
- Nothing in this profile changes with this record. The library is SJL-13.2's, the cut-overs are SJL-13.3's own
  changes, and this record is what they apply.

**Date:** 2026-10-26.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the owner of the home and of the other engine;
- the verification-boundary owner, as the holder of
  [JSD-0014](0014-the-source-front-end-and-the-verification-boundary.md)'s decision 1, which decision D33 reads;
- the conformance owner, as the holder of the diagnostic registry ([JSD-0009](0009-the-diagnostic-registry-and-the-position-encoding.md)),
  which decision D12 revises;
- the Broiler.Unicode maintainer, for the identifier classes the library reads through the request's provider
  (decision D8).

**All five roles are held by one person** (EX-30), and this record does not claim that anyone independent has read
it.

**Milestone:** the programme's SJL-13, work package SJL-13.1, with SJL-13.0's start.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section
  "Area verdicts", with that repository's half of the decisions that bind it (D6 to D17, D25, D26 and D28), by these
  numbers. It is filed in Broiler.JS's own change, on its branch `sjl-13-1-js`, and is observed until it merges; the
  ledger names its merge commit when it has. The same change carries the stage table
  (`Broiler.ECMAScript/docs/frontend/stages.tsv`), the parse corpus's classification and the home's rule.
- **SJL-13 started before SJL-12, SJL-3 and SJL-7, which it waits on, met their gates**, at the owner's words of
  2026-10-25, which the programme ledger's SJL-13.0 entry records and reads (decision D1). SJL-13 may not meet its
  gate before SJL-12, SJL-3 and SJL-7 meet theirs. This record moves no gate.

**The owner's words of 2026-10-25.** The owner wrote: "Continue with the remaining SJL milestones". The programme
reads them as the choice to start SJL-13, and the same words, read the same way, start SJL-14 and SJL-15 when each
files its own start entry (JSD-0081's decision D2). That is a reading of words, recorded as one (decision D1).

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." This record reads it in two parts, as JSD-0080, JSD-0081 and JSD-0082 did:
- each decision SJL-13's plan leaves open is taken as the plan recommends it, and recorded below as the owner's
  decision, dated 2026-10-26, the day it is filed. The correspondence's findings F1 to F32 and the stage table's items
  S1 to S4 were each answered the same day, as recommended: by a revision of the decision a finding bears on, or as a
  record only;
- the programme proceeds through SJL-13's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-13 is accepted by it.
- SJL-13 still may not meet its gate before SJL-12, SJL-3 and SJL-7 meet theirs.
- Rules still come before runs. Bundles `sjl-13-001` and `sjl-13-002`'s rules, and the home's, are committed before
  any code they judge.
- **Three kinds of entry are not taken under it**: the start (decision D1), which is a reading of the owner's words;
  the owner's own choice on SJL-14's measurement clause, which SJL-D-23's note of 2026-10-20 reserves and which
  SJL-14.1 puts to the owner; and each publication of the shared stream (decision D30).

**Names.**
- **Decisions.** "Decision Dn" is always this record's. Another record's decision is named with its record, as in
  "JSD-0080's decision D27". The numbers are those of SJL-13's plan, which numbers D1 to D74 across SJL-13 to SJL-16;
  this record files D1 to D39, and the revisions of D41 and D44 to D47 that the findings required. D40 to D56 are
  SJL-14's, filed by Broiler.JS's SJL-14 section at SJL-14.1; D57 to D66 are SJL-15's and D67 to D74 SJL-16's, filed
  by their first work packages; a reference to one of them here names that later decision.
- **Rows.** The correspondence's rows are named by their letters: T for sub-area (T), P for (P), N for (N), D and B
  for (D) (B01 to B34 are the binder's checks), R for (R). Its section D's lists are named D-a to D-h, and its
  sub-rows `D12-1` to `D12-10` and `D18-1` to `D18-7` are decision D12's and decision D18's lists.
- **Findings.** F1 to F32 are the correspondence's findings against the plan and the records, in its section "What the
  rows find against the plan and the records". S1 to S4 are the items the stage table's step reported.
- **Hand-offs.** H1 to H24 are the rows of the correspondence's hand-off table, which this record carries (section
  "The hand-offs").
- **Steps.** A step of SJL-13 is named by its work package, SJL-13.0 to SJL-13.6. SJL-13.2's library is taken in six
  stages, one merge each: **a**, the skeleton and the request; **b**, the tokenizer; **c**, the tree and the parser;
  **d**, the diagnostics and the parse-time early errors; **e**, the limits, determinism and fuzzing; **f**, the
  done-when, whose merge is the library commit. SJL-13.3 has two cut-over steps, **step 1** and **step 2**, each one
  merge; step 1's first commit is **the registry commit**, and **the tools commit** lands before step 1's base
  (decisions D27 and D35). SJL-13's plan is held in no repository, so its task numbers appear here only where a
  decision quotes its text.
- **Rules.** "Bundle `sjl-13-001`'s rule" is this family's
  [`decision-rule-area.md`](../../../../docs/evidence/sjl-13-001/decision-rule-area.md), for step 1; "bundle
  `sjl-13-002`'s rule" is [its step 2 rule](../../../../docs/evidence/sjl-13-002/decision-rule-area.md); "the home's
  rule" is Broiler.JS's `docs/evidence/sjl-13-001/decision-rule.md`.
- **Classes.** C1 to C5 are the classes of the programme's comparison templates
  ([`validation.md`](../../../../docs/javascript-shared-libraries/validation.md) section 4). R, R2, S1 and S3 to S8, F
  and L are the classes of part B of bundle `sjl-13-001`'s rule; (R), (G), (B) and (F) the kinds of its part A's C1.
- **Paths.** A line number is the line at `4cf9ca4` in this repository and at `d6795fc8` in Broiler.JS, as the
  correspondence gives them.

---

## Context

The area's design is the programme's
[`areas/frontend.md`](../../../../docs/javascript-shared-libraries/areas/frontend.md), and its contract is JSD-0080's,
filed at SJL-12.1: an immutable parse request with goals, switches, the inherited context of eval code, proposal flags,
a lexer extension, limits and providers; a tree with full spans and raw facts and no desugaring; diagnostics with
stable codes and message identifiers; determinism, no ambient state, limits checked before the stack is. Two earlier
records bear on it:
- [JSD-0081](0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md)'s decision D18 placed this
  family's literal validation on Broiler.Regex's validate entry from SJL-11.4, behind the seam JSD-0080's decision D41
  names for SJL-13 (decision D7);
- [JSD-0063](0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md) re-scoped milestone JS-2 to "the general
  front end is the shared frontend", which SJL-13's bundles settle (decision D33).

**The correspondence is from code, and every row whose answer no retained run already held was run.** It reads:
- this family at **`4cf9ca4`**, the default branch after SJL-9.1's records, whose product code is SJL-12's after
  commit `1de3af6`'s;
- Broiler.JS at **`d6795fc8`**, whose parser is SJL-12's after commit `c29d653f`'s and whose compiler changed in four
  merges since;
- the early-error catalogue at SHA-256 `7a9109b0db685df98a7327cd95b9337cc538337f834fe4f22692b249058aa4f1`, the mapping
  and the registry at revision 18, the parse corpus at manifest SHA-256
  `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc`, test262 `ccaac100`, and ECMA-262 ES2026 at
  `tc39/ecma262@0248456c` as this family archives it.

Each probe was run once in each engine, serially, from one Release build of each in a scratch worktree: this family
through the slice compiler's `--parse-corpus` mode (sub-populations B1, B3 and B4), the conformance root and the
command-line root; Broiler.JS through its shell and SJL-12's parse-corpus tool (entries P and K). Where bundle
`sjl-12-001`'s after baselines already held an answer, the row cites them instead.
[`correspondence.md`](../../../../docs/evidence/sjl-13-001/correspondence.md), which this record's change retains in
bundle `sjl-13-001` with every probe, both engines' answers and its analysis tables
([`correspondence-runs/`](../../../../docs/evidence/sjl-13-001/correspondence-runs/)), names every member by file and
line and every row with its class. Where a run disagreed with the reading, the code was read again and the row states
what the run showed.

**The default branches after the pinned revisions**, and no move reaches a row:
- this family's is `4cf9ca4`, the head read and run;
- Broiler.JS's moved to `8a95076c` (Broiler-Platform/Broiler.JS#1079) while the correspondence was written. That
  merge changes `ForLoopPreparation.cs`, two compiler tests and the changelog: a label of a C-style `for (using …)` or
  `for (await using …)` loop now labels the loop. The catalogue, the scanner, the parser, `SyntaxValidation.cs` and
  every other early-error site are byte-identical at both heads, and no probe input and no corpus input holds such a
  loop, so no row's Broiler.JS answer can differ there; no probe was run again.

**The stage table and the classification** are Broiler.JS's, filed in its SJL-13.1 change (observed at `684667e2` on
its branch `sjl-13-1-js`, cut from `8a95076c`, where the stage table's SHA-256 is
`3c9c49d97162650a48acfeec33b3d1d084dddda6bf649ad648c2e5ff1f1ce6a6`; both were first committed at `70bc4899`, and
`684667e2` fills the stage table's goal rows' compiler-only and lazy cells and three of this family's site cells, the
classification changing only in the digest it records):
- `Broiler.ECMAScript/docs/frontend/stages.tsv` gives every catalogue row its stage under decision D10, `syntax`,
  `request`, `engine` or `semantics`, with each engine's stage and site re-read at the heads above, whether Broiler.JS
  checks a `syntax` row only in its compiler and whether lazily, and the catalogue cells that no longer hold
  (decision D28). Its only `request` row is EX0023; its `engine` rows are the engine-only Policy and Limit rows,
  EX0021 among them;
- `scripts/frontend/classify_parse_corpus.py` writes `docs/evidence/sjl-13-001/classification/`: for each corpus
  input, variant and configuration (V, this family's request; J, Broiler.JS's), the expected outcome at the syntax
  half, the rows whose shared codes may come first, and the reason (decision D26). Two runs are identical, and every
  parse-negative test262 file and every engine-origin input has a row or a reason.

**What the correspondence found:**
- **Every row is classified**, in seven classes: same; a correction of this family; a correction of Broiler.JS; a
  correction of both; implementation-defined; engine policy, a parameter of the request; engine-kept (admission,
  ceilings, rendering and each engine's own surfaces). No row's class is unknown.
- **This family's corrections that step 1 makes**: the slice tokenizer's template scans on the wide path (T6, T7, T8),
  white space inside an escape's digits (T13), strict legacy octal in object keys and class elements (T16), a
  regular-expression token after `for await (…)`, after a prefix update and after `await` used as an identifier (T24d
  to T24f), an import-attribute key that is an identifier name (P05a), a parenthesised nested assignment target (P15b),
  an assignment rest property whose target is a literal (P31), `new.target` in a class field initialiser (D32), and a
  reserved word as an import specifier without `as` (D40).
- **Corrections of this family that SJL-13 does not make**: the dynamic function's goals (P26, D61, H18), the
  top-level-`await` reading of an async arrow (P32 item 10, H19), `continue` to an outer label of a label set (D30,
  H20), and the regular-expression validator's errors (R19, H21).
- **`JsParser` emits base codes of split rows**, at sites that serve one rule and at sites that serve several (section
  D-b, findings F1 and F2), and **its parse-stage lists are cascades**, not all errors (rows D04 and D05, finding F9).
- **The binder's checks of `syntax` rows are listed member by member** (section D-d, finding F14), and Broiler.VM's
  binder still refuses `L1: L2: while (c) { continue L1; }`, which Broiler.JS accepts since #1075 (section D-e, D30).
- **The left-operand gap is wider than bundle `sjl-12-001` recorded**: four shapes, not two (row R14, finding F18).
- **Every stub of the plan's surveys and critics in the five sub-areas was reproduced or refuted by a run** or by the
  code, never carried over, and every item earlier records labelled SJL-13 has a hand-off (H1 to H17).

## The scope reading of ADR 0011's standing refusal (decision D5)

[ADR 0011](../../../../docs/adr/0011-source-level-profile-contract.md)'s standing refusals include "A shared syntax
tree or parser - Refused permanently." and "Lexing, source positions, diagnostic formatting - Waits for a second text
front end." `Broiler.ECMAScript.Frontend` is literally a shared tokenizer, parser, syntax tree and diagnostics, the
first library of the programme that is, so this record states the reading instead of leaving it implicit.
[ADR 0014](../../../../docs/adr/0014-shared-javascript-libraries.md) records the scope reading that decision SJL-D-8
(a) took, and the programme's governance row V13 and route MVP-16 carry it: the refusals concern a component shared
between Broiler.VM's profiles or placed in the core namespace. The frontend is neither. It is an engine-neutral
library outside Broiler.VM, admitted for one profile family's compiler (decision D29), so the refusals do not reach it,
and they stand unchanged for what they refuse. ADR 0011's table is not edited.

## The gate, sub-area by sub-area

### (T) The tokenizer

**X1 - met.**
- **This family**: `SliceTokenizer`, which the wide path and the slice path both construct, the wide path with
  structured templates and HTML-like comments set from the goal; `JsParser` re-reads some lexical facts (template
  cooking, escaped reserved words, strict legacy octal).
- **Broiler.JS**: `FastScanner` with `CharExtensions`, read on demand by `FastTokenStream`; some lexical rules are
  checked by the compiler's `SyntaxValidation`.
- Both read Broiler.Unicode's lexical classes and `NumericLiterals.Evaluate`.

**X2 - met.**
- **Same:** T1, T2, T3, T4, T5, T10, T11, T14, T15, T17, T20, T24g, T24h, T25, T27, T29, T32.
- **Corrections of this family:** T6, T7, T8, T13 (the wide path; the slice path keeps T13 until SJL-15.4, H23).
- **Corrections of Broiler.JS:** T12, T19, T21, T23, T24a, T24b, T24c, T24i, T26, T28.
- **Corrections of both:** T16, T24d, T24e, T24f.
- **Implementation-defined:** T9, T31.
- **Engine policy, a parameter:** T18 (the `m` suffix, the lexer extension), T22, T30 (HTML-like comments).

### (P) The parser and its tree

**X1 - met.**
- **This family**: `JsParser`, which builds the immutable records of `JsSyntax.cs`; the binder, the numeric admission
  and the lowering read that tree.
- **Broiler.JS**: `FastParser` over `FastTokenStream`, building the `Ast` package's tree, some of whose members the
  compiler sets afterwards; `FastScopeItem` declares names as the parse goes.
- **Neither engine parses the dynamic function's parameters and body under their own goals** (FormalParameters and
  FunctionBody); the contract's request carries both goals, and the library implements them for whichever engine
  routes `CreateDynamicFunction` through them (P26; H18 for this family, SJL-14 for Broiler.JS).

**X2 - met.**
- **Same:** P01, P03, P05d, P05e, P06b, P08, P16, P17, P25, P35.
- **Corrections of this family:** P05a, P15b.
- **Corrections of Broiler.JS:** P02, P05c, P07, P09, P10, P11, P12, P13, P14, P15a, P18, P21, P22, P23, P24.
- **Corrections of both:** P26 (handed on, H18), P31.
- **Engine policy, a parameter:** P06a, P19, P20.
- **Engine-kept:** P04, P05b, P27, P28, P29, P30, P32, P33, P34, P36. P32 holds every fact `JsParser` computes into
  `JsSyntax`, which the adapter rebuilds (decision D15); its item 10, the top-level-`await` reading of an async arrow,
  is a correction of this family that SJL-15's model makes (H19). P33 holds the facts `FastParser` computes that
  Broiler.JS's compiler reads, which SJL-14's bridge rebuilds.

### (N) Positions

**X1 - met.**
- **This family**: `SliceToken`'s line, column, offset and end; the spans `JsParser` gives each node and each
  diagnostic; the binder's node spans. The lowering writes them into the artifact's position table and source text.
- **Broiler.JS**: `FastToken`'s `Span` and `SpanLocation`s, carried into the `Ast` tree and into
  `FastParseException`'s message.

**X2 - met.**
- **Same:** N1, N2, N3, N4, N5, N6, N7, N8, N16, N17, N20, N21.
- **Corrections of Broiler.JS:** N11, N25, N26.
- **Implementation-defined:** N10, N12 and N24, where each engine anchors a lexical, a parse and a binder refusal
  (the library takes this family's places, decision D15; Broiler.JS's message positions change under SJL-14's class
  M); N27, positions in the dynamic function's assembled text.
- **Engine policy, a parameter:** N13.
- **Engine-kept:** N9, N14, N15, N18, N19, N22, N23.

### (D) Diagnostics and the parse-time early errors

**X1 - met.**
- **This family**: `SliceSourceDiagnostic`s with registry codes from five stages, the tokenizer, `JsParser`, the
  numeric admission, the binder and the lowering, each of which stops the unit; the binder holds the early errors
  SJL-12.6 moved out of the lowering.
- **Broiler.JS**: one `FastParseException`, or a `SyntaxError`, from its parser, or later from `SyntaxValidation`,
  `FastCompiler.EarlyErrors`, the `Visit*` methods or `DirectEvalSupport`.

**X2 - met.**
- **Same:** D12-1, D12-2, D12-5 to D12-9, D31, D43 to D45, D47, D48, D50 to D54, D56 to D60, D62; the binder's checks
  B01 to B05, B07 to B20, B22 to B24 and B26 to B34.
- **Corrections of this family:** D30 (handed on, H20), D32.
- **Corrections of Broiler.JS:** D33 to D39, D41, D42, D46, D55, D63; B06, B21.
- **Corrections of both:** D40, D49, D61 (handed on, H18).
- **Implementation-defined:** D02, D05 (the stages' order and a stage's order, fixed for the library by decision D11).
- **Engine policy, a parameter:** D03 (the cap), D04 (first-error or all-errors, decision D11), D12-10, D18-4, D18-6,
  D18-7, B25 (the regular-expression validator, decision D7).
- **Engine-kept:** D01, D06 to D09, D12-3, D12-4, D18-1 to D18-3, D18-5.

### (R) The request, switches, limits and providers

**X1 - met.**
- **This family**: the public `SliceParseOptions`, with `JsParser`'s constructor arguments and the tokenizer's
  settings beside it; eval code carries six request flags; the parse runs on `CompilationStack`'s thread.
- **Broiler.JS**: `FastParserOptions`, by value since SJL-12.5, built from `JSCompilationOptions` by four presets; the
  direct-eval context travels as call-site constants and through `JSEngine.Current`; the parse runs on
  `CompilationStack` or on the caller's stack, with `StackSegment`'s hop.
- **The limits are an adoption** (X1): Broiler.JS checks no nesting or tree depth today and survives deep input
  through stack segmentation; it takes the contract's limits by JSD-0080's decision D27, and its inputs past the bound
  become listed refusals at SJL-14 (class C3).

**X2 - met.**
- **Same:** R02, R03, R06, R07, R08, R17, R22, R23, R25 to R28, R30, R31, R33.
- **Correction of this family:** R19 (handed on, H21).
- **Corrections of Broiler.JS:** R20, R24, R36.
- **Implementation-defined:** R13 (the tree-depth bound), R14 (the left-operand gap, decision D14).
- **Engine policy, a parameter:** R04, R05 (top-level `await`, decision D10), R09 (the proposal flags, decision D17),
  R10, R12 (the nesting bound), R16, R18.
- **Engine-kept:** R01, R11, R15, R21, R29, R32, R34, R35.

### X3 - neutral contract, all five sub-areas

- **What the library takes and answers.** It takes a request of text, a goal, switches, the inherited context of eval
  code, a proposal flag per form, the lexer extension, limits and providers; it answers an immutable tree with full
  spans and raw facts, side tables ordered by source position (decision D10), and diagnostics with a shared code, a
  message identifier with arguments, a phase and a span (decision D11). No engine type, no coercion, no property
  access.
- **A refusal is data.** Every refusal is a diagnostic, in the library's order, the first one only when the request
  asks for the first error (decision D11).
- **Limits are explicit**: the nesting depth a parameter with each engine's default, the tree depth a constant of the
  library derived with the stack it requires (decision D14).
- **Providers, not dependencies**: Broiler.Unicode's `IUnicodeTables`, required (decision D8); a regular-expression
  validator interface each engine supplies (decision D7); and an optional `WorkCharge` (decision D32), passed null by
  both engines at SJL-13: the work is linear in the source with bounded constants, and this family charges a
  compilation before the compiler runs.

### X4 - no edge between the engines, none into Broiler.VM

`Broiler.ECMAScript.Frontend` references `Broiler.ECMAScript.Algorithms` (literal values) and
`Broiler.Unicode.Algorithms` (identifier classes), which SL1 already admits (decision D6), and no `Broiler.Regex`
(decision D7), neither engine and nothing in this repository. Neither engine references the other.

### X5 - boundaries hold

- **No platform regular-expression API and no culture enter the library.** The literal's validity comes through the
  provider (decision D7); the home's SL rules and this family's rule I2 hold the rest.
- **No ambient state and no mutable statics** (SL2), and determinism over runs and threads (SL9, and the parse corpus's
  determinism in the home's rule, decision D25).
- **No frontend in a compiler-free root.** The library is admitted for `Broiler.VM.Profile.JavaScript.Compiler` only,
  and rule I3 keeps it out of `JavaScript.ExecutionOnly` and `JavaScript.Android` (decision D29, SJL-13.4).
- **Native AOT compatible**: the home's Native AOT probe runs the `syntax` vectors (decision D25).

### X6 - the deletion lists, per engine (decision D20)

- **This family, at step 1**: `JsParser`; `SliceTokenizer`'s wide-only members (the structured-template mode, its
  nesting limit, `SliceTemplate`, `SliceSubstitution` and their tables); the binder's `CheckRegExp`, which leaves for
  the provider (decision D7); and every binder check of a row the stage table assigns `syntax`, member by member: the
  correspondence's B03 to B07, B10 to B15, B17 to B20, B25 to B31 and B34. **Kept**: B16 (EE0018) and B21 (EE0054),
  whose rows are `semantics`, and B33, which also refuses EE0171 (`semantics`); the library refuses EE0182 first, so
  EE0182 has no member of its own. Each `BinderAdmitted` assertion of the lowering that mirrors a deleted check stays,
  as the lowering's guard. `JsParser.DefaultBindingName` and `StarName`, which the binder and the lowering read, move
  to `JsSyntax.cs`; `SawTopLevelAwait` becomes the adapter's, by `JsParser`'s rule. `SliceTokenizer` stays for the
  slice path and the census until SJL-15.4.
- **This family, at step 2**: `JsSyntax` and the adapter; the two constants move to the lowering's module code, and
  `SawTopLevelAwait` becomes the lowering's.
- **Broiler.JS, for SJL-14**: SJL-14.5's list as decision D41 reads it (`FastParser`, `FastScanner`,
  `FastTokenStream`'s scanning code, `FastKeywordMap`'s table, `CharExtensions`' scanner members, `RegExpValidator`
  and the parser's half of `FastScope`; the two token re-scans of `SyntaxValidation` and `DirectEvalSupport`), and the
  disposition of every public type it empties (SJL-D-25), which exists only from SJL-14.1. **A dated note on this
  record at SJL-14.1 completes Broiler.JS's half member by member from SJL-14's `dispositions.md`** (SJL-13's plan's
  decision D40), so that the gate roadmap section 5 applies has its Broiler.JS half on record before SJL-14's code.

Each deleted member of this family is registered in rule I5 in the change that deletes it (bundle `sjl-13-001`'s
rule, part D).

### X7 - owners accept

The JavaScript profile owner, the Broiler.JS maintainer, the verification-boundary owner, the conformance owner and
the Broiler.Unicode maintainer accept all five sub-areas, and record decisions D2 to D39 under the owner's direction
of 2026-10-18. One person holds the roles (EX-30).

## Every row, classified

Each row is in one class, as the correspondence's section "The rows by class" puts it, and the gate's X2 above lists
them sub-area by sub-area. **What the decisions of 2026-10-26 add to the classes:**
- **Rows in one class with a second class for some inputs**, as the correspondence's notes say: P06a (engine policy
  for the call forms' positives; a correction of Broiler.JS for the negatives under the flag, decision D17); P32
  (engine-kept, but its item 10, a correction of this family that SJL-15's model makes, H19); T15, T27, P08, D12-2,
  D12-7, D31, D44, D45, D51 to D54 and D59, the same in outcome, with Broiler.JS refusing only in its compiler (stage E:
  the stage table's matter, and SJL-14's classes E and T, decision D45); D46 and D55, the same in outcome, with a
  correction of Broiler.JS on some inputs.
- **D30** stays a correction of this family that SJL-13 does not make (H20): the rows it holds are `semantics`, and no
  parser checks jump targets (finding F30). The catalogue's "both" for EE0082, EE0137, EE0166 and EE0176 is stale
  (decision D28).
- **D40** is a correction of both, read from the run: Broiler.JS accepts a reserved word as an import specifier
  without `as` too (finding F19); its Broiler.JS half is SJL-14's (decision D45's K10).
- **R24** is a correction of Broiler.JS, read from the run: `eval('super()')` in a base-class constructor is admitted at
  Broiler.JS's parse and fails when it runs (decision D44).
- **Every correction of Broiler.JS** is SJL-14's, listed in its `corrections.md` (decision D45, H22).

## The corrections step 1 makes, and how each is shown

Bundle `sjl-13-001`'s part A, kind (F), and part B, class F, name exactly these rows (decision D25, findings F19 and
F32). No test262 file reaches any of them, so part A's kind (F) is expected to list nothing.

| Row | What step 1 corrects | How it is shown |
|---|---|---|
| T6 | `#!` and `-->` right after a substitution's `${` | the `syntax` vectors (part H's C1) and the correspondence's probes |
| T7 | the template-end scan and HTML-like comments in a substitution | the same |
| T8 | the template-end scan's regular-expression-or-division guess | the same |
| T13 | ASCII white space inside `\x` and `\u` escapes, on the wide path | the same; the slice path keeps it until SJL-15.4 (H23) |
| T16 | strict refusal of legacy octal and non-octal decimal literals in object keys and class element names | the same |
| T24d, T24e, T24f | a regular-expression literal after a `for await (…)` head, after a prefix `++` or `--`, and after `await` used as an identifier | the same |
| D32 | `new.target` in a class field initialiser at function depth 0 | the same |
| P31 | an assignment rest property whose target is an object or array literal | the same |
| P15b | a parenthesised nested target that is not a simple target, in a cover grammar | the parse corpus (part B's class F), which holds inputs of it |
| P05a | an import-attribute key that is an identifier name not starting with a letter | the correspondence's probes only: only the Module goal reaches it |
| D40 | a reserved word as an import specifier written without `as` | the correspondence's probes only: only the Module goal reaches it |

**Corrections of this family that SJL-13 does not make**, each handed on and admitting nothing in either of SJL-13's
bundles: P26 and D61, the dynamic function's goals (H18); P32's item 10, the top-level-`await` reading of an async
arrow at a module's top level (H19); D30, `continue` to an outer label of a label set (H20); R19, the
regular-expression validator's errors (H21, SJL-11.4).

## The hand-offs

H1 to H17 are the items earlier bundles and records labelled SJL-13, each reassigned with its source; H18 to H24 are
what the correspondence hands to later work. Line numbers are at the heads the correspondence names.

| Hand-off | Source | What | Where it goes | Why |
|---|---|---|---|---|
| H1 | Broiler.VM `docs/evidence/sjl-1-001/README.md:55` | `07.5` read as 7.5 | closed | settled at SJL-3.4 (`src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md:94-97`); both engines refuse it through the library (T17) |
| H2 | Broiler.VM `docs/evidence/sjl-3-001/README.md:269` | the front end's escape and template scans: `SliceTokenizer` (3), `JsParser.ScanSubstitution` (1) | `ScanSubstitution`: closed (removed at SJL-12.6). The three `SliceTokenizer` reads are the `HexNumber` parses `VMT:1180`, `:1196`, `:1723`, which carry T13's defect: the wide path stops using them at step 1; the slice path keeps them until SJL-15.4 | the milestone keeps the slice tokenizer until SJL-15 (`docs/javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md:69-71`) |
| H3 | Broiler.VM `docs/evidence/sjl-5-001/README.md:248`, `:258-261` | the tokenizer's code-point read (`CodePointAt` `VMT:2984`), its escapes, the lexical `WhiteSpace` predicate | WhiteSpace: closed (the owner's since SJL-7.4, T1). CodePointAt and the escape readers: off the wide path at step 1; SJL-15.4 for the slice path | as H2 |
| H4 | Broiler.JS `docs/evidence/sjl-3-001/README.md:211-215` | U+0085 as white space in the lexer | closed | settled at SJL-7.5 (`JSS:429-431`, T1); the SharedLibraries note of 2026-10-16 already says so |
| H5 | Broiler.JS `docs/evidence/sjl-5-001/README.md:317` | the lexer's code-point reads and its identifier and escape tests (`FastScanner` 10, `CharExtensions` 3) | SJL-14 | SJL-13 does not change Broiler.JS's front end. T12 (`_` in IsDigitPart and HexValue) is one of these tests |
| H6 | Broiler.JS `docs/evidence/sjl-7-001/README.md:176` | the decimal-digit lookahead after `?.` and `.` | SJL-14 | the member is `_ReadToken`, now `JSS:679` and `:703` (the bundle's line numbers are stale); T19 |
| H7 | Broiler.JS `docs/evidence/sjl-7-001/README.md:576` | eight lines of the scanner and `CharExtensions.IsLineTerminator` | SJL-14 (the lexer); `JSUnicode.IsLineTerminator` is the owner's | T2 |
| H8 | Broiler.JS `docs/roadmap/SharedLibraries.md:335` | the tagged-template path's trimming of a raw segment | SJL-14 | T28 (K3) |
| H9 | Broiler.JS `docs/roadmap/SharedLibraries.md:1381-1390`, JSD-0081 D22 | DecodeBracedUnicodeEscapes and the parenthesis patch; the division fallback | SJL-11.5 (else SJL-14) for the first two; SJL-14 for the fallback (S9) | T26, T25, T24i |
| H10 | Broiler.VM `docs/javascript-shared-libraries/areas/numbers.md:173`, `milestones/sjl-3-number-conversion-pilot.md:286` | literal scanning, diagnostics and strict-mode checks stay in each front end until SJL-13 and SJL-14 | consistent: Broiler.VM's move with step 1, Broiler.JS's at SJL-14 | T16's corrections land with those moves |
| H11 | Broiler.VM `src/Broiler.VM.Profile.JavaScript/docs/decisions/0071-the-profile-on-the-shared-text-algorithms.md:107-108` | "`JsParser.IsWellFormed`, the compiler's early error on a lone surrogate in an export name, which the correspondence assigns to SJL-15" | **SJL-13**, not SJL-15: the library's check of EE0178 (`JSE1619`, mapped to 2101 within meaning, `frontend-mapping.tsv:290`), a `syntax` row by D10 because both parsers refuse it at parse time (`VMJP:1402-1417` through `ModuleExportName` `:1439`; Broiler.JS `FastParser.Import.cs:186-200`, called at `:126`, `FastParser.Export.cs:105`, `:244`). Broiler.VM's site goes with `JsParser` at step 1 (D20, X6, I5); Broiler.JS's at SJL-14's W2. Its row is D56. The second caller, `ModuleSpecifier` `VMJP:1468-1495` (`:1477-1483`, the export-name text copied), refuses EX0029, an `engine` row: its owner after step 1 is F6's | baseline T041637, T041712, T041720, T041722, T041724 (B1 2101, P and K refused: agree); JSD-0071's sentence is stale from step 1 (F31) |
| H12 | Broiler.VM `…/decisions/0065-number-conversion-moves-to-the-shared-algorithms-library.md:66` | literal *scanning* divergences left "until SJL-13 and SJL-14": Broiler.JS's `strict-non-octal-decimal`, `legacy-octal-fraction`, `empty-exponent`, `bigint-fraction`; Broiler.VM's `vm-legacy-octal-fraction` | **only `strict-non-octal-decimal` survives**, and it is **SJL-14's** (K1; T16): Broiler.JS `scripts/conformance/number-divergences.txt:20-25` at `d6795fc8` (vectors `numbers/literal/0082`, `0084`, `0088`, `0090`). The other three of Broiler.JS were retired at corpus revision 5 (Broiler.JS `docs/evidence/sjl-3-001/engine/retired-divergences.tsv`), Broiler.VM's at revision 6 (Broiler.JS `docs/evidence/sjl-4-001/engine/retired-divergences.tsv`, three rows): **closed** | T16, T17, H1; JSD-0065:66 is a stale statement: its dated note is SJL-14.1's |
| H13 | Broiler.VM `…/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md:272` | "Sharing the lexers' and the regular expressions' white space: SJL-13, SJL-14 and SJL-11 own them" | **T's, and the sentence holds.** The lexers already read one predicate, Broiler.Unicode's `WhiteSpace` (SJL-7.4 and SJL-7.5: `JsCompilerUnicode.cs:48-53` behind `VMT:2916`; `JSS:204-207`, T1, H3, H4). What moves is the code that reads it: Broiler.VM's wide path at step 1 (the library's tokenizer, through the request's `IUnicodeTables`, D8); the slice path's `SliceTokenizer.IsWhiteSpace` at SJL-15.4 (H3, D60); Broiler.JS's `FastScanner.IsWhiteSpace` at SJL-14's W2. The regular expressions' white space (`\s`) is SJL-11's, outside the correspondence | T1; no record stale |
| H14 | Broiler.JS `Broiler.JS/CHANGELOG.md:158-166` at `4ae1c747` = `:254-262` at `d6795fc8` (`ContainsContinue`, internal; `Ast` grants its internals to `Compiler`), and `:88-96` (#1076, `IsLoopBookkeeping`) | "which raw facts a shared tree publishes is SJL-14's to decide" | **SJL-14's bridge (W1) synthesises `ContainsContinue` only** (D44 lists it): the parser sets it (`FastParser.ForStatement.cs:256-309`, counting at `FastParser.Statement.cs:561-562`) and the for-loop pass reads it (`ForLoopPreparation.cs:263-284`). **`IsLoopBookkeeping` needs no bridge**: the pass sets it (`ForLoopPreparation.cs:860-953`) and `FastCompiler.cs:465` reads it (decision D44: "Not `IsLoopBookkeeping`"). The internals grant (`Broiler.JavaScript.Ast.csproj:37`) stays until the compiler stops reading `Ast` (SJL-15.5, D41). A tree fact, not a token: P33 item 4 | the bridge's list is decision D44's |
| H15 | Broiler.JS `Broiler.ECMAScript/docs/frontend/early-errors.md:19` and `:283` | the catalogue names "the message identifier the shared front end (SJL-13) emits"; "SJL-13 defines each identifier's arguments" | **SJL-13.2 stage 4d** ("message identifiers with arguments (`early-errors.md`'s section 6)", SJL-13.2d; decision D11). **D's** sub-area | open until stage 4d |
| H16 | Broiler.JS `Broiler.ECMAScript/docs/frontend/design.md:216-220` ("What this document does not decide") | the library's names, types and API (SJL-13); the registry revision (SJL-13); the `Ast` and `Parser` disposition (SJL-14.4); the model (SJL-15); "the nesting bound's value, which is SJL-13's measurement" | names and API: Broiler.JS's SJL-13.1 change (`design.md` revised, decisions D6 to D17) and SJL-13.2a; the registry revision: step 1's registry commit (decision D12); the disposition: SJL-14.1's `dispositions.md` and SJL-14.4 (decision D50); the model: SJL-15.2; the bound: SJL-13.2e's tree-depth derivation, the nesting depth staying each engine's parameter (decision D14; `design.md:74` reads the same). **R's** for the limits, **D's** for the registry | consistent with this record; no record stale until each lands |
| H17 | Broiler.VM `docs/javascript-shared-libraries/native-aot.md:226` (G5) | Broiler.JS's Native AOT sample "evidence at SJL-14 but in no clause" | **SJL-14's**, already a clause: `milestones/sjl-14-broiler-js-syntax.md:107-110` (clause 4, revised 2026-10-11); SJL-13's plan's decision D53 and SJL-14's rule. Not SJL-13's | closed as a record |
| H18 | the correspondence (rows P26, D61; probes PR01, PR02) | Broiler.VM's dynamic function parses its parameters and its body under the library's FormalParameters and FunctionBody goals before it compiles the assembled function (`VMP/JsRealm.Dynamic.cs:159-187`) | a later Broiler.VM change with its own row and window; recommended: SJL-15's Broiler.VM window, which re-pins the frontend | no change of SJL-13 routes `FromSource` through the library's goals (F32) |
| H19 | the correspondence (row P32, item 10; probe PR06) | Broiler.VM counts an `await` inside an async arrow at a module's top level as a top-level `await`, which changes evaluation order | SJL-15's model (stage e, module records and top-level `await`); the adapter keeps Broiler.VM's rule at both steps of SJL-13 | part B admits no byte difference of an accepted input at step 1 or step 2 (F12) |
| H20 | the correspondence (row D30; probes PD01, RD01) | Broiler.VM's binder refuses `continue` naming an outer label of a loop's label set (`JBW:653-666`, `:1455-1477`), and `SC/BinderChecks.cs:127-130` pins it | a Broiler.VM correction of the binder's `Target`, with the checks row, outside SJL-13's library (the rows are `semantics`); or SJL-15's jump-target fact | the catalogue's "both" for EE0082, EE0137, EE0166 and EE0176 is stale (F30) |
| H21 | the correspondence (rows R19, R20) | the two engines' regular-expression validators decide some patterns wrongly | Broiler.VM's at SJL-11.4 (Broiler.Regex's validate entry behind the provider, JSD-0081 D18); Broiler.JS's at SJL-11.5, or K9 at SJL-14 (D45) | consistent with JSD-0081; the library only calls the provider (D7) |
| H22 | the correspondence (the column "Correction of Broiler.JS" in every sub-area) | every correction of Broiler.JS this file classes | SJL-14's `corrections.md` (D45's K1 to K10), with the specific actions F23 to F28 name: the derived-constructor flag (R24), the direct-eval prefix and its skip sites (N25, N26), the two token re-scans (R36, T16), each entry point's request (R35), the bridged token facts (N18), and the tests that assert acceptance past the limits (R12, R13) | SJL-13 changes no Broiler.JS front end; the library corrects these when Broiler.JS parses with it (F28) |
| H23 | the correspondence (rows T13, T12's .NET hexadecimal parses on the slice path; H2, H3) | the slice path's tokenizer defects (white space and `_` inside an escape's hexadecimal digits) | SJL-15.4, which removes `SliceTokenizer` (D60) | the slice path keeps `SliceTokenizer` until SJL-15 (F17 on part F) |
| H24 | the correspondence (row R35, J1k) | Broiler.JS's opt-in tiering recompile parses a promoted function's text under the Script goal whatever goal the function was compiled under, and gets strictness by a `'use strict';` prefix | SJL-14's W1 (the bridge passes the function's goal and forced strictness at that site), or Broiler.JS's own backlog | behind a switch that is off by default; no probe reaches it (F26) |

## The owner's decisions of 2026-10-26

Each decision below was taken on 2026-10-26 by the owner, under the direction quoted above, but decision D1, which is
a reading of the owner's words.
- **D2 to D39** are SJL-13's plan's recommendations, each with its reason. Each takes the plan's text, with its task
  numbers given as the steps and records they name, and with no figure (validation principle 5).
- **D7, D10 to D18, D20, D21, D23, D25, D26, D28 and D39** were decided again the same day, on the findings named in
  each, and are recorded as revised. Each quotes the plan's text it replaces.
- **D41 and D44 to D47** bind SJL-14. They were decided again the same day on the findings named in each, and are
  recorded here as this record's answers to those findings; Broiler.JS's SJL-14 section records them, with D40 to
  D56, at SJL-14.1.

The decisions that bind Broiler.JS, in the home or in its engine, are D6 to D17, D25, D26 and D28, and D41 and D44 to
D47 for SJL-14. Broiler.JS's area entry records its half of D6 to D17, D25, D26 and D28, by these numbers.

### The start, numbers and records

**Decision D1 - the start, a reading of the owner's words.** **Recorded as a reading, not as a decision taken under
the direction**: the direction takes recommended decisions, and it does not choose to start a milestone. SJL-13's plan
had the owner asked, when SJL-13.1 began, for words choosing SJL-13, and had SJL-13.0 quote words given before
SJL-13.1's records merge; without them it would have quoted the words of 2026-10-19. On 2026-10-25 the owner wrote
"Continue with the remaining SJL milestones". **SJL-13.0 quotes those words and reads them as the choice to start
SJL-13.** It says:
- that this reading extends to SJL-13 the ledger's two earlier readings of the owner's words of 2026-10-19, "Also
  continue with the other remaining SJL milestones" (its SJL-11.0 and SJL-9.0 entries), which named SJL-11, SJL-9 and
  SJL-17 and excluded nothing, and that SJL-8.0's reading of general words was narrower ("the next milestone in the
  delivery order");
- that SJL-13 may not meet its gate before SJL-12, SJL-3 and SJL-7 meet theirs (update rule 10);
- that SJL-14 and SJL-15 file their own start entries, reading the same words the same way, and that SJL-16 is not
  started.

The reason: JSD-0081's decision D2 and JSD-0082's decision D1, one start entry per milestone; roadmap section 3
permits a start before the predecessors are accepted, and every such start has rested on the owner's dated words.
The plan's text read: "Without them, **SJL-13.0 quotes the words of 2026-10-19, "Also continue with the other
remaining SJL milestones", and reads them as the choice to start SJL-13**". The owner's fresher words were given
before SJL-13.1's records merged, so they are quoted instead, as the plan provided.

**Decision D2 - a dated note under roadmap section 4.** **Yes.** SJL-13's note, which also records late the starts of
SJL-11 (2026-10-25) and SJL-9 (2026-10-26), as the note of 2026-10-15 recorded SJL-6.1 to SJL-6.4 late. SJL-14's and
SJL-15's starts file their own. The reason: section 4's order is where a reader looks, and the starts of SJL-11 and
SJL-9 filed no note there; SJL-13 is on the critical path. The plan's text recorded those two starts as made "by the
same words"; the ledger's SJL-11.0 and SJL-9.0 entries quote the owner's words of 2026-10-19 for them, and SJL-13
starts at those of 2026-10-25, so the note quotes each start's words as the ledger records them.

**Decision D3 - numbers, names and people.** This record is the verdict, at the next free number, JSD-0083; **a second
record, the cut-over's, at step 1**, recording registry revision 19 (governance P15) and the admission, with a dated
note at step 2. A JSC entry at SJL-13.1, JSC-320, for JS-2's stale "Next action"; JSC entries at SJL-13.5 for JS-2's
closure; an exclusion at step 1 (decision D31). Bundles `sjl-13-001` and `sjl-13-002` in this repository and
`sjl-13-001` in the home. The owner, the co-signers and the EX-30 sentence are as the header states. The reason:
JSD-0081's decision D28 and JSD-0082's decision D2, every area files a verdict and a cut-over record; numbers are the
single writer's, taken in merge order at filing.

**Decision D4 - how the extraction gate is applied.** **Five sub-areas**: (T) the tokenizer; (P) the parser and the
tree; (N) positions; (D) diagnostics and the parse-time early errors; (R) the request, switches, limits and
providers. X1 to X7 per sub-area. They are one library, so a refusal of any is a refusal of SJL-13. The reason: the
precedent of SJL-5's to SJL-10's per-sub-area verdicts; JSD-0080's decision D1 left the frontend's verdict to
SJL-13.1.

**Decision D5 - ADR 0011's "A shared syntax tree or parser - Refused permanently".** **Cited and read** in this
record (section "The scope reading"): ADR 0014's scope reading, governance row V13, confines the refusal to components
shared between Broiler.VM's profiles or placed in the core, and SJL-D-8 records the route. The frontend is neither.
The reason: this is the first library that is literally a shared parser, and the verdict must not leave the reading
implicit.

**Decision D39 - stale statements corrected at SJL-13.1** *(revised 2026-10-26, finding F31)*. The records this change
corrects, each by a dated note quoting what it replaces: roadmap sections 4, 5, 10 and 11; `areas/frontend.md` section
3; `architecture.md` sections 2 and 3.1; governance section 8; the programme README's status and its line on
Broiler.JS's `FrontEnd/Semantics` boundary; JSD-0081's decision D18; **JSD-0071's "What stays"**
(`JsParser.IsWellFormed`, EE0178's check, moves to the library at step 1 with EE0178's code, not to SJL-15);
**SJL-13's milestone's "Does not do"** ("It does not share the static semantics", read with decisions D10 and D20: the
parse-time early errors and their declarations walk are SJL-13's, and the binder keeps only its `semantics` checks;
added after review); JS-2's "Next action" (JSC-320); and, in Broiler.JS's change, its status row and `design.md`. The
reason: update rule 1; each would mislead SJL-13's executors. The plan's text ended "`README.md:3`, `:191`; JSD-0081
D18; JS-2's "Next action" (JSC); Broiler.JS's status row and `design.md`." JSD-0071 assigns `JsParser.IsWellFormed` to
SJL-15, but step 1 deletes `JsParser` and EE0178 is a `syntax` row both parsers refuse (hand-off H11).

### The library

**Decision D6 - the library's shape.** Project and package **`Broiler.ECMAScript.Frontend`** (SJL-D-2 (a)), in
`Broiler.ECMAScript/Broiler.ECMAScript.Frontend/` with `.Frontend.Tests/`; namespaces `Broiler.ECMAScript.Syntax`
(the request, the tokenizer, the tree, the parser, the diagnostics) and `Broiler.ECMAScript.Semantics` (the
declarations walk now, the model at SJL-15); SL1's row `[Algorithms, Broiler.Unicode.Algorithms]` and **no
`Broiler.Regex` edge**; `PublicAPI.txt`; assurance; `IsPackable` false until the done-when. The reason: the home's
`design.md` sketches these namespaces; SL1 already admits the `Broiler.Unicode.` prefix; one package per boundary.

**Decision D7 - the regular-expression seam** *(revised 2026-10-26, finding F3)*. **A validator interface in the
request's providers, with no package edge.** Each engine supplies it.
- This family wraps the validator its binder's `CheckRegExp` calls at the base (the flag checks and
  `JsRegExpMatcher`, or Broiler.Regex's validate entry if SJL-11.4 has landed), keeping the code (renumbered by
  revision 19) and the literal token's span. **`CheckRegExp` leaves the binder at step 1.**
- **The provider's answer** is valid, or invalid with the failing condition's message identifier where the validator
  can tell it and EE0019's `regexp.literal.invalid` where it cannot, with the validator's message as its argument.
  This family's wrapper of `Format.JsRegExpMatcher` gives the latter, and revision 19 maps every regular-expression
  condition to one registry code (decision D12), so the condition named changes no code this family renders.
- Broiler.JS supplies its validator at SJL-14 (decision D51).
- Dated notes on `architecture.md` sections 2 and 3.1 and on JSD-0081's decision D18 and X5. The package edge is
  decision D65's.

The reason: this family admits no `Broiler.Regex` and Broiler.JS moves its pin only at SJL-11.5 (JSD-0081's decision
D12), so an edge now would admit an unadmitted package into the compiler's closure or move a pin early; the milestone
sends literals "to the validation provider" either way. The plan's text read: "**`CheckRegExp` leaves the binder at
step 1**. Broiler.JS supplies its validator at SJL-14 (D51)." It did not say what the provider answers, and the
matcher tells pattern rules apart only by message text (finding F3).

**Decision D8 - the Unicode provider** (JSD-0080's decision D20). The request takes Broiler.Unicode's
**`IUnicodeTables`**, required, with no default. This family passes `JsCompilerUnicode`'s Format tables; Broiler.JS
passes its own tables (their home is decision D50's). The reason: "no default that throws"; rules I4 and N22 are
untouched here.

**Decision D9 - literal values and `Algorithms`.** The library calls `NumericLiterals.Evaluate` as both engines do,
with unbounded length; this family applies its BigInt ceiling at admission. **No change to
`Broiler.ECMAScript.Algorithms` at SJL-13, an added member included**: a helper the library needs is internal to the
frontend. If another library later needs it, it moves into `Algorithms` inside SJL-14's cut-over merge, the first
Broiler.JS cut-over merge that can carry it, and this family adopts it at SJL-15's re-pin, listed in SJL-15's cut-over
record. The reason: Broiler.JS references `Algorithms` by project from its product projects, so any `Algorithms`
merge is a Broiler.JS product change, and SJL-13 has no Broiler.JS cut-over to carry one; the publication check
before step 1 (decision D30) then finds `Algorithms` unchanged.

**Decision D10 - which early errors the syntax half reports** *(revised 2026-10-26, findings F7, F8, F13 and F15)*. A
choice between two alternatives, and the owner takes **(a), the union**:
- **(a) the union**: the grammar, and every early error either engine's parser reports at parse time today
  (`JsParser`'s `ValidateDeclaredNames` family; `FastScopeItem.AddVariable` and the parser's other checks), through a
  declarations walk in `Broiler.ECMAScript.Semantics` that also exposes, **as side tables ordered by source position,
  each scope's kind and declarations (each block-level function declaration with whether it is an `if` clause's),
  each function's parameter names, and each program's and function's strictness**, which the library computes for
  its own early errors, so that neither engine recomputes them (decisions D15 and D44); and the regular-expression
  literal's validity (decision D7).
- **(b)**, not taken: this family's parse-time set only.

The engine-only rows are sorted by the catalogue's own kinds. Grammar rows and the contract's two limits (nesting, tree
depth) are the library's. A Policy row governed by a switch the contract's request already has is a `request` row the
library applies (EX0023, top-level `await`): **with the switch off, the library reads the top level as it reads a
non-async function's body of the same goal and reports what it reports there, so EX0023 has no diagnostic of its
own**, and the mapping's EX0023 row says so from the registry commit (decision D12). **The switch applies to whichever
goal the request sets it for**: this family sets it for the Module goal only; Broiler.JS also for a Script whose top
level may await (its `ScriptWithTopLevelAwait` preset, `JSContext.EvalWithTopLevelAwaitAsync` and the shell's retry).
Every other Policy or Limit row stays with its engine (`engine` rows: manifest admission, format ceilings, the eval
scope depth, module keys, requests, and EX0021's import attribute the host cannot honour, which this family's lowering
keeps as module-load policy and Broiler.JS's bridge takes from its parser at SJL-14), and the library never reports it.
Everything else stays with each engine until SJL-15 (this family's binder; Broiler.JS's compiler stages). **The stage
table records the assignment row by row**: EE0017 is `syntax` by the grammar clause though no parser checks it today,
EE0179 is `syntax` because EE0168, a `syntax` row, refuses every source it refuses, and EE0018 and EE0054, which no
parser checks, are `semantics`. Dated notes on SJL-13.2 and SJL-15.2.

The reason: (b) leaves Broiler.JS's parse-time checks without a producer from SJL-14.5's deletion of `FastScope` to
SJL-15.5, or keeps them in the bridge; (a) gives every parse-time check one shared producer, which is the programme's
point, and its walk is the seed of the model's "Declarations" fact. A Policy row made an unconditional library
refusal would make engine policy shared; JSD-0080's "What each engine keeps" keeps manifest admission and format
ceilings here. (a) also strands binder checks of this family, which decision D20 deletes, and moves parse-entry stages
in Broiler.JS, which SJL-14's classes E and T admit (decisions D45 and D64). The plan's text read:
- "A Policy row governed by a switch the contract's request already has is a `request` row the library applies
  (EX0023, top-level `await`)." The switch's diagnostic and its goals were unsaid, and the slice compiler's checks row
  holds today's codes and texts for the three constructs (findings F7 and F8);
- "through a declarations walk in `Broiler.ECMAScript.Semantics` that also exposes declarations per scope, in source
  order, as a side table;" which names no strictness and no other side fact the adapter and the bridge need (finding
  F13);
- "The stage table records the assignment row by row." The four rows no parser checks needed their stages said
  (finding F15).

**Decision D11 - diagnostics and their text** *(revised 2026-10-26, findings F7 and F9)*. Shared codes, `JSE` and
four digits, from the catalogue; message identifiers with arguments, defined at SJL-13.2d (`early-errors.md` section
6); phases; source order; the cap a parameter (this family passes the cap its wide compiler applies today, by its
member); **first-error or all-errors, a parameter, both engines asking first-error at SJL-13 and SJL-14**:
- the library stops at its first refusal, in its order: a refusal as the parse reaches its construct, and a scope's
  declared-name conflicts when the scope closes, in the source order of the conflicting declarations;
- so this family's parse-stage list, which `JsParser` made of its first refusal and the cascade of the statement in
  progress, is one diagnostic from step 1 (bundle `sjl-13-001`'s part B, classes S7 and S8);
- all-errors stays a parameter for an engine that asks for it later (SJL-15's model).

**This family renders every message text it renders today for a code whose meaning stays**, the 2104 suffix " is not
admitted by the declared feature manifest" included (`Test262Families`, `SourceFrontEndChecks`). **The
top-level-`await` row of `SourceFrontEndChecks` holds unchanged**: with the switch off, `await`, `for await` and
`await using` at a module's top level are refused with the code and text of the same construct in a non-async
function (decision D10).

The reason: SJL-D-11; messages are not contract, but this family's tallies and checks read them. `JsParser`'s list
after its first refusal is its recovery's (it ends the statement in progress, skips the scope checks and reports one
conflict per scope), not a contract, and every reader grades the first diagnostic; reproducing that recovery would put
one engine's artefact in a shared library, and an all-errors list with the library's own recovery would need a class
admitting any tail. Checking a scope's conflicts when it closes keeps this family's first diagnostic wherever
`JsParser`'s is a refusal the library makes; Broiler.JS's first refusal moves on a source with a grammar error later
in the same scope, which SJL-14's corrections name (decision D45). The plan's text read: "first-error or all-errors
(Broiler.VM all, Broiler.JS first)." (rows D04 and D05 show this family's parser is neither, finding F9); its reason
read "SJL-D-11. Messages are not contract, but Broiler.VM's tallies and checks read them."; and its sentence on texts
ended at "(`Test262Families`, `SourceFrontEndChecks`)." (finding F7).

**Decision D14 - the limits** *(revised 2026-10-26, finding F18)*. **The tree-depth constant is derived at SJL-13.2e
as JSD-0022 derived this family's**, with the stack it requires stated beside it; **the derivation must admit this
family's bound on the stack `CompilationStack` declares**; if it cannot, the library is reworked (explicit stacks),
not the bound lowered. `SliceParseOptions.MaximumTreeDepth` stays, equal to the library's (a test holds the equality),
so rule N19's text checks stand. The nesting depth stays a parameter with each engine's default (this family's
`DefaultMaximumNestingDepth` and `MaximumSupportedNestingDepth`). **The left-operand gap is closed in the library in
all four shapes the base compiles past the bound**: a binary chain whose first operand is a parenthesised chain, a
template whose substitution holds one, a binary chain whose first operand is a member or call chain, and a member or
call chain whose object is a parenthesised binary chain. **The charge**, which the home's part G reads at the bound:
tree depth is charged as `JsParser.Deepen` charges it, one level for each node a binary chain or a member or call
chain adds, and none for a parenthesis, a template or a substitution; and a left operand's spine adds its own length
to the chain it begins, so a chain of `a` nodes as the left operand of a chain of `b` nodes is charged `a + b`. Each
refusal is an implementation limit (bundle `sjl-13-001`'s part B class L and part A's C3, the home's outcome L), never
a correction (part B's class F and part A's kind (F) list none of them). The specification sets no depth limit and the
base loses no process on any of them, so what the gap costs at the base is depth accounting, not robustness. The
reason: JSD-0080's decision D27; rule N19 reads the constant's text; a lower bound would move refusals with no class;
bundle `sjl-12-001` carried the gap here, and keeping it would put a known hole in a shared library. The plan's text
read: "**The left-operand gap is closed in the library** (class L; part C's fifth row)." The correspondence's row R14
found two shapes besides the two bundle `sjl-12-001` records (finding F18).

**Decision D15 - positions and anchors** *(revised 2026-10-26, findings F10, F11 and F12)*.
- **Units.** Offsets are 0-based indices in UTF-16 code units; lines and columns are 1-based, columns in UTF-16 code
  units; every line terminator of the specification is a new line, and CR LF one; the request takes text, and a
  leading byte-order mark is the host's to drop; full spans on every node.
- **This family's adapter reproduces every anchor and offset `JsParser` gave** (a call at its member name,
  `NameSpan`, function and class source offsets, cooked and raw template text, numeric-key spellings by `NumberText`)
  **and every fact `JsParser` computes into `JsSyntax`** (the correspondence's P32: strictness, from the library's side
  fact, decision D10; function kinds, names and keys; parenthesisation; the chain wrappers; the desugarings and
  synthetic names; the top-level-`await` fact), each by `JsParser`'s rule, the adapter's at step 1 and the lowering's
  at step 2. `JsParser`'s top-level-`await` rule counts an `await` inside an async arrow at a module's top level,
  which the specification does not: this family keeps it at both steps, the library's tree carries the
  specification's fact (SJL-13.2c), and the correction is SJL-15's (hand-off H19).
- **Diagnostic places.** For each refusal the library takes from this family's front end, its diagnostic's span
  begins where that front end anchors the diagnostic at the base: a lexical refusal at the construct's start; a parse
  refusal at the token or the production's start the `JsParser` site names; a check taken from the binder at the node
  the binder names. Bundle `sjl-13-001`'s `correspondence-runs/analysis/jsparser-anchors.tsv` and
  `binder-anchors.tsv` list the sites, and this family renders the span's start as the line and column. The binder's
  report at no place (`Declares`) takes the library's place (part B's class R2). **No position class.** Broiler.JS's
  message positions change under SJL-14's class M (decision D46).

The reason: the lowering reads each anchor into the artifact, so byte identity depends on them, and full spans can
carry all of them. Part B compares each refused input's `code:line:column` list and admits no position class, so the
places are fixed before any run; a span is the contract's (JSD-0080's output), and fixing its start once serves both
engines. The top-level-`await` flag is in the artifact, and part B admits no byte difference of an accepted input at
either step. The plan's text read: "Offsets, lines and columns in UTF-16 code units, 1-based, every line terminator
of the specification a new line and CRLF one;" (both engines' offsets are 0-based, row N1, finding F11) and ended
"numeric-key spellings by `NumberText`). No position class." (it fixed no diagnostic place, finding F10, and no
computed fact, finding F12); its reason read "The lowering reads each anchor into the artifact, so byte identity
depends on them; full spans can carry all of them."

**Decision D17 - proposal flags, the lexer extension, the switches** *(revised 2026-10-26, finding F16)*. Proposal
flags off by default. This family admits none (the decorator is JSE7201, mapped to 2104 within meaning), and reports
it where the parse reaches it. **One flag per form for the import phases**, one per catalogue row (the declaration
forms `import source x from` and `import defer * as ns from`, EP0011 and EP0013; the call forms `import.source(…)` and
`import.defer(…)`, EP0012 and EP0014), refining JSD-0080's single "import phases" parameter. Broiler.JS admits what
the correspondence finds it parses today: decorators, `accessor` and the two call forms, not the declaration forms
(rows R09 and P06b); configuration J sets those flags and no other. The `m` suffix only through the extension
parameter (this family passes none: `1m` stays 2003). HTML-like comments by goal; Annex B on for scripts. The reason:
JSD-0080's decision D41 and SJL-D-17. The plan's text read "Broiler.JS admits what task 1.1 finds it parses today."
With one import-phase flag on, the library would accept every declaration-form input configuration J expects refused
(finding F16).

**Decision D32 - metering and cancellation.** **An optional `WorkCharge`** in the request (SJL-D-10's shape: nullable,
charged before the work, units and block size documented), passed null by both engines at SJL-13; no cancellation
poll, the work being linear in the source with bounded constants. The reason: governance section 8, obligation 1;
SJL-D-10 already settles the meter's shape; this family charges a compilation before the compiler runs.

### The registry

**Decision D12 - the registry revision** *(revised 2026-10-26, findings F1, F2, F3, F4, F5 and F7)*. **Revision 19, at
step 1's registry commit**: one new code per meaning every `split` row leaves (the three language rules coded 2104,
the rows the mapping found, and the non-language splits EX0013, EX0020, EX0024, EX0028, EX0030 and EE0021).
- **A meaning is the kind of fact a registry row's name and declaration state**, as the mapping's header reads a
  meaning, **and not a catalogue condition**. The mapping's revision groups the split conditions so that a binder site
  that serves several of them keeps one constant: every misplaced `super`, at the binder's property and call sites;
  every regular-expression pattern and flags condition, the validator's nesting bound included, at `CheckRegExp`'s
  sites; every private name no class declares, the dynamic function's included, at the binder's private-name site.
  The shared codes keep the catalogue's conditions apart.
- No `split` relation remains. The mapping names the catalogue's and the stage table's digests and revision 19,
  restates `sites-read-at` as the base commit, and cites, for each split row, the site that refuses it at the base
  (the binder's, where the lowering now holds a `BinderAdmitted` assertion). EX0023 has no emitter (decision D10): the
  mapping's revision keeps its code and withdraws its split other code, the `for await` refusal at a module's top
  level with the switch off being EX0008's, within meaning.
- Each new code has an enum member, a registry row with stage and reachability, a retained-corpus case (N7), the
  producer's mirror (N8) and a `LanguageErrors` class (EarlyError for language rules). **`Test262Run` grades each new
  code as it graded the code it left, except the three language rules coded 2104 (`await`, `using`, a logical
  assignment's target), which it grades as language early errors.** Renumbered binder and lowering emissions keep
  their stage.
- **The registry commit also changes the code at each `JsParser` or binder site that serves one rule alone and whose
  code revision 19 changes**, message texts unchanged, so that every renumbering at such a site shows at the registry
  commit (part B's class R): the code argument at the relation-split rows' sites (`Attributes`' repeated
  import-attribute key, `ParseNewTarget`'s refusal, `BigIntLiteral`'s and `BigIntKey`'s width refusals) and at the
  split other codes' sites (`ValidateAccessorParameters`' setter arity, and the private-name refusals of
  `ParseFunctionRest` and `BindingName`, each EX0008's alone); and the binder's constant for a split other code at a
  site that serves one rule (the `delete` of a private reference, EE0030's and EE0031's; a destructuring declaration
  without an initialiser, EX0009's), each to the code the mapping's revision gives that rule (the correspondence's
  section D-b lists the sites). **It also changes the codes and texts the slice compiler's `--checks` rows pin where
  revision 19 changes them** (`BinderChecks.cs`, `BigIntChecks.cs`, `SourceFrontEndChecks.cs`), so that the lane
  passes on the branch.
- **The other codes the mapping marks `split` at sites that serve several rules** (`JsParser`'s
  `ValidateDeclaredNames`, `Expect`, `BindingName`, `ParsePrimary`, `ParseObjectEntry`, `ParseObjectBindingPattern`
  and `TryParseArrow` sites, for EE0087, EE0155, EE0168 and EX0008; and the binder's `Declares` check, which reports
  EE0168 and EE0169 at no place)
  are consolidated by the library's emission at the after commit, the binder's place with them; part B's class R2
  names them. **At an arrow head whose cover does not cover its parameters, the library reports EE0087 (EE0155 for an
  async arrow head)**: it parses the head as the cover and refuses the conversion with the cover's rule, where
  `JsParser`'s generic sites reported EX0009's or EX0008's codes; a head that is no cover expression at all is refused
  by the grammar's own row.

The reason: JSD-0080's decision D13 ("SJL-13 mints the new numbers"); every split row says so; JSD-0009's rule, which
numbers meanings: the mapping already reads a meaning as a kind of fact and puts several conditions under one row (a
parameter bound twice and a lexical name declared twice are both 2201), and a constant change keeps the registry
commit a renumbering, where a context test would change behaviour and the matcher tells pattern rules apart only by
message text. Grading a format ceiling as it was graded keeps verdicts still; the three language rules are the regrade
the mapping exists for. A registry commit on the base attributes every renumbering before the parse changes, which
needs the parser's and the binder's rule-specific sites too, because they emit the base code of some split rows; a
site that serves several rules cannot be changed by a constant.

The plan's text read:
- "the non-language splits EX0013, EX0020, EX0024, EX0028, EX0030 and EE0021); no `split` relation remains; the
  mapping names the catalogue's and the stage table's digests and revision 19." (finding F3, what a meaning is; F4, the
  mapping's stale sites);
- "**The registry commit also changes the code argument at each `JsParser` site that serves one relation-split row
  alone** (the repeated import-attribute key, `ParseNewTarget`'s refusal, the BigInt width refusals; task 1.1 lists
  them), message texts unchanged, so every renumbering shows at the registry commit." (findings F1 and F2: three more
  `JsParser` sites and two binder sites serve one rule; F5: the checks lane pins the codes);
- "The other codes the mapping marks `split` at `JsParser` sites that serve several rules (EE0087, EE0155, EE0168,
  EX0008, EX0023 at the plan's heads) are consolidated by the library's emission at the after commit; part B's class
  R2 names them." (findings F1, F2 and F7: `ParseFor`'s site leaves the list, `Declares` joins it);
- and its reason: "JSD-0080 D13 ("SJL-13 mints the new numbers"); every split row says so; JSD-0009's rule."

**Decision D13 - test262 at step 1, and the floors** *(revised 2026-10-26, finding F20)*. Bundle `sjl-13-001`'s part
A names its C1 kinds before any run, (R) the regrade, (G) the decorator's stage, (B) the admission walk and (F) a
correction by row, and C3 (L) for the left-operand charge; no C2, no C5. **No floor is re-based unless a classified
move lowers it**; then by hand, in the after commit, with the class named in its header, as `test262-wide.floor`
records its earlier re-base. The retained suite's floor moves only through new codes' classes, which decision D12
fixes. **The native floor's `atMost unsupported` row**, which still holds the BigInt row the bytecode floor retired by
hand on 2026-09-21, is re-set by hand to the bytecode floor's row in step 1's merge, before the after runs, with its
reason, as the bytecode floor's was, so that the native form holds "no unclassified move" as the bytecode form does.
The native run at SJL-12's after commit reports no unsupported variant, so the re-set holds at the base. The reason:
the moves are foreseeable from the mapping and JSD-0080's decisions D13 and D18, and must be classified before the
runs; the hosted workflow holds each form to its floor. The plan's text ended "The retained suite's floor moves only
through new codes' classes, which D12 fixes." The native floor could not see the move it exists for (finding F20).

### This family's request and cut-over

**Decision D16 - this family's request mapping** *(revised 2026-10-26, finding F21)*. `SliceParseOptions` stays this
family's public options (N10, I6): the compiler maps it internally onto the request. The six eval flags map to forced
strictness and the function context, and the rest reach the request as the inherited context; top-level `await`
reaches it from the module options, as today. **The visible private names are not passed at SJL-13**: the request's
private-name set is absent, which leaves an eval program's undeclared private name to the executor's run-time check.
**The caller's lexical names**, which JSD-0080's request lists for Broiler.JS's direct `eval`, are absent from the
request at SJL-13 and SJL-14: the conflict they serve is EvalDeclarationInstantiation's, a run-time SyntaxError each
engine raises today (row R28), until SJL-15's model; the home's rule lists the field as absent with this reason. The
reason: no public shared type in this family (I6). Eval's private names are checked by the executor at run time today,
so passing them would move a run-time check to parse time, a behaviour change the model may take at SJL-15 as a
correction. The plan's text ended "which leaves an eval program's undeclared private name to the executor's run-time
check." It said nothing of the caller's lexical names (finding F21).

**Decision D18 - this family's admission steps** *(revised 2026-10-26, finding F6)*. `JsParser` makes the BigInt
refusals during the parse, in source order, and the wide compiler stops on any parser diagnostic before
`JsNumericAdmission` and the binder run, so moving them after the parse changes their order. **From step 1 an
admission walk makes them over the shared tree**, after a parse that reported nothing and before the numeric
admission and the binder, and its refusal stops the unit as the parse's did: the BigInt refusal under a manifest that
does not admit BigInt (2104, EX0017), the BigInt width ceiling (its new code, EX0028), and **a module specifier that is
not well-formed Unicode (EX0029, 2101 with today's text)**, an `engine` row the library never reports. **The walk runs
on every entry point that parsed with `JsParser`**: `JsCompiler.Compile`, eval code and `JsCompiler.Requests`. The
request's form refusals keep their place. Two consequences, named before any run: such a refusal is absent when the
parse refuses anywhere in the unit (bundle `sjl-13-001`'s part B, class S3), and a negative test262 variant whose first
diagnostic was such a refusal may now show a later language refusal first (part A's kind (B)). **No input of the parse
corpus reaches one of these refusals, so both are expected to list nothing**; they are named so that a move would be
classified. The reason: JSD-0080's "What each engine keeps" keeps manifest admission and format ceilings here, so the
library must not make them; keeping source order would put an engine callback inside the library. Order decides first
codes, which `Test262Run` grades, so the change is classified before any run. The plan's text read "the BigInt refusal
under a manifest that does not admit BigInt (2104, EX0017) and the BigInt width ceiling (its new code, EX0028)." and
"and a negative test262 variant whose first diagnostic was such a refusal may now show a later language refusal first
(part A's kind (B))." EX0029 is a Policy row `JsParser` refuses during the parse that nothing would refuse after step 1
(finding F6).

**Decision D19 - the two steps.** **Step 1**: the adapter builds `JsSyntax`; after the parse only the admission walk
(decision D18) and the deletion of the binder's checks of `syntax` rows (decision D20) change. **Step 2**: "`JsSyntax`'s
parse-only parts" is read as `JsSyntax` whole: the lowering, `JsNumericAdmission` and the binder read the shared tree,
the binder's facts and their order unchanged; the adapter and `JsSyntax` are deleted. A dated note on SJL-13.3. The
reason: leaving the binder on `JsSyntax` keeps the adapter, which step 2 deletes; moving the binder's walks is
mechanical, and leaves SJL-15.3 only the change of the facts' source, which keeps its comparison attributable.

**Decision D20 - the deletions at step 1** *(revised 2026-10-26, findings F14 and F15)*. `JsParser`;
`SliceTokenizer`'s wide-only members (the structured-template mode, its nesting limit, `SliceTemplate`,
`SliceSubstitution` and their tables); the binder's `CheckRegExp`; **every binder check of a row the stage table
assigns `syntax`**, which the library now refuses before the binder runs, named in X6 member by member and registered
in I5: the correspondence's B03 to B07, B10 to B15, B17 to B20, B25 to B31 and B34 (EX0032's and EX0033's `return`;
`await` outside an async unit; the private half of `delete`; an update of a non-reference; `new.target` and
`import.meta` outside their contexts; a destructuring declaration without an initialiser, and EE0044's `const` without
one; `using` outside a disposing list; EE0017's shorthand initialiser outside a pattern; the assignment-target checks;
`CheckRegExp`; EE0005's bare import specifier; a name imported twice, EE0179's with EE0168's; the module scope's
duplicate and var-lexical checks; an imported name also declared).
- The binder keeps B16 (EE0018) and B21 (EE0054), whose rows are `semantics`, and B33, which also refuses EE0171
  (`semantics`); the library refuses EE0182 first, so EE0182 has no member of its own.
- Each `BinderAdmitted` assertion in the lowering that mirrors a deleted check stays, as the lowering's guard that the
  front end admitted the construct, its remark naming the library.
- `JsParser.DefaultBindingName` and `StarName`, which the binder and the lowering read, move to `JsSyntax.cs` at step 1
  and to the lowering's module code at step 2; `SawTopLevelAwait` becomes the adapter's, by `JsParser`'s rule
  (decision D15), and the lowering's at step 2.
- The lowering's refusal of an import attribute this family cannot honour (EX0021) is an `engine` row and stays.
  `SliceTokenizer` stays for the slice path and the census until SJL-15.4.

The reason: code nothing reaches is deleted in the merge that strands it (roadmap section 6 item 11: no fallback copy,
no switch and no dead path survives it); I5 registers it. The plan's text read: "**every binder check of a row the
stage table assigns `syntax`**, which the library now refuses before the binder runs (among them EE0044's `const`
without an initialiser, EE0182's reserved local name in an export, EX0032's and EX0033's `return`, and the binder's
second checks of `parser+lowering` rows such as `new.target`). Each is in X6 and registered in I5." Its examples
missed live checks and dead copies, named EE0182, which has no member of its own, and gave no home to the members
read outside `JsParser.cs` (findings F14 and F15).

**Decision D21 - "the shared tokenizer's behaviour must match the slice path's"** *(revised 2026-10-26, finding
F17)*. **A checks row added at step 1** compares the two tokenizers' streams over every input the slice path accepts
(bundle `sjl-13-001`'s part F); no class. It compares each fact where each tokenizer keeps it, by a field map the rule
fixes: an identifier's `SliceToken.RawText` against the library token's cooked name, and the text between `Offset`
and `End` against its raw spelling; the string value for string literals only; the kind map applied to the library's
kind and escaped flag, an escaped word mapping to an identifier; the escaped flag for identifiers only; and the
library's request with HTML-like comments off, as the slice path sets them. The library's token keeps its raw facts
and does not take `SliceToken`'s conventions. The reason: the milestone names it, and the corpus is the population it
names. The plan's text read: "**A checks row added at step 1** compares the two tokenizers' streams over every input
the slice path accepts (part F); no class." Compared fact by fact as written, it would fail on escaped identifiers,
which `SliceToken` keeps by their cooked name (rows T31 and T32, finding F17).

**Decision D23 - JS-2's concurrency and deep-nesting items on the wide path** *(revised 2026-10-26, finding F18)*.
**Two `--checks` rows in the tools commit**, judged by part C at both commits, the deep-nesting row holding only sources
the base refuses; **a further row at step 1**, "a left operand's spine is charged to the chain it begins", with the four
shapes the base compiles past the bound (the two bundle `sjl-12-001` records, and the two row R14 adds: a binary chain
whose first operand is a member or call chain, and a member or call chain whose object is a parenthesised binary
chain), judged at the after commit. The reason: the rows that hold these items today compile through
`SliceSourceCompiler` only; a row that fails at the base cannot be judged at both commits. The plan's text read "with
the two cases bundle `sjl-12-001` records the base compiles, judged at the after commit." (finding F18).

**Decision D27 - the tools commit.** One merge before step 1's base, named by both rules, touching only the slice
compiler root, the architecture tests' named files, `eng/check-frontend-mapping.py`, the lane workflow and `docs/`
(never `eng/packages/`, `eng/test262-evidence.py` or the evidence workflow): the wide fuzz session (decision D22), the
two rows of decision D23, the compile-measurement harness (decision D24), sub-population B5 of `--parse-corpus`
(`JsCompiler.Requests`, which `Broiler.JSeal.Vm` and `Broiler.HtmlBridge.Scripting.Vm` call), rule N19's widened
pattern (JSD-0080's decision D16, with its witness), the mapping checker's revision-19 checks, and the correction of
`ParseCorpus.cs`'s stale remark. The reason: the base then runs every new row and sub-population, and precedes every
change the rule judges; `JsCompiler.Requests` is public and parses through the library from step 1.

**Decision D29 - admission and closures.** The frontend is admitted for `Broiler.VM.Profile.JavaScript.Compiler` only
(I1, A2); I3 gains its subject, and its register row's "UNTIL SJL-3" is corrected by a dated note; K4's rows for
SliceCompiler, Conformance, Cli and PolyglotCli; A7's graph; N37's `packages.md`; I7 in both bundles; the notices; the
pristine consumer's transcript (N37) with the missing-package control. PolyglotCli, which has no lane, is published in
three modes by the collector; the Android root stays exclusion EX-126. The reason: each of these roots reaches the
frontend, the pristine package consumer transitively; I3's register row ("THE FRONTEND CLAUSE HAS NO SUBJECT UNTIL
SJL-3") is itself wrong; PolyglotCli has no CI lane.

**Decision D31 - assurance.** The frontend's units are under the home's SL8; **an exclusion entry at step 1** records
the parser's units leaving this family's J1 to J12 coverage, as EX-125 and EX-127 to EX-132 did; the profile's release
reads the home's review state (SJL-D-5 (a)). The reason: the parser's units leave this family's tree, and every
cut-over area since SJL-3 recorded the same.

**Decision D33 - JS-2, and "one early-error stage".** **JS-2 is settled by the milestone's clauses 4 to 6 and
JSD-0063's requirements on bundles `sjl-13-001` and `sjl-13-002`**; clause 10 is met by the JSC entries and the
profile ledger's closed row, filed by SJL-13.5. **JSD-0014's decision 1** (static semantics is one stage, the only
place an early error is reported), which JSD-0080 adopted as a requirement on the shared front end "which SJL-13 and
SJL-15 meet": from step 1 to SJL-15 this family keeps two producers after the tokenizer, the library's parse-time
static semantics (part of the shared front end's parse) and the binder, as it keeps `JsParser`'s checks and the binder
today under JSD-0080's decision D10 ("The wide parser keeps its own static-semantic checks ... until SJL-13 deletes
`JsParser` and SJL-15's model takes every early error that is not purely grammatical"). SJL-13 meets the requirement's
shared half, one producer per check and no copy in the binder (decision D20); SJL-15 meets it whole, when the model is
the one stage. The reason: JSD-0063's settling condition, which clauses 4 to 6 meet, so there is no circularity; and
JSD-0080's decision D10's own reading of the requirement.

**Decision D34 - JSB-1's judge.** **A JSB-1 row in the profile's ledger at SJL-13.5**, judged on the two wide-lowering
rows bundle `sjl-13-001`'s part C runs. The reason: JSC-319's finding against JSD-0080's decision D14, carried by
bundle `sjl-12-001`.

**Decision D35 - SJL-13's windows.** **Two windows in this repository, one merge each**, bases at their first parents;
step 1's registry commit on its branch, directly on the base, read for attribution; `ParseCorpus.cs` unchanged across
each window. The reason: JSD-0082's decisions D3 and D4 and JSD-0081's decision D16; validation principle 4's reading.

**Decision D36 - the order with SJL-11.4.** **Whichever is ready first; never overlapping.** SJL-11.4's landing branch
(its admission commit first) is cut only when its cut-over is ready, which bounds its hold. If step 1 lands first,
SJL-11.4 re-points the provider's implementation; dated notes on JSD-0081's decision D18 and on the SJL-11 milestone's
work package 11.4; and **a new dated rule file, `docs/evidence/sjl-11-001/decision-rule-area-revised-<date>.md`**, filed
with step 1's records, before any of that rule's runs, quoting the old one whole and naming the provider's validator
where part B named the binder's `CheckRegExp`, as `sjl-3-001/decision-rule-revised-2026-10-20.md` did; the old file
stays unchanged, as its "Revisions" requires. The reason: SJL-11's rule here takes its bases at its admission commit
and voids them if any merge lands between the branch's creation and its merge, so the windows cannot overlap; its
"Revisions" reads "Before SJL-11.4's code lands, a later bundle shows this file unchanged."

**Decision D37 - the evidence workflows.** **No change to either repository's `test262-evidence.yml` or this family's
`eng/test262-evidence.py`** for SJL-13 to SJL-15. Per-variant first diagnostics are read by one-process local
re-runs. The reason: SJL-11's rules hold both workflows and the driver byte-identical to their rule commits until their
after runs.

**Decision D38 - UBC-3.** A dated note on UBC-3.1: from step 2 the lowering reads the shared tree; no UBC-3 lowering
merge inside an SJL-13 or SJL-15 window. The reason: governance P10; risk A7; one product merge per engine per window
(JSD-0082's decision D3). UBC-3 is `Not started` and waits on UBC-6a.

### Vectors, fuzzing, measurement, the classification and publication

**Decision D22 - fuzzing.** **A wide-path source session, `--fuzz-wide`, in the slice compiler root** (no new
project: ADR 0001's budget), added by the tools commit, its seeds committed with the rule, CI running the seeds the
lane's seed file names from the Native AOT image; **the home's fuzz target** at SJL-13.2e. Bundle `sjl-13-001`'s part G
runs the slice session too, because step 1 edits `SliceTokenizer`; bundle `sjl-13-002`'s part G runs both sessions on
SJL-13's final state. Dated notes on SJL-13.6 ("the fuzz host's source campaigns") and governance section 8. The
reason: no session reached the wide parser, and step 1 takes the wide path off `SliceTokenizer`, the only part the
slice session reached; clause 9 is judged at both steps.

**Decision D24 - measurement.** **This family's compile measurement is a rule part (template T5) in each bundle, not
a gate clause**, with its harness in the tools commit and its workloads in the rule's change; its NOT MET is recorded
with its workloads and not read as a finding that the library is slower. The reason: SJL-13's milestone has no
measurement clause, and JSD-0081's decision D19 read SJL-11's the same way; this family had no compile-time harness.

**Decision D25 - vectors, SL9 and the probe** *(revised 2026-10-26, finding F19)*. **A `syntax` vector area in schema
1, at a corpus revision before the library's first code**, with both engines' first runs classified and this family's
pin; no schema revision. `op` names the parse (`ParseScript`, or `PerformEval`), `in` holds the source for the
library-level runner, `script` the same source, or a top-level direct `eval` of it, for the engine-level runners, and
`out` is `error: SyntaxError` or the completion value. Module-goal and switch-dependent rows have no vector; the parse
corpus judges them. **The lexical and `syntax` vectors carry every correction of this family that step 1 makes and a
Script or a direct eval can carry** (rows T6, T7, T8, T13, T16, T24d to T24f, D32 and P31), each with that row in its
provenance, so that bundle `sjl-13-001`'s part H names it in its C1; the Module-goal corrections (P05a, D40) are shown
by the correspondence's probes only, and this record lists them so (section "The corrections step 1 makes"). Engine
policy rows are `impl-defined` with per-engine fields; this family's manifest refusals of normative constructs and
Broiler.JS's `m` extension are `divergence` fields; each cites that engine's records. **A revision after each engine's
cut-over retires the divergence fields that no longer hold** (after step 1; after SJL-14's cut-over), with this
family's pin move; SJL-15's `semantics` vectors follow the same route (decision D61). SL9 is not revised: it already
runs the whole vector corpus through the library-level runner, which gains a frontend adapter. The home's rule adds the
parse corpus's determinism, and the probe runs the `syntax` vectors. The reason: SJL-1.6 and the matrix's L1 cell.
Schema 1 has no goal, switch, code or span field and refuses an unknown key, and this family's runner reads `id`,
`class`, `out` and `script` only; a schema revision would need a product merge here in the conformance root before
step 1's base, for first codes the corpus parts already judge. Retirement follows the revisions after SJL-7's, SJL-8's
and SJL-10's cut-overs. The plan's text read "Module-goal and switch-dependent rows have no vector; the parse corpus
judges them." Of the corrections step 1 makes, no test262 file reaches any, and the corpus only P15b, so without
vectors no part could show them (finding F19).

**Decision D26 - the classification of the corpus** *(revised 2026-10-26, item S1)*. Built at SJL-13.1 by
`scripts/frontend/classify_parse_corpus.py` from test262 metadata, the stage table and both engines' SJL-12 after
baselines, in two configurations (V, this family's request; J, Broiler.JS's), committed with the home's rule. **The
home's corpus runner answers a regular-expression literal, in each configuration, as that configuration's engine
validator answered the input in its SJL-12 after baseline, as the classification records it** (the home holds
neither validator): it refuses a literal, naming the first row the classification names that the stage table bases on
the provider, exactly when the classification expects the input refused in that configuration with such a row among
its first rows, and admits every other literal. A `json` variant is read under the Script goal, as SJL-12's tool read
it (row P04). The reason: rules before runs, the library cannot shape its own expectations. The validators'
disagreements are the engines' (rows R19 and R20), not the library's, which only calls the provider (decision D7); a
provider answering from the classification judges the library's seam without a validator the home does not hold, and
cannot answer otherwise than the baselines did. The plan's text read "in two configurations (V, Broiler.VM's request;
J, Broiler.JS's), committed with the home rule." and its reason "Rules before runs: the library cannot shape its own
expectations." The home's rule could not be run as classified without saying what the runner's provider answers
(item S1).

**Decision D28 - the catalogue's drift** *(revised 2026-10-26, findings F4 and F30, item S3)*. **Its cells are not
edited** (`early-errors.md`'s policy). The stage table re-reads both engines' sites at the SJL-13.1 heads (SJL-12.6's
binder; #1075's label check) and the mapping names its digest. **The stage table's `stale_cells` column names the
catalogue cells that no longer hold beyond their line numbers**: `vm_wide_stage` where SJL-12.6 moved a check into the
binder or the parser refuses a half; the `status` of EE0082, EE0137, EE0166 and EE0176, whose "both" is stale for
Broiler.JS since #1075 and wrong for the specification; EX0023's `notes`; and three `js_site` paths. This record
records them as stale. The mapping's revision at the registry commit restates its own sites (decision D12). The
reason: the catalogue reads sites at fixed heads (`completeness.md`), and both engines moved since: SJL-12.6 moved this
family's lowering checks into the binder, and #1075 changed Broiler.JS's label check. The plan's text read "The stage
table re-reads both engines' sites at the SJL-13.1 heads (SJL-12.6's binder; #1075's label check) and the mapping
names its digest."

**Decision D30 - publication and the stream.** The stream version carrying the frontend is **published with the
owner's approval before step 1 merges**; a library not complete is not packable. Before asking, **for every
`Broiler.ECMAScript.*` package this family pins and will re-pin** (`Algorithms`, and `Generation`, which its
architecture tests use), the source diff between the pinned version and the new one is empty or wholly a milestone's
listed in that milestone's cut-over record; `Algorithms`' is empty (decision D9). Admitting it re-pins every
`Broiler.ECMAScript.*` package this family holds, the runtime's `Algorithms` included, in step 1's merge. **At SJL-15's
re-pin the same check covers the frontend's syntax half**: its diff since the version step 2 adopted is empty, or each
change is listed in SJL-15's cut-over record and classified by SJL-15's rule here as a correction by row. Broiler.JS
packs the frontend for SJL-14's pristine consumer from its own commit; a Broiler.JS release that carries the bridge
waits for a published version. The publication itself is the owner's own choice, not taken here. The reason: SJL-D-4
(c); JSD-0082's decision D29's shape. One version serves every package of the stream, so a re-pin carries every
package's changes into the window that adopts it.

### Decisions that bind SJL-14, revised in answer to the findings

Broiler.JS's SJL-14 section records D40 to D56 at SJL-14.1. The findings F21 to F28 and F8 and F9 bear on five of
them, and the owner's answers are these. Each states the decision as revised, and quotes the plan's text it replaces.

**Decision D41 - the compiler and the shared tree at SJL-14** *(revised 2026-10-26, finding F25)*. **At SJL-14 the
compiler parses through the bridge**, which builds Broiler.JS's `AstNode` tree from the shared tree; `PortableCompiler`
and `ModuleSourceAnalysis` read the shared tree directly. **The compiler moves to the shared tree at SJL-15.5, with
the model**; nothing in the engine reads the bridge after that, and the bridge remains only as the obsolete adapter
packages' implementation until their removal version. A work-package revision of SJL-14.3's "in stages ... the bridge
goes" and of SJL-15.5; **SJL-15's clause 10 revised at SJL-14.1, before any evidence and under update rule 5, to add
"and the compiler reads no bridged tree"**, which SJL-15's Broiler.JS rule judges by its deletion search. "The remains
of `SyntaxValidation`" (objective) is read as its compiler-stage checks, which SJL-15.5 deletes. **Its two token
re-scans, `SyntaxValidation.ContainsLegacyOctalLiteral` and `DirectEvalSupport.ContainsStrictReservedWordUsage`, are
deleted at SJL-14's cut-over**, with the direct-eval prefix (decision D44): the library checks their rows at the parse
once the request carries forced strictness, and the reserved-word re-scan refuses legal programs (row R36). The
reason: clause 1 asks that the compiler "parse with" the frontend, and every clause is met with the bridge; moving the
compiler at SJL-14 would port `SyntaxValidation`, `FastFunctionScope` and `FreeNameScan`, which SJL-15.5 deletes, and
other sessions change the compiler daily; without the clause revision, no exit clause would judge the move. The
plan's text read: ""The remains of `SyntaxValidation`" (objective) is read as its `FastTokenStream` re-scans, which
walk the bridge's token chain from SJL-14." (finding F25).

**Decision D44 - what the bridge passes and synthesises** *(revised 2026-10-26, findings F13, F21, F22, F23, F24, F26
and F27)*.
- **The inherited context, at SJL-14's bridge merge** (JSD-0080's "What each engine keeps": "The inherited context
  becomes an option of the request at SJL-14"): Broiler.JS's direct-eval support and `Function` pass strictness, the
  function, method, derived-constructor and field-initialiser flags (a static block's eval sets the function and
  method flags: PerformEval has no static-block parameter), and the dynamic function's parameter and body goals.
  **The derived-constructor flag is computed from the class's heritage** (a constructor of a class with an `extends`
  clause), not from `MemberInits`, which every class constructor has (row R24). The visible private names only where
  Broiler.JS checks them at parse time today, the set otherwise absent so that the check stays the engine's; the
  caller's lexical names absent, as at SJL-13 (decision D16).
- **When the library parses direct-eval code, the bridge passes the caller's strictness in the request**, and
  `DirectEvalSupport` drops its `"use strict";` prefix together with the two places that skip the prefixed statement
  (`TryEvaluateInertProgram`, `TryGetSingleStatement`) and the strict-reserved-word re-scan (decision D41); on the old
  path, while the switch is off, the prefix stays until the cut-over.
- **The bridge serves every Broiler.JS parse entry point** (row R35), each with the request fields it passes:
  `DirectEvalSupport.Validate` and `ValidateIndirectEval`, and direct eval's compile after them, all with the inherited
  context, so that the second parse admits what the first did; `CreateDynamicFunction`'s four compiles, with the
  parameter and body goals; `ShadowRealm.evaluate`; the module loader and `ModuleSourceAnalysis` (the Module goal,
  top-level `await` on); `PortableCompiler`; the debugger; and `RecompileForTiering`, whose goal is hand-off H24.
- The bridge synthesises a linked `FastToken` chain from the shared token stream (`Number`, `CookedText`, `Flags`,
  `CookedInvalid`, keyword facts, **the token's source, offset, length and start location**, `Start`, `End`,
  `Previous`, `Next`); `HoistingScope` and `AnnexBFunctionNames` from **the library's side tables in source order**
  (decision D10: each scope's kind, its declarations, each block-level function declaration with whether it is an `if`
  clause's, the enclosing function's parameter names), `AnnexBFunctionNames` by `RegisterAnnexBFunctionHoisting`'s
  rule; `IsSyntheticFunctionStatementBlock` and **the Annex B.3.4 block, which JSD-0080's decision D23 lists for
  SJL-14: the bridge synthesises it from the shared tree, which keeps raw facts and no rewrite**; `ContainsContinue`;
  `WasParenthesized`; `TokenTypes.Decimal` for the `m` suffix. Not `IsLoopBookkeeping` (the pass sets it). **The bridge
  also refuses an import attribute Broiler.JS cannot honour** (EX0021, an `engine` row, decision D10), as its parser
  does today, with today's exception.

The reason: without the inherited context, from the bridge's merge the library would refuse `eval("new.target")`
inside a function (EA0002) and every other eval-context row the stage table assigns `syntax`, regressions no class
admits. JSD-0080's decision D23 lists the Annex B.3.4 block "in Broiler.JS's plan for SJL-14". `FastScopeItem`'s
dictionary enumerates in insertion order, which the dumps check. The plan's text read:
- "Broiler.JS's direct-eval support and `Function` pass strictness, the function, method, derived-constructor,
  field-initialiser and static-block flags, and the dynamic function's parameter and body goals;" (finding F22: no
  input reaches a static-block flag; F23: the derived-constructor flag);
- "the set otherwise absent so that the check stays the engine's." (findings F21 and F24: the lexical names, the
  strict caller's prefix, every entry point);
- "The bridge synthesises a linked `FastToken` chain from the shared token stream (`Number`, `CookedText`, `Flags`,
  `CookedInvalid`, keyword facts, `Start`, `End`, `Previous`, `Next`);" (finding F27);
- "`HoistingScope` and `AnnexBFunctionNames` from the declarations side table in source order;" (finding F13).

**Decision D45 - Broiler.JS's corrections, and the stage classes** *(revised 2026-10-26, findings F8, F9, F23, F24,
F25, F28 and F32)*. **K1** the strict-mode `NonOctalDecimalIntegerLiteral` (`strict-non-octal-decimal`); **K2** the
digit lookahead after `?.` and `.` (`char.IsDigit`); **K3** the tagged template's raw-segment trimming; **K4** line
terminators in the lexer; **K5** an invalid regular-expression literal refused, not read as division (row S9 of
JSD-0081's correspondence); **K6** the braced-escape decoding, if SJL-11.5 has not landed; **K7** early errors the
parser missed (the catalogue's `syntax` rows Broiler.JS does not check); **K8** the grammar failures Broiler.JS admits
(`completeness.md`); **K9** the regular-expression rows EE0186 to EE0190 and EE0208, only if the provider wraps
Broiler.Regex's validate entry; **K10** rows SJL-14's correspondence adds, among them those this record's
correspondence classes: N11 (the end of input at 0,0); N25 and N26 (the direct-eval prefix's line shift and its
refusal of a hashbang, decision D44); R24 (the derived-constructor flag); R36 (the reserved-word re-scan, decision D41);
T24a, T24b and T24f (a regular-expression token taken as a binary operator, stage E); D40 (a reserved word as an import
specifier without `as`); D63 (the rows Broiler.JS checks in `StrictModeValidator`, which never enters a `case` or
`default` clause: class F on Broiler.JS's side, not class E); P26's Broiler.JS half (the dynamic function's goals); the
scope-close timing of a declared-name conflict (decision D11); and Broiler.JS's tests that assert acceptance past the
contract's limits (`DeeplyNestedSourceTests` and the corpus inputs of rows R12 and R13), which are C3 under SJL-14's
rule or revised at SJL-14. **Class E (early-error stage)**: the `syntax` rows whose catalogue `js_stage` names only
compiler sites (`SyntaxValidation`, `FastCompiler.EarlyErrors`, `Visit*`, `DirectEvalSupport`), for an input P
accepts and P or B refuses, K refusing on both sides (among them `await using` in a script's block, which
`SyntaxValidation` refuses as "Unexpected await" today); **class T (timing)** for those of them checked lazily
(decision D64); both listed by row in SJL-14's `corrections.md` before evidence and admitted in its rule's parts C and
D (E and T) and A (T). JSD-0080's decision D27's listed refusals are C3, not corrections. The reason: the hand-offs of
SJL-3's, SJL-7's and SJL-12's bundles; the milestone's expected list, completed. Decision D10 moves into the parse
every check this family's parser makes, many of which Broiler.JS makes only in its compiler, some lazily; K7 covers
only rows Broiler.JS does not check. The plan's text read "**K10** rows task 1.1 adds." (finding F28) and "for an input
P accepts and P or B refuses, K refusing on both sides;" (finding F8).

**Decision D46 - messages** *(revised 2026-10-26, findings F8 and F24)*. V8-style text rendered from identifiers;
**the texts hosts and tests read are kept** ("Unexpected await", which the shell's retry reads, and the texts
Broiler.JS's tests assert); any other change is class M, listed by a generated table, among them N25's move (a strict
caller's eval positions, one line less once the prefix goes, decision D44) and, if the dynamic function's goals are
used, N27's, with the same exception type; SJL-14's correspondence records the inherited-context probes' message
texts, positions included, for that table. "Unexpected await" stays the text of a refusal of `await`, `for await` or
`await using` at a top level whose request had the switch off (decision D10), which the shell's retry reads. The
changelog states the rendering change once. The reason: SJL-D-11; a changelog entry per message would repeat the
generated table. The plan's text read "any other change is class M, listed by a generated table; the changelog states
the rendering change once."

**Decision D47 - limits and the parse thread** *(revised 2026-10-26, finding F26)*. Broiler.JS passes the library's
supported maximum nesting. Two alternatives were weighed. **(a), taken for SJL-14: the parse runs on a thread with the
stack the library documents**, reused across parses rather than created per compile, the hop skipped when the compile
already runs on a thread the engine started with that stack; the parse uses no `StackSegment`. **The entry points that
parse off `CompilationStack` enter the parse thread**: direct and indirect eval's validation parse
(`DirectEvalSupport.Validate`), `PortableCompiler` and `ModuleSourceAnalysis` (row R35's J2, J4 and J6; the two token
re-scans, J3 and J5, are deleted at the cut-over, decision D41), so that the library's limits are checked on the stack
the tree-depth derivation (decision D14) assumes. **(b) The library's explicit stacks** (decision D14's fallback),
which need no thread but are the larger library change. JSD-0080's decision D27's listed refusals are enumerated by
differencing corpus outcomes against the SJL-12 baselines, run on the stack those baselines record. SJL-14's rule
measures the interval with the hop. The reason: JSD-0080's decision D27, and its "What each engine keeps" ("Stack
segmentation, if it keeps it" is Broiler.JS's): the library holds no engine type. (b) is taken, by a library change in
a dark merge, if SJL-14's measurement shows the hop's cost. The plan's text ended "the parse uses no `StackSegment`."
(finding F26).

## Readings recorded with the stage table

- **The admission walk's refusals reach no corpus input.** Bundle `sjl-13-001`'s part A, kind (B), and part B's S3
  for those refusals are expected to list nothing; they are named so that a move would be classified (decision D18,
  finding F6).
- **Two rows that are not `syntax` have a Broiler.JS parser site**, and a person read both and confirmed their stages:
  EX0021 (`engine`), which Broiler.JS's bridge refuses at SJL-14 as its parser does today and the library never
  reports; and EX0023 (`request`), which the library applies through the goal's await parameter, the rule Broiler.JS's
  parser site applies (decision D10, item S4). No `semantics` row's Broiler.JS site is the parser.
- **The stage table's reading covered every item the correspondence asked of it** (finding F29): the corpus joins;
  positions compared on sloppy, raw and module rows, and codes rather than positions in the classification; the
  validators' differences by configuration; EE0044's two halves; EA0005's stage (`semantics`, so its place at no place
  stays the binder's); the stages of the rows behind P08, P15b and P24; and the strict legacy-octal rows' Broiler.JS
  stage.
- **The catalogue's stale cells are named** in the stage table's `stale_cells` column and recorded here as stale, the
  label-set rows' `status` among them (decision D28, findings F4 and F30, item S3).
- **The classifier pins two of the correspondence's analysis tables by digest**: `rows-P-baseline.tsv.gz` and
  `rows-P-agree.tsv.gz`. This record's change retains bundle `sjl-13-001`'s `correspondence-runs/` byte for byte as
  assembled, every `hashes.txt` verifying, and both files' digests equal the classifier's pins (item S2).

## The correspondence's findings, and what answers each

| Finding | What it found | Answered by |
|---|---|---|
| F1 | Three more `JsParser` sites serve one rule; the library's code at an uncovered arrow head was unsaid | Decision D12, revised; the registry commit's sites; part B's class R2 |
| F2 | Two binder sites of split other codes serve one rule; `Declares` serves two, at no place | Decision D12, revised; part B's class R2 with its place |
| F3 | What "one new code per meaning" means at a binder site serving several conditions | Decisions D12 and D7, revised |
| F4 | The mapping's and the catalogue's cited sites are stale | Decisions D12 and D28, revised; the stage table's `stale_cells` |
| F5 | The checks lane pins codes and texts revision 19 and step 1 change | Decision D12, revised; the registry commit's and step 1's files |
| F6 | EX0029 has no owner after step 1; the BigInt classes are empty on the corpus | Decision D18, revised; (b) record only, section "Readings recorded with the stage table" |
| F7 | EX0023's emitter and the top-level-`await` checks row | Decisions D10, D11 and D12, revised |
| F8 | Top-level `await` is a Broiler.JS switch of the Script goal too | Decisions D10, D45 and D46, revised |
| F9 | This family's parse-stage lists are cascades, not all errors | Decision D11, revised; part B's classes S7 and S8 |
| F10 | Diagnostic anchors have no decision and no class | Decision D15, revised |
| F11 | D15's wording on offsets | Decision D15, revised |
| F12 | The adapter reproduces this family's computed facts, one of which differs from the specification | Decision D15, revised; hand-off H19 |
| F13 | The library's outputs name no strictness, and the side table only declarations | Decisions D10 and D44, revised |
| F14 | D20's deletion list, X6 and I5, member by member | Decision D20, revised; X6 |
| F15 | Four binder checks waited on the stage table | Decisions D10 and D20, revised, as the stage table decided |
| F16 | The import-phase proposal flag must be one per form | Decision D17, revised |
| F17 | Part F as written fails on escaped identifiers | Decision D21, revised; hand-off H23 |
| F18 | The left-operand gap has four shapes, and its class is an implementation limit | Decisions D14 and D23, revised |
| F19 | The corrections step 1 makes reach no test262 file; the columns' wording | Decision D25, revised; section "The corrections step 1 makes" |
| F20 | The native whole-suite floor still holds the unsupported row the bytecode floor retired | Decision D13, revised |
| F21 | The caller's lexical names | Decisions D16 and D44, revised |
| F22 | "Static-block flags" name no input of either engine | Decision D44, revised |
| F23 | Broiler.JS's derived-constructor test admits `super()` in a base-class constructor's eval | Decisions D44 and D45, revised |
| F24 | A strict caller's direct eval in Broiler.JS is positioned in a composed text | Decisions D44 and D46, revised |
| F25 | The two token re-scans should be deleted at SJL-14 | Decision D41, revised |
| F26 | The bridge and the parse thread must name every Broiler.JS entry point | Decisions D44 and D47, revised; hand-off H24 |
| F27 | The bridged token facts | Decision D44, revised |
| F28 | SJL-14's corrections list gains the rows this correspondence classes | Decision D45, revised |
| F29 | What the stage table must read and do | Record only: section "Readings recorded with the stage table" |
| F30 | This family's binder still refuses `continue` to an outer label of a label set | Decision D28, revised; hand-off H20 |
| F31 | JSD-0071's export-name check belongs to SJL-13 | Decision D39, revised; JSD-0071's dated note; hand-offs H11 to H17 carried here |
| F32 | P26's correction of this family has no step | Hand-off H18; part A's kind (F) and part B's class F never name P26 |
| S1 | The home's runner holds neither engine's validator | Decision D26, revised |
| S2 | The classifier pins two analysis tables kept in this repository's bundle | Record only: they are retained byte for byte, digests checked |
| S3 | The stage table's `stale_cells` | Record only: decision D28 |
| S4 | Two non-syntax rows with a Broiler.JS parser site | Record only: read and confirmed |

## The rules' changes from SJL-13's plan

SJL-13's plan wrote the three rules in full before the correspondence existed. Each is filed with the owner's
decisions of 2026-10-26 applied, and each change is listed here with the finding it answers, not in the rule. "The
plan's section 4.1" is bundle `sjl-13-001`'s area rule, "4.2" bundle `sjl-13-002`'s, and "4.3" the home's rule.

### Bundle `sjl-13-001`'s area rule (the plan's section 4.1)

1. **Numbers and date at filing.** "<filing date>" reads 2026-10-26 and "JSD-<next>" reads JSD-0083, with the link's
   file name `0083-the-shared-syntax-library-and-the-wide-path.md` (free at Broiler.VM `4cf9ca4`; the records step
   re-checks it at filing and changes both if another JSD has taken 0083). Answers no finding.
2. **"What step 1 changes", its heading**: the decisions named gain D11 and D15. Answers F9 and F10.
3. **"Registry revision 19"**: a meaning is the kind of fact a registry row states, not a catalogue condition, and the
   three groups that let a binder site keep one constant are named (decision D12 as revised); the registry commit
   changes the code at every `JsParser` or binder site that serves one rule alone, the split other codes' sites
   included, each named by member. Answers **F1**, **F2** and **F3**.
4. **"Consolidation at the parse"**: the sites serving several rules are named by member, the binder's `Declares`
   check among them with its place; the code the library reports at an uncovered arrow head is stated. Answers **F1**
   and **F2**. `ParseObjectBindingPattern` joins the sites after review: an arrow head reaches its two refusals with
   no cover (the correspondence's D12-5 and D12-9).
5. **"The parse"**: how the library applies the top-level-`await` switch, with no diagnostic of its own (decisions D10
   and D11 as revised). Answers **F7**. The same bullet's last sentence names the four left-operand shapes. Answers
   **F18**.
6. **A new bullet, "The first refusal"** (decision D11 as revised). Answers **F9**.
7. **"The admission walk"**: EX0029 joins the BigInt refusals; the walk runs on every entry point that parsed with
   `JsParser`; no corpus input reaches one of them. Answers **F6**.
8. **"What does not change"**: the adapter's computed facts, `JsParser`'s top-level-`await` rule among them, and the
   diagnostics' places (decision D15 as revised). Answers **F10** and **F12**.
9. **"What earlier bundles taught", part C**: the base compiles all four shapes. Answers **F18**.
10. **"The registry commit"**: the sites of D12 as revised, and the three `--checks` files whose pinned codes and
    texts revision 19 changes. Answers **F2** and **F5**.
11. **Part A, C1 kind (F)**: the rows step 1 corrects are named, from both columns of the correspondence, and the
    corrections handed on admit nothing. Answers **F19** and **F32** (P26 is never listed).
12. **Part A, C3**: the four shapes; never kind (F). Answers **F18**. Not from a finding: C1's "of three kinds"
    reads "of four kinds", the number of kinds it names, (R), (G), (B) and (F).
13. **Part A, "How it is read"**: the native floor's unsupported row (decision D13 as revised); kinds (B) and (F)
    expected to list nothing. Answers **F20**, **F6** (b) and **F19**.
14. **Part B, class R2**: confined to the sites serving several rules, named by member, the binder's `Declares`
    included with its place; the arrow head's code. Answers **F1** and **F2**. `ParseObjectBindingPattern` joins the
    sites after review, as in item 4.
15. **Part B, class S, kind S3**: EX0029's refusal beside the BigInt refusals. Answers **F6**.
16. **Part B, class S, new kinds S7 (first refusal) and S8 (scope order)**. Answers **F9**.
17. **Part B, class F**: the rows named, as part A's kind (F). Answers **F19** and **F32**.
18. **Part B, class L**: the four shapes; never class F. Answers **F18**.
19. **Part B, "no ordering, template or position class"**: the diagnostics' places are this family's at the base,
    the binder's report at no place excepted under R2. Answers **F10**.
20. **Part B, "How it is read"**: binder sites in R2; one diagnostic per refused input from the after commit. Answers
    **F2** and **F9**.
21. **Part C, population and configuration**: the fifth row holds the four shapes, and its reason cites row R14.
    Answers **F18**.
22. **Part D, MET**: I5 registers the binder's checks member by member (decision D20 as revised). Answers **F14**.
23. **Part F, configuration**: the field map and HTML-like comments off (decision D21 as revised). Answers **F17**.
24. **Part F, "How it is read"**: what `SliceToken` keeps, and T13 handed to SJL-15.4. Answers **F17**.
25. **Part H, "How it is read"**: the vectors carry the corrections step 1 makes; the Module-goal ones are shown by the
    probes (decision D25 as revised). Answers **F19**.

Unchanged: the header's milestone, owner and EX-30 sentences; "What this file is"; the other parts of "What earlier
bundles taught"; the tools, after and base commits' conditions; part A's population, configuration, completeness,
comparison, kinds (R), (G) and (B), C4, "no C2", "no C5" and MET; part B's population, configuration, completeness,
control, comparison, classes R, S1, S4, S5 and S6, and MET; part C's controls, comparison and MET;
part E; part G; part H's rule; part I; part J; "Revisions"; "What this file does not decide".

### Bundle `sjl-13-002`'s area rule (the plan's section 4.2)

1. **Numbers and date at filing.** "<filing date>" reads 2026-10-26 and "JSD-<next>" reads JSD-0083 (re-checked at
   filing, as rule 4.1's). Answers no finding.
2. **"Step 2 changes no outcome, no byte and no diagnostic"**: the lowering computes the adapter's facts by
   `JsParser`'s rules, the top-level-`await` rule included, and places diagnostics as step 1 did (decision D15 as
   revised). Answers **F12** (and F10 at step 2).
3. **Part B, population, configuration, controls and comparison, and "Step 2 changes no outcome"**: sub-population
   B5, the ordered module specifiers `JsCompiler.Requests` returns, joins B1 to B4; the comparison reads B5's
   specifiers, the control changes one, and a specifier list that differs for a B5 input accepted at both is a
   regression, with no class. Not from a finding of the correspondence: the plan gave rule 4.1's part B the
   sub-population B5 the tools commit adds (decision D27) and left rule 4.2's at B1 to B4, though step 2 rewrites
   `JsCompiler.Requests`, which walks `JsSyntax`. Found in review.

Everything else is unchanged. Part C's fifth row holds the four left-operand shapes through bundle `sjl-13-001`'s
part C, which it cites (F18); part B admits no class, so step 2 keeps step 1's single diagnostic per refused input
(F9).

### The home's rule, Broiler.JS's bundle `sjl-13-001` (the plan's section 4.3)

1. **Numbers and date at filing.** "<filing date>" reads 2026-10-26 and "JSD-<next>" reads JSD-0083 (re-checked at
   filing, as Broiler.VM's rules). Answers no finding.
2. **"The syntax half"**: how the library applies the `request` row, with no diagnostic of its own (decision D10 as
   revised). Answers **F7**.
3. **"The two configurations"**: V never sets top-level `await` for the Script goal; J sets the proposal flags one per
   form, the two call forms on and the two declaration forms off (decision D17 as revised), and top-level `await` per
   goal as Broiler.JS's options set it (decision D10 as revised); the provider in both answers as part A says.
   Answers **F8** and **F16**, and the stage step's first item (the runner's provider).
4. **Part A, configuration**: the runner's regular-expression provider in each configuration answers from the
   classification (decision D26 as revised); a `json` variant is read under the Script goal. Answers the stage step's
   first item; the `json` reading is the classification README's, not a finding.
5. **Part G, population**: the four left-operand shapes (decision D14 as revised). Answers **F18**.
6. **Part H, MET**: the caller's lexical names are listed as absent with decision D16's reason (decision D16 as
   revised). Answers **F21**.
7. **Part A, configuration and admitted outcome L, and part G, MET**: a refusal at either limit reports its depth;
   L admits a refusal with the contract's nesting diagnostic (EX0011's shared code) past the nesting parameter the
   configuration sets, or with the tree-depth diagnostic (EX0012's) past the library's tree-depth constant, which no
   configuration sets; part G expects each source refused with its own kind's diagnostic. The plan named "the
   contract's nesting diagnostic" for both, and the left-operand charge of decision D14 is EX0012's. Found in review;
   no finding of the correspondence.
8. **Part A, comparison, and part H, configuration**: each script is committed to the bundle's `tools/` after this
   file and before any run of its part, and part A's reads only the runner's report, the classification and the
   catalogue's shared codes, with no expectation of its own. The plan said "a script committed to the bundle with
   this file"; neither script can be written before the library writes what it reads (the runner's report, the
   library's `PublicAPI.txt`). Part A's "How it is read" says why its script cannot move an expectation. Found in
   review; no finding of the correspondence.

Unchanged: the header's milestone, owner and EX-30 sentences; "What this file is"; "Which part reads which clause";
"As classified"; the commits; part A's population, completeness, controls and MET; parts B to F; part G's
configuration; part H's population and controls; "Revisions"; "What this file does not decide".

## Decision

1. **All five sub-areas move to `Broiler.ECMAScript.Frontend`**, under X3's constraints: a request with providers and
   limits, an immutable tree with full spans and raw facts, side tables ordered by source position, and diagnostics
   with shared codes, in the library's order (decisions D6 to D11, D14, D15, D17 and D32). SJL-13.2 writes it in the
   home, against the parse corpus as classified, the `syntax` vectors and fuzzing.
2. **The syntax half** reports the grammar and every early error either engine's parser reports at parse time today,
   and applies the `request` row through the request's switch; it never reports an `engine` row (decision D10). The
   stage table in the home records the assignment row by row.
3. **This family's wide path** parses with the library at step 1, through an adapter that reproduces every anchor,
   offset, computed fact and diagnostic place (decision D15), with an admission walk for the policy and limit
   refusals `JsParser` made (decision D18) and the binder's checks of `syntax` rows deleted (decision D20); at step 2
   the lowering, the numeric admission and the binder read the shared tree (decision D19).
4. **Registry revision 19** is minted at step 1's registry commit, one code per meaning a split row leaves, a meaning
   being a registry meaning group (decision D12); test262 regrades only the three language rules coded 2104.
5. **Corrections.** This family's are the rows section "The corrections step 1 makes" names, made at step 1;
   Broiler.JS's are SJL-14's.
6. **The deletion lists** are X6's (decision D20). Broiler.JS's half is completed at SJL-14.1.
7. **The bases** are bundles `sjl-13-001`'s and `sjl-13-002`'s, at the first parent of each step's merge, and the
   home's at its library commit (decisions D27 and D35). The rules are committed with this record.
8. **ADR 0011's refusal stands** for what it refuses, and does not reach the library (decision D5).

## What this rejects, and why

- **A package edge from the library to Broiler.Regex now**: it would admit an unadmitted package into this family's
  compiler or move Broiler.JS's pin early (decision D7). The edge is decision D65's.
- **A binder-side copy of the parser's checks**: two producers of one check, which roadmap section 6 item 11 and
  JSD-0014's decision 1 exclude (decisions D20 and D33).
- **A smaller tree depth**: refusals would move with no class; the library is reworked instead (decision D14).
- **Renumbering at step 2**: every renumbering is attributed at step 1's registry commit (decisions D12 and D19).
- **This family's admission or format ceilings inside the library**: JSD-0080's "What each engine keeps" keeps them
  here (decision D18).
- **An engine-only Policy row as an unconditional library refusal**: engine policy would become shared (decision D10).
- **A vector schema revision at SJL-13**: it would need a product merge here before step 1's base, for first codes the
  corpus parts already judge (decision D25).
- **A shared library that reproduces `JsParser`'s recovery or its cascade**: one engine's artefact in a shared
  library, or a class admitting any tail (decision D11, finding F9).
- **A context test at a binder site to split a meaning group**: it would change behaviour at the registry commit, and
  the matcher tells pattern rules apart only by message text (decision D12, finding F3).
- **A position class in part B**: it would admit any move of a place, for every refused input (decision D15, finding
  F10).
- **Passing the visible private names or the caller's lexical names at SJL-13**: each would move a run-time check to
  the parse (decision D16).
- **One import-phase proposal flag**: SJL-14 would accept declaration forms Broiler.JS has no deferred namespace for
  (decision D17).
- **Correcting `JsParser`'s top-level-`await` reading at step 1**: part B admits no byte difference of an accepted
  input, and the correction belongs to SJL-15's model (decision D15, finding F12).

## Settling condition

This record is settled when SJL-13's clauses 4 to 9 have been judged on bundles `sjl-13-001` and `sjl-13-002`, and
clauses 1 to 3 on the home's bundle `sjl-13-001`, whatever each decides; clause 10 is the record of JS-2's closure that
clauses 4 to 6 settle (decision D33).

## Correction entries

- **JSC-320**, in this profile's [`roadmap.corrections.md`](../roadmap.corrections.md#jsc-320): JS-2's "Next action"
  in [`roadmap.delivery.md`](../roadmap.delivery.md), which still waited on SJL-12. SJL-13.5 files JS-2's closing JSC
  entries.
- **Dated notes** beside the text they correct, each quoting it, in this record's change (decision D39): SJL-13's
  milestone file (SJL-13.2's scope and done-when, SJL-13.3's step 2 and the shared tokenizer's match, SJL-13.6's fuzz
  campaigns and measurement, clause 10); SJL-15's milestone file (SJL-15.2); the programme roadmap's sections 4, 5, 10
  and 11; `architecture.md` sections 2 and 3.1; `areas/frontend.md` sections 3 and 9; governance section 8; the
  programme README's status and its line on Broiler.JS's `FrontEnd/Semantics` boundary;
  [JSD-0081](0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md) under its decision D18 and
  X5; and [JSD-0071](0071-the-profile-on-the-shared-text-algorithms.md) under "What stays".

## What this record does not decide

- **Broiler.JS's front end**: SJL-14 moves it, files no verdict of its own, and completes X6's Broiler.JS half here by
  a dated note at SJL-14.1.
- **The semantic model**: SJL-15.
- **The library's package edge to Broiler.Regex**: decision D65's, at SJL-15.1.
- **Each publication** of the shared stream: the owner's own (decision D30).
- **SJL-14's measurement clause** while no controlled lane exists: the owner's own choice, put to the owner at
  SJL-14.1.
- **A speed claim of any kind.** Part J of bundle `sjl-13-001`'s rule and part F of `sjl-13-002`'s measure this
  family's compile, and are not gate clauses (decision D24).

## Falsified if

- A corpus input the classification does not explain at library level.
- A shared code that reaches two registry meanings.
- Two compilations of one source that differ.
- The shared tokenizer disagreeing with the slice tokenizer on an input the slice path accepts.
- A tree-depth derivation that cannot admit this family's bound on this family's stack.
