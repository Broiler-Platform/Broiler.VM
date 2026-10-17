<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0077 - The profile on the shared time zones and calendars: SJL-8.5's cut-over

**Status:** Decided 2026-10-17. It applies
[JSD-0076](0076-time-zone-and-calendar-data-move-to-the-data-home.md), the area's verdict. **The code
landed in the changes before this record**, so that the evidence this record cites could be taken at
commits of the code it judges; this record is the change after them.

**Date:** 2026-10-17.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list, rule I4, rule I5's register and the register rows of rules N29 to N32, and the release owner,
for the notices rows. **All three roles are held by one person** (EX-30), and this record does not claim
that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-8, work package SJL-8.5
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-8-time-zones-and-calendars.md)).
SJL-8 started before SJL-3 was met, and SJL-8 may not meet its gate before SJL-3 meets SJL-3's; the ledger
records both.

---

## Context

JSD-0076 accepted the area's three sub-areas - the tz database, the zone algorithms and the calendar
tables - and placed them: the pins, the compiler and the table generators with the data home, the
Broiler.JS repository, which no package of tables leaves (decision SJL-D-6 (c)), and the decoder, the zone
algorithms and the calendars' year sources in `Broiler.ECMAScript.Algorithms`, under
`Broiler.ECMAScript.Time`. It changed nothing in this profile. This record is the change itself: the
architecture tests regenerate the embedded tables with the home's generator, rule I4 holds this
repository's pins to the home's, every zone and calendar-year answer the family computed is the shared
library's, and the members that computed them are deleted. The evidence that judges it is bundle
[`sjl-8-001`](../../../../docs/evidence/sjl-8-001/README.md), under the rule SJL-8.1 committed before any of
this code existed.

**The changes**, each building, each with the tests it touches green:

| Change | What it does |
|---|---|
| `4df595b` | Admits `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5` from the committed feed, where `0.1.0-preview.4` was; both lock files, rule I1's witness and the notices row move with it |
| `f09a0dd` | Takes the package baseline again, bundle [JS-10-009](../evidence/js-10-009/README.md), at `4df595b`, because rule N37 holds the baseline's candidate to the version the allow-list pins ([JSC-312](../roadmap.corrections.md#jsc-312)) |
| `c5943b1` | Admits `Broiler.ECMAScript.Generation` `0.1.0-preview.5` test-only; the tz compiler, the two table generators and their pin readers leave the architecture tests, and rules N29 to N32 regenerate with the home's; N29 to N32 gain their register rows; rule I4 holds the tz and calendar pins and CLDR's time zone keys to the home's |
| `186a1d3` | The cut-over: the profile's zone operations, zone lookup, offset identifiers and table-backed calendar years call the shared library, and the members that computed them are deleted |
| `8613309` | Intl data that does not carry the tz release reads as before (section 10) |
| `eed568a` | Rule I5 registers the 24 deleted members and types; the notices' ICU4X entry says where the port went |

## Decision

### 1. References

**`Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` `0.1.0-preview.5`**, the shared
stream's next preview, packed locally from Broiler.JS `2d37a38a2560b076ebf2ca29989bfe20db64ed64` and
restored from the committed feed `eng/packages/` with lock files. **They are not on nuget.org at this
record**; each `.pin` beside its package on the feed says so, and a release of this family waits for their
publication, which `eng/verify-feed.ps1` enforces.

| Package | Use | Projects rule I1 admits it for |
|---|---|---|
| `Broiler.ECMAScript.Algorithms` | product | `Broiler.VM.Profile.JavaScript`, `.Compiler`, as before |
| `Broiler.ECMAScript.Generation` | test-only | `Broiler.VM.Architecture.Tests` alone, under rule I1's test-only entry (JSD-0075) |

**Rule I1 admits one version of a package**, so the allow-list's entry, the feed, both lock files and rule
I1's witness moved to `0.1.0-preview.5` together, and `0.1.0-preview.4` left the feed, in `4df595b`.
**The package baseline moved with it**: rule N37 holds the baseline's candidate to declaring the version
the allow-list pins or a later one, and JS-10-008's candidate declared `0.1.0-preview.4`. Bundle
JS-10-009 is the pack, the pristine consumer, the rollback and roll forward, the negative control and the
Native AOT run at `4df595b`, every step as JS-10-008's; `packages.md` and `support.md` name it.

