<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-10.1's correspondence from code, part one: the grammar, the calendars and serialization

**Written:** 2026-10-18, at SJL-10.1, as one half of the input of JSD-0078's X1, X2 and X6, and of the area's
comparison rules, before any code of SJL-10 exists. This half covers three of the verdict's seven sub-areas, **the
grammar, the calendars and serialization**, and **the proposal revision each engine follows**. The other half covers
records and limits, balancing, durations and rounding, and differences; where a row here touches those, it says so in
one line and leaves the row to that half.

**Read at** Broiler.VM `14072b0`, the `claude` branch, whose product sources are `047dcec`'s (the default branch after
SJL-8); and Broiler.JS `d3464413`, its default branch after SJL-8.7. The shared library is read in Broiler.JS at
`d3464413`; no file of it differs from `a04cd447`, from which the package `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.5` that Broiler.VM consumes was published. **By reading, and by running where a row says so.**
- A Broiler.VM cell marked *(run)* was run from the Release build of the checkout through the conformance root, as a
  raw test262 test in a test262-shaped directory (`correspondence-runs/syntax/vm262/`, whose `features.txt` is the
  pinned suite's and whose `harness/` was copied from the pinned suite `ccaac100` for the run and is not retained),
  under the manifest `broiler.javascript.wide`, whose realm has Intl and Temporal.
- A Broiler.JS cell marked *(run)* was run through the shell, `BroilerJS.dll --script-host`, built in Release, on .NET
  10.0.12, with no `LANG` and `TZ=UTC`, on Ubuntu 24.04.5.
- A cell marked *(host)* is behaviour that comes from .NET; *(reading)* is code not executed.

SJL-10.2's vectors and the retained reference dataset confirm or refute each row. **It holds no figure of a run**:
the values below are inputs, the answers they give, and counts of data.

**Paths.** In Broiler.VM, `VM/` is `src/Broiler.VM.Profile.JavaScript/` and `TD/` is `src/tests/temporal/`. In
Broiler.JS, `T/` is `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/`, and `AL/` is
`Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/`. `runs/` is
[`correspondence-runs/syntax/`](correspondence-runs/syntax/). It holds every probe (`probes/<name>.js`, with the shared
`prelude.js` and `epilogue.js`), the program each engine ran (`built/<name>.js`, and `vm262/test/sjl10/<name>.js` with
its test262 front matter), each engine's raw answer (`<name>.bjs.txt`, `<name>.vm.raw.txt` and the extracted
`<name>.vm.txt`) and the comparison of the two (`<name>.compare.txt`, its differing lines marked `!!`), named by row
group; `run-probe.sh` and `sidebyside.py` are SJL-8.1's harness, and `compare-retained.py` compares the retained
dataset's runs with the polyfill's answers (it writes `d-retained-*.compare.txt` in place of `sidebyside.py`, from
`TD/temporal.polyfill-e8cc03fc.txt`, `TD/calendars/temporal-calendars.polyfill-e8cc03fc.txt` and their
`divergences.txt`). A probe that compares acceptance prints a refusal as its error's type only; one that prints messages
says so, and its `!!` then also marks lines whose messages alone differ. A file over 100 KB is retained gzip-compressed
under its name with `.gz`.

**References.** "The proposal" is `tc39/proposal-temporal` at `e8cc03fc`, its specification text (`spec/*.html`)
read at that commit and cited by clause id. "The era proposal" is `tc39/proposal-intl-era-monthcode` at `5833eae`
(its Stage 4 draft of 19 March 2026, the revision SJL-8.1 read), cited by its section numbers and tables: Table 1
(calendar types and aliases), Table 2 (eras), Table 3 (month codes) and Table 6 (Chinese and Korean reference
years). Both histories were read from blobless clones taken on 2026-10-18 and are not retained. A line number is the
line at the revision above; a bare `:n` is in the file named last in the same cell or bullet.

## Facts

### Where each engine's code is

**Broiler.VM** keeps the three sub-areas in seven files:

| File | What | Coupling to the engine |
|---|---|---|
| `VM/JsTemporalParser.cs` (1,164 lines) | the grammar: a recursive-descent parser, one method per production (:220-811), `Parse` (:819-919), `IsTimeZoneIdentifier` (:928-932), `IsUtcOffset` (:937-941), `OffsetNanoseconds` (:949-975), the duration grammar `Duration` (:983-1163) | `Parse` takes a `JsEngine` only to construct `RangeError`s, at five sites (:856, :861, :868, :875, :886). Input a CLR `string`, output the neutral `JsTemporalParse` (:39-83); no other member names the engine |
| `VM/JsCalendars.cs` (942 lines) | every calendar but ISO 8601: years, months, month codes, eras, Table 6 (:867-899), over the shared year sources for the table calendars (:286-289) | **none**: no `JsEngine`, `JsValue` or throw of a JavaScript error |
| `VM/JsTemporal.Calendar.cs` (872 lines) | identifiers (:182-206), `ToCalendarIdentifier` and `ParseCalendarString` (:251-293), field preparation and month-code syntax (:323-435), field resolution, ISO regulation, from-fields, ISO add and until, `CalendarAnnotation` (:865-871) | 19 `engine.Error` sites; property reads in `CalendarWithIsoDefault` (:305) and `PrepareFields` (:342), coercion in `PrepareFields` (:355) and `MonthCodeOf` (:418); `ParseCalendarString` catches `JsThrow` (:284) |
| `VM/JsTemporal.CalendarNonIso.cs` (541 lines) | the era proposal's section 4.1: month-code validity and constraint (:80-100), `NonISOResolveFields` (:105-171), to-ISO (:176-189), month-day reference dates (:211-312), add (:370-389), until (:437-513), ISO-to-date (:518-540) | 18 `engine.Error` sites; the calendar systems are reached through `engine.Intl!.Calendars` (`VM/JsTemporal.Calendar.cs:211`), the Intl data Temporal is admitted only with (JSD-0054 section 3) |
| `VM/JsTemporal.Conversions.cs` (582 lines) | `ToTemporal*` from strings (:123-486) and the string forms (:493-575) | the `ToTemporal*` members are adapter code; of the string forms, `DateString`, `DateTimeString`, `YearMonthString` and `MonthDayString` are engine-free, `InstantString` and `ZonedDateTimeString` take the engine only to read a zone's offset |
| `VM/JsTemporalCore.cs` (723 lines; strings :576-722) | padding, `PadYear`, fractional seconds, time, date, offsets and duration strings | **none** |
| `VM/JsTemporal.Options.cs` (442 lines) | the readers of `calendarName`, `timeZoneName`, `offset`, `roundingMode`, `fractionalSecondDigits` and `smallestUnit` (:121-272), `ToOffsetString` (:424-441) | reads properties in specification order; the rule half (the allowed values, `Precision` :222-237) is engine-free |

The realm files hold the objects and each `toString` and `toJSON`: `VM/JsRealm.TemporalInstant.cs:104-123, 133-137`,
`VM/JsRealm.TemporalZonedDateTime.cs:203-223, 231-235`, `VM/JsRealm.TemporalDuration.cs:116-142, 144-147`,
`VM/JsRealm.TemporalPlainDate.cs:173-178, 186-190`, `VM/JsRealm.TemporalPlainDateTime.cs:172-196, 204-208`,
`VM/JsRealm.TemporalPlainTime.cs:118-135, 143-145`, and for `PlainYearMonth` and `PlainMonthDay`
`VM/JsRealm.TemporalYearMonthDay.cs:101-106, 114-118, 266-271, 279-283`.

**Broiler.JS** spreads the same responsibilities across its object classes, every member throwing through
`JSEngine.NewRangeError` and its siblings, which find the realm ambiently:

- **eight regular-expression parsers, one per type**, each returning an engine object: `ParseTemporalDateString`
  (`T/JSTemporalPlainDate.cs:739-767`), `ParseTemporalDateTimeString` (`T/JSTemporalPlainDateTime.cs:1053-1094`),
  `ParseTemporalTimeString` (`T/JSTemporalPlainTime.cs:588-657`), `ParseTemporalYearMonthString`
  (`T/JSTemporalPlainYearMonth.cs:700-742`), `ParseTemporalMonthDayString` (`T/JSTemporalPlainMonthDay.cs:479-541`),
  `ParseZonedDateTimeString` (`T/JSTemporalZonedDateTime.cs:1595-1744`), `ParseTemporalInstant`
  (`T/JSTemporalInstant.cs:442-504`) and `ParseTemporalDurationString` (`T/JSTemporalDuration.cs:410-453`);
- **a ninth, lenient parser** in `TemporalIsoString` (`T/TemporalIsoString.cs:233-293`) for calendar strings, time
  zone strings and `relativeTo`, and the annotation checks every parser shares (:125-192);
- **the calendars**: identifiers and the Gregorian family's eras (`T/JSTemporalCalendar.cs`), the formula calendars
  (`T/TemporalArithmeticCalendar.cs`), the arithmetic over the table calendars' shared years
  (`T/TemporalTableCalendar.cs`), their dispatch (`T/TemporalCalendarMath.cs`), and the month codes, field resolution,
  reference dates and arithmetic of every non-ISO calendar (`T/TemporalNonIso.cs`), with per-type month-code copies
  in five classes;
- **a formatter per type** (section 6 lists them).

**What is already shared, and so not SJL-10's move** (members both engines' Temporal code calls, counted by reading):

- SJL-6's civil-day helpers, `AL/Time/IsoCalendarMath.cs`: `IsLeapYear`, `DaysInYear`, `DaysInMonth`, `EpochDays`,
  `FromEpochDays`, `DayOfWeek`, `DayOfYear`, `WeekOfYear`, `BalanceYearMonth`, `IsValidIsoDate` (:62-234). Both
  parsers already validate a date with `IsoCalendarMath.IsValidIsoDate` (`VM/JsTemporalParser.cs:297`,
  `T/TemporalIsoString.cs:311`).
