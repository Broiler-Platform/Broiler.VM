<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-11.1's correspondence from code: regular expressions in both engines

**Written:** 2026-10-21, at SJL-11.1 (work package SJL-11.1), as the input of the area
verdict JSD-0081's X1, X2 and X6 for each of the area's six sub-areas (S syntax and early errors with
literal validation, M compilation and matching, B budget and limits, F flags, T text helpers, U data
use), and of the three `sjl-11-001` rules, before any code of SJL-11.2 exists. **Every behavioural cell
was run in both engines.** **Each row carries its class**, which the area verdict
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md)
decided on 2026-10-21, and no row's class is unknown. A class that rests on a reading of the
specification's text stands until `regexpref` (SJL-11.3's reference) checks that reading; a reading it refutes is
a finding against JSD-0081, and the row is reclassified there.

**Read and run at**
- **Broiler.VM `aaa39fb`** (the default branch after PR #156, the preview.7 re-pin). Every
  regular-expression source is byte-identical at today's `origin/main` `3fa4b39` but
  `CP/JsCompiler.Literals.cs`, whose two hunks are both in `CompileNamedValue`, outside the literal; the
  compiler's eval path changed around a literal compile that did not. At `daa92eb`, merged later the same
  day, the literal's check moved from the lowering to the binder, unchanged (section "Revisions" below).
- **Broiler.JS `1f91b8f1`** (the default branch after PR #1057). Its `u`/`v` path is **Broiler.Regex
  `0.1.0-preview.4`**, the package `Directory.Packages.props` restores, whose nuspec names commit
  `c1422f4d824aa28ff6edd4766d56be25ba7c8a15`; Broiler.RegEx's `origin/main` `0799342` holds the same
  `Broiler.Regex/` sources. At today's `origin/main` `792ff56e` the validators have moved to
  `Broiler.JavaScript.Ast` (SJL-12.5) with comments and a forwarder as their only changes, and the scanner
  changed outside the literal scan (section "Revisions").

**How each column was run.**
- **A Broiler.VM cell** was run from the Release build (`--disable-build-servers`) of a detached
  worktree at `aaa39fb`, through the conformance composition root, as a raw test262 test in a
  test262-shaped directory (`correspondence-runs/vm/vm262/`, with the pinned suite's `features.txt`;
  test262's `harness/` at `ccaac100` is copied in for the run and not retained), under the manifest
  `broiler.javascript.wide`, fuel 200,000,000,000 unless the probe's first line names an allowance, wall
  clock 600,000 ms, no `LANG`, `TZ=UTC` (`correspondence-runs/vm/run-vm.sh`). A probe whose program
  ended before it printed (an exhaustion or a stop) has one line, the root's verdict. Row B7's smallest
  completing allowances were found by `vm/bisect-vm-fuel.sh`, each step one process of the same command
  under another `--fuel`. Four cases were also run through the **command-line composition root**, the
  root that part D of this bundle's area rule (`decision-rule-area.md`) runs, to show how it
  reports what the conformance root reports (`vm/run-vm-cli.sh`, `vm/cli/`).
- **A Broiler.JS cell** was run through the shell, `BroilerJS.dll --script-host`, built in Release
  (`--disable-build-servers`) from a detached worktree at `1f91b8f1`, `TZ=UTC`, with no `LANG`, `LC_ALL`
  or `LANGUAGE` (the invariant run) and, for the probes holding case-insensitive rows (`m1`, `m2`, `s7`,
  `s8`, `u1`), again with `LANG` and `LC_ALL` set to `tr_TR.UTF-8` and to `lt_LT.UTF-8`
  (`correspondence-runs/js/run-js.sh --cultures`). The culture took effect: each of those five probes'
  first line is `Intl`'s default locale, which the shell reads from .NET's current culture
  (`BI/Intl/JSIntl.cs:2380`); it reads `"tr-TR"` and `"lt-LT"` in those runs and `"en-US"` in the run with
  no culture set. `s2` holds no case-insensitive pattern. `b7`'s one case-insensitive pattern is a `v`
  pattern, which the shell hands to Broiler.Regex (`BI/RegExp/JSRegExp.Broiler.cs:64-100`), whose folding
  reads no culture (M2a's `u` and `v` rows answer alike in all three cultures), so B7 is not a culture row.
- **The machine** for every run: Ubuntu 24.04.5 LTS, 4 processors, .NET SDK 10.0.401 with runtime
  10.0.12, and ICU 74 (`libicuuc.so.74.2`), whose casing and culture data .NET's case-insensitive
  matching reads in M2a.
- **A library cell** (Broiler.Regex `0.1.0-preview.4` called directly) was run where the shell cannot show
  the package's own answer, because `TryRouteToBroiler` catches every exception and falls back to .NET
  (`correspondence-runs/js/library/`, a scratch console project referencing the package).
- **Three paths for every pattern a probe tries with `both()`** (`probes/prelude.js`): the constructor;
  the literal evaluated (an indirect eval of the literal's text); and **the literal never evaluated**,
  its text after a statement that throws, in an indirect eval, as test262's negative tests place a
  literal after `$DONOTEVALUATE()`. On the third path a `SyntaxError` is an early error (13.2.7.1), and
  `not early` means the program was admitted and ran, so whatever error the literal has comes only when
  it is evaluated. `f1`'s literal flags and `s3`'s `(?:` depths take the third path by the same
  `early()`.
- **How a value is printed**: a string by `q()` and an engine's error message by `plain()`, each code
  unit outside printable ASCII as `\uXXXX`, so a NUL or a line terminator a message echoes stays on its
  line and no output holds a NUL; a probe's own label or a group name it declares may hold a non-ASCII
  character, written as UTF-8 (`m1`, `s2`, `s4`). The shell's stack traces name the program by its
  absolute path, which `run-js.sh` writes as `<runs>/` (the folder the script is in); that is the only
  rewrite of an output.
- *(reading)* marks a cell read from code and not executed.

**References.** "The specification" is ECMA-262 ES2026 as Broiler.VM archives it
(`src/Broiler.VM.Profile.JavaScript/docs/specification/ecma-262-es2026-spec.html`, tc39/ecma262 `0248456c`),
22.2, with Annex B.1.2, and the ES2025 duplicate-named-groups grammar it includes. A cell that says what the
specification answers reads that text; `regexpref` (SJL-11.3's reference) is the arbiter that will check those
readings, and a row whose reading it refutes is reclassified. Unicode data is UCD 17.0.0 (the programme's
pin), read from Broiler.Unicode's archived `data/unicode/pins/ucd-17.0.0/`. A test262 path is at the
suite's pin `ccaac100`; a test262 verdict cited is from Broiler.VM's bundle `sjl-12-001`
`base/hosted/wide-bytecode.report.gz` (tested commit `889ea15`, whose regular-expression sources and
literal scanner are byte-identical to `aaa39fb`) or Broiler.JS's bundle `sjl-10-001`
`test262/after/test262-results.json.gz` (`00b65dd0`, whose validators, scanner, compiler literal and
`BuiltIns/RegExp/` are byte-identical to `1f91b8f1`'s), original variant.

**Paths.** In Broiler.VM, `F/` is `src/Broiler.VM.Profile.JavaScript.Format/`, `VM/`
`src/Broiler.VM.Profile.JavaScript/`, `CP/` `src/Broiler.VM.Profile.JavaScript.Compiler/`. In Broiler.JS,
`BI/` is `Broiler.JS/Broiler.JavaScript.BuiltIns/`, `PA/` `Broiler.JS/Broiler.JavaScript.Parser/`, `RT/`
`Broiler.JS/Broiler.JavaScript.Runtime/`, `CO/` `Broiler.JS/Broiler.JavaScript.Compiler/`, `SH/`
`Broiler.JS/Broiler.JavaScript/`. In Broiler.RegEx at `c1422f4d`, `BR/` is `Broiler.Regex/`. A line number
is the line at the revision above; a bare `:n` is in the file named last in the same cell.

**The runs.** `runs/` is [`correspondence-runs/`](correspondence-runs/):
- `vm/`: `probes/` (each probe, with the shared `prelude.js` and `epilogue.js`), `built/` (the program
  run), `vm262/test/sjl11/` (the same program as a raw test262 test, with its front matter), and per probe
  `<name>.vm.raw.txt` (the command, the revision and the root's transcript) and `<name>.vm.txt` (the
  probe's lines); `<name>.fuel-bisect.txt` (row B7: every step of the bisection and its result);
  `cli/<name>.cli.raw.txt` (the command-line root's output and exit); `run-vm.sh`, `bisect-vm-fuel.sh`,
  `run-vm-cli.sh`; `hashes.txt`.
- `js/`: `probes/` (the same files, byte for byte), `built/`, per probe `<name>.bjs.txt` (the command, the
  revisions, the shell's output and exit) and, where run, `<name>.tr-TR.bjs.txt` and
  `<name>.lt-LT.bjs.txt`; `compare/<name>.compare.txt`, written by `sidebyside.py` from both engines'
  files; `library/` (the library probe, its project and `library.out.txt`); `run-js.sh`; `hashes.txt`.
- A file over 100 KB is retained gzip-compressed under its name with `.gz` (`gzip -n -9`): the VM's
  transcripts of the 200,000-deep patterns, which echo the pattern, and the shell's stack trace for B8.

**How a comparison is marked** (`compare/`). Each line starts with two marks: Broiler.VM against Broiler.JS
under the invariant culture (blank equal; `~` both throw the same constructor with different messages,
which are each engine's own; `!` any other difference; `?` the VM printed no line), then Broiler.JS across
cultures (`T` tr-TR differs, `L` lt-LT differs). Every literal-path refusal is a `SyntaxError` in both
engines but one: Broiler.VM answers a literal that ends in a backslash at the end of the evaluated source,
`/a[bc\`, with an `EvalError` (row S9b). The VM's `SyntaxError` says "the evaluated source is not a program this
profile admits", Broiler.JS's names the token where its scanner's fallback stopped (row S9), or, for an error that
comes only at evaluation, the constructor's message (row S11).

**It holds no figure of a run in prose.** The values below are inputs, answers and code positions. The
elapsed times some probes print (`m5`, `b2b`, `b3b`, `b4a`, `b4b`) and the allowances
`bisect-vm-fuel.sh` finds are in the run files only; no cell states one, and no class rests on one.

## Revisions: the pinned revisions against today's default branches

Every cell was run at the revisions above. The default branches were fetched on 2026-10-21 and every
regular-expression source compared with them (`git diff --stat`); none of the cells was run again at
them. A file not named here does not differ.

**Broiler.VM, `aaa39fb` against `origin/main` `3fa4b39`.**

| File | Difference | Could a cell's answer differ at `origin/main`? |
|---|---|---|
| `F/JsRegExpMatcher.cs`, `VM/JsRealm.RegExp.cs`, `VM/JsExecution.cs` | none | no |
| `CP/SliceTokenizer.cs`, `CP/JsParser.cs` (the literal's scan, rows S9 and S9b) | none | no |
| `CP/JsCompiler.Literals.cs` | two hunks, both in `CompileNamedValue` (`:50-56`, a fingerprint comment; `:70-76`, the `Closure` emit passes the function as written) | no: `CompileRegExpLiteral` (`:430-457`) and `RefuseRegExpLiteralFlags` (`:470-509`) are unchanged |
| `CP/JsCompiler.Eval.cs`, `JsCompiler.Hoisting.cs`, `JsCompiler.Statements.cs` and seven other compiler files; `CP/JsBinder.cs` and `CP/JsBinder.Walks.cs` (new, SJL-12.6) | the eval, hoisting and statement paths move onto the new binder | no, by reading: every literal-path cell runs through an indirect eval, whose compile path changed, but none of these changes touches `CompileRegExpLiteral`, which is unchanged, and the binder walks a regular-expression literal as a leaf (`JsBinder.Walks.cs:665-668` at `origin/main`); the eval'd programs of the cells declare nothing the binder's new early errors judge, so the literal refusals cannot move. Not run |
| `src/compositions/.../SliceCompiler/BinderChecks.cs` (new) | a composition's checks | no |

**Broiler.VM, `3fa4b39` against `origin/main` `daa92eb`** (PR #166, SJL-12.6 item 2, merged on 2026-10-21 after
these cells were taken; read on the same day by the area verdict). **The literal's check moved from the
lowering to the binder**, and nothing else in the area changed:

| File | Difference | Could a cell's answer differ at `daa92eb`? |
|---|---|---|
| `CP/JsCompiler.Literals.cs`, `CP/JsBinder.EarlyErrors.cs` | `RefuseRegExpLiteralFlags` and the pattern check of `CompileRegExpLiteral` move, by their remark "unchanged" (`CP/JsBinder.EarlyErrors.cs:461-463`), into the binder's `CheckRegExp` (`:469-519`): the same flag checks (`:478-503`), the same compile by `Format.JsRegExpMatcher.Compile` (`:505-518`), the same code (`UnexpectedToken`) at the literal's span; `CompileRegExpLiteral` (`CP/JsCompiler.Literals.cs:415-421`) now only emits the constructor call, and its remark (`:396-412`) says the binder checks each literal once, where the lowering checked it at each emission | no, by reading: a literal refused before is refused by the same matcher at the same span, as an early error, and a literal admitted before is admitted. Not run |
| `F/JsRegExpMatcher.cs`, `VM/JsRealm.RegExp.cs`, `VM/JsExecution.cs`, `CP/SliceTokenizer.cs` | none | no |
| `CP/JsParser.cs` | a comment (the binder refuses a stray cover entry) and the diagnostic cap named by a constant | no |

At `daa92eb`, then, the literal validation SJL-11.4 re-points onto Broiler.Regex's validate entry is the binder's
`CheckRegExp`, not `CompileRegExpLiteral` and `RefuseRegExpLiteralFlags` (JSD-0081, decisions D18 and D20).

**Broiler.JS, `1f91b8f1` against `origin/main` `792ff56e`** (PR #1066, SJL-12.5's WP5.5, merged while
these cells were taken; the head before it was `2345da11`, where only `PA/FastScanner.cs` differed).

| File | Difference | Could a cell's answer differ at `origin/main`? |
|---|---|---|
| `BI/RegExp/` (all), `BI/String/JSStringPrototype.Search.cs`, `PA/RegExpValidator.cs`, `RT/LegacyRegExpState.cs`, `CO/Expressions/FastCompiler.VisitLiteral.cs`, `Directory.Packages.props` (Broiler.Regex `0.1.0-preview.4`) | none | no |
| `RT/RegExpGroupNames.cs`, `RT/Unicode/JSUnicodePropertyNames.cs` | moved to `Broiler.JavaScript.Ast/` byte for byte | no |
| `RT/RegExpUnicodeValidator.cs` | moved to `Broiler.JavaScript.Ast/`, ten lines added (`#nullable disable`, a remark) | no |
| `RT/Unicode/JSUnicode.cs`, `JSUnicodeTables.cs`, `JSUnicodeTableData.g.cs` | moved to `Broiler.JavaScript.Ast/Unicode/`, comments only | no |
| `RT/AssemblyInfo.cs` | `TypeForwardedTo` for the moved public types | no |
| `PA/FastScanner.cs` | +70 −18: white space and identifier classes read from the parser options' provider (none by default, so the same tables), the scanner's own end-of-file and empty tokens; no hunk in the literal's scan (`:996-1453` at `1f91b8f1`) | no, by reading. Lines after `:200` move by 52: `DecodeBracedUnicodeEscapes` `:1048-1145`, the decision `:1224-1285`, `IsValid` `:1505` at `792ff56e` |
| `PA/FastParser*.cs`, `FastParserGoal.cs`, `FastParserOptions.cs`, `FastTokenStream.cs`, `CharExtensions.cs`; `RT/CoreScript.cs`, `IJSCompiler.cs`, `JSCode.cs` and the compilers | goal and options plumbing | no, by reading: no regular-expression or division decision changed |

**Broiler.RegEx:** `c1422f4d` (the nuspec commit of `0.1.0-preview.4`) against `origin/main` `0799342`: no
file under `Broiler.Regex/` differs.

## Facts

### Where each engine's code is

**Broiler.VM: one matcher, the realm's adapter, the compiler's literal check, the scanner.**
- `F/JsRegExpMatcher.cs` (4,671 lines): `JsRegExpCharge` (`:31`), `JsRegExpSyntaxError` (`:41`),
  `JsRegExpOverflowError` (`:62`), `JsRegExpCase` (`:114`, Canonicalize `:129-130` over the owner's
  folding tables), `JsRegExpCharSet` (`:168`; a property escape is a reference, `AddProperty` `:302`, and
  a class's `Weight` `:320` is charged per character tested, `:4486-4490`), `JsRegExpMatch` (`:420`), and
  `JsRegExpMatcher` (`:559`): its limits `MaximumNestingDepth = 512` (`:564`), `MaximumFrames = 1 << 20`
  (`:569`), `MaximumTrail = 1 << 21` (`:574`), `StepsPerCharge = 512` (`:579`); `Compile` (`:772-793`);
  `Match` with its anchored form and prefilter (`:807-855`); the parser (`:1494`), with its name
  pre-scan and duplicate refusal (`:1559-1617`), `GroupName` (`:1655-1706`), the nesting counter
  (`:1816-1819`, `:2615-2618`), escapes (`:2354-2417`, `:3260-3377`), `ReadFixedHex` (`:3382-3403`),
  the `\u` escape (`:3405-3459`), `\k` (`:2422-2464`) and property names (`:2532-2596`); the lowering
  (`:3537`), where a greedy quantifier over a class is one `Run` instruction (`:3833-3846`) and any other
  unbounded quantifier a loop with one `Split` per iteration (`:3869-3887`) whose iteration writes a
  progress cell (`:3937`) and clears its groups (`:3941`); and the runner (`:4012`), whose ceilings throw
  `JsRegExpOverflowError` (`PushFrame` `:4341-4345`, `Write` `:4379-4383`), and `TakeBackReference`
  (`:4618-4669`).
- `VM/JsRealm.RegExp.cs` (2,452 lines): the String methods' dispatch (`:420-520`, `RegExpDispatch`
  `:657-704`); `RegExp.escape` (`RegExpEscapeText` `:1460-1493`, `RegExpEscapeCodePoint` `:1508-1557`,
  `RegExpEscapeHex` `:1565-1571`); `RegExpBuild`, whose compile charge is `source.Length + 32` (`:1729`);
  `RegExpNormalizeFlags` (`:1738-1786`); `RegExpCompile` (`:1791-1809`); `RegExpSourceText`
  (`:1814-1875`); `RegExpMatchOne` (`:1900-1964`), which steps a mid-pair `lastIndex` back to the pair
  under `u` (`:1926-1932`) and anchors a sticky match (`:1936`); `RegExpRun`, which turns the matcher's
  ceilings into `JsAbort(Exhausted)` (`:1969-1980`); the meter, `engine.Charge` (`:1985`).
  `VM/JsExecution.cs:506` reports `JsAbortKind.Exhausted` as `ProfileFault/AllowanceExhausted`, with no
  dimension named.
- `CP/JsCompiler.Literals.cs`: `CompileRegExpLiteral` (`:430-457`) and `RefuseRegExpLiteralFlags`
  (`:470-509`) compile a literal with the same matcher, charging nothing.
- `CP/SliceTokenizer.cs`: `ReadRegularExpressionLiteral` (`:1923-1946`) and `ScanRegularExpressionBody`
  (`:2202-2255`), the literal's own grammar.

**Broiler.JS: a translator to .NET, Broiler.Regex for every `u`/`v` pattern, and prechecks.**
- `BI/RegExp/JSRegExp.cs` (4,965 lines): `RegExp.escape` (`:41-134`); `EscapePatternSource`, the `source`
  getter (`:170-267`, a `/` escaped outside a class only, `:225`); the constructors (`:307-399`);
  `ParseFlags` (`:671-758`, `RegexOptions.IgnoreCase` without `CultureInvariant` `:698-701`);
  `CreateRegex` (`:768-1046`) with three prechecks, the Unicode validator (`:780`), group names (`:789`)
  and property names (`:796`), then `TryRouteToBroiler` (`:805`) and the translator from
  `TransformES3Patterns` (`:817`) to `RewriteCaptureGroups` (`:1011`), which checks the modifiers
  (`ValidateInlineModifiers` `:850`, `:1148`), rewrites `v` classes (`:886`,
  `JSRegExp.UnicodeSets.cs:70`), rewrites `^` and `$` only under `m` (`:931-983`) and `.` for the four
  line terminators (`:928-929`, `:2580-2620`).
- `BI/RegExp/JSRegExp.Broiler.cs`: `TryRouteToBroiler` (`:64-100`) parses **every** pattern with
  Broiler.Regex's parser (`:76`), catches every exception (`:78-81`, `:96-99`), routes every `u`/`v`
  pattern Broiler builds and the non-`u` gap shapes (`GapScan`, `:104-228`); `RunMatch` (`:284-316`)
  falls back to .NET, or to no match when the translator built none, on `RegexOverflowException`
  (`:292-304`).
- `BI/RegExp/JSRegExpPrototype.cs`: `ExecMatch` (`:153`), which starts at `lastIndex` as it stands
  (`:176-177`) and emulates sticky by a forward search whose answer it discards unless it starts there
  (`:179`).
- `BI/String/JSStringPrototype.Search.cs`: `Search` (`:186-225`), with the direct call for a `JSRegExp`
  (`:210-217`, `jSRegExp.value.Match` `:212`).
- `PA/FastScanner.cs`: `DecodeBracedUnicodeEscapes` (`:996-1093`); the regular-expression-or-division
  decision (`:1172-1233`); the parenthesis patch (`:1411-1440`); the decode call (`:1449-1450`);
  `RegExpValidator.IsValid` (`:1453`), whose `false` makes the token a division. `PA/RegExpValidator.cs`
  (996 lines, `IsValid` `:18-160`) calls the three prechecks (`:89`, `:101`, `:111`), under `v` only a
  structural check (`ValidateUnicodeSetsPattern` `:309`), and otherwise ends in `new Regex(pattern,
  options)` (`:153`), which accepts repeated, upper-case and colon-less inline options.
- `CO/Expressions/FastCompiler.VisitLiteral.cs:30-31`: a literal is built when it is evaluated
  (`JSRegExpBuilder.New`), so `CreateRegex`'s checks run then.
- `RT/RegExpUnicodeValidator.cs` (`IsValidUnicodePattern` `:16`), `RT/RegExpGroupNames.cs` (`FirstError`
  `:33`, `FirstDuplicate` `:117`, which compares a name as written `:166-167`),
  `RT/Unicode/JSUnicodePropertyNames.cs` (`FirstRefused` `:31`).
- `SH/Program.cs:128`: the shell runs a script on a thread with a 16 MiB stack.
- Broiler.Regex `0.1.0-preview.4`: `BR/Parsing/RegexParser.cs` (the pre-scan's duplicate refusal over
  the name as written `:138-141`, `\0`'s lookahead `char.IsDigit` `:1166`, `ReadGroupName` `:1372-1382`);
  `BR/Matching/Matcher.cs` (`StepLimit = 10_000_000` `:68`, the budget reset at every start position
  `:122-126`, `CompileSequence`'s continuation chain `:216-267`, the uncharged disjunction `:311-326`, the
  stack guard `:420-421`, `Budget.Step` only at `:436`, `:452`, `:476`, `:591`, `:623`, `:638`, the
  single-character fast path's early exit on the budget `:591-592`, look-around `:777-793`, `CompileChar`
  `:800-811`, `ReadAt` `:1012`); `BR/Matching/MatchState.cs` (`Budget` `:29`).

### What already agrees

The answers both engines give alike, run: every example and note of 22.2.2 probed (`m1`, run with and
without `u`, including look-behind captures, nullable quantifiers, back-references within the BMP and
`d` indices); flags and their order and the `flags` getter's reads (`f1`); the valid modifiers and the
constructor's refusals of invalid ones (`s7`); `RegExp.escape` over the inputs of `t2`; the property names
of `s6`, exact names only, in both; `\s`, `\d`, `\w`, the Unicode 17.0 scripts and the string properties of
`u1` but one row; the `v`-mode set semantics of `s8`; and, on the literal never evaluated, every refusal
both engines make early. These are the rows marked **same in both** below.

## S. Syntax and early errors, literal validation

| Row | Input | Broiler.VM (`aaa39fb`) | Broiler.JS (`1f91b8f1`, preview.4) | The specification | Class |
|---|---|---|---|---|---|
| S1 | Duplicate named groups: `(?<a>x)\|(?<a>y)` under none, `d`, `u`, `v`; with `\k<a>`; three alternatives; `((?<a>x)\|(?<a>y))(?<a>z)`; `$<a>`; group order; each by the three paths *(run, `s1`; library)* | **`SyntaxError` for every duplicate**, constructor, literal and literal never evaluated (an early error): the pre-scan refuses any repeated name (`F/JsRegExpMatcher.cs:1592-1617`, `:1612`) | Accepted across alternatives, with `groups.a` the participating group, `indices.groups`, `\k<a>`, `$<a>` and property order as the specification; refused within one alternative (`RT/RegExpGroupNames.cs:33`). Under `u`/`v` **the library refuses every duplicate, the valid ones too** (`BR/Parsing/RegexParser.cs:138-141`; `library.out.txt`, `duplicate-alternatives`) and the shell answers through .NET | ES2025: a name may repeat only in alternatives that cannot both participate | **Correction of Broiler.VM** (kind D, SJL-D-14 (i)) **and of Broiler.Regex's preview.4** (the valid duplicates it refuses, masked in Broiler.JS by the .NET fallback; R6). Survey row "duplicate names": **confirmed** |
| S2 | Escaped group names: `\u0041`, `\u{41}`, `\uD835\uDC00`, `\u{1D400}`, a raw pair, `\uD800`, `\u{D800}`, `\x41`, ZWNJ, digits, punctuation, U+1D7DA, U+1F98A, each without and with `u` *(run, `s2`; library)* | The specification's answers for every case, all three paths | The same answers through the constructor, and through the literal but two rows (S2b). The library refuses `(?<\u{1D400}>x)` without `u` (`RegexSyntaxException`), throws `ArgumentOutOfRangeException` for `(?<\uD835\uDC00>x)` without `u` and for `(?<\uD800>x)` under `u` (`BR/Parsing/RegexParser.cs:1372-1382`); the shell masks all three (its precheck or .NET answers) | `RegExpIdentifierName` with `\u` escapes in both modes; a lone surrogate is a `SyntaxError` | **Same in both** at engine level; **correction of Broiler.Regex's preview.4** (escapes in names, exception kind; R5), each masked in Broiler.JS. What Broiler.JS itself shows of escaped names is S2a and S2b. Survey row "escaped group names": **confirmed** at library level; Broiler.JS's own defects are S2a and S2b |
| S2a | The same name spelled once with an escape: `(?<\u0061>x)(?<a>y)` without and with `u` *(run, `s1`; library)* | `SyntaxError`, early (the pre-scan decodes before comparing, `:1607-1612`) | **Accepted**, all three paths; `groups.a` is `"x"` without `u` and `"y"` with it: the duplicate test compares names as written (`RT/RegExpGroupNames.cs:117`, `:166-167`), and so does the library's (`BR/Parsing/RegexParser.cs:138-141`), which accepts it too | A duplicate in one alternative: an early `SyntaxError` | **Correction of Broiler.JS and of Broiler.Regex's preview.4** (the duplicate test after decoding) |
| S2b | A reference spelled with an escape: `(?<A>x)\k<\u0041>`, `(?<\u{1D400}>x)\k<𝐀>` *(run, `s2`)* | Accepted, all three paths | Constructor accepts; **the literal is refused, early** (`PA/RegExpValidator.cs:89`, `PA/FastScanner.cs:1453`) | Accepted: the reference names the group | **Correction of Broiler.JS** (literal validation disagrees with the constructor) |
| S3 | The nesting policy: 510, 511, 512 and 1,000 nested `(?:`, `(`, `(?=` and `v`-mode `[`, constructor and literal (the `(?:` form also never evaluated), with and without `u` *(run, `s3`)* | 511 nested groups accepted; **512 and more refused with `SyntaxError`**, early on the literal, in every form (`F/JsRegExpMatcher.cs:1816-1819`, `:2615-2618`: the pattern is level 1, each group or class one more, at most 512) | Accepted at every depth probed, with and without `u`, on every path | No bound | **Engine policy that becomes a parameter** (nesting 512 in both, JSD-0081 D7; Broiler.JS's deeper patterns become listed refusals). Survey row "the 512 nesting policy": **confirmed** |
| S3b | 20,000 and 200,000 nested `(?:`, without and with `u` *(run, `s3b`, `s3c`, `s3e`, `s3f`, the constructor)* | `SyntaxError` | 20,000: **the specification's answer**, both modes (the shell's script thread has 16 MiB, `SH/Program.cs:128`). **200,000: the process aborts with a stack overflow in `RegexParser.ParseAtom`, with and without `u`**, because every pattern is parsed by Broiler.Regex first (`BI/RegExp/JSRegExp.Broiler.cs:76`) | No bound | 20,000: **engine policy** (D7: the case becomes a refusal for the nesting limit, JS part A's C3). 200,000: **correction of Broiler.JS and of Broiler.Regex's preview.4** (R4: a nesting limit checked before the stack is) |
| S4 | `\0` before a non-ASCII decimal digit: `\0` + U+0661 and + U+FF11 under `u`, in a class, without `u`; `\01`, `\00`, `[\00]`, `\8`, `\2(a)`, `\x4`, `\u12`, `\u{110000}`, `\u{3}`, `\c`, `[\c_]`, `\cJ`, `\-`, `\/` *(run, `s4`; library)* | The specification's answers for every case: `\0` then U+0661 matches NUL, U+0661 | **The same answers** in the shell, all three paths. The library: under `u` it refuses `\0`+U+0661 ("Invalid escape \0 in Unicode mode", `BR/Parsing/RegexParser.cs:1166`, `char.IsDigit`), and **without `u` it builds `\0`+U+0661 and answers no match on NUL, U+0661**, where the specification matches at 0 (`library.out.txt`, `nul-nd`); the shell's routing catches the first and its non-`u` path is .NET, so neither is observable in Broiler.JS | `CharacterEscape :: 0 [lookahead ∉ DecimalDigit]`, `DecimalDigit` ASCII only | **Same in both** at engine level; **correction of Broiler.Regex's preview.4** (both answers, one cause: `:1166`). Survey row "`\0` + Nd": **confirmed at library level, refuted at engine level** (masked by the .NET fallback) |
| S5 | `\k`: `\k<b>(?<a>x)` with and without `u`, forward reference, `\k<a>` without named groups, `\k`, `\k<a`, `\k<>`, `[\k<a>](?<a>x)` *(run, `s5`; library)* | The specification's answers, except **`[\k<a>](?<a>x)` accepted** on all three paths, `\k` read in a class as the letter (`F/JsRegExpMatcher.cs:3322-3329`) | The specification's answers, `[\k<a>](?<a>x)` refused early; `\k<b>(?<a>x)` refused by .NET's parser (its message names the renamed group). The library: **`RegexParser.Parse` accepts `\k<b>(?<a>x)`**, its constructor refuses | With a `GroupName` anywhere, `IdentityEscape[+NamedCaptureGroups]` excludes `k`, in a class too (B.1.2); a `\k<b>` naming no group is an early error | VM: **correction of Broiler.VM** (kind R, on this reading; `regexpref` checks it). Library: **correction of Broiler.Regex's preview.4** (the validate entry, SJL-11.2). Survey row "`\k` parse-only": **confirmed at library level**, not observable in Broiler.JS |
| S6 | Property names: 57 names under `u` (constructor and literal), `v`, and in a `u` class: exact names and aliases, loose spellings, blocks, `Is` prefixes, `Hrkt`, string properties; `\P{RGI_Emoji}`, `[^\p{RGI_Emoji}]` under `v`; `\p{L}` without `u` *(run, `s6`; library)* | Exact names only (`F/JsRegExpMatcher.cs:2532-2596`, the owner's `UnicodeProperties`); `Hrkt` accepted; string properties under `v` only | **The same answer for every name**, on all three paths (`RT/Unicode/JSUnicodePropertyNames.cs:31`, since SJL-7.5). The library alone accepts `\p{letter}` and `\p{Script=greek}` and refuses `\p{sc=Hrkt}` | Table-exact names (22.2.2.9) | **Same in both**; **correction of Broiler.Regex's preview.4** (R11). Survey row "loose names (corrected at SJL-7.5)": **confirmed corrected** in Broiler.JS; the library is still loose |
| S7 | Annex B and `u` mode: quantified look-ahead, look-behind and lone braces, brackets, `{`-forms, class ranges with escapes, large quantifiers, empty groups and classes, unbalanced parentheses *(run, `s7`, three cultures for Broiler.JS)* | The specification's answers, every refusal early; `(?<=a)*` refused (`F/JsRegExpMatcher.cs:1899-1901`) | The same, every refusal early, except **`(?<=a)*` without `u` accepted on all three paths**: the constructor's library parse refuses it and is caught (`BI/RegExp/JSRegExp.Broiler.cs:76-81`), and .NET's `Regex` accepts the translation; the literal's check is the same `new Regex` (`PA/RegExpValidator.cs:153`). The library refuses it (`library.out.txt`) | `QuantifiableAssertion` is look-ahead only (B.1.2), an early error; test262 `language/literals/regexp/invalid-optional-lookbehind.js`, `invalid-optional-negative-lookbehind.js`, `invalid-range-lookbehind.js`, `invalid-range-negative-lookbehind.js`, each failing in Broiler.JS's `sjl-10-001` run | **Correction of Broiler.JS**. The modifiers `s7` also probes are F3 and S11 |
| S8 | The `v` flag's classes: set operators, nested classes, `\q{}`, reserved punctuators, syntax characters, `vi` folding of strings; and test262's `built-ins/RegExp/prototype/unicodeSets/breaking-change-from-u-to-v-01.js` to `-28.js` by their patterns, `[(]` `[)]` `[[]` `[{]` `[}]` `[/]` `[-]` `[\|]` `[&&]` `[!!]` `[##]` `[$$]` `[%%]` `[**]` `[++]` `[,,]` `[..]` `[::]` `[;;]` `[<<]` `[==]` `[>>]` `[??]` `[@@]` ``[``]`` `[~~]` `[^^^]` `[_^^]` *(run, `s8`, three cultures; library)* | The specification's answers; every refusal early | The set semantics the same. **Every breaking-change pattern but `[[]`, and `[a\|]` and `[a!!b]`, accepted on all three paths**: the library refuses each (`library.out.txt`, `v-syntax`), the constructor catches that (`BI/RegExp/JSRegExp.Broiler.cs:76-81`) and its `v` translation (`BI/RegExp/JSRegExp.cs:886`, `JSRegExp.UnicodeSets.cs:70`) and .NET admit it; the literal's `v` check is structural only (`PA/RegExpValidator.cs:309`). `[[]` is refused early by the literal path and by the constructor with a .NET `ArgumentOutOfRangeException`'s message as its `SyntaxError`'s, which reads the culture (lt-LT writes the minus sign as U+2212, the only culture mark outside the locale lines). `[a&&&b]`, `[a--b&&c]`, `[a-\d]`, `\q{a}` are refused only when evaluated (S11) | `ClassSetSyntaxCharacter` and `ClassSetReservedDoublePunctuator` are early errors; each breaking-change file is a negative test of phase parse | **Correction of Broiler.JS** (every breaking-change file but `-03` fails in Broiler.JS's `sjl-10-001` run, and `-03` passes) |
| S9 | Division against a literal, through `eval`: `var x = {}\n/a/gg`, `var x = {}\n/a/g`, `var x = function(){}\n/a/g`, `yield /a/g` outside a generator; invalid literals where the grammar wants one; valid literals for contrast *(run, `s9`)* | The grammar's answers: `NaN` for the three divisions after `}`, `1` for `yield`, `SyntaxError` for each invalid literal, the valid literals admitted | `var x = {}\n/a/gg`: **`SyntaxError`**; `var x = {}\n/a/g` and `var x = function(){}\n/a/g`: **the object or function** (`/a/g` read as a literal statement); `yield`: `1`; each invalid literal a `SyntaxError`, reported at the token where the scanner's fallback stops (`Divide`, `Identifier`, `QuestionMark`, `Empty` or `LineTerminator`; `PA/FastScanner.cs:1172-1233`, `:1453`); the valid literals admitted | After `var x =`, `{}` is an object literal and `function(){}` a function expression; after an expression the lexical goal is InputElementDiv, so `/` divides | **Correction of Broiler.JS, outside SJL-11** (the scanner's goal is the shared frontend's: JSD-0081 D22, listed for SJL-14). Survey row "division": **confirmed** (and a valid literal read where the grammar divides) |
| S9b | A literal's own grammar: a backslash before a line terminator, `/\<LF>/`, `/ab\<U+2028>/`, `/ab[c\<LF>]/`, by direct and indirect eval; a literal ending in a backslash at the end of the source, `/a[bc\`; an unterminated string, for contrast *(run, `s9`)* | `SyntaxError` for each but **`/a[bc\`: `EvalError`, "the artifact provider did not supply a program: HostFailure/HostCapabilityFaulted"**: a backslash that is the source's last character moves the scan past the end (`CP/SliceTokenizer.cs:2227`), and the refusal's token text is then cut past it (`:1944`) *(the mechanism by reading)*. Test262 `staging/sm/RegExp/7.8.5-01.js` fails in Broiler.VM's `sjl-12-001` base run on it ("Expected a SyntaxError but got a EvalError") | `SyntaxError` for each | `RegularExpressionBackslashSequence :: \ RegularExpressionNonTerminator`: a `SyntaxError` | **Correction of Broiler.VM, outside SJL-11** (the scanner is the front end's, SJL-12 and SJL-13; SJL-11.4 does not touch it, so `7.8.5-01.js` fails at both commits of part A and is no C1 row) |
| S10 | Under `u`, a four-digit lead-surrogate escape before a braced escape: `\uD83D\u{DC38}+`, `\u{D83D}\u{DC38}+`, `\u{D83D}\uDC38+`, `\uD83D\uDC38+`, `\uD83D\u{3042}*`, `[\uD83D\u{3042}]*`, `[\uD83D\u{DC38}]`, the same under `v` and without `u` *(run, `s10`; library)* | **`SyntaxError` ("Invalid escape") for every pattern that has a four-digit lead escape followed by `\u{`**, early, under `u` and `v`: after a four-digit lead escape, a following `\u` commits to `ReadFixedHex(4)` (`F/JsRegExpMatcher.cs:3440-3458`), which throws under `u` on `{` (`:3397-3400`). The others the specification's answers | **The specification's answers** on all three paths **but one**: the library refuses the same patterns ("Invalid hexadecimal escape", `library.out.txt`, `lead-then-braced`), and the shell's .NET translation answers. **`[\uD83D\u{DC38}]` under `u` on `"\udc38"` answers `null`**, through the constructor and through the literal evaluated (`js/compare/s10-lead-surrogate-escapes.compare.txt`). *(The mechanism, by reading; the library's run covers the three patterns above, and its refusal of this one is read from the same `\u` commitment.)* The library's refusal sends the pattern to the translator, whose `TransformBracedUnicodeEscapes` (`BI/RegExp/JSRegExp.cs:878`, `:3096`) rewrites `\u{DC38}` inside a class to `\uDC38` with no separator (it wraps a lone surrogate only outside a class, `:3129`); the class reader `ReadClassCodePoint` (`:2397-2420`) then joins `\uD83D\uDC38` into U+1F438 (`:2412-2420`), so the class holds one astral code point and neither lone surrogate. On the literal path the scanner keeps a lone surrogate's braced escape braced (`PA/FastScanner.cs:1043-1061`), and the same translation answers | Only two four-digit escapes form a pair (`RegExpUnicodeEscapeSequence[+UnicodeMode] :: u HexLeadSurrogate \u HexTrailSurrogate`); a braced escape never joins, so each pattern is a lone lead surrogate and a following character: `\uD83D\u{DC38}+` on `"\uD83D\uDC38\uDC38"` is `null`, `\uD83D\u{3042}*` on `"\uD83D"` is `["\uD83D"]`, and the class `[\uD83D\u{DC38}]` holds U+D83D and U+DC38, so on `"\uDC38"` it matches at 0, `["\uDC38"]` | **Correction of Broiler.VM** (kind R: test262 `staging/sm/RegExp/unicode-braced.js`, `unicode-lead-trail.js`, `unicode-class-lead-trail.js` fail in Broiler.VM's `sjl-12-001` base run, "Invalid escape") **and of Broiler.Regex's preview.4** (the same refusal, masked in Broiler.JS). The class case `[\uD83D\u{DC38}]`: **correction of Broiler.JS** (the translator's join; no test262 file at `ccaac100` tries it: `unicode-class-lead-trail.js` tries `[\uD83D\u{3042}]`, which Broiler.JS answers as the specification does, so no Broiler.JS test262 row moves for it) |
| S11 | A literal Broiler.JS's scanner admits and its constructor refuses: the invalid modifiers `(?ii:a)`, `(?i-i:a)`, `(?-:a)`, `(?i)a`, `(?imsi:a)`, `(?I:a)`, `(?M:a)`, `(?S:a)`, `(?m-m:a)`, `(?s-s:a)`, `(?ims-m:a)`, `(?m-ims:a)`, `(?-ii:a)`, `(?-imsi:a)`, `(?ii-:a)`, `(?imsi-:a)`, `(?I-:a)`, `(?M-:a)`, `(?S-:a)`, `(?-I:a)`, `(?-M:a)`, `(?-S:a)`, `(?ms-i)`, `(?-s)`, `(?i-)`; under `v`, `[a&&&b]`, `[a--b&&c]`, `[a-\d]`, `\q{a}` *(run, `s7`, `s8`, three paths)* | Every one an early `SyntaxError` (`CP/JsCompiler.Literals.cs:430-457`) | **The literal is admitted when the program is parsed and throws its `SyntaxError` only when evaluated** (the literal never evaluated: `not early`; the literal evaluated: the constructor's message): the literal check ends in `new Regex` (`PA/RegExpValidator.cs:153`), which accepts repeated, upper-case and colon-less inline options, and under `v` checks structure only (`:309`); the literal is built at evaluation (`CO/Expressions/FastCompiler.VisitLiteral.cs:30-31`), where `CreateRegex` checks the modifiers (`BI/RegExp/JSRegExp.cs:850`, `:1148`) and the translator refuses the `v` cases. Every culture alike | Each is an early error of the pattern (22.2.1.1), reported for a literal before evaluation (13.2.7.1); test262's literal tests put `$DONOTEVALUATE()` first | **Correction of Broiler.JS** that D22's re-pointing of `PA/FastScanner.cs:1453` at the validate entry closes (SJL-11.5). The test262 rows it explains, each failing in Broiler.JS's `sjl-10-001` run with "negative test expected SyntaxError but got different error": `language/literals/regexp/early-err-modifiers-code-point-repeat-i-1.js`, `-code-point-repeat-i-2.js`, `-other-code-point-uppercase-I.js`, `-should-not-case-fold-i.js`, `-should-not-case-fold-m.js`, `-should-not-case-fold-s.js`, and `early-err-arithmetic-modifiers-add-remove-i.js`, `-add-remove-m.js`, `-add-remove-multi-duplicate.js`, `-add-remove-s.js`, `-both-empty.js`, `-code-point-repeat-i-1.js`, `-code-point-repeat-i-2.js`, `-no-colon-1.js`, `-no-colon-2.js`, `-no-colon-3.js`, `-other-code-point-uppercase-I.js`, `-reverse-add-remove-i.js`, `-reverse-add-remove-m.js`, `-reverse-add-remove-multi-duplicate.js`, `-reverse-add-remove-s.js`, `-reverse-code-point-repeat-i-1.js`, `-reverse-code-point-repeat-i-2.js`, `-reverse-other-code-point-uppercase-I.js`, `-reverse-should-not-case-fold-i.js`, `-reverse-should-not-case-fold-m.js`, `-reverse-should-not-case-fold-s.js`, `-should-not-case-fold-i.js`, `-should-not-case-fold-m.js`, `-should-not-case-fold-s.js` |

## M. Compilation and matching

| Row | Input | Broiler.VM | Broiler.JS | The specification | Class |
|---|---|---|---|---|---|
| M1 | The examples and notes of 22.2.2 (46 patterns), split, replace, matchAll *(run, `m1`, three cultures)* | The specification's answers | **The same**, in every culture (non-`u` gap shapes routed to Broiler.Regex, `BI/RegExp/JSRegExp.Broiler.cs:104-228`) | 22.2.2 | **Same in both** (a back-reference across a surrogate pair under `u` is M7) |
| M2 | F2 of bundle `sjl-7-001`: `[a-z]/i` against U+212A *(run, `m2`)* | `false` (`F/JsRegExpMatcher.cs:129-130`) | **`true`**, in every culture (.NET casing, `BI/RegExp/JSRegExp.cs:698-701`, `:4495`) | Canonicalize without `u`: U+212A is its own upper case; `k` is `K` | **Correction of Broiler.JS** (lands when the translator retires). Survey row "F2": **confirmed** |
| M2a | The dotted and dotless i, `i`/`I` against U+0130 and U+0131, in literals, classes, back-references and modifiers, without and with `u`/`v` *(run, `m2`, invariant, tr-TR, lt-LT)* | `false` for every case | Invariant: `false` for every case. **tr-TR: `true` for `i`↔U+0130 and `I`↔U+0131** (literal, class, back-reference, `(?i:)`); **lt-LT: `true` for `i`↔U+0130 and `I`↔U+0130**: `RegexOptions.IgnoreCase` without `CultureInvariant` (`BI/RegExp/JSRegExp.cs:698-701`) reads .NET's current culture and ICU's casing. With `u` or `v` (Broiler.Regex) `false` in every culture | Canonicalize reads no culture: every case `false` | **Correction of Broiler.JS** (the culture independence of part D, JSD-0081 D33) |
| M2b | U+1F80 against U+1F88 under `i` without `u` *(run, `m2`)* | `false` | **`true`**: .NET's simple case mapping under `IgnoreCase` (`BI/RegExp/JSRegExp.cs:698-701`) | U+1F80's upper case is two code points, so it canonicalizes to itself; so does U+1F88 | **Correction of Broiler.JS** |
| M2c | The rest of `m2` and `u1`'s case rows: long s, Kelvin, sharp s, final sigma, micro sign, U+0390, Deseret, `\w`/`\W`/`\b` under `iu`, property classes under `iu`/`iv` *(run)* | The specification's answers | The same, in every culture | | **Same in both** |
| M3 | Line terminators (bundle `sjl-7-001`'s W5): `^b` and `a$` under `m` and `gm` on LF, CR, LS, PS, CRLF and U+0085 separators; `a.b` without and with `u`, `s`; `(?m:^b)`; `a$` without `m` on `"a\n"`; split and replace over `^`/`$` *(run, `m3`)* | The specification's answers | **Under `m`, `^` and `$` ignore U+2028 and U+2029** (`/^b/m` on `"a\u2028b"` is `null`); **without `m`, `$` matches before a final LF** (`/a$/.exec("a\n")` matches, so does `/[^]$/`); split and replace follow. Under `u` (Broiler.Regex) the specification's answers | LineTerminator is LF, CR, LS, PS; `$` without `m` is the input's end only | **Correction of Broiler.JS** (`BI/RegExp/JSRegExp.cs:931-983`: only `\r` added to .NET's `\n` under `m`; no rewrite of `$` without `m`). Survey row "W5": **confirmed**, and wider than W5 read (`$` without `m`) |
| M4 | `lastIndex` inside a surrogate pair, under `g`, `y`, `u`, `v`: `/./gu`, `/./gv`, `/./yu`, `/\udf06/gu`, `/\udf06/yu`, `/(?:)/gu`, `/(?<=\ud834)./gu` at `lastIndex` 1 of U+1D306, `/./gu` at `lastIndex` 2 of `"a"` + U+1D306; the empty-match advance in match, replace, split, matchAll *(run, `m4`)* | Steps the start back to the pair and reports it (`VM/JsRealm.RegExp.cs:1926-1932`, its remark naming V8's answer): `/./gu`, `/./gv`, `/./yu` → **index 0**, the pair, `lastIndex` 2; on `"a"` + pair → **index 1**; `/\udf06/gu` and `/yu` → `null`; `/(?:)/gu` → index 0, `lastIndex` 0; the look-behind → `null` | Starts at `lastIndex` itself (`BI/RegExp/JSRegExpPrototype.cs:176-177`): `/./gu`, `/./gv`, `/./yu` → index 1, `"\udf06"`, `lastIndex` 2; on `"a"` + pair → index 2; **`/\udf06/gu` and `/yu` → index 1, a lone half**; `/(?:)/gu` → index 1, `lastIndex` 1; **the look-behind → index 1**. The empty-match advances agree | RegExpBuiltinExec: the matcher runs from "the character that was obtained from element `lastIndex` of S" (the pair), and `"index"` is `lastIndex` with the Match Record `{ [[StartIndex]]: lastIndex, [[EndIndex]]: e }`. For a non-empty match the text is determinate: `/./gu` gives index 1, `"\udf06"`, `lastIndex` 2 (on `"a"` + pair, index 2); `/\udf06/gu` and the look-behind fail at the pair and give `null`. For the empty match `/(?:)/gu` it is not: e is 0, so `lastIndex` becomes 0, `"index"` is 1, and the record `{1, 0}` is one GetMatchString's assertion forbids | Non-empty matches, the `"index"` and substring: **correction of Broiler.VM** (kind R, pending `regexpref`; no test262 row moves, since no test at `ccaac100` runs `exec` from a mid-pair `lastIndex` under `u`, and `exec/u-lastindex-value.js` starts at 0 and reads only `lastIndex`). The half-pair matches (`/\udf06/gu`, `/yu`, the look-behind): **correction of Broiler.JS**. The empty match: **a defect of the specification's text**, recorded with the text's reading (index 1, `lastIndex` 0, a Match Record whose start passes its end, which GetMatchString's assertion forbids) and both engines' answers (VM index 0, `lastIndex` 0; JS index 1, `lastIndex` 1). `regexpref`, written from the text, has no answer to give for it, and an ill-formed record is not a policy the specification leaves to the implementation. **It is classed implementation-defined by the owner's decision of 2026-10-21** (JSD-0081 decision D41), this case only, **until the specification is corrected**; the rules' class (b) admits it by citing that decision, not the text. Plan task 6.1's "`lastIndex` stepping back out of a surrogate pair" may move only the matcher's start, never the reported `"index"` or substring (JSD-0081 decision D23). Survey row "`lastIndex` in a surrogate pair": **confirmed** |
| M5 | Sticky: `/a/y` at 0 and 1, `/b\|ab/y`, `^` and `\b` at `lastIndex`, look-behind, search, match, replace, split; `/x\|(a+)+b/y` and `/uy` at 0 on `"y"` + 22 `a` *(run, `m5`)* | Every answer the specification's; the attempt is anchored (`F/JsRegExpMatcher.cs:807-855`, `VM/JsRealm.RegExp.cs:1936`) | **The same answers.** *(reading)* The non-`u` pattern runs a forward search over every later start and discards an answer that does not start at `lastIndex` (`BI/RegExp/JSRegExpPrototype.cs:179`); the `u` pattern is anchored by Broiler.Regex | One attempt at `lastIndex` | **Same in both** (answers), with a resource note: the forward search is work no answer shows, which SJL-11.5's anchored argument removes (plan task 2.4a, "an anchored argument"; 6.1, "sticky through the anchored argument"). Survey row "sticky emulation": **confirmed by reading**, cost only, the .NET path only; no answer differs |
| M6 | `String.prototype.search` with `RegExp.prototype[@@search]` deleted, and with an instance's own `@@search` undefined or null *(run, `m6`)* | `TypeError` for every call after the delete (the fresh RegExp has no `@@search`); `-1` for the own-undefined and own-null cases (RegExpCreate from `"/b/"`) | **`1`, `2`, `1`** (the direct `value.Match`, `BI/String/JSStringPrototype.Search.cs:210-217`, `:212`), ignoring `y`; **a pattern only Broiler.Regex holds throws `ReferenceError` ("Object reference not set…")**; the own-undefined and own-null cases `1` | 22.1.3.21: RegExpCreate(regexp), then Invoke(rx, @@search) | **Correction of Broiler.JS**. Survey row "`String.search`'s direct call": **confirmed** |
| M7 | A back-reference under `u` across a surrogate pair: `foo(.+)bar\1` on `"foo\uD834bar\uD834\uDC00"`, `"…\uD834\uD834"`, `"…\uD834A"`, `"fooAbarA\uDC00"`, `"foo\uDC00bar\uDC00\uD834"`; `^(.+)\1$`; under `iu`; `\k<x>`; a look-behind `(?<=\1(.))b` on `"\uD834\uDC00\uDC00b"`; under `v`; without `u` *(run, `m7`; library)* | **`foo(.+)bar\1` under `u` and `v`, and `\k<x>`, match `"\uD834"` against the first half of the pair** (index 0, `["foo\uD834bar\uD834", "\uD834"]`); **the look-behind under `u` matches** (index 3) where its `\1` reads the pair's second half; the others, `iu` included, the specification's answers: the comparison is code units (`string.CompareOrdinal`, `F/JsRegExpMatcher.cs:4642`), with no check that the reference ends on a code point, and backwards from a start the capture's length in code units before the position (`:4631`), while the `i` path reads code points (`:4647-4665`) | The specification's answers for every forward case (Broiler.Regex compares characters; `library.out.txt`, `backreference-pair`). **The look-behind under `u` matches at index 3, `["b", "\uDC00"]`, as Broiler.VM's does** (`js/compare/m7-backreference-surrogates.compare.txt`): a `u` pattern is Broiler.Regex's (`BI/RegExp/JSRegExp.Broiler.cs:64-100`). *(The mechanism, by reading; the library's run covers the forward case only.)* Its backward back-reference starts the capture's length in code units before the position (`BR/Matching/Matcher.cs:739-745`) and compares from there (`:747`, `RegionEquals` `:992-1007`) with no check that the start is on a code point, so it reads the pair's second half alone | Under `u` the input is a list of code points; BackreferenceMatcher compares characters, and U+1D000 is not `\uD834`: `null` for the pair cases and the look-behind; without `u`, code units: a match | **Correction of Broiler.VM** (kind R: test262 `staging/sm/RegExp/unicode-back-reference.js` fails in Broiler.VM's `sjl-12-001` base run, "Expected SameValue(«foo\uD834bar\uD834,\uD834», «null»)"). The look-behind: **also a correction of Broiler.Regex's preview.4, and so of Broiler.JS**, observable in the shell under `u`; no test262 file at `ccaac100` runs a look-behind back-reference under `u` (`built-ins/RegExp/lookBehind/back-references.js`, `back-references-to-captures.js` and `sliced-strings.js` set no `u`), so no Broiler.JS test262 row moves for it |

## B. Budget and limits

Broiler.VM charges every instruction and every character a run examines to the realm's fuel and stops a
match at a frame or trail ceiling; Broiler.JS passes no meter, and Broiler.Regex `0.1.0-preview.4`
spends a private budget of `StepLimit` per start position on its `u`/`v` patterns.

**Broiler.VM's remark refuted.** The matcher's class remark says neither ceiling "is reachable by a pattern
that was going to answer" (`F/JsRegExpMatcher.cs:509-512`). B1 and B6 refute it: `/a*/` over 1,049,000
characters and `(?:ab)*` over 1,049,000 iterations stop at the frame ceiling, and `(?:(a)(b)(c))*` over
162,000 iterations at the trail ceiling, where the specification answers the whole input. The lowering's
`Run` serves a greedy quantifier over a **class** only (`:3833-3846`); over a single character or a group
it is a loop with a frame per iteration (`:3869-3887`). So the adversarial corpus's third presumption ("a
greedy quantifier over one class runs as one instruction ... as Broiler.VM's Op.Run", Broiler.RegEx's
`tests/Adversarial/README.md`, section 3) holds for classes only; if the new backend runs `a*` as one
instruction, B1's Broiler.VM stop becomes an answer at SJL-11.4, which part A's C2 (moves between exhausted
dimensions) does not obviously cover. JSD-0081 classes B1's VM cell as engine policy (its decision D10) and
records the presumption for classes only (its decision D37); this bundle's `decision-rule-area.md` says how its
part A reads such a move.

**Which ceiling stopped** is not in either root's report: the conformance root says "FAIL ... the invocation
answered ProfileFault/AllowanceExhausted and carried no payload" (`VM/JsExecution.cs:506`), and the
command-line root says the same and **exits 7**, the code its own help calls "host defect", where a fuel
exhaustion exits 5, "AllowanceExhausted on Fuel" (`vm/cli/`: `b1i`, `b6e` against `b4b`; `b1h` completes
with exit 0). The dimension named in B1 and B6 is a reading of the lowering, bracketed by runs either side
of the ceiling it names.

| Row | Input | Broiler.VM | Broiler.JS | The specification | Class |
|---|---|---|---|---|---|
| B1 | A greedy run over 10,000,001 `a`: `/a*/u`, `/a*/`, `/[a]*/u`, `/^a*$/u`, `/^a*$/`, `/a*/u` over 9,999,999; and `/a*/` over 1,048,000 and 1,049,000 *(run, `b1a`-`b1i`, one case per file; `b1h`, `b1i` also through the command-line root)* | `/[a]*/u`: the whole input (one `Run`). **`/a*/`, `/a*/u`, `/^a*$/` with or without `u` over more than 2^20 characters: the invocation stops, `ProfileFault/AllowanceExhausted`, no payload**; over 1,048,000 the whole input. *(Which ceiling: reading.)* A character lowers to a loop with one frame (`Split`, `F/JsRegExpMatcher.cs:3872`) and one cell write (`:3937`) per iteration, so the frames' 2^20 (`:569`) comes long before the trail's 2^21 (`:574`); `b1h` completes and `b1i` stops, bracketing 2^20 frames | **`/a*/u` and `/[a]*/u`: index 0, length 10,000,000 (a shortened match)**; **`/^a*$/u`: `false`**; over 9,999,999 the whole input; without `u` (.NET) the whole input and `true` | The whole input; `true` | Broiler.JS: **correction of Broiler.JS and of Broiler.Regex's preview.4** (R1, the fast path's early exit `BR/Matching/Matcher.cs:591-592`). Broiler.VM: **engine policy that becomes a parameter** (the frame ceiling, `MaximumFrames` `F/JsRegExpMatcher.cs:569`, which plan task 5.2 passes to the new backend under JSD-0081's decision D10). Survey rows "shortened match" and "budget no-match": **confirmed** |
| B2 | Exhaustion inside one alternative: `/(?:(a+)+b\|a*c)/` and `/u` on 16 `a` then `c`, and `/u` on 24 `a` then `c` *(run, `b2a`, `b2b`)* | Index 0, length 17; index 0, length 25 (completes under the default allowance) | 16: index 0, length 17. **24 under `u`: index 4, length 21** | Index 0, length 17 and 25 | **Correction of Broiler.JS and of Broiler.Regex's preview.4**: the budget is reset at every start (`BR/Matching/Matcher.cs:122-126`), so exhaustion at one start is read as that start's failure and the next start answers; R1 removes the per-start reset. Survey row "the wrong index": **confirmed** |
| B3 | Exhaustion inside a negative look-ahead: `/^(?!(?:(a+)+b\|a*c))/` and `/u` on 16 `a` then `c`; `/u` on 28 `a` then `c` (Broiler.VM under fuel 2,000,000,000) *(run, `b3a`, `b3b`)* | 16: `null`. 28: **exhausted, fuel** | 16: `null`. **28 under `u`: a match at 0, length 0** | `null` | **Correction of Broiler.JS and of Broiler.Regex's preview.4** (`BR/Matching/Matcher.cs:777-793` reads exhaustion as no match and so passes the negative assertion). Broiler.VM's exhaustion is its fuel policy. Survey row "the look-around flip": **confirmed** |
| B4 | Quantifier-free alternation: `(?:a\|a)` written out 12, 16, 20, 22 and 24 times, then `b`, on as many `a`, with and without `u`; 24 under `u` with Broiler.VM's fuel at 10,000,000; and `(?:(?:a\|a)` written out n times `b\|a+)` on n `a`, n = 12, 20, 24 *(run, `b4a`, `b4b`, `b4c`)* | Every completed case the specification's answer; **24 under fuel 10,000,000: exhausted, fuel** (each instruction is charged; through the command-line root, exit 5) | Every case the specification's answer: `null` for `b4a`, `b4b`; **`b4c` under `u` at 24: index 0, length 24**. *(Reading, the row's ground.)* No alternation and no literal character charges the budget: `Budget.Step` is called only on the quantifier paths (`BR/Matching/Matcher.cs:436`, `:452`, `:476`, `:591`, `:623`, `:638`), never in `CompileDisjunction` (`:311-326`) or `CompileChar` (`:800-811`). The runs are consistent with it: `b4a` and `b4b` answer `null` whether or not alternation is charged, and `b4c`'s index 0 is what an uncharged first alternative gives, where a budget of `StepLimit` per start spent by the first alternative's 2^24 failing paths at start 0 would read that start as a failure and answer a later index or `null` | `null`; `b4c` index 0, length n | **Correction of Broiler.Regex's preview.4** (R1: alternation is charged). Broiler.JS passes no meter (D10), so its answer does not move. Survey row "uncharged alternation": **confirmed by reading**, the runs consistent |
| B5 | The stack guard: general quantifiers over 100,000 and 1,000,000 iterations (`(?:ab)*` under `u`; `(?:[\s--\d]a)*` under `v`, a class the translator cannot express); 50,000 general quantifiers in sequence before `c`, and before `(?:x)?(a?b??)*`, against 10 *(run, `b5a`-`b5g`)* | The specification's answer in every case | Every iteration count: the whole input (the general quantifier is iterative at preview.4, `BR/Matching/Matcher.cs:396-413`). **50,000 in sequence under `v`: `null`** (the guard, `:420-421`, throws, and no .NET engine exists: `BI/RegExp/JSRegExp.Broiler.cs:301-303`). **50,000 before `(a?b??)*` under `u`: `["ab", ""]`, the .NET translation's answer**, where 10 in sequence give the library's `["ab", undefined]` | The whole input; `["ab", undefined]` | **Correction of Broiler.JS and of Broiler.Regex's preview.4** (R4; no fallback). Survey rows "the stack-guard fallback" (to .NET, and to no match): **both confirmed** |
| B6 | Broiler.VM's two ceilings: `(?:ab)*` over 100,000, 1,048,000, 1,049,000 and 1,100,000 iterations; `(?:(a)(b)(c))*` over 161,000, 162,000 and 400,000 *(run, `b6a`-`b6g`; `b6e` also through the command-line root)* | `(?:ab)*`: 100,000 and 1,048,000 the whole input; **1,049,000 and 1,100,000: the invocation stops, `ProfileFault/AllowanceExhausted`, no dimension named** (`VM/JsRealm.RegExp.cs:1976-1979`, `VM/JsExecution.cs:506`). `(?:(a)(b)(c))*`: 161,000 the whole input; **162,000 and 400,000 stop the same way**. *(Which ceiling: reading.)* `(?:ab)*` pushes one frame and writes one cell per iteration, so the frames' 2^20 is reached first, and `b6f`/`b6g` bracket it; `(?:(a)(b)(c))*` pushes one frame and writes 13 cells per iteration (the progress cell `F/JsRegExpMatcher.cs:3937`, a clear of six capture cells `:3941`, six saves `:3805`, `:3807`), so the trail's 2^21 is reached while the frames are far under 2^20, and `b6d`/`b6e` bracket it | The whole input in every case | The whole input | **Engine policy that becomes a parameter**: Broiler.VM's `MaximumFrames` and `MaximumTrail` (`F/JsRegExpMatcher.cs:569`, `:574`), which plan task 5.2 passes to the new backend under JSD-0081's decision D10, so Broiler.VM's stop stays; Broiler.JS's frame `1 << 20` and trail `1 << 21` with a `RangeError` (D10) |
| B7 | Compilation's charge: `[\p{L}` ×50`]` under `vi` and `[\p{RGI_Emoji}` ×20`]` under `v`, Broiler.VM under fuel 200,000; and each compiled alone beside a control of the same source length with no property (`[\x41a` ×50`]` under `vi`, `[\x41abcdefghi` ×20`]` under `v`) *(run, `b7`, `b7b`-`b7e`; bisected, `b7b`-`b7e.fuel-bisect.txt`)* | Both compile and match under 200,000. **Each pattern needs exactly the smallest allowance its control needs** (`b7b` and `b7c` alike, `b7d` and `b7e` alike): compiling fifty references to `\p{L}` under `vi`, or twenty to `RGI_Emoji`, is charged what a class of plain characters is. The charge is `source.Length + 32` (`VM/JsRealm.RegExp.cs:1729`) *(reading)*; a property escape is a reference, not a copy of its ranges (`F/JsRegExpMatcher.cs:302`), and is paid per character tested (`:4486-4490`) | Both compile and match (no meter) | Compilation is not observable | **Engine policy that becomes a parameter** (D8: compile charges proportional to the expansion; R2). Not a correction of either engine |
| B8 | A flat sequence: `(?:a)` 100,000 times under `u` on 100,000 `a` *(run, `s3d`)* | The whole input | **The process aborts: stack overflow in `Matcher.ReadAt`**: each term is a continuation that calls the next (`CompileSequence`'s chain, `BR/Matching/Matcher.cs:216-267`, `:259-266`; `CompileChar` `:800-811` calls its continuation), so the match recurses once per term (the trace alternates `CompileChar` and `CompileSequence` frames down to `ReadAt` `:1012`); the guard is not on this path | The whole input | **Correction of Broiler.JS and of Broiler.Regex's preview.4** (R4: no CLR recursion in the matcher, D6) |

**Seven `RegExp` files the base reports exhausted on a load allowance: outside the matcher, no row.** Broiler.VM's
`sjl-12-001` base run reports these files exhausted on a nested-load allowance
(`src/Broiler.VM.Abstractions/VmBudgetVocabulary.cs:60`, `:63`), not on fuel or on a matcher ceiling:
`test/annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js`, `RegExp-trailing-escape-BMP.js`, and
`test/language/literals/regexp/S7.8.5_A1.1_T2.js`, `S7.8.5_A1.4_T2.js`, `S7.8.5_A2.1_T2.js` and
`S7.8.5_A2.4_T2.js` on `NestedLoadFanOut`; `test/staging/sm/RegExp/unicode-class-braced.js` on `NestedLoadBytes`.
*(Why, by reading the tests at `ccaac100`.)* The first six build a literal for every code unit from 0 to 0xFFFF
and evaluate each through its own `eval`, so they ask for more nested loads than the allowance admits; the seventh
evaluates a literal whose braced escape carries 2^24 leading zeros, a source longer than the nested-load byte
allowance. What stops them is the load, which no regular-expression code charges, so the area holds no row for
them: neither engine's matcher, nor Broiler.Regex, nor SJL-11.4's adapter spends either allowance. In the native
form one variant, `RegExp-trailing-escape-BMP.js` in strict mode, ends on the wall clock instead
(`sjl-12-001/base/hosted/wide-native.failures.txt`), a race between two allowances. Part A of Broiler.VM's rule
reads them as it reads any row: a verdict that stays is no difference, and one that moves in a file holding a
regular-expression literal is read under the rule's classes (C2 for a charge JSD-0081's decision D8 moved, C4 (c')
for the wall clock).

## F. Flags

| Row | Input | Broiler.VM | Broiler.JS | The specification | Class |
|---|---|---|---|---|---|
| F1 | Flag strings `""`, `g`, `gg`, `x`, `G`, `uv`, `vu`, every order of `dgimsuvy`, spaces and NUL; literal flags, by the three paths *(run, `f1`)* | Each valid string accepted and reported in the order `dgimsuvy`; each invalid one a `SyntaxError`, early on the literal (`VM/JsRealm.RegExp.cs:1738-1786`) | The same answers, the literal's refusals early too (`BI/RegExp/JSRegExp.cs:671-758`); messages differ | 22.2.3.1, 22.2.6.4 | **Same in both** |
| F2 | The `flags` getter over a plain object, its read order, the accessors on an instance and on the prototype, `String(new RegExp("a","yg"))` *(run, `f1`)* | The specification's answers | The same | 22.2.6.4 | **Same in both** |
| F3 | Modifiers `(?i:)`, `(?-i:)`, `(?m:)`, `(?s:)`, `(?m-s:)`, `(?i:\u212a)` under `u`; the invalid `(?u:a)` and the 25 invalid modifiers of S11 *(run, `s7`, three cultures for Broiler.JS)* | The specification's answers; every invalid modifier an early `SyntaxError` | The valid modifiers the specification's answers, in every culture; every invalid modifier refused by the constructor; `(?u:a)` refused early by the literal path; **the other invalid modifiers refused only when the literal is evaluated** (S11) | ES2025 modifiers | **Same in both** for the valid modifiers and the constructor; the literal's late refusals are S11's |

## T. Text helpers: `RegExp.escape` and `source`

| Row | Input | Broiler.VM | Broiler.JS | The specification | Class |
|---|---|---|---|---|---|
| T1 | `source` of literals with braced escapes, each beside its constructor twin with the same pattern and flags: `\u{61}` under `u`, `\u{1F600}` under `u`, `\u{D83D}` under `u`, `[\u{61}-\u{7A}]` under `v`; and the literals `/\u{3}/`, `/\\u{61}/u` *(run, `t1`)* | The pattern as written, literal and constructor alike | **The literal's `source` is rewritten** (`\u0061`, `\ud83d\ude00`, `\u{d83d}`, `[\u0061-\u007a]`; `PA/FastScanner.cs:996-1093`, `:1449-1450`); **the constructor's is the pattern as written**: the same pattern and flags give two texts | EscapeRegExpPattern (22.2.6.13.1): S may differ from P if a literal of S behaves as P does, and calls with the same P and F give the same S. Each rewritten text alone is equivalent; the literal and the constructor, given the same P and F, disagree | **Correction of Broiler.JS** (the literal and the constructor disagree for one P and F). Survey row "`source` rewriting": **confirmed** |
| T2 | `/` in a class: `/[/]/`, `new RegExp("[/]")` *(run, `t1`)* | `[\/]` for both: `RegExpSourceText` escapes every `/`, in a class too (`VM/JsRealm.RegExp.cs:1840-1842`) | `[/]` for both: `EscapePatternSource` escapes a `/` outside a class only (`BI/RegExp/JSRegExp.cs:225`) | Either text is equivalent, and each engine gives one text for one P and F | **Implementation-defined** |
| T3 | A backslash before a line terminator: `new RegExp("\\\n")`, `new RegExp("\\\u2028")` *(run, `t1`)* | **`source` holds the backslash and the raw LF, or the raw U+2028** (`VM/JsRealm.RegExp.cs:1826-1831` copies the escaped character) | `\n` and `\u2028` | Line terminators are escaped so that `/source/flags` is a literal | **Correction of Broiler.VM** (kind R: test262 `staging/sm/RegExp/escape.js` fails in Broiler.VM's `sjl-12-001` base run, "Expected SameValue(«"\\\n"», «"\\n"»)") |
| T4 | Other `source` and `toString` cases: `/`, `\/`, `a/b`, LF, CR, LS, PS, empty, the prototype, a lone surrogate, U+0085, a round trip *(run, `t1`)* | The specification's answers | The same | 22.2.6.13 | **Same in both** |
| T5 | `RegExp.escape` over 28 strings: leading ASCII letters and digits, `_`, white space (U+00A0, U+FEFF, U+1680, U+202F, U+3000), U+0085 and U+180E (not white space), LS, PS, lone surrogates, a pair, punctuators, syntax characters, control escapes, NUL, U+0080-U+0100; a number argument; length; round trips *(run, `t2`)* | The specification's answers (`VM/JsRealm.RegExp.cs:1460-1571`) | The same (`BI/RegExp/JSRegExp.cs:41-134`) | 22.2.5.1 | **Same in both** |

## U. Data use

| Row | Input | Broiler.VM | Broiler.JS | The specification | Class |
|---|---|---|---|---|---|
| U1 | `\s` against U+FEFF, U+0085, U+180E, U+00A0, U+3000, U+2029, U+1680; `\d` against U+0661; `\w` against U+00E9 and U+0130 *(run, `u1`, three cultures)* | The specification's answers | The same, in every culture | WhiteSpace and LineTerminator at Unicode 17.0.0 | **Same in both** |
| U2 | Unicode 17.0 data under `u`: `\p{L}`, `\p{Lu}`, `\p{ID_Start}`, `\p{Assigned}` against U+A7CE; Garay and Sidetic letters; the 17.0 scripts Sidetic, Beria_Erfe, Tolong_Siki, Tai_Yo accepted, `Chisoi` refused; `\p{Extended_Pictographic}` and `^\p{RGI_Emoji}$` against U+1FAE9; flag, keycap and ZWJ sequences; folding of U+A7CE, U+1C89, U+10D50, U+16EA0 under `iu`; a group named U+A7CE *(run, `u1`)* | The specification's answers | The same | UCD 17.0.0 | **Same in both** |
| U3 | U+A7CE against U+A7CF under `i` without `u`, as a literal and in a class *(run, `u1`, `m2`)* | `true` | **`false`**, in every culture: .NET's casing data under `IgnoreCase` (`BI/RegExp/JSRegExp.cs:698-701`) predates U+A7CE | Canonicalize without `u` uses toUppercase: U+A7CF's is U+A7CE, and SpecialCasing has no entry for either, so at 17.0.0 both canonicalize to U+A7CE | **Correction of Broiler.JS by data decision** (C5, SJL-D-6 (c)) |

## The survey rows, each confirmed or refuted

| Survey row | Row here | Verdict of the run |
|---|---|---|
| Duplicate group names | S1, S2a | Confirmed: Broiler.VM refuses every duplicate; Broiler.JS admits ES2025's and also an escaped duplicate in one alternative; the library refuses ES2025's under `u`/`v` |
| The 512 nesting policy | S3, S3b | Confirmed: Broiler.VM refuses from 512 nested groups; Broiler.JS has no limit and aborts the process at 200,000, with or without `u` |
| F2 | M2 | Confirmed, in every culture |
| `lastIndex` inside a surrogate pair | M4 | Confirmed: Broiler.JS matches lone halves; Broiler.VM reports the pair's start where the text reports `lastIndex`; the text is ill-formed for an empty match |
| `source` rewriting | T1 | Confirmed: the literal and its constructor twin disagree |
| The stack-guard fallback (two rows) | B5 | Confirmed, both: a .NET answer that differs, and no match |
| The budget no-match | B1 | Confirmed |
| The shortened match | B1 | Confirmed |
| The wrong index | B2 | Confirmed |
| The look-around flip | B3 | Confirmed |
| Uncharged alternation | B4 | Confirmed by reading (`Budget.Step`'s call sites); the runs are consistent, `b4c` answering what an uncharged alternation gives |
| `\0` followed by a decimal digit | S4 | Confirmed at library level, in both modes; refuted at engine level (the shell's .NET fallback answers as the specification) |
| `\k` parse-only | S5 | Confirmed at library level; not observable in Broiler.JS (its constructor and .NET refuse) |
| Escaped group names | S2, S2a, S2b | Confirmed: the library's escapes and exception kind (masked in Broiler.JS); Broiler.JS's escaped duplicate (S2a) and escaped reference (S2b) |
| Loose property names (corrected at SJL-7.5) | S6 | Confirmed corrected in Broiler.JS; still loose in the library |
| Division against a regular-expression literal | S9 | Confirmed, and wider: a valid literal is read where the grammar divides |
| `String.search`'s direct call | M6 | Confirmed, and a pattern only Broiler.Regex holds throws `ReferenceError` |
| Sticky emulation | M5 | Confirmed by reading, as cost on the .NET path; no answer differs |
| The translator's line terminators (W5) | M3 | Confirmed, and wider: `$` without `m` |

## Rows no survey named

S2b (an escaped reference refused by Broiler.JS's literal path), S5's class case (Broiler.VM accepts `[\k<a>]`
beside a named group), S7 (Broiler.JS quantifies a look-behind), S8 (Broiler.JS admits every one of test262's
breaking-change patterns but `[[]`, and two more `v`-mode classes the library refuses), S9b (Broiler.VM's scanner
faults on a literal ending in a backslash), S10 (Broiler.VM and the library refuse a lone lead escape before a
braced escape; and Broiler.JS's class that joins a lead escape to a braced trail escape), S11 (Broiler.JS reports
a literal's errors at evaluation, not early), M2a's lt-LT folding, M2b (U+1F80/U+1F88), M3's `$` without `m`, M4's
`"index"` for a non-empty match (Broiler.VM), M7 (Broiler.VM's back-reference across a pair, and Broiler.JS's
look-behind through the library), the library's valid-duplicate refusal (S1) and non-`u` `\0` answer (S4), B1's
Broiler.VM stop on `/a*/` past 2^20 characters, B6's stops reported with no dimension (and by the command-line
root as exit 7), B8 (a flat sequence aborts Broiler.JS's process), T3 (Broiler.VM's raw line terminator in
`source`), U3 (U+A7CE without `u`).

## The rows by class

JSD-0081 decided these classes on 2026-10-21.

- **Correction of Broiler.VM:** S1 (kind D); kind R: S5 (the class case), S10, M4 (the `"index"` and
  substring of a non-empty match), M7, T3. Outside SJL-11: S9b (the scanner, SJL-12/SJL-13's).
- **Correction of Broiler.JS:** S2b, S7, S8, S10 (the class `[\uD83D\u{DC38}]`), S11, M2, M2a, M2b, M3, M4
  (half-pair matches), M6, T1; with the library: S2a, S3b (200,000), M7 (the look-behind), B1, B2, B3, B5, B8;
  by data: U3. Outside SJL-11: S9 (SJL-14's).
- **Correction of Broiler.Regex's preview.4** (with the rows above that name it, and alone where Broiler.JS
  masks it): S1 (valid duplicates refused), S2, S4, S5 (parse-only), S6, S10 (the refusal), B4.
- **Implementation-defined:** T2; M4's empty match, by the owner's decision of 2026-10-21 (JSD-0081 decision
  D41) until the specification is corrected.
- **Engine policy that becomes a parameter:** S3 (nesting 512), S3b (20,000: a refusal under D7), B1's and
  B6's ceilings (Broiler.VM's frames and trail; Broiler.JS's under D10), B7 (the compile charge).
- **Same in both:** M1, M2c, M5 (answers; a resource note), F1, F2, F3 (valid modifiers and the
  constructor), T4, T5, U1, U2.
