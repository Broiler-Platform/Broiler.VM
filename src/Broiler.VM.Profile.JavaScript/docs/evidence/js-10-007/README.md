# Bundle JS-10-007 — the packages after the Unicode cut-over's references, a pristine consumer, and a rollback

**Collected:** 2026-10-16. **Milestone:** the shared JavaScript libraries programme's SJL-7, work
package SJL-7.4, which it does not close; JS-10, which it does not close either. **Owner:** the
JavaScript profile owner. **Reviewer:** none; every role is held by one person (EX-30).

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), taken again when the Unicode cut-over gave the
family's packages dependencies they did not have: `Broiler.Unicode.Algorithms` `0.1.0-preview.4`,
which `Broiler.VM.Profile.JavaScript`, `.Format`, `.Compiler` and `.Intl` declare, and
`Broiler.Unicode.Segmentation` `0.1.0-preview.4`, which `.Intl` alone declares
([JSD-0074](../../decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md);
the cut-over's record, JSD-0075). It holds the candidate pack of the whole solution with every
package's metadata and file list, a pristine consumer of the JavaScript profile's packages restoring
and running with upstream feeds unreachable from a local feed that also holds the shared packages, a
rollback to the package set that was actually published and a roll forward again, a negative control,
and the consumer published and run as Native AOT. It replaces [JS-10-006](../js-10-006/README.md) as
the bundle the profile's package baseline, [`docs/packages.md`](../../packages.md), freezes, and rule
**N37** holds the two to each other and to the checkout (JSC-308).

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
| **Source** | `manifest.txt` (`commit a9be9f1`, `tree clean`) and `hashes.txt`; section 2 says how that commit stands to the change that carries this bundle |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests and the admitted shared packages placed on the feed in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` at `a9be9f1` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 7,643,472 bytes, in `consumer.log`; the composition roots' closures are in bundle [sjl-7-001](../../../../../docs/evidence/sjl-7-001/cutover/README.md)'s `cutover/` directory |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From a clean checkout of `a9be9f1` on 2026-10-16:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-007 --out <scratch>/js-10-007
```

and the six files copied here unchanged; `hashes.txt` checks them. The steps are JS-10-006's, and the
collector is unchanged since it: before any restore it places on the local feed every package rule I1
admits, from the committed feed `eng/packages/`, which now holds `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.4` and the Unicode area's three packages, `Broiler.Unicode.Algorithms`,
`.Segmentation` and `.Generation`, all `0.1.0-preview.4`. The consumer references none of them
directly; it restores the first three through the family's packages, and nothing declares the fourth,
which rule I1 admits test-only. The rollback builds the same source with `BroilerVmIntl=false` and
`BroilerVmAbstractionsNamespace=false`, as JS-10-005 records.

**Why `a9be9f1`, and not the change that carries this bundle.** The baseline must move in the change
that adds the references, so that rule N37 holds at every commit, and a change cannot carry evidence
collected at itself. `a9be9f1` is that change's tree before this bundle, the composition roots' closures
and the records naming them were added to it; nothing else was. Every path that differs between the two
is a documentation path, under `docs/` or a `docs/` directory, so the two have the same product sources:
the digest bundle sjl-7-001's `cutover/identity.txt` records over `git ls-tree -r`, every path outside
any `docs/` directory, is the same at both, and anyone can recompute it at the change. The candidate's
assemblies carry `+a9be9f1b57fc4a342f88a4674142f4dbdebec380` in their informational version for the
same reason.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102`, `local-feed` the only source named | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

Each run prints the informational version of every assembly it loaded, so the transcript shows which
set answered - the candidate's carry `+a9be9f1b57fc4a342f88a4674142f4dbdebec380` - rather than which
was asked for.

**The decision's first condition, read as the baseline reads it.** `Broiler.VM.Profile.JavaScript`
and `Broiler.VM.Profile.JavaScript.Compiler` declare `Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`
and `Broiler.Unicode.Algorithms` `0.1.0-preview.4`; `Broiler.VM.Profile.JavaScript.Format` declares
`Broiler.Unicode.Algorithms` `0.1.0-preview.4`; `Broiler.VM.Profile.JavaScript.Intl` declares
`Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation`, both `0.1.0-preview.4`. Every other
dependency in `nuspecs.txt` is a `Broiler.VM.` package. [JSD-0064](../../decisions/0064-the-package-baseline-admits-shared-dependencies.md)
admits a shared package that rule I1 admits for shipping, at the version its allow-list pins, and
JSC-298 reads release gate 8's "no foreign dependency" the same way; this bundle reads the condition as
they do, as JS-10-005 and JS-10-006 did. Rule I1 admits each of the three shared packages for
shipping, for the projects that declare it and no other. Read literally, the condition does not hold, and no baseline would be
taken.

**What moved since JS-10-006.** In `nuspecs.txt`: the Unicode area's dependencies above, which no
earlier pack declared; the runtime's and the compiler's dependency on `Broiler.ECMAScript.Algorithms`,
from `0.2.0-preview.1` to `0.1.0-preview.4`, the version the checkout has referenced since the Date
cut-over (JSC-306); and every package's `repository` element, which names the commit. `contents.txt`
lists the same files in every package; their sizes differ, the notices among them. In
`consumer.log`, the feed holds the Unicode area's three packages beside the shared algorithms package,
and the Native AOT image is 7,643,472 bytes, where JS-10-006's was 7,602,160.

## 4. Negative controls

- **The restore that must fail**, which would have succeeded had nuget.org been reachable.
- **Rule N37's witnesses**, under `src/tests/Broiler.VM.Architecture.Tests/witnesses/js-packages/`,
  among them a transcript whose rollback failed a check and metadata declaring a shared package rule
  I1 admits only for tests. Each fails the rule with the violation it names.

## 5. Exclusions

1. **Nothing was published**, and the candidate's version is not a release's.
2. **One machine, one RID**, `linux-x64`; the consumer's other RIDs are not run.
3. **The rolled-back set has no Intl package and no Unicode package**, so the rollback exercises the
   consumer's core half only, with the Unicode code the published set carried in its own assemblies.
   That is what the previous set was.
4. **The shared packages are previews from the committed feed**, `eng/packages/`: the Unicode area's
   three packed locally from Broiler.Unicode `6e14f93` and not yet on nuget.org, and the shared
   algorithms package as published from Broiler.JS `d1e4b11`. Nothing was restored from nuget.org.
   `eng/verify-feed.ps1` fails a release whose consumer cannot restore every dependency outside it from
   nuget.org, so no release of these packages can be cut until the Unicode packages are published.
5. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
