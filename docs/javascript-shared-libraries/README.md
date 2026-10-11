# Shared JavaScript libraries: programme documents

**Date:** 2026-10-11. **Status:** planned. **Nothing here is done or accepted, and no record is
filed.** Every decision point was taken on 2026-10-11 ([decisions.md](decisions.md)); the records
that formalise the decisions are deliverables of the milestones. These documents turn
[the feasibility report](../javascript-shared-libraries-feasibility.md) into a programme. The
programme's milestones are `SJL-0` to `SJL-17`, and their state is recorded in
[the programme ledger](status.md). Every row is `Not started` on this date. *(Revised 2026-10-11,
when the decisions were taken. The status read "Nothing here is done, decided or accepted", and the
milestones ran to `SJL-16`.)*

**What the programme is for.** Broiler.VM's JavaScript profile family and Broiler.JS each
implement the same JavaScript language algorithms, data and source front end separately. The
programme gives each of those responsibilities **one maintained implementation**, in libraries
that name neither engine, and has **both engines consume them**. Broiler.JS stays an
independently usable, public reference engine built on the same libraries. Each engine keeps its
own value model, object model, execution machinery, compiler back end, host integration and
resource policy. The libraries hold language-level algorithms, data and syntax. They hold no
execution machinery.

**What these documents are not.**

- **They are not records.** Every record they name (an ADR, a profile decision, a correction, a
  route row, a Broiler.JS record) is a deliverable of the milestone that files it, and is not
  written here. Every choice that belonged to an owner is a *decision point* (`SJL-D-n`) with its
  alternatives. All were taken on 2026-10-11, and each is formalised only when the record its
  section names is filed.
- **They contain no measurements.** They state no timing, no count of a suite or a corpus, no
  ratio and no size, under the rule every plan in Broiler.VM applies to itself. Version
  identifiers, commit identifiers and dates are identifiers, not figures. A measurement belongs
  in an evidence bundle, behind a rule written before the measurement was taken.
- **They make no speed claim** in either direction, and plan no comparison of one engine's speed
  against the other's. Broiler.VM's release gate 10 forbids such a comparison.
- **They do not cite source line numbers.** Files and members are named instead, because line
  numbers go stale and Broiler.VM's review rule H3 forbids them in review documents.

---

## Which document is authoritative for what

| Document | Authoritative for | Not authoritative for |
|---|---|---|
| [Feasibility report](../javascript-shared-libraries-feasibility.md) | The original assessment and its reasoning, with dated corrections | Plans, state, decisions |
| [review.md](review.md) | The verification of every claim the feasibility report makes, and what the review added | Plans |
| [roadmap.md](roadmap.md) | Scope, delivery order, the critical path, standing obligations, and the summary of every milestone, rule and record | State; the full text of a milestone |
| [milestones/](milestones/) | Each milestone's work packages, deliverables, exit gate, evidence and exclusions | State |
| [status.md](status.md) | **What has happened**: milestone states, decision states, evidence links | Plans |
| [decisions.md](decisions.md) | The decision points: question, alternatives, recommendation, holder, the choice taken, and where its record is filed | The records themselves, which milestones file |
| [architecture.md](architecture.md) | The target library catalogue, layering, deployment boundaries and contract conventions | Per-area detail |
| [inventory.md](inventory.md) | The consolidated list of duplicated, single-engine and engine-specific responsibilities | Per-area migration detail |
| [areas/](areas/) | Per-area technical design: current state in each engine, differences, target API, adapters, data, vectors, deletion lists | Delivery order and gates |
| [governance.md](governance.md) | Repositories, owners, the records and rules each repository must change, cross-repository change and release flow, review, security, licensing | Technical design |
| [validation.md](validation.md) | Evidence layers, predeclared comparison rules, bundle shapes, the per-milestone evidence matrix | Gates, which are in the milestones |
| [risks.md](risks.md) | Risks and stop conditions | Mitigation work, which is in the milestones |
| [native-aot.md](native-aot.md) | Whether the decisions and the plan are compatible with Native AOT, and why | Gates, which are in the milestones; evidence, which bundles hold |

Where two documents disagree, **the milestone file wins over the roadmap summary, the ledger wins
over any plan about state, and a filed record wins over any document here.** A sentence in these
documents that a landed milestone makes false is corrected in place, with the superseded text
quoted and dated, as Broiler.VM's core documents do.

---

## Reading order

1. [review.md](review.md): what the feasibility report got right, what it got wrong, and what it
   left out.
2. [architecture.md](architecture.md): what the end state looks like.
3. [roadmap.md](roadmap.md): how the programme gets there, and in which order.
4. [decisions.md](decisions.md): what the owners decided, and where each decision is recorded.
   **The record of decision `SJL-D-7` has an external deadline** (see below).
