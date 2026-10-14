<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-5.1's correspondence from code: the text algorithms in both engines

**Written:** 2026-10-14, at SJL-5.1, as the input of
[JSD-0070](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md)'s
X1, X2 and X6, before any code of SJL-5 exists. **Read at** Broiler.VM `d6fe46a` and Broiler.JS `62c42e3`,
whose product sources are those of the default branches after SJL-4's merge, Broiler.VM `1f253ff` and
Broiler.JS `62c42e3`. **By reading only:** no engine was run for it, except where a row says it was run
through this repository's command-line root at `d6fe46a`. Where a behaviour is stated, it was worked
from the code, and where it depends on the .NET runtime's documented behaviour it says so; SJL-5.2's
vectors confirm or refute each one. The Broiler.VM cells of rows P1, P2, P5, S1, S3, B1 to B3, J2, J4
to J7 and X1 were also run through this repository's command-line root at `d6fe46a`, and agree. **It
holds no figure of a run.**

**Paths.** `VM/` is `src/Broiler.VM.Profile.JavaScript/` in Broiler.VM, `VMC/` its `.Compiler`, `VMF/` its
`.Format`. `BI/`, `RT/`, `GL/`, `PA/`, `CO/` and `MO/` are `Broiler.JS/Broiler.JavaScript.BuiltIns/`,
`.Runtime/`, `.Globals/`, `.Parser/`, `.Compiler/` and `.Modules/` in Broiler.JS, and `SL/` is
`Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/` there. A line number is the line at the revision
above.

**Two facts of the checkout frame every section.** Both engines reference `Broiler.ECMAScript.Algorithms`
from their runtime and compiler projects (SJL-3, SJL-4); Broiler.VM's `.Format` assembly, which holds
the regular-expression matcher, references no shared package, so its copies below stay until SJL-11 or
a later admission. And `JsRealm.String`'s own `split`, `replace` and `replaceAll`
(VM/JsRealm.String.cs:519-592, 650-654) are overwritten by `SetupRegExp`, which runs after `SetupString`
(VM/JsRealm.cs:226, 233) and defines all three again (VM/JsRealm.RegExp.cs:522-609): the String file's
`split` and `replace` are unreachable, and its `replaceAll` is reached only through the RegExp file's
string path.

---

## 1. Whitespace and UTF-16

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| TrimString's classification (22.1.3.32.1) | `StringTrimEnds` VM/JsRealm.String.cs:1429-1452, through the library's `NumberParsing.IsStrWhiteSpaceChar` (:1437, :1445); trimLeft and trimRight are aliases (:501-502) | `TrimString` BI/String/JSStringPrototype.Transform.cs:245-263, through the library's `NumberParsing.TrimStrWhiteSpace`; trimLeft and trimRight are the same function objects (BI/BuiltInsAssemblyInitializer.cs:972-976) |
| The `StrWhiteSpaceChar` set | the library's `StrWhiteSpace.Contains` (SL/Numbers/StrWhiteSpace.cs:45-88), SJL-3's, for trim, `StringToNumber`, `parseInt`, `parseFloat` and `StringToBigInt` | the same member, for the same operations and `NumberParser.TryCoerceToUInt32` RT/NumberParser.cs:40 and Intl's `TryParseExactDecimal` BI/Intl/JSIntl.cs:4934 |
| IsStringWellFormedUnicode (6.1.4) | `StringWellFormed` VM/JsRealm.String.cs:695-717, called by `isWellFormed` :289-295 | `IsWellFormedUtf16` BI/String/JSStringPrototype.Transform.cs:193-212, called by `isWellFormed` :177-183 |
| toWellFormed (22.1.3.31) | inline loop VM/JsRealm.String.cs:297-331; the input itself when it is well formed | `ToWellFormedUtf16` BI/String/JSStringPrototype.Transform.cs:214-243; always a new string |
| CodePointAt (11.1.4) | inline in `codePointAt` VM/JsRealm.String.cs:248-270; the string iterator's pair test `CreateStringIterator` VM/JsRealm.Symbol.cs:496-518; `ReadCodePoint` VM/JsRealm.String.cs:868-878 (normalize); `JsUnicodeCasing.NextCodePoint`, `PreviousCodePoint` VM/JsUnicodeCasing.cs:167-192; `SliceTokenizer.CodePointAt` VMC/SliceTokenizer.cs:2539-2551 | `JSString.CodePointAt` BI/String/JSStringPrototype.Extract.cs:93-115; the iterator's `CodePointEnumerator.TryNext` BI/String/JSString.cs:548-570 |
| AdvanceStringIndex (22.2.7.3) | `RegExpAdvanceIndex` VM/JsRealm.RegExp.cs:1231-1234 over `JsRegExpMatcher.Advance` VMF/JsRegExpMatcher.cs:855-866; the `u`-flag back-up in `RegExpMatchOne` VM/JsRealm.RegExp.cs:1924-1930 | `AdvanceStringIndex` BI/BuiltInsAssemblyInitializer.cs:1493-1500 (`@@split` only) and six inline copies: :1651-1656 (`@@match`), :1775-1781 and :1813-1818 (`@@replace`), BI/RegExp/JSRegExpStringIterator.cs:98-107 (matchAll), BI/RegExp/JSRegExp.cs:443-460 (`Match`, which reads `unicode` and `unicodeSets` with observable Gets) and BI/RegExp/JSRegExp.Broiler.cs:338-346 |

