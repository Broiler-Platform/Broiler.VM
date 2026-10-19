<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0080 - The shared frontend contract and the in-place preparation: SJL-12's contract record, decisions SJL-D-11 and SJL-D-12 recorded, and the owner's decisions of 2026-10-19

**Status:** Decided 2026-10-19.
- **The shared front end's contract is recorded**: the parse request, the output, the properties, the limits, the
  providers and the inherited evaluation context (section "The contract"). SJL-13 builds the library against it.
- **This record is not an area verdict** (decision D1). It applies no X1 to X7. The frontend area's verdict is
  SJL-13.1's, which applies the programme's extraction gate to the library SJL-13 writes.
- **Decisions SJL-D-11 and SJL-D-12, taken on 2026-10-11, are recorded here**: codes and message identifiers, each
  engine rendering its own text; and a new immutable tree seeded from this family's wide design, with Broiler.JS's
  public `Ast` and `Parser` packages kept as adapter packages, obsolete and later removed. That disposition is recorded
  in Broiler.JS's `docs/public-api.md` at SJL-14.4.
- **The owner's decisions D1 to D42 of 2026-10-19 are recorded**, taken under the owner's direction of 2026-10-18.
- **Both front ends are prepared in place under this record**: Broiler.JS's by SJL-12.5, this family's by SJL-12.6.
  Nothing in this profile changes with this record. Its rules are committed with it, before any code they judge
  (decision D8).
- **Corrections are listed before any run.** Broiler.JS has two, J1 and J2 (decision D21), and its rule also
  admits a compile crash the preparation resolves, as a correction its changelog names. This family has the template
  corrections of decision D15. No other change either engine makes at SJL-12 moves an input between accepted and
  refused.
- **The diagnostic registry stays at revision 18** (decision D13). The mapping records every code a later revision
  must split, and SJL-13 mints the new numbers.

**Date:** 2026-10-19.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the other engine's owner and the home's;
- the verification-boundary owner, whose JSD-0014 this record adopts as a requirement on the shared front end;
- the conformance owner, whose JSD-0009 governs the registry the shared codes map onto.

**All four roles are held by one person** (EX-30), and this record does not claim that anyone independent has
read it.

**Milestone:** the programme's SJL-12, work packages SJL-12.1 and SJL-12.2. The same contract is recorded in
Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "The frontend contract (SJL-12)",
and in the home's design document, `Broiler.ECMAScript/docs/frontend/design.md` in the Broiler.JS repository
(decision D5). Where either differs from this record, this record and the programme's area design rule, and the other
document is corrected.

**SJL-12 started on 2026-10-19, by the owner's choice.** A survey of both front ends for SJL-12's plan had been
started on the recommendation that SJL-12 feeds SJL-13 on the critical path and may start after SJL-0. On
2026-10-19 the owner wrote: "Continue with SJL-12 after the plan is ready." This record is filed from that plan.
- **SJL-12's in-place work packages need no admission** (the milestone's "Waits on"), so the words choose the
  milestone and lift no gate.
- **SJL-12 may not meet its gate before SJL-3 meets SJL-3's**, because the contract cites SJL-3's literal-value
  operations. The ledger's update rule 10 leaves SJL-12's row out only because SJL-12 may start from SJL-0. This
  record moves no gate.

**The owner's direction.** On 2026-10-18, after SJL-10's correspondence reported its open points, the owner wrote:
"Please do all recommended decisions and skip waiting for review." This record reads it in two parts, as JSD-0078
did:
- each decision SJL-12's plan leaves open is taken as the plan recommends, and recorded below as the owner's
  decision of 2026-10-19 under that direction (decisions D1 to D42);
- the programme proceeds through SJL-12's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-12 is accepted by it.
- SJL-12 still may not meet its gate before SJL-3 meets SJL-3's.
- Rules still come before runs. Bundle `sjl-12-001`'s rules are committed with this record (decision D8).

**Names.**
- **Decisions.** "Decision Dn" is always this record's. Another record's decision is named with its record, as in
  "JSD-0078's decision D12".
- **Corrections and rules.** Broiler.JS's two corrections are "correction J1" and "correction J2". This family's
  assurance rules are always "rules J1 to J12".
- **Items.** The work packages' numbered items are "SJL-12.5 item n" and "SJL-12.6 item n", as the milestone numbers
  them.
- **The bundle's names.** The sub-populations B1 to B4, the request-flag sets E0 to E7, the difference classes and
  the controls K1 and K2 are the names bundle `sjl-12-001`'s rule gives them.

---

## Context

**The design** is the programme's
[`areas/frontend.md`](../../../../docs/javascript-shared-libraries/areas/frontend.md) and
[`areas/static-semantics.md`](../../../../docs/javascript-shared-libraries/areas/static-semantics.md). They state
both front ends as read at Broiler.VM `88d8b51d` and Broiler.JS `44a214dc`.

**The facts below were re-derived from code** at **Broiler.VM `046357b`** and **Broiler.JS `24510b0e`**, on
2026-10-19.
- **Five surveys did the work**: the programme's records, Broiler.VM's front end, Broiler.JS's front end, the
  early errors of the pinned specification, and a parse corpus.
- **An independent critic read each survey**, and the plan applied the critics' corrections.
- **Every fact this record states was read again** from the code at those commits.

**Two gaps are accepted as recorded:**
- the surveys' probes and notes are not retained. The probes that witnessed corrections J1 and J2 are reproduced as
  tests by SJL-12.5 item 1 (decision D21);
- the surveys' figures are not quoted here. They are not evidence. Bundle `sjl-12-001` takes its own runs, under its
  rule.

**What the re-derivation found in this family's wide path:**
- **Analysis and emission share methods.** `HoistProgram`, `HoistFunction` and `HoistEval` collect declarations and
  emit in the same bodies, and evaluation scope-map rows are recorded during emission.
- **Two outputs follow a dictionary's order.** The global lexical names are held in `programLexicals`, a
  `Dictionary<string, bool>`, and `HoistEval`'s lexical names in another. The emitted order is the dictionary's
  enumeration order.
- **Early errors are reported in two stages after tokenizing.**
  - The parser reports its static-semantic checks: `ValidateDeclaredNames` and its family in `JsParser`, from
    `LexicallyDeclaredNames` to `ValidateAccessorParameters`.
  - The lowering reports the rest: `super`, `return` and `new.target` placement, cover-grammar leftovers, duplicate
    `__proto__`, regular-expression patterns and flags, `break` and `continue` targets, assignment targets, `const`
    initialisers, private names, `arguments` in field initialisers, `import.meta`, and the module declaration,
    collision and export checks.
