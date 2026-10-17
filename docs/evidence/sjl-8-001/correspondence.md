<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-8.1's correspondence from code: time-zone and calendar data in both engines

**Written:** 2026-10-17, at SJL-8.1, as the input of
[JSD-0076](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0076-time-zone-and-calendar-data-move-to-the-data-home.md)'s
X1, X2 and X6 and of the area's comparison rules, before any code of SJL-8 exists. **Read at** Broiler.VM `fc42cb3`,
whose product sources are `9508bd5`'s, the default branch after SJL-7's merges and the move to Broiler.Unicode
`0.1.0-preview.5`; and Broiler.JS `a1a96ca2`, whose sources under `Broiler.JS/`, `Broiler.ECMAScript/` and `scripts/`
are `6563786e`'s, its default branch after the same move. Broiler.JS's calendars consume the CLDR package
`Broiler.Unicode.Cldr.LocaleData` `0.1.0-preview.3`, built from Broiler.Unicode `2affa78`. **By reading, and by
running where a row says so.**
- A Broiler.VM cell marked *(run)* was run from the Release build of the checkout through the conformance root, over a
  test262-shaped directory (`correspondence-runs/tz/vm262/`, whose harness files are the pinned suite's), under the
  manifest `broiler.javascript.wide`, whose realm has Intl and Temporal and runs under invariant globalization; or,
  where the cell says so, through the command-line root, which has no Intl.
- A Broiler.JS cell marked *(run)* was run through the shell, `BroilerJS.dll --script-host`, built in Release, on
  .NET 10.0.12 with the host's ICU 74.2, with no `LANG` and `TZ=UTC` unless the row names another zone. The host is
  Ubuntu 24.04.5, whose installed tz data is `tzdata 2026c-0ubuntu0.24.04.1`.
- A cell marked *(host)* is behaviour that comes from .NET, ICU or the host's tz data; *(reading)* is code not
  executed.

SJL-8.4's vectors confirm or refute each row. **It holds no figure of a run**: the values below are inputs, the
answers they give, and counts of data.

**Paths.** In Broiler.VM, `VM/` is `src/Broiler.VM.Profile.JavaScript/`, `VI/` its `.Intl/`, `AT/` is
`src/tests/Broiler.VM.Architecture.Tests/`, `TP/` is `src/tests/tzdb/pins/` and `CF/` is
`src/compositions/Broiler.VM.Composition.JavaScript.Conformance/`. In Broiler.JS, `BI/` is
`Broiler.JS/Broiler.JavaScript.BuiltIns/`, `TZ/` is `BI/Temporal/Tz/` and `ZD` is
`BI/Temporal/JSTemporalZonedDateTime.cs`. `runs/` is [`correspondence-runs/tz/`](correspondence-runs/tz/) in sections 1
and 2 and in the facts and readings on time zones, and [`correspondence-runs/cal/`](correspondence-runs/cal/) in section 3
and in the facts and readings on calendars. Each holds every probe, each engine's
raw answer and the comparison of the two, named by row. A file over 100 KB is retained gzip-compressed under its name
with `.gz`. The calendar probe's .NET project and source are kept with `.txt` appended, so that no build and no
rule reads them as this repository's. Third-party texts the rows quote (the Temporal and Intl era and month code proposals, the Hong Kong
Observatory's 2057 table, tzdb's `NEWS` and `Makefile`) and the test262 files run are not retained: the rows cite them
by name, and the pinned suite and archive hold the last two. A line number is the line at the revision above; a bare
`:n` is in the file named last in the same cell or bullet.

## Facts

**The time zones.**

- **Broiler.VM's data is tzdb 2026e, pinned and compiled in the repository.** `TP/tzdb.pin:20-42` records the
  tarball's length, SHA-256 and SHA-512 and twelve members' hashes; rule N29 holds them (`AT/TzdbRuleTests.cs:23-92`).
  `AT/TzdbCompiler.cs:54-55` compiles nine files (no `backzone`, no `factory`); `AT/TzdbTableGenerator.cs:63-89`
  writes `VI/JsTzdbTables.g.cs` (`Version = "2026e"`, :40), which N30 compares byte for byte
  (`AT/TzdbRuleTests.cs:95-133`). The comparison with `zic` was one run of a prototype on 2026-10-05
  (`AT/TzdbCompiler.cs:45-48`, JSD-0053 section 2); no test runs `zic`. **Run here:** the host's `zic -b fat` over the
  pinned nine files agrees with Broiler.VM's offsets at every transition and the second before, 1800 to 2037, for
  593 of the 597 identifiers; the four others are `CST6CDT`, `EST5EDT`, `MST7MDT` and `PST8PDT`, which Broiler.VM
  reads from their CLDR primaries by design (`VM/JsTimeZones.cs:96-105`), each differing only up to 1966-10-30
  (`runs/z10-vm-vs-zic.txt`).
- **Broiler.JS's data is tzdb 2025b, copied from a build host and not reproducible.** `scripts/tzdata/generate_tzdata.py:21-71`
  reads `/usr/share/zoneinfo/tzdata.zi` and each Zone's TZif file and raw-DEFLATEs them; nothing pins its input.
  The committed `TZ/tzdata.bin` is 128,107 bytes (SHA-256 `7b87bc44…`): release 2025b, 447 Zones and 151 Links,
  fat TZif files whose listed transitions end in 2037 (some with a no-op entry at 2038-01-19T03:14:07Z), `Factory`,
  and backzone's Zones for zone.tab's names
  (`runs/data/tzdata-bin-facts.txt`). The same script on this host writes a 2026c blob of 127,194 bytes (SHA-256
  `1f4be48e…`) *(run, host)* (`runs/data/regenerate.out`). The checkout is shallow (history from `44a214dc`), so
  how often the blob was committed cannot be read here.
- **The identifier sets are equal but for `Factory`**: 597 names in Broiler.VM, the same 597 and `Factory` in
  Broiler.JS; no name was added or removed between 2025b and 2026e (`runs/data/identifier-sets.txt`). Twelve names
  differ in their primary identifier (Z7).
- **Over 1800 to 2037 the two engines' offset histories agree for 465 of the 597 common names** *(run)*. 20 differ
  between 1970 and 2037, each one a change of tzdb 2025c to 2026e. 112 differ only before 1970: 110 by backzone and
  2 (`Europe/Dublin`, `Eire`) by 2026e's correction of 1925 (`runs/z10-transitions-compare.txt`,
  `runs/data/backzone-check.txt`). After 2037 Broiler.JS's offsets freeze (Z4); at 2150 the retained program's
  offsets differ for 201 names (Z16).
- **The zone algorithms agree on every input inside the data both engines share**: gaps and overlaps of 30
  minutes, an hour and 24 hours, a gap of 44 minutes 30 seconds, midnight gaps, transitions that change only the
  daylight flag, the limits of the instant range, offset zones and `Etc/GMT` zones (A1 to A4, A6, A8, A9, A12, A14). Their answers part
  only where the data does (release, backzone, the 2037 freeze), and at three of Broiler.JS's input checks (Z10,
  Z11, A13). The two fallbacks the design names, Broiler.JS's zero offset for an unknown name and its local
  midnight, are not reached through any entry point probed (A6, A11).
- **Broiler.JS reads one zone from two datasets.** Broiler.VM's `Date`, `Temporal.Now` and default `Intl.DateTimeFormat` zone are UTC whatever the host says. Broiler.JS's `Date` and its
  default formatter zone read the host's `TimeZoneInfo.Local` (here tzdata 2026c, which follows the TZif footer),
  while its `Temporal` reads `tzdata.bin` (2025b, frozen after 2037), so the two disagree on one zone inside one
  engine (A15).

**The calendars.**

- **Broiler.VM's sources are pinned and generated.**
  - `calendars.pin` records `icu_calendar` 2.3.0 and `calendrical_calculations` 0.2.4: archive length,
    SHA-256 and SHA-512, and nine members (`src/tests/calendars/pins/calendars.pin:20-37`). The
    extracted members hash as recorded.
  - Rule N32 regenerates `VI/JsCalendarTables.g.cs` (1,958 bytes) from them, and the decoded table
    equals the members (`runs/vm-table-decode.txt`).
  - The years cover Qing 1900–1911, China and Korea 1912–2102, Umm al-Qura 1300–1600 AH and the
    Persian corrections 1502–2987 AP. Outside them the profile uses ported code: ICU4X's approximation
    and the tabular and 33-year formulas.
  - Broiler.VM reproduces its retained dataset line for line: 3,999 lines, the 70 declared divergences
    included, and every one of ICU4X's 2,055 conversion lines (`runs/retained-calendars.vm.txt`).
