# Evidence bundle JS-3A-001

**Milestone:** JS-3a — the diagnostic registry, the position encoding, the pinned suite, and the
oracle. **This bundle covers the first two. It does not touch the last two, and they are the
larger half of the milestone.**

**Verdict this bundle supports:** JS-3a is **In progress**, not accepted. No suite revision is
pinned and no harness exists; three registry rows are reachable from no artifact and are named as
such; and **no human has reviewed anything**, which the roadmap makes a precondition for
`Accepted` on any milestone.

This bundle was produced by `eng/collect-js-evidence.py`. Every file beside this one is its
output. **A command written in a plan is not evidence that the command ran**; the logs are what
ran.

**No result from any other component is evidence here.** No figure, total, conformance result,
benchmark or Native AOT sample from the Broiler.VM core, from the legacy JavaScript engine, or
from any other component appears in this bundle or is cited by it.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-3A-001` |
| Milestone | JS-3a, **registry half only** |
| Registry revision | 1 |
| Core contract version | 1 (implemented; **not accepted**) |
| Format version | 1, accepted range 1–1 |
| Feature manifest set | `broiler.javascript.slice`, and nothing else |
| Claimed RID | `win-x64`, and nothing else |
| Owner | conformance owner |
| Co-signer | verification-boundary owner, for the registry split |
| Reviewer | **none** |

Owner and co-signer are the same person, and the roadmap requires the non-independence to be
recorded rather than resolved by assertion. **No decision in this bundle was reviewed by anyone
who did not make it.**

## What this milestone claims, in one paragraph

The diagnostic-code registry is published at `docs/diagnostics/registry.txt` at revision 1, one
row per code, stating the member that declares it, **the one core reason every emission carries**,
the stage that refuses, **which half of the registry it belongs to**, the case that reaches it,
and the revision its meaning dates from. Five registered rules bind it to four independently
written artefacts and to the position factories, so no single edit makes it agree with everything.
The retained corpus grew from 51 to 59 entries to close the backward binding. The position
encoding is decided, implemented, and pinned by four corpus rows through a new manifest column.
**Nothing here is a conformance claim**: no suite is pinned, no harness exists, and a published
registry is a record of what this verifier does rather than of what any specification requires.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 132 architecture tests passed, 0 failed |
| Assurance gate mode | `assurance-gate.log` | Passed: every generated artefact is byte-identical to what the generator would write |
| Assurance release mode | `assurance-release.log` | **Refused, as it must.** Every relevant unit is `HUMAN_PENDING`, and each blocking declaration is named individually |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0.** Catalog tables byte-identical across modes for both roots |
| Corpus replay, in every published mode | `publish-and-run.log` | **59 entries replayed to their recorded answers**, twice, with both passes agreeing row for row |
| Closure reports | `closure-executiononly.txt`, `closure-slicecompiler.txt` | 6 and 7 managed assemblies under JIT and trimmed; 0 under Native AOT |
| Negative controls, suite-judged | `negative-controls.log` | **15 injected, 15 failed while injected and passed after revert, 0 skipped** |
| Negative controls, corpus-judged | `corpus-controls.log` | **7 injected, 7 failed while injected and passed after revert, 0 skipped** |

A **skipped** control is now a collection failure rather than a line in a log. A control whose
anchor has moved was never injected, so the row above it would be a name with nothing behind it;
the collector counts skips, names them as the gap they are, and exits non-zero after writing
everything. This bundle records zero.

## The registry, and what binds it

`docs/diagnostics/registry.txt`, revision 1, **40 rows over a 40-member vocabulary**. Each row
carries the code, the declaring member, its half, its one core reason, the emitting stage, its
reachability, its case, and the revision its meaning dates from.

| Rule | Binds the registry to | The control that shows it bites |
|---|---|---|
| **N5** | The `JavaScriptDiagnosticCode` declaration | `N5-the-registry-omits-a-declared-code` — one row deleted while the code stays declared |
| **N6** | Every emission site in the profile assembly | `N6-the-registry-names-a-reason-the-sites-do-not-carry` — one reason swapped for another real core reason |
| **N7** | The retained corpus manifest | `N7-the-registry-names-a-case-the-corpus-does-not-have` — one case renamed to an entry nobody wrote |
| **N8** | The composition's deliberately restated constants | `N8-a-restated-code-drifts-from-the-registry` — one constant renumbered |
| **N9** | The position factories | `N9-a-position-is-built-outside-the-factory` — the verifier answering with its own convention again |

**No one artefact is the authority.** The producer restates its expected codes rather than reading
them from the profile it tests, and that duplication is only worth its cost while a third artefact
holds both halves; the registry is that third artefact, and N8 is what holds them to it.

### Three rows are reachable from no artifact, and they are named

Thirty-seven of the forty rows are produced by a named corpus entry. Three are not:

| Code | Why no artifact reaches it |
|---|---|
| `1003 DescriptorFormatVersionMismatch` | The core screens the descriptor's format version against the registered range **before calling the profile**, and this build registers exactly one version |
| `1006 DescriptorManifestMismatch` | The same for the manifest: this build accepts exactly one |
| `1903 ReaderStopped` | Every bounded-read status this build was compiled against has an arm of its own, so this arm answers a status the reader does not currently have |

**The list of three lives in rule N7 rather than in the registry.** A row claiming to be
unreachable is a row excused from the backward binding; if the registry alone decided which rows
may claim it, the excuse would be available by editing the file the rule reads. A fourth is an
edit to a test, which is a review. **This is a stated limit on the gate clause, not a pass.**

## The position encoding, and what it corrected

Decision [JSD-0009](../../decisions/0009-the-diagnostic-registry-and-the-position-encoding.md)
states this profile's use of all four fields of the core's position record: the section index is
the framed section's **ordinal**, `-1` meaning an offset into the artifact rather than into a
section body; the byte offset is relative to whichever of those the section index names; and the
two profile-owned coordinates carry a one-based line and column from the canonical position table,
with **zero in both meaning the position is not known**, reserved by a verifier that now refuses a
table row declaring it.

**Landing it corrected a real conflation.** Every diagnostic the link and walk stages produce
carries an offset into the *code section*, and every one of them went through a helper that set
the section index to `-1` — which under this encoding says the number is an offset into the
artifact. The number was right and the frame it named was wrong, so a consumer resolving it would
have landed on an unrelated byte. This is the failure roadmap section 7 predicted between two
profiles, found inside one.

Four corpus rows pin the encoding through a new manifest column, and the two corpus controls show
each failing for its own reason:

| Injection | What stopped agreeing |
|---|---|
| The code-section position reported under the artifact-relative marker again | The three code-relative rows. The read-stage row, which is genuinely `-1`, correctly did not move |
| The covering-row scan stopping at the first row | **Only** `a-refusal-covered-by-the-second-position-row`, which is the entry written to discriminate exactly that |

`EntryStackNotEmpty` was declared at JS-1 and emitted by nothing. Its case was answered as a join
mismatch, reported by whichever of two arrivals the worklist popped second — a diagnostic that
depended on a traversal order no artifact can see. It is refused on the **edge** now, and the
third corpus control shows the difference: with the edge check removed, the entry that expects
`SemanticValidationFailed/1505` observes `InconsistentStructure/1404`.

## The corpus

59 retained entries at `src/tests/corpus/js-1/`, up from 51. **Nine were added to close the
backward binding** — codes the vocabulary declared and nothing reached:

| Entry | Code it closes |
|---|---|
| `a-manifest-identity-longer-than-the-format-admits` | `1005 ManifestIdTooLong` |
| `an-overlong-variable-length-integer` | `1902 MalformedEncoding` — the reader's status, as opposed to the header's own version check |
| `a-declared-maximum-above-the-formats-own-ceiling` | `1201 DeclaredMaximumTooLarge` |
| `more-constants-than-the-limits-section-admits` | `1303 ConstantCountExceedsDeclaredMaximum` |
| `a-code-section-of-no-length` | `1410 EmptyCode` |
| `an-entry-point-name-of-no-length` | `1504 MalformedEntryName` |
| `an-entry-point-reached-with-operands-on-the-stack` | `1505 EntryStackNotEmpty` |
| `a-position-row-inside-an-operand` (pinned position added) | the covering-row half of `1506` |
| `a-refusal-covered-by-the-second-position-row` | the position encoding itself |

16 control entries still verify successfully, the corpus still replays twice with no residue, and
the verifier throws on none of it.

## Exclusions — what this bundle does not show

1. **The oracle half of JS-3a is not started, and it is the larger half.** No suite revision is
   pinned, no harness, self-check, sharding, merge, audit or scope tooling exists, no
   per-host-mode totals are published, and no ratchet is set. The suite-revision dependency in the
   ledger's section 3 is still open: retrieving, hashing and archiving third-party material is a
   human action and nobody has performed it. **That, and not the registry work, is what keeps this
   milestone `In progress`.**
2. **Three registry rows are reachable from no artifact**, named above and in JSD-0009. The gate
   clause reads "every code in it is reachable from a named case"; thirty-seven are.
3. **No `embedder-seam` code exists.** The registry's two halves are decided and only one is
   populated, because the front end that would mint a source rejection is JS-3b's. That half of
   the split is declared and not exercised.
4. **Four corpus rows pin a position and fifty-five pin none.** Writing a position on every row
   would mean hand-computing offsets into bytes the producer assembles, which no reader could
   check; having the producer ask the verifier for them would be recording the answer under test.
   The four pin the encoding, and each fails differently if it moves.

   > **Correction, 2026-09-02.** Both reasons are true and neither is exhaustive.
   > [Bundle JS-3A-003](../js-3a-003/README.md) takes a third way - the producer derives the
   > position from its own construction and the replay compares it against what the verifier
   > derived from what it read - and the three pins that name a section are derived now, with the
   > hand-computed strings kept as the answer the derivation must reproduce. **It pinned no new
   > row**, because which byte a refusal reports is not written down anywhere; that is a decision
   > record and is named as the next work. The corpus regenerated byte-identical. The count above
   > is also stale: the corpus holds 66 entries now, so it is four and sixty-two.
5. **The language-specification edition is still unpinned.** JS-3a was asked to record the pin
   actually taken and no pin has been taken, for the same reason as the suite revision.
6. **The public API baseline still does not cover the profile's assemblies.** Carried from JS-1
   with its two named routes, and untouched here.
7. **One RID, one machine, one operating system.** `win-x64`, because it was published and run.
8. **Nothing is fuzzed.** The corpus is a fixed set of hand-written entries and finds only what it
   was written to find. JS-9 owns the untrusted-input surfaces.
9. **The rules are over syntax, not semantics.** N5 through N9 parse the profile's sources with
   Roslyn because rule A11 keeps the profile assembly out of the architecture test project's
   reference set. A code whose only emission site the reader cannot see would fail N6's
   every-declared-code-is-emitted clause rather than pass quietly, but the reader is a parser and
   not a compiler.
10. **Two review-document clauses do not reach this bundle, and both are stated.** The
    review-document rules built their corpus from the component's own `docs/` and this profile's
    documents live elsewhere, so none of them governed this file. JS-3a closed that and
    [JSD-0010](../../decisions/0010-which-review-rules-govern-this-profiles-documents.md) records
    it: the corpus now reaches every profile family, and the mark vocabulary is per family with
    two legends that share no member. **Two clauses still do not.** A profile bundle is not held
    to carrying one section 9 exclusion table, because it enumerates its exclusions as this
    numbered list and mints no `EX-nn` identifier — and retrofitting one into the bundles that
    already exist is not available, because a retained bundle is immutable. And the
    quoted-figure rule does not read this file at all, because it cannot source the logs it would
    compare against: it reads `test.log` and an English `Passed:` line, and this bundle's is
    `suite.log` in the collecting machine's own locale. Each has a named condition and an owner in
    JSD-0010.
11. **The suite this bundle ran is not the suite the earlier bundles ran.** The architecture suite
    reads more documents than it did, so the total in `suite.log` is over a wider corpus than
    JS-0-001's or JS-1-001's. Nothing either of them demonstrates is affected — no rule either
    cites changed — but the totals are not comparable across the change, and the ledger says so
    too.
12. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
