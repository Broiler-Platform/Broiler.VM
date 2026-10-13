# Bundle JS-10-006 — the packages after the numerics cut-over, a pristine consumer, and a rollback

**Collected:** 2026-10-13. **Milestone:** the shared JavaScript libraries programme's SJL-4, work
package SJL-4.4, which it does not close; JS-10, which it does not close either. **Owner:** the
JavaScript profile owner. **Reviewer:** none; every role is held by one person (EX-30).

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), taken again after the numerics cut-over moved
the family's shared dependency to `Broiler.ECMAScript.Algorithms` `0.2.0-preview.1`
([JSD-0069](../../decisions/0069-the-profile-on-the-shared-numerics.md)): the candidate pack of the
whole solution with every package's metadata and file list, a pristine consumer of the JavaScript
profile's packages restoring and running with upstream feeds unreachable from a local feed that also
holds the shared package, a rollback to the package set that was actually published and a roll forward
again, a negative control, and the consumer published and run as Native AOT. It replaces
[JS-10-005](../js-10-005/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to
the checkout (JSC-302).

**Evidence class: `packaging`.** **The one predeclared decision**, written into `manifest.txt` by
`eng/collect-js-packages.py` before anything was packed, in the words JSD-0061 gave it: the candidate
set is recorded as this profile's packable baseline if every family package's metadata names only
Broiler.VM packages, the consumer restores and runs against it with upstream unreachable, rolls back to
the published previous set and forward again, and the negative control fails to restore.
**Read as JSD-0064 and JSC-298 read it, every condition holds.** Section 3 says what that reading is.

**What this bundle is not.** **Not a publication**: the candidate was packed to a directory and
nothing was pushed anywhere. Not a support claim, and **not accepted**.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit 6bd3009`, `tree clean`) and `hashes.txt` |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests and the admitted shared package placed on the feed in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` at `6bd3009` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 7,602,160 bytes, in `consumer.log`; the composition roots' closures are bundle [sjl-4-001](../../../../../docs/evidence/sjl-4-001/README.md)'s |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From a clean checkout of `6bd3009`, the cut-over's commit, on 2026-10-13:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-006 --out <scratch>/js-10-006
```

and the six files copied here unchanged; `hashes.txt` checks them. The steps are JS-10-005's, and the
collector is unchanged since it: before any restore it places on the local feed every package rule I1
admits, from the committed feed `eng/packages/`, which now holds `Broiler.ECMAScript.Algorithms`
`0.2.0-preview.1` alone. The consumer references none of it directly; it restores it through
`Broiler.VM.Profile.JavaScript` and `.Compiler`. The rollback builds the same source with
`BroilerVmIntl=false` and `BroilerVmAbstractionsNamespace=false`, as JS-10-005 records.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102`, `local-feed` the only source named | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

Each run prints the informational version of every assembly it loaded, so the transcript shows which
set answered - the candidate's carry `+6bd3009527ca293e54fd73ee7a4704fe79935800` - rather than which
was asked for.

**The decision's first condition, read as the baseline reads it.** `Broiler.VM.Profile.JavaScript`
and `Broiler.VM.Profile.JavaScript.Compiler` declare `Broiler.ECMAScript.Algorithms`
`0.2.0-preview.1`; every other dependency in `nuspecs.txt` is a `Broiler.VM.` package.
[JSD-0064](../../decisions/0064-the-package-baseline-admits-shared-dependencies.md) admits a shared
package that rule I1 admits for shipping, at the version its allow-list pins, and JSC-298 reads release
gate 8's "no foreign dependency" the same way; this bundle reads the condition as they do, as JS-10-005
did. Read literally, the condition does not hold, and no baseline would be taken.

**What moved since JS-10-005.** In `nuspecs.txt`, the two packages' dependency on the shared package,
from `0.1.0-preview.1` to `0.2.0-preview.1`, and every package's `repository` element, which names the
commit and, since this checkout was on a branch, the branch as well. `contents.txt` lists the same
files in every package; their sizes differ, the notices among them.

## 4. Negative controls

- **The restore that must fail**, which would have succeeded had nuget.org been reachable.
- **Rule N37's witnesses**, under `src/tests/Broiler.VM.Architecture.Tests/witnesses/js-packages/`,
  among them a transcript whose rollback failed a check and metadata declaring a shared package rule
  I1 admits only for tests. Each fails the rule with the violation it names.

## 5. Exclusions

1. **Nothing was published**, and the candidate's version is not a release's.
2. **One machine, one RID**, `linux-x64`; the consumer's other RIDs are not run.
3. **The rolled-back set has no Intl package**, so the rollback exercises the consumer's core half
   only. That is what the previous set was.
4. **The shared package is a preview from the committed feed**, `eng/packages/`, packed from Broiler.JS
   `01209c4`, as decision SJL-D-4 (c) has it for an area in development; nothing was restored from
   nuget.org.
5. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
