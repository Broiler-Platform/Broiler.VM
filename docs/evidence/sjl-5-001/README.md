<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-5-001 - the text area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-5 of the shared JavaScript libraries programme,
[`sjl-5-text-algorithms.md`](../../javascript-shared-libraries/milestones/sjl-5-text-algorithms.md). **Rule:**
[`decision-rule.md`](decision-rule.md), written at SJL-5.1 before any code of the milestone existed.
**Records:** [JSD-0070](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md). **Owner and reviewer:** one person holds every
role (EX-30), and nobody independent has read this bundle.

**State:** collected, 2026-10-15, at SJL-5.6. **Parts A, B and C are MET** under
[`decision-rule.md`](decision-rule.md) as written, with no revision (sections 5 to 7). Every row the
test262 comparisons changed is a wall-clock exhaustion at the base whose re-runs alone agree at both
commits, and no row needed class C1, C3 or (e). The vectors, the closures, the rules, the packages and
the simplification check are sections 8, 10 and 11, section 9 holds the exclusions, and section 12 holds
the Broiler.VM clauses, each met. *(Revised 2026-10-15, later the same day: the exclusions moved to
section 9, the one section rule H2 reads a definition from, ahead of the closures and the simplification
check, which were sections 9 and 10; their content is unchanged. The sentence read "The vectors, the
closures, the rules, the packages and the simplification check are sections 8 to 10, and section 12
holds the Broiler.VM clauses, each met.")* *(Revised 2026-10-15, at SJL-5.6. The paragraph read: "**State:** collection in progress. Nothing here is
a verdict until its section says so. **Taken:** the base (section 1) and the first engine-level runs on
the text area (section 2). The after runs, their comparisons and the rest of SJL-5.6 are added when
SJL-5.4's cut-over has landed; [`collect.py`](collect.py) gains their steps then.")*

## 1. The base

| | |
|---|---|
| Commit | `e600da2`, a clean checkout ([`base/identity.txt`](base/identity.txt)): the rule's commit `acf2bec` and this bundle's collector, which is an evidence script. Outside the documents, it differs from the default branch after SJL-4's merge (`1f253ff`) only by `.gitattributes`, part B's six probes and part C's thirty documents with their README, which the identity lists |
| Taken | 2026-10-14, 16:48Z to 17:33Z, by `collect.py --only base-identity --only base-test262 --only base-twins --only base-json-modules`, before any code of SJL-5 landed ([`base/collect.log`](base/collect.log)). Nothing else built, tested or generated on the machine meanwhile: the session edited files and ran git, and the Broiler.JS base ran on GitHub's runners |
| Part A | test262 at the pin in both forms, through bundle `sjl-0-001`'s procedure: [`base/wide-bytecode.report.gz`](base/wide-bytecode.report.gz) and [`base/wide-native.report.gz`](base/wide-native.report.gz), each with its run log, merge log, floor log, JSON report and failure manifest. Bytecode: 101,723 variants, pass 100,372, fail 154, exhausted 44, skipped 1,153. Native: pass 100,336, fail 188, exhausted 46, skipped 1,153. The bytecode floor holds; the native run's exit code 4 is the floor's re-basing note, because the floor was set in the win64 native form and this run is sysv, as in SJL-3's and SJL-4's runs |
| Part B | [`base/fuel-parity.log`](base/fuel-parity.log): each of the ten wide twins and the six text probes, its smallest completing allowance bisected under the bytecode form, then each form at that figure (completes, with its answer) and at one less (refused on fuel). Both forms agree at every figure |
| Part C | [`base/json-modules-1.txt`](base/json-modules-1.txt) and [`base/json-modules-2.txt`](base/json-modules-2.txt): the harness run twice over the thirty documents of `src/tests/forms/text/json-modules` and the twelve `.json` files of the pinned suite's `test/`, each document's outcome, artifact hash and first diagnostic's code. The two runs are byte-identical: 25 documents compiled, 17 refused, 14 of them `InvalidJsonModule` and 3 `NestingTooDeep` |

## 2. Engine-level vectors: the first runs on the text area (SJL-5.2)

**The first runs are taken and not yet classified.** Corpus revision 10 (Broiler.JS `1ae7a54`) adds the
text area, and revision 11 (Broiler.JS `753d42e`) widens the schema's clause ids so that Broiler.JS's
validator reads the clause ids the text area cites; revision 11 changes no vector. This family's
pin stays at revision 9 until a revision classifies both engines' first runs. Each run used a scratch
pin that records the archive's digests ([`vectors/revision-10.pin`](vectors/revision-10.pin),
[`vectors/revision-11.pin`](vectors/revision-11.pin)); `git archive` of each commit, run twice, gave
byte-identical files. The archives are not committed, and `git archive` of the commit reproduces either
one.

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 10 | `1ae7a54`, 30 files, content `e4861fd6...` | 42,943 vectors: 42,938 match, 0 classified divergences, 5 unclassified differences | [`run-revision-10.log`](vectors/run-revision-10.log), [`run-revision-10.tsv.gz`](vectors/run-revision-10.tsv.gz) |
| Revision 11 | `753d42e`, 30 files, content `5c7aae56...` | the same, row for row | [`run-revision-11.log`](vectors/run-revision-11.log), [`run-revision-11.tsv.gz`](vectors/run-revision-11.tsv.gz) |

