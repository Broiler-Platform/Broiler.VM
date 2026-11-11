<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-13-001, `after/publication/`: every JavaScript root published in three modes at step 1's after commit

**Collected:** 2026-11-10, at task 7.2 of step 1's evidence, by bundle sjl-7-001's
[`collect-area.py`](../../../sjl-7-001/collect-area.py) `--only cutover-closures`, run with `--out` outside the tree
from a clean detached worktree of the after commit `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`, as SJL-9.4 collected
bundle [`sjl-9-001`](../../../sjl-9-001/cutover/README.md)'s `cutover/closures/`. No collector of this repository has
a step for bundle sjl-13-001: `eng/collect-js-release-candidate.py` has none, and that step is the one the bundles
before took for the closures a composition register row names. This directory holds what it wrote, the Native AOT
image records aside (their SHA-256 are in [`closures/mstat.sha256`](closures/mstat.sha256), as in the bundles before),
and [`identity.txt`](identity.txt) names this bundle in its first line where the collector wrote sjl-7-001's.
**What it is for:** the rule's "What this file does not decide" leaves clause 8's publication of every root in three
modes to the collector's retained runs, closures and mstat files (rules I3, K4 and I7); rules K3, K4 and I7 read the
closures a register row's evidence cell names; and JSD-0083's decision D29 and JSD-0087's section 10 ask that the rows
of the roots that compile source name the shared syntax library once closures that carry it are retained. **It is no
verdict**: task 7.3 reads it.

## How it was taken

- **The first collection** published every root under the JIT and trimming, and every Native AOT publish failed at
  restore with NuGet's audit error NU1903 against `Microsoft.DotNet.ILCompiler` 10.0.12 (advisory
  GHSA-p3jp-9v83-76w9), which the collector, publishing with warnings as errors, does not pass; bundle sjl-9-001's
  `cutover/README.md` records the same advisory at SJL-9.4. It is not retained here.
- **The collection retained here** is the same step from the same worktree with `NuGetAudit=false` in the
  environment and nothing else changed, as the brief of this stage allows where NU1903 blocks a local Native AOT
  restore. Every root published and ran in all three modes; the collector says so in its last line.
- **The roots:** ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli. The Android root is exclusion EX-126,
  as before.

## What it holds

| Record | What it holds |
|---|---|
| [`identity.txt`](identity.txt) | The commit, its cleanliness, the product sources' digest, the SDK and the machine |
| `closures/closure-<root>.txt` | Each root's non-framework assemblies in each mode |
| `closures/catalog-<root>.txt` | The catalog each root printed |
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of each admitted package the closures carry |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record, which is not committed |
| `closures/publish-and-run.log`, `closures/publish-and-run-polyglotcli.log` | Every publish and run, as the collector wrote them |
| [`collector.log`](collector.log) | The collector's own output |
