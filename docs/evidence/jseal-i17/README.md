# Detached clone adoption on a second realm and thread: JSeal slice I17

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. It adds public host
API; it adds no guest-visible global and no `WorkerRealms` capability (that is JSeal I18).

## What changed

- `JsHostRealm` (public, JSD-0024 section 17): `DetachClone(JsHostValue value,
  ReadOnlySpan<JsHostValue> transfer = default)` answers a `JsHostCloneCarrier`, and
  `AdoptClone(object carrier)` answers a `JsHostValue` built in the receiving realm. Both open with
  the JSD-0024 gate (inside a step, on the realm's own thread, one host call charged, aborts
  latched), so rule N21 covers them.
- `JsClone.cs`: the new public, opaque `JsHostCloneCarrier` (`Profile`, `FormatVersion`, static
  `CurrentFormatVersion`, `IsSingleUse`, `IsConsumed`, `ChargedBytes`; nothing of the graph), and on
  the internal carrier `Charged`, `Claimed`, `EntryBytes = 32` and `ChargeFor`.
- `JsHostValue.cs`: `JsHostRefusal.ForeignCarrier = 4` and `JsHostRefusal.CarrierConsumed = 5`.
- `JsRealm.Clone.cs`: **the sender pays `LiveBytes` for the carrier.** After the transfer list and
  the graph are checked and before the commit loop, the serializer reports `32 x entries + copied
  buffer and string bytes` (moved bytes excluded: the sender already retained them) with
  `RetainOrAbort`, so a refused ceiling ends the operation before any buffer is detached. This
  applies to the internal members too, so JSD-0032's "carrier memory is not reported" limitation
  narrows to carriers held after their sender is disposed.
- Rules, as decided: a carrier holds data only and may cross threads through any synchronising
  handoff; it stays valid after its source is disposed; one without transferred bytes is adoptable
  any number of times, from any realm and thread; one with transferred bytes is claimed atomically by
  its first adoption and every later or racing one is `CarrierConsumed`; anything that is not this
  profile build's carrier (another engine's payload, a second copy of the assembly, any object) is
  `ForeignCarrier`; guest-value refusals stay guest `TypeError`s beginning `DataCloneError:`.
- Decisions: JSD-0024 gains section 17; JSD-0032 is amended (header, clauses 2, 3 and 5, a
  cost-model row and paragraph, new section 5b, follow-ups, falsifiers); the decisions index row now
  reads I14-I17. No new JSD number, opcode or diagnostic code was taken, and no product file was
  added, so the `ReviewRecordRuleTests` file count is unchanged.
- API baseline: `docs/api/public-api.txt` regenerated with `BROILER_API_WRITE=1`; it gains the
  eleven lines of the type, its six properties, the two realm members and the two refusal values.
- `HostSurfaceChecks.Clone.cs` (new, CLI composition) adds four `--host-surface` rows;
  `HostSurfaceChecks.cs` runs them.

## The four host-surface rows

Every runtime lives on a host thread of its own from creation to disposal and is reached through
the lane's turn model (a program, a `#host-turn` invocation, another program). The carrier is the
only thing handed between host threads, through `Thread.Join`.

1. A graph with every supported brand, a cycle, a shared object and a hole is detached on thread A;
   A's source is mutated after the detach, then its instance, artifact and runtime are disposed and
   a collection forced. Thread B adopts it; B's guest checks every prototype against B's own
   intrinsics, the cycle, the shared references, the view/buffer sharing and the original values.
   Thread C adopts it twice; the two copies share no object and no buffer. The turn threads are
   distinct.
2. A carrier with a transferred buffer: the source's buffer and view read empty in the next
   program; two receivers on two threads, released together by a barrier, race for it and exactly
   one adopts it (bytes and view intact); a later adoption on a third thread is `CarrierConsumed`.
3. `new object()` and a string are `ForeignCarrier`, `null` is `ArgumentNullException`; between
   invocations `AdoptClone` and `DetachClone` are `RealmNotCurrent`, and so is `AdoptClone` from a
   second thread while the realm's own thread holds the step; none of these claims the single-use
   carrier, which the turn then adopts.
