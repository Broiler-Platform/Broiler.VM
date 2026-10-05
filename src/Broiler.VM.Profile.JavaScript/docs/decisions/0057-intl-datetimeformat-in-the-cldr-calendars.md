<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0057 - Intl.DateTimeFormat in the CLDR calendars

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The implementation is in the tree. It
is phase F8's slice T4, which [JSD-0056](0056-temporal-in-the-cldr-calendars.md) section 6 leaves
open: `Intl.DateTimeFormat` writes every calendar of the Intl era and month code proposal's Table 1,
and `Intl.supportedValuesOf("calendar")` lists them, as the proposal's 1.1.1 asks.

**What the owner decided.** On 2026-10-05 the repository owner chose to build Temporal slice by slice,
each slice followed by a whole test262 run, set the Intl data budget at 768 KiB, and asked that day
for slice T4 to be continued. Nothing here is signed beyond those choices.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. Phase F8, slice T4.

---

## 1. What is built

- **The formatter resolves every calendar of Table 1**: `buddhist`, `chinese`, `coptic`, `dangi`,
  `ethioaa`, `ethiopic`, `gregory`, `hebrew`, `indian`, `islamic-civil`, `islamic-tbla`,
  `islamic-umalqura`, `iso8601`, `japanese`, `persian` and `roc`, from the `calendar` option or the
  `-u-ca-` keyword, the aliases `ethiopic-amete-alem` and `islamicc` canonicalized by the locale
  resolution's key data. `islamic` and `islamic-rgsa`, which the proposal's CreateDateTimeFormat
  falls back from, resolve to `islamic-tbla`.
- **`Intl.supportedValuesOf("calendar")`** lists those sixteen, sorted, and not the two deprecated
  Hijri ones, which are not rows of Table 1.
- **Each calendar's fields are Temporal's.** The formatter takes a time value's local Gregorian
  date and asks the calendar `JsCalendars.cs` built for slice T3 for its year, month, day, day of
  the year, era and era year, so a date formats in the calendar exactly as `Temporal.PlainDate`'s
  getters answer in it: the Japanese eras from Meiji 6, a Coptic year before the epoch as a negative
  year of `am`, a Hijri year before the Hijra as a year of `bh`, ICU4X's Chinese and Korean years.
- **CLDR's names and patterns for each calendar**:
  - the month names, by month code: a Hebrew month by CLDR's thirteen keys (Adar I as `6`, Adar II
    and the leap year's Adar as `7-yeartype-leap`), a Chinese or Korean month by its number, with
    CLDR's `monthPatterns` for a leap month (`4bis`, `Mo4bis`) in each width and context;
  - the era names by CLDR's era index of Table 2's era (the Japanese eras Meiji to Reiwa are 232 to
    236; Gregorian years before Meiji 6 are written in CLDR's Gregorian eras);
  - the Chinese and Korean calendars' cyclic year names (`U`, `jia-chen`) and related Gregorian year
    (`r`), which `formatToParts` types `yearName` and `relatedYear`;
  - each calendar's date formats, available formats, interval formats and date-time glue.

## 2. The data and where it comes from

- **Eleven more npm packages of CLDR 48.2.0** are archived under rule N27 in
  `src/tests/cldr/pins/`: `cldr-cal-buddhist-full`, `-chinese-`, `-coptic-`, `-dangi-`, `-ethiopic-`,
  `-hebrew-`, `-indian-`, `-islamic-`, `-japanese-`, `-persian-` and `-roc-full`, each tarball
  retrieved twice, byte-identical and equal to the registry's sha512 integrity, and from each its
  `package.json` and its `ca-*.json` files for `de` and `en` but `ca-islamic-rgsa.json`; and from
  `cldr-dates-full`, retrieved again and identical to slice I3's, `ca-generic.json` for `de` and `en`.
  43 files in all.
- **The generator writes each calendar as a layer**, so the data holds only what differs:
  - `lang@generic` is CLDR's generic calendar's patterns over the language's Gregorian data, the
    patterns every calendar without its own inherits;
  - `lang@<calendar>` is the calendar's names and patterns, each line one that differs from the layer
    below, and a line `-key` a key the layer below has and the calendar has not. `dangi` lies over
    `chinese`, `ethioaa` over `ethiopic`, the three Hijri calendars over `islamic`, every other
    calendar over `generic`; Japanese eras before Meiji are left out, as no Temporal date names them.
  - The reader composes each calendar's data from its layers once, when the tables are first read.
- **The layers are 1,924 lines and 82,540 bytes. The data is 761,598 bytes, 24,834 under the owner's
  budget of 768 KiB.**
- **`de` and `en` only**, as for every other Intl table; a locale that resolves to neither formats
  as `en` does, as before.

## 3. Choices the proposal and ECMA-402 leave to an implementation

- **The fallback of `islamic-rgsa`**: the proposal's CreateDateTimeFormat, as test262 states it, lets
  an implementation choose any available calendar; the profile takes `islamic-tbla` for both
  deprecated identifiers, the one the proposal's text names for `islamic`.
- **A date before a calendar's epoch** is written in the era and era year Temporal gives it (Table
  2): `-84 AM` for a Coptic year before the epoch, as `era` and `eraYear` read; ICU writes such years
  in a placeholder era counted backwards.
