# Milestone SJL-5 — Text algorithms: whitespace, UTF-16, substitution, URI, base64 and hex, JSON lexing

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/text.md)

- **Objective.** The text algorithms below come from `Broiler.ECMAScript.Algorithms` in both
  engines, and each engine's own copies are gone:
  - the string helpers both engines implement for themselves: `trim`, well-formedness, padding
    under a length policy, relative-index normalisation;
  - replacement-template parsing, as data, for `GetSubstitution`;
  - the URI codecs;
  - the base64 and hex codecs with partial-write semantics;
  - JSON lexing, validation, quoting and gap normalisation.
- **Owner roles.** The Broiler.JS maintainer and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS, Broiler.VM.
- **Waits on.** SJL-3.

**What deliberately stays in the engines.** Some steps of these algorithms have effects a script can
observe, so they stay in each engine's adapter: the evaluation of replacement parts (named groups
are read with `Get`, which can run a getter), and the `JSON.parse` and `JSON.stringify` drivers
(reviver, `toJSON`, replacer, proxies, source records, the raw-JSON brand).
**String search stays too**: ordinal `indexOf`, `includes` and `startsWith` are base class library
calls, so there is no algorithm to share. Broiler.JS's culture-sensitive calls are corrected in
Broiler.JS itself and listed.

---

## Work packages

### SJL-5.1 — Area verdict and comparison rules

As in SJL-4.1, one verdict per sub-area:

- whitespace and UTF-16;
- padding and indices;
- substitution templates;
- URI;
- base64 and hex;
- JSON lexing.

### SJL-5.2 — Vectors

The corpus gains vectors for:

- `TrimString` over the whitespace set;
- `isWellFormed` and `toWellFormed` over lone surrogates;
- `padStart` and `padEnd` at the length policy's edges;
- replacement templates: `$$`, `$&`, `` $` ``, `$'`, `$n`, `$nn` and `$<name>` edge cases;
- `encodeURI`, `encodeURIComponent`, `decodeURI` and `decodeURIComponent` over every malformed
  UTF-8 and surrogate case, and `escape` and `unescape`;
- base64 and hex over both alphabets, every last-chunk mode, and partial writes into a short
  destination (the `setFromHex("aaZZ")` shape);
- JSON:
  - lexing and validation: depth at the policy limit, lone surrogates escaped and raw, `1e400`;
  - quoting: control characters, lone surrogates;
  - gap normalisation;
  - the indentation line ending, which is always LF.

### SJL-5.3 — The implementation

The algorithms library gains:

- **Whitespace and UTF-16:** a whitespace predicate (shared with SJL-3), trimming, well-formedness,
  and code-point reading.
- **Padding:** a padding builder under an explicit maximum length. Exceeding it is a status, which
  Broiler.VM maps to `RangeError` at its string ceiling. Broiler.JS today raises a .NET exception.
- **Substitution templates.** A parser turns a replacement template into a list of parts: literal,
  match, before, after, capture *n*, or named capture `<name>`. **Each engine's adapter evaluates the
  parts in order**, so a named-group `Get` happens where the specification puts it.
- **URI:** codecs with status and error position.
- **Base64 and hex:** codecs that write into a caller's span and report what they read and wrote,
  with the alphabet and last-chunk handling as parameters.
- **JSON:**
  - a token and event reader over a span, with a depth limit from the caller's policy and a meter.
    Engines build objects from the events, run the reviver, and record source text;
  - `QuoteJSONString` and gap normalisation;
  - a validator that Broiler.VM's compile-time JSON module translation uses.

### SJL-5.4 — Broiler.VM's cut-over

**Deleted:**

- `StringWellFormed` and the `toWellFormed` loop;
- `StringTrimEnds`'s classification, which moves to the shared predicate;
- `StringPadTo`'s builder;
- the core of the template expansion in `StringExpandDollars`, the inline copy in
  `RegExpReplaceText` and the template walk of `RegExpSubstitute`. Each becomes "parse, then
  evaluate the parts";
- the `GlobalUri*`, `GlobalEscapeText`, `GlobalUnescapeText` and `GlobalUriHexValue` helpers with
  their constant sets;
- `Base64Encode`, `Base64Decode` and its helpers, and `Base64DecodeHex`;
- the lexical half of `JsRealm.Json`'s parser, and `JsonQuote`;
- `JsJsonModule`'s reader.

### SJL-5.5 — Broiler.JS's cut-over

**Deleted:**

- `IsWellFormedUtf16` and `ToWellFormedUtf16`;
- `StringPad`'s builder;
- `JSString.GetSubstitution`, and the initializer's local `GetSubstitution`;
- the `UriHelper` class. Its provenance is cited from a third-party engine and is checked under
  [governance.md](../governance.md#9-licensing-and-provenance);
- the base64 and hex cores of `JSUInt8Array`;
- `JSJSON.QuoteString`;
- `JSJsonParser`'s use of `System.Text.Json` for guest JSON, together with its exception mapping.

**Corrections, listed:**

- the JSON nesting limit inherited from System.Text.Json becomes a policy parameter;
- lone surrogates;
- `1e400`;
- CRLF in indented output on Windows;
- partial writes in `setFromHex`;
- the `split` limit with an empty separator;
- ordinal string search, fixed in Broiler.JS's own search methods;
- forgeable raw JSON, fixed in Broiler.JS's brand check, an engine-side object-model fix.

### SJL-5.6 — Evidence

As SJL-3.6, for this area.

---

## Exit gate

**Home clauses.**

1. Every sub-area that was not refused is implemented under the conventions, and its vectors and
   reference arms pass under the area rule.
2. The `SL` rules, the API baseline and package validation pass, and the Native AOT probe runs.

**Broiler.JS clauses.**

3. The deletion list is absent.
4. test262 per path meets the area rule.
5. Every correction is listed.

**Broiler.VM clauses.**

6. Rules `I1` to `I7` hold, and rule `I5` holds the deletion list.
7. test262 in both forms meets the area rule, and the fuel twins agree.
8. Every root is published and run in three modes.
9. Compile-time JSON modules produce byte-identical artifacts for the retained module corpus, or
   every difference is classified.

**Records.** The area-verdict JSD and Broiler.JS record, and the record of any refused sub-area.

---

## Evidence bundles

`docs/evidence/sjl-5-001/` in each repository.

---

## Does not do

- **It does not share any driver whose steps a script can observe:** the JSON drivers, `split`,
  `replace`, the `Symbol` protocols, or species lookups.
- **It does not share `localeCompare`.** That belongs to SJL-17, under decision `SJL-D-24` (c).
