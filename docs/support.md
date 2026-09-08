# Broiler.VM support table

> **The core ships no language profile.** Broiler.VM is a semantics-neutral bytecode-VM core. It
> executes nothing on its own: every opcode, value, frame and verifier rule belongs to a profile,
> and no profile ships in any package listed here. A host that installs these three packages has
> installed a mechanism and no language.

**Core contract version: 1.** Versioned separately from any profile format, feature manifest,
package version and persisted-envelope schema version. Its amendment procedure is published in
[ADR 0003](adr/0003-core-contract-v1-and-amendments.md).

**Status: preview, unaccepted.** No milestone in this component has been accepted, because
`HUMAN_REVIEW.md` is unsigned and `PENDING`. Nothing here is a release, and the binding rule in
ADR 0001 stands: **no Broiler.VM package is published without a completed review naming the
reviewed commit.**

Roadmap section 16 makes an **untruthful support claim a stop condition**, not a defect to fix
later. Every row below therefore states what has been *demonstrated on retained evidence*, and
says so where nothing has been.

---

## 1. Packages

| Package | Version | What it is | Depends on |
|---|---|---|---|
| `Broiler.VM.Abstractions` | 0.1.0-preview.1 | The profile-neutral contracts: descriptors, results, diagnostics, budgets, the source-level profile contract | Nothing |
| `Broiler.VM.Binary` | 0.1.0-preview.1 | Bounded binary reading: checked readers, canonical-only LEB128, framing, allocation guards. No format, no schema, no semantics. **A format whose specification requires padded variable-length encodings to be accepted cannot use the variable-length readers or `TryReadDeclaredCount`**, and decodes its own integers over the byte-level members instead | Nothing |
| `Broiler.VM.Runtime` | 0.1.0-preview.1 | The catalog, the runtime and its lifecycle, resource authority, guest-load mediation, external suspension | `Broiler.VM.Abstractions`, `Broiler.VM.Binary` (assembly references; see below) |

**The three packages depend on nothing outside themselves.** `Broiler.VM.Runtime` declares the
other two and that is the whole of it: no third-party package, no other Broiler component, nothing
from nuget.org. Rule C2 asserts it against the retained `.nuspec` of every produced package, and
the pristine feed consumer is the same claim from the other side - that sample's only restore
source is a directory holding these three, with nuget.org unreachable, so a dependency on anything
else would fail its restore outright.

No fourth package exists. ADR 0001's budget section fixes the set at three and requires a dated
revision of that record before a fourth `PackageId` may appear; rule A6 asserts it.

---

## 2. What the core supports

| Capability | Support | Evidence |
|---|---|---|
| **Static profile registration** | Supported | A direct-factory catalog with no reflection and no name-based discovery. Rules B5 and A9; the catalog-drift baselines in `docs/compositions.md` |
| **The source-level profile contract** | Supported | Two application-local profiles and one package-only sample profile are written against it. Rule A11 forbids any project outside a composition root to reference a profile assembly, so the contract is the only route |
| **Verification separable from execution** | Supported, and required to stay so | One verification entry point in the whole core, asserted by rule V9. An artifact becomes a handle before anything instantiates it |
| **Immutable verified artifacts** | Supported | Caller bytes are decoded into a profile-bound handle; mutation, disposal and concurrent overwrite after verification change nothing |
| **Resource authority (15 dimensions)** | Supported | Host ceiling authoritative, profile may tighten, artifact may only request less; the intersection is computed before allocation and recorded on the handle |
| **Aggregate budgets across runtimes** | Supported | Shared rather than multiplied, under concurrency |
| **Guest-initiated loads** | Supported, bounded | Depth, fan-out, cumulative bytes and nested verifier work, charged to the requesting operation. A composition registering no provider refuses every request deterministically |
| **External suspension** | Supported, opt-in | Declared by the profile and enabled by the host; both gates required |
| **Cancellation and bounded disposal** | Supported | Cancellation reaches a step inside a profile; disposal drains in-flight steps under the host's own wall-clock bound |
| **Thread affinity** | **Partial** | `OperationThreadPinned` is enforced where the core can see a thread - on resume, the only place a second thread enters an existing operation. A profile that starts its own threads is invisible to the core. EX-89 |
| **Asynchronous instantiation** | **Not supported** at contract version 1 | Recorded as a decision, not an omission. No member returns a task; rule V8 asserts it |
| **Persisted envelope** | **Not supported** at contract version 1 | Admitted as contract by ADR 0010 decision 4 and implemented by no milestone. Release 1 exposes no envelope member; rule V10 asserts it. EX-25 |
| **Streaming or incremental verification** | **Not supported** at contract version 1 | The contract is whole-bytes-to-handle. Adding one is a numbered amendment, not a signature widening; rule V10 asserts it |
| **A binary plug-in ABI** | **Not supported, and not planned** | Extensibility is compile-time. Nothing here implies a loadable plug-in format |

