# A function body's own variable environment, and super and private names in eval: JSeal V15-finish

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It finishes what JSeal V15 left refused under the proposed, **unsigned** decision record
[JSD-0026](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0026-the-direct-eval-environment-boundary.md);
what it did is recorded in the record's new section 14. Approvals are deferred under the MVP terms
([`docs/mvp.md`](../../mvp.md)). It relates to JSP-3, JSP-10 and JSW-3 and accepts none of them.

Checkout: detached worktree at `484f389` of Broiler.VM plus the merged wave-1 to wave-3 patch
(`base-vm-w4`), Release build, Windows 11, .NET SDK 10.0.401, Node v24.17 as a diagnostic comparison
engine only. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed

1. **FunctionDeclarationInstantiation step 28.** A function whose parameter list has expressions
   (`ContainsExpression` of the formals: a default, a pattern default, a computed pattern key) and
   where the difference is observable - the list makes a closure (function, arrow or class
   expression, object-literal method, or a mention of `eval`), or the sloppy body mentions `eval` -
   now pushes a record for its body after the parameter list. Body `var`s, functions and top-level
   lexicals live there; a body `var` of a parameter's name (and of `arguments` when the object is
   bound) starts with the parameter's value. Closures made by the parameter list, in their own syntax
   or through a parameter-list eval, see the parameters and never the body's declarations. A list
   without expressions keeps one record, as the specification says; so does every function whose
   list makes no closure and whose body never mentions `eval` (the fast path).
2. **The two V15 refusals are answered.** A direct eval in the body of a function whose parameter
   list mentions `eval`, and a parameter-list eval in a function whose body redeclares a parameter,
   now evaluate per specification. The eval scope map has a new row kind
   `EvalScopeKind.FunctionBody = 7`, the variable environment of a body site; the verifier admits it
   only as the outermost record a function unit pushes.
3. **`super` in evaluated source** is answered: the eval frame already runs with its caller's active
   function, so the lowering simply stopped writing `EvalRefusal.SuperReference`.
4. **Private names in evaluated source** are answered through a bounded extension of the map: the
   map now writes a class's private-name slots (`##name`, which no identifier can spell); eval code
   reads an undeclared private name with `LoadEvalName`, and the declaration row lists those names in
   a new fifth list, `PrivateNameConstants`. Before instantiation the executor resolves each through
   the caller's map and throws the `SyntaxError` of `AllPrivateIdentifiersValid` when no enclosing
   class declares it; a global evaluation listing any throws it too. That check reads the map's
  declarative rows only: it passes over `with` rows without asking their objects, so the early
  error runs no guest code (see "Review fix" below).

Format and surface: no opcode, no diagnostic code, no decision number and no product file were
added. The public surface grew by `JsFormat.EvalScopeKind.FunctionBody` and
`JsEvalDeclarationRow.PrivateNameConstants` (recorded in
`src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`); the declaration row's encoding changed,
so eval-code artifacts written before this change must be compiled again (none is retained).
`EvalRefusal.SuperReference` and `PrivateName` stay defined and are no longer written. Removed:
the V15 refusal machinery (`RefusesBodyEvalSites`, `RefusesParameterEvalSites`,
`BodyRedeclaresParameter` and two walks only it used).

## Regressions, written first