**The Intl data assembly references nothing new.** It hands the profile bytes, as it did: the three time
zone tables, the calendar table, and since this change the tz release (section 2). The data stays in the
`.Intl` assembly, which only roots that hand in `IntlData` carry, and rule I3 holds that (JSD-0076's X5,
"optionality").

### 2. Seams

| Seam | What it is |
|---|---|
| `JsIntlTable.TimeZoneRelease` (Format, public) | The tz release the three time zone tables were compiled from, ASCII. `JsCldrData` answers it from `JsTzdbTables.Version`, `2026e` |
| `JsIntlTables.TimeZones` | The shared `TimeZoneDatabase`, as `ITimeZoneDatabase`, over the tables `TimeZones`, `TimeZoneIds` and `TimeZoneRegions` and the release; made the first time a zone is named, once per Intl data the composition hands in, and decoding each zone the first time its rules are read, as `JsTimeZones` did |
| `JsIntlTables.Calendars` | `JsCalendars` over the shared `CalendarTables` of the table `Calendars`, made the first time Temporal names a non-ISO calendar, once per Intl data |
| `JsTemporal.RulesOf` | A zone's `ITimeZoneRules`: an offset zone's `FixedOffsetTimeZoneRules`, a named zone's from the database, which reads its primary identifier's zone, and in a realm without Intl data UTC's alone |
| `JsCalendarSystem.SharedYear` | A table-backed calendar's year from `CalendarYears.GetYear`, as this profile's `JsCalendarYear` |

### 3. Tables, the generator and rule I4

