# Responsibility inventory

**Date:** 2026-10-11. **Revisions read:**

| Repository | Revision |
|---|---|
| Broiler.VM | `88d8b51d` |
| Broiler.JS | `44a214dc` |
| Broiler.Unicode | `2affa78f` |
| Broiler.RegEx | `c1422f4d` |
| Broiler.DateTime | `8de12cc3` |

**Status:** survey. Each row is re-derived at named commits at SJL-0.1, and that retained copy is
the one the gates use. Member names are given so that a deletion can be checked. **No line numbers
are given:** they go stale, and review documents may not cite them.

**Coupling classes:**

| Class | Meaning |
|---|---|
| **pure** | A static function of plain CLR values |
| **light** | Separable, but sits inside an engine class or performs coercion, charging or errors at its edges |
| **heavy** | Interleaved with the object model, or with property access a script can observe |
| **host** | Depends on host globalization, the host's time zone or the runtime's Unicode data |

**Dispositions:**

| Disposition | Meaning |
|---|---|
| **shared** | One implementation, both engines |
| **adoption** | One engine's implementation becomes shared and the other adopts it |
| **parameter** | Engine policy, passed to a shared operation |
| **retained** | Stays in each engine ([areas/retained-in-engines.md](areas/retained-in-engines.md)) |

---

## 1. Duplicated between the engines

### Number conversion (SJL-3)

| Responsibility | Broiler.VM | Broiler.JS | Coupling (VM / JS) | Disposition |
|---|---|---|---|---|
| `Number::toString` radix 10 | `JsNumberFormat.ToJsString`, `Decompose`, `Place`; `JsParser.NumberText` | `JSNumber.ToECMAString`, `RoundTrippable`; the `JSValue.NumberToECMAString` delegate | pure / pure | shared |
| Radix 2–36 | `JsNumberFormat.ToRadixString`, `RoundUpRadix` | `JSNumberPrototype.DecimalToBase`, `DecimalToArbitrarySystem` (public) | pure / pure | shared |
| `toFixed`, `toExponential`, `toPrecision` | `JsRealm.Number` `Number*Text` statics | `JSNumberPrototype` kernels | light / light | shared |
| `StringToNumber` | `JsNumberFormat.ToNumber`, `Trim`, `IsDecimalLiteral`, `ParseRadix` | `NumberParser.CoerceToNumber` (public), `ParseCore`, `RefineEstimate` | pure / pure | shared |
| `parseInt`, `parseFloat` | `JsNumberFormat.ParseInt`, `ParseFloat` | `NumberParser.ParseInt`, `ParseFloat`; two wrapper pairs | pure / pure | shared |
| Literal values | `SliceTokenizer.ReadNumericLiteral` values; `JsParser.TryBigIntValue` | `NumberCoercion`; `JSBigInt.TryParseBigIntLiteral`; `FastCompiler.CreateClass.BigIntLiteralToValue` | light / light | shared |
| Array-index keys | `JsObject.IsArrayIndex` | `NumberParser.TryGetArrayIndex` | pure / pure | shared |
| `StrWhiteSpaceChar` | `JsNumberFormat.IsWhiteSpace` (runtime `Zs`) | `NumberParser.IsWhiteSpaceOrLineTerminator`, `trimCharacters`, .NET `char.IsWhiteSpace` paths | host / host | shared (pinned table) |

### Numerics (SJL-4)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Integer conversions | `JsValue.ToInt32`, `ToUint32`, `ToInteger` | `JSValue.ToUint32`, `IntValue`; `JSNumericOperators` (public) | pure / pure | shared |
| Clamping, BigInt64 bits, `ToIndex`, `ToLength` | `JsElements.Clamp`, `Low64`, `Narrow`; `BinaryToIndex` | inline clamp; masks; `DataView.ToIndex`, `JSBigInt.ToBitsIndex`, `JSJSON.ToLength` | pure / pure | shared |
| Float16 | `JsFloat16` | inline `(Half)` casts | pure / pure | shared (NaN policy is a parameter) |
| Math kernels | `JsRealm.Math` helpers | `JSMath`; `JSValue.Power`; `**` emitted as `Math.Pow` | light / light | shared where custom |
| Unsigned rounding, exact rationals | `JsTemporalCore` rounding; `JsDecimal.JsUnsignedRounding` | four copies across Temporal and Intl | pure / light | shared |
| BigInt kernel | `JsBigInt` (metered, width ceiling) | `JSBigInt` (unbounded, unmetered, culture text) | pure / light | shared (width is a parameter) |

