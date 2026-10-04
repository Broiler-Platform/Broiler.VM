<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English lists, checked against ICU

This is the dataset slice I4's `Intl.ListFormat` record keeps, decision
[JSD-0048](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0048-intl-listformat.md).

- [`lists.js`](lists.js) is the program. It makes an `Intl.ListFormat` for each of `en`, `en-US`,
  `de` and `de-DE`, in each of the three types and three styles: 36 formats. Each joins six lists, of
  no element to five, one with an empty element, as a string and as parts, and writes its resolved
  options. Thirteen last lines answer the defaults, `undefined`, a string and a set as iterables, the
  errors ECMA-402 states (a non-String element, with the iterator closed; `null`; options that are
  neither undefined nor an object; an invalid type or style; a call without `new`) and
  `supportedLocalesOf`. It uses no host function, and it completes with its lines, so Node and the
  profile run the same text.
- [`lists.icu-77.1.txt`](lists.icu-77.1.txt) is Node's answer, retained as written. It comes from
  Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. It was produced on 2026-10-04
  by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('lists.js','utf8'))+'\n')" > lists.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 36 of its 481 lines on which the profile answers
  otherwise, in one group: the parts of a list with an empty element, which ICU leaves out and the
  draft keeps. Each line is the profile's answer.

The slice compiler's check `intl/i4/german-and-english-lists-match-icu` runs the same program under
the profile and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** Every string agrees. CLDR 47 and 48.2 state the same
German and English list patterns, as reading both releases' files shows. ICU is a second
implementation, not the specification, and the one divergence records the clause it is checked
against.

**Re-running.** If `lists.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and the reason is checked again.
