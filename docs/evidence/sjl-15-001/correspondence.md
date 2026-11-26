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

*(Noted 2026-11-17, at Broiler.JS's SJL-15.1, by JSD-0088's decision D-x2.)* The column "Broiler.JS" read "(Broiler.JS's half)" in every row from SC1 to EE2 and in SL1, and this file said "Until then **no row has a class across both engines**, and the class each row carries below is this family's answer read against the specification only." Broiler.JS's half is filed: its column, read from code at Broiler.JS `6fda6ca4`, with the probes of both engines that settle the rows a reading cannot, is Broiler.JS's bundle `sjl-15-001`'s `correspondence-broiler-js.md`, merged in Broiler.JS as `e4e45d38` (observed). Each cell of the column is filled from it, shortened, its paths `C/` for `Broiler.JS/Broiler.JavaScript.Compiler/` and `P/` for `Broiler.JS/Broiler.JavaScript.Parser/`, its probe ids those of that bundle's `probes/answers.tsv`, and each ends with the row's class across both engines, in the classes of bundle `sjl-13-001`'s correspondence; that file's rows and findings J1 to J7 are the full text. The column "Class (this half)" is this family's, as read, and no cell of this family's columns changes. SL1's cell, which this file left to Broiler.JS, says that Broiler.JS has no slice path; rows SL2 to SL5 keep "(none: Broiler.VM only)" and "(none)", each followed by the row's class across both engines as Broiler.JS's column gives it, so every row carries one.

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
| SC1 | Scopes and their kinds | The binder keys its facts by node: each block's, catch block's and `switch`'s declarations (`JB:66-72`, `:247-254`), each hoisting scope's (`JB:456-511`), each `for` head's and `catch` parameter's names (`JB:101-113`); the kinds are implicit in which table holds the node | One `Scope` per program and per block, hoisting or not (`SSS:146-167`, `:786-800`); no function scope (the slice manifest has no function) | `Scope` with `ScopeKind`, every scope in source order (`DT:26-54`, `:126-152`) | The parser's `FastScopeItem` stack, kinds `Block` (program, block, catch, switch), `ForStatement` and `FunctionExpression` (`P/FastParser.cs:46`, `P/FastScopeItem.cs:7`); the compiler's `FastFunctionScope` per function and block (`C/Scope/FastFunctionScope.cs:132`, `:630`); **across both engines:** Same | Specification | SJL-15.3 reads the model's scopes; the binder's tables go with `JsBinder` |
| SC2 | The `with` scope, the eval scope, a class body's scope, function-body separation | No scope object: `with` is the lowering's dynamic marker (`VC/JsCompiler.Names.cs:393-445`); eval code is a unit of its own (`JB:205-228`); the separate body is a function fact (FF4) | (none: `with`, eval and classes are refused by the manifest, `SSS:599-635`) | No `ScopeKind` for `with`, eval code (read as Script), a class body or a separate var environment (`DT:26-54`) | `with`: the compiler's `withBoundaries` (`C/FastCompiler.cs:32`); eval code: a program compiled with its caller's context (`C/FastCompiler.cs:134-135`); a separate parameter environment only where a sloppy function holds a direct eval (`C/Declarations/FastCompiler.CreateFunction.cs:327-338`); **across both engines:** Same where present, but for FF4; the model lacks the kinds (F7) | Specification, where present; the model lacks the kinds `areas/static-semantics.md` section 1 lists | SJL-15.2 adds the kinds the area design lists, or the model's records say why a kind is a flag |
| DE1 | Lexical declarations of a statement list | `DeclarationsOf` (`JBW:1413-1460`): each `let`, `const`, resource, class and function declaration, labelled ones included, in source order with multiplicity, with `Constant` and `IsFunction` (`JB:379-404`) | `DeclareLexicalNames`, `DeclareLexical` (`SSS:361-411`): `let` and `const` declarators only, in source order | `Scope.Declarations` with `IsLexical` (`DT:92-124`) | The parser's `FastScopeItem.AddVariable` by kind (`P/FastScopeItem.cs:94-235`), the block's `HoistingScope` (`P/FastParser.Block.cs:114`), from `GetVariables` (`P/FastScopeItem.cs:237`); the compiler's `CreateVariable` (`C/Scope/FastFunctionScope.cs:906`); **across both engines:** Same | Specification | The lowering reads the model's declarations; `Scope.Declare`'s allocation stays (`VC/JsCompiler.SupportingTypes.cs:328`) |
| DE2 | Var-scoped declarations of a hoisting scope | `CollectVarScope` (`JBW:1505-1633`): every `var` name down to the next function, in source order with multiplicity; the top-level function declarations through labels (`JB:456-511`) | `HoistVarBindings`, `VarDeclaratorsWithin`, `DeclareVar` (`SSS:255-357`): each `var` name once, the first occurrence's | `Scope.Declarations` with `IsLexical` false, interleaved with the lexical ones in source order (`DT:138-151`) | `AddVariable` hoists each `var` to the nearest function or program scope, once, marking every scope it passes (`P/FastScopeItem.cs:158-231`); **across both engines:** Same | Specification | The model's list is filtered by `IsLexical`; the order the lowering needs is recovered from it, not recomputed (row SL1 for the slice) |
| DE3 | Declared-name conflicts | The library's since SJL-13's step 1 (`sjl-13-001`'s B28 to B31 and B34, deleted); the binder keeps none | `DuplicateLexicalDeclaration` (`SSS:394-400`), `VarAndLexicalCollision` (`SSS:415-428`), checked by this stage itself | Refused by the walk, rows EE0168, EE0169 and their kin, at the scope's close (`DW:1180-1195`; decision D11 of JSD-0083) | The parser's `FastParseException`s (`P/FastScopeItem.cs:121`, `:140`, `:154`, `:188`); a direct eval's `var` against the caller's lexical names given to `SyntaxValidation.ContainsDirectEvalVarConflict` (`C/SyntaxValidation.cs:454`); EvalDeclarationInstantiation's at run time; probe V046, eval code, accepts a `var` of eval code against the calling eval code's `let` (row EA0007's step); no vector holds it yet; **across both engines:** Same, but a correction of Broiler.JS for V046 (C1's row EA0007, in eval code) | Specification, but for the order and the code of the slice path's refusals, which are this stage's (row SL3) | SJL-15.4: the slice path's checks go, the library's refusal stands; whether the slice corpus's refusals stay byte for byte is part S's, and JSD-0088's decision D-census |
| AB1 | Annex B.3.3 block-function aliases | `ScanAnnexB` (`JBW:1708-1854`), membership by declaration, not by name (`JB:247-267`), none in strict code; blocking sets from parameter names, `arguments` and the body's lexical names (`JBW:323-362`) | Refused: no function in the slice manifest | `Declaration.IsBlockFunction` and `IsIfClauseFunction` (`DT:115-123`); no alias decision: which block function Annex B admits is not a model fact yet | The parser's `RegisterAnnexBFunctionHoisting` (`P/FastParser.Function.cs:166-235`), by name, blocked by a parameter, by `arguments` in a function that is no arrow, and by an intermediate lexical name; applied in sloppy code only; probes V023 to V030, in eval code, the specification's; no vector holds V029's or V030's eval-code input yet; **across both engines:** Correction of Broiler.VM for V028 (a block function named `arguments`, the pinned edition against test262 at `ccaac100`, which the Script vector `semantics/annex-b/ab1/0005` also shows), and in eval code for V029 (an intermediate `let`) and V030 (strict eval code); same otherwise; the whole fact implementation-defined (Annex B), on in both engines' requests | Specification | SJL-15.2 adds the alias decision by declaration; SJL-15.3 reads it, and `ScanAnnexB` goes |
| FF1 | Strictness | Read from the model (`JB:224-228`, `JBW:2402-2406`), the caller's imposed strictness or-ed in | `Validate` decides it from the directive's raw text (`SSS:204-216`) and the goal | `FunctionFacts.IsStrict` (`DT:154-178`) | From the directive prologue (`C/FastCompiler.cs:216`, `:432`; `C/Declarations/FastCompiler.CreateFunction.cs:250`); **across both engines:** Same | Specification on the wide path; on the slice path, a correction where a strict directive carries a legacy escape (row SL4, hand-off H25) | SJL-15.4 reads the model's strictness on the slice path |
| FF2 | Simple parameter list; parameter names | `IsSimpleParameterList` (`JBW:2126`); `PatternNames` over the parameters (`JBW:310-316`) | (none: no function) | `FunctionFacts.ParameterNames` (`DT:154-178`); simplicity not a fact | `HasSimpleParameterList`, `CollectParameterNames` (`C/FastCompiler.EarlyErrors.cs:34`, `:62`); **across both engines:** Same | Specification | SJL-15.2 adds simplicity and parameter expressions |
| FF3 | `arguments` needed | `UsesArguments` (`JBW:2159-2195`): not an arrow, no parameter binds it, and its own code or an arrow's in it mentions `arguments` **or `eval`** (`JB:544-556`) | (none) | Not a fact | By reference: `MaterializeArgumentsBinding` (`C/Expressions/FastCompiler.VisitIdentifier.cs:483`) on a reference to `arguments` or a direct eval call (`C/Declarations/FastCompiler.CreateFunction.cs:400-403`); probes V001 to V008 the specification's in both engines; **across both engines:** Same in outcome; each engine's materialisation its policy | **Approximation**: any mention of `eval`, a direct call or not, materialises the object | SJL-15.3 reads the model's precise need, as the milestone asks ("precise instead of an over-approximation"); artifacts move by class P |
| FF4 | A separate var environment for the body | `separateBody` (`JBW:334-335`): not simple, parameter expressions, and either a parameter may make a closure (`JBW:2305`) or the body may evaluate | (none) | Not a fact | A separate parameter environment only for a direct eval, not for every parameter expression; probe V010 answers `"2,2"` where the specification gives `"2,1"`; **across both engines:** Correction of Broiler.JS (V010); same otherwise | **Approximation**: the specification separates the environment whenever the list has parameter expressions (FunctionDeclarationInstantiation step 28); this family skips it where no closure or eval can observe it | SJL-15.3 reads the model's fact, and the lowering may keep its skip as engine policy only if no artifact moves; else class P |
| FF5 | Direct-eval contamination | `BodyMayEvaluate` and `ParametersMayEvaluate` (`JB:564`, `:580`; computed at `JBW:332` and `:337`): sloppy, and a **mention** of `eval` (`JBW:2324-2342`, `:2197-2218`) | (none: eval refused) | Not a fact | A call whose callee is the identifier `eval` (`C/Expressions/FastCompiler.VisitIdentifier.cs:162`, `:174`), not a mention; probe V015, an arrow's direct eval, answers `2` where the specification gives `1`; **across both engines:** Correction of Broiler.JS (V015); same otherwise; Broiler.VM's mention its policy | **Approximation**: a mention that is not a direct call contaminates | SJL-15.3 reads the model's direct-eval facts; class P |
| FF6 | Function kind and the eval code's inherited flags | The binder's `Frame` and `Context` (`JBW:156-243`), from the tree and the eval request's flags (`JBE:84-85`) | (none) | The walk's `Shape` (`DW:105-119`), private; not a public fact | `DirectEvalSupport.Execute`'s parameters (`C/DirectEvalSupport.cs:103`) and the compile's context (`C/FastCompiler.cs:134-135`); probes V049 to V054 the specification's in both engines; **across both engines:** Same | Specification | SJL-15.2 publishes the kind and the inherited context's reading (`areas/static-semantics.md` section 3, "Inherited context") |
| RS1 | Static resolution (hops, slot) | The lowering, on its own scope chain as it emits (`VC/JsCompiler.Names.cs:587` `TryResolve`); slots allocated by `Scope.Declare` | `Resolve` (`SSS:757-784`): innermost lexical, then var, per scope; the resolution table by node identity (`SSS:20-48`) | Not a fact | `FastFunctionScope.GetVariable` to the binding's storage (`C/Scope/FastFunctionScope.cs:983`); a free name a run-time global lookup; **across both engines:** Engine policy for the storage; same for which declaration | Engine policy for the slot; specification for which declaration a name means | SJL-15.3 reads the model's resolution, and keeps (hops, slot) as the lowering's (the milestone: "Slot allocation stays in the lowering") |
| RS2 | Dynamic markers (`with`, sloppy direct `eval`) | `Shadowable` (`VC/JsCompiler.Names.cs:393-445`), `EmitDynamicName` (`:469`) | (none) | Not a fact | `withBoundaries` and the eval-shadow bindings (`C/Scope/FastFunctionScope.cs:1048`); probes V018, V019; **across both engines:** Same | Specification | SJL-15.3 reads the model's `Dynamic(reason)` |
| RS3 | Unresolvable names | The global, after the scopes and the imports (`VC/JsCompiler.Names.cs:122-160`) | **Refused**: `UnresolvableIdentifier` (`SSS:770-779`), because the slice manifest declares no global object | Not a fact | A run-time `ReferenceError`, `undefined` under `typeof`; probes V020, V021; **across both engines:** Same on the wide path; the slice refusal Broiler.VM's policy | Engine policy (admission) | SJL-15.4: the refusal becomes a slice admission rule over the model's resolution (`Global`), with the same code, message and place |
| RS4 | Assignment to a `const` | A run-time `TypeError` (`VC/JsCompiler.Names.cs:285-300`) | **Refused at compile time**: `AssignmentToConstant` (`SSS:730-740`), which the specification makes a run-time `TypeError`, not an early error | Not a fact | A run-time `TypeError`; probe V022; **across both engines:** Same on the wide path; the slice refusal Broiler.VM's policy | Engine policy (admission): the slice path refuses what it cannot lower | SJL-15.4: kept as an admission rule over the model's resolution, byte for byte (SJL-D-13 (a)) |
| LB1 | Jump targets | `Target` and the exit stack (`JBW:1364-1389`, `:70-133`), the lowering's reading of a label; one reader, the binder's own refusal (`JB:280-298`); the lowering keeps its own exit stack for emission | `IllegalBreak`, `IllegalContinue` by loop depth (`SSS:546-565`); the slice manifest has no label | Label sets for duplicates only (`DW:1368-1394`); no target | `SyntaxValidation`'s `ControlFlowValidator` (`C/SyntaxValidation.cs:491-700`), `continue` to any label of a label set since #1075; probes V031 to V033 the specification's, Broiler.VM refusing V031 and V032; **across both engines:** Correction of Broiler.VM (H20); same otherwise | **Correction of Broiler.VM** for `continue` naming an outer label of a loop's label set (`sjl-13-001`'s D30, hand-off H20); otherwise specification | SJL-15.2 adds the jump-target fact; SJL-15.3 reads it, correcting H20 (class F) |
| PN1 | Private-name environments | `PrivateNamesOf` per class (`JBW:1911-1934`), `ResolvePrivate` (`JBE:222-259`), refusing a name no enclosing class declares; **eval code whose caller is in a class is not checked here**: the executor checks it at run time (JSD-0083's decision D16) | (none) | Duplicate private names refused by the walk (`DW:1007-1031`); no environment | `StrictModeValidator` (`C/SyntaxValidation.cs:1217`, `:1253`); eval code's visible private names passed to its validation (`C/DirectEvalSupport.cs:742`), refused before it runs; probes V034 to V038 the specification's in both engines; **across both engines:** Same in every outcome the probes reach; F6's timing not shown by them | Specification for direct code; for eval code a **timing** difference, the check run by the executor | SJL-15.3 passes the visible private names in the inherited context, so eval code's refusal moves to the compile of the eval code; class T (decision D64's second half, JSD-0088) |
| MR1 | Module records: declarations, requests, exports | `ModuleDeclarationsOf` (`JBW:1994-2034`); `CheckModule` (`JBE:503-591`): an export name given twice (2402) and an export of no declared binding (2403); requests from `JsCompiler.Requests` | (none: modules are not the slice path's) | `ScopeKind.Module`, `BindingKind.Import` and `ExportDefault` (`DT:26-90`); no module record | `SyntaxValidation`'s module checks (`C/SyntaxValidation.cs:75`, `:124`, `:345`) and the lowering's export check (`C/Statements/FastCompiler.Module.cs:147`); not run; **across both engines:** Same, by code | Specification | SJL-15.2 adds the module record; SJL-15.3 reads it |
| MR2 | Top-level `await` reading | `SawTopLevelAwait`, counted with `functionDepth == 0` (`VC/JsAdmissionWalk.cs:99`, `:228`, `:335`), which counts an `await` in an async arrow at a module's top level | (none) | The parse's [Await] parameter only (`DW:43`) | The parser's async top level: an `await` at function depth zero sets the program's `IsAsync` (`P/FastParser.SingleExpression.cs:445-446`, `P/FastParser.Program.cs:13`), refused in a script without the permission (`C/SyntaxValidation.cs:30-31`); an `await` in an async arrow is the arrow's, its body parsed with the arrow's own async flag (`P/FastParser.Expression.cs:199`, `:213`; `P/FastParser.SingleExpression.cs:386-402`); not run; **across both engines:** Correction of Broiler.VM (H19); Broiler.JS the specification's, by code | **Correction of Broiler.VM** (`sjl-13-001`'s P32 item 10, hand-off H19) | SJL-15.2's module record carries top-level `await`; SJL-15.3 reads it (class F) |
| NE1 | Named-evaluation names | `CompileNamedValue` (`VC/JsCompiler.Literals.cs:56`) and its callers, the name passed by the lowering; `export default`'s `default` (`JB:150-170`) | (none) | Not a fact | The lowering names a binding's anonymous function (`C/Statements/FastCompiler.VisitVariableDeclaration.cs:60-80`, `C/Declarations/FastCompiler.CreateClass.cs:241`, `:693`); probes V039, V041 and V042, in indirect-eval code, give no name; as Scripts and in a direct eval's code every name is given; **across both engines:** Correction of Broiler.JS in indirect-eval code (V039, V041, V042); same otherwise | Specification | SJL-15.3 reads the model's name |
| FC1 | Free names and captures | **No producer**: every binding lives in an environment record (`areas/static-semantics.md` section 2) | Exact by construction: no closure, `eval`, `with` or `debugger` in the manifest (`SSS:100-105`) | Not a fact | `FreeNameScan` (`C/Declarations/FreeNameScan.cs:48`), the only producer of either engine; **across both engines:** No producer in Broiler.VM: an extraction from Broiler.JS, an adoption for Broiler.VM | No producer: the model's captures are an adoption for this family, which this family need not read | SJL-15.3 reads no capture; recorded so that X1 reads it as an adoption |
| EE1 | Early errors the binder keeps (`semantics` rows) | `sjl-13-001`'s B01, B02 (jumps, `JBW:675-702`), B08, B09 (`super`, `JBW:855-895`), B16 (`__proto__` twice, `JBW:1185-1199`), B21 (a `for`-`in`/`of` head not a reference, `JBW:1285-1299`), B22, B24 (`arguments` in an initialiser, `JBE:185-275`), B23 (`JBE:222-259`), B32, B33 (`JBE:503-591`) | `IllegalBreak`, `IllegalContinue`, `InvalidAssignmentTarget` (`SSS:545-565`, `:719-728`) | Not reported (`DW:43`) | `SyntaxValidation.ValidateProgram` (`C/SyntaxValidation.cs:21`), `FastCompiler.EarlyErrors`, the checks in `Visit*` (`C/Expressions/FastCompiler.VisitCallExpression.cs:397`, `C/Expressions/FastCompiler.VisitMemberExpression.cs:25`, `C/Expressions/FastCompiler.VisitIdentifier.cs:344`) and `DirectEvalSupport`'s; the timing probes, eval code, find many super and arguments rows refused in none of their three forms (the `Visit*` checks skip direct-eval code, which an indirect eval's code also is, and `DirectEvalSupport`'s validator reaches no function body but an arrow's and no parameter's initialiser), and super-call rows refused with an engine `Error`, a hang or a run-time `TypeError`; as Scripts every one is refused before any code runs but `arguments` in a static block and `this` as a for-in target; **across both engines:** Correction of Broiler.JS for each row of Broiler.JS's bundle `sjl-15-001`'s `corrections/corrections.tsv`, and for each row of its `corrections/class-t.tsv` that a probe or a vector shows differing; the other class-T rows, listed by site before any run (Broiler.JS's decision D-js-timing), show no difference and are the same in every outcome observed; same otherwise, but for LB1's and PN1's rows | Specification, but for LB1's and PN1's rows | SJL-15.2 reports them from the model; SJL-15.3 deletes the binder's checks, each code and place kept or listed by row |
| EE2 | The order and cap of early errors | `InSourceOrder` (`JBE:121-160`): sorted by place, capped after the sort | Every refusal reported, in walk order (`SSS:820-828`) | The library's first refusal, by place (`DW:170-185`; JSD-0083's decision D11) | The first refusal: the parser's, then `ValidateProgram`'s validators in a fixed order (`C/SyntaxValidation.cs:50-51`), then the lowering's; **across both engines:** Engine policy (both ask the library for its first refusal); the first refusal can move (class M of Broiler.JS's rule) | Engine policy (this family asks the library for its first refusal on the wide path) | Part B's classes name what the first-refusal reading removes, as `sjl-13-001`'s class S7 did |
| SL1 | Slot order on the slice path | Not applicable | Slots by declaration order: every `var` of the program first, once each in source order (`SSS:255-357`), then each scope's lexical names as the scope is entered (`SSS:361-411`, `:786-800`) | The model interleaves var-scoped and lexical declarations in source order (`DT:138-151`) | None: Broiler.JS has no slice path; **across both engines:** Engine policy (this family's slice lowering's allocation), with no counterpart in Broiler.JS | Engine policy (the slice lowering's allocation) | SJL-15.4 allocates from the model's lists in the base's order, var first; part S judges the order byte for byte |
| SL2 | The slice path's parser and tree | Not applicable | `SliceParser` (`SP:48-`), loose by design: it parses the language and rules on nothing (`SP:20-45`); its own tree (`VC/SliceSyntax.cs`) | The shared tree and parse | (none: Broiler.VM only); **across both engines:** Engine policy (Broiler.VM's, to go) | Engine policy (to go) | SJL-15.4 deletes `SliceParser` (decision D60) and, by JSD-0088's decision D-slicetree, `SliceSyntax.cs`'s records, the slice path reading the shared tree |
| SL3 | The slice path's refusals | Not applicable | `SliceStaticSemantics`' codes (`SSS:386-440`, `:545-565`, `:599-650`, `:719-779`), each reported, every one, in walk order | The library's first refusal under its own codes | (none); **across both engines:** Engine policy (Broiler.VM's admission) | Engine policy for admission (RS3, RS4, the manifest); specification for the rest | Part S judges the refusals byte for byte (code, message, place, order) as clause 5 asks; JSD-0088's decision D-census puts to the owner what that means where the library refuses first |
| SL4 | The slice tokenizer's defects | Not applicable | `ST`'s .NET `HexNumber` parses (`ST:956`, `:972`, `:1501`) admit white space and `_` in an escape's digits (`sjl-13-001`'s T13, hand-off H23); a regular-expression body ending in a backslash at the end of the text throws (`sjl-13-001`'s T33, H25); a strict directive with `\8` or `\08` is kept with no early error (EE0004, H25) | The library refuses each | (none); **across both engines:** Correction of Broiler.VM (H23, H25) | **Correction of Broiler.VM** (H23, H25) | SJL-15.4 parses with the library, which corrects each; part S lists the inputs; decision D-census |
| SL5 | The construct census | Not applicable | `SliceConstructCensus.Take` (`VC/SliceConstructCensus.cs:90`): files read, parsed, compiled and faulted, occurrences and files per construct kind, and parse failures by the first slice diagnostic's code | Not applicable | (none); **across both engines:** Engine policy (a tool of Broiler.VM's) | Engine policy (a tool of this family) | SJL-15.4 takes the census over the shared parse; part S judges its output byte for byte; a file the library refuses that `SliceParser` parsed would move it (decision D-census) |

