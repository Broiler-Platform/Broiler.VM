# JSeal I11-upstream, I12-upstream, JSD-0024-missing: module state, eval referrers, top-level await, Missing

**Date:** 2026-09-22. **Status:** local implementation validation, not accepted milestone evidence.
Base: `HEAD` plus `base-vm-w10.patch` (waves 1-9). Failing-first runs use that base tree extracted
unchanged (`git archive` of the base tree) with only the new rows copied in; the decision record is
[JSD-0024 section 20](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0024-the-in-realm-host-surface.md).

Four described changes, each with failing-first rows where the base could compile them. Taken:
diagnostic code 1632 (`MalformedScriptReferrers`, registry revision 16) and format section kind 15
(`ScriptReferrers`). No opcode, decision number, rule-register row or product file was taken.

## 1. `JsHostRealm.TryGetModuleState` (I11-upstream)

- `public bool TryGetModuleState(string moduleKey, out JsHostModuleState? state)` (with
  `[NotNullWhen(true)]`): `false` when the realm holds no module of that key. `JsHostModuleState`
  carries `Key`, `Status` (`JsHostModuleStatus`: `Unlinked` 0, `Linking` 1, `Linked` 2,
  `Evaluating` 3, `EvaluatingAsync` 4, `Evaluated` 5), `HasTopLevelAwait` (`[[HasTLA]]`),
  `EvaluationError` (the identical value; `Missing` while none), `HasEvaluationError`, and
  `CycleRoot` (the key of `[[CycleRoot]]`, empty while unset).
- Charged as `Enter(1)`, refused outside a step (`RealmNotCurrent`), runs no guest code.
  Observability of each status is specified on the member and in JSD-0024 section 20.1:
  `Evaluating` only from host code a module body calls during the walk; a graph evaluated from a
  host callback stays `Linked` until a drain; `Linking`/`Unlinked` are never answered.
- Host-surface rows (four new, one extended): statuses across a walk (read from a host function the
  body calls), an awaiting module and its importer before and after a drain, a throw inside a cycle
  that errors `root`, `c1`, `c2` with one identical value and leaves `later` `Linked`, a graph a
  host callback evaluated (`Linked` inside and after the turn, errored after the drain), equal
  primitive errors (`throw 1`) from a module that awaits and one that does not, a cycle member
  `Evaluated` while its awaiting root is `EvaluatingAsync`, a null key, and `RealmNotCurrent`
  outside a step. The expected lines were derived from the ES2026 `Evaluate`,
  `InnerModuleEvaluation` and `AsyncModuleExecutionRejected` steps before the run. These rows name
  the new member and could not be run against the base.

## 2. `SliceParseOptions.AllowTopLevelAwait` honoured (I11-upstream)

- Decision: **honour, not remove** (removing breaks a public constructor every composition calls).
  `JsParser` sets the module top level's await context from the option. With `false`, top-level
  `await`/`await using` answer `2209 ReservedWordAsBinding` and `for await` answers
  `2101 UnexpectedToken` - the diagnostics the same constructs get in a module's non-async
  function; no `[[HasTLA]]`. Defaults unchanged. Four call sites in three files (the CLI's `Host`
  and `WideHost`, two in the polyglot lane) passed `false` with `--max-depth`; they now pass
  `module`, keeping their behaviour.
- Slice-compiler check `a module compiled without top-level await refuses it by name, and one
  compiled with it does not`: failed with the parser line reverted
  ([checks-tla-base.txt](checks-tla-base.txt)); passes after. The check count moves from 371 to 372
  passed, 2 not run on this machine before and after.

## 3. The referrer of `import()` in eval and `Function` code (I12-upstream)

