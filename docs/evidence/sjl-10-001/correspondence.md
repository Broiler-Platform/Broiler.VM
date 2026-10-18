<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-10.1's correspondence from code: the Temporal core in both engines

**Written:** 2026-10-18, at SJL-10.1, as the input of
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md)'s
X1, X2 and X6 for each of the area's seven sub-areas, and of the area's comparison rules, before any code of SJL-10
exists. **It was written in two halves on 2026-10-18 and merged the same day.**
- **The core half** covers records and limits, balancing, durations and rounding, and differences: sections 1 to 4,
  with their rows R, B, D and F.
- **The syntax half** covers the grammar, the calendars and serialization, and the proposal revision each engine
  follows: sections 5 to 7, with their rows G, C and S, and the facts on the revision.

The merge de-duplicates the two headers and the two sets of facts, and puts the halves' lists of what moves, what is
deleted and what needed a decision under one heading each (sections 8 to 12). **Every row keeps its identifier and
its content as its half wrote it.** Where a half's text calls something "the other half's", "the calendar half's" or
"the grammar half's", it means sections 1 to 4 or sections 5 to 7. A row's "decision n" is item n of section 12.

**Read at** Broiler.VM `14072b0`, whose product sources are `047dcec`'s, the default branch after SJL-8; and
Broiler.JS `d3464413`, its default branch after SJL-8.7. The shared library is read in Broiler.JS at `d3464413`; every
file of it cited here is byte-identical at `a04cd447`, the commit Broiler.VM's package `Broiler.ECMAScript.Algorithms`
`0.1.0-preview.5` was packed from (`eng/packages/Broiler.ECMAScript.Algorithms.0.1.0-preview.5.pin:54`). **By reading,
and by running where a row says so.**

**Three columns.** Every table has a Broiler.VM column and a Broiler.JS column. The third is the reference, the
proposal's polyfill at `e8cc03fc`, where it was run.
- **In sections 1 to 4** the column "The proposal and the reference" gives the pinned specification text and the
  polyfill's answer. Every probe of those sections ran under the polyfill, and where its answer and the pinned text
  disagree, the row says so (D8).
- **In sections 5 to 7** the probes were not run under the polyfill, so the tables have no reference column; each
  class cell cites the specification text it reads. There the polyfill's reading is the retained datasets' answers,
  which both engines were run against (Facts).

