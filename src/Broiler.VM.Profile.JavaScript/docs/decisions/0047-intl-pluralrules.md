<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0047 - Intl.PluralRules: CLDR's cardinal, ordinal and range rules over the number format's rounding

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the second part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.PluralRules`, after
[JSD-0046](0046-intl-locale.md)'s `Intl.Locale`. It is built on the number format of
[JSD-0044](0044-intl-numberformat.md), whose plural rules it already evaluated.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the fifth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.PluralRules`, whole, as the draft states it** (s17). It has:
  - the constructor, which requires `new` and reads `localeMatcher`, `type`, `notation`,
    `compactDisplay` and every digit option;
  - `select`, `selectRange`, `resolvedOptions` and `supportedLocalesOf`.
- **The digit options and the rounding are the number format's.** The constructor calls
  SetNumberFormatDigitOptions with the defaults 0 and 3. ResolvePlural (s17.5.2) reads the string
  FormatNumericToString writes. Both are JSD-0044's code, so `select` answers for the string a
  decimal `Intl.NumberFormat` with the same options would write.
- **`select` takes any Intl mathematical value**: a Number, a BigInt or a decimal string, converted by
  ToIntlMathematicalValue (s17.3.3) as the number format converts them.
- **`resolvedOptions` follows table 32**: `notation` always and `compactDisplay` with the compact
  notation, and `pluralCategories` sorted `zero`, `one`, `two`, `few`, `many`, `other`.

## 2. The data

- **The archive grows under rule N27** by one file of the `cldr-core` 48.2.0 tarball slice I2
  verified: `supplemental/ordinals.json`. It was retrieved twice more, checked against the
  registry's sha512 integrity and found byte-identical.
- **Under rule N28 one table joins the generated file**: each supported language's ordinal rules, read
  as the cardinal ones are, without their samples. The cardinal rules and the plural ranges are
  slice I2's.
- **The size**: 472,689 bytes of table data on 2026-10-04, 127 more, in an assembly of 483,328 bytes.
  That is still under JSD-0043's provisional 512 KiB bound, and no budget is set.

## 3. The selection

- **PluralRuleSelect** (s17.5.1) evaluates the language's rules of the type, in CLDR's order, over the
  operands of FormatNumericToString's string. The first rule that holds names the category, and
  `other` holds when none does. Not-a-number and the infinities are `other` (s17.5.2).
- **The exponent operands `c` and `e` are 0.** The string FormatNumericToString writes is the whole
  value's, not a compact or scientific mantissa. The rules of the supported languages read neither
  operand, so no answer depends on this. A language whose rules read them (French's `many` for a
  compact million) would need the exponent ComputeExponent gives, and such a language waits for a
  wider locale list.
- **A range** (s17.5.4) is a `RangeError` at not-a-number. Where both ends write one string it is the
  start's category. Otherwise it is CLDR's plural range for the two categories, and `other` where the
  data names none.
- **An ordinal range reads the same range data.** CLDR states ranges for cardinals only, and ICU
  resolves an ordinal range by them. The draft leaves PluralRuleSelectRange to the implementation.

## 4. Validation

- **The retained plural dataset**
  ([`src/tests/cldr/plurals/`](../../../tests/cldr/plurals/README.md)) holds 1,299 lines against Node
  22.22.0's ICU 77.1. It has 27 formats over `en`, `en-US`, `de` and `de-DE`, both types, every digit
  option family, three rounding modes, an increment, both priorities and the three other notations.
  Each format selects 36 values and 11 ranges and writes its resolved options. **Every category
  agrees** but for 123 lines in four groups, each named in `divergences.txt`:
  - Node 22's `select` refuses a BigInt, which the draft converts (27 lines);
  - ICU resolves a range whose ends write one string by the range data, where the draft answers the
    start's category (65);
  - Node 22 predates `notation` and its compact rounding (4);
  - the resolved options: Node 22 predates `notation`, ICU lists the categories in its rules' order,
    and V8 resolves `en-US` and `de-DE` to their languages (27).

  CLDR 47, ICU 77.1's, and CLDR 48.2 state the same German and English cardinal, ordinal and range
  rules. This was found by reading both releases' files.
- **test262**, against the pinned suite:
  - `test/intl402/PluralRules` passes 78 of 82 scored variants, from 4. The 4 failing are two files,
    and both need locales the data lacks: `plural-categories-order.js` (`ar`, `fa`, `fr`, `gv`, `ko`,
    `sl`) and `select/notation.js` (`fr`).
  - 11 files are skipped, because the suite tags them `Intl.NumberFormat-v3`, a proposal: `selectRange`
    and the option and key order. Run by hand under the profile with their harness, all 11 pass.
- **A slice-compiler check** holds the plural dataset as the Locale check holds the Locales.

## 5. What is not done

- **Locales**: the section 5 list, as for I1 to I4's `Locale`. A tag of another language resolves to
  the default, `en-US`.
- **The exponent operands**, which no supported language reads (section 3).
- **`ListFormat`, `RelativeTimeFormat`, `DisplayNames`, `Segmenter` and `DurationFormat`** are the
  rest of slice I4, each its own change.

## 6. What would falsify this

- A line of the retained plural dataset that the profile answers otherwise and `divergences.txt` does
  not name, or a divergence whose reason the data or the draft's text does not bear out.
- An option read in another order than the constructor's, or read twice.
- A category `select` answers that is not the rules' category for the string a decimal
  `Intl.NumberFormat` with the same digit options writes.
- A `pluralCategories` list that misses a category `select` can answer, or names one it cannot.
