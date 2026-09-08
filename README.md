# Broiler.VM

Broiler.VM is a new, planned NativeAOT-compatible component that executes verified bytecode
artifacts. It is a **host for language profiles, not a language**: it owns profile selection,
bounded loading, the verification boundary, the execution lifecycle, resource authority,
diagnostics, and composition evidence, and it owns no opcode set, value representation, or
language semantics of its own.

Profiles are **product project families in this component**, at
`src/Broiler.VM.Profile.<Language>*`, and the core never references a profile: a composition root
references both, which is the only direction that exists. **JavaScript** and **WebAssembly** are the
two intended first profiles, as `Broiler.VM.Profile.JavaScript` and
`Broiler.VM.Profile.WebAssembly`. Neither is planned by the core roadmap, and no core milestone
depends on either existing.

*(Corrected 2026-09-07. The first sentence read "Profiles are separate components that reference the
core", a reading ADR 0001's 2026-08-31 revision withdrew when it ruled that a language profile is a
set of product projects in Broiler.VM rather than a component of its own. The half of the sentence
that was load-bearing is unchanged and is restated rather than dropped: the reference runs one way,
and no core project references any profile. What changed is where the projects live, and the
correction is recorded rather than made silently because a reader who took the old sentence at its
word would conclude that trees which exist in this checkout cannot exist.)*

A profile is added by compiling it into a product and registering its descriptor directly.
Broiler.VM does not discover plug-ins by scanning assemblies, loading types by name, or using a
runtime extension directory, and it offers no binary plug-in ABI. That is part of the Native AOT
contract: every executable profile and host capability must be rooted by a direct, typed
reference.

## Status

[The status ledger](docs/roadmap.status.md) is the authority for accepted evidence. It records
**VM-0 through VM-7 as in progress and unaccepted.**

*(Corrected 2026-09-07. This sentence read "VM-0 through VM-4 as in progress and unaccepted, and
VM-5 and VM-6 as not started", and had been wrong since VM-5 and VM-6 collected bundles: the ledger
moved and this summary of it did not. The correction is recorded rather than made silently, because
a stale summary of the authority is the failure mode the ledger's update rule 1 exists against, and
it went unnoticed here for long enough to be worth a line.)*

