# Bundle JS-10-016 — the packages after the compiler took the shared syntax library

**Collected:** 2026-11-09. **Owner:** the JavaScript profile owner. **Reviewer:** none; every role is
held by one person (EX-30).

**What this bundle is.** Release gate 8's facts ([section 22](../../roadmap.gates.md#22-release-gates)),
taken again when `Broiler.VM.Profile.JavaScript.Compiler` took a new shared dependency at SJL-13.3's step 1,
`Broiler.ECMAScript.Frontend` `0.1.0-preview.10`, and the shared stream's other packages the family declares,
`Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Intl`, moved from `0.1.0-preview.9` to it. The stream
was published to nuget.org on 2026-11-04 from Broiler.JS `29061d65`, and the syntax library is admitted for
the compiler alone
([JSD-0083](../../decisions/0083-the-shared-syntax-library-and-the-wide-path.md), decision D29). It replaces
[JS-10-015](../js-10-015/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to the
checkout.

**Evidence class: `packaging`.** The one predeclared decision, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-015's.

**What this bundle is not.** Not a publication: the candidate was packed to a directory and nothing was
pushed anywhere. Not a support claim, and not accepted.

## How it was collected

At Broiler.VM `26b436f`, the change that admits the package and adds the reference, on Linux x64 with SDK
10.0.401, with the machine's own package cache:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-016 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-016 --rid linux-x64 --aot-pack-version 10.0.12
```

**Why `manifest.txt` says the tree was dirty.** The collector places every admitted shared package on the
consumer's local feed from the committed feed `eng/packages/`, which holds no package since 2026-10-30. As
for JS-10-012 to JS-10-015, the seven admitted packages were taken from nuget.org (the four of the
`0.1.0-preview.10` stream downloaded for the admission, the three Unicode packages from the machine's package
cache), each archive checked against its `archiveHash` in the allow-list, and copied into `eng/packages/` for
the run, untracked; they are the seven lines `manifest.txt` lists under `dirty:`, and were removed after the
run. No tracked file differed from `26b436f`.

## What it shows

`consumer.log` holds each step's own output: the candidate restored and run with upstream unreachable,
the rollback to the published `0.1.0-preview.5` set and the roll forward, the negative control failing to
restore a version only nuget.org holds, and the consumer published and run as Native AOT. Every step went
as the decision asks.

Against JS-10-015, the retained metadata differs only where the change and the place of collection put
it: `Broiler.VM.Profile.JavaScript.Compiler.nuspec` declares `Broiler.ECMAScript.Frontend` `0.1.0-preview.10`
beside `Broiler.ECMAScript.Algorithms` `0.1.0-preview.10`; `Broiler.VM.Profile.JavaScript.nuspec` declares
`Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Intl` at `0.1.0-preview.10`, the versions the
allow-list pins; and every `.nuspec` names this branch and commit in its repository element. No other
package declares a new dependency, and every package carries the files JS-10-015's did; the sizes in
`contents.txt` move with the code and the notices since `c4beaf1`. `hashes.txt` records the digests of the
files the collector wrote.
