<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Intl.Locale over tags of every shape, checked against ICU

This is the dataset slice I4's `Intl.Locale` record keeps, decision
[JSD-0046](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0046-intl-locale.md). A Locale is any
well-formed tag, not only a supported locale, so its likely subtags, aliases and region and script
information are read for every language. A dataset over many tags holds them where test262 holds
little: the suite scores the information methods as a proposal.

- [`locales.js`](locales.js) is the program. It makes 65 Locales: languages of many scripts and
  regions, `und` with a script or a region, deprecated and grandfathered forms, variants, `-u-`, `-t-`
  and private-use extensions, and option overrides of every keyword and of the language, script,
  region and variants. For each it writes its identifier and parts, its variants, its keywords, its
  first day of the week, its maximal and minimal forms, and what its seven information methods answer,
  each on a line of its own. It reads the information as the current draft's methods
  (`getWeekInfo`) or, where an engine has the earlier accessors (`weekInfo`), as those. It uses no
  host function and completes with its lines, so Node and the profile run the same text.
- [`locales.icu-77.1.txt`](locales.icu-77.1.txt) is Node's answer, retained as written. It comes from
  Node 22.22.0, whose `process.versions` reports ICU 77.1 and CLDR 47.0. It was produced on 2026-10-04
  by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('locales.js','utf8'))+'\n')" > locales.icu-77.1.txt
  ```

- [`divergences.txt`](divergences.txt) names the 158 of its 780 lines on which the profile answers
  otherwise, in eight groups, each under its reason:
  - Node 22 predates three of the draft's features: `firstDayOfWeek` (8 lines), `variants` (5) and
    `language` for `und` (8).
  - The profile's data is narrower than ICU's in three ways: calendars (21), collations (54) and
    numbering systems (2). For a language its Collator lacks, the profile answers since SJL-9.4 the
    collations of its default locale, en-US, which has none but the standard one, so `[]` (row L18,
    JSD-0082 decision D32).
  - A region's time zones are IANA Zone names, where Node answers CLDR's older names (3). Until
    2026-10-05, when JSD-0053 archived the IANA Time Zone Database, the profile answered no zones,
    and this group named 21 lines.
  - The profile lists every hour cycle in common use where Node lists one (57).

  Each line is the profile's answer.

  *(Revised 2026-11-11, at SJL-9.6, after bundle sjl-9-001's runs at the base and after commits, whose
  part B admitted no change here outside classes Z8 and L18 (JSD-0085). SJL-9.4's row L18 rewrote
  and added lines of the collations group. The paragraph read "names the 132 of its 780 lines", and
  its second item "calendars (21), collations (28) and numbering systems (2).", without the sentence
  after it. The calendars item's reason, and the calendar group's header in `divergences.txt`, are
  left as committed: JSD-0082 and JSD-0046 §5 record that header's reason as stale, and this change
  is not the one that rewrites it.)*

The slice compiler's check `intl/i4/locales-match-icu` runs the same program under the profile and
compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** Every identifier, part, maximal and minimal form, line
direction and week agrees with Node's except where Node predates the draft. The divergences are where
ECMA-402's information operations read data the profile holds differently from ICU, or where Node 22
implements an earlier stage, and each is recorded with the clause it is checked against.

**Re-running.** If `locales.js` changes, the answers are produced again by the command above, and the
divergences again from the check's report, each reason checked again.
