<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0070 - Text algorithms move to the shared algorithms library: SJL-5's area verdict, sub-area by sub-area

**Status:** Decided 2026-10-14: **all six sub-areas accepted.** The programme's extraction gate (its
roadmap section 5, conditions X1 to X7) holds for each of the text area's sub-areas on its own:
whitespace and UTF-16, padding and indices, substitution templates, URI, base64 and hex, and JSON
lexing. Each moves to `Broiler.ECMAScript.Algorithms` at milestone SJL-5. Nothing in this profile
changes with this record; the cut-over is SJL-5.4's own change, and this record is what it applies.

**Date:** 2026-10-14.

**Owner:** the JavaScript profile owner. **Co-signers:** the Broiler.JS maintainer, as the home's
owner, and the core architecture owner, for rule I1's allow-list and the composition register. **All
three roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the programme's SJL-5, work package SJL-5.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts". **SJL-5 started before
SJL-3 was met**, by the owner's dated choice the programme ledger records; this record moves no gate of
either milestone.

---

## Context

The area's design is the programme's
[`areas/text.md`](../../../../docs/javascript-shared-libraries/areas/text.md). It has no vectors yet:
SJL-5.2 adds them after this record, and the design marks its readings *(reading)* until they do. So the
correspondence is **from code**: it was re-derived by reading both engines at **Broiler.VM `1f253ff`** and
**Broiler.JS `62c42e3`** (the default branches after SJL-4's merge; the later commit on this branch
changes records only). Every member is named by file and line in
[`correspondence.md`](../../../../docs/evidence/sjl-5-001/correspondence.md), which this record's change
retains in bundle `sjl-5-001` as the gate's input, with each difference numbered (W, P, S, B, J, X).
Where a difference is stated with a value, it was worked from the code, and this family's side of most
of them was also run; SJL-5.2's vectors confirm or refute each one, and **a difference they find that
this record does not classify falsifies it**.

The re-derivation found that the area is smaller in one place than the design thought and larger in
others. Trim's classification is already shared, since SJL-3. Broiler.JS holds a third template walk,
six more `AdvanceStringIndex` copies and a culture-sensitive string search the design did not name, and
most of the area's corrections are Broiler.JS's. Section "The design's readings" lists what does not
hold; X6 extends the deletion lists.

## The gate, sub-area by sub-area

### Whitespace and UTF-16

**X1 - met.** Both engines implement `IsStringWellFormedUnicode`, `toWellFormed`'s replacement,
`CodePointAt` and `AdvanceStringIndex` (correspondence section 1). **TrimString's classification is
already shared**: both engines' trim calls the library's `StrWhiteSpaceChar` predicate, which SJL-3
moved, so it is not extracted again; `TrimString` itself, the span trimmed at either end, becomes a
library member beside it so that neither engine keeps the loop.

**X2 - met.** No difference: W1 to W3. The white-space and lone-surrogate scans of the lexers, of the
regular expressions, of `Date.parse`, of `normalize` and of the format are outside the area, and the
correspondence names each with the milestone that owns it: **U+0085 is white space to Broiler.JS's lexer
and to its `RegExp.escape`**, and `normalize` lets a .NET exception through on a lone surrogate there;
none of those moves here.

**X6 - extended.** Broiler.VM: `StringWellFormed`, the `toWellFormed` loop, `codePointAt`'s inline read,
`CreateStringIterator`'s pair test, `ReadCodePoint`, and `RegExpAdvanceIndex`'s body, which calls the
shared `AdvanceStringIndex`; the matcher's own readers in `.Format` stay, because that assembly admits no
shared package (SJL-11). Broiler.JS: `IsWellFormedUtf16`, `ToWellFormedUtf16`, `JSString.CodePointAt`'s
arithmetic, `CodePointEnumerator`'s pair test, and **`AdvanceStringIndex` with its six inline copies**.
The design's item "`StringTrimEnds`' classification" is already done.

### Padding and indices

**X1 - met.** Both engines implement StringPad and the relative-index normalisation, each in many places
(correspondence section 2).

**X2 - met, with three corrections and one policy.**

- **P1 and P2 correct this family**: `padStart` and `padEnd` refuse past the string ceiling before the
  filler is converted, and refuse an empty filler there; the specification converts the filler first and
  returns the string for an empty one.
- **P4 and P5 correct Broiler.JS**: past .NET's largest string, a pad or a repeat lets
  `OutOfMemoryException` through; and `%TypedArray%.prototype.lastIndexOf` ignores `fromIndex` when more
  than two arguments are passed.
- **The string ceiling is an engine policy and becomes a parameter**: this family's is `StringLengthCeiling`,
  2^24 code units, and the shared builder answers a status past it, which this family maps to `RangeError`
  as today. Broiler.JS passes its own and maps the status to `RangeError`, not to a .NET exception.

