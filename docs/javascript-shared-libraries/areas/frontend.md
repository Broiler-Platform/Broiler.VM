# Area: the shared frontend: tokenizer, parser, syntax tree and diagnostics

**Delivered by:**

- [SJL-12](../milestones/sjl-12-frontend-contract.md): the contract and in-place preparation;
- [SJL-13](../milestones/sjl-13-shared-syntax-and-vm.md): the library and Broiler.VM's wide path;
- [SJL-14](../milestones/sjl-14-broiler-js-syntax.md): Broiler.JS.

**Static semantics** are in [static-semantics.md](static-semantics.md) and delivered by SJL-15.

**Library:** `Broiler.ECMAScript.Frontend`. **Status:** design. Feature statements come from reading
both front ends at Broiler.VM `88d8b51d` and Broiler.JS `44a214dc`.

*(Added 2026-10-19, at SJL-12.1.)* [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md)
re-derived these statements from code at Broiler.VM `046357b` and Broiler.JS `24510b0e`, and records the contract of
section 4 with the specifics its decisions D27 and D41 fix. Each reading that no longer held is corrected beside it,
dated, with the superseded text quoted (its decision D42).

*(Added 2026-10-25, at SJL-12.6's records.)* SJL-12.6 changed Broiler.VM's wide path in four merges, `066abef`,
`3fa4b39`, `daa92eb` and `1de3af6`, the last of them bundle `sjl-12-001`'s after commit. The statements of section 2
and the wide column of section 7 that they changed are corrected beside the reading, dated, with the superseded text
quoted, as read at `1de3af6`. None of it is a verdict: bundle `sjl-12-001` judges SJL-12.6.

---

## 1. Why this area is the largest opportunity and the largest risk

**The opportunity.** The front end is where both engines duplicate the most language logic:

- tokenizing;
- parsing every production of the language;
- cover grammars;
- automatic semicolon insertion;
- template cooking;
- the early errors;
- the declaration analysis behind hoisting.

**The risk.** It is also where the engines' designs differ most:

- Broiler.JS's tree is public, mutable and partly desugared;
- Broiler.VM's is internal and immutable;
- Broiler.VM's conformance runner grades refusals by diagnostic code, and requires byte-identical
  compiler output.

Hence a contract, then preparation in place, then the library under one engine, then the other.

---

## 2. Current state: Broiler.VM

**One tokenizer, two parsers.** `SliceTokenizer` reads the whole input into tokens and never throws.
It records one diagnostic, stops, and appends an end token. Its token records facts that the parser
rules on:

- legacy octal, and `08`/`09`;
- legacy and `\8`/`\9` escapes;
- whether an identifier was escaped;
- line-terminator precedence.

Three things it does not do, and one thing it does:

- **identifiers and whitespace** come from the pinned Unicode tables, through the compiler's
  `JsCompilerUnicode`, which reads Broiler.Unicode's lexical classes over the tables Format embeds *(revised
  2026-10-19, at SJL-12.1, by JSD-0080 decision D42: so since SJL-7.4. The item read "come from the pinned Unicode
  tables, through the Format assembly's `JsUnicodeLexical`;")*;
- **regular-expression literals** are one opaque token, delimited but not validated;
- **template literals** are one opaque token *(noted 2026-10-25, at SJL-12.6 item 4, `8ff3a58` and `964b460`: still
  so in the stream both paths read, and the tokenizer now scans a template in one loop with a stack of its own,
  where its scan called itself once per level; under an internal mode only the wide compiler sets, it also
  tokenizes each substitution with the lexer, into a run of its own behind the stream)*;
- **decorators** (`@`) are refused in the tokenizer itself.

**The slice path** runs `SliceParser`, `SliceStaticSemantics` and the slice lowering.

- **Tree.** Immutable records for the slice subset; everything else is a generic construct node.
- **Parser.** A deliberately loose recursive-descent parser over the whole language.
- **Static semantics.** One stage that never sees the source text and reports every error. It
  computes strictness, var and lexical scoping, collisions, the strict reserved names, and assignment
  validity. **It refuses an unresolvable identifier as an early error, by design**, because the
  slice has no global object. **It already allocates frame slots.**

**The wide path** runs `JsParser` into `JsSyntax`, then the lowering in `JsCompiler`.

- **Tree.** Immutable records with init-only properties; side facts in identity-keyed sets.
- **What is in the tree:**
  - cooked and raw template chunks;
  - exact BigInt values;
  - private names, `#x in obj`, chains, `super`, `import()`, `import.meta`, `new.target`;
  - `using` declarations;
  - class members, including static blocks.
- **What it supports.** The whole modern language **except** decorators, the `accessor` keyword and
  import phases. Some doc comments in `JsParser` and `JsSyntax` that say otherwise are stale.
- **How it parses.** The parser never rewinds; arrow functions are found by bracket scanning; cover
  grammars are resolved by conversion. **Template substitutions are read in place** by the same
  parser, from the runs the tokenizer's structured mode writes, so the nesting depth, the tree depth,
  the parameter context, the labels and the offsets carry into them *(revised 2026-10-25, at SJL-12.6
  item 4, `964b460`, JSD-0080 decision D15: the sentence read "**Template substitutions are
  re-tokenised** by a fresh tokenizer and sub-parser.")*.

**Options and context.**

- `SliceParseOptions`: goal (Script or Module), top-level `await`, nesting depth (bounded),
  `GoalIsStrict`, and the evaluation flags.
- The wide parser's constructor takes forced strictness, the enclosing function depth, and whether
  `yield` and `await` are operators.
- `JsFormat.EvalRequestFlags` carries six evaluation flags. **Only two reach the parser**; the
  binder pass reads the rest for its early errors, and the lowering for emission *(revised
  2026-10-25, at SJL-12.6 item 2, `daa92eb`: the item ended "the lowering consumes the rest")*.

**Limits.**

- A nesting depth: settable within a bound.
- A tree-depth bound: a constant, which **rule N19 forbids the host to raise**.
- Every compilation runs on its own large thread (`CompilationStack`) *(noted 2026-10-25, at
  SJL-12.6 item 4, `2c590e4`, JSD-0080 decision D16: so does the parse `JsCompiler.Requests` makes,
  which ran on its caller's stack before)*.
- Format ceilings: locals, constants, the operand stack, call arguments, BigInt width.
- A slot ceiling in the wide path.
- A depth limit for JSON modules.
- A cap on wide-parser diagnostics.

**Diagnostics.** `SliceSourceDiagnosticCode` is bound to the registry's embedder-seam rows 2001 to
2406 by rules N5 to N9.

- Messages are English prose and are not the contract; **the codes are**.
- The conformance runner grades a refusal by its code.
- Some lowering refusals carry a zero position.
- **Guests see only a generic `SyntaxError`**, because the artifact provider drops the diagnostics.

**Where early errors are checked.** In the slice path, one stage. In the wide path, **the parser,
and then the binder pass**, which runs after the parse and before the lowering:

- the binder pass checks `super` validity, private-name resolution, `arguments` in field
  initialisers, `return` placement, `break`/`continue` targets, regular-expression literal syntax
  (through the matcher), duplicate `__proto__`, cover-grammar leftovers, `const` initialisers and
  module import/export rules, each with the code the lowering gave it, and a unit it refuses is not
  lowered;
- the parser checks the rest;
- the lowering keeps the refusals that are not language early errors: format ceilings, manifest and
  form refusals, module-load policy and the eval scope-depth ceiling (JSD-0080 decision D12).

*(Revised 2026-10-25, at SJL-12.6 item 2, `daa92eb`, JSD-0080 decisions D10 to D12. The paragraph read "In
the wide path, **both the parser and the lowering**:", and its first item began "the lowering checks"; the third
item is new.)*

**Coupling.** The binder pass, `JsBinder`, collects the declarations and the per-function facts,
and `HoistProgram`, `HoistFunction` and `HoistEval` read them and emit. Evaluation scope-map rows are
recorded during emission. *(Revised 2026-10-25, at SJL-12.6 item 1, `3fa4b39`, JSD-0080 decision D11:
the first sentence read "`HoistProgram`, `HoistFunction` and `HoistEval` collect declarations **and**
emit instructions in the same methods.")*

**Determinism.** Required: the same source gives the same bytes. The wide hoisting order is the
source's, and two checks rows compile the wide and module corpora twice and on two threads, and read
the order back from the artifact; they are stage JSB-1's standing check. *(Revised 2026-10-25, at
SJL-12.6 item 3, `066abef`, JSD-0080 decision D14, and
[JSC-319](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-319): the item read "But
the wide hoisting order relies on dictionary insertion order. Backend stage JSB-1, not started, records
this.")*

**Ambient state.** None: rule N12 forbids mutable statics, `[ThreadStatic]`, `AsyncLocal` and
`ThreadLocal` in the compiler assembly.

**Run-time compilation.** `eval`, the `Function` constructor and dynamic `import()` reach the
compiler through a guest-initiated load answered by an artifact provider. The composition roots
supply the provider; ADR 0010 requires the compiler to stay a byte producer.

## 3. Current state: Broiler.JS

**The parser.**

- `FastParser(FastTokenStream)` with `ParseProgram()` and a static `ParseModule`.
- Several productions and backtracking members are public.
- No options object.
- Errors are thrown as `FastParseException` with V8-style English messages.

**The scanner.**

- A pull scanner with one token of lookahead.
- Tokens are a mutable linked list, and line terminators are tokens.
- Templates are split into begin, part and end tokens with cooked text.
- Numbers include BigInt and a non-standard `m` (`Decimal`) suffix.
- Identifiers are classified with Broiler.Unicode's lexical classes, through `Runtime`'s `JSUnicode`
  *(revised 2026-10-19, at SJL-12.1, by JSD-0080 decision D42: so since SJL-7.5. The item read "Identifiers are
  classified with the runtime's Unicode categories plus hand-written patches.")*.
- **Regular-expression literals are validated by the literal's early checks** (group names, property names and,
  under `u` or `v`, the Unicode pattern), **then by compiling them with .NET; an invalid pattern still becomes
  division.** *(Revised 2026-10-19, at SJL-12.1, by JSD-0080 decision D42. The Unicode-pattern check,
  `RegExpUnicodeValidator`'s, was already made at `44a214dc`; the property-name check came with SJL-7.5, Broiler.JS
  #1040, and the group-name check with Broiler.JS #1041, both of 2026-10-17. The item read "**Regular-expression
  literals are validated by compiling them with .NET; an invalid pattern becomes division.**")*

**The tree.**

- About fifty public classes, many with mutable fields and setters: parenthesisation, hoisting
  scopes, Annex B names, inference names, token links.
- A shared static end-of-file token whose `Previous` field every parse writes.
- Positions are `StringSpan` (source, offset, length) from the `Ast` assembly, plus 1-based line and
  column.

**Ambient state.**

- The module goal and top-level-`await` permission are read from `AsyncLocal` state in `Runtime`'s
  `CoreScript`, through `InternalsVisibleTo`.
- Desugaring temporaries are named from a **process-wide static counter**, so the tree depends on
  what was parsed earlier in the process.

**Runtime reference.** The parser uses six `Runtime` types:

- `CoreScript`;
- `CancellableDisposableAction`;
- `RegExpUnicodeValidator`;
- `JSUnicode`;
- `JSUnicodePropertyNames`;
- `RegExpGroupNames`.

*(Revised 2026-10-19, at SJL-12.1, by
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md)
decision D42, from the Parser assembly's metadata at Broiler.JS `24510b0e`. The first three are the three this
section named. `JSUnicode` and `JSUnicodePropertyNames` came with SJL-7.5, Broiler.JS #1040, and `RegExpGroupNames`
with the regular-expression literal's group-name check, Broiler.JS #1041. The sentence read "The parser uses
exactly three `Runtime` types:", and the list ended at `RegExpUnicodeValidator`. Decision D20 disposes of all six at
SJL-12.5.)*

**Desugaring.** `for (let/const …)` is lowered in the parser into blocks with temporaries, copy-back
and an injected `if (!test) break;`.

**Scope analysis.**

- In the parser: `FastScope`. It fills hoisting scopes in dictionary order and does not track
  strictness.
- In the compiler: `FastFunctionScope` and `FreeNameScan`.

**Early errors are spread across:**

- the parser;
- `SyntaxValidation`, which re-tokenises the source to find legacy octal;
- `FastCompiler.EarlyErrors`;
- `Visit*` methods;
- `DirectEvalSupport.Validate`.

**Limits.** None. Deep input survives through stack segmentation and a large compile thread whose
size a static and an environment variable configure.

**The portable compiler.** It parses with `FastParser` on the calling thread, outside any ambient
scope, so always as a script, and **runs no early-error validation**. It applies its admission rules
during emission.

**Broiler.JS's own plan.** Its `dependencies.md` proposes a `FrontEnd/Semantics` boundary, "Shared by
IL and bytecode; references neither back end". It is a hypothesis pending MOD-M2. MOD-M2-4 inventories
the parser's runtime reference, and MOD-M2-5 asks for a priced extraction plan. **None of it has
started.**

## 4. The contract

Recorded at SJL-12.1.

### Input: an immutable parse request

| Field | Content |
|---|---|
| Source | Text, source name, base offset, base line and column (for sub-parses) |
| Goal | `Script`, `Module`, `Eval`, `FormalParameters`, `FunctionBody`. The last two let `CreateDynamicFunction` parse its pieces as the specification requires; neither engine does so today. |
| Switches | Forced strictness; top-level `await` allowed; Annex B on or off; HTML-like comments on or off (script goal only) |
| Inherited evaluation context | `InFunction`, `InMethod`, `InDerivedConstructor`, `InClassFieldInitializer`, `InClassBody`, strictness; the visible private names; the caller's lexical names (Broiler.JS needs them for direct `eval`) |
| Proposal flags | Decorators, `accessor`, import phases. Off by default; an engine admits what its manifest admits. |
| Lexer extensions | A narrow parameter for non-standard literal suffixes. Broiler.JS's `m` is the only one; decision [SJL-D-17](../decisions.md#sjl-d-17) keeps its meaning in Broiler.JS. |
| Limits | Nesting depth (bounded) and tree depth (a constant of the library, preserving rule N19's property); a diagnostic cap; first-error or all-errors |
| Providers | Unicode lexical classes (from the Unicode owner, SJL-7); regular-expression syntax validation (from SJL-11's engine, or the engine's own matcher until then) |

### Output

| Part | Content |
|---|---|
| Tree | Immutable nodes with full spans (offset, length, line, column), and **raw facts**: directive raw text, legacy-octal and escape flags, cooked and raw template chunks, regular-expression pattern and flags, BigInt literal text, parenthesisation, trailing commas. Side facts sit in identity-keyed tables. **No desugaring.** |
| Diagnostics | A stable code, a phase (tokenizer, parser, semantics), a span, a source name, and a message identifier with arguments, in a deterministic order. Text is rendered per engine (decision [SJL-D-11](../decisions.md#sjl-d-11)). |
| Semantic model | See [static-semantics.md](static-semantics.md) |

### Properties

- **Deterministic:** the same request gives the same output, across runs and threads.
- **No ambient state, no mutable statics** (rule SL2, the shape of Broiler.VM's N12).
- **Limits are checked before the stack is.** No unbounded recursion: explicit stacks, or depth
  checks with the large thread supplied by the caller (`CompilationStack` in Broiler.VM).
- **Native AOT compatible** and trimmable; no reflection, no dynamic code.
- **Never a source-to-verified-artifact path.** ADR 0010 forbids such members in Broiler.VM, and the
  frontend produces trees, never artifacts.

## 5. The canonical tree

Chosen by decision [SJL-D-12](../decisions.md#sjl-d-12) (a), taken on 2026-10-11: **a new immutable
tree seeded from Broiler.VM's wide design**, for these reasons:

- it already meets the properties above;
- its immutability and side tables suit both lowerings;
- Broiler.JS's tree would have to drop mutability, parse-time desugaring and ambient state to
  qualify, and its early-error split is what Broiler.VM's JSD-0014 rejects.

Broiler.JS's public `Ast` package becomes an adapter that builds the old tree from the new one,
obsolete and then removed. Broiler.JS's design work is not lost: its feature coverage (`accessor`,
decorator parsing) and its cooked-template tokens inform the new tree.

## 6. What each engine keeps

| Broiler.VM keeps | Broiler.JS keeps |
|---|---|
| Manifest admission and its "unsupported" refusals (code 2104) | Its `for` per-iteration lowering, moved out of the parser into a compiler preparation pass |
| The diagnostic registry and its mapping from shared codes | Its V8-style message rendering |
| Format ceilings (locals, constants, stack, arguments, BigInt width) | Numeric speculation, boxing and deferral |
| Slots, environment records, evaluation scope-map rows | Its expression-tree emission |
| Byte determinism of artifacts, verification | Run-time evaluation support |
| The slice path's deliberate refusal of unresolvable identifiers (as an admission rule) | The `m` literal's `Decimal` meaning |
| `CompilationStack`'s thread | Stack segmentation, if it keeps it |

## 7. Feature support today

| Feature | Broiler.VM slice | Broiler.VM wide | Broiler.JS |
|---|---|---|---|
| Classes, fields, private names, static blocks, accessors | refused (2104) | yes | yes |
| `accessor` keyword | no | no | yes (auto-accessor) |
| Decorators | refused in the tokenizer | refused in the tokenizer | parsed and discarded |
| Async functions, generators, async generators | refused | yes | yes |
| Destructuring, spread, rest, optional chaining, `??=`/`&&=`/`\|\|=`, `**` | refused | yes | yes |
| Templates (cooked and raw; invalid escapes in tagged templates) | opaque, refused | yes (substitutions read in place by the same parser) *(revised 2026-10-25, at SJL-12.6 item 4, `964b460`: the cell read "yes (re-tokenises substitutions)")* | yes (template tokens) |
| Regular-expression literals | opaque, refused | validated in the binder pass by Broiler.VM's matcher *(revised 2026-10-25, at SJL-12.6 item 2, `daa92eb`: the cell read "validated in the lowering by Broiler.VM's matcher")* | early checks of group names, property names and the Unicode pattern, then validated by .NET; an invalid pattern still becomes division *(revised 2026-10-19, at SJL-12.1, by JSD-0080 decision D42: the Unicode-pattern check was already made at `44a214dc`, the property-name check came with SJL-7.5, Broiler.JS #1040, and the group-name check with Broiler.JS #1041. The cell read "validated by .NET; an invalid pattern becomes division")* |
| BigInt, separators | separators validated | exact BigInt value; width ceiling | yes |
| `08`/`09` in strict code | refused | refused | accepted (no check found) |
| `1m` | refused (2003) | refused (2003) | `Decimal` token (non-standard) |
| Modules, `export * as ns`, string export names | scanned, refused | yes | yes |
| Import attributes | (none) | JSON modules; other attributes refused (2405) | the `type` key only |
| `import defer`, `import source` | no | no | dynamic forms only |
| Top-level `await` | refused | yes (option) | yes (ambient) |
| `using`, `await using` | not recognised | yes, with placement rules | yes, with placement rules |
| HTML-like comments | off | script goal only | script goal only (ambient) |
| Annex B.3.3 block functions, labelled functions | refused | yes | yes |
| Unicode escapes in identifiers | pinned Unicode 17 | pinned Unicode 17, escaped keywords refused | Broiler.Unicode's lexical classes through `JSUnicode`, escaped keywords flagged *(revised 2026-10-19, at SJL-12.1, by JSD-0080 decision D42: so since SJL-7.5. The cell read "runtime Unicode, escaped keywords flagged")* |

The shared frontend supports the **union** of what both engines parse today, behind the proposal
flags where the feature is a proposal. Each engine admits what its own manifest admits.

## 8. Migration

1. **SJL-12.** The contract, the catalogue and the corpus. Broiler.JS's ambient state, counter,
   end-of-file race and runtime reference removed; its desugaring moved out of the parser.
   Broiler.VM's binder pass separated; its wide early errors moved into the binder; its ordering made
   deterministic; its templates structured. *(Noted 2026-10-25: Broiler.VM's four items landed in
   `066abef`, `3fa4b39`, `daa92eb` and `1de3af6`, and bundle `sjl-12-001` judges them; the item is kept
   as written.)*
2. **SJL-13.** The library, developed against the corpus and test262's syntax tests. Broiler.VM's
   wide path moves in two steps: first an adapter into `JsSyntax`, which isolates the parser in the
   artifact comparison, then direct consumption. `JsParser` is deleted.
3. **SJL-14.** Broiler.JS moves through a bridge to `AstNode`, side by side with the old parser,
   then to the shared tree. `FastParser`, `FastScanner` and `FastTokenStream` are deleted. The
   portable compiler switches directly and gains its early errors.
4. **SJL-15.** The semantic model replaces both binders. The slice path is rebuilt on the shared
   tree and model, keeping its census counts, slot order and refusals byte for byte (decision
   [SJL-D-13](../decisions.md#sjl-d-13) (a)).

## 9. Risks

| Risk | Mitigation |
|---|---|
| A diagnostic code shifts in Broiler.VM and regrades its conformance results | The mapping of SJL-12.3. A registry revision for every meaning change. Test262 compared per variant, including early-error verdicts. |
| Broiler.JS's parse speed or allocation worsens: Broiler.VM's tokenizer allocates a string per token | Broiler.JS's own measurement under a rule. The shared tokenizer may keep spans instead of strings, which the contract allows. |
| Behaviour that depended on Broiler.JS's parse-time desugaring | The desugaring moves to a compiler pass at SJL-12, with test262 per path compared before the parser changes |
| Deep-nesting policy: Broiler.VM refuses at a bound, Broiler.JS survives through stack segmentation | **Decided at SJL-12.1** (JSD-0080 decision D27). The tree depth is a library constant, derived as JSD-0022 derives Broiler.VM's, against the large thread the caller supplies; the nesting depth is a bounded parameter. In both engines, every input deeper than the bound is refused with the contract's nesting diagnostic. Broiler.JS's inputs it accepts today past the bound become listed refusals at SJL-14, as an implementation limit (class C3), enumerated from the parse corpus's baseline. SJL-13 derives the bound's value, and SJL-12 changes no Broiler.JS acceptance. *(Revised 2026-10-19. The cell read "A library constant, with the large thread supplied by the caller. Broiler.JS's inputs deeper than the bound become a listed refusal, decided at SJL-12.")* |
| The universal bytecode programme changes Broiler.VM's lowering in the same period | Neither programme waits on the other. The frontend's output is the "validated tree" JSD-0036 names. Each cut-over lands on whichever lowering exists then. |
| Public API break for Broiler.JS's `Ast` and `Parser` consumers | Decision `SJL-D-12` (i): adapter packages, obsolete, and then removed in a major version |
