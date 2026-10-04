<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0048 - Intl.ListFormat: CLDR's list patterns, one template per type and style

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the third part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.ListFormat`, after
[JSD-0046](0046-intl-locale.md)'s `Intl.Locale` and [JSD-0047](0047-intl-pluralrules.md)'s
`Intl.PluralRules`.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the sixth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.ListFormat`, whole, as the draft states it** (s14). It has:
  - the constructor, which requires `new` and reads `localeMatcher`, `type` and `style`;
  - `format`, `formatToParts`, `resolvedOptions` and `supportedLocalesOf`.
- **The options are not coerced.** ResolveOptions takes the options through GetOptionsObject, so
  `undefined` is an empty bag and any other non-object is a `TypeError`. The profile already had that
  operation for `Uint8Array`'s base64 methods, and the constructor uses it.
- **A list is any iterable of Strings** (StringListFromIterable, s14.5.5). `undefined` is the empty
  list. A value that is not a String closes the iterator and is a `TypeError`.
- **The parts are CreatePartsFromList's** (s14.5.2). A list of two takes the pair pattern. A longer
  one takes the end pattern for its last two elements, then the middle patterns leftward, then the
  start pattern. Every element is a part of type `element`, an empty one included, and the text
  between them is `literal`.

## 2. The data

- **The archive grows under rule N27** by one more npm package, `cldr-misc-full` at 48.2.0. Its
  tarball was retrieved twice, found byte-identical and checked against the registry's sha512
  integrity. From it the archive holds `listPatterns.json` for `de` and `en` and its `package.json`.
- **Under rule N28 one table joins the generated file**: each supported language's nine list
  patterns, each with its start, middle, end and two-element patterns. They are `standard`, `or` and
  `unit`, each in a long, a `-short` and a `-narrow` form.
- **The size**: 473,642 bytes of table data on 2026-10-04, 953 more, in an assembly of 484,352 bytes.
  That is still under JSD-0043's provisional 512 KiB bound, and no budget is set.

## 3. The templates

- **A type and a style select one of CLDR's nine patterns**: `conjunction` is `standard`,
  `disjunction` is `or` and `unit` is `unit`. `long` is the plain form, and `short` and `narrow` are
  CLDR's `-short` and `-narrow`.
- **[[Templates]] holds one template.** The draft lets an implementation choose among several by the
  elements, as Spanish chooses `y` or `e` by the word that follows. German and English have no such
  choice, so the index CreatePartsFromList computes is always 0.

## 4. Validation

- **The retained list dataset**
  ([`src/tests/cldr/lists/`](../../../tests/cldr/lists/README.md)) holds 481 lines against Node
  22.22.0's ICU 77.1. It has 36 formats, every type and style over `en`, `en-US`, `de` and `de-DE`.
  Each joins six lists, of none to five elements, one with an empty element, as a string and as
  parts. Thirteen lines more answer the iterables and the errors the draft states. **Every string
  agrees.** 36 lines differ, in one group named in `divergences.txt`: ICU leaves no part for an empty
  element, where the draft makes one of type `element` with the empty String. CLDR 47, ICU 77.1's,
  and CLDR 48.2 state the same German and English patterns. This was found by reading both releases'
  files.
- **test262**, against the pinned suite: `test/intl402/ListFormat` passes 154 of 162 variants, from
  2. The 8 failing are four files, and all four need Spanish: `es-es-long.js` and `es-es-short.js`
  under `format` and under `formatToParts`.
- **A slice-compiler check** holds the list dataset as the plural check holds the plurals.

## 5. What is not done

- **Locales**: the section 5 list, as for the rest of F7. A tag of another language resolves to the
  default, `en-US`, and writes English lists.
- **Choosing among templates by the elements**, which no supported language needs (section 3).
- **`RelativeTimeFormat`, `DisplayNames`, `Segmenter` and `DurationFormat`** are the rest of slice
  I4, each its own change.

## 6. What would falsify this

- A line of the retained list dataset that the profile answers otherwise and `divergences.txt` does
  not name, or a divergence whose reason the draft's text does not bear out.
- An option read in another order than the constructor's, read twice, or coerced.
- A `format` whose text is not its `formatToParts` joined.
- A list whose iterator is not closed when an element is not a String.
