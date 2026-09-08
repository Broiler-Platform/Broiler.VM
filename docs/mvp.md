# The MVP programme, and what deferring a decision does not defer

**Owner:** Broiler.VM architecture owner, who also holds the release, security, contract-minting and
review roles [ADR 0012](adr/0012-security-ownership-and-support-matrix.md) names — six roles held by
one person, which this repository records as EX-30 rather than resolves.

**Date:** 2026-09-07
**Core contract version:** 1
**Status:** Not a decision. This record defers decisions and takes none.

This is the record every other document in this repository defers to when it says that a decision, a
review or an acceptance is deferred. It is not an ADR: it decides nothing, mints no core contract
version, approves no boundary record and moves no ledger row. What it does is fix the *scope* of a
deferral, so that "deferred" means one thing in every document that uses the word rather than
whatever the document's own convenience needs it to mean on the day.

**Where this record and another document disagree about what a deferral covers, this record is the
one to correct, and the other document is not thereby right.** A governing record that quietly
widens its own scope is the failure the whole documentation system exists to prevent, and a record
that claims the last word on its own accuracy has already committed it. If a reader finds a
statement here that the ledgers, the rule register or a retained bundle contradict, the ledgers, the
register and the bundle are the authority — this document is prose about them.

---

## 1. The instruction, and what it is for

**The instruction, recorded as an instruction and dated 2026-09-07.** The repository owner asked for
the following, in these terms:

> Advance Broiler.VM to multiple INPUT profiles — JavaScript and WebAssembly; to multiple OUTPUT
> forms — bytecode, x86-64 and arm64; to full-featured JavaScript including its compiler; delivered
> as an MVP with decisions and reviews DEFERRED; documentation first, then implementation, then test
> cases and benchmarks including test262 and Octane, with a CLI an end user can drive.

It is quoted rather than paraphrased because every later section of this record is an argument about
what those words reach, and an argument about a paraphrase is an argument about the paraphraser.

**What the instruction is for, stated plainly: capability now, ceremony later.** The deferrals it
names are all procedural. Approving a proposal, signing a review, co-signing an amendment and
collecting an acceptance bundle are acts a second person performs, or that one person performs on a
day set aside for it; none of them changes what the component can do, and all of them are currently
gated behind a single individual who holds every role that would perform them. An MVP that waited
for them would wait on a queue of one. So the instruction buys the right to build the capability
first and to perform the ceremony afterwards, which is exactly the trade
[update rule 8](roadmap.status.md) already grants in the core ledger's own words: **human review
gates a release, not a development step.**

**What the instruction cannot do, and this is the whole reason this record is long.** An instruction
can defer a *procedure*. It cannot defer a *fact*. Nobody can instruct a rule to pass, a suite to be
green, a milestone to have evidence it does not have, or a support table to be true of a component
it is not true of. The deferrals below are therefore a list of procedures, and section 3 is the list
of facts they do not reach — and the second list is the load-bearing one.

**The ordering the instruction fixes — documentation first, then implementation, then tests and
benchmarks — carries a trap, and naming it is part of what this record is for.** Writing the plan
before the code means that for a long stretch the most detailed description of this component's
capabilities is a description of capabilities it does not have. Every document written in that
stretch is *plan*, which the status vocabulary of both ledgers defines as proposed scope,
sequencing, ownership or an exit gate — and not implementation or validation evidence — and which
the `Not started` state answers with the sentence that governs this whole programme: **planning text
does not change this state.** A reader who meets a WebAssembly milestone written to the same
standard as a JavaScript one may reasonably infer that the two are equally far along. They are not,
the ledgers say so, and the documents that describe the work must not undo that by writing well
about it.

---

## 2. What is deferred

Four items. Each is stated as what is deferred, what deferring it would have bought had it not been
deferred, and what its absence costs while it stands. The third part is the one a reader should not
skip: a deferral with no stated cost is a deferral nobody has priced.

### 2.1 Approval of boundary records — every ADR stays `Proposed`

**What is deferred.** No record under [`docs/adr/`](adr/README.md) moves from `Proposed` to any
approved status, and the ten contract-bearing records stay at core contract version 1 with their
status unchanged. The candidate-amendment register of
[ADR 0003](adr/0003-core-contract-v1-and-amendments.md) continues to record shape and not intent: no
candidate is proposed, approved or scheduled.

