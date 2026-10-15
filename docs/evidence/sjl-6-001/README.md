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
