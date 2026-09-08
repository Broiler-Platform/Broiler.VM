# Broiler.VM.Profile.WebAssembly roadmap status

**Last updated:** 2026-09-07

**This file is part of the [WebAssembly profile roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split).

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[WebAssembly profile roadmap](roadmap.md). The roadmap defines planned work and objective exit
gates; this ledger records whether those gates have accepted evidence. Where the core changed, a
sibling's dated finding settled something, or the plan replaced its own earlier reading, the plan
carries the new reading and [the corrections and rejections](roadmap.corrections.md) carry what it
replaced — **that file records no status and advances nothing here**.

**At this snapshot, five milestones own code and none is accepted** *(updated 2026-09-07)*.
The component has a source tree, projects in the solution, a descriptor, a decoder, a validator, a
store and an interpreter, two never-advertised composition roots, its own group in the component's
rule register, and an assurance record. It has no pinned specification revision, no pinned suite
revision, no evidence bundle, and no human review of anything. WA-0, WA-1, WA-3 and WA-5 move to
`In progress` on the strength of milestone-owned code that exists and runs; WA-2 owns code too and
stays `Blocked`, because its blocker binds acceptance and publication rather than authorship.
**No milestone is complete because its design appears in the roadmap, and none is complete because
its code compiles and answers**, and nothing in this component may be described as validated,
accepted, supported, or published.

**An MVP programme now exists, and it changes what is scheduled rather than what is true**
*(added 2026-09-07)*. By an instruction of the repository owner dated 2026-09-07, recorded at
[the MVP programme record](../../../docs/mvp.md), this profile is to be brought up as an MVP, and
that record fixes exactly four deferrals: the approval of boundary records, human review,
evidence-bundle collection and milestone acceptance, and the co-signing step of the core contract
amendment procedure — and fixes five things it does **not** defer: the automated gates, the status
vocabulary, the stop condition on an untruthful support claim, the non-advertisement of every
composition and the three-package pack set, and the prohibition on publishing. **Four deferred and
five not deferred, counted exactly.** **Every row of section 2 was `Not started` when this
paragraph was written, and this paragraph is not the thing that moved any of them** — an instruction
to plan is planning text, and planning text does not change a state; what has moved rows since is
code that exists and runs, recorded row by row below *(amended 2026-09-07)*. What the programme adds to this ledger is a named path through the
milestones, recorded in section 2 below with the milestones it targets and, at greater length, the
ones it rules out; and the two findings that path meets in its first commits, recorded there as
observed repository state. **The MVP buys the right to build and merge unreviewed work, which
update rule 8 already granted. It buys nothing about what may be claimed**, so every sentence of
*What this component is not claiming* stands unamended, the automated gates are unrelaxed, and an
exit gate is not made easier by being attempted under an MVP.

**Placement is fixed by the core's topology record**, which rules that a language profile is a set
of product projects in the `Broiler.VM` component rather than a component of its own, and names
`src/Broiler.VM.Profile.WebAssembly/` — where these documents sit. The assurance system, the rule
register and the licence and notice files are therefore the host component's, adopted rather than
duplicated, and what this profile stands up of its own is its evidence-bundle contract, its
collection script, and its group in the rule register. WA-0 owns everything about this profile that
the topology does not fix *(corrected: WAC-08)*.

