# JSP-4/JSP-5 follow-up correctness fixes: JSeal slice VM-FIX-C

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It advances the JSP-4 and JSP-5 exit gates in `roadmap.parity.md` and does not mark either stage
complete.

Base: HEAD `484f389` plus the merged wave-1 patch (`base-vm-w2b`). Windows 11, .NET SDK 10.0.401,
`Release` configuration, bytecode form.

## What changed (four separately described fixes)

1. **LengthOfArrayLike.** `ArrayLengthOf` (`JsRealm.Array.cs`, also used by `JSON`) reads `length`
   with `ToLength` instead of `ToUint32`, so `-1` is 0 and 2^32 stays 2^32. `push`, `unshift`,
   `splice` and `toSpliced` throw the specification's `TypeError` when the new length would pass
   2^53-1, before writing anything; the new length is computed removal-first so no intermediate
   double sum rounds back under the ceiling. `toReversed`, `toSorted`, `with`, `toSpliced` and the
   array-like path of `Array.from` throw `ArrayCreate`'s `RangeError` for a length past 2^32-1
   before reading elements. Every index loop still charges fuel per visited index; no loop was
   added. `reverse` now leaves two holes alone instead of deleting both (a Proxy's
   `deleteProperty` trap sees the difference; `reverse/length-exceeding-integer-limit-with-proxy.js`).
2. **CreateDataPropertyOrThrow through the result's own [[DefineOwnProperty]].** `ArrayCreateDataAt`
   routes every non-Array result through the realm's define path (the one `Object.defineProperty`
   uses); a plain Array keeps a direct path that is observably identical. That path
   (`JsRealm.Object.cs`) is now one boolean `[[DefineOwnProperty]]` (`ObjectDefineOwn`) whose
   refusals are `false` and whose guest exceptions propagate, with the exotic rules it lacked:
   an Array refuses an ARRAY INDEX at or past a closed length (keys >= 2^32-1 are not refused), and
   a typed array answers every canonical numeric key itself (invalid index, accessor, or a
   `false` configurable/enumerable/writable field is refused; the value is converted with
   `ToNumber`). `Reflect.defineProperty` and a Proxy's missing-trap forwarding use it instead of
   catching every exception, so an Array `length` `RangeError` now propagates from both.
3. **%ThrowTypeError%.** The realm builds one `ThrowTypeErrorFunction` (length 0 and name "" both
   non-writable/non-configurable, not extensible). Every unmapped `arguments` object - strict, or
   sloppy with a non-simple parameter list - gets `callee` as a non-enumerable, non-configurable
   accessor whose get and set are that function; `Function.prototype.caller`/`arguments` share it.
4. **OrdinarySet receiver half.** `LandOnReceiver` (both String and Symbol keys;
   `SetSymbolWithReceiver` now uses it) reads the receiver with `[[GetOwnProperty]]`, updates an
   existing writable data property with `[[DefineOwnProperty]](P, { value })` only, and creates an
   absent one with `CreateDataProperty`, both through `ObjectDefineOwn`; a Proxy receiver's `false`
   is returned as `false` instead of thrown.

No public API, global inventory, artifact format or native-backend code changed.

## Regression probe

`src/tests/differential/the-array-length-and-receiver-writes.js` (81 cases), written before the
fix. Run case by case against the base build (the whole file stops at case 14 on the base build:
`unshift` over a 2^53-1 length exhausts the fuel allowance), 58 of 81 answers differed from
Node 24.17. After the fix 80 of 81 agree; case 17 is declared
(`#diverges node 17`): the specification compares the mathematical `len + insertCount -
actualDeleteCount` and throws, while Node sums in doubles and splices. The retained answers were
written with `--write` only after that check.

`python eng/run-differential.py --against node --timeout 20` over all probes shows the same
failures with the base build (`D:/Broiler.VM` main-tree binary) and after the change, in probes
this change does not touch: `the-json-date-and-regexp-surface.js` (locale time-zone name, cases
158-159 console encoding), `the-later-library-methods.js` (invalid UTF-8, case 147),
`the-seam-between-generators-and-the-rest.js` (Node itself exits 1) and
`the-settling-of-promises.js` (four stale Node declarations). No retained answer changed.

