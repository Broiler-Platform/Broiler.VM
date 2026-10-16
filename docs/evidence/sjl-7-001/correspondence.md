<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-7.1's correspondence from code: Unicode character data and algorithms in both engines

**Written:** 2026-10-16, at SJL-7.1, as the input of
[JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md)'s
X1, X2 and X6 and of its owner conditions, before any code of SJL-7 exists. **Read at** Broiler.VM `16a35b2`,
its default branch after SJL-6's merges, whose product sources are `24632b1`'s; Broiler.JS `bdb1a3ca`, its
default branch after SJL-6's merges, whose sources under `Broiler.JS/` and `Broiler.ECMAScript/` are
`4d70821e`'s; Broiler.Unicode `0faa1f9`, whose code is `2affa78`'s, the commit its `0.1.0-preview.3` packages
were built from; and Broiler.Regex `0799342`, whose code is `c1422f4`'s, the commit its `0.1.0-preview.4`
package was built from. Broiler.JS consumes those two packages. **By reading, and by running where a row says
so.** A Broiler.VM cell marked *(run)* was run from sources built in Release at `24632b1`, in two realms:
- through the command-line root, whose manifest `broiler.javascript.wide` has no Intl;
- through the conformance root under the same manifest, whose realm has Intl.

The cell says which realm when the two differ. A Broiler.JS cell marked *(run)* was run through the shell,
`BroilerJS.dll --script-host`, built in Release from `4d70821e`'s sources on `linux-x64`. That build used .NET
10.0.12 and the host's ICU 74.2, which carries Unicode 15.1, as the hosted Ubuntu runners' does. The culture
was invariant (no `LANG`). Where a Broiler.JS behaviour comes from the .NET runtime or the host's ICU, the row
says *(host)*. Where a row describes code it did not execute, it says *(reading)*. SJL-7.3's vectors confirm or
refute each row. **It holds no figure of a run**: the values below are inputs and the answers they give.

**Paths.** In Broiler.VM, `F/` is `src/Broiler.VM.Profile.JavaScript.Format/`, `VM/` is
`src/Broiler.VM.Profile.JavaScript/`, `CP/` its `.Compiler/`, `VI/` its `.Intl/`, and `AT/` is
`src/tests/Broiler.VM.Architecture.Tests/`. In Broiler.JS, `PA/`, `RT/`, `AS/` and `BI/` are
`Broiler.JS/Broiler.JavaScript.Parser/`, `.Runtime/`, `.Ast/` and `.BuiltIns/`, and `SL/` is
`Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/`. `BU/` is Broiler.Unicode's `src/` and `BR/` is
Broiler.Regex's `Broiler.Regex/`. A line number is the line at the revision above; a bare `:n` is in the file
named last.

**Five facts of the checkouts frame every section.**

- **Broiler.VM's Unicode data is its own, pinned and generated.** `src/tests/unicode/pins/unicode.pin` names
  the UCD 17.0.0 files, the auxiliary break data and tests, the emoji 17.0 sequence files and three property
  tables of the specification, each hashed. `AT/UnicodeTableGenerator.cs` over `AT/UnicodeDatabase.cs` writes
  five generated tables. Rule N22 regenerates them byte for byte under its size budget, which they sit close
  to, from the pin (`AT/UnicodeTableGenerator.cs:60`, `AT/UnicodePinRuleTests.cs:216`). **The segmentation
  tables and the Soft_Dotted table are not among them**: `AT/CldrTableGenerator.cs:89-106, 578-631` writes
  them into the Intl assembly from the same `UnicodeDatabase`, and rule N28 holds them, also near its budget.
  Every operation that grows with its input charges fuel through `JsEngine.Charge`, and strings stop at the
  ceiling of 2^24 code units (`VM/JsRealm.String.cs:60`).
- **Broiler.JS's Unicode data is three sources at once.** Its own code reads the .NET runtime's character
  data and the host's ICU *(host)*. It consumes `Broiler.Unicode.Properties` and
  `Broiler.Unicode.Emoji.StringProperties` `0.1.0-preview.3` transitively, through `Broiler.Regex`
  `0.1.0-preview.4` (`Directory.Packages.props:16-17` names only `Broiler.Regex` and the CLDR package). Yet it
  calls both directly (`using` at `BI/RegExp/JSRegExp.cs:5, 12`, `BI/RegExp/JSRegExp.UnicodeSets.cs:7-8`,
  `BI/String/JSStringPrototype.Transform.cs:10`). It patches the gaps by hand. Nothing in the area charges or
  takes a limit. No Broiler.JS configuration that loads the built-ins runs under invariant globalization: only
  the Native AOT sample, which loads none, and the shared probe set it.
