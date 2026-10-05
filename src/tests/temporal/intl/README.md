<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Intl over Temporal objects, and the ISO 8601 calendar, checked against the reference polyfill

This is the dataset slice T2's record keeps, decision
[JSD-0055](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0055-intl-over-temporal-objects.md).

- [`temporal-intl.js`](temporal-intl.js) is the program. Each of its 1,233 lines is one formatting and
  its answer, or the name of the error it threw:
  - in English and German, in the Gregorian and the ISO 8601 calendars, under eighteen option sets
    (no options, each date and time style, single fields, a mixed set, eras, hour cycles, fractional
    seconds and an offset zone name), an `Intl.DateTimeFormat` in Pacific/Apia formats eleven
    values: a plain date in each calendar and one in 44 BCE, a plain date-time and time, a
    year-month and a month-day in each calendar, an instant and a zoned date-time; then a `Date`,
    `formatToParts`, `formatRange`, `formatRangeToParts`, and a range of two types;
  - each value's `toLocaleString`, under seven locale and option pairs;
  - the available calendars and a resolved `-u-ca-iso8601`.

  It uses no host function and completes with its lines, so the polyfill and the profile run the
  same text.
- [`temporal-intl.polyfill-e8cc03fc.txt`](temporal-intl.polyfill-e8cc03fc.txt) is the answer of the
  reference polyfill at `e8cc03fc`, the same one [`../README.md`](../README.md) describes, which
  formats through Node 22.22.0's `Intl.DateTimeFormat`, ICU 77.1 with CLDR 47. It ran on 2026-10-05
  by the command that README gives, with this program's name.
- [`divergences.txt`](divergences.txt) names the 82 lines on which the profile answers otherwise, in
  three groups, each under its reason:
  - Node writes a space where ICU writes U+202F before a day period (74 lines);
  - the polyfill fakes `dateStyle` for a year-month or month-day with fixed options, where the
    draft's AdjustDateTimeStyleFormat keeps the style's own fields (6);
  - CLDR 48's new available format `GyM`, which CLDR 47 lacks (2).

  Until slice T5 a fourth group named one line, a specific zone name the profile wrote in the GMT
  format; since CLDR's metazones are read
  ([JSD-0058](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0058-time-zone-names.md)) the
  profile answers it as the polyfill does.

The slice compiler's check `temporal/t2/intl-over-temporal-matches-the-reference-polyfill` runs the
program and compares every line: the polyfill's, or the divergence that replaces it.
