<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The retained Octane pin

**Owner:** verification-boundary owner. **Reviewer:** none.

The workload roadmap's
[JSW-10](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.workloads.md#jsw-10--the-runs-per-manifest-whole)
asks that the Octane checkout be *retrieved, hashed and archived so a run against it means something
the next reader can check*, the way
[the conformance suite's pin](../../conformance/pins/README.md) already works. Until
[`octane.pin`](octane.pin) existed, the end-user host took a path on a machine and kept no copy: a
benchmark answer had no digest behind it, and the ledger's own sentence — that pointing a tool at a
directory performs none of the retrieve-hash-archive the human action calls for — was exactly right.

## What is here

| | |
|---|---|
| [`octane.pin`](octane.pin) | The revision, the two digests, the file count, and how the archive was taken |
| `octane-<revision>.tar.gz` | The workload, as `git archive` produced it at that commit |
| [`octane-LICENSE.txt`](octane-LICENSE.txt) | The upstream terms, met by the copy |

## Why `git archive` rather than a downloaded tarball

The conformance pin retains the bytes a codeload archive returned, and records the digest of those
bytes. That is the right shape when the retrieval is the thing being pinned. Here the retrieval was
a clone, and a tarball made from a working tree carries mtimes, a filesystem's ordering and a
gzip timestamp — none of which are facts about the commit. `git archive --format=tar` at the named
revision, piped through `gzip -n`, produces bytes that are a function of the commit alone, so a
reader who clones the same commit and runs the same command gets the same digest.

## What a pin does not authorise

**Nothing about performance.** Octane is retired upstream — the pinned commit is the retirement
commit — and a score from it is a number about this configuration rather than a comparison with
anything, including with itself on another day. Throughput, baselines and the measurement lane are
`JS-10`'s, and roadmap [section 17](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
governs any figure that ever gets retained. What the pin makes possible is narrower and is the whole
of what JSW-10 asked for: that *which* Octane a run was against is a fact a reader can check.