*(Noted 2026-11-24, later, at Broiler.JS's corpus revision 44, observed; every cell above is left as written.)* Row
AB1's column "Broiler.JS" reads "no vector holds V029's or V030's eval-code input yet", and row DE3's, of probe V046, "no
vector holds it yet". Broiler-Platform/Broiler.JS#1131, merged as `8b1fa311da6af71baac6378e8b4f8597a99c1312`, adds
corpus revision 44, which holds them as `DirectEval` vectors, "as eval code a direct eval at a Script's top level
evaluates, whose variable and lexical environments are those of the probes' indirect eval" (its
`conformance/vectors/REVISIONS.md`, row 44): row AB1's `semantics/annex-b/ab1/0012` (V029) and
`semantics/annex-b/ab1/0013` (V030), and row DE3's `semantics/scopes/ea0007/0005` (V046). Both engines' runs on them
are classified there. Broiler.JS differs on V046's input, which `scripts/conformance/semantics-divergences.txt`
classifies under row DE3, "class C1 of its bundle sjl-15-001's rule (corrections.tsv, row EA0007)". Broiler.VM differs
on none of them, so these vectors do not show the answer row AB1 records for Broiler.VM on probes V029 and V030, "in
eval code for V029 (an intermediate `let`) and V030 (strict eval code)": those probes ran as an indirect eval's code,
and the vectors are `DirectEval`. Nothing is classified for them, no row's class changes, and row AB1 stays, across
both engines, a correction of Broiler.VM; a later corpus revision could add the indirect form. These runs are
Broiler.JS's, retained in its bundle `sjl-15-001`'s `engine/`, and are no run of this bundle's rule.