- **Neither engine's Unicode code is shared yet**, except the white space of `trim`, `Number` and `parseFloat`.
  That is the `StrWhiteSpaceChar` set SJL-3 moved into `Broiler.ECMAScript.Algorithms`, generated from the
  pinned `PropList.txt` (`SL/Numbers/StrWhiteSpace.cs:45-88`). Both engines call it: `VM/JsRealm.String.cs:1351,
  1359` and `BI/String/JSStringPrototype.Transform.cs:198-216`. Rows W2 below are therefore agreements, not
  differences.
- **Broiler.Unicode's tables are UCD 17.0.0's.** Its four property generators and Broiler.Regex's case-folding
  generator were run in memory against Broiler.VM's pinned files at SJL-7.1, writing nothing. Each reproduced
  its committed `.g.cs` byte for byte. Its committed emoji 17.0 inputs, `data/unicode/17.0/emoji-sequences.txt`
  and `emoji-zwj-sequences.txt`, have the hashes Broiler.VM's pin records for the same files. **So the owner's
  data and Broiler.VM's agree today, and each difference below is one of algorithm, lookup or host data**,
  apart from P2.
- **The regular-expression engines are SJL-11's.** Rows about `RegExp` here are about the Unicode data and
  lookups the engines call. A row whose difference lives in Broiler.JS's .NET translator or in either matcher
  is named as SJL-11's.

## 1. Properties: `\p{…}` resolution

**Broiler.VM** resolves a property by exact name.
- `F/JsRegExpMatcher.cs:2514-2551` (`ReadPropertyBody`) admits ASCII letters, digits, `_` and one `=`.
- `F/JsUnicodeProperties.cs:148-182` (`TryResolveLone`, `TryResolve`) searches generated name lists.
- `Find` and `Compare` (:212-259) compare ordinally, with no case folding and no skipping.
- A set is `JsUnicodeSet`, an index and a count into constant data (:41).
- The names are those of the specification's archived tables (`VM/docs/specification/table-*.html`),
  cross-checked against `PropertyAliases.txt` and `PropertyValueAliases.txt` (`AT/UnicodePin.cs:303-347`).

**Broiler.JS** tries Broiler.Regex first (`BI/RegExp/JSRegExp.Broiler.cs:64-100`).
- Broiler.Regex's `UnicodeCharSets.ResolveProperty` (`BR/Unicode/UnicodeCharSets.cs:178-210`) folds the name
  loosely (`Normalize`, :255-266: lower case, `_`, space and `-` dropped).
- It then calls Broiler.Unicode's `UnicodeProperties.Get*` (`BU/UnicodeProperties/UnicodeProperties.cs:24-54`).
  That folds the same way again (`Normalize`, :80-91) and parses hex-text tables on first use under one global
  lock (:15, :57-78, :93-141).
- It hands every caller the same cached array (:68-69, :75-76).
- What Broiler.Regex cannot parse goes to Broiler.JS's .NET translator: `TranslateUnicodeProperty`
  (`BI/RegExp/JSRegExp.cs:3697-3781`), with its own `NormalizeKey` (:3702-3712).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| P1 | `/\p{letter}/u`, `/\p{Script_Extensions=greek}/u`, `/\p{GC=Lu}/u`, `/\p{lu}/u`, `/\p{rgiemoji}/v` *(run)* | `SyntaxError` | accepted | **correction of Broiler.JS**: UnicodeMatchProperty and UnicodeMatchPropertyValue admit the tables' names and aliases exactly and "must not support any other property names or aliases" |
| P2 | `/\p{sc=Hrkt}/u`, `/\p{Script=Katakana_Or_Hiragana}/u` *(run)* | valid, an empty set (`F/JsUnicodeProperties.cs:132-134`) | `SyntaxError` | **correction of Broiler.JS**: `Hrkt` is a value of Script in `PropertyValueAliases.txt` and in the specification's table. Broiler.Unicode's `generate-scripts.py` emits keys only for scripts that have code points (`src/UnicodeProperties/tools/generate-scripts.py:196-203`), and Hrkt has none in `Scripts.txt` or `ScriptExtensions.txt` |
| P3 | a lone name or a `Name=Value` that Broiler.Regex refuses and .NET admits | `SyntaxError` | left verbatim for .NET's `\p` (`BI/RegExp/JSRegExp.cs:3742-3743, 3778-3780`) *(reading)* | **correction of Broiler.JS** (as P1). Not reached by `/\p{IsLatin}/u`, which Broiler.Regex refuses first *(run)* |
| P4 | General_Category inside a `v`-mode set operation, when Broiler.Regex declines the pattern | resolved | `SyntaxError`: `ResolvePropertyOperand` takes only `sc`, `scx` and binary properties (`BI/RegExp/JSRegExp.UnicodeSets.cs:419-457`) *(reading)* | **correction of Broiler.JS**. `/[\p{gc=Lu}--\p{Script=Latin}]/v` is valid in both *(run)*, because Broiler.Regex takes it |
| P5 | the lookup's result | an immutable index into constant data | a mutable array shared by every caller *(reading)* | no observable difference, since neither engine writes to it; the owner's results must be immutable (milestone item 3) |
| P6 | `/\p{Lu}/u`, `/\p{General_Category=Lu}/u`, `/\p{gc=LC}/u`, `/\p{Lowercase_Letter}/u`, `/\p{Any}/u`, `/\p{Script=Zzzz}/u`, `/\p{Script_Extensions=Unknown}/u`, `/\p{scx=Grek}/u` on U+0342, `/\p{scx=Hira}/u` on U+30FC, `/\p{ID_Start}/u` on U+2118 *(run)* | as Broiler.JS | as Broiler.VM | agreement; `\p{ASCII=Y}` and `\p{IsLatin}` are `SyntaxError` in both |

