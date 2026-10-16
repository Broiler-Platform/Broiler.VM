<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-6-001 - the hosted test262 workflow against the local procedure

**Milestone:** SJL-6 of the shared JavaScript libraries programme,
[`sjl-6-date.md`](../../javascript-shared-libraries/milestones/sjl-6-date.md), work package SJL-6.0.
**Rule:** [`decision-rule.md`](decision-rule.md), committed with the workflow and before its first run.
**Collector:** [`collect.py`](collect.py). **Owner and reviewer:** one person holds every role (EX-30),
and nobody independent has read this bundle.

**State:** collected, 2026-10-15, at SJL-6.0. **The check is MET** under [`decision-rule.md`](decision-rule.md)
as written, with no revision: the workflow's first run is whole and retainable in both forms, and its
comparison with bundle `sjl-5-001`'s local runs of the same commit puts every difference in an admitted
class (sections 2 and 3). This is the first half of SJL-6.0's done-when; the second, SJL-6.1's rule naming
the workflow for SJL-6's base and after runs, is SJL-6.1's to write.

**The Date area (sections 5 to 16):** collected, 2026-10-16, at SJL-6.6. **Its rule,
[`decision-rule-area.md`](decision-rule-area.md), is MET in both parts as written, with no revision**, at the
after commit `24632b1`: every changed test262 verdict in either form is a race between allowances that class C4
admits, and no fuel charge moved (sections 11 and 12). Clauses 6 to 9 are met (section 16). Nothing is accepted:
review is deferred, and SJL-6 may not meet its gate before SJL-3 meets SJL-3's.

## 1. The run

| | |
|---|---|
| Workflow | [`.github/workflows/test262-evidence.yml`](../../../.github/workflows/test262-evidence.yml) with its driver [`eng/test262-evidence.py`](../../../eng/test262-evidence.py), landed in `9c3d809` (Broiler-Platform/Broiler.VM#128) |
| Run | [38731527446](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38731527446), attempt 1, dispatched from `main` at `9c3d809` on 2026-10-15 at 12:01:21Z, completed at 12:08:43Z ([`hosted/run.txt`](hosted/run.txt)) |
| Tested commit | `72ad941`, bundle `sjl-5-001`'s after commit, a clean checkout, both forms in one dispatch |
| Partition | four runners of four shards per form, sixteen shards, the local runs' partition |
| Allowances | fuel 100,000,000 and 5,000 ms per variant, as bundle `sjl-0-001`'s procedure states them |
| Retained | [`hosted/`](hosted/): the workflow's artifact `test262-evidence`, every file unchanged, with [`hosted/imported.txt`](hosted/imported.txt), which records the digest of the zip as downloaded |
| Builds | one: every runner and the merging job built assemblies whose list hashes alike, in each form ([`hosted/wide-bytecode.binaries.log`](hosted/wide-bytecode.binaries.log), [`hosted/wide-native.binaries.log`](hosted/wide-native.binaries.log)) |
| Runners | GitHub's standard Linux runner, four processors, .NET SDK 10.0.402, native backend `x86-64-sysv` as the harness reports it. The runners did not all run one image release: bytecode runner 1 and native runners 0 and 1 ran `ubuntu24` 20261004.327.1, and the other five and the merging job 20261012.334.1 (each form's `hosted/<run>.runners.txt`, and `hosted/run.txt`). The assemblies they built are the same, which the binaries logs show |

The local runs are bundle `sjl-5-001`'s own at the same commit
([`../sjl-5-001/after/`](../sjl-5-001/after/)), taken on one machine with four processors and the .NET SDK
10.0.401, by the same procedure. The two machines' builds differ in their SDK's patch release, and their
assemblies are not compared: the rule compares verdicts.

## 2. Completeness

**Both forms are whole and retainable.** [`completeness.log`](completeness.log) reads each off the
artifact:

| Run | run-test262's exit code | Whole | Retainable | Shard findings | Distinct assembly lists |
|---|---|---|---|---|---|
| `wide-bytecode` | 1: cases failed, and the floor holds | yes | yes | none | 1 |
| `wide-native` | 4: the floor was set in the native `x86-64-win64` form and this run is `x86-64-sysv`, so the floor asks to be re-based | yes | yes | none | 1 |

