# Direct eval against caller bindings: JSeal slice V14

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It implements steps 1-5 of the proposed decision record
[JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md),
which **nobody has signed**: the owner has not reviewed it and approvals are deferred under the MVP
terms ([`docs/mvp.md`](../../mvp.md)), so this slice implemented a proposal and claims no approval of
it. What V14 did differently from the record's text is in the record's section 12. It relates to
JSP-3 and JSW-3 and accepts neither.

Checkout: detached worktree at `484f389` of Broiler.VM plus the merged wave-1 patch, Release build,
Windows 11, .NET SDK 10.0.401, Node v24.17 as a diagnostic comparison engine only. Pinned suite:
`test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed, in the design's order

1. **Refusal hygiene.** A direct `eval` at a module's top level, and in a script body whose current
   record is not its entry record (a block, `for`-`let`, `switch`, `catch` or `with` record around
   the call), throws the explicit `EvalError` instead of evaluating globally. `eval(...xs)` is
   lowered to the new `CallEvalSpread` (`0x90`) and keeps its directness; an `eval` with a `with`
   record between the call and a static binding of the name is `CallEval`, so the identity check
   decides. The silent wrong answers of JSD-0026 section 1 at those sites became the refusal first;
   the ones at a script's top level with no record in between did not (see below).
2. **Format.** Section kind 13 (`EvalScopes`: scope shapes, sites, eval declarations), the
   `EvalCode` function flag, the five name instructions `LoadEvalName`, `LoadEvalNameOrUndefined`,
   `StoreEvalName`, `LoadEvalNameWithBase`, `DeleteEvalName` (`0x91`-`0x95`, wide set only; the
   numeric manifest refuses `EvalCode`). The verifier holds the map to the code, the function table
   and the abstract pass's scope depth; two core codes, `EvalScopesOutsideManifest` (1626) and
   `MalformedEvalScopes` (1627), are diagnostic registry revision 13, reached by thirteen retained
   `eval-scopes-*` corpus entries (ten refusals, three that verify and run).
3. **Request vocabulary.** The `0x01` eval request (mark, flags byte, source), source-provider
   capability version 2 (`(source-request)->artifact`, minimum provider version 2), and one shared
   dispatch, `JsCompiler.TryReadProgramRequest`, adopted by the end-user host, the conformance
   harness, the polyglot host and the native lifecycle check host.
4. **Compiler.** A site row for every direct-eval site in a function unit, in eval code, and in a
   script body under a record; `arguments` materialised by every non-arrow function that mentions
   `eval`; the eval goal (`SliceParseOptions.Eval`), whose top-level `let`/`const`/`class` - and in
   strict code `var` and functions - are slots of its own boundary record and whose free names are
   eval name instructions.
5. **Executor.** A site with a row sends the eval request once per evaluation, binds the answer to
   it (entry `eval`, `EvalCode`, the site's flags), enters a fresh boundary record under the
   caller's current record with the caller's `this`, `new.target`, `this` cell and active function,
   and resolves free names through the map one record at a time (one fuel unit each). Still refused
   by name: a sloppy evaluation declaring a `var` or function (V15), the caller's `super`, a private
   name of the calling class, a site in a parameter initialiser (step 9) and any site whose scope
   reaches a module.

Acceptance: `function f(){ let x = 7; return eval('x') }` answers 7 (probe case 1); an eval
assignment is visible to the caller and to its closures (cases 2-4); nested and re-entrant calls
evaluate against their own records (cases 5-8); restricted compilation is refused on the new route
as on the others (corpus entry `eval-scopes-a-site-no-provider-answers`: a composition with no
provider answers the catchable `EvalError`); the interpreter and the baseline native form agree
(three new rows of the check host, below).

## What stays wrong: a script's top level with nothing in between

A direct eval at a script's top level with no record between it and the body has no site row and
still takes the old global script path; JSD-0026 plans its goal change for step 8 (V15). This slice
neither refuses nor fixes it, so these answers stay silently wrong, exactly as on the base build
(worktree CLI, script goal; the right answer in brackets):

- eval lexicals leak as global lexicals: `eval('let zz = 1'); typeof zz` is `"number"`
  (`"undefined"`);
- no global conflict check: `let c = 1; eval('var c = 2')` assigns `c = 2` without an error
  (`SyntaxError`, `c` stays 1);
- strictness is not inherited: `"use strict"; eval('var sv = 1'); typeof sv` is `"number"`
  (`"undefined"`), and a strict caller's `eval('with ({}) {}')` runs (`SyntaxError`);
- eval-created globals are not configurable: `eval('var gg = 1'); delete gg` is `false` (`true`).

Strict isolation and eval-local lexicals therefore hold at sites with a scope-map row - in function
code, in eval code and in a script body under a block, `for`-`let`, `switch`, `catch` or `with`
record - and not at the plain script top level.

## What ran

- Gate: `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 dotnet test
  Broiler.VM.slnx -c Release --no-build`; build again; `dotnet test Broiler.VM.slnx -c Release
  --no-build`: Contract 267/267, Architecture 256/256 before and after. The architecture changes
  are the profile API baseline (`BROILER_API_WRITE=1`, additions only), the registry's revision and
  counts, and the reviewed-file count (JsEvalMap.cs is new).
- Check host `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: every row
  passed before and after except the two `x86-64-sysv` rows this machine cannot enter; it has three
  new rows, `native/baseline/two-forms-agree-over-the-wide-manifest/a direct eval ...`, in which the
  bytecode and the baseline form (x86-64-win64, emitted, armed and run in process) answer
  `20:21:20`, `local` and `EvalError` alike.