- SJL-8's table calendars' years: `CalendarYears.GetYear` over `CalendarTables`, `TableCalendar` and
  `MaxYearMagnitude` (`AL/Time/CalendarYears.cs:30-451`), read by `VM/JsCalendars.cs:286-289` and
  `T/TemporalTableCalendar.cs:37-42`; and its offsets: `OffsetTimeZones.FormatOffsetIdentifier`,
  `TryParseOffsetIdentifier` and `IsTimeZoneOffsetString` (`AL/Time/OffsetTimeZones.cs:46-107`), and the zone
  algorithms of `ZonedTime`.
- SJL-4's `ExactRounding` (`AL/Numbers/ExactRounding.cs:53-240`), which both engines' string rounding and Broiler.VM's
  duration-string conversion (`VM/JsTemporalParser.cs:1156`) call.
- **Not shared**: the grammar, the identifiers, month codes, eras and reference dates, the formula calendars, the
  arithmetic over the table calendars' years (month code to ordinal, the year a day falls in), and every string form.

### The proposal revision each engine follows

- **Broiler.VM follows `tc39/proposal-temporal@e8cc03fc970a65a3359e8870e3b35e687ac94e55`** (JSD-0053 section 6, the
  "Stage 4 Draft / July 27, 2026"), committed 2026-07-27 as "update status of fullcalendar/temporal-polyfill", a
  README change. **What it includes:** the proposal reached Stage 4 by 2026-03-12 (`06d3e081`, README). Its last
  normative changes to the specification are `41faca47` (2026-01-29, PlainYearMonth refuses adding weeks, days and
  time units) and `377fb80e` (2026-02-02, plain objects formatted without regard to daylight saving, an Intl change
  that is SJL-9's); every later commit to `spec/` is editorial, the last `e2da8c7e` (2026-03-30). Later commits fix
  the polyfill: among them `385b479b` (2026-02-26, a calendar annotation canonicalised before the YYYY-MM and MM-DD
  check, issue 3269) and `69a60a2a` (2026-05-11, out-of-range years in a PlainMonthDay bag refused early). It carries
  test262 `227d9055` (2026-05-21) as its submodule. For the era proposal, Broiler.VM follows Tables 1, 2, 3 and 6 as
  JSD-0056 builds them, which are the era proposal's at `5833eae` (SJL-8.1 read the same draft).
- **Both engines run test262 `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`** (2026-05-08): Broiler.VM's pin
  (`src/tests/conformance/pins/test262.pin`, JSD-0053 section 6) and Broiler.JS's evidence (its
  `.github/workflows/test262.yml` default and `docs/evidence/sjl-8-001/decision-rule.md:49`). It lies between the
  proposal's own pins: `d0c1b455` (2026-04-29, set by the proposal's `4df4199a` on 2026-05-11) is its ancestor, and
  `227d9055`, the one `e8cc03fc` carries, is 32 commits after it. Five of those 32 change Temporal tests, tests only:
  `7f2001f6` (not-yet-supported calendars), `b1f9a0ae` (a regression test), `0f323ada` (a feature tag), `a70493eb`
  (a spurious extra smallest unit in rounding) and `2fac7182` (ZonedDateTime at a repeated midnight).
- **Broiler.JS pins no revision.** Its code cites `proposal-temporal #3141` (`T/JSTemporalZonedDateTime.cs:1014-1015`,
  `T/JSTemporalZonedDateTime.Rounding.cs:129`), an issue the proposal fixed in `87af06d0` (2025-09-23), and Table 6
  (`T/TemporalNonIso.cs:101-104`), which the era proposal added in `ad5028b` (2026-01-21). **Its behaviour is
  `e8cc03fc`'s on every marker probed in these sub-areas but the era names**
  (`runs/r01-revision-markers.compare.txt`, *run* in both engines, 46 of 56 lines equal):
  - the API the proposal removed between 2024-06-13 and 2024-09-05 is absent in both: `Temporal.Calendar` and
    `Temporal.TimeZone` (`b8892420`), `getISOFields` (`7899d593`), `getCalendar` and `getTimeZone` (`92b19abd`),
    `withPlainDate` (`f7acbd2f`), `PlainTime#toPlainDateTime` (`f81d05fa`), epoch seconds and microseconds
    (`a6cffb25`), `PlainDateTime#toPlainYearMonth` (`efebfa2c`), `Temporal.Now.plainDate` (`2ae23dde`); and
    `getTimeZoneTransition` (`133921b2`) is present. `Temporal`'s own properties are the same nine names;
  - so is the behaviour: U+2212 refused (`ae9c7855`, 2024-07-04), years of a duration below 2³² (`1104cad8`), offset
    time zones to the minute (`70bc509f`), strict matching of a precise offset (`1117eaf8`, 2025-06-02), month-day
    reference years 1973 to 2035 and Table 6 (`10f83f39`, 2025-09-23; `ad5028b`), large dates refused in a non-ISO
    month-day string (`bd5ac121`), PlainYearMonth's refusal to add days, weeks or hours (`41faca47`, 2026-01-29), and
    both polyfill fixes above;
  - **the one exception in these sub-areas is the era names** (C9): Broiler.JS answers the era codes of the era
    proposal's 2025-06-30 change (`08de50a`) but also accepts, on input, codes that change replaced (`gregory`,
    `gregory-inverse`, `coptic`, `hebrew`, `ethiopic`, `ethioaa`, `saka`, `persian`), aliases the era proposal removed
    on 2025-11-21 (`684dd16`: `mundi`, `minguo`, `before-roc`), and `incarnation`, which no revision of the era proposal
    has (its alias was `incar`, from `1ff75a2`, 2023-01-27, to `684dd16`); it accepts none of `incar`, `minguo-qian`,
    `coptic-inverse`, `roc-inverse` or `japanese-inverse`. No single revision accepts what it accepts. The other
    differing marker, `Duration#add` with a calendar unit throwing a plain `Error` (`8ae0dbd7`), is the other half's.
- **The revision both engines' behaviour is closest to** is the proposal from `41faca47` (2026-01-29) onward, with
  the polyfill fixes of 2026-02-26 and 2026-05-11. No normative change to the specification separates `41faca47`
  from `e8cc03fc` but `377fb80e`, which is SJL-9's. So `e8cc03fc` and the era proposal at `5833eae` describe
  Broiler.VM on every row below, and Broiler.JS on every row but the corrections section 4 lists.
- **What could and could not be established.** The proposal's and the era proposal's histories and specification
  texts were read, and test262's commit graph (a treeless clone): established as above. Not established: which
  revision Broiler.JS's authors read, since nothing in its tree names one; whether the five test262 commits after
  `ccaac100` would change either engine's verdicts (not run here); and the polyfill's own answers to the probes, since
  the polyfill was not run (the retained dataset is its only reading here).

### Other facts

- **Both engines answer every probe's refusals with the same error type**, `RangeError` or `TypeError`, except where
  a row says otherwise; their messages differ throughout, which is each engine's choice (Broiler.VM names the
  operation, JSD-0054 section 5). A .NET exception inside Broiler.JS's parsers surfaces as a plain `Error` whose
  message is .NET's (G9, C6).
- **The retained datasets** (`TD/temporal.js`, 1,629 lines; `TD/calendars/temporal-calendars.js`, 3,999 lines) were
  run unchanged in both engines (`runs/d-retained-*`). Broiler.VM answers every line as the polyfill at `e8cc03fc`
  does or as `divergences.txt` names. Broiler.JS answers 7 lines of the first and 103 of the second otherwise. Of the
  103, 26 are this half's (C5, C15, C16, C17); the other 77 (`until` rounded to months, `Duration#round` and `total`
  with a non-ISO `relativeTo`) and all 7 of the first are the other half's (`runs/d-retained-*.compare.txt`).

## 1. The grammar

**Broiler.VM** parses the proposal's grammar (`#sec-temporal-iso8601grammar`) by recursive descent: one method per
production, each advancing over its text or restoring the position and failing (`VM/JsTemporalParser.cs:85-92`).
- Digits are ASCII only (:172); a year is four digits or an ASCII sign and six, `-000000` refused (:220-248); month
  and day are range-checked as the grammar's `DateMonth` and `DateDay` (:252-282), and a date that matches but is not
  a date fails as the early error (:297-305).
- `TimeSpec` tries the extended form, then the basic one, hour 00-23, minute 00-59, second 00-59 or 60, and a
  fraction of one to nine digits after a period or comma (:374-463); `UtcOffset` likewise, its sub-minute part only
  where allowed (:474-512).
- `Annotations` reads a time zone annotation only first, then key-value annotations with the grammar's key and value
  (:561-668); `Parse` then applies `ParseISODateTime`'s steps: the first `u-ca` wins, a critical one conflicting with
  another or an unknown critical key is a `RangeError`, and a year-month without a day or a month-day without a year
  in another calendar is a `RangeError` (:845-879).
- The goals (:710-811) are the grammar's: `Z` only where `DateTimeUTCOffset[+Z]` allows it (:685-705), the early
  errors that refuse a designator-less time that is also a month-day or year-month (:774-784).
- The duration grammar is a separate scanner that keeps each part exact as a `BigInteger` and converts it once, by
  `ExactRounding.ToDouble` (:983-1163).

**Broiler.JS** parses with a regular expression per type, over `RegexOptions.CultureInvariant` and so without
`RegexOptions.ECMAScript`: .NET's `\d` there matches every Unicode decimal digit, which `int.Parse` then refuses with a
`FormatException`. The annotation suffix is matched as `(?:\[[^\]]*\])*` (`T/TemporalIsoString.cs:68`) and checked
afterwards by three shared helpers: no several calendars where one is critical (:125-135), keys and values of the
grammar's shape (:145-158), at most one time zone annotation and no unknown critical key (:169-192). The time zone
annotation's own text is checked only for a sub-minute offset. Calendar strings, time zone strings and `relativeTo`
use a ninth parser, `TryParse` (:233-293), which peels annotations from the end.