## Unit suites

`dotnet build Broiler.VM.slnx -c Release`, then `dotnet test Broiler.VM.slnx -c Release --no-build`:
Contract 267/267, Architecture 256/256 before and after (after the assurance write pass and
rebuild).

## Test262 (pinned suite ccaac100, bytecode form, `--shards 1 --jobs 1`)

Main selection: `test/built-ins/Array` (full), `test/language/arguments-object`,
`test/built-ins/Reflect/set`, `test/built-ins/Proxy/set`, `test/built-ins/Proxy/defineProperty`,
6713 variants:

| | pass | fail | unsupported | exhausted |
|---|---|---|---|---|
| before | 6032 | 625 | 40 | 16 |
| after | 6233 | 440 | 40 | 0 |

201 variants moved to pass (200 under `Array/prototype`, among them every former
length-near/exceeding-integer-limit, `clamps-to-integer-limit` and `create-*-invalid-len`
variant and the 16 wall-clock exhaustions; plus `arguments-object/10.6-13-c-3-s.js`) and none
moved from pass. By method: splice 20, map 18, indexOf 16, push 14, slice 14, some 14, every 12,
pop 12, lastIndexOf 10, reduceRight 8, unshift 8, filter 6, forEach 6, reduce 6, reverse 6,
toSpliced 6, toReversed 4, toSorted 4, and 2 each for copyWithin, fill, findLast, findLastIndex,
includes, join, shift, sort.

Wider selection, for the define-path change: `Object/defineProperty`, `Object/defineProperties`,
`Reflect`, `Proxy`, `ThrowTypeError`, `TypedArrayConstructors/internals`, `JSON`,
`Function/prototype`, `Object/freeze`, `Object/seal`, 6131 variants. The "before" run used the
main-tree build of the same base (sources of every file this change touches are identical):

| | pass | fail | unsupported | exhausted |
|---|---|---|---|---|
| before | 5508 | 464 | 159 | 0 |
| after | 5558 | 414 | 159 | 0 |

52 variants moved to pass (ThrowTypeError 14, TypedArray `DefineOwnProperty` 26, Object
defineProperty/defineProperties 8, `Function.prototype` caller/arguments `prop-desc` 4). Two
variants moved from pass to fail: `TypedArrayConstructors/internals/DefineOwnProperty/conversion-operation-consistent-nan.js`
(sloppy and strict). It passed vacuously: `new Float64Array(iterable)` builds a length-0 array in
this profile (a pre-existing typed-array constructor defect, outside this slice), and the old
define silently discarded `Object.defineProperty(samples, "0", ...)`; the correct typed-array
`[[DefineOwnProperty]]` now refuses index 0 of an empty view. The list of every moved variant is
in `test262-comparison.txt`.

## Still failing in these subsets, and why

- `arguments-object`: 10.5-1-s and 10.5-7-b-1-s need direct eval (V14); 10.6-13-a-2/-a-3 need
  `Function.caller` (feature `caller`).
- Cross-realm variants (`Proxy/set`, `Proxy/defineProperty`, `ThrowTypeError/distinct-cross-realm`,
  Array species): `$262.createRealm` refuses; counted as failures, not coverage.
- `push`/`unshift` `set-length-array-is-frozen` / `-length-is-non-writable` (8 variants),
  `copyWithin/return-abrupt-from-delete-target`: the Array mutators do not yet use
  `DeletePropertyOrThrow` and assign past a closed length without throwing - JSP-5's
  "mutator on a sealed array" clause, not in this slice.
- `SetWithReceiver` still writes a typed array's element directly when the typed array is the
  target and the receiver differs (the TypedArray `[[Set]]` receiver rule); not changed here.

## Not exercised

The native execution form was not run: it cannot instantiate on this machine
(`ProfileFault/UnsatisfiedHostAssumption`, pre-existing). The changed code is not in `JsNative*`
or `JsBaselineHandlers.cs`. Cross-realm behaviour cannot be exercised in this profile.
