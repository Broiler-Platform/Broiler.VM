<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0063 - Shared libraries: the admission, invariant 11, and the seed copy withdrawn

**Status:** Decided 2026-10-11. The owner took decisions SJL-D-7, SJL-D-9 and SJL-D-23 of
[the shared JavaScript libraries programme](../../../../docs/javascript-shared-libraries/decisions.md)
on that date, and this record files this profile's half of them, as the programme's work package
SJL-0.4 requires. **Nothing is implemented by it**: no package is admitted, no line is copied or moved,
and no rule is minted.

**Date:** 2026-10-11, before 2026-11-30, the date on which
[JSD-0005](0005-the-seed-waited-on-set-and-snapshot-stop-condition.md)'s stop condition would have
taken the seed snapshot.

**Owner:** the profile architecture owner, who is also the JavaScript profile owner. **Co-signer:** the
core architecture owner, for the fork policy, and the Broiler.JS maintainer, for the part of it that
binds that component. **Every one of these roles is held by one person** (EX-30), and this record does
not claim the co-signature is independent.

**Milestone:** JS-2, whose gate it re-scopes. Recorded at the programme's milestone SJL-0.

---

## Context

This profile's plan was written for a **snapshot copy** of the Broiler.JS engine: a fork with its own
history and no dependency edge in either direction, from which this checkout would take its general
front end, its property storage and its registration generator. Milestone JS-2 takes that snapshot.
It is `Blocked`, and the ledger says no snapshot has been taken. JS-2's settling route is written in
the delivery plan: either the core contract is accepted, "or a record says the seed copy will not be
taken and JS-2's gate is re-scoped". JSD-0005 gave the snapshot a stop condition: taken as-is on
2026-11-30, or after 400 further seed commits, whichever comes first.

Both of this profile's front ends were since written in this checkout. The slice front end was
written here by [JSD-0014](0014-the-source-front-end-and-the-verification-boundary.md), and so was the
wide one. The ledger says "The general front end is still the ingest's and still waits".

The shared JavaScript libraries programme gives the language algorithms, data and source front end
both engines implement one maintained implementation each, in libraries that name neither engine,
consumed by both. Core record [ADR 0014](../../../../docs/adr/0014-shared-javascript-libraries.md)
records its admission policy, and the scope reading of ADR 0011 that the admission rests on. **A
programme that shares the front end makes the copy pointless**: the profile would share code with a
fork of itself, and every shared area would have a third copy until the fork's copy was deleted.

## Decision

### 1. Admission: the family may depend on admitted shared libraries

This profile family - `Broiler.VM.Profile.JavaScript`, `.Compiler`, `.Format` and `.Intl` - **may
depend on the shared libraries ADR 0014 admits**, each only from the projects its allow-list names.
That is the only kind of dependency outside this component the family may take. Until the programme's
milestone SJL-2 mints rule `I1` and revises rules A2, N1 and N37, the family takes none: those rules
refuse every shared package today, and this record does not weaken them.

### 2. Invariant 11 is revised

Invariant 11 read: "**The fork is one-way and mechanical.** No project reference, package reference,
or shared source item runs between this component and any legacy Broiler component in either
direction, and an architecture rule with a passing witness enforces it. Fixes do not flow across the
fork after the snapshot, and neither side is the other's upstream."

It becomes the fork policy the programme took as decision SJL-D-9, as it binds this profile:

- **No edge reaches an engine.** No project reference, package reference, or shared source item runs
  between this component and the Broiler.JS engine, or any other legacy Broiler engine component, in
  either direction. Architecture rules with passing witnesses enforce it.
- **The shared libraries are upstream of both engines.** This family's only dependencies outside the
  component are the shared libraries ADR 0014 admits, each named on rule `I1`'s allow-list and pinned
  by content hash. **Fixes to a shared area flow through the shared library**, in both directions.
- **Neither engine is the other's upstream.** Engine code does not flow between the engines. A defect
  in this profile's own code is this profile's to fix.
- **No local patches.** A defect in a shared library is never patched here. This profile may pin back
  to the previous release; a temporary adapter workaround needs a dated correction entry naming the
  upstream defect and the release that removes it.

### 3. Invariant 12 is not touched

**"No evidence transfers" stands exactly as written.** No conformance result, benchmark, measurement,
review decision or Native AOT sample produced by the Broiler.JS repository, by a shared library's home
or by any other component is this profile's evidence. Every gate here is met by evidence this
repository collects, running this profile against the shared package version this repository pins.
Agreement between the two engines is a cross-check and never an oracle: shared code means shared
bugs, so the specification and Test262 stay the oracles.

### 4. The seed copy rows are withdrawn

Roadmap section 4.3's copy table withdraws these rows:

| Row | What replaces it |
|---|---|
| Tokenizer and parser | The shared frontend, `Broiler.ECMAScript.Frontend` (programme milestones SJL-12 and SJL-13) |
| Syntax tree and visitors | The shared frontend's canonical tree (SJL-12, SJL-13) |
| Backend-neutral static analysis | The shared frontend's static semantics (SJL-15) |
| Property storage | Nothing is copied. The object model stays this profile's own, as it already is in this checkout; the programme shares no object model (decision SJL-D-18) |
| The built-in registration source generator | Nothing is copied. Registration stays this profile's own; the programme shares no built-in object implementation |
| "Acquire as this checkout's own dependencies": the regular-expression engine, the Unicode property tables and the locale data | **Replaced by admitted packages**: `Broiler.Regex` (SJL-11), the Unicode owner's packages (SJL-7), and the locale and time data generated from the shared generators and pins into this family's own data assembly (SJL-8, SJL-9, decision SJL-D-6 (c)) |

