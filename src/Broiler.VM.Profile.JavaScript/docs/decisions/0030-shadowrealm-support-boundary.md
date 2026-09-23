<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0030 - The ShadowRealm support boundary, and why a child realm inherits its parent's compilation policy

**Status:** Proposed, owner decision pending. 2026-09-21. This record recommends a deferral and
fixes the boundary any later implementation must hold. It does not take itself: the owner's act of
taking it, and the two choices in section 8, are outstanding.

**Owner:** MaiRat. **Co-signer:** none. If the record is taken, **both roles are held by one
person**, and it will not claim the co-signature is independent - there is no second signature to
claim it of.

**Milestone:** none of this profile's. It answers card D04 of the Broiler.JSeal cross-repository
plan (`docs/roadmap.vm-features.md` in that repository) and is read against
[section 13](../roadmap.md#13-realms-agents-and-the-host-boundary), which says a realm is this
profile's object and that one instance may hold several.

**Context.** Broiler.JS ships a `ShadowRealm`; this profile does not. The JSeal review listed
ShadowRealm among the gaps that need a scope decision rather than an implementation, and the card
asks for one that pins a specification, defines isolation, wrapping, the values that cross, the
compilation policy, lifetime and modules, and separates the API from workers and structured clone.
Its one hard acceptance clause is the reason this record exists at all: **a restricted parent cannot
regain dynamic compilation simply by constructing a child realm.** Its one exclusion is the easy
wrong answer: reusing one global object and calling it isolation.

---

## 1. Current behaviour, verified

| Question | Answer | Where it was read or run |
|---|---|---|
| Does the profile define `ShadowRealm`? | **No.** `typeof ShadowRealm` is `"undefined"` and `"ShadowRealm" in globalThis` is `false` under the default wide surface | CLI probe, this worktree's Release build |
| Can a program create a second realm any other way? | **No.** `$262.createRealm` exists only to refuse: "this profile creates no nested realm" | `JsRealm.Global.cs:231` |
| How many realms does an instance hold? | **One.** `JsEngine`'s constructor builds exactly one `JsRealm` and exposes it as a get-only `Realm` | `JsEngine.cs:109`, `JsEngine.cs:490` |
| Does a function know its realm? | **No.** `JsFunction` carries a name and an arity and no `[[Realm]]`; only `JsProxy` holds the realm that made it | `JsFunction.cs:153`, `JsProxy.cs:73` |
| Where do error objects, wrappers and prototype lookups come from? | From the engine's one `Realm` (`Realm.CreateError`, `Realm.NumberPrototype`, ...) | `JsEngine.cs:804`, `JsEngine.cs:935-938` |
| Are well-known Symbols and the `Symbol.for` registry agent-wide? | **No, per realm**, and the file says so and says it becomes observable "the day an agent model exists" | `JsRealm.Symbol.cs:32-43` |
| What gates compilation from a string? | Two layers. The realm builds `eval` and `Function` only if the composition admitted `broiler.javascript.dynamic`; every compilation then goes to the mediator the current invocation supplied, which the engine holds only for that invocation's extent and reaches only through `Loader` ("set per invocation and cleared after it"), and so to the one provider the composition registered, which may refuse it, which the guest sees as a `SyntaxError` | `JsRealm.cs:197`, `JsEngine.cs:280`, `JsEngine.cs:295`, `JsEngine.cs:331-395` |
| Where does JSeal's `AllowGuestEval` live on the VM? | In one `VmSourceProvider` per runtime: a single host permit, consumed before decoding, and every request without it is a guest request refused when guest evaluation is off | `Broiler.JSeal.Vm/VmSourceProvider.cs:47-58`, `VmEngineProvider.cs:51` (JSeal) |
| Where is the host-object seam? | One `JsHostRealm` per engine, over the one realm | `JsEngine.cs:466`, [JSD-0024](0024-the-in-realm-host-surface.md) |
| Is there an agent-wide job queue? | Yes: one microtask queue per engine, which a second realm on the same engine would share as the specification's single agent queue requires | `JsEngine.cs:113-139` |

**Broiler.JS, for comparison.** `JSShadowRealm` creates a fresh `JSContext` as the child realm.
Commit `e0bb15b7` (2026-09-14) made `evaluate` raise `EvalEvent` on the context that constructed the
ShadowRealm before compiling (`JSShadowRealm.cs:68`), so a refusing parent can no longer compile
into a child through `evaluate`. `importValue` throws `TypeError` "not implemented"
(`JSShadowRealm.cs:126`). **Code already running inside the child is not covered:** the global
`eval` dispatches on the current context (`JSGlobal.cs:124`), which inside a ShadowRealm is the
child, and nothing subscribes there. A probe against the Broiler.JS Release build (built after
its HEAD, 2026-09-19) with a parent handler that admits exactly the source `(s) => eval(s)` and
refuses every other string:

```text
parent-eval:refused | evaluate:refused | child-eval:42
events: 1+1 ; 2+2 ; (s) => eval(s)
```

The child compiled `6*7` and the parent's handler never saw it. JSeal's own policy is all or
nothing, so under JSeal the route is unreachable, as `BroilerJsRealm.cs:87-91` (JSeal) already
records. An embedder with a selective handler - a CSP hash allowlist is the obvious one - is exposed.
This is the failure the card's acceptance clause names, reached one step later than the card
imagined, and it is why section 4 inherits the policy for every route rather than for `evaluate`.

---

## 2. The pin

| Artifact | Pinned at | What it says |
|---|---|---|
| Proposal | `tc39/proposal-shadowrealm` at **`9ff2a01f1ec68a427f6f37ce710aa7cba8150678`** (2025-02-10, "The correct stage (#416)"), the head of `main` when read on 2026-09-21 | **Stage 2.7**, rendered spec dated "Stage 2.7 Draft / February 10, 2025". Stage history: 3 (2021), back to 2 (2023-09), 2.7 (2024-02) |
| Language edition | ES2026, pinned by [JSD-0019](0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md) | **No ShadowRealm** (zero occurrences in the archived `ecma-262-es2026-spec.html`). `HostEnsureCanCompileStrings` exists and is asked for `eval` and `CreateDynamicFunction` |
| Conformance suite | test262 `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, pinned by [JSD-0020](0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md) | `test/built-ins/ShadowRealm` holds **67 files**: 8 constructor, 2 prototype, 37 `evaluate`, 15 `importValue` (3 of them `_FIXTURE`), 5 `WrappedFunction` - 64 cases: 12 `importValue` and 52 others. Three of the 52 (`prototype/evaluate/throws-error-from-ctor-realm.js`, `wrapped-function-proto-from-caller-realm.js`, `wrapped-function-throws-typeerror-from-caller-realm.js`) also claim `cross-realm` and call `$262.createRealm()`. `features.txt` lists `ShadowRealm` in its **proposed** section and `cross-realm` (line 136) in its test-harness section |

Because the suite calls it a proposal, [JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md)
already excludes every one of those 64 cases from a scored run. **Nothing in this record changes
that.** An implementation that wants them scored has to admit the flag by a record of its own. Once
it does, the three `cross-realm` cases still do not score: JSD-0018 counts a case that claims a
test-harness feature as **unselectable**, and `$262.createRealm` refuses (D1). Admitting the flag
also ends JSD-0018's measured fact that no scored case claims a test-harness feature, so the record
that admits it has to re-measure and restate that fact. That is why section 7 counts 49 and 12,
not 52 and 12, and gives the three their own slice (SR-7).

The proposal semantics this record relies on, from the pinned revision: `evaluate` asks
`HostEnsureCanCompileStrings(evalRealm, « », sourceText, false)` before parsing; an abrupt
completion inside becomes a fresh `TypeError` of the **caller's** realm; `GetWrappedValue` passes
primitives, wraps callables through `WrappedFunctionCreate` (prototype from the caller realm's
`%Function.prototype%`, `length` and `name` copied by `CopyNameAndLength`), and throws `TypeError`
for every other object; the constructor builds the realm with `InitializeHostDefinedRealm` and
tells the host with `HostInitializeShadowRealm`; the child's global object must be an extensible
**ordinary** object; `importValue` returns a promise and loads through `HostLoadImportedModule`.

---

## 3. What was considered

| Option | What it does | Why it was not taken |
|---|---|---|
| **Implement now** | Constructor, `evaluate`, wrapped functions, then `importValue` | The proposal is at 2.7, not in the pinned edition, and its suite cases are excluded from scoring, so the work would be graded by nothing this component counts. Two prerequisites (section 7, SR-1 and SR-2) change the engine's realm model underneath every built-in, which is a large change to buy an API with no consumer named in any ledger |
| **A stub** | Define `ShadowRealm` and throw from the constructor or from `evaluate` | Feature detection (`typeof ShadowRealm === "function"`) would lie. JSeal's cross-provider test already passes a provider "by not defining it", and a stub would turn that into a failure explained by nothing |
| **One global, reused** | `evaluate` runs in the parent realm, or in a fresh global over the parent's intrinsics | The card's exclusion, and it breaks the specification's first observable property: `sr.evaluate("Array") !== Array` in any conforming engine |
| **A separate core runtime per child** | Build the child as a worker-style agent | Section 13 says an agent is a runtime and a realm is not. It would put a synchronous same-thread call across a runtime boundary and give the child a budget of its own, which is the opposite of what section 5 wants |
| **Defer, and fix the boundary now** | Keep the feature absent; write down what an implementation must hold so a later slice cannot quietly relax it | **Recommended** |

---

## 4. The decision

**D1. Absent, not stubbed.** Until the follow-ups in section 7 land, `ShadowRealm` stays undefined
and `$262.createRealm` keeps refusing (SR-7 is the one slice allowed to change the latter). No partial constructor ships.

**D2. The pin is section 2.** An implementation targets proposal revision `9ff2a01f` and is
measured against the 64 cases of test262 `ccaac100`; moving either is a new record.

**D3. Isolation is a whole realm.** A ShadowRealm owns a new `JsRealm` with its own intrinsics and
its own extensible ordinary global object. Nothing of the parent's global, intrinsics or host
objects is visible in it. The only agent-wide state is what the specification makes agent-wide:
the well-known Symbols, the `Symbol.for` registry, and the job queue.

**D4. Only primitives and callables cross.** Every value that crosses in either direction - an
`evaluate` completion, a wrapped function's `this` value and arguments (crossing inward), its return
value (crossing outward) - goes through one `GetWrappedValue`:

- a primitive, Symbols included, crosses as itself;
- a callable - an ordinary function, a bound function, a callable Proxy, another wrapped
  function - crosses as a **new** wrapped function each time, so wrappers never carry identity;
- every other object, including a host object installed through [JSD-0024](0024-the-in-realm-host-surface.md)'s
  seam, is refused with a `TypeError`. For an `evaluate` completion that is a `TypeError` of the
  realm that called `evaluate`; inside a wrapped function's `[[Call]]` it is a `TypeError` of the
  **caller realm, the wrapper's `[[Realm]]`**, including when the refused value is a `this` or an
  argument crossing inward (the pinned text: every exception produced after the start of
  `[[Call]]` is associated with the caller realm; Node answers `add({}, 1)` with the parent's
  `TypeError`);
- an exception crossing out becomes a fresh `TypeError` of the caller realm. The inner error object
  never crosses. If its text is copied, it is read without invoking guest code, or not copied.

**D5. A child realm inherits its parent's compilation policy, on every route, by construction.**
This is the acceptance clause, and it is held structurally rather than by checks:

- **One mediator.** A child realm has no `Loader` of its own. `evaluate`, the child's own `eval`,
  `Function` and generator/async constructors, `import()`, `importValue`, and a ShadowRealm nested
  inside the child all request compilation through the mediator the current invocation supplied,
  reached only through the engine's `Loader` (`JsEngine.cs:280-295`, set per invocation and cleared
  after it), and therefore through the one provider the composition registered - under JSeal, the one
  `VmSourceProvider` whose guest refusal is `AllowGuestEval`.
- **Always a guest request.** Every one of those requests is a guest request. A host permit
  (JSeal J04: armed immediately before the host's own compilation and consumed by it) cannot be
  consumed by `evaluate`, because the host's source has already consumed it by the time any guest
  code, including a `new ShadowRealm().evaluate(...)` inside that source, runs.
- **Same surfaces, never wider.** The child is built only where the parent admits
  `broiler.javascript.dynamic`, and it admits exactly the parent's surface set. No API, public or
  internal, constructs a child with a different provider, a different surface set or a
  different strictness.
- **Asked about the child, answered by the parent.** `HostEnsureCanCompileStrings` names the child
  realm; the answer is the policy that governs the parent's instance. A refusal reaches the caller
  as the `SyntaxError` a provider refusal already maps to (`JsEngine.cs:392`).

**D6. Lifetime and cost belong to the instance.** A child realm lives inside the parent instance,
on its thread, inside the operation that is running. It is reachable only through its ShadowRealm
object and the wrappers made from it, and is collected with them. It has no budget of its own:
constructing one and everything it runs are charged to the parent's fuel, live-bytes and call-depth
allowances, and a realm construction is charged in proportion to the intrinsics it builds.
Disposing the instance ends every child realm. Children share the parent's job queue.

**D7. No host surface in a child by default.** `HostInitializeShadowRealm` adds nothing. A
`JsHostRealm` is the parent's; JSeal's `IJsRealm` maps to the parent only; no embedder API reaches
a child realm. Giving a child a host surface is a later decision, not a default.
**Only the parent's explicit act can hand a child a host callable.** JSD-0024's host methods are
ordinary `JsNativeFunction` values, so under D4 a host method the parent passes in crosses as a
wrapper, like any callable; its object never does. A call through that wrapper is a parent-realm
call made inside the running invocation, so it runs inside the host window that invocation opened
(`JsExecution.cs:867/880`, `JsEngine.BeginHostStep`/`EndHostStep`) and is charged to the parent's
budget exactly as a direct call would be. No second window and no second meter exist for a child.

**D8. `importValue` is optional and later.** It waits for the module contract (JSeal I09-I13).
When it lands, each child realm has its own module map, and every load is a guest compilation under
D5. If `evaluate` ships first, `importValue` ships as a refusal with a stable message, and its 12
suite cases are reported as failures, never as skips or passes.

**D9. Not a worker, not structured clone.** A ShadowRealm is a second realm in the **same agent**:
same thread, same runtime, same queue, synchronous calls, and no objects cross at all. Workers
(section 13's agents, JSeal `JsCapabilities.WorkerRealms`, I17) are separate runtimes that exchange
**copies** through structured clone (I14-I18), may run on another thread, and are asynchronous.
Neither is built on the other, ShadowRealm is never advertised under `WorkerRealms`, and
`$262.agent` keeps refusing.

---

## 5. Evaluation examples

Expected results under D3-D4, confirmed against Node v24.17.0 with `--experimental-shadow-realm`
(its V8 follows the same proposal):

| Program (after `const sr = new ShadowRealm()`) | Result |
|---|---|
| `sr.evaluate("6 * 7")` | `42` |
| `sr.evaluate("({})")` | `TypeError`: a non-callable object does not cross |
| `globalThis.leak = 1; sr.evaluate("typeof leak")` | `"undefined"`: separate global |
| `sr.evaluate("Array") === Array` | `false`: `Array` is callable, so it crosses as a new wrapper, never as the parent's `Array` |
| `const add = sr.evaluate("(a, b) => a + b"); add(2, 3)` | `5`; `typeof add` is `"function"`, `add.length` is `2`, its prototype is the parent's `Function.prototype` |
| `add({}, 1)` | `TypeError`: the object argument does not cross inward |
| `sr.evaluate("(f) => f(20) + 1")((x) => x * 2)` | `41`: a parent callable crosses inward as a wrapper |
| `sr.evaluate("() => { throw new RangeError('inside') }")()` | `TypeError` of the parent realm, not the `RangeError` |
| `sr.evaluate("Symbol.for('k')") === Symbol.for("k")` | `true`: the registry is agent-wide (D3) |
| `sr.evaluate("Symbol.iterator") === Symbol.iterator` | `true`: the property `JsRealm.Symbol.cs` would get wrong with two realms today (SR-1) |
| `sr.evaluate("globalThis.f = () => 1; f") === sr.evaluate("f")` | `false`: a new wrapper every crossing |
| `sr.evaluate("globalThis.n = 1"); sr.evaluate("++n")` | `2`: the child realm keeps its state |
| `sr.evaluate("(")` | `SyntaxError` |
| `sr.evaluate(1)` | `TypeError` |

## 6. Policy-isolation examples

Each is an acceptance case for SR-3 (section 7). "Restricted" means the provider refuses guest
compilation - JSeal `AllowGuestEval: false` on the VM provider.

1. **A restricted parent gains nothing by constructing.** `new ShadowRealm()` succeeds, because it
   compiles nothing. `sr.evaluate("6 * 7")` throws `SyntaxError` in the parent, and the provider
   saw one guest request. Node with `--disallow-code-generation-from-strings` gives the same shape.
2. **A host permit is not borrowed.** A JSeal host script whose source is
   `new ShadowRealm().evaluate("1")` runs - its own compilation consumed the permit - and its
   `evaluate` is refused as a guest request.
3. **The child's own compilers ask too.** Under a provider that admits only the text
   `(s) => eval(s)`, `sr.evaluate("(s) => eval(s)")("6*7")` throws `SyntaxError`, and the provider
   saw two requests. This is the case Broiler.JS answers `42` to today (section 1).
4. **So does a nested realm.** Under the same provider, the inner `evaluate` in
   `sr.evaluate("(s) => new ShadowRealm().evaluate(s)")("1")` is refused.
5. **A declined surface declines the child.** A composition that did not admit
   `broiler.javascript.dynamic` has no `ShadowRealm` global, because there is no route by which
   one could evaluate anything.
6. **No host object leaks.** A host object the embedder installed in the parent is absent from
   `sr.evaluate("Object.getOwnPropertyNames(globalThis)")`'s answer, and passing it to a wrapped
   function throws `TypeError`.
7. **A host callable crosses only as a wrapper, and only when the parent hands it over.** With a
   host method `m` of an installed host object `h`, `sr.evaluate("(f) => typeof f")(m)` is
   `"function"`, `sr.evaluate("(f) => f()")(m)` returns `m`'s primitive result, the call runs
   inside the invocation's host step and is charged to the parent's fuel, and `h` itself is never
   reachable from the child (the wrapper's own keys are only `length` and `name`, and passing `h`
   throws `TypeError`).

---

## 7. Bounded follow-up slices

Deferred until the trigger in section 8 is met. SR-1 and SR-2 are prerequisites and change no
observable single-realm behaviour; they may be taken earlier only if another feature needs them.

| Slice | Work | Accept |
|---|---|---|
| **SR-1** Agent-scoped Symbols | Move the well-known Symbols and the `Symbol.for` registry from `JsRealm` to the engine | Contract, Architecture and conformance baselines unchanged; a unit test builds two `JsRealm`s on one engine and shows equal `Symbol.iterator` and a shared registry; the remark in `JsRealm.Symbol.cs` is corrected |
| **SR-2** A running realm | Give every function a `[[Realm]]` and the engine a current realm used where `JsEngine.Realm` is read today; still one realm built | Baselines unchanged; an architecture rule reports any new read of a fixed realm outside construction |
| **SR-3** Constructor, `evaluate`, wrapped functions | D3-D7 behind a surface identity (section 8), built only with the dynamic surface | The 49 non-`importValue`, non-`cross-realm` cases pass when the flag is admitted by record; the 3 `cross-realm` cases are reported unselectable under JSD-0018, not passed, failed or skipped; all seven section 6 cases pass as VM tests; a realm construction's fuel and live-bytes cost is measured and retained |
| **SR-4** JSeal coverage | Extend `ShadowRealmEvaluatesNothingInARealmThatForbidsGuestEvaluation` with section 6 cases 2-4 for both providers | Passes for both providers where `ShadowRealm` is defined and by absence elsewhere; no capability flag is added |
| **SR-5** `importValue` | D8, after JSeal I09-I13 and SR-3 | The 12 `importValue` cases pass (none claims `cross-realm`); a restricted parent's `importValue` rejects without loading |
| **SR-6** Broiler.JS, cross-repository | Make compilation inside a child `JSContext` ask the creator's `EvalEvent` (eval, dynamic functions, nested ShadowRealm) | The section 1 probe answers `child-eval:refused`; filed with the Broiler.JS owner, not a VM slice |
| **SR-7** `$262.createRealm` | After SR-2 (and SR-1), make the harness's `createRealm` build an ordinary new `JsRealm` on the same engine, under D3's rules, and return its `$262`; admitting the `cross-realm` harness feature is a record of its own under JSD-0018 | The 3 `cross-realm` ShadowRealm cases pass once both flags are admitted, bringing the non-`importValue` total to 52; every other scored case that claims `cross-realm` is counted and reported; the refusal message is removed only by this slice |

## 8. What the owner has to decide

- **Take or reject the deferral, and name its trigger.** Recommended trigger: the proposal returns
  to stage 3 at a revision this record's successor pins, **or** a named consumer asks for it. The
  owner has to say whether "a named consumer" is enough on its own.
- **The surface identity.** Recommended: a new manifest identity, provisionally
  `broiler.javascript.shadowrealm`, admitted only together with `broiler.javascript.dynamic`, so a
  composition that wants `eval` can still decline realms. The alternative is to fold it into the
  dynamic surface. Allocation follows [JSD-0002](0002-feature-manifest-allocation.md)'s rule and is
  SR-3's record to make; what is needed to decide is whether any composition needs `eval` without
  realms.
- **SR-6 is outside this component.** It is a finding for Broiler.JS's owner, with the probe above
  as its reproduction.

## 9. What would falsify this

- **A second mediator.** If any code path gives a child realm a `Loader` or provider other than the
  engine's one, D5 stops being structural and becomes something to keep true by review.
- **An object crossing.** If any non-callable object, or an inner error object, is ever observed on
  the other side of the boundary, D4 is broken and so is the isolation that D3 claims.
- **A child that outlives its instance or charges nothing.** Either makes D6 false and hands a
  guest unmetered work.
- **The proposal changing its compilation hook.** If a later revision stops asking
  `HostEnsureCanCompileStrings` for `evaluate`, D5 still holds here, but the pin in section 2 would
  no longer be the text it rests on.
