# Area: number conversion (the pilot)

**Delivered by:** [SJL-3](../milestones/sjl-3-number-conversion-pilot.md). **Library:**
`Broiler.ECMAScript.Algorithms` (decision `SJL-D-2`). **Status:** design. Members are named as they
exist on 2026-10-11, at Broiler.VM `88d8b51d` and Broiler.JS `44a214dc`. Statements about what a
script would observe are **readings of the code**, marked *(reading)*. They are confirmed or refuted
by the vectors of SJL-1, not by this document.

---

## 1. Scope

| Responsibility | Specification |
|---|---|
| `Number::toString(x, radix)` for radix 10 (shortest round-trip) and radix 2–36 | ECMA-262 `Number::toString` |
| `Number.prototype.toFixed`, `toExponential`, `toPrecision` digit generation | the three built-ins' algorithms |
| `StringToNumber` and its grammar `StringNumericLiteral` | `StringToNumber` |
| `parseInt`, `parseFloat` | the two global functions |
| Numeric literal values, BigInt literal values | `NumericLiteral` static semantics (MV) |
| Array-index and canonical numeric string keys | `CanonicalNumericIndexString`, array index |
| The whitespace set used by `StringToNumber` and `TrimString` | `WhiteSpace`, `LineTerminator`, `StrWhiteSpaceChar` |

---

## 2. Current state: Broiler.VM

| Responsibility | Where | Notes |
|---|---|---|
| `Number::toString` radix 10 | `JsNumberFormat.ToJsString` → `Decompose` (uses `double.ToString("R")` in the invariant culture) → `Place`; a small-integer string cache | Pure. The class is `internal static`. Its usings are `System`, `System.Collections.Generic`, `System.Globalization` and `System.Text`, and it references no engine type. **No check that `"R"`'s digits round-trip.** |
| Compiler copy | `JsParser.NumberText`, used by `NumberKey` | Exists because the compiler may not reference the runtime profile |
| Radix 2–36 | `JsNumberFormat.ToRadixString`, `RoundUpRadix` | Stops when the remaining fraction is below half the gap to the next double; rounds half-to-even with carry |
| `toFixed`, `toExponential`, `toPrecision` | `JsRealm.Number` private statics: `NumberFixedText`, `NumberExponentialText`, `NumberPrecisionText`, `NumberRoundSignificant`, `NumberShortestDigits`, `NumberIsMidpoint`, through `NumberIncrementFixed`, with their constants | .NET `"F"`/`"E"` formats with Broiler.VM's own exact-tie detection. **Not in `JsNumberFormat`**, contrary to the feasibility report's wording. |
| `StringToNumber` | `JsNumberFormat.ToNumber`, `Trim`, `IsDecimalLiteral`, `ParseRadix`; called by `JsEngine.Conversions.ToNumberFromText` (which charges the text length first) and by JSON numbers | Radix strings accumulate in a `double`, rounding twice beyond 2^53 *(reading)* |
| `parseInt`, `parseFloat` | `JsNumberFormat.ParseInt`, `ParseFloat`; wrappers in `JsRealm.Global` | `parseInt` accumulates in a `double`, which is not correctly rounded for long inputs *(reading)*. `Number.parseInt === parseInt`; placeholder copies in `JsRealm.Number` are overwritten. |
| Literal values | `SliceTokenizer.ReadNumericLiteral`, `FinishNumeric`; BigInt values in `JsParser.TryBigIntValue` (width capped by the format) | Separators validated; legacy octal and `08`/`09` flagged; radix literals accumulate in a `double` |
| Whitespace | `JsNumberFormat.IsWhiteSpace`: the listed code points, then `CharUnicodeInfo` for `Zs` | **Follows the .NET runtime's Unicode data**, not Broiler.VM's pinned Unicode 17.0.0 |
| Metering | Callers charge before or after (`ToNumberFromText`, `JsRealm.Number` charges output length) | No meter inside the algorithms |

## 3. Current state: Broiler.JS

