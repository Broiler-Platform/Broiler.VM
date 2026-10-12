# Responsibility inventory, re-derived at named commits (SJL-0.1)

**Work package:** SJL-0.1. **Supersedes, for the gates:** the survey copy in `docs/javascript-shared-libraries/inventory.md`. **No line numbers are given.**

## Named commits

| Repository | Commit | Commit date |
|---|---|---|
| Broiler.VM | `0087e8d73ae3833200fa263b68f71db15b059a9c` | 2026-10-11 |
| Broiler.JS | `44a214dc5b04a5c8db556dc9b57894beaeb20d0c` | 2026-10-11 |
| Broiler.Unicode | `2affa78ffa96d2c975ad65eb8e990722b18f3e22` | 2026-10-11 |
| Broiler.RegEx (repository `Broiler.Regex`) | `c1422f4d824aa28ff6edd4766d56be25ba7c8a15` | 2026-10-11 |
| Broiler.DateTime | `8de12cc335170fb17ef9055458a125982e089b61` | 2026-10-07 |

## How this was derived

Every member the survey names was searched for by name in the checkout of its repository at the commit above, over the C# sources outside test, benchmark and sample projects. A member counts as present only when a declaration of it was found: a type declaration for a bare type name, or a method, property, field, nested type or local function declared inside that type for a `Type.Member` name. The file of each declaration is recorded in the companion `inventory-checks.txt`. A dotted name such as `JsRealm.Number` or `JsTemporal.Zones` names a partial-class file, and counts as present when that file declares the partial class. Where a cell names a type with a descriptor (helpers, kernels, formulas), the type was checked and the descriptor was resolved to members only where a bracketed list says so. Prose cells are kept and marked `[prose; not checked]`; qualifiers in parentheses (versions, metering, limits) are the survey's and were not re-checked, except the `2026e` release of `JsTzdbTables`, which was. Survey prose that gave a count is kept without it. Coupling classes, dispositions and milestones are the survey's, mapped to the vocabulary of SJL-0.1. Section 2 carries the coupling of the matching section-1 row. Section 3 applies the class definitions to the retained file's own reasons (object-model and execution code is heavy) or to the matching section-1 row, and says "not classified" where neither decides it; section 4's components are not classified.

Annotations: `[not found]` means no declaration at the named commit; a bracketed note gives a different name or file than the survey implies, or the members a reference resolves to; `[added]` marks a member the survey missed that plainly belongs to the row.

## 1. Duplicated between the engines

### Number conversion (SJL-3)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| `Number::toString` radix 10 | `JsNumberFormat.ToJsString`, `Decompose`, `Place`; `JsParser.NumberText` | `JSNumber.ToECMAString`, `RoundTrippable`; the `JSValue.NumberToECMAString` delegate | none | pure / pure | shared | SJL-3 |
| Radix 2–36 | `JsNumberFormat.ToRadixString`, `RoundUpRadix` | `JSNumberPrototype.DecimalToBase` [declared in partial class `JSNumber`, file `JSNumberPrototype.cs`], `DecimalToArbitrarySystem` (public) [same: `JSNumber`, file `JSNumberPrototype.cs`] | none | pure / pure | shared | SJL-3 |
| `toFixed`, `toExponential`, `toPrecision` | `JsRealm.Number` `Number*Text` statics [`NumberExponentialText`, `NumberFixedText`, `NumberPrecisionText`] | `JSNumberPrototype` kernels [partial class `JSNumber`, file `JSNumberPrototype.cs`: `Compare10Pow`, `ExactSignificantDigits`, `FormatExponential`, `RoundScaled`, `ShortestSignificantDigits`, `ToExactFraction`, `ToExponential`, `ToFixed`, `ToPrecision`] | none | light / light | shared | SJL-3 |
| `StringToNumber` | `JsNumberFormat.ToNumber`, `Trim`, `IsDecimalLiteral`, `ParseRadix` | `NumberParser.CoerceToNumber` (public), `ParseCore`, `RefineEstimate` | none | pure / pure | shared | SJL-3 |
| `parseInt`, `parseFloat` | `JsNumberFormat.ParseInt`, `ParseFloat` | `NumberParser.ParseInt`, `ParseFloat`; wrapper pairs: `JSGlobalStatic.ParseInt`, `ParseFloat` (file `JSGlobal.cs`), `JSNumber.ParseInt`, `ParseFloat` (file `JSNumberStatic.cs`) | none | pure / pure | shared | SJL-3 |
| Literal values | `SliceTokenizer.ReadNumericLiteral` values; `JsParser.TryBigIntValue` | `NumberCoercion`; `JSBigInt.TryParseBigIntLiteral`; `FastCompiler.CreateClass.BigIntLiteralToValue`, `FastCompiler.CreateClass.ParseBigIntRadix` [added] | none | light / light | shared | SJL-3 |
| Array-index keys | `JsObject.IsArrayIndex` | `NumberParser.TryGetArrayIndex` | none | pure / pure | shared | SJL-3 |
| `StrWhiteSpaceChar` | `JsNumberFormat.IsWhiteSpace` (runtime `Zs`) | `NumberParser.IsWhiteSpaceOrLineTerminator`, `trimCharacters`, .NET `char.IsWhiteSpace` paths [prose; not checked] | none | host / host | shared (pinned table) | SJL-3 |

