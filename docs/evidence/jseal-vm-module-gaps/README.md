# Module-scoped direct eval and async module evaluation: JSeal V15-module and I11-async

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It closes two gaps that earlier slices left open. The first is the direct eval sites that were
refused by name when their scope reached a module (V15,
[JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md)
section 15). The second is the specification's async module evaluation (I11 upstream,
[JSD-0024](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0024-the-in-realm-host-surface.md)
section 15.4). Both records are proposed and unsigned. This work relates to JSP-3, JSP-10 and JSW-3
and accepts none of them. JSeal has not adopted it.

Checkout: detached worktree of Broiler.VM at HEAD plus `base-vm-w5.patch`. Release build on
Windows 11 with .NET SDK 10.0.401. Node v24.17 was used only as a diagnostic comparison engine.
Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed

1. **The eval scope map has a module row** (`EvalScopeKind.Module = 8`). It lists the module's
   slots and its imports; each import carries the new flag `EvalBindingImport = 16` and indexes the
   artifact's import table. An evaluation reads an import through the exporting module's
   environment, so it sees the live value and the dead zone; a write throws `TypeError`. Every site
   in a module has a row, including one at the top level with no block around it. Module code is
   strict, so the evaluation keeps its own `var`s and functions. Eval code is Script-goal code, so
   `import`, `export` and `import.meta` in it are `SyntaxError`s. The verifier admits the row kind
   only as the root of a module body's chain. It admits the flag only on that row, only together
   with the immutable flag, and only when the entry is inside the import table.
2. **The module evaluation walk follows the pinned ES2026 algorithm.** It implements `Evaluate`,
   `InnerModuleEvaluation`, `ExecuteAsyncModule`, `GatherAvailableAncestors` and
   `AsyncModuleExecutionFulfilled`/`Rejected`, and each instance carries the specification's own
   fields. This replaces the ordered list, its wait entries and its give-back. An async module no
   longer holds up its siblings. A module that waits on async dependencies runs when the last of
   them finishes. The modules one completion releases run in `[[AsyncEvaluationOrder]]`.
   `[[EvaluationError]]` caching and `[[CycleRoot]]` are the algorithm's own.
3. **Evaluations never overlap.** A guest `import()` links and evaluates from a job, as
   `ContinueDynamicImport` does. It settles from the reaction to the evaluation's promise, so it
   settles two turns later than before. A host `EvaluateModule` that arrives while an evaluation is
   running is deferred the same way.
4. **The late failure of an entry graph is raised.** Nobody holds the entry graph's evaluation
   promise. If it rejects after an `await`, the rejection is raised from a job that the drain
   reports; before, it was lost. The conformance harness (`Test262Run.cs`) counts such a job as the
   runtime error a negative module test declares.

Fuel and cancellation are unchanged in kind. Each module visited, parent recorded, ancestor
gathered and module released is charged one unit. Every wait is a promise reaction.
No product file, opcode, diagnostic code or decision number was added. The public additions
(`EvalScopeKind.Module` and `EvalBindingImport`, plus `EvalBindingFlagBits` going from 15 to 31)
are recorded in `docs/api/public-api.txt`.

## Regressions first

- `the-module-scoped-eval.mjs` has 35 cases, with dependencies `modules/scoped-eval-*`. The base
  build met the `EvalError` refusal in 34 of them
  ([scoped-eval-probe-before.txt](scoped-eval-probe-before.txt)).
- `the-async-module-siblings.mjs` has 11 cases, with dependencies `modules/siblings-*`. The base
  build differed from Node in cases 1, 2 and 4 to 8
  ([siblings-probe-before.txt](siblings-probe-before.txt)). There the async siblings ran one after
  another, and failures stopped the sibling that was still suspended.
- `the-direct-eval-in-a-module.mjs`: its two `#diverges` declarations, which recorded the old
  refusal, are removed. Its retained answers now agree with Node.

## Executed

- Build, and `dotnet test Broiler.VM.slnx -c Release --no-build`: before, Contract 267/267 and
  Architecture 256/256. After the API baseline write, the assurance write run and a rebuild, gate
  mode gives Contract 267/267 and Architecture 256/256.
- Slice-compiler `--checks`: 353 passed and 2 not run, before and after. The retained corpus was
  regenerated with `--write src/tests/corpus/js-1`. It gained only the two new refusal entries,
  `eval-scopes-an-import-outside-a-module-row` and `eval-scopes-a-module-row-under-a-script-body`
  (`MalformedEvalScopes`, 1627). The ExecutionOnly replay passes 22 checks.
- CLI acceptance: 224 of 224, before and after. `--host-surface`: every check passed, before and
  after, including the eight module checks.
- Differential probes compared with Node (`--against node`): `the-module-scoped-eval`,
  `the-async-module-siblings`, `the-direct-eval-in-a-module`, `the-module-identity`,
  `the-module-evaluation-errors`, `the-module-goal`, `the-module-namespace-integrity` and
  `the-using-declarations-in-a-module` all exit 0. The full lane differs only in the known case 36
  of `the-later-library-methods`.
- Pinned Test262 (bytecode form, `--shards 1 --jobs 1`, one output directory per run):
  - Module selection over `language/module-code`, `expressions/dynamic-import`,
    `eval-code/direct`, `language/import`, `expressions/import.meta` and `built-ins/Promise`
    (4129 variants). The pass count moves from 3293 to 3301. Eight variants go from fail to pass
    and none from pass to fail ([test262-module-selection.txt](test262-module-selection.txt)).
  - Eval selection over `eval-code`, `function-code`, `arguments-object`, class elements,
    `expressions/super` and `global-code` (7347 variants; the base binaries were kept from before
    the change). The pass count moves from 7226 to 7228: `export.js` and `import.js` go from fail
    to pass, and none go from pass to fail ([test262-eval-selection.txt](test262-eval-selection.txt)).
  - The 36 skipped files in `top-level-await` are fixtures, and they run through the tests that
    import them.

## Not exercised, and what still fails

- The native execution form (the pre-existing `UnsatisfiedHostAssumption` on this machine). No
  opcode was added.
- `top-level-await/await-expr-reject-throws.js` still fails. It now reaches its real cause: an
  `await` of a promise rejected with a Symbol resumes with a `TypeError`. That bug belongs to async
  functions in general and is outside this slice.
- The tick at which `import()` settles now follows `ContinueDynamicImport` when the load is
  synchronous. No probe measures that tick against a second engine.
- JSeal's adoption (I11/I12). No Linux run.
