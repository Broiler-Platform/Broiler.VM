# Array.fromAsync: JSeal slice F16

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSP-7/JSW-6 (modern library surface) and does not mark any stage complete.

## What changed

- New `src/Broiler.VM.Profile.JavaScript/JsRealm.ArrayFromAsync.cs` installs `Array.fromAsync`
  (length 1, writable/configurable/non-enumerable, not a constructor), called from `SetupArray`.
  It follows the pinned ES2026 steps as a built-in async function:
  - the answer is an ordinary `%Promise%`; every abrupt completion, including the synchronous head
    (non-callable mapper, nullish input, non-callable `Symbol.asyncIterator`/`Symbol.iterator`,
    throwing `length`, `ArrayCreate` RangeError, throwing constructor), becomes its rejection;
  - `GetMethod(@@asyncIterator)`, then `GetMethod(@@iterator)` wrapped by the realm's existing
    `CreateAsyncFromSyncIterator`, then array-like; each Symbol is read once and the iterator is
    made from the method read (`GetIteratorFromMethod`), using the realm's intrinsic Symbols;
  - `IsConstructor(this) ? Construct(this)` (iterator path, no argument) or
    `Construct(this, len)` (array-like path), otherwise `ArrayCreate`; elements go through the V07
    `CreateDataPropertyOrThrow` helper and the final `length` through the strict `Set` helper;
  - `next()` results, array-like elements and mapper results are awaited with the realm's own
    `AwaitOn` (PromiseResolve + PerformPromiseThen); async-iterator values are not awaited;
  - `AsyncIteratorClose` (with the `return` result awaited before rejecting with the original
    reason) on mapper throw, mapper rejection and element-definition failure only; a failing
    `next`, non-object step or throwing `done`/`value` rejects without closing, as the spec says.
- Each stretch between two awaits is a method; the continuation runs in its own promise job, so
  there are no blocking CLR waits, no synchronous reaction execution and no deep recursion. Each
  step charges fuel (plus what `AwaitOn`, `EnqueueJob` and the calls already charge and retain).
  `{length: 1e9}` and an infinite async iterator both end with `AllowanceExhausted on Fuel`
  (CLI exit 5) rather than hanging.
- `ReviewRecordRuleTests.H3` product-file count 184 -> 185 for the new file (with a comment).
  No public API change; no new global (`Array` already existed), so rule N17 is unaffected.

## Executed on Windows

- Probe `src/tests/differential/the-array-from-async.js` (42 cases: input shapes and preference,
  sync-before-then ordering, mapper and thisArg, rejection at each point, mapper ordering, close on
  mapper throw/rejection and on a refused element, awaited `return` before rejection, constructor
  customisation for both paths, non-constructor receivers, thenables). Before the change the probe
  aborted at case 3 ([probe-before.txt](probe-before.txt)). After it,
  `python eng/run-differential.py --only the-array-from-async --against node --timeout 20` reports
  ok with three declared divergences (19, 20, 41), where Node 24.17 calls `return` after a rejected
  `next` / throwing `done` getter; the pinned spec uses `?` there and does not close. Retained
  answers were written with `--write` after that check. The explicit drain is the CLI host's
  stated end-of-script drain point.
- Full retained lane (`python eng/run-differential.py --timeout 20`): only the known pre-existing
  code-page failures (`the-later-library-methods` 147 invalid UTF-8, `the-json-date-and-regexp-surface`
  158-159) differ.
- Pinned Test262 (`ccaac100`), bytecode form. `Array.fromAsync` is in the suite's standard
  feature section, so these tests run (not skipped).
  - `--dir test/built-ins/Array/fromAsync --shards 1 --jobs 1` (95 files, 186 variants):
    before pass 0, fail 156, unsupported 30; after pass 146, fail 10, unsupported 30.
  - `--dir test/built-ins/Array --shards 1 --jobs 4` (6115 variants): before pass 5457, fail 602,
    unsupported 40, exhausted 16; after pass 5603, fail 456, unsupported 40, exhausted 16.
    146 variants fixed (all in `fromAsync`), 0 regressions (per-variant comparison of the two
    `test262.json` files). The "before" build was this worktree with the one install call removed.
- `dotnet test Broiler.VM.slnx -c Release --no-build`, before the change and again after the
  assurance write run and a rebuild: Contract 267/267, Architecture 256/256.

## Not exercised or still failing

- 10 failing variants (5 files x 2 modes: `async-iterable-input`,
  `async-iterable-input-does-not-await-input`, `sync-iterable-with-rejecting-thenable-closes`,
  `this-constructor`, `this-constructor-operations`) fail on a pre-existing front-end defect, not
  in `Array.fromAsync`: a hoisted function declaration inside a function cannot see a `let`/`const`
  of that function (`function t(){ const a = 1; function f(){ return a; } return f(); } t()` throws
  ReferenceError; reproduced with the main-tree binary). With `let`/`const` rewritten to `var` in
  local copies all five pass through the CLI; that rewrite is diagnostic only.
- 30 variants are unsupported: the test bodies use BigInt literals, which the profile refuses.
- The native form was not exercised (the artifact does not instantiate on this host:
  ProfileFault/UnsatisfiedHostAssumption, pre-existing). The change has no native-specific code.
- Cross-realm behaviour is not exercised (`$262.createRealm` refuses in this profile).
