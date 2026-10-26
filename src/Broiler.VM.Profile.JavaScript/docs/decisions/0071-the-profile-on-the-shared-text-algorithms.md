<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0071 - The profile on the shared text algorithms: SJL-5.4's cut-over

**Status:** Decided 2026-10-15, in the change that lands the cut-over. It applies
[JSD-0070](0070-text-algorithms-move-to-the-shared-algorithms-library.md), the area's verdict.

**Date:** 2026-10-15.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list and rule I5's register, and the release owner, for the notices row. **All three roles are held
by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-5, work package SJL-5.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-5-text-algorithms.md)). The
owner chose on 2026-10-14 to start SJL-5 before SJL-3 is met, and the ledger records that choice.

---

## Context

JSD-0070 accepted the text area's six sub-areas for `Broiler.ECMAScript.Algorithms` and took the
algorithm choices; it changed nothing in this profile. This record is the change itself: the family
references the package version that carries the area, its UTF-16 readings, padding and relative
indices, replacement templates, URI codecs, base64 and hex codecs and JSON reading call the library,
and the members that computed them are deleted. The evidence that judges it - test262 in both forms,
the fuel-parity twins and exhaustion verdicts, the JSON modules' artifacts, the closures and the
packages - is bundle [`sjl-5-001`](../../../../docs/evidence/sjl-5-001/README.md), under the rule SJL-5.1
committed before any of this code existed.

## Decision

### 1. References

