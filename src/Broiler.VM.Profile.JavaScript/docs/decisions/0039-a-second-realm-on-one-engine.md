<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0039 - A second realm on one engine: the running realm, the agent's Symbols, and `$262.createRealm`

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. Taking the record accepts the realm model in section 2, `$262.createRealm` as section 3
builds it, and the scoring in section 4; not taking it leaves an engine that holds several realms
with no record saying how.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the first record of phase F5 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile), and it builds three of
the slices [JSD-0030](0030-shadowrealm-support-boundary.md) section 7 names: SR-1 (agent-scoped
Symbols), SR-2 (a running realm) and SR-7 (`$262.createRealm`). SR-7 says that admitting the
suite's `cross-realm` harness feature "is a record of its own under JSD-0018"; section 4 is that
record. ShadowRealm itself (SR-3 to SR-5) is not decided here.

**Context.** Until this record an engine built exactly one realm, `JsEngine.Realm` was get-only, a
function carried no realm, and `$262.createRealm` existed only to refuse: "this profile creates no
nested realm". Roadmap [section 13](../roadmap.md#13-realms-agents-and-the-host-boundary) has said
from the start that a realm is this profile's object and that one instance may hold several. The
conformance suite measures cross-realm behaviour through `createRealm` in 281 files: every
`proto-from-ctor-realm` case of the constructors, `ArraySpeciesCreate`'s cross-realm step, the
Symbol registry, `GetFunctionRealm` through bound functions and proxies, and SpiderMonkey's
`newGlobal` tests in `staging/sm`.

---

## 1. What was true before, verified

| Question | Answer | Where |
|---|---|---|
| How many realms did an engine hold? | One, built in the constructor and exposed as a get-only `Realm` | `JsEngine.cs` |
| Did a function know its realm? | No; only a proxy held the realm that made it | `JsFunction.cs`, `JsProxy.cs` |
| Were the well-known Symbols and the `Symbol.for` registry per agent? | No, per realm, with a remark saying the difference becomes observable "the day an agent model exists" | `JsRealm.Symbol.cs` |
| What did `$262.createRealm()` answer? | A `TypeError`, "this profile creates no nested realm" | `JsRealm.Global.cs`, `runs/the-host-members-that-refuse.js` |
| How did the suite's cross-realm cases score? | Selected by the wide dialect and failed: 534 variants over the 281 files, 523 failing and 11 skipped, in the whole run [JSC-263](../roadmap.corrections.md#jsc-263) records | `test262` at `ccaac100` |
| How many embedder views did an engine have? | One `JsHostRealm`, told of the one realm at instantiation | `JsEngine.cs`, `JsExecution.cs` |

## 2. The model (SR-1 and SR-2)

1. **The agent's Symbols are the engine's.** The fifteen well-known Symbols and the registry
   `Symbol.for` and `Symbol.keyFor` share are one object per engine (`JsAgentSymbols`), made before
   the first realm and handed to every realm the engine builds. Every realm's `Symbol.iterator` is
   the same Symbol; a key one realm registered is found by another. The `Symbol` constructor and
   `Symbol.prototype` stay each realm's own.
2. **Every function has a `[[Realm]]`.** A built-in's is the realm that built it; a script
   function's is the realm running when the closure was made; a bound function has none and is
   answered through its target, as the specification's `GetFunctionRealm` does.
3. **The engine has a running realm, and every frame and built-in sets it.** Entering a script
   function's frame - a call, a construction, a generator's or async function's resumption, the
   value form's direct call - makes the function's realm the running one until the frame ends,
   however it ends; calling or constructing a built-in of another realm does the same for the
   built-in's. A script, module or eval body runs in the realm already running, which whoever
   started it chose: an embedder's evaluation enters its view's realm. `JsEngine.Realm` read
   anywhere is therefore the specification's current Realm Record, and every intrinsic a step
   reaches for comes from it.
4. **Where the specification names another realm, that realm is used.**
   - a sloppy function's `this` is its own realm's global object, and a primitive receiver is
     wrapped with its own realm's prototype (`OrdinaryCallBindThis`);
   - a class constructor called without `new` throws its own realm's `TypeError` (10.2.1 step 2);
   - `GetPrototypeFromConstructor` falls back on the intrinsic of `new.target`'s realm when
     `new.target.prototype` is not an object, for ordinary constructions, for built-ins that read
     `new.target` themselves and for built-ins the engine re-points; a revoked proxy on the way is a
     `TypeError`;
   - `ArraySpeciesCreate` treats another realm's `%Array%` as no species;
   - a generator's or async generator's object falls back on its function's realm's prototype;
   - a proxy's `apply`, `construct` and `defineProperty` traps are handed arrays and descriptor
     objects of the running realm, not of the realm that made the proxy;
   - a built-in iterator's `next` checks the kind it was made as, by name, so one realm's
     `%StringIteratorPrototype%.next` steps another realm's string iterator.
