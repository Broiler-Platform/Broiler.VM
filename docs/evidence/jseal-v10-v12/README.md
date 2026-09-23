# JSP-6 binary species: JSeal slices V10, V11 and V12

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSP-6 for the binary surface and does not mark that stage complete.

## What changed

`%TypedArray%.prototype.map`, `filter`, `slice` and `subarray` and `ArrayBuffer.prototype.slice`
now construct their results through the receiver's species (`JsRealm.Binary.cs`):

- `SpeciesConstructor` reads `constructor`, then `Symbol.species`, with the specified defaults and
  `TypeError`s; the default is the intrinsic constructor of the receiver's own kind.
- `TypedArraySpeciesCreate` / `TypedArrayCreateFromConstructor`: the result must be a typed array
  (a proxy around one is refused), not detached, and at least as long as a single requested
  length; its content type is compared explicitly (`JsElements.HoldsBigInts`, false for all nine
  kinds today) rather than assumed from the shared `double` representation.
- `map` constructs before its first callback; `filter` runs every callback before reading the
  species. Values are converted by the result's own kind (clamping, fractions, wrap).
- `slice` re-checks the receiver's detachment after construction when the count is non-zero,
  copies bytes forward only when the element *kind* is the same, and converts element by element
  otherwise. `subarray` hands the species `(buffer, beginByteOffset, newLength)` over the same
  buffer and is not held to the length check.
- `ArrayBuffer.prototype.slice` validates the species result (ArrayBuffer, not detached, not the
  receiver, large enough), re-checks the receiver's detachment, and only then copies.

Fuel, allocation charging and guest exception propagation use the existing construction path
(`JsEngine.Construct`, `BinaryNewBuffer`); per-element charges are kept on every loop.

## Executed on Windows

- Differential probe `src/tests/differential/the-binary-species.js` (70 cases). Before the change,
  55 of 70 answers differed from Node 24.17; after it, none. Every retained answer was checked
  against the specification steps named above, not only against Node.
  [binary-species-node.json](binary-species-node.json) is the passing comparison report.
- The full retained lane (`python eng/run-differential.py --timeout 10`) fails only in the two
  pre-existing places: case 36 of `the-later-library-methods` and cases 158-159 of
  `the-json-date-and-regexp-surface` (lone-surrogate/emoji output spelling), which fail the same
  way with the binary built from the unmodified main tree.
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256,
  after the assurance write run.
- Pinned Test262, bytecode form; [test262-comparison.txt](test262-comparison.txt) lists the
  newly passing variants and the failure causes:
  - focused (`TypedArray/prototype/{map,filter,slice,subarray}`, `ArrayBuffer/prototype/slice`):
    pass 174 -> 282, fail 390 -> 282, unsupported 152, no newly failing variant. Every
    `ArrayBuffer/prototype/slice/species*` variant passes.
  - spot-check adding `TypedArrayConstructors` and all of `ArrayBuffer`: pass 844 -> 952,
    fail 1261 -> 1153, no newly failing variant.

## Not exercised, or blocked

- Out of scope and reported separately: BigInt typed arrays (126 failing and 152 unsupported
  variants in the focused run), resizable buffers (48), SharedArrayBuffer (2).
- The 106 remaining in-scope failures of the focused run are all blocked elsewhere: 88 fail first
  on the harness's `makeIterable` factory, because the typed-array constructor does not yet
  drain an iterable argument; 18 fail on `$262.detachArrayBuffer`, which this host refuses.
  Detachment is covered instead by probe cases that use `ArrayBuffer.prototype.transfer`.
- The native form was run on the same subtrees before and after: every variant fails to
  instantiate on this host (`ProfileFault/UnsatisfiedHostAssumption`), so it says nothing about
  this change. The changed code is realm built-in code and has no native-form counterpart.

## JSeal buffer reads (V11 acceptance) need a companion JSeal change

Honouring species in `subarray` is correct and matches Node, but it changes what JSeal's
`VmRealm.TryGetArrayBufferBytes` does. That read built one `Uint8Array` over the buffer and cut
each chunk with the pinned `%TypedArray%.prototype.subarray` before rendering it with `join`. With
this change `subarray` reads the view's `constructor` and then `Uint8Array[Symbol.species]`, both
of which a page may redefine. A page could therefore run code inside a host read step, throw from
it, or answer a different typed array. An answer longer than the chunk overran the host's
`byte[]` with an `IndexOutOfRangeException`.

The companion JSeal change is `D:/wt/patches/vm-v10-v12-jseal.patch` (worktree
`D:/wt/vm-v10-v12-jseal`). It constructs each chunk's view directly as
`new Uint8Array(buffer, offset, length)` with the pinned constructor, which consults no species,
and drops the now unused `subarray` from JSeal's bridge. Its regression test,
`APageThatRedefinesTypedArraySpeciesCannotChangeWhatTheHostReads`, redefines
`Uint8Array[Symbol.species]` (longer answer) and `Uint8Array.prototype.constructor` (throwing
getter), and asserts that the host reads the real bytes and that no page code runs.

To run JSeal against this VM build, the JSeal Release-VM test output was copied and the package
`Broiler.VM*.dll` files in the copy were replaced with this worktree's Release build:

- Test added, JSeal unchanged, this VM: 381/382. The new test fails with
  `System.IndexOutOfRangeException` in `TryGetArrayBufferBytes`, as the review predicted.
- JSeal fix, this VM: 382/382.
- JSeal fix, published `0.1.0-preview.3` packages (`dotnet test -c Release-VM`): 382/382.
  Before any change: 380/380.
- JSeal fix, `-c Release`: 202/202.

This VM patch must not land without the JSeal patch, or JSeal's buffer reads can be steered by
a page, and can fault the host, as soon as JSeal takes a VM package that contains it.
