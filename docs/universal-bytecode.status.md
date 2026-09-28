# The universal bytecode programme — status ledger

**Last updated:** 2026-09-28 (UBC-4's clauses 4 and 5 met on bundle `ubc-4-006`, under a rule revised after bundle `ubc-4-005`'s runs were seen; every clause of UBC-4 is met on retained evidence, and it is not accepted). Before that: 2026-09-28 (UBC-4's clauses 4 and 5 judged on bundle `ubc-4-005` under a new dated rule, and still unmet: its decision is NOT MET on a set of NaN answers no class admits). Before that: 2026-09-28 (UBC-4's clause 6 met on bundle `ubc-4-004`, on a gate revised on its owner's decision; clauses 4 and 5 still unmet). Before that: 2026-09-28 (UBC-4's clause 3 met on bundle `ubc-4-003`, for the region rows the E2 check did not reach; clauses 4, 5 and 6 still unmet). Before that: 2026-09-26 (UBC-4 moved to `In progress` on bundles `ubc-4-001` and `ubc-4-002`, with clauses 3, 4, 5 and 6 unmet; decision UBC-D-5 recorded as taken early for `Broiler.VM.Ubc` and `Broiler.VM.Emitter.Bytecode`). Before that: 2026-09-25 (UBC-0 recorded; UBC-3 and UBC-5 to UBC-10 given the reason they cannot meet their gates; UBC-1 and UBC-2 moved to `In progress` on bundles `ubc-1-001` and `ubc-2-001`)

**Authority:** this file is the authoritative current-evidence ledger for the milestones in
[the universal bytecode roadmap](universal-bytecode.roadmap.md). The roadmap defines planned work
and objective exit gates; this ledger records whether those gates have accepted evidence. The
concept the roadmap plans is [`docs/universal-bytecode.md`](universal-bytecode.md), which decides
nothing and records no state.

**On this file's date every milestone is `Not started`.** No line of the programme's code exists,
no record it names has been filed, and no bundle under `docs/evidence/ubc-*` exists. That is the whole
of what this ledger says today, and a reader who finds a later row moved should find a dated line
beside it saying why.

*(Moved later on 2026-09-25, and the paragraph above is kept as what this file said when it was
created.)* Milestone UBC-0 now owns records and a retained bundle: [ADR 0013](adr/0013-the-universal-bytecode-extraction-record.md) is filed. It
**accepts** the universal bytecode and its one loop (candidate A). For the native-form mechanism beside
it (candidate B) it records that G1 is unsatisfied - one product profile has a native form and the
other has no emitted code at all - so the gate cannot be invoked for it, in the dated note ADR 0011
prescribes for that state, which carries no verdict and refuses nothing. That note is why the UBC-5
row names a holder, and why every row that waits on UBC-5, directly or through UBC-6a and UBC-3, says
it cannot meet its gate. Only UBC-1, UBC-2 and UBC-4 can. No line of the programme's code exists yet.

*(Moved again later on 2026-09-25, and the paragraph above is kept as what this file said when UBC-0
was recorded.)* The programme's code now exists: `Broiler.VM.Ubc`, `Broiler.VM.Emitter.Bytecode`, the
fixture family `Com.Example.Tally` and its composition root, each with the rules, the corpora and the
records its milestone names. UBC-1 and UBC-2 each hold every clause of their exit gates on a retained
bundle, and neither is accepted, because review is deferred. UBC-4, the one other milestone that can
meet its gate, waits on UBC-2 and on the WebAssembly profile owner's decisions UBC-D-2 and UBC-D-3 for
its clause 7, and has not started. Every milestone after them still cannot meet its gate while ADR
0013's note on candidate B stands, for the reason the UBC-5 row gives.

