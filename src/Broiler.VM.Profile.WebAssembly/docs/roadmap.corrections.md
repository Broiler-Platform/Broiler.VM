# Broiler.VM.Profile.WebAssembly roadmap — corrections and rejections

**Last updated:** 2026-09-26

**This file is part of the [Broiler.VM.Profile.WebAssembly roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split). It carries no numbered section of the
plan, because it is not part of the argument: like the [evidence ledger](roadmap.status.md), it is
a record, and it numbers its own sections.

**What this file is.** The plan's dated history. The roadmap states what is planned, in the
present tense, in one voice. When the core changed underneath it, or a sibling profile's dated
decision settled something it had left open, or the document corrected its own earlier reading,
the roadmap is edited to say the new thing — and the reading it replaced is recorded here, with
its date and its authority.

**What this file is not.** It is not the ledger: [roadmap.status.md](roadmap.status.md) is the
authority for what has been accepted, and nothing here advances a milestone. It answers one
question a roadmap cannot answer about itself: *which sentence of the plan changed, when, and on
whose authority.*

**Why this profile needs one at all**, given that it has written no code. Every correction below
comes from somewhere other than this component's own implementation, and that is exactly why they
are easy to miss *(corrected 2026-09-26: this profile has had code since 2026-09-07, which
[the ledger](roadmap.status.md) records; the sentence was written before it had, and the fifth
source below is the first whose subject is that code)*:

1. **The core moved.** A defect in the core's ceiling resolution was found and removed, and this
   plan had two sections written around it.
2. **A sibling profile went first.** The other intended first profile has landed milestones, taken
   a dated decision series, and met build failures this plan would meet identically. Where its
   findings are the core's rules rather than its own language's, they are this profile's findings
   too, available for free and before the build that would otherwise teach them.
3. **The document corrected itself.** Three of the readings below were caught by reading this plan
   against itself rather than against anything outside it, and each retraction was written inline
   where the reading had been — which is the practice this file exists to end.
