# JSD-0020 - The retained conformance-suite pin, and the one it replaces

**Status:** Accepted. The suite is **retrieved, hashed and NOT archived**, so the pin is retained
here and the material is not; the ledger row stays open on the archiving action alone. Nothing here
accepts a manifest, advances a milestone, or sets a floor over any figure.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** this component's architecture owner. **Both
roles are held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a, which [section 14](../roadmap.md#14-the-conformance-oracle) and
[the ledger](../roadmap.status.md#3-open-external-dependencies) both name as the milestone that
pins the suite.

## What was open, and what turned out to be worse than open

The ledger has carried the suite revision as unpinned since JS-0. On 2026-09-03 a checkout of
test262 was retrieved and 53,469 of its files were put through the harness, and the ledger recorded
the result honestly as far as it went: **the pin is over a transient checkout** — retrieved,
hashed, read and left in a temporary directory — so no floor was set over any figure.

**What that description missed is that the pin was self-certifying.** The harness's `--pin` mode
computes a digest over a directory and writes it **into that directory**. Every test262 figure this
component has published was obtained against a `suite.pin` the harness had generated inside the
checkout it was about to score. Verifying against it proves that the directory has not changed
since the harness last looked at it — nothing about which upstream revision the directory is, and
nothing an editor of the checkout could not arrange, because the pin and the suite are editable in
one gesture. "Transient" was true and was not the part that mattered
([JSC-68](../roadmap.corrections.md#jsc-68)).

## 1. The authority moves into this repository

[`src/tests/conformance/pins/test262.pin`](../../../../src/tests/conformance/pins/README.md) is a
pin the suite cannot reach:

| | |
|---|---|
| Suite | test262 |
| Upstream | `tc39/test262` |
| **Revision** | **`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`** (committed 2026-05-08) |
| Archive | codeload `.tar.gz` of that commit, 9,487,173 bytes |
| Archive digest | `f58ce79141529c9fa33592e22ff3ff0d83b83830ac8e372ecd32e1623db1c040` |
| **Content digest** | **`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`** |
| Files | 56,560 |
| Archived | **no** |

`--expect <pin>` makes a run answerable to it: the suite's name, content digest and file count must
be the ones this repository decided, and a disagreement stops the run rather than shrinking a
total.

**The pin is a commit, and the content digest is what checks it.** The commit is what roadmap
section 14 asks for — "an immutable commit… never a branch name" — and the digest is what turns it
from a label into something a run can verify without asking a network.

**The file count is checked beside the digest and is not redundant.** A digest says two things
differ; a count says how. A checkout that gained or lost files is a different accident from one
whose bytes moved, and a run that reported only "the digest is wrong" would leave the reader to
find out which.

## 2. The pin was taken twice, on purpose

The archive was retrieved on 2026-09-03 and again the same day into a second directory. **The two
downloads were byte-identical**, and the second was extracted into a fresh directory and hashed
**independently of the first**.

That is not ceremony. The first checkout is the one this component had been writing a `suite.pin`
into for a day, so a digest computed there is a digest over a directory this component had
modified. The second reading is over content nothing here had touched, and it produced the same
figure — which is what makes `content-sha256` a reading rather than a copy of an earlier reading.

## 3. What is not archived, and why that is a different decision from the specification's

The language specification was archived in this repository on the same day: one file, 2.9 MB, under
a licence that permits redistribution. **The suite is 232 MB over 56,560 files**, and archiving it
is a decision about the shape of this repository rather than about evidence — clone times, checkout
times on every lane job, and a tree in which the third-party material outweighs the component by an
order of magnitude.

So the two are pinned the same way and archived differently, and the difference is recorded rather
than left to be inferred. **What archiving would buy is the same thing it bought for the
specification** — a digest checkable in a checkout with no network — and the row stays open until
somebody decides that is worth 232 MB.

**Nothing of the suite is redistributed here except its licence text**, which is retained beside the
pin so that the obligation is met in advance rather than discovered at a publish.

## What this refuses to do

- **It sets no floor over any test262 figure.** A floor is a ratchet over a suite revision, and
  ratcheting a figure obtained against material this repository does not hold would be a promise
  about a directory somebody else controls. Every figure published from that suite stays a
  measurement and not a commitment.
- **It does not discharge the attribution row.** Roadmap section 14 lands that row "in the change
  that first ingests a suite file", and this change ingests none. What has expired is the *reason*
  [JSC-30](../roadmap.corrections.md#jsc-30) gave for deferring it — an attribution for material
  nobody had read — because the material has now been read.
- **It accepts no manifest and advances no milestone.** The ledger's suite row stays open; what
  changes is that two of its three actions are done and the third is named.
- **It does not put the suite in a lane.** No CI job scores test262, and pointing one at 232 MB of
  third-party material is a decision this record does not take.