*(Moved on 2026-09-26, and the paragraph above is kept as what this file said when UBC-1 and UBC-2 were
recorded.)* UBC-4 now owns code, records and bundles `ubc-4-001` and `ubc-4-002`. The WebAssembly profile translates a module
into universal bytecode, its family executes through the bytecode emitter, and the profile's own
interpreter is retired; decisions UBC-D-2 and UBC-D-3 are taken in that profile's own decision series.
UBC-4 is `In progress` and not accepted, and clauses 3, 4, 5 and 6 of its exit gate are unmet, each
named in its row: clause 3 for the region-access primitive rows, which the E2 check does not reach; clauses 4 and 5,
which the predeclared rule judges with one decision that cannot be MET while the specification's test
suite is not pinned and has no reader; and clause 6, because this milestone changed the store's
retention report. So the paragraphs above that count UBC-4 among the milestones that can meet their
gates no longer hold of it as things stand: clauses 4 and 5 now need a new dated rule and a base run that
only a commit with the retired path can give. Decision UBC-D-5 was taken early for `Broiler.VM.Ubc` and
`Broiler.VM.Emitter.Bytecode`, which are packable, because the WebAssembly profile, already packable,
now references the first; the fixture family and its root stay unpackable. Every milestone other than
UBC-0, UBC-1, UBC-2 and UBC-4 still cannot meet its gate while ADR 0013's note on candidate B stands,
for the reason the UBC-5 row gives. **This move is late.** Under update rules 1 and 4 the UBC-4 row
should have moved to `In progress` on 2026-09-25, in the change that first gave the milestone code, a
bundle or a record - at the latest in `9d160d6`, which filed its decision records and edited this file's
decision rows - and it stood at `Not started` until this change. Bundle `ubc-4-001`'s README, committed
in `0837208`, describes this row's naming of clause 5's holder before the row said it. The decision
row UBC-D-5 is late in the same way: the decision took effect in code in `1b55423` and was recorded in
`9ccc9c5`, and its row stood at `open` until this change.

*(Moved on 2026-09-28, and the paragraph above is kept as what this file said when UBC-4 was recorded.)*
UBC-4's clause 3 is met. Its UBC-4 row said of it: "the region-access `Primitive` rows - the loads, the
stores and `memory.size` - have no reference handler the E2 corpus reaches, because the E2 lane and the
programme's primitive corpus cover the numeric rows only; holder: the WebAssembly profile owner, for a
region input corpus and a lane". Both now exist: the retired interpreter's memory arms, recovered as the
profile's reference for those rows, a region input corpus beside the primitive inputs, and a harness
lane comparing the two, retained in bundle `ubc-4-003` with a negative control. Clauses 4, 5 and 6
stand as the row gives them. The core defect bundle `ubc-4-002` named - an instantiation the core
drops leaving its retention counted - was fixed in the same branch, in `45778cf`, before UBC-4 was
merged into it; it was never one of this milestone's clauses, and it moves no row here.

*(Moved again on 2026-09-28, and the paragraph above is kept as what this file said when clause 3 was
met.)* UBC-4's clause 6 is met, on a gate revised the same day. The UBC-4 row said of it: "`memory.grow`'s
guest-observable refusal is unchanged, but the `LiveBytes` retention report is not - the store charges
retention before its allocation, and a growth a core budget refuses ends the step at the growth - and
no test asserts the amounts; holder: the WebAssembly profile owner, who either accepts the change
through a gate revision under update rule 5 or has the store report after the allocation again". The
owner accepted the change: the store keeps its order, and the roadmap's clause 6 now asks for the
amounts the retired executor reported and for the order's two consequences by name, with the
superseded clause quoted beside it. Under update rule 5 the existing evidence was re-evaluated rather
than carried: bundle `ubc-4-002` stands for the refusal the guest observes, and asserted no amount.
Bundle `ubc-4-004` retains the checks the revised clause asks for, and its README says what they do not
show. Clauses 4 and 5 stand as the row gives them.

