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
