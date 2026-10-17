<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0076 - Time-zone and calendar data move to the data home: SJL-8's area verdict, sub-area by sub-area, decisions SJL-D-20 and SJL-D-21, and the tz release both engines adopt

**Status:** Decided 2026-10-17.
- **All three sub-areas are accepted.** Each is a responsibility both engines implement today:
  - the tz database: the pinned release, its compiler and table format, identifiers, primaries and regions;
  - the zone algorithms over it;
  - the calendar tables.
- **The data has one home: the Broiler.JS repository** (SJL-D-1 (b-iii)). It holds the pins, the generators and the
  table format.
  - **No package carries the tables** (SJL-D-6 (c)). Each engine embeds what the home's generator writes.
  - **The decoder and the zone algorithms go to `Broiler.ECMAScript.Algorithms`.**
- **Decisions SJL-D-20 and SJL-D-21 are recorded.** Both were taken on 2026-10-11.
- **The release both engines adopt is pinned by this dated record: tzdb 2026e.**
- **JSD-0053 and JSD-0056 are amended where they move.** Their generators leave this profile for the home. Their
  pins, their generated tables and rules N29 to N32 stay.
- Nothing in this profile changes with this record. The cut-over is SJL-8.5's own change, and this record is what
  it applies.

**Date:** 2026-10-17.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the data home's owner and the other holder of SJL-D-20 and SJL-D-21;
- the core architecture owner, for rule I1's allow-list, the composition register and rules N29 to N32.

**All three roles are held by one person** (EX-30), and this record does not claim that anyone independent has
read it.

**Milestone:** the programme's SJL-8, work package SJL-8.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts". **SJL-8 started before SJL-3 was
met**, at the owner's direction of 2026-10-17, which the programme ledger records: SJL-8 may not meet its gate
before SJL-3 meets SJL-3's. This record moves no gate.

---

## Context

The area's design is the programme's
[`areas/time-zones-and-calendars.md`](../../../../docs/javascript-shared-libraries/areas/time-zones-and-calendars.md).
It has no vectors yet. SJL-8.4 adds them after this record, and the design marks its readings *(reading)* until
they do.

So the correspondence is **from code**: it was re-derived by reading both engines at **Broiler.VM `fc42cb3`**,
whose product sources are `9508bd5`'s, and **Broiler.JS `a1a96ca2`**, whose sources are `6563786e`'s.
- **Every behavioural cell the rows rest on was also run**: through this family's conformance root and through
  Broiler.JS's shell, under UTC and under named host zones.
- Every member is named by file and line in
  [`correspondence.md`](../../../../docs/evidence/sjl-8-001/correspondence.md), which this record's change retains in
  bundle `sjl-8-001`, with every probe and both engines' answers beside it. Each difference is numbered: Z for the
  data, A for the algorithms, K for the calendars.
- SJL-8.4's vectors confirm or refute each row, and **a difference they find that this record does not classify
  falsifies it**.

**What the re-derivation found:**
- **The engines' algorithms agree.** Every probed input inside the data both engines share gives the same answer:
  - offsets, including sub-minute ones;
  - gaps and overlaps of 30 minutes, an hour, 24 hours and 44 minutes 30 seconds, under all four disambiguations;
  - midnight gaps, and changes of the daylight flag alone;
  - the limits of the instant range;
  - unknown names, at eight entry points;
  - offset zones and `Etc/GMT` zones.

  **Every difference the rows find comes from the data, from three of Broiler.JS's input checks, or from the host
  zone.**
- **The data differs as the design says, and more.**
  - Broiler.JS's tzdb 2025b differs from this family's 2026e in 20 names after 1970.
  - Its backzone differs in 110 names before 1970.
  - Its 2037 freeze holds southern zones on daylight time.
  - Its blob cannot be reproduced on any other host (Z15).
- **The calendars agree inside the years the proposal publishes, but for one.**
  - Inside the ranges the Intl era and month code proposal's Table 1 bases on published data, the engines differ
    in one year only, `chinese` 2057 (K2). There Broiler.JS reads .NET's table and the Hong Kong Observatory agrees
    with ICU4X.
  - Elsewhere the differences are years Table 1 leaves to an implementation's approximation (K4 to K6, K11), or
    Broiler.JS defects at its sources' bounds (K7, K12).
