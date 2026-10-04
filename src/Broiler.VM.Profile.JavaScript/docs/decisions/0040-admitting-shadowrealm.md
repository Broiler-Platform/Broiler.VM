<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0040 - Admitting ShadowRealm at proposal revision `9ff2a01f`, behind `broiler.javascript.shadowrealm`

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. Taking the record admits a stage 2.7 proposal ahead of the pinned edition, as
[JSD-0034](0034-admitting-explicit-resource-management-ahead-of-the-edition.md) admitted explicit
resource management, inside the boundary [JSD-0030](0030-shadowrealm-support-boundary.md) fixed; not
taking it means removing a global the edition does not define and the surface identity minted for it.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the second record of phase F5 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile). It builds JSD-0030's
slices SR-3 (the constructor, `evaluate` and wrapped functions) and SR-5 (`importValue`) on the realm
model of [JSD-0039](0039-a-second-realm-on-one-engine.md), and mints the identity
[JSD-0002](0002-feature-manifest-allocation.md)'s reopened section left to "the record admitting
ShadowRealm".

---

## 1. The pin and the identity

- **The proposal is pinned where JSD-0030 D2 pinned it**: `tc39/proposal-shadowrealm` at
  `9ff2a01f1ec68a427f6f37ce710aa7cba8150678`, stage 2.7. Its text is not archived here, as JSD-0034's
  proposal text is not. The suite's cases under `test/built-ins/ShadowRealm` at `ccaac100` measure it.
- **The identity is `broiler.javascript.shadowrealm`**, an optional surface owning the global
  `ShadowRealm`: a program that names the global declares the surface, and a composition that
  declines it refuses that program at verification, as for every surface. **It is admitted only
  together with `broiler.javascript.dynamic`** (JSD-0030 section 8): a descriptor naming it without
  the dynamic surface is refused when it is built, and the conformance runner's `--decline` of the
  dynamic surface declines this one with it. Every door that admits every surface admits it.
- **The conformance command scores the `ShadowRealm` flag**, beside `explicit-resource-management`;
  every other proposal stays skipped, and JSD-0018 is otherwise unchanged.

## 2. What is built

Each item is JSD-0030 section 4's, built as written unless it says otherwise.

1. **The constructor** (D3, D6). `new ShadowRealm()` reads its prototype from `new.target` first,
   then builds a realm on the same engine from the same surface set, charged as `$262.createRealm`
   charges one, and with **nothing a host adds**: no `print`, `console`, `read` or `$262` on its
   global, and no host surface told of it (D7). Calling it without `new` is a `TypeError`. A shadow
   realm admits what its engine admits, `ShadowRealm` included, so realms nest.
2. **`evaluate(sourceText)`** (D4, D5). A receiver that is not a ShadowRealm, and a source that is not
   a String, are `TypeError`s. The source is compiled **in the caller's realm, through the engine's
   one mediator, as a guest eval request** - so a refusal, a parse failure and an early error are the
   caller's own errors - and run as global eval code of the shadow realm: sloppy, its global as
   `this`, its lexical declarations its own and its `var`s and functions properties of that global.
   Anything the evaluation throws becomes a fresh `TypeError` of the caller's realm whose message
   copies nothing the guest wrote; the completion crosses through `GetWrappedValue`.
3. **`GetWrappedValue`** (D4). A primitive crosses as itself; a callable - a function, a bound
   function, a callable proxy, a wrapper - crosses as a **new** wrapped function every time; any other
   object is a `TypeError` of the realm running.
4. **Wrapped functions.** A built-in of the realm it was wrapped for, with `Function.prototype` of that
   realm, a call and no construction. `length` and `name` are copied as `CopyNameAndLength` copies
   them - an own `length` that is a Number, infinities kept, and a `name` that is a String - and a
   failure while copying is a `TypeError`. A call wraps the receiver and each argument into the
   target's realm, calls, and wraps the result back; an exception from the target becomes a fresh
   `TypeError` of the wrapper's realm, and a revoked proxy's missing realm is one too.
5. **`importValue(specifier, exportName)`** (D8, SR-5). The specifier through `ToString` and an
   `exportName` that must be a String, checked synchronously; then the engine's dynamic import, asked
   from the shadow realm against the running script, through the same mediator as every other load.
   A fulfilment reads the export and wraps it into the caller's realm, and a missing export is a
   `TypeError`; a rejection becomes a `TypeError` of the caller's realm through its
   `%ThrowTypeError%`.

**What is not the proposal's, on purpose.** **A shadow realm shares the engine's module map**
(JSD-0039 section 5), where D8 says each child realm has its own: a module imported for a shadow
realm and imported again by its parent is one instance, and its functions belong to the realm that
was running when it was instantiated. The suite's cases do not observe it.

## 3. JSD-0030 section 6, as checks

The CLI's host-surface lane holds six of the seven policy-isolation cases with a provider that
counts what it is asked: a restricted parent constructs and its `evaluate` is refused after one guest
request (case 1); a host script's permit is not borrowed by the `evaluate` inside it (case 2); a
provider admitting only `(s) => eval(s)` answers that and refuses the inner `eval` (case 3), and the
same for a nested `ShadowRealm` (case 4); no host object is visible inside and one handed over is
refused (case 6); a host method crosses only as a wrapper whose keys are `length` and `name` and whose
call answers the method's primitive (case 7). The slice compiler's checks hold case 5: a composition
that declined the dynamic surface has no `ShadowRealm`, and a descriptor naming the ShadowRealm
surface alone is refused.

**Cases 3 and 4 answer a `TypeError` where JSD-0030 section 6 wrote `SyntaxError`.** The refusal
inside the shadow realm is a `SyntaxError` there, and it reaches the caller through a wrapped
function, which D4 - the same record - says turns every exception into a fresh `TypeError` of the
caller's realm. The provider's counts show the request was made and refused, which is what the cases
are about.

## 4. What the suite scores

All 124 scored variants of the 64 cases under `test/built-ins/ShadowRealm` pass: 49 of `evaluate`,
the constructor and wrapped functions, the 3 that also claim `cross-realm`, and the 12 of
`importValue`, in both modes where a case has two. The three `_FIXTURE` modules are skipped as
fixtures. That is JSD-0030 SR-3's acceptance (49 cases, the 3 `cross-realm` ones then unselectable)
and SR-5's (12 cases), with SR-7's 52 reached by JSD-0039.

## 5. What a realm costs, measured

On 2026-10-04, averaged over a hundred realms held at once in one instance of the Release build, a
realm built from every surface held **504,818 bytes** of managed heap when `$262.createRealm` made it
and **489,699** as a ShadowRealm's, and each took about 2 ms to build, which is what the interpreter
spends on about 37,000 units of fuel on the same machine. A realm is therefore charged **524,288 live
bytes and 32,768 fuel** before it is built. JSD-0039 section 3's first charges, 262,144 bytes and
4,096 fuel, were half and a ninth of that (*JSC-265*).

## 6. What would falsify this

- Source evaluated in a shadow realm, or a module imported for one, compiled or loaded anywhere but
  through the engine's one mediator, or as anything but a guest request.
- A non-callable object, or an exception object, observed on the other side of the boundary; a
  callable crossing as itself.
- A host member on a shadow realm's global, or a host surface told of one.
- A descriptor admitting `broiler.javascript.shadowrealm` without `broiler.javascript.dynamic`.