In this section's table, a bare `:n` in a Broiler.VM cell is in `VM/JsTemporalParser.cs` unless the cell names another
file.

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| G1 | Years through every `from()`: `2020-01-01`, `+002020-01-01`, `-002020-01-01`, `-000000-01-01`, `+000000-01-01`, `+0020-01-01`, `02020-01-01`, `020-01-01`, `0000-01-01`, `-000001-12-31`, `±999999`, both limits and the day past each *(run)* | four digits or sign and six; `-000000` refused (`VM/JsTemporalParser.cs:220-248`); `+000000-01-01` is `0000-01-01`; `+999999-12-31` refused but by `PlainMonthDay` (`12-31`) | the same answers: each pattern's `\d{4}\|\+\d{6}\|-(?!000000)\d{6}` (`T/JSTemporalPlainDate.cs:731-735`, `T/JSTemporalPlainDateTime.cs:1048-1051`, `T/JSTemporalPlainYearMonth.cs:694-698`, `T/JSTemporalPlainMonthDay.cs:472-475`, `T/JSTemporalZonedDateTime.cs:1579-1587`, `T/JSTemporalInstant.cs:434-440`) | **same**. `DateYear` and its early error. `runs/g01-dates.compare.txt` |
| G2 | Date separators: `20200101`, `2020-0101`, `202001-01`, `+0020200101` *(run)* | both separators or neither (:291-309) | the same (two alternatives in each pattern) | **same**. `DateSpec[Extended]`. `runs/g01-dates.compare.txt` |
| G3 | A year-month string with a day that is not a date: `2020-02-30`, `2020-01-00`, `2020-01-32`, `2021-02-29`, `2020-04-31`, `1900-02-29`, and `2020-02-30[u-ca=gregory]` *(run)* | `RangeError`: the date is the grammar's (`DateSpec`'s early error, :297-305) | accepted: `2020-02`, `2020-01`, …, `2020-02-01[u-ca=gregory]`. The day is captured and never validated for ISO 8601 or the Gregorian family (`T/JSTemporalPlainYearMonth.cs:700-742`, only non-ISO calendars check it at :733-739) | **correction of Broiler.JS** (lenient acceptance; the area's table). `#sec-temporal-iso8601grammar-static-semantics-early-errors`. `runs/g01-dates.compare.txt`, `runs/g05-goals.compare.txt` |
| G4 | A time from a date-time whose date is not a date: `2021-13-45T12:00`, `2020-13-01T12:00`, `2020-02-30T12:00`, `2021-02-29T12:00`, `2020-00-01T12:00`, `2020-01-32T12:00` *(run)* | `RangeError` (`TemporalTimeString` reaches the date through `AnnotatedDateTime`, :833) | `12:00:00` for each: `DateTimeFormPattern` matches any two digits (`T/JSTemporalPlainTime.cs:549-551`) and the date is never validated (:588-657) | **correction of Broiler.JS** (the area's table). `runs/g05-goals.compare.txt`, `runs/g11-more.compare.txt` |
| G5 | Times: hour alone, `hhmm`, `hh:mm:ss`, mixed separators, fractions of 1, 9 and 10 digits, comma, `:60`, `24:00`, a fraction on minutes or hours, `T`, `t`, space, `TT`; alone for `PlainTime` and after a date in every goal *(run)* | the grammar's (:374-463): `12` and `T12` are `12:00:00`, `12:30:45,5` is `12:30:45.5`, `12:30:60` is `12:30:59`; `12:3045`, `1230:45`, `24:00`, `12:30.5` and ten digits refused | the same answers in every plain goal and `ZonedDateTime` (`T/TemporalIsoString.cs:36-45`) | **same**. `#sec-temporal-parseisodatetime`. `runs/g02-times.compare.txt` |
| G6 | `Instant.from("2020-01-01T12:3045Z")` and `"…T1230:45Z"` *(run)* | `RangeError`: one form per time (:407-463) | `2020-01-01T12:30:45Z` for both: `InstantPattern` makes each separator optional on its own, `:?` (`T/JSTemporalInstant.cs:437`) | **correction of Broiler.JS**: "each such part is itself in either basic format or extended format". `runs/g02-times.compare.txt` |
| G7 | Designator-less times that are also month-days or year-months: `2021-12`, `202112`, `1214`, `0229`, `1230`, `--1214`; and `0230`, `1232`, `2021-13`, `2020-0101` *(run)* | refused, refused; and `02:30:00`, `12:32:00`, `20:21:00`, `20:20:00` (:774-784, with `IsValidMonthDay` on the reference leap year :352-359) | the same (`T/JSTemporalPlainTime.cs:661-684`) | **same**. The early errors of `AnnotatedTime`. `runs/g02-times.compare.txt` |
| G8 | Offsets after a time in every goal: `Z`, `z`, `±00:00`, `-00`, `+01`, `+0100`, `+01:00:00`, `+01:00:00.5`, `,5`, nine and ten fraction digits, `+01:0000`, `+0100:00`, `+1`, `+001`, `+01:00Z`; and `+24:00`, `+23:60`, `+23:59:60` *(run)* | `Z` refused in the plain goals; the rest as the grammar (:474-512, :685-705); `+24:00`, `+23:60`, `+23:59:60` refused everywhere | the same, but `+24:00`, `+23:60` and `+23:59:60` are **accepted** by `PlainDate`, `PlainDateTime`, `PlainYearMonth` and `PlainMonthDay`, whose offset is matched by `OffsetCore` without a range (`T/TemporalIsoString.cs:31-32, 43-45`); `PlainTime`, `ZonedDateTime` and `Instant` range-check theirs (`T/JSTemporalPlainTime.cs:576-586`, `T/JSTemporalZonedDateTime.cs:1680`, `T/JSTemporalInstant.cs:489`) | **correction of Broiler.JS** for the four plain types (lenient acceptance); same otherwise. `UTCOffset`'s `Hour` and `MinuteSecond`. `runs/g03-offsets.compare.txt` |
| G9 | Decimal digits other than ASCII (Arabic-Indic, fullwidth) in the year, a day, a minute, a fraction, an offset, a duration part, in each parser and in `relativeTo` *(run)* | `RangeError` everywhere: `IsDigit` is ASCII (:172), as the grammar's `DecimalDigit` | an `Error` whose message is .NET's `System.FormatException: The input string '٢٠٢٠' was not in a correct format.` from every parser and `relativeTo` *(host)*; in the offset of the four plain types the digits are **accepted** and the offset ignored (`2020-01-01T12:00+٠١:٠٠` is `2020-01-01`) | **correction of Broiler.JS**, as SJL-8.1's A13 was for zone offsets. `runs/g10-nonascii.compare.txt`, `runs/g03-offsets.compare.txt`, `runs/g12-relativeto.compare.txt` |
| G10 | U+2212 MINUS SIGN as the sign of a year, an offset, an offset annotation and a duration, in each parser *(run)* | `RangeError` (`EatAny("+-")`, :224, :478, :988) | `RangeError` in each, by an explicit check in three parsers (`T/JSTemporalPlainDate.cs:743-744`, `T/JSTemporalPlainYearMonth.cs:704-705`, `T/JSTemporalPlainMonthDay.cs:483-484`) and the patterns' `[+-]` elsewhere | **same** (`ae9c7855`, 2024-07-04). Broiler.JS's handling of U+2212 after a match (`Replace('−', '-')` at `T/JSTemporalPlainDate.cs:756`, `T/JSTemporalPlainDateTime.cs:1069`, `T/JSTemporalPlainYearMonth.cs:717`, `T/JSTemporalZonedDateTime.cs:1635`; `"−"` signs at :1564, :1676 and `T/JSTemporalDuration.cs:427`) is **code no input reaches**. `runs/g10-nonascii.compare.txt` |
| G11 | An empty or flag-only annotation, `[]` and `[!]`, after a date-time in each goal, and `PlainDate.from("2020-01-01[]")` *(run)* | `RangeError`: neither a time zone identifier nor a key-value pair (:561-619) | **accepted** in `PlainDate`, `PlainDateTime`, `PlainTime`, `PlainYearMonth`, `PlainMonthDay` and `Instant` (the empty text counts as a time zone annotation and is not checked, `T/TemporalIsoString.cs:169-192`); in `ZonedDateTime` and `relativeTo` a `RangeError` for an unknown zone `""` | **correction of Broiler.JS** (the area's table). `Annotations`. `runs/g04-annotations.compare.txt`, `runs/g05-goals.compare.txt` |
| G12 | A time zone annotation the grammar refuses, after a plain date-time and an instant: `[Mars Olympus]`, `[ UTC]`, `[UTC ]`, `[/a]`, `[a/]`, `[a//b]`, `[9a]`, `[+24:00]` *(run)* | `RangeError` (`TimeZoneIdentifier`, :516-556) | **accepted**: only a sub-minute offset is checked (`T/TemporalIsoString.cs:182-184`) | **correction of Broiler.JS**. `TimeZoneIANAName`, `UTCOffset[~SubMinutePrecision]`. Names the grammar admits but no zone has (`[Mars/Olympus]`, `[.]`, `[..]`, `[a..b]`) are accepted by both outside `ZonedDateTime`. `runs/g04-annotations.compare.txt` |
| G13 | A time zone annotation after another annotation: `[u-ca=iso8601][UTC]`, `[foo=bar][UTC]`, in every goal, as a `ZonedDateTime` string and as `relativeTo` *(run)* | `RangeError`: the zone is read only first (:597-601) | **accepted** everywhere, the zone taken from wherever it stands (`T/JSTemporalZonedDateTime.cs:1606-1624`, `T/TemporalIsoString.cs:240-262`); `ZonedDateTime.from("2020-01-01T00:00[u-ca=iso8601][UTC]")` is `2020-01-01T00:00:00+00:00[UTC]` and the same `relativeTo` rounds | **correction of Broiler.JS** (the area's "wrong order … partly checked"). `AnnotatedDateTime` puts `TimeZoneAnnotation` before `Annotations`. `runs/g04-annotations.compare.txt`, `runs/g05-goals.compare.txt`, `runs/g12-relativeto.compare.txt` |
| G14 | Critical flags, keys and values: `[!u-ca=…]`, `[!UTC]`, several calendars with and without a critical one, `[foo=bar]`, `[!foo=bar]`, `[U-CA=…]`, `[_x=y]`, `[1a=b]`, `[a=b--c]`, `[a=b_c]`, `[a=b=c]`, two time zone annotations *(run)* | the first `u-ca` wins; any critical one among several, an unknown critical key, a key or value off the grammar, a second zone: `RangeError` (:580-614, :623-668, :845-863) | the same in every `from()` (`T/TemporalIsoString.cs:125-192`) | **same** in the `from()`s; the calendar and time zone string parsers differ (G17, G18). `runs/g04-annotations.compare.txt` |
| G15 | Goal by goal: year-month `202001`, `+00202001`, `2020-01[u-ca=gregory]`, `2020-01[u-ca=ISO8601]`, the limits; month-day `0101`, `--01-01`, `--0101`, `-01-01`, `02-29`, `02-30`, `11-18[+01:00][u-ca=ISO8601]`, `01-01[u-ca=gregory]`; zoned date-only, `Z` with an annotation, precise offsets that do and do not match (Monrovia `-00:44:30`, `-00:44`, `+01:00:00.000000001`); instant without offset, at and past both limits *(run)* | as the grammar and `ParseISODateTime`; the ISO 8601 check is ASCII-case-insensitive (:866, :873) | the same answers on all of them | **same**, and consistent with `385b479b` and `1117eaf8`. `runs/g05-goals.compare.txt` |
| G16 | A time on a year-month: `2020-01T12:00`, `202001T1200`, `2020-01 12:00`, `2020-01T12:00[u-ca=iso8601]` *(run)* | `RangeError`: `AnnotatedYearMonth` has no time (:799-803) | **accepted** as `2020-01`: the time tail follows an optional day (`T/JSTemporalPlainYearMonth.cs:695-698`) | **correction of Broiler.JS**. A month-day with a time is refused by both. `runs/g11-more.compare.txt` |
| G17 | Calendar strings (`ParseTemporalCalendarString`), as a bag's `calendar`, `withCalendar` and the constructor: `T10:00Z`, `10:00Z`, `2020-01[u-ca=gregory]`, `01-01[u-ca=gregory]`, `2020-01-01[]`, `…[u-ca=gregory][UTC]`, `…[!foo=bar]`, `…[u-ca=gregory][!u-ca=japanese]`, `…T12:00+25:00[u-ca=gregory]`, `…[Mars Olympus][u-ca=gregory]`, `1214[u-ca=gregory]`; and `[u-ca=gregory][u-ca=japanese]` *(run)* | `RangeError` for each, by `ParseISODateTime`, then not an `AnnotationValue` (`VM/JsTemporal.Calendar.cs:269-293`); the last answers `gregory`, the first annotation | **accepted** (`iso8601`, `gregory` or `japanese`) by `TryParse`, which refuses none of these (`T/TemporalIsoString.cs:233-293`, reached from `T/JSTemporalCalendar.cs:156-165`); `[u-ca=gregory][u-ca=japanese]` answers `japanese`, the **last** (`T/TemporalIsoString.cs:250-262` walk the peeled list outermost first). The constructor takes no ISO string in either | **correction of Broiler.JS**. `#sec-temporal-parsetemporalcalendarstring`. `runs/g07-calendar-strings.compare.txt`, `runs/g11-more.compare.txt` |
| G18 | Time zone strings (`ParseTemporalTimeZoneString`), through `toZonedDateTimeISO` and a bag's `timeZone`: `T10:00Z`, `10:00Z`, `1214Z`, `…[u-ca=gregory][Europe/Berlin]`, `…[foo=bar][Europe/Berlin]`, `…Z[u-ca=gregory][!foo=bar]`, `…Z[Europe/Berlin][UTC]` *(run)* | `RangeError` for each (`VM/JsTemporal.Zones.cs:69-110`) | `UTC`, `UTC`, `UTC`, `Europe/Berlin`, `Europe/Berlin`, `UTC`, and `UTC` (the last annotation): `CanonicalizeTimeZone` falls back to `TryExtractTimeZone` (`T/JSTemporalZonedDateTime.cs:1379-1389`, `T/TemporalIsoString.cs:353-363`) | **correction of Broiler.JS**. `#sec-temporal-parsetemporaltimezonestring`. Offsets, sub-minute offsets and annotations otherwise agree (39 inputs × 3 entry points). `runs/g08-timezone-strings.compare.txt`, `runs/g11-more.compare.txt` |
| G19 | Offset strings (`ToOffsetString`), as `with({offset})` and a bag's `offset`: `Z`, `z`, `+25:00`, `+24:00`, `+01:00:60`, `+01:0000`, U+2212, Arabic-Indic digits, `""`, `UTC`; a number, `null`, `true`, a BigInt *(run)* | `Z` and `z` `RangeError`; the rest as `UTCOffset[+SubMinutePrecision]` (`VM/JsTemporal.Options.cs:424-441`) | `Z` and `z` **accepted** (`T/JSTemporalZonedDateTime.cs:1557`); the rest the same, `+25:00` refused since SJL-8.7 put the shared `IsTimeZoneOffsetString` in front (:1561) | **correction of Broiler.JS** for `Z` (the area's table names `Z` and `+25:00`; `+25:00` no longer differs). `#sec-temporal-tooffsetstring`. `runs/g09-offset-strings.compare.txt` |
| G20 | `relativeTo` strings in `Duration#round` and `total`: dates, date-times, zoned forms, `Z` without an annotation, `[]`, annotation order, `+25:00`, `+01:0000`, a year-month, a month-day, a time, `2020-02-30`, `T24:00`, non-ASCII digits, U+2212, a non-ISO calendar *(run)* | the grammar with `TemporalDateTimeString[+Zoned]` and `[~Zoned]` (`VM/JsTemporal.Durations.cs:315`) | `TryParseRelative`, then the zoned or the date parser (`T/JSTemporalDuration.Arithmetic.cs:333-356`): annotation order and `+25:00` **accepted** (G8, G13); non-ASCII digits an `Error` (G9); every other input the same type of answer | **correction of Broiler.JS** where G8, G9 and G13 are. A non-ISO `relativeTo` (`Error`, "not yet implemented") is the other half's. `runs/g12-relativeto.compare.txt` |
| G21 | Duration strings: designators in either case, part order, weeks with other parts, a fraction on the last time part only, `P`, `PT`, signs, spaces, repeated parts, `PT46H66M71.50040904S`, each field at and past its limit *(run)* | the grammar (:983-1163) | the same answers on 68 of 73 strings (`T/JSTemporalDuration.cs:406-453`), the other five being G22 and G9; the fields of five read back equal | **same**. `#sec-temporal-parsetemporaldurationstring`. `runs/g06-durations.compare.txt` |
| G22 | `P1YT`, `P1DT`, `P1Dt`: a time designator with no time part *(run)* | `RangeError` (:1130) | **accepted** as `P1Y`, `P1D`: the pattern's time group has only optional parts (`T/JSTemporalDuration.cs:407`) and the check at :420 asks only for one part anywhere | **correction of Broiler.JS**. `DurationTime` needs a part. `runs/g06-durations.compare.txt` |
| G23 | Exactness of duration digits: every limit, `PT9007199254740991.999999999S`, `PT2501999792983.999999999H`, `P00000000000000000000001D` *(run)* | each part a `BigInteger`, converted once by `ExactRounding.ToDouble` (:1042, :1156) | each part `double.Parse` (`T/JSTemporalDuration.cs:416`), fractions exact (:456-461) | **same**: the specification reads each part by `ToIntegerWithTruncation` of `StringToNumber`, round to nearest, which both give; every field a valid duration admits is below 2⁵³ and exact in both. The area's "exact (`BigInteger`) / `double.Parse`" is true of the code and of no answer. `runs/g06-durations.compare.txt` |
| G24 | When `ZonedDateTime.from` reads its options against its string: a valid string, a mismatched offset, an invalid date, zone, calendar, annotation or offset, with logging option getters *(run)* | no option read before any of the string's errors; then `disambiguation`, `offset`, `overflow` | the same reads (`T/JSTemporalZonedDateTime.cs:1694-1699`) | **same**. The area's "reads options part-way through the parse" describes the code's shape (the options are read inside the parser, after its checks), not an observable order. `runs/g13-zoned-option-reads.compare.txt` |

## 2. The calendars

**Broiler.VM** canonicalises an identifier by ASCII lower-casing against Table 1's types and two aliases
(`VM/JsTemporal.Calendar.cs:182-206, 220-233`). Every calendar but ISO 8601 is a `JsCalendarSystem`
(`VM/JsCalendars.cs:138-310`) that answers a year's months, its leap month, its eras and Table 6
(`:318-421, 429-517, 525-560, 570-609, 619-698, 707-798, 809-900`), the table calendars' years from the shared sources
(:286-289). The era proposal's section 4.1 runs over them: a month code's syntax at the field read (`MonthCodeOf`,
`VM/JsTemporal.Calendar.cs:416-435`), its validity for the calendar (`IsValidMonthCode`,
`VM/JsTemporal.CalendarNonIso.cs:80-82`), the constraint of a missing leap month (:87-100), field resolution with era
years (:105-171), and the month-day reference date: Table 6 for `chinese` and `dangi`, else the latest date in
1900-1972 or the earliest in 1973-2035 (:211-312).

**Broiler.JS** canonicalises with `ToLowerInvariant` against a list that adds two aliases
(`T/JSTemporalCalendar.cs:87-127`). The Gregorian family is ISO arithmetic with a year offset and eras
(`T/JSTemporalCalendar.cs:177-293`); the formula calendars are `T/TemporalArithmeticCalendar.cs`; the table calendars'
arithmetic is `T/TemporalTableCalendar.cs`, over the shared years. `T/TemporalNonIso.cs` resolves fields, constrains,
and finds a month-day's reference date by a search over calendar years: 1900 to 2035 for `chinese` and `dangi`, 1900
to 2100 for the others (:105-187). Month-code syntax is checked by `^M\d{2}L?$` at the field read
(`T/TemporalIsoString.cs:85-92`, and copies at `T/JSTemporalPlainDate.cs:534-540`,
`T/JSTemporalPlainDateTime.cs:794-800`).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| C1 | Every identifier of Table 1, the aliases `ethiopic-amete-alem` and `islamicc`, in any ASCII case, by constructor and annotation; `islamic`, `islamic-rgsa`, near-misses (`iso-8601`, a trailing space or NUL, U+0130, a Cyrillic `а`); `Intl.supportedValuesOf("calendar")` *(run)* | the 16 types and two aliases, canonicalised (`VM/JsTemporal.Calendar.cs:182-206`); the rest `RangeError` | the same answers (`T/JSTemporalCalendar.cs:87-127`), the same 16 names listed | **same**. `#sec-temporal-canonicalizecalendar` over the era proposal's 1.1.1. `runs/c01-identifiers.compare.txt` |
| C2 | `gregorian` and `minguo` in any case, by constructor, annotation, bag and `withCalendar` *(run)* | `RangeError` | **accepted** as `gregory` and `roc` (`T/JSTemporalCalendar.cs:94, 97`; also `IsSupported`, :167-173) | **correction of Broiler.JS**: AvailableCalendars "must consist of the 'Calendar Type' value of every row of" Table 1, whose aliases are `ethiopic-amete-alem` and `islamicc` only (era proposal 1.1.1). `runs/c01-identifiers.compare.txt`, `runs/g07-calendar-strings.compare.txt` |
| C3 | `islamic`, which `TemporalArithmeticCalendar` lists *(reading; refusal run)* | not a calendar | listed as arithmetic with the civil epoch (`T/TemporalArithmeticCalendar.cs:27-29, 47`), but no identifier reaches it: canonicalisation refuses `islamic` (`T/JSTemporalCalendar.cs:109-112`) | **code no input reaches**. `runs/c01-identifiers.compare.txt` |
| C4 | Month codes in each of the 16 calendars at a leap year, under `constrain` and `reject`: `M00`, `M00L`, `M01`, `M01L`, `M02L`, `M05L`, `M06L`, `M12`, `M12L`, `M13`, `M13L`, and the malformed `m01`, `M1`, `M001`, `M01l`, `L01M`, `M01 `, `""` *(run)* | the grammar's `MonthCode` at the read (`M00` refused, `M00L` passes, `VM/JsTemporal.Calendar.cs:426-427`), then Table 3's validity (`VM/JsTemporal.CalendarNonIso.cs:80-82, 151-157`) | the same answers on 675 of 704 lines; the 29 others are C5 | **same** but C5. `#sec-temporal-parsemonthcode`; era proposal 4.1.4. `runs/c02-month-codes.compare.txt` |
| C5 | **Invalid month codes under `constrain`**: `M00`, `M14`, `M99` in `coptic`, `ethiopic`, `ethioaa`; `M00`, `M13`, `M14`, `M99` in `indian`, `islamic-civil`, `islamic-tbla`, `islamic-umalqura` and `persian` *(run)*; the retained dataset's `from M13` in five calendars *(run)* | `RangeError` in each (`VM/JsTemporal.CalendarNonIso.cs:151-157`) | **constrained**: `M00` to `M01`, the others to the last month (`persian 1403 M13` is 1403/`M12`/1). `^M\d{2}L?$` admits them (`T/TemporalIsoString.cs:85-92`); `OrdinalFromMonthCode` returns the number unchecked outside Hebrew and the lunisolar calendars (`T/TemporalArithmeticCalendar.cs:158-165`, `T/TemporalTableCalendar.cs:116-121`), and `RegulateToIso` clamps it (`T/TemporalNonIso.cs:396-416`). Under `reject` both refuse | **correction of Broiler.JS**: NonISOResolveFields throws a `RangeError` where IsValidMonthCodeForCalendar is false (era proposal 4.1.4, 4.1.24). Not in ISO 8601, the Gregorian family, Hebrew, `chinese` or `dangi`. `runs/c02-month-codes.compare.txt`, `runs/d-retained-temporal-calendars.compare.txt` |
| C6 | A month code with decimal digits other than ASCII: `M٠١`, `M0١`, `M０１`, `M٠٥L`, in six calendars, `PlainMonthDay` and `PlainYearMonth` *(run)* | `RangeError` (`VM/JsTemporal.Calendar.cs:426-427`) | `RangeError` for ISO 8601 and the Gregorian family; an `Error` with .NET's `FormatException` for Hebrew, `chinese`, `coptic`, `persian`, the Hebrew month-day and the ISO year-month *(host)*: the syntax check's `\d` admits the digits and `int.Parse` refuses them (`T/TemporalNonIso.cs:600-606`, `T/JSTemporalPlainYearMonth.cs:532-545`) | **correction of Broiler.JS**. `MonthCode`'s digits are ASCII. `runs/c10-monthcode-digits.compare.txt` |
| C7 | Leap month codes in years without that month: Hebrew 5783 `M05L`, `chinese` and `dangi` 2024 `M02L`, `chinese` 2023 `M03L`; month and month code together; month 14; the month codes of each month of a leap year *(run)* | `M05L` to `M06`, `M02L` to `M02` under `constrain`, `RangeError` under `reject`; Hebrew 5784 `M01…M05 M05L M06…M12` | the same | **same**. Era proposal 4.1.6 (Hebrew skips forward). `runs/c02-month-codes.compare.txt` |
| C8 | Eras: every era of Table 2 in every calendar with `eraYear` 5; the era and era year of 29 ISO dates at era edges (Meiji 1868/1873, Taishō, Shōwa, Heisei, Reiwa, ROC 1912, the Coptic, Ethiopic, Indian, Hijri epochs, year 0, the limits) in 15 calendars; `eraYear` zero and negative; year against era; era or `eraYear` alone *(run)* | Table 2's codes and arithmetic years (`VM/JsCalendars.cs:364-420, 500-516, 555-559, 604-608, 687-697, 793-797`), Japanese eras from Meiji 6 (:373-399) | the same answers on 1,036 of 1,060 lines (`T/JSTemporalCalendar.cs:177-293`, `T/TemporalArithmeticCalendar.cs:190-233`, `T/TemporalTableCalendar.cs:140-158`) | **same** but C9. Era proposal 4.1.2, 4.1.10-4.1.13. `runs/c03-eras.compare.txt` |
| C9 | **Era names**: `CE`, `Ce`, `AM`, `AH`; `gregory`, `gregory-inverse`, `minguo`, `before-roc`, `coptic`, `hebrew`, `incarnation`, `ethiopic`, `mundi`, `ethioaa`, `saka`, `persian`; and `incar`, `minguo-qian`, `roc-inverse`, `coptic-inverse`, `japanese`, `japanese-inverse`, `islamicc` *(run)* | `RangeError` for each: an era is matched exactly (JSD-0056 section 3) | the first two groups **accepted**: era names are lower-cased (`ToLowerInvariant`, `T/JSTemporalCalendar.cs:257`, `T/TemporalArithmeticCalendar.cs:203`, `T/TemporalTableCalendar.cs:145`) and matched against Table 2 and older names (`T/JSTemporalCalendar.cs:263-275`, `T/TemporalArithmeticCalendar.cs:207-223`, `T/TemporalTableCalendar.cs:148`); the third group refused. The answer is Table 2's code (`gregory-inverse` 5 is `bce` 5) | **correction of Broiler.JS**: CanonicalizeEraInCalendar (era proposal 4.1.2) admits Table 2's codes and aliases. The accepted names are the era proposal's before `08de50a` (2025-06-30) and its aliases before `684dd16` (2025-11-21), in part; `incarnation` is in none. `runs/c03-eras.compare.txt`, `runs/r01-revision-markers.compare.txt` |
| C10 | **Month-day reference dates**: every month code of each of the 16 calendars (Table 3's leap months included) with days 1, 29, 30, 31 under `reject` and 30, 31, 32 under `constrain`; Table 6's `M11L` split at day 10; a year given with the month-day; a month without a year; a year past the limits *(run)* | Table 6 as a table for `chinese` and `dangi` (`VM/JsCalendars.cs:867-899`), else the latest date in 1900-1972 or the earliest in 1973-2035 (`VM/JsTemporal.CalendarNonIso.cs:211-312`): Hebrew `M05L` 30 is 1970-03-08, `chinese` `M11L` 1 is 2033-12-22 and `M11L` 11 2034-01-01, `islamic-civil` `M12` 30 1971-02-26 | **the same answers on all 1,670 lines**, by a search with no table (`T/TemporalNonIso.cs:105-291`), whose span reproduces Table 6 since SJL-8.6 | **same** in every answer probed; the code differs (search against table). Era proposal 4.1.21 and Table 6 (`ad5028b`). The search's upper year 2100 for calendars other than `chinese` and `dangi` (`T/TemporalNonIso.cs:111, 125-129`) answers nothing a 2035 bound would not: every such month-day occurs by 1972 (**code no input reaches**). `runs/c04-reference-years.compare.txt.gz` |
| C11 | Calendar fields beyond 32 bits: `year` 2³²+y, `day` and `month` 2³²+1 under both settings, `eraYear` 2³²+5, in eight calendars, `PlainDate`, `PlainYearMonth` and `PlainMonthDay` *(run)* | `RangeError` for every year, the day and month constrained | the same answers: `(int)` of a `double` saturates on .NET 9 and later (`T/JSTemporalPlainDate.cs:486-492`, `T/TemporalNonIso.cs:584-588`) *(host)*, so no value wraps | **same**. No narrowing that wraps reaches a calendar field. `runs/c07-slot-and-narrowing.compare.txt` |
| C12 | An ISO month-day's year beyond 2³¹, where it decides only whether 29 February exists: `{year: 2147483652, month: 2, day: 29}`, and 2147483648, 2³², 10¹⁰, 10¹⁵, under both settings, and through `with()` *(run)* | `02-29`: the year is reduced modulo 400 past 10⁹ and keeps its leap year (`VM/JsTemporal.Calendar.cs:654-662`) | `RangeError` under `reject`, `02-28` under `constrain`: the year saturates to 2,147,483,647, which is not a leap year (`T/JSTemporalPlainMonthDay.cs:286-290, 414-417`) *(host)* | **correction of Broiler.JS**: CalendarMonthDayToISOReferenceDate regulates against the given year. The milestone's integer narrowing, as it reaches a calendar field: saturating, not wrapping. `runs/c08-saturation.compare.txt` |
| C13 | The calendar a `PlainMonthDay` lends as a `calendar` value: a Hebrew one in a `PlainDate` bag, `withCalendar` and a `PlainMonthDay` bag; and the other Temporal types *(run)* | `hebrew` (`VM/JsTemporal.Calendar.cs:238-246`) | `iso8601` for any `PlainMonthDay` (`T/JSTemporalCalendar.cs:143`); the other types agree | **correction of Broiler.JS**: ToTemporalCalendarIdentifier returns the object's `[[Calendar]]`. `runs/c07-slot-and-narrowing.compare.txt` |
| C14 | The formula calendars at their edges: 46 ISO dates (both limits, years 0 and 1, each epoch, new years and leap days around 2020, 1582, 1900, 2100) in `gregory`, `buddhist`, `roc`, `japanese`, `coptic`, `ethiopic`, `ethioaa`, `indian`, `islamic-civil`, `islamic-tbla`, `hebrew`, every field; the round trip at both limits and a day past them; Hebrew years of all six lengths *(run)* | formulas (`VM/JsCalendars.cs:318-798`) | the same answers on every line (`T/TemporalArithmeticCalendar.cs`, `T/JSTemporalCalendar.cs:177-214`); the 22 lines marked differ in their messages only | **same**, as SJL-8.1's K16 found on the retained dataset. `runs/c05-formula-edges.compare.txt` |
| C15 | **The year-month a date gives**: `PlainDate#toPlainYearMonth` of 2023-03-15 in each calendar, and through `PlainDateTime#toPlainDate` *(run)*; the retained dataset's 15 `toPlainYearMonth` lines *(run)* | the first day of the calendar month (`chinese` 2023-02-20, `hebrew` 2023-02-22, `gregory` 2023-03-01) | **the date's own ISO day**, 2023-03-15, in every calendar but ISO 8601 (`T/JSTemporalPlainDate.cs:417-424`). A year-month built from fields agrees | **correction of Broiler.JS**: `toPlainYearMonth` calls CalendarYearMonthFromFields "to create a PlainYearMonth object with the [[Day]] field … set correctly" (`#sec-temporal.plaindate.prototype.toplainyearmonth`). `runs/c09-conversions.compare.txt` |
| C16 | **The month-day a date gives in the Gregorian family**: `PlainDate#toPlainMonthDay` in `gregory`, `buddhist`, `roc`, `japanese`, and `with()` on it *(run)*; the retained dataset's 4 lines *(run)* | the date's calendar, reference 1972 | **`iso8601`**: the month-day is built without a calendar (`T/JSTemporalPlainDate.cs:441`) | **correction of Broiler.JS**. `#sec-temporal.plaindate.prototype.toplainmonthday`. `runs/c09-conversions.compare.txt` |
| C17 | **The date a month-day gives in `buddhist` and `roc`**: `toPlainDate({year})` with the calendar year of 2001-06-01 *(run)*; the retained dataset's 2 lines *(run)* | 2001-02-10 in both | ISO 2544-02-10 and 0090-02-10: the calendar year is read as an ISO year (`T/JSTemporalPlainMonthDay.cs:239-246`) | **correction of Broiler.JS**. `#sec-temporal.plainmonthday.prototype.toplaindate`. `runs/c09-conversions.compare.txt` |
| C18 | The table calendars at their tables' edges: `chinese` and `dangi` 1899-1901, 2050-2051, 2100-2103; Umm al-Qura 1299-1301 and 1599-1601 AH; Persian 1501-1502 and 2987-2988 AP; a year, a month and `until` across each seam *(run)* | the shared years and this profile's arithmetic over them | the same answers | **same**; the years are SJL-8's (its K1-K13), and the arithmetic over them agrees at the seams. `runs/c06-table-edges.compare.txt` |

## 3. Serialization

**Broiler.VM** writes every string form from one set of members: `PadYear`, `FractionalSeconds`, `FormatTime`,
`DateToString`, `FormatOffsetNs`, `FormatOffsetRounded` and `DurationToString` (`VM/JsTemporalCore.cs:581-722`),
`DateString`, `DateTimeString`, `YearMonthString`, `MonthDayString`, `InstantString` and `ZonedDateTimeString`
(`VM/JsTemporal.Conversions.cs:493-575`), and `CalendarAnnotation` (`VM/JsTemporal.Calendar.cs:865-871`). The options
are read by `VM/JsTemporal.Options.cs:121-272` and turned into a precision by `Precision` (:222-237).

**Broiler.JS** writes each type's form in its own class, each with its own copies of padding and calendar-name reading
(section 6 lists them).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| S1 | Years 0, 1, 999, 1000, 9999, 10000, -1, -9999, -10000, ±123456 and both limits, in `PlainDate`, `PlainYearMonth`, `PlainDateTime`, `Instant`, `ZonedDateTime`; reference days and years; fields and fractions padded *(run)* | four digits for 0-9999, else a sign and six (`VM/JsTemporalCore.cs:587-588`) | the same answers (`T/JSTemporalInstant.cs:525-528`, and copies in each formatter) | **same**. `#sec-temporal-padisoyear`. `runs/s01-forms.compare.txt` |
| S2 | `calendarName` `auto`, `always`, `never`, `critical`, `undefined`, and the invalid `x`, `AUTO`, `""`, `null`, `1`, `true`, in each type with a calendar, in ISO 8601, `gregory`, `hebrew`, `chinese`; a year-month and month-day outside ISO 8601 showing their reference day and year *(run)* | `[u-ca=…]`, `[!u-ca=…]` or nothing (`VM/JsTemporal.Calendar.cs:865-871`); the reference day or year shown when the calendar is not ISO 8601 or the annotation forced (`VM/JsTemporal.Conversions.cs:506-531`) | the same answers on all 220 lines (`T/JSTemporalPlainDate.cs:1189-1195`, `T/JSTemporalPlainYearMonth.cs:804-825`, `T/JSTemporalPlainMonthDay.cs:577-596`) | **same**. `#sec-temporal-formatcalendarannotation`, `#sec-temporal-temporalyearmonthtostring`, `#sec-temporal-temporalmonthdaytostring`. `runs/s01-forms.compare.txt` |
| S3 | `toJSON` of every type, and `toString` with an options argument that is not an object (`null`, `1`, `"x"`, `true`) *(run)* | `toString` with no options; `TypeError` | the same | **same**. `runs/s01-forms.compare.txt` |
| S4 | `fractionalSecondDigits`: `undefined`, `"auto"`, 0-9, 10, -1, 2.5, 9.9, -0.5, `"3"`, `"x"`, `NaN`, ±∞, `true`, `null`, `"AUTO"`, `3n`, in `PlainTime`, `PlainDateTime`, `Instant`, `ZonedDateTime`, `Duration` *(run)* | a Number floored and checked 0-9, anything else `"auto"` or `RangeError` (`VM/JsTemporal.Options.cs:183-217`) | the same answers (`T/JSTemporalRoundingOptions.cs:130-150`) | **same**. `#sec-temporal-gettemporalfractionalseconddigitsoption`. `runs/s02-precision.compare.txt` |
| S5 | `smallestUnit`: each time unit, plurals, `hour`, `day`, `year`, `week`, `"MINUTE"`, `"auto"`, `""`; with `fractionalSecondDigits` *(run)* | `Precision` (`VM/JsTemporal.Options.cs:222-237`); `hour` and the date units refused by every type, `minute` also by `Duration` | the same answers | **same**. `#sec-temporal-tosecondsstringprecisionrecord`. `runs/s02-precision.compare.txt` |
| S6 | `roundingMode`: each of the nine modes and three invalid names, at ties and off them, positive and negative, before the epoch, across a day and at both limits *(run)* | `ExactRounding` over the epoch or time nanoseconds (`VM/JsTemporal.Conversions.cs:580-581`) | the same answers (`ExactRounding` in each formatter) | **same**. `#sec-temporal-getroundingmodeoption`. `runs/s02-precision.compare.txt` |
| S7 | `ZonedDateTime#toString`'s `timeZoneName` and `offset` (valid and invalid values, `critical`), with a calendar annotation *(run)* | `VM/JsTemporal.Conversions.cs:548-575` | the same answers (`T/JSTemporalZonedDateTime.cs:1801-1844`) | **same**. `#sec-temporal-temporalzoneddatetimetostring`. `runs/s03-zoned-instant.compare.txt` |
| S8 | Sub-minute offsets in strings and the `offset` getter: Monrovia 1970 (−00:44:30), Dublin 1900 (−00:25:21), New York 1850 (−04:56:02), Kolkata 1850 and 1900, Abidjan 1850; the string's round trip *(run)* | the string's offset rounded to the minute (`VM/JsTemporalCore.cs:674-678`), the getter's to the second (:657-666): `1969-12-31T23:15:30-00:45[Africa/Monrovia]`, `-00:44:30` | the same answers (`T/JSTemporalZonedDateTime.cs:1777-1792, 1755-1769`) | **same**. `#sec-temporal-formatdatetimeutcoffsetrounded`. The getters differ on an offset with a fraction of a second, which Broiler.JS drops (`T/JSTemporalZonedDateTime.cs:1755-1769`): no zone has one (**code no input reaches**). `runs/s03-zoned-instant.compare.txt` |
| S9 | `Instant#toString`'s `timeZone`: names, offsets, an ISO string, an unknown name, a sub-minute offset, `null`, 0; with rounding *(run)* | `VM/JsRealm.TemporalInstant.cs:104-123` | the same answers (`T/JSTemporalInstant.cs:281-353`) | **same**. `#sec-temporal-temporalinstanttostring`. `runs/s03-zoned-instant.compare.txt` |
| S10 | Duration strings: zero, signs, every field alone, sub-second fields past a second, each field at its limit, with `fractionalSecondDigits` 0 and 3; rounding that carries across units and to the limits *(run)* | `VM/JsTemporalCore.cs:683-722`, rounding over the time duration (`VM/JsRealm.TemporalDuration.cs:116-142`) | the same answers (`T/JSTemporalDuration.cs:466-577`) | **same**. `#sec-temporal-temporaldurationtostring`. `runs/s04-durations.compare.txt` |
| S11 | The order each `toString` reads its options in, and whether an invalid one stops the reading *(run)* | alphabetical; an invalid value thrown when read, `smallestUnit` validated after every read | the same reads and errors | **same** (`3eaaadf7`, 2025-08-22). `runs/s05-option-order.compare.txt` |

## 4. The milestone's corrections in these sub-areas

| Correction (milestone SJL-10.5) | Verdict here | Rows |
|---|---|---|
| Lenient grammar acceptances | **Confirmed**, and wider than the area's table: invalid days in year-month strings (G3), invalid dates under the time goal (G4), mixed separators in instants (G6), offsets out of range in the four plain types (G8), digits other than ASCII (G9), empty annotations (G11), malformed zone annotations (G12), annotations out of order (G13), a time on a year-month (G16), the calendar and time zone string parsers (G17, G18), `Z` as an offset string (G19), `relativeTo` (G20), a bare `T` in durations (G22) | G3, G4, G6, G8, G9, G11-G13, G16-G20, G22 |
| Invalid month codes silently constrained | **Confirmed** for `M00`, `M13` (where not a month), `M14` and `M99` in eight calendars under `constrain` (C5); not for ISO 8601, the Gregorian family, Hebrew, `chinese` or `dangi`, which refuse them as Broiler.VM does (C4). Related: a month code with other digits is an `Error` (C6) | C4, C5, C6 |
| Reference years computed without the proposal's table | **Confirmed of the code, refuted of the answers**: Broiler.JS searches and has no table, and answers as Table 6 and the 1900-2035 rule on all 1,670 lines of month-days probed (C10). The table belongs in the library either way | C10 |
| Integer narrowing that wraps, where it touches parsing | **Refuted for the grammar**: every digit run the grammar reads is bounded, duration parts are doubles, and no parse narrows (G1, G23). **Confirmed in a narrow form for calendar fields**: the `(int)` casts saturate on .NET 9 and later rather than wrap *(host)*, so no field wraps (C11), but an ISO month-day's year past 2³¹ loses its leap year (C12). The wrapping in arithmetic (`add({years: 4294967295})`) is the other half's | G1, G23, C11, C12 |

Not in the milestone's list and found here: the era names (C9), the calendar a `PlainMonthDay` lends (C13), the three
conversions (C15-C17), the aliases `gregorian` and `minguo` (C2), and the last-annotation reading of calendar and time
zone strings (G17, G18). **No row is a correction of Broiler.VM**: on every probe its answer is the specification's
as read here, and the retained datasets agree with it line for line.

## 5. The area design's readings, for these sub-areas

What follows reads [`areas/temporal.md`](../../javascript-shared-libraries/areas/temporal.md) sections 2 and 3
against the rows above.

- **"`JsTemporalParser.Parse(JsEngine engine, …)` … the engine is used only to construct `RangeError`s, at a handful of
  sites"**: confirmed, five sites (`VM/JsTemporalParser.cs:856, 861, 868, 875, 886`); `ParseCalendarString` catches
  `JsThrow` (`VM/JsTemporal.Calendar.cs:284`), which a `TryParse` replaces.
- **"`JsCalendars` (which has no engine coupling)"**: confirmed. The realm-facing calendar parts have 37 error sites
  between them and read properties in three places (Facts).
- **"The real string parsers are private per type"**: confirmed, eight, plus a ninth in `TemporalIsoString` the design
  counts among its helpers. **"`ParseZonedDateTimeString`, which reads options part-way through the parse"**: true of
  the code's shape, with no observable consequence (G24).
- **`TemporalIsoString`** (`T/TemporalIsoString.cs`): confirmed: regular-expression helpers (:31-45, :68, :200-220,
  :382-385), validators that throw (:52-65, :88-92, :125-192), members that perform an observable `ToPrimitive` and
  throw `TypeError` (`RequireMonthCodeString`, `RequireOffsetString`, `RequireStringField`, :77-114), and the dead
  `RejectIncompatibleStyle` (:341-348), which no file of the repository calls.
- **Section 3's table**, row by row:
  - `PlainDate.from('2020-01-01[]')`, `PlainYearMonth.from('2020-02-30')`, `PlainTime.from('2021-13-45T12:00')`:
    confirmed by run (G11, G3, G4);
  - "Bracketed annotations in the wrong order or malformed | refused | partly checked": confirmed, and what is
    unchecked is the order (G13), the zone annotation's text (G12) and emptiness (G11); keys, values and critical
    flags are checked (G14);
  - "Offset strings in `with()` | accepts `Z` and `+25:00`": `Z` confirmed; `+25:00` refused since SJL-8.7 (G19);
  - "Invalid month codes (`M13` in a 12-month calendar) | constrained silently": confirmed, eight calendars (C5);
  - "Month-day reference years | a search without it": confirmed of the code, with equal answers (C10);
  - "Duration strings | exact (`BigInteger`) | `double.Parse`": confirmed of the code, with equal answers (G23).
- **Not in the design**: G6, G8, G9, G16-G20, G22, C2, C6, C9, C12, C13, C15-C17. The design's grammar table should
  carry them as accept and refuse cases for SJL-10.2's vectors.

## 6. For SJL-10.5: Broiler.JS's deletion list in these sub-areas

In this section and the next, a bare `:n` is in the file its bullet, or the paragraph that opens its list, names.

**The per-type regular-expression parsers**, each with its patterns:
- `T/JSTemporalPlainDate.cs`: `DatePattern` and `CalendarAnnotation` (:731-737), `ParseTemporalDateString` (:739-767);
- `T/JSTemporalPlainDateTime.cs`: `DateTimePattern` (:1048-1051), `ParseTemporalDateTimeString` (:1053-1094),
  `CalendarAnnotation` (:1096);
- `T/JSTemporalPlainTime.cs`: `StrictTimeCore`, `OffsetDesignator`, `DateTimeFormPattern`, `BareTimeFormPattern`,
  `TrailingAnnotationsPattern`, the two ambiguity patterns and `NumericUtcOffsetPattern` (:538-574),
  `ValidateNumericUtcOffset` (:576-586), `ParseTemporalTimeString` (:588-657), `IsAmbiguousTimeString` (:661-684);
- `T/JSTemporalPlainYearMonth.cs`: `Year` and `YearMonthPattern` (:694-698), `ParseTemporalYearMonthString`
  (:700-742), `CalendarAnnotation` (:744);
- `T/JSTemporalPlainMonthDay.cs`: `MonthDayPattern`, `FullDatePattern`, `CalendarAnnotation` (:465-477),
  `ParseTemporalMonthDayString` (:479-541), `ResolveCalendarId` (:544-545);
- `T/JSTemporalZonedDateTime.cs`: `OffsetValuePattern` and `TryParseOffsetString` (:1550-1571), `YearField`,
  `ZonedCorePattern`, `ZonedTrailingAnnotation` (:1579-1589), `ParseZonedDateTimeString` (:1595-1744), and the
  string fallback of `CanonicalizeTimeZone` (:1385-1387);
- `T/JSTemporalInstant.cs`: `YearField`, `InstantPattern` (:434-440), `ParseTemporalInstant` (:442-504);
- `T/JSTemporalDuration.cs`: `DurationPattern` (:406-408), `ParseTemporalDurationString` (:410-453),
  `FractionToNanoseconds` (:456-461).

**The per-type formatters**:
- `T/JSTemporalPlainDate.cs`: `ToISOString` (:1172-1185), `FormatCalendarAnnotation` (:1189-1195);
- `T/JSTemporalPlainDateTime.cs`: `ToISOString` and `FormatISOString` (:1278-1317);
- `T/JSTemporalPlainTime.cs`: `FormatTime` (:368-385), `ToISOString` (:733-748);
- `T/JSTemporalPlainYearMonth.cs`: `ToISOString` (:804-825);
- `T/JSTemporalPlainMonthDay.cs`: `ToISOString` (:571-596);
- `T/JSTemporalZonedDateTime.cs`: `FormatOffset` (:1755-1769), `FormatOffsetRounded` (:1777-1792), `ToISOString` and
  `FormatToString` (:1795-1844);
- `T/JSTemporalInstant.cs`: `FormatISO` (:511-543), `AppendFraction` (:547-560);
- `T/JSTemporalDuration.cs`: `ToISOString` (:466-577), `DefaultLargestUnitRank` (:581-593).
- The `toString` bodies' precision arithmetic (`T/JSTemporalPlainTime.cs:307-364`, `T/JSTemporalPlainDateTime.cs:583-651`,
  `T/JSTemporalInstant.cs:281-353`, `T/JSTemporalZonedDateTime.cs:436-515`, `T/JSTemporalDuration.cs:141-184`) keeps
  only its option reading; the five `ReadCalendarName` copies (`T/JSTemporalPlainDate.cs:379-394`,
  `T/JSTemporalPlainDateTime.cs:661-676`, `T/JSTemporalPlainMonthDay.cs:253-268`, `T/JSTemporalPlainYearMonth.cs:479-494`,
  `T/JSTemporalZonedDateTime.cs:1847-1862`) become one adapter over the library's rule.

**`TemporalIsoString`** (`T/TemporalIsoString.cs:24-386`), whole:
- the helpers: `OffsetCore`, `TimeCore`, `TimeAndOffsetTail`, `AnnotationsTail` (:31-68), `RejectTimeTailForCalendarOnly`
  (:52-65), `MonthCodeSyntaxPattern` and `RequireWellFormedMonthCode` (:85-92), `CalendarAnnotationPattern` and
  `RejectMultipleCalendarAnnotations` (:118-135), `RejectMalformedAnnotations` (:141-158), `RejectInvalidAnnotations`
  and `MinutePrecisionOffsetName` (:169-195), the patterns and `TryParse`, `FillTime`, `IsValidDate`, `IsValidTime`
  (:200-320), `TryExtractCalendar` (:324-334), `TryExtractTimeZone` (:353-363), `TryParseRelative` (:368-378),
  `StrictOffsetPattern` and `IsStrictOffset` (:382-385);
- the coercing members, which become adapter code: `RequireMonthCodeString`, `RequireOffsetString`,
  `RequireStringField` (:77-114);
- the dead `RejectIncompatibleStyle` (:341-348), removed.

**`TemporalNonIso`'s calendar parts** (`T/TemporalNonIso.cs:17-615`): `CalendarYmd` (:24-25), `ToIsoFromBag` (:31-39),
`WithToIso` (:48-84), `YearMonthToIso` (:88-94), the reference-date search (:105-291: `MonthDaySearchYears`,
`MonthDayOrdinal`, `MonthDayExact`, `MonthDayMaxLength`, `MonthDayFromCode`, `MonthDayFromYearMonth`,
`MonthDayFromBag`, `MonthDayCore`), `MonthDayFields` and `MonthDayToPlainDate` (:294-307), `ResolveYearMonth`
(:312-387), `RegulateToIso` (:396-416), `ResolveMonthAfterYearShift`, `AddCalendarMonths`, `AddCalendarYearsMonths`,
`AddToIso` (:424-499), `Difference` (:503-580, the other half's), `ParseMonthCode` and `CompareDate` (:600-614). Its
coercions (`ToIntegerWithTruncation`, `ToPositiveIntegerWithTruncation`, :584-597) and the property reads inside
`ToIsoFromBag`, `WithToIso`, `MonthDayFromBag` and `ResolveYearMonth` are adapter code.

**Calendar code the milestone does not name, for the decision (section 8)**: the formula calendars
(`T/TemporalArithmeticCalendar.cs:25-348`, whose `islamic` entry no input reaches, C3); the arithmetic over the shared
years (`T/TemporalTableCalendar.cs:18-191` but `Year`, :37-42); the dispatch (`T/TemporalCalendarMath.cs:9-57`); the
Gregorian family's identifiers, years and eras (`T/JSTemporalCalendar.cs:39-293`, of which `ToSlotValue` and
`ResolveCalendarIdentifierArgument` are adapter code); and the per-type month-code copies: `MonthFromCode`
(`T/JSTemporalPlainDate.cs:515-528`, `T/JSTemporalPlainDateTime.cs:776-789`, `T/JSTemporalPlainMonthDay.cs:311-324`,
`T/JSTemporalPlainYearMonth.cs:532-545`, `T/JSTemporalZonedDateTime.cs:1429-1442`), `ValidateMonthCodeSyntax` and
`MonthCodeSyntaxPattern` (`T/JSTemporalPlainDate.cs:534-540`, `T/JSTemporalPlainDateTime.cs:794-800`),
`MonthFromCodeIso` (`T/JSTemporalPlainDate.cs:545-552`, `T/JSTemporalPlainDateTime.cs:805-812`) and
`ParseMonthCodeLeap` (`T/JSTemporalPlainYearMonth.cs:253-259`).

## 7. For SJL-10.4: Broiler.VM's members that move in these sub-areas, and their coupling

- **`JsTemporalParser`** (`VM/JsTemporalParser.cs:26-1164`), whole, behind a `TryParse` that returns the record and a
  `TemporalParseError`:
  - its five `RangeError` sites in `Parse` (:856 calendar conflict, :861 unknown critical key, :868 year-month in
    another calendar, :875 month-day in another calendar, :886 no goal matched) become error codes; the engine
    argument goes;
  - `IsTimeZoneIdentifier`, `IsUtcOffset`, `OffsetNanoseconds`, `Duration` and `IsAnnotationValue` take no engine
    already (:644-668, :928-1163); `Duration`'s `null` becomes an error code at its one caller
    (`VM/JsTemporal.Durations.cs:155-156`);
  - `ParseCalendarString`'s `JsThrow` catch (`VM/JsTemporal.Calendar.cs:269-293`) becomes a test of `TryParse`'s
    result, then of `IsAnnotationValue`; the thin wrapper that stays raises the `RangeError`;
  - the callers that stay: `VM/JsTemporal.Conversions.cs:143, 198, 271, 322, 382, 428, 468`,
    `VM/JsTemporal.Durations.cs:315`, `VM/JsTemporal.Zones.cs:70-104, 128`, `VM/JsTemporal.Options.cs:435`.
- **`JsTemporal.Conversions`' string formatting** (`VM/JsTemporal.Conversions.cs:493-581`): `DateString`,
  `DateTimeString`, `YearMonthString`, `MonthDayString` and `RoundInstant` are engine-free; `InstantString` and
  `ZonedDateTimeString` take the engine only for `OffsetNanosecondsFor` and `IsoDateTimeFor`, so they move as
  functions of an offset in nanoseconds and a date-time, the zone lookup staying in the adapter (or going through
  SJL-10's time-zone provider). With them, `JsTemporalCore`'s string members (`VM/JsTemporalCore.cs:581-722`, no
  coupling) and `CalendarAnnotation` (`VM/JsTemporal.Calendar.cs:865-871`, none).
- **The core of `JsTemporal.Calendar`** (`VM/JsTemporal.Calendar.cs`): `AvailableCalendars` and the rule of
  `CanonicalizeCalendar` (:182-233, one `RangeError`); `SupportsEra`, `ExtraFields`, `KeysToIgnore`, `MergeFields`
  (:216, :312-315, :440-507, none); `ResolveFields`, `RegulateDate`, `DateToIso`, `DateFromFields`,
  `YearMonthFromFields`, `MonthDayFromFields`, `DateAdd`, `DateUntil`, `IsoToDate`, `DateToFields` (:525-860), coupled
  by 12 `RangeError` and `TypeError` sites, each an error code to map; the month-code syntax of `MonthCodeOf` (:426-427,
  after its `ToPrimitive`, which stays). `PrepareFields` (:323-392), `CalendarWithIsoDefault` (:298-307),
  `ToCalendarIdentifier` (:251-264) and `CalendarOf` (:238-246) read properties or objects and stay.
- **The core of `JsTemporal.CalendarNonIso`** (`VM/JsTemporal.CalendarNonIso.cs:53-540`), whole, coupled by 18
  `RangeError` and `TypeError` sites and by `SystemOf`'s `engine.Intl!.Calendars` (`VM/JsTemporal.Calendar.cs:211`),
  which becomes the library's calendar set over SJL-8's provider.
- **`JsCalendars`** (`VM/JsCalendars.cs:30-942`), whole, with no coupling; its `CalendarTables` already the shared
  library's (:917).
- **The rule half of `JsTemporal.Options` in these sub-areas** (`VM/JsTemporal.Options.cs`): the allowed values of
  `calendarName`, `timeZoneName`, `offset` (:121-134), `FractionalSecondDigits`' range (:183-217, which reads and so
  splits), and `Precision` (:222-237, none).

## 8. What needs a decision

- **The pin.** Both engines behave as `e8cc03fc` on every revision marker probed but Broiler.JS's era names, and run the
  same test262 `ccaac100`, which precedes the test262 `e8cc03fc` carries by five Temporal test commits. Whether
  Broiler.JS pins `e8cc03fc` (and the era proposal at `5833eae`), and whether the area's test262 rule runs `ccaac100` or
  `227d9055`, is the verdict's.
- **The deletion list's reach.** The milestone names `TemporalNonIso`'s calendar parts; the formula calendars, the
  arithmetic over the table years, the dispatch, the Gregorian family's eras and the five month-code copies (section 6)
  are calendar parts in other files. X6 asks for every member the cut-over deletes.
- **The era names (C9)**: whether Broiler.JS's older names are a correction (the specification's
  CanonicalizeEraInCalendar admits none of them) or a compatibility the library keeps as an alias table. Read here as a
  correction.
- **The aliases `gregorian` and `minguo` (C2)**: a correction under the era proposal's 1.1.1, unless the verdict reads
  CLDR's legacy aliases into AvailableCalendars.
- **Rows the area's grammar table lacks** (section 5): SJL-10.2's grammar vectors should cover G3-G4, G6, G8-G13,
  G16-G20 and G22 as well as the table's cases.