**Whitespace and lone-surrogate scans outside the area**, named so that none is lost, each with the
milestone that owns it:

- the lexers: `JsUnicodeLexical.IsWhiteSpace` VMF/JsUnicodeLexical.cs:77-79 and `SliceTokenizer.IsWhiteSpace`,
  `IsLineTerminator` VMC/SliceTokenizer.cs:2459, 2472; **`JsParser.TemplateReader`'s `char.IsWhiteSpace`**
  VMC/JsParser.cs:7639, the .NET set, a copy of the tokenizer's "previous significant character" test with
  a different set; Broiler.JS's `FastScanner._ReadToken` PA/FastScanner.cs:376-386, `char.IsWhiteSpace`
  plus U+FEFF, so **U+0085 is white space to Broiler.JS's lexer** (SJL-13 and SJL-14);
- the module early error on a lone surrogate: `JsParser.IsWellFormed` VMC/JsParser.cs:1336-1353 and
  `FastParser.ModuleExportNameLiteral` PA/FastParser.Import.cs:186-204 (SJL-15);
- the regular expressions' `\s`: `JsRegExpMatcher.SpaceRanges` VMF/JsRegExpMatcher.cs:589-593 and
  `JSRegExp.TransformUnicodeWhitespace` BI/RegExp/JSRegExp.cs:4649-4765, both hand-written lists of the
  `StrWhiteSpaceChar` set; `RegExp.escape`'s white-space test, from the .NET runtime's Unicode data in
  `RegExpEscapeCodePoint` VM/JsRealm.RegExp.cs:1547-1549 and from `char.IsWhiteSpace` in
  `JSRegExp.TryAppendEscape` BI/RegExp/JSRegExp.cs:123, which escapes U+0085 as well (SJL-11);
- `Date.parse`'s separators, `DateParseTextual` VM/JsRealm.Date.cs:1077-1078 and `DateParser` RT/DateParser.cs:17,
  93, 204-216, whose `\s` is .NET's (SJL-6);
- `String.prototype.normalize` and the collator over a lone surrogate: Broiler.JS calls `string.Normalize`
  (BI/String/JSStringPrototype.Transform.cs:16-31, BI/Intl/JSIntl.cs:6342-6345), which .NET documents as
  throwing `ArgumentException` on invalid UTF-16 (SJL-7, SJL-17);
- the format's own text and the harness: `JsFormat.EncodeText`, `DecodeText` VMF/JsFormat.cs:874-993,
  `JsCollationData.Valid` VM/JsCollationData.cs:510 and `Test262Json.Quote`, which are not guest
  operations.

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| W1 | trim over every code unit | `StrWhiteSpaceChar` | the library's set | the library's set | none: SJL-3 already shares it |
| W2 | `isWellFormed`, `toWellFormed` over lone and paired surrogates | 6.1.4, 22.1.3.31 | as specified | as specified | none |
| W3 | `codePointAt` and the iterator at a lone surrogate | CodePointAt | the code unit | the code unit | none |

