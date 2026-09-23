# JSeal VM-FIX-J: small defects recorded by VM-FIX-I, F09 and B07

**Date:** 2026-09-22. **Status:** local implementation validation, not accepted milestone evidence.
Base: `HEAD` plus `base-vm-w10.patch` (waves 1-9); before-runs use that base built in the same
worktree before any change, its binaries copied aside.

Five items, each with a failing-first regression where behaviour changed.

## 1. `Date.prototype.toJSON` is generic

ES2026 21.4.4.37: `O = ToObject(this)`, `tv = ToPrimitive(O, number)`, a non-finite Number answers
`null`, otherwise `Invoke(O, "toISOString")`. It read a Date's time value and refused every other
receiver. Test262 `built-ins/Date/prototype/toJSON`: 16 variants move to passing (8 files).

## 2. Array iteration methods and the `indexOf`/`lastIndexOf` start

- `Array.prototype.entries`, `keys` and `values` (also `[Symbol.iterator]`, the same function) now
  begin with ToObject, so a nullish receiver throws when the method is called and a primitive is
  iterated as its wrapper.
- `JsValue.ToInteger` is the realm's ToIntegerOrInfinity and returned -0 for -0 (and for values
  truncating to it); the operation answers a mathematical integer, so it now answers +0. This is
  what made `[true].indexOf(true, -0)` answer -0. The infinities and `lastIndexOf`'s `length - 1`
  default (an explicit `undefined` is 0) were already right and are now pinned by probe cases.
  The change is shared by every caller of ToIntegerOrInfinity; the wide Test262 selection below
  shows no variant changing from passing.
- After review: the array iterator's step (`CreateIndexedIterator`) now measures a non-typed-array
  receiver with LengthOfArrayLike (`ToLength`) instead of `ToNumber`, so `{ length: 2.7 }` yields
  two elements and `{ length: "2.5" }` two keys; and a `keys` iterator yields the index without
  reading the element, so a getter at that index is not run (ES2026 %ArrayIteratorPrototype%.next).
  Both were pre-existing; probe cases 51-55 pin them (the base answered 51, 52 and 54 wrongly).

## 3. VM-FIX-I review minors

- `BinaryDrainArrayValues`: the first remark paragraph said the iterator "carries its own native
  `next`", contradicting the paragraph rewritten by VM-FIX-I; it now says the caller checks both
  `values` and `%ArrayIteratorPrototype%.next`.
- `CloneIsErrorPrototype` removed: the Error and NativeError prototypes have class name `Object`
  since VM-FIX-I, so the class-name test alone is the `[[ErrorData]]` test (a comment says so). The
  `CloneWriter.realm` field it was the only reader of is removed with it. Behaviour unchanged; the
  existing clone rows of the host-surface lane pass.
- Crossing accounting: every `IJsHostExotic` hook (`TryGetNamed`, `TryGetIndex`, `TrySetNamed`,
  `SupportedNames`, `IndexedLength`) now enters one crossing (`JsHostRealm.TryEnterHook`, `Enter(1)`
  inside a step) inside the hook's translation, as the deletion hook and the module loader already
  did. Documented as JSD-0024 section 19.2. Two host-surface rows: an ordinary property of an exotic
  object read 1000 times under a 200 `HostCalls` ceiling completes (no crossing), and a named read
  1000 times under the same ceiling ends in ResourceExhaustion. With `JsHostObject.cs` at its base
  content the second row failed (the run completed normally).
- After review: outside every step the hook is not asked and nothing is charged or raised. The
  first revision gated those reads too, and the only reads that reach a hook then are the engine's
  own, rendering an uncaught value or a completion value after the step closed: an exotic object
  thrown uncaught ended as `ProfileFault/ProfileContractViolation` instead of the ordinary
  `ProfileFault/ProfileFaultUnspecified`, and a script completing with one faulted. Two rows cover
  them (the `Check` helper gained an optional expected reason); both failed against the first
  revision and pass now. JSD-0024 section 19.2 states the outside-a-step rule.
  [host-surface.txt](host-surface.txt): 78 rows at the base, 82 after, every row passing.

## 4. F09 matcher minors (documentation only, no code change)

