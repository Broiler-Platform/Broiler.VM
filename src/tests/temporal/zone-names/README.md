<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Time zone names, checked against the reference polyfill

This is the dataset slice T5's record keeps, decision
[JSD-0058](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0058-time-zone-names.md).

- [`zone-names.js`](zone-names.js) is the program. Each of its 2,713 lines is one formatting and its
  answer, or the name of the error it threw:
  - in English and German, for each of the 445 primary identifiers the profile lists, at three
    instants - January and July 2024, and July 1995 - the zone's name under each of the four
    `timeZoneName` styles that name it (`short`, `long`, `shortGeneric`, `longGeneric`), one line to
    a zone and instant;
  - for seven zones, the full date and time style, the long time style, `formatToParts`, a range
    across a change of offset, and `Temporal.ZonedDateTime`'s `toLocaleString` with a long and a
    short generic name, and test262's Vienna case.

  It uses no host function and completes with its lines, so the polyfill and the profile run the
  same text. The zones are written out, because ICU lists some under CLDR's older names.
- [`zone-names.polyfill-e8cc03fc.txt`](zone-names.polyfill-e8cc03fc.txt) is the answer of the reference
  polyfill at `e8cc03fc`, the same one [`../README.md`](../README.md) describes, which formats through
  Node 22.22.0's `Intl.DateTimeFormat`, ICU 77.1 with CLDR 47. It ran on 2026-10-05 by the command
  that README gives, with this program's name.
- [`divergences.txt`](divergences.txt) names the 275 lines on which the profile answers otherwise, in
  three groups, each under its reason:
  - Node writes a space where ICU writes U+202F before a day period (17 lines);
  - CLDR 48 changes which metazone a zone uses when, or a metazone's golden zone (59);
  - CLDR 48 renames metazones and exemplar cities (199).

  The last two are CLDR's data, not the algorithm: the profile built with CLDR 47's
  `metaZones.json`, `primaryZones.json` and `timeZoneNames.json` for `de` and `en` in their place, on
  2026-10-05, answered every one of their 258 lines as the polyfill does, and every other line as
  well but America/Coyhaique's six, which ICU 77.1 maps to the Chile metazone ahead of CLDR 47.

The slice compiler's check `temporal/t5/zone-names-match-the-reference-polyfill` runs the program
and compares every line: the polyfill's, or the divergence that replaces it.