**X6 - extended.** Broiler.VM: `StringPadTo`'s builder; `StringRelativeIndex` and the relative half of
`ArrayRelative`; the inline normalisations of String `substr`, `at` and `lastIndexOf`, of Array `at`,
`with`, `indexOf`, `lastIndexOf` and `includes`, and of TypedArray `indexOf`, `lastIndexOf`, `includes`,
`at` and `with`. Broiler.JS: `StringPad`'s builder; the relative half of `Relative`; `ClampRelativeIndex`;
and every inline normalisation correspondence section 2 lists, ArrayBuffer's and SharedArrayBuffer's
`slice` and `sliceToImmutable` included. **Clamping without the relative step** (`StringBoundIndex`,
`Clamp`, `ArraySpliceCount`) is a single `Math.Clamp`, and stays.

### Substitution templates

**X1 - met.** Both engines implement GetSubstitution, three times each (correspondence section 3).

**X2 - met, with three corrections.** **S1 corrects this family**: `replaceAll`'s string path converts the
replacement only after a match is found. **S2 and S3 correct Broiler.JS**: a named group's value is
converted with the CLR's `ToString`, not the specification's; and a RegExp whose `Symbol.replace` is
absent is matched as a regular expression, by a template walk that never substitutes `$<name>`, where the
specification searches for its text. Every other reading agrees, `$nn`'s fallback and `$<`'s literal case
included.

**What is shared and what is not.** The library parses a template into parts: literal, `$&`, `` $` ``,
`$'`, capture *n* and named capture *name*. **Each engine evaluates the parts in order**, so a named
group's `Get` and its `ToString` happen where the specification puts them, in the engine, with the
engine's own objects.

**X6 - extended.** Broiler.VM: the template walks of `StringExpandDollars`, of `RegExpReplaceText` and of
`RegExpSubstitute`. Broiler.JS: `JSString.GetSubstitution`, the initializer's local `GetSubstitution`, and
**`JSRegExp.Replace(string, string)`**, whose caller takes the string path after S3's correction.

### URI

**X1 - met.** Both engines implement Encode, Decode, `escape` and `unescape` with the specification's sets
(correspondence section 4).

**X2 - met.** No difference was found.

**Provenance (governance section 9).** Broiler.JS's `UriHelper` cites Jurassic's `GlobalObject.cs` by
commit and line, and Broiler.JS's notices do not yet record whether it derives from it. **It is deleted,
not moved**: the shared codec is written from the specification and from this family's implementation,
which cites no third party. Broiler.JS's notices drop the open entry for `UriHelper` when it is deleted,
and record that it was deleted rather than resolved.

**X6 - extended.** Broiler.VM: `GlobalUriEncode`, `GlobalUriDecode`, `GlobalEscapeText`,
`GlobalUnescapeText`, `GlobalUriHexValue`, `GlobalUriOctetAt`, `GlobalUriSequenceLength`,
`GlobalUriAppendUtf8`, `GlobalUriAppendOctet`, `GlobalUriAppendCodePoint`, `GlobalUriIsAlphanumeric`,
`GlobalUriInSet` and the five constant sets; and the two identical hex-digit copies `JsonHexDigit` and
`Base64HexDigit`, which call the shared one. Broiler.JS: `UriHelper` whole.

### Base64 and hex

**X1 - met.** Both engines implement the proposal's decode with its three last-chunk modes, the hex
decode, and the two encoders (correspondence section 5).

**X2 - met, with four corrections, all of Broiler.JS.** **B1**: `setFromHex` parses the whole input before
honouring the destination's length, so `setFromHex("aaZZ")` into one byte writes it and then throws, where
the specification reports two read and one written. **B2 and B3**: an options argument that is not an
object is ignored, and a `null` alphabet or last-chunk name is read as the default, where GetOptionsObject
and the option readers refuse both. **B4**: a detached or out-of-bounds array encodes as empty, where the
specification refuses it.

**What is shared and what is not.** The library decodes into a caller's span and reports what it read and
wrote, with the alphabet and the last-chunk handling as parameters, and encodes. Option reading,
receiver validation, bounds and detachment, and the errors stay in the engines. **`Base64Options` is not
base64's alone**: it is this family's GetOptionsObject, which four Intl constructors call, and it stays.

**X6 - extended.** Broiler.VM: `Base64Encode`, `Base64Result`, `Base64SkipSpace`, `Base64Decode`,
`Base64DecodeFull`, `Base64DecodeFinal`, `Base64HexDigit`, `Base64DecodeHex` and `Base64Alphabet`, and
`toHex`'s encoder. Broiler.JS: `Base64DecodeResult`, `DecodeBase64`, `DecodeBase64Chunk`,
`SkipAsciiWhitespace`, `Base64Value`, `TryParseHexByte`, `HexDigitValue`, the inline hex parses of
`FromHex` and `SetFromHex`, and `ToBase64`'s alphabet rewriting.