- **Broiler.JS's sources are the runtime's and the CLDR package's, unpinned.**
  - From .NET 10.0.12 (`runs/netprobe-ranges.txt`): `ChineseLunisolarCalendar` 1901-02-19..2101-01-28,
    `KoreanLunisolarCalendar` 0918-02-19..2051-02-10 and `PersianCalendar` 0622-03-22..9999-12-31.
  - From `Broiler.Unicode.Cldr.LocaleData` `0.1.0-preview.3`: `CldrChineseCalendar` for every other
    `chinese` and `dangi` year, and `CldrUmmAlQura`.
  - Outside .NET's Persian range, the 33-year rule.
- **The two Umm al-Qura tables are identical.** All 301 years, 1300 to 1600 AH, have the same 1
  Muharram and month lengths (`runs/umalqura-table-compare.txt`). Both fall back to `islamic-civil`,
  and the engines agree on every probed date (K8, K9).
- **Broiler.JS's calendar answers do not vary with the host, but they do vary with the runtime.**
  - Invariant globalization, `fa-IR`/Tehran and `ko-KR`/Seoul give byte-identical output (K14).
  - The lunisolar tables and the Persian astronomy are code and data of `System.Private.CoreLib`
    (`runs/netprobe-reflect.txt`). The shell runs on whichever 10.0.x patch is installed.
  - Only 10.0.12 was available, so no difference between runtime patches was observed.
- **Inside Table 1's published ranges the engines differ in one year only**: `chinese` 2057 (K2).
  Everywhere else the differences are in years Table 1 leaves to an implementation-defined
  approximation (K4–K6, K11), or they are defects of Broiler.JS at the far dates and at .NET's bounds
  (K7, K12).

## 1. The tz database

**Broiler.VM** compiles its own tables from the pinned archive and decodes them lazily.
- `AT/TzdbCompiler.cs:68-128` parses Rule, Zone and Link lines of the nine files (:54-55); `:280` compiles a zone as
  `zic` does, for offsets and the daylight flag (:28-50).
- `AT/TzdbTableGenerator.cs:103-155` writes the zone table: per zone a first offset, a palette of offsets (twice the
  offset, plus one for daylight), each transition as a varint delta in minutes or seconds and a palette index,
  then the recurring rules with the standard offset and the last listed year.
- `AT/TzdbTableGenerator.cs:194-251` writes every identifier with the zone it reads and its primary: CLDR 48.2.0's
  `bcp47/timezone.json` key (`_iana`, or its first alias) where CLDR names it (:43, :208-227), tzdb's Links
  otherwise (:229-241), and `Etc/UTC`, `Etc/GMT`, `GMT` as `UTC` (:243-246). `:254-260` writes zone.tab's zones per
  region.
- `VI/JsTzdbTables.g.cs` holds a 78,428-byte binary zone table, 597 identifier lines (445 primaries) and 247 region
  lines (`runs/data/vm-table-sizes.txt`); `VI/JsCldrData.cs:98-100` hands them to `VM/JsIntlTables.cs:107`.
- `VM/JsTimeZones.cs:84-105` loads the identifiers and points each non-primary at its primary's zone, so
  `EST5EDT` reads New York's history; `:113-124` finds each zone's start in one pass; `:161-162` decodes a zone the
  first time it is read; `:134-145` looks a name up: a character above U+007F refuses it, and otherwise an
  `OrdinalIgnoreCase` dictionary matches (:56-57).
- `JsZone.OffsetAt` (`VM/JsTimeZones.cs:378-388`) continues a zone after its last listed year by its recurring
  rules (:515-531), without limit.
- `Intl.DateTimeFormat` resolves a name to its primary (`VM/JsRealm.DateTimeFormat.cs:1197-1216`);
  `Intl.supportedValuesOf` lists the primaries (:263-266); `Intl.Locale.prototype.getTimeZones` lists zone.tab's
  zones (`VM/JsRealm.Locale.cs:174`).
- Only roots that hand in `IntlData` carry the data, for example the conformance root (`CF/Test262Manifests.cs:142`);
  the command-line root's build output holds no `Broiler.VM.Profile.JavaScript.Intl.dll`.

**Broiler.JS** embeds a host's compiled zoneinfo and reads it as TZif.
- `BI/Broiler.JavaScript.BuiltIns.csproj:12-14` embeds `TZ/tzdata.bin` in every build.
- `TZ/IanaTimeZoneDatabase.cs:96-135` inflates the whole blob on first use (:103-108), adds eleven hard-coded legacy
  aliases as Links (:81-94, :119-123), and folds every name with `ToUpperInvariant` (:125-127). `:58-77` parses a
  zone's TZif file on its first `Get` and caches it under one global lock.
- `TZ/TzifReader.cs:11-34, 66-103` reads the 64-bit data block and never the footer's TZ string, so
  `TZ/IanaTimeZone.cs:42-56` answers the last type for every later instant. Its initial type is the first
  non-daylight type (`TZ/TzifReader.cs:94-100`), where RFC 8536 says type 0; they are the same type in every zone of
  the blob (`runs/data/initial-type.txt`).
- A name's primary is its Link target, one hop (`TZ/IanaTimeZoneDatabase.cs:47-53`), with the UTC family as `UTC`
  (`ZD:1555-1561`); `Intl.supportedValuesOf` lists the names that are their own primary (`ZD:1569-1580`,
  `BI/Intl/JSIntl.cs:196-203`).
- `Intl.DateTimeFormat` keeps the name, case-normalised, through `CanonicalizeTimeZoneId`
  (`BI/Intl/JSIntl.cs:7388-7408`), which also accepts an ISO string's zone (`ZD:1598-1608`).
