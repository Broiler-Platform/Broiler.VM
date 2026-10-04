<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English number formatting, checked against ICU

This is the dataset slice I2 of [JSD-0027](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0027-intl-scope-and-data-strategy.md)
section 7 asks to keep: `(locale, options, value) → string` for German and English, checked against
Node's ICU at a recorded ICU version. Decision
[JSD-0044](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0044-intl-numberformat.md)
section 6 records what it found.

- [`numbers.js`](numbers.js) is the program. It formats 32 values under 56 `Intl.NumberFormat`s over
  `en`, `en-US`, `de` and `de-DE`, the `-u-nu` keyword and the `numberingSystem` option. Each value
  is written as `format` gives it and as `formatToParts` gives its parts, and so are 7 ranges by
  `formatRange` and `formatRangeToParts`, and each format's `resolvedOptions`. The values include
  `-0`, `NaN`, both infinities, `1e21`, ties at 0.5, 1.5, 2.5 and 1.005, grouping at 3 to 9 digits,
  exact decimal strings, a hexadecimal string, strings past the Number range in both directions, and
  BigInts. The formats cover every style, every notation, every sign display, rounding modes,
  priorities and increments, grouping, currency displays and signs, and units in each width,
  compound ones among them. It uses no host function, and it completes with its lines, so Node and
  the profile run the same text.
- [`numbers.icu-77.1.txt`](numbers.icu-77.1.txt) is Node's answer, retained as written. It comes from
  Node 22.22.0, whose `process.versions` reports ICU 77.1, CLDR 47.0 and Unicode 16.0. It was
  produced on 2026-10-04 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('numbers.js','utf8'))+'\n')" > numbers.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 24 lines on which the profile answers otherwise, in
  four groups, each under the reason ICU's answer is not ECMA-402's: ICU's range sources after a
  spaced currency code, its approximate percent range, Node's exact value for a string whose Number
  is zero, and ICU's identity test for a range. Each line is the profile's answer.

The slice compiler's check `intl/i2/german-and-english-numbers-match-icu` runs the same program
under the profile and compares every line: ICU's, or the divergence that replaces it.

**What the comparison is, and what it is not.** ICU 77.1 carries CLDR 47, and the profile carries
CLDR 48.2. None of these formats reads data that moved between those releases, so the two must agree,
and they do on every line but the named ones. ICU is a second implementation, not the specification.
Each divergence is recorded with the clause of ECMA-402 it is checked against, and a new disagreement
means one of the two is wrong, which is decided before this file changes.

**Re-running.** If `numbers.js` changes, the answers are produced again by the command above, under a
Node whose ICU version is the one the file name states, or under a newer one with the file renamed.
The divergences are then produced again from the check's report, and each reason is checked again.
