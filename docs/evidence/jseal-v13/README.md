# JSeal slice V13: the direct-eval environment boundary (design only)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
V13 adds one proposed decision record,
[JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md),
and its row in the decisions index. No product code, format, opcode or retained answer changed, so
there is no before/after behaviour to compare; the runs below confirm that nothing regressed and
record the observations the record's section 1 quotes.

Checkout: detached worktree at `484f389` of Broiler.VM, Release build, Windows 11, .NET SDK 10.0.401.
Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`. Comparison engine (diagnostic
only, not the oracle): Node v24.17.

## Build and tests

```
dotnet build Broiler.VM.slnx -c Release               # 0 warnings, 0 errors (before and after)
dotnet test Broiler.VM.slnx -c Release --no-build     # Contract 267/267, Architecture 256/256 (before and after)
```

No assurance regeneration: no product code unit changed.

## Test262 (partial selections, not retained as floors)

```
python eng/run-test262.py --suite D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e --dir <subtree> --form <form> --shards 1 --jobs 1 --digest-cache D:/test262-pinned/digest.cache --out <scratch>
```

| Subtree | Form | Files | Variants | Passed | Failed |
|---|---|---|---|---|---|
| `test/language/eval-code` | bytecode | 347 | 454 | 139 | 315 |
| `test/language/eval-code` | native | 347 | 454 | 0 | 454 |
| `test/annexB/language/eval-code` | bytecode | 469 | 470 | 308 | 162 |
| `test/language/types/boolean` | native | 5 | 10 | 4 | 6 |

Of the 315 bytecode failures in `test/language/eval-code`, 149 are `Expected a SyntaxError but got a
EvalError` and 42 are the function-scope refusal uncaught. Every native failure, in both native
rows, is `the artifact would not instantiate: ProfileFault/UnsatisfiedHostAssumption`; because
`types/boolean` fails the same way, the native failure is not specific to `eval` and its cause is
not diagnosed here. No native count for eval is quoted anywhere as a result.

## Behaviour probes

Small scripts run through
`src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Cli.exe --quiet`
(one also with `--module`), each wrapped as `var x = "global"; try { <case> } catch (e) { print(...) }`,
and through Node with `print = console.log`. The intended answer is the pinned ES2026 specification.
The function-scope, spread, `with`, global and module rows are listed in JSD-0026 section 1. Rows added
after review, all at a script's top level:

| Case | VM | Specification | Node |
|---|---|---|---|
| `{ let x = 'block'; eval('x') }` | `global` | `block` | `block` |
| `for (let i = 5; i < 6; i++) eval('i')` | `ReferenceError` | `5` | `5` |
| `try { throw 'caught' } catch (e) { eval('e') }` | `ReferenceError` | `caught` | `caught` |
| `{ let y = 1; eval('y = 2'); y }` | `1` | `2` | `2` |
| `with ({ x: 'with' }) eval('x')` | `global` | `with` | `with` |
| `switch (1) { case 1: let s = 'sw'; eval('s') }` | `ReferenceError` | `sw` | `sw` |
| `try { throw 1 } catch (e) { eval('var e = 2'); e }` | `1` | `2` | `2` |
| `try { throw [1] } catch ([e]) { eval('var e = 2'); e }` | `1` | `2` | `SyntaxError` |
| `{ eval('x') }`, `{ function bf(){} eval('x') }`, `eval('x')` | `global` | `global` | `global` |
| arrow in a function: `(() => eval('arguments[0]'))()` | `EvalError` (refusal) | the function's argument | same |

The destructured-catch row follows the pinned `EvalDeclarationInstantiation` text, whose Annex B
replacement step exempts any catch-clause environment record; the simple-parameter condition is
only in the static early error. Node's `SyntaxError` there is its divergence.

## Not exercised

No differential probe was added: a design-only slice changes no answer, and each implementation step
in JSD-0026 section 9 adds its own. The full retained differential lane was not run. The native
instantiation failure above was not investigated. No Linux run.
