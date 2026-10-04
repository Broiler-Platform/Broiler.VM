<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0046 - Intl.Locale: any well-formed tag, its likely subtags, and the information the data holds

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the first part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.Locale`. It is built on the data
boundary and locale core of [JSD-0043](0043-intl-data-boundary-and-collation.md) and the formats of
[JSD-0044](0044-intl-numberformat.md) and [JSD-0045](0045-intl-datetimeformat.md).

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the fourth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.Locale`, whole, as the draft states it** (s15). It has:
  - the constructor, which takes a String or a Locale and every option, the language, script, region
    and variants among them;
  - its twelve accessors: `baseName`, `calendar`, `caseFirst`, `collation`, `firstDayOfWeek`,
    `hourCycle`, `language`, `numberingSystem`, `numeric`, `region`, `script` and `variants`;
  - `maximize`, `minimize` and `toString`;
  - the seven information methods: `getCalendars`, `getCollations`, `getHourCycles`,
    `getNumberingSystems`, `getTimeZones`, `getTextInfo` and `getWeekInfo`.
- **A Locale is any well-formed tag, not only a supported locale.** Its parts and keywords come from
  the tag and the options. Canonicalization, the aliases and the likely subtags read CLDR's data for
  every language, as `Intl.getCanonicalLocales` already does.
- **A Locale is a locale wherever ECMA-402 takes one.** CanonicalizeLocaleList (s9.2.1) reads its
  identifier, and a lone Locale is a list of one, so `getCanonicalLocales`, every `supportedLocalesOf`
  and every constructor accept it.
- **The information methods are the draft's, not the earlier accessors.** The draft has
  `getWeekInfo()` and the rest as methods. The `weekInfo`-style accessors some engines shipped belong
  to an earlier stage and are not built.

## 2. The data

- **The archive grows under rule N27** by two files of the `cldr-core` 48.2.0 tarball slice I2
  verified: `scriptMetadata.json` and `supplemental/weekData.json`. Both were retrieved twice more and
  found byte-identical to the verified tarball's.
- **Under rule N28 two tables join the generated file, and one is widened**:
  - each region's week: its first day, its weekend and the minimal days of the first week, empty
    where CLDR states none and the world's applies;
  - each script's line direction, where CLDR states one;
  - the hour cycles, which covered the supported locales' likely regions, now cover every region
    CLDR names, because a Locale's region is any region.
- **The size**: 472,562 bytes of table data on 2026-10-04, 6,693 more, in an assembly of 482,816
  bytes. That is still under JSD-0043's provisional 512 KiB bound, and no budget is set.

## 3. The locale core, corrected

Two operations of JSD-0043's locale core answered otherwise than UTS #35 states. `Intl.Locale` is
their first direct reader, and its tests found them.

- **Add Likely Subtags follows UTS #35's current algorithm.** The core looked up a language with its
  region before the language with its script, and then the same keys with `und`. UTS #35 since CLDR 44:
  - removes `Zzzz` and `ZZ`;
  - answers a tag with a language, a script and a region unchanged;
  - looks up the language with script and region, then with script, then with region, then alone.

  A language the data does not name is no longer completed from `und` with its script or region:
  `xyz-Cyrl` stays `xyz-Cyrl`, as ICU 77.1 answers it.
- **`-u-` attributes are sorted**, as UTS #35's canonical form requires. The core removed duplicate
  attributes and kept them in the tag's order.