- **Three of the lowering's language rules are coded 2104**, the manifest's code: `await` outside an async function,
  a logical assignment to a target that is neither a name nor a property, and a `using` declaration outside a
  statement list that can dispose it. `return` inside a class static block is coded 2207.
- **Two artefacts of the lowering show in its diagnostics.** A `finally` block is lowered more than once, so a
  diagnostic inside one can repeat. The lowering also stops a construct at its first refusal, so a later early error
  in it is not reported.
- **Template substitutions are re-tokenised.** `TemplateReader` reads them heuristically, and `ParseInterpolation`
  parses each with a fresh tokenizer and sub-parser. The sub-parser does not carry the enclosing parse's nesting depth
  or parameter context.
  - Template nesting is not counted toward the nesting bound, so a deep enough nesting ends the process in the wide
    parser, past the tree-depth guarantee.
  - `${yield}` and `${await 1}` are accepted in formal parameters.
- **The shared tokenizer's template scan recurses.** `SliceTokenizer`'s `ScanTemplateBody` and `ScanSubstitution`
  call each other, so a deeper nesting ends the process in the tokenizer both paths share.
- **One public entry point parses on the caller's stack.** `JsCompiler.Requests` does not run through
  `CompilationStack`.
- **The tree-depth margin is unmeasured.** `CompilationStack`'s remarks say that whether the compile walk's own
  frame cost still leaves `SliceParseOptions.MaximumTreeDepth` the margin it was derived with has not been
  re-measured.
- **The determinism row covers the slice path only.** The slice compiler's checks row "the lowering is
  deterministic" compiles the slice front end's sources twice. No row compiles the wide path twice.

**What the re-derivation found in Broiler.JS:**
- **The parser reads ambient state.** `FastParser` and `FastScanner` read the module goal and the top-level-`await`
  permission from `CoreScript`'s `AsyncLocal` scopes in `Broiler.JavaScript.Runtime`, through `InternalsVisibleTo`.
- **Its temporaries come from a process-wide counter.** `FastParser.TempVarID` is a mutable static, so the tree
  depends on what the process parsed before.
- **Every parse writes a shared token.** `FastScanner.EOF` and `FastScanner.EmptyToken` are static read-only fields
  of the mutable type `FastToken`, and every parse writes the end token's `Previous`.
- **The parser references six `Runtime` types, not three.** The Parser assembly's metadata names `CoreScript`,
  `CancellableDisposableAction`, `JSUnicode`, `JSUnicodePropertyNames`, `RegExpGroupNames` and
  `RegExpUnicodeValidator`. The last four came with the regular-expression literal's early checks, merged as
  Broiler.JS #1041 on 2026-10-17.
- **The parser rewrites three constructs.** It desugars `for (let …)` and `for (const …)` loops, with temporaries,
  copy-back and an injected `break`. It also synthesizes a block for an Annex B.3.4 function declaration that
  shadows a parameter, and decodes a regular-expression literal's braced `\u{…}` escapes under `u` and `v`.
- **The top-level-`await` permission leaks into code compiled during a run.** While `EvalWithTopLevelAwaitAsync`
  runs, the ambient permission reaches every compilation it starts: direct and indirect `eval`,
  `ShadowRealm.prototype.evaluate`, the `Function` constructor and the debugger's evaluate (correction J1).
- **The code cache does not key the permission.** In one context, the same text answers from whichever mode's
  compilation ran first (correction J2).

**What the design reads as current and is not:** section "The design's readings" lists it. The design is corrected
beside each reading (decision D42).

## This record is not an area verdict (decision D1)

Roadmap section 5 applied the programme's extraction gate "at the start of every area milestone (SJL-3 to SJL-15,
and SJL-17)". **SJL-12 applies none of it.**
- **SJL-12 writes no shared code.** Its milestone's "Does not do" says so, and the shared syntax library is SJL-13's.
- **So X3 to X6 have no subject here.** There is no shared member whose contract X3 could judge, no library whose
  edges X4 could read, no closure X5 could bound and no cut-over whose deletions X6 could list.
- **Its milestone has no verdict work package**, and its exit gate no X clause.
- **The frontend's verdict is SJL-13.1's.** SJL-13.1 applies the gate to the library SJL-13 writes, against this
  contract, the catalogue and the corpus.

Roadmap section 5's range is corrected in place to leave SJL-12 out, with a dated note that quotes it and records
this reading. The note also records that section 11 names no verdict JSD for SJL-13, though SJL-13.1 files one. The
ledger's entry for SJL-12's start states that SJL-12 cannot exit before SJL-3 does.

## The contract (SJL-12.1)