*(Noted 2026-11-25, later, at Broiler.JS's SJL-15.2 stage 2, observed; every cell above is left as written.)* Row EE1's
column "Broiler.VM, wide path (`a86b6f5d`)" names "B08, B09 (`super`, `JBW:855-895`)" and "B22, B24 (`arguments` in an
initialiser, `JBE:185-275`)", and its last cell reads "SJL-15.2 reports them from the model; SJL-15.3 deletes the
binder's checks, each code and place kept or listed by row". Stage 2 merged as Broiler-Platform/Broiler.JS#1135,
`6c61879bce81b1a7badf987e16c7b0531030258d`, the second model commit: with `ParseRequest.BuildSemanticModel` on, "the
parse also reports the model's `semantics` rows among its diagnostics, in decision D11's order, and refuses on them"
(Broiler.JS's `Broiler.ECMAScript/CHANGELOG.md`), the super group, "ContainsArguments of a field initialiser (EE0130)
and of a static block (EE0138), at the member" and "eval code's from the caller's `InheritedContext`, PerformEval's
steps 11.f to 11.h (EA0003, EA0004, EA0005)" among them. Broiler.JS's decision D-js-channel of 2026-11-25 reads "Each
row is placed where Broiler.VM's binder names it (design.md section 3), except EA0005, which takes the library's own
place, and ContainsArguments, which reports at every enclosing member", and its plan, `VMC/` being "Broiler.VM's
`src/Broiler.VM.Profile.JavaScript.Compiler/`", names these, of this family's binder, among "The places where the
library differs from either engine, named for SJL-15.3":

- "Broiler.VM does not search an arrow's parameters for `arguments` (its remark in `VMC/JsBinder.EarlyErrors.cs`), which
  the specification's ContainsArguments does (an inference)";
- "Broiler.VM reports only the innermost member when members nest, because its search does not enter a class body
  (`VMC/JsCompiler.Walk.cs`), where the library reports every enclosing member once";
- "the library reports EA0005 at a real place, each `arguments` reference, where the stage table's row reads "the binder
  reports it at 0:0 (N14); from SJL-15 the model's place is real"".

It names one more among the places where "The library also differs from either engine in what it refuses, named for
SJL-15.3 too":

- "Broiler.VM drops EA0005 when the eval code itself declares `arguments` (`VMC/JsBinder.Walks.cs`,
  `VMC/JsBinder.EarlyErrors.cs`), where ContainsArguments, and so the library, reads every reference; that shows only in
  sources already refused, since eval code in a field initialiser is strict and a strict binding of `arguments` is
  refused".