*(Noted again on 2026-09-28, and the paragraph above is kept as what this file said when clause 6 was
met.)* UBC-4's clauses 4 and 5 were judged the same day, and they are still unmet, for a different reason.
The UBC-4 row said of them: "the predeclared rule judges both with one decision, which is NOT MET because
population A is not judged - the specification's test suite is not pinned and has no reader". That no
longer holds. The owner directed that the clauses be taken up once the suite turned out to be reachable,
and the suite is now pinned, with a reader in the harness root. A new dated rule quoting the old one was
committed before either, and before any command of the suite had run. Bundle `ubc-4-005` judges the
clauses under that rule, and its decision is NOT MET for the reason the row now gives. The state is
unchanged: the milestone is `In progress`, with clauses 4 and 5 unmet.

*(Moved again on 2026-09-28, and the paragraph above is kept as what this file said when clauses 4 and 5
were first judged.)* UBC-4's clauses 4 and 5 are met, under a rule revised after the runs it judges were
seen. The UBC-4 row said of them: "The decision is NOT MET on one condition: a set of NaN answers differs
between the two commits, every one passing the specification's verdict at both. The cause is the
family's NaN canonicalisation, which no class of the rule admits", and it named the holder as "the
WebAssembly profile owner, who either revises the rule in a new dated file quoting this one, written
after seeing those answers and saying so, or leaves the clauses unmet". The owner revised the rule the
same day. Bundle `ubc-4-006`'s rule quotes `ubc-4-005`'s whole, adds the one class those answers lacked,
and opens by saying it was not predeclared. The base run was taken again under it, and nothing else
moved. Every clause of UBC-4's gate is now met on retained evidence, and the row says of clauses 4 and 5
that their rule was written after the runs. The milestone is not accepted, because review is deferred,
so its state stays `In progress`.

---

## 1. Reading this ledger

The three categories the core ledger keeps apart are kept apart here, in its words:

- **Plan** is proposed scope, sequencing, ownership or an exit gate in the roadmap. It is not
  implementation or validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout. It can explain a
  status but cannot by itself satisfy a gate.
- **Accepted evidence** is an immutable, reviewable evidence bundle that identifies the exact sources
  and gate, records the executed commands and environment, retains their outputs, and demonstrates
  every part of the objective exit gate. Only accepted evidence may advance a milestone to
  `Accepted`, and under [`docs/mvp.md`](mvp.md) no milestone reaches it while review is deferred.

**This ledger records core-owned work and profile-owned work in the same programme, and keeps the
two apart by naming which is which.** A milestone that changes a language profile (UBC-3, UBC-4)
moves its row here for the programme's gate, and that profile's own ledger records the observed
repository state under its own legend; neither advances the other (core ledger update rule 6, in both
directions). A profile result never appears here at any strength, and no figure of any run appears
here at all.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation, record or bundle has been recorded. Planning text does not change this state. |
| `In progress` | The milestone owns code, records or a retained bundle, and at least one clause of its exit gate is unmet. The row names the unmet clauses individually. |
| `Blocked` | A named clause cannot be met because of a decision or precondition another party holds; the row names the holder and the unblock condition. |
| `Accepted` | Every clause of the exit gate is covered by accepted evidence and the owner and reviewer have recorded a decision. Unreachable while `docs/mvp.md` defers review. |

*(Added 2026-09-25, when the first row reached it.)* A milestone whose every clause is met on
retained evidence, and which is not accepted because [`docs/mvp.md`](mvp.md) defers review, stays
`In progress` and says both halves in its row. The definition above reads "at least one clause of
its exit gate is unmet", which such a row does not satisfy, and the core ledger's own definition -
work has begun "but the objective exit gate has not been accepted" - is the reading this ledger takes
for it. No fifth state is minted, because a state that meant "met but unaccepted" would read as
a verdict the owner has not given.

This ledger uses no bracketed mark tokens. It is not one of the review documents the architecture
rules H1 to H5 govern, and it adopts none of their legends; a reader comparing it with the core ledger
should read the words in the State column and nothing else.

---

## 2. Current milestone status

