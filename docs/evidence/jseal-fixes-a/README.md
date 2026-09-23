# JSeal follow-up fixes VM-FIX-A

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers three follow-up fixes found during wave 1; none of them is a numbered roadmap slice, and
nothing here marks a JSP/JSW stage or a milestone complete. This record describes the patch after
its adversarial review (revision of 2026-09-21); where the first revision differed, it says so.

## What changed

1. **End-user host output encoding.** `Broiler.VM.Composition.JavaScript.Cli` replaces each
   standard writer **whose stream is redirected** (a pipe or a file) with a UTF-8 writer (no
   byte-order mark, auto-flushed) at the top of `Main`, instead of letting the runtime encode with
   the console's code page. A stream that is a console window keeps the runtime's writer: the raw
   stream under a console window is decoded with the window's own code page, so UTF-8 written there
   showed `"é"` as `"├®"` on code page 850 (the first revision did that; review finding). It does
   not set `Console.OutputEncoding`, which would change the console window's code page after the
   process exits. JSD-0017 section 3 records the rule and the trade: every byte a caller can capture
   is deterministic UTF-8, and an interactive console shows what its code page can display (on code
   page 850 `"é"` correctly and `"∛"` still as `?`). Three rows under `src/tests/cli/encoding/` pin
   the redirected half (`print`, completion value, uncaught error on standard error).
2. **`$262.detachArrayBuffer` in the conformance harness.** New public member
   `JsHostRealm.DetachArrayBuffer(JsHostValue)` performs the profile's own detach (the act
   `ArrayBuffer.prototype.transfer` performs), charged through `Enter` like every crossing; a
   non-`ArrayBuffer` is a guest `TypeError`. The conformance composition's new `Test262Host`
   (an `IJsHostSurface`) replaces only the `detachArrayBuffer` member of the profile's existing
   `$262`; every other member still refuses. It is installed only in runs that load the suite's
   harness, which build their descriptor with `DescriptorHostingRealms` and register
   `HostSurfaceCapability`. The end-user host registers neither, so ordinary guests keep the
   refusing stub (its stale message "this profile has no ArrayBuffer to detach" is corrected).
   Recorded in JSD-0024 section 14 and `docs/api/public-api.txt`. The command-line root's
   `--host-surface` lane now exercises the member (see below).
3. **A typed array whose buffer is detached, and numeric keys that are not indices.**
   `JsTypedArray` no longer hides its ordinary properties when detached: only numeric keys are
   emptied, so `a.foo = 1` and `a.constructor = X` are stored and read back, listed by
   `Reflect.ownKeys`, and deletable. Doing that exposed that numeric keys which are not array
   indices (`"-0"`, `"1.5"`, `"-1"`, `"NaN"`, `"Infinity"`) had always reached the ordinary map.
   A bounded `CanonicalNumericIndexString` check (`JsTypedArray.IsNumericKey`) routes them through
   the integer-indexed branch in the own-property overrides **and** in the engine's `[[Get]]`,
   `[[HasProperty]]` and `[[Set]]` walks (`JsEngine`; the first revision stopped at the overrides,
   so the walks still reached the prototype chain): such a key is never read off the chain, never
   reaches an inherited setter, and an assignment still runs `ToNumber` before discarding. A typed
   array met part-way up another object's chain answers a numeric key itself, and `Reflect.set`
   with a receiver other than the view converts nothing for a key that names no element.
   `[[DefineOwnProperty]]` (`ObjectApplyDescriptor`) refuses a numeric key that names no element
   (detached, out of range or non-index), refuses a descriptor an element cannot take, and converts
   a defined value with the engine's `ToNumber`. Enabling a real detach also turned previously
   unearned passes into honest failures, and they are fixed: `fill` and `copyWithin` revalidate
   after converting their arguments, the `TypedArray(buffer, byteOffset, length)` constructor
   converts `length` before its detached check, and `DataView` and the nine typed array
   constructors read `new.target.prototype` themselves at the specification's point (before the
   final detach check), flagged by `JsNativeFunction.BuildsFromNewTarget` so the engine does not
   read it a second time.

## Executed on Windows

Everything below was run on the final tree of this revision. The logs are retained outside the
repository under `D:/wt/t262/vm-fixes-a/` (`gate-r2.log`, `cli-acceptance-r2.log`,
`host-surface-r2.log`, `differential-r2.log`, `probe-node-r2.log`, `r4-a.log`, `r4-b.log`).

- Base: `dotnet build Broiler.VM.slnx -c Release`; `dotnet test Broiler.VM.slnx -c Release --no-build`:
  Contract 267/267, Architecture 256/256. After (gate mode, after `BROILER_API_WRITE=1` for rule
  N10 in the first revision and `BROILER_ASSURANCE_WRITE=1`): Contract 267/267, Architecture 256/256.
