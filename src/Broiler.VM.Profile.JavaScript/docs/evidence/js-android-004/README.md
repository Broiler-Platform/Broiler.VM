# Bundle JS-ANDROID-004 — a control for K1, and the three composition controls together

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The clause [JS-ANDROID-003](../js-android-003/README.md) named as owed:
*"K1 and K2 have no control of their own... K1 — which holds the register and the checkout to the
same composition set — has none at all. It is what would notice the head being deleted from the
register, or a fourth root arriving unregistered."* K1 has one now, and this run exercises all
three composition controls together rather than the new one alone.

---

## 1. Why K1 needed one, when it already rejects three ways

K1's own test asserts three rejecting directions: an undocumented root, a phantom row, and a kind
outside the schema. **All three are built in memory inside the test.** So what was shown is that
the function rejects; what was not shown is that **the rule reads the real register and the real
checkout**.

That is the same gap JS-ANDROID-003 closed for K3 and K4, and it is worth closing separately for
K1 because K1 is the rule that decides whether the other three have anything to say. A row deleted
from `docs/compositions.md` is not a row the others complain about — **they iterate the rows, so a
deleted row is a row they no longer check.** If K1 did not reach the real file, a composition could
leave the register and every rule in group K would stay green.

## 2. The control

`K1-the-register-loses-the-android-row` deletes the Android head's row from `docs/compositions.md`
while the root stays in the checkout. That is the shape a real drift takes: a root lands, or is
renamed, and the register is not updated.

**It fails K1 and nothing else**, which the log records by test name and which is the load-bearing
half of this bundle: it is simultaneously the demonstration that K1 works and the demonstration
that no other rule would have caught it.

## 3. Results

`composition-controls.log`, unedited. **Three run, three passed:**

| Control | Failed |
|---|---|
| `K1-the-register-loses-the-android-row` | `K1_The_Register_And_The_Checkout_Name_The_Same_Compositions` — alone |
| `K3-the-android-catalog-baseline-drifts-from-what-the-head-printed` | `K3_…` and `K2_…` |
| `K4-the-android-closure-ships-an-assembly-the-register-does-not-declare` | `K4_…` |

The K3 and K4 rows are re-runs of what JS-ANDROID-003 retained, included because a scoped run of
one control tells a reader less than a scoped run of the set it belongs to.

## 4. Exclusions — what this bundle does not show

1. **K2 still has no control of its own.** It fires here as a side effect of the K3 injection, as
   it did in JS-ANDROID-003. An injection whose *subject* is K2 — a register row whose declared
   profile assemblies disagree with the reference set — is not written.

   > **Correction, 2026-09-02.** Written, and by a different clause than this exclusion guessed:
   > [Bundle JS-ANDROID-005](../js-android-005/README.md) has the row claim a profile its own
   > catalog table does not report, which fires **K2 alone** — K1 counts identities against
   > assemblies and both counts hold, K3 reads two files neither of which moves, and K4 reads
   > assemblies rather than identities. Group K now has a control per rule over this row.
   > **Nothing in the log below is edited.**
2. **This is a SCOPED run, not the control matrix.** Three rows of twenty. The full matrix is what
   `eng/collect-js-evidence.py` collects, and no run of it is retained here.
3. **One direction of K1 is exercised, not both.** The control removes a row for a root that
   exists. The opposite drift — a row naming a composition the checkout does not have — is
   asserted only in the test's own in-memory list, because injecting it would leave K3 looking for
   a catalog baseline that does not exist and failing on a missing file rather than on the rule.
4. **Nothing here runs on a device**, and no milestone moves. Nothing here is reviewed.
