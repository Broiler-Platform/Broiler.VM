<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0052 - Intl.DisplayNames: CLDR's locale display names, composed as ICU composes them

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The code this record describes is in
the tree. It is the seventh and last part of slice I4 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7, `Intl.DisplayNames`, after `Locale`,
`PluralRules`, `ListFormat`, `RelativeTimeFormat`, `Segmenter` and `DurationFormat`
([JSD-0046](0046-intl-locale.md) to [JSD-0051](0051-intl-durationformat.md)). It waited for the size
budget, JSD-0027's owner decision (c), which the repository owner set on 2026-10-05 at 768 KiB.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the tenth implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

**The text followed** is ECMA-402's current draft, the 14th edition (2027), as published at
`https://tc39.es/ecma402/` on 2026-10-04. Clause numbers in the code and below are that draft's. The
profile pins no ECMA-402 edition. The pinned test262 checkout is what scores it.

---

## 1. What is built

- **`Intl.DisplayNames`, whole, as the draft states it** (s12). It has:
  - the constructor, which requires `new` and an options object, and reads `localeMatcher`, `style`,
    `type`, `fallback` and `languageDisplay` through GetOptionsObject; `type` is required;
  - `of`, `resolvedOptions` and `supportedLocalesOf`.
- **All six types are named**: `language`, `region`, `script`, `currency`, `calendar` and
  `dateTimeField`.
- **A code is checked and its case regularized first** (s12.5.1 CanonicalCodeForDisplayNames), so
  `us`, `latn` and `GREGORY` are named as `US`, `Latn` and `gregory` are. A language code is
  canonicalized, so `iw` is named as `he` is.
- **A language is composed as ICU's LocaleDisplayNames composes it.**
  - Under the `dialect` display, the whole code's own name is taken where CLDR has one, so `en-GB`
    is "British English". Only the whole code's: `zh-Hant-TW` has no name of its own, and it is
    "Chinese (Traditional, Taiwan)", not composed from `zh-Hant`'s "Traditional Chinese".
  - The language's name follows, then the script, region and variants it did not cover, joined by
    CLDR's locale separator inside its locale pattern.
  - An unnamed part is written as its code under the `code` fallback, "xyz (Germany)" as ICU writes
    it. Under the `none` fallback it makes the whole name undefined.
- **The short and narrow styles take CLDR's `-alt-short` forms** for languages and regions, "UK
  English" and "UK", as ICU does. Scripts, calendars and currencies have one name in every style. A
  date-time field's styles are CLDR's `dateFields` widths.

## 2. The data

- **The archive grew by a fifth CLDR package**, `cldr-localenames-full` at 48.2.0, under N27: its
  tarball, retrieved twice, byte-identical, and checked against the registry's sha512 integrity. From
  it come `languages.json`, `territories.json`, `scripts.json`, `variants.json` and
  `localeDisplayNames.json` for `de` and `en`, and its `package.json`. Currency names come from slice
  I2's archive, and date-time field names from slice I3's.
- **Under rule N28 one table is new**, `DisplayNames`: per language, every language, region, script
  and variant name, the locale pattern and separator, each calendar named by its BCP 47 key through
  CLDR's alias, and each date-time field in three widths.
- **The data is 572,024 bytes on 2026-10-05**, 68,227 more, in an assembly of 583,680 bytes, 214,408
  under the owner's budget of 786,432 bytes (768 KiB). Rule N28's size test holds the budget, which
  replaces JSD-0043's provisional 512 KiB bound.

## 3. Validation

- **The retained display-name dataset**
  ([`src/tests/cldr/displaynames/`](../../../tests/cldr/displaynames/README.md)) holds 823 lines
  against Node 22.22.0's ICU 77.1. 759 agree, among them the dialect and standard displays, the short
  forms, the locale pattern, the currencies and the date-time fields. 64 differ, in four groups, each
  named in `divergences.txt`:
  - ECMA-402 regularizes a code's case before naming it, and V8 hands ICU the code as written (30
    lines);
  - CLDR 48 renamed English's `iso8601` and German's tabular and Umm al-Qura Hijri calendars, where
    Node carries CLDR 47 (9);
  - ICU joins `sl-rozaj-biske`'s variants into one unnamed key and names `und` "root", where CLDR's
    data names both (24);
  - an empty script code is a RangeError by s12.5.1, and V8 returns the empty string (1).
- **test262**, against the pinned suite: `test/intl402/DisplayNames` passes all 114 scored variants,
  from 8. `test/intl402/Intl` passes all 130, from 126: the 4 that failed name the calendars and
  currencies `supportedValuesOf` lists through `DisplayNames`.
- **A slice-compiler check** holds the dataset as the duration check holds the durations.

## 4. What is not done

- **Locales**: the section 5 list, as for the rest of F7. Names exist for `de` and `en` only, and a
  `DisplayNames` of another locale resolves to the default locale and names in English.
- **CLDR's other alternative forms**, `-alt-variant`, `-alt-long`, `-alt-official` and the like, are
  carried in the table but never chosen, and `-alt-menu` is not carried. ECMA-402 has no option that
  selects them.
- **Slice I4's time-zone data and the `tr`, `az` and `lt` case tailorings** JSD-0027 section 7 lists
  with the constructors are not built. Time zones are phase F8's.

## 5. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the draft's text or the archived data does not bear out.
- An option read in another order than the constructor's, or read twice.
- A code accepted that CanonicalCodeForDisplayNames refuses, or refused that it accepts.
- A name that is not in the archived files, or a composed name that does not follow their locale
  pattern and separator.
- The generated data past the owner's budget, which N28's size test would report.