- **Remove Likely Subtags is new** (UTS #35 s4.4). Of the language alone, the language with its
  region, and the language with its script, it keeps the first whose likely subtags are the tag's.
  Variants and extensions stay as they are.

The supported locales are maximal or minimal under both orders, so negotiation and the three formats
answer as before. The slice checks of I1 to I3 pass unchanged, and no `test/intl402` variant moved
back.

## 4. The constructor and the accessors

- **The options are read in the draft's order, each once** (s15.1.1 to s15.1.3):
  - UpdateLanguageId reads `language`, `script`, `region` and `variants`. Each is checked against
    its subtag's grammar, and the tag is canonicalized again with them;
  - then `calendar`, `collation`, `firstDayOfWeek`, `hourCycle`, `caseFirst`, `numeric` and
    `numberingSystem`.

  `firstDayOfWeek` is read as WeekdayToUValue states: `0` to `7` become `sun` to `sun`.
- **MakeLocaleRecord** puts each option over the keyword it names, canonicalizes the value by the
  BCP 47 data, and writes `true` as nothing. The Locale's keywords are the seven of
  `%Intl.Locale%.[[LocaleExtensionKeys]]` (s15.2.2): the five every implementation has (`ca`, `co`,
  `fw`, `hc` and `nu`), and `kf` and `kn`, because the profile's Collator has both.
- **The accessors answer the canonical tag's parts.** `variants` answers them joined by hyphens, in
  canonical order, and `language` answers `und` for `und`.

## 5. The information

Each method follows its draft operation, over the data the profile holds:

- **`getCalendars`** (s15.5.9): the region's calendars in use, kept where AvailableCalendars has them.
  That is `gregory` alone, the only calendar the profile formats (JSD-0045 section 7).
- **`getCollations`** (s15.5.10): the collator's own collations for a locale it supports, without the
  default, and `emoji` and `eor` for one it does not, as the operation states. German answers
  `phonebk`, and English answers an empty list.
- **`getHourCycles`** (s15.5.11): CLDR's time data for the language with the region, or the region,
  under `-u-rg` first and then the region. It gives the preferred cycle, then the allowed ones, in
  their order.
- **`getNumberingSystems`** (s15.5.12): the NumberFormat's default numbering system for a locale it
  supports, `latn` otherwise.
- **`getTimeZones`** (s15.5.13): `undefined` without a region, as the operation states, and an empty
  list with one. The zones in use in a region are the time-zone database's knowledge, which the
  profile does not carry (JSD-0027 decision (d)). Of the zones it admits, none is a region's.
- **`getTextInfo`** (s15.5.14): the line direction CLDR states for the locale's script, or for its
  likely script.
- **`getWeekInfo`** (s15.5.17): the week CLDR states for the `-u-rg` region, the region or the
  world. The first day is overridden by `-u-fw`, and the weekend is the days from its start to its
  end.
- **The region** is RegionPreference's (s15.5.8): the tag's, the one a `-u-sd` subdivision names, or
  the likely one, with `-u-rg`'s as an override.

## 6. Validation

- **The retained Locale dataset**
  ([`src/tests/cldr/locales/`](../../../tests/cldr/locales/README.md)) holds 780 lines against Node
  22.22.0's ICU 77.1. It covers 65 tags:
  - languages of many scripts and regions;
  - `und` with a script or a region;
  - deprecated and grandfathered forms, and variants;
  - `-u-`, `-t-` and private-use extensions;
  - option overrides of every keyword and of every part.

  For each tag it writes the identifier, the parts, the keywords, the maximal and minimal forms and
  the seven methods' answers. **Every identifier, part, maximal and minimal form, direction and week
  agrees with Node** except where Node predates the draft. 150 lines differ, in eight groups, each
  named in `divergences.txt`:
  - Node 22 predates three of the draft's features: `firstDayOfWeek` (8 lines), `variants` (5) and
    `language` for `und` (8);
  - the profile's data is narrower than ICU's: calendars (21), collations (28), numbering systems
    (2) and time zones (21);
  - Node answers one hour cycle where CLDR's time data allows several, all in common use (57).

  The first runs differed in two more groups, both corrected: the likely subtags' lookup order and the
  attributes' order, as section 3 records.
- **test262**, against the pinned suite:
  - `test/intl402/Locale` passes all 218 scored variants, from none.
  - 43 files are skipped, because the suite tags them `Intl.Locale-info`, a proposal. Run by hand
    under the profile with their harness, 41 pass. The two that fail ask for at least one collation
    for `en`, and at least one time zone for `en-US`. Section 5 records why the profile answers an
    empty list to both.
  - `test/intl402/Intl` passes 124 of 130, from 116. `getCanonicalLocales` now takes a Locale, and
    `supportedValuesOf`'s calendars, collations and numbering systems are each accepted by one. The
    six failing need `DisplayNames` or `RelativeTimeFormat`.
  - All of `test/intl402` passes 1,242 of its 4,418 variants, from 1,016, with none moving back.
- **A slice-compiler check** holds the Locale dataset as the dates check holds the dates.

## 7. What is not done

- **Calendars other than the Gregorian, collations other than German's `phonebk`, and numbering
  systems other than the formats' defaults** are not in the data, and the information methods answer
  from what is.
- **Time zones in use in a region**: JSD-0027's decision (d) holds.
- **`PluralRules`, `ListFormat`, `RelativeTimeFormat`, `DisplayNames`, `Segmenter` and
  `DurationFormat`** are the rest of slice I4, each its own change.

## 8. What would falsify this

- A line of the retained Locale dataset that the profile answers otherwise and `divergences.txt` does
  not name, or a divergence whose reason the data or the draft's text does not bear out.
- An option read in another order than the constructor's, or read twice.
- A Locale whose `toString` is not a canonical Unicode locale identifier, or whose `maximize` or
  `minimize` is not UTS #35's.
- An information method whose answer is not its operation's over the data the profile holds.
- A guest-visible answer that depends on the host's culture.