- **This family has no correction of its own.**
- **Two of the milestone's listed Broiler.JS corrections are to code no input reaches** (A6, A11): the zero offset
  for an unknown name, and the local-midnight fallback of start of day. They are deleted with that code.

Section "The design's readings" lists what does not hold, and X6 extends the deletion lists.

## The gate, sub-area by sub-area

### The tz database

**X1 - met.**
- **This family** compiles tzdb 2026e from a pinned, hashed archive with its own compiler:
  - `TzdbCompiler` and `TzdbTableGenerator` in the architecture tests;
  - rules N29 and N30;
  - `JsTzdbTables.g.cs` in the `.Intl` assembly.

  It decodes the tables in `JsTimeZones`.
- **Broiler.JS** embeds a build host's compiled zoneinfo as `tzdata.bin`, written by
  `scripts/tzdata/generate_tzdata.py`, and reads it with `TzifReader`, `IanaTimeZone` and `IanaTimeZoneDatabase`.

**X2 - met, with three corrections, six policy differences and two engine surfaces.**
- **Policy differences under SJL-D-20, every one Broiler.JS's:**
  - the release, Z1, Z2 and Z15;
  - backzone, Z3;
  - extrapolation after the last listed transition, Z4;
  - `Factory`, Z5;
  - CLDR primaries, Z7.
- **Corrections of Broiler.JS:**
  - Z10: lookup admits only an ASCII-case-insensitive match, so `America/Loſ_Angeles` is refused;
  - Z11: an ISO date-time string is not a formatter's `timeZone`;
  - Z12: `getTimeZones` answers the region's zones from zone.tab, not a hand table of fourteen regions.
- **Agreement:** Z6 and Z9.
- **Implementation-defined:** Z13, the format, which becomes the home's; Z14, optionality, which stays each engine's.
- **Z8, the formatter's resolved `timeZone`, is not a data difference.**
  - This family resolves a link to its primary and skips test262's `canonical-tz` cases, as a proposal (JSD-0054).
    Broiler.JS keeps the name, as `canonical-tz` asks.
  - SJL-D-20 makes it follow "the specification edition both engines pin", and the two engines do not yet pin one
    edition for it.
  - The shared database answers both the name and its primary. Which one the formatter reports stays each engine's
    until that edition is settled. `Intl.DateTimeFormat` is SJL-9's.
- **Z16, the retained `timezones.js` program**, is covered by the rows above, but for the `longOffset` name's
  dropped seconds in Broiler.JS, which is zone display and SJL-9's.

**The home's inputs include CLDR's `bcp47/timezone.json`.** The primaries are CLDR's, so the generator reads that
file from CLDR 48.2.0, as this family's does. The home pins it beside the tz archive, as a copy of this family's
pinned member.

**X6 - extended.**
- **This family** moves `TzdbCompiler`, `TzdbTableGenerator` and the pin reader they use (`TzdbPin`) to the home,
  which starts from copies of `src/tests/tzdb/pins/`. It keeps:
  - its pin;
  - `JsTzdbTables.g.cs`, which the home's generator now writes;
  - rules N29 and N30, which regenerate with it.

  It deletes `JsTimeZones`' decoding, lookup and zone rules. They are replaced by the shared decoder.
- **Broiler.JS** deletes:
  - `TzifReader`, `IanaTimeZone` and `IanaTimeZoneDatabase`;
  - `tzdata.bin` with its `EmbeddedResource` item;
  - `generate_tzdata.py`;
  - `JSIntl`'s region table for `getTimeZones` (Z12).

### The zone algorithms

**X1 - met.**
- **This family's** live in `JsTemporal.Zones` over `JsTimeZones`, with `ResolveTimeZone`, `EtcGmtZones` and the
  offset parser in `JsRealm.DateTimeFormat`.