- `JsEngine.activeReferrer` is set by `Execute` for every frame and restored in a `finally`: a
  function's `[[ScriptOrModule]]` (`JsScriptFunction.ScriptOrModule`, fixed at creation from the
  creating frame), a module initialiser/body's key, a script body's placed referrer, and for eval
  code and `Function` programs the caller's. Built-ins enter no frame, so they see their caller's
  (`GetActiveScriptOrModule`). An `ImportCall` whose operand is empty uses it. Every queued job
  keeps the referrer active when it was enqueued and `JsEngine.CallJob` restores it around the call
  (`HostEnqueuePromiseJob`), so `p.then(eval)` and `p.then(Function)` resolve against the code that
  queued the job. Empty when no guest frame and no job is on the stack (the specification's null
  referrer): the loader is offered `""` as before.
- Scripts carry their placement in a new optional format section `ScriptReferrers` (kind 15; one
  row per script body compiled with a non-empty `JsScriptUnit.Referrer`), verified by
  `LinkScriptReferrers` (diagnostic 1632). Seven retained entries `script-referrers-*` (five
  refusals, two that run) were added; **the corpus `src/tests/corpus/js-1` was regenerated** with
  `SliceCompiler --write src/tests/corpus/js-1`, which added those seven files and seven manifest
  lines and changed no existing entry.
- Host-surface rows (three new): module code's indirect/direct eval, `Function` body, arrow and
  `Function` made by eval code, eval reached through a host function, a `Function` made by module
  code and called later by the host, a host-made `Function` (offered `""`), eval after top-level
  `await`, with completion resolving `./b` against `mem:/dir/`; and a host script's own `import()`,
  indirect and direct eval, `Function` bodies and a declared function called later, all offered
  `script:dir/page.js`; and `eval` and `Function` called directly by promise jobs module code
  queued (offered `mem:/dir/j`) beside a host job queued outside guest code (offered `""`). All
  three failed on the base ([host-surface-base.txt](host-surface-base.txt)); the job row also failed
  on this change with the job referrer reverted ([review-fixes.txt](review-fixes.txt)).
- Differential probe `src/tests/differential/the-eval-code-import-referrer.mjs` (helpers
  `modules/referrer-target.mjs`, `modules/referrer-inner.mjs`), nine cases. Every case's code is
  created by `referrer-inner.mjs`, one directory below the probe, so an empty referrer (which the
  CLI resolves against the entry directory) finds no module. The base VM answers `TypeError` for
  all nine; this build answers `true` for all nine; Node answers `true` for cases 1-8 and
  `TypeError` for case 9 (a `Function` made directly by a promise job), declared as
  `#diverges node 9` because the pinned `HostEnqueuePromiseJob` and `OrdinaryFunctionCreate` give
  that function the queuing module.

## 4. `JsHostValue.Missing` at every crossing (JSD-0024-missing)

- `Unwrap(Missing)` is now `undefined`, never the empty binding marker. Members that require a
  value refuse it first with `ArgumentException` naming the parameter: `Invoke`/`Construct`
  arguments, `SetProperty` target and value, `DefineValue`/`DefineIndex` value,
  `GetProperty`/`GetIndex` target, `NewArray` elements, `Throw`; `ResolvePromise`/`RejectPromise`
  and `DetachClone` already did. `undefined`: `Invoke` receiver, a host body's return, exotic hook
  answers, `ToJsString`/`ToNumber`/`ToBoolean`. Table in JSD-0024 section 20.4.
- Host-surface rows (two new): on the base both ended the invocation as
  `ProfileFault/ProfileContractViolation`; after, every refusal names its parameter, nothing is
  written, and the guest sees `undefined` from a receiver, a body, both hooks and a construct body.

## Commands and results

