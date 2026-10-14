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
base (section 1). The after runs, their comparisons and the rest of SJL-5.6 are added when SJL-5.4's
cut-over has landed; [`collect.py`](collect.py) gains their steps then.

## 1. The base

| | |
|---|---|
| Commit | `e600da2`, a clean checkout ([`base/identity.txt`](base/identity.txt)): the rule's commit `acf2bec` and this bundle's collector, which is an evidence script. Outside the documents, it differs from the default branch after SJL-4's merge (`1f253ff`) only by `.gitattributes`, part B's six probes and part C's thirty documents with their README, which the identity lists |
| Taken | 2026-10-14, 16:48Z to 17:33Z, by `collect.py --only base-identity --only base-test262 --only base-twins --only base-json-modules`, before any code of SJL-5 landed ([`base/collect.log`](base/collect.log)). Nothing else built, tested or generated on the machine meanwhile: the session edited files and ran git, and the Broiler.JS base ran on GitHub's runners |
| Part A | test262 at the pin in both forms, through bundle `sjl-0-001`'s procedure: [`base/wide-bytecode.report.gz`](base/wide-bytecode.report.gz) and [`base/wide-native.report.gz`](base/wide-native.report.gz), each with its run log, merge log, floor log, JSON report and failure manifest. Bytecode: 101,723 variants, 100,372 pass, 154 fail, 44 exhausted, 1,153 skipped. Native: 100,336 pass, 188 fail, 46 exhausted, 1,153 skipped. The bytecode floor holds; the native run's exit code 4 is the floor's re-basing note, because the floor was set in the win64 native form and this run is sysv, as in SJL-3's and SJL-4's runs |
| Part B | [`base/fuel-parity.log`](base/fuel-parity.log): each of the ten wide twins and the six text probes, its smallest completing allowance bisected under the bytecode form, then each form at that figure (completes, with its answer) and at one less (refused on fuel). Both forms agree at every figure |
| Part C | [`base/json-modules-1.txt`](base/json-modules-1.txt) and [`base/json-modules-2.txt`](base/json-modules-2.txt): the harness run twice over the thirty documents of `src/tests/forms/text/json-modules` and the twelve `.json` files of the pinned suite's `test/`, each document's outcome, artifact hash and first diagnostic's code. The two runs are byte-identical: 25 documents compiled, 17 refused, 14 of them `InvalidJsonModule` and 3 `NestingTooDeep` |
