<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle sjl-1-001: Broiler.VM against the shared conformance vectors

**Milestone:** [SJL-1](../../javascript-shared-libraries/milestones/sjl-1-shared-conformance-vectors.md),
its Broiler.VM clauses 4 to 6. **Rules:** [`decision-rule.md`](decision-rule.md), committed at SJL-0,
and [`decision-rule-revised-2026-10-12.md`](decision-rule-revised-2026-10-12.md), written after this
engine's first run was seen. **Collected:** 2026-10-12. **Owner:** the JavaScript profile owner.
**Reviewer:** none; every role is one person (EX-30), and nothing here was read by a second one.

## Decision

| Rule | Decision | Why |
|---|---|---|
| [`decision-rule.md`](decision-rule.md), as committed | **NOT MET** | 16 differences are on `impl-defined` vectors, which its class (d) does not admit ("the vector is classed divergence"), and its negative control asks for a copied archive that its own first clause refuses |
| [`decision-rule-revised-2026-10-12.md`](decision-rule-revised-2026-10-12.md) | **MET**, and **weaker evidence**, because the rule was revised after the run it judges was seen | Every clause below holds |

The revision changes exactly those two clauses and says why; everything else is the original's. The
milestone's own text asks for every difference to carry one of three classes, and an engine that
does not yet meet its own plan's expectation on an `impl-defined` vector is the first of them, "engine
defect, naming the engine". The revised rule's class (d) admits it. The original rule had no place
for that case.

| Clause | What the bundle holds |
|---|---|
| **4. A named corpus revision, pinned by hash, read only by a test-only root** | [`vectors.pin`](vectors.pin), a copy of [`src/tests/vectors/pins/vectors.pin`](../../../src/tests/vectors/pins/README.md): corpus revision 3 at Broiler.JS `593f2c5`, its archive digest and its content digest. The conformance root reads it with `--vectors`; the architecture test `SharedVectorsPinTests` recomputes both digests from the archive ([`n13-and-pin-tests.log`](n13-and-pin-tests.log)); [`refused-archive.log`](refused-archive.log) is the root refusing revision 2's archive under this pin before any vector runs |
| **4. N13 passes with the new directory and fails on the witness** | [`n13-and-pin-tests.log`](n13-and-pin-tests.log): N13 with `tests/vectors` in its suite directories passes on the checkout, and `N13_Rejects_A_Product_Project_That_Carries_The_Shared_Vectors` holds it failing on the witness in which the profile's own project file carries the archive. [`architecture-tests.log`](architecture-tests.log) is the whole suite |
| **5. The run, every difference classified** | [`run-revision-3.tsv`](run-revision-3.tsv) and [`run-revision-3.log`](run-revision-3.log): every vector of revision 3 has a result; every difference is a classified divergence with a `divergence.broiler-vm` field naming its reason and the plan that answers it, listed in [`classification.tsv`](classification.tsv); every `impl-defined` match is listed by identifier and class in the run |
| **5. The validator accepted the revision and refused its negative control** | The home's validator, in Broiler.JS's bundle `docs/evidence/sjl-1-001/tests.log`, on the same revision and digest |
| **6. The negative control is caught** | [`negative-control.log`](negative-control.log) and [`negative-control.tsv`](negative-control.tsv): `--mutate numbers/to-fixed/0001` changed that vector's expected value in the verified in-memory copy, and the run reported it as the one unclassified difference and exited with the failure code. Broiler.JS's runner caught the same mutation in its own bundle |

## The runs

| Run | Corpus | Classified | Unclassified |
|---|---|---|---|
| First, [`run-revision-2.tsv.gz`](run-revision-2.tsv.gz), [`run-revision-2.log`](run-revision-2.log), under [`run-revision-2.pin`](run-revision-2.pin) | Revision 2, Broiler.JS `9b0f2dd` | None | Every difference |
| Judged, [`run-revision-3.tsv`](run-revision-3.tsv) | Revision 3, Broiler.JS `593f2c5` | Every difference | None |