The full design is [`areas/frontend.md` section 4](../../../../docs/javascript-shared-libraries/areas/frontend.md#4-the-contract).
This record fixes it, with what decisions D27 and D41 add.

### Input: an immutable parse request

| Field | Content |
|---|---|
| **Source** | Text, source name, base offset, base line and column (for sub-parses) |
| **Goal** | `Script`, `Module`, `Eval`, `FormalParameters` or `FunctionBody`. The last two let `CreateDynamicFunction` parse its pieces as the specification requires, which neither engine does today. **`Eval`, `FormalParameters` and `FunctionBody` are reserved for both engines** at SJL-12: neither engine's in-place options carry them as goals of their own. Broiler.VM's eval code is the `Script` goal with `SliceParseOptions.IsEval` and the request's flags (decision D41). |
| **Switches** | Forced strictness; top-level `await` allowed; Annex B on or off; HTML-like comments on or off (script goal only) |
| **Inherited evaluation context** | This family's six evaluation flags, `JsFormat.EvalRequestFlags`: `Strict`, `InFunction`, `InMethod`, `InDerivedConstructor`, `InClassFieldInitializer` and `InClassBody`. Also the visible private names, and the caller's lexical names, which Broiler.JS needs for direct `eval` (decision D41). |
| **Proposal flags** | Decorators, `accessor`, import phases. **Off by default**; an engine admits what its own manifest admits (decision D41). |
| **Lexer extensions** | One narrow parameter for non-standard literal suffixes. Broiler.JS's `m` is the only one, and its `Decimal` meaning stays in Broiler.JS (SJL-D-17; decision D41). |
| **Limits** | See "Limits" below |
| **Providers** | See "Providers" below |

### Output

| Part | Content |
|---|---|
| **Tree** | Immutable nodes with full spans (offset, length, line, column), and **raw facts**: directive raw text, legacy-octal and escape flags, cooked and raw template chunks, regular-expression pattern and flags, BigInt literal text, parenthesisation, trailing commas. Side facts sit in identity-keyed tables. **No desugaring.** |
| **Diagnostics** | A stable code, a phase (tokenizer, parser, semantics), a span, a source name, and a message identifier with arguments, in a deterministic order. Each engine renders its own text (SJL-D-11). |
| **Semantic model** | Section "The semantic model's design" below |

### Properties

- **Deterministic.** The same request gives the same output, across runs and threads.
- **No ambient state and no mutable static** (rule SL2, the shape of this family's rule N12).
- **Limits are checked before the stack is.** Recursion is bounded by explicit stacks, or by depth checks on the
  large thread the caller supplies (`CompilationStack` here).
- **Native AOT compatible and trimmable.** No reflection and no dynamic code.
- **Never a source-to-verified-artifact path.** ADR 0010 forbids such members in Broiler.VM, and the front end
  produces trees, never artifacts.

### Limits (decisions D27 and D41)

- **The nesting depth is a bounded parameter.** This family's default and maximum are `SliceParseOptions`'s
  `DefaultMaximumNestingDepth` and `MaximumSupportedNestingDepth`.
- **The tree depth is a constant of the library.** It is never host-settable, which keeps rule N19's property. This
  family's is `SliceParseOptions.MaximumTreeDepth` today. SJL-13 derives the shared one as JSD-0022 derived this
  family's, against the caller's large thread.
- **In both engines, every input deeper than the bound is refused** with the contract's nesting diagnostic
  (decision D27).
- **A diagnostic cap**, the wide parser's own cap today.
- **A first-error or all-errors policy.**

### Providers

- **Unicode lexical classes** come from the Unicode owner, Broiler.Unicode (SJL-7). Both engines already read them
  from there: this family through `JsCompilerUnicode` since SJL-7.4, and Broiler.JS through `JSUnicode` since
  SJL-7.5.
- **Regular-expression syntax validation** goes through a seam until SJL-11's engine serves both engines. Behind the
  seam is the engine's own matcher or validator.

## Decisions SJL-D-11 and SJL-D-12, recorded (SJL-12.2)

**SJL-D-11, taken 2026-10-11: codes and message identifiers.**
- **What the front end emits**: a stable code, a phase, a span, a source name, and a message identifier with
  arguments. An optional default English rendering may come with them.
- **This family maps codes onto its registry**, with a new registry number for every meaning that changes, as
  JSD-0009 requires (decision D13).
- **Broiler.JS renders its V8-style text** from the identifier and the arguments.
- **Neither engine's guest-visible text is part of the shared contract.**
- **Its first application is the early-error catalogue and the mapping of SJL-12.3** (decisions D30 and D31).

**SJL-D-12, taken 2026-10-11: (a) with (i).**
- **(a) The tree** is a new immutable tree, seeded from this family's wide design, with an adapter into each
  consumer's current tree during the transition.
- **(i) Broiler.JS's public `Broiler.JavaScript.Ast` and `Broiler.JavaScript.Parser` packages** become adapter
  packages that build the old tree from the new one. They are marked obsolete, then removed in a later major
  version.
- **This record records (a) and (i). The packages' disposition** is recorded in Broiler.JS's `docs/public-api.md` at
  SJL-14.4, when the adapters exist. SJL-12 changes neither package's surface except as decision D29 lists.

## SJL-D-13 and SJL-D-17, as this record applies them

**SJL-D-13 (a): this family's slice path is untouched at SJL-12, and the binder is temporary.**
- **SJL-D-13 (a) rebuilds the slice path on the shared tree and semantic model at SJL-15**, keeping its census
  counts, slot order and refusals byte for byte. Preparing it in place now would be work SJL-15 replaces, under an
  acceptance check SJL-15 already owns. So SJL-12.6 changes no slice outcome.
- **The one exception is the shared tokenizer's template scan** (decision D15). It becomes iterative, so a deeply
  nested template no longer ends the process. No slice outcome changes for any input that completes today.
- **The binder pass SJL-12.6 separates is the wide path's, and it is temporary.** It is this family's own form of
  the semantic model's facts. SJL-15 replaces it with the shared model, and its record is SJL-15.4's, which also
  records SJL-D-13.

**SJL-D-17: the `m` suffix is a lexer-extension parameter.** Broiler.JS's `Decimal` literal stays Broiler.JS's. The
shared tokenizer takes one narrow parameter for non-standard literal suffixes and implements only the specification
without it. Broiler.VM passes none, and refuses `1m` with 2003 as it does today.

## What this record adopts, and what it is compatible with

- **JSD-0014's decisions 1 and 4 are adopted as requirements on the shared front end** (governance row P6).
  - Decision 1: static semantics is one stage, the only place an early error is reported.
  - Decision 4: parse options are a value, and no front-end state outlives a call.
  - SJL-12.5 removes Broiler.JS's ambient state, and SJL-12.6 moves the wide path's lowering errors into one stage.
    The shared library meets both requirements at SJL-13 and SJL-15 (decision D10).
- **JSD-0014's decision 5 is kept**: deep nesting is refused, at an explicit bound (decision D27).
- **The record is compatible with JSD-0036** (governance row P10). JSD-0036 reads: "the front end returns a
  validated tree; the lowering consumes it and emits universal bytecode". The shared front end is that front end.
  Each later cut-over lands on whichever lowering exists then.
- **It applies JSD-0009's rule** that a rejection whose meaning changes takes a new number while the old one is
  retired. No code changes meaning at SJL-12 (decision D13).

## The semantic model's design

[`areas/static-semantics.md` section 3](../../../../docs/javascript-shared-libraries/areas/static-semantics.md#3-the-model)
sketched the model's shape and said that its design is filed at SJL-12.1. **It is filed here** (decision D41).
SJL-15 builds it.

**What the model answers**, one fact per question, every fact a language fact:
- **Scopes**, in source order, and the scope of any node: program, function, block, module, `with`, eval, class body,
  and the separate parameter scope of a function.
- **Declarations**: each binding identifier's declaration, with its kind (`var`, `let`, `const`, `class`, function,
  parameter, `catch`, import) and mutability, and every conflict.
- **Annex B.3.3 block-function aliases.**
- **Per-function facts**:
  - strict, simple parameters, parameter expressions;
  - a separate variable environment;
  - `arguments` needed and mapped;
  - direct `eval` in the body or the parameters, and `with`;
  - the function's kind.
- **Resolution** of each identifier reference: static, to a scope and declaration; dynamic, with its reason, where
  `with` or sloppy direct `eval` makes it so; or global.
- **Jump targets** for `break` and `continue`, and labels.
- **Private-name environments** and the resolution of each private-name reference.
- **The module record**: requests with attributes, top-level `await`, import and export entries, and their errors.
- **Named-evaluation names.**
- **Free names and captures**, per function.
- **The early errors** that are not purely grammatical, in a deterministic order.

**How it answers:**
- **Every collection is in source order.** No order comes from a hash table.
- **Facts are immutable and keyed by node identity.** An ordered list keeps a declaration's multiplicity where the
  language has more than one.
- **It is computed per compilation unit**, after the parse.
- **Inherited context comes from the request.** Evaluation code takes the caller's scope facts from the parse
  request. This family passes its evaluation flags and the names its scope map records. Broiler.JS passes them from
  its own direct-`eval` support.

**What it does not answer** stays each lowering's: slot and environment-record allocation, this family's evaluation
scope-map rows, Broiler.JS's boxing, deferral and closure representation, admission against a manifest, emission
order and instruction choice, and format ceilings.

**SJL-12.6's binder pass computes these facts for the wide path** (decision D11). It reproduces today's
approximations exactly, so that bytes do not move: `UsesArguments`' over-approximation, and the `!simple` and
`!strict` gates of the separate body. SJL-15's model replaces the approximations with the specification's facts,
under SJL-15's own rules.

## What each engine keeps

| Broiler.VM keeps | Broiler.JS keeps |
|---|---|
| Manifest admission and its "unsupported" refusals (code 2104) | Its `for` per-iteration lowering, moved out of the parser into a compiler preparation pass at SJL-12.5 (decision D22) |
| The diagnostic registry and its mapping from shared codes | Its V8-style message rendering |
| Format ceilings (locals, constants, stack, arguments, BigInt width) | Numeric speculation, boxing and deferral |
| Slots, environment records, evaluation scope-map rows | Its expression-tree emission |
| Byte determinism of artifacts, and verification | Run-time evaluation support |
| The slice path's refusal of unresolvable identifiers, as an admission rule | The `m` literal's `Decimal` meaning (SJL-D-17) |
| `CompilationStack`'s thread | Stack segmentation, if it keeps it |

**What stays outside "reads no ambient state"** for Broiler.JS's parser at SJL-12 (decision D24):
- `StackSegment`'s `[ThreadStatic]` flag and its environment variable;
- `CompilationStack`'s statics;
- `Ast`'s `FastToken.Empty` and `ArraySpan<T>.Empty`;
- `FastCompiler`'s read of `JSEngine.Current` for direct-`eval` context.

Each decides where a parse runs, or is the compiler's. The inherited context becomes an option of the request at
SJL-14.

## The owner's decisions of 2026-10-19

Each decision was taken on 2026-10-19 by the owner, under the direction quoted above, as SJL-12's plan recommends it.
Each states its reason. Broiler.JS's plan section records decisions D7, D19 to D29 and D38 by these numbers.

### The milestone's reading, and the records

**Decision D1 - SJL-12 is not an area milestone owing an X1 to X7 verdict.** The frontend's verdict is SJL-13.1's.
- A dated note under roadmap section 5 says so. It also says that section 11 names no verdict JSD for SJL-13, though
  SJL-13.1 files one.
- The ledger's start entry says that update rule 10 leaves SJL-12 out only because SJL-12 may start from SJL-0, and
  that SJL-12 cannot exit before SJL-3 does.
- The reason: SJL-12 has no verdict work package and writes no shared code, so X3 to X6 have no subject. SJL-13.1
  applies the gate.

**Decision D2 - no start record.** There is no SJL-12.0. SJL-12's start is one dated ledger entry, quoting the
owner's words that chose it and the standing direction. The reason: SJL-12's in-place work packages need no
admission (the milestone's "Waits on").

**Decision D3 - numbers and names.**
- This record is JSD-0080.
- The bundle is `sjl-12-001` in both repositories. This family's rule file is `decision-rule-area.md`, and
  Broiler.JS's is `decision-rule.md`.
- A JSC entry is filed for SJL-12.6 only, at the next free number.
- EX-133 is taken only if an exclusion is needed, and none is planned.
- The reason: the precedent of SJL-6 to SJL-10. Numbers are taken at filing.

**Decision D4 - this record's people and status.** The status is "Decided" with the filing date. The owner is the
JavaScript profile owner. The co-signers are the Broiler.JS maintainer and the holders of JSD-0014 and JSD-0009. The
EX-30 sentence is stated. The reason: as JSD-0078.

**Decision D5 - the home's design document** is `Broiler.ECMAScript/docs/frontend/design.md` in the Broiler.JS
repository, linked from a dated revision of `Broiler.ECMAScript/README.md`.
- The catalogue sits beside it (decision D31).
- No `Broiler.ECMAScript.Frontend/` folder is created at SJL-12.
- The reason: SJL-12 writes no library code. A library folder would bring in the conventions' scope and rule SL1's
  table.

**Decision D6 - Broiler.JS's record** is a new section of its shared-libraries plan, after "Area verdicts", with
status rows. The reason: that section's preamble says every entry applies X1 to X7.

**Decision D7 - SJL-12.5's base is taken afresh**, at the Broiler.JS rule commit, with negative tests.
- The milestone's SJL-12.5 done-when read "against SJL-0's base". It is revised as a work package's done-when
  (roadmap section 13), quoting the old text, and the ledger says so in one sentence.
- It is not a gate revision: exit clause 2, "test262 per path meets the predeclared rule", names no base.
- The reason: SJL-0's base predates SJL-3 to SJL-10 and excludes negative tests, and Broiler.JS's comparison script
  refuses two runs whose `includeNegative` differs.

**Decision D8 - the rules are committed with SJL-12.1**, before any code they judge. No work package of the milestone
named them. A dated milestone note says so.
- The reason: SJL-12.5 and SJL-12.6 are judged "against the predeclared rule". Rules come before runs (validation
  principle 3; roadmap section 6 item 10).

**Decision D9 - the base commits.** test262's bases are taken at each repository's rule commit. The corpus and
determinism bases are taken at each repository's corpus commit, under the path-list, ancestry and identity
conditions each rule states. The reason: the corpus is built after the rules, and its base still precedes every
change the rules judge.

### This family's preparation, SJL-12.6

**Decision D10 - clause 4's "one stage" is read, and recorded.**
- After parsing, the wide path has one early-error stage, the binder pass. Every early error the lowering raises
  today moves there (SJL-12.6 item 2).
- The wide parser keeps its own static-semantic checks, `ValidateDeclaredNames` and its family, until SJL-13 deletes
  `JsParser` and SJL-15's model takes every early error that is not purely grammatical.
- JSD-0014's decision 1 is adopted as a requirement on the **shared** front end (governance row P6), which SJL-13 and
  SJL-15 meet.
- The reason:
  - item 2 moves errors "from the lowering";
  - moving the parser's checks now is work SJL-13 deletes;
  - it would also change the first code of refused inputs, for which the artifact-identity template has no class.

**Decision D11 - the binder's shape.**
- `internal sealed class JsBinder`, in its own files of the compiler assembly.
- It runs per compilation unit, after the parse and after `JsNumericAdmission`.
- Its facts are immutable and keyed by node identity (`ReferenceEqualityComparer`), with ordered lists that keep
  multiplicity. They are computed once and read by each emission, so a `finally` block the lowering emits more than
  once reads the same facts each time, and the binder reports each diagnostic once.
- It reproduces today's approximations exactly.
- `HoistProgram`, `HoistFunction` and `HoistEval` read the facts, and keep allocation, interning, unit compilation and
  emission order.
- The reason: it is the only shape that keeps bytes identical. SJL-15 swaps it for the shared model.

**Decision D12 - which errors move, and how they are reported.**
- **They move with their codes unchanged**: every language early error the lowering reports. That is each check the
  "Context" section lists, with the module declaration, collision and export checks.
- **The three 2104-coded language rules, and 2207 for `return` in a static block, move with the rest and keep their
  codes.** They become catalogue rows, and the mapping records them as split candidates (decision D13).
- **A binder refusal skips the lowering.** The binder reports in source order, within the wide parser's diagnostic
  cap.
- **These stay in the lowering**, because they are not language early errors:
  - format ceilings, 2301 to 2303;
  - manifest and form refusals, 2104, in `JsCompiler.Assembly.cs`;
  - module-load policy: 2405, 2406 and the duplicate module key;
  - the eval scope-depth ceiling.
- The reason: JSD-0014 holds that an early error never produces bytes. Changing a code is SJL-13's registry
  revision.

**Decision D13 - no registry revision at SJL-12.** The registry stays at revision 18.
- **The mapping records the split candidates**:
  - 2206, for undeclared private names and for `arguments` in field initialisers;
  - 2402, for duplicate import-attribute keys;
  - 2207, for `return`;
  - 2104, for `await` and `using`;
  - 2203, for destructuring without an initialiser;
  - 2101, from the binder, for `super`, `return`, `new.target` and regular-expression patterns.
- **SJL-13 mints the new numbers** under JSD-0009.
- The reason: SJL-12 "changes no language surface". A recode would move the retained conformance suite's scoring,
  which `LanguageErrors.MayScore` grades by code, and part B's same-code class.
- The whole-suite test262 run is not regraded by a split. `Test262Run`'s refusal grading passes a refusal of any
  parse- or resolution-phase `SyntaxError` negative test unless its first diagnostic is 2104.

**Decision D14 - the ordering change lands first, alone, and is stage JSB-1's standing check.** Section "Stage
JSB-1" states it.

**Decision D15 - structured templates.**
- **The tokenizer's mode.** An internal opt-in mode of the tokenizer, set only by `JsCompiler`, tokenizes each
  substitution with the real lexer into a side table keyed by token index.
- **The parser.** The same `JsParser` instance parses the substitutions, so the nesting depth, the tree depth, the
  parameter context, the labels and the offsets carry through. `TemplateReader` and the re-tokenising
  `ParseInterpolation` are deleted.
- **No public member changes** (rule N10).
- **Corrections, listed now:**
  - `${yield}` and `${await …}` in formal parameters are refused with 2101;
  - template nesting counts toward the nesting bound, and past it is refused with 2103;
  - `SliceTokenizer`'s template scan becomes iterative. This is a listed slice correction, in which only a crash
    becomes a refusal.
- **Positions inside substitutions may only become correct** (bundle class P).
- **A dated note on the milestone's "Does not do"** says that no slice outcome changes for any input that completes
  today.
- The reason: it fixes two acceptance defects and two stack overflows that end the process, the wide parser's and
  the tokenizer's, at no cost to the slice path's outputs.

**Decision D16 - `JsCompiler.Requests` runs through `CompilationStack`, at SJL-12.6 item 4.**
- Widening rule N19's textual pattern, so that it catches such an entry point, is SJL-13's rule revision. A dated
  note on roadmap section 10's SJL-13 row records it.
- The reason: a source deep enough ends the process today, because the parse runs on the caller's stack. Roadmap
  section 10 mints and revises no rule at SJL-12.

**Decision D17 - N19's property, not only its text.**
- The binder's walks are iterative.
- Two checks rows hold the property: a source at `MaximumTreeDepth` compiled through the binder; and template nesting
  past the bound, refused.
- The reason: `CompilationStack` records that the margin has not been re-measured, and the template sub-parse broke
  the tree-depth guarantee.

**Decision D18 - rule N12 and assurance.**
- N12's test asserts how many of the compiler assembly's files it scans. Each commit that adds a file updates that
  assertion, with a dated comment naming the file.
- Annotations are added and the generated artefacts regenerated in the same change, and rules J1 to J12 stay green.
- A moved unit starts unreviewed.
- The reason: roadmap section 6 items 2 and 3.

### Broiler.JS's preparation, SJL-12.5

**Decision D19 - the options' shape.**
- **`FastParserOptions`**: an immutable type in the Parser assembly, carrying:
  - the goal: `Script` or `Module`, with `Eval`, `FormalParameters` and `FunctionBody` reserved;
  - `AllowTopLevelAwait`;
  - the providers.
- **Where it goes.** It is passed to `FastScanner`, `FastTokenStream`, `FastParser` and `ParseModule`. The parser
  refuses a stream built with other options.
- **`JSCompilationOptions`** gains `bool AllowTopLevelAwait { get; init; }`, which enters the code cache's key.
- **`IJSCompiler`** gains a default interface method, `Compile(in StringSpan, in JSCompilationOptions, …)`. It forwards
  for a script without top-level `await`, and otherwise throws `NotSupportedException` naming the host compiler.
- The reason:
  - the scanner reads two tokens in its constructor, so the options must reach it there;
  - `Runtime` cannot name `Parser`'s types;
  - binary compatibility is kept.

**Decision D20 - the six `Runtime` types.**
- **`CoreScript`'s goal state becomes options** (decision D19).
- **These move to `Broiler.JavaScript.Ast`**: the Unicode tables and their `IUnicodeTables` implementation,
  `JSUnicode`, `JSUnicodePropertyNames`, `RegExpGroupNames` and `RegExpUnicodeValidator`.
  - `Ast` is already below `Runtime` and `Parser`, and already references `Broiler.Unicode.Algorithms`.
  - The public validator keeps a `[TypeForwardedTo]` in `Runtime`.
- **The options' providers are typed by Broiler.Unicode's `IUnicodeTables`.** The `Ast`-held table is the default,
  and there is **no default that throws**.
- **`CancellableDisposableAction` moves to `Ast`** with a forwarder, and `UndoMark` is marked `[Obsolete]`
  (SJL-D-25 (b)). Removal under SJL-D-25 (c) waits for a recorded consumer check.
- The reason:
  - no new package and no behaviour change;
  - the portable compiler and the parser's tests keep working;
  - the contract names the Unicode owner as provider. SJL-13 defines the shared provider, and the tables move once
    more then, to the engine's provider home.
- **A default that throws was rejected**: the portable compiler has no `Runtime` reference, and the parser's Unicode
  and regular-expression tests construct parsers without one.

**Decision D21 - the witnessed leaks are listed corrections.**
- **Correction J1.** Code compiled while `EvalWithTopLevelAwaitAsync` runs, but not as its own program, is compiled
  as the specification's goal without top-level `await`, as under `Eval`. This covers direct and indirect `eval`,
  `ShadowRealm.prototype.evaluate`, the `Function` constructor and the debugger's evaluate.
- **Correction J2.** The code cache keys a compilation on its top-level-`await` permission. One context no longer
  answers a compilation from the other mode's entry.
- **Both are listed** in Broiler.JS's `CHANGELOG.md` and in its rule. The message "Unexpected await" is kept, because
  the shell's retry reads it.
- The reason: probes witnessed both at `24510b0e`, and the milestone allows "listed corrections".

**Decision D22 - the `for`-loop pass.**
- **Where.** A compiler preparation pass runs after the parse, at `FastCompiler`'s and `DirectEvalSupport`'s
  `ParseProgram` calls, before `SyntaxValidation`. It emits today's shapes exactly.
- **What moves to it.** The parser records `ContainsContinue` on `for`, `for-in` and `for-of`. `HeadTdzNames` and the
  `using`-in-`for` block wrap move to the pass. `ThrowIfLabelledFunctionLoopBody` stays in the parser.
- **Temporary names** are `<d>:<n>`. `d` is the direct-`eval` nesting depth the compilation inherits, and `n` is a
  per-pass counter from 1. The names are deterministic, are never identifiers, and differ from an enclosing
  compilation's carriers.
- **The parser stops using the three desugaring walkers**, `AstIdentifierReplacer`, `IdentifierNameCollector` and
  `ClosureDetector`, which `Ast` declares.
- The reason:
  - the parser's own comment on its `continue` count warns that a search of the tree misses a `switch` case's
    statements, and that one missed `continue` turns a working loop into an infinite one. So the parser records the
    fact while it parses, and the pass reads it;
  - the compilation identifier comes from a process-wide counter, so it cannot name temporaries.

**Decision D23 - the other parser rewrites stay.**
- Clause 1's "does not desugar" is read as SJL-12.5 item 5's `for`-loop desugaring.
- These two stay, listed here and in Broiler.JS's plan for SJL-14, whose shared tree keeps raw facts:
  - the Annex B.3.4 synthetic block, in `FastParser.Statement.cs`;
  - the regular-expression literal's `\u{…}` decoding, in `FastScanner.cs`.
- The `using`-in-`for` wrap moves (decision D22).
- The reason: neither introduces temporaries or restructures control flow. Moving the Annex B block touches
  `SyntaxValidation` for no gain at SJL-12.

**Decision D24 - the scope of "reads no ambient state"** is the Parser assembly's own declarations and references.
Section "What each engine keeps" lists what stays outside it. The reason: it keeps clause 1 from claiming more than
the parser.

**Decision D25 - where the architecture test lives.**
- A new project, `Broiler.JS/Broiler.JavaScript.Architecture.Tests`, in the solution.
- It links `Broiler.ECMAScript.Architecture.Tests`' `AssemblyFacts.cs`, compiles its witness in memory, and reads
  `BROILER_ARCH_PARSER_ASSEMBLY`.
- There is no engine rule register. The test asserts an exact list of today's violations, and each later item shrinks
  the list.
- The reason: the parser's tests have no Roslyn, and the integration tests would bury the test. Every step stays
  witnessed.

**Decision D26 - Broiler.JS's order.** Items 6, 3 and 2 (the architecture test, the end token, the counter) land in
one pull request; then item 1; then item 4; then item 5 with the records. The reason: each step is testable alone,
and the riskiest comes last.

**Decision D27 - the deep-nesting policy is decided now**, as `areas/frontend.md` section 9 asks.
- The tree depth is a library constant, derived as JSD-0022 derives this family's, against the caller's large thread.
  The nesting depth is a bounded parameter.
- In both engines, every input deeper than the bound is refused with the contract's nesting diagnostic.
- Broiler.JS's inputs that it accepts today past the bound become **listed refusals at SJL-14**, as an
  implementation limit (class C3), enumerated from the parse corpus's baseline.
- The bound's value is SJL-13's to derive. SJL-12 changes no Broiler.JS acceptance.
- The reason: it keeps rule N19 and JSD-0014's decision 5, and answers the design's open row.

**Decision D28 - MOD-M2.** The disposition of Broiler.JS's modernization items:
- **MOD-M2-4 closes at SJL-12.5.** Dated revisions follow in `Modernization.md`, `Assemblies.status.md` and
  `dependencies.md` (governance row J2).
- **MOD-M2-5 is superseded in part.** The contract supersedes its boundary specification, and SJL-13 to SJL-15
  supersede its extraction plan.
- **MOD-M2-3 stays open.** No SJL-12.5 item touches Storage's `Ast` dependency or the foundation cycle. Its
  cross-reference of 2026-10-12 is corrected in `Modernization.md`, and governance row J6 is corrected beside it.
- **Roadmap section 11's SJL-12 cell for Broiler.JS**, "MOD-M2-3, MOD-M2-4 and MOD-M2-5 closed or cross-referenced",
  was met by the cross-references of 2026-10-12.

**Decision D29 - the public API.** Forwarders by default. `docs/public-api.md` records these at SJL-12.5:
- the new overloads;
- `JSCompilationOptions.AllowTopLevelAwait`;
- the `IJSCompiler` default method, as a **behavioural** change for host compilers;
- `UndoMark` obsolete;
- the Parser package's dependency on `Runtime` dropped, which is source-breaking for transitive users;
- the changed `JSCode.Key` text.

The changelog lists corrections J1 and J2 and each of these. The reason: SJL-D-25, SJL-D-12 (i) and `risks.md`'s
row D6.

### The catalogue, the mapping and the corpus

**Decision D30 - the catalogue's form.**
- **The edition** is ES2026 at `tc39/ecma262@0248456c`, whose document digest begins `ce7bc301`. The es2026-errata
  tag `d89c03f2` is recorded as changing no rule.
- **Rows** are `EE0001` onward, one per rule of the specification's early-error clauses, in document order. They come
  with four families of their own:
  - **EA**: eval and dynamic functions;
  - **EG**: Annex B grammar-only rules and the parser's Annex B relaxations;
  - **EP**: proposals that test262 tests or either engine parses (explicit resource management, decorators, import
    phases), each marked admitted or not per engine;
  - **EX**: engine-only refusals.
- **Identity.** Identifiers are never reused. A row's content key is its anchor, its productions and a short digest
  of its rule text.
- **Codes and identifiers.** Shared codes are `JSE` with four digits, grouped by clause. Message identifiers are
  dotted, as in `scope.lexical.duplicate`.
- The reason: both repositories already pin this edition, and a re-pin is a mechanical difference.

**Decision D31 - the homes.**
- **The catalogue** is Broiler.JS's `Broiler.ECMAScript/docs/frontend/early-errors.tsv`, with `early-errors.md`.
- **The mapping** is this family's `src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv`. It names
  the catalogue by Broiler.JS commit and SHA-256.
- **Bundle `sjl-12-001` retains the copy it checked.**
- The reason: SJL-15 builds against the catalogue in the home. The registry is this family's (governance row P15).

**Decision D32 - the corpus's home and layout.**
- **Home.** Broiler.JS's `conformance/parse/`, with a byte-identical copy in this repository at
  `src/tests/corpus/parse-1/`. The copy's README names the Broiler.JS commit and the manifest's SHA-256.
- **Inputs.** test262 inputs are referenced by path and digest, and every other input is stored.
- **Rules.** No project item names the corpus, so rule N13 is untouched and no rule is revised.
- The reason: shared clause 8 says "in both repositories". It follows the vector corpus's home without minting a pin
  rule.

**Decision D33 - how the population is built.**
- Front matter is read as YAML, and variants follow test262's `INTERPRETING.md`.
- `_FIXTURE` files are module inputs, and JSON fixtures JSON inputs.
- "Every positive parse test" means every test that is not parse-negative.
- **Broiler.JS's parser and compiler test inputs** are captured by a hook in a scratch worktree at the Broiler.JS rule
  commit. The hook is never committed, and the inputs are deduplicated with their provenance.
- **This family's `SliceSourcePrograms` texts** come from their named members.
- The reason: a regular expression misreads block-style flag lists, and a string-literal heuristic misses inputs
  the capture finds.

**Decision D34 - this family's baseline content.**
- **Per input:** the outcome, the ordered (code, line, column) list, and for an accepted input the artifact's
  SHA-256 and length. Never the bytes.
- **Fixed names.** Source names and module keys are fixed by the corpus manifest.
- **The populations** are the sub-populations B1 to B4 and the request-flag sets E0 to E7, as the bundle's rule
  states them. The native form runs on linux-x64 with `x86-64-sysv` only.
- The reason:
  - a module's bytes depend on its key;
  - the accepted inputs' artifacts are too large to retain, and any commit regenerates them;
  - eval code was otherwise outside every identity population.

**Decision D35 - the tools.**
- **This family**: the slice compiler's `--parse-corpus` mode, `eng/compare-parse-corpus.py` and
  `eng/check-frontend-mapping.py`.
- **Broiler.JS**: the bundle tool `docs/evidence/sjl-12-001/tools/parse-corpus`, following its `sjl-0-001` API
  lister, and `scripts/compliance/compare_parse_corpus.py`.
- **Each tool has its control.**
- The reason: no existing tool covers a corpus, and each comparison could otherwise pass vacuously.

**Decision D36 - roadmap section 9's "diagnostics distributions"** are part B's refused-input lists and per-code
tallies at each commit, retained in the bundle and read through its classes S and T. Broiler.JS's part C messages
are its distribution. The reason: section 9 asks for them, and they are figures, so they stay in bundles.

**Decision D37 - validation section 7's mapping control** is carried by this family's part E at SJL-12, not
deferred. The reason: it is cheap, and the mapping exists at SJL-12.

### The rules

**Decision D38 - Broiler.JS's rule has four parts.**
- A: template T2, in UTC, with negative tests.
- B: the architecture test and its witness.
- C: the parse corpus's acceptance.
- D: determinism.

There is no part in a zone with daylight-saving time. The reason: section 9 marks determinism for both
repositories, and shared clause 8 requires the corpus baseline.

**Decision D39 - this family's test262 classes.**
- Class C1 has two kinds named in advance, V1 and V2. Any other C1 row is a finding against this record.
- Class C4 admits (a), (b), (c') and (e).
- There is no class C2, C3 or C5.
- The reason: SJL-12 changes no charge, pin, data or policy, and the only verdicts it may improve are the ones this
  record names.

### Records

**Decision D40 - ledger housekeeping.**
- Broiler.JS's half of SJL-10.6 is recorded in the programme ledger first.
- The ledger's "Today" paragraph is then revised, with a dated note quoting it:
  - SJL-12 is among the milestones in progress;
  - the areas are named without a number;
  - the shared stream's current version is named;
  - bundle `sjl-10-001`'s state is current;
  - the bundles are listed by name, without a count.
- The reason: update rules 1 and 8, and no edit to the ledger collides with the shared stream's preview.7 move.

**Decision D41 - the contract's specifics** are those section "The contract" states:
- the goals, three of them reserved;
- the inherited context;
- the limits: a bounded nesting parameter, a constant tree depth, the diagnostic cap and the error policy;
- the providers;
- proposal flags off by default;
- the `m` suffix as a lexer-extension parameter.

The semantic model's design is filed here. The reason: the milestone's SJL-12.1, and `areas/frontend.md` section 4.

**Decision D42 - the design's stale readings are corrected at SJL-12.1.** Section "The design's readings" lists them.
The reason: the contract must not rest on the readings at `88d8b51d` and `44a214dc`.

## Stage JSB-1 (decision D14)

**JSB-1** is the stage of this profile's backend roadmap that asks for determinism over the wide front end. Its
gate, in `roadmap.backends.md`, asks:
- for every source of the wide and module corpora compiled twice and compared byte for byte, in a check that runs
  where the slice front end's equivalent already runs, the slice compiler's checks lane;
- for the global lexical hoisting order derived from the source;
- for a negative control watched failing and passing after revert;
- for a correction entry saying which half of the determinism claim is covered.

Its State bullet of 2026-09-07 says nothing of it was built. The universal bytecode programme's UBC-3.1 names the
same gate as its own first work package.

**SJL-12.6 item 3 meets that gate, and lands first, in a merge of its own** (bundle `sjl-12-001`'s "ordering
commit").
- **The order.** `programLexicals` and `HoistEval`'s lexical names become source-ordered lists, each with a
  membership set.
- **Two standing rows** join the slice compiler's `--checks`:
  - "the wide lowering is deterministic": the retained wide and module corpora, compiled twice and on two threads;
  - "the wide lowering declares lexical names in source order".
- **Two controls**, each watched failing in a scratch copy and passing after revert:
  - K1 reverses the order, and is caught by the source-order row and by artifact identity against the base;
  - K2 shuffles the order per compilation, and is caught by two runs or two threads.

**What the records say:**
- **The JSC entry SJL-12.6 files corrects `roadmap.backends.md`**: the "Where this document stands" paragraph, which
  says JSB-1 is not started, and JSB-1's State bullet. JSB-1's gate is met by those rows. The profile's ledger judges
  JSB-1 on its own legend; this programme's evidence completes no other plan (the ledger's update rule 3).
- **A dated note on UBC-3.1** in `docs/universal-bytecode.roadmap.md` records that UBC-3 re-runs the rows.

**The reasons:**
- JSB-1's gate asks for a check where the slice equivalent runs, which is `SourceFrontEndChecks`' "the lowering is
  deterministic".
- A merge of its own attributes any byte difference to the ordering alone (bundle class O), before items 1, 2 and 4
  can move bytes.

## The design's readings

Each is corrected in the design beside the reading, which is quoted there (decision D42):
- **`areas/frontend.md` section 2:** the tokenizer's identifier and white-space classes come through
  `JsCompilerUnicode` from Broiler.Unicode over Format's tables since SJL-7.4, not Format's `JsUnicodeLexical`.
- **Section 3, identifiers:** Broiler.JS classifies identifiers with Broiler.Unicode's lexical classes through
  `JSUnicode` since SJL-7.5, not the runtime's categories with hand-written patches.
- **Section 3, the `Runtime` reference:** six types, not "exactly three".
- **Sections 3 and 7, regular-expression literals:**
  - since Broiler.JS #1041, the literal's check also runs the group-name, property-name and Unicode-pattern checks,
    then compiles the pattern with .NET;
  - an invalid literal still falls back to division.
- **Section 9, the deep-nesting row:** decided (decision D27).
- **`areas/static-semantics.md` section 3:** the model's design is filed here and in the home's design document.

The milestone file is corrected likewise. "It references `Broiler.JavaScript.Runtime` for exactly three types"
reads six, and SJL-12.5 item 4's dispositions follow decision D20.

## Decision

1. **The contract** is section "The contract", and the semantic model's design is section "The semantic model's
   design". SJL-13 builds the library against both.
2. **SJL-D-11 and SJL-D-12 are recorded.** SJL-D-12 (i)'s package disposition is recorded at SJL-14.4.
3. **SJL-12 is not an area verdict** (decision D1). SJL-13.1 applies the extraction gate.
4. **This family's preparation, SJL-12.6, has four items:**
   - the binder pass (decision D11);
   - every lowering early error moved into it, with its code (decisions D10 and D12);
   - source-ordered declarations, merged first and alone, which meet JSB-1's gate (decision D14);
   - structured templates, with their listed corrections (decision D15).

   `JsCompiler.Requests` runs through `CompilationStack` (decision D16). The slice path is untouched but for the
   iterative template scan (SJL-D-13 (a); decision D15).
5. **Broiler.JS's preparation, SJL-12.5, has six items**, in the order of decision D26:
   - explicit options (decision D19);
   - a counter per parse;
   - a token per scanner;
   - no `Runtime` reference (decision D20);
   - the `for`-loop pass (decision D22);
   - an architecture test with a witness (decision D25).

   Its corrections are J1 and J2 (decision D21).
6. **No registry revision at SJL-12** (decision D13). The mapping records the split candidates, and SJL-13 mints
   them under JSD-0009.
7. **The catalogue, the mapping and the corpus** are as decisions D30 to D35 state.
8. **The bases.** Bundle `sjl-12-001`'s rules are committed with this record, and each base is taken at the commit
   its rule names (decisions D7 to D9).

## What this rejects, and why

- **An area verdict at SJL-12**: decision D1.
- **Moving the wide parser's own checks into the binder now**: decision D10.
- **Recoding any registry code at SJL-12**: decision D13.
- **A binder that drops multiplicity, iterates a hash set, or changes interning or emission order**: decision D11.
- **The ordering change merged with other items**: decision D14.
- **A provider seam whose default throws**, and **temporaries named from the compilation identifier**: decisions D20
  and D22.
- **SJL-0's base for Broiler.JS's test262 comparison**: decision D7.
- **Moving the Annex B.3.4 block or the regular-expression escape decoding at SJL-12**: decision D23.
- **A `Broiler.ECMAScript.Frontend/` project folder at SJL-12**: decision D5.
- **Deferring the mapping control to SJL-13**: decision D37.

## Settling condition

This record is settled when SJL-12.5's six items and SJL-12.6's four have landed, and every rule of bundle
`sjl-12-001` has been judged in both repositories, whatever each decides. This family's exit clauses 4 to 6 are read
on that bundle.

## Correction entries

None in this profile's plan documents at this record. The JSC entry SJL-12.6 files corrects `roadmap.backends.md`'s
JSB-1 text (decision D14). The programme's milestone, its two area designs, roadmap section 5 and governance row J6
carry dated notes beside the text they correct, each quoting it.

## What this record does not decide

- **The shared library.** SJL-13 writes it, and SJL-13.1 applies the extraction gate to it.
- **The registry revision.** SJL-13 mints the split candidates' numbers under JSD-0009.
- **The disposition of Broiler.JS's `Ast` and `Parser` packages.** SJL-14.4 records it.
- **The semantic model's implementation.** SJL-15 builds it, and SJL-15.4 records SJL-D-13.
- **The shared bounds' values.** SJL-13 derives the tree depth and the nesting bound.
- **Rule N19's revision.** SJL-13's (decision D16).
- **A speed claim of any kind.** SJL-12 measures nothing, and the provider seam's cost is not measured.

## Falsified if

- After SJL-12.6, the lowering still raises a language early error.
- Two compilations of one source differ, in two runs or on two threads.
- A shared code maps to two registry meanings.
- After SJL-12.5, a Broiler.JS parse's tree depends on what the process parsed before it.