## 2. Padding and indices

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| StringPaddingBuiltinsImpl and StringPad (22.1.3.17.1, 22.1.3.17.2) | `StringPadTo` VM/JsRealm.String.cs:1457-1506 (padStart and padEnd :644-648), under `StringLengthCeiling` :58, 2^24 code units | `PadStart`, `PadEnd` and `StringPad` BI/String/JSStringPrototype.Transform.cs:33-80, under `int.MaxValue` |
| `repeat`'s size check | inline in `repeat` VM/JsRealm.String.cs:610-642, the same ceiling | inline in `Repeat` BI/String/JSStringPrototype.Transform.cs:82-123, `int.MaxValue` |
| The ceiling's other users | `NormalizeText` VM/JsRealm.String.cs:779-782, `StringChangeCase` :1215-1218, `RegExpAppend` VM/JsRealm.RegExp.cs:1257-1272, `RegExpEscapeText` :1485-1488 | none |
| Relative-index normalisation (the pattern of 22.1.3.22 step 4 and its kin) | helpers `StringRelativeIndex` VM/JsRealm.String.cs:1146-1155 (slice) and `ArrayRelative` VM/JsRealm.Array.cs:1609-1620 (Array copyWithin, toSpliced, splice, fill, slice; TypedArray subarray, slice, fill, copyWithin; ArrayBuffer `slice` VM/JsRealm.Binary.cs:302, 305; SharedArrayBuffer `slice` VM/JsRealm.Shared.cs:145, 148); inline: String `substr` VM/JsRealm.String.cs:448-450, `at` :276, `lastIndexOf` :351-357; Array `at` VM/JsRealm.Array.cs:276, `with` :459, `indexOf` :1062-1073, `lastIndexOf` :1102-1106, `includes` :1136-1141; TypedArray `indexOf` VM/JsRealm.Binary.cs:962, `lastIndexOf` :992, `includes` :1032, `at` :1133, `with` :1274 | helpers `Relative`, `Clamp` BI/String/JSStringPrototype.Extract.cs:18-23 (slice, substr) and `ClampRelativeIndex` BI/Array/Typed/JSTypedArray.prototype.cs:41-46 (copyWithin only); inline: String `at` BI/BuiltInsAssemblyInitializer.cs:951-956; Array `at`, `includes`, `indexOf`, `lastIndexOf` BI/Array/JSArrayPrototype.Search.cs:23-24, 48-52, 103-109, 141-150, `slice`, `toSpliced`, `with` BI/Array/JSArrayPrototype.Utility.cs:109-120, 304-309, 360-364, `copyWithin`, `fill`, `splice` BI/Array/JSArrayPrototype.Modification.cs:103-109, 174-178, 520-538; TypedArray `fill`, `with`, `at`, `includes`, `indexOf`, `lastIndexOf`, `slice`, `subarray` BI/Array/Typed/JSTypedArray.prototype.cs:141-155, 231-232, 260-261, 370-379, 405-410, 468-492, 687-693, 846-851; ArrayBuffer `slice`, `sliceToImmutable` BI/Array/Typed/JSArrayBuffer.cs:464-473, 508-517; SharedArrayBuffer `slice` BI/Array/Typed/SharedArrayBuffer.cs:135-144 |
| Clamping without the relative step | `StringBoundIndex` VM/JsRealm.String.cs:1133-1141 (indexOf, lastIndexOf, includes, startsWith, endsWith, substring); `ArraySpliceCount` VM/JsRealm.Array.cs:1625-1647 | `Clamp` above (substring and the search methods, BI/String/JSStringPrototype.Search.cs:147-149) |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| P1 | `"x".padEnd(2**24 + 1, { toString() { ... } })` | ToString(fillString) runs before StringPad, which may then refuse | `RangeError` before `toString` is called | `toString` called, then the result is built | correction of Broiler.VM (step order) |
| P2 | `"x".padEnd(2**24 + 1, "")` | an empty filler returns S, whatever the length | `RangeError` | `"x"` | correction of Broiler.VM |
| P3 | `"x".padEnd(2**40)` | an implementation may refuse a string past its ceiling, with `RangeError` | `RangeError` | `RangeError` (past `int.MaxValue`, after the filler is converted). *The design's reading, "raises a .NET exception", does not hold for this input* | none |
| P4 | `"x".padEnd(2**30 + 1)`, `"x".repeat(2**30 + 1)` | as P3 | `RangeError` (past 2^24) | the builder allocates past .NET's largest string, and `OutOfMemoryException` escapes as a plain `Error` (.NET documented; RT/JSException.cs:238-245 is how it reaches a script) | correction of Broiler.JS: the length policy maps it to `RangeError` |
| P5 | `ta.lastIndexOf(x, 1, extra)` on a typed array | fromIndex is present whenever a second argument is | honoured | ignored when more than two arguments are passed (`a.Length == 2`, BI/Array/Typed/JSTypedArray.prototype.cs:468) | correction of Broiler.JS |

