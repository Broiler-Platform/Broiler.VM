# using and await using declarations: JSeal slices F21, F22

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is **local implementation validation, not accepted milestone evidence** or a conformance score.
It implements the syntax half of explicit resource management. It also implements the proposed,
unsigned decision record
[JSD-0034](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0034-admitting-explicit-resource-management-ahead-of-the-edition.md),
which admits the feature ahead of the pinned ES2026 edition, against
`tc39/proposal-explicit-resource-management@38c13295`. Nobody has approved that record.

Checkout: a detached worktree at `484f389` of Broiler.VM plus the merged wave-1/2 patch. Release
build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was used only as a diagnostic comparison.
Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed

- **Decision and harness.** JSD-0034 records the decision. `SuiteFeatures.AdmittedProposals`
  names `explicit-resource-management` beside `JSD-0034`. The `--test262` command therefore scores
  tests with that flag and still skips every other proposed flag, and its `features` line names the
  admission. The ingested-dialect command (`--suite`) and its fixture suite are unchanged.
- **F21 grammar and static semantics** (`JsParser`). `using x = …` is accepted in blocks (including
  `catch` and `finally` bodies), function and static-block bodies, modules, and `for`/`for-of` heads. Early errors cover:
  - the top level of a script or an eval;
  - a declaration directly in a case clause;
  - a declaration in a single-statement or labelled position;
  - a `for-in` head, a binding pattern, or a missing initialiser;
  - a binding of `let`, a duplicate lexical name, and `export using`.

  `using` stays an identifier in every other reading, including `using[x]` and a line break before
  the name. `for (using of of xs)` keeps the lookahead reading. An escaped `using` is not the
  keyword.
- **F21 lowering** (`JsCompiler`). A statement list that declares a resource keeps a disposal
  scope in a hidden `#dispose` slot and opens a `finally`-kind region around its statements.
  `break`, `continue` and `return` dispose inline through a new exit kind. The handler folds a
  throw or a generator's forced return and re-raises. A `catch` body opens its own scope inside the
  parameter's scope, so its resources are disposed before control leaves the handler. A `for-of`
  head gets one scope per turn,
  disposed before the iterator is closed. A counted `for` head gets one scope for the whole loop.
  Disposal errors are reported at the position of the declaration that opened the scope.
- **Format, verifier and executor.** Five instructions were added: `DisposeScope` `0xA0`,
  `DisposeAdd` `0xA1` (u8 hint), `DisposeFold` `0xA2`, `DisposeStep` `0xA3` (u32; two-effect branch,
  target one below) and `DisposeEnd` `0xA4` (u8 mode). The verifier refuses `DisposeStep` outside an
  async unit with the existing code 1611 `AwaitOutsideAsync`. It refuses a hint or mode above 1
  with 1401 `UnknownOpcode`. No new diagnostic code was taken. The baseline native form got five
  entry points, two of them run-alone steps. The partition classes are P, R, P, T E and E.
  Three corpus entries were added to `WideCorpus`:
  - `wide-a-disposal-step-outside-an-async-function` (1611);
  - `wide-a-disposal-end-whose-mode-is-undefined` (1401);
  - the control `wide-a-block-disposes-its-resource`, which completes with `20`.

  The profile API baseline gained the five `JsOpcode` members (additions only). No product file was
  added, so rule H3's count stays 191. No global changed, so rule N17 and `globals.txt` are
  untouched.
- **F22** (`JsRealm.Disposal.cs`). `DisposeStep` runs entries until one owes an await. The
  lowering awaits with the ordinary `Await`, whose rejection is folded by a catch region. The
  `needsAwait`/`hasAwaited` rules of `DisposeResources` hold, and the turn counts match Node (probe
  case 41). `await using` at a module's top level marks the module body async, the same way a
  top-level `await` does. The method lookup is shared with the stacks through `DisposalMethod`.
- **Budget policy** (JSD-0034). Registration charges one fuel unit and 64 live bytes. Unwinding
  charges one fuel unit per entry. Only a guest throw is folded. Checked by hand: when a disposer
  spends the allowance (`--fuel 200000`), the run ends `AllowanceExhausted on Fuel` and the outer
  disposer does not run. This held for both the sync and the async form.

## Executed on Windows (win-x64, Release, bytecode form)

- Gate:
  1. Base: `dotnet build Broiler.VM.slnx -c Release`, then
     `dotnet test Broiler.VM.slnx -c Release --no-build`. Result: Contract 267/267,
     Architecture 256/256.
  2. After: build, then `BROILER_API_WRITE=1 BROILER_ASSURANCE_WRITE=1 dotnet test …`, then build,
     then the gate-mode test. Result: Contract 267/267, Architecture 256/256.

  `Broiler.VM.Composition.JavaScript.SliceCompiler --checks` reported 282 checks passed and 2 not
  run on this machine (x86-64-sysv).