| Milestone | Objective, in one line | State | Unmet clauses / holder | Evidence | Last moved |
|---|---|---|---|---|---|
| UBC-0 | The extraction record, the records that move, the predeclared parity rules | `In progress` | **every clause met on retained evidence; not accepted**, because `docs/mvp.md` defers review (update rule 7). The verdict is an acceptance, of candidate A (the universal bytecode and its one loop); for candidate B (the native-form mechanism) the record carries the unsatisfied-G1 note ADR 0011 prescribes, which carries no verdict, so UBC-0.6's refusal branch is not reached | [bundle ubc-0-001](evidence/ubc-0-001/README.md); [ADR 0013](adr/0013-the-universal-bytecode-extraction-record.md); the three decision rules, first committed as `6486add` before any code of the milestones they judge | 2026-09-25 (records filed, bundle retained) |
| UBC-1 | `Broiler.VM.Ubc`: format, common family, table schema, primitive table, walk, corpus | `In progress` | **every clause met on retained evidence; not accepted**, because `docs/mvp.md` defers review (update rule 7). The walk's reservations of decoded rows are estimates the bundle names as such, and two adversarial reviews' findings are fixed in the commits it lists | [bundle ubc-1-001](evidence/ubc-1-001/README.md); the corpus `src/tests/corpus/ubc-1/`; the registry `docs/ubc/diagnostics/registry.txt`; the baseline `docs/ubc/api/public-api.txt`; rules U1, U2, U4, U8 and U9 | 2026-09-25 (code landed, bundle retained) |
| UBC-2 | `Broiler.VM.Emitter.Bytecode` and the fixture family | `In progress` | **every clause met on retained evidence; not accepted**, because `docs/mvp.md` defers review (update rule 7). One RID, `win-x64`, which is not a supported one; E2's differential check itself first has an emitter to run against at UBC-6a, and only its corpus is retained here; the fold check's Native AOT half is asserted, not excluded | [bundle ubc-2-001](evidence/ubc-2-001/README.md); the program and primitive corpora `src/tests/corpus/ubc-2/`; routes MVP-13 and MVP-14; the register row of `docs/compositions.md`; rules A11, A12 and A13 revised | 2026-09-25 (code landed, bundle retained) |
| UBC-5 | `Broiler.VM.Ubc.Native`, the `x86` pivot and the execution half | `Not started` | all; **it cannot meet clause 1 while ADR 0013's note that G1 is unsatisfied for the native-form mechanism stands**, because clause 1 writes that mechanism's assembly. Holder: the core architecture owner. Unblock condition, in ADR 0011's words: the second product profile - a second product profile's own emitted code over its bytecode, in merged code - or a ruling by that owner that moving one profile's native machinery into an emitter family is not an extraction between profiles | none | 2026-09-25 (reason recorded) |
| UBC-6a | The `x86-64` emitter over the fixture family | `Not started` | all; it cannot meet its gate while UBC-5 cannot, for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-3 | The JavaScript family | `Not started` | all; clause 10 is gated on decision UBC-D-1; **the whole milestone waits on UBC-6a** (roadmap section 10, "Waits on"), so it cannot meet its gate while UBC-5 cannot, for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-4 | The WebAssembly family | `In progress` | **every clause met on retained evidence, clauses 4 and 5 under a rule revised after the runs it judges were seen; not accepted**, because `docs/mvp.md` defers review (update rule 7). **Clause 3 is met**: its second half for the region rows on bundle `ubc-4-003`, where the harness's `--regions` lane holds the profile's own memory arms to the table's region primitive over the retained region input corpus in three modes, with a negative control. **Clauses 4 and 5 are met on bundle `ubc-4-006`, under a rule revised after the runs it judges were seen.** The specification's core test scripts are pinned at `wg-1.0` and read by a reader in the harness root, and population A is taken at the base `a56180b` and at the after commit. Bundle `ubc-4-005` judged the clauses first, under a new dated rule quoting `ubc-4-001`'s and committed before any command of the suite ran. Its decision is NOT MET on one condition: a set of NaN answers differs between the two commits, every one passing the specification's verdict at both, because the family canonicalises NaNs, and no class of that rule admits them. The WebAssembly profile owner chose to revise the rule. Bundle `ubc-4-006`'s rule quotes `ubc-4-005`'s whole, adds that one class, (n), and says it was written after those answers were seen; the base run was taken again under it. Every condition holds under it: both runs are deterministic, the run after is alike in three modes, every difference is class (f) or (n), population B is identical, the negative control fails and then passes, no verdict is worse, and the floor is re-based. The decision that was predeclared stays `ubc-4-005`'s NOT MET. **Clause 6 is met on its revised gate**: the WebAssembly profile owner kept the store's order on 2026-09-28, the roadmap records the gate revision with the superseded clause quoted (update rule 5), and bundle `ubc-4-004` retains the harness's retention checks in three modes - the amounts at instantiation, at a growth, at each kind of refused growth and at disposal, and the fuel a refused growth spends - with two negative controls, the retired executor's order and a changed amount, each failing. The RID `win-x64` only, but for clause 3's region rows and clauses 4, 5 and 6, on `linux-x64`, which is not a supported one; translation runs outside the core's accounts (route MVP-15); the move of a counterweight position that `WAC-41` records waits on that owner's confirmation | [bundle ubc-4-001](evidence/ubc-4-001/README.md), the base run and [the rule](evidence/ubc-4-001/decision-rule.md); [bundle ubc-4-002](evidence/ubc-4-002/README.md), the run after, the comparison, the negative control failing and passing, and the roots published and run in each mode; [WAD-0001 to WAD-0003](../src/Broiler.VM.Profile.WebAssembly/docs/decisions/README.md); that profile's `WAC-30` to `WAC-41` and `WAC-43`; route MVP-15; the register rows of `docs/compositions.md`; rules W1 and W2 revised; [bundle ubc-4-003](evidence/ubc-4-003/README.md), clause 3's region rows, and the corpus `src/tests/corpus/ubc-2/regions.txt`; [bundle ubc-4-004](evidence/ubc-4-004/README.md), clause 6 on its revised gate; [bundle ubc-4-005](evidence/ubc-4-005/README.md), clauses 4 and 5 under a new dated rule, NOT MET, and the suite pinned at `src/tests/wasm/spec`; [bundle ubc-4-006](evidence/ubc-4-006/README.md), clauses 4 and 5 under that rule revised by one class after its runs were seen, MET | 2026-09-28 (clauses 4 and 5 met on bundle `ubc-4-006`, under a rule revised after the runs it judges were seen; earlier that day judged NOT MET on bundle `ubc-4-005`, clause 6 met on bundle `ubc-4-004` and clause 3 on bundle `ubc-4-003`) |
| UBC-6b | The language families in the `x86-64` form | `Not started` | all; it waits on UBC-3 and UBC-4, and UBC-3 cannot meet its gate while UBC-5 cannot, for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-7 | The `arm64` emitter, emitting-only | `Not started` | all; clause 6 is gated on decision UBC-D-4; it waits on UBC-6b, and its pivot references `Broiler.VM.Ubc.Native`, so it cannot meet its gate for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-8 | The polyglot composition | `Not started` | all; it waits on UBC-3, which cannot meet its gate for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-9 | Records, the support table and packability | `Not started` | all; clause 4 is gated on decision UBC-D-5, which names `Broiler.VM.Ubc.Native`; the milestone is complete only after UBC-8, which cannot meet its gate for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-10 | The component split — **optional** | `Not started` | all; the milestone exists only on decision UBC-D-6, and it waits on UBC-6b and UBC-7, which cannot meet their gates for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |

