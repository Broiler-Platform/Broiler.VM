<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-15.1's correspondence from code: static semantics, this family's half

**Written:** 2026-11-16, at SJL-15.1, as an input of the area verdict
[JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md)
and of bundle `sjl-15-001`'s rule [`decision-rule-area.md`](decision-rule-area.md), before any code either judges
exists: no semantic model beyond the declarations walk SJL-13.2 began, no tools commit, no cut-over and no deletion. The
rows are lettered by the facts the milestone lists: **SC** scopes and their kinds, **DE** declarations and conflicts,
**AB** Annex B.3.3 aliases, **FF** per-function flags, **RS** reference resolution, **LB** labels and jump targets, **PN**
private names, **MR** module records, **NE** named-evaluation names, **FC** free names and captures, **EE** the early
errors outside the parse, and **SL** the slice path. The letters are this file's own; JSD-0083's correspondence names its
rows T, P, N, D, B and R, and a row of that file is named here with its bundle, as "`sjl-13-001`'s B16".

**This is this family's half.** Every row gives this family's answer, read from its code, and the model surface the
shared library has today. The column "Broiler.JS" is left empty: Broiler.JS's half (its `FastScope`, `FastFunctionScope`,
`FreeNameScan`, `SyntaxValidation`, `FastCompiler.EarlyErrors`, the early-error checks in `Visit*` and
`DirectEvalSupport.Validate`, at a stated revision) is written in Broiler.JS's own change for SJL-15.1, and completes
this file by a dated note that fills that column and gives each row its class across both engines (JSD-0088, decision
D-x2). Until then **no row has a class across both engines**, and the class each row carries below is this family's
answer read against the specification only.

**Read at**
- **Broiler.VM `a86b6f5d59a1dfb8ccc9e86e217ff8556513d623`**, the default branch (Broiler-Platform/Broiler.VM#213), whose
  wide path reads the shared tree since SJL-13's step 2 (`467ad7c3`, Broiler-Platform/Broiler.VM#206) and admits
  `Broiler.ECMAScript.Frontend` `0.1.0-preview.11`.
- **Broiler.JS `6fda6ca44c041cea348dd5302c91ff90a0fe67bc`**, the default branch (Broiler-Platform/Broiler.JS#1119), read
  only, for the model surface: `Broiler.ECMAScript/Broiler.ECMAScript.Frontend/DeclarationsWalk.cs` and
  `DeclarationTables.cs`, which the published `0.1.0-preview.11` carries as this family admits it.
- **No cell was run.** Every cell is a reading of code, and is marked so by this sentence once rather than cell by
  cell; a row whose answer a run must settle says so, and the rule's parts settle it (its part B over the parse corpus,
  its part S over the slice corpus, its part E over the `semantics` vectors).

**Paths.** In Broiler.VM, `VC/` is `src/Broiler.VM.Profile.JavaScript.Compiler/`, and in it `JB` is `JsBinder.cs`, `JBW`
`JsBinder.Walks.cs`, `JBE` `JsBinder.EarlyErrors.cs`, `SSS` `SliceStaticSemantics.cs`, `SP` `SliceParser.cs` and `ST`
`SliceTokenizer.cs`. In Broiler.JS, `FE/` is `Broiler.ECMAScript/Broiler.ECMAScript.Frontend/`, `DT` its
`DeclarationTables.cs` and `DW` its `DeclarationsWalk.cs`. A line number is the line at the revision above; a bare `:n`
is in the file named last in the same cell.

*(Corrected 2026-11-16, at SJL-15.1, before this file merged, after a review checked its citations against
`a86b6f5d`.)* Four citations pointed a few lines off or at a neighbouring member, and are corrected in their cells: row
FF4 read "`separateBody` (`JBW:335-337`)"; row FF5 read "`BodyMayEvaluate` and `ParametersMayEvaluate` (`JBW:332`,
`:339`)", line 339 being a comment; row SL5 read "`SliceConstructCensus.Take` (`VC/SliceConstructCensus.cs:60-`)", line
60 being the class's declaration; and row LB1 read "the binder's own refusal (`JB:269-295`)", lines 269 to 278 being
`AdmitsAnnexB` and `PrivateNames`. No answer, class or step of any row changes.