**How each column was run.**
- A Broiler.VM cell marked *(run)* was run from the Release build of the checkout through the conformance root, as a
  raw test262 test in a test262-shaped directory (`correspondence-runs/core/vm262/` and
  `correspondence-runs/syntax/vm262/`, each with the pinned suite's `features.txt`), under the manifest
  `broiler.javascript.wide`, whose realm has Intl and Temporal. The harness files of test262 at the common pin
  `ccaac100` were copied into each directory for the run and are not retained; a raw test reads none of them.
- A Broiler.JS cell marked *(run)* was run through the shell, `BroilerJS.dll --script-host`, built in Release, on .NET
  10.0.12, with no `LANG` and `TZ=UTC`, on Ubuntu 24.04.5.
- **The reference** is the proposal's polyfill at `e8cc03fc`, run as the retained dataset's README runs it
  (`src/tests/temporal/README.md`): Node 22.22.0, `lib/shim.mjs` imported first, its three runtime dependencies
  installed from the npm registry at their current versions (big-integer 1.6.52, call-bind 1.0.9, es-abstract
  1.24.2).
- A cell marked *(host)* is behaviour that comes from .NET; *(reading)* is code not executed.

SJL-10.2's vectors and the retained reference datasets confirm or refute each row. **It holds no figure of a run**:
the values below are inputs, the answers they give, and counts of data.

**Paths.** Both halves' abbreviations are kept, so that every row reads as its half wrote it.
- **In Broiler.VM**, `VM/` is `src/Broiler.VM.Profile.JavaScript/` and `TD/` is `src/tests/temporal/`. In `VM/`,
  `Core` is `JsTemporalCore.cs`, `Dur` `JsTemporal.Durations.cs`, `Opt` `JsTemporal.Options.cs`, `Cal`
  `JsTemporal.Calendar.cs`, `Conv` `JsTemporal.Conversions.cs`, `Zon` `JsTemporal.Zones.cs`, and `R.Dur`, `R.PD`,
  `R.PDT`, `R.PT`, `R.YMD`, `R.Ins`, `R.ZDT` the realm files `JsRealm.TemporalDuration.cs`, `…PlainDate.cs`,
  `…PlainDateTime.cs`, `…PlainTime.cs`, `…YearMonthDay.cs`, `…Instant.cs`, `…ZonedDateTime.cs`.
- **In Broiler.JS**, `T/` is `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/`. In `T/`, `J.Dur` is
  `JSTemporalDuration.cs`, `J.DurA` `JSTemporalDuration.Arithmetic.cs`, `J.Rnd` `JSTemporalZonedDateTime.Rounding.cs`,
  `J.PD`, `J.PDT`, `J.PT`, `J.PYM`, `J.PMD`, `J.Ins`, `J.ZDT` the `JSTemporal*.cs` files of those types, `J.RO`
  `JSTemporalRoundingOptions.cs` (class `TemporalRoundingOptions`) and `J.NI` `TemporalNonIso.cs`.
- **The shared library**, `Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/` in Broiler.JS, is `SL/` in sections 1 to
  4 and `AL/` in sections 5 to 7.
- **The proposal** at `e8cc03fc` is `PT/`; `PT/spec/x.html` names the file an abstract operation is in.

A line number is the line at the revision above; a bare `:n` is in the file named last in the same cell or bullet.

**References.** "The proposal" is `tc39/proposal-temporal` at `e8cc03fc`, its specification text (`spec/*.html`) read
at that commit and cited by clause id, and its polyfill (`polyfill/lib/`). "The era proposal" is
`tc39/proposal-intl-era-monthcode` at `5833eae` (its Stage 4 draft of 19 March 2026, the revision SJL-8.1 read), cited
by its section numbers and tables: Table 1 (calendar types and aliases), Table 2 (eras), Table 3 (month codes) and
Table 6 (Chinese and Korean reference years). Both histories were read from blobless clones taken on 2026-10-18. The
proposal's sources and polyfill and the era proposal are not retained: the rows cite them by revision and path.

**The runs.** `runs/` is [`correspondence-runs/core/`](correspondence-runs/core/) in sections 1 to 4, and
[`correspondence-runs/syntax/`](correspondence-runs/syntax/) in sections 5 to 7 and in the facts. Each directory holds:
- every probe (`probes/<name>.js`, with the shared `prelude.js` and `epilogue.js`);
- the program each engine ran (`built/<name>.js`, and `vm262/test/sjl10/<name>.js` with its test262 front matter);
- each engine's raw answer (`<name>.bjs.txt`, `<name>.vm.raw.txt` and the extracted `<name>.vm.txt`), and in
  `core/` the reference's (`<name>.ref.txt`);
- the comparison (`<name>.compare.txt`), named by row or row group;
- the harness: `run-probe.sh` and `sidebyside.py`, SJL-8.1's, in each; in `core/`, `run-dataset.sh`, which runs the
  retained dataset (`d6-retained-dataset.*`: each engine's lines and their `diff` against the retained answer), and
  `run-all.sh`, which runs everything; in `syntax/`, `compare-retained.py`, which compares the retained datasets' runs
  with the polyfill's answers (it writes `d-retained-*.compare.txt` in place of `sidebyside.py`, from
  `TD/temporal.polyfill-e8cc03fc.txt`, `TD/calendars/temporal-calendars.polyfill-e8cc03fc.txt` and their
  `divergences.txt`).

A file over 100 KB is retained gzip-compressed under its name with `.gz`.

**How a comparison is marked.**
- **In `core/`**, a comparison line starts with two marks, Broiler.VM's then Broiler.JS's, each against the
  reference: blank where the engine answers as the reference does, `~` where both throw the same constructor with
  different messages, `!` for any other difference. Messages differ on every error line; the proposal states only an
  error's type, and the message stays each engine's (JSD-0054 section 5).
- **In `syntax/`**, the comparison is of the two engines, its differing lines marked `!!`. A probe that compares
  acceptance prints a refusal as its error's type only; one that prints messages says so, and its `!!` then also
  marks lines whose messages alone differ.

## Facts

### Where each engine's code is

**Records, balancing, durations and differences.**

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
  `relativeTo`) and `J.Rnd` (489 lines; zoned `relativeTo`). Section 11 lists the copies.

