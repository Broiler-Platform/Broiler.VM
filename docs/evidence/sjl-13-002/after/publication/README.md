<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-13-002, `after/publication/`: every JavaScript root published in three modes at step 2's after commit

**Collected:** 2026-11-15, at task 7.3 of step 2's evidence, by bundle sjl-7-001's
[`collect-area.py`](../../../sjl-7-001/collect-area.py) `--only cutover-closures`, run with `--out` outside the tree
from a clean clone of this repository detached at the after commit `467ad7c395234b335c816b8f49ae5b505740e32c`, as bundle
[`sjl-13-001`](../../../sjl-13-001/after/publication/README.md) collected step 1's closures. No collector of this
repository has a step for bundle sjl-13-002, as none had for sjl-13-001. This directory holds what it wrote, the Native
AOT image records aside (their SHA-256 are in [`closures/mstat.sha256`](closures/mstat.sha256), as in the bundles
before). **What it is for:** the rule's "What this file does not decide" leaves clause 8's publication to "the
collector's runs at this file's after commit", and rules K3, K4 and I7 read the closures a register row's evidence cell
names. **It is no verdict**: the bundle's README reads it in its section 5.

## How it was taken

- **The checkout:** `git clone --no-checkout` of this repository into a tmpfs, detached at the after commit, with a
  clean status ([`logs/checkout.txt`](logs/checkout.txt)); step 1's used a detached worktree.
- **The run:** `NuGetAudit=false python3 -I docs/evidence/sjl-7-001/collect-area.py --tree <clone> --out <scratch>
  --only cutover-closures`, from the clone's root, behind the bundle's hard idle gate and lock, with build servers and
  node reuse off in the environment ([`logs/`](logs/): the driver `publication.sh.txt`, the command line, exit, wall
  time, peak memory, the machine records and the gate's record). `NuGetAudit=false` is set as step 1's retained
  collection set it, where NuGet's audit refuses the Native AOT restore; nothing else is changed. The collector's own
  `dotnet publish` calls pass no `--disable-build-servers`.
- **The roots:** ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli. The Android root is exclusion EX-126,
  as before.
- **The identity record**, [`identity.txt`](identity.txt), is kept as the collector wrote it: its first line names
  bundle sjl-7-001's cut-over, the step's own bundle, where step 1's copy was changed to name its bundle.

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
| [`logs/`](logs/) | The run's record |
| `hashes.txt` | The SHA-256 of every other file here |
