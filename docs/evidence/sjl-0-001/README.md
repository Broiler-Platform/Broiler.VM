<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle SJL-0-001

**Milestone:** SJL-0 of [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-0-records-decisions-and-baselines.md):
work package SJL-0.1's retained inventory, and the Broiler.VM half of work package SJL-0.8's
baselines. **Collected:** 2026-10-11, from a separate checkout of the base commit `0087e8d7`, a clean
tree, by [`collect.py`](collect.py) in this directory. **Core contract version:** 1, unchanged.
**What it decides:** nothing, except the one predeclared reading of the measurement sub-bundle in
[`measurement/`](#7-the-measurement-sub-bundle). A base is not a verdict: every later comparison of the
programme reads it under a rule committed before the code it judges.
**The milestone is not accepted**: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while
review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | SJL-0-001 (Broiler.VM's; Broiler.JS keeps its own bundle of the same name, which does not exist yet) |
| Milestone | SJL-0, exit-gate clause 5, and the inventory of work package SJL-0.1 |
| Base commit | `0087e8d73ae3833200fa263b68f71db15b059a9c`, a clean tree, checked out separately from the tree this bundle is committed in. Every build, run and hash of sections 3 to 9 was taken there |
| Commits it names besides | Broiler.JS `44a214dc5b04a5c8db556dc9b57894beaeb20d0c`, Broiler.Unicode `2affa78ffa96d2c975ad65eb8e990722b18f3e22`, Broiler.RegEx `c1422f4d824aa28ff6edd4766d56be25ba7c8a15`, Broiler.DateTime `8de12cc335170fb17ef9055458a125982e089b61`, each a clean checkout |
| Rules it is a base for | [`../sjl-3-001/decision-rule.md`](../sjl-3-001/decision-rule.md), parts A, B and C, committed in `f55f83c` before this bundle was collected; [`../sjl-1-001/decision-rule.md`](../sjl-1-001/decision-rule.md) names no base here |
| RID | `linux-x64`, one machine, four processors; `identity.txt` names the SDK |
| Owner | the JavaScript profile owner, with the core architecture owner; one person holds every role (EX-30) |
| Reviewer | none |
| Evidence class | baselines: conformance, resource accounting, closures, packages, API identity and a cross-check, with no decision; and one measurement, in its own sub-bundle, with one decision |

## 2. What it retains, against SJL-0.8's list

| SJL-0.8 item | Retained | Where |
|---|---|---|
| 1. Whole-suite test262, wide manifest, bytecode and native forms | **Yes**, both forms, each a whole run of the pinned suite the runner calls retainable | section 3 |
| 2. Closures of every JavaScript root under JIT, trimming and Native AOT | **Yes**, every root in all three modes, each publish with warnings as errors and each image run | section 6 |
| 3. The API baselines, with their hashes | **Yes** | section 5 |
| 4. Packages: the family's nuspecs and contents | **Yes**, the nuspecs and contents of the packed set; the consumer steps of the same collector failed at this base, section 8 | section 8 |
| 5. The differential harness, over its retained answers and against Broiler.JS | **Yes**, as a cross-check: no difference from the retained answers, and every finding against Broiler.JS listed as unadjudicated | section 9 |
| 6. The fuel-parity twins and the retained exhaustion verdicts | **Yes** | section 4 |
| 7. Measurements of the pilot's operations under SJL-0.9's rule | **Yes**, as a sub-bundle | section 7 |
| SJL-0.1: the retained inventory | **Yes** | section 10 |
| The rule E2 and E4 run | **Yes**, at the records' commit, with a negative control | section 11 |

## 3. Test262, both forms

The pinned suite is `tc39/test262` at `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, extracted from the
archive `src/tests/conformance/pins/test262.pin` names. Both runs went through the base's own
`eng/run-test262.py` and its conformance root built in Release, with the allowances and floors
`eng/collect-js-release-candidate.py` fixes, which is the procedure SJL-0.8 names as its model. The
collector read each run's effective limit vector back first.

| Run | Files | Runner's reading |
|---|---|---|
| `wide-bytecode` | `limits-wide-bytecode.log`, `wide-bytecode.run.log`, `.merge.log`, `.floor.log`, `.test262.json`, `.report.gz` (the merged report, every variant), `.failures.txt` | A whole, pinned run whose verdicts account for it. Exit 1, the ordinary outcome of a whole run. **Its floor holds**: `wide-bytecode.floor.log` |
| `wide-native` | the same files, `wide-native.*` | A whole, pinned run whose verdicts account for it. Exit 4, because **its floor could not be compared**: the floor in `src/tests/conformance/floors` was set on the `x86-64-win64` backend, and this run used `x86-64-sysv`, so the runner refused to hold one to the other and asked for a re-base, which this collector does not do |

**The two forms do not agree at the base, and the difference is deterministic.**
`forms-at-base.log` is the base's own `eng/compare-test262-forms.py --exempt-guest-loads` over the two
merged reports. Its class (c) rows are races between the fuel and wall-clock allowances, which that
script admits. Its unclassified rows are every one of them a tail-call test (`tco-*.js`) - the
strict variant, or for the four `tco-non-eval-*` files the sloppy one - that the bytecode form passes
and the native form fails with an uncaught `RangeError` on this RID. That is a property of the base, recorded so nobody later reads it as the pilot's: part A of
SJL-3's rule compares each form with itself (`--same-form`), so these rows are the native form's base
and not a difference the pilot is judged on.

## 4. The fuel-parity twins

`fuel-parity.log`: for each `*.small.js` twin of `src/tests/forms/wide` at the base, the smallest
completing fuel allowance, bisected under the bytecode form through the command-line root, and in each
form whether the twin completes at that figure and refuses with `AllowanceExhausted on Fuel` at one
less. **Every twin completes at its figure and refuses at one less, in both forms.** Part B of SJL-3's
rule holds the after commit to these figures. They differ from the table in section 4 of
`src/tests/forms/wide/README.md`, which was taken at an earlier commit; these are the base's.

The retained exhaustion verdicts are the `Exhausted` rows of both test262 runs' failure manifests and
merged reports.

## 5. The API baselines

`api-baselines.txt`: the core's `docs/api/public-api.txt` and the JavaScript family's
`src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`, which freezes every assembly of the
family, each by the SHA-256 of its LF bytes and by its git blob id at the base commit.

## 6. Closures

`closures/` is the base tree's own `eng/collect-js-evidence.py` compositions procedure, the one
bundle JS-10-004 used, called by this bundle's collector: the four JavaScript roots -
`ExecutionOnly`, `SliceCompiler`, `Conformance` and `Cli` - each published under JIT, trimming and
Native AOT with warnings as errors, each image's catalog printed with `--closure`, each image run on
its own workload, and each closure read off the published output (`closure-*.txt`, `catalog-*.txt`,
`publish-and-run.log`). **Every publish and every run exited zero.** Each closure and each catalog is
identical to bundle JS-10-004's, collected on `win-x64` at `88d8b51d`, apart from the RID line.

A Native AOT publish directory holds no managed assembly, so each `[aot]` section lists none. The
record of what ILC compiled into an image, the mstat file rule `I3` will read from SJL-2, is not
retained, because nothing asked ILC to write one at this base.

## 7. The measurement sub-bundle

[`measurement/`](measurement/) holds one evidence class, a measurement, with its own `manifest.txt`
(made read-only before any lane ran), `runs.tsv` (every repetition, warm-ups included and named),
`conditions.tsv` (every answer checked before and after each workload's lanes), `summary.txt` and
`hashes.txt`. It is **the base arm of part C** of [SJL-3's rule](../sjl-3-001/decision-rule.md),
measured alone: for each workload of `src/tests/forms/numbers` (committed in `f55f83c`, after the
base, which is why the collector reads them from its own tree and the manifest names that commit), in
both forms, the base arm B, the workload's control and an A/A lane B-again, interleaved inside every
repetition, with the repetition and warm-up counts the rule cites. The binaries are the base's.

**Its one decision, stated in the manifest before any lane ran:** each workload's attribution
E(B) = Q(B) - Q(control) is recorded as the base's figure when |E(B)| exceeds the A/A difference
|Q(B) - Q(B-again)|, and as below resolution otherwise. **Every workload, in both forms, reads
`recorded`**, and the figures are in `summary.txt` and nowhere else. Every lane's answer held before
and after, and every timed child reported one effective-configuration line. **It decides nothing
about SJL-3**: that milestone rebuilds B from the base commit and runs it interleaved with its after
arm.

## 8. Packages

`packages/` is the base tree's own `eng/collect-js-packages.py`, the collector of bundle JS-10-002,
run by this bundle's collector at the base. **Its pack step succeeded**: `pack.log`, and the produced
metadata of every packed package in `nuspecs.txt` and its file list in `contents.txt`. That is what
SJL-0.8 item 4 asks for, and every produced dependency names a `Broiler.VM.` package.

**Its consumer steps failed, and the failure is the base's.** The base commit `0087e8d7` rewrote
`samples/NuGet.config` to restore from nuget.org rather than from the local feed alone, and pinned
`samples/Directory.Packages.props` to `0.1.0-preview.6`. The collector points every proxy at a closed
port so that a package found anywhere but the local feed fails the restore, so all three restores,
the Native AOT consumer and the negative control fail on the unreachable nuget.org (`consumer.log`,
`collector.log`). No consumer, rollback or negative-control result is retained as a base. The
programme's governance item V20, which describes the consumer as restoring only from
`../artifacts/feed`, describes the tree before that commit.

`packages/hashes.txt` is that collector's own list, written before this bundle's collector hid the
machine's paths in `pack.log` and `consumer.log`; those two entries therefore name the unhidden
bytes. `manifest.json` names every retained file as it is.

## 9. The differential cross-check

`differential/` is the base tree's own `eng/run-differential.py`, run over every probe of
`src/tests/differential` with the base's command-line root, and against the Broiler.JS engine built
in Release from a clean checkout of `44a214dc` (`--against-kind broiler-js`): `run.log` and the
harness's full report, compressed, `report.json.gz`. **It is a cross-check, and it meets no
conformance clause.**

- **Against the retained answers: no difference.** Every finding names Broiler.JS; Broiler.VM's run
  of every probe exited zero and printed its retained answers.
- **Against Broiler.JS: every declared divergence still differs, and no declaration is stale.** The
  declarations were taken against Broiler.JS `c249764` on 2026-09-30.
- **Every undeclared finding is listed as unadjudicated.** They are in `run.log`, probe by probe.
  They include cases Broiler.JS prints and Broiler.VM does not in the Unicode normalization probes;
  two probes whose Broiler.JS run ended abnormally, one of them a normalization probe; two module
  probes Broiler.JS did not finish inside the harness's timeout; and early errors the two engines
  answer differently. **None is
  adjudicated here**: SJL-0 refreshes no declaration, and a declaration written now would adjudicate
  against a Broiler.JS commit no programme milestone has yet moved. Bundle `sjl-3-001` reads this run
  as its base under template T8.

## 10. The inventory

`inventory.md` is the programme's responsibility inventory re-derived at the five named commits of
section 1, with a disposition and an area milestone for every row; `inventory-checks.txt` records, for
every member name it checked, whether a declaration was found and in which file. It replaces the
survey copy in `docs/javascript-shared-libraries/inventory.md` as the copy later gates read, and lists
every difference from the survey in its last section. It cites no line numbers.

## 11. The rule E2 and E4 run

`rules-e2-e4.log`: the architecture tests' group E rules, run alone at the records' commit
`61c24e9` (ADR 0014 filed and indexed), each passing. `rule-e4-control-failing.log` is the same run
of rule E4 with ADR 0014's index row removed, which fails naming
`0014-shared-javascript-libraries.md` as not listed; `rule-e4-control-passing.log` is the run after
the row was restored, which passes, with the index byte-identical to the committed one.
`architecture-tests.log` is the whole architecture suite at the same commit, with this bundle's
files and the programme documents' updates in the working tree, built from `Broiler.VM.slnx` in
Release, every test passing; it reads this README as a review document.
`decision-text-diff.log` shows, for exit-gate clause 3, that `docs/adr/0011-source-level-profile-contract.md`
and `docs/roadmap.md` gained only the two pointer paragraphs between the base and `61c24e9`: no line
of either file is removed.

## 12. What this bundle excludes, and does not claim

**Not retained:**

- **The pristine consumer, its rollback and its negative control**, for the reason section 8 gives.
- **ILC's mstat records** of the Native AOT images (section 6).
- **Any run on another RID.** One machine, `linux-x64`. A base for a cut-over judged on another RID is
  taken there, and nothing here stands for it. The native form's floor was set on `win-x64`.
- **Broiler.JS's half of SJL-0.8.** Broiler.JS's own bundle `sjl-0-001` holds it, in its own
  repository, and does not exist yet. Section 9's Broiler.JS build is the comparison engine of a
  cross-check, not a Broiler.JS baseline.
- **Fuzzing, and negative controls** other than rule E4's. A baseline judges nothing a control could
  falsify.

**Not claimed:**

- **No verdict on any milestone.** A base is read by a later rule; SJL-0's own gate is read off its
  clauses by the programme ledger.
- **No figure outside this directory.** The measurement's figures are in `measurement/summary.txt`, the
  test262 totals in the run logs, and no plan or ledger quotes either.
- **No comparison of the two engines**, in conformance or in speed. Validation layer L5 forbids
  comparing the two test262 runners' totals, and Broiler.VM's release gate 10 forbids a speed
  comparison.
- **No review.** Nothing here has been read by anyone but its author (EX-30).

## 13. How it was collected

```text
python3 docs/evidence/sjl-0-001/collect.py --tree <a checkout of 0087e8d7> \
    --suite <the pinned suite, extracted> --out docs/evidence/sjl-0-001 [--only STEP ...]
```

The checkout of the base was a separate working tree of this repository at `0087e8d7`, clean
throughout, with the pinned suite extracted beside it. The collector ran in four invocations, from
this repository's tree, and only gained steps between them; every step it took is in the retained
file unchanged:

1. `identity`, `test262`, `api` and `twins`, from a revision of `collect.py` that had only those four
   steps (sha256 `c3f98728160098e84aaced9b38ea587dbadae4a66f5ad4bad2b7cd94b3985c62`), at 20:04 UTC;
2. `measure`, on an otherwise idle machine, after `f55f83c` committed the rule and the workloads;
3. `closures` and `packages`;
4. `differential`, with `--against-broiler-js` and `--against-root` naming the Broiler.JS build.

`forms-at-base.log` was produced afterwards from the two retained merged reports, with the base's
`eng/compare-test262-forms.py`. The architecture tests, the rule E4 control and the decision-text
diff were run from this repository's tree at `61c24e9`. Paths are written `<tree>` for the base's
checkout, `<scratch>` and `<bundle>` for working directories, `<root>` for this repository and
`<broiler-js>` for the Broiler.JS checkout. `manifest.json` names the sources at the base by blob id
and every retained file by the SHA-256 of its LF bytes.

**Environment.** `linux-x64`, four processors, .NET SDK `10.0.401` (`identity.txt`); Native AOT with
the platform's clang and lld. The test262 runs took four jobs each and had the machine to themselves
apart from light document work.
