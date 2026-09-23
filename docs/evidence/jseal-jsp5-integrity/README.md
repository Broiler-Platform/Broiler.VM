# JSP-5/JSP-4 object-integrity follow-up fixes: JSeal slice VM-FIX-F

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It advances the JSP-4 and JSP-5 exit gates in `roadmap.parity.md` (and the typed-array half of
JSW-2 that JSP-5 restates) and does not mark any stage complete.

Base: HEAD `484f389` plus the merged waves 1-2 patch (`base-vm-w3`). Windows 11, .NET SDK 10.0.401,
`Release` configuration, bytecode form. Comparison engine: Node v24.17 (diagnostic, not the oracle).

## What changed

1. **SetIntegrityLevel through [[DefineOwnProperty]].** `Object.freeze`/`Object.seal` define each
   own key with the partial descriptor (`DefinePropertyOrThrow`) instead of patching stored
   attributes (`ObjectSetIntegrity`, `JsRealm.Object.cs`). Freezing or sealing a non-empty typed
   array is now a `TypeError` (after `[[PreventExtensions]]`); an empty one still freezes.
2. **Array mutators throw on refusal.** `ArrayDeleteAt` is `DeletePropertyOrThrow`, so `pop`,
   `shift`, `splice`, `copyWithin`, `reverse`, `unshift` and `sort` over a sealed/frozen receiver or
   a non-configurable element throw. The Array append fast path in `JsEngine.SetIndexed` is taken
   only when no prototype holds the index (`ChainMayAnswerIndex`; a Proxy or typed array on the
   chain always takes the full `OrdinarySet`), so an inherited setter or read-only element answers
   an append (`push/unshift set-length-array-is-frozen`, `...-length-is-non-writable`,
   `for-in/head-lhs-let`). `copyWithin` returns `ToObject(this)`.
3. **Typed-array canonical numeric keys and the [[Set]] receiver rule** (guidance items 3-4) were
   verified already correct after VM-FIX-A on every path probed (own and inherited get/has/set,
   `Reflect.set` with a foreign receiver, `defineProperty`, `delete`); no code change.
4. **ArraySetLength.** `ArrayLengthOrRefuse` converts with `ToUint32` and again with `ToNumber`
   (`valueOf` runs twice; differing answers are a `RangeError`). An assignment to an already
   non-writable `length` is refused before any conversion. Redefining `length` refuses an accessor
   and `writable: true` once closed (`define-own-prop-length-coercion-order.js`).
5. **Module namespace [[DefineOwnProperty]].** String keys go to `ObjectDefineExport`: unexported
   is `false`, configurable/non-enumerable/non-writable/accessor is `false`, a value answers
   `SameValue` with the binding, and nothing is written; a TDZ export is a `ReferenceError`.
   `Reflect.set` with a namespace receiver answers through the same rule; `Object.freeze(ns)` with
   exports throws, `Object.seal(ns)` succeeds.
6. **Array.from** constructs through `this` when it is a constructor (`Construct(C)` /
   `Construct(C, [len])`, else `ArrayCreate`), fills with `CreateDataPropertyOrThrow`, sets
   `length` with a strict `Set`, reads `Symbol.iterator` once and closes the iterator when the
   mapper or the definition throws.
7. **%TypedArray%.from** accepts any constructor (subclasses included) through
   `TypedArrayCreateFromConstructor`, reads `Symbol.iterator` once, drains an iterable before
   mapping, reads an array-like with `ToLength` element by element, writes with a strict `Set`.
   **%TypedArray%.prototype.set** reads an array-like `length` with `ToLength` and checks the
   receiver's buffer after converting the offset.
8. **JSON.** `JSON[Symbol.toStringTag]` is `"JSON"` (configurable only). An array replacer is
   recognised with `IsArray` (Proxy over an Array included) and read with `LengthOfArrayLike`.
9. **Iterator steps.** `JsEngine.TryIterateNext` marks the record done when the `done` or `value`
   getter throws, so `for-of`, destructuring and `Array.from` do not call `return` afterwards.

No opcode, diagnostic code, decision record, public API, product file or global inventory entry was
added; N17 and the absent-feature records are unaffected (`JSON` gained a Symbol-keyed property, not
a global).

## Regression probes

- `src/tests/differential/the-object-integrity-paths.js` (83 cases), written before the fixes. The
  base build (`probe-before.txt`, built from the base tree) differed from Node in 26 of 83 answers;
  after the change all 83 agree with Node and the specification.
- `src/tests/differential/the-module-namespace-integrity.mjs` (21 cases, self-importing module).
  The base build (`namespace-probe-before.txt`) disagreed with the specification in 5 cases (2, 7,
  13, 15, 20). After the change all 21 follow the specification; case 14 is declared
  (`#diverges node 14`): V8 refuses `Reflect.set({}, "a", 1, ns)`, while `OrdinarySet` defines
  `{ [[Value]]: 1 }` on the namespace and its `[[DefineOwnProperty]]` answers `SameValue` = true.

`python eng/run-differential.py --timeout 30` (retained answers, all probes): only the long-standing
case 36 of `the-later-library-methods.js` (cube-root spelling) differs, as on the base. No existing
retained answer changed.

## Unit suites

`dotnet build Broiler.VM.slnx -c Release`, then `dotnet test Broiler.VM.slnx -c Release --no-build`:
Contract 267/267, Architecture 256/256 before and after (after the assurance write pass and rebuild).

## Test262 (pinned suite ccaac100, bytecode form, `--shards 1 --jobs 1`)

Main selection (`test262-comparison.txt` lists it): Object freeze/seal/isFrozen/isSealed, Array,
TypedArray, TypedArrayConstructors, JSON, Reflect, Proxy, module-code, for-of, for-in, assignment,
array expressions, destructuring; 15511 variants. The pass count moves from 12739 to 12811 (72
variants fixed, none newly failing; unsupported 821 and skipped 165 unchanged). Regression watch
(Object, arguments-object, object expressions, Iterator, for statements; 11208 variants): identical
before and after, no variant changed.

Remaining failures in the touched areas have other causes: BigInt, resizable and shared buffers,
nested realms, generator/async function constructors not admitted by the manifest, `for-in-order`
in `JSON/stringify/property-order.js`, and `Object/seal/seal-symbol.js` (the host fails to render a
Symbol script completion value; `Object.seal(Symbol())` itself returns the Symbol).

## Not exercised

- Native execution form (cannot instantiate on this machine, pre-existing).
- `Array.of` and `%TypedArray%.of` still ignore a non-intrinsic `this` constructor; the async and
  `yield*` iterator steps were not changed.
- The whole-suite Test262 run and Linux were not run.