**The rows are in delivery order, not numeric order**, because the roadmap's section 3 builds the
emitter machinery over the fixture family before the JavaScript family is cut over, and a reader who
wants to know what is next reads down.

### Decision points

| Decision | State | Holder |
|---|---|---|
| UBC-D-1 — retire the value form's substrate with the old JavaScript pipeline, or keep it behind opt-in roots | open | the JavaScript profile owner |
| UBC-D-2 — the WebAssembly memory representation for a region a native form addresses | **taken 2026-09-25**: a pinned managed array reallocated on growth with its base republished, recorded as [WAD-0001](../src/Broiler.VM.Profile.WebAssembly/docs/decisions/0001-the-memory-representation.md) | the WebAssembly profile owner with the security owner |
| UBC-D-3 — mint the WA-5 manifest or record its absence | **taken 2026-09-25**: the absence is recorded; the family's table stays under `broiler.webassembly.slice`, [WAD-0002](../src/Broiler.VM.Profile.WebAssembly/docs/decisions/0002-the-family-table-stays-under-the-slice-identity.md) | the WebAssembly profile owner |
| UBC-D-4 — whether the `arm64` emitter emits the handler-call form | open | the core architecture owner |
| UBC-D-5 — packability of `Broiler.VM.Ubc` and `Broiler.VM.Ubc.Native` | **taken early 2026-09-25, at UBC-4, for `Broiler.VM.Ubc` and `Broiler.VM.Emitter.Bytecode`**, which are packable, recorded in [ADR 0013](adr/0013-the-universal-bytecode-extraction-record.md)'s section of that date and [ADR 0001](adr/0001-component-topology-and-dependency-graph.md)'s revision; the fixture family and its root stay unpackable; **open** for `Broiler.VM.Ubc.Native`, which does not exist | the release owner with the architecture owner |
| UBC-D-6 — whether to split the repository into components | open | the repository owner |

