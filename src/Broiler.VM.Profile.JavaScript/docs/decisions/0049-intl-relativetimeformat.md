<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0049 - Intl.RelativeTimeFormat: CLDR's relative time data, a number format and plural rules of its own

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the fourth part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.RelativeTimeFormat`, after
`Locale`, `PluralRules` and `ListFormat` ([JSD-0046](0046-intl-locale.md) to
[JSD-0048](0048-intl-listformat.md)). It writes its numbers with
[JSD-0044](0044-intl-numberformat.md)'s number format and chooses its patterns with
[JSD-0047](0047-intl-pluralrules.md)'s plural rules.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the seventh implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.RelativeTimeFormat`, whole, as the draft states it** (s18). It has:
  - the constructor, which requires `new` and reads `localeMatcher`, `numberingSystem` (with `-u-nu`),
    `style` and `numeric`;
  - `format`, `formatToParts`, `resolvedOptions` and `supportedLocalesOf`.
- **Its number format and plural rules are made as the draft makes them**: an `Intl.NumberFormat` of
  the resolved locale and numbering system, and an `Intl.PluralRules` of the resolved locale, each by
  its own constructor. A relative time's number is written exactly as that decimal format writes it,
  grouping and fraction digits included.
- **A unit is one of the eight the draft names, or its plural** (SingularRelativeTimeUnit, s18.5.1).
  Any other is a `RangeError`, as is a value that is not finite.
- **The parts are MakePartsList's** (s18.5.3). A pattern's literals carry no unit, and the number's
  own parts each carry the unit.

## 2. The data

- **The archive does not grow.** Slice I3 archived `dateFields.json` for `de` and `en`, and the
  relative time data is in it.
- **Under rule N28 one table joins the generated file**: for each language, each of the eight units,
  and each unit's `-short` and `-narrow` fields:
  - CLDR's literals for values (`-1`, `0`, `1`, and German's `-2` and `2`);
  - the `future` and `past` pattern for each plural category.
- **The size**: 483,743 bytes of table data on 2026-10-04, 10,101 more, in an assembly of 494,592
  bytes. That is still under JSD-0043's provisional 512 KiB bound, with 40,545 bytes of headroom.
  No budget is set. The headroom is now smaller than the name tables `Intl.DisplayNames` would need
  for two languages, so the owner's decision (c) becomes a precondition of that constructor.

## 3. The pattern

- **Under `numeric: 'auto'`**, the literal CLDR names for `ToString(value)` is the answer, where there
  is one. `-0` is `"0"`, so it is "today" for a day.
- **Otherwise the tense is `past` for a negative value or `-0`**, and the pattern is the tense's for
  the plural category the plural rules select.
- **The number written is the value's magnitude.** The draft's PartitionRelativeTimePattern passes
  `ℝ(value)` to PartitionNumberPattern. Taken literally, a past value would be written with its minus
  sign inside a pattern that already says "ago", as "-1 day ago". test262 asserts "1 day ago", and
  ICU and every engine write that. The profile reads the tense as carrying the sign, and formats the
  magnitude.
- **A style's field is used where CLDR has it**, `day-short` for `short`, and otherwise the long field,
  as the draft states.

## 4. Validation

- **The retained relative time dataset**
  ([`src/tests/cldr/relativetimes/`](../../../tests/cldr/relativetimes/README.md)) holds 2,460 lines
  against Node 22.22.0's ICU 77.1. It has 24 formats over `en`, `en-US`, `de` and `de-DE` in every
  style and numeric option, each writing twelve values in each unit and five as parts, and twelve
  lines more for plural units, numbering systems and errors. **Every line agrees** but 96, in one
  group named in `divergences.txt`: under `numeric: 'auto'`, ICU writes 0.0001 as the literal for 0,
  because its number rounds to 0. The draft looks the literal up by `ToString(value)`, "0.0001",
  and writes the value with its pattern. CLDR 47, ICU 77.1's, and CLDR 48.2 state the same German
  and English relative time data. This was found by reading both releases' files.
- **test262**, against the pinned suite:
  - `test/intl402/RelativeTimeFormat` passes 148 of 160 variants, from none. The 12 failing are six
    files, and all six need Polish (`pl-pl-style-long.js`, `-short.js` and `-narrow.js`, under
    `format` and under `formatToParts`).
  - `test/intl402/Intl` passes 126 of 130. The 4 failing need `DisplayNames`.
- **A slice-compiler check** holds the dataset as the list check holds the lists.

## 5. What is not done

- **Locales**: the section 5 list, as for the rest of F7. A tag of another language resolves to the
  default, `en-US`.
- **`DisplayNames`, `Segmenter` and `DurationFormat`** are the rest of slice I4, each its own change.
  `DisplayNames` also waits on the size budget (section 2).

## 6. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the draft's text does not bear out.
- An option read in another order than the constructor's, or read twice.
- A relative time whose number is not the string a decimal `Intl.NumberFormat` of its locale and
  numbering system writes for the value's magnitude.
- A `format` whose text is not its `formatToParts` joined.