All from the worktree root, Release, 2026-09-22.

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_API_WRITE=1 BROILER_ASSURANCE_WRITE=1 dotnet
  test Broiler.VM.slnx -c Release --no-build` (write mode; the API baseline gained the 19 lines
  listed in JSD-0024 section 20); build; `dotnet test Broiler.VM.slnx -c Release --no-build`:
  Contract 267/267, Architecture 266/266, before and after (DiagnosticRegistryRuleTests updated for
  revision 16: 71 core codes, 70 corpus-reached rows).
- Slice-compiler `--checks`: 371 before, 372 after, 2 not run on this machine both times.
- `python eng/run-cli-acceptance.py --binary-directory src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0`:
  225 of 225 before and after.
- `ExecutionOnly --corpus src/tests/corpus/js-1`: 22 checks before and after (corpus regenerated
  as above).
- `Cli --host-surface`: 78 rows before; 87 after, all passing ([host-surface-after.txt](host-surface-after.txt)).
- `python eng/run-differential.py --binary-directory ...` (retained answers, all probes): every
  probe matches except `the-later-library-methods.js`, which fails identically with the base
  binary (the known code-page-850 case 147). `--only the-eval-code-import-referrer --against node
  --timeout 20`: exit 0, agreeing except the one declared divergence (case 9).
- Test262 (`python eng/run-test262.py --suite D:/test262-pinned/test262-ccaac100... --dir
  test/language/expressions/dynamic-import --dir test/language/module-code --dir
  test/language/eval-code --dir test/built-ins/Function --shards 1 --jobs 1`): 3619 variants, the
  pass count moves from 2941 to 2943; the only changed verdicts are
  `dynamic-import/usage-from-eval.js` sloppy and strict, now passing; no regressions
  ([test262-comparison.txt](test262-comparison.txt)). Re-run after the review fixes: the same totals
  and the identical failure list.
- Hot path: `Execute`'s referrer save and restore was timed on a call-heavy script (30 x `fib(22)`,
  about 1.7 million guest calls), six runs each against the base build; the wall-clock times
  overlap within run-to-run noise ([review-fixes.txt](review-fixes.txt)).

## Not exercised

- The native execution form (cannot instantiate on this machine, pre-existing).
- The 552 variants the harness skips in those directories (features it treats as proposed or
  declined, e.g. import attributes/defer); `AllowTopLevelAwait` has no Test262 coverage because
  every composition compiles modules with it true.
- The JSeal adoption itself (a separate slice in another repository); no package was built.

## What the JSeal adoption should change

1. **Status** - in `VmModule.Status`/`EvaluationError`, inside a realm step, call
   `realm.TryGetModuleState(key, out var s)` and map: `Linked` -> `Linked`; `Evaluating` ->
   `Evaluating`; `EvaluatingAsync` -> `EvaluatingAsync`; `Evaluated` with `s.HasEvaluationError` ->
   `Errored` (error `s.EvaluationError`, the identical value); `Evaluated` otherwise -> `Evaluated`,
   except that JSeal's `Evaluated` means "its promise fulfilled", so a module whose `s.CycleRoot`
   names a different key whose state is `EvaluatingAsync` should report `EvaluatingAsync`. Remove
   `MayAwait`'s source-text search (use `s.HasTopLevelAwait`), the inference from promise-reaction
   timing, and the three `InvalidOperationException` refusals; `false` from `TryGetModuleState`
   means the realm holds no such module.
2. **Dynamic import** - no API change: `OfferImport` now receives the calling module's key, or the
   script's placed referrer (`\u0001script:<label>`, which `VmSourceProvider` already sets for
   host scripts) for eval and `Function` code, including eval or `Function` a promise job calls
   directly. Keep answering `Now` only when `request.Referrer` is empty (no script or module on the
   stack: host code outside any guest frame, or a job such code queued), and drop the
   "eval/Function refused" gap.
3. **Missing** - `VmMarshal` must not pass `JsHostValue.Missing` where it means `undefined`;
   the members listed in section 4 now throw `ArgumentException` for it, and a host body or hook
   that answers `Missing` gives the guest `undefined`.
4. **Top-level await** - no change needed: JSeal compiles modules with `SliceParseOptions.Module`
   (`AllowTopLevelAwait` true).
