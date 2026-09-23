<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0027 — Intl stays deferred, what the locale-named methods answer instead, and the data an Intl would need

**Status:** Proposed. 2026-09-21. **Two halves with different standing.** The deferral itself is
[JSD-0002](0002-feature-manifest-allocation.md)'s, restated here with the consumer limitation it
never wrote down; the record of what the locale-named methods answer is a measured fact of the
tree at `484f389`. **The data strategy in section 5 and the slice plan in section 7 are
recommendations, owner decision pending** — nothing in this record approves a dependency, a
licence or a package-size budget. Nothing here has been read by a human.

**Amendment, 2026-09-21 (VM-FIX-B, follow-up N1; local validation, not accepted).** Sections 1
and 2 record the tree at `484f389`, and their two `toLocaleString` rows describe that tree, not the
current one. `Array.prototype.toLocaleString` is now an own method (`length` 0) running the
ECMA-262 algorithm of section 2: each non-nullish element's `toLocaleString` is invoked with no
arguments, a non-callable one throws `TypeError`, the results are joined with `","`, and each
element is metered. `%TypedArray%.prototype.toLocaleString` runs the same algorithm and throws
`TypeError` for a non-callable method instead of falling back to `ToString`, and the
`JsRealm.Binary.cs` remark is corrected. N1's acceptance in section 7 is met except for one test:
`TypedArray/prototype/toLocaleString/detached-buffer.js` still fails, because this profile's
`$262.detachArrayBuffer` refuses (no `ArrayBuffer` can be detached here), which predates N1 and is
not a `toLocaleString` defect. N1 is therefore not complete until that test passes or this record
excludes it by name. The retained focused cases are cases 30-39 of the differential probe
`src/tests/differential/the-reference-key-and-exponent-edges.js`. The consumer limitation in
section 6 no longer lists the `Array.prototype.toLocaleString` defect.

**Owner:** MaiRat. **Co-signer:** none. **Both roles are held by one person**, and this record
does not claim the co-signature is independent — there is no second signature to claim it of.

**Milestone:** none advanced. Card D01 of the JSeal VM feature plan asks for this decision;
`broiler.javascript.intl` stays an allocated, unminted manifest identity.

**Context.** JSD-0002 allocated `broiler.javascript.intl` and deferred it "until it has a run".
The wide manifest ([JSD-0021](0021-the-wide-bring-up-manifest-and-format-version-2.md)) meanwhile
ships every ECMA-262 method whose name contains *locale*, each with some fallback. Nobody had
written down what those fallbacks answer, which of them the language permits, or what data an
implementation of ECMA-402 would stand on. The [parity roadmap](../roadmap.parity.md) carries one
paragraph on it, and one clause of that paragraph is not quite what the tree does (section 2).

---

## 1. What the locale-named methods answer today

Probed with the end-user host built from this tree
(`Broiler.VM.Composition.JavaScript.Cli --quiet`, wide manifest, the composition's own
`InvariantGlobalization=true`), and contrasted with Node 24.17 (V8 with full ICU, run on a machine
whose locale is `de-DE` and whose zone is `Europe/Berlin`) only to show where a locale would
change the answer. `d` is `new Date(Date.UTC(2020, 0, 2, 3, 4, 5))`.