**The grammar, the calendars and serialization.** Broiler.VM keeps the three sub-areas in seven files:

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

Broiler.JS spreads the same responsibilities across its object classes, every member throwing through
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
- **a formatter per type** (section 11 lists them).

### What is already shared, and so not SJL-10's to move

Members both engines' Temporal code calls, counted by reading:

- **From SJL-4**, `SL/Numbers/ExactRounding.cs` and `RoundingMode.cs`: `RoundNumberToIncrement`,
  `RoundNumberToIncrementAsIfPositive`, `Negate`, `GetUnsignedRoundingMode`, `ApplyUnsignedRoundingMode`, `ToDouble`
  (an exact ratio rounded once to a double) and `TryParseRoundingMode` (`:53-240`). Also
  `IntegerConversions.ToIntegerOrInfinity` (`SL/Numbers/IntegerConversions.cs:119`). Both engines round and convert
  every Temporal quantity through these: Broiler.VM at 19 call sites, Broiler.JS at 54. Both engines' string rounding
  and Broiler.VM's duration-string conversion (`VM/JsTemporalParser.cs:1156`) call `ExactRounding` too.
- **From SJL-6**, `SL/Time/IsoCalendarMath.cs`: `IsLeapYear`, `DaysInYear`, `DaysInMonth`, `EpochDays`,
  `FromEpochDays`, `DayOfWeek`, `DayOfYear`, `WeekOfYear`, `BalanceYearMonth` and `IsValidIsoDate` (`:62-235`), with
  `EcmaTime.FloorDivide` under them. These are the civil-day half of `AddISODate`, `BalanceISOYearMonth` and the
  limits. Both parsers already validate a date with `IsoCalendarMath.IsValidIsoDate` (`VM/JsTemporalParser.cs:297`,
  `T/TemporalIsoString.cs:311`). The helpers' **preconditions are exceptions**: a year above 2^53 in magnitude, a day
  count above 2^60, or a month outside 1 to 12 throws `ArgumentOutOfRangeException` (`:239-260`).
