<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-9-001, `cutover/`: the closures at SJL-9.4's first stage

**Collected:** 2026-11-03, by bundle sjl-7-001's `collect-area.py --only cutover-closures` run with `--out` outside the
tree, from a clean checkout; this directory holds what it wrote, the Native AOT image records aside, and
`identity.txt` names this bundle in its first line where the collector wrote sjl-7-001's. **What it is for:** rules
K3, K4 and I7 read the closures a composition register row's evidence cell names, and SJL-9.4's change gives every
JavaScript row a shared assembly it had not declared: `Broiler.ECMAScript.Intl`, the shared Intl library the profile
references since that change (JSD-0082, decision D9). The rows' evidence cells name [`closures/`](closures/) since
then. **What it is not:** a run of this bundle's rule. The rule's base and after runs are taken at the commits
[`decision-rule-area.md`](../decision-rule-area.md) defines, and the closures of every root in three modes are taken
again at the after commit (SJL-9.6).

## Which commit

[`identity.txt`](identity.txt) names `2b798a7`, the branch's commit that cuts sub-area (a) over, before this directory
and the register rows that name it were added. The register's rows and the closures they are held to land together,
so the commit that carries them could not be the commit they were collected at. **Every path that differs between
`2b798a7` and that commit is a documentation path**, under `docs/` or a `docs/` directory, and `identity.txt` records
the digest of the product sources - the SHA-256 of `git ls-tree -r`'s lines for every other path, joined by newlines
with a final newline - which anyone can recompute at the later commit and find equal:

```sh
git ls-tree -r HEAD | python3 -c "import sys,hashlib; l=[x for x in sys.stdin.read().splitlines() if not x.split('\t',1)[-1].startswith('docs/') and '/docs/' not in x.split('\t',1)[-1]]; print(hashlib.sha256(('\n'.join(l)+'\n').encode()).hexdigest(), len(l))"
```

*(Revised 2026-11-04, at SJL-9.4's records.)* The closures were taken again, by the same step and the same method,
from a clean checkout of `b3fd2ba`, the branch's change after its last change to a path outside the documents,
`69f9e6a`, so that every product-source change of sub-areas (b) to (e) and of the records is under them.
[`identity.txt`](identity.txt) names `b3fd2ba` and the product sources' digest, which `69f9e6a` gives too, and which the
branch's later changes, documents only, keep. Every closure, catalog and identity of an admitted package is byte for
byte the one the first collection retained; the transcripts differ in timings and in a heap plateau check's readings
alone, and each Native AOT image record has a new digest. The paragraph above describes the first collection, which
named `2b798a7` and recorded the product sources' digest `37743fbc…`; it is kept as written.

*(Revised 2026-11-04, later, after the review of SJL-9.4's records. The paragraph above said the branch's later
changes are "documents only"; one is not.)* `65ad719` changed one path outside the documents after `b3fd2ba`: rule
I5's architecture test, `src/tests/Broiler.VM.Architecture.Tests/I5RetiredDuplicatesRuleTests.cs`, which no root
compiles. The product sources' digest moved with it, to `14c1478e…` at `1fac124`, while no root's input moved. The
closures were taken again by the same step from a clean checkout of `1fac124`, the change after it:
- every root's JIT and trimmed publish and run, every closure's JIT and trimmed lines and every catalog matched the
  committed ones byte for byte, but every Native AOT publish failed at restore. NuGet's audit reported error NU1903,
  advisory GHSA-p3jp-9v83-76w9 against `Microsoft.DotNet.ILCompiler` 10.0.12, which its vulnerability data had not
  reported at the collection from `b3fd2ba`, and the collector publishes with warnings as errors;
- a further collection from the same checkout, with NuGet's audit turned off in the environment and nothing else
  changed, published and ran every root in three modes, and every closure, catalog and admitted package's identity
  matched the committed ones byte for byte.

Neither is committed, since the first is incomplete and the second departs from the step's method; both are kept
outside the tree. The collection from `b3fd2ba`, which [`identity.txt`](identity.txt) names, stays this directory's.
SJL-9.6 takes the closures again at the after commit, where the advisory has to be met first.

The packages the closures carry are the ones the allow-list admits, restored from nuget.org;
[`closures/admitted-packages.json`](closures/admitted-packages.json) names each by its version, its content and
archive hashes and the commit its nuspec names (rule I7).

## What it holds

**Every JavaScript root this machine can publish was published and run in three modes**: ExecutionOnly,
SliceCompiler, Conformance, Cli and PolyglotCli, each under the JIT, trimming and Native AOT, with the closure of each
mode, the catalog each printed, and the transcripts of every publish and run in `publish-and-run.log` and
`publish-and-run-polyglotcli.log`. The Android root is exclusion EX-126, as before. Every publish and every run exited
as it should: the slice compiler's 721 checks pass in every mode, with three not run on this machine and claimed by
nothing, the Intl, tzdb and Temporal checks among them; the execution-only root's 22 pass in every mode; every catalog
is identical across a root's three modes, and to the one sjl-7-001's `cutover/` retains; PolyglotCli's `nottext.wasm`
is refused by design; and each Native AOT publish leaves no managed assembly beside the image.

Against sjl-7-001's `cutover/closures/`, every closure differs in one line per managed mode, the same in every root:
`Broiler.ECMAScript.Intl`, under the JIT and under trimming. Trimming keeps it in every root, the three that compose
no Intl data among them, because the profile's Intl surface is built at run time from whether a composition hands
it data, so the trimmer cannot see that a root never builds it.

| Record | What it holds |
|---|---|
| [`identity.txt`](identity.txt) | The commit, its cleanliness, the product sources' digest, the SDK and the machine |
| `closures/closure-<root>.txt` | Each root's non-framework assemblies in each mode |
| `closures/catalog-<root>.txt` | The catalog each root printed |
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of each admitted package the closures carry |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one |
| `closures/publish-and-run.log`, `closures/publish-and-run-polyglotcli.log` | Every publish and run, with its exit |
