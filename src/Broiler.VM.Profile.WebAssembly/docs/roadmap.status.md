# Broiler.VM.Profile.WebAssembly roadmap status

**Last updated:** 2026-09-29 (WA-4's selection pipeline, shards and merge, failure queue and tooling regression suite, [record wa-spec-005](../../../docs/evidence/wa-spec-005/README.md) retaining them; no row moves). Before that: 2026-09-29 (WA-4's ratchet set for the malformed and invalid families, [record wa-spec-004](../../../docs/evidence/wa-spec-004/README.md) retaining the run; no row moves). Before that: 2026-09-29 (the spec lane's self-check negative control as a retained case, its named configuration failures, and each family's totals in the roadmap's six counts; no row moves). Before that: 2026-09-29 (rule W7, WA-4's ingestion-path scan, and WA-4 moved to `In progress` with every other clause of its gate listed as open). Before that: 2026-09-29 (rule W6, no profile code naming a core outcome category where it executes; no row moves). Before that: 2026-09-29 (every function of the verified corpus executed, and no exception escapes; no row moves). Before that: 2026-09-29 (named cases for a call-stack exhaustion; no row moves). Before that: 2026-09-29 (two runtimes driving one verified handle at once, WA-5's concurrent clause; no row moves). Before that: 2026-09-29 (cases for two WA-3 clauses: nothing throws across the corpus, and what a translation reads each byte for, which shows a function body walked twice; no row moves). Before that: 2026-09-29 (the translation's bound on a unit's locals reached by a corpus pair, and why the other two bounds are not; no row moves). Before that: 2026-09-29 (a proportionality fixture for `memory.grow`, with its flat-charge control, and the growth's uncharged copy of the current contents recorded as a question for WAD-0001; no row moves). Before that: 2026-09-29 (the handle-immutability scan WA-5 names, written as rule W5, and the one defect it found corrected; no row moves). Before that: 2026-09-29 (the verifier outcomes the corpus cannot hold, and an invalid artifact's position, checked by a harness lane; the spec lane's remaining failures sorted by cause; no row moves). Before that: 2026-09-29 (the family hook's codes reached by named checks of the harness root's hook lane; no row moves). Before that: 2026-09-28 (the two composition roots published trimmed and Native AOT on one unclaimed identifier and run in three modes, the corpus replayed identically in each; [record wa-modes-001](../../../docs/evidence/wa-modes-001/README.md) retains them; no row moves). Before that: 2026-09-28 (WA-3's diagnostic registry published and bound in both directions by rule W3, one code given back its single reason, and phase-order cases added to the corpus; [record wa-spec-003](../../../docs/evidence/wa-spec-003/README.md) retains them; no row moves). Before that: 2026-09-28 (three validation rules moved from the decoder to the validator, and WA-3's fusion decision drafted as WAD-0004, Proposed; [record wa-spec-002](../../../docs/evidence/wa-spec-002/README.md) retains them; no row moves). Before that: 2026-09-28 (the specification's core scripts led to four corrections - custom-section names, import entries, a function type with two results, `call_indirect`'s type check - and to the harness lane scoring a module assertion by what the refusal says; [record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md) retains them; no row moves). Before that: 2026-09-28 (the conformance suite pinned for the universal bytecode programme; notes in section 2 and section 3). Before that: 2026-09-26

**This file is part of the [WebAssembly profile roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split).

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[WebAssembly profile roadmap](roadmap.md). The roadmap defines planned work and objective exit
gates; this ledger records whether those gates have accepted evidence. Where the core changed, a
sibling's dated finding settled something, or the plan replaced its own earlier reading, the plan
carries the new reading and [the corrections and rejections](roadmap.corrections.md) carry what it
replaced — **that file records no status and advances nothing here**.