- The metering unit is now stated in `JsRegExpMatcher`'s remarks: one step is one instruction
  dispatched or one character a greedy run examines. A run's character costs what an `Op.Set`
  dispatch costs because it does the same read and class test (a property-escape class's weight is
  added in `TakeSet` for both); the four other instructions the replaced loop dispatched per
  character were bookkeeping a run does not do, so they are not charged. Kept at one step, deliberately.
- Memory: a backtrack frame is five 32-bit fields including `Floor`, 20 bytes, so one match's frame
  array tops out at 2^20 x 20 bytes = 20 MiB (16 MiB before `Floor`); the undo trail at 16 MiB.
  Neither is charged to `LiveBytes` - pre-existing, recorded in the remarks, not changed.

## 5. `Array.of` over a BigInt typed array constructor

`Array.of.call(function () { return new BigInt64Array(2); }, 1)` already throws a `TypeError` at
this base (CreateDataPropertyOrThrow reaches ToBigInt(1)), and with `1n` the strict `length` write
throws, as in Node. No code change; probe cases 48-50 retain it.

## Regression probe

New `src/tests/differential/the-generic-to-json-and-array-search.js` (55 cases, retained answers
written after checking each against the steps and Node). The base build's answers
([probe-before.txt](probe-before.txt)) differ in 23 cases (4-13, 18, 20, 22-25, 30-33, 51, 52, 54).
`python eng/run-differential.py --only the-generic-to-json-and-array-search --against node --timeout 20`
agrees with Node, no declared divergence; against the base binaries it fails the retained answers.
The whole differential lane differs only in the known code-page case 36 of
`the-later-library-methods` (identical on the base build).

## Commands and results

All from the worktree root, Release.

- `dotnet build Broiler.VM.slnx -c Release`; `dotnet test Broiler.VM.slnx -c Release --no-build`:
  Contract 267/267, Architecture 266/266 before; after the assurance write run
  (`BROILER_ASSURANCE_WRITE=1`) and a rebuild, Contract 267/267, Architecture 266/266 (re-run the
  same way after the review fixes, same counts). No product
  file added; no public member, opcode, diagnostic code, rule or decision number taken.
- Slice-compiler `--checks`: 371 checks and 2 not-run rows, as at the base.
- `python eng/run-cli-acceptance.py --binary-directory src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0`: 225 of 225.
- `ExecutionOnly --corpus src/tests/corpus/js-1`: 22 checks; no lowering change, corpus not regenerated.
- `Cli --host-surface`: every row passes (82).

## Test262

[test262-comparison.txt](test262-comparison.txt). Pinned suite, bytecode form.

- Focused (`Date/prototype/toJSON`, `Array/prototype/{entries,keys,values,indexOf,lastIndexOf,Symbol.iterator}`,
  `Array/of`; 465 files, 928 variants): passing moves from 896 to 926; the remaining 2 need
  `$262.createRealm`.
- Wide (`built-ins/{Array,Date,String,TypedArray,TypedArrayConstructors,Number,Math,JSON,ArrayBuffer,DataView,RegExp,Iterator,Object}`,
  `annexB/built-ins`; 14720 files, 29161 variants): passing moves from 27696 to 27728; 32 variants
  newly pass (the focused 30 plus `Array/prototype/methods-called-as-functions.js`), none newly
  fails; 4 exhausted and 161 skipped before and after.

After the review fixes (focused selection plus `built-ins/ArrayIteratorPrototype`; 492 files,
974 variants; base binaries against the fixed build): passing moves from 942 to 972; the remaining
2 are `Array/of/proto-from-ctor-realm.js`. The wide selection was re-run after the review fixes
(`D:/wt/t262/vm-fix-j/after2-wide`) but had not finished when this record was written, so the wide
figures above are those of the first revision only.

## Not exercised

- The native execution form (pre-existing `ProfileFault/UnsatisfiedHostAssumption` on this machine).
- The whole Test262 suite; the floor file was not re-based.
- JSeal's own suites against this build (the exotic-hook charging reaches JSeal only through a
  later package); the hook accounting is exercised by the host-surface lane alone.
- No fuel measurement was taken for item 4: the metering code did not change.