## 2. Lexical classes: identifiers, white space and line terminators

**Broiler.VM** classifies identifiers with the public `JsUnicodeLexical` (`F/JsUnicodeLexical.cs:47-79`).
- `IsIdentifierStart` and `IsIdentifierPart` read ID_Start and ID_Continue from `DerivedCoreProperties.txt`.
  The generator refuses that file unless it equals the UAX #31 derivation, Pattern_Syntax and
  Pattern_White_Space subtracted (`AT/UnicodeDatabase.cs:566-602`).
- `IsWhiteSpace` is the WhiteSpace production: TAB, VT, FF, ZWNBSP, SP, NBSP and every Zs above U+00FF.
- The tokenizer (`CP/SliceTokenizer.cs:822, 2145, 2242-2245, 2507-2531`) and the matcher's group names
  (`F/JsRegExpMatcher.cs:1747, 1752`) call it.

**Broiler.JS** classifies identifiers in `PA/CharExtensions.cs:100-170`.
- It accepts `$`, `_` and the Other_ID_Start and Other_ID_Continue code points by hand.
- Otherwise it reads the runtime's categories, `char.GetUnicodeCategory` *(host)*.
- Then it reads two Unicode 17 patch tables taken from test262's identifier tests (:172-208).
- The scanner's white space is `char.IsWhiteSpace` or U+FEFF (`PA/FastScanner.cs:376-386`) *(host)*.

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| I1 | `var ⸯ = 1` (U+2E2F, Lm and Pattern_Syntax), literal and as `ⸯ` *(run)* | `SyntaxError` | accepted | **correction of Broiler.JS**: IdentifierStartChar is ID_Start, which subtracts Pattern_Syntax (UAX #31) |
| I2 | identifiers of Unicode 14, 16 and 17 (U+0870, U+16D40, U+11B60), and U+2118, U+1885, U+00B7, U+200C *(run)* | accepted | accepted | agreement at Unicode 17.0, but **from different data**: Broiler.JS's classes follow the runtime's data plus hand lists and patches, so a runtime update changes them *(reading)*; under SJL-D-6 the pinned data replaces them, class C5 |
| I3 | `/(?<ⸯx>.)/` *(run)*; a group name with a Unicode-17-only letter *(reading)* | `SyntaxError`; accepted | valid; `SyntaxError` | **correction of Broiler.JS**: `IsRegExpIdentifierStart` and `IsRegExpIdentifierPart` (`BI/RegExp/JSRegExp.cs:2028-2052`) copy the identifier tests without Pattern_Syntax and without the Unicode 17 tables, so an identifier and a group name disagree |
| I4 | a native function's name in `Function.prototype.toString` | `FunctionIsNativeName` (`VM/JsRealm.Function.cs:322-356`) asks `char.IsLetter` and `char.GetUnicodeCategory` | `IsValidNativeFunctionToStringName` (`BI/BuiltInsAssemblyInitializer.cs:859-879`) asks `char.IsLetter` and `char.IsLetterOrDigit` | no observable difference: every native name is ASCII. **Both are platform classification** that X5 refuses in the area, and both become the owner's lookups |
| W1 | `1\u0085+1` as source; `RegExp.escape(" \u0085　")` *(run)* | `SyntaxError`; `\x20` + U+0085 + `　` | accepted; `\x20\x85　` | **correction of Broiler.JS**: U+0085 is Cc and in neither WhiteSpace nor LineTerminator. It is white space to Broiler.JS's scanner, to `RegExp.escape` (`BI/RegExp/JSRegExp.cs:124`), to `StringSpan.Trim` (`AS/Misc/StringSpan.cs:117-170`, no caller at this revision) and to `Intl.Segmenter`'s word categories (`BI/Intl/JSIntl.cs:3313`), all `char.IsWhiteSpace` or `Rune.IsWhiteSpace` *(host)* |
| W2 | `"\u0085".trim()`, `"᠎".trim()`, `"﻿　  x".trim()`, `Number("\u00851")`, `parseInt("᠎1")` *(run)* | as Broiler.JS | as Broiler.VM | agreement: both call the shared `StrWhiteSpaceChar` set of SJL-3 |
| W3 | `/\s/` on U+0085, U+180E and U+FEFF; `1᠎+1` and `1﻿+1` as source *(run)* | false, false, true; `SyntaxError`, accepted | the same | agreement, from three hand-written copies of one set: Broiler.VM's `SpaceRanges` (`F/JsRegExpMatcher.cs:589-593`), the translator's literal (`BI/RegExp/JSRegExp.cs:4632`) and Broiler.Regex's (`BR/Unicode/UnicodeCharSets.cs:75-88`). The matchers' copies are SJL-11's |
| W4 | `RegExp.escape(" ﻿ ")` *(run)* | ` ﻿\xa0` | the same | agreement. Broiler.VM's `RegExpEscapeCodePoint` (`VM/JsRealm.RegExp.cs:1508-1556`) asks `CharUnicodeInfo` for Zs, BMP only, which is platform classification |
| W5 | line terminators | hard-coded in each reader (`CP/SliceTokenizer.cs:2459`, `F/JsRegExpMatcher.cs:1987, 4166-4167`, `CP/JsParser.cs:6793, 7540, 7572, 7729, 7740`, `VM/JsRealm.RegExp.cs:1547, 1856-1862`) | hard-coded (`PA/CharExtensions.cs:87-92`, `PA/FastScanner.cs:291-301, 764-777, 1333-1338, 1548-1624`) | agreement on the set. The translator's multiline `^` and `$` on its global path add only `\r` to .NET's `\n` (`BI/RegExp/JSRegExp.cs:921-973`) *(reading)*, which is SJL-11's |
| W6 | `CP/JsParser.cs:855, 1387, 6225, 7639, 7761` | `char.IsLetter`, `char.IsWhiteSpace` and `char.IsLetterOrDigit` in five routing checks of the parser and its template reader | - | no observable difference found: each routes a construct the tokenizer then reads or refuses *(reading)*. **They are platform classification** in the compiler, and become the owner's lookups |

## 3. Case folding

**Broiler.VM**: `JsUnicodeCaseFolding` (`F/JsUnicodeCaseFolding.cs:57-302`).
- `SimpleFold` (:62-67) reads `CaseFolding.txt`'s rows of status C and S.
- `Orbit` (:80-132) writes into the caller's span.
- `SimpleUpper` (:156-161) is the canonicalisation without `u` or `v`: the full upper-case mapping, kept
  where the result is one unit, the source is no surrogate and no non-ASCII source maps into ASCII
  (`AT/UnicodeTableGenerator.cs:326-358`).
- The matcher calls it (`F/JsRegExpMatcher.cs:126-150`).

**Broiler.JS**:
- Under `u` and `v`, Broiler.Regex's `CaseFolding` (`BR/Unicode/CaseFolding.cs:57-111`) runs over
  `CaseFoldingData.g.cs`, the same two derivations with the same guards.
- Without `u`, when Broiler.Regex declines, the translator expands literals into classes from .NET's casing
  (`BI/RegExp/JSRegExp.cs:4729-4866, 4994-5161`). It uses no `RegexOptions.CultureInvariant` *(host)*.

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| F1 | `/\w/iu` on U+017F, `/[a-z]/iu` on U+212A, `/ΐ/iu` on U+1FD3, `/ß/i` and `/ß/iu` on U+1E9E, `/\u{10400}/iu` on U+10428, `/σ/i` on U+03C2 *(run)* | true, true, true, false, true, true, true | the same | agreement, **and the same data**: Broiler.Regex's folding table, regenerated from Broiler.VM's pin, is byte-identical to its committed one, and both are the derivation above |
| F2 | `/[a-z]/i` on U+212A *(run)* | false | true | **correction of Broiler.JS**: Canonicalize without `u` maps U+212A to itself and `k` to U+004B, so they differ. The translator's .NET casing folds them; the translator is SJL-11's, and this correction lands when it retires |
| F3 | an orbit | written into a span the caller owns | a shared array, built lazily under a lock (`BR/Unicode/CaseFolding.cs:209-222`) *(reading)* | no observable difference; a contract condition for the owner (immutable results) |

## 4. Case mapping

**Broiler.VM**: `JsUnicodeCasing` (`VM/JsUnicodeCasing.cs:44-192`) with its generated table.
- The table holds `UnicodeData.txt`'s simple mappings, overlaid with `SpecialCasing.txt`'s unconditional lines,
  and Cased and Case_Ignorable from `DerivedCoreProperties.txt` (`AT/UnicodeDatabase.cs:651-741`).
- `StringChangeCase` (`VM/JsRealm.String.cs:1090-1136`) applies Final_Sigma through `IsFinalSigma`.
- `TransformCase` (:1145-1157) canonicalizes the locale list and takes its first entry or `en-US`. It matches
  `az`, `lt` and `tr` by prefix, the Unicode extension removed. It reads no host culture.
- `LanguageCasing` (:1166-1338) applies the Turkic and Lithuanian rules before the default mapping, with
  After_I, After_Soft_Dotted, Before_Dot and More_Above. Soft_Dotted comes from the Intl assembly's table
  (`VI/JsCldrTables.g.cs:10312-10315`, `VM/JsIntlTables.cs:194-219`).
- A realm without Intl ignores the locale (:1147-1150).

**Broiler.JS**: `JSStringPrototype.Transform` (`BI/String/JSStringPrototype.Transform.cs:128-178`) calls
Broiler.Unicode's `JSStringSpecialCasing` (`BU/UnicodeProperties/JSStringSpecialCasing.cs`).
- The locale is `CultureInfo.GetCultureInfo(locale.ToString())`, or the host's `CultureInfo.CurrentCulture` for
  `null` and `undefined`.
- The class's tables hold the one-to-many mappings, hand patches for Unicode 15.1 to 17 and the astral runs
  (:21-80, :644-756).
- Every other mapping is the runtime's `ToUpperInvariant`, `ToLowerInvariant` or `TextInfo` (:590-642,
  :147-160) *(host)*.
