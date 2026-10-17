<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-7-001 - the Unicode area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-7 of the shared JavaScript libraries programme,
[`sjl-7-unicode.md`](../../javascript-shared-libraries/milestones/sjl-7-unicode.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-7.1 before any code of the milestone
existed, with its probes and workloads in `src/tests/forms/unicode`.

**Records:** [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md),
whose input is [`correspondence.md`](correspondence.md).

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State, 2026-10-17, at SJL-7.6: the after arm is taken at `be0b610` (sections 5 to 13). Parts A and B of
[`decision-rule-area.md`](decision-rule-area.md) are MET as written, with no revision**: every changed test262
verdict in either form is a race between allowances that class C4 admits, and no fuel charge moved (sections 6
and 7). Clauses 9, 10, 12 and 13 are met, and clause 11 is met on parts A and B (section 13).
- **The engine-level vectors at corpus revision 20 all match** (section 8).
- **Part C, the measurement, is NOT MET on clause 3** (section 12): the after arm is slower than the base beyond
  the A/A floor for casing and for identifier scanning in the native form. JSD-0031's fifth reason therefore stays
  open for this engine for those two, in the native form.
- **Nothing is accepted.** Review is deferred, and SJL-7 may not meet its gate before SJL-3 meets SJL-3's.

*(Revised 2026-10-17, later at SJL-7.6, when the vectors at revision 20 and part C were added. The first bullet
read "**Two things are not yet in this bundle:** part C, the measurement (section 12), and the engine-level
vectors at corpus revision 20 (section 8). Until part C is taken, the evidence item it answers, and with it
JSD-0031's fifth reason for this engine, stays open.")* *(Revised 2026-10-17, at SJL-7.6. The state read: "**State, 2026-10-16, at SJL-7.4: the base is taken, the
vectors are at corpus revision 19, and the cut-over has landed (section 4); nothing here is a verdict.**", with
the bullets "Parts A and B's bases were taken at the commit that carries the rule, `7b44dc7`, before any code of
SJL-7 lands. Section 2 records them.", "Part C's base arm is built from the same commit when the after arm
exists, because the rule interleaves the arms inside each repetition." and "The after runs follow SJL-7.4's
cut-over, at SJL-7.6.")* *(Revised 2026-10-16, at SJL-7.4. The heading read "at SJL-7.3: the base is taken, the engine-level vectors are
run and the pin is at corpus revision 19, and nothing here is a verdict.")* *(Revised 2026-10-16, at SJL-7.3, when the vectors were run. The heading read "at SJL-7.2: the base is taken, and
nothing here is a verdict.")* *(Revised 2026-10-16, at SJL-7.2, when the base was taken. The state read: "rules and correspondence only,
2026-10-16, at SJL-7.1. **Nothing here is a verdict, and nothing has been taken.**", with the bullets "The base
of all three parts is taken at the commit that carries the rule, before any code of SJL-7 lands.", "The after
runs follow SJL-7.4's cut-over." and "A collector joins this bundle when there is a run to take.")*

The owner's clauses are judged in Broiler.Unicode's bundle of the same name, and Broiler.JS's in its own. This
bundle reads neither's runs.

## 1. SJL-7's area rule and its correspondence (SJL-7.1)

**The correspondence.** It was read from code in both engines, in Broiler.Unicode and in Broiler.Regex, at the
revisions it names. It numbers every difference the area verdict classifies (P, I, W, F, C, N, S, G), and every
behavioural cell its rows rest on was run in both engines. **This family's own corrections in the area are
three:**
- its casing ceiling is checked on the whole result;
- so is its normalization ceiling;
- its segmenter charges every code point it scans.

The rest of the differences correct Broiler.JS, or are SJL-11's.

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It
  admits a moved exhaustion verdict as class C2 on `test/intl402/Segmenter/` alone, where JSD-0074's decision 4
  changes segmentation's charging units, and nowhere else.
- **Part B** holds the wide twins and the Unicode probes to their base figures, with no class.
- **Part C** is the measurement that answers JSD-0031's fifth reason for this engine. Its three workloads
  answer with counts of completed operations, so that no correction can fail its condition.

## 2. The base (SJL-7.2)

[`collect-area.py`](collect-area.py) took it. It reuses bundle `sjl-6-001`'s area collector, and through it
that bundle's import of a hosted run and bundle `sjl-5-001`'s twin bisection, over the wide twins and the
Unicode probes. It judges nothing.

| | |
|---|---|
| Commit | `7b44dc7`, the commit that carries the rule, from a clean checkout ([`base/identity.txt`](base/identity.txt)). Against the default branch after SJL-6's merges, `24632b1`, it adds 10 files outside the documents, all of them part B's probes and part C's workloads in `src/tests/forms/unicode`, which the rule commits. Its product sources are `24632b1`'s, as the rule says |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), `sha256=b4908773...` in the identity |
| Taken | 2026-10-16: part A's base by the workflow from 15:22Z to 15:30Z; part B's base locally from 16:24Z, on the programme's local machine (`linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv`), while other work ran beside it. Nothing in part B is timed |

**Part A's base.** [38923456434](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38923456434),
attempt 1, was dispatched from `main` at `16a35b2` on 2026-10-16 at 15:22:53Z, with both forms and the commit
`7b44dc7` named by all forty digits, and completed at 15:29:55Z ([`base/hosted/run.txt`](base/hosted/run.txt)).
- The workflow and its driver at the dispatched ref are byte-identical to the rule commit's: `git diff 16a35b2
  7b44dc7` names neither.
- The artifact `test262-evidence` is retained unchanged in [`base/hosted/`](base/hosted/).
  [`base/hosted/imported.txt`](base/hosted/imported.txt) records the digest of the zip as downloaded, which is
  the digest GitHub records.
- Every runner and the merging job ran image `ubuntu24` 20261012.334.1.

| Run | Complete ([`base/completeness.log`](base/completeness.log)) | Totals |
|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants |
| `wide-native` | exit 4, a re-base, as the rule admits; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants |

The totals are bundle `sjl-6-001`'s after run's, figure for figure, as they should be for the same product
sources.

**Part B's base** ([`base/fuel-parity.log`](base/fuel-parity.log)).
- **The method:** each twin and probe's smallest completing fuel allowance was bisected under the bytecode
  form, then both forms were run at that figure and at one less.
- **The result:** every one completes at its figure and is refused on fuel at one less, in both forms, and
  both forms complete with the same answer. That figure is what part B's after run is held to.
- **The wide twins' figures and answers equal bundle `sjl-6-001`'s after run's.**

| Twin or probe | Figure | Answer |
|---|---|---|
| `wide/array-loop` | 73,082 | 999000 |
| `wide/call-loop` | 28,026 | 1000 |
| `wide/closure-loop` | 31,044 | 1000 |
| `wide/fib` | 7,682 | 144 |
| `wide/generator-loop` | 40,075 | 499500 |
| `wide/numeric-loop` | 23,022 | 498497 |
| `wide/property-loop` | 29,029 | 183125 |
| `wide/string-loop` | 26,083 | 25 |
| `wide/throw-loop` | 34,026 | 1000 |
| `wide/try-loop` | 34,022 | 499500 |
| `unicode/casing` | 328,460 | 179280 |
| `unicode/normalize` | 607,821 | 420519 |
| `unicode/properties` | 14,348,623 | 35050 |

[`area-manifest.json`](area-manifest.json) lists the digest of every file of the base and of the vector runs
(section 3), the collector, the rule and the correspondence.

## 3. Engine-level vectors on the Unicode area (SJL-7.3)

Broiler.JS's corpus revision 18 (`b69eb709`) adds the Unicode area: 37,738 vectors in five files. This
profile's conformance root ran on it, and every vector matched. Revision 19 (`c976cbea`) classifies both engines'
first runs. It holds none of this profile's differences, since there were none, and 3,678 of Broiler.JS's. The
pin then moved to revision 19, in `d81c0c1`, by its README's procedure: the archive made twice,
byte-identical, its 42 files hashed after extraction.

| Run | Commit | Result | Files |
|---|---|---|---|
| Revision 18 | `286fd23`, whose product sources are `24632b1`'s, taken 2026-10-16 from 17:43Z with the pin [`vectors/revision-18.pin`](vectors/revision-18.pin) beside a scratch copy of its archive, which `git archive` of `b69eb709` makes again | 88,465 vectors: 88,465 match, 0 classified divergences, 0 unclassified differences | [`run-revision-18.log`](vectors/run-revision-18.log), [`run-revision-18.tsv.gz`](vectors/run-revision-18.tsv.gz) |
| Revision 19 | `d81c0c1`, clean, from 18:35Z, on the committed pin | 88,465 vectors: 88,465 match | [`run-revision-19.log`](vectors/run-revision-19.log), [`run-revision-19.tsv.gz`](vectors/run-revision-19.tsv.gz) |
| Its negative control | the same, with `--mutate unicode/casing-upper/0001` | 88,464 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-19.log`](vectors/run-negative-control-revision-19.log), [`run-negative-control-revision-19.tsv.gz`](vectors/run-negative-control-revision-19.tsv.gz) |

