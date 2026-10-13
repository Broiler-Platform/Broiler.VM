# Bundle JS-10-005 — the packages after the first shared dependency, a pristine consumer, and a rollback

**Collected:** 2026-10-13. **Milestone:** the shared JavaScript libraries programme's SJL-3, work
packages SJL-3.4 and SJL-3.6, which it does not close; JS-10, which it does not close either.
**Owner:** the JavaScript profile owner. **Reviewer:** none; every role is held by one person (EX-30).

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), taken again after the number-conversion
cut-over gave the family its first dependency outside Broiler.VM
([JSD-0066](../../decisions/0066-the-profile-on-the-shared-number-conversions.md)): the candidate pack
of the whole solution with every package's metadata and file list, a pristine consumer of the
JavaScript profile's packages restoring and running with upstream feeds unreachable from a local feed
that also holds `Broiler.ECMAScript.Algorithms`, a rollback to the package set that was actually
published and a roll forward again, a negative control, and the consumer published and run as Native
AOT. It replaces [JS-10-002](../js-10-002/README.md) as the bundle the profile's package baseline,
[`docs/packages.md`](../../packages.md), freezes, and rule **N37** holds the two to each other and to
the checkout (JSC-300).

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
| **Source** | `manifest.txt` (`commit 49ea00e`, `tree clean`) and `hashes.txt` |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests and the admitted shared package placed on the feed in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` at `49ea00e` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 7,598,048 bytes, in `consumer.log`; the composition roots' closures are bundle [sjl-3-001](../../../../../docs/evidence/sjl-3-001/README.md)'s |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From a clean checkout of `49ea00e` on 2026-10-13:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-005 --out <scratch>/js-10-005
```

and the six files copied here unchanged; `hashes.txt` checks them. The steps are JS-10-002's, with
two differences the commit carries:

1. **The admitted shared package travels with the family's.** Before any restore, the collector
   places on the local feed every package rule I1 admits, from the committed feed `eng/packages/`:
   `Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`. The consumer references none of it directly; it
   restores it through `Broiler.VM.Profile.JavaScript` and `.Compiler`.
2. **The rollback builds the same source with two switches.** The published `0.1.0-preview.5` has no
   Intl package, as before (`BroilerVmIntl=false`), and declares the core's `VmProfileDescriptor` in
   `Broiler.VM` rather than `Broiler.VM.Abstractions`, where the candidate declares it since the core
   moved its contract types. The consumer names that type, so the rollback also passes
   `BroilerVmAbstractionsNamespace=false`, which drops the one `using` directive the candidate needs.
   `49ea00e` adds the switch: without it the rollback's build failed with `CS0234`, and a first
   collection at the cut-over commit also failed every restore, because `samples/Directory.Packages.props`
   had stopped reading the version the collector passes. Neither failed collection is retained.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102`, `local-feed` the only source named | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

Each run prints the informational version of every assembly it loaded, so the transcript shows which
set answered - the candidate's carry `+49ea00e`, the rolled-back set's `+ebd7079` - rather than which
was asked for.

**The decision's first condition, read as the baseline reads it.** `Broiler.VM.Profile.JavaScript`
and `Broiler.VM.Profile.JavaScript.Compiler` declare `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.1`; every other dependency in `nuspecs.txt` is a `Broiler.VM.` package. JSD-0061's words
"only Broiler.VM packages" were written before any shared package existed.
[JSD-0064](../../decisions/0064-the-package-baseline-admits-shared-dependencies.md) admits a shared
package that rule I1 admits for shipping, at the version its allow-list pins, and JSC-298 reads release
gate 8's "no foreign dependency" the same way; this bundle reads the condition as they do, and says so
here rather than in the manifest, which the collector writes before anything runs. Read literally, the
condition does not hold, and no baseline would be taken.

**The source break the rollback found.** A consumer of `0.1.0-preview.5` that names
`VmProfileDescriptor` does not compile against the candidate until it adds
`using Broiler.VM.Abstractions;`, and the reverse. That is a change to the core's public surface
between two preview sets, not a defect of the packages; this bundle records it because the rollback is
the step that shows it, and the consumer's switch is how the same source still builds against both.

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
4. **The shared package is a preview from the committed feed**, `eng/packages/`, as decision SJL-D-4
   (c) has it for an area in development; nothing was restored from nuget.org. *(Added 2026-10-13,
   after collection: the package was published to nuget.org that day, and the committed feed now holds
   the published package, whose content hash differs from the one `consumer.log` lists; its pin says
   why.)*
5. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
