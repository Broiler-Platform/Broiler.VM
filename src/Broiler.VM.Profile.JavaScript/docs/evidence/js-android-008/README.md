# Bundle JS-ANDROID-008 — K1's other direction, and three tests that assumed a row has a subject

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The clause every bundle from
[JS-ANDROID-004](../js-android-004/README.md) onward carried: *"One direction of K1 is exercised,
not both. The opposite drift — a row naming a composition the checkout does not have — would leave
K3 failing on a missing baseline file rather than on the rule, so it stays asserted in-test only."*

Both directions are controlled now, **and the reason the second one could not be controlled before
was a defect rather than an inconvenience.**

---

## 1. What a phantom row used to do

A row naming `Broiler.VM.Composition.Deleted`, which no project builds, produced this:

| Rule | What happened |
|---|---|
| K1 | reported it correctly — the rule whose subject it is |
| K2 | **threw** `InvalidOperationException: Sequence contains no matching element`, from `Single` over a project that is not there |
| K3 | **threw** `FileNotFoundException`, looking for `catalogs/deleted.catalog.txt` |
| K4 | reported *"the bundle retained no closure report"* — **a true sentence blaming the wrong file** |

So three tests crashed or misattributed around the one rule with something accurate to say, and
the earlier bundles read that noise as a reason not to write the control. It was a reason to fix
the tests.

## 2. What changed

K2, K3 and K4 now iterate the rows that name a composition root the checkout has. **This is not a
weakening**: a row with no subject is still reported, by K1, which is the rule whose subject it is.
The three rules below it ask questions about a row's artefacts — its project's reference set, its
catalog baseline, its retained closure — and a row naming a composition that does not exist has
none of those to be wrong about. They presuppose a subject now; before, they assumed one.

## 3. Results

`composition-controls.log`, unedited. **Two run, two passed** — K1's two directions:

| Control | Direction | Fired | Threw |
|---|---|---|---|
| `K1-the-register-loses-the-android-row` | a root the register does not name | K1 alone | nothing |
| `K1-the-register-names-a-composition-the-checkout-does-not-have` | a row naming a composition that does not exist | K1 alone | nothing |

**The second is the worse drift of the two**, which is why leaving it uncontrolled mattered: an
undocumented root is a gap, while a row for a composition nobody can run reads as a support claim
for something that does not exist.

## 4. Exclusions — what this bundle does not show

1. **The other seven composition controls are not re-run here.** JS-ANDROID-006 and -007 retain
   them; two suite runs to reprint eleven rows would be cost without a reader.
2. **The clause attribution limit stands**, as JS-ANDROID-007 states it: the suite reports that a
   rule went red and no more.

   > **Correction, 2026-09-02.** Retired by
   > [Bundle JS-ANDROID-009](../js-android-009/README.md), whose reporter writes the rules' own
   > messages. **Nothing in the log below is edited.**
3. **This is a SCOPED run**, two rows of twenty-nine, not the control matrix.
4. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
