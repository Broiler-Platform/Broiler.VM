<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Temporal in the CLDR calendars, checked against the reference polyfill and ICU4X

This is the dataset slice T3's record keeps, decision
[JSD-0056](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0056-temporal-in-the-cldr-calendars.md).

- [`temporal-calendars.js`](temporal-calendars.js) is the program. Each of its 3,999 lines is one
  operation and its answer, or the name of the error it threw, in each of the fifteen calendars of
  the Intl era and month code proposal's Table 1 other than ISO 8601:
  - the calendar fields (era, era year, year, month, month code, day, day of year, days in the month
    and the year, months in the year, leap year) of 137 ISO dates: every 613th day from 1850 to 2050,
    and seventeen dates from 501 BCE to the year 10000, among them the first days of the Japanese
    eras and the Gregorian reform (2,055 lines);
  - dates from fields - ordinal months, month codes, a thirteenth month, a leap month, eras, and the
    errors of a missing or a disagreeing field - each date added nine durations to, with and without
    `overflow: 'reject'`, differences to five dates in four largest units and rounded, `with`,
    year-months and month-days made from it, a year-month added to and differenced, and a duration
    rounded and totalled relative to it;
  - a month-day for fifteen month codes and days 1, 29 and 30, which names its reference year.

  It uses no host function and completes with its lines, so the polyfill and the profile run the
  same text.
- [`temporal-calendars.polyfill-e8cc03fc.txt`](temporal-calendars.polyfill-e8cc03fc.txt) is the answer
  of the reference polyfill at `e8cc03fc`, the same one [`../README.md`](../README.md) describes. The
  polyfill computes every calendar but ISO 8601 by formatting dates with Node 22.22.0's
  `Intl.DateTimeFormat`, that is with ICU4C 77.1. It ran on 2026-10-05 by the command that README
  gives, with this program's name.
- [`temporal-calendars.icu4x-2.3.0.txt`](temporal-calendars.icu4x-2.3.0.txt) is ICU4X's answer for the
  2,055 conversion lines: [`icu4x-reference.rs`](icu4x-reference.rs), built on 2026-10-05 with Rust
  1.97.0 against `icu_calendar` 2.3.0 and `calendrical_calculations` 0.2.4, the crates
  [`../../calendars/pins/`](../../calendars/pins/README.md) archives (with `compiled_data`, which adds
  `icu_calendar_data` 2.3.0 for the Japanese eras), writes each calendar's fields of the same dates
  in the program's line format. ICU4X is the implementation the proposal's test262 cases were
  checked against, and the source of the published years the profile reads.
- [`divergences.txt`](divergences.txt) names the 70 lines on which the profile answers otherwise than
  the polyfill, in three groups, each under its reason. On every one of them ICU4X answers as the
  profile does:
  - Chinese and Korean years outside the published ones, which Table 1 leaves to an approximation:
    ICU4C's is astronomical, ICU4X's and the profile's are mean new moons and solar terms (64);
  - five Chinese dates within the published years where ICU4C's astronomy disagrees with the
    observatories' calendars, among them two defects ICU tracks (5);
  - the Hebrew year 5806, which ICU4C makes a day longer than the calendar's arithmetic does (1).

The slice compiler's checks run the program and compare every line:
`temporal/t3/calendars-match-the-reference-polyfill` against the polyfill's answer or the divergence
that replaces it, and `temporal/t3/calendar-conversions-match-icu4x` against every line of ICU4X's.
