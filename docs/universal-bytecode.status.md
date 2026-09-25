# The universal bytecode programme — status ledger

**Last updated:** 2026-09-25 (UBC-0 recorded; UBC-3 and UBC-5 to UBC-10 given the reason they cannot meet their gates)

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
| UBC-1 | `Broiler.VM.Ubc`: format, common family, table schema, primitive table, walk, corpus | `Not started` | all | none | 2026-09-25 (row created) |
| UBC-2 | `Broiler.VM.Emitter.Bytecode` and the fixture family | `Not started` | all | none | 2026-09-25 (row created) |
| UBC-5 | `Broiler.VM.Ubc.Native`, the `x86` pivot and the execution half | `Not started` | all; **it cannot meet clause 1 while ADR 0013's note that G1 is unsatisfied for the native-form mechanism stands**, because clause 1 writes that mechanism's assembly. Holder: the core architecture owner. Unblock condition, in ADR 0011's words: the second product profile - a second product profile's own emitted code over its bytecode, in merged code - or a ruling by that owner that moving one profile's native machinery into an emitter family is not an extraction between profiles | none | 2026-09-25 (reason recorded) |
| UBC-6a | The `x86-64` emitter over the fixture family | `Not started` | all; it cannot meet its gate while UBC-5 cannot, for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-3 | The JavaScript family | `Not started` | all; clause 10 is gated on decision UBC-D-1; **the whole milestone waits on UBC-6a** (roadmap section 10, "Waits on"), so it cannot meet its gate while UBC-5 cannot, for the reason UBC-5's row gives | none | 2026-09-25 (reason recorded) |
| UBC-4 | The WebAssembly family | `Not started` | all; clause 7 is gated on decisions UBC-D-2 and UBC-D-3 | none | 2026-09-25 (row created) |
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
| UBC-D-2 — the WebAssembly memory representation for a region a native form addresses | open | the WebAssembly profile owner with the security owner |
| UBC-D-3 — mint the WA-5 manifest or record its absence | open | the WebAssembly profile owner |
| UBC-D-4 — whether the `arm64` emitter emits the handler-call form | open | the core architecture owner |
| UBC-D-5 — packability of `Broiler.VM.Ubc` and `Broiler.VM.Ubc.Native` | open | the release owner with the architecture owner |
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
