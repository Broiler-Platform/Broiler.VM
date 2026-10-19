<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0078 - The Temporal core moves to the shared algorithms library: SJL-10's area verdict, sub-area by sub-area, the proposal revision both engines follow, and the owner's decisions of 2026-10-18

**Status:** Decided 2026-10-18.
- **All seven sub-areas are accepted.** Each is a responsibility both engines implement today:
  - records and limits;
  - balancing, regulation and addition;
  - durations and rounding;
  - differences;
  - the grammar;
  - calendars: the formula calendars, the arithmetic over the table calendars' years, month codes, eras and
    reference dates;
  - serialization.

  The options' rules go with the sub-area each rule serves (section "Context").
- **Each moves to `Broiler.ECMAScript.Algorithms`.** The engines keep their Temporal objects, the reads in
  specification order, coercion, `relativeTo` resolution, their errors, and `Temporal.Now`'s clock and system zone.
- **Both engines follow `tc39/proposal-temporal@e8cc03fc`** (decision D1). This family has followed it since JSD-0053,
  and Broiler.JS adopts it. The area's rules run test262 `ccaac100` (decision D2).
- **The owner's decisions D1 to D9 of 2026-10-18 are recorded**, taken under the owner's direction of that day.
- **This family has one correction**, row R6: three constructors that end the invocation on a year past 2^53. It is
  made at SJL-10.4 (decision D9).
- **Broiler.JS's corrections are listed by row.** Its deletion list reaches its formula and table calendar copies
  (decision D3).
- Nothing in this profile changes with this record. The cut-overs are SJL-10.4's and SJL-10.5's own changes, and this
  record is what they apply.

**Date:** 2026-10-18.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the shared library's owner and the other engine's;
- the core architecture owner, for rule I1's allow-list and the composition register.

**All three roles are held by one person** (EX-30), and this record does not claim that anyone independent has
read it.

**Milestone:** the programme's SJL-10, work package SJL-10.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts", with the proposal revision that
engine now follows. **SJL-10 started before SJL-6 and SJL-8, which it waits on, met their gates**, at the owner's
direction of 2026-10-18, which the programme ledger records. SJL-10 may not meet its gate before SJL-6 and SJL-8 meet
theirs, and so not before SJL-3 meets SJL-3's. This record moves no gate.

**The owner's direction.** On 2026-10-18, after the correspondence's second half reported its open points, the owner
wrote: "Please do all recommended decisions and skip waiting for review." This record reads it in two parts:
- each decision the correspondence leaves open is taken as the correspondence recommends, and recorded below as the
  owner's dated decision under that direction (decisions D1 to D9);
- the programme proceeds through SJL-10's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-10 is accepted by it.
- SJL-10 still may not meet its gate before SJL-6 and SJL-8 meet theirs, and so not before SJL-3 meets SJL-3's.

**Names.** The decisions are numbered D1 to D9, as the owner took them. The correspondence's duration rows are
numbered D1 to D12 too. In this record a decision is always "decision Dn", and a row always "row Dn".

---

## Context

The area's design is the programme's
[`areas/temporal.md`](../../../../docs/javascript-shared-libraries/areas/temporal.md). It has no vectors yet. SJL-10.2
adds them after this record, and the design marks its readings *(reading)* until they do.

So the correspondence is **from code**: it was re-derived by reading both engines at **Broiler.VM `14072b0`**, whose
product sources are `047dcec`'s, and **Broiler.JS `d3464413`**.
- **Every behavioural cell the rows rest on was also run**, through this family's conformance root and through
  Broiler.JS's shell.
- **The probes of the four core sub-areas also ran under the reference**, the proposal's polyfill at `e8cc03fc`. The
  probes of the grammar, the calendars and serialization did not; there the retained datasets, which the polyfill
  produced, are its reading, and both engines ran them.
- It was written in two halves on 2026-10-18 and merged into one
  [`correspondence.md`](../../../../docs/evidence/sjl-10-001/correspondence.md), which this record's change retains in
  bundle `sjl-10-001`, with every probe and each engine's answers beside it. Each difference is numbered: R for
  records and limits, B for balancing, D for durations, F for differences, G for the grammar, C for the calendars and
  S for serialization.
- SJL-10.2's vectors confirm or refute each row, and **a difference they find that this record does not classify
  falsifies it**.

**Two gaps are accepted as recorded:**
- the test262 harness files copied into the runs' `vm262/` directories are not retained. The pinned suite holds
  them, as at SJL-8;
- the polyfill's three runtime dependencies were installed from the npm registry at their current versions, which
  the correspondence names.

**What the re-derivation found:**
- **Every difference is classified**, and section "Every row, classified" puts each row in one class.
- **The milestone's seven listed Broiler.JS corrections are confirmed, two of them in part.**
  - Day rounding that ignores daylight saving holds for `until` and `since` only (F7). `round`, `hoursInDay` and
    rounding relative to a zoned date already use real day lengths (D9).
  - The reference years are computed without the table, but they answer as the table does on every month-day
    probed (C10).
- **Broiler.JS's corrections are wider than the milestone listed.** The rows find more grammar leniencies, a
  year-month at the lower limit, a `compare` that refuses what it need not, three ways of rounding to months relative
  to a date that disagree with each other, conversions in other calendars that lose a field, and era names and
  aliases of earlier drafts.
- **This family has one correction**: R6.
- **The pinned specification text and its polyfill disagree in one place found** (row D8). This family follows the
  text.
- **SJL-8 already published the records the area needs**: `IsoDate`, `IsoTime` and `IsoDateTime` in
  `Broiler.ECMAScript.Time`.

**The options' rules.** The design gives the options a scope row of their own ("the **rules**: allowed values,
defaults, unit validation, difference settings"), and the milestone's seven sub-areas do not name them. So this record
judges each rule with the sub-area that uses it, as the correspondence reads it:
- `overflow`, with balancing (B3);
- the units, their groups, the increments and the rounding modes, with durations and rounding (D5);
- the difference settings, with differences (F1);
- `calendarName`, `timeZoneName`, `offset`, `fractionalSecondDigits` and `smallestUnit`'s precision, with
  serialization (S2 to S5, S7, S11).

The reading of each option, with `Get` and coercion in specification order, stays each engine's.

Section "The design's readings" lists what does not hold, and X6 extends the deletion lists.

## The gate, sub-area by sub-area

### Records and limits

**X1 - met.**
- **This family**: `JsTemporalCore`'s records (`JsIsoDate`, `JsTimeRecord`, `JsIsoDateTime`, `JsDateDuration`,
  `JsInternalDuration`), units and limit checks, all engine-free; the constructors' conversions in the realm files.
- **Broiler.JS**: each `JSTemporal*` class's own `int` fields, limits and validity checks, and `TemporalNonIso`'s.

