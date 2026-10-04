<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0045 - Intl.DateTimeFormat: ICU's pattern generator and interval formats, UTC and fixed offsets

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is slice I3 of [JSD-0027](0027-intl-scope-and-data-strategy.md) section 7,
`Intl.DateTimeFormat`, built on the data boundary of
[JSD-0043](0043-intl-data-boundary-and-collation.md) and the number data of
[JSD-0044](0044-intl-numberformat.md).

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the third implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built, and where it is wider than I3 was drawn

- **`Intl.DateTimeFormat`, whole, for the Gregorian calendar.** It has every component option in each
  width ECMA-402 admits (`weekday`, `era`, `year`, `month`, `day`, `dayPeriod`, `hour`, `minute`,
  `second`, `fractionalSecondDigits`, and `timeZoneName` in all six styles). It has `dateStyle` and
  `timeStyle` in every combination, the four hour cycles by `hourCycle`, `hour12` or `-u-hc`, and
  `-u-nu` and `numberingSystem`. Its members are `format`, `formatToParts`, `formatRange`,
  `formatRangeToParts`, `resolvedOptions` and `supportedLocalesOf`. It takes ECMA-402's normative
  optional legacy constructor path, as `Intl.NumberFormat` does.
- **The three `Date.prototype.toLocale*` methods** route through it, with ECMA-402 s20.4's required
  and default components. Without `Intl` they answer as before.
- **`Intl.supportedValuesOf`** answers all six keys (s8.3.2). JSD-0044 deferred it to this slice so
  that its `calendar` and `timeZone` lists describe a `DateTimeFormat` that exists. They are
  `gregory`; `phonebk`; the currencies the data names; the numeric numbering systems; `UTC` and the
  `Etc/GMT` zones of section 5; and the sanctioned units.
- **JSD-0027 drew I3 as "the component options the section 5 locales need"**. Every component is
  built instead. ECMA-402 admits each on every format, and refusing some would be a constructor the
  standard does not describe. Ranges were not named either, and they are built: `formatRange` is in
  every edition JSD-0027 could mean.

## 2. The data

- **The archive grows under rule N27.** From `cldr-dates-full` at 48.2.0 it gains
  `ca-gregorian.json`, `dateFields.json` and `timeZoneNames.json` for `de` and `en`, and from
  `cldr-core` it gains `timeData.json` and `dayPeriods.json`. Each was taken as JSD-0043's were: an
  npm tarball checked against the registry's integrity, retrieved twice and compared byte for byte.
- **Three tables join the generated file under rule N28**:
  - each language's calendar data, flattened to dotted keys: month, day, day period and era names;
    the date, time and date-time patterns; the available formats, append items and interval
    formats; the date fields' display names; and the GMT formats and the names of UTC and of the
    GMT zone;
  - the hour cycles of the regions the supported locales are likely to name, and of the world;
  - each language's day period rules.

  Alternate forms (`-alt-`) and plural-dependent formats (`-count-`) are not carried.
- **The size**: 465,869 bytes of table data on 2026-10-04 (27,038 more), and an assembly of 475,648
  bytes. That is still under JSD-0043's provisional 512 KiB bound, and no budget is set.
- **The resolved JSON serves where ICU reads a locale and its root.** cldr-json's locale files
  include what they inherit, so the generator reads one file per language. ICU 77.1 treats every
  available format the same whether it is the locale's own or root's. A German weekday on its own is
  `Di`, the stand-alone form root's `ccc` gives, and a German hour and minute are `00:00`, from
  root's `Hm`.

## 3. The pattern of a format

- **The best-fit matcher is ICU's DateTimePatternGenerator**, which ECMA-402 leaves to the
  implementation (s11.5.3).
  - The generator holds a language's patterns as ICU does. It adds the sixteen canonical items, then
    the language's time and date styles, then its available formats in key order, under ICU's rules
    for a base or skeleton already held.
  - A skeleton is matched by ICU's distances between field types. A missing field costs more than a
    different width, and an extra field costs more again. Of two equal matches, ICU's tie-break
    keeps the one whose missing fields are the higher bits.
  - Fields no pattern holds are added with CLDR's append items. A date half and a time half are
    joined by the `atTime` date-time pattern for the width the month asks for.
  - Widths are adjusted as ICU adjusts them. A numeric minute and second keep the pattern's width,
    so they are always `2-digit` in `resolvedOptions`, as in every ICU engine.
  - The hour keeps the skeleton's width unless the skeleton named the same width the found pattern's
    skeleton names. This is why German `{hour: 'numeric'}` is `HH 'Uhr'`.
- **The skeleton is built as V8 builds it**: one run of letters per component, with the hour in the
  resolved cycle's letter, and the result's hour fields rewritten in that letter.
  - A twenty-four-hour cycle drops a day period, as ICU's matcher does. German `{hour, dayPeriod}`
    is `14 Uhr`, and `resolvedOptions` has no `dayPeriod`.
- **The styles are CLDR's date and time formats**, joined by the date style's `atTime` pattern, as
  ICU's DateFormat joins them. Where the resolved hour cycle is not the time style's, the pattern is
  made again from its skeleton in the resolved cycle.
- **`resolvedOptions` reports the fields the pattern has** (s11.3.2): each component's width read
  from its letters, and `hourCycle` and `hour12` only where there is an hour.

## 4. The fields and the ranges

- **Every field is written from CLDR's data**: names in the context the letter asks for, numbers in
  the format's numbering system, and fractional seconds truncated to their digits.
  - The calendar is proleptic Gregorian over the whole time-value range. The year 0 is 1 BC, and
    the first time value is 20 April 271822 BC.
  - A flexible day period follows the language's rules. It is noon at noon exactly where the
    language names noon, and midnight is written as the period it falls in, as ICU 57 and later
    write it.