## 3. Substitution templates

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| GetSubstitution for a string pattern (no captures, no named groups) | `StringExpandDollars` VM/JsRealm.String.cs:1608-1661, reached from `replaceAll`'s string path `StringReplaceWith` :1521-1596; and inline in `RegExpReplaceText` VM/JsRealm.RegExp.cs:2096-2136, reached from `replace`'s string path | `JSString.GetSubstitution` BI/String/JSStringPrototype.Pattern.cs:49-94, reached from `replace` :137 and `replaceAll` :192 |
| GetSubstitution with captures and named groups | `RegExpSubstitute` VM/JsRealm.RegExp.cs:2203-2312, from `RegExpSymbolReplace` :872-1028; the built-in groups object `RegExpNamedGroups` :2152-2177 | the initializer's local `GetSubstitution` BI/BuiltInsAssemblyInitializer.cs:1502-1593, from `@@replace` :1883; and **a third copy, `JSRegExp.Replace(string, string)`** BI/RegExp/JSRegExp.cs:593-672 |

Both engines handle `$$`, `` $& ``, `` $` ``, `$'` alike; `$0` and `$00` stay literal; `$nn` above the
capture count falls back to one digit; `$<` stays literal when there are no named groups or no closing
`>`; and a named group is read with `Get` once per occurrence, left to right, so a getter is observable.

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| S1 | `"abc".replaceAll("x", { toString() { ... } })` | ToString(replaceValue) before the search (22.1.3.20 step 7) | `toString` not called when nothing matches: the conversion follows the search (VM/JsRealm.String.cs:1529-1537) | called | correction of Broiler.VM |
| S2 | a named group whose value is an object with `Symbol.toPrimitive`, or a Symbol | `ToString(capture)` | the specification's ToString | the CLR `ToString()` (BI/BuiltInsAssemblyInitializer.cs:1502-1593 through RT/JSObject.DynamicDispatch.cs:31-50), which calls `toString` directly | correction of Broiler.JS |
| S3 | `"a/a/".replace(re, "x")` for a RegExp `re` whose `Symbol.replace` is `undefined` or `null` | the search value is converted with ToString and searched for as text (22.1.3.19 steps 2, 3) | the text `"/a/"` is searched for | the pattern is matched as a regular expression by `JSRegExp.Replace` (BI/String/JSStringPrototype.Pattern.cs:115-116), whose templates never substitute `$<name>` | correction of Broiler.JS |

## 4. URI

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| Encode (19.2.6.5) | `GlobalUriEncode` VM/JsRealm.Global.cs:333-376 with `GlobalUriAppendUtf8` :491-515 and `GlobalUriAppendOctet` :520-525; charged one unit per step, a pair one step | `UriHelper.Encode` RT/UriHelper.cs:231-301 |
| Decode (19.2.6.6) | `GlobalUriDecode` VM/JsRealm.Global.cs:381-454 with `GlobalUriOctetAt` :459-475, `GlobalUriSequenceLength` :480-486 and `GlobalUriAppendCodePoint` :530-541; charged one unit per step and per continuation octet | `UriHelper.Decode` RT/UriHelper.cs:114-218 with `ParseHexNumber` :74-97 and a throwing `UTF8Encoding` :14, 203-210 |
| escape, unescape (B.2.1) | `GlobalEscapeText` VM/JsRealm.Global.cs:546-576, `GlobalUnescapeText` :581-631, charged one unit per step | `UriHelper.Escape` RT/UriHelper.cs:99-103, `Unescape` :303-364 |
| The sets | `GlobalUriUnescapedComponent` VM/JsRealm.Global.cs:52, `GlobalUriUnescapedUri` :61, `GlobalUriPreservedUri` :71, `GlobalEscapeUnescaped` :76, `GlobalUriHexDigits` :81, `GlobalUriIsAlphanumeric` :647-648, `GlobalUriInSet` :657-668 | in `UriHelper` |
| Hex digits | `GlobalUriHexValue` VM/JsRealm.Global.cs:636-642, with identical copies `JsonHexDigit` VM/JsRealm.Json.cs:622-628 and `Base64HexDigit` VM/JsRealm.Base64.cs:578-584 (and `JsRegExpMatcher.HexValue` VMF:3432, SJL-11) | `ParseHexNumber` above, `HexDigitValue` BI/Array/Typed/JSUInt8Array.cs:443-462 |
| Callers | `SetupGlobalUriFunctions` VM/JsRealm.Global.cs:160-183 | GL/JSGlobal.cs:100-112, 151-170, 203-208, with `CoerceLegacyUriString` |

