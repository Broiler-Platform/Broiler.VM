# JSP-10 host surface: native ArrayBuffer read and construction (JSeal slices I01 and I03)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It adds host-surface members toward JSP-10 and does not mark that stage complete.

## What changed

`JsHostRealm` gains three members and one status enum (`JsHostRealm.cs`, `JsHostValue.cs`):

- `TryReadArrayBuffer(JsHostValue, out byte[])` and
  `TryReadArrayBuffer(JsHostValue, Span<byte>, out int)` copy a buffer's bytes into memory the host
  owns. The brand test is the realm's own `JsArrayBuffer` type; no property, function or global is
  consulted, so no guest code runs. `JsHostBufferStatus` answers `Copied` (also for an empty
  buffer), `NotAnArrayBuffer`, `Detached`, or `DestinationTooSmall` (span form, which then answers
  the required length). Only `Copied` writes anything.
- `NewArrayBuffer(ReadOnlySpan<byte>)` makes a buffer on the intrinsic `ArrayBuffer.prototype` and
  copies the caller's bytes into it before answering.
- Metering: each member enters the crossing charge first (rule N21), then charges one fuel unit per
  byte before copying. The construction admits its live bytes with `TryCharge` through the new
  `JsEngine.RetainOrAbort`, so a `LiveBytes` refusal is decided before the buffer exists. A refused
  charge or an observed cancellation ends the operation as a latched `JsHostTerminatedException`
  with no partial result.

The contract, including compatibility and versioning, is written in section 12 of
[JSD-0024](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0024-the-in-realm-host-surface.md).
The profile API baseline (`src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`) records the
additions; it was regenerated with `BROILER_API_WRITE=1`.

## Executed on Windows

- Host-surface lane, `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`. Eleven checks were
  added (`HostSurfaceChecks.Binary.cs`). They were written against the unchanged profile first: the
  composition did not compile (`CS1061`: `JsHostRealm` has no `TryReadArrayBuffer` /
  `NewArrayBuffer`; `CS0103`: no `JsHostBufferStatus`). After the change every check passes,
  13 before and 24 after: [host-surface-after.txt](host-surface-after.txt).
- The checks were also run against three deliberate faults, each restored afterwards: answering the
  realm's own storage instead of a copy (four checks report `shared`), copying into the span before
  charging (the fuel and cancellation checks report a written destination), and `Retain` instead of
  `RetainOrAbort` (the live-bytes check reports an answered value). Seven checks failed.
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256, after
  the assurance write run, before and after the change.
- Differential lane (`python eng/run-differential.py --timeout 20`): only the known pre-existing
  code-page failures (`the-later-library-methods`, `the-json-date-and-regexp-surface`). No probe was
  added: the probes run guest scripts through the command-line host, which has no host surface, so
  they cannot reach these members.
- Pinned Test262, bytecode form, `test/built-ins/ArrayBuffer`: 404 variants, pass 180, fail 196,
  unsupported 8, skipped 20, identical before and after with the same failing set. These tests are
  guest-level and exercise nothing added here; the run shows the guest `ArrayBuffer` is unchanged.

## Not exercised, or out of scope

- The native execution form was not run; it cannot instantiate on this host
  (`ProfileFault/UnsatisfiedHostAssumption`, pre-existing), and the change has no native counterpart.
- SharedArrayBuffer and resizable buffers do not exist in this profile, so no case covers them.
- Arbitrarily large copies (near `Array.MaxLength`) were not run; the largest case is 4,000,000 bytes.
- Found in passing, not changed: a host body that catches `JsHostTerminatedException` and returns
  normally lets the guest run on until the step ends, where the latched abort is raised. The
  probes re-raise the termination, as an embedder should.
- JSeal adoption (I02, I04) is not part of this change.