The counts are in each run's header. Revision 3 changed no expected value: it added the classifications
in [`classification.tsv`](classification.tsv), and every rendered outcome of the judged run is the
first run's.

**What the differences are**, all engine defects of this profile:

| Reason | Answered by |
|---|---|
| `Number::toString` at a radix other than 10: more digits than the shortest that round-trip | SJL-3.4 |
| The same: fraction digits that do not denote the value, because the expansion stops at half the gap to the next double above, computed in double arithmetic | SJL-3.4 |
| The same: integer digits that do not denote the value at 2^53 or more, because the integer part is divided by the radix in double arithmetic | SJL-3.4 |
| `Number::toString` at radix 10: 16 digits that do not round-trip where the shortest that do are 17 | SJL-3.4 |
| `parseInt` not correctly rounded beyond 2^53, at radix 10 within 20 digits and at the power-of-two radices | SJL-3.4 |
| `parseInt` at an approximating radix: not the correctly rounded value SJL-3.3 commits this engine to (the 16 `impl-defined` vectors) | SJL-3.4 |
| Hexadecimal, octal and binary integers beyond 2^53, as literals and strings, not rounded to nearest, ties to even | SJL-3.4 |
| `07.5` read as the literal 7.5, in sloppy and strict code | SJL-13.3 |

## What a reader should know

- **A finding for SJL-3, recorded here and in the ledger, not acted on.** SJL-3.3's table names
  "Broiler.VM's algorithm" for `Number::toString` at radix 2 to 36, and says its integer part "is exact
  for values beyond `long`". This run shows that algorithm failing the specification's shortest-digits
  criterion, giving digits that do not denote the value, and giving wrong integer digits at 2^53 and
  above. The algorithm choice is SJL-3.1's area verdict to revisit; this bundle changes no plan.
- **The runner reads the pinned archive and no other copy.** It hashes the archive, extracts it in
  memory and hashes every file before it runs a vector. Every vector gets a runtime of its own, for
  the reason `Execution` gives: a fuel allowance is spent over a runtime's life. A thrown object is
  rendered as an error when `Object.prototype.toString` (read before the vector's script runs) gives
  it the built-in tag `Error`, which is the host's way to ask for an [[ErrorData]] slot.
- **The corpus is Broiler.JS's**, generated by an exact reference computation that is checked against
  CPython; no expected value is engine-observed. The two engines are compared with the vectors, never
  with each other.
- **The release gate gained a blocker.** `THIRD_PARTY_NOTICES.md` lists the vectors among ingested
  material, unconfirmed, and `src/Broiler.VM.Profile.JavaScript/docs/release-gate.md` records the
  blocker that row raises (rule N38). Confirming it is the release owner's statement.

## Files

| File | What it is |
|---|---|
| [`decision-rule.md`](decision-rule.md) | The rule, committed at SJL-0 |
| [`decision-rule-revised-2026-10-12.md`](decision-rule-revised-2026-10-12.md) | Its revision, written after the first run, quoting it whole |
| [`identity.txt`](identity.txt) | The commits, the pins' digests, the configuration, the toolchain and the machine |
| [`vectors.pin`](vectors.pin) | The pin the judged run read |
| [`run-revision-3.tsv`](run-revision-3.tsv), [`run-revision-3.log`](run-revision-3.log) | The judged run: per vector, its identifier, class, expected outcome, rendered outcome and verdict |
| [`run-revision-2.tsv.gz`](run-revision-2.tsv.gz), [`run-revision-2.log`](run-revision-2.log), [`run-revision-2.pin`](run-revision-2.pin) | The first run and the pin it read |
| [`classification.tsv`](classification.tsv) | Every difference of the first run, its class and verdict at revision 3, and its reason |
| [`negative-control.log`](negative-control.log), [`negative-control.tsv`](negative-control.tsv) | The negative control's run, its difference line kept |
| [`refused-archive.log`](refused-archive.log) | The root refusing an archive its pin does not name |
| [`n13-and-pin-tests.log`](n13-and-pin-tests.log), [`architecture-tests.log`](architecture-tests.log) | Rule N13, its witnesses and the pin test; the whole architecture suite |
