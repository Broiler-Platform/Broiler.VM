# Bundle JS-10-014 — the packages after the shared algorithms package's preview.9

**Collected:** 2026-11-03. **Owner:** the JavaScript profile owner. **Reviewer:** none; every role is
held by one person (EX-30).

**What this bundle is.** Release gate 8's facts ([section 22](../../roadmap.gates.md#22-release-gates)),
taken again when the family's shared algorithms dependency moved from `0.1.0-preview.8` to
`0.1.0-preview.9`: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.9`, which `Broiler.VM.Profile.JavaScript`
and `.Compiler` declare, published to nuget.org on 2026-11-03 from Broiler.JS `6f7284da`, with
`0.1.0-preview.8`'s code ([JSC-322](../../roadmap.corrections.md#jsc-322)). It replaces
[JS-10-013](../js-10-013/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to the
checkout ([JSC-323](../../roadmap.corrections.md#jsc-323)).

**Evidence class: `packaging`.** The one predeclared decision, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-013's.

**What this bundle is not.** Not a publication: the candidate was packed to a directory and nothing was
pushed anywhere. Not a support claim, and not accepted.

## How it was collected

At Broiler.VM `3008a99`, the change that moves the version, on Linux x64 with SDK 10.0.401, with a new
package cache that held only the Native AOT packs of runtime 10.0.12:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-014 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-014 --rid linux-x64 --aot-pack-version 10.0.12
```

**Why `manifest.txt` says the tree was dirty.** The collector places every admitted shared package on the
consumer's local feed from the committed feed `eng/packages/`, which holds no package since 2026-10-30. As
JS-10-012 and JS-10-013 did, the admitted packages were downloaded from nuget.org, each archive checked
against its `archiveHash` in the allow-list, and copied into `eng/packages/` for the run, untracked; they
are the five lines `manifest.txt` lists under `dirty:`, and were removed after the run. No tracked file
differed from `3008a99`. A first run without them is not retained: its consumer restores refused the
shared packages, which the local feed did not hold, and no baseline was taken from it.

## What it shows

`consumer.log` holds each step's own output: the candidate restored and run with upstream unreachable,
the rollback to the published `0.1.0-preview.5` set and the roll forward, the negative control failing to
restore a version only nuget.org holds, and the consumer published and run as Native AOT. Every step went
as the decision asks.

Against JS-10-013, the retained metadata differs only where the move and the place of collection put it:
the runtime's and the compiler's `.nuspec` declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.9`, the
version the allow-list pins, and every `.nuspec` names this commit and branch in its repository element.
No package declares the test-only `Broiler.ECMAScript.Generation`, and every package carries the files
JS-10-013's did. `hashes.txt` records the digests of the files the collector wrote.
