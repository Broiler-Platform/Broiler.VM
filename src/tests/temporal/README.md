<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Temporal in the ISO 8601 and Gregorian calendars, checked against the reference polyfill

This is the dataset slice T1's Temporal record keeps, decision
[JSD-0054](../../Broiler.VM.Profile.JavaScript/docs/decisions/0054-temporal-in-the-iso-and-gregorian-calendars.md).

- [`temporal.js`](temporal.js) is the program. Each of its 1,629 lines is one operation and its
  answer, or the name of the error it threw:
  - ten durations, each rounded to five largest units and totalled in six units against two plain
    dates and two zoned date-times, and rounded to months in all nine rounding modes;
  - six dates, from 1 BCE to the last representable day, with their calendar fields, arithmetic
    with and without `overflow: 'reject'`, differences in four largest units, and the Gregorian
    calendar's eras;
  - times and date-times rounded in every unit and mode;
  - seven time zones (New York, Berlin, Sydney, Kolkata, Apia across its 2011 skipped day, a fixed
    offset and UTC) at five instants near transitions: hours in the day, start of day, a day added
    as a calendar day and as 24 hours, rounding to the day, the next and previous transitions, and
    a month's difference; and five wall-clock times in a gap or a fold under each disambiguation;
  - instants rounded in every mode, including exact ties, and plain year-months and month-days.

  It uses no host function, and it completes with its lines, so the polyfill and the profile run
  the same text. Its zones and dates are ones whose rules agree between the polyfill's time zone
  data (Node's, tzdb 2025b) and the profile's (tzdb 2026e).
- [`temporal.polyfill-e8cc03fc.txt`](temporal.polyfill-e8cc03fc.txt) is the reference polyfill's
  answer, retained as written. The polyfill is `polyfill/lib` of
  [tc39/proposal-temporal](https://github.com/tc39/proposal-temporal) at `e8cc03fc` (2026-07-27),
  the revision JSD-0053 pins, with its three runtime dependencies installed. It ran on Node 22.22.0
  on 2026-10-05, from a directory holding that `lib`, by:

  ```sh
  node --js-regexp-duplicate-named-groups --input-type=module -e "await import('./lib/shim.mjs'); \
    const fs = await import('fs'); \
    process.stdout.write((0, eval)(fs.readFileSync('temporal.js', 'utf8')) + '\n')" \
    > temporal.polyfill-e8cc03fc.txt
  ```

- [`divergences.txt`](divergences.txt) names the lines on which the profile answers otherwise. It
  names none.

The slice compiler's `--checks` runs the program and compares every line
(`temporal/t1/temporal-matches-the-reference-polyfill`). The polyfill is the proposal's reference
implementation, not the specification: where the two disagree, the record says which this profile
follows and why.
