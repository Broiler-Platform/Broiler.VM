<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-3-001 - the number-conversion pilot: Broiler.VM's cut-over and its evidence

**Milestone:** SJL-3 of the shared JavaScript libraries programme,
[`sjl-3-number-conversion-pilot.md`](../../javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md):
its Broiler.VM work packages SJL-3.4 (the cut-over), SJL-3.6 (the evidence) and SJL-3.7 (the
simplification check). **Rule:** [`decision-rule.md`](decision-rule.md), written at SJL-0.9 before any
code of the pilot existed and confirmed unchanged at SJL-3.2 (`rules-in-force.log`). **Records:**
[JSD-0066](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md),
on [JSD-0065](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0065-number-conversion-moves-to-the-shared-algorithms-library.md).
**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this
bundle.

**State:** collection in progress. The cut-over landed first, with the exclusions of section 9; each
run of SJL-3.6 is added to this bundle as it is taken, at the after commit of section 1, and this
README says which have been. Nothing here is a verdict until its section says so. **Taken:** the
identity, the closures, the packages (bundle JS-10-005), test262 in both forms with its comparison
and its (c') re-runs, the fuel-parity twins, the differential cross-check, the rules, the engine-level vectors, and the
simplification check.

## 1. Identity

| | |
|---|---|
| Base | Bundle [`sjl-0-001`](../sjl-0-001/README.md)'s base commit, before any engine changed |
| After | `49ea00e`, a clean checkout ([`identity.txt`](identity.txt)): the cut-over (`a8b3d0e`) and the repair of the pristine consumers' version (`49ea00e`), which changes no product source. Every run of this bundle is taken from it; the commits after it touch nothing outside `docs/evidence/`, `src/Broiler.VM.Profile.JavaScript/docs/evidence/` and the architecture tests |
| Package | `Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`, from the committed feed `eng/packages/`, pinned by `eng/packages/Broiler.ECMAScript.Algorithms.0.1.0-preview.1.pin`. Every run of this bundle used the copy the feed held at `49ea00e`, packed locally from Broiler.JS `93df3fd`; `closures/admitted-packages.json` and JS-10-005 record that copy's hash. *(Added 2026-10-13, after the runs: the package was published to nuget.org that day from Broiler.JS `2cb5275`, whose library sources are `93df3fd`'s, and the feed now holds the published package, with a new content hash, so that a restore from either source records one hash. The engine-level vectors, run again with it, match row for row, and the architecture suite passes.)* |
| Shared vectors | This family's pin, `src/tests/vectors/pins/` (corpus revision 3) |

## 2. Engine-level vectors

**Every vector matches.** [`vectors/run-revision-3.log`](vectors/run-revision-3.log) and
[`vectors/run-revision-3.tsv.gz`](vectors/run-revision-3.tsv.gz): the conformance root, built in
Release at `141f7cd` (the product sources of `49ea00e`), over this family's pin of the shared corpus,
revision 3: **10,132 vectors, 10,132 match, 0 classified divergences, 0 unclassified differences**.
The report says of the 391 vectors that carry a `divergence.broiler-vm` field that the divergence no
longer holds; JSD-0066 section 4 lists them by reason. Every vector that matched in bundle
[sjl-1-001](../sjl-1-001/README.md)'s judged run matches here, so JSD-0066's first falsifier on
vectors does not fire. The rows are byte-identical to the run taken at the cut-over before it was
committed. Retiring the 391 fields is a corpus revision in the home, and this family's pin moves to
it in a later change.

## 3. Test262, both forms (rule part A)

**MET.** All four runs are retainable as whole runs. Every changed verdict of either pair is class C4
(c') and is listed in its comparison. Each (c') row was run again alone at both commits, and **no
re-run differs**: 616 of 616 bytecode rows and 776 of 776 native rows give the same verdict at the base
and after. No row needs C1, C2 or C3. The whole runs and their comparisons come first below, then the
re-runs.

Both whole runs were taken at `49ea00e` through bundle sjl-0-001's
procedure (`eng/run-test262.py` at the pin, the conformance root built in Release, fuel 100,000,000
and 5,000 ms per variant, four jobs, the floors of the release candidate), and each was compared with
the base's retained report through `eng/compare-test262-forms.py --same-form`, the native pair with
`--exempt-guest-loads`, as class C4 has it:

| Run | Runner | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds ([`wide-bytecode.run.log`](wide-bytecode.run.log)) | pass 99,791, fail 158, exhausted 621, skipped 1,153 | pass 100,368, fail 158, exhausted 44, skipped 1,153 | 616 variants differ, **every one class (c')** ([`compare-wide-bytecode.log`](compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base: the floor was set in the win64 native form | pass 99,591, fail 192, exhausted 787, skipped 1,153 | pass 100,334, fail 192, exhausted 44, skipped 1,153 | 776 variants differ, **every one class (c')** ([`compare-wide-native.log`](compare-wide-native.log)) |

