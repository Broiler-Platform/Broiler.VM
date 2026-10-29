<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-13.1's correspondence from code: the two engines' front ends, from the tokenizer to the request

**Written:** 2026-10-26, at SJL-13.1 (work package SJL-13.1, task 1.1), as the input of the area verdict's gate X1 to
X7 for each of the area's five sub-areas (**T** the tokenizer, **P** the parser and its tree, **N** positions, **D**
diagnostics and the parse-time early errors, **R** the request, its switches, limits and providers), of the stage
table and the parse corpus's classification (task 1.2, in Broiler.JS), and of the rules of bundles `sjl-13-001` and
`sjl-13-002`, before any code of SJL-13.2 exists. **Every row was read from code at the heads below, and every row
whose answer no retained run already holds was probed in both engines; the run decides.** Where a run disagreed with
the reading, the code was read again and the row states what the run showed (section "Where the runs corrected the
reading"). **Each row carries its class**, one of: same; correction of Broiler.VM; correction of Broiler.JS;
correction of both; implementation-defined; engine policy (a parameter of the request); engine-kept (admission,
ceilings, rendering and each engine's own surfaces). No row's class is unknown. The area verdict filed with this file
records the classes; a class it changes is the verdict's.

*(Added 2026-10-27, under the owner's decisions D-asi and D-t24 of that day, which JSD-0083 records: row **P16b**,
automatic semicolon insertion in a for statement's head, is added, a correction of Broiler.JS, and P16 is narrowed to
the rest of automatic semicolon insertion; rows **T24b** and **T24f** record, beside the answer they held, Broiler.JS's
answer where `;` or the end of input follows the regular-expression literal. SJL-13.2's first runs of the `syntax`
vectors (Broiler.JS's corpus revision 29, classified at revision 30) found both. Both were read from code and probed
in both engines at the heads below on 2026-10-27, by `probes/run_p16b.sh` (section "How each probe was run"). Every
other row stands as written on 2026-10-26.)*

**Read and run at**
- **Broiler.VM `4cf9ca4`** (`4cf9ca449fc96a1a5be810f6d0ca31c42b855c8f`, the merge of #171). Every first-parent merge
  since SJL-12's after commit `1de3af6` changes only `docs/`, so the product code read and run is `1de3af6`'s.
- **Broiler.JS `d6795fc8`** (`d6795fc86ca607698983a06739ddd15b9aaa9ccd`, the merge of #1078). The parser
  (`Broiler.JavaScript.Parser`) is unchanged since SJL-12's after commit `c29d653f`; the compiler changed in #1074,
  #1075, #1076 and #1078. Broiler.JS's default branch moved to `8a95076c` while this file was written; section
  "Revisions" says why no row's answer can differ there.
- **The early-error catalogue (CAT):** Broiler.JS `Broiler.ECMAScript/docs/frontend/early-errors.tsv`, SHA-256
  `7a9109b0db685df98a7327cd95b9337cc538337f834fe4f22692b249058aa4f1`, the same at `d6795fc8` and `8a95076c`. Its site
  cells were read at its own heads (`early-errors.md` section 4) and are not edited (the plan's decision D28); the rows
  below cite the sites at the heads above.
- **The mapping (MAP) and the registry (REG):** Broiler.VM
  `src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv` and `registry.txt`, registry revision 18.
- **The parse corpus:** `src/tests/corpus/parse-1/corpus.manifest` in Broiler.VM, byte-identical to
  `conformance/parse/corpus.manifest` in Broiler.JS, SHA-256
  `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc`; test262 at
  `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

**The retained runs cited instead of a new probe** (bundle `sjl-12-001`'s after baselines in both repositories):
- **VB**: Broiler.VM `docs/evidence/sjl-12-001/after/parse-corpus-after-run1.tsv.gz`, the slice compiler's
  `--parse-corpus` at SJL-12's after commit (sub-population B1 in the bytecode form unless B2, B3 with a flag set E0 to
  E7, B4 or the native form is named; diagnostics as `code:line:column`).
- **JB**: Broiler.JS `docs/evidence/sjl-12-001/after/parse-corpus-c.tsv.gz`, SJL-12's parse-corpus tool at
  `c29d653f`: entry **P** the parser alone, entry **K** `CoreScript.Compile` (no top-level `await`), run on a 256 MiB
  thread. Its K answers hold at `d6795fc8` except for `continue` to an outer label of a label set, which #1075 changed
  (row D30), where JB is not cited.
- **VK** (also written VCK): Broiler.VM `docs/evidence/sjl-12-001/after/checks-after.log.gz`, the slice compiler's
  `--checks` lane at the product of `4cf9ca4`, cited by line.
- **VT**: Broiler.VM `docs/evidence/sjl-12-001/after/hosted/wide-bytecode.report.gz`; **JT**: Broiler.JS
  `docs/evidence/sjl-12-001/after/part-a/test262-results.json.gz`.
- **The parse-negative table**: Broiler.JS `docs/evidence/sjl-12-001/catalogue/parse-negative.tsv` (the same bytes at
  `d6795fc8`).
- A citation such as `T041058:sloppy` names one input and variant of the corpus, in VB and JB alike. Broiler.JS's
  `strict` variant prefixes the manifest's prologue line, so its positions are one line below Broiler.VM's, which forces
  strictness on the text unchanged (row N9).

**How each probe was run** (`correspondence-runs/probes/run_all.sh`, serially, one build at a time):
- **Broiler.VM**: one Release build (`--disable-build-servers`) of a detached scratch worktree at `4cf9ca4`, of three
  composition roots. The **slice compiler** ran each sub-area's probe manifest through `--parse-corpus` (B1 in both
  forms, B3 under E0 to E7 for the eval-family inputs, B4). The **conformance root** ran the value probes and the run
  probes, each as a test262 test (`flags: [raw]`, or a module test with its `_FIXTURE` neighbours), under the manifest
  `broiler.javascript.wide` in the bytecode form, fuel 200,000,000,000, wall clock 600,000 ms. The **command-line root**
  ran C01 and C02 (`--numeric --check --all`), NR01 and NR02.
- **Broiler.JS**: one Release build of a detached scratch worktree at `d6795fc8`: the shell `BroilerJS.dll`
  (`--script-host`; PR06 through `--module-host` with test262's async harness preloaded) and SJL-12's parse-corpus
  tool (`docs/evidence/sjl-12-001/tools/parse-corpus/`, entries P and K, and its `--dump` mode for T32).
- **Row T32's token dumps**: two probe programs (`probes/T/tokens/vm-tokens/`, `js-tokens/`, each project kept as
  `*.csproj.txt`), built against that engine's Release output, printing every token's fields.
- **A value probe** is one line of a file run whole as `prelude.js` + `<x>-values.js` + `epilogue.js`; each line
  prints `<label> => <value>` or `<label> => THROWS <name>: <message>`, every source under test compiled by an
  indirect eval so that one refusal cannot stop the file; a string is quoted, and a code unit outside printable ASCII,
  `"` or `\` is written `\uXXXX`. Where node 22 was run on the same line as a reading of the specification the tables
  say so; node is not an engine under test.
- Every probe ran under `TZ=UTC` with no `LANG`, `LANGUAGE`, `LC_*` or `BROILER_*` variable and under a timeout; an
  exit code is recorded, never fatal. The machine: Ubuntu 24.04.5 LTS, 4 processors, .NET SDK 10.0.401 with runtime
  10.0.12.
- **The Broiler.VM phase ran twice.** The script as first run recorded the directory it was started from, a local
  path, in the command lines of its logs; it was changed to run from its own folder (the comment at the change says
  so), and the phase was run again. The retained answers are the second run's. With that path written as `<corr>`,
  every answer and every report of the first run equals the second's; only the logs' elapsed times, restore order
  and progress lines differ.
- *(Added 2026-10-27.)* **The probes of 2026-10-27** (rows P16b, T24b and T24f) were run by `probes/run_p16b.sh`,
  which takes them as `run_all.sh` took the others, at the same heads, `4cf9ca4` and `d6795fc8`: one Release build
  (`--disable-build-servers`) of a detached scratch worktree of each, serially, Broiler.VM's of the slice compiler
  and the conformance root, Broiler.JS's of the shell and SJL-12's parse-corpus tool, each worktree's `bin/` and
  `obj/` deleted and the worktree removed after its phase. The slice compiler ran `P/probe-P16b.manifest` over every
  population (B3 compiles each input as eval code under E0 to E7) and `T/parse/t24-end.manifest` over B1 and B4; the
  conformance root ran `P/value/p-values.js` and `T/value/t24-values.js` as raw test262 tests under the same fuel and
  wall clock; the tool ran both manifests (entries P and K) and the shell both value files (`--script-host`). The
  environment, the timeouts, the machine, the .NET SDK and runtime, and the rewrite of local paths are those above.
  The answers the code reading expected are `probes/plan-p16b.tsv`, written before the run; every answer is the one
  it expected.

**References.** "The specification" is ECMA-262 ES2026 at `tc39/ecma262@0248456c` as Broiler.VM archives it
(`src/Broiler.VM.Profile.JavaScript/docs/specification/ecma-262-es2026-spec.html`). The plan is SJL-13 to SJL-16's
plan of 2026-10-26, whose decisions D1 to D74 and rules 4.1 to 4.3 the verdict and the bundles' rule files carry with
the same numbers. `survey-vm`, `survey-js`, `survey-records`, `survey-deps`, their critics `critic-vm`, `critic-js`,
`critic-records`, `critic-deps`, and `plan-critic` are the working papers behind that plan; they are not records, and
this file names their items by their own numbering, as SJL-9's correspondence named its surveys', and settles every one
that falls in the five sub-areas.

**It holds no figure of a run in prose.** The answers, inputs and code positions are in the tables. The readings'
per-input tables of the baselines are in `correspondence-runs/analysis/`, and no cell tallies them; a count that a cell
gives is of code or records (sites, catalogue rows, constants), or is an answer (a probe's diagnostics).

## Revisions: the heads read against today's default branches

- **Broiler.VM**: `origin/main` is `4cf9ca4`, the head read and run.
- **Broiler.JS**: every row was read, and every probe run, at `d6795fc8`, the head when the reading began. The default
  branch then moved to **`8a95076c`** (#1079), from which the stage table's and the records' branches are cut. #1079
  changes four files: `Broiler.JS/Broiler.JavaScript.Compiler/ForLoopPreparation.cs`, two test files of
  `Broiler.JavaScript.Compiler.Tests` and `Broiler.JS/CHANGELOG.md`. The catalogue, the scanner, the parser,
  `SyntaxValidation.cs` and every other early-error site are byte-identical at both heads. **No row's Broiler.JS
  answer can differ at `8a95076c`, so no probe was run there.** The reason: the for-loop pass's new branch runs only
  for a C-style loop whose head declares with `using` or `await using` (`VisitForStatement`, the `declaration.Using`
  arm; `VisitLabeledStatement`'s new arm acts only on a block that arm made). There it moves the loop's labels inside
  the block the pass wraps the loop in, so that a `continue` naming such a label, refused at entry K at `d6795fc8`
  ("No label found"), is accepted at `8a95076c`; nothing else moves. No probe input holds a labelled C-style
  `for (using …)` or `for (await using …)` loop (RD01's loop is `for (var …)`), and neither does any input of the parse
  corpus: the engine-origin inputs at `d6795fc8` and every file of the pinned test262 archive were searched for a label
  before such a head, and none has one, as #1079's changelog entry also says of test262. Entry P, the parser alone, is
  the same at both heads.

## The runs

[`correspondence-runs/`](correspondence-runs/) holds:
- `vm/` and `js/`: per probe `<id>.txt` (a header naming the probe, its input and the command, with the directory it ran
  in; then the probe's rows of its report, the value line, the verdict line or the program's output), and a value
  probe's lines as `TV-<line>.txt` (sub-area T) or `RE<nn>.txt` (sub-area R), so that no answer's file name differs
  from a parse probe's only in case; `reports/` (each `--parse-corpus` report, each value file's transcript, each
  conformance-root report, the token dumps), `logs/` (each command's log and each build's log), `built/` (the programs a
  value probe ran), `revision.txt`, `build.txt` and `hashes.txt` (the SHA-256 of every other file in the folder). Every
  local path a run printed is written as a placeholder: `<corr>` (the folder the probes were run from), `<probes>`,
  `<runs>`, `<work>` (the runner's scratch folder), `<vm>` and `<js>` (the two worktrees). That is the only rewrite of
  a run's output, and the runner made it before hashing.
- `probes/`: each sub-area's probe inputs, manifests (format 1 of the parse corpus's manifest, each input checked
  against its SHA-256 before the runs), value files, run probes and their test262 forms, and T32's two token-dump
  programs; `plan.tsv`, one line per probe and engine with the answer the code reading expected and the answer's
  file, and one line per row answered by a cited baseline (its `runs/<engine>/`, like a run header's `<runs>/<engine>/`,
  is this folder's `<engine>/`); `run_all.sh`, the script that ran them; `hashes.txt`. **Two
  rewrites, both of comments and local paths:** in `run_all.sh` the scratch folder, the two checkouts and the two
  worktree paths are read from the environment (`SCRATCH`, `VM_REPO`, `JS_REPO`, `VMW`, `JSW`) where the script that
  ran named them literally; the comments that named them name the variables or the checkout, the opening comment says
  how the script was written and run in place of naming a working paper, and the comment on the fix made at the run no
  longer names the path or how often the logs recorded it. In `plan.tsv` the header names the worktrees by those
  variables, the readings by name in place of their files and of the script that generated the plan, and the two
  joins' folder as this folder's `analysis/`. No other byte of either file differs from the files that ran; the rest
  of `plan.tsv` cites the readings by name (`rows-T` to `rows-R`), as they wrote their expected answers.
- `analysis/`: derived tables the rows cite, each written by the readings from the files it names and holding no local
  path: `rows-P-baseline.tsv.gz` (every input whose outcome differs between VB's B1 and JB's P or K, joined and
  assigned to a row or bucket of this file), `rows-P-agree.tsv.gz` (every test262 input the manifest calls positive
  that B1, P and K all refuse, with its row), `jsparser-anchors.tsv` and `binder-anchors.tsv` (every refusal call of
  `JsParser` and of the binder at `4cf9ca4` with the anchor it passes), `pos-compare.txt` (the first diagnostic's place
  in VB against the place in JB's P message, every row both refuse), `lt-inputs.txt` and `scan-inputs.txt` (the corpus
  inputs holding CR LF, a lone CR, LS or PS, U+FEFF, U+0085 or an astral character, and their outcomes),
  `catalogue-index.tsv` (every catalogue row, its stage cells, the rows of this file that hold its sites, and its corpus
  inputs' outcomes), `relocated.tsv` and `mapsites.tsv` (the catalogue's and the mapping's cited sites carried to the
  heads), `own-samples.tsv` and `samples.tsv.gz` (the corpus inputs tied to each catalogue row and their outcomes),
  `jsparser-refusals.tsv`, `binder-refusals.tsv` and `lowering-refusals.tsv` (every refusal site of Broiler.VM's parser,
  binder and lowering at `4cf9ca4`, with its code), and `b4-escaped-tokens.txt.gz` (every input B4 accepts in VB that
  holds an identifier token written with a `\u` escape, and each such token with its place, offsets, spelling, cooked
  name and keyword-table kind); `hashes.txt`.
- Nothing in it is over 50 MB, no file is a project file (a probe project is `*.csproj.txt`), and no file holds a NUL.
- *(Added 2026-10-27.)* **The probes of 2026-10-27 joined these folders as new files only.** In `probes/`:
  `P/probe-P16b.manifest` with its inputs under `P/inputs/` (`for-head-*.js`, `for-in-head-line-break.js`,
  `for-of-head-line-break.js`), `P/value/` (`prelude.js`, T's with only its opening comment changed, `p-values.js`,
  and T's `epilogue.js`), `T/parse/t24-end.manifest` with its inputs under `T/parse/inputs/sjl13-T/`
  (`t24b3-` to `t24b6-`, `t24f2-` and `t24f3-*.js`), `T/value/t24-values.js`, `plan-p16b.tsv` (`plan.tsv`'s columns)
  and `run_p16b.sh`; `plan.tsv`, `run_all.sh` and every earlier manifest are unchanged. In `vm/` and `js/`: each new
  probe's answer, named as above (`PP11.txt` to `PP19.txt`, `T24B3.txt` to `T24F3.txt`, `PE01.txt` to `PE09.txt` for
  P's value lines and `TV-T24b3.txt` to `TV-T24f3.txt` for T's), the reports `reports/P16b-parse.tsv`,
  `T24-end-parse.tsv`, `P16b-values.transcript.txt` and `T24-end-values.transcript.txt`, the logs
  `logs/P16b-parse.log`, `T24-end-parse.log` and `p16b-*.log` (the builds and the worktree), the programs
  `built/p-values*` and `built/t24-values*`, and `p16b-revision.txt` and `p16b-build.txt` beside `revision.txt` and
  `build.txt`. The new worktree logs, `vm/logs/p16b-worktree.log` and `js/logs/p16b-worktree.log`, hold git's
  carriage returns as the earlier ones do. The `hashes.txt` of `probes/`, `vm/` and `js/` were written again, by the
  runner's own function, over every file each folder holds: every earlier line is unchanged and the new files' lines
  are added. No earlier file changed, and `analysis/` is untouched.

## T. The tokenizer

**Where each engine's code is.** Broiler.VM's tokenizer is `SliceTokenizer`, which the wide path and the slice path
both construct (the wide path with structured templates and HTML-like comments set from the goal); `JsParser` re-reads
some lexical facts (template cooking, escaped reserved words, strict legacy octal). Broiler.JS's is `FastScanner` with
`CharExtensions`, read on demand by `FastTokenStream`; some lexical rules are checked by the compiler's
`SyntaxValidation` (stage E below). Both read Broiler.Unicode 17's lexical classes and `NumericLiterals.Evaluate`.

**Abbreviations in this section.** Broiler.VM, under `src/Broiler.VM.Profile.JavaScript.Compiler/`: `VMT` =
`SliceTokenizer.cs`, `VMJP` = `JsParser.cs`, `VMJC` = `JsCompiler.cs`, `VMSS` = `SliceSourceCompiler.cs`. Broiler.JS:
`JSS` = `Broiler.JS/Broiler.JavaScript.Parser/FastScanner.cs`, `JSX` = `…/Parser/CharExtensions.cs`, `JSP.<x>` =
`…/Parser/FastParser.<x>.cs`, `JSV` = `Broiler.JS/Broiler.JavaScript.Compiler/SyntaxValidation.cs`, `JSC.<x>` =
`Broiler.JS/Broiler.JavaScript.Compiler/Expressions/FastCompiler.<x>.cs`, and `JSC/` = `Broiler.JS/Broiler.JavaScript.Compiler/`.
**Stage E** means that Broiler.JS accepts at entry P and refuses at entry K (the check lives in `JSV` or the compiler):
the stage table's matter (task 1.2), not a difference of outcome. "The spec" is the specification's answer; where
node 22 was run on a value line it agrees with the specification except where the tables say.

### The rows

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| T1 | WhiteSpace: TAB VT FF SP NBSP ZWNBSP and Zs; U+0085 and U+180E are not | `VMT:2916` IsWhiteSpace over the owner's predicate (`JsCompilerUnicode.cs:48-53`, Broiler.Unicode 17) | `JSS:204-207` IsWhiteSpace (provider or `JSUnicode`), loop `:429-442` | U+FEFF between tokens; U+0085, U+180E refused 2001 | the same (U+0085 settled at SJL-7.5) | baseline T051983 (U+180E refused both), T051966, T051955 (agree), T051937; values T01a-f | same |
| T2 | LineTerminator LF CR LS PS, CRLF as one; the line-break facts ASI and `-->` read | `VMT:2851` AdvanceLine, `PrecededByLineTerminator` on SliceToken `:377-388` | `JSX:90` IsLineTerminator, `JSS:432-442` one LineTerminator token per run | as the spec | as the spec | baseline T041026, T041056, T041058, T041585 (agree); values T14b, T14c | same |
| T3 | Comments: `//`, `/* */` (one holding a line terminator counts as one), unterminated refused | `VMT:909-950` SkipTrivia | `JSS:1660-1677` SkipSingleLineComment (returns a LineTerminator token), `:1601-1657` SkipMultilineComment | as the spec; unterminated 2002 | as the spec | baseline T028581 (VM 2002:15:1, JS refused), T028583, T028626 (agree); value T01e | same |
| T4 | HTML-like comments in the source text (Annex B): Script goal only; `<!--` anywhere a token may start; `-->` at a line start or the input's start | `VMT:878-895` in SkipTrivia, `lineHasOnlyTrivia :621`, switch `:601-613` set from the goal at `VMJC:836` and `:1072` | `JSS:762-774` (`-->` only after an Empty or LineTerminator token, `:774`), switch `:45` set `!IsModule` at `:166` | as the spec | as the spec | baseline T000242-T000249 (accepted both, first-line forms included), T041620-T041622 (module: refused both) | same |
| T5 | Hashbang at offset zero of a Script, Module or eval text; not in a Function body; escaped `#\!` refused | `VMT:838-851` SkipHashbangComment (`index == textStart`) | `JSS:997-1000` IsHashbangStart (`position == 0`) | as the spec | as the spec | baseline T028592, T028602, T028603, T028605, T028608, T028615, T028619, T028620 (agree) | same (a Script, a Module, indirect eval code and a sloppy caller's direct-eval text; a strict caller's direct eval in Broiler.JS is N26's) |
| T6 | `#!` and `-->` right after a substitution's `${` | `VMT:776-780` each substitution is read by an inner tokenizer whose text starts after `${`, so SkipHashbangComment and `lineHasOnlyTrivia` (true at a text start) treat it as a source start; the remark `:828-833` keeps it on purpose | `#` is a Hash token (`JSS:486-489`); `-->` is not after a LineTerminator, so `--` `>` | accepted (`${#!⏎1}` and `${-->⏎1}` are `1`) | refused (parser) | probes T06A, T06B (T06C control); values T06a-c (node: SyntaxError for a, b) | correction of Broiler.VM |
| T7 | The template-end scan and HTML-like comments in a substitution | `VMT:2347-2507` ScanTemplateBody has arms for `//` and `/* */` but none for `<!--` or `-->`, so a `}` inside one closes the substitution | real tokens; the comment is skipped by the scanner; brace depth `JSS:517-537` | `` `${1 <!-- }⏎}` `` is `"1\n}"` (a different template, no error) | `"1"` | values T07a, T07b | correction of Broiler.VM |
| T8 | The template-end scan's regex-or-division guess | `VMT:2449-2456`: a `/` starts a regex only if the previous significant character is one of `StartsRegularExpression` `:2714-2716`; after `typeof`, `return` (a letter) it is division, so a `}` or a backtick in the literal ends the substitution or opens a template | real tokens; `typeof`, `return` are in the keyword list `JSS:1268-1284` | `${typeof /}/}` refused 2006 (from the substitution's run, deferred); `${…return /`/…}` refused 2007 | accepted | probes T08A, T08B; values T08a, T08b | correction of Broiler.VM |
| T9 | When a lexical refusal is reported | the whole input is tokenized first and any tokenizer diagnostic stops the unit before the parse (`VMJC:841-847`); refusals inside substitutions are kept (`VMT:785-789`) and re-reported where the parse reaches them (`VMJP:6997-7006`); `@` is a tokenizer refusal 2104 (`VMT:2756-2769`) | tokens on demand, one or two ahead; a scan error is thrown when the scan reaches it | `var 1; "\u{110000}";` first 2005:1:8; `var 1;⏎@dec…` first 2104:2:1 | both refused at the `1` (1:5) | probes T09A, T09B | implementation-defined (the order of diagnostics; D17's S6 and (G) classes carry the move at step 1) |
| T10 | Identifier characters: ID_Start/ID_Continue, `$`, `_`, ZWNJ, ZWJ, astral by surrogate pair | `VMT:998-1064` ReadIdentifierOrKeyword, `:2952`/`:2971` IdentifierStart/PartWidth, `:2984` CodePointAt (Broiler.Unicode 17) | `JSS:1723-1786` ReadIdentifier, `:209-215` (provider or `JSUnicode`) | as the spec | as the spec | baseline T040575, T040639 (accepted both), T040800 (refused both) | same |
| T11 | `\u` escapes, well formed: `\uHHHH`, `\u{H…}` with leading zeros up to U+10FFFF; an escaped identifier character must itself be ID_Start/ID_Continue | `VMT:1087-1129` ReadIdentifierEscape, `:1165-1209` ReadUnicodeEscapeValue | `JSS:1788-1841` ReadIdentifierEscape, `:1002-1038` ScanUnicodeCodePointEscape | as the spec | as the spec | baseline T040642 (accepted), T041590 (refused both); values T11a, T11b | same |
| T12 | `_` among an escape's hex digits | .NET `int`/`ushort`/`byte.TryParse(…, HexNumber)` (`VMT:1180`, `:1196`, `:1723`) and `Uri.IsHexDigit` in template cooking (`VMJP:6816-6891`) refuse it: 2005 | `JSX:40-47` IsDigitPart returns true for `_` whatever the radix, and `JSX:17-36` HexValue(`_`) is 0; used by ScanHexEscape `JSS:963-993` (strings, and template cooking through ScanEscaped), ScanUnicodeCodePointEscape `:1002-1038` and ReadIdentifierEscape | refused 2005 (untagged and string); tagged cooks undefined | accepted: `"\u{0_41}"` is `"A"`, `"\x_1"` is U+0001, and a tagged template cooks U+0001 where the spec has undefined | baseline T040667 (identifier `\u{00_76}`: VM 2005:48:5, JS P and K accepted); probes T12A, T12B, T12C, T12E; values T12a-d | correction of Broiler.JS |
| T13 | ASCII white space inside `\x`/`\u` escapes | the same three .NET parses take `NumberStyles.HexNumber`, which allows leading and trailing white space (U+0009-U+000D, U+0020): `"\x 1"`, `"\x1 "`, `"\u 041"`, `a\u 062` and `"\x⏎1"` (a raw LF inside a string, consumed by the escape with no AdvanceLine) are accepted; template cooking (`VMJP:6816-6891`, `Uri.IsHexDigit`) is strict | ScanHexEscape and ReadIdentifierEscape take hex digits only | accepted on the wide path (B1). On the slice path (B4) the tokenizer accepts every form too (no 2005, where T12's malformed escapes give 2005 at B4), but the slice parser makes a string literal in an expression a `StringValue` construct (`SliceParser.cs:1443-1448`), which the slice manifest refuses (`SliceSourcePrograms.cs:273`, "refuse-a-string-value"; only a directive's string is admitted), so B4 refuses the four string forms with 2104 at the literal (1:9) and accepts the identifier form `a\u 062` (T13C) | refused | probes T13A-T13F (T13E the strict template control); values T13a-f | correction of Broiler.VM |
| T14 | String escapes and values: single escapes, Annex B legacy octal and `\8` `\9` in sloppy code, `\0`, line continuations, raw LS and PS | `VMT:1597-1772` ReadStringLiteral (octal `:1674-1699`) | `JSS:776-994` ScanEscaped (Annex B octal `:932-950`), `:1679-1720` ReadString | as the spec | as the spec | baseline T041534, T041557, T041581, T041584, T041585 (agree); values T14a-c | same |
| T15 | Strict refusal of legacy octal and non-octal decimal escapes in strings | `VMJP:7238-7262` (Advance, RefuseLegacyEscape) and ParseDirectives' retroactive check `:716-731`: 2210 at the parse | `JSV:34-36` with `:369-396` (whole-program strict) and the validator's VisitLiteral `:1038-1048` (strings in any strict scope): refused at K | refused 2210 | P accepts, K refuses (stage E) | baseline T041574, T041580 (VM 2210, JS P=a K=r) | same (outcome); stage differs |
| T16 | Strict refusal of legacy octal and non-octal decimal integer literals (`010`, `08`, `08.5`) | the literal `VMJP:5822-5828` refuses 2210 when `token.IsLegacyOctal`; PropertyKey's numeric arm `VMJP:6340-6352` has no such check (object keys, class element names) | only whole-program strictness: `JSV:34-36` re-scans the text, `TryGetLegacyOctalToken :398-408` counts a second character 0-7 only (so `08`, `09`, `08.5` pass: K1); no check for a class body's strictness (`JSV:1038-1048` checks strings only). A function's own strictness **is** checked: `CreateFunction` (`JSC/Declarations/FastCompiler.CreateFunction.cs:251`) calls `ValidateFunctionEarlyErrors` (`JSC/FastCompiler.EarlyErrors.cs:13-31`), which for a strict function runs `LegacyOctalLiteralDetector` (`:99-126`) over the body; it visits every numeric literal, an object key included, through the same `TryGetLegacyOctalToken` (`:128`, 0-7 only) | strict `{ 010: 1 }` accepted; class `010(){}` accepted; strict `08.5` 2210; function-strict key accepted; function-strict `return 010` 2210 | strict `{ 010: 1 }` P=a K=r; class accepted; strict `08.5` accepted; both function-strict forms P=a K=r ("Unexpected legacy octal literal in strict mode", stage E) | baseline T041204 (VM 2210, JS P=a K=r), T041126, T041212 (strict `08`: VM 2210:39:1, JS accepted); probes T16A, T16B, T16D, T16E, T16F | correction of both (Broiler.VM: a legacy octal object key or class element name in strict code; Broiler.JS: K1's `08`, `09` and `08.5`, and a class body's strictness. A function's own strictness Broiler.JS checks at K, by `FastCompiler.EarlyErrors`) |
| T17 | Numeric literals: separators, legacy octal and non-octal decimal (sloppy), BigInt shapes, `07.5`, an IdentifierStart or digit right after a literal | `VMT:1297-1460` ReadNumericLiteral, `:1476-1486` value through `NumericLiterals.Evaluate`, `:1490-1540` FinishNumeric (touching identifier `:1526-1529`), `:1554-1572` BigIntShapeRefusal | `JSS:1846-1971` ReadNumber, CommitNumberToken `:1874-1895` (`$` and `@` exempt from the touching check `:1877`), value `NumericLiterals.Evaluate(strict: false)` `:1885` | as the spec; `07.5` and `3$` refused 2003 | as the spec; `07.5` refused (library); `3$` refused by the parser (`3` then the identifier `$`) | baseline T041195, T041205, T041213, T041214, T041258, T041061, T041063, T041070 (agree); probes T17A, T17B; values T17a, T17b | same (`3$`: stage differs) |
| T18 | The `m` suffix | FinishNumeric's touching check: 2003 | `JSS:1954-1955` `m` after the integer and fraction (not after an exponent or a radix prefix) commits `TokenTypes.Decimal` without NumericLiterals.Evaluate; `JSP.Literal:23`, `JSP.Expression:36`, `JSC.VisitLiteral:27-28` (JSDecimalBuilder.New) | `1m`, `08m`, `1e3m` refused 2003 | `1m`, `08m` accepted (a Decimal); `1e3m` refused. `String(1.5m)` is `"1.5n"`: `JSDecimal.ToString` appends `n` (`Broiler.JavaScript.BuiltIns/Decimal/JSDecimal.cs:76`) | probes T18A-T18C; value T18a | engine policy (the lexer-extension parameter, SJL-D-17, D17) |
| T19 | The decimal-digit lookahead after `?.` and `.` | `VMT:2737-2741` ASCII digits only | `char.IsDigit` (every Nd digit) at `JSS:679` and `:703` (K2) | refused 2001 at the U+0663 | refused (`.٣` read as a number the library refuses) | probes T19A, T19B; value T19a | correction of Broiler.JS (diagnostic only; the outcome is the same) |
| T20 | Punctuators, longest match; `..` is none | `VMT:2727-2782` ReadPunctuator; the second `.` is refused by `JsParser.AfterDot`, "a property name after `.` is an identifier name" (`VMJP:6285-6298`, 2101 `UnexpectedToken`) | `JSS:415-759` (`..` throws `:711-716`) | as the spec (`a..b` is refused with 2101 at the second `.`, then 2102) | as the spec | baseline T042343, T042344 (agree); probe T20A | same |
| T21 | A private name `#x` is one token | `VMT:1002-1008` reads `#` and the name together | `#` is a Hash token (`JSS:486-489`), joined with the next identifier by the parser with no adjacency check (`JSP.PropertyName:68-76`, `JSP.SingleMemberExpression:176-188`, `JSP.SingleExpression:185`) | `# x` refused 2102 | `# x` accepted | baseline T034993-T035010, T046794-T046811 (VM 2102, JS P and K accepted) | correction of Broiler.JS |
| T22 | Decorators `@` | the tokenizer refuses `@` with 2104 over the whole input (`VMT:2756-2769`); mapping `frontend-mapping.tsv:354` JSE7201/EP0009 → 2104 within meaning, registry EX0017 `:93` | parsed (`JSS:725-726`, `JSP.Decorator:28-40`) | refused 2104 | accepted | baseline T031800-T031807, T043428-T043439, T052001-T052002 (VM 2104, JS accepted); probe T09B (the stage) | engine policy (a proposal flag, off by default in the contract; D17 moves the refusal's stage: S6, (G)) |
| T23 | Escaped reserved words and contextual keywords | the token carries IsEscaped; the parser refuses an escaped word where it would be a keyword, and escaped `await`/`yield` where reserved: `VMJP:6575-6598` RefuseEscapedReservedWord and the contextual sites `:1510`, `:1603`, `:1622`, `:2054`, `:5625`, `:5666` (2209) | `JSS:2042-2140` CommitIdentifier: escaped `yield`, `await`, `let`, `static` and others are "OK as identifiers" whatever the context (`:2066-2084`) | refused 2209 | accepted | baseline: the `*-escaped` rows where they differ (e.g. T030364, T030976: VM 2209, JS accepted) | correction of Broiler.JS |
| T24a | Regex or division after a line terminator or a line comment | the previous significant token decides (`VMT:1804-1826`); trivia is not a token | the LineTerminator token is `lastToken` (`JSS:409-411`; a line comment returns one, `:1676`) and falls to the default (scan a regex) in ReadCommentsOrRegExOrSymbol `JSS:1222-1287` | `a⏎/b/g` divides | `a⏎/b/g` is `a; /b/g` (a different program, no error) | values T24a, T24a2 (VM 1, JS 6) | correction of Broiler.JS |
| T24b | After a `}` that closes an expression (object literal, function expression) | bracket stack: `VMT:1944-2087` Follow, OpeningForBrace, `:2130` OpensBlock; the brace's EndsStatement decides | CurlyBracketEnd falls to the default: a regex is tried first and kept if it scans. The parser then takes the regular-expression token as a binary operator: after an operand, `FastParser.Expression.cs:266-296` refuses a following token only if it is the first token's kind, an identifier or `IsLiteralValueStart` (`:33-40`), which omits `RegExLiteral`, so `NextExpression` consumes it as the operator; the compiler refuses the node, "Undefined binary operation RegExLiteral" (`JSC/Expressions/FastCompiler.VisitBinaryExpression.cs:163`). *(Added 2026-10-27, decision D-t24.)* Where `;` or the end of input follows the literal, `NextExpression` consumes the token (`FastParser.NextExpression.cs:218`), finds no right operand, is not refused because the token has no operator precedence (`:227-228`; `OperatorPrecedence` `:466-492`, `_ => int.MaxValue`), and ends at `EndOfStatement` (`:230-234`, `FastParser.cs:207-226`); `Expression` keeps the left operand alone (`FastParser.Expression.cs:301-304`), so the literal is dropped | division (also where `;` or the end of input follows: NaN, *added 2026-10-27*) | a regex, P accepted, K refused "Undefined binary operation RegExLiteral" (stage E). *(Added 2026-10-27, decision D-t24.)* Where `;` or the end of input follows the literal: P and K accepted, and the program completes with the left operand's value, the object or the function, where the specification divides (NaN) | probes T24B, T24B2; value T24b; *added 2026-10-27*: probes T24B3 to T24B6, values T24b3 to T24b8 (T24b7 and T24b8 are the sources of the vectors `syntax/lexical/regexp-delimiting/0047` and `0046`) | correction of Broiler.JS (the answer added 2026-10-27 has the same class, D-t24) |
| T24c | After `of` in a for-of head, `extends`, `export default` | `of` in a for head reads a regex (Follow's `Of` arm); `extends`, `default` fall to `_ => true` | identifier-keywords not in the list `JSS:1268-1284` read division | a regex | division (refused) | probes T24C1-T24C3; values T24c1, T24c2 | correction of Broiler.JS |
| T24d | After a `for await (…)` head | Opening.Head needs If/While/For/With before `(` (Follow `VMT:1971-1972`); after `await` it is no head: division | OpensStatementHead `JSS:220-223` is the same set: division | refused | refused | probe T24D | correction of both |
| T24e | After prefix `++`/`--` | PlusPlus/MinusMinus read division (`VMT:1818`) | Increment/Decrement read division (`JSS:1258-1265`: "no valid program has a regex here"; `++/x/.lastIndex` is one) | refused | refused | probe T24E; value T24e (node 1) | correction of both |
| T24f | After `await` used as an identifier (Script) | Await falls to `_ => true`: a regex | `await` is in the keyword list (`JSS:1275`): a regex, which the parser then takes as a binary operator (as T24b); *(added 2026-10-27, decision D-t24)* where `;` or the end of input follows it, the token is dropped and the left operand kept, by T24b's sites | refused (also where `;` or the end of input follows: B1 2102:1:36, *added 2026-10-27*) | refused (P accepted, K refused "Undefined binary operation RegExLiteral", stage E). *(Added 2026-10-27, decision D-t24.)* Where `;` or the end of input follows the literal: P and K accepted, and the program completes with `await`'s value (6) where the specification divides (1) | probe T24F; value T24f (node 2); *added 2026-10-27*: probes T24F2, T24F3, values T24f2, T24f3 (T24f3 is the source of the vector `syntax/lexical/regexp-delimiting/0052`) | correction of both (the answer added 2026-10-27 has the same class, D-t24) |
| T24g | After `yield` | InGenerator `VMT:2179` (nearest function body, arrows included) | YieldIsKeyword set by the parser (`Parser/FastParser.cs:66-70`, `JSP.Function:127`, arrows `JSP.Expression:212`) | as the spec | as the spec | probes T24G1-T24G3; values T24g1-g3 | same |
| T24h | After a statement head's `)`, a block's `}`, a function declaration's `}` | Head and EndsStatement in Follow | parenIsStatementHead `JSS:500-509`; `}` reads a regex by default | a regex | a regex | probe T24H; value T24h | same |
| T24i | A literal that fails to scan or validate | reported as the literal's refusal (2006, or the pattern's later, D7) | ScanRegEx returns false (including the validator `:1505-1506` and flags) and the `/` is re-read as division (`JSS:1313-1329`); the parser then refuses | refused | refused, "Unexpected token …" | baseline T041390, T041391, T041392 (VM 2101/2102, JS "Unexpected token Identifier: G"), T041418, T041420 | correction of Broiler.JS (diagnostic; JSD-0081 D22 keeps it SJL-14's, S9) |
| T25 | The regular-expression body and flags as tokens (classes, escapes, no line terminator; flags IdentifierPart) | `VMT:2214` ReadRegularExpressionLiteral, `:2643-2696` ScanRegularExpressionBody (flags `:2684-2687`); split at `VMJP:5933-5943`; pattern and flag checks later (R and D) | `JSS:1331-1510` ScanRegEx, `:1512-1597` ScanFlags, the parenthesis patch `:1463-1492` (deleted at SJL-11.5, D22) | as the spec | as the spec | baseline T041299, T041490, T041518 (accepted both), T041056, T041390-T041392 (refused both) | same (the pattern's early errors are R's and D's) |
| T26 | `source` of a `u`/`v` literal holding `\u{…}` | the body text is kept (`VMJP:5933-5943`) | `JSS:1048-1145` DecodeBracedUnicodeEscapes, applied `:1501-1502`, rewrites the pattern the token carries; `JSC.VisitLiteral:30-31` passes it to the RegExp | the body text kept: `\u{1F600}`, `\u{61}`, `\u{D83D}` (TV-T26a to c) | the pattern text rewritten: `\ud83d\ude00` for `\u{1F600}`, `\u0061` for `\u{61}`, `\u{d83d}` for `\u{D83D}` (TV-T26a to c) | values T26a-c | correction of Broiler.JS (K6; deleted at SJL-11.5 by JSD-0081 D22, else SJL-14) |
| T27 | Template cooking and raw values: escapes, CR and CRLF to LF in both, invalid escapes undefined when tagged and refused when untagged | `VMJP:6708-6797` ParseTemplate (CR to LF `:6776-6781`), `:6816-6891` CookEscape, `:6896-6944`, `:6969-6974` Normalise | `JSS:1147-1213` ReadTemplateString (cooked in the scanner, CR to LF `:1195-1203`, invalid deferred), `JSC.VisitTaggedTemplateExpression:22-43` | as the spec | as the spec; untagged invalid refused at K | baseline T039968, T040021, T040023 (agree); T039979, T039984, T040033 (VM 2005, JS P=a K=r) | same (outcome; stage differs), apart from T12, T13 and T28 |
| T28 | Raw strings of a tagged template | raw from the chunk text | `JSC.VisitTaggedTemplateExpression:71-80`: `r.TrimStart('}')` (`:74-75`) drops every leading `}` of a segment, not only the substitution's closing brace, and of the first segment too | `String.raw` keeps them | `"1x2y"` for `${1}}}x${2}}y`, `"a1"` for `}}a${1}` | values T28a-d (T28b the cooked control) | correction of Broiler.JS (K3) |
| T29 | Template structure: nesting, `$` alone, braces in a substitution, unterminated | `VMT:2277` ReadTemplateLiteral, ScanTemplateBody, TemplateNestingLimit `:589` | ReadTemplateString, brace depth `JSS:517-537` | as the spec | as the spec | baseline: test/language/expressions/template-literal and tagged-template agree but for T27's stage rows | same (outside T6-T8, T12, T13, T28) |
| T30 | HTML-like comments on the slice path | `VMSS:209` constructs `new SliceTokenizer(source)` with HtmlLikeComments at its default, off, for the Script goal too | the switch follows the goal (`JSS:166`) | B1 accepts `-->` and `<!--` comments; B4 refuses them (2101) | accepted | probes T30A, T30B (B4) | engine policy (a request parameter, `design.md:39`; the slice path's setting is SJL-15's); see finding F17 |
| T31 | What a token carries | SliceToken `VMT:377-388`: kind, raw, values, line, column, offset, end, PrecededByLineTerminator, IsLegacyOctal, IsEscaped. **RawText is the source between Offset and End for every kind but one**: numbers `VMT:1518-1535`, strings `:1618-1771`, regular expressions `:2223-2236`, templates `:2287-2309`, punctuators `:2745`; **an escaped identifier's RawText is its cooked name** (`:1054-1063`, remark `:355-363`), and its StringValue is empty. IsEscaped is set on identifiers only (strings pass `false`, `:1618-1771`); IsLegacyOctal on numbers and strings | `Broiler.JavaScript.Ast/Misc/FastToken.cs:8-49`: `Span` is always the source spelling (`:17`) and `CookedText` the cooked value (`:19`: an escaped identifier's name, a string's value); no legacy-octal flag (`JSV` re-scans the raw text); no general escaped flag (`IsEscapedReservedWord` `:24` for reserved words only; `CookedText != null` marks an escaped identifier) | - | - | T32's token probes (T32A-T32C, VM-tokens and JS-tokens) | implementation-defined (the token's representation; no outcome differs): the two tokenizers keep different fields for one fact, so part F compares them only through a stated map (T32, F17) |
| T32 | An escaped identifier's token: raw text, kind, value and the escaped flag | `ReadIdentifierOrKeyword` `VMT:997-1064`: the name is built cooked (`ReadIdentifierEscape` `:1087-1129`), `escaped = index - start != text.Length` (`:1052`), kind `isPrivate \|\| escaped ? Identifier : KeywordKind(text)` (`:1055`), RawText the cooked name (`:1056`), StringValue empty (`:1058`), IsEscaped (`:1063`); Offset and End set by the caller to the escaped spelling (`:690-691`). `JsParser` reads RawText as the name and asks `KeywordKind(token.RawText)` for an escaped word (`RefuseEscapedReservedWord` `VMJP:6575-6598`) | `ReadIdentifier` `JSS:1723-1784` builds the cooked name only when an escape occurs; `CommitIdentifier` `JSS:2042-2140`: `Span` the source slice, `CookedText` the name, `isKw = cooked == null && …` (`:2052`), so an escaped word has no keyword and no contextual-keyword fact; a reserved word gets `IsEscapedReservedWord` (`:2059-2090`); the tree's name is `CookedText ?? Span` (`AstIdentifier.cs` ctor, `FastParser.Identifier.cs:42`) | `implements`: Identifier, RawText `implements`, source `implements` (24..39), escaped; `of`: Identifier (plain `of` is Of), RawText `of` | `implements`: Identifier, Span `implements` (24..39), CookedText `implements`, IsKeyword false (plain: true, `implements`); `of`: Identifier, CookedText `of`, ContextualKeyword none (plain: `of`) | outcome: baseline T040442, T040533, T040535, J001138 (B1, B4, P and K accepted); fields: probes T32A, T32B, T32C (token dumps, VM and JS; JS tree dump) | same (outcome and bound name); the token's fields are implementation-defined (T31), so part F compares them only through a stated map (F17). Every B4-accepted input of VB that holds an escaped identifier is listed in `analysis/b4-escaped-tokens.txt.gz`, the first T040442 (`{ let implements = 4; }`) |

### The probes, and both engines' answers as run

**Parse probes** are the rows of `T/parse/corpus.manifest` (corpus `sjl13-T`), run by the slice compiler's
`--parse-corpus` with populations B1 and B4, and by SJL-12's tool (P and K). **Value probes** are the lines of
`T/value/t-values.js`, each named here by its label (its answer file is `TV-<label>.txt`). **Token probes** are the
three rows of `T/tokens/corpus.manifest`, run like the parse probes and through the two token-dump programs. How to read
an answer: Broiler.VM's `B1 refused 2104:2:1` is the first diagnostic of every variant and form (a bracket names the
variants or flag sets an answer holds for when they differ), `(+n)` counts the diagnostics that follow it in the list,
and B4 is the slice front end, whose answer for a probe of the wide grammar is the slice manifest's, not a tokenizer's;
Broiler.JS's `P` and `K` give the outcome and, for a refusal, the message's first line. A value is printed as the
value probes print it. Rows T2, T4, T5, T10, T15, T21, T23, T24i, T27 and T29 have no probe and are
answered by the baselines their rows cite; the other rows' baselines stand beside their probes, and the hand-offs need
no probe. *(Added 2026-10-27: the rows of `T/parse/t24-end.manifest` (corpus `sjl13-T24-end`, B1 and B4, P and K) and
the lines of `T/value/t24-values.js` (each answer `TV-<label>.txt`), run the same way that day, are rows T24b's and
T24f's probes with `;` or the end of input after the literal, decision D-t24's; they are the rows below marked so.)*

| Probe | Row | Input (`correspondence-runs/probes/`) | Broiler.VM, as run | Broiler.JS, as run |
|---|---|---|---|---|
| T06A | T6 | `T/parse/inputs/sjl13-T/t06a-hashbang-at-substitution-start.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Negate: ! at 1, 13"; K refused "Unexpected token Negate: ! at 1, 13"` |
| T06B | T6 | `T/parse/inputs/sjl13-T/t06b-html-close-at-substitution-start.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Greater: > at 1, 14"; K refused "Unexpected token Greater: > at 1, 14"` |
| T06C | T6 control | `T/parse/inputs/sjl13-T/t06c-html-close-at-line-start-in-substitution.js` | `B1 accepted; B4 refused 2104:1:9` | `P accepted; K accepted` |
| T08A | T8 | `T/parse/inputs/sjl13-T/t08a-typeof-regex-brace-in-substitution.js` | `B1 refused 2006:1:19; B4 refused 2104:1:9` | `P accepted; K accepted` |
| T08B | T8 | `T/parse/inputs/sjl13-T/t08b-return-regex-backtick-in-substitution.js` | `B1 refused 2007:1:9; B4 refused 2007:1:9` | `P accepted; K accepted` |
| T09A | T9 | `T/parse/inputs/sjl13-T/t09a-parse-error-before-lexical-error.js` | `B1 refused 2005:1:8; B4 refused 2005:1:8` | `P refused "Unexpected token Number: 1 at 1, 5"; K refused "Unexpected token Number: 1 at 1, 5"` |
| T09B | T9, T22 | `T/parse/inputs/sjl13-T/t09b-parse-error-before-decorator.js` | `B1 refused 2104:2:1; B4 refused 2104:2:1` | `P refused "Unexpected token Number: 1 at 1, 5"; K refused "Unexpected token Number: 1 at 1, 5"` |
| T12A | T12 | `T/parse/inputs/sjl13-T/t12a-string-braced-escape-underscore.js` | `B1 refused 2005:1:9; B4 refused 2005:1:9` | `P accepted; K accepted` |
| T12B | T12 | `T/parse/inputs/sjl13-T/t12b-string-hex-escape-underscore.js` | `B1 refused 2005:1:9; B4 refused 2005:1:9` | `P accepted; K accepted` |
| T12C | T12 | `T/parse/inputs/sjl13-T/t12c-template-hex-escape-underscore.js` | `B1 refused 2005:1:9; B4 refused 2104:1:9` | `P accepted; K accepted` |
| T12E | T12 | `T/parse/inputs/sjl13-T/t12e-string-unicode-escape-underscore.js` | `B1 refused 2005:1:9; B4 refused 2005:1:9` | `P accepted; K accepted` |
| T13A | T13 | `T/parse/inputs/sjl13-T/t13a-string-hex-escape-leading-space.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Assign: = at 1, 12"; K refused "Unexpected token Assign: = at 1, 12"` |
| T13B | T13 | `T/parse/inputs/sjl13-T/t13b-string-unicode-escape-leading-space.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Assign: = at 1, 12"; K refused "Unexpected token Assign: = at 1, 12"` |
| T13C | T13 | `T/parse/inputs/sjl13-T/t13c-identifier-unicode-escape-leading-space.js` | `B1 accepted; B4 accepted` | `P refused "Unexpected token Identifier: var at 1, 8"; K refused "Unexpected token Identifier: var at 1, 8"` |
| T13D | T13 | `T/parse/inputs/sjl13-T/t13d-string-hex-escape-trailing-space.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Assign: = at 1, 13"; K refused "Unexpected token Assign: = at 1, 13"` |
| T13E | T13 control | `T/parse/inputs/sjl13-T/t13e-template-hex-escape-leading-space.js` | `B1 refused 2005:1:9; B4 refused 2104:1:9` | `P accepted; K refused "Invalid escape sequence in template literal"` |
| T13F | T13 | `T/parse/inputs/sjl13-T/t13f-string-hex-escape-line-feed.js` | `B1 accepted; B4 refused 2104:1:9` | `P refused "Unexpected token Assign: = at 1, 12"; K refused "Unexpected token Assign: = at 1, 12"` |
| T16A | T16 | `T/parse/inputs/sjl13-T/t16a-strict-legacy-octal-property-key.js` | `B1 accepted; B4 refused 2104:1:9 (+1)` | `P accepted; K refused "Unexpected legacy octal literal in strict mode"` |
| T16B | T16 | `T/parse/inputs/sjl13-T/t16b-class-legacy-octal-method-name.js` | `B1 accepted; B4 refused 2104:1:1 (+1)` | `P accepted; K accepted` |
| T16D | T16 | `T/parse/inputs/sjl13-T/t16d-strict-non-octal-decimal-fraction.js` | `B1 refused 2210:1:9; B4 accepted` | `P accepted; K accepted` |
| T16E | T16 | `T/parse/inputs/sjl13-T/t16e-strict-function-legacy-octal-key.js` | `B1 accepted; B4 refused 2104:1:1 (+4)` | `P accepted; K refused "Unexpected legacy octal literal in strict mode"` |
| T16F | T16 control | `T/parse/inputs/sjl13-T/t16f-strict-function-legacy-octal-value.js` | `B1 refused 2210:1:37; B4 refused 2104:1:1 (+2)` | `P accepted; K refused "Unexpected legacy octal literal in strict mode"` |
| T17A | T17, H1 | `T/parse/inputs/sjl13-T/t17a-legacy-octal-with-fraction.js` | `B1 refused 2003:1:9; B4 refused 2003:1:9` | `P refused "Unexpected token Assign: = at 1, 13"; K refused "Unexpected token Assign: = at 1, 13"` |
| T17B | T17 | `T/parse/inputs/sjl13-T/t17b-numeric-literal-touching-dollar.js` | `B1 refused 2003:1:9; B4 refused 2003:1:9` | `P refused "Unexpected token Identifier: $ at 1, 10"; K refused "Unexpected token Identifier: $ at 1, 10"` |
| T18A | T18 | `T/parse/inputs/sjl13-T/t18a-m-suffix.js` | `B1 refused 2003:1:9; B4 refused 2003:1:9` | `P accepted; K accepted` |
| T18B | T18 | `T/parse/inputs/sjl13-T/t18b-m-suffix-on-a-leading-zero.js` | `B1 refused 2003:1:9; B4 refused 2003:1:9` | `P accepted; K accepted` |
| T18C | T18 | `T/parse/inputs/sjl13-T/t18c-m-suffix-after-exponent.js` | `B1 refused 2003:1:9; B4 refused 2003:1:9` | `P refused "Unexpected token Assign: = at 1, 12"; K refused "Unexpected token Assign: = at 1, 12"` |
| T19A | T19 | `T/parse/inputs/sjl13-T/t19a-optional-chain-before-arabic-indic-digit.js` | `B1 refused 2001:1:31; B4 refused 2001:1:31` | `P refused "Unexpected token QuestionMark: ? at 1, 31"; K refused "Unexpected token QuestionMark: ? at 1, 31"` |
| T19B | T19 | `T/parse/inputs/sjl13-T/t19b-dot-before-arabic-indic-digit.js` | `B1 refused 2001:1:22; B4 refused 2001:1:22` | `P refused "Unexpected token Identifier: a at 1, 22"; K refused "Unexpected token Identifier: a at 1, 22"` |
| T20A | T20 | `T/parse/inputs/sjl13-T/t20a-two-dots.js` | `B1 refused 2101:1:11 (+1); B4 refused 2102:1:12` | `P refused "Unexpected token Identifier: a at 1, 12"; K refused "Unexpected token Identifier: a at 1, 12"` |
| T24B | T24b | `T/parse/inputs/sjl13-T/t24b-object-literal-then-division.js` | `B1 accepted; B4 refused 2104:1:9` | `P accepted; K refused "Undefined binary operation RegExLiteral"` |
| T24B2 | T24b | `T/parse/inputs/sjl13-T/t24b2-function-expression-then-division.js` | `B1 accepted; B4 refused 2104:1:9` | `P accepted; K refused "Undefined binary operation RegExLiteral"` |
| T24C1 | T24c | `T/parse/inputs/sjl13-T/t24c1-for-of-regex.js` | `B1 accepted; B4 refused 2104:1:1 (+4)` | `P refused "Unexpected token Divide: / at 1, 15"; K refused "Unexpected token Divide: / at 1, 15"` |
| T24C2 | T24c | `T/parse/inputs/sjl13-T/t24c2-extends-regex.js` | `B1 accepted; B4 refused 2104:1:1 (+2)` | `P refused "Unexpected token Divide: / at 1, 17"; K refused "Unexpected token Divide: / at 1, 17"` |
| T24C3 | T24c | `T/parse/inputs/sjl13-T/t24c3-export-default-regex.mjs` | `B1 accepted; B4 refused 2104:1:1 (+1)` | `P refused "Unexpected token Divide: / at 1, 16"; K refused "Unexpected token Divide: / at 1, 16"` |
| T24D | T24d | `T/parse/inputs/sjl13-T/t24d-for-await-head-then-regex.js` | `B1 refused 2101:1:46 (+2); B4 refused 2101:1:46` | `P refused "Unexpected token Divide: / at 1, 46"; K refused "Unexpected token Divide: / at 1, 46"` |
| T24E | T24e | `T/parse/inputs/sjl13-T/t24e-prefix-increment-regex.js` | `B1 refused 2101:1:3 (+1); B4 refused 2101:1:3` | `P refused "Unexpected token Divide: / at 1, 3"; K refused "Unexpected token Divide: / at 1, 3"` |
| T24F | T24f | `T/parse/inputs/sjl13-T/t24f-await-identifier-then-division.js` | `B1 refused 2102:1:30; B4 refused 2102:1:36` | `P accepted; K refused "Undefined binary operation RegExLiteral"` |
| T24G1 | T24g | `T/parse/inputs/sjl13-T/t24g1-yield-regex-in-generator.js` | `B1 accepted; B4 refused 2104:1:1 (+2)` | `P accepted; K accepted` |
| T24G2 | T24g | `T/parse/inputs/sjl13-T/t24g2-yield-division-in-arrow-in-generator.js` | `B1 accepted; B4 refused 2101:1:76` | `P accepted; K accepted` |
| T24G3 | T24g | `T/parse/inputs/sjl13-T/t24g3-yield-identifier-division.js` | `B1 accepted; B4 refused 2101:1:44` | `P accepted; K accepted` |
| T24H | T24h | `T/parse/inputs/sjl13-T/t24h-statement-head-then-regex.js` | `B1 accepted; B4 refused 2104:1:20 (+13)` | `P accepted; K accepted` |
| T24B3 (added 2026-10-27) | T24b | `T/parse/inputs/sjl13-T/t24b3-object-literal-regex-then-semicolon.js` | `B1 accepted; B4 refused 2104:1:26` | `P accepted; K accepted` |
| T24B4 (added 2026-10-27) | T24b | `T/parse/inputs/sjl13-T/t24b4-object-literal-regex-at-end-of-input.js` | `B1 accepted; B4 refused 2104:1:26` | `P accepted; K accepted` |
| T24B5 (added 2026-10-27) | T24b | `T/parse/inputs/sjl13-T/t24b5-function-expression-regex-then-semicolon.js` | `B1 accepted; B4 refused 2104:1:26` | `P accepted; K accepted` |
| T24B6 (added 2026-10-27) | T24b | `T/parse/inputs/sjl13-T/t24b6-function-expression-regex-at-end-of-input.js` | `B1 accepted; B4 refused 2104:1:26` | `P accepted; K accepted` |
| T24F2 (added 2026-10-27) | T24f | `T/parse/inputs/sjl13-T/t24f2-await-identifier-regex-then-semicolon.js` | `B1 refused 2102:1:36; B4 refused 2104:1:30 (+1)` | `P accepted; K accepted` |
| T24F3 (added 2026-10-27) | T24f | `T/parse/inputs/sjl13-T/t24f3-await-identifier-regex-at-end-of-input.js` | `B1 refused 2102:1:36; B4 refused 2104:1:30 (+1)` | `P accepted; K accepted` |
| T30A | T30 | `T/parse/inputs/sjl13-T/t30a-html-close-comment-slice-path.js` | `B1 accepted; B4 refused 2101:2:3` | `P accepted; K accepted` |
| T30B | T30 | `T/parse/inputs/sjl13-T/t30b-html-open-comment-slice-path.js` | `B1 accepted; B4 refused 2101:1:12` | `P accepted; K accepted` |
| T32A | T31, T32 (F17) | `T/tokens/inputs/sjl13-T-tokens/t32a-escaped-identifiers-slice-path.js` | B1 accepted; B4 accepted; token fields below | P accepted; K accepted; token fields below |
| T32B | T31, T32 (F17) | `T/tokens/inputs/sjl13-T-tokens/t32b-escaped-contextual-keywords-as-names.js` | B1 accepted; B4 accepted; token fields below | P accepted; K accepted; token fields below |
| T32C | T31, T32 (F17) | `T/tokens/inputs/sjl13-T-tokens/t32c-escaped-string-literal.js` | B1 accepted; B4 refused 2104:1:9; token fields below | P accepted; K accepted; token fields below |
| T01a | T1 | `T/value/t-values.js (line T01a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected character U+0085 (a control character) at 1, 2` |
| T01b | T1 | `T/value/t-values.js (line T01b)` | `2` | `2` |
| T01c | T1 | `T/value/t-values.js (line T01c)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected character '\u180e' (U+180E) at 1, 2` |
| T01d | T1 | `T/value/t-values.js (line T01d)` | `3` | `3` |
| T01e | T1, T3 | `T/value/t-values.js (line T01e)` | `3` | `3` |
| T01f | T1, T25 | `T/value/t-values.js (line T01f)` | `3` | `3` |
| T06a | T6 | `T/value/t-values.js (line T06a)` | `"1"` | `THROWS SyntaxError: Unexpected token Negate: ! at 1, 5` |
| T06b | T6 | `T/value/t-values.js (line T06b)` | `"1"` | `THROWS SyntaxError: Unexpected token Greater: > at 1, 6` |
| T06c | T6 | `T/value/t-values.js (line T06c)` | `"1"` | `"1"` |
| T07a | T7 | `T/value/t-values.js (line T07a)` | `"1\u000a}"` | `"1"` |
| T07b | T7 | `T/value/t-values.js (line T07b)` | `"1\u000a}"` | `"1"` |
| T08a | T8 | `T/value/t-values.js (line T08a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `"object"` |
| T08b | T8 | `T/value/t-values.js (line T08b)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | ``"`"`` |
| T11a | T11 | `T/value/t-values.js (line T11a)` | `"A"` | `"A"` |
| T11b | T11 | `T/value/t-values.js (line T11b)` | `5` | `5` |
| T12a | T12 | `T/value/t-values.js (line T12a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `"A"` |
| T12b | T12 | `T/value/t-values.js (line T12b)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `"\u0001"` |
| T12c | T12 | `T/value/t-values.js (line T12c)` | `undefined` | `"\u0001"` |
| T12d | T12 | `T/value/t-values.js (line T12d)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `"\u0001"` |
| T13a | T13 | `T/value/t-values.js (line T13a)` | `"\u0001"` | `THROWS SyntaxError: Unexpected token Empty:  at 1, 4` |
| T13b | T13 | `T/value/t-values.js (line T13b)` | `"A"` | `THROWS SyntaxError: Unexpected token Empty:  at 1, 4` |
| T13c | T13 | `T/value/t-values.js (line T13c)` | `1` | `THROWS SyntaxError: Unexpected token Empty:  at 1, 5` |
| T13d | T13 | `T/value/t-values.js (line T13d)` | `7` | `THROWS SyntaxError: Unexpected token Identifier: var at 1, 8` |
| T13e | T13 | `T/value/t-values.js (line T13e)` | `1` | `THROWS SyntaxError: Unexpected token Empty:  at 1, 4` |
| T13f | T13 control | `T/value/t-values.js (line T13f)` | `undefined` | `undefined` |
| T14a | T14 | `T/value/t-values.js (line T14a)` | `"A\| 0\|8\|9\|\u0000x\|\u00008"` | `"A\| 0\|8\|9\|\u0000x\|\u00008"` |
| T14b | T14 | `T/value/t-values.js (line T14b)` | `"abcdef"` | `"abcdef"` |
| T14c | T14 | `T/value/t-values.js (line T14c)` | `5` | `5` |
| T17a | T17 | `T/value/t-values.js (line T17a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token Empty:  at 1, 5` |
| T17b | T17 | `T/value/t-values.js (line T17b)` | `8.5` | `8.5` |
| T18a | T18 | `T/value/t-values.js (line T18a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `"1.5n"` |
| T19a | T19 | `T/value/t-values.js (line T19a)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token QuestionMark: ? at 1, 16` |
| T24a | T24a | `T/value/t-values.js (line T24a)` | `1` | `6` |
| T24a2 | T24a | `T/value/t-values.js (line T24a2)` | `1` | `6` |
| T24b | T24b | `T/value/t-values.js (line T24b)` | `1` | `THROWS SyntaxError: Undefined binary operation RegExLiteral` |
| T24c1 | T24c | `T/value/t-values.js (line T24c1)` | `1` | `THROWS SyntaxError: Unexpected token Divide: / at 1, 26` |
| T24c2 | T24c | `T/value/t-values.js (line T24c2)` | `"function"` | `THROWS SyntaxError: Unexpected token Divide: / at 1, 17` |
| T24e | T24e | `T/value/t-values.js (line T24e)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token Divide: / at 1, 3` |
| T24f | T24f | `T/value/t-values.js (line T24f)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Undefined binary operation RegExLiteral` |
| T24g1 | T24g | `T/value/t-values.js (line T24g1)` | `"a"` | `"a"` |
| T24g2 | T24g | `T/value/t-values.js (line T24g2)` | `1` | `1` |
| T24g3 | T24g | `T/value/t-values.js (line T24g3)` | `1` | `1` |
| T24h | T24h | `T/value/t-values.js (line T24h)` | `boolean true` | `boolean true` |
| T24b3 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b3)` | `NaN` | `object [object Object]` |
| T24b4 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b4)` | `NaN` | `object [object Object]` |
| T24b5 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b5)` | `NaN` | `function function () {}` |
| T24b6 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b6)` | `NaN` | `function function () {}` |
| T24b7 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b7)` | `NaN` | `object [object Object]` |
| T24b8 (added 2026-10-27) | T24b | `T/value/t24-values.js (line T24b8)` | `NaN` | `function function () {}` |
| T24f2 (added 2026-10-27) | T24f | `T/value/t24-values.js (line T24f2)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `6` |
| T24f3 (added 2026-10-27) | T24f | `T/value/t24-values.js (line T24f3)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `6` |
| T26a | T26 | `T/value/t-values.js (line T26a)` | `"\u005cu{1F600}"` | `"\u005cud83d\u005cude00"` |
| T26b | T26 | `T/value/t-values.js (line T26b)` | `"\u005cu{61}"` | `"\u005cu0061"` |
| T26c | T26 | `T/value/t-values.js (line T26c)` | `"\u005cu{D83D}"` | `"\u005cu{d83d}"` |
| T28a | T28 | `T/value/t-values.js (line T28a)` | `"1}}x2}y"` | `"1x2y"` |
| T28b | T28 control | `T/value/t-values.js (line T28b)` | `"\|}}x\|}y"` | `"\|}}x\|}y"` |
| T28c | T28 | `T/value/t-values.js (line T28c)` | `"\|}\|"` | `"\|\|"` |
| T28d | T28 | `T/value/t-values.js (line T28d)` | `"}}a1"` | `"a1"` |

**Row T32's token fields, as the token-dump programs printed them** (`reports/T32-tokens.txt` in each engine's runs;
as the programs quote them: a backslash of the source doubled, a code unit outside printable ASCII as `\uXXXX`). SJL-12's tool's tree
dump (`js/reports/T-tokens-dump.txt`) gives each identifier node its offsets and its cooked `Name`, not the token's
spelling, which the JS-tokens column gives.

| Probe | Token | Broiler.VM (`SliceTokenizer`) | Broiler.JS (`FastScanner`) | Broiler.JS tree node |
|---|---|---|---|---|
| T32A | line 1 `implements` | `Identifier raw="implements" source="implements" raw-is-source=yes string="" at=1:5 span=4..14 escaped=false keyword-table=Identifier` | `Identifier span="implements" at=4..14 cooked=null keyword=true/implements contextual=none escaped-reserved=false` | `Identifier @4..14 Name="implements"` |
| T32A | line 2 `implements` | `Identifier raw="implements" source="impl\\u0065ments" raw-is-source=no string="" at=2:5 span=24..39 escaped=true keyword-table=Identifier` | `Identifier span="impl\\u0065ments" at=24..39 cooked="implements" keyword=false/none contextual=none escaped-reserved=false` | `Identifier @24..39 Name="implements"` |
| T32A | line 3 `implements` | as line 2, `at=3:7 span=51..66` | as line 2, `at=51..66` | `Identifier @51..66 Name="implements"` |
| T32A | line 4 `a\u2118` | `Identifier raw="a\u2118" source="a\\u2118" raw-is-source=no at=4:5 span=78..85 escaped=true` | `Identifier span="a\\u2118" at=78..85 cooked="a\u2118"` | `Identifier @78..85 Name="a\u2118"` |
| T32A | line 5 `\u{16D40}` | `Identifier raw="\uD81B\uDD40" source="\\u{16D40}" raw-is-source=no at=5:5 span=91..100 escaped=true` | `Identifier span="\\u{16D40}" at=91..100 cooked="\uD81B\uDD40"` | `Identifier @91..100 Name="\uD81B\uDD40"` |
| T32B | `of` | `Identifier raw="of" source="\\u006ff" raw-is-source=no at=1:5 span=4..11 escaped=true keyword-table=Of` | `Identifier span="\\u006ff" at=4..11 cooked="of" keyword=false/none contextual=none` (a plain `of` gets `contextual=of`) | `Identifier @4..11 Name="of"` |
| T32B | `async` | `Identifier raw="async" source="\\u0061sync" raw-is-source=no at=2:5 span=21..31 escaped=true keyword-table=Async` | `Identifier span="\\u0061sync" at=21..31 cooked="async" keyword=false/none contextual=none` (a plain `async` gets `keyword=true/async`) | `Identifier @21..31 Name="async"` |
| T32C | `'it\'s'` | `StringLiteral raw="'it\\'s'" source="'it\\'s'" raw-is-source=yes string="it's" at=1:9 span=8..15 octal=false escaped=false` | `String span="'it\\'s'" at=8..15 cooked="it's"` | - |

Neither program reported a diagnostic. On every escaped identifier Broiler.VM's raw text is the cooked name and its
`escaped` flag is set, while Broiler.JS keeps the spelling in `span` and the name in `cooked`; Broiler.VM's kind is
`Identifier` where its keyword table names a keyword kind (T32B); the string token's `escaped` flag is clear (T32C).
T32C's B4 refusal is the slice manifest's (the string is an expression; T13), not the tokenizer's.

## P. The parser and its tree

**Where each engine's code is.** Broiler.VM's wide parser is `JsParser`, which builds the records of `JsSyntax.cs`
(79 records and 4 enums, every one immutable); the binder, the numeric admission and the lowering read that tree.
Broiler.JS's parser is `FastParser` over `FastTokenStream`, building the `Ast` package's tree, some of whose members the
compiler sets afterwards; `FastScopeItem` declares names as the parse goes.

**Abbreviations in this section.** Broiler.VM: `VMC/` = `src/Broiler.VM.Profile.JavaScript.Compiler/`, `VMP/` =
`src/Broiler.VM.Profile.JavaScript/`, `VMS/` = `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`.
Broiler.JS: `JSP/` = `Broiler.JS/Broiler.JavaScript.Parser/`, `JSA/` = `Broiler.JS/Broiler.JavaScript.Ast/`, `JSC/`
= `Broiler.JS/Broiler.JavaScript.Compiler/`, `JSB/` = `Broiler.JS/Broiler.JavaScript.BuiltIns/`; `Entries.cs` is
Broiler.JS `docs/evidence/sjl-12-001/tools/parse-corpus/Entries.cs` (SJL-12's tool). A VM answer from a baseline is
`refused <code>:<line>:<column>` (the first diagnostic); a JS answer is P's and K's: accepted, refused or crashed.

**The corpus join.** Every input whose outcome differs between VB's B1 and JB's P or K is assigned to one row of this
file or to a bucket another sub-area's row explains, in `correspondence-runs/analysis/rows-P-baseline.tsv.gz` (column
`row`); none is unassigned, and two runs of the join give identical files. Every test262 input the manifest calls
positive that B1, P and K all refuse is in `analysis/rows-P-agree.tsv.gz`, each with its row (P06b or P35); none is
unassigned. The buckets other sub-areas hold are listed after the rows.

### The rows

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| P01 | Script goal: a Script with a directive prologue | `VMC/JsParser.cs:447-489` Parse (non-module path); `:672-734` ParseDirectives (exact raw `"use strict"`/`'use strict'`) | `JSP/FastParser.cs:152-158` ParseProgram; `JSP/FastParser.Program.cs:8-16` | parses | parses | baseline: T042982:strict, T028332:strict (accepted in both); T043028:strict (refused in both) | same |
| P02 | Module goal: `using` and `await using` at a module's top level | `VMC/JsParser.cs:452-470` (`usingAllowed = Goal == Module`); `:1648-1690` ParseUsingDeclaration | `JSP/FastParser.Block.cs:32-33` (`atScriptTopLevel = isProgramRoot`, module roots too); `JSP/FastParser.Statement.cs:652-654` | accepted | refused, "'using' is not allowed at the top level of a script" | baseline: T051369:module, T051388:module, T042895:module, T042913:module, T052032:module | correction of Broiler.JS |
| P03 | Eval code: no Eval goal in either parser; the eval text is a Script parsed with inherited context | `VMC/JsCompiler.cs:821-933` Run (tokenizer, then `JsParser(forceStrict, enclosingFunctionDepth)` at `VMC/JsParser.cs:383-402`); `export` refused by RequireModuleGoal `:1383-1397` (2401) | `JSC/DirectEvalSupport.cs:686-745` (EvalOptions = `FastParserOptions.Script`; Validate runs the parse, ForLoopPreparation, the direct-eval early errors and `SyntaxValidation.ValidateProgram(…, privateNamesInScope)`); `JSP/FastParserOptions.cs:59` throws NotSupportedException for the Eval goal; module-item position `JSP/FastParser.Statement.cs:683-687` | `eval("export {};")` throws SyntaxError | throws SyntaxError | probe PR03; baseline: test/language/eval-code rows agree (T028759:sloppy) | same (the parse). The inherited context (strictness, function, method, derived constructor, field initialiser, private names) is R's (D16, D44) |
| P04 | JSON modules | B1 compiles a `json` row as a JSON module: invalid JSON 2406, other typed modules 2405 (`VMC/JsCompiler.cs:975`) | the SJL-12 tool parses a `json` row as a Script at P and K (no JSON reader on that path) | T040811 refused 2406; T040813, T040823, T040838 accepted | T040811 accepted; T040813, T040823, T040838 refused "Unexpected token Colon" | baseline: T040811:json, T040813:json, T040823:json, T040838:json; probe PP04 is only the import target of PP01-PP03, PP05 | engine-kept (no JSON goal in JSD-0080's five goals; each engine reads a JSON module with its JSON reader; the JS tool's Script reading of `json` rows is a tool artefact for task 1.2) |
| P05a | Import attributes: an AttributeKey is IdentifierName or StringLiteral | `VMC/JsParser.cs:919-928` (in the attribute list `:896-973`): an identifier key whose first character is not a letter (`$`, `_`, an escape) is refused 2102 | `JSP/FastParser.Import.cs:244-314` reads String, keyword and identifier keys (grammar admits `_k`); the policy check of P05b then refuses it | `with { _k: "v" }` refused 2102:1:36 | refused "Invalid attribute key \"_k\"." (policy, P05b) | probe PP01 | correction of Broiler.VM (grammar); what is refused afterwards is P05b's policy |
| P05b | Import attributes the host honours (EX0021): keys other than `type`, `type` values other than `json` | parser admits any key; lowering refuses (2405): `VMC/JsCompiler.Modules.cs:196-229` AttributeType, refusing at `:131` (imports) and `:163` (exports); `VMC/JsCompiler.cs:975` | parser refuses a key other than `type`: `JSP/FastParser.Import.cs:286-287` "Invalid attribute key"; every `type` value admitted at parse | `with { k: "v" }` refused 2405:1:1 (lowering); `type: "text"`/`"bytes"` refused 2405 | `k` refused at parse; `text`/`bytes` accepted | probe PP02; baseline: T040828:module ... T040840:module (text, bytes: VM 2405, JS accepted), V000050:module; T041737:module (unknown key: refused in both, at different stages) | engine-kept. EX0021 is an `engine` row of the stage table (D10): the library admits every key and value the grammar admits (P05a) and never reports it. Broiler.VM's lowering keeps the refusal as module-load policy (`VMC/JsCompiler.Modules.cs:196-229`, `:221`; `VMC/JsCompiler.cs:975`), and Broiler.JS's bridge takes it from its parser at SJL-14 with today's exception (D44). No request parameter carries the honoured keys or types. The specification fails the module's loading, not its parse, so neither engine's stage is the library's |
| P05c | Import attributes: a line break before a string key | tokens carry `PrecededByLineTerminator`; no newline token reaches the attribute list (`VMC/JsParser.cs:896-973`) | `JSP/FastParser.Import.cs:279`: a LineTerminator token before a String key reaches Unexpected (elsewhere `CheckAndConsume` skips line breaks, `JSP/FastTokenStream.cs:231-243`) | `with {\n "type": "json"\n}` accepted | refused (Unexpected) | probe PP03 | correction of Broiler.JS |
| P05d | Import attributes: the removed `assert` spelling | `with` only (`VMC/JsParser.cs:896`); the statement ends at Semicolon (`:7286-7304`) | `with` only (`JSP/FastParser.Import.cs:244-314`) | refused 2102:1:29 | refused | probe PP05 | same |
| P05e | Import attributes: a key given twice | `VMC/JsParser.cs:947-958` (2402) | `JSP/FastParser.Import.cs:289-294` | refused 2402 | refused | baseline: T041729:module, T041730:module, T041731:module | same |
| P06a | Import phases, the call forms (EP0012, EP0014): `import.defer(…)`, `import.source(…)`. | `VMC/JsParser.cs:5625-5633` in ParseImportMeta (`import.` + non-`meta`, 2101; the catalogue's `:5566` is an older line) | `JSP/FastParser.SingleMemberExpression.cs:45-70` (`:61`: `defer` or `source` after `import.` and before `(` is an ImportCall, ParseImportCall `:278-302`); the compiler treats the call as a plain `import(…)` (the comment at `:47-49`) | positives refused (2101); negatives refused | positives accepted; negatives under the proposal missed: `new import.defer(x)` accepted, `import.defer(...a)` accepted at P and crashed at K (NotImplementedException) | baseline (bucket `P06a` of `analysis/rows-P-baseline.tsv.gz`): positives T036783:strict, T036784:strict, T036138:strict, T036351:module, T053475:strict (staging `import.source`); negatives T036491:strict, T036492:strict, T036494:strict, T036495:strict | engine policy (the call forms' proposal flags, D17: off in configuration V, where Broiler.VM admits none; on in configuration J, where Broiler.JS admits them today) for the positives; correction of Broiler.JS for the negatives under the flag (with the flag off both refuse). The flags must be per form (F16) |
| P06b | Import phases, the declaration forms (EP0011, EP0013): `import defer * as ns from "m"`, `import source x from "m"` | `VMC/JsParser.cs:822-861` ParseImportDeclaration: `defer` or `source` is read as a default binding (`:845`), then ExpectContextual("from") (`:861`) refuses at the `*` or at the name after it, 2102; under the Script goal RequireModuleGoal refuses the declaration first, 2401 | `JSP/FastParser.Import.cs:10-105` Import has no `defer` or `source` path: Identitifer reads either as a default binding (`:51`), then ExpectContextualKeyword(from) (`:73`) throws "Expecting keyword from"; the call path of P06a is the only phase path in the parser | refused 2102 (module), 2401 (script) | refused at P and K ("Expecting keyword from") | baseline: the engines agree, so `analysis/rows-P-baseline.tsv.gz` has no row; `analysis/rows-P-agree.tsv.gz` lists every such test262 positive in bucket `P06b`: the `import defer * as ns from` declarations under `test/language/import/import-defer/` (e.g. T040993:module valid-defer-namespace.js, VB 2102:21:14, JB P "Expecting keyword from at 21, 14"; T040985:module with attributes; T040841:module) and the `import source x from` fixtures (T042048:module, T042050:module, T053476:module). Controls accepted by both: T040992:module (`import defer from`, a binding named `defer`), T042049:module (a binding named `source`). The `import-defer/syntax/invalid-*.js` negatives carry no module flag, and both refuse them as an import or export in a script (T040986 to T040991, strict and sloppy, VB 2401) | same (both refuse; neither engine implements the declaration forms). Under the proposals the declarations parse, so the library's flags for EP0011 and EP0013 must be separate from EP0012's and EP0014's, and off in both configurations, J included, for the classification to expect "refused" as both baselines hold (F16). The importers whose B1 refusal is a neighbour's P06b declaration are P34's |
| P07 | Dynamic `import(…)` grammar: no `new import(x)`, no spread argument, no bare `import`, `yield` as an identifier argument in strict code | `VMC/JsParser.cs:5572-5608` ParseImportCall (`()` refused, one or two arguments, trailing comma, no spread); `:5694` ParseMemberOnly (no `new import`) | `JSP/FastParser.SingleMemberExpression.cs:278-302` ParseImportCall (arguments read as an ArrayExpression, so a spread is admitted; with `asNew`, `new import(x)` is admitted); `:71-78` | refused 2101 (2209 for `yield`) | accepted at P; `import(...a)` crashed at K (NotImplementedException) | baseline: T036496:strict (no-new-call-expression), T036497:strict (no-rest-param), T036501:strict (typeof-import), T036345:strict (2nd-param-yield-ident-invalid) | correction of Broiler.JS |
| P08 | Expressions bound to a goal or a function: `import.meta` in a Script, `new.target` outside a function | `VMC/JsParser.cs:5620-5650` ParseImportMeta then `:1383-1397` (2404 in a script); `:5662-5689` ParseNewTarget (2101 at function depth 0) | both parsed (`JSP/FastParser.SingleMemberExpression.cs:45-70`; `new . target` at `JSP/FastParser.SinglePostfixPrefixExpression.cs:217-225`) and refused by the compiler, not the parser: `JSC/Expressions/FastCompiler.VisitMeta.cs:63` "Cannot use 'import.meta' outside a module", `:25` "new.target expression is not allowed here" | refused (2404, 2101) | P accepted, K refused | baseline: T037836:strict (import.meta goal-script), T040495:strict/sloppy and T040494 (global-code new.target), T041644:module; all in D's stage-E bucket | same in outcome at K; the stage of the JS refusal (compiler, where the grammar and the early errors put it at parse) is D's |
| P09 | `[no LineTerminator here]`: `async` then a line break before a method name; `let` then a line break then `await 0` in a non-async function | `VMC/JsParser.cs:6135` (object literal) and `:3226` (class member): an `async` head needs no line break after it; the `let await 0` case ends at Semicolon `:7286-7304` | `JSP/FastParser.ObjectLiteral.cs:69` `CheckAndConsume(async)`, which skips line breaks (`JSP/FastTokenStream.cs:231-243`); `let` path `JSP/FastParser.Statement.cs:173-211` | refused 2102 | accepted | baseline: T039299:strict/sloppy, T050971:strict/sloppy | correction of Broiler.JS |
| P10 | `do Statement while (…)`: no `;` between the body and `while` | `VMC/JsParser.cs:1879-1894` ParseDoWhile (Expect(While) straight after the body; an optional `;` after `)`) | `JSP/FastParser.DoWhile.cs:19` `stream.CheckAndConsume(TokenTypes.SemiColon)` after the body | `do {}; while (false)` refused 2102 | accepted | baseline: T028430:strict/sloppy (test/language/asi/S7.9_A9_T8.js) | correction of Broiler.JS |
| P11 | Declarations where a single Statement stands: `const` as the body of do, while, for, for-in, for-of, with; `L: async function f() {}` | `VMC/JsParser.cs:4334-4431` ParseNestedStatement / BeginsRefusedDeclaration (2101) | `JSP/FastParser.Statement.cs:209-210` (`const` has no single-statement check, unlike `let` at `:173-207`); LabeledLoop `:324-435` admits `async function` | refused 2101 | accepted | baseline: T047540, T048813, T048944, T049719, T051619 (strict, sloppy), T051776:sloppy, T050827:sloppy | correction of Broiler.JS |
| P12 | CaseBlock: one `default` at most; nothing before the first clause | `VMC/JsParser.cs:2266-2338` ParseSwitch: a second default 2101 at `:2289-2300`; a statement before the first clause fails Expect('}') with 2102 | `JSP/FastParser.Switch.cs:61-73` admits a second default; `:74-75` puts statements before the first clause into it | refused | accepted | baseline: T051012:strict/sloppy (S12.11_A2_T1, two defaults), T051017:strict/sloppy (S12.11_A3_T5) | correction of Broiler.JS |
| P13 | `~` is unary only | `VMC/JsParser.cs:5010` ParseBinary has no `~`; the statement then fails at Semicolon `:7286-7304` | `JSP/FastParser.NextExpression.cs:348` reads `TokenTypes.BitwiseNot` as a binary operator | `a & f ~ 0` refused 2102:1:27 | accepted | baseline: J000557:strict/sloppy | correction of Broiler.JS |
| P14 | Arrow parameters: a reserved word is no ArrowParameters (`switch => 1`) | `VMC/JsParser.cs:4709-4842` TryParseArrow (a bracket scan of the head, no rewinding); IsAsyncArrowHead `:4881-4915`; parameters by ParseParameters `:2498` and BindingName `:6602` | arrow detection `JSP/FastParser.SingleExpression.cs:241`; `JSP/FastParser.Expression.cs:179` `CheckAndConsumeNoLineTerminator(Lambda)`; parameters converted from the expression by `VariableDeclarator.From` (backtracking: `JSP/FastParser.cs:127` BeginUndo) | refused 2101 | accepted | baseline: T029525:strict/sloppy; every other test/language/expressions/arrow-function row differs only in P15 or D rows (T029209:strict accepted in both, T029226:strict refused in both) | correction of Broiler.JS |
| P15a | Cover grammars to patterns (JS misses): rest element not last, with an initialiser, or followed by an elision; object rest not last; `yield` as an identifier target in strict code; `({...import.meta} = …)` | `VMC/JsParser.cs:7072-7208` ToPattern; `:7213-7220` ToPatternElement; RefusedAfterRest `:7447` (called at `:2541`, `:2632`, `:2672`); BindingName `:6602` (2209 for `yield`) | cover grammar by conversion (`VariableDeclarator.From` in `JSP/FastParser.Expression.cs`; `JSP/FastParser.AssignmentLeftPattern.cs`) with no rest-position check; a rest with an initialiser reaches a compiler NotImplementedException at K | refused (2101, 2102, 2205, 2209) | accepted at P; several crashed at K | baseline: T029705:strict (array-rest-before-element), T029711:strict (array-rest-init, K crashed), T029904:strict (obj-rest-not-last), T029606:strict (yield-ident), T029283:strict (ary-ptrn-rest-not-final), T029280:strict (rest-init, K crashed), T037844:module | correction of Broiler.JS |
| P15b | Cover grammars to patterns (VM misses): a parenthesised nested target that is not a simple target: `({ a: (b = 7) } = …)`, `({ a: ([b]) } = …)` | the `parenthesised` set (`VMC/JsParser.cs:441`) is consulted only for the whole left-hand side (`:4492-4500`); ToPatternElement `:7213-7220` turns `(b = 7)` into a target with a default, and ToPattern `:7072` turns `([b])` into a pattern, neither consulting it | P admits; K refuses in `JSC/SyntaxValidation.cs:1010` "Invalid parenthesized destructuring pattern" (stage E) | accepted | P accepted, K refused | baseline: J000802:strict/sloppy, J001458:strict/sloppy | correction of Broiler.VM (the JS stage is D's) |
| P16 | Automatic semicolon insertion (outside the restricted productions of P09 and P10, and outside a for statement's head, which is P16b's) *(revised 2026-10-27, decision D-asi: it read "Automatic semicolon insertion (outside the restricted productions of P09 and P10)". The row was read from test262's `test/language/asi` tests, none of which puts a declaration before the line break, so its answers and class hold for the rest of automatic semicolon insertion and P16b holds the for statement's head)* | `VMC/JsParser.cs:7286-7304` Semicolon (`}`, end of input, or a preceding line terminator carried on the token) | EndOfStatement in `JSP/FastParser.cs` (LineTerminator tokens end a statement) | as the specification | as the specification | baseline: every test/language/asi row agrees but S7.9_A9_T8 (P10): T028331:strict refused in both, T028332:strict accepted in both | same |
| P16b | Automatic semicolon insertion in a for statement's head *(row added 2026-10-27, decision D-asi)*: no semicolon is inserted where it would become one of the two semicolons of the head (ES2026 12.10.1), so a line break where either belongs is a SyntaxError; a line break elsewhere in a head stands (a for-in or for-of head has no semicolon) | `VMC/JsParser.cs:1899` ParseFor reads each of the head's semicolons with `Expect(SliceTokenKind.Semicolon, ";")`: after a declaration (`:2049`), after an expression (`:2101`) and after the test (`:2111`). `Expect` (`:7269-7281`) takes the token itself or refuses 2102 (ExpectedToken); the head never calls `Semicolon` (`:7286-7304`), the one place a preceding line terminator stands for a `;`. A for-in or for-of head is taken before either `Expect`, at the `in` or `of` (`:1981`, `:2058`) | `VariableDeclarationStatement` (`JSP/FastParser.VariableDeclaration.cs:57-69`) reads a head's declarators with `Parameters(…, TokenTypes.SemiColon, …)` (`:64`). The initialiser's `Expression` ends at a line break before a token that is no operator (`JSP/FastParser.Expression.cs:252-275`); `Parameters` then skips the line break (`JSP/FastParser.Parameters.cs:47-49`) and ends the list at any token that is neither the `;` it was called for nor `,`, consuming nothing (`:51-55`). `ForStatement` (`JSP/FastParser.ForStatement.cs:36`) takes that declaration for a `var`, `let` or `const` head (`:93`, `:99`, `:105`), skips line breaks (`:145`) and reads the test (`:200`) with no check that the head's first `;` was read. An expression head (`ExpressionList`, `:548-595`) loops until it consumes a `;`, so its test then ends at the `)` and `:212` refuses it; a line break where the second `;` belongs ends the test with no terminator (`JSP/FastParser.WhileStatement.cs:89-112`), refused at `JSP/FastParser.ForStatement.cs:212`. With no line break at all, the initialiser's `Expression` refuses the identifier that follows it (`JSP/FastParser.Expression.cs:297-298`) | refused 2102 at the token after the line break, after a `var`, `let` or `const` declaration, after an expression and where the second semicolon belongs (B1 in both variants and forms, B3 under E0 to E7); the controls as the specification | after a `var`, `let` or `const` declaration, P and K accepted, and the loop runs as though the `;` were written (PE01, PE02: 3; PE03: the `const` binding's update throws TypeError); after an expression and where the second semicolon belongs, refused ("Unexpected token BracketEnd: )", "Unexpected token Identifier: i"); the controls as the specification | probes PP11 to PP15, controls PP16 to PP19 (a line break in a for-in and in a for-of head, PP11 with no line break, a line break after a written `;`); values PE01 to PE09 (node 22: SyntaxError for PE01 to PE05 and PE08, "a", 3 and 3 for the controls). Found by the vector `syntax/lexical/asi/0009` of Broiler.JS's corpus revision 30, `for (var i = 0` LF `i < 1; i++) ;` under PerformEval, which Broiler.JS accepted in SJL-13.2's run (its parser is the same at that run's commit as at `d6795fc8`) | correction of Broiler.JS: SJL-14 answers it as decision D45's other corrections of Broiler.JS (K10's rows, by JSD-0083's note of 2026-10-27; hand-off H22). Broiler.VM's answer is the specification's, so no correction of this family and nothing for step 1 |
| P17 | Templates, tagged templates, substitutions | `VMC/JsParser.cs:6708` ParseTemplate, `:6816` CookEscape, `:6997` ParseSubstitution; tree `VMC/JsSyntax.cs:416` JsTemplateLiteral(Cooked nullable, Raw, Substitutions) | `JSP/FastParser.Template.cs`; tree `JSA/Expressions/AstTemplateExpression.cs:8-11` (Parts: string literal expressions, cooked from the token's CookedText, raw from its span) | parses | parses | baseline: template-literal positives agree (T039978:strict), tagged-template rows agree (T039950:strict); the template-literal negatives that differ are D's stage-E bucket (legacy octal escapes, T000741:strict) | same (grammar); the chunk structure the adapter rebuilds is N's and T's (D15) |
| P18 | Classes and private names: `# x` (space after `#`); `super.#x`; `for (#x in …)`; ClassHeritage is a LeftHandSideExpression | private name is one token (`VMC/JsParser.cs:6472` IsPrivateName), so `# x` is refused 2102; `super.#x` refused 2101 at `:3463-3473`; `#x in` read only where `!noIn` (`:5026`); heritage `:2986-2993` `ParseCallChain()` then Expect('{') | `#` and the name are separate tokens (`JSP/FastParser.SingleMemberExpression.cs:176-188`; `JSP/FastParser.Identifier.cs:34-48`), so whitespace is admitted; `super` followed by `.` admits `#m` (`JSP/FastParser.SingleExpression.cs:103-112`); heritage by `Expression(out @base)` (`JSP/FastParser.Class.cs:48-52`) | refused (2101, 2102) | accepted | baseline: T034988:strict (`# x`), T034995..T035010:strict (whitespace forms), T034993/T034994/T035003/T035004:strict (accessor-named forms), T034973:strict and T035083:strict (`super.#`), T037862:strict (`for (#x in`), T034862, T034863, T046663, T046664 (arrow heritage); probe PP06 (conditional heritage) | correction of Broiler.JS |
| P19 | Decorators (EP0009) | `VMC/SliceTokenizer.cs:2754-2760` refuses `@` with 2104 before any parse (the tokenizer's diagnostics return first, `VMC/JsCompiler.cs:821-933`; the catalogue's `:2316` is an older line) | parsed and discarded: `JSP/FastParser.Statement.cs:45-52`, `JSP/FastParser.Decorator.cs`, `JSP/FastParser.Class.cs:70` | refused 2104 | accepted (no node kept) | baseline: T031800:strict, T043429:strict | engine policy (proposal flag, D17, off by default); with the flag on, the library's tree carries the decorators that JS's tree drops |
| P20 | Auto-accessors (EP0010) | `accessor` is a field name; the name after it on the same line is refused at ConsumeFieldTerminator `VMC/JsParser.cs:3411` (2102; the catalogue's `:3364` is an older line) | `JSP/FastParser.ObjectLiteral.cs:100-125` parses `accessor` ClassElementName Initializer? | refused 2102 | accepted | baseline: T035085:strict, T046886:strict, T052001:strict, T052002:strict | engine policy (proposal flag, D17) |
| P21 | `using` declarations: a binding pattern after the first binding; in a case or default clause; as a single-statement body; `await using` binding `await` in a static block | `VMC/JsParser.cs:1648-1690` ParseUsingDeclaration (2101 when `!usingAllowed`, `:1663-1671`); case clauses set `usingAllowed = false` in ParseSwitch `:2266-2338`; ParseNestedStatement `:4334-4431` | `JSP/FastParser.Statement.cs:620-671` Using: `Parameters` after the first identifier admits patterns; no case-clause or single-statement check | refused (2101, 2209) | accepted | baseline: T042899, T042905, T042907, T042908, T042917, T042918, T042919..T042924, T051364, T051375, T051381 (strict) | correction of Broiler.JS |
| P22 | Reserved words where an IdentifierReference or a LabelIdentifier stands: `await:` inside async functions, methods and generators; `case`, `catch`, `default`, `finally`, `true`, `false`, `null` as a reference; `(debugger)` | labels `VMC/JsParser.cs:604-658` (with RefuseStrictReservedIdentifier and the await/yield names at `:608-617`); ParsePrimary `:5784` | a label `await` is refused only in the module goal (`JSP/FastParser.Statement.cs:337-340`); True/False/Null/In/InstanceOf tokens become AstIdentifier (`JSP/FastParser.SingleExpression.cs:44-52`); the IdentifierReference path refuses only enum and extends plus contextual yield/await (`:116-148`) | refused 2101 | accepted | baseline: T030369:strict (await-as-label), T040995 (case), T040996 (catch), T040998 (default), T041002 (finally), T042368, T042370, T042372 (false, null, true), T047509 (`(debugger)`), T051172, T051173 (S12.14_A16 `catch`, `finally`) | correction of Broiler.JS |
| P23 | Rest parameter: last, and no trailing comma after it | `VMC/JsParser.cs:2498-2558` ParseParameters; RefusedAfterRest `:2541`, `:7447` | `JSP/FastParser.Parameters.cs:9-57` has no rest-position or trailing-comma check (and ends the list at EOF, `:51`) | refused 2101 | accepted | baseline: T030474:strict, T037416:strict, T035309:strict | correction of Broiler.JS |
| P24 | `return` in a class static block inside a function (EX0033: ClassStaticBlockStatementList has no [Return]) | binder, not parser: `VMC/JsBinder.Walks.cs:729-735` (2207); the lowering asserts it at `VMC/JsCompiler.Exceptions.cs:268-270` | Return refuses only at function depth 0 (`JSP/FastParser.Statement.cs:587-588`), so a static block in a function admits it | refused 2207 | accepted | baseline: T047162:strict/sloppy | correction of Broiler.JS; the VM's stage (binder, where the grammar puts it in the parser) is D's |
| P25 | `using` at the top level of a Script and of eval code | `usingAllowed` false outside a module, 2101 at `VMC/JsParser.cs:1663-1671` | `JSP/FastParser.Statement.cs:652-654` | refused 2101:1:1 (script; B3 eval rows E0-E7 likewise) | refused "'using' is not allowed at the top level of a script" | probe PP07 (parse, with eval-family rows in the VM), PR04 (runtime eval); baseline: test/language/statements/using refusals agree (T051363:sloppy) | same |
| P26 | The dynamic function's parameters and body (CreateDynamicFunction: parameters parsed alone as FormalParameters, body alone as FunctionBody) | `VMP/JsRealm.Dynamic.cs:159-187` FromSource: concatenates `(` + keyword + ` anonymous(` + parameters + `\n) {\n` + body + `\n})` and evaluates it as indirect eval; neither part is parsed alone | `JSB/Function/JSFunction.cs:1288-1358` CreateDynamicFunction: compiles the parameters with an empty body, the body with empty parameters (`:1350-1351`), then the whole; no FormalParameters or FunctionBody goal (`JSP/FastParserOptions.cs:59` throws NotSupportedException for both) | PR01 `new Function("/*", "*/){")` accepted; PR02 `new Function("", "}); (function(){")` accepted (answers the second function) | PR01 throws SyntaxError; PR02 accepted (the body alone still parses as a Script) | probes PR01, PR02, PR05 (control) | correction of both, which SJL-13 does not deliver. **PR01** is a correction of Broiler.VM that no change of SJL-13 lands: `FromSource` is in the profile (`VMP/JsRealm.Dynamic.cs:159-187`), outside step 1's files (WP6 commit 6.3) and step 2's (WP8); the library carries the FormalParameters and FunctionBody goals from stage 4a, but no planned Broiler.VM change routes `FromSource` through them, and step 1 changes only how the assembled text is parsed (hand-off H18, F32). **PR02** is a correction of Broiler.JS at SJL-14 (the bridge passes the dynamic function's goals from W1, D44; K10) and of Broiler.VM with PR01. Part A's kind (F) and part B's class F list nothing for P26 at either step: no input of B1 to B5 is a `Function` call, and every `test/built-ins/Function/` file passes in Broiler.VM's hosted after report (VT) |
| P27 | The tree's mutability | records (`VMC/JsSyntax.cs`: 79 records, 4 enums); side state only in the parser (`parenthesised` identity set, `VMC/JsParser.cs:441`) | settable state the compiler writes: `JSA/AstNode.cs:15` WasParenthesized, `JSA/AstBlock.cs:13` HoistingScope, `JSA/AstIdentifier.cs:24` InferenceName, `JSA/Statements/AstStatement.cs:16` IsLoopBookkeeping (internal) | immutable tree | mutable tree | none (no program-observable answer) | engine-kept (each engine keeps its tree behind its adapter, D44; the library's tree is immutable per JSD-0080) |
| P28 | Raw facts the tree carries | parenthesisation: the side set (`:441`), not on the node; BigInt: `JsBigIntLiteral(Value: BigInteger)` with no source text (`VMC/JsSyntax.cs:61`); legacy octal: `JsNumberLiteral.IsLegacyOctal` (`:52`); directives: `JsStringLiteral(Value, RawText)` (`:67`); trailing commas on array and object literals (`:97-108`, `:177`); regexp: `JsRegExpLiteral(Pattern, Flags)` (`:83`) | parenthesisation: `AstNode.WasParenthesized`; literals keep their token, so raw text is the token span (`JSA/AstLiteral.cs:10-27`); regexp: `(Start.CookedText, Start.Flags)` (`:27`), where the cooked pattern is the scanner's rewrite (D23's `\u{}` rewrite, kept until SJL-14); no trailing-comma field in `JSA/` | as listed | as listed | none | engine-kept (the library carries every raw fact JSD-0080 lists; each adapter maps them. Gaps the library closes: VM has no BigInt text and no parenthesisation on the node; JS has no trailing-comma fact and a rewritten regexp pattern) |
| P29 | Desugarings in the trees | concise arrow body to a JsReturnStatement in a JsFunctionNode (`VMC/JsParser.cs:4919-4987`, `:4968`); a field initialiser wrapped in a JsFunctionNode returning the value (`:3359-3366`); default exports named `*default*`/`default` (`:774`, `:1149`, `:1178`) | concise body to AstReturnStatement (`JSP/FastParser.Expression.cs`); Annex B.3.4 synthetic block (`JSP/FastParser.Statement.cs:82-126`); AstOptionalChain wrapper (`JSP/FastParser.SingleMemberExpression.cs:268-269`); decorators discarded (`JSP/FastParser.Class.cs:70`); `true`/`false`/`null`/`in`/`instanceof` tokens as AstIdentifier (`JSP/FastParser.SingleExpression.cs:44-52`) | as listed | as listed | none | engine-kept (the library's tree has no desugaring; each adapter reproduces its engine's; D23 keeps JS's Annex B block until SJL-14). **The two synthetic names**: `JsParser.DefaultBindingName` (`VMC/JsParser.cs:774`, `"*default*"`) and `JsParser.StarName` (`:779`, `"*"`) are written into `JsExportSpecifier.Local` by `:1224` (`export default` of an expression) and `:1050` (`export * as ns from`), and are the only members of `JsParser` named outside it besides the constructor and `Parse`, `Diagnostics`, `AdmitsBigInt` and `SawTopLevelAwait` (`VMC/JsCompiler.cs:857-875`, `:1085-1090`): the lowering reads them at `VMC/JsCompiler.Eval.cs:80`, `VMC/JsCompiler.Modules.cs:341`, `:405`, `:408`, `:553`, the binder at `VMC/JsBinder.EarlyErrors.cs:651`. Step 1 deletes `JsParser` (D20), so the two constants move with the adapter that writes them, or onto `JsSyntax.cs`, which step 1 keeps; at step 2, when `JsSyntax` goes, into the lowering's module code. X6 and I5 name each move (F14) |
| P30 | Scope facts in the JS tree | none in the tree (the binder computes them) | `JSP/FastScopeItem.cs:9` keeps declared names in a `Dictionary`; GetVariables `:237-248` enumerates it to build the HoistingScope the compiler reads (no Remove anywhere, so insertion order in practice, not by contract) | n/a | dictionary order | none | engine-kept; the library may not take an order from a hash table (EC conventions 6), so the bridge orders hoisted names by source position (F13) |
| P31 | An AssignmentRestProperty whose target is an object or array literal (catalogue EE0037, JSE1333, status `neither`): `({...{a}} = o)`, `({...[a]} = o)`. The AssignmentRestElement form `[...[a]] = []` is legal (control) | `VMC/JsParser.cs:7118-7140` ToPattern, object case: `rest = ToPattern(entry.Value)` (`:7139`) turns the literal into a nested JsObjectPattern or JsArrayPattern with no refusal (the array case's `rest = ToPattern(spread.Argument)`, `:7108`, is the legal form); lowered as any pattern (`VMC/JsCompiler.Patterns.cs:601-637`, PrepareTarget and BindPrepared of `pattern.Rest`) | `JSA/VariableDeclarator.cs:68` LiteralToPattern keeps the spread's argument unconverted (`new ObjectProperty(null, spe.Argument, null, true)`); SyntaxValidation has no rest check; the compile takes the rest at `JSC/Expressions/FastCompiler.VisitAssignmentExpression.cs:779-781` (CreateAssignment of the value with CreateObjectRest) and, finding an object or array literal, reaches `:1116` `throw new NotImplementedException()` | accepted (the rest object is destructured) | P accepted; K crashed (System.NotImplementedException), not a SyntaxError | probes PP08, PP09, control PP10. No baseline row: neither test262 at `ccaac100` (the catalogue's search: no negative test) nor any stored corpus input holds the construct (a search of `conformance/parse/inputs/` for a spread of a literal inside braces finds only an array spread, J000685) | **correction of both** (of Broiler.VM and of Broiler.JS): the library refuses it as a `syntax` row. Broiler.VM's outcome for such an input moves from accepted to refused at step 1, which part B's class F admits by this row (in B1 to B3 and B5); no corpus input or test262 variant holds it, so step 1's evidence shows the move only in the probe and the `syntax` vectors (a Script carries it). Broiler.JS's crash becomes a SyntaxError at SJL-14 (K7) |
| P32 | The non-positional facts `JsParser` computes into `JsSyntax` and the binder, the numeric admission and the lowering read: what step 1's adapter must rebuild besides N's positions | see "P32 in detail" below (fifteen items with setter, readers and source) | n/a | as listed | n/a | none for items 1 to 9 and 11 to 15 (part B's identity at step 1 and step 2 is their check); runtime probe PR06 for item 10 | engine-kept (the adapter, D15 and D19). Item 10 is also a correction of Broiler.VM, which the adapter must not make at step 1 or step 2 (part B admits no byte difference of an accepted input): SJL-15's model makes it (F12, hand-off H19) |
| P33 | The facts `FastParser` computes into the `Ast` tree that Broiler.JS's compiler reads: what SJL-14's bridge must synthesise (D44), beyond positions and P27's mutable members | n/a | see "P33 in detail" below (eight items) | n/a | as listed | none (SJL-14's part B identity with the switch off and part C's side-by-side run are their check) | engine-kept (the bridge, D44). Item 2, the Annex B.3.3 names, needs inputs D10's side table does not yet name (F13) |
| P34 | Module-graph composition and resolution: B1 compiles a module input with the graph the conformance root's resolution reaches, so a refusal anywhere in the graph refuses the input | `VMS/ParseCorpus.cs:46-48` (reading 1: "a module is compiled with the graph the conformance root's resolution reaches from it"), LoadGraph `:1221-1294` (JsCompiler.Requests per module, then JsCompiler.Compile over every unit); B5 (the tools commit) reports each module's requests alone | entry P parses the input alone (`FastParser.ParseModule`), K compiles it alone (`CoreScript.Compile(…, isModule: true)`, `docs/evidence/sjl-12-001/tools/parse-corpus/Entries.cs:73-84`); graphs are composed by the module host (`Broiler.JavaScript.Modules`, `JSModuleContext`) at run time | refused with the neighbour's diagnostic | accepted (P and K) | baseline: bucket `P34` of `analysis/rows-P-baseline.tsv.gz`: the resolution-phase negatives and the fixture importer whose neighbour holds a deliberate error, T040810 (json-invalid.js, 2406:0:0 through its JSON fixture), T041819, T041821, T041823, T041825, T041827, T041830, T041834; and the importers whose only refusal is a neighbour's P06b declaration, T040847, T040850, T040853, T040856, T040858, T040859, T040861, T040864 (their own text holds no import phase) | engine-kept: composing, resolving and loading a module graph is each engine's (Broiler.VM's B1 and B5 and its realm; Broiler.JS's module host), and the library parses one unit per request (JSD-0080's request has one Source). Task 1.2's reason: the input's own text is accepted at the syntax half; its B1 outcome follows the neighbour, which is a corpus input classified by its own row (P35, P06b) |
| P35 | test262 `_FIXTURE` files that hold a deliberate error: the manifest lists every fixture positive, because a fixture has no frontmatter, and a resolution-phase or dynamic-import negative relies on the error | the agreeing eight are refused by `JsParser` (2102, or 2205 for `0++;`); of the four that differ, `break;` is refused by the binder (2207, `VMC/JsBinder.Walks.cs:682`) and the module redeclaration with 2202 | the agreeing eight are refused by the parser at P and K; the four that differ pass P and are refused by the compiler's checks at K | refused | refused (eight); P accepted, K refused (four) | agreeing, in `analysis/rows-P-agree.tsv.gz` bucket `P35`: T015699 (ShadowRealm import-value_syntax_error_FIXTURE.js, prose), T040830 (import-attributes text-javascript_FIXTURE.js, `invalid { javascript`), T040879 (import-defer syntax-error_FIXTURE.js, `invalid syntax!`), T041820, T041822, T041826, T041828, T041831 (`0++;`); differing, in D's stage-E bucket: T041824, T041829, T041832 (`break;`, VB 2207) and T036287 (dynamic-import/catch/script-code_FIXTURE.js, `var smoosh; function smoosh() {}` in a module, VB 2202) | same for the agreeing eight (the library refuses each; task 1.2's reason: a fixture that holds a deliberate error, so the manifest's positive does not apply); the four that differ are D's stage rows. The import-phase fixtures that hold a P06b declaration (T040841 and others) are P06b's, not this row's |
| P36 | Entry K compiles a module with no module record, so `import.meta` throws at compile | B1 compiles them as modules with their graph (`VMC/JsParser.cs:5620-5650` ParseImportMeta in the module goal) | `CoreScript.Compile(…, isModule: true)` binds no record (`Entries.cs:73-84`); `import.meta` compiles to a read of the record and throws a ReferenceError when none is bound (`JSC/Expressions/FastCompiler.VisitMeta.cs:58-69`, `:66` "import.meta is not available: this module has no module record"), which the tool records as a crash; the module host binds a record, so the engine accepts these programs | accepted | P accepted; K crashed | baseline: bucket `P36` of `analysis/rows-P-baseline.tsv.gz`: T036092, T037823, T037824, T037825, T037827, T037834, T037835, T053035 | engine-kept: the module record is the engine's host's, and entry K's crash is a tool artefact of compiling a module outside one. Task 1.2's reason: accepted at the syntax half (P accepts; the library parses `import.meta` in a module) |

### P32 in detail: what `JsParser` computes that the adapter must rebuild

Section N's "What the adapter of D15 must reproduce" lists positions and spellings (anchors, `NameSpan`, synthetic
spans, SourceStart and SourceEnd, cooked and raw chunks, numeric keys), and P28 lists raw facts. This list is the
rest: every fact of `JsSyntax.cs` that is not a copy of the source's tokens, with who sets it and who reads it. "Source
at step 1" says what the adapter builds it from; at step 2 the lowering, the binder and `JsNumericAdmission` read the
shared tree (D19) and take the same facts from the same sources.

| # | Fact | Set by (`VMC/JsParser.cs`) | Read by | Source at step 1 |
|---|---|---|---|---|
| 1 | `JsProgramNode.IsStrict` (`VMC/JsSyntax.cs:1021-1025`) | `:488` from `strict`: the request's forced strictness (`:393`), the module goal (`:454`), a directive whose raw text is exactly `"use strict"` or `'use strict'` (`:702-707`) | `VMC/JsBinder.cs:195` (`program.IsStrict \|\| imposed`) | the strictness the library computes for its own strict-mode early errors, if exposed (F13); otherwise recomputed from the directives' raw text, the goal and the request |
| 2 | `JsFunctionNode.IsStrict` (`VMC/JsSyntax.cs:222-231`) | `:2862-2879` (`inner`: the enclosing strictness, a class body's, `:2946`, or the function's own directive); an arrow inherits (`:4974`); a field initialiser's wrapper is strict (`:3365`) | `VMC/JsBinder.Walks.cs:318` (`definer \|\| function.IsStrict`) | as item 1 |
| 3 | `JsFunctionNode.IsArrow`, `IsGenerator`, `IsAsync` | the function productions | the binder (`VMC/JsBinder.Walks.cs:1115`, `:1807-1808`), the lowering (`VMC/JsCompiler.Functions.cs:229`, `:239`, `:345`; `ControlFlow.cs:516`; `Exceptions.cs:412`, `:519`; `ResourceScopes.cs:127`, `:212`; `SupportingTypes.cs:167`, `:200`), `VMC/JsNumericAdmission.cs:329`, `:335` | the library's function kinds (syntax) |
| 4 | `JsFunctionNode.Name` | the binding identifier, empty when anonymous; `default` for an anonymous default-exported function or class (`:1149`, `:1178`, `:1208`) | the lowering (function `name`, NamedEvaluation) | the library's binding identifier; the `default` name is P29's desugaring |
| 5 | Property keys: `JsClassMember.Key`, `JsObjectEntry.Key`, `JsPatternProperty.Key` | PropertyKey `:6313-6365`: a string literal's cooked value, an IdentifierName's text (the token's `RawText`, cooked for an escaped name, T's row), a number by NumberKey (`NumberText.ToString`, `:6424-6431`, N21), a BigInt by BigIntKey's exact decimal (`:6408-6420`, `NumericLiterals.Evaluate` under the format's ceiling; its width refusal leaves for the admission walk, D18), a private name with its `#`; a computed key empty, with `Computed` set | the lowering (`VMC/JsCompiler.Classes.cs:197`, `:274`, `:278`, `:410` (`"constructor"`), the literal and pattern lowerings) | the library's key node and raw facts, spelt by the same functions |
| 6 | `JsClassMember.Kind` (JsMethodKind), `IsStatic`, `IsPrivate` | the class element productions | the binder (`VMC/JsBinder.Walks.cs:1991`), the lowering (`VMC/JsCompiler.Classes.cs:183`, `:258`, `:272`, `:308`, `:407`) | syntax |
| 7 | `JsClassNode.HasHeritage` (an `extends` written, `class D extends null` included) | the class head | `VMC/JsBinder.Walks.cs:551`; `VMC/JsCompiler.Classes.cs:122`, `:146`, `:149`, `:154`, `:161` | syntax |
| 8 | Parenthesisation. The `parenthesised` identity set (`:441`; filled at `:5963-5966` for a parenthesised logical expression, array or object literal, or identifier) is parser-private; it reaches the tree only as `JsAssignmentExpression.ParenthesisedTarget` (`:4552`, a parenthesised identifier target) | `:4552` | `VMC/JsCompiler.Operators.cs:554`, `:770` (no function name is inferred) | the library's parenthesisation raw fact. The set's two other uses are parse checks the library takes over (`:4494-4500`, a parenthesised literal is no pattern; `:5148`, MixesCoalesce) |
| 9 | `JsChainExpression`, the end of an optional chain | every optional chain is wrapped where it ends (`:5534`) and at the two tagged-template refusals (`:5494`, `:5526`) | `VMC/JsBinder.Walks.cs:906`; `VMC/JsCompiler.Calls.cs:134`, `:167`, `:314`; `Expressions.cs:249`; `Operators.cs:119`; `Walk.cs:385`; `VMC/JsNumericAdmission.cs:521` | one wrap per OptionalExpression of the library's tree |
| 10 | **Top-level await**: `JsParser.SawTopLevelAwait` (`:431`) | set wherever an `await` expression (`:5241`), a `for await` (`:1931`, `:1962`) or an `await using` (`:1656`) is read at `functionDepth == 0`; **an arrow does not raise `functionDepth`** (`:2503-2506`, `:2833-2836`), so an `await` inside an async arrow at a module's top level counts | `VMC/JsCompiler.cs:1090` → the binder (`:1004`, `topLevelAwait`) and the lowering (`VMC/JsCompiler.Modules.cs:500`: the module body unit is flagged Async, which `VMP/JsModule.cs:273` reads as `HasTla`, and `VMP/JsEngine.Modules.cs:336-342` evaluates asynchronously) | **Broiler.VM's rule, reproduced by the adapter at step 1 and step 2** (the flag is in the artifact, and part B admits no byte difference of an accepted input). The specification's HasTLA does not look into an ArrowFunction (Contains), and Broiler.JS agrees (`JSP/FastParser.SingleExpression.cs:445-446`; an arrow raises `functionDepth`, `JSP/FastParser.Expression.cs:199`). Correction of Broiler.VM, deferred to SJL-15's model (stage e, module records and top-level await); probe PR06; F12 |
| 11 | `JsObjectEntry.Cover`, `Shorthand`, `IsMethod` | the object literal (Cover: a CoverInitializedName kept for the binder's refusal) | Cover `VMC/JsBinder.Walks.cs:1236` (a `syntax` row's binder check, which D20 deletes), `VMC/JsCompiler.Literals.cs:289`; Shorthand `JsBinder.Walks.cs:1255`, `JsCompiler.Literals.cs:363`; IsMethod `JsBinder.Walks.cs:1248`, `:1254`, `JsCompiler.Literals.cs:306` | syntax; from step 1 the library refuses a CoverInitializedName outside a pattern, so no accepted input reaches the adapter with Cover set |
| 12 | Name-or-pattern splits: `JsDeclarator.Name`/`Pattern`, `JsForInStatement` and `JsForOfStatement` `Name`/`Pattern`/`Target`, `JsTryStatement.CatchParameter`/`CatchPattern` | the parser takes the string form for a single identifier | the binder and the lowering | the library's binding node, mapped the same way |
| 13 | The synthetic names and desugarings of P29: the concise arrow body's return, the field initialiser's function, `*default*` and `default`, `*` for `export * as ns` | `:774`, `:779`, `:1050`, `:1224`, `:3359-3366`, `:4968` | the lowering and the binder (P29's note) | built by the adapter; the two constants' home at step 1 is F14's |
| 14 | Literal values (T's rows and P28): `JsNumberLiteral.Value`/`IsLegacyOctal`, `JsBigIntLiteral.Value` (a `BigInteger` with no text), `JsStringLiteral.Value`/`RawText`, `JsTemplateLiteral` cooked and raw, `JsRegExpLiteral`, `JsIdentifier.Name` (cooked) | the literal productions | the lowering, `JsNumericAdmission` | the library's raw facts, evaluated by the functions `JsParser` uses (`NumericLiterals.Evaluate`, the template cooking) |
| 15 | Positions (N's list) | — | — | N's "What the adapter of D15 must reproduce" |

Besides the tree, `JsCompiler` reads four members of `JsParser` (`Parse`, `Diagnostics`, `AdmitsBigInt`,
`SawTopLevelAwait`: `VMC/JsCompiler.cs:857-875`, `:1085-1090`) and the two constants of item 13; nothing else of
`JsParser` is named outside it.

### P33 in detail: what `FastParser` computes that Broiler.JS's compiler reads

The table of survey-js S-2.2, re-read at `d6795fc8` and completed. P27 lists the settable members; this is every
parser-computed fact, with what SJL-14's bridge (D44) builds it from. Facts the compiler itself sets
(`AstStatement.IsLoopBookkeeping`, `HeadTdzNames`, `AstIdentifier.InferenceName`) need no bridge.

| # | Fact | Set by | Read by | The bridge builds it from |
|---|---|---|---|---|
| 1 | `AstBlock.HoistingScope` (Block, Program, Switch; `JSA/AstBlock.cs:13`) | `JSP/FastParser.Block.cs:114`, `FastParser.Program.cs:13`, `FastParser.Switch.cs:83`, `FastParser.Statement.cs:98`, from `FastScopeItem.GetVariables()` (a `Dictionary` in insertion order, P30) | `JSC/Statements/FastCompiler.VisitBlock.cs:18`, `VisitProgram.cs:103`, `VisitSwitchStatement.cs:39`; `JSC/Declarations/FastCompiler.CreateFunction.cs:946-948`; `JSC/FastCompiler.cs:229-232`, rewritten at `:242`; `JSC/ForLoopPreparation.cs:979-981`; `JSC/SyntaxValidation.cs:104-110` | D10's side table, declarations per scope in source order (P30, open question 4) |
| 2 | `AstBlock.AnnexBFunctionNames` (`JSA/AstBlock.cs:21`) | `JSP/FastParser.Block.cs:115`, `FastParser.Statement.cs:99`, carried to the program at `FastParser.Program.cs:13`; computed by `RegisterAnnexBFunctionHoisting` (`JSP/FastParser.Function.cs:166-232`): the nearest function-body or program-body scope; nothing for a plain top-level declaration unless it is an `if` clause's (B.3.4); nothing when a parameter of the enclosing function has the name, or for `arguments` in a non-arrow function; blocked by a lexical binding of the name in any scope between; first occurrence kept (`JSP/FastScopeItem.cs:51-64`) | `JSC/Statements/FastCompiler.VisitProgram.cs:283-286` (sloppy, not direct eval), `JSC/Declarations/FastCompiler.CreateFunction.cs:389-391`, `JSC/Statements/FastCompiler.VisitBlock.cs:33`, `JSC/ForLoopPreparation.cs:983-984` | **the same rule over the side table, which then has to carry, per scope, its kind (program body, function body, arrow, block, switch, catch, loop head, static block), the function's parameter names, the lexical declarations, and each block-level function declaration with whether it is an `if` clause's** (F13). D10 names "declarations per scope" only |
| 3 | `AstBlock.IsSyntheticFunctionStatementBlock` (`JSA/AstBlock.cs:29`) and the synthetic block itself | `JSP/FastParser.Statement.cs:82-126` (`:100`) | `JSC/SyntaxValidation.cs:1264` | synthesised from the shared tree (JSD-0080 D23 lists the B.3.4 block for SJL-14; D44) |
| 4 | `ContainsContinue` (internal; `AstForStatement` `:22`, `AstForInStatement` `:19`, `AstForOfStatement` `:20`) | `JSP/FastParser.ForStatement.cs:283-309`: the count of `continue` statements parsed changed across the body, **nested functions included** (`ParsedContinueCount`, `:10-27`), an over-approximation | `JSC/ForLoopPreparation.cs:251-340`, `:863` | any `continue` statement inside the loop body's span, nested functions included, as the parser counts it |
| 5 | `AstProgram.IsAsync` (`JSA/AstProgram.cs:10`) | an `await` expression (`JSP/FastParser.SingleExpression.cs:445-446`), a `for await` (`FastParser.ForStatement.cs:56-57`, `:457-458`, `:524-525`) or an `await using` (`FastParser.Statement.cs:662-663`) at `functionDepth == 0`; an arrow raises `functionDepth` (`FastParser.Expression.cs:199`) | `JSC/FastCompiler.cs:180`, `:371`; `JSC/SyntaxValidation.cs:31` | the shared tree: a top-level await outside every function, arrows included (the specification's HasTLA; unlike Broiler.VM, P32 item 10) |
| 6 | `AstNode.WasParenthesized` (`JSA/AstNode.cs:15`) | `JSP/FastParser.SingleExpression.cs:256`, `:276`, `:304`; `FastParser.NextExpression.cs:55`, `:68`, `:72` | `JSC/ForLoopPreparation.cs:204`; `JSC/Expressions/FastCompiler.VisitAssignmentExpression.cs:192`; SyntaxValidation (`IsParenthesized`) | the library's parenthesisation raw fact |
| 7 | `FastToken.Previous` and `Next` (`JSA/Misc/FastToken.cs:38-39`) and the token facts (`Number`, `CookedText`, `Flags`, `CookedInvalid`, keyword facts, `Start`, `End`) | the scanner and token stream | `JSC/SyntaxValidation.cs:1626-1627`, `:1640-1641`; `JSC/DirectEvalSupport.cs:756`; node `Start`/`End` everywhere | a linked chain synthesised from the shared token stream (D44) |
| 8 | `AstFunctionExpression.Async`, `Generator` (public fields, `JSA/Expressions/AstFunctionExpression.cs:13-14`), and P29's desugarings (concise body, `AstOptionalChain`, decorators dropped, `true`/`false`/`null` as `AstIdentifier`) | the parser | the compiler | syntax; P29 |

The parse-time redeclaration errors `FastScopeItem.AddVariable` raises are not a tree fact: under D10 the library
reports them (section D's rows).

### The corpus join's buckets that other sub-areas' rows explain

| Bucket of `analysis/rows-P-baseline.tsv.gz` or `rows-P-agree.tsv.gz` | Sub-area | What it is |
|---|---|---|
| `D:stage-E (JS checks it in its compiler)` | D (stages, task 1.2) | B1 refuses, JS P accepts and K refuses: JS checks the rule in `SyntaxValidation.cs` or the compiler. The largest bucket |
| `D:early-error JS misses (<catalogue rows>)` | D | negative tests B1 refuses and JS accepts at P and K, grouped by the catalogue rows the parse-negative table cites |
| `D:JS over-refuses (duplicate parameters, sloppy async or generator)` | D | J000072, J001979 |
| `D:early-error JS misses (strict reserved word or static-block await)` | D | J001072, J001780 |
| `R:top-level-await-at-K` | R | K has no top-level await; a module fixture with `await` is refused at K only |
| `R:regexp-validator` | R (D7, JSD-0081 D18) | regexp pattern validation differences |
| `R:nesting-and-tree-depth` | R (D14) | 2103 refusals |
| `R:engine-kept-format-ceiling` | R | 2301, 2302 lowering ceilings |
| `P34`, `P36` | P (rows P34, P36; task 1.2 cites them) | B1 composes the module graph (resolution-phase negatives refused, importers refused through a neighbour); K has no module record (`import.meta` crashes at K) |
| `P06b`, `P35` in `analysis/rows-P-agree.tsv.gz` | P (rows P06b, P35) | test262 positives both engines refuse: the import-phase declarations, and fixtures that hold a deliberate error |

### The probes, and both engines' answers as run

**Parse probes** are the rows of `P/probe-P.manifest`, run by `--parse-corpus` over every population (B3 compiles PP07
as eval code under E0 to E7) and by SJL-12's tool. **Run probes** are a script for Broiler.JS's shell under `P/run/`
and the same program as a test262 test under `P/t262/test/sjl13-P/` for Broiler.VM's conformance root (PR06 is a
module test with two `_FIXTURE` neighbours, which Broiler.JS's shell ran through `--module-host` with the async
harness preloaded). Answers are written as in section T. Rows answered by a cited baseline and not probed: P12's
duplicate `default`, and every row whose probe cell cites baseline rows only (P06b, P34, P35 and P36 among them). P32
(but its item 10) and P33 have no probe: their only observable check is byte identity at step 1 and step 2 (rule 4.1
part B, rule 4.2 part B) and SJL-14's identity and side-by-side runs. *(Added 2026-10-27: row P16b's probes, run that
day, are the rows of `P/probe-P16b.manifest` (corpus `sjl13-probes-P16b`, every population, B3 under E0 to E7) and the
lines of `P/value/p-values.js`, run as T's value lines are (each answer `PE<nn>.txt`); they are the rows below marked
so, and every answer is the one `probes/plan-p16b.tsv` expected.)*

| Probe | Row | Input (`correspondence-runs/probes/`) | Broiler.VM, as run | Broiler.JS, as run |
|---|---|---|---|---|
| PP01 | P05a | `P/inputs/attr-key-underscore.mjs` | `B1 refused 2102:1:36 (+2); B4 refused 2104:1:1` | `P refused "Invalid attribute key "_k"."; K refused "Invalid attribute key "_k"."` |
| PP02 | P05b | `P/inputs/attr-key-plain.mjs` | `B1 refused 2405:1:1; B4 refused 2104:1:1` | `P refused "Invalid attribute key "k"."; K refused "Invalid attribute key "k"."` |
| PP03 | P05c | `P/inputs/attr-newline-string-key.mjs` | `B1 accepted; B4 refused 2104:1:1` | `P refused "Unexpected token LineTerminator: \n   at 1, 35"; K refused "Unexpected token LineTerminator: \n   at 1, 35"` |
| PP04 | P04 | `P/inputs/attr-target.json` | `B1 accepted; B4 not-compiled (no-json-goal)` | `P accepted; K accepted` |
| PP05 | P05d | `P/inputs/attr-assert-legacy.mjs` | `B1 refused 2102:1:29; B4 refused 2104:1:1` | `P refused "Unexpected token Identifier: assert at 1, 29"; K refused "Unexpected token Identifier: assert at 1, 29"` |
| PP06 | P18 | `P/inputs/class-heritage-conditional.js` | `B1 refused 2102:1:30 (+2); B4 refused 2102:1:30` | `P accepted; K accepted [strict,sloppy]` |
| PP07 | P25 | `P/inputs/eval-using-top.js` | `B1 refused 2101:1:1; B3 refused 2101:1:1; B4 refused 2102:1:7` | `P refused "'using' is not allowed at the top level of a script"; K refused "'using' is not allowed at the top level of a script" [strict,sloppy]` |
| PP08 | P31 | `P/inputs/rest-property-object-target.js` | `B1 accepted; B4 refused 2205:1:9 (+1)` | `P accepted; K crashed "The method or operation is not implemented." [NotImplementedException] [strict,sloppy]` |
| PP09 | P31 | `P/inputs/rest-property-array-target.js` | `B1 accepted; B4 refused 2205:1:9 (+1)` | `P accepted; K crashed "The method or operation is not implemented." [NotImplementedException] [strict,sloppy]` |
| PP10 | P31 control | `P/inputs/rest-element-array-target.js` | `B1 accepted; B4 refused 2205:1:8 (+1)` | `P accepted; K accepted [strict,sloppy]` |
| PR01 | P26 | `P/t262/test/sjl13-P/PR01-function-params-comment.js` (Broiler.VM); `P/run/function-params-comment.js` (Broiler.JS) | `FAIL test/sjl13-P/PR01-function-params-comment.js [raw] expected a runtime-phase SyntaxError and nothing was thrown` | `PR01 threw SyntaxError` |
| PR02 | P26 | `P/t262/test/sjl13-P/PR02-function-body-closes.js` (Broiler.VM); `P/run/function-body-closes.js` (Broiler.JS) | `FAIL test/sjl13-P/PR02-function-body-closes.js [raw] expected a runtime-phase SyntaxError and nothing was thrown` | `PR02 accepted function` |
| PR03 | P03 | `P/t262/test/sjl13-P/PR03-eval-export.js` (Broiler.VM); `P/run/eval-export.js` (Broiler.JS) | `pass test/sjl13-P/PR03-eval-export.js [raw]` | `PR03 threw SyntaxError` |
| PR04 | P25 | `P/t262/test/sjl13-P/PR04-eval-using-top.js` (Broiler.VM); `P/run/eval-using-top.js` (Broiler.JS) | `pass test/sjl13-P/PR04-eval-using-top.js [raw]` | `PR04 threw SyntaxError` |
| PR05 | P26 control | `P/t262/test/sjl13-P/PR05-function-separate-parse-control.js` (Broiler.VM); `P/run/function-separate-parse-control.js` (Broiler.JS) | `pass test/sjl13-P/PR05-function-separate-parse-control.js [raw]` | `PR05 accepted 3` |
| PR06 | P32 item 10 (F12) | `P/t262/test/sjl13-P/PR06-arrow-await-not-tla.js (+ PR06-a_FIXTURE.js, PR06-b_FIXTURE.js)` (Broiler.VM); `P/t262/test/sjl13-P/PR06-arrow-await-not-tla.js (+ the two fixtures, copied to $WORK/js-mod)` (Broiler.JS) | `FAIL test/sjl13-P/PR06-arrow-await-not-tla.js [module] Test262:AsyncTestFailure:Test262Error: Test262Error: PR06 order a,tick,b` | `Test262:AsyncTestComplete` |
| PP11 (added 2026-10-27) | P16b | `P/inputs/for-head-var-line-break.js` | `B1 refused 2102:2:1; B3 refused 2102:2:1; B4 refused 2102:2:1` | `P accepted; K accepted [strict,sloppy]` |
| PP12 (added 2026-10-27) | P16b | `P/inputs/for-head-let-line-break.js` | `B1 refused 2102:2:1; B3 refused 2102:2:1; B4 refused 2102:2:1` | `P accepted; K accepted [strict,sloppy]` |
| PP13 (added 2026-10-27) | P16b | `P/inputs/for-head-const-line-break.js` | `B1 refused 2102:2:1; B3 refused 2102:2:1; B4 refused 2102:2:1` | `P accepted; K accepted [strict,sloppy]` |
| PP14 (added 2026-10-27) | P16b | `P/inputs/for-head-expression-line-break.js` | `B1 refused 2102:2:1; B3 refused 2102:2:1; B4 refused 2102:2:1` | `strict: P refused "Unexpected token BracketEnd: ) at 3, 11"; K refused "Unexpected token BracketEnd: ) at 3, 11" \| sloppy: P refused "Unexpected token BracketEnd: ) at 2, 11"; K refused "Unexpected token BracketEnd: ) at 2, 11"` |
| PP15 (added 2026-10-27) | P16b | `P/inputs/for-head-second-semicolon-line-break.js` | `B1 refused 2102:2:1; B3 refused 2102:2:1; B4 refused 2102:2:1` | `strict: P refused "Unexpected token Identifier: i at 3, 1"; K refused "Unexpected token Identifier: i at 3, 1" \| sloppy: P refused "Unexpected token Identifier: i at 2, 1"; K refused "Unexpected token Identifier: i at 2, 1"` |
| PP16 (added 2026-10-27) | P16b control | `P/inputs/for-in-head-line-break.js` | `B1 accepted; B3 accepted; B4 refused 2104:1:1 (+1)` | `P accepted; K accepted [strict,sloppy]` |
| PP17 (added 2026-10-27) | P16b control | `P/inputs/for-of-head-line-break.js` | `B1 accepted; B3 accepted; B4 refused 2104:1:1 (+1)` | `P accepted; K accepted [strict,sloppy]` |
| PP18 (added 2026-10-27) | P16b control | `P/inputs/for-head-var-no-line-break.js` | `B1 refused 2102:1:16; B3 refused 2102:1:16; B4 refused 2102:1:16` | `strict: P refused "Unexpected token Identifier: i at 2, 16"; K refused "Unexpected token Identifier: i at 2, 16" \| sloppy: P refused "Unexpected token Identifier: i at 1, 16"; K refused "Unexpected token Identifier: i at 1, 16"` |
| PP19 (added 2026-10-27) | P16b control | `P/inputs/for-head-line-break-after-semicolon.js` | `B1 accepted; B3 accepted; B4 refused 2104:2:8` | `P accepted; K accepted [strict,sloppy]` |
| PE01 (added 2026-10-27) | P16b | `P/value/p-values.js (line PE01)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `3` |
| PE02 (added 2026-10-27) | P16b | `P/value/p-values.js (line PE02)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `3` |
| PE03 (added 2026-10-27) | P16b | `P/value/p-values.js (line PE03)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS TypeError: Cannot assign to read only variable` |
| PE04 (added 2026-10-27) | P16b | `P/value/p-values.js (line PE04)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token BracketEnd: ) at 3, 11` |
| PE05 (added 2026-10-27) | P16b | `P/value/p-values.js (line PE05)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token Identifier: i at 2, 1` |
| PE06 (added 2026-10-27) | P16b control | `P/value/p-values.js (line PE06)` | `"a"` | `"a"` |
| PE07 (added 2026-10-27) | P16b control | `P/value/p-values.js (line PE07)` | `3` | `3` |
| PE08 (added 2026-10-27) | P16b control | `P/value/p-values.js (line PE08)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected token Identifier: i at 1, 16` |
| PE09 (added 2026-10-27) | P16b control | `P/value/p-values.js (line PE09)` | `3` | `3` |

## N. Positions

**Where each engine's code is.** Broiler.VM's positions are `SliceToken`'s line, column, offset and end, the spans
`JsParser` gives each node and each diagnostic, and the binder's node spans; the lowering writes them into the artifact's
position table and source text. Broiler.JS's are `FastToken`'s `Span` and `SpanLocation`s, carried into the `Ast` tree
and into `FastParseException`'s message.

**Abbreviations in this section.** Broiler.VM: `VMC/` = `src/Broiler.VM.Profile.JavaScript.Compiler/`, `VMS/` =
`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`, `VMT/` =
`src/compositions/Broiler.VM.Composition.JavaScript.Conformance/`, `VMI/` =
`src/compositions/Broiler.VM.Composition.JavaScript.Cli/`, `VMP/` = `src/Broiler.VM.Profile.JavaScript/`. Broiler.JS:
`JSP/` = `Broiler.JS/Broiler.JavaScript.Parser/`, `JSA/` = `Broiler.JS/Broiler.JavaScript.Ast/`, `JSC/` =
`Broiler.JS/Broiler.JavaScript.Compiler/`, `JSR/` = `Broiler.JS/Broiler.JavaScript.Runtime/`, `JSH/` =
`Broiler.JS/Broiler.JavaScript/` (the shell), `JST/` = `docs/evidence/sjl-12-001/tools/parse-corpus/`, `JSB/` =
`Broiler.JS/Broiler.JavaScript.BuiltIns/`. **Positions are compared on `sloppy`, `raw` and `module` rows**, because
JB's `strict` variant is one line lower (row N9). A JS answer quotes the P message (K carries the same message for a
`FastParseException`, `JSR/CoreScript.cs:109-112`).

### The rows

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| N1 | Coordinates: offsets 0-based UTF-16 indices into the unit's own text; lines 1-based; columns 1-based, counting UTF-16 code units from the line's start | `VMC/SliceTokenizer.cs:377-388` (`SliceToken` Line, Column, Offset, End), `:678` (`column = index - lineStart + 1`), `:690-691` (Offset, End); `VMC/SliceSourceDiagnostics.cs:255-277` (one-based line and column, UTF-16 units); `VMC/SliceSyntax.cs:28` (`SliceSourceSpan(Line, Column)`, no offset) | `JSA/Misc/SpanLocation.cs:3-17` (1-based, UTF-16 units); `JSA/Misc/FastToken.cs:17` (Span = source, offset, length), `:35-36` (Start, End locations); `JSP/FastScanner.cs:55-60`, `:337-361` (Consume), `:1975-1990` (State.Commit: offset `Text.Offset + position`) | as described | as described | probe NP01; baseline: where both refuse and the JS message carries a position, the sloppy rows agree on line and column or on the line, and the rest differ by N10's and N12's anchors (`analysis/pos-compare.txt`), e.g. T049540:sloppy (VB 2102:17:17, JB "Expecting BracketEnd at 17, 17"), T040803:module | same. **D15's wording** ("Offsets, lines and columns in UTF-16 code units, 1-based") reads as if offsets were 1-based; they are 0-based in both engines (finding F11) |
| N2 | Line terminators between tokens: LF, CR, U+2028 and U+2029 each end a line; CR LF is one | `VMC/SliceTokenizer.cs:2851-2863` AdvanceLine (CR LF once), `:2902` IsLineTerminator, SkipTrivia `:856-875` | `JSP/FastScanner.cs:337-361` Consume (the CR of a CR LF left to the LF), `JSP/CharExtensions.cs:90-91`, whitespace loop `:432-441` | as described | as described | probes NP01-NP04; baselines: T041050:sloppy (lone CR, VB 2102:17:7, JB 17,7), T041052:sloppy and T041053:sloppy (LS/PS, VB 2101:18:1 and 2101:16:1, JB 18,1 and 16,1); every input holding CR LF is accepted in both (e.g. T006493; `analysis/lt-inputs.txt`); VT and JT agree on every `test/language/line-terminators/` file | same |
| N3 | Line terminators inside tokens: LS/PS inside a string literal; a line continuation (CR LF once); line breaks inside a template body and a block comment | string `VMC/SliceTokenizer.cs:1631-1640` (LS/PS AdvanceLine), `:1745-1756` (continuation), SkipEscape `:2877-2890`; template ScanTemplateBody `:2347-2440` and ScanStringBody (LS/PS); comment `:909-945` | every character goes through Consume (`JSP/FastScanner.cs:337-361`): ReadString `:1679`, ReadTemplateString `:1147-1205` (CR LF consumed as one), SkipMultilineComment `:1601-1640` | as described | as described | probes NP05-NP08; VT and JT agree on `test/language/literals/string/`, `test/language/comments/` (but S7.4_A5/A6, exhausted in VT, not positional) | same |
| N4 | Surrogate pairs: an astral character is two columns and two offset units, in strings, comments and identifiers | index arithmetic throughout; identifier start by code point with its width, `VMC/SliceTokenizer.cs:998-1040` | Consume per UTF-16 unit; astral identifier start `JSP/FastScanner.cs:467-469` | as described | as described | probes NP09, NP10; baselines: J001445:sloppy (VB 2001:1:1, JB "Unexpected character ... (U+1F600) at 1, 1"), T041464:sloppy (15:1 both); every input holding an astral character is accepted by both or refused by both (`analysis/lt-inputs.txt`) | same |
| N5 | U+FEFF: white space to both compilers; a leading UTF-8 byte-order mark is the host's to drop | compiler: `VMC/SliceTokenizer.cs:2916` (`JsCompilerUnicode.Lexical.IsWhiteSpace`). Hosts drop a leading BOM before the compiler sees the text: `VMS/ParseCorpus.cs:1726-1730` Decode, `VMT/Test262Run.cs:775-780`, `VMT/Test262Modules.cs:182-186`, the CLI (usage text, `VMI/Program.cs` Usage) | compiler: `JSP/FastScanner.cs:432` (IsWhiteSpace; its comment `:428-431` names U+FEFF). Hosts: the shell's `File.ReadAllText` drops a BOM (`JSH/Program.cs` RunScriptHost); the SJL-12 tool keeps it (`JST/Manifest.cs:186-205`, `UTF8Encoding.GetString`), so the text begins with U+FEFF | inner U+FEFF: one column of white space; leading BOM: dropped by every host | inner: as VM; leading: dropped by the shell, kept by the tool (the first token at column 2) | probe NP11 (leading); baselines T041356:sloppy, T041378:sloppy, T041408:sloppy (inner U+FEFF, VB 2101:19:1, JB 19,1); VT and JT agree on every `test/language/white-space/` file; no corpus input begins with a BOM (`analysis/scan-inputs.txt`) | same (the compilers); engine-kept (host decoding: the library takes text, and what a host decodes is the host's) |
| N6 | Positions after a hashbang, a `//` comment and an HTML-like comment | SkipHashbangComment `VMC/SliceTokenizer.cs:838-850` (its terminator left to SkipTrivia), SkipTrivia `:877-907` | `JSP/FastScanner.cs:487-488`, IsHashbangStart `:997-1000`, SkipSingleLineComment `:1660-1677` (returns a LineTerminator token, consuming the terminator), `<!--` `:575-577`, `-->` `:650-652`, `:768-774` | the next line's first token at column 1 | same | probes NP12 (also B3: eval code), NP13; VT and JT agree on `test/annexB/language/comments/` and `test/language/comments/hashbang/` | same |
| N7 | Positions inside a template substitution are the source's own (a substitution is not positioned from its own start) | substitution runs tokenized at their line and line start, `VMC/SliceTokenizer.cs:499-515`, `:765-793`; deferred lexical refusals keep their place, `:785-788`, reported by `VMC/JsParser.cs:6997-7006` (and a refused substitution's placeholder at its first character, `:7006`) | one continuous scanner; TemplateBegin/Part/End tokens (`JSP/FastScanner.cs:1147-1205`); `JSP/FastParser.Template.cs:9-50` | real positions | real positions | probes NP14, NP15; SJL-12's class P (JSD-0080 D15, "positions inside substitutions may only become correct") was empty at its after commit: `docs/evidence/sjl-12-001/after/part-b-classes.tsv` header "artifact 0" | same |
| N8 | Where a unit's positions start. A script, a module, a JSON module's translated text, indirect eval code and a sloppy caller's direct-eval code are positioned from offset 0, line 1, column 1 of their own text; no engine passes a base offset, line or column (the request's "Source" field, JSD-0080). Two units are not: a strict caller's direct-eval code in Broiler.JS, parsed after a prefix (N25, N26), and the dynamic function in both engines, parsed as an assembled text (N27) | per unit (`VMC/JsCompiler.cs:821-933` Run; eval program request `:635-672`, whose `Strict` flag forces strictness on the text unchanged, `:853-855`; JSON module `:983-997`) | per text (`JSC/FastCompiler.cs:165`; a sloppy caller's direct eval `JSC/DirectEvalSupport.cs:710`; indirect eval `:693-695` and `JSR/CoreScript.cs:67-112`); **not** a strict caller's direct eval, whose parsed text is `"use strict";\n` + the text (`JSC/DirectEvalSupport.cs:127-128`, N25) | from 1:1 of the unit, alike under every eval flag set | from 1,1 of the text, except a strict caller's direct eval (the text's line 1 is line 2) | probe NP12's B3 rows (eval code under E0-E7); for the exception NP16, NP17, NR02 and baseline J001363 (N25) | same for the units listed (both pass the request's defaults; the base-offset field stays unused at SJL-13). The two composed units are N25 (correction of Broiler.JS) and N27 (implementation-defined). Broiler.VM's eval and dynamic-function positions are not guest-visible: a refusal throws a fixed message (`VMP/JsEngine.Eval.cs:135`, `:429`), so they show in B3's lists only |
| N9 | How each SJL-12 baseline composes the `strict` variant | forced strictness, text unchanged (VB header, "strict: ForceStrict, text unchanged"; `sjl-12-001/decision-rule-area.md` part B) | the manifest's prologue line, then the text (`JST/README.md`, "Variants"; JB header `strict-variant`) | `strict` positions = `sloppy` positions | `strict` positions one line lower | baseline T049540 (VB 2102:17:17 in both variants; JB 18,17 strict, 17,17 sloppy) | engine-kept (harness composition, not a front-end behaviour). For task 1.2: compare positions on sloppy, raw and module rows, or shift JB's strict lines by one; the library's forced-strictness switch positions the text as Broiler.VM does (finding F29) |
| N10 | Where a **lexical** refusal is anchored: an unterminated string, block comment, template or regular expression, a bad numeric literal or escape | at the start of the construct: the quote, `/*`, the backtick, the `/`, the literal's first character (`VMC/SliceTokenizer.cs:1610-1620`, `:909-955`, `:2296-2304`, `:2214-2236`, RefuseNumeric `:1576-1582`, the refusal helper `:2897-2898`) | where scanning stopped: `Unterminated(...)` at the scanner's location (`JSP/FastScanner.cs:94-99`); the scanner's `Unexpected()` names the token before and the scanner's cursor (`:77-81`) | construct start | stop point | probe NP15; baselines: T041058:sloppy (lone CR in a string: VB 2004:16:1, JB "Unterminated string literal at 16, 2"), T041022:sloppy (VB 2004:16:1, JB 16,2), T028581:sloppy (VB 2002:15:1, JB "Unterminated comment at 16, 1"), J000380:sloppy (VB 2002:1:1, JB 1,10), V000061:sloppy (VB 2007:1:9, JB "Unterminated template literal at 2, 1"), T013876:sloppy (regular expression: VB 2101:18:1 at the literal, JB 18,3 from the scanner's cursor) | implementation-defined (no clause names a position). Broiler.VM's anchors are judged byte for byte by its rule's part B; Broiler.JS's appear only in message text (SJL-14's class M). The library takes Broiler.VM's (finding F10) |
| N11 | Where a refusal at the **end of input** is anchored | the EndOfSource token at the end, after trailing trivia (`VMC/SliceTokenizer.cs:672-684`; on a refusal `:716-718`) | the end-of-file token is a placeholder with no position, Start (0,0) (`JSP/FastScanner.cs:186`; `JSP/FastTokenStream.cs:19-23`, `:46-50` say so); `Expect` reports "at 0, 0" (`:144-151`); the compile's SyntaxError gets line 0 (`JSR/CoreScript.cs:109-112`) | a real position | 0, 0 | baseline T028428:sloppy (VB 2101:18:1, JB "Unexpected token EOF:  at 0, 0"); every JB P message at the end of input reads "at 0, 0" (`analysis/pos-compare.txt`, kind `js-0-0`) | correction of Broiler.JS (a position the text does not have; the library's end-of-input token has a real span; listed for SJL-14's K10, finding F28) |
| N12 | Where a **parse** refusal is anchored | per site (every refusal call of `JsParser` with its anchor: `analysis/jsparser-anchors.tsv`): the current token (`Span()`, `VMC/JsParser.cs:7265`; Expect `:7269-7281`, Semicolon `:7286-7304`), the start of the production being parsed (`span`, e.g. "a function declaration needs a name" at `function`, `:2408`; `??` beside `\|\|`/`&&` at the chain's start, `:5108-5116`), a node's span, or one named token | `FastParseException`'s token: the current token (`JSP/FastTokenStream.cs:19-23`, Expect `:144-176`), the statement's first token plus the stop point when a statement was rewound (UnexpectedStatement `:38-61`), a node's Start for the parser's checks | per site | per site | baselines: T041004:sloppy (`function = 1;`: VB 2102:20:1, JB "Expecting BracketStart at 20, 10"), T039584:sloppy (VB 2101:17:3; JB "... at 17, 1 - the statement starting here could not be parsed; the parse stopped at SemiColon at 17, 3"), T030037:sloppy (VB 2205:20:1, JB 20,7); where the rule is the same and the anchor is the token the places agree (`analysis/pos-compare.txt`) | implementation-defined; the library's diagnostic span begins where Broiler.VM's front end anchors that diagnostic today, because part B of `sjl-13-001` compares `code:line:column` lists and admits no position class (finding F10) |
| N13 | The ordered diagnostic list: all errors with recovery, or the first only | every refusal, at most 64 per stage, with the parser's recovery (`VMC/JsParser.cs:7460-7470`; `VMC/SliceSourceDiagnostics.cs:300-310`); a stage stops the unit (`VMC/JsCompiler.cs:843-909`) | the first only (an exception) | ordered lists; many refused B1 rows of VB hold further parser codes after a first parser code | one diagnostic | baseline: VB (e.g. the recovery 2102s after a nesting 2103, `sjl-12-001/README.md` "T and V1 are read on the after list's first diagnostic") | engine policy (first-error or all-errors is a request parameter, D11). The tail's codes and anchors are part B's to compare: the library's all-errors mode must reproduce `JsParser`'s recovery, or part B needs a class for diagnostics after the first parse diagnostic (finding F9, D's to settle) |
| N14 | Diagnostics at no position (0:0) | binder: `arguments` in eval code inside a field initialiser (EA0005), `VMC/JsBinder.EarlyErrors.cs:240-252` (`Report(context, default, ...)`); a module name both imported and declared, `:705-718`; lowering: 2302 and the scope-depth and JSON-depth 2103s; a JSON module 2406 | EA0005 in `JSC/DirectEvalSupport.cs:286` (message only); no position concept for these | 0:0 | no position | baselines: VB B3 E6 2206:0:0 (e.g. T028763, T028765, T029604), B1 2302:0:0, 2406:0:0 (e.g. V000034:json); `areas/frontend.md:127` | engine-kept while the binder and the lowering keep these checks. If task 1.2 assigns EA0005 `syntax`, D20 moves it into the library, whose diagnostic has a span: the move from 0:0 then needs a row in the correspondence's "Correction of Broiler.VM" column for part B's class F (finding F29) |
| N15 | The positions the artifact records and an error's `stack` shows | every expression and statement places its span in the position table when it changes (`VMC/JsCompiler.Expressions.cs:33`, `VMC/JsCompiler.Statements.cs:44`, Position `VMC/JsCompiler.Emission.cs:114-125`, clamped to 1); a call again at its member's name (`:109-110`; `NameSpan` set only for a dotted, non-private member, `VMC/JsParser.cs:6302-6307`, `VMC/JsSyntax.cs:340-354`), a construction at `new`, a named member read at its name (`VMC/JsCompiler.Operators.cs:493`; JSD-0038 section 4). A node's span is the token current when its production began (`VMC/JsParser.cs:7265`), parentheses included (ParseUnaryCore `:5227`, ParsePostfix `:5330`). Synthetic nodes: a concise arrow body's return at the arrow (`:4968`), a field initialiser's function and return at the field (`:3363`) | per statement start (`CallStackItemBuilder.Step(... stmt.Start.Start.Line, ...Column)`, `JSC/Statements/FastCompiler.VisitBlock.cs:216`, `VisitProgram.cs:376`) and per function start (`JSC/Declarations/FastCompiler.CreateFunction.cs:312-321`) | expression granularity, calls at the member name | statement granularity | probe NR01 (runtime) | engine-kept (rendering; `stack` is not in the edition). The library's full spans must let the adapter rebuild every Broiler.VM anchor (D15) and the bridge every Broiler.JS token start (D44) |
| N16 | Function and class source text (`Function.prototype.toString`, JSD-0037) | SourceStart = the offset of the token at the node's span (OffsetAt, `VMC/JsParser.cs:2899-2914`), SourceEnd = the end of the last consumed token (`:2917`); a method's source starts after `static` (`:3216-3221`, `:3316`) while its member's span includes `static`; recorded at `VMC/JsCompiler.Functions.cs:406`, `VMC/JsCompiler.Classes.cs:158` | `AstNode.Code`, start token's offset to end token's end (`JSA/AstNode.cs:17-35`), `JSC/Declarations/FastCompiler.CreateFunction.cs:105-107`, class `JSC/Declarations/FastCompiler.CreateClass.cs:743-746` | the specification's source text | the same | baseline: every file of `test/built-ins/Function/prototype/toString/` passes in VT and JT, the CR, CR LF and static-method files among them | same (the adapter must rebuild SourceStart and SourceEnd from the library's spans, the method's start after `static` included) |
| N17 | Template chunk text: CR and CR LF become LF in the cooked and the raw chunk; LS and PS stay | `VMC/JsParser.cs:6776-6788` (cooked; raw through `Normalise`, `:6969`) | cooked `JSP/FastScanner.cs:1192-1200`; raw `JSC/Expressions/FastCompiler.VisitTaggedTemplateExpression.cs:22-43`, `:82-85` | as described | as described | baseline: VT and JT agree on `test/language/expressions/template-literal/`, `tagged-template/`, `test/built-ins/String/raw/` | same (the raw segment's `TrimStart('}')`, `:72`, is K3, T's) |
| N18 | Offsets that carry meaning in Broiler.JS beyond a position | not applicable (the artifact holds positions and source text only) | the tagged template's object is keyed by its start offset (`JSC/Expressions/FastCompiler.VisitTaggedTemplateExpression.cs:117-123`); a function frame's code offsets decide direct eval's lexical visibility (`JSC/Declarations/FastCompiler.CreateFunction.cs:313-321`); `ClassBodyMayDirectEval` scans member source by offsets (`JSC/Declarations/FastCompiler.CreateClass.cs:858-875`); the parser's ASI test for `.5` reads a node's end line (`JSP/FastParser.SingleMemberExpression.cs:238-245`) | n/a | as described | none (no program-observable difference between the engines) | engine-kept (the bridge must give each synthesised `FastToken` the library token's source, offset, length and start location, D44; finding F27) |
| N19 | Line terminators as tokens or as a flag | a flag on the next token, `PrecededByLineTerminator` (`VMC/SliceTokenizer.cs:383`) | LineTerminator tokens spanning the trivia run (`JSP/FastScanner.cs:432-441`), consecutive ones collapsed with their `Previous` link kept (`:240-262`), one per single-line comment (`:1660-1677`) | flag | tokens | none | engine-kept (the library keeps the flag; the bridge synthesises Broiler.JS's tokens from the text between token spans, D44) |
| N20 | The coordinates part F compares (the two tokenizers on the slice path's inputs) | `SliceToken` Line, Column, Offset, End (`VMC/SliceTokenizer.cs:377-388`); EndOfSource after trailing trivia (`:672-684`); a template one token, backtick to backtick | n/a | as N1-N7 | n/a | the slice-accepted inputs (B4), at the after commit | same (a requirement: the library's tokens carry `SliceTokenizer`'s line, column, offset and end, its end-of-input position included) |
| N21 | Numeric property keys spelt by the number area | `NumberKey` = `NumberText.ToString` (`VMC/JsParser.cs:6424-6431`) | `FormatLiteralPropertyName` = `NumberText.ToString` (`JSC/Declarations/FastCompiler.CreateClass.cs:17-27`) | `{ 1e3: 0 }` key "1000" | the same | baseline: VT and JT agree on the 51 `literal-numeric`, `literal-property-name` and numeric computed-name files | same (the adapter spells keys from the library's value, as D15 lists) |
| N22 | The source name a diagnostic carries, and its readers | `SliceSourceDiagnostic.SourceName` (`VMC/SliceSourceDiagnostics.cs:279-293`), set per unit (`VMC/JsCompiler.cs:1050-1061`); read as `name:line:column` by `VMI/HostSurfaceChecks.Scripts.cs:455`, `:555-558` and by `SliceSourceDiagnostic.ToString` (`:311-326`) | the compile's `location` reaches the SyntaxError's stack (`JSR/CoreScript.cs:109-112`, `JSR/JSException.cs:150`) | name, line, column | name, line | none | engine-kept (rendering; the library carries the request's source name, JSD-0080) |
| N23 | Where the admission walk (D18) reports the BigInt refusals | `JsParser` reports at the literal (`BigIntLiteral(span, token)`, `VMC/JsParser.cs:6391-6422`; `:5811-5819`) and the numeric admission at its node (`VMC/JsNumericAdmission.cs:97-110`) | no such refusal (unbounded BigInt, D18) | at the literal's start | n/a | VB: refused BigInt rows (2104 at the literal) | engine-kept (admission); the walk over the shared tree must report at the literal's start, or part B's lists move outside S3 |
| N24 | Where the binder's checks of `syntax` rows are anchored (D20 moves them into the library) | node spans (`VMC/JsBinder.Walks.cs:732-743` `return` at the statement, `:1176-1189` `const` without initialiser at the declarator, `:825` the three 2104 language rules at the expression; `analysis/binder-anchors.tsv`), held as `code@line:column` by `VMS/BinderChecks.cs:102-176` (41 rows, e.g. `({ a = 1 });` "2101@1:4", `({ __proto__: 1, __proto__: 2 });` "2101@1:18") | the compiler's checks report through `FastParseException` at a node's Start (`JSC/SyntaxValidation.cs:114-131`, `:534-568`) or with no position (`JSEngine.NewSyntaxError`, `JSC/FastCompiler.cs:517`) | node anchors | node Start or none | the checks lane at the base (BinderChecks); VB rows with binder codes | implementation-defined; the library reports each moved check at the node anchor the binder used (finding F10) |
| N25 | A strict caller's direct eval: where its positions start | the eval request's `Strict` flag forces strictness and the text is parsed unchanged (`VMC/JsCompiler.cs:853-855`; `VMC/JsParser.cs:363-371`), so B3 gives E1 and E3 the positions of E0 and E2 | `Execute` prefixes `"use strict";\n` (14 UTF-16 units) when `inheritStrictMode`, the caller's `IsStrictMode` passed as a constant (`JSC/DirectEvalSupport.cs:127-128`; `JSC/Expressions/FastCompiler.VisitCallExpression.cs:190`). `Validate` re-scans and parses the prefixed text (`:704-712`) and throws a SyntaxError carrying the parser's message (`:737-740`); a lexical error raised by the re-scan at `:704`, before the `try`, reaches the guest as the same SyntaxError (`JSR/JSException.cs:265-268`). The compile and the code's statement steps read the same text (`:210-211`; N15). The prefixed statement is skipped for the completion value (`:460-465`, `:494`) and for the nested-eval shortcut (`:561-570`) | the eval text's line and column; not shown to the guest (`VMP/JsEngine.Eval.cs:429`: "the evaluated source is not a program this profile admits") | line + 1, column unchanged (the prefix ends in LF), in every message that carries a position; messages that carry none (the strict-reserved-word re-scan, `:705`; the node messages of `DirectEvalEarlyErrorValidator`, `:262-317`, and of `SyntaxValidation`) are unaffected; by the code, a `stack` frame inside the eval code is one line lower too (not probed) | probes NP16, NP17 (B3 E1 for Broiler.VM; JS-parse's `strict` variant, whose prologue is the prefix's 14 characters, for Broiler.JS's parse of the same text), NR02 cases A and B (both engines' guest-visible answers); baseline J001363 (VB B1 both variants and B3 E0-E7 all 2004:1:6; JB P and K `sloppy` "Unterminated string literal at 1, 13", `strict` "... at 2, 13") | correction of Broiler.JS (a line the eval text does not have, as N11; it shows only in message text and `stack`, so at SJL-14 it is class M, not C1; finding F24) |
| N26 | A hashbang at the start of a strict caller's direct-eval text, `eval('#!x\n1')` | skipped at the text's start whatever the flags (`VMC/SliceTokenizer.cs:838-845`, `index == textStart`) | `IsHashbangStart` is `position == 0` of the scanned text (`JSP/FastScanner.cs:486-489`, `:997-1000`); after the prefix the `#` is at offset 14, so it scans as `#` and the parse refuses at the `!` (`JSP/FastParser.SingleExpression.cs:185-192`); a sloppy caller's text has no prefix | 1 | SyntaxError in a strict caller (by the code, "Unexpected token Negate: ! at 2, 2"); 1 in a sloppy caller | probe NR02 case C (strict and sloppy callers); baselines: VT passes `test/language/comments/hashbang/eval.js` and `no-line-separator.js` in the `strict` variant (a strict caller's direct eval of `#!…`); JT runs both in the sloppy form only (its runner adds `"use strict";` to `onlyStrict` files alone, `scripts/compliance/run_test262.py:1889-1893`), so no Broiler.JS run shows the refusal; T028614:raw gives Broiler.JS's message for a `#!` not at offset 0 (JB "Unexpected token Negate: ! at 1, 3") | correction of Broiler.JS (PerformEval parses the text as a Script, whose start admits a HashbangComment). It lands when the prefix goes (F24) and moves no test262 verdict, so SJL-14's K10 lists it with NR02 case C as its evidence. T5's "eval text" holds in Broiler.JS for sloppy callers only |
| N27 | The dynamic function: positions in an assembled text | `FromSource` assembles `(` + kind + ` anonymous(` + parameters + `\n) {\n` + body + `\n})` and evaluates it as indirect eval (`VMP/JsRealm.Dynamic.cs:159-187`): body from line 3, parameters from column 21; not guest-visible (`VMP/JsEngine.Eval.cs:135`) | `CreateDynamicFunction` compiles `(` + kind + ` (` + parameters + `\n) {\n\n})`, then `(` + kind + ` (\n) {\n` + body + `\n})`, then the whole (`JSB/Function/JSFunction.cs:1341`, `:1358-1361`); a refusal's message is the parser's (`JSR/CoreScript.cs:109-112`): body from line 3, parameters from column 12 | assembled text; not shown | assembled text; shown in the message | probe NR02 case D | implementation-defined (CreateDynamicFunction itself assembles the source text, and no clause names a message's position). If SJL-14's bridge parses the parameters and the body under their own goals (D44), Broiler.JS's body positions start at 1,1 of the body: class M, as N25 (F24). Broiler.VM's parse of the parts is P26's deferred hand-off |

### What the adapter of decision D15 must reproduce (from the code)

1. **Each node's line and column** as `JsParser` gives it: the token current when the production began, parentheses
   included, for every node record of `JsSyntax.cs`; the lowering places every expression and statement (N15).
2. **`NameSpan`** for a dotted member that is not private; none (default) for a computed or private member, so their
   calls are placed at the call's start (N15).
3. **Synthetic nodes' spans**: a concise arrow body's return at the arrow's span; a field initialiser's function and
   return at the field's span; a refused substitution's placeholder at its first character (N7, N15).
4. **SourceStart and SourceEnd** of every function and class, a method's after `static` (N16).
5. **Cooked and raw chunks** with CR and CR LF as LF (N17); **numeric keys** by `NumberText` (N21).
6. **Not in D15 but judged by rule 4.1's part B**: every diagnostic's `line:column` (N10 to N14, N23, N24) and the
   list's tail (N13); F9 and F10 say what D15 or part B must add. The computed facts of P32 are the adapter's too (F12).

### The probes, and both engines' answers as run

**Parse probes** are the rows of `N/probe-N.manifest` (byte-exact inputs), run by `--parse-corpus` (B1 in both forms,
B3 for the eval-family rows NP12, NP16 and NP17, B4) and by SJL-12's tool. **Run probes**: NR01 and NR02 run through
Broiler.VM's command-line root (which prints the completion value; its wide host registers the source provider, so
`eval` and `Function` compile) and through Broiler.JS's shell, the same texts ending in `print(…)`. Broiler.VM's B4 is
the slice front end; its answer is read, not predicted. Rows answered by a cited baseline and not probed: N9 (T049540),
N10 outside templates (T041058, T041022, T028581, J000380, V000061, T013876), N11 (T028428), N12 (T041004, T039584,
T030037), N13 and N14 (VB), N16, N17 and N21 (VT and JT); in part N25's parse (J001363 in both baselines) and N26's
Broiler.VM answer (VT). NP16, NP17 and NR02 add the direct eval itself, which no baseline runs in Broiler.JS.

| Probe | Row | Input (`correspondence-runs/probes/`) | Broiler.VM, as run | Broiler.JS, as run |
|---|---|---|---|---|
| NP01 | N1, N2 | `N/inputs/lf-lines.js` | `B1 refused 2101:3:1; B4 refused 2101:3:1` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP02 | N2 | `N/inputs/crlf-lines.js` | `B1 refused 2101:3:1; B4 refused 2101:3:1` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP03 | N2 | `N/inputs/lone-cr-lines.js` | `B1 refused 2101:3:1; B4 refused 2101:3:1` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP04 | N2 | `N/inputs/ls-ps-lines.js` | `B1 refused 2101:3:1; B4 refused 2101:3:1` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP05 | N3 | `N/inputs/ls-in-string.js` | `B1 refused 2101:3:1; B4 refused 2101:3:1` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP06 | N3 | `N/inputs/continuation-crlf.js` | `B1 refused 2101:2:5; B4 refused 2101:2:5` | `P refused "Unexpected token BracketEnd: ) at 2, 5"; K refused "Unexpected token BracketEnd: ) at 2, 5"` |
| NP07 | N3 | `N/inputs/template-lines.js` | `B1 refused 2101:4:1; B4 refused 2101:4:1` | `P refused "Unexpected token BracketEnd: ) at 4, 1"; K refused "Unexpected token BracketEnd: ) at 4, 1"` |
| NP08 | N3 | `N/inputs/comment-lines.js` | `B1 refused 2101:3:4; B4 refused 2101:3:4` | `P refused "Unexpected token BracketEnd: ) at 3, 4"; K refused "Unexpected token BracketEnd: ) at 3, 4"` |
| NP09 | N4 | `N/inputs/astral-string.js` | `B1 refused 2101:1:9; B4 refused 2101:1:9` | `P refused "Unexpected token BracketEnd: ) at 1, 9"; K refused "Unexpected token BracketEnd: ) at 1, 9"` |
| NP10 | N4 | `N/inputs/astral-identifier.js` | `B1 refused 2101:1:13; B4 refused 2101:1:13` | `P refused "Unexpected token BracketEnd: ) at 1, 13"; K refused "Unexpected token BracketEnd: ) at 1, 13"` |
| NP11 | N5 | `N/inputs/bom-start.js` | `B1 refused 2101:1:1; B4 refused 2101:1:1` | `P refused "Unexpected token BracketEnd: ) at 1, 2"; K refused "Unexpected token BracketEnd: ) at 1, 2"` |
| NP12 | N6, N8 | `N/inputs/hashbang-line.js` | `B1 refused 2101:2:1; B3 refused 2101:2:1; B4 refused 2101:2:1` | `P refused "Unexpected token BracketEnd: ) at 2, 1"; K refused "Unexpected token BracketEnd: ) at 2, 1"` |
| NP13 | N6 | `N/inputs/html-close-line.js` | `B1 refused 2101:3:1; B4 refused 2101:2:3` | `P refused "Unexpected token BracketEnd: ) at 3, 1"; K refused "Unexpected token BracketEnd: ) at 3, 1"` |
| NP14 | N7 | `N/inputs/substitution-position.js` | `B1 refused 2101:2:3; B4 refused 2104:1:1` | `P refused "Unexpected token BracketEnd: ) at 2, 3"; K refused "Unexpected token BracketEnd: ) at 2, 3"` |
| NP15 | N7, N10 | `N/inputs/substitution-lexical.js` | `B1 refused 2004:1:4; B4 refused 2104:1:1` | `P refused "Unterminated string literal at 1, 6"; K refused "Unterminated string literal at 1, 6"` |
| NP16 | N8, N25 | `N/inputs/strict-eval-line1.js` | `B1 refused 2101:1:7; B3 refused 2101:1:7; B4 refused 2101:1:7` | `strict: P refused "Unexpected token BracketEnd: ) at 2, 7"; K refused "Unexpected token BracketEnd: ) at 2, 7" \| sloppy: P refused "Unexpected token BracketEnd: ) at 1, 7"; K refused "Unexpected token BracketEnd: ) at 1, 7"` |
| NP17 | N25 | `N/inputs/strict-eval-line2.js` | `B1 refused 2101:2:3; B3 refused 2101:2:3; B4 refused 2101:2:3` | `strict: P refused "Unexpected token BracketEnd: ) at 3, 3"; K refused "Unexpected token BracketEnd: ) at 3, 3" \| sloppy: P refused "Unexpected token BracketEnd: ) at 2, 3"; K refused "Unexpected token BracketEnd: ) at 2, 3"` |
| NR01 | N15 | `N/run/stack-vm.js` (Broiler.VM); `N/run/stack-js.js` (Broiler.JS) | `Error: x / at f (<probes>/N/run/stack-vm.js:2:10) / at m (<probes>/N/run/stack-vm.js:4:35) / at <probes>/N/run/stack-vm.js:6:3` | `Error: x / at f:<probes>/N/run/stack-js.js:2,3 / at native:<probes>/N/run/stack-js.js:5,1` |
| NR02 | N25, N26, N27 | `N/run/strict-eval-vm.js` (Broiler.VM); `N/run/strict-eval-js.js` (Broiler.JS) | `A1 SyntaxError: the evaluated source is not a program this profile admits / A2 SyntaxError: the evaluated source is not a program this profile admits / B1 SyntaxError: the evaluated source is not a program this profile admits / B2 SyntaxError: the evaluated source is not a program this profile admits / C1 value 1 / C2 value 1 / D1 SyntaxError: the evaluated source is not a program this profile admits` | `A1 SyntaxError: Unexpected token BracketEnd: ) at 2, 7 / A2 SyntaxError: Unexpected token BracketEnd: ) at 1, 7 / B1 SyntaxError: Unexpected token BracketEnd: ) at 3, 3 / B2 SyntaxError: Unexpected token BracketEnd: ) at 2, 3 / C1 SyntaxError: Unexpected token Negate: ! at 2, 2 / C2 value 1 / D1 SyntaxError: Unexpected token BracketEnd: ) at 3, 4` |

## D. Diagnostics and the parse-time early errors

**Where each engine's code is.** Broiler.VM reports `SliceSourceDiagnostic`s with registry codes from five stages
(tokenizer, `JsParser`, the numeric admission, the binder, the lowering), each of which stops the unit; the binder holds
the early errors SJL-12.6 moved out of the lowering. Broiler.JS throws one `FastParseException` (or a `SyntaxError`)
from its parser, or later from `SyntaxValidation`, `FastCompiler.EarlyErrors`, the `Visit*` methods or
`DirectEvalSupport` (stage E). This section also holds the plan's three lists: every refusal `JsParser` makes with a
code the mapping marks `split` (D-b, decision D12), every policy or limit refusal `JsParser` makes during the parse
(D-c, decision D18), and every binder check with its catalogue rows and whether either parser checks the row (D-d,
decision D20); and it answers whether Broiler.VM's binder still refuses `L1: L2: while (c) { continue L1; }` (D-e).

**Abbreviations in this section.** Broiler.VM: `VMC/` = `src/Broiler.VM.Profile.JavaScript.Compiler/`; `JP` =
`VMC/JsParser.cs`; `JBW` = `VMC/JsBinder.Walks.cs`; `JBE` = `VMC/JsBinder.EarlyErrors.cs`; `JC` = `VMC/JsCompiler.cs`;
`SSD` = `VMC/SliceSourceDiagnostics.cs`; `VMT` = `VMC/SliceTokenizer.cs`; `SC/` =
`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`; `CF/` =
`src/compositions/Broiler.VM.Composition.JavaScript.Conformance/`; `MAP` and `REG` as above. Broiler.JS: `JSP/` =
`Broiler.JS/Broiler.JavaScript.Parser/`; `JSS` = `JSP/FastScanner.cs`; `JSC/` = `Broiler.JS/Broiler.JavaScript.Compiler/`;
`JSV` = `JSC/SyntaxValidation.cs`; `DES` = `JSC/DirectEvalSupport.cs`. "Parse time" is the parser (Broiler.VM's
tokenizer and `JsParser`; Broiler.JS's scanner, parser and `FastScopeItem`); `JSV`, `FastCompiler.*`, `Visit*` and
`DES` are compiler stages (stage E). "Dead" means that the parser refuses first every source that could reach the
check. The catalogue's site cells were read at its own heads and are stale (decision D28); every site below is at the
heads of this file.

### D-a. The diagnostic model

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| D01 | What a refusal is | `SliceSourceDiagnosticCode` (`SSD:49-254`, codes 2001-2406) in `SliceSourceDiagnostic(Code, Message, Line, Column)` with `SourceName` (`SSD:273-298`); rendered `code:Name at source:line:column: message` (`SSD:319-326`); carried in `JsCompilation.Diagnostics` | no code: `FastParseException(FastToken, message)` (`Broiler.JS/Broiler.JavaScript.Ast/Misc/FastParseException.cs:7-10`), thrown by the parser and by 86 compiler sites (52 `JSV`, 6 `Statements/FastCompiler.Module.cs`, 6 `DES`, 3 `FastCompiler.EarlyErrors.cs`, …); 5 compiler sites throw `JSEngine.NewSyntaxError` instead (`Statements/FastCompiler.VisitBreakStatement.cs:17`, `FastCompiler.cs:517`, `DES:705`, `DES:739`, `Expressions/FastCompiler.VisitMeta.cs:17`) | a registry code, a text and a place per diagnostic | one exception, V8-style text and a token position | VK lines (`code@line:column`); JB's `type`/`message` columns | engine-kept (rendering): the shared contract emits a JSE code, message identifier, phase, span and source name (JSD-0080 "Output"); VM maps the code through `MAP` (D11), JS renders text (SJL-14, D46) |
| D02 | Stages, and where each stops the unit | wide `Run` (`JC:821-933`): tokenizer over the whole input, stopping at its first refusal (`SliceTokenizer.cs:664-690` ReadStream breaks on the first diagnostic) and returning before the parse (`JC:843-847`); parser (`JC:872-876` returns on any parser diagnostic); numeric admission only under the numeric manifest (`JC:883-891`); binder (`JC:899-909`); lowering. A substitution's lexical refusal is kept and re-reported by the parser where the parse reaches it (`JP:6997-7006`, `Refuse` at `:7003`). REG's stage column (`REG:177-199`: 2001-2007 tokenizer; 2101-2104, 2401, 2404 parser; 2201-2210, 2402-2406 semantics; 2301-2303 lowering) is the code's, not the producer's: 2101 is also the binder's, 2104 the admission's, binder's and lowering's | scanner on demand inside the parse; parser (first `FastParseException`); `ForLoopPreparation.Prepare` (`JSC/FastCompiler.cs:167`); `SyntaxValidation.ValidateProgram` (`:170-171`; `JSV:21-52`); `FastCompiler.EarlyErrors` and `Visit*` as each function is compiled; `DES` for direct eval (`DES:686-745`) | five stages, each refusing the unit and ending the compilation (`Refused`) | the first exception any stage throws | VK "early error skips the lowering" | implementation-defined (stage order); step 1's stage moves are part B's S kinds; SJL-14's are classes E and T (D45, D64) |
| D03 | The cap | `SliceSourceDiagnostic.MaximumPerStage` = 64 (`SSD:300-310`), applied by the parser (`JP:7458-7470`: every refusal counted, the first 64 kept), the admission (`JsNumericAdmission.cs:99`), the binder after sorting (`JBE:79-139`, whose remark records that a unit past the cap keeps another set of codes than the lowering did) and the lowering (`JsCompiler.Emission.cs:264`); the tokenizer has none (it stops at one) | none: one error | 64 per stage | 1 | PD11; VK "early errors stop at 64"; VB T050099 (a parser cascade of 64: 2103 then 63 × 2102) | engine policy (the cap a request parameter; Broiler.VM passes 64, D11) |
| D04 | First-error or all-errors | neither, purely: tokenizer first; **parser = its first refusal plus the cascade of the statement in progress**: `StreamRefused` (`JP:190`, read at 28 places) stops every list after a refusal (the top-level loop `JP:478`, blocks, class bodies, import and attribute lists) and skips every scope check (`ValidateBlockScope` `JP:3836`, `ValidateVarScope`); each scope validator reports at most one conflict (`ValidateDeclaredNames` `JP:3780-3829` returns after the first); admission, binder and lowering report all (capped) | first only | parser lists are cascades, e.g. VB T029226 `2102:62:10;2102:62:10;2101:62:10;2101:62:13;2102:62:14;2102:62:14`; binder lists complete (VK "return in a class static block, and a break after it: 2207@1:20 2207@1:32") | one | PD08, PD09, PD10 | engine policy (first-error or all-errors a request parameter, D11) — but VM's parser policy is not "all-errors"; see F9 |
| D05 | Order | parser: detection order, which is source order for the first refusal except that a scope conflict is detected when its scope closes and only if nothing was refused in it (`JP:3831-3852`); binder: sorted by line then column, ties in walk order (`JBE:100-123`); lowering: emission order | detection order: `FastScopeItem.AddVariable` throws at the second declaration as the parse reaches it | `{ let a; let a; 1 = 2; }` → `[2205:1:17]` (the duplicate is never reported) | refused at the duplicate (1:14) | PD08; VK "early errors are in source order: 2207@1:1 2101@1:30" | implementation-defined (order); a library that reports in source order moves VM's first code on such inputs (part B's S1 admits only "the same diagnostics, in source order") |
| D06 | Message texts, and who reads them | the registry's English texts. Readers: `CF/Test262Families.cs:44-83` (the 2104 suffix " is not admitted by the declared feature manifest", written by `JP:7403-7407`), called for every `Unsupported` verdict (`CF/Test262Run.cs:437-445`); `SC/SourceFrontEndChecks.cs:84` ("65,535"), `:113-157` (the top-level-await row compares code **and** message, F7); `SC/BigIntChecks.cs:589` ("a BigInt literal is not admitted"), `:607` ("wider than 65536 bits"); `CF/RealmChecks.cs:61`, `SC/SharedChecks.cs:113`, `SC/IntlChecks.cs:102` read other components' messages | V8-style texts; `Broiler.JavaScript/Program.cs:260` retries on "Unexpected await" (`JSV:29-32`); test files read message text (critic-js) | texts kept per code (D11) | texts kept (D46) | VK:35, :979-983 | engine-kept (rendering) |
| D07 | How a first diagnostic is graded | `CF/Test262Run.cs:429-466`: a first code 2104 → `Unsupported` (family from the text); any other code on a parse or resolution `SyntaxError` negative → `Passed`; else `Failed`. So among codes only 2104 against the rest moves a test262 verdict; `LanguageErrors.Classified`/`MayScore` grade the retained suite | the exception type and phase | as stated | as stated | sjl-12-001 part A | engine-kept (grading); D12's rule grades each new code as the code it left, the three 2104 language rules regraded |
| D08 | Who reads a diagnostic's place | `Cli/HostSurfaceChecks.Scripts.cs:453-457`, `:555-559` (first diagnostic's source name, line, column); `SC/SourceFrontEndChecks.cs:184`, `:194`, `:213`, `:291`; `SC/BinderChecks.cs:104-305` (`code@line:column` lists); `--parse-corpus` (`code:line:column`). The binder reports two refusals at no place (`Report(context, default, …)`): `JBE:247` (`arguments` through eval in a field initialiser, EA0005) and `JBE:711` (a name imported and declared, EE0168/EE0169) | the token's position | VK "arguments in eval code in a field initialiser … 2206@0:0 2206@0:0", "a name imported and declared: 2201@0:0"; VB B3 E6 rows 2206:0:0 (T028763) | token positions | VK lines | engine-kept (who reads a place; the spans are N's). The 0:0 places move if a `syntax` row's check leaves the binder (B34) |
| D09 | Codes pinned by `--checks` rows outside the registry commit's files | `SC/BinderChecks.cs:104-305` pins 2101 (super, `return`, regular expressions, `__proto__`, cover initialiser, `new.target`-free), 2207 (static-block `return`), 2206 (private names, `arguments`), 2203, 2201, 2202, 2402, 2403; `SC/BigIntChecks.cs:41`, `:589`, `:607` pin 2104 for EX0017 and EX0028; `SC/SourceFrontEndChecks.cs:113-157` pins equality of codes and texts | — | all pass at the base (VK) | — | VK:130-240, :979-983 | engine-kept; F5 |

### D-b. Decision D12's list: every refusal `JsParser` makes with a code the mapping marks `split`

`MAP`'s rows with a `split` relation or a `split` other code were read site by site at the head. Their Broiler.VM
producers are the binder or the lowering for every relation-split row except the groups D12-1 to D12-4 (super,
`return`, regular expressions, private names, `arguments`, `__proto__`, the cover initialiser, EX0013, EX0020, EX0024,
EX0030 and EE0021 have no `JP` site; the `JP` sites `MAP` cites were read at `d82b07a` and have moved, D-h). The column
"Revision 19 (D12)" says what the registry commit does at the site: change the code argument (a site serving one
relation-split row alone), or leave the consolidation to the library at the after commit (rule 4.1 part B's class R2);
"Class" is the behaviour's class between the engines.

| Row | Catalogue rows (relation) | `JP` site at the head | Serves that rule alone? | Other producers | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Revision 19 (D12) | Class |
|---|---|---|---|---|---|---|---|---|---|
| D12-1 | EE0180 → 2402 (split) | `Attributes` `JP:896-973`, refusal `:947-958` (`Refuse` `:951`, `DuplicateExportName`) | **yes** | none | 2402 at the second key | `JSP/FastParser.Import.cs:289-294` refuses; test262's three samples are refused first at `:279` (P05c) | VB T041729:module `2402:22:3`, T041730 `2402:23:3`, T041731 `2402:23:3` (JB P and K refused, "Unexpected token LineTerminator") | the registry commit changes the code argument at `:951` (D12) | same |
| D12-2 | EE0163, EE0173, EA0002, EP0016 → 2101 (split) | `ParseNewTarget` `JP:5662-5689`, refusal `:5678-5686` (`Refuse` `:5680`, when `functionDepth == 0`) | **yes** (one condition, `meta.new-target.outside-function`, four rows) | binder `JBW:1022-1029` (a second check behind the parser: a non-arrow function frame, eval flags at `JBW:436-452`); lowering assertion `JsCompiler.Expressions.cs:106` | 2101 at `new` | compiler: `JSC/Expressions/FastCompiler.VisitMeta.cs:24-25` (stage E); `DES` for eval (EA0002) | VB T040494, T040495 2101 (JB P accepted, K refused "new.target expression is not allowed here"); VB B3 T040495 E0/E1 `2101:20:1`, E2-E7 accepted | the registry commit changes `:5680` (D12). The site's depth test is also wrong for field initialisers: row D32 | same (outcome; Broiler.JS at K, stage E) |
| D12-3 | EX0028 → 2104 (split) | `BigIntLiteral` `JP:6391-6404` via `OutsideExpression` (`:6395`) | **yes** | none | 2104 at the literal: "a BigInt literal wider than 65536 bits is not admitted by the declared feature manifest" | no width bound (CAT EX0028) | PD05; VK:981, :983 (`2104 at 1:9`) | split from 2104 by revision 19 (EX0028's new code); from step 1 the admission walk makes the refusal (D18) | engine-kept (a format ceiling) |
| D12-4 | EX0028 → 2104 (split) | `BigIntKey` `JP:6408-6423` via `Refuse(span, what)` (`:6412`) | **yes** | none | 2104 at the key | accepted | PD07 | as D12-3 | engine-kept (a format ceiling) |
| D12-5 | EE0087, EE0155 → 2101 (within); other 2102 and 2209 split | `Expect` `JP:7269-7280` (`:7277`, 2102); `BindingName` `JP:6600-6670` (`:6666`, 2209); `ParseObjectBindingPattern` `JP:2656-2760` (`:2694`, 2102, "`:` was expected", a string, numeric or computed key without `:`; `:2723`, 2209, "`x` is not a binding name", a shorthand key `IsIdentifierName` `:4652-4661` refuses, such as `default` or `extends`). `TryParseArrow` `JP:4709-4860` reaches it with no cover: once its scan finds `=>` it parses the head through `ParseArrowParameters`, `ParseParameters`, `ParseBindingElement` and `ParseBindingTarget` | **no**: generic sites (every missing token, EX0009 within meaning; every non-name where a binding goes); `ParseObjectBindingPattern`'s two serve every object binding pattern, `:2723` also EE0006's and EE0007's reserved words (2209 within) and EX0008's keyword shorthand (D12-9) | `RefusedAfterRest` `JP:7454` (2101, within) | cascades starting 2102 or 2209 on uncovered arrow heads | the parser refines the expression (`FastParser.SingleExpression.cs:306`, `VariableDeclarator.cs:43`) | VB T029226 first `2102:62:10`; T029280-T029282 first 2102 (JB P accepted, K crashed); VB T029423 (`({ default }) => {}`) and T029431 (`({ extends }) => {}`) first `2209:40:12` strict and sloppy, at `:2723` (JB P and K refused, "Unexpected token CurlyBracketEnd") | R2 candidates only if the library reports EE0087/EE0155 there rather than EX0008/EX0009 (F1) | same where both refuse; P14 and P15a hold the inputs Broiler.JS admits |
| D12-6 | EE0168 → 2201 (within); other 2202 split | `ValidateDeclaredNames` `JP:3780-3829` (`:3818`, 2202) | **no**: the var/lexical collision of EE0041, EE0063, EE0079, EE0134, EE0161, EE0169 (within) and the module function misread as a var (split) | binder `JBE:605-640` (module checks, behind the parser for lexical names) | 2202 at the lexical declaration | `FastScopeItem` "is already defined in current scope" | VB T041701, T041703, T041705, T041707 (module) `2202:23:16`, JB P and K refused | R2 (consolidated at the after commit) | same |
| D12-7 | EX0008 → 2101 (within); other 2102 split | `ValidateAccessorParameters` `JP:4243-4285` (`:4268`, "a setter takes exactly one parameter") | **yes** (setter arity only) | — | 2102 at the setter | compiler: `JSV:763` "Setter must have exactly one formal parameter" (stage E) | VB J000767 `2102:1:4` (JB P accepted, K refused) | R2 as planned, or R if the registry commit changes `:4268` (it serves one rule alone; F2) | same (outcome; Broiler.JS at K, stage E) |
| D12-8 | EX0008 other 2209 split ("a private name as a function or binding name") | `ParseFunctionRest` `JP:2384`; `BindingName` `JP:6610` | **yes**, each | `TryParseArrow` `JP:4748` (an async arrow's single parameter: a private name, a non-name or `await`): **no**, it also serves EE0009/EE0011 (2209 within) | 2209 at `#x` | the parser refuses (a `#` token where a name goes) | PD16 | R2 as planned, or R for `:2384` and `:6610` | same |
| D12-9 | EX0008 other 2209 split ("a reserved word where no production admits it") | `ParsePrimary` `JP:5889`; `ParseObjectEntry` `JP:6236`; `BindingName` `JP:6666`; `ParseObjectBindingPattern` `JP:2723` (a keyword as a shorthand key of any object binding pattern) | **no**: they also serve EE0006-EE0014, EE0140 and EX0023 (2209 within) | — | 2209 | parser | VB rows with 2209 first (e.g. T029485) | R2 | same; D37 holds the inputs Broiler.JS admits |
| D12-10 | EX0023 → 2209 (within); other 2101 split ("`for await` at a module top level without top-level await") | `ParseFor` `JP:1918` | **no**: the same refusal serves EX0008's `for await` outside an async context in a script or a non-async function (within) | — | 2101 | parser (`JSP/FastParser.ForStatement.cs:49`) or `JSV:31` | no corpus input (B1 and `JsCompiler.Requests` take `SliceParseOptions.Module`, top-level await on: `SliceParseOptions.cs:142`, `:244`); VK:35 | R2; and F7 | engine policy (the request's top-level-`await` switch, R04) |

**Adjacent: split codes the binder and the lowering reach** (not `JP`, so outside class R2 as rule 4.1 writes it; F2,
F3):

| Site | Code | Rows | Alone? | Reached by | Consequence |
|---|---|---|---|---|---|
| `JBW:836-844` | 2205 | EE0030, EE0031 other (delete of a private reference) | yes | live: VK "delete of a private member: 2205@1:21"; VB first 2205 on the `delete` early-error family (e.g. T034864, T034871; JB K refuses with another reason or P and K accept, bucket `D:early-error JS misses (EE0030;EE0031)`) | the registry commit must change this constant (to 2101 or a new code) or part B has no class: F2 |
| `JBW:1172-1181` | 2203 | EX0009 other (destructuring declaration without an initialiser) | yes | live: VK "a destructuring declaration without an initialiser: 2203@1:5"; no corpus row | as above |
| `JBE:695-720` (`Declares`, at 0:0) | 2201 | EE0169 other (an import a `var` redeclares) and EE0168 (within) | no | live: VK "a name imported and declared: 2201@0:0"; no corpus row | consolidation and a place change at step 1 |
| `JBW:822-833` | 2104 | EE0140, EX0008 other (`await` outside an async unit) | no | dead (the remark at `JBW:815-821`; VK's six template-substitution rows answer 2101 from the parser) | none on the corpus |
| `JBW:1195-1204`, `JBW:1288-1299`, `JBW:1329-1338` | 2104, 2104, 2205 | EP0006-EP0008; EE0035; EX0008 | yes | dead (parser first) | none on the corpus |
| relation-split binder sites | 2101, 2206, 2207 | `JBW:943` (super property rows **and** EE0162/EE0172 "super outside a method", EA0003, EP0017), `JBW:966` (super-call rows and EE0162/EE0172 for a call), `JBE:483`/`:496` (EE0019 flags), `JBE:517` (every pattern row EE0184-EE0213, EG0002, EX0026, told apart only by the matcher's message), `JBE:177`/`:247` (EE0130, EE0138, EA0005: one condition), `JBE:215` (EE0167, EE0177, EA0009 **and** EX0031, two conditions), `JBW:732` (EX0032), `JBW:740` (EX0033) | `:943`, `:966`, `:517`, `:215`: **no** | live | "one new code per meaning" (D12) is not a constant change at these four sites: F3 |
| lowering | 2206, 2103, 2201, 2104, 2302 | `JsCompiler.Classes.cs:367` (EA0009 past the eval ceiling), `JsCompiler.Eval.cs:280` (EX0013), `JsCompiler.Modules.cs:60` (EX0020), `JsCompiler.Statements.cs:149` and `JsCompiler.Expressions.cs:324` (EX0024), `JsCompiler.Assembly.cs:454-497` (EX0030), `JsCompiler.Emission.cs:197` (2302; EE0021 has no producer) | yes, each | live | constant changes, as planned |

### D-c. Decision D18's list: every policy or limit refusal `JsParser` makes during the parse

`JC:872-876` returns on any parser diagnostic before `JsNumericAdmission` (`JC:883-891`) and the binder, so each of
these refusals sits in the parser's list in parse order and stops the unit (plan-critic B2, reproduced). After it,
`StreamRefused` ends the parse at the end of the statement in progress (D04), so **a later language refusal in the
source is never reported** today.

| Row | Refusal | `JP` site | Catalogue row | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| D18-1 | a BigInt literal under a manifest that does not admit BigInt | `ParsePrimary` `JP:5811-5819` (`AdmitsBigInt` false → `OutsideExpression` → `Refuse(span, what)` `JP:7403-7407`); `AdmitsBigInt` is set from the request (`JC:246`, `:867`, `:1087`) | EX0017 (2104 within) | `2104 at 2:9: a BigInt literal is not admitted by the declared feature manifest` | accepted (no manifests) | VK:979; C02 (the order) | engine-kept (manifest admission; D18's admission walk) |
| D18-2 | a BigInt literal wider than the format's ceiling | D12-3 | EX0028 | D12-3 | accepted | PD05, PD06 | engine-kept (D18) |
| D18-3 | a BigInt key wider than the ceiling | D12-4 | EX0028 | D12-4 | accepted | PD07 | engine-kept (D18) |
| D18-4 | top-level `await`, `for await`, `await using` when the module's request has top-level await off | `Parse` `JP:451-466` (`awaitIsOperator = options.AllowTopLevelAwait`), refusals at `JP:5889` (2209), `JP:1918` (2101), and the `await using` path | EX0023 (2209 within, 2101 split) | the in-function refusal, same code and text (VK:35 passes) | `JSV:29-32` "Unexpected await" (K has top-level await off); `JSP/FastParser.ForStatement.cs:49` when the options forbid it | VK:35; PD12 (each tool's default) | engine policy (the request's switch; a `request` row, D10); F7 |
| D18-5 | a module specifier holding a lone surrogate | `ModuleSpecifier` `JP:1468-1495` (`Refuse` `:1479`, 2101, with the export-name check's text copied: "an export name holds a lone surrogate") | EX0029 (Policy, 2101 within) | 2101 at the specifier | accepted (`JSP/FastParser.Import.cs` checks export names only) | PD17 | engine-kept (an `engine` row by D10): **missing from D18's admission walk**, F6 |
| D18-6 | nesting and tree depth | `ParseTemplate` `JP:6728`; `Enter` `JP:7308-7329`; `Deepen` `JP:7344-7362`; `EnterNesting` `JP:7367-7385` | EX0011, EX0012 (2103) | 2103 | none (no bound; CAT) | R's rows (D14) | engine policy (nesting a request parameter); the tree depth a constant of the library (D14) |
| D18-7 | proposal constructs | the tokenizer's `@` (2104), re-reported for a substitution by `JP:7003`; `accessor` (`JP:3425`, 2102); import phases (`JP:822-866`, `:5627`) | EP0009-EP0014 | refused | admitted where Broiler.JS parses them | T22, P06, P19, P20 | engine policy (proposal flags, D17) |

### D-d. Decision D20's list: every binder check, its catalogue rows, and whether either engine checks the row at parse time

"Proposed stage" is D10's definition (a row either engine's parser checks is `syntax`; else `semantics`), for task 1.2,
which decides; "D20" says whether step 1 deletes the check on that proposal. "Class" is the class of the behaviour the
check decides, between the engines. The rows B01 to B34 cover every diagnostic site of the binder (`JsBinder.Walks.cs`
and `JsBinder.EarlyErrors.cs`, by a search for `SliceSourceDiagnosticCode.`).

| Row | Binder site (code) | Check | Catalogue rows | Broiler.VM parser | Broiler.JS parser | Reach | Proposed stage (D10) | D20 | Class |
|---|---|---|---|---|---|---|---|---|---|
| B01 | `JBW:675-686` (2207) | `break` with no target | EE0059, EE0081, EE0136, EE0165, EE0175 | no | no (`JSV:523-541`) | live | semantics | keep | same (outcome; Broiler.JS at K) |
| B02 | `JBW:689-702` (2208) | `continue` with no target | EE0058, EE0082, EE0137, EE0166, EE0176 | no | no (`JSV:544-557`) | live; label-set defect, D30 | semantics | keep | same (outcome; Broiler.JS at K), but for a label set (D30) |
| B03 | `JBW:728-736` (2101) | `return` outside a function | EX0032 | no | **yes** (`JSP/FastParser.Statement.cs:587-588`) | live (VK:139) | syntax | delete | same |
| B04 | `JBW:737-745` (2207) | `return` in a static block | EX0033 | no | **yes**, partly (`:587-588`, only where no function encloses the class) | live (VK:161) | syntax | delete | same, but for a static block inside a function (P24) |
| B05 | `JBW:822-833` (2104) | `await` outside an async unit | EE0140, EX0008 | **yes** (`JP:5889`, `:5252`) | yes | dead | syntax | delete | same |
| B06 | `JBW:836-844` (2205) | `delete` of a private reference | EE0030, EE0031 | **yes** for the bare-name half (`JP:5276-5287`), not for this half | no (`JSV:998`; JB misses the private half: bucket below) | live | syntax | delete; the library must refuse the private half (else a regression) | correction of Broiler.JS (D34) |
| B07 | `JBW:847-857` (2205) | update of a non-reference | EE0028, EE0029 (EG0008) | **yes** for strict `eval`/`arguments` (`JP:5310-5323`) | yes (`Broiler.JavaScript.Ast/Expressions/AstUnaryExpression.cs:31`, partial) | live (VK "update of a call in strict code: 2205@1:15"; VB T039592 sloppy `this--` 2205:27:1) | syntax | delete; the library takes the whole rule | same; D39 holds the inputs Broiler.JS admits |
| B08 | `JBW:939-950` (2101) | `super` property outside a method | EE0074, EE0075, EE0102, EE0103, EE0117, EE0118, EE0150, EE0151, EE0162, EE0172, EA0003, EP0017 | no | no (`Visit*`, `DES`) | live (VK:135, :141) | semantics | keep | same (outcome; Broiler.JS in `Visit*`), but for D41 |
| B09 | `JBW:962-978` (2101) | `super` call outside a derived constructor | EE0015, EE0076, EE0077, EE0093, EE0104-EE0106, EE0119-EE0121, EE0124, EE0126, EE0131, EE0139, EE0142, EE0152, EE0153, EA0004, EP0018 (and EE0162/EE0172 for a call) | no | no | live (VK:137) | semantics | keep | same (outcome), but for an eval in a base-class constructor (R24) |
| B10 | `JBW:1020-1031` (2101) | `new.target` outside a function | EE0163, EE0173, EA0002, EP0016 | **yes** (`JP:5680`) | no | dead (a second check) | syntax | delete | same, but for a field initialiser (D32) |
| B11 | `JBW:1032-1043` (2404) | `import.meta` outside a module | EE0027 | **yes** (`JP:5640`) | no (`VisitMeta.cs:63`) | dead | syntax | delete | same (outcome; Broiler.JS at K, P08) |
| B12 | `JBW:1172-1181` (2203) | destructuring declaration without an initialiser | EX0009 (grammar; CAT EE0044's notes) | no (`ParseDeclarators` `JP:1733-1770` admits it) | **yes** (`JSP/FastParser.VariableDeclaration.cs:52-53`) | live (VK:193) | syntax | delete | same |
| B13 | `JBW:1182-1191` (2203) | `const` without an initialiser | EE0044 (the rule is IsConstantDeclaration's, so it covers `using` and `await using` too: test262 cites it for them) | **yes for `using` and `await using`**: `ValidateResources` `JP:1698-1730` refuses at `:1720` (code `:1722`, "`using x` needs an initialiser"), reached from `ParseUsingDeclaration` `:1675` and from a counted `for` head's `using` `:2043`; **no for `const`** (`ParseDeclarators` `JP:1733-1770` admits it, and this binder check refuses it) | **yes**: `ValidateDeclaratorInitializers` `JSP/FastParser.VariableDeclaration.cs:35-55`, `using` at `:45-47`, `const` at `:49-50` (callers `:22`, `FastParser.Statement.cs:668`, `FastParser.ForStatement.cs:220`) | live for `const` (VK:191; VB T047460-T047462, T047477, T047478 2203 first, from the binder); the `using` half is the parser's (VB T042914-T042916, T051366-T051368 `2203` first, e.g. T051368 `2203:29:9`; JB P and K refused "Missing initializer in using declaration" / "… await using declaration") | syntax | delete the binder's `const` half; the parser's `using` half leaves with `JsParser` (both halves are the library's EE0044) | same |
| B14 | `JBW:1192-1204` (2104) | `using` outside a disposing list | EP0006-EP0008 | **yes** (`JP:1666`) | EP0008 yes | dead | syntax | delete | same |
| B15 | `JBW:1233-1245` (2101) | `{ a = 1 }` outside a pattern | EE0017 | no | no (`JSV:899`) | live (VK:143; VB T038408) | **semantics by the definition, though purely grammatical** (F15) | keep, or delete if task 1.2 makes it `syntax` | same (outcome; Broiler.JS at K) |
| B16 | `JBW:1252-1266` (2101) | `__proto__` twice | EE0018 | no | no (`JSV:908`) | live (VK:145; VB T038372) | semantics (F15) | keep | same (outcome; Broiler.JS at K) |
| B17 | `JBW:1288-1299` (2104) | logical assignment to a non-name, non-property | EE0035 (D13) | **yes** (`JP:4518-4535`, 2205) | yes (`JSP/FastParser.NextExpression.cs:305`) | dead | syntax | delete | same |
| B18 | `JBW:1305-1312` (2205) | assignment target not a reference | EE0033, EE0034, EE0035 (EG0008) | **yes** (same test, `JP:4518-4535`) | partly | dead | syntax | delete | same; D39 holds the inputs Broiler.JS admits |
| B19 | `JBW:1329-1338` (2205) | a declaration pattern's leaf not a name | EX0008 | **yes** (patterns are built from `BindingName`) | yes | dead | syntax | delete | same |
| B20 | `JBW:1350-1360` (2205) | a destructuring-assignment leaf not a reference | EE0036, EE0039 | **yes** (`ToPattern` `JP:7072-7208`) | yes (`VariableDeclarator.cs:43`) | dead | syntax | delete | same; D39 holds the inputs Broiler.JS admits |
| B21 | `JBW:1391-1403` (2205) | a `for`-`in`/`of` head not a reference | EE0054 (EG0008) | no | no (CAT: vm-only) | live (VK "a call as a for-of target in strict code: 2205@1:20"; VB T001070, T048863) | semantics by the definition; same condition as EE0033 (F15) | keep | correction of Broiler.JS (D39: Broiler.JS's K crashes) |
| B22 | `JBE:170-183` (2206) | `arguments` in a field initialiser or a static block | EE0130, EE0138 | no | no (`Visit*`) | live (VK) | semantics | keep | same (outcome; Broiler.JS in `Visit*`) |
| B23 | `JBE:205-222` (2206) | private name no class declares | EE0167, EE0177, EA0009, EX0031 | no | no (`JSV:1217`, `:1253`) | live | semantics | keep | same (outcome; Broiler.JS at K) |
| B24 | `JBE:240-254` (2206, at 0:0) | `arguments` through eval in a field | EA0005 (EE0130) | no | no (`DES`) | live (VB B3 E6 2206:0:0) | semantics | keep | same (R25) |
| B25 | `JBE:469-520` `CheckRegExp` (2101) | flags, then the pattern through `Format.JsRegExpMatcher` | EE0019, EE0183-EE0213, EG0002, EX0026 | no | **yes** for most (the scanner's validator, `JSP/FastScanner.cs:1331-1510`; not EE0186-EE0190, EE0208) | live | syntax | moves to the provider (D7), deleted from the binder | engine policy (the request's validator, D7; R18), with R19's and R20's corrections behind it |
| B26 | `JBE:565-578` (2209) | `arguments`/`eval` bound by an import specifier written without `as` | EE0005 | **yes** for the `as` form (`BindingName` `JP:6630`); the bare form reaches only the binder (`ParseImportClause` `JP:975-1011`) | **yes** (`JSP/FastParser.Import.cs:212-218`) | live: VB T041639, T041642 `2209:35:10` (the binder, at the specifier); T041640, T041641 `2209:35:15` (the parser) | syntax | delete (**not in D20's list**) | same |
| B27 | `JBE:579-588` (2201) | a name imported twice (one set across every import of the module) | EE0168, and **EE0179** (one ImportDeclaration binding a name twice: the same check) | no (imports are not in `ValidateDeclaredNames`) | no: `AddImportedBinding` `JSV:260-268` (`:267`), from `ValidateModuleEarlyErrors` `JSV:75-95` (stage E; VB T040801) | live (VK "a name imported twice: 2201@1:35") | EE0168 syntax (row-level: VM's parser checks EE0168's lexical names); **EE0179 semantics by D10's definition** (neither parser checks it) | delete only if task 1.2 makes EE0179 `syntax` as well; otherwise the binder keeps this check for EE0179 (F15) | same (outcome; Broiler.JS at K) |
| B28 | `JBE:599-610` (2201) | a function declared twice in a module | EE0168 | no (the parser reads a module's functions as var names, `JP:3881-3883`) | yes (`FastScopeItem`) | live (VK "a function declared twice in a module: 2201@1:17") | syntax | delete | same |
| B29 | `JBE:611-619` (2202) | a var and a function in a module | EE0169 | no | yes | live (VK "2202@1:8") | syntax | delete | same |
| B30 | `JBE:621-634` (2201) | lexical name declared twice in a module | EE0168 | **yes** (`JP:3806`) | yes | dead (a second check) | syntax | delete | same |
| B31 | `JBE:635-643` (2202) | lexical and var in a module | EE0169 | **yes** (`JP:3818`) | yes | dead | syntax | delete | same |
| B32 | `JBE:660-676` (2402) | an export name given twice | EE0170 | no | no (`JSV:345`) | live (VB T041623 et al.) | semantics | keep | same (outcome; Broiler.JS at K) |
| B33 | `JBE:678-690` (2403) | an export clause naming no declared binding | EE0171 **and** EE0182 (a reserved word is never declared) | no | EE0182 **yes** (`JSP/FastParser.Export.cs:165-176`); EE0171 no | live (VK "2403@1:10"; VB T041636) | EE0182 syntax, EE0171 semantics | **keep the site** for EE0171; the library refuses EE0182 first; D20's "EE0182's reserved local name in an export" names no member of its own | same (PD13) |
| B34 | `JBE:695-720` `Declares` (2201, at 0:0) | an imported name also declared | EE0168, EE0169 | no | yes for a lexical name (`FastScopeItem`); a `var` is the compiler's (`ValidateModuleEarlyErrors` `JSV:108-114`, "Identifier … has already been declared", stage E; `JSV:258` was the catalogue's ec494f66 line) | live (VK:"2201@0:0") | syntax | delete; the library's place is real, not 0:0 | same (outcome; Broiler.VM's place is 0:0) |

Each deleted check has a mirror in the lowering, a `BinderAdmitted` assertion (for example
`JsCompiler.Exceptions.cs:259`, `:270`, `JsCompiler.Statements.cs:217`, `:237`, `JsCompiler.Expressions.cs:106`,
`:197`, `:304`, `JsCompiler.Operators.cs:150`, `:393`, `JsCompiler.Modules.cs:141`, `:359`), which then asserts that
the front end admitted the construct and stays or goes with the check (F14).

### D-e. Does Broiler.VM's binder still refuse `L1: L2: while (c) { continue L1; }`?

**Yes: it still refuses it, and the run shows it.** PD01 (`var c = false;` / `L1: L2: while (c) { continue L1; }`)
is refused 2208:2:21 in B1, in both variants and both forms, and RD01 (`continue L1` in a `for` under `L1: L2:`) fails
in Broiler.VM's conformance root with "the front end refused the source: 2208:IllegalContinue at 7:44"; Broiler.JS
accepts PD01 at P and K and runs RD01 to `RD01 3`. The checks lane pins the refusal (VK:159). The catalogue's "both" for
EE0082, EE0137, EE0166 and EE0176 is stale for Broiler.JS since #1075 and wrong for the specification (F30).

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| D30 | `continue` naming an outer label of a loop's label set | the parser checks no jump target (`JP:2154-2176`). The binder: a label whose body is not a loop or `switch` is pushed as an exit of kind `Label` (`JBW:653-666`), so only the innermost label `L2` becomes the loop's own; `Target` (`JBW:1455-1477`) accepts a labelled `continue` only at an exit of kind `Loop` with that label, so `continue L1` finds none and `JBW:689-702` reports 2208 at the `continue`. `SC/BinderChecks.cs:127-130` pins the refusal: "continue to the outer label of a label set stays refused", `a: b: while (1) { continue a; }` → `2208@1:19` | `JSV:560-597` (`ControlFlowValidator.VisitLabeledStatement` looks through nested labels: #1075, `10ce4120`); the compiler continues the loop (`JSC/FastCompiler.cs:509-522` `LoopScope.GetContinue`; `JSC/Statements/FastCompiler.VisitLabeledStatement.cs:60-76` `PrefixesLoop`) | **yes, still refused**: `2208` at the `continue` (VK:159 `2208@1:19`, the retained run at the head's product) | accepted; the loop continues | PD01 (`2208:2:21` expected), PD02 (control), RD01; VK:159. No test262 file stacks labels on a loop (searched `test/language`); JB predates #1075 | **correction of Broiler.VM** (the specification's ContainsUndefinedContinueTarget adds every label of the set to the iteration set). CAT EE0082/EE0137/EE0166/EE0176 ("both"; `completeness.md:235-236`) is wrong for JS since #1075 and for the specification. The rows are `semantics` (neither parser checks jump targets), so the correction is the binder's (`Target`) or SJL-15's jump-target fact, and the checks row changes with it |

### D-f. Early errors where the engines disagree on the parse corpus (the corpus join's `D:` buckets)

| Row | Behaviour | Broiler.VM site | Broiler.JS site | Broiler.VM answer | Broiler.JS answer | Baseline | Class |
|---|---|---|---|---|---|---|---|
| D31 | **Stage E**: VM refuses at parse or bind, JS accepts at P and refuses at K (its compiler); the largest bucket. By JS message: an invalid assignment target (EE0033-EE0035, EE0039: `JSC/Expressions/FastCompiler.VisitAssignmentExpression.cs:337`); a private name undeclared, on `delete` of a private reference (VM 2205) and elsewhere (VM 2206 or 2101); a `super` call or property (`Visit*`); `use strict` with non-simple parameters (EE0072 et al., `FastCompiler.EarlyErrors.cs:19`); strict binding names (EE0005/EE0006, `JSV:791-794`, `:871`, `:937`, `:970`); duplicate parameters (`JSV:813`); `arguments` in a field (`Visit*`); duplicate private names (`JSV:1374`); a rest with an initialiser (`JSV:863`); labels and jumps (`JSV:523-568`, `:1206`); strict assignment to `eval`/`arguments` (`JSV:1020`, `:1031`); functions in statement position (`JSV:1271`); update targets (`JSV:987`); `with` (`JSV:1053`); template escapes; legacy octal (`JSV:36`, `:1044`); duplicate exports; `import.meta`; `new.target` | `JP` and the binder (B-table) | `JSV`, `FastCompiler.EarlyErrors`, `Visit*`, `DES` | refused | P accepted, K refused | `analysis/rows-P-baseline.tsv.gz` bucket `D:stage-E`; e.g. T030017, T034864, T030387, T029210, T034967 | same (outcome); the stage is task 1.2's (`js_stage`) and SJL-14's classes E and T (D45, D64) |
| D32 | `new.target` in a class field initialiser at function depth 0 (alone, or in an arrow in it) | `ParseFieldTail` `JP:3334-3370` parses the initialiser without raising `functionDepth` (raised only at `JP:2505`, `:2835`), so `ParseNewTarget` `JP:5680` refuses; the binder's frame for the initialiser's function admits it (`JBW:1093-1097`) | `inMemberInitializer` (`JSC/Declarations/FastCompiler.CreateClass.cs:569-579`), arrow-transparent (`FastCompiler.CreateFunction.cs:256-262`), admits it (`VisitMeta.cs:24-25`) | refused 2101 at `new` (B1 1:15 / 1:21; B3 E0 and E1 likewise; E2-E7 accepted) | accepted | PD03, PD04 (no test262 file has it: only static blocks, `test/language/statements/class/static-init-expr-new-target.js`) | **correction of Broiler.VM** (the Script rule's `Contains` does not look into field initialisers; PerformEval's likewise); the library takes it at step 1 (part B class F; part A has no file) |
| D33 | escaped reserved words and contextual keywords as names | `RefuseEscapedReservedWord` `JP:6575-6598` and the contextual sites (T23) | `FastScanner.CommitIdentifier` admits them (T23) | 2209 | accepted | bucket EE0010-EE0014 (e.g. T030364, T030421) | correction of Broiler.JS (T23 holds the sites) |
| D34 | `delete` of a private reference | binder B06 | not refused (P, K accept) | 2205 | accepted | bucket EE0030;EE0031 (e.g. T034871, T034874) | correction of Broiler.JS |
| D35 | lexical redeclarations JS misses (async functions and generators in blocks, `catch` and others) | `ValidateDeclaredNames` `JP:3780-3829` | `FastScopeItem.AddVariable` misses these forms | 2201 | accepted | bucket EE0040/EE0062/EE0078/EE0133/EE0160/EE0168/EG0004 (e.g. T028483-T028488); EE0041/EE0063/… (e.g. T028542); EE0066 (e.g. T051316, T051317); EE0051/EE0056 (e.g. T049564) | correction of Broiler.JS |
| D36 | declarations and labelled functions where a single statement stands | `ParseNestedStatement` `JP:4334-4431` | missing (P11) | 2101 | accepted | bucket EE0064/EG0003/EG0005/EX0008 (e.g. T047471, T047472); EE0045-EE0061 (e.g. T051787) | correction of Broiler.JS (P11 holds the grammar half) |
| D37 | `yield`/`await` as names, strict reserved words, static-block `await` | `JP:5889`, `:6236`, `:6610-6666` | missing at the sites the bucket names | 2209 | accepted | buckets EE0006/EE0007/EE0140/EX0008/EX0023 (e.g. T029485, T029501); EE0012 (e.g. T039150); EE0006-EE0009 (e.g. T030905); EE0006/EE0007/EX0008 (e.g. T039159); EE0006-EE0009/EE0087/EE0155/EX0008 (e.g. T039301); J001072, J001780 | correction of Broiler.JS |
| D38 | `yield`/`await` expressions in parameters | `ParseYield` `JP:4587`; `ParseUnaryCore` `JP:5252` | missing | 2101 | accepted | buckets EE0083/… (e.g. T035157); EE0084/… (e.g. T039300) | correction of Broiler.JS |
| D39 | private name `#constructor`; parameter/lexical collision; `let` bound by `using` heads; assignment and update targets JS admits | `ValidatePrivateElement` `JP:3156`; `ValidateParameters`; `ValidateBindingList` `JP:4046`; B07, B18, B21 | missing | 2209, 2201, 2205 | accepted | buckets EE0132 (e.g. T034987); EE0073/… (e.g. T030391); EE0042/EE0055/EP0001/EP0004 (e.g. T049531); EE0033-EE0035/EE0039/EG0008 (e.g. T038110); EE0028/EE0029/EG0008 (e.g. T039592); EE0054/EG0008 (e.g. T001070 crashes at K); EE0036/EE0039 (e.g. T047589); EE0130/EE0138 (e.g. T047157); EE0003 (e.g. T041212); EX0025 (e.g. T040667, T12) | correction of Broiler.JS |
| D40 | an import specifier that is a reserved word or a string, written without `as` | `ParseImportClause` `JP:975-1011` takes the `ModuleExportName` as the local name without `BindingName`; the binder checks only `arguments`/`eval` (B26) | `JSP/FastParser.Import.cs:118-150` requires `as` after a string or a token `IsKeywordPropertyName` types on its own (`true`, `false`, `in`, `instanceof`, `null`; `FastParser.PropertyName.cs:17-22`). Every other reserved word reaches it as an `Identifier` token carrying a keyword, which `Identitifer` (`FastParser.Identifier.cs:11-20`) takes, and `RejectReservedImportedBinding` (`FastParser.Import.cs:210-219`) refuses only `await` and the strict-mode restricted names, so `import { if } from …` is accepted | accepted (no check) | a string: refused ("Expecting keyword as"). A reserved word such as `if`: **accepted** at P and K (PD14) | PD14, PD15 (no test262 file) | **correction of both**: of Broiler.VM, which admits a string and a reserved word written without `as` (PD15, PD14); of Broiler.JS, which admits a reserved word other than `true`, `false`, `in`, `instanceof` and `null` (PD14). ImportSpecifier needs an ImportedBinding; P's grammar, met here |
| D41 | super property in a function-valued property that is not a method | binder B08 | missing (`Visit*` admits it in a property's function) | 2101 | accepted | J000011 | correction of Broiler.JS |
| D42 | duplicate parameters in a sloppy async function or generator | none (sloppy simple lists admit duplicates) | `JSV:813` refuses | accepted (sloppy) | refused | J000072, J001979 (sloppy) | correction of Broiler.JS (CAT records it as a finding, `early-errors.md` section 2) |

### D-g. Catalogue rows with a parse-time site, each named

Every catalogue row with a parse-time site in either engine (Broiler.VM's tokenizer or parser, Broiler.JS's scanner or
parser) that no other row holds. The catalogue's site cells are its own heads' (`early-errors.md` section 4: Broiler.VM
`d82b07a`, Broiler.JS `ec494f66`) and are stale (D28): `JsParser`'s lines have moved by +42 to +103, `SliceTokenizer`'s
by +291 to +443, `FastScanner`'s by +52 and `SyntaxValidation`'s by +9 to +19 (#1075 and SJL-12.5 among the causes).
Each site below was carried to the heads by the text of the cited line (`analysis/relocated.tsv`), then opened and
named by member. The baseline column reads VB's B1 and JB's P and K for the inputs the catalogue's test262 cell names
(`analysis/own-samples.tsv`); `analysis/samples.tsv.gz` lists, per row, every input the parse-negative table ties to
it. The appendix maps every catalogue row to the rows that hold its sites.

| Row | Catalogue rows | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| D43 | **Lexical refusals of both tokenizers**: EE0001, EE0002 (an escaped identifier code point that may not start or continue one), EX0001 (a code point beginning no token; `\` not followed by `u`), EX0002 (unterminated comment), EX0003 (malformed numeric literal), EX0004 (unterminated string), EX0005 (an undefined string escape), EX0006 (unterminated regular expression), EX0007 (unterminated template) | `SliceTokenizer` (VMT): `ReadIdentifierEscape` `VMT:1087-1129` (EE0001/EE0002 `:1115`, through `JsCompilerUnicode.Lexical`; EX0001's missing `u` `:1091`); `ReadPunctuator` `:2771` (EX0001); `SkipTrivia` `:940` (EX0002); `RefuseNumeric` `:1576-1578` (EX0003, every malformed literal); `ReadStringLiteral` `:1612` (EX0004), `:1707` and `:1729` (EX0005); `ReadRegularExpressionLiteral` `:2228` (EX0006); `ReadTemplateLiteral` `:2296` (EX0007). The tokenizer stops at its first refusal, and the parse never starts (D02) | `FastScanner` (JSS): `ReadIdentifierEscape` `JSS:1788-1841` (`:1838` start or part, `:1791` no `u`); `_ReadToken` `:758` (`UnexpectedCharacter`); `SkipMultilineComment` `:1625`, `:1635`; `ReadNumber` `:1854`, `:1878`, `:1889`, `:1894`, `:1911`; `ReadString` `:1692`, `:1697`; `ScanEscaped` `:876`, `:897`, and `ScanUnicodeCodePointEscape` `:1008`, `:1022`, `:1028`; `ReadTemplateString` `:1190`. For EX0006, `ReadCommentsOrRegExOrSymbol` declines the literal (`:1380`, `:1391`), and the parser refuses the `/` (`FastTokenStream.cs:22`). All of these run at parse time | 2001 to 2007 at the token's start | P refused: the scanner's message, or an unexpected token after it (EX0003, EX0005, EX0006; anchors are N10's) | agree, both refused at P: T040665 (EE0001, `2001:15:5`), T040797 (EE0002), T040800 and T051983 (EX0001), T028581 (EX0002), T041214 (EX0003), T041059 (EX0004), T041558 (EX0005), T041055 (EX0006). EX0007 has no corpus input: **PD18** | same. T3, T10, T11, T14, T17, T25 and T29 hold the lexical grammar |
| D44 | EE0004: legacy octal and non-octal decimal escapes in a strict string | `RefuseLegacyEscape` `JP:7257-7262` (`:7258`, 2210), called from `Advance` for every string in strict code and from `ParseDirectives`' retroactive check (T15) | `SyntaxValidation.ValidateProgram` `JSV:21-52` (`:36`, a rescan of the whole program for a strict program); `StrictModeValidator.VisitLiteral` `JSV:1038-1048` (`:1044`); `FastCompiler.EarlyErrors.ValidateFunctionEarlyErrors` `:13-22` (`:22`, a function's own directive). All stage E | 2210 | P accepted; K refused | D31 (stage E): T041580 strict `2210:27:1`, T041579, T041573 (JB K: "Unexpected legacy octal literal in strict mode" / "Octal escape sequences are not allowed in strict mode") | same (outcome); JS stage E (T15) |
| D45 | EE0020, EE0022, EE0023, EE0024: a NotEscapeSequence in an untagged template's NoSubstitutionTemplate, head, tail or middle | `ParseTemplate` `JP:6708-6797` refuses at `:6766` (2005, at the template's span), with `CookEscape` deciding, chunk by chunk in source order; a tagged template keeps the chunk with no cooked value | the scanner marks the part `CookedInvalid` (`ReadTemplateString` `JSS:1147-1213`); `FastCompiler.VisitTemplateExpression` `JSC/Expressions/FastCompiler.VisitTemplateExpression.cs:15-31` refuses at `:31` (stage E) | 2005 | P accepted; K refused "Invalid escape sequence in template literal" | D31: T039979 `2005:13:1`, T039984, T039992 (EE0022). EE0023 and EE0024 have no corpus input: **PD19**, **PD20** | same (outcome); JS stage E (T27) |
| D46 | EE0016: an object-literal method named by a private name | `PropertyKey` `JP:6313-6330` refuses at `:6328` (2101) every private name read as an object-literal key, `{ #m: 1 }` included | the parser admits a private key. `StrictModeValidator.VisitClassProperty` `JSV:727-730` refuses it only when no enclosing class declares the name (stage E) | 2101 at the key | undeclared name: P accepted, K refused. A name the class declares: accepted at P and K (by reading) | D31: T039442 `2101:23:3`, T039440 (JB K "Private name is not declared in an enclosing class"); T034972 refused by both at P. **PD21** (a declared name) | same (outcome; JS stage E) for an undeclared name; **correction of Broiler.JS** for a name the class declares (PD21) |
| D47 | EE0025: CoverParenthesizedExpressionAndArrowParameterList must cover a ParenthesizedExpression | `ParsePrimary` `JP:5784-5995` refuses at `:5991` (2101, "`)` begins no expression"); `ExpectArrow` `JP:7420-7426` refuses at `:7424` (an arrow's `=>` on a later line) | `BracketExpression` `JSP/FastParser.SingleExpression.cs:235-280` refuses at `:245` (an empty list or a trailing comma) and `:255` (a rest) unless `=>` follows: parser | 2101 | P refused | T029546 (`2101:15:1`; JB P "Unexpected token )"). `parse-negative.tsv` also ties the first refusal of P07, P14, P18 and P22 inputs to this row through VM's generic `:5991`, and those rows hold them | same |
| D48 | EE0026: a template in an optional chain | `ParseCallChainCore` `JP:5397-5525` refuses at `:5489` (`?.` then a template) and `:5521` (a template later in the chain) | `SingleMemberExpression` `JSP/FastParser.SingleMemberExpression.cs:109`, `:122`; ``a?.`x` `` falls to the generic `:234`: parser | 2101 | P refused | T039526 `2101:24:4`, T039528 (both at P) | same |
| D49 | EE0032, EE0038: an object or array literal on the left must cover an AssignmentPattern | `ToPattern` `JP:7072-7208` refuses at `:7102` (a rest not last), `:7133` (a rest property not last), `:7145` (an accessor) and `:7190` (not a reference), all 2205 at parse time | `ExpressionPatternExtensions.ToPattern` `Broiler.JavaScript.Ast/VariableDeclarator.cs:12-43` (`:43`), called by the parser: partial | 2205 | P refused, except for the rest positions (P15a) | refused by both at P: T029629, T029875. JS misses (P15 bucket): T029705 and T029904 accepted at P and K; T029711 crashes K | correction of both: P15a's (Broiler.JS's rest positions) and P15b's (Broiler.VM's parenthesised nested targets) |
| D50 | Duplicate bindings and catch collisions: EE0043, EP0002, EP0005 (one `let`/`const`/`using`/`await using` list binding a name twice); EE0065 (a catch parameter binding a name twice); EE0067 and EG0006 (a `var` naming a destructuring catch parameter; Annex B.3.4 exempts a plain name) | `ValidateBindingList` `JP:4024-4062` refuses at `:4059` (2201), reached for `using` from `:1680`; `ValidateCatchParameter` `JP:3934-4000` refuses at `:3954` (2201) and at `:3996` (2202). Before the var check it returns for a plain parameter (B.3.4) | `FastScopeItem.AddVariable` `JSP/FastScopeItem.cs:94-188`: `:121` for a second lexical binding, `:188` for a `var` hoisting through a catch pattern's scope (`FastParser.Statement.cs:481-487`). A plain catch parameter is never registered (`:477`). All parser | 2201 / 2202 | P refused | EE0065: T051315 (both at P). EE0067 and EG0006: positives of the exemption only (T001083, T001086, T000558, accepted by both). EE0043, EP0002 and EP0005 have no input: **PD22**, **PD23**, **PD24** | same |
| D51 | Duplicate parameters: EE0068, EE0069, EE0070, EE0090, EE0097, EE0111, EE0147 | `ValidateParameters` `JP:4156-4230` refuses at `:4222` (2201, at the parameter's span). Unique lists come from methods, accessors and arrows (`:4964`); otherwise the list must be strict or non-simple | `StrictModeValidator.VisitFunctionExpression` `JSV:786-813` refuses at `:813`. `alwaysRejectDuplicates` (`:806-810`) covers arrows, generators, async functions, methods and accessors (`_inMethodProperty`, `:769-777`) and non-simple lists. Stage E | 2201 | P accepted; K refused "Duplicate parameter name not allowed in this context" | D31: T029486 `2201:33:8`, T050225, T050459, T042556, T042498. EE0090 has no input: **PD25**. D42 holds JS's over-refusal | same (outcome); JS stage E |
| D52 | `eval` or `arguments` as a strict function's name: EE0071, EE0098, EE0112, EE0148 | `ParseFunctionRest` `JP:2344-2430` refuses at `:2424` (2209, at the function's span), judged by `body.IsStrict` (`:2421`). In code already strict, `BindingName` `JP:6630` refuses first, at the name | `VisitFunctionExpression` `JSV:789-791` (`functionStrict` is the enclosing strictness or the body's directive): stage E | 2209 | P accepted; K refused "Invalid function name in strict mode" | D31: T050448 `2209:17:1`, T050444, T030907, T042495. EE0098 has no input: **PD26** | same (outcome); JS stage E |
| D53 | A label nested inside itself: EE0080 (function body), EE0135 (static block), EE0164 (script), EE0174 (module) | `ParseStatement` `JP:626-654` refuses at `:641` (2201, at the inner labelled statement). The set `labels` (`JP:436`) is the enclosing body's, with a new one per body (`JP:2842-2853`) | `ControlFlowValidator.VisitLabeledStatement` `JSV:560-597` refuses at `:568`. Its stacks reset at function boundaries, and it visits switch clauses (`:615-636`). Stage E. #1075 changed the same member's continue targets (D30), not this check | 2201 | P accepted; K refused "Label 'a' has already been declared" | D31: T047159 (static block) `2201:21:8`, T041630 (module) `2201:17:3`. EE0080 and EE0164 have no input: **PD27**, **PD28** | same (outcome); JS stage E |
| D54 | `use strict` over non-simple parameters: EE0085, EE0088, EE0091, EE0095, EE0099, EE0109, EE0113, EE0141, EE0145, EE0159 (D31 names EE0072 and the rest) | `ValidateParameters` refuses at `JP:4181` (2101, at the first parameter), called from `ParseFunctionBody` (`JP:2871`, `DeclaresUseStrict(directives)`) | `FastCompiler.EarlyErrors.ValidateFunctionEarlyErrors` `:13-19` refuses at `:19`, as each function is compiled: stage E | 2101 | P accepted; K refused "Illegal 'use strict' directive in function with non-simple parameter list" | D31: T029547 `2101:19:10`, T039452, T039445, T039390, T050727, T031502, T042546, T043260, T042490, T030383 | same (outcome); JS stage E |
| D55 | Class body rules: EE0122 (two constructors), EE0123 (a private name twice), EE0125 (a special `constructor`), EE0127 (static `prototype`), EE0128 (a field named `constructor`), EE0129 (a static field named `prototype` or `constructor`) | `ValidateClassBody` `JP:3059-3145` (called after the body, `JP:3019`) refuses at `:3089` (static `prototype`), `:3108` (static field `constructor`), `:3125` (a non-static `constructor` that is not a plain method) and `:3139` (a second constructor); `ValidatePrivateElement` `JP:3150-3190` refuses at `:3185`. All 2201 | `StrictModeValidator.ValidateClassEarlyErrors` `JSV:1308-1374` refuses at `:1319` (EE0122), `:1336` (EE0127, EE0129), `:1342` (EE0128, EE0129), `:1348` (EE0125's generator and async half) and `:1374` (EE0123). It is called only from the class visit (`JSV:942`), at stage E. EE0125's accessor half is the parser's (`FastParser.ObjectLiteral.cs:324`). **StrictModeValidator does not enter a `case` clause's statements**: `VisitSwitchStatement` `JSV:1074-1098` calls the base, whose `VisitCase` is a no-op (`Broiler.JavaScript.Ast/AstVisitor.cs:572`). So a class in a case clause is not checked (by reading) | 2201 | P accepted; K refused (stage E). In a case clause, accepted (by reading) | D31: T046761 `2201:24:3`, T046782, T046825, T046836, T045922, T045924. T046826 (an accessor named `constructor`) is refused by both at P. **PD29** (a case clause) | same (outcome; JS stage E); **correction of Broiler.JS** for a class in a case clause (PD29) |
| D56 | EE0178: an export name that is not well-formed Unicode | `ModuleExportName` `JP:1430-1460` refuses at `:1441` (`IsWellFormed` `:1402`; 2101). This covers either side of `as`, in import and export clauses, and after `export * as` | `ModuleExportNameLiteral` `JSP/FastParser.Import.cs:186-200` refuses at `:200`, called from `:126`, `FastParser.Export.cs:105` and `:244`: parser | 2101 | P refused "A module export name must be well-formed Unicode" | T041720 `2101:21:10`, T041722, T041724 (both at P) | same. Owner: the library at SJL-13 (T's H11, against JSD-0071's SJL-15). EX0029 reuses the text (D18-5) |
| D57 | EE0181: a string as an export clause's local name without `from` | `ParseExportClause` `JP:1064-1120` refuses at `:1117` (2101) | `Export` `JSP/FastParser.Export.cs:149`, `:167-168` (remembered at `:203`): parser | 2101 | P refused | T041723 `2101:21:10` (both at P) | same |
| D58 | EG0001: HTML-like comments in a module | the tokenizer is built with `HtmlLikeComments = Goal != Module` (`JC:836`), so `<!--` reaches `ParsePrimary` `JP:5991` (2101) and `-->` is refused at its `>` | `htmlLikeComments = !options.IsModule` (`JSS:45`, `:166`, `:764`, `:770`); the result is refused as an unexpected token (`FastParser.SingleExpression.cs:224`): parser | 2101 | P refused | T041621, T041622 (both at P) | same (T4, R07) |
| D59 | EG0007: `for (var x = e in o)` in strict code | `ParseFor` `JP:1899-…` refuses at `:1997` (2101): an initialiser is admitted only for `var` with `in`, in sloppy code, with a plain name (`:1993-1995`) | the parser refuses the forms B.3.5 never admits (`FastParser.ForStatement.cs:346` for-of, `:349` let/const, `:359` a pattern). `StrictModeValidator.VisitForInStatement` `JSV:1164-1181` refuses the strict `var` form at `:1181` (stage E) | 2101 | strict `var` form: P accepted, K refused; the other forms: P refused | T001074 strict `2101:15:10` (P accepted, K refused: D31); T001071, T001072, T001075 (both at P) | same; JS stage E for the strict half |
| D60 | EX0010: an import or export declaration in a Script | `RequireModuleGoal` `JP:1383-1395` refuses at `:1390` (2401), from `:824` (import) and `:1023` (export). `Run` `JC:953-957` refuses a module unit whose options say Script, which is a request inconsistency. A nested import or export is 2101 at `ModuleItem` `JP:804` (EX0008) | `RequireModuleItemPosition` `JSP/FastParser.Statement.cs:683-686` refuses at `:686` with one message, for a script and for a nested item: parser. The compiler checks again (`FastCompiler.Module.cs:206`, `:253`) | 2401 | P refused "'import' may only appear at the top level of a module" | T040488, T040489 (`2401:21:1`, both at P) | same |
| D61 | EA0010, EA0011, EA0012: the dynamic function's parameters, body and assembled source | P26 holds them. `FromSource` `VMP/JsRealm.Dynamic.cs:159-187` assembles one source and evaluates it as indirect eval; the wide front end refuses it (`Run` `JC:846`, `:875`, `:889`, `:929`), and the refusal is a SyntaxError (`JsEngine.Eval.cs:133-135`) | `CreateDynamicFunction` `Broiler.JavaScript.BuiltIns/Function/JSFunction.cs:1288-1361` compiles the parameters (`:1358`), the body (`:1359`) and the whole (`:1361`) | SyntaxError | SyntaxError | run time only: T000031, T006715 and T006548 parse as programs in both. P26's probes PR01, PR02, PR05 | correction of both, which SJL-13 does not deliver (P26's class; hand-off H18) |
| D62 | EP0015: `ShadowRealm.prototype.evaluate` of a text that is not a Script | `JsRealm.ShadowRealm.cs:74-85` (`:84`) calls `ShadowRealmEvaluate` (`JsEngine.ShadowRealm.cs:74-81`), which calls `LoadEvalCode(EvalRequestFlags.None, text)` (`:77`; `JsEngine.Eval.cs:411-430`). A provider refusal is thrown as a SyntaxError at `:427-429`, and the executor checks private names (`JsEngine.ShadowRealm.cs:81`) | `JSShadowRealm.cs:107` (`CoreScript.Compile`); a failure throws `NewSyntaxError` at `:112` | SyntaxError of the calling realm | the same | run time only: T015673 and T015674 parse in both. **RD02** | same. The request is an eval request with no inherited context (R's D16) |
| D63 | **Broiler.JS's `StrictModeValidator` does not enter a `case` or `default` clause**: the 55 CAT rows whose JS site is in it (`JSV:702-1681`; at `ec494f66` `:683-1662`), e.g. EE0004, EE0005, EE0016, EE0060, EE0068-EE0071, EE0122-EE0129, EE0167, EA0009, EG0007 (the list: `analysis/catalogue-index.tsv`, column `js_strictmode`) | every VM check is the parser's or the binder's, and both walk a clause's statements (`ParseSwitch` `JP:2266-2338`; the binder's statement walk) | `ValidateProgram` runs `new StrictModeValidator(…).Visit(program)` (`JSV:51`). Its `VisitSwitchStatement` (`JSV:1074-1098`) checks only duplicate and labelled function declarations in the clauses, then calls `AstReduce.VisitSwitchStatement` (`Broiler.JavaScript.Ast/AstVisitor.cs:476-483`). That method's `VisitCase` is a no-op (`:572`), so no other check of the validator reaches a clause's statements. `ControlFlowValidator` does visit them (`JSV:615-636`), and `FastCompiler.EarlyErrors` runs per compiled function, so neither is affected | refused, as outside a switch | accepted at P and K for the validator's checks (by reading). The CAT's "group 3's finding" for EE0122-EE0129 is this case, and the gap is wider | **PD29** (EE0122), **PD30** (EE0060: `with` in strict code; VM `2101` at `ParseWith` `JP:1846`) | **correction of Broiler.JS** (SJL-14: the library's checks replace the validator's; this is class F on Broiler.JS's side, not class E, because Broiler.JS accepts these sources today) |

EE0044's two halves are B13's (the parser refuses `using` and `await using` without an initialiser, the binder `const`;
F29), and EE0179 is B27's. The catalogue rows that no row of this file holds, because neither engine has a site for
them before the lowering or any site at all, are EA0008, EX0014, EX0015, EX0016, EX0018, EX0019 and EX0022 (the
appendix gives each its pointer).

### D-h. The mapping's split-row sites, re-read at the head

`MAP` says `sites-read-at: d82b07a` (`MAP:16`). The sites its split rows cite in their `meaning` and `notes` cells were
each carried to `4cf9ca4` by the line's text and opened (`analysis/mapsites.tsv`). The relation-split rows that cite no
site (super, regular expressions, private names, `arguments`, EE0019) are in D-b's adjacent table with their sites at
the head.

| `MAP` cites (at `d82b07a`) | Rows | At the head | What the site is now |
|---|---|---|---|
| `JsParser.cs:1857`, `:2323`, `:3757`, `:4207`, `:4468`, `:5619`, `:5828`, `:6604`, `:7235`, `:7364` | EX0023; EX0008; EE0168; EX0008; EE0035; EE0163, EE0173, EA0002, EP0016; EX0008; EE0087, EE0155; EE0087, EE0155; EX0028 | `JP:1918`, `:2384`, `:3818`, `:4268`, `:4529`, `:5680`, `:5889`, `:6666`, `:7277`, `:7406` (D-b's `Refuse` lines) | the same refusals, moved by +42 to +62 lines |
| `JsCompiler.Exceptions.cs:261` | EX0032 | `BinderAdmitted` assertion `JsCompiler.Exceptions.cs:259` | the refusal is the binder's: `JBW:728-736` (B03) |
| `JsCompiler.Exceptions.cs:275` | EX0033 | assertion `:270` | `JBW:737-745` (B04) |
| `JsCompiler.Expressions.cs:107` | EE0163, EE0173, EA0002, EP0016 | assertion `JsCompiler.Expressions.cs:106` | `JBW:1020-1031` (B10, dead behind `JP:5680`) |
| `JsCompiler.Expressions.cs:314` | EE0140, EX0008 (other 2104) | assertion `:304`. The 2104 at `:324-326` is EX0024's default arm, not this check | `JBW:822-833` (B05, dead) |
| `JsCompiler.Operators.cs:152` | EE0030, EE0031 (other 2205) | assertion `JsCompiler.Operators.cs:150` | `JBW:836-844` (B06, live) |
| `JsCompiler.Operators.cs:770` | EE0035 (other 2104) | assertion `:744` | `JBW:1288-1299` (B17, dead) |
| `JsCompiler.Patterns.cs:173` | EX0008 (other 2205) | assertion `JsCompiler.Patterns.cs:171` | `JBW:1329-1338` (B19, dead) |
| `JsCompiler.ResourceScopes.cs:278` | EP0006, EP0007, EP0008 (other 2104) | assertion `JsCompiler.ResourceScopes.cs:279` | `JBW:1192-1204` (B14, dead) |
| `JsCompiler.Statements.cs:219` | EX0009 (other 2203) | assertion `JsCompiler.Statements.cs:217` | `JBW:1172-1181` (B12, live) |
| `JsCompiler.Modules.cs:443` | EE0169 (other 2201) | assertion `JsCompiler.Modules.cs:359` | `JBE:695-720` `Declares` (B34, live, at 0:0) |
| `JsCompiler.Classes.cs:420` | EX0031 (2206) | the language rule is the binder's, `JBE:205-222` (B23). The lowering keeps a 2206 only past the scope-depth ceiling, after EX0013's 2103 (`JsCompiler.Classes.cs:367`), and asserts at `:376` | split between the binder and a ceiling consequence |
| `JsCompiler.Modules.cs:62` | EX0020 | `:62` (the code; `Refuse` at `:60`) | unchanged (critic-vm W7) |
| `JsEngine.Eval.cs:514` | EA0009 | `:514` | unchanged (the executor's `ThrowSyntaxError`) |
| `JsEngine.ShadowRealm.cs:77` | EP0016, EP0017, EP0018 | `:77` | unchanged (`LoadEvalCode` with no flags) |

Three of the cited sites are unchanged at the head, and the ten `JsParser` sites hold the same refusal on moved lines.
At each of the others the lowering site the mapping names is now a `BinderAdmitted` assertion, and the refusal is the
binder's, or for EX0031 is split between the binder and a ceiling consequence. Rule 4.1's part E checker reads the
mapping's cells, not its sites. D-b's adjacent table, D-d and this table are the current list of the sites whose
constants revision 19 renumbers (F4).

### The probes, and both engines' answers as run

**Parse probes** are the rows of `D/probe-D.manifest`, run by `--parse-corpus` over every population (B3 compiles PD03
and PD04 as eval code under E0 to E7; B2 has no row) and by SJL-12's tool. **C01 and C02** run through Broiler.VM's
command-line root (`--numeric --check --all`), which prints `broiler-js: <file>: <code>:<Name> at <line>:<column>:
<message>` on standard error and names no unit. **RD01 and RD02** run as a test262 test in Broiler.VM's conformance
root and as a script in Broiler.JS's shell. Answers are written as in section T. Rows answered by a cited baseline and
not probed: D12-1 (VB T041729 to T041731), D12-5 (VB T029226), D12-6 (VB
T041701 to T041707), D12-7 (VB J000767), B26 (VB T041639 to T041642), the binder rows VK holds (VK:130-240), every D-f
bucket, D18-4's Broiler.VM answer with top-level `await` off (VK:35), the D-g rows but those the table names a probe
for, and B13's `using` half (VB T042914 to T042916, T051366 to T051368).

| Probe | Row | Input (`correspondence-runs/probes/`) | Broiler.VM, as run | Broiler.JS, as run |
|---|---|---|---|---|
| PD01 | D30 | `D/inputs/d01-label-set-continue.js` | `B1 refused 2208:2:21; B4 refused 2104:2:1 (+2)` | `P accepted; K accepted [strict,sloppy]` |
| PD02 | D30 control | `D/inputs/d02-label-set-break.js` | `B1 accepted; B4 refused 2104:1:1 (+2)` | `P accepted; K accepted [strict,sloppy]` |
| PD03 | D32, D12-2 | `D/inputs/d03-new-target-field.js` | `B1 refused 2101:1:15; B3 refused 2101:1:15 [E0,E1]; accepted [E2,E3,E4,E5,E6,E7]; B4 refused 2104:1:1 (+2)` | `P accepted; K accepted [strict,sloppy]` |
| PD04 | D32 | `D/inputs/d04-new-target-field-arrow.js` | `B1 refused 2101:1:21; B3 refused 2101:1:21 [E0,E1]; accepted [E2,E3,E4,E5,E6,E7]; B4 refused 2104:1:1 (+3)` | `P accepted; K accepted [strict,sloppy]` |
| PD05 | D12-3, D18-2 | `D/inputs/d05-bigint-wide-literal.js` | `B1 refused 2104:1:9; B4 refused 2104:1:9` | `P accepted; K accepted` |
| PD06 | D18-2, F6 | `D/inputs/d06-bigint-wide-then-error.js` | `B1 refused 2104:1:9; B4 refused 2104:1:9 (+1)` | `P accepted; K refused "Invalid left-hand side in assignment"` |
| PD07 | D12-4, D18-3 | `D/inputs/d07-bigint-wide-key.js` | `B1 refused 2104:1:4; B4 refused 2104:1:2 (+1)` | `P accepted; K accepted` |
| PD08 | D04, D05 | `D/inputs/d08-error-after-duplicate.js` | `B1 refused 2205:1:17; B4 refused 2201:1:14 (+1)` | `strict: P refused "a is already defined in current scope at 2, 14"; K refused "a is already defined in current scope at 2, 14" \| sloppy: P refused "a is already defined in current scope at 1, 14"; K refused "a is already defined in current scope at 1, 14"` |
| PD09 | D04 | `D/inputs/d09-two-conflicts-one-scope.js` | `B1 refused 2201:1:14; B4 refused 2201:1:14 (+1)` | `strict: P refused "a is already defined in current scope at 2, 14"; K refused "a is already defined in current scope at 2, 14" \| sloppy: P refused "a is already defined in current scope at 1, 14"; K refused "a is already defined in current scope at 1, 14"` |
| PD10 | D04 | `D/inputs/d10-binder-all-errors.js` | `B1 refused 2203:1:7 (+1); B4 refused 2203:1:7 (+1)` | `P refused "Missing initializer in const declaration"; K refused "Missing initializer in const declaration" [strict,sloppy]` |
| PD11 | D03 | `D/inputs/d11-break-cap.js` | `B1 refused 2207:1:1 (+63); B4 refused 2207:1:1 (+69)` | `P accepted; K refused "Illegal break statement" [strict,sloppy]` |
| PD12 | D18-4, D12-10 | `D/inputs/d12-tla-for-await.mjs` | `B1 accepted; B4 refused 2104:1:1 (+2)` | `P accepted; K refused "for await (... of ...) is only valid in async functions and modules"` |
| PD13 | B33 | `D/inputs/d13-export-reserved-local.mjs` | `B1 refused 2403:1:10; B4 refused 2104:1:1` | `P refused "'if' is not a valid exported binding name"; K refused "'if' is not a valid exported binding name"` |
| PD14 | D40 | `D/inputs/d14-import-reserved-no-as.mjs` | `B1 accepted; B4 refused 2104:1:1` | `P accepted; K accepted` |
| PD15 | D40 | `D/inputs/d15-import-string-no-as.mjs` | `B1 accepted; B4 refused 2104:1:1` | `P refused "Expecting keyword as at 1, 16"; K refused "Expecting keyword as at 1, 16"` |
| PD16 | D12-8 | `D/inputs/d16-function-private-name.js` | `B1 refused 2209:1:10; B4 refused 2104:1:1` | `strict: P refused "Expecting BracketStart at 2, 10"; K refused "Expecting BracketStart at 2, 10" \| sloppy: P refused "Expecting BracketStart at 1, 10"; K refused "Expecting BracketStart at 1, 10"` |
| PD17 | D18-5 | `D/inputs/d17-specifier-lone-surrogate.mjs` | `B1 refused 2101:1:15; B4 refused 2104:1:1` | `P accepted; K accepted` |
| PD18 | D43 (EX0007) | `D/inputs/d18-template-unterminated.js` | `B1 refused 2007:1:9; B4 refused 2007:1:9` | `strict: P refused "Unterminated template literal at 3, 1"; K refused "Unterminated template literal at 3, 1" \| sloppy: P refused "Unterminated template literal at 2, 1"; K refused "Unterminated template literal at 2, 1"` |
| PD19 | D45 (EE0023) | `D/inputs/d19-template-tail-escape.js` | `B1 refused 2005:1:1; B4 refused 2104:1:1` | `P accepted; K refused "Invalid escape sequence in template literal" [strict,sloppy]` |
| PD20 | D45 (EE0024) | `D/inputs/d20-template-middle-escape.js` | `B1 refused 2005:1:1; B4 refused 2104:1:1` | `P accepted; K refused "Invalid escape sequence in template literal" [strict,sloppy]` |
| PD21 | D46 (EE0016) | `D/inputs/d21-object-private-method-declared.js` | `B1 refused 2101:1:30; B4 refused 2104:1:1 (+5)` | `P accepted; K accepted [strict,sloppy]` |
| PD22 | D50 (EE0043) | `D/inputs/d22-let-duplicate-binding.js` | `B1 refused 2201:1:8; B4 refused 2201:1:8` | `strict: P refused "a is already defined in current scope at 2, 8"; K refused "a is already defined in current scope at 2, 8" \| sloppy: P refused "a is already defined in current scope at 1, 8"; K refused "a is already defined in current scope at 1, 8"` |
| PD23 | D50 (EP0002) | `D/inputs/d23-using-duplicate-binding.js` | `B1 refused 2201:1:19; B4 refused 2102:1:9` | `strict: P refused "x is already defined in current scope at 2, 19"; K refused "x is already defined in current scope at 2, 19" \| sloppy: P refused "x is already defined in current scope at 1, 19"; K refused "x is already defined in current scope at 1, 19"` |
| PD24 | D50 (EE0067) | `D/inputs/d24-catch-pattern-var.js` | `B1 refused 2202:1:26; B4 refused 2104:1:1 (+3)` | `P refused "e has already been declared"; K refused "e has already been declared" [strict,sloppy]` |
| PD25 | D51 (EE0090) | `D/inputs/d25-setter-pattern-duplicate.js` | `B1 refused 2201:1:10; B4 refused 2104:1:2 (+5)` | `P accepted; K refused "Duplicate parameter name not allowed in this context" [strict,sloppy]` |
| PD26 | D52 (EE0098) | `D/inputs/d26-generator-eval-name-strict-body.js` | `B1 refused 2209:1:1; B4 refused 2104:1:1 (+1)` | `P accepted; K refused "Invalid function name in strict mode"` |
| PD27 | D53 (EE0080) | `D/inputs/d27-label-duplicate-in-function.js` | `B1 refused 2201:1:21; B4 refused 2104:1:1 (+2)` | `P accepted; K refused "Label 'a' has already been declared" [strict,sloppy]` |
| PD28 | D53 (EE0164) | `D/inputs/d28-label-duplicate-in-script.js` | `B1 refused 2201:1:6; B4 refused 2104:1:1 (+1)` | `P accepted; K refused "Label 'a' has already been declared" [strict,sloppy]` |
| PD29 | D55 (EE0122), D63 | `D/inputs/d29-class-two-constructors-in-case.js` | `B1 refused 2201:1:49; B4 refused 2104:1:1 (+3)` | `P accepted; K accepted [strict,sloppy]` |
| PD30 | D63 (EE0060) | `D/inputs/d30-strict-with-in-case.js` | `B1 refused 2101:1:22 (+1); B4 refused 2104:1:1 (+2)` | `P accepted; K accepted` |
| C01 | D18-1, R11 | `D/cli/c01-bigint-numeric.js` | `broiler-js: c01-bigint-numeric.js: 2104:ConstructOutsideManifest at 1:9: a BigInt literal is not admitted by the declared feature manifest (exit=3)` | not run (no counterpart) |
| C02 | D18-1 | `D/cli/c02-bigint-numeric-then-error.js` | `broiler-js: c02-bigint-numeric-then-error.js: 2104:ConstructOutsideManifest at 1:9: a BigInt literal is not admitted by the declared feature manifest (exit=3)` | not run (no counterpart) |
| RD01 | D30 | `D/t262/test/sjl13-D/RD01-label-set-continue.js` (Broiler.VM); `D/run/rd01-label-set-continue.js` (Broiler.JS) | `FAIL test/sjl13-D/RD01-label-set-continue.js [raw] the front end refused the source: 2208:IllegalContinue at 7:44: 'continue' names no enclosing loop` | `RD01 3` |
| RD02 | D62 (EP0015) | `D/t262/test/sjl13-D/RD02-shadowrealm-evaluate-syntax.js` (Broiler.VM); `D/run/rd02-shadowrealm-evaluate-syntax.js` (Broiler.JS) | `pass test/sjl13-D/RD02-shadowrealm-evaluate-syntax.js [raw]` | `RD02 SyntaxError` |

## R. The request, switches, limits and providers

**Where each engine's code is.** Broiler.VM's request is the public `SliceParseOptions` with `JsParser`'s
constructor arguments and the tokenizer's settings beside it; eval code carries six request flags; the parse runs on
`CompilationStack`'s thread. Broiler.JS's is `FastParserOptions` (by value since SJL-12.5), built from
`JSCompilationOptions` by four presets; the direct-eval context travels as call-site constants and through
`JSEngine.Current`; the parse runs on `CompilationStack` or on the caller's stack, with `StackSegment`'s hop.

**Abbreviations in this section.** Broiler.VM: `VMC/` = `src/Broiler.VM.Profile.JavaScript.Compiler/`, `VMP/` =
`src/Broiler.VM.Profile.JavaScript/`, `VMF/` = `src/Broiler.VM.Profile.JavaScript.Format/`, `VMS/` =
`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`. Broiler.JS: `JSP/` =
`Broiler.JS/Broiler.JavaScript.Parser/`, `JSC/` = `Broiler.JS/Broiler.JavaScript.Compiler/`, `JSX/` =
`Broiler.JS/Broiler.JavaScript.Expressions/`, `JSR/` = `Broiler.JS/Broiler.JavaScript.Runtime/`, `JSG/` =
`Broiler.JS/Broiler.JavaScript.Engine/`, `JSM/` = `Broiler.JS/Broiler.JavaScript.Modules/`, `JSA/` =
`Broiler.JS/Broiler.JavaScript.Ast/`, `JSB/` = `Broiler.JS/Broiler.JavaScript.BuiltIns/`, `JSH/` =
`Broiler.JS/Broiler.JavaScript/` (the shell `BroilerJS`). The corpus join's buckets whose `row` is `R:…`
(`analysis/rows-P-baseline.tsv.gz`) are explained by rows R04, R11, R12, R13 and R18 to R20.

### The rows

| Row | Behaviour | Broiler.VM site (`4cf9ca4`) | Broiler.JS site (`d6795fc8`) | Broiler.VM answer | Broiler.JS answer | Probe or baseline | Class |
|---|---|---|---|---|---|---|---|
| R01 | The parse request: its type and what travels beside it | `VMC/SliceParseOptions.cs:60-278`, a public `readonly record struct`: `Goal` (`:176`), `AllowTopLevelAwait` (`:203`), `MaximumNestingDepth` (`:225`), `IsEval`/`EvalFlags` (`:266`, `:271`); the tree bound is a constant (`:136`). Beside it: `JsParser`'s `forceStrict` and `enclosingFunctionDepth` (`VMC/JsParser.cs:383-402`), `AdmitsBigInt` (`:421`), and the tokenizer's `HtmlLikeComments`, `StructuredTemplates`, `TemplateNestingLimit`, set by `VMC/JsCompiler.cs:834-839`, `:1070-1075` | `JSP/FastParserOptions.cs:33-112`, a sealed record: `Goal` (`:53-64`), `AllowTopLevelAwait` (`:72`), `UnicodeTables` (`:86-94`); built from `JSCompilationOptions` (`IsModule`, `AllowTopLevelAwait`) by `JSC/FastCompiler.cs:102-115`. The direct-eval context is not in it: the compile reads `JSEngine.Current` (`JSC/FastCompiler.cs:153-160`) | - | - | none (no program-observable answer) | engine-kept (each engine keeps its options as its own public face; Broiler.VM maps `SliceParseOptions` onto the request internally, D16; Broiler.JS's bridge maps `JSCompilationOptions`, D44). Fields of JSD-0080's request neither carries: an Annex B switch, a cap, an error policy (R16), proposal flags (R09), a lexer extension (R10); Broiler.JS also lacks forced strictness (R03), limits (R12, R13) and a regular-expression provider (R18) |
| R02 | Goals | `VMC/SliceParseOptions.cs:28-35`: Script and Module; eval code is the Script goal with `IsEval` (`:266-278`); a JSON module is translated to source and parsed with the module options (`VMC/JsCompiler.cs:965-997`) | `JSP/FastParserOptions.cs:53-64`: Script and Module; `Eval`, `FormalParameters`, `FunctionBody` throw `NotSupportedException` (`:59`); eval code is parsed as a Script (`JSC/DirectEvalSupport.cs:686`) | Script, Module | Script, Module | rows P03 (eval code), P04 (JSON modules), P26 (the dynamic function's parameters and body) | same (both serve the two goals JSD-0080 does not reserve) |
| R03 | Forced strictness: the harness's strict variant, and eval code in a strict caller (EA0001) | `forceStrict` makes the parse strict before the prologue (`VMC/JsParser.cs:363-371`, `:393`), from the unit's `ForceStrict` or the eval request's `Strict` flag (`VMC/JsCompiler.cs:853-855`); the parse corpus's strict variant is `ForceStrict` with the text unchanged (`VMS/ParseCorpus.cs:34`, `:80`) | no forced-strictness input; strict is text. The corpus tool prefixes the manifest's prologue (`docs/evidence/sjl-12-001/tools/parse-corpus/Program.cs:324-328`); direct eval prefixes `"use strict";\n` (`JSC/DirectEvalSupport.cs:127-128`), re-scans the tokens for a strict reserved word before an assignment operator (`:704-705`, `:743-787`) and passes `inheritStrictMode` to `SyntaxValidation` (`:714`; `JSC/SyntaxValidation.cs:34-36`) | strict-only early errors refused in a strict caller's eval | the same | probes RE01-RE05; baseline B3 `T028069` (refused 2205:17:5 under E1 and E3 only) | same (outcome). JS's prefix moves a strict eval's diagnostic line by one in its message (positions are N's); the library takes forced strictness as a switch, which the bridge passes instead of the prefix (D44) |
| R04 | Top-level `await`, Module goal (EX0023) | on by default for the module goal (`VMC/SliceParseOptions.cs:141-144`, remark `:178-200`); `awaitIsOperator = AllowTopLevelAwait` (`VMC/JsParser.cs:452-468`); off only for an embedder that states it, then refused as in a module's non-async function (2209 for `await` and `await using`, 2101 for `for await`) | `JSP/FastParserOptions.cs:48`, `:72` (Module preset on); `JSC/FastCompiler.cs:103`, `:109-115`; a host's `CoreScript.Compile` without options takes a script's options, top-level `await` off (`JSR/CoreScript.cs:91`, `:118-119`), so `isModule: true` is a module without it; the module loader turns it on (`JSM/JSModuleContext.cs:1225`); refusal "'await' is reserved in module code" (`JSP/FastParser.SingleExpression.cs:98`) | on: accepted; off: refused | on: accepted; off: refused | baseline: the P bucket `R:top-level-await-at-K`, e.g. `T042055:module`, `T042119:module` (VB accepted; JB P accepted, K refused); VCK:35 (Broiler.VM with the switch off); D's probe PD12 | engine policy (one switch of the request; EX0023 is a `request` row, D10). Same when both set it alike |
| R05 | Top-level `await` on the **Script** goal | ignored: the switch is read under the module goal only (`VMC/JsParser.cs:452-468`; `VMC/SliceParseOptions.cs:195-198`, "a script is unaffected either way"); `await using` is a declaration only where `await` is the operator (`VMC/JsParser.cs:1618-1625`) | honoured: `JSP/FastParser.cs:88`, `JSP/FastParser.SingleExpression.cs:386-416` (an await expression at a script's top level when allowed) and `:446`; `JSC/FastCompiler.cs:102` (`ScriptWithTopLevelAwait`); `JSG/JSContext.cs:2704-2717` (`EvalWithTopLevelAwaitAsync`). Without the permission the parser still admits `await using` in a block and marks the program async (`JSP/FastParser.Statement.cs:241-245`, `:620-663`), and `SyntaxValidation` refuses it as "Unexpected await" (`JSC/SyntaxValidation.cs:31-32`), the message the shell retries under the permission (`JSH/Program.cs:258-266`) | RP10 refused 2102:1:9; RP11 refused 2102:1:15; RT01, RT02 refused at compile | RP10: P accepted, K refused "Unexpected await"; RP11: P and K refused; RT01 accepted (prints `RT01 accepted`); RT02 refused (no retry) | probes RP10, RP11, RT01, RT02 | engine policy (a Broiler.JS host mode the specification does not have: Script code has no top-level `await`). Finding F8 |
| R06 | `[[HasTLA]]` read out of the parse | `VMC/JsParser.cs:226`, `:431` `SawTopLevelAwait`; read by `TryParse` (`VMC/JsCompiler.cs:1090`) and given to the binder (`:1004`) | `AstProgram.IsAsync`, set at `JSP/FastParser.SingleExpression.cs:446`, `JSP/FastParser.ForStatement.cs:55-56`, `JSP/FastParser.Statement.cs:660-663`; read by `JSM/ModuleSourceAnalysis.cs:50-51` (`HasTopLevelAwait`) and the compiler | a module awaiting at its top level compiles as an async unit | the same | baseline `T042055:module` (VB accepted with bytes; JB P accepted) | same. The library's syntax half must return the fact (F12) |
| R07 | HTML-like comments, by goal | `VMC/JsCompiler.cs:836`, `:1072` (`HtmlLikeComments` = the goal is not Module); the slice path's setting is T30's | `JSP/FastScanner.cs:166` (`htmlLikeComments = !options.IsModule`) | script: comments; module: refused | the same | baseline `T000242`-`T000249` (scripts, accepted in both), `T041620`-`T041622` (modules, refused in both, e.g. VB 2101:17:5, JB "Unexpected token Greater: > at 17, 5"), `T000026`-`T000031` (dynamic function bodies and parameters, accepted in both) | same (the wide path) |
| R08 | Annex B | no switch (`SliceParseOptions` has none); Annex B grammar on for scripts (R07; legacy octal, T14-T16; B.3.3 and B.3.4 in the binder) | no switch; on for scripts | on | on | the parse corpus's `test/annexB` rows agree but for D's stage-E rows | same (the contract's switch defaults on; neither engine sets it off) |
| R09 | Proposal flags: decorators, `accessor`, `import defer`, `import source` (EP0009-EP0014) | admits none: `@` refused 2104 by the tokenizer (`VMC/SliceTokenizer.cs:2754-2760`), `accessor x` 2102 (`VMC/JsParser.cs:3411`), the import phases 2101/2102 (`:822-866`, `:5625-5633`) | no flag: the parser admits decorators (`JSP/FastParser.Decorator.cs`, `JSP/FastParser.Class.cs:70`), `accessor` (`JSP/FastParser.ObjectLiteral.cs:100-125`) and the two import-phase call forms (`JSP/FastParser.SingleMemberExpression.cs:45-70`) whatever the request, and refuses the two declaration forms (`JSP/FastParser.Import.cs:51`, `:73`; P06b); `JavaScriptFeatureFlags` gates built-ins only (`JSG/JavaScriptFeatureFlags.cs`) | refused | decorators, `accessor` and the call forms accepted; the declaration forms refused (P06b) | baseline `T031800:strict` (VB 2104:46:13; JB accepted), `T035085:strict` (2102:18:12; accepted), `T036783:strict` (2101:26:10; accepted), `T036138:strict` (2101:50:16; accepted); rows P06, P19, P20, T22 | engine policy (the request's proposal flags, off by default and one per form, F16; configuration J sets the forms Broiler.JS parses today: decorators, `accessor` and the two import-phase call forms, D17) |
| R10 | The `m` suffix | none: `1m` refused 2003 | `JSP/FastScanner.cs:1954-1955` commits `TokenTypes.Decimal`, whatever the request | refused | accepted | row T18 (probes T18A-T18C) | engine policy (the lexer-extension parameter, SJL-D-17, D17) |
| R11 | Manifest admission and the BigInt ceiling inside the parse (EX0017, EX0028) | `VMC/JsParser.cs:421` `AdmitsBigInt`, from `JsCompileRequest.AdmitsBigIntLiterals` (`VMC/JsCompiler.cs:246`, `:867`, `:1087`): under the numeric manifest a BigInt literal is refused 2104 (`VMC/JsParser.cs:5811-5819` through `:7405-7409`); under the wide manifest a literal wider than `CeilingBigIntConstantBytes`×8 bits is refused 2104 (`:6376-6422`) | none (no manifests; literals of any width) | refused 2104 | accepted | D's probes PD05-PD07, `cli/c01`, `cli/c02` (D lists the sites) | engine-kept (admission and format ceilings; the admission walk makes them from step 1, D18) |
| R12 | The nesting bound | a request parameter: `MaximumNestingDepth`, default 64 (`VMC/SliceParseOptions.cs:71`), 1 to 512 or the constructor throws (`:149-166`); charged by `Enter` (`VMC/JsParser.cs:7308-7329`) at `ParseStatement` (`:497`) and `ParseAssignment` (`:4457`), by `EnterNesting` (`:7367-7385`) at binding patterns (`:2590`), recursing unary prefixes (`:5194`) and `new` (`:5379`), and for templates by the tokenizer's `TemplateNestingLimit` (`VMC/JsCompiler.cs:838`, `:1074`) and `ParseTemplate` (`VMC/JsParser.cs:6724-6731`); refused 2103 where the bound is passed, then the parser's cascade of 2102 | no bound: `Expression()` continues a deep descent on a fresh stack (`JSP/FastParser.Expression.cs:101-126`, R15); `PreventStackoverFlow` (`:13-19`) is a loop guard, not a bound | refused past 64 | accepted | baseline `T050099:strict`/`sloppy` (32 nested functions: VB 2103:31:85; JB accepted); `J000243` (25,000 nested conditionals: VB 2103:1:531; JB accepted); `J000799` (15,000: the same); `J001976` (12,000 parentheses before an error: VB 2103:1:72; JB refused at 1, 24009); VCK:245, VCK:247 (template nesting); probes RP07, RP08, RP09 | engine policy (a parameter of the request; Broiler.VM passes 64, Broiler.JS the library's supported maximum, D14, D47; Broiler.JS's past-the-bound inputs become listed refusals at SJL-14, D27). F28 |
| R13 | The tree-depth bound | a constant, not host-settable: `SliceParseOptions.MaximumTreeDepth` = 10,000 (`:136`), derived against `CompilationStack`'s 16 MiB (`VMC/CompilationStack.cs:77`; margin not re-measured, `:61-72`); `Deepen` (`VMC/JsParser.cs:7344-7362`) charges each node the two iterative builders add, the binary chain (`:5128`) and the member and call chain (`:5541`); rule N19 reads the constant's text | no bound | a chain past 10,000 refused 2103 | accepted | baseline `J000451` (20,000 additions: VB 2103:1:24; JB accepted); VCK:181, VCK:183 (at the bound compiled; one past refused 2103@1:9); probe RP06 (a member chain one past) | implementation-defined (an implementation limit, the library's constant, derived at stage 4e as JSD-0022 derived Broiler.VM's, D14) |
| R14 | The left-operand gap | a chain's depth starts from the depth at the chain's own start: `Continue` saves `treeDepth` (`VMC/JsParser.cs:5078`) and restores it (`:5136`); `ParseCallChainCore` likewise (`:5450`, `:5549`). A left operand's spine built by the other builder, or inside parentheses or a substitution, is not charged to the chain it begins (bundle `sjl-12-001` README:489-493; `VMS/TemplateChecks.cs:142-149`) | n/a (no bound) | at the base: RP01, RP02, RP04, RP05 accepted (trees about 10,003 deep); RP03 refused 2103:1:12014. After step 1 (D14): RP01, RP02, RP04, RP05 refused with the contract's nesting diagnostic at the bound | all accepted (no bound); from SJL-14, refused at the library's bound like every input past it (JSD-0080 D27's listed refusals) | probes RP01-RP05 (RP03's text is VCK:249's) | **implementation-defined** (an implementation limit, with R13). The specification sets no depth limit, so the base's acceptances of RP01, RP02, RP04 and RP05 are correct. What differs is the accounting. The base charges a chain from the depth at the chain's own start and restores that depth when the chain ends (`Continue`, `:5078` and `:5136`; `ParseCallChainCore`, `:5450` and `:5549`), so a left operand's spine is not charged to the chain it begins. The library charges it (D14). Step 1 therefore moves these inputs from accepted to refused. Rule 4.1 admits that move only as part B's class L and part A's C3, each listed with the depth the library reports. The home rule admits it as part A's outcome L (and part G's corpus). Broiler.JS's rule 4.4 admits it as C3. It is not part A's kind (F), which admits only "a variant failing at the base that passes after". It is not part B's class F either: F would admit the move without the depth that L lists, so one move would be readable under two classes. See F18 |
| R15 | The stack a parse runs on | `VMC/CompilationStack.cs:50-115`: a fresh 16 MiB thread per compilation, for every public entry point, `Requests` included (`VMC/JsCompiler.cs:598`, `:712`, `:775`, `:812`) and the slice path (`VMC/SliceSourceCompiler.cs:194`) | `JSX/CompilationStack.cs`: 64 MiB pooled workers (`DefaultSizeBytes`), inline below 512 characters (`InlineSourceLengthLimit`), `SizeBytes` settable and read from `BROILER_JS_COMPILE_STACK_BYTES`; used by `JSR/CoreScript.cs:103`. `JSX/StackSegment.cs:50-171`: the hop threshold from `BROILER_JS_VISITOR_SEGMENT_BYTES` (`:64-74`), a `[ThreadStatic]` retry flag (`:84-85`), `TryEnsureSufficientExecutionStack` (`:138-139`) | - | - | none; JB's header (`thread-stack-bytes 268435456`, `compile-stack-bytes 0`) | engine-kept (JSD-0080's "What each engine keeps": `CompilationStack`'s thread; "Stack segmentation, if it keeps it"; D47) |
| R16 | The diagnostic cap, and first error or all errors | all errors, at most 64 per stage (`VMC/SliceSourceDiagnostics.cs:310`; `VMC/JsParser.cs:7460-7470`) | the first error: a `FastParseException` ends the parse | many | one | D's rows (probe `d11-break-cap`) | engine policy (both are parameters of the request, D11) |
| R17 | The Unicode provider | `VMC/JsCompilerUnicode.cs:43-87`: a static `EcmaScriptLexical` over Format's tables (`FormatTables : IUnicodeTables`, Unicode 17.0.0, `VMF/JsUnicodeData.cs:49`), read by the tokenizer; two parser routing checks ask `IsLetter` by General_Category (`:66-70`) | `JSP/FastParserOptions.cs:86-94`: an optional `IUnicodeTables`, null meaning the engine's tables (`JSA/Unicode/JSUnicode.cs:23-26`; `JSA/Unicode/JSUnicodeTables.cs:40`, 17.0.0); `JSP/FastScanner.cs:167`, `:204-215` | - | - | rows T1, T10 (their baselines agree) | same (one owner, one data version). D8 makes the provider required with no default, so the bridge passes `JSUnicode.Tables` where the options pass null |
| R18 | The regular-expression validation seam | after the parse, in the binder: `VMC/JsBinder.Walks.cs:1045-1046` → `VMC/JsBinder.EarlyErrors.cs:469-519` `CheckRegExp`: the flags (`dgimsuyv`, none twice, not `u` with `v`), then `Format.JsRegExpMatcher.Compile`; refused 2101 at the literal's span, in the binder's source order; no provider in `SliceParseOptions` | in the scanner: `JSP/FastScanner.cs:1331-1510` `ScanRegEx`: the parenthesis-balance patch (`:1463-1492`), `DecodeBracedUnicodeEscapes` (`:1501-1502`), `RegExpValidator.IsValid` (`:1505-1506`; `JSP/RegExpValidator.cs:18`, `System.Text.RegularExpressions`); a literal that fails is re-read as division (`:1313-1329`) and the parser refuses later; no provider in `FastParserOptions` ("until milestone SJL-11", `:80-84`) | refused 2101 at the literal | refused at a later token ("Unexpected token …") | VCK:147, VCK:149, VCK:151 (pattern, repeated flag, `u` with `v`: 2101@1:1); `T041390` (row T24i) | engine policy (the validator is a provider of the request, D7). Broiler.VM's refusal moves to the parse (part B's S5); Broiler.JS's division fallback is T24i's, SJL-14's (S9) |
| R19 | Broiler.VM's validator decides some patterns wrongly | `Format.JsRegExpMatcher` through `CheckRegExp`: refuses duplicate named groups in different alternatives, and a braced lead surrogate before an unbraced trail in `u` mode; admits `\k` in a class of a pattern with named groups | `RegExpValidator` answers as the specification does in these cases | `T013838` etc. refused; `J000314` accepted | `T013838` etc. accepted; `J000314` refused | baseline `T013838`-`T013847`, `T014784`, `T014785`, `T016241`, `T016242`, `T051997`-`T051999` (VB 2101, JB accepted: valid duplicate names); `T052671`, `T052676`, `T052687` (staging `u`-mode: VB 2101, JB accepted); `J000304`, `J000824` (`/(?<a>a)\|(?<a>b)/`: VB 2101:1:1, JB accepted); `J000314` (`/(?<a>.)[\k]/`: VB accepted, JB refused "Unexpected token Divide") | correction of Broiler.VM (behind the provider; SJL-11.4's validate entry replaces the matcher, JSD-0081 D18; not the library's) |
| R20 | Broiler.JS's validator misses early errors | `CheckRegExp` refuses them (2101) | `RegExpValidator` admits: the modifiers' early errors (EE0186-EE0190), the `u`-to-`v` breaking changes, a quantified lookbehind, an unescaped `/` in a `v`-mode class string | refused | accepted | baseline `T041332`-`T041412` (modifiers: VB 2101:19:1, JB accepted), `T014939`-`T014966` (`u` to `v`), `T041423`-`T041426` (lookbehind), `J000305` (`/[\q{/}--[a]]/v`: VB 2101:1:1, JB accepted; a ClassSetCharacter may not be `/`) | correction of Broiler.JS (behind the provider; SJL-11.5, or K9 at SJL-14, D45; not the library's) |
| R21 | A regular-expression nesting bound (EX0026) | `MaximumNestingDepth` 512 in the matcher (`VMF/JsRegExpMatcher.cs:564`), reported 2101 through `CheckRegExp` | none | RP12 refused 2101:1:9 | accepted | probe RP12 | engine-kept (the provider's own limit; Broiler.Regex's nesting limit replaces it, JSD-0081 D18) |
| R22 | Eval code's function context: `new.target` (EA0002) | the call site records `InFunction` (`VMC/JsCompiler.Eval.cs:231-236`; an arrow keeps its definer's, `VMC/JsCompiler.Functions.cs:159-170`); the parse takes it as `enclosingFunctionDepth` 1 (`VMC/JsCompiler.cs:862-865`), the binder as `Frame.Function` (`VMC/JsBinder.Walks.cs:450-452`, `:1021-1029`) | a call-site constant `rejectNewTarget = !inMemberInitializer && !EnclosedByOrdinaryFunction(…)` (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:155-157`, `:612-623`), nested eval reading `DirectEvalRejectsNewTarget`; indirect eval rejects (`JSC/DirectEvalSupport.cs:693-695`); checked after the parse (`:309-313`) | as the specification | as the specification | probes RE06-RE12; baseline B3 `T040494` (E0, E1 refused 2101:36:3; E2-E7 accepted) | same |
| R23 | the method context: super property (EA0003) | `InMethod` (`VMC/JsCompiler.Eval.cs:238-241`) → `Frame.Method` (`VMC/JsBinder.Walks.cs:440-443`) → refused 2101 outside a method (`:940-948`) | `allowSuperProperty = scope.Top.Super != null` (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:117`) → `JSC/DirectEvalSupport.cs:278-281` | as the specification | as the specification | probes RE13-RE16; baseline B3 `T030388` (E0-E3 refused 2101:15:17; E4-E7 accepted) | same |
| R24 | the derived-constructor context: super call (EA0004) | `InDerivedConstructor` (`VMC/JsCompiler.Eval.cs:243-246`) → `Frame.Derived` (`VMC/JsBinder.Walks.cs:445-448`) → refused 2101 elsewhere (`:963-971`) | `allowSuperCall = allowSuperProperty && !inMemberInitializer && (root scope's MemberInits \|\| a nested eval's grant)` (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:134-136`) → `JSC/DirectEvalSupport.cs:264-267`. `MemberInits` is non-null in **every** class constructor's scope, a base class's included (`JSC/Declarations/FastCompiler.CreateClass.cs:705` passes `memberInits` to every constructor; `JSC/Scope/FastFunctionScope.cs:564`), so the test does not tell a derived constructor from a base one | as the specification | as the specification, except in a base-class constructor: `eval('super()')` there is admitted at parse and throws `TypeError: Value is not a constructor` at run time (RE19), where the specification has a SyntaxError | probes RE17-RE21; baseline B3 `T030387` (refused 2101:15:17 under every set but E5) | **correction of Broiler.JS** (the base-class constructor; the other cases are the same) |
| R25 | the field-initialiser context: `arguments` (EA0005) | `InClassFieldInitializer` (`VMC/JsCompiler.Eval.cs:248-251`; every field initialiser, static or not, is compiled `isFieldInitialiser`, `VMC/JsCompiler.Classes.cs:317`) → `EvaluatedInField` (`VMC/JsBinder.EarlyErrors.cs:65-66`, `:239-…`): 2206 | `rejectArguments: inFieldInitializer` (the call site's `inMemberInitializer`; static fields too, `JSC/Declarations/FastCompiler.CreateClass.cs:567-580`) → `JSC/DirectEvalSupport.cs:301-302` | as the specification | as the specification | probes RE22-RE26; baseline B3 `T028769` (E6 refused 2206:0:0; the others accepted) | same (the 0:0 place is N's) |
| R26 | the class static block | no flag of its own: a static block compiles as a method body (`isMethod: true, isStaticBlock: true`, `VMC/JsCompiler.Classes.cs:260-266`; `VMC/JsCompiler.Functions.cs:120-170`), so its eval sites carry `InFunction` and `InMethod`, not `InClassFieldInitializer` | no flag of its own: the block is compiled as a function (`CreateFunction`, `JSC/Declarations/FastCompiler.CreateClass.cs:788`), outside the `inMemberInitializer` scope a field initialiser sets (`:569-579`), and `CreateFunction` clears that flag for every non-arrow function (`JSC/Declarations/FastCompiler.CreateFunction.cs:260-262`), so the block's eval sites pass `rejectArguments: false` | RE27 undefined; RE28 "function"; RE29 "object" | RE27 undefined; RE28 "function"; RE29 "object" (the call site does not see a member initialiser) | probes RE27-RE29 (node: undefined, "function", SyntaxError) | same: both engines answer as the specification reads (PerformEval tests the body function's `[[ClassFieldInitializerName]]`, which a static block's function does not carry); node, like V8, refuses RE29. The term "static-block flags" names no input of either engine (F22) |
| R27 | the visible private names (EA0009) | an `InClassBody` flag when any scope around the site holds a private name (`VMC/JsCompiler.Eval.cs:197-229`, `:253-256`); with it the binder leaves an undeclared name to the executor (`VMC/JsBinder.EarlyErrors.cs:200-220`), which refuses before anything runs (critic-vm W5); without it the binder refuses 2206 | `privateNamesInScope`, captured at the call site (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:115`), checked by `SyntaxValidation`'s `StrictModeValidator` (`JSC/SyntaxValidation.cs:51`, `:706-711`) before the eval runs | SyntaxError before the eval's body runs | the same | probes RE30-RE34; baseline B3 `T034967` (2206 under every set but E7) | same (outcome; D16 keeps the request's set absent at SJL-13) |
| R28 | the caller's lexical names (EA0006, EA0007) | at run time: `EvalDeclarationInstantiation`'s walk by the executor (`VMP/JsEngine.EvalDeclarations.cs:69`, `:83-152`) | before the body runs: `lexicalBindings` captured at the call site (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:110`), `SyntaxValidation.ContainsDirectEvalVarConflict` (`JSC/SyntaxValidation.cs:38-43`), parameter bindings (`JSC/DirectEvalSupport.cs:715-719`), catch parameter names | SyntaxError | SyntaxError | probes RE35-RE38 | same (outcome). JSD-0080's request names the caller's lexical names; the plan does not (F21) |
| R29 | How the inherited context travels | the eval request's six flags (`JsFormat.EvalRequestFlags`) in the payload (`VMC/JsCompiler.cs:635-641`; `SliceParseOptions.Eval`), computed at the call site when the caller is compiled (`RecordEvalSite`) | constants and captured arrays passed to `DirectEvalSupport.Execute` (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:105-190`); the eval's own compile reads the rest from `JSEngine.Current` (`JSC/FastCompiler.cs:153-160`) | - | - | none | engine-kept until SJL-14's W1 (the bridge passes it in the request, D44; the `JSEngine.Current` read is on JSD-0080 D24's list) |
| R30 | The dynamic function inherits no context | the assembled source is evaluated as a global program, indirectly (`VMP/JsRealm.Dynamic.cs:33`, `FromSource`; row P26) | `JSB/Function/JSFunction.cs:1288-1358` compiles the assembled source as a Script | strictness, `new.target` legality, `super`, private names: not inherited | the same | probes RE39-RE42 | same |
| R31 | The compile meter | the realm charges before the compiler runs, 1 + bytes: `VMP/JsEngine.Eval.cs:100-102` (`Evaluate`), `:339-340` (`EvaluateScript`), `:420-421` (`LoadEvalCode`); the compiler charges no fuel outside the native backends | none | - | - | none (no program-observable answer; part B's identity carries fuel parity) | same (no meter in either parse; both pass a null `WorkCharge`, D32) |
| R32 | Ambient state in the parse path | none: options by value (`VMC/SliceParseOptions.cs:52-56`; rule N12) | options by value since SJL-12.5 (`JSP/FastParserOptions.cs:9-31`); the parser assembly holds no AsyncLocal, ThreadLocal or `[ThreadStatic]` (part B's test); outside it: R15's environment variables and `[ThreadStatic]`, `JSEngine.Current` (R29), `BROILER_JS_DUMP_PROGRAMS` (`JSC/FastCompiler.cs:149-151`) | - | - | VCK:23 (two goals in one process; the slice path, critic-vm W3); JS `docs/evidence/sjl-12-001/after/part-b-architecture-test-after.log` | engine-kept (JSD-0080 D24's list; the library takes none, SL3) |
| R33 | The module-request seam | `VMC/JsCompiler.cs:683-740` `Requests`: parses on `CompilationStack` with `forceStrict`; specifiers in source order, typed by the `type` attribute, each once | `JSM/ModuleSourceAnalysis.cs:50-51`: `FastParser.ParseModule(code, FastParserOptions.Module)`; requests keyed by specifier and type, in source order; `HasTopLevelAwait` | - | - | none (sub-population B5 is the tools commit's, D27) | same |
| R34 | The engine's other depth limits | the eval scope-depth ceiling (EX0013; `VMC/JsCompiler.Eval.cs:267-290`, 255 hops) in the lowering, which needs more nested records than the default nesting bound admits; the JSON module's depth (EX0027; `VMC/JsCompiler.cs:983-993`) | none | - | - | none (EX0013 is unreachable at the default bound of 64; JSON modules are P04's) | engine-kept |
| R35 | The parse entry points, and the options each passes | Every parse goes through `JsCompiler`. Its four public entry points run on `CompilationStack` (`VMC/JsCompiler.cs:591-598`, `:711-716` `Requests`, `:772-778`, `:808-816`). Scripts and eval code are parsed in `Run` (`:834-870`): the unit's `SliceParseOptions`; `forceStrict` from `ForceStrict` or the eval flag; `enclosingFunctionDepth`; `AdmitsBigInt`; and the tokenizer's goal-set HTML-like comments and `TemplateNestingLimit`. Modules and `Requests` are parsed in `TryParse` (`:1065-1092`, `forceStrict: true`; called at `:716`, `:997`). The slice path has its own parser (`VMC/SliceSourceCompiler.cs:194`, `:209`). Eval code and the dynamic function's assembled source reach `Compile` through the realm (R30, R31). Each text is parsed once, with no token re-scan after the parse and no strictness by text (R03) | Four parse sites and two token re-scans (inventory J1-J6): `JSC/FastCompiler.cs:131-174`, whose options come from `ParserOptions(JSCompilationOptions)` (`:100-115`); `JSC/DirectEvalSupport.cs:701-741`, `EvalOptions` = Script (`:686`); `Broiler.JS/Broiler.JavaScript.Portable.Compiler/PortableCompiler.cs:33-45`, Script; `JSP/FastParser.cs:177-192` `ParseModule`, from `JSM/ModuleSourceAnalysis.cs:48-54`, Module; and the re-scans `JSC/DirectEvalSupport.cs:743-787` and `JSC/SyntaxValidation.cs:369-396`. Every product option set is one of four presets: Script, Script with top-level `await`, Module, and Module without it. None passes `UnicodeTables` (null means the engine's tables). Eleven routes reach `FastCompiler`. An eval or `Function` text, and a source-text module, is parsed two to four times | - | - | JB's P column gives `PortableCompiler`'s and `DirectEvalSupport.Validate`'s parse answer for a script and `ModuleSourceAnalysis`'s for a module: the tool's P entry passes the same presets (`docs/evidence/sjl-12-001/tools/parse-corpus/Entries.cs:46-53`). K is `CoreScript.Compile` without options (`:73-83`): Script, or Module without top-level `await` (R04's bucket). Probes RE43-RE48 (R36) | **engine-kept** (each engine's entry points, and the options each chooses, are its own; from SJL-14 each Broiler.JS site asks the library through a request the bridge maps, D44; F25, F26). Outcomes differ only where an entry point adds a check of its own: R36 (the reserved-word re-scan), T16 (the legacy-octal re-scan), R05 (the shell's retry) and R04 (K's module without top-level `await`) |
| R36 | A strict caller's direct eval: Broiler.JS's strict-reserved-word re-scan refuses a class element named by a strict reserved word | The eval text is parsed forced-strict (R03). A strict reserved word is refused only where it is an identifier: `RefuseStrictReservedIdentifier` (`VMC/JsParser.cs:6538-6549`, 2209) is called for a reference (`:5929`), a label (`:616`) and an object literal's shorthand (`:6226`, `:6257`). A class element name is an IdentifierName and is not checked | Before the parse, `ContainsStrictReservedWordUsage` runs (`JSC/DirectEvalSupport.cs:704-705`, `:743-797`). It refuses any keyword token among `implements interface package private protected public static yield` that follows no `.` or `?.` and precedes an assignment or update operator. The scanner marks those words as keywords whatever the context (`JSP/FastScanner.cs:2052`, `:2093-2137`), so `class C { static = 1 }` (a field named `static`) and `class C { public = 2 }` match | RE43 1, RE44 2, RE45 1, RE46 3, RE47 4; RE48 refused (2209) | RE43, RE44, RE47 refused "Unexpected strict mode reserved word"; RE45 1 (no re-scan for a sloppy caller); RE46 3 (after `.`); RE48 refused (the re-scan) | Probes RE43-RE48 (node: 1, 2, 1, 3, 4, SyntaxError). Strict code outside eval is not affected: `T046622:strict` and `T034826:strict` (a field named `static`) are accepted by VB B1 and by JB P and K | **correction of Broiler.JS**. EE0012's `DirectEvalSupport` site applies the rule to IdentifierNames. The library's forced-strict parse corrects it from SJL-14 if the re-scan goes (F25) |

### R35's inventory: Broiler.JS's parse entry points at `d6795fc8`

Found by `git grep` at `d6795fc8` for `new FastParser(`, `FastParser.Parse*`, `new FastTokenStream(`, `new
FastScanner(` and `FastParserOptions.` over `Broiler.JS/` and `Broiler.ECMAScript/`, tests and benchmarks set aside,
and the callers of `CoreScript.Compile` the same way. Product code constructs a parser or a token stream at six places,
J1 to J6. A token stream always constructs its scanner (`JSP/FastTokenStream.cs:121-127`), and the parser always takes
the stream's options (`JSP/FastParser.cs:14`, `:25-44`), so each row's goal is the preset it names.

| # | Site | Reached from | Options (goal, top-level `await`) | Strictness | Inherited context | Stack | After the parse | At SJL-14 (plan) |
|---|---|---|---|---|---|---|---|---|
| J1 | `JSC/FastCompiler.cs:131-166`: parse at `:164-166` | `CoreScript.Compile` (`JSR/CoreScript.cs:67-113`) through the compiler registered at `JSC/Infrastructure/CompilerAssemblyInitializer.cs:17-27`; routes J1a-J1k | `ParserOptions` (`:109-115`) maps `JSCompilationOptions` (`IsModule`, `AllowTopLevelAwait`; `JSR/JSCode.cs:8-31`) to Script, `ScriptWithTopLevelAwait`, Module or `ModuleWithoutTopLevelAwait` (`:102-103`) | The text's own directive; a module is strict (`inheritStrictMode: isModuleCompilation`, `:173`) | Read from `JSEngine.Current` (`:153-160`: direct-eval flags, binding names, private names; R29) | `CompilationStack` (`JSR/CoreScript.cs:103-105`; R15) | `ForLoopPreparation`, then `SyntaxValidation.ValidateProgram` (`:171-174`), which runs J5 | Through the bridge (D41). The request carries the preset and, from W1, the inherited context in place of the `JSEngine.Current` read (D44) |
| J2 | `JSC/DirectEvalSupport.cs:701-741` `Validate`: parse at `:709-712` | `Execute` (`:123-130`, direct eval) and `ValidateIndirectEval` (`:693-695`, from `Broiler.JS/Broiler.JavaScript.Globals/JSGlobal.cs:132`) | `EvalOptions` = `FastParserOptions.Script` (`:686`), whatever the caller's goal or permission | A strict caller's text is prefixed `"use strict";\n` (`:127-128`; R03, N25) | Call-site constants passed to `Execute` (R22-R29). Indirect eval passes none, with `rejectNewTarget: true` | The caller's (no `CompilationStack`) | `ForLoopPreparation`, `ValidateDirectEvalEarlyErrors`, `ValidateProgram` (with J5), parameter-binding conflicts and restricted declarations (`:712-733`) | Through the bridge, with the inherited context and forced strictness in the request (D44; F24) |
| J3 | `JSC/DirectEvalSupport.cs:743-787` `ContainsStrictReservedWordUsage`: token scan (`:746`) | `Validate`, before the parse, for a strict caller only (`:703-705`) | `EvalOptions` (Script) | It scans the prefixed text | - | The caller's | Refuses "Unexpected strict mode reserved word" (`:705`). It over-refuses class elements (R36) | Plan task 4.3 (D41): walks the bridge's token chain. F25: delete it instead |
| J4 | `Broiler.JS/Broiler.JavaScript.Portable.Compiler/PortableCompiler.cs:22-45`: parse at `:38-40` | `Program.cs:13` (the `portable-compiler` tool) | `FastParserOptions.Script` | The text's own directive | None | The caller's | None: no `SyntaxValidation` and no `ForLoopPreparation` (survey-js S-5). Its parse answer is JB's P column | Reads the shared tree directly (D41, D53, plan task 4.2) |
| J5 | `JSC/SyntaxValidation.cs:369-396` `ContainsLegacyOctalLiteral`: token scan (`:375`) | `ValidateProgram` (`:34-36`), from J1 (`FastCompiler.cs:173`) and J2 (`DirectEvalSupport.cs:714`) | The options the program was parsed under: J1's preset, or J2's Script | It runs only when the whole program is strict: a module, inherited strictness, or a top-level directive | - | J1's or J2's | Refuses "Unexpected legacy octal literal in strict mode". It misses `08` and `09`, and a strict function in a sloppy program (T16) | Plan task 4.3 (D41); F25 |
| J6 | `JSP/FastParser.cs:177-192` `ParseModule` (`:186`) | `JSM/ModuleSourceAnalysis.cs:48-54`, after J1d compiled the same text (`JSM/JSModuleContext.cs:1224-1227`) | `FastParserOptions.Module` (top-level `await` on), J1d's preset | Module | None | The caller's | None ("the source must already have compiled", `:46-47`). Reads the requests, the entries and `[[HasTLA]]` (R06, R33) | Reads the shared tree directly (D41, D53, plan task 4.2) |

The routes into J1, with the `JSCompilationOptions` each passes. `JSContext.CompilationOptions`
(`JSG/JSContext.cs:131-134`) sets neither `IsModule` nor `AllowTopLevelAwait`, so "the context's options" means Script:

| # | Route | Options, and so the preset |
|---|---|---|
| J1a | `JSG/JSContext.cs:2663-2689` `Eval`, from the shell (`JSH/Program.cs:258`) and indirect eval after J2 (`JSGlobal.cs:135-147`) | The context's: Script |
| J1b | `JSG/JSContext.cs:2704-2719` `EvalWithTopLevelAwaitAsync`, from the shell's retry (`JSH/Program.cs:260-266`; R05) | The context's with `AllowTopLevelAwait`: Script with top-level `await` |
| J1c | `JSR/CoreScript.cs:130-175` `EvaluateWithTasks`, `Evaluate`, `EvaluateAsync` | `AsScript` (`:118-119`): Script |
| J1d | `JSM/JSModuleContext.cs:1224-1225`, a source-text module | `IsModule` and `AllowTopLevelAwait`: Module. A JSON module is not parsed (`:1205-1209`) |
| J1e | `JSM/JSModuleContext.cs:1216`, a CommonJS module | The context's, with the CommonJS parameters as `argsList`: Script |
| J1f | `JSR/CoreScript.cs:91-93`, a host's `Compile` without options (the tool's K) | `AsScript` of the context's: Script; with `isModule: true`, Module without top-level `await` (R04) |
| J1g | `JSC/DirectEvalSupport.cs:209-211`, direct eval's compile after J2, unless the body is empty or inert (`:135-148`) | The context's: Script; on J2's prefixed text |
| J1h | `JSB/Function/JSFunction.cs:1356-1362` `CreateDynamicFunction`: the parameters with an empty body, the body with empty parameters, the whole, then `CoreScript.Evaluate` of the whole (J1c), so four parses (P26, R30) | The context's: Script |
| J1i | `JSB/ShadowRealm/JSShadowRealm.cs:107` `evaluate` | The realm's: Script |
| J1j | `Broiler.JS/Broiler.JavaScript.Debugger/V8Runtime.cs:99`, `:137`, `:163` | The context's: Script |
| J1k | `JSB/Function/JSFunction.cs:399-441` `RecompileForTiering`: the function's source wrapped as `(…)`, prefixed `'use strict';` when the function is strict (`:420`). Only with `JSContextOptions.FunctionTiering` enabled, which is off by default (`JSR/FunctionTiering.cs:9-12`) and which the shell does not set | The realm's: Script, **whatever goal the function was compiled under**. The tiering gate has no module condition (`JSC/Declarations/FastCompiler.CreateFunction.cs:588-603`). Under the Script goal the scanner reads `<!--` and `-->` as comments (`JSP/FastScanner.cs:166`). So a module function the gate admits, whose text holds `<!--` (in a module, `a <!--b` is `a < !--b`), would compile to other code after promotion. Not probed: no shell switch reaches it (F26, R-Q5) |

Outside product code: the public constructors without options default to Script (`JSP/FastTokenStream.cs:85-88`,
`:111-114`; `JSP/FastScanner.cs:148-151`), and `ParseModule(code)` is Module (`JSP/FastParser.cs:167`). Callers: the
SJL-12 parse-corpus tool (P at `Entries.cs:46-53`, K at `:73-83`, which is J1f), the catalogue's site probe
(`docs/evidence/sjl-12-001/catalogue/probes/js/Program.cs:63-64`, as P), the benchmarks and tests (survey-js S-4.4).
Downstream, JSeal's module analysis calls `new FastParser(new FastTokenStream(in code, null))` (survey-js S-11.1, at
JSeal `ebb5e6ac`); against this API that call is the Script preset, and rule 4.4's part G judges that consumer.

What the inventory shows:
- **How many times a text is parsed.** A strict caller's direct eval: two parses (J2, then J1g) and up to three scans
  (J3, then J5 within each parse). A sloppy caller's direct eval: two parses. An indirect eval: two parses (J2, then
  J1a). A `Function` call: four parses. A source-text module: two parses (J1d, then J6), under one preset. Broiler.VM
  parses each of these texts once.
- **Off `CompilationStack`.** J2, J3, J4, J6, and J5 when J2 calls it, parse on the caller's stack; only
  `FastParser.Expression`'s `StackSegment` hop guards their depth (R15). D47's "the parse runs on a thread with the
  stack the library documents" must name them (F26).

### The probes, and both engines' answers as run

**Parse probes** are the rows of `R/parse/probe-R.manifest` (RP06 to RP12) and of `R/parse/probe-R-gap.manifest` (RP01
to RP05), the second run in a process of its own because its five inputs are trees about 10,003 deep that Broiler.VM
compiles at the base in both forms; the run lost no process (the plan's question R-Q4: the base compiles them, in both
forms). **Value probes** are the lines of `R/value/r-values.js` (RE01 to RE48), run as in section T. **Run probes** RT01
and RT02 are a script for Broiler.JS's shell and a raw test262 test, negative at parse, for Broiler.VM's conformance
root. Answers are written as in section T; Broiler.VM's message for a refused eval text is fixed ("the evaluated source
is not a program this profile admits") and carries no position. Rows answered by a cited baseline and not probed: R04
(the corpus join's `R:top-level-await-at-K` bucket, VK:35), R07, R09, R18 (VK:147 to VK:151), R19, R20, and the
B3 rows of R03 and R22 to R27 for Broiler.VM's parse under each flag set; R12 (T050099, J000243, J000799, J001976) and
R13 (J000451, VK:181, VK:183) cite those baselines beside their probes; R11 and R16 are D's probes, R10 T's; R35's parse answers per entry point are JB's P and K
columns; J1k, the tiering recompile, has no probe because no shell switch enables tiering.

| Probe | Row | Input (`correspondence-runs/probes/`) | Broiler.VM, as run | Broiler.JS, as run |
|---|---|---|---|---|
| RP01 | R14 | `R/parse/inputs/sjl13-R-gap/rp01-gap-parenthesised-chain-first.js` | `B1 accepted` | `P accepted; K accepted` |
| RP02 | R14 | `R/parse/inputs/sjl13-R-gap/rp02-gap-template-chain-first.js` | `B1 accepted` | `P accepted; K accepted` |
| RP03 | R14 control | `R/parse/inputs/sjl13-R-gap/rp03-gap-control-parenthesised-chain-last.js` | `B1 refused 2103:1:12014 (+2)` | `P accepted; K accepted` |
| RP04 | R14 | `R/parse/inputs/sjl13-R-gap/rp04-gap-member-chain-first.js` | `B1 accepted` | `P accepted; K accepted` |
| RP05 | R14 | `R/parse/inputs/sjl13-R-gap/rp05-gap-parenthesised-chain-object.js` | `B1 accepted` | `P accepted; K accepted` |
| RP06 | R13 | `R/parse/inputs/sjl13-R/rp06-member-chain-past-tree-bound.js` | `B1 refused 2103:2:9` | `P accepted; K accepted` |
| RP07 | R12 | `R/parse/inputs/sjl13-R/rp07-templates-nested-63.js` | `B1 refused 2103:1:198` | `P accepted; K accepted` |
| RP08 | R12 | `R/parse/inputs/sjl13-R/rp08-templates-nested-600.js` | `B1 refused 2103:1:198` | `P accepted; K accepted` |
| RP09 | R12 | `R/parse/inputs/sjl13-R/rp09-parentheses-nested-600.js` | `B1 refused 2103:1:72 (+63)` | `P accepted; K accepted` |
| RP10 | R05 | `R/parse/inputs/sjl13-R/rp10-script-await-using-in-block.js` | `B1 refused 2102:1:9 (+1)` | `P accepted; K refused "Unexpected await"` |
| RP11 | R05 | `R/parse/inputs/sjl13-R/rp11-script-await-expression.js` | `B1 refused 2102:1:15` | `P refused "Unexpected token Identifier: Promise at 1, 15"; K refused "Unexpected token Identifier: Promise at 1, 15"` |
| RP12 | R21 | `R/parse/inputs/sjl13-R/rp12-regexp-groups-nested-600.js` | `B1 refused 2101:1:9` | `P accepted; K accepted` |
| RE01 | R03 | `R/value/r-values.js (line RE01)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected strict mode reserved word` |
| RE02 | R03 | `R/value/r-values.js (line RE02)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected legacy octal literal in strict mode` |
| RE03 | R03 | `R/value/r-values.js (line RE03)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Strict mode code may not include a with statement` |
| RE04 | R03 | `R/value/r-values.js (line RE04)` | `9` | `9` |
| RE05 | R03 | `R/value/r-values.js (line RE05)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Invalid function name in strict mode` |
| RE06 | R22 | `R/value/r-values.js (line RE06)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected new.target in eval code` |
| RE07 | R22 | `R/value/r-values.js (line RE07)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected new.target in eval code` |
| RE08 | R22 | `R/value/r-values.js (line RE08)` | `undefined` | `undefined` |
| RE09 | R22 | `R/value/r-values.js (line RE09)` | `boolean true` | `boolean true` |
| RE10 | R22 | `R/value/r-values.js (line RE10)` | `undefined` | `undefined` |
| RE11 | R22 | `R/value/r-values.js (line RE11)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected new.target in eval code` |
| RE12 | R22 | `R/value/r-values.js (line RE12)` | `undefined` | `undefined` |
| RE13 | R23 | `R/value/r-values.js (line RE13)` | `boolean true` | `boolean true` |
| RE14 | R23 | `R/value/r-values.js (line RE14)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected super property in eval code` |
| RE15 | R23 | `R/value/r-values.js (line RE15)` | `"function"` | `"function"` |
| RE16 | R23 | `R/value/r-values.js (line RE16)` | `"function"` | `"function"` |
| RE17 | R24 | `R/value/r-values.js (line RE17)` | `1` | `1` |
| RE18 | R24 | `R/value/r-values.js (line RE18)` | `1` | `1` |
| RE19 | R24 | `R/value/r-values.js (line RE19)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS TypeError: Value is not a constructor` |
| RE20 | R24 | `R/value/r-values.js (line RE20)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected super call in eval code` |
| RE21 | R24 | `R/value/r-values.js (line RE21)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected super call in eval code` |
| RE22 | R25 | `R/value/r-values.js (line RE22)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected arguments in eval code inside a class field initializer` |
| RE23 | R25 | `R/value/r-values.js (line RE23)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected arguments in eval code inside a class field initializer` |
| RE24 | R25 | `R/value/r-values.js (line RE24)` | `2` | `2` |
| RE25 | R25 | `R/value/r-values.js (line RE25)` | `3` | `3` |
| RE26 | R25 | `R/value/r-values.js (line RE26)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected arguments in eval code inside a class field initializer` |
| RE27 | R26 | `R/value/r-values.js (line RE27)` | `undefined` | `undefined` |
| RE28 | R26 | `R/value/r-values.js (line RE28)` | `"function"` | `"function"` |
| RE29 | R26 | `R/value/r-values.js (line RE29)` | `"object"` | `"object"` |
| RE30 | R27 | `R/value/r-values.js (line RE30)` | `7` | `7` |
| RE31 | R27 | `R/value/r-values.js (line RE31)` | `THROWS SyntaxError: the evaluated source names the private name #q, which no class enclosing the call declares` | `THROWS SyntaxError: Private name is not declared in an enclosing class` |
| RE32 | R27 | `R/value/r-values.js (line RE32)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Private name is not declared in an enclosing class` |
| RE33 | R27 | `R/value/r-values.js (line RE33)` | `boolean true` | `boolean true` |
| RE34 | R27 | `R/value/r-values.js (line RE34)` | `1` | `1` |
| RE35 | R28 | `R/value/r-values.js (line RE35)` | `THROWS SyntaxError: a direct eval cannot declare var x: a lexical declaration, or a parameter the evaluation's parameter list binds, of the same name lies between the call and its variable environment` | `THROWS SyntaxError: Invalid declaration in direct eval code` |
| RE36 | R28 | `R/value/r-values.js (line RE36)` | `THROWS SyntaxError: a direct eval cannot declare var y: a lexical declaration, or a parameter the evaluation's parameter list binds, of the same name lies between the call and its variable environment` | `THROWS SyntaxError: Invalid declaration in direct eval code` |
| RE37 | R28 | `R/value/r-values.js (line RE37)` | `1` | `1` |
| RE38 | R28 | `R/value/r-values.js (line RE38)` | `2` | `2` |
| RE39 | R30 | `R/value/r-values.js (line RE39)` | `8` | `8` |
| RE40 | R30 | `R/value/r-values.js (line RE40)` | `undefined` | `undefined` |
| RE41 | R30 | `R/value/r-values.js (line RE41)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: 'super' keyword is only valid inside a method or constructor` |
| RE42 | R30 | `R/value/r-values.js (line RE42)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Private name is not declared in an enclosing class` |
| RE43 | R36 | `R/value/r-values.js (line RE43)` | `1` | `THROWS SyntaxError: Unexpected strict mode reserved word` |
| RE44 | R36 | `R/value/r-values.js (line RE44)` | `2` | `THROWS SyntaxError: Unexpected strict mode reserved word` |
| RE45 | R36 | `R/value/r-values.js (line RE45)` | `1` | `1` |
| RE46 | R36 | `R/value/r-values.js (line RE46)` | `3` | `3` |
| RE47 | R36 | `R/value/r-values.js (line RE47)` | `4` | `THROWS SyntaxError: Unexpected strict mode reserved word` |
| RE48 | R36 | `R/value/r-values.js (line RE48)` | `THROWS SyntaxError: the evaluated source is not a program this profile admits` | `THROWS SyntaxError: Unexpected strict mode reserved word` |
| RT01 | R05 | `R/t262/test/sjl13-R/RT01-script-await-using-in-block.js` (Broiler.VM); `R/run/rt01-script-await-using-in-block.js` (Broiler.JS) | `pass test/sjl13-R/RT01-script-await-using-in-block.js [raw]` | `RT01 accepted` |
| RT02 | R05 | `R/t262/test/sjl13-R/RT02-script-await-expression.js` (Broiler.VM); `R/run/rt02-script-await-expression.js` (Broiler.JS) | `pass test/sjl13-R/RT02-script-await-expression.js [raw]` | `Uncaught SyntaxError: Unexpected token Identifier: Promise at 1, 15 / at Compile in <probes>/R/run/rt02-script-await-expression.js:line 1 / at Compile:<probes>/R/run/rt02-script-await-expression.js:1,1 (exit=1)` |

## The surveys' and critics' items, each reproduced or refuted

Every item of the plan's working papers (`survey-vm`, `survey-js`, `survey-records`, `survey-deps`, their critics and
`plan-critic`) that falls in one of the five sub-areas, with what the code at the heads says of it. An item no row
reproduces is refuted here, with its evidence; none is left open.

### Sub-area T

- **S1** survey-vm 2.1 "Moves" (the lexical grammar as the wide path uses it): **reproduced**. T1-T31 are that grammar;
  T6-T8 and T13 are what moves with corrections.
- **S2** survey-vm 2.1 inference, "the decorator's 2104 … Broiler.VM refuses it in its own admission step":
  **refuted** (as critic-vm W2 says). The mapping makes the library's `decorator.not-in-edition` 2104 within meaning
  (`frontend-mapping.tsv:354`, EX0017 `:93`); what changes is the stage (T9, T22).
- **S3** survey-vm 2.1 "the regex-or-division answer can differ from RegularExpressionIsAllowedHere's on inputs the
  heuristic gets wrong": **reproduced for the wide path** (T24d, T24e, T24f, where the heuristic is wrong; T8, where the
  template scan is) and **refuted for B4 and part F**: the slice manifest admits no regular-expression literal, so no
  B4-accepted input holds one.
- **S4** critic-vm W8 (four construction sites plus the inner one; the "one call site" remark is stale): **reproduced**
  (`VMJC:834`, `:1070`, `VMSS:209`, `SliceConstructCensus.cs:97`, inner `VMT:776`; remark `VMT:390-415`).
- **S5** critic-vm U1 (source fuzzing reaches the wide path's tokenizer): **reproduced** (`SourceFuzzing.cs:420` → `VMSS:209`).
- **S6** critic-vm U2 (lexical refusals in substitutions are deferred): **reproduced** (`VMT:785-789`, `VMJP:6997-7006`); T9.
- **S7** critic-vm W11 (citations): **reproduced** where it touches T; template CR to LF is `VMJP:6776-6781`.
- **S8** critic-records B-9 (`07.5`, `ScanSubstitution` closed): **reproduced** (H1, H2).
- **S9** critic-records B-10 (Broiler.JS lexer hand-offs are SJL-14's): **reproduced** (H4-H8).
- **S10** critic-records B-11 (`SliceTokenizer` duplicates are SJL-15.4's): **reproduced** (H2, H3).
- **S11** critic-records C-8 (the digit lookahead's lines moved): **reproduced** (`JSS:679`, `:703`; H6).
- **S12** records 16-b (Broiler.JS has no compile-time regex early error; an invalid literal becomes division):
  **reproduced** (T24i; baseline T041390-T041392 show the parser's "Unexpected token").
- **S13** plan-critic M2 (part B's S class lacks the tokenizer-stage suppressions): **reproduced**; rule 4.1's class S6 now
  names it. T9's probes are its cases.
- **S14** plan-critic M3 (part F undefined for regexes and templates): **reproduced in part**: rule 4.1's part F now
  defines the template mapping; the slice-accepted population holds no regex, template, BigInt or decorator token, so
  those arms are not exercised; the HTML-like comment switch still differs (T30, F17), and so do the
  identifier token's fields, which **are** exercised: B4-accepted inputs hold escaped identifiers (T32, F17).
- **S15** critic-js C-10 (the HTML-like comment switch is tied to the goal): **reproduced** (`JSS:166`); T4, T30.
- **S16** survey-js S-3.2 (FastScanner): **reproduced** member by member (T18, T24i, T25, T26 cite it); its `:1043-1145`
  for DecodeBracedUnicodeEscapes is `:1048-1145` at `d6795fc8`.
- **S17** U+0085 (records): **reproduced closed** (H4, T1).
- **S18** critic-records D-8 (hand-offs to SJL-13 to SJL-15 in records the records survey did not open): **reproduced**
  for JSD-0071:107-108 (H11, but its owner is SJL-13, not SJL-15), JSD-0065:66 (H12; only `strict-non-octal-decimal`
  survives), JSD-0070:272 (H13), the changelog's `ContainsContinue` (H14) and `native-aot.md:226` G5 (H17);
  **refuted in part** for the changelog: "SJL-14.2's bridge must synthesise both facts" holds for `ContainsContinue`
  only, since the pass sets `IsLoopBookkeeping` itself (H14). The changelog lines it cites are `:254-262` and `:88-96`
  at `d6795fc8` (#1078 moved them).

### Sub-area P

| Item | Where | Verdict | Evidence |
|---|---|---|---|
| C-2 (`FastScopeItem` enumerates a Dictionary; `unsafe bool Expression`; `AllowUnsafeBlocks`) | critic-js | reproduced | `JSP/FastScopeItem.cs:9`, `:237-248` (no Remove); `JSP/FastParser.Expression.cs:101`; `Broiler.JavaScript.Parser.csproj:6` (row P30) |
| 15-b (the Broiler.JS column "Dictionary enumeration order" unverified since `44a214dc`) | survey-records | reproduced for the parser at `d6795fc8` | same sites as C-2; the `LoopScope` and label changes of #1074/#1075 are compiler-side (D's) |
| W11 (79 records and 4 enums; StreamRefused reads) | critic-vm | reproduced | `VMC/JsSyntax.cs` (79 records, 4 enums); StreamRefused has 28 occurrences, 27 reads |
| W2 (decorators refused by the tokenizer before any parse, not by an admission step) | critic-vm | reproduced | `SliceTokenizer.cs:2754-2760` (2104), `VMC/JsCompiler.cs:821-933` returns the tokenizer's diagnostics first (row P19) |
| W5 (eval's private names) | critic-vm | reproduced | VM checks an eval's private names when it runs; JS checks them when it compiles the eval (`JSC/DirectEvalSupport.cs:714`, `SyntaxValidation.ValidateProgram(…, privateNamesInScope)`). R carries it to D44 |
| survey-vm 2.3 (`TrailingComma` read only by the parser and the syntax file) | survey-vm 2.3 | reproduced | reads only in `JsParser.cs` (2) and `JsSyntax.cs` (4) (row P28) |
| survey-vm 2.11 (arrows by a bracket scan, no rewinding) | survey-vm 2.11 | reproduced | `VMC/JsParser.cs:4709-4842` (row P14) |
| survey-vm 2.2 (at most 64 refusals per stage) | survey-vm 2.2 | reproduced | `VMC/JsParser.cs:7460-7470` (MaximumPerStage); D owns it |
| S-3.6 (AstOptionalChain wrapper) | survey-js S-3.6 | reproduced | `JSP/FastParser.SingleMemberExpression.cs:268-269` (row P29) |
| JSD-0080 D23 (JS's Annex B.3.4 synthetic block) | the contract | reproduced | `JSP/FastParser.Statement.cs:82-126` (row P29) |
| catalogue sites EP0009, EP0010, EP0012, EP0014 (VM) | early-errors.tsv | refined (moved lines) | EP0009 `SliceTokenizer.cs:2316` is now `:2754-2760`; EP0010 `JsParser.cs:3364` is now ConsumeFieldTerminator `:3411`; EP0012 and EP0014 `:5566` are now `:5625-5633` |
| completeness.md: JS refuses `using` at a module's top level (`FastParser.Statement.cs:652-653`) | the catalogue's `completeness.md` | reproduced | `JSP/FastParser.Block.cs:32-33` and `JSP/FastParser.Statement.cs:652-654` (row P02) |
| completeness.md: JS refuses a line break after a comma in an import attribute list (`FastParser.Import.cs:279`) | the catalogue's `completeness.md` | refined | the refusal is of a line break before a string (or keyword) key, whatever precedes it; elsewhere line breaks are skipped (row P05c, probe PP03) |
| S-2.2, its non-position half (the parser-computed members the compiler reads) | survey-js | reproduced at `d6795fc8`, and completed | every member of its table re-read (lines moved: `ForLoopPreparation.cs:979-984` after #1076); added: `AstProgram.IsAsync` with its rule, `ContainsContinue`'s over-approximation (nested functions count), the inputs of the Annex B.3.3 rule (`FastParser.Function.cs:166-232`), and the `DirectEvalSupport.cs:756` token read (row P33) |
| S-14 open question 2 (who produces `HoistingScope`, `AnnexBFunctionNames`, `IsSyntheticFunctionStatementBlock` and `AddVariable`'s redeclaration errors once `FastParser` goes; must the dictionary order be kept) | survey-js | settled | the bridge, from SJL-14's W1 (D44): `HoistingScope` from D10's side table, `AnnexBFunctionNames` by the parser's own rule over the side table once it carries scope kinds, parameters and if-clause positions (F13), the synthetic block from the shared tree (JSD-0080 D23); the redeclaration errors are the library's (D10's union, D's rows); the order is P30's and open question 4's |
| catalogue EP0011, EP0013 (`js_admits` "no") | `early-errors.tsv` | reproduced: Broiler.JS parses neither declaration form, so it does not "admit all four" import-phase forms | `JSP/FastParser.Import.cs:51`, `:73`; the test262 positives both engines refuse (row P06b; R09 states it) |
| catalogue EE0037 (status `neither`; VM accepts, JS's compile throws NotImplementedException) | early-errors.tsv | reproduced by reading | `VMC/JsParser.cs:7139`; `JSA/VariableDeclarator.cs:68`; `JSC/Expressions/FastCompiler.VisitAssignmentExpression.cs:779-781`, `:1116` (row P31, probes PP08 to PP10) |

Not P's: critic-vm W7 (a duplicate module key, `JsCompiler.Modules.cs:63`, is the module graph's), M2 (the left-operand
gap of the tree-depth bound, R's), the other critic-vm items (records, roots, fuel, floors); completeness.md's
duplicate regexp group names (T or R) and `L1: L2: while` (D).

### Sub-area N

| Item | Where | Verdict | Evidence |
|---|---|---|---|
| Byte identity depends on parser-computed positions: line and column per node, a call at its member's name, UTF-16 offsets per function and class, cooked and raw text with CR as LF, numeric keys by `NumberText` | survey-vm section 0 item 4 and section 2.3 | reproduced, and refined: a method's SourceStart excludes `static` while its member's span includes it; `NameSpan` is absent for computed and private members; synthetic nodes take the arrow's or the field's span | rows N15, N16, N17, N21 |
| `SliceSourceSpan` carries a line and a column only; a column counts UTF-16 units; the position table writes a row when line or column changes, clamped to 1 | survey-vm 2.3 | reproduced | `VMC/SliceSyntax.cs:28`; `VMC/SliceSourceDiagnostics.cs:265-268`; `VMC/JsCompiler.Emission.cs:114-125` |
| The library must reproduce "the substitution spans" | survey-vm 2.7 | settled: the runs carry real positions at the head; SJL-12's class P was empty | row N7 |
| R-c "Position anchors and offsets reach artifact bytes" | survey-vm 7.5 | reproduced, and widened to diagnostics, which part B compares as `code:line:column` lists | rows N10-N14, N23, N24; findings F10, F9 |
| Step 1's adapter has to supply the per-node anchors | survey-vm 2.11 | reproduced | "What the adapter of D15 must reproduce" above |
| `SpanLocation` 1-based line and column in UTF-16 units, every LineTerminator a new line | survey-js S-2.1 | reproduced, and refined: the end-of-file token has no position (0,0) | `JSA/Misc/SpanLocation.cs:3-17`; row N11 |
| The bridge must synthesise tokens with `Start`/`End` locations | survey-js S-2.2 | refined: the compiler reads token start locations (statement steps, a function's point) and spans; an end location is read only by the parser's ASI test; Broiler.JS gives offsets meaning (template identity, direct eval) | rows N15, N18; finding F27 |
| Lexer refusals inside substitutions are deferred and re-reported in parse order | critic-vm U2 | reproduced for the position half: each keeps its own line and column; the order half is D's | `VMC/SliceTokenizer.cs:785-788`, `VMC/JsParser.cs:6999-7006`; probe NP15 |
| Part B records `code:line:column` only | critic-vm U3 | reproduced; this is why the diagnostic anchors are part B's | `VMS/ParseCorpus.cs` report readings; rows N12-N14 |
| `HostSurfaceChecks.Scripts.cs` reads the first diagnostic's source name, line and column | critic-vm M11 | reproduced | `VMI/HostSurfaceChecks.Scripts.cs:455`, `:555-558`; row N22 |
| "Positions inside substitutions may only become correct (bundle class P)" | JSD-0080 D15 | settled: correct at the head, class P empty at SJL-12's after commit | row N7 |
| "Some lowering refusals carry a zero position" | `areas/frontend.md:127` | reproduced, and the binder has two such sites too | row N14 |
| "Offsets, lines and columns in UTF-16 code units, 1-based" | PLAN D15 | refuted in part: offsets are 0-based in both engines | row N1; finding F11 |
| R6 "an anchor or offset the adapter cannot reproduce" | PLAN section 7 | supported, and extended: a diagnostic anchor the library does not reproduce is also NOT MET under part B | rows N10-N14, N24 |

### Sub-area D

| Item | Where | Verdict | Evidence |
|---|---|---|---|
| B2 (split codes `JsParser` emits; D18 "as today" false) | plan-critic | **reproduced and completed**: EE0180, the four `new.target` rows and EX0028 (two sites) are the relation-split rows `JP` emits, each at a site serving it alone; the other-code splits at `JP` are EE0087, EE0155, EE0168, EX0008 (2102 and 2209) and EX0023; **added**: EX0008's 2102 at `:4268` and 2209 at `:2384` and `:6610` serve EX0008 alone, so the registry commit could change them too; `TryParseArrow` `:4748` is a further 2209 site | D-b |
| B8 (binder checks stranded by D10) | plan-critic | **reproduced and completed**: the list adds EE0005 (B26, live), EX0009 (B12), EE0027 (B11), EE0028/EE0029 (B07), EE0030/EE0031 (B06), EE0168/EE0169 (B27-B31, B34), the dead copies (B05, B14, B17-B20), and the questions EE0017, EE0018, EE0054; **refuted in part**: EX0021 is not a binder check (lowering `JsCompiler.Modules.cs:196-229`), and EE0182 has no binder check of its own (B33) | D-d |
| B4 (stage pairs) | plan-critic | **reproduced**: CAT gives (parser, SyntaxValidation) 33, (parser, FastCompiler.EarlyErrors) 11, (parser, Visit*) 5, (parser+lowering, SyntaxValidation / Visit* / DirectEvalSupport) 3 + 3 + 2 | CAT columns 14 and 17 |
| W1 (2104's future decided by `MAP`) | critic-vm | **reproduced**: `MAP:387` EX0024, `:391` EX0028, `:393` EX0030 are split from 2104; `CF/Test262Run.cs:437-445` grades a first 2104 as `Unsupported`; no B1 input reaches EX0028 (F6) | `MAP`; VB |
| S2 (the mapping's sites predate SJL-12.6) | critic-vm | **reproduced**: `JsCompiler.Exceptions.cs:259`, `:270` are `BinderAdmitted` assertions now; the manifest helper is `JP:7403-7407` | D-b, D-d |
| U2 (stage order incomplete) | critic-vm | **reproduced and refined**: five stages, each stopping the unit; the tokenizer stops at its first; the parser at its first plus the statement's cascade | D02, D04 |
| U3 (message readers) | critic-vm | **reproduced and completed**: also `SC/SourceFrontEndChecks.cs:125-137` and `SC/BigIntChecks.cs:589`, `:607` | D06 |
| M11 (diagnostic consumers) | critic-vm | **reproduced** | D08 |
| OQ2 (stage and order of the numeric manifest's 2104) | critic-vm | **answered**: `JP:5819`, during the parse, in parse order, stopping the parse | D18-1 |
| new question 1 (non-language splits' verdict classes, floors) | critic-vm | **answered in part**: only a first 2104 moves a verdict (D07); EX0028, EX0024, EX0030 graded as 2104 under D12 keep `Unsupported`; no corpus input reaches EX0028 | D07, F6 |
| B-4 / open question 6 (15-c; does the binder still refuse the label set?) | critic-records | **reproduced**: yes, 2208; the VM pins it in a checks row | D30 |
| D-2 / open question 4 (D13's test262 consequence) | critic-records | **reproduced**: `Test262Run` grades on the first code, 2104 alone being `Unsupported` | D07 |
| M2 (catalogue drift on Broiler.VM's side) | critic-deps | **reproduced**: no CAT `vm_wide_site` names `JsBinder`; e.g. EE0200 cites `JsCompiler.Literals.cs:447` and the check is `JBE:469-520` | D-d |
| B-2 (#1075, not #1074, changed `SyntaxValidation.cs`) | critic-js | **reproduced**: `git diff c29d653f d6795fc8 -- …/SyntaxValidation.cs` is #1075's hunk at `:567-578` alone | D30 |
| C-7 (the compiler's direct `SyntaxError`s) | critic-js | **reproduced**: five sites | D01 |
| S-4.2 (86 `FastParseException` sites; the shell's retry) | survey-js | **reproduced**: 86 (52, 6, 6, 3, 3, 2, 2, 2, 1 × 8); `Broiler.JavaScript/Program.cs:260` | D01, D06 |
| 2.2 counts and the cap | survey-vm | **reproduced**: 2101 46, 2209 21, 2102 13, 2201 12, 2205 10, 2103 4, 2202 3, 2210 2, and one each of 2005, 2401, 2404, 2402, 2104, 2203; `MaximumPerStage` 64 (`JP:7458-7470`) | D03 |
| 2.6 (the binder's 2104 sites; `CheckRegExp`) | survey-vm | **reproduced**: `JBW:825-828`, `:1199-1202`, `:1293-1296`; `JBE:469-520` | B05, B14, B17, B25 |
| 2.1 item 5 / 2.2 "unassigned" parse-time checks | survey-vm | **settled by D10** (the library's `syntax` half); D-d lists what the binder keeps | D-d |
| W2 (the decorator) | critic-vm | settled by T (S2); D adds that a substitution's `@` is re-reported by `JP:7003` | D18-7 |
| W7 (where the duplicate module key is refused) | critic-vm | **reproduced**. At the head `JsCompiler.cs:965-995` holds only 2405 (`:975`) and 2406/2103 (`:988-989`). The duplicate key is refused by `CompileModule` `JsCompiler.Modules.cs:52-66`: `Refuse` at `:60`, code 2201 at `:62`, message at `:63`, at `program.Span`, for a unit whose key an earlier unit of the same `JsCompiler.Compile` request (`JC:774`, `:810`, `:823`; one `CompileModule` per unit, `JC:1015`) already used. The line is the same at `d82b07a`, `f2ce6e9` and `4cf9ca4`. `MAP:383` (EX0020, JSE9304) splits it from 2201, and the registry commit changes the constant at `:62`, as D-b's adjacent table plans. **Reach:** no root presents two units under one key: `Cli/ModuleGraph.cs:213` and `PolyglotCli/ModuleGraph.cs:202` key one file once. No checks row, test or corpus input reaches it either (grep of `src/compositions` and `src/tests` for the message). The refusal exists only on the embedder API. Broiler.JS has no counterpart: each compile takes one unit, and modules are loaded by the runtime. Class: engine-kept (the request and the module graph, P34's); a constant change at step 1 | `JsCompiler.Modules.cs:52-66`; `MAP:383`; `JC:774-1015` |
| M1 (the test262 floors) | critic-vm | **reproduced, with two further floors and a check**. (1) The retained suite: `.github/workflows/broiler-vm-lane.yml:268-283` runs `--floor src/Broiler.VM.Profile.JavaScript/docs/conformance/floor-test262.txt`, whose script and module rows require every case to pass, scored through `LanguageErrors.MayScore` (`CF/LanguageErrors.cs:241-242`; `Execution.cs:342`, `:375`). (2) The whole suite: `src/tests/conformance/floors/test262-wide.floor` (bytecode) holds an `atLeast passed` row, an `atMost failed` row and **`atMost unsupported 0`**; `test262-wide-native.floor` still holds a non-zero `atMost unsupported` row, the BigInt row the bytecode floor retired on 2026-09-21. `eng/run-test262.py` exits 4 on a crossed floor (`:80`, `:463-486`), and the hosted workflow holds each form to its floor (`test262-evidence.yml:6`, `:15`). The bytecode floor's header records its hand re-base of 2026-09-08 (JSC-207). (3) Two further floors, over Broiler's own suites: `floor.txt` (the lane's ratchet, `broiler-vm-lane.yml:728-733`) and `floor-ingest-shape.txt` (`:746-750`). (4) **A check the floors depend on:** `IngestionChecks.TheLanguageClassMapIsTotalOverTheVocabulary` (`CF/IngestionChecks.cs:56-68`) fails while any `SliceSourceDiagnosticCode` member has no row in `LanguageErrors.Classify` (`CF/LanguageErrors.cs:97-178`) and `Classified` (`:203-235`); an unclassified code cannot score (`:241-242`), which lowers floor (1) at once. Only a first 2104 moves a test262 verdict (D07), and no bytecode variant is `Unsupported` today, so revision 19's moves off 2104 (EX0024, EX0028, EX0030) cannot regrade a bytecode variant; any new first 2104 at step 1 crosses the bytecode floor's `atMost unsupported 0` (the admission walk, D18, or the decorator's stage could put one first). Decision D13 and the registry commit's grading and classification of the new codes cover (1), (2) and (4); the native floor's stale row would hide such a move in the native form only (F20) | `CF/LanguageErrors.cs`, `CF/IngestionChecks.cs`, the five floor files, `eng/run-test262.py` |
| new question 6 (is the mapping re-read against the head before revision 19?) | critic-vm, with its S2 | **answered: not by the mapping; now by these rows**. `MAP:16` still says `sites-read-at: d82b07a`. Of the sites its split rows cite, three are unchanged and ten `JsParser` sites hold the same refusal on moved lines. At the other eleven the lowering site is a `BinderAdmitted` assertion and the refusal is the binder's (one is split with a ceiling consequence). D-h gives each at the head, D-b's adjacent table the relation-split rows that cite no site, and D-d every binder check. Part E's checker reads cells, not sites, so it cannot find this (F4) | D-h; `analysis/mapsites.tsv` |

### Sub-area R

| Item | Where | Verdict | Evidence |
|---|---|---|---|
| `SliceParseOptions`' fields; constants 64, 512, 10,000; only `Strict` and `InFunction` reach the parser | survey-vm 2.4 | reproduced | `VMC/SliceParseOptions.cs:71`, `:95`, `:136`, `:149-171`; `VMC/JsCompiler.cs:853-865` (R01, R12, R13) |
| Who reads the other four eval flags | critic-vm W6 | reproduced | `VMC/JsBinder.EarlyErrors.cs:66`, `:210`; `VMC/JsBinder.Walks.cs:440`, `:445`, `:450`; `VMC/JsCompiler.Eval.cs:307`, `:355-358`; `VMC/JsCompiler.Classes.cs:347` (R22-R27) |
| "No Annex B switch and no proposal flags; HTML-like comments a tokenizer property set from the goal" | survey-vm 2.4 | reproduced | R07, R08, R09 |
| `CompilationStack`'s callers | survey-vm 2.4 | reproduced | `VMC/JsCompiler.cs:598`, `:712`, `:775`, `:812`; `VMC/SliceSourceCompiler.cs:194` (R15) |
| `JsCompilerUnicode` is the provider Broiler.VM would hand the library | survey-vm 2.9 | reproduced | R17 |
| `CheckRegExp` is the milestone's validation seam | survey-vm 2.6 | reproduced | `VMC/JsBinder.EarlyErrors.cs:469-519`; JSD-0081 D18 (R18) |
| Step 1's adapter must supply `SawTopLevelAwait` | survey-vm 2.11 | reproduced; not in the plan's D15 | F12 |
| The fuel charge sites; the compiler unmetered | survey-vm 7.1, critic-vm W9, W10 | reproduced with W10's labels | `VMP/JsEngine.Eval.cs:100-102`, `:339-340`, `:420-421` (R31) |
| Metering and cancellation (survey-vm open question 5) | survey-vm OQ5, critic-vm M7 | settled by SJL-D-10's shape and D32 | R31 |
| The tree-depth constant and N19's text check | survey-vm OQ7 | settled by D14 (Broiler.VM keeps a constant equal to the library's) | R13 |
| The left-operand gap, carried to SJL-13 | critic-vm M2; survey-records 6; plan-critic B1 | reproduced and widened | R14, F18, probes RP01-RP05 |
| Eval's private names are checked by the executor at run time | critic-vm W5 | reproduced; outcome the same in both engines | R27, probes RE30-RE34 |
| No Broiler.Regex package edge before SJL-11.4 | critic-vm M5; survey-deps Q3; critic-deps 7 | settled by D7 (a provider of the request) | R18 |
| Broiler.JS's options lack limits, a cap, Annex B, forced strictness, inherited context, and the HTML-like switch follows the goal | survey-js S-3.1; critic-js C-10 | reproduced | R01, R03, R07, R12 |
| `RegExpValidator`'s only caller is the scanner, `System.Text.RegularExpressions` behind it | survey-js S-3.5 | reproduced | `JSP/FastScanner.cs:1505`; `JSP/RegExpValidator.cs:18` (R18) |
| Broiler.JS has no nesting bound; `StackSegment` survives depth | survey-js S-3.7 | reproduced | R12, R15 |
| D27's enumeration needs no per-input depth; the baseline's stack is 256 MiB | critic-js C-4; survey-js OQ10 | reproduced | JB's `#%` header (R15) |
| The inherited context belongs in the request at SJL-14 | plan-critic B5 | reproduced; refined | R22-R29; F21, F22 |
| The Unicode provider's type | survey-records 4 | settled: Broiler.Unicode's `IUnicodeTables` in both | R17 |
| Public surface: no shared type in Broiler.VM's API | survey-vm OQ8 | settled by D16 | R01 |
| Order against SJL-11.4 and SJL-11.5 | survey-vm OQ9; survey-js OQ4 | settled by D36 and D51 (no code evidence in R changes them) | R18 |
| Catalogue EX0023's note, EX0011/EX0012 sites, the regular-expression rows' Broiler.VM site | early-errors.tsv | refuted as current (stale since SJL-12.5 and SJL-12.6) | F4 |
| Parse sites in product code: `FastCompiler`, `DirectEvalSupport`'s parse and its reserved-word re-scan, `SyntaxValidation.ContainsLegacyOctalLiteral`, `ModuleSourceAnalysis`, `PortableCompiler` | survey-js S-4.1 | Reproduced: the same six construction sites at `d6795fc8` (`FastCompiler.cs:164-166`, `DirectEvalSupport.cs:709-712` and `:746`, `SyntaxValidation.cs:375`, `PortableCompiler.cs:38-40`, `FastParser.cs:186` via `ModuleSourceAnalysis.cs:50`). Completed with the options each passes and the eleven routes into `FastCompiler` | R35's inventory (J1-J6, J1a-J1k) |
| Do the `FastTokenStream` re-scans move onto the shared tree at SJL-14, or does `FastTokenStream` outlive SJL-14.5? | survey-js OQ3 | Settled by the plan: D41 and plan section 2.2 task 4.3 have them walk the bridge's token chain. This file adds that the reserved-word re-scan over-refuses (R36), so the recommendation is to delete both once the request carries forced strictness | F25 |

## Where the runs corrected the reading

Every probe's answer was compared with the answer its row read from the code (`probes/plan.tsv`, column `expected`).
Where they differed, the code was read again and the row now states what the run showed; the run is the evidence and
the reading was the hypothesis. Every other answer, in both engines, is the one the reading expected, the open
questions the readings left to the run among them (section "The reading's open questions, and where each went"). *(Added
2026-10-27: the probes of that day were compared with `probes/plan-p16b.tsv` the same way, and every answer is the one
it expected. The two rows marked so below are where SJL-13.2's runs of the `syntax` vectors, not a probe of this file,
corrected rows P16, T24b and T24f; the owner's decisions D-asi and D-t24 of that day answer them.)*

| Probe | Row | The reading expected | The run showed | The code, read again | What it changes |
|---|---|---|---|---|---|
| T13A, T13B, T13D, T13F (Broiler.VM, B4) | T13 | accepted on the slice path too | refused 2104:1:9 | the slice tokenizer accepts the escapes, but the slice parser makes a string literal in an expression a `StringValue` construct (`SliceParser.cs:1443-1448`) and the slice manifest refuses it (`SliceSourcePrograms.cs:273`); only the identifier form T13C is accepted | T13's class stands (the wide path's correction); the slice path's tokenizer defect reaches part F only through an identifier (F17) |
| T32C (Broiler.VM, B4) | T32 | accepted | refused 2104:1:9 | as T13: `var s = 'it\'s';` holds a string expression; J001493 (`'it\'s'` alone) is a directive, which B4 accepts | none: the string-escape inputs F17 names are all directives |
| T20A (Broiler.VM, B1) | T20 | 2102 at 1:11 | 2101:1:11, then 2102:1:12 | `JsParser.AfterDot` refuses a second `.` ("a property name after `.` is an identifier name", `VMJP:6285-6298`) before `Expect` does | none (same) |
| C01, C02, RD01 (Broiler.VM, rendering) | D18-1, D30 | `… at script0:1:9: …` | `… at 1:9: …`, the same code, place and message | the command-line and conformance roots name no unit in the place | none |
| T16E, T16F (Broiler.JS, K) | T16 | accepted at P and K | P accepted, K refused "Unexpected legacy octal literal in strict mode" | a strict function's body is checked for legacy octal literals, object keys included, by `ValidateFunctionEarlyErrors` (`FastCompiler.EarlyErrors.cs:13-31`, `:99-126`), called from `CreateFunction.cs:251` | T16's Broiler.JS half is K1 and a class body's strictness; a function's own strictness is stage E (F25) |
| T24B, T24B2, T24F (Broiler.JS, P) | T24b, T24f | refused at P | P accepted, K refused "Undefined binary operation RegExLiteral" | the parser takes the regular-expression token as a binary operator, because `IsLiteralValueStart` leaves it out (`FastParser.Expression.cs:33-40`, `:266-296`); the compiler refuses the node (`VisitBinaryExpression.cs:163`) | the classes stand; the refusal is stage E (F28) |
| TV-T18a (Broiler.JS) | T18 | `"1.5"` | `"1.5n"` | `JSDecimal.ToString` appends `n` (`Broiler.JavaScript.BuiltIns/Decimal/JSDecimal.cs:76`) | none (engine policy) |
| T32A, T32B, T32C (Broiler.JS tree dump) | T32 | the dump prints each identifier's token spelling | it prints the node's offsets and cooked `Name` | SJL-12's tool dumps the tree's nodes, not their tokens | none: the spelling is read from the token dump, and every token field matched |
| NR01 (Broiler.JS) | N15 | frames for `f`, `m` and the script, at their statement starts | frames for `f` (2,3) and the script (5,1) only, the script's named `native` | the shell's `--script-host` turns proper tail calls on (`Broiler.JavaScript.Runtime/DictionaryCodeCache.cs:156`; `Broiler.JavaScript.ExpressionCompiler/Generator/ILCodeGenerator.VisitCall.cs:44-48`), so `return f();` leaves `m`'s frame; a frame with no name prints as `native` (`Broiler.JavaScript.BuiltIns/Error/JSError.cs:31-50`) | none (engine-kept rendering); the positions matched |
| PD14 (Broiler.JS) | D40 | refused (`as` expected) | accepted at P and K | only a string and the five keyword-typed tokens (`true`, `false`, `in`, `instanceof`, `null`) must be renamed with `as`; every other reserved word reaches `Identitifer` as an `Identifier` token, and `RejectReservedImportedBinding` (`FastParser.Import.cs:210-219`) refuses only `await` and the strict-mode restricted names | D40 becomes a correction of both (F19, F28) |
| RE19 (Broiler.JS) | R24 | `THROWS SyntaxError` | `THROWS TypeError: Value is not a constructor` | `eval('super()')` in a base-class constructor is admitted at the parse, because `MemberInits` is set for every class constructor (`CreateClass.cs:705`; `FastFunctionScope.cs:564`), and fails when it runs | R24 becomes a correction of Broiler.JS (F23) |
| `syntax/lexical/asi/0009` (Broiler.JS's engine-level run of corpus revision 30, SJL-13.2; added 2026-10-27) | P16 | as the specification (read from test262's `test/language/asi` tests, none of which puts a declaration before the line break) | accepted: `for (var i = 0` LF `i < 1; i++) ;` | `Parameters` ends a declaration head's declarator list at the line break without reading the head's first `;`, and `ForStatement` reads the test without it (P16b's sites; probes PP11 to PP19, values PE01 to PE09) | P16 narrowed to automatic semicolon insertion outside a for statement's head; P16b added, a correction of Broiler.JS that SJL-14 answers (decision D-asi) |
| `syntax/lexical/regexp-delimiting/0046`, `0047` and `0052` (the same run; added 2026-10-27) | T24b, T24f | refused at compilation, "Undefined binary operation RegExLiteral" (the rows' probes put an operand after the literal) | the program completes with the left operand's value: `;` or the end of input follows the literal | `NextExpression` takes the regular-expression token as an operator and, with no right operand, ends the expression with the left operand alone (`FastParser.NextExpression.cs:218-234`, `FastParser.Expression.cs:301-304`) | the classes stand; T24b and T24f record the answer beside the one they held (decision D-t24; probes T24B3 to T24F3, values TV-T24b3 to TV-T24f3) |

## The reading's open questions, and where each went

No question is left open by this file: each was settled by a run, or is a choice the plan's decisions or task 1.2
make, named in a finding.

| Question | Settled by, or sent to |
|---|---|
| T16B: is a legacy octal method name in a class body an early error? (node 22 accepts it) | the specification: every part of a class is strict code, so it is a SyntaxError; both engines accept it, T16 is a correction of both |
| T24G2: does Broiler.VM's parser admit `var yield` inside an arrow inside a generator? | the run: B1 accepted, as the tokenizer's division reading needs |
| T11: does .NET's hexadecimal parse take twelve digits with leading zeros? | the run: TV-T11a and TV-T11b answer as the specification |
| T32B in B4 | the run: accepted |
| T32 and part F's field map | F17 (a choice for decision D21 and rule 4.1's part F) |
| P: the conformance root's flags for a run probe; the JS tool on a manifest with no test262 row | the runs (every run probe and parse probe ran as written) |
| P26: does SJL-13 deliver the FormalParameters and FunctionBody goals to Broiler.VM's dynamic function? | no (P26; hand-off H18, F32) |
| P30: may Broiler.JS's bridge order hoisted names by source position? | F13 (the side table's order and content, for decision D10 and stage 4d) |
| P08, P15b, P24: the stage of a rule each engine checks at a later stage | F29 (task 1.2) |
| P05b: which request parameter carries the honoured import attributes? | none: EX0021 is an `engine` row (P05b) |
| PR06: is Broiler.VM's top-level-await reading of an arrow observable? | the run: yes, `a,tick,b` against `a,b,tick` (F12) |
| P: the side table's content | F13 |
| NP14: where does Broiler.JS stop in a substitution? | the run: at the `)`, 2, 3 |
| N: the library's diagnostic anchors, or a position class | F10 |
| N: the recovery tail | F9 |
| N: EA0005's stage and its 0:0 place | F29 (task 1.2) |
| N: does the home rule's corpus runner drop a leading byte-order mark? | no corpus input begins with one (`analysis/scan-inputs.txt`); the request takes text, and decoding is the host's (N5) |
| NR01: Broiler.JS's `stack` rendering | the run (N15) |
| NR02: do Broiler.JS's eval messages carry a tail? | the run: no |
| N25: is a `stack` frame inside a strict caller's eval one line lower in Broiler.JS? | read from the code, not run; N25's class does not rest on it |
| D: what is a "meaning" for revision 19? | F3 (decision D12) |
| D: all errors or the first refused statement for Broiler.VM's request | F9 (decision D11) |
| D: the stage of EE0017, EE0018, EE0054 and EE0179 | F15 (decisions D10 and D20; task 1.2) |
| D: EX0023's emitter | F7 |
| D: EX0029's owner after step 1 | F6 |
| D: does the library report EE0087 and EE0155 for an uncovered arrow head? | F1 |
| D: Broiler.JS's texts for PD08, PD09, PD13 to PD16, and PD21, PD24, PD29, PD30 | the runs (PD14 against the reading: D40) |
| D30: does Broiler.VM's binder still refuse a label set's outer `continue`? | the run: yes (D-e, F30) |
| R-Q1: does the top-level-`await` switch apply to the Script goal? | F8 |
| R-Q2: `eval("arguments")` in a static block | the run: both engines answer `"object"`, the specification's reading; the stage table's reading is F22's |
| R-Q3: the caller's lexical names in the request | F21 |
| R-Q4: does a native compile of a tree deeper than the bound lose the process? | the run: the base compiles RP01 to RP05 in both forms, and no process was lost (F18) |
| R-Q5: Broiler.JS's tiering recompile's goal | F26 and hand-off H24 |
| The PD11 slice front end (read, not predicted) | the run: B4 reports every `break` of the 70, with no cap (D03) |

## The hand-offs: items earlier records labelled SJL-13, and items this file hands on

H1 to H17 are the items earlier bundles and records labelled SJL-13 (PLAN section 1.2 and the records survey's critic,
D-8), each reassigned with its source; H18 to H24 are what this correspondence hands to later work.

| Hand-off | Source | What | Where it goes | Why |
|---|---|---|---|---|
| H1 | Broiler.VM `docs/evidence/sjl-1-001/README.md:55` | `07.5` read as 7.5 | closed | settled at SJL-3.4 (`src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md:94-97`); both engines refuse it through the library (T17) |
| H2 | Broiler.VM `docs/evidence/sjl-3-001/README.md:269` | the front end's escape and template scans: `SliceTokenizer` (3), `JsParser.ScanSubstitution` (1) | `ScanSubstitution`: closed (removed at SJL-12.6). The three `SliceTokenizer` reads are the `HexNumber` parses `VMT:1180`, `:1196`, `:1723`, which carry T13's defect: the wide path stops using them at step 1; the slice path keeps them until SJL-15.4 | B-11; the milestone keeps the slice tokenizer until SJL-15 (`docs/javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md:69-71`) |
| H3 | Broiler.VM `docs/evidence/sjl-5-001/README.md:248`, `:258-261` | the tokenizer's code-point read (`CodePointAt` `VMT:2984`), its escapes, the lexical `WhiteSpace` predicate | WhiteSpace: closed (the owner's since SJL-7.4, T1). CodePointAt and the escape readers: off the wide path at step 1; SJL-15.4 for the slice path | as H2 |
| H4 | Broiler.JS `docs/evidence/sjl-3-001/README.md:211-215` | U+0085 as white space in the lexer | closed | settled at SJL-7.5 (`JSS:429-431`, T1); the SharedLibraries note of 2026-10-16 already says so |
| H5 | Broiler.JS `docs/evidence/sjl-5-001/README.md:317` | the lexer's code-point reads and its identifier and escape tests (`FastScanner` 10, `CharExtensions` 3) | SJL-14 | B-10: SJL-13 does not change Broiler.JS's front end. T12 (`_` in IsDigitPart and HexValue) is one of these tests |
| H6 | Broiler.JS `docs/evidence/sjl-7-001/README.md:176` | the decimal-digit lookahead after `?.` and `.` | SJL-14 | the member is `_ReadToken`, now `JSS:679` and `:703` (the bundle's 627 and 651 are stale, C-8); T19 |
| H7 | Broiler.JS `docs/evidence/sjl-7-001/README.md:576` | eight lines of the scanner and `CharExtensions.IsLineTerminator` | SJL-14 (the lexer); `JSUnicode.IsLineTerminator` is the owner's | T2 |
| H8 | Broiler.JS `docs/roadmap/SharedLibraries.md:335` | the tagged-template path's trimming of a raw segment | SJL-14 | T28 (K3) |
| H9 | Broiler.JS `docs/roadmap/SharedLibraries.md:1381-1390`, JSD-0081 D22 | DecodeBracedUnicodeEscapes and the parenthesis patch; the division fallback | SJL-11.5 (else SJL-14) for the first two; SJL-14 for the fallback (S9) | T26, T25, T24i |
| H10 | Broiler.VM `docs/javascript-shared-libraries/areas/numbers.md:173`, `milestones/sjl-3-number-conversion-pilot.md:286` | literal scanning, diagnostics and strict-mode checks stay in each front end until SJL-13 and SJL-14 | consistent: Broiler.VM's move with step 1, Broiler.JS's at SJL-14 | T16's corrections land with those moves |
| H11 | Broiler.VM `src/Broiler.VM.Profile.JavaScript/docs/decisions/0071-the-profile-on-the-shared-text-algorithms.md:107-108` | "`JsParser.IsWellFormed`, the compiler's early error on a lone surrogate in an export name, which the correspondence assigns to SJL-15" | **SJL-13**, not SJL-15: the library's check of EE0178 (`JSE1619`, mapped to 2101 within meaning, `frontend-mapping.tsv:290`), a `syntax` row by D10 because both parsers refuse it at parse time (`VMJP:1402-1417` through `ModuleExportName` `:1439`; Broiler.JS `FastParser.Import.cs:186-200`, called at `:126`, `FastParser.Export.cs:105`, `:244`). Broiler.VM's site goes with `JsParser` at step 1 (D20, X6, I5); Broiler.JS's at SJL-14's W2. Its row is D56. The second caller, `ModuleSpecifier` `VMJP:1468-1495` (`:1477-1483`, the export-name text copied), refuses EX0029, an `engine` row: its owner after step 1 is F6's | baseline T041637, T041712, T041720, T041722, T041724 (B1 2101, P and K refused: agree); JSD-0071's sentence is stale from step 1 (F31) |
| H12 | Broiler.VM `…/decisions/0065-number-conversion-moves-to-the-shared-algorithms-library.md:66` | literal *scanning* divergences left "until SJL-13 and SJL-14": Broiler.JS's `strict-non-octal-decimal`, `legacy-octal-fraction`, `empty-exponent`, `bigint-fraction`; Broiler.VM's `vm-legacy-octal-fraction` | **only `strict-non-octal-decimal` survives**, and it is **SJL-14's** (K1; T16): Broiler.JS `scripts/conformance/number-divergences.txt:20-25` at `d6795fc8` (vectors `numbers/literal/0082`, `0084`, `0088`, `0090`). The other three of Broiler.JS were retired at corpus revision 5 (Broiler.JS `docs/evidence/sjl-3-001/engine/retired-divergences.tsv`), Broiler.VM's at revision 6 (Broiler.JS `docs/evidence/sjl-4-001/engine/retired-divergences.tsv`, three rows): **closed** | T16, T17, H1; JSD-0065:66 is a stale statement: its dated note is already planned at SJL-14.1 (PLAN section 3.2, Broiler.VM item 3) |
| H13 | Broiler.VM `…/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md:272` | "Sharing the lexers' and the regular expressions' white space: SJL-13, SJL-14 and SJL-11 own them" | **T's, and the sentence holds.** The lexers already read one predicate, Broiler.Unicode's `WhiteSpace` (SJL-7.4 and SJL-7.5: `JsCompilerUnicode.cs:48-53` behind `VMT:2916`; `JSS:204-207`, T1, H3, H4). What moves is the code that reads it: Broiler.VM's wide path at step 1 (the library's tokenizer, through the request's `IUnicodeTables`, D8); the slice path's `SliceTokenizer.IsWhiteSpace` at SJL-15.4 (H3, D60); Broiler.JS's `FastScanner.IsWhiteSpace` at SJL-14's W2. The regular expressions' white space (`\s`) is SJL-11's, outside this correspondence | T1; no record stale |
| H14 | Broiler.JS `Broiler.JS/CHANGELOG.md:158-166` at `4ae1c747` = `:254-262` at `d6795fc8` (`ContainsContinue`, internal; `Ast` grants its internals to `Compiler`), and `:88-96` (#1076, `IsLoopBookkeeping`) | "which raw facts a shared tree publishes is SJL-14's to decide"; D-8 reads it as "SJL-14.2's bridge must synthesise both facts" | **SJL-14's bridge (W1) synthesises `ContainsContinue` only** (D44 lists it): the parser sets it (`FastParser.ForStatement.cs:256-309`, counting at `FastParser.Statement.cs:561-562`) and the for-loop pass reads it (`ForLoopPreparation.cs:263-284`). **`IsLoopBookkeeping` needs no bridge**: the pass sets it (`ForLoopPreparation.cs:860-953`) and `FastCompiler.cs:465` reads it (D44: "Not `IsLoopBookkeeping`"; PLAN 1.4 A-1). The internals grant (`Broiler.JavaScript.Ast.csproj:37`) stays until the compiler stops reading `Ast` (SJL-15.5, D41). A tree fact, not a token: P33 item 4 | D-8 reproduced for `ContainsContinue`, refuted for `IsLoopBookkeeping` (S18) |
| H15 | Broiler.JS `Broiler.ECMAScript/docs/frontend/early-errors.md:19` and `:283` | the catalogue names "the message identifier the shared front end (SJL-13) emits"; "SJL-13 defines each identifier's arguments" | **SJL-13.2 stage 4d** ("message identifiers with arguments (`early-errors.md`'s section 6)", PLAN WP4; D11). **D's** sub-area | open until stage 4d |
| H16 | Broiler.JS `Broiler.ECMAScript/docs/frontend/design.md:216-220` ("What this document does not decide") | the library's names, types and API (SJL-13); the registry revision (SJL-13); the `Ast` and `Parser` disposition (SJL-14.4); the model (SJL-15); "the nesting bound's value, which is SJL-13's measurement" | names and API: SJL-13.1 task 1.7 (design.md revised, D6 to D17) and stage 4a; the registry revision: step 1's registry commit (D12); the disposition: SJL-14.1's `dispositions.md` and SJL-14.4 (D50); the model: SJL-15.2; the bound: stage 4e's tree-depth derivation, the nesting depth staying each engine's parameter (D14; `design.md:74` reads the same). **R's** for the limits, **D's** for the registry | consistent with the PLAN; no record stale until each lands |
| H17 | Broiler.VM `docs/javascript-shared-libraries/native-aot.md:226` (G5) | Broiler.JS's Native AOT sample "evidence at SJL-14 but in no clause" | **SJL-14's**, already a clause: `milestones/sjl-14-broiler-js-syntax.md:107-110` (clause 4, revised 2026-10-11); PLAN D53 and rule 4.4 part E. Not SJL-13's | closed as a record |
| H18 | this correspondence (P26, D61; probes PR01, PR02) | Broiler.VM's dynamic function parses its parameters and its body under the library's FormalParameters and FunctionBody goals before it compiles the assembled function (`VMP/JsRealm.Dynamic.cs:159-187`) | a later Broiler.VM change with its own row and window; recommended: SJL-15's Broiler.VM window, which re-pins the frontend | no change of SJL-13 routes `FromSource` through the library's goals (F32) |
| H19 | this correspondence (P32 item 10; probe PR06) | Broiler.VM counts an `await` inside an async arrow at a module's top level as a top-level `await`, which changes evaluation order | SJL-15's model (stage e, module records and top-level `await`); the adapter keeps Broiler.VM's rule at both steps of SJL-13 | part B admits no byte difference of an accepted input at step 1 or step 2 (F12) |
| H20 | this correspondence (D30; probes PD01, RD01) | Broiler.VM's binder refuses `continue` naming an outer label of a loop's label set (`JBW:653-666`, `:1455-1477`), and `SC/BinderChecks.cs:127-130` pins it | a Broiler.VM correction of the binder's `Target`, with the checks row, outside SJL-13's library (the rows are `semantics`); or SJL-15's jump-target fact | the catalogue's "both" for EE0082, EE0137, EE0166 and EE0176 is stale (F30) |
| H21 | this correspondence (R19, R20) | the two engines' regular-expression validators decide some patterns wrongly | Broiler.VM's at SJL-11.4 (Broiler.Regex's validate entry behind the provider, JSD-0081 D18); Broiler.JS's at SJL-11.5, or K9 at SJL-14 (D45) | consistent with JSD-0081; the library only calls the provider (D7) |
| H22 | this correspondence (the column "Correction of Broiler.JS" in every sub-area) | every correction of Broiler.JS this file classes | SJL-14's `corrections.md` (D45's K1 to K10), with the specific actions F23 to F28 name: the derived-constructor flag (R24), the direct-eval prefix and its skip sites (N25, N26), the two token re-scans (R36, T16), each entry point's request (R35), the bridged token facts (N18), and the tests that assert acceptance past the limits (R12, R13) | SJL-13 changes no Broiler.JS front end; the library corrects these when Broiler.JS parses with it (F28) |
| H23 | this correspondence (T13, T12's .NET hexadecimal parses on the slice path; H2, H3) | the slice path's tokenizer defects (white space and `_` inside an escape's hexadecimal digits) | SJL-15.4, which removes `SliceTokenizer` (D60) | the slice path keeps `SliceTokenizer` until SJL-15 (F17 on part F) |
| H24 | this correspondence (R35's J1k) | Broiler.JS's opt-in tiering recompile parses a promoted function's text under the Script goal whatever goal the function was compiled under, and gets strictness by a `'use strict';` prefix | SJL-14's W1 (the bridge passes the function's goal and forced strictness at that site), or Broiler.JS's own backlog | behind a switch that is off by default; no probe reaches it (F26) |

## What the rows find against the plan and the records

The plan is SJL-13 to SJL-16's plan of 2026-10-26, with its decisions D1 to D74 and its rules 4.1 (`sjl-13-001`'s
area rule), 4.2 (`sjl-13-002`'s) and 4.3 (the home's rule). "A record of section 3.1" is one of the records SJL-13.1
files (the verdict, its X6 and hand-off table, the bundles, the dated notes, the ledger, Broiler.JS's records). Each
finding names its rows, says what in the plan or the records it changes, and is answered by an owner's decision or
recorded as it stands when the verdict is filed; none of them changes a class above.

### Decision D12, the registry revision, and the registry commit

- **F1. D12's list of `JsParser` sites, completed.** The relation-split rows `JsParser` emits are EE0180 (2402 at the
  repeated attribute key, `JP:951`), the four `new.target` rows EE0163, EE0173, EA0002 and EP0016 (2101 at
  `ParseNewTarget`, `JP:5680`, one condition) and EX0028 (2104 at `BigIntLiteral`, `JP:6395`, and at `BigIntKey`,
  `JP:6412`); each site serves its rows alone (D12-1 to D12-4, plan-critic B2 reproduced). The other codes the mapping
  marks `split` at `JsParser` sites are EE0087 and EE0155 (2102 at `Expect`, `JP:7277`; 2209 at `BindingName`,
  `JP:6666`; 2102 and 2209 at `ParseObjectBindingPattern`, `JP:2694` and `:2723`, which an arrow head reaches with no
  cover), EE0168 (2202 at `ValidateDeclaredNames`, `JP:3818`), EX0008 (2102 and 2209) and EX0023 (2101 at
  `ParseFor`, `JP:1918`). Three of EX0008's sites serve EX0008 alone: 2102 at `ValidateAccessorParameters` (`JP:4268`)
  and 2209 at `ParseFunctionRest` (`JP:2384`) and `BindingName` (`JP:6610`) (D12-7, D12-8); the others serve several
  rules (`JP:5889`, `:6236`, `:6666`, `:2723`, and `TryParseArrow`'s `:4748`). And whether the library reports EE0087 or EE0155
  at an uncovered arrow head, where `JsParser`'s generic sites report EX0008's or EX0009's codes, decides which inputs
  class R2 holds (D12-5). **Changes:** D12's list and rule 4.1's registry-commit paragraph name the code arguments
  the registry commit changes: `JP:951`, `:5680`, `:6395`, `:6412`, and also `:4268`, `:2384` and `:6610`, whose moves
  are then class R, not R2; class R2 is confined to `JP:3818`, `:7277`, `:6666`, `:2694`, `:2723`, `:5889`, `:6236`, `:4748`
  and `:1918`; and the verdict states, before step 1, which code the library reports at an uncovered arrow head.
- **F2. Split other codes at binder sites are outside class R2 and outside the registry commit.** `JBW:836-844`
  (2205 for EE0030 and EE0031, `delete` of a private reference: live on the corpus, the `delete` early-error family,
  e.g. VB T034864, T034871), `JBW:1172-1181` (2203 for EX0009, a destructuring declaration without an initialiser:
  live in VK) and `JBE:695-720` (2201 at 0:0, EE0169 with EE0168: two rows) reach codes the mapping marks `split`.
  Rule 4.1 has the registry commit change "the emission constants of the codes revision 19 renumbers in the binder and
  the lowering" (relation splits) and R2 admit consolidations "reached at a `JsParser` site" only. **Changes:** D12
  and rule 4.1's registry-commit paragraph: the registry commit also changes the code at each binder site of a split
  other code that serves that rule alone (`JBW:836-844`, `JBW:1172-1181`); class R2's wording admits a consolidation
  at a binder site that serves several rules (`JBE:695-720`), reported by the library from the after commit, where D20
  deletes the binder's check.
- **F3. "One new code per meaning" is not a constant change at four binder sites.** `JBW:943` (a `super` property in a
  non-method, and `super` in script or module code), `JBW:966` (the same for calls), `JBE:517` (every
  regular-expression pattern rule, told apart only by the matcher's message) and `JBE:215` (EX0031 against EE0167,
  EE0177 and EA0009) each serve several split meanings with one constant (D-b's adjacent table). **Changes:** D12
  says whether a meaning is a catalogue condition (a message identifier) or a registry meaning group; if the former,
  rule 4.1's registry-commit paragraph admits the context tests those sites need, and D7's provider reports which
  pattern rule failed (neither `Format.JsRegExpMatcher` nor the provider does today).
- **F4. The mapping's and the catalogue's cited sites are stale.** `MAP:16` still reads `sites-read-at: d82b07a`. Of
  the sites the split rows cite, three are unchanged, the ten `JsParser` sites hold the same refusal on moved lines,
  and at the others the cited lowering site is now a `BinderAdmitted` assertion with the refusal in the binder (D-h).
  The catalogue's cells are stale too: EX0023's note names an ambient `CoreScript.AllowTopLevelAwait` that is now
  `JSCompilationOptions.AllowTopLevelAwait` passed by value; EX0011's and EX0012's `JsParser` lines have moved; the
  regular-expression rows cite the lowering's `JsCompiler.Literals.cs:447`, where the check is the binder's
  `CheckRegExp` since SJL-12.6. Rule 4.1's part E reads cells, not sites, so it cannot see either drift.
  **Changes:** the mapping's revision at the registry commit (WP6 commit 6.1) restates `sites-read-at` as the base
  commit and cites the binder sites in place of the assertions, from D-b, D-d and D-h; D28's stage table re-reads the
  catalogue's sites (as planned) and says which cells are stale.
- **F5. The checks lane pins codes and texts that revision 19 and step 1 change.** `SC/BinderChecks.cs:104-305` and
  `SC/BigIntChecks.cs:41`, `:589`, `:607` pin codes revision 19 renumbers (`super`, `return`, the static block's 2207,
  regular expressions, private names, `arguments`, EX0028's 2104), and `SC/SourceFrontEndChecks.cs:113-157` pins codes
  and texts (D09, D06). Rule 4.1 lets the registry commit change nothing in the slice compiler root but the corpus
  producer's mirror, so its `--checks` pre-check fails; at step 1 the rows of the deleted binder checks change, and VK:159
  with D30's correction if it lands. **Changes:** rule 4.1's registry-commit condition and WP6 commit 6.1 name
  `SC/BinderChecks.cs`, `SC/BigIntChecks.cs` and `SC/SourceFrontEndChecks.cs` among the registry commit's files, and
  WP6 commit 6.3 among step 1's.

### Decisions D10, D11 and D18: what the library reports, and when

- **F6. D18's admission walk misses a policy refusal, and its classes are empty on the corpus.** (a) `JP:1479`
  refuses a module specifier holding a lone surrogate (EX0029, a Policy row, 2101, with the export-name check's text
  copied) during the parse (D18-5, PD17). D10 keeps every engine-only Policy row out of the library, and D18's walk
  names only the BigInt refusals, so after step 1 nothing refuses EX0029. (b) No input of the parse corpus reaches a
  BigInt refusal (VB's first-2104 rows are all the decorator's), so rule 4.1's part A kind (B) and class S3's BigInt
  clause are expected empty there; only the probes (PD05 to PD07, C01, C02) and `SC/BigIntChecks.cs` exercise them.
  **Changes:** (a) D18 and rule 4.1's paragraph "The admission walk" add EX0029, with S3's and kind (B)'s wording, or a
  dated note moves the refusal to the module graph; (b) none to a rule: the verdict records that kind (B) and S3's
  BigInt clause are expected empty on the corpus.
- **F7. EX0023's emitter and the top-level-`await` checks row.** `SC/SourceFrontEndChecks.cs:113-157` holds that a
  module compiled with top-level `await` off refuses `await`, `for await` and `await using` at its top level with the
  same code **and** text as the same construct in a non-async function (VK:35). Under the mapping EX0023 is 2209 while
  `for await` in a non-async function is EX0008 (2101), and EX0023's message identifier
  (`engine.top-level-await.disabled`) is not EE0007's (D18-4, D12-10). If the library emits EX0023 for the switch, the
  row fails at step 1. **Changes:** D10 (the `request` row EX0023) and D11: either the library reports the in-function
  rule for a top level whose switch is off (EX0023 then has no emitter of its own, and the mapping's EX0023 row says
  so), or step 1's merge revises the checks row and D11 names the two texts.
- **F8. Top-level `await` is a Broiler.JS switch of the Script goal too.** `FastCompiler`'s
  `ScriptWithTopLevelAwait`, `JSContext.EvalWithTopLevelAwaitAsync` and the shell's retry on "Unexpected await" ask
  for a Script whose top level may await (R05, RP10, RP11, RT01, RT02); Broiler.VM reads the switch under the Module
  goal only. JSD-0080's request has the switch with no goal restriction, but D10 calls EX0023 a module row, stage 4a
  names "the top-level-`await` switch", and configuration J sets "top-level `await` as Broiler.JS's module options set
  it". **Changes:** stage 4a and rule 4.3's configuration J say that the switch applies to the Script goal when the
  request sets it (Broiler.VM never sets it there); D46 keeps "Unexpected await" as a text a host reads (already
  named); SJL-14's class E and T rows include `{ await using x = null; }` in a script, which the library refuses at
  the parse where `SyntaxValidation` refused it, so the shell's retry fires only if the bridge renders the library's
  refusal as "Unexpected await".
- **F9. Broiler.VM's parse-stage lists are cascades, not all errors.** After its first refusal `JsParser` stops at the
  end of the statement in progress, skips every scope check, and each scope validator reports one conflict (D04, D05;
  PD08 gives `[2205:1:17]` and never the duplicate at 1:14; PD09 gives one conflict of two). Many refused rows of VB,
  B2 and B3 hold more than one diagnostic. Rule 4.1's part B compares, for an input refused at both commits, "the
  ordered diagnostic list"; no class admits a parse-stage tail that differs, so a library that recovers otherwise,
  reports in source order, or reports every conflict is NOT MET on most multi-diagnostic inputs (D04, N13). **Changes:**
  D11 states what Broiler.VM's request asks the library for (all errors, with what recovery, or stop at the first
  refused statement), and rule 4.1's part B either gains a class for the parse stage's diagnostics after its first
  (that first equal up to R, R2 and S1 to S6) or compares the parse stage's first diagnostic and the later stages'
  lists.
- **F10. Diagnostic anchors have no decision and no class.** Rule 4.1's part B compares ordered `code:line:column`
  lists and admits "no ordering, template or position class: ... the adapter reproduces every anchor and offset
  (decision D15)", but D15 covers the tree's anchors only. Broiler.VM anchors a lexical refusal at the construct's
  start, a parse refusal per site (the current token, the production's start, a node, or one named token) and a
  binder refusal at a node's span; Broiler.JS at the scanner's stop point, the current or rewound token, or nowhere
  (N10, N11, N12, N24). **Changes:** D15 (or D11) gains: "the library's diagnostic span begins where Broiler.VM's front
  end anchors that diagnostic at the base (the construct's start for a lexical refusal; for a parse or binder
  refusal, the token or node `JsParser` or the binder names), and Broiler.VM renders the span's start", with
  `analysis/jsparser-anchors.tsv` and `analysis/binder-anchors.tsv` as the site lists; Broiler.JS's message positions
  change under SJL-14's class M. The alternative, a position class in part B, would weaken it.
- **F11. D15's wording.** "Offsets, lines and columns in UTF-16 code units, 1-based" reads as if offsets were 1-based;
  both engines' offsets are 0-based (N1). **Changes:** D15 reads "offsets 0-based UTF-16 indices; lines and columns
  1-based, columns in UTF-16 code units". The request takes text, and a leading byte-order mark is the host's to drop
  (N5).

### Decisions D15, D19, D10 and D20: what the adapter reproduces and what step 1 deletes

- **F12. The adapter reproduces Broiler.VM's computed facts, and one of them differs from the specification.** Besides
  anchors, the adapter must rebuild every fact `JsParser` computes into `JsSyntax` (P32's fifteen items: strictness,
  function kinds, names, keys, parenthesisation, the chain wrappers, the desugarings and the synthetic names), and
  `SawTopLevelAwait` (R06), which the lowering writes into the artifact. `JsParser` counts an `await` inside an async
  arrow at a module's top level as a top-level `await` (P32 item 10, `VMC/JsParser.cs:2503-2506`, `:2833-2836`); the
  specification's HasTLA and Broiler.JS do not; and the run shows it observable (PR06: Broiler.VM `a,tick,b`,
  Broiler.JS `a,b,tick`). Rule 4.1's and 4.2's part B admit no byte difference of an accepted input. **Changes:** D15's
  list ("every anchor and offset `JsParser` gave") becomes "every anchor and offset and every computed fact of
  P32", top-level `await` among them, computed by Broiler.VM's rule at step 1 and step 2; stage 4c's tree carries the
  top-level-`await` fact (JSD-0080's output table puts it in SJL-15's module record); the correction is SJL-15's model
  (hand-off H19).
- **F13. The library's outputs name no strictness, and D10's side table only declarations.** Broiler.VM's adapter
  needs each program's and function's strictness (P32 items 1 and 2); Broiler.JS's bridge, to rebuild
  `AnnexBFunctionNames` by `RegisterAnnexBFunctionHoisting`'s rule (P33 item 2), needs each scope's kind, the enclosing
  function's parameter names, the lexical declarations and each block-level function declaration with whether it is an
  `if` clause's; `HoistingScope` is built in `FastScopeItem`'s insertion order, which agrees with source order while no
  name is removed (P30). **Changes:** D10's side table and stage 4d name those facts and order them by source
  position, and the library exposes strictness as a side fact (it computes both for its own early errors, so there is
  one producer); or D15 and D44 say that each engine recomputes them from raw facts, and name the facts.
- **F14. D20's deletion list, X6 and I5, member by member.** D20's examples miss live binder checks of `syntax` rows:
  EE0005's bare import specifier (B26; VB T041639, T041642), EX0009 (B12), EE0028 and EE0029 (B07), EE0030 and EE0031's
  private half (B06), EE0168 and EE0169's module checks (B27 to B29, B34); and dead copies (B05, B10, B11, B14, B17 to
  B20, B30, B31). EX0021 is no binder check (the lowering's, P05b), and EE0182 has no member of its own (`JBE:678-690`
  also refuses EE0171, a `semantics` row, and stays: B33). The lowering's `BinderAdmitted` assertions mirror the
  deleted checks. `JsParser`'s `DefaultBindingName` and `StarName` are read outside the file step 1 deletes, by the
  lowering and the binder (P29), and `SawTopLevelAwait` by the compiler (P32 item 10). **Changes:** the verdict's X6 and
  rule I5's registrations list B03 to B07, B10 to B14, B17 to B20, B25 to B31 and B34 by member (and B15, B16, B21 and
  B27 as F15 settles), keep B33, say what happens to each matching assertion, and say where the two constants and the
  top-level-`await` fact live at step 1 (the adapter or `JsSyntax.cs`) and at step 2 (the lowering's module code).
- **F15. Four binder checks wait on task 1.2's stage.** EE0017 (`{ a = 1 }` outside a pattern, B15), EE0018
  (`__proto__` twice, B16) and EE0054 (a `for`-`in`/`of` head that is not a reference, B21) are checked by no parser
  today, so D10's definition makes them `semantics`, though EE0017 is purely grammatical and EE0054 is EE0033's
  condition in a `for` head. EE0179 (one ImportDeclaration binding a name twice) is checked by no parser either, and the
  binder check that refuses it (B27) also serves EE0168, which is `syntax`. **Changes:** task 1.2 assigns each row's
  stage; D20 deletes B15, B16 and B21 only if their rows are `syntax`, and B27 only if EE0179 is `syntax` too
  (otherwise the binder keeps B27 for EE0179, and X6 says so).

### Decisions D17, D21, D23 and D25, and the rules' parts

- **F16. The import-phase proposal flag must be one per form.** JSD-0080's request lists "import phases" as one flag,
  and rule 4.3's configuration J sets "Broiler.JS's admitted proposals on" (D17: "Broiler.JS admits what task 1.1
  finds it parses today"). Broiler.JS parses the call forms `import.defer(…)` and `import.source(…)` (EP0012, EP0014;
  P06a) and refuses the declaration forms `import defer * as ns from` and `import source x from` (EP0011, EP0013; P06b),
  as the catalogue's `js_admits` says. With one flag on, the library would accept every P06b positive in configuration
  J against both baselines. **Changes:** D17 and the stage table give the import phases one flag per catalogue row (or
  per form), and configuration J sets the two call-form flags only, so every P06b input is expected refused in V and J;
  R09 states Broiler.JS's answer per form. (One flag with J expecting the declarations accepted would have SJL-14 accept
  `import defer * as ns` in an engine with no deferred namespace; not recommended.)
- **F17. Rule 4.1's part F, as written, fails on escaped identifiers.** Part F compares "kind (through the row's
  fixed kind map), raw text, numeric and string value, line, column, offset and end, the preceded-by-line-terminator,
  legacy-octal and escaped flags". `SliceToken` keeps those facts otherwise than a token that keeps raw facts (T31,
  T32): an escaped identifier's `RawText` is its cooked name while `Offset` and `End` span the escaped spelling
  (`VMT:1054-1063`, `:690-691`); `StringValue` is empty for every identifier; an escaped word's kind is `Identifier`
  whatever it spells (`of`, `async`, `let`, `static` included); `IsEscaped` is set on identifiers only, never on a
  string holding an escape. The run confirms every field (T32A to T32C). B4-accepted inputs of VB hold escaped
  identifiers (`analysis/b4-escaped-tokens.txt.gz`, the first T040442) and directive strings holding an escape
  (J000262, J001009, J001493, J001531, J002042, J002129), so a library token whose raw text is the spelling, or whose
  escape flag marks a string, differs on them. Two further differences are latent: the HTML-like comment switch, which
  the slice path sets off for the Script goal where the library follows the goal (T30), and the slice tokenizer's
  acceptance of white space inside an escape's hexadecimal digits (T13), which can reach part F only through an
  identifier, since the slice manifest refuses every string in an expression (the run, T13A to T13F). The verdict's
  "Falsified if" counts any disagreement on a slice-accepted input as a falsifier, and part F admits no class.
  **Changes:** before rule 4.1 is filed, part F's configuration states the field map: for an identifier,
  `SliceToken.RawText` against the library token's cooked name, and the text between `Offset` and `End` against the
  library's raw spelling; the string value for string literals only; the kind map applied to the library's kind and
  escaped flag, an escaped word mapping to `Identifier`; the escaped flag for identifiers only; and the library's
  request with HTML-like comments off, as the slice path sets it. (The alternative, stage 4b's token taking
  `SliceToken`'s conventions, would make the library's raw text a cooked value and drop the raw fact the design keeps;
  not recommended.) D21 says the same.
- **F18. The left-operand gap is wider than the records say, and its class is an implementation limit.** Bundle
  `sjl-12-001`'s README, D23, rule 4.1's part C fifth row and rule 4.3's part G name a chain whose first operand is a
  parenthesised chain and a template whose substitution holds one. The base restores the depth at the end of both
  builders (`VMC/JsParser.cs:5136`, `:5549`), so a member chain as the first operand of a binary chain with no
  parenthesis (RP04) and a parenthesised binary chain as the object of a member chain (RP05) pass the bound too; the
  base compiles all four shapes in both forms and loses no process (RP01, RP02, RP04, RP05). The specification sets no
  depth limit, so the base's acceptances are correct and the library's charge is an implementation limit (R14,
  implementation-defined, with R13). **Changes:** D23's fifth checks row, rule 4.1's part C population and rule 4.3's
  part G corpus add RP04's and RP05's shapes; rule 4.1's part A kind (F) and part B class F list none of these inputs,
  which are class L and C3 (with their depth), as rule 4.3's outcome L and rule 4.4's C3 admit them; risk R7 at the
  base is accounting, not robustness.
- **F19. Corrections of Broiler.VM that step 1 makes and that no test262 file or corpus input reaches.** T6, T7, T8,
  T13 (the wide path), T16 (a strict legacy octal object key or class element name), T24d to T24f, D32 (`new.target`
  in a class field initialiser at depth 0, `JP:5680`), P31 (an object rest whose target is a literal), P05a (an import
  attribute key that is not a letter-initial name) and D40 (a string or reserved word as an import specifier without
  `as`) have no test262 file and no corpus input; only P15b among the corrections of Broiler.VM has corpus inputs
  (J000802, J001458). Rule 4.1's part A kind (F) and part B class F list "a row of the correspondence in its column
  Correction of Broiler.VM", so they will list nothing for these rows, and part H's C1 lists only the rows a vector
  carries. **Changes:** WP2 task 2.1's lexical and `syntax` vectors carry every one of these rows a Script or a direct
  eval can carry (T6, T7, T8, T13, T16, T24d to T24f, D32, P31), so that part H's C1 names each with its row; the
  module-goal ones (P05a, D40) are shown by this file's probes only, and the verdict lists them so; part B's class F
  names P15b and D32.
- **F20. The native whole-suite floor still holds the unsupported row the bytecode floor retired.** The bytecode floor
  (`src/tests/conformance/floors/test262-wide.floor`) holds `atMost unsupported 0`; `test262-wide-native.floor` still
  holds a non-zero `atMost unsupported` row, the BigInt row the bytecode floor retired by hand on 2026-09-21. At step
  1, a variant whose first diagnostic becomes 2104 crosses the bytecode floor, while the native floor would let such
  moves through unnoticed. `IngestionChecks.TheLanguageClassMapIsTotalOverTheVocabulary` fails until every new code has
  a `LanguageErrors` class (the registry commit's grading and classification cover it). **Changes:** D13 adds: before
  SJL-13's after runs, the native floor's unsupported row is re-set to 0 by hand with its reason, as the bytecode
  floor's was; otherwise the native form does not hold D13's "no unclassified move".

### Decisions D16, D44, D46 and D47: the inherited context and Broiler.JS's bridge

- **F21. The caller's lexical names.** JSD-0080's request lists "the visible private names, and the caller's lexical
  names, which Broiler.JS needs for direct `eval`"; stage 4a, D16 and D44 list the flags and private names only. The
  conflict Broiler.JS checks with the lexical names is EvalDeclarationInstantiation's (a run-time SyntaxError Broiler.VM's
  executor makes), not an early error (R28, RE35 to RE38). **Changes:** D44 and stage 4a say that the request's
  lexical names are absent at SJL-13 and SJL-14, as D16 says of private names, and that the check stays Broiler.JS's
  until SJL-15's model; rule 4.3's part H lists that request field as "absent" with its reason.
- **F22. "Static-block flags" name no input of either engine.** D44 and stage 4a name "static-block flags", but
  PerformEval has no such parameter: a static block's body function is a method with no
  `[[ClassFieldInitializerName]]`, and Broiler.VM compiles a static block as a method (`InFunction`, `InMethod`). Both
  engines answer `"object"` for `eval('typeof arguments')` in a static block (RE29), the specification's reading; only
  V8 (node) refuses it (R26). **Changes:** D44 drops the term, or defines it as "the request's function and method
  flags, set for a static block"; the stage table reads EA0005 for a static block as both engines and the
  specification do.
- **F23. Broiler.JS's derived-constructor test admits `super()` in a base-class constructor's eval.** The run of RE19
  shows `eval('super()')` in a base-class constructor admitted at the parse and failing at run time with `TypeError:
  Value is not a constructor`, where the specification and Broiler.VM refuse it as a SyntaxError: the call site grants
  a super call when the root scope has `MemberInits` (`JSC/Expressions/FastCompiler.VisitCallExpression.cs:134-136`),
  which every class constructor has, a base class's included (`JSC/Declarations/FastCompiler.CreateClass.cs:705`;
  `JSC/Scope/FastFunctionScope.cs:564`) (R24). **Changes:** D44's inherited context says the derived-constructor flag
  is computed from the class's heritage (a constructor of a class with an `extends` clause), not from `MemberInits`;
  SJL-14's `corrections.md` gains R24 (a TypeError at run time becomes an early SyntaxError: class C1 in its part A
  if test262 holds such a file, else a K row).
- **F24. A strict caller's direct eval in Broiler.JS is positioned in a composed text.** Broiler.JS parses
  `"use strict";\n` + the text (`JSC/DirectEvalSupport.cs:127-128`), so every position its messages carry is one line
  below the text's (N25; NP16, NP17, NR02 cases A and B), and a hashbang at the text's start is refused (N26, NR02 case
  C); its parser has no forced-strictness input (R03). **Changes:** (1) D44's list: when the library parses direct-eval
  code, the bridge passes the caller's strictness in the request, and `DirectEvalSupport` drops the prefix **together
  with** the two places that skip the prefixed statement (`TryEvaluateInertProgram`, `:460-465` and `:494`;
  `TryGetSingleStatement`, `:561-570`) and the strict-reserved-word re-scan (`:704-705`, `:743-787`); left behind, the
  skips would drop a strict caller's own first statement (`eval("5")` would answer undefined, and `eval('a = 1;
  eval("a")')` would run only its second statement). While the switch is off the old parser has no strictness input,
  so on that path the prefix stays until W2. (2) D46: N25's move (one line less) and, if the dynamic function's goals
  are used, N27's (body lines two less, parameter columns eleven less) are class M with the same exception type; no
  entry point of SJL-14's parts C and D runs a direct eval, part A reads no message, and no Broiler.JS test asserts a
  position inside eval code, so SJL-14's task 1.1 probes of the inherited context in strict callers record the message
  text, position included, and the generated table and the changelog list it. (3) K10 gains N26, with NR02 case C as
  its evidence; no test262 verdict moves, because Broiler.JS's runner runs `hashbang/eval.js` and
  `no-line-separator.js` sloppy only. (4) Positions compared across engines for strict eval code are shifted by one line,
  or compared on sloppy rows (F29).
- **F25. The two token re-scans should be deleted at SJL-14, not moved onto the bridge's token chain.** Plan section
  2.2, WP4 task 4.3, and D41's reading of "the remains of `SyntaxValidation`" keep `DirectEvalSupport.ContainsStrictReservedWordUsage`
  and `SyntaxValidation.ContainsLegacyOctalLiteral`, walking the bridge's token chain. Each checks a `syntax` row the
  library checks at the parse once the request carries forced strictness (EE0012's `DirectEvalSupport` site; the
  strict legacy-octal rows, T16). The reserved-word re-scan refuses legal programs, a class element named `static` or
  `public` in a strict caller's eval (R36; RE43, RE44, RE47: Broiler.VM and the specification 1, 2, 4; Broiler.JS
  "Unexpected strict mode reserved word"). The legacy-octal re-scan runs only for a whole-program strictness and counts
  a second digit 0 to 7 only, so it misses `08`, `09` and `08.5` and a class body's strictness; a function's own
  strictness is checked at K by `FastCompiler.EarlyErrors`' per-function detector, with the same digit test (the run,
  T16E and T16F). **Changes:** WP4 task 4.3 and D41's reading: both re-scans are deleted in W2, and the eval prefix with
  them once the bridge passes strictness as a switch (F24); SJL-14's `corrections.md` gains R36 (class C1 with a K
  row); survey-js OQ3 is answered "deleted". If the plan keeps the re-scans instead, this file's R36 is a known Broiler.JS
  divergence carried to SJL-15 with its probes, and rule 4.4's part C expects RE43, RE44 and RE47 to differ.
- **F26. D44's bridge and D47's parse thread must name every Broiler.JS entry point.** The bridge serves six
  construction sites, and eleven routes reach `FastCompiler` (R35's inventory). Direct eval is parsed twice: J2 checks
  the inherited context from call-site constants, and J1g reads it from `JSEngine.Current`; once both go through the
  library, the context must be passed at both, or the second parse refuses what the first admitted. J2, J3, J4, J6, and
  J5 when J2 calls it, parse off `CompilationStack`, so D47's thread must be entered at each, or the library's limits are
  checked on a stack the derivation (D14) did not assume. J1k, the opt-in tiering recompile, passes the Script goal for a
  function that may have been compiled as module code, and gets strictness by a `'use strict';` prefix. **Changes:** D44
  lists the call sites (`DirectEvalSupport.Validate` and `ValidateIndirectEval`, `CreateDynamicFunction`'s four compiles,
  `ShadowRealm.evaluate`, the module loader and `ModuleSourceAnalysis`, `PortableCompiler`, the debugger and
  `RecompileForTiering`), each with the request fields it passes; D47 names the entry points that must hop; J1k's goal is
  hand-off H24.
- **F27. D44's bridged token facts must include each token's source, offset, length and start location.** Broiler.JS
  gives offsets meaning beyond a position: a tagged template's object is keyed by its start offset, a function frame's
  code offsets decide direct eval's lexical visibility, `ClassBodyMayDirectEval` scans member source by offsets, and the
  parser's ASI test for `.5` reads a node's end line (N18). **Changes:** D44's list of synthesised `FastToken` facts
  names the library token's source, offset, length and start location.

### Decision D45 and SJL-14's lists

- **F28. SJL-14's corrections list gains rows this file classes, with their stages.** Every correction of Broiler.JS
  above is SJL-14's (hand-off H22). Beyond the plan's K1 to K9, these need naming in `corrections.md` before SJL-14's
  evidence: N11 (the end of input at 0,0), N25 and N26 (F24), R24 (F23), R36 (F25), T24a, T24b and T24f (a
  regular-expression token the parser takes as a binary operator: P accepts, K refuses "Undefined binary operation
  RegExLiteral", stage E; the run, T24B, T24B2, T24F), D40 (a reserved word other than the five keyword-typed tokens as
  an import specifier without `as`: accepted at P and K; the run, PD14), D63 (the catalogue rows whose Broiler.JS check
  is in `StrictModeValidator`, which never enters a `case` or `default` clause's statements: accepted at P and K today,
  so the library's refusal is class F on Broiler.JS's side, not class E; PD29, PD30), and Broiler.JS's own tests that
  assert acceptance past the contract's limits (`Broiler.JavaScript.Compiler.Tests/DeeplyNestedSourceTests.cs:26`,
  `:42`, `:66`, `:101`, `:138`; corpus J000243, J000451, J000799, J001976), which are C3 under rule 4.4 or are revised
  at SJL-14 (D27, D47). **Changes:** D45's K10 names these rows; the stage table's `js_stage` for D63's rows carries
  the case-clause hole, so that SJL-14's parts A and C meet the moves with a named row.
  *(Note of 2026-10-27, decision D-asi.)* The rows that need naming in `corrections.md` beyond K1 to K9 include
  **P16b**, automatic semicolon insertion in a for statement's head: Broiler.JS accepts a line break where the head's
  first `;` belongs after a `var`, `let` or `const` declaration, at P and K (PP11 to PP13, PE01 to PE03), and the
  catalogue's `completeness.md`, which K8 reads, does not list it. D45's K10 names it by JSD-0083's note of the same
  day. T24b and T24f, named above, also record the answer where `;` or the end of input follows the literal: P and K
  accept and the program completes with the left operand's value (decision D-t24; T24B3 to T24F3).

### Task 1.2, the stage table and the classification

- **F29. What task 1.2 must read and do.** (a) The corpus join's files: every input whose outcome differs between B1
  and P or K has a row or bucket (`analysis/rows-P-baseline.tsv.gz`), and every test262 positive both engines refuse
  has one too (`analysis/rows-P-agree.tsv.gz`: P06b and P35); `classify_parse_corpus.py` reads both, or makes the same
  cuts, so that every input has a row or a reason. (b) Positions are compared on `sloppy`, `raw` and `module` rows, or
  JB's `strict` lines shifted by one (N9, F24). (c) The regular-expression differences of the corpus are validator
  differences behind the provider (R19, R20), so configurations V and J give different first codes for the same pattern
  and the classification marks those inputs by configuration. (d) EE0044 has two Broiler.VM halves, `using` and
  `await using` refused by the parser (`JP:1720`) and `const` by the binder (B13); the catalogue's `vm_wide_stage`
  (`lowering` for both) is stale twice, and the stage table records each half (both are the library's: EE0044 is
  `syntax`). (e) EA0005's stage decides whether its 0:0 place becomes a real one at step 1; if `syntax`, rule 4.1's part
  B class F needs a row in the column "Correction of Broiler.VM" for it (N14). (f) P08, P15b and P24 are rules each
  engine checks at a later stage than the grammar puts them; their stages are the table's (D31). (g) The strict
  legacy-octal rows' Broiler.JS stage is `SyntaxValidation` for a whole-program strictness and `FastCompiler.EarlyErrors`
  for a function's own (T16). **Changes:** PLAN task 1.2's inputs and checks name (a) to (g); none changes a decision.
- **F30. Broiler.VM's binder still refuses `continue` to an outer label of a label set, and the catalogue is stale
  there.** D-e: PD01 2208:2:21, RD01 FAIL; Broiler.JS accepts since #1075, and the specification adds every label of a
  label set to the loop's iteration set (ContainsUndefinedContinueTarget). The catalogue's "both" for EE0082, EE0137,
  EE0166 and EE0176 and `completeness.md:235-236` are stale for Broiler.JS and wrong for the specification. The rows
  are `semantics` (no parser checks jump targets), so the correction is not the library's at SJL-13. **Changes:** the
  stage table records the rows as `semantics` with each engine's answer; the verdict records the correction of
  Broiler.VM and hands it on (H20): a Broiler.VM change to the binder's `Target`, with the checks row
  `SC/BinderChecks.cs:127-130`, or SJL-15's jump-target fact; D28's note says the catalogue's cells for these rows are
  stale.

### Records of section 3.1 and the plan's section 1.2

- **F31. JSD-0071's export-name check belongs to SJL-13, not SJL-15.** JSD-0071:107-108 assigns `JsParser.IsWellFormed`
  (EE0178, an export name that is not well-formed Unicode) to SJL-15, and the plan's section 1.2 repeats "JSD-0071's
  lone-surrogate export name to SJL-15"; but step 1 deletes `JsParser` (D20), and D10 makes EE0178 a `syntax` row the
  library refuses, since both parsers refuse it at parse time (H11, D56). **Changes:** D39's list and section 3.1
  item 4 gain a dated note on JSD-0071's "What stays" (the check moves to the library at step 1, with EE0178's code,
  2101 within meaning); the verdict's hand-off table carries H11 to H17 beside H1 to H10, with their sources and owners.
  JSD-0065:66 (H12) needs no SJL-13 note: section 3.2 files it at SJL-14.1. EX0029's owner after step 1 is F6's.
- **F32. P26's correction of Broiler.VM has no step.** Broiler.VM's dynamic function assembles its parameters and body
  into one source and evaluates it as indirect eval (`VMP/JsRealm.Dynamic.cs:159-187`), so `new Function("/*",
  "*/){")` is admitted (PR01) and `new Function("", "}); (function(){")` answers the second function (PR02). The library
  carries the FormalParameters and FunctionBody goals from stage 4a, but no planned Broiler.VM change routes
  `FromSource` through them. **Changes:** the verdict's hand-off table gains H18; rule 4.1's part A kind (F) and part B
  class F list nothing for P26 at either step (no input of B1 to B5 is a `Function` call, and every
  `test/built-ins/Function/` file passes in VT); Broiler.JS's half (PR02) is SJL-14's under K10 (D44 passes the goals).

## The rows by class

Each row is listed once, under its class; a row whose class cell names a second class for some of its inputs is listed
under the class that differs (a correction before "same"), and the note below the table names the second class. The
area verdict filed with this file records these classes.

| Class | T | P | N | D | R | Rows |
|---|---|---|---|---|---|---|
| Same | T1, T2, T3, T4, T5, T10, T11, T14, T15, T17, T20, T24g, T24h, T25, T27, T29, T32 | P01, P03, P05d, P05e, P06b, P08, P16, P17, P25, P35 | N1, N2, N3, N4, N5, N6, N7, N8, N16, N17, N20, N21 | D12-1, D12-2, D12-5, D12-6, D12-7, D12-8, D12-9, D31, D43, D44, D45, D47, D48, D50, D51, D52, D53, D54, D56, D57, D58, D59, D60, D62, B01, B02, B03, B04, B05, B07, B08, B09, B10, B11, B12, B13, B14, B15, B16, B17, B18, B19, B20, B22, B23, B24, B26, B27, B28, B29, B30, B31, B32, B33, B34 | R02, R03, R06, R07, R08, R17, R22, R23, R25, R26, R27, R28, R30, R31, R33 | 109 |
| Correction of Broiler.VM | T6, T7, T8, T13 | P05a, P15b | none | D30, D32 | R19 | 9 |
| Correction of Broiler.JS | T12, T19, T21, T23, T24a, T24b, T24c, T24i, T26, T28 | P02, P05c, P07, P09, P10, P11, P12, P13, P14, P15a, P16b, P18, P21, P22, P23, P24 | N11, N25, N26 | D33, D34, D35, D36, D37, D38, D39, D41, D42, D46, D55, D63, B06, B21 | R20, R24, R36 | 46 |
| Correction of both | T16, T24d, T24e, T24f | P26, P31 | none | D40, D49, D61 | none | 9 |
| Implementation-defined | T9, T31 | none | N10, N12, N24, N27 | D02, D05 | R13, R14 | 10 |
| Engine policy (a parameter of the request) | T18, T22, T30 | P06a, P19, P20 | N13 | D03, D04, D12-10, D18-4, D18-6, D18-7, B25 | R04, R05, R09, R10, R12, R16, R18 | 21 |
| Engine-kept | none | P04, P05b, P27, P28, P29, P30, P32, P33, P34, P36 | N9, N14, N15, N18, N19, N22, N23 | D01, D06, D07, D08, D09, D12-3, D12-4, D18-1, D18-2, D18-3, D18-5 | R01, R11, R15, R21, R29, R32, R34, R35 | 36 |
| All rows | 40 | 43 | 27 | 94 | 36 | 240 |

*(Revised 2026-10-27, decision D-asi: row P16b joins the class "Correction of Broiler.JS". The table read "| Correction of Broiler.JS | … | P02, P05c, P07, P09, P10, P11, P12, P13, P14, P15a, P18, P21, P22, P23, P24 | … | 45 |" and "| All rows | 40 | 42 | 27 | 94 | 36 | 239 |". P16 stays under "Same", narrowed to automatic semicolon insertion outside a for statement's head. T24b and T24f keep their classes, with the answer decision D-t24 adds.)*

**Notes.**
- **Rows listed under one class with a second class for some inputs:** P06a (engine policy for the call forms'
  positives; correction of Broiler.JS for the negatives under the flag); P32 (engine-kept; its item 10 is a
  correction of Broiler.VM that SJL-15's model makes, H19); T15, T27, P08, D12-2, D12-7, D31, D44, D45, D51 to D54 and
  D59 (same in outcome; Broiler.JS refuses at K, stage E: the stage table's matter, and SJL-14's classes E and T);
  D46 and D55 (same in outcome; the correction of Broiler.JS is PD21's declared name and PD29's case clause); B02, B04,
  B07, B08, B09, B10, B18 and B20 (same; the inputs they name are D30's, P24's, D39's, D41's, R24's and D32's).
- **Rows that restate another sub-area's row:** D33 holds T23's sites, D34 B06's, D36 P11's early-error half, D49
  P15a's and P15b's, D56 H11's, D58 T4's and R07's, D61 P26's; each keeps its own row so that the catalogue's index
  reaches it.
- **The corrections SJL-13 delivers** are the rows in the column "Correction of Broiler.VM" (and Broiler.VM's half of
  "Correction of both") whose change is the library's at step 1: T6, T7, T8, T13 (the wide path), T16, T24d to T24f,
  P05a, P15b, P31, D32 and D40. P26 and D61 (H18), P32's item 10 (H19), D30 (H20) and R19 (H21, SJL-11.4) are corrections
  of Broiler.VM that SJL-13 does not deliver. Every correction of Broiler.JS is SJL-14's (H22).

## Appendix: every catalogue row and the rows that hold its sites

One line per row of the early-error catalogue (`analysis/catalogue-index.tsv` holds the same with the catalogue's stage
cells and the counts this table leaves out). A row id names a row of this file whose table cites the catalogue row; a
`D:` bucket of the corpus join counts toward the D-f row that holds it; "D-b adjacent" is D-b's table of the binder and
lowering sites of split codes; "D63" marks a catalogue row whose Broiler.JS site is in `StrictModeValidator`, which a
`case` clause escapes. The corpus column reads VB's B1 and JB's P and K on the inputs the catalogue's test262 cell
names, or, where it names none in the corpus, on the inputs the parse-negative table ties to the row. Its kinds:
"both P" (Broiler.VM refused, Broiler.JS's parser refused), "VM; JS K (E)" (Broiler.JS refused only in its compiler:
stage E), "VM only" (Broiler.JS accepted at P and K, or crashed), "JS only", and "both accept" (positive or run-time
tests), each with one example. A row with no parse-time site in either engine says so.

| Catalogue row | Status | Rows of this file that hold its sites | Corpus (B1 / P / K on the inputs the catalogue names: kinds, an example) |
|---|---|---|---|
| EE0001 | both | D43 | both P (T040799) |
| EE0002 | both | D43 | both P (T040797) |
| EE0003 | both | D39 | VM; JS K (E) (T041204); VM only (T041212) |
| EE0004 | both | D44, D63 | VM; JS K (E) (T041580) |
| EE0005 | both | B26, D31, D63 | VM; JS K (E) (T051573); both P (T041642) |
| EE0006 | both | D12-9, D31, D37, D63 | VM; JS K (E) (T040476); VM only (T040518) |
| EE0007 | both | D12-9, D37 | both P (T042353) |
| EE0008 | both | D12-9, D37, D63 | both P (T050729) |
| EE0009 | both | D12-8, D12-9, D37 | VM only (T031140); both P (T042474) |
| EE0010 | both | D12-9, D33, D63 | VM only (T050728); VM; JS K (E) (T050728) |
| EE0011 | both | D12-8, D12-9, D33 | VM only (T042473); both P (T042475) |
| EE0012 | both | D12-9, D33, D37, D63 | VM only (T039150); VM; JS K (E) (T040439) |
| EE0013 | vm-only | D12-9, D33 | VM only (T043292) |
| EE0014 | both | D12-9, D33 | both P (T040770) |
| EE0015 | both | B09 | VM; JS K (E) (T039302) |
| EE0016 | both | D46, D63 | VM; JS K (E) (T039442) |
| EE0017 | both | B15, D63 | VM; JS K (E) (T038408) |
| EE0018 | both | B16, D63 | VM; JS K (E) (T038372) |
| EE0019 | both | D-b adjacent, B25 | both P (T041418) |
| EE0020 | both | D45 | VM; JS K (E) (T039979) |
| EE0021 | neither | D-b adjacent | no input |
| EE0022 | both | D45 | VM; JS K (E) (T039992) |
| EE0023 | both | D45 | VM; JS K (E) (T039979) (via the parse-negative table) |
| EE0024 | both | D45 | VM; JS K (E) (T039979) (via the parse-negative table) |
| EE0025 | both | D47 | both P (T029546) |
| EE0026 | both | D48 | both P (T039526) |
| EE0027 | both | B11 | VM; JS K (E) (T037836) |
| EE0028 | both | B07, D39, D63 | both P (T039629); VM; JS K (E) (T039630); VM only (T039630) |
| EE0029 | both | B07, D39, D63 | both P (T039696); VM; JS K (E) (T039664); VM only (T039664) |
| EE0030 | both | D-b adjacent, B06, D34, D63 | VM only (T034895); VM; JS K (E) (T035984) |
| EE0031 | both | D-b adjacent, B06, D34, D63 | VM only (T034879); VM; JS K (E) (T035983) |
| EE0032 | both | D49 | VM only (T029705); VM only (K crash) (T029711) |
| EE0033 | both | B18, B21, D31, D39, D63 | VM; JS K (E) (T030031); VM only (T030073) |
| EE0034 | both | B18, D31, D39, D63 | VM; JS K (E) (T035788); VM only (T030068) |
| EE0035 | both | D-b adjacent, B17, B18, D31, D39, D63 | VM; JS K (E) (T038118); VM only (T038110) |
| EE0036 | both | B20, D39, D63 | VM; JS K (E) (T029836) |
| EE0037 | neither | P31 | both accept (T052981) |
| EE0038 | both | D49 | both P (T029629) |
| EE0039 | both | B20, D31, D39, D63 | both P (T029619); VM; JS K (E) (T029651) |
| EE0040 | both | D35, D63 | both P (T028561); VM; JS K (E) (T028529); VM only (T028529) |
| EE0041 | both | D12-6, D35 | both P (T028577) |
| EE0042 | both | D39 | both P (T050969) |
| EE0043 | both | D50 | both P (T028486) (via the parse-negative table) |
| EE0044 | both | B12, B13 | both P (T047462) |
| EE0045 | both | D36, D63 | VM; JS K (E) (T050815) |
| EE0046 | both | D36, D63 | VM; JS K (E) (T050817) |
| EE0047 | both | D36, D63 | VM; JS K (E) (T050816) |
| EE0048 | both | D36, D63 | VM; JS K (E) (T047544) |
| EE0049 | both | D36, D63 | VM; JS K (E) (T051623) |
| EE0050 | both | D36 | both P (T050021) |
| EE0051 | both | D35, D36 | both P (T050015) |
| EE0052 | both | D36 | both P (T048877) |
| EE0053 | both | D36 | both P (T048865); VM only (T049059) |
| EE0054 | vm-only | B21, D36, D39 | VM only (T048869) |
| EE0055 | both | D36, D39 | both P (T049548) |
| EE0056 | both | D35, D36 | both P (T049547) |
| EE0057 | both | D36 | both P (T049545) |
| EE0058 | both | B02, D36 | VM; JS K (E) (T047486) |
| EE0059 | both | B01, D36 | VM; JS K (E) (T042963) |
| EE0060 | both | D36, D63 | VM; JS K (E) (T051804) |
| EE0061 | vm-only | D36, D63 | VM; JS K (E) (T051787); VM only (T051787) |
| EE0062 | both | D35, D63 | both P (T051106); VM; JS K (E) (T051089); VM only (T051089) |
| EE0063 | both | D12-6, D35 | both P (T051114) |
| EE0064 | both | D36, D63 | VM; JS K (E) (T050831) |
| EE0065 | both | D50 | both P (T051315) |
| EE0066 | vm-only | D35 | VM only (T051317) |
| EE0067 | both | D50 | both accept (T001086) |
| EE0068 | both | D51, D63 | VM; JS K (E) (T029486) |
| EE0069 | both | D51, D63 | VM; JS K (E) (T050225) |
| EE0070 | delegates | D51, D63 | VM; JS K (E) (T050459) |
| EE0071 | both | D52, D63 | VM; JS K (E) (T050448) |
| EE0072 | both | D31, D54 | VM; JS K (E) (T050487) |
| EE0073 | both | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0074 | both | B08 | VM; JS K (E) (T050424) |
| EE0075 | both | B08 | VM; JS K (E) (T050418) |
| EE0076 | both | B09 | VM; JS K (E) (T050423) |
| EE0077 | both | B09 | VM; JS K (E) (T050417) |
| EE0078 | both | D35 | both P (T028486); VM only (T028483); VM; JS K (E) (T028483) (via the parse-negative table) |
| EE0079 | both | D12-6, D35 | both P (T028490); VM only (T028542) (via the parse-negative table) |
| EE0080 | both | D53 | VM; JS K (E) (T041630) (via the parse-negative table) |
| EE0081 | both | B01 | VM; JS K (E) (T042974) |
| EE0082 | both | B02, D30 | VM; JS K (E) (T047493) |
| EE0083 | both | D38 | both P (T029483) |
| EE0084 | both | D38 | both P (T030371); both accept (T052876) |
| EE0085 | both | D54 | VM; JS K (E) (T029547) |
| EE0086 | vm-only | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0087 | both | D12-5, D37 | VM only (T029284); VM only (K crash) (T029281) |
| EE0088 | both | D54 | VM; JS K (E) (T039452) |
| EE0089 | both | D39 | both P (T039416) |
| EE0090 | both | D51, D63 | VM; JS K (E) (T029222) (via the parse-negative table) |
| EE0091 | both | D54 | VM; JS K (E) (T039445) |
| EE0092 | both | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0093 | both | B09 | VM; JS K (E) (T039386) |
| EE0094 | vm-only | D38 | VM only (T046960) |
| EE0095 | both | D54 | VM; JS K (E) (T039390) |
| EE0096 | both | D39 | both P (T039377) |
| EE0097 | delegates | D51, D63 | VM; JS K (E) (T050497) |
| EE0098 | both | D52, D63 | VM; JS K (E) (T030428) (via the parse-negative table) |
| EE0099 | both | D54 | VM; JS K (E) (T050727) |
| EE0100 | both | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0101 | both | D38 | both P (T050703) |
| EE0102 | both | B08 | VM; JS K (E) (T030388) (via the parse-negative table) |
| EE0103 | both | B08 | VM; JS K (E) (T030388) (via the parse-negative table) |
| EE0104 | both | B09 | VM; JS K (E) (T030387) (via the parse-negative table) |
| EE0105 | both | B09 | VM; JS K (E) (T030387) (via the parse-negative table) |
| EE0106 | both | B09 | VM; JS K (E) (T030387) (via the parse-negative table) |
| EE0107 | vm-only | D38 | VM only (T035157); both P (T029483) (via the parse-negative table) |
| EE0108 | vm-only | D38 | both P (T030370); VM only (T039300) (via the parse-negative table) |
| EE0109 | both | D54 | VM; JS K (E) (T031502) |
| EE0110 | both | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0111 | delegates | D51, D63 | VM; JS K (E) (T042556) |
| EE0112 | both | D52, D63 | VM; JS K (E) (T030907) |
| EE0113 | both | D54 | VM; JS K (E) (T042546) |
| EE0114 | both | D39 | both P (T030912) |
| EE0115 | both | D38 | both P (T030917) |
| EE0116 | both | D38 | both P (T030913) |
| EE0117 | both | B08 | VM; JS K (E) (T030916) |
| EE0118 | both | B08 | VM; JS K (E) (T030909) |
| EE0119 | both | B09 | VM; JS K (E) (T030915) |
| EE0120 | both | B09 | VM; JS K (E) (T030908) |
| EE0121 | both | B09 | VM; JS K (E) (T046762) |
| EE0122 | both | D55, D63 | VM; JS K (E) (T046761) |
| EE0123 | both | D55, D63 | VM; JS K (E) (T046782) |
| EE0124 | both | B09, D63 | VM; JS K (E) (T046817) |
| EE0125 | both | D55, D63 | VM; JS K (E) (T046825); both P (T046826) |
| EE0126 | both | B09, D63 | VM; JS K (E) (T046837) |
| EE0127 | both | D55, D63 | VM; JS K (E) (T046836) |
| EE0128 | both | D55, D63 | VM; JS K (E) (T045922) |
| EE0129 | both | D55, D63 | VM; JS K (E) (T045924) |
| EE0130 | both | D-b adjacent, B22, B24, D39 | VM; JS K (E) (T045960) |
| EE0131 | both | B09 | VM; JS K (E) (T045961) |
| EE0132 | vm-only | D39, D63 | VM only (T046788) |
| EE0133 | both | D35 | both P (T047160) |
| EE0134 | both | D12-6, D35 | both P (T047161) |
| EE0135 | both | D53 | VM; JS K (E) (T047159) |
| EE0136 | both | B01 | VM; JS K (E) (T047164) |
| EE0137 | both | B02, D30 | VM; JS K (E) (T047165) |
| EE0138 | vm-only | D-b adjacent, B22, D39 | VM only (T047157) |
| EE0139 | both | B09 | VM; JS K (E) (T047163) |
| EE0140 | vm-only | D12-9, D-b adjacent, B05, D37 | VM only (T047158) |
| EE0141 | both | D54 | VM; JS K (E) (T043260) |
| EE0142 | both | B09 | VM; JS K (E) (T039302) |
| EE0143 | vm-only | D38 | VM only (T039300) |
| EE0144 | both | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0145 | both | D54 | VM; JS K (E) (T042490) |
| EE0146 | both | D38 | both P (T042492) |
| EE0147 | delegates | D51, D63 | VM; JS K (E) (T042498) |
| EE0148 | both | D52, D63 | VM; JS K (E) (T042495) |
| EE0149 | both | D39 | both P (T042500) |
| EE0150 | both | B08 | VM; JS K (E) (T042502) |
| EE0151 | both | B08 | VM; JS K (E) (T042497) |
| EE0152 | both | B09 | VM; JS K (E) (T042501) |
| EE0153 | both | B09 | VM; JS K (E) (T042496) |
| EE0154 | vm-only | D39 | both P (T030433); VM only (T030391) (via the parse-negative table) |
| EE0155 | both | D12-5, D37 | both P (T030409) |
| EE0156 | both | D38 | both P (T029483); VM only (T035157) (via the parse-negative table) |
| EE0157 | both | D38 | both P (T030385); both accept (T052876) |
| EE0158 | vm-only | D39 | VM only (T030391) |
| EE0159 | both | D54 | VM; JS K (E) (T030383) |
| EE0160 | both | D35 | VM only (T047347) |
| EE0161 | both | D12-6, D35 | both P (T028486); VM only (T028542) (via the parse-negative table) |
| EE0162 | both | D-b adjacent, B08, B09 | VM; JS K (E) (T040513) |
| EE0163 | both | D12-2, B10 | VM; JS K (E) (T040495) |
| EE0164 | both | D53 | VM; JS K (E) (T041630) (via the parse-negative table) |
| EE0165 | both | B01 | VM; JS K (E) (T042964) |
| EE0166 | both | B02, D30 | VM; JS K (E) (T047487) |
| EE0167 | both | D-b adjacent, B23, D63 | VM; JS K (E) (T040493) |
| EE0168 | both | D08, D12-6, D-b adjacent, B27, B28, B30, B34, D35 | VM; JS K (E) (T041635); both P (T041631) |
| EE0169 | both | D08, D12-6, D-b adjacent, B29, B31, B34, D35 | VM; JS K (E) (T042023); both P (T041643) |
| EE0170 | both | B32 | VM; JS K (E) (T041628) |
| EE0171 | both | B33 | VM; JS K (E) (T041638) |
| EE0172 | both | D-b adjacent, B08, B09 | both P (T041646) |
| EE0173 | both | D12-2, B10 | VM; JS K (E) (T041644) |
| EE0174 | both | D53 | VM; JS K (E) (T041630) |
| EE0175 | both | B01 | VM; JS K (E) (T041647) |
| EE0176 | both | B02, D30 | VM; JS K (E) (T041648) |
| EE0177 | both | D-b adjacent, B23, D63 | VM; JS K (E) (T042038) |
| EE0178 | both | H11, D56 | both P (T041724) |
| EE0179 | both | B27 | VM; JS K (E) (T040801) |
| EE0180 | both | D12-1 | both P (T041731) |
| EE0181 | both | D57 | both P (T041723) |
| EE0182 | both | B33 | VM; JS K (E) (T041636) (via the parse-negative table) |
| EE0183 | neither | B25 | both accept (T014788) |
| EE0184 | both | D-b adjacent, B25 | both P (T041444) |
| EE0185 | both | D-b adjacent, B25 | both accept (T013356) |
| EE0186 | vm-only | D-b adjacent, B25, R20 | VM only (T041393) |
| EE0187 | vm-only | D-b adjacent, B25, R20 | VM only (T041337) |
| EE0188 | vm-only | D-b adjacent, B25, R20 | VM only (T041363) |
| EE0189 | vm-only | D-b adjacent, B25, R20 | VM only (T041338) |
| EE0190 | vm-only | D-b adjacent, B25, R20 | VM only (T041332) |
| EE0191 | both | D-b adjacent, B25 | both P (T041440) |
| EE0192 | both | D-b adjacent, B25 | both P (T041501) |
| EE0193 | both | D-b adjacent, B25 | both P (T041500) |
| EE0194 | both | D-b adjacent, B25 | both accept (T013439) |
| EE0195 | both | D-b adjacent, B25 | both P (T013890) (via the parse-negative table) |
| EE0196 | both | D-b adjacent, B25 | both accept (T013461) |
| EE0197 | both | D-b adjacent, B25 | both P (T041445) (via the parse-negative table) |
| EE0198 | both | D-b adjacent, B25 | both P (T041465) |
| EE0199 | both | D-b adjacent, B25 | both P (T041471) |
| EE0200 | both | D-b adjacent, B25 | both accept (T013859) |
| EE0201 | both | D-b adjacent, B25 | both P (T014482) |
| EE0202 | both | D-b adjacent, B25 | both P (T014415) |
| EE0203 | both | D-b adjacent, B25 | both P (T014433) |
| EE0204 | both | D-b adjacent, B25 | both P (T014346) |
| EE0205 | both | D-b adjacent, B25 | both P (T014345) |
| EE0206 | both | D-b adjacent, B25 | both P (T014344) |
| EE0207 | both | D-b adjacent, B25 | both accept (T052242) |
| EE0208 | vm-only | D-b adjacent, B25 | no input |
| EE0209 | both | D-b adjacent, B25 | both P (T041420) |
| EE0210 | both | D-b adjacent, B25 | both P (T041500); both accept (T001062) |
| EE0211 | both | D-b adjacent, B25 | both accept (T013439) |
| EE0212 | both | D-b adjacent, B25 | both P (T013890) (via the parse-negative table) |
| EE0213 | both | D-b adjacent, B25 | both accept (T013461) |
| EA0001 | both | R03 | both accept (T028989) |
| EA0002 | both | D12-2, B10, R22 | both accept (T028981) |
| EA0003 | both | D-b adjacent, B08, R23 | both accept (T029007) |
| EA0004 | both | B09, R24 | both accept (T029001) |
| EA0005 | both | N14, D08, D-b adjacent, B24, R25 | both accept (T045894) |
| EA0006 | both | R28 | both accept (T029030) |
| EA0007 | both | R28 | both accept (T029032) |
| EA0008 | neither | no parse-time site (eval-time; neither engine refuses: CAT `neither`) | both accept (T000558) |
| EA0009 | both | D-b adjacent, B23, D63, R27 | both accept (T046398) |
| EA0010 | both | D61 | both accept (T000031) |
| EA0011 | both | D61 | both accept (T006715) |
| EA0012 | both | D61 | both accept (T006548) |
| EG0001 | both | D58 | both P (T041622) |
| EG0002 | both | D-b adjacent, B25 | both P (T041495) |
| EG0003 | both | D36, D63 | VM; JS K (E) (T050831); both accept (T001080) |
| EG0004 | both | D35, D63 | VM; JS K (E) (T028528) |
| EG0005 | both | D36, D63 | VM; JS K (E) (T050800) |
| EG0006 | both | D50 | both accept (T001086) |
| EG0007 | both | D59, D63 | VM; JS K (E) (T001074) |
| EG0008 | both | B07, B18, B21, D39, D63 | VM only (T030073); VM; JS K (E) (T030070) |
| EP0001 | both | D39 | both P (T030365); VM only (T049531); VM; JS K (E) (T040451) (via the parse-negative table) |
| EP0002 | both | D50 | both P (T028486) (via the parse-negative table) |
| EP0003 | both | D37 | both P (T028331); VM only (T034997); VM; JS K (E) (T050973) (via the parse-negative table) |
| EP0004 | both | D39 | both P (T030365); VM only (T049531); VM; JS K (E) (T040451) (via the parse-negative table) |
| EP0005 | both | D50 | both P (T028486) (via the parse-negative table) |
| EP0006 | vm-only | D-b adjacent, B14 | VM only (T042907); both P (T051383) |
| EP0007 | vm-only | D-b adjacent, B14 | VM only (T042908); both P (T051384) |
| EP0008 | both | D-b adjacent, B14 | both P (T051386); both accept (T051385) |
| EP0009 | vm-only | T22, P19, D18-7, R09 | VM only (T043434) |
| EP0010 | vm-only | P20, D18-7, R09 | VM only (T046886); both accept (T045905) |
| EP0011 | both | P06b, D18-7, R09 | both accept (T042051); VM only (T053475) |
| EP0012 | vm-only | P06a, P06b, D18-7, R09 | VM only (T036958); both P (T030096) |
| EP0013 | both | P06b, D18-7, R09 | both P (T040993) |
| EP0014 | vm-only | P06a, P06b, D18-7, R09 | VM only (T036956); both P (T030095) |
| EP0015 | both | D62 | both accept (T015674) |
| EP0016 | both | D12-2, B10 | VM; JS K (E) (T040494) (via the parse-negative table) |
| EP0017 | both | D-b adjacent, B08 | VM; JS K (E) (T030388) (via the parse-negative table) |
| EP0018 | both | B09 | VM; JS K (E) (T030387) (via the parse-negative table) |
| EX0001 | both | D43 | both P (T051983) |
| EX0002 | both | D43 | both P (T028581) |
| EX0003 | both | D43 | both P (T041214) |
| EX0004 | both | D43 | both P (T041059) |
| EX0005 | both | D43 | both P (T041558) |
| EX0006 | both | D43 | both P (T041055) |
| EX0007 | both | D43 | no input |
| EX0008 | both | D12-5, D12-7, D12-8, D12-9, D12-10, D-b adjacent, B05, B19, D36, D37, D60 | both P (T028343) |
| EX0009 | both | D12-5, D-b adjacent, B12, D37 | both P (T028331) |
| EX0010 | both | D60 | both P (T040489) |
| EX0011 | vm-only | D18-6 | VM only (T050099) |
| EX0012 | vm-only | D18-6 | no input |
| EX0013 | vm-only | D-b adjacent, R34 | no input |
| EX0014 | vm-only | no parse-time site (VM lowering, 2301; R34) | no input |
| EX0015 | vm-only | no parse-time site (VM lowering, 2302; R34) | VM only (T052739) |
| EX0016 | vm-only | no parse-time site (VM verifier; R34) | no input |
| EX0017 | vm-only | T22, D09, D18-1, R11 | no input |
| EX0018 | vm-only | no parse-time site (VM run time; engine row) | both accept (T047464) |
| EX0019 | vm-only | no parse-time site (VM run time; engine row) | both accept (T040530) |
| EX0020 | vm-only | D-b adjacent | no input |
| EX0021 | both | P05b | VM only (T040828); both P (T041737) |
| EX0022 | vm-only | no parse-time site (VM lowering, 2406; P04) | VM only (T040810) |
| EX0023 | both | D12-9, D12-10, D18-4, D37, R04 | JS only (T042120) |
| EX0024 | both | D-b adjacent | no input |
| EX0025 | both | D39 | VM only (T040667) |
| EX0026 | vm-only | D-b adjacent, B25, R21 | no input |
| EX0027 | vm-only | R34 | no input |
| EX0028 | vm-only | D09, D12-3, D12-4, D18-2, D18-3, R11 | no input |
| EX0029 | vm-only | H11, D18-5, D56 | no input |
| EX0030 | vm-only | D-b adjacent | no input |
| EX0031 | both | D-b adjacent, B23, D63 | both accept (T006742) |
| EX0032 | both | D-b adjacent, B03 | both P (T040496) |
| EX0033 | both | P24, D-b adjacent, B04 | VM only (T047162) |

