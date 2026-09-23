# Explicit resource management runtime: JSeal slices F18, F19, F20

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the runtime half of explicit resource management only. No `using` or `await using`
syntax is parsed or lowered (F21/F22), and nothing here claims it is.

## What changed

- **F18.** `Symbol.dispose` and `Symbol.asyncDispose` are well-known Symbols on `Symbol`
  (non-writable, non-enumerable, non-configurable), minted per realm like every other well-known
  Symbol here (the per-realm/per-agent note in `JsRealm.Symbol.cs` and decision JSD-0030 apply
  unchanged). `SuppressedError(error, suppressed, message)` is a NativeError-shaped constructor
  (`[[Prototype]]` is `Error`, its prototype inherits `Error.prototype`, `name`/`message` on the
  prototype, length 3, callable without `new`, registered in `ErrorConstructors`). The message is
  converted first, then `error` and `suppressed` are defined as non-enumerable own properties holding
  the arguments unchanged; there is no options bag and no `cause`.
- **F19.** `DisposableStack` (requires `new`): `use`, `adopt`, `defer`, `move`, `dispose`, the
  `disposed` getter, `[Symbol.dispose] === dispose`, `[Symbol.toStringTag]`. The dispose method is
  captured at registration; entries unwind LIFO; a second throw is combined as
  `SuppressedError { error: new, suppressed: previous }`; a disposed stack answers `ReferenceError`
  to `use`/`adopt`/`defer`/`move` and `dispose()` twice is a no-op; `move()` builds an intrinsic
  `DisposableStack` holding the entries and marks the source disposed. The stack is marked disposed
  and its entries detached before the first entry runs, so a reentrant `dispose()` or `move()` from a
  disposer cannot reach an entry twice.
- **F20.** `AsyncDisposableStack` with `disposeAsync` (always answers a promise; a bad receiver
  rejects), `[Symbol.asyncDispose] === disposeAsync`, the same registration methods, and the
  `Symbol.dispose` fallback wrapped so a synchronous disposer's result is not awaited and its throw
  becomes a rejection. Unwinding is a continuation driven by the realm's `AwaitOn`, so every await is
  an ordinary promise reaction on the job queue; `needsAwait`/`hasAwaited` follow the proposal's
  `DisposeResources` (a nullish `use` owes one `Await(undefined)`).
- `%IteratorPrototype%[Symbol.dispose]` and `%AsyncIteratorPrototype%[Symbol.asyncDispose]` are
  installed on the existing intrinsic objects (see "Not exercised" for what that does not reach).
- Unwinding charges fuel per entry and registration retains live bytes per entry. Only a guest
  `throw` (`JsThrow`) is folded; `JsAbort` (allowance, cancellation) is never caught, so a spent
  allowance stops the unwind with the remaining entries not run. Checked by hand: 200,000 deferred
  disposers under `--fuel 3000000` end with `AllowanceExhausted on Fuel` and a guest `finally` after
  `dispose()` does not run; 100,000 async deferred disposers settle normally with the default
  allowance and exhaust under `--fuel 2000000`.
- The realm's published global set (`docs/realm/globals.txt`, rule N17) was regenerated and now
  lists `AsyncDisposableStack`, `DisposableStack` and `SuppressedError`; none of them is on the
  ledger's absent list, so rule N17 needed no other change. Rule H3's product-file count moves from
  184 to 185 for the new `JsRealm.Disposal.cs`. No public API changed.

## The oracle

The pinned ES2026 edition contains none of this (decision JSD-0019 records that, and it remains
true). The oracle used is the pinned Test262 (`ccaac100`) subtree under the
`explicit-resource-management` feature. The harness lists that feature under "Proposed language
features", so `eng/run-test262.py` **skips** all 240 of these files (both before and after). They
were therefore run explicitly with [run-explicit-test262.py](run-explicit-test262.py): `assert.js`,
`sta.js`, `doneprintHandle.js` for async tests and the declared includes are concatenated with the
test and run by the end-user CLI (`--quiet`), non-strict and strict variants as the harness would,
`Test262:AsyncTestComplete` required for async tests. The runner appends `;void 0;` because the CLI
throws while rendering a Symbol script completion (pre-existing, unrelated to this slice). Sanity
check of the runner on `test/built-ins/AggregateError`: the pass count is 48 of 50 variants, the two
failures being `proto-from-ctor-realm.js` (`$262.createRealm`).