| Method | Where | This profile answers | Node, for contrast |
|---|---|---|---|
| `typeof Intl`, `'Intl' in globalThis` | [`docs/realm/globals.txt`](../realm/globals.txt), absent-globals block of the [status ledger](../roadmap.status.md) | `"undefined"`, `false`; `Intl.Collator` is a `ReferenceError` | `"object"`, `true` |
| `Number.prototype.toLocaleString` | `JsRealm.Number.cs` — the body is `JsNumberFormat.ToJsString` | **exactly `toString`**: `1234567.891` → `"1234567.891"`, `1e21` → `"1e+21"`, `-0` → `"0"`, `1e-7` → `"1e-7"` | `"1.234.567,891"`, `"1.000.000…"`, `"-0"`, `"0"` |
| `String.prototype.localeCompare` | `JsRealm.String.cs` — `Math.Sign(string.CompareOrdinal(…))` | **ordinal over UTF-16 code units, then sign**: `'a'` vs `'B'` → `1`; `['b','a','ä','Z']` sorts to `Z a b ä` | `-1`; `a ä b Z` |
| `String.prototype.toLocaleUpperCase`, `toLocaleLowerCase` | `JsRealm.String.cs` — the same `StringChangeCase` as `toUpperCase`/`toLowerCase` | **exactly the non-locale forms**: `'i'.toLocaleUpperCase('tr')` → `"I"` | `"İ"` |
| `Date.prototype.toLocaleString` | `JsRealm.Date.cs` — `DateToFullText` | **exactly `toString`**: `"Thu Jan 02 2020 03:04:05 GMT+0000 (Coordinated Universal Time)"`; an invalid date (`new Date(NaN)`) answers `"Invalid Date"` (`DateInvalidText`), as do the two methods below | `"2.1.2020, 04:04:05"`; `"Invalid Date"` for all three |
| `Date.prototype.toLocaleDateString` | `JsRealm.Date.cs` — `DateToCalendarText` | **exactly `toDateString`**: `"Thu Jan 02 2020"` | `"2.1.2020"` |
| `Date.prototype.toLocaleTimeString` | `JsRealm.Date.cs` — clock text plus the fixed zone tail | **exactly `toTimeString`**: `"03:04:05 GMT+0000 (Coordinated Universal Time)"` | `"04:04:05"` |
| `%TypedArray%.prototype.toLocaleString` | `JsRealm.Binary.cs` | each element's own `toLocaleString`, joined with `,` — so `toString` of each number: `"1234.5,2"`. **When the element's `toLocaleString` is not callable it silently uses `ToString(element)`**: after `Number.prototype.toLocaleString = 5`, or after deleting it and `Object.prototype.toLocaleString`, `new Int8Array([1,2]).toLocaleString()` → `"1,2"` | `"1.234,5,2"`; the non-callable case throws `TypeError` |
| `Array.prototype.toLocaleString` | **no own property**; the name resolves to `Object.prototype.toLocaleString` (`JsRealm.Object.cs`), which calls `this.toString()` | **`join`, not per-element `toLocaleString`**: `[{toLocaleString(){return "L"}, toString(){return "S"}}].toLocaleString()` → `"S"` | `"L"` |
| `Object.prototype.toLocaleString` | `JsRealm.Object.cs` | `this.toString()`, as the specification says | same |
| `BigInt.prototype.toLocaleString` *(added 2026-09-21, card B05)* | `JsRealm.BigInt.cs` — the body is `JsBigInt.ToDecimalString` after `thisBigIntValue` | **exactly `toString()` with no radix**: `12345n` → `"12345"`, `-7n` → `"-7"`; its arguments are ignored and never coerced, and a receiver that is not a BigInt or a BigInt object is a `TypeError` | `"12.345"`, `"-7"` |

`BigInt.prototype.toLocaleString` was not in the table when this record was written, because
`BigInt` was not in the profile (`typeof BigInt` was `"undefined"`). *(Amended 2026-09-21: card B05
admitted it, and it joined the table and the FIXED row of section 6 in the same change, on the
terms section 6 set for it.)*

Four properties hold across the whole table and are what a consumer may rely on:

- **Every locale and options argument is ignored, and none is coerced.** `(1).toLocaleString('not
  a tag!')`, `'a'.localeCompare('b', 'x', {sensitivity: 'base'})` and
  `d.toLocaleString('de-DE', {timeZone: 'Europe/Berlin'})` answer as if the argument were absent,
  where an ECMA-402 engine throws `RangeError` on the first two. A locale object whose `toString`
  records that it ran is never called.
- **The time zone is UTC and cannot be anything else.** `JsRealm.Date.cs` fixes the local zone to
  UTC (`getTimezoneOffset()` is `0`) so that a benchmark does not depend on where it ran.
