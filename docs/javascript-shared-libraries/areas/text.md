# Area: text algorithms: whitespace, UTF-16 helpers, substitution, URI, base64 and hex, JSON lexing

**Delivered by:** [SJL-5](../milestones/sjl-5-text-algorithms.md). **Library:**
`Broiler.ECMAScript.Algorithms`. **Status:** design. Observations marked *(reading)* are to be
confirmed by vectors.

---

## 1. Scope

| Sub-area | Shared | Stays in the engines |
|---|---|---|
| Whitespace and UTF-16 | `TrimString`'s classification, well-formedness, code-point reading | Receiver conversion |
| Padding and indices | A padding builder under an explicit length policy; relative-index normalisation | The length ceiling as engine policy, and its error |
| Substitution | **Parsing** a replacement template into parts | **Evaluating** the parts, because named captures are read with `Get`, which can run guest code |
| URI | `Encode`, `Decode`, `Escape`, `Unescape` with status and error position | `ToString`, `URIError`, charging |
| Base64 and hex | Codecs that write into a caller's span and report read and written counts; alphabet and last-chunk handling as parameters | Option reading, receiver validation, buffer bounds and detachment, errors |
| JSON lexing | A token and event reader over a span with a depth limit and a meter; `QuoteJSONString`; gap normalisation; a validator | Object construction, the reviver, source-text records, the raw-JSON brand, the `JSON.stringify` driver (`toJSON`, the replacer, proxies, cycle detection) |

**String search** (`indexOf`, `includes`, `startsWith`, `endsWith`) **is not shared**: the ordinal
base class library calls are the algorithm. Broiler.JS's culture-sensitive calls are corrected in
Broiler.JS itself.

---

## 2. Current state and differences

| Responsibility | Broiler.VM | Broiler.JS | Differences *(reading)* |
|---|---|---|---|
| Well-formedness | `JsRealm.String.StringWellFormed`; `toWellFormed` loop | `JSStringPrototype.Transform.IsWellFormedUtf16`, `ToWellFormedUtf16` | Equivalent |
| `trim` | `StringTrimEnds` with `JsNumberFormat.IsWhiteSpace` | `trimCharacters` with `string.Trim(char[])` | Equal sets today; other Broiler.JS paths use .NET `char.IsWhiteSpace` (U+0085 included, U+FEFF excluded) |
| `padStart`/`padEnd`, `repeat` | `StringPadTo`, `repeat`, with a string-length ceiling raising `RangeError` | `StringPad` with no limit and an `int` cast; `repeat` refuses above `int.MaxValue` | `"x".padEnd(2**40)` raises a .NET exception in Broiler.JS |
| `GetSubstitution` | `StringExpandDollars`, an inline copy in `RegExpReplaceText`, and `RegExpSubstitute` | `JSString.GetSubstitution`, and a local copy in `BuiltInsAssemblyInitializer` | Equivalent apart from details; three copies in Broiler.VM, two in Broiler.JS |
| `split` by string | `JsRealm.String` `split`; `RegExpSplitText` | `JSStringPrototype.Pattern.Split` | `"abc".split("", 2)` gives three items in Broiler.JS: the empty-separator branch ignores the limit |
| URI | `JsRealm.Global` sets, `GlobalUriEncode`/`Decode`, `GlobalEscapeText`/`GlobalUnescapeText`; charged | `Runtime/UriHelper` (cites a third-party engine) | None found |
| Base64, hex | `JsRealm.Base64` `Base64Encode`, `Base64Decode`, `Base64DecodeHex` (pure apart from a charge; return bytes, read count and error) | `JSUInt8Array.DecodeBase64` reads options itself and builds exceptions eagerly; `SetFromHex` parses the whole input before honouring the destination length | `setFromHex("aaZZ")` into a one-byte array: the specification and Broiler.VM report two read and one written; Broiler.JS writes one byte, then throws. Broiler.JS ignores a non-object options argument. |
| `JSON.parse` lexing | A hand-written reader in `JsRealm.Json`; charged per character; nesting refused past a fixed depth; lone surrogates preserved | `System.Text.Json` deserialisation with default options; exceptions mapped to `SyntaxError` | Broiler.JS rejects documents nested past System.Text.Json's default depth; refuses an escaped lone surrogate; lets a raw lone surrogate raise an uncaught .NET exception; probably refuses `1e400`, which should be `Infinity` |
| `QuoteJSONString`, gap | `JsonQuote`, `JsonApplySpace`, `JsonJoin` | `JSJSON.QuoteString(in StringSpan)`, gap through `IndentedTextWriter` | **Broiler.JS's indented output uses CRLF on Windows**; the specification uses LF |
| Raw JSON brand | A distinct .NET type | A structural check: any frozen object with a string `rawJSON` passes | Forgeable in Broiler.JS. This is an object-model fix in Broiler.JS, not shared code. |
| JSON modules | Compile time: `JsJsonModule` (its own reader) emits `export default` source | Load time: `JSJSON.Parse` | Different phases by design. The shared validator serves both. |

