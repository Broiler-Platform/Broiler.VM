<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0044 - Intl.NumberFormat: exact decimals, CLDR patterns and ICU's layers

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is slice I2 of [JSD-0027](0027-intl-scope-and-data-strategy.md) section 7,
`Intl.NumberFormat`, built on the data boundary of
[JSD-0043](0043-intl-data-boundary-and-collation.md).

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the second implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code are that draft's. The profile
pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built, and why wider than I2 was drawn

- **`Intl.NumberFormat`, whole.** It has every style (`decimal`, `percent`, `currency`, `unit`) and
  every notation (`standard`, `scientific`, `engineering`, `compact` in both displays). It has every
  sign display, the nine rounding modes, the three rounding priorities and the fifteen increments,
  `trailingZeroDisplay`, `useGrouping` in all its forms, every currency display and both currency
  signs, and every sanctioned unit and their `-per-` compounds in three widths. The members are
  `format`, `formatToParts`, `formatRange`, `formatRangeToParts`, `resolvedOptions` and
  `supportedLocalesOf`. It also takes ECMA-402's normative optional legacy constructor path (a call on
  an object that inherits from the prototype stores the format there under
  `IntlLegacyConstructedSymbol`).
- **JSD-0027 drew I2 narrower**: "Units, compact and scientific notation are a later slice". That is
  not taken. ECMA-402 admits those options on every `Intl.NumberFormat`, so a constructor that
  accepted them and formatted as if they were absent would be the partial surface section 6 refused,
  and refusing them would be a constructor ECMA-402 does not describe. The data for all of them is in
  the same CLDR files.
- **`Number.prototype.toLocaleString` and `BigInt.prototype.toLocaleString`** route through the
  realm's own `%Intl.NumberFormat%`. **`Array.prototype.toLocaleString` and
  `%TypedArray%.prototype.toLocaleString`** hand each element the locales and options, as ECMA-402
  s20.5.1 replaces them. Without `Intl` all four are unchanged.

## 2. The data

- **The archive grows under rule N27.** It gains `numbers.json` for `de`, `en` and `und`,
  `currencies.json` for `de` and `en`, `units.json` for `de` and `en`, and from `cldr-core`
  `currencyData.json`, `numberingSystems.json`, `plurals.json` and `pluralRanges.json`. Each was taken
  the way JSD-0043's were: npm tarballs checked against the registry's integrity, retrieved twice and
  compared byte for byte.
- **Seven tables join the generated file under rule N28**: each language's number data flattened to
  dotted keys, its currency names, the currencies whose fraction digits are not 2, the numeric
  numbering systems, the cardinal plural rules and plural ranges, and the patterns of the sanctioned
  units and of the compounds CLDR names. A currency symbol also states whether its first and last
  characters are symbols or separators, read from the pinned UCD, because CLDR's currency spacing
  asks that and the profile has no General_Category table.
- **The size**: 438,831 bytes of table data on 2026-10-04, and an assembly of 448,000 bytes. That is
  still under JSD-0043's provisional 512 KiB bound, and no budget is set.
- **Currency digits are CLDR's.** The current draft makes CurrencyDigits implementation-defined, and
  CLDR's fractions are what ICU uses.

## 3. The exact decimal

- **ToIntlMathematicalValue is followed whole.** A Number is read as its shortest round-trip decimal
  (`1.005` rounds to `1.01`), and a String as an exact StringNumericLiteral, hexadecimal, octal and
  binary included. A BigInt is read exactly. A value whose Number would be an infinity or a zero
  becomes that infinity or that zero (RoundMVResult, step 9).
- **The value is digits and a point, never a binary integer** (`JsDecimal`). ToRawFixed and
  ToRawPrecision round a digit string at a decimal position. Every admitted increment divides
  10,000, so the remainder and the quotient's parity come from the last five digits. A BigInt of a
  million bits formats in time linear in its digits, and charges for them.

## 4. The formatting

- **Patterns are CLDR's**, read as UTS #35 Part 3 reads them: quoted literals, the currency sign,
  the percent and per-mille signs, the sign's place, and a negative subpattern where one is given.
