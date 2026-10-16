<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-7-001, `cutover/`: the closures at SJL-7.4's cut-over

**Collected:** 2026-10-16, by `collect-area.py --only cutover-closures`, from a clean checkout. **What it
is for:** rules K3, K4 and I7 read the closures a composition register row's evidence cell names, and
SJL-7.4's change gave five JavaScript rows shared assemblies they had not declared:
`Broiler.Unicode.Algorithms` in each, and `Broiler.Unicode.Segmentation` in the two whose closure carries
the Intl assembly. The rows' evidence cells name [`closures/`](closures/) since that change (JSD-0075,
section 1). **What it is not:** part of the bundle's base or of SJL-7.6's after runs, which the bundle's
README describes; the bundle's `area-manifest.json` does not list this directory.

## Which commit

[`identity.txt`](identity.txt) names `a9be9f1`, a commit of SJL-7.4's change before this directory, bundle
JS-10-007 and the records that name them were added to it. The change that carries them could not be the
commit they were collected at, and rule K4 holds a row's declaration to its closure in every commit, so the
references and their evidence land together. **Every path that differs between `a9be9f1` and that change is
a documentation path**, under `docs/` or a `docs/` directory, and `identity.txt` records the digest of the
product sources - the SHA-256 of `git ls-tree -r`'s lines for every other path, joined by newlines with a
final newline - which anyone can recompute at the change and find equal:

```sh
git ls-tree -r HEAD | python3 -c "import sys,hashlib; l=[x for x in sys.stdin.read().splitlines() if not x.split('\t',1)[-1].startswith('docs/') and '/docs/' not in x.split('\t',1)[-1]]; print(hashlib.sha256(('\n'.join(l)+'\n').encode()).hexdigest(), len(l))"
```

The assemblies the closures carry name `+a9be9f1b57fc4a342f88a4674142f4dbdebec380` in their informational
version for the same reason.

## What it holds

**Every JavaScript root this machine can publish was published and run in three modes**: ExecutionOnly,
SliceCompiler, Conformance, Cli and PolyglotCli, each under the JIT, trimming and Native AOT, with the closure
of each mode, the catalog each printed, and the transcripts of every publish and run in
`publish-and-run.log` and `publish-and-run-polyglotcli.log`. The Android root is exclusion EX-126, as before. Every
publish and every run exited as it should - the slice compiler's 646 checks pass in every mode, with three
not run on this machine and claimed by nothing, as before, the execution-only root's 22 pass in every mode,
every catalog is identical across a root's three modes, and PolyglotCli's `nottext.wasm` is refused by
design - and each Native AOT publish leaves no managed assembly beside the image.

| Root | `Broiler.Unicode.Algorithms` | `Broiler.Unicode.Segmentation` |
|---|---|---|
| ExecutionOnly | under the JIT and trimming | not carried |
| SliceCompiler | under the JIT and trimming | under the JIT and trimming |
| Conformance | under the JIT and trimming | under the JIT and trimming |
| Cli | under the JIT and trimming | not carried |
| PolyglotCli | under the JIT and trimming | not carried |

| Record | What it holds |
|---|---|
| [`closures/admitted-packages.json`](closures/admitted-packages.json) | Rule I7's identity of each admitted package the closures carry: its version, its content hash as the allow-list pins it and as the committed feed's copy has it, and the commit its nuspec names |
| [`closures/mstat.sha256`](closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one; rule I3 read them once at this change, placed in the tree for that run alone |
