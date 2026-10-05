<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0058 - Time zone names

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The implementation is in the tree. It
is phase F8's slice T5, which [JSD-0057](0057-intl-datetimeformat-in-the-cldr-calendars.md) section 6
and [JSC-283](../roadmap.corrections.md#jsc-283) leave as the last gap before F8's exit gate:
`Intl.DateTimeFormat` writes an IANA zone's specific and generic names from CLDR's metazones, as ICU
does, where it wrote the localized GMT format.

**What the owner decided.** On 2026-10-05 the repository owner asked for slice T5 to be continued.
The names, the metazone periods and the exemplar cities measured about 62 KB of text, against 24,834
bytes left under the 768 KiB budget; asked whether to compress the table, raise the budget or reduce
the scope, the owner chose to **raise the budget**. The profile takes 832 KiB, the figure the question
named; JSD-0027 decision (c) is amended accordingly. Nothing here is signed beyond that choice.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. Phase F8, slice T5.

---

## 1. What is built

- **Specific names** (`timeZoneName: 'short'` and `'long'`, ICU's `z` and `zzzz`): the zone's own
  standard or daylight name where CLDR gives one (British Summer Time, Irish Standard Time), else its
  metazone's at the instant (Central European Summer Time, PST, MESZ), else the GMT format.
- **Generic names** (`'shortGeneric'`, `'longGeneric'`, ICU's `v` and `vvvv`), as ICU's
  `TimeZoneGenericNames` composes them:
  - the zone's own generic name, or its metazone's;
  - the metazone's **standard** name instead where the zone keeps no daylight time at the instant
    nor across a transition within 184 days, unless that name is the generic one (India Standard
    Time, Mountain Standard Time for Phoenix);
  - a **partial location** name where the metazone's golden zone for the locale's region keeps
    another offset or daylight flag at the instant (`Hawaii-Aleutian Time (Adak)` under CLDR 47);
  - else the **location** name: the country in the region format where the zone is the country's
    only zone or its primary one (Austria Time, Vereinigtes Königreich (Ortszeit)), the exemplar city
    otherwise (Los Angeles (Ortszeit), Sydney Time);
  - else the GMT format.
- **The locale's region** decides the golden zones: the requested region, or the language's likely
  one (`US` for `en`, `DE` for `de`).
- **UTC and fixed offsets** are written as before.
- **The daylight flag** a name reads is kept in the time zone tables (section 2).

## 2. The data and where it comes from

- **Two more files of the `cldr-core` 48.2.0 package** are archived under rule N27:
  `supplemental/metaZones.json` (each zone's metazone periods and each metazone's golden zone per
  region) and `supplemental/primaryZones.json` (the primary zone of a country with several). The
  tarball was retrieved twice, byte-identical, and equal to the integrity the pin already states.
- **The names** are in the `timeZoneNames.json` files of `de` and `en` that slice I3 archived:
  each metazone's long and short generic, standard and daylight names, each zone's own names, the
  region and fallback formats, and each zone's exemplar city.
- **The generator** writes, by primary identifier (JSD-0053's), a new table `MetaZones` (the periods,
  the golden zones and the primary zones, 1,013 lines) and each language's names into the date data
  (923 lines); an exemplar city is written only where the reader cannot derive it from the primary
  identifier's last segment.
- **The tzdb tables keep the daylight flag** (rule N30's generator and the reader): each palette entry
  carries it, and a change of the flag alone - America/Chihuahua's move to standard time in 2022 at
  its daylight offset - is a transition the reader skips where Temporal asks for a change of offset.
  The flag is the **rearguard** one ICU's data carries: where a zone line's rules still save a
  negative amount, the larger save is the daylight one, so Ireland's and Morocco's summers are
  daylight time, and Namibia's until its negative saves end in 2017.
- **The data is 841,145 bytes, 10,823 under the budget of 832 KiB**: the CLDR tables grew by 78,154
  bytes and the tzdb tables by 1,393.

## 3. Choices left to an implementation

- **The golden zone is compared at the same instant**, by offset and daylight flag; ICU compares the
  raw offset and the save at the zone's wall time. The two differ only within hours of a transition
  one zone makes and the other does not.
- **The daylight flag is the rearguard one by rule, not by ICU's rearguard file**: between Namibia's
  last switch to summer time on 2017-09-03 and its change of zone line on 2017-10-24, ICU's data names
  daylight time and the profile standard time.
- **A metazone period** without a start begins on 1970-01-01 and one without an end ends at
  9999-12-31 23:59 UTC, as ICU reads CLDR's; outside them a zone has no metazone.

## 4. Where the references disagree

The reference polyfill formats through Node 22.22.0's `Intl.DateTimeFormat`, ICU 77.1 with CLDR 47;
the profile reads CLDR 48.2. The dataset's divergences file names each line:

- **Node writes a space where ICU writes U+202F** before a day period (17 lines);
- **CLDR 48 changes which metazone a zone uses when, or a metazone's golden zone** (59 lines):
  Russia's regional zones, Istanbul, Famagusta, the Caucasus and Central Asia, Monrovia and Palmer,
  and the Hawaii-Aleutian metazone, whose golden zone is now America/Adak;
- **CLDR 48 renames metazones and exemplar cities** (199 lines): West Africa Time as one name,
  Khovd, Kyzylorda, Pohnpei, São Paulo, Córdoba, Ürümqi, Aktau, the Antarctic stations and the
  Pacific islands.

**The last two are CLDR's data, not the algorithm**: built with CLDR 47's `metaZones.json`,
`primaryZones.json` and `timeZoneNames.json` in place of CLDR 48's, in a scratch copy, the profile
answered all 258 of their lines as the polyfill does, and every other line too but six:
America/Coyhaique's, a zone tzdb added in 2025 that ICU 77.1 maps to the Chile metazone ahead of
CLDR 47, as CLDR 48 does.

## 5. Validation

- **The retained dataset** ([`src/tests/temporal/zone-names/`](../../../tests/temporal/zone-names/README.md)):
  2,713 lines - every primary identifier's four names at three instants in English and German, and
  zone names inside full styles, parts, ranges and Temporal's `toLocaleString`. **Against the
  reference polyfill at `e8cc03fc`**: 2,438 lines agree and 275 are named in the three groups above.
- **The T2 dataset**'s divergence for a specific zone name (MEZ) is gone: the profile now answers the
  line as the polyfill does.
- **test262**, against the pinned suite `ccaac100`:
  - `test/intl402/Temporal` passes **all 3,982** scored variants, where it passed 3,980: the last
    file asked `ZonedDateTime`'s `toLocaleString` for Central European Standard Time;
  - `test/built-ins/Temporal` passes all 9,176, `test/built-ins/Date` all 1,188, `test/intl402/Intl`
    all 132, `test/intl402/DateTimeFormat` 478 of 488 as before;
  - no variant scored before moves from passing.
- **F8's exit gate is met**: `test/built-ins/Temporal` and `test/intl402/Temporal` pass, and
  `Temporal` left the `absent-globals` block in slice T1.
- **The whole pinned suite**, run after the change: recorded in
  [JSC-284](../roadmap.corrections.md#jsc-284).
- **The slice compiler's checks**: one new, 646 in all; the tzdb check holds the profile's offsets to
  Node's as before.
- **Rules N27, N28 and N30**: the archive, the regenerated tables and the budget; one new architecture
  test reads the rearguard daylight flag of Dublin, Windhoek, Casablanca, Chihuahua, Istanbul and
  London.

## 6. What is not done

- **Locales other than `de` and `en`**, as for every Intl service.
- **The specific location format** (`VVVV`), which no ECMA-402 option asks for.

## 7. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a line of groups 2 and 3 that a build with CLDR 47's three files does not answer as the
  polyfill does.
- A zone's offset, or Temporal's next or previous transition, that differs from what it was before
  the daylight flag was kept.
- A metazone period, golden zone or name in the generated tables that is not what the generator
  writes from the archived files.
