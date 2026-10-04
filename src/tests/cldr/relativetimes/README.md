<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English relative times, checked against ICU

This is the dataset slice I4's `Intl.RelativeTimeFormat` record keeps, decision
[JSD-0049](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0049-intl-relativetimeformat.md).

- [`relativetimes.js`](relativetimes.js) is the program. It makes an `Intl.RelativeTimeFormat` for each
  of `en`, `en-US`, `de` and `de-DE` in each style and numeric option: 24 formats. Each writes twelve
  values in each of the eight units. The values are both signs of 0, values with and without a
  literal of their own, a fraction, a negative fraction, a value grouped in thousands, and one that
  rounds to 0. Each format also writes five values as parts, and its resolved options. Twelve last
  lines answer the plural units, a numbering system by option and by keyword, a string value, the
  errors ECMA-402 states and `supportedLocalesOf`. It uses no host function, and it completes with
  its lines, so Node and the profile run the same text.
- [`relativetimes.icu-77.1.txt`](relativetimes.icu-77.1.txt) is Node's answer, retained as written. It
  comes from Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. It was produced on
  2026-10-04 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('relativetimes.js','utf8'))+'\n')" > relativetimes.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 96 of its 2,460 lines on which the profile answers
  otherwise, in one group: a value that rounds to 0 under `numeric: 'auto'`, which ICU writes as the
  literal for 0 and the draft writes with its pattern. Each line is the profile's answer.

The slice compiler's check `intl/i4/german-and-english-relative-times-match-icu` runs the same program
under the profile and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** Every other line agrees, the past written with the
value's magnitude as ICU writes it. CLDR 47 and 48.2 state the same German and English relative time
data, as reading both releases' files shows. ICU is a second implementation, not the specification,
and the one divergence records the clause it is checked against.

**Re-running.** If `relativetimes.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and the reason is checked again.
