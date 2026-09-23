# JSW-2 resizable ArrayBuffer: JSeal slices F04, F05 and F06

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSW-2 (binary surface); it does not mark any stage complete.

## What changed

- **F04, the buffer model** (`JsArrayBuffer`, `JsBinary.cs`). A buffer is fixed-length or
  resizable: `MaxByteLength` is `null` or the specification's `[[ArrayBufferMaxByteLength]]`, and
  it survives a detach. A resize REPLACES the storage array (`TryReplaceStorage`): the common prefix
  is copied, the rest of a larger array is already zero, and `Data.Length` is still the byte length,
  so every reader of `Data` keeps the invariant it had and a fixed-length buffer pays nothing. The
  realm operation (`BinaryResizeBuffer`, `JsRealm.Binary.cs`) charges fuel for the new block, asks
  the live-bytes ceiling (`RetainOrAbort`) to admit any growth past the buffer's high-water mark
  (`RetainedByteLength`), then allocates it (an `OutOfMemoryException` becomes a `RangeError`), and
  only then commits. (Review correction: the first version allocated before asking the ceiling,
  against `RetainOrAbort`'s "abort before the allocation" contract.) A refused charge, a refused retention, an allocation failure, a limit past
  the maximum, a fixed-length receiver and a detach during the length coercion all leave the old
  bytes and length in place. A shrink retains and releases nothing (no allocation in the realm is
  released to `LiveBytes`); regrowth up to the mark is not charged twice, so a `resize(max)` /
  `resize(0)` loop costs `max` retained bytes, not an unbounded sum. No storage is reserved up to
  the maximum; a maximum past `Array.MaxLength` is a `RangeError` at construction, which the
  specification allows.
- **Retained storage audit.** Nothing outside a single native step holds a buffer's array:
  - views (`JsTypedArray`, `JsDataView`) read `Buffer.Data` on every access and never cache it;
  - `%TypedArray%` built-ins that copy bytes (`slice` same-kind copy, `copyWithin`, `set`) read
    `Data` after the last step that can run guest code;
  - `ArrayBuffer.prototype.slice` and `transfer`/`transferToFixedLength` read the source after the
    species constructor and coercions; `slice` now copies nothing, without error, when the receiver
    shrank past the start;
  - host surface (JSD-0024 section 12): `TryReadArrayBuffer` (both forms) measures and copies inside
    one crossing in which no guest code runs; `NewArrayBuffer` builds a fixed-length buffer;
    `DetachArrayBuffer` detaches a resizable buffer like any other. A resizable buffer answers
    `Copied` with its current bytes, or `DestinationTooSmall` with its current length.
    `JsHostBufferStatus` is **not** extended; the enum remark and JSD-0024's compatibility paragraph
    are amended to say so and why (a shared buffer is still the case that would need its own member);
  - clone serializer (JSD-0032): a resizable buffer, and a typed array or DataView over one, are
    refused (`UnsupportedBrand`) before a view's offset or length is read; the matrix row is updated.
- **F05, views.** `JsTypedArray` and `JsDataView` take `int?` lengths; `null` is `auto` (length
  tracking), given only when the length is omitted over a resizable buffer. `IsOutOfBounds` is
  `IsTypedArrayOutOfBounds` / `IsViewOutOfBounds` against the buffer as it is now, and `Length` /
  `ByteLength` are computed on every read (zero while out of bounds). Indexing, `in`, `delete`,
  own keys, `defineProperty` and the engine's integer-indexed `[[Get]]`/`[[Set]]`/`[[Has]]` paths
  all go through `TryReadAt`/`TryWriteAt`/`TryGetOwnProperty`, so they follow the current state and
  a fixed-length view recovers over the buffer's current bytes when it grows back. The typed-array
  and DataView constructors follow `InitializeTypedArrayFromArrayBuffer` and the `DataView`
  constructor, including the second range check after `new.target.prototype` is read. The DataView
  constructor measures the buffer once, before converting `byteLength`, and checks the given length
  against that measurement (steps 5 and 9.b). Review correction: the first version measured after
  the conversion, so a `valueOf` that grew the buffer admitted a view the specification refuses, and
  one that detached it met a `RangeError` instead of step 11's `TypeError` (the detach half was
  already wrong in the base). Typed-array getters answer zero when out of bounds; DataView getters
  and accessors throw `TypeError`.
- **F05, the typed array's `[[PreventExtensions]]`** (review addition; `JsTypedArray.IsFixedLength`
  and `PreventExtensions`). `IsTypedArrayFixedLength` is false for EVERY typed array over a
  resizable buffer - length-tracking or not, detached or not - and for such a view
  `[[PreventExtensions]]` answers false: `Object.preventExtensions`, `Object.seal` and
  `Object.freeze` throw `TypeError` (`SetIntegrityLevel` step 3, before any property is touched),
  `Reflect.preventExtensions` answers false, and so does a `Proxy` with no `preventExtensions` trap
  over one. Without it a "frozen" view gained writable indices when its buffer grew, which breaks
  the essential invariant for non-extensible objects. The hook is the smallest one the object model
  allows: one branch at each of the four places that apply `[[PreventExtensions]]`
  (`Object.preventExtensions` and `ObjectSetIntegrity` in `JsRealm.Object.cs`,
  `Reflect.preventExtensions` in `JsRealm.Reflect.cs`, the no-trap path of
  `JsProxy.ProxyPreventExtensions`). A DataView has no exotic `[[PreventExtensions]]` and is
  unchanged.