Broiler.JS's records read "None of it is accepted: no second holder exists to review it." Its decision D-js-channel did
not take "(e) Only the innermost member for ContainsArguments, as Broiler.VM reports: it leaves out a refusal the
specification makes and changes the first error". Row EE1's last cell asks that SJL-15.3, when it deletes the binder's
checks, keep each code and place or list it by row; these places are noted here for that, and nothing is classified for
them here: no cell and no row's class changes, and this family's code reads none of the model before SJL-15.3.

*(Noted 2026-11-25, later yet, at Broiler.JS's SJL-15.2 stage 3, observed; every cell above is left as written.)* Row
LB1's column "Broiler.VM, wide path (`a86b6f5d`)" names "`Target` and the exit stack (`JBW:1364-1389`, `:70-133`), the
lowering's reading of a label; one reader, the binder's own refusal (`JB:280-298`)", its class reads "**Correction of
Broiler.VM** for `continue` naming an outer label of a loop's label set (`sjl-13-001`'s D30, hand-off H20); otherwise
specification", and its last cell "SJL-15.2 adds the jump-target fact; SJL-15.3 reads it, correcting H20 (class F)". Row
EE1's column "Broiler.VM, wide path (`a86b6f5d`)" names "`sjl-13-001`'s B01, B02 (jumps, `JBW:675-702`)", and its last
cell reads "SJL-15.2 reports them from the model; SJL-15.3 deletes the binder's checks, each code and place kept or
listed by row". Stage 3 merged as Broiler-Platform/Broiler.JS#1137, `d87044c9db5fe2a26841782308bd53e2f9294656`, the
third model commit: with `ParseRequest.BuildSemanticModel` on, `SemanticModel.TargetOf` answers "a jump's target, never
null, one instance per target statement; a `continue` naming any label of an iteration statement's label set targets
that statement (hand-off H20, by the specification), and a labelled `break` the labelled item, never the
`LabelledStatement`", and "the parse also reports the jump rows, each at its jump" (Broiler.JS's
`Broiler.ECMAScript/CHANGELOG.md`). Broiler.JS's decision D-js-jump-targets of 2026-11-25 did not take "(b) The
`LabelledStatement` as a labelled `break`'s target, where LabelledEvaluation catches it and where Broiler.VM's binder
names it: a loop's labels and a block's would be read two ways" or "(c) Only the label written directly on a loop, as
Broiler.VM's exit stack carries it: that is H20", and its decision D-js-adapter-groups has its adapter answer the
vectors of "`lb1`, row LB1, on EE0082 and EE0166, `ScriptEvaluation` only". Its plan, `VMC/` being "Broiler.VM's
`src/Broiler.VM.Profile.JavaScript.Compiler/`", names, among "The places where the library differs from either engine,
named for SJL-15.3", these of this family's binder:

