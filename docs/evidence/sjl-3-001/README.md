<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-3-001 - the number-conversion pilot: Broiler.VM's cut-over and its evidence

**Milestone:** SJL-3 of the shared JavaScript libraries programme,
[`sjl-3-number-conversion-pilot.md`](../../javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md):
its Broiler.VM work packages SJL-3.4 (the cut-over), SJL-3.6 (the evidence) and SJL-3.7 (the
simplification check). **Rule:** [`decision-rule.md`](decision-rule.md), written at SJL-0.9 before any
code of the pilot existed and confirmed unchanged at SJL-3.2 (`rules-in-force.log`). **Records:**
[JSD-0066](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0066-the-profile-on-the-shared-number-conversions.md),
on [JSD-0065](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0065-number-conversion-moves-to-the-shared-algorithms-library.md).
**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this
bundle.

**State:** collection in progress. The cut-over landed first, with the exclusions of section 9; each
run of SJL-3.6 is added to this bundle as it is taken, at the cut-over's commit, and this README says
which have been. Nothing here is a verdict until its section says so.

## 1. Identity

| | |
|---|---|
| Base | Bundle [`sjl-0-001`](../sjl-0-001/README.md)'s base commit, before any engine changed |
| After | The commit that lands SJL-3.4's cut-over; named here once the runs are taken from it |
| Package | `Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`, from the committed feed `eng/packages/`, pinned by `eng/packages/Broiler.ECMAScript.Algorithms.0.1.0-preview.1.pin` |
| Shared vectors | This family's pin, `src/tests/vectors/pins/` (corpus revision 3) |

## 2. Engine-level vectors

Pending in this bundle. At the cut-over, before it was committed, the conformance root ran this
family's pin of the shared vectors against the final package and matched every vector, the 391 that
carried a `divergence.broiler-vm` field among them; JSD-0066 section 4 lists them by reason.

## 3. Test262, both forms (rule part A)

Pending.

## 4. The fuel-parity twins (rule part B)

Pending.

## 5. Closures, the composition register and the packages

Pending.

## 6. The differential cross-check

Pending.

## 7. The measurement sub-bundle (rule part C)

Pending.

## 8. The simplification check (SJL-3.7)

Pending.

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-125 | Open | **The number-conversion code that left this component's assurance coverage.** The 34 members rule I5 registers (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at SJL-3.4. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Numbers`, which the home's own assurance component covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters that call the library stay in this family and stay covered |
| EX-126 | Open | **The Android root's closure is not re-collected after the cut-over.** Collecting it means publishing for `android-x64` and running on an Android emulator, which the machine that took this bundle does not have. The root reaches `Broiler.ECMAScript.Algorithms` through the runtime's package reference, so its register row declares it, and says in its cell that its retained closure, bundle JS-ANDROID-001's, predates it; rule K4 holds that closure to the row without the shared assembly, and refuses it once a re-collected closure carries one |
