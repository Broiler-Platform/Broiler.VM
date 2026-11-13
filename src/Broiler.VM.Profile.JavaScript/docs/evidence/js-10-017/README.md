# Bundle JS-10-017 — the packages after the shared stream moved to `0.1.0-preview.11`

**Collected:** 2026-11-13. **Owner:** the JavaScript profile owner. **Reviewer:** none; every role is
held by one person (EX-30).

**What this bundle is.** Release gate 8's facts ([section 22](../../roadmap.gates.md#22-release-gates)),
taken again when the shared stream's packages the family declares, `Broiler.ECMAScript.Algorithms`,
`Broiler.ECMAScript.Intl` and `Broiler.ECMAScript.Frontend`, moved from `0.1.0-preview.10` to
`0.1.0-preview.11`, published to nuget.org on 2026-11-13 from Broiler.JS `e4b6b49e` (tag
`ecmascript-v0.1.0-preview.11`), with the test-only `Broiler.ECMAScript.Generation`
([JSC-327](../../roadmap.corrections.md#jsc-327)). It replaces [JS-10-016](../js-10-016/README.md) as the bundle
the profile's package baseline, [`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two
to each other and to the checkout.

**Evidence class: `packaging`.** The one predeclared decision, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-016's.

**What this bundle is not.** Not a publication: the candidate was packed to a directory and nothing was
pushed anywhere. Not a support claim, and not accepted.

## How it was collected

At Broiler.VM `539b291`, the change that moves the packages, on Linux x64 with SDK 10.0.404, with the
machine's own package cache, in a clean checkout of that commit cloned from the branch it was pushed to:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-017 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-017 --rid linux-x64 --aot-pack-version 10.0.15
```

The Native AOT step's platform packs are 10.0.15, the runtime this machine's SDK carries; JS-10-016 seeded
10.0.12, the one its machine's SDK 10.0.401 carried.

**Why `manifest.txt` says the tree was dirty.** The collector places every admitted shared package on the
consumer's local feed from the committed feed `eng/packages/`, which holds no package since 2026-10-30. As
for JS-10-012 to JS-10-016, the seven admitted packages were copied there for the run, untracked: the four of
the `0.1.0-preview.11` stream as downloaded from nuget.org for the admission, and the three Unicode packages
from the machine's package cache, each archive checked against its `archiveHash` in the allow-list. They are
the seven lines `manifest.txt` lists under `dirty:`. No tracked file differed from `539b291`.

## What it shows

`consumer.log` holds each step's own output: the candidate restored and run with upstream unreachable,
the rollback to the published `0.1.0-preview.5` set and the roll forward, the negative control failing to
restore a version only nuget.org holds, and the consumer published and run as Native AOT. Every step went
as the decision asks.

Against JS-10-016, the retained metadata differs only where the change and the place of collection put
it: `Broiler.VM.Profile.JavaScript.nuspec` declares `Broiler.ECMAScript.Algorithms` and
`Broiler.ECMAScript.Intl` at `0.1.0-preview.11`, and `Broiler.VM.Profile.JavaScript.Compiler.nuspec`
declares `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Frontend` at `0.1.0-preview.11`, the
versions the allow-list pins; and every `.nuspec` names the branch and commit it was packed from in its
repository element. No package declares a new dependency, and every package carries the files JS-10-016's
did; the sizes in `contents.txt` move with the code, the notices and the metadata since `26b436f`.
`hashes.txt` records the digests of the files the collector wrote.