- **A number is built in ICU's three layers**: the exponent (inner), the pattern's affixes with the
  sign written where ICU's `patternInfoToStringBuilder` writes it (middle), and a unit's or a
  currency's name (outer). A compact currency pattern carries its own currency. Any other compact
  pattern's affix joins the style's next to the number.
- **A range shares or repeats each layer by ICU's rules.** The outer layer is shared when it is the
  same name, in the plural the language's plural ranges give, or `other` where they name none. The
  middle layer is shared when its text is equal and longer than one character. Spaces go round the
  separator when a layer is repeated. Two values that format alike are written approximately, with
  the approximately sign at the sign's place, as the spec's identity test says.
- **Parts follow ICU's fields.** A field is trimmed of default-ignorable characters into its
  neighbouring literals, so the compact `1 thousand` is an integer, a literal and a compact part.
  Adjacent literals join, and a joined literal whose pieces came from two sources is `shared`.
- **Grouping is ICU's test**: the locale's minimum grouping digits for `auto`, two for `min2`, one for
  `always`.
- **Plural categories come from CLDR's rules**, read by a UTS #35 evaluator over the formatted
  number's operands, so a currency name or a unit is chosen by `1.00` as `other`.

## 5. Numbering systems

Every numeric system CLDR lists is supported, by `-u-nu` or `numberingSystem`, with `latn` the default.
**The symbols are the locale's `latn` ones.** ICU writes `arab` and `arabext` with root's own
symbols (`٫`, `٬`, and a minus sign carrying an Arabic letter mark), which the cldr-json `und` file does
not carry. Digits are right in every system, and the separators of those two are not. That is a
limitation of this slice.

## 6. Validation

- **The retained German and English numbers**, which JSD-0027's I2 acceptance asks for, are under
  [`src/tests/cldr/numbers/`](../../../tests/cldr/numbers/README.md). They are 2,241 lines: 32 values
  under 56 formats, each with its parts, 7 ranges each and every format's resolved options. Node
  22.22.0's ICU 77.1 answered all of them. The profile answers every line as ICU did, but for 24 in
  four groups, each checked against the draft's text and recorded in `divergences.txt`:
  - ICU shifts range sources by the currency spacing it inserts after a currency code (4 lines);
  - ICU multiplies an approximate percent range by 100 twice (6 lines);
  - Node keeps the exact value of a string whose Number is zero (13 lines);
  - ICU's range identity compares values, where the spec compares formatted strings (1 line).

  The first run differed in one more line: a plural range CLDR does not name resolved to the end's
  category. ICU's `other` is the correction.
- **test262**, against the pinned suite:
  - `test/intl402/NumberFormat` passes 280 of 324 scored variants, from 2. Every failing one
    expects a locale the data does not carry (`ja-JP`, `ko-KR`, `zh-TW`, `en-IN`), except
    `this-value-ignored.js`, which needs `DateTimeFormat`. 91 are skipped as proposals.
  - `Number/prototype/toLocaleString` and `BigInt/prototype/toLocaleString` under `intl402` pass all
    36 variants, and `Array` and `TypedArray` theirs.
  - Under `test/built-ins`, the `toLocaleString` directories of `Array`, `TypedArray`, `Number` and
    `BigInt` pass all 110 variants.
- **A slice-compiler check** holds the numbers dataset as the collation one holds the orderings.

## 7. What is not done

- **Locales**: `ja`, `ko`, `zh-TW` and `en-IN` are not in the data. Each is a size line JSD-0027
  section 5 item 4 asks a reviewer to read, and none is added here.
- **`Intl.supportedValuesOf`** waits for I3, so that its `calendar` and `timeZone` lists describe a
  `DateTimeFormat` that exists.
- **Symbols of numbering systems other than `latn`**, section 5.
- **`Intl.DateTimeFormat`, `PluralRules`, `Locale`** and the other constructors are later slices.

## 8. What would falsify this

- A line of the retained numbers that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the draft's text does not bear out.
- A value rounded otherwise than ApplyUnsignedRoundingMode states, for any mode, increment or digit
  option.
- An option read in another order than InitializeNumberFormat's, or read twice.
- A format whose work is not linear in the digits of its value, or not charged.
- A guest-visible answer that depends on the host's culture.