Both exit codes are the local runs' own at this commit, for the same reasons
([`../sjl-5-001/after/wide-bytecode.run.log`](../sjl-5-001/after/wide-bytecode.run.log),
[`../sjl-5-001/after/wide-native.run.log`](../sjl-5-001/after/wide-native.run.log)), and the rule
admits 0, 1 and 4.

## 3. The comparison

**Every difference is in an admitted class, in both pairs.** [`compare-wide-bytecode.log`](compare-wide-bytecode.log)
and [`compare-wide-native.log`](compare-wide-native.log): `eng/compare-test262-forms.py --same-form`, the
local report as control and the hosted one as candidate, the native pair with `--exempt-guest-loads`
over the extracted suite. Both exit 0. Each pair joins 101,723 variants with none on one side only; the
two reports of each pair have the same totals.

| Run | Path | Variant | Local | Hosted | Class |
|---|---|---|---|---|---|
| `wide-bytecode` | `test/built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js` | sloppy | exhausted, wall clock | exhausted, fuel | (c') |
| `wide-bytecode` | `test/staging/sm/Date/dst-offset-caching-4-of-8.js` | sloppy | exhausted, fuel | exhausted, wall clock | (c') |
| `wide-bytecode` | `test/staging/sm/TypedArray/sort_large_countingsort.js` | sloppy | exhausted, wall clock | exhausted, fuel | (c') |
| `wide-bytecode` | `test/staging/sm/regress/regress-610026.js` | sloppy | exhausted, wall clock | exhausted, nested-load bytes | (c') |
| `wide-bytecode` | `test/staging/sm/regress/regress-610026.js` | strict | exhausted, wall clock | exhausted, nested-load bytes | (c') |
| `wide-native` | `test/annexB/built-ins/RegExp/RegExp-trailing-escape-BMP.js` | strict | exhausted, nested-load fan-out | exhausted, wall clock | (c') |

**What they are.** In every row both machines gave no answer: each spent an allowance, and they differ
only in which allowance ran out first, the wall clock on one machine and a counter on the other. That
is the race class (c') names, and the rule does not run them again. No row moved between a pass, a
failure, a refusal and an exhaustion. In four rows the local run reached the wall clock first, and in two the
hosted run did, so neither machine is the one that runs out of wall clock throughout.

## 4. What this allows, and what it does not

- **SJL-6.1's rule may name the workflow** for SJL-6's base and after runs. A run it takes is compared with
  another hosted run, never with a local one, as the workflow's own header says.
- **The workflow takes no re-run of a single row.** A rule that re-runs a (c') or (e) row says where.
- **Nothing here judges an engine change**: the commit it ran at is one bundle `sjl-5-001` already judged.

## 5. SJL-6's area rule (SJL-6.1)

*(Added 2026-10-15, at SJL-6.1. Nothing above changes.)* Two files join this bundle with the Date area's
verdict, [JSD-0072](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0072-date-algorithms-move-to-the-shared-algorithms-library.md),
before any code of SJL-6 exists:

- [`correspondence.md`](correspondence.md), the verdict's input: both engines' Date algorithms by file and
  line, and every difference numbered and classified;
- [`decision-rule-area.md`](decision-rule-area.md), the area's rule for this repository's clause 7. Its
  part A takes SJL-6's base and after runs with the workflow this bundle's check judged, which completes
  SJL-6.0's done-when; its part B holds the fuel-parity twins and the probes in `src/tests/forms/date` to
  the base.

The area's runs will be retained here under directories of their own, beside `hosted/`, which stays
SJL-6.0's. [`manifest.json`](manifest.json) lists what SJL-6.0's collection retained; the area's collection
writes its own.

## 6. The Date area's base and its first vectors (SJL-6.2)

*(Added 2026-10-15, at SJL-6.2. Nothing above changes.)* The area's collector is
[`collect-area.py`](collect-area.py), which reuses SJL-6.0's [`collect.py`](collect.py) for a hosted run's
import and completeness, and bundle `sjl-5-001`'s collector for the twin bisection, so that each arm runs the
code an earlier arm ran. It writes into [`base/`](base/) and [`vectors/`](vectors/), and lists what it retains
in [`area-manifest.json`](area-manifest.json); the steps' output is [`base/collect.log`](base/collect.log).

**The base commit is `2df523a`**, the commit that carries [`decision-rule-area.md`](decision-rule-area.md),
whose tree is the merge `99ad3e1`'s; outside the documents it differs from the default branch after SJL-5,
`72ad941`, by the workflow, its driver and the Date probes alone ([`base/identity.txt`](base/identity.txt)).

### Part A's base, on hosted runners

| | |
|---|---|
| Run | [38752500179](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38752500179), attempt 1, dispatched from `main` at `99ad3e1` on 2026-10-15 at 14:33:44Z with both forms and the commit `2df523a` named by all forty digits, completed at 14:41:59Z ([`base/hosted/run.txt`](base/hosted/run.txt)) |
| Retained | [`base/hosted/`](base/hosted/): the artifact `test262-evidence`, every file unchanged, with [`base/hosted/imported.txt`](base/hosted/imported.txt), which records the digest of the zip as downloaded, the digest GitHub records |
| Complete | both forms, as the rule states it ([`base/completeness.log`](base/completeness.log)): `wide-bytecode` exits 1 and `wide-native` 4, the floor having been set in the `x86-64-win64` form; each whole and retainable, no shard finding, one assembly list per form |
| Runners | GitHub's standard Linux runner, four processors, .NET SDK 10.0.402, `x86-64-sysv`. Bytecode runners 0 to 2, native runners 0 and 3 and the merging job ran image `ubuntu24` 20261012.334.1, and bytecode runner 3 and native runners 1 and 2 ran 20261004.327.1 (each form's `base/hosted/<run>.runners.txt`); the assemblies they built are the same |

The after runs are compared with this one under the rule, per variant; this section judges nothing.

### Part B's base: the fuel-parity twins and probes

Taken locally on 2026-10-15 from 16:12Z, from a clean checkout of `2df523a`, on the programme's local machine
(four processors, .NET SDK 10.0.401, Linux), otherwise idle, by `collect-area.py --only base-twins`
([`base/fuel-parity.log`](base/fuel-parity.log), [`base/build-cli.log`](base/build-cli.log)). **Every twin
of `src/tests/forms/wide` and every probe of `src/tests/forms/date`, 13 in all, completes at its bisected
figure and is refused one fuel unit below it with an exhaustion on fuel, in both forms, with the same answer
in both.** The ten wide twins' figures and answers are those of bundle `sjl-5-001`'s after run, at
`72ad941`. The three probes' answers are those Node.js 22 gives for the same scripts under `TZ=UTC`, as their
README requires:

| Probe | Figure | Answer |
|---|---|---|
| `date/parse.small.js` | 71,850 | 22044730 |
| `date/renderings.small.js` | 27,282 | 153540 |
| `date/time-arithmetic.small.js` | 124,617 | -49431117 |

### Engine-level vectors: the first runs on the Date area

Corpus revision 14 (Broiler.JS `ff6cd24`) adds the Date area. This family's first run on it used a scratch
pin that records the archive's digests ([`vectors/revision-14.pin`](vectors/revision-14.pin)); `git archive`
of the commit, run twice, gave byte-identical files, and the archive is not committed. The conformance root
was the base commit's, built in Release:

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 14 | `ff6cd24`, 37 files, content `0e0c1704...` | 50,715 vectors: 50,699 match, 0 classified divergences, 16 unclassified differences | [`run-revision-14.log`](vectors/run-revision-14.log), [`run-revision-14.tsv.gz`](vectors/run-revision-14.tsv.gz) |

The sixteen are the three rows of [`correspondence.md`](correspondence.md) that JSD-0072 classes as
corrections of this family:

| Vector | Expected | Broiler.VM | Row |
|---|---|---|---|
| `date/make-day/0863` | May of year 0, the month 1e19 taken modulo 12 exactly | January: the modulo is taken in floating point | T4 |
| `date/month-day/0021`, `0022`, `0025`, `0026`, `0037`, `0038`, `0041`, `0042` | February 29 regulated in the year given, 1000000100, 1000000128, 2000000100 and -1000000100 | regulated in `1972 + year % 400` | G3 |
| `date/round-trip/0001`, `0005`, `0007`, `0009`, `0011`, `0013`, `0015` | `Date.parse` of this family's own `toUTCString` answers the time value | NaN for a negative year | P9 |

This family reads back its renderings of year 0 and of every year past 9999, as JSD-0072's "for years 0 and
above only" says; row P9's cell "past 9999, the same" means the same time value.

**Every vector of corpus revision 15 has a classified result in this family.** Revision 15 (Broiler.JS
`a3452d9`) classifies both engines' first runs, and this family's pin
([`src/tests/vectors/pins/vectors.pin`](../../../src/tests/vectors/pins/vectors.pin)) moves to it; the
archive was taken twice, the copies byte-identical, and the second extracted afresh and hashed there. On
the pinned archive, with the same conformance root
([`vectors/run-revision-15.log`](vectors/run-revision-15.log),
[`vectors/run-revision-15.tsv.gz`](vectors/run-revision-15.tsv.gz)): **50,715 vectors, 50,699 match, 16
classified divergences, 0 unclassified differences.** The 16 are the `divergence.broiler-vm` fields revision
15 adds: T4's one (`vm-month-float-modulo`), G3's eight (`vm-month-day-reduced-year`) and P9's seven
(`vm-parse-negative-year`), each naming SJL-6.4. The negative control
([`vectors/run-negative-control-revision-15.log`](vectors/run-negative-control-revision-15.log)) changes
the expected value of `text/trim/0001` in memory, and the run reports that vector, and only it, as an
unclassified difference. The run of the negative control on revision 14 is retained beside it and judges
nothing.

**What the runs say of JSD-0072.** Every difference this family's first run found is one the
correspondence classified, so the record's first falsifier does not fire, and no new record is needed.
The 22 numeric slash dates, `impl-defined`, match this family's expectation, an invalid date. Broiler.JS's
first runs, in UTC and in `America/New_York`, are its own bundle's; corpus revision 15 classifies them under
the rows that bundle names.

## 7. The vectors after this family's cut-over (SJL-6.4)

This family's Date area is the shared library's since `31f405b`
([JSD-0073](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0073-the-profile-on-the-shared-date-algorithms.md)),
and its pin moves with it to corpus revision 16 (Broiler.JS `6f29deb`), which adds twelve `impl-defined`
vectors of strings outside the format and keeps revision 15's sixteen divergence fields. The archive was
taken twice, the copies byte-identical, and the second extracted afresh and hashed there. The area
collector's `vectors` step ran at `31f405b` from a clean checkout and rebuilt the conformance root there,
whose build log replaces revision 15's in [`vectors/build-conformance.log`](vectors/build-conformance.log)
(the earlier one is in this repository's history):

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 16 | `6f29deb`, 37 files, content `819c526e...` | 50,727 vectors: 50,727 match, 0 classified divergences, 0 unclassified differences | [`run-revision-16.log`](vectors/run-revision-16.log), [`run-revision-16.tsv.gz`](vectors/run-revision-16.tsv.gz) |
| Its negative control, `text/trim/0001` | the same | 50,726 match, 1 unclassified difference, that vector | [`run-negative-control-revision-16.log`](vectors/run-negative-control-revision-16.log), [`run-negative-control-revision-16.tsv.gz`](vectors/run-negative-control-revision-16.tsv.gz) |

**Each of the sixteen divergence fields reports "divergence no longer holds"**: T4's
`date/make-day/0863`, G3's eight and P9's seven, the vectors of section 6's table, and nothing else does.
The twelve vectors of revision 16 match this family's expectation, its dialect as the library ports it.
This is the cut-over's record, not part A or part B: SJL-6.6 takes the after runs both parts judge.
`area-manifest.json` lists the new files.

**The published package.** `0.1.0-preview.4` was published to nuget.org later the same day, from
Broiler.JS `d1e4b11`, whose library code is `e04874d`'s, and the committed feed now holds it as published
([JSC-307](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-307)). The run above
used the copy packed from `e04874d`. With the published package, the same conformance root's run on
revision 16 gives a report identical byte for byte to `run-revision-16.tsv.gz`'s, and its negative control
reports its one vector; neither run is retained, since neither differs.

## 8. The vectors at corpus revision 17 (SJL-6.5)

Broiler.JS's Date area is the shared library's since its `31c0480` (SJL-6.5), and corpus revision 17
(Broiler.JS `520ecf9`) retires every divergence field of the Date area, this family's sixteen of section 7
and Broiler.JS's 1,016, and gives Broiler.JS's expectation of `date/outside-format/0001` and `0010` its
reading since that cut-over, which is this family's dialect's; no expectation of this family changes. This
family's pin moves to it in `99d4945`. The archive was taken twice, the copies byte-identical, and the second
extracted afresh and hashed there. The area collector's `vectors` step ran at `99d4945` from a clean checkout
and rebuilt the conformance root there, whose build log replaces revision 16's in
[`vectors/build-conformance.log`](vectors/build-conformance.log) (the earlier one is in this repository's
history):

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 17 | `520ecf9`, 37 files, content `7d2193be...` | 50,727 vectors: 50,727 match, 0 classified divergences, 0 unclassified differences | [`run-revision-17.log`](vectors/run-revision-17.log), [`run-revision-17.tsv.gz`](vectors/run-revision-17.tsv.gz) |
| Its negative control, `text/trim/0001` | the same | 50,726 match, 1 unclassified difference, that vector | [`run-negative-control-revision-17.log`](vectors/run-negative-control-revision-17.log), [`run-negative-control-revision-17.tsv.gz`](vectors/run-negative-control-revision-17.tsv.gz) |

The vectors that carried this family's sixteen fields are `normative` again, and match. This is not part A
or part B: SJL-6.6 takes the after runs both parts judge. `area-manifest.json` lists the new files.

## 9. Exclusions

*(Added 2026-10-16, at SJL-6.6. Nothing above changes. The exclusions come first among SJL-6.6's sections
because rule H2 reads an exclusion's definition from a bundle's section 9 alone.)*

| ID | Status | Exclusion |
|---|---|---|
| EX-129 | Open | **The Date code that left this component's assurance coverage.** The 53 members rule I5 registers for SJL-6.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), and the inline MakeFullYear of `DateFromFields` and `setYear` and the civil arithmetic and month table of `JsLocalTime.From` that JSD-0073 section 4 names, left the JavaScript family's sources at SJL-6.4. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Time`, which the home's own assurance component covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters, the provider of this family's UTC and `DateObject` stay in this family and stay covered. EX-125, EX-127 and EX-128 say the same of the earlier areas |

## 10. The after arm (SJL-6.6)

The after steps were added to
[`collect-area.py`](collect-area.py) at SJL-6.6, when there was an after commit to run them at. They write
into [`after/`](after/) and reuse bundle `sjl-5-001`'s collector, and through it `sjl-4-001`'s, `sjl-3-001`'s
and `sjl-0-001`'s, so that SJL-5's and SJL-6's after arms run the same code; part A's after run is a dispatch
of the workflow, imported as the base's was.

| | |
|---|---|
| Commit | `24632b1`, the default branch after SJL-6's merges, a clean checkout ([`after/identity.txt`](after/identity.txt)). Its product sources are those of `16deb1f`, the merge that landed SJL-6.4's cut-over (`31f405b`, JSD-0073) with the published package. Against the cut-over commit, the identity lists 11 files outside the documents: the published package on the committed feed with its pin, the feed's README, both lock files, the allow-list, the notices row (JSC-307), and this family's pin of the shared corpus at revision 17 with its archives and README (section 8). The last five are all that `24632b1` adds to `16deb1f` outside the documents, and none is product code |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), with the same SHA-256 at the base and after (both identities) |
| Collector | [`collect-area.py`](collect-area.py) as this change commits it, run against the clean checkout |
| Taken | 2026-10-16. Part A's after run by the workflow from 10:36Z to 10:44Z; locally, step by step, `after-identity` and `after-twins` from 10:40Z, `vectors` to 10:46Z, `after-import`, `after-complete` and `compare` at 10:55Z, `rerun` and `wall-race` from 10:55Z to 11:01Z, then `closures`, `rules`, `simplification` and `packages`, on an otherwise idle machine. Broiler.JS's after runs took place on GitHub's runners meanwhile, not on this machine |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv` |
| Base side of the re-runs | a clean checkout of the base commit `2df523a`, its conformance root built there in Release |

## 11. Test262, both forms (rule part A)

**MET.** All four runs are complete as the rule states it, and every changed verdict is in class C4: (b)
or (c'), with every (c') row's re-runs listed below, and the four whose re-runs differ admitted under (e).

**The after run.** [38888330708](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38888330708),
attempt 1, dispatched from `main` at `24632b1` on 2026-10-16 at 10:36:21Z with both forms and the commit
`24632b1` named by all forty digits, completed at 10:44:03Z ([`after/hosted/run.txt`](after/hosted/run.txt)).
The workflow and its driver at the dispatched ref are byte-identical to the rule commit's (`git diff 2df523a
24632b1` names neither). The artifact `test262-evidence` is retained unchanged in
[`after/hosted/`](after/hosted/), with [`after/hosted/imported.txt`](after/hosted/imported.txt), which records
the digest of the zip as downloaded, the digest GitHub records. Every runner and the merging job ran image
`ubuntu24` 20261012.334.1, the base's majority image (each form's `after/hosted/<run>.runners.txt`), and each
form's runners built one assembly list.

| Run | Complete ([`after/completeness.log`](after/completeness.log)) | Totals after | Totals at the base | Comparison |
|---|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153 | the same | 10 variants differ, all (c') ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base: the floor was set in the win64 native form; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153 | the same | 19 variants differ: 2 (b), 17 (c') ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

Both comparisons exit 0. **No variant moved between a pass, a failure, a refusal and an exhaustion**: in every
changed row, both runs gave no answer, and they differ only in which allowance ran out first.

**Class (b).** `staging/sm/regress/regress-610026.js` [sloppy] and [strict], native: exhausted on the wall
clock at the base and on nested-load bytes after, a guest-load variant, which the rule admits under
`--exempt-guest-loads` without a re-run. Bundle `sjl-6-001`'s check (section 3) saw the same path race
between the same two allowances.

**Class (c') and its re-runs** ([`after/rerun-wide-bytecode.tsv`](after/rerun-wide-bytecode.tsv),
[`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their logs and every report): each side was
one process of the conformance root over the rows' paths, alone, at `2df523a` and at `24632b1`, under the
part's manifest, form and allowances.

| Rows | Pair | Base -> after | Re-run at the base, after | Class |
|---|---|---|---|---|
| `decodeURI/S15.1.3.1_A2.5_T1.js` and `decodeURIComponent/S15.1.3.2_A2.5_T1.js`, each [sloppy] and [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-3-of-8.js` [sloppy], `-7-of-8.js` [sloppy] and [strict] | bytecode | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-8-of-8.js` [sloppy] and [strict] | bytecode | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/TypedArray/sort_large_countingsort.js` [strict] | bytecode | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `annexB/built-ins/RegExp/RegExp-trailing-escape-BMP.js` [strict] | native | Exhausted/NestedLoadFanOut -> Exhausted/WallClock | Exhausted/NestedLoadFanOut, the same | **C4 (c')**, the re-runs agree |
| `decodeURI/S15.1.3.1_A2.5_T1.js` and `decodeURIComponent/S15.1.3.2_A2.5_T1.js`, each [sloppy] and [strict] | native | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-1-of-8.js` [strict], `-4-of-8.js` [sloppy] and [strict], `-8-of-8.js` [sloppy] | native | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-7-of-8.js` [sloppy] and [strict] | native | Exhausted/WallClock -> Exhausted/Fuel | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/TypedArray/sort_large_countingsort.js` [sloppy] | native | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/Fuel, Exhausted/Fuel | **C4 (c')**, the re-runs agree |
| `staging/sm/TypedArray/sort_large_countingsort.js` [strict] | native | Exhausted/Fuel -> Exhausted/WallClock | Exhausted/WallClock, Exhausted/WallClock | **C4 (c')**, the re-runs agree |
| `staging/sm/Date/dst-offset-caching-1-of-8.js` [sloppy], `-2-of-8.js` [sloppy] and [strict], `-8-of-8.js` [strict] | native | Exhausted/Fuel -> Exhausted/WallClock | differ: one side Exhausted/Fuel, the other Exhausted/WallClock | **C4 (e)**, below |

**Class (e)** ([`after/wall-race-wide-native.tsv`](after/wall-race-wide-native.tsv), with its log and both
reports): the four rows run alone once more at each commit on this machine, with the part's fuel allowance and
ten times its wall-clock allowance, 50,000 ms, the machine otherwise idle. **Each gives `Exhausted/Fuel` at both
commits**: the same verdict, neither a wall-clock exhaustion nor an internal defect, so each is admitted.

**What the rows say.** Every row is one of the long-running tests this family's earlier bundles saw race between
the wall clock and another allowance, on both kinds of machine. Fifteen of the 29 are
`staging/sm/Date/dst-offset-caching-*`, in the area this milestone moved, five in the bytecode pair and ten in
the native: each exhausts fuel at both commits when the wall clock is out of the way, as fuel is deterministic
and JSD-0072 keeps the charging units. No row needed C1 or C3: none of the corrections
T4, G3 and P9 changed a test262 verdict, as their cases are the vectors' (sections 6 and 7).

**The exhaustion verdicts** clause 7 requires: every row exhausted on fuel at the base is exhausted on fuel
after or, where the whole run reached the wall clock first, exhausts fuel at both commits alone; and every row
that completes at the base completes after.

## 12. The fuel-parity twins and probes (rule part B)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log): every twin of `src/tests/forms/wide` and every
probe of `src/tests/forms/date` that [`base/fuel-parity.log`](base/fuel-parity.log) bisected, ten twins and
three probes, each in the bytecode form and in the native form under `x86-64-sysv`, 26 rows, completes at its
base figure with the base's answer and refuses with `AllowanceExhausted on Fuel` at one less. **No charge
moved**, inside the area or outside it, as JSD-0072 requires; the probes' figures are section 6's, 71,850,
27,282 and 124,617.

## 13. Engine-level vectors at the after commit

**Every vector passes.** The conformance root of `24632b1` over this family's pin, corpus revision 17:
**50,727 vectors, 50,727 match, 0 classified divergences, 0 unclassified differences**
([`vectors/after-revision-17.log`](vectors/after-revision-17.log)); its report, decompressed, is
byte-identical to section 8's `vectors/run-revision-17.tsv.gz`, taken at `99d4945` (the compressed files
differ in their gzip headers alone). The negative control changes the expected value of `text/trim/0001` in
memory, and the run reports that vector, and only it, as an unclassified difference
([`vectors/after-negative-control-revision-17.log`](vectors/after-negative-control-revision-17.log)); its
report, decompressed, is byte-identical to section 8's too. The step rebuilt the conformance root at
`24632b1`, whose build log replaces section 8's in
[`vectors/build-conformance.log`](vectors/build-conformance.log) (the earlier one is in this repository's
history).

## 14. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, and each closure
declares the algorithms assembly.** [`after/closures/`](after/closures/) holds, for ExecutionOnly,
SliceCompiler, Conformance, Cli and PolyglotCli, the closure under the JIT, trimming and Native AOT, the
catalog each mode printed, and the transcripts of every publish and run. Each JIT and trimmed closure lists
`Broiler.ECMAScript.Algorithms`, and each Native AOT publish leaves no managed assembly beside the image. All
646 of the slice compiler's checks pass in every mode, with three not run on this machine and claimed by
nothing, as before; the execution-only root's 22 pass in every mode. The Android root is still exclusion
EX-126.

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the admitted package the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`, its content hash as the allow-list pins it and as the committed feed's copy has it, and the commit its nuspec names, Broiler.JS `d1e4b11`: the package as published |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed; rule I3 read them in the rules run, which placed them in the tree for that run alone |

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture suite,
built and run as the lane runs it, at `24632b1`: **381 tests, every one passed**, rules I1, I2, I3, I5, I6
and I7 among them; I3 read the five image records named in the log's first line. Rule I4 is SJL-7's, so "I1
to I7" is six rules at this milestone. **Rule I5 holds the deletion list**: its register holds the 53
members SJL-6.4 deleted, beside SJL-3.4's 34, SJL-4.4's 44 and SJL-5.4's 44, and it passes.

**The packages.** [`after/packages/`](after/packages/) is `eng/collect-js-packages.py` at `24632b1`, under its
own predeclared decision (`manifest.txt`), with the collector's transcript in
[`after/packages.log`](after/packages.log): the candidate pack of the whole solution with every package's
metadata and file list, a pristine JavaScript consumer that restores and runs with upstream feeds unreachable
from a local feed holding the admitted shared package, the rollback to the published `0.1.0-preview.5` set and
forward again, the negative control, and the consumer as Native AOT. **Every step is as expected**: the
consumer ran in each of its three runs, the negative control failed to restore, and the Native AOT consumer
ran. Its nuspecs that declare a shared dependency declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`,
which rule N37 admits. It does not replace the profile's package baseline, bundle JS-10-006, which rule N37
still holds.

## 15. The simplification check

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log)
searches the simple name of every member rule I5 registers, 175 of them (SJL-3.4's, SJL-4.4's, SJL-5.4's and
SJL-6.4's), as a declaration in the JavaScript family's sources. **Each of SJL-6.4's 53 is declared nowhere
as itself**: 48 of their simple names are declared nowhere, and the other five are other members of the same
simple name in other types: the Temporal grammar's production `JsTemporalParser.DateDay`, a non-ISO calendar
year's `DaysInMonth(int)` in `JsCalendars`, and the pattern generator's field indices `DayOfYear`,
`WeekOfYear` and `Weekday` in `JsDatePatternGenerator`. The earlier areas' 16 are the ones bundle `sjl-5-001`
section 11 read. Rule I5 decides by the fully qualified name, and it passes (section 14).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's product sources for the constructs that would mean a surviving copy of the area: the
400-year cycle's days, the civil epoch's shift, the day's milliseconds and the time value's limit spelled as
code, the platform's dates and zones, the civil-day helpers by name, and the month and weekday names a
rendering spells. The platform's `DateTime` is named with its namespace, its constructor or one of its static
members, because Temporal names its own date-time record and goals `DateTime`; the collector's first run of
the step, with a bare `DateTime`, found 78 hits, 40 of them those names, and the step was run again with the
pattern its committed form holds. **38 hits; none is a copy the area's deletion list or JSD-0072's X6 names**:

| Hits | Where | What they are |
|---|---|---|
| 7 | `JsCalendars` (2), `JsTemporal.Calendar` (4), `JsTimeZones` (1) | calls to the library's `IsoCalendarMath` |
| 11 | `JsTemporal.CalendarNonIso` (9), `JsCalendars` (2) | the non-ISO calendars' own month lengths, and the mean Gregorian year in a comment of their astronomy; the non-ISO calendars stay with SJL-8 and SJL-10, as JSD-0072 says |
| 3 | `JsTemporal.Calendar` (1), `JsRealm.Temporal` (1), `JsTemporalCore` (1) | Temporal's own `DaysInMonth` field of a date's calendar parts, its read for `daysInMonth`, and a comment naming the library's members |
| 5 | `JsRealm.Date` | the type's comment, which says the calendar is the specification's and not `System.DateTime` (4), and `Date.now`'s read of the host clock, which computes no calendar (1) |
| 3 | `JsTemporalParser` | the Temporal grammar's `DateTimeOffset` production, a name |
| 4 | `Broiler.VM.Runtime` (`VmRuntime`, `VmInstanceImplementation`) | the runtime's drain deadlines, which are not JavaScript |
| 5 | the composition roots | the structured-clone checks' scripts, which clone `new Date(86400000)` (4), and the test262 digest cache's file timestamp (1) |

No clause of SJL-6 reads the duplicate search.

## 16. The Broiler.VM clauses

| Clause | What it asks | Where | State |
|---|---|---|---|
| 6 | Rules I1 to I7 hold, and rule I5 holds the deletion list | section 14, [`after/rules.log`](after/rules.log) | **Met** at `24632b1`: 381 of 381, I5 among them, its register holding SJL-6.4's 53 |
| 7 | test262 in both forms meets the area rule | sections 11 and 12 | **Met**: part A MET in both pairs, every change a (b) row or a (c') row whose re-runs agree or, for four, whose run with the wall clock out of the way agrees; part B MET; the exhaustion verdicts hold |
| 8 | Every root is published and run in three modes | section 14 | **Met** for every root this machine can publish, at `24632b1`; Android is EX-126 |
| 9 | Engine-level vectors pass | sections 8 and 13 | **Met**: every vector of corpus revision 17 matches at `24632b1`, with its negative control |

The home clauses, and clauses 1 to 5, Broiler.JS's, are judged in Broiler.JS's bundle of the same name.
None is accepted: review is deferred, and SJL-6 may not meet its gate before SJL-3 meets SJL-3's.
