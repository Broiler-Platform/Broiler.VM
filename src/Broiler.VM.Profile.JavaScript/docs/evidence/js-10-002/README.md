# Bundle JS-10-002 — the packages, a pristine consumer, and a rollback

**Collected:** 2026-10-05. **Milestone:** JS-10, which it does not close.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** Release gate 8's facts
([section 22](../../roadmap.gates.md#22-release-gates)), retained once: the candidate pack of the
whole solution with every package's metadata and file list, a pristine consumer of the JavaScript
profile's packages restoring and running with upstream feeds unreachable, a rollback to the package
set that was actually published and a roll forward again, a negative control, and the consumer
published and run as Native AOT. Phase F9's slice R3, decision
[JSD-0061](../../decisions/0061-packages-consumers-and-the-release-candidate.md). The profile's
package baseline, [`docs/packages.md`](../../packages.md), freezes what it shows, and rule **N37** holds
the two to each other and to the checkout.

**Evidence class: `packaging`.** **The one predeclared decision**, written into `manifest.txt` before
anything was packed: the candidate set is recorded as this profile's packable baseline if every family
package's metadata names only Broiler.VM packages, the consumer restores and runs against it with
upstream unreachable, rolls back to the published previous set and forward again, and the negative
control fails to restore. Every condition holds.

**What this bundle is not.** **Not a publication**: the candidate was packed to a directory and
nothing was pushed anywhere. Not a support claim, and **not accepted**: the reviewer field above says
`none` and means it.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit 6ac061e`, `tree clean`) and `hashes.txt` |
| **Dependencies and corpus** | `nuspecs.txt` (every dependency every produced package declares); the previous set's digests in `consumer.log` |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, the unreachable upstream, the packs seeded for Native AOT) |
| **Procedure** | section 2 below and `eng/collect-js-packages.py` |
| **Results** | `pack.log`, `nuspecs.txt`, `contents.txt`, `consumer.log` |
| **Negative controls** | the restore of a version only nuget.org holds, in `consumer.log`; rule N37's five witnesses - section 4 |
| **Closure** | the consumer's Native AOT image, 7,561,152 bytes, in `consumer.log`; no closure report |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From the checkout at `6ac061e`, a clean tree, on 2026-10-05:

```sh
python3 eng/collect-js-packages.py --bundle JS-10-002 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-002
```

1. **The manifest first**, made read-only before anything was packed.
2. **The candidate pack**: `dotnet pack Broiler.VM.slnx` at `0.1.0-preview.6.f9-candidate`, a version
   no release would carry. Eleven packages, each `.nuspec` and each file list retained.
3. **The previous set, as published**: the six packages the consumer references at
   `0.1.0-preview.5`, downloaded from nuget.org and hashed. That set has no Intl package; none was
   published.
4. **Three restores from nothing**, each with a fresh packages folder and HTTP cache, every proxy
   variable pointing at a closed port, and `samples/NuGet.config` listing one local source: the
   candidate, the **rollback** to `0.1.0-preview.5` with the consumer's Intl half switched off, and the
   **roll forward** to the candidate.
5. **The negative control**: a restore of exactly `0.1.0-preview.4`, which nuget.org holds and the
   feed does not. It fails with `NU1102`, naming `local-feed` as the only source searched.
6. **Native AOT**: the consumer published for `linux-x64` from the candidate and run. The SDK's own
   platform packs for that publish - six, named in the manifest, none a Broiler.VM package or a
   dependency of one - are the one thing seeded into that step's packages folder.

## 3. Results

| Step | Package set | Checks | Exit |
|---|---|---|---|
| Restore and run | candidate | 6 of 6 | 0 |
| Roll back and run | published `0.1.0-preview.5` | 4 of 4; Intl and Temporal not run, the set has no Intl package | 0 |
| Roll forward and run | candidate | 6 of 6 | 0 |
| Negative control | `0.1.0-preview.4`, absent from the feed | restore refused, `NU1102` | 1 |
| Native AOT, publish and run | candidate | 6 of 6 | 0 |

Each run prints the informational version of every assembly it loaded, so the transcript shows which
set answered - the candidate's carry `+6ac061e`, the rolled-back set's `+ebd7079` - rather than which
was asked for.

**No foreign dependency.** Every dependency in `nuspecs.txt` is a `Broiler.VM.` package.

## 4. Negative controls

- **The restore that must fail**, step 5, which would have succeeded had nuget.org been reachable.
- **Rule N37's five witnesses**, under
  `src/tests/Broiler.VM.Architecture.Tests/witnesses/js-packages/`: a baseline naming a package the
  checkout does not pack, one omitting a packed package, one misstating a dependency, metadata
  declaring a foreign dependency, and a transcript whose rollback failed a check. Each fails the rule
  with the violation it names.

## 5. Exclusions

1. **Nothing was published**, and the candidate's version is not a release's.
2. **One machine, one RID**, `linux-x64`; the consumer's other RIDs are not run.
3. **The rolled-back set has no Intl package**, so the rollback exercises the consumer's core half
   only. That is what the previous set was.
4. **The packed descriptions of three family packages were stale** - the profile's said it implemented
   only the slice manifest, the format's that only format version 1 exists, the lowering's that it had
   no tokenizer. They are corrected in the checkout after this collection; this bundle's `nuspecs.txt`
   keeps the text as packed.
5. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
