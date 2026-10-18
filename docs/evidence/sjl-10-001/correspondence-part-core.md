<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-10.1's correspondence from code, core half: records, balancing, durations and differences in both engines

**Written:** 2026-10-18, at SJL-10.1, as the input of JSD-0078's verdicts on four of the area's seven sub-areas (records
and limits; balancing; durations and rounding; differences) and of the area's comparison rules, before any code of
SJL-10 exists. The other half (the grammar, calendars, serialization) is written beside it, and a difference that
touches those is named here in one line and left to it. **Read at** Broiler.VM `14072b0`, whose product sources are
`047dcec`'s, the default branch after SJL-8; and Broiler.JS `d3464413`, its default branch after SJL-8.7. The shared
library is read in Broiler.JS at `d3464413`; every file of it cited here is byte-identical at `a04cd447`, the commit
Broiler.VM's package `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5` was packed from
(`eng/packages/Broiler.ECMAScript.Algorithms.0.1.0-preview.5.pin:54`). The proposal is
`tc39/proposal-temporal@e8cc03fc`, JSD-0053's pin, read in its `spec/` and `polyfill/lib/`. **By reading, and by
running where a row says so.**
- A Broiler.VM cell marked *(run)* was run from the Release build of the checkout through the conformance root, as a
  raw test262 test in a test262-shaped directory (`runs/vm262/`), under the manifest `broiler.javascript.wide`. The
  harness files of test262 at the common pin `ccaac100` were copied into it for the run and are not retained; a raw
  test reads none of them.
- A Broiler.JS cell marked *(run)* was run through the shell, `BroilerJS.dll --script-host`, built in Release, on .NET
  10.0.12, with no `LANG` and `TZ=UTC`, on Ubuntu 24.04.5.
- **The reference** is the proposal's polyfill at `e8cc03fc`, run as the retained dataset's README runs it
  (`src/tests/temporal/README.md`): Node 22.22.0, `lib/shim.mjs` imported first, its three runtime dependencies
  installed from the npm registry (big-integer 1.6.52, call-bind 1.0.9, es-abstract 1.24.2). Every probe ran under it
  too. Where its answer and the pinned text disagree, the row says so (D8).
- A cell marked *(host)* is behaviour that comes from .NET; *(reading)* is code not executed.

SJL-10.2's vectors and the retained dataset confirm or refute each row. **It holds no figure of a run**: the values
below are inputs, the answers they give, and counts of data.

