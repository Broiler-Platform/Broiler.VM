# Bundle JS-ANDROID-006 — K2's remaining clauses, and a parser that accepted a row nobody wrote

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The clause [JS-ANDROID-005](../js-android-005/README.md) carried: *"K2 has
four clauses and this control fires one... its other three have no control of their own."* Three
more are controlled now. **And writing them found a defect in the register parser**, which is the
part of this bundle worth a reader's time.

---

## 1. The defect, and how it was found

An injection written to test K2's *referenced-but-undeclared* clause emptied the row's
profile-assembly cell. It was expected to fire K1 — the counts would no longer match — and it did
not. It fired K2, K3 and K4, and K1 stayed green.

**The parser drops empty cells before indexing.** So clearing a cell does not produce an empty
value; it shifts every column after it **left**. The row came back declaring its *sibling* as its
profile assembly, its capability column as its siblings, and an evidence path one column over — and
K1 was silent because the two counts moved together. Every rule that did fire, fired for a reason
that had nothing to do with the edit.

That is the worst shape a hand-maintained register can fail in: **a row nobody wrote, parsed
without complaint.** Clearing a cell is exactly what an editor does when a column stops applying.

**The parser now counts each row's non-empty cells against the header's and stops**, naming the
line and both counts.

**The first version of that fix was itself a no-op**, and it is recorded because it is the same
class of error twice: it compared the *split length*, which does not move when a cell is emptied —
the pipes stay where they are. It passed the injection it was written to stop. The check counts
non-empty cells now, and `the-register-row-loses-a-cell` is the control that would have caught
either mistake.

## 2. What is controlled now

| Control | K2 clause it targets | Fired |
|---|---|---|
| `K2-the-register-claims-a-profile-the-android-head-does-not-compose` *(JS-ANDROID-005)* | catalog profile IDs versus the row's | K2 alone |
| `K2-the-android-row-declares-a-profile-assembly-nothing-references` | a declared profile assembly nothing references | K2, K4 |
| `K2-the-android-row-declares-a-different-profile-than-it-references` | a referenced assembly the register declares as neither profile nor sibling | K2, K4 |
| `K2-the-android-catalog-table-names-a-different-composition` | the catalog naming a different composition | K2, K3 |
| `the-register-row-loses-a-cell` | *(not K2)* the parser | the parser stops the read |

**Co-firing is a fact about the rule set, not a defect in the injection.** K4 builds its allowed
set from the row's assembly columns and K3's whole subject is the catalog files, so a clause of K2
that lives in either place cannot be reached alone. **Only the profile-identity clause can**, which
is why JS-ANDROID-005's control is the one that fires K2 by itself.

## 3. Results

`composition-controls.log`, unedited. **Eight run, eight passed** — the five above plus the K1, K3
and K4 controls re-run with them.

## 4. Exclusions — what this bundle does not show

1. **The log's granularity is the rule, not the clause.** Four rows target particular K2 clauses
   and the suite reports only that K2 went red; which clause is argued from the injection's shape
   and stated in the row. xunit prints an empty-collection assertion without printing the
   collection. The one exception is the parser row, which throws, and whose message does reach the
   log.
2. **Three of K2's clauses still have no control**: the catalog's package identities against the
   row's *(it co-fires with the assembly controls and is nobody's subject)*, a profile named twice,
   and a consumer profile claiming the reserved first label. The last two are refusals the core
   makes at catalog construction, so an injection would have to forge a catalog table rather than
   edit a real one.
3. **One direction of K1**, as JS-ANDROID-004 and -005 both record.
4. **This is a SCOPED run**, eight rows of twenty-five, not the control matrix.
5. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
