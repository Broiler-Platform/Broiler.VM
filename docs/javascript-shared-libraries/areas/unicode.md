# Area: Unicode character data and algorithms

**Delivered by:** [SJL-7](../milestones/sjl-7-unicode.md). **Owner:** Broiler.Unicode, under the
conditions of SJL-7.1; otherwise the Broiler.JS repository (decision
[SJL-D-1](../decisions.md#sjl-d-1) (b), taken on 2026-10-11). **Data:** the owner holds the
generator and the pins, and each engine embeds the tables (decision
[SJL-D-6](../decisions.md#sjl-d-6) (c)). **Status:** design; its verdict is [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), all six sub-areas
accepted and placed with Broiler.Unicode under its owner conditions, and decision SJL-D-6 recorded, on
2026-10-16. Data comparisons below were made by decoding the generated tables of both sides without executing
either engine. *(Revised 2026-10-16: the status read "design.")*

---

## 1. Scope

| Sub-area | Responsibilities |
|---|---|
| Properties and lexical classes | ECMAScript-exact `\p{…}` resolution (General_Category, binary properties, Script, Script_Extensions); ID_Start and ID_Continue under UAX #31 with Pattern_Syntax subtracted; White_Space; line terminators |
| Case folding | Simple case folding, its orbits, and the non-Unicode canonicalisation `RegExp` uses without `u` |
| Case mapping | Full upper and lower mappings with Final_Sigma; language-sensitive mapping for Turkic and Lithuanian, with Soft_Dotted from the UCD |
| Normalization | NFC, NFD, NFKC, NFKD |
| Properties of strings | `RGI_Emoji` and its components for `v`-flag classes |
| Segmentation | UAX #29 grapheme, word and sentence boundaries, and word-likeness for `Intl.Segmenter` |

**Collation is not this area.** It belongs to [SJL-17](../milestones/sjl-17-collation.md), under
decision [SJL-D-24](../decisions.md#sjl-d-24) (c).

---

## 2. Current state

### Versions

| Area | Broiler.VM | Broiler.JS | Packages |
|---|---|---|---|
| Normalization | UCD 17.0.0, pinned, generated, charged | **Host**: `string.Normalize` (ICU's version, or a no-op under invariant globalization) | none |
| Case mapping | UCD 17.0.0 including SpecialCasing | `JSStringSpecialCasing`'s one-to-many tables (identical to Broiler.VM's), plus .NET simple mappings, plus hand patches | `Broiler.Unicode.Properties` (mixed) |
| Folding (`/i`) | 17.0.0 | `u`/`v`: Broiler.Regex 17.0.0 (identical); otherwise .NET casing through the translator | Broiler.Regex's own generated data |
| `\p` sets | 17.0.0, exact names | Broiler.Unicode 17.0.0 (identical apart from `Katakana_Or_Hiragana`), loose names; .NET `\p` in the fallback | `Broiler.Unicode.Properties` |
| Properties of strings | Emoji 17.0.0 | Emoji 17.0 (identical) | `Broiler.Unicode.Emoji.StringProperties` (its README still says 16.0) |
| ID_Start, ID_Continue | 17.0.0 with Pattern_Syntax subtracted | Runtime categories, hand-written Other_ID lists and Unicode 17 patches; **no Pattern_Syntax subtraction** | none |
| Segmentation | UCD 17.0.0 auxiliary data, full UAX #29 | Graphemes from `StringInfo`; words by a heuristic; **sentences are the whole string** | none |
| .NET runtime data | not used for these areas | Unicode 16.0 on .NET 10, as Broiler.VM's JSD-0031 measured | none |

### Where the code is

**Broiler.VM's Format assembly** holds the data the compiler and the runtime both need:

- `JsUnicodeProperties` and its generated table;
- `JsUnicodeCaseFolding` and its generated table;
- `JsUnicodeStringProperties` and its generated table;
- the public `JsUnicodeLexical`;
- the matcher (SJL-11).

The reason is that Format is the one assembly both may reference.

**Broiler.VM's profile** holds what only the runtime needs:

- `JsUnicodeNormalization` and its generated table;
- `JsUnicodeCasing` and its generated table;
- the algorithms in `JsRealm.String`;
- `JsSegmenter`;
- the segmentation tables, in the optional `.Intl` assembly.

*(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): there is no type `JsSegmenter`: the file `JsSegmenter.cs` declares `JsBreakData` and `JsSegmentation`.
Soft_Dotted for the language casings is a table of the Intl assembly too)*

**How the tables are produced.** The generator is `UnicodeTableGenerator` over `UnicodeDatabase`,
reading `src/tests/unicode/pins/unicode.pin`. **Rule N22** regenerates the tables byte for byte under
a size budget. **Rule N23** forbids the platform normalizer's identifiers in the profile's sources.
*(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): the segmentation tables and Soft_Dotted are written by `CldrTableGenerator`, from the same
`UnicodeDatabase`, into the Intl assembly, and **rule N28** holds them under its own budget; N22 holds the
other five tables. Rule N23 reads the profile, the compiler and Format, not the Intl assembly)*

**The platform reads that remain in Broiler.VM** are these:

- `JsNumberFormat.IsWhiteSpace` asks `CharUnicodeInfo` for `Zs`;
- `RegExpEscapeCodePoint` uses `CharUnicodeInfo`;
- `FunctionIsNativeName` uses `char.IsLetter`;
- a few character heuristics remain in `JsParser`.

*(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): `JsNumberFormat.IsWhiteSpace` was retired at SJL-3.4, and `trim` and `Number` read SJL-3's shared
white-space set. `JsParser`'s heuristics are five routing checks over `char.IsLetter`, `char.IsWhiteSpace`
and `char.IsLetterOrDigit`, W6)*

**Broiler.JS's code** lives in:

- `JSStringPrototype.Transform` (normalization, casing);
- `Parser/CharExtensions` (identifiers);
- `FastScanner`, `NumberParser`, `JSString.trimCharacters`, `StringSpan.Trim` and `JSRegExp.Escape`
  (whitespace, in different sets); *(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): `JSString.trimCharacters` was deleted and `NumberParser`'s list
  replaced at SJL-3.5, so `trim` and `Number` read the shared set and exclude U+0085; U+0085 remains white
  space in the scanner, `RegExp.escape`, `StringSpan.Trim`, which has no caller, and the segmenter's word
  categories, W1 and W2)*
- `JSRegExp` (identifier classes and property lookups);
- `JSIntl` (segmentation; normalization for the collator).

It consumes `Broiler.Unicode.Properties` and the emoji package **only transitively**, through
Broiler.Regex. *(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): transitively by package reference, through `Broiler.Regex` `0.1.0-preview.4`, but its
code calls both packages' namespaces directly, in `JSRegExp` and `JSStringPrototype.Transform`)*

**Broiler.Unicode** has these characteristics:

- its generators download their UCD inputs at generation time, without committing or hashing them;
- the scripts its headers name are not in the repository;
- its range tables are hex-text dictionaries, parsed on first use under a global lock;
- its lookup matches names loosely;
- it returns the same mutable array to every caller;
- it has no code-point lookups;
- it has no normalization, segmentation, folding or collation;
- `JSStringSpecialCasing` falls back to `ToUpperInvariant`, `ToLowerInvariant`, `TextInfo` and
  `CharUnicodeInfo`;
- the lone-surrogate repair of commit `2affa78` shipped in preview.3.

---

## 3. Observable differences

| Case | Broiler.VM | Broiler.JS |
|---|---|---|
| `"é".normalize()` under invariant globalization | `"é"` | unchanged (the platform does nothing), as JSD-0031 §2 measured |
| `normalize` with a lone surrogate | passed through | a CLR `ArgumentException` *(reading)* |
| `'ªΣ'.toLowerCase()` | final sigma (U+00AA is Cased) | `σ` (U+00AA is not Cased in the approximation) *(reading)* |
| `toLocaleUpperCase(['tr','en'])` | Turkic mapping | `"tr,en"` reaches `CultureInfo`, which raises `RangeError` *(reading)* |
| A Turkish host running `toUpperCase()` | default mapping | default mapping, but `toLocale*` without arguments uses the host culture |
| `/\p{letter}/u`, `/\p{Script_Extensions=greek}/u` | `SyntaxError` (exact names) | accepted (loose names) |
| `/\p{sc=Hrkt}/u` | valid (empty set) | `SyntaxError` |
| U+2E2F in an identifier | refused (Pattern_Syntax) | accepted *(reading)* |
| U+0085 as whitespace | not whitespace | whitespace in several Broiler.JS paths |
| `new Intl.Segmenter("en", {granularity: "sentence"})` | UAX #29 sentences | one segment |

*(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): the rows marked *(reading)* were run in both engines and hold as stated; so do the property rows,
U+2E2F and sentences. The run found more: Broiler.JS reads Unicode 16's decompositions as nothing on a host
whose ICU carries Unicode 15.1, N2; its Turkic lower case misses the Unicode 16 and 17 pairs, C4; its
Lithuanian casing misses four Soft_Dotted code points and treats a class-0 mark as transparent, C5 and C6;
`null` reads the host culture, C8; its words split `can't`, `a_b` and `c.d`, G2; and its group names accept
U+2E2F, I3)*

