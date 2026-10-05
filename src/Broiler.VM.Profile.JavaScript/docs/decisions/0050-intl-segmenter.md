<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0050 - Intl.Segmenter: UAX #29's default boundaries from the pinned UCD, without a dictionary

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the fifth part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.Segmenter`, after `Locale`,
`PluralRules`, `ListFormat` and `RelativeTimeFormat` ([JSD-0046](0046-intl-locale.md) to
[JSD-0049](0049-intl-relativetimeformat.md)).

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the eighth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04, and Unicode Standard Annex #29 for Unicode 17.0.0. Clause
and rule numbers in the code and below are theirs. The profile pins no ECMA-402 edition. The pinned
test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.Segmenter`, whole, as the draft states it** (s19). It has:
  - the constructor, which requires `new` and reads `localeMatcher` and `granularity`, its options
    through GetOptionsObject;
  - `segment`, `resolvedOptions` and `supportedLocalesOf`;
  - Segments objects, with `containing` and `[Symbol.iterator]`;
  - Segment Iterators on `%Iterator.prototype%`, tagged "Segmenter String Iterator";
  - segment data objects with `segment`, `index`, `input`, and `isWordLike` for words.
- **The boundaries are UAX #29's default rules** for grapheme clusters (GB1 to GB999, GB9c's
  conjuncts and GB11's emoji sequences included), words (WB1 to WB999) and sentences (SB1 to SB998).
  They apply to every locale, with no locale tailoring. On the retained dataset, ICU's boundaries for
  `en` and `de` are the same defaults but for its dictionary (section 4).
- **A Segments object finds its boundaries once**, on the first `containing` or step of an iterator,
  and keeps them. FindBoundary is then a search, and every iterator of the object shares them. Every
  code point scanned is charged.

## 2. The data

- **The UCD archive grows under rule N22** by UAX #29's six `auxiliary/` files of Unicode 17.0.0: the
  three break property files and their three conformance test files. They were retrieved twice from
  the same site as the rest, found byte-identical, and archived unmodified. The test files generate
  nothing, like `NormalizationTest.txt`.
- **The tables are in the Intl data assembly, not the profile's Unicode tables.** A composition that
  does not admit `broiler.javascript.intl` therefore carries no segmentation data, as JSD-0027
  requires of every Intl surface. `CldrTableGenerator` already reads the verified UCD archive for
  Soft_Dotted, and it writes two more tables under rule N28:
  - a binary table of six properties over the whole code space, as runs, each run naming one of the
    distinct combinations. The properties are Grapheme_Cluster_Break, Word_Break, Sentence_Break,
    Extended_Pictographic, Indic_Conjunct_Break, and whether a code point is Ideographic or Hiragana;
  - a text table naming each property's values in the order the codes number them. The profile reads
    the codes by name, so the generator's order and the profile's need not agree.
- **The size**: 503,669 bytes of table data on 2026-10-04, 19,926 more, in an assembly of 515,072
  bytes. That is still under JSD-0043's provisional 512 KiB bound on the data, with 20,619 bytes of
  headroom. No budget is set.

## 3. What is word-like

The draft leaves `isWordLike` to the implementation, and recommends that spaces and punctuation alone
are not word-like, as ICU's WORD_NONE marks them. A word segment is word-like where it holds:

- a code point whose Word_Break is ALetter, Hebrew_Letter, Numeric or Katakana;
- or an Ideographic or Hiragana code point.

That is the set ICU's word statuses mark as letters, numbers, kana and ideographs. `_` alone,
emoji, spaces and punctuation are not word-like, and `a_b` is.

## 4. Validation

- **UAX #29's own conformance files**, run by three slice-compiler checks through `Intl.Segmenter`:
  all 766 lines of `GraphemeBreakTest.txt`, all 1,944 of `WordBreakTest.txt` and all 512 of
  `SentenceBreakTest.txt` segment at exactly the boundaries they mark.
- **The retained segment dataset**
  ([`src/tests/cldr/segments/`](../../../tests/cldr/segments/README.md)) holds 118 lines against Node
  22.22.0's ICU 77.1. It segments 17 texts by each granularity under `en` and `de`: prose with
  abbreviations, numbers, quotes and line ends, emoji and flags, conjuncts and Hangul, Japanese,
  Hebrew and lone surrogates. It also answers `containing`, the iterator and the errors. **Every
  line agrees** but 2, in one group named in `divergences.txt`. ICU joins Japanese ideographs into
  words by a dictionary, where UAX #29's default rules make each ideograph a word. ICU 77.1
  implements Unicode 16.0's rules, and the conformance files hold the profile to 17.0's.
- **test262**, against the pinned suite: `test/intl402/Segmenter` passes 154 of 158 variants, from
  4. The 4 failing are two files, and both need the Serbian locale: `locales-valid.js` and
  `supportedLocalesOf/locales-specific.js`.

## 5. What is not done

- **Dictionary segmentation** of Chinese, Japanese, Thai, Lao, Khmer and Burmese words. It needs
  dictionaries far larger than the provisional bound, and the draft does not require it.
- **CLDR's sentence suppressions** (`Mr.`, `etc.`), which ICU applies only under `-u-ss-standard`. ICU
  77.1 breaks after them by default too, as the dataset shows.
- **Locales**: the section 5 list, as for the rest of F7.
- **`DisplayNames` and `DurationFormat`** are the rest of slice I4. `DisplayNames` waits on the size
  budget, which this slice brings closer.

## 6. What would falsify this

- A line of a pinned UAX #29 conformance file that the segmenter breaks otherwise.
- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name.
- A segment data object whose properties are not the draft's, in its order.
- A `containing` that answers a segment not holding the code unit asked for.