### Numerics (SJL-4)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Integer conversions | `JsValue.ToInt32`, `ToUint32`, `ToInteger` | `JSValue.ToUint32`, `IntValue`; `JSNumericOperators` (public) | none | pure / pure | shared | SJL-4 |
| Clamping, BigInt64 bits, `ToIndex`, `ToLength` | `JsElements.Clamp`, `Low64`, `Narrow`; `BinaryToIndex` | inline clamp [prose; not checked]; masks [prose; not checked]; `DataView.ToIndex`, `JSBigInt.ToBitsIndex`, `JSJSON.ToLength` | none | pure / pure | shared | SJL-4 |
| Float16 | `JsFloat16` | inline `(Half)` casts [prose; not checked] | none | pure / pure | shared (NaN policy as a parameter) | SJL-4 |
| Math kernels | `JsRealm.Math` helpers [`MathExpMinusOne`, `MathHypotenuse`, `MathLogOnePlus`, `MathPower`, `MathRoundHalfUp`, `MathSumPrecise`] | `JSMath`; `JSValue.Power`; `**` emitted as `Math.Pow` [prose; not checked] | none | light / light | shared (only where an engine has custom logic) | SJL-4 |
| Unsigned rounding, exact rationals | `JsTemporalCore` rounding [`ApplyUnsigned`, `RoundToIncrement`, `RoundToIncrementAsIfPositive`, `UnsignedMode`]; `JsDecimal.JsUnsignedRounding` [top-level enum in `JsDecimal.cs`, not nested in `JsDecimal`] | copies across Temporal and Intl (survey gives a count) [prose; not checked]; `JSIntlNumberFormat` copy [`GetUnsignedRoundingMode`, `UnsignedRoundingMode`] [added], `TemporalRoundingOptions` copy (file `JSTemporalRoundingOptions.cs`) [`RoundToIncrement`, `RoundToIncrementAsIfPositive`, `UnsignedRoundingMode`] [added], `JSTemporalZonedDateTime` copy [`UnsignedRoundingMode`] [added], `JSTemporalDuration` copy [`RoundToIncrement`] [added] | none | pure / light | shared | SJL-4 |
| BigInt kernel | `JsBigInt` (metered, width ceiling) | `JSBigInt` (unbounded, unmetered, culture text) | none | pure / light | shared (width ceiling as a parameter) | SJL-4 |

### Text (SJL-5)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Well-formedness | `StringWellFormed` | `IsWellFormedUtf16`, `ToWellFormedUtf16` | none | light / light | shared | SJL-5 |
| Padding | `StringPadTo` (ceiling) | `StringPad` (no limit) | none | light / light | shared (length ceiling as a parameter) | SJL-5 |
| Substitution templates | `StringExpandDollars`; copies in `RegExpReplaceText`, `RegExpSubstitute` | `JSString.GetSubstitution`; initializer copy [local function `GetSubstitution` in `BuiltInsAssemblyInitializer`] | none | light / light | shared (template parse; evaluation stays in each engine) | SJL-5 |
| URI codecs | `GlobalUri*` [`GlobalUriEncode`, `GlobalUriDecode` and the other `GlobalUri*` helpers and constants], `GlobalEscapeText`, `GlobalUnescapeText` | `UriHelper` | none | light / pure | shared | SJL-5 |
| Base64 and hex | `Base64Encode`, `Base64Decode`, `Base64DecodeHex` | `JSUInt8Array` base64 and hex cores [`Base64DecodeResult`, `Base64Value`, `DecodeBase64`, `DecodeBase64Chunk`, `HexDigitValue`, `SkipAsciiWhitespace`, `TryParseHexByte`] | none | light / light–heavy | shared | SJL-5 |
| JSON lexing and quoting | `JsRealm.Json` reader [`JsonParseValue`, `JsonParseString`, `JsonParseNumber` and the other `JsonParse*` members, `JsonSkipSpace`, `JsonHexDigit`]; `JsonQuote`; `JsJsonModule` reader [nested class `JsJsonModule.Reader`] | `System.Text.Json` deserialisation [prose; not checked]; `JSJsonParser` [added]; `JSJSON.QuoteString` | none | heavy / heavy (drivers) | shared (reader and quoting; drivers retained in engines) | SJL-5 |

### Date (SJL-6)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Time arithmetic | `JsRealm.Date` `Date*` functions [`DateMakeDay`, `DateMakeTime`, `DateMakeDate`, `DateTimeClip`, `DateDayFromYear`, `DateYearFromTime` and the other `Date*FromTime` and `Date*Year` functions] | `JSDateMath` | none | pure / pure | shared | SJL-6 |
| Civil-day helpers | `JsTemporalCore`, `JsTimeZones` [its civil-day helper is `JsZone.DaysFromCivil`, in the same file], `JsLocalTime.From` | copies in every Temporal type [prose; not checked], calendar classes (survey gives a count) [prose; not checked], `TemporalIsoString` | Broiler.DateTime `ExtendedIsoDateTime.DaysFromCivil` [added], `CivilFromDays` (internal) [added], `IsLeapYear` [added], `DaysInMonth` [added] (not used by the plan) | pure / pure | shared | SJL-6 |
| Local time | UTC (policy) [prose; not checked] | `TimeZoneInfo.Local` [prose; not checked]; `JSDateMath.LocalTime` [added], `UTC` [added], `GetLocalOffsetMs` [added] | none | pure / host | shared (engine policy as a parameter) | SJL-6 |
| Date parsing | `DateParseText`, `DateParseTextual` | `DateParser` (public), `TryParseExtendedIso` | Broiler.DateTime `ExtendedIsoDateTime.Parse` [added], `TryParse` [added] (not used by the plan) | light / host | shared (normative grammar; each engine keeps its dialects) | SJL-6 |
| Date renderings | `DateTo*Text` [`DateToCalendarText`, `DateToClockText`, `DateToFullText`, `DateToIsoText`, `DateToUtcText`] | `JSDatePrototype.Formatters` [file `JSDatePrototype.Formatters.cs`, partial class `JSDate`] | none | pure / host | shared | SJL-6 |