No difference was found: both engines refuse a lone surrogate on encode and every malformed or
overlong UTF-8 sequence, encoded surrogate and value above U+10FFFF on decode with `URIError`, keep a
preserved character's escape on `decodeURI`, write uppercase hexadecimal, and copy a malformed
`unescape` sequence through. **Provenance:** `UriHelper`'s documentation comments cite Jurassic's
`GlobalObject.cs` by commit and line (RT/UriHelper.cs:65, :106), and Broiler.JS's notices say that
whether the code derives from it "is not recorded here yet". SJL-5.5 deletes `UriHelper`; the shared
codec is written from the specification and Broiler.VM's implementation, not from it.

## 5. Base64 and hex

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| The adapters | `SetupUint8ArrayCodecs` VM/JsRealm.Base64.cs:54-150 (fromBase64 :59-79, fromHex :81-98, toBase64 :100-108, toHex :110-116, setFromBase64 :118-134, setFromHex :136-149); `Base64Argument` :155-156, `Base64Receiver` :164-173, `Base64ReadAlphabet` :204-220, `Base64ReadLastChunk` :225-242, `Base64BytesOf` :250-261, `Base64Capacity` :269-277, `Base64Settle` :289-305, `Base64Allocate` :310-315 | `FromBase64` BI/Array/Typed/JSUInt8Array.cs:48-62, `FromHex` :64-85, `ToBase64` :87-105, `ToHex` :107-117, `SetFromBase64` :119-144, `SetFromHex` :405-441; `GetBase64Alphabet`, `GetBase64OmitPadding`, `GetBase64LastChunkHandling` :351-403 |
| GetOptionsObject | `Base64Options` VM/JsRealm.Base64.cs:181-195, **which Intl's constructors also call** (VM/JsRealm.ListFormat.cs:137, Segmenter.cs:185, DurationFormat.cs:185, DisplayNames.cs:141) | inline in the option getters |
| FromBase64 (the proposal's decode, with its last-chunk modes) | `Base64Decode` VM/JsRealm.Base64.cs:393-529, `Base64DecodeFull` :534-546, `Base64DecodeFinal` :554-573, `Base64SkipSpace` :371-379, `Base64Alphabet` :48-49, `Base64Result` :350-366; charged `length + 1` before the loop, nothing when the destination is empty | `DecodeBase64` BI/Array/Typed/JSUInt8Array.cs:160-273, `Base64DecodeResult` :146-158, `DecodeBase64Chunk` :284-325, `SkipAsciiWhitespace` :327-339, `Base64Value` :341-349 |
| FromHex | `Base64DecodeHex` VM/JsRealm.Base64.cs:592-620, `Base64HexDigit` :578-584 | `TryParseHexByte`, `HexDigitValue` BI/Array/Typed/JSUInt8Array.cs:443-462, and inline in `FromHex` and `SetFromHex` |
| Encode | `Base64Encode` VM/JsRealm.Base64.cs:320-345 over `Convert.ToBase64String` and `Base64Url.EncodeToString`; `toHex` over `Convert.ToHexStringLower` | `ToBase64` over `Convert.ToBase64String` with replacements; `ToHex` over `Convert.ToHexString` and `ToLowerInvariant` |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| B1 | `new Uint8Array(1).setFromHex("aaZZ")` | `{ read: 2, written: 1 }`: decoding stops at the destination's length | `{ read: 2, written: 1 }` (run at `d6fe46a`) | writes `0xAA`, then `SyntaxError` (BI/Array/Typed/JSUInt8Array.cs:420-430): the whole input is parsed first | correction of Broiler.JS |
| B2 | an options argument that is not an object, e.g. `Uint8Array.fromBase64("", 1)` | GetOptionsObject: `TypeError` | `TypeError` | ignored | correction of Broiler.JS |
| B3 | `{ alphabet: null }` or `{ lastChunkHandling: null }` | not one of the names: `TypeError` | `TypeError` | the default (`IsNullOrUndefined`, :355-366) | correction of Broiler.JS |
| B4 | `toBase64`, `toHex`, `setFromBase64` on a detached or out-of-bounds array | ValidateUint8Array and GetUint8ArrayBytes: `TypeError` | `TypeError` | `""`, or `{ read: 0, written: 0 }` | correction of Broiler.JS |

## 6. JSON

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| The lexical grammar of `JSON.parse` (25.5.1) | inside `JsRealm.Json`'s reader, interleaved with object building: `JsonSkipSpace` VM/JsRealm.Json.cs:254-267, `JsonParseString` :496-584, `JsonParseHex` :594-617, `JsonHexDigit` :622-628, `JsonParseNumber` :633-698 (the text to the library's `StringToNumber`), `JsonParseKeyword` :703-712, `JsonSyntax`, `JsonUnexpected` :717-726, `JsonValueStart` :295-333; the building half `JsonParseEntry` :217-249, `JsonParseValue` :272-285, `JsonParseUnrecorded` :338-384, `JsonParseObject` :389-451, `JsonParseArray` :456-491, `JsonRecord` :1315-1336; charged one unit per value, per `:` and `,`, per skipped space and per string's length | `JSJsonParser.Parse`, `ParseWithSource` BI/Json/JSJsonParser.cs:25-43 and `Read`, `ReadArray`, `ReadObject`, `ReadWithSource`, `GetSourceText`, `AddJsonObjectProperty` :47-184, over `System.Text.Json` with default options; the exception filter `JSJSON.Parse` BI/Json/JSJSON.cs:376-428 |
| The depth limit | `JsonMaximumDepth` VM/JsRealm.Json.cs:63, 512; past it `RangeError` (:343-346), the same limit in `JsonInternalize` :740 and stringify's `JsonEnter` :1138-1141 | System.Text.Json's default `MaxDepth`, 64 (.NET documented); past it `SyntaxError` |
| QuoteJSONString (25.5.2.3) | `JsonQuote` VM/JsRealm.Json.cs:1231-1287 with `JsonIsLoneSurrogate` :1292-1307, charged `length + 1` | `JSJSON.QuoteString` BI/Json/JSJSON.cs:884-976 |
| The gap (25.5.2 steps 6 to 8) | `JsonApplySpace` VM/JsRealm.Json.cs:934-962 | inline BI/Json/JSJSON.cs:443-479 |
| Indentation | `JsonJoin` VM/JsRealm.Json.cs:1158-1199: `"\n"` | `IndentedTextWriter` over a `StringWriter`, BI/Json/JSJSON.cs:437, 509-513, whose `NewLine` is never set |
| JSON.rawJSON, isRawJSON | `RawJsonObject` VM/JsRealm.Json.cs:129-148 (the brand is the .NET type), `JsonRawJsonEntry` :160-192, `JsonRawMayStart`, `JsonRawMayEnd` :197-204, isRawJSON :106-107 | `RawJSON` BI/Json/JSJSON.cs:533-563, `IsRawJSON`, `TryGetRawJsonText` :568-592, `IsIllegalRawJsonBoundaryChar` :565-566 |
| JSON modules | at compile time, `JsJsonModule.TryTranslate` and its `Reader` VMC/JsJsonModule.cs:65-449, with its own depth limit `MaximumDepth` :54, 1024, called from VMC/JsCompiler.cs:953-963; it writes `export default (` a literal `);` with every key computed and every code unit outside printable ASCII escaped in uppercase | at load time, `JSJSON.Parse` from MO/JSModuleContext.cs:1203-1210 |
| Other guest JSON | none | `FetchResponse.json()` JS/Broiler.JavaScript.Network/FetchResponse.cs:71-74, through `JSJSON.Parse` |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| J1 | a document nested 65 to 512 deep | an implementation may refuse past its own limit | parsed | `SyntaxError`: System.Text.Json's default, which nobody chose | correction of Broiler.JS: a policy parameter |
| J2 | `JSON.parse('"\\ud800"')` | the one code unit | the one code unit | `SyntaxError` (`GetString` throws `InvalidOperationException`, .NET documented) | correction of Broiler.JS |
| J3 | `JSON.parse('"' + "\ud800" + '"')` | the one code unit | the one code unit | an `ArgumentException` from the transcoding to UTF-8, outside the filter at BI/Json/JSJSON.cs:405, escapes as a plain `Error` (.NET documented) | correction of Broiler.JS |
| J4 | `JSON.parse("1e400")` | `Infinity` | `Infinity` | `SyntaxError` (`GetDouble` refuses it, .NET documented) | correction of Broiler.JS |
| J5 | `JSON.stringify([1], null, 1)` on Windows | `"[\n 1\n]"` | LF | CRLF | correction of Broiler.JS |
| J6 | `JSON.stringify(Object.freeze({ rawJSON: "}{" }))` | an ordinary object: `{"rawJSON":"}{"}` | an ordinary object | its text is emitted as raw JSON, unvalidated: the brand is structural (BI/Json/JSJSON.cs:568-592) | correction of Broiler.JS, in its object model |
| J7 | `Object.keys(JSON.rawJSON("1"))` | `["rawJSON"]`: CreateDataPropertyOrThrow makes it enumerable | `["rawJSON"]` | `[]`: the property is not enumerable (:533-563) | correction of Broiler.JS |
| J8 | a document nested deeper than 61 levels, as a JSON module | the language admits it | refused, `NestingTooDeep`: the source the translation writes is parsed under the module's parse options, 64 levels (run at `d6fe46a`), so the translation's own limit of 1024 is never reached | refused past System.Text.Json's depth, as J1 | none in SJL-5 for Broiler.VM: part C of the bundle's rule holds the translation's artifacts to the base, and the depth the parse options give is SJL-12's |

