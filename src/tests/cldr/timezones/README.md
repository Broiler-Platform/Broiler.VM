<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Every tzdb 2026e identifier, checked against ICU

This is the dataset decision
[JSD-0053](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0053-time-zone-data-and-temporal-admission.md)
keeps for the IANA Time Zone Database data the profile generates.

- [`timezones.js`](timezones.js) is the program. For each of tzdb 2026e's 597 Zone and Link names it
  makes an `Intl.DateTimeFormat` with the `longOffset` zone name, and writes the identifier it
  resolves to and the UTC offset it formats at eight instants: 1850, 1900, 1940, 1975, 2000, January
  and July 2026, and 2150. Thirteen last lines answer nine regions' zones, an identifier in upper
  case, an unknown and a non-ASCII one, and the identifiers `Intl.supportedValuesOf` lists. It uses no
  host function, and it completes with its lines, so Node and the profile run the same text.
- [`timezones.icu-77.1.txt`](timezones.icu-77.1.txt) is Node's answer, retained as written. It comes
  from Node 22.22.0, whose `process.versions` reports ICU 77.1, CLDR 47.0 and tzdb 2025b. It was
  produced on 2026-10-05 by running this in the directory:

  ```sh
  node -e "const fs=require('fs');process.stdout.write((0,eval)(fs.readFileSync('timezones.js','utf8'))+'\n')" > timezones.icu-77.1.txt
  ```

  Node 22 has `Intl.Locale`'s older `timeZones` accessor rather than `getTimeZones`, so the program
  uses whichever the engine has.
- [`divergences.txt`](divergences.txt) names the 51 of its 610 lines on which the profile answers
  otherwise, in three groups, each under its reason:
  - ECMA-402 resolves a name to its IANA Zone, and V8 to CLDR's older canonical name (45 lines, on
    every one of which the offsets agree);
  - Morocco's permanent +00 from 2026-09-20, which tzdb 2026e has and Node's 2025b does not (2);
  - the same resolution in the region lists and the listed identifiers, which V8 also gives without
    UTC and the `Etc/GMT` offset zones (4).

  Each line is the profile's answer.

The slice compiler's check `tzdb/time-zone-offsets-match-icu` runs the same program under the profile
and compares every line: Node's, or the divergence that replaces it.

**What the comparison is, and what it is not.** It holds the profile's reading of its generated tables
to ICU's at 4,776 instants, and agrees at every one but Morocco's two. The compiler that wrote the
tables was held to `zic` itself, on the same 2026e files, at every transition from 1800 to 2500, as
JSD-0053 records; this dataset checks what the profile does with them. Zone names other than the GMT
format are not compared: the profile does not yet carry CLDR's metazones.

**Re-running.** If `timezones.js` changes, the answers are produced again by the command above. The
divergences are then produced again from the check's report, and each reason is checked again.
