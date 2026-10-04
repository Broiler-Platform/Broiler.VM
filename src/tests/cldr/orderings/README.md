<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English orderings, checked against ICU

This is the dataset slice I1 of [JSD-0027](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0027-intl-scope-and-data-strategy.md)
section 7 asks to keep: German and English orderings, checked against Node's ICU at a recorded ICU
version. Decision [JSD-0043](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0043-intl-data-boundary-and-collation.md)
section 6 records what it found.

- [`orderings.js`](orderings.js) is the program. It sorts 81 words, among them umlauts, `ß`,
  ligatures, accents, digits, punctuation and case pairs, under 27 collators over `en`, `en-US`,
  `de`, `de-DE` and German's `phonebk` collation. It covers every sensitivity, `ignorePunctuation`,
  `numeric`, both `caseFirst` orders, `usage: 'search'`, and the `-u-kn` and `-u-kf` keywords. For
  each collator it writes the words in order, with `=` between two the collator calls equal and `<`
  otherwise. It uses no host function, and it completes with its lines, so Node and the profile run
  the same text.
- [`orderings.icu-77.1.txt`](orderings.icu-77.1.txt) is Node's answer, retained as written. It
  comes from Node 22.22.0, whose `process.versions` reports ICU 77.1, CLDR 47.0 and Unicode 16.0.
  It was produced on 2026-10-04 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('orderings.js','utf8'))+'\n')" > orderings.icu-77.1.txt
  ```

The slice compiler's check `intl/i1/german-and-english-orderings-match-icu` runs the same program
under the profile, with the data `JsCldrData` hands over, and compares every line.

**What the comparison is, and what it is not.** ICU 77.1 carries CLDR 47 and UCA 16. The profile
carries CLDR 48.2 and UCA 17. None of these words is a character whose weight moved between those
versions, so the two must agree, and they do on every line. A word added later that falls in that gap
would make the comparison report a version difference, not a defect. Such a word belongs in the
archived CollationTest files, not here. ICU is a second implementation, not the specification: a
disagreement means one of the two is wrong, and the record says which before the file changes.

**Re-running.** If `orderings.js` changes, the answers are produced again by the command above,
under a Node whose ICU version is the one the file name states, or under a newer one with the file
renamed.