- **From SJL-8**:
  - in `SL/Time/ZonedTime.cs`, the neutral records `IsoDate(long, int, int)`, `IsoTime` (six `int`s, no overflow days)
    and `IsoDateTime` (`:29-47`); `MinEpochNanoseconds` and `MaxEpochNanoseconds` (`:149-154`); the zone operations
    both engines call (`:172-380`); and private copies of GetUTCEpochNanoseconds, a time's nanoseconds,
    BalanceISODateTime and a date-time's validity (`:531-580`), the same arithmetic as `Core:249-251, 341-342,
    361-365`;
  - the table calendars' years: `CalendarYears.GetYear` over `CalendarTables`, `TableCalendar` and `MaxYearMagnitude`
    (`AL/Time/CalendarYears.cs:30-451`), read by `VM/JsCalendars.cs:286-289` and `T/TemporalTableCalendar.cs:37-42`;
  - the offsets: `OffsetTimeZones.FormatOffsetIdentifier`, `TryParseOffsetIdentifier` and `IsTimeZoneOffsetString`
    (`AL/Time/OffsetTimeZones.cs:46-107`).

  The area design's target records (`IsoDate`, `TimeOfDay`, `IsoDateTime` in `Broiler.ECMAScript.Temporal`) overlap
  these already (section 12, item 2).
- **Not shared**: the records' limits, balancing and arithmetic above SJL-6's civil days; the durations and their
  relative rounding; the differences; the grammar; the identifiers, month codes, eras and reference dates; the
  formula calendars; the arithmetic over the table calendars' years (month code to ordinal, the year a day falls in);
  and every string form.

### The proposal revision each engine follows

- **Broiler.VM follows `tc39/proposal-temporal@e8cc03fc970a65a3359e8870e3b35e687ac94e55`** (JSD-0053 section 6, the
  "Stage 4 Draft / July 27, 2026"), committed 2026-07-27 as "update status of fullcalendar/temporal-polyfill", a
  README change. It cites the pinned draft's operations by section (JSD-0053, JSD-0054). **What the revision
  includes:** the proposal reached Stage 4 by 2026-03-12 (`06d3e081`, README). Its last normative changes to the
  specification are `41faca47` (2026-01-29, PlainYearMonth refuses adding weeks, days and time units) and `377fb80e`
  (2026-02-02, plain objects formatted without regard to daylight saving, an Intl change that is SJL-9's); every later
  commit to `spec/` is editorial, the last `e2da8c7e` (2026-03-30). Later commits fix the polyfill: among them
  `385b479b` (2026-02-26, a calendar annotation canonicalised before the YYYY-MM and MM-DD check, issue 3269) and
  `69a60a2a` (2026-05-11, out-of-range years in a PlainMonthDay bag refused early). It carries test262 `227d9055`
  (2026-05-21) as its submodule. For the era proposal, Broiler.VM follows Tables 1, 2, 3 and 6 as JSD-0056 builds
  them, which are the era proposal's at `5833eae` (SJL-8.1 read the same draft).
- **Both engines run test262 `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`** (2026-05-08): Broiler.VM's pin
  (`src/tests/conformance/pins/test262.pin`, JSD-0053 section 6) and Broiler.JS's evidence (its
  `.github/workflows/test262.yml` default and `docs/evidence/sjl-8-001/decision-rule.md:49`). It lies between the
  proposal's own pins: `d0c1b455` (2026-04-29, set by the proposal's `4df4199a` on 2026-05-11) is its ancestor, and
  `227d9055`, the one `e8cc03fc` carries, is 32 commits after it. Five of those 32 change Temporal tests, tests only:
  `7f2001f6` (not-yet-supported calendars), `b1f9a0ae` (a regression test), `0f323ada` (a feature tag), `a70493eb`
  (a spurious extra smallest unit in rounding) and `2fac7182` (ZonedDateTime at a repeated midnight).
- **Broiler.JS pins no revision.** It cites the proposal's operations and test262 cases by name, at no revision. Its
  code cites `proposal-temporal #3141` (`T/JSTemporalZonedDateTime.cs:1014-1015`,
  `T/JSTemporalZonedDateTime.Rounding.cs:129`), an issue the proposal fixed in `87af06d0` (2025-09-23), and Table 6
  (`T/TemporalNonIso.cs:101-104`), which the era proposal added in `ad5028b` (2026-01-21). **Its behaviour is
  `e8cc03fc`'s on every marker probed in the grammar, the calendars and serialization but the era names**
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
  - **the one exception in those sub-areas is the era names** (C9): Broiler.JS answers the era codes of the era
    proposal's 2025-06-30 change (`08de50a`) but also accepts, on input, codes that change replaced (`gregory`,
    `gregory-inverse`, `coptic`, `hebrew`, `ethiopic`, `ethioaa`, `saka`, `persian`), aliases the era proposal removed
    on 2025-11-21 (`684dd16`: `mundi`, `minguo`, `before-roc`), and `incarnation`, which no revision of the era proposal
    has (its alias was `incar`, from `1ff75a2`, 2023-01-27, to `684dd16`); it accepts none of `incar`, `minguo-qian`,
    `coptic-inverse`, `roc-inverse` or `japanese-inverse`. No single revision accepts what it accepts. The other
    differing marker, `Duration#add` with a calendar unit throwing a plain `Error` (`8ae0dbd7`), is row D3's.
