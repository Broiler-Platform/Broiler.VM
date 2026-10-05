<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0051 - Intl.DurationFormat: unit and clock formats over the profile's own number and list formats

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The code this record describes is in
the tree. It is the sixth part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.DurationFormat`, after `Locale`,
`PluralRules`, `ListFormat`, `RelativeTimeFormat` and `Segmenter` ([JSD-0046](0046-intl-locale.md)
to [JSD-0050](0050-intl-segmenter.md)). It writes with [JSD-0044](0044-intl-numberformat.md)'s
number format and [JSD-0048](0048-intl-listformat.md)'s list format.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the ninth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.DurationFormat`, whole, as the draft states it** (s13). It has:
  - the constructor, which requires `new` and reads `localeMatcher`, `numberingSystem`, `style`, each
    unit's style and display, and `fractionalDigits`, its options through GetOptionsObject;
  - `format`, `formatToParts`, `resolvedOptions` and `supportedLocalesOf`.
- **Each unit's style and display are GetDurationUnitOptions's** (s13.5.6), validated by
  ValidateDurationUnitStyle. A clock's minutes and seconds follow its hours as `2-digit`, and a
  fraction of a second follows its unit as `fractional`.
- **Every number is written by an `Intl.NumberFormat` and the whole joined by an `Intl.ListFormat`**,
  each built by its own constructor with the options PartitionDurationFormatPattern names. A unit is
  a unit format of its style. A clock is three decimal formats without grouping, joined by the
  locale's time separator. Only the first unit written carries the duration's sign.
- **A fraction is exact.** The draft adds the fractional units to a larger one as a mathematical
  value, and notes that floating point cannot. The sum is made as a BigInteger of the smallest unit
  and written as a decimal string, which the number format reads exactly. IsValidDuration's limit of
  2^53 seconds is checked the same way, in nanoseconds.
- **A Duration Record is ToDurationRecord's** (s13.5.3): its ten fields read in alphabetical order,
  each an integral Number, one at least, signs agreeing. Temporal's Duration objects and duration
  strings wait for phase F8.

## 2. The data

- **The archive does not grow.** A digital format's separators are the locale's `timeSeparator`
  number symbol, which slice I2 archived. Whether its hours have two digits is read from CLDR's
  `durationUnit` patterns, in the units file slice I2 archived: German and English write `h:mm:ss`, so
  they do not.
- **Under rule N28 one table grows**: the number data carries each language's three `durationUnit`
  patterns. The data is 503,797 bytes on 2026-10-05, 128 more, in an assembly of 515,072 bytes, 20,491
  under JSD-0043's provisional bound.

## 3. A correction to the number format

- **A unit's pattern for a plural category now falls back to the same width's `other` form before a
  wider width.** JSD-0044's unit lookup searched the widths for the category first. CLDR gives
  German's short nanosecond, millisecond and microsecond, and its narrow nanosecond, only an `other`
  form. One
  nanosecond in German short was therefore written with the long pattern, "1 Nanosekunde", where ICU
  writes "1 ns".
- The retained numbers of slice I2 do not cover these units, and all 2,241 lines answer as before.
  The duration dataset found the defect.

## 4. Validation

- **The retained duration dataset**
  ([`src/tests/cldr/durations/`](../../../tests/cldr/durations/README.md)) holds 553 lines against
  Node 22.22.0's ICU 77.1. **Node 22 has `Intl.DurationFormat` only behind V8's
  `--harmony-intl-duration-format` flag**, and implements an earlier stage of the proposal. It is a
  weaker reference here than for the other constructors. 362 lines agree, among them every unit name,
  plural form, list join, digital clock and fractional second where the stages agree. 191 differ, in
  three groups, each named in `divergences.txt`:
  - Node's stage differs from the draft in seven ways, each with the clause the profile follows (165
    lines). They are:
    - the sign written on every unit;
    - a time separator glued to a written unit;
    - seconds dropped after numeric minutes;
    - grouping in a clock;
    - rounding instead of truncation;
    - three validations it does not make;
    - `numberingSystem`'s place in `resolvedOptions`;
  - CLDR 48 changed German's narrow hour and millisecond patterns (14);
  - the `arab` numbering system's own symbols, which JSD-0044 records the profile does not carry (12).
- **test262**, against the pinned suite: `test/intl402/DurationFormat` passes 208 of 210 scored
  variants, from none. The 2 failing are one file, `supportedLocalesOf/locales-specific.js`, which
  needs Serbian. Six more files take Temporal arguments and are skipped as the proposal they claim.
- **A slice-compiler check** holds the dataset as the segment check holds the segments.

## 5. What is not done

- **Temporal's Duration objects and duration strings as arguments** wait for phase F8.
- **Locales**: the section 5 list, as for the rest of F7.
- **Symbols of numbering systems other than `latn`**, as JSD-0044 section 5 records.
- **`DisplayNames`** is the last constructor of slice I4, and it waits on the owner's size budget.

## 6. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the draft's text does not bear out.
- An option read in another order than the constructor's, or read twice.
- A unit written otherwise than an `Intl.NumberFormat` of the options PartitionDurationFormatPattern
  names writes it, or a list joined otherwise than the unit `Intl.ListFormat` joins it.
- A fractional value that differs from the exact sum of its units.
