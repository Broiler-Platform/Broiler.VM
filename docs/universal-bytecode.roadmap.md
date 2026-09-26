# The universal bytecode programme — roadmap

**Owner:** the Broiler.VM architecture owner, who also holds the release, security, contract-minting
and review roles [ADR 0012](adr/0012-security-ownership-and-support-matrix.md) names — six roles held
by one person, recorded as EX-30. Every owner role named below is that person until a second holder
is named, and no gate below is independently confirmed until then.

**Date:** 2026-09-25. **Core contract version:** 1, unchanged by anything here.

**Status:** planned work and objective exit gates for
[the universal bytecode concept](universal-bytecode.md). **Nothing here is done, and nothing here is
accepted.** [The programme's ledger](universal-bytecode.status.md) is the only authority on what has
happened; this document is the authority on what is planned and on what would count. Every milestone
below is `Not started` on this document's date *(corrected 2026-09-25, later the same day: milestone
UBC-0 moved once its records were filed, and [ADR 0013](adr/0013-the-universal-bytecode-extraction-record.md)
accepts candidate A and notes that G1 is unsatisfied for the native-form mechanism, which keeps UBC-3 and
UBC-5 to UBC-10 from meeting their gates for the reason the ledger gives; the ledger is the authority on
every row, and no plan below is edited by this note)*.

**What this document is.** The concept says what the universal bytecode, the emitter profiles and the
language profiles are and why; this roadmap says in what order they are built, by which work packages,
what each delivers, what each retains as evidence, and what a run decides before a milestone's gate is
met. It promotes the concept's stages `UBC-0` to `UBC-10` to milestones with a ledger row each, which
is the act the concept said it would not perform and this document does perform, on the request
recorded in section 1. **The component split is optional, is the last milestone, and nothing before it
depends on it** (section 1.3).

**What this document is not.** It is not the ledger and moves no row in one; it is not a decision, and
every record a milestone files is named as a deliverable and not written here; it carries **no figure of
any kind** — no timing, no count of a suite, no ratio — under the rule every plan in this repository
writes for itself; and it makes **no speed claim** in any direction. It plans against
[`docs/mvp.md`](mvp.md): approval of records, human review, evidence acceptance and the amendment
procedure's co-signing stay deferred, and the automated gates, the status vocabulary, the stop condition
on an untruthful support claim, the non-advertisement of every composition with the three-package pack
set, and the prohibition on publishing stay in force at every milestone.

---

## Contents