Both runs are retainable as whole runs (neither exit is 2 or 3). **No difference outside (c'):** no
variant failing at the base passes after, so part A lists no C1 row, and none moves in any other
class. Every (c') row is the after run exhausting the wall clock where the base passed (577 bytecode,
743 native) or exhausted fuel or the nested-load fan-out: the runs were taken while Broiler.JS's
whole-suite run held the same four processors, which the base's run did not share.

**The (c') re-runs.** `collect.py --only rerun` ran each pair's (c') paths again on the idle machine,
alone, at both commits. Each side was one process of the conformance root over those paths, under the
same manifest, fuel and wall allowance. The base side used the root rebuilt from a clean checkout of
`0087e8d`; its hash differs from the one bundle sjl-0-001 names, because the build records the
directory it ran in. The after side used the binary of the whole runs. The two re-runs of each pair
were compared with `compare-test262-forms.py --same-form`, the native pair with
`--exempt-guest-loads`:

| Pair | (c') rows (paths) | Re-run, base and after alike | Re-runs that differ | Where |
|---|---|---|---|---|
| bytecode | 616 (345) | 577 pass; 22 exhaust fuel; 17 exhaust the nested-load fan-out | **0** | [`rerun-wide-bytecode.log`](rerun-wide-bytecode.log), [`.tsv`](rerun-wide-bytecode.tsv), both reports |
| native | 776 (405) | 743 pass; 18 exhaust the nested-load fan-out; 15 exhaust the wall clock | **0** | [`rerun-wide-native.log`](rerun-wide-native.log), [`.tsv`](rerun-wide-native.tsv), both reports |

Each base re-run gives the base's whole-run verdict back on every row but 15 native ones. Those 15
exhausted fuel in sjl-0-001's run, and alone they exhaust the 5,000 ms wall clock first, at both
commits alike. Which of the two limits such a row reaches first depends on how fast the machine runs
it at the time. They do not differ between the commits, so part A reads no regression in them. The `.tsv` files list every row with its four verdicts: the base
run, the after run and the two re-runs.

The re-runs were started three times. The first start was stopped after eleven minutes, before
either side had ended, so that the machine could build and test a CI fix in Broiler.JS without
sharing processors with the re-runs. The second was stopped after about 40 seconds and started again
outside a two-hour task limit. Neither stopped start wrote anything to this bundle; the third start,
from 11:23 to 12:42 on 2026-10-13, is the one retained.

## 4. The fuel-parity twins (rule part B)

**MET.** [`fuel-parity.log`](fuel-parity.log): every twin of `src/tests/forms/wide` that bundle
sjl-0-001 bisected - ten, each in the bytecode form and in the native form under `x86-64-sysv` -
completes at its base figure and refuses with `AllowanceExhausted on Fuel` at one less, after as at
the base. No charge moved, so part B admits no C2 row, as JSD-0066 section 2 said it would not: the
adapters charge one unit and the library the text's length, the total the deleted code charged.

## 5. Closures, the composition register and the packages

**Every JavaScript root this machine can publish was published and run in three modes, and each
closure declares the algorithms assembly.** [`closures/`](closures/) holds, for ExecutionOnly,
SliceCompiler, Conformance, Cli and PolyglotCli, the closure under the JIT, trimming and Native AOT
(`closure-<root>.txt`), the catalog table each mode printed, identical across the three
(`catalog-<root>.txt`), and the transcripts of every publish and run (`publish-and-run.log`, and
`publish-and-run-polyglotcli.log` for PolyglotCli, whose runs are bundle UBC-4-002's, `nottext.wasm`
refused by design). Each JIT and trimmed closure lists `Broiler.ECMAScript.Algorithms`; each Native
AOT publish leaves no managed assembly beside the image. The Android root is exclusion EX-126.

| Record | What it holds |
|---|---|
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of the admitted package the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`, its NuGet content hash as the allow-list pins it and as the committed feed's copy has it, and the upstream commit its nuspec names, Broiler.JS `93df3fd` |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed (megabytes each); rule I3 read them in the rules run, which placed them in the tree for that run alone |

**The rules on the real closures.** [`rules.log`](rules.log) is the whole architecture suite, built
and run as the lane runs it (the solution in Release, then the tests), at `141f7cd`, a clean checkout
whose product sources are `49ea00e`'s: **381 tests, every one passed**. Among them rules I1, I2, I3,
I5, I6 and I7, K4 and N37; I3 read the five image records of `closures/`, placed in the tree for that
run alone and named with their hashes in the log's first line. Rule I4, data agreement, is SJL-7's
and is not yet in the register, so "I1 to I7" is six rules at this milestone.

**The composition register** names this directory as the retained closure of every JavaScript row
but Android's, and its shared-assemblies column declares the algorithms assembly on each; rule K4
holds the two to each other, exactly.

**The packages** are bundle
[JS-10-005](../../../src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-005/README.md), taken from
the same commit: the runtime's and the compiler's nuspecs declare `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.1`, which rule N37 admits at the allow-list's pin, and the pristine JavaScript
consumer restored and ran from a local feed holding the shared package beside the family's, with
upstream unreachable, rolled back to the published `0.1.0-preview.5` and forward again, refused a
version only nuget.org holds, and ran as Native AOT. Its first collection at the cut-over commit
failed every restore, because `samples/Directory.Packages.props` had stopped reading the version the
collector passes; `49ea00e` repairs that, and gives the consumer the switch its rollback needs across
the core's move of `VmProfileDescriptor` into `Broiler.VM.Abstractions`. JS-10-005's README says both.

## 6. The differential cross-check

**No new undeclared finding against the base.** [`differential/`](differential/) is the after tree's
`eng/run-differential.py` over every probe of `src/tests/differential`, with the after commit's
command-line root, against the same Broiler.JS the base was compared with: `44a214dc`, built in
Release from a clean checkout. It is a cross-check under template T8 and meets no conformance clause.

- **Against the retained answers: no difference.** Every finding names Broiler.JS.
- **Against Broiler.JS: the same findings as the base, line for line, but one**
  ([`differential/run.log`](differential/run.log) against sjl-0-001's). In
  `the-async-module-siblings.mjs`, the base's run listed an undeclared ordering divergence on case 11;
  here Broiler.JS did not finish the probe, `Uncaught TypeError: undefined is not a function` at its
  line 59, where `globalThis.siblingsRelease` is set by a module the probe imported dynamically ten
  turns earlier. Neither engine's code nor the probe moved for that line: run alone afterwards,
  Broiler.JS `44a214dc` failed the same way on every run from either tree's identical copy of the probe,
  and aborted once ([`differential/the-async-module-siblings-alone.log`](differential/the-async-module-siblings-alone.log)).
  It is Broiler.JS's own nondeterminism in when a dynamic import evaluates, not a change of Broiler.VM's
  answers, which the line does not compare.
- **No declaration is corrected.** None of the declared divergences moved, so none needs a new
  reason; the base's undeclared findings stay unadjudicated, as bundle sjl-0-001 left them.

## 7. The measurement sub-bundle (rule part C)

Pending: it is taken last, on the idle machine, after the (c') re-runs of section 3.

**Read before it runs: clause 1 cannot hold.** Part C's condition asks every lane's answer to equal
the workload's answer at the base arm. `to-string.js` answers with the total length of the strings it
converts, `toString` at radixes 2, 7, 16 and 36 among them, and the cut-over corrects radix digits
(JSD-0066 section 4), so its answer moves: 2515784 at the base (bundle sjl-0-001's base reading),
2516095 at `49ea00e`, taken with the closures' published command-line root before any lane ran.
`to-number.js` and `literals.js`, and every control, answer as at the base. Broiler.JS's own rule
counts conversions instead of results for this reason; this one does not.

**The owner's choice, 2026-10-13, before the measurement:** measure under the rule as written, not
under a revised rule. Part C will therefore read NOT MET on clause 1 for `to-string.js` in both forms,
whatever its timings show, and D against N is retained beside it for every workload. Clause 13 stays
open, and SJL-3 with it, until a dated rule judges a condition that a correcting cut-over can meet.

## 8. The simplification check (SJL-3.7)

**The deletion check.** Rule I5 holds the 34 members its register names
(`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) to never being declared again in the
JavaScript family's product sources, by fully qualified name; its run is in [`rules.log`](rules.log),
`I5_No_Retired_Member_Is_Declared_Again` and its witness, both passed. [`simplification/deletion-check.log`](simplification/deletion-check.log) adds
a search a reader can repeat: each member's simple name as a declaration in the family's C# sources.
**None is declared in the type it was removed from.** The lines it finds are same-named members of
other types: `JsHostRealm.ToJsString`, BigInt's `ToRadixString` and `ToNumber`, the `ToNumber` of
`JsEngine.Conversions`, `JsHostRealm`, `JsWord`, `JavaScriptValue` and two opcode handlers, a
collation table's `DigitValue`, eighteen properties, fields and methods named `Value`, and the tokenizer's `IsWhiteSpace`, which
reads the lexer's own table (`JsUnicodeLexical.IsWhiteSpace`, the front end's, section 1 of the area).

**The adapter review.** Each adapter of JSD-0066 section 2 was read at this commit against the area's
adapter definition (areas/numbers.md section 5.3: coercion, errors, charging, object creation, and no
algorithm).

| Adapter | What it holds | Algorithm |
|---|---|---|
| `JsRealm.Number`: `toString`, `toFixed`, `toExponential`, `toPrecision` | `thisNumberValue`; `ToIntegerOrInfinity` of the argument in each method's order (toFixed's range check before the value is looked at, toExponential's and toPrecision's non-finite answer before it); the `RangeError`s; one charge of the text's length plus one; the string | none: the text is `NumberText`'s |
| `JsRealm.Global`: `parseInt`, `parseFloat` | `ToString`, `ToInt32` of the radix, one unit of charge, the library charging the text | none |
| `JsEngine.Conversions.ToNumberFromText`, `JsRealm.Json`'s number, `JsBinary.NumberOf` | one unit of charge; for JSON, the grammar that found the span | none |
| `JsRealm.String`: `trim`, `trimStart`, `trimEnd` | the `this` coercion, the index walk and the substring | none: the class of each character is `NumberParsing.IsStrWhiteSpaceChar`'s |
| `JsNumberFormat`: `NumberToString`, `ToUintString`, `TrimWhiteSpace` | the shared strings of 0 to 255, `NaN` and the infinities; an array index's decimal text (`uint`, exact); the original string when nothing is trimmed | none |
| `JsObject.IsArrayIndex`, `JsRealm.Object`'s and `JsBinary`'s canonical numeric test | nothing: each is one call | none |
| `SliceTokenizer`'s literal values, `JsParser.BigIntValueOf`, `JsParser.NumberKey` | the scan, the diagnostics, strictness, the format's BigInt ceiling passed as the width limit | none: `NumericLiterals.Evaluate` and `NumberText.ToString` |

**The duplicate search.** [`simplification/duplicate-search.log`](simplification/duplicate-search.log)
lists 128 hits in the component's C# product sources. **None is a surviving duplicate of the area**:

| Class | Hits | Where | Reading |
|---|---|---|---|
| Composition roots' own harness and checks | 105 | Conformance (35: test262 metadata, pins, selection, the completion protocol), SliceCompiler (59, 55 of them `"R"` as an operand-kind label in `NativeTemplateScanChecks`), Cli (2), PolyglotCli (3), the UBC fixture (2), the WebAssembly harness (4) | text of a harness's own files and options, and WebAssembly's `f32`/`f64` text, which is not this area's; no JavaScript value is converted |
| Other areas' code | 13 | BigInt (`JsBigInt`, 2: SJL-4), Intl (`JsDecimal`, `JsIntlTables`, `JsNumberFormatter`, `JsPluralRule`, 7: SJL-9), Temporal (`JsTemporalParser`, 2: SJL-10), and the front end's escape and template scans (`SliceTokenizer`, 3; `JsParser.ScanSubstitution`, 1: SJL-13) | each is answered by its own milestone; areas/numbers.md section 1 does not name them |
| Names and options | 4 | `VmIdentityPrimitives`, `PicoVmOptions` | `string.IsNullOrWhiteSpace` on identifiers |
| Comments recording this cut-over | 3 | `JsCompiler.Emission`, `SliceTokenizer`, `JsUnicodeLexical` | the text of what was replaced |
| A host-side diagnostic rendering | 1 | `JavaScriptValue.ToDiagnosticString` | **A finding, not a duplicate.** It is documented as deliberately not `Number::toString`, and it is not: it renders a host value for a corpus entry or a report. Its remark calls the rendering round-trip, and .NET's `"R"` is not, for `2**-25` and the double below it render alike (`2.980232238769531E-08`). The constant pool's key had the same defect, and SJL-3.4 corrected it, because there it changed a program's values (JSD-0066 section 4); here two values would print alike in a report. Left for its owner, outside this pilot |

The search pattern is in the log's header; it is the same as Broiler.JS's bundle sjl-3-001 runs,
with .NET's other double conversions added.

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-125 | Open | **The number-conversion code that left this component's assurance coverage.** The 34 members rule I5 registers (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at SJL-3.4. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Numbers`, which the home's own assurance component covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters that call the library stay in this family and stay covered |
| EX-126 | Open | **The Android root's closure is not re-collected after the cut-over.** Collecting it means publishing for `android-x64` and running on an Android emulator, which the machine that took this bundle does not have. The root reaches `Broiler.ECMAScript.Algorithms` through the runtime's package reference, so its register row declares it, and says in its cell that its retained closure, bundle JS-ANDROID-001's, predates it; rule K4 holds that closure to the row without the shared assembly, and refuses it once a re-collected closure carries one |