## Executed on Windows (win-x64, .NET SDK 10.0.401, Release, bytecode form)

- Base: `dotnet build Broiler.VM.slnx -c Release`, then `dotnet test Broiler.VM.slnx -c Release
  --no-build`: Contract 267/267, Architecture 256/256.
- After (build, `BROILER_ASSURANCE_WRITE=1` test run, build, gate-mode test run): Contract 267/267,
  Architecture 256/256.
- Explicit Test262 run, 240 files / 480 variants, the pass count moves from 0 to 458
  ([explicit-before.txt](explicit-before.txt), [explicit-after.txt](explicit-after.txt)):

  | Directory | Variants | Before | After |
  |---|---|---|---|
  | SuppressedError | 44 | 0 | 42 |
  | DisposableStack | 186 | 0 | 184 |
  | AsyncDisposableStack | 208 | 0 | 206 |
  | Symbol/dispose | 6 | 0 | 4 |
  | Symbol/asyncDispose | 6 | 0 | 4 |
  | Iterator/prototype/Symbol.dispose | 12 | 0 | 0 |
  | AsyncIteratorPrototype/Symbol.asyncDispose | 18 | 0 | 18 |

  The 22 remaining failures: 10 variants are `proto-from-ctor-realm.js`/`cross-realm.js`, which
  need `$262.createRealm` (this profile creates no nested realm, JSD-0030); 12 are
  `Iterator/prototype/Symbol.dispose`, see below.
- Harness run over `SuppressedError`, `DisposableStack`, `AsyncDisposableStack`, `Symbol`,
  `Iterator/prototype/Symbol.dispose`, `AsyncIteratorPrototype`, `NativeErrors`, `AggregateError`
  and `Error` (513 files, 782 variants), base build versus this patch: identical per-variant
  verdicts (pass 450, fail 88, unsupported 4, skipped 240 on both sides; the 240 skipped are the
  proposed-feature files above).
- Neighbour subsets `Promise`, `GeneratorPrototype`, `AsyncGeneratorPrototype`, `global`,
  `Object/prototype/toString`, `Array/prototype/Symbol.iterator` (857 files, 1669 variants): identical
  per-variant verdicts before and after.
- New differential probe `src/tests/differential/the-disposal-stacks.js` (72 cases). On the base
  build all 72 cases differ from Node v24.17.0 ([probe-before.txt](probe-before.txt)). After:
  `python eng/run-differential.py --only the-disposal-stacks --against node --timeout 20` exits 0
  with one declared divergence, case 41: the SuppressedError that disposal builds has no own
  `message` per the proposal's `DisposeResources`, while V8 gives it one.
- Full retained differential lane: only the pre-existing code page 850 failures
  (`the-json-date-and-regexp-surface`, `the-later-library-methods`) fail, and they fail identically
  on the base build.

## Not exercised or still failing

- `Iterator/prototype/Symbol.dispose` (12 variants) reaches `%IteratorPrototype%` as
  `getPrototypeOf(getPrototypeOf([][Symbol.iterator]()))`. In this realm an Array iterator inherits
  `%IteratorPrototype%` directly (there is no `%ArrayIteratorPrototype%`), so that path lands on
  `Object.prototype`. The disposer itself is installed and works when `%IteratorPrototype%` is reached
  through a generator (probe cases 51-53). Adding `%ArrayIteratorPrototype%` belongs to the Iterator
  work in flight elsewhere and was not done here.
- Cross-realm Symbol identity and `proto-from-ctor-realm` (10 variants): no nested realm exists.
- `using` / `await using` declarations (F21/F22) are not implemented; the Test262 language tests for
  them were not run.
- The native execution form was not exercised (pre-existing `ProfileFault/UnsatisfiedHostAssumption`
  on this host). The new built-ins have no separate native implementation.
- Linux was not run.
