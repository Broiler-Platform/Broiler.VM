# Broiler.VM.Profile.JavaScript roadmap — corrections and rejections

**Last updated:** 2026-09-01

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