- **The Japanese calendar before Meiji 6** is written in Gregorian years and eras, as Table 2's eras
  `ce` and `bce` give it, where ICU names the earlier Japanese eras (`3 Kaei`); CLDR's 232 earlier
  eras are not in the data.
- **The Buddhist and ROC calendars before 1582** are proleptic Gregorian, as Temporal's are; ICU
  switches to the Julian calendar.

## 4. Where the references disagree

The reference polyfill formats through Node 22.22.0's `Intl.DateTimeFormat`, ICU 77.1 with CLDR 47;
the profile reads CLDR 48.2. The dataset's divergences file names each line, in four groups:

- **CLDR 48 names eras CLDR 47 does not**: the Coptic and Ethiopian eras, `ERA0` and `ERA1` in
  CLDR 47, are `AA`, `AM` and `Anno Martyrum`; the Indian `Saka` is `Śaka`; English's long name of
  the Hijri era, `AH`, is `Anno Hegirae`. Each of the 1,386 lines differs from the polyfill's in the
  era name alone, which the archived CLDR 47 packages, retrieved for the comparison and not kept,
  confirm.
- **German's date-time glue in the Chinese and Korean calendars**: CLDR's data, 47's as 48's, gives
  `{1} {0}`; ICU writes the generic calendar's `{1}, {0}` (2 lines).
- **The polyfill drops the year of a plain date-time in the Chinese and Korean calendars**: it builds
  that formatter from `resolvedOptions()`, which ICU gives no `year` for them; a `Date` formatted with
  the same options keeps the year in Node as in the profile (4 lines).
- **The deprecated Hijri calendars** (section 3): ICU falls `islamic-rgsa` back to
  `islamic-umalqura`, and lists both deprecated identifiers among the available calendars (3 lines).

## 5. Validation

- **The retained dataset** ([`src/tests/temporal/calendars-intl/`](../../../tests/temporal/calendars-intl/README.md)):
  5,951 lines - in English and German, in the fourteen calendars other than Gregorian and ISO 8601,
  fifteen option sets over thirteen dates chosen for leap months, eras and new years, then parts,
  ranges and their parts, a date with a time, each Temporal type's `toLocaleString` in the calendar
  and in another, and the calendar 21 requested values resolve to. **Against the reference polyfill
  at `e8cc03fc`**: 4,556 lines agree and 1,395 are named in the four groups above.
- **test262**, against the pinned suite `ccaac100`:
  - `test/intl402/DateTimeFormat` passes 478 of 488 scored variants, where it passed 456: the 10
    that fail need a locale other than `de` and `en` (Chinese related years, Japanese's `h11` hour
    cycle) or a date's digits in another numbering system, and failed before;
  - `test/intl402/Temporal` passes 3,980 of 3,982, where it passed 3,962: the two that fail write a
    zone's long name, which needs CLDR's metazones;
  - `test/intl402/Intl` passes all 132, where it passed 130;
  - no variant scored before moves from passing.
- **The whole pinned suite**, run after the change: recorded in
  [JSC-283](../roadmap.corrections.md#jsc-283).
- **The slice compiler's checks**: one new, 645 in all - the dataset against the polyfill.
- **Rule N28** holds the regenerated tables to the generator and the data to the budget; the
  architecture suite's 339 tests pass.

## 6. What is not done

- **Locales other than `de` and `en`**, here as for every Intl service, and **numbering systems
  other than Latin digits in dates**: the ten failing variants under `test/intl402/DateTimeFormat`
  stay failing.
- **Zone long names**, which need CLDR's metazones, as JSD-0055 says.
- **The `canonical-tz` proposal's cases** stay skipped, as JSD-0054 says.

## 7. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose stated reason the CLDR data does not bear out.
- A date that formats in a calendar with a year, month, day or era other than the one its Temporal
  getters answer in that calendar.
- A scored case under `test/intl402/DateTimeFormat`, `test/intl402/Temporal` or `test/intl402/Intl`
  that fails for a reason other than section 6 names.
- A layer line of the generated tables that is not what the generator writes from the archived
  files, or a composed calendar whose names or patterns differ from the CLDR file's.