**What the vectors cannot show of this profile's corrections.** The area verdict names three: the casing
ceiling, the normalization ceiling and the segmenter's charge, each on the whole result (rows C12, N6 and G5).
None is visible in a vector of an ordinary length. Broiler.Unicode's reference-arm comparison holds the first
two against a copy of this profile's code, and part B's fuel parity holds the units.

**The record.** That comparison's first run found differences of Broiler.JS that this bundle's correspondence did
not name. It found none of this profile's. Rows C2, C3, C6 and N1 are revised in [`9b031e9`](correspondence.md),
each with a dated note, and so is JSD-0074's list of Broiler.JS's data-policy rows.

## 4. The cut-over (SJL-7.4)

[JSD-0075](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0075-the-profile-on-broiler-unicode.md) records
it. The changes, each building, with the tests it touches green:

| Commit | What it does |
|---|---|
| `093feb8` | The owner's generator reaches the architecture tests, test-only, under a dated revision of rule I1; rule I4 holds this family's Unicode pin to the owner's and each admitted assembly's Unicode version to the pin's |
| `0271008` | The lookups, lexical classes, folding, properties of strings and segmentation move to the owner. Format, the runtime, the compiler and the Intl data reference `Broiler.Unicode.Algorithms`, and the Intl data alone `Broiler.Unicode.Segmentation`. The closures of every JavaScript root in three modes ([`cutover/`](cutover/README.md)) and bundle JS-10-007's pack land with them |
| `b0ac6e2` | Normalization and case mapping move to the owner, and Soft_Dotted leaves the Intl data |
| `8aa88db` | Rules N18 and N23 read the Intl data assembly, and say where the moved code is held |
| `d4f7d54` | JSD-0075, the record |
| `8e9e411` | The committed feed holds the three packages as Broiler.Unicode's Publish workflow pushed them to nuget.org (JSC-310), from the commit the copies before it were packed from |