- Corpus: `--write src/tests/corpus/js-1` added thirteen entries and changed no existing byte;
  `ExecutionOnly.exe --corpus src/tests/corpus/js-1` replays all of them;
  `python eng/corpus-integrity.py` detects both mutations.
- Conformance harness suites `js-3a` (44/44) and `ingest-shape` (5/5), CLI runs (88/88): unchanged.
- CLI acceptance `python eng/run-cli-acceptance.py`: 209/209 before, 210/210 after. The run
  `an-indirect-eval.js` asserted the old function-scope refusal; it now asserts that a local is
  read and written (`local:2`) and that a sloppy `var` declaration is still refused.
- Differential: new probes `the-direct-eval-boundary.js` (76 cases) and
  `the-direct-eval-in-a-module.mjs` (4 cases), `python eng/run-differential.py --only <name>
  --against node --timeout 20` exit 0 with declared divergences only. The full retained lane fails
  only `the-json-date-and-regexp-surface` and `the-later-library-methods`, the known code page 850
  cases, exactly as on the base build.

### The probe, step by step

`the-direct-eval-boundary.js` was run on the base build (the main tree's Release CLI, built from
the same base), after step 1 and after step 5. On the base build 55 of the 76 cases answered
`EvalError` and nine answered a wrong value silently: cases 22 and 24 (a spread direct eval,
`"global"`), 26 (`with` in front of a local `eval`, `"global"`), and 68-73 (script-body evaluations
under a block, `for`-`let`, `catch`, `with` or `switch` record: `"global"`, `ReferenceError` or a
stale `1`). After step 1 all nine answered `EvalError`, and so did case 74, a sloppy `var` declared
from a script-level block, whose global answer had been right only because nothing in between
declared the same name - the design refuses it until step 8 rather than keep a path that is wrong
as soon as something does. After step 5, 66 cases agree with Node and
ten differ, each declared: 22 and 24 (the comparison engine evaluates a spread direct eval
globally; Test262 `expressions/call/eval-spread.js` and the specification say it is direct), 43,
44, 74 (sloppy declarations, V15), 46 and 47 (`super` and private names, refused by name), 64 and
65 (parameter initialisers, step 9) and 76 (Node runs a `.js` file as CommonJS, where the top level
is a function body).