### Text (SJL-5)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Well-formedness | `StringWellFormed` | `IsWellFormedUtf16`, `ToWellFormedUtf16` | light / light | shared |
| Padding | `StringPadTo` (ceiling) | `StringPad` (no limit) | light / light | shared (ceiling is a parameter) |
| Substitution templates | `StringExpandDollars`; copies in `RegExpReplaceText`, `RegExpSubstitute` | `JSString.GetSubstitution`; initializer copy | light / light | shared parse; engine evaluation |
| URI codecs | `GlobalUri*`, `GlobalEscapeText`, `GlobalUnescapeText` | `UriHelper` | light / pure | shared |
| Base64 and hex | `Base64Encode`, `Base64Decode`, `Base64DecodeHex` | `JSUInt8Array` base64 and hex cores | light / light–heavy | shared |
| JSON lexing and quoting | `JsRealm.Json` reader; `JsonQuote`; `JsJsonModule` reader | `System.Text.Json` deserialisation; `JSJSON.QuoteString` | heavy / heavy (drivers) | shared reader and quote; drivers retained |

### Date (SJL-6)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Time arithmetic | `JsRealm.Date` `Date*` functions | `JSDateMath` | pure / pure | shared |
| Civil-day helpers | `JsTemporalCore`, `JsTimeZones`, `JsLocalTime.From` | copies in every Temporal type, two calendar classes, `TemporalIsoString` | pure / pure | shared |
| Local time | UTC (policy) | `TimeZoneInfo.Local` | pure / host | provider (parameter) |
| Date parsing | `DateParseText`, `DateParseTextual` | `DateParser` (public), `TryParseExtendedIso` | light / host | shared normative grammar; dialects |
| Date renderings | `DateTo*Text` | `JSDatePrototype.Formatters` | pure / host | shared |

### Unicode (SJL-7)

