<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0073 - The profile on the shared Date algorithms: SJL-6.4's cut-over

**Status:** Decided 2026-10-15. It applies
[JSD-0072](0072-date-algorithms-move-to-the-shared-algorithms-library.md), the area's verdict. **The code
landed one change before this record**, in `31f405b`, so that the vector run this record cites could be
taken at a clean commit of the code it judges; this record and the run are the change after it.

**Date:** 2026-10-15.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list and rule I5's register, and the release owner, for the notices rows. **All three roles are held
by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-6, work package SJL-6.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-6-date.md)). SJL-6 started
before SJL-3 was met, and SJL-6 may not meet its gate before SJL-3 meets SJL-3's; the ledger records both.

---

## Context

JSD-0072 accepted the Date area's four sub-areas - the time arithmetic, the civil-day helpers, the Date
string grammar and the Date string renderings - for `Broiler.ECMAScript.Algorithms`, and filed decision
SJL-D-19, which makes local time a provider each engine supplies. It changed nothing in this profile. This
record is the change itself: the family references the package version that carries the area, `Date`, the
civil-day arithmetic of `Temporal`, of the zone rules and of `Intl`'s field reading, and Temporal's
date grammar call the library, and the members that computed them are deleted. The evidence that judges
it - test262 in both forms, the fuel-parity twins and the Date probes, the closures and the packages - is
bundle [`sjl-6-001`](../../../../docs/evidence/sjl-6-001/README.md), under the rule SJL-6.1 committed before
any of this code existed.

## Decision

### 1. References

