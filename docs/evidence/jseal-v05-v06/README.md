# Mapped arguments object: JSeal slices V05 and V06

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It relates to JSP-6 (the VM semantic-correctness refinements V04-V12) and does not mark that stage
complete.

## What changed

- **V05, the parameter map.** A sloppy-mode function whose parameter list is simple (no default,
  rest element or pattern) now gets a mapped `arguments` object: indices below
  `min(actuals, formals)` alias the parameter bindings in both directions. The map is a row of
  flags over the function environment record's own slots (`JsMappedArguments` in
  `src/Broiler.VM.Profile.JavaScript/JsObject.cs`), so closures, callbacks and re-entrant calls see
  one value. Strict functions, class code and non-simple lists keep the unmapped object; an arrow
  still has none of its own.
- **Duplicate formals.** A simple list with a repeated name no longer takes the binding prologue.
  Each earlier occurrence gets a hidden `#shadowed` slot, so slot `i` is parameter `i`, the name
  resolves to the last occurrence, and aliasing an earlier position to its unreachable slot is
  indistinguishable from leaving it unmapped, as `CreateMappedArgumentsObject` specifies.
- **V06, the exotic internal methods.** `[[GetOwnProperty]]`/`[[Get]]` answer the live binding;
  a data write or define writes the binding and then disconnects if the property became
  non-writable (so `{ value: v, writable: false }` sets `v` and then disconnects); an accessor
  define disconnects without writing; a successful delete disconnects. A descriptor refused by
  validation never reaches the object, so a failed define leaves the map unchanged. The map has no
  accessor and is not visible to the host API.
- **Artifact format and verifier: unchanged.** No opcode or flag was added; `NewArguments` decides
  from the unit's existing `Strict` and `BindsParameters` bits. Compatibility impact: an artifact
  compiled before this change with a repeated parameter name carries `BindsParameters` and still
  gets the unmapped object when run by this build; recompiling it gives the mapped one.

## What ran (Windows 11, .NET SDK 10.0.401, Release)

- `dotnet build Broiler.VM.slnx -c Release`, then the assurance write run, then gate mode:
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
- New probe `src/tests/differential/the-mapped-arguments-object.js` (64 cases). Before the fix, 33
  cases differed from Node, for example case 1 `f(x){ arguments[0]=9; return x }` answered 1 where
  the language says 9. After the fix, `python eng/run-differential.py --only
  the-mapped-arguments-object --against node` agrees on all 64 cases, and every retained answer was
  checked by hand against the specification's mapped-arguments algorithms.
- `the-general-surface` case 132 now answers 9; its `#diverges node 132` declaration was removed
  as stale and the lane agrees with Node apart from its remaining five declared cases.
- Full retained lane `python eng/run-differential.py --timeout 10`: all probes agree except
  `the-later-library-methods` (known case 36, invalid UTF-8 spelling) and
  `the-json-date-and-regexp-surface` cases 158-159 (non-ASCII output through the console code
  page). Both fail identically on the untouched build and were not touched.

## Test262 (pinned suite ccaac100, bytecode form, one shard)

| Subtrees | Before | After |
|---|---|---|
| `language/arguments-object`, `built-ins/Object/defineProperty`, `defineProperties`, `getOwnPropertyDescriptor`, `language/function-code` (4875 variants) | pass 4812, fail 59, unsupported 4 | pass 4841, fail 30, unsupported 4 |
| of which `language/arguments-object` (460 variants) | pass 432, fail 28 | pass 455, fail 5 |
| of which `built-ins/Object/defineProperty` (2250 variants) | pass 2230, fail 18 | pass 2236, fail 12 |
| `language/statements/function`, `expressions/function`, `statements/generators`, `expressions/arrow-function`, `statements/async-function` (2553 variants) | pass 2452, fail 101 | pass 2453, fail 100 |

Per-variant comparison: 30 variants moved from fail to pass (23 under `arguments-object`, including
the `mapped/` family; `defineProperty/15.2.3.6-4-292-1` to `-296-1`; and
`async-function/evaluation-mapped-arguments`). No variant moved from pass to fail.

The five remaining `arguments-object` failures are outside these slices: two need direct eval in a
function (`10.5-1-s`, `10.5-7-b-1-s`, V14); two need `Function.prototype.caller` behaviour
(`10.6-13-a-2`, `-a-3`); and `10.6-13-c-3-s` expects the **unmapped** object's `callee` accessor to
be non-configurable, where this profile defines it configurable.

## Not exercised

- **The native form.** `--form native` (and the CLI's `--native x86-64-win64`) refuses every
  artifact on this host with `ProfileFault/UnsatisfiedHostAssumption`, including a one-line
  `print(1+1)`, on the untouched build as well (4870 of 4875 variants in that run). No native-form
  result is claimed. Its baseline handler for `NewArguments` steps into the interpreter's own
  dispatch, so the object is built by the same code, but that was not observed here.
- Linux, the whole-suite Test262 run and floors, and direct eval.
