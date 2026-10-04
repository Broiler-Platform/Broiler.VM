<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English plural categories, checked against ICU

This is the dataset slice I4's `Intl.PluralRules` record keeps, decision
[JSD-0047](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0047-intl-pluralrules.md). It holds
the categories a German or English format selects, cardinal and ordinal, as I2's dataset holds the
numbers they write.

- [`plurals.js`](plurals.js) is the program. It makes 27 `Intl.PluralRules` over `en`, `en-US`, `de`
  and `de-DE`. They cover both types, fraction and significant digits, three rounding modes, a
  rounding increment, `stripIfInteger`, both rounding priorities, minimum integer digits, all three
  other notations, and a keyword and the lookup matcher. Each format selects 36 values:
  - the integers each language's cardinal and ordinal rules tell apart, from 0 to 1,000,000;
  - fractions that round to 1 or away from it;
  - negative numbers, `-0`, `NaN` and both infinities;
  - two decimal strings, `1e21` and a BigInt.

  It also selects 11 ranges, two of them refused, and writes its resolved options. Three last lines
  answer a call without `new`, an invalid type and `supportedLocalesOf`. It uses no host function, and
  it completes with its lines, so Node and the profile run the same text.
- [`plurals.icu-77.1.txt`](plurals.icu-77.1.txt) is Node's answer, retained as written. It comes from
  Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. It was produced on 2026-10-04
  by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('plurals.js','utf8'))+'\n')" > plurals.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 123 of its 1,299 lines on which the profile answers
  otherwise, in four groups, each under its reason:
  - Node 22's `select` refuses a BigInt, which the draft converts exactly (27 lines);
  - ICU resolves a range whose ends write one string by CLDR's range data, where the draft answers the
    start's category (65);
  - Node 22 predates the draft's `notation` option and its compact rounding (4);
  - the resolved options: Node 22 predates `notation`, ICU lists the categories in its rules' order
    rather than the draft's, and V8 resolves `en-US` and `de-DE` to their languages (27).

  Each line is the profile's answer.

The slice compiler's check `intl/i4/german-and-english-plurals-match-icu` runs the same program under
the profile and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** Every category the two select for one value agrees,
but for a BigInt and the compact rounding Node does not implement. CLDR 47 and 48.2 state the same
German and English cardinal, ordinal and range rules. ICU is a second implementation, not the
specification. Each divergence is where ECMA-402's current draft says what the answer is, and records
the clause.

**Re-running.** If `plurals.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and each reason is checked again.
