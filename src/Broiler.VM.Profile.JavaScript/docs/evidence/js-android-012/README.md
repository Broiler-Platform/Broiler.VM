# Bundle JS-ANDROID-012 — five rules restated as message lists, and one that is not a rule this suite asserts

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-011](../js-android-011/README.md) reported 68 of the
register's 76 rules and named eight it could not reach. Six of those eight were said to need their
rule **rewritten** as a message list — "writing a new rule rather than reporting the one that
exists… not a thing a reporting mechanism may do quietly". This bundle is the loud version: the
rewrite was asked for, five rules are message lists now, and **the sixth turned out not to belong
on that list at all**.

**73 of 76 rules are reported.**

---

## 1. The six were not six

| Rule | What JS-ANDROID-011 said | What is true |
|---|---|---|
| `C1` | asserts equalities | correct — **restated**, and the restatement needed a clause to stay as strong (§2) |
| `C3` | an absence "with no message list behind it" | **wrong on its own terms** — the list of languages the search *found* was already computed and thrown at `Assert.Empty`. The messages existed and were never phrased as any |
| `E1` | asserts equalities | correct — **restated** |
| `M1` | asserts equalities | **wrong.** `Violations(...)` has returned `exported but not declared:` / `declared but not exported:` messages since M1 was written. Nothing was rewritten; a report entry was added |
| `N10` | asserts equalities | **wrong**, same way. Its two non-vacuity guards are new messages; its comparison was always a list |
| `E5` | "produces no collection at all" | **the wrong objection entirely.** See §3 |

**Two of the six were mis-sorted by a survey that read shapes without opening bodies.** The survey
that produced the exclusion counted `Assert.Empty`/`Assert.Equal` occurrences per rule and sorted
on the answer; M1 and N10 assert `violations.Count == 0` against a message list, which the counter
scored as an equality. The classification was mechanical and the bodies were never read. Recorded
because it is the same failure this component has now hit three times: **a check that looks like it
is checking.**

## 2. The C1 clause that keeps the restatement honest

C1 compared two ordered sequences with `Assert.Equal`. The obvious rewrite — report the names the
pack produced that are not declared, and the declared names it did not produce — is **weaker than
what it replaces**: two sequences compared elementwise disagree when one holds a duplicate, and a
membership check over the same two does not.

So C1 also reports when the number of matched lines is not three. Control
`C1-a-fourth-nupkg-duplicates-a-declared-one` exists to prove that clause is load-bearing rather
than decorative: it adds a second `Broiler.VM.Runtime.0.1.0-preview.1.nupkg`, which is **no stray
name, no missing name, and the written count still 3**, and C1 says:

```
the pack log names 4 nupkg files for 3 declared packages
```

That is the one injection a set-difference rewrite would have passed. A restatement that dropped it
would have left the rule green on a defect the old assertion caught — the exact quiet weakening the
report exists to make impossible.

## 3. E5 is not a rule this suite asserts

The recorded objection — "produces no collection at all" — reads as a rule written awkwardly. It is
not one:

- E5's register status is **Deferred**, superseded at VM-1 by V1 and V2;
- its `activationMilestone` is **`never`**, and its `permanenceReason` says there is no milestone
  that could activate it;
- **no test asserts it**, because `RuleRegisterTests.Deferred_Rules_Are_Not_Asserted_And_Name_A_Later_Milestone`
  requires that none does.

Reporting what E5 said about this checkout would mean **writing the rule the register says is not
asserted**. That is a stronger objection than the one on record, and a different kind: the other
five exclusions were about how a rule is written, and this one is about whether the rule exists.
The register's row stays Deferred and unreported.

**J10 and J11 are unchanged and still unreported**, for the reason JS-ANDROID-011 gave: their clean
direction is asserted over a **witness input** rather than over this checkout, so there is no "what
this rule said here" to write down. Giving them one would need an input the test does not use.

> **Correction, 2026-09-02.** That reason is wrong, and this bundle repeated it without checking
> it. Both tests assert over a witness in the middle and over **the checkout** last — J10 over
> `ProductUnits`, J11 over `AssuranceGenerator.Current`. Both are reported by
> [Bundle JS-ANDROID-013](../js-android-013/README.md), which takes the count to **75 of 76** and
> leaves `E5` as the only rule the mechanism does not reach. That bundle also found that **J11 is
> not silent on a clean checkout** — it reports 905 blockers, every relevant unit being
> `HUMAN_PENDING` — so this bundle's §4 claim that "every one of the 73 rules said nothing" is
> true of the 73 it covered and is **not** a property of the report in general.

