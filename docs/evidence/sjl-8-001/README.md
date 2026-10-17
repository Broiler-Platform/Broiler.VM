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

**State:** rules, correspondence and the base, 2026-10-17. **Nothing here is a verdict.**
- The base of all three parts was taken at the commit that carries the rule, `431c20e`, before any code of SJL-8
  landed (section 2).
- The after runs follow SJL-8.5's cut-over.

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

The collector's `vectors` step, bundle `sjl-6-001`'s, ran the conformance root in Release.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 21 | At `431c20e`, the base, from 16:57Z, with the scratch pin [`vectors/revision-21.pin`](vectors/revision-21.pin) beside a scratch copy of its archive, which `git archive` wrote twice, byte-identical | 92,434 vectors: 92,434 match | [`run-revision-21.log`](vectors/run-revision-21.log), [`run-revision-21.tsv.gz`](vectors/run-revision-21.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 92,433 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-21.log`](vectors/run-negative-control-revision-21.log) |
| Revision 22 | At `15da5fd`, clean, on the committed pin, which moves to revision 22 there | 92,434 vectors: 92,434 match, 0 classified divergences, 0 unclassified differences | [`run-revision-22.log`](vectors/run-revision-22.log), [`run-revision-22.tsv.gz`](vectors/run-revision-22.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 92,433 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-22.log`](vectors/run-negative-control-revision-22.log) |

**This profile matches every time-zone vector, each of its own `per-engine` expectations included.** The vectors
confirm the correspondence's reading that this family has no correction of its own in the area. Both engines' runs
on the shared corpus disagree only where JSD-0076 says Broiler.JS's policy or code differs.

The pin is at revision 22 (`src/tests/vectors/pins/vectors.pin`). Its archive is taken twice, byte-identical, and
its 49 files are hashed from an extracted copy.