**This family's three corrections, and their consequence.**
- **C12 and N6.** The casing and normalization ceilings are checked on the whole result.
- **G5.** Segmentation charges every code point it reads.
- **The consequence:** a text longer than the ceiling is refused even when the operation would leave it unchanged.
  Such a text is reachable only by concatenation.

JSD-0075's section 6 lists each, with the cells it checked.

**At `8e9e411`, from a clean tree, with the packages as published:**
- **The shared vectors at corpus revision 19 match, every one:** 88,465 vectors, 88,465 match, 0 classified
  divergences, 0 unclassified differences
  ([`vectors/cut-over-revision-19.log`](vectors/cut-over-revision-19.log),
  [`vectors/cut-over-revision-19.tsv.gz`](vectors/cut-over-revision-19.tsv.gz)).
- **Part B's twins and probes keep their base figures.** Each of the ten wide twins and the three Unicode probes
  completes at the figure section 2's table gives, with its answer, and is refused on fuel at one less, in both
  forms. [`cutover/fuel-parity.log`](cutover/fuel-parity.log) is identical to the base's
  [`base/fuel-parity.log`](base/fuel-parity.log) line for line. Part B's after run is SJL-7.6's, at the commit
  this bundle names then. This is the same check, taken at the cut-over.
- **The tests pass:** 383 architecture tests and 473 contract tests, after a locked restore.

**Also run at the cut-over's own commits** (JSD-0075, section 7):
- **The vectors and the twins:** at `0271008`'s and `b0ac6e2`'s trees, with the copies packed locally.
- **The CLI acceptance suite:** 375 of 375.
- **The differential probes.** One probe fails, a `Math.cbrt` digit, which fails identically at the base and is
  not this area's.
- **A local test262 subset over the area's directories:** the same verdict as the base on every variant.

SJL-7.6 judges parts A and B; this section judges nothing.

## 5. The after arm (SJL-7.6)