5. The milestone files, the area files and the supporting documents, as needed.

---

## Document map

### Programme

- [roadmap.md](roadmap.md): the programme roadmap.
- [status.md](status.md): the programme ledger.
- [decisions.md](decisions.md): the decision register.
- [review.md](review.md): the claim-by-claim review of the feasibility report.

### Design

- [architecture.md](architecture.md): target architecture and contract conventions.
- [inventory.md](inventory.md): responsibility inventory across both engines and the existing
  component libraries.

### Areas

| Area | File | Delivered by |
|---|---|---|
| Number conversion, the pilot | [areas/numbers.md](areas/numbers.md) | SJL-3 |
| Integer conversions, Float16, Math kernels, BigInt | [areas/numerics.md](areas/numerics.md) | SJL-4 |
| Whitespace, UTF-16 helpers, substitution, URI, base64 and hex, JSON lexing | [areas/text.md](areas/text.md) | SJL-5 |
| Date arithmetic and the Date string grammar | [areas/date.md](areas/date.md) | SJL-6 |
| Unicode character data and algorithms | [areas/unicode.md](areas/unicode.md) | SJL-7 |
| Time-zone and calendar data | [areas/time-zones-and-calendars.md](areas/time-zones-and-calendars.md) | SJL-8 |
| Locale data and Intl algorithms, collation included | [areas/locale-data-and-intl.md](areas/locale-data-and-intl.md) | SJL-9; collation SJL-17 |
| Temporal core | [areas/temporal.md](areas/temporal.md) | SJL-10 |
| Regular expressions | [areas/regexp.md](areas/regexp.md) | SJL-11 |
| Tokenizer, parser, syntax tree, diagnostics | [areas/frontend.md](areas/frontend.md) | SJL-12, SJL-13, SJL-14 |
| Static semantics | [areas/static-semantics.md](areas/static-semantics.md) | SJL-15 |
| Shared conformance vectors | [areas/conformance-vectors.md](areas/conformance-vectors.md) | SJL-1, then every area |
| What stays in each engine, and why | [areas/retained-in-engines.md](areas/retained-in-engines.md) | (no milestone) |

### Milestones

| Milestone | File |
|---|---|
| SJL-0: records, decisions and baselines | [milestones/sjl-0-records-decisions-and-baselines.md](milestones/sjl-0-records-decisions-and-baselines.md) |
| SJL-1: the shared conformance vectors | [milestones/sjl-1-shared-conformance-vectors.md](milestones/sjl-1-shared-conformance-vectors.md) |
| SJL-2: the admission path | [milestones/sjl-2-the-admission-path.md](milestones/sjl-2-the-admission-path.md) |
| SJL-3: number conversion, the pilot | [milestones/sjl-3-number-conversion-pilot.md](milestones/sjl-3-number-conversion-pilot.md) |
| SJL-4: numerics | [milestones/sjl-4-numerics.md](milestones/sjl-4-numerics.md) |
| SJL-5: text algorithms | [milestones/sjl-5-text-algorithms.md](milestones/sjl-5-text-algorithms.md) |
| SJL-6: Date arithmetic and the Date string grammar | [milestones/sjl-6-date.md](milestones/sjl-6-date.md) |
| SJL-7: Unicode character data and algorithms | [milestones/sjl-7-unicode.md](milestones/sjl-7-unicode.md) |
| SJL-8: time-zone and calendar data | [milestones/sjl-8-time-zones-and-calendars.md](milestones/sjl-8-time-zones-and-calendars.md) |
| SJL-9: locale data and Intl algorithms | [milestones/sjl-9-locale-data-and-intl.md](milestones/sjl-9-locale-data-and-intl.md) |
| SJL-10: Temporal core | [milestones/sjl-10-temporal.md](milestones/sjl-10-temporal.md) |
| SJL-11: regular expressions | [milestones/sjl-11-regexp.md](milestones/sjl-11-regexp.md) |
| SJL-12: the frontend contract and in-place preparation | [milestones/sjl-12-frontend-contract.md](milestones/sjl-12-frontend-contract.md) |
| SJL-13: the shared syntax library and the VM wide path | [milestones/sjl-13-shared-syntax-and-vm.md](milestones/sjl-13-shared-syntax-and-vm.md) |
| SJL-14: Broiler.JS on the shared syntax library | [milestones/sjl-14-broiler-js-syntax.md](milestones/sjl-14-broiler-js-syntax.md) |
| SJL-15: shared static semantics | [milestones/sjl-15-static-semantics.md](milestones/sjl-15-static-semantics.md) |
| SJL-16: close-out | [milestones/sjl-16-close-out.md](milestones/sjl-16-close-out.md) |
| SJL-17: collation | [milestones/sjl-17-collation.md](milestones/sjl-17-collation.md) |

