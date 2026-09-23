# JSW-2 half precision: JSeal slices F01, F02 and F03

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSW-2 (binary surface) and JSP-7/JSW-6 (library surface); it does not mark any stage
complete.

## What changed

- **F01, one binary16 conversion.** `JsFloat16` (`JsBinary.cs`) is the only Number/binary16
  conversion in the profile: `Encode` (round to nearest, ties to even, overflow to a signed
  infinity, subnormals kept, signed zero kept, every NaN stored as the quiet NaN `0x7E00`),
  `Decode` (exact) and `Round`. `Math.f16round` now calls `JsFloat16.Round` instead of its own
  cast. The audit found the existing cast correct: the platform's `(System.Half)double` rounds
  once, directly from the double, not through `float`. That was checked with
  [half-rounding-check.cs.txt](half-rounding-check.cs.txt), which compares the cast with an
  integer-arithmetic reference over every binary16 value, every midpoint between neighbours, the
  doubles either side of each midpoint and random points between neighbours. There were no
  mismatches. The double-rounding witness `1 + 2**-11 + 2**-40` rounds to `1 + 2**-10`; going
  through `float` gives 1. No second conversion was written.
- **F02, `DataView.prototype.getFloat16` / `setFloat16`.** A new `JsElementKind.Float16`
  (value 9, 2 bytes) sits in `JsElements.All`, so the accessors come from the existing DataView
  loop and use its path unchanged: brand check, `ToIndex(offset)`, `ToNumber(value)` (setter),
  `ToBoolean(littleEndian)`, then the detached check (TypeError), then bounds (RangeError).
- **F03, `Float16Array`.** The same kind gives the constructor, prototype, `BYTES_PER_ELEMENT`
  (2, frozen on both), indexed reads and writes, views and every shared `%TypedArray%` method,
  including species, through the existing infrastructure. No method got a per-kind copy. The
  element-kind audit covered `WidthOf`, `HoldsBigInts` (still refuses unnamed kinds; Float16 is
  named), `ConstructorNameOf`, `Read`/`Write`, the binary surface table
  (`JsSurfaces.BinaryGlobals`, so a program naming `Float16Array` declares
  `broiler.javascript.binary` and a composition that declines it refuses the artifact) and the
  published inventory `docs/realm/globals.txt` (regenerated; `Float16Array` added). The ledger's
  `absent-globals` block never named it, so rule N17 needed no change. Nothing serializes element
  kinds, and the verifier does not read them.
- **Iterable constructor argument, for every kind** (needed for F03, and follow-up to V10-V12).
  `BinaryConstructTypedArray` now calls `GetMethod(argument, @@iterator)` before the array-like
  path, drains the iterator (`JsEngine.GetIteratorFromMethod`, new, so the method is read only
  once) and then allocates and converts: this is `InitializeTypedArrayFromList`. `undefined` or
  `null` falls through to the array-like path. Any other non-callable value is a TypeError. The
  array-like length is now `ToLength`, not `ToUint32`, so a length of 2**53 is a RangeError
  instead of an empty view.
- **A plain Array argument keeps its old cost** (after review). Driving the protocol for the
  commonest argument, `new XArray(array)`, cost a native `next` call, a result object and two
  property reads per element. When the argument is a `JsArray` and the `Symbol.iterator` just read
  is this realm's own `Array.prototype.values`, `BinaryDrainArrayValues` reads the elements by
  index instead. Nothing observable changes: that iterator carries its own native `next`, `length`
  is re-read before each element, every element goes through `GetIndexed` (holes reach the
  prototype chain, accessors run in index order), and conversion still happens only after the
  drain. An own or replaced `Symbol.iterator`, a subclass that overrides it, a Proxy or any other
  object takes the full protocol. Each element is charged once, in the drain.
- **Host buffer round trip, through the realm API.** A new `--host-surface` check in the CLI
  composition: the host fills a buffer through the realm's own `ArrayBuffer`/`Uint8Array`, the
  guest reads and writes it as a `Float16Array`, and the host reads the bytes back and reads an
  element through a `Float16Array` of its own. Every host step is a public realm call
  (`Construct`, `GetIndex`, `SetProperty`), so bytes cross as Numbers. The host surface has no raw
  view of a buffer's bytes yet (the host-copy contracts I01-I04), so this shows that host and
  guest agree on the layout. It does not show a raw memory copy between them.

Fuel, allocation charging and guest exceptions go through the existing paths
(`BinaryNewTypedArray`, `JsEngine.TryIterateNext`, per-element `Charge`). An infinite iterator,
and an Array whose element getters keep defining the next element, both stop with
`AllowanceExhausted on Fuel`. No public API changed:
`JsSurfaces.BinaryGlobals` gained an element but kept its signature.

## Executed on Windows

- Regression first: [probe-before.txt](probe-before.txt). On the base build the new probe stops
  at its first use of `Float16Array`. Iterable arguments built empty views, and an object with
  both `length` and `Symbol.iterator` was read as an array-like.