The quoting, the gap and the line ending of Broiler.VM agree with the specification; Broiler.JS's
quoting and gap agree with it too, so J5 is the only stringify difference found.

## 7. Split, and string search

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| X1 | `"abc".split("", 2)` | `["a", "b"]` | `["a", "b"]` (`RegExpSplitText` VM/JsRealm.RegExp.cs:2317-2358) | `["a", "b", "c"]`: the empty separator's branch ignores the limit (BI/String/JSStringPrototype.Pattern.cs:300-308) | correction of Broiler.JS, in its own `split` |
| X2 | `indexOf`, `includes`, `startsWith`, `endsWith` and the non-standard `contains` over text a culture compares specially, e.g. `"a\r\nb".indexOf("\n")` or a search for an ignorable character | ordinal comparison of code units | ordinal everywhere (VM/JsRealm.String.cs:339, 361, 377, 390, 409; VM/JsRealm.RegExp.cs:2074, 2339) | `string.IndexOf(string, int)`, `StartsWith(string)`, `EndsWith(string)` and `string.Compare` without an ordinal comparison (BI/String/JSStringPrototype.Search.cs:35, 57, 64, 84, 112, 126): the current culture's, which under ICU treats `\r\n` as one unit and ignores ignorable characters (.NET documented) | correction of Broiler.JS, in its own search methods |

The other culture-sensitive calls the reading found are outside the area: `Date.parse`'s fallback to the
current culture RT/DateParser.cs:213 (SJL-6, and its culture-dependent legacy parse stays under
SJL-D-17), `toLocaleLowerCase` and `toLocaleUpperCase` without a locale BI/String/JSStringPrototype.Transform.cs:134,
152 (SJL-9), the compiler's and the module loader's prefix tests on source text and specifiers
(CO/Declarations/FastCompiler.CreateClass.cs:211, 451; MO/JSModuleContext.cs:143, 190, 266, 317), and
host interop. The tagged-template path trims every leading `}` from a raw segment
(CO/Expressions/FastCompiler.VisitTaggedTemplateExpression.cs:75), which is a syntax defect (SJL-14), not
a text algorithm.