### JSON lexing

**X1 - met.** This family implements the JSON grammar by hand, twice: `JSON.parse`'s reader and the
compile-time JSON module's. Broiler.JS implements it through System.Text.Json, which is its
implementation in merged code (correspondence section 6). Both implement QuoteJSONString and the gap.

**X2 - met, with seven corrections, all of Broiler.JS, and one policy.** **J1**: the depth limit is
System.Text.Json's default, which nobody chose. **J2 to J4**: an escaped lone surrogate is refused, a raw
one lets a .NET exception through, and `1e400` is refused, where the specification gives the code unit and
`Infinity`. **J5**: the indented output's line ending is the platform's, CRLF on Windows. **J6 and J7**:
the raw-JSON brand is structural, so a frozen object with a `rawJSON` string forges it, and the brand's
property is not enumerable. J6 is an object-model fix in Broiler.JS, not shared code. **The depth limit is
an engine policy and becomes a parameter**: this family's is `JsonMaximumDepth`, 512, past which it raises
`RangeError`, and its JSON module translation's is `JsJsonModule.MaximumDepth`, 1024; each passes its own.
Broiler.JS chooses its own at SJL-5.5 and lists it.

**J8** is recorded and not corrected here: a JSON module deeper than about sixty levels is refused by
this family, because the source the translation writes is parsed under the module's parse options, so the
translation's own limit is never reached. Part C of the bundle's rule holds the translation's artifacts
to the base, so SJL-5 keeps that behaviour; the depth the parse options allow is SJL-12's.

**What is shared and what is not.** The library has a token and event reader over a span, with the depth
limit and a meter from the caller, which reports a failure as a code and an offset; `QuoteJSONString`;
gap normalisation; and a validator, which this family's compile-time JSON module translation uses. Object
and array construction, the reviver, source-text records, the raw-JSON brand and the whole
`JSON.stringify` driver stay in the engines, and so does the translation's writer of the synthetic module.

**X6 - extended.** Broiler.VM: the lexical half of `JsRealm.Json`'s reader - `JsonSkipSpace`,
`JsonParseString`, `JsonParseHex`, `JsonHexDigit`, `JsonParseNumber`, `JsonParseKeyword` and the grammar
tests of `JsonParseObject`, `JsonParseArray` and `JsonValueStart` - with `JsonQuote`, `JsonIsLoneSurrogate`
and `JsonApplySpace`'s normalisation; and `JsJsonModule`'s `Reader`. Broiler.JS: `JSJsonParser`'s use of
System.Text.Json for guest JSON (`Parse`, `ParseWithSource`, `Read`, `ReadArray`, `ReadObject`,
`ReadWithSource`, `GetSourceText`) with `JSJSON.Parse`'s exception filter; `JSJSON.QuoteString`; the gap's
inline normalisation; and the `IndentedTextWriter`.

### X3 - neutral contract, all six sub-areas

Every shared member takes spans, numbers and enumerations and answers values, spans and statuses: no
engine type, no coercion, no property access. A failure is data: a status with a position for URI and
JSON, a status for padding past its maximum, a read and written count with an error for base64 and hex.
Every limit is a parameter: the maximum length, the depth. **Every operation whose work grows with its
input takes the caller's meter**, as SJL-4's kernels do, through `WorkCharge`.

### X4 - no edge between the engines, none into Broiler.VM

The library references neither engine and nothing in this repository. Neither engine references the
other.

### X5 - boundaries hold

Every member is pure computation over its arguments. The library uses no platform globalization - every
comparison in it is ordinal - and no regular-expression or JSON API. The JSON validator serves this
family's compiler, which already references the library for SJL-3's and SJL-4's areas, so no
compiler-free root gains a frontend.

### X7 - owners accept

The JavaScript profile owner and the Broiler.JS maintainer accept all six sub-areas; one person holds both
roles (EX-30).

## The design's readings

Each is corrected in `areas/text.md` beside the reading, which is quoted there.

- **Trim's classification is already shared** (SJL-3), and Broiler.JS's `trimCharacters` with
  `string.Trim(char[])` no longer exists; the design's "equal sets today; other Broiler.JS paths use .NET
  `char.IsWhiteSpace`" is true of the lexer and `RegExp.escape`, which are outside the area.
- **`"x".padEnd(2**40)` raises `RangeError` in Broiler.JS**, not a .NET exception; the .NET exception is
  between .NET's largest string and `int.MaxValue` (P4).
- **This family's `JsRealm.String` `split` and `replace` are unreachable**, overwritten by the RegExp
  setup; `replaceAll`'s string path is the String file's.