The rows the table already marked **Rewrite**, **Exclude**, **Delete** or **Amputate** need no
withdrawal: nothing was ever going to be copied from them. The rows on the test corpus and on the
conformance harness are withdrawn with the copy: nothing is copied or ported from the seed, and the
harness method stays this profile's own.

### 5. Decision SJL-D-7, taken as (a): the seed copy is not taken, and JS-2 is re-scoped

**The seed copy will not be taken.** **JSD-0005's stop condition is withdrawn** with it: no snapshot
is taken on 2026-11-30, and none after any number of seed commits. JSD-0005's candidate snapshot
identity stays in the record as a candidate nobody took.

**JS-2's gate is re-scoped** to: **the general front end is the shared frontend (SJL-13).** It closes
when the programme's milestone SJL-13 meets its Broiler.VM clauses: this profile's wide parser is
gone, the wide path parses with the shared frontend, the artifacts are byte-identical over the parse
corpus or every difference is classified under the milestone's predeclared rule, and test262 in both
forms meets that rule. The rest of JS-2's former gate becomes a requirement on the shared frontend,
held by the programme's rules rather than by a copy:

- no ambient parse state: the goal and top-level-await permission are explicit options, and two
  parses with different goals run concurrently;
- no thread-static field and no async-local type;
- no conditional compilation, IL emission, assembly loading, name-based type resolution, activator
  construction, run-time generic construction or module initializer;
- trim and AOT analyzers clean across its reference closure;
- a deep-nesting corpus refused rather than terminating the process;
- one early-error stage, which JSD-0014's rejection of the seed's four-place split becomes.

**This settles JS-2's open blocker by the route JS-2 already names**: a record says the seed copy will
not be taken, and JS-2's gate is re-scoped. The core contract's acceptance no longer blocks JS-2,
because nothing is adapted from a copy to a moving contract. JS-2 now waits on the shared frontend,
which is built in the Broiler.JS repository, and the ledger records that as its external dependency.

### 6. JSD-0005's other rulings

- **The waited-on set** has no subject: nothing is copied, so nothing waits for the seed's in-flight
  work.
- **The satellite dependency** stays opened, with the profile built-ins owner as its holder. It is
  fulfilled by admitted component packages and by the shared generators, in the milestones section 4
  names, and not by acquisition into this checkout's own tree.
- **Nullable reference types** stay enabled. **The unsafe position has no subject**: no seed front end
  arrives, so no project of this family is given `AllowUnsafeBlocks` for one.

### 7. Decision SJL-D-23: the programme continues past the pilot

**The programme continues past its pilot, milestone SJL-3, a choice taken before the pilot ran.** For
this profile that means the shared libraries are this profile's plan for every area the programme
lists, not an option conditional on the pilot. Each area still meets its own gate, and an area the
programme's extraction gate refuses is refused alone.

## What this rejects, and why

- **Keeping the copy** (SJL-D-7 (b)). The snapshot would have been taken on 2026-11-30 by default, and
  this profile would then have shared code with a fork of itself: every shared area would carry a
  third copy until the fork's copy was deleted. Both front ends were written here anyway, so the copy
  no longer buys what JS-2 was written for.
- **Deferring** (SJL-D-7 (c)): revising JSD-0005's stop condition to a later date or a programme
  milestone. It was the fallback, admissible only if this record could not be filed before 2026-11-30.
  It is filed, so the fallback is not taken.
- **Keeping invariant 11 as written.** Once a shared library exists, the text forbids fixing it: a fix
  to a shared area would be a fix flowing across the fork.
- **A weaker fork policy that lets this profile patch a shared library locally.** It recreates the
  duplication the programme exists to remove.
- **Touching invariant 12.** Evidence collected elsewhere is not this profile's evidence, whoever wrote
  the code it ran.

## Settling condition

The admission and the fork policy are settled in the change that mints rule `I1` and revises rules A2,
N1 and N37 (the programme's SJL-2), each with its witnesses. JS-2's re-scoped gate is settled when the
programme's SJL-13 meets its Broiler.VM clauses, and JS-2 closes then with its correction entries.
**This record is reopened only by a new dated record**, for example if the core-contract owner rules
that ADR 0011's refusals reach the shared frontend (route MVP-16 of `docs/mvp.md`). That ruling would
leave JS-2 with no route to closure except a new decision, and this record would say so beside it.

## Correction entries

[JSC-291](../roadmap.corrections.md#jsc-291) (invariant 11), [JSC-292](../roadmap.corrections.md#jsc-292)
(sections 4 and 5, the edge clause), [JSC-293](../roadmap.corrections.md#jsc-293) (section 4.3, the copy
rows), [JSC-294](../roadmap.corrections.md#jsc-294) (gates section 21's dependency row, gate 2 and the
stop row) and [JSC-295](../roadmap.corrections.md#jsc-295) (JS-2's gate text).

## What this record does not decide

- **The area verdicts.** Each area's accept or refuse verdict is a JSD of its own, filed by the
  programme milestone that applies it: the pilot's at SJL-3.1, Unicode's at SJL-7.1, regular
  expressions' at SJL-11.1, and so on.
- **Anything about the core.** ADR 0014 is the core's record; this record names it and does not
  restate it.
- **Any evidence.** No bundle is cited as meeting a clause, and none is collected by this record.

## Falsified if

- a package reference outside ADR 0014's allow-list appears in this family's graph;
- a fix to a shared area lands in this profile's copy of an algorithm after that area's cut-over;
- a gate here cites a result collected in another repository;
- a snapshot of the Broiler.JS engine is taken into this checkout.
