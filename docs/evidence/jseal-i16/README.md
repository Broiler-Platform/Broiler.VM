# ArrayBuffer transfer in the internal clone carrier: JSeal slice I16

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. It adds no public
API, no guest-visible global and no `WorkerRealms` capability.

## What changed

- `JsRealm.Clone.cs`: `CloneSerializeWithTransfer(value, transfer, maxEntries, maxBytes)` is HTML's
  `StructuredSerializeWithTransfer` for fixed-length `ArrayBuffer`s. It works in four steps.
  1. The list is checked before any getter runs: every entry must be an `ArrayBuffer`
     (`NotTransferable`) and none may be listed twice (`DuplicateTransfer`). Each entry gets its own
     record in the memory map.
  2. The graph is walked as before. Every reference to a listed buffer, and every view over one,
     names that one record.
  3. Every listed buffer is checked again. It must still be attached (`DetachedBuffer`), and its
     bytes are counted against `MaxBytes` (`TooLarge`).
  4. Only then are the buffers detached, in a loop that charges nothing and cannot throw. Each
     record takes over its source's array without a copy.

  **This deliberately departs from HTML.** HTML detaches each buffer as soon as it passes the last
  check, so a later failure leaves the earlier buffers detached. Card I16 forbids that.
- Adoption: a carrier holding moved bytes is single-use. `CloneDeserialize` claims it with an
  atomic exchange before it builds or charges anything, and every later adoption is refused with
  `Consumed`. The destination buffer is built from the moved bytes with one copy, which is charged
  per byte and reported to `LiveBytes`. The record then drops its reference to the bytes. A
  transferred record without bytes is refused rather than rebuilt empty.
- The copy exists to keep `JsBinary.cs` untouched while cards F04-F06 reshape `JsArrayBuffer`,
  which has no constructor that adopts an existing array. It makes no observable difference.
- `JsClone.cs`: `JsCloneRecord.Transferred`, `JsCloneCarrier.Moved` and `TryClaimMoved()`, and the
  refusal reasons `NotTransferable = 7`, `DuplicateTransfer = 8` and `Consumed = 9`. All are
  internal.
- `JsHostRealm.cs`: one **internal** member, `CloneSerializeWithTransfer(JsHostValue, JsHostValue[],
  long, long)`, behind the same JSD-0024 gate as `CloneSerialize`. A refusal becomes a guest
  `TypeError` whose message begins `DataCloneError:`.
- `CloneChecks.cs` adds a third `UnsafeAccessor`, gives the `cloneHost` object `transfer`,
  `transferBounded` and `captureTransfer`, and adds 5 rows (26 clone rows in all).
- JSD-0032 is amended: the header, the lifetime clause, the refusal list, the cost-model rows, the
  transfer-related matrix rows, a new section 5a, the evidence count, the follow-ups and the
  falsifiers. The decisions README index row now reads I14-I16. No new JSD number, opcode or
  diagnostic code was taken, and no product file was added, so the `ReviewRecordRuleTests` count is
  unchanged.
- **Resizable buffers.** At this base there are none: `JsArrayBuffer` is sealed, has no resizable
  flag, and `new ArrayBuffer(4, { maxByteLength: 8 })` builds a fixed buffer. No code path can
  refuse one yet. The row `clone/i16/resizable-buffers-are-refused-until-f04-f06-integrate` is a
  tripwire. Today it answers `refused-or-absent`. It fails as soon as a buffer reporting
  `resizable === true` is cloned or transferred instead of being refused and left usable.

## Executed on Windows 11, .NET SDK 10.0.401, Release

- Base (HEAD plus `base-vm-w3.patch`): `dotnet build Broiler.VM.slnx -c Release`, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
- Failing first: the checks were written against an `internal` stub of
  `CloneSerializeWithTransfer` that ignored the list. `SliceCompiler.exe --checks --verbose` then
  failed the four behavioural I16 rows; the resizable tripwire passed. The `before:` lines in
  [clone-checks.txt](clone-checks.txt) record this. The expected answer of the first row was
  corrected once afterwards: `[...].join(',')` renders `undefined` as empty, so the row now reads
  `String(u8[0])`.
- After: build, `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build` (write mode; Architecture
  H3 and H4 failed during that run, as expected), rebuild, then gate mode: Contract 267/267,
  Architecture 256/256, build with 0 warnings. In `--checks --verbose`, all 26 clone rows answer
  `ok` and no row fails. The only `not-run` rows are the two existing x86-64-sysv rows. See the
  `after:` lines in [clone-checks.txt](clone-checks.txt).
- Falsification ([mutations.txt](mutations.txt)); after each mutation the file was restored and
  rebuilt:
  1. Detaching each buffer as it passes validation (HTML's order) fails two rows: the refused-list
     row and the byte-bound row. In the refused-list row the answer is a `ProfileFault`, because
     after the premature detachment the program's own final `tr(a, [a])` throws an uncaught
     refusal. This harness reports any uncaught guest throw that way, which was checked with a
     throwaway row.
  2. Removing both the single-use claim and the transferred-record guard fails the single-use row:
     the second adoption is refused only by chance, as a view outside an empty buffer.
     Removing the claim alone does not fail that row, because the guard still refuses. The
     claim is kept for the atomicity I17 will need across threads.
- Test262 (`test/built-ins/ArrayBuffer`, pinned suite): the before run was taken with the product
  changes reverse-applied and rebuilt. Before and after give the same totals and the same 192
  failing variants ([test262-comparison.txt](test262-comparison.txt)). This is a no-regression
  check only, because no guest-visible behaviour changed. The pinned suite has no
  `structuredClone` or transfer-list test (`arraybuffer-transfer` in `features.txt` is
  `ArrayBuffer.prototype.transfer`, which this slice does not touch).
- Differential lane (`python eng/run-differential.py --timeout 10`, 29 probe files): the only
  difference from the retained answers is the known pre-existing case 36 (cube root) of
  `the-later-library-methods`. No probe was added, because the transfer is not reachable from a
  script.

## Not exercised, or out of scope

- The native execution form: not exercised (it cannot instantiate on this machine, a pre-existing
  condition). The Native AOT publish of the slice compiler was not re-run for this slice.
- An abort (fuel or cancellation) during a transfer was not driven by a check. The claim that it
  detaches nothing rests on the construction: every charge and every throw comes before the
  commit loop, and that loop contains only `Detach()`. The guest-throw and refusal paths are
  exercised.
- Cross-thread adoption and a public carrier remain cards I17 and I18. The claim is atomic, but no
  second thread was exercised.
- `SharedArrayBuffer` (JSD-0028), host-object transfer (card exclusion) and resizable buffers
  (absent, see above).
- The source realm's `LiveBytes` is not credited when a buffer is transferred away. This matches
  `ArrayBuffer.prototype.transfer`.
