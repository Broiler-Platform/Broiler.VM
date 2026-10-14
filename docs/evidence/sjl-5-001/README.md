<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-5-001 - the text area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-5 of the shared JavaScript libraries programme,
[`sjl-5-text-algorithms.md`](../../javascript-shared-libraries/milestones/sjl-5-text-algorithms.md). **Rule:**
[`decision-rule.md`](decision-rule.md), written at SJL-5.1 before any code of the milestone existed.
**Records:** [JSD-0070](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md). **Owner and reviewer:** one person holds every
role (EX-30), and nobody independent has read this bundle.

**State:** collection in progress. Nothing here is a verdict until its section says so. **Taken:** the
base (section 1) and the first engine-level runs on the text area (section 2). The after runs, their comparisons and the rest of SJL-5.6 are added when SJL-5.4's
cut-over has landed; [`collect.py`](collect.py) gains their steps then.

## 1. The base

| | |
|---|---|
| Commit | `e600da2`, a clean checkout ([`base/identity.txt`](base/identity.txt)): the rule's commit `acf2bec` and this bundle's collector, which is an evidence script. Outside the documents, it differs from the default branch after SJL-4's merge (`1f253ff`) only by `.gitattributes`, part B's six probes and part C's thirty documents with their README, which the identity lists |
| Taken | 2026-10-14, 16:48Z to 17:33Z, by `collect.py --only base-identity --only base-test262 --only base-twins --only base-json-modules`, before any code of SJL-5 landed ([`base/collect.log`](base/collect.log)). Nothing else built, tested or generated on the machine meanwhile: the session edited files and ran git, and the Broiler.JS base ran on GitHub's runners |
| Part A | test262 at the pin in both forms, through bundle `sjl-0-001`'s procedure: [`base/wide-bytecode.report.gz`](base/wide-bytecode.report.gz) and [`base/wide-native.report.gz`](base/wide-native.report.gz), each with its run log, merge log, floor log, JSON report and failure manifest. Bytecode: 101,723 variants, 100,372 pass, 154 fail, 44 exhausted, 1,153 skipped. Native: 100,336 pass, 188 fail, 46 exhausted, 1,153 skipped. The bytecode floor holds; the native run's exit code 4 is the floor's re-basing note, because the floor was set in the win64 native form and this run is sysv, as in SJL-3's and SJL-4's runs |
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
