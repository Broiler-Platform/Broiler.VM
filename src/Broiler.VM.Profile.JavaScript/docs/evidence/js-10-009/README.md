# Bundle JS-10-009 — the packages after the shared algorithms package's preview.5, a pristine consumer, and a rollback

**Collected:** 2026-10-17. **Milestone:** the shared JavaScript libraries programme's SJL-8, which it
does not close; JS-10, which it does not close either. **Owner:** the JavaScript profile owner.
**Reviewer:** none; every role is held by one person (EX-30).

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), taken again when the family's shared
algorithms dependency moved from `0.1.0-preview.4` to `0.1.0-preview.5`: `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.5`, which `Broiler.VM.Profile.JavaScript` and `.Compiler` declare, packed locally from
Broiler.JS `2d37a38` and not yet on nuget.org (JSC-312). Its preview.5 adds the time-zone and calendar
area of milestone SJL-8.3 beside the areas preview.4 carried, unchanged. *(Added 2026-10-17, later:
`0.1.0-preview.5` was published to nuget.org that day, from Broiler.JS `a04cd44`, with the same library
code, and the committed feed now holds the published package (JSC-313). This bundle's pack used the copy
packed from `2d37a38`.)* The bundle holds the candidate
pack of the whole solution with every package's metadata and file list, a pristine consumer of the
JavaScript profile's packages restoring and running with upstream feeds unreachable from a local feed
that also holds the shared packages, a rollback to the package set that was actually published and a
roll forward again, a negative control, and the consumer published and run as Native AOT. It replaces
[JS-10-008](../js-10-008/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to
the checkout.

**Evidence class: `packaging`.** **The one predeclared decision**, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-008's, in the words JSD-0061 gave it.
**Read as JSD-0064 and JSC-298 read it, every condition holds.** Section 3 says what that reading is.

**What this bundle is not.** **Not a publication**: the candidate was packed to a directory and
nothing was pushed anywhere. Not a support claim, and **not accepted**.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit 4df595b`, `tree clean`) and `hashes.txt`; section 2 says how that commit stands to the change that carries this bundle |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests and the admitted shared packages placed on the feed in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` at `4df595b` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 8,012,112 bytes, in `consumer.log` |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From a clean checkout of `4df595b` on 2026-10-17:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-009 --out <scratch>/js-10-009
```

and the six files copied here unchanged; `hashes.txt` checks them. The steps and the collector are
JS-10-008's. Before any restore the collector places on the local feed every package rule I1 admits,
from the committed feed `eng/packages/`, which now holds `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.5`, packed locally from Broiler.JS `2d37a38`, and the Unicode area's three packages,
`Broiler.Unicode.Algorithms`, `.Segmentation` and `.Generation`, all `0.1.0-preview.5` as
Broiler.Unicode's Publish workflow pushed them to nuget.org. The Native AOT packs were already in the
machine's package cache, as `manifest.txt`'s `aot-seeded` line lists.

**Why `4df595b`, and not the change that carries this bundle.** The baseline moves with the change
that moves the reference, and a change cannot carry evidence collected at itself. `4df595b` is that
change, the admission of `0.1.0-preview.5`; this bundle and the records naming it are the change after
it, and every path the two differ in is a documentation path, under a `docs/` directory, so the two have
the same product sources. The candidate's assemblies carry `+4df595b41a9ca058cb1b2315e3b5003fcbbc329d`
in their informational version for the same reason. Rule N37 does not hold at `4df595b` itself, whose
baseline still named JS-10-008's candidate, which declares `0.1.0-preview.4`, below the version the
allow-list then pinned; it holds from the change that adds this bundle, as it did from the change that
added JS-10-008 after `84017dd`.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102`, `local-feed` the only source named | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

**The decision's first condition, read as the baseline reads it.** `Broiler.VM.Profile.JavaScript`
and `Broiler.VM.Profile.JavaScript.Compiler` declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5`
and `Broiler.Unicode.Algorithms` `0.1.0-preview.5`; `Broiler.VM.Profile.JavaScript.Format` declares
`Broiler.Unicode.Algorithms` `0.1.0-preview.5`; `Broiler.VM.Profile.JavaScript.Intl` declares
`Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation`, both `0.1.0-preview.5`. Every other
dependency in `nuspecs.txt` is a `Broiler.VM.` package. JSD-0064 admits a shared package that rule I1
admits for shipping, at the version its allow-list pins, and this bundle reads the condition as
JS-10-005 to JS-10-008 did. Read literally, the condition does not hold, and no baseline would be taken.

**What moved since JS-10-008.** In `nuspecs.txt`: the shared algorithms dependency of the runtime and
the compiler, from `0.1.0-preview.4` to `0.1.0-preview.5`, and every package's `repository` element,
which names the commit and the branch. `contents.txt` lists the same files in every package. The Native
AOT image is 8,012,112 bytes, where JS-10-008's was 8,032,880; this bundle was collected with SDK
10.0.401, and JS-10-008 with 10.0.112, so the two images are not compiled by the same toolchain, and
this bundle decides nothing on the 20,768 bytes between them.

## 4. Negative controls

- **The restore that must fail**, which would have succeeded had nuget.org been reachable.
- **Rule N37's witnesses**, under `src/tests/Broiler.VM.Architecture.Tests/witnesses/js-packages/`,
  unchanged. Each fails the rule with the violation it names.

## 5. Exclusions

1. **Nothing was published**, and the candidate's version is not a release's.
2. **One machine, one RID**, `linux-x64`; the consumer's other RIDs are not run.
3. **The rolled-back set has no Intl package and no Unicode package**, so the rollback exercises the
   consumer's core half only. That is what the previous set was.
4. **The shared algorithms package comes from the committed feed**, `eng/packages/`, as packed locally
   from Broiler.JS `2d37a38`, and is not on nuget.org: a release of the family that depends on it waits
   for its publication (`eng/verify-feed.ps1`). The Unicode packages come from the same feed, as
   published to nuget.org; nothing was restored from nuget.org during the run.
5. **The candidate predates the cut-over.** It was packed before SJL-8.5's product cut-over, which
   changes no package's dependencies or files; the consumer's Intl and Temporal checks ran the code the
   family had before it.
6. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
