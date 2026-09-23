<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0026 - The direct-eval environment boundary: what crosses into dynamic compilation, and what stays refused until it does

**Status:** Proposed. 2026-09-21. **Steps 1-5 of section 9 are implemented (JSeal V14, 2026-09-21)
and steps 6-9 too (JSeal V15, 2026-09-21); JSeal V15-finish (2026-09-21) answers the two
parameter-list shapes, `super` and private names that section 13 left refused, and module-scoped
sites keep their refusal.** It is the design that JSeal's roadmap slice V13 asks for; the
implementation is V14 and V15 of that plan. **The record is still unsigned**: V14 and V15 implemented
a proposal nobody has approved, and what each did differently from the text below is listed in
sections 12, 13 and 14.

**Owner:** this profile's owner, MaiRat, **who has not reviewed this record**; it was drafted by an
AI agent for slice V13 and carries no human decision. **Co-signer:** none. A record nobody has signed
is a proposal, and the status line says so rather than the owner line pretending otherwise.

**Milestone:** none in the `JS-` series. The JSeal plan relates V13-V15 to
[JSP-3](../roadmap.parity.md#jsp-3--the-static-semantics-the-wide-front-end-does-not-have),
[JSP-10](../roadmap.parity.md#jsp-10--the-host-surface-an-embedder-meets-first) and
[JSW-3](../roadmap.workloads.md#jsw-3--the-dynamic-surface-and-a-refusal-that-happens-where-the-plan-says-it-does);
the defects it answers are the two `eval` rows of [parity section 4.6](../roadmap.parity.md#46-the-static-semantics-and-where-declarations-live),
and the route every byte takes is [section 11](../roadmap.md#11-guest-initiated-loads-eval-the-function-constructor-dynamic-import-modules).
**This record accepts no stage and moves no ledger row.**

**Context.** A direct `eval` inside function code throws a catchable `EvalError` by design
(`JsEngine.Evaluate`), because the artifact a provider answers with was compiled without knowledge of
the frame that asked for it. That refusal is honest and it is the right answer until something better
exists. Replacing it with a global evaluation - the one change that would be quick - is excluded by
the card, because it answers a program that reads a local with a global's value. What follows is what
"something better" has to carry, where each piece lives, and in what order it can land without any
intermediate state answering wrongly.

---

## 1. What the checkout does today, observed rather than read

**Name resolution is finished at lowering.** `JsCompiler.TryResolve` walks a compile-time `Scope`
chain and answers a `(hops, slot)` pair; `LoadScoped`/`StoreScoped` carry that pair and the executor
walks `hops` records up a `JsEnvironment` chain and indexes `Slots`. Every binding - parameter, `var`,
`let`, the `arguments` object, a closure's capture - is a slot in a heap record, so a closure is a
pointer to a record and **there are no registers and no copied captures**: a write through a slot is
seen by every closure over that record. A name no scope binds becomes `LoadGlobal`/`StoreGlobal`, or
`LoadImport` in a module. `JsEnvironment`'s own invariant is that **a declarative record has no names
in it**; the only record searched by name is the object record a `with` pushes, reached by
`ResolveName` with a compile-time bound computed by `Shadowable` and charged one unit per record
visited.

**Directness is a fact about the spelling, recorded by an opcode.** `CompileCall` emits `CallEval`
when the callee is the bare identifier `eval` and no enclosing scope binds that name; the executor then
compares the callee against `JsRealm.EvalIntrinsic` and makes an ordinary call when they differ. A
locally bound `eval` is an ordinary `Call` at compile time. `CallEval` runs alone in the baseline
native form (`JsBaselineBlocks.RunsAlone`, `JsBaselineHandlers.StepCallEval`).

**Evaluation is a guest-initiated load and nothing else.** `Evaluate` refuses when the caller unit
lacks `FunctionFlags.ProgramBody`, charges `1 + bytes`, and sends the UTF-8 source as the opaque
payload of a `VmArtifactRequest` to the one artifact provider the profile has
(`VmRuntime.ProviderFor`). The provider compiles it as a **Script** unit named `main`; the engine
runs its entry with a fresh environment whose parent is `null` and `this` bound to the global object
(`RunEntry`). A `ProviderRefused` answer becomes a `SyntaxError`; every other failed load is an
`EvalError`. Indirect `eval` and the `Function` constructor take the same route with `direct: false`.
The only payload vocabulary is `JsFormat.ModuleRequestMark` (`0x00`) for dynamic `import()`.

**Compile permission is the provider's answer.** A composition that registers no provider refuses
every route; JSeal's `VmSourceProvider` consumes a one-shot host permit at the top of `Answer` and
otherwise refuses guest compilation unless `AllowGuestEval` (J04). Because every route reaches the
same `Answer`, every route is subject to the same permit today.

Observed on 2026-09-21 with the worktree's Release build of the end-user host
(`Broiler.VM.Composition.JavaScript.Cli.exe --quiet`) and, as a diagnostic only, Node v24.17:

| Program (abridged) | This profile | Specification |
|---|---|---|
| `function f(){ let x = 7; return eval('x') }` | `EvalError` (the published refusal) | `7` |
| `function f(){ var eval = g; return eval('1') }` | ordinary call to `g` | ordinary call |
| `function f(){ var x = 'local'; return (0, eval)('x') }` | the global `x` | the global `x` |
| `function f(){ var x = 'local'; return eval?.('x') }` | the global `x` | the global `x` (an optional call is not direct) |
| `function f(){ var x = 'local'; return eval(...['x']) }` | **the global `x`** | `'local'` - a spread call is direct (Test262 `expressions/call/eval-spread.js`); Node also answers the global, which is Node's divergence and not the oracle |
| `function f(){ var x = 'local'; with ({ eval }) return eval('x') }` | `EvalError` | `'local'` |
| `function f(){ var x = 'local'; var eval = g; with ({ eval: realEval }) return eval('x') }` | **the global `x`** | `'local'` - the reference's base is the `with` record, so the call is direct |
| top level: `eval('let gl = 1'); typeof gl` | **`"number"`** | `"undefined"` |
| top level: `let c = 1; eval('var c = 2')` | **no error, `c` becomes 2** | `SyntaxError`, `c` stays 1 |
| top level: `let c = 1; eval('let c = 3'); c` | **`3`** | `1` |
| strict top level: `eval('var sv = 1'); typeof sv` | **`"number"`** | `"undefined"` |
| strict top level: `eval('eval("with({}){}")')` | **no error** | `SyntaxError` (strictness is inherited) |
| module top level: `let x = 1; eval('x')` | **`ReferenceError`** | `1` |
| top level: `var x = 'global'; { let x = 'block'; eval('x') }` | **`'global'`** | `'block'` |
| top level: `{ let y = 1; eval('y = 2'); y }` | **`1`** | `2` |
| top level: `for (let i = 5; i < 6; i++) eval('i')` | **`ReferenceError`** | `5` |
| top level: `switch (1) { case 1: let s = 'sw'; eval('s') }` | **`ReferenceError`** | `'sw'` |
| top level: `try { throw 'caught' } catch (e) { eval('e') }` | **`ReferenceError`** | `'caught'` |
| top level: `try { throw 1 } catch (e) { eval('var e = 2'); e }` | **`1`** | `2` - Annex B.3.4 lets the `var` through and its initialiser assigns the catch parameter |
| top level: `var x = 'global'; with ({ x: 'with' }) eval('x')` | **`'global'`** | `'with'` |
| top level: `{ eval('x') }`, `{ function bf(){} eval('x') }` | the global `x` | the global `x` (nothing binds `x` in between) |

**The bold rows are wrong answers on surfaces that are admitted today**, not refusals. The global
ones are parity section 4.6's. The spread row is already stated as a divergence in the source:
`JsCompiler.CompileArguments` says a direct `eval` spelled with a spread loses its directness, and
that there is deliberately no `CallEvalSpread` because no program in the corpus uses the spelling.
The `with`-in-front-of-a-local, module and program-body-under-a-record rows are recorded here for the
first time. **The program-body rows have one cause**: `Evaluate` admits a direct `eval` whenever the
caller *unit* carries `FunctionFlags.ProgramBody`, and that flag says nothing about the block,
`for`-`let`, `switch`, `catch` or `with` records a program body pushes around the call site, so the
source runs against the global scope with those records out of sight. The pinned Test262 subtree `test/language/eval-code` (347
files, 454 variants) passed **139** under the bytecode form on the same build; of the 315 failures, 149
are `Expected a SyntaxError but got a EvalError` and 42 are the refusal itself uncaught, so most of the
directory is gated on this design rather than on anything smaller. The native form failed all 454 at
instantiation (`ProfileFault/UnsatisfiedHostAssumption`), which is not about `eval`: 6 of the 10
variants of `test/language/types/boolean` fail the same way with the same build (checkout `484f389`;
commands in [the V13 validation record](../../../../docs/evidence/jseal-v13/README.md)). The run is a
partial selection and is not retained as a floor.

## 2. The decision in one paragraph

**The caller's artifact describes, for each direct-`eval` site, which names are visible there and
where they live; the evaluated source is compiled as eval code, with its caller's strictness and
syntactic permissions, and resolves every name it does not declare itself through that description at
run time against the caller's live records.** Nothing about the caller's slots is sent to the provider
or trusted from its answer. A sloppy function that contains a direct `eval` additionally owns a
by-name record for the `var`s an evaluation may add to it. Every step keeps the current refusal for
every shape it has not yet made correct.

## 3. What crosses the boundary, and in which direction

| Carried | From | To | How |
|---|---|---|---|
| Directness | the spelling | the executor | `CallEval` (exists) and `CallEvalSpread` (new, section 9 step 1, reversing the choice `CompileArguments` records) |
| Caller's visible bindings | the caller's lowering | the executor | an **eval scope map** row per site, in the caller's artifact (section 4) |
| Caller's live records | the executing frame | the eval entry | the frame's current record becomes the eval entry record's parent |
| Strictness and syntactic permissions | the caller's unit flags and site row | the provider | the **eval request** payload (section 5) |
| `this`, its binding cell, `new.target`, the active function | the executing frame | the eval frame | passed as the arguments `Execute` already takes |
| Source identity | the profile | the eval artifact | a fixed unit key and eval-relative positions (section 7) |
| Compile permission | the composition | every route | the one provider, asked once per evaluation (section 8) |

**What does not cross is as important.** The provider never learns a slot number, and the eval
artifact never contains one of the caller's; the only thing a provider could get wrong is the eval
code itself, which the verifier reads as it reads any artifact.

## 4. The caller half: the eval scope map

**A new optional section kind, `EvalScopes = 13`, under format version 2.** Kinds 9 to 12 were added
to version 2 the same way - optional, their absence meaning "declares none", admitted only beside the
surface that needs them - and this one is admitted only by an artifact that declares
`broiler.javascript.dynamic`. It holds two tables:

- **Scope shapes.** One row per compile-time `Scope` that is visible from at least one site: its kind
  (function, block, catch clause, `with`, eval-variables, program, module, or the eval boundary that
  ends a chain inside eval code), its parent row, and its names, each with a slot and a binding kind -
  `var`/parameter/function, `let`, `const`, `class`, catch parameter, immutable function-expression
  name. **The catch clause is a scope kind and not only a binding kind** because the Annex B.3.4
  exemption in `EvalDeclarationInstantiation` is stated per record: the pinned ES2026 text (this
  profile's `docs/specification`) replaces the throw with "if *thisEnv* is not the Environment Record
  for a Catch clause, throw", with no condition on the parameter's form. The simple
  `BindingIdentifier` condition belongs only to the static early error for a `var` written in the
  catch block itself. A destructured catch parameter is therefore exempt for evaluated declarations;
  V8 throws there, which is its divergence. A `with` row carries no names; it tells the executor to ask the object at
  that depth. Rows are shared by every site in the same scope, so the table grows with the number of
  distinct scopes that contain a site, not with the number of sites.
- **Sites.** One row per `CallEval`/`CallEvalSpread` in a function unit, in eval code's own units
  (which is what nesting needs), **and in a script's program body whenever a declarative or `with`
  record lies between the site and the program's entry record** - a block with declarations, a
  `for`-`let` head, a `switch` body, a `catch` clause, a `with`. Each row gives the unit, the
  instruction's offset, its innermost scope-shape row, the hop count to the variable record (for a
  program-body site the chain ends at the program row and the variable environment is the global
  one), and the flags section 5 sends. Only a program-body site with no record in between has no row,
  and it alone keeps the global path.

**The verifier checks what the bytes can show:** every site offset is an instruction boundary holding
one of the two opcodes; every shape chain ends at a program, module or eval-boundary row; the chain's
length at a site equals the scope depth the abstract pass already computes at that offset; names are
interned names. As for `LoadScoped` today, **slot bounds are the executor's to check** - a record may
belong to a closure outside the unit - and it already aborts with `InternalDefect` when a slot does
not exist, so a wrong map is a wrong answer or a defect and never an unowned read.

**The refusal becomes data-driven, and that is what keeps it explicit.** A `CallEval` in a function
unit with no site row, or in a program body whose current record is not its entry record and that has
no site row - every artifact written before this section, and every shape a later step has not
admitted - keeps the `EvalError` (for the program-body case, from step 1 on). Admission is additive
per row.

## 5. The eval half: an eval request and an eval-code goal

**The payload gains a second mark.** `JsFormat.EvalRequestMark = 0x01`, followed by one flags byte
and the UTF-8 source, read back by `JsFormat.TryReadEvalRequest` beside `TryReadModuleRequest`. The
flags are: strict (inherited from the caller unit's `Strict`), in-function, `new.target` permitted,
`super` property permitted, `super` call permitted, `arguments` forbidden (a class field initialiser
or static block). These are exactly the facts `PerformEval` needs before parsing and nothing else.

**The provider compiles the source under a new goal, `SliceParseOptions.Eval(flags)`**, and the
lowering differs from a Script's in four places: top-level `let`/`const`/`class` are slots of the
eval's own record, never the realm's lexical half; in strict eval code top-level `var` and function
declarations are slots of that record too; a name the eval code does not bind is lowered to one of the
new name instructions below, not to `LoadGlobal`; and the entry unit carries a new flag,
`FunctionFlags.EvalCode = 1024`, plus a declaration row naming its sloppy `var` and function
declarations and the lexical names they must not collide with.

**Five instructions, each `u8` hops-to-boundary and `u16` name**, where the boundary is the eval
entry record: `LoadEvalName`, `LoadEvalNameOrUndefined` (for `typeof`), `StoreEvalName`,
`LoadEvalNameWithBase` (a call's callee and receiver: a `with` object answers itself, every other
record `undefined`), and `DeleteEvalName`. The executor walks to the boundary, reads the record's
**eval view** - the site row it was entered with, the caller's record, and the view that record was
itself entered with - and resolves outward: declarative rows by the map to a live slot (TDZ and
`const` exactly as `LoadScoped`/`StoreScoped` treat them), `with` rows by the object (with
`Symbol.unscopables`), eval-variable rows by their own bindings, then the realm's global lexical
half and the global object. Unresolvable reads throw `ReferenceError`; unresolvable strict writes
too; sloppy writes create a global property.

**The eval view lives on the record, not on the engine**, and that is the whole answer to nested and
re-entrant evaluation: a closure created by eval code and called after the evaluation returned walks
to the same boundary record and finds the same view, and two activations of one function each have
their own record and therefore their own view.

**The answer is bound to the request.** An answer to an eval request must be an artifact whose entry
unit carries `EvalCode` and the request's flags; anything else is refused with an `EvalError` naming a
provider that answered for another goal. That is the fail-closed half of the provider-compatibility
rule in section 8.

## 6. The walk-through

Each case names the intended answer (by the specification) and the mechanism above that produces it.
The step that makes each one true is in the last column; until then the case is refused, not answered.

| Case | Program | Answer | Mechanism | Step |
|---|---|---|---|---|
| Local read | `function f(){ let x = 7; return eval('x') }` | `7` | `LoadEvalName` → site row → caller slot | 5 |
| Local write | `function f(){ var x = 1; eval('x = 2'); return x }` | `2` | `StoreEvalName` writes the caller's slot | 5 |
| TDZ | `function f(){ { eval('x'); let x } }` | `ReferenceError` | the slot is still `Empty`; the same case at function-body top level waits on the missing TDZ parity section 4.6 records, not on this design | 5 |
| Constant | `function f(){ const k = 1; eval('k = 2') }` | `TypeError` | the map's binding kind, as `ThrowImmutable` | 5 |
| Captured | `function f(){ let x = 1; const g = () => x; eval('x = 5'); return g() }` | `5` | the slot is in the record `g` closes over | 5 |
| Closure inside eval | `function f(){ let x = 1; const h = eval('() => x'); x = 9; return h() }` | `9` | `h` walks to the boundary record and its view | 5 |
| Nested eval | `function f(){ let x = 1; return eval("let x = 2; eval('x')") }` | `2` | the inner site is in eval code; its map covers the eval's own record, then the outer view | 5 |
| Re-entrant | `function f(n){ let x = n; return n ? eval('f(n - 1) + x') : 0 }` with `f(3)` | `6` | one record and one view per activation | 5 |
| Shadowed, statically | `function f(){ var eval = g; eval('x') }` | ordinary call | `Call`, as today | exists |
| Shadowed, at run time | `eval = g; function f(){ eval('x') }` | ordinary call | intrinsic identity check, as today | exists |
| Shadowed inside eval code | `function f(){ return eval("var eval = g; eval('1')") }` | ordinary call to `g` | the inner `CallEval` loads `eval` through the view, finds the introduced binding, fails the identity check | 6 |
| `with` supplies the intrinsic | `with ({ eval }) eval('x')` in a function | direct | the site row covers the `with` record | 5 |
| `with` in front of a local `eval` | `function f(){ var eval = g; with ({ eval: realEval }) return eval('x') }` | direct | today an ordinary `Call` (the name is statically resolvable), so a silent global evaluation; step 1 emits `CallEval` whenever a `with` record lies between the site and the binding | refused in 1, admitted in 5 |
| Indirect | `(0, eval)(s)`, `var e = eval; e(s)`, `eval?.(s)` | global | not `CallEval`; unchanged | exists; goal change is step 8 |
| Spread | `function f(){ var x = 1; return eval(...['x']) }` | `1` | `CallEvalSpread` | refused in 1, admitted in 5 |
| Strict isolation | `function f(){ 'use strict'; eval('var v = 1'); return typeof v }` | `"undefined"` | strict eval `var` is a slot of the eval's own record | 5 |
| Strict source, sloppy caller | `function f(){ eval("'use strict'; var v = 1"); return typeof v }` | `"undefined"` | the eval code's own directive makes it strict | 5 |
| Inherited strictness | strict caller: `eval("with ({}) {}")` | `SyntaxError` | the strict flag in the request | 5 (function), 8 (top level) |
| Lexical isolation | `function f(){ eval('let y = 1'); return typeof y }` | `"undefined"` | eval lexicals are slots of the eval's own record | 5 |
| Sloppy `var` introduction | `function f(){ eval('var y = 1'); return y }` | `1` | the eval-variables record (below) and the caller's dynamic lookup of `y` | refused in 5, admitted in 6 |
| Introduced binding is deletable | `function f(){ eval('var y = 1'); delete y; return typeof y }` | `"undefined"` | eval-variable bindings are deletable; slots are not | 6 |
| Existing `var` reused | `function f(a){ eval('var a = 2'); return a }` | `2` | the name is in the function's shape row; no new binding | 6 |
| Conflict with an enclosing lexical | `function f(){ let a; { eval('var a') } }` | `SyntaxError`, nothing created | the declaration row checked against every shape row between the site and the variable record, before any creation | 7 |
| Catch parameter | `try { throw 1 } catch (e) { eval('var e = 2'); return e }` in a function | `2`, no error (Annex B.3.4) | the catch-clause shape row is exempt from the conflict walk; the initialiser resolves to the catch parameter | 7 |
| Destructured catch parameter | `try { throw [1] } catch ([e]) { eval('var e = 2'); return e }` in a function | `2`, no error | the exemption is per catch-clause record, not per parameter form (pinned ES2026 `EvalDeclarationInstantiation`); Node throws a `SyntaxError`, which is not the oracle | 7 |
| Program body, block | top level: `var x = 'g'; { let x = 'b'; eval('x') }` | `'b'` | a site row, because a record lies between the site and the program; the same resolution as a function site, falling through to the global scope | refused in 1, admitted in 5 |
| Program body, other records | top level: `for (let i = 5; i < 6; i++) eval('i')`; `switch (1) { case 1: let s = 1; eval('s') }`; `try { throw 1 } catch (e) { eval('e') }`; `{ let y = 1; eval('y = 2') }` | `5`; `1`; `1`; `y` becomes `2` | as above; the `for`-`let` row is the per-iteration record the frame holds at the call | refused in 1, admitted in 5 |
| Program body, `with` | top level: `with ({ x: 'w' }) eval('x')` | `'w'` | the site row's `with` row asks the object | refused in 1, admitted in 5 |
| Program body, declaring | top level: `{ let b; eval('var v = 1') }`; `try { throw 1 } catch (e) { eval('var e = 2') }` | `v` a configurable global; the catch parameter becomes `2` | the global variable environment and the conflict walk over the site's rows | refused in 5, admitted in 8 (after 7) |
| Program body, nothing in between | top level: `eval('x')`, or a block that declares nothing | the global `x` | no site row; the global path | exists; goal change is step 8 |
| Arrow function | `function f(a){ return (() => eval('[arguments[0], this, new.target]'))() }` | the enclosing function's `arguments`, `this` and `new.target` | an arrow binds none of the three, so the map resolves `arguments` to the enclosing non-arrow function's slot, and the eval frame takes the values the arrow's own frame answers for `this` and `new.target`; the enclosing function is marked `UsesArguments` when an arrow nested in it through arrows only contains a direct `eval` | 5 |
| Arrow at top level | top level: `(() => eval('this'))()` | the global `this` | a function unit, so the site has a row; nothing in its chain binds `arguments` | 5 |
| Global conflict | top level: `let c; eval('var c')` | `SyntaxError` | global lexical half checked first | 8 |
| Non-definable global | top level: `eval('function NaN(){}')` | `TypeError` | `CanDeclareGlobalFunction` | 8 |
| Configurable global `var` | top level: `eval('var gv'); delete gv` | `true` | eval-created global properties are configurable | 8 |
| `arguments` read | `function f(a){ return eval('arguments[0]') }` | `a` | a function containing a direct `eval` always materialises `arguments` | 5 |
| `arguments` aliasing | sloppy `function f(a){ eval('arguments[0] = 2'); return a }` | `2` | the same object the function has; aliasing is V05-V06's, not this record's | after V05-V06 |
| `arguments` in a field initialiser | `class C { x = eval('arguments') }` | `SyntaxError` | the forbidden flag | 5 |
| `this` / `new.target` | `eval('this')` in a method; `eval('new.target')` in a function; at top level | the caller's; the caller's; `SyntaxError` | frame values and flags | 5 |
| `this` before `super()` | derived constructor: `eval('this')` before `super()` | `ReferenceError` | the frame's binding cell is passed, not a value | 5 |
| `super` in eval | `eval('super.m()')`, `eval('super()')` | per specification | **not admitted by any step here; refused** | none |
| Private names | `eval('this.#p')` in a class body | per specification | **refused**; the private environment is not described by the map | none |
| Parameter initialiser | `function f(a = eval('var z = 1'), b = z) {}` | per specification | **refused until step 9**: the separate variable record of a non-simple parameter list | 9 |
| Module top level | `let x = 1; eval('x')` | `1` | **refused in step 1** (today it answers `ReferenceError`); imports need a map row kind of their own | none in V14-V15 |

**The eval-variables record (steps 6-7).** A sloppy function whose body contains a direct `eval`
gets one more record, placed immediately outside its own function record - between it and the
record it closes over - holding only bindings an evaluation introduced, by name, deletable. It is a
new `JsEnvironment` kind with `ScopeKind.EvalVariables` as its compile-time twin, and its only reader
is a variant of the `with` machinery: `Shadowable` treats it as a search point, so a free name in that
function, or in any function nested in it, costs a search, a branch and the static fallback, exactly
as a name inside a `with` does now. Two things differ from a `with` record: its base is always
`undefined`, and it has no `Symbol.unscopables`. **Its object is never reachable from guest code**,
which is the property that `JsEnvironment`'s current falsifier - "a lookup by name reaches a slot of a
declarative record" - must be restated to keep: a by-name lookup reaches an object a `with` pushed or
a binding an evaluation introduced, and still never a compiled slot. Placing it outside the function
record rather than inside is safe because an evaluation never introduces a name the function record
already binds; section 5's declaration row sends such a name to the existing slot.

## 7. Source identity

The eval artifact's entry key is `eval`, not `main`, so a diagnostic says what the code was. Positions
in it are relative to the evaluated text; the call site's position is already in the caller's position
table. **No label travels with an eval request**: a guest cannot name its own evaluated code, and a
host label (JSeal J17) is a host-script concern that neither reaches this path nor grants anything. No
stack frame is invented for the boundary.

## 8. Compile permission on every route

| Route | Asks the provider | Permit in JSeal |
|---|---|---|
| Direct `eval` in a function, a program body, or eval code | yes, once per evaluation | `Guest`, subject to `AllowGuestEval` |
| Indirect `eval` | yes | `Guest`, or a host permit armed immediately before the captured intrinsic |
| `Function` constructor | yes | `Guest` |
| Dynamic `import()` | yes, or answered from the artifact | `Guest` |
| A host script through the captured intrinsic | yes | `Host`/`Classic`, consumed before parsing |

**The eval scope map grants nothing.** It is data the verifier admitted with the caller; it names
slots the caller could already reach and contains no code, so no route gains a way to compile.
**A composition that registers no provider still refuses every route before any payload is read**, and
the new payload changes nothing about when the mediator charges, bounds depth or converts nested
failures.

**The one new hazard is a provider that does not know the mark.** JSeal's provider decodes every
payload as source (it already does that to a module request) and copies the capability version from
the profile's own descriptor, so a version bump alone would not stop it. Three measures together
close it: `SourceProviderCapability` moves to version 2 with the signature text
`(source-request)->artifact` and a documented obligation to dispatch marks `0x00` and `0x01` and to
refuse any other leading control byte as `MalformedEncoding`; the answer binding of section 5 turns a
provider that compiled the bytes as a Script into an explicit `EvalError`; and every first-party
provider in this repository (the end-user host, the conformance harness, the polyglot host and the
check hosts) adopts the mark in the same step. **What remains is a provider that refuses the marked
bytes as a syntax error, which the guest would see as a `SyntaxError`**; that is a provider contract
breach at version 2 and it is why JSeal's adoption belongs to the pin update that takes step 3, not to
a later one.

## 9. The implementation steps, in order

Each step is independently shippable, adds a focused differential probe and names its Test262
subset; none removes a refusal before the shape it covers is verified.

**V14:**

1. **Refusal hygiene, no new semantics.** Refuse direct `eval` at module top level with the existing
   `EvalError`. **Refuse a direct `eval` in a program body whenever the frame's current record is not
   the record the program body was entered with** - exactly when a block, `for`-`let`, `switch`,
   `catch` or `with` record lies between the site and the program - with the same `EvalError`; the
   executor sees this at `CallEval` without a new operand, and a block that pushes no record binds
   nothing the global path could miss. Add `CallEvalSpread`, which performs a direct evaluation only
   where `CallEval` would and refuses everywhere else, instead of the silent indirect evaluation
   `CompileArguments` emits today. **This reverses the choice that method's comment records** (no
   opcode for a spelling the corpus does not use): the pinned
   `test/language/expressions/call/eval-spread*.js` cases exercise the spelling, and the card prefers
   a refusal to a plausible wrong answer, which a silent global evaluation of a call the specification
   calls direct is. Emit `CallEval` rather than `Call` when a `with` record lies between an `eval`
   call site and a static binding of the name, so the executor's identity check decides - and, inside
   a function, refuses.
2. **Format.** Section kind 13, the two tables, the `EvalCode` flag and the five name instructions in
   the wide opcode set (never the numeric one); verifier rules of section 4 with malformed-corpus
   entries for each; no producer yet, so every artifact is still refused where it was.
3. **Request vocabulary.** `EvalRequestMark`, `TryReadEvalRequest`, capability version 2 and the
   first-party providers' dispatch. The engine does not send the mark yet.
4. **Compiler.** Site rows for function-scope sites and for program-body sites under a record;
   `UsesArguments` true for a non-arrow function containing a direct `eval` in its own code or in an
   arrow nested in it through arrows only; the eval goal and its lowering; the declaration row.
5. **Executor.** A `CallEval` with a site row (function scope, or a program body under a record) sends
   the eval request, checks the answer
   binding, enters with the eval view and the caller's frame values, and runs the five instructions.
   Strict eval and eval lexicals are admitted; **a sloppy evaluation that declares a `var` or a
   function is refused** from its declaration row with an explicit `EvalError`, at a program-body site
   as in a function. Baseline handler slots
   for the new opcodes. This is where the card's first acceptance clause becomes true.

**V15:**

6. **Sloppy introduction.** The eval-variables record, its `Shadowable` lowering in the caller, and
   deletable bindings; closures over introduced bindings. Unless step 7 lands in the same change, an
   introduced name that appears in any lexical row between the site and the variable record is
   refused with an `EvalError`, so no binding is created where step 7 would have thrown.
7. **Conflicts.** All of `EvalDeclarationInstantiation`'s checks before any creation, including the
   Annex B.3.4 catch exemption.
8. **Global and indirect eval on the eval goal.** Program-body direct `eval` with no site row and
   indirect `eval` move to the eval request with global flags; a program-body site with a row gains
   sloppy `var` and function introduction into the global variable environment, after step 7's
   conflict walk over its rows (the catch-clause exemption included). Together these fix the global
   rows of section 1 and parity section 4.6: lexical isolation, strict `var` isolation, inherited strictness, configurable global
   `var`s and the global conflict checks. **JSeal's host scripts ride this path**, so its provider's
   forced strictness moves to the request's strict flag in the same pin update.
9. **The remainder V15 names.** Parameter-initialiser evaluation with its separate record, Annex B
   function-in-block hoisting inside eval code, `arguments` against V05-V06's mapped object, thrown
   parse errors, and the bounded pinned subset: `test/language/eval-code`,
   `test/language/expressions/call/eval-*`, `test/built-ins/eval`. The blanket function-scope refusal
   is withdrawn only for shapes this step verifies; `super`, private names and module-level evaluation
   remain refused by name.

## 10. Memory and fuel

- **Artifacts.** Only artifacts with a site pay for the section, in proportion to the names visible
  from the distinct scopes that contain one. The verifier's work is linear in the section and counted
  under the same verifier-work bound as the rest of the artifact, including the nested bound for an
  eval artifact that itself contains a site.
- **Per call of a function containing a direct `eval`:** one `arguments` object always, and from step
  6, in sloppy code, one eval-variables record whose bindings are allocated only when an evaluation
  declares one. Introduced bindings are reported through `Retain` as a collection entry is, since
  their number is guest-controlled; it is bounded by source bytes the evaluation already paid for.
- **Per evaluation:** the existing `1 + bytes` charge, the mediator's host-call unit and nesting
  depth, verification, and a fresh verified artifact kept alive by any closure it created. **Nothing is
  cached** (V15 excludes it), so a loop that evaluates the same text pays every time, as today.
- **Per name instruction:** one fuel unit per record walked, as `ResolveName` charges, plus the
  ordinary charges of any property operation on a `with` object; bounded by the 255-record scope
  ceiling. `EvalDeclarationInstantiation` charges per name checked and per record walked, so no
  unmetered loop is added. Cancellation is observed through the same charge polling.
- **Caller-side cost (from step 6):** every free name in a sloppy function containing a direct `eval`,
  and in its nested functions, pays the `with`-shaped search. Functions without one pay nothing, which
  is the property the static model exists for and which this design keeps.

## 11. Native-form compatibility

- **The numeric form is untouched**: its front end refuses `eval` by name, so no artifact of it can
  carry the section, and the five instructions are not in its closed instruction set.
- **The baseline form needs a defined handler slot for each new opcode**, or it answers `Defect`.
  They reach guest code through `with`-object lookups and proxies, and the site of any evaluation
  nests an activation, so `CallEvalSpread` and all five name instructions are **classified in
  `JsBaselineBlocks.RunsAlone`** rather than argued into a block against that method's falsifier.
  `NativeTemplateScanChecks` gains their rows. None of them has a code target, so the compare tree of
  landing offsets does not change; re-emission equality holds because emission stays a function of
  the bytecode.
- **The eval entry is set up in managed code for both forms** - the view, the frame values and the
  declaration instantiation happen before the first instruction - and `RequireInstanceForm` already
  refuses an eval artifact of the other form, so the provider must compile eval code in the caller's
  form, as it does today.
- **Both forms run the probe of every step.** The observation in section 1 that the native form fails
  this subtree at instantiation with checkout `484f389` - and fails unrelated subtrees the same way -
  has to be understood before a native count is quoted for any step.

## What this refuses to do

- **Rejected: global evaluation in place of the refusal.** The card excludes it and section 1's
  table shows what it would answer.
- **Rejected: compiling eval code against a serialised caller shape**, so that its free names become
  static `(hops, slot)` pairs. It would make eval code faster, and it would put the caller's slot
  numbers in the payload, trust the provider's answer to address them, and require the executor to
  prove the artifact was compiled for this site's shape. Run-time lookup through a verified map keeps
  the only trusted description on the caller's side; if profiling after V15 shows the lookups matter,
  this is the optimisation to revisit, with the shape bound by digest.
- **Rejected: dictionary environments for every function that contains an `eval`.** A second
  environment model beside the slot model would double what every binding instruction means; the
  eval-variables record confines the by-name part to bindings an evaluation created.
- **Rejected: format version 3.** [JSD-0021](0021-the-wide-bring-up-manifest-and-format-version-2.md)
  says adding a section kind to a *frozen* format is what a version break is for. Version 2 is the
  wide bring-up surface's, and section kinds 9 to 12 were added to it as optional kinds. **If JS-5 freezes
  version 2 before step 2 lands, this section goes into a version break instead**, and that is the
  condition that would overturn this choice.
- **Rejected: a second artifact provider for eval.** The core binds one provider per profile; a
  second would need a core amendment and would split the permit into two places to keep in step.

## What would falsify this design

A direct `eval` whose answer differs from the pinned specification on any case in section 6 that its
step claims; any route that compiles source without asking the provider; a name instruction that
reaches a compiled slot the site row does not name; or an intermediate step whose probe shows a wrong
answer where the step before it showed a refusal.

## 12. What JSeal V14 implemented, and where it differs from the text above

Recorded 2026-09-21 by the implementing slice, not by the owner; it changes no decision above and
signs nothing. Evidence: VM `docs/evidence/jseal-v14/README.md`.

- **Steps 1-5 landed in order**, each checked before the next: the refusal hygiene (module top level,
  a script body whose current record is not its entry record, `CallEvalSpread`, `CallEval` for an
  `eval` with a `with` record in front of its binding); section kind 13 with its three tables, the
  `EvalCode` flag, the five name instructions (`0x91`-`0x95`, `CallEvalSpread` is `0x90`) and the
  verifier rules of section 4; the `0x01` eval request, `JsFormat.TryReadEvalRequest`, source-provider
  capability version 2 (signature `(source-request)->artifact`, minimum provider version 2) and every
  first-party provider's dispatch through one shared reader, `JsCompiler.TryReadProgramRequest`; the
  site rows, the eval goal (`SliceParseOptions.Eval`) and its lowering; and the executor.
- **One request flag more than section 5 lists**, `InClassBody`: an enclosing class declares private
  names the map does not describe, so an evaluation that names a private name it does not declare is
  refused with an `EvalError` there and is the `SyntaxError` the language gives everywhere else.
- **A refusal code on the declaration row.** A sloppy `var` or function declaration (steps 6-8), a
  reference to the caller's `super` (no step) and a caller's private name (no step) compile, and the
  executor refuses the program by name before its first instruction; they are not syntax errors.
- **Two kinds of site get no row and keep their refusal**: a site whose chain reaches a module's
  record (section 6 already refuses the module top level; a function inside a module is refused the
  same way, because imports are not slots), and a site in a parameter initialiser (step 9).
- **Eval code is not a program body.** Its entry unit is named `eval`, carries `EvalCode` and the
  caller's strictness, and has no `ProgramBody` flag; a site inside it always has a row.
- **Verifier details the section does not spell out**: a site's strictness must equal its unit's; a
  strict request must produce a strict unit (a sloppy one may produce a strict unit, through the
  source's own directive); a site is found by decoding its unit from its first byte, so a site in
  unreachable code is still checked; the depth is held to the abstract pass's where the pass reaches.
  Two core codes carry the refusals, `EvalScopesOutsideManifest` (1626) and `MalformedEvalScopes`
  (1627), in diagnostic registry revision 13, each reached by retained `eval-scopes-*` entries.
- **The executor answers a source that begins with U+0000 to U+0008 or U+000E to U+001F with a
  `SyntaxError` without sending it**, since no program begins with one and the first two are the
  marks.
- **`arguments` is materialised by every non-arrow function that mentions the name `eval`** in its
  own code or its arrows, which over-approximates "contains a direct eval" by the mentions that are
  not calls.
- **The public surface grew, deliberately, and `docs/api/public-api.txt` records it**: in the format
  assembly `SectionKind.EvalScopes`, `FunctionFlags.EvalCode`, `EvalScopeKind`, `EvalRequestFlags`
  with `EvalRequestFlagBits`, `EvalRefusal`, `EvalBindingImmutable`, `CeilingEvalScopeRows`,
  `EvalRequestMark`, `EvalRequest`, `TryReadEvalRequest`, `StartsWithReservedMark`, the four row
  records and `JsArtifactWriter.EvalScopes`, and the six opcodes; in the profile the two diagnostic
  codes; in the lowering assembly `SliceParseOptions.Eval`, `IsEval`, `EvalFlags` and
  `JsCompiler.TryReadProgramRequest`, which is the dispatch a version-2 provider owes. JSeal's
  provider copies the profile's capability version, so the pin update that takes this change must
  adopt that dispatch in the same change; without it a direct eval there is answered as a
  `SyntaxError` instead of the `EvalError` refusal.
- **The script top level with nothing in between keeps the old global path** until step 8, as
  section 6 plans: the rows of section 1 for eval lexicals leaking as global lexicals, a `var`
  conflicting with a global lexical, and strict top-level evaluation (strictness not inherited) still
  answer wrongly and silently, and eval-created globals are not configurable. Steps 1-5 turned only
  the other silent answers of section 1 into refusals or right answers.
- **Native form**: the six opcodes have baseline handler slots and are classified in
  `JsBaselineBlocks.RunsAlone`; a caller artifact emitted for the baseline form verifies. Native
  execution was observed only in process, in three new rows of the check host where the
  x86-64-win64 baseline form agrees with the interpreter; through the end-user host and the
  conformance harness every native artifact on the implementing machine fails to instantiate
  (section 1).

## 13. What JSeal V15 implemented, and where it differs from the text above

Recorded 2026-09-21 by the implementing slice, not by the owner; it changes no decision above and
signs nothing. Evidence: VM `docs/evidence/jseal-v15/README.md`.

- **Steps 6-9 landed together.** A sloppy evaluation's `var` and function declarations are its
  caller's: the executor runs the part of `EvalDeclarationInstantiation` that reaches outside the
  evaluation before its first instruction - every check first, over the caller's verified map, then
  the new bindings - and the evaluated program writes its function objects, and its hoisted Annex B
  aliases, to the variable environment with a new instruction. The conflict walk follows the
  pinned ES2026 text: the catch-clause exemption is per record (a destructured parameter is exempt
  too), and Annex B hoisting is decided per evaluation, against the caller's bindings and, at the
  global environment, `CanDeclareGlobalVar`.
- **The eval-variables record is held by the function record, not placed beside it** (section 6
  placed it immediately outside). `JsEnvironment.EvalVariables` is a prototype-less object created by
  the first evaluation that declares something in the function; a name is found in the record's
  slots first and there second, which is the order an outer record would give because an
  evaluation never introduces a name the function record binds. No record moves, so no
  `(hops, slot)` pair changes. The lowering marks every sloppy function (arrows included) whose own
  code mentions `eval`, and `Shadowable` treats it as a search point for every name it does not
  bind; `ResolveName` asks the object by own property, without `Symbol.unscopables`.
- **Two instructions more than section 5 lists**: `WithBaseObject` (`0x9A`) gives a call through a
  name the eval variables answered an `undefined` receiver, so the object never reaches guest code,
  and `StoreEvalVariable` (`0x9B`, the eval name instructions' `u8 u16` shape) writes the variable
  environment directly and only for names the instantiation declared. The first is an ordinary block
  instruction of the baseline form; the second runs alone (at the global environment it is a `Set`).
- **Two binding flags and two declaration lists.** `EvalBindingLexical` (2) marks the names a
  declared `var` collides with: a function row's top-level `let`, `const` and `class` (this lowering
  keeps them in the function's record), every name of a block or evaluation row, and a catch row's
  names after its parameters. `EvalBindingHidden` (4) marks, in the row a parameter-initialiser site
  sees, the names the body declares. A declaration row now carries the `var` names, the function
  names in initialisation order and the Annex B candidates separately (`FunctionNameConstants`,
  `AnnexBNameConstants`); a strict unit whose row names any is refused by the verifier.
  `EvalRefusal.VarDeclarations` stays defined and is no longer written.
- **Step 8 is the eval goal for global evaluations.** An indirect `eval`, and a direct one at a
  script's top level with no record around the call, send an eval request (strict only for a
  strict script's direct call) and run against a boundary record with no parent and a view of the
  global scope. The `Function` constructor keeps the script path: its assembled source declares
  nothing at its top level, so the two goals cannot differ for it.
- **Step 9: a parameter-initialiser site gets a row of its own.** The function's row as its
  parameter list sees it carries the parameters and the `arguments` object as colliding bindings
  and hides the body's declarations; the function record is the variable environment, and the
  eval variables it gains are searched by the later parameters and by the body. `arguments` is
  materialised when a parameter list mentions `eval`. **Two shapes keep their refusal**, because
  this lowering keeps one record for a parameter list and its body where the specification's
  `FunctionDeclarationInstantiation` gives the body a separate variable environment once the list
  has expressions. A direct eval in the body of a function whose parameter list mentions `eval` gets
  no row, whether or not the list makes closures in its own syntax: the evaluated source can make
  them too, and such a closure would see the body evaluation's `var`. A direct eval in the parameter
  list of a function whose body declares a parameter's name - or a bound `arguments` - again gets no
  row either, when the redeclaration is a function or a `var` the body (its nested functions
  included) writes or mentions: a closure that evaluation made would see the body's assignments to
  the shared slot instead of the parameter. A bare `var a;` nothing else in the body names cannot be
  told apart from the parameter and stays admitted.
- **A locally bound `eval` is a direct call site too.** The language decides directness by the
  reference's name and the callee's identity with %eval%, not by where the name resolved, so
  `CompileCall` now emits `CallEval` for every bare identifier `eval` and the executor's identity
  check decides; `var eval = globalThis.eval; eval(s)` in a function evaluates in the function's
  scope. Section 1's "a locally bound `eval` is an ordinary `Call`" no longer holds.
- **Artifacts written by the V14 lowering are not gated.** Their maps carry no `EvalBindingLexical`
  flags and their functions no eval-variable search, and nothing in the format tells them apart,
  so under this executor an evaluation against one misses a lexical conflict and its closures miss
  the names the evaluation introduced. The V14 lowering was never an accepted format; the retained
  corpus entries compiled from source with a direct eval were rewritten, and a V14 artifact must be
  compiled again.
- **Still refused by name**: `super` and private names in evaluated source, and every site whose
  scope reaches a module (the design has no row kind for imports).
- **A defect of V14's lowering fixed on the way**: a directive's string is eval code's completion
  value (`eval("'1'")` is `"1"`), as it always was for a script.
- **Consumers**: JSeal runs host scripts through the captured `eval` intrinsic, which is indirect
  and therefore now global eval code rather than a script - its top-level `let` and `const` no
  longer persist between host scripts, and its forced strictness has no place in an indirect
  request. The pin update that takes this change must move host scripts to a script-goal route or
  accept eval semantics explicitly (section 8 anticipated the strictness half). **No such route
  exists yet**: `JsHostRealm` has no script entry, and the executor refuses a script-goal answer to
  an eval request. JSeal's pin update is therefore blocked until a deliberate host entry for
  script-goal evaluation in an existing realm is added under JSD-0024 and the API baseline - work
  this slice does not do.
  *(Addendum 2026-09-21, JSeal V15-host, recorded by that slice and not by the owner: the entry now
  exists - `JsHostRealm.EvaluateScript`, [JSD-0024](0024-the-in-realm-host-surface.md) section 16 -
  and the executor's refusal of a script-goal answer to an eval request stands, because the route is
  a request of its own rather than an eval request answered differently.)*

## 14. What JSeal V15-finish implemented, and where it differs from the text above

Recorded 2026-09-21 by the implementing slice, not by the owner; it changes no decision above and
signs nothing. Evidence: VM `docs/evidence/jseal-v15-finish/README.md`.

- **A function body gets a variable environment of its own** - FunctionDeclarationInstantiation
  step 28 - wherever the difference is observable: a non-simple parameter list that makes a closure
  (a function, arrow or class expression, an object literal's method, or a mention of `eval`, whose
  source may make one), or a sloppy body that mentions `eval`, since a body evaluation's `var` of a
  parameter's name is a new binding of the body's environment. The unit pushes the record with
  `PushScope` after the parameter list (after `EnterBody` in a generator); a body `var` of a
  parameter's name, `arguments` included when the object is bound, starts with the parameter's
  value. Every other function keeps the one record, and a call pays for no second record: a
  call-heavy loop over simple and defaulted parameter lists shows no difference beyond the
  machine's noise (evidence).
- **Section 13's two parameter-list refusals are gone.** A body site's chain passes the body's
  record, a new row kind `EvalScopeKind.FunctionBody = 7`, which the executor takes as the variable
  environment - its slots, then its own eval-variables set - before the parameters' `Function` row;
  a parameter-list site's row is the parameters' record, which the body's redeclarations never
  reach. The verifier admits the new kind only as the outermost record a function unit pushes.
  A parameter-list site in a function the closure walk did not find making a closure (a node the
  walk cannot see into) still gets no row and keeps the refusal. `EvalBindingHidden` stays defined;
  the lowering now has only temporaries past a parameter record's parameters, and it writes none.
- **`super` in evaluated source is its caller's.** The eval frame is entered with its caller's active
  function, whose `[[HomeObject]]` a `super` property reference and whose constructor a `super()`
  call already used, so the only change is that the lowering no longer writes
  `EvalRefusal.SuperReference`; the syntactic permissions stay those of section 5's request flags.
- **Private names are read through the map.** The eval scope map now writes a class's private-name
  slots (`##` and the name), which no identifier can spell; eval code in a class body lowers a private
  name it does not declare to `LoadEvalName` of that spelling, and the declaration row lists every
  such name in a new, fifth list, `PrivateNameConstants` (encoded after the Annex B list; the
  verifier requires each to be an interned name beginning `##`). Before the program's declarations
  are instantiated the executor resolves each through the caller's map and, when no class around the
  call declares it, throws the `SyntaxError` `AllPrivateIdentifiersValid` makes it. That check walks
  the declarative rows alone: it passes over a `with` row without asking its object and ignores names
  an evaluation introduced, so the early error runs no guest code and no guest code sees the `##`
  spelling (an adversarial review found the first version asking a `with` proxy around the class); a global
  evaluation listing any throws the same. `EvalRefusal.SuperReference` and `PrivateName` stay defined,
  as `VarDeclarations` did, and are no longer written.
- **Format**: no opcode and no diagnostic code was added; the eval scope section's declaration row
  gained one list, which changes the section's encoding - an artifact written before this change
  that carries a declaration row does not verify, and its eval code must be compiled again (only
  eval-code artifacts carry declaration rows, and none is retained). The public surface grew by
  `EvalScopeKind.FunctionBody` and `JsEvalDeclarationRow.PrivateNameConstants`, recorded in
  `docs/api/public-api.txt`.
- **Still refused by name**: every site whose scope reaches a module. *(Answered since JSeal
  V15-module, section 15.)*

## 15. What JSeal V15-module implemented, and where it differs from the text above

Recorded 2026-09-21 by the implementing slice, not by the owner; it changes no decision above and
signs nothing. Evidence: VM `docs/evidence/jseal-vm-module-gaps/README.md`.

- **A module's environment is a row kind, `EvalScopeKind.Module = 8`.** It is the root of every
  chain inside a module, as a program row is inside a script. Its names are the module's slots,
  except the `export default` slot, which no source can spell. After them come the module's imports,
  each flagged with a new binding flag, `EvalBindingImport = 16`, always together with
  `EvalBindingImmutable`. An import's "slot" is its entry in the artifact's import table. The
  executor reads it through the exporting module's environment on every access, as `LoadImport`
  does. So an evaluation sees a live binding and its dead zone. A write is the `TypeError` every
  assignment to an import is; a `delete` of one cannot be written, since the evaluation is strict. After the module row, resolution goes
  to the realm's global lexical half and the global object, as after a program row.
- **A module-level site always has a row**, even with no block around the call: at a module's
  top level the caller's scope is the module's, not the global one. JSD-0026 step 1's refusal
  now meets only an artifact written without the row.
- **Nothing new is needed for strictness.** Module code is strict, so the request is strict: the
  evaluation's `var`s and functions stay in its own record, `with` is a `SyntaxError`, and an
  undeclared write throws. Eval code is Script-goal code, so `import`, `export`, `import.meta`,
  `new.target` and `await` in it are `SyntaxError`s (Test262 `eval-code/direct/export.js`,
  `import.js`, `expressions/import.meta/not-accessible-from-direct-eval.js`).
- **The verifier** admits the kind as a chain root only under a module body. It admits the import
  flag only on a module row, and only with the immutable flag. It bounds an import's entry by the
  artifact's import table, exactly as a `LoadImport` operand is bounded; every other slot stays the
  executor's to bound. Two retained corpus entries reach the new refusals:
  `eval-scopes-an-import-outside-a-module-row` and `eval-scopes-a-module-row-under-a-script-body`,
  both `MalformedEvalScopes` (1627).
- **Format**: no opcode, no diagnostic code and no decision number was taken. The public surface
  grew by `EvalScopeKind.Module` and `EvalBindingImport`, and `EvalBindingFlagBits` changed from 15
  to 31. `docs/api/public-api.txt` records all three. An artifact written before this change still
  verifies, and its module sites keep their refusal.