- New probe `src/tests/differential/the-parameter-environment.js` (66 cases). On the base binary
  (answers retained in `probe-before.txt`) 39 of the first 61 cases answered otherwise: 21 silently
  wrong (a parameter-list closure seeing the body's `var`, function or assignment; a body eval's
  `var` of a parameter name reusing the parameter) and 18 with the V15 `EvalError` refusal. Cases
  62-66 were added after a self-review found that a list with no expressions must keep one record;
  the base answers 62-64 as the specification does and the change keeps them. After the change every
  case agrees with Node except four declared divergences, each settled by the pinned text or Test262:
  43 (`var arguments` in a parameter-list eval collides with the arguments object whatever the body
  declares, `eval-code/direct/func-decl-fn-body-cntns-arguments-func-decl-declare-arguments.js`) and
  62-64 (no parameter expressions, one environment: ES2026 FunctionDeclarationInstantiation, "If
  hasParameterExpressions is false ... Let varEnv be env").
- New probe `src/tests/differential/the-eval-super-and-private-names.js` (40 cases). On the base
  binary 30 of the first 37 cases answered the `EvalError` refusal; after the change all 40 agree
  with Node.
- Updated retained answers, for the right reason: `the-eval-declarations` cases 88-94, 98 and 99
  (the V15 refusals) now answer as Node does and their nine declarations were removed;
  `the-direct-eval-boundary` cases 46 and 47 (`super` and a private name) likewise, two declarations
  removed.
- Corpus: `eval-scopes-a-function-body-row-in-a-program-body` (a `FunctionBody` row in a script body
  is refused, 1627) and `eval-scopes-a-function-body-site-the-lowering-wrote` (a body site the
  lowering wrote verifies and answers `3`). A fresh `SliceCompiler --write` matches every retained
  entry byte for byte.

## Review fix: the private-name check asked a `with` object

An adversarial review found that the first version resolved an undeclared private name with the
ordinary eval name walk, which continues past the enclosing classes into a `with` object around them:
it called `HasProperty` with the internal spelling `##y` and read `Symbol.unscopables`, so a proxy's
`has` trap ran during what is an early error and could replace the `SyntaxError` with its own
exception. The check now uses a dedicated walk (`EvalPrivateNameDeclared`) that reads only the
declarative rows' slots, passes over `with` rows and never consults evaluation-introduced names.
Cases 38-40 of `the-eval-super-and-private-names` cover it (a throwing `has` trap, a logging trap, and
a declared name under the same `with`). On the build before the fix (retained in
`probe-withproxy-before.txt`) case 38 answered `RangeError` and case 39 logged `##y`; after the fix
they answer `SyntaxError` and `SyntaxError:eval|__log`, as Node does. The Test262 selection was run
again on the fixed build with the same totals (see below).

## What ran

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 BROILER_API_WRITE=1 dotnet
  test Broiler.VM.slnx -c Release --no-build`; build again; `dotnet test Broiler.VM.slnx -c Release
  --no-build`: Contract 267/267, Architecture 256/256 before and after, and again after the review
  fix (with the assurance write pass; every gate below was also rerun after the review fix with the
  same results).
- `SliceCompiler.exe --checks`: 327 checks pass and 2 are not run on this machine, before and after
  (the two new corpus entries are replayed inside its corpus-replay check, which the verbose run
  shows reaching both). `ExecutionOnly.exe --corpus src/tests/corpus/js-1`: 22 checks pass.
- CLI acceptance `python eng/run-cli-acceptance.py`: 219/219 before and after. `Cli.exe
  --host-surface`: every check passes before and after.
- Differential lane `python eng/run-differential.py --timeout 60`: 42 probes, only
  `the-later-library-methods` failing (the known code page 850 case); the whole lane was not rerun on
  the base binary, whose known failure is the same one. Each new or updated
  probe with `--only <name> --against node --timeout 20`: declared divergences only.
- Performance (the fast path): a 400000-iteration call loop (`add(s, i)` with a simple list, with a
  default `b = 1`, and with a closure default `g = () => a`), nine interleaved runs each, loop time
  read with `Date.now()` in the script. Minimum milliseconds, base then after: simple 651 / 627,
  default 882 / 741, closure 1090 / 1067 in a first run (an intermediate build with the body record
  and without the later `super`, private-name and no-expression changes); 798 / 803, 856 / 818 and 1648 / 1631 in a
  second run beside a Test262 run. The spread between runs of one binary is larger than any
  difference, so no regression is visible and no speedup is claimed.

## Test262 (bytecode form, one shard, a named selection, not retained as floors)

```
python eng/run-test262.py --suite D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e --binary-directory <base or after conformance host> --dir test/language/eval-code --dir test/language/function-code --dir test/language/arguments-object --dir test/language/expressions/class/elements --dir test/language/statements/class/elements --dir test/annexB/language/eval-code --dir test/language/statements/function --dir test/language/expressions/function --dir test/language/expressions/arrow-function --dir test/language/expressions/object/method-definition --dir test/language/expressions/super --dir test/language/statements/class/super --shards 1 --jobs 1 --digest-cache D:/test262-pinned/digest.cache --out <scratch>
```

5721 files, 10225 variants, 4 unsupported and 5 skipped in both runs. The pass count moves from 9975
to 10087; 112 variants move from fail to pass and none from pass to fail (list in
`test262-fixed.txt`). The after run was repeated on the build with the review fix: the same totals
and the same 129 failing variants.

| Area (failing variants) | Before | After |
|---|---|---|
| `language/eval-code/direct` | 34 | 2 |
| `language/expressions/class` | 70 | 46 |
| `language/statements/class` | 102 | 54 |
| `language/expressions/super` | 14 | 6 |
| every other area of the selection | 21 | 21 |

The 112: the 24 `*-fn-body-cntns-arguments-func-decl-declare-arguments*` variants V15's review
turned into refusals, `super-prop-method.js`, the `*-direct-eval-contains-superproperty-*` class
element variants, the `super/prop-*-from-eval.js` variants and the 24
`private-*-visible-to-direct-eval*` variants. What still fails in the selection is not this slice's:
module-scoped eval (`eval-code/direct/export.js`, `import.js`, refused by name), `$262.evalScript`
and `$262.createRealm`, the class early errors reported at run time, computed-key coercion order,
private-method double initialisation and the other pre-existing rows.

## Not exercised

- The native form through the end-user host and the conformance harness (native artifacts fail to
  instantiate on this machine, `ProfileFault/UnsatisfiedHostAssumption`, pre-existing); the check
  host's in-process native rows pass as before, and no new opcode needs a baseline handler.
- No Linux run, no Broiler.JS comparison, no JSeal consumer run. JSeal's provider copies the
  profile's capability version; the declaration-row encoding changed inside the same capability
  version, so a JSeal pin that takes this change must take the matching compiler with it (it
  compiles through `JsCompiler` already).
- Test262 subtrees outside the selection above were not run for this slice.