- in what it refuses: "Broiler.VM's binder refuses `continue` to the outer label of a loop's label set (its `Target`, in
  `VMC/JsBinder.Walks.cs`, where `VMC/` is Broiler.VM's `src/Broiler.VM.Profile.JavaScript.Compiler/` as in stage 2's
  note; Broiler.VM pins it in its `BinderChecks.cs`, "continue to the outer label of a label set stays refused"), a
  correction of Broiler.VM, class F, row LB1";
- in the target's mapping: "Broiler.VM's `JumpTarget` names the `LabelledStatement` for a `break` to any label not
  written directly on the loop or `switch` it labels, both a target that is no loop or `switch` and an outer label of a
  loop's or a `switch`'s label set (`A: B: while (1) break A;`), and carries only the label written directly on a loop
  or `switch` (`VMC/JsBinder.Walks.cs`, `VMC/JsBinder.cs`), where the library names the labelled item with its whole
  label set; a difference of mapping for SJL-15.3 to read, not of fact but for H20 (an inference)";
- in places: "none known, both engines placing every jump refusal at the jump, as the library does."

Broiler.JS's records read "None of it is accepted: no second holder exists to review it." The first is the correction
row LB1 already records, class F, which its last cell has SJL-15.3 make when it reads the jump-target fact; the second
is, as Broiler.JS's plan words it, "a difference of mapping for SJL-15.3 to read, not of fact but for H20 (an
inference)", which SJL-15.3 reads where the lowering reads a label; the third names no place of row EE1's jump rows
where the library differs. These places are noted here for that, and nothing is classified for
them here: no cell and no row's class changes, and this family's code reads none of the model before SJL-15.3.