All six holders are one person, recorded as EX-30.

---

## 3. Required evidence bundle

Each milestone retains its bundle under `docs/evidence/ubc-n-nnn/` in the core's bundle shape: a
`README.md` stating what the bundle demonstrates, what it excludes and what it does not claim; a
`manifest.json` naming every input, script and binary by hash and the commit each was built from; the
retained logs the roadmap's evidence-bundle field lists; and, where the milestone judges a comparison,
the `decision-rule.md` committed before the code it judges. A bundle holds figures where it must; no
document outside a bundle quotes them.

---

## 4. Update rules

1. Update this ledger in the same change that lands, blocks, supersedes or materially narrows a
   milestone's claim. Preserve earlier evidence links and decisions as dated history; correct a
   sentence by quoting the superseded reading beside the new one, never by editing it away.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state what
   it demonstrated, including failures and exclusions.
3. Do not infer completion transitively: UBC-2 met does not meet UBC-5; a fixture-family result does
   not meet a language family's clause; JIT, trimmed or one-mode success does not meet a Native AOT
   clause.
4. A milestone that owns any code, record or bundle is `In progress`, however little; a milestone with
   a gated clause whose decision is open is `Blocked` on that clause and `In progress` on the rest,
   which the row says in those words.
5. If a gate changes, record the gate revision in the roadmap with the superseded text quoted and
   re-evaluate existing evidence; evidence gathered for an older population is not carried forward
   silently.
6. Do not record a profile's status here and do not advance a profile's row from here. The programme's
   milestones UBC-3 and UBC-4 record their gates here; what those profiles' own ledgers say is theirs.
7. A milestone moves to `Accepted` only after its owner and a reviewer confirm every objective exit
   condition is covered, with the decision date and bundle identifier in the row. Under
   [`docs/mvp.md`](mvp.md) no row reaches it while review is deferred, and rule 8 of the core ledger —
   human review gates a release, not a development step — applies here unchanged: a milestone may be
   built, landed and evidenced unreviewed, and may not be published, claimed or accepted.
8. No figure enters this file: no count, timing, ratio or score, whether or not a bundle retains it.
   A row cites the bundle.
9. The optional milestone UBC-10 stays `Not started` until decision UBC-D-6 is recorded, and a
   decision not to split moves its row to `Not started` with the decision's date and closes it; the
   programme is complete at UBC-9 either way.