## 4. Results

Fifteen reports over a clean checkout, unedited: `clean-A.txt`, `clean-A7.txt`, `clean-A14.txt`,
`clean-B.txt`, `clean-C.txt`, `clean-D.txt`, `clean-E.txt`, `clean-H.txt`, `clean-J.txt`,
`clean-K.txt`, `clean-L.txt`, `clean-M1.txt`, `clean-N.txt`, `clean-N10.txt`, `clean-V.txt`.
**Every one of the 73 rules said nothing.** Suite: 154 architecture and 207 contract tests, green.

**Seven negative controls, one per restated rule and three for C1**, each judged by the suite's exit
code *and* quoted from the rule's own report. Each injects into a real file and is reverted by
copying back a backup taken before the edit — never `git checkout --`, which has twice discarded
uncommitted work in this component. `negative-controls.log` retains them.

| Control | Suite | What the rule itself said |
|---|---|---|
| `C1-a-produced-package-is-named-wrong` | failed | `the pack produced a nupkg for Broiler.VM.Bnary, which is not one of the three declared packages` + `the pack produced no nupkg for Broiler.VM.Binary` |
| `C1-a-fourth-nupkg-duplicates-a-declared-one` | failed | `the pack log names 4 nupkg files for 3 declared packages` |
| `C1-the-written-count-disagrees-with-the-names` | failed | `the pack log counts 4 nupkg, and 3 packages are declared` |
| `C3-a-package-description-names-a-language` | failed | names **javascript** *and* **java** (§5.6) |
| `E1-the-record-declares-a-version-the-build-does-not-implement` | failed | `0003-core-contract-v1-and-amendments.md declares Core contract version 2, and the build implements 1` |
| `M1-the-baseline-omits-an-exported-member` | failed | `exported but not declared: … VmCoreContract.MinimumSupportedVersion : const System.Int32 = 1` |
| `N10-the-baseline-omits-an-exported-member` | failed | `exported but not declared: … JavaScriptFormat.CeilingConstants : const System.UInt32 = 65536` |

Every one names **its own** rule and no other.

## 5. Exclusions — what this bundle does not show

1. **Three rules are still unreported**: `E5` for the reason in §3, `J10` and `J11` for the reason
   in §2 of JS-ANDROID-011. The count is 73 of 76 and will not reach 76 without deciding to change
   what those rules claim.
2. **Five rules are stated differently than they were**, and that is a change to the tests rather
   than only to the reporter. Each restatement is a **move** — the test asserts `Assert.Empty` over
   the same function the report calls, so there is one implementation — but a reader comparing this
   suite to JS-ANDROID-011's would find C1, C3 and E1 written differently. The controls above are
   the evidence that the restatements did not weaken them; they are not proof that no clause was
   lost, because a clause nobody thought to control cannot be missed by a control.
3. **E1 is slightly wider than it was.** The deleted `HeaderInteger` helper asserted the header
   field existed before comparing it, so a record that had dropped the field failed differently
   from one that had it wrong. Both now read as one rule. This is a deliberate widening, not an
   accident of the rewrite.
4. **M1 and N10 gained non-vacuity messages they did not have as messages.** Their tests still
   assert emptiness separately; the report needs the messages because a rule that described no
   assembly would compare nothing, find no disagreement, and report the silence of a satisfied
   rule.
5. **M1 and N10 are silent under `BROILER_API_WRITE=1`**, as rules J3, J5 and J7 are under the
   assurance write switch. A run regenerating a baseline has not compared anything against it.
6. **C3 matches substrings, and the report now makes that visible.** Its control injected
   `JavaScript` and C3 named both `javascript` and `java`, because `java` is a substring of
   `javascript`. That is how the rule has always been written and no behaviour changed here; it is
   recorded because the report is the first thing that ever showed it, and a reader of a future
   control log will otherwise read two findings where one defect exists.
7. **A message is what a rule said, not proof that the rule is right.** The report quotes; it does
   not check.
8. **The clean reports are this checkout on this machine.** They are a floor, not a measurement.
9. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
