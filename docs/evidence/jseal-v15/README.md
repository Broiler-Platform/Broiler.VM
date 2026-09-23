# Eval declarations and strictness: JSeal slice V15

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It implements steps 6-9 of the proposed decision record
[JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md),
which **nobody has signed**: approvals are deferred under the MVP terms ([`docs/mvp.md`](../../mvp.md)),
so this slice implemented a proposal and claims no approval of it. What it did differently from the
record's text is in the record's section 13. It relates to JSP-3, JSP-10 and JSW-3 and accepts none
of them.

Checkout: detached worktree at `484f389` of Broiler.VM plus the merged wave-1/2 patch
(`base-vm-w3`), Release build, Windows 11, .NET SDK 10.0.401, Node v24.17 as a diagnostic comparison
engine only. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed, in the design's order

6. **Sloppy introduction.** A sloppy direct eval's `var` and function declarations become bindings
   of its caller's variable environment: a slot the function already declares, or a deletable
   binding of the function's eval-variables set (`JsEnvironment.EvalVariables`, a prototype-less
   object held by the function record and created by the first evaluation that declares
   something). The lowering marks every sloppy function - arrows included - whose own code mentions
   `eval`, and searches it by name for every name it does not bind, exactly as it searches a `with`
   object; a call through such a name gets an `undefined` receiver (`WithBaseObject`, `0x9A`).
7. **Conflicts.** Before anything is created, the executor walks the caller's verified map from the
   site to the variable environment: a lexical binding of a declared name is a `SyntaxError` (a
   function's top-level `let`/`const`/`class`, a block's, a `for`-`let` head's, a `switch`'s, an
   enclosing evaluation's own), a `with` record is passed, and a catch clause's parameter is exempt
   whatever its form - the pinned ES2026 text conditions Annex B.3.4 on the record, not the
   parameter. Annex B block functions are hoisted per evaluation under the same walk. The map's name
   rows carry the new `EvalBindingLexical` flag, and declaration rows list `var`s, functions and
   Annex B candidates separately.
8. **Global and indirect eval on the eval goal.** An indirect eval, and a direct eval at a script's
   top level with nothing around the call, are now eval requests: their `let`/`const`/`class` stay
   their own, a strict script's direct eval is strict, their `var`s and functions become
   configurable global properties, and the global checks run first (a global lexical of the same
   name is a `SyntaxError`, `CanDeclareGlobalFunction`/`CanDeclareGlobalVar` failures are a
   `TypeError`, nothing created either way). Function objects are written with
   `StoreEvalVariable` (`0x9B`). The `Function` constructor keeps the script path.
9. **Parameter initialisers.** A direct eval in a parameter list sees the function as its parameter
   list does: parameters and `arguments` collide with its `var`s (`var arguments` there is the
   `SyntaxError` Test262 expects), the body's declarations are hidden (`EvalBindingHidden`), and what
   it declares is found by later parameters and by the body. This lowering keeps one record for a
   parameter list and its body, where the specification gives the body a separate variable
   environment, so two shapes keep an explicit `EvalError` (after review): any direct eval in the
   body of a function whose parameter list mentions `eval` - a closure the list made, in its own
   syntax or through the source it evaluated, would see the body evaluation's `var` - and a
   parameter-list eval in a function whose body declares a parameter's name (or a bound
   `arguments`) again as a function or as a `var` the body writes or mentions - a closure the
   evaluation made would see the body's writes instead of the parameter. A bare `var a;` nothing
   else names stays admitted. Also refused by name: `super`, private names, and module-scoped sites.

Also fixed after review: a call through a locally bound name `eval` is now `CallEval`, so the
executor's identity check decides directness as the language does (reference name plus SameValue
with %eval%). `function f() { var eval = globalThis.eval; eval("var sh = 1"); }` used to be an
ordinary call - a silent global evaluation whose `sh` became a configurable global property - and
now declares `sh` in `f`.

Also fixed: a directive's string is eval code's completion value (`eval("'1'")`), which the eval
lowering had missed and which the global route exposed.

The format grew, deliberately, and `src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`
records it: opcodes `WithBaseObject` (`0x9A`) and `StoreEvalVariable` (`0x9B`), the constants
`EvalBindingLexical`, `EvalBindingHidden` and `EvalBindingFlagBits`, and two more lists on
`JsEvalDeclarationRow`. No diagnostic code, decision number or product file was added.

## Acceptance, case by case