**Paths.** In Broiler.VM, `VM/` is `src/Broiler.VM.Profile.JavaScript/`; there `Core` is `JsTemporalCore.cs`, `Dur`
`JsTemporal.Durations.cs`, `Opt` `JsTemporal.Options.cs`, `Cal` `JsTemporal.Calendar.cs`, `Conv`
`JsTemporal.Conversions.cs`, `Zon` `JsTemporal.Zones.cs`, and `R.Dur`, `R.PD`, `R.PDT`, `R.PT`, `R.YMD`, `R.Ins`, `R.ZDT`
the realm files `JsRealm.TemporalDuration.cs`, `…PlainDate.cs`, `…PlainDateTime.cs`, `…PlainTime.cs`,
`…YearMonthDay.cs`, `…Instant.cs`, `…ZonedDateTime.cs`. In Broiler.JS, `T/` is
`Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/`; there `J.Dur` is `JSTemporalDuration.cs`, `J.DurA`
`JSTemporalDuration.Arithmetic.cs`, `J.Rnd` `JSTemporalZonedDateTime.Rounding.cs`, `J.PD`, `J.PDT`, `J.PT`, `J.PYM`,
`J.PMD`, `J.Ins`, `J.ZDT` the `JSTemporal*.cs` files of those types, `J.RO` `JSTemporalRoundingOptions.cs` (class
`TemporalRoundingOptions`) and `J.NI` `TemporalNonIso.cs`. `SL/` is `Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/`.
`PT/` is the proposal at `e8cc03fc`; `PT/spec/x.html` names the file an abstract operation is in. `runs/` is
[`correspondence-runs/core/`](correspondence-runs/core/). It holds every probe (`probes/`), the program each engine ran
(`built/`, `vm262/test/sjl10/`), each engine's raw answer (`*.bjs.txt`, `*.vm.raw.txt`), the reference's (`*.ref.txt`),
and the comparison (`*.compare.txt`), named by row. `run-dataset.sh` runs the retained dataset (`d6-retained-dataset.*`, each
engine's lines and their `diff` against the retained answer), and `run-all.sh` runs everything. A file over 100 KB is retained
gzip-compressed with `.gz`. The proposal's sources and polyfill are not retained: the rows cite them by revision and
path. **A comparison line starts with two marks**, Broiler.VM's then Broiler.JS's, each against the reference: blank
where the engine answers as the reference does, `~` where both throw the same constructor with different messages, `!`
for any other difference. Messages differ on every error line; the proposal states only an error's type, and the
message stays each engine's (JSD-0054 section 5). A line number is the line at the revision above; a bare `:n` is in
the file named last in the same cell or bullet.

## Facts

**Where the code is.**

- **Broiler.VM: one engine-free core, and engine-facing files around it.**
  - `Core` (723 lines) holds the records, the limits, the balancing, the duration helpers and the formatting. It names
    no engine type, raises no error (no `engine.Error` site) and charges nothing. It throws only where the shared
    library's preconditions do (R6).
  - `Dur` (1,025 lines) holds the duration arithmetic, the differences and the relative rounding. It takes the engine
    for 19 error sites, for the zone operations (`Zon:237-304`, over SJL-8's `ZonedTime`) and for the calendar
    operations (`Cal:671-810`, which dispatch to the realm's calendars, `Cal:211`). Only `ToPartialDuration`,
    `ToDuration` and `RelativeToOption` read properties (`Dur:108-172, 258-348`).
  - `Opt` (442 lines) reads options in specification order and holds their rules beside the reads.
  - `Cal` holds the ISO `RegulateDate`, `DateAdd` and `DateUntil` bodies (`Cal:577-594, 671-810`).
  - The realm files (`R.*`) keep the objects, the reads and the errors, and call the above.
  - No Temporal file meters; only the generic per-call charge applies.
- **Broiler.JS: the arithmetic in the object classes, copied per type.** Each `JSTemporal*` class stores its fields as
  C# `int`s (`J.PD:30`, `J.PDT:31-32`, `J.PT:28`), a duration as ten doubles (`J.Dur:27`), an instant as a
  `BigInteger`. Each class carries its own limits, regulation, addition, balancing, difference and difference
  settings, and raises errors through `JSEngine`'s static delegates (`JSEngine.NewRangeError` and kin: 29 sites in
  `J.DurA`, 48 in `J.PDT`, 84 in `J.ZDT`). The duration arithmetic is in `J.DurA` (850 lines; plain-date
  `relativeTo`) and `J.Rnd` (489 lines; zoned `relativeTo`). Section 5 lists the copies.
- **Only Broiler.VM pins the proposal.** Broiler.VM cites the pinned draft's operations by section (JSD-0053,
  JSD-0054). Broiler.JS cites the proposal's operations and test262 cases by name, at no revision.

**Already shared, and so not SJL-10's to move.**

- **From SJL-4**, `SL/Numbers/ExactRounding.cs` and `RoundingMode.cs`: `RoundNumberToIncrement`,
  `RoundNumberToIncrementAsIfPositive`, `Negate`, `GetUnsignedRoundingMode`, `ApplyUnsignedRoundingMode`, `ToDouble`
  (an exact ratio rounded once to a double) and `TryParseRoundingMode` (`:53-240`). Also
  `IntegerConversions.ToIntegerOrInfinity` (`SL/Numbers/IntegerConversions.cs:119`). Both engines round and convert
  every Temporal quantity through these: Broiler.VM at 19 call sites, Broiler.JS at 54.
- **From SJL-6**, `SL/Time/IsoCalendarMath.cs`: `IsLeapYear`, `DaysInYear`, `DaysInMonth`, `EpochDays`,
  `FromEpochDays`, `DayOfWeek`, `DayOfYear`, `WeekOfYear`, `BalanceYearMonth` and `IsValidIsoDate` (`:62-235`), with
  `EcmaTime.FloorDivide` under them. These are the civil-day half of `AddISODate`, `BalanceISOYearMonth` and the
  limits. Their **preconditions are exceptions**: a year above 2^53 in magnitude, a day count above 2^60, or a month
  outside 1 to 12 throws `ArgumentOutOfRangeException` (`:239-260`).
- **From SJL-8**, `SL/Time/ZonedTime.cs`:
  - the neutral records `IsoDate(long, int, int)`, `IsoTime` (six `int`s, no overflow days) and `IsoDateTime`
    (`:29-47`);
  - `MinEpochNanoseconds` and `MaxEpochNanoseconds` (`:149-154`);
  - the zone operations both engines call (`:172-380`);
  - private copies of GetUTCEpochNanoseconds, a time's nanoseconds, BalanceISODateTime and a date-time's validity
    (`:531-580`), the same arithmetic as `Core:249-251, 341-342, 361-365`.

  The area design's target records (`IsoDate`, `TimeOfDay`, `IsoDateTime` in `Broiler.ECMAScript.Temporal`) overlap
  these already (decision 2).

**The limits.**

| Limit | The proposal | Broiler.VM | Broiler.JS |
|---|---|---|---|
| Instant | \|ns\| ≤ 8.64 × 10^21 (IsValidEpochNanoseconds, `PT/spec/instant.html`) | `Core:119-124, 347` | `J.Ins:25-26`, `J.ZDT:30-31, 83`, `J.DurA:562-563` |
| Date-time | strictly within the instant range widened by a day (ISODateTimeWithinLimits) | `Core:214-223`, a ±100,000,001-day check then the exact one | `J.PDT:27-29, 1171-1180`; `J.DurA:564-575` |
| Date | the date at noon is a valid date-time (ISODateWithinLimits) | `Core:209` | epoch days −100,000,001 … 100,000,000: `J.PD:27-28, 801-805`, `J.NI:20-21`, `J.PYM:765-775` |
| Year-month | April −271821 to September 275760 (ISOYearMonthWithinLimits) | `Core:228-230` | `J.PYM:776-783` |
| Duration | years, months, weeks below 2^32; the time with days below 2^53 seconds (IsValidDuration) | `Core:402-442` | `J.Dur:334-370` |
| A field's width | mathematical values | years `long`, months and days `int`, after a double (`R.PD:42-47`) | every date and time field `int` (`J.PD:30`) |
| Early refusals that the limits would also refuse | none | \|year\| > 10^9 from fields (`Cal:601-604`); years > 10^9, months or weeks > 10^10, days > 10^11 in `DateAdd` (`Cal:673-677`); a month-day's year past 10^9 reduced modulo 400 (`Cal:656-662`) | a zoned addition's year beyond ±400,000 (`J.ZDT:898-899`) |

The early refusals answer what the limits answer, a `RangeError`, only sooner; no probe tells them apart. They keep
the shared library's preconditions unbroken. The constructors do not refuse early in Broiler.VM (R6).

## 1. Records and limits

**Broiler.VM**:
- **The records** are `JsIsoDate`, `JsTimeRecord` (with its overflow days), `JsIsoDateTime`, `JsDateDuration` (four
  doubles) and `JsInternalDuration` (a date duration and a `BigInteger` of nanoseconds) (`Core:47-90`). The units are
  `TemporalUnit` with `Unset` and `Auto` (`Core:27-45`).
- **The constructors** convert each argument with `IntegerWithTruncation` (`Opt:377-389`). They cast the double to
  `long` or `int`, test `IsoCalendarMath.IsValidIsoDate` or `IsValidTime` (`R.PD:36-47`, `R.PDT:41-60`, `R.PT:42-54`,
  `R.YMD:36-47, 223-234`), and the factories test the limits (`Conv:43-102`).

**Broiler.JS**:
- **Each constructor** converts with the shared `ToIntegerOrInfinity` (`J.RO:85-92`). It narrows the double to `int`
  with a cast (`J.PD:486-499`, `J.PDT:755-767`, `J.PT:416-422`, `J.PYM:505-516`, `J.PMD:286-292`, `J.ZDT:1423-1427`,
  `J.NI:584-590`). On .NET 9 and later that conversion saturates at `int.MinValue` and `int.MaxValue` *(host)*.
- **The arithmetic** narrows a `long` year sum to `int` unchecked, which wraps (R7).

| Row | Input | Broiler.VM | Broiler.JS | The proposal and the reference | Class |
|---|---|---|---|---|---|
| R1 | Dates at and one day beyond each limit, through the constructor, `from` a string, `from` a bag, and a month-day's reference year *(run)* | −271821-04-19 and +275760-09-13 accepted; −271821-04-18, +275760-09-14 and the reference years beyond them `RangeError` | the same | ISODateWithinLimits, CreateTemporalDate, CreateTemporalMonthDay; the reference agrees | **same**. `runs/r1-date-limits.compare.txt` |
| R2 | Date-times at the limits: −271821-04-19T00:00 and +1 ns; +275760-09-13T23:59:59.999999999; +275760-09-14T00:00; through the constructor, `from`, a bag and `PlainDate.toPlainDateTime` *(run)* | `RangeError`, accepted, accepted, `RangeError` on every path | the same | ISODateTimeWithinLimits's strict comparison; the reference agrees | **same**. `runs/r2-datetime-limits.compare.txt` |
| R3 | Year-months −271821-03/04 and +275760-09/10, with reference days 1 and 19 *(run)* | the inner two accepted, the outer two `RangeError` | the same | ISOYearMonthWithinLimits | **same**. `runs/r3-yearmonth-limits.compare.txt` |
| R4 | Instants at ±8.64 × 10^21 ns and one beyond, from nanoseconds, milliseconds and strings with offsets; the wall clock of the extreme instants in `Etc/GMT-14`, `Etc/GMT+12` and `-23:59` *(run)* | at the limits accepted, beyond `RangeError`; +275760-09-13T14:00, −271821-04-19T12:00, −271821-04-19T00:01 | the same | IsValidEpochNanoseconds | **same**. `runs/r4-instant-limits.compare.txt` |
| R5 | Time and date fields: 24 hours, 60 minutes, 1,000 ms, −1, 1.9, −0.5, NaN, ∞, February 30 *(run)* | `RangeError` for the invalid ones; 1.9 → 1, −0.5 → 0; NaN and ∞ `RangeError` | the same | IsValidTime, IsValidISODate, ToIntegerWithTruncation | **same**. `runs/r5-time-records.compare.txt` |
| R6 | Integers beyond the fields. Years 2^31, 2^32 + 2020, 2^53, 2^53 + 2, ±10^20 to `new PlainDate`; 10^20 to `new PlainDateTime`, `new PlainYearMonth` and as `new PlainMonthDay`'s reference year; months and days 2^32 + 1; the same in bags under both overflows and in `with` *(run)* | Up to 2^53 every case answers as the reference. **Years 2^53 + 2 and ±10^20 in `new PlainDate`, and 10^20 in `new PlainDateTime` and as `new PlainMonthDay`'s reference year, end the invocation** (`ProfileFault/ProfileContractViolation`, no JavaScript error, `runs/r6b…r6f-*.vm.raw.txt`). The cast to `long` saturates (`R.PD:42-47`, `R.PDT:47-60`, `R.YMD:229-234`). `IsValidIsoDate` tests no year (`SL/Time/IsoCalendarMath.cs:234-235`). `DateTimeWithinLimits` then calls `IsoCalendarMath.EpochDays` (`Core:216`), whose precondition (\|year\| ≤ 2^53) throws `ArgumentOutOfRangeException` (`SL/Time/IsoCalendarMath.cs:96-99, 239-247`) before the day-range test. `new PlainYearMonth` is safe: its limit compares the year first (`Core:228-230`). Bags are safe: `Cal:601-604` | `RangeError` for every case: the saturated `int` fails IsValidISODate or the limits *(host)*. Bags constrain to 12, 31, 23 and reject with `RangeError`, as the reference does | CreateTemporalDate and CreateTemporalDateTime: "If ISODateWithinLimits(…) is false, throw a RangeError"; the reference throws `RangeError` for all | **correction in Broiler.VM** for the five faulting cases: the limit must be tested on a bounded year before the shared epoch-day arithmetic. Otherwise **same**. Not in the area's table. `runs/r6-integer-conversion.compare.txt`, `runs/r6b-plaindate-1e20.compare.txt` to `runs/r6f-plaindate-2e53-plus-2.compare.txt` |
| R7 | Narrowing inside addition (**the milestone's listed correction**): `add` and `subtract` of `{years: 4294967295}` to a `PlainDateTime`, `PlainYearMonth`, `PlainDate`, `ZonedDateTime` and Hebrew and Coptic dates; `{nanoseconds: 9e24}`, `{microseconds: 9e21}` and `{milliseconds: 9e18}` to dates and date-times; `Duration.round` and `total` of `{nanoseconds: 9e24}` relative to 2020-01-01 *(run)* | `RangeError` for each (`Cal:673-677`), as the reference | **Wraps**: 2020-06-15T12:00 + P4294967295Y = 2019-06-15T12:00, and − = 2021-06-15T12:00 (`J.PDT:1193`); + P4294964275Y = −001001-06-15T12:00; `PlainYearMonth` 2019-06 and 2021-06 (`J.PYM:347`); Hebrew 2019-06-26[u-ca=hebrew], Coptic 2021-06-15[u-ca=coptic] (`J.NI:480`, also `:467`). **Saturates** *(host)*: 2020-01-01 plus 9e24 ns is 2312-04-11, and as a date-time 2312-04-11T23:47:16.854775807, because the nanoseconds field is cast to `long` (`J.PD:293-299`, `J.PDT:1098-1101`); the duration relative to 2020-01-01 rounds to PT9223372037S, balances to P292Y3M10DT23H47M16.854775807S and totals 106751.99116730064 days (`J.DurA:295-297`). `PlainDate` and `ZonedDateTime` years, the microseconds and the milliseconds are right: `long` sums and range checks (`J.PD:819-837`, `J.ZDT:895-899`) | CalendarDateAdd ends "If ISODateWithinLimits(result) is false, throw a RangeError" (`PT/spec/calendar.html`); the reference throws for all | **correction in Broiler.JS**, **confirmed**: it wraps at four sites and saturates at three. The `AddYearsMonths` copies cast the same way (`J.PD:1118`, unused; `J.ZDT:1274`, reached only with valid dates) *(reading)*. `runs/r7-narrowing-in-addition.compare.txt` |
| R8 | Duration limits: 2^32 − 1 and 2^32 years, months, weeks; 104,249,991,374 and …375 days; 2^53 − 1 and 2^53 seconds; 2^53 − 1 s + 999,999,999 ns and one more; 9 × 10^24 ns; 2,501,999,792,983 h 36 and 37 min; mixed signs; 1.5; ∞; −0; the same from strings *(run)* | the reference's answers; −0 stored as +0; 9e24 ns kept as the double 9000000000000000385875968 | the same | IsValidDuration, maxTimeDuration = 2^53 × 10^9 − 1 | **same**. Strings are the grammar half's. `runs/r8-duration-limits.compare.txt` |

## 2. Balancing, regulation and addition

**Broiler.VM**:
- **Balancing** over exact nanoseconds: BalanceTime (`Core:256-284`), AddTime (`Core:304`), BalanceISODateTime
  (`Core:361-365`) and AddDaysToISODate over the shared epoch days (`Core:194-195`).
- **Regulation**: RegulateTime (`Dur:386-406`) and RegulateISODate (`Cal:577-594`).
- **The ISO `CalendarDateAdd`**: the shared `BalanceYearMonth`, then `RegulateDate`, then the days, then the limit
  (`Cal:684-693`).
- **Additions**: AddInstant (`Dur:490-500`) and AddZonedDateTime (`Dur:505-523`). Each type's AddDurationTo… is in its
  realm file (`R.PD:227-233`, `R.PDT:250-259`, `R.PT:182-187`, `R.YMD:143-162`, `R.Ins:181-197`, `R.ZDT:356-363`).
- **Rounding of points**: RoundTime, RoundISODateTime (`Core:309-334, 379-383`) and RoundTemporalInstant
  (`Conv:580`).

**Broiler.JS** does each per type:
- **AddISODate**: `J.PD:815-841`, `J.PDT:1190-1208`, `J.ZDT:893-917`, and inline in `J.PYM:346-350`.
- **Time carried into days**: `J.PDT:398-403, 739-747`, `J.PT:173-182`, `J.Ins:124-129`, `J.ZDT:848-873`.
- **Regulation**: `J.PD:770-786`, `J.PDT:1020-1042`, `J.PT:447-466`, `J.PYM:746-760`.
- **Rounding of points**: `J.PT:291-298`, `J.PDT:551-574`, `J.Ins:225-272`, `J.ZDT:545-642`.

Both use the shared civil-day helpers.

| Row | Input | Broiler.VM | Broiler.JS | The proposal and the reference | Class |
|---|---|---|---|---|---|
| B1 | `PlainTime` arithmetic: across midnight both ways, 10^9 hours, the maximum time duration, 9 × 10^24 ns, date units in the duration *(run)* | wraps modulo a day exactly: 12:00 + 9e24 ns = 04:00:00.385875968; date units ignored | the same (`BigInteger`, `J.PT:173-182`) | AddDurationToTime, AddTime, BalanceTime | **same**. `runs/b1-balance-time.compare.txt` |
| B2 | `PlainDateTime` and `Instant` arithmetic across days, month ends and 366 × 86,400 × 10^9 ns; 2^53 − 1 s; days on an instant *(run)* | balanced as the reference; out of range and days on an instant `RangeError` | the same | AddDurationToDateTime, BalanceISODateTime, AddDurationToInstant | **same**. `runs/b2-balance-datetime.compare.txt` |
| B3 | AddISODate under both overflows: January 31 + 1 month, February 29 + 1 year, 2019-01-31 + P1Y1M, P2W3D, − P15M, + 1,200,000 months, + 10^11 days, + 10^9 + 1 years; `overflow` `"bogus"` and `null` *(run)* | 2020-02-29 and 2021-02-28 (under `reject`, `RangeError`), 2020-02-29, 2020-01-18, 2018-12-15, +102020-01-01; `RangeError` for 10^11 days, 10^9 + 1 years and `"bogus"`; `TypeError` for `null` | the same | CalendarDateAdd's ISO branch, RegulateISODate, GetTemporalOverflowOption | **same**. `runs/b3-add-iso-date.compare.txt` |
| B4 | RegulateISODate and RegulateTime from bags: February 31, month 13, month or day 0, 25:61:61.1000, −1 hours, 1,000 µs and ns, second 60, `with` across a leap day *(run)* | constrained as the reference; `reject` `RangeError`; month or day 0 `RangeError` | the same | RegulateISODate, RegulateTime, ToPositiveIntegerWithTruncation | **same**. `runs/b4-regulate.compare.txt` |
| B5 | Addition at the limits: a day past either date limit; 10^8 days from 1970; the date-time limits; `PlainYearMonth` −271821-04 + P0M, **−271821-05 − P1M**, +275760-08 + P1M; an instant past the limit; `ZonedDateTime` at and near it *(run)* | as the reference; **−271821-05 − P1M: `RangeError`**, because 1 April −271821 is not a representable date (`Cal:688-691`) | as the reference, but **−271821-05 − P1M = −271821-04**: the year-month is checked against the year-month limits only (`J.PYM:343-350, 746-760`) | AddDurationToYearMonth adds to the first of the month through CalendarDateAdd, which ends with ISODateWithinLimits (`PT/spec/plainyearmonth.html`, `calendar.html`); the reference throws ("−271821-04-01 is outside of supported range") | **correction in Broiler.JS**. Not in the area's table. `runs/b5-limits-in-addition.compare.txt` |
| B6 | `ZonedDateTime` arithmetic in New York across both 2026 transitions: P1D against PT24H, from the gap's hour and the repeated one; `reject`; P1M1W *(run)* | the calendar day on the wall clock, the hours on the timeline: 2026-03-07T02:30−05:00 + P1D = 03-08T03:30−04:00; 2026-11-01T00:30−04:00 + P1D = 11-02T00:30−05:00, + PT24H = 11-01T23:30−05:00 | the same (`J.ZDT:838-878`) | AddZonedDateTime | **same**. `runs/b6-zoned-addition.compare.txt` |
| B7 | A `PlainDate` plus time units: PT47H, PT48H, −PT25H, −PT23H, 86,400 × 10^9 − 1 ns, P1DT24H, P1MT24H, 2^53 − 1 s *(run)* | whole days, truncated toward zero: 01-02, 01-03, 12-31, 01-01, 01-01, 01-03, 03-01, `RangeError` | the same | ToDateDurationRecordWithoutTime | **same**. `runs/b7-date-add-time-units.compare.txt` |
| B8 | `PlainYearMonth` arithmetic: − P1M, + P1Y13M, weeks, days or hours in the duration, − 1,200,000 months *(run)* | 2019-12, 2022-02, `RangeError`, −097980-01 | the same | AddDurationToYearMonth | **same**. `runs/b8-yearmonth-addition.compare.txt` |
| B9 | Rounding of points: `PlainTime` 12:30 and the instant −1.5 s in all nine modes; increments up to a day; `PlainDateTime` rounding to a day at the limits; `Instant` at the limit; `ZonedDateTime` to the second in New York's gap and fold, and to a day at the limit *(run)*. Also the retained dataset's lines of times and date-times rounded in every unit and mode, and instants at exact ties *(run)* | as the reference; increments that do not divide a day, and results past a limit, `RangeError` | the same | RoundTime, RoundISODateTime, RoundTemporalInstant, `Temporal.ZonedDateTime.prototype.round` | **same**. `runs/b9-round-points.compare.txt`; `runs/d6-retained-dataset.*` |

## 3. Durations: validity, sign, largest unit, addition, comparison, rounding and totals

**Broiler.VM**:
- **The basic operations**: IsValidDuration, DurationSign, DefaultTemporalLargestUnit, the internal records and
  TemporalDurationFromInternal (`Core:402-574`). The last rounds each field to a Number once, with `ExactRounding.ToDouble`
  (`Core:559-566`).
- **The methods**: compare, AddDurations, round and total (`R.Dur:173-420`).
- **The relative machinery**: DateDurationDays, ComputeNudgeWindow, NudgeToCalendarUnit, NudgeToZonedTime,
  NudgeToDayOrTime, BubbleRelativeDuration, RoundRelativeDuration and TotalRelativeDuration (`Dur:696-1024`). It runs
  over the calendar's `DateAdd` and `DateUntil` and the zone's `EpochNsFor`, so every calendar takes one path.
- **Totals** are exact fractions (`JsFraction`, `Dur:27-32`), rounded once.
- **Where it follows the polyfill against the draft**: JSD-0054 section 4 names two places. ComputeNudgeWindow's start
  is the origin when the start *duration* is zero (`Dur:774-785`). ISODateSurpasses reckons weeks and days in epoch
  days (`Cal:719-743`).

**Broiler.JS**:
- **In `J.Dur`**: RejectDuration, DurationSign and compare (`J.Dur:223-370`).
- **In `J.DurA`**: add and subtract, round, total, and a plain-date `relativeTo` machinery of its own (`J.DurA:87-781`).
  It is ISO-only: it adds through `JSTemporalPlainDate.AddCalendarDate` and differences through `DiffCalendarDate`.
- **In `J.Rnd`**: a second, zoned machinery that follows the proposal's operations by name (`J.Rnd:52-414`).
- **A non-ISO `relativeTo`** is refused with a plain `Error` (`J.DurA:327-328, 348-349, 406-407, 419-420`).
- **Totals** are exact, through `ExactRounding.ToDouble` (`J.DurA:277, 679, 688`, `J.Rnd:107, 225`).
- **Both machineries take the start-duration test**, as the polyfill does: `J.Rnd:287-289`, `J.PD:1001-1003`.

| Row | Input | Broiler.VM | Broiler.JS | The proposal and the reference | Class |
|---|---|---|---|---|---|
| D1 | `sign`, `blank`, `negated` (zeros stay +0), `abs`, `with` making mixed signs, an empty `with` *(run)* | as the reference | the same | DurationSign, CreateNegatedTemporalDuration, ToTemporalPartialDurationRecord | **same**. `runs/d1-sign-and-shape.compare.txt` |
| D2 | DefaultTemporalLargestUnit and the balance of a sum: PT30M + PT90M, PT1H + PT90M, P1D + PT24H, PT59S + 1,001 ms, 1 µs + 2,000 ns; (2^53 − 1) ns + 2 and + 4; the maximum time duration and one more *(run)* | PT120M, PT2H30M, P2D, PT60.001S, PT0.000003S; 9007199254740992 and 9007199254740996, ties to even; `RangeError` | the same (`J.DurA:800-843`) | AddDurations, TemporalDurationFromInternal, 𝔽 | **same**. `runs/d2-largest-unit-and-balance.compare.txt` |
| D3 | `Duration.prototype.add` and `subtract` with calendar units (**the milestone's listed correction**): P1Y + P1M without and with `{relativeTo}` (a string and a `ZonedDateTime`); P1W + P1D; and a time-only sum with a second argument of `5`, `null`, and an object whose `relativeTo` getter logs *(run)* | `RangeError` in every calendar case. The second argument is not read: `add` takes one parameter (`R.Dur:104-108, 224-244`). PT2H for the time-only sums, with no read | Without `relativeTo`: `RangeError` (`J.DurA:92-96`). **With it: a plain `Error`, "… with a relativeTo calendar/time-zone is not yet implemented"** (`J.DurA:97-98`). The second argument is read as options: `5` and `null` throw `TypeError`, and the getter runs (`J.DurA:75-83, 90`) | `Temporal.Duration.prototype.add ( other )` takes one parameter; AddDurations throws `RangeError` when the larger default largest unit is a calendar unit (`PT/spec/duration.html`). The reference: `RangeError`; PT2H; no read | **correction in Broiler.JS**, **confirmed**, in three parts: the plain `Error`, the `TypeError` for a second argument, and the observable read of a parameter the proposal no longer has. Broiler.VM's "ignores `relativeTo` per the current proposal" (the area's table) is confirmed. `runs/d3-add-calendar-units.compare.txt` |
| D4 | `compare`: PT24H against P1D; months against days with no `relativeTo` and relative to 2020-02-01 and 2020-01-01; P1D against PT24H and PT23H on New York's 23-hour day; identical durations with no or an invalid `relativeTo`; **{years: 1, seconds: 9e15} against {years: 1}** and against 9e15 − 1 seconds, relative to 2020-01-01; Hebrew `relativeTo` *(run)* | as the reference; **1 and 1** for the long durations: DateDurationDays adds only the years, months and weeks to `relativeTo` (`Dur:696-707`), then Add24HourDaysToTimeDuration bounds the time (`R.Dur:197-218`) | as the reference, but **`RangeError` for the long durations**: it adds the whole duration, time included, to `relativeTo` and requires a representable date-time (`J.Dur:279-283`, `J.DurA:550-585`); Hebrew `Error` (D11) | `Temporal.Duration.compare` with DateDurationDays (`PT/spec/duration.html`): no date-time limit is involved; the reference answers 1 and 1 | **correction in Broiler.JS** (the range check). Not in the area's table. Otherwise **same**. `runs/d4-compare.compare.txt` |
| D5 | `round` with no `relativeTo`. PT1H30M, −PT1H30M and PT2H30M to the hour, and −PT1H20M to 30 minutes, in all nine modes. `largestUnit` day and hour; day increments of 2, 10^9 and 2.5; 15-minute increments. Missing or crossed units; increments 5 and 24 for hours; an increment with a coarser `largestUnit`; calendar units with no `relativeTo`; the maximum time duration rounded up *(run)* | as the reference in every line | the same | `Temporal.Duration.prototype.round`, RoundTimeDurationToIncrement, ValidateTemporalRoundingIncrement | **same**. `runs/d5-round-time.compare.txt` |
| D6 | `round` with a plain-date `relativeTo`: 22 cases of nudging, bubbling, weeks, increments, negative durations and the limits (`runs/probes/d6-round-plain-relative.js`). **The retained dataset** (`src/tests/temporal/temporal.js`, unchanged) *(run)* | as the reference on the 22; the dataset: all 1,629 lines equal its retained answer | as the reference on the 22. **The dataset: 7 lines differ**: P1Y2M3W4DT5H6M7.008009010S rounded to months relative to 2020-01-31, in `ceil`, `expand`, `halfCeil`, `halfFloor`, `halfExpand`, `halfTrunc` and `halfEven`, is **P1Y2M30D**, where the reference says P1Y3M. Broiler.JS rounds the total month count, adds it to `relativeTo`, then re-derives years and months with `DiffCalendarDate` (`J.DurA:602-623`). 2020-01-31 + 15 months is 2021-04-30, which the difference reads as 1 year 2 months 30 days | RoundRelativeDuration keeps the nudge's EndDuration (P1Y3M) and bubbles it (`PT/spec/duration.html`) | **correction in Broiler.JS**. Not in the area's table. `runs/d6-round-plain-relative.compare.txt`; `runs/d6-retained-dataset.bjs.diff`, `.vm.diff` (empty) |
| D7 | `round` relative to a date with a coarser `largestUnit`, where the destination lies past the first window: {months: 10, days: 29, hours: 12} to months, `largestUnit` year, `trunc` and `halfExpand`, relative to 2019-03-31 *(run)* | **P11M** | **P10M29D**, by D6's re-derivation | the text: P11M; the reference: P1Y (D8) | **correction in Broiler.JS** whichever way D8 is settled: Broiler.JS's three answers to one question (P10M29D here, P1Y and P11M in D8) are not one behaviour. `runs/d6-round-plain-relative.compare.txt`, `runs/d8-shifted-window.compare.txt` |
| D8 | **The nudge after a shifted window.** The same duration relative to `2019-03-31T00:00[UTC]` (a string and a `ZonedDateTime`); `PlainDateTime` and `ZonedDateTime` 2019-03-31T00:00 `until` 2020-02-29T12:00 with `smallestUnit` month and `largestUnit` year *(run)* | **P11M in all five.** After the second window, a unit rounded down to r1 keeps the window's start as the nudged instant (`Dur:844-851`). Bubbling then sees 2020-02-29 before 2019-03-31 + 1 year | **P1Y** for the zoned `relativeTo`: the nudged instant is the window's end whenever the window was shifted (`J.Rnd:227-230`). **P11M** for both `until`s: `J.PD:1054-1056` keeps the start | **The pinned text and its polyfill disagree.** The text of NudgeToCalendarUnit: "Else, Let *resultDuration* be *startDuration*. Let *nudgedEpochNs* be *startEpochNs*" (`PT/spec/duration.html`), so P11M. The polyfill: `nudgedEpochNs: didExpandCalendarUnit ? endEpochNs : startEpochNs` (`PT/polyfill/lib/ecmascript.mjs:3363`), so it bubbles into a year: **the reference answers P1Y in all five**, though 2019-03-31 + 1 year (2020-03-31) is after the destination and its own `total` of the duration is 11.016129032258064 months | **needs a decision** (decision 1). Broiler.VM follows the text, as JSD-0054 section 4 does wherever it names no exception. The retained dataset does not reach the case. Not in the area's table. `runs/d8-shifted-window.compare.txt`; the same cases in `runs/f5-datetime-differences.compare.txt` and `runs/f7-zoned-differences.compare.txt` |
| D9 | `round` with a zoned `relativeTo` across New York's 2026 transitions. PT23H and PT12H to a day; PT24H and PT25H to a largest day; P1D to a largest hour. A 23-hour window from 2026-03-07T12:00 and a 25-hour window from 2026-10-31T12:00. Hour increments with a largest day. Windows at the instant limit *(run)* | real day lengths: P1D, P1DT1H, PT23H, P1D; PT12H → P1D on the 23-hour day and PT0S on the 25-hour day; the windows' days 23 and 25 hours; past the limit `RangeError` | the same: `J.Rnd` nudges over the zone's instants (`J.Rnd:243-293, 299-365`) | NudgeToCalendarUnit with a time zone, NudgeToZonedTime | **same**. Bears on the listed DST correction (F7). `runs/d9-round-zoned-relative.compare.txt` |
| D10 | `total`: PT1H30M in hours; P45D in months relative to 2020-01-01, plain and zoned; Number.MAX_SAFE_INTEGER ns in seconds; the maximum time duration in nanoseconds and days; P1D in hours and PT12H in days on New York's 23- and 25-hour days; P1M in days; P1Y in weeks; −P45D in months; calendar totals with no `relativeTo`; windows at the limit *(run)* | 1.5; 1.4827586206896552 twice; 9007199.25474099; 9.007199254740992e+24, 104249991374.31703; 23, 0.5217391304347826, 0.48; 29; 52.285714285714285; −1.5161290322580645; `RangeError` | the same | TotalRelativeDuration, TotalTimeDuration; the NOTE on one division | **same**: exact ratios rounded once in both. `runs/d10-total.compare.txt` |
| D11 | `round`, `total` and `compare` relative to a date in another calendar: Hebrew (a string, a `PlainDate`, a bag, a leap year), Islamic civil, Coptic (zoned); Gregorian and Japanese *(run)* | answers as the reference: P1M, P1M16D, 1.5333333333333334, 1.5172413793103448, P1Y, P1M15D, P1M16D; −1 and 1 for `compare` | **a plain `Error`**, "a relativeTo with the "hebrew" calendar is not yet implemented", for every non-ISO calendar. Gregorian and Japanese agree: Broiler.JS does ISO arithmetic for them | GetTemporalRelativeToOption takes any calendar; the relative operations run through CalendarDateAdd and CalendarDateUntil | **correction in Broiler.JS**, as the area's table says ("Non-ISO `relativeTo`"). SJL-8's run of the retained calendar dataset shows the same on two lines per calendar, 22 in all (`../sjl-8-001/correspondence-runs/cal/retained-calendars.other-differing.txt`). `runs/d11-non-iso-relative.compare.txt` |
| D12 | Code no input reaches *(reading)* | a second window for week and day units: ComputeNudgeWindow ignores the shift for them (`Dur:751-769`), so the window cannot change | the shift for years and months only (`J.Rnd:207`); "time zone returned inconsistent Instants" (`J.Rnd:323-324`); a fourth day correction (`J.Rnd:150`, `J.ZDT:1036`) | the text shifts years and months only and asserts that the second window holds the destination; DifferenceZonedDateTime corrects at most twice | **code no input reaches**, in both. The shared code asserts or fails with a status |

## 4. Differences: `until` and `since` for every type

**Broiler.VM**:
- **The settings** come from one GetDifferenceSettings (`Opt:312-370`). It reads in alphabetical order, validates the
  unit group and the disallowed units, defaults, and negates the mode for `since`.
- **Each type's DifferenceTemporal…** is in its realm file (`R.PD:238-272`, `R.PDT:264-281`, `R.PT:192-201`,
  `R.YMD:167-210`, `R.Ins:202-211`, `R.ZDT:368-407`). Under them are DifferenceISODateTime (`Dur:528-551`),
  DifferencePlainDateTimeWithRounding (`Dur:556-578`), DifferenceZonedDateTime and its rounding (`Dur:608-676`),
  DifferenceInstant (`Dur:250-251`) and CalendarDateUntil (`Cal:753-810`, ISO; another calendar dispatches,
  `Cal:762-765`).
- **The rounding is RoundRelativeDuration for every calendar.**

**Broiler.JS**:
- **Each type reads its own settings**: `J.PD:588-625`, `J.PDT:841-876`, `J.PT:195-229`, `J.Ins:145-190`,
  `J.ZDT:1128-1156`, `J.PYM:564-597`.
- **Date differences** are DifferenceISODate (`J.PD:844-894`, a copy at `J.ZDT:1214-1261`) and `TemporalNonIso.Difference`
  (`J.NI:503-581`).
- **The calendar-unit rounding** is the ISO day-axis nudge `RoundDateDifference` (`J.PD:939-1063`). `PlainDate`,
  `PlainYearMonth`, `PlainDateTime` and `ZonedDateTime` all call it. The time-unit rounding is `RoundTimeDifference`
  (`J.PDT:484-530`), and `J.Rnd` for a `ZonedDateTime` (`J.ZDT:955-957`).

| Row | Input | Broiler.VM | Broiler.JS | The proposal and the reference | Class |
|---|---|---|---|---|---|
| F1 | The settings: each type's defaults (P439D, P2DT12H30M, PT22H30M15S, PT90000.5S, PT25H, P1Y10M); units outside the group; disallowed week and day for year-months; crossed units; increments 7 and 60 for minutes and 24 and 12 for hours; increments 0 and 10^9 + 1; a bad mode; `auto`; options `5`; other calendars and zones *(run)* | as the reference | the same | GetDifferenceSettings (`PT/spec/abstractops.html`) | **same** in effect, read by six copies of the rules (section 5). `runs/f1-settings-and-defaults.compare.txt` |
| F2 | `PlainDate` differences in every largest unit, both directions, anchored on the receiver across month ends and leap days; the full range in years, days and weeks *(run)* | as the reference: 2020-01-31 → 2020-03-01 P1M1D; 2021-02-28 → 2020-02-29 −P11M28D; −271821-04-19 → +275760-09-13 P547581Y4M25D | the same | DifferenceTemporalPlainDate, CalendarDateUntil, ISODateSurpasses | **same**. `runs/f2-date-differences.compare.txt` |
| F3 | ISO `PlainDate` rounding: months and years under several modes, weeks with `since` (the negated mode), a 5-day increment under a largest month, a 2-month increment, bubbling to years, a 10^9-day increment, past the limits *(run)* | as the reference: P7M, P1Y, −P3W, P2M20D, P4M, P2Y, P1000000000D, `RangeError` | the same | RoundRelativeDuration over NudgeToCalendarUnit and NudgeToDayOrTime | **same**. `runs/f3-date-rounding.compare.txt` |
| F4 | Differences in another calendar with rounding options (**the milestone's listed correction**): Hebrew `PlainDate`, `PlainDateTime`, `ZonedDateTime` and `PlainYearMonth`; Coptic weeks; Islamic civil days by 7; Gregorian *(run)* | rounded in the calendar, as the reference. Hebrew `PlainDate` 2020-01-01 → 07-20 to months P7M, → 2021-10-20 to years P2Y, `since` −P7M, → 01-16 to months P1M; Coptic → 03-20 to weeks (`ceil`) P12W; Islamic civil to 7 days P77D; Hebrew `ZonedDateTime` to a day P3D, to months P7M, to hours P2DT13H; Hebrew `PlainYearMonth` to years P1Y | **`PlainDate`, `PlainYearMonth` and `ZonedDateTime` ignore the rounding**: the same cases give P6M24D, P1Y10M10D, −P6M24D, P15D; P11W2D; P79D; P2DT13H, P6M24D, P2DT13H20M; P11M. The code: `J.PD:341-344`; `J.PYM:381-392`; for a `ZonedDateTime`, `J.ZDT:1045-1050` skips the calendar-unit rounding and `:955-957` the time-unit rounding. **`PlainDateTime` rounds against ISO months**: Hebrew 2020-01-01T00:00 → 2020-01-16T00:00 to months (`halfExpand`) is PT0S. It passes the Hebrew years and months to `RoundDateTimeDateDifference` (`J.PDT:453-461`), whose windows are ISO additions (`J.PD:930-934`); likewise its time-unit carry (`J.PDT:519-524`) *(reading)*. Gregorian agrees: Broiler.JS treats it as ISO | DifferenceTemporal… rounds whenever the smallest unit or the increment asks, for every calendar, through CalendarDateAdd and CalendarDateUntil | **correction in Broiler.JS**, **confirmed** for `PlainDate`, `PlainYearMonth` and `ZonedDateTime`; for `PlainDateTime` the options are applied but measured in ISO months. SJL-8's run of the retained calendar dataset shows it on five "until … round months" lines per non-ISO calendar, 55 in all (`../sjl-8-001/correspondence-runs/cal/retained-calendars.other-differing.txt`). The calendars' own `until` cores are the other half's. `runs/f4-non-iso-rounding.compare.txt` |
| F5 | `PlainDateTime` differences: the borrowed day both ways, a largest hour, a day rounding, month ends, rounding to microseconds that bubbles to P2Y, hour increments, weeks, the full range in nanoseconds and years *(run)* | as the reference, but D8's case | the same, but D8's case | DifferenceISODateTime, DifferencePlainDateTimeWithRounding | **same**, but D8. `runs/f5-datetime-differences.compare.txt` |
| F6 | `PlainTime` and `Instant` differences: negative, the negated mode of `since`, a largest minute, 6-hour increments under `halfEven`, the full instant range in hours, nanoseconds and microseconds, 30-minute increments under `expand` *(run)* | as the reference: −PT1H30M, PT1H30M, PT90M29S, PT12H and PT24H, PT4800000000H, PT17280000000000S, 17280000000000000000 µs | the same | DifferenceTemporalPlainTime, DifferenceTemporalInstant, DifferenceInstant | **same**. `runs/f6-time-and-instant.compare.txt` |
| F7 | `ZonedDateTime` differences across New York's 2026 transitions (**the milestone's listed DST correction**). A largest hour and day over both transition days. **A smallest day** from 2026-03-07T12:00 to 23:45 (a 23-hour window), from 2026-10-31T12:00 to 11-01T00:15 (a 25-hour window), and with a largest month. Hours over the transition; weeks; times in the gap and fold. `round` to a day and `hoursInDay` *(run)* | real day lengths: PT23H, P1D, P1D; **P1D, PT0S, P1D**; P1DT1H, P1D, P1M1W; PT1H, PT1H, PT24H30M; `round` 2026-03-08T00:00−05:00 and 2026-11-02T00:00−05:00; 23 and 25 | as the reference, but **PT0S, P1D, PT0S** for the three day roundings. A calendar smallest unit reuses the PlainDate day-axis nudge on 24-hour days (`J.ZDT:1055-1084`; its comment: "a DST day length is not yet modelled here"). Hour rounding (through `J.Rnd`), `round` (`J.ZDT:593-607`) and `hoursInDay` use real lengths | NudgeToCalendarUnit's window is measured in the zone's instants (ComputeNudgeWindow with a time zone) | **correction in Broiler.JS**, **confirmed** for `until` and `since` with a calendar smallest unit. It is **refuted** for `ZonedDateTime.prototype.round`, `hoursInDay`, and `Duration` rounding and totals relative to a zoned date (D9), all DST-aware. Week to year windows take the same 24-hour path *(reading)*. `runs/f7-zoned-differences.compare.txt` |
| F8 | `PlainYearMonth` differences: largest month, years under three modes, a 5-month increment, both limits, other calendars *(run)* | as the reference: P22M, P2Y, −P2Y, P1Y10M; −271821-04 to −05 `RangeError`; +275760-09 since −08 P1M | the same | DifferenceTemporalPlainYearMonth | **same**. `runs/f8-yearmonth-differences.compare.txt` |

## 5. What moves and what is deleted

**For SJL-10.4, the Broiler.VM members of these four sub-areas that move, with their engine coupling:**

| Members | Where | Coupling today |
|---|---|---|
| The records, units and limits: `TemporalUnit`, `JsIsoDate`, `JsTimeRecord`, `JsIsoDateTime`, `JsDateDuration`, `JsInternalDuration`; `NsPerDay`, `NsMaxInstant`, `NsMinInstant`, `MaxTimeDuration`, `UnitLength`, `UnitNames`, `IsCalendarUnit`, `IsDateUnit`, `Larger`, `MaximumIncrement`; `DateWithinLimits`, `DateTimeWithinLimits`, `YearMonthWithinLimits`, `DaysWithinRange`, `IsValidTime`, `IsValidEpochNs`, the three `Compare`s, `Exact` | `Core:27-235, 242-244, 289-294, 347, 370-374, 390` | **none**. `DateTimeWithinLimits` needs R6's guard. The records meet SJL-8's `IsoDate`, `IsoTime` and `IsoDateTime` (decision 2) |
| Balancing and addition: `TimeNs`, `BalanceTime` (both), `DifferenceTime`, `AddTime`, `RoundTime`, `UtcEpochNs`, `FromEpochNs`, `BalanceDateTime`, `RoundDateTime`, `AddDays` | `Core:194-195, 249-383` | **none** |
| Regulation and the ISO `CalendarDateAdd` and `CalendarDateUntil`: `RegulateTime`, `RegulateDate`, `DateAdd`'s ISO body and its early refusals, `Surpasses`, `DateSurpasses`, `DateUntil`'s ISO body; `AddInstant` | `Dur:386-406, 490-500`; `Cal:577-594, 671-810` | `RangeError`s (`engine.Error`); `DateAdd` and `DateUntil` dispatch other calendars to the realm's (`Cal:211, 679-682, 762-765`), the calendar provider interface's place |
| Durations: `IsValidDuration`, `TimeFromComponents`, `DurationSign`, both `Sign`s, `DefaultLargestUnit`, `ToInternal`, `ToInternalWith24HourDays`, `FromInternal` | `Core:402-574` | **none** |
| The duration operations: `JsFraction`, `Negated`, `DateDurationRecord`, `AdjustDate`, `AddTimeDuration`, `Add24HourDays`, `RoundTimeDurationToIncrement`, `RoundTimeDuration`, `TotalTimeDuration`; `CreateDuration`'s validity test | `Dur:27-32, 62-79, 84-94, 183-245` | `RangeError`s |
| The relative machinery: `DateDurationDays`, `NudgeWindow`, `EpochFor`, `ComputeNudgeWindow`, `NudgeToCalendarUnit`, `NudgeToZonedTime`, `NudgeToDayOrTime`, `BubbleRelativeDuration`, `RoundRelativeDuration`, `TotalRelativeDuration`, `JsNudge` | `Dur:51, 696-1024` | `RangeError`s; calendar callbacks (`DateAdd`, `DateUntil`); zone callbacks (`EpochNsFor`, `Zon:279-289`) |
| Differences: `DifferenceInstant`, `AddZonedDateTime`, `DifferenceIsoDateTime`, `DifferencePlainDateTimeWithRounding`, `…WithTotal`, `DifferenceZonedDateTime`, `…WithRounding`, `…WithTotal` | `Dur:250-251, 505-691` | `RangeError`s; calendar and zone callbacks (`IsoDateTimeFor`, `EpochNsFor`, `Zon:243-289`) |
| The arithmetic of the realm methods, after their reads: `DurationCompare`, `AddDurations`, `DurationRound`, `DurationTotal`, and each `DifferenceTemporal…` and `AddDurationTo…` | `R.Dur:180-244, 279-377, 389-419`; `R.PD:227-272`; `R.PDT:250-281`; `R.PT:182-201`; `R.YMD:143-210`; `R.Ins:181-211`; `R.ZDT:356-407` | objects, reads and errors stay in the realm files; the cores call the library |
| The rule half of the options: the unit names and groups, `ValidateIncrement`, `ValidateUnit`'s test, GetDifferenceSettings's validation and defaults (steps after the four reads), the rounding-mode names | `Opt:43-52, 170-178, 277-307, 326-369` | `RangeError`s; the reads (`StringOption`, `RoundingIncrement`, `UnitOption`, `IntegerWithTruncation`…) stay |

**Staying in Broiler.VM** in these sub-areas:
- the reads: `ToPartialDuration`, `ToDuration`, `RelativeToOption` (`Dur:108-172, 258-348`) and `Opt`'s reading members;
- `InterpretDateTimeFields` and `InterpretDateTimeOffset` (`Dur:376-381, 411-483`), which serve `relativeTo` and
  `ToTemporal…`; the zone half of the second is SJL-8's;
- the object factories (`Conv:31-116`).

**For SJL-10.5, the Broiler.JS copies these four sub-areas delete.** Every type listed is `internal` or `private`
except the `JSTemporal*` classes, which stay as shells (SJL-D-25).

- **Validity and limits**:
  - `J.PD:27-28, 801-805`, `J.NI:20-21`;
  - `J.PDT:27-29, 1167-1180`;
  - `J.PT:424-426`;
  - `J.PYM:765-783`;
  - `J.Ins:25-26`; `J.ZDT:30-31, 83`;
  - `J.DurA:562-585` (`RejectRelativeDateTimeRange`, `ValidateRelativeEndpoints`);
  - `J.Dur:321-370` (`DurationSign`, `RejectDuration`), `J.Dur:581-593` (`DefaultLargestUnitRank`);
  - `J.DurA:22, 61-73` (`HasCalendarUnits`, `DefaultLargestUnit`); `J.Rnd:25-34`; `J.PD:926-927`.
- **Regulation**:
  - `J.PD:770-786` (`RegulateISODate`);
  - `J.PDT:1020-1042` (`RegulateDateTime`);
  - `J.PT:447-466` (`RegulateTime`);
  - `J.PYM:746-760` (`RegulateYearMonth`).

  `J.PMD:547` (`RegulateMonthDay`) is the calendar half's.
- **Addition**:
  - `J.PD:815-841` (`AddISODate`), `J.PD:930-934`, `J.PD:1148-1153`, and `J.PD:1115-1122` (`AddYearsMonths`, unused);
  - `J.PDT:1190-1208`, `J.ZDT:893-917`;
  - the arithmetic of each `AddDuration`: `J.PD:284-314`, `J.PDT:389-425`, `J.PT:167-183`, `J.PYM:310-351`,
    `J.Ins:115-130`, `J.ZDT:838-878`;
  - `J.PDT:1098-1101`, `J.ZDT:882-888`, `J.Ins:403-409` (time-duration nanoseconds);
  - `J.Rnd:52-70`, `J.DurA:87-110` (`AddSubtract`, with the "not yet implemented" `Error`).

  `J.NI:453-500` (`AddToIso`) is the calendar half's.
- **Balancing**:
  - `J.PDT:533-548, 726-728, 739-747`;
  - `J.PT:258-288, 404-406`;
  - `J.Ins:202-222`;
  - `J.ZDT:1096-1115`;
  - `J.DurA:784-843` (`BalanceTimeOnly`, `BalanceFromNanoseconds`), `J.Dur:294-304`, `J.DurA:25, 295-297`;
  - `J.Rnd:422-470` (`BuildDuration`).
- **Small copies**:
  - `CompareISODate` at `J.PD:807-813`, `J.PDT:1182-1188`, `J.PYM:784-790`, `J.ZDT:1263-1269`;
  - `FloorDiv` at `J.PD:1163`, `J.PDT:1210-1222`, `J.PYM:792-797`, `J.Ins:421`, `J.ZDT:1748`, `J.DurA:288-293`;
  - the unit-length tables `J.PDT:1224-1232`, `J.PT:722`, `J.Ins:411`, `J.ZDT:984-992`, `J.DurA:32-42`, `J.Rnd:36-45`.
- **The separate relative-rounding implementations** (three):
  - plain-date `relativeTo`: `J.DurA:116-278` (the arithmetic of `RoundImpl` and `TotalImpl`), `J.DurA:550-557` and
    `587-781`;
  - zoned `relativeTo`: `J.Rnd:72-414`;
  - the day-axis nudge the differences use: `J.PD:901-924, 939-1092`, and `J.PDT:484-530` (`RoundTimeDifference`);
  - and `J.Dur:223-289` (`Compare`'s arithmetic).
- **`JSTemporalRoundingOptions`' arithmetic and rules**:
  - `NudgeTotal` and `NudgeRoundsToEnd` (`J.RO:160-179`) go;
  - the unit order, normalisation, maximum increment, increment validation and `Pow10` (`J.RO:27-66, 106-127, 152-158`)
    become the library's rules.

  `GetRoundingMode`, `ToIntegerWithTruncation`, `GetRoundingIncrement` and `GetFractionalSecondDigits` (`J.RO:68-104,
  129-150`) stay as adapter code.
- **Differences**:
  - `J.PD:326-357` (the arithmetic), `844-894`, `1096-1112`;
  - `J.PDT:437-478`, `1103-1163`;
  - `J.PT:243-254`; `J.Ins:141-198`;
  - `J.ZDT:924-1093`, `1214-1261`, `1271-1280`;
  - `J.PYM:374-430`.

  `J.NI:503-581` (`Difference`) is the calendar half's.
- **The settings' rules**: `J.PD:586-625`, `J.PDT:841-891`, `J.PT:195-237`, `J.Ins:145-190`, `J.ZDT:1128-1183`,
  `J.PYM:564-597`, `J.DurA:44-58`. Each keeps only its four reads, now one adapter.
- **Dead code**: `J.ZDT:829-830` (`NotImplementedArithmetic`) and `J.ZDT:1158-1168` (`ReadLargestUnit`) have no
  caller.

## 6. The milestone's listed corrections in these sub-areas

- **"Integer narrowing that wraps instead of throwing a `RangeError`": confirmed (R7).**
  - It wraps at `J.PDT:1193`, `J.PYM:347` and `J.NI:467, 480`.
  - It saturates at `J.PD:293-299`, `J.PDT:1098-1101` and `J.DurA:295-297`.
  - The constructors' narrowing saturates (.NET 9 and later) and so answers as the proposal does (R6) *(host)*.
- **"`Duration.prototype.add` with calendar units throwing 'not yet implemented'": confirmed (D3)**, when a
  `relativeTo` is passed. Without one, both engines throw `RangeError`. The same code also reads a second parameter
  that the pinned proposal does not have.
- **"Rounding options ignored in non-ISO differences": confirmed (F4)** for `PlainDate`, `PlainYearMonth` and
  `ZonedDateTime`. A non-ISO `PlainDateTime` applies them, but against ISO month lengths.
- **"Day rounding that ignores daylight-saving changes": confirmed (F7)** for `ZonedDateTime` `until` and `since`
  with a calendar smallest unit. It is refuted for `round`, `hoursInDay` and zoned-relative duration rounding and
  totals (D9).

**Found here and not in the area's table**:
- R6, Broiler.VM's faulting constructors: **the only correction of Broiler.VM**;
- B5, a year-month subtraction at the lower limit;
- D4, `compare`'s range check;
- D6 and D7, rounding to months re-derived through a clamped difference;
- D8, the text against the polyfill.

## 7. For a decision

1. **The pinned revision's text and its polyfill disagree on NudgeToCalendarUnit's nudged instant (D8).** The text
   gives P11M; the polyfill gives P1Y, though a year has not passed.
   - Broiler.VM follows the text. JSD-0054 section 4 names two other places where it follows the polyfill against the
     text.
   - The shared library, and the area's pin for both engines, must say which artifact rules where they disagree, and
     whether this becomes a third named place.
   - No line of the retained dataset reaches it. JSD-0054 section 6 records that Broiler.VM passes every scored case
     under `test/built-ins/Temporal` at `ccaac100`, so none of them asks for the polyfill's answer *(reading)*.
2. **The records.**
   - SJL-8 already published `IsoDate`, `IsoTime` and `IsoDateTime` in `Broiler.ECMAScript.Time` (`SL/Time/ZonedTime.cs:29-47`).
     It also published private copies of BalanceISODateTime and GetUTCEpochNanoseconds (`:531-580`).
   - The area design's target names `IsoDate`, `TimeOfDay` and `IsoDateTime` in `Broiler.ECMAScript.Temporal`.
   - Broiler.VM's time record carries overflow days (`Core:64`).
   - One set has to serve the zones, the core and both engines.
3. **The year's width and the shared preconditions (R6).**
   - Broiler.VM holds years as `long` and Broiler.JS as `int`.
   - The shared civil-day helpers throw beyond 2^53.
   - The library's limit checks have to take any year an engine converts, and answer a status rather than an exception.
     Otherwise every adapter must bound its years before calling, as Broiler.VM's bags do and its constructors do not.
