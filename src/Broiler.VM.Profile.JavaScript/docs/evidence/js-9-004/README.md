# Bundle JS-9-004 — the first retained sessions that are guided, and how little the guidance yields

**Collected:** 2026-09-02. **Milestone:** JS-9. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** JS-9's row carried one clause that needed neither a human nor an unblocked
milestone: *"**No retained session is guided**: the four in Bundle JS-9-001 were collected before
any of this and their logs are unedited, so closing the clause needs a collection that has not
happened."* This is that collection.

---

## 1. The collection script was describing sessions that no longer existed

The harness writes a header into every retained `fuzz.log`. It said:

> NOT coverage-guided. The mutator draws every mutant from the fixed seed corpus and takes no
> feedback from what a mutant reached… **no session in this log closes it.**

The sessions have taken feedback since 2026-09-01. `Fuzzing.cs` builds a `SeedPool`, keeps a mutant
whose published answer no seed produces, and reports what it kept. **A collection run before this
change would have retained a log denying what its own sessions printed four lines below.**

**This header has now been wrong in both directions**, and the corrected text says so: it once
claimed coverage-guided when the mutator took no feedback at all — `JSC-38` corrected that — and it
then claimed no feedback after the sessions began taking it. The precise claim is *feedback by
published answer, and no edge coverage anywhere*.

## 2. What the four sessions did

| Seed | Iterations | Exit | Answers from seeds | Answers at end | Kept | Pool |
|---|---|---|---|---|---|---|
| 1 | 25,000 | 0 | 44 | 46 | 2 | 66 → 68 |
| 2 | 25,000 | 0 | 44 | 45 | 1 | 66 → 67 |
| 3 | 25,000 | 0 | 44 | 46 | 2 | 66 → 68 |
| 4 | 25,000 | 0 | 44 | 45 | 1 | 66 → 67 |

No counterexample in 100,000 iterations. Each session verified mutants that were instantiated and
invoked, so both existing surfaces were reached.

## 3. The yield is one or two answers, and that is the honest headline

**A reader told "the sessions are guided now" would reasonably expect more than this.** Twenty-five
thousand iterations move the pool by one or two artifacts. The seeds already reach 44 published
answers and a session ends on 45 or 46.

That is not a defect in the loop; it is what the signal is. **The guidance is by published answer**,
so a mutant is only interesting when it makes the profile say something no seed made it say — and
this profile's vocabulary of answers over a slice-scope corpus is small. JSD-0013 states the same
limit from the other side: two paths to one answer are one signal, and a defect on a path that
answers like its neighbour is invisible.

**The clause the roadmap asked to close is closed. The clause it stands next to is not**, and the
gap between "guided" and "guided usefully" is measured here rather than left for a reader to
assume.

## 4. Results

`fuzz.log` retains all four sessions unedited. Alongside: `build.log` (exit 0), `suite.log` (172
architecture and 207 contract tests, exit 0), `assurance-gate.log` (exit 0) and
`assurance-release.log` (**exit 1, which is correct** — every relevant unit is `HUMAN_PENDING` and
a release gate that passed here would be the defect), plus identity, environment and hashes.

## 5. Exclusions — what this bundle does not show

1. **This collection is scoped to the guidance clause.** It was run with `--skip-publish` and
   `--skip-controls`, so no composition was published here and no control matrix was run. The
   twenty-nine controls are retained in earlier bundles and none was re-judged.
2. **Two of the four surfaces are still not fuzzed**, because the source tokenizer and parser and
   the regular-expression matcher do not exist. Nothing here may be read as covering them.
3. **The yield is one or two answers per session**, per section 3. Guidance being present is not
   guidance being effective, and this bundle claims only the former.
4. **The corpus is still slice-scope, not full-format.** There is no object model to fuzz.
5. **Four seeds, one machine, one RID.** The seeds and iteration counts are stated so a session is
   reproducible, not because any of them is a number something justifies.
6. **No finding was produced**, so the regression discipline — a counterexample becomes a corpus
   entry with a recorded answer and the defect is fixed — is declared here and not exercised.
7. **Nothing is reviewed**, and no milestone moves on this bundle alone: JS-9's other open clauses
   are untouched.
