<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English date formatting, checked against ICU

This is the dataset slice I3 of [JSD-0027](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0027-intl-scope-and-data-strategy.md)
section 7 asks to keep: `(locale, options, time value) → string` for German and English, over the
whole time-value range, checked against Node's ICU at a recorded ICU version. Decision
[JSD-0045](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0045-intl-datetimeformat.md)
section 6 records what it found.

- [`dates.js`](dates.js) is the program. It formats 10 time values under 84 `Intl.DateTimeFormat`s over
  `en`, `en-US`, `de` and `de-DE` and the `-u-hc`, `-u-nu` and `-u-ca` keywords. Each value is written
  as `format` gives it and as `formatToParts` gives its parts, and so are 6 ranges by `formatRange`
  and `formatRangeToParts`, and each format's `resolvedOptions`. The values are the epoch, the instant
  before it, a leap day, noon, an afternoon with milliseconds, the first days of the year 0 (1 BC)
  and of 2 BC, the last millisecond of 9999, and the first and last time values, plus and minus
  8.64e15. The ranges are two hours, a minute, a day, a month and a year apart, and one instant to
  itself. The formats cover every date and time style and their combinations; every
  component in each width; the four hour cycles by option, by keyword and by `hour12`; flexible day
  periods; fractional seconds; every time zone name style for UTC, the zero offset, fixed offsets
  and an `Etc/GMT` zone; a numbering system; and a format whose missing field CLDR's append items
  write. Two last lines answer the three `Date.prototype.toLocale*` methods. It uses no host
  function, and it completes with its lines, so Node and the profile run the same text. Every format
  names its time zone, UTC where the case does not, so that Node's host zone does not reach the
  answers.
- [`dates.icu-77.1.txt`](dates.icu-77.1.txt) is Node's answer, retained as written. It comes from
  Node 22.22.0, whose `process.versions` reports ICU 77.1, CLDR 47.0 and Unicode 16.0. It was
  produced on 2026-10-04 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('dates.js','utf8'))+'\n')" > dates.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 352 of its 1,430 lines on which the profile answers
  otherwise, in three groups, each under its reason. Node writes a space where ICU writes U+202F in a
  format's string but not in its parts (336 lines). CLDR 48 changed German's flexible-day-period hour
  pattern (15 lines). Node's `resolvedOptions` reads a quoted word as fields (1 line). Each line is the
  profile's answer.

The slice compiler's check `intl/i3/german-and-english-dates-match-icu` runs the same program under
the profile and compares every line: ICU's, or the divergence that replaces it.

**What the comparison is, and what it is not.** ICU 77.1 carries CLDR 47, and the profile carries
CLDR 48.2. The formats here read one pattern that moved between those releases, German's `Bh`, and
its lines are named. ICU is a second implementation, not the specification. ECMA-402 leaves the
patterns, the range patterns and the best-fit matcher to the implementation, and the profile follows
ICU's. Where ECMA-402 does say what an answer is, a divergence is recorded with the clause it is
checked against. A new disagreement means one of the two is wrong, which is decided before this
file changes.

**Re-running.** If `dates.js` changes, the answers are produced again by the command above, under a
Node whose ICU version is the one the file name states, or under a newer one with the file renamed.
The divergences are then produced again from the check's report, and each reason is checked again.
