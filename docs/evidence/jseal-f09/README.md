# Unicode property escapes under u, and the u-mode Canonicalize on CaseFolding.txt: JSeal slice F09

Date: 2026-09-22. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence** and not a conformance
score. It implements slice U4 of decision
[JSD-0031](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md)
(still proposed; its new section 11 records this slice) on the U2 tables. No new decision number,
opcode (`JsOpcode`) or diagnostic code was taken, no product file was added, and no public API moved.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-6 patch (`base-vm-w7`)
and the U2 patch (`vm-unicode-u2`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0
(Unicode 17.0) was a comparison engine only; the pinned specification and Test262 `ccaac100` are
the oracle.

## What changed

- **Property escapes** (`JsRegExpMatcher.cs`, parser): under `u`, `\p{...}` and `\P{...}` are class
  escapes in atom and class position. `ReadPropertyEscape` reads the braces
  (`UnicodePropertyValueCharacters`, optionally `name=value`) and resolves them through
  `JsUnicodeProperties.TryResolveLone` / `TryResolve`, by exact ordinal name only. Anything else -
  `\p{letter}`, `\p{Script_Extensions=greek}`, `\p{script_extensions=Greek}`, `\p{Scx=Greek}`,
  `\p{scx:Greek}`, `\p{ASCII=Y}`, `\p{Block=...}`, `\p{WSpace}`, a property of strings, `\pL`, a
  missing brace, an empty name - is a `SyntaxError` ("Invalid property name"), early for a literal
  because the front end reads the same parser. `[\p{L}-z]` stays a `SyntaxError` under `u`.
  Outside `u`, `\p` is still the Annex B identity escape.
- **Classes hold properties by reference** (`JsRegExpCharSet.AddProperty`, `Weight`): building a
  class costs one entry per escape whatever the property's width, so there is no per-class
  construction proportional to a table to charge. The per-character cost is charged instead: a
  class holding property escapes adds one step per referenced property per character tested (five
  under `i`: the character and up to four case variants), and the prefilter scan hands each skip to
  the meter as it goes rather than after the scan.
- **One Canonicalize under `u`** (`JsRegExpCase`): `Canonicalize(c, unicode: true)` is
  `JsUnicodeCaseFolding.SimpleFold` (CaseFolding.txt statuses C and S) and the `u` closure is
  `JsUnicodeCaseFolding.Orbit`, for literals, classes, back-references, `\w`/`\b` under `iu` and
  property escapes alike, supplementary planes included. The `lower(upper(c))` approximation, its
  U+0130/U+0131/U+017F hand exceptions and the astral-pair reconstruction are removed. The non-`u`
  path is unchanged (platform, Unicode 16 data), as JSD-0031 section 7 decided.
- **Greedy runs** (`Op.Run`, an internal matcher instruction, not a `JsOpcode`): a greedy
  quantifier over one class consumes its characters in one instruction and leaves one backtrack
  point that gives them back a character (a code point under `u`) at a time. The generated tests
  match `^\p{X}+$` over up to the whole code space; the loop form pushed a frame per character and
  cannot hold 1,114,112 frames under `MaximumFrames` (2^20). Backtracking order is the loop's; every
  character read is a metered step (the loop charged about five instructions per character, a run
  charges one). A side effect recorded in [probe-and-checks.txt](probe-and-checks.txt):
  `/^[a-z]+$/.test("a".repeat(2000000))` answered AllowanceExhausted on the base build and `true`
  now.
- **Documentation of what the matcher lacks**: `JsRegExpMatcher`'s and `JsRegExpCase`'s remarks
  (and `JsUnicodeCaseFolding`'s) now say what is and is not supported: `u`-mode property escapes by
  exact name; no `v` flag, set operations or properties of strings; non-`u` folding still Unicode 16.
- **Probes**: new `src/tests/differential/the-unicode-property-escapes.js` (74 cases; retained
  answers written after checking each against Node and the specification; three declared
  divergences: `Hrkt` admitted by the specification and refused by V8, `WSpace` refused by the
  specification's table and accepted by V8, and the `v` flag this realm refuses).
  `the-json-date-and-regexp-surface` case 98 (`\p{L}` under `u` against U+00E9) now answers
  `"prop"` like Node, so its stale `#diverges node 98` line was removed and its retained answer
  updated.
- **JSD-0031** section 11 and the decisions index row record U4.

## Acceptance (JSD-0031 U4)

- **The 441 generated and 144 grammar Test262 property-escape files pass under the harness's
  limits** (100,000,000 fuel, 5,000 ms, per variant): 1,170 of 1,170 variants, from 284 on the base.
  A single generated file takes about 1.5 s through the end-user host.
- **Misspelled names and aliases are early SyntaxErrors, by exact resolution**: probe cases 34-56
  and every negative grammar file.
- **`/\p{Lu}/iu.test('a')` is `true`** (probe case 27), and so is
  `new RegExp('\\p{Letter}', 'u').test('a')`, the F09 card's example (case 1).
- **All u-mode Canonicalize and Variants read CaseFolding.txt**: for the 28 Unicode 17.0 pairs,
  `/\p{Lu}/iu`, `/X/iu` and `/[X]/iu` all match the lower-case partner (case 62: 28 pairs, none
  missed), and over all 1,414 `\p{Lu}` code points whose lower case is one code point, none is
  missed in any of the three forms (cases 64-66). On the base build the literal and class forms
  missed exactly those 28 (probe-and-checks.txt). Test262 `unicode_full_case_folding.js` now passes.
- **The matcher's own documentation is updated** in the same change (above).
- **`v` and properties of strings are not advertised**: `new RegExp('[\\p{L}]', 'v')` is a
  SyntaxError for the flag, and `\p{RGI_Emoji}` and `\p{Basic_Emoji}` under `u` are SyntaxErrors by
  name (cases 58-61). The 7 positive `generated/strings` files (14 variants) still fail by that
  refusal; the 21 negative ones pass because the refusal is also the specification's answer under
  `u`, which is not evidence of `v` support.
- **Metering, cancellation, compile time and memory**: a class of 60,000 escapes (560,002
  characters of source) compiles in about 42 ms and 50,000 lone escapes in about 27 ms; a class of
  60,000 `\p{Lu}` scanned over 100,000 characters ends as `AllowanceExhausted on Fuel` in about a
  second instead of running six billion lookups. The matcher fuzzer (`--fuzz-regexp`, seed 1, 20,000
  iterations, `\p{L}` among its seeds) found no counterexample.

## Commands and results

- `dotnet build Broiler.VM.slnx -c Release`, then `dotnet test Broiler.VM.slnx -c Release --no-build`:
  on the base, Architecture failed 5 tests until the assurance artefacts the patches exclude were
  regenerated with `BROILER_ASSURANCE_WRITE=1`, after which the base was Contract 267/267,
  Architecture 266/266. After this change, the write run, a rebuild, and the gate run: Contract
  267/267, Architecture 266/266. The product-file count stays 199.
- **Failing first**: before the matcher changed, the new probe was refused whole at parse on the
  base build ("Unicode property escapes are not supported by this matcher"), and the literal and
  class forms of the `\p{Lu}` pair walk missed 28 of 1,414 (probe-and-checks.txt).
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks`: 371 checks and 2 not-run rows,
  before and after. `python eng/run-cli-acceptance.py`: 225/225 before and after.
  `Broiler.VM.Composition.JavaScript.ExecutionOnly.exe --corpus src/tests/corpus/js-1`: 22 checks
  before and after; the JavaScript lowering did not change and the corpus was not regenerated.
  `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passed, 73 rows before and
  after.
- `python eng/run-differential.py --only the-unicode-property-escapes --against node --timeout 20`:
  three declared divergences, no finding. The whole differential lane against the retained answers:
  only `the-json-date-and-regexp-surface` case 98 (updated, above) and the known code-page case 36
  of `the-later-library-methods` (cube root, pre-existing) differed.
- **Native AOT**: `dotnet publish src/compositions/Broiler.VM.Composition.JavaScript.Cli -c Release
  -r win-x64 -p:PublishAot=true` (Visual Studio installer directory on `PATH` for `vswhere`, as the
  U2 bundle did): exit 0, no warning in the output, and the published image answers all 74 cases of
  the new probe exactly as the retained answers do.

## Test262 (pinned, bytecode form, `--shards 1 --jobs 1`)

`test/built-ins/RegExp` and `test/language/literals/regexp`, 2,117 files and 4,219 variants each
time: the pass count moves from 2,760 to 3,648, with 8 exhausted (`NestedLoadFanOut` in
`literals/regexp`, pre-existing) and 13 skipped both times; no variant that passed before fails
after. The 888 variants that moved are 882 generated and 4 grammar property-escape variants and
the 2 of `unicode_full_case_folding.js`. The run was repeated on the final build, after the
prefilter's charge under `i` was made the same as a class test's: every variant's verdict was
identical. Per area: [test262-summary.txt](test262-summary.txt). A
separate run of `test/built-ins/RegExp/property-escapes` alone: 613 files and 1,226 variants, all
but the 14 positive properties-of-strings variants passing.

## `Broiler.Unicode`

The owner asked whether the `Broiler.Unicode` component, which Broiler.JS uses with Unicode 17
data, could serve F08/F09. JSD-0031 section 9.3 answered it for the data and it still holds here:
rules A2 and N1 forbid the reference, its name lookup is loose (it would accept `\p{letter}`, which
this slice must refuse), and it has no case-folding table; U2 cross-checked the pinned tables
against its data (946 of 950 set comparisons agree, none differs). The matcher reads only the
pinned tables.

## Not exercised, and not done

- The native execution form: not exercised (it cannot instantiate on this machine, pre-existing).
- The Android composition was not built.
- Not in scope and unchanged: the `v` flag and properties of strings; the non-`u` Canonicalize
  (U+A7CE under `i` without `u` still misses U+A7CF); `ID_Start`/`ID_Continue` for group names;
  `normalize` (U3/F08, another slice). A regular-expression literal with the `v` flag is still
  refused at run time rather than early (pre-existing).
- Found and not fixed (pre-existing, not this slice): with `u` and `lastIndex` inside a surrogate
  pair, `exec` starts at the trailing unit; the specification starts at the pair's code point, and
  Node answers accordingly.