- Cased is Lu, Ll or Lt, and Case_Ignorable is Mn, Me, Cf, Lm or Sk plus a hand list, both from
  `CharUnicodeInfo` (:97-132) *(host)*.
- The language comes from `TwoLetterISOLanguageName` (:241-245, :523).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| C1 | `'ªΣ'.toLowerCase()`, `'ºΣ'.toLowerCase()` *(run)* | `ªς`, `ºς` | `ªσ`, `ºσ` | **correction of Broiler.JS**: U+00AA and U+00BA are Cased (Other_Lowercase), so Σ is final (Final_Sigma, `SpecialCasing.txt`) |
| C2 | Final_Sigma across `'`, `.`, U+00B7 and U+02B0: `'ΑΣ\'Α'`, `'ΑΣ.Α'`, `'ΑΣ·Α'`, `'ΑΣʰ'` *(run)* | σ, σ, σ, ς | the same | agreement here; Broiler.JS's Case_Ignorable is an approximation of `DerivedCoreProperties.txt`'s *(reading)* and is replaced with it |
| C3 | default mappings of Unicode 16 and 17: U+1C89, U+A7CB, U+A7CE, U+A7CF, U+A7DC, U+019B, U+10D50; and `ß`, `İ`, `ǅ`, `ŉ`, U+1F80, U+10400, U+1E030 *(run)* | the specification's | the same | agreement at 17.0, **through hand patches over the runtime's data** *(host)*; class C5 under SJL-D-6 |
| C4 | `'Ɤ'.toLocaleLowerCase('tr')`, `'꟎'.toLocaleLowerCase('tr')` *(run)* | U+0264, U+A7CF | U+A7CB, U+A7CE | **correction of Broiler.JS**: the Turkic lower-case path maps every other code point with `ToLowerInvariant` and skips the class's table (`BU/UnicodeProperties/JSStringSpecialCasing.cs:494`), so the runtime's older data shows |
| C5 | `'\u{1DF1A}̇'.toLocaleUpperCase('lt')` *(run)* | U+1DF1A | U+1DF1A U+0307 | **correction of Broiler.JS**: U+1DF1A is Soft_Dotted (`PropList.txt`), and its hand set (:527-534) lacks U+1DF1A, U+1E04C, U+1E04D and U+1E068 |
| C6 | `'Iः̀'.toLocaleLowerCase('lt')` *(run)* | `i` U+0903 U+0300 | `i` U+0307 U+0903 U+0300 | **correction of Broiler.JS**: More_Above requires no intervening character of combining class 0 or 230, and U+0903 is class 0; its `IsContextTransparent` reads "a mark not of class 230" (:411-418). Classes 216, 7 and 232 agree *(run)* |
| C7 | `'i'.toLocaleUpperCase(['tr','en'])` *(run, Intl realm)* | `İ` | `RangeError` | **correction of Broiler.JS**: ECMA-402's TransformCase canonicalizes the list and takes its first element; `"tr,en"` reaches `CultureInfo` |
| C8 | `'i'.toLocaleUpperCase(null)` *(run, Intl realm)*; no argument on a Turkish host *(reading)* | `TypeError`; the default mapping | `I`; the host culture's mapping | **correction of Broiler.JS**: CanonicalizeLocaleList(null) throws, and with no argument the default locale is the engine's, not the host's culture. "Casing no longer depends on the host culture" |
| C9 | `'i'.toLocaleUpperCase('tr')`, `('az')`, `('TR')`, `('tr-u-co-search')`, `()`, `('x!')`; the Turkic and Lithuanian conditions of C3 to C6 elsewhere *(run, Intl realm)* | as Broiler.JS | as Broiler.VM | agreement |
| C10 | the same in a realm without Intl *(run, command-line root)* | the default mapping, the argument unread | - | Broiler.VM's policy: without ECMA-402 the locale-sensitive result of `toLocaleUpperCase` and `toLocaleLowerCase` is the implementation's. It stays the adapter's |
| C11 | lone surrogates: `'\uD800a'.toUpperCase()`, `'\uDC00Σ'.toLowerCase()` *(run)* | passed through | the same | agreement since Broiler.Unicode `2affa78` |
| C12 | the ceiling | 2^24, a `RangeError`, checked only after a mapping is appended, and not at all in `LanguageCasing` *(reading)* | none: no limit, no meter | Broiler.VM's limit becomes the owner's maximum result length, checked on the whole result, which **corrects Broiler.VM's own enforcement**; Broiler.JS passes its string ceiling (an engine policy) |
| C13 | the charge | the input's length, plus each mapping's length in code points, plus the units `IsFinalSigma` scans; `LanguageCasing` a further input length (`VM/JsRealm.String.cs:973-974, 1090-1238`) | none | Broiler.VM's units (decision 4 of the verdict) |