**At this snapshot, five milestones own code and none is accepted** *(updated 2026-09-07)*. *(Noted 2026-09-29: six, since WA-4 owns rule W7, its ingestion-path scan; none is accepted.)*
The component has a source tree, projects in the solution, a decoder, a validator, a translator into
the universal bytecode, and the instruction family that bytecode's emitter runs — its table, its
verifier hook, its handlers and its instance store — two never-advertised composition roots, its own
group in the component's rule register, and an assurance record *(corrected 2026-09-26: this sentence
read "a descriptor, a decoder, a validator, a store and an interpreter". The universal bytecode
programme's milestone UBC-4 deleted the profile's descriptor, its interpreter and the store types the
interpreter used, on 2026-09-25, and what took their place is recorded under
[What now exists](#what-now-exists-and-what-each-part-of-it-is-not). Nothing else in this paragraph
moved, and no row below moved with it)*. It has no pinned specification revision, no pinned suite
revision, no evidence bundle, and no human review of anything. WA-0, WA-1, WA-3 and WA-5 move to
`In progress` on the strength of milestone-owned code that exists and runs; WA-2 owns code too and
stays `Blocked`, because its blocker binds acceptance and publication rather than authorship. *(Noted 2026-09-29: WA-4 moves to `In progress` too, on the strength of rule W7, the ingestion-path scan its gate names.)*
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
| [NONE] | **WA-0 — boundary, identity, assurance floor** | **In progress** | Milestone-owned work exists and no gate has been accepted. The profile project, the two never-advertised composition roots, the entry in the frozen project graph, this profile's own group in the component's rule register with a witness and a negative control for each rule, the family public-API baseline, the assurance annotations on every relevant unit, and the fifteen hard maxima and fifteen defaults with the three guest-load defaults written as large finite numbers all exist in the checkout. **No evidence bundle is retained and no gate clause is demonstrated**: nothing has been published or run on any runtime identifier, under trimming or under Native AOT, and the exit gate asks for exactly that. *(Noted 2026-09-28: the sentence before this one no longer describes the checkout's evidence. Bundle [ubc-4-006](../../../docs/evidence/ubc-4-006/README.md) ran the harness root trimmed and under Native AOT for the universal bytecode programme, and [record wa-modes-001](../../../docs/evidence/wa-modes-001/README.md) publishes both roots trimmed self-contained and Native AOT with trim and AOT warnings treated as errors, runs each in the three modes, finds each mode printing the JIT run's lines, and reads the closure off each published output. It is `linux-x64` only and that identifier is not claimed, so the clause this row's action names - on a claimed runtime identifier - is still not demonstrated, and neither are the decisions and the licence obligation's owner the gate needs.)* | Publish and run the two composition roots on a claimed runtime identifier under JIT, trimmed self-contained and Native AOT with trim and AOT warnings treated as errors, read the closure off the published output rather than asserting it, and retain the three tables as a bundle. Until that exists this row cannot move past `In progress`, and the licence obligation still has no owner and no co-signer recorded. |
| [NONE] | **WA-1 — the whole contract loop on a slice module** | **In progress** | Milestone-owned work exists and no gate has been accepted. The slice manifest identity, the family's declaration carrying every row of the full-arity descriptor a root builds from it, the payloads and the allocation-meter adapter, this profile's own variable-length integer layer, the binary corpus encoder in a harness root and the execution composition root all exist; **the contract loop closes end to end** — a module is translated, and its artifact cataloged, verified, instantiated and invoked, and the harness root prints what each step answered. The entry-point encoding decision is taken and written down beside the code that implements it: length-prefixed, so an export name carrying the encoding's own separators resolves, with float literals as hexadecimal bit patterns so they round-trip exactly. *(Corrected 2026-09-26: this cell read "The full-arity descriptor, the seven core-facing types, the slice manifest identity, format version 1 as a bare module, ... and the execution-only composition root all exist", and its action cell read "The operand-stack bound is computed at validation and stored on the verified state, which is one gate clause held; the rest are not". The universal bytecode programme's milestone UBC-4 deleted the profile's descriptor, verifier, executor, verified-state marker and continuation, made the module the translator's input rather than the artifact, and left the execution root claiming no label. The validator still computes the operand-stack bound and nothing sizes anything from it: a universal bytecode unit declares its own operand height, which the translator writes and the core's walk checks. So the clause the action cell called held is held by nothing in this profile. The row's state and its mark are unchanged, because the programme moves no row of this ledger.)* *(Noted 2026-09-29: of the five verifier outcomes, three are retained corpus entries - an accepted module, an invalid artifact with its code, a resource exhaustion naming its dimension and scope - and the other two are not properties of a module's bytes, so the harness root's outcome lane produces them by named checks rather than corpus entries: a cancellation, at translation and at the core's verification, and an unsupported profile. The same lane holds an invalid artifact's position to the encoding the diagnostic registry publishes, at three places, since the corpus manifest records a code and not a position. Of the five execution-step kinds, the execution checks produce an instantiation, a completion and a fault; no named test produces a contract violation from a non-conforming variant, and the refusal that declares `Suspended` unreachable is not tested. The row's state is unchanged.)* *(Noted again 2026-09-29: the refusal is tested now. The outcome lane invokes an export with the operation's control handle and asks it to suspend, and the core answers `Unsupported` with `ExternalSuspensionNotDeclared`, because this profile declares no external suspension. A contract violation still has no named test: the family is internal to the profile, so a non-conforming variant of it cannot be composed from a root.)* | Retain the run as a bundle rather than as a console transcript, then produce each of the five verifier outcomes and each of the five execution-step kinds from a named case in that bundle, on every claimed runtime identifier under all three publish modes. |
| [NONE] | **WA-2 — the decoder, the integer decision, the malformed corpus** | **Blocked** (the blocker binds acceptance and publication; milestone-owned work has begun and is recorded here) | Milestone-owned work exists and no gate has been accepted. The decoder covers the whole binary grammar this format version admits, with the section-order table written as a table rather than as an identifier comparison, its own signed and unsigned variable-length readers, the bound-before-use ordering re-derived, strict UTF-8 name validation, and custom sections read past. *(Corrected 2026-09-28: a custom section was read past whole, its name included, and an import section was refused at its count, so a malformed custom-section name or import entry was admitted or answered as unadmitted. The decoder now reads a custom section's name and every import entry, and refuses a declared import only after the whole module has decoded; [record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md) retains the corpus rows, the script commands that moved and a negative control for each. This cell's "the corpus is not retained with a hash per entry" does not describe the checkout either: `src/tests/wasm/corpus/corpus.manifest` pins every entry by SHA-256, and `eng/wasm-corpus-integrity.py` re-hashes it and mutates it. Replay under three publish modes is still missing.)* A malformed and invalid corpus lives in the harness root and every entry reproduces its recorded triple when that root is run. **What the exit gate asks for and does not have**: the corpus is not retained with a hash per entry, it is not replayed under three publish modes, the three tables are not compared, the specification revision is not pinned, and the correction this milestone owes the core's metering-split record has not been filed. *(Noted 2026-09-28: [record wa-modes-001](../../../docs/evidence/wa-modes-001/README.md) replays the retained corpus under JIT, trimmed self-contained and Native AOT on `linux-x64`, and the three tables compare equal line for line; every entry reproduces its recorded answer in every mode, and the corpus integrity check, run with the Native AOT image as the replay, detects each byte it flips. The identifier is not claimed, so the gate's "JIT, trimmed, and Native AOT hosts" is shown on one unclaimed identifier and no more. The retained hashes were already true of the checkout, as the note above says; the pin and the metering-split correction are untouched.)* *(Noted again 2026-09-28: the corpus holds a derived entry for every adjacent pair the section-order table forbids, the tag and data-count pairs included, and one for the pair it requires where the identifiers descend - the data count section before the code section, which must be accepted. Under this build the tag section before the memory section is refused at the tag section, as unadmitted, before the order can be judged, and its entry records that. Ordering sections by identifier instead of by the table fails exactly the three entries where the two disagree.)* *(Noted again 2026-09-28: the scan the gate asks for is rule W4 of the component's register. The profile names none of the core's canonical variable-length readers, and compares a declared count with its ceiling only in the two members the rule lists - the one reader of a count from the payload, and the decoder's total of a body's local runs. Its witness is a source with one of each defect, and the rule reports exactly those.)* | Pin the specification revision or record a named exclusion; retain the corpus with its hashes; replay it under JIT, trimmed and Native AOT; and file or confirm the metering-split correction with the core's architecture owner. **Under the MVP programme the named dependency blocks this row's acceptance and this component's publication, and does not block writing the code** ([WAC-29](roadmap.corrections.md#wac-29)); the code is written and the row stays where it is. |
| [NONE] | **WA-3 — validation and the diagnostic registry** | **In progress** | Milestone-owned work exists and no gate has been accepted. Validation is the specification's single-pass algorithm over a value stack and a control stack with polymorphic unreachable code; decoding completes before validation begins at module granularity; implementation-limit refusals answer as resource exhaustion naming a dimension and a scope rather than as an invalid artifact; and every rejection carries a stable code and a byte position. **The registry is not published**: the codes exist in the source as a closed enumeration and there is no versioned registry document bound in both directions, so the gate clause asking for one is unmet. The nesting corpus at and beyond the structural-depth ceiling is not written, and no case yet fails when the two phases are fused. *(Noted 2026-09-28: a function type with more than one result is refused now, with its own code; [record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md). The specification's scripts, scored by what a refusal says, show three disagreements this row owns. The decoder refuses three things the format validates: a minimum above its maximum, a memory above the format's page maximum, and a constant expression of other than one instruction. Those are answered as undecodable where the scripts expect invalid. A constant expression's other instructions, two memories and two tables are answered as unadmitted features, where version 1.0 calls them invalid. And decoding and validation are fused within a function body, because the validator reads the bodies: a body invalid before it is malformed is answered invalid. The gate asks for that fusion decision to be recorded, and it is not. The lane's self-check now carries a malformed-and-invalid fixture, which is the scorer's check and not the named case this gate asks for.)* *(Noted again 2026-09-28: the first of the three disagreements is gone, and the other two stand. The decoder no longer refuses limits, memory sizes or constant expressions as malformations. The validator refuses them with codes of its own, and [record wa-spec-002](../../../docs/evidence/wa-spec-002/README.md) retains the corpus rows, the script commands that moved and four negative controls. The fusion decision is drafted as [WAD-0004](decisions/0004-decode-and-validate-fused-within-a-function-body.md), Proposed and not taken. What version 1.0 calls invalid and this profile answers as unadmitted - imports, a second memory or table - is unchanged.)* *(Noted a third time 2026-09-28: the registry is published, as [`docs/diagnostics/registry.txt`](diagnostics/registry.txt) at its first revision, and rule W3 binds it in both directions: to the enumeration; to every emission site, with its one reason and in the passes its row names; to the corpus and the harness root's execution checks; and to the corpus manifest, which now states the registry revision it is dated by. The registry states which fields of the core's position record each carrier fills, and records as found that the trap payload fills them differently from the verification path. Writing it found one code emitted with two reasons, and that code has one now. The corpus holds phase-order cases, each invalid at a byte before the one at which it is malformed and each refused as malformed, and each fails under a control that fuses the two phases at module granularity. [Record wa-spec-003](../../../docs/evidence/wa-spec-003/README.md) retains them. The sentence above that the nesting corpus is not written does not describe the checkout: the corpus holds entries nested to the effective structural-depth ceiling and one level beyond it. **What the gate asks for and still does not have**: no named case reaches the rows the rule lists as unreached - the family hook's own codes and the two it shares only with a retired decoder check, the translation's bounds, the two defect codes, the reader's malformation and the trap the earlier revision named; an execution row's case observes a trap's kind, not the code beside it; the fusion decision is drafted and not taken; and the nesting corpus has not run under Native AOT on any runtime identifier.)* *(Corrected 2026-09-28, later the same day: the note above says an execution row's case observes a trap's kind and not the code beside it. The harness root's execution checks now read the code as well, against their own table of the registry's codes, and rule W3 holds that table to the registry's rows.)* *(Noted 2026-09-28: [record wa-modes-001](../../../docs/evidence/wa-modes-001/README.md) replays the corpus in the three publish modes on `linux-x64`: the invalid half replays identically in all three, and the nesting entries are accepted at the structural-depth ceiling and refused one level beyond it as a resource exhaustion naming `StructuralDepth`, under Native AOT as under JIT, without terminating the process. The identifier is not claimed, and the gate asks for every claimed one.)* *(Noted 2026-09-29: the harness root's hook lane reaches the family hook's own codes and the two it shares only with a retired decoder check, each with a check of its own - the artifact the translator wrote for a small module, altered in one way the translator never would, and handed to the core's verification, with the answer written down beside it. The registry names each check as its case, and rule W3 holds the check's answer to the row. Disabling the hook's check of the Positions order, or of a global's type at the instruction that reads it, makes the core accept the altered artifact and fails that check. What no named case reaches is now the translation's bounds, the two defect codes, the reader's malformation and the trap the earlier revision named, and the rule lists them.)* *(Noted again 2026-09-29: one of the translation's bounds is reached. The corpus holds a pair at and one past the locals one universal bytecode unit declares. Both modules validate, the lowering admits the first, and it refuses the second with the bound's code. The operand-height and jump-table bounds stay unreached, and the rule now says why in its own words. Each takes a module with one push or branch table past the bound, which is tens of thousands of them. Its twin at the bound, which a refusing entry needs beside it, is verified twice a replay under the one runtime every lane of the harness root shares. The height twin left the later lanes little of that runtime's allocation ceiling, and the jump-table twin passed it. The operand-range code, the two defect codes, the reader's malformation and the earlier revision's trap stand as they were.)* *(Noted a third time 2026-09-29: two clauses of the gate this row never mentioned now have cases, in a decoding lane of the harness root that runs on every run. The first is that the validator throws on nothing across the whole corpus. The translator answers any exception that escapes it as the reserved defect code, and the lane translates every retained module and finds none answered with it. With the validator made to throw on one instruction, the lane names each module that reaches it. The second is that each module is decoded at most once during verification, and it holds only in part. The core never reads the module, so the translation is where it is decoded, and the lane reads what the translation charges per byte by bisecting its work allowance. A custom section's payload costs one unit a byte, so the module's sections are decoded once, and a translation made to decode the module twice fails the check. A function body's instructions are read three times. The decoder copies them, charged, and the validator decodes them, charged. Then the lowering walks them again to write the artifact, reading every immediate again, and it charges nothing because the validator paid for the bytes. The lane holds a body to two charged units a byte, so a third charged pass fails it, and it does not claim a body is decoded once. Whether the lowering's walk counts as a decode under the clause is a question for the gate's owner. The universal bytecode programme's design put the walk there, and this ledger does not answer the question.)* | Publish the versioned diagnostic-code registry and bind it in both directions; write the malformed-and-invalid case that fails when the phases are fused at module granularity; write the nesting corpus at and one level beyond the ceiling and run it under Native AOT on every claimed runtime identifier. *(Noted 2026-09-28: the registry, its rule and the phase-order cases are written, and the nesting corpus exists; see the evidence cell. What stays is to reach the rows rule W3 lists as unreached by named cases, to take or reject WAD-0004, and to run the nesting corpus under Native AOT on every claimed runtime identifier.)* |
| [NONE] | **WA-4 — the oracle** | **In progress** *(moved 2026-09-29 from `Not started`: milestone-owned work exists, rule W7, the ingestion-path scan this milestone's gate names; see the note at the end of the evidence cell)* | None. No suite pin, no script reader, no harness, no self-check fixture. *(Corrected 2026-09-28: the first three no longer hold, and the fourth is partly false. The conformance suite is pinned in `src/tests/wasm/spec`. The harness root's `--spec` lane reads it and runs every command. Before every run, the lane runs a self-check of declared verdicts that includes the malformed-before-invalid fixture this milestone's method asks for. Since [record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md), it scores a module assertion by the category the refusal names. The row stays `Not started`: the lane was written for the universal bytecode programme's UBC-4 rule, and none of this milestone's gate exists. There is no selection pipeline, shard, scope manifest, per-assertion-family total of the gate's shape, configuration failure, failure queue, per-family ratchet, harness regression suite or ingestion-path scan from the execution root.)* *(Noted 2026-09-29: what the lane still fails, sorted by cause rather than counted - `docs/evidence/wa-spec-003/comparison.log` has the per-family totals. Most are modules that import, which this build refuses as unadmitted until WA-6, and the commands that name such a module's instance. After a call-stack exhaustion an instance answers every later call as terminally faulted, where the specification lets it continue: [ADR 0004](../../../docs/adr/0004-lifecycle-and-state-machine.md) makes a resource exhaustion fault its instance always, so that is the core contract's position and not this profile's defect. Reading an exported global has no surface in the lane - the embedding seam is WA-7's - and tables shared between modules are WA-6's. A second memory or table, and a later version's segment forms, are refused as unadmitted. Of the malformed modules, one is answered as the declared-count exhaustion roadmap section 7 reclassifies, and one is draft decision WAD-0004's. The lane's reader refuses a script written as a bare run of module fields, `inline-module.wast`, which is a gap in the reader and this milestone's to close.)* *(Noted again 2026-09-29: that gap is closed. The reader reads a run of bare module fields as one module, as the specification's interpreter does, so `inline-module.wast` is one module command that instantiates where it was three commands the reader refused; no other command of the lane moved. The lane's self-check carries a fixture of the form, and reading each field as a command of its own fails the self-check and runs no script.)* *(Noted a third time 2026-09-29, and the reason the row moved: work written for this milestone's gate exists. Rule W7 is the ingestion-path scan the gate names. It holds rule N13's clauses over the WebAssembly harness root and the specification's scripts, so the root is referenced by nothing, packs to nothing, is advertised nowhere, and appears in no advertised closure, and no project names the suite. A seventh clause refuses a harness file compiled into another project, which neither N13 nor A3 refuses. The gate's own control, the execution root referencing the harness, is reported. **Every other clause of the gate is open.** The suite is pinned to a commit, but nothing resolves it once before a shard and re-reads the checkout, because there are no shards. The self-check runs before every run, with broken fixtures, a passing control and the malformed-before-invalid fixture, but its negative control has been run by hand and is not a retained case. The malformed and invalid families run to completion, and the retained records publish their totals, but no run has set a ratchet. Removing a shard's report, a configuration field differing between shards, an empty selection and an all-skipped selection are not reported as such, because there is no merge. The failure manifest is not proved a queue. The harness, merge, audit and scope tooling carry no regression tests. And the release owner's confirmation that the core's third-party claim stays scoped is owed, as section 3 records; that one is a co-signature and not work.)* *(Noted a fourth time 2026-09-29: three of those clauses are now held by the spec lane. First, the self-check's negative control is a case the lane runs every time. After the self-check agrees with its declared verdicts, the scorer is made to pass a malformed or an invalid assertion on any refusal, as it did until 2026-09-28. The same script must then disagree, and it does, with three verdicts. The regression is reverted before any script of the suite is read, and a scorer that ignored the injection fails the self-check. Second, the configuration failures a lane without shards or scope manifests can show are named, and each ends the run with its own exit code: an empty selection, no executed tests, and a family that selected commands and executed none. Third, every assertion family's totals are published in the roadmap's six counts, selected, executed, passed, failed, skipped and timed out, with the effective limit vector the lane already printed. Timed out is zero by construction, because every command is bounded by its runtime's budget. The per-command answers did not move. Still open are the ratchet, shards and their merge, the failure queue, the tooling's regression tests, the missing-revision failure the ratchet brings, and the co-signature.)* *(Noted a fifth time 2026-09-29: the ratchet is set. [Record WA-SPEC-004](../../../docs/evidence/wa-spec-004/README.md) retains the run that sets it. The malformed and invalid families run to completion at the pinned revision, from an exact commit, under the effective limit vector the run prints. That run writes the floor, their two passing totals, to [`src/tests/wasm/spec/wasm-spec.ratchet`](../../tests/wasm/spec/wasm-spec.ratchet). The file records the revision, the manifest and the limits it was set under, and names those two families and no others. A second run is held to it. The lane compares a run with a floor only when all three match. A file naming another family is refused, and so is a write that would lower a floor. The record shows each refusal with its exit code. So does the missing-revision failure, which ends a run that asks for a ratchet without a pin. The record also shows the self-check's negative control failing the lane when the injection is ignored. Still open are shards and their merge, the selection pipeline and scope manifests, the failure queue, the tooling's regression tests, and the co-signature. The record's section 5 says so too.)* *(Noted a sixth time 2026-09-29: the machinery the last note listed as open exists, and [record WA-SPEC-005](../../../docs/evidence/wa-spec-005/README.md) retains it working over the pinned scripts. Selection is a recorded pipeline, with the count after each stage printed before any shard is chosen. The stages are discovery, known-incorrect exclusion, the slice's scope manifest ([`wasm-spec.slice.scope`](../../tests/wasm/spec/wasm-spec.slice.scope), naming every script, because scoping one out is this milestone's owner's to decide) and per-file selectability. A script's shard is a stable hash of its name. The merge proves the shards covered the whole selection. It names a missing shard as incomplete coverage, a field that differs between shards as an inconsistent configuration, and a report cut short as a shard that did not finish. Four shards give the whole run's answers line for line, and the merge holds the ratchet. The failure queue, [`wasm-spec.queue`](../../tests/wasm/spec/wasm-spec.queue), lists every failing script. A run reports a listed script that passes, a hand-written name it cannot confirm, and an unlisted failure. The tooling's own regression suite runs before any shard and any merge, and a merge patched to sum a partial set fails it. What stays open is the co-signature, the audit command the next-action list names, and a cache of the resolved pin; the record's section 6 lists them.)* | After WA-3, and in parallel with WA-5. This is the milestone whose value is lost by serialising it: the malformed and invalid families can be scored before any interpreter exists, and that is the main structural advantage this profile has. *(Noted 2026-09-29: the ingestion-path scan is written; what stays is every other clause the evidence cell lists as open.)* |
| [NONE] | **WA-5 — value model, store, interpreter** | **In progress** | Milestone-owned work exists and no gate has been accepted. **The nine value, store and frame routes are recorded in this profile's decision series** as [WAD-0003](decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md), which states where each stands now that the universal bytecode executes this profile's modules — the numeric, reference and rooting rows carried over, the `LiveBytes`-breach row answered by a retention charged before the allocation, the call convention, frames, trap propagation and metering the universal bytecode's, and the sixteen-byte slot reversed — and the memory representation is [WAD-0001](decisions/0001-the-memory-representation.md)'s pinned array, reallocated on a successful growth. What executes a module is this profile's family under the universal bytecode's bytecode emitter: the numeric surface for all four types, locals, globals, one linear memory with its loads, stores, size and growth, structured control flow with all four branch forms, direct calls and indirect calls. *(Noted 2026-09-28: an indirect call's type check is structural, the callee's function type against the named one, where it compared type indices as the retired interpreter did; [record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md).)* The family's instance store charges each memory's and each table's allocation and retention, and each growth's, before it allocates, and releases what it retained on every failure the emitter answers, though not when the core drops an instantiation the emitter answered as complete *(corrected: WAC-43)*; instantiation evaluates global initialisers, applies element and data segments in order with each segment bounds-checked whole before any of it is written, and runs the start function. **What the exit gate asks for and does not have**: no evidence bundle, no fixtures or Native AOT representation probes beside WAD-0003's rows, no call-depth default derived from a retained per-runtime-identifier frame-cost measurement, no memory-growth proportionality fixture with a flat-charge negative control, no structural scan proving that no mutable state is reachable from a handle, and one member of the closed trap list is declared and unreachable — the earlier specification revision's name for an out-of-bounds indirect call, which this build reports under the current revision's name. *(Corrected 2026-09-26: this cell read "**The nine-row value, store and frame decision is taken and written down before the interpreter's first line**, each row with the alternative not taken: an untyped operand stack ..., a sixteen-byte slot reserving the vector width now, heap-allocated frames ...", and "The interpreter runs the numeric surface ...; the store, its memories and its tables are allocated, charged and reported retained". The universal bytecode programme's milestone UBC-4 deleted the interpreter and its value slot on 2026-09-25, moved the nine routes from the slot's source remarks into WAD-0003, and replaced the store's report after allocation with a charge before it. The row's state and its mark are unchanged, because the programme moves no row of this ledger.)* *(Noted 2026-09-29: the handle-immutability scan exists, as rule W5, so that item of the next-steps cell is done. It starts at the module definitions the family hook answers with, which the universal bytecode's verified program keeps and the core's handle wraps. From there it walks every type a stored member names, and holds each member to `readonly` and to an immutable type. It found one defect: the position index kept its sorted keys in an array. The index now keeps them in an immutable array over the same storage. The rule's register row states what the scan does not show: it reads syntax, it holds a read-only memory window by its type, and it does not reach the gate's concurrent-reader half. The row's other gaps stand, and so does its state.)* *(Noted again 2026-09-29: the proportionality fixture exists, as a lane of the harness root that runs on every run, so that item of the next-steps cell is done too. It measures what `memory.grow` is charged against WAD-0001's declared function: one unit of fuel a page added, granularity one page, and the pages' bytes as allocated and as retained. Each delta runs on a fresh instance against a control that is the same call with a delta of zero. The deltas double, up to a large delta that is the gate's negative control. With the growth's fuel made a flat charge, every execution check still passes and this fixture fails, which is the control the gate asks for. The fixture also found something the declared function does not cover. A growth copies the memory's current contents into its new array, and nothing charges the copy, so growing by one page costs the same fuel from a memory of one page as from one of hundreds. The page ceiling bounds that copy. Whether the charge should cover it is a question for WAD-0001's charge row, which is a taken decision, so the lane prints the measurement and does not judge it. The fixture is assembled on every run and no module or figure of it is retained, because retaining figures is the evidence bundle's job and there is no bundle yet.)* *(Noted a third time 2026-09-29: the gate's concurrent clause has a case, beside the scan: "two runtimes read one shareable handle concurrently with no synchronisation". A lane of the harness root verifies a module in one runtime, and a second runtime composed alike admits that runtime's handle, because the family declares its artifacts shareable. Each runtime's instance is then stepped from a thread of its own, at once. The module keeps a counter in a mutable global and one in its memory, and every answer of each instance is its own next count, so neither ever took a value the other advanced. The threads record that their runs overlapped, and an instance made afterwards still starts from the module's own state. With the family made to share one globals array among the instances of a definition, every execution check still passes and this case fails. The scan and this case together are the clause's mechanism, and its residual is the scan's stated limits and the one module this case drives.)* *(Noted a fourth time 2026-09-29: a call-stack exhaustion has named cases in the harness root's execution checks. The spec lane's exhaustion commands scored it before, and those run only when that lane is asked for. The bytecode emitter keeps a guest's frames in an array and charges `CallDepth` for each, so the meter refuses a recursion and the process's stack never does. The cases are three. A recursion that stands as many frames as the ceiling completes. One frame more is refused as a resource exhaustion naming `CallDepth`. And a recursion with no end is refused the same way, after which the run goes on. The scope named is the runtime's: every level holds the same ceiling, and the core's meter names the outermost level that would refuse. Each refused instance is faulted, as ADR 0004 requires. With the emitter made to leave one frame uncharged, the case one frame past the ceiling completes and fails. What the clause still asks for is the run on every claimed runtime identifier under Native AOT, and a default derived from a measured frame cost. The ceiling these cases use is the profile's placeholder, and they move with it.)* *(Noted a fifth time 2026-09-29: the gate's clause that no exception escapes the interpreter across the corpus has a case. An exception that leaves a family handler is answered as a profile fault with the contract-violation reason, so the clause is observable as no answer carrying that reason. Most retained modules that verify export nothing, so a lane of the harness root writes each of them again with every function it defines exported. It instantiates each one, which runs its start function and segments, and calls each function on a fresh instance with zeros. No answer is the contract violation, and at least one call must complete. With the family's memory-access handler made to throw, the function that loads is named. The fuzz corpus the clause also names does not exist yet; it is WA-9's.)* *(Noted a sixth time 2026-09-29: the gate's step-kind clause has its scan, rule W6: "a scan asserts no profile code names a core outcome category". The universal bytecode's emitter answers every execution step, and the family's handlers answer it through the bytecode's own status. So no source of the profile names the execution step, and none names a core outcome category except the translator, which the rule lists. The translator names only the four categories a verification of this profile answers, because a translation answers in the fields the core's verification of the bare module answered in. A category named in the family's handler fails the rule, and so does a fifth category named in the translator. That the emitter maps the family's status onto the five step kinds is the universal bytecode's claim, not this rule's.)* | Measure the native cost of one frame of the bytecode emitter per claimed runtime identifier and derive the call-depth default from it; write the proportionality fixture and its unsimplified control; write the handle-immutability scan; and retain a bundle. The guest-observable growth refusal exists and is exercised, against **this profile's own** page ceiling — which is the route recorded as taken without a decision, and not a gate clause held. |
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
from it. *(Corrected 2026-09-26: two phrases of this paragraph are false since the universal bytecode
programme's milestone UBC-4 — "whose Broiler-owned reference set is the two core assemblies and
nothing else", which now also holds `Broiler.VM.Ubc`, and "a value slot, a store with memories,
tables and globals, an interpreter", whose types were deleted. The paragraph added below on the same
date says what took their place, and the lifecycle the harness root demonstrates now begins with a
translation.)*

**Three things about that surface are worth stating in the direction that costs.** The frames a
module runs in are the universal bytecode's, heap-allocated and owned by the operation, so guest call
depth never grows the CLR stack and the only thing bounding recursion is the call-depth charge —
which means an exhausted call depth is a resource exhaustion naming a dimension, and a process that
stopped would be a defect rather than a limit. Nothing here parks, so no step produces a suspension
for a resume to take, and the continuation, resume and unwind are the bytecode emitter's; this
profile implements none of them. And one member of the closed trap list, the earlier specification
revision's name for an out-of-bounds indirect call, is declared and never raised, because this build
reports that case under the current revision's name. *(Corrected 2026-09-26: the first two sentences
read "The interpreter's frames are heap-allocated, so guest call depth never grows the CLR stack ..."
and "`Resume` answers the named invalid-state refusal because nothing here parks, and `Unwind`
releases a store and runs no guest code — but no path in this build mints a continuation, so that
release arm is written and unreached". The interpreter, the executor and the continuation they
describe were deleted at the universal bytecode programme's milestone UBC-4.)*

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
*(Corrected 2026-09-26: "This profile's Broiler-owned reference set is the two core assemblies and
nothing else" stopped being true at the universal bytecode programme's milestone UBC-4, when the set
gained `Broiler.VM.Ubc` and the roots gained it and `Broiler.VM.Emitter.Bytecode` as siblings. The
claim of incapability still holds, now over the set as it is: neither universal bytecode assembly,
and no source of this profile's, names any of those entry points, and the memory representation
[WAD-0001](decisions/0001-the-memory-representation.md) chose is a pinned managed array rather than a
reserved range.)*

**The memory-growth route recorded above as taken without a decision is now code.** Growth is gated
first on this profile's own declared page ceiling, which is not a core budget and refuses nothing on
the meter, so the specification's minus-one answer is produced, the module observes it, the
operation completes normally and no allowance was spent — and the harness root exercises exactly
that. **A refusal caused by a CORE budget is still not guest-observable**: the charge latches
exhaustion and the core rewrites the completed step, so the module never runs the instruction after
the growth. That remains a deviation from what the specification says the growth instruction
answers, it is written into the code that implements it, and **publishing it in a support table is
WA-10's release decision and not WA-5's** ([WAC-16](roadmap.corrections.md#wac-16),
[WAC-28](roadmap.corrections.md#wac-28)). Nothing here publishes it. *(Corrected 2026-09-26: "the
charge latches exhaustion and the core rewrites the completed step" describes the store the universal
bytecode programme's milestone UBC-4 deleted. The family's store charges a growth's allocation and
its retention before it allocates, so a growth a core budget refuses is never observed as a success:
the family ends the step at the growth, and the core answers with the exhaustion or cancellation the
meter latched. The module no longer runs the instruction after the growth, where the base's
interpreter pushed the minus one of a refused fuel or allocation charge and ran on until the core
rewrote the completed step *(corrected: WAC-43)*; the refusal is still not guest-observable, and the
deviation and its owner are unchanged.)*

**The universal bytecode programme's milestone UBC-4 changed what runs a module, and this ledger
records what it left in the tree** *(added 2026-09-26)*. Observed repository state in section 1's
sense, like the paragraphs above. The profile now holds a **translator**, `WasmTranslator` and its
lowering, which decodes and validates a module with this profile's own decoder and validator and
lowers it into a universal bytecode artifact; and an **instruction family** for that bytecode,
`WasmFamily`, with its table `WasmFamilyTable` selected by `broiler.webassembly.slice`, the codec of
the module definitions it writes into the artifact's family data, the verifier hook
`WasmFamilyVerifier` the core calls inside its own verification, the instance store holding a memory,
a table and globals as [WAD-0001](decisions/0001-the-memory-representation.md) decides, and the
reference arms the programme's differential check compares the universal bytecode's primitive table
against. Its Broiler-owned reference set is the two core assemblies and `Broiler.VM.Ubc`, which rule
W1 asserts. **The profile's own descriptor, verifier, executor, interpreter and value slot are
deleted, with the store, instance and continuation types they used**: the WebAssembly execution and
harness roots and the polyglot command-line root each translate a module and hand the core the
artifact, under a descriptor each builds with `UbcDescriptors.Build` from the family's registration
and declaration over the bytecode emitter, `Broiler.VM.Emitter.Bytecode`, which executes it. The
decision series exists at [`decisions/`](decisions/README.md), holding the memory representation
(WAD-0001), the recorded absence of a second manifest (WAD-0002) and the nine value, store and frame
routes (WAD-0003). The diagnostic registry gained the family hook's codes 2851 to 2860 and the
translation's limit codes 2871 to 2874 — 2875 was minted and withdrawn before release, and its number
is unused — and codes 2003 and 2004 are now emitted by nothing. The float comparisons the harness's
execution lane failed on now answer the specification's values, because the universal bytecode
executes them from its primitive table, and the reference arms that routed them to the integer arm
are corrected. And route MVP-15 — the module as source — is recorded in
[the MVP programme record](../../../docs/mvp.md) as taken without a decision, beside the three
routes section 2 names below.

**None of it moves a row here, and none of it is this profile's evidence.** The programme's own rule
is that no other ledger's row moves because of it, and update rule 6 keeps its bundles — `ubc-4-001`
and `ubc-4-002`, under the component's `docs/evidence/` and judged under the component's legend —
out of this ledger's evidence column, so every row above keeps its state and its `[NONE]`. The
programme's state is [its own ledger](../../../docs/universal-bytecode.status.md)'s. The plan's gates
are now read through roadmap corrections [WAC-30](roadmap.corrections.md#wac-30) to
[WAC-41](roadmap.corrections.md#wac-41) and [WAC-43](roadmap.corrections.md#wac-43) *(corrected: WAC-43)*, which narrow no clause, and no row here held evidence for
update rule 5 to re-evaluate.

### What this component is not claiming

Stated positively, because a table of empty rows invites a reader to fill them in:

- **No WebAssembly is supported.** One feature manifest identity is allocated in the family's table
  and declaration, and nothing has scored it: the specification's own conformance suite is not
  pinned, no harness reads it, and a run against modules this component wrote is not a conformance
  result. A specification version name would not be a conformance claim either. *(Corrected
  2026-09-26: the first sentence read "allocated in the descriptor", which the universal bytecode
  programme's milestone UBC-4 deleted.)*
- **The admitted surface is wider than the manifest that names it, and that is a defect this ledger
  records rather than a scope note.** Roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
  defines `broiler.webassembly.slice` as one type, one function, one export, integer arithmetic,
  local access and structured control flow — no memory, no table, no global and no float. The
  decoder, the validator, the translator and the family's table admit more than that under the same
  manifest identity, so a module declaring the slice manifest and using a float is accepted here
  where section 6 says it must be refused at validation. **A manifest is refused and not degraded**,
  and per-manifest surface restriction is not implemented; the milestone that mints a second manifest
  owns closing it, and [WAD-0002](decisions/0002-the-family-table-stays-under-the-slice-identity.md)
  records that milestone UBC-4 kept the one table under the slice identity rather than minting that
  manifest. *(Corrected 2026-09-26: the fourth sentence read "The decoder, the validator and the
  interpreter admit more than that", and the interpreter it named was deleted at that milestone.)*
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
interpreter line and then writes the interpreter. *(Corrected 2026-09-26: the path's last step no
longer describes the tree. The interpreter was written under routes, and the universal bytecode
programme's milestone UBC-4 deleted it; the routes are recorded in WAD-0003, and what executes a
module is this profile's family under the universal bytecode's bytecode emitter. The path's order and
its end are unchanged.)*

**WA-4 is not on the path, and it is not deferred either — it is unschedulable.** Its first input
is a conformance suite revision nobody has retrieved, which section 3 records as an unopened
dependency rather than as a blocker, and retrieving it is a human action. **That is a cost the MVP
pays and not a saving it makes**, and it is worth stating in the direction that hurts: the delivery
order calls the WA-4/WA-5 fork the main structural advantage this profile has over a language with
no external oracle, because the malformed and invalid families can be scored before any interpreter
exists. An MVP that does not take that fork grades its verifier against its own corpus alone, and a
corpus this component wrote cannot find a rejection this component never thought of. *(Noted 2026-09-28: the
suite's first input exists now. The specification's core test scripts are pinned at `wg-1.0` in
`src/tests/wasm/spec`, retrieved on this profile owner's direction for the universal bytecode programme's
UBC-4 rule. The harness root reads them with its `--spec` lane, and bundle `ubc-4-005` retains two runs of
them. WA-4 itself is not started: the harness has no sharding, per-shard self-check, scope manifests,
per-family totals or failure queue, and nothing here advances its row.)* *(Noted 2026-09-29: the last sentence no longer holds. WA-4 is `In progress` since rule W7, its ingestion-path scan, exists. The harness still has none of the other machinery named there, and WA-4 is still not on the path.)*

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
  with a smaller name. *(Corrected 2026-09-28: a script reader exists, in the harness root and in no
  shipped image. It is the `--spec` lane, with its own text-module encoder, written for the universal
  bytecode programme's UBC-4 rule. The rule this bullet states - no text format in the profile - still
  holds.)*
- **No vectors, no garbage-collected type surface, no exceptions, no tail calls, no threads, no
  64-bit addressing, and no multiple memories.** Each has a manifest identity allocated or refused
  in roadmap
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) and none of them
  is minted here. Threads stay excluded by name for the reason
  [section 14](roadmap.md#14-suspension-threads-and-what-this-profile-does-not-declare) gives,
  which is not scope.
- **No second execution arm, no IL emission, no code generator, and no tiering.** The MVP has one
  interpreter and no promotion path, and there is no tier for a promotion to reach. *(Corrected
  2026-09-26: "The MVP has one interpreter" is false since the universal bytecode programme's
  milestone UBC-4. This profile has no interpreter of its own; its modules run on the universal
  bytecode's bytecode emitter, a loop every family shares, and the profile's translator lowers a
  module into that bytecode rather than into code. Nothing emits IL, no native form runs a module of
  this profile's — that is the programme's milestone UBC-6b — and there is still no tier for a
  promotion to reach.)*
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
entry-point encoding — and being code makes neither of them a decision. *(Corrected 2026-09-26: the
sixteen-byte slot of MVP-2 was code too, and is not now. The universal bytecode programme's milestone
UBC-4 deleted it with the interpreter; a value is an eight-byte word of the universal bytecode's word
plane, and [WAD-0003](decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)
records the slot's row as reversed rather than settled, the vector width being a question for that
bytecode's next format version. The same milestone took a fourth route, MVP-15 — the module as
source, which the paragraph under What now exists names — and it is code as well, and no more a
decision than the others.)*

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
| **The conformance suite revision has not been pinned**, and the licence and attribution consequences of ingesting it into this tree have not been confirmed. *(Noted 2026-09-28: the first half no longer holds. `test/core` of `WebAssembly/spec` is pinned at commit `977f970`, tag `wg-1.0`, in `src/tests/wasm/spec`, and the notice row landed in the same change. The release owner's confirmation that the core's third-party claim stays scoped is recorded there as owed, so the second half still holds.)* | Same. | WA-0 records the obligation, names its owner and names the release owner who co-signs it. **WA-4 resolves the commit and lands both the attribution row and the standing-claim confirmation**, in the change that first ingests a suite file, because a notice cannot carry forward content this tree does not hold *(corrected: [WAC-15](roadmap.corrections.md#wac-15))*. |
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

*(Noted 2026-09-28, and the paragraph above is kept as it was written.)* The conformance suite has been
pinned in `src/tests/wasm/spec` since 2026-09-28; the specification document has not. Runs of the suite are
retained in the universal bytecode programme's bundles `ubc-4-005` and `ubc-4-006`, and in
[record wa-spec-001](../../../docs/evidence/wa-spec-001/README.md). None of them is a conformance result under this ledger, because none is a WA-4 bundle.