- **Broiler.JS's** live in `JSTemporalZonedDateTime` over `IanaTimeZone`, with the offset-zone normalisation in
  `JSIntl` and the formatting engine's zone helpers.

**X2 - met, with two corrections and one policy difference.**
- **Agreement:** A1 to A4, A6, A8, A9, A11, A12 and A14.
- **Policy difference under SJL-D-20 (extrapolation):** A5, A7 and A10, which follow from Z4.
- **Corrections of Broiler.JS:**
  - A13: an offset with non-ASCII digits is a `RangeError`, not a .NET `FormatException`;
  - A16: a host zone that is not an available primary is not returned unvalidated.
- **Implementation-defined:** A15, the host zone. It is each engine's policy under SJL-D-19, which SJL-8 does not
  change. Broiler.JS's local-time provider may read the shared data (milestone SJL-8.6), which ends `Date` and
  `Temporal` reading one zone from two datasets.
- **A2's sampling** differs without effect on either dataset. **The shared algorithm samples as this family does**,
  at the wall time and a day either side, because data with a shorter period would need it.
- **The two unreached Broiler.JS fallbacks (A6, A11)** are deleted with their code. The shared start of day
  **fails with a status when no instant exists** (milestone SJL-8.3), as this family's does today.

**X6 - extended.**
- **This family** deletes the algorithm bodies of `JsTemporal.Zones`, and the core of `ResolveTimeZone` with
  `EtcGmtZones` and its offset parser.
- **Broiler.JS** deletes:
  - `JSTemporalZonedDateTime`'s offset, candidate, disambiguation, start-of-day and transition helpers, with the
    zero offset and the local-midnight fallback;
  - `JSIntl`'s offset-zone normalisation;
  - the formatting engine's zone helpers, with their host-zone fallback for an unknown name.

### The calendar tables

**X1 - met.**
- **This family** generates its year tables from the pinned ICU4X crates, `icu_calendar` 2.3.0 and
  `calendrical_calculations` 0.2.4, with `CalendarTableGenerator` under rules N31 and N32 into
  `JsCalendarTables.g.cs`. It decodes them in `JsCalendarData`, and outside them ports ICU4X's approximation, the
  tabular Islamic year and the 33-year rule.
- **Broiler.JS** reads .NET's `ChineseLunisolarCalendar`, `KoreanLunisolarCalendar` and `PersianCalendar` in their
  ranges. Outside them it reads the CLDR package's `CldrChineseCalendar` and the 33-year rule, and it reads
  `CldrUmmAlQura` for Umm al-Qura.

**X2 - met, with three corrections and five policy differences.**
- **Agreement:** K1 (from different data), K3, K8 (the same data), K9, K10, K13 and K16.
- **Corrections of Broiler.JS:**
  - K2: `chinese` 2057, inside Table 1's published range;
  - K7: far dates that throw a .NET exception or give impossible months;
  - K12: repeated dates at .NET's Persian bounds.
- **Policy differences under SJL-D-21:**
  - K4: `dangi` after 2050, which Broiler.JS computes as Chinese at Beijing's meridian;
  - K5: `dangi` 918 to 1899;
  - K6: `chinese` and `dangi` outside the tables;
  - K11: Persian outside Table 1's range;
  - K14: answers that are the installed .NET runtime's.
- **No observable difference:** K15.
- **SJL-10's:** K17, the arithmetic over the calendars, which this record does not move.

**The sub-area is the years, not the arithmetic over them.** What is shared is each table-backed calendar's year,
read from the table inside its range and from ICU4X's approximation, the tabular Islamic year or the 33-year rule
outside it. A year here is its new year, its months and their lengths, and its leap month. This is SJL-D-21's
"Broiler.VM's ICU4X sources" in both directions. The calendars' arithmetic over those years, and the formula
calendars, are SJL-10's.

**X6 - extended.**
- **This family** moves `CalendarTableGenerator` and its pin reader (`CalendarPin`) to the home, which starts from
  copies of `src/tests/calendars/pins/`. It keeps:
  - its pin;
  - `JsCalendarTables.g.cs`, which the home's generator writes;
  - rules N31 and N32.

  It deletes `JsCalendarData`'s decoding and the ported year sources outside the tables. They are replaced by the
  shared decoder and year sources.