4. Under an aggregate parent, the sender's live bytes move by exactly `ChargedBytes` for a copying
   carrier (which covers its 4096 buffer bytes and 1000-character string) and for a transfer carrier
   (which is under 4096: moved bytes are not charged twice); the receiver's move by exactly the 8192
   bytes of the two buffers it rebuilt. With a four-mebibyte ceiling and a three-mebibyte buffer
   already held, detaching a carrier that copies it ends the turn as `ResourceExhaustion`, the
   host's `catch` sees `JsHostTerminatedException`, and the spent instance runs nothing afterwards.

## Executed on Windows 11, .NET SDK 10.0.401, Release

- Base (HEAD plus `base-vm-w4.patch`): `dotnet build Broiler.VM.slnx -c Release`, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  Slice-compiler `--checks`: its summary line counts 327 checks and 2 not run. CLI acceptance:
  219/219. The `--host-surface` lane: every check passes (43 rows).
- After: build, `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build` (write mode), build,
  `BROILER_API_WRITE=1` over the architecture tests, then gate mode: Contract 267/267,
  Architecture 256/256, build with 0 warnings. Before the entry charge was moved to the top of
  `AdoptClone`, rule N21 rejected it (it opened with the null check); that was fixed, and the
  write and gate cycles repeated. Slice-compiler `--checks`: the same summary, 327 checks and 2
  not run (the existing x86-64-sysv rows), so the 26 clone rows are unaffected by the new charge.
  CLI acceptance: 219/219. The `--host-surface` lane: every check passes (47 rows; see
  [host-surface.txt](host-surface.txt)).
- Failing first: the four rows were written after the public members existed (they cannot compile
  against the base), so no pre-implementation run exists. [mutations.txt](mutations.txt) shows rows
  2-4 failing under a mutation of the property each guards (row 1 was not mutated; its prototype and
  identity assertions are the I14-I15 properties the slice-compiler clone rows already falsify):
  no sender charge (row 4), no compatibility check (row 3), no step gate on adoption (row 3), and
  no single-use claim, pre-check or record guard (row 2). Each mutation was reverted and the lane
  passed again. The step-gate mutation was first made while the null check preceded the gate, and
  was repeated after the gate moved to the top of the member, with the same failure.
- Test262 (`test/built-ins/ArrayBuffer`, pinned suite), before with the product changes
  reverse-applied and rebuilt, and after: the same totals and the same 24 failing variants
  ([test262-comparison.txt](test262-comparison.txt)). A no-regression check only: no guest-visible
  behaviour changed, and the pinned suite has no structured-clone or cross-realm transfer test.
- Differential lane (`python eng/run-differential.py --timeout 10`): the only difference from the
  retained answers is the known code-page-850 case in `the-later-library-methods`. No probe was
  added, because nothing here is reachable from a script.

## Not exercised, or out of scope

- That a refused carrier leaves its transfer list attached is not observed: the ceiling refusal
  ends the operation and the instance, so no later crossing can read the buffer. It rests on the
  order in the serializer (the charge precedes the commit loop), as the binary rows' resize refusal
  does; the refusal and guest-throw paths of I16 remain the observed atomicity evidence.
- A carrier from a genuinely different profile build (a second copy of the assembly in another load
  context) was not constructed; the lane uses a composition-defined object and a string. In this
  build the profile and format-version comparisons cannot fail; they exist for a later layout.
- The race in row 2 releases both receivers together but cannot force their claims to overlap; the
  row asserts the outcome (exactly one winner), which holds for every interleaving.
- Native execution form and the Native AOT publish of the CLI and slice compiler: not exercised.
- `WorkerRealms`, a profile-created second realm or thread, `postMessage` and an event loop: card
  I17 exclusions (JSeal I18 owns the capability). JSeal's provider is not changed here.
- **Remaining limitation:** the sender's `LiveBytes` charge is released when its instance is
  disposed, so a carrier the host keeps after that is host memory bounded per carrier (JSD-0032's
  two maxima) and by no budget. The charge is also never credited earlier (no retention in this
  profile is), so a long-lived sender that sends many messages accumulates it.
