# BigInt typed arrays and DataView BigInt accessors: JSeal slices B07-B08

Date: 2026-09-22. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Decision
[JSD-0033](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0033-the-internal-bigint-value-and-the-gated-literal.md)
is still proposed and unsigned; this slice amends it as section 8 (no new decision number). Nothing
here advances a milestone.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-5 patch
(`base-vm-w6`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was a diagnostic
comparison only; the pinned ES2026 specification and the pinned Test262
(`test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`) are the oracle.

## What changed

- **Two element kinds** (`JsBinary.cs`): `JsElementKind.BigInt64` (10) and `BigUint64` (11), eight
  bytes, `JsElements.HoldsBigInts` true for both. Elements now move as a `JsValue` (a Number, or a
  BigInt for these two) rather than a `double`: `JsElements.ReadValue`/`WriteValue`,
  `JsTypedArray.TryReadAt`/`TryWriteAt`, `JsDataView.TryRead`/`TryWrite`. A write stores the value
  modulo 2**64 (`ToBigInt64`/`ToBigUint64`: one write, signedness is the read's). The Number-only
  `Read`/`Write` refuse a BigInt kind by name (`InternalDefect`) instead of rounding.
- **One conversion** (`JsEngine.ToElementValue`): `ToBigInt` for a BigInt kind (a Number is a
  `TypeError`), `ToNumber` otherwise; a value wider than 64 bits is narrowed once and charged for
  its words. Every element write uses it: `[[Set]]` (three engine paths), `[[DefineOwnProperty]]`,
  a class field defined on a typed array (it used to bypass the engine), `set`, `fill`, `with`,
  `map`, `filter`, `of`, `from`, the constructors, and `DataView` setters.
- **Content types do not mix**: `set` from a typed array of the other content type (after the
  RangeError, as the specification orders it), construction from one, and a species result are
  `TypeError`s. `sort` compares BigInts exactly; `indexOf`/`lastIndexOf`/`includes` compare values.
- **Three deliberate behaviour changes to the existing Number kinds** (card B07 asks that Number
  arrays stay behaviourally unchanged; these are spec corrections the BigInt tests needed, and no
  Test262 variant regressed): `%TypedArray%.of` constructs through its receiver (a subclass such as
  `class S extends Float64Array {}` used to be refused by name; `S.of(1, 2) instanceof S` is now
  `true`); `%TypedArray%.prototype[Symbol.toStringTag]` exists (it was missing for every kind); and
  a class field defined on any typed-array subclass instance goes through the typed array's
  `[[DefineOwnProperty]]`, so a field value's `valueOf` runs and an out-of-range numeric field key is
  a `TypeError` (it used to be stored without the engine, an object becoming NaN).
- **DataView (B08)**: `getBigInt64`, `getBigUint64`, `setBigInt64`, `setBigUint64` through the
  shared accessor path (ToIndex, then ToBigInt for a setter, then littleEndian, then the
  detached/out-of-bounds TypeError, then the RangeError).
- **Two surfaces**: the constructors and accessors are built only when a composition admits both
  `broiler.javascript.binary` and `broiler.javascript.bigint`. The two names are on
  `JsSurfaces.BinaryGlobals` and `JsSurfaces.BigIntGlobals`; the lowering declares both surfaces for
  them (`JsCompiler.DeclareSurfaceOf`). No public member added (API baseline unchanged).
- **Clone** (`JsRealm.Clone.cs`): unchanged carrier (JSD-0032 names views by constructor); a
  destination realm without the kind now refuses the name rather than faulting. JSD-0032's
  typed-array row carries a dated amendment.
- **Inventories**: `docs/realm/globals.txt` regenerated (`BigInt64Array`, `BigUint64Array` added);
  **the status ledger's machine-readable `absent-globals` block lost those two lines** - the only
  edit to a ledger, which rule N17 requires; its surrounding prose is not edited. The CLI fixture
  `runs/a-typed-array-over-a-buffer.js` now pins `undefined:undefined:function` (JSD-0028 amended
  with a dated note). The CLI and polyglot help paragraphs, `docs/compositions.md`,
  `JavaScriptProfile.BigIntManifest`'s remarks and JSD-0033 section 7 item 2 carry dated corrections.
  No opcode, no diagnostic code, no product file added (count stays 193).

## Executed

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build`;
  rebuild; `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture
  256/256 before and after.
- `SliceCompiler.exe --checks --verbose`: the pass count moves from 363 to 365 (366 after the
  review fixes, below), the two `not-run`
  x86-64-sysv rows unchanged. New rows: `bigint/b07/the-bigint-typed-arrays-need-both-surfaces`
  (a composition declining BigInt, or declining binary, refuses `BigInt64Array`/`BigUint64Array`
  with 1608 at verification and builds neither constructor nor the four accessors) and
  `clone/b07/bigint-views-clone-by-kind-and-share-one-buffer`. These rows were written after the
  implementation; they were not run against the base.
- **Failing first** ([probe-before.txt](probe-before.txt)): the new probe
  `src/tests/differential/the-bigint-typed-arrays.js` (60 cases) on the base build answers
  differently from Node in all 60 (a `ReferenceError` or a missing accessor).
- Differential: `python eng/run-differential.py --only the-bigint-typed-arrays --against node
  --timeout 20` agrees with Node except the declared case 27: `subarray` of a `BigInt64Array` whose
  species is `Float64Array` is a `TypeError` by ES2026 `TypedArraySpeciesCreate` step 4, where V8
  answers the `Float64Array`. `the-bigint-public-surface` case 60 now answers
  `function,function,function` as Node does; its retained answer was rewritten and its stale
  declaration removed. The whole lane (`--timeout 60`, retained answers) differs only in the known
  code-page case of `the-later-library-methods`, as on the base.
- CLI acceptance: 225 of 225 command lines answered as declared, before and after (one fixture's
  pinned line changed, above). Host-surface lane: every check passed, before and after. Corpus
  replay (`ExecutionOnly --corpus src/tests/corpus/js-1`): 22 checks passed before and after; the
  lowering of BigInt typed arrays uses no new instruction, so the corpus was not regenerated.

## Review fixes (2026-09-22)

Details and the retained output: [review-fixes.txt](review-fixes.txt).

- **`JSON.parse`'s reviver wrote through the engine-less element path.** A reviver that makes a
  `BigInt64Array` the holder of the next level and returns a Number, a String or an object for an
  element ended the whole invocation with `ProfileFault/ProfileContractViolation` (JsAbort
  `InternalDefect` in `JsElements.ElementOf`) instead of a catchable `TypeError` or a stored value;
  a `Float64Array` holder given a BigInt did the same (that half predates this slice, B05). The
  reviver's store (`JsRealm.JsonReviveInto`) now is `CreateDataProperty`, the target's own
  `[[DefineOwnProperty]]` through `ObjectDefineOwn`, with a refusal ignored and a conversion error
  propagated, as ES2026 `InternalizeJSONProperty` says. That also stops a reviver overwriting a
  property of a frozen holder, which is why two more `JSON/parse/reviver-*-non-configurable-prop-create.js`
  files pass. The other `SetOwnProperty` call sites whose target a guest can choose were audited
  with a 39-route script against Node (species results, `Array.prototype` methods called on a
  typed array, `Object.assign`, `Reflect.set` with a receiver, destructuring and `for-of`
  targets, the Iterator.prototype setters, a Proxy over a typed array, class fields); none other
  ends the invocation. The `ElementOf` remark and JSD-0033 section 8 item 2 and its falsified-if
  clause now say so rather than claiming the property unconditionally.
- **Probe cases 61-63** cover the reviver route (BigInt and Number holders, an out-of-range index, an
  `undefined` return, a frozen holder). Before the fix the probe run ended with exit 7; after it,
  the probe agrees with Node except the declared case 27.
- **A metering row** `bigint/b07/a-wide-element-write-is-charged-for-its-narrowing`: under one fuel
  ceiling two hundred stores of a 64-bit BigInt complete and two hundred stores of a 2**20-bit one
  exhaust the allowance. The slice-compiler pass count moves from 365 to 366.
- **Mutation runs**: removing the narrowing's charge in `ToElementValue` fails the metering row
  alone; removing the BigInt-surface declaration in `DeclareSurfaceOf` fails
  `the-bigint-typed-arrays-need-both-surfaces` alone. The clone row was not mutated.
- After the fixes: Contract 267/267, Architecture 256/256 (after assurance regeneration); CLI
  acceptance 225 of 225; corpus replay 22 checks; host-surface lane all checks; the whole
  differential lane differs only in the known code-page case. Test262 over
  `built-ins/{JSON,TypedArrayConstructors,TypedArray,DataView}` re-run: no variant that passed in
  the earlier after-runs fails, and 4 JSON variants move to passing.

## Test262

[test262-comparison.txt](test262-comparison.txt). Two selections, base and after, no variant that
passed on the base fails after. Run 1 (typed arrays, DataView, Map, Set, `Array/prototype/map`,
ArrayBuffer, Atomics): 1803 variants move to passing. `TypedArray/**/BigInt/` moves from 2 of 958
to 952 of 958 passing and the BigInt `TypedArrayConstructors` subtrees from 2 of 573 to 497 of 573;
every remaining failure there needs `$262.createRealm` or a `SharedArrayBuffer`. The DataView BigInt
accessors pass 134 of 136 (two skipped as `immutable-arraybuffer`, a proposed feature). `Map/valid-keys.js`,
`Set/valid-values.js` and the `{Array,TypedArray}/prototype/map/resizable-buffer*.js` files pass.
Run 2 (Array, Object, BigInt, Reflect, JSON and the language subtrees the F04-F06 list touches): 162
variants move to passing. The 149 files the F04-F06 evidence ran through a shimmed harness now pass
unshimmed, 298 of 298 variants (none passed unshimmed on the base).

## Not exercised

- The native execution form (pre-existing `ProfileFault/UnsatisfiedHostAssumption` on this machine).
- The whole Test262 suite (only the two selections above), and the floor file was not re-based.
- Mutation runs against the clone row `clone/b07/bigint-views-clone-by-kind-and-share-one-buffer`;
  none of the three new rows was run against the base build (they were written after the
  implementation). The reviver route through a Proxy's `deleteProperty` trap was not examined
  (the `undefined` branch still calls `DeleteOwnProperty`, unchanged).
- Atomics over BigInt arrays and `SharedArrayBuffer` (absent, JSD-0028); cross-realm variants.