| Responsibility | Where | Notes |
|---|---|---|
| `Number::toString` radix 10 | `BuiltIns/Number/JSNumber.ToECMAString` (internal) and `RoundTrippable` | `"R"`, re-parsed, with a `"G17"` fallback. **G17 is round-trippable but not guaranteed shortest** *(reading)*. |
| The seam | `Runtime/JSValue.NumberToECMAString` delegate, installed by `BuiltInsAssemblyInitializer` | Exists because `Runtime` and the compiler may not reference `BuiltIns` |
| Radix 2–36 | `JSNumberPrototype.ToString`; `DecimalToBase` and `DecimalToArbitrarySystem` (both **public**) | Integer part through a `long` cast; up to a fixed cap of fraction digits without final rounding; the second routine cites a Stack Overflow answer |
| `toFixed`, `toExponential`, `toPrecision` | `JSNumberPrototype.ToFixed`, `ToExponential`, `ToPrecision`; kernels `RoundScaled`, `ToExactFraction`, `ExactSignificantDigits`, `Compare10Pow`, `ShortestSignificantDigits`, `FormatExponential` | Exact `BigInteger` rationals, ties away from zero. Coercion is inside the methods. |
| `StringToNumber` | `Runtime/NumberParser.CoerceToNumber(in StringSpan)` (**public**) → `ParseCore`, `RefineEstimate`, `ParseHex`, `ParseOctal`, `ParseBinary` | Correctly rounded decimal path; exponent capped; takes the Ast-owned `StringSpan` |
| `parseInt`, `parseFloat` | `NumberParser.ParseInt` (BigInteger refinement), `ParseFloat`; **two wrapper pairs** in `Globals/JSGlobal` and `JSNumberStatic` | Correctly rounded |
| Literal values | `Parser/NumberCoercion` (its own); BigInt literal values in `JSBigInt.TryParseBigIntLiteral` (current-culture parse) and a second copy in `FastCompiler.CreateClass.BigIntLiteralToValue`/`ParseBigIntRadix` | Long radix literals truncate through a BigInteger-to-double cast *(reading)*; an `m` suffix produces a non-standard `Decimal` token |
| Whitespace | `NumberParser.IsWhiteSpaceOrLineTerminator`; `JSString.trimCharacters`; .NET `char.IsWhiteSpace` via `string.Trim` in `JSTrim` (used by `parseInt`) and in BigInt parsing; `StringSpan.Trim` | **Several sets.** The .NET set includes U+0085, which is not ECMAScript whitespace, and excludes U+FEFF, which is. |
| Metering | None | |

**Inside Broiler.JS, number conversion therefore lives in several places**:

- the parser's `NumberCoercion`;
- the runtime's `NumberParser`;
- the built-ins' `JSNumber`, reached through a delegate.

---

## 4. Differences already visible

These are inferred by reading the code and are **to be confirmed by SJL-1's vectors**. Every entry
is a candidate *correction* (decision `SJL-D-16`), not a design choice.

| Input | Specification | Broiler.VM | Broiler.JS |
|---|---|---|---|
| `String(2**-25)` | `"2.9802322387695312e-8"` | possibly `…31e-8`, if .NET's `"R"` still misses here; Broiler.JS's own test records that it once did | correct (G17 fallback) |
| `(5).toString(0)`, `(5).toString(null)` | `RangeError` | `RangeError` | `"5"` |
| `(5).toString(NaN)` | `RangeError` | `RangeError` | a .NET `ArgumentException` |
| `(0.5).toString(3)` | an approximation is allowed; implementations stop at the precision limit | a bounded digit string | a fixed long run of digits |
| `(2**64).toString(2)` | `1` followed by sixty-four zeros | correct | a saturated `long` cast gives the wrong digits |
| `(1).toPrecision(100.5)` | allowed: the argument is truncated first | accepted | `RangeError` |
| `Infinity.toPrecision({valueOf(){throw 1}})` | throws, because the argument is converted first | throws | returns `"Infinity"` |
| `parseInt("200000000000011", 16)` | correctly rounded | double accumulation, rounded twice | correct |
| `Number("0x200000000000011")` and the literal `0x200000000000011` | correctly rounded | rounded twice | rounded twice or truncated |
| `parseInt(null, 36)` | `ToString(null)` is `"null"` | correct | `NaN` |
| `parseInt("\u00851")` | `NaN` (U+0085 is not whitespace) | `NaN` | `1` |
| `01n` | `SyntaxError` | `SyntaxError` | `1n` |
| `08` in strict code | `SyntaxError` | refused | accepted (no check found) |

---

## 5. The target

