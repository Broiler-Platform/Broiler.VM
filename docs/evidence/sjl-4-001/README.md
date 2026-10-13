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
base (section 1). The after runs, their comparisons and the rest of SJL-4.6 are added when SJL-4.4's
cut-over has landed; [`collect.py`](collect.py) gains their steps then.

## 1. The base

| | |
|---|---|
| Commit | `d0e607e`, a clean checkout ([`base/identity.txt`](base/identity.txt)): the rule's commit `6ab3d47` and this bundle's collector, which is an evidence script. Outside the documents, it differs from the default branch after SJL-3's merge (`a04b2e5`) only by part B's ten probe files, which the identity lists |
| Taken | 2026-10-13, 16:34Z to 17:04Z, by `collect.py --only base-identity --only base-test262 --only base-twins`, before any code of SJL-4 landed, on an otherwise idle machine ([`base/collect.log`](base/collect.log)) |
| Part A | test262 at the pin in both forms, through bundle `sjl-0-001`'s procedure: [`base/wide-bytecode.report.gz`](base/wide-bytecode.report.gz) and [`base/wide-native.report.gz`](base/wide-native.report.gz), each with its run log, merge log, floor log, JSON report and failure manifest. Bytecode: 101,723 variants, 100,368 pass, 158 fail, 44 exhausted, 1,153 skipped. Native: 100,334 pass, 192 fail, 44 exhausted, 1,153 skipped. The bytecode floor holds; the native run's exit code 4 is the floor's re-basing note, because the floor was set in the win64 native form and this run is sysv, as in SJL-3's runs |
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