| Responsibility | Broiler.VM | Broiler.JS / packages | Coupling | Disposition |
|---|---|---|---|---|
| `\p` properties | `JsUnicodeProperties` (exact) | `Broiler.Unicode.Properties` (loose); the translator's lookups | pure / pure | shared, owner Broiler.Unicode, exact API |
| Lexical classes | `JsUnicodeLexical` | `CharExtensions`; several whitespace lists | pure / host | shared |
| Case folding | `JsUnicodeCaseFolding` | Broiler.Regex `CaseFoldingData` (identical); the translator's .NET casing | pure / host | shared |
| Case mapping | `JsUnicodeCasing`; `JsRealm.String` language casing | `JSStringSpecialCasing` with host fallbacks; `CultureInfo` | light / host | shared |
| Normalization | `JsUnicodeNormalization`; `JsRealm.String` passes | host `string.Normalize` | light / host | shared |
| Properties of strings | `JsUnicodeStringProperties` | `Broiler.Unicode.Emoji.StringProperties` (identical data) | pure / pure | shared |
| Segmentation | `JsSegmenter` and its tables | `StringInfo`, heuristics, whole-string sentences | light / host | adoption (Broiler.VM's algorithm) |

### Time zones and calendars (SJL-8)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| tz data and format | `JsTimeZones`, `JsTzdbTables` (2026e, pinned) | `Tz/*`, `tzdata.bin` (2025b, host-built) | pure / pure | shared (one dataset, one policy) |
| Zone algorithms | `JsTemporal.Zones` | `JSTemporalZonedDateTime` helpers | light / heavy | shared |
| Calendar tables | `JsCalendars` table parts, `JsCalendarTables` (ICU4X) | .NET calendars; the CLDR package's calendars | pure / host | shared (pinned sources) |

### Locale data and Intl (SJL-9)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Locale tags, likely subtags | `JsLocaleTag`; `JsIntlTables` | `JSIntl` validation and aliases; `CldrLikelySubtags` | pure / light | shared |
| Negotiation | `JsRealm.Intl` | `JSIntl` | light / host | shared; defaults are providers |
| Intl mathematical value, number formatting | `JsDecimal`, `JsNumberData`, `JsNumberFormatter` | `JSIntlNumberFormat` | light / host | shared |
| Plural rules | `JsPluralRule` | the CLDR package's rules | pure / pure | shared |
| Date-time formatting, intervals, zone names | `JsDatePatternGenerator`, `JsDateTimeFormatter`, `JsDateIntervalFormat`, `JsZoneNames`, `JsDateData` | `JSIntlDateTimeFormatEngine` (English) | light / host | shared |
| List, relative time, display names, duration | the `JsRealm.*` cores | `JSIntl*` classes; display names are a stub | light / light | shared |
| Collation | `JsCollator`, `JsCollationData` | host `CompareInfo` | light / host | shared later, at SJL-17 (`SJL-D-24` (c)) |
| CLDR data | `JsCldrTables` (generated, pinned) | `Broiler.Unicode.Cldr.LocaleData`; hand-written subsets; host | pure / host | shared: Broiler.VM's generator and pins in the Broiler.JS repository, tables embedded by each engine (`SJL-D-22` (a), `SJL-D-6` (c)) |

### Temporal (SJL-10)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Records, limits, balancing | `JsTemporalCore` | per-type copies | pure / light | shared |
| Durations, rounding, totals | `JsTemporal.Durations` | `JSTemporalDuration*`, `ZonedDateTime.Rounding`, `PlainDate` rounding | light / heavy | shared |
| Differences | realm `Difference*` cores | per-type `Difference*` | light / heavy | shared |
| Grammar | `JsTemporalParser` | eight per-type parsers; `TemporalIsoString` | light / heavy | shared (Broiler.VM's grammar) |
| Option rules | `JsTemporal.Options` | about two dozen readers | heavy / heavy | rules shared; reading retained |
| Calendars | `JsCalendars` formulas, `JsTemporal.CalendarNonIso` | `TemporalNonIso`, `JSTemporalCalendar` | pure / light | shared |
| Serialization | `JsTemporalCore`, `JsTemporal.Conversions` | per-type formatters | pure / pure | shared |

### Regular expressions (SJL-11)

| Responsibility | Broiler.VM | Broiler.JS / package | Coupling | Disposition |
|---|---|---|---|---|
| Engine | `JsRegExpMatcher` (metered, bounded) | .NET translator plus `Broiler.Regex` (unmetered, loose) | light / heavy | shared, owner Broiler.Regex, fixed to Broiler.VM's requirements (`SJL-D-14` (a)) |
| Flags, `escape`, `source` | `JsRealm.RegExp` helpers | `JSRegExp` helpers; scanner; validator | pure / pure | shared |
| Literal validation | the compiler, with the same parser | .NET compilation in the scanner | light / host | shared |

### Front end (SJL-12 to SJL-15)

| Responsibility | Broiler.VM | Broiler.JS | Coupling | Disposition |
|---|---|---|---|---|
| Tokenizer | `SliceTokenizer` | `FastScanner`, `FastTokenStream` | pure / host | shared |
| Parser and tree | `JsParser`/`JsSyntax` (wide); `SliceParser`/`SliceSyntax` (slice) | `FastParser`; `Ast` | pure / heavy (ambient) | shared |
| Diagnostics | `SliceSourceDiagnosticCode`, registry | `FastParseException` messages | light / light | shared codes; rendering retained |
| Static semantics and early errors | `SliceStaticSemantics`; the wide binder in `JsCompiler` | `FastScope`, `FastFunctionScope`, `FreeNameScan`, `SyntaxValidation`, `FastCompiler.EarlyErrors`, `DirectEvalSupport` | heavy / heavy | shared |

---

## 2. Duplicated inside one engine

The programme removes these in the same change as the cross-engine copy.

| Engine | Duplicate | Cause | Removed at |
|---|---|---|---|
| Broiler.VM | `JsParser.NumberText` beside `JsNumberFormat.ToJsString` | The compiler may not reference the runtime profile | SJL-3 |
| Broiler.VM | `JsParser.TryBigIntValue` beside the runtime's BigInt parsing | Same | SJL-3 |
| Broiler.VM | `JsJsonModule`'s reader beside `JsRealm.Json`'s reader | Same | SJL-5 |
| Broiler.VM | The regular-expression matcher and the Unicode lookups placed in Format; the tables stay there, regenerated, under `SJL-D-6` (c) | The compiler and the runtime both need them | SJL-7, SJL-11 |
| Broiler.VM | Gregorian day arithmetic in `JsRealm.Date`, `JsTemporalCore`, `JsTimeZones` and `JsLocalTime.From` | Written per area | SJL-6 |
| Broiler.VM | Unsigned rounding in `JsTemporalCore`, and in `JsDecimal` with `JsNumberFormatter` | Written per area | SJL-4 |
| Broiler.VM | Template expansion in `StringExpandDollars`, `RegExpReplaceText`, `RegExpSubstitute` | Written per call site | SJL-5 |
| Broiler.JS | Number conversion in `NumberCoercion`, `NumberParser` and `JSNumber` | Layering: the runtime may not reference the built-ins, so a delegate seam | SJL-3 |
| Broiler.JS | `parseInt` and `parseFloat` wrapper pairs in `JSGlobal` and `JSNumberStatic` | — | SJL-3 |
| Broiler.JS | Whitespace sets in `NumberParser`, `trimCharacters`, `char.IsWhiteSpace`, `StringSpan.Trim`, `FastScanner`, `JSRegExp.Escape` | — | SJL-3, SJL-7 |
| Broiler.JS | BigInt literal parsing in `JSBigInt` and `FastCompiler.CreateClass` | — | SJL-3 |
| Broiler.JS | Leap year, civil days, ISO validity, unsigned rounding, `ReadOverflow` and string parsers, once per Temporal type | Written per type | SJL-6, SJL-10 |
| Broiler.JS | Regular-expression flag parsing in several places; two regular-expression engines | Incremental adoption of Broiler.Regex | SJL-11 |
| Broiler.JS | Early errors across the parser, `SyntaxValidation`, `FastCompiler.EarlyErrors`, `Visit*` and `DirectEvalSupport` | Historical | SJL-15 |

---

## 3. Present in one engine only, and retained in both

See [areas/retained-in-engines.md](areas/retained-in-engines.md), sections 1–5.

---

## 4. Existing component libraries

| Component | Published | Used by Broiler.JS | Used by Broiler.VM | Readiness gaps the programme closes |
|---|---|---|---|---|
| `Broiler.Regex` | preview.4 | `u`/`v` patterns and gap shapes only; .NET for the rest | No (rule A2) | Every Broiler.VM requirement (`SJL-D-14` (a)): caller budget and exhaustion status, cancellation, limits without recursion, exact names, group names, duplicate names, `Katakana_Or_Hiragana`, offset-only results, Unicode data through the owner's provider, AOT flags, test262 in CI |
| `Broiler.Unicode.Properties` | preview.3 | Transitively, through Broiler.Regex; called directly | No (rules A2 and N1; JSD-0031 §4(b)) | Exact names, immutable results, code-point lookups, pinned and hashed inputs, a public version, tests, normalization, folding, casing without host fallbacks, and tables through a provider so each engine embeds its own (`SJL-D-6` (c)) |
| `Broiler.Unicode.Emoji.StringProperties` | preview.3 | Transitively | No | The ECMAScript name map; no allocation per call; a corrected README |
| `Broiler.Unicode.Cldr.LocaleData` | preview.3 | Directly (`BuiltIns`) | No (JSD-0027 §4: "Candidate input, not accepted") | None: Broiler.VM's generator replaces it as the source of locale data (`SJL-D-22` (a)), and Broiler.JS stops using it at SJL-8 and SJL-9 |
| `Broiler.DateTime` | preview.1 | No (dropped on 2026-09-18) | No | The civil-day primitives are internal in every committed state. A change making them public exists only in an uncommitted stash. Not used by the plan: `SJL-D-1` placed time data in the Broiler.JS repository. |
| `Broiler.JSeal` | preview.6 | — (it consumes both engines) | — | None. It is the host-facing abstraction above both engines; its package gate constrains shared package names (`SJL-D-2`). |
