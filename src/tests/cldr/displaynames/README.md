<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English display names, checked against ICU

This is the dataset slice I4's `Intl.DisplayNames` record keeps, decision
[JSD-0052](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0052-intl-displaynames.md).

- [`displaynames.js`](displaynames.js) is the program. For each of `en` and `de`, each of the six
  types and each style, it makes an `Intl.DisplayNames` with the code fallback and one with the none
  fallback. Languages are made under both language displays. Each pair names the codes of its type
  and writes its resolved options. The codes are:
  - languages with scripts, regions and variants, and legacy codes;
  - regions and areas, scripts, currencies, calendars and date-time fields;
  - codes the data does not name, and codes in another case.

  Thirteen last lines answer the invalid codes and errors ECMA-402 states, and
  `supportedLocalesOf`. It uses no host function, and it completes with its lines, so Node and the
  profile run the same text.
- [`displaynames.icu-77.1.txt`](displaynames.icu-77.1.txt) is Node's answer, retained as written. It
  comes from Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. It was produced on
  2026-10-05 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('displaynames.js','utf8'))+'\n')" > displaynames.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 64 of its 823 lines on which the profile answers
  otherwise, in four groups, each under its reason:
  - ECMA-402 regularizes a code's case before naming it, and V8 does not (30 lines);
  - CLDR 48 renamed three calendars these locales name (9);
  - ICU names `sl-rozaj-biske` and `und` otherwise than CLDR's data (24);
  - an empty script code is a RangeError, where V8 returns the empty string (1).

  Each line is the profile's answer.

The slice compiler's check `intl/i4/german-and-english-display-names-match-icu` runs the same program
under the profile and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** It is a comparison with one other implementation over
two locales. The lines that agree, 759 of them, are where the profile's names are ICU's: the dialect
and standard language displays, the alternative short forms, the locale pattern and separator, the
currencies' names and the date-time fields' widths.

**Re-running.** If `displaynames.js` changes, the answers are produced again by the command above.
The divergences are then produced again from the check's report, and each reason is checked again.
