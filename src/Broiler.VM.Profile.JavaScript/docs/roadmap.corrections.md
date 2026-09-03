# Broiler.VM.Profile.JavaScript roadmap — corrections and rejections

**Last updated:** 2026-09-03

**This file is part of the [Broiler.VM.Profile.JavaScript roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split). It carries no numbered section of the
plan, because it is not part of the argument: like the [evidence ledger](roadmap.status.md) and
the [decision records](decisions/README.md), it is a record, and it numbers its own sections.

**What this file is.** The plan's dated history. The roadmap states what is planned, in the
present tense, in one voice. When implementation or a dated decision invalidated, rejected,
re-scoped or corrected something the roadmap said, the roadmap is edited to say the new thing —
and the reading it replaced is recorded here, with its date and its authority.

**What this file is not.** It is not the ledger: [roadmap.status.md](roadmap.status.md) is the
authority for what has been accepted, and nothing here advances a milestone. It is not a decision
record: [`decisions/`](decisions/README.md) holds the argument, the alternatives, and the reason
each was refused, and this file cites those records rather than restating them. It is an **index
over both**, answering one question a roadmap cannot answer about itself: *which sentence of the
plan changed, when, and on whose authority.*

**Why it exists at all**, stated once so the split is not mistaken for tidiness:

1. **A plan that carries its own corrections inline reads as two plans at once.** A section that
   states a rule and then spends a paragraph retracting an earlier version of it makes a reader
   decide, sentence by sentence, which half is current. The roadmap's job is to be readable
   start to finish by someone who has never seen an earlier draft.
2. **A correction deleted without a record is indistinguishable from a plan that never said the
   thing.** This component's whole method is that a claim can be re-derived by a reader. That
   applies to the plan's own history as much as to a corpus entry.
3. **A reader who remembers the earlier reading needs somewhere to find out it was retracted.**
   Several of the entries below were load-bearing readings that whole sections were written
   around: the catalog-wide maximum clamp, the copied standard library, the open
   sibling-reference question. Someone who planned against one of them is owed the retraction, not
   silence.

---

## 1. Reading this file

Every entry carries the same five fields, and an entry with a missing field is an incomplete
record rather than a short one:

| Field | What it holds |
|---|---|
| **ID** | `JSC-nn`, minted once and never reused. Referenced from the roadmap where the correction is load-bearing at the point of reading. |
| **Where** | The roadmap file and section the correction lands in. |
| **What the plan said** | The reading that was replaced, stated fairly enough that someone who planned against it recognises it. Not a straw man, and not a quotation of a file that no longer contains the words. |
| **What replaced it** | The reading the roadmap now carries. |
| **Authority and date** | The decision record, core ADR, evidence bundle, or ledger row that settles it, and the date it was settled. **An entry with no authority outside this file is not a correction; it is an opinion**, and does not belong here. |

Four rules govern the set:

- **The roadmap never carries a correction inline.** Where the settled statement is enough, the
  roadmap states it and says nothing about what it replaced. Where a reader of that section would
  actively be misled without knowing an earlier reading existed — because they may have planned
  against it — the roadmap carries a bare pointer of the form *(corrected: JSC-nn)* and nothing
  more.
- **An entry records a changed *reading*, not a changed sentence.** An edit that sharpens wording,
  fixes a tense, or repairs a reference without changing what the plan means is not a correction
  and gets no entry — a file that recorded every edit would bury the entries a reader needs behind
  the ones nobody does.
- **An entry is never edited away.** A correction that a later change reverses gets its own new
  entry naming the one it reverses. The history is append-only, which is what makes it worth
  reading.
- **Nothing here is a status claim.** Every entry below describes a change to a *plan*. Whether
  anything was implemented, and whether any of it was accepted, is the ledger's answer and only
  the ledger's.

---

## 2. Corrections

Minted in the order the readings were replaced, and indexed below by the roadmap section each
lands in, so this file can be read beside the plan either way. Every
entry below is dated **2026-08-31** — the day JS-0, JS-1, JS-3a and JS-9 landed together with the
decision records that settle them — except where an entry says otherwise, and every entry carries
its own date. **A single date is a fact
about this programme's history, not a filing convention**: the plan was written before any of it
existed and met its first implementation all at once. **A later date is a different fact**: an
entry carrying one is a reading the plan replaced when it was read against its own gates rather
than against an implementation, which is why its authority is a gate, a ledger row, or the map
rather than a decision record.