Both runs used the conformance root of the base commit (section 1), built in Release, and took place on
2026-10-14 after the base was complete. The five differences are the three rows of
[`correspondence.md`](correspondence.md) that JSD-0070 classes as corrections of this family:

| Vector | Expected | Broiler.VM | Row |
|---|---|---|---|
| `text/pad-order/0001`, `0003` | the filler's `toString` called once | not called: `RangeError` past the ceiling comes first | P1 |
| `text/pad-order/0002`, `0004` | `"x"`, for an empty filler | `RangeError` | P2 |
| `text/replace-order/0001` | `replaceAll`'s `toString` called once when nothing matches | not called | S1 |

No other row of the correspondence shows up as a difference here. *(Revised 2026-10-14, when the pin
moved. The paragraph went on: "The corpus revision that classifies both engines' first runs moves this
family's pin. The run at the moved pin, with its negative control, is added to this section then.")*

**Every vector of corpus revision 12 has a classified result in this family.** Revision 12 (Broiler.JS
`9ae67ee`) classifies both engines' first runs, and this family's pin
([`src/tests/vectors/pins/vectors.pin`](../../../src/tests/vectors/pins/vectors.pin)) moves to it. On the
pinned archive, with the same conformance root
([`vectors/run-revision-12.log`](vectors/run-revision-12.log),
[`vectors/run-revision-12.tsv.gz`](vectors/run-revision-12.tsv.gz)): **42,943 vectors, 42,938 match, 5
classified divergences, 0 unclassified differences.** The 5 are the `divergence.broiler-vm` fields revision
12 adds: P1's two (`vm-pad-filler-order`), P2's two (`vm-pad-empty-filler`) and S1's one
(`vm-replaceall-order`), each naming SJL-5.4. The negative control
([`vectors/negative-control-revision-12.log`](vectors/negative-control-revision-12.log)) changes the
expected value of `text/trim/0001` in memory, and the run reports that vector, and only it, as an
unclassified difference.

**What the runs say of JSD-0070.** Every difference either engine's first run found is one the
correspondence classified, so the record's first falsifier does not fire, and no new record is needed.
The vectors confirm every Broiler.VM cell the correspondence ran and refute one Broiler.JS cell: row J4
read that Broiler.JS refuses `JSON.parse("1e400")`, and its first run answers `Infinity`, as the
specification does. JSD-0070's "J2 to J4" sentence is not edited; Broiler.JS's plan records the
correction, and that engine's changelog will not list J4. Rows P4, X1 and X2 have no vector, and row
J5's vectors match on Linux, where both runs were taken.

## 3. The cut-over's vectors (SJL-5.4) and corpus revision 13

**After this family's cut-over (`6614fa9`, JSD-0071), every vector matches.** With the conformance root of
that commit, built in Release, on the archive pinned at revision 12
([`vectors/cut-over-revision-12.log`](vectors/cut-over-revision-12.log),
[`vectors/cut-over-revision-12.tsv.gz`](vectors/cut-over-revision-12.tsv.gz)): **42,943 vectors, 42,943
match, 0 classified divergences, 0 unclassified differences.** Each of the five vectors of section 2 that
carried a `divergence.broiler-vm` field reports "divergence no longer holds".

**Corpus revision 13 (Broiler.JS `bb1fc66`) retires the text area's divergence fields**, this family's five
and Broiler.JS's 42, which its own cut-over (SJL-5.5) corrects, and this family's pin moves to it. `git
archive` of the commit, run twice, gave byte-identical files, and the second, extracted afresh and hashed
there, gave the pin's content digest, which the harness's own reading agrees with. On the pinned archive,
with the same conformance root ([`vectors/run-revision-13.log`](vectors/run-revision-13.log),
[`vectors/run-revision-13.tsv.gz`](vectors/run-revision-13.tsv.gz)): **42,943 vectors, 42,943 match, 0
classified divergences, 0 unclassified differences.** The negative control
([`vectors/negative-control-revision-13.log`](vectors/negative-control-revision-13.log)) changes the
expected value of `text/trim/0001` in memory, and the run reports that vector, and only it, as an
unclassified difference.

*(Added 2026-10-15, after the runs. The package was published to nuget.org that day as
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.3`, from Broiler.JS `6e20998`, whose library code is
`0db9042`'s. The committed feed now holds the published package, and the checkout references it
(JSC-305). Every run of this bundle used the copy packed from `0db9042`. With the published package, the
same conformance root gives a report identical byte for byte to `vectors/run-revision-13.tsv.gz`, the
negative control again reports `text/trim/0001` alone, and the architecture suite passes 381 of 381 and
the contract suite 473 of 473.)*

## 4. The after arm

| | |
|---|---|
| Commit | `72ad941`, a clean checkout ([`after/identity.txt`](after/identity.txt)). Its product sources are the cut-over commit's, `6614fa9` (JSD-0071), but for the 11 files outside the documents the identity lists: the published package on the committed feed with its pin, the allow-list, both lock files, the feed's README and the notices row (JSC-305), and this family's pin of the shared corpus at revision 13 with its archive and README (section 3). The published package's library code is the one `6614fa9` was built with |
| Rule and harness | [`decision-rule.md`](decision-rule.md) and [`json-modules.cs`](json-modules.cs), each with the same SHA-256 at the base and after (both identities) |
| Collector | [`collect.py`](collect.py) at `72ad941`, whose after steps reuse bundle `sjl-4-001`'s collector, and through it `sjl-3-001`'s and `sjl-0-001`'s |
| Taken | 2026-10-15, 07:11Z to 08:20Z, step by step - `after-identity`, `after-test262`, `after-twins`, `after-json-modules`, `compare`, `rerun`, `wall-race`, `vectors`, `closures`, `rules`, `simplification` - on an otherwise idle machine, into [`after/`](after/) and [`vectors/`](vectors/). Then, from 08:20Z, `eng/collect-js-packages.py` at the same commit into [`after/packages/`](after/packages/) (section 10). Broiler.JS's after run took place on GitHub's runners meanwhile, not on this machine |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, four jobs, the native backend `x86-64-sysv` |
| Base side of the re-runs | a clean checkout of the base commit `e600da2`, whose conformance root has the SHA-256 the base's runs record ([`base/test262-binary.txt`](base/test262-binary.txt)) |

## 5. Test262, both forms (rule part A)

**MET.** All four runs are retainable as whole runs, and every changed verdict is in class C4 (c'),
listed below with its re-runs, which agree at both commits.

| Run | Runner | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds ([`after/wide-bytecode.run.log`](after/wide-bytecode.run.log)) | pass 100,372, fail 154, exhausted 44, skipped 1,153 | the same | 19 variants differ, all (c') ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base: the floor was set in the win64 native form | pass 100,338, fail 188, exhausted 44, skipped 1,153 | pass 100,336, fail 188, exhausted 46, skipped 1,153 | 3 variants differ, all (c') ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

Neither exit is 2 or 3. **The changed rows and their re-runs**
([`after/rerun-wide-bytecode.tsv`](after/rerun-wide-bytecode.tsv),
[`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their logs and every report): each
side was one process of the conformance root over the rows' paths, alone, at `e600da2` and at `72ad941`.

| Rows | Pair | Base -> after | Re-run at the base, after | Class |
|---|---|---|---|---|
| `decodeURI/S15.1.3.1_A2.5_T1.js` [sloppy], [strict]; `decodeURIComponent/S15.1.3.2_A2.5_T1.js` [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-1-of-8.js` to `-8-of-8.js`, each [sloppy] and [strict] (16 rows) | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js` [strict] | native | Exhausted/WallClock -> Exhausted/NestedLoadFanOut | Exhausted/NestedLoadFanOut, the same | **C4 (c')**, the re-runs agree |
| `staging/sm/TypedArray/element-setting-converts-using-ToNumber.js` [sloppy], [strict] | native | Exhausted/WallClock -> Passed | Passed, Passed | **C4 (c')**, the re-runs agree |

**What the rows say.** At the base each row reached the wall clock in the whole run, and alone, at the
same commit, it gave the after commit's verdict, so what differs is the base's whole run and not the
base's engine. `decodeURI/S15.1.3.1_A2.5_T1.js` [strict] is the row bundle `sjl-4-001` saw racing
between the two allowances in its native pair, and this area moved its code: here, in the bytecode pair,
both commits exhaust fuel when it runs alone, which is what this family's unchanged charging units
promise. **No row needed class (e)**, so the `wall-race`
step had nothing to run, and none needed C1 or C3: no test262 row changed its verdict through P1, P2 or
S1, whose cases the vectors carry (section 3).

**The exhaustion verdicts** clause 7 requires: every row exhausted on fuel at the base is exhausted on
fuel after, and every row that completes at the base completes after. The 22 rows above were exhausted
on the wall clock at the base, and their re-runs give each commit the same verdict.

## 6. The fuel-parity twins and probes (rule part B)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log): every twin of `src/tests/forms/wide` and every
probe of `src/tests/forms/text` that [`base/fuel-parity.log`](base/fuel-parity.log) bisected - ten twins
and six probes, each in the bytecode form and in the native form under `x86-64-sysv`, 32 rows - completes
at its base figure with the base's answer and refuses with `AllowanceExhausted on Fuel` at one less.
**No charge moved**, inside the area or outside it, as JSD-0070 decision 3 requires.

## 7. The JSON modules' artifacts (rule part C)

**MET.** [`after/json-modules-1.txt`](after/json-modules-1.txt) and
[`after/json-modules-2.txt`](after/json-modules-2.txt): the harness twice at `72ad941`, over the same
42 documents as the base. [`after/json-modules-compare.txt`](after/json-modules-compare.txt) sets them
beside the base's: each commit's two runs agree on every document, and **every document gives the same
outcome, artifact hash and diagnostic code at the base and after** - 25 compiled with byte-identical
artifacts, 17 refused with the same codes. No row needed class C1: the shared reader admits and refuses
what `JsJsonModule`'s reader did on this population, and the writer, which stays in this family, writes
the same bytes.

## 8. Engine-level vectors at the after commit

**Every vector passes.** The conformance root of `72ad941` over this family's pin, corpus revision 13:
**42,943 vectors, 42,943 match, 0 classified divergences, 0 unclassified differences**
([`vectors/after-revision-13.log`](vectors/after-revision-13.log)); its report is byte-identical to
section 3's `vectors/run-revision-13.tsv.gz`. The negative control changes the expected value of
`text/trim/0001` in memory, and the run reports that vector, and only it, as an unclassified difference
([`vectors/after-negative-control-revision-13.log`](vectors/after-negative-control-revision-13.log)).

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-128 | Open | **The text code that left this component's assurance coverage.** The 44 members rule I5 registers for SJL-5.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), and the inline readings, template walks and normalisations JSD-0071 section 3 names, left the JavaScript family's sources at SJL-5.4. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Text`, which the home's own assurance component covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters stay in this family and stay covered. EX-125 and EX-127 say the same of the earlier areas |

## 10. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, and each
closure declares the algorithms assembly.** [`after/closures/`](after/closures/) holds, for
ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, the closure under the JIT, trimming
and Native AOT, the catalog each mode printed, and the transcripts of every publish and run. Each JIT
and trimmed closure lists `Broiler.ECMAScript.Algorithms`, and each Native AOT publish leaves no managed
assembly beside the image. All 646 of the slice compiler's checks pass in every mode, with three not run
on this machine and claimed by nothing, as before. The Android root is still exclusion EX-126.

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the admitted package the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.3`, its content hash as the allow-list pins it and as the committed feed's copy has it, and the commit its nuspec names, Broiler.JS `6e20998`: the package as published |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed; rule I3 read them in the rules run, which placed them in the tree for that run alone |

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture
suite, built and run as the lane runs it, at `72ad941`: **381 tests, every one passed**, rules I1, I2,
I3, I5, I6 and I7 among them; I3 read the five image records named in the log's first line. Rule I4 is
SJL-7's, so "I1 to I7" is six rules at this milestone. **Rule I5 holds the deletion list**: its register
holds the 44 members SJL-5.4 deleted, beside SJL-3.4's 34 and SJL-4.4's 44, and it passes.

**The packages.** [`after/packages/`](after/packages/) is `eng/collect-js-packages.py` at `72ad941`,
under its own predeclared decision (`manifest.txt`): the candidate pack of the whole solution with every
package's metadata and file list, a pristine JavaScript consumer that restores and runs with upstream
feeds unreachable from a local feed holding the admitted shared package, the rollback to the published
`0.1.0-preview.5` set and forward again, the negative control, and the consumer as Native AOT. **Every
step is as expected**: the consumer ran in each of its three runs, the negative control failed to
restore, and the Native AOT consumer ran. Its nuspecs that declare a shared dependency declare
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.3`, which rule N37 admits. It does not replace the profile's
package baseline, bundle JS-10-006, which rule N37 still holds (JSD-0071 section 1).

## 11. The simplification check

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log)
searches the simple name of every member rule I5 registers - 122, SJL-3.4's, SJL-4.4's and SJL-5.4's -
as a declaration in the JavaScript family's sources. **All 44 of SJL-5.4's are declared nowhere.** 106 of
the 122 are declared nowhere; the other 16 are the earlier areas' names that bundle `sjl-4-001` section 8
read, matching other members of the same simple name in other types, or the adapters that keep a deleted
member's name. Rule I5 decides by the fully qualified name, and it passes (section 10).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's product sources for the constructs that would mean a surviving copy of the area:
UTF-16 read or written by the platform or by the surrogate constants, padding by the platform, base64,
hex and URI by the platform, JSON by `System.Text.Json`, a `\uXXXX` escape written by format string,
and the line terminators and byte order mark spelled as code. **117 hits; none is a copy the area's
deletion list or JSD-0070's X6 names**:

| Hits | Where | What they are |
|---|---|---|
| 34 | `JsRegExpMatcher` (`.Format`) | the matcher's own readers, which stay: `.Format` admits no shared package (SJL-11; JSD-0071 section 3) |
| 10 | `JsRealm.RegExp` | `RegExp.escape`'s code-point read and its escapes, `source`'s escaping of the line terminators, and the `u`-flag back-up in `RegExpMatchOne`, which the correspondence names under AdvanceStringIndex and leaves to SJL-11 |
| 18 | `JsCollationData`, `JsCollator`, `JsUnicodeCasing`, `JsSegmenter`, `JsNumberFormatter`, `JsIntlTables`, `JsUnicodeStringProperties` | code-point reads and writes in collation, case mapping, segmentation and Intl, the areas of SJL-7, SJL-9 and SJL-17 |
| 9 | `JsParser`, `SliceTokenizer`, `JsUnicodeLexical` | the compiler's lexers and parser: the module early error on a lone surrogate (SJL-15), the tokenizer's code-point read and escapes, and the lexical `WhiteSpace` predicate (SJL-13); the correspondence names each |
| 6 | `JsRealm.String` | writing a code point as UTF-16 - `fromCodePoint`, its comment, `normalize`'s output - which the library does not do: its `Utf16` members read code points |
| 3 | `JsFormat` | the artifact format's UTF-8 text encoding, in `.Format`, which admits no shared package |
| 1 | `JsJsonModule` | the JSON module translation's writer, which stays in this family (JSD-0070) |
| 2 | `JsRealm.DurationFormat`, `JsTemporalCore` | zero-padded digits of a number in Intl's and Temporal's output, not `StringPad` (SJL-9, SJL-10) |
| 1 | `JsRealm.Function` | the `Function` constructor's test of a parameter name's first character |
| 1 | `Broiler.VM.Runtime` | the runtime's own descriptor-name validation, which is not JavaScript |
| 32 | the composition roots and harnesses | hashes written in hexadecimal, their reports' JSON, padded columns and `\u` escapes in their own output, and the WebAssembly harness's script text |

**Two findings for later milestones.** The correspondence's CodePointAt row names
`JsUnicodeCasing.NextCodePoint` and `PreviousCodePoint` and `SliceTokenizer.CodePointAt` among this
family's copies, and JSD-0070's X6, which lists what SJL-5.4 deletes, does not: each is a hand-written
pair read that `Utf16.CodePointAt` could serve. They stay, and this bundle names them for the milestones
that own their code, SJL-7 (case mapping) and SJL-13 (the lexer). No clause of SJL-5 reads the duplicate
search.

## 12. The Broiler.VM clauses

| Clause | What it asks | Where | State |
|---|---|---|---|
| 6 | Rules I1 to I7 hold, and rule I5 holds the deletion list | section 10, [`after/rules.log`](after/rules.log) | **Met** at `72ad941`: 381 of 381, I5 among them |
| 7 | test262 in both forms meets the area rule, and the fuel twins agree | sections 5 and 6 | **Met**: part A MET in both pairs, every change a (c') row whose re-runs agree; part B MET; the exhaustion verdicts hold |
| 8 | Every root is published and run in three modes | section 10 | **Met** for every root this machine can publish, at `72ad941`; Android is EX-126 |
| 9 | Compile-time JSON modules produce byte-identical artifacts for the retained module corpus, or every difference is classified | section 7 | **Met**: 42 of 42 documents identical, no difference |

Clauses 1 and 2, the home's, and 3 to 5, Broiler.JS's, are judged in Broiler.JS's bundle of the same
name. None is accepted: review is deferred, and SJL-5 may not meet its gate before SJL-3 meets SJL-3's.
