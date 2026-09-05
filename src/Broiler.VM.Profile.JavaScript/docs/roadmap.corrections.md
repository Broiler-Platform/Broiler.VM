# Broiler.VM.Profile.JavaScript roadmap — corrections and rejections

**Last updated:** 2026-09-05

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
| [JSC-69](#jsc-69) | `Fuzzing`'s remark on section 7's four surfaces; the ledger's JS-3b and JS-9 rows | Three of the four surfaces existed and the remark still said two, so the source front end was fuzzed by nothing while being described as absent | four sessions of 25,000 iterations over the source surface |
| [JSC-70](#jsc-70) | roadmap section 6's manifest allocation table, and the same table in [JSD-0002](decisions/0002-feature-manifest-allocation.md) | The table gains a row: `broiler.javascript.wide`, minted for bring-up, which is deliberately NOT the `core` row JS-5 was promised and which closes no milestone | [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md) |
| [JSC-71](#jsc-71) | roadmap section 7, and `JavaScriptFormat`'s remark that version 1 is the only version this build defines | There are two format versions, and what version 2 adds - a function table, an environment model, exception regions carrying a scope depth - is what a version break exists for | [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md) |
| [JSC-72](#jsc-72) | roadmap section 8 and the descriptor's five provisional rows | Two of the five are settled by CONSTRUCTION and not by benchmark: a declared poll bound is an upper bound on the largest single charge the profile makes | the poll-bound violation a bulk code-section read produced |
| [JSC-73](#jsc-73) | the descriptor's budget declaration matrix, and [JSD-0008](decisions/0008-descriptor-rows-that-are-provisional-and-why.md) | The `HostCalls` row is `Charged`, because one optional host capability import now exists - the flip JSD-0008 said JS-6 would make, arriving from another milestone | the descriptor, and the composition that registers the import |
| [JSC-74](#jsc-74) | registry rows 1003 and 1006, and rule N7's admitted set | Two rows stopped being unreachable: a defensive row's justification is a fact about the build, and registering a second format version and a second manifest expired both | two retained corpus entries, one per direction |
| [JSC-75](#jsc-75) | roadmap section 15 and [JSD-0017](decisions/0017-the-end-user-host-and-what-an-exit-code-promises.md) | The end-user host runs the wide surface by default, and naming several files runs them as separate scripts sharing ONE realm rather than as a sweep | the host's own closure report and its two workloads |
| [JSC-76](#jsc-76) | roadmap section 14 and the conformance composition's `--run` mode | A second mode runs a pinned third-party checkout this repository does not hold, with four verdicts rather than two, because an unadmitted construct is neither a pass nor a failure | the retained runs over named subtrees |
| [JSC-77](#jsc-77) | `JavaScriptFormat.MaximumFormatVersion`'s own remark | It is the version-1 reader's ceiling and not the descriptor's, and the two are different questions | the descriptor's declared range |
| [JSC-78](#jsc-78) | the retained corpus manifest's header and roadmap section 7's corpus discipline | One corpus holds entries of two format versions, distinguished by the replay mode column that already distinguished the nine entries bytes alone cannot produce | the regenerated corpus and its replay |
| [JSC-80](#jsc-80) | roadmap section 9's *strict mode is recognised by the tokenizer and ruled on by the validator*, as JS-3b's ledger row states it | Strictness a CALLER imposes reached the lowering and never the parse, so every strict-only early error was invisible in the one variant that exists to test it | the conformance variant that regressed when an unconditional refusal stopped masking it |
| [JSC-79](#jsc-79) | roadmap section 8's *`CallDepth` is measured, not chosen*; the gates' lifecycle clause | The interpreter recurses on the CLR stack, so the premise the bound rested on is false - and a conformance case terminated the process before anything refused it | the case that terminated it, and the declared stack that repairs it |

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
than argued, **and the first measurement was taken on the wrong platform**: about two minutes warm
on `win-x64` and over ten cold, dominated by hashing 56,560 files rather than by scoring 1,084 of
them. **On the Linux runner the lane uses it is about twelve seconds**, which is the figure that
decides whether the step belongs there, and it was the lane rather than the workstation that
produced it.

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

---

### JSC-69

**Where:** `Fuzzing`'s remark on which of roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s
four surfaces exist; the [ledger](roadmap.status.md#2-current-milestone-status)'s JS-3b row, which
named the gap, and its JS-9 row, which owns the sessions.

**What the record said.** "The section asks for coverage-guided fuzzing over four surfaces: the
verifier, the source tokenizer and parser, the regular-expression matcher, and the executor over
verified-but-adversarial artifacts. **Two of the four exist at this milestone** — the verifier and
the executor — and the other two are surfaces this profile has not written yet."

**What was actually true.** Three of the four existed. The source tokenizer and parser landed at
JS-3b — a tokenizer, a syntax tree, a recursive-descent parser, a validation stage and a lowering —
and the sentence above went unrevisited, so the surface was **fuzzed by nothing while being
described as absent**. The ledger's JS-3b row had already noticed the half of this that was its
own: *the front end is fuzzed by nothing — roadmap section 7's source tokenizer and parser now
EXIST, so JS-9's row can no longer say that surface is unfuzzed because it is absent.* What neither
row said is that the sibling session's own docstring was still asserting the absence.

**What replaced it.** A session over the source surface, in the slice-compiler root, which is where
it has to be: the execution-only image carries no lowering, so a session over source could not run
there at all. Same design as the sibling — a seeded mutator written out rather than a library's, a
bounded pool primed from the retained corpus, answer-guided keeping, and a guidance loop the
session proves before it reports any figure and the composition asserts as a named check.

**The assertion is not "it did not crash", and that is the point of the whole piece.** It is that
**a source this front end compiles produces an artifact this profile's own verifier accepts.** The
two stages check disjoint things by design — a refused source has no bytes, and the verifier reads
bytes whatever produced them — so the seam between them is exactly where a lowering can emit
something structurally wrong with nothing noticing: the front end's checks end at *it compiled* and
the verifier's begin at bytes somebody handed it.

**That seam is not hypothetical, and this component spent 2026-09-03 repairing three defects that
lived in it.** A loop whose body always breaks emitted a continuation nothing reached; a block was
lowered on after a statement control could not pass; and a loop nothing could leave left a program
tail no execution reached — refused as `UnreachableCode`, `UnreachableCode` and, once the tail was
suppressed, `JumpTargetNotAnInstructionBoundary`. **Every one is a source that compiled and did not
verify**, and every one was found by pointing a third-party suite at the host rather than by
anything here. A session asserting this invariant would have found all three without a suite.

**What the first two drafts got wrong, both found by running it rather than by reading it.** The
seed set was `*.js` under the corpus directory, which is 55 of the corpus's 57 entries: the two it
missed are retained as `generated` because they are a program of 65,536 declarations and one of
65,536 distinct constants, and they are the only sources that reach `TooManyLocals` and
`TooManyConstants`. Eight sessions over 200,000 mutants reached 21 of the 24 seam codes rather than
23. **The first fix was a mutation that built such programs itself**, which was the wrong
instrument twice over — the corpus already had generators for exactly this, and a 25,000-iteration
session went from two seconds to over ten minutes. It was backed out. What replaced it separates
two questions the first draft asked as one: **every entry is primed, and only entries under 64 KB
are drawn from.**

**What it reaches, and what it does not.** The corpus primes **23 of the 24 embedder-seam codes**;
the twenty-fourth is `OperandStackTooDeep`, which no source can reach because the parse depth bound
refuses at about 170 levels while the operand-stack ceiling needs more than a thousand — the
registry already records it defensive with that reason. The **mutator** reaches 21 of them, the two
limit codes being reached only by seeds too large to draw from. Four sessions of 25,000 iterations
found **no finding**, and about 26,000 mutants per session compiled and verified.

**And the guidance kept nothing, which is worth saying plainly rather than dressing up.** Across
100,000 mutants the seed set never grew: every answer a mutant reached, the retained corpus already
reached. That is the sibling's own stated expectation for a corpus that covers its surface, and it
means the *guidance* in this session is doing nothing today. What the session does that no fixture
does is check the seam invariant over tens of thousands of compiled programs.

**Authority and date.** Four sessions of 25,000 iterations at seeds 1 to 4, and eight earlier ones
that measured the seeding defect; the seam vocabulary of the published registry diffed against what
the sessions reach; 2026-09-03.

---

### JSC-70

**Where:** roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s
allocation table, and the same table and its admission criterion in
[JSD-0002](decisions/0002-feature-manifest-allocation.md).

**What the plan said.** Seven identities, fixed at JS-0, each with the earliest milestone that may
mint it: the slice at JS-1; `broiler.javascript.core` — "objects, prototypes, properties, closures,
functions, classes, exceptions, iteration, destructuring, strict mode, and the core standard
library" — at JS-5, with increments extending it; `modules` at JS-7; `dynamic` at JS-8; `regexp` at
JS-6 "or excluded with a published failure"; `intl` and `temporal` deferred until each has a run.
The table is "extendable by a later milestone but never silently widened", and JSD-0002's admission
criterion says what extending it costs: an increment mints **one** further manifest identity with a
reviewed scope, extends the retained malformed corpus, and re-runs the oracle against the ratchet.
Three rules govern it — increments do not inherit; a manifest with no retained run of its own is
not accepted; an increment closes no milestone and re-enters JS-5's vertical-slice loop.

**What replaced it.** The table gains a row. `broiler.javascript.wide` is minted for bring-up, and
format version 2 is defined against it ([JSC-71](#jsc-71)). It admits objects, prototypes,
properties, closures, functions, exceptions, strict mode and a standard library, and it refuses by
name at the front end every class, generator, `async` function, module, destructuring, spread,
template literal, tagged template, `for … of`, optional chain, `with`, Proxy, Reflect, Symbol,
BigInt, typed array, `eval` and `Function` constructor.

**The question worth arguing is not why the table grew but why the new row is not the row that was
already there**, because a reader looking at that list of admissions will reach for
`broiler.javascript.core` and the two are close enough that the difference has to be stated rather
than assumed. It is not `core`, for two reasons that are independent of each other and either of
which is sufficient.

**The first is the table's own column.** `core` is allocated to JS-5, and JS-5 has not started; the
column exists precisely so that an identity is not minted by whoever happens to need one. Minting
`core` here would not be extending the table — it would be spending a row the table had already
promised to a milestone, which is the *silently widened* case in the same sentence that authorises
extension at all.

**The second is that it would have contradicted a neighbouring milestone on the day it was
minted.** [JS-6's row](roadmap.delivery.md#19-milestones) requires the temporal,
internationalization and regular-expression surfaces to be minted as separate manifest identities
and **all three left out of `broiler.javascript.core`**. This manifest admits a `RegExp` — an
approximation, translated to `System.Text.RegularExpressions` and declared as one in the file that
does it, which is a second reason it does not belong under a name JS-6 owns. A surface with a
regular expression in it cannot be called `core` without the name being wrong in exactly the
direction JS-6 was told to keep it right.

**And widening `slice` in place was never the third option it looks like.** The slice's artifacts,
its retained corpus and its conformance fixtures all name `broiler.javascript.slice` and mean by it
what they meant on the day each was written; widening that identity would silently change what
every one of them claims, without editing any of them. A second name leaves all of them true.

**What this row does not buy, stated so no reader infers it from the fact that something now
runs.** It **closes no milestone** — rule 3 — and nothing in the ledger moves because it exists. It
**inherits nothing** — rule 1 — so nothing the slice demonstrated is evidence about this manifest,
and no argument of the form *the smaller manifest works, therefore this one does* is available to
anybody. Its scope is written down and **nothing here has been read by a human**, so the half of the
criterion asking for a *reviewed* scope is unmet and this entry does not claim it. And it has **no
retained conformance run of its own**: what exists is runs over subtrees somebody chose, which
measure those subtrees ([JSC-76](#jsc-76)). Section 6's sentence — *a manifest with no retained run
of its own is not accepted, and the support table says so* — is therefore unmet on the day the
manifest was minted, and it stays unmet.

**Authority and date.** The implementation of 2026-09-04 in this checkout, read against JSD-0002's
allocation table and its three rules, and against JS-6's row in the delivery order; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-71

**Where:** roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier), the format
subsection and the first of its three disciplines; and `JavaScriptFormat`'s own remarks.

**What the plan said.** "Format version 1 is defined with the first manifest and **grows with the
interpreter**", and the constant carrying the number calls itself "the only format version this
build defines". Section 7's corpus discipline says the retained malformed corpus **grows at every
milestone that grows the format**, and that it holds control entries that verify successfully,
because "a corpus in which nothing passes is a corpus that would not notice a verifier that rejects
everything".

**What replaced it.** A second format version, defined against `broiler.javascript.wide`
([JSC-70](#jsc-70)). Version 2 keeps version 1's framing unchanged — the same magic, a
variable-length version integer, a manifest identity, a declared section count, framed sections in
strictly ascending kind order, and one opcode plus a fixed operand — so a version-2 artifact is
refused by a version-1 reader **because the version integer differs, and not because a section it
did not expect turned up**.

**The plan's word was *grows*, and the correction is that this did not grow; it forked.** That is
the distinction the four additions are worth reading for, because each is a version break rather
than an extension, and for the same reason each time: a version-1 reader handed these bytes would
not fail to understand them, it would understand them as something else.

**A function table.** Version 1 declares one frame and one flat set of locals, which is what a
program with no functions needs. Version 2 declares one code unit per function — parameter count,
environment-slot count, operand-stack maximum, code range and flags — in a section of its own.
Section 7 already settled what that costs: adding a section to a frozen format is a format-version
break, which is why exception regions and suspension targets were framed from version 1 rather than
added when something needed them.

**An environment model addressed by depth and slot.** Bindings live in a chain of environment
records and are reached by a static (depth, slot) pair; nothing addresses a variable by name at run
time except a global, which is a property of an object and therefore a name by definition. This one
could not have been an extension under any framing, because it changes what an existing operand
*means*: the same index, the same width, a different referent.

**Exception regions that carry a scope depth beside the operand-stack height.** A handler that knew
neither would have to reconstruct both by walking back, and a handler entered at the wrong stack
height is exactly the defect a verifier exists to make unrepresentable. A version-1 reader parsing a
version-2 region would read the added fields as the beginning of the next one.

**Absolute branch targets rather than version 1's displacements.** All code units share one code
section, so an absolute target is checkable against the range of the unit that contains the branch,
which a displacement is not. And this is the addition that settles the general rule: the operand is
the same width in the same position and only its meaning moved, so there is no framing under which a
reader could notice it was reading the wrong format. **A change a reader can skip is an extension; a
change that re-reads bytes the reader already reads is a version.**

**The corpus discipline was honoured rather than cited.** The retained corpus gained one entry per
structural refusal version 2 adds — a function row the format cannot represent, an entry point
naming no code unit, code units that do not tile the code section, a scope popped past the frame, an
exception handler outside its code unit — two more where the caller mislabels the bytes
([JSC-74](#jsc-74)), and **one whole version-2 program**, a closure called through a property of an
object, which verifies, instantiates and completes with a value. The last of those is not padding:
the discipline asks for entries that pass, and a set of refusals alone would be replayed green by a
verifier that refused everything.

**Authority and date.** The implementation of 2026-09-04 in this checkout, read against section 7's
format list and its first discipline; the retained corpus manifest, which names the entries added
and the mode each replays under; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-72

**Where:** roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model)'s measurement
discipline, and the profile descriptor's own remarks, which name five provisional rows and the
milestone that settles them.

**What the plan said.** Five rows of the descriptor are provisional — the call-depth default and
maximum, the uncharged-work bound, the charging granularity and the cancellation poll bound — each
filled with a value that is safe rather than with a number that looks settled, and **JS-5 replaces
all five with numbers derived from a retained measurement**, because section 8 says each is measured
rather than chosen: "a number chosen from a measurement and recorded with it, not a round figure".

**What replaced it.** Two of the five — the cancellation poll bound and the uncharged-work bound —
are settled, and **not by a benchmark**. They are fixed by construction, which is the part worth
reading, because *measured by construction* reads at first like a euphemism for chosen.

**The argument runs through what the bound actually bounds.** The core's bounded reader charges one
work unit per byte consumed and polls after every charge. The meter reports a poll that arrives
after more work than the declared bound as a **poll-bound violation**. So the declared number is not
a statement about how often this profile would *like* to be interruptible — it is an upper bound on
the **largest single charge the profile makes**, asserted by the profile and checked by the meter
against the profile's own behaviour. Those are two different quantities, and one number had been
standing in for both.

**Which means the old value was not conservative; it was false.** A verifier that reads a
four-kilobyte code section in one call has made a single charge many times larger than a declared
bound of 256, and the meter says so in those words. That is not a hypothesis about a workload — it
is what the meter reported when a code-section read of that size met a bound of 256. **A profile
cannot honestly declare a bound smaller than its own largest read.**

**So the number and the behaviour are one fact stated twice.** The bound is 65,536; the verifier
reads every bulk run in windows no larger than the bound; and either half of that pair can be
derived from the other. A benchmark could not have produced this figure, because a benchmark
measures how long something takes and this row asserts something about what the code does — which is
the stronger derivation of the two, since a measurement is true of the machine it ran on and a
construction is true of the source.

**What a measurement would still be for, so this is not read as retiring the discipline.** Whether
polling this often costs anything is a latency question, and it is open; nothing here measured it.
What is settled is the direction the row constrains, and the row constrains it upward.

**The other three stay provisional and JS-5 still owns them.** The call-depth default and maximum
wait on a retained measurement of the per-frame cost on each claimed RID, which is a fact about the
size of a heap object rather than about the structure of a loop, and the charging granularity waits
with them. **A row settled by construction settles nothing about a row settled by a frame cost**,
and an entry that let two of the five carry the other three would be the same conflation this one
repairs.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the descriptor's two
rows and the verifier's windowed reads, each stating the other's reason — read against section 8's
measurement discipline and the descriptor's own account of its five provisional rows; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-73

**Where:** the profile descriptor's budget declaration matrix, and
[JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md), which dated
the correction of four rows JSD-0004 had intended charged and named the milestone that would flip
each.

**What the plan said.** The `HostCalls` row says `NotApplicable`, and the reason given is structural
rather than incidental: the slice **imports no host capability**, so the dimension is unreachable
rather than merely unused, and declaring it charged would be a claim the rest of the descriptor
contradicts. JSD-0008 recorded when that would stop being true: **JS-6 flips the host-call row when
the standard library imports something**, and JS-8 flips the three nested-load rows when
guest-initiated loads are declared.

**What replaced it.** `Charged`. The wide surface's standard library has a `print`, and `print`
reaches a host through one import, `broiler.javascript.write`, which carries one run of UTF-8 text.

**Why an import at all, when a sink somebody sets would have been shorter.** A static sink on the
profile type would be process-wide, would outlive a runtime, and would let one composition's output
reach another's — which is the ambient platform surface the core's capability table exists to
prevent. A `print` that reached a console without the composition having registered anything is that
surface with a friendly name on it. Registration is the permission, and there is no other door.

**The import is OPTIONAL, and that is the design rather than a hedge.** A composition that registers
nothing still creates a runtime and still runs programs; what it does not have is a `print` that
reaches anywhere. The profile asks whether the binding is bound and answers `undefined` either way,
so the difference between a host that shows output and one that does not is a registration rather
than a branch in the guest — which is also why the row is `Charged` outright rather than conditional
on a composition. The dimension is reachable; whether a particular image reaches it is that image's
business, and a matrix row is a statement about the profile.

**This is the flip JSD-0008 anticipated, and it arrived from a different milestone than the one it
named.** The record predicted the *event* exactly — the row moves when the standard library imports
something — and named JS-6 as the milestone that would carry it, and JS-6 has not started. That is
worth recording rather than smoothing over, because the same record's other prediction is still
outstanding in its original form: the three nested-load rows still say `NotApplicable`, guest loads
are still not declared, and JS-8 still owns them. A record whose first prediction landed early is not
a record whose second one has.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the descriptor's one
optional capability import and the matrix row that follows from it — read against JSD-0008's account
of the four inapplicable rows and the milestones it assigned them; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-74

**Where:** `docs/diagnostics/registry.txt`, rows 1003 `DescriptorFormatVersionMismatch` and 1006
`DescriptorManifestMismatch`; and rule N7, which holds the admitted set of codes no artifact
reaches.

**What the plan said.** Both rows are `defensive`, and had been since revision 1. The reason
recorded for each is the same, and it is about the shape of the build rather than the shape of the
code: the core screens an artifact's declared format version and feature manifest against the
descriptor **before this profile's verifier is called**, and this build registered exactly one format
version and admitted exactly one manifest — so each screen compared against a set with one member in
it, and a mismatch was not a thing a caller could construct.

**What replaced it.** Both are `corpus`, and each names the retained entry that reaches it: an
artifact whose bytes are version 1 announced as version 2, and an artifact carrying the slice
manifest announced as the wide one. Registering a second format version ([JSC-71](#jsc-71)) and
admitting a second manifest ([JSC-70](#jsc-70)) is the whole of what changed. **No line of the screen
was edited**, and that is the entry rather than an aside to it.

**The general reading is what to carry away, because it applies to every defensive row this registry
will ever hold.** A row marked defensive is making a claim about reachability, and the justification
for these two was never a property of the code — it was a property of a *set* that happened to have
one member. The code always compared the artifact's declaration against the descriptor's; what it
lacked was anything to disagree with. **So the justification expires when the build changes,
silently, with nothing failing.** A registry that never revisited its defensive rows would today
carry two rows asserting that a mismatch cannot be observed, in a file whose own header names two
format versions and two feature manifests — the contradiction sitting in one file, a few dozen lines
apart, with nothing on either side able to notice it.

**Which is why the admitting list lives in rule N7 and not in the registry.** A row claiming to be
unreachable is a claim held by a rule written where the row cannot reach it, so moving a row out of
that set is an edit to a rule rather than a change to a comment. The rows that remain defensive keep
reasons of the same kind — facts about what this build contains rather than about what its code can
do — and each is owed the same re-reading at the next change that widens what the build contains.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the registry at revision
3, its two rows and the retained corpus entries they name, read against rule N7's admitted set and
against the registry's own header; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-75

**Where:** roadmap
[section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding) and
[JSD-0017](decisions/0017-the-end-user-host-and-what-an-exit-code-promises.md), the end-user host and
what an exit code promises.

**What the plan said.** The host is the `narrow-runtime-compiler` composition and the first root that
earns the label; it is handed a path by a person; it compiles, verifies and runs
`broiler.javascript.slice`; and **it is not advertised and not packable**, because "a tool advertised
as a JavaScript host has to be able to run JavaScript, and `broiler.javascript.slice` admits no
function, no object, no string value and no property access". Several named paths were several
programs: each was run, each got a realm that did not outlive it, and the host reported the worst
answer any of them gave.

**What replaced it.** `broiler.javascript.wide` by default, the slice behind `--slice`, and — the
part a reader would otherwise be surprised by — **naming several files runs them as separate scripts
sharing one realm, in order**, rather than as a sweep with a realm each.

**The reason is that both target workloads are shaped that way and neither can be expressed
otherwise.** A benchmark harness and its benchmark, or a conformance harness and its test, are
separate *scripts* that share a global object: the first defines what the second uses. Under a realm
each, the second file cannot see the first, and every workload of that shape fails for a reason that
is about the host rather than about the program.

**And concatenating them into one script is a real defect rather than a shortcut**, which is worth
arguing because it is the obvious cheap answer and it is wrong twice over. **It changes what a
directive prologue means**: a prologue is only a prologue at the head of a script, so joining two
files demotes the second file's `"use strict"` to an ordinary expression statement that does
nothing, and promotes the first file's into a rule over everything after it — in both directions,
silently. **And through that it changes `this` inside a constructor**: a function called without
`new` sees the global object in sloppy mode and `undefined` in strict, so a harness whose error type
assigns to `this` starts writing to a different object, or throws, depending on which file it was
joined with. Neither is a syntax error. Both are a wrong verdict.

**One realm and several scripts is what the format already provides**, rather than a convention the
host has to keep: the wide surface's artifact carries one code unit and one named entry point per
script, the instance is the realm, and an instance outlives one invocation. The host invokes in
order against one instance and gets the required shape from the artifact.

**A directory argument is still a sweep, and that asymmetry is deliberate.** A tree is not a program:
one file's globals must not decide the next one's result, and a sweep that shared a realm would
report a distribution in which every row after the first is contingent on the rows before it. Named
files are what a person composed; a directory is what a person pointed at.

**The host is still not advertised and still not packable, and only the reason has moved.** It used
to be that the manifest admitted no function; it is now that the manifest refuses by name a long list
a reader would expect a JavaScript host to run — every class, generator, `async` function, module,
destructuring, spread, template literal, tagged template, `for … of`, optional chain, `with`, Proxy,
Reflect, Symbol, BigInt, typed array, `eval` and `Function` constructor — that `RegExp` is translated
to `System.Text.RegularExpressions` and is an approximation declared as one in the file that does it,
that `Date` fixes the local time zone to UTC, that `arguments` is unmapped, that script-level `let`
and `const` become properties of the global object so a read before the declaration answers
`undefined` instead of throwing, and that there is no job queue at all. **A tool advertised as a
JavaScript host still has to be able to run JavaScript**, and a manifest that refuses a named list is
no more advertisable than one that refused everything — it is only likelier to be mistaken for one
that does not. The exit-code contract JSD-0017 records is untouched.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the host's default
surface, its `--slice` flag and its one-realm rule, read against JSD-0017's account of what the host
runs and why it is not advertised; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-76

**Where:** roadmap [section 14](roadmap.md#14-the-conformance-oracle), and the conformance
composition's `--run` mode.

**What the plan said.** The harness scores this component's own fixture trees and nothing else. That
is section 14 asking for exactly what it got — the harness is built "against the smallest scoring
target that exists rather than after the language it will eventually score" — and it is also the
ledger's own reading of what every figure the harness had produced was worth: a statement about the
instrument rather than about JavaScript, because the declarations and the code that judges them have
one author. No third-party suite was scored by anything here.

**What replaced it.** The pin exists and the suite is retained as an archive ([JSC-68](#jsc-68)), and
the composition gained a second mode: `--test262 <root>` runs an unpacked checkout of it, with
`--test`, `--dir`, `--limit`, `--fuel` and `--wall`. Three things in it are decisions rather than
mechanics, and each would be wrong in an interesting way if taken the other way.

**The harness files are evaluated as SEPARATE scripts in the test's realm, because INTERPRETING.md
requires exactly that.** The suite's own contract says so, and [JSC-75](#jsc-75) argues at length why
concatenation is a defect rather than a shortcut — it moves a directive prologue and, through it,
`this` inside a function called without `new`, which is precisely what a suite's own error type
depends on. This runner gets the required shape from the format rather than from a convention it has
to keep: one code unit and one named entry per script, and the instance is the realm.

**A fresh realm per variant, because tests destroy the harness they were given.** They redefine
`Object.prototype` members, freeze intrinsics and replace `assert`; a runner that reused a realm
would score the next test against the previous one's wreckage, and would do it in an order-dependent
way that a rerun of one test would not reproduce. A realm here is an instance, so a fresh one costs a
runtime and nothing more — which is what makes the honest choice also the cheap one.

**And a fourth verdict, UNSUPPORTED, beside pass, fail and skipped.** A front end that refuses a
class declaration **has not found a syntax error** — it has found a construct it does not implement —
and a test that expects a `SyntaxError` would be scored a pass on that refusal. Counting it would
turn every unimplemented feature into a point, and the more the manifest refused the better the total
would read. This is section 14's own rule — a refusal answers a question about the language only when
it was a language answer ([JSC-54](#jsc-54)) — reaching a second harness by a different route.
Unsupported is neither a pass nor a failure and is reported on its own. Module tests,
`resolution`-phase negatives and the `CanBlockIsFalse` flag are declined by name rather than run and
scored, and an asynchronous test cannot complete at all, because there is no job queue.

**The mode scores nothing on its own, and this entry claims nothing on its behalf.** `--test262`
takes a root and whichever subtrees somebody names, and **a run over a list somebody chose is a
measurement of that list** — not of the suite, not of the manifest, and not of this profile. It sets
no floor and admits no total. Section 6's rule that a manifest with no retained run of its own is not
accepted is exactly the rule this does not answer ([JSC-70](#jsc-70)): what a whole-suite run would
settle a named subtree cannot, and the difference is not one of size but of who chose the population.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the composition's second
mode, its four verdicts, its per-variant realms and its separate-script evaluation — read against
section 14's method and against the suite's own INTERPRETING.md at the retained pin; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.

---

### JSC-77

**Where:** `JavaScriptFormat.MaximumFormatVersion`, in the format assembly, and its own summary.

**What the plan said.** That the constant is "the highest format version the profile descriptor
accepts".

**What replaced it.** It is the highest version **the version-1 reader** accepts. The descriptor's
range now runs from the minimum up to format version 2's number ([JSC-71](#jsc-71)), which is wider
than this constant, and the constant is the screen one verifier applies to bytes it is about to parse
as version 1.

**The two are different questions, and the name answers the one it is not about.** What this build
admits is a fact about the descriptor; what a reader can parse is a fact about that reader. They were
the same number for as long as there was one reader, and a name written while they agreed reads,
once they diverge, as an assertion that they still do.

**Nothing here is a defect in the code.** The constant is used where it belongs, its value is right
for what it does, and no artifact is admitted or refused differently because of this entry. What is
corrected is a reading: somebody who trusted the name would conclude this build admits one format
version, and the same assembly's neighbour says otherwise.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the descriptor's
format-version range read against the constant and against the verifier that uses it; 2026-09-04.

---

### JSC-78

**Where:** the retained corpus manifest's own header, and roadmap
[section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s corpus discipline.

**What the plan said.** The header names one manifest and one format version — the slice, at format
version 1 — and the discipline says the corpus grows at every milestone that grows the format. Read
together they say what the corpus is: the retained record of one manifest at one version.

**What replaced it.** One corpus, one manifest, one integrity check over it, holding entries of
**both** format versions and distinguishing them by the replay **mode** column. The `format version
1` in that header is the sentence this entry retires; the discipline beside it is untouched, and is
what produced the new entries ([JSC-71](#jsc-71)).

**The mode column was already carrying information of exactly this kind, which is why it was the
right place.** It is not a description of the bytes — it is what the replay must *do* — and it
already held every case a row's bytes cannot express on their own: a token cancelled before the read
begins, a profile the catalog does not hold, and one entry per ceiling a host declined — each of
which asks something of the replay that no byte of an artifact can state. Which format a row's bytes
are is an instruction of that shape: the replay has to know which reader to hand them to, and no
byte of the artifact tells it anything the column does not.

**Why one corpus rather than two, which is the choice a reader would otherwise wonder about.** The
integrity check is over **the manifest**, not over the directory: a mutated entry is caught because
the manifest records its hash and the replay recomputes it, and the control that proves this injects
into the retained bytes rather than into source. A second corpus would therefore be a second
manifest, a second integrity check, a second header to keep honest, and a second place the replay has
to be told to look — four things to keep in step where there had been one, in exchange for a
separation nothing needs. **And the replay's whole job is to compare an observed triple against a
recorded one across three publish modes.** Splitting the record by format version splits that
comparison along an axis that is not the comparison's, so a drift affecting both versions at once
would be reported twice, with nothing relating the two reports.

**What this does not claim.** The corpus is a corpus: it records what the verifier and the executor
answered and holds them to it. Growing it across a format version shows that the discipline was
applied, and no more than that.

**Authority and date.** The implementation of 2026-09-04 in this checkout — the retained corpus
manifest, its mode column and the integrity check over it — read against section 7's corpus
discipline and against the header the manifest carried; decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md); 2026-09-04.
---

### JSC-79

**Where:** roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model)'s *`CallDepth` is
measured, not chosen*, and the lifecycle clause of
[gates section 21](roadmap.gates.md#21-format-and-verifier-safety) that says a call-stack overflow
is reported as a resource exhaustion and is not fatal.

**What the plan said.** That the bound could be promised at all *because of how a frame is stored*:
"Because a frame is a heap object rather than a CLR frame, the bound is a **counted number compared
against a limit** and not a stack probe — which is what makes it promisable under Native AOT at
all." The gates say the same thing from the other side, and name the failure it is protecting
against as a stop condition: a process termination on a nesting case blocks the milestone, because
a stack overflow is not translatable and claiming to handle it would be an untruthful capability
claim.

**What was actually built.** **The wide surface's interpreter recurses on the CLR stack.** One
JavaScript call is one C# call - `Call`, then `Invoke`, then `Execute` - and a JavaScript frame is
those three CLR frames plus a heap-allocated operand stack and environment. The plan's premise is
therefore false for this executor, and the sentence that rests on it does not carry: a counted
bound compared against a limit is still what the code does, but what it protects is a *native*
stack rather than a heap the profile controls.

**And the consequence was observed rather than reasoned about.** A conformance case that recurses a
hundred thousand deep - `test/language/statements/if/tco-if-body.js`, one of a family testing
tail-call optimisation - **terminated the process** at 776 frames on the one-megabyte stack a
Windows process hands its main thread. Neither the call-depth ceiling nor the CLR's own
sufficient-stack probe reached it first: the ceiling stood at a thousand, and the probe reserves a
fixed margin that an interpreter frame of over a kilobyte can step past between two calls. **That is
exactly the stop condition the gates name**, found by pointing a third-party suite at the host and
not by anything here.

**What replaced it.** Two changes, and the first is the one that makes the second promisable.

**The profile runs a guest invocation on a thread whose stack it declares** - sixteen megabytes -
rather than on whatever stack the caller happened to have. A call-depth bound that means one thing
on a host's main thread and another on a thread pool's is not a bound the profile can promise, and
the depth at which a process dies is not a property anybody can measure once and rely on. Choosing
the stack is what turns the bound into a property of this profile rather than of its caller.
The profile declares `Agile` thread affinity, so the core pins no operation to a thread, and the one
host capability it imports declares caller-thread affinity - which this satisfies, because the
thread that calls it is the thread the guest is running on.

**And the ordinary answer to a recursing program is the budget, not the backstop.** The engine's own
ceiling is now higher than the `CallDepth` default, so a recursing program is refused by the
dimension the gate names, as a resource exhaustion the guest cannot catch. The engine's ceiling
remains beneath it as the answer for a host that granted more call depth than the declared stack can
hold, and it is a `RangeError` - which is what the language says and what a program that catches one
expects.

**What is still open, stated rather than implied.** The gate asks for the refusal **on every claimed
RID under Native AOT**, and this component publishes Native AOT in a lane and retains nothing from
it; the observation above is one machine, one RID, JIT. The per-frame cost is not measured and the
sixteen megabytes is chosen against an estimate of it rather than against a retained measurement,
which is the same discipline [JSC-72](#jsc-72) satisfied for two other rows and does not satisfy
here. And the plan's own answer - a heap frame model, which section 8 lists among the rows JS-4 must
settle before it closes - is not built and is not withdrawn: this entry records that the executor
that exists does not have one, not that the design was abandoned.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the conformance case
that terminated the process, and
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md). 2026-09-04.

### JSC-80

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s answer that
strict mode is **recognised** by the tokenizer and **ruled on** by the validator, as JS-3b's row of
[the ledger](roadmap.status.md) states it.

**What the plan said.** That strictness has two owners and both are inside the front end - the
tokenizer recognises it, the validator rules on it - and that this is what deletes the seed's source
re-scans rather than reimplementing them. Nothing in that reading is wrong about a program that
declares its own strictness in a directive prologue.

**What was actually built.** A third source of strictness that neither owner could see. A caller can
impose strict mode without the source asking - `JsScriptUnit.ForceStrict` - and a conformance runner
does exactly that to produce the strict variant of a test flagged `onlyStrict`. **That flag reached
the LOWERING and never the parse.** The parser's own strictness came from the directive prologue and
the module goal alone, so a force-strict variant was *parsed as sloppy* and only lowered as strict.

**Why that is not a detail.** Strict mode changes the **grammar**, not only the semantics. `yield`
becomes a reserved word; a legacy octal literal becomes a syntax error. Both are **early** errors,
and an early error is precisely what a lowering never gets to see, because the parse it would have
had to fail has already succeeded. So every strict-only early error was invisible in the one variant
that exists to test it, and the profile answered a whole class of conformance cases by running a
program the language says must not parse.

**How it was found, which is the part worth keeping.** It was found by a REGRESSION, and only
because something else was repaired first. `await` and `yield` had been refused unconditionally as
constructs outside the manifest - wrong, since both are contextual keywords and `var await = 1` is
an ordinary program in a script. Admitting them where the language admits them made one conformance
case go from pass to fail: a test asserting that `yield` is a reserved word **in strict code**,
which had been passing because the refusal fired regardless of strictness. **The unconditional
refusal had been standing in for the missing rule and hiding its absence**, and the figure it
produced was right for the wrong reason. No audit of refusals could have found this; only making
one of them conditional could.

**What replaced it.** Strictness imposed by a caller now reaches the parse, and the parser states
the invariant it always relied on: nothing turns strictness off. A prologue can add it, the module
goal can add it, and a caller that imposed it keeps it - so an inner function without a directive
cannot undo an outer one that has it, and cannot undo the caller either.

**What is still open, stated rather than implied.** The strict-only early errors this reaches are
the ones the front end already knew how to report - a reserved word as a binding, and a legacy
octal. **The rest of strict mode's early errors are not implemented and this does not implement
them**: duplicate parameter names, `delete` of an unqualified name, `with`, assignment to an
undeclared name at parse time, and octal escapes in strings are each their own rule, and none of
them is written. What changed is that strictness is now *knowable* at parse; what is not claimed is
that everything knowable is checked.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the conformance
variant `test/language/future-reserved-words/yield-strict.js`, whose movement in both directions is
in [Bundle JS-4-001](evidence/js-4-001/README.md).


### JSC-81

**Where:** the workload roadmap's
[section 3.4](roadmap.workloads.md#34-the-two-failures-that-are-defects-rather-than-absences), which
states that `pdfjs` is refused by this component's own verifier on bytes this component's own
lowering produced, and asks which of two components was wrong.

**What the plan said.** That the answer was one of two: *either* the lowering emits something the
format does not admit, *or* the format admits something the verifier's semantic stage then rejects.
The stage's objective is written around that fork, and its first clause is to decide it.

**Which of the two it was.** **Neither, and the fork was drawn one level too high.** The lowering
emitted only instructions the format admits and the verifier decodes, and the verifier's semantic
stage was right to refuse them. What was wrong was the *composition* of instructions the lowering
chose for one construct: an array literal that is not dense-and-under-a-thousand-elements is built
element by element and then has its `length` set, and `SetProperty` **pops a value and a base and
pushes the value back**. Setting `length` on the array under construction therefore replaced the
array with the count, and the `Pop` that followed discarded the count — leaving the literal
expression with **nothing** on the operand stack where every caller expects one value. The verifier
reported an operand-stack underflow, at whatever later instruction first popped one value too many,
which is a position a long way from the cause.

**What replaced it.** The array is duplicated before its `length` is set, so the literal leaves
exactly one value. Section 3.4's fork stands as a question a reader should ask; what this entry adds
is the third answer it did not offer — **a lowering that is internally inconsistent while emitting
nothing the format or the verifier could object to on its own**. A stage that had only looked for a
disagreement between the two components named would not have found this.

**What the repair does not do.** It does not make the verifier's position any nearer the cause. A
version-2 artifact carries a position table that the verifier parses and discards, so a refusal
still names a code-section offset and never a line — which is how a defect in eleven lines of
JavaScript took a benchmark of thirty-three thousand to expose.

**Retained as a fixture rather than as a benchmark.** `src/tests/cli/runs/an-array-literal-with-holes.js`
is the same construct with no third-party file behind it, and its rows in
`src/tests/cli/expected.txt` assert the answers rather than the exit code alone.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the Octane `pdfjs`
run through the ordinary command line that moved from an exit-4 refusal to a named absent
constructor. 2026-09-04.

### JSC-82

**Where:** the workload roadmap's
[section 3.4](roadmap.workloads.md#34-the-two-failures-that-are-defects-rather-than-absences),
which records the `typescript` benchmark failing with a type error against a value the program did
not expect to be `undefined`, and the ledger's
[section 2](roadmap.status.md#2-current-milestone-status) statement that **`arguments` is unmapped**.

**What the plan said.** That the divergence in this area is the *mapping* — that the arguments
object of a sloppy-mode function does not alias its parameters, which is a declared approximation
and an observable one. Nothing said the binding itself could be wrong.

**What was actually built.** A function whose formal parameter list contains the name `arguments`
had that parameter's value **destroyed on entry**. The lowering declares the parameters into slots,
then declares `arguments` for the object — and the compile-time scope answers a repeat declaration
with the slot it already has, so the object was written into the parameter's own slot before the
first statement ran. The actual the caller passed was simply gone. The specification says the
opposite from the other end: function declaration instantiation sets `argumentsObjectNeeded` to
false when `arguments` is one of the parameter names, precisely so that the parameter is the
binding.

**How it was found, which is the part worth keeping.** By a machine-generated program, exactly as
[section 3.4](roadmap.workloads.md#34-the-two-failures-that-are-defects-rather-than-absences)
predicted such a defect would be. The Octane TypeScript benchmark carries a compiler with
`function FuncDecl(name, bod, isConstructor, arguments, vars, scopes, statics, nodeType)` and a
body that reads `this.arguments`, so `arguments.members.length` read a property of the arguments
object and threw. **No hand-written test in this repository would have contained that function**,
and the shape is not exotic: naming a parameter `arguments` is legal sloppy-mode JavaScript that a
code generator has no reason to avoid.

**What replaced it.** A parameter named `arguments` suppresses the object entirely. A `var
arguments` or a function declaration of that name is deliberately **not** this case: each is
initialised after the object is, which is the order the specification asks for and the order the
lowering already produced.

**What is still true, and must not be read as repaired.** The arguments object is still **unmapped**
— writing `arguments[0]` does not change the first parameter and vice versa. That divergence is the
ledger's and this entry does not touch it.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the Octane `typescript`
benchmark moving from a type error to a score through the ordinary command line, and the fixture
`src/tests/cli/runs/a-parameter-named-arguments.js`. 2026-09-04.

### JSC-83

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s account of the
front end's static walks, and the same ledger sentence [JSC-82](#jsc-82) cites.

**What the plan said.** Nothing that was wrong, and that is why this is a correction to a *reading*
rather than to a sentence. The walk that decides whether a function must materialise an `arguments`
object stops at every function-like node, because a nested function has an `arguments` of its own
and a mention inside one is not a mention of the enclosing function's. That reading is correct for
every function-like node **except one**.

**What was actually built.** An arrow function is parsed as a function expression carrying an arrow
flag, so the walk stopped at arrows too. An arrow has **no `arguments` of its own**: a mention
inside one is a mention of the enclosing function's, which is the whole of what makes
`function f() { return () => arguments[0]; }` a legal and ordinary program. The enclosing function
therefore declared no slot, the inner reference fell through to a global read, and the program threw
a `ReferenceError` naming `arguments` at run time — a refusal that looks exactly like an absent
global rather than like a defect in a walk.

**What replaced it.** The walk descends into an arrow's body and stops only at an ordinary function.

**Why it is recorded beside [JSC-82](#jsc-82) rather than folded into it.** They are two defects in
one binding, found by the same repair session, and only the first was reachable from either
workload. The second was found by asking what *else* the same walk answers wrongly, which is the
habit — not the benchmark — that produced it, and a record that collapsed them would say the
workload found both.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the fixture
`src/tests/cli/runs/a-parameter-named-arguments.js`, whose fourth and fifth lines are the arrow
cases. 2026-09-04.

### JSC-84

**Where:** the workload roadmap's [section 1](roadmap.workloads.md#1-the-target-stated-as-behaviour-rather-than-as-a-score),
which states the Octane target as *every benchmark reports a score through the ordinary command
line*, and roadmap [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core).

**What the plan said.** That the gap between this profile and a whole Octane run is a gap in the
**language** surface, and that section 3.1's table — a manifest identity per row — names all of it.

**What one benchmark showed instead.** `zlib` carries an asm.js module whose emscripten runtime
decides which host it is on by asking for `window`, `process` and `importScripts`, concludes "a
shell" when it finds none, and then reads the global **`read`** to wire into its own module object.
It never calls it. That is not a language surface and no manifest owns it: it is the **host shell**,
and a profile that answered a `ReferenceError` there was refusing a whole program over a capability
the program does not use.

**What replaced it.** The realm has a `read` that exists and refuses, in the shape
`$262.agent`'s members already established: answering `undefined` would let a program proceed on a
false premise, and refusing to exist makes an environment probe fail rather than answer.

**And why it refuses rather than working, which is the part that belongs in section 18.** A value
capability takes bytes and answers a `long` or an opaque reference, and an opaque reference is by
construction not dereferenceable. **There is no registration any composition could make that would
let a host answer a guest with a file's contents.** That is a limit of core contract version 1, not
a decision of this profile's, and it is the first amendment this profile has an observed reason to
ask for: a value capability that answers with bytes.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the Octane `zlib`
benchmark, whose failure moved from a language absence to this. 2026-09-04.

### JSC-85

**Where:** roadmap [section 8](roadmap.md#8-the-value-frame-and-call-model)'s *`CallDepth` is
measured, not chosen*, the workload roadmap's
[JSW-9](roadmap.workloads.md#jsw-9--the-depth-a-generated-program-needs), and
[JSC-79](#jsc-79), which recorded the repair this entry corrects.

**What the plan said, and what JSC-79 left standing.** That a recursing program is refused as a
resource exhaustion naming `CallDepth` and never terminates the process; and that the repair for the
conformance case that DID terminate the process was a counted bound compared against a limit.
[JSC-79](#jsc-79) recorded what that repair did not do: *measure the per-frame cost the declared
stack was chosen against*. The bound stood at three thousand frames on a sixteen-megabyte stack.

**What the measurement found.** Two things, and the second is the one nobody would have guessed.

**First, the per-frame cost.** `eng/measure-frame-cost.py` bisects the published binary against a
recursion with no base case, raising the call-depth ceiling one step at a time and asking of each
child process whether it ANSWERED or DIED. This interpreter survives **8,666** JavaScript calls on
the declared stack: **1,936 bytes of native stack per call** — and the same figure for a frame
holding nineteen live values across a call as for one holding none, because the operand stack and
the environment are heap objects and only `Call`, `Invoke` and `Execute` are on the stack. So the
three-thousand-frame bound was not too generous. It was **too small by a factor of nearly three**,
which is a defect of a different kind: it refused programs the stack could have run.

**Second, and this is the correction.** With the bound at three thousand, a recursion **terminated
the process at three thousand frames** — on a stack holding 8,666 of them. Not because the frames
did not fit, but because **the refusal did not**: the bound was reported by throwing a
`RangeError`, and building and dispatching that exception from that depth needed stack the program
had spent getting there. The bound was checked before the runtime's own stack probe, so the probe
never ran. **A counted bound that reports itself by throwing is a bound whose safety depends on the
cost of the throw**, and nothing in the plan said so because nobody had measured it.

**What replaced it.** Three changes that only make sense together. The backstop **ends the
operation as a resource exhaustion** rather than throwing a value the guest could catch, which is
what section 8 asked for in those words and what makes the report survive the depth it reports on.
The backstop is set to 6,000 — a third short of what the stack holds. And the profile's declared
call-depth **maximum** comes down from 16,384 to 4,096, which is short of the backstop, so the
ordinary answer is always the budget ceiling's named exhaustion and the backstop is reached only by
a host that granted more than the profile said it could. Sixteen thousand frames was four times what
the stack holds and had never been reached by anything.

**What is still open.** The measurement was taken on one machine, on Linux, under the JIT. The gate
asks for the recursive workload under Native AOT on every claimed runtime identifier, and that is a
collection rather than a change to this checkout; until one exists, the figures above are a
measurement of this configuration and the margins are what carry the rest.

**Authority and date.** The measurement of 2026-09-04, retained as `eng/measure-frame-cost.py` and
reproducible against `src/tests/cli/limits/an-unbounded-recursion.js`, and the four acceptance rows
that pin the answers. 2026-09-04.

### JSC-86

**Where:** roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s
allocation table and its rule that *a well-formed artifact that uses a construct outside its declared
manifest is rejected at verification*.

**What the plan said.** That a feature manifest is a thing an artifact **names**, one per artifact,
and that the rule above follows from that: a composition declines a manifest by not accepting it,
and an artifact naming it is refused before it runs. The table listed seven identities and read as
though every one of them worked that way. The whole of the reasoning is sound for a surface made of
**constructs** — a module declaration, a `class`, `eval` as a spelled call site — because the front
end refuses those by name and they never reach an artifact at all.

**What implementing two of the identities showed.** It does not work for a surface made of
**globals**. A program that constructs a `Uint8Array` is, byte for byte, a program that reads a
name; there is no construct to refuse, no section that says which names matter, and an artifact
using the binary surface is indistinguishable from one that does not. A composition could decline
`broiler.javascript.binary` and the artifact would verify and then meet an absent constructor at run
time — which is exactly the run-time refusal section 6 distinguishes a declined manifest FROM.

**What replaced it.** A second kind of identity, named as such in section 6. An **optional surface**
is declared by the artifact **beside** the manifest it names, in a section of its own that the
verifier reads and refuses an unadmitted entry of. The lowering records a surface when a free name
that belongs to one resolves to a global; a `typeof` deliberately records nothing, so
`typeof Uint8Array === "undefined"` stays a question rather than becoming a refusal. A composition
declines by naming the surfaces it admits when it builds the descriptor it registers, and there is
no other door.

**What that costs, stated plainly.** The artifact format grew a section kind and the diagnostic
registry grew three codes — one for a surface declared twice, one for a surface this build does not
implement, one for a surface the composition declined — and the three are distinguished because a
reader of a refusal should not have to guess whether the artifact or the composition was the reason.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the three retained corpus
entries `wide-a-surface-declared-twice`, `wide-a-surface-this-build-does-not-implement` and
`wide-a-surface-the-composition-declined`, and the four composition-root checks that record what
each configuration answers. 2026-09-04.

### JSC-87

**Where:** the workload roadmap's
[section 3.1](roadmap.workloads.md#31-what-each-workload-meets-today), titled *what each workload
meets today*, and [section 3.2](roadmap.workloads.md#32-the-surface-that-is-absent-from-the-realm)'s
list of what is absent from the realm.

**What the plan said.** Both sections state facts in the present tense about a checkout, which is
what makes them checkable — and what makes them go out of date the moment the work they describe is
done. Section 3.1 named, for each workload, the absence it met and the stage that owned it; section
3.2 named the globals the realm did not have.

**What replaced it.** Both now carry **two readings**: what the workload met when the document was
written, and what it meets now. That shape is deliberate and is not tidiness. The document's own
method is that "every row above is a behaviour, and every one of them is reproducible" — a row
rewritten in place would still be reproducible and would have destroyed the only evidence that the
programme did anything. A reader who wants to know whether the binary surface was worth building
needs both columns.

**One row changed kind rather than closing**, and the document says so where a reader meets it:
`zlib` no longer meets a language absence, it meets a **shell** the benchmark assumes — the global
`read` — which no manifest in section 6's allocation owns and which core contract version 1 has no
shape for ([JSC-84](#jsc-84)).

**What is NOT corrected.** Sections 1 and 7 are unchanged. The target is still that the suite RUNS
with the `unsupported` column empty for the claiming manifest, no stage's exit gate has been
accepted by anybody, and nothing here moves a row in the ledger — which section 5's own preamble
says and which stays true however much of section 5 is built.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the Octane runs
through the ordinary command line that produced the second column. 2026-09-04.

### JSC-88

**Where:** roadmap [section 10](roadmap.md#10-the-executor-and-the-realm)'s abstract operations, and
the property [JSC-79](#jsc-79) and [JSC-85](#jsc-85) both exist to hold: **a refusal is an answer and
a process termination is not one.**

**What the code said.** `ToNumber` named the five primitives it converts and sent everything else
through `ToNumber(ToPrimitive(value, "number"))`. That is the specification's own shape and it was
right for as long as the only remaining case was an object.

**What `Symbol` did to it.** `ToPrimitive` of a primitive is that primitive — it is the operation's
first clause. So a Symbol reaching that arm converted to itself, for ever: not a hang, not a budget
exhaustion, but a stack overflow, which is the one failure the runtime cannot turn into an exception
and the one outcome this profile may never produce. `+Symbol()`, `Symbol() - 0`, `Math.abs(symbol)`
and every other numeric coercion terminated the process. The suite found it three times in one
afternoon — `built-ins/Array`, `built-ins/Symbol` and `built-ins/TypedArray` each died mid-run — and
nothing written in this repository had.

**What replaced it.** `ToNumber` refuses a Symbol **by name**, exactly as `ToString` already did,
and the reason is the same one rather than a defensive addition: a Symbol is a key nobody can forge,
and a key that silently became a number would be forgeable by arithmetic.

**The shape of the defect is worth more than the defect.** `ToString` had the Symbol arm because
`String(symbol)` is a case somebody thought about; `ToNumber` did not, because `Number(symbol)` is a
case nobody writes. **A conversion table with a recursive default arm is safe only while the set of
primitives is closed**, and JSW-6 opened it. Any future primitive — `BigInt` is the one this profile
expects — must be given its arm in both operations at the moment the type is added, not at the
moment a suite dies of it.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the twelve-case
differential probe against the comparison engine, and the three test262 subtree runs that terminated
before it and completed after. 2026-09-04.

### JSC-89

**Where:** [JSW-5](roadmap.workloads.md#jsw-5--the-core-language-surface-still-refused-by-name)'s
clause that a family moves from *refused by name* to *admitted and exercised*, **and no family moves
to refused as an unexpected token on the way**.

**What the front end did.** `new.target` was admitted and parsed, and the parse **returned** it from
the call-chain reader rather than handing it to the loop that reads member accesses and calls. So
`new.target` alone was right and `new.target.name` was a syntax error at the `.`, reported as an
expected-token diagnostic — a construct the language admits, refused as an unexpected token, by the
very stage that had just admitted the family.

**What replaced it.** `new.target` joins the suffix loop like any other head. It is a
*MemberExpression* in the grammar and not a finished expression, which is what makes
`new.target.name`, `new.target === C` and `new.target.prototype` ordinary.

**Why it survived a bundle that verified the family against a comparison engine.** Every probe case
used `new.target` bare or compared it, and none read a property off it. **A family's audit has to
walk the syntactic positions the construct can appear in, not the ones a probe author thought of** —
which is the audit section 4 of bundle JS-4-001 describes, and this is the second time that audit
has been the thing that would have caught a refusal produced for the wrong reason.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the differential probe
that reported `E` for `new.target.name` under the comparison engine and a syntax error here. 2026-09-04.

### JSC-90

**Where:** the workload roadmap's
[JSW-10](roadmap.workloads.md#jsw-10--the-runs-per-manifest-whole), whose objective is a run over the
Octane checkout in which **every benchmark reports a score through the ordinary command line**.

**What the host offered.** Three of the four ceilings a person meets were settable from the command
line — `--fuel`, `--wall` and, since [JSC-85](#jsc-85), `--call-depth`. The memory allowance was not,
so the profile's default — sized for a program a person types — decided which workloads could be run
at all.

**What that produced, which is worse than a refusal.** `zlib` printed its score and *then* met a
`LiveBytes` ceiling, so the process exited non-zero on a run that had produced exactly the thing the
target asks for. A caller reading the exit code was told the run failed; a caller reading the output
was told it succeeded. **Neither reading was wrong, which is what made it the least useful of the
outcomes.**

**What replaced it.** `--live-bytes <n>`, beside the other three, with the same shape and the same
refusal for a value that is not a positive count. It widens what a **caller may ask for** and not
what the profile permits: the profile's hard maximum still bounds it, and a composition that wants a
smaller ceiling still gets one.

**What is NOT corrected.** The default is unchanged. A benchmark needing a gigabyte is a fact about
the benchmark, and moving the default to accommodate it would make every program this host runs pay
for one workload's working set.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the `zlib` run that
scored and then exhausted. 2026-09-04.

### JSC-91

**Where:** the workload roadmap's
[section 3.2](roadmap.workloads.md#32-the-surface-that-is-absent-from-the-realm), which lists what
is absent from the realm, and [JSW-6](roadmap.workloads.md#jsw-6--the-core-library-still-absent-from-the-realm),
whose objective is that the core library stops being absent.

**What both said.** That what the realm lacked was a set of **globals** — the keyed collections,
`Symbol`, `Promise`, the weak references — and that a published global set checked against the
documents closes the question. Rule N17 makes that check mechanical and it passes.

**What a probe found anyway.** A global being present says nothing about the methods on its
prototype. A 230-case differential probe against the comparison engine, written to cover the surface
rather than to confirm it, found seven absences and one wrong answer inside globals the realm has:
`Array.prototype.at`, `flat`, `flatMap`, `findLast`, `findLastIndex` and `copyWithin`; the four
change-by-copy methods `toSorted`, `toReversed`, `toSpliced` and `with`; `String.fromCodePoint`,
`String.raw` and `String.prototype.normalize`; and **`Array.from` reading only the array-like shape**,
which had been true and correct until JSW-6 gave the realm an iteration protocol and stopped being
either. `Array.from(new Set([1,2]))` answered an empty Array — not a refusal, an empty collection —
and `Array.from` of a string outside the basic plane counted code units where the iterator counts
code points.

**What replaced it.** All of them, with `Array.from` consulting `Symbol.iterator` first and falling
back to the array-like reading. The probe now differs from the comparison engine in two places and
both are declared.

**`normalize` is the interesting one, because it is a refusal rather than an implementation.** Every
composition here runs in globalization-invariant mode, and in that mode the platform's own
`String.Normalize` **returns the input unchanged and reports that it is already normalized**. Wiring
the method to it would have produced a wrong answer that looks like a right one — the shape this
profile refused for regular expressions *(JSC-75)* and still carries for `Date`. So `normalize`
validates its form, answers an ASCII string unchanged because all four forms are provably the
identity there, and **refuses anything else by name**, saying that the Unicode tables are not held
by this component.

**The finding worth keeping is about the rule and not about the methods.** N17 compares a set of
NAMES. A rule over names cannot see a missing method, a wrong argument count or a method that reads
the wrong protocol, and a reader who watched N17 pass would reasonably have concluded the library
question was closed. **The probe is the instrument for that layer and there is no rule for it**,
which is stated here rather than left for the next reader to rediscover.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the 230-case probe and the
52-case probe over the additions, both run against the comparison engine. 2026-09-04.

### JSC-92

**Where:** [JSC-81](#jsc-81), which records the lowering emitting a composition of instructions the
verifier was right to refuse, and states that the fork between *the format admits too little* and
*the verifier rejects too much* had been drawn one level too high.

**What that entry implied.** That the case was closed: the array literal's stack effect was repaired
and the workload that found it reported a score.

**What the same shape did again.** `try { } catch (e) { }` — an empty protected block — lowered to
an exception region whose start offset **equalled** its end offset, and the verifier refused an
artifact this lowering had just produced, at `InconsistentStructure`. Every program containing an
empty `try` was refused whole, and nothing in the front end had a word to say about it, because
nothing about the program is outside the manifest.

**The verifier is right and the lowering is wrong, again.** A region protecting no instruction is a
region nothing can enter, and its handler is code the abstract pass seeds as an entry at a height
nothing establishes. So the repair puts an instruction inside the range rather than weakening the
rule: a `Nop`, emitted only when the block lowered to nothing.

**The alternative was considered and is worse.** Emitting no region and no handler would leave the
handler's instructions in the unit reached by nothing, and **an instruction stream carrying code no
entry seeds is how unverified code gets into a verified artifact**. A `Nop` costs one instruction in
a block that had none; every non-empty `try` is unchanged.

**What this says about the class rather than the case.** JSC-81's defect and this one are the same
defect: a lowering that is internally inconsistent while emitting nothing either named component
could object to on its own. Both were found by a program written to cover the surface rather than to
confirm it, and neither by a fixture written here.

**Authority and date.** The implementation of 2026-09-04 in this checkout and
`src/tests/differential/the-statement-and-object-surface.js`, whose case 43 is the empty `try`.
2026-09-04.

### JSC-93

**Where:** roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s treatment of
strict mode, and [JSC-80](#jsc-80), which records strictness a caller imposes not reaching the parse.

**What was repaired then, and what was not.** JSC-80 made a caller's `--strict` reach the **grammar**,
so strict-only early errors became visible. It said nothing about the *runtime* half, and two of
those were missing.

**Assigning to an undeclared name in strict code created a global.** `"use strict"; undeclared = 1`
answered normally and put a property on the global object, where the language gives a
`ReferenceError`. That is the single thing `"use strict"` buys a reader of an unfamiliar program,
and this profile did not give it.

**Deleting a non-configurable property in strict code answered `false`.** The object refused, the
refusal was reported as a value, and a program that did not read the value carried on as though the
property were gone.

**What replaced it.** Both are the same rule and are now written as one: an operation the object
refused is a value where a program may not have asked and an exception where it said it wanted to
know.

**Authority and date.** The implementation of 2026-09-04 in this checkout, cases 12 and 58 of
`src/tests/differential/the-statement-and-object-surface.js`. 2026-09-04.

### JSC-94

**Where:** the realm's `Object` intrinsic, and
[section 3.2](roadmap.workloads.md#32-the-surface-that-is-absent-from-the-realm)'s reading that what
is absent from the realm is a set of **globals**.

**What was absent.** `Object.prototype.__proto__`. It is Annex B rather than the core language, and
it is what a great deal of real code uses to read or set a prototype.

**What that produced, which is the part that matters.** `o.__proto__ = null` created **an ordinary
own property named `__proto__`**. Every later read answered what was stored, so the program saw its
assignment take effect and the prototype never moved. Not a refusal, not an absence: a wrong answer
that looks like a right one — the third state this profile keeps having to name.

**What replaced it.** The accessor pair the specification gives, on `Object.prototype`, configurable
and non-enumerable. The setter is a no-op rather than a throw for a non-object value, because the two
ways of being a no-op are distinguished by the receiver and not by the argument.

**Authority and date.** The implementation of 2026-09-04 in this checkout, cases 59 and 60 of
`src/tests/differential/the-statement-and-object-surface.js`. 2026-09-04.

### JSC-95

**Where:** `JsNumberFormat.ToRadixString`, whose own comment stated the deviation being corrected.

**What the code said.** That twenty fraction digits is "past the point where a binary64 fraction
carries information in any radix this accepts", and that stopping there keeps an irrational-looking
expansion from running for ever.

**Why the first half is false.** A digit carries `log2(radix)` bits. At radix 36 that is over five,
so twenty digits is more than a double holds; at radix 3 it is one and a half, so fifty-three bits
need **thirty-four** digits. `(0.1).toString(3)` was a fourteen-digit **prefix** of its own answer —
the shape a truncation always has, and the reason a fixed digit count cannot be right across a range
of radices.

**What replaced it.** The stopping rule is the value's own precision: half the distance to the next
representable double, scaled by the radix at each step, with digits produced while the remaining
fraction exceeds it and a half-way case that rounds up and carries — through the fraction and, when
it runs off the front, into the integer part. Twenty-three cases across every radix and both
extremes of the range now agree with the comparison engine exactly.

**The second half of the old comment was right and is kept**: an expansion that ran while the
fraction was non-zero would not terminate. What replaced it is a bound that means something rather
than a bound that was convenient.

**Authority and date.** The implementation of 2026-09-04 in this checkout, case 97 of
`src/tests/differential/the-statement-and-object-surface.js` and the twenty-three-case radix probe.
2026-09-04.

### JSC-96

**Where:** [JSC-85](#jsc-85), which set the profile's declared call-depth maximum below the engine's
own bound **so that the budget ceiling always answers first**, and roadmap
[section 8](roadmap.md#8-the-value-frame-and-call-model)'s *`CallDepth` is measured, not chosen*.

**What that arrangement bought and what it cost.** It bought the property JSC-79 exists for: a
runaway recursion ends as a named resource exhaustion rather than terminating the process. It cost
the language's own answer. A budget exhaustion is an **abort the guest cannot catch**, so
`try { recurse(); } catch (e) { }` — written by a recursive descent probing its own depth, by a
benchmark sizing a workload, and by every conformance case that asserts the error's type — never ran
its own guard. `Maximum call stack size exceeded` is a catchable exception in every engine, and this
profile did not have it at all.

**What replaced it: two bounds answering two different questions, in the right order.**

- The **runtime's stack probe** answers *is there room to do anything here*. When it says no there is
  no safe action left, so the operation ends. That is the case JSC-85 diagnosed.
- The **engine's counted bound** answers *has this interpreter recursed further than it promises*. It
  is reached with the probe still satisfied, so a `RangeError` can be built and thrown — and it is
  thrown, because that is what the language says. Folding the two into one condition, which is what
  the code did, gave the unsafe case's answer to the safe one.
- The **budget's `CallDepth` ceiling** is the host's own limit and stays an abort, because a budget a
  guest could swallow is not a budget. What changed is that the profile's default now sits **above**
  the engine's bound, so a host that states nothing gets the language's behaviour and a host that
  wants a program refused at a hundred frames states that and gets it.

**A third fault was hiding behind the first**, and only the split exposed it: building the
`RangeError` runs `CreateError`, which constructs an object, which re-enters the bound at a depth
already past it and throws again — a recursion with no base case inside the code that exists to
refuse one. A flag is the base case, cleared as the exception unwinds.

**The figures are re-measured rather than carried over**, because [JSC-97](#jsc-97) moved one of
them by a factor of eight. `eng/measure-frame-cost.py` now reports two depths and refuses a build
where they disagree.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the bisections
`eng/measure-frame-cost.py` performs, and six rows of the CLI acceptance table pinning all three
answers. 2026-09-04.

### JSC-97

**Where:** roadmap [section 10](roadmap.md#10-the-executor-and-the-realm)'s statement that a
JavaScript `throw` travels on the CLR's own exception mechanism and each frame catches it and looks
for a region covering the instruction that was executing.

**What that described, and what it did.** The description is accurate and the implementation was
literal: `Execute` wrapped its whole dispatch loop in `try { … } catch (JsThrow) { … throw; }`, and a
frame with no region for the current instruction **rethrew**.

**What a rethrow costs, which nothing here had measured.** A frame with a `catch` is entered during
the runtime's *second* pass: the handler runs as a funclet above the current stack, and the rethrow
starts a fresh dispatch from there. A throw crossing a thousand interpreter frames accumulated a
thousand funclets and their dispatchers — **and the process died**, on a stack that holds eight
thousand ordinary calls. **A guest `throw` from any depth past about five hundred was fatal**,
whether or not the guest had a `catch` waiting for it. That is not a bound anybody chose; it is a
cost nobody had looked for, and it made every deep recursive algorithm that reports failure by
throwing unusable.

**What replaced it.** An exception **filter**: `catch (JsThrow) when (TryFindHandler(…))`. A filter
runs in the *first* pass, without unwinding and without a funclet per frame, so a frame with no
region for this instruction answers false and is passed over and exactly one dispatch reaches the
frame that has one. `TryFindHandler` is a pure search, which is what a filter has to be.

**The two depths agree now and did not before**, which is the fact worth keeping rather than the
repair: a recursion returns from 8,061 frames and a throw unwinds from 8,047. `eng/measure-frame-cost.py`
measures both and fails a build where they diverge, because a divergence is this defect back.

**What this says about the class.** The executor's exception handling had been read, reviewed and
documented, and the documentation was *true*. What nobody had asked is what the described mechanism
costs when there are a thousand of it. A per-frame cost that only appears at depth is invisible to
every fixture written at depth one.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the bisection over a
guest `throw` at increasing depths, and case 79 of
`src/tests/differential/the-json-date-and-regexp-surface.js`, which is the recursion-inside-a-`try`
that found it. 2026-09-04.

### JSC-98

**Where:** the realm's `Date` intrinsic, and the workload roadmap's own method of comparing
behaviour against a second engine rather than against a document.

**Two things a Date could not do.**

**`date + ""` produced the epoch milliseconds.** A Date is the only object in the language whose
*default* hint means `"string"`, and without `Date.prototype[Symbol.toPrimitive]` the ordinary
conversion answered the default hint with `valueOf`. So a Date concatenated with a string became a
number that looks like an answer and is not the one every program expects, while `String(date)` —
which asks with the string hint — was right the whole time. The pair passing and failing together is
what makes this the kind of defect a probe finds and a reader does not.

**`Date.parse` refused this realm's own output.** The specification requires it to accept whatever
`toString`, `toUTCString` and `toISOString` produced; only the ISO form was implemented, so
`Date.parse(d.toUTCString())` was `NaN` and a round trip through the format a program is most likely
to have stored did not come back.

**What replaced them.** The exotic `Symbol.toPrimitive` the specification gives, installed where the
realm has a Symbol to key it with rather than where Date is built; and a textual reader for the two
forms this realm renders, written to accept what it produces and a little either side rather than to
be a date-string parser — because every non-ISO format is implementation-defined, and accepting more
would be inventing a dialect that every program relying on it would be relying on this
implementation for.

**Authority and date.** The implementation of 2026-09-04 in this checkout, cases 34 and 36 of
`src/tests/differential/the-json-date-and-regexp-surface.js`, and a twenty-case round-trip probe.
2026-09-04.

### JSC-99

**Where:** the object model's treatment of an Array's <c>length</c>, and roadmap
[section 8](roadmap.md#8-the-value-frame-and-call-model)'s exotic-object rules.

**What the code did.** An Array's <c>length</c> was reported as always writable, because it is
derived from the elements and there is nowhere in the property store for its attributes to live.
So <c>Object.defineProperty(a, "length", { writable: false })</c> was **accepted, reported as
accepted, and then ignored by the next <c>push</c>**.

**That is the one answer this profile refuses to give**: not a refusal and not an absence, but a
refusal that reports success. A program that closes an Array's length is doing so because something
downstream depends on it not moving, and every such program was told it had.

**What replaced it.** The Array carries the attribute itself, reports it, and refuses both a length
change and an element write past the length while it is closed. The prototype mutators now write
strictly — <c>Set(O, key, value, true)</c>, which is the specification's own choice for them and not
a policy added here — so a receiver that refuses makes the call throw rather than answer a new length
it does not have. A twenty-three-case probe over the closed length, <c>Object.freeze</c> and
<c>Object.seal</c> agrees with the comparison engine throughout.

**Authority and date.** The implementation of 2026-09-04 in this checkout and cases 51 to 73 of
`src/tests/differential/the-later-library-methods.js`. 2026-09-04.

### JSC-100

**Where:** [JSC-76](#jsc-76), which records the `--test262` mode arriving, and the ledger's reading
of it: *a run over named subtrees measures those subtrees*.

**What was true and insufficient.** The mode ran what a caller named, reported four verdicts, and
scored nothing on its own. Every word of that is still right. What it could not do is the thing
[JSW-10](roadmap.workloads.md#jsw-10--the-runs-per-manifest-whole) asks for, and the gap was not
size: **a run could not say what it had been run under.**

**Three things a transcript has to carry that this one did not.**

**The manifest.** The mode had no notion of one. A run under a composition admitting every optional
surface and a run under one declining the binary surface produced transcripts that looked identical,
and roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule
is *per manifest*. A run now states the manifest, the format version, the admitted surfaces and the
declined ones before it scores anything, and `--decline` exercises the verification refusal that
identity exists for.

**What the `unsupported` column is made of.** A number says a manifest declined *something*. The
column is a table now — each construct family with its count and an example, derived from the front
end's own refusal message rather than from a list somebody maintains, so a family that falls to zero
stops appearing and one nobody predicted still shows up.

**Whether the run was whole.** A run of a subtree, a sharded run, a run `--limit` truncated and a
merge missing a shard all used to look like a run. Each renders `coverage partial` with its reason
now, in a field a rule can read. **A transcript of half the suite that reads as a whole-suite run is
the failure this repository's records exist against**, and until this change nothing but a reader's
memory stood between the two.

**And one verdict was missing rather than mis-stated.** A variant that spent a budget was counted as
a **failure**, so *we did not wait long enough* and *this engine is wrong* were the same number. It
is a fifth verdict now, carrying its dimension, with every exhausted variant named — and a run in
which no variant reached a verdict about the engine is a configuration failure rather than a green
run of nothing.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the nine harness checks
added with it, and the four-shard merge watched to equal an unsharded run over the same list.
2026-09-04.

### JSC-101

**Where:** [JSC-85](#jsc-85) and [JSC-96](#jsc-96), which measure the depth this interpreter survives
and set the engine's bound from the measurement, and `JsExecution.GuestStackBytes`, which declares
the native stack one guest invocation runs on.

**What both entries assumed without saying so.** That a per-frame cost measured once stays measured.
The figure JSC-96 recorded — 1,936 bytes per JavaScript call, 8,061 calls on a sixteen-megabyte
stack — was taken against the executor as it stood that morning.

**What admitting three construct families did to it.** Each bundle added cases to the executor's
dispatch loop, and a switch's frame is sized for the widest live set across all of its arms. The
frame grew from 1,936 bytes to 3,158, so the same sixteen megabytes held **5,278** calls — below the
engine's own bound of 6,000. The bound therefore could not be reached, and a runaway recursion
**terminated the process** again, which is the outcome JSC-79, JSC-85 and JSC-96 each exist to
prevent. Nothing in the language surface changed; the measurement did.

**What replaced it.** The stack is 64 MB and the measurement is re-taken: 21,246 calls, against a
call-depth maximum a host may be granted of 8,192 — a factor of 2.6 rather than the factor of 1.34
that had silently become a factor of 0.88. The figures are recorded beside the bound they justify.

**The finding is about the shape of the claim rather than about the number.** A bound derived from a
measurement of the code is a bound that goes stale when the code changes, and it goes stale
SILENTLY — a build in which it is wrong compiles, verifies, passes every fixture written at depth
one, and dies only on a program that recurses. So the ratio between the measurement and the declared
maximum is now stated where the bound is, and `eng/measure-frame-cost.py` reports both depths and
fails a build in which they disagree. **A number nobody re-measures is an estimate with a date on
it.**

**Authority and date.** The implementation of 2026-09-04 in this checkout, the bisections
`eng/measure-frame-cost.py` performs before and after the merge, and the acceptance rows that
distinguish the catchable refusal from the host's ceiling. 2026-09-04.

### JSC-102

**Where:** [JSC-91](#jsc-91), which records a differential probe finding eight things wrong inside
globals the realm has, and states that the instrument for that layer is the probe because no rule
can see it.

**What JSC-91 did not reach, and why.** A probe finds what its cases compose. JSC-91's cases were
written when the realm had an iteration protocol and three construct families that use one — spread,
`for … of` and destructuring — were still refused by name. **The seam between a protocol and the
constructs that consume it cannot be probed until both exist**, so two absences sat in the realm
unreachable by any case anybody could write.

**What the seam showed the moment `for … of`, spread and generators landed.** `new Map(g())` and
`new Set(userIterable)` answered a `TypeError` **saying the argument is not iterable**, about an
argument that is: the collection constructors read an array-like and never consulted
`Symbol.iterator`. And a typed array was indexable and not iterable — `[...bytes]`,
`for (const b of bytes)` and `yield* bytes` all failed — because `%TypedArray%.prototype` carried no
`[Symbol.iterator]`, `values`, `keys` or `entries`.

**Both were correct when they were written and stopped being correct without being edited.** The
collection reader's own remarks called it *this realm's stand-in for iterating an iterable*, which
was the only reading available while the realm had no `Symbol`; the typed arrays were built before
the symbols existed. Neither was a mistake. **A component that grows a capability has to be re-asked
the questions its earlier answers were conditioned on**, and nothing here asks that question
automatically.

**What replaced them.** The collection constructors consult `Symbol.iterator` first and keep the
array-like reading as the fallback; `%TypedArray%.prototype` has `values`, `keys`, `entries` and
`[Symbol.iterator]`, with `values` and `[Symbol.iterator]` **the same function object**, as
`Array.prototype` has them. A twenty-one-case probe over both agrees with the comparison engine
throughout, and the cases are retained.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the merge of the
generator bundle whose seam probe reported both, and the retained cases appended to
`src/tests/differential/the-later-library-methods.js`. 2026-09-04.

### JSC-103

**Where:** [JSW-5](roadmap.workloads.md#jsw-5--the-core-language-surface-still-refused-by-name)'s
clause that a family is *admitted and exercised*, and the parameter-binding prologue the
non-simple-parameter-list lowering uses.

**What is admitted and what is not quite right.** A generator with a **non-simple** parameter list —
a default, a rest parameter or a pattern — binds its parameters in the unit's own prologue, and a
generator's prologue is body code. So the defaults run at the **first `next()`** where the language
runs them at the **call**: `function* g(a = side()) {}` orders the side effect after the call here
and before it everywhere else, and a default that throws throws from `next()` rather than from
`g()`. A simple parameter list is unaffected.

**It is recorded rather than repaired, and the reason is the format.** Separating the two would need
the code unit to declare where its prologue ends, so a generator's construction could run the
prologue and suspend after it. That is a format change with a verifier rule attached, and it is not
a merge's to make. **A fixture pins the current answer** — `runs/a-generator-default-runs-at-the-first-next.js`
— and goes red the day it is repaired, which is the point of pinning it.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the 113-case probe
over the seam between generators and the constructs merged beside them, retained as
`src/tests/differential/the-seam-between-generators-and-the-rest.js`. 2026-09-04.

### JSC-104

**Where:** [JSC-93](#jsc-93), which made an assignment to an undeclared name in strict code a
`ReferenceError` — *the single thing `"use strict"` buys a reader of an unfamiliar program* — and the
program-level hoisting the lowering performs.

**What JSC-93 did not check, and what it broke.** A function declaration at the top level of a script
was **written** to the global object and never **declared** there: the lowering emitted a closure and
a store, and the store created the property. That worked for as long as a store could create one. The
moment strict code was forbidden from creating a global by assigning to one, **every strict script
containing a function declaration threw a `ReferenceError` about the function it was declaring**.

**It survived four differential probes, an acceptance table and a full architecture suite**, and the
reason is worth more than the repair: every probe here is a sloppy script, and the acceptance table's
strict rows exercised refusals rather than declarations. A repair that narrows what a program may do
has to be checked against the programs that were *already* doing the narrower thing, and nothing here
was.

**What found it.** A test262 sweep over the subtrees this programme has just admitted, where the
`[strict]` variant of case after case failed with *`g` is not defined* about a generator declared two
lines above. The suite runs every test in both strictnesses; nothing written in this repository did.

**What replaced it.** The declaration is emitted separately from the write, which is what the
specification does and for this reason: the binding exists before anything assigns to it. Fourteen
cases covering strict declarations, strict refusals and the sloppy forms beside them are retained,
and two acceptance rows pin a strict script with a function declaration in both directions.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the sweep that found it,
and the cases appended to `src/tests/differential/the-statement-and-object-surface.js`. 2026-09-04.

### JSC-105

**Where:** the tokenizer's identifier predicate, its own remark, and section 3 of the ledger, which
carries **the Unicode data acquisition as an open external dependency** with a named holder.

**What the remark said.** That identifiers are ASCII plus `$` and `_`; that the language's answer is
the Unicode `ID_Start` property, which needs data this component has not acquired; and that a
non-ASCII identifier is therefore refused as an unexpected character, recorded as a conformance
exclusion.

**Two things were wrong with that, and they point in opposite directions.** The code had not matched
the remark for some time — it admitted any character `char.IsLetter` accepts. And the dependency the
remark leaned on is not the one this needs: **the identifier properties are derivable from the
general categories the platform already carries**, plus two small literal sets Unicode publishes as
lists. What the unacquired data is actually for is case folding, normalisation and the property
escapes in a pattern.

**What that cost, measured.** `char.IsLetter` answers the letter categories and nothing else, so
`Nl` — a Roman numeral is an identifier — and the six characters Unicode lists as `Other_ID_Start`
were refused, along with every combining mark, connector punctuation, non-ASCII digit, and the
zero-width joiner and non-joiner in continuation position. A sweep of the subtrees this programme has
just admitted found **1,012 variants refused as an unexpected character**, `U+2118` alone accounting
for 980. That is the one refusal this front end may not make about a construct the language admits:
a reader is sent looking for a typo, and the harness scores a failure rather than an unsupported
construct.

**What replaced it.** `ID_Start` and `ID_Continue` as the specification derives them, with one
subtraction stated where it is made: `U+2E2F` is the only character in both the letter categories
and `Pattern_Syntax`, and every other character that subtraction would remove is put back by
`Other_ID_Start` — which is why that set exists. Fifty-three code points spanning every category
involved agree with the comparison engine, and the cases are retained.

**What is still excluded is now stated rather than implied**: an identifier character outside the
basic plane, which needs a predicate over code points rather than over UTF-16 units.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the sweep that counted the
refusals, and the cases appended to `src/tests/differential/the-general-surface.js`. 2026-09-04.

### JSC-106

**Where:** the lowering of an object literal, and the instruction set it lowers to.

**Two members of an object literal were compiled as assignments, and neither of them is one.** The
computed member `{ [k]: v }` was lowered to the ordinary store, and `{ __proto__: p }` to an ordinary
property definition. Both were invisible for as long as nothing on `Object.prototype` had an opinion
about a key.

**What made them visible was a repair.** `Object.prototype.__proto__` is an accessor pair, and this
realm did not have it until the same day; the moment it did, `{ [k]: v }` with `k` of `"__proto__"`
stopped defining a property and started moving the object's prototype, because a store walks the
chain and finds a setter where a definition does not look. The second member had been wrong from the
beginning and in the other direction: `{ __proto__: p }` set no prototype and made an own property
called `__proto__`, which `Object.keys` reported, `JSON.stringify` serialised, and every program that
expected `p` to be the prototype read as an ordinary object.

**Neither is an assignment, and the language says so in different ways.** A computed member is
`CreateDataPropertyOrThrow`, which is why it answers to nothing on the chain — not a setter, not a
read-only property inherited from a frozen prototype. The `__proto__` member is a *separate
production*: it is not a property definition at all, it sets `[[Prototype]]` directly, and it
answers to nothing on the chain either — not even to the accessor of the same name, which a program
may delete without changing what a literal means.

**Three spellings that look like it are not it, and the lowering has to tell them apart**:
`{ ["__proto__"]: p }` and `{ __proto__() {} }` define properties, and so does the shorthand
`{ __proto__ }` — which is the one the parser had no way to distinguish by the time the lowering
saw it, so the entry now records that it was written shorthand. Writing the member twice is an early
error, because a literal that set its prototype twice would have an order nobody could read off the
source.

**What replaced them.** `SetPrototypeLiteral`, an instruction rather than a store, for the reason
above: the operation the language performs here answers to nothing that a store would consult. The
computed member defines. Twenty cases covering all four spellings, both directions of
`Object.getPrototypeOf`, a spread of a `__proto__` own property, `JSON.parse` — which makes an own
property and not a prototype — and the destructuring pattern that reuses the same syntax, agree with
the comparison engine and are retained.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the probe that found the
first while checking the repair that caused it, and the cases appended to
`src/tests/differential/the-statement-and-object-surface.js`. 2026-09-04.

### JSC-107

**Where:** `Reflect.construct`, and what an engine answers when asked whether its own built-ins are
constructors.

**`Reflect.construct` checked that its target and its new target were CALLABLE.** The language says
constructor, and the two are not the same set: an arrow, a method, a getter and every built-in in
this realm that has no `[[Construct]]` are all callable and none of them may be `new`ed.

**The cost is larger than the function, because the suite asks this question THROUGH it.** test262's
own `isConstructor` is written as a call to `Reflect.construct` with the function under test as the
new target, so a callable check here makes the realm answer *"yes, that is a constructor"* about
thirteen of its own built-ins — every function whose test asserts the opposite. Twenty-eight variants
failed for one wrong predicate, and the same predicate would have let a guest write
`Reflect.construct(C, [], Math.max)` and get an instance whose prototype came from a function that
has none.

**Two neighbours were wrong in the same file and found by the same sweep.**
`Reflect.setPrototypeOf` reported `false` for setting the prototype an object already has when the
object was not extensible — `[[SetPrototypeOf]]` asks whether the answer would *change*, and a
non-extensible object refuses only a change — and it let a cycle throw where this namespace answers
every refusal with `false`. `Reflect.defineProperty` read its property key inside the `try` that
turns a refusal into `false`, so a `toString` on the key that threw was reported as the object
declining rather than as the program's own exception.

**What replaced them.** The constructor predicate the object model already carries, the two
`[[SetPrototypeOf]]` steps in the specification's order, and the key and descriptor read before the
`try` that may swallow. The pinned suite's `built-ins/Reflect` subtree moved from 250 of 306 variants
to 286; the twenty that remain all name `Proxy`, which this realm does not have.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the sweep of
`test/built-ins/Reflect` that found all three. 2026-09-04.

### JSC-108

**Where:** the lowering of every position where the language infers a function's name, and the
remark on `CompileNamedValue` that said only a class takes one.

**What the remark said, and what it left out.** That `const C = class { }` names the class `C`,
that this is done in the lowering because the name is baked into the code unit, and that *an
anonymous function expression still reports the empty name it always has, which is a divergence this
profile already carried*. The divergence was stated. What was not stated is its size: **every
anonymous function in the realm reported the empty string**, which is the answer for
`var f = function () {}`, for `{ m: function () {} }`, for `[a = () => {}]`, for a parameter default,
and for a logical assignment — six positions the language names and this lowering did not.

**What that cost, measured.** A sweep of `test/language/expressions/object` found **eighty variants
failing on the name alone**, in five families — arrow, function, generator, class and the cover
grammar — each asserting the name a destructuring default infers. The same five appear under every
other subtree that has a destructuring pattern in it. It is the shape of failure that is easy to
leave: nothing crashes, no program stops, and a suite reports a number.

**Why a lowering rather than the executor.** A code unit belongs to exactly one syntactic site, so
the name a site infers is the name every closure over that site has — there is no case where two
closures over one unit need different names. **The closure is emitted directly rather than through
the named-function-expression path, and the difference is a binding**: a name in the TEXT of a
function expression is bound inside its own body, and an inferred one is not.
`var f = function () { f = 1; }` assigns the outer `f`, and routing an inferred name through the
path that creates the self-binding would have made that assignment silently write a binding nobody
can see.

**Two positions are still not named, and they have the same cause.** A computed member —
`{ [k]: function () {} }` — infers its name from a key that is not known until it is evaluated, and
this lowering names units rather than function objects. Naming it would need either an instruction
that names the object on the stack or a marker saying that this particular value was written
anonymously *here*; naming every unnamed function a computed member happens to receive would rename
`{ [k]: alreadyAnonymous }`, which the language leaves alone. Both are retained as declared
divergences rather than left for a reader to find.

**A third divergence was found while checking this one and is now stated.**
`Function.prototype.toString` answers `function f() { [native code] }` for a function written in the
guest, where the language says the source text. The artifact carries no source — the position table
maps offsets to positions, not to characters — so this cannot be produced from what the executor
holds. It was undocumented until the inferred name changed which name appeared in it.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the sweep that counted
the eighty, and the thirty cases appended to `src/tests/differential/the-general-surface.js`.
2026-09-04.

### JSC-109

**Where:** the `[Yield]` and `[Await]` grammar contexts the wide front end tracks, and the
sub-parser `JsParser.ParseInterpolation` builds for a template substitution.

**What was assumed.** That a template substitution's tokens could be parsed by a parser told the
enclosing strictness and the enclosing function depth, and nothing else. Those were the two contexts
that had ever mattered, because they were the two the substitution's own grammar could observe.

**What was true.** A substitution inherits every parameter of the production it sits in, and two of
them are `[Yield]` and `[Await]`. The sub-parser was built without either, so `` `x${yield 1}` ``
inside a generator read `yield` as an identifier and answered "`1` follows the expression of a
template substitution" — a surprise token where the language has an ordinary yield expression. The
same hole would have swallowed every `` `x${await p}` `` the day async functions were admitted, and
that is how it was found: by writing the case, not by auditing the parser.

**What the shape of it is, and why it is worth an entry.** The hole was invisible while the
constructs it hid were refused. `yield` was admitted before the substitution's context was, and
nothing failed, because a program that writes a suspension inside a template is rare enough that no
fixture had one. **A context threaded through one path and not through another is a defect that
waits for a construct to become legal**, and the two-year-old half of it was found by the change
that would have created the second half.

**What replaced it.** The sub-parser is handed both flags, exactly as it is handed the strictness
and the function depth, and a suspension inside a template substitution now parses in a generator
and in an async function alike.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the differential
probe over the async family, which is where the case was written. 2026-09-04.

### JSC-110

**Where:** [JSC-101](#jsc-101), and the figures it left recorded in `JsEngine.MaximumCallDepth`,
`JavaScriptProfile` and `JsExecution.GuestStackBytes`.

**What JSC-101 asked for, and what this is.** It asked that the per-frame cost be re-measured
whenever the instruction set moves, because the executor's frame is sized for the widest live set
across every arm of one switch. Admitting `async` and `await` added an arm and two locals the async
driver carries across its own `try`, so the measurement was re-taken rather than reasoned about.

**What it found.** 3,671 bytes per JavaScript call, against 3,463 before the family and 3,158 before
the generators. The sixty-four-megabyte stack holds **18,277** calls, against 19,377 before. The
engine's own bound is 6,000 and the call-depth maximum a host may be granted is 8,192, so the
ordering both figures exist to guarantee still holds — the capacity is still more than twice the
maximum a host can ask for — and neither the stack nor either bound had to move.

**What is worth saying rather than the number.** An `await`'s resumption does NOT stack. A `yield*`
chain holds one interpreter frame per level, because each resumption is nested inside the last; an
async chain holds one at a time, because every resumption starts from the job queue with the
previous frame already returned. So the family that grew the per-frame cost is also the family least
able to spend it, and what an async program exhausts is `Fuel` — which is the dimension its own
acceptance row names.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the bisection
`eng/measure-frame-cost.py` performs against a build with both bounds lifted, which is the only
arrangement that measures the stack rather than the promise. 2026-09-04.

### JSC-111

**Where:** the lowering's refusal of `return` outside a function, which asked the CURRENT compile-time
scope rather than the enclosing hoisting one.

**What it let through.** `return` at the top level of a script is an early error in the language, and
the lowering refused it by testing `scope.Kind == ScopeKind.Program`. That test is true only when no
scope has been pushed since the program's own — so `{ let a; return 1; }` at the top of a script,
whose block pushes a scope because it declares a lexical name, was **admitted**: the lowering emitted
a `Return` in the program's code unit, the verifier accepted it because a `Return` at height one is a
legal instruction there, and the script completed with the returned value. The same shape with no
lexical declaration in the block was refused, because that block pushes nothing. **A refusal that
depends on whether an unrelated declaration is present is not a refusal anybody can predict.**

**Why admitting `with` is what found it.** A `with` pushes a scope unconditionally, so it opened a
second way in — `with ({}) { return 1; }` at the top of a script — and writing the fixture for that
position is what made the first case visible. The defect is older than the construct that exposed it,
which is the shape [JSC-82](#jsc-82) and [JSC-83](#jsc-83) also have.

**What replaced it.** The test is `FunctionScope().Kind == ScopeKind.Program`, which walks past every
block and every object environment record to the hoisting scope the `return` would return from. That
is the question the rule was always asking; the old test answered it only when nothing was in the way.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the acceptance fixtures
for the `with` family, which exercise a `return` out of two nested object environment records.
2026-09-04.

### JSC-112

**Where:** [JSC-101](#jsc-101), which measures the per-frame cost of the executor's dispatch loop and
derives the call-depth bound from it, re-taken for the fourth time.

**What the measurement says now.** Admitting `with` adds two arms to the dispatch loop, one of which
holds a walk of the scope chain, and the executor's own frame grew from **3,463 bytes to 3,479**. The
sixty-four-megabyte guest stack holds **19,288** calls where it held 19,377, against an engine bound
of 6,000 and a call-depth maximum a host may be granted of 8,192 — a factor of 2.35, so nothing had to
move. `eng/measure-frame-cost.py` reports both depths stopped by the declared bound rather than by the
stack, which is the outcome the script exists to require.

**It is recorded even though nothing changed, and that is the point.** JSC-101's finding was that a
bound derived from a measurement goes stale silently, and a bundle that skipped the measurement because
it expected no change would be making exactly the assumption JSC-101 named. The figures are recorded
beside the bound in `JsEngine.MaximumCallDepth`, in `JavaScriptProfile`'s maxima and in
`JsExecution.GuestStackBytes`.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the two bisections
`eng/measure-frame-cost.py` performs — one against the published binary, one against a build with the
engine's bound and the profile's call-depth maximum lifted, which is what reports the capacity rather
than the promise. 2026-09-04.

### JSC-113

**Where:** `JsRealm.CreateListIterator`, which every iterator over a keyed collection is built from,
and the remark on it that said the list is the live one.

**What was assumed.** That the iterator could hold the cursor and step it by one per call, and that
the reader it was given would answer about the slot it was handed.

**What was true.** A keyed collection's table does not compact — it keeps a deleted entry as a
tombstone precisely so that an iterator's position stays meaningful — so its reader walks *past* a
tombstone to the next live slot and answers about a slot the caller did not name. A cursor stepped
by one then re-read the entry the reader had just answered with. **A Map with one deleted entry
yielded its last entry twice**, and did so through `keys`, `values`, `entries`, the spread, and
`for … of` — every path except `forEach`, which walks the table itself and was right all along.
`size` said two while the iterator produced three.

**Two things kept it hidden.** The realm's own tests build collections and read them; they do not
delete from one and then iterate it. And `forEach` — the shape a program written by a person is
most likely to use — took the other path.

**What replaced it.** The reader answers with the slot the cursor should land on, which is the only
party that knows. Exhaustion is latched at the same time, because the language retires the iterator
when it runs out — it drops the reference to what it was iterating — and a cursor that merely sat at
the end would have reached an entry appended afterwards. Both directions agree with the comparison
engine and are retained.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the probe over the
collections that found it, retained in `src/tests/differential/the-later-library-methods.js`.
2026-09-04.

### JSC-114

**Where:** `%TypedArray%`, its prototype, and the nine constructors under it.

**What the code said.** That `Int8Array.from` is reachable through the superclass *(the remark on
`constructor.Prototype = superclass` says exactly that)* — and then defined `from` and `of` on each
of the nine constructors, so nothing was ever reached through the superclass. A program that asks
`Int8Array.from === Uint8Array.from` got `false` where the language says `true`, and
`Object.getOwnPropertyNames(Int8Array)` listed two members the language does not put there.

**What else was missing, measured against the comparison engine.** Six members of
`%TypedArray%.prototype`: `findLast`, `findLastIndex`, `toLocaleString`, and the change-by-copy trio
`toReversed`, `toSorted` and `with`. `toSpliced` is **not** among them and its absence is correct —
splicing changes a length, and a view over a buffer has none to change.

**And `from` read only an array-like**, which its own remark defended on the grounds that iterables
were out of this profile's scope. They stopped being out of scope when `for … of` and
`Symbol.iterator` were admitted, and the remark outlived the reason: `Int8Array.from(new Set([1,2]))`
answered with an empty view rather than with two elements.

**What replaced them.** One `from` and one `of`, on the superclass, resolving the kind from the
receiver through a map from constructor to kind — so a receiver that is not one of the nine is
refused by name rather than answered with a view of some default kind. The six members are defined,
each returning a view of the receiver's own kind. Twenty cases covering the member lists, the
identity of the shared functions, the iterable source and the copying methods agree with the
comparison engine and are retained.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the probe over the
binary surface, retained in `src/tests/differential/the-later-library-methods.js`. 2026-09-04.

### JSC-115

**Where:** the constant and name tables of format version 2 — how a String is written into an
artifact and read back out of one.

**What was assumed.** That a JavaScript String is text, and that the platform's UTF-8 encoder
therefore carries one.

**What was true.** A JavaScript String is a sequence of UTF-16 code **units**, not of scalar values.
`"\uD800"` is a legal String with a legal length, a legal `charCodeAt` and a legal comparison; it is
also an unpaired surrogate, which **no UTF-8 sequence encodes**. `System.Text.Encoding.UTF8` answers
a replacement character for it and says nothing, so the literal reached the artifact as `U+FFFD` and
every later answer about it was about the replacement: its units, its length in a comparison, its
equality with another such literal, and what `JSON.stringify` escaped.

**It was silent in both directions, which is what makes it worth an entry.** Nothing threw, no
verification failed, and the corruption happened between a front end that had the right units and an
executor that never saw them. The probe that found it was not looking for it: an unpaired surrogate
reached a case by accident, and the case answered `true` where the comparison engine answered
`false`.

**What replaced it.** WTF-8, defined in the format rather than borrowed from the platform: a
surrogate is written as its own three bytes, which UTF-8 forbids and this format therefore states.
**Every well-formed String encodes to exactly the bytes it encoded before** — byte for byte, digest
for digest, so the retained corpus is untouched — and only a String no UTF-8 encoder could have
carried is written differently. The decoder answers replacement characters for malformed input
rather than throwing, because an artifact is untrusted input and the verifier already ends a bad one
by diagnosis.

**Two members were missing beside it and are now present**: `String.prototype.isWellFormed` and
`toWellFormed`, which are the language's own way to ask this question — and which could not have
answered it truthfully while the artifact was destroying the evidence.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the probe over the
pattern, text, instant and number surfaces, retained in
`src/tests/differential/the-json-date-and-regexp-surface.js`. 2026-09-04.

### JSC-116

**Where:** `String.prototype.matchAll`, which was absent.

**What its absence cost.** A global `match` answers the matched TEXT of each match and throws the
captures away, so a program that wants every match *with* its groups has to loop `exec` and manage
`lastIndex` by hand — which is the loop `matchAll` exists to be, and the loop a program written
against a modern engine simply does not contain. A workload using it got a `TypeError` about a
method the prototype did not have.

**What it is, and the two things that are easy to get wrong.** It iterates over a **copy** of the
pattern, so a program that interleaves `matchAll` with `exec` on one RegExp sees neither disturb the
other's `lastIndex`; and it refuses a non-global RegExp with a `TypeError` rather than answering an
iterator of one, because the loop would not terminate without the `lastIndex` a global pattern
keeps. An empty match advances the cursor by hand, for the same reason the global `match` beside it
does.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the twelve cases
retained in `src/tests/differential/the-json-date-and-regexp-surface.js`. 2026-09-04.

### JSC-117

**Where:** four members of the realm that were absent, and one that answered where it should have
refused. Found by one probe pass over the error objects, the global functions, the `Object` statics,
the function objects and the Array.

**`AggregateError` was absent**, and it is the one error subtype with a different shape: its first
argument is the errors and its second is the message, where every other subtype takes the message
first. That is not a wart — the type exists for `Promise.any`, which has a LIST of reasons and no one
reason to report — and a program that catches what `Promise.any` rejects with had no constructor to
test against.

**`Object.fromEntries` was absent**, which is the inverse of `Object.entries` and takes an *iterable*
rather than an Array, so `Object.fromEntries(map)` is the shape most programs use it in.
**`Object.groupBy` and `Map.groupBy` were absent**: the first answers an object with a **null
prototype**, which is the whole reason it beats the four lines a program would write instead — a
group key of `toString` collides with nothing — and the second keeps a key that is not a string as
itself.

**And a strict `arguments.callee` answered with the function.** The language poisons it: `callee` on
a strict arguments object is an accessor pair whose halves both throw, and the property is present
so that `"callee" in arguments` stays true. This realm gave every arguments object the ordinary data
property, so strict code could ask which function it was in — a question strict mode exists to
refuse, and one a program can use to reach a function that was never handed to it.

**One divergence found in the same pass is declared rather than repaired.** An error object here
carries no `stack`. It is not a member the language defines, and producing one would need the
executor to keep a frame list it does not keep — the depth is a counter, and the interpreter's frames
are the host's own. It is now a declared divergence rather than an unstated absence.

**Authority and date.** The implementation of 2026-09-04 in this checkout, and the ninety-eight cases
appended to `src/tests/differential/the-general-surface.js`. 2026-09-04.

### JSC-118

**Where:** the four keyed-collection constructors, and the iterator protocol they reach their
arguments through.

**What was assumed.** That `new Map(entries)` may read the entries and store them, because storing
them is what it does.

**What was true.** The language builds a Map by reading the collection's **own `set`** once and
CALLING it per entry, and a Set by calling its own `add`. Two things follow that this realm did not
do. A subclass that overrides `set` sees its override used by the constructor — observable, and the
reason the rule exists. And **a `set` that throws stops the walk**, which is the only thing that ends
`new Map(iterable)` over an iterator that never reports done.

**What that cost, measured.** This realm collected the whole iterable into a list first and stored
second. Over an infinite iterator that never returns, so the pinned suite's
`Map/iterator-close-after-set-failure.js` and its four neighbours spent the wall clock rather than
answering — **ten variants of `built-ins/Map` ended in exhaustion**, which is the verdict this
harness reserves for a program that ran out of allowance rather than one that was wrong.

**What replaced it.** A walk that hands one element at a time to a step, closing the iterator when
the step is abrupt, and constructors that read their own adder and call it. `built-ins/Map` went from
326 of 405 variants to 399, with no exhaustion left.

**Three absences and a species accessor were found in the same sweep.** `Map.prototype.getOrInsert`
and `getOrInsertComputed` are ES2026 members the pinned suite tests and the comparison engine does
not have — the one direction where the suite is the oracle and the comparison is behind. The seven
**set operations** — `union`, `intersection`, `difference`, `symmetricDifference`, `isSubsetOf`,
`isSupersetOf` and `isDisjointFrom` — were absent, and `built-ins/Set` went from 456 of 764 variants
to 730 when they arrived. `Symbol.species` had no accessor on any constructor; it is installed now,
with the divergence stated where it is installed: the methods here still build a result of the
receiver's own kind rather than consulting it.

**Authority and date.** The implementation of 2026-09-04 in this checkout, the sweeps of
`test/built-ins/Map` and `test/built-ins/Set` before and after, and the thirty-one cases appended to
`src/tests/differential/the-later-library-methods.js`. 2026-09-04.

### JSC-119

**Where:** `JsEngine.TryIterateNext`, which is every `for … of`, every spread, every destructuring
and every built-in that iterates.

**What it did.** Read `value` off an iterator result that had just said it was `done`.

**What the language says.** `IteratorStep` reads `done`; the `value` of a done result is read by
`IteratorValue`, which only a caller that WANTS the return value calls — and there is exactly one
such caller, the `yield*` delegation, whose own value is the inner iterator's return value.

**Why an extra property read is a defect rather than a waste.** It is observable. A result object
with a `value` **getter** counts the read, and the pinned suite counts exactly that: its set-like
iterators record every `getting done` and `getting value`, and compare the trace with the one the
specification prescribes. Twelve variants across the Set operations failed on a trace that was right
except for one read at the end.

**What replaced it.** The caller says whether it wants the completion value, and only the delegation
does.

**Authority and date.** The implementation of 2026-09-04 in this checkout and the
`set-like-class-order` cases of the pinned suite, which are what the trace comes from. 2026-09-04.

### JSC-120

**Where:** the conformance harness's `--test262` command, and every whole-suite figure it has ever
produced.

**Two things it did not do, and both moved the numbers.**

**It never drained the job queue.** test262's asynchronous tests call `$DONE` from a promise
reaction, so the line the runner reads its verdict off is printed by a JOB and not by the script. The
runner invoked the scripts and stopped, saw no completion, and reported *"an asynchronous test
printed no completion, and this profile has no job queue"* — a sentence that was true when it was
written and stopped being true the day JSW-7 built one. **710 variants of `built-ins/Promise` alone**
were scored as failures for a queue nobody asked to run. The drain point is the host's to choose;
this one now chooses where the end-user host chooses, after the last script.

**It scored tests about proposals.** The suite's own `features.txt` separates proposed flags from
standard ones and says in its prose that the proposed ones exist so consumers may omit them. The
ingested-dialect command reads that file and excludes them; this command did not, so a run reported
an engine as failing the language over constructs no published edition contains — and would have
reported a PASS as conformance the same way. Two selection paths that disagree about what is scored
make one checkout answer two different numbers.

**Neither is a defect in the engine, and that is the point.** A harness that under-reports is worse
than one that over-reports, because the work it invents is work somebody does.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the
`test/built-ins/Promise` sweeps either side of it. 2026-09-05.

### JSC-121

**Where:** `Promise` — its statics, its `then`, its `finally`, and what they build their answers
with.

**What was assumed.** That a promise this realm answers with is a promise this realm made.

**What was true.** Every static on `Promise` builds its answer through `NewPromiseCapability(this)`,
so the RECEIVER decides what is constructed: `Promise.all.call(C, xs)` answers a `C`, and
`Promise.resolve` called on something that is not a constructor is a `TypeError` rather than a
promise. `then` builds its result through `SpeciesConstructor(this, %Promise%)`, which is the one
hook a library has to make its own promise type survive a chain. This realm reached for its own
constructor everywhere, so the combinators were reachable from a subclass and useless to one.

**Five smaller things were wrong in the same place, each observable.** The combinators walked their
argument by collecting it first — so an infinite iterator whose walk should stop at the first `then`
that throws never stopped. Each element's handlers had no latch, so a thenable calling its
`onFulfilled` twice counted twice and settled a result while elements were still outstanding.
`Promise.all` and `race` build one handler per element where the language passes the capability's
own function to every element — which a test can see by comparing the two it was handed.
`Promise.any` rejected with an object SHAPED like an `AggregateError`, because the realm had none
when it was written. And `Promise.withResolvers`, `Promise.try` and
`Promise.prototype[Symbol.toStringTag]` were absent.

**What that cost, measured.** `test/built-ins/Promise` scored **250 of 1,348 variants** before this
work and **1,268 of 1,311** after it, with nothing exhausted; the six that remain name `Proxy`,
a nested realm, or one thenable-identity rule.

**A crash was found on the way and is the reason this entry names `JsEngine` too.**
`Describe` — which builds the message of *"… is not a function"* — asked any value that was not one
of five primitives whether it was callable, which reads it as an OBJECT. A **Symbol** is not one, so
`Symbol()()` failed the cast while building the very `TypeError` it was about, and ended the whole
invocation as `ProfileFault/ProfileContractViolation`: an internal fault, uncatchable, in place of
the language's own error.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/Promise` before and after, and the twenty-seven cases retained as
`src/tests/differential/the-settling-of-promises.js`. 2026-09-05.
### JSC-122

**Where:** `JsParser.ParseClassMember`, the arm that reads an `async` modifier in a class body, and
`JsCompiler.FindConstructor`, which decides which member is the class's own constructor.

**What was assumed.** That a member named `constructor` is the class's constructor, and that the
question is answered by three things: that the member is not static, that its key is not computed,
and that the key is the string `constructor`. The `async` arm was written to the same shape as the
ordinary method arm and produced a member of kind `Method`, so it satisfied all three.

**What was true.** The language settles this on a fourth thing the check did not ask: whether the
member is a SPECIAL method. `class C { async constructor() { } }` is a Syntax Error, and so are
`get constructor`, `set constructor` and `*constructor`. This front end accepted all four. The
`async` one is the worst of them, because it did not merely accept a member the language refuses — it
made that member the CLASS CONSTRUCTOR and compiled it with `ClassConstructor`, `Constructible` and
`Async` at once, and the constructible bit is dropped from an async unit. So
`class C { async constructor(){} }` compiled, and `new C()` answered
`TypeError: function is not a constructor` — a run-time refusal, naming nothing the source wrote,
for a program the front end owed a Syntax Error.

**What that cost, measured.** The four shapes are reachable from three characters of source, and the
outcome was an error about a function where the language says an error about a class. In the pinned
suite's `test/language/statements/class` subtree the `definition` directory contributed **10** of the
parse-phase failures before the repair, and the early-error rows under `elements/syntax/early-errors`
another **36**.

**What replaced it.** `ValidateClassBody`, run over the parsed member list, refuses a non-static
non-computed `constructor` that is anything but a plain method, refuses a static member named
`prototype`, and refuses a private name declared twice. `FindConstructor` additionally asks that the
member is not private, so `#constructor` cannot become one either. The code is
`2201:DuplicateLexicalDeclaration` in each case, because each is the second declaration of a name
something has already declared — the class definition's own for `constructor` and `prototype`, the
body's own for a repeated private name.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the comparison engine at
`/opt/node22/bin/node` — which answers `SyntaxError: Class constructor may not be an async method`
for the first shape and `SyntaxError` for the other three — and the differential probe's cases 241
and 242, taken from that engine before they were written down. 2026-09-05.

### JSC-123

**Where:** `JsEngine.Evaluate`, the arm that answers when the artifact provider does not supply a
program.

**What was assumed.** That every way a guest-initiated load can fail is this host's own plumbing, so
one `EvalError` naming the outcome and the reason serves them all.

**What was true.** One of those ways is not plumbing at all. The mediator answers `ProviderRefused`
when the provider it asked declined, and the only providers this profile's compositions register are
source compilers — so `ProviderRefused` means *the front end refused the source*, which is the one
case the language has its own answer for: `eval` of text that is not a program throws a
**`SyntaxError`**. Programs test for it, and a conformance case that writes
`assert.throws(SyntaxError, function () { eval(src); })` is asking about the language rather than
about this host's load path.

**What that cost, measured.** In `test/language/statements/class` alone, **136 variants** failed with
`Expected a SyntaxError but got a EvalError` — every one of them a case whose subject this host
answers correctly and whose verdict was decided by the wrapper around the answer. The subtree's
failures fell from **998 to 926** when this was repaired. The difference that remains is the cases
that use a DIRECT `eval`, which this profile refuses for a reason of its own that is published and
which is not this defect.

**What replaced it.** `ProviderRefused` is tested before the outcome and answered with
`ThrowSyntaxError`. Every other way a load can fail — no provider registered, a budget exhausted, the
mediator out of scope, a foreign artifact, no entry point — keeps the `EvalError` it had, because
each of those really is this host's plumbing and none of them is a statement about the source.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the two sweeps of
`test/language/statements/class` that bracket it, and the differential probe's cases 234 to 244 —
the eleven that disagreed with the comparison engine before the repair and agree after it.
2026-09-05.

### JSC-124

**Where:** `JsParser.ParseClassMember`, the arm that decides whether `get` and `set` are modifiers.

**What was assumed.** That `get` and `set` are member names rather than modifiers exactly when the
token after them ends a member name — `(`, `=`, `;` or `}` — which is what `IsMemberNameEnd` lists.

**What was true.** There is a fifth token that makes them names, and it is not an ending: `*`.
`class C { get` / newline / `*a(){} }` declares a FIELD called `get` and then a generator method
called `a`, because `get` is a modifier only before a *PropertyName* and `*` is not one. With `*`
absent from the list the parser read `get` as an accessor modifier, took `*` as the key, and reported
that a class element with a `get` modifier needs a parameter list — a diagnostic about a construct
the source did not write.

**What that cost, measured.** **Four variants** of the pinned suite's
`elements/syntax/valid/grammar-field-named-get-followed-by-generator-asi.js` and its `set` twin, each
of which is a VALID program this front end refused. It was reachable only once class fields were
admitted, because without them `get` alone was refused as a field before the `*` was reached.

**What replaced it.** The `get`/`set` arm additionally requires that the next token is not `*`.
`static` and `async` are deliberately unchanged: `static *m(){}` is a static generator and
`async *m(){}` is an async generator, so for those two the `*` keeps them modifiers.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the two suite sweeps
that bracket it. 2026-09-05.

### JSC-125

**Where:** `SliceSourcePrograms.Refused`, the row `refuse-an-unexpected-character`, and through it the
retained source corpus and the registry's reachability claim for code `2001`.

**What was assumed.** That `1 @ 2` is a source refused for an unexpected character, and that the
registry row
`2001|UnexpectedCharacter|embedder-seam|-|tokenizer|source|refuse-an-unexpected-character` therefore
names a case that produces the code.

**What was true.** Since commit `b6bda26` — the bundle that admitted class declarations — the
tokenizer refuses `@` **by name, as a decorator**, with `2104:ConstructOutsideManifest`. That was the
right change and it is documented where it was made; what nobody re-ran was the check that depends on
it. So the retained manifest recorded a code the front end had stopped producing for that source, and
the registry claimed a reachability code `2001` no longer had.

**What that cost, measured.** **One of the sixteen** checks
`Broiler.VM.Composition.JavaScript.SliceCompiler --checks` performs was failing, and had been since
that commit: `every source the manifest excludes is refused by name: refuse-an-unexpected-character:
wanted UnexpectedCharacter, got ConstructOutsideManifest`. It is not one of the gates the workload
stages run, which is how it survived. Rule N7 was green throughout, because N7 reads the manifest
rather than re-running the front end — so the rule that exists to keep the registry honest could not
see the one thing that had made it dishonest.

**What replaced it.** The source is `1 ¡ 2`. An inverted exclamation mark is not an identifier start,
begins no token either grammar defines, and — unlike `@` — is not a character any proposal is going to
give a meaning to, which is the property the row needs and `@` never had. The corpus was regenerated
and all sixteen checks pass.

**Authority and date.** The implementation of 2026-09-05 in this checkout and
`Broiler.VM.Composition.JavaScript.SliceCompiler --checks`, run before and after. 2026-09-05.

### JSC-126

**Where:** [JSC-101](#jsc-101), which measures the per-frame cost of the executor's dispatch loop and
derives the call-depth bound from it, re-taken for the fifth time.

**What the measurement says now.** Admitting the class body adds six arms to the dispatch loop —
`DefineClassElement`, `RunStaticElements`, `NewPrivateName`, `LoadPrivate`, `StorePrivate` and
`HasPrivate` — and the executor's own frame grew from **3,736 bytes to 4,073**. The
sixty-four-megabyte guest stack holds **16,478** calls where it held 17,963, against an engine bound
of 6,000 and a call-depth maximum a host may be granted of 8,192. That is **2.75** times the bound and
**2.01** times the ceiling, so nothing had to move. The returning and throwing depths agree exactly,
at 16,478 apiece, which is the property [JSC-97](#jsc-97) established and this measurement
re-confirms.

**It is recorded even though nothing moved, and the narrowing is why.** JSC-101's finding was that a
bound derived from a measurement goes stale silently. This is the first re-measurement where the
factor against the ceiling a host may be granted reached two: one more family of this size takes it
under, and at that point either the stack is re-declared or the ceiling is. Saying so now is cheaper
than discovering it from a terminated process. The figures are recorded beside the bound in
`JsEngine.MaximumCallDepth`, in `JavaScriptProfile`'s maxima and in `JsExecution.GuestStackBytes`.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the two bisections
`eng/measure-frame-cost.py` performs — one against the published binary, which reports both depths
stopped by the declared bound, and one against a build with `JsEngine.MaximumCallDepth` and both of
the profile's call-depth maxima lifted to 400,000, which is what reports the capacity rather than the
promise. 2026-09-05.

### JSC-127

**Where:** the lowering of `&&=`, `||=` and `??=`, which admitted them against a NAME and refused
them against a property.

**What the refusal said.** *"a logical assignment to a property is not admitted"* —
`2104:ConstructOutsideManifest`, at the front end, for `o.x ||= v`.

**Why it was there and why that reason had expired.** The three operators were admitted against a
name, where the lowering is a load, a test and a store. Against a property it is not, and the
difference is the whole of it: the reference has to be evaluated ONCE and the write has to happen
only when the test asks for it. `f().x ||= v` calls `f` exactly once, and `o.x ||= v` over a truthy
`o.x` performs no `[[Set]]` at all — so a setter does not run, and a read-only property does not
throw in strict mode. The rewrite this looks like, `o.x = o.x || v`, gets both of those wrong, which
is presumably why the refusal was written rather than the lowering.

**What replaced it.** The base — and the key, when the member is computed — is evaluated once and
kept beneath the value the test reads, and the two paths meet at one operand height, which is what
lets the verifier check a construct whose two halves leave different things behind. Twenty-four
cases agree with the comparison engine, including the two frozen-object cases that distinguish the
lowerings: `o.x ||= 2` on a frozen truthy property is silent in strict mode, and on a frozen falsy
one it is a `TypeError`.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the cases appended to
`src/tests/differential/the-statement-and-object-surface.js`. 2026-09-05.

### JSC-128

**Where:** `Object.freeze` and `Object.seal` over an Array, and the predicate that told them to skip
its `length`.

**What the predicate said.** That an Array's `length` is *unattributable* — a property whose
attributes may not be set — so the integrity walk stepped over it.

**What was true.** It is attributable like any other property; what it is not is *deletable*, which
is a different question asked somewhere else. Skipping it left **a frozen Array whose length was
still writable**, so `Object.freeze([1,2,3])` produced an object that could still be truncated to
nothing — and `Object.isFrozen` answered `true` about it, because it consulted the same predicate.
An integrity guarantee that reports itself held while not holding is worse than one that is absent.

**Three neighbours were wrong in the same area, and the pinned suite counts all four together.**
An Array's `length` was neither checked nor coerced on the way in: `a.length = -1` and
`a.length = 1.5` were silently accepted where the language raises a `RangeError`, and
`a.length = "2"` set nothing rather than two — the check is that `ToUint32` and `ToNumber` agree,
which is what refuses the first two and accepts the third. A shortening deleted every element above
the new length regardless of whether it MAY delete them, where the language stops at the first
non-configurable element and leaves the length there — a partial result rather than either extreme.
And a write past a length that had been made non-writable was dropped silently in strict code, where
it owes a `TypeError`: the object model cannot know the mode, so the refusal was only half made.

**Two absences were found in the same sweep**: `Object.hasOwn` — which is
`Object.prototype.hasOwnProperty.call(o, k)` with a name, and works for an object with a null
prototype — and the four Annex B accessor helpers `__defineGetter__`, `__defineSetter__`,
`__lookupGetter__` and `__lookupSetter__`, which the language keeps because the web does.

**What that cost, measured.** `test/built-ins/Object` scored **6,175 of 6,802 variants** before this
work and **6,511** after it. Of the 289 that remain, 116 name `Proxy`.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/Object` before and after, and the forty-one cases appended to
`src/tests/differential/the-later-library-methods.js`. 2026-09-05.

### JSC-129

**Where:** the pattern protocol — the five Symbols a String method dispatches through — and the
header remark in `JsRealm.RegExp.cs` that explained its absence.

**What the remark said.** *"No `Symbol.match`, `Symbol.replace`, `Symbol.search` or `Symbol.split`
protocol. This surface has no Symbols at all, so the six built-ins here test for a RegExp object
rather than dispatching on a method."*

**The first sentence of the reason stopped being true when the realm acquired `Symbol`**, and the
remark outlived it — the same shape as [JSC-105](#jsc-105) and [JSC-114](#jsc-114): a stated
limitation whose stated cause has gone, which is harder to notice than an unstated one because the
document looks like it was thought about.

**What that cost.** A pattern in this language is *an object with the right Symbol*, not a RegExp:
`"x".replace(p, r)` asks `p` for `Symbol.replace` and calls it, and a program's own object answering
that Symbol is a pattern. Testing for a RegExp object refused every such object, and
`Symbol.matchAll` was not even minted.

**What replaced it.** The five methods on `RegExp.prototype`, under their Symbols, and the five on
`String.prototype` dispatching through `GetMethod` in the specification's order — a nullish pattern
is not asked at all, so `"null".replace(null, "X")` still replaces the text; a Symbol that is present
and not callable is a `TypeError` rather than a fall-through.

**What is still not done is now stated precisely rather than by a sentence that had rotted.** The
five methods on `RegExp.prototype` run the matcher directly instead of reading the receiver's own
`exec`, so a subclass overriding `exec` changes what `re.exec(s)` answers and not what
`s.match(re)` answers. That is worth about 170 variants of `built-ins/RegExp` and is a separate
piece of work.

**One crash was found in the same sweep.** `String.fromCodePoint(0xD800)` ended the invocation as
`ProfileFault/ProfileContractViolation`: the platform's converter encodes SCALAR values and refuses
a surrogate, and the exception it raises is not a JavaScript error. The language says a surrogate
code point is a legal argument and one code unit — the same thing `fromCharCode` answers — and
twenty-four variants of the suite build strings that way.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/String` and `test/built-ins/RegExp`, and the thirty cases appended to
`src/tests/differential/the-json-date-and-regexp-surface.js`. 2026-09-05.

### JSC-130

**Where:** [JSC-105](#jsc-105), which recorded that the Unicode character database is an open
external dependency and that what it is needed for is *case folding, normalisation and the property
escapes in a pattern*. That entry named the dependency; this one prices it.

**Measured, on the pinned suite.** `test/built-ins/RegExp` scores 1,660 of 3,743 variants. Of the
2,070 that fail:

- **1,170 are property escapes** — `\p{…}`. The generated tests are 442 files: **350 name a Script
  or a Script_Extensions**, 54 name a binary property, and 38 name a General_Category. The platform
  this component runs on publishes General_Category and nothing else, so **the 38 are reachable
  without acquiring anything and the other 404 are not**.
- **302 are the `v` flag** and the set notation it brings, which is a feature rather than a data
  gap.
- **124 are inline modifiers** — `(?i:…)` — likewise.

**What that says about the dependency.** Acquiring the UCD would move about a thousand variants of
one subtree, and nothing else in the suite is waiting on it except normalisation *(JSC-91)* and case
folding. It is the largest single conformance figure attributable to one unmade decision, and the
decision is a licensing and provenance one that the ledger's
[section 3](roadmap.status.md#3-open-external-dependencies) records as a human action rather than a
piece of work anybody here can do.

**Authority and date.** The sweep of `test/built-ins/RegExp` on 2026-09-05 in this checkout, and the
file counts under `test/built-ins/RegExp/property-escapes/generated`. 2026-09-05.

### JSC-131

**Where:** `Function.prototype`, which had no `caller` and no `arguments` at all, and `Math`, which
was missing the two members the language added last and a `Symbol.toStringTag`.

**What was assumed about the two restricted properties.** That removing them is the same as not
having them.

**What was true.** They exist **in order to refuse**. `caller` and `arguments` were how a function
reached its caller's frame; the language took the capability away and put an accessor pair in its
place on `Function.prototype`, both halves of which throw. A strict function reading `f.caller`
therefore gets a `TypeError` — through inheritance, without every function carrying its own
property. Leaving them out answers `undefined`, which is a different answer to the same question,
and **thirty-four variants of `built-ins/Function` look at exactly that**. The subtree went from 733
of 893 variants to 754.

**`Math.sumPrecise` is the one arithmetic method here that a loop cannot implement.** Its answer is
the exactly rounded sum, which a running total does not give: adding left to right rounds at every
step, so `0.1 + 0.2 + 0.3` and `0.3 + 0.2 + 0.1` differ. It keeps a list of non-overlapping partial
sums and rounds once — and **three things about it are easy to get wrong and are each worth a test
the suite has**: an intermediate sum may overflow where the answer does not, so a list with a term
near the top of the range is distilled in a scaled domain and scaled back; the final addition needs
the half-even correction across two partials, without which one of the suite's cases is two units in
the last place out; and `-0` is the identity, so an empty list and a list of nothing but negative
zeros sum to `-0` while one `+0` anywhere makes it `+0`. Nothing is coerced — an element that is not
a Number is a `TypeError` — and the walk stops at that element and closes the iterator.

**`Math.f16round` and `Object.prototype.toString.call(Math)`** complete the namespace: the first is
`fround`'s half-precision twin, and the second answered `[object Object]` for want of a tag.
`built-ins/Math` went from 622 of 654 variants to 652; the two that remain need `BigInt`.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/Function` and `test/built-ins/Math` before and after, and the seventeen cases
appended to `src/tests/differential/the-later-library-methods.js`, ten of which are declared
divergences because the comparison engine predates the two members. 2026-09-05.

### JSC-132

**Where:** the tokenizer's numeric literal, and the front end's treatment of a regular expression
literal — both of which accepted text the language rejects before it ever runs.

**What was assumed.** That a literal is a value the front end reads and hands on, and that whether
its text is well formed is a question the runtime answers when the value is built.

**What was true of both.** *An early error is not a late error that arrives sooner.* It is a
different observable: a script containing one produces **no side effect at all**, because it is
never evaluated. `try { eval("var x = /(/;") } catch (e) {}` leaves no `x` and no output; a build
that constructs the pattern at the point the literal is reached has already run everything above it.
The suite tests exactly that shape — a `negative: { phase: parse }` file is a file that must not run
— and neither of these two was answering it.

**The pattern.** `/(/`, `/a{2,1}/`, `/[z-a]/` and `/\p{Nonsense}/u` are each a `SyntaxError` at parse
time. This build compiled a literal into a `RegExp` construction and left the pattern for the
matcher to reject at run time, so all four ran the program above them first. The front end now
compiles the pattern while it is compiling the literal and **discards the result**: the compilation
is performed for its refusal, not for its output, because the value the guest gets must still be
built by the same constructor the guest could have called itself. That required the matcher to be
visible to the compiler, and it was in the profile assembly, which the compiler must not reference —
so `JsRegExpMatcher` moved to `Broiler.VM.Profile.JavaScript.Format`, where the tokenizer and the
realm can both see it. It became public rather than internal-visible because **rule A10 of
[ADR 0001](decisions/0001-what-this-component-is.md) forbids `InternalsVisibleTo` in a product
project**; a type two assemblies need is a type with a surface, and pretending otherwise is what that
rule exists to stop.

**The separator.** `1_000_000` is a number and `1__0`, `1_`, `_1`, `0x_1` and `1e_5` are each a
syntax error. This tokenizer stripped every underscore and parsed what was left, so **all five were
numbers**. The rule is not that a numeric literal may contain underscores; it is that an underscore
may sit *between two digits of the literal's own radix* — checked per radix, before the digits are
folded into a value. `0_1` is refused for a second reason and refused first: a leading zero followed
by a digit is the legacy octal shape, and the language declines to grow a form it is retiring, so the
test is on the literal's kind and is made ahead of the placement rule that `0_1` would otherwise
satisfy.

**Measured, on the pinned suite.** `test/language/literals` went from 516 of 1,037 variants to
**944**; `test/built-ins/RegExp` from 1,660 of 3,743 to **1,986**. Eleven cases were appended to
`src/tests/differential/the-statement-and-object-surface.js`, each using an indirect `eval` because
this manifest refuses a direct one — which is itself the reason a probe can ask a parse-time question
at all.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/language/literals` and `test/built-ins/RegExp` before and after, and the comparison against
the second engine for every separator form named above. 2026-09-05.
---

### JSC-133

**Where:** the wide surface's lowering — `JsCompiler.EmitStaticStore` — and the remark on it that
defended refusing an assignment to a `const` at compile time as consistency with every other
occurrence of the same mistake.

**What the plan said.** That an assignment to an immutable binding is an early error. The seam half
of [the diagnostic registry](diagnostics/registry.txt) has carried `2204:AssignmentToConstant` since
revision 2, a retained source entry is refused with it, and the source corpus's own remark says a
front end that accepted `const x = 1; x = 2;` would be a front end whose `const` means nothing.

**What was true.** **The language makes it a run-time `TypeError`.** `const x = 1; x = 2;` parses,
compiles and runs, and the failure happens when the assignment executes. Every engine does this, and
it is not a nicety: it is what makes `assert.throws(TypeError, function () { x = 1; })` a program —
a program the conformance suite writes repeatedly, because that is the only way to observe the rule
the specification states. Refusing it at the front end said this manifest does not admit an
assignment, when what it does not admit is the assignment **succeeding**.

**What that cost, measured.** `test/language/statements/const` of the pinned suite — a subtree that
has nothing to do with modules and that this checkout could already run — went from **42 passing and
33 failing to 46 passing and 29 failing** on this repair alone, with no case moving the other way.
Four cases had been failing for as long as the wide surface has had a `const`, and every one of them
is the same shape: `assert.throws(TypeError, function () { x = 1; })`.

Seven further cases of `test/language/module-code` were scored `fail` with the front end's own
refusal as the reason — `instn-iee-bndng-fun`, `instn-iee-bndng-var`, `instn-named-bndng-fun`,
`instn-named-bndng-trlng-comma`, `instn-named-bndng-var`, `instn-star-binding` and
`instn-local-bndng-const`. Six of those are about an **imported** binding and would have been
introduced by this stage had it copied the rule; the seventh is the `const` one again. The cost
outside the suite is a program shape this host could not run at all: the ordinary way to test that a
binding is immutable.

**How it was found.** By writing the immutable-import rule the same way, and then reading what the
module subtree said about it. The suite is emphatic where a person's intuition is not: seven tests
disagreed in the same words, and the seventh named `const` rather than an import — which is what
turned "my new rule is wrong" into "the rule it was copied from is wrong too".

**What replaced it.** One opcode, `ThrowImmutable`, which pops a value and throws a `TypeError`
naming the binding. An assignment to a constant or to an import lowers to a duplicate and that
instruction, so the expression still has its value and the store never happens. **It is deliberately
not terminal**: it always throws, but reachability is a property of the instruction stream, and
marking it terminal would make the return after an assignment at the end of a function body
unreachable code and refuse a correct program.

**What this does NOT change, stated because a reader would reasonably assume it did.** The slice
front end still refuses the assignment at compile time, its retained source entry still records
`2204`, and the registry row is untouched. The slice's manifest has no exceptions and no `try`, so a
run-time `TypeError` there would be an unconditional failure with no way to observe it; the two
front ends genuinely differ, and the code stays reachable through the one that still emits it.

**Authority and date.** The implementation of 2026-09-05 in this checkout, and two runs of the
pinned suite at `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` taken either side of the repair — one over
`test/language/statements/const` and one over `test/language/module-code`. The acceptance row
`runs/an-assignment-to-a-class-binding.js` is the same program from the other side: it was a
`refused/` row asserting `2204` and is now a `runs/` row asserting the `TypeError`. 2026-09-05.

---

### JSC-134

**Where:** [the workload roadmap](roadmap.workloads.md#jsw-8--the-module-goal)'s JSW-8 exit gate:
"a cyclic import terminates with a named diagnostic rather than by exhausting a budget".

**What the plan said.** Read plainly, that a cyclic import is a thing this profile answers with a
named refusal. The contrast the sentence draws is with a budget exhaustion, which is the failure
mode of a resolver that follows specifiers without a module map, and the clause is right to name
that as the outcome to avoid.

**What was true.** **A cycle in the module graph is ordinary, and a correct implementation runs
it.** `a` importing `b` while `b` imports `a` is a legal program every engine evaluates: the module
map stops the second visit, the bodies are evaluated in the order a depth-first walk leaves them,
and what a module of the cycle observes about the other is the temporal dead zone. Refusing every
cycle in order to satisfy the sentence would have refused a program family the pinned suite has a
subtree for, and would have been a conformance exclusion adopted to make a gate read true.

**What is genuinely a cycle with no answer** is a cycle in an export **resolution** — `a`
re-exporting a name from `b` while `b` re-exports the same name from `a` — which names a binding
that exists nowhere, and which a resolver following the chain would walk until something ran out.
That is the case the gate's contrast is about, and it is the one that takes the named diagnostic.

**What that cost, measured.** Nothing was built wrong, because the distinction was drawn before the
linker was written. What it would have cost is stated instead, since that is the decision this entry
records: the `test/language/module-code` subtree of the pinned suite contains cyclic-import cases
that pass in this checkout and would each have been a refusal.

**What replaced it.** Two mechanisms, and both are exercised. Evaluation marks a module as under way
**before** walking its requests, so the module that closes a cycle finds its starting point already
running and returns — which is what makes an ordinary cycle terminate at all. Export resolution
carries the (module, name) pairs it has visited and answers `1618:ModuleExportCircular` on re-entry,
at verification, before anything runs. The retained corpus holds one entry for each, and the
command-line acceptance table holds a row for each — a graph cycle that reaches a value and a
resolution cycle refused by that code.

**And one exclusion this stage does NOT add, recorded because it was drafted and is wrong.** An
earlier draft of this entry excluded top-level `await` from `broiler.javascript.modules`, on the
ground that settling a promise needs a job queue this profile did not have. **That ground stopped
being true before this stage landed.** The surface it was written against had neither `async`
functions nor a queue; the one this stage was re-expressed onto has both, and the conformance
harness drains the queue between invocations. So top-level `await` is **admitted**: a module body
whose parse saw an `await` outside any function carries `FunctionFlags.Async`, graph evaluation is
an ordered list rather than a recursive walk, and a body that suspends registers a continuation that
resumes the list where it stopped — which is what makes an importer wait for a dependency that
awaits. The `top-level-await` subtree of the pinned suite is run rather than skipped, and the
differential probe's ordering cases were compared against the comparison engine before they were
written down.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the
`test/language/module-code` run this stage records, the acceptance rows `modules/cycle-a.mjs`,
`modules/circular-a.mjs` and `modules/top-level-await.mjs`, and the retained probe
`src/tests/differential/the-module-goal.mjs`. 2026-09-05.

---

### JSC-135

**Where:** the executor's `DeleteIndex` arm — the computed form of `delete`, `delete o[k]`.

**What was assumed.** That the two spellings of one operator need one implementation each, and that
the difference between them is only where the key comes from.

**What was true.** They are one operator and the language has one rule for it: a `delete` the object
refuses answers `false` in sloppy code and throws a `TypeError` in strict code. The static arm,
`DeleteProperty`, has carried that rule and a remark explaining it since it was written. The
computed arm dropped the answer on the floor: it pushed `false` and went on, **in strict code as
well**. So `delete o.frozen` threw and `delete o["frozen"]` did not, and a program that reached a
refused delete through a computed key — which is every program that deletes a key it holds in a
variable — was told nothing and carried on as though the property were gone.

**What that cost, measured.** Two cases of `test/language/module-code/namespace/internals` in the
pinned suite, which delete a Symbol-keyed property of a module namespace and expect the throw; a
Symbol key can only be written computed, which is what makes those two reach this arm. The wider
cost is not measured here and is not claimed: the arm is reached by every computed `delete` in the
suite, and the subtree this stage ran is the only figure this entry has.

**How it was found.** By implementing the module namespace's own refusals and finding that the test
asserting `delete ns[Symbol.toStringTag]` throws still failed after the namespace had been made to
refuse it. The namespace was refusing correctly; the operator was discarding the refusal.

**What replaced it.** The computed arm keeps the answer, throws in strict code with the key in the
message, and pushes the same boolean it always did. Nothing about the static arm moved.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the
`test/language/module-code` run this stage records. 2026-09-05.

---

### JSC-136

**Where:** the conformance composition — `Test262Command`, which writes each shard's transcript — and
the module resolvers, which encode a specifier to hand across the capability seam.

**What was assumed.** That a JavaScript string is UTF-8 text, so `Encoding.UTF8.GetBytes` and
`File.WriteAllText` are the way to move one.

**What was true.** A JavaScript string is a sequence of UTF-16 code units and an unpaired surrogate
is a legal one. `Encoding.UTF8` throws `EncoderFallbackException` on it by default, and this
component already knows that: `JsFormat.EncodeText` and `JsFormat.DecodeText` exist because the
constant pool had to carry exactly such a string, and they are WTF-8 for that reason. The
conformance harness did not use them.

**What that cost, measured.** A whole shard of the pinned suite aborted with
`EncoderFallbackException: 55356` **after it had scored every file it was given**, so the run lost
one eighth of its results and reported a crash rather than a score. The suite has a family of module
tests whose export names are deliberately unpaired surrogates — `export-expname-unpaired-surrogate`
and its four neighbours — which is what reaches the specifier encoding; a source file's own text can
contain one too, which is what reaches the transcript writer.

**How it was found.** By running `test/language/module-code`, which is the first subtree this
component has run that contains such a name at all.

**What replaced it.** The engine's specifier encoding and the resolvers' decoding go through
`JsFormat.EncodeText` and `JsFormat.DecodeText`, which is what those functions are for; and the
transcript writer holds one `UTF8Encoding` built with `throwOnInvalidBytes: false`, so a transcript
substitutes where it cannot encode rather than taking the run down with it. A transcript is a report,
and a substitution in one is a legible loss; a crash after the work is done is not.

**Authority and date.** The implementation of 2026-09-05 in this checkout and the
`test/language/module-code` run this stage records, which completes on all eight shards. 2026-09-05.

---

### JSC-137

**Where:** the parser's three tests for whether a `let` at the cursor begins a lexical DECLARATION or
is an ordinary identifier — one at statement position and one in each of the two `for` heads.

**What was assumed.** That the set of tokens a binding name can begin with could be written out as a
literal list, and that writing it three times was harmless because the list does not change.

**What was true.** The list was short by two, and by the two whose membership is CONTEXTUAL rather
than fixed. `yield` is a binding name in sloppy code outside a generator, and `await` is one outside
an async function and outside the module goal — both of which the parser already decides correctly,
in `IsIdentifierName`, for every other position a name may stand in. `let yield = 4;` therefore fell
through to the identifier arm, `let` became an expression statement, and the name after it became the
surprise: `2102:ExpectedToken`, *"`;` was expected and `yield` was found"*, on a program every engine
runs.

**What that cost, measured.** Twelve variants of `test/language/statements/for-await-of` in the
pinned suite, all of them generated from templates that declare `let yield` or `let await` at the top
of the file and never reach the construct under test at all. A conformance runner scores a refused
source as a FAILURE rather than as unsupported, so all twelve were counted against a family that had
nothing to do with them. The same refusal is reachable from a two-line program with no `for await` in
it, which is what makes it a defect in the parser rather than in the family that surfaced it.

**What replaced it.** One predicate, `BeginsLetDeclaration`, asked in all three places: the token
after the `let` is a declaration head when `IsIdentifierName` admits it or when it opens a
destructuring pattern. The contextual rules are then stated once, where they already were, and the
next name added to the identifier set cannot be added to two of the three sites.

**Authority and date.** The implementation of 2026-09-05 in this checkout, and the subtree run of
`test/language/statements/for-await-of` before and after, in which the twelve refusals become passes.
2026-09-05.

### JSC-138

**Where:** the lowering of a function DECLARATION written inside a block, and the environment its
closure captures.

**What was assumed.** That a block-scoped function declaration behaves like the function expression a
reader would substitute for it, so that a body naming a `let` of the enclosing block reaches it.

**What was true.** It does not. The shortest witness is one line and needs no family this stage
admits: `{ let n = 0; function fn() { n += 1; } fn(); print(n); }` answers
`uncaught ReferenceError: n is not defined`, where the same block with
`var fn = function () { n += 1; };` answers `1`. The declaration is hoisted to the enclosing FUNCTION
scope and its closure captures that scope, so the block's own environment record — which is where `n`
lives — is not on the chain the body resolves against.

**What that cost, measured.** Four variants of `test/language/statements/for-await-of`, each of the
shape `{ let iterCount = 0; async function fn() { … } … }`, which the suite generates inside a block
so that the file's own bindings do not leak. The four fail on `iterCount` rather than on anything
they test. The cost outside the suite is not measured here and is not small: the shape is what every
`if (…) { function f() {} }` in sloppy code produces.

**What replaced it.** Nothing, in this bundle. The repair is in the hoisting pass rather than in the
family this stage admits, and it needs the block-level function semantics of Annex B decided before
it is written — a declaration hoisted to the block, with a `var`-scoped alias assigned where the
declaration stands. Recording it is what stops the next reader treating the four failures as an
async-iteration defect.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the witness above run
through the published binary, and the subtree run that names the four. 2026-09-05.

### JSC-139

**Where:** [JSC-101](#jsc-101), which measures the per-frame cost of the executor's dispatch loop and
derives the call-depth bound from it, re-taken for the sixth time — and, this time, the first at
which something had to move.

**What the measurement says now.** Asynchronous iteration adds five arms to the dispatch loop — the
four steps of a `for await` head and the check its close owes — and the executor's own frame grew
from **4,073 bytes to 4,551**. On the sixty-four megabytes `JsExecution.GuestStackBytes` declared,
that is **14,737** calls against a call-depth maximum a host may be granted of 8,192: a factor of
**1.80**, where [JSC-126](#jsc-126) had already recorded 2.01 as the narrowest the margin had ever
been. Below two is not a margin that has narrowed; it is the ordering the ceiling depends on no
longer holding, because a program granted the maximum could reach the stack before it reached the
ceiling and a stack overflow is the one failure the CLR cannot turn into an exception.

**What replaced it.** The guest stack, raised from sixty-four megabytes to **ninety-six**, at which
the same bisection measures **22,122** calls — 2.70 times the grantable ceiling and 3.69 times
`JsEngine.MaximumCallDepth`. The stack is a reservation of ADDRESS SPACE committed a page at a time,
so a program that never recurses pays for none of it; the alternative, lowering the grantable
ceiling, would have answered a question about the machine by changing what a program is allowed to
do, which is the choice [JSC-85](#jsc-85) already declined.

**Both figures are measured on a build with the bounds lifted**, because a bisection that stops at a
declared bound reports the promise rather than the capacity — which is the distinction
`eng/measure-frame-cost.py` prints in as many words when both depths are stopped by the bound. The
script's own `DEFAULT_STACK_BYTES` is stated rather than read, so it moved with the constant.

**Authority and date.** The implementation of 2026-09-05 in this checkout and three bisections: one
against the published binary, which reports both depths stopped by the declared bound; one against a
build with `JsEngine.MaximumCallDepth` and the profile's call-depth maximum lifted, on the
sixty-four-megabyte stack, which reports 14,737; and one against the same build on ninety-six, which
reports 22,122. 2026-09-05.

### JSC-140

**Where:** the lowering of a `const` declared at the top level of a PROGRAM, and every write to it.

**What was assumed.** That the refusal `2204:AssignmentToConstant` covers assignment to a constant
binding wherever one is written, which is what the refusal's own message says.

**What was true.** It covers a constant that resolves to a SLOT. A `const` at program scope with a
block depth of zero is published as a property of the global object instead — which is what makes it
reachable across entry points — and a write to it therefore resolves to no slot at all and is
lowered to `StoreGlobal`, where nothing asks whether the binding was constant. `const c = 1; c = 9;`
answers `9` at the top level of a script and is refused inside a function. A destructuring target
takes the same path, so `[c] = [9]` mutates it too; the pattern is not the cause and the constant is
not protected either way.

**What that cost, measured.** Four variants of `test/language/statements/for-await-of` whose
generated body assigns to a `const` through a `for await` head and asserts the `TypeError` the
language owes — reported as *"Promise incorrectly fulfilled"*, because the assignment succeeded.
Beyond the suite the cost is a silent one: a program that writes to a top-level `const` gets the
write rather than the error, and nothing anywhere reports it.

**What replaced it.** Nothing, in this bundle. The repair belongs with the global binding model
rather than with this stage's families: the specification gives the global environment a DECLARATIVE
part precisely so that `let` and `const` at the top level are not properties, and adding a constant
flag to the global object's property table would answer the assignment while leaving both
enumerable in `for…in`, which they are not. Recording it is what keeps the four failures from being
read as an async-iteration defect.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the witness
`const c = 1; c = 9; c` run through the published binary, and the subtree run that names the four.
2026-09-05.

### JSC-141

**Where:** the parse of a `for` head, which accepts an initialiser on a head that iterates.

**What was assumed.** That a `for … of` or `for … in` head with a declarator carrying an initialiser
is a shape the grammar cannot produce, so the parser could take the single declarator and drop
whatever else was on it.

**What was true.** The grammar cannot produce it and a PROGRAM can write it, which is the difference
between a production and an early error. `for (var x = 1 of []) ;` is a Syntax Error in the language
and this front end accepts it, discards the `= 1` and runs the loop; the same holds for `let` and
`const`.

**What that cost, measured.** Six variants of `test/language/statements/for-await-of` —
`head-var-init`, `head-let-init` and `head-const-init`, each in both modes — which expect a
parse-phase `SyntaxError` and get a program that runs. It is one of several parse-phase early errors
this front end does not implement, and it is recorded rather than repaired because it is the one this
stage's subtree runs measured: the other twenty refusals in the same subtree are escaped keywords, a
strict directive under a non-simple parameter list, and a `let` followed by a line terminator.

**What replaced it.** Nothing, in this bundle. The repair is one test at each of the two head sites —
a declarator with an initialiser reaching an `of` or `in` head is `2101:UnexpectedToken` — and it is
not taken here because the whole class deserves one pass over the early errors rather than the two
this bundle happened to trip over.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the witness
`for (var x = 1 of []) ;` run through the published binary, and the subtree run that names the six.
2026-09-05.

---

### JSC-142

**Where:** the fourth divergence of the [ledger's section 2](roadmap.status.md#2-what-the-runs-found),
the remark at the head of `JsCompiler` that declared it, and the three `dead-zone/` rows of
`src/tests/cli/expected.txt` that pinned it so the day it moved would be a day something went red.

**What the plan said.** That a script-level `let` and `const` may be properties of the global object
rather than bindings of a separate global lexical environment, that the observable difference is a
read before the declaration answering `undefined` instead of throwing, and that **nothing this
profile is built to run depends on either**.

**What was true.** The difference is not one answer, it is three, and the third is the one that
makes the deviation a defect rather than a simplification:

- **`globalThis` shows them.** `const x = 1; Object.getOwnPropertyDescriptor(globalThis, "x")`
  answered a descriptor where every engine answers `undefined`, and `for … in` over the global
  object enumerated them.
- **There is no dead zone.** A read before the declaration answered `undefined`. The temporal dead
  zone was repaired for every other lexical binding on 2026-09-03
  *([JSC-62](#jsc-62))*, and the repair could not reach this one because a property has no
  uninitialised state to be in.
- **A `const` was not constant.** `const c = 1; c = 9;` answered `9`. The assignment reached
  `StoreGlobal`, which writes a property, and the immutability lived in the compiler's slot table —
  which a script-level name never enters. The correction that made a `const` reassignment a
  run-time `TypeError` rather than an early error *([JSC-133](#jsc-133))* said every path through
  the store emits `ThrowImmutable`; at script level there was no path to emit it on, and the
  sibling stage that found this *([JSC-140](#jsc-140))* recorded it against the binding model
  rather than repairing it.

**What replaced it.** The realm carries **the declarative half of the global environment record**
beside its global object: a table of bindings, each with a mutability and an initialised state, in
`JsRealm.Lexical.cs`. Three instructions reach it — `DeclareGlobalLet`, `DeclareGlobalConst` and
`InitialiseGlobalLexical` — and `LoadGlobal`, `LoadGlobalOrUndefined` and `StoreGlobal` ask it
before they ask the object, which is the order the specification's global environment record has
and the reason a script-level `let Array` shadows the intrinsic rather than replacing it.

**It is the REALM's and not the unit's, and that is the whole design.** A slot in the declaring
script's frame would have been simpler and is what this profile does for every other lexical
binding, and it cannot work here: a conformance run evaluates its harness files as separate scripts
in one realm, seven of the pinned suite's harness files publish a helper with a top-level `const`,
and a binding in the declaring frame would be gone before the test that reads it ran.

**One narrower deviation is left in its place, and it is stated rather than removed.** A
re-declaration REPLACES the binding where the language raises a `SyntaxError` before the script
runs. The same lowering serves evaluated source — the dynamic surface hands a String to the
composition's provider and gets a program back, compiled as a script — and the language gives eval
code a lexical environment of its own that is discarded afterwards, so a second
`(0, eval)("let x = 1")` is a program and not an error. Replacing is right for that caller and
lenient for the other; refusing would be right for one and wrong for the other.

**Measured, on the pinned suite.** `test/language/global-code` went from 45 of 75 variants to
**49**, `test/language/statements/const` from 248 of 271 to **254**, and
`test/language/statements/let` from 241 of 287 to **249**. The three `dead-zone/` command lines that
pinned the deviation now answer `1|ReferenceError`, which is what the same three files under
`--slice` have always answered: the two manifests stopped disagreeing about the language.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the three subtree sweeps
before and after, the comparison against the second engine for each of the three answers above, and
the acceptance table's own dead-zone block, whose comment recorded in advance that these rows would
have to be moved deliberately. 2026-09-05.

---

### JSC-143

**Where:** three shapes the wide front end had no production for, found by reading the `unsupported`
and failing columns of a subtree run rather than by an audit: the logical assignment operators over
the two targets they were not admitted for, the strict-mode restrictions on `eval` and `arguments`,
and `delete` applied to a bare name.

**What was assumed about the logical assignment.** That its targets are a name and a property. The
grammar says its target is a `LeftHandSideExpression`, and two more of those are references:
`super.x` and `this.#x`. Both were refused as *a target that is neither a name nor a property* —
which is a MANIFEST refusal, and a conformance runner scores it as a construct declined rather than
as a defect, so it is exactly the shape [bundle JS-4-001](evidence/js-4-001/README.md) section 4
exists to catch. **The private target is the one worth writing down**: `o.#m ??= v` where `#m` is a
private METHOD is a program when `#m` is not nullish, because the store that would refuse it never
runs, and only a lowering whose assigning path is the sole path that stores answers it.

**What was assumed about `eval` and `arguments`.** That strictness changes what a name RESOLVES to.
It also changes what a name may BE. Strict code binds neither of them and assigns to neither, and
both rules are early errors: `"use strict"; var eval = 1;`, `arguments = 1`, `++eval` and
`arguments ||= 1` are programs that do not parse. This front end parsed all four and answered a
run-time `ReferenceError` at script level, where the language answers a `SyntaxError` before
anything runs — and a conformance file written for the rule carries `negative: { phase: parse }`,
so the wrong phase is a failure even when the program does throw. The two names are RESTRICTED and
not reserved, which is why the test is on a string rather than on a token kind: `eval("1")` and
`arguments.length` are strict-mode programs, and the refusal is exactly where the program would
change what the name stands for.

**What was assumed about `delete`.** That its operand is an expression. It is a REFERENCE, and the
operator never evaluates it. The lowering compiled the operand, discarded the value and pushed
`true`, so `delete undeclared` — which the language answers `true` for — threw a `ReferenceError`
about the name, and `var v = 1; delete v` answered `true` where every engine answers `false`,
because a `var` of the global object is not configurable. A name that reaches a slot is answered at
compile time, because a slot binding is never deletable and the compiler already knows which names
those are; everything else reaches the new `DeleteGlobalBinding`, whose three answers are three
different facts about the name — a lexical binding is not deletable, a property answers what its own
`[[Delete]]` answers, and a name neither half carries answers `true` because nothing was there.

**And `delete x` is itself a syntax error in strict code**, whatever `x` names, which is the fourth
early error this entry adds.

**Measured, on the pinned suite.** `test/language/expressions/logical-assignment` went from 120 of
132 variants to **126**, with its `unsupported` column emptied. The six that remain are one shape:
a member reference whose base is nullish must throw before its key is converted, which is an
ordering this executor's indexed access does not have and which is not this entry's.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the subtree sweeps before
and after, ten cases appended to `src/tests/differential/the-statement-and-object-surface.js` whose
answers were taken from the comparison engine before they were written down, and the comparison of
`delete` over five kinds of name against that engine. 2026-09-05.

---

### JSC-144

**Where:** the executor's indexed read and write — `JsEngine.GetIndexed` and `SetIndexed` — and the
six variants of `test/language/expressions/logical-assignment` that [JSC-143](#jsc-143) measured and
left, whose failure was not the operator's.

**What was assumed.** That a computed member access is a base and a key, and that converting the key
to a property key is part of reading it.

**What was true.** **`base[expr]` builds a REFERENCE without converting the key**, and the
conversion happens where the reference is read or written. The order is observable whenever the key
is an object: `null[{ toString() { throw new RangeError(); } }]` is the `TypeError` about the base
and the `RangeError` never happens, because there is nothing to convert a key for. This executor
converted first, so it answered a program's own exception where the language answers a `TypeError`
— and the conformance suite writes the case with a distinguishable exception on both sides
precisely so an engine cannot pass it by accident.

**What replaced it.** Both halves check the base for nullishness before anything the key could run.
**A key that needs no user code is still named in the message** — a String or a Number is converted
by nothing a program wrote — so `null["x"]` keeps the message it had and only the object-keyed case
loses the name it could not have obtained without running code the language does not run.

**Measured, on the pinned suite.** `test/language/expressions/logical-assignment` went from 126 of
132 variants to **132**: the subtree passes whole, and the six that remained after JSC-143 were all
this one ordering.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the subtree sweep before
and after, and three cases appended to `src/tests/differential/the-statement-and-object-surface.js`
whose answers were taken from the comparison engine before they were written down. 2026-09-05.

---

### JSC-145

**Where:** `THIRD_PARTY_NOTICES.md`, whose ingestion table named two pieces of third-party material
and not the third, and rule N13's list of suite directories, which named one of the two the notice
file's mechanism sentence would have to be true of.

**What the plan said, and what was done instead.** The notice file states an obligation in its own
words: *a component that ingests or copies third-party source confirms this scoping, or amends this
file, in the change that introduces the material.* The retained Octane workload — 37 files of
BSD 3-Clause JavaScript, archived at `src/tests/octane/pins/` on 2026-09-04 under the workload
roadmap's last stage — was committed with its upstream licence beside it and **without the notice
entry the same paragraph asks for**. The pin's own README cites the stage that asked for the
archive; nothing cited the obligation the archive triggers.

**What was true, and why it is not merely a missing row.** The mechanism sentence the notice file
writes for the conformance suite — *no project file names the suite directory, and rule N13 asserts
it* — **was not true of this material**, because N13's list held `tests/conformance` alone. Writing
the row without the list entry would have published a claim about a rule that was not making it,
which is the failure mode the whole file is arranged to avoid. So the list gained `tests/octane`
first, and the row follows it.

**The confirmation is recorded as owed rather than supplied.** The row's third column reads
`not yet confirmed`, because the confirmation is a release-facing statement a person makes and this
change is not a person making it. The two rows above it carry a date and a named co-signature; this
one carries the reason it does not.

**Authority and date.** The notice file's own obligation paragraph, the pin at
`src/tests/octane/pins/octane.pin` and the licence retained beside it, and rule N13 as it now
stands, which passes with the second directory in its list and would report a violation for a
project file naming either. 2026-09-05.

---

### JSC-146

**Where:** `JsRealm.Reflect.cs`, and specifically the paragraph of its own header remark that read
*"What is absent is `Proxy`, and the two are usually met together. … Nothing here assumes an ordinary
object in a way that would have to change."*

**What was assumed.** That a namespace whose members are named after the internal methods was
therefore written in terms of them, so an object that answers those methods differently would need
nothing here.

**What was true.** Four of the thirteen members were written in terms of an ordinary object's
STORAGE rather than in terms of its internal method, and each of the four is a different way for the
same assumption to fail.

- **`ownKeys` read the two key tables separately.** `[[OwnPropertyKeys]]` is one internal method and
  this asked for the String keys and then the Symbol keys — which for a Proxy is the `ownKeys` trap
  called **twice**, visibly, with nothing to make the two answers agree.
- **`preventExtensions` assigned instead of asking.** Its whole body was `target.Extensible = false;
  return JsValue.True`, which is right for an object that cannot refuse and is a false report for
  one that can. A refusing trap got `true`.
- **`setPrototypeOf` ran `OrdinarySetPrototypeOf`'s three tests inline** — already-the-same,
  non-extensible, cyclic — against an object entitled to define its own `[[SetPrototypeOf]]`. Two of
  the three also read the proxy's prototype and its extensibility, which is two trap calls the
  language never asks for at that point.
- **`defineProperty` validated the descriptor against the proxy** before reaching the trap that
  decides, for the same reason and at the same cost.

**What that cost, measured.** `test/built-ins/Reflect` scored **286 of 306 variants**, and every one
of the twenty failures was a Proxy case: thirteen `return-abrupt-from-result.js` files that assert a
trap's exception propagates, `preventExtensions/return-boolean-from-proxy-object.js`, and their
strict twins. The entry is not "these could not run for want of `Proxy`": with `Proxy` present and
these four members unchanged they would have gone on failing, which is what makes this a defect in
existing code rather than a gap. The subtree is now **306 of 306**.

**What replaced it.** Each member at its own site. `ownKeys` asks `JsObject.OwnKeys`, a new virtual
that is the concatenation for an ordinary object and one trap call for a proxy.
`preventExtensions` returns what the operation answered. `setPrototypeOf` and
`Object.setPrototypeOf` now share one `ObjectSetPrototypeOrdinary`, which answers the boolean the
specification gives it and which `Object.setPrototypeOf` turns into the two `TypeError`s it owes —
the copy of the cycle walk that stood in `Reflect` for want of a shared body is gone.
`defineProperty` asks a proxy directly and returns its `false` rather than catching a `TypeError` it
should never have provoked.

**One thing the paragraph got right is worth keeping.** The other nine members needed nothing,
because they were already written through the engine's own property paths — which is the arrangement
the paragraph was describing, and it was true of most of the file.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweep of
`test/built-ins/Reflect` before and after, and the cases of
`src/tests/differential/the-proxy-and-its-invariants.js`. 2026-09-05.

### JSC-147

**Where:** the realm, which had no `Proxy` — the last named absence in the `absent-globals` block of
[the status ledger](roadmap.status.md) besides `BigInt` and the two typed arrays that depend on it.

**What was assumed by everything that had to change to admit it.** That the object model's virtual
methods answer about an object's own storage, cheaply, without running anything.

**What was true.** A Proxy answers them with guest code. That is the whole difficulty, and it decides
the shape of the implementation: thirteen internal methods, every one of which may call a function
the program wrote, which may throw, may re-enter, may itself be a Proxy, and **may lie**.

**Where each trap is invoked, and why there.** The decision was taken once per internal method rather
than once for all of them, because the language does not put them in one place.

- **Nine are `JsObject` virtuals**, because for those the specification's internal method and this
  profile's own-property operation are the same operation: `[[GetOwnProperty]]` over both key kinds,
  `[[DefineOwnProperty]]` over both, `[[Delete]]` over both, `[[OwnPropertyKeys]]`,
  `[[GetPrototypeOf]]`, `[[SetPrototypeOf]]`, `[[IsExtensible]]` and `[[PreventExtensions]]`.
  Overriding them means every existing caller — `Object.keys`, `JSON.stringify`, `for…in`, the
  spread of an object literal, all of `Reflect` — traps without being told about proxies at all.
  `Prototype` and `Extensible` became virtual for this, and the storage behind them moved from an
  auto-property to a field so that `base` still has somewhere ordinary to keep an answer, and so that
  the constructor can write the field rather than call an override before the derived type is built.
- **Five are invoked from `JsEngine`**: `[[Get]]`, `[[Set]]`, `[[HasProperty]]`, `[[Call]]` and
  `[[Construct]]`. These are operations over a WHOLE prototype chain or a whole call, and the walk
  belongs to the engine. The test is inside each walk's loop rather than before it, because a proxy
  is as likely to be somebody's prototype as to be the object a program named — and once the walk
  reaches one, the trap decides the rest of it, including whether a prototype is consulted at all.
- **The proxy holds its realm**, because those nine virtuals have no engine parameter to pass one
  through, and adding one would have changed the signature of every property operation in the profile
  for the sake of one object kind.

**The invariants are most of the work and all of the value.** A proxy that forwarded each trap's
answer without checking passes every easy test. What it breaks is everything downstream that had
already looked at the target: a `get` trap may not report a value other than the target's
non-configurable, non-writable one, nor a value at all for a non-configurable accessor with no
getter; `getOwnPropertyDescriptor` may not report a non-configurable property the target does not
have, nor an existing non-configurable one as absent, nor a writable one as non-writable and
non-configurable; `ownKeys` must include every non-configurable own key, may not repeat a key, and on
a non-extensible target must report exactly the target's set; `isExtensible` must simply agree, which
makes it the one trap with no freedom at all; `preventExtensions` may not report a success the target
did not take; and `defineProperty`, `has`, `set` and `deleteProperty` each have their own. **The
duplicate check in `ownKeys` is ordered first on purpose**: every check after it removes keys from a
working copy of the trap's list, and a repeated key would let one target key satisfy two removals,
which is a way to hide a real one.

**What that cost, measured.** `test/built-ins/Proxy` scored **0 of 606 variants** and now scores
**533**. Every one of the 73 that remain fails on `$262.createRealm`, which this profile does not
have and which roadmap [section 13](roadmap.md#13-realms-agents-and-the-host-boundary) owns: each is
the `-realm.js` twin of a case whose single-realm form passes. That is the ceiling for this subtree
until this profile creates nested realms, and no invariant is behind it.

**What a Proxy made the rest of the realm say** is [JSC-148](#jsc-148), [JSC-149](#jsc-149) and
[JSC-150](#jsc-150). The Proxy suite is unusually good at finding that class of defect, because a
trap is a way to ask an operation what it is really doing.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/Proxy`, `test/built-ins/Reflect` and `test/built-ins/Object` before and after, and
the 154 cases of `src/tests/differential/the-proxy-and-its-invariants.js`, every one of which agrees
with `/opt/node22/bin/node` and none of which is a declared divergence. 2026-09-05.

### JSC-148

**Where:** three exotic objects that project an own property instead of storing one —
`JsArray`'s `length`, a String wrapper's `length` and its character indices, and a RegExp's
`lastIndex` — and the `DeleteIndex` opcode.

**What was assumed.** That an override which answers `TryGetOwnProperty` for a synthesised property
has made that property real.

**What was true.** `JsObject.DeleteOwnProperty` answers **`true` for every key it does not find**,
which is correct — deleting an absent property succeeds — and is exactly wrong for a property that is
present but not in the map it searches. All three of these are non-configurable, so:

- **`delete [].length` answered `true`** and deleted nothing; in strict code it answered `true` where
  the language owes a `TypeError`.
- **`delete new String("str").length` and `delete new String("str")[0]`** did the same.
- **A frozen Array's element did the same.** `Object.freeze` moves an element out of the dense store
  into the ordinary map, leaving a hole; the delete saw an index inside the dense range, cleared the
  hole again and answered `true`. `TryGetOwnProperty` beside it already fell through to the map for
  exactly this case and said so in a comment.

**And the refusal had nowhere to go anyway.** `delete a.x` reported a refusal as a `TypeError` in
strict code and **`delete a[k]` did not** — the `DeleteIndex` opcode never consulted the mode — so
the two spellings of one operation gave different answers, and code after a refused computed delete
went on as though the property were gone.

**`lastIndex` is the other half of the same shape.** It is projected from a field, so
`Object.defineProperty(re, "lastIndex", { writable: false })` was accepted, changed nothing, and the
property went on reporting itself writable and going on being written — which is what
`Object.freeze` on a RegExp did too. It now carries a writability bit of its own, in the same shape
`JsArray` already uses for `length`, and the `exec` protocol's two writes are the specification's
`Set(R, "lastIndex", …, true)` and throw where the property is closed.

**What that cost, measured.** These are the last failures of `test/built-ins/Proxy` outside the
realm-gated set. `test/built-ins/String` went from **2,271 to 2,273 of 2,441** and
`test/built-ins/RegExp` from **1,660 to 1,678 of 3,743**. Both subtrees were measured again after
[JSC-149](#jsc-149) and [JSC-150](#jsc-150) and neither moved further, so both figures are this
entry's and `Proxy`'s alone. `test/built-ins/Array` went from **5,232 to 5,276 of 6,115**, summed
over the run's eight shards because the merged report was written in neither run — a shard of that
subtree ends by raising an unhandled `EncoderFallbackException` out of the harness while rendering
a case that builds an astral character, which is a defect in the harness rather than in the
profile, is present at the base commit as well, and is not this bundle's to fix.

**What replaced it.** A refusal at each of the four projections, a hole test in the Array delete, and
the mode check in `DeleteIndex`. The message there does not re-coerce the key, because a Symbol has
no `ToString` and naming it would replace the refusal being reported with a different `TypeError`
about the report.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweeps of
`test/built-ins/Proxy`, `test/built-ins/RegExp` and `test/built-ins/String` before and after, and
cases 125 to 134 of `src/tests/differential/the-proxy-and-its-invariants.js`, each of which was taken
from `/opt/node22/bin/node` before it was written down. 2026-09-05.

### JSC-149

**Where:** `Array.isArray`, and the three other places that asked the same question by asking a
different one.

**What was assumed.** That `IsArray(v)` is `v is JsArray`.

**What was true.** It is a predicate about the object at the END of a chain of proxies, and the
language branches on it in four places that must all agree: `Array.isArray` itself,
`Array.prototype.concat`'s spreading, `Array.prototype.flat`'s descent, and `JSON.stringify`'s choice
between a list and an object.

**And `Object.prototype.toString` asks it too**, which is where the same assumption produced a
second wrong answer of its own. A proxy first carried its TARGET's class name, so
`Object.prototype.toString.call(new Proxy(new Date(), {}))` answered `[object Date]`. It is not a
Date: the tag comes from internal SLOTS and a proxy has none of the ones that make one, so the
language asks it exactly two questions — is it callable, is it an Array — and gives it one of three
tags. `ClassName` is now virtual for that, and a proxy derives it on each read rather than at
creation, because `IsArray` may throw for a proxy over an already-revoked proxy and the
specification puts that refusal at the `toString` call.

**What that cost, measured.** `Array.isArray(new Proxy([], {}))` answered `false`.
`[].concat(new Proxy([1, 2], {}))` appended the proxy whole and had length 1 where the language gives
2. And **`JSON.stringify(new Proxy([1, 2], {}))` produced `{"0":1,"1":2}`** — valid JSON of the wrong
shape, which is the worst kind of wrong answer a serialiser can give, because nothing downstream
fails on it. Three of the four were found by the differential probe against `/opt/node22/bin/node`
rather than by the suite.

**What replaced it.** One `ArrayIsArray` that walks through proxies and refuses a revoked one, and
four callers of it. `SerializeJSONArray` now reads its length as a PROPERTY — which is
`LengthOfArrayLike`, what the specification says, and the only way to ask a proxy how long it is.

**Authority and date.** The implementation of 2026-09-05 in this checkout, and cases 98 to 106 and
147 to 154 of `src/tests/differential/the-proxy-and-its-invariants.js` — the `Array.isArray` cases,
the `JSON.stringify` cases, `concat`, `Array.from`, `Object.assign` and the eight tag cases — every
one of them measured against `/opt/node22/bin/node` before it was written down. 2026-09-05.

### JSC-150

**Where:** the `Object` statics that walk an object's own keys — `freeze`, `seal`, `isFrozen`,
`isSealed`, `getOwnPropertyDescriptors`, `assign`, and the shared body behind `defineProperties` and
`create`.

**What was assumed.** That `OwnPropertyNames()` is an object's own keys.

**What was true.** It is half of them. The Symbol-keyed table is separate storage —
`JsObject` explains at length why, and the reasons are good — but `[[OwnPropertyKeys]]` is **one**
internal method, and every static that means to walk an object completely has to ask for both. Each
of these asked for one, and the branch that would have asked for the other was written at none of the
seven sites.

**What that cost.** A Symbol-keyed property survived `Object.freeze` writable and configurable, and
`Object.isFrozen` agreed the object was frozen — because it asked the same half-question. So an
object keeping state under a Symbol was never frozen by either, and nothing said so. `Object.assign`
silently dropped every Symbol-keyed property, which is how a great deal of code copies an object.
`Object.getOwnPropertyDescriptors`, whose plural is the only thing distinguishing it from its
singular, answered the descriptors of half the object. And `Object.create(p, { [Symbol()]: … })`
returned the object having defined nothing.

**What that cost, measured.** `test/built-ins/Object` went from **6,511 to 6,656 of 6,802 variants**
over the whole of this work, and the subtree was measured again between the two rounds to divide it:
**99 of the 145 recovered variants are `Proxy` existing** (with [JSC-146](#jsc-146) and
[JSC-148](#jsc-148)), and **46 are this entry and [JSC-149](#jsc-149)**. Nothing in the subtree
regressed. The named files are the six `proxy-no-ownkeys-returned-keys-order.js`,
`freeze/frozen-object-contains-symbol-properties-{,non-}strict.js`,
`getOwnPropertyDescriptors/symbols-included.js` and `assign/strings-and-symbol-order.js`.

**What replaced it.** `JsObject.OwnKeys`, the virtual added for `[[OwnPropertyKeys]]`, and two
key-agnostic helpers beside the statics so the branch is written once rather than seven times.

**Two dead predicates went with it.** `ObjectShadowsDenseSlot` had no caller at all;
`ObjectIsUnattributable` had two, and its entire body was `return false` — a question whose answer
had become "nothing" when an earlier correction let an Array's `length` carry attributes after all,
leaving behind a named predicate that read as though it still excluded something. Removing them is
the second half of that correction rather than a new one.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the sweep of
`test/built-ins/Object` before and after (and once between the two rounds, to divide the movement),
the six `proxy-no-ownkeys-returned-keys-order.js` files of that subtree, which are what named the
defect, and cases 135 to 141 of `src/tests/differential/the-proxy-and-its-invariants.js`.
2026-09-05.


### JSC-151

**Where:** `JsCompiler.BindArrayPattern`, and the region rows every lowering in this compiler emits.

**What was assumed.** That an exception region cannot guard an expression. The remark on the array
pattern's lowering said so in as many words: a `for … of` body that throws closes its iterator
because that path has a region, and a pattern *"cannot have one, because a region's handler is
entered at a fixed operand-stack height and a pattern is applied in the middle of an expression
whose stack is not empty."* On that reasoning the lowering declared a divergence and left the
iterator open.

**What was true.** The height is not fixed. It is a FIELD of the region row — `StackHeight` has been
in the exception-region section since format version 1, the verifier has always seeded a handler's
abstract entry at that height plus the one value the executor pushes, and the executor has always
unwound to it rather than to zero. Every one of the three agreed about a mechanism no lowering had
ever used, because until this pattern every region a lowering opened began at a statement boundary,
where the operand stack is empty and zero is the right answer. The compiler wrote a literal `0` into
every row it emitted. **The objection was to the constant, not to the mechanism** — and the compiler
already tracks the operand height it would have needed, in `UnitBuffer.Height`, which is the number
`MaximumStack` is derived from.

The second half of the assumption was that the iterator record had to stay on the operand stack,
because *"a pattern nests, so two of them can be live at once and each nesting level would need a
slot of its own chosen at compile time."* Both halves of that sentence are true and the conclusion
does not follow: the nesting depth is known **while the pattern is being lowered**, so each level
declares a temporary of its own exactly as a computed member target already does, and two live
records never share one.

**What that cost.** An iterator abandoned part-way through a pattern was never given its `return`.
The three completions the language distinguishes were all wrong together: a throw completion left it
open, a normal completion over an unexhausted iterator left it open, and a generator's `return()`
arriving at a `yield` inside a pattern left it open and completed the generator as though nothing
had been abandoned.

**What replaced it.** The record goes to a slot, and the pattern is wrapped in **two** regions whose
declared height is the height the pattern began at. Two and not one because the completion decides
how the iterator is closed: a `catch`-kind region closes it QUIETLY, discarding whatever `return`
raises because the exception already travelling is the one the program is owed, and a
`finally`-kind region closes it LOUDLY, letting an error from `return` through and making a
non-object answer a `TypeError` of its own. The catch region is recorded first so a throw finds it;
the finally region second so a forced return, which passes catch regions over, finds that one. An
iterator that is already done is closed by neither, because `IterateClose` reads the record's own
flag — which is why a rest element, which runs the iterator out, still closes nothing.

**The other design, and why it was refused.** The executor could have closed what an exception left
behind: when a throw unwinds to a handler at height `H`, the values in `stack[H..sp)` are exactly
what the abandoned expression had built, `JsIteratorRecord` is a `JsObject` and so is identifiable
at run time, and its done flag already prevents a second close. It needs no lowering at all, which
is its whole appeal. It was refused for three reasons, in order of weight. **It has nowhere to do
the work in the frame that unwinds without a handler**, and that is the case these tests turn on: a
forced return with no `finally` in the frame leaves through the executor's own dispatch, where `sp`
is a local that never reaches anybody, and buying the case back needs a filter or a catch in every
frame — the shape whose removal that dispatch's own remark records, having killed the process at a
guest `throw` from depth five hundred. **It moves a rule of the language into the executor**, where
the artifact no longer says what closes an iterator and the verifier can no longer check that
anything does. And **it grows the executor**, which is the one budget a lowering-only change leaves
untouched: this change adds no opcode, no dispatch arm and no diagnostic, so the native frame is
byte-for-byte what it was and the call-depth margin re-measured at 2.70× today stands unmoved.
`eng/measure-frame-cost.py` stopped both depths at the declared bound of 5,999, as it did before.

**What that cost, measured.** `test/language/expressions/assignment/dstr` went from **505 to 537 of
640 variants** and `test/language/statements/for-of/dstr` from **1,030 to 1,062 of 1,095**; the whole
of `test/language/statements/for-of` went from 1,294 to 1,326 of 1,436. Sixty-four variants
recovered, none regressed. The named files are the sixteen each tree carries under both names —
`array-elem-iter-{thrw,rtrn}-close{,-err}.js`, `array-elem-trlg-iter-list-thrw-close{,-err}.js`,
`array-elem-trlg-iter-rest-{thrw,rtrn}-close{,-err,-null}.js`,
`array-rest-iter-{thrw,rtrn}-close{,-err,-null}.js` and `array-rest-lref-err.js`.

**`test/built-ins/Array` was measured at both ends and did not move**, at 5,276 of 6,115 variants —
which is the check that matters for this correction, because closing an iterator that should not be
closed is what a change of this shape gets wrong, and the Array built-ins are where a spurious
`return` call would show.

**The remark that declared the divergence is gone**, replaced by one that says what the two regions
are for, why the height is read where it is read, and why the executor-side design was refused. A
reader who finds this entry and the old remark would otherwise still believe the mechanism was
unavailable.

**Authority and date.** The implementation of 2026-09-05 in this checkout; the sweeps of
`test/language/expressions/assignment/dstr`, `test/language/statements/for-of` and
`test/built-ins/Array` before and after; and cases 318 to 334 of
`src/tests/differential/the-statement-and-object-surface.js`, every one of them measured against
`/opt/node22/bin/node` before it was written down. 2026-09-05.

### JSC-152

**Where:** the same lowering, one step earlier: what an array ASSIGNMENT pattern does before it
steps the iterator.

**What was assumed.** That a destructuring assignment stores the way every other assignment in this
compiler stores — value first, reference second. `CompileStoreTo` parks the value in a temporary and
then evaluates the target's base and key, and the pattern called straight into it.

**What was true.** `AssignmentElement` evaluates the target reference **first**, before the iterator
is stepped at all, and it does so for every target that is not itself an object or array literal.
The order is observable with nothing more exotic than a getter: `[ {}[f()] ] = iterable` calls `f`,
`f` throws, and `next` is never called even once.

**What that cost.** One extra `next` per element whose reference throws, which the suite counts
directly. It was worse at a rest element, where the lowering drained the WHOLE iterator into an
Array before evaluating the reference that was going to fail — `[...{}[f()]] = iterable` stepped an
iterator eleven times where the language steps it none, and the same shape with one bound element
ahead of it stepped eleven where the language steps one. These are the counts the recovered files
assert; a lowering that closed the iterator correctly but stepped it first would still have failed
every one of them.

**What replaced it.** `PrepareTarget` evaluates the base, and then the computed key, into temporaries
before the step; `BindPrepared` stores through them afterwards. A name and a nested pattern prepare
nothing, and that is the language's rule rather than an optimisation — an identifier reference is
resolved where it is stored, which is why an assignment to an undeclared name in strict code still
fails at the store.

**What it does NOT change.** An ordinary assignment `o.x = v` still evaluates its value before its
reference. That is a divergence of the same family and it is left standing, because nothing measured
here reaches it and correcting it touches every assignment this compiler lowers rather than the
pattern that this entry is about.

**What that cost, measured.** Not separable from [JSC-151](#jsc-151) and not measured apart from it:
the sixty-four variants that entry names need both corrections, and neither alone recovers any of
them. It is a separate entry because it is a separate assumption, found by reading what the
recovered files assert about `nextCount` rather than about `returnCount`.

**Authority and date.** The implementation of 2026-09-05 in this checkout, the same sweeps
[JSC-151](#jsc-151) names, and cases 318 to 321 and 333 of
`src/tests/differential/the-statement-and-object-surface.js` — the four that count `next` and the one
that counts a computed key's evaluations — measured against `/opt/node22/bin/node` before they were
written down. 2026-09-05.