- **No host culture reaches a guest answer.** The profile's sources use `System.Globalization`
  only through `CultureInfo.InvariantCulture`, `NumberStyles` and `CharUnicodeInfo` — no
  `CurrentCulture`, no `CompareInfo`, no `TimeZoneInfo.Local`. Every composition project here sets
  `InvariantGlobalization=true` except `Broiler.VM.Composition.JavaScript.Android`, and the profile
  does not rely on the setting: casing through `CultureInfo.InvariantCulture.TextInfo` was probed
  under both ICU and invariant globalization on .NET 10 for the characters in section 2 and
  answered identically. That is a sample, not a proof over the code space.
- **Receivers are still checked.** `Number.prototype.toLocaleString.call('x')` and
  `Date.prototype.toLocaleString.call({})` throw `TypeError`, as they must.

## 2. Which of those answers the language permits without ECMA-402

ECMA-262 (the pinned edition, [`docs/specification/`](../specification/README.md)) specifies each
of these methods twice: once by reference to ECMA-402, and once for "an implementation that does not
include the ECMA-402 API". This profile is the second kind, so the second text governs. Measured
against it:

| Method | Verdict | Why |
|---|---|---|
| `Number.prototype.toLocaleString` | **Permitted** | The text allows returning "the same thing as `toString`" — "permissible, but not encouraged". |
| `Date.prototype.toLocale*String` | **Permitted** | The contents are implementation-defined. |
| Ignoring the reserved arguments everywhere | **Required, and met** | An implementation without ECMA-402 "must not use those parameter positions for anything else". |
| `toLocaleUpperCase` / `toLocaleLowerCase` equal to the plain forms | **Permitted** for the locale half | The difference is intended only "where the rules for that language conflict", and this profile's current locale is the root. |
| **The plain forms those delegate to** | **Not conformant** | `toUpperCase`/`toLowerCase` must apply the Unicode Default Case Conversion including the locale-insensitive `SpecialCasing.txt` mappings. `.NET`'s invariant `TextInfo` is a simple, one-to-one mapping and also omits the Turkic dotted/dotless pair: `'ß'.toUpperCase()` stays `"ß"` (should be `"SS"`), `'ﬀ'.toUpperCase()` stays `"ﬀ"` (`"FF"`), `'ΣΣ'.toLowerCase()` gives `"σσ"` (`"σς"`), `'İ'.toLowerCase()` stays `"İ"` (`"i̇"`). |
| **`localeCompare`** | **Not conformant in one clause** | An ordinal comparison is a consistent total order, which is required, but the method "must recognize and honour canonical equivalence", returning `+0` for canonically equivalent strings. `'ä'.localeCompare('ä')` answers `-1`. |
| **`Array.prototype.toLocaleString`** | **Not conformant** | The method is specified even without ECMA-402 — it invokes each non-nullish element's `toLocaleString` and skips `null`/`undefined`. Inheriting `Object.prototype.toLocaleString` gives `join`, which calls `toString`. |
| **`%TypedArray%.prototype.toLocaleString`** | **Not conformant in one clause** | The method is "the same algorithm as `Array.prototype.toLocaleString`" with `TypedArrayLength` for the length, so each element goes through `Invoke(element, "toLocaleString")`, which throws `TypeError` when the property is not callable. `JsRealm.Binary.cs` instead tests `IsCallable` and falls back to `ToStringValue(element)`. Only a program that removes or overwrites `Number.prototype.toLocaleString` can see it, and no test in the pinned test262 revision exercises that path (the `return-abrupt-*` and `calls-*` tests install callable methods and so take the conformant branch). |

**The four non-conformant rows are not Intl work** and must not wait for it. They are ECMA-262
obligations of exactly the implementation this profile is. They are listed as follow-ups N1-N3 in
section 7 (the two `toLocaleString` rows share N1, since both are the one `Array` algorithm). Two
need Unicode data — full case mapping (N2) and canonical equivalence (N3). Card F07 is choosing
the Unicode data source, but its card scopes the tables to the F08/F09 behaviours (normalization
and regex properties), and F08 explicitly excludes case mapping. N3 needs only what F08 needs
(the decomposition data). **N2 needs more than F07 is asked for**: the case fields of
`UnicodeData.txt` and `SpecialCasing.txt`. That is recorded here as a note for the F07 owner —
extend the table set under the same pin, or N2 will have to open a second data boundary — not as
something F07 already provides.