| ID | Where | In one line | Authority |
|---|---|---|---|
| [JSC-01](#jsc-01) | roadmap §3 | The core's catalog-wide clamp on profile **maxima** was retracted; only a neighbour's **default** still reaches this profile | ADR 0001, ADR 0007 |
| [JSC-02](#jsc-02) | roadmap §3 | Four of the fifteen budget rows are declared inapplicable today, where the intended matrix says charged | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) |
| [JSC-03](#jsc-03) | roadmap §4.2 | The regular-expression backend item is ruled **do not wait**, not **wait** | [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md) |
| [JSC-04](#jsc-04) | roadmap §4.2 | The snapshot stop condition is a date and a commit budget, not a placeholder | [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md) |
| [JSC-05](#jsc-05) | roadmap §5 | A profile's own siblings sit outside the two-assembly reference set, and rule A11 gained a sibling exemption | ADR 0011 P1, ADR 0001 rev 5, [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) |
| [JSC-06](#jsc-06) | roadmap §5 | The composition roots are not named `Broiler.VM.Profile.JavaScript.Composition.*` | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) |
| [JSC-07](#jsc-07) | roadmap §5 | The assembly topology is decided rather than hypothesised: three assemblies, plus composition roots | [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) |
| [JSC-08](#jsc-08) | roadmap §7, §9 | The position encoding is stated, and stating it found a conflation in the verifier | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| [JSC-09](#jsc-09) | roadmap §7 | The third of section 7's three disciplines was never implemented, and no bundle had said so | Bundle JS-1-002 |
| [JSC-10](#jsc-10) | roadmap §8 | The value, frame and call ABI is taken, and the answer is **replace** | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| [JSC-11](#jsc-11) | roadmap §10 | The entry-point answer is taken: an artifact declares named program entries | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) |
| [JSC-12](#jsc-12) | roadmap §15 | Two composition roots differing by one reference; the difference is the `execution-only` label, and the second root claims none | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md), [JSD-0003](decisions/0003-deployment-composition-labels.md) |
| [JSC-13](#jsc-13) | roadmap §16 | The persisted cache key is the core's closed set, cited rather than re-enumerated — three hand-written enumerations were each wrong differently | core ADR 0006; ADR 0010 for source identity |
| [JSC-14](#jsc-14) | roadmap §18 | The argument-channel row is re-graded **strong**, and the result channel is split out of it | [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md) |
| [JSC-15](#jsc-15) | delivery §19, §20 | `JS-3` became `JS-3a` and `JS-3b`, split by dependency rather than by size | the roadmap itself; ledger section 2 |
| [JSC-16](#jsc-16) | delivery §19 | The public-API baseline clause moved from JS-0 to JS-3b to JS-3a, and closed there | [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) |
| [JSC-17](#jsc-17) | delivery §19 | JS-6 is re-scoped from a copy to a rewrite, before it starts | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| [JSC-18](#jsc-18) | gates §17 | No declared repetition count was fixed at JS-1; the obligation moves to the milestone that first measures | ledger section 2; bundle JS-1-001 |
| [JSC-19](#jsc-19) | gates §23 | One risk narrowed, one stop condition fired and honoured, one discharged in part — three different states | [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md), [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md), ADR 0007 |
| [JSC-20](#jsc-20) | roadmap header; delivery §19; ledger header and closing summary | The plan stopped transcribing counts and inventories that the ledger already holds | ledger update rule 10 |
| [JSC-21](#jsc-21) | placement, throughout | This profile is product projects inside `Broiler.VM`, and mechanisms the plan assumed it would own are adopted from the host component | [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md), [JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md) |
| [JSC-22](#jsc-22) | roadmap §6 | The list of deliberately underspecified surfaces is per manifest; the slice answered two of the five in code and published no list | the shipped slice surface, against [JSD-0002](decisions/0002-feature-manifest-allocation.md) |
| [JSC-23](#jsc-23) | delivery §19, §20 | JS-9 opens against JS-1 and closes after JS-8; the delivery order drew only its closing edge | ledger section 2, row JS-9 |
| [JSC-24](#jsc-24) | roadmap §4.3 | The copy table's standard-library row is a rewrite, and two neighbouring rows follow it | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| [JSC-25](#jsc-25) | roadmap header; ledger preamble | The public API baseline was adopted from the host component and became this family's own | [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) |
| [JSC-26](#jsc-26) | roadmap §7 | Format version 1 carries a section the plan never listed, and reserves an interning the plan described as performed | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md); the shipped format |
| [JSC-27](#jsc-27) | roadmap §8 | The value representation is neither candidate the plan offered, and an interpreter frame is a heap object | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| [JSC-28](#jsc-28) | roadmap §9 | Every diagnostic code is `core-result` today; "half the registry" was a projection stated as a fact | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| [JSC-29](#jsc-29) | roadmap §18 | Seven of ten amendment rows named no counterweight position; four of those seven had a recorded and contrary one | the other profile's own roadmap |
| [JSC-30](#jsc-30) | roadmap §4.5, §14; delivery §19, §25 | The ingested suite's attribution is a second licence obligation the seed's notice change never covered, and it lands at JS-3a | gate 12; ledger section 3 |
| [JSC-31](#jsc-31) | roadmap §18; delivery §19, §25; gates §22 | The amendment register answers nothing at release, but its **state** is published there rather than inferred | gate 1; roadmap §18's unexecutable procedure |
| [JSC-32](#jsc-32) | gates §23; delivery §19, §25 | The extraction-gate state is recorded at JS-10 regardless, because no mechanism-owning milestone could be sure of having anything to record | gates §23's own stop condition; the map's blank-cell rule |
| [JSC-33](#jsc-33) | delivery §19, §25; gates §22 | The support table names the declared-default vector's reconciliation as unowned, rather than publishing the vector and stopping | ledger section 3; gate 1 |
| [JSC-34](#jsc-34) | roadmap §5; gates §21 | The fuzz mutator and the soak are in a composition root too, not only the corpus — so the execution-only closure carries them | the shipped composition roots, against rules A11 and A12 |
| [JSC-35](#jsc-35) | roadmap §7; gates §21 | Each of the four exhaustion dimensions gets a corpus entry, because an exhaustion answer carries no code and the registry's binding cannot reach it | the corpus manifest, against rule N7 |
| [JSC-36](#jsc-36) | delivery §19, §25 | Four clauses the release gates require and the map assigns, written into no exit gate — one defect with four instances | gates §22 gates 1 and 13; gates §24 |
| [JSC-37](#jsc-37) | delivery §19, §25 | A fifth instance, and the first one a correction itself created: JSC-35's exhaustion entries were written into the evidence matrix and the map, and into no exit gate | gates §21; [JSC-35](#jsc-35); the map's blank-cell rule |
| [JSC-38](#jsc-38) | ledger §2; delivery §19, §25; bundle JS-9-001 | The fuzz target is seeded mutation and was called coverage-guided; the guidance section 7 asks for was in no exit gate | the fuzz target's own source; ledger update rules 2 and 4 |
| [JSC-39](#jsc-39) | roadmap §7; gates §21; delivery §19; ledger §2 | The verifier names seven exhaustion dimensions, not four, and JSC-35's binding covered four of seven arms | the verifier's own source; the corpus manifest; rule N7 |
| [JSC-40](#jsc-40) | roadmap §5, §14; gates §21; delivery §19, §25 | The conformance harness is a never-advertised composition root, not a test project, and the ingestion scan is over advertised closures rather than published ones | rule A11; the composition register's advertised set |
| [JSC-41](#jsc-41) | roadmap §7; ledger §2 | The artifact-bytes exhaustion is the core's answer and no host ceiling reaches the verifier's own arm for it; six of the seven dimensions are this profile's to answer | the core's verification path; the retained corpus manifest |
| [JSC-42](#jsc-42) | roadmap §7; gates §21; delivery §19; ledger §2 | A session's guidance is keyed on the answer this profile publishes and not on an edge, and what a session judges about itself is its loop rather than its growth | [JSD-0013](decisions/0013-the-fuzz-sessions-coverage-signal.md) |
| [JSC-43](#jsc-43) | roadmap §9; delivery §19; ledger §2 | The slice front end is written in this checkout rather than ingested from the seed, so JS-3b's tokenizer, parser, validation stage and lowering need neither of JS-2's blockers | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) |
| [JSC-44](#jsc-44) | roadmap §9; the published registry; ledger §2 | The registry's embedder-seam half is twenty-two rows at revision 2, its reason column is `-`, its stages are their own vocabulary and its reachability is a retained source corpus | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) |
| [JSC-45](#jsc-45) | delivery §19; ledger §2 | JS-4 deletes JS-1's hand-written PROGRAMS; the instruction buffer beside them is the source lowering's back end and is not deleted | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) |
| [JSC-46](#jsc-46) | roadmap §9; delivery §19 | A source is tokenized at most once during COMPILATION and the verifier tokenizes nothing; the clause as written was satisfiable only by a fused design | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) |
| [JSC-47](#jsc-47) | roadmap §7; delivery §19; ledger §2 | One of JS-9's two unfuzzed surfaces exists now, so it is a gap rather than an absence, and the two admissions may not share a sentence | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md); the checkout's own front end |
| [JSC-48](#jsc-48) | ledger §2; delivery §19 | The soak's plateau reading was coupled to what the process allocated BEFORE the soak, so it measured heap the collector had not returned rather than a per-cycle leak | the check's own curve, read on four platforms |
| [JSC-49](#jsc-49) | roadmap §9; ledger §2 | The feature manifest is a validation-stage clause and not a grammar restriction; the parser reads JavaScript and refuses nothing | [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md), decision 1, read against its own implementation |
| [JSC-50](#jsc-50) | roadmap §9; ledger §2 | The parse depth bound has a measured maximum, and it dominates the format's operand-stack ceiling | the parser's own measured recursion limit |
| [JSC-51](#jsc-51) | roadmap §14; ledger §2 | The closed set of configuration failures is the plan's five plus one the plan states without naming | [JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md), decision 6 |
| [JSC-52](#jsc-52) | roadmap §14; ledger §2 | A conformance case runs in a runtime of its own; the plan specifies aggregation and says nothing about isolation | the harness's own first scored run |
| [JSC-53](#jsc-53) | [JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md); ledger §2, §3 | The harness's metadata reader could not read the dialect it claimed to be shaped like, and a suite is read whole, so a real checkout would have scored nothing | five suite-shaped files put through the harness |
| [JSC-54](#jsc-54) | roadmap §14; ledger §2 | A refusal answers a suite's negative expectation only when it was a language answer; this manifest's ordinary refusal is not one | [JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md), decision 3 |
| [JSC-55](#jsc-55) | ledger §2, the construct census | Seven of the twenty-four files measured as "the Octane benchmark" are not benchmark sources, and over the seventeen that are, the thirteen highest-ranked constructs admit none | a census re-run over the same checkout |
| [JSC-56](#jsc-56) | `docs/compositions.md` section 3; roadmap section 15; ledger section 2 | The `narrow-runtime-compiler` label is claimed by an end-user host handed source from outside its own image, which is narrower than the source surface the register said it waited on | [JSD-0017](decisions/0017-the-end-user-host-and-what-an-exit-code-promises.md) |
| [JSC-57](#jsc-57) | ledger section 2, the construct census; roadmap section 9 | The census reads at the largest nesting bound and the shipped default is 64, so two Octane files are refused for depth before the manifest is consulted and the two documents disagree | the host run over the same checkout at both bounds |
| [JSC-58](#jsc-58) | `SliceSourceCompiler`'s own remark; roadmap section 9; ledger section 2 | The unreachable-code exclusion named the rare shape; thirteen test262 files fail on the common one, a loop whose body always breaks | the host over test262 |
| [JSC-59](#jsc-59) | [JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md) decision 1; ledger section 2 | The dialect reader could not read a CR-only file or an indented block, and no check this component wrote could have found either | the harness over test262 |
| [JSC-60](#jsc-60) | `SliceSourceCompiler`'s remark as JSC-58 left it; ledger section 2 | The loop-continuation defect JSC-58 called fixable is fixed; the host's completions over test262 moved 103 to 116 and the harness's totals did not move at all | the host over test262 before and after |
| [JSC-61](#jsc-61) | the tokenizer's trivia handling; ledger section 2 | `#!` opening a source text is a comment in the language since ES2023 and this tokenizer did not know it | six test262 files |
| [JSC-62](#jsc-62) | roadmap sections 7 and 9; ledger section 2 | The temporal dead zone was unexpressible: format version 1 had no instruction that could fail at all, so reading an uninitialised slot answered `undefined` | eight test262 cases |
| [JSC-63](#jsc-63) | [JSC-62](#jsc-62) and the opcode it added; ledger section 2 | The dead-zone opcode declared a push it never made, which the WRITE half could not extend; it is a guard that moves nothing, and the write is taken | the acceptance suite, no suite figure moving |
| [JSC-64](#jsc-64) | [JSC-60](#jsc-60); the declaration lowering's and the executor's remarks; ledger section 2 | Dead code after a terminator was declined on a hazard that was not there: the executor writes `undefined` into every slot, in a loop, on purpose | the executor's own initialisation |
| [JSC-65](#jsc-65) | `SliceSourceCompiler`'s remark; [JSC-64](#jsc-64); ledger section 2 | A loop nothing can leave was called the format's answer three times: the verifier requires every REACHABLE path to return, and that loop has no path that ends | the verifier's own rule |
| [JSC-66](#jsc-66) | the ledger's account of the four remaining test262 failures, twice; [Bundle JS-3B-002](evidence/js-3b-002/README.md) section 3; [JSC-54](#jsc-54) | The harness had no way to exclude by feature at all, and the cost was not four failures but 117 passes it had not earned | the suite's own `features.txt`, and a census over every scored case |
| [JSC-67](#jsc-67) | the ledger's unopened-dependency row for the language edition, open since JS-0; the JS-10 row; Bundle JS-3A's exclusion 5 | The row was written as though there were two states and section 24 defines three: the edition is pinned PROVISIONALLY, and two of the three actions bought three checked claims and one disagreement | ECMA-262 retrieved at five editions and hashed |
| [JSC-68](#jsc-68) | the ledger's conformance-suite row and its account of the test262 runs; Bundles [JS-3B-001](evidence/js-3b-001/README.md) and [JS-3B-002](evidence/js-3b-002/README.md) | The suite pin was not merely transient, it was SELF-CERTIFYING: `--pin` writes its digest into the directory it just read, so the checkout was vouching for itself | the archive retrieved and hashed twice, independently |

### JSC-01

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses),
and the risk row in gates [section 23](roadmap.gates.md#23-risks-and-stop-conditions).

**What the plan said.** The core clamped every runtime ceiling to the tightest **maximum** in the
catalog, one step before it clamped to the tightest default. Section 3 planned around that at
length, and the consequence it drew was sharp: this profile must publish finite guest-load maxima,
a neighbour declaring those dimensions inapplicable could write zero into them, and `eval` would
fail with a resource exhaustion naming a dimension the other profile never used — in a verifier
that had done nothing wrong. The obligation that followed was that this profile publish an
unconstrained maximum on any dimension a neighbour might declare inapplicable.

**What replaced it.** The clamp was a defect in the core and has been removed. A profile maximum
is applied **at verification, against the profile the artifact names**, which is where the core's
own record always placed it; it binds this profile's own artifacts and reaches no profile composed
beside it. The exposure is gone, and so is the obligation.

**What survives, moved one column across.** A neighbour's zero **default** still reaches this
profile wherever a host adopts defaults rather than stating ceilings, because at runtime creation
no profile has been selected and the tightest default in the catalog is the only safe answer.
That is the smaller, real version of the same hazard, and it is the one the plan now carries.

**Authority and date.** The core's removal of the clamp and the correction of the two core records
that still described it, 2026-08-31; graded in
[JSD-0004](decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md), which sets this
profile's `NestedLoadDepth` **default** generously rather than at the 1 this profile would need,
precisely so a neighbour adopting defaults is not strangled.

### JSC-02

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses),
the fifteen-row budget declaration matrix.

**What the plan said.** The matrix names all fifteen dimensions **charged**, and JS-1 fixes it.

**What replaced it.** The matrix as written describes the profile this component is growing into.
The descriptor JS-1 actually built declares **`HostCalls`, `NestedLoadDepth`, `NestedLoadFanOut`
and `NestedLoadBytes` inapplicable**, because the slice imports no host capability and declares no
guest-initiated load — which makes those four structurally unreachable rather than merely unused,
and the catalog checks a declaration against the structural consequences of the rest of the
descriptor. Declaring them charged would have been a claim the descriptor contradicts two rows
further down. **JS-6 flips the host-call row** when the standard library imports something, and
**JS-8 flips the three nested rows** when guest loads are declared. The defaults on all four stay
generous, for JSC-01's reason.

**Authority and date.** [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md)
correction 1, 2026-08-31.

### JSC-03

**Where:** roadmap [section 4.2](roadmap.md#42-what-after-the-fix-work-lands-can-and-cannot-mean),
the waited-on table.

**What the plan said.** For the regular-expression backend adoption in the seed: "**Wait**, or
scope the first manifest to exclude regular expressions and record the exclusion. Either is
legitimate; drifting into it is not."

**What replaced it.** The ruling is **do not wait**, and the first manifest is scoped to exclude
regular expressions with the exclusion published. `broiler.javascript.regexp` is already a
separate manifest identity, so the exclusion is a manifest that has not been minted rather than a
hole in one that has. The waited-on set is therefore **one `Wait` and four `Do not wait`**, and
the shape of that answer is deliberate: waiting is justified only where the seed is about to
become more correct *in the surface being copied*.

**Authority and date.** [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md),
2026-08-31.

### JSC-04

**Where:** roadmap [section 4.2](roadmap.md#42-what-after-the-fix-work-lands-can-and-cannot-mean).

**What the plan said.** "JS-0 records a date, or a commit-count budget, after which the snapshot is
taken as-is."

**What replaced it.** The stop condition is recorded: **2026-11-30, or 400 further commits on the
seed's default branch beyond the candidate revision, whichever comes first.** After that trigger
the remaining waited-on item is re-derived on this side of the fork and JS-2 records what it cost.
If the trigger arrives with the awaited module and early-error work unlanded, **the stop condition
fires and the wait is abandoned rather than extended**.

**What this did not close.** JS-2's other blocker — the core contract is not accepted — is
untouched, and the ledger still carries it with its holder and its unblock condition.

**Authority and date.** [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md),
2026-08-31.

### JSC-05

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph).

**What the plan said.** The profile's Broiler.VM reference set is exactly the two core assemblies —
and one question inside that rule was the core's to answer and was **open**: whether a profile's
own format sibling counts as a member of the set. Section 5 drew the format pivot without knowing
whether the graph it drew was legal.

**What replaced it.** It is legal. ADR 0011's obligation P1 carries an editorial revision stating
that the set is of **Broiler.VM-owned** assemblies and that a profile component's own siblings —
its format assembly, its lowering, its composition roots — are not members of it; ADR 0001 carries
the same qualifier for the same reason, which is that the format pivot is incoherent unless a
profile may reference its own format assembly. **One thing had to change on the core's side:**
rule A11 forbade any reference to a `Broiler.VM.Profile.*` assembly from outside a composition
root, which made the pivot unreachable. A11 now exempts a sibling **in the same profile family**,
keyed on the language segment, so a JavaScript project referencing a WebAssembly one is still a
violation.

**Authority and date.** ADR 0011 obligation P1, editorial revision 2026-08-31; ADR 0001 revision 5;
this profile's half is [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md).

### JSC-06

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph),
the assembly table.

**What the plan said.** Composition roots are named `Broiler.VM.Profile.JavaScript.Composition.*`.

**What replaced it.** They cannot be, and the reason is a collision rather than a preference:
**every architecture rule that identifies a profile assembly does so by the
`Broiler.VM.Profile.` prefix**, so a composition root under that prefix *is* a profile assembly to
rules A8, A11 and A13 — and A8, which forbids a profile project from referencing the runtime,
fired correctly on the first build, because a composition root must reference the runtime. The
roots are `Broiler.VM.Composition.JavaScript.ExecutionOnly` and
`Broiler.VM.Composition.JavaScript.SliceCompiler`, matching the core's own
`Broiler.VM.Composition.*` convention. Rule N4 no longer covers them; A12 and the composition
register hold them instead, which is the right pair of rules for a composition root.

**Authority and date.** [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md)
correction 3, 2026-08-31.

### JSC-07

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph).

**What the plan said.** The assembly names "are hypotheses until JS-0 proves the graph with project
shells and an explicit assembly budget", and whether the profile is one assembly or several "is a
JS-0 decision with a dated record, not an assumption".

**What replaced it.** JS-0 took the decision and built the shells. The family is **three
assemblies** — the format, the profile, and the lowering — plus one composition root per named
composition, with the format as the pivot both the executor and the lowering depend on and which
depends on nothing. The single-assembly default was not taken and the split is justified in the
record: the format must be referenced by two projects that may not reference each other, and the
lowering must be absent from an execution-only closure. Rules N1 through N4 hold the graph.

**Authority and date.** [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md),
2026-08-31; bundle JS-0-001.

### JSC-08

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier) and
[section 9](roadmap.md#9-the-semantic-front-end-and-lowering).

**What the plan said.** "This profile states at JS-3a which of the four fields it populates, what
it puts in the two coordinates, and what a section index of `-1` means for it" — written in the
future tense, and written as something a paragraph could satisfy.

**What replaced it.** The encoding is stated, and it is a pair of factories and a rule rather than
a paragraph: rule N9 asserts that a core position record is constructed in one file of the profile
assembly and nowhere else. **Stating it found a defect a paragraph would not have found.** Every
diagnostic the link and walk stages produce carries an offset into the *code section*, and every
one of them went through a helper that set the section index to `-1`, which under the encoding
means an offset into the artifact. The number was right and the frame it named was wrong, so a
consumer resolving it would have landed on an unrelated byte. **This is exactly the failure
section 7 predicted, found inside one profile rather than between two.** Four retained corpus
entries now pin the encoding, each failing differently if it moves, and the corpus manifest gained
a position column to carry them.

**A second correction landed in the same change.** `EntryStackNotEmpty` was declared at JS-1 and
emitted by nothing: a path arriving at an entry point with operands on the stack was reported as
an inconsistent-stack-height join, which is a worse diagnostic and was also **order-dependent** —
which of the two arrivals reported the mismatch was a property of a worklist order no artifact can
see. The check is on the **edge** now, which makes the answer a property of the program.

**Authority and date.** [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md),
2026-08-31; bundle JS-3A-001.

### JSC-09

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the third of the
three disciplines.

**What the plan said.** Nothing wrong — and that is the point of this entry. Section 7 asks that
three orderings be asserted mechanically **for every corpus entry, including every failing one**:
the effective ceilings materialized before the first byte is read, a refusal before the allocation
it would have authorised, and a declared count compared against its bound before it sizes
anything.

**What was actually true.** JS-1 observed **one** ordering, and it was a different one. No bundle
claimed the discipline and no bundle named its absence, so it was **silently missing rather than
wrongly claimed** — which is the harder failure to notice, because an exclusion list is where a
reader looks for what a bundle did not show.

**What replaced it.** The section now says the quantifier out loud: **all three** orderings are
asserted mechanically for **every** corpus entry including every failing one, and a bundle that
observes one of the three has not demonstrated the discipline. Bundle JS-1-002 lands the three
assertions, and the ledger records the discrepancy rather than letting the original bundle stand
as if it had covered them.

**Why this is a correction and not a defect report.** The plan did not change. What changed is the
record of what the plan had been shown to hold, which is exactly the distinction the ledger's
first section draws between plan, observed repository state, and accepted evidence — and the
failure mode it exists to catch.

**Authority and date.** Bundle JS-1-002, 2026-08-31; ledger section 2, row JS-1.

### JSC-10

**Where:** roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model).

**What the plan said.** The value representation is an open question, stated as eight rows to be
decided, with the representation row gating the others: the seed boxes every value on the heap and
an unused eight-byte tagged struct sits beside it; "either answer is defensible; an unrecorded
answer is not". Gates [section 23](roadmap.gates.md#23-risks-and-stop-conditions) made shipping
library code while the question was open a stop condition.

**What replaced it.** The answer is **replace**: this profile keeps its own tagged struct and does
not adopt the seed's boxed hierarchy. The reason recorded is not that a struct is faster, though
section 4.4 records the boxing as the seed's own most-measured defect. It is that **this profile
already has a struct value model with an executor written against it and a retained corpus pinning
its semantics** — so adopting the hierarchy would not avoid a rewrite, it would move the rewrite
onto JS-1's executor and replace this component's working code with a defect the seed has measured
and not fixed. All eight rows are recorded, each with what it buys, what it costs, and what would
falsify it.

**What it did not settle.** No fixtures and no Native AOT representation probes are retained,
because seven of the eight rows have nothing to exercise until JS-4 and JS-5 exist; the gate that
requires them is JS-4's. **A taken entry gate is not a started milestone**, and JS-4 depends on
JS-2.

**Authority and date.** [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md), 2026-08-31.
The consequence for JS-6 is [JSC-17](#jsc-17); the consequence for the stop condition is
[JSC-19](#jsc-19).

### JSC-11

**Where:** roadmap [section 10](roadmap.md#10-execution-mapping-javascript-onto-the-core-lifecycle).

**What the plan said.** An invocation request carries one UTF-8 entry-point name and no argument
channel; three answers exist and JS-1 picks one — encode the call into the entry-point text, lower
a one-line calling program and verify it as a guest-initiated load, or propose an amendment.

**What replaced it.** The first answer, in its honest form: **an artifact declares named program
entries and an invocation names one.** Arguments, where a program needs them, are encoded by the
lowering into the artifact the host asked for, which is what a browser does anyway because the
caller-driven path compiles a *program* rather than a call. An entry-point name nothing is bound to
is a **ReferenceError** carried as this profile's typed fault, because resolving a name and finding
nothing is what that error is in the language. The conventional name is `main` and nothing in the
format privileges it.

**The cost is recorded rather than hidden.** A host that wants to call `f(1, 2)` against an
already-instantiated realm cannot: it must lower a new program and verify it. The second answer —
lowering a one-line calling program through the mediator — is correct, costs a verification, and
needs guest-initiated loads, so it is JS-8's and not available yet. **Rejected outright:** encoding
the call into the entry-point text, which works and is ugly, and which would make the entry-point
name a parsed surface with its own grammar, escaping and early errors — a second format inside a
string, in the one place the contract deliberately carries bytes it does not interpret.

**Authority and date.** [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md),
2026-08-31.

### JSC-12

**Where:** roadmap [section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding).

**What the plan said.** Three composition labels, and JS-1 stands up "the named execution-only
composition root".

**What replaced it.** Two composition roots. They differ by exactly one reference — the lowering —
and **that difference is the whole of the `execution-only` label**: the execution-only root names
the profile and not the compiler, so it cannot turn source into an artifact however it is invoked,
and every artifact it runs is precompiled and read as bytes from the retained corpus. The
slice-compiler root beside it **claims no label at all**. A flag on one binary would have made the
difference a run-time choice inside one closure, and **a closure report cannot see a flag**.

**Why the second root claims nothing.** `narrow-runtime-compiler` belongs to a composition carrying
a lowering for a named restricted **source** surface, and there is no source surface until JS-3b
writes the tokenizer and the static semantics. What the slice-compiler root lowers is a
programmatic builder. It is registered as a demonstration; JS-3b claims the label.

**Authority and date.** [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md)
correction 4 and [JSD-0003](decisions/0003-deployment-composition-labels.md), 2026-08-31.

### JSC-13

**Where:** roadmap [section 16](roadmap.md#16-persistence-and-the-code-cache).

**What the plan said.** An earlier statement of the persisted cache key named **source identity**
first, which quietly made the key a *derivation* — source plus lowering version plus format
version — whose validity rests on deterministic lowering, a property section 9 preserves but
explicitly declines to warrant. A related earlier version stated the per-import capability tuple as
two fields.

**What replaced it.** The key is over the **output bytes**, and the core's set is cited rather than
re-enumerated. Three enumerations of it in this plan were wrong in three different ways, which is
the argument for citing it: source identity was named first, making the key a derivation; the
per-import capability tuple was stated as **two** fields where the core defines **seven** —
capability ID, version, signature ID, kind, reentrancy, exception-translation mode, and whether an
optional import was bound, the last three load-bearing because all three change the legal control
flow at a call site; and the enumeration omitted **the profile's declared hard maxima**, which is
precisely the term that is in the persisted key while being *out* of in-process handle identity,
where the effective ceilings subsume it. Source identity is **echoed, not compared**: it is the
host's own lookup key, which the core records and never compares.

**Authority and date.** The core's own persisted-envelope key set, read against the plan's
enumeration, 2026-08-31; the source-identity row is the core's embedding-decisions record rather
than the ownership one, which is itself worth knowing — the key set and the thing that is echoed
rather than compared are written down in two different places. **This entry is here rather than in
a decision record because no decision was needed** — the core had already fixed the answer, and the
plan had restated it wrongly,
three times.

### JSC-14

**Where:** roadmap [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core).

**What the plan said.** The argument-channel candidate was graded **weak**, on the ground that a
fixed-entry-point profile whose lowering encodes arguments into the program it compiles would not
need it — and argument and result were filed as one row.

**What replaced it.** Two corrections, and the second is the sharper one.

**The argument channel is graded strong.** The other intended profile rates it the strongest ask
in its own document, on the ground that a language with no parser, no text format, no dynamic
loads and no notion of a program still needs it — **which is the counterweight test passing, not
failing.** And it stops being mild for this profile the moment it hosts another one:
`instance.exports.f(a, b)` is a typed call whose arguments originate here.

**The result channel is split out and graded none-needed.** The typed payload already carries
results, and several of them, so multi-value returns are expressible today. Filing argument and
result as one amendment would put two differently-scoped versions of one capability into the
register, **which is how a capability gets approved at the wrong width.**

**The general rule the correction establishes.** A counterweight grade is not this profile's to
write alone: the core's procedure requires every amendment record to state whether the other
intended profile could use the capability, is unaffected, or refuses it, so **a row graded without
knowing the other profile's grade is a row whose answer changes depending on which component files
first.**

**Authority and date.** [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md)
part 2, 2026-08-31.

### JSC-15

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones) and
[section 20](roadmap.delivery.md#20-delivery-order).

**What the plan said.** One milestone `JS-3` carried the conformance harness *and* the
static-semantics consolidation *and* the lowering, sequenced after JS-2. Section 20's list of
decisions that need no copied code omitted the harness.

**What replaced it.** `JS-3a` and `JS-3b`, split **by dependency rather than by size**. Nothing in
the oracle method needs a copied line: it needs a scoring target, and JS-1 already produces one.
Leaving the harness fused to the static-semantics work put this component's only external
correctness signal behind **both** of its external blockers — the core acceptance gate and the
seed's waited-on set — when it needed to be behind neither. A team that serialised them would
spend the whole acceptance wait with no oracle.

**What the split did not change.** Nothing is accepted under either shape, so the split changes no
evidence claim. It changes what a reader is told is schedulable today, and it makes the milestone
set twelve rows rather than eleven.

**A bookkeeping note that was true for one change and is not any more.** For a short window the
delivery file carried the new shape and the ledger carried the old one, and the delivery file said
so. The ledger carries the new shape now; the note has been removed rather than left to be
disbelieved.

**Authority and date.** The roadmap itself, 2026-08-31; ledger section 2 carries the new shape.

### JSC-16

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-0's exit gate.

**What the plan said.** JS-0's gate asks that "the public API baseline mechanism exists and compares
in both directions". The clause could not be closed at JS-0 and was carried; JS-1 named the
obstacle and parked it at JS-3b.

**What replaced it.** The obstacle was real: the component's own describer describes a surface by
**loading** an assembly, which needs a project reference, which rule A11 forbids a test project to
have on a profile. The route taken describes the family from its **build output** with a metadata
load context, which reflects **without running anything** — so it needs neither the reference A11
forbids nor the execution invariant 2 forbids. Rule N10 holds it and the clause is closed at
**JS-3a**, which needed neither of JS-3b's two blockers.

**Rejected: leaving the clause at JS-3b and recording that it is schedulable.** It is the cheapest
honest option and it was considered. The ledger would then carry, for two milestones, an open
clause with a note saying it could be closed at any time — and **a clause nobody can schedule and a
clause nobody has scheduled read the same in a table.**

**Authority and date.** [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md),
2026-08-31; bundle JS-3A-002. Two implementation defects the baseline work uncovered are recorded
in that decision and not restated here.

### JSC-17

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-6.

**What the plan said.** JS-6 copies the seed's standard library and its tests, "as a port wherever
the value model changed at JS-4, and labelled as such" — a qualifier on part of the milestone.

**What replaced it.** The qualifier is the whole milestone. The seed's library is typed against a
boxed value base type this profile does not adopt ([JSC-10](#jsc-10)), so it is **re-implemented
against the value struct rather than copied and re-typed**. This is a re-scope of size, not of
order: JS-6 sits where it sat.

**Rejected: a mechanical re-typing.** If the seed's library named one abstract base and nothing
else, a re-typing would be a copy with a find-and-replace. It does not: a library that
pattern-matches on concrete value subclasses is re-implemented wherever it does, and the parts that
survive arrive as unreviewed code that no test in this component covers, **wearing a copy's name
and a copy's schedule.** Naming it a rewrite is the cheaper mistake.

**What is untouched.** The storage half of the copy table — shapes and the transition table,
shape-only slot storage, element arrays, the named-property store — is copied with its tests and
its recorded defect history, because it is about *storage keyed by a value* and not about the
value's representation. The front-end analyses JS-3b re-homes are untouched.

**What JS-6 must now carry that a copy would not.** Its own scope estimate, its own review budget —
a rewrite is unreviewed code written here rather than unreviewed code copied here, and this
component's review debt counts both — and an exclusion list published on the day it lands, because
**a rewritten library is smaller than a copied one and the difference is a support claim**.

**Authority and date.** [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md), 2026-08-31,
taken before the milestone starts, which is where the stop condition requires it to happen.

### JSC-18

**Where:** gates [section 17](roadmap.gates.md#17-measurement-discipline).

**What the plan said.** Among the two things this component adds to the core's measurement rules:
"a **declared repetition count**, fixed at JS-1 and published with every bundle, because 'retained
repetitions' is a release gate nobody can fail without a number".

**What was actually true.** JS-1 fixed no repetition count and no bundle publishes one. The
sentence read as a settled fact about a number that does not exist — the exact failure mode the
sentence itself names.

**What replaced it.** The obligation stands and its owner moves to the milestone that first
produces a figure — JS-4, whose gate demands a retained figure per value kind under section 17's
rules by name. Those rules bind the first bundle that publishes a figure rather than describing a
discipline already in force, and the ledger is where a reader learns whether any bundle yet carries
one.

**Authority and date.** Bundle JS-1-001 and ledger section 2, read against the sentence,
2026-08-31.

### JSC-19

**Where:** gates [section 23](roadmap.gates.md#23-risks-and-stop-conditions).

**What the plan said.** Three risk rows stood in one undifferentiated state, each read as a risk
still wholly ahead of the component.

**What replaced it.** Each of the three now says which way it moved, because a risk that has been
*narrowed* and one whose stop condition has *fired* are not the same state and a register that
records them alike teaches nothing.

**Narrowed — the two-profile declaration row.** The maxima half is retired with the core's clamp
([JSC-01](#jsc-01)). What remains is the defaults half, which is real: a neighbour's zero default
reaches this profile wherever a host adopts rather than states, and the reconciliation belongs to
whichever component composes both and has no owner.

**Fired — the value-representation row.** Its stop condition read: "if the answer is replace, JS-6
is re-scoped from a copy to a rewrite before it starts, not during it." The answer is replace and
the re-scope happened before the milestone started ([JSC-17](#jsc-17)). **A stop condition that
fires and is honoured is the mechanism working**, and the row stays in the table with its outcome
recorded, because a risk register that deletes the rows that fired teaches nothing.

**Discharged in part — the stall row.** The waited-on set is itemised per open item with a stated
reason, and a snapshot-as-is date and commit-count budget are recorded with a named owner
([JSC-03](#jsc-03), [JSC-04](#jsc-04)). What is **not** discharged is the row's other half, the
core-acceptance blocker, which the ledger carries with its holder and its unblock condition — lack
of scheduling is still not a blocker while an unaccepted contract is.

**Authority and date.** ADR 0007 and the core's clamp removal;
[JSD-0011](decisions/0011-the-value-frame-and-call-abi.md);
[JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md). All 2026-08-31.

### JSC-20

**Where:** the roadmap header, delivery [section 19](roadmap.delivery.md#19-milestones)'s opening
notes, and the [ledger](roadmap.status.md)'s own header and closing summary.

**What the plan said.** The roadmap's opening paragraph carried its own inventory of what exists —
a milestone-state sentence, a count of decision records, a list of artefacts — transcribed from the
ledger into prose.

**What was actually true.** It went stale, quietly, exactly as the ledger's own update rule 10
predicts: **no count, total, graph, commit or score is copied into prose, because a number
transcribed into a sentence goes stale silently.** By the time JS-3a and JS-9 landed, the header
under-counted the decision records, omitted a milestone that had moved to `In progress`, and named
neither the frozen public-API baseline nor a single artefact JS-9 produced — while still naming the
diagnostic registry, which is what a partly-updated inventory looks like.

**What replaced it.** The header states the durable things — what this component is, that it starts
from a snapshot copy, that nothing in it is accepted, and what it does **not** contain — and points
at the ledger for everything that moves. **The ledger is cited, not summarised.**

**The same discipline now applies to the plan as to the ledger.** A roadmap is not exempt from a
rule the ledger imposes on itself; it is the document a reader is most likely to read first, and
therefore the one where a stale number does the most damage.

**Authority and date.** Ledger update rule 10, applied to the plan, 2026-08-31.

### JSC-21

**Where:** throughout, most visibly the roadmap's opening and
[section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph).

**What the plan said.** The document was written for a **component with its own repository**, and
assumed this component would own its assurance system, its architecture rule register, its public
API baseline, and its licence and notice files. Several sections described standing those up.

**What replaced it.** This profile is a **family of product projects inside the `Broiler.VM`
component**, at `src/Broiler.VM.Profile.JavaScript*`, with its roadmap and decisions in the profile
assembly's own project directory. The assurance system, the rule register and the licence and notice
files are the host component's, adopted rather than duplicated, because one repository policy
implemented twice is precisely the drift the platform's assurance policy exists to prevent; each
adoption is recorded as a dated deviation. **The fourth, the API baseline, was adopted and then had
to be granted back** — see [JSC-25](#jsc-25). What this profile stands up of its own is the part
adoption cannot supply: its evidence-bundle contract and its collection script, because a bundle
collected by the host's script would merge two ledgers.

**What is emphatically not shared: evidence.** A JS bundle is cited only by this profile's ledger, a
core bundle only by the core's, and no gate here may cite a result from another component. Where
the text says "this component", read "this profile" — the argument is unaffected, and the places
where the placement genuinely changes an answer say so.

**One cost observed while adopting.** A run of the architecture suite from profile work rewrites a
line in the core's most recent evidence bundle, because the rule that writes it reads the current
bundle from a register field naming a **core** milestone and there are now two milestone series in
one repository. It was reverted rather than committed. It is recorded as an observed wrinkle whose
fix is the core's decision, not this profile's.

**Authority and date.** [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) and
[JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md), 2026-08-31.

### JSC-22

**Where:** roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted).

**What the plan said.** "JS-1 records the list with the first format version" — the list of surfaces
the specification leaves implementation-defined, implementation-approximated or host-defined, each
marked fixed or varying, so that no corpus entry is ever written over a legitimately varying answer.

**What was actually true.** **No list was published, and yet two of the five surfaces were
answered.** The slice reaches two of them and settles both in its own product code: the contents and
format of an error message are **declared varying and excluded from the retained corpus by name**,
so a corpus entry pins the error kind and never the string; and number-to-string at the edges is
**fixed**, by a rendering the profile owns for its own evidence and which no caller may present as
what a JavaScript program would print. The other three — property enumeration order,
locale-sensitive behaviour, and anything the host supplies — the slice cannot reach at all.

So the obligation was met in code and not as a document, which is the worst of both: a reader
looking for the list finds nothing, and a reader who finds the two answers cannot tell whether they
are the whole of the list or the part somebody happened to write down.

**What replaced it.** The list is **per manifest**, and a manifest's entry is published with it. The
slice's entry has two rows, one varying and one fixed, and is silent on the three it cannot reach;
`broiler.javascript.core` is where the other three arrive. Nothing about the obligation is weakened
— what changed is that it is discharged per manifest, in the document rather than only in a remark.

**Authority and date.** The shipped slice surface — the fault's message text and the value's
diagnostic rendering, each of which names the section-6 obligation in its own remark — read against
[JSD-0002](decisions/0002-feature-manifest-allocation.md)'s obligation, 2026-08-31. **JSD-0002 puts
the list at JS-1 and no record re-homes it**, so the milestone that mints the next manifest owns
confirming this reading with a dated record of its own.

### JSC-23

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones) and
[section 20](roadmap.delivery.md#20-delivery-order).

**What the plan said.** The delivery order drew JS-9 as a milestone after JS-8, with one edge and
one position. Read literally, its adversarial-input, soak and aggregate-budget work waits on the
seven milestones before it.

**What replaced it.** JS-9 **opens against JS-1 and closes after JS-8**. The retained corpus grows
from the first product code onward, and the soak and the shared-aggregate-budget exercises need
nothing a later milestone delivers. What holds the milestone open is not the work but the gate: two
of the four untrusted-input surfaces it must fuzz — the source parser and the regular-expression
matcher — do not exist until JS-3b and JS-6, and a session over surfaces that do not exist may not
be read as covering them.

**What this changes about reading the order.** The diagram gained a third arrow kind. **A milestone
whose gate closes late is not a milestone that starts late**, and drawing only the closing edge is
how a schedulable milestone gets read as blocked — which is the same defect the JS-3 split
corrected ([JSC-15](#jsc-15)), in a milestone nobody had looked at twice.

**Authority and date.** The ledger's JS-9 row, read against the delivery order's single closing
edge, 2026-08-31.

### JSC-24

**Where:** roadmap [section 4.3](roadmap.md#43-the-copy-table) and
[section 4.4](roadmap.md#44-what-the-copy-actually-costs).

**What the plan said.** The copy table's standard-library row read "**Copy, or port** — whether it
is a copy or a port is decided by the value-representation decision", with three neighbouring rows
resting on the same unresolved condition: the optional surfaces were "**Copy** behind separate
manifests", the re-homed prototype patching was to be "copied or ported with the library", and the
test corpus was to be copied "as a port wherever the value model changed".

**What replaced it.** The condition is discharged and all four rows say so. The library's core
surface is **rewritten**; the optional surfaces are **rewritten too**, because they are the same
assembly typed against the same value base type and nothing about being behind a later manifest
makes them copyable; the prototype patching is **re-implemented with the library**; and the test
corpus splits — the storage half is copied, the library half is ported. The conditional was correct
while the decision was open, and a conditional left standing after its condition resolves is how a
reader ends up planning for the branch that did not happen.

**One thing this entry decides that no record before it did.** [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md)
scopes itself to JS-6 and says it "touches nothing else in the copy table". The optional surfaces
are not JS-6's, so the record left them alone — but the reason it gives applies to them word for
word, and a table whose verdict column reads *copy* for half the library it cannot copy is worse
than a table that draws the obvious conclusion. **The milestone that mints each of those manifests
owns confirming it**, and may record a different answer with a dated record of its own.

**Authority and date.** [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md), 2026-08-31. The
milestone-level consequence is [JSC-17](#jsc-17).

### JSC-25

**Where:** the roadmap header, and the ledger's placement paragraph.

**What the plan said.** Four mechanisms this profile assumed it would own are the host
component's, adopted rather than duplicated: the assurance system, the architecture rule register,
**the public API baseline**, and the licence and notice files.

**What replaced it.** Three of the four are the host's and stay so. The fourth is not, and the
reason is structural rather than preferential: the host's describer describes a surface by
**loading** an assembly, which needs a project reference that the architecture rules forbid a test
project to hold on a profile. This family therefore has a baseline file and a rule of its own,
which describes the surface from the build output without running anything.

**Why it is filed separately from [JSC-21](#jsc-21).** That entry records the placement and what
adoption costs; this one records the single mechanism where adoption did not hold, because a
reader who takes "four things are the host's" as current would look for this family's frozen
surface in the wrong file.

**Authority and date.** [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md),
2026-08-31; the clause's journey is [JSC-16](#jsc-16).

### JSC-26

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the list of what
format version 1 carries.

**What the plan said.** Eight things, among them "a constant pool with load-time property-name
interning, so a name is interned once per program rather than at each use", and exception regions
carried while suspension targets were separately described as merely *reserved*.

**What replaced it.** In three parts.

**A section was missing.** Version 1 frames a section of **named program entries and their code
offsets**, which is the whole of the entry-point answer ([JSC-11](#jsc-11)) and the reason an
invocation carrying one UTF-8 name resolves to anything. A format list that omits the section
holding the answer to another chapter's central problem is a list a reader cannot check the format
against.

**An interning was claimed rather than reserved.** Version 1 carries an interned-name *tag*
admitted by no manifest; nothing interns anything, because the first manifest has no strings and no
property access, and an artifact carrying the tag is refused. The plan described the eventual
behaviour in the present tense.

**Two sections were described as being in different states when they are in the same one.**
Exception regions and suspension targets are both framed, both parsed, both admitted by no
manifest, and a non-zero count of either is refused with its own diagnostic. Reserving them is the
same act for the same reason — a section added to a frozen format is a format-version break — and
splitting the description invited a reader to think one was live.

**Authority and date.** [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md)
and the shipped format, 2026-08-31.

### JSC-27

**Where:** roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model), the representation and
frame rows, and the `CallDepth` subsection.

**What the plan said.** Two candidate representations: the seed's boxed hierarchy, and the seed's
unused **eight-byte** tagged struct — "either answer is defensible". And the `CallDepth` default
derived from a measurement of **native frame cost** per interpreter frame.

**What replaced it.** Neither candidate. The decision is a tagged struct **of this profile's own**,
with an explicit kind, a `double` payload and a *managed-reference* payload — wider than either
candidate, and chosen because a moving collector may not have a reference hidden inside a payload
word, so a compact packing would buy back the allocation the representation exists to avoid. The
compact packings are registered as candidates against a measurement, on the plan's own rule that **a
representation is not accepted because it looks compact**.

**And the frame row removes the native stack from the question.** An interpreter frame is a **heap
object owned by the operation** and the dispatch loop is one CLR frame regardless of guest depth, so
guest recursion grows a list rather than the CLR stack. `CallDepth` is therefore **a counted number
compared against a bound**, not a stack probe — which is what makes "refused rather than terminating
the process, under Native AOT" a promise this profile can keep at all. The default is still measured
and still unchosen; what changed is what is being measured.

**Authority and date.** [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) rows 1 and 4,
2026-08-31.

### JSC-28

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the
verification-boundary subsection.

**What the plan said.** "**Half** of this component's published registry therefore has a transport
that is the embedder's own seam rather than a core result" — stated as a property of the registry,
in a list of three consequences none of which was settled.

**What replaced it.** The registry publishes a transport column and **every row in it is
`core-result`**. No embedder-seam code exists, because the front end that would mint one is JS-3b's.
The plan's *half* was a projection of a finished profile stated as a fact about this one, and the
difference matters to anyone reading the registry: an empty half is a milestone boundary, not an
oversight.

**And one of the three consequences is now settled**, which the plan did not distinguish: which
half a code belongs to is decided, published and bound by rules. Two remain open and are JS-3b's —
whether the verifier re-derives every early error from artifact bytes, and what a doubly-bad
artifact answers.

**One clause was also quoted that never existed.** The plan warned against a gate asserting that
every early error "maps onto exactly one core **invalid-artifact** reason". No gate says that; the
clause that exists, and the rule that enforces it, say **one core reason** — which is the wording
that survives an embedder-seam code, and was already the wording before the warning was written.

**Authority and date.** [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md)
and the published registry, 2026-08-31.

### JSC-29

**Where:** roadmap [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core),
the counterweight column.

**What the plan said.** The section promised that "the rest name the other profile's position where
it is known". Three of the ten rows named it; **seven did not**, and on four of those seven the
other profile's position was recorded, dated, and contrary.

**What replaced it.** Every row names it. Four rows change character. Two are the counterweight test
**failing outright** — a need that looks general and is one language's shape — and two are weaker
than that and corrected for their own reasons:

- **An in-process producer input form.** The other profile records *not needed, and would not
  co-sign it*: every byte it runs arrives from outside the trust boundary, so serialization is its
  input rather than a critical-path cost.
- **Lazy per-section verification.** *Not needed, and actively declined* — its own specification
  offers the permission and its validation chapter refuses it, because a deferred check is a check
  reported as a trap.
- **Nested instantiation through the mediator.** *Not needed* — a plain absence of need rather than
  a refusal: its language has no instruction that asks for code while running.
- **Streaming or incremental verification.** *Wanted eventually, needed by nobody yet* — not a
  decline, and not a reason to move the grade: both profiles want it, which is the counterweight
  test passing, and neither has the measurement that would open it.

**Why this is a correction and not a re-grading.** **All ten grades are unchanged**; what changed
is that a column promising the other profile's position now carries it, and a counterweight column
that silently omits a decline is the failure mode the counterweight exists to prevent. Streaming
keeps its *strong: general* standing from
[JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md), and what the row gains
is the other profile's urgency — wanted, and needed by nobody yet — which bears on when it is
filed and not on how general it is.

**Authority and date.** The other intended profile's own roadmap, read against this table,
2026-08-31.

### JSC-30

**Where:** roadmap [section 4.5](roadmap.md#45-licence-attribution-and-one-notice-that-must-change)
and [section 14](roadmap.md#14-the-conformance-oracle); delivery
[section 19](roadmap.delivery.md#19-milestones), JS-3a; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map).

**What the plan said.** One licence obligation, argued in section 4.5 and landed by JS-2 —
the seed is Apache-2.0, a copy carries the notices forward, and JS-2's gate closes it in the same
change that introduces the copied tree. [Release gate 12](roadmap.gates.md#22-release-gates) and
the evidence matrix additionally demanded **the ingested conformance suite's own attribution**, and
nothing in the plan said where that one came from: no chapter argued for it, JS-3a ingests the suite
and its gate was silent about it, and the map handed the whole *Licence and attribution* evidence
area to chapter 4 and JS-2.

**What replaced it.** Two obligations, named apart and landed apart. Section 4.5 stays the seed's
and says so. The suite's is section 14's, and **JS-3a lands it**, in the same change that first
ingests a suite file — with modified files marked and the core's standing third-party claim
re-confirmed against what the ingestion adds to the tree, or amended, the release owner
co-signing. The map's chapter-14 row carries that evidence
area and gate 12 beside its conformance cells.

**Why it could not be landed earlier, which is the whole reason it moved rather than being added to
JS-2.** What a notice carries forward is the ingested material's own content, and that content is
not in this checkout until the suite revision is retrieved, hashed and archived — the human action
the ledger records as unperformed and the same one JS-3a's pin waits on. A row written at JS-0 or
JS-2 would be an attribution for material nobody has read, which is the failure gate 12 exists to
prevent rather than an early discharge of it.

**Authority and date.** [Release gate 12](roadmap.gates.md#22-release-gates) and the *Licence and
attribution* row of [section 21](roadmap.gates.md#21-test-and-evidence-matrix), read against
[section 3 of the ledger](roadmap.status.md#3-open-external-dependencies), whose conformance-suite
row records the revision unpinned and its licence and attribution obligations unexamined;
2026-09-01.

### JSC-31

**Where:** roadmap [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core);
delivery [section 19](roadmap.delivery.md#19-milestones), JS-10; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map);
[gates section 22](roadmap.gates.md#22-release-gates), gate 1.

**What the plan said.** Every row filed and held, none admissible until it names a merged or
approved capability, and the procedure unexecutable while one person holds the minting role and
both co-signing roles. The map recorded the consequence as a deliberate blank: section 18 *closes
no gate and appears in no evidence area*.

**What replaced it.** The blank was right about the **answers** and wrong about the **state**. No
milestone can schedule an amendment nobody can mint, and none is asked to. But every held row is a
capability this profile does not provide, and gate 1 already refuses a support table that leaves an
unimplemented capability unnamed — so **JS-10 publishes the register's state**: per row, whether it
is filed, held or opened, the deterministic failure or named exclusion it leaves standing, and that
the procedure is unexecutable and why. The gate is over the publication and never over the answer,
so a release that names every row and moves none of them passes it.

**What this does not change.** No row becomes admissible because a release names it, no grade
moves, and no counterweight position is re-read. The amendment procedure is exactly as unexecutable
after this correction as before it; what changed is that a reader meets the consequence in the
support table instead of in a refusal.

**Authority and date.** [Gate 1](roadmap.gates.md#22-release-gates), which admits no unnamed
unimplemented capability, read against section 18's own record that the procedure is unexecutable;
2026-09-01.

### JSC-32

**Where:** [gates section 23](roadmap.gates.md#23-risks-and-stop-conditions), the extraction-gate
row; delivery [section 19](roadmap.delivery.md#19-milestones), JS-10; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map), the chapter-5 row.

**What the plan said.** This profile supplies its half of the comparison when a second product
profile's implementation has merged, *recorded at the milestone that owns the mechanism*; where the
second implementation has not merged, it records the first condition unsatisfied. The stop condition
was that an unrecorded state is a failure.

**What replaced it.** The same obligation, plus a milestone that can always discharge it: recorded
at the mechanism-owning milestone **where the condition is already met, and at JS-10 regardless**.

**Why the earlier reading could not close.** Whether a second implementation has merged is a
schedule this component does not hold and may never see. No mechanism-owning milestone's gate
carried the clause, and none could be written to, because such a milestone can pass at a moment
when there is nothing to compare against and nothing to record. A stop condition over an unrecorded
state needs one point at which the state must exist whichever way it fell, and the release is that
point.

**What is unchanged.** No verdict is recorded, then or ever — a verdict changes the core graph and
is the core architecture owner's — and no identifier from another profile component appears in this
component's documents, which JS-10's gate asserts by scan rather than by intention.

**Authority and date.** [Section 23](roadmap.gates.md#23-risks-and-stop-conditions)'s own stop
condition, read against [section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map)'s
rule that a blank delivering cell is a finding; 2026-09-01.

### JSC-33

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-10; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map), the chapter-3 row;
[gates section 22](roadmap.gates.md#22-release-gates), gate 1.

**What the plan said.** The fifteen defaults are published with the maxima, and the
*Composed-profile safety* evidence row records the defaults as the neighbour-facing half.
Reconciling two profiles' declarations belongs to whichever component composes both. Nothing
required the **support table** to say either thing, so a reader of the published table met a default
vector with no statement about who reconciles it.

**What replaced it.** JS-10 publishes the vector as the neighbour-facing half **with its
reconciliation named as unowned**, and gate 1 refuses a table that does not. The reconciliation
itself is not deferred to the release and is not this component's to take at any milestone: the
component that would own it does not exist.

**Authority and date.** [Section 3 of the ledger](roadmap.status.md#3-open-external-dependencies),
whose declared-defaults row records the reconciliation as still unowned, read against
[gate 1](roadmap.gates.md#22-release-gates)'s rule that no row reads as a bare yes; 2026-09-01.

### JSC-34

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph), the
test-only row and the new subsection beside it; gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), the dependency-architecture row.

**What the plan said.** That the test-only row holds "conformance host, fuzz host, soak host, bench
host", each "never referenced by a product project and never present in a published closure", with
exactly one carve-out: the retained corpus, which a composition root writes because a corpus a test
project produced would be a corpus the product path never exercised.

**What replaced it.** The carve-out is larger than one row. **The fuzz mutator, the soak over
recycled runtimes, and the shared-aggregate-budget exercises are in a composition root too**, for a
reason the corpus argument does not cover and that the rules settle rather than taste: each drives
this profile's own verifier and executor, a test project may not reference a profile assembly, and
a composition root may not reference the core's fixture assembly. There is nowhere else for them to
be, and the implementation already reasons this way in its own source.

**Why the earlier reading matters enough to record.** A reader of the old row would have expected a
`Broiler.VM.JavaScript.Fuzz.Host` under `src/tests/` and would have found none — and, more
consequentially, would have read
[section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding)'s
`execution-only` row as a file inventory. It is not. The execution-only image publishes a closure
that contains a mutator and a soak driver, and **the closure gate's "no test assembly" clause is
satisfied by the assembly boundary while the property is weaker than it reads.** Section 5 now says
so where a reader meets it, rather than leaving it to be inferred from a closure report that counts
assemblies.

**What it does not change.** No test *assembly* is in any closure, no advertised composition
exists, and nothing about the reference sets moved. What moved is the plan's account of what a
composition root contains, and the register now records it.

**Authority and date.** The shipped composition roots, read against the host component's active
rules on where a profile assembly may be referenced from and what a composition root may reference;
2026-09-01.

### JSC-35

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the
resource-exhaustion bullet of the verifier's rejection list; gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), the format-and-verifier row.

**What the plan said.** That the verifier answers `ResourceExhaustion` naming structural depth,
section count, declared counts or artifact bytes, each with one dimension and one scope — with no
statement of what binds any of the four to a case.

**What replaced it.** **Each of the four gets a corpus entry of its own**, and the bullet now says
why nothing else would serve. Every invalid-artifact arm is held to a named case by the diagnostic
registry, bound in both directions by rules the register carries. **An exhaustion answer carries no
profile diagnostic code**, so the registry does not reach this bullet at all — which makes the
four dimensions the one part of the rejection list whose category rests on prose. That is precisely
where [section 21](roadmap.gates.md#21-test-and-evidence-matrix) already names *a ceiling breach
recorded as an invalid artifact* as a release blocker, so the gap was between two clauses that each
assumed the other covered it.

**What this is not.** It is not a claim that the arms are wrong: the verifier maps every one of the
core's bounded-read statuses onto its correct category today. It is a claim that one of the four is
reached by a retained entry, one is reached incidentally by an ordering check, one is reached only
inside a fuzz session, and one is reached by nothing — and that a reader of the plan could not
have learned any of that from the plan.

**Authority and date.** The retained corpus manifest read against roadmap section 7's own list, and
against rule N7's both-directions binding, which by construction covers only coded rejections;
2026-09-01.

### JSC-36

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), the exit gates of JS-3a, JS-5
and JS-10; delivery [section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map).

**What the plan said.** Four things, each stated somewhere and gated nowhere:

- [Section 24](roadmap.gates.md#24-specification-and-platform-references) and the ledger both put
  the **language-specification pin** on JS-3a. JS-3a's exit gate named only the suite revision.
- [Gate 1](roadmap.gates.md#22-release-gates) requires the support table to carry the **pinned
  specification edition and suite revision** and **the list of surfaces this profile declares
  varying rather than fixed**. JS-10's exit gate carried neither.
- [Gate 13](roadmap.gates.md#22-release-gates) requires the **operational holders** to be named,
  and the map's own closing bullet says "JS-10 names them or the gate refuses". JS-10's exit gate
  did not mention them.
- JS-5's gate required "a proportionality fixture for each named operation family of section 8" —
  a list that includes regular-expression matching, which arrives with the matcher at JS-6, and the
  string and array families, which arrive with the library.

**What replaced it.** Each clause is now written into the exit gate that would have to fail without
it, and JS-5's is scoped to the families its own increment ships.

**Why these are one entry rather than four.** They are one defect with four instances, and naming
it is worth more than naming them: **a clause stated in the argument or in the release gates,
agreed by the map, and never written into an exit gate.** A release gate cannot refuse a tree at a
milestone that has already closed, and the map records who *should* close a clause rather than
testing that anyone can — so a map read in one direction confirms coverage that does not exist.
The map's closing bullets now carry the pattern, not only the four instances.

**What it does not change.** No release gate moved, no clause is new to the programme, and nothing
here advances or narrows a milestone: every one of the four was already required of a release.

**Authority and date.** [Gates section 22](roadmap.gates.md#22-release-gates) gates 1 and 13, and
[section 24](roadmap.gates.md#24-specification-and-platform-references), read against the exit
gates of JS-3a, JS-5 and JS-10 and against the map's blank-cell rule; 2026-09-01.

### JSC-37

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-9's exit gate; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map), the closing bullets.

**What the plan said.** That JS-9 closes when the full corpus replays to its recorded triples on
three publish modes, a mutated entry is detected, the fuzz sessions retain what they must, the soak
reaches a plateau, and the aggregate-budget exercises hold. **Nothing in that gate mentioned the
four exhaustion dimensions**, while [JSC-35](#jsc-35) had already written them into roadmap
[section 7](roadmap.md#7-the-bytecode-format-and-the-verifier) and into gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), and the map's chapter-7 row had already
named JS-9 as owing them.

**What replaced it.** JS-9's exit gate carries the clause: an entry per dimension, recording the
dimension and the scope, with a manifest column for the pair — and, where a dimension is
unreachable at the manifest the milestone closes against, the bundle names it and the milestone that
makes it reachable rather than passing over it.

**Why this one is worth its own entry rather than a line in [JSC-36](#jsc-36).** It is the same
defect, but it arrived **by a different route and after the sweep that was supposed to find them
all**. JSC-36's four were clauses the release gates had always asked for and no exit gate had ever
carried. This one was *created* on the same day, by a correction that added a requirement to the
argument and to the evidence matrix, agreed it in the map, and stopped there. **A correction is an
edit to the plan and is therefore subject to the plan's own failure modes**, which the sweep did not
allow for: it read the gates against the milestones, and the thing that had just moved was neither.
The pattern the map now carries is the general one — a requirement is not in the programme until an
exit gate would fail without it, whatever introduced it.

**What it does not change.** No release gate moved, the requirement is JSC-35's rather than new, and
nothing here advances or narrows a milestone. What the corpus actually reaches today — one dimension
by an entry, one by an ordering assertion, two by nothing, and no manifest column for the dimension
and the scope — is a fact about the checkout and is recorded in the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row, not here.

**Authority and date.** [JSC-35](#jsc-35) and gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), read against JS-9's exit gate and the
map's blank-cell rule; 2026-09-01.

### JSC-38

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row; delivery
[section 19](roadmap.delivery.md#19-milestones), JS-9's exit gate; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map); the header of bundle
[JS-9-001](evidence/js-9/README.md) and the fuzz-log header the collection script writes.

**What the plan said.** Nothing wrong: [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s
second discipline and [section 21](roadmap.gates.md#21-test-and-evidence-matrix) both ask for
*coverage-guided* fuzzing. What was wrong is what the ledger and the bundle said the plan had been
shown to hold. The JS-9 row called the retained target "a coverage-guided fuzz target", bundle
JS-9-001's header said the same, and the collection script wrote it into the head of every fuzz
log. And JS-9's exit gate asked a session to retain its seed, its iteration budget, its settings and
its counterexamples — and to be guided by nothing in particular.

**What was actually true.** The target is seeded mutation. It draws every mutant from the fixed
retained corpus, perturbs it by one of ten operators, and takes no feedback from what the mutant
reached: no instrumentation, no coverage signal, and no seed added for new behaviour. The only
feedback anywhere in a session is the outcome histogram it prints at the end, which decides whether
the session exercised more than one path and nothing else. That is a fuzzer worth having — the
operand-targeting mutation finds a class of defect the corpus cannot — and it is not the one the
section names.

**What replaced it.** The ledger says seeded mutation and names the guidance as an open clause of
JS-9; the bundle's header carries a dated correction and no retained log was edited; the script's
header says what the sessions are. And JS-9's exit gate now carries the guidance — a session
observes what a mutant reached and keeps the mutants that reached something new as further seeds —
so that a session over a fixed seed set cannot close the milestone. That last part is
[JSC-36](#jsc-36)'s shape from the other side: the adjective was in the argument and in the
evidence matrix, agreed by the map, and never reached the gate's clause, so the gate was
satisfiable by the thing the argument excludes.

**What it does not change.** Nothing about the sessions moves: the seeds, the iteration counts, the
histogram and the one fuzz control are what they were, and the corpus-integrity mutation and the
soak are untouched. What moved is the name the ledger gave the sessions, which is update rule 2's
subject — *state what it demonstrated* — and update rule 4's: a result promoted beyond what it
proves.

**Authority and date.** The fuzz target's own source, read against section 7's second discipline
and JS-9's exit gate; ledger update rules 2 and 4; 2026-09-01.

### JSC-39

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the
resource-exhaustion bullet of the verifier's rejection list; gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), the format-and-verifier row; delivery
[section 19](roadmap.delivery.md#19-milestones), JS-9's exit gate; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row.

**What the plan said.** That the verifier answers `ResourceExhaustion` on four dimensions —
structural depth, section count, declared counts and artifact bytes — and, since
[JSC-35](#jsc-35), that each of the four gets a corpus entry because the registry's binding cannot
reach an exhaustion answer.

**What was actually true.** The verifier as built names seven. The four are the bounded reader's
ceilings, answered through its statuses. The bounded allocator refuses an allocation and the
verifier answers `AllocatedBytes` — in the constant pool, in the code section, and in the three
arrays the link stage allocates over the code; the work charge the link stage makes over the code
is refused and the verifier answers `VerifierWork`; and a poll that stops for a budget rather than
for a token is answered as `WallClock`. [Section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses)'s
matrix already declared all three charged at verification. JSC-35's argument — an exhaustion answer
carries no diagnostic code, so nothing but a corpus entry binds its category — applies to the
allocator's arm, the work charge's arm and the poll's arm exactly as it applies to the reader's,
and the plan bound four arms of seven.

**What the checkout reaches today**, stated once here because [JSC-35](#jsc-35) and
[JSC-37](#jsc-37) counted the same four two different ways — *one inside a fuzz session and one by
nothing* against *two by nothing* — and both were reading the fuzz session's tight vector, which
provokes a dimension the session never records: section count by an entry; artifact bytes by an
ordering assertion; declared counts and allocated bytes only inside a fuzz session, whose histogram
records the category and never the dimension; and structural depth, verifier work and the wall
clock by nothing. The ledger's JS-9 row carries that per dimension from now on.

**What replaced it.** Section 7 names the seven and their three sources; section 21 and JS-9's exit
gate hold every one of them to an entry; the ledger's JS-9 row states what reaches each.

**What it does not change.** No arm is wrong: every one of the seven maps its status onto the
exhaustion category and not the invalid-artifact one, and the reader's mapping is one switch. What
was unbound is the category of three arms nothing in the retained evidence provokes, which is the
same defect JSC-35 found, three arms wider.

**Authority and date.** The verifier's own source, read against section 7's bullet and the corpus
manifest; rule N7's binding, which by construction covers only coded rejections; 2026-09-01.

### JSC-40

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph) and
[section 14](roadmap.md#14-the-conformance-oracle); delivery
[section 19](roadmap.delivery.md#19-milestones), JS-3a's exit gate; delivery
[section 25](roadmap.delivery.md#25-the-chapter-milestone-and-gate-map); gates
[section 21](roadmap.gates.md#21-test-and-evidence-matrix), the conformance row.

**What the plan said.** That the ingestion path "ships nowhere", asserted by a scan that the suite
harness appears in no product package "and in no published closure"; and JS-3a's gate asked the
same of the harness, its cache and every suite file — "no closure report" — with a negative
control adding "a product reference" to the ingestion path. Nothing said where the harness lives.

**What replaced it.** The harness is a composition root that is never advertised, and the scan is
over every package and every *advertised* composition's closure, with the control injected from the
execution-only root. The reason is the one section 5 already gives for the corpus writer, the fuzz
mutator and the soak, and it is the rule rather than a preference: the harness lowers suite
sources, verifies the artifacts and runs them, so it drives this profile's own lowering, verifier
and executor, and rule A11 forbids a test project to reference any of the three. A root publishes a
closure of its own, so a scan over "no published closure" would fail on the very root that has to
contain the harness — and would then be relaxed into meaninglessness by whoever met it.
[JSC-34](#jsc-34) drew this consequence for everything that produces evidence except the one thing
that produces the most of it, and the other intended profile reached the same placement from the
same rule.

**What it does not change.** No shipped image contains a harness, a suite cache or a suite file,
and no advertised composition ever may. What moved is the boundary that carries the property —
from *project kind* to *advertised or not*, which the composition register already records — and
the direction of the control, which now injects the reference that would actually ship.

**Authority and date.** Rule A11's registered statement and its path-keyed allow-list, read against
section 14's bullet and JS-3a's gate; the composition register's advertised set, which is empty;
2026-09-01.

---

## 3. Rejections

An option considered and refused is not a correction: nothing in the plan changed, and the record
of the refusal is what makes the choice reviewable. **This section is an index, not a second copy.**
Each row names where the argument lives; the reason column is a summary and the record is the
authority.

**Rejected by a decision record.**

| What was refused | Why, in one line | Record |
|---|---|---|
| Adopting the seed's boxed value hierarchy | It would move the rewrite onto JS-1's executor rather than avoid one | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| A mechanical re-typing of the standard library | The library pattern-matches on concrete value subclasses; a re-typing is a rewrite wearing a copy's schedule | [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) |
| Encoding a call into the entry-point text | It makes the entry-point name a parsed surface with its own grammar and early errors | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) |
| One binary with two composition modes | A closure report cannot see a flag | [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) |
| Generating the diagnostic registry from the source | The forward binding becomes a tautology and the third artefact the rules need disappears | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| Retiring the three diagnostic codes no artifact reaches | Deleting a defensive arm costs a wrong answer at the moment the answer matters | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| Stating the position encoding in the roadmap rather than in code | A paragraph would have satisfied the sentence and missed the conflation, which was in the code | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| Writing a position on every corpus row | Hand-computed offsets no reader can check, or the producer asking the verifier — recording the answer under test | [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) |
| A second assurance system and a second rule register for this profile | One repository policy implemented twice is the drift the policy names as the thing to avoid. The API baseline was refused on the same ground and later had to be granted anyway, for a structural reason the refusal did not foresee — see [JSC-25](#jsc-25) | [JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md) |
| A group of **review-document** rules of this profile's own | What the profile needed was its own **legend**, and a vocabulary is data the rules read. This is not the profile's group in the architecture register, which exists and holds its graph and registry rules | [JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md) |
| Excluding this profile's documents from the review rules | Three of the five rules pass over them unchanged, so they are the same genre with a different vocabulary | [JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md) |
| Leaving the API-baseline clause at JS-3b as "schedulable" | A clause nobody can schedule and a clause nobody has scheduled read the same in a table | [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) |
| A composition root that prints its own public surface | It puts a reflection host in a product assembly, which is what the closure reports exist to keep out | [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) |
| A fourth deployment composition label | Three labels describe *when* source is compiled, which is the only axis a label carries | [JSD-0003](decisions/0003-deployment-composition-labels.md) |
| A cross-profile value channel in the core | A shared mutable region is shared semantics by another name; this profile co-signs the refusal | [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md) |
| A profile veto over a core amendment | It would establish a profile-to-profile dependency by governance rather than by reference | [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md) |

**Refused by the plan itself, and still refused.** These are not history: they are live constraints
and they stay in the roadmap where a reader meets them.
[Section 1's non-goals](roadmap.md#non-goals) refuse a second execution arm, a second verifier, a
second lowering, a security-sandbox claim, CLR interop, a debug wire protocol, ownership of the
filesystem, network or module map, a change to the core, and any performance claim about another
engine. [Section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) refuses any
design that can only be hosted by a second core state machine. They are listed here so that this
file is a complete answer to "what has this programme refused", and **not** so that anyone reads
them as settled history.

---

## 4. Hazards and deviations a reader will meet

Notes addressed to a reader rather than to the plan. They belong out of the roadmap because none
of them changes what is built; each exists because someone will otherwise be misled.

**A competing, superseded plan exists in the seed's own documents.** The seed component's documents
still contain a substantial amount of stale prose describing a **retired** plan to build a
JavaScript bytecode profile inside that component: sequencing rows, dependency bullets, and
rationale text. The plan documents themselves were deleted; the prose was not. A reader who goes
looking will find it. **The roadmap in this directory is the plan.** Nothing in that component
plans, schedules or gates anything here, and no item identifier from it appears anywhere in this
profile's documents — in either direction, deliberately, because a roadmap that cited its origin's
plans would re-create the dependency the fork exists to avoid.

**Two mark legends exist in one repository, and rule H1's own words for them are different
vocabularies about different subjects.** The component's own legend has nine marks in two
vocabularies — four evidence verdicts about a piece of evidence, and five review verdicts about a
gate clause in a bundle. This profile's ledger publishes three, and they are one evidence verdict
about a whole milestone row. So a granularity reading covers at most four of the nine and misses
the five outright, while the subject reading covers all of them. Both legends do
contain evidence verdicts, which is exactly why a mark from one used in the other's
documents is a rule violation rather than a harmless synonym — and why the rule decides which
legend governs a document **by path and not by content**: deciding from what a document contains
would let a document choose its own vocabulary by using it.

**A suite total collected before JS-3a is a total over a different corpus.** The architecture suite
now reads more documents than it did. The two bundles collected before that change are unaffected
in what they demonstrate — no rule either cites changed — but each one's suite log is a run of a
suite that did not read this profile's ledger. The ledger records this rather than leaving a reader
to infer it.

**Two review clauses do not reach this profile's bundles**, and both are stated with the condition
that would close them and an owner: the requirement that a bundle carry a single structured
exclusion table, which a profile bundle satisfies as a numbered prose list instead; and the
comparison of quoted headline figures against retained logs, which cannot source a profile bundle's
differently-named, differently-localised logs. The second is a stated limit rather than a shrug,
because the fallback was not harmless: a profile bundle would otherwise have been compared against
the component's own logs, a comparison that passes today only by coincidence.

**Collecting profile evidence touches a core bundle.** See [JSC-21](#jsc-21). A collector reverts
what their run touched outside their own bundle until the core decides the fix.

---

## 5. Open, and therefore not corrected

A reader using this file to tell settled from unsettled needs the other half of the answer. These
are carried by the plan as **questions**, and nothing below has been decided. **The
[ledger](roadmap.status.md) is the authority** for every one of them; this list exists only so that
an open question is not mistaken for a correction nobody wrote down.

**Where a question cannot be answered by this component at all, the plan names the point at which
its *state* is recorded, and that is not the same as answering it.** Four of the entries below are
of that kind — the amendment register, the extraction-gate comparison, the reconciliation of two
profiles' declared defaults, and the two third-party pins — and each says where the state lands.
A release that publishes a question truthfully has not closed it.

- **Where the verification boundary falls** — whether the verifier re-derives every early error
  from artifact bytes, and what a doubly-bad artifact answers. JS-3b owns it. JS-3a settled only
  the narrower question it was asked: which half of the diagnostic registry a code belongs to.
- **The direct-`eval` decision** — whether the seed's textual heuristic is replaced by a decision
  the front end records during binding analysis, or declared an intentional documented
  approximation with its deviation published. JS-8 owns it.
- **The argument-channel amendment, and every other row of section 18** — filed and held, not
  scheduled. The amendment procedure is currently unexecutable: no amendment has been minted, and
  the minting role and both co-signing roles are held by one person. **State recorded at JS-10**,
  which publishes the register row by row with the deterministic failure or exclusion each held row
  leaves standing — see [JSC-31](#jsc-31). Publishing a held row does not make it admissible.
- **The extraction-gate comparison** — its first condition is a second product profile's verifier
  having merged, which is a schedule this component does not hold. **State recorded at JS-10** as
  this profile's half or as the condition unsatisfied, with **no verdict**, which is the core
  architecture owner's — see [JSC-32](#jsc-32).
- **The language-specification edition and the conformance-suite revision** — neither is pinned.
  Retrieving, hashing and archiving third-party material is a human action nobody has performed,
  and the ledger carries each as a named exclusion. **The suite's licence and attribution obligation
  waits on the same action** and lands at JS-3a with the ingestion — see [JSC-30](#jsc-30).
- **The reconciliation of two profiles' declared defaults** — it belongs to whichever component
  composes both, and that component does not exist and has no owner. **Named as unowned in the
  support table at JS-10** — see [JSC-33](#jsc-33) — which publishes the position and reconciles
  nothing.
- **Human review** — nothing in this profile has been read by a human, and nothing that will be
  copied arrives reviewed. It gates the release rather than any development step, and
  [gate 11](roadmap.gates.md#22-release-gates) is where it bites.

---

## 6. Update rules

1. **Correct the roadmap; record the correction here.** Edit the plan to say the new thing in its
   own voice, then add an entry. A roadmap sentence that narrates its own history is the state this
   file exists to remove.
2. **One entry per changed reading, minted in order, never reused, never edited away.** A later
   change that reverses an earlier one gets a new entry naming the one it reverses.
3. **An entry names an authority outside this file** — a decision record, a core ADR, an evidence
   bundle, or a ledger row. An entry whose only support is this file is not a correction.
4. **Cite records, do not copy them.** Where a decision record already argues a rejection, this
   file names it and summarises in one line. Two copies of an argument drift on the first edit.
5. **Transcribe no moving count.** The same rule the ledger imposes on itself: a number that a
   later milestone changes — a corpus size, a rule count, a bundle count, a milestone state — is
   named by the record that holds it and never restated here. A number a dated decision fixed for
   good, and that the entry is *about*, is part of the entry.
6. **This file records no status.** It never marks a milestone, never uses the ledger's mark
   vocabulary, and never says anything is accepted, validated or supported.
7. **A pointer in the roadmap is a bare identifier.** Where a section must warn a reader that an
   earlier reading existed, it carries *(corrected: JSC-nn)* and **no account of what the earlier
   reading was** — that account is this file's, and repeating it in the plan is the inline
   correction coming back. Stating the *current* reading as forcefully as the section needs is not
   a summary; it is the plan doing its job.

### JSC-41

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the
resource-exhaustion bullet's artifact-bytes clause; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row.

**What the plan said.** That all four of the reader's ceilings — structural depth, section count,
declared counts and artifact bytes — are compared against the effective ceilings the core
materialized before the first byte was read and **answered through the bounded reader's own
statuses**. [JSC-39](#jsc-39) restated the same four the same way when it added the other three.

**What was actually true.** Three of the four are. Artifact bytes is not, and no host ceiling can
make it be: the core compares the payload length against the same effective vector the reader would
be handed, one call **before** the verifier is entered, and answers the exhaustion itself. The
verifier's two artifact-bytes arms — the bounded reader's constructor, and the code section
declaring a length past the bound — are therefore unreachable through the core's verification path.
The second is unreachable by arithmetic as well as by ordering: a section's declared length is
bounded by the bytes remaining, which is bounded by the payload length, which the core has already
compared. Both arms are **defensive**, in the sense [rule N7](roadmap.status.md#2-current-milestone-status)
already uses for a diagnostic code no artifact reaches, and the ordering assertions reach the first
of them by calling the verifier directly with bounds of their own.

**What replaced it.** Section 7 names the party that answers each of the seven rather than
attributing four to the reader. The corpus entry for artifact bytes records the dimension and the
scope the answer named, which is what its gate clause asks of it, and the ledger's JS-9 row states
that the answer is the core's — so a reader counting this profile's own exhaustion arms counts six
provoked by a host and one that is a defensive arm behind a check it does not own.

**What it does not change.** The dimension and the scope are the same pair either way, and no arm
is wrong: a verifier that trusts the core to have pre-checked would be a verifier whose safety
depends on its caller. The exhaustion bullet still names seven dimensions and the gate still asks
for seven entries. What moved is which of them is evidence about this profile's verifier and which
is evidence about the core's ordering — a distinction the entry could not carry while the plan said
the reader answered all four.

**Authority and date.** The core's verification path, read against the reader's constructor and the
code section's own bound; the retained corpus manifest, whose artifact-bytes entry is answered
before the verifier is called; 2026-09-01.

### JSC-42

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the second of
the three disciplines; gates [section 21](roadmap.gates.md#21-test-and-evidence-matrix), the
format-and-verifier row; delivery [section 19](roadmap.delivery.md#19-milestones), JS-9's exit
gate; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row.

**What the plan said.** *Coverage-guided fuzzing*, in the sense the adjective carries in the tools
it was borrowed from: a session observes which edges of the code a mutant reached and keeps the
mutants that reached new ones. [JSC-38](#jsc-38) wrote that reading into JS-9's exit gate on
2026-09-01, after finding that the retained sessions took no feedback at all and had been called
coverage-guided anyway.

**What was actually true.** The reading is not one this component can deliver, and the reason is a
rule rather than an effort estimate. A mutant's edges can only be observed by instrumenting the
code that runs it; rule A11 forbids a test project to reference a profile assembly, so the sessions
live in a composition root ([JSC-34](#jsc-34)); and instrumenting from there means either a
coverage host in a published closure — in the one image whose dependency list is evidence — or an
instrumented build of the assembly under test, which makes a session evidence about instrumented
code. Both are refused, and refused for reasons the plan states elsewhere in its own words.

**What replaced it.** A session is **guided by the answer this profile publishes**: the diagnostic
code of a refusal, which names the site that refused; the dimension of an exhaustion, which carries
no code; the step or the fault kind of a mutant that ran; the type of an exception that escaped. A
mutant whose answer no seed artifact produces is kept as a further seed, so the seed set grows with
the surface — which is the property JSC-38's clause asks for — and **two paths to one answer are
one signal**, which is the bound, stated in every session's own output and in the ledger.
[JSD-0013](decisions/0013-the-fuzz-sessions-coverage-signal.md) records the decision, the rejected
alternative and what would falsify the rejection.

**And a second reading replaced with it, which the plan never had and the implementation needed.**
A session judges its **loop** and not its **growth**. How much a seed set grows is a fact about the
corpus as much as about the mutator — a corpus that already reaches every answer the mutator can
reach makes an honest session keep nothing — so a gate clause keyed on growth would fail harder the
better the corpus got, and this component's corpus grows at every format-growing milestone. What a
session is held to instead holds whatever the corpus contains: every mutant it drew was offered to
the seed pool, and the pool keeps a new answer while refusing a repeat.

**What it does not change.** The two surfaces that do not exist are still not fuzzed and a session
still may not be read as covering them; a counterexample is still closed by a named regression and
never by an allow-list entry; and no session budget is justified by anything. **Nor does it close
JS-9's guidance clause** — the ledger is the authority for that, and closing needs a retained
bundle.

**Authority and date.** [JSD-0013](decisions/0013-the-fuzz-sessions-coverage-signal.md); rule A11's
registered statement, read against where the sessions live; 2026-09-01.

---

### JSC-43

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the static-semantics
and *what the front end is not* subsections; delivery
[section 19](roadmap.delivery.md#19-milestones), JS-3b's dependencies and its seed row; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the plan said.** JS-3b's front end is **ingested**. Its seed row read *copied and re-homed —
the post-parse validation stage and the free-name analysis*, its dependency row named *JS-2 for the
copied analysis*, and section 9 was written throughout as instructions for adapting the seed's four
early-error places into one: consolidate them, carry the facts the re-scans recover, delete the
re-scans. JS-2 is blocked on the core contract's acceptance, so JS-3b was blocked behind it and the
profile had no way to compile a line of JavaScript.

**What was actually true.** The dependency is a dependency of the *general* front end and not of a
front end at all. `broiler.javascript.slice` admits numbers, booleans, `undefined`, local bindings,
the operators the format has opcodes for, and structured control flow — a surface whose tokenizer,
parser and validation stage are about two thousand lines written from the grammar, needing nothing
from the seed and blocked by nothing. Waiting for the ingest bought this profile no code and cost
it every decision section 9 leaves open, because none of the five could be evaluated against a
front end that did not exist.

**What replaced it.** The slice front end is **written here**. It is not a subset of an ingest and
not a placeholder for one: the tokenizer, the syntax tree, the parser, the one validation stage and
the source lowering are this component's own code, and
[JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) records the five
answers section 9 asks for, each taken against something that runs. Section 9's instructions still
describe the ingest, which is still JS-2's and still blocked; what changed is that they are no
longer the only route to a front end. **Twenty-five source programs compile, verify and run to
recorded values in the retained corpus, and twenty-nine are refused by name.**

**This is the JSC-15 argument a second time, and stated as such.** That correction split JS-3 by
dependency rather than by size, because leaving the conformance harness fused put this component's
only external correctness signal behind two blockers it needed neither of. The same reading applies
here: the ingest and the slice front end were fused by the plan's sentence rather than by anything
technical, and the slice half needs neither blocker.

**What it does not change.** JS-2 is still blocked and its blocker is unmoved; the general front
end — functions, objects, strings, `try`, modules, regular expressions — is still the ingest's and
still waits; the seed row for the parts that are still copied is unchanged; and **JS-3b is not
closed by this.** Its exit gate has clauses this front end does not reach, the harness half of
JS-3a is still open, and the ledger is the authority for both.

**Authority and date.** [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md);
the reading of [JSC-15](#jsc-15) applied to the same shape; 2026-09-03.

---

### JSC-44

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the
verification-boundary subsection's first bullet; the published registry's own header;
[JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md); the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a row.

**What the plan said.** *Every row is `core-result` today — no `embedder-seam` code exists, because
the front end that would mint one is JS-3b's.* [JSC-28](#jsc-28) added that sentence so a reader
would not read the empty half as an oversight. The registry's own header carried the same, and
JSD-0009 recorded a `half` column with one of its two values declared and unused.

**What was actually true then and is not true now.** It was exactly right at JS-3a. JS-3b wrote the
front end, and a front end that refuses source refuses it with something.

**What replaced it.** The registry is at **revision 2** and the seam half is twenty-two rows —
five tokenizing, four parsing, ten static-semantic, three at the lowering's own ceilings. Four
things about the second half are decided here rather than inherited from the first:

- **Its reason column is `-` on every row, and that is not a gap.** A rejection of source reaches
  no core result, so there is no envelope for a `VmReason` to travel in; a row naming one would
  claim a transport the code does not use. Rule N6 holds it in that direction now.
- **Its stages are a separate closed vocabulary** — `tokenizer`, `parser`, `semantics`,
  `lowering` — rather than four more members of the verification stages. A row whose half and
  whose stage disagree is a rule violation rather than a plausible-looking row.
- **Its reachability is `source`**, a third kind beside `corpus` and `defensive`, and it is bound
  to a **retained source corpus** at `src/tests/corpus/js-1/source/source.manifest` — one row per
  program, read off disk by rule N7 and by nothing that produced it. That file is to this half what
  the artifact corpus is to the other one.
- **The half has no defensive row**, and refusing to give it one is the decision. All three of its
  format-ceiling codes are genuinely reachable — a program really can declare more locals than the
  frame admits — so recording them as unreachable would have been recording something untrue to
  avoid generating three sources. The generators are three lines each and the manifest says which
  entries they produce.

**And the two vocabularies are in two assemblies that cannot see each other**, which is why they
are published in one file: rule N1 keeps the profile assembly and the lowering assembly apart, so
neither compiler could notice a number used in both, and the registry plus rule N5 is the only
reader of the pair.

**One code was retired without ever being emitted.** 2211, `DuplicateBoundName`, was declared while
the vocabulary was being written and nothing ever reached it — a duplicate bound name is two rows
that already exist. It is retired rather than reused, because a code is never given a second
meaning.

**What it does not change.** The core-result half is untouched, row for row; no retained corpus
entry moved; the seam half advances no milestone, and JS-3a's oracle half is open exactly as it
was.

**Authority and date.** [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md),
decision 3; the registry at revision 2 and rules N5, N6 and N7 as registered; 2026-09-03.

---

### JSC-45

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-4's next action and its
exit gate; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-4 row; the
`SliceProgramBuilder` remark that says it is scheduled for deletion.

**What the plan said.** *Delete JS-1's hand-written encoder and lowering, and assert the deletion* —
one clause over one thing, with a named owner, on the ground that a second lowering which outlived
its milestone is a second lowering.

**What was actually true.** JS-1 built two things under one name. `SliceLowering` is a list of
**hand-written programs**: a human choosing instructions to make a point about the language, which
is exactly what a source front end replaces. `SliceProgramBuilder` is an **instruction buffer** —
constant interning, a local frame, label definition and patching, and the section framing — which
is what any lowering needs and which the source front end uses rather than duplicating.

**What replaced it.** The clause is two clauses. **The programs are still scheduled for deletion**
and the owner is unchanged: every one of them is expressible as source, and once JS-4's object
model makes the remaining ones expressible the hand-written list is a second corpus with no reason
to exist. **The builder is not deleted**, because it is now the source lowering's back end; the
deletion assertion is over `SliceLowering` and not over the file it sits beside.

**Why this is a correction and not a quiet re-reading.** A deletion clause that names the wrong
subject fails in the worse direction: someone discharging it would either delete a builder the
front end needs, or read the clause as discharged because the file it names is still there. The
`SliceProgramBuilder` remark carried *scheduled for deletion* in its own words and now carries what
it actually is.

**What it does not change.** JS-4 is still `Not started` and still waits on JS-2; the hand-written
programs are still in the corpus and still write half of it; and the deletion is still a gate
clause with an owner rather than something this milestone did.

**Authority and date.** [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md),
the closing section; the reference set the front end actually has; 2026-09-03.

---

### JSC-46

**Where:** delivery [section 19](roadmap.delivery.md#19-milestones), JS-3b's exit gate; roadmap
[section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the static-semantics subsection's
closing sentence.

**What the plan said.** *Each artifact is tokenized at most once **during verification**, asserted
by a case.*

**What was actually true.** Nothing is tokenized during verification, and after
[JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) nothing can be.
The clause presumes the design the boundary decision rejects — one in which the verifier holds the
front end and therefore could tokenize twice. Verification takes bytes; a tokenizer takes
characters; the two run in different assemblies over different inputs, and the execution-only
composition publishes and runs with a verifier and no tokenizer at all.

**What replaced it.** *Each source is tokenized at most once **during compilation**, and the
verifier tokenizes nothing.* Both halves are asserted, and the second is the stronger one: the
first is a property of the front end's call graph — the tokenizer has exactly one caller — and the
second is a property of a published closure that does not contain a tokenizer to call.

**Why the wording mattered.** As written the clause was satisfiable only by a fused design and
vacuous under an unfused one, so it would have been reported as met by a component that never
tokenized anything. That is the failure mode this file exists for: a gate whose subject stopped
existing reads as passing.

**What it does not change.** The clause's purpose is unchanged — the seed re-tokenizes raw source
in two places and section 9 wants those scans gone — and both are gone, with the facts they
recovered carried on the token instead. JSD-0014's decision 2 records which fact replaced which
scan.

**Authority and date.** [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md),
decisions 2 and 3; 2026-09-03.

---

### JSC-47

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the
untrusted-input surfaces; delivery [section 19](roadmap.delivery.md#19-milestones), JS-9's exit
gate and the dependency figure; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-9
row.

**What the plan said.** *Two of the four surfaces are not fuzzed because they do not exist* —
the source tokenizer and parser, and the regular-expression matcher — with the delivery file's
dependency figure carrying the same in a single clause, *do not exist until JS-3b and JS-6*.

**What was actually true, from 2026-09-03.** JS-3b wrote the front end. The source tokenizer and
parser exist, they are the largest untrusted-input surface this component has ever had — they
take a caller's text rather than a caller's bytes — and **no fuzz session reaches them**. The
sentence stayed literally readable and stopped being honest, because it explains an absence for
something that is present.

**What replaced it.** The two admissions are separated. The regular-expression matcher is
**absent** and waits on JS-6. The source tokenizer and parser are **present and unfuzzed**, which
is a gap this milestone opened and JS-9 owns. Both are still surfaces no session may be read as
covering, and that half of the original sentence is unchanged.

**Why the distinction is worth a numbered correction.** *Nobody has written the target* and *there
is nothing to target* are answered by different work and carry different risk. A reader deciding
what JS-9 still needs gets the wrong answer from the merged sentence: an absence resolves itself
when the milestone that owns it lands, and a gap resolves only when somebody writes a mutator for
a character stream. This component's whole method is that a claim can be re-derived by a reader,
and this one could not be.

**What it does not change.** No session moved, no counterexample appeared, and the guidance clause
is open exactly as it was. The four retained sessions were seeded from a 66-entry manifest against
a checkout that now holds 91, so under update rule 5 they were already evidence over a different
population and this correction adds a second reason to re-run them rather than the first.

**Authority and date.** The checkout's own front end, read against the sentence;
[JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md); 2026-09-03.

---

### JSC-48

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-9 row, the soak's
plateau clause; delivery [section 19](roadmap.delivery.md#19-milestones), JS-9's exit gate.

**What the plan said.** The soak's plateau check compares the heap at the midpoint of the run
against the heap at the end, against a band, and **both readings are after warm-up, which is what
makes them comparable in every publish mode**. That sentence is the check's premise, it is printed
in the check's own output, and [JSC-41](#jsc-41)'s neighbouring correction had already moved the
baseline to the midpoint once to make it true.

**What was actually true.** The check never verified its premise, and the premise was not the only
thing that decided the reading. With two forced collections in the whole run — one at the
midpoint and one at the end — the final number included heap the collector had not returned,
and **how much that was scaled with how much the process had allocated before the soak started**.
The soak is preceded in the same invocation by the corpus replay, so the check was coupled to the
size of the retained corpus, which is a fact about a different milestone's evidence and about
nothing this check claims to be measuring.

**How it was found, and it was not by reading.** JS-3b grew the retained corpus from 66 entries to
91. The plateau check then failed on macOS under Native AOT, on **both** architectures, and it
failed **byte-identically across three independent runs** — 191,496 bytes at the midpoint to
352,120 at the end, a factor of 1.84 against a band of 1.20 — while `win-x64`, `linux-x64` and
the Android head stayed flat. Determinism to the byte is what ruled out noise and made it worth
diagnosing rather than re-running.

**What replaced it.** The check records **sixteen samples of the heap across the run** and prints
them. The sampling was added to tell warm-up from a leak — two endpoints cannot, which is why
the previous occurrence needed a diagnosis rather than a glance — and it turned out to remove
the failure, which is the finding rather than a side effect. Keeping the heap trimmed through the
run leaves the reading measuring live bytes, which is what a per-cycle leak moves and what the
process's history does not.

**The sample count is therefore load-bearing, and the code says so where someone would delete it.**
More sampling makes this check **stricter** rather than kinder: a real leak grows live bytes, which
every reading sees however often they are taken, while the signal that disappeared was never about
leaking. **No band was widened and no threshold moved** — the band is the 1.20 [JSC-41](#jsc-41)
tightened it to, and it is unchanged.

**What it does not change.** The soak still demonstrates only that recycling a runtime for the
cycle count it uses does not grow the heap without bound; it still measures nothing, its band is
still loose, and JS-5 still owns measurement. **It closes no clause**: the plateau clause needs a
retained bundle and none has been collected since, which the JS-9 row already said and still says.

**Authority and date.** The check's own curve, read on four platforms across three CI runs;
2026-09-03.

---

### JSC-49

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the
static-semantics subsection; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b
row.

**What the plan said, and what JS-3b's first implementation did.** The plan says the parser rules
on nothing and one validation stage carries every early error, and
[JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md)'s first decision
records that as this component's answer. **The implementation did not do it.** The parser refused a
<c>function</c>, an object literal, a string value and a loose equality itself, as unparseable
reserved words and construct refusals raised from the pass that owns the grammar.

**What was actually true.** That put the feature manifest's boundary inside the parser, which is
the split the decision was written to avoid, and it had a consequence nobody noticed until
something needed it: **this front end could not READ JavaScript.** It stopped at the first
construct outside a manifest that admits no function, no object and no string, which is the first
few tokens of essentially every real program. A front end that cannot read the language cannot
count what the language contains, and counting is what the remaining milestones' scope should be
decided from.

**What replaced it.** The grammar is the language's. The parser produces a node for every
construct it recognises `— functions, classes, objects, arrays, calls, member access, regular
expressions, templates, destructuring, arrows, generators, modules, `try`, `switch`, labels `— and
refuses only what is not a tree at all. `SliceManifest` holds what the manifest admits, the
validation stage refuses everything else by name, and it **walks into** each refusal rather than
stopping at it, so one source yields one diagnostic per occurrence. The precise node types the
lowering switches on are unchanged, which is why every retained corpus entry replays to the same
answer and the artifact bytes are identical.

**And it is what made the two measurements possible.** A construct census over the Octane benchmark
and over test262 is now a thing this component can run, and the roadmap's remaining scope has a
ranked input rather than an assumption. The ledger's JS-3b row carries what those runs found, as
observed repository state under section 1's third category `— they satisfy no gate and are not the
conformance oracle [section 14](roadmap.md#14-the-conformance-oracle) specifies, which needs a
pinned suite revision, a self-check, per-host-mode totals and a ratchet, and has none of them.

**What it does not change.** The manifest is the same manifest and admits exactly what it admitted;
nothing new is lowered; JS-3b is not closed; and no third-party source entered this repository `— the
census takes a path and keeps no copy, because retrieving, hashing and archiving the suite is the
human action [section 3](roadmap.status.md#3-open-external-dependencies) still records as open.

**Authority and date.** [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md),
decision 1, read against its own implementation; 2026-09-03.

---

### JSC-50

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering), the deep-nesting
subsection; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row; the published
registry's row for 2303.

**What the plan said.** Deep nesting is bounded by an explicit compile-time depth bound carried in
the parse options, and a process termination on a nesting case blocks the milestone. JSC-43's
implementation added the bound with a default of 64 and **no maximum**.

**What was actually true.** A bound a caller can set arbitrarily high is not a bound. One level of
source nesting costs several stack frames `— an assignment, a conditional, two binary levels, a
unary, a postfix, a call chain, a primary `— so a caller asking for four thousand levels got a
stack overflow, which is precisely the process termination the mechanism exists to prevent,
reached through the mechanism. **This component's own generated corpus entry was that caller**, and
it terminated the process on the first run after the grammar landed.

**What replaced it.** A measured maximum. Right-nested addition parses at 512 levels of source and
dies at 768, so the options refuse a bound above 512 counter units `— about 170 levels of source, a
third of the measured failure point `— at construction rather than at the overflow.

**And a finding fell out of it, which is why this is a correction and not a patch.** The format's
operand stack admits 1,024 values, and the only shape in this manifest whose operand stack grows
with its nesting is right-nested operators: a left-nested chain runs at a height of two however
long it is, and parentheses emit no instruction at all. Reaching the stack ceiling therefore takes
more than a thousand levels of nesting, and the depth bound refuses at about a hundred and seventy.
**The parse bound dominates the stack ceiling**, so `OperandStackTooDeep` is reachable by no source
this front end will accept. The registry records it as defensive with that reason, which is a true
statement about this build rather than a generated source nobody can write.

**What it does not change.** The bound is still the answer to section 9's nesting question and the
worklist rewrite is still refused for the reasons JSD-0014 records. The nesting corpus entry still
refuses rather than surviving, and the 100,000-level case still returns a diagnostic rather than
ending the process.

**Authority and date.** The parser's own measured recursion limit, taken on `win-x64` under JIT;
2026-09-03.
### JSC-51

**Where:** roadmap [section 14](roadmap.md#14-the-conformance-oracle), the configuration-failure
clause; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a row.

**What the plan said.** "Configuration failures are a closed, named set and each is a failure:
inconsistent shard configuration, missing suite revision, incomplete variant coverage, empty
selection, no executed tests." Five names, and the word *closed*.

**What was actually true.** The very next sentence states a sixth requirement and gives it no
name: "Removing one shard's report must produce incomplete coverage, not a smaller total." A run
that is missing a shard is misconfigured in exactly the way the other five are, and a set that
could not name it would leave the plan's own next sentence unimplementable inside its own
vocabulary. A closed set with a behaviour outside it is not closed.

**What replaced it.** The set is six: the plan's five, with `IncompleteShardCoverage` added for the
behaviour above, and "incomplete variant coverage" spelled `IncompleteHostModeCoverage` because the
axis this profile varies over is the host mode rather than a minifier. Two things are deliberately
*not* members. A self-check mismatch is not, because the self-check runs before a shard is
configured at all and a mismatch has no run to be a property of - it stops the process on an exit
code of its own, and folding it in would let a reader believe a run had been configured and had
then gone wrong. And a selection pipeline that fails to account for every candidate is not, because
that is a defect in the harness rather than in the run: it is asserted at run time and reported as
a harness defect, and the harness's own regression suite has a check for it.

**What it does not change.** Each of the six is still a failure of the run and never a smaller
total, and the merge still refuses to add reports it has not first proved are one run's.

**Authority and date.**
[JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md), decision 6;
2026-09-03.

### JSC-52

**Where:** roadmap [section 14](roadmap.md#14-the-conformance-oracle); the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a row.

**What the plan said.** Section 14 specifies, in detail, how a run's results are selected, sharded,
counted, merged, ratcheted and refused. **It says nothing about isolating one case from another.**
Read as a whole it implies the property - a per-host-mode total is only meaningful if a case's
verdict is that case's - but nothing in it makes the implication a requirement, and the obvious
implementation composes the engine once per shard.

**What was actually true.** A fuel allowance is spent over a runtime's whole life rather than reset
per invocation. One composed runtime for a whole shard therefore means the first program that does
not terminate spends the allowance and **every case after it is reported as a timeout**. The first
scored run of this suite reported thirty-four timeouts and nothing else - a total indistinguishable
from an engine that had stopped working, which is precisely the reading section 14 exists to make
impossible. The self-check did not catch it, because the one non-terminating fixture happened to
sort last.

**What replaced it.** Every case runs in a runtime of its own, and section 14 says so. The
self-check gained a fixture whose only job is to run *after* the non-terminating one and pass; a
shared allowance makes it report a timeout, which is the shape of the defect rather than a proxy
for it. The allowance stays fuel rather than the wall clock, because fuel is charged per
instruction and bounds a runaway program in instructions rather than in seconds - a wall-clock
allowance would make one test pass on an idle machine and fail on a busy one, and a floor sensitive
to that is a floor nobody can act on.

**What it does not change.** No total, no host mode and no configuration failure moves. The defect
was in what the harness shared between cases, not in what it counted.

**Authority and date.** The harness's own first scored run, on `win-x64` under JIT, and
[JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md), decision 3;
2026-09-03.

---

### JSC-53

**Where:** [JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md)'s
remark on the metadata reader; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a
row; [section 3](roadmap.status.md#3-open-external-dependencies)'s suite-revision row.

**What the record said.** The harness's metadata reader carries a remark that the shape it reads is
"deliberately the one the conformance suite uses ... so that the day a pinned suite is retrieved,
this reader is pointed at it rather than replaced". Section 3's row was narrowed on the strength of
it: what remained blocked was scoring third-party material, on the pin.

**What was actually true, and it was measured rather than argued.** Five files were written in the
real dialect - a nested `negative` mapping, a folded `description`, an `info` block scalar, and no
`expected` key, because no such key exists in that dialect - and put through the harness on
2026-09-03. **All five were refused**, each with `declares no readable expectation`. A suite is
read whole and an unreadable file makes the whole read a harness defect, so the run scored nothing
at all: not a wrong total, no total. Pointed at a real checkout the harness would have produced one
complaint per file and a `HarnessDefect` exit.

The reader was shaped like a **simplification** of the dialect rather than the dialect: flat
`key: value` lines only, no nested mapping, no block scalars, and one required key the dialect does
not have. Three further gaps had no answer anywhere - which of this build's diagnostic codes stand
for which JavaScript error, that a file declaring no strictness is defined to be read twice, and
that both vocabularies use a flag spelled `raw` for different things.

**What replaced it.** A reader for the dialect as written, a translation into this harness's
vocabulary, and the rule [JSC-54](#jsc-54) records. Both dialects are kept and a run is told which
it is reading; the native one stays because an ingested suite cannot express a positive expectation
without an assertion library this manifest has no function to load. The self-check is read in the
suite's dialect, and `--selfcheck <dir>` lets a run against a checkout point at fixtures this
repository holds.

**What it does not change.** No suite is retrieved, pinned or held; nothing is fetched; the
`MissingSuiteRevision` refusal is untouched. Section 3's row narrows again and does not close: what
was blocked on a person and a reader is now blocked on a person alone. The `js-3a` suite's totals
are identical in every host mode, which is the check that the native path was not disturbed.

**Authority and date.** Five suite-shaped files run against the harness as built, and
[JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md);
2026-09-03.

---

### JSC-54

**Where:** roadmap [section 14](roadmap.md#14-the-conformance-oracle), the negative-metadata clause;
the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a and JS-3b rows.

**What the plan said.** Negative tests are opt-in "with the uncaught error reported by its
JavaScript type name so a parse-phase syntax error is matched on what it is". That sentence is
right and it is not enough, because it says how to compare an answer with a declaration and says
nothing about **when this profile's answer is an answer to that question at all.**

**What was actually true.** `broiler.javascript.slice` admits no function, no object, no string
value and no property access, so it refuses valid JavaScript on almost every line - and it refuses
it with one code, `ConstructOutsideManifest`. A suite's negative tests almost all declare that a
refusal must happen. Compare the two on the observable outcome and they agree nearly every time,
for reasons that have nothing to do with each other: **a manifest that admits almost nothing would
have reported a near-perfect conformance total**, silently, at scale, in the direction that
flatters. Section 14 opens by saying an engine that grades itself is not evidence and names this
exact failure - an engine that refused a test "for the wrong reason" - and then specifies no
mechanism that would catch it.

**What replaced it.** Every source-refusal code carries a declared language class, and only one of
the four may answer a suite's expectation: a genuine early error. The manifest's own refusal, this
profile's two recorded divergences, and its four implementation ceilings may not. A case whose
refusal cannot score is reported **unscorable** - not a pass, because the engine did not earn one;
not a failure, because nothing there is a defect and the failure manifest is a repair queue. The
rule runs ahead of the comparison, so no declaration can be written that gets past it, and it holds
in the positive direction too.

**What it does not change.** This component's own fixtures are unaffected: one declaring
`refused-by-source ConstructOutsideManifest` asked whether *this front end* refuses a construct
outside its manifest, which the refusal answers exactly, and it is still scored as a pass. The
`js-3a` suite's totals do not move.

**Authority and date.**
[JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md),
decision 3, with eight negative controls retained in
[Bundle JS-3A-005](evidence/js-3a-005/README.md); 2026-09-03.

---

### JSC-55

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s construct-census subsection,
the row reading `The Octane benchmark | 24 | 24 | 0` and the ranked list under it.

**What the ledger said.** Twenty-four files were measured, all twenty-four parsed, none contained
only constructs the manifest admits, and a ranked list followed: a string value in 24 files, a call
in 23, a function in 23, and so on.

**What was actually true, on a re-run over the same checkout.** Every figure is reproducible and
none is wrong. **What is wrong is calling the twenty-four files "the Octane benchmark",** which
invites the reader to take them for twenty-four benchmarks. They are not. Three are the
demonstration page's own assets - jQuery and two Bootstrap plugins, shipped so the checkout's
`index.html` renders - and have nothing to do with the workload. One is the harness that defines
the benchmark type, one is the runner, and two are data blobs a benchmark reads rather than code it
runs. **Seventeen are benchmark sources.**

**And the ranked list, read as a scope input, points the wrong way.** A ranking by how many files
need a construct invites "buy the top of the list first"; the number a scope decision actually
wants is what buying the top *k* would admit. Measured over the twenty-four-file corpus, one
construct - a string value - admits a whole file, which reads as a cheap first win. **It is not a
benchmark: it is `typescript-input.js`, a data blob.** Measured over the seventeen benchmark
sources, the answer is the opposite one: **the thirteen highest-ranked constructs admit nothing at
all.** Nothing is admissible until the fourteenth, one file arrives there, and all seventeen need
all twenty-eight. The nearest benchmark source needs nine constructs.

**What replaced it.** The census reports the curve beside the ranking - what admitting the ranked
constructs in order buys, and how many constructs each file needs - so the ranking cannot be read
as a purchase order without the number that contradicts it. The ledger's subsection carries the
corrected composition of the corpus and both readings.

**What it does not change.** No number already published moves; the parse rate, the file counts and
the occurrence counts are the same. Nothing is accepted, no manifest grows, and the checkout stays
where it is: the census takes a path and keeps no copy, and no third-party file entered this
repository for this correction any more than for the first one.

**Authority and date.** A census re-run over the same Octane checkout, per file and in aggregate;
2026-09-03.

---

### JSC-56

**Where:** [`docs/compositions.md`](../../../docs/compositions.md) section 3, the paragraph
beginning "Neither is `narrow-runtime-compiler`"; roadmap
[section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding); the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said.** The register recorded that no composition root here held the
`narrow-runtime-compiler` label, that the slice-compiler root "is only shaped like one", and that
"there is no source surface until JS-3b writes the tokenizer and the static semantics". The ledger
recorded the consequence as an open gate clause: a publish-and-run of that composition on every
claimed RID, "which is a collection nobody has made".

**What was actually true after JSC-43.** JS-3b wrote the tokenizer, the parser, the one validation
stage and the source lowering, so **the source surface the label waits on has existed since
2026-09-03** and nothing in the register said so. What was still genuinely missing was narrower
than "a source surface": it was a composition **handed source from outside its own image**. Every
JavaScript root here reads its input from inside one - the slice-compiler root lowers a
programmatic builder, the execution-only root reads an embedded corpus, the conformance root reads
a fixture tree this repository also wrote - and a root lowering its own input cannot demonstrate
what the label means.

**What replaced it.** `Broiler.VM.Composition.JavaScript.Cli`, the end-user host: a path on a
command line, compiled, verified, run, with the completion value printed. Its catalog table prints
`narrow-runtime-compiler` where its siblings print `narrow-runtime-compiler-shaped`, and **its
closure is the first here a reader can compare against section 15's row without a paragraph of
exceptions** - it carries no corpus replay, no corpus writer, no fuzz mutator, no soak, no
cross-profile checks and no conformance harness, all of which its siblings carry because rules A11
and A12 leave such code nowhere else.

**What it does not change.** The advertised set is still empty and this root is a demonstration
like every other. That is not deferral: **a tool advertised as a JavaScript host has to be able to
run JavaScript**, and this manifest admits no function, no object, no string value and no property
access - pointed at the Octane benchmark the host refuses all twenty-four files. The gate clause is
narrowed rather than closed: the composition exists and is published and run on one RID, and
"every claimed RID" is still a collection nobody has made.

**Authority and date.** [JSD-0017](decisions/0017-the-end-user-host-and-what-an-exit-code-promises.md),
with [Bundle JS-3B-001](evidence/js-3b-001/README.md); 2026-09-03.

---

### JSC-57

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s construct-census
subsection; roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s deep-nesting
subsection; [JSC-50](#jsc-50), which set the bound's maximum and left its default at 64.

**What the ledger said.** The census measured 24 Octane files and reported **24 parsed**, 0
containing nothing outside the manifest, and a ranked list of the constructs the manifest excludes.
Every one of those figures re-derives.

**What running the end-user host over the same files found.** **Two of the twenty-four are refused
before the manifest is consulted at all**, with `2103:NestingTooDeep` - `earley-boyer.js` and
`mandreel.js` nest deeper than the 64 levels the parse options admit by default. Raise the bound to
the largest the parser supports and both files get past the parser and are then refused by the
manifest instead, so all 24 report `ConstructOutsideManifest`.

**Both numbers are right and they answer different questions.** The census reads at
`MaximumSupportedNestingDepth` deliberately, and its own remark says why: "a census wants to read
the file rather than to enforce the slice's own conservative default". So the census measures **the
language**, and the host at its default measures **the product**. What was missing is that nothing
said the two disagree, and the disagreement runs in the direction that flatters: `24 parsed` is not
a statement about what a person pointing the shipped default at those files sees.

**And the diagnostic a user gets is the less useful one.** `NestingTooDeep` is a ceiling the
specification permits an implementation to have - it is classified as an implementation limit by
[JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md)'s
map, and no refusal carrying it may answer a conformance question. So at the default those two
files report a refusal that says nothing about the manifest, when the manifest is what a reader
wants to know about.

**What replaced it.** The host takes `--max-depth`, so the two readings are measurable rather than
being a discrepancy between two documents, and the ledger's census subsection carries both. **The
default is not changed by this entry**: whether 64 is the right default for a host is a decision
with a measurement behind it now and an owner still to name it.

**What it does not change.** No census figure moves, no manifest grows, and nothing is accepted.
The Octane checkout stays where it is: the host takes a path and keeps no copy.

**Authority and date.** `Broiler.VM.Composition.JavaScript.Cli` run over the same Octane checkout
at both bounds, retained in [Bundle JS-3B-001](evidence/js-3b-001/README.md); 2026-09-03.

---

### JSC-58

**Where:** `SliceSourceCompiler`'s own remark on the one shape it cannot emit; roadmap
[section 9](roadmap.md#9-the-semantic-front-end-and-lowering); the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said.** "One shape it cannot emit, stated because it is a real program. A loop
whose exit is reachable from nothing — `while (true) { }` with no `break` — lowers to a tail the
verifier refuses as unreachable code. That is the format's answer rather than this lowering's, and
it is a conformance exclusion the decision record carries."

**What was actually true.** The sentence is true and it names the wrong shape. Sweeping test262
through the end-user host found **thirteen files refused with `1411:UnreachableCode`, and not one
of them is that shape**: every one is `for (…) { break; }`, whose exit is perfectly reachable —
through the `break`, and in some of them through the test as well. What is unreachable in those is
**the loop's own continuation**: a body that always breaks never falls into the update expression
or the back-edge, and the lowering emits both regardless.

That makes two shapes rather than one, and the common one was the unnamed one:

| Shape | Unreachable | Ordinary JavaScript? |
|---|---|---|
| A loop whose body always exits — `while (true) { break; }`, `for (var i = 0; i < 3; i++) { break; }` | the update and the back-edge | **yes**, and 13 test262 files are this |
| A loop with no exit at all — `for (;;) { var x = 1; }` | everything after it, **including the program's tail** | yes, and it was the only one named |

**And the second half of the sentence is only right about the second shape.** "The format's answer
rather than this lowering's" holds where suppressing the unreachable region would leave a function
with no terminator. It does not hold for the first shape: there the lowering could stop emitting
once the position is unreachable and resume at a label something branches to, and the program would
verify. **The first is a defect with a reproduction, not a limitation with an example.**

**What replaced it.** The remark now names both shapes and says which looks fixable here. Three
minimal reproductions are pinned in the host's acceptance suite under `known-defects/`, declared
with the exit code they currently answer with, **so the suite goes red when the defect is
repaired** — which is how a characterisation case reports a fix. The repair itself is not in this
change.

**What it does not change.** Both remain conformance exclusions. No total already published moves:
these programs never verified, so no artifact whose bytes are retained is affected, and a lowering
that stopped emitting unreachable code would change the bytes of exactly the programs that do not
verify today.

**Authority and date.** The end-user host over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, retained in
[Bundle JS-3B-001](evidence/js-3b-001/README.md); 2026-09-03.

---

### JSC-59

**Where:** [JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md)
decision 1 and the dialect reader it records; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3a row.

**What the record said.** The ingested dialect is read "as written", by a reader that handles the
shapes the dialect actually uses and refuses what it does not recognise rather than skipping it.
Seventeen harness checks covered nested mappings, folded and literal block scalars, both list
spellings, unknown flags, unknown phases and eight malformed blocks.

**What was actually true.** Pointed at 53,469 real suite files the reader refused **two**, and both
refusals were its own defects rather than the files':

- **A file written with carriage returns alone.** The reader normalised `\r\n` and split on `\n`,
  so a CR-only file was one line, no key was found, and it reported that the file *declared no
  description*. The file whose subject is line-terminator normalisation is written in CR
  deliberately, which is the joke at the reader's expense: the one file in the suite most likely to
  use an unusual terminator does.
- **A file whose whole metadata block is indented.** The check for a top-level key compared
  indentation against column zero, so a block indented as a unit — legal, and one file does it —
  had every key reported as *indented under no key*.

**Neither was reachable from a check this component wrote**, and that is the part worth recording.
Both hand-written fixtures and both harness checks used LF and a block at the margin, because that
is what somebody writing a fixture writes. **A dialect reader's fidelity is measured against
material nobody here authored, or it is measured against its author's habits.**

**What replaced it.** Every line terminator the language has is normalised, pair before lone
carriage return so a CRLF file does not gain a blank line between every pair; and the block is
dedented by the indentation its own lines share before anything is parsed, so relative indentation
is what the reader means. With both, the reader reads all 53,469.

**What it does not change.** The suite is still read whole and a block the reader cannot parse
still refuses the run rather than declining one file. **That is deliberate and it is why these two
were found**: a reader that had quietly declined them would have reported a clean run over a suite
it had misread twice, and nobody would have looked. The cost is that one malformed upstream file
blocks a run, and that is the trade this keeps.

**Authority and date.** The harness over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, retained in
[Bundle JS-3B-001](evidence/js-3b-001/README.md); 2026-09-03.

---

### JSC-60

**Where:** `SliceSourceCompiler`'s remark on the shapes it cannot emit, as
[JSC-58](#jsc-58) left it; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said.** JSC-58 corrected the remark to name two shapes instead of one and said of
the common one — a loop whose body always exits — that it "looks fixable here rather than being the
format's answer", and that "the repair itself is not in this change".

**What is now true.** It was fixable, and it is fixed. `SliceControlFlow` answers whether control
can reach past a statement, and the three loop lowerings emit their continuation only where
something reaches it: the back-edge where the body can fall through, and a `for` loop's update where
the body can fall through **or** a `continue` targets that loop. `while (true) { break; }` and
`for (var i = 0; i < 3; i = i + 1) { break; }` run.

**The evidence is a number that moved by exactly the right amount.** The host's sweep of the same
53,469 suite files went from **103 completions and 13 artifact refusals** to **116 completions and
none** — the thirteen, and nothing else. Every other column is unchanged.

**And the number that did not move is worth as much.** The conformance harness's totals over the
same suite are **identical to the byte before and after** — 8,572 selected, 1,188 executed, 1,170
passed. The thirteen files are positive tests needing the assertion prelude, so the harness never
selected them. Two instruments, one repair, and only the one that runs files as programs could see
it: **a repair visible to every instrument would have been a repair to something else.**

**Why this could be done safely at all.** The analysis is conservative in one direction: where it
is unsure it answers "reachable", which is what the lowering did unconditionally before. So the
only bytes that can move belong to programs the verifier was already refusing — and **the retained
corpus regenerated byte-identical**, which is the check rather than the claim.

**What it does not change.** The second shape — a loop with no exit at all — is still refused, and
it is the shape the remark named before JSC-58. Everything after such a loop is unreachable
including the program's own tail, and suppressing a tail leaves a function with no terminator: a
different invalid artifact rather than a valid one. It remains the format's answer and a
conformance exclusion, and it is pinned separately in the host's acceptance suite **so that one
change cannot quietly claim both**.

**And one shape is named here without being repaired.** A block continues to be lowered whole even
after a statement control cannot pass — `for (;;) { break; var x = 1; }` emits the declaration.
Suppressing it is not the same edit: `var` is hoisted, its slot must be written before anything
reads it, and a slot the executor never wrote is a state the profile does not guarantee. That is a
separate piece of work with a separate hazard and it is recorded rather than folded in.

**Authority and date.** The host over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` before and after the repair, with the corpus
regeneration, retained in [Bundle JS-3B-001](evidence/js-3b-001/README.md); 2026-09-03.

---

### JSC-61

**Where:** the tokenizer's trivia handling; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What was true.** `#!` opening a source text is a **comment** — `HashbangComment :: #!
SingleLineCommentChars_opt`, in the language since ES2023 — and this tokenizer did not know it. Six
files of test262 failed on it, each refused at the `!` with `2102:ExpectedToken`, because a
JavaScript file carrying the interpreter line an operating system reads is a JavaScript file.

**What replaced it.** The hashbang is skipped once, before the token loop, and **only at offset
zero**. The grammar admits it at the start of a source text and nowhere else, so a `#!` anywhere
later stays what it was: a character that begins no token. That is asserted in both directions —
a hashbang at the start runs, one on the second line is still refused, and a second `#!` after a
first is refused too.

**The line terminator ending it is deliberately left behind** for the trivia skipper. A statement's
end is decided partly by whether a line terminator came before the next token, so consuming it here
would have lost the newline the first real statement is entitled to see.

**What it does not change.** No manifest grows: a hashbang is a comment, so what follows it is
admitted or refused exactly as before. **One of the six is now DECLINED rather than passed**, which
is the honest outcome: the file parses past the hashbang and then needs a construct this manifest
does not admit, so the refusal is not a language answer and
[JSC-54](#jsc-54)'s rule reports it unscorable.

**Authority and date.** The host and the harness over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, retained in
[Bundle JS-3B-002](evidence/js-3b-002/README.md); 2026-09-03.

---

### JSC-62

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s format
subsection; [section 9](roadmap.md#9-the-semantic-front-end-and-lowering); the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What was true.** `let` and `const` have a temporal dead zone: reading one before its initialiser
has run is a **runtime `ReferenceError`**. This profile answered `undefined`. Eight cases of test262
— four files in both strictness readings — failed on it, and the reason they failed is worth
stating exactly: **reading a slot that had not been written yet was indistinguishable from reading
one holding `undefined`**, which is precisely the distinction the dead zone exists to draw.

**And nothing in the format could express the difference.** Division by zero is `Infinity` here and
every other instruction is total, so **format version 1 had no instruction that could fail at
all**. There was no way to lower the dead zone, not merely a lowering that had not been written.

**What replaced it.** One opcode, `ThrowUninitializedBinding` (`0x71`), which the executor answers
with a `ReferenceError`. Roadmap section 7 sanctions the growth in those words — "format version 1
is defined with the first manifest and **grows with the interpreter**", with compatibility promised
only when a persisted-artifact version is accepted, which no milestone grants.

**Two things about the opcode are decisions rather than details.** It **declares a push of one and
never pushes**: it stands exactly where a `LoadLocal` would have stood, so declaring that height
keeps every join, every bound and every reachability answer identical to the program with no dead
zone in it, and the frame is abandoned before the push happens. And it **carries no operand**, so
the message names no binding — naming one needs an interned name, and the constant pool's
interned-name tag is reserved from version 1 and admitted by no manifest yet.

**The detection is in the lowering and is exact for this manifest.** The lowering walks the tree in
the order the program runs, so a set of slots whose initialiser has already been lowered answers
the question directly. **That equivalence holds because of what this manifest leaves out** — no
function, no closure, no `eval`, no label, so no way to re-enter the middle of a block or defer a
read past its lexical position. In a manifest with any of those it would be a runtime question and
a set would be wrong; here the two orders are the same order, and the record says so rather than
leaving a later reader to discover that the analysis stopped being sound.

**What it does not change.** `var` is untouched and still reads `undefined` before its declaration,
which is the difference the dead zone draws. **The retained corpus regenerated byte-identical**: no
retained program reads a lexical binding early. And a position row is emitted for the fault and for
no other read — the first draft emitted one on every identifier read, which moved the bytes of
every program that reads a variable and was caught by the corpus comparison.

**What is still not done.** A **write** before initialisation — `x = 1; let x;` — is a
`ReferenceError` in the language and is not one here. It was not among the failing cases and the
assignment path is a separate site; it is recorded rather than folded in.

**Authority and date.** The host and the harness over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, with the corpus regeneration, retained in
[Bundle JS-3B-002](evidence/js-3b-002/README.md); 2026-09-03.

---

### JSC-63

**Where:** [JSC-62](#jsc-62) and the opcode it added; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said.** JSC-62 added `ThrowUninitializedBinding` for the temporal dead zone and
recorded two properties as decisions: that it **declares a push of one and never pushes**, standing
where a `LoadLocal` would have stood so that the verifier's height model is unchanged; and that a
**write** before initialisation — `x = 1; let x;` — is a `ReferenceError` in the language, is not
one here, and was "recorded rather than folded in".

**What was actually true when the write half was taken.** The declared push could not survive it.
The instruction a write replaces is a `StoreLocal`, which **pops**, so an opcode standing in place
of the operation would have had to be worth `+1` at one site and `−1` at another — the same opcode,
two stack effects, decided by which instruction it was impersonating. The read-only design was not
wrong; it was **a design that could not be extended**, and the extension is what showed it.

**What replaced it.** The opcode is a **guard** that moves no operand, emitted immediately before
the `LoadLocal` or `StoreLocal` it prevents. That instruction is emitted as it always was and
simply never runs, so the height the verifier computes is the height of the program with no dead
zone in it — **not because the guard declares a height it does not produce, but because it produces
none.** One contract, both sites, and nothing impersonating anything.

**The write's guard goes after the right-hand side, and that is a semantic choice.** The language
throws where `PutValue` happens, which is after the value has been evaluated — so
`x = (y = 1)` with `x` in the dead zone still assigns `y`. A guard at the top of the assignment
would have been simpler and would have got that wrong.

**And the message changed with it.** It read "a binding was read before its initialiser ran"; both
halves throw here, so it reads *used*.

**What it does not change, and this is the part worth reading.** **No figure in any suite moved.**
The harness's totals over test262 are identical before and after — 1,205 executed, 1,201 passed,
4 failed — and so is the host's sweep. The cases that would exercise a write in the dead zone are
not in the selectable slice: they need the assertion prelude this manifest admits no call to load.
**This is a repair the language required and the suite did not ask for**, which is why it is pinned
in the host's acceptance suite instead — the only instrument here that could hold it.

**The retained corpus regenerated byte-identical**, as it did for the read half: no retained program
touches a lexical binding early.

**Authority and date.** The host's acceptance suite, extended to both halves and both directions,
with the harness and the host re-run over test262 at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` and neither moving; 2026-09-03.

---

### JSC-64

**Where:** [JSC-60](#jsc-60), which named this shape without repairing it; the declaration
lowering's remark on why a declaration with no initialiser writes its slot; the executor's remark
on why every local starts as `undefined`; the
[ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said.** JSC-60 named a third shape of unreachable code and declined it: "a block
continues to be lowered whole even after a statement control cannot pass — `for (;;) { break; var x
= 1; }` emits the declaration. Suppressing it is not the same edit: **`var` is hoisted, its slot
must be written before anything reads it, and a slot the executor never wrote is a state the
profile does not guarantee.**"

**What was actually true.** The hazard is not there, and the code that removes it was already
written with a comment saying so. The instance constructor writes `undefined` into **every** local
slot when it is built — "which is what `var` does in the language", in its own words. So a hoisted
`var` whose initialiser is unreachable reads exactly what the language says it should, and
suppressing the initialiser removes an instruction no execution reaches.

**The claim came from the declaration lowering's own remark**, which says a declaration with no
initialiser writes its slot because "a slot the executor never wrote and a slot holding `undefined`
must not be distinguishable, and **only the latter is something it guarantees**". That reads as a
statement about the executor and it is a statement about the lowering's caution: the executor
guarantees both, in a loop, on purpose. **Two remarks in two assemblies, and the more pessimistic
one was the one consulted.**

**What replaced it.** A block stops being lowered after a statement control cannot pass. The
program body does **not** — its tail is the `Return`, and suppressing that would leave a function
with no terminator, which is a different invalid artifact rather than a valid one.

**And both remarks are now right about their own subject.** The executor's said the slice has no
temporal dead zone, which stopped being true when the format grew an instruction that can fail. The
lowering's now says what the write actually carries: `let x;` initialises the binding at that point
and the store is what ends its dead zone — a better reason than the one it had, and a load-bearing
one.

**What it does not change.** **No figure in any suite moved.** No file of test262's selectable
slice has dead code after a terminator, so the host's sweep and the harness's totals are identical
before and after — as they were for [JSC-63](#jsc-63), and for the same reason: the cases live
behind an assertion prelude this manifest admits no call to load. It is pinned in the host's
acceptance suite instead, with the two directions an over-eager suppression would fail — a
statement *before* the terminator still runs, and a `break` inside an `if` does not end the block.

**The retained corpus regenerated byte-identical.**

**What is still refused, and it is now the only one left of the three.** A loop with no exit at all.
Everything after it is unreachable including the program's own tail, and that is the shape the
compiler's own remark named before any of this began.

**Authority and date.** The executor's own initialisation of every local, read against the
lowering's remark about it; the host's acceptance suite; the host and the harness re-run over
test262 at ref `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` with neither moving; 2026-09-03.

---

### JSC-65

**Where:** `SliceSourceCompiler`'s remark on the shapes it cannot emit, as
[JSC-58](#jsc-58) and [JSC-60](#jsc-60) left it; [JSC-64](#jsc-64), which called this the last of
the three; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row.

**What the record said, three times.** A loop nothing can leave — `for (;;) { var x = 1; }` — makes
everything after it unreachable **including the program's own tail**, "and suppressing a tail leaves
a function with no terminator: a different invalid artifact rather than a valid one." That was
recorded as the format's answer rather than the lowering's, and was carried forward unexamined
through two repairs of its neighbours.

**What was actually true.** The verifier's rule is that **every *reachable path* ends in a return**.
A loop nothing leaves has **no path that ends at all**: the code finishes on a backward jump, not by
falling off the end, so nothing reaches a point where a return is owed. The sentence is true of
every other way of arriving at the end of the code and is not true of this one — and the difference
was never checked, three times over.

**What replaced it.** The program's tail is emitted only where something reaches it, the same
discipline the loop continuations and then the blocks already had. `for (;;) { var x = 1; }` now
**runs, forever, until it spends its instruction allowance** — which is what an infinite loop is,
and what a host should do with one.

**It needed a second repair to work, and that one is the interesting half.** `while (true)` emitted
its test and a `JumpIfFalse` past the loop. That branch is taken by no execution, and once the tail
was suppressed its target sat **past the end of the code**, so the verifier refused the jump target
instead — a different diagnostic for the same mistake. **A test that can never be false is not a
branch**, and the three loop lowerings no longer emit one for it. `IsAlwaysTrue` admits only
literals, so nothing observable is skipped.

**One retained artifact changed bytes, and it is the first this session.**
`source-break-leaves-the-loop` is `var i = 0; while (true) { i = i + 1; if (i === 3) { break; } } i`,
and its lowering lost the test and the branch. **Its recorded answer is unchanged at `3`** and the
replay confirms it; what moved is the hash and the length. That is the corpus doing its job rather
than an accident: an artifact whose bytes move without its answer moving is exactly the event the
manifest exists to make visible.

**What it does not change.** **No figure in any suite moved** — no file of test262's selectable
slice contains a loop nothing can leave — so this is the third repair running in a row that the
suite could not ask for and the host's acceptance suite had to hold instead. And the answers of
every other loop are untouched: a loop with a `break`, a counted loop, `while (false)` and a
`do`/`while` all lower and run as before.

**All three unreachable-code shapes are now repaired**, and the directory that pinned them was
called `known-defects`. It holds none.

**Authority and date.** The host's acceptance suite; the retained corpus regenerated and replayed;
the host and the harness over test262 at ref `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` with neither
moving; 2026-09-03.

---

### JSC-66

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s account of the four
remaining test262 failures, twice; [Bundle JS-3B-002](evidence/js-3b-002/README.md) section 3,
which repeats it; [JSC-54](#jsc-54), whose rule this went around.

**What the record said.** The four were `using` declarations, "and those four should not have been
scored at all: the construct is a proposal, **the harness can filter by the suite's own feature
metadata, and this run did not pass a filter.** That is a mistake in how the run was made."

**Two things in that sentence were wrong, and the second is worth more than the first.**

**The harness could not have filtered.** `--features` is an **inclusion** filter: a run states the
features it wants and every test claiming none of them is removed. There is no set it could have
been given that removes one feature — and a test claiming **no** feature matches no inclusion set
at all, so any value at all would have deleted the 665 scored cases that claim nothing, which is
more than half the total. What the harness had was a reader for the metadata and no way to act on
it. "The run did not pass a filter" named an operator's slip where the mechanism did not exist.

**And four failures were not what it cost.** The run scored **121** cases claiming that feature and
**117 of them passed**. Every one is a `refused-by-source` on a `using` or `await using` syntax
test: this front end has no production for the form and refuses **every** spelling of it, and those
tests declare a `SyntaxError` for one particular malformed spelling. The outcomes agree and the
reasons have nothing to do with each other — which is exactly the scoring bug
[JSC-54](#jsc-54) was written against, arriving through the one door that rule cannot see. Its
question is *was this refusal a language answer*, and `ExpectedToken` genuinely is one — 672 of the
run's 1,201 passes rested on that code, and 561 still do, so reclassifying it was never available.
What the rule cannot ask is *was this test about the language at all*.

**So the record had it backwards.** It read as four failures to remove. What was actually there was
**117 passes to give back** and four failures beside them.

**What replaced it, and the authority is the suite's own**
([JSD-0018](decisions/0018-which-tests-are-about-this-language-and-who-decides.md), which records
the four alternatives this rejected). An ingested suite ships a
`features.txt` splitting its flags into a proposed section and a standard one, and says in its own
prose that the proposed flags are there "so that consumers may more easily omit them as necessary".
The harness now reads it and scores no test claiming a proposed feature. **Reading it is required
rather than offered**: a run whose suite has no readable list stops, in the same voice as a run
pointed at a suite with no pin, because a run that cannot tell a proposal from the language has no
business scoring either. A hand-written list in this repository would have been a list this
component chose, and a list this component chose is one it can quietly grow whenever something
fails.

**The reader has one way to be wrong and it is silent, so it is pinned by a check and by a control.**
The file writes `##` for ordinary comments inside a section as well as for its own headings — the
pinned checkout does it twice — so a reader keying on the prefix ends the proposed section at the
first such comment. That would have lost **twelve of its twenty-one proposals** and reported the
same shape of success. Headings are matched by their whole text; a list missing one is refused
rather than read as a suite with no proposals.

**What it cost, which is the point of recording it.** Over the same checkout at the same ref:

| | before | after |
|---|---:|---:|
| Executed | 1,205 | **1,084** |
| Passed | 1,201 | **1,084** |
| Failed | 4 | **0** |
| Candidates excluded for claiming a proposal | 0 | **8,304** |

**The number that matters is that the passes fell by 117 and nothing was repaired to make it
happen.** A change that removed four failures and left the passes alone would have been the same
change with its cost hidden.

**Three things were found while checking rather than while writing.** The merge compared four of
the eight pre-sharding selection figures, and the four it omitted — the scope, both feature stages,
the negatives — are exactly the ones a differently configured shard moves; it compares all eight
now. And **two of this change's own new checks passed under their own negative controls**: one
asserted a malformed feature list produces a complaint without asserting it produces no features,
and one could not tell the two stage orders apart because the case it used was removed by both.
Both are repaired, and the six controls now each fail exactly the check that names them.

**What is still open, and this repair narrows it rather than closing it.** Section 3 records the
**language edition** as unpinned, and it still is: what this reads is one suite's opinion of its own
flags at one revision, which is the nearest thing to an edition that anything here pins. A construct
the suite has not flagged at all is still scored on whatever this front end happens to do with it.
And the test-harness section is read and **not** excluded, because those flags name host
capabilities rather than constructs — every such test needs a call this manifest does not admit and
is counted unselectable a stage later. That is measured over the checkout rather than assumed: no
scored case claims one.

**Authority and date.** The suite's own `features.txt` at ref
`ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, read against the run it changes; a census of every
feature claimed by every scored case, before and after; six negative controls, each injected,
caught and reverted; the retained corpus regenerated and replayed; 2026-09-03.

---

### JSC-67

**Where:** the [ledger](roadmap.status.md#3-open-external-dependencies)'s unopened-dependency row
for the language-specification edition, open since JS-0; the JS-10 row and the JS-3a bundle's
exclusion 5, which repeat it.

**What the record said.** "The language-specification edition is not pinned, and JS-0 did not pin
it… JS-0 was asked to record the intended edition and **no decision record does, because recording
an edition nobody has retrieved would be a pin in name only.**"

**The reasoning was right, and it is why the row stood for eleven milestones.** An edition name
written down by somebody who had not gone and got the document is a pin that cannot be checked, and
refusing to write one was the correct call every time it was made.

**What was wrong is that the row was written as though there were two states.** Roadmap
[section 24](roadmap.gates.md#24-specification-and-platform-references) defines three, and names
the middle one in its own words: a document **retrieved, hashed and archived** is a pin taken, and
"until someone performs it the pin is **provisional** and carries a named exclusion in the ledger".
Provisional is not unpinned. The row read as a binary, so the only move it offered was one nobody
could make — and the adjacent row had already demonstrated the middle state on the same day, when
the conformance suite was retrieved into a temporary directory, hashed, read and recorded as a pin
over a transient checkout with the archive named as outstanding.

**What replaced it.** The pin actually taken, recorded in
[JSD-0019](decisions/0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md):
**ECMA-262, 17th edition (ES2026)**, at `tc39/ecma262` commit
`0248456c758431e4bb8e5d26333ff1865123c9cd`, with `spec.html` of 2,978,793 bytes hashing to
`ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0`, re-derivable by anyone in one
line. A commit rather than the `es2026` tag, because a tag can be moved; the published edition
rather than `es2026-errata`, because errata accumulate and pinning them means pinning a moving
target under a name that sounds fixed. **The row is still open**, and it now has a holder and an
unblock condition it did not have: a named human archives the document.

**What two of the three actions bought, which is the part worth reading.** Three claims this
component had made about the language in prose were checked against a fixed document for the first
time. `#!` is a comment "since ES2023" — **confirmed**, absent from ES2022 and present from ES2023
onward. `using` declarations are in no published edition, which is the premise
[JSC-66](#jsc-66) removed 121 cases on — **confirmed**, no `UsingDeclaration`, `DisposableStack` or
`SuppressedError` appears in the pinned edition. A binding used before its initialiser is a runtime
`ReferenceError` — **confirmed**.

**And the second authority disagreed with the first once, which is what a second authority is
for.** Of the twenty-one flags the pinned checkout of test262 lists as proposals, twenty carry no
marker in the pinned edition and one does: **`regexp-duplicate-named-groups` is in ES2025 and
ES2026** while the suite still calls it a proposal, so JSC-66's exclusion removes 19 files that
**are** about this language. It moves no figure — none of the 19 was scored even by the run that
had no exclusion at all, because a regular expression is not in this manifest — and the point is
that the risk [JSD-0018](decisions/0018-which-tests-are-about-this-language-and-who-decides.md)
recorded without a size now has one.

**The pin is declared in code so that a run states it and one edit cannot move it quietly.**
`JavaScriptLanguageEdition` carries the revision, the digest and — as a field rather than a
paragraph — **whether the document has been archived**; the conformance report carries an `edition`
line beside the suite revision it already carried, and refuses a report scored against a different
one; the end-user host prints it under `--version`, with an acceptance row over the words *NOT
archived* as well as over the edition's name. **Rule N14** holds the code, the decision record and
this ledger to naming the same revision and digest, and holds the archived field to the ledger's
account of the pin in both directions.

**What it does not do.** It accepts no manifest — the ledger's rule that none may be accepted
against an unpinned edition is not answered by a provisional one — it archives nothing, and no
third-party document is in this repository. And it does not make the feature filter edition-aware:
flags do not map onto clauses mechanically, so JSD-0018's reader still uses the suite's own split.

**Authority and date.** ECMA-262 retrieved at five editions from `tc39/ecma262` and hashed; the
three prose claims checked against them; the twenty-one proposal flags of the pinned checkout
searched in the pinned edition; four negative controls over rule N14, each moving the pin in one of
the places that must agree and each caught; 2026-09-03.

---

### JSC-68

**Where:** the [ledger](roadmap.status.md#3-open-external-dependencies)'s conformance-suite row and
its account of the test262 runs in
[section 2](roadmap.status.md#2-current-milestone-status); [Bundle JS-3B-001](evidence/js-3b-001/README.md)
and [Bundle JS-3B-002](evidence/js-3b-002/README.md), which say it in the same words.

**What the record said, in every place it was said.** "The pin is still over a **transient
checkout** — retrieved, hashed, read and left in a temporary directory — and section 3 asks for
material retrieved, hashed **and archived**. No suite file is in this repository."

**Every word of that is true, and it is not the part that mattered.** The harness's `--pin` mode
computes a digest over a directory and writes it **into that directory**. Every test262 figure this
component has published was obtained against a `suite.pin` the harness had generated inside the
checkout it was about to score. **Verifying against it proves the directory has not changed since
the harness last looked at it** — nothing about which upstream revision the directory is, and
nothing that an editor of the checkout could not arrange, because the suite and the pin sit side by
side and are editable in one gesture. The record described the pin as temporary. It was
**self-certifying**, which is a different and larger defect, and the word "transient" reads as the
smaller one.

**What replaced it.** A pin retained in **this** repository, at
[`src/tests/conformance/pins/test262.pin`](../../tests/conformance/pins/README.md), in a
file the suite cannot reach: the upstream commit `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, the
content digest `46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93` over 56,560
files, and the archive those bytes came from. `--expect` makes a run answerable to it, and a
checkout whose name, digest or file count is not the one this repository decided stops the run
rather than shrinking a total.
[JSD-0020](decisions/0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md) records
the decision.

**The pin was taken twice, and the second time is the one that counts.** The first checkout is the
one this component had been writing a `suite.pin` into for a day, so a digest computed there is a
digest over a directory this component had modified. The archive was retrieved again into a fresh
directory — **the two downloads were byte-identical** — extracted, and hashed independently. It
produced the same figure, which is what makes the retained digest a second reading rather than a
copy of the first.

**No figure moved and none was expected to.** The suite is the same suite; what changed is who says
so. The run under the retained pin reports the same 1,084 executed, 1,084 passed, 0 failed as the
run before it.

**What was still open when this was written was one action rather than three, and it was taken the
same day.** The suite is now **archived** — as the `.tar.gz` it was retrieved as, one file of
9,487,173 bytes rather than 232 megabytes of extracted tree, which carries the same evidence at
four per cent of the size — and with it **section 14's attribution row is discharged**, in the
change that first ingests suite material, exactly where [JSC-30](#jsc-30) said it would land.

**And one sentence of this entry expired within the hour, which is worth leaving visible rather
than editing away.** It read: *no floor is set over any test262 figure and none may be while the
material is somebody else's to change.* The material stopped being somebody else's to change when
it was archived, and **the floor is set**: 1,063 Script cases and 21 Module cases at revision
`46d54f57…1cfe93`, enforced once per lane on one Linux runner rather than on every publish cell.
The second objection this entry named — a lane cost nobody had agreed to — was measured rather
than argued: the run is about two minutes over a warm extraction and is dominated by hashing
56,560 files rather than by scoring 1,084 of them.

**And the replacement had the same defect in miniature, which the archive is what found.** The
first draft of the retained-pin check compared against the `SuiteRevision` the harness had
resolved — **which comes from the `suite.pin` inside the checkout**, the artifact this whole entry
is about. It passed only because the working checkout happened to carry one somebody had generated
in it. Pointed at a pristine extraction of the archived suite, it refused the suite for being
called `unnamed-suite`: a retained pin that requires the material to have already certified itself
certifies the self-certification. It now compares the digest **this run computed from the files it
read**, the name is the retained pin's to supply rather than the checkout's to assert, and a
checkout verified this way is reported as pinned — which a pristine third-party extraction, the
normal case, previously could not be.

**Authority and date.** The archive retrieved twice and extracted twice, the second independently
of the first; the harness's own digest over both; the run re-scored under the retained pin with
every figure unmoved, and again from the archived copy extracted into a directory carrying no pin
of its own; 2026-09-03.
