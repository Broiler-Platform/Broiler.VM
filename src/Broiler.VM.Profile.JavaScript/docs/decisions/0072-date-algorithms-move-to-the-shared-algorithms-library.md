<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0072 - Date algorithms move to the shared algorithms library: SJL-6's area verdict, sub-area by sub-area, and decision SJL-D-19

**Status:** Decided 2026-10-15: **all four sub-areas accepted**, and **decision SJL-D-19 recorded**. The
programme's extraction gate (its roadmap section 5, conditions X1 to X7) holds for each of the Date area's
sub-areas on its own: the time arithmetic, the civil-day helpers, the Date string grammar and the Date
string renderings. Each moves to `Broiler.ECMAScript.Algorithms` at milestone SJL-6. Local time moves
with them as an adoption, under SJL-D-19, which this record files. Nothing in this profile changes with
this record; the cut-over is SJL-6.4's own change, and this record is what it applies.

**Date:** 2026-10-15.

**Owner:** the JavaScript profile owner. **Co-signers:** the Broiler.JS maintainer, as the home's
owner and as SJL-D-19's other holder, and the core architecture owner, for rule I1's allow-list and the
composition register. **All three roles are held by one person** (EX-30), and this record does not claim
that anyone independent has read it.

**Milestone:** the programme's SJL-6, work package SJL-6.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts". **SJL-6 started before
SJL-3 was met**: its first work package, SJL-6.0, changes no product code, and the programme ledger
records that SJL-6 may not meet its gate before SJL-3 meets SJL-3's. This record moves no gate of either
milestone.

---

## Context

The area's design is the programme's
[`areas/date.md`](../../../../docs/javascript-shared-libraries/areas/date.md). It has no vectors yet:
SJL-6.2 adds them after this record, and the design marks its readings *(reading)* until they do. So the
correspondence is **from code**: it was re-derived by reading both engines at **Broiler.VM `9c3d809`**,
whose product sources the default branch after SJL-5's merges carries unchanged, and **Broiler.JS
`bc3597f`**, its default branch after SJL-5's merges. Every member is named by file and line in
[`correspondence.md`](../../../../docs/evidence/sjl-6-001/correspondence.md), which this record's change
retains in bundle `sjl-6-001` as the gate's input, with each difference numbered (T, D, G, L, P, R). **Every
behavioural cell the rows below rest on was also run**, through this family's command-line root and through
Broiler.JS's shell, under UTC and under `America/New_York`; SJL-6.2's vectors confirm or refute each one,
and **a difference they find that this record does not classify falsifies it**.

The re-derivation found the area smaller than the design in one place and larger in others. **Four of
the design's Broiler.JS differences no longer exist**: SJL-4.5 made `TimeClip` fold -0, `MakeTime` and the
setters truncate, and the setters convert large arguments without a wrap, and Broiler.JS's changelog lists
them there. Everywhere else the area holds more than the design named: the civil-day arithmetic is copied
in nine Broiler.JS types and four places here; Broiler.JS's `Date` keeps two offsets for one zone and
reads the offset of the moment of the call for dates it cannot hold; and **this family has three
corrections of its own** (T4, G3, P9). Section "The design's readings" lists what does not hold; X6
extends the deletion lists.

## The gate, sub-area by sub-area

### Time arithmetic

**X1 - met.** Both engines implement every operation of 21.4.1, from `Day` to `TimeClip`, over doubles
(correspondence section 1). Neither has a member for `MakeFullYear`, which each writes inline in two to
four places; it becomes a library member beside the rest.

**X2 - met, with two corrections.**