- **A range is ICU's DateIntervalFormat**, whose patterns ECMA-402 leaves to the implementation.
  - The format's skeleton is split into date and time halves, normalized as ICU normalizes them, and
    matched against CLDR's interval skeletons.
  - The largest field the two ends differ in chooses the interval pattern. Without one, both ends
    are written in full, joined by CLDR's fallback; ends on one day write the date once and the
    times as a range.
  - A pattern serving a skeleton without a year is found under the skeleton with one.
- **A range's sources are ICU's spans**: the first and second occurrences of the fields the range
  repeats. Ends whose formatted fields are all equal are one date, every part `shared`, as
  PartitionDateTimeRangePattern's practical equality makes them.

## 5. Time zones

The profile carries no time-zone database (JSD-0027 section 5 item 5, decision (d)). The time zones
it admits are:

- **`UTC`, with every name IANA links to it** (`Etc/UTC`, `Etc/GMT`, `GMT`, `Etc/Universal`,
  `Etc/Zulu` and the rest), each resolving to the primary identifier `UTC`, as s6.5.1 states for
  the first three. The default time zone is `UTC`, the profile's own.
- **An offset string**, `+05:30`, `+0530` or `+05`, resolving to its `±HH:MM` form. A seconds field
  is a `RangeError`, as CreateDateTimeFormat requires.
- **IANA's `Etc/GMT+1` to `Etc/GMT+12` and `Etc/GMT-1` to `Etc/GMT-14`.** Item 5 named `±hh:mm`
  identifiers only. These names are fixed offsets by their definition, with no transition to look
  up, so admitting them reads item 5's "fixed offsets" rather than widening it. test262's
  `offset-timezone-gmt-same.js` compares each with its offset string.

**Every other name is a `RangeError` that names the limitation.** The names ICU writes are:

- UTC's names, `UTC` and `Coordinated Universal Time`;
- the GMT metazone's names for the zero offset, between 1970 and 9999 as ICU maps them;
- CLDR's GMT format otherwise, long (`GMT+05:30`) or short (`GMT+5:30`, `GMT-8`).

## 6. Validation

- **The retained German and English dates**, which JSD-0027's I3 acceptance asks for, are under
  [`src/tests/cldr/dates/`](../../../tests/cldr/dates/README.md). They are 1,430 lines: 10 time
  values from the first to the last, the year 0 among them, under 84 formats, each with its parts,
  6 ranges each and every format's resolved options. Node 22.22.0's ICU 77.1 answered all of them.
  The profile answers every line as ICU did, but for 352 lines in three groups, each recorded in
  `divergences.txt`:
  - Node's `format`, `formatRange` of equal ends and `toLocale*` write a space where ICU and Node's
    own `formatToParts` write U+202F (336 lines). FormatDateTime (s11.5.7) is the concatenation of
    the parts, so the profile writes ICU's character in both.
  - CLDR 48 writes the German flexible-day-period hour `h 'Uhr' B`, where CLDR 47, ICU 77.1's, writes
    `h B` (15 lines). This was found by reading the two releases' `ca-gregorian.json` for `de`. The
    CLDR 47 file was read for this comparison only, not archived.
  - Node's `resolvedOptions` reads the letters of a quoted append-item name as fields (1 line).
  The first runs differed in two more groups, both corrected:
  - a second added for a fractional second, which ICU adds only beside a minute;
  - the GMT zone's name outside its metazone's 1970 to 9999 mapping.
- **test262**, against the pinned suite:
  - `test/intl402/DateTimeFormat` passes 326 of 350 scored variants, from 6. The 24 failing are 12
    files. Ten need a calendar other than the Gregorian (`chinese`, `dangi`, `islamic-civil`,
    `iso8601`, and the related-year and calendar-pattern tests). One needs a locale the data lacks
    (`ja`), and one the `arab` digits' own decimal separator. 73 are skipped as proposals.
  - `test/intl402/Intl` passes 116 of 130, from 80. The failing ones need `Intl.Locale`,
    `Intl.DisplayNames` or `Intl.RelativeTimeFormat`.
  - `test/intl402/Date` passes all 24 variants, from 18, and `test/built-ins/Date` still passes all
    1,172. The whole of `test/intl402` passes 1,016 of its 4,418 variants, from 608, with none
    moving back.
- **A slice-compiler check** holds the dates dataset as the numbers one holds the numbers.

## 7. What is not done

- **Calendars other than the Gregorian.** `iso8601`, `buddhist`, `chinese` and the others are not in
  the data; `-u-ca-` and `calendar` resolve to `gregory`.
- **IANA time zones** other than UTC's links and the `Etc/GMT` offsets: JSD-0027's decision (d)
  holds, and a named zone is refused by name.
- **Locales**: the section 5 list, as for I1 and I2.
- **Symbols of numbering systems other than `latn`**: a fractional second's separator is the
  language's `latn` one, as JSD-0044 section 5 records for numbers.
- **Temporal objects as arguments** wait for phase F8; a `DateTimeFormat` formats time values.
- **`PluralRules`, `Locale`** and the other constructors are slice I4 and later.

## 8. What would falsify this

- A line of the retained dates that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the data or the draft's text does not bear out.
- An option read in another order than CreateDateTimeFormat's, or read twice.
- A `format` whose text is not its `formatToParts` joined, or a `formatRange` whose text is not its
  `formatRangeToParts` joined.
- A time value whose fields are not its proleptic Gregorian fields in the format's zone.
- A guest-visible answer that depends on the host's culture or time zone.