- **Test262, explicit-resource-management and neighbours** (`--dir` for `statements/using`,
  `statements/await-using`, `for-of`, `for-await-of`, `built-ins/{DisposableStack,
  AsyncDisposableStack, SuppressedError, Symbol/dispose, Symbol/asyncDispose, AsyncIteratorPrototype,
  Iterator/prototype/Symbol.dispose}` and `staging/explicit-resource-management`; 2454 files, 4802
  variants).
  - "Before" is the base engine with only the harness admission applied. It was built from the
    exported base tree, because without the admission every flagged file is skipped.
  - The pass count moves from 4471 to 4741. No variant that passed before fails after, except the
    two below.
  - Remaining failures and unsupported variants are listed in
    [remaining-failures.txt](remaining-failures.txt).

  | Directory | Variants | Before | After |
  |---|---|---|---|
  | `test/language/statements/using` | 152 | 60 | 150 |
  | `test/language/statements/await-using` | 184 | 56 | 184 |
  | `test/staging/explicit-resource-management` | 105 | 38 | 83 |
  | `test/language/statements/for-of` | 1442 | 1414 | 1421 |
  | `test/language/statements/for-await-of` | 2431 | 2431 | 2431 |
  | `test/built-ins/DisposableStack` | 186 | 182 | 182 |
  | `test/built-ins/AsyncDisposableStack` | 208 | 204 | 204 |
  | `test/built-ins/SuppressedError` | 44 | 42 | 42 |
  | `test/built-ins/Symbol` | 12 | 8 | 8 |
  | `test/built-ins/AsyncIteratorPrototype` | 26 | 24 | 24 |
  | `test/built-ins/Iterator` | 12 | 12 | 12 |

  - The "before" syntax passes are the refusals JSD-0018 describes: negative tests answered by a
    front end with no production at all.
  - `using/static-init-await-binding-invalid.js` (2 variants) passed before for that reason and fails
    now. `using await = null` inside a class static block should be a SyntaxError. The parser does
    not reserve `await` as a binding name in static blocks for any declaration (`let await` is
    accepted too); that gap is pre-existing.
  - Of the 22 remaining staging variants, 20 declare a function inside `asyncTest` that reads the
    enclosing function's `let`. On the base build such a nested function declaration resolves the
    name as a global (`ReferenceError`). That defect is pre-existing and independent of `using`.
    The same shapes rewritten with `var` or a function expression agree with Node. The other 2 are
    `staging/explicit-resource-management/Symbol/dispose/cross-realm.js` (sloppy and strict), which
    need `$262.createRealm`; this profile creates no nested realm.
  - The other remaining variants need `$262.createRealm`, BigInt or resizable buffers, or are
    pre-existing `for-of` failures.
- **Neighbour subsets.** These cover `statements/{block, for, for-in, try, switch, labeled, let,
  const, async-function, async-generator, generators, variable}`, `expressions/await`, `module-code`,
  `identifiers`, `asi`, `global-code` and `eval-code` (3486 files, 5799 variants). Base versus this
  patch gave identical per-variant verdicts: pass 5129, fail 517, skipped 153 on both.
- **New differential probes.**
  - `the-using-declarations.js` has 54 cases and `the-using-declarations-in-a-module.mjs` has 2.
    Cases 45-54 (added after review) cover `using` and `await using` in a `catch` body, with and
    without an enclosing resource scope, with a destructured parameter, on `return` and on a
    throwing disposer, and in a `finally` body on the normal and the exceptional path.
  - The base build refuses both at parse (`ExpectedToken` at the first `using`).
  - After the change, `python eng/run-differential.py --only <name> --against node --timeout 20`
    exits 0 for each. One divergence is declared: case 29. A `SuppressedError` built by disposal
    has no own `message`, as the proposal says; V8 gives it one.
- **Full retained lane.** Only `the-later-library-methods` case 36 fails (cube root). The mismatch
  is pre-existing and outside this slice.

- **Review fix: resources in a `catch` body.** Before the fix, the handler body was compiled as a
  plain statement list. `try { throw 1; } catch (e) { using a = …; }` with no enclosing resource
  scope was refused at compile time (`2104:ConstructOutsideManifest`, the whole probe with cases
  45-54 refused), and with an enclosing scope the resource was disposed by that outer scope, too
  late. After the fix all ten new cases agree with Node. A re-run of the Test262 selection above
  (`erm-after2`) gives per-variant verdicts identical to the first "after" run, and
  `statements/try` (388 variants) is identical to the neighbour run. Gates after the fix and the
  assurance regeneration: Contract 267/267, Architecture 256/256; `--checks` unchanged.

## Not exercised, or known wrong

- **Unmet F21 acceptance ('abrupt exits pass'): `try … finally` inside a resource scope.** Consider a `return` or `break` that passes a
  `finally` and then a scope whose disposer throws. The finaliser runs twice (`fin,a,fin,caught a`;
  Node gives `fin,a,caught a`). This comes from the existing `finally` lowering, which inlines
  unwinding code inside the inner protected range. It is not new: on the base build, an iterator
  `return` that throws after a `finally` shows the same double run (`fin,fin,c`). It is not fixed
  here, so F21 is implemented with this acceptance item unmet.
- **Inherited static-semantics gaps.** `using await = null` in a class static block and, in strict
  code, `using static = null` (a strict reserved word) are accepted; Node refuses both. The same
  holds for `let`/`const`, so the gaps belong to the front end and are recorded in JSD-0034.
- **Forced return through `for-of`.** A generator's forced return through a `for (using x of …)`
  disposes the turn's resource. It does not close the iterator, because the loop's region is a
  catch region. That behaviour is pre-existing.
- **Not run or not claimed:**
  - The native execution form was not run (`ProfileFault/UnsatisfiedHostAssumption` on this host,
    pre-existing).
  - Linux was not run.
  - The Broiler.JS comparison was not run.
  - The slice manifest's front end still has no production for these declarations.
  - The numeric-manifest refusal is in code but was not exercised.
  - No JSeal module capability is claimed (card I13).
