# The conformance floor

**Owner:** verification-boundary owner. **Reviewer:** none.

[`floor.txt`](floor.txt) is the ratchet roadmap
[section 14](../roadmap.md#14-the-conformance-oracle) asks for: the first per-host-mode totals
admitted for a suite revision, which no later run of that revision may fall below.

**It is a measurement discipline and not a status.** *Admitted* is deliberately not the
[ledger](../roadmap.status.md)'s `Accepted`, which additionally needs a reviewer decision nothing
in this component has. A floor that could only be set by an accepted milestone would be a floor
nothing could ever set. Nothing here advances a milestone, and a floor holding is not evidence for
any gate.

**It records the revision it was set under, and is never compared across revisions.** A suite that
added tests would otherwise read as a regression, and one that removed them would silently lower
the bar. A revision change re-bases the floor from the first run admitted on the new revision, and
the old floor stays in the file on a `retired` line with the reason. This is the discipline the
[diagnostic registry](../diagnostics/registry.txt) and the retained corpus already apply to their
own pinned revisions.

**Re-basing is a decision somebody makes.** The component's CI lane compares against this file and
fails a run that falls below it; it never writes it. A run on a revision the floor was not set
under reports that and changes nothing, because a lane that re-based its own floor would be a
ratchet with no teeth.

**What may set one.** A merged run of every shard, carrying no configuration failure and having
executed something. A single shard's run may not, and neither may a run that covered less than it
claimed to cover: the whole value of a floor is that the number behind it was measured over the
selection it names.

## The current floor

Set on 2026-09-03 from the merged three-shard run retained in
[Bundle JS-3A-004](../evidence/js-3a-004/README.md), on `win-x64`. The suite is this component's
own — `broiler.javascript.slice.js-3a` — because roadmap section 14 builds the harness against the
smallest scoring target that exists rather than after the language it will eventually score. **No
third-party conformance suite is pinned**, so no floor over one exists and none is implied by this
file.

## The second floor, and what it guards that the first one does not

[`floor-ingest-shape.txt`](floor-ingest-shape.txt) is the floor over
`broiler.javascript.slice.ingest-shape`, the suite written in the dialect a third-party suite uses.
Set on 2026-09-03 from an unsharded run on `win-x64`, retained in
[Bundle JS-3A-005](../evidence/js-3a-005/README.md).

**It exists because a skip is not a failure.** A case the harness declines to score is reported
`Skipped`, and a run full of skips exits zero. So the adapter could start declining files it used
to run, or the language-class rule could start calling earned refusals unearned, and every lane
would stay green over a shrinking number of passes. A floor names the figure, so the shrink is
what fails.

**It carries no `Raw` row, and the absence is the point rather than an omission.** The ingested
dialect's `raw` flag means *source with no harness prelude and no strictness variant*; this
harness's raw mode means *artifact bytes that no front end lowers*. A case reaching the raw mode
from that suite would be the flag having been carried across two vocabularies that spell it the
same way, which is a defect
[JSD-0016](../decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md)
records and the harness's own checks catch.

**Two floors, one manifest, and that is not a contradiction.** A floor is over a *suite revision*,
never over a manifest or a milestone. These are two suites, they are pinned separately, and neither
figure is comparable with the other's.

**This floor moved on 2026-09-03 and it was a re-base rather than a change in what the engine
does.** The suite gained a `features.txt` — the list an ingested suite carries to say which of its
own feature flags name proposals rather than the language — and three fixtures for the exclusion
that reads it, so its revision moved and the old row is retired in the file with its reason. Script
rose by the one added fixture that is scored; the two that claim a proposal are excluded before
selection and are in no mode's total, which is the whole of what they are there to show.

**A run in the ingested dialect now requires that list, and its absence stops the run.** A harness
that could not tell a proposal from the language was scoring tests about constructs no published
edition contains — and scoring them as passes far more often than as failures, because an engine
with no production for a construct refuses every spelling of it, including the ones a negative test
declares an error. [JSD-0018](../decisions/0018-which-tests-are-about-this-language-and-who-decides.md)
records the decision and [JSC-66](../roadmap.corrections.md#jsc-66) what it cost: 117 passes given
back over a real checkout.
