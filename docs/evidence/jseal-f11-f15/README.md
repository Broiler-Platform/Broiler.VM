# JSP-7 / JSW-6 iterator surface: JSeal slices F11-F15

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the `Iterator` global, the iterator prototype hierarchy and the eleven helpers on
`Iterator.prototype`; it does not mark JSP-7 or JSW-6 complete.

## What changed

- **F11, hierarchy.** Each built-in iterator kind now has its own prototype
  (`%ArrayIteratorPrototype%`, `%StringIteratorPrototype%`, `%MapIteratorPrototype%`,
  `%SetIteratorPrototype%`, `%RegExpStringIteratorPrototype%`), each inheriting from the one
  `%Iterator.prototype%` and carrying a branded `next` and its `Symbol.toStringTag`. Before, every
  iterator was an ordinary object with its own `next` closure directly on `%Iterator.prototype%`,
  so a property installed for one kind reached all of them (probe case 5) and `%Iterator.prototype%`
  carried a throwing `next` the language does not give it. The per-kind steps are unchanged; the
  shared `next` adds the brand check, a done latch (an exhausted Array iterator stays done when the
  array grows, case 11) and the running-generator `TypeError` on re-entry. Typed-array iterators
  share `%ArrayIteratorPrototype%`.
- **F11, constructor.** `Iterator` is abstract (`TypeError` when `new.target` is undefined or
  `Iterator` itself; subclassable); `Iterator.prototype` is non-writable/non-configurable;
  `Iterator.prototype.constructor` and `[Symbol.toStringTag]` are accessor pairs whose setter is
  `SetterThatIgnoresPrototypeProperties`; `Iterator.from` uses `GetIteratorFlattenable` (strings
  iterated, other primitives refused), answers an iterator that already inherits from
  `Iterator.prototype` as itself (prototype walk metered, proxy traps honoured) and wraps anything
  else with `%WrapForValidIteratorPrototype%` (`next`, `return`).
- **F12.** `%IteratorHelperPrototype%` (`next`, `return`, tag `"Iterator Helper"`) over a native
  state machine with the four generator states; `return` before the first `next` completes and
  then closes the underlying iterator, after it resumes the closure with a return completion;
  re-entry is a `TypeError`. `map` and `filter` step lazily with a callback index.
- **F13.** `take` and `drop`: `ToNumber` before `next` is read, `NaN` and negative limits are
  `RangeError`s that close the receiver, `ToIntegerOrInfinity` (so `-0.5` is zero, `Infinity`
  kept). `take` closes the underlying iterator when its limit is reached, without a further `next`;
  `drop` skips with `IteratorStep` (never reading `value`), one metered guest call per skipped
  element.
- **F14.** `flatMap` with `GetIteratorFlattenable(~reject-primitives~)`; an inner step failure
  closes the outer iterator; `return` closes inner then outer, and an inner close failure wins
  while the outer is still closed quietly.
- **F15.** `reduce` (absent initial value = argument count, empty source `TypeError`), `toArray`,
  `forEach`, `some`, `every`, `find`. Short circuits close through the propagating close; callback
  failures close quietly and rethrow the callback's error.
- **Also on the constructor: `Iterator.concat`.** The pinned edition places it on `Iterator` and
  the pinned suite treats `iterator-sequencing` as a standard feature, so leaving it absent would
  have published a partial constructor. It reuses the helper state (empty
  `[[UnderlyingIterators]]`, lazy opening, `return` closes only the open iterable).
- Every callback-taking helper checks callability, and `take`/`drop` convert the limit, BEFORE
  `next` is read (`GetIteratorDirect` reads `next` once). No helper materializes its input; the
  unbounded cases below end with the fuel allowance.
- `src/Broiler.VM.Profile.JavaScript/docs/realm/globals.txt` was regenerated (`--globals --write`)
  and now lists `Iterator`; the N17 absent list never named it.

## Executed on Windows