### Process

- [governance.md](governance.md): repositories, owners, records, rules, change and release flow,
  review, security, licensing.
- [validation.md](validation.md): evidence and test strategy.
- [risks.md](risks.md): risks and stop conditions.
- [native-aot.md](native-aot.md): the Native AOT assessment of every decision and area, and the
  corrections it made.

---

## The one deadline that does not wait

Broiler.VM's JavaScript profile planned to start from a **snapshot copy** of Broiler.JS (its
milestone JS-2, ledger state `Blocked`). Profile decision JSD-0005 gives that snapshot a stop
condition: it is taken as-is on **2026-11-30**, or earlier once a stated number of further commits
has landed on the seed's default branch. JS-2 settles only when "either the core contract is
accepted, or a record says the seed copy will not be taken and JS-2's gate is re-scoped". **A programme that
shares the front end instead of copying it makes the copy pointless.** Decision `SJL-D-7` was taken
on 2026-10-11: JS-2 is re-scoped, and the seed copy is not taken. **That does not disarm the stop
condition; only a filed JSD does.** SJL-0.4 files that JSD before 2026-11-30, or, failing that, a
JSD that revises the stop condition. See [decisions.md](decisions.md#sjl-d-7). *(Revised
2026-10-11, when the decision was taken. The paragraph ended "Decision `SJL-D-7` (the re-scope of
JS-2) therefore has to be taken before that date, or explicitly deferred with the stop condition
revised in the same record.")*

---

## Identifiers this programme uses

| Identifier | Meaning | Where it is minted |
|---|---|---|
| `SJL-n` | A milestone, `SJL-0` to `SJL-17`. SJL-17, collation, was added on 2026-10-11 by decision `SJL-D-24`, and comes before SJL-16 in delivery order. | [roadmap.md](roadmap.md) |
| `SJL-n.m` | A work package of milestone `SJL-n` | The milestone file |
| `SJL-D-n` | A decision point | [decisions.md](decisions.md) |
| `sjl-n-nnn` | An evidence bundle of milestone `SJL-n`, in the repository that collects it | The milestone file names it; the bundle holds it |
| Rule group `I` (proposed) | Broiler.VM architecture rules governing **imported** shared libraries (`I1` to `I7`) | Minted at SJL-2. `I` is one of the letters no register, ADR, roadmap or ledger uses. If the owner prefers the JavaScript family's `N` group, only the identifiers move. |
| Rule group `SL` (proposed) | Architecture rules of the shared libraries themselves, in their home repository | Minted at SJL-2 |
| ADR, JSD, JSC, MVP and EX numbers | Records in the existing Broiler.VM series | **Taken at filing time**, as the next free number. On this document's date the next free numbers are ADR 0014, JSD-0063, JSC-290, MVP-16, EX-125 and roadmap-amendment register row 24. These documents name records by subject, not by a number that another change may take first. |

`SJL-`, `JSL-`, `SHL-` and `SL-` occur nowhere in either repository on this date. `JSH-` is already
taken by the JavaScript profile's hosting stages.

---

## How this programme relates to other plans

