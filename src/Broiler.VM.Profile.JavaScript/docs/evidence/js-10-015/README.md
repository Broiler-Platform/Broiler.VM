# Bundle JS-10-015 — the packages after the profile took the shared Intl package

**Collected:** 2026-11-03. **Owner:** the JavaScript profile owner. **Reviewer:** none; every role is
held by one person (EX-30).

**What this bundle is.** Release gate 8's facts ([section 22](../../roadmap.gates.md#22-release-gates)),
taken again when `Broiler.VM.Profile.JavaScript` took a new shared dependency at SJL-9.4:
`Broiler.ECMAScript.Intl` `0.1.0-preview.9`, published to nuget.org on 2026-11-03 from Broiler.JS `6f7284da`
and admitted for that project alone
([JSD-0082](../../decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D9). It replaces [JS-10-014](../js-10-014/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to the
checkout.

**Evidence class: `packaging`.** The one predeclared decision, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-014's.

**What this bundle is not.** Not a publication: the candidate was packed to a directory and nothing was
pushed anywhere. Not a support claim, and not accepted.

## How it was collected

At Broiler.VM `c4beaf1`, the change that admits the package and adds the reference, on Linux x64 with SDK
10.0.401, with the machine's own package cache:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-015 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-015 --rid linux-x64 --aot-pack-version 10.0.12
```

**Why `manifest.txt` says the tree was dirty.** The collector places every admitted shared package on the
consumer's local feed from the committed feed `eng/packages/`, which holds no package since 2026-10-30. As
for JS-10-012 to JS-10-014, the six admitted packages were downloaded from nuget.org, each archive checked
against its `archiveHash` in the allow-list, and copied into `eng/packages/` for the run, untracked; they
are the six lines `manifest.txt` lists under `dirty:`, and were removed after the run. No tracked file
differed from `c4beaf1`. JS-10-014 was collected with a package cache that held only the Native AOT packs
of runtime 10.0.12; this one ran with the machine's cache, which changes where the candidate pack's own
restore found its packages and nothing the consumer steps restore, each of which uses a fresh, empty
packages folder with upstream unreachable.

## What it shows

`consumer.log` holds each step's own output: the candidate restored and run with upstream unreachable,
the rollback to the published `0.1.0-preview.5` set and the roll forward, the negative control failing to
restore a version only nuget.org holds, and the consumer published and run as Native AOT. Every step went
as the decision asks.

Against JS-10-014, the retained metadata differs only where the change and the place of collection put
it: `Broiler.VM.Profile.JavaScript.nuspec` declares `Broiler.ECMAScript.Intl` `0.1.0-preview.9`, the version
the allow-list pins, and every `.nuspec` names this commit and branch in its repository element. No other
package declares a new dependency, and every package carries the files JS-10-014's did. `hashes.txt`
records the digests of the files the collector wrote.