**What approval would have bought.** A boundary that a later document may cite as settled rather
than as argued. An ADR at `Proposed` is a position with reasoning attached; an approved one is a
position a downstream record may build on without re-opening it, and the difference matters most
where two components meet, because that is where re-opening is expensive.

**What its absence costs.** Every citation of an ADR in this repository is a citation of *reasoning*
and never of *settlement*, and a document that reads an ADR as settled has made a claim the ADR does
not support. This bites hardest on the multi-profile work: the WebAssembly plan is written against
the shipped core rather than against the core's prose, which is the right choice precisely because
the prose is unapproved. It also means that a route this programme takes because an ADR argues for it
is a route taken without a decision, and section 5 is where such routes are recorded.

### 2.2 Human review — `HUMAN_REVIEW.md` stays unsigned and `PENDING`

**What is deferred.** No named human records a review decision on any code unit.
[`HUMAN_REVIEW.md`](../HUMAN_REVIEW.md) stays unsigned, and every relevant unit's human line reads
`PENDING` — the state the generated records report as `HUMAN_PENDING`, and the term the other
documents of this programme use for it. New work adds units and adds pending lines with them; the
count of units grows and the reviewed count does not.

**What review would have bought.** Release gate 9, and with it everything the gate stands in front
of: a published package, a claimed runtime identifier, an issued support table, and any milestone at
`Accepted`. The decision is bound to the fingerprint of each declaration rather than to a revision,
so a reviewed unit that changes afterwards reports as stale — which is the property that makes
review worth having at all, rather than a signature on a moment.

**What its absence costs.** Exactly those four things, and it costs them absolutely rather than
partially. **Building and merging is permitted; publishing anything is not.** A reader who wants a
weaker reading of this — that an unreviewed component may ship as a preview, or may claim a runtime
identifier it has demonstrably published and run on — should read ADR 0001's binding rule again: no
Broiler.VM package is published without a completed review naming the reviewed commit. The rule has
no preview limb. The generated machinery is deliberately unhelpful here: the assurance generator
refuses to write a human verdict it was not given, so a missing signature cannot be manufactured by
regenerating anything, and that refusal is itself a rule.

### 2.3 Evidence-bundle collection and milestone acceptance — no row reaches `Accepted`

**What is deferred.** The collection of the acceptance bundles that would let a milestone's owner and
reviewer confirm that every objective exit condition is covered, and therefore the transition itself.
No row in any ledger in this repository moves to `Accepted` under this programme.

**What acceptance would have bought.** The only status in the vocabulary that means a claim is
finished: an immutable, reviewable bundle identifying its sources and its gate, recording the
commands and the environment, retaining its outputs, and demonstrating every part of the exit gate —
plus a recorded owner-and-reviewer decision naming the date and the bundle. Partial success cannot
use that state, and that is the point of it.

**What its absence costs.** Rows accumulate at `In progress` with their open gate conditions listed,
which is section 4's subject. The cost that is easy to miss is procedural rather than
presentational: update rule 2 forbids copying a planned exit gate into an evidence column, so the
absence of a bundle cannot be written around by describing what the bundle would have shown. An
evidence column with no bundle says so, in those words, or it is untrue. And update rule 4 forbids
promoting a seed, shell, smoke, analyzer-only or shape-only result beyond what it proves, so a
partial bundle collected under this programme leaves its milestone `In progress` rather than earning
a qualified pass.

### 2.4 The core contract amendment procedure's co-signing

**What is deferred.** The co-signature the amendment procedure of ADR 0003 section 6 requires before
a new core contract version may be minted.

**What co-signing would have bought.** A minted version, and with it the nine required rows — the
driving capability, the per-class diff, the counterweight check, the additive-or-breaking test, the
new minimum supported version, the public API impact, the evidence re-evaluation table, the support
rows to be published, and the list of contract-bearing records that change revision.