*(Added 2026-10-17, at SJL-7.6. Sections 1 to 4 do not change. The exclusions are section 9, because rule H2
reads an exclusion's definition from a bundle's section 9 alone; sections 8 and 12 hold the vectors at corpus
revision 20 and part C's measurement, which were added to this bundle later the same day.)*

The after steps were added to [`collect-area.py`](collect-area.py) at SJL-7.6, when there was an after commit to
run them at. They write into [`after/`](after/) and run bundle `sjl-6-001`'s code wherever that bundle has the
step:
- the import of a hosted run and its completeness (that bundle's `collect.py`);
- the twins at the base's figures, the comparison, the (c') re-runs and the (e) wall-clock race (bundles
  `sjl-4-001` and `sjl-5-001`, through `sjl-6-001`);
- the closures and the architecture suite (bundle `sjl-3-001`'s).

Four steps are this bundle's own: `after-identity`, with this bundle's names; `closures`, which adds the digest
of each Native AOT image record; `simplification`, the Unicode area's search; and `measure`, part C, which no
bundle has had since `sjl-3-001`. `packages` is `sjl-6-001`'s with this bundle's name.
[`area-manifest.json`](area-manifest.json) now also lists [`after/`](after/) and [`cutover/`](cutover/README.md).

| | |
|---|---|
| Commit | `be0b610`, a clean checkout ([`after/identity.txt`](after/identity.txt)). It lands SJL-7.4's cut-over: its product sources are those of `8e9e411`, which carries the cut-over's code (`093feb8`, `0271008`, `b0ac6e2` and `8aa88db`, which JSD-0075 records in `d4f7d54`) with the committed feed re-pinned to Broiler.Unicode `0.1.0-preview.4` as published (JSC-310). Against `8e9e411` the identity lists 5 files outside the documents, all of them `be0b610`'s: this family's pin of the shared corpus at revision 20, with its two archives and its README, and the notices row that names the pin. `250d6b9`, between them, changes documents alone. None of the five is product code |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), with the same SHA-256 at the base and after (both identities) |
| Collector | [`collect-area.py`](collect-area.py) as this change commits it, run from this repository's checkout against clean checkouts of `be0b610` and, for the re-runs, `7b44dc7` |
| Taken | Part A's after run by the workflow, 2026-10-16 23:57Z to 2026-10-17 00:04Z. Locally, on 2026-10-17: `after-identity` and `after-twins` at 00:07Z; `after-import`, `after-complete` and `compare` at 00:09Z; `closures`, `rules`, `simplification` and `packages` from 00:09Z to 00:38Z, while a Broiler.JS vector run shared the machine until 00:31Z (none of those steps is timed); `rerun` and `wall-race` from 00:39Z to 00:42Z, the machine otherwise idle |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv` |
| Base side of the re-runs | a clean checkout of the base commit `7b44dc7`, its conformance root built there in Release |

## 6. Test262, both forms (rule part A)

**MET.** All four runs are complete as the rule states it. Every changed verdict is in class C4: two (b) rows and
twenty (c') rows. Every (c') row's re-runs are listed below, and every one agrees, so no row needed (e). No row is
in C1, C2 or C3, and the comparison classified every row it lists.

**The after run.** [38984038036](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38984038036),
attempt 1, was dispatched from `main` at `16a35b2` on 2026-10-16 at 23:57:02Z, with both forms and the commit
`be0b610` named by all forty digits. It completed at 00:04:17Z on 2026-10-17
([`after/hosted/run.txt`](after/hosted/run.txt)).
- The workflow and its driver at the dispatched ref are byte-identical to the rule commit's and to `be0b610`'s:
  neither `git diff 16a35b2 7b44dc7` nor `git diff 7b44dc7 be0b610` names them.
- The artifact `test262-evidence` is retained unchanged in [`after/hosted/`](after/hosted/).
  [`after/hosted/imported.txt`](after/hosted/imported.txt) records the digest of the zip as downloaded,
  `88f26d0f...`, which is the digest GitHub records.
- Every runner and the merging job ran image `ubuntu24` 20261012.334.1, as at the base (each form's
  `after/hosted/<run>.runners.txt`), and each form's runners built one assembly list.

| Run | Complete ([`after/completeness.log`](after/completeness.log)) | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants | the same, figure for figure | 12 variants differ, all (c') ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base: the floor was set in the win64 native form; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants | the same, figure for figure | 10 variants differ: 2 (b), 8 (c') ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

Both comparisons exit 0. **No variant moved between a pass, a failure, a refusal and an exhaustion**: in every
changed row both runs gave no answer, and they differ only in which allowance ran out first.

**No row is in C1, C2 or C3.**
- **C1.** No variant that failed at the base passes after. The corrections C12 and N6 change an answer only
  past the ceiling of 2^24 code units, and no test262 variant reaches it there.
- **C2.** No exhaustion verdict under `test/intl402/Segmenter/` moved, although G5 changed segmentation's
  charging units. In each form the directory's 158 variants give the same verdict at both commits, and none of
  them is exhausted: 154 of them passed and 4 failed in each run.
- **C3.** No implementation-defined verdict moved.

**Class (b).** `staging/sm/regress/regress-610026.js` [sloppy] and [strict], native: exhausted on the wall clock
at the base and on nested-load bytes after. It is a guest-load variant, which the rule admits under
`--exempt-guest-loads` without a re-run. Bundle `sjl-6-001` saw the same path race between the same two
allowances.

**Class (c') and its re-runs** ([`after/rerun-wide-bytecode.tsv`](after/rerun-wide-bytecode.tsv),
[`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their logs and every report). Each side was one
process of the conformance root over the rows' paths, alone, at `7b44dc7` and at `be0b610`, under the part's
manifest, form and allowances. The two re-runs of each pair, compared with `--same-form`, differ in no variant.

| Rows | Pair | Base -> after | Re-run at the base, after | Class |
|---|---|---|---|---|
| `built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` and `built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js`, each [sloppy] and [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-2-of-8.js` [strict], `-4-of-8.js` [strict], and `-8-of-8.js` [sloppy] and [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-5-of-8.js` [strict], and `-7-of-8.js` [sloppy] and [strict] | bytecode | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/TypedArray/sort_large_countingsort.js` [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-3-of-8.js`, `-5-of-8.js` and `-6-of-8.js`, each [sloppy] and [strict] | native | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-7-of-8.js` [sloppy] and [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |

**Class (e): none.** No (c') row's re-runs differ, so the `wall-race` step had no row to run and wrote no file;
its transcript says so for both pairs.

**What the rows say.** Every row is one of the long-running tests this family's earlier bundles saw race between
the wall clock and another allowance. Fifteen of the twenty (c') rows are `staging/sm/Date/dst-offset-caching-*`,
in the area SJL-6 moved; each exhausts fuel at both commits when it runs alone, as fuel is deterministic. **No
changed row is in the Unicode area**: none is under `intl402`, and none tests `normalize`, case mapping, a
property escape or an identifier.

**The exhaustion verdicts.** Every row exhausted on fuel at the base is exhausted on fuel after or, where the
whole run reached the wall clock first, exhausts fuel at both commits alone; and every row that completes at the
base completes after.

## 7. The fuel-parity twins and probes (rule part B)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log) covers every twin of `src/tests/forms/wide` and every
probe of `src/tests/forms/unicode` that [`base/fuel-parity.log`](base/fuel-parity.log) bisected: ten twins and
three probes, each in the bytecode form and in the native form under `x86-64-sysv`, 26 rows. Each completes at
its base figure with the base's answer, and is refused with `AllowanceExhausted on Fuel` at one less.
- **No charge moved**, inside the area or outside it, as JSD-0074 requires.
- The probes complete at section 2's figures with section 2's answers: `unicode/casing` at 328,460 with
  179280, `unicode/normalize` at 607,821 with 420519, and `unicode/properties` at 14,348,623 with 35050.
- The command-line root was built at `be0b610` for the step ([`after/build-cli.log`](after/build-cli.log)).

## 8. Engine-level vectors at corpus revision 20

Broiler.JS's corpus revision 20 (`a34b1f78`) retires Broiler.JS's 3,678 divergences of the Unicode area, which
its cut-over corrects; the Unicode area's five files are revision 18's again, and none of this family's
expectations changes. This family's pin moved to it in `be0b610`, by its README's procedure: the archive made
twice, byte-identical, its 42 files hashed after extraction. The conformance root of `be0b610`, built in Release
from a clean checkout, ran over the committed pin, by the commands of section 3's runs, outside the collector and
on the same machine:

| Run | Commit | Result | Files |
|---|---|---|---|
| Revision 20 | `be0b610`, clean, from 2026-10-16 23:56Z, on the committed pin | 88,465 vectors: 88,465 match, 0 classified divergences, 0 unclassified differences | [`run-revision-20.log`](vectors/run-revision-20.log), [`run-revision-20.tsv.gz`](vectors/run-revision-20.tsv.gz) |
| Its negative control | the same, from 2026-10-17 00:01Z, with `--mutate unicode/casing-upper/0001` | 88,464 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-20.log`](vectors/run-negative-control-revision-20.log), [`run-negative-control-revision-20.tsv.gz`](vectors/run-negative-control-revision-20.tsv.gz) |

Every vector matches after the cut-over, at revision 19 (section 4) and at revision 20. Broiler.JS's run on the
same revision, in its bundle of the same name, matches every vector but its number area's four classified
divergences.

## 9. Exclusions

*(Added 2026-10-17, at SJL-7.6.)*

| ID | Status | Exclusion |
|---|---|---|
| EX-130 | Open | **The Unicode code that left this component's assurance coverage.** The 29 members and types rule I5 registers for SJL-7.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), and the bodies of `NormalizeText`, `StringChangeCase` and `TransformCase` but for what JSD-0075 section 4 says each keeps, left the JavaScript family's sources at SJL-7.4 (JSD-0075 section 8). What computes them now is `Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation`, which the owner's own assurance component covers in the Broiler.Unicode repository (`src/assurance.config.json`: an annotation on every relevant declaration of its four projects, its manifest generated and checked in CI, rule SL8, convention 18). No rule of this component reads those units, their human review is the owner's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters of JSD-0075 section 4, the seams of its section 2 and the embedded tables stay in this family and stay covered. EX-125, EX-127, EX-128 and EX-129 say the same of the earlier areas |

## 10. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, with the packages as
published.** [`after/closures/`](after/closures/) holds, for ExecutionOnly, SliceCompiler, Conformance, Cli and
PolyglotCli:
- the closure under the JIT, trimming and Native AOT;
- the catalog each mode printed;
- the transcripts of every publish and run.

Every publish and every run exited as it should:
- the slice compiler's 646 checks pass in every mode, with three not run on this machine and claimed by
  nothing, as before;
- the execution-only root's 22 pass in every mode;
- every catalog is identical across its root's three modes;
- PolyglotCli's `nottext.wasm` is refused by design;
- each Native AOT publish leaves no managed assembly beside the image.

The Android root is still exclusion EX-126. Each root's closure and catalog name the same assemblies as at the
cut-over ([`cutover/closures/`](cutover/README.md)), whose closures the composition register's rows name:

| Root | `Broiler.Unicode.Algorithms` | `Broiler.Unicode.Segmentation` |
|---|---|---|
| ExecutionOnly | under the JIT and trimming | not carried |
| SliceCompiler | under the JIT and trimming | under the JIT and trimming |
| Conformance | under the JIT and trimming | under the JIT and trimming |
| Cli | under the JIT and trimming | not carried |
| PolyglotCli | under the JIT and trimming | not carried |

Every JIT and trimmed closure also carries `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`, as after SJL-6.

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the three admitted packages the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4` from Broiler.JS `d1e4b11`, and `Broiler.Unicode.Algorithms` and `.Segmentation` `0.1.0-preview.4` from Broiler.Unicode `6e14f93`. Each content hash is the allow-list's and the committed feed's: the packages as published (JSC-310). The two Unicode hashes differ from `cutover/closures/admitted-packages.json`'s, which records the copies packed locally, as section 4 and `cutover/README.md` say |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one; rule I3 read them in the rules run, which placed them in the tree for that run alone, and the log's first line names the same five digests |

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture suite, built
and run as the lane runs it, at `be0b610`: **383 tests, every one passed.**
- **Clause 9's rules:** I1, I2, I3 (which read the five image records), I4, I5, I6 and I7. I4 has two tests:
  the Unicode pin equals the owner's and every admitted assembly's version equals the pin's, and its witnesses
  (a pin differing by one hash, a library declaring another version) are reported.
- **Rule I5 holds the deletion list.** Its register holds the 29 members and types SJL-7.4 deleted, beside
  SJL-3.4's 34, SJL-4.4's 44, SJL-5.4's 44 and SJL-6.4's 53, and it passes with its witness.
- **Clause 10's rules:** N22's eight tests (the tables are what the owner's generator writes from this
  repository's pin, twice to the same bytes, under the budget, with four witnesses refused), N28's five (the same
  for the Intl assembly, the break table among them, with its witness), and N18's and N23's two each (the rule
  and its witness), re-pointed at SJL-7.4.
- **K3 and K4** pass on the closures the register's rows name.

**The budgets, restated by citation (clause 13).** Rule N22's cap is `UnicodeTableGenerator.BudgetBytes`, 300 x
1024 = 307,200 bytes, and rule N28's is `CldrTablesRuleTests.Budget`, 832 x 1024 = 851,968 bytes (JSD-0075
section 3).
- **N22:** the tables hold 291,891 bytes, Format's file 182,686 and the profile's 109,205.
- **N28:** the Intl assembly's tables hold 840,365 bytes, the break table's file included.

JSD-0075 section 3 gives those figures, and this bundle counted them again at `be0b610` the way the two rules
count. No generated file differs between `8aa88db` and `be0b610`. Both budget tests pass in
[`after/rules.log`](after/rules.log).

**The packages.** [`after/packages/`](after/packages/) is `eng/collect-js-packages.py` at `be0b610`, under its
own predeclared decision (`manifest.txt`), with the collector's transcript in
[`after/packages.log`](after/packages.log). It holds:
- the candidate pack of the whole solution, with every package's metadata and file list;
- a pristine JavaScript consumer that restores and runs, with upstream feeds unreachable, from a local feed
  holding the admitted shared packages;
- the rollback to the published `0.1.0-preview.5` set and forward again;
- the negative control;
- the consumer as Native AOT.

**Every step is as expected.** The consumer ran in each of its three runs, the negative control failed to
restore, and the Native AOT consumer ran. The nuspecs declare `Broiler.Unicode.Algorithms` `0.1.0-preview.4`
(the profile, Format, the compiler and the Intl data) and `Broiler.Unicode.Segmentation` `0.1.0-preview.4` (the
Intl data alone), beside `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`, which is what JSD-0075 section 1 and
bundle JS-10-007's baseline say. It does not replace that baseline, which rule N37 holds.

## 11. The simplification check

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log)
searches the simple name of every member rule I5 registers, 204 of them, as a declaration in the JavaScript
family's sources. Since SJL-7.6 the search also takes a type's declaration that ends its line.
- **Each of SJL-7.4's 29 is declared nowhere**: none of their simple names has a declaration in the family's
  C# sources.
- The earlier areas read as bundle `sjl-6-001` section 15 read them: 27 of SJL-3.4's 34, 35 of SJL-4.4's 44,
  all 44 of SJL-5.4's and 48 of SJL-6.4's 53 are declared nowhere, and the others are same-named members of
  other types.
- Rule I5 decides by the fully qualified name, and it passes (section 10).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's C# product sources (`src/`, every `tests/` excluded) in three parts, each with its
pattern:
- **host:** the platform's Unicode classification, casing, normalization and text elements: `char` and `Rune`'s
  `Is*` tests that read Unicode data, their casing, `GetUnicodeCategory`, `CharUnicodeInfo`, `Normalize`,
  `IsNormalized`, `StringInfo`, `TextInfo` and string casing;
- **algorithm:** the area's algorithms by their names and constants: Hangul composition's constants, the casing
  contexts, combining classes and decompositions, the break properties, the identifier and pattern properties,
  and the UCD files a hand-written copy would cite;
- **ranges:** tables of code-point ranges and inline range tests above ASCII.

The UTF-16 tests (`IsSurrogate` and its kin) are the text area's, and the ASCII tests (`IsAscii*`) read no Unicode
data, so neither is searched. **115 hits. None is a copy of what the area's deletion list or JSD-0074's X6
names.** Two of them read guest text through the platform's casing before an ASCII check, and the number
formatter holds two hand-written sets; each is below the table.

| Part | Hits | Where | What they are |
|---|---|---|---|
| host | 27 | the HyperV sample and `PicoVm` (5), `JavaScript.Cli`'s BigInt host checks (1), the conformance root (14), the WebAssembly harness (7) | casing of command-line words, enum names and booleans, and twelve calls of the conformance root's own `Suite.Normalize`, which normalizes a test's path. None reads guest text, and none is JavaScript |
| host | 2 | `JsRegExpMatcher` | `char.IsUpper` of a class escape's letter, one of `d D s S w W`: the ASCII marker JSD-0075 section 8 leaves to SJL-11. And `char.ToLowerInvariant` in `HexValue`, which only an ASCII hexadecimal digit reaches (`AllHex` or `IsHex` first) |
| host | 1 | `JsRealm.RegExp` | a comment naming what the code asked before SJL-7.4 (row W4) |
| host | 14 | `JsLocaleTag` (4), `JsRealm.DisplayNames` (6), `JsRealm.NumberFormat` (2), `JsRealm.Intl` (1), `JsRealm.Locale` (1, `CanonicalizeUValue`) | Intl's ASCII case conversion of text already held to ASCII. It is a tag `JsLocaleTag.Parse` accepted (it refuses any character but ASCII letters, digits and `-` before it lowers), a region, script, calendar or currency code after its well-formedness check, or an option value a constructor held to the `type` grammar or to a list. On ASCII the platform's invariant casing is ECMA-402's ASCII case conversion |
| host | 1 | `JsRealm.Locale`, the `variants` option of `Intl.Locale` | **lowered by the platform before its check**: below |
| host | 1 | `JsTemporalParser`, a duration's designator | **upper-cased by the platform from guest text**: below |
| algorithm | 38 | `SliceTokenizer` (7), `JsRegExpMatcher` (11), `JsRealm.RegExp` (4), `JsRealm.String` (6), `JsUnicode` (2), `JsIntlData` (1), `JsArm64Assembler` (2), `JsArm64GoldenChecks` (1), `JsCollationData` (1), and the slice compiler's `IntlChecks` (3) | documentation naming a property, a UCD file or a decomposition. Also the instruction encoders' field "decomposition" (3); the provider's range test over the owner's `UnicodeTable` numbers; collation's call of the owner's `CombiningClass` through `JsUnicode`; and the slice compiler's checks reading the UCD break test files as test data |
| algorithm | 9 | `JsUnicodeTables.g.cs` | the names of the tables the owner's generator writes, which rule N22 holds |
| algorithm | 2 | `JsCldrTables.g.cs` | CLDR's display name of the script `Hang`, "Hangul" |
| algorithm | 7 | `JsNumberFormatter` | `IsPatternWhiteSpace` and `IsDefaultIgnorable`, their uses and comments: below |
| ranges | 5 | `JsRegExpMatcher` (2), `JsCollationData` (1), `JsRealm.String` (1), the WebAssembly harness (1) | surrogate ranges, UTF-16's and the text area's |
| ranges | 1 | `JsRegExpMatcher` | U+2028 and U+2029, the language's line terminators, added to a set |
| ranges | 1 | `JsCollationData` | the unified ideographs' core blocks, for collation's implicit weights, which are SJL-17's |
| ranges | 1 | `JsEngine.Eval` | the control characters a source may not begin with, the family's own refusal |
| ranges | 5 | `JsNumberFormatter` | `IsDefaultIgnorable`'s ranges: below |

**Two hits read guest text through the platform's casing before an ASCII check.** Both are the same at `7b44dc7`
and `be0b610`, so SJL-7 did not change them, and neither JSD-0074 nor the area's deletion list names them. They
are recorded here and judged by no clause. Each was probed through the conformance root of both commits with a
scratch test, which this bundle does not retain.
- **`new Intl.Locale("en", { variants: "Kaaaa" })`.** `JsRealm.Locale` lowers the option with
  `ToLowerInvariant` before it checks each variant. The platform lowers U+212A KELVIN SIGN to `k` in either
  globalization mode, so the text passes the check. The invocation then ends with
  `ProfileFault/ProfileContractViolation` at both commits, where ECMA-402's ASCII-lowercase leaves U+212A as it
  is and the constructor throws a `RangeError`. The same tag given as the first argument is refused with the
  `RangeError`, and an ASCII variant through the option is accepted.
- **`Temporal.Duration.from("PT1ſ")`.** `JsTemporalParser` upper-cases a duration's designator with
  `char.ToUpperInvariant`. Under the invariant globalization every JavaScript root of this repository sets,
  U+017F LATIN SMALL LETTER LONG S stays itself, and the probe throws the `RangeError` at both commits. Under ICU
  the platform maps it to `S`, so an embedder that runs the profile without invariant globalization would read it
  as the seconds designator.

**The number formatter's two hand-written sets.** `JsNumberFormatter.IsPatternWhiteSpace` is Pattern_White_Space,
eleven code points, which the range pattern's separator is tested against. `IsDefaultIgnorable` is "ICU's
default-ignorable set", which trims a formatted part. Both read CLDR's pattern text and the formatter's own
output, not guest text. They are Unicode classification written out in the family outside the owner, which
JSD-0074's correspondence did not list and no deletion list names. No clause of SJL-7 reads the duplicate
search.

## 12. The measurement (rule part C)

**NOT MET, on clause 3.** [`measurement/`](measurement/README.md) holds the run, a sub-bundle of its own evidence
class with its own README, manifest and hashes, which [`area-manifest.json`](area-manifest.json) does not list.
`collect-area.py --only measure` took it on 2026-10-17, its lanes from 00:54:00Z to 00:58:31Z, on the base's
machine with no other build, test or run beside it.

| Clause | On this run |
|---|---|
| 1. Every lane's condition passes | **Holds**: no answer check differs from arm B's, in 72 checks, and no timed child's answer does |
| 2. Every effective-configuration line identical | **Holds**: one line in every timed child and check |
| 3. D <= N for every workload and form | **Fails for two of six.** It holds for normalization, casing and identifiers in the bytecode form and for normalization in the native form. It fails for casing in the native form, D = +55.1 ms against N = 25.6 ms, and for identifiers in the native form, D = +39.5 ms against N = 14.8 ms |

The rule has no tolerance but the A/A lane and allows no re-run of a clause that fails, so this run stands. Under
the rule's reading, **JSD-0031's fifth reason, the owner's runtime cost, stays open for this engine for casing and
identifier scanning in the native form.** The sub-bundle's README gives every figure, and reads the two failures
without deciding anything. In both, arm A's seven runs spread widely, and arm A-again's median also lies above arm
B's by more than the floor. A second run would be a new measurement under a dated revision of the rule, which is
weaker evidence, and taking one is the owner's choice.

## 13. The Broiler.VM clauses

| Clause | What it asks | Where | State |
|---|---|---|---|
| 9 | Rules I1 to I7 hold, and rule I4 agrees with the pin | section 10, [`after/rules.log`](after/rules.log) | **Met** at `be0b610`: 383 of 383, I1 to I7 among them; I4 holds the pin equal to the owner's and every admitted version to the pin's, with its witnesses; I5's register holds SJL-7.4's 29 |
| 10 | Rules N22 and N28 regenerate the embedded tables with the owner's generator from Broiler.VM's pin, which rule I4 holds equal to the owner's; N18 and N23 are re-pointed; each has its witnesses | section 10 | **Met** at `be0b610`: N22's eight tests, N28's five, N18's two and N23's two pass, each with its witness |
| 11 | test262 in both forms meets the area rule | sections 6 and 7 | **Met** on parts A and B: part A MET in both pairs, every change a (b) row or a (c') row whose re-runs agree; part B MET; the exhaustion verdicts hold. Part C, the rule's measurement, answers the evidence item and is NOT MET on clause 3 (section 12) |
| 12 | Every root is published and run in three modes | section 10 | **Met** for every root this machine can publish, at `be0b610`, with the packages as published; Android is EX-126 |
| 13 | N22's and N28's size budgets hold for the regenerated tables, and the bundle restates them by citation | section 10 | **Met**: 291,891 of 307,200 bytes and 840,365 of 851,968, both budget tests passing at `be0b610` |

**Part C's measurement** (section 12), which is no Broiler.VM clause of its own, is NOT MET on clause 3: the
evidence item "each engine's measurement for `normalize`, casing and identifier scanning under predeclared rules" is
taken, and it leaves JSD-0031's fifth reason open for this engine for casing and identifiers in the native form. The owner's clauses, 1 to 4, are judged in
Broiler.Unicode's bundle of the same name, and Broiler.JS's, 5 to 8, in Broiler.JS's. None is accepted: review is
deferred, and SJL-7 may not meet its gate before SJL-3 meets SJL-3's.
