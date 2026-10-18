# Bundle JS-10-010 — the packages after the shared algorithms package's preview.6, a pristine consumer, and a rollback

**Collected:** 2026-10-18. **Milestone:** the shared JavaScript libraries programme's SJL-10, which it
does not close; JS-10, which it does not close either. **Owner:** the JavaScript profile owner.
**Reviewer:** none; every role is held by one person (EX-30).

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), taken again when the family's shared
algorithms dependency moved from `0.1.0-preview.5` to `0.1.0-preview.6`: `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.6`, which `Broiler.VM.Profile.JavaScript` and `.Compiler` declare, packed locally from
Broiler.JS `bf81d1b` and not yet on nuget.org (JSC-314). Its preview.6 adds the Temporal core of
milestone SJL-10.3 beside the areas preview.5 carried, unchanged. The bundle holds the candidate
pack of the whole solution with every package's metadata and file list, a pristine consumer of the
JavaScript profile's packages restoring and running with upstream feeds unreachable from a local feed
that also holds the shared packages, a rollback to the package set that was actually published and a
roll forward again, a negative control, and the consumer published and run as Native AOT. It replaces
[JS-10-009](../js-10-009/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to
the checkout.

**Evidence class: `packaging`.** **The one predeclared decision**, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, is JS-10-009's, in the words JSD-0061 gave it.
**Read as JSD-0064 and JSC-298 read it, every condition holds.** Section 3 says what that reading is.

**What this bundle is not.** **Not a publication**: the candidate was packed to a directory and
nothing was pushed anywhere. Not a support claim, and **not accepted**.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit 66b0877`, `tree clean`) and `hashes.txt`; section 2 says how that commit stands to the change that carries this bundle |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests and the admitted shared packages placed on the feed in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` at `66b0877` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 8,040,992 bytes, in `consumer.log` |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From a clean checkout of `66b0877` on 2026-10-18:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-010 --out <scratch>/js-10-010
```

and the six files copied here unchanged; `hashes.txt` checks them. The steps and the collector are
JS-10-009's. Before any restore the collector places on the local feed every package rule I1 admits,
from the committed feed `eng/packages/`, which now holds `Broiler.ECMAScript.Algorithms` and the
test-only `Broiler.ECMAScript.Generation`, both `0.1.0-preview.6`, packed locally from Broiler.JS
`bf81d1b`, and the Unicode area's three packages, `Broiler.Unicode.Algorithms`, `.Segmentation` and
`.Generation`, all `0.1.0-preview.5` as Broiler.Unicode's Publish workflow pushed them to nuget.org. The
Native AOT packs were already in the machine's package cache, as `manifest.txt`'s `aot-seeded` line lists.

**Why `66b0877`, and not the change that carries this bundle.** The baseline moves with the change
that moves the reference, and a change cannot carry evidence collected at itself. `66b0877` is that
change, the admission of `0.1.0-preview.6`; this bundle and the records naming it are the change after
it, and every path the two differ in is a documentation path, under a `docs/` directory, so the two have
the same product sources. The candidate's assemblies carry `+66b087790c30e958e5982d52ee459ecce56b7e56`
in their informational version for the same reason. Rule N37 does not hold at `66b0877` itself, whose
baseline still named JS-10-009's candidate, which declares `0.1.0-preview.5`, below the version the
allow-list then pinned; it holds from the change that adds this bundle, as it did from the change that
added JS-10-009 after `4df595b`.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102`, `local-feed` the only source named | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

**The decision's first condition, read as the baseline reads it.** `Broiler.VM.Profile.JavaScript`
and `Broiler.VM.Profile.JavaScript.Compiler` declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.6`
and `Broiler.Unicode.Algorithms` `0.1.0-preview.5`; `Broiler.VM.Profile.JavaScript.Format` declares
`Broiler.Unicode.Algorithms` `0.1.0-preview.5`; `Broiler.VM.Profile.JavaScript.Intl` declares
`Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation`, both `0.1.0-preview.5`. Every other
dependency in `nuspecs.txt` is a `Broiler.VM.` package; no package declares the test-only
`Broiler.ECMAScript.Generation`. JSD-0064 admits a shared package that rule I1 admits for shipping, at
the version its allow-list pins, and this bundle reads the condition as JS-10-005 to JS-10-009 did. Read
literally, the condition does not hold, and no baseline would be taken.

**What moved since JS-10-009.** In `nuspecs.txt`: the shared algorithms dependency of the runtime and
the compiler, from `0.1.0-preview.5` to `0.1.0-preview.6`, and every package's `repository` element,
which names the commit and the branch. `contents.txt` lists the same files in every package. The Native
AOT image is 8,040,992 bytes, where JS-10-009's was 8,012,112; both were collected with SDK 10.0.401 on
this machine, and this bundle decides nothing on the 28,880 bytes between them.

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
   from Broiler.JS `bf81d1b`, and is not on nuget.org: a release of the family that depends on it waits
   for its publication (`eng/verify-feed.ps1`). The Unicode packages come from the same feed, as
   published to nuget.org; nothing was restored from nuget.org during the run.
5. **The candidate predates the cut-over.** It was packed before SJL-10.4's product cut-over, which
   changes no package's dependencies or files; the consumer's Intl and Temporal checks ran the code the
   family had before it.
6. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