---

## 3. Runtime identifiers

| RID | Publish and run | Status |
|---|---|---|
| `linux-x64` | JIT, trimmed self-contained, Native AOT | **Demonstrated.** Every collection in `docs/evidence/` was taken on it, publishing and running the fixtures host, both composition roots and the package-only sample with trim and AOT warnings as errors |
| `win-x64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed.** No retained collection exists on it. The CI lane publishes and runs every composition root here as Native AOT on a stock runner, which is what a claim would need to be *about* and is not a claim. EX-45. **The reason this row used to give is withdrawn**: the publish does not need a `vcvars64` environment, it needs `vswhere.exe` on `PATH`, which ADR 0001 records and the lane confirms — EX-42 |
| `linux-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, and declared for the *architecture* rather than for a consumer: nothing publishes it, and it is the only place this component compiles arm64 at all. **A green job on it is not Android coverage** — it shares an instruction set with `android-arm64` and nothing else, not the runtime, not the trimming configuration, not the head. EX-45 |
| `win-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, for the pair neither `win-x64` nor `linux-arm64` reaches: the Windows Native AOT toolchain targeting arm64. The limit on the row above applies here unchanged — this is not Android coverage either. EX-45 |
| `osx-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01** for the clang-into-Mach-O toolchain, which no other declared RID reaches: a different object format, a different linker, `dyld`, and an ad-hoc code signature Apple silicon requires before a binary will run at all. EX-45 |
| `osx-x64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, and with the weakest ground of the six: the toolchain is reached by the row above and the architecture three times over, so what it adds is the pair. Collectible only on the successor Intel image — `macos-13` is retired. EX-45 |
| `android-x64` | **Mono, no trimming, no AOT** — in the CI lane and one retained collection | **Not claimed. Declared 2026-09-02** on the rule's consumer limb rather than the grid limb: the consuming repository's Android head ships it, and it is a fourth runtime family rather than a fourth toolchain cell. Bundle `js-android-001` retains a collection taken on an **emulator**, which is not a device. **Nothing about it is evidence about Native AOT** |
| `android-arm64` | published, **not run** | **Not declared.** The head builds it and nothing executes it — an arm64 emulator on an x64 host is not usable and this component has no device. A RID whose evidence stops at a build is what publish-and-run refuses |
| `ios-arm64` | — | **Excluded, and not for want of effort.** It is a *device* RID: publishing needs an Apple signing identity, running needs a tethered device, and no hosted runner has either. Only a self-hosted Mac with a device attached could ever collect it |
| `iossimulator-arm64`, `iossimulator-x64` | — | **Excluded pending a head.** Reachable in principle on a macOS runner's simulator; this component has not written the iOS head that would publish them |
| *(none)* | — | **No RID is reserved as of 2026-09-01.** The category is kept because a reserved RID is one this table may never name, and a rule nobody can find is a rule nobody applies `osx-arm64` was attempted and passed in the CI lane until 2026-09-01, when the lane was brought back to the declared matrix — the ADR's revisions record the withdrawal, and that a reserved RID with a lane behind it is one edit away from being read as supported |
| `android-arm64`, `android-x64` | — | **Excluded.** The consuming repository's Android head is `net10.0-android36.0` on Mono with trimming off, so the exclusion's original reason — no evidence that ILCompiler Native AOT targets an Android RID — answers a question that head does not ask. The gap that would have to close first is narrower: this component has no Android-targeted project and no device or emulator harness, and *publish and run* on an Android RID means an application package and a device. EX-32 |

**One machine, one RID for every retained collection.** That is EX-45, and it is still the widest
limit on everything in this table — but its old wording said "no CI lane has ever run", and that is
no longer true. The lane in `.github/workflows/` runs on hosted runners and passes, publishing and
running every composition root as Native AOT on **the declared RIDs**, which since 2026-09-01 is
also every RID it attempts — two of them at first, then all six as ADR 0012's revisions of that day
widened the matrix to the full grid. Every one published and ran at the first attempt it was given a
runner for; one `win-arm64` attempt of three failed inside the SDK installer on the hosted image,
before any of this component was built, which is recorded in the workflow rather than smoothed over
here.

**Since 2026-09-03 that sentence is about the FULL lane and not about every run.** The lane was
split: `broiler-vm.yml` runs on every push and pull request over two of the six cells —
`linux-x64` and `linux-arm64`, one per architecture — and `broiler-vm-full.yml` runs all six with
the emulator on a release tag, a weekly schedule and a button. So "every declared RID published and
ran" is a statement about a release tag and a Monday, not about the last push. It changes nothing
in the table above, because that table is fed by retained collections and by no lane at all; it is
recorded here so that a reader does not take a green pull request as the six-cell answer.

**That moves no row above, and the reason is the one distinction this table rests on.** A support
claim is made on a *retained collection*: a bundle in `docs/evidence/`, collected deliberately by a
person, naming its machine, its SDK, its effective configuration and its raw outputs. The lane
collects none of that — its own header says so — so a green job is evidence that the component
builds and runs somewhere, and evidence of nothing a reader could check twice. **A workflow that
has not run is a plan; a workflow that has run is still not a bundle.**

What did change is the honesty of the gap. A reader was previously told that nothing had ever
published on `win-x64`; something has, repeatedly. That row stays unclaimed because no collection
exists on it, which is a narrower and truer reason than the one it carried.

**And one row left this table rather than moving up it.** `osx-arm64` had a row of its own here
while the lane published on it. ADR 0012 marks that RID reserved, and a reserved RID may never
appear in a support table — so the row is gone and the lane entry with it. Nothing was claimed and
nothing is now unclaimed that was not: what a reader loses is a row that implied this component was
going somewhere it has not decided to go.

**Four rows arrived by the opposite route, and the difference is the point.** `linux-arm64`,
`win-arm64`, `osx-arm64` and `osx-x64` are not in this table because a lane runs on them; they are
here because the matrix was widened by dated decisions that state what each row does and does not
stand for, and the lane follows the matrix rather than leading it. That is the order the withdrawal
above exists to restore — and `osx-arm64` is the same RID that left this table that morning,
readmitted in the afternoon by an argument rather than by a job.

**The six are a grid rather than a list**: three Native AOT toolchains — MSVC into PE-COFF, clang
into ELF, clang into Mach-O — by two architectures, every cell filled. A seventh RID cannot come
from that reasoning, only from a consumer, which is where the Android and iOS RIDs sit and why they
remain excluded: no targeted project and no harness that can *run* what is published.

---

## 3a. Artifact output forms

**What this section may be read as.** A statement of which artifact payload forms exist in this
component at all, and — for each form that exists — which runtime identifiers it has been published
**and run** on, on a retained collection.

**What it may not be read as, and each of these is a rule rather than a caution.** It may not be
read as a claim that a form exists because the core contract admits it; the contract's payload is
opaque and profile-owned, which means it admits forms nobody has written. It may not be read as a
claim about speed: this component measures its own overhead and never a language's, and **a release
may state that a native form exists and may not state what it is worth.** And it may not be read as
a claim that a form works on a runtime identifier merely because that identifier appears in section
3 — section 3's rows are about publishing and running *this component*, and a form is something a
profile emits.

**The core generates no machine code and learns no encoding.** Instruction selection, register
allocation and calling convention belong to the profile that owns them; what the core owns is
whether a composition may execute the result, which is declared in [the composition
register](compositions.md) — where, since 2026-09-07, the five JavaScript rows and the polyglot
root declare `x86-64`, six of the register's ten rows, and the other four (the two core fixture
compositions and the two WebAssembly roots) declare `none`, and where a rule now holds every one of
those cells against the tree in both directions *(corrected 2026-09-08: this read "the five
JavaScript rows declare `x86-64` and the four others declare `none`", a count that was right for
the hours between the column landing on 2026-09-07 and the tenth row arriving later the same day,
and the register itself has said "Four cells read `none` and six read an architecture" since.
Understating the set a declared permission covers is the same defect as overstating it, and this
table is the document least entitled to either)*. **That column records a permission and not a
run**, and a rule reading it makes the permission checkable and makes no run out of it; nothing in
it is a support claim. This section is where a run would be recorded, and it records none.

**Two forms exist beyond bytecode as of 2026-09-07, and the whole of what follows is the difference
between existing and being supported** *(this section said until that day that no such form existed
anywhere, which was true when it was written)*. **No evidence bundle has been retained for either
of them.** Every cell of the table below states what has been demonstrated **on retained evidence**,
which is the rule at the head of this document, and for both new rows the answer to that is
*nothing*: what exists is code in a working tree that was observed to run on one machine, and a
working tree is not a collection. A reader who takes anything below as a claim about a runtime
identifier has read past that sentence.

| Output form | Does it exist | Published and run | Deterministic refusal elsewhere |
|---|---|---|---|
| **Bytecode** | **Yes**, and it is the only artifact payload form anything in this checkout has ever verified or executed | Exactly the runtime identifiers section 3 records and no others. **This row adds no claim to any of them**: section 3's Status column is what says which are demonstrated, which are unclaimed and which are excluded, and it is unchanged by this section | Not applicable. There is no runtime identifier where a bytecode artifact is refused for want of a form |
| **x86-64 machine code** | **Yes, and it is emitted and executed.** The JavaScript profile's format carries an emitted-code section and a symbol section; its lowering carries encoders for both x86-64 calling conventions, named `x86-64-win64` and `x86-64-sysv`; its verifier answers for the payload structurally and, where an image carries the lowering, by recompiling the artifact's own bytecode and comparing the emitted bytes; and one type in that profile maps a page and arms it readable-and-executable. **The language it compiles is `broiler.javascript.numeric` and it is not JavaScript** — a program outside that manifest is refused at compile time by name *(corrected 2026-09-08: that last clause had one exception until this date and now has none. The front end admitted a **BigInt literal** and dropped its suffix, so `--numeric --native x86-64-win64` compiled `9007199254740993n` and emitted machine code returning `9007199254740992` — a plausible wrong integer produced by the armed native path rather than a refusal naming the construct. The parser now answers `2104:ConstructOutsideManifest` for the literal, which was checked by running that command line on this date. **The sentence is not being weakened, it is being dated**: a reader who took it on trust before today was told something this form did not do in one case, and a support table is the last document entitled to leave that unrecorded)* | **No runtime identifier is claimed, and none can be today.** The Windows convention was emitted and executed in a working tree on `win-x64`, answering what the interpreter answered for the same source; `win-x64` is **Not claimed** in section 3 for want of a retained collection, this row adds nothing to it, and a form cannot claim an identifier this component has not claimed for itself. **The System V convention has been emitted and executed nowhere**: on a Windows host it verifies and refuses to instantiate. **No bundle retains any of this**, and no cell here is a CI-lane result | **Yes, and it is deterministic and by name.** An artifact whose architecture and convention the running process is not refuses to instantiate, as a contract violation naming an unsatisfied host assumption, rather than falling back to the bytecode beside it in the same artifact. And a composition that declines the native surface refuses the artifact **at verification**, with an invalid-artifact reason, before any instruction of it is reachable |
| **arm64 machine code** | **Yes as an encoder, and it is EMITTING-ONLY.** A backend named `arm64-aapcs64` emits into the same sections and its bytes are pinned against known-good encodings by golden rows in a checks lane. **It has run nowhere**, and the reason is unchanged and is not a shortage of machines: the architecturally required instruction-cache maintenance sequence has no managed expression and no dependable library export | **None, and none is claimed. `emitting-only` is the word**, and it is in this cell rather than in a footnote because [release gate 11](roadmap.md#15-release-gates) and the native-execution milestone's exit gate require it here. Nothing has been published or run on any arm64 runtime identifier by this form, on any machine, at any time | **Yes.** An image asked to instantiate an arm64 artifact refuses by name, on every machine including an arm64 one, because no process this component runs in reports an architecture this build will arm for. The refusal is the same one the row above names and it is reachable from the command line |

**What has to happen before any cell above changes again, stated so that a filled cell is
recognisable as an event.** The native-execution milestone's exit gate requires this table to name,
per declared runtime identifier, which native backends have published **and run** — and to name the
deterministic refusal everywhere else. **The second half can now be written and the first half
cannot**, which is the exact inversion of what this paragraph said until 2026-09-07, when it said
neither half could be written because there was no backend and no refusal path. The refusal path
exists and is named above. The publish-and-run half needs a retained collection on a claimed runtime
identifier, and section 3 claims one runtime identifier, which is not the one this form has run on.
**A reader who wants to know whether native execution works should read the Does-it-exist column and
then read the Published-and-run column, and treat the second as the answer.**

**Two routes are already fixed for a native form and neither was decided.**
[`docs/mvp.md`](mvp.md) records them: arm64 is planned as an **emitting-only** backend — bytes
compared against a retained expectation, never bytes run, no runtime identifier claimed and no
figure attached, and **emitting-only** is the word [the roadmap](roadmap.md)'s release gate 11 and
the native-execution milestone's exit gate require this table to carry in the row itself rather
than in a footnote under it — and x86-32 is dropped from scope entirely, so `win-x86` is not a
runtime identifier of this component and this table will not gain a row for it. Both are recorded
there as routes taken **without a decision**, which means they may be reversed without anything
being corrected, and a reader planning against either should read that record rather than this one.

---

## 3b. Input profiles

**What this section may be read as.** Whether a language profile exists as source in this
repository, whether it is advertised, and whether it is packable. All three are observable facts
about the checkout.

**What it may not be read as, and this is the load-bearing half.** It may not be read as a status
claim about any profile. **This table reads no profile ledger and carries no profile result.** A
profile's status belongs to that profile's own ledger — core ledger update rule 6, and the profile
family's own bidirectional counterpart — so a conformance total, a benchmark score, a differential
transcript or a milestone state collected by a profile is evidence about that profile, is recorded
in that profile's own bundles, and does not appear here at any strength. **Sharing a repository is
not sharing a ledger.** And the claim at the head of this document is unchanged by anything below
it: **the core ships no language profile**, and no profile ships in any package section 1 lists.

| Input profile | In this repository | Advertised | Packable | Where its state is recorded |
|---|---|---|---|---|
| **JavaScript** | **Yes.** A product project family under `src/Broiler.VM.Profile.JavaScript`, composed by demonstration composition roots that [the composition register](compositions.md) lists by name | **No.** The register's advertised set is empty, every root composing this profile is a demonstration, and advertisement is a release decision ADR 0012 owns rather than an edit to a column | **No.** Every project in the family declares no package identity and carries the literal non-packable property the family rule reads out of the project file. The packable set stays exactly the three packages of section 1 | Its own ledger at `src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md`, under its own three-mark legend. **This table does not read it, and a green result there moves no row here** |
| **WebAssembly** | **Yes** *(corrected 2026-09-07; this cell read "No. Planned, and no code exists" and listed a documentation directory with no project file, no source file, no descriptor, no composition root, no rule-register group, no evidence tree and no entry in either solution — every clause of which was true when written and none of which is true now)*. A product project under `src/Broiler.VM.Profile.WebAssembly` carrying a descriptor, a decoder for the binary format, a validator, a store and an interpreter, composed by two never-advertised composition roots [the composition register](compositions.md) lists by name | **No.** The register's advertised set is empty, both roots composing this profile are demonstrations, and advertisement is a release decision ADR 0012 owns rather than an edit to a column | **No.** The project declares no package identity. The packable set stays exactly the three packages of section 1 | Its own ledger at `src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md`, which records which of its milestones own code and states in its own words that **none is accepted, none has a retained bundle, and code that exists and runs is not retained evidence**. **This table does not read that ledger, and a green result there moves no row here** |

**Swept on 2026-09-08 for a claim about the JavaScript profile's conformance run, and there is
none to correct — which is the rule above working rather than an omission.** On that date the wide
manifest's front end was given back a refusal it had lost: a BigInt literal, which it had been
admitting and evaluating as a Number, is refused at compile time naming the construct. The
consequence over the suite runs the other way from the repair — variants that had passed or failed
over the BigInt subtree now meet a refusal instead, so the whole-suite `passed` total falls and the
wide run's `unsupported` column stops being empty. **Every record anywhere in this repository
saying the wide run names no unsupported family, or that its unsupported column is empty, is false
from this date**, and the retained bundle `jsw-10-001` states figures for a run taken before the
change. **None of those records is in this document**, because this document reads no profile
ledger and carries no profile result: the sweep found no conformance total, no `unsupported`
column and no pass count here to correct, and section 3b's own rule — a profile's status belongs
to that profile's ledger — is why there was nothing to find. It is recorded here anyway so that a
reader who knows what moved that day, and comes here expecting a correction, is told the answer
rather than left to conclude that the sweep was not made. **This section states no figure, old or
new, and it would state none if they were known.**

*(Corrected 2026-09-08, later the same day, and the correction is of a promissory note rather than
of a wrong fact. The paragraph above ended "**A fresh whole-suite run is being taken and its totals
are not known**, so this section states no figure, old or new, and it would state none if they were
known." The run has since been taken and its totals are known, so the first half of that sentence
is a note whose answer has arrived and it is withdrawn; the second half is the rule and it stands
word for word, which is why this correction adds a date and a name and still adds no number. **A
record that leaves a promissory note standing after the answer arrives is the stale-summary failure
update rule 1 exists against**, and a support table is the last document entitled to leave one
there.

**The run this sweep describes is named rather than summarised**, so that a reader can tell which
run any figure they meet elsewhere belongs to: the whole `tc39/test262` suite driven under
`broiler.javascript.wide` by `python3 eng/run-test262.py` against the pinned suite revision
`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`, taken by the orchestrator on
**2026-09-08** on **`win-x64`**, whole rather than sampled and pinned rather than floating, and
taken **in a working tree rather than into an evidence tree**. **It is therefore retainable and
not retained**: no bundle has collected it, nothing about it has been read by a person, and no row
of any ledger in this repository reaches `Accepted` on it. Its counts belong to
`src/Broiler.VM.Profile.JavaScript/docs` under core ledger update rule 6, and a reader who wants
them opens that ledger or runs the script and reads their own — which is the same instruction this
document gave before the run existed and gives unchanged now that it does.

**The one thing the sweep can state here is a name and not a count.** The wide manifest's
whole-suite `unsupported` column is **no longer empty**, and every variant in it is the same
construct rather than a family list — the BigInt literal, meeting the refusal that names it — so a
record anywhere in this repository saying the wide run names no unsupported family, or that its
`unsupported` column is empty, is false from 2026-09-08 whether or not it states a figure. The
retained bundle `jsw-10-001` continues to state figures for a run taken **before** the change, and
that is not a defect in the bundle: a bundle describes the run it retains and nothing later.)*

**A plan naming a profile is not a profile, and this section exists so that a reader of the plan does
not infer one.** The core roadmap already names both intended first profiles and keeps a list of what
each expects to require of the contract. **That sentence used to end by saying one of those plans
stands over an empty directory, and it no longer does** — the directory has a project in it, which
changes the In-this-repository column and changes nothing in the three columns beside it. **A profile
existing is the weakest of the four claims this table makes**, and it is the only one either profile
has earned: neither is advertised, neither is packable, and what each has demonstrated is its own
ledger's to say and not this table's.

**No composition composes both, and none is coming from this table.** The product that would need
both at once is a browser; [the composition register](compositions.md) refuses to carry a row for
that composition, because the rule binding the register to the checkout fails on a row naming a root
that does not exist. Until such a component exists, the closure, the runtime-identifier matrix and
the Native AOT evidence for it belong to nobody, which the register says in its own words.

**Why an unadvertised, unaccepted component is nevertheless being advanced.**
[`docs/mvp.md`](mvp.md) records the instruction, dated 2026-09-07, and fixes exactly what it defers:
the approval of boundary records, human review, evidence-bundle collection and milestone acceptance,
and the co-signing the amendment procedure needs. What that instruction does **not** defer is the
automated gates, the status vocabulary, the stop condition on an untruthful support claim, the
non-advertisement of every composition and the three-package pack set, and the prohibition on
publishing. **An MVP here buys the right to build and merge unreviewed work; it buys nothing
whatever about what may be claimed.**
Every row of this table is a claim, so no row moves because work was done — a row moves when a
retained collection shows it, and not before.

---

## 4. Deterministic exclusions

Behaviour that is bounded, deliberate and will not change without an amendment.

| | Exclusion |
|---|---|
| **A profile is never discovered** | Registration is a direct factory call in a composition root. There is no probing path, no assembly scan and no configuration file that can add one |
| **An unknown format version is refused, never guessed** | Interpreting old bytes under new semantics is prohibited. The refusal is deterministic and identical on repetition |
| **A composition with no artifact provider refuses every guest load** | That is the content policy expressed as a contract outcome, not an error condition |
| **An artifact cannot loosen a host ceiling** | It may request less. The effective policy is the intersection and is computed before any allocation proportional to a declared count |
| **Diagnostics cannot carry free text** | Every member of the record is an enum, a number, or one of four identity types. A host capability that throws an exception carrying a secret produces a failure that carries none of it - rule V11 |
| **A profile cannot reach undeclared CLR surface** | No profile-facing contract takes or returns `object`, a `Type`, a delegate, a reflection type, an assembly load context or a raw pointer - rule V12 |
| **The core reaches no legacy Broiler component** | An architecture-tested rule, not a convention. Rules A1, A2 and D1 |

---

## 5. Measured overhead

Published with its method in [the baseline register](baselines.md), and bounded by what it is:
figures from **one four-processor `linux-x64` machine**, of the **core's own overhead** around a
fixture profile whose executor is a toy.

**No language performance claim follows from any of it.** The core publishes only what it costs a
profile; what a language costs is that language's own.

---

## 6. Operations

| Question | Answer |
|---|---|
| **Rollback** | Exercised, not described. A consumer restores one package set from a feed, then is rolled back to the previous one and still restores, builds and runs - and prints the informational version it actually loaded, so the transcript shows which set answered. `feed-consumer.log` in the current bundle |
| **Format-version rejection** | Deterministic and repeatable; asserted from a package consumer's position by the sample |
| **Envelope recovery** | Not applicable at contract version 1: there is no persisted envelope to recover |
| **Vulnerability response** | The owner named in [ADR 0012](adr/0012-security-ownership-and-support-matrix.md) holds all six roles, security included. There is no separate security contact and no published disclosure timeline, which is a gap this table names rather than hides |
| **Recertification** | Required when the SDK or runtime, core contract version, package graph, host capability surface, Native AOT settings, RID matrix, cache identity, resource defaults, or representative workload changes. Each evidence bundle carries its own triggers in section 8 |
| **Support lifetime** | None stated. A preview with no accepted milestone has no support commitment, and inventing one here would be the untruthful claim section 16 forbids |

---

## 7. What this table does not say

- It does not say the component is production-ready. No milestone is accepted.
- It does not claim any platform beyond `linux-x64`.
- It does not claim a security review by anyone other than the author, who holds every role.
- It does not claim performance for any language, and the core has no language to claim it for.
- It does not say that a native artifact form is **supported**. Two now exist — section 3a says which,
  per form, rather than leaving it to be inferred — and **neither has a retained collection, a claimed
  runtime identifier or a reviewed line**, so nothing about either is a support claim. *(This bullet
  said until 2026-09-07 that no native artifact form existed on any architecture. It did, from the
  commit that emitted one.)*
- It does not say that the arm64 backend runs. It is **emitting-only**, in that word, and it has run
  nowhere.
- It does not say that a WebAssembly profile is supported. One exists as of 2026-09-07; nothing about
  it is claimed here, and its own ledger is where its state is. *(This bullet said that a plan for one
  existed and the profile did not.)*
- It does not claim any language performance — the bullet above says so, and **release gate 8 fails
  a core record that does**. A profile's conformance totals and benchmark scores belong to that
  profile's own ledger and bundles under core update rule 6, and **no automated rule checks a core
  record for one**: that half is a rule a reader enforces and not a gate. It is said here rather
  than left to be discovered from a green suite — which is the term [the composition
  register](compositions.md) used for its own native-execution column while no rule read it, and a
  rule reads that one from 2026-09-07 while this half of release gate 8 still has none.