---

## 3. The target

```csharp
namespace Broiler.ECMAScript.Text;

public static class Utf16
{
    public static bool IsWellFormed(ReadOnlySpan<char> s);
    public static void ToWellFormed(ReadOnlySpan<char> s, Span<char> destination);
    public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> s, TrimEnds ends);   // StrWhiteSpaceChar
    public static PadStatus Pad(ReadOnlySpan<char> s, long targetLength, ReadOnlySpan<char> filler,
                                PadSide side, long maximumLength, WorkCharge? charge, out string result);
    public static long RelativeIndex(double relative, long length);              // already ToIntegerOrInfinity'd
}

public static class ReplacementTemplate
{
    // Parses "$1 $& $` $' $<name> $$" into parts; the engine evaluates each part in order.
    public static ReplacementPart[] Parse(ReadOnlySpan<char> template, int captureCount, bool hasNamedGroups);
}

public static class UriCodec
{
    public static UriStatus Encode(ReadOnlySpan<char> s, UriUnreservedSet set, StringBuilder output, WorkCharge? c, out int errorAt);
    public static UriStatus Decode(ReadOnlySpan<char> s, UriReservedSet preserve, StringBuilder output, WorkCharge? c, out int errorAt);
    public static string Escape(ReadOnlySpan<char> s);  public static string Unescape(ReadOnlySpan<char> s);
}

public static class Base64Codec
{
    public static FromBase64Result FromBase64(ReadOnlySpan<char> s, Base64Alphabet alphabet,
        LastChunkHandling handling, Span<byte> destination, WorkCharge? charge);   // Read, Written, Error
    public static FromBase64Result FromHex(ReadOnlySpan<char> s, Span<byte> destination, WorkCharge? charge);
    public static string ToBase64(ReadOnlySpan<byte> bytes, Base64Alphabet alphabet, bool omitPadding);
    public static string ToHex(ReadOnlySpan<byte> bytes);
}

public static class Json
{
    public ref struct JsonReader { /* Next() -> token/event; depth limit; meter; (code, offset) errors */ }
    public static void Quote(ReadOnlySpan<char> s, StringBuilder output);       // lowercase \u escapes, lone surrogates escaped
    public static string Gap(double? spaceNumber, ReadOnlySpan<char> spaceString);
    public static JsonValidation Validate(ReadOnlySpan<char> text, int maximumDepth, WorkCharge? charge);
}
```

**Policies are parameters.** The JSON depth limit, the string-length ceiling and the Float16 NaN
policy are each engine's choice. Broiler.VM's depth limit is its own constant. Broiler.JS chooses
its own, and **its current depth limit, which System.Text.Json imposed by default, is not a policy
anyone chose**.

**What adapters may contain:**

- receiver and argument conversion;
- option-object reading in specification order;
- typed-array validation and bounds;
- the evaluation of replacement parts;
- object and array construction;
- the reviver;
- source-text records;
- raw-JSON branding;
- error construction;
- charging policy.

---

## 4. Deletion lists

**Broiler.VM:**

- `StringWellFormed` and the `toWellFormed` loop;
- `StringTrimEnds`' classification;
- `StringPadTo`'s builder;
- the template walk in `StringExpandDollars`, `RegExpReplaceText` and `RegExpSubstitute`;
- `GlobalUriEncode`, `GlobalUriDecode`, `GlobalEscapeText`, `GlobalUnescapeText`,
  `GlobalUriHexValue` and its helpers, and their constant sets;
- `Base64Encode`, `Base64Result`, `Base64SkipSpace`, `Base64Decode`, `Base64DecodeFull`,
  `Base64DecodeFinal`, `Base64HexDigit`, `Base64DecodeHex` and `Base64Alphabet`;
- the lexical half of `JsRealm.Json`'s parser, and `JsonQuote`;
- `JsJsonModule`'s reader.

**Broiler.JS:**

- `IsWellFormedUtf16`, `ToWellFormedUtf16`;
- `StringPad`'s builder;
- `JSString.GetSubstitution` and the initializer's copy;
- `UriHelper`;
- `JSUInt8Array`'s `Base64DecodeResult`, the decode core, `DecodeBase64Chunk`, `SkipAsciiWhitespace`,
  `Base64Value`, `TryParseHexByte` and `HexDigitValue`;
- `JSJSON.QuoteString`;
- `JSJsonParser`'s use of `System.Text.Json` for guest JSON.

---

## 5. Risks

| Risk | Mitigation |
|---|---|
| Broiler.JS's JSON parsing speed changes when it leaves System.Text.Json | Broiler.JS's own measurement under its rule. Correctness corrections (depth, surrogates, `1e400`) are not negotiable. |
| The substitution split (shared parse, engine evaluation) moves an observable `Get` | The order of evaluation is the specification's, covered by test262's `replace` tests and dedicated vectors with getters in both engines |
| Provenance of `UriHelper` | It is deleted, not moved. The shared codec is written from the specification and Broiler.VM's implementation. |
