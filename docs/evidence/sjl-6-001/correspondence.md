<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-6.1's correspondence from code: the Date algorithms in both engines

**Written:** 2026-10-15, at SJL-6.1, as the input of
[JSD-0072](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0072-date-algorithms-move-to-the-shared-algorithms-library.md)'s
X1, X2 and X6, before any code of SJL-6 exists. **Read at** Broiler.VM `9c3d809`, whose product sources
the default branch after SJL-5's merges carries unchanged (`310c601`), and Broiler.JS `bc3597f`, its default
branch after SJL-5's merges, whose sources under `Broiler.JS/` and `Broiler.ECMAScript/` are those of
bundle `sjl-5-001`'s after commit `1ebde70`. **By reading, and by running where a row says so.** A
Broiler.VM cell marked *(run)* was run through this repository's command-line root built in Release from
those sources; a Broiler.JS cell marked *(run)* was run through Broiler.JS's shell, `BroilerJS.dll
--script-host`, built in Release from `bc3597f`, in the invariant culture and in the zone the row names
(`TZ=UTC` unless it says New York, `America/New_York`). Where a behaviour depends on the .NET runtime's
documented behaviour it says so. SJL-6.2's vectors confirm or refute each row. **It holds no figure of a
run**: the values below are inputs and the answers they give, not counts.

**Paths.** `VM/` is `src/Broiler.VM.Profile.JavaScript/` in Broiler.VM and `D/` its `docs/decisions/`.
`BI/`, `RT/` and `CL/` are `Broiler.JS/Broiler.JavaScript.BuiltIns/`, `.Runtime/` and `.Clr/` in
Broiler.JS, and `SL/` is `Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/` there. A line number is the
line at the revision above; a bare `:n` is in the file named last.

**Three facts of the checkout frame every section.**

- **Neither engine's Date code is shared yet**, beyond SJL-4's `IntegerConversions.ToIntegerOrInfinity`,
  which both call for the integral conversions of `MakeTime`, `MakeDay`, `TimeClip` and the setters
  (VM/JsRealm.Date.cs:20, 232, 452, 816-819, 837-839, 878; BI/Date/JSDateMath.cs:120-122, 156-159, 210 and
  `IntegerComponent` BI/Date/JSDatePrototype.Setters.cs:16). The library has no time or calendar member:
  its namespaces are `Broiler.ECMAScript`, `.Numbers` and `.Text`.
- **This family's local time is UTC, by omission.** No `LocalTime`, `UTC` or `LocalTZA` member exists:
  the getters read the time value, the constructor from components has no UTC step (VM/JsRealm.Date.cs:462),
  and both parsers default the offset to 0 (:942, :1075). The policy is stated in
  [JSD-0053](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0053-time-zone-data-and-temporal-admission.md)
  (D/0053…:142-143, "The local time zone stays UTC") and before it in JSD-0027 (:86-87, :189); **both
  records are Proposed**, and JSD-0053's only decided part is its tzdb archive (:6-14). The class remark at
  VM/JsRealm.Date.cs:39-44 gives the reason: a host zone would make a benchmark's figures depend on where
  it ran.
- **Broiler.JS stores a `DateTimeOffset` and a raw double.** `JSDate.value` (BI/Date/JSDate.cs:27) holds
  dates in 0001 to 9999, at an offset; `rawTimeMs` (:34) holds the rest; `InvalidDate`, the invalid
  sentinel, is `DateTimeOffset.MinValue` (:16), which is also the real instant 0001-01-01T00:00Z.
  `SetTimeValue` (:81-115) stores an in-range time value **at the zone's base offset**,
  `FromUnixTimeMilliseconds(ms).ToOffset(Local)` (:99), where `Local` is `TimeZoneInfo.Local.BaseUtcOffset`
  (:25), which ignores daylight-saving time.

---

## 1. Time arithmetic

| Responsibility (21.4.1) | Broiler.VM | Broiler.JS |
|---|---|---|
| Day, TimeWithinDay, the floor modulo | `DateDay` VM/JsRealm.Date.cs:610, `DateTimeWithinDay` :615, `DateFloorMod` :605 | `Day` BI/Date/JSDateMath.cs:44, `TimeWithinDay` :47, `FloorDiv` :15, `FloorMod` :18-22 |
| DaysInYear, DayFromYear, TimeFromYear, the leap year | `DateDaysInYear` :620-633, `DateDayFromYear` :638-642, `DateTimeFromYear` :647 | `DaysInYear` :41 and `TimeFromYear` :28, both unused; `DayFromYear` :25; `IsLeapYear` :31-38 |
| YearFromTime, InLeapYear, DayWithinYear | `DateYearFromTime` :657-672 (an estimate, then two correcting loops), `DateInLeapYear` :677-678, `DateDayWithinYear` :683-684 | `YearFromTime` :50-61 (the same shape); InLeapYear and DayWithinYear inline at :70-71, :86-88, :139 |
| MonthFromTime, DateFromTime | `DateMonthStartDay` :689-703 and a second, inline month table in `DateMonthFromTime` :708-764; `DateDateFromTime` :769-772 | :67-80 and :83-91, over the tables at :63-64 |
| WeekDay, HourFromTime and its siblings | `DateWeekDay` :780 (Sunday 0); `DateHourFromTime`, `DateMinFromTime`, `DateSecFromTime`, `DateMsFromTime` :785-803 | `WeekDay` :94; :97, :100, :103, :106 |
| MakeTime | `DateMakeTime` :808-820: NaN for a non-finite argument, then `ToIntegerOrInfinity` on each | `MakeTime` :148-161: the same |
| MakeDay | `DateMakeDay` :830-851: `ToIntegerOrInfinity` on each; NaN for a non-finite year and month sum; no range step, every caller clipping (:462, :541-542, :587-588, :236, :1044, :1162) | `MakeDay` :112-142: `ToIntegerOrInfinity` on each; NaN past 1e15 (:131-133); then `(long)` casts (:135-136) |
| MakeDate, TimeClip | `DateMakeDate` :856-864; `DateTimeClip` :869-879, which folds -0 to +0 | `MakeDate` :145; `TimeClip` :200-211, which folds -0 to +0 |
| MakeFullYear | no member: inline in `DateFromFields` :450-460 and `setYear` :226-233 | no member: inline at BI/Date/JSDateStatic.cs:68, BI/Date/JSDatePrototype.cs:140 and BI/Date/JSDatePrototype.Setters.cs:390-392, and dead at JSDateStatic.cs:240-242 |
| The constructor, `Date.UTC`, `Date.now` | VM/JsRealm.Date.cs:90-118 with `DateTimeValueOf` :416-430 and `DateFromFields` :438-463; `Date.UTC` :125; `Date.now` :123, `DateCurrentTime` :381-382 | BI/Date/JSDatePrototype.cs:20-195 (components through `Get7Double` RT/Arguments.cs:439-463, then a `DateTimeOffset` at the base offset :169-177, or `JSDateMath` stored raw :145-155, :181-194); `Date.UTC` BI/Date/JSDateStatic.cs:39-75; `Date.now` :77-82 |
| The getters | eight local-and-UTC pairs over `DateFieldOf` :387-409, one delegate under both names (`DatePair` :316-320); `getTimezoneOffset` :172-176 (NaN or +0); `getYear` :214-218 | BI/Date/JSDatePrototype.Getters.cs: the local eight read the stored `DateTimeOffset`'s fields at the offset it was stored with, or `JSDateMath` over `LocalTime` for a raw date (:10-193); the UTC eight `value.ToUniversalTime()` or `JSDateMath` over the raw value (:214-316); `getTimezoneOffset` :204-212 |
| The setters | `setTime` :150-156; `DateSetDateFields` :551-592 and `DateSetTimeFields` :504-546 for each local-and-UTC pair (`DateSetterPair` :322-326), with `DateFieldSupplied` :477-481 and `DateFieldArgument` :486-498; `setYear` :220-239 | BI/Date/JSDatePrototype.Setters.cs: twelve over `DateTimeOffset` arithmetic (setDate :36-53, setHours :175-219, setMilliseconds :221-240, setMinutes :242-280, setMonth :282-321, setSeconds :323-358, setFullYear in 1 to 9999 :134-160, and the UTC date, millisecond, minute, month and second setters :423-443, :522-545, :547-589, :591-632, :634-673); four over `JSDateMath` (setTime :361-369, setUTCFullYear :445-483, setUTCHours :485-520, setFullYear outside 1 to 9999 :69-85, :162-172); `setYear` :371-421; arguments through `IntegerComponent` :16 and `Months` :20-21 |

**Consumers of the arithmetic outside `Date`.** Broiler.VM: `DateTimeClip` in Intl's `ClippedTime`
(VM/JsRealm.DateTimeFormat.cs:295-305), and `DateCurrentTime` in the same file (:344) and in `Temporal.Now`
(VM/JsRealm.TemporalInstant.cs:267-268). Broiler.JS: Intl reads `JSDateMath` in about a dozen places
(BI/Intl/JSIntl.cs:6653, 7035, 7128, 7308, 7554; BI/Intl/JSIntlDateTimeFormatEngine.cs:35-41, 233-234, 646,
657).

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| T1 | `new Date(-0)`, `setTime(-0.5)` | TimeClip folds -0 to +0 | +0 *(run)* | +0 *(run)*, since SJL-4.5 | none |
| T2 | `setUTCHours(1.5)`, fractional and large setter arguments | MakeTime and MakeDay apply ToIntegerOrInfinity | truncated, no wrap *(run)* | truncated, no wrap *(run)*, since SJL-4.5 | none |
| T3 | a setter's argument order: the time value read first, then each present argument converted in order, the date written last | 21.4.4.20 and its siblings | as specified *(run)*: a `valueOf` that moves the date does not change the result | as specified *(run)*, except that `IsValid` (BI/Date/JSDatePrototype.Getters.cs:38-64) writes the invalid sentinel as soon as the first argument is `undefined` or NaN, before the later arguments are converted, so a later argument's `valueOf` reads `getTime()` as NaN (worked from the code) | correction of Broiler.JS |
| T4 | `Date.UTC(-833333333333333333, 1e19, 1)`, a month at or past 2^53 | `mn = 𝔽(ℝ(m) modulo 12)`, exact: year 0, May 1, -62156764800000 | -62167219200000, January *(run)*: the modulo is taken in floating point (`DateFloorMod`), where 1e19 - 12 × ⌊1e19 / 12⌋ rounds to 0 | -62148816000000, August *(run)*: `(long)month` saturates (:136) | correction of both |
| T5 | a year so large that no time value has it, `MakeDay` alone | NaN | a number, which every caller's TimeClip makes NaN | NaN past 1e15 | none: not observable, since every caller clips |
| T6 | `Date.UTC(99)`, `Date.UTC(-0.5)`, a year 0 to 99 through the constructor and `setYear` | MakeFullYear | 1999, 1900 *(run)* | as specified, inline | none |

**Date's storage in Broiler.JS.** The storage rows are this engine's own, not shared; the cut-over
makes the time value its only storage (milestone SJL-6.5), and each row below is a correction of
this engine's own code that the storage change makes, under decision SJL-D-16 (a).

| # | Input | Specification | Broiler.JS | Class |
|---|---|---|---|---|
| D1 | a group-one setter whose result is a valid date outside 0001 to 9999: `new Date(9999,11,31).setDate(32)`, `setUTCDate(32)`, `setUTCMonth(-1)` from 0001-01, `setHours(24*365*20000)` | that date | NaN *(run)*: the `DateTimeOffset` arithmetic throws and the catch stores the sentinel | correction of Broiler.JS |
| D2 | a group-one setter on a date stored raw, outside 0001 to 9999 or in range but stored raw by the components path: `new Date(-1e15).setHours(1)`, `new Date('0000-06-15').setHours(1)`, `new Date(2021,0,1,0,0,0,2**31).setHours(1)` | the new date | NaN, and the date unchanged *(run)* | correction of Broiler.JS |
| D3 | a result exactly at 0001-01-01T00:00Z: from 0001-01-01T00:00:00.001Z, `setUTCMilliseconds(0)` | that instant, -62135596800000 | NaN *(run)*: the instant is the sentinel | correction of Broiler.JS |
| D4 | `getTimezoneOffset` on a valid date stored raw | the offset | NaN *(run)*: it has no raw path (:204-212) | correction of Broiler.JS |
| D5 | `structuredClone(new Date(-1e15))` | the same time value | NaN *(run)*: the clone copies only `value` (BI/BuiltInsAssemblyInitializer.cs:253-258) | correction of Broiler.JS |

## 2. Civil-day helpers

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| Leap year, month length, days in year | `JsTemporalCore.IsLeapYear` VM/JsTemporalCore.cs:179, `DaysInMonth` :184-189, `DaysInYear` :194; `JsZone.MonthLength` VM/JsTimeZones.cs:638-641 with its own leap test; `JsLocalTime.From`'s leap test and month table VM/JsDateTimeFormatter.cs:172-173; `JsTemporalParser.ValidMonthDay` VM/JsTemporalParser.cs:285-286 | `IsLeapYear` and a month-length member in each of `JSTemporalInstant` BI/Temporal/JSTemporalInstant.cs:505, 507-513; `JSTemporalPlainDate` BI/Temporal/JSTemporalPlainDate.cs:799, 801-807; `JSTemporalPlainDateTime` BI/Temporal/JSTemporalPlainDateTime.cs:1166, 1168-1174; `JSTemporalPlainMonthDay` BI/Temporal/JSTemporalPlainMonthDay.cs:568, 570-576; `JSTemporalPlainYearMonth` BI/Temporal/JSTemporalPlainYearMonth.cs:762, 764-770; `JSTemporalZonedDateTime` BI/Temporal/JSTemporalZonedDateTime.cs:2004, 2006-2012; `TemporalIsoString` BI/Temporal/TemporalIsoString.cs:322, 324-330; `TemporalArithmeticCalendar.GregorianLeap` BI/Temporal/TemporalArithmeticCalendar.cs:321; `JSDate.DaysInMonth` BI/Date/JSDateStatic.cs:205-220 with an inline leap test :210 |
| Epoch days in both directions | Hinnant's `days_from_civil` and `civil_from_days` over `long`: `JsTemporalCore.EpochDays` :199-207 (and its overload :212), `FromEpochDays` :217-228; `JsZone.DaysFromCivil` VM/JsTimeZones.cs:626-634; `JsLocalTime.From` inline VM/JsDateTimeFormatter.cs:160-169 | the same formulas in `JSTemporalInstant` :516-524, 526-538; `JSTemporalPlainDate` :1219-1227, 1229-1241 (wrapped by `EpochDaysFor` :1157 and `DateFromEpochDays` :1159-1163); `JSTemporalPlainDateTime` :1311-1319, 1321-1333; `JSTemporalPlainYearMonth` :811-819, 821-833; `JSTemporalZonedDateTime` :2046-2054, 2056-2068; `TemporalArithmeticCalendar` :355-363, 365-377; `TemporalNonIso` BI/Temporal/TemporalNonIso.cs:605-613, 615-627 |
| Weekday, day of year, ISO week | `JsTemporalCore.DayOfWeek` :263-267 (Monday 1 to Sunday 7), `DayOfYear` :272, `WeekOfYear` :277-302; `JsZone.Weekday` VM/JsTimeZones.cs:646 (Monday 0); `JsLocalTime.From`'s weekday (Sunday 0) and day of year VM/JsDateTimeFormatter.cs:170, 174 | `IsoDayOfWeek`, `IsoWeek` and `WeeksInIsoYear` in `JSTemporalPlainDate` :1182-1187, 1190-1203, 1205-1209, `JSTemporalPlainDateTime` :1222-1226, 1228-1236, 1238-1242 and `JSTemporalZonedDateTime` :2017-2021, 2023-2031, 2033-2037; inline days of year at `JSTemporalPlainDate` :119, `JSTemporalPlainDateTime` :150 and `JSTemporalZonedDateTime` :239; Intl's `DayOfWeek` BI/Intl/JSIntlDateTimeFormatEngine.cs:233-234 through `JSDateMath` |
| Validity, balancing, the floor division they use | `IsValidIsoDate` :233-234, `AddDays` :239, `BalanceYearMonth` :244-249 | `IsValidISODate` in `JSTemporalPlainDate` :809-816, `JSTemporalPlainDateTime` :1180-1181, `JSTemporalPlainMonthDay` :578-579, `JSTemporalPlainYearMonth` :772-773 and `JSTemporalZonedDateTime` :2014-2015; `FloorDiv` beside each civil copy (`JSTemporalInstant` :420-426, `JSTemporalPlainDate` :1211-1216, `JSTemporalPlainDateTime` :1244-1256, `JSTemporalPlainYearMonth` :804-809, `JSTemporalZonedDateTime` :2039-2044, `TemporalArithmeticCalendar` :379-384) |

**Not this sub-area, named so that none is lost.** Temporal's range limits (`JsTemporalCore`'s
`DateWithinLimits` and its siblings VM/JsTemporalCore.cs:307-333, Broiler.JS's epoch bounds beside each type)
and `BalanceTime` (:354-373) are Temporal's (SJL-10); `JsZone.YearOf` (VM/JsTimeZones.cs:371), a mean-year
estimate of a rule's year, and the test-side copies in `src/tests/Broiler.VM.Architecture.Tests/TzdbCompiler.cs`
(:186-202, :240) are the zone compiler's (SJL-8); the non-ISO calendars' .NET helpers in Broiler.JS
(`TemporalLunisolarCalendar`, `TemporalSolarHijriCalendar`) and this family's `JsCalendars.cs` are SJL-8's
calendars; `JsTemporalCore.PadYear` (:685-686) renders a year as `DateToIsoText` does, and is Temporal's.

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| G1 | leap years, month lengths, epoch days both ways, weekdays and days of year, over the years each engine's Temporal admits | ISO 8601's proleptic Gregorian calendar (Temporal's ISO calendar operations) | as specified | as specified | none |
| G2 | the ISO week of a date in a negative year: `-000002-12-28`, `-000001-01-01`, `-000008-12-28` | ISOWeekOfYear: week 53 of -2, 53 of -2, 53 of -8 | as specified: `WeekOfYear` reads weekdays through floor-correct epoch days | 1 of -1, 52 of -2, 1 of -7 *(run)*: `WeeksInIsoYear`'s `P(y)` divides with truncation, in all three copies | correction of Broiler.JS |
| G3 | `Temporal.PlainMonthDay.from({year: 1000000100, month: 2, day: 29}, {overflow: "reject"})`, and the same with `constrain`; year 1000000128 | the year's own leap-ness: RangeError, and `02-28`; `02-29` for 1000000128 | `02-29` for both, and a RangeError for 1000000128 *(run, through a build with Intl and Temporal)*: `MonthDayFromFields` reduces a year past 1e9 to `1972 + year % 400` (VM/JsTemporal.Calendar.cs:654-657), whose leap-ness differs | as specified | correction of Broiler.VM |
| G4 | the weekday convention | WeekDay: Sunday 0; ISODayOfWeek: Monday 1 to Sunday 7 | three: Date's Sunday 0, Temporal's Monday 1 to 7, `JsZone.Weekday`'s Monday 0 | two: Date's Sunday 0, Temporal's Monday 1 to 7 | none: the shared members answer the specification's two, and a zone rule maps its own |

## 3. Local time

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| LocalTime, UTC, the offset at an instant | none: UTC by omission (the frame above) | `JSDateMath.LocalTime` BI/Date/JSDateMath.cs:175 and `UTC` :168, over `GetLocalOffsetMs` :181-195: `TimeZoneInfo.Local.GetUtcOffset` at the instant within `DateTimeOffset`'s range (:187-191), DST-aware; **otherwise the offset in force now** (:193-194); and `JSDate.Local`, the base offset (BI/Date/JSDate.cs:25), for storage and the components constructor |
| Skipped and repeated local times | Temporal's `PossibleEpochNs` and `Disambiguate` (VM/JsTemporal.Zones.cs:176-227, 232-269), which Date does not reach | none for Date: `UTC(t)` reads the offset at `t` taken as a UTC instant |
| `getTimezoneOffset` | :172-176: NaN or +0 | :204-212: `-(int)GetUtcOffset(Value).TotalMinutes`, DST-aware, for a date stored as a `DateTimeOffset` |
| Every read of the host's zone or clock | `DateTimeOffset.UtcNow` (:381-382) | `BaseUtcOffset` (BI/Date/JSDate.cs:25), `GetUtcOffset` (BI/Date/JSDateMath.cs:190, :194; BI/Date/JSDatePrototype.Getters.cs:210), the zone's names (BI/Date/JSDatePrototype.Formatters.cs:282-294), `DateTimeOffset.Now` (BI/Date/JSDatePrototype.cs:17, :59; BI/Date/JSDateStatic.cs:80), `ToLocalTime()` (BI/Date/JSDatePrototype.cs:113; BI/Date/JSDatePrototype.Setters.cs:412), `LocalDateTime` (BI/Date/JSDate.cs:138), and the parser's `DateTimeKind.Local` and `AssumeLocal` (RT/DateParser.cs:132, 199-216) |

**Temporal reads other data.** Broiler.JS's Temporal takes its offsets from its bundled tzdata
(BI/Temporal/Tz/IanaTimeZoneDatabase.cs), not from the host, and reads `TimeZoneInfo.Local` only for the
identifier of `Temporal.Now`'s zone (BI/Temporal/JSTemporalNow.cs:21-38). Which data both read is decision
SJL-D-20's, SJL-8's.

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| L1 | the local time zone | implementation-defined: the host's, or a fixed one | UTC | the host's | policy: the provider each engine supplies (SJL-D-19) |
| L2 | New York: `new Date(2021,6,1)`, its `getHours()`, `getTime()` and `String()`; `new Date(Date.UTC(2021,6,1,12))`'s `getHours()` and `toString()`; a January date moved to July by `setMonth(6)` | one offset per instant, the one in force at it: 00:00 EDT, 04:00Z | not applicable (UTC) | `getHours()` 0 with `toString()` 01:00 EDT and 05:00Z; `getHours()` 7 with `toString()` 08:00; `getHours()` 12 with `toString()` 13:00 EDT *(run)*: storage and the components constructor use the base offset, `toString` and `getTimezoneOffset` the DST-aware one, and a local setter keeps whichever offset the date was stored with | correction of Broiler.JS |
| L3 | New York: a skipped local time, `new Date(2021,2,14,2,30)`; a repeated one, `new Date(2021,10,7,1,30)`; a local time just after a transition read through `UTC()`, `Date.parse('2021-03-14T03:30:00.0000')` | UTC(t) disambiguates: a skipped time is read at the offset before the transition (07:30Z, shown as 03:30 EDT); a repeated one takes the earlier instant (05:30Z); a time after the transition takes the offset after it (07:30Z) | not applicable | 07:30Z with `getHours()` 2; 06:30Z; 08:30Z *(run)*: storage and the components constructor use the base offset, and `UTC(t)` reads the offset at `t` taken as a UTC instant, which within the offset's width after a transition is the offset before it | correction of Broiler.JS |
| L4 | New York, the offset of a date outside `DateTimeOffset`'s range: `new Date(10000,0,1)`, `'+010000-01-01T00:00'`, `String(new Date(Date.UTC(-1,0,1)))` | the offset the zone's rules give at that instant | not applicable | the offset in force at the moment of the call *(run, on 2026-10-15, under EDT)*: 04:00Z for a January date, and `GMT-0400 (Eastern Daylight Time)` in year -1 | correction of Broiler.JS |
| L5 | New York: `new Date(2021,6,1,12).getHours()` against `Temporal`'s hour for the same instant in the same zone | the same hour | not applicable | 12 against 13 *(run)*: a consequence of L2; the two also read different zone data (above) | correction of Broiler.JS (L2); the data is SJL-8's |

## 4. Parsing

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| The entry and the charge | `Date.parse` VM/JsRealm.Date.cs:124 and the string constructor :428, both through `DateParseText` :901-1045, which charges `text.Length + 1` at entry through `JsEngine.Charge` (:903), before any character is read, whatever the outcome | `Date.parse` BI/Date/JSDateStatic.cs:90-104 and the string constructor BI/Date/JSDatePrototype.cs:96-114, through `DateParser.Parse` RT/DateParser.cs:180-221, then `JSDate.TryParseExtendedIso` for what it refuses; no charge |
| The Date Time String Format (21.4.1.32) | inline in `DateParseText` (:908-1044), digits through `DateReadDigits` :1214-1237: `±YYYYYY` with `-000000` refused (:908-925), else four digits (:926); `-MM`, `-DD`; `T` and `HH:mm`, `:ss`, `.sss` of exactly three digits; `Z` or `±HH:mm`; ranges at :1032-1039, a day from 1 to 31 in any month; the offset subtracted, then TimeClip (:1041-1044) | twice: the `\dT` and `DefaultFormats` lists of `DateParser` (RT/DateParser.cs:49-56, 69-84, 197-204), .NET format strings under `AssumeUniversal` or `AssumeLocal`, and `TryParseExtendedIso` BI/Date/JSDateStatic.cs:123-201, a regular expression (`EcmaIsoDateTime` :113-115) with range checks through `DaysInMonth` |
| The implementation-defined fallback | `DateParseTextual` :1067-1163, with `DateMonthByName` :1168-1183 and `DateWholeNumber` :1188-1209, reached only by a string that starts with neither a sign nor four digits (:933): it splits on space, comma and tab, stops at `(`, reads an exact-case month abbreviation, a clock of two or three all-digit parts, `GMT` or `UTC` alone or with `±hhmm` unchecked, a numeric token of three or more characters as the year and a shorter one as the day, and skips every other token | `DateParser`: `NormalizeToStringZone` and `NormalizeExtendedYear` (RT/DateParser.cs:14-47); SpiderMonkey-style `N/N/N`, first and exclusively (:93, :100-139, two-digit years 50-99 in the 1900s and 0-49 in the 2000s, local); then `SecondaryFormatsUTC` and `SecondaryFormats` (:141-178, :207-210); then `DateTime.TryParse` in **the current culture** (:213), then the invariant culture (:216), all `AssumeLocal` but the UTC list |

**Stale comments.** `DateParseText`'s says expanded years and the legacy forms are refused (VM/JsRealm.Date.cs:884,
888-892), which :908-934 contradict; `DateParseTextual`'s says it reads its own renderings (:1053-1056) and a
four-digit year (:1061), which rows P9 and P7 qualify. Both go with the code at SJL-6.4.

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| P1 | a date-only form: `"2021-03-04"`, `"2021-03"`, `"2021"`, `"+002021-03-04"`, under New York | UTC | UTC | **local** for years 0001 to 9999 *(run)*: `DefaultFormats` under `AssumeLocal`; UTC for year 0, negative years and years past 9999, through the regular expression; `"0001-01-01"` local in New York and UTC in Berlin *(run)* | correction of Broiler.JS |
| P2 | a date-time form without an offset | local time | UTC, which is this family's local time | local time | none |
| P3 | the format at its limits: `-000000`, `+000000`, `+275760-09-13T00:00:00.000Z` and 1 ms past it, `-271821-04-20T00:00:00.000Z` and 1 ms before it, `T24:00` and `T24:00:00.001` | as stated in 21.4.1.32 and TimeClip | as specified *(run)* | as specified *(run)* | none |
| P4 | a fraction of other than three digits: `.1`, `.12`, `.1234`, zero digits (`"…07.Z"`) | outside the format: implementation-defined | NaN *(run)* | one to nine digits accepted and truncated, zero digits accepted *(run)*; up to seven with a space separator | implementation-defined |
| P5 | a day past its month's length: `"2021-02-30"`, `"2021-04-31"` | outside the format ("illegal values"): implementation-defined | carried into the next month: 2021-03-02, 2021-05-01 *(run)* | NaN *(run)* | implementation-defined |
| P6 | lower case `t` and `z`; an offset without its colon, `+0100`; `"2021-03-04Z"` | outside the format: implementation-defined | NaN *(run)* | `t…z` and `t…Z` accepted, `T…z` NaN; `+0100` accepted *(run)* | implementation-defined |
| P7 | the fallback's shapes: `"Jan 1 70"`; `"Thu Jan 01 1970 00:00:00 GMT+9999"`; a weekday that does not match, `"Fri Jan 01 1970 00:00:00 GMT+0000"`; `"Feb 30 2021"`; `"1970 Jan 01"`; `"03/04/2021 10:00"` and `"04.03.2021"` | implementation-defined | year 70; 99 h 99 min east; the weekday ignored, 0; 2021-03-02; NaN, since a leading four-digit year never reaches the fallback; NaN; NaN *(run)* | 1970; NaN; NaN, since .NET refuses a weekday that does not match; NaN; 1970-01-01; March 4 and April 3 in the invariant culture, and April 3 and March 4 under the German and British ones *(run)* | implementation-defined; Broiler.JS's culture dependence stays in Broiler.JS (SJL-D-17) |
| P8 | the engine's own `toString` and `toUTCString` read back, for a year from 0 to 9999 | Date.parse's expectation (21.4.3.2): the same time value | the same *(run)* | the same *(run)* in the invariant culture; NaN west of UTC under a culture whose minus sign is not U+002D (R3) | correction of Broiler.JS (R3) |
| P9 | the same for a negative year and for a year past 9999: `String(new Date(Date.UTC(-1,0,1)))`, its `toUTCString()`, and year 10000's | the same time value | NaN for a negative year *(run)*: `DateWholeNumber` reads no sign; past 9999, the same | NaN outside 0001 to 9999 *(run)*: the .NET formats cannot hold the year | correction of both |
| P10 | the charge of `Date.parse` and of the string constructor | none: fuel is this family's | `length + 1`, at entry | none | policy: this family's units, kept (JSD-0072) |

## 5. Renderings

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| DateString, TimeString, TimeZoneString, the year's padding | `DateToCalendarText` VM/JsRealm.Date.cs:1303-1305, `DateToClockText` :1310-1313, `DatePadded` :1244-1255, `DateWeekDayName` :1260-1274, `DateMonthName` :1279-1298; the zone text the constant `DateZoneText` :83 | `DateString` BI/Date/JSDatePrototype.Formatters.cs:256-263, `ClockString` :267-272, `HumanYearString` :276-278, `ToTimeZoneString` :280-300 |
| `toString`, `toDateString`, `toTimeString`, `toUTCString` (and `toGMTString`), `toISOString`, `toJSON` | `DateToFullText` :1318-1321 (`"Invalid Date"` from `DateInvalidText` :78), `DateToUtcText` :1326-1329, `DateToIsoText` :1339-1354; the methods :186-208; `toISOString`'s RangeError :247-249; `toJSON` :257-269 | `toString` :199-213, `toDateString` :45-53, `toTimeString` :215-225, `toUTCString` :227-239, `toISOString` :55-63 over `ToIsoString` :90-117; `toJSON` :65-88, replaced by an identical closure in BI/BuiltInsAssemblyInitializer.cs:603-625; `toGMTString` :648-650 |
| `toLocale*` | :276-314: the plain renderings without Intl; with Intl, `ToLocaleDateString` (VM/JsRealm.DateTimeFormat.cs:548-552) | BI/Date/JSDatePrototype.Formatters.cs:119-171: Intl, or, with a string second argument, a .NET format string over the stored `DateTimeOffset` (`FormatWithNetCulture` :188-197) |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| R1 | every rendering at years -271821, -1, 0, 1 to 9999, 10000 and 275760 | 21.4.4.41.1 to .4, 21.4.4.36 to .43: a signed year of at least four digits; `toISOString` `±YYYYYY` outside 0 to 9999 | as specified *(run)* | as specified *(run)* | none |
| R2 | `toISOString` of an invalid date; `toJSON` of a non-finite value | RangeError; `null` | as specified | as specified | none |
| R3 | the sign in `toString`'s and `toTimeString`'s `GMT±hhmm` under the `sv_SE`, `fa_IR` and `ar_SA` cultures, New York | TimeZoneString: U+002B or U+002D | `+`, the offset being 0 | the culture's negative sign *(run)*: U+2212, U+200E U+2212, U+061C and `-`, from `$"GMT{hhmm:d4}"` (:296-299) | correction of Broiler.JS |
| R4 | the zone's name in parentheses | implementation-defined | `Coordinated Universal Time`, fixed | the host's `StandardName` or `DaylightName`, in the current UI culture's language *(run)* | implementation-defined: the provider's name |
| R5 | `toLocaleString(undefined, 'yyyy-MM-dd')` of a date outside 0001 to 9999 | (an extension) | not applicable | the sentinel's fields, `"0001-01-01"` *(run)* | Broiler.JS's extension (SJL-D-17): its answer outside the view's range is decided and listed at SJL-6.5 |

---

## Outside the area, with the milestone that owns each

- **Temporal's own grammar and limits**: `JsTemporalParser` (its `DateYear` VM/JsTemporalParser.cs:219-247
  refuses `-000000` as Date does) and Broiler.JS's `TemporalIsoString` parsing, and both engines' epoch
  limits (SJL-10).
- **Intl's formatting**: this family's date-time formatter charges its output's length
  (VM/JsDateTimeFormatter.cs:306, 324; VM/JsRealm.DateTimeFormat.cs:455), and `JsLocalTime.From` is the only
  civil copy in it; Broiler.JS's `JSIntl.cs:7162` lets `ArgumentOutOfRangeException` through for
  `formatToParts(8.64e15)` *(run)* (SJL-9).
- **Time-zone data and calendars**: `JsZone` and the zone compiler; Broiler.JS's bundled tzdata and its
  .NET calendars (SJL-8).
- **Broiler.JS's CLR interop**: `JSValueToClrConverter.To[Nullable]DateTime[Offset]`
  (RT/JSValueToClrConverter.cs:52-62) falls back to `DateTime.Parse` in the current culture when a guest
  string is converted to a CLR date. It is a conversion to a host type, not `Date.parse`, and stays.
