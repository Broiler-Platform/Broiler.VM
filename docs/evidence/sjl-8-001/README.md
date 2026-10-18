<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-8-001 - the time-zone and calendar area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-8 of the shared JavaScript libraries programme,
[`sjl-8-time-zones-and-calendars.md`](../../javascript-shared-libraries/milestones/sjl-8-time-zones-and-calendars.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-8.1 before any code of the milestone
existed.

**Records:** [JSD-0076](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0076-time-zone-and-calendar-data-move-to-the-data-home.md),
whose input is [`correspondence.md`](correspondence.md), with every probe and both engines' answers in
[`correspondence-runs/`](correspondence-runs/).

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State, 2026-10-18, at SJL-8.7: the after arm is taken at `0a52e03` (sections 5 to 14). Parts A, B and C of
[`decision-rule-area.md`](decision-rule-area.md) are MET as written, with no revision** (sections 6 to 8).
- **Part A:** every changed test262 verdict in either form is a race between the wall clock and another allowance,
  which class C4 (c') admits, and each agrees at both commits run alone. No verdict moved between a pass, a
  failure and an exhaustion, and no row is a correction.
- **Part B:** each of the seven named checks prints the base's line, and no file of the reference datasets changed.
- **Part C:** every wide twin completes at its base figure with its answer, and is refused one below, in both
  forms.
- **Clauses 12 to 17 are met** on this bundle (section 14). The engine-level vectors at corpus revision 23 all
  match at the after commit (section 10), and the Intl data budget holds, unchanged (section 12).
- **Nothing is accepted.** Review is deferred, and SJL-8 may not meet its gate before SJL-3 meets SJL-3's.

*(Revised 2026-10-18, at SJL-8.7. The state read: "**State:** rules, correspondence, the base, the vectors and the
cut-over, 2026-10-17. **Nothing here is a verdict.**", with the bullets "The base of all three parts was taken at
the commit that carries the rule, `431c20e`, before any code of SJL-8 landed (section 2).", "SJL-8.5's cut-over is
in place, with the checks it ran (section 4)." and "The after runs are SJL-8.7's.")*

The data home's clauses and Broiler.JS's are judged in Broiler.JS's bundle of the same name. This bundle reads
none of its runs.

## 1. SJL-8's area rule and its correspondence (SJL-8.1)

**The correspondence.**
- It was read from code in both engines at the revisions it names. It numbers every difference the area verdict
  classifies: Z for the tz database, A for the zone algorithms and K for the calendar tables.
- Every behavioural cell its rows rest on was run in both engines: this family through the conformance root, and
  Broiler.JS through its shell.
- **The engines' zone algorithms agree on every input inside the data both share.** Every difference is one of
  these:
  - the data's policy under SJL-D-20 or SJL-D-21: the release, backzone, extrapolation after 2037, `Factory`, the
    primaries and the calendar sources;
  - one of eight Broiler.JS corrections: lookup by invariant upper-casing, ISO strings as a formatter's zone, the
    region lists, non-ASCII offset digits, an unvalidated host zone, `chinese` 2057, the far lunisolar dates and the
    Persian seams at .NET's bounds;
  - the host zone, which SJL-D-19 keeps each engine's.
- **This family has no correction of its own in the area.**

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It admits no
  class C2, C3 or C5: this family's charging, release, policy and calendar sources do not change. A correction the
  run finds is admitted only as a finding the correspondence missed.
- **Part B** holds this family's reference datasets unchanged:
  - `timezones.js` and the Temporal programs, run by the slice compiler's checks at both commits;
  - their programs, answers and divergences, byte for byte.
- **Part C** holds the wide twins to their base figures, with no class. The command-line root reaches none of the
  area's code, so these twins show only that no charge moved outside it.

**The release both engines adopt** is tzdb 2026e, the one this family's pin records. JSD-0076 pins it by its dated
record, from IANA's `NEWS` read on 2026-10-17.

## 2. The base (SJL-8.2)

[`collect-area.py`](collect-area.py) took it. It reuses bundle `sjl-7-001`'s area collector, and through it bundle
`sjl-6-001`'s import of a hosted run and bundle `sjl-5-001`'s twin bisection, over the wide twins. It adds part B's
step: the slice compiler's checks, with the digest of every dataset file. It judges nothing.

| | |
|---|---|
| Commit | `431c20e`, the commit that carries the rule, from a clean checkout ([`base/identity.txt`](base/identity.txt)). Against the default branch after SJL-7's merges, `9508bd5`, it changes no file outside the documents, so its product sources are `9508bd5`'s, as the rule says |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), `sha256=dbd07094...` in the identity |
| Taken | 2026-10-17: part A's base by the workflow from 13:50Z to 14:13Z; parts B and C locally from 13:55Z, on the programme's local machine (`linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv`), while other work ran beside it. Nothing in parts B and C is timed |

**Part A's base.** [39038214808](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39038214808),
attempt 1, was dispatched from `main` at `bb3e8aa` on 2026-10-17 at 13:50:25Z. It ran with both forms and the commit
`431c20e`, named by all forty digits, and completed at 14:12:42Z ([`base/hosted/run.txt`](base/hosted/run.txt)).
- The workflow and its driver at the dispatched ref are byte-identical to the rule commit's: `git diff bb3e8aa
  431c20e` names neither.
- The artifact `test262-evidence` is retained unchanged in [`base/hosted/`](base/hosted/).
  [`base/hosted/imported.txt`](base/hosted/imported.txt) records the digest of the zip as downloaded, which is the
  digest GitHub records.
- Every runner and the merging job ran image `ubuntu24` 20261012.334.1.

| Run | Complete ([`base/completeness.log`](base/completeness.log)) | Totals |
|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants |
| `wide-native` | exit 4, a re-base, as the rule admits; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants |

The pass, fail, exhausted and skipped totals are bundle `sjl-7-001`'s after run's, figure for figure. The exhausted
rows split differently between the wall clock and fuel, as hosted runs do. Part A's comparison reads them row by
row, and its class C4 (c') covers that split.

**Part B's base** ([`base/datasets/`](base/datasets/)).
- **The method:** the slice compiler, built in Release at the base commit, ran `--checks --verbose`.
  - [`checks.log`](base/datasets/checks.log) keeps its whole output, with the digest of the binary.
  - [`named-checks.txt`](base/datasets/named-checks.txt) keeps the line of each check the rule names.
  - [`files.sha256`](base/datasets/files.sha256) keeps the digest of every file under `src/tests/cldr/timezones/`
    and `src/tests/temporal/`, 26 files.
- **The result:** every named check passes. The whole run passed 648 checks. Three ran nowhere: the
  `x86-64-win64` baseline rows, which this machine cannot enter.

| Check | Lines |
|---|---|
| `tzdb/time-zone-offsets-match-icu` | 610, 51 of them as the divergences name |
| `temporal/t1/temporal-matches-the-reference-polyfill` | 1,629, none as divergences |
| `temporal/t2/intl-over-temporal-matches-the-reference-polyfill` | 1,233, 82 as divergences |
| `temporal/t3/calendars-match-the-reference-polyfill` | 3,999, 70 as divergences |
| `temporal/t3/calendar-conversions-match-icu4x` | 2,055 conversions |
| `temporal/t4/calendar-formats-match-the-reference-polyfill` | 5,951, 1,395 as divergences |
| `temporal/t5/zone-names-match-the-reference-polyfill` | 2,713, 275 as divergences |

**Part C's base** ([`base/fuel-parity.log`](base/fuel-parity.log)).
- **The method:** each wide twin's smallest completing fuel allowance was bisected under the bytecode form. Then
  both forms ran at that figure and at one less.
- **The result:** every twin completes at its figure and is refused on fuel at one less, in both forms, and both
  forms complete with the same answer.
- **Every figure and answer equals bundle `sjl-7-001`'s base.**

| Twin | Figure | Answer |
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

[`area-manifest.json`](area-manifest.json) lists the digest of every file of the base, the collector, the rule and
the correspondence. `correspondence-runs/` is the correspondence's own record and is not listed.

## 3. Engine-level vectors on the time-zone area (SJL-8.4)

Broiler.JS's corpus revision 21 (`09fa9ac5`) adds the time-zone area: 3,969 vectors in seven files under
`conformance/vectors/time-zones/`.
- **Where they come from.** Every expected value is `scripts/conformance/tzref.py`'s, an independent reference. It
  compiles the data home's pinned tzdb 2026e with the host's `zic -b fat`, reads the compiled files with its own
  reader, and applies SJL-D-20's policy. It shares no code with the generator or the shared library.
- **What they cover.** Instants on both sides of every policy difference and correction JSD-0076 lists, and a
  sample of ordinary offsets, transitions, gaps, overlaps and starts of day.
- **Two kinds are implementation-defined:**
  - the name a formatter resolves a non-primary identifier to (row Z8), whose expectation for this profile is the
    primary;
  - a region's zones (row Z12).

Revision 22 (`4ce20ac7`) classifies Broiler.JS's first run: 1,161 differences, every one under a row the area
verdict names. It holds none of this profile's.

Revision 23 (`2d496a55`) retires those 1,161 fields, which Broiler.JS's cut-over (SJL-8.6) corrects. The area's seven
files are revision 21's again, and this profile's expectations are unchanged.

The collector's `vectors` step, bundle `sjl-6-001`'s, ran the conformance root in Release.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 21 | At `431c20e`, the base, from 16:57Z, with the scratch pin [`vectors/revision-21.pin`](vectors/revision-21.pin) beside a scratch copy of its archive, which `git archive` wrote twice, byte-identical | 92,434 vectors: 92,434 match | [`run-revision-21.log`](vectors/run-revision-21.log), [`run-revision-21.tsv.gz`](vectors/run-revision-21.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 92,433 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-21.log`](vectors/run-negative-control-revision-21.log) |
| Revision 22 | At `15da5fd`, clean, on the committed pin, which moves to revision 22 there | 92,434 vectors: 92,434 match, 0 classified divergences, 0 unclassified differences | [`run-revision-22.log`](vectors/run-revision-22.log), [`run-revision-22.tsv.gz`](vectors/run-revision-22.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 92,433 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-22.log`](vectors/run-negative-control-revision-22.log) |
| Revision 23 | At `458c10a`, clean, on the committed pin, which moves to revision 23 there | 92,434 vectors: 92,434 match, 0 classified divergences, 0 unclassified differences | [`run-revision-23.log`](vectors/run-revision-23.log), [`run-revision-23.tsv.gz`](vectors/run-revision-23.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 92,433 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-23.log`](vectors/run-negative-control-revision-23.log) |

**This profile matches every time-zone vector, each of its own `per-engine` expectations included.** The vectors
confirm the correspondence's reading that this family has no correction of its own in the area. Both engines' runs
on the shared corpus disagree only where JSD-0076 says Broiler.JS's policy or code differs.

The pin is at revision 23 (`src/tests/vectors/pins/vectors.pin`) since `458c10a`, and was at revision 22 from `15da5fd`.
Each time its archive is taken twice, byte-identical, and its 49 files are hashed from an extracted copy.

## 4. The cut-over (SJL-8.5)

[JSD-0077](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0077-the-profile-on-the-shared-time-zones-and-calendars.md)
records it. The changes, each building, with the tests it touches green:

| Commit | What it does |
|---|---|
| `4df595b` | `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5`, packed at Broiler.JS `2d37a38a` and not yet published, is admitted from the committed feed in place of `0.1.0-preview.4` |
| `f09a0dd` | The package baseline is bundle JS-10-009's (JSC-312), as rule N37 asks |
| `c5943b1` | The data home's generator, `Broiler.ECMAScript.Generation` `0.1.0-preview.5`, reaches the architecture tests, test-only. The tz compiler, both table generators and their pin readers leave this repository. Rules N29 to N32 regenerate the tables with the home's generator from this repository's pins and are registered. Rule I4 holds the tz pin, the calendar pin and CLDR's `bcp47/timezone.json` to the home's by digest, and the package's six version constants to this repository's pins |
| `186a1d3` | The profile's zones and calendar years are the shared library's: `TimeZoneDatabase` and `CalendarTables` over the Intl data's tables, `ZonedTime`, `OffsetTimeZones` and `CalendarYears`. Each status maps to the RangeError this family threw before, with the same message |
| `8613309` | Intl data that does not carry the new tz-release table reads its zones as before, with a slice-compiler check that says so |
| `eed568a` | Rule I5 registers the area's 24 deleted members and types, and the notices say where the ICU4X port went |
| `05f99db` | JSD-0077, the record |

**This family has no correction of its own in the area** (JSD-0076), and the cut-over admits none.

**Checked at the product tree of `8613309`.** `eed568a` and `05f99db` change no product file. The files are in
[`cutover/`](cutover/):
- **The generated tables.**
  - Regenerated with the home's generator in write mode, both files stay byte-identical to the base's.
  - A changed byte of either is refused, by N30 or by N32 ([`cutover/n30-negative-control.log`](cutover/n30-negative-control.log),
    [`cutover/n32-negative-control.log`](cutover/n32-negative-control.log)).
- **Part B's reference datasets.**
  - Each of the seven named checks prints the line the base's [`base/datasets/named-checks.txt`](base/datasets/named-checks.txt)
    gives ([`cutover/named-checks.txt`](cutover/named-checks.txt)).
  - No file under `src/tests/cldr/timezones/` or `src/tests/temporal/` changed.
  - The slice compiler passes 649 checks: the base's 648 and `8613309`'s one.
- **The shared vectors at revision 22 match, every one:** 92,434 vectors, 92,434 match, 0 classified divergences,
  0 unclassified differences ([`vectors/run-cut-over-revision-22.log`](vectors/run-cut-over-revision-22.log)).
- **Part C's twins keep their base figures.** Each of the ten wide twins completes at its base figure, with its
  answer, and is refused on fuel at one less, in both forms. The rows of
  [`cutover/fuel-parity.log`](cutover/fuel-parity.log) are the base's [`base/fuel-parity.log`](base/fuel-parity.log)
  line for line.
- **A local test262 subset, bytecode form,** over the area's directories and `Date`: 7,696 files and 15,330 variants,
  every one with the hosted base's verdict ([`cutover/test262-subset.txt`](cutover/test262-subset.txt)).
  - On both sides, 15,240 variants passed, 12 failed, 16 exhausted their allowance and 62 were skipped.
  - The sixteen exhausted variants are `staging/sm/Date/dst-offset-caching`'s. They read no zone, because this
    profile's local time is UTC.
  - Eleven of them ran out of wall clock on this machine where the base ran out of fuel. With a wall clock of 120
    seconds, all sixteen exhaust on fuel.
- **The tests pass:** the architecture tests, rule I4's two new tests among them, and the contract tests, with
  JSD-0077's figures. A full Release build has no warning.

JSD-0077's section 7 gives each in full, and its departures name the edges the cut-over leaves:
- the region lists asked code by code;
- the lunisolar month counts at the ±300,000-year bound;
- the generated files' stale header text, kept because the bytes must not change.

**The published packages.** Both packages were published to nuget.org later the same day, from Broiler.JS
`a04cd44`, whose library code is `2d37a38`'s, and the committed feed now holds them as published
([JSC-313](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-313)). The checks above used
the copies packed from `2d37a38`. With the published packages:
- a locked restore with an empty package cache succeeds, and the solution builds with no warning;
- the architecture tests and the contract tests pass;
- the conformance root's run on revision 22 gives a report identical byte for byte to
  `run-revision-22.tsv.gz`'s, and its negative control's report is identical to
  `run-negative-control-revision-22.tsv.gz`'s.

Neither run is retained, since neither differs.

SJL-8.7 judges parts A to C; this section judges nothing.

## 5. The after arm (SJL-8.7)

*(Added 2026-10-18, at SJL-8.7. Sections 1 to 4 do not change. The exclusions are section 9, because rule H2
reads an exclusion's definition from a bundle's section 9 alone.)*

The after steps were added to [`collect-area.py`](collect-area.py) at SJL-8.7, when there was an after commit to
run them at. They write into [`after/`](after/) and run bundle `sjl-7-001`'s code wherever that bundle has the
step:
- the import of a hosted run, its completeness and the comparison (bundle `sjl-6-001`'s);
- the twins at the base's figures, the (c') re-runs and the (e) wall-clock race (bundles `sjl-4-001` and
  `sjl-5-001`);
- the closures, with the digest of each Native AOT image record (bundle `sjl-7-001`'s), and the architecture
  suite (bundle `sjl-3-001`'s).

Three steps are this bundle's own:
- `after-identity`, with this bundle's names;
- `after-datasets`, part B's after run, beside its comparison with the base;
- `simplification`, the area's search, with the lines each C# file gained and lost.

`packages` is `sjl-6-001`'s with this bundle's name. [`area-manifest.json`](area-manifest.json) now also lists
[`after/`](after/), and names the after commit.

| | |
|---|---|
| Commit | `0a52e03`, the default branch after SJL-8.6's merge, a clean checkout ([`after/identity.txt`](after/identity.txt)). Its product sources are those of `88f1081`, which carries SJL-8.5's cut-over (`4df595b`, `f09a0dd`, `c5943b1`, `186a1d3`, `8613309` and `eed568a`, which JSD-0077 records in `05f99db`) with the committed feed pinned to `0.1.0-preview.5` as published (JSC-313). Against `88f1081` the identity lists 5 files outside the documents, all of them `458c10a`'s: this family's pin of the shared corpus at revision 23, with its two archives and its README, and the notices row that names the pin. None is product code |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), with the same SHA-256 at the base and after (both identities) |
| Collector | [`collect-area.py`](collect-area.py) as this change commits it, run from this repository's checkout against clean checkouts of `0a52e03` and, for the re-runs, `431c20e` |
| Taken | Part A's after run by the workflow, 2026-10-17 from 23:07Z to 23:21Z. Locally, on 2026-10-17: `after-identity`, `after-twins` and `after-datasets` from 23:16Z; `vectors` from 23:22Z; `after-import`, `after-complete` and `compare` at 23:22Z; `simplification` at 23:34Z. On 2026-10-18: `closures` from 00:02Z (its second run, section 11) and `rules` to 00:27Z; `rerun` and `wall-race` from 00:28Z to 00:31Z, with no other build, test or run on the machine; `packages` from 00:31Z. Broiler.JS's after runs shared the machine for part of the time before 00:28Z, and none of the steps they overlapped is timed |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv` |
| Base side of the re-runs | a clean checkout of the base commit `431c20e`, its conformance root built there in Release |

## 6. Test262, both forms (rule part A)

**MET.** All four runs are complete as the rule states it. Every changed verdict is in class C4 (c'): five rows in
the bytecode pair and thirteen in the native pair. Every (c') row's re-runs are listed below, and every one agrees,
so no row needed (e). No row is in C1, and the comparison classified every row it lists.

**The after run.** [39077773716](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39077773716),
attempt 1, was dispatched from `main` at `0a52e03` on 2026-10-17 at 23:07:16Z. It ran with both forms and the commit
`0a52e03`, named by all forty digits, and completed at 23:21:31Z ([`after/hosted/run.txt`](after/hosted/run.txt)).
- The workflow and its driver at the dispatched ref are byte-identical to the rule commit's: `git diff 431c20e
  0a52e03` names neither.
- The artifact `test262-evidence` is retained unchanged in [`after/hosted/`](after/hosted/).
  [`after/hosted/imported.txt`](after/hosted/imported.txt) records the digest of the zip as downloaded,
  `47991246...`, which is the digest GitHub records.
- Every runner and the merging job ran image `ubuntu24` 20261012.334.1, as at the base (each form's
  `after/hosted/<run>.runners.txt`), and each form's runners built one assembly list.

| Run | Complete ([`after/completeness.log`](after/completeness.log)) | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants | the same, figure for figure | 5 variants differ, all (c') ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants | the same, figure for figure | 13 variants differ, all (c') ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

Both comparisons exit 0, and each says every difference is in an admitted class. **No variant moved between a
pass, a failure, a refusal and an exhaustion**: in every changed row both runs gave no answer, and they differ
only in which allowance ran out first. So every variant skipped at the base is skipped after, the cases JSD-0054
skips among them, as the rule expects.

**No row is in C1.** No variant that failed at the base passes after, so the run finds no correction the
correspondence missed. Classes C2, C3 and C5 are not in the rule, and no row would need them: no exhaustion on fuel
became an answer or a failure, and no verdict moved because the data moved.

**Class (c') and its re-runs** ([`after/rerun-wide-bytecode.tsv`](after/rerun-wide-bytecode.tsv),
[`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their logs and every report). Each side was one
process of the conformance root over the rows' paths, alone, at `431c20e` and at `0a52e03`, under the part's
manifest, form and allowances.

| Rows | Pair | Base -> after | Re-run at the base, after | Class |
|---|---|---|---|---|
| `annexB/built-ins/RegExp/RegExp-trailing-escape-BMP.js` [strict] | bytecode | Exhausted/WallClock -> Exhausted/NestedLoadFanOut | Exhausted/NestedLoadFanOut, Exhausted/NestedLoadFanOut | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-3-of-8.js` [sloppy] and [strict], and `-5-of-8.js` [sloppy] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-4-of-8.js` [sloppy] | bytecode | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js` [strict] | native | Exhausted/WallClock -> Exhausted/NestedLoadFanOut | Exhausted/NestedLoadFanOut, Exhausted/NestedLoadFanOut | **C4 (c')**, the re-runs agree |
| `built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` and `built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js`, each [sloppy] and [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-1-of-8.js`, `-2-of-8.js`, `-6-of-8.js` and `-8-of-8.js`, each [sloppy] and [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |

The two re-runs of each pair, compared with `--same-form`, differ in no variant. They ran from 00:28Z to 00:31Z on
2026-10-18, with no other build, test or run on the machine.

**Class (e): none.** No (c') row's re-runs differ, so the `wall-race` step had no row to run and wrote no file. Its
transcript says so for both pairs.

**What the rows say.** Every row is one of the long-running tests this family's earlier bundles saw race between
the wall clock and another allowance. Twelve of the eighteen are `staging/sm/Date/dst-offset-caching-*`, which
read no zone, because this profile's local time is UTC (section 4). The others are the two `RegExp` escape tests,
which race the wall clock against the nested-load fan-out, and `decodeURI` and `decodeURIComponent`'s long loops.
**No changed row is in the area**: none is under `built-ins/Temporal`, `intl402/Temporal`, `intl402/DateTimeFormat`
or `intl402/Locale`, and none reads a zone, an offset or a calendar.

**The exhaustion verdicts.** Every row exhausted on fuel at the base is exhausted on fuel after or, where the
whole run reached the wall clock first, exhausts fuel at both commits alone. Every row that completes at the base
completes after.

## 7. The reference datasets (rule part B)

**MET.** The slice compiler, built in Release at `0a52e03` from the clean checkout, ran `--checks --verbose`
([`after/datasets/checks.log`](after/datasets/checks.log)).
- **Each of the seven named checks prints the base's line**, word for word:
  [`after/datasets/named-checks.txt`](after/datasets/named-checks.txt) against
  [`base/datasets/named-checks.txt`](base/datasets/named-checks.txt). Every line of every program answers as its
  retained answer does, with the same lines named by its divergences: section 2's table, unchanged.
- **No dataset file changed.** [`after/datasets/files.sha256`](after/datasets/files.sha256) is byte-identical to
  the base's, over the same 26 files, and `git diff --name-only 431c20e 0a52e03` names no file under
  `src/tests/cldr/timezones/` or `src/tests/temporal/`.
- [`after/datasets/compare.txt`](after/datasets/compare.txt) records the three comparisons, line by line.

The slice compiler passes 649 checks: the base's 648 and `8613309`'s one, which checks that Intl data without the
tz release reads its zones as before (section 4). It leaves three not run, the `x86-64-win64` baseline rows, which
this machine cannot enter.

## 8. The fuel-parity twins (rule part C)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log) covers every twin of `src/tests/forms/wide` that
[`base/fuel-parity.log`](base/fuel-parity.log) bisected: ten twins, each in the bytecode form and in the native
form under `x86-64-sysv`, 20 rows.
- Each completes at its base figure with the base's answer, and is refused with `AllowanceExhausted on Fuel` at one
  less. The figures and answers are section 2's table.
- **No charge moved outside the area**, as JSD-0076 requires. Every figure, verdict and answer is also section 4's
  cut-over check's.
- The command-line root was built at `0a52e03` for the step ([`after/build-cli.log`](after/build-cli.log)).

## 9. Exclusions

*(Added 2026-10-18, at SJL-8.7.)*

| ID | Status | Exclusion |
|---|---|---|
| EX-131 | Open | **The time-zone and calendar code that left this component's assurance coverage.** The 24 members and types rule I5 registers for SJL-8.5 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), with the bodies of the zone operations and `ResolveTimeZone`'s core that JSD-0077 section 8 names, left the JavaScript family's sources at SJL-8.5. The tz compiler, both table generators and their pin readers left the architecture tests (JSD-0077 section 8). What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Time`, and the test-only `Broiler.ECMAScript.Generation`. The home's own assurance component covers both in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json` lists both projects: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters of JSD-0077 section 4, its seams of section 2, the pins, the archives and the two generated files stay in this component and stay covered. EX-125, EX-127, EX-128, EX-129 and EX-130 say the same of the earlier areas |

## 10. Engine-level vectors at the after commit

**Every vector matches.** The conformance root of `0a52e03`, built in Release from the clean checkout, ran over
this family's pin, corpus revision 23 (`2d496a55`), with the collector's `vectors` step:
- **92,434 vectors, 92,434 match, 0 classified divergences, 0 unclassified differences**
  ([`vectors/after-revision-23.log`](vectors/after-revision-23.log)). Its report, decompressed, is byte-identical
  to section 3's [`vectors/run-revision-23.tsv.gz`](vectors/run-revision-23.tsv.gz), taken at `458c10a`. The
  compressed files differ in their gzip headers alone.
- **The negative control** changes the expected value of `text/trim/0001` in memory. The run reports that vector,
  and only it, as an unclassified difference
  ([`vectors/after-negative-control-revision-23.log`](vectors/after-negative-control-revision-23.log)). Its
  report, decompressed, is byte-identical to section 3's too.

The step rebuilt the conformance root at `0a52e03`, and its build log replaces section 3's in
[`vectors/build-conformance.log`](vectors/build-conformance.log). The earlier one is in this repository's history.

## 11. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, with the packages as
published.** [`after/closures/`](after/closures/) holds, for ExecutionOnly, SliceCompiler, Conformance, Cli and
PolyglotCli:
- the closure under the JIT, trimming and Native AOT;
- the catalog each mode printed;
- the transcripts of every publish and run.

Every publish and every run exited as it should:
- the slice compiler's 649 checks pass in every mode, with three not run on this machine and claimed by nothing,
  as part B's in-tree run gives (section 7);
- the execution-only root's 22 checks pass in every mode;
- every catalog is identical across its root's three modes;
- PolyglotCli's `nottext.wasm` is refused by design;
- each Native AOT publish leaves no managed assembly beside the image.

The Android root is still exclusion EX-126.

*The step was taken twice.* Its first run, from 23:34Z, ran each published root with the collector's own working
directory, a scratch folder. There a published slice compiler finds none of the retained datasets its checks
read, so it left 22 of them not run in every mode, the seven named checks of part B among them; every other
publish and run was as above. Bundle `sjl-7-001`'s collector was run from a checkout, where it found them. The
step now runs the roots from the tree it publishes ([`collect-area.py`](collect-area.py)), and was run again from
00:02Z on 2026-10-18. Every closure and catalog of the second run is the first's, and so is every image record's
digest. The first run is not retained.

**Optionality is unchanged** (clause 17). Each root carries what it carried after SJL-7, bundle `sjl-7-001`
section 10, assembly for assembly:

| Root | `Broiler.VM.Profile.JavaScript.Intl` | `Broiler.ECMAScript.Algorithms` |
|---|---|---|
| ExecutionOnly | not carried | under the JIT and trimming |
| SliceCompiler | under the JIT and trimming | under the JIT and trimming |
| Conformance | under the JIT and trimming | under the JIT and trimming |
| Cli | not carried | under the JIT and trimming |
| PolyglotCli | not carried | under the JIT and trimming |

The time zone and calendar tables are in the Intl data assembly, so only the two roots that hand in Intl data carry
them, as before the cut-over. Rule I3 holds this on the real closures (below).

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the three admitted product packages the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5` from Broiler.JS `a04cd44`, and `Broiler.Unicode.Algorithms` and `.Segmentation` `0.1.0-preview.5` from Broiler.Unicode `b802014`. Each content hash is the allow-list's and the committed feed's: the packages as published (JSC-311, JSC-313). `Broiler.ECMAScript.Generation` is test-only and in no closure |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one. Rule I3 read them in the rules run, which placed them in the tree for that run alone, and the log's first line names the same five digests |

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture suite, built
and run as the lane runs it, at `0a52e03`. **Every test passes**, none skipped.
- **Clause 12's rules:** I1, I2, I3 (which read the five image records), I4, I5, I6 and I7. I4 has four tests:
  - the Unicode pin equals the owner's, with its witnesses, as at SJL-7;
  - **the time pins equal the home's and every declared time version the pins'**
    (`I4_The_Time_Pins_Equal_The_Homes_And_Every_Declared_Time_Version_The_Pins`): the tz pin, the calendar pin
    and CLDR's `bcp47/timezone.json` against the home's by length and SHA-256, and the generator package's six
    version constants against this repository's pins;
  - its witnesses, a tz pin differing by one hash and a generator declaring another release, are reported
    (`I4_A_Time_Pin_Differing_From_The_Homes_And_A_Generator_Declaring_Another_Release_Are_Reported`).
- **Rule I5 holds the deletion list.** Its register holds the 24 members and types SJL-8.5 deleted, beside
  SJL-3.4's 34, SJL-4.4's 44, SJL-5.4's 44, SJL-6.4's 53 and SJL-7.4's 29, and it passes with its witness.
- **Clause 13's rules.** N29 and N31 verify every archive and member with the home's readers, and each refuses a
  changed byte and another release. N30 and N32 regenerate `JsTzdbTables.g.cs` and `JsCalendarTables.g.cs` with
  the home's generator from this repository's pins, compare them whole, generate the same bytes twice and refuse
  a hand edit. N30 also reads known histories and the rearguard daylight flag; N32 reads the published years.
  Each of the four has its register row (`rules.register.json`, status `Active`, witness `TzdbRuleTests` or
  `CalendarRuleTests`).
- **Rule N28** holds the budget (section 12).

**The packages.** [`after/packages/`](after/packages/) is `eng/collect-js-packages.py` at `0a52e03`, under its
own predeclared decision (`manifest.txt`), with the collector's transcript in
[`after/packages.log`](after/packages.log). It holds:
- the candidate pack of the whole solution, with every package's metadata and file list;
- a pristine JavaScript consumer that restores and runs, with upstream feeds unreachable, from a local feed
  holding the admitted shared packages;
- the rollback to the published `0.1.0-preview.5` set and forward again;
- the negative control;
- the consumer as Native AOT.

**Every step is as expected.** The consumer ran in each of its three runs, the negative control failed to restore,
and the Native AOT consumer ran. The nuspecs declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5` (the profile
and the compiler), `Broiler.Unicode.Algorithms` `0.1.0-preview.5` (the profile, Format, the compiler and the Intl
data) and `Broiler.Unicode.Segmentation` `0.1.0-preview.5` (the Intl data alone). That is what JSD-0077 section 1
and bundle JS-10-009's baseline say.
- **The nuspecs are JS-10-009's** but for each package's `repository` element. It names `0a52e03` where the
  baseline names `4df595b`, and it carries no URL or branch, because the clean checkout the step ran in was a local
  clone whose remote is a local path. Every dependency is the same, and so is every package's list of file names.
- It does not replace that baseline, which rule N37 holds.

## 12. The Intl data budget, restated by citation

**The budget is JSD-0027's owner decision (c)**, in
[JSD-0027](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0027-intl-scope-and-data-strategy.md):
- **768 KiB**, set by the repository owner on 2026-10-05, replacing JSD-0043's provisional 512 KiB bound;
- **832 KiB**, to which the owner raised it the same day for the zones' names (JSD-0058), its amendment "the
  budget raised to 832 KiB for the zones' names".

**Rule N28 holds it**: `CldrTablesRuleTests.Budget`, 832 x 1024 = 851,968 bytes, counted from the generated
literals of the Intl data assembly's generated files (`N28_The_Table_Data_Stays_Under_The_Budget`).
- The time zone tables have counted against it since JSD-0053, and the calendar table since JSD-0056. The break
  table, the owner's Unicode generator's, has counted since JSD-0074.
- JSD-0077 section 3 says: "Rule N28's budget is unmoved, since no table changed."

**At `0a52e03`, counted the way N28 counts**, the four generated files of
`src/Broiler.VM.Profile.JavaScript.Intl/` hold:

| File | Bytes |
|---|---|
| `JsCldrTables.g.cs`, the CLDR tables | 718,779 |
| `JsTzdbTables.g.cs`, the three time zone tables rule N30 holds | 100,111 |
| `JsCalendarTables.g.cs`, the calendar table rule N32 holds | 1,958 |
| `JsUnicodeTables.g.cs`, the break table | 19,517 |
| **All four** | **840,365, 11,603 under the budget** |

- **The figure is the base's.** No generated file differs between `431c20e` and `0a52e03`, and the same count at
  `431c20e` gives the same bytes, file by file. It is also bundle `sjl-7-001`'s figure at `be0b610`, section 10.
- **The assembly.** Built in Release, the Intl data assembly's file is 854,016 bytes at `0a52e03`, as at `431c20e`.
  It holds the tables and the code that hands them to the profile. The budget counts the tables alone.
- **The rule passes**: `N28_The_Table_Data_Stays_Under_The_Budget` in [`after/rules.log`](after/rules.log).

## 13. The simplification check

No clause of SJL-8 reads this section. It is taken as bundle `sjl-7-001` section 11 took it, with one record
added: the lines each C# file gained and lost.

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log)
searches the simple name of every member rule I5 registers, 228 of them, as a declaration in the JavaScript
family's sources.
- **Each of SJL-8.5's 24 is declared nowhere as itself.** 22 of their simple names are declared nowhere. The other
  two are other members of the same simple name in other types:
  - `Day`: the pattern generator's field index `JsDatePatternGenerator.Day`, a local function in
    `JsRealm.Locale`, and Temporal's `Day` property of a calendar's fields;
  - `NewYear`: the Coptic and Hebrew calendars' own, which are formula calendars and SJL-10's.
- **The earlier areas** read as bundle `sjl-7-001` section 11 read them, with two more names gone: 27 of SJL-3.4's
  34, 37 of SJL-4.4's 44, all 44 of SJL-5.4's, 48 of SJL-6.4's 53 and all 29 of SJL-7.4's are declared nowhere.
  SJL-4.4's two are its `Unsigned` members, whose same-named reader in `JsTimeZones` was deleted with that file.
- Rule I5 decides by the fully qualified name, and it passes (section 11).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's C# product sources (`src/`, every `tests/` excluded) in three parts, each with its
pattern:
- **host:** the platform's zones and calendars: `TimeZoneInfo`, the local-time conversions, `DateTimeOffset`, and
  .NET's calendar classes, which SJL-D-21 keeps out of both engines;
- **algorithm:** the area's algorithms by their names and constants: the tz database's files and compiled form, the
  table format's varints, the lunisolar and Persian calendars' astronomy and rules, and the proposal's names of the
  zone operations;
- **identifiers:** an IANA zone identifier spelled as a string literal, which a hand-written list of zones would
  spell.

**38 hits. None is a copy of what the area's deletion list or JSD-0076's X6 names.**

| Part | Hits | Where | What they are |
|---|---|---|---|
| host | 1 | `JsRealm.Date` | `Date.now`'s read of the host clock, which computes no zone or calendar (bundle `sjl-6-001` section 15 read it so) |
| host | 3 | `JsTemporalParser` | the Temporal grammar's `DateTimeOffset` production, a name |
| host | 1 | the conformance root | the test262 digest cache's file timestamp |
| algorithm | 17 | `JsIntlData` (2), `JsTzdbTables.g.cs` (1), `JsCalendars` (4), `JsRealm.Locale` (2), `JsTemporal.Zones` (5), `JsZoneNames` (3) | documentation naming `zone.tab`, the 33-year rule, the new moon and the solar terms, or the proposal's operation an adapter implements |
| algorithm | 3 | `JsTemporal.Zones` | the adapters' calls of the shared `ZonedTime.GetPossibleEpochNanoseconds`, `DisambiguatePossibleEpochNanoseconds` and `GetStartOfDay` (JSD-0077 section 4) |
| algorithm | 2 | `JsCalendars`, `JsEastAsianCalendar` | the mean synodic month, with which the Chinese and Korean calendars count months between two year starts the shared source gives. It is the calendars' arithmetic over the years, which JSD-0077 section 8 leaves this family as SJL-10's, and it is the same at `431c20e` |
| algorithm | 10 | `WasmDecoder` (1), the WebAssembly harness's corpus (9) | WebAssembly's LEB128 varints, not this area's |
| identifiers | 1 | `JsZoneNames.City` | `Etc/` zones have no exemplar city: CLDR's naming rule, SJL-9's, the same at `431c20e` |

No hand-written zone list, offset table or platform zone survives. No zone offset, and no year of the four
table-backed calendars, is computed outside the shared library. The formula and tabular calendars stay this
family's, as JSD-0077 section 8 says.

**The lines** ([`after/simplification/lines.log`](after/simplification/lines.log)), by `git diff --numstat`
between `431c20e` and `0a52e03` over every C# file:

| Group | Files | Added | Removed | Net |
|---|---|---|---|---|
| The JavaScript family's product sources | 12 | 418 | 1,387 | −969 |
| Other product sources (the slice compiler's check of `8613309`) | 1 | 37 | 0 | +37 |
| Tests (the architecture tests: the tz compiler, both table generators and their pin readers gone, the home's generator embedded, rule I4's time clause) | 15 | 621 | 1,828 | −1,207 |

`JsTimeZones.cs` went whole, 632 lines, and `JsCalendars.cs` lost 317 and gained 78. No generated file changed.

## 14. The Broiler.VM clauses

Each clause is judged as the milestone writes it, and parts A to C as [`decision-rule-area.md`](decision-rule-area.md)
writes them, with no revision.

| Clause | What it asks | Where | State |
|---|---|---|---|
| 12 | Rules I1 to I7 hold, with I4 for the tz and calendar pins | section 11, [`after/rules.log`](after/rules.log) | **Met** at `0a52e03`: the whole suite passes, I1 to I7 among them. I4's time clause holds the tz pin, the calendar pin and CLDR's zone file equal to the home's and the generator's declared versions to the pins, with its witnesses; I5's register holds SJL-8.5's 24 |
| 13 | Rules N29 to N32 regenerate the tables with the home's generator from Broiler.VM's pins, rule I4 holds those pins equal to the home's, and N29 to N32 are registered | section 11 | **Met** at `0a52e03`: N29 to N32 verify and regenerate with the home's readers and generator, byte for byte, each with its witnesses; I4 as in clause 12; each of the four has an active register row |
| 14 | test262 in both forms meets the area rule | sections 6 and 8 | **Met** on parts A and C. Part A: all four runs complete, and every changed verdict a (c') row whose re-runs agree, five in the bytecode pair and thirteen in the native; no row in C1. Part C: every twin at its base figure with its answer, in both forms |
| 15 | The reference datasets are unchanged | section 7 | **Met** on part B: the seven named checks print the base's lines, and no file under `src/tests/cldr/timezones/` or `src/tests/temporal/` differs between `431c20e` and `0a52e03` |
| 16 | Every root is published and run in three modes | section 11 | **Met** for every root this machine can publish, at `0a52e03`, with the packages as published; Android is EX-126 |
| 17 | Optionality is unchanged, as rule I3 shows | section 11 | **Met**: I3 passes on the five image records; only SliceCompiler and Conformance carry the Intl data assembly, and with it the zone and calendar tables, as after SJL-7 |

**Also in this bundle, and no clause of their own:**
- the engine-level vectors at corpus revision 23, which match at the after commit as at `458c10a` (section 10);
- the Intl data budget, restated by citation, which holds 840,365 of 851,968 bytes, unchanged (section 12);
- the packages, every step as expected (section 11);
- the simplification check (section 13);
- EX-131, the code that left this component's assurance coverage (section 9).

The data home's clauses, 1 to 6, and Broiler.JS's, 7 to 11, are judged in Broiler.JS's bundle of the same name,
which this bundle does not read. **None is accepted**: review is deferred, and SJL-8 may not meet its gate before
SJL-3 meets SJL-3's.
