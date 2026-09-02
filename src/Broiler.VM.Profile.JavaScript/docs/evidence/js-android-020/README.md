# Bundle JS-ANDROID-020 — the recogniser reconsidered, so a rule can quote the defect it was minted for

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-019](../js-android-019/README.md) named a pattern and said what
to do about it: *"A rule that forbids a sentence shape cannot use that shape to explain why it
exists… The repair has been reported speech every time. If a fifth appears, the thing to reconsider
is the recogniser, not the prose."* The recogniser is reconsidered here, at four rather than five.

---

## 1. What the recogniser could not tell apart

| | |
|---|---|
| **an assertion** | `45 covered source files are compared whole` — a claim about the tree, which can rot |
| **a quotation of one** | seven rows said `45 covered source files` — history, which cannot |

Rule J12 reads shapes, and these two have the same shape. Four times a register row explaining why
a rule exists tripped a rule by showing the sentence that was wrong: **J12's row three times,
A15's once.** The repair was always the same — paraphrase the quotation until its shape was gone —
and it cost the row the evidence it was trying to present.

**Four repairs is a design, not a coincidence.** The register's rows are where this component
explains why its rules exist; a rule forbidding a sentence shape has to let a row *show* that shape.

## 2. The exemption, and why it needs two halves

A figure is exempt when it is **inside a code span** AND **attributed by a reporting verb within 72
characters before it**. `The row said `45 covered source files`` is a quotation; `The comparison
covers `45 covered source files`` is not.

**This is the one place in the rule where a miss is unsafe.** Everywhere else, a vocabulary that
fails to recognise something produces a false report — the safe direction. Here, a verb wrongly
matched means a live figure goes unread. So the verb list is deliberately short and **`read`,
`carries` and `names` are absent**: all three are ordinary words in this register, and each would
hand out the exemption on a sentence that is asserting.

**It is an escape hatch and can be abused.** A row could quote a live claim and evade the clause.
What the design buys is that abuse must be **deliberate** — backticks and an attributing verb, both
— where the paraphrase route was available by accident to anyone who worded a sentence differently.
It also buys legibility: a reader sees backticks and a verb and knows the row is reporting, where a
paraphrase looks like ordinary prose.

## 3. Five sentences got their evidence back

J12's row quotes `THIS RULE IS RED AT THIS MILESTONE, WHICH IS THE CLAUSE WORKING` again, and
`Forty-four units are assessed High or Critical`, and `45 covered source files`. A15's row quotes
`19 projects and 55 edges`. Each had been flattened into paraphrase to get past the rule it was
describing.

**One of them still had to be reworded, and that is the design working.** A sentence reading *"went
on to say"* was not exempt, because the verb list holds `said`, `says` and `saying` and not the
bare `say`. The fix was to use a listed verb rather than to widen a list that grants exemptions.

## 4. Results — and the only result that matters

`controls-re-run.log` retains **all eighteen controls minted for J12 and A15, re-run**. The
exemption can only **weaken** these rules: it removes text from what the clauses read. So the
question is not whether the new witness passes but whether **any control that used to fire has
stopped**.

**All eighteen still fire.**

Seventeen reports over a clean checkout; **77 of the register's 78 rules reported**, J12 and A15
silent. Suite: 172 architecture and 207 contract tests, green.

The new witness carries one attributed quotation and **three near-misses** — a code span with no
verb, a verb with no code span, and a verb too far from its span. All three are reported; the
quotation is not. Each near-miss carries a **different figure**, because the clause reports a row
and a claim, so three identical claims would collapse into one message and the test would pass
while two of the three had stopped working.

## 5. Exclusions — what this bundle does not show

1. **The exemption is an escape hatch**, per section 2. Nothing here detects a row quoting a live
   claim to evade the clause, and no rule could without reading what the quotation is *about*.

   > **Correction, 2026-09-02.** [Bundle JS-ANDROID-021](../js-android-021/README.md) measures
   > this rather than leaving it asserted: an injection presents a live figure as a quotation and
   > **the suite stays green on a stale number**. It is labelled a limit demonstration, not a
   > control. The hatch is still not closed, but it is no longer silent - **the report lists what
   > the exemption let through**, on its own line, so a reviewer has a list rather than an absence.
2. **The verb list is a literal vocabulary**, which this component distrusts for good reason — rule
   J9's recogniser was defeated four times by rewording. The mitigation is that this one gates an
   exemption rather than a report, so an unrecognised verb costs a false report and not a missed
   one. That is a different trade, not an escaped one.
3. **72 characters is a number chosen by looking at the sentences**, not derived from anything. A
   quotation attributed from further away is not exempt.
4. **Only rule J12 has the exemption.** Rule A15 reads one sentence shape in one document and has
   never had to quote itself; if it does, it will need this too.
5. **The pattern is addressed, not proven absent.** Four occurrences motivated this; a fifth in a
   rule that does not use code spans would look exactly like the first four.
6. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
