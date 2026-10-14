<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-4-001 - the numerics area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-4 of the shared JavaScript libraries programme,
[`sjl-4-numerics.md`](../../javascript-shared-libraries/milestones/sjl-4-numerics.md). **Rule:**
[`decision-rule.md`](decision-rule.md), written at SJL-4.1 before any code of the milestone existed.
**Records:** [JSD-0067](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0067-numerics-move-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md). **Owner and reviewer:** one person holds every
role (EX-30), and nobody independent has read this bundle.

**State:** collected, 2026-10-14, at SJL-4.6. **Part A is NOT MET** in the native pair, under
[`decision-rule.md`](decision-rule.md) and under its revision
[`decision-rule-revised-2026-10-14.md`](decision-rule-revised-2026-10-14.md) alike, and met in the
bytecode pair (section 4). Two rows the original rule admits as class C4 (c') still differ in their
exhaustion dimension when run again alone, which it reads as a regression. The revision's class (d)
admits one of them; the other, `decodeURI/S15.1.3.1_A2.5_T1.js` [strict], reached only the wall clock
in all twelve of its runs at the base, which class (d) does not admit. **Part B is MET** (section 5). The vectors,
the closures, the rules and the simplification check are sections 6 to 8, and section 10 holds the
Broiler.VM clauses. *(Revised 2026-10-14, at SJL-4.6. The paragraph read: "**State:** collection in
progress. Nothing here is a verdict until its section says so. **Taken:** the base (section 1) and the
engine-level vectors of SJL-4.2 (section 2). The after runs, their comparisons and the rest of SJL-4.6
are added when SJL-4.4's cut-over has landed; [`collect.py`](collect.py) gains their steps then.")*
*(Revised again 2026-10-14, later at SJL-4.6, when the revised rule's races step had run. Its first
two sentences after the date read: "**Part A is NOT MET** in the native pair and met in the bytecode
pair (section 4): two rows the rule admits as class C4 (c') still differ in their exhaustion dimension
when run again alone, which the rule reads as a regression, and a diagnostic shows both commits racing
between the same two allowances on them.")*

## 1. The base

| | |
|---|---|
| Commit | `d0e607e`, a clean checkout ([`base/identity.txt`](base/identity.txt)): the rule's commit `6ab3d47` and this bundle's collector, which is an evidence script. Outside the documents, it differs from the default branch after SJL-3's merge (`a04b2e5`) only by part B's ten probe files, which the identity lists |
| Taken | 2026-10-13, 16:34Z to 17:04Z, by `collect.py --only base-identity --only base-test262 --only base-twins`, before any code of SJL-4 landed, on an otherwise idle machine ([`base/collect.log`](base/collect.log)) |
| Part A | test262 at the pin in both forms, through bundle `sjl-0-001`'s procedure: [`base/wide-bytecode.report.gz`](base/wide-bytecode.report.gz) and [`base/wide-native.report.gz`](base/wide-native.report.gz), each with its run log, merge log, floor log, JSON report and failure manifest. Bytecode, of 101,723 variants: pass 100,368, fail 158, exhausted 44, skipped 1,153. Native: pass 100,334, fail 192, exhausted 44, skipped 1,153. The bytecode floor holds; the native run's exit code 4 is the floor's re-basing note, because the floor was set in the win64 native form and this run is sysv, as in SJL-3's runs |
| Part B | [`base/fuel-parity.log`](base/fuel-parity.log): each of the ten wide twins and nine numerics probes, its smallest completing allowance bisected under the bytecode form, then each form at that figure (completes, with its answer) and at one less (refused on fuel). Both forms agree at every figure |

**The base was taken twice, and this bundle reads the second take.** The first take, from 16:01Z to
16:33Z at the same commit and from the same checkout, ran while SJL-4.2's reference computations ran
on the same four-CPU machine: short Python processes, one of them using about a minute of CPU and a
large amount of memory near 16:26Z, during the native test262 run. The rule's header asks for an
otherwise idle machine, so the three steps were taken again with nothing else running. The first take
is retained whole in [`base/not-idle/`](base/not-idle/) and neither part reads it. The two takes
agree: the same conformance binary
([`base/test262-binary.txt`](base/test262-binary.txt)), the same verdict on every row of both test262
runs, and the same figure and answer for every twin and probe.

## 2. Engine-level vectors (SJL-4.2)

**Every vector of corpus revision 7 has a classified result in this family.**
[`vectors/run-revision-7.log`](vectors/run-revision-7.log) and
[`vectors/run-revision-7.tsv.gz`](vectors/run-revision-7.tsv.gz): the conformance root, built in Release
from this change's sources, over this family's pin of the corpus, revision 7 (Broiler.JS `b725bb0`,
[`src/tests/vectors/pins/vectors.pin`](../../../src/tests/vectors/pins/vectors.pin)): **22,634 vectors,
22,534 match, 100 classified divergences, 0 unclassified differences.** The 100 are the
`divergence.broiler-vm` fields revision 7 adds, all in the Math kernels: 70 `expm1` and `log1p` results,
23 cube roots, 5 hypotenuses and 2 `sumPrecise` sums that flush a tiny term; each names SJL-4.4.
The negative control ([`vectors/negative-control.log`](vectors/negative-control.log)) changes the
expected value of `numerics/toint32/0010` in memory, and the run reports that vector, and only it, as an
unclassified difference.

**The first run, on revision 6**, is [`vectors/run-revision-6.log`](vectors/run-revision-6.log) and
[`vectors/run-revision-6.tsv.gz`](vectors/run-revision-6.tsv.gz), taken over the archive of Broiler.JS
`e3892d6` under the scratch pin [`vectors/revision-6.pin`](vectors/revision-6.pin), which records its
digests (`git archive` of that commit reproduces it): 22,533 match and 101 differences. Revision 7
classifies 100 of them, and the 101st was a vector defect it fixes: `numerics/tostring-long/0016` built a
70,000-bit input as one literal, which this family's wide manifest refuses. Revision 6 also retired the
391 `divergence.broiler-vm` fields of the number area, which SJL-3.4's cut-over corrects; every one of
those vectors matches in both runs.

**What the runs found that JSD-0067 did not classify** is
[JSD-0068](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0068-the-numerics-vectors-find-what-x2-did-not-classify.md):
JSD-0067's first falsifier fired on four differences between the engines, three of them Broiler.JS's
and one this family's literal limit. **The run admits Intl and Temporal** since this change, because the
rounding vectors run through Temporal; until revision 6 its realms admitted every surface but those two.

## 3. The after arm

| | |
|---|---|
| Commit | `f617962`, a clean checkout ([`after/identity.txt`](after/identity.txt)). Its product sources are the cut-over commit's, `6bd3009` (JSD-0069), and those of the correction it carries (section 7): the identity lists the nine files outside the documents that differ from `6bd3009`. Five are not read by a product - the notices, the assurance manifest, and this family's pin of the shared corpus, which section 6 moves to revision 9 - and three are the correction: `BigIntChecks.cs` of the slice compiler, and `JsRealm.Date.cs` and `JsTemporal.Options.cs`, whose two truncations call the library |
| Rule | [`decision-rule.md`](decision-rule.md), byte-identical at the base and after (the same SHA-256 in both identities) |
| Taken | 2026-10-14, 01:16Z to 02:11Z, by [`collect.py`](collect.py) step by step - `after-identity`, `after-test262`, `after-twins`, `compare`, `rerun`, `vectors`, `closures`, `rules`, `simplification` - on an otherwise idle machine, into [`after/`](after/) and [`vectors/`](vectors/) |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, four jobs, the native backend `x86-64-sysv` |

**The first take.** The after arm was first taken at `b572321`, from 00:24Z to 01:09Z the same day,
and is kept whole in [`first-take/`](first-take/). Its closures found the slice compiler's BigInt checks
failing in every mode (section 7), and its duplicate search found two hand-written truncations
(section 8). The commit that corrects both, `f617962`, changes product sources, so every step was
taken again there. This bundle reads none of the first take. The two takes changed the same four
test262 rows of class C3 in each form, gave the same figures and answers for every twin and probe, and
the same vectors results. They differ in the native pair's (c') rows, which are the rows that exhaust
an allowance on the clock: three in the first take, whose re-runs agreed, and five in this one
(section 4).

## 4. Test262, both forms (rule part A)

**NOT MET**, in the native pair. The bytecode pair meets the rule: both its runs are retainable as
whole runs, and its four changed rows are class C3, listed below. The native pair's runs are
retainable, and its changed rows are the same four C3 rows and five of class C4 (c'). Of the five, two
gave a different exhaustion dimension at each commit when they were run again alone, and the rule
reads such a row as a regression: "each (c') row is run again alone at both commits and the re-run is
retained, and a row that still differs outside C1 and C3 is a regression".

| Run | Runner | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds ([`after/wide-bytecode.run.log`](after/wide-bytecode.run.log)) | pass 100,372, fail 154, exhausted 44, skipped 1,153 | pass 100,368, fail 158, exhausted 44, skipped 1,153 | 4 variants differ ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base: the floor was set in the win64 native form | pass 100,338, fail 188, exhausted 44, skipped 1,153 | pass 100,334, fail 192, exhausted 44, skipped 1,153 | 9 variants differ ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

Neither exit is 2 or 3, so all four runs are retainable. The comparison script admits only class C4's
own rows. It lists the others as unclassified, and the rule leaves C1 and C3 to this table:

| Row | Pairs | Base -> after | Class |
|---|---|---|---|
| `test/staging/sm/Math/expm1-approx.js` [sloppy], [strict] | bytecode, native | Failed -> Passed | **C3**. `Math.expm1` is implementation-approximated (ECMA-262, `Math.expm1`), and the test asks for results near zero within a few units in the last place. The base computed `exp(x) - 1` and lost the significant digits there; the library's `Expm1` is within one unit (JSD-0067 decision 5). The 43 `numerics/expm1` vectors of reason `vm-expm1-log1p`, such as `numerics/expm1/0008`, classify it; corpus revision 9 retires them (Broiler.JS `docs/evidence/sjl-4-001/engine/retired-divergences-revision-9.tsv`) |
| `test/staging/sm/Math/log1p-approx.js` [sloppy], [strict] | bytecode, native | Failed -> Passed | **C3**, as above: the base computed `log(1 + x)`. The 27 `numerics/log1p` vectors of the same reason, such as `numerics/log1p/0026`, classify it |
| `test/built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` [sloppy]; `test/built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js` [sloppy]; `test/staging/sm/Date/dst-offset-caching-4-of-8.js` [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | **C4 (c')**. The re-runs alone agree: the first two exhaust fuel at both commits, the third the wall clock |
| `test/built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | **C4 (c'), and its re-runs differ**: the wall clock at the base, fuel after - **a regression as the rule reads it** |
| `test/staging/sm/Date/dst-offset-caching-6-of-8.js` [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | **C4 (c'), and its re-runs differ**: fuel at the base, the wall clock after - **a regression as the rule reads it** |

The re-runs are [`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their log and both
reports: each side was one process of the conformance root over the five rows' paths, alone, at
`d0e607e` (the base, rebuilt in a clean checkout) and at `f617962`. Their comparison, through the
same script, admits all three of its differences as (c'). The rule, though, admits a (c') row only if
its re-runs agree, and on these two they do not.

**What the two rows are.** Neither passes at either commit: each exhausts an allowance in every run,
at both commits, and only which allowance is reached first changes. That is a race between part A's
fuel allowance, 100,000,000, and its wall clock, 5,000 ms, on a row that needs about both.
[`race-diagnostic.py`](race-diagnostic.py) is a diagnostic, not a step of the rule, and decides
nothing. It ran the two paths alone ten times at each commit, alternating which commit went first, on
the same idle machine ([`after/race-diagnostic.tsv`](after/race-diagnostic.tsv)):

| Row | Base: fuel, wall clock | After: fuel, wall clock |
|---|---|---|
| `decodeURI/S15.1.3.1_A2.5_T1.js` [strict] | 4, 6 | 3, 7 |
| `dst-offset-caching-6-of-8.js` [strict] | 0, 10 | 1, 9 |

Both commits split between the two allowances on both rows, so the dimension a single run reports is
not stable at either commit. The rule does not read the diagnostic, and it does not admit a race of
exhaustion dimensions. **No row passing at the base fails, is exhausted or is unsupported after**, and
none needs class C1. *(Revised 2026-10-14, later at SJL-4.6: after "exhaustion dimensions." the
paragraph read "Admitting one would take a revised rule, written now that these runs have been seen,
and so weaker evidence. That is the owner's decision; until it is taken, part A stays NOT MET."; the
owner then had the revision below written.)*

**Under the revised rule.** [`decision-rule-revised-2026-10-14.md`](decision-rule-revised-2026-10-14.md),
written at the owner's direction after these runs were seen, and so weaker evidence, adds class C4 (d):
a (c') row whose re-runs differ is run alone ten more times at each commit, alternating which commit
goes first, and is admitted only if it is exhausted in every run at both commits and each commit
reaches both allowances at least once. The collector's `races` step took the twenty runs afresh, on the
idle machine, from 05:18Z to 05:26Z on 2026-10-14, with the conformance roots of the re-runs
([`after/races-wide-native.tsv`](after/races-wide-native.tsv), every report in
[`after/races-wide-native/`](after/races-wide-native/)). Over each row's twelve runs at each commit -
the whole run, the re-run and the ten repetitions
([`after/races-wide-native-summary.tsv`](after/races-wide-native-summary.tsv)):

| Row | Base: fuel, wall clock | After: fuel, wall clock | Class (d) |
|---|---|---|---|
| `decodeURI/S15.1.3.1_A2.5_T1.js` [strict] | 0, 12 | 2, 10 | **not admitted**: the base reached the wall clock only, in every run, and the rule reads that as a regression |
| `dst-offset-caching-6-of-8.js` [strict] | 1, 11 | 1, 11 | **admitted**: exhausted in every run, and both allowances reached at each commit |

**So part A stays NOT MET under the revised rule as well**, on the one row. All twenty repetitions
reached the wall clock first, at both commits alike: on this machine, since the restart that preceded
the step, both rows spend the 5,000 ms before the 100,000,000 units of fuel. The repetitions show no
difference between the commits. But class (d) asks each commit to reach both allowances, so that a
commit that always reaches one of them is not mistaken for a race, and in this regime the base reached
only one of them on that row. A further revision would be written after this result too, and so weaker
again; that, or leaving clause 9 unmet, is the owner's decision.

**The exhaustion verdicts** milestone SJL-4.6 requires: every variant exhausted at the base is
exhausted after, and every variant that completes at the base completes after. The dimension moved on
the five native (c') rows alone, from the wall clock to fuel, and the two rows above are the ones whose
re-runs did not settle it.

## 5. The fuel-parity twins and probes (rule part B)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log): every twin of `src/tests/forms/wide` and
every probe of `src/tests/forms/numerics` that [`base/fuel-parity.log`](base/fuel-parity.log)
bisected - ten twins and nine probes, each in the bytecode form and in the native form under
`x86-64-sysv`, 38 rows - completes at its base figure and refuses with `AllowanceExhausted on Fuel`
at one less, after as at the base, and completes with the base's answer. **No charge moved**,
inside the area or outside it: the BigInt kernel keeps this family's charging units, as JSD-0067
decision 3 requires, and the adapters' charges are the deleted code's.

## 6. Engine-level vectors at revision 9

**Every vector passes.** This family's pin of the shared corpus moves to revision 9, Broiler.JS
`f2c3597`, in `b572321` ([`src/tests/vectors/pins/vectors.pin`](../../../src/tests/vectors/pins/vectors.pin)
quotes what it read for revision 7). Revision 9 retires every divergence field of the numerics area:
this family's 100, which SJL-4.4's cut-over corrected, and Broiler.JS's 54. At `f617962` the conformance
root over the pin gives **22,634 vectors, 22,634 match, 0 classified divergences, 0 unclassified
differences** ([`vectors/run-revision-9.log`](vectors/run-revision-9.log),
[`vectors/run-revision-9.tsv.gz`](vectors/run-revision-9.tsv.gz)). The negative control changes the
expected value of `numerics/toint32/0010` in memory, and the run reports that vector, and only it, as
an unclassified difference ([`vectors/negative-control-revision-9.log`](vectors/negative-control-revision-9.log)).

## 7. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, and each
closure declares the algorithms assembly.** [`after/closures/`](after/closures/) holds, for
ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, the closure under the JIT, trimming
and Native AOT (`closure-<root>.txt`), the catalog table each mode printed, identical across the three
(`catalog-<root>.txt`), and the transcripts of every publish and run (`publish-and-run.log`, and
`publish-and-run-polyglotcli.log` for PolyglotCli, `nottext.wasm` refused by design). Each JIT and
trimmed closure lists `Broiler.ECMAScript.Algorithms`, and each Native AOT publish leaves no managed
assembly beside the image. The Android root is still exclusion EX-126, whose reasons hold unchanged.

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the admitted package the closures carry: `Broiler.ECMAScript.Algorithms` `0.2.0-preview.1`, its NuGet content hash as the allow-list pins it and as the committed feed's copy has it, and the upstream commit its nuspec names, Broiler.JS `01209c4` |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed (megabytes each); rule I3 read them in the rules run, which placed them in the tree for that run alone |

**A defect the closures found.** In the first take, the slice compiler's checks failed in every mode,
with a `MissingMethodException` for `JsBigInt.AsIntN`. `BigIntChecks` reaches the internal `AsIntN`
and `AsUintN` through `UnsafeAccessor`, declared with the `Action<ulong>` meter that SJL-4.4's cut-over
replaced with the shared library's `WorkCharge`. An accessor binds only when it is called, and nothing
calls these checks but this procedure, so neither the cut-over's tests nor the architecture suite saw
it. `f617962` declares the accessors with `WorkCharge`, and at `f617962` all 646 of the slice
compiler's checks pass in every mode. The first take's transcript is kept in
[`first-take/after/closures/`](first-take/after/closures/).

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture
suite, built and run as the lane runs it (the solution in Release, then the tests), at `f617962`:
**381 tests, every one passed**. Among them are rules I1, I2, I3, I5, I6 and I7, K4 and N37; I3 read the
five image records of the closures, which were placed in the tree for that run alone and are named with
their hashes in the log's first line. Rule I4, data agreement, is SJL-7's and is not yet in the
register, so "I1 to I7" is six rules at this milestone. **Rule I5 holds the deletion list**: its register
holds the 44 members SJL-4.4 deleted, beside SJL-3.4's 34, and it passes.

**The packages** are bundle
[JS-10-006](../../../src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-006/README.md), collected at
the cut-over's commit, whose baseline rule N37 holds (JSC-302). `f617962` changes no package's
dependencies or files.

*(Added 2026-10-14, after the runs. The package was published to nuget.org that day as
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.2`, from Broiler.JS `b3d54f7`: the library
`0.2.0-preview.1` was packed from, with one change to its code, `BigIntPolicy.RepresentationLimit`,
which this family's BigInt policy of 2^20 bits never reaches. The committed feed now holds the
published package, and the checkout references it (JSC-303). Every run of this bundle used the copy the
feed held at `f617962`, which `after/closures/admitted-packages.json` records. With the published
package, the engine-level vectors at revision 9 give a report identical byte for byte to
`vectors/run-revision-9.tsv.gz`, their negative control reports `numerics/toint32/0010` alone, and the
architecture suite passes 381 of 381.)*

## 8. The simplification check

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log)
searches the simple name of every member rule I5 registers - 78, SJL-3.4's and SJL-4.4's - as a
declaration in the JavaScript family's sources. 62 are declared nowhere. The other 16 match other
members of the same simple name, in other types (`JsTimeZones.Unsigned`, `JsDecimal.ParseDecimal`,
`JsTemporalParser.Bounded`, `JsCollationData.DigitValue`, the instruction handlers' `Negate` and
`ToNumber`, and the like), or the adapters that call the library under a deleted member's name:
`JsEngine.ToInt32`, `ToUint32` and `ToInteger`, `JavaScriptValue.ToInt32` and `ToUint32`,
`JsBigInt.Negate`, `JsBigInt.ToRadixString` and `JsTemporal.Durations`' `ToDouble`, each one line over
`IntegerConversions`, `BigIntMath`, `BigIntText` or `ExactRounding`. Rule I5 decides by the fully
qualified name, and it passes (section 7).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's product sources for the constructs that would mean a surviving copy of the
area: the platform's `Math.Pow`, `Cbrt`, `Round` and `Truncate`, `ExpM1` and `LogP1`, binary16 by the
platform, a rounding mode spelled as a string or the platform's midpoint rule, `BigInteger`'s own
parse and power, and the 32-bit wrap written out. **30 hits, none a copy of the area:**

| Hits | Where | What they are |
|---|---|---|
| 3 | `JsNumberFormatter`, `JsRealm.NumberFormat`, `JsTemporal.Options` | the rounding modes' option names, which the engine's `GetOption` validates; the modes are the library's `RoundingMode` |
| 7 | `JsRealm.DurationFormat`, `JsRealm.Number`, `JsTemporal.Options`, `JsTemporalCore`, `JsRealm.Array` | an integrality test (`IsIntegralNumber`), the bounds `2^32` and `2^53` that `IsValidDuration` and `DurationFormat` compare against, and `ArrayKeyOf`'s array-index test, which names its key with `JsNumberFormat` |
| 1 | `JsRealm.Math` | `Math.trunc` itself, which keeps -0 and is not `ToIntegerOrInfinity` |
| 1 | `JsCalendars` | the lunar calendars' count of mean synodic months since an anchor, calendar arithmetic outside the area |
| 3 | `JsTemporalParser` | ISO digit fields read as integers, and the power of ten that scales a fraction of a second |
| 9 | `Broiler.VM.Profile.WebAssembly`, `Broiler.VM.Ubc` | WebAssembly's and the universal bytecode's own `nearest` and `trunc`, which are not this area |
| 6 | the Cli's, the slice compiler's and the WebAssembly harness's checks | literals in checks and a host value read back by a check |

The first take found 32 hits. The two more were hand-written `ToIntegerOrInfinity` on specification
paths, which behaved as specified: Date's `TimeClip`, a truncation that folded -0 by hand, and
Temporal's `ToIntegerWithTruncation`, a truncation plus 0.0. `f617962` makes both call the library.

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-127 | Open | **The numerics code that left this component's assurance coverage.** The 44 members rule I5 registers for SJL-4.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at SJL-4.4. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Numbers` (`IntegerConversions`, `Float16`, `MathKernels`, `PreciseSum`, `ExactRounding`, `BigIntMath`, `BigIntText`), which the home's own assurance component covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters that call the library stay in this family and stay covered. EX-125 says the same of the number-conversion area |

## 10. The Broiler.VM clauses

| Clause | What it asks | Where | State |
|---|---|---|---|
| 8 | Rules I1 to I7 hold, and rule I5 holds the deletion list | section 7, [`after/rules.log`](after/rules.log) | **Met** at `f617962`: 381 of 381, I5 among them |
| 9 | Whole-suite test262 in both forms meets the area rule | section 4 | **Not met**, under the rule and under its revision: the native pair's (c') row `decodeURI/S15.1.3.1_A2.5_T1.js` [strict], whose re-runs differ and which class (d) does not admit; the bytecode pair meets it *(revised 2026-10-14, later at SJL-4.6; read "**Not met**: the native pair's two (c') rows whose re-runs differ in dimension, a regression as the rule reads it; the bytecode pair meets it")* |
| 10 | The fuel-parity twins and the exhaustion verdicts meet the area rule | sections 4 and 5 | **Met**: part B MET; the exhaustion verdicts hold, but for the dimension of the two rows of clause 9 |
| 11 | Every root published and run in three modes, its closure declared | section 7 | **Met** for every root this machine can publish, at `f617962`; Android is EX-126 |
| 12 | Engine-level vectors pass | section 6 | **Met**: 22,634 of 22,634 at corpus revision 9 |
