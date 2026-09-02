# Bundle JS-ANDROID-003 — controls for the Android head's catalog table and closure report

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The second half of the gap
[JS-ANDROID-001](../js-android-001/README.md) opened.
[JS-ANDROID-002](../js-android-002/README.md) controlled what the head *does* — the corpus replay
and the extraction it rests on. Nothing controlled what the head *declares*: the catalog table and
the closure report it prints, which rules K3 and K4 compare against a checked-in baseline and the
composition register. Two controls now do.

---

## 1. What these are about, and it is not what K3 and K4 do

**Both rules already had a rejecting direction, and neither of them reached this row.** K3 and K4
doctor a register row in memory inside their own tests and assert the violation — over `Rows[0]`
and over the calculator, both desktop compositions. So what was shown is that the rules reject
*something*. What was not shown is that they read **the Android row's** baseline and **the Android
row's** retained artefacts.

That distinction is the whole reason these controls exist. A composition can be registered, given
an evidence column, and compared by nothing — a row that looks governed and is not. The Android
head was registered on 2026-09-02 and until this ran, that is exactly what it might have been.

## 2. The two controls

| Control | Injects into | Failed |
|---|---|---|
| `K3-the-android-catalog-baseline-drifts-from-what-the-head-printed` | the checked-in baseline `catalogs/android.catalog.txt`, which gains a profile line the head never printed | `K3_Each_Catalog_Baseline_Matches_What_The_Published_Composition_Printed` **and** `K2_Each_Row_Agrees_With_The_Reference_Set_And_The_Catalog` |
| `K4-the-android-closure-ships-an-assembly-the-register-does-not-declare` | the **retained** `closure-android.txt` in JS-ANDROID-001, whose format sibling becomes the other profile family's assembly | `K4_Each_Published_Closure_Contains_Exactly_What_It_Declares` |

**The first trips two rules and that is deliberate.** A catalog naming a profile the register does
not name is two rules' business, and an injection chosen to trip exactly one of them would be one
that flatters a rule rather than one a defect would look like.

**The second injects into retained bytes rather than into source**, which is the direction the
corpus-integrity control already takes and the one that would otherwise be taken on trust: a
closure report is a file, and a file nobody checks is a file that can say anything. It also picks
the worst plausible drift — the assembly of the *other profile family*, which rule N2 exists to
keep out of a JavaScript image.

## 3. Results

`composition-controls.log`, unedited. **Two run, two passed.** Each injected suite exits 1 and each
reverted suite exits 0, and the log names the failing tests, which is what says the failure is the
one the row is about rather than an unrelated red.

## 4. Exclusions — what this bundle does not show

1. **This is a SCOPED run, not the control matrix.** It exercises the two new rows and not the
   other seventeen in `CONTROLS`. The full matrix is what `eng/collect-js-evidence.py` collects,
   and no run of it is retained here.
2. **K1 and K2 have no control of their own.** K2 fires here as a side effect of the first
   injection rather than as the subject of a control, and K1 — which holds the register and the
   checkout to the same composition set — has none at all. It is what would notice the head being
   deleted from the register, or a fourth root arriving unregistered.

   > **Correction, 2026-09-02.** K1's half of this exclusion is closed by
   > [Bundle JS-ANDROID-004](../js-android-004/README.md), which deletes the Android row from the
   > register and shows K1 failing **alone** — the other three rules iterate the rows, so a
   > deleted row is one they no longer check. K2's half stands: it still fires only as a side
   > effect. **Nothing in the log below is edited.**
3. **Nothing here runs on a device.** These are suite controls over files. What the head does on
   Android is JS-ANDROID-001's and JS-ANDROID-002's subject.
4. **Nothing here is reviewed**, and no milestone moves.