1. [Scope: what this roadmap plans, and what it leaves alone](#1-scope-what-this-roadmap-plans-and-what-it-leaves-alone)
2. [How to read a milestone](#2-how-to-read-a-milestone)
3. [The delivery order and the critical path](#3-the-delivery-order-and-the-critical-path)
4. [What every milestone owes before it lands](#4-what-every-milestone-owes-before-it-lands)
5. [Milestone UBC-0 — the extraction record and the records that move](#5-milestone-ubc-0--the-extraction-record-and-the-records-that-move)
6. [Milestone UBC-1 — `Broiler.VM.Ubc`](#6-milestone-ubc-1--broilervmubc)
7. [Milestone UBC-2 — `Broiler.VM.Emitter.Bytecode` and the fixture family](#7-milestone-ubc-2--broilervmemitterbytecode-and-the-fixture-family)
8. [Milestone UBC-5 — `Broiler.VM.Ubc.Native`, the `x86` pivot and the execution half](#8-milestone-ubc-5--broilervmubcnative-the-x86-pivot-and-the-execution-half)
9. [Milestone UBC-6a — the `x86-64` emitter over the fixture family](#9-milestone-ubc-6a--the-x86-64-emitter-over-the-fixture-family)
10. [Milestone UBC-3 — the JavaScript family](#10-milestone-ubc-3--the-javascript-family)
11. [Milestone UBC-4 — the WebAssembly family](#11-milestone-ubc-4--the-webassembly-family)
12. [Milestone UBC-6b — the language families in the `x86-64` form](#12-milestone-ubc-6b--the-language-families-in-the-x86-64-form)
13. [Milestone UBC-7 — the `arm64` emitter, emitting-only](#13-milestone-ubc-7--the-arm64-emitter-emitting-only)
14. [Milestone UBC-8 — the polyglot composition](#14-milestone-ubc-8--the-polyglot-composition)
15. [Milestone UBC-9 — records, the support table and packability](#15-milestone-ubc-9--records-the-support-table-and-packability)
16. [Milestone UBC-10 — the component split, optional and last](#16-milestone-ubc-10--the-component-split-optional-and-last)
17. [Decision points the owner must take, and when](#17-decision-points-the-owner-must-take-and-when)
18. [The evidence and test matrix](#18-the-evidence-and-test-matrix)
19. [Rules minted or moved, by milestone](#19-rules-minted-or-moved-by-milestone)
20. [Records filed, by milestone](#20-records-filed-by-milestone)
21. [Risks and stop conditions](#21-risks-and-stop-conditions)
22. [The ledger, and how it is updated](#22-the-ledger-and-how-it-is-updated)

---

## 1. Scope: what this roadmap plans, and what it leaves alone

### 1.1 The request

"Create a fully detailed roadmap to achieve this concept. The component split is very optional and
should stay at the very end." The concept is [`docs/universal-bytecode.md`](universal-bytecode.md) as
revised on 2026-09-25, section 16 included.

### 1.2 What is planned

Everything the concept's section 13 names as a stage, promoted to a milestone: the extraction record
and the records that move (UBC-0); the shared format assembly (UBC-1); the bytecode emitter and the
fixture family (UBC-2); the native contracts, the `x86` pivot and the execution half (UBC-5); the
`x86-64` emitter (UBC-6, split here into 6a over the fixture family and 6b over the language families,
for the reason section 3 gives); the JavaScript family (UBC-3); the WebAssembly family (UBC-4); the
`arm64` emitter (UBC-7); the polyglot composition (UBC-8); the records, the support table and the
packability decision (UBC-9); and, optional and last, the component split (UBC-10). The concept's stage
numbers are kept as milestone numbers so that a reader can move between the two documents; the order
in which they are built is this document's and is not the numeric order.

### 1.3 The component split is optional

Milestone UBC-10 is reached only if the owner decides to split the repository, and no milestone before
it waits on it, refers to it as a precondition, or is shaped differently because it might happen. What
the earlier milestones do owe it is what they owe anyway: the assembly names the concept fixes, the
public-API baselines, and the universal bytecode contract version of UBC-9, each of which is worth
having in one repository. A programme that ends at UBC-9 is complete; UBC-10 is a further step, not an
unfinished one.

### 1.4 What is left alone

- **The language surfaces.** Nothing here makes the JavaScript profile compile more JavaScript or the
  WebAssembly profile admit a further instruction family; those are the workload and parity roadmaps'
  (`JSW-n`, `JSP-n`) and the WebAssembly plan's (`WA-n`), and a milestone here that widened either
  would be conflating two programmes.
- **Performance.** No milestone is justified by a speed, none measures one as a gate, and none may
  state one. Where a milestone produces a form that invites a number, the bundle's README is the only
  place a figure goes, behind a predeclared rule, and the ledger prints none.
- **Review, acceptance, advertisement, packaging, publication.** Deferred under `docs/mvp.md` and
  unchanged by any milestone here, with the one packability decision UBC-9 names as a decision rather
  than performs.
- **The core contract.** No milestone amends it. The one reading the concept takes of ADR 0011's P1
  is route UBC-R1, and UBC-0 either files it or is `Blocked` naming the core as holder.

---

## 2. How to read a milestone

Each milestone states the same fields, in the same order, and a field that is empty says so.

| Field | Meaning |
|---|---|
| **Objective** | One sentence: what is true after the milestone that was not true before. |
| **Owner roles** | The ADR 0012 roles that own the milestone; all one person today. |
| **Waits on** | Milestones whose exit gates must be met first. A milestone may *start* before its predecessors are accepted (ledger update rule 8: human review gates a release, not a development step), and it may not *meet its gate* before they meet theirs. |
| **Work packages** | Numbered `UBC-n.m`, each with what it builds or moves, where in the tree, and its own done condition. A work package is a unit of a change, not of time; nothing here has a duration. |
| **Deliverables** | The code, records, rules and evidence the milestone leaves behind, by name. |
| **Exit gate** | Numbered conditions a run can decide. A clause is met or it is not; a milestone with an unmet clause is `In progress`, however complete the rest. |
| **Evidence bundle** | The bundle directory under `docs/evidence/`, and what it retains. A bundle is the milestone's evidence and never the ledger's verdict. |
| **Does not do** | What a reader might infer and may not. |

**Identifiers.** Milestones are `UBC-n` (with `UBC-6a` and `UBC-6b`); work packages `UBC-n.m`; evidence
bundles `ubc-n-nnn` under `docs/evidence/`, in the core's bundle shape (a `README.md` that states what
the bundle demonstrates and what it excludes, a `manifest.json` naming every input by hash, and the
retained logs); decision points `UBC-D-n` (section 17); rules `Un` in the architecture register; routes
`UBC-Rn` as the concept names them, filed as `MVP-` rows when taken. No identifier from the `VM-`,
`JS-`, `JSB-`, `JSV-`, `WA-` or `MVP-` series is minted here.

---

## 3. The delivery order and the critical path

**The order is chosen so that no capability the tree has today is absent between two milestones.**
The concept's dependency list would allow the JavaScript family to be cut over to the universal bytecode
before any native emitter exists, which would leave the tree with no `x86-64` form from that milestone
until the emitter lands. This roadmap builds the emitter machinery over the fixture family first, and
cuts the JavaScript family over only when the bytecode form, the handler-call mechanism and the
`x86-64` encoder all exist for it to land on. That is why UBC-5 and UBC-6a precede UBC-3, and why UBC-6
is split: 6a is the emitter proved on a family that is not a language; 6b is the language families in
that form, once they exist.

```text
UBC-0  the extraction record, the records that move, the predeclared parity rules
  │
UBC-1  Broiler.VM.Ubc: format, common family, table schema, primitive table, walk, corpus
  │
UBC-2  Broiler.VM.Emitter.Bytecode and the fixture family: the bytecode form runs
  │
  ├──────────────────────────────────────────────────────────────┐
UBC-5  Broiler.VM.Ubc.Native, Broiler.VM.Emitter.X86.Format,     │
       Broiler.VM.Emitter.X86.Execution (the MachineCode         │
       project renamed): the native mechanism, one arming path   │
  │                                                              │
UBC-6a Broiler.VM.Emitter.X86 over the fixture family:           │
       templates, scan, re-emission, E1–E9 on a non-language     │
  │                                                              │
  ├──────────────────────┐                                       │
UBC-3  the JavaScript    UBC-4  the WebAssembly family           │   (UBC-4 waits on UBC-2 only and
       family: bytecode         (translator, tables, handlers,   │    may run beside UBC-5/6a/3; it
       form, numeric and        harness, parity)                 │    is drawn here where its gate
       baseline forms           │                                │    is most likely to be met)
       recovered, old           │                                │
       pipeline retired         │                                │
  │                             │                                │
  └──────────┬─────────────────┘                                │
UBC-6b the language families in the x86-64 form: whole-suite   │
       runs in both forms, forms comparison, the JS checks      │
       lane's native rows restored                              │
  │                                                             │
UBC-7  Broiler.VM.Emitter.Arm, emitting-only                    │
  │                                                             │
UBC-8  the polyglot composition: one interpreter, two families ◄┘ (needs UBC-3 and UBC-4)
  │
UBC-9  records, the support table's emitter table, packability, the universal bytecode contract version
  │
  ╎ (optional, only on the owner's decision UBC-D-6)
UBC-10 the component split
```

**The critical path** is UBC-0 → UBC-1 → UBC-2 → UBC-5 → UBC-6a → UBC-3 → UBC-6b → UBC-8 → UBC-9.
UBC-4 is off the critical path until UBC-8 and may be built in parallel with UBC-5 through UBC-3 by an
owner who has the hands for it; UBC-7 depends on UBC-6b only for the pivot's shape having met a second
family's tables and may be started earlier as an encoder over the fixture family. **What can start
today** is UBC-0 in whole and UBC-1's shells; nothing else, because every later milestone consumes a
type UBC-1 defines.

**A refusal at UBC-0 ends the programme with the record filed**, and that is a passing gate for UBC-0
and the whole of what the programme then delivers.

---

## 4. What every milestone owes before it lands

These are not gate clauses; they are the repository's standing rules, restated so that a milestone
cannot be reported met while owing one of them.

1. **Assurance.** Every new or moved code unit carries the two-line assurance annotation; the generator
   regenerates the file headers, `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and `assurance.manifest.json`
   in the same change, and rules J1 to J12 pass. Every unit is `HUMAN_PENDING`; moving a unit is not
   reviewing it.
2. **Rules.** A rule minted or revised carries a witness watched failing when injected and passing
   after revert, and its register row names the witness. A rule that moves a statement (B5c, X1) moves
   the statement and the evidence line together.
3. **Graph.** `graph.manifest.json` and ADR 0001's budget sentence say what the tree holds, in the same
   change that changes the tree (rules A7, A15), by a dated ADR 0001 revision with the superseded counts
   quoted.
4. **Corrections.** A plan sentence the change makes false is corrected where the plan's discipline
   says: inline with the superseded text quoted in the core documents; a `JSC-nn` or `WAC-nn` entry with
   a bare pointer in the profile plans.
5. **Ledger.** The milestone's row in [the programme ledger](universal-bytecode.status.md) moves in the
   same change, under that ledger's update rules; no other ledger's row moves because of it (core ledger
   update rule 6, in both directions).
6. **Figures.** No count, timing, ratio or score enters any plan, ledger, decision or support document.
   A bundle README may hold a figure the bundle retains; nothing else may quote it.
7. **Claims.** No milestone adds a runtime identifier, a supported form, an advertised composition, a
   package or a review decision. The support table moves only on retained evidence and only to say what
   a cell already may.
8. **Publish and run.** A milestone that adds or changes a composition root publishes and runs it under
   JIT, trimming and Native AOT and retains the closure read off the published output; a linker
   annotation is not a run.

---

## 5. Milestone UBC-0 — the extraction record and the records that move

- **Objective.** Section 8's extraction gate is invoked with real merged code on both sides, the
  verdict is filed, every record the concept asks to move at this point is moved or its refusal
  recorded, and the comparison rules every later milestone will be judged by are committed before any
  code exists that could shape them.
- **Owner roles.** The core architecture owner, with the JavaScript and WebAssembly profile owners
  supplying their halves (a profile supplies file paths, revisions and a correspondence table and
  records no verdict; the verdict is the core's).
- **Waits on.** Nothing.

### Work packages

- **UBC-0.1 — The correspondence table.** A table, one row per mechanism, naming the JavaScript
  implementation and the WebAssembly implementation by file and member at a stated source revision,
  and what the language-free part is. The rows: the artifact framing and section walk (`JsVerifier`
  section dispatch ↔ `WasmDecoder`'s section reader); the code walk with height and target checks
  (`JsVerifier`'s instruction pass ↔ `WasmValidator`'s typed stack walk and sealed jump table); the
  dispatch loop (`JsEngine.ExecuteCore<TMode>` ↔ `WasmInterpreter`'s switch over `code[pc]`); the frame
  model (the JavaScript heap frame list ↔ `WasmFrame` and the shared operand stack); unwinding (exception
  regions and the catch-filter search ↔ traps as `WasmRunStatus` return codes); the charge and poll sites
  (`Charge` before each arm ↔ `WasmPacing`); the constant and immediate decoding; and, for one side
  only, the native-form machinery (`JsNativeActivation`, `JsBaselineHandlers`, `JsNativeScan`,
  `JsNativeTemplates`) against nothing on the other side — stated as one implementation, which is what
  G1 asks about and what the verdict must weigh. *Done when* the table is in ADR 0013 with revisions
  named and every row's "shared part" column written without a language identifier.
- **UBC-0.2 — ADR 0013, the extraction record.** Filed as `docs/adr/0013-...md`, header
  `**Core contract:** not contract-bearing`, status `Proposed`, carrying ADR 0011's six record items:
  the two implementations with paths and revisions; the measured duplication as the table of 0.1; the
  proposed public surface of `Broiler.VM.Ubc` written with no identifier from any language's vocabulary
  (the concept's section 5 and 7.1 types, listed by name); the resulting graph edges demonstrating G4
  (the concept's section 4); the two named consumers; and the verdict per condition G1 to G4 with its
  date and deciding owner. The record also carries the universal bytecode's *reopening* of ADR 0011's
  standing refusal of a shared opcode set as a new dated verdict, and explains the 2026-09-18 extraction
  of the arming path into the MachineCode project, which the composition register says no record
  explains. The ADR index gains the row; rules E2 and E4 pass. *Done when* the record is filed with a
  verdict, accept or refuse, on every condition.
- **UBC-0.3 — The core documents that move.** Through ADR 0003 section 11's roadmap-amendment
  register: roadmap section 1's non-goal "one universal opcode set, tagged value, or frame ABI shared
  across languages" (revised to refuse a shared value and frame ABI and to admit a shared encoding
  whose meanings are owned); section 8's candidate-table row on a shared opcode set and its extraction
  register; section 10's "a backend is a choice inside one lowering" (gains the sentence that the
  choice may leave the lowering for an emitter family); section 16's first risk row. ADR 0011 gains an
  editorial pointer paragraph beside its standing-refusals table naming ADR 0013 as the new dated
  verdict, without editing the table's decision text. The README's component-boundary sentences gain a
  dated correction. *Done when* each superseded sentence is quoted beside its replacement and the
  amendment register carries each with status `Applied`.
- **UBC-0.4 — The profile plans, notified and not yet amended.** The JavaScript plan's section 9
  intermediate-form sentence is discharged by **JSD-0036**, a dated decision that the universal bytecode
  is the back-end-neutral form the plan promised, closing JSB-3's open clause; the JavaScript non-goals
  gain the "a document proposing … stands beside this plan; naming it here schedules nothing" paragraph
  the backend roadmap already received on 2026-09-07. The WebAssembly plan's non-goals gain the same
  kind of paragraph — a concept proposing a translator stands beside the plan; its compiler, framing and
  second-execution-arm entries stand until the milestone that takes them (UBC-4) — and no `WAC-nn`
  correction is filed here, because nothing in that tree has changed yet. *Done when* JSD-0036 is in
  the JavaScript decision index and both plans carry the paragraph.
- **UBC-0.5 — The routes taken here.** Routes UBC-R1 (P1's reading) and UBC-R2 (the primitive table as
  mechanism) are taken by filing the record, and are filed as `MVP-` rows in `docs/mvp.md` section 5 in
  the same change, in that record's four-column shape. *Done when* both rows exist with their settling
  conditions and deciders.
- **UBC-0.6 — The verdict's consequence, either way.** On accept: nothing further here. On refuse of
  any condition: the record states which and what would reopen it, each duplicated implementation gains
  a source-level pointer to the record, and the programme ends with UBC-0's gate met and every other
  milestone's row `Not started` with the reason. *Done when* the ledger says which.
- **UBC-0.7 — The predeclared comparison rules.** Three `decision-rule.md` files, committed before any
  code of the milestone they judge exists, each holding no figure: `docs/evidence/ubc-3-001/` — the
  JavaScript parity rule: the whole pinned `tc39/test262` suite under the wide manifest, driven by
  `eng/run-test262.py` against the retained pin, compared per variant between the last run of the old
  interpreter at the cut-over's base commit and the first run of the universal bytecode form, with the
  admitted divergence classes named (a refusal the new walk gives where the old verifier admitted a
  malformed artifact the corpus re-base records; a variant the re-based ratchet retires by name with its
  reason) and every other difference a regression; the fifteen pinned Octane benchmarks must report a
  score in the new form and the rule states that no score is compared; the fuel-parity twins must give
  one verdict at every ceiling tried. `docs/evidence/ubc-4-001/` — the WebAssembly parity rule: the
  specification's test suite through the harness, per assertion, before and after the translator, with
  the float-comparison rows named as the one class expected to *change*, from defect to answer. And
  `docs/evidence/ubc-6b-001/` — the native-form parity rule: both suites in the bytecode form and the
  `x86-64` form under JSD-0025 section 11's three admitted classes, and obligation E2's primitive
  differential corpus with no admitted difference at all. *Done when* the three files exist at commits
  earlier than the first code commit of UBC-3, UBC-4 and UBC-6b respectively, which the bundles will
  name.

### Deliverables

ADR 0013; JSD-0036; the roadmap-amendment register entries; the README correction; the two plan
paragraphs; two `MVP-` rows; three decision-rule files; the ledger's UBC-0 row.

### Exit gate

1. ADR 0013 exists, is not contract-bearing, carries the six record items and a per-condition verdict,
   and rules E2 and E4 pass over the index.
2. The correspondence table names both implementations by file, member and revision, and no cell of
   its shared-part column contains a language identifier (checked by the same banned-vocabulary scan
   rule U2 will use, run over the record).
3. Every core sentence UBC-0.3 names is revised with the superseded text quoted, and the amendment
   register carries each as `Applied`.
4. JSD-0036 is filed and indexed, and JSB-3's State bullet in `roadmap.backends.md` names it as the
   dated decision its open clause asked for.
5. Both profile plans carry the "stands beside this plan" paragraph, and the WebAssembly corrections
   file carries no entry for this milestone.
6. `docs/mvp.md` section 5 carries the rows for UBC-R1 and UBC-R2.
7. The three decision-rule files exist and hold no figure, and each names the evidence class, the
   population, the admitted difference classes and the one decision it produces.
8. The ledger's UBC-0 row says `In progress` while any clause above is unmet and names the unmet
   clauses individually; on a refuse verdict it says so and every later row says `Not started` with
   the reason.

### Evidence bundle

`docs/evidence/ubc-0-001/`: `README.md`; `manifest.json` naming the two source revisions compared
and every file the record cites by hash; the correspondence table as retained; the run of the
banned-vocabulary scan over the record; the architecture-test run showing E2 and E4 green.

### Does not do

It writes no code under `src/`, mints no project, and moves no `JS-` or `WA-` row. It does not make
the extraction verdict *accept*: the verdict is the gate's to give, and a refuse is a met gate.

---

## 6. Milestone UBC-1 — `Broiler.VM.Ubc`

- **Objective.** The shared assembly exists, holds no language concept, reads and refuses its own
  format under every ceiling, and is pinned by a corpus of its own before any family exists.
- **Owner roles.** The core architecture owner with the security owner (the verifier is a parser over
  untrusted bytes, which is the security owner's subject).
- **Waits on.** UBC-0 with an accept verdict.

### Work packages

- **UBC-1.1 — The project shell and the graph.** `src/Broiler.VM.Ubc/Broiler.VM.Ubc.csproj`:
  references exactly `Broiler.VM.Abstractions` and `Broiler.VM.Binary`; `IsPackable=false` and no
  `PackageId` during the MVP; `ImplicitUsings` off and nullable on as `Directory.Build.props` sets;
  `AllowUnsafeBlocks` off (nothing here names a pointer). Listed in `Broiler.VM.slnx`;
  `graph.manifest.json` and ADR 0001's budget sentence revised. Rules **U1** (the reference set) and
  **U2** (no exported identifier from the banned vocabulary — `Js`, `JavaScript`, `Wasm`,
  `WebAssembly`, every family mnemonic prefix — and no family row) minted with witnesses. *Done when*
  A7, A15, U1 and U2 pass and U2's witness is watched.
- **UBC-1.2 — The container codec.** `UbcFormat` (magic `"BUBC"`, format version 1, the section
  kinds and field layouts of the concept's Appendix E); `UbcArtifactReader` over `VmBoundedReader`,
  charging `VerifierWork`, `AllocatedBytes`, `DeclaredCount`, `SectionCount`, `StructuralDepth` and
  `ArtifactBytes` through the family's declared limits **before** any allocation the count would
  justify, in the read-order recorder's canonical order; `UbcArtifactWriter`, deliberately unchecked
  so that a corpus author can write a malformed artifact, in the position `JsArtifactWriter` occupies
  today. *Done when* every section round-trips byte for byte and every framing refusal in the corpus
  (1.7) is produced.
- **UBC-1.3 — Slot types, shapes and descriptors.** `UbcSlotType` (`i32`, `i64`, `f32`, `f64`, `v`),
  `UbcOperandShape` (the closed list of 5.4, `U8U8` included), `UbcEffect` (fixed pops and pushes as
  typed lists, or `pops = operand + k`), `UbcTarget` (none, or a code target with the taken-edge
  adjustment), `UbcTrapCode` (the universal codes Appendix D's primitives raise, and the family-coded
  `trap` form). *Done when* each is exported, documented per member, and reached by a corpus entry.
- **UBC-1.4 — The common family.** `UbcOpcode` per Appendix A; `UbcOpcodes` — `Shape`,
  `OperandWidth`, `InstructionWidth`, `IsTerminal`, `HasCodeTarget`, `TryDescribe` — one table, the
  only place the walk, the interpreter and every encoder read the common family's effects (rule
  **U4**, with a witness that perturbs one row and watches the verifier and an encoder disagree).
  *Done when* every row of Appendix A is present with the effect the appendix states and no other row
  exists.
- **UBC-1.5 — The family table schema and the contracts.** `UbcInstructionRow`,
  `UbcInstructionTable`, `UbcInstructionKind` (`Dynamic`, `Primitive`, `Branch`, `Call`, `Suspend`,
  `Throw`), `UbcPrimitive` (Appendix D, with the NaN-canonicalisation flag on the table),
  `UbcFamilyRegistration` (identity, table per manifest, handler set, plane type, hook, payload
  factories, frame codec, the authored and built-against universal bytecode contract versions),
  `UbcFamilyDeclaration` (descriptor rows 1 to 3 and 8 to 30), `IUbcFamily` (static abstract members),
  `IUbcFamilyVerifier`, `IUbcValuePlane`, `UbcActivation`, `UbcStatus`, `UbcCallRequest`, the frame
  codec pair, `IUbcEmitter`, `IUbcFormVerifier`, `UbcEmitterSet`, and `UbcDescriptors.Build`. *Done
  when* the public surface is captured in `docs/ubc/api/public-api.txt`, compared in both directions
  by rule **U9**, and the fixture family of UBC-2 can be written against it with no internal access
  (rule A10 holds).
- **UBC-1.6 — The verifier walk.** `UbcVerifier : IVmProfileVerifier`, built by
  `UbcDescriptors.Build`, running the structural layer and the code walk of the concept's 6.1 and
  calling the family hook at every family instruction and family section; every refusal maps to one
  core reason and one universal diagnostic code; the codes are registered in
  `docs/ubc/diagnostics/registry.txt` under rule **U8** (the registry and the code vocabulary agree in
  both directions, every code maps to exactly one `VmReason`, every row is reachable from a named
  corpus entry — the shape N5 to N7 hold the JavaScript registry to). *Done when* every refusal the
  concept's 6.1 lists has a code, a reason and a corpus entry.
- **UBC-1.7 — The malformed corpus.** `src/tests/corpus/ubc-1/`: self-authored artifacts, each with
  its hash, expected outcome, reason and diagnostic code, in ADR 0011's published entry schema;
  control entries that verify; mutated-entry detection; a runner in the contract tests that replays the
  corpus and, from UBC-2, a replay across three publish modes with byte-identical failure-class tables.
  Every operand shape, effect form, target form, section, ceiling and walk refusal has an entry that
  exercises it. *Done when* the runner passes and rule U8's reachability clause is green.
- **UBC-1.8 — Determinism of the codec.** A check that every corpus control artifact and every sample
  artifact reads and writes to the same bytes, twice. *Done when* it is in the contract tests.
- **UBC-1.9 — The records.** ADR 0001's dated revision; `docs/ubc/api/public-api.txt`;
  `docs/ubc/diagnostics/registry.txt`; the ledger row; any correction the change makes necessary.

### Deliverables

The assembly; rules U1, U2, U4, U8, U9 with witnesses; the API baseline; the diagnostic registry; the
corpus and its runner; the ADR 0001 revision; bundle `ubc-1-001`.

### Exit gate

1. `Broiler.VM.Ubc` references exactly Abstractions and Binary (U1), exports no banned identifier and
   no family row (U2, witness watched), and its public surface equals `docs/ubc/api/public-api.txt` in
   both directions (U9).
2. The common family's every row is in the assembly with Appendix A's effect and read from one table
   (U4, witness watched).
3. The codec round-trips every corpus control and sample artifact byte for byte, twice.
4. The malformed corpus is retained with hash, expected outcome, reason and code per entry; the runner
   replays it to its recorded answers; a mutated entry is detected; every refusal of the concept's 6.1
   has an entry; and U8 holds the registry in both directions.
5. Every operand shape, effect-descriptor form and target-descriptor form has a corpus entry that
   exercises it.
6. ADR 0001's budget sentence, `graph.manifest.json` and the tree agree (A7, A15).
7. `CoreContractVersionTests` and every existing suite pass unchanged: the core's public API baseline
   (M1) has not moved.
8. The ledger's UBC-1 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-1-001/`: `README.md`; `manifest.json` naming the corpus manifest and every
artifact by hash; the corpus replay log; the architecture-test run; the round-trip check's log.

### Does not do

Nothing executes. No family exists. No descriptor is registered in any catalog, because the factory
has nothing to build one for. The assembly is not packable and the packability question is UBC-9's.

---

## 7. Milestone UBC-2 — `Broiler.VM.Emitter.Bytecode` and the fixture family

- **Objective.** The universal bytecode executes, over a family that is not a language, in one
  dispatch loop generic over the family, and the emitter obligations that need no encoder are
  demonstrated on it.
- **Owner roles.** The core architecture owner with the security owner.
- **Waits on.** UBC-1.

### Work packages

- **UBC-2.1 — The project shells and the rules.** `src/Broiler.VM.Emitter.Bytecode/` referencing
  Abstractions, Binary and Ubc; the fixture family `src/tests/Com.Example.Tally/` in the position the
  application-local consumer profiles occupy, referencing Abstractions, Binary and Ubc and nothing
  else (rule A13 revised to admit Ubc for a consumer family); the demonstration root
  `src/compositions/Broiler.VM.Composition.Ubc.Fixture/`. Rules A11 and A12 gain the
  `Broiler.VM.Emitter.<Architecture>` family pattern, keyed on the segment, under the same
  no-cross-family rule N2 states, with witnesses in both directions (an emitter referencing a language
  family; a language family referencing an emitter). *Done when* the three projects build, A7/A15 are
  revised, and the pattern's witnesses are watched.
- **UBC-2.2 — The interpreter.** `UbcInterpreter<TFamily>` where `TFamily : struct, IUbcFamily`:
  the common family's every row implemented in the loop; the word plane as a `ulong[]` and the value
  plane as the family's `IUbcValuePlane`; heap frames owned by the operation, one CLR frame regardless
  of guest depth, `CallDepth` charged per push and released per pop; per-row fuel from the table's
  cost before the row's effects, polls at the family's declared bound; `Primitive` rows executed from
  Appendix D's C# implementations with the family's NaN flag honoured; `Dynamic`, `Branch`, `Call`,
  `Suspend` and `Throw` rows dispatched to the family's handler through the static abstract member
  with the fixed signature; call requests performed as frame pushes; regions searched innermost first
  with `OnLand` called before the handler's first instruction; suspension captured through the family's
  codec; guest loads routed through the mediator to the same descriptor's verifier. *Done when* the
  fixture family's program corpus runs to its expected transcripts under JIT.
- **UBC-2.3 — The executor and the factory.** `UbcExecutor : IVmProfileExecutor` (Instantiate,
  Invoke, Resume, Unwind, mapping the interpreter's answers onto `VmExecutionStep` and never onto a
  core outcome); the bytecode emitter's `IUbcEmitter` (identity `bytecode`, version 1, emitting
  nothing) and `IUbcFormVerifier` (nothing to verify); `UbcDescriptors.Build` producing a descriptor
  the catalog admits, routing execution by the artifact's form and refusing at verification a form no
  composed emitter admits. *Done when* the catalog accepts the descriptor and the contract tests of
  UBC-2.7 pass.
- **UBC-2.4 — The fixture family.** One value type of one field; three rows — a `Dynamic` row that
  reads and writes the value plane, a `Primitive` row over `i64` words, a `Throw` row — plus the
  common rows; a hook that refuses one thing (an operand out of the family's declared range); a
  payload; a frame codec; a trivial lowering from a fixed program list into universal bytecode through
  `UbcArtifactWriter`; a program corpus with expected transcripts, including a recursion that exhausts
  `CallDepth`, a suspension and resumption, a region that lands, a trap, and a guest load answered by a
  provider the root registers. Beside it the **primitive input corpus** for obligation E2: for every
  entry of Appendix D, a retained set of inputs covering every NaN class, both signed zeros, every
  overflow and every trap edge, with the family handler's answer recorded — shared by every later
  emitter. And the **fuel-parity twins**: programs paired so that one verdict at every ceiling is
  expected. *Done when* the corpora are retained with hashes and the contract tests read them.
- **UBC-2.5 — The fixture composition.** The root composes the fixture family over the bytecode
  emitter; publishes and runs under JIT, trimming and Native AOT; retains its catalog table and its
  closure read off the published output; carries a register row (`demonstration`, profile
  `com.example.tally`, sibling `Broiler.VM.Ubc`, native execution `none`); replays the UBC-1 corpus
  across the three publish modes; and holds the two-profile hostile-neighbour test the roadmap's
  section 14 has asked for since VM-3 — the fixture family composed beside `Com.Example.Ledger` with
  adverse hard maxima, showing the neighbour's maxima do not reach it while its adopted defaults do.
  *Done when* rules K1 to K5 pass over the row and the three publishes run.
- **UBC-2.6 — Route UBC-R7's fold check.** On the JIT lane, the loop's disassembly for the fixture
  instantiation (`DOTNET_JitDisasm` over `UbcInterpreter<TallyFamily>.Run`) is retained and asserted
  to contain no type test on the family struct; on the Native AOT lane the same assertion is made if a
  disassembler over the published image is available in the lane, and otherwise the bundle names the
  AOT half as an exclusion. *Done when* the JIT half is asserted and the AOT half is asserted or
  excluded by name.
- **UBC-2.7 — Contract tests.** Every `VmExecutionStepKind` reachable from the fixture family;
  recursion refused as `ResourceExhaustion` naming `CallDepth` under every published mode rather than
  terminating the process; suspension round-trip restoring both planes; cancellation observed within
  the declared poll bound; a form the composition does not admit refused at verification by name; a
  guest-loaded artifact of another language refused as a provider breach; fuel parity across the twins.
  *Done when* they pass on the three publish modes.
- **UBC-2.8 — Records.** ADR 0001 revision; the register row; the ledger row; MVP rows for UBC-R3
  (the value plane by index) and UBC-R7 (the generic loop), taken here.

### Deliverables

Two product projects and one test family; the fixture composition root with its register row and
retained closure; the primitive input corpus and the fuel-parity twins; rule revisions A11, A12, A13
with witnesses; two `MVP-` rows; bundle `ubc-2-001`.

### Exit gate

1. The fixture root publishes and runs under JIT, trimming and Native AOT, and its closure read off the
   published output contains exactly its register row's assemblies with `Broiler.VM.Ubc` in the sibling
   cell (K1 to K5).
2. Obligations E1, E3, E4 and E6 hold over the fixture family's program corpus and twins on all three
   modes; E2's primitive input corpus is retained with the handler answers recorded (the check itself
   first has an emitter to run against at UBC-6a).
3. `UbcDescriptors.Build` yields a descriptor whose rows 1 to 3 and 8 to 30 equal the declaration's,
   rows 4 to 7 the universal bytecode's, and which the catalog admits; a descriptor built with a family
   whose built-against universal bytecode contract version differs from the assembly's is refused at
   catalog construction.
4. Guest recursion is refused naming `CallDepth` on every published mode; a suspension restores both
   planes; cancellation lands within the declared bound.
5. The UBC-1 corpus replays to its recorded answers across the three publish modes with byte-identical
   failure-class tables.
6. The two-profile hostile-neighbour test exists in the fixture root and passes.
7. Route UBC-R7's fold check: the JIT disassembly is retained and asserted; the AOT half is asserted or
   named as an exclusion in the bundle.
8. Rules A11, A12 and A13 are revised with witnesses watched in both directions; A7 and A15 agree with
   the tree.
9. The ledger's UBC-2 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-2-001/`: `README.md`; `manifest.json`; the three publish-and-run logs; the
catalog table and closure report per mode; the corpus replay logs per mode; the JIT disassembly of the
loop; the contract-test run; the hostile-neighbour test's log.

### Does not do

No language runs. No native form exists. The fixture family proves the contract and is deliberately
shaped to fit it (roadmap section 8's caveat about fixture agreement applies: it is evidence about the
universal bytecode's own tests and not about a language). Nothing is packable.

---

## 8. Milestone UBC-5 — `Broiler.VM.Ubc.Native`, the `x86` pivot and the execution half

- **Objective.** One activation, one handler-table mechanism, one template schema and one scan for
  every family, in the core's `Broiler.VM.Ubc.Native`; one arming path and one execution half, in the
  `x86` emitter family; and the existing MachineCode project renamed into that family with nothing lost.
- **Owner roles.** The core architecture owner with the security owner (the arming path, the wrappers
  and the thread-static slot are the security owner's subject).
- **Waits on.** UBC-2.

### Work packages

- **UBC-5.1 — `Broiler.VM.Ubc.Native`.** `src/Broiler.VM.Ubc.Native/`, referencing Abstractions,
  Binary and Ubc, `AllowUnsafeBlocks` on (it names pointer types in contracts and calls through
  unmanaged function pointers; it maps no memory). Holds: `UbcNativeFrame` (the handler table's
  address at offset zero, the activation cookie at offset eight, nothing else — `JsBaselineFrame`'s
  shape, generalised) and `UbcNativeAbi` (offsets and statuses: an offset is never negative, so
  `Exit`, `Threw`, `Taken`, `Request`, `Suspend`, `Defect` are the negative answers); the native
  activation (`UbcNativeActivation`: the family's activation state, the planes, the pc the managed side
  computed, the pending exception, the outcome) rooted by the managed entry's frame and by one
  thread-static slot written only by that entry for exactly the lifetime of one emitted call and read
  only after the cookie check — rule **U7**, a text rule over this assembly's source; the
  **slot wrappers**: two hundred and fifty-six `[UnmanagedCallersOnly]` static methods, one per handler
  table slot, each non-generic, each resolving the family from the activation and calling the family's
  handler through a function pointer the registration filled — the shape `JsBaselineHandlers` has,
  made family-free; the handler-table builder (`UbcHandlerTable`: unmanaged memory allocated once per
  (family, manifest) at registration, one wrapper pointer per defined row, the defect wrapper in every
  other slot, self-checked, never freed); the template-table schema (`UbcTemplate`: fixed bytes, and
  for each variable field the closed set of values it admits); the template-closure scan
  (`UbcTemplateScan`: `JsNativeScan`'s decoder generalised to any table of that schema, with the shape
  clauses S1 to S4 and the twelve named outcomes JSD-0025 and the backend roadmap list, and the
  handler-call clause that every indirect call is through the table's slot for a row of the unit's
  family); the form-verifier contract; and the native executor scaffolding (`UbcNativeExecutor`: the
  managed entry that enters a unit through an unmanaged function pointer, dispatches a status, lands a
  suspension through the codec, performs an unwinding by re-entering at a landing, and performs a
  call request through the direct-call protocol's `Prepare` and `Finish` helpers). Rules **U3**
  (no emitter-side source names a language, a family or an opcode of one — scoped to this assembly
  and every `Broiler.VM.Emitter.*` assembly) and **U7** minted; **X2** revised to every frame type this
  assembly declares; **X3** revised to name this assembly's wrapper file and nothing else; **X4**
  revised to say every family's wrappers keep the cookie, pc and opcode checks. *Done when* the fixture
  family's handler table builds and self-checks and the scan refuses every negative fixture of 5.4.
- **UBC-5.2 — `Broiler.VM.Emitter.X86.Format`.** `src/Broiler.VM.Emitter.X86.Format/`, referencing
  Ubc and Ubc.Native: the template tables per (form, emitter version) for `x86-64-sysv` and
  `x86-64-win64` — at this milestone the tables of the common rows, the handler-call template, the plane
  helper calls, the prologue and epilogue, the compare tree, the direct-call sequence and every
  primitive of Appendix D, written as tables and not yet emitted by anything; the two conventions'
  stack reservations and argument registers (`JsX64Abi`'s content, language-free); the frame
  reservations. *Done when* every template is a row of a table the scan can read and rule U3 passes
  over the assembly.
- **UBC-5.3 — `Broiler.VM.Emitter.X86.Execution`.** The existing `Broiler.VM.Profile.MachineCode`
  project renamed and re-homed: `VmNativePage` and its two platform halves kept as the arming path,
  unchanged in mechanism; `VmNativeFrame`, `VmNativeReturn` and `VmNativeArchitecture` retired in
  favour of `Broiler.VM.Ubc.Native`'s contracts; `MachineCodeProfile`, `MachineCodeFormat`,
  `MachineCodeVerifier`, `MachineCodeExecutor`, `MachineCodeArtifactWriter`, `MachineCodeProgram` and
  `MachineCodeReadAdapter` retired with the `"BMC\0"` form (nothing composes them; the concept's 2.3
  says why the form cannot be the native form); the assembly gains the `x86` native executor (an
  `IVmProfileExecutor` over `UbcNativeExecutor`, arming the Emission section at instantiation and
  refusing by name — `UnsatisfiedHostAssumption` — an architecture or convention the process is not)
  and the form verifier layer (structural, the scan over this family's tables in every image,
  re-emission where an encoder is composed). Rules **B5c** and **X1** revised to name this assembly and
  `VmNativePage`, which their tests already read; the three JavaScript roots' project references and
  register sibling cells renamed; the `JsNativePage.Mapper` hook left in place until UBC-3 retires it.
  *Done when* the renamed project builds in both solutions, K1 to K5 pass over the renamed rows, and
  B5c and X1's witnesses are watched under the new statements.
- **UBC-5.4 — The scan's negative fixtures.** For each of the scan's named outcomes, a byte string
  the scan must refuse: bytes belonging to no unit, overlapping units, a sequence matching no template,
  a sequence matching two, an operand outside its field's set, an instantiation crossing a unit's end, a
  branch leaving its unit, a branch into an instruction, an indirect call outside the handler table's
  slot rule, a unit whose last instruction is not a return, a byte in the alignment padding, a unit
  missing its prologue or epilogue, a branch into either, a template with a memory destination outside
  the word plane and the frame. Retained as corpus entries of the universal bytecode corpus with
  Emission sections, each with hash, expected outcome, reason and code (`NativePayloadNotTemplateClosed`
  re-minted under the universal registry). *Done when* the entries replay to their answers through the
  fixture root with no encoder in the image.
- **UBC-5.5 — Records.** ADR 0001 revision (the rename and the two new projects); the composition
  register's sibling cells; a `JSC-nn` entry recording that the JavaScript native machinery's home has
  moved (the plan's section 15 and `roadmap.backends.md` name it as the profile's); the MVP row for
  route UBC-R8 (the arming path stays with the `x86` emitter until a second executing emitter exists),
  taken here; the ledger row.

### Deliverables

`Broiler.VM.Ubc.Native`; `Broiler.VM.Emitter.X86.Format`; `Broiler.VM.Emitter.X86.Execution` (renamed);
rules U3, U7 minted and B5c, X1, X2, X3, X4 revised, each witnessed; the scan's negative fixtures in the
corpus; the ADR 0001 revision; one `MVP-` row; bundle `ubc-5-001`.

### Exit gate

1. `Broiler.VM.Ubc.Native` references exactly Abstractions, Binary and Ubc; `Broiler.VM.Emitter.X86.Format`
   exactly Ubc and Ubc.Native; `Broiler.VM.Emitter.X86.Execution` exactly Abstractions, Binary, Ubc,
   Ubc.Native and X86.Format (U1's second clause, and the emitter pattern's rules).
2. U3 passes over both new assemblies and the renamed one, with a witness watched; U7's text rule
   passes with a witness watched (a write of the slot outside the entry).
3. B5c and X1 name `Broiler.VM.Emitter.X86.Execution` and `VmNativePage` in their statements, their
   tests read the same, and their witnesses are watched under the new statements; X2, X3 and X4 are
   revised and witnessed.
4. Every ImplMap row naming a mapping, protection or instruction-cache entry point in every assembly a
   published image can contain is in `Broiler.VM.Emitter.X86.Execution` and nowhere else; every
   composition's native-execution cell is unchanged and K5 passes.
5. The scan refuses every negative fixture of 5.4 by its named outcome, through the fixture root with
   no encoder in its image, and accepts every control.
6. The `"BMC\0"` form, `MachineCodeProfile` and `JsNativeCompiler` are gone from the tree, and the
   contract test of the native pipeline seam is rewritten to route UBC-R5's reading (the seam remains;
   its implementation arrives at UBC-6a).
7. ADR 0001's budget sentence, `graph.manifest.json`, both solutions and the tree agree (A7, A15).
8. The ledger's UBC-5 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-5-001/`: `README.md`; `manifest.json`; the architecture-test run showing every rule
of clauses 1 to 4; the scan fixtures' replay log; the fixture root's publish-and-run logs after the
rename with closure per mode.

### Does not do

No byte is emitted and no page is armed for any new form: the execution half arms nothing until an
encoder exists (UBC-6a). The JavaScript profile's native forms keep running on their old pipeline
through the `JsNativePage.Mapper` hook until UBC-3. No language table exists yet. Route UBC-R8 is a
route: a second executing emitter reopens it.

---

## 9. Milestone UBC-6a — the `x86-64` emitter over the fixture family

- **Objective.** Universal bytecode of a family that is not a language is emitted as `x86-64` machine
  code under both conventions, verified in every image, armed, executed, and compared with its bytecode
  form; every emitter obligation is demonstrated before a language reaches the emitter.
- **Owner roles.** The core architecture owner with the security owner; the release owner for the
  register and the support-table row.
- **Waits on.** UBC-5.

### Work packages

- **UBC-6a.1 — `Broiler.VM.Emitter.X86`, the encoder.** `src/Broiler.VM.Emitter.X86/`, referencing
  Ubc, Ubc.Native and X86.Format, no unsafe code, no mutable static (the rule N12 states for the
  JavaScript lowering, restated for every emitter as part of U3's row): `X86Assembler`
  (`JsX64Assembler` made language-free: encodings, labels, rel32 patching); the emitter proper —
  for every unit a prologue and an epilogue from the pivot's templates, a compare tree over the unit's
  declared landings, a defect block; per row: common rows as templates over registers and native slots,
  `Primitive` rows as the primitive's template with its trap edge and, where the family's table says
  so, the NaN canonicalisation sequence, every other family row as `call qword [rbx + slot*8]` with the
  status dispatch (`test eax, eax; js leave`, then the compare tree), `v`-plane shuffles as plane helper
  calls, `call` and call requests as the direct-call protocol, `Suspend` and `Throw` answers as exits to
  the managed entry; both conventions from one table set. It implements `IUbcEmitter` (identity per
  convention, semantic version 1, `TryEmit` answering the whole artifact's Emission or a refusal naming
  a row) and `IVmNativeCompiler` under route UBC-R5's reading: universal bytecode in, the same artifact
  with an Emission section out, under the input profile's identity, refusing a request whose
  architecture names no form this emitter has. *Done when* every fixture program emits under both
  conventions and the emission is deterministic across two runs (E3).
- **UBC-6a.2 — The form verifier layer, wired.** `UbcDescriptors.Build` composed with the `x86`
  execution half runs the structural layer and the scan in every image, and re-emission when the
  descriptor is built with the encoder (`DescriptorReEmittingWith`'s shape, generalised into
  `UbcEmitterSet`). *Done when* a fixture root with the encoder verifies by re-emission and a root
  without it verifies by the scan alone, and both refuse the UBC-5.4 fixtures.
- **UBC-6a.3 — The fixture composition, native.** `Broiler.VM.Composition.Ubc.Fixture` gains the
  encoder and the execution half, declares `x86-64` in its register row, publishes and runs the fixture
  program corpus in both forms under JIT, trimming and Native AOT, and retains closures per mode. The
  scan is exercised in both directions in the root's checks: everything the encoder emits is accepted,
  every template of both tables is instantiated by some fixture program or named as unreached, and
  legal-but-unemitted byte strings (an indirect call through a register outside the slot rule, a
  `syscall`, an off-grid displacement, a materialised immediate no unit answers with) are refused by
  name. Golden rows pin every template's bytes as regression pins, in the shape the numeric form's
  golden rows have today. *Done when* K1 to K5 pass over the row and the three publishes run both
  forms.
- **UBC-6a.4 — Obligations E1 to E9 over the fixture family.** E1 (common rows' meaning in this form,
  by the fixture program corpus); E2 (every primitive's inline implementation against the family
  handler's recorded answers over the primitive input corpus, bit for bit, including every NaN, both
  signed zeros, overflow and trap edges — the check refuses on the first difference and names the
  primitive and the input); E3 (two emissions of one program compared); E4 (a fixture program with a
  row the emitter refuses yields a refusal naming the row and no partial emission — a negative control);
  E5 (X2 over `UbcNativeFrame`); E6 (the fuel-parity twins in both forms at every ceiling tried, one
  verdict); E7 (transcript equality between the two forms over the program corpus, with no admitted
  class for a family that loads nothing); E8 (6a.3); E9 (U3). *Done when* each is a named row of the
  fixture root's checks lane and the bundle retains its log.
- **UBC-6a.5 — The `eval`-shaped path.** A fixture program that requests a guest load in a native-form
  instance receives a provider-compiled artifact in the instance's form, verified by the same
  descriptor, armed and run; a provider answering the bytecode form to a native instance is an internal
  defect and never a fallback. *Done when* both cases are contract tests on all three modes.
- **UBC-6a.6 — Records.** The `x86` emitter family's ledger (its own `docs/` tree under
  `src/Broiler.VM.Emitter.X86/docs/`, in the shape the JavaScript family's has: a roadmap that points
  here, a status ledger with its own legend, a corrections file, an evidence directory) — because an
  emitter profile has "an identity, a ledger, a support-table row and a version" (concept 7.1); the
  support table's section 3a row for the `x86-64` forms updated to say what exists and that no runtime
  identifier is claimed; the register row; the ledger row; the contract test of the seam rewritten
  under route UBC-R5.

### Deliverables

`Broiler.VM.Emitter.X86`; the wired form layers; the fixture root in two forms with its register row;
the checks lane rows E1 to E9; golden rows; the emitter family's `docs/` tree; bundle `ubc-6a-001`.

### Exit gate

1. `Broiler.VM.Emitter.X86` references exactly Ubc, Ubc.Native and X86.Format, declares no mutable
   static, and passes U3.
2. E1 to E9 each hold over the fixture family under both conventions, each as a named checks-lane row
   with a retained log, E2 over the whole primitive input corpus with no admitted difference.
3. The fixture root publishes and runs both forms under JIT, trimming and Native AOT, with closures
   read off the published output matching its register row, which declares `x86-64`; a root composing
   the execution half and not the encoder verifies the same artifacts by the scan and refuses the
   UBC-5.4 fixtures.
4. The scan accepts everything both conventions emit, every template is reached or named unreached,
   and the legal-but-unemitted strings are refused by name; golden rows pin every template.
5. Re-emission equality holds across the fixture corpus with the encoder in the image.
6. The guest-load cases of 6a.5 pass on all three modes.
7. The emitter family's `docs/` tree exists with a ledger whose rows say what this bundle shows and
   nothing more; the support table's section 3a says no runtime identifier is claimed for either
   convention.
8. The ledger's UBC-6a row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-6a-001/`: `README.md`; `manifest.json`; the checks-lane log per convention; the E2
differential log; the scan's both-direction logs; the golden rows' log; the publish-and-run logs and
closures per mode; the guest-load tests' log.

### Does not do

No language is emitted: the emitter has met the common rows, the primitives and one family's handler
rows, and nothing else. No runtime identifier is claimed for either convention, and the support table
says so. No figure is stated: the fixture programs are not a workload and the bundle prints no timing.

---

## 10. Milestone UBC-3 — the JavaScript family

- **Objective.** The JavaScript lowering has one exit, the profile owns no loop, no walk and no
  format of its own, its three output forms are recovered on the universal bytecode and its emitters,
  and every verdict the bytecode form gave before it gives after.
- **Owner roles.** The JavaScript profile owner, with the core architecture owner for the descriptor
  factory and the security owner for the handlers' rooting.
- **Waits on.** UBC-6a (so that the numeric and baseline forms land in the same milestone that retires
  their old substrate), and UBC-0.7's JavaScript parity rule committed before this milestone's first
  code commit.

### Work packages

- **UBC-3.1 — Determinism first.** The wide lowering compiled twice over the whole wide and module
  corpora and compared byte for byte, in the slice-compiler root's checks lane where the slice front
  end's equivalent already runs; the global lexical hoisting sites' emission order derived from source
  order rather than a dictionary's; a negative control that perturbs the order, watched failing and
  passing after revert; a `JSC-nn` entry recording which half of the determinism claim was covered.
  This is JSB-1's gate, met here and recorded in JSB-1's State bullet. *Done when* the lane row is green
  and the control is watched.
- **UBC-3.2 — The base run, retained.** Before any other work package of this milestone changes the
  engine: the whole pinned suite under the wide manifest, driven by `eng/run-test262.py` at the base
  commit, its per-variant verdict file retained in `docs/evidence/ubc-3-001/` beside the decision rule
  UBC-0.7 committed there; the fifteen pinned Octane benchmarks run once at the base commit with their
  scores retained in the same bundle and compared by no clause; the fuel-parity twins' verdicts at the
  base commit. *Done when* the bundle names the base commit and every retained file by hash.
- **UBC-3.3 — The family tables, in the format assembly.** `JsFamilyTables`: the wide table (every row
  of Appendix B with its shape, effect, target, kind, cost and handler), the numeric table (the
  forty-seven admitted rows reclassified over word slots as Appendix B's last column says, the
  operator rows as `Primitive` over `f64`, comparisons over `[f64 f64] → [i32]`, the family's reserved
  word for `undefined`), the slice table (format version 1's thirty rows), the five surfaces as family
  data a composition declines at verification; the constant-pool codec, the scope maps, the module
  records, the script declarations, the referrers and the eval scopes as the JavaScript FamilyData
  section's schema; the format assembly's reference list becomes exactly `Broiler.VM.Ubc` (rule N3
  revised). *Done when* every `JsOpcode` and `JavaScriptOpcode` member has a row or a recorded reason
  for its absence, and U9-style baselines cover the family's public surface (rule N10 re-based).
- **UBC-3.4 — The lowering's one exit.** `JsCompiler.Assemble` emits Units, Types, Code, JumpTables,
  Regions, Entries, Positions and the JavaScript FamilyData through `UbcArtifactWriter`; per-manifest
  emission choices in the front end (word locals and `const.f64` under the numeric manifest, where the
  numeric admission has proved a binding local; `js.load_undefined` before `return` where
  `ReturnUndefined` was written); `JsOutputForm` leaves the compile request; the three native encoders,
  `JsNativeBackend`, `JsNativeCompiler` and every `JsX64*` and `JsArm64*` file leave the Compiler
  project (their language-free content arrived in the `x86` family at UBC-5 and UBC-6a; the `arm64`
  encoder's content is parked for UBC-7 in the bundle rather than deleted from history); rules N12 and
  N19 hold unchanged; the slice front end (`SliceSourceCompiler`) emits the slice table's rows with
  absolute targets. *Done when* every source of the wide, module, numeric and slice corpora lowers to
  an artifact the universal walk and the family hook admit.
- **UBC-3.5 — The handlers.** `JsFamily : IUbcFamily`: the value plane as a `JsValue[]`; the handler
  dispatch as a static method switching on the family opcode byte into the arms of
  `JsEngine.ExecuteCore<TMode>`, moved as text, with the interpreter's ambient state — the frame list,
  the scope list, the pending exception, the realm, the program — re-homed in the family's activation
  state; `Call`-kind handlers answering a call request for a script function of the same program and
  completing the call themselves for an intrinsic, a host function, a bound function or a proxy;
  `Branch`-kind handlers answering taken or not; `Suspend`-kind handlers answering a continuation for
  the generator object or the promise reaction; `Throw`-kind handlers answering `Threw` with the value
  in the activation; `OnLand` pushing the exception for a catch region and the completion record for a
  finally region; the frame codec over the family's records for generators and async functions; the
  payload factories (`JsCompletion`, `JsUncaught`, `JsPause`, the trap payload). The interpreter's loop,
  frame list, region search and charge sites are dropped, not moved. *Done when* the fixture-style
  contract tests of UBC-2.7 pass over the JavaScript family and the family's own contract tests pass.
- **UBC-3.6 — The hook.** `JsFamilyVerifier : IUbcFamilyVerifier`: constant-pool tags against the
  manifest and the surfaces, scope-map depths, unit flags, module and import tables, eval scopes,
  the numeric table's admissions (a numeric artifact using an absent row is refused by the walk before
  the hook sees it), each refusal mapped to a `JavaScriptDiagnosticCode` and a core reason under rules
  N5 to N8. *Done when* every refusal the old `JsVerifier` gave that is the family's to give has a hook
  refusal with the same code, and every refusal that is now the walk's is recorded in the corpus
  re-base of 3.8.
- **UBC-3.7 — The runtime library on the new mechanisms.** Calls through call requests, generators
  and async functions through the codec, regions through `OnLand`, `eval`, the `Function` constructor
  and dynamic and static imports through guest loads compiled in the instance's form, host realms and
  the capability imports carried by the family declaration, the module graph and the job queue on the
  family's instance state. *Done when* the whole pinned suite runs in the bytecode form (3.9).
- **UBC-3.8 — The corpus re-base and the registry.** Every retained JavaScript malformed-corpus entry
  re-authored onto the universal container with its old answer carried, or its new answer recorded
  beside the reason (a framing refusal that is now the walk's gets the universal code; a language
  refusal keeps its family code); rules N5 to N8 held over the result in both directions; rule N16
  retired with its reason recorded (the family has no format version of its own; rule **U5**, minted
  here, binds a family table version to the manifest that selects it and requires one walk to read
  every table). *Done when* N5 to N8 and U5 pass and every old entry is accounted for.
- **UBC-3.9 — Parity.** The whole pinned suite under the wide manifest in the bytecode form on the
  universal bytecode, compared per variant against the base run of 3.2 under the predeclared rule of
  UBC-0.7; the ratchet under `src/tests/conformance` re-based by hand with retired rows and reasons
  written beside them; the fifteen Octane benchmarks reporting a score in the new form; the fuel-parity
  twins giving one verdict at every ceiling tried, compared with the base commit's; the numeric and
  baseline forms emitted by the `x86` emitter from the wide and numeric tables, verified by the scan and
  by re-emission, armed and run through the JavaScript CLI's native option, with the checks-lane rows
  for the native forms restored over the universal container (the forms comparison of 6b is where the
  whole suite runs natively). *Done when* the rule's verdict is met with every difference in an admitted
  class named in the bundle.
- **UBC-3.10 — Retirement.** Format versions 1 and 2, `JsFormat`, `JavaScriptFormat`, both artifact
  writers, `JsVerifier`, `JavaScriptVerifier`, `JsEngine`'s loop and `ExecuteCore<TMode>`'s modes,
  `JsEngine.Baseline`, `JsBaselineHandlers`, `JsValueHelpers`, `JsNativeActivation`, `JsNativePage`,
  `JsNativeExecution`, `JsNativeAbi`, `JsBaselineBlocks`, `JsValueLayout`, `JsValueFrame`, `JsWord`,
  `JsHandleTable`, `JsValueSlab`, `JsValueStack`, `JsValueWindows`, `JsWordCodec`, `JsWordChecks`, the
  handle-stress descriptor, and the value form's opt-in options are removed; `JavaScriptProfile`'s
  descriptor and its variants become `JavaScriptFamily.Registration` and `.Declaration` with the
  variants as declaration parameters; the JavaScript roots build descriptors through
  `UbcDescriptors.Build`; the `JsNativePage.Mapper` hook and every root's filling of it are gone; rule N1
  is revised to Abstractions, Binary, Ubc and the format. **This work package is gated on decision
  UBC-D-1** (section 17): the owner's ruling of 2026-09-25 keeps the value form in the tree as an
  unadopted opt-in form, and retiring its substrate reverses that ruling. Until the ruling is given the
  old pipeline stays in the tree behind its opt-in roots and the milestone is `Blocked` on this clause
  naming the owner as holder. *Done when* the retired files are gone and `HUMAN_REVIEW.md` is
  regenerated.
- **UBC-3.11 — Records.** JSD-0036 amended with what landed; `JSC-nn` entries for every plan sentence
  the milestone makes false (the profile's section 5 package boundaries, section 7 format, section 10
  execution, section 15's execution-only label, `roadmap.backends.md`'s stages JSB-1, JSB-3, JSB-4,
  JSB-5, JSB-11 closed or superseded by name, `roadmap.gates.md` where it names the interpreter); the
  JavaScript ledger's rows recording the observed repository state under that ledger's own legend and
  never a state this ledger gives; the MVP rows for routes UBC-R4's sibling (none here) and UBC-R6 (one
  call per `Dynamic` row, taken when the baseline form is recovered); the programme ledger's row.

### Deliverables

The three JavaScript assemblies on the universal bytecode; the corpus re-base; the ratchet re-base with
its retired rows; the parity bundle; the retired pipeline; rules N1, N3, N10 revised, N16 retired, U5
minted; JSD-0036 amended and the `JSC-nn` entries; bundles `ubc-3-001` (the base run and the rule) and
`ubc-3-002` (the cut-over run and the comparison).

### Exit gate

1. Rule N1's list is Abstractions, Binary, `Broiler.VM.Ubc` and the format; N3 reads exactly
   `Broiler.VM.Ubc`; N12, N19, N20 and N21 hold unchanged; N16 is retired with its reason and U5 passes
   with a witness.
2. The wide lowering is deterministic over the whole wide and module corpora, compiled twice and
   compared byte for byte, with the hoisting control watched (JSB-1's gate).
3. Every `JsOpcode` and `JavaScriptOpcode` member has a row of Appendix B's tables in the format
   assembly or a recorded reason for its absence; the family's public surface equals its re-based
   baseline in both directions (N10).
4. The retained corpus is re-based entry by entry, each old answer carried or its new answer recorded
   with a reason, and N5 to N8 hold in both directions.
5. The whole pinned suite under the wide manifest, driven by `eng/run-test262.py`, gives per variant
   the verdict the retained base run gives, outside the classes the predeclared rule admits, each
   difference named in the bundle; the ratchet is re-based by hand with retired rows and reasons.
6. The fifteen pinned Octane benchmarks report a score in the new form; no score is compared and none
   is printed anywhere but the bundle.
7. The fuel-parity twins give one verdict at every ceiling tried, equal to the base commit's.
8. The numeric and baseline forms are emitted from the numeric and wide tables by the `x86` emitter,
   verified by the scan in every image and by re-emission where the encoder is present, armed and run
   through the JavaScript CLI under both conventions where the host allows, and the checks-lane rows for
   the native forms are restored over the universal container.
9. The execution-only root's closure holds no compiler, no encoder and no arming path, and its
   register row declares `none`; the compiler-bearing roots' rows declare `x86-64` and their closures
   match.
10. The retired files of 3.10 are gone, `JsNativeCompiler` and every `JsNativePage` hook with them, and
    `HUMAN_REVIEW.md` is regenerated — or the row says `Blocked` on decision UBC-D-1 naming the owner.
11. The Android head builds in the mobile solution.
12. The ledger's UBC-3 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-3-001/`: the decision rule; the base run's per-variant verdict file, its Octane
scores and its twins' verdicts, the base commit named by hash. `docs/evidence/ubc-3-002/`: the
cut-over run's verdict file; the comparison per the rule with every difference classed; the ratchet's
retired rows; the Octane run's scores; the twins' verdicts; the checks-lane log with the native rows;
the corpus re-base log; the publish-and-run logs and closures of every JavaScript root per mode.

### Does not do

It compiles no more JavaScript than the day before: every construct the front end refused it still
refuses, and the workload and parity roadmaps own the rest. It claims no runtime identifier and prints
no figure. It does not carry the value form: JSD-0011 Row 1's "registered and not adopted" stands, and
whether the form's substrate may be retired is the owner's ruling and not this milestone's.

---

## 11. Milestone UBC-4 — the WebAssembly family

- **Objective.** The WebAssembly profile decodes, validates and translates a module to universal
  bytecode, owns its instruction family, executes through the emitter, and gives the specification's
  test suite the verdicts it gave before — with one named class that changes from a defect to an answer.
- **Owner roles.** The WebAssembly profile owner, with the core architecture owner for the descriptor
  factory and the security owner for the memory representation.
- **Waits on.** UBC-2; independent of UBC-5, UBC-6a and UBC-3. UBC-0.7's WebAssembly parity rule
  committed before this milestone's first code commit.

### Work packages

- **UBC-4.1 — The base run, retained.** The specification's test suite through the harness at the base
  commit, per assertion, retained in `docs/evidence/ubc-4-001/` beside the decision rule; the twelve
  float-comparison assertions named in the bundle as the class expected to change. *Done when* the
  bundle names the base commit and every file by hash.
- **UBC-4.2 — The translator.** `WasmTranslator`: from a validated `WasmModule` to a universal
  bytecode artifact through `UbcArtifactWriter`, walking each body's bytes with the validator's sealed
  jump table: labels to absolute offsets, block results to `squash` sequences, `if`/`else` to
  conditional jumps, `br_table` to a JumpTables row with trampolines in first-use order, LEB immediates
  to fixed-width operands, dead code dropped, every instruction mapped per Appendix C, function types
  to the Types section, the memory, table, global, data and element definitions and the export names to
  the WebAssembly FamilyData section. Deterministic: two translations of every corpus module compared
  byte for byte in the harness root's checks. *Done when* every module of the harness corpus translates
  to an artifact the walk and the hook admit and the check is green.
- **UBC-4.3 — The family tables.** `WasmFamilyTables` per manifest (`broiler.webassembly.slice`
  today; the WA-5 manifest when decision UBC-D-3 mints it): the rows of Appendix C with their
  primitives, trap codes and the NaN-canonicalisation flag set; the region declarations `memory0` and
  `globals`; the trap-code vocabulary mapped to `WasmTrapKind`. *Done when* every `WasmOpcode` member
  has a row or a common mapping and no other row exists.
- **UBC-4.4 — The memory representation (decision UBC-D-2).** The family's `memory0` region must
  have a base that does not move for the instance's life and a length an emitter reads at the access,
  and a successful `memory.grow` must invalidate every view: a pinned managed array reallocated on
  growth with the base republished to the activation, or native memory owned by a `SafeHandle`. The
  choice is the WA-5 value-model row that profile's section 9 reserves, taken and recorded in its own
  decision series; either choice keeps route MVP-1's guest-observable refusal and the `LiveBytes`
  retention report. *Done when* the record exists and the store implements it.
- **UBC-4.5 — The handlers and the hook.** `WasmFamily : IUbcFamily` with no value plane: the
  `Dynamic` and `Call` rows' handlers (`memory.grow`, `call_indirect` answering a call request after the
  table and signature checks, with the family's trap codes for a null entry, an out-of-range index and
  a mismatch) and every `Primitive` row's reference handler from the interpreter's arms — the E2 check
  needs a handler per primitive row; the payload factories (`WebAssemblyResults`, `WebAssemblyTrap`,
  the entry-point fault); the store, tables, globals and segments as instance state with their atomic
  segment initialisation and proportional charges; `WasmFamilyVerifier : IUbcFamilyVerifier` refusing
  a memory access with no memory declared, an immutable global's set, a type index out of range, a
  segment out of bounds, each with a code under that profile's registry rules. The entry-point argument
  grammar of route MVP-6 is unchanged. *Done when* the family's contract tests pass on all three modes
  under the bytecode emitter.
- **UBC-4.6 — The float-comparison negative control.** Obligation E2's differential check run over the
  `wasm.f32.*` and `wasm.f64.*` comparison rows against the unmodified arms of the checkout: it fails,
  naming the rows, and is retained failing; the arms are corrected; it passes and is retained passing.
  *Done when* both logs are in the bundle.
- **UBC-4.7 — Parity.** The specification's test suite driven through the harness over the
  translator in the bytecode form, compared per assertion with the base run under the predeclared rule;
  the ratchet re-based by hand with the float-comparison class named; the WebAssembly corpus of
  malformed modules re-based as refusals at translation with the same answers where the decoder or
  validator refuses and new universal codes where the walk does. *Done when* the rule's verdict is met
  and every difference is in the named class.
- **UBC-4.8 — Retirement and the roots.** `WasmInterpreter`'s loop and `WasmValue` removed; the
  execution root composes the family over the bytecode emitter and carries the translator, its register
  row's sibling cell naming `Broiler.VM.Ubc` and its native-execution cell `none`; the harness root feeds
  modules through the translator; rule W1's list gains `Broiler.VM.Ubc` and nothing else; rule W2's
  baseline re-based. *Done when* both roots publish and run under the three modes with closures matching
  their rows.
- **UBC-4.9 — Records.** `WAC-nn` corrections for the plan's compiler and second-execution-arm
  non-goals, section 5's package boundaries, section 7's "the artifact is a WebAssembly module,
  unwrapped", section 10's execution mapping and section 17's restatement of the core's refusal; the
  memory decision record (UBC-D-2); the manifest decision (UBC-D-3) or its recorded absence; the
  profile's ledger rows recording observed repository state under its own legend; the MVP row for route
  UBC-R4 (the module as source), taken here; the programme ledger's row.

### Deliverables

The translator; the family tables, handlers, hook and payloads; the memory decision; the corpus
re-base; the parity bundle; the retired interpreter; rules W1, W2 revised; the `WAC-nn` entries; one
`MVP-` row; bundles `ubc-4-001` (base run and rule) and `ubc-4-002` (the run after and the comparison).

### Exit gate

1. Rule W1's list is Abstractions, Binary and `Broiler.VM.Ubc`; W2's baseline matches in both
   directions; U3 is not engaged (this is a language family) and U2 passes over `Broiler.VM.Ubc`
   unchanged.
2. The translator is deterministic over the harness corpus, two translations compared byte for byte.
3. Every `WasmOpcode` member has a row of Appendix C's table or a common mapping, and every
   `Primitive` row has a reference handler the E2 corpus reaches.
4. The float-comparison check is retained failing on the unmodified arms and passing after the
   correction, both watched.
5. The specification's test suite through the harness gives per assertion the verdict the retained base
   run gives, outside the named class; the ratchet is re-based by hand with the class named.
6. `memory.grow`'s guest-observable refusal and the `LiveBytes` retention report are unchanged, with
   the existing tests passing over the new store.
7. The memory representation decision exists in that profile's series and the store implements it;
   the WA-5 manifest is minted or its absence is recorded (UBC-D-3).
8. Both WebAssembly roots publish and run under the three modes with closures matching their register
   rows, the execution root's sibling cell naming `Broiler.VM.Ubc` and its native cell `none`.
9. The `WAC-nn` entries exist for every plan sentence the milestone makes false, and the plan carries
   the bare pointers.
10. The ledger's UBC-4 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-4-001/`: the decision rule and the base run. `docs/evidence/ubc-4-002/`: the run
after; the comparison per the rule; the float-comparison check's failing and passing logs; the
determinism check's log; the corpus re-base log; the publish-and-run logs and closures per mode.

### Does not do

It admits no instruction outside the manifest's surface: prefixed families, reference instructions
and sign-extension operators are refused at validation exactly as today. It adds no native form for
WebAssembly (UBC-6b). It claims no runtime identifier and prints no figure. It does not decide the
vector width question: a `v128` slot type is a universal bytecode format version 2 question, and route
MVP-2's reservation is annotated as such in `docs/mvp.md` rather than answered.

---

## 12. Milestone UBC-6b — the language families in the `x86-64` form

- **Objective.** Every family table the `x86` emitter admits is emitted, verified, armed, executed and
  compared with its bytecode form over the whole of each language's own suite — the JavaScript numeric
  and baseline forms recovered whole, and a native WebAssembly form for the first time in this
  repository.
- **Owner roles.** The core architecture owner with both profile owners and the security owner; the
  release owner for the support table.
- **Waits on.** UBC-3 and UBC-4; UBC-0.7's native-form parity rule committed before this milestone's
  first code commit.

### Work packages

- **UBC-6b.1 — Admission per table.** The emitter admits the JavaScript wide, numeric and slice
  tables and the WebAssembly tables, or refuses an artifact naming the row it cannot emit; the handler
  tables for each (family, manifest) pair built at registration and self-checked; the family-specific
  templates that exist today in the JavaScript numeric and baseline forms found again as rows of the
  common, primitive and handler-call templates, with any template that has no counterpart named in the
  bundle. *Done when* every artifact of every corpus under every admitted table emits under both
  conventions or is refused naming a row.
- **UBC-6b.2 — Obligation E2 over the language families.** The primitive differential check over the
  JavaScript numeric table's `Primitive` rows (against the arms, over the primitive input corpus) and
  over the WebAssembly tables' `Primitive` rows, including the NaN-canonicalisation flag's effect on
  every float row. *Done when* no difference remains and the logs are retained.
- **UBC-6b.3 — The whole suites in the native form.** The whole pinned `tc39/test262` suite under the
  wide manifest in the `x86-64` form, compared per variant with the bytecode form's run of UBC-3.9
  under the predeclared rule, with JSD-0025 section 11's three admitted classes (a refusal naming the
  emission ceiling; a fuel exhaustion on a guest-loading variant; a wall-clock exhaustion) and nothing
  else; the same for the specification's test suite in the WebAssembly family, which is the first native
  WebAssembly run this repository has held and is recorded as a capability and not as a figure; the
  fifteen Octane benchmarks reporting a score in the native form; the fuel-parity twins of both families
  in both forms at every ceiling tried, excluding guest-loading programs by name. *Done when* the rule's
  verdict is met with every difference classed.
- **UBC-6b.4 — The numeric forms comparison, restored.** `src/tests/forms` and `eng/compare-forms.py`
  run both forms of every kernel on the universal bytecode, printing the emitter's refusal for the
  named Octane file exactly as today; `eng/compare-test262-forms.py` compares the two forms' whole-suite
  verdict files. *Done when* both scripts run in the checks lane and their READMEs carry the dated
  correction that names the new substrate.
- **UBC-6b.5 — The scan and the corpus over language tables.** Every template of both conventions'
  tables reached by some artifact of some family or named unreached; the JavaScript checks lane's native
  rows — the four zero bytes, the unit ending without a return, the branch leaving its unit, the return
  code no emitter materialises, the word after a unit's return — re-authored as universal-container
  entries and replayed by the execution-only JavaScript root with no encoder in its image; re-emission
  equality across every corpus with the encoder in the image; the emitter's semantic version rule
  (JSD-0025 section 7: a change to either table bumps the version and re-bases retained artifacts in the
  same change) restated for the pivot's tables in the emitter family's own records. *Done when* both
  scan directions are green over every admitted table.
- **UBC-6b.6 — The compositions.** The JavaScript CLI's native option and the conformance root's
  native mode over the universal bytecode, both conventions, register rows `x86-64`; a WebAssembly
  demonstration root that composes the family over the `x86` emitter (a new root, or the harness root
  gaining a native lane — the choice is the WebAssembly owner's and the register records it), row
  `x86-64`; publish and run under the three modes with closures matching. *Done when* K1 to K5 pass
  over every changed row.
- **UBC-6b.7 — Records.** The support table's section 3a: the `x86-64` row says what exists for each
  family and that no runtime identifier is claimed, in the words gate 11 requires; `roadmap.backends.md`
  gains a dated note at its head that the forms it describes have been re-homed, and its JSB stages'
  State bullets are closed or superseded by name; `docs/mvp.md`'s rows MVP-3, MVP-7 and MVP-8 are
  annotated as carried over; the emitter family's ledger rows; the programme ledger's row.

### Deliverables

The emitter admitting every language table; the E2 logs over the language families; the whole-suite
runs in both forms for both families; the forms comparison restored; the native corpus rows re-based;
the compositions with `x86-64` rows; the support-table and backend-roadmap corrections; bundle
`ubc-6b-001`.

### Exit gate

1. Every artifact of every corpus under every admitted table emits under both conventions or is
   refused naming a row (E4), and two emissions of each compare equal (E3).
2. E2 holds over every `Primitive` row of the JavaScript numeric table and the WebAssembly tables with
   no admitted difference.
3. The whole pinned `tc39/test262` suite in the `x86-64` form gives the bytecode form's verdict per
   variant outside the three admitted classes, each difference classed in the bundle.
4. The specification's test suite in the `x86-64` form gives the bytecode form's verdict per assertion
   outside the same classes.
5. The fifteen Octane benchmarks report a score in the native form; no score is compared or printed
   outside the bundle.
6. The fuel-parity twins of both families give one verdict at every ceiling tried in both forms,
   guest-loading programs excluded by name (E6).
7. The forms comparison runs both forms of every kernel and prints the named Octane refusal.
8. The scan accepts everything both conventions emit over every admitted table, every template is
   reached or named unreached, the re-based native corpus rows are refused by the execution-only root,
   and re-emission equality holds with the encoder present (E8).
9. Every changed composition row passes K1 to K5 with closures read off published output under the
   three modes.
10. The support table's section 3a row and `roadmap.backends.md`'s note carry the corrections, and
    neither claims a runtime identifier or prints a figure.
11. The ledger's UBC-6b row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-6b-001/`: the decision rule (committed under UBC-0.7); the whole-suite verdict files
in both forms for both families and the comparisons; the Octane scores; the twins' verdicts; the E2
logs; the scan's both-direction logs; the corpus replay logs; the publish-and-run logs and closures.

### Does not do

It claims no runtime identifier for any convention and prints no figure. It does not weigh route
UBC-R6 (one call per `Dynamic` row): the baseline form is recovered at the granularity JSD-0025 took,
and a measurement of it is a bundle a later record may collect under its own predeclared rule, not this
milestone's.

---

## 13. Milestone UBC-7 — the `arm64` emitter, emitting-only

- **Objective.** A second architecture against a fixed contract, pinned without a run, admitting what
  it can emit and refusing the rest by name.
- **Owner roles.** The core architecture owner; the release owner for the support-table row.
- **Waits on.** UBC-6b (the pivot's shape has met a second family's tables); may be started as an
  encoder over the fixture family after UBC-6a.

### Work packages

- **UBC-7.1 — `Broiler.VM.Emitter.Arm.Format` and `Broiler.VM.Emitter.Arm`.** The pivot with the
  `arm64-aapcs64` template tables per emitter version, its convention's reservations and argument
  registers; the encoder (`JsArm64Assembler`, `JsArm64Backend` and `JsArm64Walk` made language-free,
  parked in bundle `ubc-3-002` at UBC-3.4 and taken up here) implementing `IUbcEmitter` for the common
  rows, the primitives, the handler-call sequence, the plane helper calls and the direct-call protocol,
  under AAPCS64. **Decision UBC-D-4**: whether the handler-call form (the JavaScript wide table) is
  emitted for `arm64`, which the `arm64` encoder refuses today, is taken by the owner at this milestone
  and recorded in the emitter family's series; either answer is a refusal by name for the tables not
  admitted. *Done when* every admitted table's artifacts emit deterministically.
- **UBC-7.2 — Pinned without a run.** A golden-byte test per template; a corpus every entry of which a
  disassembler reads back and compares against the instruction the table names, both failing on a
  changed byte; the scan over the `arm64` tables in every image; an artifact in the form verifies and
  refuses to instantiate on every host with the unsatisfied-host-assumption contract violation, from the
  command line and from a retained corpus entry. *Done when* the golden rows, the disassembler corpus and
  the refusal are checks-lane rows with retained logs.
- **UBC-7.3 — No execution assembly.** The emitter family has no `.Execution` project; a search of the
  tree finds no ImplMap row for an instruction-cache maintenance entry point; the register's
  native-execution cells of every root composing the encoder stay as they were (`x86-64` or `none`),
  because emitting is not arming. *Done when* K5 passes over every row with the encoder in an image.
- **UBC-7.4 — Records.** The emitter family's `docs/` tree with its ledger; the support table's row
  saying *emitting-only* in the row, naming no runtime identifier and carrying no figure, and the rule
  that fails a release where the word is missing confirmed to read the new row; `docs/mvp.md`'s row
  MVP-4 annotated as carried over; UBC-D-4's record; the programme ledger's row.

### Deliverables

Two assemblies; golden rows; the disassembler corpus; the refusal fixtures; the emitter family's
`docs/` tree; the support row; bundle `ubc-7-001`.

### Exit gate

1. `Broiler.VM.Emitter.Arm.Format` references exactly Ubc and Ubc.Native; `Broiler.VM.Emitter.Arm`
   exactly those and its pivot; U3 passes over both with a witness watched.
2. A golden-byte test per template and a disassembler-checked corpus exist and fail on a changed byte,
   both watched.
3. An artifact in the `arm64` form verifies through the scan in every image and refuses to instantiate
   on every host by name, from the command line and from a corpus entry replayed by an execution-only
   root.
4. No assembly a published image can contain declares an instruction-cache maintenance entry point,
   and K5 passes over every row.
5. The support table's row says *emitting-only* in the row, claims no runtime identifier and carries
   no figure; the release rule that reads the word passes.
6. UBC-D-4 is recorded, and every table it does not admit is refused naming the row.
7. The ledger's UBC-7 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-7-001/`: the golden rows' log; the disassembler corpus and its log; the refusal
logs; the scan logs; the checks-lane run.

### Does not do

Nothing executes. No figure and no capability claim attaches to the form, and a document that
describes it as demonstrated, supported or working fails the release under gate 11. The exclusion's
reason is unchanged and is not a shortage of machines.

---

## 14. Milestone UBC-8 — the polyglot composition

- **Objective.** One interpreter, two families, one image — and the negative controls that show the
  families stay apart inside it.
- **Owner roles.** The core architecture owner with both profile owners.
- **Waits on.** UBC-3 and UBC-4.

### Work packages

- **UBC-8.1 — The root.** `Broiler.VM.Composition.PolyglotCli` composes the JavaScript and
  WebAssembly registrations into one bytecode emitter through `UbcDescriptors.Build` twice — one
  descriptor per language, two catalog rows — states its ceilings as today, and runs a JavaScript
  program and a WebAssembly module in one process from one command line; publishes and runs under the
  three modes with closures matching its register row (profiles `broiler.javascript`,
  `broiler.webassembly`; siblings `Broiler.VM.Ubc`, the JavaScript format and lowering; native `none`).
  *Done when* K1 to K5 pass over the row.
- **UBC-8.2 — The negative controls.** An artifact declaring both language families in its Families
  section is refused at verification by the walk with a named code; a unit naming a family slot its
  artifact did not declare is refused; a JavaScript `v` handed to a WebAssembly unit is unrepresentable
  (a contract test that the type system refuses it, not a run-time check). *Done when* the three are
  contract tests on all three modes.
- **UBC-8.3 — The two-profile catalog test over product families.** UBC-2.5's hostile-neighbour test
  repeated over the two product families: each family's hard maxima do not reach the other, each
  family's adopted defaults do, and the root's stated ceilings decide. *Done when* it passes on the three
  modes and the composition register's row names it.
- **UBC-8.4 — The cross-runtime bound.** Where the root writes a host object bridging the two
  runtimes under one shared parent budget, the call chain is bounded by the parent's `CallDepth`,
  witnessed by the two-row case the roadmap's risk row asks for — one row with a parent, one without —
  closing the depth half of the bound the roadmap has carried as unproven since 2026-08-31 for a chain
  that crosses two product runtimes. *Done when* both rows are contract tests and the support table's
  "declared and not demonstrated" clause for the depth bound is corrected with a date.
- **UBC-8.5 — Records.** The register row; the support table's cross-profile clauses; the roadmap's
  section 14 row on the two-profile test corrected with a date; the programme ledger's row.

### Deliverables

The root on the universal bytecode; the negative controls; the two-profile test; the bound's two-row
case; the corrections; bundle `ubc-8-001`.

### Exit gate

1. The root publishes and runs under the three modes with closures matching its register row, which
   declares `none`.
2. A JavaScript program and a WebAssembly module run in one process from one command line under one
   set of stated ceilings, in a retained transcript.
3. The three negative controls of 8.2 pass on the three modes.
4. The two-profile hostile-neighbour test over the two product families passes and the register names
   it.
5. The cross-runtime `CallDepth` bound is witnessed by the two-row case, and the support table's clause
   is corrected with a date.
6. The ledger's UBC-8 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-8-001/`: the publish-and-run logs and closures per mode; the transcript; the
contract-test run; the catalog table.

### Does not do

It opens no value channel: the negative controls are the proof. It advertises nothing, and the root's
project file keeps saying why. It adds no native form to the polyglot image (its row stays `none`).

---

## 15. Milestone UBC-9 — records, the support table and packability

- **Objective.** What exists is described exactly and nothing more, in every record that describes it,
  and the two questions the programme leaves to a decision are answered or recorded as open.
- **Owner roles.** The release owner with the core contract owner and the architecture owner.
- **Waits on.** Whichever milestones have landed; this milestone may run after any of them and is
  complete only after UBC-8.

### Work packages

- **UBC-9.1 — The support table.** Section 3a becomes the emitter table: one row per form, with
  *exists*, *published and run* and *deterministic refusal elsewhere* answered on retained evidence and
  *emitting-only* in the row where it applies; section 3b's rows for the two languages restated over
  the universal bytecode; the two "declared and not demonstrated" bounds updated for UBC-8.4; section 1
  stating that the core ships no language and no emitter. *Done when* every cell cites a bundle or says
  "none".
- **UBC-9.2 — The composition register.** Every row's sibling and native-execution cells as section 9
  of the concept lists; the register's introductory text names the emitter pattern beside the profile
  pattern. *Done when* K1 to K5 pass and the text is corrected with dates.
- **UBC-9.3 — The graph and the budget.** ADR 0001's budget sentence, `graph.manifest.json`, both
  solutions and the tree agree; the record carries one dated revision per milestone that changed the
  graph. *Done when* A7 and A15 pass and each revision quotes its superseded counts.
- **UBC-9.4 — Packability (decision UBC-D-5).** A dated ADR 0001 revision answering whether
  `Broiler.VM.Ubc` and `Broiler.VM.Ubc.Native` become the fourth and fifth packages — naming, as the
  record's budget rule requires, which boundary each enforces and why it is not enforceable inside an
  existing assembly — or refusing; rules A6, C1 and C2 revised to say what the answer says, with
  witnesses; the packages' READMEs under `eng/nuget/` if packed; rule C3 confirmed over the new nuspecs
  (no language named). *Done when* the revision exists and the rules agree with it.
- **UBC-9.5 — The universal bytecode contract version (route UBC-R9).** A dated record in the core's
  ADR set (an ADR 0013 revision, or ADR 0014) minting universal bytecode contract version 1: what it
  covers (the container format, the common family, the slot types, the operand shapes, the table
  schema, the primitive table, and the public surface of the two assemblies as baselined), how it is
  amended (a dated record, additive or breaking, in that record's own procedure and not the core
  contract's), and the authored and built-against integers every family registration and emitter
  carries, checked at catalog construction. *Done when* the record exists, the integers are in the
  contracts, and the mismatch refusal is a contract test.
- **UBC-9.6 — The route register.** Every `MVP-` row the programme filed carries its state; routes not
  yet filed because their milestone did not land are named as not taken; MVP-2's vector reservation
  annotated as a universal format question; MVP-3, MVP-4, MVP-7 and MVP-8 annotated as carried over.
  *Done when* `docs/mvp.md` section 5 says so.
- **UBC-9.7 — Every correction this programme made** is an entry in the file that owns it with the
  superseded text quoted, and the concept document itself gains a dated note per milestone saying which
  of its sections the milestone changed the truth of — the concept is a proposal document and is
  corrected in place, quoting the superseded text, as the backend roadmap was. *Done when* a sweep of
  the concept against the tree finds no sentence that is false without a dated note.
- **UBC-9.8 — Route UBC-R5's ruling.** The core contract owner rules on `IVmNativeCompiler`'s reading
  (kept, implemented by the native emitters under the input profile's identity) or names it as an open
  candidate in ADR 0003's register. *Done when* the ruling or the candidate row exists.

### Deliverables

The support table; the register; the ADR 0001 revisions; the packability decision; the universal
bytecode contract version record; the route register's state; the concept's dated notes; bundle
`ubc-9-001`.

### Exit gate

1. Every cell of the support table's section 3a cites a retained bundle or says "none", says
   *emitting-only* where it applies, claims no runtime identifier the core has not claimed, and prints
   no figure.
2. K1 to K5 pass over every register row with the cells section 9 of the concept lists.
3. A7 and A15 agree with the tree and every ADR 0001 revision quotes its superseded counts.
4. The packability decision exists as a dated ADR 0001 revision, and rules A6, C1, C2 and C3 say what
   it says, witnessed.
5. The universal bytecode contract version record exists; every family registration and emitter
   carries the two integers; a mismatch is refused at catalog construction in a contract test.
6. `docs/mvp.md` section 5 carries the state of every route the programme took and names the ones not
   taken.
7. The concept document carries a dated note per landed milestone and no sentence false against the
   tree without one.
8. Route UBC-R5's ruling or its candidate row exists.
9. The ledger's UBC-9 row names each unmet clause individually while any is unmet.

### Evidence bundle

`docs/evidence/ubc-9-001/`: the rule runs for A6, A7, A15, C1 to C3, K1 to K5 and M1; the nuspecs if
packed; the contract-test run for the version mismatch; the sweep of the concept against the tree.

### Does not do

It advertises nothing, publishes nothing, claims no runtime identifier and moves no row to `Accepted`.
A packability decision in the affirmative makes two assemblies packable and ships neither.

---

## 16. Milestone UBC-10 — the component split, optional and last

**This milestone exists only if decision UBC-D-6 is taken in the affirmative, and nothing before it
waits on it.** A programme that ends at UBC-9 is complete. The milestone is stated here so that its
cost and its gate are known before anyone decides, and so that no earlier milestone quietly does part
of it.

- **Objective.** Each component of the concept's section 16 builds, publishes and runs from the others'
  packages alone, in a repository of its own, with its own records.
- **Owner roles.** The repository owner as the deciding owner; the release owner for the packages and
  the feeds; the architecture owner for the rules; every profile and emitter owner for their tree.
- **Waits on.** UBC-9 (the packability decision in the affirmative and the universal bytecode contract
  version record), UBC-3, UBC-4, UBC-6b and UBC-7 for the components that would exist to be split out,
  and decision UBC-D-6.

### Work packages

- **UBC-10.1 — The decision and its record.** UBC-D-6 recorded as a dated revision of ADR 0001
  reversing its ruling of 2026-08-31 — "a language profile is a set of product projects in the
  Broiler.VM component rather than a component of its own" — with the superseded text quoted, the
  one-way reference direction restated, and the README sentence and roadmap section 1's *VM profile*
  row corrected with the same date. *Done when* the three records carry the correction.
- **UBC-10.2 — The feed.** A pristine feed holding `Broiler.VM`'s five packages, produced by the
  existing pack lane (`eng/pack.ps1`, `eng/verify-feed.ps1`), from which every other component
  restores; the feed-consumer sample under `samples/` extended to compose the fixture family over the
  bytecode emitter's package, proving the shape once in the core repository before any component
  depends on it. *Done when* the sample restores and runs from the feed under the three modes.
- **UBC-10.3 — The repositories.** One repository per component of section 16.1, each created from
  the corresponding directory tree with its history, its `docs/` tree moved whole and its ledger rows
  unchanged in state, its own `Directory.Build.props` in the shape this repository's has (no chaining
  to a parent), its own solution, its own architecture-test project with its own rule register holding
  its reference set by package name and its own witnesses, its own assurance generator run, its own
  publish-and-run lane per declared runtime identifier, and its own support table naming what it has
  published and run on retained evidence. Project references across a component root are replaced by
  package references to the feed. *Done when* each repository builds, tests, publishes and runs on its
  own from the feed alone.
- **UBC-10.4 — The rules in the core repository.** D1 revised to key on the legacy components it
  exists for, with a witness (a sibling `Broiler.VM.*` component's package reference into the core is
  not a breach; a legacy component's still is); A11, A12 and N2 reduced to the families the core
  repository still holds (the fixture family) and their content restated as each component's own rule;
  K1 to K5 read only the core repository's roots; `graph.manifest.json` and ADR 0001's budget sentence
  hold the core repository's tree only. *Done when* the core repository's suite passes with the
  component trees absent.
- **UBC-10.5 — Composition roots re-homed.** Each component keeps the demonstration roots its
  evidence needs; the polyglot root and any root composing a native emitter with a language move to the
  product that ships them, or stay in the core repository as demonstration roots composed from the feed
  — the choice recorded in the composition register with the reason. *Done when* every root has one
  register row in one repository and its closure is read off the published output there.
- **UBC-10.6 — The reopened refusals.** ADR 0011's standing refusals of the assurance tooling and the
  conformance-harness method are reopened by the split's own condition — a second component's own
  implementation exists — and each receives a new dated verdict in the core ADR set, accept or refuse,
  filed by the extraction gate's procedure. *Done when* both verdicts are filed.
- **UBC-10.7 — The support tables.** `Broiler.VM`'s says the core ships no language and no emitter;
  each component's says what it has published and run; a product's names the components it composes by
  name and universal bytecode contract version. *Done when* no table claims a runtime identifier, a
  supported form or a figure it did not claim the day before the split.

### Deliverables

The ADR 0001 revision; the feed and the extended sample; one repository per component with its
records, rules and lanes; the core repository's revised rules; the re-homed roots; the two reopened
verdicts; bundle `ubc-10-001` in the core repository and one bundle per component in its own.

### Exit gate

1. A pristine feed of the five core packages restores and builds every component with no project
   reference across a component root, and the feed-consumer sample composes the fixture family over the
   bytecode emitter from it under the three modes.
2. Each component's own rule register holds its reference set by package name with a negative control
   watched; no component references another except through the core's packages, and the core
   references none.
3. Rule D1 is revised and its witness watched in both directions; the core repository's suite passes
   with the component trees absent.
4. Every composition root has exactly one register row in exactly one repository, and its closure read
   off the published output matches it.
5. The universal bytecode contract version appears in the core's support table and in every
   component's registration, and a mismatch is refused at catalog construction across a package
   boundary in a contract test.
6. ADR 0001's ruling of 2026-08-31 is reversed by a dated revision with the superseded text quoted, and
   the README and roadmap section 1 carry the correction.
7. The JavaScript and WebAssembly `docs/` trees are in their components whole, with their ledgers'
   rows unchanged in state and a dated line saying the tree moved.
8. Both reopened standing refusals carry a new dated verdict.
9. No component's table claims a runtime identifier, a supported form or a figure it did not claim the
   day before the split.
10. The ledger's UBC-10 row names each unmet clause individually while any is unmet — or says the
    decision was not to split, and closes.

### Evidence bundle

`docs/evidence/ubc-10-001/` in the core repository: the feed's package list by hash; the sample's
publish-and-run logs; the rule runs; the register; per component, in its own repository, its first
bundle naming the commit the tree was split at.

### Does not do

It changes nothing the concept's sections 3 and 5.13 state: the core is untouched, the cross-profile
boundary is the embedder's, and a component boundary is a stronger form of the family boundary and not
a weaker one. It publishes nothing to a public feed, advertises nothing and claims no runtime
identifier: a pristine local feed is the proof shape, as it is today for the three packages.

---

## 17. Decision points the owner must take, and when

A decision point is a place where two routes are both admissible under the published rules and a
milestone's clause cannot be met until one is chosen. Each is recorded when taken; until then the
clause is `Blocked` naming the holder, and the rest of the milestone proceeds.

| # | Decision | Alternatives | Taken by | Needed before |
|---|---|---|---|---|
| UBC-D-1 | Retire the value form's substrate together with the old JavaScript pipeline | (a) retire at UBC-3.10, reversing the ruling of 2026-09-25 that keeps the form in the tree as opt-in; (b) keep the old pipeline frozen behind its opt-in roots, doubling the profile's maintained code and its assurance surface until a later ruling | the JavaScript profile owner | UBC-3 clause 10 |
| UBC-D-2 | The WebAssembly memory representation a native form can address | (a) a pinned managed array reallocated on growth with the base republished; (b) native memory owned by a `SafeHandle`; either with growth invalidating every view | the WebAssembly profile owner with the security owner | UBC-4 clause 7 |
| UBC-D-3 | Mint the WA-5 manifest (`broiler.webassembly.numeric1`) so that the family's tables are selected by the surface they admit | (a) mint it at UBC-4 with its own retained run; (b) keep the slice identity and record that the table admits more than the identity says, as the code does today | the WebAssembly profile owner | UBC-4 clause 7 |
| UBC-D-4 | Whether the `arm64` emitter emits the handler-call form (the JavaScript wide table) | (a) emit it, pinned by golden bytes and the disassembler corpus like every other template; (b) refuse the wide table by name as the encoder does today | the core architecture owner | UBC-7 clause 6 |
| UBC-D-5 | Packability of `Broiler.VM.Ubc` and `Broiler.VM.Ubc.Native` | (a) the fourth and fifth packages, each naming the boundary it enforces; (b) refused, with the split of UBC-10 thereby refused too | the release owner with the architecture owner | UBC-9 clause 4 |
| UBC-D-6 | Whether to split the repository into components | (a) split, under UBC-10's gate; (b) keep one repository with the component names, which is complete at UBC-9 | the repository owner | UBC-10, if at all |
| UBC-R1 | ADR 0011's P1 reading (a route, not a decision point, listed for completeness) | editorial revision versus amendment | the core contract owner | UBC-0; a ruling for amendment blocks the programme naming the core as holder |

**Every holder is one person on this document's date**, and each decision is recorded as taken by one
person with the non-independence stated in the record's own words.

---

## 18. The evidence and test matrix

What each milestone runs, where, and what it retains. "Three modes" is JIT, trimmed and Native AOT,
published and run; "lane" is the composition's checks lane; a corpus is retained with hashes.

| Milestone | Runs | Where | Retains |
|---|---|---|---|
| UBC-0 | the banned-vocabulary scan over the record; E2/E4 over the index | architecture tests | the correspondence table; the scan log |
| UBC-1 | the malformed corpus replay; the codec round trip; U1, U2, U4, U8, U9 | contract and architecture tests | the corpus with hashes and answers; the registry; the API baseline |
| UBC-2 | the fixture program corpus; the twins; the UBC-1 corpus across three modes; the hostile-neighbour test; the JIT fold disassembly; K1–K5 | the fixture root, three modes | closures per mode; the disassembly; the corpora |
| UBC-5 | the scan's negative fixtures through the fixture root with no encoder; B5c, X1, X2, X3, X4, U3, U7 | fixture root; architecture tests | the fixture entries; the rule runs |
| UBC-6a | E1–E9 over the fixture family under both conventions; the scan both directions; golden rows; re-emission; the guest-load cases | the fixture root's lane, three modes | the E2 log; the scan logs; the golden rows; closures |
| UBC-3 | the wide determinism check; the whole `tc39/test262` suite (base and after) under the predeclared rule; Octane scores; the twins; the corpus re-base; N1, N3, N5–N8, N10, N12, N19–N21, U5; the native checks rows | the JavaScript roots' lanes, three modes; `eng/run-test262.py`, `eng/run-octane.py` | both verdict files and the comparison; the ratchet's retired rows; the re-base log; closures |
| UBC-4 | the translator determinism check; the specification's suite (base and after) under the rule; the float-comparison check failing and passing; the corpus re-base; W1, W2 | the WebAssembly roots' lanes, three modes | both assertion files and the comparison; the check's two logs; closures |
| UBC-6b | E2 over the language tables; both suites in both forms under the rule; Octane in the native form; the twins per form; the forms comparison; the scan both directions over every admitted table; the re-based native corpus rows through the execution-only root; re-emission | every compiler-bearing root's lane, three modes; `eng/compare-forms.py`, `eng/compare-test262-forms.py` | the verdict files in both forms and the comparisons; the E2 logs; the scan logs; closures |
| UBC-7 | golden bytes per template; the disassembler corpus; the scan over the `arm64` tables; the refusal to instantiate from the command line and from a corpus entry | the lanes that carry the encoder | the golden rows; the corpus and its log; the refusal logs |
| UBC-8 | the polyglot root's transcript; the three negative controls; the two-profile test; the `CallDepth` two-row case | the polyglot root, three modes | closures; the transcript; the contract-test run |
| UBC-9 | A6, A7, A15, C1–C3, K1–K5, M1; the version-mismatch refusal; the sweep of the concept against the tree | architecture and contract tests | the rule runs; the nuspecs if packed; the sweep |
| UBC-10 | the feed restore per component; each component's own suite and lanes; D1's witness | each repository | per-repository bundles |

**What no milestone runs**: a benchmark whose figure a record would quote; a run on a runtime
identifier not declared in ADR 0012's matrix; anything on an `arm64` machine that would be read as
execution of the `arm64` form.

---

## 19. Rules minted or moved, by milestone

| Milestone | Minted | Revised | Retired |
|---|---|---|---|
| UBC-0 | — | — (the roadmap-amendment register moves; no rule) | — |
| UBC-1 | U1 (reference set), U2 (banned vocabulary, no family row), U4 (one common-family table), U8 (universal diagnostic registry), U9 (API baseline of the shared assemblies) | A7, A15 (graph and budget) | — |
| UBC-2 | — | A11, A12 (the emitter family pattern), A13 (a consumer family may reference Ubc), A7, A15 | — |
| UBC-5 | U3 (no language in an emitter-side assembly), U7 (the thread-static slot) | B5c, X1 (name `Broiler.VM.Emitter.X86.Execution` and `VmNativePage`), X2, X3, X4, A7, A15; U1's second clause | — |
| UBC-6a | — | K5 (the fixture root's `x86-64` cell), A7, A15 | — |
| UBC-3 | U5 (a family table version bound to its manifest; one walk reads every table) | N1, N3, N10; the JavaScript roots' K rows | N16 |
| UBC-4 | — | W1, W2; the WebAssembly roots' K rows | — |
| UBC-6b | — | the compiler-bearing roots' K rows | — |
| UBC-7 | — | K5 confirmed unchanged over roots carrying the encoder; the release rule that reads *emitting-only* confirmed over the new row | — |
| UBC-8 | — | K rows for the polyglot root | — |
| UBC-9 | — | A6, C1, C2, C3 per the packability decision | — |
| UBC-10 | per-component registers | D1; A11, A12, N2 reduced to the core repository's families; K1–K5 scoped to one repository | — |

Every minted or revised rule carries a witness watched failing when injected and passing after
revert, named in its register row.

---

## 20. Records filed, by milestone

| Milestone | Records |
|---|---|
| UBC-0 | ADR 0013 (the extraction record, not contract-bearing); JSD-0036; ADR 0003 section 11 register entries for roadmap sections 1, 8, 10 and 16; the ADR 0011 pointer paragraph; the README correction; the "stands beside this plan" paragraphs in both profile plans; `MVP-` rows for UBC-R1 and UBC-R2; the three decision-rule files |
| UBC-1 | ADR 0001 revision; `docs/ubc/api/public-api.txt`; `docs/ubc/diagnostics/registry.txt` |
| UBC-2 | ADR 0001 revision; the fixture root's register row; `MVP-` rows for UBC-R3 and UBC-R7 |
| UBC-5 | ADR 0001 revision; register sibling cells; a `JSC-nn` entry; `MVP-` row for UBC-R8 |
| UBC-6a | the `x86` emitter family's `docs/` tree with its ledger; the support table's section 3a row; the register row; the native-pipeline contract test under UBC-R5's reading |
| UBC-3 | JSD-0036 amended; `JSC-nn` entries for every plan sentence made false; the JavaScript ledger's observed-state rows; `MVP-` row for UBC-R6; UBC-D-1's record |
| UBC-4 | `WAC-nn` entries; UBC-D-2's and UBC-D-3's records; the WebAssembly ledger's observed-state rows; `MVP-` row for UBC-R4 |
| UBC-6b | the support table's section 3a; `roadmap.backends.md`'s dated note and closed State bullets; `docs/mvp.md` annotations on MVP-3, MVP-7, MVP-8 |
| UBC-7 | the `arm` emitter family's `docs/` tree; the support row; `docs/mvp.md` annotation on MVP-4; UBC-D-4's record |
| UBC-8 | the register row; the support table's cross-profile clauses; the roadmap's section 14 correction |
| UBC-9 | the packability revision of ADR 0001 (UBC-D-5); the universal bytecode contract version record (UBC-R9); the route register's state; the concept's dated notes; UBC-R5's ruling or candidate row |
| UBC-10 | the ADR 0001 revision reversing the 2026-08-31 ruling (UBC-D-6); D1's revision; the reopened verdicts; per-component ledgers and support tables |

---

## 21. Risks and stop conditions

| Risk | Mitigation / stop condition |
|---|---|
| **UBC-0's verdict is a refuse**, or the P1 ruling makes the programme wait on the amendment procedure | The refuse is filed and the programme ends with a met gate. A ruling that P1 is contract content blocks UBC-0 naming the core as holder; the ledger says so and nothing after it starts. **Stop: an unrecorded verdict, or code written under a blocked UBC-0.** |
| **A language concept leaks into the shared assembly** — an identifier, a row, a `switch` on a family | U2 and U3 with witnesses; UBC-0's scan over the record itself. **Stop: a shared-assembly change that passes U2 by renaming rather than by removing.** |
| **The cut-over loses a verdict.** A test262 variant or a specification assertion answers differently after UBC-3 or UBC-4 outside the admitted classes | The base runs retained before any engine change; the predeclared rules committed before the code; every difference classed in the bundle. **Stop: a difference outside the classes is a regression, and the milestone stays `In progress` until it is fixed or the rule is revised with the superseded text quoted and the base re-run.** |
| **The capability window.** A form the tree has today is absent between two milestones | The delivery order of section 3: emitters over the fixture family before any language cut-over; the JavaScript forms recovered in the same milestone that retires their substrate. **Stop: a milestone landed with a form absent that its predecessor had, unless the ledger says so in the row and the support table says so in the cell.** |
| **The value form's substrate** is retired by a milestone against the ruling that keeps it | UBC-D-1 named as a decision point; UBC-3.10 `Blocked` on it. **Stop: retiring it without the ruling.** |
| **A primitive's inline implementation disagrees with a handler** on an input the corpus did not hold | E2 over a retained input corpus covering every NaN class, both zeros, overflow and trap edges, run per emitter and per family; the float-comparison defect as the first negative control. **Stop: a `Primitive` row admitted with no corpus entry for one of its trap edges.** |
| **Emitted code holds a managed reference** | X2 over every frame type of `Broiler.VM.Ubc.Native`; U7 over the slot; the rooting statement of the concept's 7.3 restated in the emitter family's records. **Stop: one such reference reopens MVP-3 and stops the form.** |
| **Determinism fails in a translator or an emitter**, silently, on a machine the lane does not have | Two-run comparisons in every lane, and re-emission equality with the encoder present. **Stop: a lane that compares once.** |
| **A figure enters a record** because a native form invites one | Section 4's rule 6; the bundles the only place a figure goes; a sweep at UBC-9. **Stop: a figure in a plan, ledger, decision or support document is an untruthful claim under the MVP's own terms.** |
| **The WebAssembly memory moves under emitted code** | UBC-D-2 taken before UBC-4's store is written; growth invalidates every view. **Stop: a native form addressing a region whose base can move.** |
| **The generic loop does not fold** in the compiled image | UBC-2.6's disassembly check on the JIT lane; the AOT half asserted or excluded by name. **A loop that does not fold reopens UBC-R7 and is recorded, not hidden.** |
| **`eval` in a native-form instance** reaches a fallback | UBC-6a.5's contract tests; the form-per-instance rule. **Stop: any path by which one artifact executes in two forms.** |
| **The split is started early** — a component's shape decided by UBC-10 before UBC-D-6 | Section 1.3; the assembly names already fixed; UBC-10's clauses reachable only after UBC-9. **Stop: a milestone before UBC-10 adding a package, a feed or a repository.** |
| **Unreviewed work accumulates**, as the core ledger already records for the checkout | Every milestone's units `HUMAN_PENDING`; nothing published, claimed or accepted; the review debt named in each bundle's README. Not a stop: the MVP's terms permit it, and the ledger says what it costs. |

---

## 22. The ledger, and how it is updated

[The programme ledger](universal-bytecode.status.md) holds one row per milestone in delivery order,
the decision points with their holders, the required bundle shape, and its own update rules, which
restate the core ledger's for this programme: a row moves in the change that lands or blocks its
milestone; a gate is never copied into the evidence column; completion is never inferred transitively;
a milestone that owns anything is `In progress` and names its unmet clauses; a gated clause is
`Blocked` naming the holder; profile ledgers are neither read nor advanced from it; `Accepted` is
unreachable while review is deferred; no figure enters it; and UBC-10 stays `Not started` until
UBC-D-6 is recorded, with the programme complete at UBC-9 either way.

**How this roadmap is corrected.** A sentence here made false by a landed milestone is corrected in
place with the superseded text quoted and dated, in the core documents' convention; a gate clause that
changes is a gate revision the ledger's update rule 5 records, with existing evidence re-evaluated
rather than carried.