*(Noted 2026-11-26, at Broiler.JS's SJL-15.2 stage 4, observed; every cell above is left as written.)* Row PN1's column
"Broiler.VM, wide path (`a86b6f5d`)" names "`PrivateNamesOf` per class (`JBW:1911-1934`), `ResolvePrivate`
(`JBE:222-259`), refusing a name no enclosing class declares; **eval code whose caller is in a class is not checked
here**: the executor checks it at run time (JSD-0083's decision D16)", its class reads "Specification for direct code;
for eval code a **timing** difference, the check run by the executor", and its last cell "SJL-15.3 passes the visible
private names in the inherited context, so eval code's refusal moves to the compile of the eval code; class T (decision
D64's second half, JSD-0088)"; finding F6 below reads "Passing them moves the refusal earlier: decision D64's second
half, class T, applies to this family for this row." Row EE1's column "Broiler.VM, wide path (`a86b6f5d`)" names "B23
(`JBE:222-259`)", the binder's refusal of a private name no class declares, and its last cell reads "SJL-15.2 reports
them from the model; SJL-15.3 deletes the binder's checks, each code and place kept or listed by row". Stage 4 merged as
Broiler-Platform/Broiler.JS#1139, `56272150fe23725a0e70978fb18e3c25f2548548`, the fourth model commit: with
`ParseRequest.BuildSemanticModel` on, `SemanticModel.Resolve` answers, for a `PrivateIdentifier`, "the binding a class
element's key declares or a reference names, in the innermost class that declares it, whatever the placement", and "the
parse also reports the private-name rows, each at its member or `in` expression, once per undeclared reference",
AllPrivateIdentifiersValid "of eval code against the private names its caller sees (EA0009), only where the request's
`InheritedContext.VisiblePrivateNames` gives them" among them (Broiler.JS's `Broiler.ECMAScript/CHANGELOG.md`).
Broiler.JS's decision D-js-private-rows of 2026-11-25 places each row "at the member or `in` expression Broiler.VM's
binder names, once per undeclared reference", and did not take "(e) The rows at the private name, where Broiler.JS
throws: it departs from the binder's place"; its decision D-js-adapter-visible-names reads "**Group `pn1` stays not
implemented**: `pn1/0004` and `pn1/0005` expect a value the Script computes after catching the eval code's refusal,
which a reading at the parse cannot match". Its plan, `VMC/` being "Broiler.VM's
`src/Broiler.VM.Profile.JavaScript.Compiler/`", names, among "The places where the library differs from either engine,
named for SJL-15.3 and SJL-15.5", these of this family's binder and executor:

- in places: "for a member form Broiler.JS throws at the private name
  (`Broiler.JS/Broiler.JavaScript.Compiler/SyntaxValidation.cs`, `VisitMemberExpression`), where the library reports at
  the member, as Broiler.VM's binder does; for `#x in o` the places coincide";
- in timing: "class T: Broiler.VM's binder skips eval code whose caller is in a class body (`ResolvePrivate`, in
  `VMC/JsBinder.EarlyErrors.cs`) and its executor refuses at run time
  (`src/Broiler.VM.Profile.JavaScript/JsEngine.Eval.cs` in Broiler.VM), where the library refuses at the eval code's
  parse when the request carries the names; that is Broiler.VM's finding F6 and class T for row PN1 at Broiler.VM's
  window (its `docs/evidence/sjl-15-001/correspondence.md` and `decision-rule-area.md`), and Broiler.JS's class T row is
  EA0009" (Broiler.JS's bundle `sjl-15-001`'s `corrections/class-t.tsv`);
- in codes: "under first error the library can name EA0009 before a row the specification raises first, an early error
  of ParseText placed later in the eval code or a PerformEval row, and Broiler.VM's executor also checks private names
  before EvalDeclarationInstantiation's conflicts."

The same paragraph reads "The probes T112 to T114, T117 and V034 to V038 all agree (the bundle's `probes/answers.tsv`),
and show neither the binder's place nor the timing", the bundle being Broiler.JS's `sjl-15-001`.

Broiler.JS's records read "None of it is accepted: no second holder exists to review it." The first names the member as
the place of the library's rows, where this family's binder places them; the second is the timing difference that row
PN1 already records, class T, as finding F6 reads it, which row PN1's last cell has SJL-15.3 make when it passes the
visible private names in the inherited context; the third is an order of codes under first error in eval code, where
this family's executor checks private names before EvalDeclarationInstantiation's conflicts, for SJL-15.3 to read when
eval code's refusal moves to the compile of the eval code and when, by row EE1's last cell, each code and place is kept
or listed by row. These places are noted here for that, and nothing is classified for them here: no cell and no row's
class changes, and this family's code reads none of the model before SJL-15.3.

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
- *(Noted 2026-11-16, after SJL-15.1 merged, on the owner's answer to JSD-0088's decision D-census.)* F4 read
  "JSD-0088 puts the reading to the owner (decision D-census)", and rows SL3, SL4, SL5 and DE3 name the decision in
  their last cells. On 2026-11-16 the owner chose "Admit listed rows": "A dated revision now, before any evidence,
  admits those three classes, each listed by row. Every other byte difference still fails part S." SJL-15's clause 5 is
  revised before any evidence, and this bundle's rule revision of the same day,
  [`decision-rule-area-revised-2026-11-16b.md`](decision-rule-area-revised-2026-11-16b.md), admits at part S the
  corrections of row SL4 (class H), the refusals of rows SL3, DE3 and EE1, and the single refusals of row SL2's
  tokenizer and parser but for the manifest's, that the library makes in place of the slice path's own (class R) and the
  census lines of row SL5 that the library's parse moves (class C), each listed by row. No row's class changes, and
  every cell above stands as read.
- **F6. Eval code's undeclared private name is the executor's check** (PN1), by JSD-0083's decision D16, which kept the
  visible private names out of the request at SJL-13. Passing them moves the refusal earlier: decision D64's second half,
  class T, applies to this family for this row.
- **F7. The model has no kind for a `with` scope, eval code's scope, a class body or a separate var environment**
  (SC2), which `areas/static-semantics.md` section 1 lists among the shared facts. SJL-15.2's records say how each is
  carried.
- *(Noted 2026-11-25, at Broiler.JS's SJL-15.2 stage 1, observed; F7 is left as written.)* F7 reads "SJL-15.2's records
  say how each is carried". Stage 1 merged as Broiler-Platform/Broiler.JS#1132,
  `be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`, with Broiler.JS's decision D-js-kinds of 2026-11-24 in its
  `docs/roadmap/SharedLibraries.md`, section "Shared static semantics (SJL-15)": "**`ScopeKind` gains `With`,
  `ClassBody`, `FunctionName` and `Parameters`; eval code is `FunctionKind.EvalCode` on the program's facts, its scope
  still a `Script` scope; the separate variable environment is a function fact (row FF4), stage 5a's.** This is the
  answer to Broiler.VM's finding F7 ("SJL-15.2's records say how each is carried")". The separate variable environment's
  fact is still to come, at that stage. No cell and no row's class changes, and this family's code reads none of it
  before SJL-15.3.

## Hand-offs this file reads

From JSD-0083's table: H18 (the dynamic function's goals; recommended there for "SJL-15's Broiler.VM window, which re-pins
the frontend"), H19 (MR2), H20 (LB1), H23 and H25 (SL4). JSD-0088 routes each.