5. **Another realm's intrinsic is found by ordinal.** Every realm on one engine builds the same
   constructors in the same order from the same surface set, so the ordinal of the constructor
   whose `prototype` an intrinsic is names its counterpart; the name is checked too, and an object
   that is no constructor's prototype answers itself.
6. **Rule N26** holds the model in the tree: no code of the profile stores a realm outside the
   members that are the model - the running and first realms, a function's `[[Realm]]`, a frame's
   saved realm, an awaiting built-in's own realm, an embedder view's realm and a proxy's maker - and
   none reads the engine's first realm except where no frame is running. Its witness caches a realm
   in a field and reads the first realm from a method.

**What it costs, not measured on its own.** One reference compare at each built-in call and
construction, and a save and a restore at each frame, beside the stack site the frame already takes
(JSD-0038).

## 3. `$262.createRealm` (SR-7)

- **It builds an ordinary new realm on the same engine and answers its `$262`**, as the suite's
  `INTERPRETING.md` defines the member: a global object and intrinsics of its own, built from the
  engine's surface set and no other, sharing the agent's Symbols and job queue (JSD-0030 D3). The
  new `$262` has the same members, `createRealm` among them.
- **It is charged before anything is built**: the 262,144 live bytes an instantiation reports for
  its first realm, admitted through the live-bytes ceiling, and 4,096 units of fuel. A loop that
  creates realms meets the allowance, not the process's memory (JSD-0030 D6). A created realm lives
  as long as something reaches it and dies with the instance.
- **An embedder's host surface is told of it**, once, through a view of the new realm, inside the
  step the guest's call runs in; the views of one instance share its step window. The conformance
  harness installs `evalScript`, `detachArrayBuffer` and `IsHTMLDDA` in the new `$262` this way,
  which is what the suite's cross-realm cases call. A surface that throws is the guest's throw, as a
  host body's is. A composition without a surface gets a realm whose `evalScript` and
  `detachArrayBuffer` refuse, as its first realm's do.
- **It is the profile's own member, not the harness's.** Any wide-surface guest can call it, as it
  could call the refusing one; nothing it builds is wider than the realm that asked.

## 4. What the conformance runs score (the record JSD-0018 is owed)

- **The wide dialect already selected the `cross-realm` cases** and scored them as failures of
  `createRealm`; nothing in its selection changes. In the whole run
  [JSC-264](../roadmap.corrections.md#jsc-264) records, 493 of the 534 variants of the 281 files
  that call `createRealm` pass; the 30 that fail need `SharedArrayBuffer` or `Atomics` (phase F6) or
  `Intl` (phase F7), and the 11 skipped claim a proposal or `host-gc-required`. Two of the 493 are
  `staging/sm/class/superPropProxies.js`, which also needed a repair of `super` assignment through
  a proxy that this change makes.
- **The slice dialect is unchanged.** JSD-0018 section 2 counts a case that claims a test-harness
  feature as unselectable there, because reaching `$262` needs a call that `broiler.javascript.slice`
  does not admit; its measured fact - no case it scores claims one - still holds.
- **ShadowRealm's three `cross-realm` cases stay skipped**, with the rest of the proposal's cases,
  until SR-3 admits the proposal by a record of its own.

## 5. What it amends and what it does not decide

- **JSD-0030 D1**: `$262.createRealm` no longer refuses; SR-7 was the one slice allowed to change
  that. D3 and D6 are built as written for a created realm. ShadowRealm stays absent.
- **JSD-0024**: an engine has one embedder view per realm, not one per engine, and a surface is
  told of every realm, not only the first.
- **Roadmap section 13** and its table of refusing host members.
- **Not decided:** a module map per realm - a created realm shares the engine's, so a dynamic
  `import()` from it answers the instance the engine already holds; a host constructor's fallback
  prototype, which stays its own realm's; any embedder API that creates a realm, which there is
  none of.

## 6. What would falsify this

- A function observed running in a realm other than its own, or a built-in of one realm handing a
  guest of another its intrinsics.
- A created realm that shares a global object or an intrinsic with another, is built from a
  different surface set, or is not charged.
- A well-known Symbol that differs between two realms of one engine.
