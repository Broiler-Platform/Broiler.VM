# JSD-0018 - Which tests are about this language, and who gets to decide

**Status:** Accepted for JS-3a's ingestion path. **The mechanism is built and measured over a
transient checkout**; no third-party suite is pinned, retrieved or held, and nothing here changes
that.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** the profile runtime owner. **Both roles are
held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a owns the harness. The clause this touches is JS-3b's, because what a
conformance figure over the parse-and-early-error slice *means* is exactly what this decides.

## What was open

[JSD-0016](0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md) decided that
a refusal answers a question about the language **only when it was a language answer**, and built
[`LanguageErrors`](../../../compositions/Broiler.VM.Composition.JavaScript.Conformance/LanguageErrors.cs)
to say which refusals qualify. That rule asks one question and it is the right one: *did this front
end refuse for a reason the language recognises?*

**It cannot ask the other one.** A run over a real checkout on 2026-09-03 scored 121 cases about
`using` declarations. Four failed and **117 passed**, every one of the 117 a refusal this front end
makes because it has no production for the construct **in any spelling**, agreeing with a test that
declared a `SyntaxError` for one particular malformed spelling. The refusals were `ExpectedToken`,
which is a genuine early-error answer — 672 of that run's 1,201 passes rest on that code — so
JSD-0016's rule passed them, correctly, on its own terms.

The ledger had already noticed the four failures and named the cause wrongly: it said the harness
"can filter by the suite's own feature metadata, and this run did not pass a filter". The harness
could **read** the metadata and had no way to **act** on it — `--features` is an inclusion filter,
and a test claiming no feature at all matches no inclusion set, so any value of it would have
deleted the 665 scored cases that claim nothing. That correction is
[JSC-66](../roadmap.corrections.md#jsc-66).

Three questions had no answer anywhere:

- **Who decides whether a construct is in the language?** [Section 3](../roadmap.status.md#3-open-external-dependencies)
  records the language edition as an **unpinned** external dependency. This component has no
  edition to check a construct against.
- **What does a run do when it cannot tell?** Nothing said, and the answer it was giving was
  "score it anyway".
- **Where does such an exclusion go in the pipeline, and is it one figure or two?**

## 1. The suite decides, because it is the only party here that has

An ingested suite ships a `features.txt` at its root that splits its own flags into a **proposed**
section and a **standard** one, with a third for host capabilities. Its own prose says the proposed
flags are there "so that consumers may more easily omit them as necessary". Omitting them is
precisely what this needs, and the list travels with the suite, is covered by the suite's pin, and
was written by people who are not this component.

**A hand-written list in this repository was rejected.** It would have been a list this component
chose, and a list this component chose is one it can quietly grow whenever something fails — the
exact failure mode
[`known-incorrect.txt`](../../../../src/tests/conformance/ingest-shape/known-incorrect.txt)'s
reason requirement exists to slow down, and this filter is more dangerous than that one because a
single entry can remove thousands of cases. Reading the suite's own answer removes the discretion
rather than disciplining it.

**Reclassifying `ExpectedToken` was rejected, and it is worth saying why it was never available.**
It is what this front end reports for a real syntax error, and it is what the majority of the run's
honest passes rest on. Refusing to score it would have deleted those too, and no signal
distinguishes "no production for this construct because it is not in the manifest" from "no
production for this construct because the source is malformed" — without an edition, which is the
thing that is missing.

## 2. Reading the list is required, not offered

A run in the ingested dialect reads `features.txt` from the suite root. **If the file is absent or
does not parse, the run stops and scores nothing**, in the same voice as a run pointed at a suite
with no pin: `MissingSuiteRevision` is a failure of the run and never a smaller total, and this is
the same shape of statement.

**A command-line switch was rejected.** A switch can be forgotten, and forgetting it over-scores:
the run would be quietly back to the state this record exists to end, reporting a number 117 too
high with nothing on the transcript to say so. What a run may not do is decide whether to ask the
question.

**Only the proposed section is excluded.** The test-harness section names capabilities reached
through the suite's own `$262` object rather than constructs of the language; reaching one needs a
call, which `broiler.javascript.slice` does not admit, so such a test is counted **unselectable**
one stage later — where what the count says is that this profile cannot present the test, rather
than that the construct is not in the language. That those two never collide is measured over the
pinned checkout rather than assumed: no scored case claims a test-harness feature.

## 3. Two figures, not one, and the exclusion is asked first

The exclusion is its own stage of the recorded pipeline, counted as `featureExcluded`, sitting
between scope filtering and the run's own `--features` filter.

**Two figures, because they are two questions.** One says the suite calls this construct a
proposal, which is true of every run. The other says this run was not interested, which is true of
this run. Added into one number they cancel: an inclusion set that widens by a hundred and an
exclusion that grows by a hundred leave the figure **and** the selected count unmoved while a
different hundred tests ran — which is the cancellation
[section 14](../roadmap.md#14-the-conformance-oracle) asks for stage-by-stage counts to prevent.
The report format goes to version 2 to carry the column.

**Asked first, because answerability is prior to interest.** In the other order `--features` is a
way to opt back into exactly the tests the suite says nobody should be scoring; and a test the
scope filter would also have removed would be attributed to the scope, reading as a case some
other run could have scored.

## What this refuses to do

- **It does not pin a language edition, and must not be read as one.** What it reads is one suite's
  opinion of its own flags at one revision. A construct that suite has not flagged is still scored
  on whatever this front end happens to do with it. [Section 3](../roadmap.status.md#3-open-external-dependencies)
  stays open; this narrows what it costs.
- **It embeds no suite content.** No feature name, path, expectation or revision from a third-party
  suite is written into this repository's code. The ingest-shape fixture suite carries a
  `features.txt` of its own, in the same **format**, exercising the two shapes that make a careless
  reader wrong — and a format is not a suite.
- **It does not decide anything about the `raw` flag, the strictness readings or the language-class
  rule.** Those are JSD-0016's and are unchanged.
- **It grades nothing.** No milestone advances, no floor is set over any third-party figure, and
  the run this was measured on is over a transient checkout that no human has archived.

## What it is for, stated plainly

A conformance figure is worth exactly what its errors cost to make. Before this, the run's
error was in the direction that never complains: it reported 1,201 passes where 1,084 were about
this language, and the 117 difference was an engine agreeing with tests by refusing everything.
**After it, the totals fell and nothing was repaired to make them fall** — which is the only
direction a correction to a self-scored number can honestly go.