**The home's generator writes both files**, run test-only in the architecture tests:
`TimeTableEmbedding` hands the home's `TzdbPin`, `CldrPin` and `CalendarPin` this repository's own pins
and archives - `src/tests/tzdb/pins/`, `src/tests/cldr/pins/` and `src/tests/calendars/pins/`, which stay
here unchanged - generates with its `TzdbTableGenerator` and `CalendarTableGenerator`, and writes the two
files with its `TzdbTableSource` and `CalendarTableSource` and this family's namespace, class, header,
summaries and annotation lines, through the assurance generator as before. **`JsTzdbTables.g.cs` and
`JsCalendarTables.g.cs` come out byte for byte** (JSD-0076's falsification clause), and the two write
switches, `BROILER_TZDB_WRITE` and `BROILER_CALENDARS_WRITE`, still write them. Rules N29 and N31 verify
every archive and member with the home's readers; N30 and N32 regenerate and compare, and each refuses a
changed byte of a generated table. Rule N28's budget is unmoved, since no table changed.

**N29 to N32 gain their register rows**, which they lacked (the milestone's clause 13): the register counts
129 active rules of 130.

**Rule I4 holds the time data too.** The home's generator package carries no pin file, so its pins are
recorded in the rule as digests, with the Broiler.JS commit they were read at, `2d37a38a`: this
repository's `tzdb.pin` and `calendars.pin` equal the home's by length and SHA-256, and `cldr.pin` names
CLDR 48.2.0's `bcp47/timezone.json`, the one file the home's `cldr.pin` names, with the home's length and
SHA-256. And every data version the generator package declares - `TimeDataVersions`' `TzdbRelease`,
`CldrVersion`, `IcuCalendarVersion` and `CalendricalCalculationsVersion`, and its tz and CLDR pin readers'
`PinnedVersion` - equals the version this repository's governing pin states, read from the package image
through metadata; a declared version no pin governs is refused. The Unicode clause no longer reads those
two `PinnedVersion` fields. Its witnesses are the tz pin with one hash changed and a generator declaring
tzdb 2026d. A move of the home's pins is a dated decision in both repositories at once, and the constants
move with it.

### 4. Adapters

Each calls the shared library and keeps what the language and this family decide - argument conversion
and its order, the parsing of ISO strings and annotations, the errors and their messages, and the
policies JSD-0076 leaves each engine - and computes no offset, instant, transition, identifier or
calendar year itself:

| Adapter | What it keeps | What it calls |
|---|---|---|
| `JsTemporal`'s `OffsetNanosecondsFor`, `PossibleEpochNs`, `Disambiguate`, `EpochNsFor`, `StartOfDay`, `NextTransition` and `PreviousTransition` | The conversion of this profile's records; an offset zone's days range checked first, for its own message; each status mapped to its `RangeError` (below) | `ZonedTime`'s operation of the same name over `RulesOf` |
| `JsTemporal.TimeZoneEquals` | A realm without Intl data, which stores no named zone but `UTC`, compares its zones as strings | `ZonedTime.TimeZoneEquals` over the database |
| `JsTemporal.ParseZone`, `ToTimeZoneIdentifier`, `TimeZoneFromIdentifier` and `NamedZone` | The proposal's steps, the TypeError and RangeErrors, a realm without Intl data knowing `UTC` alone | `OffsetTimeZones.TryParseOffsetIdentifier` and `FormatOffsetIdentifier`; the database's `TryFind` |
| `JsTemporalCore.FormatOffsetRounded` | The rounding to the minute | `OffsetTimeZones.FormatOffsetIdentifier` |
| `Intl.DateTimeFormat`'s `ResolveTimeZone` | The option's conversion, its two `RangeError`s, and the formatter's report of the primary identifier (Z8) | `OffsetTimeZones.IsTimeZoneOffsetString`, `TryParseOffsetIdentifier`, `FormatOffsetIdentifier` and `TryGetEtcGmtOffset`; the database's `TryFind`, `Primary` and `Rules` |
| The formatter (`JsDateTimeFormatter`) and the zone names (`JsZoneNames`) | The names, the metazones and their periods, which are SJL-9's | `ITimeZoneRules.OffsetSecondsAt`, `IsDaylightAt`, `NextTransition` and `PreviousTransition` with the daylight flag; the database's `RegionTimeZones`, `TryFind`, `Primary` and `Rules` |
| `Intl.Locale.prototype.getTimeZones` | The array and its order | The database's `RegionTimeZones` |
| `chinese`, `dangi`, `persian` and `islamic-umalqura` (`Year`) | The calendars' arithmetic over the years, which is SJL-10's; the tabular Islamic calendars and the formula calendars stay this profile's | `CalendarYears.GetYear` over `CalendarTables`, through `SharedYear` |

**Each status is the `RangeError` this family threw before, with the same message** (X3: "each engine maps
a status to its own error"), in `JsTemporal.ZoneError`:

| Status | Message |
|---|---|
| `Ambiguous` (a repeated wall time under `reject`) | `Temporal: the local time is ambiguous in` and the zone |
| `Skipped` (a skipped wall time under `reject`) | `Temporal: the local time does not exist in` and the zone |
| `NoStartOfDay` | `Temporal: the start of the day cannot be found in` and the zone |
| `OutOfRange` | `Temporal: the date-time is outside the representable range` |

An offset zone's wall time outside the days range is refused before the shared operation is called, with
`Temporal: the date is outside the representable range`, as before; the shared operation would report the
same input as `OutOfRange`. `GetYear`'s `OutOfRange`, a year past `CalendarYears.MaxYearMagnitude`,
300,000, is not mapped: every caller keeps `JsTemporal.YearBound`, which is now that constant, and
`SharedYear` throws `InvalidOperationException` if one did not, an engine defect and not a script's error.

### 5. Charging and limits

**Nothing in the area charges, and nothing does now** (JSD-0076's X3 and decision 4): no shared member
takes a meter, and no adapter added a charge. So there is no class C2, and every fuel-parity twin keeps
its base figure (section 7).

**The limits are the shared library's.** The rules answer instants within `TimeZoneDatabase.MaxEpochSeconds`,
10^15 seconds, far beyond ECMAScript's; `ZonedTime` answers at most `MaxPossibleEpochNanoseconds`, three,
candidates; and `CalendarYears` answers years within ±300,000, the bound `JsTemporal.YearBound` already
was.

### 6. Corrections

**None.** JSD-0076 found that this family has no correction of its own in the area; every correction it
names is Broiler.JS's, at SJL-8.6. Nothing this change does is admitted as a correction, and section 7's
checks find no answer that moved.

### 7. Evidence at this change

Checked locally at `eed568a`'s tree, whose product sources are the cut-over's with `8613309`; the test262
subset was also run at `186a1d3`'s, with the same result. Part A's and part B's hosted after runs are
SJL-8.7's.

**The generated tables**: regenerated by the home's generator in write mode, both files are left
byte-identical to the base's, and each changed byte of a generated table is refused by N30 or N32
(section 3).

**The reference datasets**, part B's seven named checks, through the slice compiler built in Release and
run with `--checks --verbose`: each prints the line the base's `named-checks.txt` gives, and no file under
`src/tests/cldr/timezones/` or `src/tests/temporal/` changed. The compiler passes 649 checks - the base's
648 and section 10's - and leaves three not run that are another calling convention's.

| Check | Lines | As the divergences name |
|---|---|---|
| `tzdb/time-zone-offsets-match-icu` | 610 | 51 |
| `temporal/t1/temporal-matches-the-reference-polyfill` | 1,629 | 0 |
| `temporal/t2/intl-over-temporal-matches-the-reference-polyfill` | 1,233 | 82 |
| `temporal/t3/calendars-match-the-reference-polyfill` | 3,999 | 70 |
| `temporal/t3/calendar-conversions-match-icu4x` | 2,055 conversions | - |
| `temporal/t4/calendar-formats-match-the-reference-polyfill` | 5,951 | 1,395 |
| `temporal/t5/zone-names-match-the-reference-polyfill` | 2,713 | 275 |

**The shared vectors**, at this family's pin of the corpus, revision 22 (Broiler.JS `4ce20ac`, archived
under `src/tests/vectors/pins/`): 92,434 vectors, 92,434 match, 0 classified divergences and 0
unclassified differences.

**A local test262 subset in the bytecode form**, `eng/run-test262.py` over `test/built-ins/Temporal`,
`test/intl402/Temporal`, `test/intl402/DateTimeFormat`, `test/intl402/Intl`, `test/intl402/Locale`,
`test/intl402/Date`, `test/built-ins/Date`, `test/staging/Temporal` and `test/staging/sm/Date`, under the
hosted run's manifest, fuel and wall clock, compared per path and variant with the base's hosted report
in bundle `sjl-8-001`: 7,696 files and 15,330 variants, every one with the base's verdict - 15,240 passed,
12 failed, 16 exhausted and 62 skipped on both sides. The sixteen exhausted variants are
`test/staging/sm/Date/dst-offset-caching-*.js`, which read no zone, since this profile's local time is UTC.
Eleven of them ran out of wall clock locally, under four parallel processes, where the hosted base's ran
out of fuel; the hosted base's own sixteen were thirteen on fuel and three on the wall clock. Run again
with a wall clock of 120 seconds, all sixteen exhaust on fuel.

**Part C's twins**, through the command-line root, each at the base figure `base/fuel-parity.log` gives
and at one less, in both forms: every one completes at its figure with the base's answer and is refused
with `AllowanceExhausted on Fuel` at one less, line for line as the base's log.

| Twin | Figure | Answer |
|---|---|---|
| `wide/array-loop.small.js` | 73,082 | 999000 |
| `wide/call-loop.small.js` | 28,026 | 1000 |
| `wide/closure-loop.small.js` | 31,044 | 1000 |
| `wide/fib.small.js` | 7,682 | 144 |
| `wide/generator-loop.small.js` | 40,075 | 499500 |
| `wide/numeric-loop.small.js` | 23,022 | 498497 |
| `wide/property-loop.small.js` | 29,029 | 183125 |
| `wide/string-loop.small.js` | 26,083 | 25 |
| `wide/throw-loop.small.js` | 34,026 | 1000 |
| `wide/try-loop.small.js` | 34,022 | 499500 |

**The rules.** The architecture tests pass, 385 of 385, the two new tests of rule I4 among them, and the
solution builds in Release with no warning.

### 8. Deleted

The code JSD-0076's X6 lists name for this family is deleted, and the 24 members and types among it are
registered in rule I5's `retired-members.json`, each with what replaced it:

- **the tz database** (`JsTimeZones.cs`, whole): `JsTimeZones`, its decoding and lookup; `JsTimeZoneId`;
  `JsZone`, the zone decoder and zone rules, with `JsZone.MondayFirst`, which JSD-0073 kept as an adapter
  and which went with its type; and `JsZoneRule`;
- **the zone algorithms**: in `JsTemporal`, `NamedOffsetSeconds`, `FloorSeconds` and `NsPerSecond`, and
  the bodies of the operations section 4 lists; in `JsRealm`, `ResolveTimeZone`'s core, `EtcGmtZones` and
  the offset parser `ParseOffset`; and `JsTemporalCore.FormatOffsetMinutes`;
- **the calendar tables**: `JsCalendarData`, the table's decoder; and the ported year sources outside the
  tables, in `JsEastAsianCalendar` ICU4X's approximation - `Approximate`, `Moment`, `OnOrBefore`,
  `RataDie`, `Day`, `UtcPlus8`, `UtcPlus9`, `BeijingMeanTime`, `MeanYear`, `MeanSolarTerm` and
  `MeanSynodicMonth` - and in `JsPersianCalendar` the 33-year rule with its corrections, `Corrected` and
  `NewYear`. `islamic-umalqura` no longer falls through to this profile's tabular years outside its
  table, which stay for the tabular Islamic calendars: the shared source answers it inside the table and
  outside it.

**What moved to the home**, and is no longer in this repository: the tz compiler `TzdbCompiler`, the table
generators `TzdbTableGenerator` and `CalendarTableGenerator`, and their pin readers `TzdbPin` and
`CalendarPin`, all test code of the architecture tests (`c5943b1`).

**What stays, and holds no zone or calendar-year algorithm:** the adapters of section 4; the pins, the
archives and the two generated files; rules N29 to N32; `JsIntlTables`' two lazy fields; the calendars'
arithmetic over the years and the formula and tabular calendars (SJL-10's); the zone names and the
formatter's patterns (SJL-9's); and the reference datasets under `src/tests/cldr/timezones/` and
`src/tests/temporal/`, unchanged (JSD-0076's decision 7).

### 9. Rules

- **I1**: the allow-list's product entry for `Broiler.ECMAScript.Algorithms` names `0.1.0-preview.5`, and
  a test-only entry admits `Broiler.ECMAScript.Generation` for the architecture tests alone; the graph
  manifest records the new package edge.
- **I4**, revised with a dated note (section 3).
- **I5** registers the 24 (section 8); its statement gains a dated revision, and its test reads the files
  the adapters live in, `JsTemporal.Zones.cs`, `JsCalendars.cs`, `JsRealm.DateTimeFormat.cs` and
  `JsZoneNames.cs`, in place of `JsTimeZones.cs`, which is deleted.
- **N29 to N32** regenerate and verify with the home's readers and generator (section 3), and their
  tests' remarks say so, dated. N32 reads the years the table covers from its section headers - China's
  and Korea's 1912 to 2102, the Qing years 1900 to 1911, Umm al-Qura 1300 to 1600 AH and 78 Persian
  corrections from 1502 AP - where it read a handful of known dates from the generator's parse, whose
  readers are internal to the home's package; the home's own tests hold those dates.
- **The rule register** counts 129 active rules of 130.

### 10. Public API

The family's preview API changes, and `docs/api/public-api.txt` records it:

- **added**: Format's `JsIntlTable.TimeZoneRelease` (31), the tz release the shared database states for
  the tables.

**An embedder's own `IJsIntlData`** that does not answer it - every one written before this change - reads
as before: the profile hands the database the release `unstated`, and nothing in the profile reads the
release (`8613309`). The zone, identifier, region and calendar tables keep their numbers and layouts, which
the shared library's readers read.

### 11. Records

- **Notices.** `THIRD_PARTY_NOTICES.md`'s row for `Broiler.ECMAScript.Algorithms` names `0.1.0-preview.5`
  and its third third-party component, the C# port of ICU4X's lunisolar approximation under the Unicode
  License v3, the port this family's `JsCalendars` carried; the row is still not confirmed, so release
  gate 12's blocker for it stands, under the same name. A build-and-test row names
  `Broiler.ECMAScript.Generation`. The tzdb and ICU4X entries say, dated, that the generators are the
  home's, and the ICU4X entry that the port is the package's and this repository compiles no copy.
- **Packages.** `packages.md`'s baseline is bundle JS-10-009's ([JSC-312](../roadmap.corrections.md#jsc-312));
  `support.md` names the bundle.
- **Governance.** The programme's `governance.md` rows for I4 and N29 to N32 say what the rules now do.
- **Assurance.** The record is regenerated. The code that left this family's assurance coverage is covered
  by the home's own assurance component in the Broiler.JS repository, as EX-125 and EX-127 say of the
  earlier areas; bundle `sjl-8-001` names it with SJL-8.7's evidence.

## Departures from the plan

Each is this record's, and none edits JSD-0076:

- **The tables carry their release.** The shared database states the release its tables were compiled
  from, which no table carried, so the Intl data answers a new table, `TimeZoneRelease` (section 10).
- **The database lists no regions.** `JsZoneNames` named every country with zones by iterating
  `JsTimeZones`' regions, and `ITimeZoneDatabase` answers a region's zones but not the list. zone.tab's
  regions are ISO 3166 codes, two ASCII capitals, so it asks the database for each of the 676 codes `AA`
  to `ZZ`, in ordinal order, the order the table lists them in, once per Intl data; a code zone.tab does not
  list has no zones and is skipped.
- **A Chinese or Korean month count stops at the years' bound.** These two calendars count months from
  their years, which the shared sources answer only within ±300,000. `BalanceMonths` returns null where a
  count would read a year past the bound, as it does for an estimate past it. No date within the ISO
  limits is in such a year, and every caller's `RangeError` is the one that year's date gave, but for one
  path no test or dataset reaches: under `overflow: 'reject'` a month count past the bound reports the
  date out of range where the day's regulation would have reported the day.
- **The generated files keep two stale references**, because the falsification clause of JSD-0076 holds them
  byte for byte: `JsTzdbTables.g.cs`' and `JsCalendarTables.g.cs`' headers name the generators as
  "(src/tests/Broiler.VM.Architecture.Tests)", and the calendar table's summary names its reader then,
  `JsCalendarData`. `TimeTableEmbedding` passes the old text as the writer's options and says so; a later
  regeneration that may change the bytes corrects both.
- **N32's known dates are the home's tests'** (section 9).
- **The changes are six before this record.** The package's admission and the baseline it forced came
  first, so that every later change built from the committed feed; the generator's admission and rule I4
  came before the cut-over, so that the tables were proved byte for byte before any reader changed.

## Correction entries

[JSC-312](../roadmap.corrections.md#jsc-312) (`packages.md` and `support.md`, the package baseline). No
other plan document of this profile says what changed.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-8 is met.** Its evidence is bundle `sjl-8-001`, read under its rules at SJL-8.7, and the
  ledger records the result.
- **Broiler.JS's cut-over**, SJL-8.6, which its own changelog records with its corrections and policy
  changes.
- **Publishing `0.1.0-preview.5`** to nuget.org, which is the owner's to choose; this family cannot be
  released before it.
- **The formatter's resolved `timeZone` (Z8), the zone names** (SJL-9) **and the calendars' arithmetic**
  (SJL-10).
- **This family's UTC host zone**, which JSD-0053 section 7 states.
- **A tz release.** The pin stays 2026e.

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 4 computes an offset, an instant, a transition, an identifier or a calendar year
  itself;
- the home's generator, run from this family's pins, does not write `JsTzdbTables.g.cs` and
  `JsCalendarTables.g.cs` byte for byte;
- a status reaches a script as another error or message than section 4's table gives it;
- a vector that matched before this change does not after it, a reference dataset's check prints another
  line, a fuel-parity twin completes at a different allowance or with a different answer, or a test262
  verdict moves, without a dated decision admitting it.
