# Bundle JS-ANDROID-011 — the rule-message report over 68 of 76 rules, and the eight it cannot reach

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-010](../js-android-010/README.md) reported 46 rules and
excluded thirty on the ground that they "assert inline: the rule IS the test body... there is
nothing for a reporter to call." **Twenty-two of those thirty are now reported**, and the eight
that are not are named with a reason that is about the rule rather than about effort.

---

## 1. What the thirty actually were

The exclusion was right that they are not functions and wrong that they are all the same problem.
Three kinds:

| Kind | Count | What was done |
|---|---|---|
| **The clean direction is already a named helper** returning a collection — `CoverageViolations(...)`, `StatusViolations(...)`, `AssuranceRelease.Blockers(...)` | 15 | **Nothing extracted.** The report calls the same helper the test calls, with the same inputs |
| **The clean direction is a collection built inline** in the test body | 7 | **Extracted to a function, and the test now calls it** — a move, not a copy |
| **The clean direction is not a collection at all** | 8 | Not reported; see below |

**Extraction was a move in every case.** Where a collection came out of a test body, the assertion
was rewritten to call the extracted function rather than to keep its own copy. Two implementations
of one rule is exactly the drift a report exists to prevent, and an extraction that left one behind
would have built the defect into the fix.

## 2. The eight that are not reported, and why

| Rule | Why not |
|---|---|
| `C1`, `E1`, `M1`, `N10` | assert **equalities** between a value and a constant, not a list of violations |
| `C3` | asserts an **absence** — that no language name appears in package text — with no message list behind it |
| `E5` | produces no collection at all |
| `J10`, `J11` | their clean direction is asserted over a **witness input**, not over this checkout: J10 over units read from a witness file, J11 over a release plan built from witness text |

The first six would need their rule **rewritten** as a message list, which is writing a new rule
rather than reporting the one that exists. The last two would need an input the test does not use,
which is a different claim wearing this one's clothes. **Neither is a thing a reporting mechanism
may do quietly**, so both are named here instead.

> **Correction, 2026-09-02.** The rewrite was asked for and done loudly, in
> [Bundle JS-ANDROID-012](../js-android-012/README.md), which takes the count to **73 of 76** and
> corrects this table in three places.
>
> - **`M1` and `N10` do not assert equalities and never did.** Both compare a surface against a
>   baseline through a `Violations(...)` helper that has returned `exported but not declared:` /
>   `declared but not exported:` messages since each rule was written; the test asserts
>   `violations.Count == 0` over that list. They needed a report entry, not a rewrite. The survey
>   behind this table sorted rules by counting assertion forms and never opened their bodies.
> - **`C3`'s messages existed too.** Its clean direction already computed the list of languages
>   found in package text and passed it to `Assert.Empty`; the list was simply never phrased as
>   messages. An absence is reportable exactly when something can be named as present.
> - **`E5`'s reason was wrong in kind.** It is **Deferred**, superseded at VM-1, its activation
>   milestone is `never`, and `RuleRegisterTests` requires that no test assert it. Reporting it
>   would mean writing the rule the register says is not asserted — an objection about whether the
>   rule exists rather than about how it is written. It stays unreported for that reason instead.
>
> `C1` and `E1` were correctly described and are genuinely restated as message lists. **Nothing in
> the reports below is edited.**
>
> **Second correction, 2026-09-02.** The `J10` / `J11` row above is wrong too, and
> [Bundle JS-ANDROID-013](../js-android-013/README.md) takes the count to **75 of 76**. Both tests
> assert over a witness **in the middle** and over the checkout **last** — J10 over
> `ProductUnits`, J11 over `AssuranceGenerator.Current` — so this exclusion was written from the
> witness clause without reading to the end of either test. That is the third exclusion in this
> chain to describe a rule's shape rather than its body. **`E5` is now the only unreported rule**,
> and only because it is a Deferred row no test may assert.

## 3. Results

Thirteen reports over a clean checkout, unedited: `clean-A.txt`, `clean-A7.txt`, `clean-A14.txt`,
`clean-B.txt`, `clean-C.txt`, `clean-D.txt`, `clean-E.txt`, `clean-H.txt`, `clean-J.txt`,
`clean-K.txt`, `clean-L.txt`, `clean-N.txt`, `clean-V.txt`. **Every one of the 68 rules said
nothing**, which is the floor the report needs before any finding it makes means anything.

One injection was run against an **extracted** rule, to check that extraction did not sever the
report from what the rule does: an edge added to `graph.manifest.json` produced
`manifest has 1 more: Broiler.VM.Abstractions -> Broiler.VM.Runtime` from A7, and two tests went
red. The extraction moved the rule; it did not weaken it.

## 4. Exclusions

1. **Eight rules unreported**, for the reasons in section 2.
2. **A7 and A14 report to files of their own** rather than into group A's, because neither is a
   per-project sweep and folding them in would put three different shapes of answer under one
   heading.
3. **A message is what a rule said, not proof that the rule is right.**
4. **The clean reports are this checkout on this machine**, and D1's answer depends on whether an
   aggregate checkout exists above the component - it reports the inconclusive branch as a message
   rather than as silence, because silence would be indistinguishable from a scan that found
   nothing.
5. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
