# The shared JavaScript libraries programme — roadmap

**Owners.** The milestones span four repositories, and each work package names the owner of the
repository it changes:

- **Broiler.VM:** the architecture owner, the JavaScript profile owner, and the six roles of
  [ADR 0012](../adr/0012-security-ownership-and-support-matrix.md).
- **Broiler.JS:** the Broiler.JS maintainer.
- **Broiler.Unicode and Broiler.RegEx:** each component's maintainer.

On this document's date **every one of those roles is held by one person**, recorded in Broiler.VM
as EX-30. No gate below is independently confirmed until a second holder exists.

**Date:** 2026-10-11. **Core contract version:** 1, unchanged by anything here.

**Status:** planned work and objective exit gates. **Nothing here is done, and nothing here is
accepted.** [The programme ledger](status.md) is the only authority on what has happened. This
document and the [milestone files](milestones/) are the authority on what is planned and what would
count. Every milestone is `Not started` on this date.

**What this document is.** It turns
[the feasibility report](../javascript-shared-libraries-feasibility.md), as corrected by
[the review](review.md), into milestones `SJL-0` to `SJL-17`. For each milestone it says which work
packages build it, what it delivers, what evidence it retains, and what a run must decide before
its gate is met.

**The decisions it follows.** Every decision point in [decisions.md](decisions.md) was taken on
2026-10-11: the recommendation wherever one was made, and (a) for `SJL-D-13`, `SJL-D-14` and
`SJL-D-23`. The plan below follows those choices. Where a choice changed an exit gate, the milestone
file quotes the clause it replaces. No decision is recorded yet; the records are deliverables of the
milestones that [section 8](#8-decision-points) names.

**What this document is not.**

- **It is not the ledger** and moves no row in any ledger.
- **It is not a decision.** Every record a milestone files is named as a deliverable and is not
  written here. Every owner's choice is a decision point in [decisions.md](decisions.md).
- **It carries no figure of any kind:** no timing, no count of a suite or a corpus, no ratio. That
  is the rule every plan in Broiler.VM writes for itself.
- **It makes no speed claim** in either direction. It plans no comparison of one engine's speed with
  the other's, which Broiler.VM's release gate 10 forbids.
- **It does not override Broiler.VM's MVP terms** ([`docs/mvp.md`](../mvp.md)). Approval of records,
  human review, evidence acceptance and amendment co-signing stay deferred. The automated gates, the
  status vocabulary, the stop condition on an untruthful support claim and the non-advertisement of
  every composition stay in force at every milestone.

---

## Contents

1. [Scope](#1-scope)
2. [Principles every milestone follows](#2-principles-every-milestone-follows)
3. [How to read a milestone](#3-how-to-read-a-milestone)
4. [The delivery order and the critical path](#4-the-delivery-order-and-the-critical-path)
5. [The programme's extraction gate](#5-the-programmes-extraction-gate)
6. [What every milestone owes before it lands](#6-what-every-milestone-owes-before-it-lands)
7. [The milestones at a glance](#7-the-milestones-at-a-glance)
8. [Decision points](#8-decision-points)
9. [The evidence and test matrix](#9-the-evidence-and-test-matrix)
10. [Rules minted or revised, by milestone](#10-rules-minted-or-revised-by-milestone)
11. [Records filed, by milestone](#11-records-filed-by-milestone)
12. [Risks and stop conditions](#12-risks-and-stop-conditions)
13. [The ledger, and how this roadmap is corrected](#13-the-ledger-and-how-this-roadmap-is-corrected)

---

## 1. Scope

### 1.1 The request

Turn the feasibility report into a fully detailed roadmap that leaves nothing out. The report is
`docs/javascript-shared-libraries-feasibility.md` of 2026-10-11. The report's assessment is that
Broiler.JS can become the home of reusable JavaScript libraries, with its current engine remaining
an independently usable public reference implementation built on them, and that "the most
promising outcome is shared language processing and algorithms, with each engine retaining its own
execution machinery".

### 1.2 What is planned

| Theme | Milestones |
|---|---|
| **Foundation**: the records that must move in four repositories, the owners' decisions, the baselines, a shared conformance-vector corpus, and the dependency-admission path | SJL-0, SJL-1, SJL-2 |
| **Language algorithms**: number conversion as the pilot, then numerics, text algorithms and Date | SJL-3, SJL-4, SJL-5, SJL-6 |
| **Data**: Unicode with its existing owner, then time-zone and calendar data, then locale data and the Intl algorithms over it, then collation | SJL-7, SJL-8, SJL-9, SJL-17 |
| **Temporal core** | SJL-10 |
| **Regular expressions** with an existing owner | SJL-11 |
| **The front end**: the contract and in-place preparation, the shared syntax library with Broiler.VM's wide path, Broiler.JS on it, then shared static semantics | SJL-12, SJL-13, SJL-14, SJL-15 |
| **Close-out**: records, support tables, the maintenance model, and confirmation of what is not planned | SJL-16 |

### 1.3 What is left alone

- **Execution machinery.** Value representation, object storage, frames, dispatch, suspension,
  compilers' back ends, emitted forms, verification, resource budgets, host integration and realm
  lifecycle stay with each engine. Whole-runtime unification is **not planned** (decision
  `SJL-D-18`).
- **Built-in object implementations.** Only algorithms move; the objects that call them stay. See
  [areas/retained-in-engines.md](areas/retained-in-engines.md).
- **Language surface growth.** No milestone makes either engine support more JavaScript as a goal.
  Where a shared implementation corrects an engine, the correction is a consequence, classified and
  listed (decision `SJL-D-16`). Surface work belongs to each engine's own plans: the JavaScript
  profile's `JS-`, `JSW-` and `JSP-` stages, and Broiler.JS's phases and Modernization items.
- **Performance.** No milestone is justified by a speed. Where a cut-over could slow an engine, the
  bundle carries that engine's own candidate-versus-control measurement under a rule written first,
  and no document outside the bundle quotes it.
- **Review, acceptance, advertisement, publication.** These stay under each repository's own rules.
  In Broiler.VM they stay deferred under `docs/mvp.md`.
- **The core contract.** No milestone amends it. The one reading the programme takes of a
  contract-bearing record, ADR 0011's scope, is a route recorded at SJL-0 (decision `SJL-D-8`).
- **Broiler.JSeal and every host's binding surface.**
- **Broiler.JS's non-standard extensions** (decision `SJL-D-17`).

### 1.4 Repositories touched

Each milestone file lists the repositories it changes. Over the programme they are:

- Broiler.VM, which consumes the libraries and holds this roadmap and its ledger;
- Broiler.JS, which hosts the algorithms, Intl, frontend and vector libraries and the time-zone,
  calendar and locale-data generators with their pins, and is a consumer;
- Broiler.Unicode, which owns the Unicode work under SJL-7.1's owner conditions;
- Broiler.RegEx, which owns the regular-expression engine.

Broiler.DateTime and Broiler.JSeal are not changed. Decision `SJL-D-1` placed time data in the
Broiler.JS repository, and the neutral prefix of decision `SJL-D-2` passes Broiler.JSeal's package
gate.

---

## 2. Principles every milestone follows

1. **One maintained implementation per responsibility.** An area is finished when both engines use
   the shared implementation **and** the deletion list is gone from both. A shared library beside
   two surviving copies is the failure the feasibility report warns about: "adding another
   abstraction layer while retaining both original implementations".
2. **Algorithms and data move; execution stays.** See [architecture.md](architecture.md).
3. **Neutral contracts.** No engine type, no coercion, no property access, no guest callback,
   failure as data, explicit limits, caller-owned metering, determinism, no ambient state. The full
   list is [architecture.md section 4](architecture.md#4-contract-conventions).
4. **Existing owners first.** Broiler.Unicode and Broiler.Regex own their areas. Broiler.DateTime
   was weighed for time data and not chosen (decision `SJL-D-1`). Nothing new is created where an
   existing owner meets the conventions.
5. **Vectors before code.** Every area's shared vectors exist, and have been run against both
   engines' current implementations with every difference classified, before the shared
   implementation lands (SJL-1, then each area).
6. **The specification and Test262 stay the oracles.** Shared code means shared bugs, so agreement
   between engines is a cross-check and never an oracle.
7. **No evidence transfers.** Each repository's clauses are met by evidence that repository
   collected.
8. **Deployment boundaries are rules, not hopes.** Execution-only compositions gain no frontend,
   and optional data stays optional. Broiler.VM's architecture rules enforce both on published
   closures.
9. **No capability is lost between milestones.** A cut-over removes an engine's implementation only
   in the change that makes the shared one live, and the change's evidence shows no verdict worse
   than the predeclared rule allows.
10. **Every step is refusable.** An area that fails the extraction gate (section 5) is refused with
    a dated record, and its duplicated implementations carry source-level pointers to that record.
    A refused area does not stop the programme, and a refused pilot does not either: decision
    `SJL-D-23` was taken before the pilot, to continue. *(Revised 2026-10-11: the last sentence
    read "A refused pilot does (decision `SJL-D-23`).")*

---

## 3. How to read a milestone

Each milestone file states the same fields, in the same order, and a field that is empty says so.

| Field | Meaning |
|---|---|
| **Objective** | One sentence: what is true after the milestone that was not true before. |
| **Owner roles** | The roles that own the milestone, per repository. All one person today. |
| **Repositories** | The repositories the milestone changes. |
| **Waits on** | Milestones whose exit gates must be met first, and decision points that must be taken first. A milestone may *start* before its predecessors are accepted (human review gates a release, not a development step) and may not *meet its gate* before they meet theirs. |
| **Work packages** | Numbered `SJL-n.m`. Each states what it builds or moves, where, and its own done condition. A work package is a unit of change, not of time; nothing here has a duration. |
| **Deliverables** | The code, records, rules and evidence the milestone leaves behind, by name. |
| **Exit gate** | Numbered clauses a run can decide, grouped as **Broiler.VM clauses**, **Broiler.JS clauses** and **component clauses**. Each group is met by evidence collected in its own repository. A clause is met or it is not, and a milestone with an unmet clause is `In progress`. |
| **Evidence bundles** | The bundle directories, one per repository that has clauses (`docs/evidence/sjl-n-nnn/` in each), and what each retains. A bundle is evidence and never a verdict. |
| **Does not do** | What a reader might infer and may not. |

**Identifiers** are listed in [README.md](README.md#identifiers-this-programme-uses). Records in
existing series (ADR, JSD, JSC, MVP, EX) take the next free number at filing time.

---

## 4. The delivery order and the critical path

```text
SJL-0  records, decisions, baselines (four repositories; no product code)
  │      └─ SJL-D-7's JSD (JS-2) filed before 2026-11-30
  │
SJL-1  shared conformance vectors: format, home, runners in both engines, number vectors
  │      (test-only; Broiler.VM consumes the corpus as a pinned archive, so no admission is needed)
  │
SJL-2  the admission path: Broiler.VM rules with witnesses, records, tooling;
  │      the shared-library foundation in the Broiler.JS repository (conventions, SL rules,
  │      API baseline, assurance gate, pack/publish stream, Native AOT probe)
  │
SJL-3  PILOT — number conversion: both engines on Broiler.ECMAScript.Algorithms, deletion lists gone
  │      └─ the programme continues (SJL-D-23, taken before the pilot)
  │
  ├──────────────┬───────────────┬──────────────────────────┬────────────────────┐
SJL-4 numerics  SJL-5 text      SJL-6 Date                 SJL-7 Unicode        SJL-12 frontend contract
  │              algorithms      arithmetic and grammar     (Broiler.Unicode)     and in-place preparation
  │                               │                          │                    (may start after SJL-0;
  │                               │                          ├──────────┐          its exit waits on SJL-3)
  │                    SJL-8 time-zone and calendar data     │          │                │
  │                               │                          │   SJL-11 RegExp           │
  │                    SJL-10 Temporal core (SJL-6, SJL-8)   │   (Broiler.Regex)         │
  │                               │                          │                           │
  │                    SJL-9 locale data and Intl (SJL-7, SJL-8; rounding from SJL-4)    │
  │                               │                                                      │
  │                    SJL-17 collation, shared after SJL-9 (SJL-D-24)                   │
  │                                                                                      │
  │                               SJL-13 shared syntax library + Broiler.VM wide path ◄──┘
  │                                      (SJL-3 literal values, SJL-7 identifier classes;
  │                                       regular-expression literals through SJL-11 or a validator seam)
  │                                         │
  │                               SJL-14 Broiler.JS on the shared syntax library
  │                                         │
  │                               SJL-15 shared static semantics (both lowerings); slice path rebuilt (SJL-D-13)
  │                                         │
  └─────────────────────────────────────────┴──► SJL-16 close-out (every area met or refused)
```

**The critical path** is SJL-0 → SJL-1 → SJL-2 → SJL-3 → SJL-7 → SJL-13 → SJL-14 → SJL-15 → SJL-16.
SJL-12 runs beside it from SJL-0 onward, because its in-place preparation changes nothing across
repositories.

**Off the critical path, in any order once SJL-3 is met:**

- SJL-4, SJL-5 and SJL-6;
- SJL-8 once SJL-3 is met, and SJL-10 once SJL-6 and SJL-8 are met;
- SJL-9 once SJL-7 and SJL-8 are met, and SJL-17 once SJL-9 is met;
- SJL-11 once SJL-7 is met.

*(Added 2026-10-13: SJL-4 started before SJL-3 was met, by the owner's dated choice, which the
[ledger](status.md) records. The order above is unchanged, and so is what it gates: SJL-4 may not
meet its gate before SJL-3 meets SJL-3's.)*

*(Added 2026-10-14: SJL-5 started before SJL-3 was met, by the owner's dated choice, which the
[ledger](status.md) records. The order above is unchanged, and so is what it gates: SJL-5 may not
meet its gate before SJL-3 meets SJL-3's.)*

*(Added 2026-10-15: SJL-6 begins with work package SJL-6.0, Broiler.VM's hosted test262 evidence
workflow, queued by the owner before any other work of SJL-6 and before SJL-6.1's rules are written.
It changes no product code, so it may start before SJL-3 is met. The order above is unchanged.)*

*(Added 2026-10-15, at SJL-6.4: SJL-6's later work packages, SJL-6.1 to SJL-6.4, went ahead before SJL-3
was met, by the owner's direction of each, which the [ledger](status.md) records late. The order above is
unchanged, and so is what it gates: SJL-6 may not meet its gate before SJL-3 meets SJL-3's.)*

*(Added 2026-10-16, at SJL-6.5: so did SJL-6.5, Broiler.JS's cut-over, by the owner's direction of
2026-10-16, which the [ledger](status.md) records. The order and what it gates are unchanged.)*

*(Added 2026-10-16: SJL-7 started before SJL-3 was met, by the owner's dated choice, which the
[ledger](status.md) records. SJL-7 is on the critical path; the order above is unchanged, and so is what it
gates: SJL-7 may not meet its gate before SJL-3 meets SJL-3's, and SJL-9, SJL-11 and SJL-13 still wait on
SJL-7's.)*

**What can start today:**

- SJL-0 in full;
- the vector format of SJL-1;
- the in-place preparation work packages of SJL-12, in each repository's own terms: Broiler.JS
  removes ambient parse state and the parser's runtime reference; Broiler.VM separates declaration
  analysis from emission.

Nothing else, because everything else crosses a repository boundary that SJL-2 opens.

**The time-critical item** is the record of decision `SJL-D-7`, which was taken on 2026-10-11
(re-scope). Until SJL-0.4 files its JSD, profile decision JSD-0005's stop condition still takes the
Broiler.JS seed snapshot as-is on 2026-11-30, and the programme would then share code with a fork of
itself.

**A refusal at SJL-0 ends the programme with its records filed**, and that is a passing gate for
SJL-0. The two decisions that could cause one, `SJL-D-8` (the ADR 0011 reading) and `SJL-D-9` (the
fork policy), were taken on 2026-10-11 as recommended. A later ruling that reverses either would
still end the programme this way. **A refusal of any area, the pilot's included,** ends that area
only, because decision `SJL-D-23` was taken before the pilot, to continue. *(Revised 2026-10-11:
the paragraph read, in part, "**A refusal of the pilot at SJL-3 ends the programme after the
pilot** (decision `SJL-D-23`). **A refusal of any later area** ends that area only.")*

---

## 5. The programme's extraction gate

Broiler.VM's own extraction gate (ADR 0011, G1 to G4) governs sharing between Broiler.VM profiles.
It cannot be invoked here, because the second implementation is in another engine, not another
profile (decision `SJL-D-8`). The programme applies **its own gate, of the same shape**, at the
start of every area milestone (SJL-3 to SJL-15, and SJL-17). The area's first work package files the
verdict.

| Condition | Statement |
|---|---|
| **X1. Two implementations** | Both engines implement the responsibility in merged code, named by file, member and revision. A responsibility only one engine implements is an **adoption**, not an extraction. It may still move, and it is recorded as such. |
| **X2. Correspondence from code** | A table compares the two implementations at stated revisions. **Every** behavioural difference is classified: the engine is wrong (a correction under `SJL-D-16`), the specification leaves it to the implementation, an engine policy (a limit, a default) that becomes a parameter, or unknown (not admissible). |
| **X3. Neutral contract** | The shared part is expressible under [architecture.md section 4](architecture.md#4-contract-conventions): no engine type, no coercion, no property access, failure as data, explicit limits, metering wherever work grows with input. |
| **X4. No edge between engines, none into Broiler.VM** | The shared library references neither engine and nothing in Broiler.VM. Neither engine references the other. |
| **X5. Boundaries hold** | Every consuming closure stays within its deployment boundary: no frontend in a compiler-free root, optional data optional. No platform globalization or regular-expression API enters. |
| **X6. Deletion list** | The area names, per engine, every member the cut-over deletes, including intra-engine copies, and the disposition of every public Broiler.JS type it empties (`SJL-D-25`). |
| **X7. Owners accept** | The JavaScript profile owner, the Broiler.JS maintainer and, where relevant, the component maintainer record acceptance. |

**On refusal**, the verdict names the failed condition and what would reopen it. Each duplicated
implementation gains a source-level pointer to the record, so the duplication stays visible.
Broiler.VM's roadmap section 8: "A duplication with no record is the failure mode this rule exists
to prevent."

---

## 6. What every milestone owes before it lands

These are not gate clauses. They are the repositories' standing rules, restated so that a milestone
cannot be reported met while owing one of them.

1. **Ledgers.**
   - The programme ledger's row moves in the same change that lands, blocks or narrows the
     milestone, under the update rules in [status.md](status.md#4-update-rules).
   - Broiler.JS's shared-libraries status file moves in the same change on its side.
   - Neither ledger advances any other ledger's row. The JavaScript profile's ledger records
     observed state under its own legend.
2. **Assurance.**
   - **In Broiler.VM**, every new or moved unit carries its annotation, the generated artefacts are
     regenerated in the same change, and rules J1 to J12 pass. Code that leaves Broiler.VM is named
     in an exclusion of the bundle that moves it.
   - **In a home repository**, rule SL8 passes.
   - A moved unit starts unreviewed.
3. **Rules.** A minted or revised rule carries a witness, watched failing when injected and passing
   after revert, named in its register row. Register rows cite figures and never type them (rule
   J12).
4. **Graph.**
   - Broiler.VM's `graph.manifest.json`, including the package-edge field SJL-2 adds, and ADR 0001's
     budget sentence say what the tree holds, in the same change (rules A7 and A15).
   - `docs/compositions.md` declares every shared assembly each root carries (rules K2 and K4).
5. **Corrections.** A plan sentence the change makes false is corrected where its document's
   discipline says:
   - inline, with the superseded text quoted, in Broiler.VM's core documents;
   - a JSC entry with a bare pointer, in the JavaScript profile's plans;
   - a dated revision, in Broiler.JS's documents.
6. **Figures.** No count, timing, ratio or score enters any plan, ledger, decision or support
   document. A bundle README may hold a figure its bundle retains; nothing else may quote it.
7. **Claims.** No milestone adds a supported runtime identifier, an advertised composition or a
   review decision. A support-table cell moves only on retained evidence and only to say what it
   already may.
8. **Publish and run.**
   - **Broiler.VM:** a change to any root's closure is published and run under JIT, trimming and
     Native AOT, and the closure read off the published output is retained. A linker annotation is
     not a run.
   - **Home repositories:** the Native AOT probe runs the vectors.
9. **Notices.** A new runtime dependency's third-party notices land in the same change as the
   dependency (Broiler.VM ADR 0001), in the engine and in the shared package.
10. **Vectors first, rules first.** An area's vectors are run against both engines' current code
    before the shared implementation lands. Its comparison rules are committed **before** any code
    they judge, which a bundle demonstrates by ancestry.
11. **Deletion in the same change.** A cut-over deletes its deletion list in the same change that
    switches. No fallback copy, no switch and no dead path survives it.
12. **Pins.** Every package, data set, specification revision and test-suite revision the change
    relies on is pinned. A pin moves only by a recorded decision.

---

## 7. The milestones at a glance

Every decision point was taken on 2026-10-11, so the "Waits on" column names milestones only. Each
decision's record is filed by the work packages that [decisions.md](decisions.md#summary) names.

| Milestone | Objective | Waits on | Repositories | File |
|---|---|---|---|---|
| **SJL-0** | The four repositories' records say what the programme does. The programme decisions, taken on 2026-10-11, are recorded. The baselines exist. Every comparison rule is predeclared. | Nothing | Broiler.VM, Broiler.JS, Broiler.Unicode, Broiler.RegEx | [sjl-0](milestones/sjl-0-records-decisions-and-baselines.md) |
| **SJL-1** | A shared vector corpus exists, with a schema and runners in both engines, and number vectors run against both engines' current code with every difference classified | SJL-0 | Broiler.JS (home), Broiler.VM (runner, pinned archive) | [sjl-1](milestones/sjl-1-shared-conformance-vectors.md) |
| **SJL-2** | A Broiler.VM JavaScript-family project can consume a named, pinned, VM-free shared package under rules that refuse anything else. A home repository can produce such a package under its own rules. | SJL-0 | Broiler.VM, Broiler.JS | [sjl-2](milestones/sjl-2-the-admission-path.md) |
| **SJL-3** | Both engines convert numbers to and from strings through one shared implementation, and neither keeps its own | SJL-1, SJL-2 | Broiler.JS (home, consumer), Broiler.VM | [sjl-3](milestones/sjl-3-number-conversion-pilot.md) |
| **SJL-4** | Integer conversions, Float16, Math kernels, exact rounding and the BigInt kernel are shared | SJL-3 | Broiler.JS, Broiler.VM | [sjl-4](milestones/sjl-4-numerics.md) |
| **SJL-5** | Whitespace, UTF-16 helpers, padding, substitution, URI, base64 and hex codecs, and JSON lexing and quoting are shared | SJL-3 | Broiler.JS, Broiler.VM | [sjl-5](milestones/sjl-5-text-algorithms.md) |
| **SJL-6** | Date arithmetic, the Date string grammar and Date formatting are shared, with local-time policy as a provider | SJL-3 | Broiler.JS, Broiler.VM | [sjl-6](milestones/sjl-6-date.md) |
| **SJL-7** | Unicode character data and algorithms come from one pinned owner, with exact ECMAScript names and caller-owned metering | SJL-3 | Broiler.Unicode, Broiler.JS, Broiler.VM | [sjl-7](milestones/sjl-7-unicode.md) |
| **SJL-8** | One pinned time-zone dataset and one set of calendar tables serve both engines, optional where the engines make them optional | SJL-3 | Broiler.JS, Broiler.VM | [sjl-8](milestones/sjl-8-time-zones-and-calendars.md) |
| **SJL-9** | Locale data and the ECMA-402 algorithms over it are shared, collation apart | SJL-7, SJL-8 | Broiler.JS, Broiler.VM | [sjl-9](milestones/sjl-9-locale-data-and-intl.md) |
| **SJL-10** | The Temporal core (records, balancing, durations, rounding, the RFC 9557 grammar, the calendars) is shared | SJL-6, SJL-8 | Broiler.JS, Broiler.VM | [sjl-10](milestones/sjl-10-temporal.md) |
| **SJL-11** | One regular-expression engine, Broiler.Regex with a caller-owned budget and exact ECMAScript syntax, serves both engines | SJL-7 | Broiler.RegEx, Broiler.JS, Broiler.VM | [sjl-11](milestones/sjl-11-regexp.md) |
| **SJL-12** | The shared front end's contract is recorded, and both engines' front ends are prepared in place to meet it | SJL-0 (start); SJL-3 (exit) | Broiler.JS, Broiler.VM | [sjl-12](milestones/sjl-12-frontend-contract.md) |
| **SJL-13** | The shared syntax library exists, and Broiler.VM's wide path parses with it | SJL-12, SJL-3, SJL-7 | Broiler.JS (home), Broiler.VM | [sjl-13](milestones/sjl-13-shared-syntax-and-vm.md) |
| **SJL-14** | Broiler.JS parses with the shared syntax library, and its own parser is retired | SJL-13 | Broiler.JS | [sjl-14](milestones/sjl-14-broiler-js-syntax.md) |
| **SJL-15** | Scopes, bindings, early errors and hoisting facts come from one shared analysis that both lowerings consume, and Broiler.VM's slice path is rebuilt on it | SJL-13, SJL-14 | Broiler.JS, Broiler.VM | [sjl-15](milestones/sjl-15-static-semantics.md) |
| **SJL-16** | The programme's records, support tables and maintenance model describe the end state, and what is not planned is confirmed | Every area met or refused | All | [sjl-16](milestones/sjl-16-close-out.md) |
| **SJL-17** | Both engines collate through one shared implementation of the Unicode Collation Algorithm over CLDR root and its tailorings | SJL-9 | Broiler.JS, Broiler.VM | [sjl-17](milestones/sjl-17-collation.md) |

---

## 8. Decision points

Every decision point was taken on 2026-10-11. [decisions.md](decisions.md) lists each with its
alternatives, the recommendation, the holder and the choice. **Broiler.VM's records of them are
filed at SJL-0, and Broiler.JS's and the component repositories' are not yet**; [the
ledger](status.md) says which decision is recorded where. A gate clause that needs a decision is met
only by its record. *(Revised 2026-10-11, at SJL-0's first records: the second sentence read "**None
is recorded yet**, and a gate clause that needs a decision is met only by its record.")*

**SJL-0 records most of them:**

- `SJL-D-7`'s JSD, before 2026-11-30 (SJL-0.4);
- ADR 0014 (SJL-0.2), carrying `SJL-D-1` to `SJL-D-5`, `SJL-D-8`, `SJL-D-9`, `SJL-D-17`, `SJL-D-18`
  and `SJL-D-23`;
- the route row and ADR 0011's pointer paragraph, the profile JSD, the Broiler.JS plan and the
  component roadmaps (SJL-0.3 to SJL-0.6), carrying their parts of these decisions and of `SJL-D-6`,
  `SJL-D-14` and `SJL-D-16`.

**Each area decision is recorded by the milestone that applies it**, as the summary table of
decisions.md shows.

**Three choices change the plan's shape:**

- **`SJL-D-23`, continue, taken before the pilot.** SJL-3 no longer decides whether the programme
  continues. Its criteria are SJL-3's exit clauses.
- **`SJL-D-24` (c).** Collation leaves SJL-9 and becomes milestone SJL-17.
- **`SJL-D-6` (c).** Each engine embeds tables that shared generators produce from shared pins, and
  no data package is published.

*(Superseded 2026-10-11, when the decisions were taken. The section read: "All open on this date.
They are listed in full in [decisions.md](decisions.md), with alternatives, recommendation, holder
and the clause each blocks." It then listed `SJL-D-7`, `SJL-D-8`, `SJL-D-9`, `SJL-D-17` and
`SJL-D-18` as on SJL-0's path, and said "SJL-2 additionally needs `SJL-D-1` to `SJL-D-5`.")*

---

## 9. The evidence and test matrix

What each milestone runs, where, and what it retains. "Three modes" means published and run under
JIT, trimming and Native AOT. Details, comparison-rule templates and bundle shapes are in
[validation.md](validation.md).

| Milestone | Runs | Where | Retains |
|---|---|---|---|
| SJL-0 | Whole-suite test262 at the common pin in both engines, by their own runners. Broiler.VM: bytecode and native forms. Public API listings, package contents and closures. Each engine's own measurements for the pilot's operations. | Each repository | The base runs, the listings, the decision-rule files |
| SJL-1 | The vector corpus against both engines' current implementations; the corpus's own schema checks | Broiler.JS test project; a Broiler.VM harness root over the pinned archive | Per-engine vector results with every difference classified; the corpus pin |
| SJL-2 | Every new or revised rule's witness, failing and then passing; the shared-library foundation's SL rules; the Native AOT probe; a pack and pristine restore through each consumption source | Broiler.VM architecture tests; the home repository's CI | Witness logs, the pack, the restore logs |
| SJL-3 | Vectors; the reference-arm comparison; test262 base against after in both engines under the predeclared rule; Broiler.VM fuel-parity twins; three modes for every Broiler.VM root; package and pristine-consumer checks; each engine's own candidate-versus-control measurement | Both repositories | Verdict files and comparisons, closures, nuspecs, measurement logs, the deletion check |
| SJL-4 to SJL-6 | As SJL-3, per area | Both | As SJL-3 |
| SJL-7 | As SJL-3, plus the Unicode test files (`NormalizationTest`, the break tests, case folding); the data-agreement rule; regenerated tables byte for byte | Broiler.Unicode, Broiler.JS, Broiler.VM | As SJL-3, plus generation logs |
| SJL-8 | As SJL-3, plus transition comparisons over the pinned dataset; the Temporal reference datasets | Both; Broiler.JS is also the data home | As SJL-3 |
| SJL-9 | As SJL-3, plus the retained Intl reference datasets with their declared divergences | Both | As SJL-3 |
| SJL-17 | As SJL-3, plus the retained `orderings` reference dataset with its declared divergences | Both | As SJL-3 |
| SJL-10 | As SJL-3, plus the retained Temporal polyfill dataset in both engines | Both | As SJL-3 |
| SJL-11 | As SJL-3, plus adversarial-pattern resource tests, test262's RegExp directories, and the component's own corpus | Broiler.RegEx and both engines | As SJL-3 |
| SJL-12 | Each engine's own parse corpora; test262's negative syntax tests; determinism over two runs and two threads; the in-place changes' own gates | Both | Diagnostics distributions, determinism logs |
| SJL-13 | The above through the shared library in Broiler.VM: retained source corpus, diagnostic registry rules, byte-identical artifacts, the concurrency gate, source fuzzing | Broiler.VM, home | As SJL-3, plus artifact comparisons |
| SJL-14 | Old against new tree comparisons over Broiler.JS's corpora and test262; the portable compiler's tests; the Native AOT sample, regenerated, published and run | Broiler.JS | As SJL-3 |
| SJL-15 | Both lowerings' full suites under the predeclared rules; byte-identical Broiler.VM artifacts; the slice corpus's census counts, slot order and refusals | Both | As SJL-3 |
| SJL-16 | The sweep of every record and deletion list against the trees | All | The sweep |

*(Added 2026-10-15: from SJL-6, Broiler.VM's whole-suite test262 runs for a bundle are taken by the
hosted evidence workflow of SJL-6.0, as Broiler.JS's already are, and SJL-6's rule takes both its base
and its after runs there. Until then they are taken on one local machine.)*

**What no milestone runs:** a comparison of one engine's speed against the other's; a run on a
runtime identifier Broiler.VM's ADR 0012 does not declare; any test whose result a plan document
would quote.

---

## 10. Rules minted or revised, by milestone

Broiler.VM rules are in its register. `SL` rules are in the shared libraries' own register.
[governance.md section 3](governance.md#3-rules-each-repository-mints-or-revises) states each one.

| Milestone | Broiler.VM minted | Broiler.VM revised | Shared libraries (`SL`) |
|---|---|---|---|
| SJL-0 | (none) | (none: the register moves no rule; records only) | (none) |
| SJL-1 | (none) | N13's suite-directory list (the vector archive is test-only) | (none) |
| SJL-2 | I1, I2, I3, I6, I7 | A2, B3 (to `Active`), N1, N37, A7 with `graph.manifest.json`, E4 | SL1 to SL10 |
| SJL-3 | I5 | K2 and K4 with the compositions register; N11's corpus remarks | Per-area vectors |
| SJL-4 to SJL-6 | (none) | I5 rows added | (none) |
| SJL-7 | I4 | N22 kept in Broiler.VM, regenerating with the owner's generator from Broiler.VM's pin, which `I4` holds equal to the owner's (`SJL-D-6` (c)); N23 and N18 remarks and vacuity guards | SL7 for Unicode |
| SJL-8 | (none) | N29 to N32 kept, regenerating with the shared generator, and given the register rows they lack | SL7 for time data |
| SJL-9 | (none) | N27, N28 kept, regenerating with the shared generator | SL7 for locale data |
| SJL-10 | (none) | (none beyond I5) | (none) |
| SJL-11 | (none) | N18 vacuity guard re-pointed; N10 baseline (the matcher's public types leave Format) | (none) |
| SJL-12 | (none) | (none: in-place changes keep N12, N19) | (none) |
| SJL-13 | (none) | N5 to N9 over the registry mapping; N12, N19 remarks for code that moved | Frontend determinism (SL9) |
| SJL-14 | (none) | (none: Broiler.JS only) | (none) |
| SJL-15 | (none) | (none: the slice path is rebuilt, not retired, so N16 stands, `SJL-D-13`) | (none) |
| SJL-16 | (none) | Every rule whose remarks the end state made stale | (none) |
| SJL-17 | (none) | (none beyond I5) | (none) |

---

## 11. Records filed, by milestone

| Milestone | Broiler.VM core | JavaScript profile | Broiler.JS | Components |
|---|---|---|---|---|
| SJL-0 | ADR 0014 (not contract-bearing), carrying the programme decisions, `SJL-D-23` among them; ADR 0003 §11 register rows for roadmap sections 1, 5, 9, 12, 14, 15 and 16 (`Proposed`); the ADR 0011 pointer paragraph; the README and core-ledger corrections; a route row in `docs/mvp.md`; the decision-rule files | A JSD covering the admission, the fork policy, the invariant 11 revision and the JS-2 re-scope (`SJL-D-7`), filed before 2026-11-30; JSC entries | A revision of `dependencies.md` and `public-api.md`; the `SharedLibraries` plan and status pair; reconciliation of `Assemblies.md`, `Roadmap.md` and `ModernizationDelivery.md` | Roadmap entries in Broiler.Unicode (a new `docs/roadmap.md`) and Broiler.RegEx |
| SJL-1 | Pin file for the vector archive; N13 revision | JSC entry if a plan names vectors | Vector corpus README | (none) |
| SJL-2 | ADR 0001 and ADR 0012 revisions; register rows `Applied`; rule register rows | JSD on `packages.md` and N37 | `publishing.md` and the shared-libraries conventions document | (none: `SJL-D-2`'s prefix passes Broiler.JSeal's gate) |
| SJL-3 | `docs/compositions.md` column and rows; `THIRD_PARTY_NOTICES.md` rows; the exclusion for code leaving J coverage | Area verdict JSD; JSC entries (support §2, release gate 8 and 12) | Area verdict; `public-api.md` dispositions; changelog | (none) |
| SJL-4 to SJL-6 | Notices if needed | Area verdict JSDs; JSC entries | Area verdicts; dispositions | (none) |
| SJL-7 | Notices; the Unicode pin record | Area verdict JSD superseding JSD-0031 §4(b)'s refusal, or confirming it | Area verdict; the host-normalization correction | Broiler.Unicode records of the API and pin changes |
| SJL-8 | Notices; tzdb and calendar pin records | Area verdict JSD amending JSD-0053 and JSD-0056 where they move | Area verdict; the `tzdata.bin` retirement | (none) |
| SJL-9 | Notices; CLDR pin record | Area verdict JSD revisiting JSD-0027 §4 | Area verdict | (none) |
| SJL-10 | (none) | Area verdict JSD | Area verdict | (none) |
| SJL-11 | (none) | Area verdict JSD; the API-baseline change | Area verdict; `Component.md` §3 closed | Broiler.RegEx records |
| SJL-12 | (none) | JSD on the frontend contract (cites JSD-0014 and JSD-0036) | MOD-M2-3, MOD-M2-4 and MOD-M2-5 closed or cross-referenced | (none) |
| SJL-13 | (none) | Registry revision; JSC entries; JS-2 closed under its re-scoped gate | (none) | (none) |
| SJL-14 | (none) | (none) | `public-api.md` for `Ast` and `Parser` (`SJL-D-12`) | (none) |
| SJL-15 | (none) | `SJL-D-13`'s record; JSC entries | Modernization invariant 3 closed | (none) |
| SJL-16 | ADR 0014 revision describing the end state; register rows' final status | Final JSC entries | Final plan and status state | Final roadmap state |
| SJL-17 | (none) | Area verdict JSD | Area verdict; the host-collation correction | (none) |

---

## 12. Risks and stop conditions

The register is [risks.md](risks.md). The stop conditions that end a milestone, or the programme,
are these:

- **An unrecorded verdict.** An area moved without its extraction-gate record, or a refused area
  with no pointer in its duplicated code.
- **A cut-over that keeps a copy.** Anything on a deletion list survives the switch.
- **A verdict worse than the predeclared rule allows**, in either engine, with no revised rule
  written before the runs that show it.
- **A deployment boundary breached.** The frontend in a compiler-free root, optional data in a root
  without its surface, a platform globalization or regular-expression API in a shared library.
- **A shared library that references an engine or Broiler.VM.**
- **A local patch of a shared library inside an engine.**
- **A figure in a plan, ledger, decision or support document.**
- **Code written under a blocked SJL-0.**

---

## 13. The ledger, and how this roadmap is corrected

[The programme ledger](status.md) holds one row per milestone, in delivery order, and one row per
decision point, with its own update rules. They restate Broiler.VM's core ledger rules for this
programme, and add one: **a milestone's row names which repository's clauses are unmet.**

**How this roadmap is corrected.** A sentence made false by a landed milestone is corrected in
place, with the superseded text quoted and dated. A gate clause that changes is a gate revision. The
ledger records it, and existing evidence is re-evaluated rather than carried forward. A milestone
file and this summary are corrected in the same change.
