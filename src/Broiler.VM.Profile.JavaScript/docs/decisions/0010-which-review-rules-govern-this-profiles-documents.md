# JSD-0010 - Which review-document rules govern this profile's documents

**Status:** Accepted for JS-3a.

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Co-signer:** the component's review-record owner. **Both
roles are held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a.

## The gap this closes

[JSD-0006](0006-assurance-evidence-and-rules-adoption.md) records that this profile adopts the
host component's assurance system, rule register, API baseline and evidence contract rather than
standing up its own. That decision was about the **assurance** system - the annotations, the
fingerprints, the generated report - and it worked.

It said nothing about the **review-document** rules, group H, and those turned out not to reach
this profile at all. Group H builds its corpus from the component's own `docs/`: `HUMAN_REVIEW.md`,
`docs/review/*.md`, `docs/evidence/*/README.md` and `docs/roadmap.status.md`. This profile's
review documents are its status ledger and its evidence bundles, and they live under
`src/Broiler.VM.Profile.JavaScript/docs/`. So the clauses that exist *because a reviewer reads
these documents* - no citation of a source line number, a closed mark vocabulary, every cited
exclusion defined - governed the ledger a profile reviewer never opens and not the one they do.

**This was a gap and not a genre difference**, which is what the investigation had to establish
before anything was changed. Three of the five group H rules pass over this profile's documents
unchanged once the corpus reaches them. The two that do not are recorded below with their reasons.

## Decision: the corpus reaches every profile family

The review-document corpus now includes, for **every** `Broiler.VM.Profile.*` project that has a
`docs` directory, that family's `roadmap.status.md` and every `evidence/*/README.md`. The families
are discovered rather than listed, so a second profile is covered on the day its docs directory
exists; two are covered today.

## Decision: the mark vocabulary is per family, and there are two legends

Widening the corpus alone would have forced one twelve-member vocabulary, and that is the repair
this record exists to refuse.

The two vocabularies are **different kinds of claim about different subjects**. The component's
nine marks are evidence verdicts about a piece of evidence and review verdicts about a gate
clause. A profile ledger's three - `[NONE]`, `[PARTIAL]`, `[FULL]` - are one evidence verdict
about a whole milestone row. Merging them would admit `[FULL]` into `HUMAN_REVIEW.md`, where
nothing defines it, and `[MET]` into a profile ledger whose own section says the vocabulary is
closed and has three members. A reader meeting a mark would have to guess which vocabulary it came
from, and the guess would change what the row means.

So **each document family is held to the legend that governs it**, resolved by where the document
lives rather than by what it contains - deciding from content would let a document choose its own
vocabulary by using it, which is the check inverted. The two sets share no member, and rule H1
carries a witness showing the same document clean under one legend and reported under the other.

**Both legends are published tables, and this profile's ledger gained one.** Section 2 of the
status ledger stated the three marks as a bullet list; it is a `Mark | Meaning` table now, in the
same idiom as `HUMAN_REVIEW.md` section 1, because a legend a rule cannot read is a legend only in
name. The WebAssembly profile's ledger gained the same table: it uses one of the three today,
which is what a ledger with no retained evidence looks like, and publishing all three is what lets
the other two be read when a row first earns one.

### One reading was corrected in the same change

The legend scan took **any** table row in the legend section whose first cell was mark-shaped.
That is true of `HUMAN_REVIEW.md` section 1 by accident - it contains nothing but legend tables -
and false of a profile ledger's section 2, which carries the legend and the milestone table
together. Under the old reading every `[NONE]` in the status table counted as a legend row, so a
ledger with nine of them published `[NONE]` nine times. Only a table under a `Mark | Meaning`
header publishes now, which is the distinction rule H2 already draws for the exclusion table.

## Decision: two clauses do not govern this profile's bundles, and both are stated

**H2's definition half.** Every review document, this profile's included, is held to *citing* only
exclusions that are defined somewhere - a profile bundle citing `EX-42` is checked like any other
document, and that works today. What is **not** extended is the requirement that a bundle carry
exactly one section 9 exclusion table: a profile bundle enumerates its exclusions as a numbered
prose list and mints no `EX-nn` identifier for any of them.

**H5 entirely.** This profile's bundles are not compared against retained figures, because the
figure loader cannot source them: it reads `test.log`, `publish-aot.log` and
`publish-jit-and-trimmed.log` and parses an English `Passed:` line, while a profile bundle is
collected by a different script into `suite.log` and `publish-and-run.log` carrying whatever the
collecting machine's SDK printed - German, on the machine that has collected every profile bundle
so far.

**The second is a stated limit rather than a shrug, because the fallback was not harmless.** A
document with no bundle of its own falls back to the *current* bundle's figures, so a profile
bundle README would have been compared against the component's own logs - a comparison that passes
today only because there is one suite and its totals are the same number in both. That is a
coincidence of the moment, not a property, and a rule that passes for a reason unrelated to what
it checks is worse than one that says it does not reach a document. The exclusion is asserted in
the rule: the excluded set must be non-empty and every member of it must be a profile document, so
it cannot quietly grow to cover a document the rule could have checked.

**Conditions that close them, with owners:**

| Limit | Closes when | Owner |
|---|---|---|
| H2's definition half | A profile bundle mints `EX-nn` identifiers under a section of its own. **Not retrofittable**: the bundles that already exist are retained evidence and immutable, so this binds the first bundle collected after this decision at the earliest | the profile's evidence owner |
| H5 | The figure loader reads a profile family's log names and parses a suite total without depending on the collecting machine's locale | the component's review-record owner |

## Rejected: giving this profile its own group of review rules

It is the shape JSD-0006 already rejected for the assurance system, for the same reason: one
repository policy implemented twice is the drift the platform's own `CODE-ASSURANCE.md` names as
the thing to avoid. What this profile needed was not its own rules but its own **legend**, and a
vocabulary is data the rules read rather than a second implementation of them.

## Rejected: excluding this profile's documents and recording that as the answer

The roadmap asks for exactly this record if the documents turn out to be a different genre. Three
of the five rules pass over them unchanged, so they are not: they are the same genre with a
different vocabulary, and the gap was that nobody had looked.

## What this changes about earlier evidence

**The architecture suite now reads more documents than it did**, so a suite total collected before
this change is a total over a different test corpus. Bundles JS-0-001 and JS-1-001 are unaffected
in what they demonstrate - no rule they cite changed - but their `suite.log` is a run of a suite
that did not read this profile's own ledger. The status ledger records this rather than leaving a
reader to infer it.

**Nothing here is reviewed.** No human has read the widened rules, the two legends or this record.
