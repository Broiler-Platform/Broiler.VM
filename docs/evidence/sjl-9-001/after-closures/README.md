<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-9-001, `after-closures/`: the closures at the after commit

**Collected:** 2026-11-11, at SJL-9.6, from a clean checkout of the after commit
`68cad8baf7403125d44d5007441d30ff07bc4edf`, the merge of Broiler-Platform/Broiler.VM#190, SJL-9.4's cut-over, by the
method [`cutover/README.md`](../cutover/README.md) records for SJL-9.4's collection: bundle sjl-7-001's
`collect-area.py --only cutover-closures`, run from the checkout with `--out` outside the tree. This directory holds
what it wrote, the Native AOT image records aside, and [`identity.txt`](identity.txt) names this bundle and this
collection in its first line where the collector wrote sjl-7-001's; every other line is the collector's.
[`closures/mstat.sha256`](closures/mstat.sha256) holds the SHA-256 of each image record, as `cutover/closures/` keeps
it. **What it is for:** the ledger's entry of 2026-11-04 and JSD-0085 section 11 leave the closures at the after commit
to SJL-9.6 (exit clause 14, "Every root is published and run in three modes"), and rules K3, K4 and I7 read closures
of this kind. **What it is not:** a run of this bundle's rule, which reads no closure; and the evidence the composition
register's rows name, which since SJL-13.3's step 1 is bundle `sjl-13-001`'s `after/publication/closures/`, a later
tree. No register row moves.

## Which commit, and how it was taken

[`local/checkout.txt`](local/checkout.txt): a `git clone --no-checkout` of this repository into a scratch directory,
then `git checkout --detach` of the after commit, with no line of `git status --porcelain`; `identity.txt` records the
tree clean and the product sources' digest, `14c1478e…` over the same entries as `cutover/identity.txt` reads them.
That is the digest `cutover/README.md` records for `1fac124`, the branch's commit after `65ad719`: the after commit's
product sources are the branch's last ones, and between `b3fd2ba`, which `cutover/identity.txt` names, and the after
commit the one path outside the documents that moves is `65ad719`'s, rule I5's architecture test, which no root
compiles (`git diff --name-only b3fd2ba 68cad8b`, outside `docs/`).

The collection ran as one step behind the idle gate this bundle's section 2 describes
([`scripts/gate.sh.txt`](scripts/gate.sh.txt), [`scripts/run.sh.txt`](scripts/run.sh.txt), by
[`scripts/closures.sh.txt`](scripts/closures.sh.txt)): a one-minute load of at most 1.0 and no `dotnet`, `testhost`,
`VBCSCompiler`, `MSBuild`, `ilc`, `crossgen` or `Broiler.*` process before it started
([`local/gate.log`](local/gate.log)), the machine recorded before and after ([`local/collect.machine.txt`](local/collect.machine.txt)),
and no other build, test or run on the machine while it ran. The collector's `dotnet publish` takes no
`--disable-build-servers`; the step ran with `MSBUILDDISABLENODEREUSE=1`, `DOTNET_CLI_USE_MSBUILD_SERVER=0` and
`UseSharedCompilation=false` in its environment, which keep the build servers off as that switch does, and the gate's
check after it found none left. Every Native AOT publish restored and published: NuGet's audit raised no error with
the SDK and compiler packages on this machine (.NET SDK 10.0.404), and the audit was not turned off.

## What it holds

**Every JavaScript root this machine can publish was published and run in three modes**: ExecutionOnly,
SliceCompiler, Conformance, Cli and PolyglotCli, each under the JIT, trimming and Native AOT, and every publish and run
exited as it should ([`local/collect.out.txt`](local/collect.out.txt): "every root published and run in three modes:
True"). The Android root is exclusion EX-126, as before.

| Record | What it holds |
|---|---|
| [`identity.txt`](identity.txt) | The commit, its cleanliness, the product sources' digest, the SDK and the machine |
| `closures/closure-<root>.txt` | Each root's non-framework assemblies in each mode |
| `closures/catalog-<root>.txt` | The catalog each root printed |
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of each admitted package the closures carry |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one |
| `closures/publish-and-run.log`, `closures/publish-and-run-polyglotcli.log` | Every publish and run, with its exit |
| [`compare.txt`](compare.txt) | This collection against `cutover/closures/`, file by file, by [`scripts/compare-closures.py.txt`](scripts/compare-closures.py.txt) |
| [`local/`](local/) | Every step's command, gate log, machine record, exit code and wall time |
| [`hashes.txt`](hashes.txt) | The SHA-256 of every other file of this directory, which `sha256sum -c` verifies |

## Against `cutover/`

[`compare.txt`](compare.txt) compares every file with `cutover/closures/`'s, byte for byte, and, for a transcript that
differs, each publish and run section with timings masked:
- **Every closure, every catalog and `admitted-packages.json` are `cutover/closures/`'s byte for byte**, in every root
  and every mode: the after commit's roots carry the same assemblies, print the same catalogs and carry the same
  admitted packages, by version, content hash, archive hash and source commit, as SJL-9.4's collection from `b3fd2ba`.
- **Each Native AOT image record has a new digest** (`mstat.sha256`). The records are not retained here or there, so
  their bytes cannot be compared. The cause is not the code, whose product sources are those the earlier collections
  read: the retake from `b3fd2ba` gave every record a new digest on the same SDK, as `cutover/README.md` records, and
  this collection was taken on .NET SDK 10.0.404 where `cutover/` was taken on 10.0.401. Rule I3 read the records
  themselves (below).
- **The transcripts** have the same lines in the same sections, every publish, run and exit in the same order with
  the same exit codes, and differ only in three ways: timings (restore and build times, each check's elapsed time and
  the run times some checks print beside their answers); the heap plateau check's readings; and, in the publish
  sections alone, the order in which the restore and the build print their per-project lines, which their parallel
  nodes decide. The slice compiler's checks pass in every mode, with three not run on this machine and claimed by
  nothing, and the execution-only root's in every mode, as at `cutover/`. Five of the slice compiler's checks print
  stack and handler addresses, which differ from run to run, and are masked too.

So the after commit's closures are SJL-9.4's, and no difference is unattributed.

## Rule I3 on the image records

The image records the collection left were placed under the checkout's `src/`, where rule I3's test reads every
`mstat-<root>.mstat`, and the whole architecture suite ran there, as SJL-9.4's collection read them and bundle
sjl-7-001's `rules` step does ([`scripts/i3-read.sh.txt`](scripts/i3-read.sh.txt)); then they were removed. Each step
ran behind the gate.
- The first attempt built the architecture tests' project alone, and four tests of rules B3, B5 and B5c, which read
  every shipping assembly the solution builds, failed for want of those builds; I3's test passed
  ([`local/i3-architecture.out.txt`](local/i3-architecture.out.txt)). It is kept, and the script was changed to build
  the solution first.
- The second built the solution in Release with `--disable-build-servers`, with no warning
  ([`local/i3-build-solution.out.txt`](local/i3-build-solution.out.txt)), and ran the suite with the records in place:
  every test passed, I3's among them ([`local/i3-architecture-2.out.txt`](local/i3-architecture-2.out.txt)). So rule I3
  holds on each root's Native AOT image at the after commit as on its project graph: no compiler-free root holds a
  shared frontend assembly, and no root carries optional data that `docs/compositions.md` does not declare in its
  closure.