4. **The owner instructed an MVP** *(added 2026-09-07)*. An instruction that defers decisions and
   reviews changes which sentences of a plan are load-bearing without changing one fact the plan
   was written from, and the last two entries below come from nowhere else *(noted 2026-09-26:
   [WAC-28](#wac-28) and [WAC-29](#wac-29), which were the last when this was written)*. It is the
   only source in this list that is neither a discovery nor the repair of an error, and it is the
   one most likely to be mistaken for permission: **the plan was not wrong, and it says something different now
   anyway**, which is exactly the pair this file was built to keep visible.
5. **A programme changed what runs a module** *(added 2026-09-26)*. The universal bytecode
   programme's milestone UBC-4 gave this profile a translator into a bytecode every language profile
   shares and an instruction family for it, and deleted the profile's own descriptor, verifier,
   executor and interpreter once the composition roots translated first. The plan was written for a
   profile that verified and ran its modules itself, and [WAC-30](#wac-30) to [WAC-41](#wac-41) and
   [WAC-43](#wac-43) are the readings that no longer hold *(noted 2026-09-26, later the same day: this
   read "[WAC-30] to [WAC-41] are the readings that no longer hold". WAC-43 was filed after the check of
   the programme's bundle `ubc-4-002` found the store's retention still described as the base's;
   [WAC-42](#wac-42), filed with the others, corrects a header stale since 2026-09-07 and is not one of
   them)*. It is the only source in this list that is the tree itself, and
   the one a reader of the plan alone would never find, because the plan's own sentences were all
   true of the profile they described.

A plan with no implementation is not a plan with no history. **It is the one most likely to be
read as though every sentence in it were still load-bearing.**

---

## 1. Reading this file

Every entry carries the same five fields, and an entry with a missing field is an incomplete
record rather than a short one:

| Field | What it holds |
|---|---|
| **ID** | `WAC-nn`, minted once and never reused. Referenced from the roadmap where the correction is load-bearing at the point of reading. |
| **Where** | The roadmap file and section the correction lands in. |
| **What the plan said** | The reading that was replaced, stated fairly enough that someone who planned against it recognises it. |
| **What replaced it** | The reading the roadmap now carries. |
| **Authority and date** | The core ADR, core source, sibling record, or ledger row that settles it, and the date. **An entry with no authority outside this file is not a correction; it is an opinion**, and does not belong here. |

Four rules govern the set:

- **The roadmap never carries a correction inline.** Where the settled statement is enough, the
  roadmap states it and says nothing about what it replaced. Where a reader of that section may
  have planned against the earlier reading, it carries a bare *(corrected: WAC-nn)* and **no
  account of what that reading was** — that account is this file's. Stating the *current* reading
  as forcefully as the section needs is not a summary; it is the plan doing its job.
- **An entry records a changed reading, not a changed sentence.** An edit that sharpens wording,
  fixes a tense or repairs a reference without changing what the plan means gets no entry.
- **An entry is never edited away.** A correction a later change reverses gets its own new entry
  naming the one it reverses.
- **Nothing here is a status claim.** Every entry describes a change to a *plan*. **No milestone of
  this profile has started**; the [ledger](roadmap.status.md) is the authority for every state, and
  no entry below is evidence that any line of this profile has been written *(corrected
  2026-09-26: the bolded clause was itself a status claim, which this rule and update rule 6 forbid,
  and it has been false since 2026-09-07. The ledger says which milestones have started. What stands
  is the rest of the rule: an entry below may describe code, from WAC-30 on, and describing it is
  not evidence of what it does)*.

---

## 2. Corrections

Ordered by the roadmap section they land in, so this file can be read beside the plan.

| ID | Where | In one line | Authority |
|---|---|---|---|
| [WAC-01](#wac-01) | roadmap §3, §17; gates §25 | The core's catalog-wide clamp on profile **maxima** was retracted; only a neighbour's **default** still reaches this profile | core ADR 0001, ADR 0007 |
| [WAC-02](#wac-02) | roadmap §3 | `StructuralDepth` is a ceiling-class dimension the core already supports as a high-water mark | the core's metering surface |
| [WAC-03](#wac-03) | roadmap §3, §12, §20 | No guest-observable `memory.grow` refusal exists on the shipped contract, in any spelling | the shipped core |
| [WAC-04](#wac-04) | roadmap §7 | Two of the malformed-input bullets are `ResourceExhaustion`, not `InvalidArtifact` | the roadmap itself, against the core's status set |
| [WAC-05](#wac-05) | roadmap §13 | A `funcref` is excluded because the capability channel carries values, not because no callback direction exists | roadmap §17 |
| [WAC-06](#wac-06) | roadmap §18 | The persisted key carries the declared hard maxima, not the effective limit vector | core ADR 0006 |
| [WAC-07](#wac-07) | roadmap §20 | The argument channel is graded strong by both profiles, and the two gradings are reconciled | the other profile's dated grading |
| [WAC-08](#wac-08) | ledger; roadmap header; delivery WA-0 | **Placement is settled by the core.** This profile is product projects inside `Broiler.VM`, at a path it already occupies — WA-0 does not decide it | core ADR 0001, 2026-08-31 |
| [WAC-09](#wac-09) | roadmap §5 | The composition root cannot be named `Broiler.VM.Profile.WebAssembly.Composition.*`; that name fires rule A8 on the first build | rules A8, A11, A13 |
| [WAC-10](#wac-10) | delivery §21, WA-0; gates §23 and §25 | The three guest-load **defaults** cannot be `Unconstrained`; the catalog refuses such a descriptor | the core's descriptor validation |
| [WAC-11](#wac-11) | roadmap §5 | A profile's own siblings sit outside the two-assembly reference set, and rule A11 exempts a same-family sibling | ADR 0011 P1, ADR 0001 rev 5 |
| [WAC-12](#wac-12) | roadmap §5 | The cross-profile boundary rule exists and binds this family in both directions today | rule N2 |
| [WAC-13](#wac-13) | delivery §21, WA-0 | The public API baseline cannot be taken by loading a profile assembly; it is described from build output | the sibling profile's dated finding |
| [WAC-14](#wac-14) | gates §19 | The declared repetition count belongs to WA-5, the first milestone whose gate demands a figure — not WA-1 | delivery §21, WA-1 and WA-5 |
| [WAC-15](#wac-15) | roadmap §4.4; delivery §21, §27; gates §23 | The suite's attribution row and the standing-claim confirmation move from WA-0 to WA-4, the milestone that first ingests a suite file | gate 11; ledger section 3 |
| [WAC-16](#wac-16) | roadmap §20; delivery §21, §27; gates §24, §25 | The one opened amendment gets a holder, an unblock condition, and a fallback that is WA-10's release decision rather than WA-5's | [WAC-03](#wac-03); gates §25's own stop condition |
| [WAC-17](#wac-17) | roadmap §20; delivery §21, §27; gates §24 | The amendment register answers nothing at release, but its **state** is published there rather than inferred | gate 1; roadmap §20's unexecutable procedure |
| [WAC-18](#wac-18) | delivery §21, §27; gates §24 | The support table names the declared-default vector's reconciliation as unowned, rather than publishing the vector and stopping | ledger section 3; gate 1 |
| [WAC-19](#wac-19) | roadmap §1, §15 | The ratchet is the first **admitted** totals, not the first *accepted* ones — which nothing could ever have set | WA-4's own exit gate; the ledger's status vocabulary |
| [WAC-20](#wac-20) | delivery §27 | The budget declaration matrix is WA-1's, not WA-0's; the map named an owner two other places contradicted | roadmap §3; WA-0's action list |
| [WAC-21](#wac-21) | roadmap §9; delivery §21 | WA-5's proportionality clause is scoped to the families it ships; its named control was an instruction WA-8 introduces | roadmap §6's manifest allocation |
| [WAC-22](#wac-22) | roadmap §1, §4.3, §5, §15; delivery §21; gates §23, §24, §25 | The harness cannot be a test project: rule A11 forbids one to reference a profile, so it is a never-advertised composition root and the scan changes with it | rule A11; [WAC-13](#wac-13) |
| [WAC-23](#wac-23) | roadmap §9; delivery §21; gates §23, §25 | The `LiveBytes`-breach question the ledger had open becomes a ninth row of the entry-gate decision | ledger's WA-5 row; the map's blank-cell rule |
| [WAC-24](#wac-24) | delivery §21, §27; gates §24, §25; ledger §3 | Two halves of section 7's third obligation were called gate clauses and written into no gate: the dimension that changes side, and the core record this profile falsifies | roadmap §7; the core's metering-split record |
| [WAC-25](#wac-25) | delivery §21, WA-0 and WA-1; delivery §27; ledger §2 | Three clauses of WA-0's gate need a descriptor or a composition root that WA-0's own scope lands neither of; each is carried to WA-1 | WA-0's scope line; rules A11 and A12 |
| [WAC-26](#wac-26) | roadmap §20; ledger §3 | The other intended profile has recorded its position on the refusable retention member — unaffected — so the row's counterweight field is no longer empty | the other intended profile's dated grading, addendum of 2026-09-01; ADR 0003's counterweight row |
| [WAC-27](#wac-27) | roadmap §7; ledger §2 | The core's `DeclaredCountExceeded` status is unreachable in this profile, so the count-ceiling refusal is manufactured here rather than mapped from a core answer | the core's bounded reader, read against section 7's resolution |
| [WAC-28](#wac-28) | roadmap §13; ledger §2, §3 | The memory-growth route is taken without the decision that would have chosen it, and the core-budget arm is published as a named deviation | the MVP programme record; [WAC-03](#wac-03), [WAC-16](#wac-16) |
| [WAC-29](#wac-29) | ledger §2, §3; delivery §21 | The unaccepted core contract blocks acceptance and publication; it does not block writing the code | ledger update rule 8; the MVP programme record |
| [WAC-30](#wac-30) | roadmap §1, §4.3; gates §25 | The profile carries a translator into the universal bytecode — a lowering, not a compiler to native code — and still produces no WebAssembly and has no compiler sibling | the universal bytecode programme roadmap, UBC-4; route MVP-15; ADR 0013 |
| [WAC-31](#wac-31) | roadmap opening, §1, §4.3; delivery opening, §22; gates opening, §24 | No interpreter of its own: its modules run on the universal bytecode's shared bytecode emitter over its family; still no IL and no native form | the programme roadmap, UBC-4; route MVP-14; ADR 0013 |
| [WAC-32](#wac-32) | roadmap opening, §1, §2, §8, §10; delivery opening; gates opening | Decoding and validation are the two phases of one translation a root runs before the core verifies the artifact with the walk and the family hook, and the registry gains a carrier | the programme roadmap, UBC-4; route MVP-15; the profile's diagnostic registry |
| [WAC-33](#wac-33) | roadmap opening, §4.3, §5; gates §24 | The reference set gains `Broiler.VM.Ubc`; still one product assembly and no format sibling, because the shared format lives there; the roots carry the universal bytecode as siblings | ADR 0013; rule W1; route MVP-10 |
| [WAC-34](#wac-34) | roadmap §1, §3, §7, §10, §11, §16, §18; delivery opening; gates opening | The module is source: the core verifies the universal bytecode artifact the translator writes, and a refused module is answered by the translator in the fields the core answered | route MVP-15; bundles `ubc-4-001` and `ubc-4-002` |
| [WAC-35](#wac-35) | roadmap §1, §3, §9, §12, §14; delivery §21, §22; gates §25 | The nine value, store and frame rows are answered in WAD-0003, and the entry gate they guarded is retired with the interpreter | WAD-0003 |
| [WAC-36](#wac-36) | roadmap opening, §1, §3, §4.3, §10, §14; delivery opening, §22; gates opening | Every core stage is answered by the universal bytecode with this profile's family inside it, and the profile implements two of the seven core-facing types | the programme roadmap, UBC-4; the universal bytecode's family contract |
| [WAC-37](#wac-37) | roadmap §11; this file §5 | Reading C's store has no executor of this profile's to live in: the family contract gives a family per-instance state and nothing scoped to a runtime | the universal bytecode's family contract |
| [WAC-38](#wac-38) | roadmap §3, §9, §13, §17, §20; delivery §21 | The memory representation is chosen by WAD-0001 at UBC-4 — a pinned array reallocated on growth — without the amendment and not as WA-5's act | WAD-0001; the programme's decision UBC-D-2 |
| [WAC-39](#wac-39) | roadmap §1, §16 | Every image that runs a module carries a lowering, so the `execution-only` label is not claimed | the execution root's catalog; `docs/compositions.md` |
| [WAC-40](#wac-40) | roadmap §3, §17 | The opcode-set half of the core's refusal to share semantics was reached by ADR 0013; no cross-profile value channel exists or is coming | ADR 0013; route MVP-12 |
| [WAC-41](#wac-41) | roadmap §3, §18, §20 | The translator is an in-process producer, so the byte round trip is a cost this profile pays; the input form is not asked for and no longer refused | route MVP-15; the programme roadmap, UBC-4 |
| [WAC-42](#wac-42) | roadmap header, its split rules, §5 | The header said no line of this profile was written; it now states only what is not accepted, pinned or retained, and leaves what exists to the ledger | the ledger's section 2, 2026-09-07 |
| [WAC-43](#wac-43) | roadmap §1, §3, §10, §13; ledger §2 | The store charges retention before it allocates, so a growth whose retention is refused is never observed as a success, and a growth a core budget refuses ends the step; what the store retained is released on every failure the emitter answers, not on every path that publishes no instance | WAD-0001 and WAD-0003, 2026-09-26; the programme's bundle `ubc-4-002` |

### WAC-01

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses)
and [section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly); the
stop conditions of gates [section 25](roadmap.gates.md#25-risks-and-stop-conditions); the ledger's
unopened-dependency table.

**What the plan said.** The core clamped every runtime ceiling to the tightest **maximum** in the
catalog, so two profiles in one image constrained each other through their maxima as well as their
defaults. Section 3 said all of the cross-profile hazard about the maxima, and section 17's third
cross-profile point said the maxima clamped each other too.

**What replaced it.** The clamp was a defect against the core's own record — which always placed a
profile maximum at verification, against the profile the artifact names — and it has been removed.
**This profile's maxima are its own business**: they bind its own modules and reach no profile
composed beside it.

**What survives, moved one column across.** A neighbour's tight **default** still reaches this
profile wherever a host adopts defaults rather than stating ceilings, because at runtime creation
no profile has been selected and the tightest default in the catalog is the only safe answer. It is
a smaller exposure — a host that states explicit ceilings never meets it — but not an absent one,
and section 17 keeps it as a coordination obligation between two independently owned components.

**Authority and date.** The core's removal of the clamp and the correction of the core records that
described it, 2026-08-31.

### WAC-02

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses),
the `StructuralDepth` row of the budget matrix.

**What the plan said.** That the core did not support charging `StructuralDepth` as a high-water
mark, so a long function of many sequential shallow blocks would accumulate a running total and be
refused for depth it never actually reached.

**What replaced it.** The core does support it, and the row was reasoning from `TryCharge` alone.
The metering surface has four members, and the **retain/release pair exists precisely for the eight
ceiling-class dimensions**, of which this is one: only a ceiling-class dimension releases, and an
allowance never refunds. So the discipline is charge on entry, release on exit, and the refusal
lands exactly at the ceiling — the failure mode the earlier reading feared is the one the pair
prevents.

**The earlier reading was internally inconsistent as well as wrong**, which is the part worth
keeping: this profile already applies exactly that discipline to `LiveBytes`, declared earlier in
the same table as reported on growth and released on disposal.

**Authority and date.** The core's metering surface, read against the row, 2026-08-31.

### WAC-03

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses),
[section 12](roadmap.md#12-traps-exhaustion-and-why-neither-is-a-process-failure), and
[section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight).

**What the plan said.** That a refused `memory.grow` could be made guest-observable locally, by
admitting or refusing growth on a `TryCharge` of `AllocatedBytes` with `LiveBytes` reported for
accounting only. On that reading the requirement was satisfiable inside this profile and no
amendment was needed.

**What replaced it.** It does not work against the shipped core. **A refused `TryCharge` at any
scope latches exhaustion on the meter, and the core then rewrites the completed step as
`ResourceExhaustion` regardless of what the profile did with the `false` it was handed.** So a
charge cannot serve as a refusable, guest-observable check, and the retention report returns
nothing, so a ceiling-class dimension cannot carry a guest-observable refusal either. **There is no
spelling of a guest-observable `memory.grow` refusal on the shipped contract at all.**

**Why this is the most consequential entry in the file.** Section 12 requires a refused
`memory.grow` to be guest-observable and non-terminating — the module decides what to do and the
operation continues — and that is the specification's behaviour rather than this profile's
preference. So the gap is not a local design problem to be worked around: **WA-5 cannot choose a
memory representation until it is resolved**, and section 20 carries a refusable retention member
as this profile's one *blocking* ask rather than a filed one. **The other intended profile's dated
grading records no position on it** — no row of its amendment table is this one — so
the core's procedural question about the other profile is, for this row, still unanswered.

**Authority and date.** The shipped core's metering behaviour, read rather than inferred from the
contract, 2026-08-31.

### WAC-04

**Where:** roadmap [section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core),
the list of what the decoder rejects.

**What the plan said.** That every bullet in the malformed-input list produces the same outcome
category, `InvalidArtifact` with a decode reason, a diagnostic code and a byte position.

**What replaced it.** Two bullets do not. **A vector length beyond the effective declared-count
ceiling, and structural depth and artifact bytes beyond theirs, are `ResourceExhaustion` naming one
dimension and one scope** — the module is well formed and this image declined to admit it, which is
the rule
section 8 applies to every other implementation limit and the one the core states for the whole
bounded-read status set.

**Why the distinction is not cosmetic.** Every corpus entry pins its observed ⟨outcome, reason,
diagnostic code⟩ triple and replays it across three publish modes, so **an entry recorded under the
wrong category passes** and encodes the wrong answer permanently — and the published mapping table
would have been built wrong from the first entry.

**Authority and date.** The roadmap itself, read against the core's bounded-read status set,
2026-08-31.

### WAC-05

**Where:** roadmap [section 13](roadmap.md#13-memories-tables-globals-and-the-host-boundary).

**What the plan said.** That a `funcref` crossing to the host is excluded because no callback
direction exists.

**What replaced it.** A callback direction does exist —
[section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly) records
that cross-runtime reentry is legal and is exactly how a host calls back into a guest. The real
reason is narrower and survives: **the capability channel this profile binds carries values, not
callable references**, and manufacturing a callable host object out of a store-owned function
reference would mean this profile publishing a projection whose identity, lifetime and reentrancy
rules it would then own on the embedder's behalf.

**What that changes about the plan.** The exclusion stands, but its scope shrinks: once the
reference-types manifest opens, an exported `funcref` is representable inside the store and in a
result payload as an opaque profile-owned reference. It is not projectable as a *callable host
object*, which is a much smaller claim than "excluded".

**Authority and date.** The roadmap's own section 17, read against the bullet, 2026-08-31.

### WAC-06

**Where:** roadmap [section 18](roadmap.md#18-persistence-and-the-code-cache).

**What the plan said.** That the persisted cache key includes the **effective limit vector**.

**What replaced it.** It does not, and cannot. The effective vector is part of the handle's
*in-process* identity — which is why two runtimes with different ceilings do not share a handle —
but it is a timing-dependent, process-local quantity, and persisting it would produce a key that
never recurs. What the key carries instead is the composition-invariant half: **this profile's
declared hard maxima**, which is what actually varies between two images and does recur.

**Correctness does not depend on it.** Loading always re-validates, and re-validation recomputes
the vector, so a persisted module never carries a ceiling decision forward.

**Authority and date.** The core's persisted-envelope key set, which states the term-by-term split
between in-process handle identity and the persisted key, 2026-08-31.

### WAC-07

**Where:** roadmap [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight).

**What the plan said.** That the argument channel was this profile's ask and the other intended
profile graded the same capability **weak**, reasoning from a browser that compiles a *program*
rather than a call.

**What replaced it.** The other profile re-graded it **strong**, on the same arguments-only scope,
and recorded why: a fixed-entry-point profile stops being unaffected the moment it hosts this one,
because an export call is a typed call whose arguments originate over there. **The two gradings
are reconciled, and this row is filed rather than blocked** — the core's procedure asks each
amendment record to state the other profile's position, and that position now agrees.

**What did not change.** The scope is still arguments and not results. The typed payload already
carries results, and several of them, so multi-value returns are expressible today; filing argument
and result as one amendment would put two differently-scoped versions of one capability into the
register, which is how a capability gets approved at the wrong width.

**Authority and date.** The other intended profile's dated grading, 2026-08-31.

### WAC-08

**Where:** the [ledger](roadmap.status.md)'s opening, the roadmap header, and delivery
[section 21](roadmap.delivery.md#21-milestones), WA-0.

**What the plan said.** That this component has no repository of its own yet, that its two
documents are staged in the aggregate repository beside the components they will sit next to, and
that **WA-0 owns the placement decision** — moving them being part of that decision rather than a
side effect of it.

**What replaced it.** **The core took the decision, and this profile already occupies the answer.**
A dated revision of the core's topology record rules that *a language profile is a set of product
projects in the Broiler.VM component rather than a component of its own*, and names the two paths
it fixes — including `src/Broiler.VM.Profile.WebAssembly/`, which is where these documents sit as
you read them. A profile's roadmap documents live inside the project directory whose assembly they
describe.

**What follows, and none of it is WA-0's to decide any more.** The assurance system, the
architecture rule register and the licence and notice files are the **host component's**, adopted
rather than duplicated, because one repository policy implemented twice is the drift the platform's
assurance policy exists to prevent. What a profile still stands up of its own is the part adoption
cannot supply: its **evidence-bundle contract and collection script**, because a bundle collected
by the host's script would merge two ledgers, and its **own group in the rule register**.

**What WA-0 still owns.** Everything that is this profile's rather than the topology's: the profile
ID and package identity, the assembly topology within the placement, the manifest allocation and
the `DET` position, the composition label, the two limit vectors, and the specification and suite
pins. The milestone did not shrink to nothing; it lost one decision and gained a settled premise.

**Authority and date.** The core's topology record, dated revision of 2026-08-31, which names this
profile's path explicitly rather than by implication from the sibling's.

### WAC-09

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph), the
candidate-assembly table.

**What the plan said.** That the composition root is named
`Broiler.VM.Profile.WebAssembly.Composition.Execution`.

**What replaced it.** It cannot be, and the reason is a collision rather than a preference.
**Every architecture rule that identifies a profile assembly keys on the `Broiler.VM.Profile.`
prefix**, so a composition root under that prefix *is* a profile assembly to those rules — and the
rule that forbids a profile project from referencing `Broiler.VM.Runtime` would fire on the first
build, because a composition root must reference the runtime. The rule that bounds where a profile
assembly may be referenced from reads a **path** allow-list, `src/compositions/`, which that name
does not put the project under either.

**The name follows the core's own convention**: `Broiler.VM.Composition.WebAssembly.Execution`,
under `src/compositions/`, beside the composition roots already there.

**This is the sibling profile's finding, not a prediction.** It proposed the same shape, built it,
and the rule fired. Recording it here costs nothing and saves this profile the same cycle.

**Authority and date.** Rules A8, A11 and A13 as registered — **A12 and the composition register
are what hold the corrected name**, which is the other half of why the rename is the right fix —
and the sibling profile's dated record of A8 firing on its first build, 2026-08-31.

### WAC-10

**Where:** delivery [section 21](roadmap.delivery.md#21-milestones), WA-0's next action and exit
gate; the evidence matrix of gates [section 23](roadmap.gates.md#23-test-and-evidence-matrix) and
the stop conditions of [section 25](roadmap.gates.md#25-risks-and-stop-conditions); the ledger's
WA-0 row.

**What the plan said.** That WA-0 publishes the three guest-load **defaults** as `Unconstrained`,
with the reason recorded.

**What replaced it.** **The catalog refuses such a descriptor.** The core's descriptor validation
rejects any `LimitDefaults` carrying an unconstrained slot, and the reason beside it is that a
default meaning unbounded would make adopting the profile default identical to declaring no ceiling
at all — which the core's invariant that omission never means unbounded forbids. A profile's hard
**maxima** may use `Unconstrained`; its **defaults** may not.

**What WA-0 publishes instead** is a **large finite value, stated as a number, with the reason
recorded** — and with what the number does not buy recorded beside it: a finite default still
participates in the catalog-wide tightest-default fold, so a host that adopts defaults still gets
the tightest of them, and the cross-profile hazard is bounded rather than removed. Naming the
residue is the point; a default that looked free would hide it.

**Why this entry exists at all.** Roadmap section 3 already carried the corrected reading while the
delivery file still carried the retracted one — **two files of one plan disagreeing on a value that
would have failed at catalog construction**. The correction had been made in the argument and not
propagated to the milestone that executes it, which is the failure the chapter-to-milestone map in
[section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map) now exists to catch.

**Authority and date.** The core's descriptor validation, read directly, 2026-09-01.

### WAC-11

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph).

**What the plan said.** That the profile's Broiler.VM reference set is exactly the two core
assemblies, full stop — written when it was an open question whether a profile's own sibling
assembly counted as a member of that set.

**What replaced it.** The set is of **Broiler.VM-owned** assemblies, and a profile family's own
siblings — a format assembly, a lowering, a composition root — are not members of it. The core
states this in its profile-obligation record, and its topology record exempts a sibling **in the
same profile family**, keyed on the language segment, from the rule that otherwise forbids
referencing a `Broiler.VM.Profile.*` assembly from outside a composition root.

**Why it matters here even though this profile has one assembly.** Section 5 keeps a second product
assembly available as a dated decision — an execution-only image carrying no vector interpreter is
a real product. **That split is legal when it is taken**, and would not have been under the earlier
reading. What stays illegal in both readings is an edge to *another language's* family, which the
exemption is deliberately keyed to exclude.

**Authority and date.** ADR 0011's obligation P1, editorial revision of 2026-08-31, and ADR 0001
revision 5.

### WAC-12

**Where:** roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph).

**What the plan said.** That no edge in either direction reaches any other Broiler profile
**component**, to be asserted by an architecture rule with a passing witness and a negative
control — written as work for WA-0.

**What replaced it.** Two things. The unit is a **family**, not a component, which follows from
[WAC-08](#wac-08). And **the rule exists and passes today**, in both directions, with its own
witnesses: it was minted by the sibling profile's first milestone and its subject is every
`Broiler.VM.Profile.<Language>` family, this one included. WA-0 inherits a rule rather than writing
one, and what it owns is the negative control that proves the rule bites on *this* family's
projects once they exist.

**Authority and date.** Rule N2 as registered, 2026-08-31.

### WAC-13

**Where:** delivery [section 21](roadmap.delivery.md#21-milestones), WA-0's exit gate.

**What the plan said.** That WA-0's gate is met when "the public API baseline mechanism exists and
compares in both directions, with an injected member failing it and a deleted member failing it
too" — with no statement of how a baseline over a profile assembly is taken.

**What replaced it.** The obvious route does not work, and the sibling profile established that at
cost. The host component's describer describes a surface by **loading** an assembly, which needs a
project reference that the architecture rules forbid a test project to hold on a profile. The route
that works describes the family from its **build output** with a metadata load context, which
reflects **without running anything** — so it needs neither the reference the rules forbid nor the
execution this profile's own invariants forbid.

**What WA-0 should expect.** The clause is real and stays; what changes is that it is not free, and
a milestone that budgets an afternoon for "stand up the baseline" has mis-scoped it. The sibling
carried this clause open across two milestones before closing it.

**Authority and date.** The sibling profile's dated record of the obstacle and the route taken,
2026-08-31.

### WAC-14

**Where:** gates [section 19](roadmap.gates.md#19-measurement-discipline), and WA-5's exit gate in
delivery [section 21](roadmap.delivery.md#21-milestones).

**What the plan said.** That the declared repetition count — the number without which "retained
repetitions" is a release gate nobody can fail — is **fixed at WA-1**.

**What replaced it.** WA-1 produces no figure. Its gate is the contract loop: the five verifier
outcomes, the five step kinds, a descriptor admitted by a catalog, a composition that publishes and
runs. Nothing in it is a measurement, so a rule binding the first bundle that publishes a figure
had been assigned to a milestone that publishes none.

**The count belongs to WA-5**, which is the first milestone whose gate demands a retained
measurement: the native frame cost it derives the `CallDepth` default from, and the proportionality
fixture it retains per operation family. WA-5's exit gate now carries the clause that declares it —
no gate carried it under the old reading either, which is the second half of the defect — and
WA-10's measurement lane uses that count rather than fixing a second one, because a count fixed by
two milestones is not a fixed count.

**Why this is worth an entry on a profile that has built nothing.** It costs nothing to correct now
and would have cost a milestone later: WA-1 would have closed without the number, and WA-5 would
have retained figures under rules naming a milestone that never set them.

**Authority and date.** The delivery file's own WA-1 and WA-5 gates, read against the rule,
2026-09-01.

### WAC-15

**Where:** roadmap
[section 4.4](roadmap.md#44-licence-attribution-and-one-notice-that-must-change); delivery
[section 21](roadmap.delivery.md#21-milestones), WA-0 and WA-4; delivery
[section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map); gates
[section 23](roadmap.gates.md#23-test-and-evidence-matrix).

**What the plan said.** That **WA-0** lands the row in the host component's notice carrying the
ingested suite's attribution, and confirms or amends the core's standing third-party claim against
what this profile's tree will contain, with the release owner co-signing. It was an exit-gate clause
of a milestone that deliberately writes no product code.

**What replaced it.** WA-0 **records the obligation**, names its owner, and names the release owner
who co-signs it. **WA-4 lands the row**, in the same change that first ingests a suite file, with
modified files marked and the standing-claim confirmation taken there too. WA-10 re-confirms against
the shipped tree, which it already did.

**Why the earlier reading could not close.** What an attribution row carries forward is the ingested
material's own notice content, and this tree holds none of it at WA-0: the suite revision is not
resolved until WA-4, retrieving and archiving third-party material is a human action the ledger
records as unperformed, and the standing-claim confirmation is a claim about tree contents that do
not exist yet. A row written at WA-0 would have been an attribution for material nobody had
retrieved — which is the failure gate 11 exists to prevent, not an early discharge of it — and
a confirmation written there would have been a claim about a hypothetical tree.

**What is unchanged.** The obligation itself, its owner, the co-signature, and the stop: an
attribution obligation discovered during a publish is a stop, and adopting the host component's
notice file discharges none of it.

**Authority and date.** [Release gate 11](roadmap.gates.md#24-release-gates), read against
[section 4.3](roadmap.md#43-what-is-acquired-rather-than-written), which makes retrieval a human
action and puts the suite's commit at WA-4, and against
[section 3 of the ledger](roadmap.status.md#3-open-external-dependencies), whose conformance-suite
row records the revision unpinned and the licence consequences unconfirmed; 2026-09-01.

### WAC-16

**Where:** roadmap
[section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight); delivery
[section 21](roadmap.delivery.md#21-milestones), WA-5 and WA-10; delivery
[section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map); gates
[section 24](roadmap.gates.md#24-release-gates) and
[section 25](roadmap.gates.md#25-risks-and-stop-conditions).

**What the plan said.** The refusable retention member is *opened rather than filed*, because
[WAC-03](#wac-03) establishes there is no local resolution and WA-5 cannot choose a memory
representation without it — stated beside the observation that the amendment procedure is
unexecutable, with no holder, no unblock condition, and nothing said about what WA-5 does if the
answer never comes.

**What replaced it.** The row is written down as the external dependency it is. **Holder:** the
core's contract and release owners. **Unblock condition:** a minted amendment carrying a
co-signature, or a recorded refusal. **An unanswered row makes WA-5 `Blocked` rather than merely
late** at the moment WA-3 would otherwise let it start, recorded with its blocker in the ledger like
any other *(superseded 2026-09-07 by [WAC-28](#wac-28): under the MVP programme it does not, because
the route WA-5 takes needs no amendment; the row stays open with the same holder and the same
unblock condition)*. And the fallback — a memory whose growth
refusal is not guest-observable — is **WA-10's release decision**, published in the support table
as a named deviation from what the specification says `memory.grow` answers, rather than WA-5's to
take quietly in order to keep moving.

**Why the earlier reading was not enough.** "Opened" described this profile's intent and not the
programme's state. An amendment that cannot be minted is not an answer that is merely late, so a
milestone gated on one has the shape of a blocked milestone and belongs in the ledger as one —
and a plan that names no fallback is a plan whose fallback is taken by whoever reaches the wall
first, unpublished. Naming it as a release decision is what keeps
[gate 1](roadmap.gates.md#24-release-gates) able to see it.

**What is unchanged.** The grade — strong, blocking, and the counterweight test passing — and
the absence of any local resolution. Nothing here makes the row admissible, and nothing here
weakens WA-5's exit gate: a guest-observable growth refusal is still what that gate asks for, and a
release that ships the fallback ships a published deviation rather than a quiet one.

**Authority and date.** [WAC-03](#wac-03), read against
[section 25](roadmap.gates.md#25-risks-and-stop-conditions)'s own stop condition that a milestone
blocked by a named external dependency is recorded with its holder and its unblock condition;
2026-09-01.

### WAC-17

**Where:** roadmap
[section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight); delivery
[section 21](roadmap.delivery.md#21-milestones), WA-10; delivery
[section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map); gates
[section 24](roadmap.gates.md#24-release-gates), gate 1.

**What the plan said.** Every row but one filed and held, none admissible until it names a merged or
approved capability, and the procedure unexecutable. The map recorded the consequence as a blank:
the chapter appears in no evidence area and closes no release gate.

**What replaced it.** The blank was right about the **answers** and wrong about the **state**. Every
held row is a capability this profile does not provide — a multi-result host import refused
rather than truncated, a `v128` split under a published encoding — and gate 1 already refuses a
support table that leaves an unimplemented capability unnamed. So **WA-10 publishes the register's
state**: per row, filed, held or opened, the deterministic failure or exclusion it leaves standing,
and that the procedure is unexecutable and why. The gate is over the publication and never over the
answer.

**What this does not change.** No row becomes admissible because a release names it, no grade moves,
and the counterweight table's refusals stay recorded rather than blocking.

**Authority and date.** [Gate 1](roadmap.gates.md#24-release-gates), which admits no unnamed
unimplemented capability, read against section 20's own record that the procedure is unexecutable;
2026-09-01.

### WAC-18

**Where:** delivery [section 21](roadmap.delivery.md#21-milestones), WA-10; delivery
[section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map), the chapter-3 row; gates
[section 24](roadmap.gates.md#24-release-gates), gate 1.

**What the plan said.** WA-0 publishes the fifteen defaults with the cross-profile consequence
stated in the decision, the *Composed-profile safety* evidence row proves a neighbour feels them,
and [section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly) names
the reconciliation as belonging to whichever component composes both. Nothing required the
**support table** to say any of it, so a reader of the published table met a default vector with no
statement about who reconciles it.

**What replaced it.** WA-10 publishes the vector as the neighbour-facing half **with its
reconciliation named as unowned**, and gate 1 refuses a table that does not. The reconciliation is
not deferred to the release and is not this profile's to take at any milestone: the component that
would own it does not exist and has no owner.

**Authority and date.** [Section 3 of the ledger](roadmap.status.md#3-open-external-dependencies),
whose cross-profile row records that no component composing two profiles exists or is planned, read
against [gate 1](roadmap.gates.md#24-release-gates)'s rule that no row reads as a bare yes;
2026-09-01.

### WAC-19

**Where:** roadmap [section 1](roadmap.md#1-terminology-and-support-claims), the ratchet row, and
[section 15](roadmap.md#15-the-conformance-oracle).

**What the plan said.** That the ratchet is "the first **accepted** per-assertion-family totals for
a manifest", and that a suite-revision change re-bases the floor "from the first accepted run on
the new revision".

**What replaced it.** *Admitted by the milestone that scores it*, not *accepted*. The two words
name different things in this programme and the ledger defines the second: `Accepted` additionally
requires an owner and a reviewer decision, which nothing in this component has and which no
milestone can grant itself. **The plan already required a ratchet before any of that could exist**
— WA-4's own exit gate says its run "sets the ratchet for those two families and for no
others" — so the earlier wording made the floor unsettable by the milestone the plan tells to set it, and
every later "unregressed against its ratchet" clause would have been comparing against nothing. A
floor is a measurement discipline, not a status.

**What it does not change.** The floor still records the pinned suite revision it was set under, is
never compared across revisions, and re-bases from the first run admitted on a new one. Only the
word that gates it moved.

**Authority and date.** WA-4's own exit gate read against
[the ledger's status vocabulary](roadmap.status.md#status-vocabulary); 2026-09-01.

### WAC-20

**Where:** delivery [section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map), the
chapter-3 row.

**What the plan said.** That chapter 3 is delivered by "WA-0 (the two vectors **and the matrix**);
WA-1 (the descriptor)".

**What replaced it.** WA-0 delivers the two vectors; **WA-1 delivers the budget declaration
matrix**, in the descriptor that carries it. Roadmap
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses) has always
said "the intended matrix, which WA-1 fixes", and WA-0's next-action list never mentioned a matrix
— so the map named an owner two other places contradicted, and a reader planning WA-0 from the map
would have scoped a decision into it that its own gate could not close.

**Why it read that way.** The row is the shape a sibling profile's map row has, where the matrix
genuinely is the boundary milestone's because that profile fixes it in the same dated record as its
two limit vectors. The shape transferred; the ownership did not.

**Authority and date.** Roadmap section 3 and WA-0's own action list, read against the map's
blank-cell rule; 2026-09-01.

### WAC-21

**Where:** roadmap [section 9](roadmap.md#9-the-value-store-and-frame-model), the proportional
charging subsection; delivery [section 21](roadmap.delivery.md#21-milestones), WA-5's exit gate.

**What the plan said.** That WA-5's gate is met when "a proportionality fixture exists for each
named operation family of section 9", with "`memory.fill` over a large memory" as the named
negative control that a flat charge fails.

**What replaced it.** A fixture for each named family **that the milestone ships**, which at WA-5
is `memory.grow` and nothing else — and `memory.grow` over a large delta as WA-5's negative
control. Section 9's family list is the whole set this profile will ever charge proportionally, and
most of it is unreachable at WA-5: the bulk-memory and table families arrive at WA-8, segment
initialisation at WA-7, and `array.*` at the manifest that admits it. **As written the clause was
unsatisfiable**, and its named control was an instruction the WA-5 interpreter cannot execute — so
a milestone reading its own gate literally would have had to widen its manifest to close it, which
is the move [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) exists
to forbid.

**What it does not change.** The rule itself is untouched: an operation family without a
proportionality fixture does not ship in the increment. What moved is which increment each family
belongs to, which the plan now states rather than leaving to a reader to infer from the manifest
table.

**Authority and date.** WA-5's own scope statement read against
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s manifest
allocation, which puts bulk memory in `core2`; 2026-09-01.

### WAC-22

**Where:** roadmap [section 1](roadmap.md#1-terminology-and-support-claims) non-goals,
[section 4.3](roadmap.md#43-what-is-acquired-rather-than-written),
[section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph) and
[section 15](roadmap.md#15-the-conformance-oracle); delivery
[section 21](roadmap.delivery.md#21-milestones), WA-0, WA-1 and WA-4; gates
[section 23](roadmap.gates.md#23-test-and-evidence-matrix),
[section 24](roadmap.gates.md#24-release-gates) and
[section 25](roadmap.gates.md#25-risks-and-stop-conditions).

**What the plan said.** That the conformance host, the script reader, the corpus store, the binary
encoder, the fuzz host, the soak host and the bench host are **test-only projects**, "never
referenced by a product project and never present in a published closure", with a scan asserting
exactly that and a negative control adding "a product reference".

**What replaced it.** They are **composition roots that are never advertised**, under
`src/compositions/`, and the scan is over every package and every *advertised* composition's
closure rather than over "every published closure".

**Why the earlier reading could not have been built.** The host component's rule A11 is active
today and forbids any project outside `src/compositions/` from referencing a
`Broiler.VM.Profile.*` assembly. Every one of those hosts has to drive **this profile's own**
verifier and executor. The core's fuzz and soak hosts sit under `src/tests/` only because they
drive the *fixture* profile, which A11 exempts by name, and nothing in that exemption reaches a
product profile. So the whole harness plan named a project shape the first build would refuse —
and the correction is not cosmetic, because the scan clause changes with the placement: a harness
root publishes a closure of its own, so an assertion that the script reader appears in "no
published closure" would fail on the very root that must contain it, and would then be relaxed
into meaninglessness by whoever met it.

**What it does not change.** The property that was ever worth having: no shipped image contains a
text-format reader, an encoder, or a suite file. What changed is the boundary that carries it —
from *project kind* to *advertised or not*, which the composition register already records — and
that the negative control now injects **from the execution root**, which is the direction that
would actually ship.

**One consequence worth stating rather than leaving to be discovered.** A corpus this profile
retains is therefore written by a root that publishes, which is the shape a sibling profile records
against its own corpus for an independent reason: a corpus a test project produced would be a
corpus the product path never exercised. The two arguments arrive at the same place.

**Authority and date.** Rule A11's registered statement and its path-keyed allow-list, read against
[WAC-13](#wac-13), which had already recorded the same collision for the API baseline without
generalising it; 2026-09-01.

### WAC-23

**Where:** roadmap [section 9](roadmap.md#9-the-value-store-and-frame-model), the decision table;
delivery [section 21](roadmap.delivery.md#21-milestones), WA-5; gates
[section 23](roadmap.gates.md#23-test-and-evidence-matrix) and
[section 25](roadmap.gates.md#25-risks-and-stop-conditions).

**What the plan said.** That the WA-5 entry-gate decision has eight rows, none of them about
`LiveBytes`. [Section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses)
separately observed that the metering latch "makes the aggregate `LiveBytes` case worse rather than
better", and the ledger's WA-5 row recorded a second decision as open beside the ABI — **whether
an aggregate `LiveBytes` breach may terminate an operation** — with no row, no gate clause and no map
cell anywhere in the plan.

**What replaced it.** A ninth row of the entry-gate decision, and a WA-5 exit-gate clause that asks
for its answer to be exercised by a named case per arm. A decision the ledger calls open and the
plan gives no home to is a decision that gets taken by whoever writes the memory representation,
which is the failure mode a gate on entry exists to prevent — and it is **not separable** from the
representation, since a representation that cannot express the answer is one this row rejects.

**Authority and date.** [The ledger's WA-5 row](roadmap.status.md#2-current-milestone-status), read
against the map's blank-cell rule; 2026-09-01.

### WAC-24

**Where:** delivery [section 21](roadmap.delivery.md#21-milestones), WA-2's and WA-10's exit gates;
delivery [section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map), the chapter-7 row;
gates [section 24](roadmap.gates.md#24-release-gates) gate 1 and
[section 25](roadmap.gates.md#25-risks-and-stop-conditions); ledger section 3.

**What the plan said.** Roadmap
[section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core) states three
obligations following from decoding integers here rather than through the core, and says each "is a
WA-2 gate clause rather than a note". **Only one and a half of them were.** WA-2's gate carried the
count-bound ordering and the scan for calls to the core's canonical readers. It did not carry the
third obligation's two remaining halves: that **the support table says which budget dimension
changes side**, and that **the core's own metering-split record is made conditional**, which
section 7 asserts in the present tense as though the edit had happened.

**What replaced it.** WA-2's gate names the dimension that changes side — `DeclaredCount` leaving
the core-metered row while `SectionCount`, `StructuralDepth` and `ArtifactBytes` stay where they
are — and reads the core's record against that answer, confirming the conditional reading or filing
a correction with its owner and recording the row as open. WA-10's gate and release gate 1 carry the
naming into the support table. Gates [section 25](roadmap.gates.md#25-risks-and-stop-conditions)
gains the risk, and the ledger's unopened-dependency table gains the row.

**Why the second half needed a home rather than a sentence.** The core's record obliges every
profile to route declared counts through the binary package. This profile establishes that a format
admitting padded encodings cannot, which makes the record false about the first product profile that
would meet it — and **a component that falsifies another component's record and reports only its own
side has fixed its support table and left the defect standing**. Nothing in this plan owned that
until now: it was neither an amendment, since no contract changes, nor a local decision, since the
record is not this profile's. It is a correction this profile owes the core, and an owed correction
with no gate behind it is one nobody files.

**What it does not change.** The integer decision, the local resolution, and the scan are untouched,
and no milestone moved. The plan's own claim that all three obligations were gate clauses is what
was false; two of them now are.

**Authority and date.** Roadmap
[section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core)'s third
obligation, read against WA-2's and WA-10's exit gates and the core's metering-split record;
2026-09-01.

### WAC-25

**Where:** delivery [section 21](roadmap.delivery.md#21-milestones), WA-0's and WA-1's exit gates;
delivery [section 27](roadmap.delivery.md#27-the-chapter-milestone-and-gate-map); the
[ledger](roadmap.status.md#2-current-milestone-status)'s WA-0 row.

**What the plan said.** That WA-0's gate is met when, among other things, "the harness roots are
composition roots under `src/compositions/` ... and the graph proves it rather than the prose
asserting it"; when "a two-profile catalog test composes this descriptor beside a second profile";
and when "a descriptor whose guest-load defaults are unconstrained is refused by the catalog, by a
named negative case" — beside a scope line saying WA-0 lands "no product code ... no descriptor,
no project that references a core package".

**What replaced it.** The three clauses are carried to WA-1, and WA-1's gate closes them. Each
needs something WA-0's own scope forbids. A harness root is a composition root, and rule A12
admits no root that references no core package and composes no profile, so no harness root can
stand in a shell graph and nothing in that graph can prove where one sits. The two-profile test
composes a descriptor WA-1 builds. And the unconstrained-default case has to build a descriptor
and drive it through a catalog, which rule A11 leaves nowhere but a root. What WA-0 keeps is what
it can do: settle and record the placement, and prove that the rule which would refuse a
harness-shaped test project bites, by a negative control over one.

**How it arose, which is the part worth keeping.** Two of the three clauses were written on
2026-09-01, with [WAC-10](#wac-10) and [WAC-22](#wac-22), into the gate of the milestone that
*owns the decision* rather than the one that can first *run the test* — so the map's rule that a
requirement is not in the programme until an exit gate would fail without it was satisfied in
letter and failed in effect, because a clause can sit in an exit gate that cannot fail on it. The
sibling profile's first milestone met the same catalog test the same way and carried it to its
second, in words its gate still carries.

**What it does not change.** The placement finding, the finite defaults, the two-profile test
itself, and the negative control that writes a zero into a guest-load default. Only the milestone
that can first run each of them moved.

**Authority and date.** WA-0's own scope line, read against its gate; rules A11 and A12 as
registered; 2026-09-01.

### WAC-26

**Where:** roadmap [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight),
the refusable-retention row; the [ledger](roadmap.status.md#3-open-external-dependencies)'s
section 3.

**What the plan said.** [WAC-03](#wac-03) recorded that the other intended profile's dated grading
"records no position on" the refusable retention member, so the core procedure's counterweight
question was, for this row, unanswered — and the row in section 20 carried no position while the
argument-channel row beside it carried one.

**What replaced it.** The other intended profile has recorded its position in its dated grading,
by an addendum of 2026-09-01: **unaffected**. It has no construct that needs a guest-observable
budget refusal and treats that as a property to preserve rather than a coincidence, so it neither
files the row nor obstructs it. That is the second of the three answers the core's procedure
admits — could use it, unaffected, refuses it — and it means this row, the one row this profile
opens rather than holds and so the one most likely to be filed first, can be filed with its
counterweight field complete rather than blank.

**What it does not change.** The grade — strong, blocking, and the counterweight test passing —
and the blocker, which is the procedure's unexecutability and nothing the other profile holds.
WAC-03's sentence was true when written and is overtaken rather than edited.

**Authority and date.** The other intended profile's dated grading, addendum of 2026-09-01, read
against ADR 0003's counterweight row, which asks whether the other intended profile could use a
capability, is unaffected, or records a refusal; 2026-09-01.

### WAC-27

**Where:** roadmap
[section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core), the
resolution and the first of the three obligations that follow it; the
[ledger](roadmap.status.md#2-current-milestone-status)'s record of the finding as observed
repository state.

**What the plan said.** That declining the core's guarded count reader left this profile with one
property to re-derive — the bound-before-use ordering — and one budget dimension to move.
On that reading the core's bounded-read status set still described this decoder's failures, and the
count-ceiling refusal was an answer the core produced and this profile translated: the status
existed, the profile mapped it onto `ResourceExhaustion` naming `DeclaredCount` and one scope, and
the re-derivation was a matter of not losing an ordering the core had already got right.

**What replaced it.** The core raises `DeclaredCountExceeded` on exactly one path, and that path is
inside the member this decoder does not call. **A decoder that declines the guarded count reader
can never observe that status**, so nothing is being translated: the count-ceiling refusal is
manufactured by this profile's own reader and exists only if this profile writes it. Section 7 now
carries the layer concretely rather than as a principle — six readers over `TryReadByte`, the byte
budget derived from the width, the terminal byte's unused bits distinguished from an over-long
encoding, padding accepted inside the budget, and one bounded count reader that decodes, compares
against the effective ceiling, and only then returns a value at all.

**Why this is more than a mapping detail.** [WAC-04](#wac-04) fixed that an over-ceiling vector
length is `ResourceExhaustion` and not `InvalidArtifact`, and every corpus entry pins its observed
⟨outcome, reason, diagnostic code⟩ triple and replays it across three publish modes. **A refusal
that nothing raises produces no triple at all** — the failure is not a wrong answer recorded and
frozen, it is an answer that never arrives — and the decoder that omitted the comparison does not
report a smaller number, it loops or sizes a buffer from the count. So the first obligation is not
a tidy restatement of what a core member already provided. It is the only thing standing between an
attacker-chosen count and an allocation.

**What it does not change.** The resolution itself, the three obligations, WA-2's scan for calls to
the canonical readers, and the dimension that migrates. No milestone moved and no state changed.

**Authority and date.** The core's bounded reader, read against section 7's resolution:
`DeclaredCountExceeded` is raised on one path and that path is inside the member this profile
declines to call; 2026-09-07.

### WAC-28

**Where:** roadmap
[section 13](roadmap.md#13-memories-tables-globals-and-the-host-boundary), the memories
subsection; the [ledger](roadmap.status.md#2-current-milestone-status)'s section 2 and its
[section 3](roadmap.status.md#3-open-external-dependencies).

**What the plan said.** That WA-5 cannot choose a memory representation until the refusable
retention member is answered, and that an unanswered row makes WA-5 `Blocked` rather than merely
late at the moment WA-3 would otherwise let it start ([WAC-16](#wac-16)). On that reading the
amendment was a precondition of the *work* and not only of the claim, and the only alternative the
plan named was a fallback that WA-10 publishes.

**What replaced it.** The MVP programme defers exactly the co-signing step the amendment waits on,
so the row does not close while the MVP runs, and a plan that waits for it delivers no interpreter
at all. **A deferred decision is a decision nobody took, not a decision that went a particular
way**, so the plan may neither wait for it nor assume it. What section 13 now records is the route
taken in its absence, named as taken without a decision: growth is refused against **this profile's
own declared memory maximum**, which is a profile limit rather than a core budget refusal, so on
that arm the specification's answer is produced exactly — the module observes `-1`, the operation
completes normally, and no core allowance is spent. On the arm where a **core** budget would
refuse, no such answer is available on the shipped contract, the step becomes `ResourceExhaustion`,
and that is a deviation from what the specification says `memory.grow` answers.

**What it does not change, and this is the load-bearing half.** [WAC-16](#wac-16) stands:
publishing the deviation is WA-10's release decision and never WA-5's. The blocker in the ledger's
section 3 stands, with the same holder and the same unblock condition. Roadmap
[section 12](roadmap.md#12-traps-exhaustion-and-why-neither-is-a-process-failure)'s requirement is
not weakened — the deviation is measured against it rather than replacing it — and an amendment
minted later closes the deviation rather than ratifying it. **What changed is only that WA-5 is not
prevented from starting**, because a route exists that does not need the amendment, and the
ledger's own definition of `Blocked` is a dependency that prevents the next action.

**Why it is recorded where its consequence is.** The MVP programme's cost clause requires a route
taken where a deferred decision would have chosen to be named at the point the consequence lands,
and not only in the record that defers it. A reader of section 13 who never opens that record still
has to meet this, because section 13 is where a memory representation gets chosen and the route is
the thing that constrains the choice.

**Authority and date.** The MVP programme record at `docs/mvp.md`, section 5 and its row MVP-1, on
the repository owner's instruction dated 2026-09-07, read against [WAC-03](#wac-03) and
[WAC-16](#wac-16); 2026-09-07.

### WAC-29

**Where:** the [ledger](roadmap.status.md#2-current-milestone-status)'s WA-2 row and the first row
of its [section 3](roadmap.status.md#3-open-external-dependencies); delivery
[section 21](roadmap.delivery.md#21-milestones), WA-2's dependency line as a reading rather than as
a sentence.

**What the plan said.** That the core contract not being accepted "blocks WA-2 onward" — stated
without a qualifier, beside a status vocabulary that defines `Blocked` as a dependency preventing
the next action. The natural reading, and the one a scheduler takes, is that no line of WA-2's
decoder may be written until the core's review record is signed.

**What replaced it.** It blocks WA-2 onward from **acceptance**, and it blocks this component from
**publishing**. It does not block writing the code. Update rule 8 already ruled that human review
gates a release rather than a development step, and the MVP programme defers acceptance on that
same shape. So WA-2's decoder, WA-3's validator and WA-5's interpreter may be written and merged
against the core contract as *implemented*; and while the dependency stands, no row moves to
`Accepted`, no package is published, no runtime identifier is claimed and no support table is
issued.

**What it does not change.** The blocker, its holder and its unblock condition are unchanged, and
the WA-2 row's state is unchanged — nothing here moves a row, and a correction never could. The two
other things standing against WA-2 are untouched, because neither is a review and neither is an
acceptance: **the specification revision is still unretrieved**, and no programme defers a human
action into existence; and the correction this profile owes the core's metering-split record is
still unopened.

**Why the qualifier is worth adding rather than assuming.** The delivery order already says the
acceptance gate binds WA-2 and not WA-0 or WA-1, which invites the reading that WA-2 is where
building stops. Under an MVP that reading would halt the programme at its second milestone while
the record described the work as merely late — and **a component idle on a dependency it was never
required to wait for looks exactly like a component making progress it has not made.**

**Authority and date.** The ledger's own update rule 8, read against the MVP programme record at
`docs/mvp.md` and the repository owner's instruction dated 2026-09-07; 2026-09-07.

### WAC-30

**Where:** roadmap [section 1](roadmap.md#1-terminology-and-support-claims), the first non-goal and
the dated paragraph after the non-goals;
[section 4.3](roadmap.md#43-what-is-acquired-rather-than-written), the binary encoder's row; gates
[section 25](roadmap.gates.md#25-risks-and-stop-conditions), the ingestion-path row.

**What the plan said.** That this profile has no compiler: it consumes what external toolchains
produce, and there is "no Broiler WebAssembly compiler, no lowering assembly, and no compiler
sibling". The binary encoder's absence from the product was what kept "no compiler" true, and the
ingestion-path stop condition named the claim it protected as "no text format, no compiler".

**What replaced it.** The profile carries a **translator**. `WasmTranslator` decodes and validates a
module with this profile's own decoder and validator and lowers it into the universal bytecode, inside
the profile assembly, and every composition root that runs a module calls it. That is a lowering from
one bytecode into another. It is not a compiler of any source language, and not a compiler to native
code: a native form for this profile is the programme's milestone UBC-6b, and nothing of it is in this
tree. **What survives is the half the non-goal was for.** This profile still produces no WebAssembly,
still consumes what external toolchains produce, and still has no lowering assembly and no compiler
sibling, because the translator lives in the one product assembly. The binary encoder's absence from
the product now keeps the product from *producing* WebAssembly, and that is the claim the
ingestion-path row protects.

**What it does not change.** The text-format non-goal; the encoder's placement in a harness root that
is never advertised; and the refusal of a format assembly, whose reason [WAC-33](#wac-33) restates.

**Authority and date.** The universal bytecode programme roadmap (`docs/universal-bytecode.roadmap.md`),
milestone UBC-4, work package UBC-4.2; route MVP-15 in `docs/mvp.md`, the module as source, taken at
that milestone; [ADR 0013](../../../docs/adr/0013-the-universal-bytecode-extraction-record.md), which
admits the universal bytecode through the core's extraction gate with this profile as one of its two
consumers; 2026-09-25.

### WAC-31

**Where:** roadmap's opening statement of what the profile is, and its reading of *the executor*
and *the interpreter*; [section 1](roadmap.md#1-terminology-and-support-claims), the third non-goal
and the scope list's closing sentence;
[section 4.3](roadmap.md#43-what-is-acquired-rather-than-written), the row of what is written here;
the opening of the [delivery file](roadmap.delivery.md), whose reading every milestone below it
takes, and delivery [section 22](roadmap.delivery.md#22-delivery-order)'s ownership table; the
opening of the gates file, and gates [section 24](roadmap.gates.md#24-release-gates), gate 12.

**What the plan said.** "This profile has one interpreter." It emits no IL, builds no expression tree,
compiles no delegate, and has no tiering path into dynamic code, and a product closure containing an
IL emitter is a release blocker. The opening listed one interpreter among what the profile is, section
4.3 put the interpreter among what is written here, and release gate 12 described the profile as
shipping "a decoder, a validator, and an interpreter".

**What replaced it.** **This profile has no interpreter of its own.** Its modules run on the universal
bytecode's bytecode emitter, `Broiler.VM.Emitter.Bytecode` — one dispatch loop, generic over a family
struct and shared with every family a composition composes — over this profile's family: its
instruction table, its handlers and its store. The profile's interpreter and its value slot were
deleted once the roots translated first. The rest of the non-goal stands with its subject moved:
nothing in this profile emits IL, builds an expression tree or compiles a delegate; no native form
executes this profile's modules, and the one the programme names is its milestone UBC-6b, not taken;
and a product closure containing an IL emitter is still a release blocker. Wherever the plan says *the
interpreter* or *the executor*, it now reads *this profile's family executed by the bytecode emitter*.

**Why this is not the second arm the non-goal refused.** The non-goal refused a second tier beside the
first. The emitter replaced the profile's arm rather than standing beside it, so there is still one —
and it is a loop the profile shares rather than one it owns, which is the change.

**What it does not change.** The refusal of a JIT, of tiering into dynamic code and of any promotion
path; the release-blocker clause; and the rejection in [section 3](#3-rejections), which gains a dated
note rather than an edit.

**Authority and date.** The programme roadmap, milestone UBC-4, work packages UBC-4.5 and UBC-4.8 and
its "does not do"; route MVP-14 in `docs/mvp.md`, one dispatch loop generic over a family struct;
ADR 0013; 2026-09-25.

### WAC-32

**Where:** roadmap's opening reading of *the verifier* and *verification*;
[section 1](roadmap.md#1-terminology-and-support-claims), the terms *Decoding* and *Validation*, the
scope list and the second non-goal; [section 2](roadmap.md#2-engineering-invariants), invariant 4;
[section 8](roadmap.md#8-validation), its opening and the diagnostic registry's carriers;
[section 10](roadmap.md#10-execution-mapping-webassembly-onto-the-core-lifecycle), the stage table's
verification row; the opening of the delivery and gates files, which read the milestones' and gates'
*verifier* the same way.

**What the plan said.** Decoding and validation are the first and second halves of the core's one
verification, "never a separate public step", reached through the one verification entry point the
core provides. A verification rejection travels as the core's own pair — this profile's code and a
position record — on the verifier outcome, and a trap travels in the payload: two carriers.

**What replaced it.** They are the two phases of **one translation**, which a composition root runs
before the core is asked anything. The core then verifies what the translation wrote — the universal
bytecode artifact — with the universal bytecode's walk and this profile's family hook,
`WasmFamilyVerifier`, inside its one verification entry point. The phase order is unchanged and still
observable: decoding completes before validation begins, and a module that is both malformed and
invalid is still answered malformed. Invariant 3 holds with its word moved: a structural or type check
happens at translation or at the core's verification of the artifact, and never at execution. **The
registry has three carriers now**: the translation's answer, which carries the decoder's, the
validator's and the translation's own codes in the fields the core's verifier outcome has; the core's
verifier outcome, which carries the family hook's; and the typed payload, which carries a trap's.

**The second non-goal survives, and the plan says why.** Whatever validates a module is still this
profile's validator, now reached through the translator. What the walk and the hook check is the
artifact, which is a different input and one a root need not have produced; where the hook repeats a
check the validator makes, it answers with the validator's code. It is a check of the artifact, not a
second validator of the module.

**What it does not change.** Totality: the translator and the hook each answer and never throw, and a
defect in either answers with the reserved code. The eager reading, and the refusal of lazy
validation. What a caller reads back for a malformed or invalid module, which [WAC-34](#wac-34)
states.

**Authority and date.** The programme roadmap, milestone UBC-4, work packages UBC-4.2 and UBC-4.5;
route MVP-15 in `docs/mvp.md`; this profile's diagnostic registry, `WebAssemblyDiagnosticCode`, whose
codes from 2851 to 2860 are the family hook's and from 2871 to 2874 the translation's; 2026-09-25.

### WAC-33

**Where:** roadmap's opening statement of what the profile references;
[section 4.3](roadmap.md#43-what-is-acquired-rather-than-written), the row for the core's assemblies;
[section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph) — the pivot paragraph, the
table's profile and root rows, the dependency graph, the reference-set rule and the paragraph on the
projects the MVP creates; gates [section 24](roadmap.gates.md#24-release-gates), gate 2.

**What the plan said.** The profile's Broiler.VM reference set is exactly the two core assemblies,
Abstractions and Binary, and nothing else Broiler-owned. The pivot argument "does not transfer": this
profile has no compiler, its only decoder is inside the verifier, and there is nothing for a format
assembly to hold apart — so the default is one product assembly, and no format sibling is created. The
profile row carried a descriptor, a verified module, a store and instance model and an interpreter, and
the roots referenced the runtime and the profile.

**What replaced it.** The reference set is the two core assemblies **and `Broiler.VM.Ubc`**, the
universal bytecode's format, family-table schema and verifier, and rule W1 asserts exactly those
three. The profile references it to declare its family, and references no emitter. **Still one product
assembly, and still no format sibling — for a different reason.** A pivot exists now, and it is
shared: the format a translator and an emitter agree on without depending on each other is
`Broiler.VM.Ubc`, which every family is written against and which ADR 0013 admits beside the core. A
format assembly of this profile's own would hold apart nothing that one does not already. The profile
row carries the decoder, the validator, the translator, the family — its table, verifier hook,
handlers and store — the payload projections and, when it is written, the linker; it builds no
descriptor. Each composition root that runs a module references `Broiler.VM.Ubc` and
`Broiler.VM.Emitter.Bytecode` as sibling assemblies and builds the family's descriptor over the
bytecode emitter with `UbcDescriptors.Build`. The MVP creates the same projects it did.

**What it does not change.** No edge to another profile family in either direction; no aggregate
profile-listing type; the rule that a second product assembly is a dated decision with a measured
closure difference; the harness's placement in composition roots.

**Authority and date.** ADR 0013; the programme roadmap, milestone UBC-4, work package UBC-4.8 and exit
gate 1; rule W1 as revised at that milestone; route MVP-10 in `docs/mvp.md`, under which ADR 0011's
promise P1 reads `Broiler.VM.Ubc` as an edge an extraction verdict names; ADR 0001's dated revision of
its graph budget for the milestone's edges; 2026-09-25.

### WAC-34

**Where:** roadmap [section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core),
the subsection on the artifact and the meaning of the format version;
[section 1](roadmap.md#1-terminology-and-support-claims), the term *The format version* and the
scope list's first item;
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses), the structural
gift and the budget matrix; [section 10](roadmap.md#10-execution-mapping-webassembly-onto-the-core-lifecycle),
the failure-phase table's first two rows;
[section 11](roadmap.md#11-the-store-instances-and-linking), reading B;
[section 16](roadmap.md#16-deployment-compositions-native-aot-and-the-browser-embedding), the browser
subsection; [section 18](roadmap.md#18-persistence-and-the-code-cache), the cache key; the opening of
the delivery and gates files, which read *format version 1 as a bare module* this way.

**What the plan said.** Format version 1 admits a bare WebAssembly binary module as the entire payload —
no Broiler framing and no re-encoding — so any toolchain's module is verified verbatim, a browser hands
the fetched bytes to verification, and the malformed corpus tests the specification's own format rather
than a Broiler framing layer. The format version is this profile's own integer and means the payload's
shape: version 1 for a bare module, and version 2 the candidate container of section 11's reading B.

**What replaced it.** **The module is source; the universal bytecode is the artifact.** A root hands
the module to the translator, and the core verifies what the translator wrote, under the universal
bytecode's format version — which every family shares and this profile does not own. Nothing about the
module is wrapped or re-encoded before the translator reads it, and its own magic and version are still
checked by this profile's decoder as the specification requires. **The malformed corpus is still about
the specification's format**: a module the decoder or the validator refuses is refused by the
translator before the core is asked, in the fields the core's verification answered — outcome, reason,
diagnostic code, dimension, scope and position — so a retained entry reproduces the answer it recorded.
A browser hands the fetched bytes to the translator. Reading B's container would be a second source
shape the translator reads rather than a second format version, because this profile has no format
version of its own to move. And the core's content hash now identifies the translation rather than the
module, so a cache key names the translator's identity and version beside the module's bytes.

**What it costs, recorded where the cost lands.** Translation runs in the root, outside the core, under
the effective ceilings the root reads and against a meter that follows the core's rules. Its work
reaches no runtime-level account, no verification wall clock and no one-verification slot, and an
artifact larger than its module can exceed an artifact-bytes ceiling the module is within. Section 3's
budget matrix states it, and the programme records it as a departure in its concept's Appendix G.

**What it does not change.** The integer decision, the section-order table, the malformed list and its
outcome split ([WAC-04](#wac-04)), the three corpus disciplines, and that the format version does not
track the specification's version.

**Authority and date.** Route MVP-15 in `docs/mvp.md`, the module as source, taken at the programme's
milestone UBC-4, and its commentary on the cost; the programme roadmap, work packages UBC-4.2 and
UBC-4.7; bundle `ubc-4-001`'s base run and bundle `ubc-4-002`'s run after it, under the decision rule
retained in the first, which compare each retained corpus entry's answer before and after the module
became source; 2026-09-25.

### WAC-35

**Where:** roadmap [section 9](roadmap.md#9-the-value-store-and-frame-model), its opening;
[section 1](roadmap.md#1-terminology-and-support-claims), the scope list's value model;
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses), the budget
matrix's `Fuel` row;
[section 12](roadmap.md#12-traps-exhaustion-and-why-neither-is-a-process-failure), how a trap leaves
execution; [section 14](roadmap.md#14-suspension-threads-and-what-this-profile-does-not-declare), the
frame bullet; delivery [section 21](roadmap.delivery.md#21-milestones), WA-5's next action and
dependencies; delivery [section 22](roadmap.delivery.md#22-delivery-order), the decisions that need no
code and the MVP path's ownership table; gates
[section 25](roadmap.gates.md#25-risks-and-stop-conditions), the value-and-frame row.

**What the plan said.** The nine-row value, store and frame decision is taken before the interpreter
is written, as a numbered decision and a gate on entry to WA-5; no interpreter source lands while it is
open; fuel is charged for every instruction dispatched; a frame model that lives on the CLR stack
cannot later be moved without rewriting the interpreter; and whether the interpreter implements trap
unwinding with a CLR exception is a WA-6 decision with a measurement attached.

**What replaced it.** The nine rows are answered in
[WAD-0003](decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md), in this
profile's decision series, which states where each stands now that the universal bytecode executes
this profile's modules: the numeric, reference and rooting rows carry over, and the `LiveBytes`-breach
row is answered by a retention charged before the allocation *(corrected 2026-09-26, later the same
day: this read "the numeric, reference, rooting and `LiveBytes`-breach rows carry over", which
WAD-0003's correction of that date contradicts; see [WAC-43](#wac-43))*; the
call convention, frames and labels, trap propagation and metering are the universal bytecode's; and
the vector row is reversed — a word is eight bytes, and a `v128` slot is a question for a universal
bytecode format version 2. **The entry gate those rows guarded is retired with the interpreter.** The
interpreter was written under routes rather than a decision and was deleted at UBC-4, so there is no
interpreter source for the decision to precede. Frames are the universal bytecode's heap frames, owned
by the operation. Fuel is charged per universal bytecode row and per frame's declared locals, so a
structural instruction the translator lowers away charges nothing. And trap unwinding is not a WA-6
decision: a trap is a status the universal bytecode's loop turns into this profile's payload at its
boundary, never a CLR exception. What WA-5's exit gate asks of the decision — fixtures and Native AOT representation probes retained beside each
row, and the `LiveBytes`-breach row exercised per arm — it still asks.

**What it does not change.** That the rows are answered in both directions; that the vector-width row
is the one whose late answer is costly, which is now a cost to every family and not to this one alone;
and the `CallDepth` default, still derived from a measured frame cost, now the emitter's.

**Authority and date.** WAD-0003, taken 2026-09-25 by the WebAssembly profile owner in the act that
deleted the interpreter; the programme roadmap, milestone UBC-4, work package UBC-4.8 and its "does not
do" on the vector width; 2026-09-25.

### WAC-36

**Where:** roadmap's opening reading of *the descriptor* and *the verified state*;
[section 1](roadmap.md#1-terminology-and-support-claims), the term *This profile*;
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses), the seven types
and the descriptor; [section 4.3](roadmap.md#43-what-is-acquired-rather-than-written), the row of what
is written here; [section 10](roadmap.md#10-execution-mapping-webassembly-onto-the-core-lifecycle),
the stage table; [section 14](roadmap.md#14-suspension-threads-and-what-this-profile-does-not-declare),
its first subsection; the opening of the delivery and gates files, and delivery
[section 22](roadmap.delivery.md#22-delivery-order)'s ownership table.

**What the plan said.** This profile implements seven core-facing types — a verifier, an executor, the
verified state, the instance state, the continuation, the payloads and the allocation-meter adapter —
and supplies one full-arity descriptor through one static accessor. Section 10 mapped each core stage
onto them: verification decodes and validates into this profile's immutable verified state;
instantiation links and allocates in this profile's executor; invocation, resume and unwind are that
executor's members; and `IVmProfileContinuation` is implemented so that a resume is refused correctly.

**What replaced it.** **Every core stage is answered by the universal bytecode with this profile's
family inside it.** At catalog build the profile supplies its family's registration and declaration,
and each composition root builds the descriptor from them over the bytecode emitter; the declaration
carries every row the deleted descriptor carried but its revision. Verification is the translation in
the root ([WAC-32](#wac-32)) and then the core's walk and family hook, producing the universal
bytecode's verified program. At instantiation the emitter instantiates through the family: its
instance state — the store, with a memory, a table and globals — is allocated, the global
initialisers are evaluated, the element and data segments are applied in order, and the start
function is run; **a start-function trap still publishes no instance** and is still a profile fault carrying this profile's trap payload. At
invocation an exported function runs on the emitter over the family's handlers and is answered through
this profile's payloads. Resume and unwind are the emitter's: nothing here parks, so no suspension
exists to resume, and this profile implements no continuation. **So of the seven core-facing types this
profile implements two** — the payloads and the allocation-meter adapter — and implements the universal
bytecode's family contract, `IUbcFamily` and `IUbcFamilyVerifier`, in place of the other five.

**What it does not change.** The four failure phases and where each lands; that a trap is a payload
and never a core category; route MVP-6's entry-point encoding, which the family's entry binding reads;
that nothing parks at any allocated manifest, and the declarations section 14 makes, which the family's
declaration now carries.

**Authority and date.** The programme roadmap, milestone UBC-4, work packages UBC-4.5 and UBC-4.8; the
universal bytecode's family contract in `Broiler.VM.Ubc` and its descriptor factory
`UbcDescriptors.Build`; 2026-09-25.

### WAC-37

**Where:** roadmap [section 11](roadmap.md#11-the-store-instances-and-linking), reading C in the table
of readings; [section 5](#5-open-and-therefore-not-corrected) of this file, the store question.

**What the plan said.** Reading C — one runtime, one store — keeps the store executor-scoped: this
profile's executor is created once per runtime and holds the store, and each instance state is one
module instance's handle into it. Its crux was the naming channel.

**What replaced it.** This profile has no executor to hold a runtime-scoped store. The executor is the
bytecode emitter, shared by every family, and the universal bytecode's family contract gives a family
state per instance, through `IUbcFamily.CreateInstance`, and nothing scoped to a runtime. **So reading
C now needs something outside this profile** — a runtime-scoped member of the universal bytecode's
family contract, which is not this profile's to add — as well as its naming channel. That is an input
WA-6's decision states, not a decision this entry takes: reading A is still rejected, B and C are still
the candidates, and WA-6 still chooses between them before the linker is written.

**Authority and date.** The universal bytecode's family contract, `UbcFamilyContracts.cs` in
`Broiler.VM.Ubc`, read against section 11's reading C; the programme roadmap, milestone UBC-4, work
package UBC-4.8; 2026-09-25.

### WAC-38

**Where:** roadmap [section 13](roadmap.md#13-memories-tables-globals-and-the-host-boundary), the
representation bullet; [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses)'s
caution paragraph and [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight)'s
refusable-retention row, each where it says WA-5 cannot choose a memory representation until the
amendment is answered; [section 9](roadmap.md#9-the-value-store-and-frame-model)'s opening;
[section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly)'s second
consequence and its commitment not to foreclose the boundary; delivery
[section 21](roadmap.delivery.md#21-milestones), WA-5's next action and its exit-gate clause on the
representation.

**What the plan said.** Whether a memory is a managed array, a pinned buffer or a reserved range with
guard pages is a WA-5 decision with Native AOT and per-RID consequences, and WA-5 cannot choose it until
the refusable retention member is answered. [WAC-28](#wac-28) had already let WA-5 start on a route,
and left those two sentences standing.

**What replaced it.** **The representation is chosen, by
[WAD-0001](decisions/0001-the-memory-representation.md), at the programme's milestone UBC-4, as its
decision UBC-D-2**: a pinned managed byte array, reallocated on a successful growth with its base
republished; a growth invalidates every view; and the base is stable between growths rather than for
the instance's life. It was chosen without the amendment, and keeps what the route keeps: a growth past
this profile's own page ceiling answers minus one before any charge, and a growth a core budget refuses
stays the published deviation. It is not WA-5's act and moves no row. What WA-5's exit gate asks of it
that the record does not say — its limits per runtime identifier, and that it does not foreclose
section 17's boundary — the gate still asks.

**What it does not change.** The amendment's row, its holder and its unblock condition;
[WAC-16](#wac-16)'s release decision; section 13's invalidation rule, which the record adopts.

**Authority and date.** WAD-0001, taken 2026-09-25 by the WebAssembly profile owner with the security
owner, both roles held by one person; the programme roadmap's decision UBC-D-2 and work package
UBC-4.4; 2026-09-25.

### WAC-39

**Where:** roadmap [section 16](roadmap.md#16-deployment-compositions-native-aot-and-the-browser-embedding),
its opening, the label table and the browser subsection;
[section 1](roadmap.md#1-terminology-and-support-claims), the term *Deployment composition*.

**What the plan said.** This profile mints exactly one composition label, `execution-only`, because a
label describes when source is compiled and this profile compiles nothing; and the browser composition
is that composition unmodified, with no lowering in the image.

**What replaced it.** **Every image that runs a module carries a lowering** — the translator — so the
ground for `execution-only` is gone, and the execution root's catalog prints no label and says that it
carries a lowering. Whether a translating image claims a label, and which, is the composition
register's to decide and not this plan's; the plan claims none. What survives of the browser argument:
there is still no text format on the path and no compiler of any source language, the bytes a browser
fetches are what the translator reads, and the closure a page exercises is still the closure a
module-running host exercises — with the universal bytecode's two assemblies in it.

**What it does not change.** That a harness root claims no label; that a further product assembly
produces further closures and not further composition kinds; the Native AOT gate.

**Authority and date.** The execution root's catalog as milestone UBC-4 left it, which prints `label
none` and `carries-lowering yes`; `docs/compositions.md`, as corrected at that milestone; route MVP-15
in `docs/mvp.md`; 2026-09-25.

### WAC-40

**Where:** roadmap [section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly),
"What the boundary actually is";
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses), the first
bullet of what the core refuses to do.

**What the plan said.** "Share mechanism, never share semantics": a shared value representation, frame
layout, or opcode set is named in the core's own table as something it exists not to own, so there is
no cross-profile value channel and none is coming. Section 3 added that the core stores no values,
inspects no frames and knows no opcode, and that there is "no shared value ABI to reach for and none is
coming".

**What replaced it.** The opcode-set half of that refusal was reached by an extraction verdict. ADR
0013 admits the universal bytecode — an opcode set, a word plane and a frame model, shared by every
family and owned beside the core — through the core's extraction gate, and route MVP-12 records that it
is reached as an encoding whose language instructions keep the meanings their profiles give them. This
profile executes on it, and its call convention, frames and trap propagation are the universal
bytecode's ([WAD-0003](decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)).
**What stands is the half section 17 is about.** No language value crosses from one profile to another
through anything shared: a universal bytecode word carries no language meaning, a family's own values
stay on its own plane, and the value-representation and frame-layout halves of the core's refusal stand
as MVP-12 records. So there is still no cross-profile value channel and none is coming, the embedder's
seam stays the route, and it keeps its price. The core's runtime itself still stores no value, inspects
no frame and knows no opcode.

**Authority and date.** ADR 0013; route MVP-12 in `docs/mvp.md`; ADR 0011's standing-refusals table,
read through that route; 2026-09-25.

### WAC-41

**Where:** roadmap [section 18](roadmap.md#18-persistence-and-the-code-cache), the neighbouring question
already answered; [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight),
the row for an in-process producer input form and the procedural paragraph after the table;
[section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses), the bullet on
the one verification input form.

**What the plan said.** "There is no in-process producer": every byte arrives from outside the trust
boundary, so the byte round trip costs nothing, serialization is this profile's input rather than a
cost on its critical path, and the input form is not needed and this profile would not co-sign it.

**What replaced it.** **The translator is an in-process producer.** Every module still arrives from
outside, but what the core verifies is written in-process by the translator and read back by the core
through the byte round trip, once per translation — and a root translates a module on every run,
because no host here reads a translation from a file. The round trip is now a cost this profile pays,
and the ground of the refusal is gone. The plan now says the input form is **not asked for**, because
no measurement shows the round trip on a critical path — WA-10's cold-start measurement, which now
includes translation, is what would reopen it — and it **no longer says this profile would not co-sign
it**, because the reason it gave was that there was nothing in-process to bypass serialization for.

**What it does not change.** The contract: at core contract version 1 the round trip is mandatory, and
this profile takes it. The other rows of what this profile does not need. The rule that a refusal in
that table is recorded rather than blocking.

**Authority and date.** Route MVP-15 in `docs/mvp.md`; the programme roadmap, milestone UBC-4, work
packages UBC-4.2 and UBC-4.8; the polyglot command-line root's refusal to compile a module to a file,
whose reason since that milestone is that a module is translated again on every run; 2026-09-25.

### WAC-42

**Where:** roadmap header, its statement of what the plan says about the present, and the placement
paragraph after it; "How this roadmap is split", the sentence naming that statement as not
transcription; [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph), the last
sentence of the paragraph on the projects the MVP creates.

**What the plan said.** "No line of this profile has been written: no source tree, no descriptor, no
decoder, no pinned specification revision, no pinned suite revision, and no evidence bundle",
defended in the split rules as "this file's opening statement that nothing has been built, which no
milestone can change without changing the ledger in the same breath"; the placement paragraph's rule
group "which no register holds today"; and section 5's "None of the three exists today".

**What replaced it.** Only what is still true and still cannot change without the ledger: **no
milestone of this profile is accepted, no specification or suite revision is pinned, and no evidence
bundle of this profile's own is retained.** What exists in the tree is the ledger's to record, and the
plan transcribes none of it.

**Why it stood uncorrected.** It was true when written and stopped being true on 2026-09-07, when this
profile acquired a source tree and the ledger recorded milestone-owned code. The ledger moved that
day; the plan's header, which says of itself that it changes only with the ledger, did not. That is
the failure the sentence names, committed by the sentence: a present-tense claim no milestone could
change without the ledger was changed by milestones, and the plan was not edited in the same breath.

**Authority and date.** [The ledger's section 2](roadmap.status.md#2-current-milestone-status), whose
rows have recorded milestone-owned code since 2026-09-07, and its headline of that date; 2026-09-26.

### WAC-43

**Where:** roadmap [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses),
the budget matrix's `LiveBytes` row and the caution paragraph's sentence on the aggregate `LiveBytes`
case; [section 10](roadmap.md#10-execution-mapping-webassembly-onto-the-core-lifecycle), the stage
table's `Unwind` row; [section 13](roadmap.md#13-memories-tables-globals-and-the-host-boundary), the
first property of a memory; [section 1](roadmap.md#1-terminology-and-support-claims), the paragraph
after the non-goals that lists the entries milestone UBC-4 filed; the
[ledger](roadmap.status.md)'s WA-5 row and its paragraph on the programme's bundles.

**What the plan said.** The `LiveBytes` row: "Reported on growth, released on instance disposal." The
caution paragraph: "The same latch makes the aggregate `LiveBytes` case worse rather than better, since
the guest has already observed a *successful* grow before the operation aborts — which is precisely the
outcome section 12 forbids." Section 13: "A memory is reported, grown, and released through the meter.
Allocation on instantiation, growth on `memory.grow`, release on store disposal. A memory that is
allocated without being reported is a ceiling that does not exist." And section 10's `Unwind` row and
the ledger's WA-5 row, as [WAC-36](#wac-36) and the programme's milestone left them: retention "is
released by the family on every path that publishes no instance", and the store "releases what it
retained on every path that publishes no instance". The ledger's correction of its section 2 route
paragraph: "The module still never runs the instruction after the growth". And the two lists of the
milestone's entries, as they stood: section 1's "with the milestone's other consequences for this plan
in [WAC-32], [WAC-33] and [WAC-35] to [WAC-41]", and the ledger's "The plan's gates are now read
through roadmap corrections [WAC-30] to [WAC-41], which narrow no clause".

**What replaced it.** The family's instance store charges each memory's and each table's `LiveBytes`
retention **before it allocates**, after the `AllocatedBytes` charge, with a charge that can be refused,
and each growth's the same way. A refused retention refuses the instantiation or ends the step at the
growth, so a growth whose retention is refused is never observed as a success — and the guest observes
no refusal either, because a refusal by a core budget is still not guest-observable. A growth whose
fuel or allocation charge a core budget refuses ends the step at the growth as well, where the base
answered minus one and its interpreter ran on until the core rewrote the completed step, so the module
no longer runs the instruction after it. What the store
retained is released through the family on every failure the bytecode emitter answers — a refused
charge, a trap in a segment or in the start function, an exception — and by the core when a published
instance is disposed. **It is not released on every path that publishes no instance**: when a refusal
or a cancellation latched during an instantiation the emitter answered as complete, the core answers an
exhaustion or a cancellation, publishes no instance and releases nothing. That is a defect of the core,
not of this profile, written out as
[a task](../../../docs/tasks/release-dropped-instantiation-retention.md). The paragraphs that list the
milestone's entries gain this one. *(The replacement sentences in the budget row and in section 13 were
first written, in `8cbbc0c`, as released "through the family when an instantiation fails", and were
qualified the same day, because the core's drop is a failed instantiation too.)*

**What it does not change.** The core's contract: the retention report still returns nothing, a refused
charge still latches, and a refusal by a core budget still has no guest-observable spelling, so
[section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight)'s amendment row stays
open and blocking. Route MVP-1's refusal against this profile's own page ceiling, which answers minus
one before any charge. The amounts: the same bytes are retained for the same memories and tables, and no
test asserts them, before this entry or after it.

**Authority and date.** [WAD-0001](decisions/0001-the-memory-representation.md)'s addition and
[WAD-0003](decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)'s
correction of 2026-09-26; the family's store as changed in `5d171ce`; the check of the programme's
bundle `ubc-4-002`, which found these sentences uncorrected; 2026-09-26.

---

## 3. Rejections

An option considered and refused is not a correction: nothing in the plan changed, and the record
of the refusal is what makes the choice reviewable. **This profile has no decision series yet** —
WA-0 mints the first — so unlike its sibling, every rejection below is the plan's own, and each
stays in the roadmap where a reader meets it. They are indexed here so this file is a complete
answer to "what has this programme refused". *(Noted 2026-09-26: it has one now, and WA-0 did not
mint it. The universal bytecode programme's milestone UBC-4 minted the series on 2026-09-25, at
[`decisions/`](decisions/README.md), numbered `WAD-nnnn`, and each record there carries the options
it rejects in its own text. The rows below are still the plan's own, and a record's rejections are
not copied into this table.)*

| What is refused | Why, in one line | Where |
|---|---|---|
| A second execution arm, a JIT, or any tiering into dynamic code | The core forbids dynamic code in a product closure, and there is no second tier for a promotion to reach *(noted 2026-09-26: since UBC-4 this profile has no execution arm of its own — its modules run on the universal bytecode's shared bytecode emitter — and the refusal of a JIT, of tiering and of promotion stands; see [WAC-31](#wac-31))* | roadmap §1 non-goals |
| A second decoder or validator | Whatever validates a module is this profile's verifier, reached through the core's one verification entry point *(noted 2026-09-26: it is reached through the translator now, and the family hook the core calls checks the translator's artifact rather than the module; the refusal stands; see [WAC-32](#wac-32))* | roadmap §1 non-goals |
| A format assembly of its own | This profile has no compiler, so there is nothing for a pivot to hold apart; creating one would be creating an assembly to shorten a file *(noted 2026-09-26: the reason moved. The profile has a translator now, and the pivot it and an emitter agree on is `Broiler.VM.Ubc`, which is shared and not this profile's; the refusal stands; see [WAC-30](#wac-30) and [WAC-33](#wac-33))* | roadmap §5 |
| The specification's lazy-validation permission | A deferred check is a check reported as a trap, and this profile's invariants make validation total | roadmap §8 |
| The core's canonical variable-length integer reader, for module immediates | The specification requires padded encodings inside a byte budget and production toolchains emit them; the decoder reads its own | roadmap §7 |
| A cross-profile value channel in the core | A shared mutable region is shared semantics by another name, and the price is paid at the embedder's seam where it is visible *(noted 2026-09-26: the core's refusal section 17 restates was reached for its opcode-set half by ADR 0013, and this refusal is untouched by it; see [WAC-40](#wac-40))* | roadmap §17, §20 |
| An in-process producer input form | Every byte this profile runs arrives from outside the trust boundary, so serialization is its input rather than a critical-path cost *(noted 2026-09-26: the ground of this row is gone — the translator is an in-process producer, and the round trip is a cost this profile pays — so the plan no longer refuses the form; it does not ask for it; see [WAC-41](#wac-41))* | roadmap §20 |
| Nested instantiation through the mediator | The language has no instruction that asks for code while running | roadmap §20 |
| Declaring suspension or threads at any allocated manifest | Declaring a capability nothing exercises would make the descriptor claim what the evidence does not show | roadmap §14 |
| A design hostable only by a second core state machine | Exactly one core state machine and one core contract version exist in a product graph at any time | roadmap §20 |

**One refusal is this profile's counterweight duty rather than its own need**, and it is worth
separating: [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight)'s
"what this profile does **not** need" table records capabilities another language profile might
reasonably ask for and this one declines to co-sign. **A counterweight that only ever asks is not
one**, and that table is the duty discharged.

---

## 4. Hazards a reader will meet

Notes addressed to a reader rather than to the plan. Neither changes what is built.

**The word *profile* collides, and no rename fixes it.** The specification has its own profiles —
`DET`, its deterministic profile, and `FUL`, its full one — and the core has profiles in the sense
this whole document is about. Wherever the roadmap says *profile* unqualified it means the core's
sense; the specification's sense is always written `DET` or `FUL` or spelled out. This profile
implements `DET`, which [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
records as a refinement rather than a subset.

**A sibling profile is further along, and its evidence is not this profile's.** The other intended
first profile has landed milestones and retained bundles. **No total, measurement, review decision
or Native AOT sample of its own establishes anything here**, and no gate in this plan may cite one.
What *does* transfer is the core's rules as it discovered them — a rule that fires on any
`Broiler.VM.Profile.*` assembly fires on this family's too — and those arrive as corrections above
rather than as evidence.

---

## 5. Open, and therefore not corrected

A reader using this file to tell settled from unsettled needs the other half of the answer. These
are carried by the plan as **questions**. **The [ledger](roadmap.status.md) is the authority** for
every one of them.

**Where a question cannot be answered by this component at all, the plan names the point at which
its *state* is recorded, and that is not the same as answering it.** Four of the entries below are
of that kind — the retention amendment, the argument channel, the two third-party pins, and the
reconciliation of two profiles' declared defaults — and each says where the state lands. A release
that publishes a question truthfully has not closed it.

- **Where the store lives relative to a core instance state** — the single most consequential open
  question in the plan. Three readings exist, one is rejected, and
  [section 11](roadmap.md#11-the-store-instances-and-linking) names the milestone that chooses
  between the other two. *(Noted 2026-09-26: since UBC-4 one of the two, reading C, needs a
  runtime-scoped member the universal bytecode's family contract does not have — see
  [WAC-37](#wac-37). The question is still open, and still WA-6's.)*
- **The guest-observable `memory.grow` refusal** — [WAC-03](#wac-03) establishes that no spelling
  exists on the shipped contract. What is open is the amendment, which is **opened as blocking
  rather than filed and held** — the one row in this plan that is. It carries a holder and an
  unblock condition, and **the fallback is WA-10's release decision** published as a named
  deviation — see [WAC-16](#wac-16). The MVP programme defers the co-signing this row waits on, and
  the plan names the route it takes in that absence, so **WA-5 is not prevented from starting** and
  the row binds what this profile may claim rather than what it may commit — see
  [WAC-28](#wac-28). **None of that answers it**, and naming a route taken without a decision is
  the most answer-shaped thing in this list that is not one. *(Noted 2026-09-26: a memory
  representation has since been chosen without it, by WAD-0001 at the universal bytecode programme's
  milestone UBC-4 — see [WAC-38](#wac-38) — and the choice answers nothing here either: the amendment
  is as open as it was.)*
- **The argument channel, and every other row of section 20** — reconciled with the other profile
  and filed, not scheduled. The amendment procedure is currently unexecutable. **State recorded at
  WA-10**, which publishes the register row by row with the deterministic failure or exclusion each
  held row leaves standing — see [WAC-17](#wac-17). Publishing a held row does not make it
  admissible.
- **The specification revision and the conformance-suite revision** — neither is pinned.
  Retrieving, hashing and archiving third-party material is a human action nobody has performed.
  **The suite's licence and attribution obligation waits on the same action** and lands at WA-4 with
  the ingestion, not at WA-0 — see [WAC-15](#wac-15).
- **The reconciliation of two profiles' declared defaults** — it belongs to whichever component
  composes both, and that component does not exist and has no owner. **Named as unowned in the
  support table at WA-10** — see [WAC-18](#wac-18) — which publishes the position and
  reconciles nothing.
- **Everything else.** No milestone has started — the [ledger](roadmap.status.md) holds each state —
  and this list names only the questions the plan itself flags as open rather than the work it has
  not begun. *(Corrected 2026-09-26: "No milestone has started" was a status claim this file does not
  make, and it has been false since 2026-09-07. The ledger holds each state; the rest of the bullet
  stands.)*

---

## 6. Update rules

1. **Correct the roadmap; record the correction here.** Edit the plan to say the new thing in its
   own voice, then add an entry. A roadmap sentence that narrates its own history is the state this
   file exists to remove.
2. **One entry per changed reading, minted in order, never reused, never edited away.**
3. **An entry names an authority outside this file** — a core ADR, core source, a sibling record,
   or a ledger row.
4. **Cite records, do not copy them.** Two copies of an argument drift on the first edit.
5. **Transcribe no moving count.** A number a later milestone changes is named by the record that
   holds it.
6. **This file records no status**, never uses the ledger's mark vocabulary, and never says
   anything is accepted, validated or supported.
7. **A sibling profile's finding enters here only where its subject is the core.** Where the
   sibling decided something about *its own language*, this profile's plan is unaffected and no
   entry is minted. Where it hit a rule that binds every profile family, the finding is this
   profile's too and arrives before the build that would otherwise teach it.
