# Area: shared static semantics

**Delivered by:** [SJL-15](../milestones/sjl-15-static-semantics.md), on the contract recorded at
[SJL-12](../milestones/sjl-12-frontend-contract.md). **Library:** `Broiler.ECMAScript.Frontend`
(semantics half). **Status:** design.

---

## 1. Scope

The semantic model is the single source of the **language-level** facts about a parsed program.
Each lowering takes its **back-end** decisions from those facts.

| Shared: language facts | Not shared: back-end decisions |
|---|---|
| Scopes and their kinds (program, function, block, module, `with`, eval, class body, function-body separation) | Slot and environment-record allocation |
| Declarations (`var`, `let`, `const`, `class`, function, parameter, `catch`, import) with their mutability, and every conflict | Evaluation scope-map rows (Broiler.VM) |
| Annex B.3.3 block-function aliases | Boxing, deferral and closure representation (Broiler.JS) |
| Per-function flags: strict; simple parameters; has parameter expressions; needs a separate var environment; `arguments` needed and mapped; direct `eval` in body or parameters; contains `with`; kind (arrow, async, generator, method, constructor, derived, field initialiser, static block) | Admission against a feature manifest (Broiler.VM) |
| Reference resolution with **dynamic markers** where `with` or sloppy direct `eval` makes resolution dynamic | Emission order and instruction choice |
| Labels and jump targets | Format ceilings |
| Private-name environments and their resolution | (none) |
| Module records: requests with attributes, top-level `await`, import and export entries, and their errors | (none) |
| Named-evaluation names (function name inference) | (none) |
| Free names and captures | (none) |
| **Every early error** that is not purely grammatical | (none) |

---

## 2. Current state

| Fact | Broiler.VM (wide) | Broiler.VM (slice) | Broiler.JS |
|---|---|---|---|
| Declarations and conflicts | Parser (`LexicallyDeclaredNames`, `VarDeclaredNames`, `ValidateDeclaredNames`); the binder pass's declarations in source order, which the lowering's `Scope.Declare` allocates *(revised 2026-10-25, at SJL-12.6 item 1, `3fa4b39`: the cell ended "the lowering's `Scope.Declare`")* | `SliceStaticSemantics` | Parser (`FastScope`), compiler (`FastFunctionScope`) |
| Resolution | The lowering: static (hops, slot), dynamic for `with` and sloppy direct `eval` (`Shadowable`) | The binding table, refusing unresolvable names | The compiler, tied to its expression variables |
| `arguments` | A syntactic over-approximation (`UsesArguments`) | (none) | The compiler |
| Direct-`eval` contamination | `EvalVariables`, `SeparateBody`, `ParametersMakeClosures` | (none) | `DirectEvalSupport` |
| Annex B.3.3 | `ScanAnnexB`, in the binder pass, whose aliases and admitted declarations the lowering reads *(revised 2026-10-25, at SJL-12.6 item 1, `3fa4b39`: the cell read "`ScanAnnexB`, in the lowering")* | refused | Parser names, compiler bindings |
| Private names, `super`, `new.target`, `return`, labels | The binder pass: each class's private names and each jump's target as facts, and their early errors; the lowering keeps an exit stack of its own for emission *(revised 2026-10-25, at SJL-12.6 items 1 and 2, `3fa4b39` and `daa92eb`: the cell read "Lowering")* | refused | Compiler, `SyntaxValidation` |
| Function names | Lowering (`CompileNamedValue`); `export default` naming in the parser | (none) | Settable `InferenceName` on the tree |
| Free names and captures | None: every binding lives in an environment record | (none) | `FreeNameScan` |
| Coupled with emission | The binder pass collects, and `HoistProgram`, `HoistFunction` and `HoistEval` read its facts and emit; evaluation scope-map rows are still recorded during emission *(revised 2026-10-25, at SJL-12.6 item 1, `3fa4b39`: the cell read "**Yes**: `HoistProgram`, `HoistFunction` and `HoistEval` collect **and** emit")* | **Yes**: the analysis allocates frame slots | **Yes**: scope objects tied to expression variables |
| Deterministic order | Source order in the wide hoisting, by construction, with two checks rows that are stage JSB-1's standing check *(revised 2026-10-25, at SJL-12.6 item 3, `066abef`: the cell read "Dictionary insertion order in the wide hoisting (stage JSB-1)")* | Yes | Dictionary enumeration order |

SJL-12 separates Broiler.VM's wide binder from its emission and makes it deterministic. That
separated binder is what SJL-15 replaces with the shared model.