`Broiler.VM.Profile.JavaScript` and `Broiler.VM.Profile.JavaScript.Compiler` reference
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`, the shared stream's next preview, packed from
Broiler.JS `e04874d069f10a3f71bbcdd66e497a5b37069368` and restored from the committed feed `eng/packages/`
with lock files. It is not on nuget.org at this record. Rule I1 admits one version of a package, so the
allow-list's entry, the feed, both lock files and rule I1's witness moved to it together, and
`0.1.0-preview.3` left the feed, in the change before the cut-over (`5bab3c0`); `packages.md` names the
version ([JSC-306](../roadmap.corrections.md#jsc-306)). The package baseline stays bundle
[JS-10-006](../evidence/js-10-006/README.md)'s candidate pack, which declares `0.2.0-preview.1`, a version
rule N37 still accepts because it is above the one the allow-list pins.

### 2. The provider, and where it goes

**This profile's local-time provider is `JsRealm.DateZone`**, the library's `FixedOffsetLocalTimeProvider`
with an offset of zero, named `Coordinated Universal Time`: the policy JSD-0053 section 7 states and
JSD-0072 keeps (SJL-D-19). **It goes where the library takes one**: the renderings, whose offset and zone
name it supplies, and a parse whose result is local time, which `EcmaLocalTime.Utc` reads in it before
`TimeClip`.

**The getters, the setters and the constructor from components keep `LocalTime` and `UTC` as the
identity**, with one body for each local and UTC pair, as before. Under a zero offset with no transition
both conversions answer their argument for every finite time, so calling them would compute nothing, and
two bodies for one computation could drift where one cannot - the reason the class's remark has always
given. A provider other than UTC, which only a decision on JSD-0053's policy could bring, would split each
pair and route its local half through `EcmaLocalTime`.

### 3. Adapters

Each calls the library and keeps the conversion of its arguments and its order, the errors, the charge
and the storage, and computes no field, day count, leap year, rendering or parse itself:

| Adapter | What it keeps | What it calls |
|---|---|---|
| The getters (`DateFieldOf`), `getYear` | The receiver test; NaN for an invalid date | `EcmaTime`'s field operations |
| `DateSetTimeFields`, `DateSetDateFields`, `setYear`, `setTime` | Every argument's conversion, in order, before the invalid date's test; `setFullYear`'s and `setYear`'s epoch for an invalid date; the store | `EcmaTime.MakeTime`, `MakeDay`, `MakeDate`, `TimeClip`; `MakeFullYear` for `setYear` |
| The constructor and `Date.UTC` (`DateFromFields`, `DateTimeValueOf`) | Each component's conversion, in order; a Date's time value copied directly; `ToPrimitive` | `EcmaTime.MakeFullYear`, `MakeDay`, `MakeTime`, `MakeDate`, `TimeClip`; `DateParseText` for a string |
| `Date.parse` and the string constructor (`DateParseText`) | The charge's first unit, before the shared parser's; a local result read in `DateZone` | `EcmaDateParser.Parse` in `DateParseDialect.BroilerVM` with the engine's `WorkCharge`; `EcmaLocalTime.Utc`; `EcmaTime.TimeClip` |
| `toString` and `Date()`; `toDateString`; `toTimeString` (`DateTimeText`); `toUTCString`; `toISOString`; `toLocaleString`, `toLocaleDateString` and `toLocaleTimeString` without Intl | `toISOString`'s `RangeError`; `toLocale*`'s routing to ECMA-402 when Intl is built, unchanged; `DateInvalidText` for an invalid date | `EcmaDateFormatter.ToDateString`, `DateString`, `TimeString` with `TimeZoneString`, `ToUtcString`, `TryToIsoString`, over `EcmaLocalTime.LocalTime` in `DateZone` |
| `Intl`'s `ClippedTime` | Its conversion | `EcmaTime.TimeClip` |
| `JsTemporalCore.AddDays`, one line; `DateTimeWithinLimits`, `UtcEpochNs`, `FromEpochNs`, `DaysWithinRange`; the ISO calendar, the non-ISO calendars' ISO conversions, the durations and the plain types | Temporal's limits, balancing of time and regulation, which are SJL-10's; each unpacks `JsIsoDate`, which gains a constructor from the library's tuple | `IsoCalendarMath` |
| `JsZone`'s rule arithmetic (`Day`, `MondayFirst`) | The zone rules' Monday-0 weekday (JSD-0072, G4) | `IsoCalendarMath.EpochDays`, `DaysInMonth`; `EcmaTime.WeekDay` |
| `JsLocalTime.From` | The record it fills, and the offset it adds | `EcmaTime`'s field operations over the local time |
| `JsTemporalParser`'s `DateSpec` and `DateSpecMonthDay` | The early error, where the date matched and is not a date | `IsoCalendarMath.IsValidIsoDate`, in the year read, or in the proposal's reference year 1972, a leap year, for a month and day |

**Charging.** JSD-0072 decision 4 holds: `Date.parse` and the string constructor charge `length + 1` at
entry, before any character is read, whatever the outcome. The adapter charges one unit through
`JsEngine.Charge` and passes the engine's meter, which the shared parser charges with the text's length
before reading it when the text is not empty: the split JSD-0065 decision 2 made for number parsing, so
every total and every exhaustion's point is what it was. Nothing else in the area charged, and nothing
does. The fuel-parity twins and the three Date probes of the bundle's part B give the base's verdict at
their base figure and at one less, in both forms, and complete with the base's answer, checked once at this
change; part B's after run is SJL-6.6's.

### 4. Deleted

The 53 members JSD-0072's deletion lists name are deleted and registered in rule I5's
`retired-members.json`, each with what replaced it:

- **`JsRealm`'s arithmetic**: `DateFloorMod`, `DateDay`, `DateTimeWithinDay`, `DateDaysInYear`,
  `DateDayFromYear`, `DateTimeFromYear`, `DateYearFromTime`, `DateInLeapYear`, `DateDayWithinYear`,
  `DateMonthStartDay` (the month table of `DateMonthFromTime`), `DateMonthFromTime`, `DateDateFromTime`,
  `DateWeekDay`, `DateHourFromTime`, `DateMinFromTime`, `DateSecFromTime`, `DateMsFromTime`,
  `DateMakeTime`, `DateMakeDay`, `DateMakeDate` and `DateTimeClip`, and the constants `DateMsPerSecond`,
  `DateMsPerMinute`, `DateMsPerHour`, `DateMsPerDay` and `DateMaxTimeValue`;
- **its grammar**: `DateParseTextual`, `DateMonthByName`, `DateWholeNumber` and `DateReadDigits`;
- **its renderings**: `DatePadded`, `DateWeekDayName`, `DateMonthName`, `DateToCalendarText`,
  `DateToClockText`, `DateToFullText`, `DateToUtcText` and `DateToIsoText`, and `DateZoneText`
  (below);
- **`JsTemporalCore`'s** `IsLeapYear`, `DaysInMonth`, `DaysInYear`, `EpochDays` (both overloads),
  `FromEpochDays`, `IsValidIsoDate`, `BalanceYearMonth`, `DayOfWeek`, `DayOfYear` and `WeekOfYear`;
- **`JsZone`'s** `DaysFromCivil`, `MonthLength` and `Weekday`;
- **`JsTemporalParser.ValidMonthDay`**.

The inline MakeFullYear of `DateFromFields` and `setYear`, the civil arithmetic and month table of
`JsLocalTime.From`, and the three stale comments around the old parser were not members, and are gone.
Rule I5's test now also requires `JsRealm.Date.cs`, `JsTimeZones.cs` and `JsTemporalParser.cs` among the
files it reads, so the register's new names cannot pass by their files going unread.

**What stays, and holds no algorithm:** the adapters of section 3, `DateObject`, and `DateInvalidText`,
now the library's `EcmaDateFormatter.InvalidDate`. And two things JSD-0072's lists treat otherwise, each
a departure that record's text does not state, which is not edited:

- **`DateZoneText` is deleted**, where JSD-0072's renderings X6 says it stays because the adapters use it
  directly. The formatter writes the zone's text from the provider, so the constant would restate the
  provider's offset and name a second time, and no adapter uses it; `DateZone` replaces it.
- **`MonthDayFromFields` keeps a reduction**, corrected, where JSD-0072's civil-day X6 lists the reduction
  for deletion. The proposal's year is whatever integer the field's conversion gives, up to the largest
  finite Number, and `RegulateDate` reaches the library through a `long` year, which no year past 2^63
  converts to. A year past 1e9 is reduced to its remainder modulo 400, which is exact in floating point
  and keeps the year's leap year, which is all a month and a day read of it. The defect G3 names was the
  base, 1972, which is not a multiple of 400; the remainder computes no calendar.

**Not this cut-over's**, as JSD-0072 says: Temporal's limits, `BalanceTime` and `PadYear` (SJL-10);
`JsZone.YearOf` and the non-ISO calendars (SJL-8); and the zone and calendar compilers' own civil-day
arithmetic in the architecture tests, which are test code outside the family's product sources.

### 5. Corrections

This family's three corrections, T4, G3 and P9, under decision SJL-D-16 (a). At this family's pin of the
shared corpus, which moves to revision 16 with this cut-over (Broiler.JS `6f29deb`), every vector matches
(50,727 of 50,727), and each of the sixteen vectors that carried a `divergence.broiler-vm` field reports
"divergence no longer holds". Revision 16's twelve vectors of strings outside the format, whose expectation
for this profile is its dialect as the library ports it, match. The run and its negative control are
retained in the bundle's [`vectors/`](../../../../docs/evidence/sjl-6-001/vectors/), at `31f405b`. A
local run of test262's Date, Temporal and Intl date directories at `5bab3c0` and at `31f405b` gave the same
verdict on every test, checked once at this change; part A's hosted runs are SJL-6.6's.

| Correction | What this family did | Vectors |
|---|---|---|
| T4 | `MakeDay` took a month at or past 2^53 modulo 12 in floating point, so `Date.UTC(-833333333333333333, 1e19, 1)` was January of year 0; the month's modulo and carry are now exact, and it is May | `date/make-day/0863` |
| G3 | `Temporal.PlainMonthDay.from` reduced a year past 1e9 to `1972 + year % 400`, so February 29 of 1000000100 was admitted under `reject` and of 1000000128 refused; the reduction keeps the year's leap year (section 4) | `date/month-day/0021`, `0022`, `0025`, `0026`, `0037`, `0038`, `0041`, `0042` |
| P9 | `Date.parse` read no sign in its textual forms, so the renderings of negative years gave NaN; the dialect reads a sign and digits as a year, and every rendering reads back | `date/round-trip/0001`, `0005`, `0007`, `0009`, `0011`, `0013`, `0015` |

Two differences of the old arithmetic that the library's reference-arm comparison found (bundle
`sjl-6-001` in Broiler.JS, SJL-6.3) are corrected with it and are not observable: `DateDay(-0)` answered
-0, and the inline MakeFullYear kept a fractional year. Every caller clips or truncates after them.

### 6. Records

- **Notices.** `THIRD_PARTY_NOTICES.md`'s row for the package names `0.1.0-preview.4` since the change
  before the cut-over, with Howard Hinnant's civil-day formulas, which their author dedicated to the public
  domain and the library credits. The package carries no other third-party component beyond the two the
  row already names, Ryu's routine and fdlibm's `expm1` and `log1p`. The row is still not confirmed, so
  release gate 12's blocker for it stands, under the same name. The vectors' row names revision 16.
- **Packages.** `packages.md` names the new version ([JSC-306](../roadmap.corrections.md#jsc-306)).
- **Assurance.** The record is regenerated. The code that left this family's assurance coverage is
  covered by the home's own assurance component in the Broiler.JS repository, as EX-125 and EX-127 say of
  the earlier areas; bundle `sjl-6-001` names it with SJL-6.6's evidence.

## Correction entries

[JSC-306](../roadmap.corrections.md#jsc-306) (`packages.md`, the shared package's version).

## The milestone's readings

- "`JsLocalTime.From` in the date-time formatter", listed among the deleted: its civil arithmetic is
  deleted, as JSD-0072's X6 reads it, and the member stays as the adapter that fills the formatter's
  record from the shared field operations.
- "`DateParseText` keeps the charge and becomes a call": it does, and it also reads a local result in the
  provider (section 2).
- "**Unchanged:** the provider Broiler.VM passes is UTC, under JSD-0053 §7": it is; JSD-0053 is
  Proposed, as JSD-0072 says, and this record decides nothing about that policy.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-6 is met.** Its evidence is bundle `sjl-6-001`, read under its rules at SJL-6.6, and the
  ledger records the result.
- **Broiler.JS's cut-over**, SJL-6.5, which its own changelog records, nor the corpus revision that
  retires this family's sixteen divergence fields.
- **Publishing `0.1.0-preview.4`** to nuget.org, which is the owner's to choose.
- **This family's UTC policy** (JSD-0053, JSD-0027).

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 3 computes a field, a day count, a leap year, a rendering or a parse itself;
- a vector that matched before this change does not after it, a fuel-parity twin or Date probe completes
  at a different allowance or with a different answer, an exhaustion verdict moves, or a test262 verdict
  moves other than by T4, G3 or P9, without a dated decision admitting it.