- **Broiler.JS** deletes:
  - `TemporalLunisolarCalendar` and `TemporalSolarHijriCalendar`;
  - the Umm al-Qura and .NET dispatch of `TemporalArithmeticCalendar` and `TemporalCalendarMath`;
  - its uses of `CldrChineseCalendar` and `CldrUmmAlQura`.

  The CLDR package keeps both types for its other consumers.
- **No Broiler.JS type the area empties is public** (SJL-D-25): every member above is `internal`.

### X3 - neutral contract, all three sub-areas

- **What the shared members take and answer.** They take identifiers as spans, numbers, instants as epoch seconds
  or nanoseconds, ISO dates and times as records, and the table bytes an engine hands in. They answer numbers,
  identifiers and statuses: no engine type, no coercion, no property access.
- **A failure is data:**
  - an unknown name is a `false` from the lookup;
  - a missing start of day is a status;
  - a rejected disambiguation is a status;
  - a year outside every source is a status.

  Each engine maps a status to its own error.
- **No member charges.** No zone or calendar code of either engine charges today. Each member's work is bounded by
  the data and by the instant range, not by the input's length, beyond an identifier's, which the lookup bounds. So
  **there is no class C2 in SJL-8's rules**: this family's charging units are unchanged, and an exhaustion verdict
  that moves on fuel is a regression.

### X4 - no edge between the engines, none into Broiler.VM

The library and the home's generator reference neither engine and nothing in this repository. Neither engine
references the other.

### X5 - boundaries hold

- **Pure computation.** Every shared member computes over its arguments and the bytes an engine hands in.
- **No host data.** The library reads no host zone, clock or culture, no `TimeZoneInfo` and no .NET calendar class.
- **No tables in a package.** No package carries the tables.
- **Optionality.** This family's data stays in the `.Intl` assembly, which only roots that hand in `IntlData` carry,
  and rule I3 holds that. Broiler.JS embeds the generated tables in `BuiltIns` until its satellite plan (A-2) makes
  them optional.
- **The compiler** does not call the area.

### X7 - owners accept

The JavaScript profile owner and the Broiler.JS maintainer accept all three sub-areas and record SJL-D-20 and
SJL-D-21. One person holds both roles (EX-30).

## Decisions SJL-D-20 and SJL-D-21, and the release

**SJL-D-20 - the time-zone dataset policy, recorded as taken on 2026-10-11.**
- **This family's policy:**
  - the default build without backzone;
  - recurring rules extrapolated without limit;
  - CLDR primaries, with `Etc/UTC`, `Etc/GMT` and `GMT` as `UTC`;
  - no `Factory`;
  - ASCII case-insensitive lookup that refuses a character above U+007F.
- **The release: the newest both engines can adopt together, which this record pins.**
- **The formatter's `canonical-tz` behaviour** follows the specification edition both engines pin (Z8 above).

**The release is tzdb 2026e.**
- It is the release this family's pin records, by length and digests, in `src/tests/tzdb/pins/tzdb.pin`. The home
  copies that pin.
- On 2026-10-17 it is IANA's newest: the head of `https://data.iana.org/time-zones/tzdb/NEWS`, read that day, is
  "Release 2026e - 2026-09-29".
- This family adopts it by keeping its pin.
- Broiler.JS adopts it by embedding what the home's generator writes from that pin. Its own release, 2025b, is
  copied from a build host and cannot be regenerated (Z15), so nothing ties it to an older release.