New probe `src/tests/differential/the-eval-declarations.js` (88 cases, 103 after review) and
`the-eval-declarations-in-strict-code.js` (8 cases), written before the change and run on the base
build first. There 70 of the 88 cases answered otherwise than they do now: 53 with the `EvalError`
refusal, two more as consequences of it (30, 49), and fifteen silently wrong on the old global path
(50-52, 54-55, 57, 59-60, 64, 66-70, 77: non-configurable eval globals, leaking eval lexicals, no
global conflict or definability checks). After the change every case agrees with Node except
six declared divergences, each resolved by the pinned text or Test262 rather than by Node: 39 (a
destructured catch parameter is exempt), 62 (functions are created before `var`s), 66 (every global
check precedes creation), 77 (Node runs a `.js` file as CommonJS), 78 (`var arguments` in a parameter
initialiser, Test262 `func-decl-no-pre-existing-arguments-bindings-are-present-declare-arguments.js`)
and 88 (the body refusal above). The strict probe answered wrongly in five of eight cases on the base
build and agrees with Node in all eight after it.

Cases 89-103 were added in response to the review. Their shapes were run first on the pre-review
build, which answered five of them silently wrong: a closure made by a parameter-list eval saw a
body eval's `var` (89, 90: `"number"` where the specification and Node give `"undefined"`) or the
body's redeclaration of a parameter (92, 93: `2` where they give `1`), and a locally bound %eval%
evaluated globally (100: `sh` became a global property). After the fixes 100-103 agree with Node
and 88-94, 98 and 99 are the explicit `EvalError`, each a declared divergence naming the record it
would need; 95-97 (a bare `var arguments;` or `var a;`, and a body that never redeclares) agree
with Node.

- eval `let`/`const`/`class` do not leak: cases 52-54 (indirect), 67 and 69 (script top level).
- strict eval `var` stays isolated: cases 43-46 and the strict probe's 1 and 3; strictness is
  inherited at a strict script's top level (strict probe 2, 6 and 8).
- sloppy eval `var` is introduced as specified without overwriting a conflicting lexical binding:
  cases 1-28 (introduction, reuse, closures, `with`, receivers, Annex B), 29-37 (conflicts, nothing
  created on a conflict), 38-42 (catch parameters exempt), 68, 76 and 77 (script-level conflicts),
  50-66 (global definability and configurability).
- thrown parse errors: 47-48; a throw after instantiation keeps the bindings: 49.
- delete/closure interactions: 2-3, 7-9, 64, 70, 87 (eval-created bindings are deletable); closures
  a parameter-list eval makes: 88-99.
- `arguments`: 21-22, 78-80, 95.
- directness through a local binding: 100-103.
- the four Test262 variants V14 turned into refusals pass again (table below).

## What ran

- Gate: `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 BROILER_API_WRITE=1
  dotnet test Broiler.VM.slnx -c Release --no-build`; build again; `dotnet test Broiler.VM.slnx -c
  Release --no-build`: Contract 267/267, Architecture 256/256 before and after, and again after
  the review fixes.
