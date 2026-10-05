<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0061 - Packages, a pristine consumer, and the release-candidate run

**Status:** Proposed, 2026-10-05. **Owner decision pending.** It records phase F9's slice R3 under
[JSD-0059](0059-the-release-under-the-mvp-programme.md): release gates 8 and 9's facts, built and
retained - the JavaScript family's package baseline and the rule that holds it, a pristine consumer
with a rollback to the package set actually published, and a release-candidate run of the pinned
suite under every manifest and form a run can name, each with its own totals, failure manifest and
effective limit vector.

**What the owner decided.** On 2026-10-05 the repository owner asked for phase F9 to be done. Nothing
here is signed beyond that request, and **nothing here publishes a package**.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are held
by one person**, and it does not claim the co-signature is independent.

**Milestone:** JS-10. Phase F9, slice R3.

---

## 1. Packages and consumers (release gate 8)

- **A pristine consumer**, `samples/Broiler.VM.Sample.JavaScriptConsumer`: no project reference, one
  local feed, the empty `samples/Directory.Build.props`. It composes the profile from its packages,
  runs two programs, sees the lowering's and the verifier's refusals reach it, and, built with the
  Intl package, runs an `Intl` program and a `Temporal` one. Rule **A14** is revised to admit it with
  the three core packages and the JavaScript family's four, and ADR 0001 records the revision.
- **The collection**, `eng/collect-js-packages.py`, and **bundle
  [JS-10-002](../evidence/js-10-002/README.md)** from a clean tree at `6ac061e`: the candidate pack's
  metadata; the consumer restoring and running from nothing with upstream unreachable; a **rollback**
  to `0.1.0-preview.5` as published on nuget.org and a roll forward; a restore of a version only
  nuget.org holds, refused; and the consumer as Native AOT. Every step went as its predeclared
  decision required.
- **The package baseline**, [`docs/packages.md`](../packages.md): four packages, each with its
  dependencies and files, dated as this budget. **Rule N37** holds it in both directions to the
  family's packable projects and their references and to the bundle's retained metadata, holds every
  produced `.nuspec` to declaring no foreign dependency, and holds the bundle's transcript to having
  shown each consumer step. Five witnesses.

## 2. The release-candidate run (release gate 9)

- **A mode, `--effective-limits`**, in the conformance root: it builds the runtime exactly as a
  variant's is built - the same manifest, declined surfaces and allowances - verifies one script with
  the front end and request a variant under that manifest is lowered with, and prints each dimension
  of the ceilings the verified handle froze. The vector beside a run's totals is then read back from
  the runtime, not restated from the profile's defaults.
- **The collection**, `eng/collect-js-release-candidate.py`, builds the conformance root once from a
  clean commit, copies it aside, and takes four whole runs with it: the wide manifest in the bytecode
  and native forms, the slice manifest, and the numeric manifest's native form. Each run retains its
  merged report compressed, its JSON summary, its merge transcript, its failure manifest - every
  variant that failed or spent an allowance - and its limit vector; the two runs with a floor are held
  to it by the runner's own `--floor`. **Nothing combines figures across runs and no percentage is
  computed.**
- **Bundle [JS-10-003](../evidence/js-10-003/README.md)** is that collection; its README quotes each
  run's totals.

## 3. Choices

- **Rolling back to what was published**, not to a second pack of the same commit. The core's own
  consumer rolls back between two suffixes packed from one tree, which shows that NuGet resolves two
  versions; a rollback to `0.1.0-preview.5` as downloaded from nuget.org shows that a consumer can
  return to the package set people could actually have taken, built from `ebd7079` - which its
  assemblies' informational versions confirm.
- **The Intl half switches off for the rollback** rather than the rollback being skipped: the
  previous set has no Intl package, and the transcript says which checks it could not run.
- **Upstream is made unreachable, not only unlisted**: a fresh packages folder and HTTP cache and
  every proxy variable pointed at a closed port, so that the negative control's refusal is evidence
  rather than configuration.
- **Four runs, not one with a breakdown**: gate 9 asks every claimed manifest for its own totals, and
  a breakdown of one run by feature tag would be a reading of that run, not a run.
- **The Native AOT step seeds six SDK packs by name** - the runtime packs, the trimmer's tasks, the
  compiler - which a RID-specific publish downloads from the SDK's feed; none is a Broiler.VM package
  or a dependency of one, and the manifest names each.

## 4. What drafting found stale

The candidate's retained metadata showed three family packages describing themselves as they were at
JS-0: the profile as implementing only the slice manifest, the format as defining only format version
1, the lowering as having no tokenizer. Their project descriptions are corrected after the
collection; the bundle keeps the text as packed ([JSC-287](../roadmap.corrections.md#jsc-287)).

## 5. Validation

- **Rules A14 and N37** pass, N37 over five witnesses; the bundle's own predeclared decision holds.

## 6. What is not done

- **Publishing anything**, which is the owner's, after review.
- **A consumer on any RID but `linux-x64`.**
- **The core's own `0.1.0-preview.5` rollback concern** - that two versions were published before any
  review - which stays the owner's answer and is a blocker of [the release gate](../release-gate.md).

## 7. What would falsify this

- A family package the baseline omits, a dependency or file it misstates, or a foreign dependency in
  produced metadata, with rule N37 passing.
- A consumer step the transcript shows passing that a re-run from the same commit fails.
- A run's limit vector that is not the vector its variants were verified under.