- Differential probe `src/tests/differential/the-float16-surface.js` (67 cases; 62-67 were added
  after review for the Array argument: holes through the prototype chain, a getter that shortens
  or grows the array, all reads before any conversion, own and replaced `Symbol.iterator`, a
  single read of an accessor `Symbol.iterator`, a subclass, a throwing getter), run with
  `python eng/run-differential.py --only the-float16-surface --against node --timeout 20`.
  It decodes all 65536 bit patterns through a DataView and a Float16Array alias and compares them
  with a decoding written from the format's definition (2046 NaN patterns). It re-encodes every
  non-NaN pattern and checks `Math.f16round` is the identity on each. It rounds all 31743
  positive midpoints, their neighbouring doubles and their negations through `Math.f16round`,
  plus a boundary sample through all three paths. After the change it agrees with Node 24.17 on
  every case ([float16-node.json](float16-node.json)), and every retained answer was checked
  against the specification, not only against Node.
- Cost of a plain Array argument, measured with the CLI (Release) against the base build
  (HEAD + wave-1 patch). `--fuel` is the smallest allowance that finishes, found by bisection:
  - `new Float64Array(new Array(100000).fill(0.5))`: base 1,000,439; iterator protocol (first
    submission) 1,600,104; with the Array drain 1,000,439. The fill alone needs 100,084.
  - `new Float64Array(new Array(1000000).fill(0.5))`: base 10,004,560; with the Array drain
    10,004,560.
  - `new Float32Array(a)` over a million-element Array, timed inside the program: base 52-59 ms;
    iterator protocol 519-569 ms; with the Array drain 59-81 ms.
- `the-later-library-methods.expected.txt`: the `#diverges node 254/255` declarations said Node
  predates `Math.f16round`. Node 24.17 has it and answers what the retained lines hold, so both
  declarations were stale and are removed. The lane cannot confirm this itself while that file
  fails on the pre-existing invalid UTF-8 output; cases 254 and 255 were compared by hand.
- The full retained lane (`python eng/run-differential.py --timeout 60`) fails only in the
  pre-existing code-page places (`the-json-date-and-regexp-surface` 158,
  `the-later-library-methods` invalid UTF-8). `the-binary-species`, `the-general-surface` and
  `the-later-library-methods` were also compared against Node, with no new finding.
- `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passes, including the
  new buffer round trip.
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267 and Architecture 256/256
  before and after (after the assurance write run and a rebuild).
- Pinned Test262, bytecode form, `--shards 1 --jobs 1`, before and after on binaries copied from
  each build. The per-directory table is in [test262-comparison.txt](test262-comparison.txt).
  No variant went from passing to failing in any run:
  - `Math/f16round` + `DataView/prototype/{getFloat16,setFloat16}`: pass 24 -> 80, fail 74 -> 18
    (14 are `$262.detachArrayBuffer`, 4 are resizable). `Math/f16round` was already 10/10.
  - `TypedArrayConstructors`: pass 552 -> 594, fail 677 -> 635 (22 more pass in `ctors/object-arg`).
  - `TypedArray`: pass 1042 -> 1410, fail 1266 -> 898. Every non-BigInt, non-resizable
    `speciesctor` variant now passes (88 -> 112 passing); the rest were blocked on `makeIterable`.
  - After the review fix (Array argument drain), `TypedArrayConstructors` and `TypedArray` were
    re-run on the rebuilt binaries: the pass count stays at 594 and 1410, and no variant's result
    changed against the run before the fix.
  - Spot check (`staging/sm/{TypedArray,Math}`, `harness`, `Array/prototype/concat`, `DataView`,
    `Math`): pass 1697 -> 1755.
- The harness adds `Float16Array` to its constructor list only when the global exists, and it
  reports only the first failing kind. So each of the 141 still-failing in-scope files
  (TypedArray + TypedArrayConstructors, excluding BigInt, resizable, SharedArrayBuffer and
  cross-realm) was re-run with the list narrowed to `Float32Array` and then to `Float16Array`.
  `$262.detachArrayBuffer` was supplied through `transfer`. The two kinds agree on every file
  (68 pass for both once detaching works, 73 fail for both). The 7 detached-buffer
  `getFloat16`/`setFloat16` files pass the same way.

## Not exercised, or blocked

- Out of scope: BigInt typed arrays, resizable buffers (the `resizable-buffer` accessor tests and
  the harness's resizable factories), `SharedArrayBuffer`, `immutable-arraybuffer` (skipped as
  proposed), cross-realm tests (`$262.createRealm` is refused by this host).
- `$262.detachArrayBuffer` is refused by this host, so the detachment tests fail in the harness
  run. Detachment is covered by the probe (`transfer`) and by the manual re-runs above.
- Pre-existing failures that also affect `Float16Array`, left alone: the engine's array iterator
  ignores a replaced `%ArrayIteratorPrototype%.next`
  (`ctors/object-arg/iterated-array-with-modified-array-iterator.js`); several
  `%TypedArray%.from` deviations (`TypedArray/from/*`); canonical-numeric-string keys on typed
  arrays (`internals/*`); `Array.prototype.concat` over typed arrays.
  `staging/sm/Math/f16round.js` fails before reaching `Math.f16round`: a hoisted nested function
  declaration cannot see a `const` or `let` of its enclosing function (`ReferenceError`), an
  engine defect unrelated to this slice.
- The native execution form was not exercised (it cannot instantiate on this host,
  `ProfileFault/UnsatisfiedHostAssumption`). The changed code is realm built-in code.
- Linux and big-endian hosts were not run. The byte order is explicit (`BinaryPrimitives`), and
  `System.Half` conversion is specified by the runtime, not the host.