*(Corrected 2026-09-08. The sentence above then read "VM-0 through VM-6 as in progress and
unaccepted, and VM-7 as not started", which is the correction beside it going stale in the same
direction one day later. VM-7's own named first next action landed on 2026-09-07: rule B5's scope
widened from the three core assemblies to every assembly a published image can contain and its
member list gained its native half, rule B5c was minted over the platform invokes no member
reference names, rule X1 over the arming path and the protections it passes, and rule K5 over the
composition register's native-execution column — each with a negative control that was watched
failing and then passing. Under [the ledger's own vocabulary](docs/roadmap.status.md) a milestone
that owns code is `In progress`, and that is the whole of what moved: no bundle accepts VM-7,
nobody has read it, and no row on this page or that one may reach `Accepted`. **Understating the
milestone that widens the executable-memory boundary is the same defect as overstating one**, and
it is recorded here rather than edited quietly for that reason.)*

**Both a WebAssembly profile and two native output forms exist at this snapshot, and none of them is
accepted, reviewed, claimed for a runtime identifier or advertised — which is a different sentence
from either half alone, and this section states both halves early rather than late.** The
WebAssembly profile is a product project in the solution — `src/Broiler.VM.Profile.WebAssembly`,
with a descriptor, a decoder for the binary format, a validator, a store and an interpreter —
composed by two never-advertised composition roots, `Broiler.VM.Composition.WebAssembly.Execution`
and `Broiler.VM.Composition.WebAssembly.Harness`, which
[the composition register](docs/compositions.md) lists by name; its plan and its own ledger are
under
[`src/Broiler.VM.Profile.WebAssembly/docs`](src/Broiler.VM.Profile.WebAssembly/docs/roadmap.md), and
that ledger rather than this file is the authority for what the bring-up has demonstrated. A
native artifact form — machine code as an artifact payload, executed in process — is
[VM-7](docs/roadmap.md#vm-7--admit-a-native-artifact-form-and-in-process-native-execution) in the
core roadmap, opened 2026-09-07, and it owns code: an **x86-64** backend whose bytes this host arms
and executes, an **arm64** backend that is **emitting-only** and has run nowhere, the enforcement
item that widened rule B5 and minted rules B5c, X1 and K5 with a negative control each, and one
retained bundle, [VM-7-CLI-001](docs/evidence/vm-7-cli-001/README.md), which accepts nothing and
closes nothing. **Existing is the weakest claim either of them makes**: no milestone of either is
`Accepted`, no person has read a line of either, `HUMAN_REVIEW.md` says `PENDING`, and the
register's advertised set is empty.

*(Corrected 2026-09-08. This paragraph read "**Neither WebAssembly nor any native output form
exists at this snapshot**", called the WebAssembly profile "documentation: a plan and a ledger of
its own ... with no project and no code beside them", and said VM-7 was "opened 2026-09-07 and
**not started**: no implementation, no evidence bundle, and not one of the rules, gates or negative
controls it names has been written". Every clause was true on the morning it was written and none
of it is true now: that directory holds twenty-one tracked source files, `Broiler.VM.slnx` carries
the profile project and its two composition roots, and VM-7's enforcement item landed the same day
with its negative controls watched failing. The correction is recorded rather than made silently
for the reason the correction above it gives — and for one more that runs the other way: **a
component that understates what it holds is as untruthful as one that overstates it**, and what
this paragraph understated was the arrival of an executable mapping, which is the last thing a
reader of this page should learn late.)*

**The work ahead of both is an MVP, on terms the repository owner set on 2026-09-07 and
[docs/mvp.md](docs/mvp.md) records.** What is deferred is approval of the boundary records, human
review, evidence collection and milestone acceptance, and the co-signing step of the contract
amendment procedure. What is **not** deferred is everything that decides whether this file is true:
the build and the suites still gate, a rule that fails still fails, the status vocabulary still
means what the ledger says it means, no composition is advertised and the packable set is still
exactly the three core assemblies, nothing is published, and an untruthful support claim remains a
stop condition rather than a price worth paying for speed. **The cost is that a deferred decision is
a decision nobody took**, not a decision that went a particular way — so where this component takes
a design route that an unmade decision would have chosen between, the route is recorded as
taken-without-a-decision at the place its consequence lands, and a reader who wants the list of what
is unmade reads `docs/mvp.md` rather than inferring it from confident prose.

What exists is [twelve boundary records](docs/adr/README.md) and an implementation of core
contract version 1: the profile-neutral contracts, the bounded binary primitives, the immutable
catalog, the runtime and its lifecycle, resource authority including shared aggregate budgets and
the full limit-precedence algorithm, guest-initiated-load mediation with its bounds, external
suspension, and two test-only fixture profiles that prove the contract. A composition-root host
publishes and runs under JIT, trimming and Native AOT.

The verification boundary is exercised by a retained malformed-input corpus of eighty-seven
artifacts, each with its hash and its expected answer, and by a deterministic fuzz target seeded
from it. Nothing about that is a claim that the component is safe: nobody has reviewed any of it,
and `HUMAN_REVIEW.md` records that absence rather than a decision.

The lifecycle is exercised under concurrency rather than described. Disposal drains a step that is
inside the profile before it releases anything, an operation a profile pinned to its starting thread
is refused on any other, a shared aggregate budget is spent once between concurrent runtimes rather
than once each, and a soak host runs four hundred thousand lifecycle cycles across a hundred
recycled runtimes and reports where the managed heap settles. Four rules that were frozen at VM-0
and implemented at VM-1 were being enforced nowhere until a suite could reach a second thread.

The composition claim is demonstrated rather than asserted. Two application-local consumer
profiles - written against `Broiler.VM.Abstractions` and `Broiler.VM.Binary` and nothing else -
are composed by two named roots listed in [the composition register](docs/compositions.md), each
of which publishes and runs under JIT, trimming and Native AOT. The closure of each published
image is read off the published output and contains exactly the profiles its register row
declares. Adding the second profile changed no file in any product project.

**What the core does not do is a language, and that is a different sentence from "no language
exists here".** The core ships no language profile: the packable set is exactly the three core
assemblies, every profile project declares no `PackageId` and carries `IsPackable=false`, and no
composition is advertised. What does exist in this repository is a JavaScript profile — a product
project family with its own bytecode format, verifier, interpreter, compiler front end, end-user
command line, ledger and evidence tree — and that ledger retains a whole conformance run and a whole
benchmark run, taken on one machine on 2026-09-05 and held as
[bundle JSW-10-001](src/Broiler.VM.Profile.JavaScript/docs/evidence/jsw-10-001/README.md): the
pinned `tc39/test262` checkout driven whole under the profile's wide manifest, and the fifteen
pinned Octane benchmarks driven whole.

**A second whole-suite conformance run was taken on 2026-09-08, and it stands beside that bundle
rather than inside it — two runs, two dates, two statuses, and this file merges them into no single
thing.** The bundle above is a collected and retained pair of runs from 2026-09-05. What was taken
on 2026-09-08 is one run and one workload: the same pinned `tc39/test262` checkout, driven whole
under the same wide manifest, by the same script, on **`win-x64`**, after the profile's front end
was given back a refusal it had lost — a BigInt literal, which it had been admitting and evaluating
as a Number, is refused at compile time naming the construct. **It was taken in a working tree
rather than into an evidence tree, and every consequence of that is a limit rather than a
footnote**: it is **retainable and not retained** — pinned, whole, and its own report says its
verdicts account for it, which is what makes it citable at all — while **no bundle has collected
it**, nobody has read a line of it, and **no row of any ledger, here or in the profile's, reaches
`Accepted` on it**. The status of the 2026-09-05 bundle is unchanged by it, and neither run's
counts may be added to, compared with, or substituted for the other's in this file.

**One fact about that run belongs on this page because it is a name and not a number.** The wide
manifest's whole-suite `unsupported` column, which had been empty, is empty no longer, and every
variant in it is the same construct — the BigInt literal meeting the refusal above — so **any
record in this repository saying the wide run names no unsupported family, or that its unsupported
column is empty, is false from 2026-09-08**. The whole-suite ratchet under `src/tests/conformance`
was re-based **by hand** for that run and could not be re-based any other way: it re-bases
automatically only when the suite revision or the manifest moved, neither moved, and what moved was
the engine on purpose — so the ratchet answered `Regressed` and refused, which is the design
working. A floor that had to be lowered by hand, with its old rows retired in writing beside the
reason, is a **weaker** claim than a floor that never moved, and it is recorded here in that
direction.

**No figure from any of those runs is printed here, and that is a rule rather than an omission.** A
conformance total, a benchmark score or a differential transcript collected by a profile is evidence
about that profile, belongs to that profile's bundles, and may not appear in a core document at all
— update rule 6 is the general form and [the MVP programme](docs/mvp.md) restates it, while release
gate 8 covers the performance half specifically: a profile result never advances a core row, and
sharing a repository is not sharing a ledger. **No automated rule checks a core record for a
profile's conformance total**, so that half is a rule a reader enforces and not a gate. A reader who
wants the counts opens the bundle, or runs `python3 eng/run-test262.py` and
`python3 eng/run-octane.py` and reads their own.

**What a reader may take from those runs, and what they may not.** They may take that a conformance
suite and a benchmark set were driven end to end, by scripts in this repository, against a pinned
and hashed checkout, with every count in the bundle read off a file the bundle retains beside it,
and that the conformance half of that was driven whole a second time on 2026-09-08 against the same
pin. They may **not** take that any of it is accepted — the bundle says in its own words that it is
not an acceptance, every relevant unit is `HUMAN_PENDING`, and the 2026-09-08 run has not even
reached a bundle to say so in; that the failing variants are diagnosed, which the bundle explicitly
declines to claim and which the later run claims no more strongly; that the benchmark scores compare
with anything, which the bundle and its own pin both refuse; that a working-tree run is
interchangeable with a collected one, which is the whole of why the two are kept apart above; or
that the core holds any evidence whatever about JavaScript, which it does not and which no passage
of this file may imply.

*(Corrected 2026-09-08. Two lead-ins above read "**No figure from either run is printed here**" and
"**What a reader may take from that run, and what they may not**", and both were written when the
only whole-suite runs this page knew of were the conformance run and the benchmark run the
2026-09-05 bundle holds. A third run exists as of this date and is described above; leaving the
singulars standing would have let a reader carry the bundle's retention properties across to a run
that has none of them, which is the one misreading this section cannot afford. **The correction
widens what is named and weakens nothing that is claimed**, and it prints no count of the new run
for the same reason it prints none of the old — which is why the rule sentence itself needed only
its arithmetic changed and not its rule.)*

*(Corrected 2026-09-07. This passage read "What does not exist is a language. There is no product
profile, no advertised composition, no persisted envelope, no concurrency evidence, no second RID,
and no performance measurement." It was true when it was written and three of its six clauses went
stale without it being touched: a product profile family arrived with the JavaScript profile's wide
surface, concurrency evidence arrived at VM-4, and core-overhead measurement arrived at VM-5 in
[the baseline register](docs/baselines.md). The other three are unchanged rather than dropped: no
advertised composition, which is restated above; no persisted envelope, which the contract admits
and no milestone implements; and no second claimed RID — `linux-x64` and `win-x64` have each
published and run, on one machine each, and the ledger records that neither is thereby claimed.
The correction is recorded rather than made silently because the stale half was the half a reader
quotes, and because it had gone stale in the direction this repository cares about most: a sentence
saying that no language exists, sitting at the front of a repository that runs a conformance suite
whole.)*

And **nothing here is accepted**: no human has reviewed the records or the code, so no capability
should be inferred from a passing test suite, a green publish, or this paragraph. Human review
gates a **release** rather than a development step - so this work is allowed to exist and to be
built on, and none of it may be published, claimed for a RID, or marked accepted until every
relevant unit carries a decision. The rule and its cost are recorded as update rule 8 in
[the status ledger](docs/roadmap.status.md).

## The two axes, and where each one stands

The component is being advanced along two axes at once, and they are independent. An **input
profile** is a language whose artifacts this host verifies and runs. An **output form** is what a
verified artifact's payload is made of. Adding one of either does not add one of the other, and a
document that reports progress on the pair as a single figure is reporting neither.

The input-profile axis holds **JavaScript**, whose plan and ledger are under
[`src/Broiler.VM.Profile.JavaScript/docs`](src/Broiler.VM.Profile.JavaScript/docs/roadmap.md), and
**WebAssembly**, whose plan and ledger are under
[`src/Broiler.VM.Profile.WebAssembly/docs`](src/Broiler.VM.Profile.WebAssembly/docs/roadmap.md).
The output-form axis holds **bytecode**, which every artifact in this repository still carries, and
machine code, which
[VM-7](docs/roadmap.md#vm-7--admit-a-native-artifact-form-and-in-process-native-execution) opened
as an artifact payload form and which the JavaScript profile's format now carries beside the
bytecode in an emitted-code section and a symbol section — **x86-64** first, whose emission and
execution are both in scope and both of which have happened on this host, and **arm64** as a
separate **emitting-only** backend whose execution is excluded rather than unscheduled: the
architecturally required instruction-cache maintenance sequence has no managed expression and no
dependable library export, so an arm64 backend claims no runtime identifier at all. The core's part
of that axis is bounded by its own invariants: the core generates no machine code and learns no
encoding, so instruction selection, register allocation and calling convention belong to the
profile that emits them, and the core owns only whether a composition may execute the result.

**At this snapshot two input profiles and three output forms exist, and none of them is
supported.** JavaScript and WebAssembly are both product projects in this repository; bytecode is
the payload form every artifact here carries, and beside it the JavaScript profile's format admits
an emitted-code section, for which that profile carries an **x86-64** encoder whose output its own
verifier answers for and this host arms and executes, and an **arm64** encoder that is
**emitting-only** and has run nowhere, its execution excluded by a missing maintenance path rather
than by a missing machine. **Existing is the weakest claim on this page**: none of the five claims
a runtime identifier, none has a retained collection on a claimed one, none is advertised or
packable, and [the support table](docs/support.md) sections 3a and 3b are what say so. **Naming an
axis is not travelling along it**, which is the whole reason this section states the count.

*(Corrected 2026-09-08. This paragraph read "**At this snapshot exactly one input profile and
exactly one output form exist.** JavaScript is implemented, and bytecode is the only payload
anything here verifies or executes. WebAssembly is a plan with no project; x86-64 is a plan with no
encoder; arm64 is a plan with no encoder", and closed by saying "The other three are named here so
that a reader finds the plan rather than a silence, and none of them is a capability". Three of the
four factual clauses are false against this checkout: `src/Broiler.VM.Profile.WebAssembly` is a
project listed in `Broiler.VM.slnx`, and the JavaScript profile's lowering carries encoders for
`x86-64-win64`, `x86-64-sysv` and `arm64-aapcs64`, of which the two x86-64 conventions emit machine
code this host arms and executes. The clause that survives is the one about arm64's exclusion, which
the correction below already rewrote once and which is restated above rather than dropped. **The
count was understated and the count is the sentence a reader quotes**, so it is corrected here in
the open; what has *not* changed is the half that matters more — existing is not supporting, and
[the support table](docs/support.md) sections 3a and 3b still carry no claimed runtime identifier
for any of the five. The paragraph above it was corrected on the same date and in the same
direction: it said machine code was a form VM-7 "plans to admit as an artifact payload", when the
JavaScript profile's format already carries an emitted-code section and a symbol section beside the
bytecode — which is also why this paragraph no longer says bytecode is "the only payload anything
here verifies or executes", a clause that would have carried the understatement forward under a new
sentence.)*

*(Corrected 2026-09-07. Both paragraphs above gave arm64 a publish-and-run obligation "that no
machine within reach of this component can currently discharge", and the second called it a plan
with "no machine this component has published and run on". Both halves were wrong and wrong in the
same direction — they made the exclusion a shortage of hardware, which a machine could end.
[The core roadmap](docs/roadmap.md) excludes arm64 execution rather than leaving it unscheduled, and
what would close the exclusion is a maintenance path this component can name, call and test rather
than a runner or a lane; and release gate 11 gives an **emitting-only** backend no publish-and-run
obligation at all, because such a backend "is named emitting-only and names none" of the runtime
identifiers. The correction is recorded rather than made silently because a reader who quoted the
old sentence would be waiting for a machine that changes nothing.)*

## Recording a review

A review is written in one place: the `// Broiler-Human:` line of the assurance annotation on the
declaration being read. Nothing else is filled in, and there is no checklist document to keep in
step with the code.

[`HUMAN_REVIEW.md`](HUMAN_REVIEW.md) is **generated** from those lines and from the per-file
headers, so it carries a row for every alias the tree names rather than one signature block, and it
records no branch or commit: each decision names the fingerprint of the declaration it was made
against, which says whether *this unit* changed rather than whether the tree did. The review lane
under `.github/workflows/` regenerates it on every pull request, and the publish lane refuses to
pack while any relevant unit is unresolved, any fingerprint is out of date, any annotation is
malformed or any generated artefact is stale.

## Component boundary

Broiler.VM owns profile selection, bounded artifact loading, the immutable verified-artifact
boundary, the common execution lifecycle, trusted resource-limit precedence, cancellation,
diagnostics, profile-neutral operation-result envelopes, the static profile catalog, the bounded
binary-reading primitives every profile needs, and the numbered core contract version that carries
them all. Bounded mediation of guest-initiated loads and of external suspension belongs to the
core; the language meaning of either belongs to the profile.

A profile owns its format, verifier, value/frame model, control flow, semantics, typed
result/fault payloads, imports, oracle, and conformance suite. The core imposes no opcode set, no
value ABI, and no language-specific result cases.

Redundancy between profiles is avoided by sharing **mechanism** — how bytes are read safely, how a
budget is charged — and never **semantics**. Values, frames, opcodes, and syntax trees are not
shared, and a new shared component is opened only through the extraction gate in
[section 8 of the roadmap](docs/roadmap.md).

## Relationship to Broiler.JS

`Broiler.JS` is a **legacy component** with its own roadmap, ledger, and consumers. Broiler.VM does
not depend on it, wrap it, or replace it on any schedule stated here, and no core gate may cite its
results as evidence. The JavaScript profile is expected to begin from a snapshot **copy** of that
component taken after its in-flight fix programme lands — a fork used as a base, with no dependency
edge in either direction. The conditions on that copy are recorded in
[section 9 of the roadmap](docs/roadmap.md).

## Roadmap

The architecture, milestones, evidence requirements, test matrix, release gates, and risks are in
[the Broiler.VM roadmap](docs/roadmap.md); current evidence is tracked separately in
[the authoritative status ledger](docs/roadmap.status.md).