- `Intl.Locale.prototype.getTimeZones` answers one zone from a hand table of fourteen regions, or `UTC`
  (`BI/Intl/JSIntl.cs:4106-4115, 4284-4299`).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| Z1 | offsets at 2027-01-15T12:00Z in America/Vancouver, America/Edmonton, America/Winnipeg, America/Inuvik, Africa/Casablanca; Europe/Chisinau at 2026-03-29T00:30Z and 2026-10-25T00:30Z; Europe/Dublin at 1925-09-25T12:00Z; America/Bogota at 1992-05-02T12:00Z; the next transition after 2026-07-01 in Vancouver and after 2026-10-01 in Casablanca *(run)* | −07:00, −06:00, −05:00, −06:00, +00:00; +02:00, +03:00; +00:00; −04:00; `null`, `null` (2026e's Vancouver change of 2026-11-01 keeps −07:00, so it changes no offset) | −08:00, −07:00, −06:00, −07:00, +01:00; +03:00, +02:00; +01:00; −05:00; 2026-11-01T01:00−08:00, 2027-02-07T02:00+00:00 | **policy difference under SJL-D-20 (release)**: the changes of 2025c (Baja California), 2026a (Moldova), 2026b (British Columbia), 2026c (Alberta, Morocco), 2026d (Northwest Territories, Colombia 1992, Iran 1979) and 2026e (Manitoba, Ireland 1925). Vancouver and Casablanca on 2026-07-01 agree. `runs/z1-release.compare.txt` |
| Z2 | the offset history 1800 to 2037 of each of the 597 common names, every change of offset *(run)* | — | — | 465 identical. The 20 that differ from 1970 on are exactly the release's: Tijuana, Ensenada, Santa_Isabel, Mexico/BajaNorte; Chisinau, Tiraspol; Vancouver, Canada/Pacific; Edmonton, Canada/Mountain, Yellowknife; Casablanca, El_Aaiun; Inuvik, Bogota, Tehran, Iran; Winnipeg, Canada/Central, Rainy_River: **policy difference (release)**. The 112 that differ only before 1970 are Z3's and Dublin's. `runs/z10-transitions-compare.txt` |
| Z3 | Europe/Amsterdam at 1850-01-01, 1937-07-01 and 1939-01-15; Atlantic/Reykjavik at 1900-01-01 and 1960-01-15; their offset changes 1800 to 1970; both in 1980 *(run)* | +00:17:30, +01:00, +00:00; −00:16:08, +00:00; 62 and 1 changes, Brussels' and Abidjan's histories (both are Links in the default build); 1980 agrees | +00:19:32, +01:20, +00:20; −01:28, −01:00; 57 and 68 changes, their own; 1980 agrees | **policy difference under SJL-D-20 (backzone)**. Over all names: 95 that are Links in the default build and backzone Zones in `tzdata.bin`, and 15 Links that backzone points elsewhere (`Arctic/Longyearbyen` to Oslo, not Berlin). The blob holds 102 of backzone's 125 Zones; the 23 it lacks are names zone.tab does not list *(run)*. `runs/z2-backzone.compare.txt`, `runs/data/backzone-check.txt` |
| Z4 | America/New_York and Europe/Berlin at 2040-07-01T12:00Z; New York at 2400-07-01; America/Santiago and Australia/Sydney at 2040-07-01; `Intl.DateTimeFormat` hour in New York and Berlin at 2040-07-01T12:00Z *(run)* | −04:00, +02:00; −04:00; −04:00, +10:00; `08`, `14` | −05:00, +01:00; −05:00; −03:00, +11:00; `7`, `13` | **policy difference under SJL-D-20 (extrapolation)**. The frozen offset is whichever follows the last transition of 2037, so southern zones freeze on **daylight** time (Santiago, Sydney), a case the area's example does not show. 2037-07-01 agrees in both. The hour's padding (`08` against `7`) is SJL-9's. `runs/z3-extrapolation.compare.txt` |
| Z5 | `new Temporal.ZonedDateTime(0n, "Factory")`, `"factory"`, `new Intl.DateTimeFormat("en", {timeZone: "Factory"})`, `Intl.supportedValuesOf("timeZone")` *(run)* | `RangeError` ×3; not listed | `1970-01-01T00:00:00+00:00[Factory]`, `Factory`, `Factory`; listed | **policy difference under SJL-D-20 (Factory)**. Broiler.VM's pin and compiler call the nine files those "`zic` compiles by default" (`TP/tzdb.pin:10-11`, `AT/TzdbCompiler.cs:53`); the release's own `Makefile` has `NDATA= factory` and `TDATA= $(YDATA) $(NDATA) $(BACKWARD)` (lines 656 and 658 of the pinned tarball's `Makefile`) *(reading)*. `runs/z4-factory.compare.txt` |
| Z6 | every name of either set as `new Temporal.ZonedDateTime(0n, name).timeZoneId` *(run)* | 597 accepted, each in tzdb's case; `Factory` a `RangeError` | 598 accepted, the same case | **agreement** but for Z5. `runs/z6-every-identifier.diff.txt` |
| Z7 | `Intl.supportedValuesOf("timeZone")`; `equals` of Europe/Bratislava and Europe/Prague, Arctic/Longyearbyen and Europe/Oslo, Europe/Busingen and Europe/Zurich, Europe/Vatican and Europe/Rome, America/Marigot and America/Port_of_Spain; of Asia/Calcutta and Asia/Kolkata, Europe/Kiev and Europe/Kyiv, EST5EDT and America/New_York, CET and Europe/Brussels, Etc/GMT and UTC *(run)* | 445 names; `false` ×5; `true` ×5 | 435 names: 11 fewer, and `Factory`; `true` ×5; `true` ×5 | **policy difference under SJL-D-20 (primaries)**: twelve names differ in their primary (`America/Kralendijk`, `Lower_Princes`, `Marigot`, `St_Barthelemy`, `Arctic/Longyearbyen`, `Atlantic/Jan_Mayen`, `Europe/Bratislava`, `Busingen`, `Mariehamn`, `Podgorica`, `San_Marino`, `Vatican`), each one CLDR keeps as a key and tzdb links away. Broiler.JS's eleven legacy aliases equal CLDR's. `runs/z5-supported.diff.txt`, `runs/z8-primaries.compare.txt`, `runs/data/identifier-sets.txt` |
| Z8 | `new Intl.DateTimeFormat("en", {timeZone}).resolvedOptions().timeZone` for Asia/Calcutta, Europe/Kiev, US/Eastern, EST5EDT, CET, Etc/UTC, GMT, `gmt`, Etc/GMT0, and for all 598 names; `timeZoneId` of the same in Temporal *(run)* | Asia/Kolkata, Europe/Kyiv, America/New_York, America/New_York, Europe/Brussels, UTC, UTC, UTC, UTC; the primary for all 152 non-primary names; Temporal keeps the name | the name, case-normalised, for all 598; Temporal keeps the name | **policy difference under SJL-D-20 (primaries)**, whose `canonical-tz` behaviour "follows the specification edition both engines pin". Broiler.VM cites ECMA-402 11.1.2 step 36 (JSD-0053 section 4) and skips test262's `canonical-tz` cases; Broiler.JS cites the canonical-tz text (`ZD:1614-1619`) *(reading)*. Temporal's `timeZoneId` agrees. `runs/z6-every-identifier.diff.txt`, `runs/z8-primaries.compare.txt`, `runs/a7-etcgmt.compare.txt` |
| Z9 | `europe/berlin`, `EUROPE/BERLIN`, `eUrOpE/bErLiN`, `utc`, `etc/utc`, `etc/gmt+5` in the constructor and `Intl.DateTimeFormat`; `[EUROPE/BERLIN]` in `ZonedDateTime.from`; `Europe/Berlin ` and `Europe/Berlin\u0000` *(run)* | `Europe/Berlin` ×3, `UTC`, `Etc/UTC` (formatter `UTC`), `Etc/GMT+5`; `Europe/Berlin`; `RangeError` | the same, but the formatter keeps `Etc/UTC` (Z8) | **agreement** on ASCII case. `runs/z7-lookup.compare.txt`, `runs/a7-etcgmt.compare.txt` |
| Z10 | `America/Loſ_Angeles` (U+017F) in the constructor, the formatter and `toZonedDateTimeISO`; `Europe/Berlın`, `Europe/Kıev` (U+0131), `Europe/İstanbul` *(run)* | `RangeError` for each (`VM/JsTimeZones.cs:136-142`) | U+017F: accepted as `America/Los_Angeles` three times; U+0131 and U+0130: `RangeError` *(host: .NET's invariant upper-casing maps U+017F to `S` and leaves U+0131)* (`TZ/IanaTimeZoneDatabase.cs:38`) | **correction of Broiler.JS**: GetAvailableNamedTimeZoneIdentifier returns a record only where its identifier "is an ASCII-case-insensitive match" for the input; SJL-D-20's lookup rule is that one. `runs/z7-lookup.compare.txt` |
| Z11 | an ISO string as a zone: `{timeZone: "2020-01-01T00:00[Europe/Berlin]"}` and `{timeZone: "2020-01-01T00:00Z"}` in `Intl.DateTimeFormat`; the first in `toZonedDateTimeISO` and in the `ZonedDateTime` constructor *(run)* | `RangeError`, `RangeError`; `Europe/Berlin`; `RangeError` | `Europe/Berlin`, `UTC`; `Europe/Berlin`; `RangeError` | **correction of Broiler.JS**: CreateDateTimeFormat takes an offset string or GetAvailableNamedTimeZoneIdentifier of the value and throws a `RangeError` when that is empty; only Temporal's ToTemporalTimeZoneIdentifier reads a zone out of an ISO string. The formatter reaches `CanonicalizeTimeZone`'s ISO fallback (`ZD:1603-1605`). Not in the area's table. `runs/z7-lookup.compare.txt` |
| Z12 | `new Intl.Locale("en-" + r).getTimeZones()` for US, DE, AT, IN, UA, GB, AQ, CN, ZZ (the retained program's last lines) *(run)* | zone.tab's zones: US 29, DE `Europe/Berlin Europe/Busingen`, AT `Europe/Vienna`, IN `Asia/Kolkata`, UA `Europe/Kyiv Europe/Simferopol`, GB `Europe/London`, AQ ten, CN two, ZZ none | one zone each from a hand table of fourteen regions, else `UTC`: US `America/New_York`, DE `Europe/Berlin`, AT `UTC`, UA `UTC`, AQ `UTC`, CN `Asia/Shanghai`, ZZ `UTC`; IN and GB agree | **correction of Broiler.JS**: the Intl Locale Info proposal's TimeZonesOfLocale answers the IANA zones in common use in the region, and none where there are none; its own comment says "Region→IANA time-zone data is not bundled" (`BI/Intl/JSIntl.cs:4293-4294`). The regions are tz data (area section 1) the area's table omits. `runs/z11-reference-timezones.compare.txt` |
| Z13 | the table format and its decoding *(reading; sizes run)* | the compact binary table of 78,428 bytes and two text tables; a zone decoded on first read | a DEFLATE blob of 128,107 bytes, all 492,972 bytes inflated on first use; a zone's TZif parsed on first `Get`, cached under one lock; the footer never read | **implementation-defined**: nothing observable but Z4; the format becomes the home's (SJL-8.2). Both keep the rearguard daylight flag (`AT/TzdbCompiler.cs:39-42`; the host data is in rearguard form, Europe/Dublin's summer `isdst` 1), which only zone names read: SJL-9's |
| Z14 | `typeof Intl`, `typeof Temporal`, `typeof Date.prototype.toTemporalInstant` *(run)* | `undefined` ×3 in the command-line root; present in the conformance root | always present: an `EmbeddedResource` of `BuiltIns` (`BI/Broiler.JavaScript.BuiltIns.csproj:14`) | **implementation-defined**: optionality stays each engine's (area section 3.3). `runs/z9-optional-cli.vm-cli.txt` |
| Z15 | regenerating each engine's data on this host *(run, host)* | N30 regenerates `VI/JsTzdbTables.g.cs` byte for byte from the pin *(reading)*; `zic -b fat` of the pinned files agrees but for four names by design (Facts) *(run)* | `generate_tzdata.py` writes tzdb 2026c, 127,194 bytes, where the committed blob is 2025b, 128,107 bytes; its Casablanca is already +00:00 | **policy difference under SJL-D-20 (release)**, with SJL-D-6 (c)'s pinned generator: Broiler.JS's input is whatever its build host installed. `runs/data/regenerate.out` |
| Z16 | the retained program `src/tests/cldr/timezones/timezones.js`, run unchanged *(run)* | 610 of 610 lines equal Node's answer with `divergences.txt` applied | 51 of 610 lines equal Broiler.VM's. The rest, a line often in several groups: the `longOffset` name drops seconds (`GMT-00:16` for `GMT-00:16:08`) on 404; the resolved name on 152 (Z8); the 2150 offset on 201 (Z4, and Z1 for Morocco and Canada); an offset before 1970 on 110 (Z3); seven region lines (Z12) and the listed names (Z7) | each difference is a row above, but the dropped seconds, which are **SJL-9's** (zone display names). `runs/z11-reference-timezones.compare.txt` |

## 2. The zone algorithms

**Broiler.VM** (`VM/JsTemporal.Zones.cs`, over `VM/JsTimeZones.cs`):
- the offset at an instant reads the zone at the floor second (:139-165);
- GetPossibleEpochNanoseconds samples the offset at the wall time read as UTC and a day either side, and keeps each
  candidate whose instant has that offset (:176-227); an offset zone balances and checks the day range (:181-191);
- DisambiguatePossibleEpochNanoseconds takes the first or last candidate, and in a gap moves the wall time by the
  difference of the offsets a day either side and samples again (:232-269);
- GetStartOfDay takes midnight's first candidate, or the next transition after the day before, and throws a
  `RangeError` if there is none (:280-295);
- transitions skip changes of the daylight flag alone (`VM/JsTimeZones.cs:507-511`) and continue by the recurring
  rules (:429-502) up to the instant limit (`VM/JsTemporal.Zones.cs:321-382`);
- an unknown name is a `RangeError` at `NamedZone` (`VM/JsTemporal.Zones.cs:131-134`), and `ResolveTimeZone` of the formatter takes an
  offset string without seconds, a name, the UTC family as `UTC` and `Etc/GMT±n` as a fixed offset
  (`VM/JsRealm.DateTimeFormat.cs:1175-1235`); `Temporal.Now` answers `UTC` (`VM/JsRealm.TemporalInstant.cs:258-262`).

**Broiler.JS** (`ZD`, over `TZ/IanaTimeZone.cs`):
- the offset at an instant reads the type at the floor second (`TZ/IanaTimeZone.cs:65-74`); an unknown name reads 0
  (`ZD:1349-1350`, :1377, :1415-1416);
- the candidates are two samples, the offsets a day either side, each kept if it round-trips (`ZD:1410-1435`);
- disambiguation picks the larger or smaller offset, and in a gap computes the instant from the two samples
  without sampling again (`ZD:112-143`);
- start of day takes the largest candidate at midnight, or the next transition after the day before, or **local
  midnight** if there is none or the name is unknown (`ZD:1499-1519`);
- transitions skip entries that change no offset, and end with the table (`TZ/IanaTimeZone.cs:84-122`, `ZD:394-434`);
- every entry point validates the name first (`ZD:54`, :92, :101, :1365, `BI/Intl/JSIntl.cs:7407`); offset strings
  are matched by .NET regular expressions (`ZD:1637-1651`, `BI/Intl/JSIntl.cs:2867-2886`); `Temporal.Now` and the
  default formatter zone read the host's `TimeZoneInfo.Local` (`BI/Temporal/JSTemporalNow.cs:21-37`), as `Date`
  does through `BI/Date/HostLocalTimeProvider.cs:21-56`.

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| A1 | `offset` and `offsetNanoseconds` of New York either side of both 2026 transitions; New York 1883 (local mean time); Kolkata; Lord Howe in January and July 2026; Kiritimati 1994-12-30 and 1995-01-02; Monrovia 1970; Paris 1900; Kathmandu; St Johns; Apia 2011-12-29 and -31; New York at −271821-04-20; `Instant.toString({timeZone: "Africa/Monrovia"})` *(run)* | −05:00, −04:00, −04:00, −05:00; −04:56:02; +05:30; +11:00, +10:30; −10:00, +14:00; −00:44:30 (−2670000000000); +00:09:21; +05:45; −02:30; −10:00, +14:00; −04:56:02; `1969-12-31T23:15:30-00:45` | the same | **agreement**: whole seconds in both, rounded to the minute only in strings. At +275760-09-13 Broiler.VM answers −04:00 and Broiler.JS −05:00 (Z4). `runs/a1-offsets.compare.txt` |
| A2 | the sampling, on both datasets *(run)* | three samples (`VM/JsTemporal.Zones.cs:197-213`) | two samples (`ZD:1418-1434`) | **agreement** in effect: in neither dataset is any offset in force for less than two days between two changes, 1800 to 2037 (the shortest periods are 167 and 95.7 hours), so the third sample never adds a candidate. The shared algorithm must keep three (or more) for data that has one. `runs/data/short-periods-both-engines.txt` |
| A3 | gaps × `compatible`, `earlier`, `later`, `reject`: New York 2026-03-08T02:30, Berlin 2026-03-29T02:30, Lord Howe 2026-10-04T02:15 (30 min), Kiritimati 1994-12-31T12:00 and Apia 2011-12-30T12:00 (24 h), Monrovia 1972-01-07T00:20 (44 min 30 s); `from` a bag in the gap, `withPlainTime("02:30")` *(run)* | New York: 03:30−04:00, 01:30−05:00, 03:30−04:00, `RangeError`; Monrovia: 01:04:30+00:00, 23:35:30−00:45 (the day before), 01:04:30+00:00, `RangeError` | the same on all 24, and on the bag and `withPlainTime` | **agreement** (only the messages differ). `runs/a2-disambiguation.compare.txt` |
| A4 | overlaps × the four modes: New York 2026-11-01T01:30, Berlin 2026-10-25T02:30, Lord Howe 2026-04-05T01:45 (30 min), Juneau 1867-10-19T12:00 (24 h); `from("2026-11-01T01:30-05:00[America/New_York]")`; an offset that does not match; `hoursInDay` on both 2026 days *(run)* | New York: −04:00, −04:00, −05:00, `RangeError`; Juneau: +15:02, +15:02, −08:58, `RangeError`; −05:00; `RangeError`; 23, 25 | the same | **agreement**. `runs/a2-disambiguation.compare.txt` |
| A5 | New York's gap 2040-03-11T02:30 and overlap 2040-11-04T01:30 × the four modes; `hoursInDay` of 2040-03-11 *(run)* | 03:30−04:00, 01:30−05:00, 03:30−04:00, `RangeError`; −04:00, −04:00, −05:00, `RangeError`; 23 | 02:30−05:00 for all four (no gap, `reject` accepts); −05:00 for all four; 24 | **policy difference under SJL-D-20 (extrapolation)**. `runs/a2-disambiguation.compare.txt` |
| A6 | start of day where midnight is skipped: Santiago 2026-09-06, Havana 2026-03-08, Beirut 1920-03-28 and 2026-03-29, Toronto 1919-03-31 (23:30 to 00:30), São Paulo 2018-11-04, Asunción 2024-10-06, Apia 2011-12-30 (a skipped day), through `PlainDate.toZonedDateTime`, `startOfDay` and `hoursInDay`; `ZonedDateTime.from("2026-09-06[America/Santiago]")` *(run)* | 01:00−03:00, 01:00−04:00, 01:00+03:00, 01:00+03:00, 00:30−04:00, 01:00−02:00, 01:00−03:00, 2011-12-31T00:00+14:00; `hoursInDay` 23 or 23.5, Apia 24; 01:00−03:00 | the same | **agreement**. Neither engine's last resort is reached: Broiler.VM's `RangeError` (`VM/JsTemporal.Zones.cs:290-294`) and Broiler.JS's local midnight (`ZD:1514-1518`) both need a gap that no transition follows, which neither dataset has *(reading)*. `runs/a3-startofday.compare.txt` |
| A7 | start of day after 2037: Santiago 2040-09-02, Havana 2040-03-11, Beirut 2040-03-25 *(run)* | 01:00−03:00, 01:00−04:00, 01:00+03:00; `hoursInDay` 23 | 00:00−03:00, 00:00−05:00, 00:00+02:00; 24 | **policy difference under SJL-D-20 (extrapolation)**: midnight exists in Broiler.JS because the offset froze, not by its fallback. The milestone's "start-of-day fallback" correction is to code no input reaches (A6). `runs/a3-startofday.compare.txt` |
| A8 | the instant range: `startOfDay` at −271821-04-20 in UTC, and at the lower limit in Etc/GMT+1 and Etc/GMT-1; `getTimeZoneTransition` at the lower and upper limits *(run)* | −271821-04-20T00:00+00:00; `RangeError`, `RangeError`; `null`, `null` | the same | **agreement**. `runs/a3-startofday.compare.txt`, `runs/a4-transitions.compare.txt` |
| A9 | next and previous transitions: New York from 2026-07-01, from exactly 2026-03-08T07:00Z, and half a second after it; the first (from 1800) and none before it; Lord Howe from 2026-01-01; London from 1968-07-01 (next) and 1971-07-01 (previous), across 1968 to 1971's change of the daylight flag alone; Riyadh from 2000; `Etc/GMT+5`, `+05:30`, `UTC`; the options-object form *(run)* | 2026-11-01T01:00−05:00 and 2026-03-08T03:00−04:00; 2026-11-01 and 2025-11-02; 2026-03-08T03:00−04:00; 1883-11-18T12:00−05:00, `null`; 2026-04-05T01:30+10:30; 1971-10-31T02:00+00:00 and 1968-02-18T03:00+01:00; `null` and 1947-03-13T23:53:08+03:00; `null` ×3 | the same | **agreement**: both skip a change of the daylight flag alone. `runs/a4-transitions.compare.txt` |
| A10 | transitions after 2037: New York next from 2037-12-01, previous from 2040-07-01, 2100-01-01 and +275760-09-01; Santiago next from 2037-12-01; Berlin next from 2040-07-01 *(run)* | 2038-03-14T03:00−04:00; 2040-03-11T03:00−04:00, 2099-11-01T01:00−05:00, +275760-03-09T03:00−04:00; 2038-04-03T23:00−04:00; 2040-10-28T02:00+01:00 | `null`; 2037-11-01T01:00−05:00 ×3; `null`; `null` | **policy difference under SJL-D-20 (extrapolation)**. `runs/a4-transitions.compare.txt` |
| A11 | unknown names: `Mars/Olympus`, `Etc/Unknown`, `""`, `America/New York`, `Europe/Berlin `, `EST5`, `Etc/GMT+13`, `Etc/GMT-15`, `Etc/GMT+05`, each through the constructor, a `from` annotation, `toZonedDateTimeISO`, `Instant.toString({timeZone})`, `PlainDate.toZonedDateTime`, `Temporal.Now.zonedDateTimeISO`, a `Duration.round` `relativeTo` bag and `Intl.DateTimeFormat` *(run)* | `RangeError` on all 72 | `RangeError` on all 72 | **agreement**. Broiler.JS's zero offset (`ZD:1349-1350`, :1377, :1415-1416) and its formatter's host-zone fallback for an unknown name (`BI/Intl/JSIntlDateTimeFormatEngine.cs:711-713`) are not reached: every entry point found validates first *(reading)*. The milestone's correction "unknown zones silently reading as offset zero" deletes unreached code; the one unvalidated name found is the host's (A16), and it throws rather than reading zero. `runs/a5-unknown.compare.txt` |
| A12 | offset zones: `+05:30`, `-05:30`, `+0530`, `+05`, `-00:00`, `+00:00`, `-00`, `+23:59`, `-23:59` in the constructor and the formatter; `Instant.toString({timeZone: "-00:00"})`; `+00:00` equals `UTC`; `+05:30` equals `+0530`; 2020-01-01T00:00 in `-12:00`; refused forms `+24:00`, `+05:60`, `+05:30:15`, `+05:30:00`, `+053015`, `+05:30:15.5`, `−05:00` (U+2212), `+05:3`, `05:30`, `+5:30`, a `[+05:30:15]` annotation *(run)* | +05:30, −05:30, +05:30, +05:00, +00:00 ×3, +23:59, −23:59; `2020-01-01T00:00:00+00:00`; `false`; `true`; 2020-01-01T12:00Z; `RangeError` for every refused form | the same | **agreement** (messages differ). `runs/a6-offsetzones.compare.txt` |
| A13 | `+٠٥:٣٠` (Arabic-Indic digits, U+0660 to U+0669) as the constructor's zone and as the formatter's `timeZone` *(run)* | `RangeError`, `RangeError` | an `Error` whose message is .NET's `System.FormatException: The input string '٠٥' was not in a correct format.` with its stack trace, both times *(host)* | **correction of Broiler.JS**: the offset grammar's digits are `DecimalDigit :: one of 0 1 2 3 4 5 6 7 8 9`, so the string is neither an offset nor an available name, a `RangeError`. .NET's `\d` matches every decimal digit (`ZD:1637-1638`, `BI/Intl/JSIntl.cs:2867-2868`) and `int.Parse` then throws (`ZD:1645`, `BI/Intl/JSIntl.cs:2879`). Not in the area's table. `runs/a6-offsetzones.bjs.txt` |
| A14 | `Etc/GMT+5`, `etc/gmt+5`, `ETC/GMT+5`, `Etc/GMT-14`, `Etc/GMT+12` in the constructor and the formatter; the hour at the epoch in `Etc/GMT+5` and `Etc/GMT-14`; `Etc/GMT+5` equals `-05:00`; its next transition; its offset in 1850; whether it is listed *(run)* | `Etc/GMT+5` −05:00 ×3, `Etc/GMT-14` +14:00, `Etc/GMT+12` −12:00, the formatter the same names; 19, 14; `false`; `null`; −05:00; `true` | the same | **agreement**: Broiler.VM's `EtcGmtZones` (`VM/JsRealm.DateTimeFormat.cs:1207-1214, 1222-1235`) and Broiler.JS's TZif data give the same fixed offsets. `Etc/GMT0`, `Etc/GMT-0`, `GMT+0`, `GMT-0`, `GMT0`, `Etc/GMT` resolve to `UTC` in Broiler.VM's formatter and keep their names in Broiler.JS's (Z8). `runs/a7-etcgmt.compare.txt` |
| A15 | the host zone, `TZ=America/New_York`: `Temporal.Now.timeZoneId()`, the default formatter's `timeZone`, `new Date(2040, 6, 1, 12).getTimezoneOffset()`, Temporal's New York offset on 2040-07-01, the default formatter's hour at 2040-07-01T16:00Z; `TZ=America/Vancouver`: `Date` against Temporal on 2027-01-15 *(run)* | `UTC`, `UTC`, 0, −04:00, 16; `UTC`, 0 (no host zone: JSD-0053 section 7) | `America/New_York`, `America/New_York`, 240, **−05:00**, 12; Vancouver: `Date` 420 (−07:00, the host's 2026c), Temporal **−08:00** (2025b) *(host)* | **implementation-defined**: the local zone is each engine's policy (SJL-D-19), which SJL-8 does not change. Inside Broiler.JS, `Date` and the default formatter read the host's data and footer while `Temporal` reads `tzdata.bin`, so one zone has two offsets; SJL-8.6 lets the provider read the shared data. Not in the area's table. `runs/a8-hostzone.compare.txt`, `runs/a9-date-vs-temporal.compare.txt` |
| A16 | host zones outside the bundled names or primaries: `TZ=posixrules`, `TZ=Europe/Bratislava`, `TZ=Factory`, `TZ=Asia/Calcutta` *(run, Broiler.JS only)* | `UTC` whatever `TZ` is *(run with `TZ=America/New_York`)* | `posixrules` from `Temporal.Now.timeZoneId()`, while `Intl.DateTimeFormat().resolvedOptions()` and `Temporal.Now.zonedDateTimeISO()` throw `RangeError`; `Europe/Prague`; `Factory`; `UTC` (this host's tz data has no backward names, so .NET falls back) *(host)* | **correction of Broiler.JS** for `posixrules`: SystemTimeZoneIdentifier returns an offset string or "a primary time zone identifier", and `SystemTimeZoneId` returns an unknown host name unchanged (`BI/Temporal/JSTemporalNow.cs:21-37`, `TZ/IanaTimeZoneDatabase.cs:50-51`). `Europe/Prague` and `Factory` are Z7's and Z5's. `runs/a8-hostzone.bjs.TZ=*.txt` |

## 3. The calendar tables

**Paths in this section**, beside the header's: `IX/` is `icu_calendar-2.3.0/src/cal/` and `CC/` is
`calendrical_calculations-0.2.4/src/`, both inside the crates `src/tests/calendars/pins/` archives, whose
members hash as `calendars.pin` records. `BU/` is Broiler.Unicode's `src/` at `2affa78`, the commit the
CLDR package `0.1.0-preview.3` was built from (its nuspec); the two calendar files there are unchanged at
its head. `runs/` is `correspondence-runs/cal/`. "Table 1" is the Intl era and month code proposal's
(<https://tc39.es/proposal-intl-era-monthcode/>, its Stage 4 draft of 19 March 2026, read on 2026-10-17). It
bases `chinese` on the Purple Mountain Observatory's data for 1900 to 2100, `dangi` on KASI's for 1900
to 2050, `islamic-umalqura` on KACST's months for 1300 to 1600 AH and `persian` on the Iranian calendar
authority's leap years for 1206 to 1498 AP. Each calendar falls back outside its range: Umm al-Qura to
`islamic-civil`, the other three to "an implementation-defined approximation".

**Broiler.VM** reads its years from one generated table in the Intl assembly. Only roots that hand in
Intl data have Temporal's calendars (`VM/JsIntlTables.cs:108`, `VI/JsCldrData.cs:101`).
- `AT/CalendarTableGenerator.cs:65-94` reads four ICU4X files and one array of `calendrical_calculations`
  as data (:37-41, :268-278). It writes `VI/JsCalendarTables.g.cs`, 1,958 bytes:
  - the Qing years 1900 to 1911 (`IX/east_asian_traditional/qing_data.rs:5-6, 10`);
  - China's and Korea's years 1912 to 2102 (`china_data.rs:5, 9`, `korea_data.rs:5, 9`). These are
    "Data obtained from `calendrical_calculations`", that is computed years, two of them past 2100
    (:207);
  - the Umm al-Qura years 1300 to 1600 AH (`IX/hijri/ummalqura_data.rs:5-6, 10`);
  - the 78 Persian years from 1502 to 2987 AP that the 33-year rule makes leap and are not
    (`CC/persian.rs:17-29`).
- Rule N31 holds the archives to the pin (`AT/CalendarPin.cs:61, 130, 204-208`,
  `AT/CalendarRuleTests.cs:23, 53, 80`). Rule N32 regenerates the table byte for byte (:96, :118, :137).
  Decoded independently, the table equals the five members year for year (`runs/vm-table-decode.txt`).
- `JsCalendarData` decodes a year per call (`VM/JsCalendars.cs:1093-1140`), and nothing is cached
  (:117-118).
- Outside the table:
  - `chinese` and `dangi` follow ICU4X's mean-motion approximation, ported and not generated
    (:916-1003). It is reckoned at UTC+8 or UTC+9 after 1912 and at Beijing's local mean time before
    (:858-866), as `IX/east_asian_traditional.rs:290-306, 433-451` does;
  - Umm al-Qura falls back to the civil tabular year (`VM/JsCalendars.cs:629-649`), as
    `IX/hijri.rs:279-292` does;
  - Persian applies the 33-year rule with no correction before 1502 or after 2987 (:550-557).
- A year more than 300,000 from the epoch is refused with `RangeError` before it is computed
  (`VM/JsTemporal.CalendarNonIso.cs:47-50, 64-72`).

**Broiler.JS** dispatches each calendar to a .NET class or to the CLDR package
(`BI/Temporal/TemporalCalendarMath.cs:10-57`).
- `chinese` and `dangi` (`BI/Temporal/TemporalLunisolarCalendar.cs:25-202`):
  - In range, the runtime's `ChineseLunisolarCalendar` and `KoreanLunisolarCalendar` (:29-30)
    *(host)*. Those are 1901-02-19 to 2101-01-28 and 0918-02-19 to 2051-02-10, read from the classes
    (:41-52). Both are `s_yinfo` tables in `System.Private.CoreLib` 10.0.12, of 200 and 1,133 years
    (`runs/netprobe-reflect.txt`).
  - Outside, `CldrChineseCalendar` (:63-64, :67-70, :90-147). That is Calendrical Calculations'
    astronomy in doubles, at Beijing's meridian (`BU/UnicodeCldr.LocaleData/CldrChineseCalendar.cs:3-18,
    147-155`). It caches every Gregorian year it builds in an unbounded `Dictionary`, under one global
    lock (:30-42).
  - For `dangi` the fallback is the Chinese computation, "as an approximation"
    (`BI/Temporal/TemporalLunisolarCalendar.cs:13-18, 59-64`).
  - The year path and the day path pick a source by year and by epoch day respectively (:63-64). At
    the classes' bounds they meet without a seam (`runs/seams.*`).
  - Both files say that the computation and .NET's table agree across 1901 to 2100 (:13-16;
    `BU/.../CldrChineseCalendar.cs:11-13`). **They do not.** The computation and ICU4X differ in 1906
    and 2033, where the computation places 2033's leap month eighth rather than twelfth, and .NET
    differs from both in 2057 (`runs/eastasian-sources-compare.txt`). No Broiler.JS answer reaches the
    computation inside that span.
- `persian` (`BI/Temporal/TemporalSolarHijriCalendar.cs:25-125`):
  - the runtime's astronomical `PersianCalendar` for 0622-03-22 to 9999-12-31, years 1 to 9378
    *(host)*;
  - the plain 33-year rule outside (:36-67).
  - The day path switches by epoch day (:71-77). The year path switches when .NET throws (:79-85,
    :91-95).
- `islamic-umalqura` (`BI/Temporal/TemporalArithmeticCalendar.cs:34-37, 62-108`) reads `CldrUmmAlQura`
  for 1300 to 1600 AH, from the civil epoch R.D. 227015 (:55, :103-108). Outside, it reads the civil
  tabular arithmetic. The table says it is ICU4C's (`BU/UnicodeCldr.LocaleData/CldrUmmAlQura.cs:3-15,
  57-111`).
- Nothing is pinned or generated. The package's version is fixed (`Directory.Packages.props:17`). The
  runtime is not: there is no `global.json`, and `BroilerJS.runtimeconfig.json` names framework 10.0.0,
  so the installed 10.0.x patch decides.

Every Broiler.JS cell below was also run under invariant globalization, under `fa_IR` with
`Asia/Tehran` and under `ko_KR` with `Asia/Seoul`, and answered byte for byte the same (K14).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| K1 | The structure of every year: new year, `monthsInYear`, `inLeapYear`, leap month code and ordinal, each month's `daysInMonth`. `chinese` 1900 to 2102 and `dangi` 1900 to 2050 (`runs/east-asian-years.*`). The retained dataset's conversions in those years, and test262's `monthCode` and `inLeapYear` fixtures for 1947 to 2047 *(run)* | ICU4X's tables, for example `chinese 2033` = 2033-01-31, 13 months, `M11L@12`; 1987-08-17 is `M06L` 23 | the same, except K2 *(run)*. From .NET's tables *(host)*, and from the CLDR computation for `chinese` 1900, 2101 and 2102 | **agreement, from different data**. On the five Chinese dates where ICU4C 77.1 disagrees (`src/tests/temporal/calendars/divergences.txt` group 2), both answer as ICU4X. Both pass the four fixtures (`runs/test262-table-calendars.*`) |
| K2 | `chinese` 2057; ISO 2057-09-28; `from({year: 2057, monthCode: 'M09', day: 1, calendar: 'chinese'})` *(run)* | months `sllslslslsls`; 2057/`M09`/1; 2057-09-28 | months `sllslsllssls`; 2057/`M08`/30; 2057-09-29 *(host: .NET's table)* | **correction of Broiler.JS**: Table 1 bases `chinese` 1900–2100 on the observatory's published data. The Hong Kong Observatory's 2057 table begins the ninth month on 28 September (<https://www.hko.gov.hk/en/gts/time/calendar/text/files/T2057e.txt>, read 2026-10-17, the table ICU4X's own test reads), as ICU4X does. It is the only year of 1901–2100 in which .NET's table and ICU4X's differ (`runs/eastasian-sources-compare.txt`). The Purple Mountain Observatory's own table was not consulted |
| K3 | Leap months from fields with `overflow: 'reject'`: `chinese` 1900 `M08L`, 1987 `M06L`, 2012 `M04L`, 2017 `M06L`, 2020 `M04L`, 2023 `M02L`, 2025 `M06L`, 2028 `M05L`, 2033 `M11L`, 2052 `M08L`, 2099 `M02L`, 2101 `M07L`; `dangi` 2012 `M03L`, 2023 `M02L`, 2033 `M11L`, 2052 `M08L`, 2101 `M07L`; `chinese` 2012 `M03L` and `dangi` 2012 `M04L` *(run)* | 1900-09-24, 1987-07-26, 2012-05-21, …, 2033-12-22, 2101-08-25; `RangeError` for the last two (`runs/east-asian-edges.vm.txt`) | the same *(run)* | **agreement** |
| K4 | `dangi` 2051 to 2102, every year *(run)* | ICU4X's Korean years, computed at UTC+9 | the CLDR **Chinese** computation, at Beijing's meridian. It differs in 25 of the 52 years: 2061 begins 2061-01-21, not 01-22; 2099's leap month is `M02L`, not `M03L`; ISO 2057-09-28 is `M09`/1, not `M08`/30 | **policy difference under SJL-D-21 (ICU4X sources; no .NET calendar class)**. Table 1 leaves `dangi` after 2050 to an implementation-defined approximation |
| K5 | `dangi` 918 to 1899: 918, 919, 1000, 1500, 1582, 1600, 1700, 1800, 1850 and 1880 to 1899 *(run)* | ICU4X's approximation, for example 918 = 354 days | .NET's `KoreanLunisolarCalendar` table *(host)*, for example 918 = 355 days. It differs in 28 of the 29 years | **policy difference under SJL-D-21**; implementation-defined under Table 1 |
| K6 | `chinese` and `dangi` outside K1, K4 and K5: 1880 to 1899 and 2103 to 2210 every year, and 23 years from −271820 to 275759 (`runs/east-asian-years.summary.txt`) *(run)* | ICU4X's approximation, for example `chinese 2104` = 2104-01-27, leap `M06L` | the CLDR computation, for example 2104-01-28, leap `M05L`. It differs in every probed year but 1800. Of the retained dataset's 64 lines in K5's and K6's years (its divergences, group 1), it answers 52 as ICU4C 77.1, 12 as neither and none as ICU4X | **policy difference under SJL-D-21**; implementation-defined under Table 1 |
| K7 | `chinese` and `dangi` far out: ISO −271821-04-19, ±050000-01-01 to ±250000-01-01, +275000-01-01 and +275760-09-13, fields read *(run)* | dates of 29- and 30-day months, for example −271821-04-19 = −271821/`M03`/17 | `Error` wrapping a .NET `IndexOutOfRangeException` from `ChineseYear.DaysInMonth` (`BU/.../CldrChineseCalendar.cs:57`) at −271821, ±50000, −150000, −250000 and +100000 to +250000. Impossible months elsewhere: +275000 is 42, 41, 31, 24, 22… days; −200000 is 28…33; +275760-09-13 falls in a 31-day month of a 356-day year (`runs/east-asian-edges.js.txt`). Uncaught, the exception ends the shell's process (`runs/east-asian-far-error.*`) | **correction of Broiler.JS**: Temporal's CalendarISOToDate "returns a Calendar Date Record" for every ISO date within the limits (<https://tc39.es/proposal-temporal/>, its Stage 4 draft of 27 July 2026, read on 2026-10-17). The proposal's 4.1.14 makes that "a valid calendar date" in a "Lunisolar calendar". test262 creates these dates without reading them ("don't care about the conversion", `test/intl402/Temporal/PlainDate/prototype/withCalendar/extreme-dates.js:55-58, 70-73`), so the suite passes |
| K8 | The Umm al-Qura data: 1 Muharram and the twelve month lengths of each of 1300 to 1600 AH, compared as data *(run)* | `ummalqura_data.rs`, generated | `CldrUmmAlQura` at R.D. 227015 + `YearStartDay` | **agreement, and the same data**: all 301 years equal (`runs/umalqura-table-compare.txt`). The package's month-length words equal ICU4C's as ICU4X's own test states them (`IX/hijri/ummalqura_data.rs:328-357`) |
| K9 | `islamic-umalqura` 1290 to 1610 AH every year; −280000, −100, 0, 1, 500, 1000, 2000, 5000 and 280000; ISO 1882-11-11 to 13 and 2174-11-24 to 26 and back; the 1,704 days around 1300 and 1600 AH; test262's `era-boundary-islamic-umalqura.js` *(run)* | 1300 = 1882-11-12 `lslslslslsls`… 1600 = 2173-12-07; 1601 = 2174-11-26, as `islamic-civil` 1601 | the same; 0 lines differ (`runs/umalqura-years.diff`, `runs/seams.*`) | **agreement**: inside the table from equal data (K8), and outside from the same civil arithmetic, as Table 1 asks |
| K10 | `persian` 1206 to 1498 AP, Table 1's range: every year 1170 to 1520, and test262's `persian-new-year-dates.js` *(run)* | the 33-year rule, with no correction before 1502 | .NET's `PersianCalendar` *(host)* | **agreement**: the 351 years differ only at 1176 and 1177 (K11). Both pass the fixture |
| K11 | `persian` years within .NET's range but outside Table 1's: `inLeapYear` for every year 1 to 9400 (`runs/persian-leap-map.*`), and 1535–1536 in detail *(run)* | for example 1535 is leap: Esfand 30 = 2157-03-20, and 1536 begins 2157-03-21 | 1535 is common, and 1536 begins 2157-03-20 *(host)*. 2,582 of the years 1 to 9378 differ: 139 before 1178, 12 in 1178–3000 (1535–36, 2026–27, 2092–93, 2583–84, 2616–17, 2715–16) and 2,431 after 3000 | **policy difference under SJL-D-21**; implementation-defined under Table 1. `CC/persian.rs:17-22` matches its corrections to a 52.5° E astronomy for 1178–3000 AP, and .NET's astronomy differs from them in those 12 years |
| K12 | `persian` at .NET's bounds: every day of 0622-01-01 to 0623-06-30 and of 9999-01-01 to +010001-06-30, to the calendar and back *(run)* | one date per day, all round-tripping: 0622-03-21 is 1/`M01`/1, a leap year of 366 days | 0622-03-21 and 0622-03-22 are both 1/`M01`/1, and 0622-03-21 returns as 0622-03-22. +010000-01-01 to 03 repeat 9378/`M10`/11 to 13 of 9999-12-29 to 31 and return as those days (`runs/seams.js.txt`) | **correction of Broiler.JS**: CalendarISOToDate gives each ISO date one calendar date, and "No known calendars have repeated dates" (proposal 4.1.13's NOTE). The arithmetic fallback starts year 1 a day before .NET does and ends 9378 three days later, and the two are joined by a range check (`BI/Temporal/TemporalSolarHijriCalendar.cs:71-85`) |
| K13 | `persian` beyond .NET's range: −272440, −10, 0, 9379, 9380, 10000, 100000 and 275130 AP; test262's `extreme-dates.js` (min −272442/`M01`/9, max 275139/`M07`/12) *(run)* | the 33-year rule | the 33-year rule from the same epoch (R.D. 226896 = JD 1948320) | **agreement** |
| K14 | Every Broiler.JS probe of K1 to K13 under `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`, `LANG=fa_IR.UTF-8 TZ=Asia/Tehran` and `LANG=ko_KR.UTF-8 TZ=Asia/Seoul` *(run)* | no host input: the tables are compiled into the Intl assembly *(reading)* | identical output in all three (`runs/host-variance/`). The answers are those of the runtime's classes in `System.Private.CoreLib` 10.0.12 *(host)*; another runtime patch was not run | **policy difference under SJL-D-21**: no host or culture dependence was found, but the answers are the runtime's, "which vary with the runtime" (SJL-D-21) |
| K15 | How the years are held *(reading)* | 1,958 constant bytes, decoded per call | .NET's statics, and an unbounded per-year cache under a global lock (`BU/.../CldrChineseCalendar.cs:30-42`) | no observable difference; the shared tables are constant data with no cache |
| K16 | The formula calendars, outside this sub-area: `gregory`, `buddhist`, `roc`, `japanese`, `coptic`, `ethiopic`, `ethioaa`, `indian`, `islamic-civil`, `islamic-tbla` and `hebrew`. The retained dataset's fields of 137 ISO dates from −500 to +10000 in each (`runs/retained-calendars.*`); test262's `extreme-dates.js` and `roundtrip-from-iso*.js` *(run)* | formulas (`VM/JsCalendars.cs:282-777`) | formulas (`BI/Temporal/TemporalArithmeticCalendar.cs`, `BI/Temporal/JSTemporalCalendar.cs:177-`) | **agreement** on all 1,507 lines, Hebrew 5806's 384 days included. The arithmetic is SJL-10's |
| K17 | The retained dataset's other 1,944 lines (arithmetic, differences, rounding, `with`, year-months, month-days), in all fifteen calendars *(run)* | the retained answers, all 3,999 lines (`runs/retained-calendars.vm.txt`) | 103 lines differ (`runs/retained-calendars.other-differing.txt`), for example `toPlainYearMonth` keeping the day, `duration round relativeTo` throwing `Error`, and `M13` accepted in `persian` and `islamic-umalqura` | **SJL-10's** |

## 4. The design's readings

What follows reads the area design, [`areas/time-zones-and-calendars.md`](../../javascript-shared-libraries/areas/time-zones-and-calendars.md)
section 2, against the rows above. JSD-0076 records which readings change the design.

**Time zones.**

- **Release, pinned, built by:** confirmed, 2026e against 2025b. "Checked against `zic` output across its validated
  range" was one run of a prototype (JSD-0053 section 2); no test repeats it. This draft's run of the host's `zic`
  repeats it for 1800 to 2037 (Facts). Broiler.JS's blob is not reproducible: shown by a run (Z15).
- **Backzone, "mostly included":** confirmed and counted: 102 of backzone's 125 Zones, the missing 23 being names
  zone.tab does not list; 110 names differ before 1970 (Z3).
- **The 2037 freeze:** confirmed for New York and Berlin. It also freezes southern zones on daylight time (Z4),
  and it splits Broiler.JS's `Date` from its `Temporal` for one zone (A15).
- **Identifiers, primaries, `DateTimeFormat`'s `timeZone`:** confirmed. Only `Factory` separates the sets; twelve
  names separate the primaries (445 against 435 listed); 152 non-primary names resolve differently in the formatter.
- **Lookup:** confirmed, and observable: invariant upper-casing admits `America/Loſ_Angeles` (Z10).
- **Algorithms:** the two- and three-sample methods do differ, without effect on either dataset (A2). "An unknown
  named zone silently reads offset zero" and "`StartOfDay` falls back to local midnight" are true of the code and
  reached by no input found (A6, A11). "Next transition is `null` after 2037": confirmed (A10). Broiler.VM's
  `RangeError` for a missing start of day is likewise unreached.
- **Format, optionality:** confirmed. A zone's TZif file is parsed lazily; only the inflate is whole.
- **Missing from the table:** the region lists (Z12); ISO strings accepted as the formatter's `timeZone` (Z11);
  non-ASCII digits in an offset surfacing a .NET `FormatException` (A13); a host zone name returned unvalidated
  (A16); `Date` and `Temporal` on two datasets inside Broiler.JS (A15); and IANA's `factory` file being part of the
  default `zic` build, which Broiler.VM's pin README and compiler comment describe as nine files (Z5).

**Calendars.**

- **"Gregorian … Hebrew | Formulas | The same formulas (reading)".** Confirmed, and now by run. The two
  engines agree on all 1,507 conversion lines of the retained dataset in these eleven calendars (K16),
  and both pass test262's `extreme-dates.js` and round-trip fixtures. Their arithmetic differs in 103
  lines across all fifteen calendars (K17), which is SJL-10's.
- **"Umm al-Qura | ICU4X tables | The CLDR package's table, identical over its range".** Confirmed by
  comparing the data. All 301 years, 1300–1600 AH, have the same 1 Muharram and month lengths (K8), and
  both engines fall back to `islamic-civil` and agree everywhere probed (K9).
  - The doc omits that the CLDR package's table is ICU4C's encoding: twelve bits a year plus a
    least-squares start estimate and a fix-up (`BU/UnicodeCldr.LocaleData/CldrUmmAlQura.cs:44-111`).
  - It also omits that Broiler.JS places the table at R.D. 227015
    (`BI/Temporal/TemporalArithmeticCalendar.cs:55`).
- **"Chinese, Korean | ICU4X's published years, with ICU4X's approximation outside them".** Confirmed
  as to code, with three refinements.
  - The table's years are 1900–2102: Qing 1900–1911, then China's or Korea's. Table 1's published
    ranges are 1900–2100 for `chinese` and 1900–2050 for `dangi`.
  - ICU4X's 1912–2102 files say "Data obtained from `calendrical_calculations`", that is computed
    years. ICU4X states agreement with PMO (1900–2025), HKO (1901–2100) and KASI (1900–2050).
  - So Broiler.VM's `dangi` 2051–2102 are ICU4X's computed Korean years, not KASI's published ones.
- **"Chinese, Korean | .NET's lunisolar calendars in their range; outside it, the CLDR package's
  astronomical computation with an unbounded cache".** Confirmed (cache at
  `BU/.../CldrChineseCalendar.cs:30-42`). The doc misses five things:
  - the ranges: `chinese` 1901-02-19..2101-01-28, `dangi` 0918-02-19..2051-02-10;
  - that `dangi`'s fallback is the **Chinese** computation, at Beijing's meridian (K4);
  - that `chinese` 2057 differs inside the published range (K2);
  - that `dangi` 918–1899 comes from .NET's Korean table (K5);
  - that the computation fails far out, throwing or producing impossible months (K7).
- **"Persian | A 33-year rule with corrections from `calendrical_calculations` 0.2.4 | .NET's
  `PersianCalendar`; equivalence not established".** Now established. The engines agree on every year
  in Table 1's 1206–1498 AP and in 1178–1534 (K10).
  - They differ in 2,582 of the years 1–9378 (K11), 12 of them in 1178–3000.
  - Outside .NET's 1–9378, Broiler.JS uses the plain 33-year rule, so the engines agree there (K13).
  - Broiler.JS has two seams at .NET's bounds (K12).
  - The corrections apply only from 1502 to 2987 AP (`VM/JsCalendars.cs:550-557`, `CC/persian.rs:23-29`).
- **"Pinned | `calendars.pin` …; rules N31 and N32 | Not pinned".** Confirmed. The pin holds two
  archives and nine members, all hashing as recorded. Broiler.JS fixes the CLDR package's version
  (`Directory.Packages.props:17`) but no data. Its .NET data follows the installed runtime patch (no
  `global.json`; runtimeconfig framework 10.0.0).
- **Not in the table.** Table 1 of the proposal is the arbiter of which differences are corrections.
  Inside its ranges only K2 differs; everything else is implementation-defined (K4–K6, K11) or a
  Broiler.JS defect (K7, K12). Area section 1's row "Chinese, Korean and Qing year tables; Umm al-Qura;
  Persian corrections" matches the generated table's five sections (`runs/vm-table-decode.txt`).
