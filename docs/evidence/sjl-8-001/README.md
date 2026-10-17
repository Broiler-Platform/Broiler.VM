<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-8-001 - the time-zone and calendar area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-8 of the shared JavaScript libraries programme,
[`sjl-8-time-zones-and-calendars.md`](../../javascript-shared-libraries/milestones/sjl-8-time-zones-and-calendars.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-8.1 before any code of the milestone
existed.

**Records:** [JSD-0076](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0076-time-zone-and-calendar-data-move-to-the-data-home.md),
whose input is [`correspondence.md`](correspondence.md), with every probe and both engines' answers in
[`correspondence-runs/`](correspondence-runs/).

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules and correspondence only, 2026-10-17, at SJL-8.1. **Nothing here is a verdict, and nothing has
been taken.**
- The base of all three parts is taken at the commit that carries the rule, before any code of SJL-8 lands.
- The after runs follow SJL-8.5's cut-over.
- A collector joins this bundle when there is a run to take.

The data home's clauses and Broiler.JS's are judged in Broiler.JS's bundle of the same name. This bundle reads
none of its runs.

## 1. SJL-8's area rule and its correspondence (SJL-8.1)

**The correspondence.**
- It was read from code in both engines at the revisions it names. It numbers every difference the area verdict
  classifies: Z for the tz database, A for the zone algorithms and K for the calendar tables.
- Every behavioural cell its rows rest on was run in both engines: this family through the conformance root, and
  Broiler.JS through its shell.
- **The engines' zone algorithms agree on every input inside the data both share.** Every difference is one of
  these:
  - the data's policy under SJL-D-20 or SJL-D-21: the release, backzone, extrapolation after 2037, `Factory`, the
    primaries and the calendar sources;
  - one of eight Broiler.JS corrections: lookup by invariant upper-casing, ISO strings as a formatter's zone, the
    region lists, non-ASCII offset digits, an unvalidated host zone, `chinese` 2057, the far lunisolar dates and the
    Persian seams at .NET's bounds;
  - the host zone, which SJL-D-19 keeps each engine's.
- **This family has no correction of its own in the area.**

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It admits no
  class C2, C3 or C5: this family's charging, release, policy and calendar sources do not change. A correction the
  run finds is admitted only as a finding the correspondence missed.
- **Part B** holds this family's reference datasets unchanged:
  - `timezones.js` and the Temporal programs, run by the slice compiler's checks at both commits;
  - their programs, answers and divergences, byte for byte.
- **Part C** holds the wide twins to their base figures, with no class. The command-line root reaches none of the
  area's code, so these twins show only that no charge moved outside it.

**The release both engines adopt** is tzdb 2026e, the one this family's pin records. JSD-0076 pins it by its dated
record, from IANA's `NEWS` read on 2026-10-17.