### Unicode (SJL-7)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| `\p` properties | `JsUnicodeProperties` (exact) | `Broiler.Unicode.Properties` (loose): see Component members; the translator's lookups [prose; not checked] | Broiler.Unicode `UnicodeProperties.GetBinaryProperty`, `GetGeneralCategory`, `GetScript`, `GetScriptExtensions`; Broiler.Regex `UnicodeCharSets.ResolveProperty` [added] | pure / pure | shared (owner Broiler.Unicode, exact-name API) | SJL-7 |
| Lexical classes | `JsUnicodeLexical` | `CharExtensions`; several whitespace lists [prose; not checked] | none | pure / host | shared | SJL-7 |
| Case folding | `JsUnicodeCaseFolding` | Broiler.Regex `CaseFoldingData` (identical): see Component members; the translator's .NET casing [prose; not checked] | Broiler.Regex `CaseFoldingData`, `CaseFolding` [added] | pure / host | shared | SJL-7 |
| Case mapping | `JsUnicodeCasing`; `JsRealm.String` language casing [`LanguageCasing`] | `JSStringSpecialCasing` with host fallbacks [not found] [declared in Broiler.Unicode instead: see Component members]; `CultureInfo` [prose; not checked] | Broiler.Unicode `JSStringSpecialCasing.ToUpper`, `ToLower`, `ToLocaleUpper`, `ToLocaleLower` | light / host | shared | SJL-7 |
| Normalization | `JsUnicodeNormalization`; `JsRealm.String` passes [`ComposeCanonically`, `IsNormalizedAlready`, `NormalizeText`, `OrderCanonically`, `ReadCodePoint`, `WriteCodePoints`] | host `string.Normalize` [prose; not checked] | none | light / host | shared | SJL-7 |
| Properties of strings | `JsUnicodeStringProperties` | `Broiler.Unicode.Emoji.StringProperties` (identical data): see Component members | Broiler.Unicode `EmojiStringProperties.GetSequences`, `EmojiTrieData` | pure / pure | shared | SJL-7 |
| Segmentation | `JsSegmenter` and its tables [not found] [no type `JsSegmenter`; file `JsSegmenter.cs` declares `JsSegmentation`, `JsBreakData` and the break-property enums], `JsSegmentation` [added], `JsBreakData` [added] | `StringInfo`, heuristics, whole-string sentences [prose; not checked]; `JSIntlSegmenter` [added], `JSIntlSegments` [added] | none | light / host | adoption (Broiler.VM's algorithm) | SJL-7 |

### Time zones and calendars (SJL-8)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| tz data and format | `JsTimeZones`, `JsTzdbTables` (2026e, pinned) | `Tz/*` [`IanaTimeZone`, `IanaTimeZoneDatabase`, `TzifReader`], `tzdata.bin` (2025b, host-built) | none | pure / pure | shared (one dataset, one policy) | SJL-8 |
| Zone algorithms | `JsTemporal.Zones` | `JSTemporalZonedDateTime` helpers | none | light / heavy | shared | SJL-8 |
| Calendar tables | `JsCalendars` table parts, `JsCalendarTables` (ICU4X) | .NET calendars [prose; not checked]; the CLDR package's calendars: see Component members | Broiler.Unicode `CldrChineseCalendar`, `CldrUmmAlQura` | pure / host | shared (pinned sources) | SJL-8 |

### Locale data and Intl (SJL-9)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Locale tags, likely subtags | `JsLocaleTag`; `JsIntlTables` | `JSIntl` validation and aliases; `CldrLikelySubtags` [not found] [declared in Broiler.Unicode instead: see Component members] | Broiler.Unicode `CldrLikelySubtags.Maximize`, `Minimize` | pure / light | shared | SJL-9 |
| Negotiation | `JsRealm.Intl` [`CanonicalizeLocaleList`, `FilterLocales`, `LookupMatchingLocale`, `ResolveLocale`] | `JSIntl` | none | light / host | shared (defaults supplied by each engine as providers) | SJL-9 |
| Intl mathematical value, number formatting | `JsDecimal`, `JsNumberData`, `JsNumberFormatter` | `JSIntlNumberFormat` | Broiler.Unicode `CldrLocaleData.ResolveCurrency` [added], `GetUnitPattern` [added] | light / host | shared | SJL-9 |
| Plural rules | `JsPluralRule` | the CLDR package's rules: see Component members; `JSIntlPluralRules` [added] | Broiler.Unicode `CldrPluralRules`, `CldrLocaleData.SelectPlural` | pure / pure | shared | SJL-9 |
| Date-time formatting, intervals, zone names | `JsDatePatternGenerator`, `JsDateTimeFormatter`, `JsDateIntervalFormat`, `JsZoneNames`, `JsDateData` | `JSIntlDateTimeFormatEngine` (English) | Broiler.Unicode `CldrLocaleData.GetTimeZoneName` [added], `GetDayPeriod` [added] | light / host | shared | SJL-9 |
| List, relative time, display names, duration | the `JsRealm.*` cores: `JsRealm.ListFormat`, `JsRealm.RelativeTimeFormat`, `JsRealm.DisplayNames`, `JsRealm.DurationFormat` | `JSIntl*` classes: `JSIntlListFormat`, `JSIntlRelativeTimeFormat`, `JSIntlDisplayNames`, `JSIntlDurationFormat`; display names are a stub [prose; not checked] | Broiler.Unicode `CldrLocaleData.GetListPattern` [added], `GetRelativeTimePattern` [added], `GetRelativeTimeExact` [added] | light / light | shared | SJL-9 |
| Collation | `JsCollator`, `JsCollationData` | host `CompareInfo` [prose; not checked]; `JSIntlCollator` [added] | none | light / host | shared (later, at SJL-17; `SJL-D-24` (c)) | SJL-17 |
| CLDR data | `JsCldrTables` (generated, pinned) | `Broiler.Unicode.Cldr.LocaleData`: see Component members; hand-written subsets [prose; not checked]; host [prose; not checked] | Broiler.Unicode `CldrLocaleData`; generated tables such as `CldrDateData`, `CldrPluralData` | pure / host | shared (Broiler.VM's generator and pins in the Broiler.JS repository; tables embedded by each engine; `SJL-D-22` (a), `SJL-D-6` (c)) | SJL-9 |

### Temporal (SJL-10)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Records, limits, balancing | `JsTemporalCore` | per-type copies [prose; not checked] | none | pure / light | shared | SJL-10 |
| Durations, rounding, totals | `JsTemporal.Durations` | `JSTemporalDuration*` [`JSTemporalDuration`, files `JSTemporalDuration.cs` and `JSTemporalDuration.Arithmetic.cs`], `ZonedDateTime.Rounding` [file `JSTemporalZonedDateTime.Rounding.cs`, partial class `JSTemporalZonedDateTime`], `PlainDate` rounding [prose; not checked] | none | light / heavy | shared | SJL-10 |
| Differences | realm `Difference*` cores [`DifferenceTemporal*`, one per Temporal type, in the `JsRealm.Temporal*` files] | per-type `Difference*` [`Difference` in each `JSTemporal*` type, with `DifferenceISODate`, `DifferenceISODateTime`, `DifferenceInternal` and others] | none | light / heavy | shared | SJL-10 |
| Grammar | `JsTemporalParser` | per-type parsers (survey gives a count) [prose; not checked]; `TemporalIsoString` | none | light / heavy | shared (Broiler.VM's grammar) | SJL-10 |
| Option rules | `JsTemporal.Options` | per-type option readers (survey gives an approximate count) [prose; not checked]; `TemporalRoundingOptions` [added], per-type `ReadOverflow` [`ReadOverflow` in each plain and zoned Temporal type] [added] | none | heavy / heavy | shared (rules; option reading retained in engines) | SJL-10 |
| Calendars | `JsCalendars` formulas, `JsTemporal.CalendarNonIso` | `TemporalNonIso`, `JSTemporalCalendar` [no type of that name; file `JSTemporalCalendar.cs` declares `TemporalCalendar`] | none | pure / light | shared | SJL-10 |
| Serialization | `JsTemporalCore`, `JsTemporal.Conversions` | per-type formatters [prose; not checked] | none | pure / pure | shared | SJL-10 |

### Regular expressions (SJL-11)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Engine | `JsRegExpMatcher` (metered, bounded) | .NET translator [prose; not checked]; plus `Broiler.Regex` (unmetered, loose): see Component members; `RegexTiering` [added], `JSRegExp.TryRouteToBroiler` [added] | Broiler.Regex `BroilerRegex`, `RegexParser`, `Matcher`, `Budget` | light / heavy | shared (owner Broiler.Regex, fixed to Broiler.VM's requirements; `SJL-D-14` (a)) | SJL-11 |
| Flags, `escape`, `source` | `JsRealm.RegExp` helpers [`RegExpEscapeCodePoint`, `RegExpEscapeText`, `RegExpFlagGetter`, `RegExpHasFlag`, `RegExpNormalizeFlags`, `RegExpSourceText`] | `JSRegExp` helpers [`BuildFlagsString`, `Escape`, `EscapePatternSource`, `ParseFlags`, `Source`]; scanner [prose; not checked]; validator [`RegExpValidator`] | Broiler.Regex `RegexFlagsParser` [added] | pure / pure | shared | SJL-11 |
| Literal validation | the compiler, with the same parser [prose; not checked] | .NET compilation in the scanner [prose; not checked] | none | light / host | shared | SJL-11 |

### Front end (SJL-12 to SJL-15)

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Tokenizer | `SliceTokenizer` | `FastScanner`, `FastTokenStream` | none | pure / host | shared | SJL-12 (contract), SJL-13 (Broiler.VM), SJL-14 (Broiler.JS) |
| Parser and tree | `JsParser`, `JsSyntax` [not found] [no type of that name; file `JsSyntax.cs` declares the wide tree, from `JsNode`] (wide); `SliceParser`, `SliceSyntax` [not found] [no type of that name; file `SliceSyntax.cs` declares the slice tree, from `SliceNode`] (slice) | `FastParser`; `Ast` [project `Broiler.JavaScript.Ast`, from `AstNode`] | none | pure / heavy (ambient) | shared | SJL-12 (contract), SJL-13 (Broiler.VM), SJL-14 (Broiler.JS) |
| Diagnostics | `SliceSourceDiagnosticCode`; registry [prose; not checked] | `FastParseException` messages | none | light / light | shared (codes; rendering retained in engines) | SJL-12 (mapping), SJL-13 (Broiler.VM), SJL-14 (Broiler.JS) |
| Static semantics and early errors | `SliceStaticSemantics`; the wide binder in `JsCompiler` | `FastScope`, `FastFunctionScope`, `FreeNameScan`, `SyntaxValidation`, `FastCompiler.EarlyErrors`, `DirectEvalSupport` | none | heavy / heavy | shared | SJL-15 |

## 2. Duplicated inside one engine

The programme removes these in the same change as the cross-engine copy. The cause is given in the Responsibility cell.

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Radix-10 number text in the compiler beside the runtime (cause: the compiler may not reference the runtime profile) | `JsParser.NumberText`; beside `JsNumberFormat.ToJsString` | none (Broiler.VM duplicate) | none | pure / — | shared | SJL-3 |
| BigInt literal values in the compiler beside the runtime's BigInt parsing (cause: as above) | `JsParser.TryBigIntValue`; the runtime's BigInt parsing [prose; not checked], `JsBigInt.TryParseStringInteger` [added] | none (Broiler.VM duplicate) | none | light / — | shared | SJL-3 |
| JSON module reader beside the realm's JSON reader (cause: as above) | `JsJsonModule`'s reader [nested class `JsJsonModule.Reader`]; `JsRealm.Json`'s reader [the `JsonParse*` members] | none (Broiler.VM duplicate) | none | heavy / — | shared | SJL-5 |
| The regular-expression matcher and the Unicode lookups placed in Format; the tables stay there, regenerated, under `SJL-D-6` (c) (cause: the compiler and the runtime both need them) | the regular-expression matcher [`JsRegExpMatcher`]; the Unicode lookups: `JsUnicodeProperties`, `JsUnicodeCaseFolding`, `JsUnicodeStringProperties`, `JsUnicodeLexical` | none (Broiler.VM duplicate) | none | pure (lookups), light (matcher) / — | shared (generated tables stay in Format, regenerated; `SJL-D-6` (c)) | SJL-7, SJL-11 |
| Gregorian day arithmetic (cause: written per area) | `JsRealm.Date` [`DateDayFromYear`, `DateDaysInYear`, `DateInLeapYear`, `DateMakeDay`, `DateYearFromTime`], `JsTemporalCore` [`DaysInMonth`, `EpochDays`, `FromEpochDays`, `IsLeapYear`], `JsTimeZones` [civil-day arithmetic in `JsZone.DaysFromCivil`, same file], `JsLocalTime.From` | none (Broiler.VM duplicate) | none | pure / — | shared | SJL-6 |
| Unsigned rounding (cause: written per area) | `JsTemporalCore` [`ApplyUnsigned`, `UnsignedMode`]; and in `JsDecimal` [top-level enum `JsUnsignedRounding` in `JsDecimal.cs`], with `JsNumberFormatter` [`Unsigned` mapping] | none (Broiler.VM duplicate) | none | pure / — | shared | SJL-4 |
| Template expansion (cause: written per call site) | `StringExpandDollars`, `RegExpReplaceText`, `RegExpSubstitute` | none (Broiler.VM duplicate) | none | light / — | shared | SJL-5 |
| Number conversion (cause: layering; the runtime may not reference the built-ins, so a delegate seam) | none (Broiler.JS duplicate) | `NumberCoercion`, `NumberParser`, `JSNumber` | none | — / pure | shared | SJL-3 |
| `parseInt` and `parseFloat` wrapper pairs | none (Broiler.JS duplicate) | `JSGlobal` [type `JSGlobalStatic`, file `JSGlobal.cs`], `ParseInt`, `ParseFloat`; `JSNumberStatic` [file `JSNumberStatic.cs`, partial class `JSNumber`], `ParseInt`, `ParseFloat` | none | — / pure | shared | SJL-3 |
| Whitespace sets | none (Broiler.JS duplicate) | `NumberParser` [`IsWhiteSpaceOrLineTerminator`], `trimCharacters`, `char.IsWhiteSpace` [prose; not checked], `StringSpan.Trim`, `FastScanner`, `JSRegExp.Escape` | none | — / host | shared | SJL-3, SJL-7 |
| BigInt literal parsing | none (Broiler.JS duplicate) | `JSBigInt` [`TryParseBigIntLiteral`], `FastCompiler.CreateClass` [`BigIntLiteralToValue`, `ParseBigIntRadix`] | none | — / light | shared | SJL-3 |
| Leap year, civil days, ISO validity, unsigned rounding, `ReadOverflow` and string parsers, once per Temporal type (cause: written per type) | none (Broiler.JS duplicate) | leap year, civil days, ISO validity, unsigned rounding and string parsers per type [prose; not checked]; `ReadOverflow` [one per plain and zoned Temporal type] | none | — / light–heavy | shared | SJL-6, SJL-10 |
| Regular-expression flag parsing in several places; the .NET engine and Broiler.Regex side by side (survey gives a count) (cause: incremental adoption of Broiler.Regex) | none (Broiler.JS duplicate) | flag parsing in several places [prose; not checked]; `JSRegExp.ParseFlags` [added]; `RegexTiering` | Broiler.Regex `BroilerRegex`, `RegexFlagsParser` | — / heavy | shared (Broiler.Regex's engine and flag parser) | SJL-11 |
| Early errors (cause: historical) | none (Broiler.JS duplicate) | the parser [prose; not checked], `SyntaxValidation`, `FastCompiler.EarlyErrors`, `Visit*` [prose; not checked], `DirectEvalSupport` | none | — / heavy | shared | SJL-15 |

## 3. Present in one engine only, and retained in both

The survey points to `areas/retained-in-engines.md`, sections 1 to 5, and lists no rows of its own. That file's rows are carried here so that each has a disposition; their member cells are the file's prose, except where a type is named and was checked.

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| Value representation (retained file, section 1) | `JsValue` (internal tagged struct) | `JSValue` (public class hierarchy) | none | heavy / heavy | retained in engines (not planned, `SJL-D-18`) | none |
| Object storage, shapes, property maps (retained file, section 1) | its own [prose; not checked] | `Storage` [project `Broiler.JavaScript.Storage`] | none | heavy / heavy | retained in engines | none |
| Frames, dispatch, suspension, generators, async (retained file, section 1) | its interpreter and native forms [prose; not checked] | its expression-tree compiler and runtime [prose; not checked] | none | heavy / heavy | retained in engines | none |
| Compiler back ends and emitted forms (retained file, section 1) | bytecode, the universal bytecode programme, native forms [prose; not checked] | IL through expression trees; the portable bytecode island [prose; not checked] | none | heavy / heavy | retained in engines | none |
| Verification, resource budgets, cancellation, continuations (retained file, section 1) | its own (core contract) [prose; not checked] | none | none | heavy / — | retained in engines | none |
| Realm and agent lifecycle, host integration, capability policy (retained file, section 1) | its composition model [prose; not checked] | its bootstrap profiles and registries [prose; not checked] | none | heavy / heavy | retained in engines | none |
| Built-in object implementations: `Array`, `Promise`, `Proxy`, `Reflect`, collections, iterators, typed arrays, `Atomics`, `WeakRef`, `FinalizationRegistry`, `ShadowRealm`; the `JSON` drivers; the `RegExp` object and its protocols; `split` and `replace` drivers; Intl and Temporal objects and their option reading; `Error` objects and stack text; `Function.prototype.toString`; string search; `Math.random` (retained file, section 2) | each built-in object [prose; not checked] | each built-in object [prose; not checked] | none | heavy / heavy | retained in engines (the algorithms underneath move to their areas) | none |
| `Date`'s local time zone (retained file, section 3) | UTC [prose; not checked] | the host's `TimeZoneInfo.Local` [prose; not checked] | none | pure / host | shared (engine policy as a parameter; `SJL-D-19`) | SJL-6 |
| Clock and its resolution (retained file, section 3) | the host clock, at its own resolution for `Temporal.Now` [prose; not checked] | the host clock, at a finer resolution [prose; not checked] | none | — (not classified) | shared (engine policy as a parameter; `SJL-D-19`) | SJL-10 |
| Default locale (retained file, section 3) | fixed `en-US` [prose; not checked] | the host culture [prose; not checked] | none | pure / host | shared (engine policy as a parameter; `SJL-D-19`) | SJL-9 |
| Available locales (retained file, section 3) | its data [prose; not checked] | the host's cultures, optionally [prose; not checked] | none | pure / host | shared (engine policy as a parameter; `SJL-D-22`) | SJL-9 |
| BigInt width ceiling (retained file, section 3) | its constant [prose; not checked] | unbounded, under `SJL-D-15` (a) [prose; not checked] | none | pure / light | shared (engine policy as a parameter; `SJL-D-15`) | SJL-4 |
| String length ceiling, JSON depth, regular-expression frames and trail (retained file, section 3) | its constants [prose; not checked] | its own choices [prose; not checked] | none | — (not classified) | shared (engine policy as a parameter: passed as limits) | SJL-5, SJL-11 |
| Metering (retained file, section 3) | fuel, cancellation and the wall clock, through its charge [prose; not checked] | none, or its own [prose; not checked] | none | — (not classified) | shared (engine policy as a parameter; `SJL-D-10`) | SJL-3, then every area |
| Broiler.JS's non-standard extensions: `Decimal`, `m` literals and `typeof` `"decimal"`; `Number.prototype.clz`; `String.prototype.contains`; the .NET format-string extensions of `toLocaleString`; the `N/N/N` date heuristics and culture-dependent legacy `Date.parse`; the legacy `RegExp` statics; CLR interop for `DateTime` and `DateTimeOffset` (retained file, section 4) | none | each extension [prose; not checked] | none | — (not classified) | retained in engines (`SJL-D-17`) | none |
| Responsibilities only Broiler.VM has: metering and limits; deterministic host policy; optional deployment data; pins and generators; full UAX #29 segmentation; UCA collation; exact property names; Pattern_Syntax-correct identifiers; compile-time regular-expression early errors; reference datasets; canonical Float16 NaN; divide-and-conquer BigInt text; CLDR metazone history and zone names; the pattern generator and interval formats; `DisplayNames` data; the `fractional` duration style (retained file, section 5) | each responsibility [prose; not checked] | none | none | — (not classified) | retained in engines (an area that moves one records it as an adoption, e.g. segmentation at SJL-7 and collation at SJL-17) | none |
| Responsibilities only Broiler.JS has: ES2025 duplicate named groups; host-wide locale coverage through ICU; finer-resolution `Temporal.Now`; backzone history before 1970 and the `Factory` zone; compact-notation data for more locales; the CLR interop surface; the non-standard extensions (retained file, section 5) | none | each responsibility [prose; not checked] | none | — (not classified) | retained in engines (duplicate named groups kept through SJL-11; backzone and `Factory` dropped at SJL-8 under `SJL-D-20`) | none |
| Dictionary-based or locale-tailored segmentation (retained file, section 5: neither engine) | none | none | none | — (not classified) | retained in engines (absent from both engines; not planned) | none |

## 4. Existing component libraries

The survey's table names no members. The member cells here were derived at the named commits (the component's principal types, and the Broiler.JS types that call it); they are not marked `[added]`, and each is in the evidence file. No reference to any of these packages was found in Broiler.VM.

| Responsibility | Broiler.VM members | Broiler.JS members | Component members | Coupling (VM / JS) | Disposition | Area milestone |
|---|---|---|---|---|---|---|
| `Broiler.Regex` (preview.4): used by Broiler.JS for `u`/`v` patterns and gap shapes only, .NET for the rest; not used by Broiler.VM (rule A2) | none (no reference found) | `JSRegExp.TryRouteToBroiler`, `GapScan` [nested class], `RegexTiering` | `BroilerRegex`, `RegexParser`, `Matcher`, `Budget`, `CaseFoldingData`, `RegexFlagsParser`, `UnicodeCharSets` | — (not classified) | shared (owner of the SJL-11 engine, after closing every Broiler.VM requirement; `SJL-D-14` (a)) | SJL-11 |
| `Broiler.Unicode.Properties` (preview.3): used by Broiler.JS transitively, through Broiler.Regex, and called directly; not used by Broiler.VM (rules A2 and N1; JSD-0031 §4(b)) | none (no reference found) | calls from `JSRegExp` (files `JSRegExp.cs`, `JSRegExp.UnicodeSets.cs`), `JSString` (file `JSStringPrototype.Transform.cs`) | `UnicodeProperties`, `JSStringSpecialCasing`, `BinaryPropertyData`, `GeneralCategoryData`, `ScriptData`, `ScriptExtensionsData` | — (not classified) | shared (owner of the SJL-7 data and algorithms; tables through a provider, `SJL-D-6` (c)) | SJL-7 |
| `Broiler.Unicode.Emoji.StringProperties` (preview.3): used by Broiler.JS transitively; not used by Broiler.VM | none (no reference found) | `JSRegExp` [calls `EmojiStringProperties.GetSequences` directly, files `JSRegExp.cs` and `JSRegExp.UnicodeSets.cs`] | `EmojiStringProperties`, `EmojiTrieData`, `EmojiSequenceEnumerator` | — (not classified) | shared (owner of the SJL-7 string-property data) | SJL-7 |
| `Broiler.Unicode.Cldr.LocaleData` (preview.3): used by Broiler.JS directly (`BuiltIns`); not used by Broiler.VM (JSD-0027 §4: "Candidate input, not accepted") | none (no reference found) | calls from `JSIntl`, `JSIntlDateTimeFormatEngine`, `TemporalArithmeticCalendar`, `TemporalLunisolarCalendar` | `CldrLocaleData`, `CldrLikelySubtags`, `CldrPluralRules`, `CldrTimeZones`, `CldrDayPeriods`, `CldrChineseCalendar`, `CldrUmmAlQura` | — (not classified) | candidate for refusal (not accepted as a source of locale data: Broiler.VM's generator replaces it, `SJL-D-22` (a); Broiler.JS stops using it at SJL-8 and SJL-9) | SJL-8, SJL-9 |
| `Broiler.DateTime` (preview.1): not used by Broiler.JS (dropped on 2026-09-18) or by Broiler.VM | none (no reference found) | none (no code reference found; test comments only) | `ExtendedIsoDateTime.DaysFromCivil`, `CivilFromDays` (both internal) | — (not classified) | candidate for refusal (not used by the plan; `SJL-D-1` placed time data in the Broiler.JS repository) | none |
| `Broiler.JSeal` (preview.6): the host-facing abstraction above both engines; its package gate constrains shared package names (`SJL-D-2`) | — (it consumes both engines) | — (it consumes both engines) | not checked (no checkout at a named commit) | — (not classified) | retained in engines (not a library of the programme: it stays where it is, above both engines, and the programme changes nothing in it) | none |

## Differences from the survey

Paths are relative to the root of the named repository. A member marked `[not found]` was searched for across the C# sources of its repository outside test, benchmark and sample projects, at the named commit. Survey prose that quoted a count (wrapper pairs, copies, calendar classes, parsers, readers, engines) is kept without the count; this is not listed below.

### Not found

- `JSStringSpecialCasing` with host fallbacks (Broiler.JS): no declaration in Broiler.JS; it is declared in Broiler.Unicode, `src/UnicodeProperties/JSStringSpecialCasing.cs`, and Broiler.JS calls it.
- `JsSegmenter` and its tables (Broiler.VM): no type of that name; the file `src/Broiler.VM.Profile.JavaScript/JsSegmenter.cs` declares `JsSegmentation` and `JsBreakData` (found).
- `CldrLikelySubtags` (Broiler.JS): no declaration in Broiler.JS; it is declared in Broiler.Unicode, `src/UnicodeCldr.LocaleData/CldrLikelySubtags.cs`, and `JSIntl` calls it.
- `JsSyntax` (Broiler.VM): no type of that name; the file `src/Broiler.VM.Profile.JavaScript.Compiler/JsSyntax.cs` declares the wide tree records, from `JsNode` (found).
- `SliceSyntax` (Broiler.VM): no type of that name; the file `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSyntax.cs` declares the slice tree records, from `SliceNode` (found).

### Found under a different name, type or file

- `JSNumberPrototype.DecimalToBase` (Broiler.JS): no type `JSNumberPrototype`; declared in partial class `JSNumber`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Number/JSNumberPrototype.cs`.
- `DecimalToArbitrarySystem` (public) (Broiler.JS): no type `JSNumberPrototype`; declared in partial class `JSNumber`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Number/JSNumberPrototype.cs`.
- `JSNumberPrototype` kernels (Broiler.JS): no type `JSNumberPrototype`; the kernels are members of partial class `JSNumber`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Number/JSNumberPrototype.cs`.
- `JsDecimal.JsUnsignedRounding` (Broiler.VM): a top-level enum `JsUnsignedRounding` in the file `JsDecimal.cs`, not a member of `JsDecimal`; `src/Broiler.VM.Profile.JavaScript/JsDecimal.cs`.
- `JsTimeZones` (Broiler.VM): the type exists, but its civil-day arithmetic is `JsZone.DaysFromCivil` in `JsTimeZones.cs`; `src/Broiler.VM.Profile.JavaScript/JsTimeZones.cs`.
- `JSDatePrototype.Formatters` (Broiler.JS): no type `JSDatePrototype`; a partial-class file of `JSDate`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Date/JSDatePrototype.Formatters.cs`.
- `ZonedDateTime.Rounding` (Broiler.JS): the survey abbreviates the name; it is the partial-class file of `JSTemporalZonedDateTime`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalZonedDateTime.Rounding.cs`.
- `JSTemporalCalendar` (Broiler.JS): no type `JSTemporalCalendar`; the file `JSTemporalCalendar.cs` declares `TemporalCalendar`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalCalendar.cs`.
- `JSGlobal` (Broiler.JS): no type `JSGlobal`; the file `JSGlobal.cs` declares `JSGlobalStatic`; `Broiler.JS/Broiler.JavaScript.Globals/JSGlobal.cs`.
- `JSNumberStatic` (Broiler.JS): no type `JSNumberStatic`; a partial-class file of `JSNumber`; `Broiler.JS/Broiler.JavaScript.BuiltIns/Number/JSNumberStatic.cs`.
- `JSRegExp` (Broiler.JS): the survey says "transitively"; `JSRegExp` also calls `EmojiStringProperties.GetSequences` directly; `Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.cs`.

### Added

- `FastCompiler.CreateClass.ParseBigIntRadix` (Broiler.JS): `FastCompiler.ParseBigIntRadix` in `Broiler.JS/Broiler.JavaScript.Compiler/Declarations/FastCompiler.CreateClass.cs`; the radix helper beside `BigIntLiteralToValue`, on the numbers area deletion list.
- `JSIntlNumberFormat` copy (Broiler.JS): `JSIntlNumberFormat.GetUnsignedRoundingMode`, `JSIntlNumberFormat.UnsignedRoundingMode` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs`; an Intl copy of unsigned rounding.
- `TemporalRoundingOptions` copy (file `JSTemporalRoundingOptions.cs`) (Broiler.JS): `TemporalRoundingOptions.RoundToIncrement`, `TemporalRoundingOptions.RoundToIncrementAsIfPositive`, `TemporalRoundingOptions.UnsignedRoundingMode` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalRoundingOptions.cs`; a Temporal copy of unsigned rounding.
- `JSTemporalZonedDateTime` copy (Broiler.JS): `JSTemporalZonedDateTime.UnsignedRoundingMode` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalZonedDateTime.Rounding.cs`; a Temporal copy of unsigned rounding.
- `JSTemporalDuration` copy (Broiler.JS): `JSTemporalDuration.RoundToIncrement` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalDuration.Arithmetic.cs`; a Temporal copy of increment rounding.
- `JSJsonParser` (Broiler.JS): `JSJsonParser` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Json/JSJsonParser.cs`; the `System.Text.Json` converter that drives guest `JSON.parse`.
- `ExtendedIsoDateTime.DaysFromCivil` (Broiler.DateTime): `ExtendedIsoDateTime.DaysFromCivil` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; an internal civil-day primitive of a component the plan does not use.
- `CivilFromDays` (internal) (Broiler.DateTime): `ExtendedIsoDateTime.CivilFromDays` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; its inverse, also internal.
- `IsLeapYear` (Broiler.DateTime): `ExtendedIsoDateTime.IsLeapYear` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; public.
- `DaysInMonth` (Broiler.DateTime): `ExtendedIsoDateTime.DaysInMonth` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; public.
- `JSDateMath.LocalTime` (Broiler.JS): `JSDateMath.LocalTime` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Date/JSDateMath.cs`; the local-time conversion over `TimeZoneInfo.Local`.
- `UTC` (Broiler.JS): `JSDateMath.UTC` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Date/JSDateMath.cs`; its inverse.
- `GetLocalOffsetMs` (Broiler.JS): `JSDateMath.GetLocalOffsetMs` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Date/JSDateMath.cs`; the host offset lookup.
- `ExtendedIsoDateTime.Parse` (Broiler.DateTime): `ExtendedIsoDateTime.Parse` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; an extended-ISO parser in a component the plan does not use.
- `TryParse` (Broiler.DateTime): `ExtendedIsoDateTime.TryParse` in `src/Broiler.DateTime/ExtendedIsoDateTime.cs`; its non-throwing form.
- `UnicodeCharSets.ResolveProperty` (Broiler.RegEx): `UnicodeCharSets.ResolveProperty` in `Broiler.Regex/Unicode/UnicodeCharSets.cs`; Broiler.Regex's property resolution over Broiler.Unicode.Properties.
- `CaseFolding` (Broiler.RegEx): `CaseFolding` in `Broiler.Regex/Unicode/CaseFolding.cs`; the folding code over `CaseFoldingData`.
- `JsSegmentation` (Broiler.VM): `JsSegmentation` in `src/Broiler.VM.Profile.JavaScript/JsSegmenter.cs`; the segmentation algorithm, under a different name from the survey.
- `JsBreakData` (Broiler.VM): `JsBreakData` in `src/Broiler.VM.Profile.JavaScript/JsSegmenter.cs`; its break-property tables.
- `JSIntlSegmenter` (Broiler.JS): `JSIntlSegmenter` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs`; the `Intl.Segmenter` implementation over those heuristics.
- `JSIntlSegments` (Broiler.JS): `JSIntlSegments` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs`; its segments object.
- `CldrLocaleData.ResolveCurrency` (Broiler.Unicode): `CldrLocaleData.ResolveCurrency` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR currency data that `JSIntlNumberFormat` reads.
- `GetUnitPattern` (Broiler.Unicode): `CldrLocaleData.GetUnitPattern` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR unit patterns that Broiler.JS reads.
- `JSIntlPluralRules` (Broiler.JS): `JSIntlPluralRules` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs`; the `Intl.PluralRules` class that calls the CLDR package.
- `CldrLocaleData.GetTimeZoneName` (Broiler.Unicode): `CldrLocaleData.GetTimeZoneName` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; the zone names `JSIntlDateTimeFormatEngine` reads.
- `GetDayPeriod` (Broiler.Unicode): `CldrLocaleData.GetDayPeriod` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR day periods.
- `CldrLocaleData.GetListPattern` (Broiler.Unicode): `CldrLocaleData.GetListPattern` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR list patterns.
- `GetRelativeTimePattern` (Broiler.Unicode): `CldrLocaleData.GetRelativeTimePattern` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR relative-time patterns.
- `GetRelativeTimeExact` (Broiler.Unicode): `CldrLocaleData.GetRelativeTimeExact` in `src/UnicodeCldr.LocaleData/CldrLocaleData.cs`; CLDR relative-time exact forms.
- `JSIntlCollator` (Broiler.JS): `JSIntlCollator` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs`; the `Intl.Collator` class over `CompareInfo`.
- `TemporalRoundingOptions` (Broiler.JS): `TemporalRoundingOptions` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalRoundingOptions.cs`; the shared rounding-option reader, file `JSTemporalRoundingOptions.cs`.
- per-type `ReadOverflow` (Broiler.JS): `JSTemporalPlainDate.ReadOverflow`, `JSTemporalPlainDateTime.ReadOverflow`, `JSTemporalPlainMonthDay.ReadOverflow`, `JSTemporalPlainTime.ReadOverflow`, `JSTemporalPlainYearMonth.ReadOverflow`, `JSTemporalZonedDateTime.ReadOverflow` in `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalPlainDate.cs`, `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalPlainDateTime.cs`, `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalPlainMonthDay.cs`, `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalPlainTime.cs`, `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalPlainYearMonth.cs`, `Broiler.JS/Broiler.JavaScript.BuiltIns/Temporal/JSTemporalZonedDateTime.cs`; the per-type overflow readers named in section 2.
- `RegexTiering` (Broiler.JS): `RegexTiering` in `Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/RegexTiering.cs`; the router between the .NET engine and Broiler.Regex.
- `JSRegExp.TryRouteToBroiler` (Broiler.JS): `JSRegExp.TryRouteToBroiler` in `Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.Broiler.cs`; the route into Broiler.Regex.
- `RegexFlagsParser` (Broiler.RegEx): `RegexFlagsParser` in `Broiler.Regex/RegexFlags.cs`; Broiler.Regex's flag parser.
- `JsBigInt.TryParseStringInteger` (Broiler.VM): `JsBigInt.TryParseStringInteger` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs`; the runtime's BigInt text parser.
- `JSRegExp.ParseFlags` (Broiler.JS): `JSRegExp.ParseFlags` in `Broiler.JS/Broiler.JavaScript.BuiltIns/RegExp/JSRegExp.cs`; one of the flag parsers, in `JSRegExp.cs`.
