<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-11-001's area rule: test262 parity in both forms on hosted runners, the compiler's literal refusals over the parse corpus, the fuel-parity twins, the adversarial corpus, and this family's measurement

**Written:** 2026-10-25, at milestone SJL-11 (work package SJL-11.1), with the area verdict
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
before any change it judges exists: no requirement of SJL-11.2 met, no admission of `Broiler.Regex`, no adapter and no
deletion. Committed in the same change: part D's programs ([`inputs/adversarial/`](inputs/adversarial/)) and part E's
workloads ([`inputs/workloads/`](inputs/workloads/README.md)). **Milestone judged:** SJL-11 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-11-regexp.md): its
Broiler.VM clauses 10 ("test262 in both forms meets the area rule"), 11 ("The fuel-parity twins and the exhaustion
verdicts meet the area rule") and 13 ("Compile-time literal refusals are unchanged, or every difference is
classified"), SJL-11.6's adversarial resource corpus in this engine, and this engine's own candidate-versus-control
measurement that SJL-11.1 names. **Owner:** the JavaScript profile owner, with the core architecture owner.
**Reviewer:** none. Every role this rule names is held by one person (EX-30), and this rule does not claim that anyone
independent has read it.

*Revised 2026-10-25, the day it was written, after review, before any run and before this change reached the default
branch. The header and part D state the command-line root's line as it prints it, on standard error behind the
prefix "broiler-js: <path>: ", where they read "the command-line root prints the same line and exits 7" and "exit 5
with "AllowanceExhausted on Fuel"" and "exit 7 with exactly the line "the invocation answered
ProfileFault/AllowanceExhausted and carried no payload""; part D retains both streams of each process, where it read
"its exit code and its output retained". Part A's comparison reads the details of every variant failing at both
commits, which the comparator does not report, where it read them only "for every row the comparison reports".
Nothing else changed.*

**What this file is.** Five predeclared decisions, one per part.
- **The templates.** Part A is template T1 of
  [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
  part C T7, part E T5. Part B is an identity of refusals over the parse corpus, read as T6 is read. Part D is
  resource accounting over a declared corpus (layer L14), with validation section 7's control for SJL-11.
- **Each part decides alone**, and no part reads another's verdict.
- **Part E is the only measurement.** Its runs are retained in a sub-bundle of their own, `sjl-11-001/measurement/`,
  with its own README and manifest (the profile's roadmap.gates.md section 17, rule 8). It is not a gate clause of
  SJL-11; JSD-0081 decision D19 says how its verdict is recorded.
- **Other rules.** Broiler.JS and Broiler.RegEx commit their own rules. Neither is read here.
- **The correspondence this rule classifies by** is [`correspondence.md`](correspondence.md) beside it, with its runs
  in [`correspondence-runs/`](correspondence-runs/). Its rows are named here by their letters and numbers, as
  JSD-0081 names them: S (syntax and early errors), M (matching), B (budget and limits), F (flags), T (text helpers)
  and U (data).
- **It holds no figure.** Where a count or an allowance matters, it names the file that fixes it.

**How this family reports a stop, read before any run** (the correspondence's section B, rows B1 and B6).
- **How the command-line root writes a result.** A program that completes has its completion value written to
  standard output. A program that does not complete has one line written to standard error: the prefix
  `broiler-js: `, the program's path as it was passed (any backslash written as a slash), `: `, and the run's detail
  (`src/compositions/Broiler.VM.Composition.JavaScript.Cli/Program.cs`, `Report`). The detail is the text after that
  prefix, and this rule reads it so. The correspondence's `correspondence-runs/vm/cli/` retains such lines.
- **A fuel exhaustion** is reported by the conformance root as the verdict `Exhausted` with the dimension `Fuel`, and
  by the command-line root with exit code 5 and the detail "the program did not settle within its allowance:
  AllowanceExhausted on Fuel".
- **A stop at a matcher ceiling**, the frame ceiling `1 << 20` or the trail ceiling `1 << 21`, which JSD-0081's
  decision D10 keeps and passes to the new backend, is not recorded by any budget dimension. `RegExpRun` turns it into
  `JsAbort(Exhausted)`, which `JsExecution` reports as `ProfileFault/AllowanceExhausted` with no payload. So the
  conformance root reports the variant `Failed`, with the detail "the invocation answered
  ProfileFault/AllowanceExhausted and carried no payload", and the command-line root exits 7, the code its help calls
  "host defect", with the same detail behind its prefix. **Neither root names which ceiling stopped.**
- **This rule reads that detail, and only that detail, as a stop at a matcher ceiling**: a verdict of its own,
  neither a wrong answer nor an abnormal end, and the same at both commits, because the adapter maps Broiler.Regex's
  `LimitExceeded` for frames or trail to the same `JsAbort(Exhausted)` (decision D10). Every other `Failed` detail and
  every exit 7 with another detail keeps its own reading.

**What earlier bundles taught this rule, before any run of SJL-11.**
- **Part A follows bundle `sjl-10-001`'s part A.** The local machine for its re-runs is the one bundles `sjl-5-001` to
  `sjl-8-001`, `sjl-10-001` and `sjl-12-001` were taken on, named in the bundle by its processor count, its SDK and its
  operating system, with no other build, test or run on it while a run of this bundle is taken.
- **This family's units move.** Broiler.Regex's new backend charges in this family's units (JSD-0081 decision D8),
  but its program is not this matcher's, so an exhaustion verdict may move. **So may a ceiling stop**: this matcher
  runs a greedy quantifier over a class as one `Run` instruction, but over a single character or a group as a loop
  with a frame per iteration (row B1), and a backend that runs `a*` as one instruction turns B1's stop into an answer
  (decision D37, the third presumption, which holds for classes only). Part A admits both under C2 alone, on variants
  that reach a regular expression, and only if the variant passes after, at ten times the allowance where it does not
  pass at the part's.
- **The twins reach none of the area's code.** No `*.small.js` twin in `src/tests/forms/wide` constructs or runs a
  regular expression, so part C admits nothing, as bundle `sjl-10-001`'s part C admitted nothing.
- **Part D's control works with the meter as it is.** The K, A, L and W cases exhaust through the realm's fuel, which
  the adapter passes to Broiler.Regex as its meter (decisions D8 and D9): the realm's charge throws
  `JsAbort(Exhausted)` itself (`JsEngine.Metering.cs`, `ChargeOnce`), and no status of Broiler.Regex's answers them.
  Only the M cases end on a status the adapter maps. So the control that patches that mapping runs over the M cases
  alone, and a second control, on a copy of the report, shows the comparison reporting the other classes.
- **Part E follows bundle `sjl-7-001`'s part C**: each workload answers with the count of the operations that
  completed, which no correction changes.
- **Part B reads bundle `sjl-12-001`'s parse corpus** with its tool, restricted to what the compiler's literal
  validation decides. Since SJL-12.6 item 2 that validation is the binder's `CheckRegExp`
  (`JsBinder.EarlyErrors.cs`), which SJL-11.4 re-points (decision D18).

**The commits, named before they exist.**
- **The rule commit** carries this file.
- **The admission commit** is the first commit of the branch that lands SJL-11.4. Its parent is the default branch's
  head when the branch is made. It adds one entry, `regexp-duplicate-named-groups`, to `SuiteFeatures.cs`'s
  `AdmittedProposals`, naming JSD-0081 (decision D15), and changes nothing else outside `docs/`. It descends from the
  rule commit and from bundle `sjl-12-001`'s corpus commit, `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`, which added
  `src/tests/corpus/parse-1/` and the slice compiler's `--parse-corpus` mode. **Every base run is taken at the
  admission commit.**
- **The after commit** is the default-branch merge that lands SJL-11.4. Its first parent must be the admission
  commit's parent. If it is not, the base runs are void, and a new admission commit is made and its bases taken
  before any after run is seen.

A part whose base was taken at any other commit is void and is not judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form, with the proposal regexp-duplicate-named-groups admitted at both commits by the admission commit's entry; test/built-ins/RegExp, test/language/literals/regexp, test/annexB and the String.prototype methods that take a regular expression are in it, and a regression on any other path fails this part too
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to this file's commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the admission commit and the after runs at the after commit, both named in bundle sjl-11-001; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log); and every file whose front matter lists regexp-duplicate-named-groups is reported under its variants, not as a skipped row of variant -, in all four runs
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite; then, by a script committed to the bundle before the base run is taken, which also writes the part's table, the detail each report's detail column gives at both commits, read for every row the comparison reports and for every variant whose verdict is Failed at both commits, which the comparison does not report, so that a stop at a matcher ceiling - the verdict Failed with the detail "the invocation answered ProfileFault/AllowanceExhausted and carried no payload" - is told from a wrong answer; a variant Failed at both commits whose detail is the ceiling stop's at one commit and any other at the other is a changed verdict, read under the classes below like any other; the comparison's count of variants present in only one report is zero, or the part is not judged
admitted difference classes:
  C1 correction, of two kinds JSD-0081 names before any run, each listed with its path, its variant and its kind: (D) a variant of a file whose front matter lists regexp-duplicate-named-groups, failing at the base because this family refused a group name declared twice (row S1), passing after (SJL-D-14 (i), SJL-D-16 (a)); (R) a variant failing at the base and passing after because of a correction of this family that a row of bundle sjl-11-001's correspondence.md names, listed with that row, the rows being S5 (\k read as a letter in a class beside a named group, accepted at the base), S10 (a four-digit lead-surrogate escape before a braced escape under u or v, refused at the base: test/staging/sm/RegExp/unicode-braced.js, unicode-lead-trail.js and unicode-class-lead-trail.js), M4 (the "index" and the substring of a non-empty match from a lastIndex inside a surrogate pair, which the base reports from the pair's start), M7 (a back-reference compared by code units under u or v: test/staging/sm/RegExp/unicode-back-reference.js) and T3 (a raw line terminator copied into source after a backslash: test/staging/sm/RegExp/escape.js); the correspondence names no test262 file for S5 or M4; a C1 row of neither kind is recorded in the bundle as a finding that the correspondence missed, and is a regression
  C2 charge model and ceilings by decision - a variant whose verdict moves into, out of or between exhausted dimensions, a stop at a matcher ceiling counted as one such dimension, in a test whose source contains a regular-expression literal or the identifier RegExp, because JSD-0081's dated decisions moved this family's regular-expression work onto Broiler.Regex's program: its charges (decision D8), or the frames and trail cells that program holds under the ceilings this family keeps (decisions D10 and D37); each listed with its verdict and dimension at both commits, a ceiling stop by its detail; a C2 row that does not pass after is run again alone at the after commit on the local machine this file's header names, one process of the conformance root over the row's path under the part's manifest and form, with the fuel allowance and the wall-clock allowance raised to ten times the part's, and is admitted if and only if that run passes; the ceilings are not allowances and the re-run does not raise them, so a row that stops at a ceiling after is not admitted; the re-run is retained; nowhere else
  C3 implementation-defined by decision - a variant whose verdict moves only because this family's answer to the empty match from a lastIndex inside a surrogate pair under u or v moves (row M4's empty match), which JSD-0081's decision D41 classes implementation-defined, and which is no correction, until the specification's text is corrected; listed with the row and the decision; nowhere else
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's and no other build, test or run on the machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no other C3 and no C5: no specification revision, test262 pin, data or policy moves at SJL-11.4 (this family's nesting limit stays 512, decision D7, and its frame and trail ceilings stay 1 << 20 and 1 << 21, decision D10), so a verdict that moves because one did is a regression
every other difference is a regression, including a variant that passes at the base and fails or is unsupported after, a variant exhausted or stopped at a ceiling after outside C2, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1, C2, C3 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-10-001`'s part A.
- The bundle retains both dispatches' artifacts, each comparison's output, and a table naming each C1 row with its
  kind and row, each C2 row with its verdict and dimension at both commits and its re-run, any C3 row, and each (c')
  and (e) row with its re-runs.
- **A stop at a matcher ceiling** is read only from the detail this file's header quotes. The comparator sees such a
  row as `Failed`: it reports a move between it and `Passed` or an exhausted dimension, and it reports no move
  between it and `Failed` with another detail, because it skips a variant whose verdict and dimension are the same at
  both commits (`eng/compare-test262-forms.py`'s loop over the shared variants). The table script reads that move
  from the two reports' detail columns, so a ceiling stop at the base that gives a wrong answer after, or the
  reverse, is a changed verdict the table must classify, and a row the table cannot read from both reports is a
  regression.
- **The files C1 (R) names are expected to move**, each from failing at the base to passing after. One that still
  fails after is no difference this part judges, and the bundle records it as a correction SJL-11.4 did not make.
- **Row S9b's file, `test/staging/sm/RegExp/7.8.5-01.js`, is no C1 row.** It fails at the base on this family's
  scanner, which is the front end's and which SJL-11.4 does not touch (JSD-0081, sub-area S's X2). A change of its
  verdict is a finding against JSD-0081 and a regression.
- **The files the base reports exhausted on a nested-load allowance** - `test/annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js`
  and `RegExp-trailing-escape-BMP.js`; `test/language/literals/regexp/S7.8.5_A1.1_T2.js`, `S7.8.5_A1.4_T2.js`,
  `S7.8.5_A2.1_T2.js` and `S7.8.5_A2.4_T2.js`; and `test/staging/sm/RegExp/unicode-class-braced.js`, as bundle
  `sjl-12-001`'s base reports them - are read as any row. No regular-expression code spends a load allowance, so a
  verdict that stays is no difference, and one that moves is read under the classes above: C2 if a regular-expression
  charge moved it into or out of another dimension, C4 (b) under `--exempt-guest-loads`, C4 (c') on the wall clock.
- The floor verdicts the harness prints do not decide part A. Each native run exits 4, which completeness admits. A
  row the comparison cannot classify is a regression even when the totals improve.

## Part B - the compiler's literal refusals over the parse corpus (clause 13)

```rule
evidence class: refusal identity (not a measurement)
population: every input of src/tests/corpus/parse-1/corpus.manifest as the admission commit holds it, in sub-population B1 (each input alone, under its goal and each of its variants, the wide manifest, the bytecode form) and sub-population B3 (each eval-family input compiled as eval code under each of the request-flag sets E0 to E7) of bundle sjl-12-001's decision-rule-area.md part B, with that rule's source names, module keys, variants, strict prologue and request flags; the test262 inputs read from src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz and each checked against the manifest's SHA-256, the others from src/tests/corpus/parse-1/inputs/
configuration: the slice compiler's --parse-corpus mode as the admission commit holds it, its source unchanged at the after commit, built in Release from a clean checkout on the local machine this file's header names; for each input, sub-population and variant, the outcome (accepted or refused), the ordered diagnostics (code, line, column) and, for an accepted input, the artifact's SHA-256 and length; run at the admission commit (the base) and at the after commit
completeness: every manifest input reported in B1 and B3 at both commits, the manifest's digest at each run equal to the admission commit's, and no run lost its process
controls: a copy of the base report with one refusal turned into an acceptance and one diagnostic's column changed, which the comparison must report as two unclassified differences; retained with its output
comparison: eng/compare-parse-corpus.py, as the admission commit holds it, joined on input, sub-population and variant: the outcome; for an input refused at both commits, the ordered diagnostic list; for an input accepted at both, the artifact hash
admitted difference classes:
  C1 (D) duplicate named groups - an input refused at the base whose first diagnostic is this family's regular-expression literal refusal, at a literal that declares one group name in two alternatives that cannot both participate, and accepted after (row S1); each listed with the literal
  C1 (R) a literal-validation correction of this family that a row of bundle sjl-11-001's correspondence.md names - an input whose outcome or first diagnostic moves at a regular-expression literal for that row's reason, the rows being S5 (a literal with \k in a class beside a named group, accepted at the base and refused after) and S10 (a literal with a four-digit lead-surrogate escape before a braced escape under u or v, refused at the base and accepted after); each listed with the literal and the row; a difference at a literal that no row explains is recorded as a finding and is a regression
no other class: the binder's CheckRegExp, which SJL-11.4 re-points onto Broiler.Regex's validate entry and the shared flag parser, keeps this family's diagnostic code and the literal token's span for every refusal (JSD-0081 decision D18), and literal validation emits no byte, so an outcome that moves outside C1, a diagnostic list that differs, and an accepted input whose hash differs are regressions
MET if and only if both runs are complete, the control is reported as stated, and every difference is in C1 and is listed.
NOT MET otherwise.
```

**How it is read.** Part B reads the compiler's validation, not the runtime's `RegExp` constructor, whose refusals are
part A's. The rows of this family's corrections that are run time (M4, M7 and T3) move no compile outcome, and a part
B difference at a literal for one of them is a finding. Hashes decide identity; the bytes are not retained. The
refused-input lists are retained at both commits.

## Part C - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine this file's header names; at the admission commit, each twin's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-11-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin, its base figure and one less
comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; no twin reaches a regular expression, so a moved figure is a defect of the cut-over outside the area, not a class
MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

## Part D - the adversarial corpus in this engine (resource accounting; SJL-11.6)

```rule
evidence class: resource accounting (not a measurement)
population: every case of docs/evidence/sjl-11-001/inputs/adversarial/manifest.tsv as committed with this rule, each a program generated by Broiler.RegEx's eng/adversarial-programs.py --engine broiler-vm from that repository's tests/Adversarial/corpus.tsv, both named in the manifest's header by commit and SHA-256; each case with its class - K catastrophic backtracking, A alternation without quantifiers, L look-around and lookbehind, G a greedy run past an old allowance, W exhaustion inside one alternative, N nesting, F a flat sequence of many terms, M frames and trail, V a large v-mode class expansion, O an ordinary pattern - its fuel allowance, and its declared outcome in this family: exhausted, an uncaught SyntaxError, or completed with a stated answer; in the bytecode form and in the native form under the backend the host arms
configuration: the command-line composition root built in Release, on the local machine this file's header names; each case one process, under its fuel allowance, a wall-clock allowance ample and the same at both commits, and every other allowance at the root's default; at the admission commit and at the after commit; for each process its exit code, its standard output (the answer) and its standard error (the verdict line) retained, each stream in a file of its own
reading: each process's verdict is read from its exit code and from its standard-error line, whose detail is the text after the prefix "broiler-js: <the program's path as passed>: " (this file's header), and its answer from its standard output - exit 0 with nothing on standard error, completed, its answer the completion value the root writes to standard output; exit 5 with exactly the detail "the program did not settle within its allowance: AllowanceExhausted on Fuel", exhausted on fuel; exit 5 with any other detail of the form "the program did not settle within its allowance: <reason> on <dimension>", exhausted on that dimension; exit 7 with exactly the detail "the invocation answered ProfileFault/AllowanceExhausted and carried no payload", stopped at a matcher ceiling, its ceiling not named (this file's header, JSD-0081 decision D10); exit 1 with the detail "uncaught " followed by the thrown error's name, alone or followed by ": " and its message, an uncaught error of that name; any other exit, an exit 1, 5 or 7 whose standard error holds no line of the form above, any other detail under exit 7, and a process that ended without an exit code of its own, an abnormal end; a case declared exhausted gives its declared outcome by ending exhausted on fuel or stopped at a matcher ceiling, never on the wall clock, which is ample so that it bounds a hung case
controls: K1, a patch to a scratch copy of the after commit, never merged, in which the adapter answers no match where Broiler.Regex reports LimitExceeded; run over the M cases, each of which must be reported as differing from its declared outcome; retained; the patch reverted and the revert checked. K2, a copy of the after report in which one case of each of the classes K, A, L and W is turned into a completion with the answer null, and one case declared to complete is given another answer, which the comparison must report as differing from their declared outcomes; retained with its output
comparison: per case and form, the verdict and answer after, read as above, against the declared outcome, and against the verdict and answer at the base, by a script committed to the bundle before the base run is taken
admitted difference classes:
  C1 (D) a case of class O declaring one group name in two alternatives, refused at the base with an uncaught SyntaxError (row S1) and completing after with its declared answer; listed
  C2 a case declared exhausted whose verdict differs between the base and after only in how it is exhausted - on fuel against stopped at a matcher ceiling - under JSD-0081's decisions D8 and D10; listed with both verdicts
no other class
MET if and only if, in both forms, after: every case declared exhausted ends exhausted on fuel or stopped at a matcher ceiling (never completing, never with null and never with a match), every case declared to throw ends with an uncaught SyntaxError, every case declared completing completes with its declared answer, and no process ends abnormally; every case's verdict and answer at the base equal those after, except the C1 and C2 rows listed; and both controls are reported as stated.
NOT MET otherwise.
```

**How it is read.** This is validation section 7's control for SJL-11, "a pattern whose budget runs out, which must
report exhaustion and not no match", made into a part, and SJL-11.6's "adversarial resource corpus" for this engine.
- **The margin.** Each declared allowance carries the corpus's margin: a case declared exhausted needs, by
  `regexpref`'s count of the specification's matcher steps, at least a hundred times its allowance's reach, and a case
  declared completing completes well inside it, so a change of units alone cannot move a declared outcome. The
  corpus's README states each case's margin and the four presumptions it rests on (JSD-0081 decision D37).
- **The G cases complete at both commits.** The corpus writes their runs as the class `[a]`, which this matcher runs
  as one `Run` instruction at the base, and the backend after by the third presumption. A G case that ends exhausted
  or stopped at a ceiling after has broken that presumption, and differs from its declared outcome.
- **The M cases end at a ceiling at both commits**, through this matcher's ceilings at the base and through the
  adapter's mapping of `LimitExceeded` after; the root names neither ceiling. They are the only cases control K1 can
  reach. Its patch makes each complete with the answer null, which differs from its declared outcome.
- **The K, A, L and W cases end on fuel at both commits**, through the realm's own charge (this file's header).
  Control K1 cannot reach them, so control K2 shows that the comparison reports each of those classes as differing
  when its case completes. A case of these classes that completes after, with null or with any answer, fails the MET
  line whatever the controls show.
- **The O cases that declare a name twice** end at the base with an uncaught `SyntaxError`, because this family
  refuses a name declared twice, and C1 (D) admits them.

## Part E - this family's measurement (template T5)

```rule
evidence class: measurement
population: the six workloads of docs/evidence/sjl-11-001/inputs/workloads as committed with this rule - scan.js (a global search for a literal across a long ASCII text), captures.js (exec with named groups and the d flag), replace.js (replace with a template and with a function), split.js, unicode.js (u and v property classes and case-insensitive matching) and backtrack.js (bounded backtracking with alternation and back-references) - each with its control, the *.control.js file beside it; each in the bytecode form and in the native form under the backend the host arms; the command-line composition root built in Release under JIT; the local machine this file's header names, and one RID, both named in the sub-bundle's manifest
arms: B the base, built from the admission commit; A the after commit; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload and form, in every repetition, B, A and A-again each run the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained, and warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; fuel and wall allowances ample and the same for every lane
condition: each lane's answer, checked before and after every lane, equals the workload's answer at the base arm; a lane whose answer differs fails the condition; each answer counts the operations that completed, so no correction changes it
configuration check: every timed child is run with --runtime, and its effective-configuration line is identical across arms, lanes and repetitions
quantity: Q(workload, arm, form) = median over retained repetitions of the workload's wall time
noise floor: N(workload, form) = |Q(workload, A, form) - Q(workload, A-again, form)|
comparison: D(workload, form) = Q(workload, A, form) - Q(workload, B, form)      (positive: the after arm slower), against N(workload, form); one engine's after arm against the same engine's base arm, never against another engine
attribution, retained beside D and deciding nothing: E(arm) = Q(workload, arm, form) - Q(control, arm, form)
admitted difference classes: none; a measurement admits no class, and a difference within the A/A floor is below resolution rather than admitted
MET if and only if all of:
  1. every lane's condition passes, before and after, for every workload and form;
  2. every effective-configuration line is identical across arms, lanes and repetitions;
  3. for every workload and form, D <= N.
NOT MET otherwise.
readings:
  - a workload and form whose |D| <= N is below resolution, and meets clause 3;
  - clause 3 has no tolerance but the A/A lane: this rule states no percentage;
  - a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped.
```

**What part E decides, and what it does not.** Whether this family's regular expressions are slower after the
cut-over than before, beyond the A/A floor, on this machine. JSD-0081 decision D19 records a NOT MET as the trade
`areas/regexp.md` section 6 names, "a slower but bounded matcher"; it is not a gate clause. It decides no speed claim,
no comparison with Broiler.JS, and nothing about another machine or RID. The workloads stay inside both ceilings and
the nesting limit, and none reaches a row of the correspondence on which the engines' answers differ
([`inputs/workloads/README.md`](inputs/workloads/README.md)).

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
and says it was written afterwards, and its decision is then weaker evidence. **Before SJL-11.4's code lands, a later
bundle shows this file unchanged.**

## What this file does not decide

- **The component clauses** (1 to 3): Broiler.RegEx's bundle `sjl-11-001` decides them, with the reference arms,
  whose class (b) holds row M4's empty match by JSD-0081's decision D41.
- **Broiler.JS's clauses** (4 to 7): Broiler.JS's rule decides them.
- **This family's other clauses**: 8 (rules I1 to I7), 9 (N10 and N18 revised with witnesses) and 12 (every root
  published and run in three modes), each by its own mechanism, the architecture tests and the release candidate's
  runs. The RegExp fuzz session's re-pointing is held by the collector's own controls.
- **The vectors.** SJL-11.3 adds them; this family's runs are retained in `vectors/`, and they may falsify JSD-0081.
- **Which matcher ceiling stopped a run.** Neither root names it, and no part reads it.
- **The differential cross-check** (template T8): its report over `src/tests/differential/the-regexp-*.js` is retained
  beside these parts without a rule, as bundle `sjl-3-001` retained its own (decision D38); T8 meets no conformance
  clause.
- **A speed claim of any kind.**
