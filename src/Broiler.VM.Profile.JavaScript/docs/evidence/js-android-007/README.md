# Bundle JS-ANDROID-007 — K2's last three clauses

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The clause [JS-ANDROID-006](../js-android-006/README.md) carried: *"Three
of K2's clauses still have no control — the catalog's package identities against the row's, a
profile named twice, and a consumer profile claiming the reserved first label."* All three are
controlled now, and **every clause K2 has is reached by a control whose subject it is.**

---

## 1. The three, and the one prefix that separates two of them

| Control | Clause it targets | Injection |
|---|---|---|
| `K2-the-catalog-reports-a-package-identity-the-register-does-not-declare` | the catalog's package identities against the row's profile assemblies | the profile line reports a **different `Broiler.*` package** |
| `K2-the-catalog-composes-the-same-profile-twice` | a profile composed twice | the profile line is duplicated |
| `K2-a-profile-claims-the-reserved-label-under-a-foreign-package` | the reserved first label's **pairing** | the profile line reports a **non-`Broiler.*` package** |

**The first and the third differ by one prefix on one word, and that is the whole of what the
reserved-label clause adds.** The label is reserved *for* Broiler rather than forbidden, so what
the core refuses is the combination: with `Broiler.` the identity clause fires alone, and without
it the reserved-label clause fires as well. Two injections a character apart isolate a clause that
neither could isolate on its own.

The duplicate clause is the record-level half of a refusal the core also makes at catalog
construction. What it catches is a bundle **documenting** a composition the runtime would have
refused — which no runtime check can, because the runtime was never asked.

## 2. Results

`composition-controls.log`, unedited. **Three run, three passed.**

The eight rows JS-ANDROID-006 retained are not re-run here: that log already says what they do,
and eight suite runs to reprint it would be cost without a reader.

## 3. Exclusions — what this bundle does not show

1. **All three fire K2 and K3, and none could be otherwise.** Every clause here lives in the
   catalog table, and the catalog table is half of what K3 compares. Co-firing is a fact about the
   rule set rather than a defect in the injection.
2. **THE CLAUSE ATTRIBUTION IS AN ARGUMENT, NOT AN OBSERVATION.** The suite reports that K2 went
   red and no more — xunit prints an empty-collection assertion without printing the collection —
   so which clause each injection reaches is argued from the injection's shape read against the
   rule's source, and stated in the row. Nothing in the log confirms it. A harness that reported
   the rule's own messages would; this one does not have one.
3. **One direction of K1**, as JS-ANDROID-004 through -006 all record.

   > **Correction, 2026-09-02.** Both directions are controlled by
   > [Bundle JS-ANDROID-008](../js-android-008/README.md), which closes this exclusion here and in
   > JS-ANDROID-004, -005 and -006, where it says the same thing. **The reason it stood so long
   > was a defect and not an inconvenience**: a phantom row made K2 and K3 throw and K4 blame the
   > wrong file, and those three now leave a row with no subject to K1. **Nothing in the log below
   > is edited.**
4. **This is a SCOPED run**, three rows of twenty-eight, not the control matrix.
5. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