### 5.1 API sketch

The names are working names. Every input is already-converted and validated, which is the
adapter's job.

```csharp
namespace Broiler.ECMAScript.Numbers;

public static class NumberText
{
    // Number::toString(x, 10); every double, shortest round-trip, exact specification placement.
    public static string ToString(double value);
    public static bool TryFormat(double value, Span<char> destination, out int written);
    public static (string Digits, int Exponent) ShortestDigits(double finiteNonZero);

    // Number::toString(x, radix), radix already validated in 2..36.
    public static string ToRadixString(double value, int radix);

    // Digit generation for finite, validated inputs.
    public static string ToFixed(double finite, int fractionDigits);           // 0..100
    public static string ToExponential(double finite, int? fractionDigits);    // null = shortest
    public static string ToPrecision(double finite, int precision);            // 1..100
}

public static class NumberParsing
{
    public static double StringToNumber(ReadOnlySpan<char> text, WorkCharge? charge = null);
    public static double ParseInt(ReadOnlySpan<char> text, int radix, WorkCharge? charge = null); // radix 0 or 2..36
    public static double ParseFloat(ReadOnlySpan<char> text, WorkCharge? charge = null);
    public static bool IsStrWhiteSpace(char c);
    public static ReadOnlySpan<char> TrimStrWhiteSpace(ReadOnlySpan<char> text, TrimEnds ends);
}

public static class NumericLiterals
{
    // Separator-free source text of the digits; radix from the prefix; legacy octal handled.
    public static double Value(ReadOnlySpan<char> digits, LiteralRadix radix);
    public static BigIntLiteralStatus TryBigIntValue(ReadOnlySpan<char> digits, LiteralRadix radix,
                                                     long maximumBits, out BigInteger value);
}

public static class NumericKeys
{
    public static bool TryParseArrayIndex(ReadOnlySpan<char> key, out uint index);
    public static bool IsCanonicalNumericString(ReadOnlySpan<char> key, out double value);
}
```

