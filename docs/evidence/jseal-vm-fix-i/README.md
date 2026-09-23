# JSeal VM-FIX-I: follow-up defects recorded by the wave-6 slices

**Date:** 2026-09-22. **Status:** local implementation validation, not accepted milestone evidence.
Base: `HEAD` plus `base-vm-w7.patch` (waves 1-6); before-runs use that base built in the same
worktree before any change, its binaries copied aside.

Five described changes, each with a failing-first regression.

## 1. Exotic hooks and a host's thrown value are translated (JSD-0024 section 19.1)

- `JsHostObject` runs every `IJsHostExotic` hook (`TryGetNamed`, `TryGetIndex`, `TrySetNamed`,
  `SupportedNames`, `IndexedLength`) and the `Unwrap` of the value a hook answers inside one
  translation, `JsHostRealm.HookRaised`: a `JsHostThrowException` is the guest throw it carries, a
  `JsHostSurfaceException` (including a foreign reference or a BigInt answered to a realm that
  declined the surface) is a guest `TypeError`, a `JsHostTerminatedException` re-raises the latched
  abort. The filter is a type test only.
- Every crossing that re-throws a host's `JsHostThrowException` (host body, host job, deletion hook,
  module loader, exotic hooks) resolves the thrown value through one helper, `JsHostRealm.Raised`,
  where a refusal becomes a guest `TypeError`. A host throwing `JsHostValue.BigInt(..)` into a realm
  that declined BigInt, or throwing another realm's reference, no longer ends the invocation.
- No public member changed; the API baseline is untouched.
- **Host-surface rows** (five new): three exotic rows (named/indexed reads that throw, are refused
  or answer a foreign value, writes that throw or are refused, enumeration hooks that throw or are
  refused, a hook whose guest callback exhausts the allowance), a thrown BigInt and a hook's BigInt
  reaching an admitting realm, and the returned/thrown/hook BigInt routes into a declining realm.
  Against the base profile four of them failed with `ProfileFault/ProfileContractViolation`
  ([host-surface-base.txt](host-surface-base.txt)); the admitting-realm BigInt row already passed.
  After: [host-surface-after.txt](host-surface-after.txt), the lane reports every check passed
  (73 rows before, 78 after).

## 2. `Array.of` follows its receiver

`IsConstructor(C) ? Construct(C, « len ») : ArrayCreate(len)`, `CreateDataPropertyOrThrow` per item,
strict `Set` of `length`. `Array.from` already built through its receiver (VM-FIX-F); probe case 9
keeps that covered. Test262 `built-ins/Array/of`: 14 variants move to passing.

## 3. `JSON.parse` reviver deletion

Examined, no code defect: `JsObject.DeleteOwnProperty` is the virtual `[[Delete]]` (a `JsProxy`
override runs `ProxyDelete`), so a Proxy holder's `deleteProperty` trap already ran, its `false`
was ignored and its throw propagated - the probe cases 10-13 agree with Node on the base build too.
Only the remark on `JsonReviveInto` is amended to record this; the cases are retained as regressions.

## 4. `Error.prototype`, the native error prototypes, `Date.prototype`, `RegExp.prototype`

They are ordinary objects (ES2026 20.5.3, 20.5.6.3, 21.4.4, 22.2.6), so they now carry class name
`Object`: `Object.prototype.toString` answers `[object Object]` for each. `Date.prototype.getTime()`
already threw the `TypeError` (brand checks use the Date type, not the class name). `RegExp.prototype`
had the same defect on the same line and is included; `RegExp.prototype.source`/`flags`/`toString`
are unchanged (probe case 18). Test262: 16 Error/NativeErrors variants and
`RegExp/prototype/15.10.6.js` (2) move to passing.

## 5. The three whole-run failures named by the wave-6 records

- `TypedArray/prototype/toString.js`: **already passes at this base** (fixed by B06's intrinsic
  toString, merged into `base-vm-w7`); nothing to change.
- `TypedArrayConstructors/ctors/object-arg/iterated-array-with-modified-array-iterator.js`: VM
  defect. The typed-array constructor's Array drain stood in for the iterator whenever
  `@@iterator` was `Array.prototype.values`, but since F06 the array iterator's `next` lives on the
  shared `%ArrayIteratorPrototype%`, which a program may replace. The drain now also requires that
  prototype's own `next` to be the intrinsic (`JsRealm.ArrayIterationIsIntrinsic`). Fixed.
- `ArrayBuffer/data-allocation-after-object-creation.js`: VM defect. The constructor allocated
  before reading `new.target.prototype`. It now follows AllocateArrayBuffer: refuse a length past
  `maxByteLength`, read the prototype, then allocate (`BuildsFromNewTarget`, so the engine does not
  read it a second time). Fixed. Node reads the prototype before the length-vs-maximum RangeError;
  the pinned spec throws first, so probe case 24 declares that divergence.

## Commands and results

All from the worktree root, Release.

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build`;
  build; `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256
  before and after.
- Slice-compiler `--checks`: the same 371 checks pass and the same 2 are not run on this machine, before and after.
- `python eng/run-cli-acceptance.py --binary-directory src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0`:
  225 of 225 before and after.
- `ExecutionOnly --corpus src/tests/corpus/js-1`: 22 checks before and after (no lowering change,
  corpus not regenerated).
- `Cli --host-surface`: the base build passes its 73 rows; the five new rows against the base profile fail four; after, all 78 pass.
- New probe `src/tests/differential/the-follow-up-built-in-fixes.js` (24 cases):
  `python eng/run-differential.py --only the-follow-up-built-in-fixes --against node --timeout 20`
  agrees with Node except the declared case 24; against the base binaries
  (`--binary-directory` the copied base CLI) it differs from the retained answers in 10 cases
  (1, 2, 5, 6, 7, 8, 14, 15, 20, 22). The whole differential lane (`python eng/run-differential.py`)
  differs only in the known code-page case 36 of `the-later-library-methods`.

## Test262

[test262-comparison.txt](test262-comparison.txt). Pinned suite, bytecode form, `--shards 1 --jobs 4`.

- Selection 1, `built-ins/{Array,JSON,Error,Date,Object/prototype/toString,TypedArray,TypedArrayConstructors,ArrayBuffer,Proxy,NativeErrors}`
  (6732 files, 13326 variants): passed moves from 12865 to 12899; 34 variants newly pass, none
  newly fails; 30 skipped before and after.
- Selection 2, `built-ins/RegExp`, `built-ins/Function/prototype/toString`, `annexB/built-ins/RegExp`
  (2021 files, 4000 variants): passed moves from 2460 to 2462; 2 newly pass, none newly fails;
  4 exhausted and 40 skipped before and after.

## Not exercised

- The native execution form (pre-existing `ProfileFault/UnsatisfiedHostAssumption` on this machine).
- The whole Test262 suite; the floor file was not re-based.
- The host crossing has no Test262 coverage; it is exercised only by the host-surface lane.
- Still failing and out of this change's scope: `Date/prototype/toJSON/*` (the method refuses a
  non-Date receiver; it is generic in the specification), `Array.prototype.{entries,keys,values}`
  with a nullish `this`, `indexOf`/`lastIndexOf` with `-0`, `Error.isError` (absent), the JSON
  `rawJSON`/reviver-context proposal files, and everything needing `$262.createRealm` or
  `SharedArrayBuffer`.