**X2 - met, with one correction of each engine.**
- **Same:** R1 to R5 and R8, at every limit, from every entry point.
  *(Note, 2026-10-19, at SJL-10.5. R4 is same but for a case Broiler.JS's cut-over corrects: the wall clock of the
  earliest instant, in a zone twelve hours behind UTC, given with its offset. Section "SJL-10.5's cut-over, read
  against this record" names it. The item above is left as written.)*
- **Correction of this family:** R6. `new PlainDate` with a year of 2^53 + 2 or ±10^20, and `new PlainDateTime` and
  `new PlainMonthDay`'s reference year with 10^20, end the invocation instead of throwing a `RangeError`. The limit
  check reaches the shared epoch-day arithmetic, whose precondition throws, before it tests the year. Decision D9
  corrects it at SJL-10.4.
- **Correction of Broiler.JS:** R7, the milestone's listed narrowing. Additions wrap at four sites and saturate at
  three.
- Broiler.JS's constructors narrow to `int`, which saturates on .NET 9 and later *(host)*, so they answer as the
  proposal does (R6). The shared records take years as `long`, as SJL-8's `IsoDate` does.

**X6 - extended.**
- **This family** moves `JsTemporalCore`'s records, units and limits (correspondence section 10). Its records become
  SJL-8's (decision D8).
- **Broiler.JS** deletes the validity and limit copies of every type, `DurationSign`, `RejectDuration` and the
  default-largest-unit copies (correspondence section 11, "Validity and limits").

### Balancing

**X1 - met.**
- **This family**: BalanceTime, AddTime, BalanceISODateTime and AddDaysToISODate in `JsTemporalCore`, over the shared
  epoch days; RegulateTime and RegulateISODate; the ISO CalendarDateAdd; AddInstant and AddZonedDateTime; and the
  rounding of points, RoundTime, RoundISODateTime and RoundTemporalInstant.
- **Broiler.JS**: the same per type. AddISODate is in four places, and time is carried into days, regulated and
  rounded in each type.

**X2 - met, with one correction.**
- **Same:** B1 to B4 and B6 to B9.
  *(Note, 2026-10-19, at SJL-10.5. B4 and B9 are same but for cases Broiler.JS's cut-over corrects: a bag's day 0
  and a month-day's `with` regulated against a leap year (B4), and a tie under `halfEven` and the earliest instant
  rounded in a zone twelve hours behind UTC (B9). Section "SJL-10.5's cut-over, read against this record" names
  them. The item above is left as written.)*
- **Correction of Broiler.JS:** B5. `PlainYearMonth` −271821-05 minus P1M is −271821-04, because it checks the
  year-month limits only. The specification adds to the month's first day, which is past the date limit, so it throws.
- `overflow`'s rule is the library's (B3); its read stays each engine's.

**X6 - extended.**
- **This family** moves the balancing, regulation, the ISO `DateAdd` body with its early refusals, and `AddInstant`
  (correspondence section 10).
- **Broiler.JS** deletes every type's regulation, addition and balancing copies, and the small copies beside them:
  `CompareISODate`, `FloorDiv` and the unit-length tables (correspondence section 11).

### Durations and rounding

**X1 - met for both engines, and an adoption in one part for Broiler.JS.**
- **This family**: validity, sign, default largest unit and the internal records in `JsTemporalCore`; the duration
  operations and one relative machinery in `JsTemporal.Durations`: DateDurationDays, ComputeNudgeWindow, the three
  nudges, BubbleRelativeDuration, RoundRelativeDuration and TotalRelativeDuration. It runs over the calendar's
  `DateAdd` and `DateUntil` and the zone's instants, so every calendar takes one path. Totals are exact fractions,
  rounded once.
- **Broiler.JS**: validity, sign and `compare` in `JSTemporalDuration`; a plain-date relative machinery in
  `JSTemporalDuration.Arithmetic`; a zoned one in `JSTemporalZonedDateTime.Rounding`. Totals are exact.
- **An adoption for Broiler.JS:** a `relativeTo` in a calendar outside ISO 8601 and the Gregorian family, which it
  refuses today with a plain `Error` (D11).

**X2 - met, with corrections of Broiler.JS and one policy decision.**
- **Same:** D1, D2, D5, D9 and D10, and D4 but for its range check.
  *(Note, 2026-10-19, at SJL-10.5. D10 is same but for cases Broiler.JS's cut-over corrects: totals whose window
  passes the limits, or whose `relativeTo` is the earliest instant's wall clock with its offset. Section "SJL-10.5's
  cut-over, read against this record" names them. The item above is left as written.)*
- **Corrections of Broiler.JS:**
  - D3, the milestone's listed `Duration.prototype.add`: a plain `Error` with a `relativeTo`, a `TypeError` for a
    second argument, and the observable read of a parameter the proposal no longer has;
  - D4: `compare` adds the whole duration to `relativeTo` and refuses a sum past the date-time limit;
  - D6 and D7: rounding to months re-derived through a clamped difference;
  - D11: a non-ISO `relativeTo`, as the design's table says.
- **Policy decision, decision D7:** D8. Where the pinned text and its polyfill disagree, the text rules. This family
  answers as the text does. Broiler.JS's answers, which differ by path, are a correction.
- **Code no input reaches:** D12, in both engines. The shared code asserts there, or fails with a status.
  *(Note, 2026-10-19, at SJL-10.5. A zone's skipped day reaches two of row D12's branches: its day window, which
  decision D11 governs, and its zoned branch, the day span whose sign NudgeToZonedTime asserts, which Broiler.JS's
  "inconsistent Instants" branch tested and decision D12 governs. Sections "SJL-10.3's library, read against this
  record" and "SJL-10.5's cut-over, read against this record" record them. The item above is left as written.)*
- The units, increments and rounding modes are the library's rules (D5).

**X6 - extended.**
- **This family** moves the duration operations and the relative machinery (correspondence section 10).
- **Broiler.JS** deletes:
  - its three separate relative-rounding implementations: the plain-date machinery, the zoned machinery, and the
    day-axis nudge its differences use;
  - `compare`'s arithmetic, and `AddSubtract` with its "not yet implemented" `Error`;
  - `JSTemporalRoundingOptions`' `NudgeTotal` and `NudgeRoundsToEnd`. Its unit order, normalisation, maximum increment
    and increment validation become the library's rules, and its reads stay as adapter code.

### Differences

**X1 - met.**
- **This family**: one GetDifferenceSettings, and each type's DifferenceTemporal… over DifferenceISODateTime,
  DifferencePlainDateTimeWithRounding, DifferenceZonedDateTime, DifferenceInstant and CalendarDateUntil. The rounding
  is RoundRelativeDuration for every calendar.
- **Broiler.JS**: each type's own settings; DifferenceISODate twice and `TemporalNonIso.Difference`; the ISO day-axis
  nudge `RoundDateDifference` for calendar units, and `RoundTimeDifference` for time units.

**X2 - met, with two corrections.**
- **Same:** F1, in effect, though six copies read it; F2, F3, F6 and F8; F5, but for row D8's case.
  *(Note, 2026-10-19, at SJL-10.5. F5 and F8 are same but for cases Broiler.JS's cut-over corrects: a rounded
  `PlainDateTime` difference whose days bubble into months, or which spans the full range (F5), and year-month
  differences at the limits (F8). Section "SJL-10.5's cut-over, read against this record" names them. The item above
  is left as written.)*
- **Corrections of Broiler.JS, both listed in the milestone:**
  - F4: rounding options ignored in non-ISO differences of `PlainDate`, `PlainYearMonth` and `ZonedDateTime`, and
    applied against ISO months for `PlainDateTime`;
  - F7: a calendar smallest unit in `ZonedDateTime` `until` and `since` rounded on 24-hour days.
- **Row D8's case in F5 and F7** is decision D7's: the text rules.
- The difference settings are the library's rules (F1). The reads stay each engine's.

**X6 - extended.**
- **This family** moves the differences (correspondence section 10).
- **Broiler.JS** deletes:
  - every type's difference copies;
  - the settings' rules of every type, each keeping only its reads, which become one adapter;
  - the dead `NotImplementedArithmetic` and `ReadLargestUnit`.

### The grammar

**X1 - met.**
- **This family**: `JsTemporalParser`, a recursive-descent parser of the proposal's grammar, one method per
  production, with a duration scanner that keeps every part exact.
- **Broiler.JS**: eight regular-expression parsers, one per type, and a ninth, lenient one in `TemporalIsoString` for
  calendar strings, time zone strings and `relativeTo`.

**X2 - met, with corrections of Broiler.JS.**
- **Same:** G1, G2, G5, G7, G10, G14, G15, G21, G23 and G24, and G8 for `PlainTime`, `ZonedDateTime` and `Instant`.
- **Corrections of Broiler.JS**, the milestone's "lenient grammar acceptances", wider than the design named: G3, G4,
  G6, G8 for the four plain types, G9, G11, G12, G13, G16, G17, G18, G19 (`Z` as an offset string), G20 and G22.
- **Code no input reaches:** Broiler.JS's handling of U+2212 after a match (G10).
- The design's "reads options part-way through the parse" (G24) and "exact (`BigInteger`) / `double.Parse`" (G23)
  describe the code, and no answer.

**X6 - extended.**
- **This family** moves `JsTemporalParser` whole, behind a `TryParse` that returns the record and a
  `TemporalParseError`. Its five `RangeError` sites become codes, and `ParseCalendarString`'s catch of `JsThrow`
  becomes a test of the result (correspondence section 10).
- **Broiler.JS** deletes:
  - the eight per-type parsers with their patterns, and the string fallback of `CanonicalizeTimeZone`;
  - `TemporalIsoString`. Its coercing members become adapter code, and the dead `RejectIncompatibleStyle` is removed.

### Calendars

**X1 - met.**
- **This family**: `JsCalendars`, every calendar but ISO 8601, engine-free, over SJL-8's shared years for the table
  calendars and with Table 6 as a table; `JsTemporal.Calendar`'s identifiers and ISO arithmetic; and
  `JsTemporal.CalendarNonIso`, the era proposal's section 4.1.
- **Broiler.JS**: `TemporalCalendar`'s identifiers and the Gregorian family's eras, in `JSTemporalCalendar.cs`; the
  formula calendars, `TemporalArithmeticCalendar`; the arithmetic over SJL-8's shared years, `TemporalTableCalendar`;
  the dispatch, `TemporalCalendarMath`; field resolution, reference dates and arithmetic, `TemporalNonIso`; and
  month-code copies in five classes. Its reference dates are a search where this family's are Table 6 and the 1900 to
  2035 rule.

**X2 - met, with corrections of Broiler.JS.**
- **Same:** C1, C7, C11, C14 and C18; C4 but for C5; C8 but for C9; C10 in every answer, the code differing.
  *(Note, 2026-10-19, at SJL-10.5. C7, C8 and C10 are same but for cases Broiler.JS's cut-over corrects: a year-month's
  `with` to a leap month the year lacks (C7); an era against a year in `with`, an era given as an object, and the eras
  a month-day reads (C8); and a month-day given a year, or a month without one (C10). Section "SJL-10.5's cut-over,
  read against this record" names them. The item above is left as written.)*
- **Corrections of Broiler.JS:**
  - C5, the milestone's "invalid month codes silently constrained", in eight calendars;
  - C2: the aliases `gregorian` and `minguo` (decision D5);
  - C6: a month code with digits other than ASCII;
  - C9: era names of earlier drafts, and `incarnation` (decision D4);
  - C12: an ISO month-day's year past 2^31, which loses its leap year when it saturates;
  - C13: the calendar a `PlainMonthDay` lends;
  - C15, C16 and C17: the three conversions that lose the calendar's day, calendar or year.
- **The milestone's "reference years computed without the proposal's table"** is confirmed of the code and refuted
  of the answers (C10). The table replaces the search, and no answer moves.
- **Code no input reaches:** `TemporalArithmeticCalendar`'s `islamic` entry (C3), and the reference-date search's
  upper year 2100 (C10).

**X6 - extended, by decision D3.**
- **This family** moves the core of `JsTemporal.Calendar`, `JsTemporal.CalendarNonIso` whole and `JsCalendars` whole
  (correspondence section 10). The reads stay: `PrepareFields`, `CalendarWithIsoDefault`, `ToCalendarIdentifier` and
  `CalendarOf`.
- **Broiler.JS** deletes `TemporalNonIso`'s calendar parts, whose coercions and property reads become adapter code.
  By decision D3 it also deletes its other calendar copies:
  - `TemporalArithmeticCalendar`;
  - `TemporalTableCalendar`'s arithmetic over the shared years. Its `Year`, which reads SJL-8's years, stays as
    adapter code until the library's calendars read them;
  - `TemporalCalendarMath`;
  - the Gregorian family's identifiers, years and era tables in `JSTemporalCalendar.cs`. Its `ToSlotValue` and
    `ResolveCalendarIdentifierArgument` stay as adapter code;
  - the five per-type month-code copies.

  JSD-0076's X6 kept `TemporalArithmeticCalendar` and `TemporalCalendarMath` "for the formula calendars". SJL-10
  takes the formula calendars, so they go.

### Serialization

**X1 - met.**
- **This family**: one set of string members in `JsTemporalCore` and `JsTemporal.Conversions`, `CalendarAnnotation`,
  and `JsTemporal.Options`' `Precision`.
- **Broiler.JS**: a formatter per type, each with its own padding and its own reading of `calendarName`.

**X2 - met.**
- **Same:** S1 to S11.
  *(Note, 2026-10-19, at SJL-10.5. S7 and S11 are same but for a case Broiler.JS's cut-over corrects:
  `ZonedDateTime.prototype.toString` converted an invalid `offset` option twice. Section "SJL-10.5's cut-over, read
  against this record" names it. The item above is left as written.)*
- **Code no input reaches:** Broiler.JS's offset getter drops a fraction of a second, which no zone has (S8).
- `calendarName`, `timeZoneName`, `offset` and `fractionalSecondDigits`, and the precision `smallestUnit` gives, are
  the library's rules (S2 to S5, S7, S11).

**X6 - extended.**
- **This family** moves the string forms of `JsTemporal.Conversions`, `JsTemporalCore`'s string members and
  `CalendarAnnotation` (correspondence section 10). `InstantString` and `ZonedDateTimeString` move as functions of an
  offset in nanoseconds and a date-time, and the zone lookup stays in the adapter.
- **Broiler.JS** deletes the per-type formatters and the precision arithmetic of the `toString` bodies, which keep only
  their reads. Its five `ReadCalendarName` copies become one adapter over the library's rule.

**No Broiler.JS type the area empties is public** (SJL-D-25). Every type above is `internal` or `private`. The public
`JSTemporal*` classes stay as shells.

### X3 - neutral contract, all seven sub-areas

- **What the shared members take and answer.** They take numbers, `BigInteger`s of nanoseconds, spans and
  enumerations; SJL-8's records and the area's duration records; and calendar and time-zone providers. They answer
  records, numbers, strings and statuses: no engine type, no coercion, no property access.
- **A failure is data, in each sub-area:**
  - **records and limits**: a limit check takes any year an engine hands it and answers a status. None throws
    (decision D9);
  - **balancing**: a time balanced past a day answers its overflow days beside its `IsoTime`, not in it (decision
    D8). A regulation under `reject` answers a status;
  - **durations and rounding**, and **differences**: an invalid duration, a calendar unit without `relativeTo`, and a
    result past a limit each answer a status. The calendar and zone operations come through provider interfaces,
    which answer statuses too;
  - **the grammar**: `TryParse` answers the record, or a `TemporalParseError`;
  - **calendars**: an unknown identifier, an invalid month code or era, and a date outside a calendar each answer a
    status;
  - **serialization**: a string. An option value outside the rule's allowed values is the rule's status, which the
    engine reads.

  Each engine maps a status to its own `RangeError` or `TypeError`, at the specification's moments.
- **Every limit is explicit**: the instant, date-time, date, year-month and duration limits are the library's.
- **Exactness.** Time durations are exact integers of nanoseconds. Totals are exact ratios, rounded once to a double
  through SJL-4's `ExactRounding`.
- **Metering.** No Temporal file of either engine charges today; only this family's generic per-call charge applies,
  and it stays the adapter's. Every member's work is bounded by the limits, except the grammar's, which grows with its
  input. **The grammar takes the caller's meter**, as SJL-6's Date parser does (JSD-0072), and each engine's adapter
  passes the charge it makes today, which is none. So no charge moves, and **there is no class C2 in SJL-10's rules**:
  an exhaustion verdict that moves on fuel is a regression.

### X4 - no edge between the engines, none into Broiler.VM

The library references neither engine and nothing in this repository. Neither engine references the other.

### X5 - boundaries hold, in each sub-area

- **Pure computation.** No shared member reads a clock, a host zone, a culture, `TimeZoneInfo` or a .NET calendar
  class. `Temporal.Now`'s clock and system zone stay each engine's provider (SJL-D-19).
- **Calendars** read the table calendars' years through SJL-8's provider, over the bytes an engine hands in. No
  package carries a table (SJL-D-6 (c)).
- **The grammar** is a linear parser. No platform regular-expression API enters, and Broiler.JS's regular expressions
  over Temporal strings go.
- **Calendar identifiers and month codes** are matched by ASCII rules, with no platform globalization API.
- **Optionality.** This family's Temporal stays admitted only with Intl and BigInt (JSD-0054). The area reaches the
  calendar tables through the `.Intl` assembly's provider only, as today, and rule I3 holds that.
- **The compiler** does not call the area.

### X7 - owners accept

The JavaScript profile owner and the Broiler.JS maintainer accept all seven sub-areas, and record decisions D1 to D9
under the owner's direction of 2026-10-18. One person holds both roles (EX-30).

## The proposal pin, and the owner's decisions of 2026-10-18

Each decision was taken on 2026-10-18 by the owner, under the direction quoted above, as the correspondence
recommends it. Each states its reasons and the rows it rests on.

**Decision D1 - both engines follow `tc39/proposal-temporal@e8cc03fc`.**
- It is the proposal's Stage 4 state, committed on 2026-07-27. Its last normative change outside Intl is `41faca47`
  (2026-01-29). The one later normative change, `377fb80e`, is an Intl change and SJL-9's.
- **This family already follows it**: JSD-0053 section 6 pins it, and the retained datasets are its polyfill's
  answers.
- **Broiler.JS adopts it, and its record names it**, as the milestone's Records clause asks. Its behaviour is the
  revision's on every marker probed but two, and both are corrections: the era names (C9, decision D4), and
  `Duration.prototype.add`'s read of a `relativeTo` the revision no longer has (D3) (the correspondence's facts on the
  revision).
- **The era and month code proposal is not pinned by a decision of its own.** Both engines' calendars follow its
  Tables 1, 2, 3 and 6 as they stand at `5833eae`, the draft SJL-8.1 read and this family's JSD-0056 tables follow.
  Decisions D4 and D5 read "the pinned revision" for eras and aliases through it.
- A later revision moves both engines together, by a dated record.

**Decision D2 - the area's rules run test262 `ccaac100`.**
- It is the suite both engines' evidence workflows run, and every base in both programmes' bundles uses it.
- Moving to `227d9055`, the proposal's own submodule, is a separate dated change outside SJL-10.
- The five test commits between them that change Temporal tests only, `7f2001f6`, `b1f9a0ae`, `0f323ada`, `a70493eb`
  and `2fac7182`, are noted and not adopted.

**Decision D3 - the deletion list reaches every calendar copy in Broiler.JS.**
- SJL-10 takes the formula calendars into the library, so X6 removes Broiler.JS's other copies too:
  `TemporalArithmeticCalendar`, `TemporalTableCalendar`'s arithmetic, `TemporalCalendarMath`, `JSTemporalCalendar`'s
  era tables, and the five per-type month-code copies.
- JSD-0076's X6 kept the first and the third "for the formula calendars"; this is that case.
- It is recorded as a revision of the milestone's SJL-10.5 deletion list, with the superseded text quoted (update
  rule 5). No evidence of SJL-10 exists, so none is re-evaluated.

**Decision D4 - the era names are a correction.**
- Broiler.JS adopts the era codes of the pinned revision: Table 2's codes and aliases.
- It refuses the codes from before the era proposal's change of 2025-06-30 (`08de50a`), the aliases removed on
  2025-11-21 (`684dd16`), and `incarnation`, which no revision has.
- The reason: the specification's CanonicalizeEraInCalendar admits none of them, and no single revision accepts what
  Broiler.JS accepts (C9; the correspondence's facts on the revision). The library keeps no alias table for them.

**Decision D5 - the calendar aliases `gregorian` and `minguo` are a correction.** Broiler.JS refuses them, as the
pinned revision and this family do: AvailableCalendars holds Table 1's calendar types, whose aliases are
`ethiopic-amete-alem` and `islamicc` only (the era proposal's 1.1.1). CLDR's legacy aliases are not read into it (C2).

**Decision D6 - the area design's grammar table gains the rows the correspondence found beyond it**, G3, G4, G6, G8
to G13, G16 to G20 and G22, so that SJL-10.2's grammar vectors cover them. Its stale `with()` reading is corrected:
`+25:00` is refused since SJL-8.7 (G19).

**Decision D7 - where the pinned text and its polyfill disagree, the text rules.**
- **The case found is row D8**: `{months: 10, days: 29, hours: 12}` rounded to months, largest unit year, `trunc`,
  relative to 2019-03-31. The text of NudgeToCalendarUnit gives P11M, which this family follows. The polyfill gives
  P1Y, though 2019-03-31 plus a year is after the destination.
- **Vectors for such a case take the text's answer**, with the polyfill's answer and the disagreement recorded in
  their provenance.
- **The retained dataset, which the polyfill produced, is unaffected**: no line of it is such a case, as this
  family's match on all of it shows.
- **Broiler.JS's answers there are a correction**: P1Y, P11M or P10M29D, by path (rows D7, D8, and F5's case of D8).
- **JSD-0054 section 4's two places stand.** There the text "does not do what it means": its loop would not end, and
  an exact year rounded to months would gain a month. Both engines and the polyfill answer alike there, so these are
  not disagreements the text decides. The shared library keeps them as named places, and D8 does not become a third.

**Decision D8 - one set of records.**
- The library extends the records SJL-8 already published, `IsoDate`, `IsoTime` and `IsoDateTime` in
  `Broiler.ECMAScript.Time`, and adds no parallel types.
- A balancing result that carries overflow days returns them beside the record, not in it.
- The area design's names (`TimeOfDay`) are corrected to the published ones.
- The reason: one set has to serve the zones, the core and both engines, and SJL-8's already serves the first and
  both engines.

**Decision D9 - the library's limit checks answer a status, and R6 is this family's correction.**
- The limit checks take any year an engine hands them and return a status. None throws.
- Row R6 is a correction in this family. Its constructors end the invocation on a year above 2^53, because the shared
  `IsoCalendarMath.EpochDays` precondition throws before the limit check. It is made at the cut-over, SJL-10.4, by
  checking limits first through the library.
- The reason: Broiler.VM holds years as `long` and Broiler.JS as `int`, and the shared civil-day helpers throw beyond
  2^53. Without a status, every adapter would have to bound its years before calling, as this family's property bags
  do and its constructors do not.

## Every row, classified

Each row is in one class. A row is in two only where it names a part, which the cell says.

| Sub-area | Same | Correction of Broiler.JS | Correction of Broiler.VM | Policy decision | Code no input reaches |
|---|---|---|---|---|---|
| Records and limits | R1, R2, R3, R4, R5, R8; R6 but its five faulting cases *(2026-10-19, at SJL-10.5: R4 but for a case Broiler.JS's cut-over corrects, in the dated section below)* | R7 | R6, its five faulting cases (decision D9) | | |
| Balancing | B1, B2, B3, B4, B6, B7, B8, B9 *(2026-10-19, at SJL-10.5: B4 and B9 but for cases Broiler.JS's cut-over corrects, in the dated section below)* | B5 | | | |
| Durations and rounding | D1, D2, D5, D9, D10; D4 but its range check *(2026-10-19, at SJL-10.5: D10 but for cases Broiler.JS's cut-over corrects, in the dated section below)* | D3, D4 (the range check), D6, D7, D11; and Broiler.JS's answers to D8 | | D8: the text rules (decision D7) | D12, in both engines *(2026-10-19, at SJL-10.5: but its day window and its zoned branch, which a zone's skipped day reaches; decisions D11 and D12, in the dated sections below)* |
| Differences | F1, F2, F3, F6, F8; F5 but D8's case *(2026-10-19, at SJL-10.5: F5 and F8 but for cases Broiler.JS's cut-over corrects, in the dated section below)* | F4, F7 | | F5's case of D8 (decision D7) | |
| The grammar | G1, G2, G5, G7, G10, G14, G15, G21, G23, G24; G8 for `PlainTime`, `ZonedDateTime` and `Instant` | G3, G4, G6, G8 (the four plain types), G9, G11, G12, G13, G16, G17, G18, G19, G20, G22 | | | G10: Broiler.JS's U+2212 handling after a match |
| Calendars | C1, C7, C11, C14, C18; C4 but C5; C8 but C9; C10 in its answers *(2026-10-19, at SJL-10.5: C7, C8 and C10 but for cases Broiler.JS's cut-over corrects, in the dated section below)* | C2 (decision D5), C5, C6, C9 (decision D4), C12, C13, C15, C16, C17 | | | C3; C10's search bound |
| Serialization | S1 to S11 *(2026-10-19, at SJL-10.5: S7 and S11 but for a case Broiler.JS's cut-over corrects, in the dated section below)* | | | | S8: Broiler.JS's offset getter's fraction of a second |

**Broiler.JS's corrections**, each a fix under decision SJL-D-16 (a), listed in its changelog at SJL-10.5 with its row:
- **the milestone's listed ones, as the rows confirm them:**
  - lenient grammar acceptances: G3, G4, G6, G8, G9, G11, G12, G13, G16, G17, G18, G19, G20 and G22;
  - integer narrowing that wraps: R7, and its saturating form in an ISO month-day's year, C12;
  - `Duration.prototype.add` with calendar units: D3;
  - rounding options ignored in non-ISO differences: F4;
  - day rounding that ignores daylight saving: F7, for `until` and `since`;
  - invalid month codes silently constrained: C5;
  - reference years computed without the table: the table replaces the search, and no answer moves (C10);
- **found beyond them:**
  - a year-month subtraction at the lower limit: B5;
  - `compare`'s range check: D4;
  - rounding to months through a clamped difference: D6 and D7;
  - the nudge after a shifted window, under decision D7: D8;
  - a non-ISO `relativeTo`: D11;
  - the aliases `gregorian` and `minguo`: C2 (decision D5);
  - month codes with digits other than ASCII: C6;
  - era names: C9 (decision D4);
  - the calendar a `PlainMonthDay` lends: C13;
  - the three conversions: C15, C16 and C17.
- **Code no input reaches is deleted with its file**:
  - the zoned machinery's shift of years and months, its "inconsistent Instants" branch and its fourth day
    correction (D12);
  - the handling of U+2212 after a match (G10);
  - the `islamic` entry (C3) and the reference-date search's upper year (C10);
  - the offset getter's dropped fraction (S8);
  - `RejectIncompatibleStyle`, `NotImplementedArithmetic`, `ReadLargestUnit` and the unused `AddYearsMonths`.

  *(Note, 2026-10-19, at SJL-10.5. The "inconsistent Instants" branch is reached, by a zone's skipped day: it tested
  the day span whose sign NudgeToZonedTime asserts, which decision D12 now governs, and Broiler.JS answered a
  `RangeError` from it. Broiler.JS's records also say that much of this code was deleted from files that remain, not
  with its file. The heading and its items above are left as written.)*

**This family's correction** is R6, made at SJL-10.4 (decision D9). Its second window for week and day units, which no
input reaches (D12), becomes the shared code's assertion or status.

## The design's readings

Four are corrected in `areas/temporal.md` beside the reading, which is quoted there:
- **the grammar table** gains the rows of decision D6, each named by its row;
- **"Offset strings in `with()` … accepts `"Z"` and `+25:00`"**: only `Z` is still accepted; `+25:00` is refused since
  SJL-8.7 (G19);
- **the target's records** are SJL-8's published `IsoDate`, `IsoTime` and `IsoDateTime` in `Broiler.ECMAScript.Time`,
  not `TimeOfDay` beside new types (decision D8);
- **the proposal pin**: both engines follow `e8cc03fc`, where the design read "No proposal revision is pinned" for
  Broiler.JS (decision D1).

The other readings are confirmed or refined in the correspondence's section 9, and a dated paragraph under the
design's table names what it omits:
- "reads options part-way through the parse" is true of the code's shape, with no observable consequence (G24);
- "Duration strings | exact / `double.Parse`" and "Month-day reference years | a search without it" are true of the
  code, with equal answers (G23, C10);
- "Rounding options in non-ISO `until`/`since` | ignored": `PlainDateTime` applies them, against ISO months (F4);
- "`ZonedDateTime` day rounding across DST | 24-hour days": in `until` and `since` only (F7, D9).

## Decision

1. **All seven sub-areas move to `Broiler.ECMAScript.Algorithms`**, beside SJL-8's `Broiler.ECMAScript.Time`. Within
   X3's constraints:
   - the records are SJL-8's (decision D8);
   - the grammar is a `TryParse` that answers a record or an error code;
   - calendars and zones are reached through provider interfaces, the table calendars over SJL-8's provider;
   - the options' rules are the library's.

   SJL-10.3 settles the names.
2. **The pin.** Both engines follow `tc39/proposal-temporal@e8cc03fc` (decision D1). The area's rules run test262
   `ccaac100` (decision D2). Where the pinned text and its polyfill disagree, the text rules (decision D7).
3. **The engines keep:**
   - their Temporal objects and slots;
   - the reads in specification order, coercion, `relativeTo` resolution and `ToTemporal*` classification;
   - the mapping of statuses to errors, and their messages (JSD-0054 section 5);
   - `Temporal.Now`'s clock and system zone (SJL-D-19);
   - Intl formatting of Temporal objects, which is SJL-9's;
   - charging. Nothing in the area charges today, and no shared member adds a charge. There is no class C2.
4. **Corrections.**
   - **Broiler.JS's** are those section "Every row, classified" lists, fixes under decision SJL-D-16 (a), each listed
     in its changelog at SJL-10.5.
   - **This family's** is R6, made at SJL-10.4 (decision D9).
5. **The deletion lists** are X6's. Broiler.JS's reaches its calendar copies (decision D3), and the milestone's SJL-10.5
   list is revised under update rule 5.
6. **The reference datasets** this family retains under `src/tests/temporal/` stay unchanged here, and run in both
   engines from SJL-10.2. Broiler.JS runs `temporal.js` and `calendars/temporal-calendars.js` against this family's
   answers, the second copied at SJL-10.2. The three Intl programs beside them are SJL-9's to share.
7. **The bases** are bundle `sjl-10-001`'s in each repository, taken at the commit that carries each repository's rule.
   The rules are committed with this record.
8. **JSD-0053, JSD-0054 and JSD-0056 are not amended.** The pin JSD-0053 records stands, JSD-0054 section 4's two
   named places stand (decision D7), and the code they built moves at SJL-10.4 under that work package's own record.

## What this rejects, and why

- **A second set of records beside SJL-8's**: decision D8.
- **The polyfill's answer where the text gives another**, and a third named place in JSD-0054 section 4: decision D7.
- **An alias table for Broiler.JS's older era names, and CLDR's legacy calendar aliases**: CanonicalizeEraInCalendar
  and AvailableCalendars admit neither (decisions D4 and D5).
- **test262 `227d9055` now**: decision D2.
- **Keeping Broiler.JS's formula and table calendar classes**: decision D3.
- **A limit check that throws**: decision D9.
- **Sharing the option reads, `relativeTo` resolution, `Temporal.Now`'s clock or Intl formatting**: they stay each
  engine's, or SJL-9's.

## SJL-10.2's vectors, read against this record

*(Added 2026-10-18, at SJL-10.2.)* Broiler.JS's corpus revision 24 adds the Temporal area's vectors, every expected
value from the proposal's polyfill at `e8cc03fc` (decision D10, recorded in the ledger). Revision 25 classifies both
engines' first runs on them. **They do not falsify this record**: every difference either engine shows is under a row
this record classifies, and none needed a new row.
- **Broiler.VM** differs on six vectors, all row R6. The correspondence counted five faulting cases. The vectors find a
  sixth with the same cause, `new Temporal.PlainDateTime(2^53 + 2, ...)`, so decision D9 holds as written.
- **Broiler.JS's differences** are all corrections this record lists: R7, B5, D3, D4, D6 to D8, D11, F4, F7, the grammar
  rows and the calendar rows.
- **Row D11 reaches one more input.** A `relativeTo` that is a `ZonedDateTime` in the `coptic` calendar is not refused
  either: Broiler.JS rounds it in ISO months. It stays a correction under D11.
- **Decision D7 decides more cases than row D8.** The vectors take the text's answer in 73 places and record the
  polyfill's beside it. Beyond D8's nudge, which accounts for 28, the polyfill's parser and era table disagree with
  the pinned texts in 45 vectors:
  - two spaces as the date-time separator;
  - an offset's decimal separator with no digit;
  - the month-days `02-30` and `04-31`;
  - calendar strings that are only a year-month or month-day;
  - time-zone strings that are a time or partial date;
  - the era aliases removed on 2025-11-21;
  - one lenient regnal era.

  Broiler.VM answers as the texts on all 45. Broiler.JS does so on 35, and its other 10 are corrections already listed
  (G17, C9). No class changes.
- **JSD-0054 section 4's places stand, as decision D7's last item says.** In ComputeNudgeWindow the vectors keep the
  polyfill's answer, which both engines give, and each such vector's note gives the literal text's answer.
- **Not vectored**, because the reference cannot vouch for them:
  - Hebrew years before AM 1, and 5806 to 5807, where ICU4C and Calendrical Calculations disagree;
  - non-ISO fields at the ISO limits;
  - the table calendars outside Table 1's published years.

  The library's own tests, and the reference datasets, hold these where they can.

## SJL-10.3's library, read against this record

*(Added 2026-10-18, at SJL-10.3.)* Broiler.JS's `Broiler.ECMAScript.Algorithms` now holds the seven sub-areas, under
`Broiler.ECMAScript.Temporal`.
- **The check against this family's code.** A differential check against this family's code at `14072b0` finds no
  difference in 76,814,791 comparisons, but one case.
- **The vectors.** At library level the library answers all but 27 of corpus revision 25's Temporal vectors, and every
  one it answers matches. The 27 are excluded by operation, because each depends on the engine: a built-in's `length`,
  a getter's reads, and the calendar an object lends (row C13).
- **Its records** are in Broiler.JS's bundle `sjl-10-001`, section 3.

**Row D12 is reached.** This record classified row D12 as code no input reaches, in both engines. Pacific/Apia skipped
2011-12-30, so a day nudge window back from 2011-12-31 ends on the skipped day and has no length. The specification
asserts that this does not happen. For example:

```
ZonedDateTime.from("2011-12-31T16:15+14:00[Pacific/Apia]")
  .since("2011-12-31T00:00+14:00[Pacific/Apia]", {largestUnit: "day", smallestUnit: "day"})
```

Each implementation answers differently:
- this family's code ends the invocation with a .NET `ArgumentOutOfRangeException`;
- Broiler.JS answers `PT0S`, from its own day arithmetic;
- the polyfill fails its own assertion.

**Decision D11 - where the pinned text asserts that a nudge window holds its destination and an input breaks the
assertion, the answer is a `RangeError`.** Taken by the owner on 2026-10-18, under the same direction as D1 to D10.
- The library's rounding and totals answer `OutOfRange` there, which each engine maps to a `RangeError`.
- In this family, row D12's day window is a correction, made at its cut-over (SJL-10.4) beside R6: an escaping host
  exception ends the invocation where a `RangeError` is catchable.
- In Broiler.JS it is a policy change, `PT0S` to a `RangeError`, made at SJL-10.5 and listed in its changelog.
- The reasons:
  - The text gives no answer there, and the polyfill's assertion is not one an engine may surface.
  - A `RangeError` is what the text gives wherever an operation's result is out of its range.
  - Neither engine's current answer is one the text supports.

The classification table above is read with this section. Row D12's week window remains code no input reaches; its day
window is reached. *(Note, 2026-10-19, at SJL-10.5: so is its zoned branch, NudgeToZonedTime's day span, which section
"SJL-10.5's cut-over, read against this record" records with decision D12.)*

## SJL-10.5's cut-over, read against this record

*(Added 2026-10-19, at SJL-10.5.)* Broiler.JS's Temporal core is the shared library's `Broiler.ECMAScript.Temporal`,
which its built-ins take by project reference: the shared stream's `0.1.0-preview.6`, with decision D12 below. The
change is observed in Broiler-Platform/Broiler.JS#1054 at its head `c33933f9`, merged as `00b65dd0` on 2026-10-19
*(revised that day, when it merged; read "observed in Broiler-Platform/Broiler.JS#1054, open and not yet merged, at its
head `c33933f9`")*: the code in
`f42eac6b` to `6dbff805`, corpus revision 26 in `feee5b7a` and `c97a9996`, and the records after them. Its records are
its changelog, `Broiler.JS/CHANGELOG.md`; its plan's "The cut-over (SJL-10.5)", in `docs/roadmap/SharedLibraries.md`;
and its bundle `sjl-10-001`, section 4 and `cutover/`.

**Every row this record classed a correction of Broiler.JS is corrected**, and its checks show it:
- **The rows:** R7; B5; D3, D4's range check, D6, D7 and D11, and its answers to D8 under decision D7, with F5's case
  of D8; F4 and F7; G3, G4, G6, G8 for the four plain types, G9, G11, G12, G13, G16, G17, G18, G19, G20 and G22; C2
  (decision D5), C5, C6, C9 (decision D4), C12, C13, C15, C16 and C17. C10's reference years are Table 6's, and no
  answer changed.
- **The vectors.** Each of corpus revision 25's 473 `divergence.broiler-js` fields, all under those rows, reports that
  it no longer holds, in UTC and in `America/New_York`, and no other vector moved. Revision 26 retires them, with this
  family's six of row R6, and Broiler.JS's runs on it match every Temporal vector in both zones.
- **The reference datasets.** Every line of this family's `temporal.js` and `temporal-calendars.js`, run in Broiler.JS
  by its rule's harness, equals this family's answer. The lines that moved there are rows D6, C5, C15, C16, C17, F4 and
  D11's, and none moved the other way.
- **The correspondence's probes.** Every line is this family's answer, by error type, but row R6's five, whose retained
  answers are this family's from before its cut-over corrected the row. Each engine keeps its own messages.
- **test262.** A local selection over the area gives every path its base verdict, in both zones, so no test262 path
  pins a correction. These are Broiler.JS's own checks; its rule's hosted runs are SJL-10.6's.
- **The deletion list is absent**, by a search for its names, and the code no input reaches is deleted: G10, C3, C10's
  search bound, S8, and row D12's shift of years and months and fourth day correction. Much of it went from files that
  remain, not with its file.

**Changes the correspondence did not see, kept under the owner's direction.** Broiler.JS's parts, the review of its
cut-over and its records found behaviour changes that no row lists. Under the owner's direction of 2026-10-18 they are
kept: each follows the pinned text, and each is the library's answer, and so this family's, where undoing it would need
engine code around the library. Broiler.JS's changelog lists each as a correction under decision SJL-D-16 (a), with
its probe:
- **part A's two:** a `relativeTo` property bag's `offset` is parsed at its read, so `+25:00` beside a `timeZone` of 5
  is the offset's `RangeError`, where the zone's `TypeError` came first; and `ZonedDateTime.prototype.toString`
  converts its `offset` option once, where it converted it twice;
- **part B's seven:**
  - a property bag in a calendar other than ISO 8601 and the Gregorian family is read in the specification's order,
    and the Gregorian family's `with` and `PlainMonthDay.prototype.toPlainDate` read `era` and `eraYear` before
    `year`;
  - a missing field's `TypeError` follows the `overflow` option's read;
  - a field is converted at its read: `toPlainDate({day: 0})` of a year-month is a `RangeError`, where the day was
    clamped to 1;
  - a month-day's `with` regulates against 1972;
  - `PlainMonthDay.from` in `buddhist` and `roc` reads a year as the calendar's, as row C17's `toPlainDate` does;
  - a non-ISO year-month whose month begins before -271821-04-19 is out of range, by row B5's rule;
  - an era and a year that disagree are a `RangeError`, where the year was taken;
- **five of part C's six** (its fifth, that `add` and `subtract` read no second argument, is row D3's, above):
  - a `relativeTo` property bag is read in one pass, and an unknown calendar is refused before any field is read;
  - its `offset` is parsed at its read;
  - its time zone is converted before `year`;
  - its `monthCode` and `day` are checked at their reads;
  - a year-month difference is zero only between equal dates, so at the limits it is a `RangeError`, where it was
    `PT0S`;
- **the review's U1 to U8:**
  - a nudge or bubble window past the date limits is a `RangeError` (U1 to U3);
  - a rounded difference's days bubble into months (U2);
  - an `era` is converted by ToString at its read (U4);
  - a non-ISO month-day given a `month` needs a year (U5);
  - `PlainMonthDay.prototype.with` reads `era` and `eraYear` in a calendar with eras, and `PlainMonthDay.from` in the
    Gregorian family reads its options before it validates the era (U6);
  - an unknown era, or an era and a year that disagree, is a `RangeError` in a year-month's and a month-day's `with`
    (U7);
  - `PlainYearMonth.prototype.with` constrains a leap month the year lacks (U8);
- **beside them:** a wall clock on -271821-04-19 with an offset is a `RangeError` under `reject` and `prefer`, by
  InterpretISODateTimeOffset's range check; and the records found the rounding of points: a minute rounds within its
  hour and a second within its minute, so `01:10` rounded to 20 minutes under `halfEven` is `01:00`, where Broiler.JS
  answered `01:20`, the case its SJL-4.5 changelog entry left for SJL-10.

**Two defects of Broiler.JS from before its cut-over**, which the review fixed, are corrections of the same kind:
- **code-1:** a Temporal object read as a property bag lends its own calendar, where its getters were read as ISO
  fields. `PlainDateTime.from` of a Hebrew `ZonedDateTime` is a Hebrew date-time, where it was `5780-04-18T10:00:00`.
- **code-2:** `ZonedDateTime.from` of a `PlainDate` or `PlainDateTime` reads it as a property bag, and throws the
  `TypeError` for its missing `timeZone`, where a .NET `ArgumentNullException` reached script as a plain `Error`.

**All of these are findings the correspondence missed.** Its probes did not reach them, and no vector, dataset line or
test262 path names any. Broiler.JS's bundle probes each, in `cutover/kept-changes.js`: in Broiler.JS before and after
its cut-over, in this family at `83fef1a` on `0.1.0-preview.6`, and in the polyfill at `e8cc03fc`. On every line that
changed, Broiler.JS's answer after the cut-over is this family's and the polyfill's (`cutover/kept-changes.compare.tsv`).

**None falsifies a class, and none changes a verdict.**
- Each is a place where Broiler.JS answered otherwise than the pinned text and this family. So each is a correction of
  Broiler.JS, a class every sub-area it falls in already holds.
- None is a correction of this family, a disagreement between the text and its polyfill, or code no input reaches.
- Section "Falsified if" reads the vectors, and no vector differs. Every sub-area stays accepted, its X2 met.
- Some fall within rows this record called same, by the inputs each row lists, though the row's probes did not reach
  them:
  - R4: the wall clock of the earliest instant, in a zone twelve hours behind UTC, given with its offset (beside
    them, above);
  - B4: a bag's day 0 (part B's third), and a month-day's `with` regulated against a leap year (part B's fourth);
  - B9: the rounding of points at a tie under `halfEven`, and the earliest instant in that zone rounded (beside them);
  - D10: a total whose window passes the limits (U1), or relative to that wall clock with its offset (beside them);
  - F5: a rounded `PlainDateTime` difference whose days bubble into months, and one over the full range (U2);
  - F8: year-month differences at the limits (part C's sixth, and part B's sixth in `coptic`);
  - C7: a year-month's `with` to a leap month the year lacks (U8);
  - C8: an era against a year in `with` (part B's seventh, U7), an era given as an object (U4), and the eras a
    month-day reads (part B's first, U4 and U6);
  - C10: a month-day given a year (part B's fifth), or a month without one (U5);
  - S7 and S11: `ZonedDateTime.prototype.toString` converted an invalid `offset` option twice (part A's second).

  Each row is same but for these cases. There the change is a correction of Broiler.JS, on inputs the two engines
  never answered alike, so the third item of section "Falsified if" is not met.

**One divergence from the text stays as it was**, in the reads decision 3 leaves each engine: Broiler.JS refuses a
string whose date is outside the limits before it reads the `overflow` option, where the text, this family and the
polyfill read it first. It is in no row of the area's.

**Decision D11 in Broiler.JS.** With `a` 2011-12-31T16:15+14:00[Pacific/Apia] and `b`
2011-12-31T00:00+14:00[Pacific/Apia], exactly these moved to a `RangeError`, "result is outside the representable
range": `since`, `until` and `round` in each of the nine rounding modes, and `total`, which takes no mode.
- `a.since(b)` and `a.until(b)` with `{largestUnit: "day", smallestUnit: "day"}`. `since` answered `PT0S` under
  `trunc` and `floor` and `P1D` in the other seven modes; `until` answered `PT0S` under `trunc` and `ceil` and `-P1D` in
  the other seven;
- `a.since(b)` with `{largestUnit: "month", smallestUnit: "day"}`, which answered `PT0S` under `trunc` and `floor` and
  `P1D` in the other seven;
- `Temporal.Duration.from("-PT16H15M").round({smallestUnit: "day", relativeTo: a})`, and its
  `total({unit: "day", relativeTo: a})`, which threw a plain `Error` carrying a .NET `ArgumentOutOfRangeException`.

Nothing else moved to a `RangeError`; decision D12's lines, below, keep their `RangeError` with another message.
`b.until(a)` and `b.since(a)` in days, rounding to weeks, differences in hours, anything relative to `b`, `compare`,
`a.round("day")`, `hoursInDay` and `a.subtract("P1D")` answer as before. So decision D11's "In Broiler.JS it is a policy
change, `PT0S` to a `RangeError`" holds for `since` and `until` under `trunc`, the default, under `floor` for `since`
and under `ceil` for `until`. In their other modes the answer was a day, and `round` and `total` threw a plain `Error`.
Broiler.JS's `cutover/d12-ext.js` and `cutover/d12-half-modes.js` probe all nine modes. The sentence is left as
written.

**Row D12's zoned branch is reached too.** This record classed all of row D12 code no input reaches, and section
"SJL-10.3's library, read against this record" found its day window reached. Broiler.JS's "inconsistent Instants"
branch, which the correspondence also read as unreached, tests the assertion at step 8 of NudgeToZonedTime (s7.5.35):
"Assert: TimeDurationSign(_daySpan_) = _sign_." The same skipped day breaks it:
- Back from `a` to an earlier instant of the same day, as to `b`, the difference has no date part. The nudge's start
  is then 2011-12-31T16:15 and its end 2011-12-30T16:15, a wall time the zone skipped, which `compatible`
  disambiguation moves 24 hours on, onto the start. The day span is zero.
- Rounding there to a time unit with a date largest unit reaches it, but to a nanosecond with increment 1, which
  returns before rounding. Examples are `a.since(b, {largestUnit: "day", smallestUnit: "hour"})` and `a.until(b)`
  alike, with a largest unit of `month`, to 30 minutes or to 2 nanoseconds, and `-PT16H15M` rounded so relative to `a`.
  The probes run these, from `a` to `b` and with `-PT16H15M`.
- A difference back to an earlier day has a date part: the nudge's end falls before the skipped day, and the span keeps
  its sign. This is read from the step; the probes run `a.since(c)`, `c` on 2011-12-29, in days only.
- Forward from `b` the day span is the next day's, 24 hours, and nothing changes.

The answers there:
- Broiler.JS answered a `RangeError` before its cut-over, "time zone returned inconsistent Instants", from its own zoned
  code.
- The polyfill at `e8cc03fc` throws the same `RangeError`.
- This family's code at `14072b0` has no such test, and answered durations from outside the text's domain: `P1DT16H`
  for `a.since(b)` above under `trunc` and `P1DT17H` under `ceil`, `-P1DT16H` for `a.until(b)` under `trunc`. Each
  counts a day more than the 16 hours 15 minutes between the two instants.
- `0.1.0-preview.6`, ported from that code, answers the same durations, line for line, and so does this family on it.
- Broiler.JS's cut-over before the decision, at `41eaa90f`, answered those durations too. Without a decision, the
  cut-over would have moved Broiler.JS's answers there from a `RangeError` to them.

Broiler.JS's `cutover/d12-facts.md` gives every line in every engine but part C's shell, from its probe
`cutover/d12z.js`; part C's shell's answers are in `cutover/d12z.part-c.txt`. No test262 path, retained dataset line
or corpus vector reaches the step.

**Decision D12 - decision D11's rule reaches NudgeToZonedTime's assertion on the day span's sign: where an input breaks
it, the answer is a `RangeError`.** Taken by the owner on 2026-10-19, under the same direction as D1 to D11.
- The library answers `OutOfRange` there, which each engine maps to a `RangeError`. Broiler.JS's `163d43aa` makes the
  change after `0.1.0-preview.6`, and the shared changelog, `Broiler.ECMAScript/CHANGELOG.md`, records it under its
  Unreleased heading.
- In this family it is a policy change pending, durations to a `RangeError`, made when it takes the shared stream's
  next version, which carries the decision. This family takes the library as `0.1.0-preview.6`, which answers the
  durations above, as this family's code did before its cut-over; until then the durations stand. It is not a
  correction in decision D11's sense: no host exception escapes there.
  *(Note, 2026-10-19, later. The shared stream's next version, `0.1.0-preview.7`, was published that day from
  Broiler.JS `5ddee404`, and this family moves to it in the change that carries this note
  ([JSC-317](../roadmap.corrections.md#jsc-317)), so the policy change is made: `a.since(b, {largestUnit: "day",
  smallestUnit: "hour"})` above, `P1DT16H` on `0.1.0-preview.6`, is a `RangeError` here, the library's `OutOfRange`
  mapped as every status of the core is, and the slice compiler's check
  `temporal/t1/a-day-span-a-zone-skips-is-a-range-error` holds it. The bullet above read the durations as standing
  "until then", and is left as written.)*
- In Broiler.JS the answer stays a `RangeError`, as before its cut-over, and only its message moves, to "result is
  outside the representable range". Its changelog lists the decision with its policy changes.
- The reasons:
  - It is decision D11's case: a day a zone skips breaks an assertion of the pinned text's rounding, and the text gives
    no answer there.
  - The polyfill throws a `RangeError` there, and Broiler.JS answered one; a `RangeError` is what the text gives
    wherever an operation's result is out of its range.
  - A duration that counts a day more than lies between its instants is not an answer the text supports.

**The classification table above is read with this section too.** Row D12's day window and its zoned branch are
reached. Its week window, and Broiler.JS's shift of years and months and fourth day correction, which its cut-over
deleted, remain code no input reaches. Rows R4, B4, B9, D10, F5, F8, C7, C8, C10, S7 and S11 are same but for the
cases listed above, each a correction of Broiler.JS; X2's lists and the table carry dated notes that say so.

**Not this section's:** the merge of Broiler.JS's change; this family's pin, which moves to corpus revision 26 after
it; the shared stream's next version, and this family's move to it; and SJL-10.6's evidence. The settling condition
below is not yet met. *(Note, 2026-10-19, later: the shared stream's next version is `0.1.0-preview.7`, and this
family moves to it with decision D12 in [JSC-317](../roadmap.corrections.md#jsc-317).)*

## Settling condition

This record is settled when SJL-10.4's and SJL-10.5's cut-overs land with the deletion lists absent from both engines
and R6 corrected, and every rule of bundle `sjl-10-001` has been judged in both repositories, whatever each decides.

## Correction entries

None in this profile's plan documents at this record: the corrections it names are applied in the cut-overs. The
milestone's SJL-10.5 deletion list is revised under update rule 5, and `areas/temporal.md` is corrected beside its
readings, each quoting the superseded text.

## What this record does not decide

- **Either cut-over.** SJL-10.4 and SJL-10.5 apply this record, and each repository's rule judges its own.
- **The vectors.** SJL-10.2 adds them, and they may falsify this record (below).
- **The library's names and the shapes of its provider interfaces**, which SJL-10.3 settles within X3.
- **A move to test262 `227d9055`, or to a later proposal revision.** Each is a dated change of its own.
- **Intl formatting of Temporal objects** (SJL-9), and **`Temporal.Now`'s clock policy** (SJL-D-19).
- **A speed claim of any kind.** SJL-10 measures nothing.

## Falsified if

- SJL-10.2's vectors find a difference between the engines, or between an engine and the pinned text, that the
  correspondence does not classify.
- A test262 case at `ccaac100` asks for the polyfill's answer where the pinned text gives another, so that decision D7
  would fail it.
- A shared member cannot answer every row the correspondence calls the same as both engines answer it today.
- This family's retained datasets move at its cut-over.
