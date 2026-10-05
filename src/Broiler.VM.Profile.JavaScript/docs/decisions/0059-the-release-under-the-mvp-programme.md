<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0059 - The release under the MVP programme

**Status:** Proposed, 2026-10-05. **Owner decision pending.** It plans phase F9 - roadmap
[section 26.4](../roadmap.delivery.md#f9--the-release)'s last phase, *JS-10 over every surface above*
- in four slices, records which of JS-10's clauses those slices can build and which stay the owner's
and a named human's, and records slice R1, which is in the tree.

**What the owner decided.** On 2026-10-05 the repository owner asked for phase F9 to be done, after
phase F8's exit gate was met. Nothing here is signed beyond that request.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are held
by one person**, and it does not claim the co-signature is independent.

**Milestone:** JS-10. Phase F9.

---

## 1. What F9 can build, and what it cannot

JS-10's exit gate ([section 19](../roadmap.delivery.md#js-10--baselines-packaging-the-support-table-and-the-release-gate))
and release gates 1 to 13 ([section 22](../roadmap.gates.md#22-release-gates)) ask for two kinds of
thing, and [the MVP programme record](../../../../docs/mvp.md) separates them: **a procedure may be
deferred, a fact may not.**

**What F9 builds and retains** - capability and evidence, each a fact a rule or a bundle can check:

| Slice | What it builds | Release gates |
|---|---|---|
| **R1** | The measurement lane, and the two figures sections 16 and 18 reopen against - verification throughput per byte and cold-start cost - on a JIT and a Native AOT arm, in a baseline register a rule holds to the bundle in both directions | 10, and JS-10's measurement clauses |
| **R2** | The support table, **drafted and not issued**: every cell gate 1 names, the amendment register's state row by row, the extraction-gate state, the declared-default vector, the varying surfaces, every unimplemented capability's failure or exclusion with the `WebAssembly` host-object surface named, the RIDs claimed and unclaimed with reasons, the operational holders, the suppression inventory - and the rules that hold the draft to the checkout | 1, 13 |
| **R3** | Packages and consumers: the packable set frozen in a baseline of its own, a pristine consumer that restores and runs with upstream feeds unreachable, a rollback to the previous package set, and a release-candidate run of the pinned suite retained with its totals per manifest, its failure manifest and its limit vector | 8, 9 |
| **R4** | The release gate: a check over gates 1 to 13 that **refuses**, naming each blocker by its declaration, with a negative control proving it cannot invent a reviewer | 11, and the gate itself |

**What F9 does not do, and may not**: issue the support table, claim a RID, advertise a composition,
publish a package, record a human review decision, name a person to an operational role, or move any
row to `Accepted`. Section 3.5 of the MVP record forbids the first four outright; section 2.2 defers
the fifth; the sixth is the owner's appointment and not a document's; and the last is update rule 2's.
**So F9 ends with JS-10 `In progress`, not `Accepted`**, and with a release gate that refuses, naming
what a human must do before it passes. That is the release this programme can make: everything a
release needs except the ceremony, and a gate that says so.

## 2. Slice R1: the measurement lane

- **The children** are modes of the conformance composition root, `Measurement.cs`:
  `--measure-verify <source>` lowers the source once and measures verifying the artifact against an
  FNV-1a pass over the same bytes; `--cold-start` composes the runtime, lowers `1 + 1`, verifies,
  instantiates and runs it; `--cold-start-control` is the same process doing none of that. Each
  reports the configuration the runtime took: RID, architecture, GC mode, concurrency and tiering
  settings, and whether it is Native AOT.
- **The harness is the core's, restated**: interleaved candidate, control and A/A lanes, seven
  repetitions all retained, the median quoted, no outlier policy, a condition before and after every
  lane. It is restated because the core's lives in a test project, which a composition root may not
  reference and which may not reference a profile (rule A11).
- **The parent**, `eng/measure-js-baselines.py`, writes the immutable manifest before either arm runs,
  publishes the two arms, runs the verify child, times ten launches per cold-start lane, fails an arm
  whose child reports another configuration, and hashes what it retained.
- **The register**, [`docs/baselines.md`](../baselines.md), quotes the four figures, and **rule N33**
  holds it to the bundle's logs in both directions, rule L1's shape over the profile's own register,
  with four witnesses.

## 3. Choices

- **The conformance root is the lane's home** rather than a new composition root: it already composes
  the lowering, the verifier, the executor and the internationalization data, it is the profile's
  harness, and rule N13 already keeps it out of every package and advertised closure. A new root would
  have needed a register row, closure baselines and CI lanes for a measurement no consumer runs.
- **The verify workload** is `src/tests/differential/the-statement-and-object-surface.js`, 64,486
  bytes of source whose artifact is 170,465 bytes: the language's statements and object surface rather
  than a single feature, and retained in the tree with its digest in the manifest.
- **The cold-start workload is one statement**, so the figure is the start a host pays before its
  first answer and not the cost of a program.
- **Default tiering on the JIT arm**: a host starts under it, and cold start is what a host pays.

## 4. Validation (R1)

- **Bundle [JS-10-001](../evidence/js-10-001/README.md)**, from a clean tree at `d9012fe`: verification
  costs **40.2643 ns per byte** under the JIT and **49.1986** under Native AOT; a cold start costs
  **276,954,782.8 ns** under the JIT and **4,485,046.9** under Native AOT. Every figure resolves above
  its A/A lane.
- **Rule N33** and its four witnesses pass; the architecture suite has 346 tests.

## 5. What is not done

- **Slices R2, R3 and R4**, which follow.
- **Every clause section 1 names as a human's or the owner's**: issuing, claiming, advertising,
  publishing, reviewing, appointing, accepting.
- **No host latency budget exists**, so the figures reopen nothing yet.

## 6. What would falsify this

- A figure in the register that the bundle's logs do not carry, or a logged figure the register omits.
- A child that reports one configuration and runs under another.
- A manifest written after an arm ran, or changed while one ran.
- A slice of F9 that issues, claims, advertises, publishes, reviews or accepts anything.