## 5. Normalization

**Broiler.VM**: `JsUnicodeNormalization` (`VM/JsUnicodeNormalization.cs:25-334`) with its generated table.
- The table holds combining classes, canonical and compatibility decompositions, primary composites and the
  NFC and NFKC quick checks. Hangul is algorithmic.
- `NormalizeText` (`VM/JsRealm.String.cs:687-759`) runs the quick check, counts and checks the ceiling, then
  charges, decomposes, orders canonically and composes. It calls `IsNormalizedAlready`, `OrderCanonically`,
  `ComposeCanonically` and `WriteCodePoints` (:772-962).
- Since SJL-5.4 it reads code points through the shared `Utf16.CodePointAt`.
- `localeCompare`'s fallback and the collator call it too (:653-654, `VM/JsCollator.cs:119`).

**Broiler.JS**: `normalize` (`BI/String/JSStringPrototype.Transform.cs:16-33`) calls the host's
`string.Normalize`. The collator calls `IsNormalized` and `Normalize` (`BI/Intl/JSIntl.cs:6345-6348`) *(host)*.

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| N1 | `'\uD800'.normalize()`, `'a\uDC00́'.normalize('NFD')` *(run)* | passed through: U+D800; `a` U+DC00 U+0301 | `Error`, the runtime's `ArgumentException` *(host)* | **correction of Broiler.JS**: `String.prototype.normalize` normalizes the string's code points, and a lone surrogate is one, of class 0, without a decomposition. `localeCompare` throws the same way through the collator *(reading)* |
| N2 | Unicode 16 decompositions: `'\u{105C9}'` and `'\u{11383}'` to NFD; `'\u{1611E}\u{1611E}'` and `'\u{105D2}̇'` to NFC *(run)* | decomposed and composed | unchanged *(host: ICU 74.2 carries Unicode 15.1)* | **correction of Broiler.JS** under SJL-D-6, class C5: "the latest Unicode Standard" of `String.prototype.normalize` is read as the pinned 17.0.0 |
| N3 | any string under invariant globalization *(reading)* | normalized | unchanged: the runtime's `Normalize` does nothing there *(host)* | **correction of Broiler.JS**: "normalization no longer depends on the host's ICU" |
| N4 | `é` to NFD, `e` U+0301 to NFC, U+FB01 to NFKC, Hangul both ways, U+0344, U+1E9B U+0323 to NFKC, `q` U+0307 U+0323, U+FDFA to NFKD, U+1D15E, U+0F73, U+16D67 *(run)* | the specification's | the same | agreement |
| N5 | `'a'.normalize('nfc')` *(run)* | `RangeError` | `RangeError` | agreement: the form is checked before the text |
| N6 | the ceiling | the **decomposition** must fit 2^24 (`VM/JsRealm.String.cs:721`), so an NFC or NFKC result that would fit can be refused *(reading)* | none | Broiler.VM's limit becomes the owner's maximum result length on the result, which **corrects Broiler.VM's own enforcement** |
| N7 | the charge | the input's length; then, unless already normalized, the input's length plus three units per code point of the decomposition | none | Broiler.VM's units (decision 4) |