---

## 4. The target

### 4.1 The owner's API

All names are working names. Under [SJL-D-6](../decisions.md#sjl-d-6) (c), the tables behind these
members come from a provider the engine supplies, holding the bytes the owner's generator produced;
how the provider is bound is settled at SJL-7.2.

```csharp
// Exact ECMAScript lookups; immutable results; a public version.
public static class EcmaUnicode
{
    public const string UnicodeVersion = "17.0.0";
    public static bool TryResolveLoneProperty(ReadOnlySpan<char> name, out CodePointSet set);           // exact ES names
    public static bool TryResolveProperty(ReadOnlySpan<char> name, ReadOnlySpan<char> value, out CodePointSet set);
    public static bool TryResolveStringProperty(ReadOnlySpan<char> name, out StringSet set);            // v flag
    public static bool IsIdentifierStart(int codePoint);  public static bool IsIdentifierPart(int codePoint);
    public static bool IsWhiteSpace(int codePoint);       public static bool IsLineTerminator(int codePoint);
    public static GeneralCategory GetGeneralCategory(int codePoint);
}
public readonly struct CodePointSet { /* immutable ranges over ReadOnlySpan; Contains; Ranges */ }

public static class CaseFolding  { public static int SimpleFold(int cp); public static ReadOnlySpan<int> Orbit(int cp);
                                   public static int NonUnicodeCanonicalize(char c); /* … */ }

public enum CaseLanguage { Default, Turkic, Lithuanian }
public static class CaseMapping
{
    public static MapStatus ToUpper(ReadOnlySpan<char> s, CaseLanguage l, long maximumLength, WorkCharge? c, out string r);
    public static MapStatus ToLower(ReadOnlySpan<char> s, CaseLanguage l, long maximumLength, WorkCharge? c, out string r);
}

public enum NormalForm { C, D, KC, KD }
public static class UnicodeNormalizer       // method names avoid the tokens Broiler.VM's rule N23 rejects
{
    public static MapStatus ToNormalForm(ReadOnlySpan<char> s, NormalForm f, long maximumLength, WorkCharge? c, out string r);
    public static bool IsInNormalForm(ReadOnlySpan<char> s, NormalForm f, WorkCharge? c);
    public static int CombiningClass(int codePoint);
}

public enum Granularity { Grapheme, Word, Sentence }
public static class Segmentation          // optional package
{
    public static void Boundaries(ReadOnlySpan<char> s, Granularity g, List<int> output, WorkCharge? c);
    public static WordStatus WordStatus(ReadOnlySpan<char> segment);
}
```

### 4.2 Two rule interactions in Broiler.VM

- **Rule N23** rejects the identifier tokens `Normalize`, `IsNormalized` and `NormalizationForm`
  anywhere in the profile's sources, so an adapter cannot call a shared `Normalize(...)` method
  without failing it. Either the shared names avoid those tokens, as sketched, or N23 is re-scoped
  in the same change. The re-scoped rule would forbid only the **platform's** normalizer, by member
  reference, which rule `I2` checks in metadata. Its vacuity guard, which requires `JsRealm.String`
  to exist, is re-pointed.
- **Rule N18** forbids `System.Text.RegularExpressions` and `new Regex(`. It is unaffected by this
  area, and SJL-11 re-points its guard.

### 4.3 What adapters may contain

- receiver and argument conversion;
- the mapping of a locale to `CaseLanguage`, through each engine's own locale canonicalisation;
- the engine's length ceiling and its `RangeError`;
- charging;
- `Intl.Segmenter`'s objects and iterators.

### 4.4 Answering JSD-0031 §4(b)

| Reason JSD-0031 gave | How SJL-7 answers it |
|---|---|
| Rules A2 and N1 forbid the package | The admission path of SJL-2 |
| Loose name resolution does not conform | The exact-name API in 4.1 |
| No decomposition or case-folding data | Normalization and folding added to the owner |
| Unreproducible provenance | Pinned, hashed inputs and an in-repository generator with a byte-for-byte rule |
| Runtime cost | Each engine's measurement under a predeclared rule, with lazy parsing of hex-text dictionaries replaced by spans over constant data |

**If any reason stands after SJL-7's evidence, the area is refused**, and the record says which
reason stood.

---

## 5. Deletion lists

**Broiler.VM:**

- the lookup code of `JsUnicodeProperties`, `JsUnicodeCaseFolding` and `JsUnicodeStringProperties`,
  and `JsUnicodeLexical`, from Format;
- the code of `JsUnicodeNormalization` and `JsUnicodeCasing`, from the profile;
- in `JsRealm.String`: `IsNormalizedAlready`, `ReadCodePoint`, `OrderCanonically`,
  `ComposeCanonically`, `WriteCodePoints`, `LanguageCasing`, `AfterCapitalI`, `AfterSoftDotted`,
  `BeforeDot`, `MoreAbove`, and the bodies of `NormalizeText` and `StringChangeCase`; *(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): `ReadCodePoint`
  was retired at SJL-5.4; `AppendCodePoint` and `TransformCase`'s body but for the locale's resolution join
  the list)*
