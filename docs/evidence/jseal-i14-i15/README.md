# Internal structured-clone graph: JSeal slices I14 and I15

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. It gives no
`WorkerRealms` capability and adds no public API or guest-visible global.

## What changed

- Decision record [JSD-0032](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0032-the-internal-structured-clone-carrier.md)
  sets the carrier format and lifetime, the getter and Error rules, the cost model and bounds, and
  the supported-type matrix. It is indexed in the decisions README. Status: proposed, owner decision
  pending.
- `JsClone.cs` (new, internal) holds the carrier types. `JsCloneCarrier` holds records, a root slot,
  a format version and the two bounds (2^22 entries, 2^28 bytes). `JsCloneRefusedException`
  carries a `JsCloneRefusal` reason.
- `JsRealm.Clone.cs` (new, internal) holds the serializer and deserializer. The serializer follows
  HTML `StructuredSerializeInternal` depth-first order using an explicit frame stack instead of CLR
  recursion. It keeps a memory map for cycles and shared references, invokes getters, copies
  buffer bytes, and is charged per record, key, value and byte. The deserializer builds everything
  from the destination realm's intrinsics in two phases and runs no guest code. It charges fuel
  and reports rebuilt buffers to `LiveBytes`.
- `JsHostRealm.cs` gains two **internal** members, `CloneSerialize(value, maxEntries, maxBytes)` and
  `CloneDeserialize(carrier)`. They go through the JSD-0024 gate: they run inside a step, charge a
  host crossing, and latch aborts. A refusal becomes a guest `TypeError` whose message begins
  `DataCloneError:`.
- `CloneChecks.cs` (new) in the slice-compiler composition adds 21 rows to `--checks`. The
  checks bind the two internal members with `UnsafeAccessor`. The first attempt used reflection
  (`MethodBase.Invoke`), which architecture rule B5 rejected. JSD-0032 section 2 records the
  accessor as a door into the profile's internals.
- `ReviewRecordRuleTests.cs`: the covered product-file count moves from 184 to 186, with a
  paragraph naming the two files. `ProportionalityChecks.cs` gets one remark sentence.
  `Program.cs` runs the new checks.

## Changes after adversarial review

- **Detached views (major).** A typed array or DataView whose buffer the memory map had already
  recorded, and which a getter then detached, was cloned over the bytes copied before detachment.
  The only detachment check was in the buffer's own record, which runs once. The typed-array and
  DataView cases now refuse a view over a detached buffer themselves (`RefuseDetachedView`,
  `DetachedBuffer`), as HTML's `IsArrayBufferViewOutOfBounds` step does. The row
  `clone/i15/a-detached-buffer-and-a-view-over-one-are-refused` gained three cases (a
  `Uint8Array`, a `DataView`, and both, each met after its buffer and a detaching getter). Before
  the fix it failed with `refused,refused,refused,cloned,cloned,cloned`; after the fix every case
  is `refused`. JSD-0032's matrix gains the row for views over a detached buffer.
- **Bound wording (minor).** JSD-0032 section 4 and the `JsCloneCarrier` remarks now say what the
  code does. An entry is a record or a value slot, and a key travels with its slot. A string is
  counted once per distinct CLR string instance, which can only make the bound stricter. A
  `TooLarge` refusal comes before the entry or copy that would pass the bound. An object's key
  list and a Map's or Set's snapshot are built in full before their entries are counted.
- **Unreported carrier memory (minor)** is kept as an accepted limitation: see below.

## Executed on Windows 11, .NET SDK 10.0.401, Release

- `dotnet build Broiler.VM.slnx -c Release`, then `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release --no-build`,
  then a rebuild, then `dotnet test Broiler.VM.slnx -c Release --no-build`. Base: Contract 267/267,
  Architecture 256/256. After: Contract 267/267, Architecture 256/256.
- `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`.
  All 21 clone rows answer `ok`, and no row of the run fails. The two `not-run` rows are the existing
  x86-64-sysv rows. [clone-checks.txt](clone-checks.txt) keeps the clone rows and the run's summary
  line. The base run was not kept, so the difference in row count is taken to be the 21 added rows
  rather than a before-and-after comparison.
- The same run from a Native AOT image
  (`dotnet publish ... -r win-x64 -p:PublishAot=true`, which needed the Visual Studio installer
  directory on `PATH` for `vswhere` on this machine) gives the same answers with no IL trim or AOT
  warning. That is the configuration the lane workflow publishes. It was re-published after the
  review fix: the same summary line, all 21 clone rows `ok`, and no IL trim or AOT warning.
- Falsification ([mutations.txt](mutations.txt)), re-run after review against the final
  `UnsafeAccessor` checks (21 rows):
  - Disabling the memory map fails six rows: cycles, Map/Set, Error cause loop, shared buffer,
    graph identity and cross-realm.
  - Removing the walk's and rebuild's fuel charges fails the proportionality row: the candidate
    and the control then cost the same.
  - Removing `RefuseDetachedView` fails the detached row. That is also the state before the
    review fix, so the new cases were seen failing first.
  - Each mutation was reverted, the file restored from a copy, and the build redone.
- The original checks were written together with the implementation, and their first run was
  green. No run of them failed before the implementation, because the members they bind did not
  exist. The mutation runs above are the evidence that they can fail.
- Differential lane (`python eng/run-differential.py --timeout 10`): 17 probe files. Only the
  known code-page-850 output cases fail (`the-json-date-and-regexp-surface`,
  `the-later-library-methods`). No new probe was added, because no guest-visible behaviour changed:
  the clone is not reachable from a script.

## Measured charge (fuel attributed to one clone of an array of n two-property objects, bisected)

n=64: 1227, n=128: 2443, n=256: 4875, n=512: 9739. That is linear and about 19 units per element
for serialization and rebuild together.

## Not exercised, or out of scope

- Test262: the pinned suite has no `structuredClone` test (a search of `test/` and `harness/` finds
  none), and this change has no guest-visible surface to run it against. No Test262 run was made.
  Native form: not exercised (it cannot instantiate on this machine, a pre-existing condition).
- Nothing public was exercised, because nothing public exists: JSeal's `VmRealm.Clone`, `Detach` and
  `Adopt`, the `WorkerRealms` capability and package consumers are cards I17-I18.
- Cross-thread adoption is not exercised. The carrier is immutable after sealing and holds no realm
  reference, but thread safety is I17's contract. Transfer and single-use carriers are I16's.
- The `MaxEntries` and `MaxBytes` bounds were reached only through the narrowed bounds the internal
  serializer accepts. No graph of four million entries was built.
- **Accepted limitation:** the carrier (records and copied bytes, up to 2^28 bytes) is host memory
  that is not reported to either instance's `LiveBytes`. It is bounded per carrier, not by the
  instance's budget, so a host that keeps many carriers holds memory outside every budget. Who pays
  for a carrier is card I17's decision. The walk's scratch (key lists, Map and Set snapshots) is
  likewise built before the entry bound is checked; it is proportional to an object the guest
  already holds.
- BigInt, resizable buffers and SharedArrayBuffer are absent from the realm. JSD-0032 section 5
  records how each enters the matrix.
- The Error brand is recognised by tag on plain objects, as `Object.prototype.toString` already
  recognises it. A dedicated `[[ErrorData]]` representation is left open in JSD-0032 section 8.
