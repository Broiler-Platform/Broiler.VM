# Host surface: module graphs and deferred imports (JSeal I11 upstream)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the VM host additions JSeal's I11 and I12 need (JSD-0024, section 15). It does not mark
any JSP, JSW or JS milestone complete, and JSeal has not adopted the API.

This record replaces the first implementation's record. That implementation was reviewed but not
merged, because VM-FIX-D ([../jseal-frontend-fixes/README.md](../jseal-frontend-fixes/README.md))
reworked the same module evaluation walk at the same time. This version is rebased onto the merged
tree (484f389 plus `base-vm-w4.patch`) and has one evaluation walk that passes both slices' tests.

## One evaluation walk

The two designs were compared against the pinned ES2026 algorithms (`InnerModuleEvaluation`,
`Evaluate`, `AsyncModuleExecutionFulfilled`/`Rejected`, `[[EvaluationError]]`, `[[CycleRoot]]`,
`ContinueDynamicImport`):

- **Kept from VM-FIX-D**:
  - `JsModuleInstance.EvaluationError`;
  - wait entries (`~index`) for a module another walk has ordered, is running or is still awaiting;
  - a waiting walk gives back its own unrun modules before it waits, then orders the graph again.
    It holds no claim while it waits. The first implementation kept its claims while waiting and
    retried in place. By analysis (not reproduced) that could deadlock after a give-back: two
    walks could each wait on a module the other holds.
  - the failure marking over the order;
  - `JsAsyncCall.DiscardsCompletion`, which is the same fix as the first implementation's
    `FulfilsWithUndefined`.
- **Ported from the first implementation**:
  - **Depth-first indices and `[[CycleRoot]]`.** `Order` now keeps `[[DFSIndex]]` and
    `[[DFSAncestorIndex]]` in a per-walk `JsModuleVisit`. Every module it claims learns its
    component's root (`JsModuleInstance.CycleRoot`).
  - **`Evaluate` step 3.** A finished module whose cycle root is not finished sends the walk to the
    root. The walk waits there, runs the root, or fails with the root's error. VM-FIX-D answered
    such a member as finished. That is wrong in two cases: while the root is still awaiting (probe
    case 17), and when a walk that stopped to wait gave the root back unrun (case 18, a gap that
    VM-FIX-D's give-back opens). When that root later throws, VM-FIX-D resolved imports of the
    member and ran a module that depends on it (case 19). The first implementation's `Owner` and
    `Done` bookkeeping was not needed on top of this and was not ported.
  - **Resolutions are confirmed before any instance is registered** (`Instantiate`). Before this,
    a refused graph left uninitialised instances that later imports adopted.
- **Host operations re-applied unchanged in behaviour**:
  - `JsHostRealm.LoadModule`, `EvaluateModule`, `CompleteModuleRequest` and `FailModuleRequest`;
  - `IJsHostModuleLoader`, `JsHostModule`, `JsHostModuleLoad` and `JsHostModuleRequest`;
  - the engine side: `EvaluateInto`, `LinkHostModule`, `CompleteImport`, and `TryOwnRequest` plus
    `MediatedModule`, which replace `ImportedModule`.
  - The API baseline additions are the same 16 lines.
  - JSD-0024 section 15 was updated to describe the merged walk. No new product file, opcode,
    diagnostic code or decision number was taken.

Fuel, cancellation, allocation accounting and guest exception handling are unchanged. The ordering
walk charges one unit per module it claims or meets. The search for a cycle root charges one unit
per module it passes. Every wait is a promise reaction.

## Executed on Windows 11, .NET SDK 10.0.401, Release

- **Base first.** `dotnet build Broiler.VM.slnx -c Release`, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  Slice-compiler `--checks`: all 327 applicable checks pass and 2 are not run on this machine.
  `python eng/run-cli-acceptance.py`: 219 of 219. `--host-surface`: every check passed.
- **Regressions first.** `the-module-identity.mjs` has 19 cases. Cases 18 and 19 were added for
  this rebase, with six dependencies under `modules/identity-split-*` and
  `modules/identity-rootfail-*`. A separate build of the base tree answers cases 17, 18 and 19
  differently from Node v24.17 ([probe-before-rebase.txt](probe-before-rebase.txt)):
  - case 17: the member settled before its root finished;
  - case 18: the same, with the root given back while the walk waited;
  - case 19: a module depending on the member ran, and the member's imports resolved.
- **After.**
  - `python eng/run-differential.py --only the-module-identity --against node --timeout 20` and
    `--only the-module-evaluation-errors` (VM-FIX-D's 21 cases) both exit 0.
  - The full lane `python eng/run-differential.py --timeout 30` differs only in the known case 36
    of `the-later-library-methods` (cube root).
  - `--host-surface`: every check passed, including the eight module checks in
    `HostSurfaceChecks.Modules.cs` ([host-surface-after.txt](host-surface-after.txt)).
  - CLI acceptance: 219 of 219, which includes VM-FIX-D's rows.
  - Slice-compiler `--checks`: all 327 applicable checks pass and 2 are not run on this machine.
  - After the assurance write run and a rebuild, gate mode gives Contract 267/267 and
    Architecture 256/256.
- **Pinned Test262** (`ccaac100`, bytecode form, `--shards 1 --jobs 1`).
  - Selection: `test/language/module-code`, `test/language/expressions/dynamic-import`,
    `test/language/import` and `test/built-ins/Promise` (2604 files, 3765 variants).
  - Base and final are identical: pass 2941, fail 55, unsupported 14, skipped 755, with the same
    failing variants. The Test262 cases the first implementation fixed already pass on this base,
    through VM-FIX-D. The skipped variants are fixtures and proposed features, so they are not
    evidence. No Test262 case reaches the host surface or the cycle-root cases above.

## Earlier records kept here

[probe-before.txt](probe-before.txt), [probe-before-review-fixes.txt](probe-before-review-fixes.txt),
[host-surface-before-fixes.txt](host-surface-before-fixes.txt) and
[host-surface-before-review-fixes.txt](host-surface-before-review-fixes.txt) come from the first
implementation. They were recorded against its earlier base, before VM-FIX-D.

## Not exercised

- The native execution form. It cannot be instantiated on this machine (the pre-existing
  `UnsatisfiedHostAssumption`).
- JSeal's adoption (I11/I12). There are no JSeal changes here.
- A budget-exhaustion case for `LoadModule`. The charge is structural (N21 `Enter`, plus the
  mediator's host-call, byte and verifier charges).
- A host-visible module status. JSeal can derive one from the `EvaluateModule` promise, but that
  derivation has not been written.
- Concurrent evaluation of independent async siblings. This deviation predates the slice:
  `async-module-does-not-block-sibling-modules.js` still fails. The order in which two waiting
  evaluations settle can also differ from the specification's async-parent order. In an ad-hoc
  variant of case 19, Node settles the dependant's import before the root's, and the VM settles
  the root's first. The outcomes agree, and the probe compares outcomes rather than that order.
- An async entry module that rejects after its first `await` is still not reported by the end-user
  host. This predates the slice (`module-import-rejection*.js` still fail).