**The classes of this half.**
- **Specification:** this family's answer is the specification's; the model gives the same fact, and nothing moves.
- **Approximation:** this family's answer is safe but wider than the specification's (an over-approximation the binder
  keeps so that no artifact moved at SJL-12.6, `JB:36-43`); the model's precise fact moves artifact bytes, and the rule's
  part B names the class (class P) before any run.
- **Correction of Broiler.VM:** this family's answer departs from the specification; the model's fact moves the
  outcome, a correction listed by row, handed to SJL-15 by JSD-0083's hand-offs or found here.
- **Engine policy:** a back-end decision this family keeps (slot and record allocation, admission against a manifest,
  scope-map rows, format ceilings), which the model informs and does not decide.
- **No producer:** this family computes no such fact; the model's fact is an adoption for this family (X1).

---

## The model surface today

At Broiler.JS `6fda6ca4` the library's semantic surface is the declarations walk's side tables, nothing more: the parse
result's `Declarations` (`FE/SyntaxParser.cs:127`), a `DeclarationTables` (`DT:180-193`) holding **every scope with its
kind and declarations** (`Scope`, `DT:126-152`; `ScopeKind`, `DT:26-54`: Script, Module, Function, Block, Switch, Catch,
For, StaticBlock), **each declaration's name, kind, binding node and its lexical, block-function and if-clause-function
flags** (`Declaration`, `DT:92-124`; `BindingKind`, `DT:56-90`), and **the program's and every function's strictness and
parameter names** (`FunctionFacts`, `DT:154-178`), each list in source order (`DW:187-203`). The walk reports every early
error of a row the stage table assigns `syntax`, and no `semantics` row (`DW:43`). It keeps label sets to refuse a
duplicate label (`DW:1368-1394`) and no jump target; it records no resolution, no function flag but strictness, no
private-name environment, no module record, no named-evaluation name and no capture.

