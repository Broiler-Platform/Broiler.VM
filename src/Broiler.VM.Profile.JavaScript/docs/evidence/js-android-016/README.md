# Bundle JS-ANDROID-016 — six figures, so the five claims that lost their numbers can state them again

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-015](../js-android-015/README.md) closed the counted-existence
class by converting eighteen claims: thirteen into citations, and **five reworded to stop asserting
a count nothing could check**. Its exclusion 4 recorded that as a loss — *"A11 no longer says how
many consumer profiles exist, because nothing computes that; the row is weaker and honest rather
than precise and unverifiable."*

**Six figures were added and all five say their numbers again.** The catalog holds 22.

---

## 1. The five, and what each needed

| Row | Was reworded to drop | Now cites | Value |
|---|---|---|---:|
| A11 | *two consumer profiles* | `{graph:consumer-profiles}` | 4 |
| H1 | *the four review documents* | `{review:documents}` | 34 |
| K3 | *two baselines and two retained catalog tables* | `{composition:registered}`, `{composition:catalog-tables}` | 2, 2 |
| V12 | *all five contracts* | `{contracts:profile-facing}` | 5 |
| K2 | *all three statements of the same fact* | `{composition:fact-sources}` | 3 |

**A consumer profile is defined as what a composition root reaches for that is not the core.** The
three packable assemblies are what *every* root references, so subtracting them leaves exactly the
profiles being consumed — a definition chosen over name-matching `.Profile.` because the fixture
profiles are deliberately not named that, which is the whole reason rule A11's subject is what a
root *consumes* rather than what a project is *called*. At VM-3 the answer was two; it is four now.

## 2. Two of the six are not counts of the tree, and the rows citing them say so

`{composition:fact-sources}` is the **arity of rule K2** — how many independent statements of one
fact it holds to each other — and `{contracts:profile-facing}` is the **size of a declared contract
set**. Both are fixed by a decision rather than by what the checkout grew into.

Citing them is still better than retyping them: the number lives in one array that the rule itself
uses, so a decision that changes updates the array and every row citing it follows. **But it is not
the same claim as a cited tree count**, and calling it one would be the sort of blurring this
chain has spent four bundles undoing.

## 3. A second stale claim fell out of H1's row

H1 named *"the two evidence-bundle READMEs"*. There are far more than two. **The count clause never
saw it**, because the figure was attached to a noun rather than to the word *exist* — the adjacency
limit JS-ANDROID-015 recorded as exclusion 2, found doing its predicted damage one row later. The
row says *"the evidence-bundle READMEs"* now.

## 4. Results

Sixteen reports over a clean checkout; **76 of the register's 77 rules reported and J12 silent**.
Suite: 163 architecture and 207 contract tests, green.

**Three controls**, in `negative-controls.log`, each against a claim that had no figure before:

| Control | Suite | What J12 said |
|---|---|---|
| `J12-a-review-document-count-is-typed-back-in` | failed | `H1 states that four of something exist without citing a figure: "four review documents that exist"` |
| `J12-a-consumer-profile-count-is-typed-back-in` | failed | the same for A11's `"Two consumer profiles exist"` |
| `J12-a-contract-count-cites-a-metric-that-is-gone` | failed | `V12 cites the figure {contracts:profile-facings}, and the figure catalog defines no such metric` |

The first two are **the sentences those rows actually carried** before JS-ANDROID-015 deleted the
numbers from them.

## 5. Exclusions — what this bundle does not show

1. **EX-102 is not further narrowed.** This adds reach to the existing clause; it does not change
   what the clause covers. A figure worded outside the counted-existence shape is still unchecked,
   and the general problem is still not solvable by a rule.
2. **The adjacency limit stands, and section 3 is an instance of it.** A figure attached to a noun
   rather than to *exist* is invisible, and one such claim was found only by reading the sentence.
   There may be others; nothing here swept for them.

   > **Correction, 2026-09-02.** There were others, and [Bundle JS-ANDROID-017](../js-android-017/README.md)
   > sweeps for them: **the entire assurance figure family was stale**, from one event - the
   > JavaScript profile coming under coverage grew the covered set from 45 files to 61, and
   > eight rows went on stating every figure derived from it. A fifth clause now reads a figure
   > standing before a countable subject. The sweep covered the REGISTER only, and two claims in
   > rule H2's row are left standing and named there.
3. **Two of the six figures are declared, not derived**, per section 2.
4. **`{review:documents}` counts the corpus, which grows with every bundle.** H1's row therefore
   states a number that changes whenever evidence is added — correct by construction and not
   stable, which is the trade a citation makes.
5. **The consumer-profile definition is a judgment.** It counts `Broiler.VM.Profile.JavaScript`
   and its `.Compiler` separately, because a composition root references each. A reader who meant
   "profile families" would want a different figure, and the catalog does not offer one.
6. **The count is internal, not the corpus.** `ReviewDocument` is a private nested type and
   widening it would export the review model to publish one number.
7. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
