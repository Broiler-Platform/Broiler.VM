# Seven small correctness gaps: JSeal follow-up VM-FIX-H

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is **local implementation validation, not accepted milestone evidence** or a conformance score.
Nothing here marks a JSP/JSW stage or a milestone complete.

Checkout: a detached worktree of Broiler.VM at `HEAD` plus the merged wave 1-4 patch
(`base-vm-w5`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was used only as a
diagnostic comparison. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`. The
"before" figures come from a second worktree at the same base, built the same way.

## What changed, one gap at a time

1. **`for await` and a forced return** (`JsCompiler`, lowering only). An async generator's
   `return()` landing at a `yield` in a `for await` body left without closing the loop's iterator.
   The loop now records a `finally`-kind region as the synchronous `for … of` already did; its
   handler parks the forced return in a slot, closes with `IterateCloseAsync`/`Await`/
   `IterateCloseCheck` (the loud close: a rejection or a primitive answer replaces the return), and
   re-raises the parked return. Probe cases 1-8; before, cases 1-4 and 6-8 never called `return`.
2. **`$262.IsHTMLDDA`** (`JsObject.IsHtmlDda`, `JsNativeFunction.EmulatesUndefined`,
   `JsValue.ToBooleanValue`/`TypeOf`, `JsEngine.LooselyEquals`, public
   `JsHostRealm.NewHtmlDdaObject()`, `Test262Host`). Only a host can make such an object; the
   conformance harness installs one as `$262.IsHTMLDDA`, the end-user host installs none. `??`,
   `??=` and optional chaining had been lowered as `LoadNull; LooseEquals`, which the slot turns
   into the wrong question, so they now emit two strict comparisons and a strict comparison of the
   answers (no opcode added). Recorded in JSD-0024 section 18.
3. **`using await` in a static block** (`JsParser`). `using` followed on the same line by `await` or
   `yield` is read as a declaration, so the binding list refuses the name with
   `2209:ReservedWordAsBinding`, as it does `let await`; before it was `2102:ExpectedToken`. New CLI
   row `refused/a-using-await-in-a-static-block.js`.
4. **RegExp group names by code point** (`JsRegExpMatcher`). Names are decoded by code point, with
   `\u` escapes (four digits, braced, or an escaped surrogate pair) in either mode, and tested
   against ID_Start/ID_Continue using the platform's categories plus the Other_ID_* lists. `\k<…>`
   is decoded the same way. Probe cases 12-22.
5. **`OfferModuleRequest` settles a request when the loader throws** (`JsHostRealm`). New
   host-surface check: a kept request then answers `AlreadyResolved`; before, `complete=Accepted`.
6. **`DetachClone` refuses `JsHostValue.Missing`** as the value or a transfer entry with
   `ArgumentException`, before anything is detached. New host-surface lines; before, `not refused`.
7. **`Test262Host` doc comments**: `evalScript`'s summary no longer sits inside `Detach`'s; each
   member has its own.

No opcode, diagnostic code, decision number or product file was added. One public member was added
(`NewHtmlDdaObject`; `docs/api/public-api.txt` regenerated). No global was added for ordinary
guests; rule N17 and `globals.txt` are untouched. The retained corpus regenerates byte-identical
(`SliceCompiler.exe --write src/tests/corpus/js-1` left no difference).

## Executed on Windows (win-x64, Release, bytecode form)

- **Gates.** Base: `dotnet build Broiler.VM.slnx -c Release`, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  After: build, `BROILER_ASSURANCE_WRITE=1 dotnet test …`, build, gate-mode test: Contract
  267/267, Architecture 256/256.
- **Slice-compiler checks** (`… SliceCompiler.exe --checks`): 353 pass and 2 are not run on this
  machine (x86-64-sysv), before and after. **Corpus replay** (`… ExecutionOnly.exe --corpus
  src/tests/corpus/js-1`): 22 checks, before and after.
- **CLI acceptance** (`python eng/run-cli-acceptance.py --binary-directory …`): 224 of 224 before,
  225 of 225 after (the new refused row).
- **Host-surface lane** (`… Cli.exe --host-surface`): every check passed, before and after; the
  lane gained two checks, and one existing check gained two lines.
  [checks-before.txt](checks-before.txt) holds the new checks failing on the base engine, and the `IsHTMLDDA` check failing before the `??` lowering was changed.
- **Differential.** New probe `the-small-correctness-gaps.js` (22 cases);
  `python eng/run-differential.py --only the-small-correctness-gaps --against node --timeout 20`
  agrees with Node on every case. [probe-before.txt](probe-before.txt) is the base build. The full
  retained lane (`python eng/run-differential.py --timeout 20`) differs only in
  `the-later-library-methods` case 36 (cube root), which is pre-existing.
- **Test262, focused selection** (`python eng/run-test262.py --suite … --dir …  --shards 1 --jobs 1
  --digest-cache …`, directories listed in [test262-comparison.txt](test262-comparison.txt)): the
  pass count moves from 6460 to 6514 of 6929 variants, with no variant newly failing or newly
  exhausted. The 54 newly passing variants are 50 annexB `emulates-undefined` variants (including
  `TypedArrayConstructors/from/iterator-method-emulates-undefined.js`) and
  `RegExp/named-groups/non-unicode-property-names.js` and
  `RegExp/prototype/Symbol.replace/named-groups.js` in both modes.

## Not exercised, or known wrong

- **`using` tests are skipped by the harness**: the pinned `features.txt` lists
  `explicit-resource-management` as a proposal. `statements/using/static-init-await-binding-invalid.js`
  and `-valid.js` were run with the CLI and `assert.js`/`sta.js` prepended (in
  [checks-before.txt](checks-before.txt)); both behave correctly before and after, and the invalid
  one now gives `2209`.
- **Three `RegExp/named-groups` files still fail** (`unicode-property-names.js`,
  `unicode-property-names-valid.js`, `non-unicode-property-names-valid.js`, both modes): the
  source tokenizer refuses an identifier character outside the basic plane (`groups.𝒜`), which is
  the source lexer, not the pattern parser.
- `test/staging/sm` `IsHTMLDDA` tests were not run. The native execution form was not exercised
  (it cannot instantiate on this machine).
