# Area: locale data and Intl algorithms

**Delivered by:** [SJL-9](../milestones/sjl-9-locale-data-and-intl.md), and
[SJL-17](../milestones/sjl-17-collation.md) for collation. **Libraries:** `Broiler.ECMAScript.Intl`
(algorithms), over locale data that Broiler.VM's generator produces from its pins, both moved to the
Broiler.JS repository, and that each engine embeds (decisions [SJL-D-22](../decisions.md#sjl-d-22)
(a) and [SJL-D-6](../decisions.md#sjl-d-6) (c)). **Status:** design. Observations marked *(reading)*
are to be confirmed by vectors and reference datasets.

---

## 1. Scope and sub-area order

The order is chosen so that every later sub-area builds on an earlier one, and each can be refused on
its own:

1. **Locale identifiers.** Parsing, canonicalisation with the full CLDR alias data, likely
   subtags, and negotiation by lookup. Everything else depends on them.
2. **Small formatters:**
   - plural rules, with operands taken from the formatted decimal text, and plural ranges;
   - list formatting;
   - relative-time formatting;
   - display names.
3. **Number formatting:**
   - the Intl mathematical value, taken from `Number::toString` digits;
   - digit options;
   - patterns, compact notation, currency and units;
   - ranges and parts.
4. **Date-time formatting:**
   - the pattern generator;
   - the formatter;
   - intervals;
   - day periods and hour cycles;
   - zone names with metazone history.
5. **Duration formatting.**

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D3.)* The order is revised, because this family's plural rules, relative time and duration call its number
formatter, and duration also needs lists:
1. **(a)** locale identifiers, likely subtags, negotiation, Locale info and `supportedValuesOf`;
2. **(b)** the Intl mathematical value, digit options and FormatNumericToString, then plural rules and plural ranges;
3. **(c)** number formatting, then list, relative-time and display-name formatting;
4. **(d)** date-time formatting, with zone names and Intl over Temporal;
5. **(e)** duration formatting.

The list above is left as written: its second item, "**Small formatters**", came before "**Number formatting**",
whose first items were the Intl mathematical value and digit options. Each sub-area can still be refused on its own.

**Collation** comes last, in its own milestone, [SJL-17](../milestones/sjl-17-collation.md), after
SJL-9 (decision [SJL-D-24](../decisions.md#sjl-d-24) (c)). Its design is section 3.4.

**Segmentation** is Unicode data and algorithms, and belongs to SJL-7.

**Stays in the engines:**

- the Intl objects, prototypes and `NewTarget` handling;
- **option reading in specification order**;
- the legacy constructor chaining (`ChainDateTimeFormat`, `ChainNumberFormat`) and unwrapping;
- bound `format` and `compare` functions and their caching;
- parts arrays;
- default-locale and available-locale policy, as providers (decision
  [SJL-D-19](../decisions.md#sjl-d-19));
- surface and composition gating in Broiler.VM.

---

## 2. Current state

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| Locale tags | `JsLocaleTag`: parse, canonicalise, aliases, likely subtags; engine-free | Regular-expression validation and partial hand-written alias tables in `JSIntl`; likely subtags from the CLDR package; `JSIntlLocale` re-parses on every getter |
| Negotiation | `JsRealm.Intl`: lookup; **unknown locales resolve deterministically** to the default | Reports the requested host locale, then falls back to English data or a GMT offset |
| Default locale | Fixed `en-US` | The host's `CultureInfo.CurrentCulture` |
| Available locales | Its data: `de`, `de-DE`, `en`, `en-US` | The host's cultures; nearly empty under invariant globalization |
| Intl mathematical value | `JsDecimal`, from `Number::toString` digits | `ExactRational` rounds the **binary** value of the double, so `1.15` with one fraction digit gives `"1.1"` where Broiler.VM gives `"1.2"` *(reading)*. Current ECMA-402 takes the `Number::toString` value. |
| Number formatting | `JsNumberData` (UTS 35 patterns), `JsNumberFormatter` (compact, currency spacing, units, ranges); charged per digit group | `JSIntlNumberFormat`: hand-written compact tables for a few locales; separators from .NET cultures; `trailingZeroDisplay` stored but not applied; `currencyDisplay: "name"` renders the symbol; options re-read from the live object at format time |
| Plural rules | `JsPluralRule`: operands from the decimal text, plural ranges; data for its locales | The CLDR package's rules over `double`; **`selectRange` always returns `"other"`**; digit options ignored |
| Date-time formatting | A port of ICU's pattern generator (`JsDatePatternGenerator`), formatter (`JsDateTimeFormatter`), interval formatter (`JsDateIntervalFormat`) and zone naming (`JsZoneNames`); CLDR calendar data for its locales and calendars | `JSIntlDateTimeFormatEngine`, whose own header says it covers English: hand-written English names, numeric order from .NET's `ShortDatePattern`, a few interval patterns, user getters run twice and out of order in `ToDateTimeOptions` *(reading)* |
| Relative time, lists | CLDR data for its locales | The CLDR package for its languages; relative time builds a new number format per call and drops `numberingSystem` |
| Display names | Full, from CLDR | **A stub**: `of` returns the canonical code |
| Duration format | Full, including the `fractional` style | No `fractional` style; `":"` hard-coded |
| Collation | UCA over CLDR root and a few tailorings, with `numeric` and `caseFirst` | The host's `CompareInfo`; `numeric` and `caseFirst` are resolved but **not applied** |
| Data and pins | CLDR 48.2.0, pinned (`src/tests/cldr/pins/cldr.pin` with npm package integrity values and per-file hashes), generated by `CldrTableGenerator` into the optional `.Intl` assembly; rules N27 and N28 with a size budget | CLDR 48.2.0 through `Broiler.Unicode.Cldr.LocaleData` (its version only in internal classes; downloaded inputs without hashes); host ICU for the rest |
| Metering | Throughout | None |

**Broiler.VM's reference datasets.** These are host-function-free programs with answers taken from
an independent implementation and with divergences declared by ECMA-402 clause. They live in
`src/tests/cldr/{dates, displaynames, durations, lists, locales, numbers, orderings, plurals,
relativetimes, segments, timezones}/` and are **the acceptance data for both engines**.

---

## 3. The target

### 3.1 The data provider

```csharp
namespace Broiler.ECMAScript.Intl;

public interface IIntlDataSet
{
    string CldrVersion { get; }
    ReadOnlySpan<byte> Table(IntlTable table);   // the shape of Broiler.VM's IJsIntlData today
}
```

Broiler.VM's existing `IJsIntlData` in the Format assembly (a version and one byte span per table)
is already engine-neutral. It is the natural contract. The shared library adopts its shape, and the
Format interface is replaced or adapted.

**The data** is Broiler.VM's generator's output: the generator moves with its pins to the Broiler.JS
repository and generates the agreed locales (decision [SJL-D-22](../decisions.md#sjl-d-22) (a)). Each
engine embeds the tables and hands them to the library through `IIntlDataSet`
([SJL-D-6](../decisions.md#sjl-d-6) (c)). `Broiler.Unicode.Cldr.LocaleData` is not extended, which
was option (b), so the verdict Broiler.VM's JSD-0027 §4 gave it, "Candidate input, not accepted",
stands.

*(Read 2026-10-26, at SJL-9.1, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D8, which revises nothing.)* **The contract has two layers.** `IIntlDataSet`, above, carries a version and
one span per `IntlTable`; the enum keeps `JsIntlTable`'s numbers, with 7, 23 and 24 retired and not reused, and
appends values only for tables Broiler.VM's data never carries, and each engine answers an empty span for a table it
lacks. The library decodes it into a question-level `ILocaleData`, which Broiler.JS's host provider also implements,
and which `CompositeLocaleData` orders: the shared data, then an optional secondary, then the parent and root. **The
Format interface is adapted, not replaced**: `IJsIntlData` and `JsIntlTable` stay public in Broiler.VM's Format
assembly, and Broiler.VM adapts them internally.

### 3.2 The algorithms

The library is organised by sub-area: `LocaleId` and `LocaleNegotiation`, `IntlDecimal` and
`NumberFormatter`, `PluralEngine`, `ListFormatter`, `RelativeTimeFormatter`, `DisplayNamesCore`,
`DurationFormatCore`, `DateTimePatternGenerator`, `DateTimeFormatter`, `DateIntervalFormatter`,
`ZoneNamer`, and, at SJL-17, `UcaCollator`.

Each takes **resolved options as records**, never an options object. It reads data through
`IIntlDataSet`, charges through the meter, and returns strings or parts records.

### 3.3 Broiler.JS's host fallback

For locales the shared data does not carry, Broiler.JS keeps an **optional provider** backed by
host cultures (decision `SJL-D-22` (c)). The provider answers the questions the shared algorithms
ask. It is never a second implementation of the algorithms. Broiler.VM never registers one.

**The provider never throws for want of host data.** Under `InvariantGlobalization`, which Native
AOT applications commonly set, the host has only the invariant culture, and
`CultureInfo.GetCultureInfo` throws for a named one. The provider then answers that the locale is
unavailable, and negotiation falls back as it does for any locale the data lacks.

*(Read 2026-10-26, at SJL-9.1, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D6 and D7, which revise nothing.)*
- **What the provider answers.** Only what .NET exposes exactly: number symbols and native digits, month, day and era
  names, AM and PM, and the first day of the week, for the locales the shared data lacks. Every other question
  answers "no data", so the algorithm uses the parent or root.
- **The default-locale and available-locale providers** keep each engine's policy (SJL-D-19). In both engines the
  available locales always include the default locale and the less narrow tags the same clause requires, as the
  pinned text requires: under invariant globalization Broiler.JS's are `en-US` and `en`, where they are none today
  (row L44). And every reader of DefaultLocale in an engine reads the one default-locale provider, its `toLocale*`
  methods and TransformCase with no locale included (row L35).

### 3.4 Collation (SJL-17)

| | Broiler.VM | Broiler.JS |
|---|---|---|
| Today | UCA over CLDR's root collation and a few tailorings for its locales (`JsCollator`, `JsCollationData`, JSD-0043), with `numeric` and `caseFirst` | The host's `CompareInfo`, with every host locale; `numeric` and `caseFirst` resolved but **not applied**; platform normalization first |
| After SJL-17 | `UcaCollator` from the library, over the collation tables its `.Intl` assembly embeds | `UcaCollator` for the locales the shared data covers; the optional host provider for the rest |

The library takes resolved options as a record and normalizes through the Unicode owner (SJL-7).
The collation data comes from the CLDR generator in the Broiler.JS repository, embedded by each
engine ([SJL-D-6](../decisions.md#sjl-d-6) (c)). The oracles are the UCA test files Broiler.VM pins
and its retained `orderings` reference dataset.

*(Noted 2026-10-26, at SJL-9.1, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md).)*
The table's "Today" cell for Broiler.JS ends "platform normalization first", which is stale since SJL-7.5: Broiler.JS's
collator decomposes both strings through Broiler.Unicode's normalization, not the platform's, before the host's
collation runs. The cell is left as written.

---

## 4. Deletion lists

**Broiler.VM:**

- `JsLocaleTag`;
- the decoders of `JsIntlTables` (`Likely`, `Alias`, `LanguageAliases`, `TryType`, `ReadAliases`,
  `ReadExtensions`);
- `JsDecimal` and `JsRawDecimal`;
- `JsNumberData` and `JsNumberFormatter`;
- `JsPluralRule`;
- `JsDatePatternGenerator`, `JsDateTimeFormatter`, `JsDateIntervalFormat` and `JsZoneNames`;
- `JsDateData` and `JsLocaleInfo`;
- the algorithm cores of `JsRealm.Intl`, `.NumberFormat`, `.PluralRules`, `.ListFormat`,
  `.RelativeTimeFormat`, `.DisplayNames`, `.DurationFormat` and `.DateTimeFormat`;
- **moved:** the generator of `JsCldrTables`, to the Broiler.JS repository, which starts from a
  copy of the pins. The generated table stays in the `.Intl` assembly, regenerated from Broiler.VM's
  pins, which rule `I4` holds equal to the home's, and held byte for byte by rule N28
  ([SJL-D-6](../decisions.md#sjl-d-6) (c));
- **at SJL-17:** `JsCollator` and `JsCollationData`.

**Broiler.JS:**

- in `JSIntl`: the locale-tag validation and canonicalisation, the hand-written alias tables,
  negotiation, the number-format engine (`ExactRational`, compact and digit tables), the plural,
  list, relative-time and display-name internals, and the duration-format internals;
- `JSIntlDateTimeFormatEngine`'s English engine, interval patterns and zone helpers;
- the `JSIntlLocale` parse helpers;
- its uses of `Broiler.Unicode.Cldr.LocaleData` for Intl data;
- **at SJL-17:** the `CompareInfo` path for the locales the shared data covers.

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D24.)* Both lists are extended. Every deleted type and member of Broiler.VM's is registered in rule I5.
- **Broiler.VM, plus:** the `JsRealm.Locale` cores; every `JsIntlTables` decoder and its lazies (numbers, dates, locale
  information, zone names), beyond those named above; `SupportedValuesOf`'s core; the helper types `JsDecimalKind`,
  `JsRawFormat`, `JsNumberPart`, `JsCurrencyNames`, `JsAffixKind` and `JsAffixToken`, `JsNumberPattern`,
  `JsPluralOperands`, `JsDatePart`, `JsDateField`, `JsZoneKind`, `JsLocalTime`, `JsZoneNaming` and `JsDayPeriodRule`;
  and, on the test side, `CldrTableGenerator.cs`, `UnicodeDatabase.cs` and `CldrPin.cs`. **Minus:** `NewCollator`
  and `CollatorKeyData` in `JsRealm.Intl`, which are SJL-17's, and `JsRealm.Segmenter.cs`, which is SJL-7's. The
  reading of DurationFormat's argument, ECMA-402's ToDurationRecord included, stays in the adapter.
- **Broiler.JS, plus:** `JSIntlDefaultNumberingSystems.g.cs` and its script; and `JSIntlDateTimeFormat`'s members no
  caller reaches (the constructor from a `CultureInfo`, `Format(DateTimeOffset, JSObject)`, `FormatEnglishHour`,
  `FormatDayPeriod` and the `locale` field). Its public Intl types stay as the engine's Intl objects, with every public
  member, the public static `Get(CultureInfo)` and `ApplyAllDefaults` among them (SJL-D-25, disposition (a)).

The lists read as above, Broiler.VM's ending at "**at SJL-17:** `JsCollator` and `JsCollationData`" and Broiler.JS's
at "**at SJL-17:** the `CompareInfo` path for the locales the shared data covers".

---

## 5. Corrections Broiler.JS should expect

Each correction is to be confirmed and listed:

- `selectRange`;
- `DisplayNames.of`;
- rounding of the Intl mathematical value;
- `trailingZeroDisplay`;
- `currencyDisplay: "name"`;
- non-English month and day names;
- the date-time `atTime` joins and the narrow no-break space before day periods;
- the order and repetition of option reads;
- the `timeZone` option ignored in one parts path;
- `numberingSystem` in relative time;
- the identity of the bound `format` function;
- `numeric` and `caseFirst` in collation, at SJL-17.

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D25.)* **Reconciled with SJL-9.5's list, which is the correspondence's column "Correction of Broiler.JS",
row by row** (the milestone file names the rows). Read against the list above:
- `selectRange` is two rows: both ends formatting alike, a correction (P5a), and every other range, an
  implementation-defined adoption (P5b);
- `DisplayNames.of` is two rows: the code's case and validation, a correction (S9), and the names, a data adoption
  (S8);
- the `atTime` joins and the narrow no-break space are CLDR data (C5, row T12), not corrections;
- the order and repetition of option reads hold for Date's `toLocale*`, NumberFormat after construction, PluralRules
  and DurationFormat's argument, and not for any constructor's option order;
- "the `timeZone` option ignored in one parts path" was fixed on 2026-10-16 (row T28), and leaves the list;
- the list's other items are confirmed, and the correspondence adds corrections in every sub-area.

The list read as above, from "`selectRange`" to "`numeric` and `caseFirst` in collation, at SJL-17."

---

## 6. Risks

| Risk | Mitigation |
|---|---|
| Broiler.JS loses locales it serves today through host cultures | The optional host provider (`SJL-D-22` (c)), plus data growth by decision |
| Broiler.VM's data size grows with locales | A size budget in every data decision, restated by citation in Broiler.VM's bundle |
| Data-format coupling: the shared library reads Broiler.VM's table format | The format is documented and versioned by the data home. A format change breaks the Intl library's provider contract, so it is a major version of the library, and each engine regenerates its embedded tables in the change that adopts it. |