**This component has no seed.** Nothing is copied, no snapshot is taken, and no fork exists. Every
line will be written here. That removes a whole family of blockers the other intended first
profile carries, and it removes a head start as well; roadmap
[section 4](roadmap.md#4-no-seed-what-greenfield-costs-and-what-it-buys) states both directions
and this ledger holds the consequence — an origin distribution that is anything other than uniform
is a finding here rather than an expected fact.

---

## 1. Reading this ledger

Four categories must remain distinct, and conflating any two of them is how an unfounded claim gets
recorded:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation evidence and not validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout — for instance that
  this component has no pinned specification revision. It can explain a status; it cannot satisfy a
  future implementation, contract, conformance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable bundle that identifies the exact sources and
  gate, records the executed commands and environment, retains their outputs, and demonstrates every
  part of the objective exit gate. Only accepted evidence may advance a milestone to `Accepted`.
- **Ingested material** is third-party content this component brings into its tree — the conformance
  test suite and the archived specification document. It carries **no status of its own**. A suite
  file is an input, never evidence; the retained *run* over it is the evidence, and the distinction
  is what stops a large corpus in the tree from reading as a large amount of work done.

**The second bullet's example was corrected on 2026-09-08, and the correction is recorded here
rather than made silently.** It read "for instance that this component contains no project file",
which was true when the glossary was written and stopped being true on 2026-09-07, when this
component acquired `Broiler.VM.Profile.WebAssembly.csproj`, twenty-one source files and two
composition roots — all of which the headline above already records. **A glossary whose worked
example a reader is invited to go and verify must not be the one line in the ledger that is
false**, and understating what stands in this directory is the same defect as overstating it. The
replacement example is the one this ledger asserts twice elsewhere — in the headline above and in
section 3's open row, *The specification revision has not been retrieved, hashed, or archived* — so
it cannot go stale ahead of the rows that would have to move with it. It is deliberately not "no
retained evidence bundle": `docs/evidence/wa-0-001/` exists, and why it is not a bundle in section
4's sense already needs a paragraph of its own.

**Work in other components is not this component's evidence.** In particular, no conformance result,
benchmark, measurement, review decision, or Native AOT sample produced by the Broiler.VM core or by
any other language profile establishes anything here, and no gate in this ledger may cite one. That
rule is what makes a number in this file mean something.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, its holder, and its unblock condition must be recorded. **Lack of scheduling is not a blocker; an unaccepted upstream contract is.** |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and an owner and reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and the decision record must be linked; evidence history is retained. |

---

## 2. Current milestone status

The leading column is an **evidence verdict** — the author's mark about what a row's retained
evidence shows. It is not a reviewer's finding and not a change of state. Every row below is
`[NONE]`, because no row has retained evidence of any kind. **Code that exists and runs is not
retained evidence**: five rows own code and none of them owns a bundle, which is precisely the
distinction the leading column measures.

**This table is this document family's mark legend, and rule H1 reads it.** The vocabulary is
closed and has three members, and it is the same vocabulary every profile ledger in this component
uses; the component's own nine-member legend in `HUMAN_REVIEW.md` is, in rule H1's own words, a
different vocabulary about a different subject — its four evidence verdicts are stated about a
piece of evidence and its five review verdicts about a gate clause in a bundle, where these three
are stated about a whole milestone row — and a mark from it appearing here is a rule violation.
Only one of the three is in use today, which is what a ledger with no retained evidence looks
like — publishing all three is what lets the other two be read when a row first earns one.

| Mark | Meaning |
|---|---|
| `[NONE]` | The row has retained evidence of no kind. |
| `[PARTIAL]` | The row has a retained bundle that demonstrates some of its exit gate, with every unmet clause named in the bundle's own exclusions. A `[PARTIAL]` row is not a qualified pass. |
| `[FULL]` | The row's bundle demonstrates every exit-gate clause. It is still not `Accepted`: acceptance additionally needs an owner and a reviewer decision. |

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [NONE] | **WA-0 — boundary, identity, assurance floor** | **In progress** | Milestone-owned work exists and no gate has been accepted. The profile project, the two never-advertised composition roots, the entry in the frozen project graph, this profile's own group in the component's rule register with a witness and a negative control for each rule, the family public-API baseline, the assurance annotations on every relevant unit, and the fifteen hard maxima and fifteen defaults with the three guest-load defaults written as large finite numbers all exist in the checkout. **No evidence bundle is retained and no gate clause is demonstrated**: nothing has been published or run on any runtime identifier, under trimming or under Native AOT, and the exit gate asks for exactly that. | Publish and run the two composition roots on a claimed runtime identifier under JIT, trimmed self-contained and Native AOT with trim and AOT warnings treated as errors, read the closure off the published output rather than asserting it, and retain the three tables as a bundle. Until that exists this row cannot move past `In progress`, and the licence obligation still has no owner and no co-signer recorded. |
| [NONE] | **WA-1 — the whole contract loop on a slice module** | **In progress** | Milestone-owned work exists and no gate has been accepted. The full-arity descriptor, the seven core-facing types, the slice manifest identity, format version 1 as a bare module, this profile's own variable-length integer layer, the binary corpus encoder in a harness root and the execution-only composition root all exist; **the contract loop closes end to end** — a module is cataloged, verified, instantiated and invoked, and the harness root prints what each step answered. The entry-point encoding decision is taken and written down beside the code that implements it: length-prefixed, so an export name carrying the encoding's own separators resolves, with float literals as hexadecimal bit patterns so they round-trip exactly. | Retain the run as a bundle rather than as a console transcript, then produce each of the five verifier outcomes and each of the five execution-step kinds from a named case in that bundle, on every claimed runtime identifier under all three publish modes. The operand-stack bound is computed at validation and stored on the verified state, which is one gate clause held; the rest are not. |
| [NONE] | **WA-2 — the decoder, the integer decision, the malformed corpus** | **Blocked** (the blocker binds acceptance and publication; milestone-owned work has begun and is recorded here) | Milestone-owned work exists and no gate has been accepted. The decoder covers the whole binary grammar this format version admits, with the section-order table written as a table rather than as an identifier comparison, its own signed and unsigned variable-length readers, the bound-before-use ordering re-derived, strict UTF-8 name validation, and custom sections read past. A malformed and invalid corpus lives in the harness root and every entry reproduces its recorded triple when that root is run. **What the exit gate asks for and does not have**: the corpus is not retained with a hash per entry, it is not replayed under three publish modes, the three tables are not compared, the specification revision is not pinned, and the correction this milestone owes the core's metering-split record has not been filed. | Pin the specification revision or record a named exclusion; retain the corpus with its hashes; replay it under JIT, trimmed and Native AOT; and file or confirm the metering-split correction with the core's architecture owner. **Under the MVP programme the named dependency blocks this row's acceptance and this component's publication, and does not block writing the code** ([WAC-29](roadmap.corrections.md#wac-29)); the code is written and the row stays where it is. |
| [NONE] | **WA-3 — validation and the diagnostic registry** | **In progress** | Milestone-owned work exists and no gate has been accepted. Validation is the specification's single-pass algorithm over a value stack and a control stack with polymorphic unreachable code; decoding completes before validation begins at module granularity; implementation-limit refusals answer as resource exhaustion naming a dimension and a scope rather than as an invalid artifact; and every rejection carries a stable code and a byte position. **The registry is not published**: the codes exist in the source as a closed enumeration and there is no versioned registry document bound in both directions, so the gate clause asking for one is unmet. The nesting corpus at and beyond the structural-depth ceiling is not written, and no case yet fails when the two phases are fused. | Publish the versioned diagnostic-code registry and bind it in both directions; write the malformed-and-invalid case that fails when the phases are fused at module granularity; write the nesting corpus at and one level beyond the ceiling and run it under Native AOT on every claimed runtime identifier. |
| [NONE] | **WA-4 — the oracle** | **Not started** | None. No suite pin, no script reader, no harness, no self-check fixture. | After WA-3, and in parallel with WA-5. This is the milestone whose value is lost by serialising it: the malformed and invalid families can be scored before any interpreter exists, and that is the main structural advantage this profile has. |
| [NONE] | **WA-5 — value model, store, interpreter** | **In progress** | Milestone-owned work exists and no gate has been accepted. **The nine-row value, store and frame decision is taken and written down before the interpreter's first line**, each row with the alternative not taken: an untyped operand stack because validation already proved the types, a sixteen-byte slot reserving the vector width now, heap-allocated frames because a frame model on the CLR stack cannot later be moved to the heap without rewriting the interpreter, arguments popped from the caller's stack into the callee's locals, traps as a return code threaded through the dispatch loop rather than as a CLR exception, and one poll per the declared uncharged-work bound placed before the charge that would cross it rather than after a fixed instruction count. The interpreter runs the numeric surface for all four types, locals, globals, one linear memory with its loads, stores, size and growth, structured control flow with all four branch forms, direct calls and indirect calls; the store, its memories and its tables are allocated, charged and reported retained; instantiation evaluates global initialisers, applies element and data segments in order with each segment bounds-checked whole before any of it is written, and runs the start function. **What the exit gate asks for and does not have**: no evidence bundle, no call-depth default derived from a retained per-runtime-identifier frame-cost measurement, no memory-growth proportionality fixture with a flat-charge negative control, no structural scan proving that no mutable state is reachable from a handle, and one member of the closed trap list is declared and unreachable — the earlier specification revision's name for an out-of-bounds indirect call, which this build reports under the current revision's name. | Measure the native cost of one interpreter frame per claimed runtime identifier and derive the call-depth default from it; write the proportionality fixture and its unsimplified control; write the handle-immutability scan; and retain a bundle. The guest-observable growth refusal exists and is exercised, against **this profile's own** page ceiling — which is the route recorded as taken without a decision, and not a gate clause held. |
| [NONE] | **WA-6 — linking, host imports, the store decision** | **Not started** | None. The store reading of roadmap [section 11](roadmap.md#11-the-store-instances-and-linking) is **open**, with three candidates and one already rejected. | Open the store decision now — it needs no code either — and cost the naming channel for the runtime-scoped reading, because that is the part with no contract member behind it. |
| [NONE] | **WA-7 — `core1` complete and the embedding seam** | **Not started** | None. No manifest is minted, no seam exists. | After WA-6. |
| [NONE] | **WA-8 — the second standardised group and the vector family** | **Not started** | None. No manifest is minted, and roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s allocation table is the authority for which milestone mints which *(corrected 2026-09-07: this cell read "No manifest beyond the slice is planned to exist before this point", which already contradicted that table's own earliest-milestone column — `broiler.webassembly.core1` opens at WA-6 — and contradicts it further now that `broiler.webassembly.numeric1` is allocated to WA-5. It is recorded as a correction rather than silently rewritten, because a cell that disagrees with the table it summarises is exactly the failure this ledger exists to prevent, and **a reader who skips the table quotes the cell**)*. | After WA-7. This is the first point at which a second validator exists to compare, so this milestone supplies **this profile's half** of the extraction-gate comparison of roadmap [section 25](roadmap.gates.md#25-risks-and-stop-conditions) — file paths, source revision, correspondence table — and records that it supplied it, or records that the first condition is unsatisfied. **It records no verdict**: that is the core architecture owner's and can only be filed in the core's own set. |
| [NONE] | **WA-9 — adversarial input, aggregate budgets, soak** | **Not started** | None. No fuzz target, no soak host, no aggregate-budget exercise. | After WA-8, though the malformed corpus grows from WA-1 onward rather than starting here. |
| [NONE] | **WA-10 — baselines, packaging, support table, release gate** | **Not started** | None. No measurement lane, no baseline register, no package, no support table, no human review decision on anything. | After WA-9, and after a named human has read every relevant unit — which is the largest single-owner task in the programme and must be scheduled, not assumed. |

### What now exists, and what each part of it is not

**Recorded as observed repository state in section 1's sense** — a reviewable fact about the current
checkout, able to explain a status and unable to satisfy a gate *(added 2026-09-07)*.

The component now holds a profile assembly whose Broiler-owned reference set is the two core
assemblies and nothing else; two composition roots under `src/compositions/`, neither advertised and
neither packable, one of them the execution-only root and one the harness that holds the binary
encoder; and, inside the profile, a decoder over the binary format, a validator, a value slot, a
store with memories, tables and globals, an interpreter, and the payload projections a caller reads
a result or a trap through. **What the harness root demonstrates when it is run is the whole
lifecycle** — catalog, verify, instantiate, invoke — over modules it assembles itself: the four
numeric types and their conversions, locals and globals, one linear memory with its loads, stores,
size and growth, `block`, `loop`, `if`/`else`, `br`, `br_if` and `br_table`, direct calls, recursion,
indirect calls through a table filled by an element segment, a start function, and the trap list.
Its output is a console transcript and **not** a retained bundle, and this ledger states no figure
from it.

**Three things about that surface are worth stating in the direction that costs.** The interpreter's
frames are heap-allocated, so guest call depth never grows the CLR stack and the only thing bounding
recursion is the call-depth charge — which means an exhausted call depth is a resource exhaustion
naming a dimension, and a process that stopped would be a defect rather than a limit. `Resume`
answers the named invalid-state refusal because nothing here parks, and `Unwind` releases a store
and runs no guest code — but no path in this build mints a continuation, so that release arm is
written and unreached. And one member of the closed trap list, the earlier specification revision's
name for an out-of-bounds indirect call, is declared and never raised, because this build reports
that case under the current revision's name.

**One directory in this component is named like a bundle and is not one, and a reader meets it before
they meet this sentence otherwise** *(added 2026-09-07)*.
`docs/evidence/wa-0-001/` holds four files: the catalog table each of the two composition roots
printed, and a closure report over a framework-dependent publish of each. Those four are what
[the composition register](../../../docs/compositions.md)'s Evidence column points at and what the
composition rules read; **they are not a bundle in [section 4](#4-required-evidence-bundle)'s sense**
— no identity, no source revision, no procedure, no negative controls — so every sentence in this
ledger saying no bundle is retained stands unchanged. Two further facts belong beside them. The
closure files say in their own headers that the trimmed and Native AOT modes were withheld because a
build with no decoder drops `Broiler.VM.Binary` and retaining that would record a true fact about a
shell as a claim about the composition; **a decoder has since landed and the four files have not been
collected again**, so what they retain describes the shell they were taken from. And they were taken
on `win-x64`, which is not a claimed runtime identifier of this repository.

**The two composition roots declare `none` in the register's native-execution column and that is now
a checked fact rather than an inherited one** *(added 2026-09-07)*. This profile's Broiler-owned
reference set is the two core assemblies and nothing else, and neither those two nor this profile's
own sources name any of the platform's reserve, protect or map entry points. **`none` there is a claim
of incapability and not of restraint**, which is what that column requires, and it is worth stating in
this ledger because the sibling profile's five rows changed to an architecture on the same day.

**The memory-growth route recorded above as taken without a decision is now code.** Growth is gated
first on this profile's own declared page ceiling, which is not a core budget and refuses nothing on
the meter, so the specification's minus-one answer is produced, the module observes it, the
operation completes normally and no allowance was spent — and the harness root exercises exactly
that. **A refusal caused by a CORE budget is still not guest-observable**: the charge latches
exhaustion and the core rewrites the completed step, so the module never runs the instruction after
the growth. That remains a deviation from what the specification says the growth instruction
answers, it is written into the code that implements it, and **publishing it in a support table is
WA-10's release decision and not WA-5's** ([WAC-16](roadmap.corrections.md#wac-16),
[WAC-28](roadmap.corrections.md#wac-28)). Nothing here publishes it.

### What this component is not claiming

Stated positively, because a table of empty rows invites a reader to fill them in:

- **No WebAssembly is supported.** One feature manifest identity is allocated in the descriptor
  and nothing has scored it: the specification's own conformance suite is not pinned, no harness
  reads it, and a run against modules this component wrote is not a conformance result. A
  specification version name would not be a conformance claim either.
- **The admitted surface is wider than the manifest that names it, and that is a defect this ledger
  records rather than a scope note.** Roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
  defines `broiler.webassembly.slice` as one type, one function, one export, integer arithmetic,
  local access and structured control flow — no memory, no table, no global and no float. The
  decoder, the validator and the interpreter admit more than that under the same manifest identity,
  so a module declaring the slice manifest and using a float is accepted here where section 6 says
  it must be refused at validation. **A manifest is refused and not degraded**, and per-manifest
  surface restriction is not implemented; the milestone that mints a second manifest owns closing
  it.
- **No composition is advertised**, none is packable, and no runtime identifier is claimed.
- **No conformance result exists.** Neither the specification nor the suite is pinned, and the
  harness is not built. No family total exists, and no aggregate percentage will ever be published
  in place of one.
- **No measurement exists**, and no figure from any other component or engine stands in for one.
- **Nothing is reviewed.** No human has read anything here.
- **The deterministic-profile position is a plan, not a demonstration.** Roadmap section 6 states
  that this component implements `DET`; no fixture asserts it, because no fixture exists.
- **The JavaScript API for WebAssembly is not provided and is not planned here.** Roadmap section 17
  prices the boundary and names it as belonging to whichever component composes two profiles. A
  reader who sees this profile in a browser image must not infer that a page can call
  `WebAssembly.instantiate`.

### The MVP path through the milestones

**The MVP programme names a path through these eleven rows, and naming it moves none of them**
*(added 2026-09-07)*. Every row above was `Not started` when this paragraph was written; a row
moves off `Not started` when code lands, and reaches `Accepted` only when a bundle is retained and a
human has decided on it. What a path fixes is which gates will be **attempted**, in what order, and
— the half a reader otherwise fills in from silence — which will not be. An exit gate is not relaxed
by an MVP. What an MVP changes is which gates are attempted, never what one demands.

**The path is WA-0, then WA-1, then WA-2, then WA-3, then WA-5, and it stops there.** It is WA-1's
shape widened to WA-5's surface: one profile that decodes, validates, instantiates and executes a
single-module payload over the four numeric types, locals, globals, one linear memory with its
loads and stores, structured control flow, `call`, `call_indirect`, and the closed trap list
[section 12](roadmap.md#12-traps-exhaustion-and-why-neither-is-a-process-failure) fixes. The order
is the delivery order's own and the MVP reorders nothing: WA-0 lands no product code, WA-1 closes
the contract loop on the slice manifest, WA-2 completes the decoder, WA-3 completes validation and
publishes the diagnostic registry, and WA-5 takes the value and frame decision before its first
interpreter line and then writes the interpreter.

**WA-4 is not on the path, and it is not deferred either — it is unschedulable.** Its first input
is a conformance suite revision nobody has retrieved, which section 3 records as an unopened
dependency rather than as a blocker, and retrieving it is a human action. **That is a cost the MVP
pays and not a saving it makes**, and it is worth stating in the direction that hurts: the delivery
order calls the WA-4/WA-5 fork the main structural advantage this profile has over a language with
no external oracle, because the malformed and invalid families can be scored before any interpreter
exists. An MVP that does not take that fork grades its verifier against its own corpus alone, and a
corpus this component wrote cannot find a rejection this component never thought of.

**What the MVP does not deliver. Each is a rule, not a scheduling note, and each stays true of the
MVP however long the MVP runs:**

- **No imports and no linker**, so no module resolves an import, no export of one module reaches
  another, no host capability is bound, and no `assert_unlinkable` case has anything to run
  against. The store reading of roadmap
  [section 11](roadmap.md#11-the-store-instances-and-linking) stays open and WA-6 still owns it;
  **an MVP that shipped a linker would be taking that decision by writing one**, which is the
  failure mode section 11 names.
- ~~**No start function, no element segments, and no data segments.**~~ *(corrected 2026-09-07:
  this bullet was written before the interpreter existed and is now false about the code. The start
  function runs, element and data segments are applied in order, and each segment is bounds-checked
  whole before any of it is written. It is recorded as a correction rather than deleted, because a
  bullet that said what the MVP would not do and was then done by it is exactly the kind of drift
  this ledger exists to catch.)* **What is still unexercised is the half of the per-segment rule
  that needs an imported memory**: a segment already applied stays applied when a later one is
  refused, and with no linker and no imported memory no instance is published and nothing outside
  holds the bytes the earlier segment wrote, so
  [section 13](roadmap.md#13-memories-tables-globals-and-the-host-boundary)'s
  not-across-segments half may not be described as held.
- **No text format.** No script reader for the specification's text format is written at all; the
  corpus is produced by a binary encoder in a harness root, and an encoder is not a script reader
  with a smaller name.
- **No vectors, no garbage-collected type surface, no exceptions, no tail calls, no threads, no
  64-bit addressing, and no multiple memories.** Each has a manifest identity allocated or refused
  in roadmap
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) and none of them
  is minted here. Threads stay excluded by name for the reason
  [section 14](roadmap.md#14-suspension-threads-and-what-this-profile-does-not-declare) gives,
  which is not scope.
- **No second execution arm, no IL emission, no code generator, and no tiering.** The MVP has one
  interpreter and no promotion path, and there is no tier for a promotion to reach.
- **No persistence and no code cache.** Roadmap
  [section 18](roadmap.md#18-persistence-and-the-code-cache) names the key and delivers nothing,
  and the MVP delivers nothing of it either.
- **No JavaScript API for WebAssembly.** Roadmap
  [section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly) prices
  the boundary and gives it to a component that composes two profiles. No such component exists,
  the MVP does not create one, and a reader who meets this profile in a browser image must not
  infer that a page can call `WebAssembly.instantiate`.
- **No aggregate conformance percentage, at any point, in any document.** This rule does not
  weaken when there is no conformance run to summarise; it is the rule that stops one being
  invented.
- **No milestone reaches `Accepted`, no package is published, no runtime identifier is claimed, and
  no support table is issued.** These are the MVP's own deferrals rather than its exclusions, and
  they are the reason the two lists are kept apart: an exclusion is work nobody will do under this
  programme, a deferral is work nobody will *approve* under it, and reading either as the other is
  how an MVP starts claiming things.

### Two findings the MVP path meets, and three routes it takes without a decision

**Both are observed repository state in section 1's sense** — reviewable facts about the current
checkout, each able to explain a status and neither able to satisfy a gate *(added 2026-09-07)*.
They are recorded here rather than left in the plan alone because the MVP path meets each of them
in code before the milestone that names it closes.

**The core's canonical-only variable-length integer readers reject encodings the specification
requires and production toolchains emit, so this profile writes its own varint layer.** The
specification admits redundant continuation bytes inside a byte budget derived from the width and
rejects only an encoding that exceeds the budget or sets unused bits in its terminal byte; the
core's readers accept the canonical form alone, for a reason that is correct for a format the core
also defines. Roadmap
[section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core) resolves it
by decoding here, over `TryReadByte` and the core's other byte-level primitives. **Three
consequences are observed state rather than plan.** The profile re-derives the bound-before-use
ordering that `TryReadDeclaredCount` provided, because that member reads a canonical integer before
comparing it against its bound and is therefore unreachable to this decoder. `DeclaredCount` moves
from the core's core-metered row to its profile-charged one, while `SectionCount` and
`StructuralDepth` stay core-metered and `ArtifactBytes` stays unevadable — WA-2's gate names the
dimension and reads the core's own metering-split record against it, and the third row of the
unopened-dependency table in section 3 below holds that as a correction this profile owes the core.
And **the core's
`DeclaredCountExceeded` bounded-read status becomes unreachable in this profile**, because the only
member that raises it is the one this decoder declines to call, so the count-ceiling refusal is
manufactured by this profile's own reader and is this profile's to get right
([WAC-27](roadmap.corrections.md#wac-27)).

**No spelling of a guest-observable `memory.grow` refusal exists on the shipped contract, and the
MVP takes a route where a deferred decision would have chosen.**
[WAC-03](roadmap.corrections.md#wac-03) establishes the gap: the retention report returns nothing,
so a ceiling-class dimension cannot carry a refusal at the point of retention, and a refused
`TryCharge` at any scope latches exhaustion so the core rewrites the completed step as
`ResourceExhaustion` whatever the profile did with the `false`. Section 3's second row opens the
amendment that would close it, and the MVP programme defers exactly the co-signing that would mint
one. **A deferred decision is a decision nobody took**, so the MVP does not get to wait for it and
does not get to pretend it went a particular way. What it does instead is take one route and name
it as taken without a decision: growth is gated on **this profile's own declared memory maximum**,
which is not a core budget refusal, so the specification's `-1` answer is produced, the module
observes it, the operation completes normally and no core allowance is spent — while a refusal
caused by a **core** budget stays non-guest-observable and is a deviation from what the
specification says `memory.grow` answers. **The deviation is published, and publishing it is
WA-10's release decision and not WA-5's**, which [WAC-16](roadmap.corrections.md#wac-16) fixed and
the MVP does not disturb ([WAC-28](roadmap.corrections.md#wac-28)).

**Three routes this profile's MVP takes are recorded as taken without a decision, and this row
names them so a reader of this ledger meets them without opening another file.** Section 5 of
[the MVP programme record](../../../docs/mvp.md) carries them: MVP-1 is the memory-growth route
above; MVP-2 is a sixteen-byte value slot that reserves the vector width now rather than widening
every slot later, which pre-empts the row roadmap
[section 9](roadmap.md#9-the-value-store-and-frame-model) calls the one whose late answer
invalidates the others; and MVP-6 is the entry-point argument encoding that
[section 10](roadmap.md#10-execution-mapping-webassembly-onto-the-core-lifecycle) says is decided at
WA-1 or by accident later. **None of the three is a decision, none advances a row here, and each may
be reversed without a correction entry, which is the cost rather than the convenience**: anything
built on one is work that may have to be unbuilt. They are recorded because a route nobody wrote
down is an invisible branch, and an invisible branch in a component nobody has reviewed is one
nobody would ever find. **Two of the three are now code** — the memory-growth route and the
entry-point encoding — and being code makes neither of them a decision.

---

## 3. Open external dependencies

A milestone blocked by a named external dependency records the blocker, its holder, and its unblock
condition. **Two are open today**: one binds WA-2 onward, and one binds WA-5 without blocking
anything yet, because WA-5's predecessors are not done either.

| Blocker | Holder | Unblock condition | Note |
|---|---|---|---|
| **The core contract is not accepted.** Every core milestone is in progress and unaccepted, and the core's review record is unsigned. The core's own ledger records that a profile roadmap may open once the contract is accepted, and that it is implemented but not accepted. | The Broiler.VM core's architecture and release owners | A recorded human review decision on the core's contract surface, at a named contract version | This blocks WA-2 onward. It does **not** block WA-0 or WA-1, which build against the contract as implemented — a distinction the roadmap's delivery order states and this ledger holds it to. **Under the MVP programme this row blocks the acceptance of WA-2 onward and the publication of anything, and does not block writing the code** *(added 2026-09-07)*: update rule 8 already rules that human review gates a release rather than a development step, and [the MVP programme record](../../../docs/mvp.md) defers acceptance on the same shape. What stays blocked is every row's move to `Accepted`, every package, every claimed runtime identifier and every issued support table. What is not blocked is writing WA-2's decoder, WA-3's validator and WA-5's interpreter against the contract as implemented, and retaining what those runs show ([WAC-29](roadmap.corrections.md#wac-29)). |
| **The refusable retention member is unfiled, and the amendment procedure is unexecutable.** [WAC-03](roadmap.corrections.md#wac-03) establishes that no guest-observable `memory.grow` refusal exists on the shipped contract in any spelling, and roadmap [section 20](roadmap.md#20-amendments-and-this-profiles-duty-as-the-counterweight) opens the row rather than filing it, because no local resolution exists. No amendment has been minted and one person holds the minting role and both co-signing roles, so no co-signature would be independent. | The Broiler.VM core's contract and release owners | A minted amendment carrying a co-signature, or a recorded refusal | This binds **WA-5**, whose memory representation the plan's earlier reading could not choose without it. It blocks nothing today, because WA-3 has not started either; **an unanswered row would have made WA-5 `Blocked` rather than merely late at the moment WA-3 would otherwise let it start, and under the MVP programme it does not** *(corrected: WAC-28)*. If it is refused or never answered, the fallback — a memory whose growth refusal is not guest-observable — is **WA-10's release decision**, published in the support table as a named deviation, and is not WA-5's to take *(corrected: WAC-16)*. The other intended profile has since recorded its position on this row, dated 2026-09-01, as *unaffected* — it neither files the row nor obstructs it — so the procedure's counterweight question is answered for this row and the blocker is the procedure alone ([WAC-26](roadmap.corrections.md#wac-26)). **The MVP programme defers the co-signing step, which is the step this row waits on, so the row does not close under the MVP and WA-5 does not wait for it either** *(added 2026-09-07)*: WA-5's memory work proceeds on a route taken without a decision — growth gated on this profile's own declared maximum, a core-budget refusal not guest-observable — and section 2 records it as such. The row stays open with the same holder and the same unblock condition, and the deviation's publication stays WA-10's release decision ([WAC-28](roadmap.corrections.md#wac-28)). |

Four further dependencies are **unopened rather than blocked**, and naming them here is the point.

| Unopened dependency | Why it is not a blocker yet | What opens it |
|---|---|---|
| **The specification revision has not been retrieved, hashed, or archived.** Retrieving and archiving a third-party document is a human action, not a build step. Until it is performed, every reference in the roadmap is a discovery link and the pin is provisional. | Nobody has been asked to do it, which is a scheduling gap and not a dependency. | WA-0 records the intended revision and names an owner; WA-2's gate requires the pin actually taken, or a named exclusion. |
| **The conformance suite revision has not been pinned**, and the licence and attribution consequences of ingesting it into this tree have not been confirmed. | Same. | WA-0 records the obligation, names its owner and names the release owner who co-signs it. **WA-4 resolves the commit and lands both the attribution row and the standing-claim confirmation**, in the change that first ingests a suite file, because a notice cannot carry forward content this tree does not hold *(corrected: [WAC-15](roadmap.corrections.md#wac-15))*. |
| **The core's metering-split record obliges every profile to route declared counts through the binary package, and this profile cannot.** Roadmap [section 7](roadmap.md#7-the-artifact-the-decoder-and-one-disagreement-with-the-core) establishes that a format admitting padded variable-length encodings may not call the guarded count reader, so `DeclaredCount` becomes this profile's own charge where the core's record calls it core-metered. The core's published support table already carries the primitive half of this; its metering-split record does not. | Nobody has raised it, which is a scheduling gap rather than a dependency. **It stopped being harmless on 2026-09-07**: this profile now has a decoder that charges the dimension itself, so the record is false about something that exists rather than about something planned, and the correction WA-2 owes has an artifact behind it. | WA-2's gate reads the record against the answer and either confirms the conditional reading or files a correction with the core's architecture owner, recording this row as open with that holder *(corrected: [WAC-24](roadmap.corrections.md#wac-24))*. |
| **The cross-profile boundary of roadmap [section 17](roadmap.md#17-the-cross-profile-boundary-the-javascript-api-for-webassembly) has no owner.** A browser that runs WebAssembly through JavaScript needs a component that composes two profiles, and none exists or is planned. Roadmap section 17 now also records the two frozen facts that shape it — a guest-initiated load may not name another profile, and cross-runtime reentry is legal and is the route the seam takes — so the price is written down even though nobody is paying it. | It is outside this component by construction, and this component's obligation is to price it rather than to pay it. | A browser-integration component, whenever one is opened. That component owns the two-profile composition's closure report, its Native AOT evidence, its shared aggregate budget, and the reconciliation of two profiles' *defaults*; their maxima are not coupled and reach no neighbour *(corrected: WAC-01)*. Until it exists, WA-0's defaults record states the cross-profile consequence, which is the half this component can discharge alone. |

---

## 4. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle carrying all applicable
fields below. **A command written in a plan is not evidence that the command ran.**

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap and gate revision, core contract version, format version, feature manifest set, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Component commit, dirty-tree state and patch identity, and the exact paths and projects under test. |
| **Pins** | The specification revision with its hash and the human action that archived it; the conformance suite commit; the scope manifests binding manifests to suite paths. A provisional pin is recorded as provisional. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, corpus and fixture hashes, and applicable provenance or licence decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets redacted without hiding semantically relevant configuration. |
| **Effective limits** | The effective limit vector every conformance and measurement run executed under. A conformance total obtained under generous ceilings is not the total a product shipping tight ones would get, and a bundle that omits the vector cannot be compared with any other. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Results** | Raw outputs retained, including failures, and conformance results reported **per assertion family**. A bundle that retains only the passing half is not a bundle, and a bundle that reports one percentage is not a conformance result. |
| **Negative controls** | Each control, the injection that must make it fail, and the revert that must make it pass. The count is stated and grows across milestones. |
| **Closure** | For any Native AOT claim: the published output's dependency closure, read off the published image rather than asserted, with the absence of the ingestion path asserted explicitly. |
| **Exclusions** | What the bundle does **not** show. Every open gate clause, every unexercised path, every single-machine or single-RID limitation, named. |

---

## 5. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, **including its failures and its exclusions**.
3. Do not infer completion transitively. WA-1 acceptance does not accept WA-2; a slice-manifest
   result does not accept a later manifest; a strong result in one assertion family does not accept
   another; and JIT, trimmed, or one-RID success does not accept an untested Native AOT or RID
   claim.
4. Do not promote shell, smoke, analyzer-only, or shape-only results beyond what they prove. A
   failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence gathered
   against a different population is not silently carried forward. **A specification or suite
   re-pin is such a change**, and so is a core contract amendment: record the new revision and
   state, per affected record, what recertifies unchanged, what must be re-collected, and what is
   superseded. A conformance total is bound to the suite revision that produced it and to nothing
   else.
6. **Do not record core work here, and never record profile work in the core's ledger.** A core
   result never advances a row in this file, and no row here advances a row there. The same rule
   holds in both directions for every other language profile: this component has no dependency edge
   to one and takes no evidence from one.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and the evidence-bundle ID in
   the affected row. Where owner and reviewer are the same person, record the non-independence in
   the row rather than resolving it by assertion.
8. **Human review gates a release, not a development step.** Development work — implementing a
   milestone, landing it, collecting its evidence — may proceed and merge without a review decision.
   A **release** may not: no package is published, no RID is claimed, no support table is issued,
   and no milestone moves to `Accepted` until a named human has read the work and recorded a
   decision on every relevant code unit, bound to that declaration's fingerprint so a unit that
   changes afterwards reports stale rather than being silently carried.

   One consequence is worth stating plainly, and it is the opposite of the one a seeded component
   feels. **This component's review debt starts at zero and grows only as fast as the work does.**
   There is no inherited body of unreviewed code arriving on day one. That makes the review queue
   trackable from the first commit, and it removes the excuse a large inherited backlog would
   supply.
9. **A unit's origin is recorded, and the expected distribution is uniform.** Every unit in this
   component is written here. The generated assurance report publishes the origin distribution, and
   **a unit whose origin is anything else is a finding** — either an undocumented copy, which the
   licence position must then cover, or a mis-annotation. A seeded component publishes this
   distribution to show how much it inherited; this one publishes it to show that it inherited
   nothing.
10. **No count, total, graph, commit, or score is copied into prose.** This ledger names the command
    or the retained record that reads it. That rule extends to the specification: instruction
    counts, opcode counts, and section counts are read off the pinned revision's own indexes and are
    not transcribed into this component's documents. A number transcribed into a sentence goes stale
    silently, and a ledger that goes stale silently is worse than one with a gap in it.

---

Until such updates are recorded, section 2 remains the complete status of this component:
**five milestones are in progress or blocked and six are not started, no milestone is accepted,
neither the specification nor the conformance suite is pinned, no language surface is supported, no
composition is advertised, no runtime identifier is claimed, no evidence bundle is retained, no
measurement or conformance result exists, and nothing has been reviewed.**
