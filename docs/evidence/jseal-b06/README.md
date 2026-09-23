# BigInt through the host surface and the clone carrier: JSeal slice B06 (VM half)

Date: 2026-09-22. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Decisions
[JSD-0024](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0024-the-in-realm-host-surface.md)
(new section 19), [JSD-0032](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0032-the-internal-structured-clone-carrier.md)
(matrix amended) and [JSD-0033](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0033-the-internal-bigint-value-and-the-gated-literal.md)
(dated notes) are still proposed and unsigned. No new decision number was taken. Nothing here
advances a milestone. The JSeal half of B06 (its `VmMarshal`, `JsValue` kind and equality contract)
is not in this change.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-5 patch
(`base-vm-w6`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was a diagnostic
comparison only; the pinned specification and the pinned Test262
(`test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`) are the oracle.

## What changed

- **Host value kind** (`JsHostValue.cs`, `JsHostRealm.cs`). `JsHostValueKind.BigInt` (10, appended),
  `JsHostValue.BigInt(BigInteger)` (realm-free, like `String`), `JsHostValue.AsBigInt()` (`null`
  for any other kind), `JsHostRealm.ToBoolean` (no guest code; `0n` false, an `[[IsHTMLDDA]]`
  object false), and `JsHostRefusal.SurfaceDeclined` (6). `Equals`/`GetHashCode` compare a BigInt
  by mathematical value, never equal to a Number; object and symbol identity and the pre-existing
  **reference** comparison of Strings are unchanged and documented (JSD-0024 section 19).
- **The crossing.** `Wrap` carries a guest BigInt out as its integer; `Unwrap` admits a host
  BigInt only where the composition admitted `broiler.javascript.bigint` (else `SurfaceDeclined`),
  refuses one wider than `JsBigInt.MaximumBits` with the language's `RangeError` before allocating,
  and charges `LinearCost` per 64-bit word in both directions. The four crossings that resolved a
  value before their own `try` now translate a refusal or an abort (`UnwrapAtCrossing`). A thrown
  BigInt reaches the host as a `JsHostThrowException` carrying it, replacing the B01 `TypeError`
  substitution; an abort raised while carrying one answers the latched termination.
- **Clone** (`JsClone.cs`, `JsRealm.Clone.cs`). Slot kind `BigInt` (6) and record kind `BigInt`
  (13) - a BigInt object is rebuilt on the destination `BigInt.prototype`, shared objects stay
  shared. Eight bytes per word count against `MaxBytes`; both ends charge per word. The carrier
  records that it holds a BigInt, and a declining destination refuses it before claiming moved
  bytes. Carrier layout `FormatVersion` 1 to 2.
- **The Test262 defect the wave-5 whole run exposed**, and the next step of the same test.
  `Array.prototype.toString` falls back to the intrinsic `%Object.prototype.toString%` (captured at
  setup) rather than reading `Object.prototype.toString`; `%TypedArray%.prototype.toString` is now
  the same function object as `Array.prototype.toString`. With that fixed,
  `non-callable-join-string-tag.js` failed next on `Object.prototype.toString`, which answered the
  internal class name as its builtin tag and read `@@toStringTag` before deriving it: it now derives
  the builtin tag first and only for the specification's ten kinds (`BuiltinTag`). That exposed six
  brands whose `Symbol.toStringTag` the realm had never installed and had left to the class name:
  `WeakMap`, `WeakSet`, `WeakRef`, `FinalizationRegistry`, `ArrayBuffer`, `DataView` (data
  properties) and the `%TypedArray%.prototype` getter; all are installed now.
- **Audit** of built-ins reading a mutable property where the specification names an intrinsic:
  every `GetProperty(JsValue.Object(<realm intrinsic>), ...)` and every read of a global by name in
  the profile was listed. The two found were the two `toString` fallbacks above; the
  `%GeneratorFunction%` constructor lookup reads the global object only at realm setup, before any
  guest code, and was left.
- **Inventories.** Public API baseline: five members added (`docs/api/public-api.txt`). No product
  file added (count stays 193). No opcode, no diagnostic code, no global added or removed (rule N17
  and `docs/realm/globals.txt` unchanged). Stale comments in `JavaScriptProfile.cs` and `JsValue.cs`
  corrected; JSD-0033's table rows and the decision index carry dated notes.

## Executed

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_API_WRITE=1` and `BROILER_ASSURANCE_WRITE=1`
  `dotnet test Broiler.VM.slnx -c Release --no-build`; rebuild;
  `dotnet test Broiler.VM.slnx -c Release --no-build`. Base: Contract 267/267, Architecture
  256/256. After: Contract 267/267, Architecture 256/256.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks`: the pass count moves from 363 to
  368, the two `not-run` x86-64-sysv rows unchanged, no row failing. New rows:
  `bigint/b06/a-declining-realm-refuses-a-host-bigint`, `bigint/b06/a-host-crossing-is-charged-per-word`
  (fuel bisection: twenty round trips of a value 1,024 words wider cost exactly 40,960 more units),
  and three `clone/b06/...` rows; `bigint/b01/a-thrown-bigint-reaches-the-host-as-a-host-throw` now
  expects `host-throw:1` where it expected `host-throw:TypeError`
  ([host-surface-and-checks.txt](host-surface-and-checks.txt)).
- `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passed; rows 65 before,
  73 after (eight BigInt rows: the crossing and its exactness at the 2^20-bit ceiling, host-built
  values and the `RangeError` one bit past it, `ToBoolean`/`ToJsString`/`ToNumber`, equality,
  properties/`Invoke`/`Construct`, a thrown BigInt and its re-raise, the clone, and a declining
  realm refusing both a host BigInt and a carrier holding one without claiming its moved bytes).
- `python eng/run-cli-acceptance.py`: 225/225 before and after.
  `Broiler.VM.Composition.JavaScript.ExecutionOnly.exe --corpus src/tests/corpus/js-1`: 22 checks
  before and after. The lowering did not change; the corpus was not regenerated.
- **Differential probe** `src/tests/differential/the-intrinsic-to-string-fallbacks.js` (23 cases,
  no declared divergence): `python eng/run-differential.py --only the-intrinsic-to-string-fallbacks
  --against node --timeout 20` exits 0. **Failing first**: on the base build 13 of the 23 cases
  differ ([probe-before.txt](probe-before.txt)). The whole differential run against Node shows no
  retained answer changed except the pre-existing cube-root case 36 of
  `the-later-library-methods.js`; its other reports (the Node locale in
  `the-json-date-and-regexp-surface.js` 30 and 34, Node's own exit in
  `the-seam-between-generators-and-the-rest.js`, four stale declarations in
  `the-settling-of-promises.js`) are unrelated to this change and match the retained answers.
- **Mutations** ([mutations.txt](mutations.txt)): removing the per-word charge, comparing a BigInt
  by reference, and moving the ceiling by one bit each fail a named row; each was reverted.
- The host-surface and slice-compiler BigInt rows could not be run against the base build: they
  call members that do not exist there (the rows fail to compile), which is the failing-first state
  for an API addition. On the base build the b01 row answered `host-throw:TypeError`.

## Test262 (pinned, bytecode form, `--shards 1 --jobs 1`)

Before is the base build in a second detached worktree; after is this change. See
[test262-comparison.txt](test262-comparison.txt).

- `built-ins/Object/prototype/toString`, `built-ins/Array/prototype`, `built-ins/TypedArray`,
  `built-ins/TypedArrayConstructors`, `built-ins/WeakMap`, `built-ins/WeakSet`, `built-ins/WeakRef`,
  `built-ins/FinalizationRegistry`, `built-ins/ArrayBuffer`, `built-ins/DataView`,
  `built-ins/Symbol/toStringTag`, `built-ins/BigInt` (6,181 files, 12,243 variants): passed moves
  from 9,848 to 9,912; **64 variants newly pass, none newly fails**. They include
  `Array/prototype/toString/non-callable-join-string-tag.js`, `TypedArray/prototype/toString.js`,
  eleven `Object/prototype/toString` files and every `Symbol.toStringTag` file of the seven brands.
- The broadest neighbourhood of `Object.prototype.toString`: `built-ins/Object`, `Function`,
  `Error`, `NativeErrors`, `Symbol`, `Proxy`, `Map`, `Set`, `Promise`, `Iterator`, `JSON`, `Math`,
  `Reflect`, `RegExp/prototype`, `String/prototype`, `language/arguments-object` and
  `annexB/built-ins` (8,964 files, 17,537 variants): passed moves from 16,575 to 16,595; **20
  variants newly pass (the eleven `Object/prototype/toString` files, also in the first selection),
  none newly fails**; the 4 exhausted and 146 skipped variants are the same before and after.

## Not exercised

- **The host crossing has no Test262 coverage**: the suite does not reach an embedder's surface.
  It is exercised only by the host-surface lane and the slice-compiler rows above.
- The **native execution form** was not run (it cannot instantiate on this machine,
  `ProfileFault/UnsatisfiedHostAssumption`, pre-existing).
- The **whole suite** was not re-run; the two selections above were chosen for the changed
  built-ins (`Object.prototype.toString` is reached from everywhere, so the second selection is
  the broadest neighbourhood of it).
- `BigInt64Array`/`BigUint64Array` (B07) and the DataView accessors (B08) are other agents'
  cards; the typed-array `@@toStringTag` getter reads the element-kind table, so it names a kind B07
  adds without a change here. The JSeal half of B06 is not made here.
- The exotic-object hooks (`IJsHostExotic`) still call `Unwrap` untranslated, as before this change
  for a foreign reference; a hook answering an oversized BigInt raises the guest `RangeError`, and one
  answering a BigInt to a declining realm raises the surface refusal untranslated. Recorded in
  JSD-0024 section 19; not widened in kind, not fixed.
