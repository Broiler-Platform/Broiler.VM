# Bundle JS-ANDROID-014 — rule J12, which stops a register row from carrying its own copy of a generated figure

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-013](../js-android-013/README.md) found that rule J10's
register row said the rule was red over a tree it is green on, corrected it, and recorded as
exclusion 5 that **the correction was prose and nothing compares it to the tree on any later day**:
*"Closing it needs a rule that reads the generated figures, and that is a rule nobody has minted."*

This is that rule. **The register now holds 77 rules and 76 of them are reported.**

---

## 1. The rule, and why it is not a comparison

The obvious rule is "compare the row's figure to the report's figure". **J12 does something
stronger: it removes the row's copy of the figure.**

A row that needs a figure **cites** one — `{criteria:required}`, `{criteria:carrying}`,
`{criteria:missing}` — and the rule resolves the citation against the `## Falsification criteria`
table of the generated `CODE-ASSURANCE.md`. There is then nothing to go stale. This is not a new
idea in this component: rule J5's own row forbids a hand-maintained figure such as
`Human-reviewed: 47/47` **outright**, and a number typed into a register row is the same object.

Three clauses:

| Clause | What it holds |
|---|---|
| 1 | every `{criteria:…}` citation resolves to a metric the report defines |
| 2 | no row states a count of units against the criterion requirement of its own |
| 3 | no row claims, in the present tense, that criteria are outstanding while the report says none are — and when some are, J10's row must say so |

Clause 2 is the one with teeth. Clause 1 only keeps the citations honest, and **the row that went
stale carried no citation at all** — a rule checking only citations would have been green over the
defect it was minted for.

## 2. The first version reported sixteen innocent figures

It tested for *a criteria word and a figure in the same sentence*. On a clean register that
reported sixteen things across five rows, every one of them correct prose: rule **J2**'s row counts
the **comment lines** an annotation parses as, rule **J11**'s counts the **five clauses** a publish
needs, rule **J5**'s discusses criteria figures while counting something else.

**A rule that fires on a row for discussing its subject is a rule about English.** The clause is
adjacency now — a figure standing immediately before *units are assessed*, *units carry*, *of them
carry*, or inside one of the report's own table rows — and
`J12_The_Vocabularies_Are_Checked_Against_The_Register_They_Read` asserts that every row
discussing criteria without counting units stays silent, so the sixteen cannot come back.

## 3. Two rows had to be reworded, and the rule was not weakened to spare them

- **J12's own row** quoted the sentence it was minted for — `THIS RULE IS RED AT THIS MILESTONE` —
  and its own clause 3 fired on it. It says the same thing in reported speech now.
- **J10's row** quoted the report's table line, digit and all: `| Required and missing | 0 |`. That
  is a figure that can rot, so it is `| Required and missing | {criteria:missing} |` now.

Rule J9 hit this exact wall — its report's own prose about a criterion stated a term the
annotations define no count for — and reworded rather than weakening the rule. Same choice, and it
is recorded because the alternative is always available and always wrong.

## 4. Results

`clean-J12.txt`: **J12 says nothing about this checkout.** Sixteen reports in all; 76 of the 77
rules are reported and `E5` remains the only one that is not. Suite: 162 architecture and 207
contract tests, green.

**Four controls, in `negative-controls.log`**, each isolating one clause:

| Control | Suite | What J12 said |
|---|---|---|
| **`J12-the-original-defect-restored`** | failed | three messages: `counts Forty-four units…`, `counts three units…`, and `says falsification criteria are outstanding, and the generated report states 'Required and missing \| 0'` |
| `J12-a-row-claims-redness-and-states-no-figure` | failed | clause 3 alone |
| `J12-a-citation-is-replaced-by-a-typed-number` | failed | clause 2 alone, on the **correct** current figure |
| `J12-the-report-renames-a-metric` | failed | clause 1, naming both rows that cite the metric |

**The first control is the one that matters.** It puts the real stale sentence back into J10's row
**in both places the row is stored** — `rules.register.json` and the hardcoded copy in
`AssuranceRegisterRows.cs` — because two agreeing copies is exactly what made the defect
invisible. The register's own row-equality test is green on that injection, as it was for however
long the sentence stood. J12 is not.

**The third control is the subtle one.** It types in `79`, which is **correct today**. J12 reports
it anyway, and that is the rule's whole claim: a right number and a stale number are the same
object seen at different times.

## 5. Exclusions — what this bundle does not show

1. **J12 reads three figures.** A register row can be wrong about anything else — a count of
   artefacts, a number of units, a milestone, a date — with nothing here to stop it. **The general
   problem of prose that stops being true is not solved**, and this bundle does not claim it is.
   That is EX-102, stated in the row itself.

   > **Correction, 2026-09-02.** [Bundle JS-ANDROID-015](../js-android-015/README.md) **narrows**
   > EX-102 rather than closing it, and the distinction is now in the row. One class is closed
   > completely — a claim that some number of things EXIST must cite its figure — and the catalog
   > is widened from these three metrics to sixteen. **The sweep that did it found eight rows
   > already wrong**, including one saying eight edges where the manifest holds fifty-nine, and
   > one sentence carrying three wrong figures at once. Every figure worded outside that shape
   > remains unchecked, and the general problem remains unsolvable by any rule.
2. **The count clause is narrow by construction.** It recognises four ways of writing "this many
   units, against the criterion requirement". A claim worded outside them is not seen, which is the
   shape of EX-71 one document over. Narrowing was the fix for sixteen false reports and it bought
   that legibility with reach.
3. **A figure of one, spelled as the word `one`, is invisible**, because this prose uses "one" as a
   pronoun in nearly every row and a rule firing on those would be unusable.
4. **The outstanding vocabulary is present tense on purpose**, so a row keeps its history. A row
   claiming redness in the past tense is not read as a claim about this tree — deliberate, and it
   means the vocabulary can be evaded by tense.
5. **No control fires J12 alone.** Renaming a metric in the generated report also makes the report
   stale, so J5 moves with it; the register injections also move nothing else, but only because the
   register is not a generated artefact. Section 4's controls are judged by the suite and quoted
   from the report, and the report is what says which rule saw what.
6. **This rule does not check that the report's figures are right.** J8 holds them to the units and
   J5 holds the file to the generator; J12 reads a figure two other rules already keep honest, and
   inherits whatever they miss.
7. **The first run of these controls was partly judged by a stale assembly, and the harness is
   fixed rather than the result quietly kept.** The revert copies a backup over the injected file
   with `shutil.copy2`, which **preserves the original timestamp** — so the restored source looks
   *older* than the assembly compiled from the injected one, MSBuild skips the rebuild, and the
   next run reads a clean tree while executing injected code. The fourth control edits only a
   markdown file, needed no rebuild of its own, and therefore ran against the third control's
   assembly; the verification run afterwards failed for the same reason and the tree was correct
   all along. The revert touches the file now, and every verdict in `negative-controls.log` is from
   a re-run. **This is the same finding as the register parser, the cell-count check and the
   disagreement message**: a step that looks like it is doing something.
8. **A message is what a rule said, not proof that the rule is right.** Nothing here runs on a
   device, no milestone moves, and nothing is reviewed.
