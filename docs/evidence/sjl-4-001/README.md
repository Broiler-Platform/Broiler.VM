<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-4-001 - the numerics area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-4 of the shared JavaScript libraries programme,
[`sjl-4-numerics.md`](../../javascript-shared-libraries/milestones/sjl-4-numerics.md). **Rule:**
[`decision-rule.md`](decision-rule.md), written at SJL-4.1 before any code of the milestone existed.
**Records:** [JSD-0067](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0067-numerics-move-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md). **Owner and reviewer:** one person holds every
role (EX-30), and nobody independent has read this bundle.

**State:** collection in progress. Nothing here is a verdict until its section says so. **Taken:** the
base (section 1) and the engine-level vectors of SJL-4.2 (section 2). The after runs, their comparisons and the rest of SJL-4.6 are added when SJL-4.4's
cut-over has landed; [`collect.py`](collect.py) gains their steps then.

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
