# JSP-6 array species and concat spreading: JSeal slices V07, V08, V09

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSP-6 and does not mark that stage complete.

## What changed

- `JsRealm.Array.cs` gains a shared `ArraySpeciesCreate` (IsArray through Proxy, `constructor`
  read, `Symbol.species` read, null/undefined fallback to `ArrayCreate`, constructability check,
  `-0` length normalised to `+0`, construction through `JsEngine.Construct` so fuel, call depth and
  guest exceptions stay on the ordinary path), `ArrayCreate`, and `ArrayCreateDataAt`
  (`CreateDataPropertyOrThrow`: define rather than assign, refusals are `TypeError`, a Proxy is
  asked through its `defineProperty` trap with the four-field descriptor).
- V07: `map` and `filter` build through it. `map` no longer writes a `length` on its result.
- V08: `slice`, `splice`, `concat`, `flat`, `flatMap` build through it, with the per-method order
  (slice: both bounds before `constructor`; splice: removed elements defined before the receiver
  is changed; flat: `length`, then depth, then `constructor`). `slice`/`splice`/`concat` still
  `Set` the result's `length` strictly; `map`/`filter`/`flat`/`flatMap` do not. `FlattenIntoArray`
  now threads the target index and refuses an index at 2^53-1. `toReversed`, `toSorted`,
  `toSpliced` and `with` are unchanged and still return plain Arrays without reading `constructor`.
- V09: `concat` uses `IsConcatSpreadable` for the receiver and each argument: one `[[Get]]` of
  `Symbol.isConcatSpreadable` (inherited flags, getters and Proxy `get` traps observed, exceptions
  propagated), `ToBoolean` when defined, otherwise IsArray. A spread operand's length is `ToLength`
  and a result past 2^53-1 is a `TypeError` before copying. Every visited index is charged, holes
  included, and holes stay holes.
- `JsArray.SetOwnProperty`: a plain-data definition of an index that the ordinary map held (a
  read-only or accessor element) now retires the map entry before writing the dense slot. Without
  it the four `target-array-with-non-writable-property.js` Test262 files regressed once
  `CreateDataPropertyOrThrow` replaced assignment.

## Regression probe

`src/tests/differential/the-array-species-and-spreading.js` (61 cases). Before the change 42 of
61 answers differed from Node 24.17 (checked case by case against the specification). After the
change all 61 agree, and
`python eng/run-differential.py --probe-directory <dir with only this probe> --against node --timeout 20`
reports `ok: true`. The retained answers were written with `--write` only after that check.

The full retained lane `python eng/run-differential.py --timeout 10` still exits non-zero for two
probes this change does not touch: `the-later-library-methods.js` (the known case 36 cube-root
spelling, reported as invalid UTF-8 output because of case 147) and
`the-json-date-and-regexp-surface.js` (cases 158-159, lone-surrogate/emoji output). The same two
failures reproduce with the main-tree binary at `484f389`, so they are pre-existing here.

## Test262 (pinned suite ccaac100, bytecode form, `--shards 1 --jobs 1`)

`test/built-ins/Array/prototype/{map,filter,slice,splice,concat,flat,flatMap,toReversed,toSorted,toSpliced,with}`,
1613 variants:

| | pass | fail | unsupported | exhausted |
|---|---|---|---|---|
| before | 1314 | 285 | 6 | 8 |
| after | 1497 | 102 | 6 | 8 |

183 variants moved from fail to pass and none from pass to fail. By method, passes before -> after:
map 381 -> 399, filter 444 -> 462, slice 98 -> 118, splice 116 -> 138, concat 52 -> 133,
flat 30 -> 38 (all pass), flatMap 31 -> 47 (all pass). toReversed 30, toSorted 36, toSpliced 54,
with 42 are unchanged.

A wider run over `test/built-ins/Array`, `Object/defineProperty`, `Object/defineProperties`,
`Reflect/defineProperty` and `Object/freeze` (9757 variants), to cover the `JsArray` change:
before pass 8884 / fail 813 / unsupported 44 / exhausted 16; after pass 9067 / fail 630 /
unsupported 44 / exhausted 16. 0 regressions, 183 fixes (the same 183).

The native form (`--form native`) was run on the same prototype subset before the change: all
1607 runnable variants fail with "the artifact would not instantiate"
(`ProfileFault/UnsatisfiedHostAssumption`) under the wide manifest, so it gives no signal for these
methods. The changed code is not in `JsNative*` or `JsBaselineHandlers.cs`. It was not re-run after.

## Still failing, and why

- 20 variants are cross-realm (`create-proto-from-ctor-realm-*`): `$262.createRealm` refuses in
  this profile. The cross-realm `%Array%` step of `ArraySpeciesCreate` is vacuous with one realm
  per engine. It is **not exercised**, and those variants are counted as failures, not as coverage.
- 18 variants need resizable ArrayBuffers or `BigInt64Array`.
- 64 variants (map, filter, slice, splice, and the four change-by-copy length-limit files) fail
  because the file-wide `ArrayLengthOf` reads `length` with `ToUint32` rather than `ToLength`.
  Examples are `create-*-invalid-len` RangeErrors, `length: Infinity`, `length: -4294967294`,
  and lengths near 2^53. Only `concat`'s spread path uses `ToLength` in this change. The file-wide
  fix is follow-up work outside these slices.

## Gates

`dotnet build Broiler.VM.slnx -c Release`: 0 errors. `BROILER_ASSURANCE_WRITE=1 dotnet test ...`
was run, then a rebuild and `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267,
Architecture 256/256.
