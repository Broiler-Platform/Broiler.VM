<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-10-001 - the Temporal core: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-10 of the shared JavaScript libraries programme,
[`sjl-10-temporal.md`](../../javascript-shared-libraries/milestones/sjl-10-temporal.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-10.1 before any code of the milestone
existed.

**Records:** [JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md), with every probe and each engine's answers in
[`correspondence-runs/`](correspondence-runs/): `core/` for records and limits, balancing, durations and
differences, and `syntax/` for the grammar, the calendars and serialization.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules and correspondence only, 2026-10-18, at SJL-10.1. **Nothing here is a verdict, and nothing has been
taken.**
- The base of all three parts is taken at the commit that carries the rule, before any code of SJL-10 lands.
- The after runs follow SJL-10.4's cut-over.
- A collector joins this bundle when there is a run to take.

Broiler.JS's clauses and the home's are judged in Broiler.JS's bundle of the same name. This bundle reads none of
its runs.

## 1. SJL-10's area rule and its correspondence (SJL-10.1)

**The correspondence.**
- It was read from code in both engines at the revisions it names, in two halves on 2026-10-18, merged the same day.
  It numbers every difference the area verdict classifies: R for records and limits, B for balancing, D for
  durations, F for differences, G for the grammar, C for the calendars and S for serialization.
- Every behavioural cell its rows rest on was run in both engines: this family through the conformance root, and
  Broiler.JS through its shell. The probes of records, balancing, durations and differences also ran under the
  proposal's polyfill at `e8cc03fc`.
- **Every difference is classified.** Each is one of these:
  - a Broiler.JS correction, the milestone's listed ones and those found beyond them, each by row;
  - the one place found where the pinned text and its polyfill disagree, where the text rules (row D8, decision D7);
  - code no input reaches, deleted with its file.
- **This family has one correction**, row R6: three constructors that end the invocation on a year above 2^53. It is
  made at SJL-10.4 (decision D9).

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0078 records decisions D1 to D9 under it. It changes no programme rule: nothing is accepted without
review, and SJL-10 may not meet its gate before SJL-6 and SJL-8 meet theirs.

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It admits no
  class C2, C3 or C5: this family's charging, its proposal revision, its test262 pin and its data do not change. Its
  only named correction is R6, and any other it finds is admitted only as a finding the correspondence missed. A
  wall-clock race is admitted under C4's (c') and (e).
- **Part B** holds this family's Temporal reference datasets under `src/tests/temporal/` unchanged:
  - the five programs, run by the slice compiler's checks at both commits;
  - their programs, answers and divergences, byte for byte.
- **Part C** holds the wide twins to their base figures, with no class. The command-line root reaches none of the
  area's code, so these twins show only that no charge moved outside it.

**The proposal revision both engines follow** is `tc39/proposal-temporal@e8cc03fc`, the one this family has followed
since JSD-0053, and the area's rules run test262 `ccaac100` (decisions D1 and D2).