## 6. Properties of strings

**Broiler.VM**: `JsUnicodeStringProperties` (`F/JsUnicodeStringProperties.cs:37-97`), generated from the emoji
17.0 sequence files the pin names. The matcher reads it under `v` only (`F/JsRegExpMatcher.cs:2776-2810,
2950-3030`). `Collect` builds a fresh set of strings on every call, at compile time.

**Broiler.JS** reads Broiler.Unicode's `EmojiStringProperties.GetSequences` (data `17.0`; its README says
16.0) through two copies of one loose name map:
- `BI/RegExp/JSRegExp.cs:3437-3446`;
- Broiler.Regex's `StringPropertyNames` (`BR/Unicode/UnicodeCharSets.cs:303-312`).

`GetSequences` allocates its list and every string on every call (`BU/UnicodeEmoji.StringProperties/Internal/EmojiTrie.cs:101-131`).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| S1 | `/^\p{RGI_Emoji}$/v` on 👍🏽, `/^\p{Basic_Emoji}$/v` on ⌚, `/\p{RGI_Emoji}/u`, `/\P{RGI_Emoji}/v` *(run)* | true, true, `SyntaxError`, `SyntaxError` | the same | agreement, **from the same data**: both sequence files equal by hash |
| S2 | `/\p{rgiemoji}/v` *(run)* | `SyntaxError` | valid | **correction of Broiler.JS**, P1's |
| S3 | the name map | one exact table (`F/JsUnicodeStringProperties.g.cs:694-709`) | two loose copies | a duplicate of the area; the map moves to the owner (milestone item 8) |