- Check host `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: 283 checks
  pass after the change (and after the review fixes), 2 `x86-64-sysv` rows not run on this machine. The native agreement row V14
  added for the refused declaration now declares a `var`, two functions (one block-level) and calls
  through them, and a new row runs an indirect eval declaring a configurable global; the bytecode
  and the x86-64-win64 baseline form answer `1:object:2:undefined` and `true:undefined` alike.
- CLI acceptance `python eng/run-cli-acceptance.py`: 217/217 before (base binary and base table)
  and 217/217 after (and after the review fixes). `runs/an-indirect-eval.js` asserted the old refusal of a sloppy `var`; it now
  asserts the introduced, deletable binding (`introduced:1:true:undefined`).
- Differential lane `python eng/run-differential.py --timeout 60`: before, 29 probes with only
  `the-later-library-methods` failing (the known code page 850 case); after, and again after the
  review fixes, 31 probes with only that one failing. `the-direct-eval-boundary` answers 43, 44, 64, 65 and 74 now as Node does (five stale
  declarations removed); `the-statement-and-object-surface` answers 294-296, 298 and 311 as Node does
  - the eval-as-a-script deviations its declarations attributed to JSC-142 are gone (five stale
  declarations removed). Each new probe: `python eng/run-differential.py --only <name> --against node
  --timeout 20` with declared divergences only.
- Corpus: regenerating with `SliceCompiler --write` into a scratch copy changed exactly the two
  entries compiled from source with a direct eval (`eval-scopes-a-function-site-the-lowering-wrote`,
  `eval-scopes-a-site-no-provider-answers`: the lexical flag and the dynamic callee search). After
  review those two `.bjsb` files and their `corpus.manifest` lines were replaced with the
  regenerated ones, because a V14-written map carries no `EvalBindingLexical` flags and nothing in
  the format gates it (an evaluation against it would miss a lexical conflict). A fresh
  regeneration now matches the retained corpus, and `ExecutionOnly.exe --corpus
  src/tests/corpus/js-1` passes its 22 checks. Artifacts a V14 build wrote elsewhere must be
  compiled again; JSD-0026 section 13 says so.

## Test262 (bytecode form, one shard, partial selections, not retained as floors)

```
python eng/run-test262.py --suite D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e --dir <subtree> ... --shards 1 --jobs 1 --digest-cache D:/test262-pinned/digest.cache --out <scratch>
```

The eval selection is `language/eval-code`, `annexB/language/eval-code`, `language/global-code`,
`annexB/language/global-code`, `built-ins/eval`, `language/statements/switch`,
`language/statements/try`, `language/expressions/call` and `language/function-code`: 2228 variants,
no variant skipped or unsupported, the base column run with `--binary-directory` naming the base
build's conformance host.

| Area (failing variants) | Before | After |
|---|---|---|
| `language/eval-code/direct` | 228 | 37 |
| `language/eval-code/indirect` | 31 | 2 |
| `annexB/language/eval-code` | 121 | 2 |
| `language/function-code` | 2 | 0 |
| `language/statements/switch` | 5 | 3 |
| `language/statements/try` | 12 | 11 |
| `language/global-code` | 21 | 21 |
| `annexB/language/global-code` | 17 | 17 |
| `language/expressions/call` | 12 | 12 |
| `built-ins/eval` | 0 | 0 |
| pass count, all 2228 | 1779 | 2123 |

344 variants moved from fail to pass and none from pass to fail. The pre-review build passed 2153;
the 30 variants the review fixes gave back are `*-fn-body-cntns-arguments-func-decl-declare-arguments*.js`
(15 function kinds, two files each), where a body `function arguments() {}` beside a parameter-list
`eval("var arguments")` now meets the parameter-site refusal (`EvalError`) instead of the expected
`SyntaxError`. The refusal is static: a closure another evaluation in the same list made would see
the body's function where the specification shows it the arguments object, and the compiler cannot
tell the two sources apart. The four variants V14 turned from accidental passes
into its refusal - `statements/switch/scope-var-none-case.js`, `scope-var-none-dflt.js`,
`statements/try/scope-catch-param-var-none.js` and `annexB/.../var-env-lower-lex-catch-non-strict.js`
- pass. What still fails: `$262.evalScript` and `$262.createRealm`, which the harness does not
provide (36 variants); a script's own global declaration checks and completion values, and tail
calls (not eval); the strict front-end gap V14 recorded (`var public` in strict eval code, 4
variants); `super` in eval (2) and module-scoped eval (2), refused by name; and the 30 refusals
above.

A wider selection for regressions outside these subtrees - `statements/{with,function,for-in,for,
variable,if,while,do-while,labeled,block,class}`, `expressions/{arrow-function,function,
compound-assignment,this,super,typeof,delete}`, `arguments-object`, `types/reference` and
`identifier-resolution`, 13999 variants - passed 13614 before and 13635 after, 21 variants from fail
to pass and none from pass to fail (16 skipped and 6 unsupported in both runs); the review fixes
leave it at 13635 with the same failure set.

## Not exercised

- The native form through the end-user host and the conformance harness: native artifacts on this
  machine fail to instantiate (`ProfileFault/UnsatisfiedHostAssumption`, pre-existing), so no native
  Test262 count is quoted; the in-process agreement rows above are the only native execution.
- No Linux run, no Broiler.JS comparison, no JSeal consumer run. **JSeal impact, blocking:** JSeal
  runs every classic, host and guest script through the captured `eval` intrinsic
  (`Broiler.JSeal.Vm/VmRealm.Source.cs`), which is indirect and is now global eval code rather than
  a script: a host script's top-level `let`/`const`/`class` no longer persist into the next one, its
  function and `var` globals become configurable, and its forced strictness has no place in an
  indirect request. **No VM route replaces it yet**: `JsHostRealm` has no script entry in the API
  baseline, and the executor refuses a script-goal answer to an eval request. So JSeal's pin
  update past this change is blocked until a deliberate host entry for script-goal evaluation in an
  existing realm exists (under JSD-0024 and the API baseline), which this slice does not add.
- Not fixed and not in this slice: script-level `GlobalDeclarationInstantiation` checks
  (`global-code/decl-lex-restricted-global.js`), the catch-block/parameter scope split
  (`statements/try/scope-catch-block-lex-open.js`), and a closure in a parameter list seeing a body
  `var` that shadows a parameter, which this lowering's single function record already allowed
  without `eval` (the same shapes reached through a parameter-list `eval` are refused, above).
