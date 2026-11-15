<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-17-001, `after-closures/`: the closures and the rule checks at the after commit

**Collected:** 2026-11-15, at SJL-17.6, from a clean checkout of the after commit
`983d690adac63f6d117ff2cd2d9cca61fc16e3d2`, the merge of Broiler-Platform/Broiler.VM#191, SJL-17.4's cut-over, by the
method bundle `sjl-9-001`'s [`after-closures/`](../../sjl-9-001/after-closures/README.md) records for SJL-9.6's: bundle
sjl-7-001's `collect-area.py --only cutover-closures`, run from the checkout with `--out` outside the tree. This
directory holds what it wrote, the Native AOT image records aside, and [`identity.txt`](identity.txt) names this bundle
and this collection in its first line where the collector wrote sjl-7-001's; every other line is the collector's.
[`closures/mstat.sha256`](closures/mstat.sha256) holds the SHA-256 of each image record.

**What it is for:** JSD-0086 section 6 leaves the composition register's closures to SJL-17.6 ("SJL-17.6 retakes them
at the after commit"), and the milestone's clauses 6, 8 and 9 name rules I1 to I7 with I5's deletion list, every root
published and run in three modes, and optionality as rule I3 shows it with N28's budget. **What it is not:** a run of
this bundle's rule, which reads no closure and no rule check; and SJL-17's exit reading, which reads clauses and
follows Broiler.JS's SJL-17.5. Nothing here is a verdict: it is read in the bundle's section 10. No register row moves.

## Which commit, and how it was taken

[`local/checkout.txt`](local/checkout.txt): a `git clone --no-checkout` of this repository into a scratch directory,
then `git checkout --detach` of the after commit, with no line of `git status --porcelain`; `identity.txt` records the
tree clean, the product sources' digest, the SDK and the machine. The steps, by
[`closures.sh.txt`](../after/scripts/closures.sh.txt):
1. the collection ([`local/collect.*`](local/collect.out.txt));
2. the solution built in Release with `--disable-build-servers` in the same checkout, with no warning
   ([`local/i3-build-solution.out.txt`](local/i3-build-solution.out.txt),
   [`local/i3-build-warnings.txt`](local/i3-build-warnings.txt), empty);
3. the whole architecture suite with the five image records placed under the checkout's `src/i3-image-records/`, where
   rule I3's test reads each `mstat-<root>.mstat` whose `<root>` names a `Broiler.VM.Composition.JavaScript.*` root,
   with a TRX of every test ([`local/i3-architecture.out.txt`](local/i3-architecture.out.txt)); the TRX is not
   retained ([`architecture-trx.sha256`](architecture-trx.sha256)), and its rows for rules I1 to I7, N27 and N28 are
   [`rules.tsv`](rules.tsv), by [`rules-from-trx.py.txt`](../after/scripts/rules-from-trx.py.txt). The records were
   removed afterwards.

Each step ran behind the lock and the idle gate the bundle's section 5 describes ([`local/gate.log`](local/gate.log)):
a one-minute load of at most 1.0 and no `dotnet`, `testhost`, `VBCSCompiler`, `MSBuild`, `ilc`, `crossgen` or
`Broiler.*` process before it started, the machine recorded before and after with the whole process list, and no other
build, test or run on the machine while it ran. The collector's `dotnet publish` takes no `--disable-build-servers`;
the step ran with `MSBUILDDISABLENODEREUSE=1`, `DOTNET_CLI_USE_MSBUILD_SERVER=0` and `UseSharedCompilation=false` in its
environment, and the gate's `stop-servers` line after it names no leftover process. Every Native AOT publish restored
and published with NuGet's audit on (.NET SDK 10.0.404).

[`deletion-search.txt`](deletion-search.txt) is git reads alone at the after commit, by
[`deletion-search.sh.txt`](../after/scripts/deletion-search.sh.txt): every file, type and member of SJL-17.4's
deletion list, as the milestone's SJL-17.4 and JSD-0086 section 5 give it, searched in the tree, and rule I5's register
rows of SJL-17.4. [`deletion-search-base.txt`](deletion-search-base.txt) is the same search at the base, where it finds
each of them, which shows the search can.