- New differential probe `src/tests/differential/the-iterator-helpers.js` (103 cases), written
  first. On the base build the host answered 98 of the 103 differently from Node v24.17.0
  ([probe-before.txt](probe-before.txt)). After: all agree with Node except the three declared
  `Iterator.concat` cases 100, 102 and 103 (Node v24.17.0 has no `Iterator.concat`; case 101 agrees
  because both refuse). `python eng/run-differential.py --only the-iterator-helpers --against node
  --timeout 20` exits 0.
- Full retained lane (`python eng/run-differential.py --timeout 60`): the only failures are the
  pre-existing code-page-850 ones (`the-json-date-and-regexp-surface` 158-159,
  `the-later-library-methods` invalid UTF-8); no retained answer changed.
- Unbounded inputs under the CLI's default allowance: `drop(Infinity).next()`, `toArray()` and a
  never-selecting `filter(...).next()` over an endless iterator each end with
  "AllowanceExhausted on Fuel" (exit 5) in under three seconds.
- Pinned Test262 (`ccaac100`), bytecode form, `--shards 1 --jobs 1`, before -> after (see
  [test262-comparison.txt](test262-comparison.txt)):
  - `test/built-ins/Iterator` (936 variants): the pass count moves from 14 to 830; fail 818 -> 2;
    unsupported 20 both times; skipped 84 both times. The 2 remaining failures are
    `proto-from-ctor-realm.js` (`$262.createRealm` refuses in this host). The 20 unsupported are
    the BigInt literal in `concat/iterable-primitive-wrapper-objects.js`. The 84 skipped claim the
    proposed features `joint-iteration` (`Iterator.zip`, `zipKeyed`) and
    `explicit-resource-management` (`Iterator.prototype[Symbol.dispose]`), neither implemented.
  - Spot-check `ArrayIteratorPrototype`, `MapIteratorPrototype`, `SetIteratorPrototype`,
    `StringIteratorPrototype`, `GeneratorPrototype`, `RegExpStringIteratorPrototype` (260
    variants): the pass count moves from 198 to 244, no Passed -> Failed.
  - Wider regression selection (Array `values`/`keys`/`entries`/`Symbol.iterator`/`from`, String
    `Symbol.iterator`/`matchAll`, `RegExp.prototype[Symbol.matchAll]`, `Map`, `Set`, TypedArray
    `values`/`keys`/`entries`, `Object.prototype.toString`, `Promise.all`,
    `language/statements/for-of`, `language/expressions/yield`, `language/expressions/array`,
    `language/destructuring`; 3543 variants), base build -> this patch: the pass count moves from
    3266 to 3272, no Passed -> Failed (the six are the Array `iteration-mutable.js` cases fixed by
    the latch).
- `dotnet test Broiler.VM.slnx -c Release --no-build` after the assurance write run and a rebuild:
  Contract 267/267, Architecture 256/256 (the H3 file count moves from 184 to 186 for the two new
  realm files).

## Not exercised or still failing

- The native form was not exercised: it cannot instantiate on this machine
  (ProfileFault/UnsatisfiedHostAssumption, pre-existing). The iterator built-ins have no separate
  native implementation.
- Cross-realm prototype selection (`proto-from-ctor-realm.js`) needs `$262.createRealm`, which this
  host refuses.
- `Iterator.zip`, `Iterator.zipKeyed` and `Iterator.prototype[Symbol.dispose]` are proposals in the
  pinned suite and are absent. Async iterator helpers are out of scope.
- Still failing in the spot-check and unchanged by this patch: 14 `RegExpStringIteratorPrototype`
  variants (the iterator does not dispatch through a custom `exec`) and 2
  `ArrayIteratorPrototype/next/detach-typedarray-in-progress.js` variants (no detach check in the
  Array iterator step).
- `toArray` is bounded by the fuel meter (one charge per step and per element); the realm has no
  separate allocated-bytes accounting for Array growth, the same as `Array.from`.
