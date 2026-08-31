# Evidence bundle JS-1-002

**Milestone:** JS-1. **What this bundle adds to [JS-1-001](../js-1/README.md):** roadmap
[section 7](../../roadmap.md#7-the-bytecode-format-and-the-verifier)'s **third discipline** — the
ordering assertions — which JS-1-001 did not carry and did not name.

**JS-1-001 is not superseded and is not edited.** A retained bundle is immutable, which is this
component's own rule at [JSD-0010](../../decisions/0010-which-review-rules-govern-this-profiles-documents.md).

**Verdict this bundle supports:** JS-1 is **In progress**, not accepted. Section 7's *second*
discipline — coverage-guided fuzzing — is still not implemented, and **nothing here has been
reviewed**.

Produced by `eng/collect-js-evidence.py`. Every file beside this one is its output.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-1-002` |
| Milestone | JS-1 |
| Adds | The ordering assertions of section 7's third discipline |
| Corpus | 60 entries, up from 59 |
| Core contract version | 1 (implemented; **not accepted**) |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

## The discipline this bundle exists for

Section 7 names three disciplines that make the verifier's answer list "provable rather than
aspirational". The retained corpus is the first. Fuzzing is the second and is still absent. The
third reads:

> **Ordering assertions.** The effective ceilings are materialized before the first byte is read; a
> refusal happens before the allocation it would have authorised; a declared count is compared
> against its bound before it sizes anything. These are asserted mechanically for every corpus
> entry including every failing one, **because the ordering is the property and the answer alone
> does not show it**.

JS-1-001 observed one ordering — that an unsupported profile is refused without a payload byte
being examined — and it is a different one. These are the other three.

### How they are observed

A composition root builds its own verification context, and the context is where the meter comes
from. So the root hands the verifier a **recording meter** and watches the order of every charge.
**Nothing in the profile knows it is being watched and nothing in the profile was changed to be
watchable**, which is what makes the observation worth anything.

| Check | What it observed |
|---|---|
| `ceilings-are-materialised-before-the-first-byte-is-read` | An 89-byte artifact against a 4-byte ceiling: `ResourceExhaustion`/`ArtifactBytes` with **zero meter events**. A ceiling materialised after the first read would have charged for the magic first |
| `a-well-formed-artifact-does-charge-for-what-it-allocates` | `addition` charges **88 allocated bytes and 97 work units across 91 meter events** |
| `a-declaration-past-its-bound-sizes-nothing` | Four entries refused on a declared count or maximum, each with **zero allocated bytes** |
| `allocation-is-proportional-to-the-bytes-present-not-to-what-they-declare` | All **60** entries under 64 allocated bytes per artifact byte; the highest is **2.9** |

**The second row is the load-bearing one.** "Nothing was allocated before the refusal" is true of a
verifier that allocates nothing ever, so a well-formed artifact is shown to charge before any of
the negative claims is read as meaning something.

## The control that shows why the discipline is separate from the corpus

`the-constant-pool-is-sized-before-its-count-is-checked` allocates the pool from the declared count
*before* comparing it against the limits section's maximum.

**The outcome, the reason and the diagnostic code are all unchanged.** Every replay row still
agrees; the corpus notices nothing. Only the ordering checks do:

| Entry | Charged before refusing |
|---|---|
| `a-constant-count-far-beyond-what-the-artifact-carries` | **960,000 allocated bytes from 57 artifact bytes** |
| `more-constants-than-the-limits-section-admits` | 32 bytes |

That is section 7's sentence demonstrated rather than quoted.

**And it is why the corpus grew.** The 32-byte case is *inside* the proportionality bound, so
without the hostile entry the injection would have been caught on one artifact and missed on the
other. `a-constant-count-far-beyond-what-the-artifact-carries` declares sixty thousand constants
and carries none, in an artifact of a few dozen bytes; its **answer is identical** to its
two-constant neighbour's, which is the point of it. The count sits below the format's own ceiling
on purpose, so what refuses it is the limits section's declaration and not a structural bound.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 138 architecture tests passed, 0 failed |
| Assurance gate mode | `assurance-gate.log` | Passed |
| Assurance release mode | `assurance-release.log` | **Refused, as it must** |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0**, 14 checks per run |
| Closure reports | `closure-*.txt` | 6 and 7 managed assemblies under JIT and trimmed; 0 under Native AOT |
| Negative controls, suite-judged | `negative-controls.log` | **16 injected, 16 caught, 0 skipped** |
| Negative controls, corpus-judged | `corpus-controls.log` | **8 injected, 8 caught, 0 skipped** |

The execution-only root now runs 14 checks rather than 10: the four above are the difference, and
they run in **every published mode**, so the ordering is observed under Native AOT as well as under
JIT.

## Exclusions — what this bundle does not show

1. **Section 7's second discipline is still absent.** Coverage-guided fuzzing over four surfaces —
   the verifier, the source tokenizer and parser, the regular-expression matcher, and the executor
   over verified-but-adversarial artifacts — is not implemented. Two of those four surfaces do not
   exist yet; the other two do, and are not fuzzed. Named here rather than left to be noticed.
2. **The ordering checks are over the retained corpus and one constructed case.** Sixty artifacts
   is sixty shapes. The proportionality bound would catch a declaration-sized allocation anywhere
   the corpus happens to reach, and nowhere it does not — which is exactly what the missing
   fuzzing discipline is for.
3. **`AllocationBytesPerArtifactByte` is a bound on the shape of the growth, not a measurement.**
   Sixty-four is comfortably above what the verifier's arrays come to and three orders of magnitude
   below what a declaration-sized allocation reaches. JS-5 owns measuring anything.
4. **One RID, one machine.** `win-x64`.
5. **The clause JS-1-001 carried was discharged elsewhere.** The public-API baseline is
   [JS-3A-002](../js-3a-002/README.md)'s, not this bundle's.
6. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
