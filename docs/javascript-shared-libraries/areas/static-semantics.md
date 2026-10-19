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
| Declarations and conflicts | Parser (`LexicallyDeclaredNames`, `VarDeclaredNames`, `ValidateDeclaredNames`); the lowering's `Scope.Declare` | `SliceStaticSemantics` | Parser (`FastScope`), compiler (`FastFunctionScope`) |
| Resolution | The lowering: static (hops, slot), dynamic for `with` and sloppy direct `eval` (`Shadowable`) | The binding table, refusing unresolvable names | The compiler, tied to its expression variables |
| `arguments` | A syntactic over-approximation (`UsesArguments`) | (none) | The compiler |
| Direct-`eval` contamination | `EvalVariables`, `SeparateBody`, `ParametersMakeClosures` | (none) | `DirectEvalSupport` |
| Annex B.3.3 | `ScanAnnexB`, in the lowering | refused | Parser names, compiler bindings |
| Private names, `super`, `new.target`, `return`, labels | Lowering | refused | Compiler, `SyntaxValidation` |
| Function names | Lowering (`CompileNamedValue`); `export default` naming in the parser | (none) | Settable `InferenceName` on the tree |
| Free names and captures | None: every binding lives in an environment record | (none) | `FreeNameScan` |
| Coupled with emission | **Yes**: `HoistProgram`, `HoistFunction` and `HoistEval` collect **and** emit | **Yes**: the analysis allocates frame slots | **Yes**: scope objects tied to expression variables |
| Deterministic order | Dictionary insertion order in the wide hoisting (stage JSB-1) | Yes | Dictionary enumeration order |

SJL-12 separates Broiler.VM's wide binder from its emission and makes it deterministic. That
separated binder is what SJL-15 replaces with the shared model.

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
