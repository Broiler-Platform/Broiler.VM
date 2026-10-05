<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# German and English durations, checked against ICU

This is the dataset slice I4's `Intl.DurationFormat` record keeps, decision
[JSD-0051](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0051-intl-durationformat.md).

- [`durations.js`](durations.js) is the program. It makes 15 `Intl.DurationFormat`s for each of `en`
  and `de`: every style, per-unit styles, displays, fractional digits, and a numbering system. Each
  writes 16 durations, the first also as parts, and writes its resolved options. The durations are:
  - every unit at once;
  - clock-like and calendar-like durations;
  - a zero, sub-second fractions and units past their carry;
  - negative durations, `-0` among them;
  - a large value grouped in thousands.

  Thirteen last lines answer the errors ECMA-402 states and `supportedLocalesOf`. It uses no host
  function, and it completes with its lines, so Node and the profile run the same text.
- [`durations.icu-77.1.txt`](durations.icu-77.1.txt) is Node's answer, retained as written. It comes
  from Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. Node 22 has
  `Intl.DurationFormat` only behind V8's `--harmony-intl-duration-format` flag, which the command
  names. It was produced on 2026-10-05 by running this in the directory:

  ```sh
  node --harmony-intl-duration-format -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('durations.js','utf8'))+'\n')" > durations.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 192 of its 553 lines on which the profile answers
  otherwise, in four groups, each under its reason:
  - Node's flagged DurationFormat predates the current draft in seven named ways: the sign, the
    clock in the list, seconds after numeric minutes, grouping, truncation, validation and the
    resolved options' order (165 lines);
  - CLDR 48 changed German's narrow hour and millisecond patterns (14);
  - the `arab` numbering system's own symbols, which the profile does not carry (12);
  - a duration string, which the Temporal proposal's amendment lets `format` read since the profile
    admitted Temporal, JSD-0054 (1; added 2026-10-05).

  Each line is the profile's answer.

The slice compiler's check `intl/i4/german-and-english-durations-match-icu` runs the same program
under the profile and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** Node 22's DurationFormat is an implementation of an
earlier stage of the proposal, so it is a weaker reference here than for the other constructors. Each
divergence of the first group names the draft's clause the profile follows instead. The lines that
agree, 361 of them, are where the stages agree: unit names, plural forms, list joining, the digital
clock and fractional seconds.

**Re-running.** If `durations.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and each reason is checked again.