**Two statements in the tree are inaccurate and are recorded here rather than repaired here.** The
`SetupTypedArrayLaterAdditions` remarks in `JsRealm.Binary.cs` say the typed-array method "does
what this profile's `Array` one does, which is to call each element's own `toLocaleString`" — the
typed-array one does that, the `Array` one does not exist — and say "the realm is built with
globalization invariant", which is a property of the compositions' project files, not of the realm.
The parity roadmap's "`Array.prototype.toLocaleString` does not exist at all" is right about the own
property but a reader would conclude the call throws; it answers `join`'s text.

## 3. What an Intl needs that this profile does not hold

| Need | Data | Who would hold it |
|---|---|---|
| Locale identification, canonicalization, negotiation | BCP 47 / CLDR likely-subtags and alias tables | CLDR |
| `Intl.Collator` and a locale-aware `localeCompare` | The root collation (DUCET as CLDR tailors it) plus per-locale tailorings; normalization for canonical equivalence | CLDR + UCD (normalization overlaps card F07/F08) |
| `Intl.NumberFormat` | Number symbols, decimal/percent/currency patterns, grouping, numbering systems, currency digits | CLDR |
| `Intl.DateTimeFormat` | Calendar names and patterns, skeleton-to-pattern availability, day periods, zone names — **and a time-zone database**, because a `timeZone` option other than UTC needs offsets and transitions | CLDR **and** IANA tzdb |
| Later: PluralRules, RelativeTimeFormat, ListFormat, DisplayNames, Segmenter, DurationFormat, Locale | Plural rules, unit/relative patterns, list patterns, display names, segmentation rules | CLDR + UCD |

**The conformance evidence exists and is already pinned.** The archived test262 revision
(`src/tests/conformance/pins/`) holds 3,324 `.js` files under `test/intl402/`: 65 under
`Collator`, 253 under `NumberFormat`, 248 under `DateTimeFormat`, 66 under `Intl`, 152 under
`Locale`, and 2,006 under `Temporal`. The ECMA-262 fallback tests are separate and small — 13 for
`localeCompare`, 12 for `Array.prototype.toLocaleString` (3 of them need `resizable-arraybuffer`), 39 for
`%TypedArray%.prototype.toLocaleString` (14 of them under `BigInt/`), 54 for the two `toLocale*Case` methods,
4 each for `Number` and the three `Date` methods. Nothing in this repository selects `intl402`
today.

## 4. Data sources considered