- **The revision both engines' behaviour is closest to** is the proposal from `41faca47` (2026-01-29) onward, with
  the polyfill fixes of 2026-02-26 and 2026-05-11. No normative change to the specification separates `41faca47`
  from `e8cc03fc` but `377fb80e`, which is SJL-9's. So `e8cc03fc` and the era proposal at `5833eae` describe
  Broiler.VM on every row of sections 5 to 7, and Broiler.JS on every row there but the corrections section 8 lists.
- **What could and could not be established.** The proposal's and the era proposal's histories and specification
  texts were read, and test262's commit graph (a treeless clone): established as above. Not established: which
  revision Broiler.JS's authors read, since nothing in its tree names one; whether the five test262 commits after
  `ccaac100` would change either engine's verdicts (not run here); and the polyfill's own answers to the probes of
  sections 5 to 7, since the polyfill was not run on them (the retained datasets are its only reading there).

### The limits

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

### Other facts

- **Both engines answer every probe's refusals with the same error type**, `RangeError` or `TypeError`, except where
  a row says otherwise; their messages differ throughout, which is each engine's choice (Broiler.VM names the
  operation, JSD-0054 section 5). A .NET exception inside Broiler.JS's parsers surfaces as a plain `Error` whose
  message is .NET's (G9, C6).
- **The retained datasets** (`TD/temporal.js`, 1,629 lines; `TD/calendars/temporal-calendars.js`, 3,999 lines) were
  run unchanged in both engines (`runs/d-retained-*`). Broiler.VM answers every line as the polyfill at `e8cc03fc`
  does or as `divergences.txt` names. Broiler.JS answers 7 lines of the first and 103 of the second otherwise. Of the
  103, 26 are rows of section 6 (C5, C15, C16, C17); the other 77 (`until` rounded to months, `Duration#round` and
  `total` with a non-ISO `relativeTo`) are rows F4 and D11, and all 7 of the first are row D6
  (`runs/d-retained-*.compare.txt`). Sections 1 to 4 ran `TD/temporal.js` once more, with the same answers
  (`correspondence-runs/core/d6-retained-dataset.*`, row D6).

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

## 5. The grammar

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

## 6. The calendars

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

## 7. Serialization

**Broiler.VM** writes every string form from one set of members: `PadYear`, `FractionalSeconds`, `FormatTime`,
`DateToString`, `FormatOffsetNs`, `FormatOffsetRounded` and `DurationToString` (`VM/JsTemporalCore.cs:581-722`),
`DateString`, `DateTimeString`, `YearMonthString`, `MonthDayString`, `InstantString` and `ZonedDateTimeString`
(`VM/JsTemporal.Conversions.cs:493-575`), and `CalendarAnnotation` (`VM/JsTemporal.Calendar.cs:865-871`). The options
are read by `VM/JsTemporal.Options.cs:121-272` and turned into a precision by `Precision` (:222-237).

**Broiler.JS** writes each type's form in its own class, each with its own copies of padding and calendar-name reading
(section 11 lists them).

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

## 8. The milestone's listed corrections

**In records and limits, balancing, durations and differences** (sections 1 to 4):

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

**In the grammar, the calendars and serialization** (sections 5 to 7):

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

## 9. The area design's readings

