# Evidence bundle JS-9-003

**Milestone:** JS-9. **What this bundle adds:** the corpus-integrity check — *a mutated corpus
entry proves the replay detects a changed observed triple* — which is **the last clause of JS-9's
exit gate that needs nothing this profile has not built**.

**It is also the consolidated one.** JS-9-001 and JS-9-002 are not superseded and are not edited;
each is the collection that first showed what it shows. This bundle re-runs all three of JS-9's
landed exercises together, so a reader who wants one collection covering the milestone's current
state reads this and a reader who wants the history reads all three.

**Verdict this bundle supports:** JS-9 is **In progress**, not accepted. Most of the milestone is
untouched and waits on work that is itself blocked, and **nothing here has been reviewed**.

Produced by `eng/collect-js-evidence.py`. Every file beside this one is its output.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-9-003` |
| Milestone | JS-9 |
| Adds | The corpus-integrity check |
| Also carries | The fuzz sessions of JS-9-001 and the host-lifetime exercises of JS-9-002, re-run |
| Corpus | 60 entries |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

## The corpus-integrity check

**Every other control in this component injects into source. This one injects into the retained
bytes.** That is the other direction, and it is the one that would otherwise be taken on trust: a
corpus is only evidence while the thing that reads it would notice if the bytes moved.

Two entries, because the replay compares a different field for each — a control entry's completion
**value** and a malformed entry's diagnostic **code** — and mutating only one would leave the other
half of the comparison unexercised. Each is mutated by one byte, replayed, restored byte for byte,
and replayed again; a restore that does not reproduce the original stops the run rather than
leaving the corpus modified.

| Entry | Expected | Observed under one flipped byte |
|---|---|---|
| `addition` | `Normal`/`NormalCompleted`/`0`/`42`/`-` | `InvalidArtifact`/`Truncated`/`1901`/`-`/`-` (hash MISMATCH) |
| `an-unknown-opcode` | `InvalidArtifact`/`UnknownFeature`/`1401`/`-`/`2:0:0:0` | `InvalidArtifact`/`Truncated`/`1901`/`-`/`-1:61:0:0` (hash MISMATCH) |

**The second row moves all five compared fields, the position among them** — which is what
JS-3a added the position column for. Both were detected: 2 mutated, 2 detected.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 138 architecture tests passed |
| Assurance gate mode | `assurance-gate.log` | Passed |
| Assurance release mode | `assurance-release.log` | **Refused, as it must** |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0**, 18 checks per run |
| Fuzz sessions | `fuzz.log` | 4 sessions, 100,000 iterations, 0 findings |
| **Corpus integrity** | `corpus-integrity.log` | **2 entries mutated, 2 detected** |
| Negative controls, suite-judged | `negative-controls.log` | 16 injected, 16 caught, 0 skipped |
| Negative controls, corpus-judged | `corpus-controls.log` | 9 injected, 9 caught, 0 skipped |
| Negative controls, fuzz-judged | `fuzz-controls.log` | 1 injected, 1 caught, 0 skipped |

Twenty-six negative controls across three judges, plus the two corpus mutations, none skipped.

## What JS-9 still does not have, and why none of it is schedulable

This is the honest close of the milestone's current state rather than a list of intentions. Each
row waits on something that is itself blocked.

| Gate clause | Waits on |
|---|---|
| The malformed corpus grown from slice scope to the **full format** | A format that has grown — JS-4's object model and JS-6's library |
| The **source parser** and the **regular-expression matcher** fuzzed | Surfaces that do not exist — JS-3b and JS-6 |
| The compile-time **nesting bound** holding under fuzz | The front end, and the nesting decision JS-2 takes |
| **Retained-bytes reporting** over the object model | An object model — JS-4 |
| **Agents** | The realm and agent surface — JS-8 |
| A stated **session and soak budget** | JS-9's own owner choosing one; four seeds, 25,000 iterations and 2,000 cycles are stated so a run is reproducible, not because any is a number something justifies |

## Exclusions — what this bundle does not show

1. **Everything in the table above.** The milestone is `In progress` for three exercises and no
   more.
2. **The plateau is a band, not a figure.** No number in the soak may be cited as a measurement;
   JS-5 owns measurement.
3. **A clean fuzz session is a statement about what the mutator reached**, and JS-9-001 records
   three things the tool got wrong that only running it against a deliberate defect exposed.
4. **One RID, one machine.** `win-x64`.
5. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