- `python eng/run-cli-acceptance.py`: 209 of 209 command lines before. The three new encoding rows
  failed on the unchanged host (standard output held `caf� ?8=2 ??`, standard error
  `gr��er als 8`); after the fix 212 of 212 answer as declared.
- Interactive console on code page 850 (`cmd /k chcp 850 & broiler-js --quiet enc.js` in its own
  console window, screen buffer read back with `ReadConsoleOutputCharacterW`; script
  `console-read.ps1`, output `console-cp850-base.txt` and `console-cp850-r2.txt`): the unchanged
  host and this revision both show `café naïve` and `uncaught Error: größer`. The first revision
  showed `caf├® na├»ve` there. Piped, this revision writes the UTF-8 bytes `c3 a9` for `é`.
- `--host-surface` lane: every check passes, including the new ones for
  `JsHostRealm.DetachArrayBuffer`. A buffer the guest handed over is detached (views read empty, a
  second detach is a no-op, a typed array and a number are guest `TypeError`s), a buffer kept from a
  previous check's realm is refused with `ForeignRealm`, and a detach from outside a step is
  refused with `RealmNotCurrent`. These are new coverage for a member this patch adds, so there is
  no earlier failing run.
- `python eng/run-differential.py --timeout 20` on this machine's default code page 850: before,
  `the-later-library-methods` failed with "invalid UTF-8 output" and
  `the-json-date-and-regexp-surface` with differing answers. After, the only failure is the known
  retained case 36 of `the-later-library-methods` (cube root spelling, `1.259921049894873` against
  the retained `1.2599210498948734`), whose retained answer is deliberately unchanged.
- Probe `src/tests/differential/the-detached-typed-array.js` (43 cases), checked against the
  pinned ES2026 steps (the TypedArray exotic object's internal methods,
  `CanonicalNumericIndexString`, `TypedArraySetElement`, `%TypedArray%.prototype.fill` and
  `.copyWithin`, `InitializeTypedArrayFromArrayBuffer`, `OrdinaryCreateFromConstructor`,
  `AllocateTypedArray`) and compared with Node v24.17.0 (`--against node`, ok): 42 agree and case
  40 is a declared divergence. Node calls `valueOf` for `Reflect.set(view, "1.5", v, other)`,
  where ES2026 converts nothing and the pinned suite asserts it
  (`internals/Set/key-is-canonical-invalid-index-reflect-set.js`). On the unchanged profile
  cases 1-7, 9-12, 17-20, 23, 26-35, 38 and 40-43 answered differently.
- Pinned Test262 (`ccaac100`), bytecode form, `--shards 1 --jobs 1`; summary and every variant
  that moved in [test262-comparison.txt](test262-comparison.txt):

  | Selection | Variants | Passing before | First revision | This revision |
  |---|---|---|---|---|
  | A: `ArrayBuffer`, `TypedArrayConstructors/internals`, `TypedArray/prototype`, `DataView` | 4745 | 1918 | 2213 | 2271 |
  | B: `TypedArrayConstructors`, `harness`, `Reflect`, `Object`, `Atomics` | 9540 | 7718 | 7767 | 7823 |

  In A, 355 variants move from failing to passing; 149 of the 178 files among them include
  `detachArrayBuffer.js`. In `TypedArrayConstructors/internals` (454 variants) the pass count
  moves from 84 to 187. `DataView/custom-proto-access-detaches-buffer.js`, which the first
  revision had turned from passing to failing, passes again in both modes.
  Two variants move from passing to failing, in both selections:
  `internals/DefineOwnProperty/conversion-operation-consistent-nan.js` (both modes). Its pass was
  unearned. Under `makeIterable` the typed array constructor ignores `@@iterator` and builds a view
  of length 0 (a pre-existing constructor gap: `new Float64Array({[Symbol.iterator]: ...})` has
  length 0 on the base as well, where Node answers 1), and `Object.defineProperty(samples, "0",
  ...)` on that empty view used to report success silently. It now throws the `TypeError` the
  specification requires for an index the view does not have. The whole-suite floor file is not
  touched.
- Native execution form: not exercised (pre-existing ProfileFault/UnsatisfiedHostAssumption).

## Not run, and what is still wrong

- A whole-suite Test262 run, the Linux lane, and Broiler.JS as a comparison engine.
- The typed array constructor's object form reads `length` and ignores `@@iterator` (above).
- `Reflect.set(view, i, v, otherTypedArray)` for a valid index lands on the receiver through the
  object model, which converts an object value without the engine (`NaN` where `ToNumber` would
  run `valueOf`): `internals/Set/key-is-valid-index-reflect-set.js` still fails.
- BigInt typed arrays, `$262.createRealm` and resizable buffers are absent, so their variants
  keep failing or are unsupported.