The criteria are this profile's existing ones: **the same program answers the same bytes on every
host and publish mode** (roadmap section 6's fixed-or-varying rule), trimming and Native AOT stay
clean, a composition that declines `broiler.javascript.intl` carries none of its cost, and every
dependency is pinned, licensed and reviewable.

| Option | What it would mean | Assessment |
|---|---|---|
| **Host ICU through `System.Globalization`** (`CultureInfo`, `CompareInfo`, `NumberFormatInfo`, `TimeZoneInfo`) | What the seed's Intl does in part — `Broiler.JS`'s `JSIntl.cs` reaches `CultureInfo.GetCultureInfo`, `CompareInfo` and `CultureInfo.CurrentCulture` | **Rejected.** The answer depends on the host's ICU build (on Windows the OS's `icu.dll` — 2,769,976 bytes on the probing machine, versioned with Windows; on Linux the distribution's `libicu`), so the same program answers differently on two hosts and no corpus entry can pin it. Every composition here but the Android one sets `InvariantGlobalization=true`, under which there is no locale data to reach at all. .NET's culture data is also not ECMA-402-shaped (patterns and resolved options differ). |
| **App-local ICU** (a pinned native ICU package loaded instead of the OS one) | One ICU version for every host | **Rejected for now.** Native binaries per runtime identifier, tens of megabytes of data, and a third-party native library to ship and load, which this component does not do today (its only native calls are the operating system's own memory-mapping entry points). It pins a version but not a portable one. |
| **ICU4X or another native CLDR engine** | Modern, data-slicing, CLDR-native | **Rejected for now** for the same native-interop reason; worth re-reading if the core ever admits a native satellite. |
| **`UnicodeCldr.LocaleData` 0.1.0-preview.1** (Broiler.Unicode, commit `5d7136fd`) | The managed CLDR package the seed references. Its assembly is 1,218,560 bytes — 2.8 times this profile's own 432,128-byte assembly | **Candidate input, not accepted.** Managed and dependency-free, Apache-2.0 code over Unicode-licensed data. But it is a preview, self-described as unreviewed generated tables; its public surface covers number symbols, currency, plural rules, list/unit/relative-time patterns, likely subtags, day periods, zone names and two calendars — **no collation data and no date-time skeletons**, which is why the seed falls back to `CultureInfo` for exactly those; it does not publish the CLDR version it was generated from; and it is served from a private package feed. |
| **Profile-owned generated CLDR subset** | A generator in this repository reads a pinned CLDR JSON release and emits compact managed tables, in a separate assembly | **Recommended** (section 5). Deterministic, AOT-clean, versioned by the pin, and sized by the locale list rather than by CLDR. It is also the most work. |
| **Nothing — stay with the fixed fallbacks** | Section 1, with N1-N3 fixed | **Taken until a consumer exists** (section 6). |

## 5. The data strategy an implementation would use — proposed, owner decision pending

1. **No host globalization.** Intl answers come from data this component pins; no
   `CultureInfo`/`CompareInfo`/`TimeZoneInfo` call may influence a guest-visible value. The
   profile's default locale is a fixed tag chosen by the composition (proposed: `en-US`, with `und`
   as the root), never the process culture.
2. **CLDR pinned by release and digest**, the same shape as the language-edition and test262 pins
   ([JSD-0019](0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md),
   [JSD-0020](0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md)); the Unicode
   version agrees with the one card F07 pins, so normalization, casing and collation read one UCD.
3. **Generated, checked-in or reproducibly regenerated tables** with no network access at build
   time, in an assembly of their own that only a composition admitting `broiler.javascript.intl`
   references. **A composition that declines the manifest pays zero bytes.**
4. **A locale list, not "all of CLDR".** Proposed first list: `und`, `en`, `en-US`, `de`, `de-DE`
   — enough for the acceptance datasets below and nothing speculative. Every further locale is a
   size line a reviewer can read.
5. **Time zones: UTC and fixed offsets first.** IANA tzdb is a second, separately pinned data set
   and a separate owner decision; until it is taken, `timeZone` accepts `"UTC"` and `±hh:mm`
   offset identifiers and refuses every IANA name with a `RangeError` that names the limitation.
   The profile's own local zone stays UTC regardless.
6. **Every Intl operation is metered and bounded** like the rest of the library — formatting and
   collation charge per input and output unit, and no table lookup allocates proportionally to
   CLDR rather than to the input.

**What the owner has to decide, and what each decision needs:** (a) whether to generate in-tree
from CLDR JSON or to adopt `UnicodeCldr.LocaleData` after it publishes its CLDR version and passes
human review — needs the owner's call on who maintains the generator; (b) acceptance of the Unicode
License v3 for CLDR and UCD data in this repository, with the notice file it requires — a licensing
decision this record cannot take; (c) a size budget for the Intl assembly per shipped locale —
needs a measured prototype of I0 below, not a guess; (d) whether IANA tzdb is ever in scope.

## 6. The decision

- **`Intl` stays deferred.** `broiler.javascript.intl` is not minted, `Intl` stays on the
  absent-globals list, and `typeof Intl === "undefined"` remains the true answer.
- **No empty or partial `Intl` object is installed as a stand-in.** The global appears in the same
  change that ships its first constructor with that constructor's acceptance dataset passing — not
  before, and not as an object carrying only `getCanonicalLocales` so that feature detection
  succeeds. A detection that succeeds on an object that cannot format is worse than a
  `ReferenceError`, because a program takes the Intl path and then fails inside it.
- **The locale-sensitive surface of the wide manifest is FIXED, and this record is its row** in
  the fixed-or-varying list of roadmap section 6: the answers in section 1's table are this
  profile's, independent of host culture, host ICU and host time zone, and a corpus entry may pin
  them — **except** the four rows section 2 marks non-conformant (`Array` and `%TypedArray%`
  `toLocaleString`, the plain case forms and so the `toLocale*Case` ones, and `localeCompare` on
  canonically equivalent strings), which may be pinned only as known failures until N1-N3 land.
- **Methods that arrive later join the same row on the same terms.** When card B05 admits
  `BigInt`, `BigInt.prototype.toLocaleString` must answer exactly what `BigInt.prototype.toString`
  answers with no radix (the text's "permissible, but not encouraged" option, as for `Number`),
  ignore and never coerce its arguments, check its receiver, and be added to this FIXED row in the
  same change. The `BigInt` typed arrays of card B07 inherit `%TypedArray%.prototype.toLocaleString`
  and so its N1 behaviour.
- **The consumer limitation, stated once so a consumer can quote it:**

  > The Broiler.VM JavaScript profile does not implement ECMA-402. `Intl` is absent. Locale and
  > options arguments to `toLocaleString`, `toLocaleDateString`, `toLocaleTimeString`,
  > `toLocaleUpperCase`, `toLocaleLowerCase` and `localeCompare` are ignored. Numbers format as
  > `toString`, dates as `toString`/`toDateString`/`toTimeString` in UTC, case mapping is
  > locale-independent, and `localeCompare` orders by UTF-16 code unit. Output does not depend on the
  > host's culture or time zone. Known defects, until they are fixed: case mapping is simple and
  > one-to-one (`'ß'.toUpperCase()` stays `"ß"`, no final sigma, no `İ`/`ı` special cases),
  > and `localeCompare` does not treat canonically equivalent strings as equal. Code that needs
  > locale-correct formatting or collation must format on the host side and pass strings in.

- **What reopens the deferral:** a named consumer with a workload that needs a specific
  constructor, recorded on the roadmap. None exists today: JSeal's tracked non-documentation files
  contain no `Intl.`, `toLocale…(` or `localeCompare(` call. When one appears, it schedules the
  first slice below that it needs, not the family.

## 7. Follow-up slices

**Owed now, independent of Intl** (ECMA-262 conformance of the fallbacks):

| Slice | Work | Accept |
|---|---|---|
| **N1** — `Array.prototype.toLocaleString`, and the typed-array one on the same algorithm | An own `Array` method per ECMA-262: `ToObject`, `LengthOfArrayLike`, separator `","`, skip `null`/`undefined`, `Invoke(element, "toLocaleString")`, metered per element. `%TypedArray%.prototype.toLocaleString` drops its `ToStringValue` fallback and throws `TypeError` for a non-callable `toLocaleString`, as `Invoke` does. Correct the `JsRealm.Binary.cs` remark | The 9 `built-ins/Array/prototype/toLocaleString` tests that need no `resizable-arraybuffer` pass. The 21 `built-ins/TypedArray/prototype/toLocaleString` tests outside `BigInt/` that need neither `resizable-arraybuffer` nor `BigInt` pass, and so does `TypedArrayConstructors/prototype/toLocaleString/inherited.js`. Excluded by name, not failed: `resizable-buffer.js`, `user-provided-tolocalestring-grow.js` and `user-provided-tolocalestring-shrink.js` in both directories, plus the typed-array `return-abrupt-from-this-out-of-bounds.js`, which must pass once cards F04-F06 land; the 14 `BigInt/` tests and `bigint-inherited.js`, which must pass once B07 lands. No pinned test reaches the non-callable path, so these focused cases are retained: `[{toLocaleString(){return "L"}}].toLocaleString()` is `"L"`; `[{toLocaleString: 5}].toLocaleString()`, and `new Int8Array([1]).toLocaleString()` after `Number.prototype.toLocaleString = 5`, throw `TypeError`; `[null, undefined, 1].toLocaleString()` is `",,1"` |
| **N2** — full default case mapping | `toUpperCase`/`toLowerCase` (and so the `toLocale*` forms) apply UCD simple mappings plus the unconditional and final-sigma `SpecialCasing.txt` rules, from the Unicode version card F07 pins; no `TextInfo`. **Prerequisite:** F07's table set extended to the `UnicodeData.txt` case fields and `SpecialCasing.txt` (section 2), which the F07 card does not currently ask for | The 56 `toUpperCase`/`toLowerCase` tests and the 54 `toLocale*Case` tests pass; `'ß'`, `'ﬀ'`, `'ΣΣ'`, `'İ'`, `'ı'` and a supplementary-plane pair answer as ECMA-262 requires; results identical under ICU and invariant globalization |
| **N3** — canonical equivalence in `localeCompare` | Compare the NFD forms (after card F08) and fall back to code-unit order; keep it a total order and metered | The 13 `localeCompare` tests pass, including the canonical-equivalence ones; `'ä'.localeCompare('ä') === 0`; ASCII fast path unchanged |

**Scheduled only when a consumer triggers section 6**, each its own slice with its own dataset and
none implying the next:

| Slice | Work | Accept |
|---|---|---|
| **I0** — data boundary and locale core | The pinned CLDR input and generator (section 5), the separate data assembly, `broiler.javascript.intl` minted with its admission scope; locale canonicalization, likely subtags and negotiation as **internal** operations. **Installs no global** | Generation is byte-reproducible; a composition without the manifest references no data assembly (architecture rule); measured assembly size per locale recorded for owner decision (c) |
| **I1** — `Intl.Collator` and a locale-aware `localeCompare` | Root collation plus the section 5 locale list's tailorings, `sensitivity`, `ignorePunctuation`, `numeric`, `caseFirst`; `resolvedOptions`, `supportedLocalesOf`; `localeCompare` routes through it; `Intl` global appears here with only `Collator` and `getCanonicalLocales` | The UCA `CollationTest` conformance file for the pinned version (non-ignorable and shifted); test262 `intl402/Collator` and `intl402/String/prototype/localeCompare`, every exclusion named; a retained dataset of German and English orderings checked against Node's ICU at a recorded ICU version |
| **I2** — `Intl.NumberFormat` (decimal, percent, currency) | Digit options, rounding, grouping, `signDisplay`, `formatToParts`; `Number.prototype.toLocaleString` routes through it. Units, compact and scientific notation are a later slice | test262 `intl402/NumberFormat` subset for the admitted options, exclusions named; a retained `(locale, options, value) → string` dataset covering `-0`, `NaN`, `±Infinity`, `1e21`, half-even/half-expand ties and grouping at 3, 4 and 5 digits |
| **I3** — `Intl.DateTimeFormat`, UTC and fixed offsets | Gregorian only, `dateStyle`/`timeStyle` and the component options the section 5 locales need, `formatToParts`; the three `Date.prototype.toLocale*` methods route through it; IANA names refused by name | test262 `intl402/DateTimeFormat` subset for the admitted options, time-zone-name tests excluded by name; a retained dataset over the full time-value range including year 0, negative years and ±8.64e15 |
| **I4 and later** | PluralRules, ListFormat, RelativeTimeFormat, DisplayNames, Segmenter, DurationFormat, Locale, tzdb-backed time zones, `toLocale*Case` tailoring for `tr`/`az`/`lt` | Each opened by its own consumer and its own card |

## 8. What was rejected

- **An `Intl` object with nothing behind it**, or with `getCanonicalLocales` alone. It makes
  `typeof Intl` lie to feature detection, which is the only question most programs ask.
- **Routing the fallbacks through `CurrentCulture`** to "look localized". It makes the fixed row
  varying, so no corpus entry could pin any of section 1, and a server's output would change with
  the account it runs under.
- **One Intl slice.** The card excludes it, and it would couple three unrelated data sets
  (collation, number patterns, calendars and zones) so that none could land before all three.
- **Treating the four non-conformant fallbacks as Intl gaps.** They are ECMA-262 gaps of an
  implementation without ECMA-402, and deferring them with Intl would hide them behind a deferral
  that may never end.

## 9. What would falsify this

- **A guest-visible answer that changes with the host's culture, ICU or time zone.** Section 1's
  claim that none does is then false, and the fixed row with it.
- **An `Intl` global, or any `Intl` property, observable while `broiler.javascript.intl` is
  unminted.**
- **A consumer with a real locale workload that this record's limitation text did not warn**,
  which would mean the limitation was not where a consumer reads.