| Plan | Relationship |
|---|---|
| [Broiler.VM roadmap](../roadmap.md) | Sections 5, 9, 12, 14, 15 and 16 state that Broiler.VM references no legacy Broiler component, that the JavaScript profile is a one-way fork of Broiler.JS, and that no evidence transfers. SJL-0 revises those sentences through the roadmap-amendment register; it does not edit them by implication. See [governance.md](governance.md#2-positions-this-programme-reverses-or-narrows). |
| [The universal bytecode programme](../universal-bytecode.roadmap.md) | Its milestone UBC-3 cuts the JavaScript family over to the universal bytecode, and profile decision JSD-0036 says "the front end returns a validated tree; the lowering consumes it and emits universal bytecode". The frontend milestones (SJL-12 to SJL-15) must land on whichever lowering exists when they run, and must not be shaped by a lowering that does not exist yet. Neither programme waits on the other. |
| JavaScript profile plans (`src/Broiler.VM.Profile.JavaScript/docs/`) | The profile's invariants 11 and 12, its JS-2 milestone, and decisions JSD-0005, JSD-0011, JSD-0014, JSD-0027 and JSD-0031 each take a position this programme revisits. Every change goes through a new profile decision and correction entries; see [governance.md](governance.md). |
| Broiler.JS roadmaps (`D:\Broiler.JS\docs\roadmap\`) | Broiler.JS's own `FrontEnd/Semantics` boundary (its dependencies document and work package MOD-M2-5) is a hypothesis shared between Broiler.JS's IL and portable back ends, not started. SJL-12 to SJL-15 make that boundary the shared frontend. Broiler.JS's documents say "No Broiler.VM integration is planned for this component". SJL-0 revises that statement in Broiler.JS's own records. |
| Broiler.Unicode, Broiler.Regex, Broiler.DateTime | Existing published component libraries. Broiler.JS already consumes the first two. The programme weighed them as owners **before** creating anything new in their area (decision `SJL-D-1`): Broiler.Unicode owns the Unicode area under SJL-7.1's conditions, Broiler.Regex is fixed to Broiler.VM's requirements and owns regular expressions (decision `SJL-D-14`), and Broiler.DateTime was not chosen for time data. |
| Broiler.JSeal | The host-facing engine abstraction layer *above* both engines. The shared libraries sit *below* them. JSeal is not changed. Its package gate constrains how shared packages may be named, and the neutral prefix decision `SJL-D-2` took passes it; see [decisions.md](decisions.md#sjl-d-2). |
| [Component and .NET reuse investigation](../component-reuse-investigation.md) | A sibling report of the same date about the existing component libraries. Its findings are used here where they hold. [review.md](review.md) records where its findings do not hold, including its claim that Broiler.DateTime's civil-day primitives were made public: they are internal in every committed state. |

---

## What the programme does not do

- **It does not unify the value model, the object model or execution.** Broiler.JS's public
  `JSValue` class hierarchy and Broiler.VM's internal tagged `JsValue` struct stay as they are, and
  so do object storage, frames, dispatch, compilers' back ends, emitted formats, verification,
  resource budgets, suspension and host integration. Whole-runtime unification is recorded as
  **not planned** (decision `SJL-D-18`), not as deferred.
- **It does not share built-in object implementations.** `Array`, `Promise`, `Proxy`, `RegExp`
  objects, `JSON` drivers, property protocols and species lookups stay in each engine. Only the
  algorithms underneath them move.
- **It does not change Broiler.JSeal** or any host's binding surface.
- **It does not publish, advertise or claim anything.** Publication stays under each repository's
  own release rules. In Broiler.VM, [docs/mvp.md](../mvp.md) keeps review and acceptance
  deferred, and no milestone here moves a support claim beyond what a cell may already say.
- **It does not make Broiler.JS's non-standard extensions shared.** The `Decimal` type and `m`
  literals, `Number.prototype.clz`, `String.prototype.contains` and the .NET format-string
  extension to `toLocaleString` stay in Broiler.JS (decision `SJL-D-17`).
- **It does not move evidence between repositories.** Each repository's gates are met by
  evidence that repository collected. Agreement between the engines is a cross-check, never an
  oracle: shared code means shared bugs, so the specification and Test262 stay the oracles.

---

## Glossary

| Term | Meaning here |
|---|---|
| **Engine** | Broiler.VM's JavaScript profile family (`Broiler.VM.Profile.JavaScript`, `.Compiler`, `.Format`, `.Intl`), or Broiler.JS. |
| **Shared library** | A library that implements a JavaScript language algorithm, data set or front-end stage, names neither engine, and is consumed by both. |
| **Component library** | An existing published Broiler library outside both engines: Broiler.Unicode, Broiler.Regex or Broiler.DateTime. |
| **Home** | The repository that owns a shared library's source, tests, data pins and releases. |
| **Area** | A coherent set of duplicated responsibilities moved in one milestone, for example number conversion. |
| **Adapter** | The engine-side code that turns engine values into shared-library inputs and shared-library results into engine values and errors. It keeps coercion order, property access, exception types, error timing and charging policy. It holds no algorithm of the area. |
| **Cut-over** | The change in which an engine stops using its own implementation of an area and starts using the shared library. |
| **Deletion list** | The named members each engine removes at cut-over. A cut-over that leaves a member on its list in place has not removed the duplication. |
| **Retired duplicate** | A deleted member, recorded so that an architecture rule can refuse its return. |
| **Reference arm** | A test-only copy of an engine's pre-cut-over implementation, kept in the shared library's test suite so the new implementation can be compared against it input by input. |
| **Vector** | One shared conformance test case: an operation, its inputs, the expected result, its provenance and, where the specification allows engines to differ, the classification of that difference. |
| **Observable change** | A difference a script can see. Every observable change at a cut-over is classified: a correction of the engine toward the specification, an implementation-defined difference, or a regression. |
| **Admission** | Broiler.VM's act of allowing a named, pinned shared package into a named project's references, through architecture rules that would otherwise refuse it. |
| **Pin** | A recorded version, revision or content hash of external input (a package, a data set, a test suite) that a rule checks. |