- Soft_Dotted in the Intl data;
- `JsSegmenter`'s algorithm;
- the remaining platform classification sites;
- the generator's normalization, casing, folding, property and string-property parts, which move to
  the owner. *(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): so do the Unicode parts of `CldrTableGenerator`, the segmentation tables and
  Soft_Dotted, and the platform classification sites include the parser's routing checks, W6)*

**Broiler.VM keeps** the generated tables, including the segmentation tables, regenerated by the
owner's generator and held by rule N22 ([SJL-D-6](../decisions.md#sjl-d-6) (c)).

**Broiler.JS:**

- the platform normalization calls;
- the `JSStringSpecialCasing` usage and the `CultureInfo` resolution;
- `CharExtensions.IsIdentifierStart`, `IsIdentifierPart` and their Unicode-17 patch tables;
- the whitespace lists;
- `JSRegExp`'s identifier classes and loose property lookups;
- `JSIntl`'s segmentation.

*(Corrected 2026-10-16 at SJL-7.1, [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), rows of bundle `sjl-7-001`'s [correspondence](../../evidence/sjl-7-001/correspondence.md): also `IsValidNativeFunctionToStringName`'s `char.IsLetter`, I4, and the collator's
`string.Normalize`, N1)*

**The owner:**

- Broiler.Unicode's loose internals, once no consumer needs them;
- Broiler.Regex's own case-folding table, which becomes the owner's.

---

## 6. Risks

| Risk | Mitigation |
|---|---|
| Broiler.Unicode does not adopt the conditions | The area moves to the Broiler.JS repository (`SJL-D-1`), and the record says why |
| Size: Broiler.VM's generated Unicode tables sit close to their budget | Rule N22's budget still holds Broiler.VM's embedded tables, now in the owner's format, and Broiler.VM's closure size is measured in its bundle |
| The owner's algorithms assume tables inside its own package | SJL-7.2 item 11: tables reach the algorithms through a provider, so each engine can embed its own (`SJL-D-6` (c)) |
| Broiler.JS changes behaviour on hosts whose ICU differs | That is the point. Every change is listed as a correction. |