- **Broiler.JS has three template walks, not two**: `JSRegExp.Replace(string, string)` is the third (S3).
- **`Base64Options` is GetOptionsObject**, shared with Intl, and is not base64's to delete.
- **This family's JSON module depth is the parse options'**, not the translation's 1024 (J8).
- **String search** is not shared, as the design says, but Broiler.JS's is culture-sensitive (X2 of the
  correspondence), which the design's "Broiler.JS's culture-sensitive calls are corrected in Broiler.JS
  itself" anticipated without naming the methods: `indexOf`, `includes`, `startsWith`, `endsWith` and the
  non-standard `contains`.

## Decision

1. **All six sub-areas move to `Broiler.ECMAScript.Algorithms`**, under a namespace of their own,
   `Broiler.ECMAScript.Text`, at milestone SJL-5, in the shape the design's section 3 gives and this
   record narrows: `TrimString` beside the shared predicate; well-formedness, its replacement,
   `CodePointAt` and `AdvanceStringIndex`; the padding builder under a maximum length, and the
   relative-index normalisation; the template parser; the URI codecs; the base64 and hex codecs; and the
   JSON reader, `QuoteJSONString`, the gap and the validator.
2. **The engines keep** receiver and argument conversion, option reading, typed-array validation and
   bounds, the evaluation of template parts, object and array construction, the reviver, source-text
   records, the raw-JSON brand, the `JSON.stringify` driver, `split` and string search, errors, and the
   charging policy.
3. **This family's charging units are kept.** Every shared operation whose work grows with its input
   charges through the caller's `WorkCharge` exactly what this family charges for it today: the URI codecs
   one unit per step and per continuation octet, the base64 decode `length + 1` before its loop and
   nothing for an empty destination, the hex decode `length + 1` and nothing for an odd length, the JSON
   reader one unit per value and per separator, the skipped white space and each string's length,
   `QuoteJSONString` `length + 1`, and so on for each member the cut-over replaces. **So there is no
   class C2 in SJL-5's rules**: an exhaustion verdict that moves on fuel is a regression. Broiler.JS
   passes no meter.
4. **Policies are parameters, each engine's own:** the string ceiling (this family's 2^24 code units) and
   the JSON depth (this family's 512 for `JSON.parse` and 1024 for the module translation). Broiler.JS
   chooses its own at SJL-5.5 and maps both statuses to `RangeError`.
5. **Corrections** are fixes under decision SJL-D-16 (a). This family's - P1, P2 and S1 - are listed in
   the cut-over's record; Broiler.JS's - P4, P5, S2, S3, B1 to B4 and J1 to J7 - in its changelog at
   SJL-5.5, with **X1 (`split`'s limit), X2 (ordinal search) and J6 (the raw-JSON brand), which are fixed in
   Broiler.JS's own code** and are not shared. **No public Broiler.JS type is emptied**: every member on its
   deletion list is internal or private, so decision SJL-D-25 has nothing to dispose of.
6. **The bases** are bundle `sjl-5-001`'s in each repository, taken at the commit that carries each
   repository's rule, before any code of SJL-5 lands; the rules are committed with this record.

## What this rejects, and why

- **Sharing the drivers** whose steps a script can observe: `JSON.parse`'s object building and reviver,
  `JSON.stringify`, `split`, `replace` and the evaluation of named groups. Each step order is the
  specification's and each touches engine objects.
- **Sharing string search**: the ordinal base class library calls are the algorithm.
- **Sharing the lexers' and the regular expressions' white space**: SJL-13, SJL-14 and SJL-11 own them.
- **Moving this family's JSON module writer**: the writer emits this profile's source text, and part C
  of the bundle's rule holds its artifacts byte for byte.

## Settling condition

This record is settled when SJL-5.4's cut-over lands with the deletion list absent from this family and
every rule of bundle `sjl-5-001` judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record: the corrections it names are applied in the
cut-overs. `areas/text.md`'s readings are corrected beside each, quoting it.

## What this record does not decide

- **Either cut-over.** SJL-5.4 and SJL-5.5 apply this record; each repository's rule judges its own.
- **The vectors.** SJL-5.2 adds them, and they may falsify this record (below).
- **J8's depth**, which SJL-12's parse options decide.
- **A speed claim of any kind.** SJL-5 measures nothing. The risk the design names for Broiler.JS's JSON
  parsing speed is answered, if at all, by a later measurement under its own rule.

## Falsified if

- SJL-5.2's vectors find a difference between the engines, or between an engine and the specification,
  that the correspondence does not classify.
- A shared operation cannot charge this family's units without changing a fuel-parity twin's or probe's
  verdict (part B of the rule).
- This family's JSON module artifacts change under the shared validator for a reason no class of part C
  admits.
