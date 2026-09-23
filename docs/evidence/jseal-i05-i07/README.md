# Host surface: JSeal slices I05 and I07

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers two additions to the in-realm host surface (JSD-0024, amended in its section 13); it does
not mark any JSP, JSW or JS milestone complete, and JSeal has not adopted either API (I06, I08).

## What changed

- **I05, optional exotic deletion.** New public interface `IJsHostExoticDeletion`
  (`TryDeleteNamed(JsHostRealm, string)`), separate from `IJsHostExotic`, so existing handlers are
  untouched. `JsHostObject` asks for it once, at mint time, and overrides `DeleteOwnProperty`: a
  string key that is not a canonical array index is offered to the handler once, before the
  ordinary deletion, which then runs regardless; `delete` answers the ordinary deletion's result.
  Symbols take `DeleteOwnSymbol` and are never offered. The offer is a charged crossing
  (`JsHostRealm.OfferDeletion`, internal, opens with `Enter`); a `JsHostThrowException` becomes the
  guest throw, a `JsHostSurfaceException` a `TypeError`, a termination the latched abort, and in
  each case the ordinary deletion does not run. This follows JSeal's `IJsExoticDelete` contract as
  both JSeal providers implement it today, including that a non-configurable own property is
  offered and then refused. No Proxy is involved.
- **I07, host promise capabilities.** New public members `JsHostRealm.NewPromiseCapability`,
  `ResolvePromise` and `RejectPromise`, the sealed class `JsHostPromiseCapability` (public
  `Promise`) and the enum `JsHostSettlement` (`Accepted`, `AlreadyResolved`). The promise is a
  `JsPromiseObject` on the realm's intrinsic `Promise.prototype` (`JsRealm.NewHostPromise`, charged
  as the constructor charges); settlement reuses `JsRealm.SettleAsyncPromise`, so thenable
  adoption, self-resolution and reaction scheduling are the engine's own. One
  `[[AlreadyResolved]]` flag per capability gives first-settlement-wins. Both settlement members
  open with `Enter`, so they are refused (`RealmNotCurrent`) outside a step, from another thread,
  and after the instance is released; a capability or value from another realm is refused
  (`ForeignRealm`); `JsHostValue.Missing` as the value or reason is refused with an
  `ArgumentException` (it would otherwise settle the promise with the engine's uninitialised-binding
  marker, a TDZ hole in guest code), and every misuse is refused before the already-resolved flag
  is read.
- Public API baseline `src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt` regenerated
  with `BROILER_API_WRITE=1`; the only additions are the members above. Rules N20 and N21 hold
  unchanged (every new public `JsHostRealm` member opens with `Enter`). No core assembly changed.

## Executed on Windows 11, .NET SDK 10.0.401, Release

- Host-surface lane, the lane JSD-0024 section 7 names:
  `src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`.
  Eleven checks were added to `HostSurfaceChecks.cs` (five for I05, six for I07) and run first
  against stubs - the new types present, `JsHostObject` not consulting the hook, the promise
  members throwing: all eleven failed and the thirteen existing checks held
  ([host-surface-before.txt](host-surface-before.txt), summary line
  `host-surface: 11 check(s) failed`). With the implementation every check prints `ok`
  ([host-surface-after.txt](host-surface-after.txt), `host-surface: every check passed`).
  After review, three checks were added (27 in all): a deletion hook refused at the seam
  (invoking a non-callable value) gives the guest a `TypeError` and keeps the property; a deletion
  hook whose guest callback spins ends the run as `ResourceExhaustion`, with the guest's `catch`
  never reached; and `Missing` is refused by both settlement members while the promise stays open
  for a real `undefined`. With the Missing guard disabled the Missing check failed
  (`missing=not refused: Accepted`) and the other 26 held
  ([host-surface-review-before.txt](host-surface-review-before.txt),
  `host-surface: 1 check(s) failed`); the two deletion checks cover behaviour that was already
  correct and so passed on that run too. With the guard every check prints `ok`.
  The checks cover: claimed and declined names; `"7"` not offered, `"007"` and `"4294967295"`
  offered, symbols not offered; `delete`, `Reflect.deleteProperty`, a trapless `Proxy` and
  `JsHostRealm.DeleteProperty` each offering once; a declined name that keeps answering; a
  non-configurable ordinary property (sloppy `false`, strict `TypeError`, property kept); a
  handler raising `Error(TypeError)` and `Throw(value)`, a seam refusal, and a termination; a handler without the hook unchanged;
  reactions only after a drain; first settlement wins for both orders; thenable adoption order
  (`get` synchronous, `call` and the reaction from jobs), self-resolution `TypeError`, a throwing
  `then` getter; a guest replacing `Promise`; a settlement from another CLR thread inside an open
  step; `Missing` refused; and, across invocations and two instances of one artifact, outside-a-step, foreign
  capability, foreign value, settle in a turn, late, and after disposal.
- `dotnet build Broiler.VM.slnx -c Release`, `BROILER_API_WRITE=1` and
  `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release --no-build`, rebuild, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256 -
  the same as the base.
- Pinned Test262 (`ccaac100`), bytecode form, `test/built-ins/Promise`,
  `test/language/expressions/delete`, `test/built-ins/Reflect/deleteProperty`,
  `test/built-ins/Proxy/deleteProperty` (774 files, 1466 variants), before -> after: pass 1397 ->
  1397, fail 24 -> 24, unsupported 8 -> 8, skipped 37 -> 37, with the same 24 failing variants on
  both sides. No Test262 case can reach the host surface, so this run is a no-regression check on
  the promise and delete paths the change sits beside, not evidence for the new members. The
  same four subtrees were run again after the review fix (`after-review`): pass 1397 of 1466
  again, with the same 24 failing variants.
- Retained differential lane (`python eng/run-differential.py --timeout 20`, code page 850): only
  the known pre-existing encoding failures (`the-later-library-methods`,
  `the-json-date-and-regexp-surface`) differ.

## Not exercised

- No differential probe was added: both APIs are reachable only from host code, which a plain
  script run under Node or the end-user host cannot be, so the lane above is where they are judged.
- The `--host-surface` lane is not part of any CI workflow in this repository; it was run by hand.
- A handler that throws a CLR exception other than the three host-surface types is not translated
  (as for every existing host hook); on the stub run such an exception ended the invocation as
  `ProfileFault/ProfileContractViolation`. No check asserts that outcome.
- The native execution form (it cannot instantiate on this machine: `UnsatisfiedHostAssumption`,
  pre-existing). The changes add no native-form code.
- Linux and the JSeal consumer side: JSeal still uses its Proxy deletion route and its captured
  `Promise` constructor until I06 and I08 adopt these members after a package release.