This family reads, of that surface, the strictness of the program and of each function only: `JsFrontend` copies
`DeclarationTables.Functions`' `IsStrict` into `Parsed.Strictness` (`VC/JsFrontend.cs:140-156`), which the binder reads
(`JB:224-228`, `JBW:2402-2406`). **Its binder computes every other declaration fact again** from the tree
(`JBW:1413` `DeclarationsOf`, `:1505` `CollectVarScope`, `:1708` `ScanAnnexB`), which is the second walk SJL-15.2's
model replaces (the milestone's SJL-15.2 note of 2026-10-26: it "does not start a second one").

---

## The rows

| Row | Fact | Broiler.VM, wide path (`a86b6f5d`) | Broiler.VM, slice path (`a86b6f5d`) | The model surface (`6fda6ca4`) | Broiler.JS | Class (this half) | What SJL-15 does |
|---|---|---|---|---|---|---|---|
| SC1 | Scopes and their kinds | The binder keys its facts by node: each block's, catch block's and `switch`'s declarations (`JB:66-72`, `:247-254`), each hoisting scope's (`JB:456-511`), each `for` head's and `catch` parameter's names (`JB:101-113`); the kinds are implicit in which table holds the node | One `Scope` per program and per block, hoisting or not (`SSS:146-167`, `:786-800`); no function scope (the slice manifest has no function) | `Scope` with `ScopeKind`, every scope in source order (`DT:26-54`, `:126-152`) | (Broiler.JS's half) | Specification | SJL-15.3 reads the model's scopes; the binder's tables go with `JsBinder` |
| SC2 | The `with` scope, the eval scope, a class body's scope, function-body separation | No scope object: `with` is the lowering's dynamic marker (`VC/JsCompiler.Names.cs:393-445`); eval code is a unit of its own (`JB:205-228`); the separate body is a function fact (FF4) | (none: `with`, eval and classes are refused by the manifest, `SSS:599-635`) | No `ScopeKind` for `with`, eval code (read as Script), a class body or a separate var environment (`DT:26-54`) | (Broiler.JS's half) | Specification, where present; the model lacks the kinds `areas/static-semantics.md` section 1 lists | SJL-15.2 adds the kinds the area design lists, or the model's records say why a kind is a flag |
| DE1 | Lexical declarations of a statement list | `DeclarationsOf` (`JBW:1413-1460`): each `let`, `const`, resource, class and function declaration, labelled ones included, in source order with multiplicity, with `Constant` and `IsFunction` (`JB:379-404`) | `DeclareLexicalNames`, `DeclareLexical` (`SSS:361-411`): `let` and `const` declarators only, in source order | `Scope.Declarations` with `IsLexical` (`DT:92-124`) | (Broiler.JS's half) | Specification | The lowering reads the model's declarations; `Scope.Declare`'s allocation stays (`VC/JsCompiler.SupportingTypes.cs:328`) |
| DE2 | Var-scoped declarations of a hoisting scope | `CollectVarScope` (`JBW:1505-1633`): every `var` name down to the next function, in source order with multiplicity; the top-level function declarations through labels (`JB:456-511`) | `HoistVarBindings`, `VarDeclaratorsWithin`, `DeclareVar` (`SSS:255-357`): each `var` name once, the first occurrence's | `Scope.Declarations` with `IsLexical` false, interleaved with the lexical ones in source order (`DT:138-151`) | (Broiler.JS's half) | Specification | The model's list is filtered by `IsLexical`; the order the lowering needs is recovered from it, not recomputed (row SL1 for the slice) |
| DE3 | Declared-name conflicts | The library's since SJL-13's step 1 (`sjl-13-001`'s B28 to B31 and B34, deleted); the binder keeps none | `DuplicateLexicalDeclaration` (`SSS:394-400`), `VarAndLexicalCollision` (`SSS:415-428`), checked by this stage itself | Refused by the walk, rows EE0168, EE0169 and their kin, at the scope's close (`DW:1180-1195`; decision D11 of JSD-0083) | (Broiler.JS's half) | Specification, but for the order and the code of the slice path's refusals, which are this stage's (row SL3) | SJL-15.4: the slice path's checks go, the library's refusal stands; whether the slice corpus's refusals stay byte for byte is part S's, and JSD-0088's decision D-census |
| AB1 | Annex B.3.3 block-function aliases | `ScanAnnexB` (`JBW:1708-1854`), membership by declaration, not by name (`JB:247-267`), none in strict code; blocking sets from parameter names, `arguments` and the body's lexical names (`JBW:323-362`) | Refused: no function in the slice manifest | `Declaration.IsBlockFunction` and `IsIfClauseFunction` (`DT:115-123`); no alias decision: which block function Annex B admits is not a model fact yet | (Broiler.JS's half) | Specification | SJL-15.2 adds the alias decision by declaration; SJL-15.3 reads it, and `ScanAnnexB` goes |
| FF1 | Strictness | Read from the model (`JB:224-228`, `JBW:2402-2406`), the caller's imposed strictness or-ed in | `Validate` decides it from the directive's raw text (`SSS:204-216`) and the goal | `FunctionFacts.IsStrict` (`DT:154-178`) | (Broiler.JS's half) | Specification on the wide path; on the slice path, a correction where a strict directive carries a legacy escape (row SL4, hand-off H25) | SJL-15.4 reads the model's strictness on the slice path |
| FF2 | Simple parameter list; parameter names | `IsSimpleParameterList` (`JBW:2126`); `PatternNames` over the parameters (`JBW:310-316`) | (none: no function) | `FunctionFacts.ParameterNames` (`DT:154-178`); simplicity not a fact | (Broiler.JS's half) | Specification | SJL-15.2 adds simplicity and parameter expressions |
| FF3 | `arguments` needed | `UsesArguments` (`JBW:2159-2195`): not an arrow, no parameter binds it, and its own code or an arrow's in it mentions `arguments` **or `eval`** (`JB:544-556`) | (none) | Not a fact | (Broiler.JS's half) | **Approximation**: any mention of `eval`, a direct call or not, materialises the object | SJL-15.3 reads the model's precise need, as the milestone asks ("precise instead of an over-approximation"); artifacts move by class P |
| FF4 | A separate var environment for the body | `separateBody` (`JBW:334-335`): not simple, parameter expressions, and either a parameter may make a closure (`JBW:2305`) or the body may evaluate | (none) | Not a fact | (Broiler.JS's half) | **Approximation**: the specification separates the environment whenever the list has parameter expressions (FunctionDeclarationInstantiation step 28); this family skips it where no closure or eval can observe it | SJL-15.3 reads the model's fact, and the lowering may keep its skip as engine policy only if no artifact moves; else class P |
| FF5 | Direct-eval contamination | `BodyMayEvaluate` and `ParametersMayEvaluate` (`JB:564`, `:580`; computed at `JBW:332` and `:337`): sloppy, and a **mention** of `eval` (`JBW:2324-2342`, `:2197-2218`) | (none: eval refused) | Not a fact | (Broiler.JS's half) | **Approximation**: a mention that is not a direct call contaminates | SJL-15.3 reads the model's direct-eval facts; class P |
| FF6 | Function kind and the eval code's inherited flags | The binder's `Frame` and `Context` (`JBW:156-243`), from the tree and the eval request's flags (`JBE:84-85`) | (none) | The walk's `Shape` (`DW:105-119`), private; not a public fact | (Broiler.JS's half) | Specification | SJL-15.2 publishes the kind and the inherited context's reading (`areas/static-semantics.md` section 3, "Inherited context") |
| RS1 | Static resolution (hops, slot) | The lowering, on its own scope chain as it emits (`VC/JsCompiler.Names.cs:587` `TryResolve`); slots allocated by `Scope.Declare` | `Resolve` (`SSS:757-784`): innermost lexical, then var, per scope; the resolution table by node identity (`SSS:20-48`) | Not a fact | (Broiler.JS's half) | Engine policy for the slot; specification for which declaration a name means | SJL-15.3 reads the model's resolution, and keeps (hops, slot) as the lowering's (the milestone: "Slot allocation stays in the lowering") |
| RS2 | Dynamic markers (`with`, sloppy direct `eval`) | `Shadowable` (`VC/JsCompiler.Names.cs:393-445`), `EmitDynamicName` (`:469`) | (none) | Not a fact | (Broiler.JS's half) | Specification | SJL-15.3 reads the model's `Dynamic(reason)` |
| RS3 | Unresolvable names | The global, after the scopes and the imports (`VC/JsCompiler.Names.cs:122-160`) | **Refused**: `UnresolvableIdentifier` (`SSS:770-779`), because the slice manifest declares no global object | Not a fact | (Broiler.JS's half) | Engine policy (admission) | SJL-15.4: the refusal becomes a slice admission rule over the model's resolution (`Global`), with the same code, message and place |
| RS4 | Assignment to a `const` | A run-time `TypeError` (`VC/JsCompiler.Names.cs:285-300`) | **Refused at compile time**: `AssignmentToConstant` (`SSS:730-740`), which the specification makes a run-time `TypeError`, not an early error | Not a fact | (Broiler.JS's half) | Engine policy (admission): the slice path refuses what it cannot lower | SJL-15.4: kept as an admission rule over the model's resolution, byte for byte (SJL-D-13 (a)) |
| LB1 | Jump targets | `Target` and the exit stack (`JBW:1364-1389`, `:70-133`), the lowering's reading of a label; one reader, the binder's own refusal (`JB:280-298`); the lowering keeps its own exit stack for emission | `IllegalBreak`, `IllegalContinue` by loop depth (`SSS:546-565`); the slice manifest has no label | Label sets for duplicates only (`DW:1368-1394`); no target | (Broiler.JS's half) | **Correction of Broiler.VM** for `continue` naming an outer label of a loop's label set (`sjl-13-001`'s D30, hand-off H20); otherwise specification | SJL-15.2 adds the jump-target fact; SJL-15.3 reads it, correcting H20 (class F) |
| PN1 | Private-name environments | `PrivateNamesOf` per class (`JBW:1911-1934`), `ResolvePrivate` (`JBE:222-259`), refusing a name no enclosing class declares; **eval code whose caller is in a class is not checked here**: the executor checks it at run time (JSD-0083's decision D16) | (none) | Duplicate private names refused by the walk (`DW:1007-1031`); no environment | (Broiler.JS's half) | Specification for direct code; for eval code a **timing** difference, the check run by the executor | SJL-15.3 passes the visible private names in the inherited context, so eval code's refusal moves to the compile of the eval code; class T (decision D64's second half, JSD-0088) |
| MR1 | Module records: declarations, requests, exports | `ModuleDeclarationsOf` (`JBW:1994-2034`); `CheckModule` (`JBE:503-591`): an export name given twice (2402) and an export of no declared binding (2403); requests from `JsCompiler.Requests` | (none: modules are not the slice path's) | `ScopeKind.Module`, `BindingKind.Import` and `ExportDefault` (`DT:26-90`); no module record | (Broiler.JS's half) | Specification | SJL-15.2 adds the module record; SJL-15.3 reads it |
| MR2 | Top-level `await` reading | `SawTopLevelAwait`, counted with `functionDepth == 0` (`VC/JsAdmissionWalk.cs:99`, `:228`, `:335`), which counts an `await` in an async arrow at a module's top level | (none) | The parse's [Await] parameter only (`DW:43`) | (Broiler.JS's half) | **Correction of Broiler.VM** (`sjl-13-001`'s P32 item 10, hand-off H19) | SJL-15.2's module record carries top-level `await`; SJL-15.3 reads it (class F) |
| NE1 | Named-evaluation names | `CompileNamedValue` (`VC/JsCompiler.Literals.cs:56`) and its callers, the name passed by the lowering; `export default`'s `default` (`JB:150-170`) | (none) | Not a fact | (Broiler.JS's half) | Specification | SJL-15.3 reads the model's name |
| FC1 | Free names and captures | **No producer**: every binding lives in an environment record (`areas/static-semantics.md` section 2) | Exact by construction: no closure, `eval`, `with` or `debugger` in the manifest (`SSS:100-105`) | Not a fact | (Broiler.JS's half) | No producer: the model's captures are an adoption for this family, which this family need not read | SJL-15.3 reads no capture; recorded so that X1 reads it as an adoption |
| EE1 | Early errors the binder keeps (`semantics` rows) | `sjl-13-001`'s B01, B02 (jumps, `JBW:675-702`), B08, B09 (`super`, `JBW:855-895`), B16 (`__proto__` twice, `JBW:1185-1199`), B21 (a `for`-`in`/`of` head not a reference, `JBW:1285-1299`), B22, B24 (`arguments` in an initialiser, `JBE:185-275`), B23 (`JBE:222-259`), B32, B33 (`JBE:503-591`) | `IllegalBreak`, `IllegalContinue`, `InvalidAssignmentTarget` (`SSS:545-565`, `:719-728`) | Not reported (`DW:43`) | (Broiler.JS's half) | Specification, but for LB1's and PN1's rows | SJL-15.2 reports them from the model; SJL-15.3 deletes the binder's checks, each code and place kept or listed by row |
| EE2 | The order and cap of early errors | `InSourceOrder` (`JBE:121-160`): sorted by place, capped after the sort | Every refusal reported, in walk order (`SSS:820-828`) | The library's first refusal, by place (`DW:170-185`; JSD-0083's decision D11) | (Broiler.JS's half) | Engine policy (this family asks the library for its first refusal on the wide path) | Part B's classes name what the first-refusal reading removes, as `sjl-13-001`'s class S7 did |
| SL1 | Slot order on the slice path | Not applicable | Slots by declaration order: every `var` of the program first, once each in source order (`SSS:255-357`), then each scope's lexical names as the scope is entered (`SSS:361-411`, `:786-800`) | The model interleaves var-scoped and lexical declarations in source order (`DT:138-151`) | (Broiler.JS's half) | Engine policy (the slice lowering's allocation) | SJL-15.4 allocates from the model's lists in the base's order, var first; part S judges the order byte for byte |
| SL2 | The slice path's parser and tree | Not applicable | `SliceParser` (`SP:48-`), loose by design: it parses the language and rules on nothing (`SP:20-45`); its own tree (`VC/SliceSyntax.cs`) | The shared tree and parse | (none: Broiler.VM only) | Engine policy (to go) | SJL-15.4 deletes `SliceParser` (decision D60) and, by JSD-0088's decision D-slicetree, `SliceSyntax.cs`'s records, the slice path reading the shared tree |
| SL3 | The slice path's refusals | Not applicable | `SliceStaticSemantics`' codes (`SSS:386-440`, `:545-565`, `:599-650`, `:719-779`), each reported, every one, in walk order | The library's first refusal under its own codes | (none) | Engine policy for admission (RS3, RS4, the manifest); specification for the rest | Part S judges the refusals byte for byte (code, message, place, order) as clause 5 asks; JSD-0088's decision D-census puts to the owner what that means where the library refuses first |
| SL4 | The slice tokenizer's defects | Not applicable | `ST`'s .NET `HexNumber` parses (`ST:956`, `:972`, `:1501`) admit white space and `_` in an escape's digits (`sjl-13-001`'s T13, hand-off H23); a regular-expression body ending in a backslash at the end of the text throws (`sjl-13-001`'s T33, H25); a strict directive with `\8` or `\08` is kept with no early error (EE0004, H25) | The library refuses each | (none) | **Correction of Broiler.VM** (H23, H25) | SJL-15.4 parses with the library, which corrects each; part S lists the inputs; decision D-census |
| SL5 | The construct census | Not applicable | `SliceConstructCensus.Take` (`VC/SliceConstructCensus.cs:90`): files read, parsed, compiled and faulted, occurrences and files per construct kind, and parse failures by the first slice diagnostic's code | Not applicable | (none) | Engine policy (a tool of this family) | SJL-15.4 takes the census over the shared parse; part S judges its output byte for byte; a file the library refuses that `SliceParser` parsed would move it (decision D-census) |

---

## What the rows find against the plan and the records

- **F1. The model surface is the declarations walk alone.** Of the facts the milestone lists, only scopes, declarations
  and strictness have a producer in the library today (rows SC1, DE1, DE2, FF1). The rest are SJL-15.2's. Nothing in this
  family's code can read them before then.
- **F2. Three of the binder's facts are approximations the milestone asks SJL-15 to make precise** (FF3, FF4, FF5). Their
  precise facts move artifact bytes. Bundle `sjl-13-002`'s rule admitted no byte difference; this bundle's part B cannot
  be "byte-identical" for them, and the milestone's clause 6 ("Artifacts are byte-identical, or every difference is
  classified") admits a class named before any run, which the rule's part B gives (class P).
- **F3. The slice path refuses at compile time two things the specification raises at run time or not at all** (RS3,
  RS4). They are the slice manifest's admission, which SJL-D-13 (a) keeps byte for byte; the model's resolution is the
  fact the admission rule reads.
- **F4. The slice path's acceptance of strict `\8` and `\08`, and its tokenizer's defects, are corrections** handed to
  SJL-15.4 (H23, H25), and parsing with the library makes them. **SJL-D-13 (a) keeps "its census counts, its slot order
  and its deliberate refusal of unresolvable identifiers, byte for byte over the slice corpus"**, and clause 5 asks that
  they be "byte-identical before and after". An input of the slice corpus that reaches H23 or H25 cannot be both corrected
  and byte-identical. JSD-0088 puts the reading to the owner (decision D-census), because SJL-D-13 (a) was the owner's
  own choice.
- **F5. The census's parse half is `SliceParser`'s answer** (SL5): a file `SliceParser` parsed and the library refuses at
  the parse (an early error of a `syntax` row, or H25's escape) moves the census's parsed and failure counts. Part S
  judges the census byte for byte; F4's question covers it.
- *(Noted 2026-11-16, later, on the owner's answer to JSD-0088's decision D-census.)* F4 read "JSD-0088 puts the
  reading to the owner (decision D-census)", and rows SL3, SL4, SL5 and DE3 name the decision in their last cells. On
  2026-11-16 the owner chose "Admit listed rows": "A dated revision now, before any evidence, admits those three classes,
  each listed by row. Every other byte difference still fails part S." SJL-15's clause 5 is revised before any evidence,
  and this bundle's rule revision of the same day,
  [`decision-rule-area-revised-2026-11-16b.md`](decision-rule-area-revised-2026-11-16b.md), admits at part S the
  corrections of row SL4 (class H), the refusals of rows SL3, DE3 and EE1 that the library makes in place of the slice
  path's own (class R) and the census lines of row SL5 that the library's parse moves (class C), each listed by row. No
  row's class changes, and every cell above stands as read.
- **F6. Eval code's undeclared private name is the executor's check** (PN1), by JSD-0083's decision D16, which kept the
  visible private names out of the request at SJL-13. Passing them moves the refusal earlier: decision D64's second half,
  class T, applies to this family for this row.
- **F7. The model has no kind for a `with` scope, eval code's scope, a class body or a separate var environment**
  (SC2), which `areas/static-semantics.md` section 1 lists among the shared facts. SJL-15.2's records say how each is
  carried.

## Hand-offs this file reads

From JSD-0083's table: H18 (the dynamic function's goals; recommended there for "SJL-15's Broiler.VM window, which re-pins
the frontend"), H19 (MR2), H20 (LB1), H23 and H25 (SL4). JSD-0088 routes each.
