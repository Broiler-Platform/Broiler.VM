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

### SJL-15.6 — Evidence

Both engines' full suites under the rules:

- Broiler.VM's artifacts byte-identical or classified;
- the slice corpus: census counts, slot order and refusals, before and after;
- determinism in two runs and on two threads;
- fuzzing;
- three modes;
- each compiler's own measurement.

---

## Exit gate

**Home clauses**

1. The semantic model is implemented under the contract.
2. Every catalogue row is covered.
3. Rule SL9 and the Native AOT probe pass.

**Broiler.VM clauses**

4. The binder pass is gone, and the lowering reads the model.
5. The slice path is rebuilt on the shared model: `SliceParser` is gone, and the slice corpus's
   census counts, slot order and refusals are byte-identical before and after.
6. Artifacts are byte-identical, or every difference is classified.
7. test262 in both forms meets the rule.
8. Rules N5 to N9, N12, N16 and N19 hold.
9. Every root is published and run in three modes.

*(Revised 2026-10-11, when SJL-D-13 was taken as (a). Clause 5 read "The slice path follows decision
`SJL-D-13`." Clause 8 read "Rules N5 to N9, N12, N16 (where revised) and N19 hold.")*

**Broiler.JS clauses**

10. Its scope analysis and its scattered early errors are gone.
11. test262 per path meets the rule. Every correction is listed.

**Records**

- The profile's JSD for decision `SJL-D-13`, with its JSC entries.
- Broiler.JS's record closing invariant 3.

---

## Evidence bundles

`docs/evidence/sjl-15-001/` in each repository.

---

## Does not do

- **It does not share a lowering, an intermediate form or an emitter.** Broiler.VM's lowering to
  its bytecode (or to the universal bytecode, under UBC-3) and Broiler.JS's lowering to expression
  trees stay their own.