## 7. Segmentation

**Broiler.VM**:
- `JsSegmentation` (`VM/JsSegmenter.cs:182-612`) implements UAX #29:
  - graphemes GB3 to GB13, with GB9c and GB11 (:248-331);
  - words WB3 to WB16 (:348-457);
  - sentences SB3 to SB11 (:474-583);
  - word-likeness from Word_Break and Ideographic or Hiragana (:592-612).
- It runs over the break properties `JsBreakData` decodes (:76-158) from the Intl assembly's tables.
- It applies no locale tailoring and no dictionary (JSD-0050).
- The objects are in `VM/JsRealm.Segmenter.cs:55-282`.

**Broiler.JS** (`BI/Intl/JSIntl.cs:3116-3354`):
- graphemes come from `StringInfo.GetTextElementEnumerator` (:3236-3244) *(host)*;
- words from a heuristic over the graphemes' first runes (:3257-3343);
- sentences are the whole string (:3246-3255);
- word-likeness is "contains a letter or a digit" (:3345-3354).

| Row | Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|
| G1 | sentences of `One. Two? Three! Mr. Smith` and `Hello world. 世界。終わり` *(run)* | `One. `, `Two? `, `Three! `, `Mr. `, `Smith`; `Hello world. `, `世界。`, `終わり` | one segment each | **correction of Broiler.JS**: UAX #29's sentence rules ("sentence segmentation follows UAX #29") |
| G2 | words of `can't`, `a_b c.d 3,000` and `日本語のテキスト` *(run)* | `can't`; `a_b`, `c.d`, `3,000`; each ideograph, `の`, `テキスト` | `can`, `'`, `t`; `a`, `_`, `b`, `c`, `.`, `d`, `3,000`; one segment | **correction of Broiler.JS**: UAX #29's word rules (WB6, WB7, WB13a, WB13b, and WB13 for Katakana), without a dictionary |
| G3 | graphemes of `é👍🏽🇩🇪`, a Hangul syllable sequence, `\r\n`, `\uD800a` *(run)* | as Broiler.JS | as Broiler.VM | agreement here; Broiler.JS's graphemes follow the runtime's data *(host)*, class C5 under SJL-D-6 |
| G4 | `isWordLike` | Word_Break ALetter, Hebrew_Letter, Numeric or Katakana, or Ideographic or Hiragana | any letter or digit | agreement on G2's segments where both segment alike; the rule differs (a segment `_`, for one) *(reading)*, and Broiler.JS's becomes the owner's word status |
| G5 | the charge | the pass charges one unit per sixteen code points (`VM/JsSegmenter.cs:203`). Several scans are not charged, though JSD-0050 says every scanned code point is (:168-169): the regional-indicator back-walk (:443-453), the Sp and Close back-walks of SB8 to SB11 (:537-546), the word-likeness scan and the segments' copies *(reading)* | none | **a defect of Broiler.VM's own charging**, corrected in the owner's segmenter by decision 4 of the verdict |
| G6 | the locale | resolved, without effect (`VM/JsRealm.Segmenter.cs:179-197`) | the same (`BI/Intl/JSIntl.cs:3076-3085`) | agreement; the specification leaves tailoring to the implementation |
| G7 | `containing` and `[Symbol.iterator]` | on %SegmentsPrototype% | own properties of each Segments object (`BI/Intl/JSIntl.cs:3131-3146, 3191-3210`) | outside the area: the objects stay in each engine's adapter |

## 8. The owner today

**Broiler.Unicode** at `0faa1f9` publishes three packages for `net8.0` and `net10.0`, `0.1.0-preview.2` and
`0.1.0-preview.3`. The CLDR one is not this area's (SJL-D-22). Against the four conditions of SJL-7.1:

- **The programme's conventions: not adopted.**
  - `ImplicitUsings` is enabled (`Directory.Build.props`) and the libraries target `net8.0` beside
    `net10.0`.
  - The lookup matches names loosely and hands out shared mutable arrays, under one global lock.
  - `JSStringSpecialCasing` reads `CultureInfo`, `TextInfo` and `CharUnicodeInfo`.
  - No operation takes a meter or a limit.
  - The properties package states no data version; the emoji package's version is a property, not a constant.
  - No project declares `IsAotCompatible` or `IsTrimmable`. No rule of the `SL` shape exists, and no probe.
  - The emoji assembly is named `UnicodeEmoji.StringProperties`, which the `Broiler.Unicode.` prefix of
    Broiler.JS's rule SL1 does not admit.
- **A roadmap: met.** `docs/roadmap.md` was filed at SJL-0 (`6c4baf7`) and merged (`0faa1f9`). Its items U1 to
  U10 are SJL-7.2's.
- **An assurance gate: not adopted.**
  - No annotation, generated artefact or gate exists.
  - `HUMAN_REVIEW.md` approves `89f134ed`, sixteen commits before the code the packages were built from.
- **Pinned and hashed generator inputs: not adopted.**
  - The generators download their inputs without a hash. The emoji inputs are committed, unhashed.
  - The generated headers name scripts that are not in the repository.
  - No test regenerates anything.

The bytes of the tables they generate are nonetheless UCD 17.0.0's, as the frame says.

**Broiler.Regex** at `0799342`:
- it carries the case-folding table SJL-7 makes the owner's (F1), byte-identical to its regeneration from
  Broiler.VM's pin;
- it carries a loose copy of the string-property name map (S3);
- it resolves `\p{…}` loosely through Broiler.Unicode (P1);
- it depends on Broiler.Unicode `0.1.0-preview.3`.

Its annotation on `ResolveProperty` names `\p{GC=Lu}` resolving as what would falsify the unit
(`BR/Unicode/UnicodeCharSets.cs:176`). It does resolve.

## 9. The design's readings

`areas/unicode.md` and the milestone are corrected beside each reading, which is quoted there.

- **`JsNumberFormat.IsWhiteSpace`** was retired at SJL-3.4 (`AT/retired-members.json:98`). **`ReadCodePoint`**
  was retired at SJL-5.4 (:428), and the normalization passes read code points through the shared
  `Utf16.CodePointAt`. **`JSString.trimCharacters`** was deleted at SJL-3.5. **`NumberParser`** no longer has
  a whitespace list of its own: `trim`, `Number` and `parseFloat` read the shared set in both engines (W2).
- **The segmentation tables and Soft_Dotted** are written by `CldrTableGenerator` and held by rule N28, in the
  Intl assembly, not by `UnicodeTableGenerator` and N22.
- **There is no type `JsSegmenter`.** The file `VM/JsSegmenter.cs` declares `JsBreakData` and `JsSegmentation`.
- **Broiler.VM's remaining platform classification** is `FunctionIsNativeName` (I4), `RegExpEscapeCodePoint`
  (W4), the parser's five routing checks (W6), and the compiler's and the realm's ASCII-only invariant
  casing. The last stays: its input is validated as ASCII first, or is a Temporal designator or a locale tag
  (`VM/JsTemporalParser.cs:1067`, `VM/JsRealm.Locale.cs:334, 385`, `VM/JsLocaleTag.cs:106, 223, 382, 784`).
  Two of them lower a tag before validating it, which is SJL-9's.
- **Broiler.JS's white space** has U+0085 in the scanner, `RegExp.escape`, `StringSpan.Trim` (no caller), the
  segmenter's word categories and the collator's tie-break (W1). `trim` and `Number` exclude it already (W2).
  A fourth hand-written `\s` list is the translator's (W3).
- **Broiler.JS's group names** do not share the identifier tables (I3).
- **Broiler.JS's `\p` lookup is loose twice**, and an unknown name can reach .NET's `\p` (P1, P3).
- **The collator's normalization** throws on a lone surrogate as `normalize` does (N1).
- **Broiler.JS's package reference** to Broiler.Unicode is transitive, through Broiler.Regex `0.1.0-preview.4`.
  Its code calls both packages' namespaces directly.
- **Rule I1 refuses an admitted package in Format** (witness `I1-admitted-package-in-the-format.csproj.witness`).
  Format holds the lookups the compiler and the runtime both read, so SJL-7.4 widens the allow-list for
  Format.
- **Rule N23 does not read the Intl assembly**, and **rule I4 has no register row or test yet**.
- **JSD-0027 says the `tr`, `az` and `lt` tailorings are not built** (:462-463); they are (C9).
- **Four comments in Broiler.VM** say that 27 Greek letters map to their title case because `SpecialCasing.txt`
  is not archived: `F/JsUnicodeCaseFolding.cs:36-46`, `F/JsRegExpMatcher.cs:96-99, 542-545`, and the
  generated summaries `F/JsUnicodeCaseFolding.g.cs:32, 563`. The generator and the data contradict them; the
  files they sit in leave at SJL-7.4.