*(Noted 2026-10-25, at SJL-12.6's records.)* The wide column above is read at bundle `sjl-12-001`'s after commit,
`1de3af6`, and each cell SJL-12.6 changed quotes what it read. The binder pass is `JsBinder`, in the compiler
assembly (JSD-0080 decision D11):
- item 1 (`3fa4b39`, Broiler.VM#165) gave it the facts: declarations per hoisting scope and statement list, Annex B
  aliases, each class's private names, each jump's target, and the per-function facts the `arguments` and
  direct-`eval` rows name, `UsesArguments`, `ParametersMakeClosures` and the separate body, computed there once
  with the same approximations;
- item 2 (`daa92eb`, Broiler.VM#166) moved the lowering's early errors into it, with their codes;
- item 3 (`066abef`, Broiler.VM#163) put the hoisting order in the source's order
  ([JSC-319](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-319)).

The paragraph above is kept as written. Whether SJL-12's clauses hold is bundle `sjl-12-001`'s to decide, on runs
that had not been taken when this note was written, and SJL-12 cannot meet its gate before SJL-3 meets SJL-3's.

---

## 3. The model

A sketch of the shape. The design is filed in
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
section "The semantic model's design", at SJL-12.1. The home's design document, `Broiler.ECMAScript/docs/frontend/design.md`
in the Broiler.JS repository, restates it. Where the two differ, JSD-0080 and this design rule. *(Revised
2026-10-19, at SJL-12.1, by JSD-0080 decisions D5 and D41. The sentence read "The design is filed at SJL-12.1.")*

```csharp
namespace Broiler.ECMAScript.Semantics;

public sealed class SemanticModel
{
    public IReadOnlyList<Scope> Scopes { get; }                    // in source order
    public Scope ScopeOf(SyntaxNode node);
    public Declaration? DeclarationOf(SyntaxNode bindingIdentifier);
    public Resolution Resolve(IdentifierReference reference);      // Static(scope, declaration) | Dynamic(reason) | Global
    public FunctionFacts FactsOf(FunctionNode function);
    public LabelTarget? TargetOf(BreakOrContinue statement);
    public PrivateNameBinding? Resolve(PrivateNameReference reference);
    public ModuleRecord? Module { get; }
    public string? NamedEvaluationName(SyntaxNode node);
    public IReadOnlyList<Declaration> Captured(FunctionNode function);
    public IReadOnlyList<Diagnostic> EarlyErrors { get; }          // deterministic order
}
```

**Ordering.** Every collection is in source order. No order comes from a hash table.

**Inherited context.** Evaluation code takes the caller's scope facts from the request. Broiler.VM
passes them as its evaluation flags plus the names its scope map records. Broiler.JS passes them from
its own direct-`eval` support.

---

## 4. Migration

1. **SJL-12.** Broiler.VM's binder pass is separated and deterministic; its wide early errors move
   into it. Broiler.JS's `for` desugaring leaves the parser.
2. **SJL-15.** The shared model is built against the early-error catalogue of SJL-12.3, every row.
   Then:
   - **Broiler.VM's lowering** reads it, and the binder pass is deleted.
   - **Broiler.VM's slice path** is rebuilt on the model (decision
     [SJL-D-13](../decisions.md#sjl-d-13) (a)). `SliceStaticSemantics` reads it, `SliceParser` is
     deleted, and the slice lowering keeps its slot allocation. Its refusal of unresolvable
     identifiers becomes an admission rule over the model's resolution.
   - **Broiler.JS's compiler** reads it. These are deleted: `FastScope`'s scope half,
     `FastFunctionScope`, `FreeNameScan`, `SyntaxValidation`, `FastCompiler.EarlyErrors`, the
     early-error checks in `Visit*` methods, and `DirectEvalSupport.Validate`.

---

## 5. Acceptance

**Both engines:**

- test262's early-error tests at the pin, per variant (Broiler.VM) and per path (Broiler.JS), under
  the predeclared rules;
- every catalogue row covered by at least one vector or test.

**Broiler.VM additionally:**

- byte-identical artifacts over the parse corpus, or classified differences;
- the slice corpus's census counts, slot order and refusals, byte for byte before and after
  (decision `SJL-D-13` (a));
- determinism over two runs and two threads.

**The timing of `SyntaxError`s changes in Broiler.JS.** Errors it raises today during compilation
are raised by the front end. A test that depended on a later error is listed.
