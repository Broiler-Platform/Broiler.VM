# Wave-1 follow-up fixes: JSeal VM-FIX-B

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It is a set of follow-up correctness fixes found during wave 1, not a numbered roadmap slice. It
refines JSP-4 (Number::exponentiate), JSP-6 (references and `super`) and the JSD-0027 follow-up N1,
and does not mark any of those stages complete.

## What changed

1. **`**` follows Number::exponentiate.** The `Exponent` opcode called `System.Math.Pow`, so
   `(-1) ** Infinity`, `1 ** Infinity` and `1 ** NaN` answered `1`. It now calls the same helper
   `Math.pow` already used (`JsRealm.MathPower`, now `internal`), which takes the specification's
   NaN-exponent, zero-exponent and `abs(base) == 1` with infinite exponent cases first. No front end
   folds `**` over constants (the slice parser lowers it to a construct the numeric form refuses and
   the wide compiler always emits the opcode), so there is no compile-time path to fix. The baseline
   native handler steps back into the same interpreter switch.
2. **A computed member reference converts its key once.** New wide-format opcode `ToPropertyKey`
   (`0x5D`, no operand, `[base, key] -> [base, key']`). The compiler emits it after the key of
   `o[k] op= v`, `o[k]++`/`--`/prefix forms, and `o[k] &&= / ||= / ??= v`, before the pair is
   duplicated, so `GetIndex` and `SetIndex` see the converted key. Only an object key is converted
   (only an object can run code); a primitive is left as it is, so the Array element path still
   takes a Number. A nullish base is refused before the key's conversion, as `GetValue` applies
   `ToObject` first. A key object whose `Symbol.toPrimitive` returns a Symbol now works, where it
   used to throw. The opcode is added to `JsOpcodes.All`, the operand-shape and stack-effect tables,
   a baseline handler slot, and `NativeTemplateScanChecks` (class R: guest code only behind
   `ToPrimitive`'s object test); it is not in `RunsAlone` and not in the numeric manifest. The
   public API baseline gains the one enum member, regenerated with `BROILER_API_WRITE=1`.
3. **`super[key]` takes a Symbol key.** `LoadSuperProperty` and `StoreSuperProperty` converted the
   key with the string-only `ToPropertyKey`, so `super[Symbol.replace](...)` in a `RegExp` subclass
   threw "Cannot convert a Symbol value to a string". They now use `JsEngine.ToPropertyKeyValue`
   (String or Symbol) and read or write a Symbol through `GetSymbolWithReceiver` /
   `SetSymbolWithReceiver` with `this` as receiver, a strict failed write throwing `TypeError`.
   The two helpers are added to the guest-code lists in the `RunsAlone` remarks and falsifier and in
   `NativeTemplateScanChecks`, since an R-class arm now reaches them.
4. **`Array.prototype.toLocaleString` is an own method (JSD-0027 N1).** It invokes each non-nullish
   element's `toLocaleString` with no arguments, joins with `,`, charges one unit per element, and a
   non-callable method is a `TypeError` from `Call`. `%TypedArray%.prototype.toLocaleString` now
   runs the same algorithm (length read once, `TypeError` for a non-callable method instead of the
   silent `ToString` fallback JSD-0027 recorded), and its remark in `JsRealm.Binary.cs`, which said
   it matched an `Array` method that did not exist, is corrected. JSD-0027 gains a dated amendment
   saying its section 1 and 2 `toLocaleString` rows describe the tree at `484f389`, and its consumer
   limitation no longer lists the `Array` defect. **N1 is not complete**: see the accounting below.

Fuel charging, cancellation, allocation accounting and guest exception handling are unchanged:
the new paths reuse `ToPrimitive`, `Call`, `Charge` and the existing property helpers.

## Executed on Windows (win-x64, worktree based on 484f389 plus base-vm-w2a.patch)

- Regression first: [`the-reference-key-and-exponent-edges.js`](../../../src/tests/differential/the-reference-key-and-exponent-edges.js),
  39 cases. On the base build, 27 of them answered wrongly against the specification: cases 1-4
  and 7 (exponentiation), 13-20, 22 and 23 (double key conversion; 22 threw), 25-29 (Symbol `super`
  keys threw), 30-35 (inherited `Object.prototype.toLocaleString`) and 37 (`new Int8Array([1])`
  after `Number.prototype.toLocaleString = 5` answered `"1"` through the silent `ToString`
  fallback instead of throwing `TypeError`). Cases 37-39 were added after review, run on the
  separate base build first, and retain the typed-array half of N1; 38 checks that an overridden
  `Number.prototype.toLocaleString` is invoked once per element with no arguments. After the change
  all 39 match the specification. Node v24.17.0 agrees except on cases 13-20, 22 and 23, where Node
  itself converts the key twice, and case 38, where Node's ECMA-402 method passes the locales and
  options arguments on; those carry `#diverges node` declarations naming the reason.
  [windows-node.json](windows-node.json) is the comparison report
  (`python eng/run-differential.py --only the-reference-key-and-exponent-edges --against node --timeout 20`, exit 0).
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267 and Architecture 256/256
  on the base before the change, and again after the change, the API write run and the assurance
  write run.
- Full retained lane `python eng/run-differential.py --timeout 30`: exit 1 only for the known
  code-page cases (`the-json-date-and-regexp-surface` 158-159 and `the-later-library-methods` invalid
  UTF-8 output); every other probe agrees with its retained answers.
- Test262 (pinned ccaac100, bytecode form), per-variant results in
  [test262-comparison.txt](test262-comparison.txt):
  - `exponentiation`, `Math/pow`, `compound-assignment`, `logical-assignment`, `super`, the four
    update-expression subtrees and `Array/prototype/toLocaleString`: the pass count moves from 1334
    to 1378 of 1514 variants. 44 variants move from failing to passing (22 `S11.13.2_A7.*_T4`,
    8 update-expression `A6_T3`, 4 exponentiation `A7`/`A8`, 10 `toLocaleString`); none moves the
    other way.
  - A wider guard selection (`TypedArray/prototype/toLocaleString`, `Array/prototype/join` and
    `toString`, `expressions/assignment`, `object/method-definition`, `statements/class/super`,
    `expressions/class/elements`, `RegExp/prototype/Symbol.replace`, `computed-property-names`),
    run on a separate base build and on this one: identical per-variant results, pass count 4347 of
    4644 on both.

## JSD-0027 N1 acceptance

- The 9 `built-ins/Array/prototype/toLocaleString` tests that need no `resizable-arraybuffer`: met,
  all pass in both modes (the ones that failed before are among the fail->pass lines of the
  comparison).
- The 21 `built-ins/TypedArray/prototype/toLocaleString` tests outside `BigInt/` that need neither
  `resizable-arraybuffer` nor `BigInt`: **20 of 21**, not met. `detached-buffer.js` fails in both
  modes, before and after this change, because this profile's `$262.detachArrayBuffer` refuses
  ("this profile has no ArrayBuffer to detach"): no `ArrayBuffer` can be detached here. That is
  pre-existing and not a `toLocaleString` defect, but N1 does not exclude the test by name, so N1
  stays open until detachment exists or JSD-0027 names the exclusion.
- `TypedArrayConstructors/prototype/toLocaleString/inherited.js`: met, both modes, in a run of
  `python eng/run-test262.py ... --dir test/built-ins/TypedArrayConstructors/prototype/toLocaleString
  --dir test/built-ins/TypedArray/prototype/toLocaleString --dir test/built-ins/Array/prototype/toLocaleString`
  on this build (same suite, cache and flags as above), which also repeats the typed-array result.
- The four typed-array tests and the `Array` ones N1 excludes by name still fail for the stated
  reasons (`BigInt`, resizable buffers); the `BigInt/` tests and `bigint-inherited.js` wait for B07.
- The retained focused cases: probe cases 31 (`"L"`), 34 (`[{toLocaleString: 5}]` throws
  `TypeError`), 37 (the `Int8Array` case throws `TypeError`), and 31 and 32 (nullish elements are
  skipped and their method is not invoked).
  The accounting lines are at the end of [test262-comparison.txt](test262-comparison.txt).

## Not exercised or not addressed

- **The native form was not exercised**: artifacts do not instantiate on this machine
  (UnsatisfiedHostAssumption, pre-existing). The baseline handler for `ToPropertyKey` steps into
  the interpreter switch by construction, which has not been observed here.
- No `test/language/expressions/super` case uses a Symbol key, so fix 3 is shown by the probe
  (cases 25-29) and not by Test262.
- **`super[k] op= v` still converts an object key twice** (`LoadSuperProperty` then
  `StoreSuperProperty`). Converting it once needs the super base captured at reference creation
  (`prop-expr-getsuperbase-before-topropertykey-*` depend on that order), which is a separate change.
- The rest-property double conversion recorded on `BindObjectPattern` is unchanged.
- Remaining failures in these subtrees are unrelated: unary-operand-before-`**` early errors
  (14 variants), direct eval, `with` scopes, cross-realm, BigInt and resizable buffers.
- The malformed-corpus entries JS-5's gate asks for per new opcode are not added for `ToPropertyKey`.