`Broiler.VM.Profile.JavaScript` and `Broiler.VM.Profile.JavaScript.Compiler` reference
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.3`, the shared stream's next preview, packed from
Broiler.JS `0db90422c8aca642926acff6b715d788cb17f9c6` and restored from the committed feed
`eng/packages/` with lock files. It is not on nuget.org at this record. Rule I1 admits one version of a
package, so the allow-list's entry, the feed, both lock files and rule I1's witness moved to it together,
and `0.1.0-preview.2` left the feed, in the change before this one (`fbdd14b`); `packages.md` names the
version ([JSC-304](../roadmap.corrections.md#jsc-304)). The package baseline stays bundle
[JS-10-006](../evidence/js-10-006/README.md)'s candidate pack, which declares `0.2.0-preview.1`, a version
rule N37 still accepts because it is above the one the allow-list pins.

### 2. Adapters

Each calls the library and keeps the coercion order, the errors, the charging and the value wrapping,
and computes no reading, normalisation, template, codec or token itself:

| Adapter | What it keeps | What it calls |
|---|---|---|
| `String.prototype.codePointAt`, `isWellFormed`, `toWellFormed`; the string iterator's step; the normalisation walks of `JsRealm.String` | The receiver's conversion and the bounds test | `Utf16.CodePointAt`, `IsWellFormed`, `ToWellFormed` |
| `RegExpAdvanceIndex`, one line | `lastIndex` after `ToLength` | `Utf16.AdvanceStringIndex` |
| `StringPadTo` (`padStart`, `padEnd`) | `ToLength`'s early answer for a length not past the receiver's, the filler's conversion, and the `RangeError` "Invalid string length" for `LimitExceeded` | `StringPadding.Pad` under `LengthLimit(StringLengthCeiling)`, 2^24 code units |
| String `at`, `slice`, `substr`; Array `at`, `with`, `indexOf`, `lastIndexOf`, `includes`, and `ArrayRelative`, one line; TypedArray `at`, `indexOf`, `lastIndexOf`, `includes` | `ToIntegerOrInfinity` of each argument, in order, and the errors | `RelativeIndex.TryElement`, `Clamped`, `LastSearchStart` |
| `JsRealm.String`'s `replaceAll` path, `RegExpReplaceText`'s string path, `StringSubstitute`, `RegExpSubstitute` | The replacement's conversion, the search, each part's evaluation in template order (a named group's `Get` where its part is, `RegExpAppend`'s units and ceiling) | `ReplacementTemplate.Parse` |
| `encodeURI`, `encodeURIComponent`, `decodeURI`, `decodeURIComponent`, through `GlobalUriAnswer`; `escape`, `unescape` | `ToString` of the argument, and one `URIError` "URI malformed" for every failure | `UriCodec.Encode`, `Decode` with `ReservedAndNumberSign` or the empty set; `Escape`, `Unescape` |
| `Uint8Array.fromBase64`, `fromHex`, and `toBase64`, `toHex`, `setFromBase64`, `setFromHex` on its prototype | The receiver, `GetOptionsObject` and the option reads in order, bounds, the result object `{ read, written }`, and the `SyntaxError` with the message this profile wrote before, chosen by the library's failure code | `Base64Codec.FromBase64` and `FromHex`, into the view's own bytes, or a buffer no decode of the text can fill; `ToBase64`, `ToHex` |
| `JSON.parse` and `JSON.rawJSON`, through `JsonRead` and `JsonBuild` with its object and array halves | Object and array construction, `CreateDataProperty`'s repeated key, the parse records and their source text, the reviver, the raw-JSON brand and its end test, and the errors with their positions | `JsonReader` under `DepthLimit(512)` |
| `JSON.stringify`: `JsonSerialize`, `JsonSerializeObject`, `JsonApplySpace` | The driver, whole | `JsonText.Quote`, `GapFromNumber`, `GapFromString` |
| `JsJsonModule.TryTranslate`, the compile-time JSON module translation | The writer of the synthetic module, unchanged, and the two refusal codes | `JsonReader` under `DepthLimit(1024)`, with no meter |

**Errors stay the engine's, and so do their words.** A JSON failure's code picks this profile's message
and its position is the library's, which is the position this profile's reader reported: `json.unexpected`
is "Unexpected token 'c' in JSON" or, at the end, "Unexpected end of JSON input"; the others are the
unterminated string, the control character, the escapes, and `RangeError` "Maximum JSON nesting depth
exceeded" for `json.too-deep`. `JSON.rawJSON` refuses what follows its value where the value ends, before
any white space after it is skipped and charged, as it did. The base64 and hex messages are the ones this
profile wrote, each picked by its failure code. The JSON module translation's refusal is worded from the
reader's failure code and offset; the diagnostic codes, `InvalidJsonModule` and `NestingTooDeep`, are the
ones part C of the bundle's rule compares, and they do not move.

**Charging.** The engine passes the meter it exposes as a `WorkCharge`, and JSD-0070 decision 3 holds:
every shared operation charges what this family charged for it, in its order. Where this family charged a
template walk differently from the parser's `length + 1` - the `replaceAll` path's `max(1, length)` once
per call, and nothing for `RegExpReplaceText`'s string path - the adapter keeps its own charge and passes
the parser no meter. `codePointAt`, the iterator's step, `AdvanceStringIndex` and the relative indices
were not charged and are not.

### 3. Deleted

The 44 members the milestone names and JSD-0070's X6 extends are deleted and registered in rule I5's
`retired-members.json`, each with what replaced it: `JsRealm`'s `StringWellFormed`, `ReadCodePoint`,
`StringRelativeIndex` and `StringExpandDollars`; the URI helpers `GlobalUriEncode`, `GlobalUriDecode`,
`GlobalEscapeText`, `GlobalUnescapeText`, `GlobalUriHexValue`, `GlobalUriOctetAt`,
`GlobalUriSequenceLength`, `GlobalUriAppendUtf8`, `GlobalUriAppendOctet`, `GlobalUriAppendCodePoint`,
`GlobalUriIsAlphanumeric` and `GlobalUriInSet`, with the five constant sets; `Base64Alphabet`,
`Base64Encode`, the nested class `Base64Result`, `Base64SkipSpace`, `Base64Decode`, `Base64DecodeFull`,
`Base64DecodeFinal`, `Base64HexDigit` and `Base64DecodeHex`; the reader of `JsRealm.Json` -
`JsonSkipSpace`, `JsonParseValue`, `JsonValueStart`, `JsonParseUnrecorded`, `JsonParseObject`,
`JsonParseArray`, `JsonParseString`, `JsonParseHex`, `JsonHexDigit`, `JsonParseNumber` and
`JsonParseKeyword` - with `JsonQuote` and `JsonIsLoneSurrogate`; and the nested class
`JsJsonModule.Reader`. The `toWellFormed` loop, `codePointAt`'s and the iterator's pair tests, the inline
template walk of `RegExpReplaceText` and of `RegExpSubstitute`, `JsonApplySpace`'s normalisation,
`toHex`'s encoder and the inline relative-index normalisations were not members, and are gone.

**What stays, and holds no algorithm:** the adapters of section 2. And four things the lists name that
stay, each for a reason the area's own rules give:

- **String `lastIndexOf`'s normalisation**, which X6 lists among the inline normalisations. It is
  `StringBoundIndex`, a clamp without the relative step, and X6's last sentence keeps such a clamp.
- **TypedArray `with`'s normalisation.** Its relative step is taken against the length before the value's
  conversion and its range test against the length after it, which the specification requires and
  `RelativeIndex.TryElement`, one length for both, cannot express. It stays inline.
- **`Base64Options`**, which is `GetOptionsObject` (JSD-0070, "The design's readings").
- **`JsParser.IsWellFormed`**, the compiler's early error on a lone surrogate in an export name, which the
  correspondence assigns to SJL-15; and the matcher's own readers in `.Format`, which admits no shared
  package (SJL-11).

*(Noted 2026-10-26, at SJL-13.1, by [JSD-0083](0083-the-shared-syntax-library-and-the-wide-path.md) decision D39.)*
`JsParser.IsWellFormed`'s export-name check is the early-error catalogue's EE0178, a `syntax` row: both engines'
parsers refuse it at parse time. So the shared syntax library refuses it from SJL-13's step 1, which deletes
`JsParser`, with EE0178's code; it is SJL-13's, not SJL-15's, as "which the correspondence assigns to SJL-15" read. Its
other caller, the module specifier's check (the catalogue's EX0029, an engine-only row), becomes this family's
admission walk's at the same step (JSD-0083 decision D18).

### 4. Corrections

This family's three corrections, P1, P2 and S1, under decision SJL-D-16 (a). At this family's pin of the
shared corpus (revision 12), each of the five vectors that carried a `divergence.broiler-vm` field now
reports "divergence no longer holds", and every vector matches (42,943 of 42,943):

| Correction | What this family did | Vectors |
|---|---|---|
| P1 | `padStart` and `padEnd` refused a length past the string ceiling with `RangeError` before converting the filler, so its `toString` was never called; the filler is now converted first, as `StringPaddingBuiltinsImpl` converts it before `StringPad` | `text/pad-order/0001`, `0003` |
| P2 | They refused such a length when the filler was empty; an empty filler now answers the receiver whatever the length | `text/pad-order/0002`, `0004` |
| S1 | `replaceAll`'s string path converted a replacement that is not callable after the search, so its `toString` was not called when nothing matched; it is now converted before, as the language's step 7 does | `text/replace-order/0001` |

The JSON module translation reads the grammar as `JSON.parse` does, so JSD-0070's class C1 could have
admitted documents read differently. None is: every document of part C's population gives the same
outcome, artifact hash and diagnostic code as at the base, checked once at this change; part C's four
runs are SJL-5.6's.

### 5. Records

- **Notices.** `THIRD_PARTY_NOTICES.md`'s row for the package names `0.1.0-preview.3` since the change
  before this one. The text area is written from the specification and from this family's code, so the
  package carries no third-party component beyond the two the row already names, Ryu's routine and
  fdlibm's `expm1` and `log1p`. The row is still not confirmed, so release
  gate 12's blocker for it stands, under the same name.
- **Packages.** `packages.md` names the new version ([JSC-304](../roadmap.corrections.md#jsc-304)).
- **Assurance.** The record is regenerated. The code that left this family's assurance coverage is
  covered by the home's own assurance component in the Broiler.JS repository, as EX-125 and EX-127 say of
  the earlier areas; bundle `sjl-5-001` names it with SJL-5.6's evidence.

## Correction entries

[JSC-304](../roadmap.corrections.md#jsc-304) (`packages.md`, the shared package's version).

## The milestone's readings

- "`StringTrimEnds`'s classification, which moves to the shared predicate": it moved at SJL-3.4, as
  JSD-0070 records; nothing of it was left to delete.
- "the lexical half of `JsRealm.Json`'s parser, and `JsonQuote`": deleted; the dispatch that chose a
  value's kind, `JsonParseValue` and `JsonParseUnrecorded`, went with it, and the building half is
  `JsonBuild`.
- "a validator that Broiler.VM's compile-time JSON module translation uses": the translation reads the
  document once, with the reader the validator is - `JsonReader.Validate` is that reader read to its end -
  and writes as it reads, so a document is not read twice.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-5 is met.** Its evidence is bundle `sjl-5-001`, read under its rule, and the ledger
  records the result.
- **J8**, the JSON module depth this family's parse options allow, which is SJL-12's.
- **Broiler.JS's cut-over**, SJL-5.5, which its own changelog records.

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 2 reads a code point, normalises an index, walks a template, encodes or decodes,
  or reads a JSON token itself;
- a vector that matched before this change does not after it, a fuel-parity twin completes at a different
  allowance or an exhaustion verdict moves, or a JSON module's outcome, artifact or diagnostic code
  changes, without a dated decision admitting it.