**What its absence costs.** Any work whose first step needs an amendment is recorded `Blocked` with
this component named as the holder, and is not scheduled. One such item already exists and is not
hypothetical: the WebAssembly profile needs a refusable retention member on the metering surface,
because on the shipped contract a retention report returns nothing and a refused charge latches
exhaustion, so there is no spelling of a guest-observable growth refusal at all. The core roadmap's
own profiles section already records that as the one WebAssembly amendment with no local workaround,
and as a blocker this component holds. Section 5 records the route the MVP takes instead.

***This deferral is the one the instruction did not create, and saying so is not a technicality.***
The procedure was already unexecutable before 2026-09-07, for the reason the core roadmap states in
its own voice: one person holds the minting role and both co-signing roles, so no co-signature would
be independent. Deferring it changes nothing about its availability. It is listed here anyway,
because a reader who finds three deferrals in this record and a fourth unexecutable procedure
elsewhere will reasonably wonder whether anybody noticed — and because a deferral that costs nothing
extra is still a deferral, and one row of section 5 exists for no other reason.

---

## 3. What is not deferred

Five items. Each is a rule rather than a preference, and each is stated with the failure it prevents,
because a rule whose failure mode is unstated is a rule a reader will trade away under pressure.

**The sentence this whole record exists to carry: an MVP may buy speed with ceremony, and may not
buy it with truth.** Everything in section 2 is ceremony. Everything below is truth. The line between
them is not a matter of degree and there is no exchange rate across it.

### 3.1 The automated gates

**The rule.** The build, the test suite, the architecture-rule register and the assurance generator's
own consistency are not deferred by anything in this programme. A rule that fails still fails; a
suite that is red is red; a generated artefact that is not byte-identical to what the generator would
write is stale, and the gate says so rather than tolerating it. Adding a code unit means hand-writing
its assurance annotation and then regenerating, because the generator refreshes annotations and never
authors them, and a write run followed by a plain run of the suite is the only thing that makes such
a change green.

**The failure it prevents.** A component that publishes a prohibition it does not check. That was
not a hypothetical failure here: until 2026-09-07 the rule forbidding dynamic loading and IL emit
reached no hand-written machine-code path at all, so a profile that mapped a page executable passed
every automated gate this component had while tripping a stop condition this component publishes.
**The hole is closed by three new rules and a widened fourth rather than named by one paragraph,
and this record is the last document still describing it as open.** Rule **B5c** reads the ImplMap
and ModuleRef tables of every shipping assembly's build output — where `VirtualProtect` and
`mprotect` are rows in the calling assembly's own metadata that a rule written over MemberRef and
TypeRef is blind to by construction — and permits them in one named arming path and nowhere else.
Rule **X1** parses the checkout's product and composition source with Roslyn and holds that arming
path to one place in the tree, and every protection it passes to a named constant the arming path
itself declares. Rule **K5** holds each composition-register row's native-execution cell against
those same ImplMap tables and the checkout's source, and fails in both directions. Rule **B5**
widened on the same date from the three core assemblies to every assembly a published image can
contain, and its member list gained the native half. All four are `Active` with no activation
milestone, each carries the negative control its register row names, and the suite is green.

*(Corrected 2026-09-08.* This paragraph read "That is **not** a hypothetical failure here: the core
roadmap's native-execution milestone records that the rule forbidding dynamic loading and IL emit
reaches no hand-written machine-code path at all, so a profile that maps a page executable
**today** passes every automated gate this component has while tripping a stop condition this
component publishes. **The honest statement is that the hole is named, not that it is closed.**"
Every clause of that was true when it was written and none of it survived 2026-09-07. **A record
that goes on describing a closed hole as open fails in the same way as one that describes an open
hole as closed** — the direction differs, the reader is misled either way, and section 3.2 below
names the shape of it — so the superseded reading is quoted here rather than deleted. What the
paragraph's last sentence guarded against is unchanged and still binds, with the tense corrected:
**any document describing a native mapping as checked beyond what those four rules actually read is
making the untruthful support claim section 3.3 stops the work for**, and what they read is
metadata tables and source text rather than a run.)*

### 3.2 The status vocabulary

**The rule.** A row says what its evidence shows and no more. `Not started` means no milestone-owned
implementation or accepted gate evidence has been recorded, and planning text does not change it.
`In progress` means work or evidence collection has begun, and requires the row to link its working
evidence and list every open gate condition. `Blocked` requires a named blocker, a named holder and a
named unblock condition, and lack of scheduling is explicitly not a blocker. `Accepted` cannot be
reached by partial success. `Superseded` requires a dated decision and a linked replacement.

