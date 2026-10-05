<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Intl.DateTimeFormat in the CLDR calendars, checked against the reference polyfill

This is the dataset slice T4's record keeps, decision
[JSD-0057](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0057-intl-datetimeformat-in-the-cldr-calendars.md).

- [`calendar-formats.js`](calendar-formats.js) is the program. Each of its 5,951 lines is one
  formatting and its answer, or the name of the error it threw:
  - in English and German, in each of the fourteen calendars of the Intl era and month code
    proposal's Table 1 other than Gregorian and ISO 8601, under fifteen option sets (no options,
    each date style, eras in each width, months in each width, two-digit and numeric fields, a
    weekday), an `Intl.DateTimeFormat` in UTC formats thirteen dates chosen for the calendars'
    months, leap months, eras and new years: Chinese leap months in 2020 and 2023, the Hebrew Adar I
    and II of 5784 and the Adar of 5785, the last day of Showa and the first of Heisei and of Reiwa,
    Nowruz, 1 Muharram, the Coptic and Ethiopian thirteenth month, and a date in 1912;
  - for each locale and calendar, `formatToParts` with the related year and the era, ranges within a
    month, across months and across years and their parts, a date with a time, the resolved
    calendar, and each Temporal type's `toLocaleString` in the calendar, and in another, which
    throws;
  - the calendar each of 21 requested values resolves to, aliases and the deprecated Hijri
    calendars included, and `Intl.supportedValuesOf("calendar")`.

  It uses no host function and completes with its lines, so the polyfill and the profile run the
  same text.
- [`calendar-formats.polyfill-e8cc03fc.txt`](calendar-formats.polyfill-e8cc03fc.txt) is the answer of
  the reference polyfill at `e8cc03fc`, the same one [`../README.md`](../README.md) describes, which
  formats through Node 22.22.0's `Intl.DateTimeFormat`, ICU 77.1 with CLDR 47. It ran on 2026-10-05
  by the command that README gives, with this program's name.
- [`divergences.txt`](divergences.txt) names the 1,395 lines on which the profile answers otherwise,
  in four groups, each under its reason:
  - CLDR 48 names eras that CLDR 47 leaves as the placeholders `ERA0` and `ERA1` (Coptic and
    Ethiopian), writes without a diacritic (`Saka`, now `Śaka`) or abbreviates (English's long Hijri
    era, `AH`, now `Anno Hegirae`). Every line differs from the polyfill's in the era name alone
    (1,386 lines);
  - German's date-time glue in the Chinese and Korean calendars, which CLDR's data, 47's as 48's,
    gives without the comma ICU 77.1 writes (2);
  - a plain date-time in the Chinese and Korean calendars, whose year the polyfill drops (4);
  - the deprecated Hijri calendars: the calendar `islamic-rgsa` falls back to, which the proposal
    leaves to an implementation, and the list of available calendars, which Table 1 fixes (3).

The slice compiler's check `temporal/t4/calendar-formats-match-the-reference-polyfill` runs the
program and compares every line: the polyfill's, or the divergence that replaces it.