**For records and limits, balancing, durations and differences**, the rows read the design's section 3 table
where they meet it: R7, D3, D11, F4 and F7 confirm or refine its five rows in those sub-areas, and section 8
lists what the table lacks. The design's target records are item 2 of section 12.

**For the grammar, the calendars and serialization:**

What follows reads [`areas/temporal.md`](../../javascript-shared-libraries/areas/temporal.md) sections 2 and 3
against the rows of sections 5 to 7.

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

## 10. For SJL-10.4: Broiler.VM's members that move, and their coupling

In this section and the next, a bare `:n` is in the file its bullet, or the paragraph that opens its list,
names.

**In records and limits, balancing, durations and differences** (sections 1 to 4):

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

**In the grammar, the calendars and serialization** (sections 5 to 7):

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

## 11. For SJL-10.5: Broiler.JS's deletion list

**In records and limits, balancing, durations and differences** (sections 1 to 4):

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

**In the grammar, the calendars and serialization** (sections 5 to 7):

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

**Calendar code the milestone does not name, for the decision (section 12, item 5)**: the formula calendars
(`T/TemporalArithmeticCalendar.cs:25-348`, whose `islamic` entry no input reaches, C3); the arithmetic over the shared
years (`T/TemporalTableCalendar.cs:18-191` but `Year`, :37-42); the dispatch (`T/TemporalCalendarMath.cs:9-57`); the
Gregorian family's identifiers, years and eras (`T/JSTemporalCalendar.cs:39-293`, of which `ToSlotValue` and
`ResolveCalendarIdentifierArgument` are adapter code); and the per-type month-code copies: `MonthFromCode`
(`T/JSTemporalPlainDate.cs:515-528`, `T/JSTemporalPlainDateTime.cs:776-789`, `T/JSTemporalPlainMonthDay.cs:311-324`,
`T/JSTemporalPlainYearMonth.cs:532-545`, `T/JSTemporalZonedDateTime.cs:1429-1442`), `ValidateMonthCodeSyntax` and
`MonthCodeSyntaxPattern` (`T/JSTemporalPlainDate.cs:534-540`, `T/JSTemporalPlainDateTime.cs:794-800`),
`MonthFromCodeIso` (`T/JSTemporalPlainDate.cs:545-552`, `T/JSTemporalPlainDateTime.cs:805-812`) and
`ParseMonthCodeLeap` (`T/JSTemporalPlainYearMonth.cs:253-259`).

## 12. What needed a decision

Each half closed with the points it left open. They are numbered here as one list, the core half's first, so
that a row's "decision n" and the facts' "item n" point here. JSD-0078 records how each was decided, under the
owner's direction of 2026-10-18.

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
4. **The pin.** Both engines behave as `e8cc03fc` on every revision marker probed but Broiler.JS's era names, and run the
   same test262 `ccaac100`, which precedes the test262 `e8cc03fc` carries by five Temporal test commits. Whether
   Broiler.JS pins `e8cc03fc` (and the era proposal at `5833eae`), and whether the area's test262 rule runs `ccaac100` or
   `227d9055`, is the verdict's.
5. **The deletion list's reach.** The milestone names `TemporalNonIso`'s calendar parts; the formula calendars, the
   arithmetic over the table years, the dispatch, the Gregorian family's eras and the five month-code copies (section 11)
   are calendar parts in other files. X6 asks for every member the cut-over deletes.
6. **The era names (C9)**: whether Broiler.JS's older names are a correction (the specification's
   CanonicalizeEraInCalendar admits none of them) or a compatibility the library keeps as an alias table. Read here as a
   correction.
7. **The aliases `gregorian` and `minguo` (C2)**: a correction under the era proposal's 1.1.1, unless the verdict reads
   CLDR's legacy aliases into AvailableCalendars.
8. **Rows the area's grammar table lacks** (section 9): SJL-10.2's grammar vectors should cover G3-G4, G6, G8-G13,
   G16-G20 and G22 as well as the table's cases.
