# A script-goal host route: JSeal slice V15-host

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It adds section 16 to the in-realm host surface record
[JSD-0024](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0024-the-in-realm-host-surface.md)
and closes the gap [JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md)
section 13 and [`../jseal-v15/README.md`](../jseal-v15/README.md) name: JSeal cannot take a VM
package containing V15 until it has a script-goal route into an existing realm. It relates to JSP-3,
JSP-10 and JSW-3 and accepts none of them; no human has reviewed or signed anything here.

Checkout: detached worktree at `484f389` of Broiler.VM plus the merged wave-1..3 patch
(`base-vm-w4`), Release build, Windows 11, .NET SDK 10.0.401, Node v24.17 as a diagnostic comparison
engine only. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`. "Before" figures come
from a copy of the base tree built beside the worktree.

## What changed

- **`JsHostRealm.EvaluateScript(source, sourceName = "", strict = false)`**, a new public member in
  the API baseline: `ScriptEvaluation` in the realm the embedder already holds, answering the
  completion value. It is a crossing (rule N21: `Enter` first, one `HostCalls` unit and 4 fuel), then
  the request's length in fuel, then the script under the same allowance.
- **A fourth request mark, `JsFormat.ScriptRequestMark` (`0x02`)**, with a flags byte (`Strict`), the
  source name and the source. The profile writes it only from `EvaluateScript`; guest `eval`,
  `import()` and `Function` cannot, so a provider may answer it under a policy that refuses guest
  evaluation. `JsCompiler.TryReadProgramRequest` reads it into a script unit carrying the name
  (`JsScriptUnit.SourceName`) and the forced strictness, so every provider built on it speaks it.
  The source-provider capability stays at version 2.
- **`GlobalDeclarationInstantiation` for every script.** A new optional section,
  `ScriptDeclarations` (kind 14), carries each script body's lexical, `var`, function and Annex B
  names; the verifier links it (new diagnostic `MalformedScriptDeclarations`, 1631, registry revision
  15) and the executor runs every conflict and definability check before the body's first
  instruction, then defines function bindings as `CreateGlobalFunctionBinding` does. A sloppy script
  whose Annex B candidate would not be hoisted is refused with an `EvalError` by name, because the
  lowering writes the alias unconditionally. Artifacts without the section keep the old lenient
  instantiation.
- **`$262.evalScript`** in the conformance harness (`Test262Host`) runs its argument through the new
  member. The profile's own refusing stub now says the host installed no script evaluation.
- Retained corpus: eight `script-declarations-*` entries (five malformed rows, three that run, one of
  which is `let undefined;` answering `uncaught:SyntaxError`), and four existing entries re-written
  by the producer because their scripts declare functions and now carry the section; the producer
  output equals the retained files again.

## Commands and results

| Check | Before | After |
|---|---|---|
| `dotnet build Broiler.VM.slnx -c Release` | 0 errors | 0 errors |
| `dotnet test Broiler.VM.slnx -c Release --no-build` | Contract 267/267, Architecture 256/256 | Contract 267/267, Architecture 256/256 |
| slice-compiler `--checks` | 327 passed, 2 not run | 327 passed, 2 not run |
| `python eng/run-cli-acceptance.py --binary-directory .../Cli/bin/Release/net10.0` | 219/219 | 224/224 |
| the same five new rows against the base binary | 4 of the 224 rows fail (both `b-*` and both `c-*` rows) | - |
| CLI `--host-surface` | 43 checks, all pass | 51 checks, all pass |
| ExecutionOnly `--corpus src/tests/corpus/js-1` | 22 checks (152 entries replayed) | 22 checks (160 entries replayed) |
| `python eng/run-differential.py --timeout 20` | lane not run on the base copy (not a git checkout); `the-later-library-methods` printed the same lines under the base binary | every probe matches its retained answers except the long-standing case 36 of `the-later-library-methods` (known issue, output identical to the base binary's) |

The API baseline was regenerated with `BROILER_API_WRITE=1` and reviewed; the assurance files with
`BROILER_ASSURANCE_WRITE=1`, then rebuilt and run in gate mode.

New probe `src/tests/differential/the-script-declarations.js` (12 cases, one script): run with
`--only the-script-declarations --against node --timeout 20`, it agrees with Node on eight cases and
declares four divergences, all because Node runs a `.js` file as CommonJS, whose top-level
declarations are a function's locals. Its answers are the same under the base binary: a single
script cannot show a cross-script conflict, so it pins script-level declaration semantics rather than
this slice's change, which the CLI rows, the host-surface lane and Test262 show.

## Test262 (bytecode form, `--shards 1 --jobs 1`)

| Selection | Before | After |
|---|---|---|
| `test/language/global-code` + `test/annexB/language/global-code` | 190 of 228 pass | 228 of 228 pass |
| `test/language/eval-code` + `test/annexB/language/eval-code` | 886 of 924 pass | 888 of 924 pass |
| `test/language` + `test/annexB` + `test/built-ins/Proxy/revocable` | of 45664 variants: passed 43239, failed 1285, exhausted 16 | of 45664 variants: passed 43277, failed 1245, exhausted 18 |

Every variant whose verdict moved is in [`test262-comparison.txt`](test262-comparison.txt): 40 moved
from failed to passed and none from passed to failed. Two moved from passed to exhausted
(`expressions/async-arrow-function/prototype.js`, both variants, "the allowance was spent running a
harness file", WallClock) while other agents' runs loaded the machine; a rerun of
`test/language/expressions/async-arrow-function` after the change passes 110 of 110. The one other
suite file that calls `$262.evalScript`, `built-ins/Proxy/revocable/tco-fn-realm.js`, still fails on
`$262.createRealm`.

## Not exercised

- The native execution form (it cannot instantiate on this machine), Linux, and the whole suite
  outside the selection above.
- No JSeal consumer run and no package: JSeal's adoption is a separate slice.
- A provider that does not speak the script mark is only described (it refuses it as a reserved
  mark); no such provider was run.