A later release moves the pin in both engines together, by a dated record. Neither engine moves alone (the
milestone's Exit gate clause 13 and rule `I4`).

**SJL-D-21 - the calendar sources, recorded as taken on 2026-10-11:**
- the pinned ICU4X sources, `icu_calendar` 2.3.0 and `calendrical_calculations` 0.2.4, which the Temporal
  reference datasets this family retains already check;
- **no .NET calendar class**, since those vary with the runtime (K14).

Inside Table 1's published ranges, this corrects Broiler.JS in one year (K2). Outside them, every difference is a
policy change under this decision, listed in K4 to K6 and K11.

## The design's readings

Each is corrected in `areas/time-zones-and-calendars.md` beside the reading, which is quoted there, and in the
milestone where it repeats one.

**Time zones.**
- **"Checked against `zic` output across its validated range"** was one run of a prototype (JSD-0053 section 2),
  which no test repeats. The correspondence repeats it with the host's `zic` over the pinned files, 1800 to 2037:
  593 of 597 names agree, and the four others read their CLDR primaries by design.
- **The 2037 freeze** holds southern zones on **daylight** time, as in Santiago and Sydney (Z4). In Broiler.JS it
  also splits `Date`, which reads the host's data, from `Temporal`, which reads the blob (A15).
- **Backzone, "mostly included"**: 102 of backzone's 125 Zones. 110 names differ before 1970 (Z3).
- **Identifiers**: only `Factory` separates the sets.
  - Twelve names differ in their primary: 445 primaries listed in this family, 435 in Broiler.JS.
  - IANA's own default build compiles the `factory` file (its `Makefile`'s `NDATA= factory`). This family's pin
    README and compiler describe the default build as nine files without it.
- **Broiler.JS's algorithms**: "an unknown named zone silently reads offset zero" and "`StartOfDay` falls back to
  local midnight" are true of the code and reached by no input (A6, A11). The two- and three-sample methods differ
  without effect (A2). A zone's TZif is parsed lazily, and only the inflate is whole.
- **Missing from the table:** the region lists (Z12), ISO strings as the formatter's zone (Z11), non-ASCII offset
  digits (A13), an unvalidated host zone (A16), and `Date` and `Temporal` on two datasets (A15).

**Calendars.**
- **"Chinese, Korean: ICU4X's published years"**: the table's years are 1900 to 2102, which ICU4X's files call
  computed. Table 1's published ranges are 1900 to 2100 for `chinese` and 1900 to 2050 for `dangi`, so this family's
  `dangi` 2051 to 2102 are ICU4X's computed Korean years.
- **Broiler.JS's Chinese and Korean calendars** have these ranges: `chinese` 1901-02-19 to 2101-01-28 and `dangi`
  0918-02-19 to 2051-02-10. Outside them, `dangi` falls back to the **Chinese** computation (K4).
  - Its `dangi` 918 to 1899 is .NET's Korean table (K5).
  - Its `chinese` 2057 differs inside the published range (K2).
  - Far out it throws or gives impossible months (K7).
  - The comments claiming the CLDR computation matches .NET over 1901 to 2100 are false: they differ in 1906,
    2033 and 2057.
- **Persian, "equivalence not established"**: now established.
  - The engines agree over Table 1's 1206 to 1498 AP, and over 1178 to 1534.
  - They differ in 2,582 of years 1 to 9378 (K11), and twice at .NET's bounds (K12).
- **Umm al-Qura**: the two tables are the same data, all 301 years (K8).
- **"Pinned | Not pinned"**: confirmed. Broiler.JS's calendar answers do not vary with the host, but they are the
  installed runtime's (K14).

## Decision

1. **All three sub-areas move.**
   - **The data and its generators go to the home**, the Broiler.JS repository (SJL-D-1 (b-iii)):
     - the tz archive's pin, and CLDR's `bcp47/timezone.json`;
     - the ICU4X crates' pin;
     - the tz compiler, the table generators and the calendar table generator;
     - byte-for-byte regeneration (rule SL7);
     - the public version constants (`TzdbRelease` and the calendar sources).
   - **The decoder, the zone algorithms and the calendar year sources go to `Broiler.ECMAScript.Algorithms`.**
     They sit under the namespace SJL-6 opened, `Broiler.ECMAScript.Time`, in the design's section 3.2 shape. That
     shape is narrowed by the statuses of X3, and extended by the year sources outside the tables (above).
   - SJL-8.2 settles the names within these constraints.