**The failure it prevents.** A roadmap that reads as a changelog. The failure has already happened
here once and is retained rather than tidied away: the core ledger carries a correction dated
2026-08-31 recording that its own summary paragraph went on saying six milestones were not started
while those milestones collected bundles, and it names the reason it matters — **a reader who skips
the table quotes the paragraph.** This programme writes a great deal of prose beside a great many
unmoved rows, which is precisely the condition that produced that drift.

### 3.3 The stop condition on an untruthful support claim

**The rule.** The core roadmap's closing stop condition is not deferred, in either half. Its second
half is the one this programme has to hold: a difficult or slow milestone is not itself a stop
condition, and an untruthful support claim is. The JavaScript profile publishes the same clause in
its own gates. So a milestone may be late, hard, re-scoped or abandoned under this programme without
anything stopping; a sentence in a support table that a retained run does not support stops the work.

**The failure it prevents.** The single failure an MVP is most likely to commit, because it is the
only one that makes the MVP look finished. **This is the one thing an MVP may not buy speed with**,
and it is why release gate 1 requires the support table to name a capability the contract admits and
the product does not provide: a capability merely absent from the table is a capability a reader
infers from silence.

### 3.4 The non-advertisement of every composition, and the three-package pack set

**The rule.** [The composition register's](compositions.md) advertised set stays empty, and its
emptiness is not a placeholder. Every composition root in the checkout stays a demonstration and
stays non-packable; the packable set stays exactly `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and
`Broiler.VM.Runtime`; every project of every profile family declares no package identity and carries
the literal non-packable property the family rule reads out of the project file. Making a composition
advertised is a release decision ADR 0012 owns, and is not an edit to a column.

**The failure it prevents.** An MVP shipping. Advertisement is the act that gives a consumer
something to depend on — a package identity, a supported runtime-identifier set, a compatibility
promise across core contract versions, and a named owner who answers for it — and none of those
exists. The composition this programme most obviously wants is the one that composes two input
profiles at once, and the register already refuses to carry a row for it, because rule K1 fails on a
row naming a root that does not exist.

### 3.5 The prohibition on publishing

**The rule.** No package is published. No runtime identifier is claimed. No support table is issued.
This is the release half of update rule 8 and the whole of ADR 0001's binding rule, and it is
unconditional rather than pending.

**The failure it prevents.** Handing a consumer a component nobody has read. It is stated separately
from 3.4 for a reason a reader should not have to reconstruct: 3.4 is about what this repository
*declares*, and 3.5 is about what it *emits*. A composition could be left unadvertised and a package
pushed anyway; a package could be withheld while a support table claimed a runtime identifier nothing
ran on. The two fail in different places and are held by different mechanisms.

**A reader who wants this section shorter will observe that 3.3, 3.4 and 3.5 are one rule said three
times.** They are not, and the difference is worth the words. 3.3 is a stop condition: it halts work
in progress. 3.4 is a state of a register: it is true or false of the checkout at any moment. 3.5 is
a prohibition on an act: it forbids something nobody is currently doing. A single rule covering all
three would have to be enforced by whichever of those mechanisms was weakest, and the weakest of the
three is a reader.

---

## 4. What this does to each ledger

**Rows accumulate at `In progress`, none reaches `Accepted`, and that is an instruction rather than
neglect.** The distinction is visible in the rows themselves, and a reader should check it rather
than take it on trust: an instructed row lists every open gate condition, names the evidence it does
have, and states what that evidence does not demonstrate; a neglected row has an evidence column
nobody has revisited. The two look different, and the difference is the only defence this programme
has against being indistinguishable from abandonment.

**The core ledger.** [`docs/roadmap.status.md`](roadmap.status.md) is the sole authority for the
state of every core milestone. Nothing in this record moves a row there, and nothing this programme
writes may. The native-execution milestone no longer has only planning text standing behind it: the
enforcement its own row named as its first action has landed in the core architecture suite as
rules **B5c**, **K5** and **X1** — `Active`, owned by ADR 0001, each with the negative control its
register row names — and the composition register now cites a retained bundle at
`docs/evidence/vm-7-cli-001/`, against a row declaring `x86-64` in the very column K5 reads. Its
instruction selection and its calling conventions remain the profile's under update rule 6, it
reaches no `Accepted` state because acceptance is exactly what this programme defers, and **what
its row should now say is the ledger's to write and not this record's**: nothing here moves it.

*(Corrected 2026-09-08.* This passage read "The native-execution milestone is `Not started` and
stays so while what exists for it is planning text — a milestone record, an invariant, a release
gate and a set of risk rows widen what a milestone *may* propose and demonstrate nothing — and its
own row already says that its first action is not implementation." Three `Active` core rules and a
retained bundle were in the checkout when that sentence was read, so this record was agreeing with
a stale row rather than deferring to a current one — which is the drift section 3.2 above names in
somebody else's document, committed here. **Understating a milestone that widens the
executable-memory boundary is the same defect as overstating one.** The superseded reading is
quoted rather than deleted, and the rule it was protecting is untouched: this record still moves no
row anywhere.)*

**The JavaScript profile's ledger.** [The profile's own
ledger](../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md) holds every JavaScript row,
under its own three-mark legend, and this record neither reads it nor writes it. Core update rule 6
and the profile's bidirectional counterpart both say why: a core result never advances a row there,
and no row there advances a row here. **Sharing a repository is not sharing a ledger.** So a
conformance total, a benchmark score or a differential transcript collected by that profile is
evidence about that profile, is recorded in that profile's own bundles, and may not appear in a core
document at all — where update rule 6 is the general form of that rule, release gate 8 fails a
release that claims or implies language performance, and the native milestone's own final clause
binds that milestone's own bundle, which does not exist. **No automated rule reads a core record
for a profile's conformance total**, so that half of the rule is one a reader enforces and not a
gate, and this record says so rather than leaving a reader to assume a check that is not written.

**The WebAssembly profile's ledger.** [The WebAssembly profile's
ledger](../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md) exists, is dated, and marks
WA-0, WA-1, WA-3 and WA-5 `In progress`, WA-2 `Blocked`, and the remaining six `Not started`. The
blocked one is blocked on the core contract not being accepted — and its own row now records which
half of that blocker binds a commit and which half binds a release, which is update rule 8 applied
rather than relaxed. One more waits on the unfiled amendment of section 2.4, and blocks nothing
today only because the milestone before it has not started either. **Nothing this programme writes
may move a WebAssembly row, because that ledger is the authority and code existing is not
evidence**: `src/Broiler.VM.Profile.WebAssembly/` now holds a project file listed in
`Broiler.VM.slnx`, twenty-one source files, the descriptor `WebAssemblyProfile.cs`, this profile's
own group in the rule register — `W1` and `W2`, both `Active` — and one evidence tree at
`docs/evidence/wa-0-001/`. **Not one of those facts is a gate met, a bundle accepted or a row
moved**, which is the distinction section 1's trap actually turns on: a plan of that quality is a
trap whether the directory beside it is empty or full, and the ledger is what keeps the two apart
either way.

*(Corrected 2026-09-08.* The two sentences above read "marks **every one of its milestones `Not
started`**" and "**Nothing this programme writes may move a WebAssembly row, because there is no
WebAssembly code**: `src/Broiler.VM.Profile.WebAssembly/` holds a documentation directory and
nothing else — no project file, no source file, no descriptor, no rule-register group, no evidence
tree, and no entry in either solution. A plan of that quality standing over an empty directory is
exactly the condition section 1's trap describes". Every clause of that enumeration was true when
it was written and none of it survived 2026-09-07, when the profile acquired the project, the
sources, the descriptor, the `W` group and the evidence directory named above; the nearest thing to
a survivor is that the project is absent from `Broiler.VM.Mobile.slnx`, which is not what "either
solution" was read to mean. **A record that understates the component beside it fails in the same
direction as one that overstates it**, so the superseded reading is quoted rather than deleted —
which is what [`docs/compositions.md` section
5b](compositions.md#5b-compositions-this-programme-plans-which-this-register-does-not-have) did
with the same drift, on the same day, for the two composition roots this profile is composed by.
The conclusion is unchanged and is now carried by the reason that is actually load-bearing.)*

**The native output forms have no ledger of their own, and will not get one from this record.** They
are a milestone of the core, because the core owns whether a composition may execute a native
payload; the instruction selection, the register allocation and the calling convention belong to the
profile that emits them, because the core generates no machine code and learns no encoding. Their
evidence is therefore split across two ledgers that never cross. A reader looking for one row that
says how far native execution has got will not find it, and inventing one here would be a third
ledger nobody agreed to keep.

**This document holds no row and is not a ledger.** If a sentence anywhere above appears to record a
state, that is a defect in this document; the ledger it contradicts is right, and the correction
belongs here rather than there.

---

## 5. Routes taken without a decision

**A deferred decision is a decision nobody took, not a decision that went a particular way.** That
sentence is the whole reason this section exists. Deferring a decision does not stop the work from
needing an answer; it means the work proceeds down one branch anyway, and the branch it took is
invisible unless somebody writes it down. So where this programme takes a design route that a
deferred decision would otherwise have chosen between, the route is recorded here and named as taken
without a decision — **so that the deferral is visible where its consequence is, and not only in the
record that defers it.**

**What a row here is, and four rules that govern the set.** A row is not a decision record: it mints
no identifier in any decision series, it settles nothing, and it does not become a decision by being
read. A row names four things and nothing else — the route taken, the alternative not taken, what
would settle the question, and who would decide it. A route recorded here may be reversed without a
correction entry, because there is nothing decided to correct, and **that is precisely the cost:
anything built on a row here is work that may have to be unbuilt.** And adding a row is the act that
makes a route visible, so taking a route and not adding a row is the failure this section exists to
prevent — not a documentation lapse but the reintroduction of the invisible branch.

Identifiers are minted in this record's own namespace, `MVP-n`, for the reason the profile's
unscheduled proposal documents give for theirs: an identifier from a milestone series would read as a
milestone somebody is tracking.

| # | Route taken | Alternative not taken | What would settle it | Who would decide |
|---|---|---|---|---|
| MVP-1 | **WebAssembly memory growth is refused against the profile's own declared maximum**, so the guest-observable refusal value the specification requires exists and is exercised, the operation completes normally and the allowance is unspent. A refusal caused by a CORE budget is not guest-observable, and is published as a named deviation from what the specification says a growth instruction answers | **A refusable retention member on the core's metering surface.** On the shipped contract a retention report returns nothing and a refused charge latches exhaustion, so the core rewrites the completed step and the guest observes an aborted operation rather than a refusal value it can act on | A minted core contract amendment carrying a co-signature, **or a recorded refusal of it**. Either answers the question; silence does not. An unanswered question would have made the interpreter milestone `Blocked` rather than merely late at the moment its predecessor would otherwise let it start — and under this programme it does not, because the route this row takes needs no amendment; the row stays open, with the same holder and the same unblock condition | The core contract and release owners, with the profile's owner supplying the driving capability |
| MVP-2 | **A sixteen-byte WebAssembly value slot, reserving the vector width now** rather than widening every slot later | **An eight-byte slot with a separate side stack for 128-bit values**, which is also defensible and is recorded here as an alternative rather than dismissed as one | The nine-row value, store and frame decision the WebAssembly plan makes a gate on *entry* to its interpreter milestone, taken against a measured interpreter and against the vector manifest's own scope | The WebAssembly profile's owner |
| MVP-3 | **The native output form's calling convention carries no managed reference at all.** The emitted frame is an unmanaged fixed-layout record of pointers to operand, local and constant storage, a fuel cell and a helper table; the entry point is reached through an unmanaged function pointer with the collector transition taken rather than suppressed | **A rooting scheme** that lets emitted code hold a managed reference — which the JavaScript profile's own value representation would otherwise force, since it carries an object reference beside a number | The roadmap's risk row — a profile that cannot state where its emitted code's references are rooted has not earned the form — is answered **by construction** here. It stops being answered the moment one emitted frame holds a reference, and that observation is what reopens this row | The JavaScript profile's owner with the core's security owner |
| MVP-4 | **arm64 is emitting-only**, in the word [the core roadmap](roadmap.md)'s release gate 11 binds. Encoding may be written and its output compared byte for byte against a retained expectation; no image runs it. The declared reason is concrete rather than a scheduling preference: the architecturally required instruction-cache maintenance sequence has no managed expression and no reliable library export | **arm64 as a second executed backend**, which multiplies publish-and-run by another architecture across the declared runtime-identifier matrix | A managed route to instruction-cache maintenance on each arm64 runtime identifier, and a publish-and-run record on each. Until both exist the support table names the arm64 backend **emitting-only**, in that word — the exact term release gate 11 and the native-execution milestone's exit gate bind, and the word a rule fails the release for omitting — and states that it has run nowhere and claims no runtime identifier at all | The core release owner with the profile that emits the form |
| MVP-5 | **x86-32 is dropped from scope**, and no backend targets it | **x86-32 as the second backend** — which is what the probe that motivated the native-execution milestone actually used | Nothing is waiting on this one, and **the rule is not that x86-32 is unsafe**: it is that a milestone whose motivating accident belongs to one calling convention does not add that convention second, and a later milestone that wants it inherits both the fixture and the ABI table it will need. A consumer asking for the runtime identifier would reopen it | The core release owner |
| MVP-6 | **WebAssembly invocation arguments are encoded into the entry-point text**, under a length-prefixed grammar, because the core carries those bytes verbatim and the invocation request has no argument channel | **A typed argument vector on the invocation request**, which is an amendment; and a host capability that hands the arguments back, which the conformance suite's own modules cannot cooperate with | A minted amendment, or a measured statement that the text encoding costs nothing that matters — including exact round-tripping of floating-point values and export names containing the encoding's own separators | The core contract owner with the WebAssembly profile's owner |
| MVP-7 | **The native output form is a WHOLE-ARTIFACT form under a restricted feature manifest.** A third manifest, `broiler.javascript.numeric`, admits a numeric subset of JavaScript and refuses every construct outside it at compile time, by name, with a diagnostic code and a source position; a program in it compiles in whole or not at all, so an artifact is native or it is bytecode and no handle carries two forms. There is no per-unit choice, no entry guard, no bailout, no fallback, no deoptimization and no promotion — the executor for a native artifact never reaches the interpreter, and a machine that cannot run the artifact's architecture refuses to instantiate it rather than running the other form | **A mixed-form artifact with per-unit compilation and an interpreter fallback**: compile the units a backend finds eligible, guard each emitted entry on the shape of its arguments, and let the interpreter carry everything else. **That is what a production engine would build**, it compiles far more of the language, and it is what this repository's published rules currently forbid — the JavaScript profile's amended non-goals pin *one form per handle, no promotion* and name a run-time form choice as the second execution arm they refuse, and the core's GC-rooting risk row makes a per-unit fallback the same thing under another name. It was the design the backend roadmap described until 2026-09-07 | **An amendment to the profile's non-goals, or a recorded refusal of one** — the rules that exclude the alternative are published records and not implementation limits, so the question is answerable only by changing one or by deciding not to. Until then the cost is paid in the open and is not small: **the compilable language is small and it is not JavaScript.** A native artifact runs numeric kernels; a program that touches a property, builds an array, holds a string or catches an error is refused before any artifact exists, where the alternative would have compiled part of it and run the rest. Widening the manifest is a different design with a rooting scheme in it and not a later increment of this one | The JavaScript profile's owner with the core's contract and security owners |

**MVP-1 is expected to stand for the whole of this programme, and that is not an argument for taking
it lightly.** The amendment is unfiled and the procedure that would mint it is unexecutable, so the
question cannot be answered from inside this component. The WebAssembly plan already places the
publication of the deviation at its release milestone, as a release decision, and explicitly not at
the interpreter milestone as a quiet implementation choice — which is the same instinct this section
is built on.

**MVP-2 is the row a reader should be most uncomfortable with.** The plan itself says the
vector-width question is the one whose late answer invalidates the answers around it, which is why
the MVP answers it early. Answering it early without a decision and answering it late with one are
both bad; this record says which was chosen, and does not pretend the choice was free.

**MVP-3 answers a risk by construction, and answering by construction is narrower than answering.**
The route buys the answer by refusing a capability. *(Corrected 2026-09-07: this paragraph said that
every guest value the emitted code cannot represent without a managed reference **becomes a bail-out
to the interpreter rather than a faster path**, which was written against a design that has since
been replaced by MVP-7 and is now false in both halves — there is no bail-out, and such a value is
not represented at run time at all because the manifest refuses the program that would produce it, at
compile time.)* What holds unchanged is the limit: **the form is worth nothing on any program the
numeric manifest does not admit**, and that limit belongs in these rows rather than in a footnote to
a benchmark nobody has run.

**MVP-7 is the row this register was built for, and it arrived after the work rather than before
it** *(added 2026-09-07)*. The route was taken in the change that built the form; this row was
written in the change that corrected the records afterwards. **Section 5's own rule is that adding
the row is what makes a route visible, so taking the route and not adding a row is the failure it
exists to prevent** — and the row was late rather than absent, which is a smaller failure and is not
none. What makes it the register's hardest row is that the alternative is not a worse design: it is
the design a reader would expect, the one that compiles a language rather than a subset of one, and
the one **this repository's own published rules forbid**. A reader who thinks those rules are wrong
is disagreeing with a record and not with an implementation, and the settling condition names the
record.

**MVP-5 is recorded even though it costs almost nothing, and that is deliberate.** A register that
holds only the uncomfortable rows is a register a reader learns to distrust, and a route dropped for
good reasons is still a route nobody decided on.

**What this register is not.** It does not rank its rows: there is no partially-taken decision, so no
row here is more settled than another. It is not an exclusion register — an exclusion identifier
records something a retained bundle does not cover, and these rows record something nobody chose. And
it is not a backlog: nothing here is scheduled, nothing here has an owner in the sense a milestone
has one, and assigning either is the act that would turn a row into work somebody is tracking.

---

## 6. How this record ends

**Each deferral has a lifting condition, and each condition is somebody performing an act rather than
time passing.** None of them lifts on its own.

**Approval of boundary records** lifts when a named approver records a decision on each record. The
obstruction is not the procedure, which is written and executable, but the roster: while one person
holds every role, an approval is that person approving their own proposal, and recording it would be
an assertion of independence this repository refuses to make elsewhere. So the honest route to
lifting this one is splitting the roles, and until that happens `Proposed` is the truthful status and
changing it would be the untruthful claim section 3.3 stops the work for.

**Human review** lifts when a named human reads the work and records a decision on every relevant
code unit, bound to that declaration's fingerprint. The machinery is built and armed: the annotation
system enumerates the units, the generator refuses to invent a verdict, and the release gate refuses
a publish while any relevant unit is unverified. **What is missing is the human, not the mechanism**
— and this is the deferral whose cost grows fastest, because every unit this programme adds is one
more unit somebody will eventually have to read.

**Milestone acceptance** lifts per milestone, when its bundle exists and its owner and reviewer
confirm that every objective exit condition for that record is covered, with the decision date and
the bundle identifier recorded in the affected row — and, where owner and reviewer are the same
person, with the non-independence recorded in the row rather than resolved by assertion.

**The amendment procedure's co-signing** lifts when a second signatory exists, or when the refusal is
recorded as a refusal. A refusal is a real answer and unblocks the work that waits on it, because a
profile that knows an amendment will not come can publish the deviation instead. Silence is the only
outcome that unblocks nothing.

**Each row of section 5 ends when its own settling condition happens**, and not before. A row does
not lapse, does not expire, and is not closed by the work built on it turning out to succeed — a
route that works is still a route nobody chose.

---

**This document is not a decision and approves nothing.** It records an instruction, fixes the scope
of what that instruction defers, names what it does not reach, and prices both. It changes no ledger
row, mints no core contract version, moves no composition into the advertised set, claims no runtime
identifier, and issues no support claim. Where a reader takes anything here as authority for a state,
the ledger is the authority and this record is not.

**And one sentence to carry away, if only one is carried.** An MVP here buys the right to build and
merge unreviewed work, which update rule 8 already grants. **It buys nothing whatever about what may
be claimed.** So the documents this programme produces plan work and record state, and they never
report a capability that no run has shown.