## Test262 (bytecode form, one shard, partial selections, not retained as floors)

```
python eng/run-test262.py --suite D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e --dir <subtree> ... --shards 1 --jobs 1 --digest-cache D:/test262-pinned/digest.cache --out <scratch>
```

| Subtree | Variants | Before | After step 1 | After step 5 |
|---|---|---|---|---|
| `language/eval-code/direct` | 336 | 55 | 52 | 106 |
| `language/eval-code/indirect` | 118 | 84 | 84 | 85 |
| `annexB/language/eval-code` | 470 | 308 | 307 | 349 |
| `built-ins/eval` | 20 | 18 | 18 | 20 |
| `language/expressions/call` | 171 | 151 | 151 | 159 |
| total | 1115 | 616 | 612 | 719 |

The figures are pass counts. Step 1 moved four variants from pass to fail, all accidental passes of
the global path under a block or catch record; three of them pass again after step 5 and the fourth,
`annexB/.../var-env-lower-lex-catch-non-strict.js`, is a sloppy function declaration in a catch
clause, which V15 owns. From the base to step 5, 104 variants moved from fail to pass and that one
from pass to fail. What still fails is dominated by V15 and step 8: sloppy declarations (the
`EvalError` refusal, and 139 variants expecting a conflict `SyntaxError`), script-level direct eval
still taking the global path (configurable globals, lexical isolation, inherited strictness), and
the parameter-initialiser and module refusals.

### A wider selection, for regressions outside the eval subtrees

The same command over the eval subtrees above plus every `language` subtree whose tests call
`eval` most - `statements/{with,try,switch,function,for-in,for,variable,if,while,do-while,labeled}`,
`expressions/{arrow-function,function,compound-assignment,this,super}`, `arguments-object` and
`types/reference` - 6870 variants. The base column was run with `--binary-directory` naming the
main tree's Release conformance host, built from the same base; over the eval subtrees it answers
exactly the "Before" column above, variant for variant. The final column is this worktree's final
build (whose eval-subtree figures are the "After step 5" column above).

| Selection | Base | Final |
|---|---|---|
| all 6870 variants | 6078 | 6238 |
| `statements/function` | 745 | 760 |
| `expressions/compound-assignment` | 702 | 720 |
| `expressions/this` | 4 | 11 |
| `statements/switch` | 213 | 211 |
| `statements/try` | 375 | 376 |

164 variants moved from fail to pass and four from pass to fail. All four are sloppy direct evals
that declare a `var` or function under a script-level record and used to pass through the global
path by accident - `statements/switch/scope-var-none-case.js`, `scope-var-none-dflt.js`,
`statements/try/scope-catch-param-var-none.js` and the annexB catch case above - and each now
answers the explicit V15 refusal, which is JSD-0026's order (refused in step 1 and 5, admitted in
step 8). No other subtree lost a variant.

## Not exercised

- The native form through the end-user host and the conformance harness: every native artifact on
  this machine verifies and then fails to instantiate (`ProfileFault/UnsatisfiedHostAssumption`),
  as recorded by V13, so no native Test262 count is quoted. The in-process agreement rows above are
  the only native execution observed.
- No Linux run; no Broiler.JS comparison; no JSeal consumer run. JSeal's `VmSourceProvider` copies
  the capability version from the profile descriptor, so after its pin update it claims version 2
  without dispatching the eval mark: a direct eval in a function there, refused today with the
  `EvalError`, would be answered as a misleading `SyntaxError`. Adopting
  `JsCompiler.TryReadProgramRequest` is a blocking precondition of that pin update (JSD-0026
  section 8), with a JSeal regression for direct eval in a function, which JSeal lacks today.
- A front-end gap met on the way and not fixed: strict eval code does not refuse `var public` or
  `var static` (nor does strict script code), so `direct/strict-caller-function-context.js` and
  `expressions/call/eval-strictness-inherit-strict.js` still fail.
