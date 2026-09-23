# Source names and line accounting: JSeal slice J17 (upstream half)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It is the VM half of JSeal J17. JSeal has not adopted it, and J17 stays open until it does.

## What changed

- **(a) Source name.** `JsScriptUnit.SourceName` is a new init-only member, empty by default. Every
  diagnostic a named unit is refused with carries it as `SliceSourceDiagnostic.SourceName`, and
  `ToString()` reads `code at name:line:column`. An unnamed unit prints what it did before. The name
  reaches no artifact byte and no compilation decision. Both members are additive, so the
  constructor and `Deconstruct` baseline entries are unchanged. The API baseline gains the two
  properties, and JSD-0014 gains a dated addendum.
- **(b) Template continuations.** The tokenizer skipped a backslash and the character after it by
  index. The terminator of a continuation inside a template, a `String.raw` template, or a string
  inside a substitution was therefore never counted, and every later diagnostic named the line
  above. CRLF was counted correctly only because its LF was left over. The new `SkipEscape` counts
  the terminator. A backslash before a terminator inside a regular expression in a substitution
  ends that literal, and the parser's `TemplateReader` scan matches the tokenizer.
- **(c) U+2028/U+2029 in strings.** A string literal ended at U+2028 or U+2029 and was refused as
  unterminated. Only LF and CR end one now. A separator is part of the value and still advances the
  line count, as Node counts it. The substitution scans in the tokenizer and in `TemplateReader`
  agree.
- **Found while verifying (c): directive completion value.** The directive prologue was held apart
  from the program body, so `(0, eval)("'a'")` answered `undefined`. Test262's `*-separator-eval.js`
  cases check the separator through exactly that. `CompileProgram` now records the last directive's
  string as the initial completion value. This is a separate defect, fixed here because the named
  Test262 targets cannot pass without it. It can be split into its own change.

## Regressions written first, and what they showed before the fix

- SliceCompiler front-end check `every line terminator inside a literal is counted once, and a
  separator is a string character` (17 positions, 3 admissions). Before: `1 of 257 checks FAILED`.
  Every template continuation, LF/CR/LS/PS, raw, nested, and string-in-substitution case answered
  `2:9` for an error on line 3. Every separator-in-string case was refused as `2004`. Only the
  CRLF case, the string continuation case, the plain template case and the two "a string still ends
  at LF/CR" controls held.
- CLI acceptance rows (`src/tests/cli/expected.txt`, J17 block): three `refused/` files that expect
  `2101:UnexpectedToken at 4:9`, and `runs/a-line-separator-in-a-string.js`, which expects
  `5 2028 2029 4 2`. The directory sweep row moves from `# Completed: 88` to `89` for the new
  file. Before: `5 of 213 command lines FAILED`. The template rows answered `at 3:9`, the separator
  rows answered `2004:UnterminatedStringLiteral`, and the sweep failed on the new file. Node reports
  line 4 for all three refused files and prints the same `runs/` value.
- Differential probe `src/tests/differential/the-line-terminators-in-literals.js` (28 cases). Before:
  the host refused the whole file (`2004:UnterminatedStringLiteral at 35:30`). Run without its
  literal-separator lines, cases 1-5, 13 and 15 differed from Node: separators refused inside
  evaluated strings, and a string-only program answered `undefined`.
- The `SourceName` check (`a source name reaches its unit's diagnostics and nothing else`) needs the
  new API and could not be run before the API existed.

## After

- `dotnet build Broiler.VM.slnx -c Release`, then `BROILER_API_WRITE=1` for the N10 baseline, then
  `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build`, rebuild, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256. The
  base read the same.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks`: the pass count moves from 256 on
  the base to 258 (two new checks). The same 2 x86-64-sysv native rows are not run on this machine.
- `python eng/run-cli-acceptance.py --binary-directory <Cli bin/Release/net10.0>`: 209/209 on the
  base, 213/213 after.
- `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passed, before and after.
- `python eng/run-differential.py --only the-line-terminators-in-literals --against node --timeout
  20`: all 28 cases agree with Node v24.17.0, and the retained answers were written with `--write`
  after that comparison. In the full retained lane (`--timeout 30`, code page 850), only the known
  non-ASCII console cases fail: `the-json-date-and-regexp-surface` 158-159 and
  `the-later-library-methods`.
- Pinned Test262 (`ccaac100`), bytecode, `--shards 1 --jobs 1`. Per-variant lists are in
  [test262-comparison.txt](test262-comparison.txt).

  | Selection | Variants | Passing before | Passing after | Passed -> Failed |
  |---|---|---|---|---|
  | `literals/string`, `expressions/template-literal`, `built-ins/String/raw`, `expressions/tagged-template`, `line-terminators` | 432 | 344 | 354 | none |
  | wider regression set (directive prologue, comments, white space, all literals, indirect eval, `eval`, `JSON.parse`, six statement directories) | 3356 | 3047 | 3093 | none |

  The target gains are `line-separator.js` and `paragraph-separator.js` from (c), plus their
  `-eval` twins and `mongolian-vowel-separator-eval.js`, which also needed the completion value.
  The wider set gains those 10 variants. Its other 36 are string-only `eval` programs that needed
  the completion value: 24 regexp-literal `S7.8.5_A1.5`/`A2.5` variants, 10 `white-space/string-*`
  variants, and `eval-code/indirect/cptn-nrml-expr-prim.js`. The json-superset tests ran under
  this harness and were not skipped.

## Not exercised or still failing

- The native form was not run. It cannot instantiate on this machine
  (`ProfileFault/UnsatisfiedHostAssumption`, pre-existing).
- Line positions have no Test262 oracle. They are checked against the specification's
  LineTerminator set and against Node's reported lines.
- Still failing in the target selection, all outside this slice: 42 strict-mode variants throw
  `ReferenceError: #template@...` because a tagged-template site's cache store is an assignment
  to an undeclared global in strict code. There are 31 legacy-octal and `\8`/`\9` early errors
  raised at run time, `invalid-escape-sequences.js` (a tagged template's invalid escapes are
  refused instead of cooking to `undefined`), and `cache-realm.js` (no `$262.createRealm`).
- Two evaluated programs that put a tagged template at the same line and column share one template
  object, because the site key is `#template@<script ordinal>:<line>:<column>` and every `eval` is
  script ordinal 1. Node answers `a b c` for `ev("String.raw\`a\`"), ev("String.raw\`b\`"),
  ev(" String.raw\`c\`")` and this build answers `a a c`. The probe gives each `String.raw` case
  its own column so that this defect does not decide its answers.
- JSeal adoption, including a package with a pinned version, is not part of this change.