## What it holds

| Record | What it holds |
|---|---|
| [`identity.txt`](identity.txt) | The commit, its cleanliness, the product sources' digest, the SDK and the machine |
| `closures/closure-<root>.txt` | Each root's non-framework assemblies in each mode |
| `closures/catalog-<root>.txt` | The catalog each root printed |
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of each admitted package the closures carry |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one |
| `closures/publish-and-run.log`, `closures/publish-and-run-polyglotcli.log` | Every publish and run, with its exit |
| [`compare.txt`](compare.txt) | This collection against bundle `sjl-9-001`'s `after-closures/closures/`, taken at this bundle's base, and against its `cutover/closures/`, file by file, by [`compare-closures.py.txt`](../after/scripts/compare-closures.py.txt) |
| [`rules.tsv`](rules.tsv) | Every test of rules I1 to I7, N27 and N28 in the architecture suite, with its outcome, and the suite's totals |
| [`deletion-search.txt`](deletion-search.txt), [`deletion-search-base.txt`](deletion-search-base.txt) | SJL-17.4's deletion list searched at the after commit and at the base |
| [`local/`](local/) | Every step's command, gate log, machine record, exit code and wall time |
| [`hashes.txt`](hashes.txt) | The SHA-256 of every other file of this directory, which `sha256sum -c` verifies |

## What it shows

- **Every JavaScript root this machine can publish was published and run in three modes**: ExecutionOnly,
  SliceCompiler, Conformance, Cli and PolyglotCli, each under the JIT, trimming and Native AOT, and every publish and
  run exited as it should ([`local/collect.out.txt`](local/collect.out.txt): "every root published and run in three
  modes: True"). The Android root is exclusion EX-126, as before (section 1's reading 10).
- **Every closure, every catalog and `admitted-packages.json` are the base's byte for byte**
  ([`compare.txt`](compare.txt)): bundle `sjl-9-001` collected its `after-closures/` at `68cad8b`, this bundle's base,
  and the after commit's roots carry the same assemblies, print the same catalogs and carry the same admitted packages,
  by version, content hash, archive hash and source commit; they are also SJL-9.4's `cutover/closures/` byte for byte.
  So no root's assemblies move at SJL-17.4, as JSD-0086 section 6 says.
- **Each Native AOT image record has a new digest.** The product sources differ: the after commit deletes the collator
  and calls the library's, and the records are not retained here or there, so their bytes cannot be compared. Rule I3
  read four of them (below).
- **The transcripts** have the same section headings, every publish, run and exit in the same order with the same exit
  codes, and differ, by `compare.txt`'s masks, only in the order of lines within a section, which the restore's and
  the build's parallel nodes decide, and in timings, the heap plateau check's readings and the addresses some checks
  print. Every difference is attributed.
- **The architecture suite passes whole** at the after commit with the image records in place, every test of rules I1
  to I7, N27 and N28 among them ([`rules.tsv`](rules.tsv)). Rule I3's test so reads the images of ExecutionOnly,
  SliceCompiler, Conformance and Cli; PolyglotCli's, whose root is `Broiler.VM.Composition.PolyglotCli`, was placed
  with them and is outside what it reads. Rule I5's tests pass with the register holding SJL-17.4's rows, and N28's
  test that holds the table data under its budget passes over the tables, which do not move.
- **SJL-17.4's deletion list is absent** at the after commit ([`deletion-search.txt`](deletion-search.txt)): no path
  `JsCollator.cs` or `JsCollationData.cs`, no declaration of `JsCollator`, `JsCaseFirst` or `JsCollationData`, and no
  declaration of `JsRealm.CollatorKeyData` or of `JsIntlTables.Collation` and its lazy; the names left are comments
  that say what went, and `JsCollatorObject.Collation`, the resolved option, a member of another type. Rule I5's
  register holds a row of SJL-17.4 for each type and member (`JsCollator`, `JsCaseFirst`, `JsCollationData`,
  `JsIntlTables.Collation`, `JsIntlTables.collation` and `JsRealm.CollatorKeyData`). At the base the same search finds
  each of them.
