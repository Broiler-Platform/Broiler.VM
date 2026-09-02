# Bundle JS-ANDROID-005 — a control whose subject is K2, and group K complete over this row

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The last clause [JS-ANDROID-003](../js-android-003/README.md) and
[JS-ANDROID-004](../js-android-004/README.md) both carried: *"K2 still has no control of its own.
It fires here as a side effect of the K3 injection... An injection whose subject is K2 is not
written."* It is written now, and **every rule in group K has a control whose subject it is** over
the Android head's row.

---

## 1. What "a control whose subject it is" means, and why the side effect was not enough

K2 already went red in this component's logs — twice, under the K3 injection, because a catalog
that gains a profile line disagrees with the register as well as with its baseline. **A rule that
fails alongside another rule has not been shown to reach anything on its own.** What a control for
K2 has to do is fire K2 and leave K1, K3 and K4 silent, because only then is the red attributable
to the clause the row names.

## 2. The control, and why it is K2's alone

The Android head's row claims it composes `com.example.absent` instead of the profile its own
catalog table reports.

**K2 is the only rule that compares those two things**, and the other three are silent by
construction rather than by luck:

| Rule | Why it stays silent |
|---|---|
| K1 | counts profile IDs against profile assemblies; both counts stay one, and the name and kind are untouched |
| K3 | compares the checked-in baseline with the retained catalog table; neither file moves |
| K4 | reads assembly names, not profile identities |

The drift it imitates is a register edited to say what someone believed a composition composes,
rather than what its catalog table says it does.

## 3. Results

`composition-controls.log`, unedited. **Four run, four passed**, and the failing test names are
what carry the claim:

| Control | Failed |
|---|---|
| `K1-the-register-loses-the-android-row` | `K1_…` — alone |
| `K2-the-register-claims-a-profile-the-android-head-does-not-compose` | `K2_…` — **alone** |
| `K3-the-android-catalog-baseline-drifts-from-what-the-head-printed` | `K3_…` and `K2_…` |
| `K4-the-android-closure-ships-an-assembly-the-register-does-not-declare` | `K4_…` — alone |

Three of the four fire exactly one rule. K3's fires two, which its own row states as deliberate: a
catalog naming a profile the register does not name is genuinely two rules' business, and an
injection tuned to trip exactly one of them would flatter a rule rather than resemble a defect.

## 4. What group K now has, stated once

Over the Android head's row: **K1, K2, K3 and K4 each have a control that fires it**, three of them
firing it alone. Before JS-ANDROID-003 none of the four had been shown to read this row at all —
every rejecting direction in their own tests is asserted over lists built in memory.

## 5. Exclusions — what this bundle does not show

1. **One direction of K1 is exercised, not both.** The control removes a row for a root that
   exists; the opposite drift — a row naming a composition the checkout does not have — would leave
   K3 failing on a missing baseline file rather than on the rule, so it stays asserted in-test
   only. JS-ANDROID-004 records the same limit.
2. **K2 has four clauses and this control fires one.** The catalog-versus-register profile
   identities. Its other three — an undeclared referenced assembly, a declared profile assembly
   nothing references, and a catalog naming a different composition — have no control of their own.
3. **This is a SCOPED run, not the control matrix.** Four rows of twenty-one. The full matrix is
   what `eng/collect-js-evidence.py` collects, and no run of it is retained here.
4. **Group K is not the whole register's business.** These four rules are about one row's
   agreement with its artefacts; nothing here controls the composition register's prose, the labels
   ADR 0003 fixes, or what any other row says.
5. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