2. **SJL-D-20 is recorded and the release pinned** as section "Decisions SJL-D-20 and SJL-D-21, and the release"
   states: tzdb 2026e. **SJL-D-21 is recorded**: ICU4X's pinned sources, and no .NET calendar class.
3. **No package carries the tables** (SJL-D-6 (c)). Each engine embeds what the home's generator writes:
   - this family in the `.Intl` assembly, as today;
   - Broiler.JS in `BuiltIns`, as today, until its satellite plan makes them optional.
4. **The engines keep:**
   - optionality, and the mapping of statuses to errors;
   - the host zone (SJL-D-19);
   - the formatter's resolved `timeZone` (Z8, until the edition is settled) and zone display names (SJL-9);
   - the arithmetic over calendars (SJL-10);
   - charging. Nothing in the area charges today, and no shared member adds a charge. There is no class C2.
5. **Corrections and policy changes, all Broiler.JS's:**
   - **The corrections** are fixes under decision SJL-D-16 (a): Z10, Z11, Z12, A13, A16, K2, K7 and K12, and the
     deletion of the two unreached fallbacks (A6, A11).
   - **The policy changes** are Broiler.JS's under SJL-D-20 and SJL-D-21:
     - the release (Z1, Z2, Z15);
     - backzone (Z3);
     - extrapolation (Z4, A5, A7, A10);
     - `Factory` (Z5);
     - primaries (Z7);
     - the calendar sources (K4, K5, K6, K11, K14).
   - Broiler.JS lists each in its changelog at SJL-8.6. **This family has none.**
6. **JSD-0053 and JSD-0056 are amended** where they move:
   - `TzdbCompiler`, `TzdbTableGenerator`, `CalendarTableGenerator` and their pin readers move to the home, and the
     profile's rules N29 to N32 regenerate with the home's generator;
   - the pins, the generated tables and the rules stay;
   - N29 to N32 gain the register rows they lack (milestone SJL-8.5).

   Their index rows carry the amendment, and their texts are unchanged.
7. **The reference datasets** this family retains run in both engines at SJL-8.4: `src/tests/cldr/timezones/` and
   `src/tests/temporal/temporal.js`, with their declared divergences. **This family keeps them unchanged.**
8. **The bases** are bundle `sjl-8-001`'s in each repository, taken at the commit that carries each repository's
   rule. The rules are committed with this record.

## What this rejects, and why

- **A package of the tables**: SJL-D-6 (c) has each engine embed them.
- **Backzone, `Factory` and a frozen offset after 2037**: SJL-D-20.
- **.NET's calendar classes and the CLDR package's astronomy**: SJL-D-21, and K2, K7 and K12.
- **Sharing zone display names, the formatter's identifier resolution or the calendars' arithmetic**: SJL-9 and
  SJL-10 own them.
- **A host zone for this family**, or a fixed one for Broiler.JS: SJL-D-19 keeps each engine's policy.

## Settling condition

This record is settled when SJL-8.5's cut-over lands with the deletion list absent from this family, and every rule
of bundle `sjl-8-001` has been judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record: the corrections it names are applied in the cut-overs.
`areas/time-zones-and-calendars.md`'s readings are corrected beside each, quoting it, and so are the milestone's
where they repeat one.

## What this record does not decide

- **Either cut-over.** SJL-8.5 and SJL-8.6 apply this record, and each repository's rule judges its own.
- **The vectors.** SJL-8.4 adds them, and they may falsify this record (below).
- **The edition that settles Z8**, and zone display names (SJL-9).
- **The calendars' arithmetic and the formula calendars** (SJL-10).
- **This family's UTC host zone**, which JSD-0053 section 7 states.
- **A speed claim of any kind.** SJL-8 measures nothing.

## Falsified if

- SJL-8.4's vectors find a difference between the engines, or between an engine and the specification, that the
  correspondence does not classify.
- The home's generator, run from this family's pins, does not write `JsTzdbTables.g.cs` and `JsCalendarTables.g.cs`
  byte for byte, under N30 and N32.
- A shared zone algorithm cannot answer every row the correspondence calls agreement as both engines answer it
  today.
