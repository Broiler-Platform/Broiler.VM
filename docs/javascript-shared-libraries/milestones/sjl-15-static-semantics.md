# Milestone SJL-15 — Shared static semantics

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/static-semantics.md)

- **Objective.** The frontend's semantic model is the single source of the facts listed below, and
  both lowerings consume it. Broiler.VM's binder pass, Broiler.JS's scope analysis and every early
  error outside the frontend are gone. Broiler.VM's slice path is rebuilt on the shared tree and
  model, as decision [SJL-D-13](../decisions.md#sjl-d-13) (a) took it.
  The facts are:
  - scopes and their kinds;
  - declarations and conflicts;
  - Annex B.3.3 aliases;
  - per-function flags;
  - reference resolution with dynamic markers;
  - labels and jump targets;
  - private-name environments;
  - module records;
  - named-evaluation names;
  - free names and captures;
  - every early error.
- **Owner roles.** The Broiler.JS maintainer (home, Broiler.JS) and the JavaScript profile owner
  (Broiler.VM). One person.
- **Repositories.** Broiler.JS (home and consumer), Broiler.VM.
- **Waits on.** SJL-13 and SJL-14. It applies decision [SJL-D-13](../decisions.md#sjl-d-13), taken
  on 2026-10-11: (a), the slice path rebuilt, keeping its census counts, slot order and refusals byte
  for byte.

**What each engine keeps.** Each engine keeps its own lowering decisions:

| Broiler.VM keeps | Broiler.JS keeps |
|---|---|
| Manifest admission and its refusals | Its per-iteration `for` lowering |
| Slot and environment-record allocation | Numeric speculation |
| Evaluation scope-map rows | Boxing and deferral |
| Format ceilings | Its expression-tree emission |
| Verification | Its run-time evaluation support |
| Byte determinism | |

---

## Work packages

### SJL-15.1 — Comparison rules

The rules are those of SJL-13 and SJL-14, applied to the semantic half. They are committed first.
They add:

- the slice corpus: its census counts, its slot order and its refusals, in Broiler.VM, which must be
  byte-identical before and after the rebuild. This is the comparison decision `SJL-D-13` once
  planned for choosing between (a) and (b), now the rebuild's acceptance check;
- Broiler.JS's closure-sensitive suites.

*(Noted 2026-11-16, at SJL-15.1, by [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md), decisions D-verdict, D-windows and D-census.)* The text above names
no area verdict, and roadmap section 5 asks the first work package of every area milestone, SJL-15 included, to file
one. SJL-15.1 files it: JSD-0088, this family's half, Broiler.JS's half of its X2 and X6 completed by a dated note on it
before any code of SJL-15. "The rules are those of SJL-13 and SJL-14, applied to the semantic half. They are committed
first." is read as bundle [`sjl-15-001`](../../evidence/sjl-15-001/decision-rule-area.md)'s rule header reads it: bundles `sjl-13-001`'s and `sjl-13-002`'s parts applied at
this family's two windows, SJL-15.3's and SJL-15.4's, and Broiler.JS's bundle `sjl-14-001`'s class T applied to this
family's half by decision D64; Broiler.JS's rule for SJL-15 is filed in its own bundle `sjl-15-001`. "The slice corpus:
its census counts, its slot order and its refusals ... byte-identical before and after the rebuild" is that rule's part
S, which admits no difference; what it means for the outputs that parsing with the library moves, in three classes
(the corrections of hand-offs H23 and H25, the refusals the library makes in place of the slice path's own, and the
census lines its parse moves), is put to the owner (decision D-census), and clause 5 stands as written meanwhile. "Broiler.JS's closure-sensitive suites" are Broiler.JS's rule's;
this family's rule names its own as its part H. The text above is left as written.
*(Noted 2026-11-16, after SJL-15.1 merged, on the owner's answer to decision D-census.)* The note above read "is put to
the owner (decision D-census), and clause 5 stands as written meanwhile". The owner chose "Admit listed rows" the same
day, so clause 5 is revised below, before any evidence, and part S admits the three classes, each listed by row, by
bundle `sjl-15-001`'s rule revision
[`decision-rule-area-revised-2026-11-16b.md`](../../evidence/sjl-15-001/decision-rule-area-revised-2026-11-16b.md);
every other byte difference still fails part S.

### SJL-15.2 — The semantic model

The frontend gains the semantic model of [areas/static-semantics.md](../areas/static-semantics.md),
deterministic and free of ambient state. It is built against:

- the early-error catalogue of SJL-12, every row;
- test262's early-error tests;
- both engines' semantic test suites.

*(Added 2026-10-26, at SJL-13.1, by [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md), decision D10.)* The model
grows from the declarations walk SJL-13.2 begins in `Broiler.ECMAScript.Semantics`: SJL-13's syntax half already
reports every early error either engine's parser reports at parse time, and exposes each scope's kind and
declarations, each function's parameter names and each program's and function's strictness as side tables ordered by
source position. SJL-15.2 adds the rest of the model to that walk; it does not start a second one.

*(Noted 2026-11-16, at SJL-15.1, by [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md), decisions D-pin and D61.)* "SJL-15.2 adds the rest of the model
to that walk; it does not start a second one." says where the model grows, and not when its code may merge. SJL-15.2's code is Broiler.JS's, in `Broiler.ECMAScript.Frontend`, which Broiler.JS's
engine references by project: until Broiler.JS's SJL-14 cut-over (its window W2), each merge of it is a dark window of
SJL-14, judged by part B of Broiler.JS's bundle `sjl-14-001` with the switch off, and a change that moves a switch-off
outcome lands only inside a cut-over window (SJL-13's plan's decision D52: "The same holds for SJL-15's model stages");
none merges while SJL-11.5's bases are held. The `semantics` vectors land in Broiler.JS's corpus before it (SJL-1.6;
decision D61), and it comes after both repositories' SJL-15.1 rules. This family reads the model only from a stream
version the owner publishes (JSD-0083's decision D30), admitted by a pin commit of its own before SJL-15.3's base. The
text above is left as written.
*(Noted 2026-11-23, after SJL-11.5's merge in Broiler.JS, observed; the note above is left as written.)* The note
above reads "none merges while SJL-11.5's bases are held". On Broiler.JS's default branch, at `69a8a13b`: SJL-11.5
merged as Broiler-Platform/Broiler.JS#1126, `b2b226cd3158f0b7d990a3f4fecbe75b259b4fa1`, whose first parent is its
base commit `5429c406`, and "The hold of decision D16 ended with that merge" (Broiler.JS's
`docs/roadmap/SharedLibraries.md`, SJL-11); its after records merged as Broiler-Platform/Broiler.JS#1127,
`fefb4f9c9331b7d16349ed2a1721285f2fb69fef`. That plan's note of 2026-11-19 on its SJL-15 list reads "That hold ended
with SJL-11.5's merge (Broiler-Platform/Broiler.JS#1126, `b2b226cd`)"; that under Broiler.JS's reading B4 of SJL-14 "W2
merges before any SJL-15.2 model merge, so none is such a dark window", and from W2's first base run until W2 merges
nothing merges to its default branch, by the owner's answer 9 of 2026-11-19, "Allow the W2 hold", the owner's own:
"The hold then starts at the first base run, and nothing merges to Broiler.JS main until W2 merges"; and that
"SJL-15.2's model still waits on the later corpus revision this note names", the one with the eval-code inputs of
probes V029, V030 and V046, which that branch names next and does not yet hold. W2 is not merged there. So no merge of
SJL-15.2's model is held for SJL-11.5 any longer: none comes before W2, none during W2's hold, and none before that
corpus revision. Nothing is revised.
*(Noted 2026-11-24, after W2's merge in Broiler.JS, observed; the notes above are left as written.)* The note above
reads "W2 is not merged there" and "none comes before W2, none during W2's hold, and none before that corpus revision".
W2, Broiler.JS's SJL-14 cut-over, merged as Broiler-Platform/Broiler.JS#1129,
`83a9bd82a5d258d3001468095ebf43f9efc3e9a7`, and "**The hold of the owner's answer 9 ended** with that merge"
(Broiler.JS's bundle `sjl-14-001`'s README, at `4f697339d2b360b422d4c0e32898cefca3423d95`, the merge of
Broiler-Platform/Broiler.JS#1130). So of the three, only that corpus revision is still to come before any merge of
SJL-15.2's model; Broiler.JS's default branch, at `4f697339`, does not hold it yet, its
`conformance/vectors/REVISIONS.md` listing revision 43 last. The first note's "until Broiler.JS's SJL-14 cut-over (its
window W2), each merge of it is a dark window of SJL-14" has no subject from W2's merge on. Nothing is revised.
*(Noted 2026-11-24, later, at Broiler.JS's corpus revision 44, observed; the notes above are left as written.)* The
note above reads "only that corpus revision is still to come before any merge of SJL-15.2's model; Broiler.JS's default
branch, at `4f697339`, does not hold it yet". It merged as Broiler-Platform/Broiler.JS#1131,
`8b1fa311da6af71baac6378e8b4f8597a99c1312`: corpus revision 44, the eval-code inputs of probes V029, V030 and V046 as
`DirectEval` vectors, classified on both engines' runs (Broiler.JS's `conformance/vectors/REVISIONS.md`, row 44). So
nothing that note names is still to come, and Broiler.JS's records name SJL-15.2's model next: "so SJL-15.2's model comes
next, then SJL-15.5, then the evidence" (its `docs/roadmap/SharedLibraries.md`, the note of 2026-11-24 on "The order of
SJL-15's Broiler.JS work"). This family still reads the model only from a stream version the owner publishes, admitted
by its pin commit before SJL-15.3's base. Nothing is revised.

### SJL-15.3 — Broiler.VM's lowering consumes it

The binder pass of SJL-12 is replaced by the shared model, and is deleted. The lowering reads:

- **resolution:** static (hops, slot) where the model says static, and dynamic for `with` and for
  sloppy direct `eval`;
- **`arguments` need**, which is precise instead of an over-approximation;
- **direct-eval contamination**;
- **Annex B aliases**;
- **private names**;
- **labels**.

**Slot allocation stays in the lowering**, because it is a back-end decision.

### SJL-15.4 — The slice path is rebuilt

Under decision [SJL-D-13](../decisions.md#sjl-d-13) (a), taken on 2026-10-11:

- **`SliceStaticSemantics` is rebuilt over the shared model.** It keeps its census counts, its slot
  order and its deliberate refusal of unresolvable identifiers, byte for byte over the slice corpus.
  The refusal becomes a slice admission rule over the model's resolution.
- **`SliceParser` is deleted**, and the slice path parses with the shared syntax library.
- **The slice lowering stays**, with its slot allocation, because that is a back-end decision.
- **Rule N16 is not revised.** The slice manifest keeps its own path, so no manifest moves to the
  wide path.

The profile's JSD records the decision, with JSC entries for the plan sentences it changes.

*(Revised 2026-10-11, when SJL-D-13 was taken. The work package offered (a) and a second branch,
"(b) The slice manifest compiles through the wide path under slice admission, and rule N16 is
revised. The slice path's parser and semantics are deleted.", and ended "**Keeping a separate slice
parser is admissible only as a recorded refusal.**")*

### SJL-15.5 — Broiler.JS's compiler consumes it

The following go:

- the compiler's `FastFunctionScope`, `FreeNameScan`, the scope half of `FastScope`;
- `SyntaxValidation`, `FastCompiler.EarlyErrors`, and the early-error checks in `Visit*` methods and
  `DirectEvalSupport.Validate`.

Broiler.JS's Modernization invariant 3 ("The semantic front end is shared … between IL and
bytecode") is closed, extended to the Broiler.VM consumer.

*(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-15 existed, by SJL-13's plan's decision D41 as
[JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md)
revised it, the owner's decision of 2026-11-03, recorded in Broiler.JS's `docs/roadmap/SharedLibraries.md`, section
"Broiler.JS on the shared syntax library (SJL-14)". Until then the work package named only what goes.)* **The
compiler also moves here from the bridged `AstNode` tree SJL-14 gives it to the shared tree**, reading the model.
At SJL-14 the compiler parses through the bridge, which builds its old tree from the shared tree, and only
`PortableCompiler` and `ModuleSourceAnalysis` read the shared tree directly (SJL-14.3 as revised the same day).
After SJL-15.5 nothing in the engine reads the bridge, and the bridge remains only as the implementation of the
obsolete `Ast` and `Parser` adapter packages, until their removal version. Clause 10 judges the move.

### SJL-15.6 — Evidence

Both engines' full suites under the rules:

- Broiler.VM's artifacts byte-identical or classified;
- the slice corpus: census counts, slot order and refusals, before and after;
- determinism in two runs and on two threads;
- fuzzing;
- three modes;
- each compiler's own measurement.

*(Noted 2026-11-16, at SJL-15.1, by [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md), decision D-measure.)* "Each compiler's own measurement" is a rule part
of each bundle (template T5), not a gate clause, as SJL-13 read its own ("The compiler's own measurement under its rule"
is "a rule part of each bundle (template T5), not a gate clause", the note of 2026-10-26 under SJL-13.6). The exit gate
below has no measurement clause, and none is added. Bundle [`sjl-15-001`](../../evidence/sjl-15-001/decision-rule-area.md)'s part J is this family's. The text above is left
as written.

---

## Exit gate

**Home clauses**

1. The semantic model is implemented under the contract.
2. Every catalogue row is covered.
3. Rule SL9 and the Native AOT probe pass.

**Broiler.VM clauses**

4. The binder pass is gone, and the lowering reads the model.
5. The slice path is rebuilt on the shared model: `SliceParser` is gone, and the slice corpus's
   census counts, slot order and refusals are byte-identical before and after, but for the corrections the
   correspondence lists by row before any run (hand-offs H23 and H25), the refusals the library makes in place of the
   slice path's own, and the census lines the library's parse moves, each input listed with its row and class.
   *(Revised 2026-11-16, after SJL-15.1 merged, before any evidence of SJL-15 existed, under the
   [ledger](../status.md)'s update rule 5, by the owner's answer to
   [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md)'s
   decision D-census: the clause read "The slice path is rebuilt on the shared model: `SliceParser` is gone, and the
   slice corpus's census counts, slot order and refusals are byte-identical before and after." The question put to the
   owner was "SJL-15 clause 5 requires the slice corpus's outputs to be byte-identical before and after the slice path
   moves onto the shared library. Moving it necessarily changes three kinds of output: the corrections of hand-offs H23
   and H25, the refusals the library now makes in place of the slice path's own, and census lines its parse moves. How
   should clause 5 read?", with the options "Admit listed rows" (recommended) and "Keep as written". The owner chose
   "Admit listed rows": "A dated revision now, before any evidence, admits those three classes, each listed by row.
   Every other byte difference still fails part S." Parsing the slice path with the library (decision D60) moves those
   three classes of the slice corpus's outputs, so the clause as written could not be met by any rebuild that parses
   with the library; admitting them changes what the clause means, which is a gate revision, not a reading. The words
   are JSD-0088's alternative (ii). Bundle `sjl-15-001` files the class of each by a new dated rule file,
   [`decision-rule-area-revised-2026-11-16b.md`](../../evidence/sjl-15-001/decision-rule-area-revised-2026-11-16b.md),
   before any run of its part S: class H by its rows now, and classes R and C, whose inputs and lines only a run can
   show, from the window-2 base run's reports, the listing committed or retained with the time it was written before
   any after run of window 2 is taken. This is the owner's own decision, not one taken under the owner's direction of
   2026-10-18. No evidence of SJL-15 existed to re-evaluate.)*
6. Artifacts are byte-identical, or every difference is classified.
7. test262 in both forms meets the rule.
8. Rules N5 to N9, N12, N16 and N19 hold.
9. Every root is published and run in three modes.

*(Revised 2026-10-11, when SJL-D-13 was taken as (a). Clause 5 read "The slice path follows decision
`SJL-D-13`." Clause 8 read "Rules N5 to N9, N12, N16 (where revised) and N19 hold.")*

*(Noted 2026-11-16, at SJL-15.1, by [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md), decision D-census, which is put to the owner and not taken.)* Clause 5
is not revised here. Its "the slice corpus's census counts, slot order and refusals are byte-identical before and after"
admits no difference, and parsing the slice path with the shared library (decision D60) moves three classes of the slice
corpus's outputs: JSD-0083's hand-offs H23 and H25, corrections of the slice path the library makes; the refusals the
library makes in place of the slice path's own, under its codes and as its first refusal; and the census lines the
library's parse moves. An input of the slice corpus in any of them cannot be both moved and byte-identical. The owner
chose SJL-D-13 (a) on 2026-10-11, so what the clause means for those inputs is the owner's: (i) as written, or (ii)
revised before any evidence under update rule 5 to admit the classes listed by row, each input listed with its class. Until the owner chooses,
clause 5 stands as written, and bundle [`sjl-15-001`](../../evidence/sjl-15-001/decision-rule-area.md)'s part S judges it so.

*(Noted 2026-11-16, after SJL-15.1 merged, on the owner's answer to decision D-census: revised. The note above said
"Until the owner chooses, clause 5 stands as written"; on 2026-11-16 the owner chose "Admit listed rows", described as
"A dated revision now, before any evidence, admits those three classes, each listed by row. Every other byte difference
still fails part S." That is alternative (ii). Clause 5 is revised above, its old text quoted beside it, and bundle
`sjl-15-001`'s part S admits classes H, R and C, each listed by row, by its rule's revision of the same day; every other
byte difference fails part S. This is the owner's own dated choice, not one taken under the owner's direction of
2026-10-18. No evidence of SJL-15 existed to re-evaluate.)*

**Broiler.JS clauses**

10. Its scope analysis and its scattered early errors are gone, and the compiler reads no bridged
    tree.
11. test262 per path meets the rule. Every correction is listed.

*(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-15 existed, under the
[ledger](../status.md)'s update rule 5, by SJL-13's plan's decision D41 as JSD-0083 revised it. Clause 10 read "Its
scope analysis and its scattered early errors are gone." At SJL-14 the compiler parses through a bridge to its own
tree, and SJL-15.5 moves it to the shared tree; without the added words, no exit clause would judge that move.
Broiler.JS's rule for SJL-15, filed at SJL-15.1, judges it by its deletion search. No evidence of SJL-15 existed to
re-evaluate.)*

**Records**

- The profile's JSD for decision `SJL-D-13`, with its JSC entries.
- Broiler.JS's record closing invariant 3.

*(Noted 2026-11-16, at SJL-15.1.)* "The profile's JSD for decision `SJL-D-13`, with its JSC entries." and the line after
it name no area verdict. The verdict is [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md), filed at SJL-15.1
(its decision D-verdict); the JSD for decision `SJL-D-13` is still SJL-15.4's. The list is left as written.

---

## Evidence bundles

`docs/evidence/sjl-15-001/` in each repository.

---

## Does not do

- **It does not share a lowering, an intermediate form or an emitter.** Broiler.VM's lowering to
  its bytecode (or to the universal bytecode, under UBC-3) and Broiler.JS's lowering to expression
  trees stay their own.