- **T4 corrects both engines**: `MakeDay`'s month at or past 2^53. The specification takes `ℝ(m) modulo
  12` exactly, so `Date.UTC(-833333333333333333, 1e19, 1)` is May 1 of year 0. This family takes the
  modulo in floating point and answers January; Broiler.JS casts the month to `long`, which saturates,
  and answers August.
- **T3 corrects Broiler.JS**: a setter whose first argument is `undefined` or NaN writes the invalid
  date before converting the later arguments, where the specification writes the date last.
- T1 and T2 agree since SJL-4.5. T5, `MakeDay` past any time value's year, is not observable, since every
  caller clips; the shared member answers NaN there, as the specification does.

**Date's storage is Broiler.JS's own.** Rows D1 to D5 (a valid date outside 0001 to 9999 refused by twelve
setters, a date stored raw that those setters cannot change, the instant 0001-01-01T00:00Z read as
invalid, `getTimezoneOffset` without a raw path, a structured clone that loses a raw date) are defects of
the `DateTimeOffset` storage, which the cut-over replaces with the time value (milestone SJL-6.5). They
are corrections of Broiler.JS's own code, not of a shared member.

**X6 - extended.** Broiler.VM: the `Date*` arithmetic members from `DateFloorMod` to `DateTimeClip`, the
inline month table of `DateMonthFromTime`, the constants `DateMsPerSecond` to `DateMaxTimeValue`, and
the inline MakeFullYear of `DateFromFields` and `setYear`; Intl's `ClippedTime` calls the shared
`TimeClip`. Broiler.JS: `JSDateMath`'s arithmetic, its unused `DaysInYear` and `TimeFromYear` included; the
inline MakeFullYear copies, and the dead one; Intl's dozen reads of `JSDateMath`, which become calls to the
shared members; and, with the storage change, `JSDate.ToDateTime`, `invalidDate` and the unused
`IsValid(JSValue, ...)`, which are dead. **`Setters.CoercedIntValue`, which the design lists, no longer
exists**: SJL-4.5 replaced it by `IntegerComponent`, a call to the shared `ToIntegerOrInfinity`, which
stays as the adapter's conversion.

### Civil-day helpers

**X1 - met.** Both engines implement the proleptic Gregorian calendar over integers: leap years, month
lengths, epoch days in both directions, the weekday, the day of the year and the ISO week (correspondence
section 2). Broiler.JS holds them in nine types; this family in `JsTemporalCore`, in `JsZone`, in
`JsLocalTime.From` and in `JsTemporalParser.ValidMonthDay`.

**X2 - met, with two corrections.**

- **G3 corrects this family**: `Temporal.PlainMonthDay.from` reduces a year past 1e9 to `1972 + year %
  400` before regulating the date, and 1972 is not a multiple of 400, so `{year: 1000000100, month: 2,
  day: 29}` is admitted under `reject`, where that year's February has 28 days.
- **G2 corrects Broiler.JS**: the ISO week of a date in a negative year, because its `WeeksInIsoYear`
  divides with truncation; `-000002-12-28` is week 1 of year -1 there, and week 53 of -2 in the
  specification and here.
- **G4**: the engines count weekdays three ways. The shared members answer the specification's two,
  `WeekDay` from Sunday 0 and `ISODayOfWeek` from Monday 1, and `JsZone`'s rule arithmetic maps its own.

**The target grows by three members.** The design's `IsoCalendarMath` cannot replace
everything its deletion list names: it gains `DaysInYear(long)`, `BalanceYearMonth(long, long)` (the
proposal's BalanceISOYearMonth) and `IsValidIsoDate(long, int, int)`, which both engines implement (this
family once, Broiler.JS five times). `AddDays` is `FromEpochDays(EpochDays(d) + n)` over this family's date
record, and stays as that one-line adapter. `DayOfYear` and `WeekOfYear` take the year, month and day;
each engine's adapter unpacks its own record.

**X6 - extended.** Broiler.VM: `JsTemporalCore`'s `IsLeapYear`, `DaysInMonth`, `DaysInYear`, both
`EpochDays`, `FromEpochDays`, `BalanceYearMonth`, `DayOfWeek`, `DayOfYear`, `WeekOfYear` and
`IsValidIsoDate` (`AddDays` becomes a call); **`JsZone`'s** `DaysFromCivil`, `MonthLength` and `Weekday`,
which the design placed in `JsTimeZones`; the civil arithmetic of `JsLocalTime.From`;
`JsTemporalParser.ValidMonthDay`; and the reduction in `MonthDayFromFields`. Broiler.JS: every copy
correspondence section 2 lists - the leap year, the month length, `DaysFromCivil` and `CivilFromDays`,
`IsoDayOfWeek`, `IsoWeek`, `WeeksInIsoYear`, `IsValidISODate`, the inline days of the year, and the
`FloorDiv` beside each that serves only them - in `JSTemporalInstant`, `JSTemporalPlainDate` (with
`EpochDaysFor` and `DateFromEpochDays`, which become calls), `JSTemporalPlainDateTime`,
`JSTemporalPlainMonthDay`, `JSTemporalPlainYearMonth`, `JSTemporalZonedDateTime`, `TemporalIsoString`,
`TemporalArithmeticCalendar` and `TemporalNonIso`; `JSDate.DaysInMonth`; and Intl's `DayOfWeek`.
**Temporal's range limits and `BalanceTime`, the zone compiler's year estimate and the non-ISO calendars
stay** with SJL-10 and SJL-8.

### The Date string grammar

**X1 - met for the format, and an adoption for this family's fallback.** Both engines implement the Date
Time String Format (21.4.1.32): this family inline in `DateParseText`, Broiler.JS twice, as .NET format
strings in `DateParser` and as a regular expression in `TryParseExtendedIso` (correspondence section 4).
This family's implementation-defined fallback, `DateParseTextual`, is its alone; Broiler.JS's is its own,
culture-dependent, and stays (SJL-D-17). Moving this family's fallback as a dialect of the shared parser is
an adoption, recorded as such; Broiler.JS consumes it for one purpose, below.

**X2 - met, with three corrections, five implementation-defined surfaces and one policy.**

- **P1 corrects Broiler.JS**: a date-only form is read as local time for years 0001 to 9999, where the
  format reads it as UTC; it already reads year 0, negative years and years past 9999 as UTC.
- **P9 corrects both**: neither engine reads back all of its own renderings, which Date.parse's
  expectation (21.4.3.2) asks of every time value. This family's fallback reads no sign, so its renderings
  of negative years give NaN; Broiler.JS's .NET formats hold no year outside 0001 to 9999. **P8** is
  Broiler.JS's within that range, under a culture whose minus sign is not U+002D (R3).
- **P4 to P7 are implementation-defined**: a fraction of other than three digits, a day past its month's
  length, lower-case `t` and `z`, an offset without its colon, and the fallbacks' shapes are outside the
  format. Each engine keeps its own reading, and the vectors hold one expectation per engine.
- **P10, the charge, is this family's policy** (decision 4).

**The dialects.** The shared parser answers a time value with its kind - UTC, local or invalid - and takes
a dialect:

- **the strict dialect** is the format and nothing else: it refuses a day past its month's length and
  every shape P4 to P7 name;
- **Broiler.VM's dialect** is this family's reading exactly as it stands: the format with its day carried
  into the next month (P5), then its textual fallback with every quirk P7 names, **except P9's
  correction**: it reads a sign and digits as a year, so that its renderings of negative years read back.

**Each engine's order.** This family calls the shared parser in its own dialect. **Broiler.JS calls the
strict dialect, then its own fallback, then Broiler.VM's dialect for what both refuse.** That order differs
from the design's, which put both shared grammars first: this family's fallback reads a two-digit year
literally and skips words it does not know, so calling it before Broiler.JS's own would change answers
Broiler.JS gives today for no reason the specification states. Called last, it changes only strings
Broiler.JS refuses today, among them its own renderings outside 0001 to 9999, which is P9's correction. What
only `TryParseExtendedIso` and `DateParser`'s format-string lists accepted beyond the format - fractions of
zero or of four to nine digits, lower-case `t` (P4, P6) - is Broiler.JS's to keep in its fallback or drop at
SJL-6.5, under SJL-D-17, and its changelog lists which.

**X6 - extended.** Broiler.VM: the body of `DateParseText`, which keeps its charge and becomes a call;
`DateParseTextual`, `DateMonthByName`, `DateWholeNumber` and `DateReadDigits`; and the three stale comments
around them. Broiler.JS: `TryParseExtendedIso` with `EcmaIsoDateTime`, and `DateParser`'s lists of the
format (`DefaultFormats` and the `\dT` lists); its `N/N/N` heuristic, its `SecondaryFormats` and its
culture parses stay as its fallback.

### The Date string renderings

**X1 - met.** Both engines implement `toString`, `toDateString`, `toTimeString`, `toUTCString` and
`toISOString`, with the specification's year padding (correspondence section 5).

**X2 - met, with one correction and one implementation-defined surface.** **R3 corrects Broiler.JS**: west
of UTC, `toString`'s `GMT-hhmm` takes its minus sign from the current culture, U+2212 under `sv_SE` and
other code points under `fa_IR` and `ar_SA`, where TimeZoneString writes U+002D. **R4**: the zone's name
in parentheses is implementation-defined; it becomes the provider's (below), this family's fixed
`Coordinated Universal Time` and Broiler.JS's host name in the UI culture's language. R1 and R2 agree. R5,
Broiler.JS's .NET format string, is its extension (SJL-D-17), and its answer outside the derived view's
range is SJL-6.5's to decide and list.

**X6 - extended.** Broiler.VM: `DateToFullText`, `DateToCalendarText`, `DateToClockText`, `DateToUtcText`,
`DateToIsoText`, `DatePadded`, `DateWeekDayName` and `DateMonthName`. `DateZoneText` and `DateInvalidText`
stay: the adapters use them directly. Broiler.JS: `DateString`, `ClockString`, `HumanYearString`,
`ToIsoString` and `ToTimeZoneString`'s text.

### X3 - neutral contract, all four sub-areas

Every shared member takes numbers, spans, enumerations and a provider, and answers numbers, strings and a
parse result: no engine type, no coercion, no property access. A failure is data: NaN for the
arithmetic, the invalid kind for a parse, `false` from `TryToIsoString` for an invalid date. **The parser
takes the caller's meter**, since its work grows with its input; nothing else in the area does (decision
4).

### X4 - no edge between the engines, none into Broiler.VM

The library references neither engine and nothing in this repository. Neither engine references the
other.

### X5 - boundaries hold

Every member is pure computation over its arguments and the provider. **The library reads no host zone,
clock or culture**: local time comes only through the provider the caller passes, every name and number
the renderings write is fixed text, and every comparison the parser makes is ordinal. Broiler.JS's
provider reads `TimeZoneInfo` in Broiler.JS, not in the library. This family's compiler does not call the
area.

### X7 - owners accept

The JavaScript profile owner and the Broiler.JS maintainer accept all four sub-areas and file SJL-D-19;
one person holds both roles (EX-30).

## Local time, and decision SJL-D-19

**Local time is not one of the four sub-areas.** This family has none: its local time is UTC by
omission, with no `LocalTime`, `UTC` or offset member anywhere (correspondence, the frame and section 3).
So X1 does not hold for it, and it moves as an adoption, by the decision the programme took on
2026-10-11 and this record files:

**SJL-D-19 - host policy as provider interfaces.** The shared libraries choose no host policy. They
define provider interfaces, and each engine supplies its own policy. For SJL-6 that is **the local-time
provider**: the library defines it and the specification's `LocalTime` and `UTC` over it, with UTC's
disambiguation - a skipped local time read at the offset before its transition, a repeated one at the
earlier instant. **This family's provider is UTC**, an offset of zero with no transition, which is what
the profile does today; JSD-0053 section 7 and JSD-0027 state that policy, and both are Proposed, so this
record keeps the behaviour and decides nothing about the policy itself. **Broiler.JS's provider wraps the
host's `TimeZoneInfo`**, read at each instant with its daylight-saving time, until SJL-8 gives both engines
the shared IANA data (SJL-D-20). The decision's other providers - the clock, the default locale, the
available locales - are filed here as decided and are defined by the milestones that first need them
(SJL-9 and SJL-10).

**What the provider corrects, all in Broiler.JS**: L2, one offset per instant, where today storage and
the components constructor use the zone's base offset and `toString` the DST-aware one; L3, UTC's
disambiguation, and the offset after a transition; and L4, the offset the zone's rules give at an instant
outside `DateTimeOffset`'s range, where today it is the offset in force at the moment of the call. L5, the
hour `Date` and `Temporal` disagree on, follows from L2.

## The design's readings

Each is corrected in `areas/date.md` beside the reading, which is quoted there, and in the milestone
where it repeats one.

- **`DaysFromCivil`, `MonthLength` and `Weekday` are `JsZone`'s**, in `JsTimeZones.cs`, not `JsTimeZones`'.
- **This family's UTC is the profile's behaviour, stated in Proposed records**: "Local time is UTC by
  decision (JSD-0053 §7)" overstates JSD-0053's standing.
- **`DateParseText` charges the length plus one**, through `JsEngine.Charge`.
- **This family's fallback** reads its own renderings for years 0 and above only (P9), accepts many shapes
  that are not its own (P7), and is never reached by a string that starts with four digits.
- **The other Gregorian copies** here include the inline month table of `DateMonthFromTime`,
  `ValidMonthDay` and `IsValidIsoDate`.
- **Broiler.JS's getters and setters** are mixed: five setter paths and every rendering use `JSDateMath`
  on any date, twelve setters use `DateTimeOffset` arithmetic only, and `getTimezoneOffset` has no raw path.
- **`DateParser` is public, but `Parse` is internal**, and its `N/N/N` heuristic runs first and alone.
  `TryParseExtendedIso` and `DaysInMonth` are members of `JSDate`, in `JSDateStatic.cs`, and the renderings
  are in `JSDatePrototype.Formatters.cs`, a file of `JSDate`.
- **The four rows SJL-4.5 corrected**: `TimeClip(-0)`, `setUTCHours(1.5)`, fractional setter arguments and
  large integer setter arguments now agree in both engines.
- **Date-only strings are local in Broiler.JS only for years 0001 to 9999** (P1); **its fractions** take zero
  to nine digits (P4); and **this family's fallback** is not "refused unless in its own textual forms" (P7).
- **The storage's derived views** include structured cloning, the public `Value` setter, the .NET format
  string of `toLocale*` and the CLR converters.
- **The deletion list's `Setters.CoercedIntValue`** no longer exists, and **its `DaysInYear`, `AddDays` and
  `BalanceYearMonth`** have no target in the design's `IsoCalendarMath` (above).
- **Broiler.JS's order of grammars** is strict, its own, then this family's (above), not both shared ones
  first.
- **Broiler.JS's `Date` and `Temporal` keep reading different zone data** while its provider wraps the
  host's `TimeZoneInfo`, since its `Temporal` reads its bundled tzdata; the milestone's "Either way" holds
  only once SJL-8 gives the provider the shared IANA data.

## Decision

1. **All four sub-areas move to `Broiler.ECMAScript.Algorithms`**, under a namespace of their own,
   `Broiler.ECMAScript.Time`, at milestone SJL-6, in the shape the design's section 4 gives and this record
   narrows and extends: `EcmaTime`, with `MakeFullYear`; `IsoCalendarMath`, with `DaysInYear`,
   `BalanceYearMonth` and `IsValidIsoDate`; the local-time provider with the specification's `LocalTime`
   and `UTC`; `EcmaDateParser` with its strict dialect and Broiler.VM's; and `EcmaDateFormatter`.
2. **Decision SJL-D-19 is filed** as section "Local time, and decision SJL-D-19" states it: provider
   interfaces, each engine supplying its own; this family's local-time provider is UTC, and Broiler.JS's
   the host's.
3. **The engines keep** argument conversion and its order, storage, the `RangeError` of `toISOString`,
   `toJSON`'s object protocol, `toLocale*`'s routing, the provider instance, the dialect and the order of
   grammars, Broiler.JS's culture-dependent fallback, Temporal's and Intl's code around the civil helpers,
   and charging.
4. **This family's charging units are kept.** `Date.parse` and the string constructor charge `length + 1`
   at entry, before any character is read, whatever the outcome. The shared parser takes an optional
   `WorkCharge` and charges the text's length before reading it, when the text is not empty; this family
   passes its meter and charges the extra one in the adapter, the split JSD-0065 (decision 2) made for
   number parsing, so every total and every exhaustion's point is what it is today. Nothing else in the area
   charges today, and no shared member adds a charge. **So there is no class C2 in SJL-6's rules**: an
   exhaustion verdict that moves on fuel is a regression. Broiler.JS passes no meter.
5. **Corrections** are fixes under decision SJL-D-16 (a). This family's - T4, G3 and P9 - are listed in the
   cut-over's record (SJL-6.4). Broiler.JS's - T3, T4, G2, L2 to L4, P1, P9 and R3 - are listed in its
   changelog at SJL-6.5, with **D1 to D5, which are fixed in Broiler.JS's own storage**, not in shared code.
   The design's three Broiler.JS corrections that SJL-4.5 already made are not SJL-6's, and the
   milestone's list is corrected beside them.
6. **Public Broiler.JS types (SJL-D-25).** `DateParser` keeps its type and has no public member to
   forward. `JSDate`'s public constructor from a `DateTimeOffset`, its `Value` and `DateTime` properties,
   its unused public `epoch`, `JSValue.CreateDate` and `ClrProxy.Marshal`'s date overloads keep their
   signatures, over the time value, as derived views (a); what each answers for a date outside 0001 to
   9999 is decided and recorded in Broiler.JS's `docs/public-api.md` at SJL-6.5.
7. **The bases** are bundle `sjl-6-001`'s in each repository, taken at the commit that carries each
   repository's rule; the rules are committed with this record. This family's test262 runs, base and
   after, are the hosted workflow's, which completes SJL-6.0's done-when.

## What this rejects, and why

- **Sharing the getters', the setters' and the constructor's drivers**: their argument conversion and its
  order, and the storage, touch engine objects.
- **Sharing Broiler.JS's culture-dependent parse** (SJL-D-17), and calling this family's fallback before it
  (section "The Date string grammar").
- **Sharing Temporal's grammar and limits, Intl's formatting and the zone data**: SJL-10, SJL-9 and SJL-8
  own them.
- **A host time zone for this family**, or a fixed one for Broiler.JS: SJL-D-19 keeps each engine's policy.

## Settling condition

This record is settled when SJL-6.4's cut-over lands with the deletion list absent from this family and
every rule of bundle `sjl-6-001` judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record: the corrections it names are applied in the
cut-overs. `areas/date.md`'s readings are corrected beside each, quoting it, and so are the milestone's
where they repeat one.

## What this record does not decide

- **Either cut-over.** SJL-6.4 and SJL-6.5 apply this record; each repository's rule judges its own.
- **The vectors.** SJL-6.2 adds them, and they may falsify this record (below).
- **The time-zone dataset** (SJL-D-20, SJL-8), and **this family's UTC policy** itself, which JSD-0053 and
  JSD-0027 state and leave Proposed.
- **The clock and locale providers** SJL-D-19 also names.
- **A speed claim of any kind.** SJL-6 measures nothing.

## Falsified if

- SJL-6.2's vectors find a difference between the engines, or between an engine and the specification,
  that the correspondence does not classify.
- A shared operation cannot charge this family's units without changing a fuel-parity twin's or probe's
  verdict (part B of the rule).
- The local-time provider cannot carry the specification's disambiguation over the host's `TimeZoneInfo`,
  so that Broiler.JS's L2 or L3 cannot be corrected through it.