`WorkCharge` is the meter of decision [SJL-D-10](../decisions.md#sjl-d-10). Formatting a double has
bounded work, so it takes no meter. Parsing scales with the input's length, so it does.

### 5.2 Algorithm decisions

1. **Shortest round-trip digits.** The library must be correct for every double. Two routes meet
   the requirement:
   - implement a proven shortest-digits algorithm in the library;
   - take the platform's `"R"` digits and verify them by a round-trip check, falling back to an exact
     shortest search on failure.

   Either is acceptable if the full vector population passes, including the exhaustive
   neighbourhood tests of SJL-1. A `"G17"` fallback is not acceptable.
2. **Radix conversion.** Broiler.VM's stopping rule and half-to-even rounding with carry. The
   integer part is computed exactly for every magnitude.
3. **Fixed, exponential and precision.** Exact digit generation with the specification's tie rule:
   "if there are two such sets, pick the larger". **The vectors decide** between Broiler.VM's
   format-plus-tie-correction route and Broiler.JS's exact-rational route; both may pass. The
   library keeps one.
4. **Parsing.** Correctly rounded for every literal form, including radix strings beyond 2^53. Both
   engines round such strings twice today. Build the significand exactly, then round once.
5. **Whitespace.** The specification's `WhiteSpace` and `LineTerminator` sets. The `Zs` part comes
   from a table generated from the pinned Unicode data, never from the runtime. At SJL-7,
   [rule I4](../governance.md#31-broilervm) checks that this table agrees with the Unicode owner's
   `Zs`.

### 5.3 What adapters may contain

**Broiler.VM's adapters** (`JsRealm.Number`, `JsRealm.Global`, `JsEngine.Conversions`,
`JsRealm.Json`, and the compiler's literal sites) and **Broiler.JS's** (`JSNumberPrototype`,
`JSGlobal`/`JSNumberStatic`, `NumberParser`'s public forwarders, `Parser`'s literal sites) may
contain:

- receiver checks, `thisNumberValue`;
- argument conversion in specification order, `ToIntegerOrInfinity`;
- range checks and the engine's `RangeError`;
- the non-finite short-circuits the specification places before digit generation;
- charging under the engine's policy;
- string-object creation and caches, such as Broiler.VM's small-integer strings;
- the scanning and diagnostics of literals (front-end work, until SJL-13 and SJL-14);
- Broiler.JS's non-standard extensions (decision `SJL-D-17`).

**They may contain no digit generation, no rounding, no grammar and no whitespace table.**

---

## 6. Vectors

The seed set is SJL-1.4. The population of the reference-arm comparison, declared at SJL-0.9,
contains:

- every double class: zero, subnormal, normal and the extremes, at both signs;
- powers of two and of ten, and their neighbours;
- a seeded random set;
- for `toFixed`, `toExponential` and `toPrecision`, every digit count with tie-producing inputs;
- for parsing, generated decimal strings of increasing length around rounding boundaries, and radix
  strings around 2^53 and 2^64.

---

## 7. Deletion lists

### Broiler.VM

| Member | Note |
|---|---|
| `JsNumberFormat.ToJsString`, `Decompose`, `Place`, `ToRadixString`, `RoundUpRadix`, `Value`, `Digit`, `ToNumber`, `ParseInt`, `ParseFloat`, `IsWhiteSpace`, `Trim`, `IsDecimalLiteral`, `ParseRadix`, `DigitValue` | If nothing remains in `JsNumberFormat`, the class goes |
| `JsRealm.Number`: `NumberFixedText`, `NumberExponentialText`, `NumberPrecisionText`, `NumberRoundSignificant`, `NumberShortestDigits`, `NumberIsMidpoint`, `NumberSplitRendered`, `NumberWithoutPoint`, `NumberPointAfterFirst`, `NumberExponentSuffix`, `NumberIncrementDigits`, `NumberIncrementFixed` and their constants; the placeholder `parseInt`/`parseFloat` | |
| `JsParser.NumberText` | The compiler's copy |
| `JsParser.TryBigIntValue` | The compiler's BigInt literal parser |
| The value accumulation in `SliceTokenizer.ReadNumericLiteral` | Scanning stays |

### Broiler.JS

| Member | Disposition |
|---|---|
| `Parser/NumberCoercion` (class) | Deleted |
| `NumberParser.ParseCore`, `ParseCoreStatus`, `integerPowersOfTen`, `ParseHex`, `ParseOctal`, `ParseBinary`, `IsWhiteSpaceOrLineTerminator`, `RefineEstimate`, `AddUlps`, `ScaleToInteger`, private `ParseInt`/`ParseFloat` bodies | Deleted |
| `NumberParser.CoerceToNumber`, `TryCoerceToUInt32`, `TryGetArrayIndex` (public) | **Forwarders** (`SJL-D-25`) |
| `JSNumber.ToECMAString`, `RoundTrippable` | Deleted |
| `JSValue.NumberToECMAString` (delegate) and its installation | Deleted |
| `JSNumberPrototype`: `RoundScaled`, `ToExactFraction`, `ExactSignificantDigits`, `Compare10Pow`, `ShortestSignificantDigits`, `FormatExponential` | Deleted |
| `JSNumberPrototype.DecimalToBase`, `DecimalToArbitrarySystem` (public) | **Obsolete forwarders**; the second's Stack Overflow-derived body goes regardless |
| `FastCompiler.CreateClass.BigIntLiteralToValue`, `ParseBigIntRadix`; `JSBigInt.TryParseBigIntLiteral` | Deleted |
| One of the two `parseInt`/`parseFloat` wrapper pairs | Deleted |
| `JSString.trimCharacters` | Deleted (shared predicate) |

---

## 8. Risks specific to this area

| Risk | Mitigation |
|---|---|
| The shortest-digits route chosen is slower in one engine's hot path, `String(number)` | Each engine's candidate-versus-control measurement under a rule written at SJL-0; the faster correct route is chosen; figures stay in bundles |
| A correction changes a Broiler.JS consumer's output, such as radix 0 or `parseInt(null)` | Listed in the changelog; decision `SJL-D-16` (a) by default; (c) only for a known dependent consumer |
| Broiler.VM's compiler output bytes change because literal values change | Only where a value was wrong. The artifact comparison classifies each case, and Broiler.VM's retained corpus re-bases by decision. |
| `StringSpan`-taking public members of Broiler.JS | Kept as forwarders that call `AsSpan()`. The shared API takes `ReadOnlySpan<char>`, so it never imports Broiler.JS's `Ast` type, as the feasibility report requires. |
