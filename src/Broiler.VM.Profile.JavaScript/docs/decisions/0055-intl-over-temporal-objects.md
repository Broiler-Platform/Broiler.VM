<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0055 - Intl over Temporal objects, and the ISO 8601 calendar

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The implementation is in the tree. It
is phase F8's slice T2, as [JSD-0054](0054-temporal-in-the-iso-and-gregorian-calendars.md) section 2
plans it: `Intl.DateTimeFormat` over Temporal objects, `toLocaleString`, and the `iso8601` calendar.

**What the owner decided.** On 2026-10-05 the repository owner chose to build Temporal slice by slice,
each slice followed by a whole test262 run. Nothing here is signed beyond that choice.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. Phase F8, slice T2.

---

## 1. What is built

- **`Intl.DateTimeFormat` formats Temporal objects**, as the proposal amends ECMA-402 (s15.4 to
  s15.7). `format`, `formatToParts`, `formatRange` and `formatRangeToParts` read their arguments with
  ToDateTimeFormattable: a `PlainDate`, `PlainTime`, `PlainDateTime`, `PlainYearMonth`,
  `PlainMonthDay` or `Instant` is formatted as itself, anything else as a Number, as before. A
  `ZonedDateTime` is a TypeError, and so is a range of two types.
- **Each type has its own format** (CreateDateTimeFormat's [[TemporalPlainDateFormat]] to
  [[TemporalInstantFormat]]). Without a style, GetDateTimeFormat (s15.6.1) chooses the type's fields
  from the options and their defaults, and answers none where the options name only fields the type
  lacks, which is the TypeError a format then throws. With a style, AdjustDateTimeStyleFormat (s15.6.2)
  keeps the style's pattern where it writes only the type's fields, and otherwise the best pattern for
  the fields it has. A format is built when a value of its type is first formatted.
- **A plain value is written at UTC**, at noon for a date, so the format's time zone does not move it;
  an instant is written in the format's zone.
- **A calendar must agree** (HandleDateTimeValue, s15.6.22): a plain date or date-time in the format's
  calendar or in `iso8601`, a year-month or month-day in the format's calendar only; otherwise a
  RangeError.
- **`toLocaleString`** of every Temporal type is ECMA-402's (s15.11): a date-time format made for the
  call, with the type's required and default fields. A `ZonedDateTime` gives the format its own time
  zone, refuses a `timeZone` option, and is written as its instant with the zone's short name by
  default. JSD-0054's declared divergence, the ISO string, is gone.

## 2. The ISO 8601 calendar

- **The proposal requires `iso8601` among the available calendars** (amended ECMA-402 s15.2.1), so
  `Intl.DateTimeFormat` resolves it, from an option or a `-u-ca-` keyword, and
  `Intl.supportedValuesOf("calendar")` lists it beside `gregory`. It does so wherever Intl is built,
  with Temporal or without: the calendar is the formatter's, not Temporal's.
- **Its patterns are CLDR's, from `common/main/root.xml`.** The JSON packages the archive holds do not
  carry the `iso8601` calendar; root is the only locale file of release 48.2 that defines it, and it
  takes its months, days, day periods and eras from each locale's Gregorian calendar by alias. The
  file is archived under rule N27 beside the collation XML, retrieved twice and found byte-identical,
  and the generator writes its patterns (date, time and date-time formats, available formats, append
  items and interval formats, 148 lines) under a pseudo-locale `@iso8601`. Each language's ISO 8601
  data is its Gregorian names under those patterns.
- **The date-time glue is the language's Gregorian one**, `{1}, {0}` and `{1} 'at' {0}` in English,
  where root's ISO 8601 calendar states `{1} {0}`. That is how ICU resolves it, and the reference this
  slice is checked against is ICU's output; CLDR's own inheritance would give root's.
- **The data is 677,100 bytes**, 6,358 more, 109,332 under the owner's budget of 768 KiB.

## 3. Validation

- **The retained dataset** ([`src/tests/temporal/intl/`](../../../tests/temporal/intl/README.md)):
  1,233 formattings in English and German, in both calendars, under eighteen option sets, of each
  Temporal type, a `Date`, their parts and ranges, and each type's `toLocaleString`, against the
  reference polyfill at `e8cc03fc`, which formats through Node 22.22.0's ICU 77.1. 1,150 lines agree;
  83 are named in four groups:
  - Node writes a space where ICU writes U+202F before a day period, as the dates dataset already
    names (74 lines);
  - the polyfill fakes `dateStyle` for a year-month or month-day with fixed options, where the
    draft's AdjustDateTimeStyleFormat, which the profile follows, keeps the style's own fields (6);
  - CLDR 48 adds the available format `GyM`, which CLDR 47 lacks (2);
  - a specific zone name, `MEZ`, which needs CLDR's metazones (1).
- **test262**, against the pinned suite `ccaac100`, beside the run
  [JSC-280](../roadmap.corrections.md#jsc-280) records:
  - `test/intl402/Temporal` passes 598 of 930 scored variants, 134 more. Of the 332 that fail, 330 use
    a calendar other than ISO 8601 and Gregorian, or need a third calendar to differ from the locale's
    (T3), and 2 write a zone's long name, which needs metazones;
  - of the 82 cases elsewhere that claim the flag, 150 variants pass and 4 fail: a `dangi` calendar
    (T3), and Arabic-Indic digits for `ar-EG`, a locale the data does not carry;
  - every variant under `test/intl402`, `test/built-ins/Date` and `test/built-ins/Temporal` scored
    before keeps its verdict or moves to passing: 244 move.
- **The whole pinned suite**, run after the change: 100,180 variants, 96,938 passing, 502 failing, 44
  exhausted and 2,696 skipped; the 244 variants above moved from failing to passing, and every other
  variant's verdict is the same ([JSC-281](../roadmap.corrections.md#jsc-281)).
- **The slice compiler's checks**: one new, 642 in all, the dataset above.

## 4. What is not done

- **T3**, the calendars beyond ISO 8601 and Gregorian.
- **Zone names other than the GMT format**, which need CLDR's metazones (JSD-0053 section 7).
- **Locales beyond `de` and `en`**, as before.

## 5. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason ICU's or CLDR's data does not bear out.
- A Temporal value that a format writes in another time zone than UTC (a plain one) or its own (an
  instant), or a type whose format writes a field the type does not have.
- A formatter that resolves a calendar it cannot write, or an `iso8601` pattern that is not root's.

## Amended 2026-10-05: zone names (unsigned)

*Recorded with phase F8's slice T5; it signs nothing. Corrections entry
[JSC-284](../roadmap.corrections.md#jsc-284).*

- **Section 6's zone names are built** under proposed [JSD-0058](0058-time-zone-names.md). The
  dataset's fourth group of divergences, `MEZ`, is gone: the profile answers that line as the
  polyfill does, and the dataset names 82 lines in three groups. The two `toLocaleString` variants
  that wrote a zone's long name pass.