- **F06, method audit by family** (`JsRealm.Binary.cs` unless named):
  - validation: `ValidateTypedArray` (`BinaryLiveTypedArray`) refuses out of bounds as well as
    detached; species results (`TypedArrayCreateFromConstructor`) likewise;
  - callbacks (`forEach`, `every`, `some`, `find`, `findIndex`, `reduce`, `reduceRight`; `map`,
    `filter`, `findLast`, `findLastIndex` already did): the length is measured once before the first
    callback, so a shrink yields `undefined` for later indices and a growth adds no visits;
  - coercions: `indexOf`/`lastIndexOf` skip indices that stopped being valid (HasProperty),
    `includes`, `join`, `at` read against the length measured before the coercion, and the
    `len = 0` early return now precedes the `fromIndex` coercion; `fill` and `copyWithin`
    revalidate, throw when out of bounds and clamp to the new length ("longest still-applicable
    prefix"); `with` checks `IsValidIntegerIndex` against the current buffer and copies an element it
    can no longer read as `undefined`; `set` converts the offset before validating the target and
    measures the target before reading the source's `length`;
  - species construction: `slice` revalidates and copies only the prefix the receiver still has;
    `subarray` of a length-tracking receiver with no end creates a length-tracking result;
  - iteration: `%ArrayIteratorPrototype%.next` over a typed array (`JsRealm.Symbol.cs`) measures
    through the buffer and throws `TypeError` at a step where the view is out of bounds;
  - buffer methods: `transfer` keeps resizability, `transferToFixedLength` drops it (one
    `ArrayBufferCopyAndDetach`, length converted before the detached check, allocation before
    detach); `slice` shrink case as above.
- **Published:** `new ArrayBuffer(len, { maxByteLength })` (the options bag was accepted and
  ignored before), `resize`, `resizable`, `maxByteLength`, `detached` and `transferToFixedLength`.
  No global was added, so `docs/realm/globals.txt` and rule N17 are unchanged. No public API,
  opcode, diagnostic code, product file or decision number was added; JSD-0024 and JSD-0032 were
  amended in place.

## Executed on Windows

- Regression first: [probe-before.txt](probe-before.txt) is the new probe on a build of the base
  tree (HEAD plus the wave-1 and wave-2 patch, built from a `git archive` of the base tree): 57 of
  its first 79 cases differ from Node 24.17 (options ignored, no `resize`, every view fixed).
- Probe `src/tests/differential/the-resizable-array-buffer.js` (79 cases in the first round, 90 after the review round), run with
  `python eng/run-differential.py --only the-resizable-array-buffer --against node --timeout 20`:
  agrees with Node on every case after the change; each retained answer was also checked against
  the ES2026 steps named in the probe header.
- `python eng/run-differential.py --timeout 60` (retained answers, whole lane): the only difference
  is the known code-page case `the-later-library-methods` 36 (cube root), unchanged by this slice.
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267 and Architecture 256/256
  before, and after (following the assurance write run and a rebuild).
- `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passes, including the new
  "a resizable buffer is read at its current length after it grows, shrinks and moves".
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: every check passes
  (2 are reported as not run on this machine), including the new
  `clone/f06/a-resizable-buffer-and-a-view-over-one-are-refused`.
- Pinned Test262, bytecode form, `--shards 1 --jobs 1`. The harness lists `resizable-arraybuffer`
  and `arraybuffer-transfer` as standard features, so their tests ran rather than being skipped.
  Per-directory tables: [test262-comparison.txt](test262-comparison.txt). No variant went from
  passing to failing in any run:
  - `ArrayBuffer`, `DataView`, `TypedArray`, `TypedArrayConstructors`: pass 3267 -> 3663 of 5819
    variants. Of the 715 variants claiming `resizable-arraybuffer` or `arraybuffer-transfer`, the
    pass count moves from 48 to 430.
  - `Array/prototype`, `ArrayIteratorPrototype`, `Object`, `Function`, `language/statements/for-of`
    and `for-in`, `language/destructuring/binding` and `staging/built-ins` (every other directory
    holding a `resizable-arraybuffer` test, apart from `SharedArrayBuffer` and `Atomics`): pass
    14516 -> 14542 of 15006 variants (14544 after the review round, below); `ArrayIteratorPrototype/next` 38 -> 40 (`detach-typedarray-in-progress.js`:
    the iterator now throws for a detached view); the feature variants' pass count moves from 10 to 34.
- The remaining run-1 feature failures are not reachable in this realm: 150 variants outside `BigInt/`
  directories fail only because `resizableArrayBufferUtils.js`'s `MayNeedBigInt`, or the test
  itself, names `BigInt64Array`, which this realm does not have (a `ReferenceError`); 106 need BigInt
  proper; 14 need `SharedArrayBuffer`; 5 claim the proposed `immutable-arraybuffer`; 8 are the
  pre-existing refusal of a subclass receiver by `%TypedArray%.from`/`of`. In run 2 the same
  `BigInt64Array`/`BigInt` harness reference blocks the feature files outside the four integrity
  files (the freeze file passes after the review round; the three staging files also need
  `SharedArrayBuffer`). As a diagnostic only, the 149 files of both runs that fail only on that
  reference ([shimmed-harness-files.txt](shimmed-harness-files.txt)) were run through the CLI with
  [run-shimmed-harness.py.txt](run-shimmed-harness.py.txt): the pinned harness, with
  `resizableArrayBufferUtils.js` patched not to add or test the BigInt constructors, and inert
  `BigInt64Array`/`BigUint64Array` functions defined so `instanceof` answers false. Before the
  change 1/76 (run 1) and 2/73 (run 2) of them succeed; after it 149/149 succeed, and the run-1 and
  run-2 halves were repeated in strict mode with the same result. These figures are not Test262
  results: the harness was modified.

### Review round (2026-09-21)

- Regressions first: cases 80-90 were added to the probe (DataView constructor with a growing,
  detaching and shrinking `byteLength` coercion; `[[PreventExtensions]]` through
  `Reflect.preventExtensions`, `Object.preventExtensions`, `Object.freeze`, `Object.seal`, a
  detached buffer, a trapless `Proxy`, a DataView and a fixed buffer).
  [probe-before-review.txt](probe-before-review.txt) is the pre-review build: 8 of them differed
  from Node. After the fixes all 90 cases agree with Node, and each new answer was checked against
  the ES2026 steps (DataView steps 5, 9.b and 11; typed array `[[PreventExtensions]]`,
  `IsTypedArrayFixedLength`, `SetIntegrityLevel`).
- New host-surface check "a resize is charged past its high-water mark, and one the live-bytes
  ceiling refuses ends the run": under a 4 MiB `LiveBytes` ceiling, eight `resize(3 MiB)` /
  `resize(0)` rounds are admitted (they would exceed the ceiling if each were charged), and a later
  growth past the ceiling ends the run with `ResourceExhaustion` before anything after it prints.
  `--host-surface`: every check passed.
- `dotnet test Broiler.VM.slnx -c Release --no-build` after the assurance write run and a rebuild:
  Contract 267/267, Architecture 256/256. `SliceCompiler --checks --verbose`: every check passes, 2
  not run on this machine. `run-differential.py --timeout 60`: only the known code-page case.
- Pinned Test262 re-run (`after-review`): run 1 is per variant identical to the first after run
  (pass 3663 of 5819). Run 2 was repeated with `test/built-ins/Reflect` and `test/built-ins/Proxy`
  added: over the original 15006 variants the pass count moves from 14516 (base) to 14544, with no
  variant going from passing to failing; the two new ones are
  `Object/freeze/typedarray-backed-by-resizable-buffer.js` (sloppy and strict). `Reflect` and
  `Proxy` (913 variants, base build run separately with `--binary-directory`): 840 before and 840
  after, per variant identical.
- The three staging `*-variable-length-typed-arrays.js` files still cannot pass: they construct a
  growable `SharedArrayBuffer`, which this realm does not have. As a diagnostic only, copies of them
  with the top-level blocks that construct a `SharedArrayBuffer` removed, plus the freeze file,
  were run through [run-shimmed-harness.py.txt](run-shimmed-harness.py.txt) (`TESTS_FROM` names the
  directory of trimmed copies): 4/4 in sloppy mode and 4/4 in strict mode. These are not Test262
  results.

## Not exercised, or blocked

- BigInt typed arrays and every resizable test that names them (B-series); growable
  `SharedArrayBuffer` (absent, JSD-0028); `immutable-arraybuffer` (proposed, skipped).
- `%TypedArray%.from`/`of` with a subclass receiver: a pre-existing limitation, not changed here.
- The three staging `*-variable-length-typed-arrays.js` files, which need a growable
  `SharedArrayBuffer` (see above).
- Actual allocation failure (`OutOfMemoryException` during `resize`) was not provoked; the path is
  a `RangeError` before the buffer changes, by construction. The live-bytes refusal on growth is
  exercised (the check above), but that the refused buffer keeps its old bytes and length cannot be
  observed from outside: the refusal ends the operation and every crossing through which a host
  could read the buffer. It holds by construction (the ceiling is asked before anything changes).
- The native execution form was not exercised (it cannot instantiate on this host,
  `ProfileFault/UnsatisfiedHostAssumption`); the changed code is realm built-in code.
- Linux was not run.
