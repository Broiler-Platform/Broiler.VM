# JSON raw values: JSeal slice F17

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers `JSON.rawJSON`, `JSON.isRawJSON` and their use by `JSON.stringify`; it does not mark any
JSP/JSW stage complete and does not implement the reviver's source-text context.

## What changed

- `JSON.rawJSON(text)` (length 1) follows the pinned ES2026 steps: `ToString` (a Symbol throws a
  TypeError), SyntaxError for the empty string, for a first code unit outside `a-z`, `0-9`, `"`,
  `-`, and for a last code unit outside `a-z`, `0-9`, `"`; then the existing `JSON.parse` reader
  must consume the whole text as one value. The permitted first characters exclude `{` and `[`,
  so only a string, number, boolean or `null` passes. The result is a null-prototype,
  non-extensible object with one own `rawJSON` property (enumerable, non-writable,
  non-configurable), i.e. frozen.
- `[[IsRawJSON]]` is the private CLR type `RawJsonObject` in `JsRealm.Json.cs`, which only
  `JSON.rawJSON` constructs. `JSON.isRawJSON(value)` (length 1) is a type test: plain lookalikes,
  frozen null-prototype copies, objects inheriting from a raw value and Proxies over one all
  answer `false`.
- `SerializeJSONProperty` returns the raw text for a branded value after `toJSON` and the replacer
  function have run, before the wrapper unboxing, as the specification orders it. Nesting,
  Array replacers and `space` go through the unchanged object/array writers. A Proxy over a raw
  value serialises as an ordinary object.
- Fuel: `JSON.rawJSON` charges one unit plus what the shared reader charges per character; the
  raw text is counted in the final `JSON.stringify` output charge like any other text.

## Executed on Windows

- New differential probe `src/tests/differential/the-raw-json-values.js` (69 cases). Before the
  change 60 of 69 cases differed from Node v24.17.0 ([probe-before.txt](probe-before.txt): both
  functions were missing, so almost every case answered TypeError). After it,
  `python eng/run-differential.py --only the-raw-json-values --against node --timeout 20` agrees on
  all 69 with no declared divergences; retained answers were written with `--write` after that
  comparison.
- Full retained lane (`python eng/run-differential.py --timeout 20`, console code page 65001): only
  the pre-existing `the-later-library-methods` cases 36 (cube root spelling) and 147 (non-ASCII
  output) differ; no other probe changed.
- Pinned Test262 (`ccaac100`), bytecode form, `--dir test/built-ins/JSON` (165 files, 330
  variants): pass 260, fail 58, unsupported 12 before; pass 290, fail 28, unsupported 12 after.
  No variant that passed before fails after. `json-parse-with-source` is in the suite's standard
  feature section, so these tests were run, not skipped. Every `rawJSON` and `isRawJSON` file
  passes in both modes except `rawJSON/bigint-raw-json-can-be-stringified.js`, which is
  unsupported (BigInt literal; it also needs the reviver source context).
- `dotnet test Broiler.VM.slnx -c Release --no-build` before the change and again after the
  assurance write run and a rebuild: Contract 267/267, Architecture 256/256.

## Not exercised or still failing

- The native form was not exercised (the artifact does not instantiate on this host:
  ProfileFault/UnsatisfiedHostAssumption, pre-existing). The change has no separate native
  implementation.
- Remaining `test/built-ins/JSON` failures are outside this slice and unchanged: the reviver
  source-text context (`parse/reviver-context-source-*`, excluded by the card) and other reviver
  cases in `parse/`, `JSON[Symbol.toStringTag]`, `stringify/property-order.js`, the Array
  replacer's `IsArray`/`LengthOfArrayLike` handling of Proxies (`stringify/replacer-array-*`) and
  `stringify/value-bigint-cross-realm.js`.
- BigInt raw values cannot be exercised: the profile has no BigInt.
