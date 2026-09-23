<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0031 - The Unicode data source, and the build boundary its tables cross

**Status:** Proposed. 2026-09-21. **Not taken.** Owner decisions are pending on the licence
notice and on the size budget ([section 8](#8-what-needs-the-owner-and-exactly-what-would-settle-it)).
The version, the source and the build boundary are recommendations and do not depend on those two
answers. **Nothing here is implemented**: the checkout contains no Unicode data file, no generator
and no generated table, and the probes below are against the tree as it stands. *(Amended
2026-09-22: slices U1 and U2 have been performed and are recorded in
[section 9](#9-2026-09-22-u1-performed-the-owners-decisions-as-given-and-what-u2-built); the owner's
licence and size decisions were given in conversation and are recorded there unsigned. The record
is still proposed, and nothing in it is signed or co-signed.)*

**Owner:** MaiRat, as this profile's owner. **Co-signer needed:** the release owner, because
[`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md) requires that co-signature from any
change that brings in third-party material. **Both roles are held by one person**, and this record
does not claim the co-signature is independent.

**Milestone:** none in the `JS-` series. This is the design slice F07 of the JSeal feature plan
(`docs/roadmap.vm-features.md` in Broiler.JSeal), which F08 (normalization) and F09 (property
escapes) both depend on. Inside this repository it answers part of an open row that already has a
holder: the ledger's [section 3](../roadmap.status.md#3-open-external-dependencies) row for the
Unicode and locale data, held by the profile built-ins owner named in
[JSD-0005](0005-the-seed-waited-on-set-and-snapshot-stop-condition.md). It also answers part of
[JSW-4](../roadmap.workloads.md#jsw-4--regular-expressions-over-the-from-scratch-matcher), whose
remaining dialect differences "are the Unicode data's". **The locale half of that row is not
addressed here.**

**Context.** [Parity section 4.3](../roadmap.parity.md#43-the-types-and-surfaces-that-are-absent)
records that the missing Unicode character database reaches further than the matcher: `normalize`,
`\p{…}`, case conversion and one identifier classification all depend on it. F08 and F09 cannot start until
someone decides which data they read, which version it is, how it enters the build, and what it
costs. The F07 card asks for exactly that: inputs pinned and validated, deterministic generation,
no network access during a build, and a statement of which F08 and F09 behaviours the tables
support.

---

## 1. What the profile does today, verified

The probe was run through the end-user host built from this checkout (`dotnet build
Broiler.VM.slnx -c Release`, then `Broiler.VM.Composition.JavaScript.Cli.exe --quiet`), with
Node v24.17.0 (`process.versions.unicode` = `17.0`) as the comparison engine.

| Surface | Where | This profile | Node |
|---|---|---|---|
| `'abc'.normalize()` | `JsRealm.String.cs`, `normalize` | `"abc"` | `"abc"` |
| `'abc'.normalize('NFX')` | same | `RangeError` | `RangeError` |
| `'e\u0301'.normalize('NFC') === '\u00e9'` | same | **`TypeError`**: ASCII only, tables not held | `true` |
| `'\ud800'.normalize('NFC').length` | same | **`TypeError`** | `1` |
| `new RegExp('\\p{Letter}', 'u').test('a')` | `JsRegExpMatcher` (Format assembly), escape parser | **`SyntaxError`**: property escapes not supported | `true` |
| `new RegExp('\\p{letter}', 'u')` (wrong case) | same | `SyntaxError`, but for the wrong reason | `SyntaxError`: invalid property name |
| `new RegExp('\\p{Letter}').test('p{Letter}')` (no `u`) | same | `true` (Annex B identity escape) | `true` |
| `new RegExp('[\\p{L}]', 'v')` | `JsRealm.RegExp.cs`, flag parser | `SyntaxError`: invalid flags | accepted |
| `/\u212a/iu.test('k')` | `JsRegExpCase` | `true` | `true` |
| `/\uA7CE/iu.test('\uA7CF')`, and the same inside a class or without `u` | `JsRegExpCase` | `false` | `true` |
| `'\u00df'.toUpperCase().length` / `'\u0130'.toLowerCase().length` | `JsRealm.String.cs`, `StringChangeCase` | `1` / `1` | `2` / `2` |
| `'\uA7CE'.toLowerCase()` | same | `"\uA7CE"` | `"\uA7CF"` |
| `var X = 1`, X being U+1D400 written literally as a surrogate pair | `SliceTokenizer.IsIdentifierStart` | `SyntaxError` | admitted |
| `var \u{1D400} = 1` (the same name, escaped) | `SliceTokenizer.ReadIdentifierEscape` | admitted | admitted |
| `var \u0021 = 1`, `var \u0030x = 1`, `var \u{1F600} = 1`, `var x\u{1F600} = 1` | same | **admitted** | `SyntaxError` |
| `1` U+0085 `+1` in source | `SliceTokenizer.IsWhiteSpace` | **`2`** | `SyntaxError` |
| `new RegExp('(?<\u2118>a)')`, and group names U+2160, U+200D (part), U+1D400 | `JsRegExpMatcher.IsGroupName` | `SyntaxError` | admitted |

The escaped-identifier rows were run both as a top-level script and through indirect `eval`, with
the same answers. U+A7CE/U+A7CF is one of **28 case pairs** (U+A7CE, U+A7D2, U+A7D4 and U+16EA0 to
U+16EB8 with their lower-case partners) for which Node's `/[X]/iu` and `/X/iu` match the partner
and this profile's match neither: a probe over all 1,414 `\p{Lu}` code points whose single-code-point
lower case Node's `iu` class matches found exactly those 28 misses. They are Unicode 17.0 case
pairs that the runtime's Unicode 16 data does not have.

**`normalize` is a refusal, not a no-op.** It validates the form, answers an ASCII string
unchanged, and throws a `TypeError` for any other string. [JSC-91](../roadmap.corrections.md#jsc-91)
records why: in globalization-invariant mode the platform's `string.Normalize` returns its input
unchanged and reports it as normalized. The profile chose to refuse rather than return that wrong
answer.

**Globalization mode is not the same in every composition here.** Nine composition roots set
`<InvariantGlobalization>true</InvariantGlobalization>`: Calculator, JavaScript.Cli,
JavaScript.Conformance, JavaScript.ExecutionOnly, JavaScript.SliceCompiler, PolyglotCli,
WebAssembly.Execution, WebAssembly.Harness and Workbench. **The exception is
`Broiler.VM.Composition.JavaScript.Android`.** It references `Broiler.VM.Profile.JavaScript` (and
through it the Format assembly), targets `net10.0-android36.0`, runs on Mono rather than CoreCLR (its
project file says so and turns off both `PublishTrimmed` and `RunAOTCompilation`), is built from
`Broiler.VM.Mobile.slnx` rather than `Broiler.VM.slnx`, and sets no `InvariantGlobalization`. So in
a composition in this repository, the platform's normalization and culture casing run on the
device's globalization support. This record did not run that composition; the point is only that
the platform's answer is chosen by the host, which section 2 relies on.

**The existing consumers of platform Unicode data.** A search of the three profile assemblies'
product source for `CharUnicodeInfo`, `char.Is*`, `TextInfo`, `To{Upper,Lower}Invariant`,
`string.Normalize` and `IsNormalized` finds these, and no others that classify or map non-ASCII
text:

| Call site | Platform call | Language rule it stands for | This record |
|---|---|---|---|
| `SliceTokenizer.IsIdentifierStart` / `IsIdentifierPart` (Compiler) | `CharUnicodeInfo.GetUnicodeCategory` per UTF-16 unit | `ID_Start` / `ID_Continue` | stays permitted; the literal astral spelling is refused because each surrogate unit is classified alone, as `Cs` |
| `SliceTokenizer.ReadIdentifierEscape` / `AppendScalar` (Compiler) | none: the escaped scalar is **not classified at all** | the escape must name an `ID_Start` / `ID_Continue` code point | current defect, recorded; not moved here |
| `SliceTokenizer.IsWhiteSpace` (Compiler, line 1817) | `char.IsWhiteSpace` | `WhiteSpace` = the listed controls plus `Zs` | stays permitted; admits U+0085, which is not `WhiteSpace` (defect, recorded) |
| `JsParser` lines 675 and 1156 (Compiler) | `char.IsLetter` on a token's first character | an `IdentifierName` token, which the tokenizer has already classified | stays permitted; a routing test, not a classification |
| `JsParser.ScanSubstitution` line 6544, `ScanRegularExpression` line 6656 (Compiler) | `char.IsWhiteSpace`, `char.IsLetterOrDigit` | a skip scanner over template substitutions and regex flags; the real tokens are read later | stays permitted |
| `JsNumberFormat.IsWhiteSpace` (profile; also `trim`) | `GetUnicodeCategory == SpaceSeparator` | `WhiteSpace` | stays permitted; `trim` over every BMP code unit agrees with Node (25 code units) |
| `JsRealm.StringChangeCase` (profile, line 788) | `InvariantCulture.TextInfo.ToUpper` / `ToLower` | full case mapping with `SpecialCasing.txt` | stays permitted; wrong for special casing and for 17.0 pairs (rows above) |
| `JsRegExpCase.Fold` / `Upper` / `UpperOf` (Format) | `char.ToUpperInvariant` / `ToLowerInvariant` | `Canonicalize` (simple folding under `u`, simple upper case without) | **the `u` path moves to the table** (section 7); the non-`u` path stays |
| `JsRegExpMatcher.IsGroupName` (Format, line 1259) | `char.IsLetter`, `char.IsDigit` | `RegExpIdentifierStart` / `Part` = `ID_Start` / `ID_Continue` | stays permitted for now; a Format-assembly consumer the same tables could serve (later slice) |

`SpaceRanges` in the matcher (`\s`) is a hand-written range list, not a platform call, and is
correct for 17.0. Hex-digit helpers that call `ToLowerInvariant` on an ASCII digit are not listed.

## 2. Platform APIs: what is permitted and why none of them can serve F08 or F09

A second probe, a .NET 10.0.12 console application with invariant globalization, compared
`CharUnicodeInfo.GetUnicodeCategory` for all 1,114,112 code points with the Unicode 17.0.0
`General_Category` sets. Those sets were taken from the pinned test262 checkout's generated
property-escape tests, which state `Unicode v17.0.0`.

- **`CharUnicodeInfo` / `Rune` / `char.ToUpperInvariant`** read data compiled into the runtime.
  That makes the results deterministic for one runtime build, but they are not pinned to any
  version this profile chooses. **4,804 code points disagree with Unicode 17.0.** 4,584 of them are
  `Lo` characters that the runtime reports as `Cn`, and one is a reclassification: U+0295, which is
  `Ll` in the runtime and `Lo` in 17.0. U+1C89, a 16.0 addition, is known to the runtime, so its
  data is Unicode 16.0. The answers also change whenever the host rolls forward to a newer
  runtime. **These APIs remain permitted where they are used today.** This decision does not move
  those uses. **F08 and F09 must not use them as their data source.**
- **`string.Normalize` / `IsNormalized` is not permitted.** In invariant mode it returned
  `e U+0301` unchanged from NFC and reported `isNormalized=True`. With invariant mode switched off
  (`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0`) it normalized correctly, but threw `ArgumentException`
  for a lone surrogate, which the language requires to pass through. Its output therefore depends
  on a host setting and on the operating system's globalization library, and it is wrong on
  ill-formed strings either way. The Android composition in this repository already runs without
  invariant mode (section 1), and an embedder outside this repository chooses its own
  globalization mode, so one profile build would give different answers on different hosts.
- **`System.Text.RegularExpressions` is forbidden** by rule **N18** throughout the profile's
  product source, so its `\p{…}` classes are not available.
- **No platform API exposes** `Script`, `Script_Extensions`, the ECMAScript binary properties,
  canonical combining classes, composition exclusions, or `CaseFolding.txt`'s simple folding.

## 3. The version: Unicode 17.0.0

ES2026 does not name a Unicode version. The archived edition's conformance clause says a conforming
implementation must use "the latest version of the Unicode Standard", and its normative reference
is the undated `https://unicode.org/versions/latest`. So the version is a pin this profile has to
choose and record, just as [JSD-0019](0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md)
did for the edition.

**Pin: Unicode 17.0.0** (UCD under `https://www.unicode.org/Public/17.0.0/ucd/`). Reasons:

- **It is the version the pinned conformance suite measures.** Every one of the 441 generated
  property-escape tests in test262 `ccaac100` states `Unicode v17.0.0`. A table from any other
  version would fail those tests on purpose.
- **It is the comparison engine's version.** Node v24.17.0 reports Unicode 17.0, so a differential
  probe compares like with like.
- It was the latest version when the pinned edition's revision was cut (2026-03-31).

**Re-pinning works the way JSD-0019 treats errata.** A later Unicode version is a recorded re-pin
and not an automatic update, and it moves **together with** the test262 pin. Moving one without the
other would turn every changed code point into an apparent failure.

## 4. The candidate sources, and why three were rejected

**(a) The platform.** Rejected for the reasons in section 2.

**(b) The `Broiler.Unicode.Properties` package**, `0.1.0-preview.1` in the local NuGet cache,
Apache-2.0 code over Unicode data, built from `Broiler-Platform/Broiler.Unicode` commit `5d7136fd`.
Its `lib/net10.0` assembly is 401,920 bytes. It provides range lists for 53 binary properties,
`General_Category`, `Script` and `Script_Extensions` from UCD 17.0.0, plus a `SpecialCasing` table.
Broiler.JS uses it: `JSRegExp.TranslateUnicodeProperty` translates `\p{…}` into .NET regex
syntax, and `normalize` calls the platform's `string.Normalize`. **Rejected, for five independent
reasons:**

1. **The architecture rules forbid it.** Rule **A2** forbids any `PackageReference` to a Broiler
   package, and rule **N1** forbids the profile assembly any `PackageReference` at all.
2. **Its name resolution does not conform.** `UnicodeProperties.Lookup` lower-cases names and
   drops `_`, ` ` and `-`, so `\p{letter}` and `\p{Script_Extensions=greek}` would resolve. ES2026
   UnicodeMatchProperty says implementations "must not support any other property names or
   aliases" and gives `script_extensions` and `Scx` as invalid examples. Broiler.JS's `NormalizeKey`
   has the same defect. Copying either would copy a defect.
3. **It covers neither F08 nor `iu` matching.** It has no decomposition, combining-class or
   composition data, and no `CaseFolding.txt` mapping.
4. **Its provenance cannot be reproduced.** The UCD input files are not committed and carry no
   hashes: the tools README says to download them. The generated files name scripts
   (`ucdtmp/gen.py`, `gengc.py`, `genscripts.py`) that are not the ones checked in
   (`tools/generate-*.py`). The repository's `HUMAN_REVIEW.md` approves commit `89f134ed` and says
   any later source change invalidates that approval. The package was built from `5d7136fd`, seven
   commits later, and the diff touches `src/UnicodeProperties`.
5. **The runtime cost is avoidable.** The ranges are stored as `Dictionary<string,string>` hex
   text, parsed on first lookup under a global lock and cached.

**(c) Copying that package's generated source into this repository.** Rejected. Rule **A3** forbids
linking the files in from outside the component root, and copying them would bring reasons 2 to 4
with them.

**(d) Unicode Character Database files, retrieved once, hashed, archived in this repository, and
turned into tables by a generator in this repository.** **Recommended.** This follows the precedent
this profile set for the specification (`docs/specification/`) and for test262
(`src/tests/conformance/pins/`): retrieval is a one-time human action, the build reads only files in
the checkout, and a rule holds the bytes to their recorded hashes.

**(e) Downloading at build time or at run time** is out of scope, and the F07 card excludes it.

## 5. The build boundary

**Inputs.** These are the UCD 17.0.0 files and what each one feeds. Test-only files are marked.

| File | Feeds |
|---|---|
| `UnicodeData.txt` | F08 decomposition mappings and canonical combining classes |
| `DerivedNormalizationProps.txt` | F08 `Full_Composition_Exclusion`; F09 `Changes_When_NFKC_Casefolded` |
| `NormalizationTest.txt` | **test only**: F08 conformance vectors |
| `PropertyAliases.txt`, `PropertyValueAliases.txt` | F09 exact names and aliases |
| `DerivedGeneralCategory.txt` | F09 `General_Category` and `Assigned` |
| `Scripts.txt`, `ScriptExtensions.txt` | F09 `Script`, `Script_Extensions` |
| `PropList.txt`, `DerivedCoreProperties.txt`, `DerivedBinaryProperties.txt`, `emoji/emoji-data.txt` | F09 binary properties |
| `CaseFolding.txt` | F09 `Canonicalize` under `iu` (status `C` and `S` rows) |
| the Unicode licence text from the same release | the licence condition (section 8) |
| **not UCD:** `table-nonbinary-unicode-properties.html`, `table-binary-unicode-properties.html`, `table-binary-unicode-properties-of-strings.html` from `tc39/ecma262` at `0248456c` | F09 the set of names ECMAScript admits (see below); archived in `docs/specification/`, not beside the UCD files |

**Pin file.** A `unicode.pin` file in the style of `test262.pin` records the version, source URL,
retrieval date, and each file's byte count and SHA-256. The files are archived beside it and
declared `binary` in `.gitattributes`. The existing `*.txt text eol=lf` line declares them text,
which lets git rewrite their line endings. Declaring them binary guarantees that the bytes checked
out are the bytes that were hashed.

**Generator.** It is written in C# and lives in the architecture test project, following the rule
**J5** pattern in `AssuranceGenerator`: "one function" both generates and checks. With
`BROILER_UNICODE_WRITE=1` it writes the tables. Without that variable, a test regenerates the
tables in memory and asserts that the checked-in files are byte-identical. **C# was chosen over
Python** so that the check runs inside `dotnet test` without needing an interpreter.
Determinism requirements:

- it reads only the pinned files, and refuses a file whose hash differs or whose header line does
  not name version 17.0.0;
- the output is sorted by code point and uses invariant formatting, LF line endings, and no
  timestamp or machine path;
- it takes the set of property names ECMAScript admits from the specification's own property
  tables, and cross-checks those names and aliases against `PropertyAliases.txt` and
  `PropertyValueAliases.txt`, failing on any disagreement.

**The specification's property tables are not archived today.** The archived
`ecma-262-es2026-spec.html` does not contain them: it pulls them in with three `<emu-import>`
elements, `table-nonbinary-unicode-properties.html`, `table-binary-unicode-properties.html` and
`table-binary-unicode-properties-of-strings.html`, and a search of the archived file for
`ASCII_Hex_Digit` finds nothing. Those three files sit beside `spec.html` in `tc39/ecma262` at the
archived revision `0248456c`. The set matters because `PropertyAliases.txt` lists many binary
properties ECMAScript does not admit, so the generator cannot derive the admitted 53 from the UCD
alone. **Decision:** the three files are added to U1's retrieval list, archived in
`docs/specification/` beside the edition, hashed, declared `binary`, and recorded in that
directory's README under the Ecma notice that already covers the edition. They do not change
`JavaScriptLanguageEdition.DocumentDigest`, which names `spec.html` alone. If U1 cannot retrieve
them, the fallback is a hand-written list of the admitted names in the generator, checked only by
test262's 441 generated and 144 grammar tests, and the cross-check and its witness are dropped
from U2.

**Outputs.** Generated `.g.cs` files are committed. **A build needs no network access and no
generator run.** The generated files are product source, so the assurance scanner reads them and
every member in them needs an annotation. **Decision: no rule change.** The generator emits the
annotations itself. Each generated table member carries the existing per-unit hatch that rule J3
already accepts and witnesses (`// Broiler-AI: EXEMPT=<reason>` with `// Broiler-Human: PENDING`,
witness `J3-an-exempt-annotation-records-no-fingerprint.cs.witness`), with a reason naming the
generator and the pin. The containing type carries an ordinary `Origin/IP/Security/Resources`
line whose fingerprint the generator computes with the fingerprint code the assurance tests
already use (`AssuranceProbe.Fingerprint` in the same test project), so a regenerated file is
annotated without a hand edit. The generator itself is test code and, like `AssuranceGenerator`,
carries no annotation. `CODE-ASSURANCE` and `HUMAN_REVIEW` are regenerated through J5 in the
same change.

**Where the tables live.** Each table goes where its consumer is, so no new public surface is
needed:

- property, alias and case-folding tables go in **`Broiler.VM.Profile.JavaScript.Format`**,
  internal to the assembly that holds `JsRegExpMatcher` (`JsCompiler` reaches that assembly through
  the matcher's existing public entry points);
- normalization tables go in **`Broiler.VM.Profile.JavaScript`**, internal to the realm that
  implements `normalize`.

This leaves N1's reference set, the API baselines and the "opens its internals to nobody" clause
unchanged.

## 6. Storage structure, and what it costs

These figures come from UCD 17.0 through Node's ICU, which is the same version. They are
estimates, and the implementing slices must measure the real sizes.

| Data | Count (17.0) | Structure | Estimate |
|---|---|---|---|
| `General_Category` | 4,144 runs (30 values; the 8 groups are unions computed from them) | run table: start code point + value byte | ~17 KB |
| `Script` | 1,717 ranges over 175 values | run table | ~7 KB |
| `Script_Extensions` | 2,087 ranges | range list per script | ~13 KB |
| 53 binary properties | 12,606 ranges (`Any`, `ASCII`, `Assigned` computed, not stored) | range list per property, 3-byte bounds | ~76 KB |
| Canonical decompositions | 2,081 (non-Hangul); 3,665 UTF-16 units fully decomposed | two-stage index + mapping pool | together with the two rows below, ~60-70 KB |
| Compatibility-only decompositions | 3,833 code points, 5,738 units; largest 18 code points (U+FDFA) | same pool, flagged | (above) |
| Primary composites | ~960 | pairs sorted for binary search | (above) |
| Hangul syllables | 11,172 | algorithmic, no table | 0 |
| Simple case folding (`C`+`S`) | about 1,500 in the Basic Multilingual Plane, per `JsRegExpCase`'s own remark, plus the supplementary pairs (the 25 new U+16EA0 pairs among them) | sorted pairs plus orbit table, whole code space | ~10-12 KB |

**How the decomposition rows were counted.** Every code point except surrogates and the 11,172
Hangul syllables was run through Node's `normalize`. "Canonical" counts code points whose NFD
differs from the input (2,081, with 3,665 UTF-16 units of NFD output). "Compatibility-only" counts
code points whose NFD equals the input and whose NFKD differs (3,833, with 5,738 units). An earlier
draft gave 3,849 and 5,778: that was the count of code points whose NFKD differs from their NFD,
which also includes 16 code points that already have a canonical decomposition. Both are estimates
of the same pool; the implementing slice measures the real table.

**Chosen encoding.** The tables are `static ReadOnlySpan<byte>` properties over constant data,
read by binary search. The compiler embeds this data in the assembly image. It needs no static
constructor, allocates nothing, uses no reflection or resource stream, and is safe under full
trimming and Native AOT, which matters because the end-user host publishes with `TrimMode=full`,
`IsAotCompatible` and warnings treated as errors.

**Rejected encodings:**
- **Embedded resources**: a stream, a parse at startup, and a trimming root.
- **Parsed hex strings** (the package's choice): allocation and a lock on first use.
- **A per-property bitmap**: about 139 KB per property for the whole code space.

**Total cost: on the order of 200 KB across two assemblies.** Today
`Broiler.VM.Profile.JavaScript.Format.dll` is 118,272 bytes and `Broiler.VM.Profile.JavaScript.dll`
is 432,128 bytes (Release). The realm always installs `normalize` and `RegExp`, so trimming
removes nothing. Every composition that contains the realm carries the tables, including the
execution-only one and the Android one, and so do the profile's packages. The Android composition
publishes with `PublishTrimmed` false, so nothing there could remove them even in principle; it is
not packable, but its application image carries both assemblies.

## 7. Exactly which F08 and F09 behaviours the tables support

**F08. All four forms, `String.prototype.normalize('NFC' | 'NFD' | 'NFKC' | 'NFKD')`, on any
string:**
- canonical and compatibility decomposition, applied recursively through the pre-expanded pool;
- canonical ordering by combining class;
- canonical composition, honouring `Full_Composition_Exclusion` and blocking;
- Hangul decomposition and composition, done algorithmically;
- supplementary characters;
- lone surrogates pass through unchanged, as a starter with no mapping, which matches the
  comparison engine's length `1`.

The tables let F08 meet its acceptance criteria against every line of `NormalizationTest.txt`, the
14 test262 `String/prototype/normalize` files, and the 4 staging files. They also make
`'e\u0301'.normalize('NFC') === '\u00e9'` true. **They do not support** `localeCompare`, collation,
`Intl`, or case mapping.

**F09. `\p{…}` and `\P{…}` under `u`, not under `v`:**
- the lone forms: a `General_Category` value or alias, or one of the 53 binary properties or their
  aliases;
- the `General_Category=` / `gc=`, `Script=` / `sc=` and `Script_Extensions=` / `scx=` forms, with
  values taken from `PropertyValueAliases.txt`;
- exact spelling, with a `SyntaxError` for anything else;
- escapes inside and outside classes, negated classes, and supplementary code points;
- under `iu`, the Canonicalize step from `CaseFolding.txt` `C` and `S` mappings, so that
  `/\p{Lu}/iu.test('a')` is `true`.

**One Canonicalize per pattern (decision).** The language has one `Canonicalize(rer, ch)`, and a
pattern's property escapes, literals and ordinary classes all go through it. If property escapes
folded with `CaseFolding.txt` while literals and classes kept `JsRegExpCase`'s `lower(upper(c))`
over the runtime's Unicode 16 data, one pattern could answer two ways. The witness is concrete:
U+A7CE is `Lu` in 17.0 with the simple folding U+A7CF, so `/\p{Lu}/iu.test('\uA7CF')` would be
`true` (Node agrees) while `/\uA7CE/iu.test('\uA7CF')` and `/[\uA7CE]/iu.test('\uA7CF')` stay
`false`, as they are today (section 1). **So U4 moves the whole `u`-mode path onto the table:**
`JsRegExpCase.Canonicalize(c, unicode: true)` and `Variants(c, unicode: true)` read the generated
`CaseFolding.txt` `C`+`S` table and its reverse (orbit) table, for supplementary code points as
well, which also replaces the "astral pairs are reconstructed" branch and the hand exceptions for
U+0130, U+0131 and U+017F. The non-`u` path (`Upper`, the simple upper-case mapping with the ASCII
guard) stays on the platform in this decision. That split is consistent within any one pattern,
because property escapes exist only under `u` and a non-`u` pattern never reads the table. Its
known cost is that `/\uA7CE/i.test('\uA7CF')` stays `false`; moving `Upper` onto
`UnicodeData.txt`'s simple upper-case field is a later slice.

The rejected alternative was to close property sets with the existing `JsRegExpCase`, dropping
`CaseFolding.txt` from F09. It would have kept the 28 Unicode 17.0 pairs wrong under `iu` on every
path, and left the three hand exceptions as the only guard against the approximation.

The targets are the 441 generated and 144 grammar tests under `built-ins/RegExp/property-escapes`,
and `new RegExp('\\p{Letter}', 'u').test('a')`.

**Not supported by these tables:**
- the `v` flag;
- the seven properties of strings (the 28 `generated/strings` tests, feature `regexp-v-flag`),
  which would need `emoji-sequences.txt` and `emoji-zwj-sequences.txt`;
- set operations;
- `MaybeSimpleCaseFolding`, which ES2026 applies only when `[[UnicodeSets]]` is true.

Outside `u`, `\p` remains the Annex B identity escape.

## 8. What needs the owner, and exactly what would settle it

**Licence (proposed, owner decision pending).** UCD data files are distributed under the Unicode
Terms of Use, which apply the Unicode License v3 (SPDX `Unicode-3.0`) to data files. That licence is
understood to permit copying and derivative works, provided the copyright and permission notice go
with every copy or with its documentation. **This record has not verified that against a retrieved
file.** The retrieval slice must retain the licence text from the pinned release and check it.

This would be the first entry in `THIRD_PARTY_NOTICES.md` whose material is **compiled into a
profile assembly**. The existing entries say "nothing is derived from it". **The file's reason for
treating the profile as outside its opening claim is no longer true of this checkout.** It says
"the profile packs nothing", held by rule N4. Since commit `d16a6aa` (2026-09-19, "Packable = true
for nuget") all three profile projects carry `<IsPackable>true</IsPackable>`, and the N4 sweep in
`ProjectFileRuleTests.cs` is commented out. So derived tables would ship inside the profile's own
packages, and not only inside published composition images.

The scope is wider than the packages. Every composition image that carries the realm carries the
tables: the five desktop roots that reference `Broiler.VM.Profile.JavaScript` (JavaScript.Cli,
JavaScript.Conformance, JavaScript.ExecutionOnly, JavaScript.SliceCompiler and PolyglotCli), and
the Android application, which is not packable but ships both assemblies inside its image.

**To decide:** the release owner approves a notices entry that names the files, the version, the
licence text, and the assemblies, packages and composition images (Android included) containing
derived tables. The owner also decides how
the package and a published image carry the notice: a notices file in the package, a notices file
beside the host, or a constant a host can print. Correcting the stale N4 reason is a separate
edit to the notices file, and this record does not make it.

**Size (proposed, owner decision pending).** The recommendation is to accept about 200 KB in the
profile's packages and in every realm-carrying composition, **with a hard budget recorded by the
generator slice: 300 KB of total table data**, checked by a test. The alternative is a separate
optional data assembly. That would need a new sibling admitted by A11/N1 and a K4 closure row, and a
composition without it would have to keep today's refusals. That is more structure than 200 KB
justifies. **To decide:** the owner accepts or amends the budget figure.

**The version and source are not waiting on the owner.** Retrieval is a human action under roadmap
[section 24](../roadmap.gates.md#24-specification-and-platform-references), so the pin stays
provisional until someone performs it.

## What this refuses to do

- **It invents no hashes.** No UCD file was retrieved for this record. The counts in section 6 come
  from Node's ICU at the same version, and the Unicode 17.0 `General_Category` data in section 2
  comes from the pinned, archived test262.
- **It does not correct the existing platform-data consumers** inventoried in section 1, except
  that U4 moves the matcher's `u`-mode `Canonicalize` onto the table (section 7). The
  unclassified identifier escape, the U+0085 whitespace admission, the group-name classification
  and case conversion are recorded as current behaviour and left to later slices.
- **It does not decide `v`-mode, string properties, case conversion or locale data.**

## Follow-up slices

- **U1 - Retrieve, hash and archive UCD 17.0.0 (human action).**
  **Accept:**
  - the UCD files listed in section 5 and the licence text are in the repository, declared
    `binary`, with a `unicode.pin` recording bytes and SHA-256;
  - the three `table-*unicode-properties*.html` files from `tc39/ecma262` at `0248456c` are archived
    in `docs/specification/`, declared `binary`, and recorded with bytes and SHA-256 in that
    directory's README under the existing Ecma notice (or U1 records that they could not be
    retrieved, and U2 takes the fallback in section 5);
  - two independent retrievals are byte-identical;
  - the `THIRD_PARTY_NOTICES.md` entry has the release owner's co-signature.
- **U2 - Generator, pin rule and tables.** Needs U1.
  **Accept:**
  - generation twice yields byte-identical output;
  - a new N-series rule fails on a changed input byte, a wrong header version, a hand edit to a
    `.g.cs` file, or a disagreement between the archived `table-*unicode-properties*.html` names
    and `PropertyAliases.txt` / `PropertyValueAliases.txt`, each shown failing on a witness (the
    last witness only if U1 archived those files);
  - the generated `.g.cs` files pass the assurance rules with annotations the generator emits (the
    J3 `EXEMPT=` hatch per table member, a computed fingerprint on the containing type), with no
    hand edit and no assurance rule changed, and `CODE-ASSURANCE` / `HUMAN_REVIEW` regenerated
    through J5 in the same change;
  - a second rule forbids `string.Normalize` and `IsNormalized` in profile product source, as N18
    does for regular expressions;
  - measured table data stays under the budget;
  - the build succeeds with the network disabled;
  - the Native AOT publish of the end-user host produces no new warnings;
  - Contract and Architecture suites stay green.
- **U3 (= F08)** implements `normalize` on the U2 tables.
  **Accept:**
  - all of `NormalizationTest.txt` parts 0-3 pass through the end-user host. No unit test can do
    this: rule A11 lets only composition roots reference the profile assembly, and no project
    under `src/tests` references it. The mechanism is a set of JavaScript probes generated by the
    U2 generator from the archived file (same write/check function, so they cannot drift from it)
    into `src/tests/differential/`, each probe holding a bounded slice of the vectors and printing
    one line per failing vector plus a count, run by `eng/run-differential.py` against its
    retained answers and against Node. The slice size is chosen so each probe finishes inside that
    driver's default 30-second per-probe timeout on the CLI host with its default limits; U3
    records the measured time of the slowest probe;
  - the 14 + 4 test262 files pass;
  - work and intermediate storage are charged through `StringCharge` and bounded for
    expansion-heavy input such as U+FDFA repeated;
  - ASCII keeps its fast path.
- **U4 (= F09)** implements `u`-mode property escapes on the U2 tables.
  **Accept:**
  - the 441 + 144 tests pass under the harness's timeout and work limits;
  - misspelled names and aliases are `SyntaxError`s;
  - `/\p{Lu}/iu.test('a')` is `true`;
  - all `u`-mode `Canonicalize` and `Variants` read the `CaseFolding.txt` table (section 7), shown
    by the mixed case: for each of the 28 Unicode 17.0 pairs in section 1, `/\p{Lu}/iu`, `/X/iu`
    and `/[X]/iu` all match the lower-case partner, as Node does, and a differential probe over
    every `\p{Lu}` code point with a single-code-point lower case reports no miss under `iu`;
  - the matcher's own documentation of what it lacks is updated in the same change.
- **Later, separately:**
  - `ID_Start` / `ID_Continue` from the pinned tables in the tokenizer, for literal identifiers
    (astral ones included) **and for escaped ones**, which today are not classified at all, so
    `var \u0021 = 1` and `var \u{1F600} = 1` are admitted (section 1);
  - `WhiteSpace` in the tokenizer as the language's set rather than `char.IsWhiteSpace`, which
    admits U+0085;
  - regular-expression group names classified with `ID_Start` / `ID_Continue` from the same
    Format-assembly tables U2 adds, instead of `char.IsLetter` / `char.IsDigit`;
  - full and special case mapping for `toUpperCase` / `toLowerCase`;
  - the non-`u` `Canonicalize` (`Upper`) from `UnicodeData.txt`'s simple upper-case field;
  - `v`-mode string properties.

## 9. 2026-09-22: U1 performed, the owner's decisions as given, and what U2 built

*Added 2026-09-22 with JSeal slice F07 U2. Local implementation validation, not accepted milestone
evidence; validation record [`docs/evidence/jseal-f07-u2/`](../../../../docs/evidence/jseal-f07-u2/README.md).*

### 9.1 U1: the retrieval

Performed on 2026-09-22 by Claude, with the repository owner's explicit permission given in
conversation on that day. Section 24 of the gates roadmap treats retrieval as a human action; this
records that a person permitted it, not that a person performed it.

- **What was retrieved:** the thirteen UCD 17.0.0 files section 5 lists, from
  `https://www.unicode.org/Public/17.0.0/ucd/`, the licence text from
  `https://www.unicode.org/license.txt`, and the three property-table files from `tc39/ecma262` at
  `0248456c758431e4bb8e5d26333ff1865123c9cd` (`raw.githubusercontent.com`): seventeen files.
- **Twice, byte-identical:** two independent retrievals into two directories, compared with
  `diff -r`; the archived copy is the first. Every file's length and SHA-256 is in
  [`src/tests/unicode/pins/unicode.pin`](../../../tests/unicode/pins/unicode.pin).
- **Where:** the UCD files keep their UCD paths under `src/tests/unicode/pins/ucd-17.0.0/`, with
  the licence as `src/tests/unicode/pins/unicode-LICENSE.txt` beside the pin. That is the shape of
  the two retained pins already here (`src/tests/conformance/pins/`, `src/tests/octane/pins/`):
  the files are read by nothing but the generator, which is test code, so they are test inputs; no
  product project directory contains them, so no glob can pack them; and a build reads the
  committed `.g.cs` files instead. The UCD tree and the licence are declared `binary` in
  `.gitattributes`, which overrides the generic `*.txt text` line. The three ECMAScript tables are
  in [`../specification/`](../specification/README.md), recorded in its README under the Ecma
  notice with lengths and hashes and covered by its existing `*.html binary` line; they do not
  change `JavaScriptLanguageEdition.DocumentDigest`.
- **The licence, checked against the retrieved text as section 8 asked:** Unicode License v3
  permits copying, modification and distribution "provided that either (a) this copyright and
  permission notice appear with all copies of the Data Files or Software, or (b) this copyright and
  permission notice appear in associated Documentation". The understanding in section 8 holds.
- **Not done:** the release owner's co-signature on the notices entry, which U1's last acceptance
  line asks for. It has not been given.

### 9.2 The owner's decisions, as given in conversation on 2026-09-22

Neither is signed, and every signature and co-signature field in this record stays pending.

- **Licence.** A `THIRD_PARTY_NOTICES.md` entry following `Broiler.Unicode`'s own pattern: the
  Unicode data and the tables derived from it are under the Unicode Terms of Use / Unicode License
  v3 (SPDX `Unicode-3.0`), the code stays Apache-2.0; the entry names the files, the version, and
  the assemblies, packages and composition images that carry derived tables, and the licence text
  ships. **Done so:** the entry is
  [Unicode data, and the tables derived from it](../../../../THIRD_PARTY_NOTICES.md#unicode-data-and-the-tables-derived-from-it),
  which also makes the correction section 8 said was owed: the profile **is** packed (since
  `d16a6aa`), so derived tables ship inside `Broiler.VM.Profile.JavaScript` and
  `Broiler.VM.Profile.JavaScript.Format` and inside every realm-carrying composition image, the
  Android one included; the file's stale "the profile packs nothing" reason carries a dated
  correction. **Mechanism for the licence text:** `THIRD_PARTY_NOTICES.md` is already packed at the
  root of every package by `eng/Broiler.Packaging.props`, and it now reproduces the full licence
  text verbatim, which rule N22 asserts against the archived file. **Not decided and not done:**
  how a published composition image carries the notice; no image carries it today.
- **Size.** About 200 KB, with a **hard cap of 300 KB of total table data**, checked by a test.
  Rule N22 reads 300 KB as 300 x 1024 = 307,200 bytes and measures the data twice, from the
  generator and by counting the byte literals of the checked-in files. **Measured: 236,721 bytes**
  (about 231 KB), above the "about 200 KB" estimate of section 6 and under the cap. In the Release
  build the Format assembly grows from 133,120 to 297,472 bytes and the profile assembly from
  602,624 to 684,032 bytes.

### 9.3 What U2 built

**The generator** is C# in the architecture test project, as section 5 decided:
`UnicodePin` (the pin, and the length, hash and header checks), `UnicodeDatabase` (parsing and
self-consistency) and `UnicodeTableGenerator` (emission, and the one gate-or-write function). With
`BROILER_UNICODE_WRITE=1` the N22 gate test writes; otherwise it regenerates in memory and asserts
byte identity. It reads only the files the pin lists, after verifying each, and refuses the whole
archive on any mismatch. It also refuses input that disagrees with itself: General_Category from
`DerivedGeneralCategory.txt` against `UnicodeData.txt`; `NFD_QC=N` and `NFKD_QC=N` from
`DerivedNormalizationProps.txt` against the decompositions it computes; 30 General_Category values
and 8 groups, each group's members as `PropertyValueAliases.txt` states them; each admitted binary
property stated by exactly one pinned file, and 53 of them; simple folding idempotent; no name
selecting two sets. The admitted names come from the three archived ECMAScript tables and are
cross-checked against `PropertyAliases.txt` as section 5 specifies: every name the tables admit
must be a UCD name or alias of the property the table says it is, and the canonical column must be
the UCD long name. The UCD's extra aliases the language does not admit (`WSpace`, for one) are not
a disagreement. `Any`, `ASCII` and `Assigned`, which UTS #18 defines and the UCD does not list,
are the one place with nothing to compare. Output is sorted, invariant, LF, and names no time,
machine or absolute path; generating twice gives the same bytes, which is asserted.

**The tables** are all `static ReadOnlySpan<byte>` over constant data, as section 6 chose, with
three-byte little-endian code points: no dictionary, no string parsing, no lock, no static
constructor.

| File | Assembly | Holds | Bytes |
|---|---|---|---:|
| `JsUnicodeProperties.g.cs` | Format | 443 range sets (30 General_Category values, 8 groups, 53 binary properties, 176 Script and 176 Script_Extensions values) over 23,045 ranges; the names the lone form admits (178), the property names of the `name=value` form (6) and the Script value names and aliases (346) | 147,331 |
| `JsUnicodeCaseFolding.g.cs` | Format | `CaseFolding.txt` status C and S: 1,512 mappings and their reverse (orbit) order | 12,096 |
| `JsUnicodeNormalization.g.cs` | profile | 403 non-zero Canonical_Combining_Class runs; 2,081 full canonical and 3,849 full compatibility decompositions over a pool of 9,222 code points; 961 primary composites (Full_Composition_Exclusion and Hangul excluded); the `NFC_QC` and `NFKC_QC` No and Maybe sets | 77,294 |

Hangul is handled in code, not tables. The counts of section 6 were estimates; its 2,081, 3,849
and about 960 are confirmed.

**The rules.** **N22** holds the tables to the generator and the archive to the pin, with a
witness per failure section 5 names: a changed input byte and a hand edit to a `.g.cs` file
(made in memory against the real archive and the real output, because a two-megabyte witness file
would be a second archive), a header naming 16.0.0 (refused even under a pin recording exactly its
bytes), and a property table admitting an alias `PropertyAliases.txt` lacks. It also asserts the
budget, the specification README's lengths and hashes, and the licence text in the notices file.
**N23** forbids the identifier tokens `Normalize`, `IsNormalized` and `NormalizationForm` in the
profile's product source, as N18 forbids the platform matcher; it reads Roslyn tokens, so comments
and string literals explaining the prohibition are not violations, and a normalizer U3 writes must
not be called `Normalize`. Both are Active; the register moves from 96 rows to 98, and from 94
Active to 96.

**Assurance, and one deviation stated rather than hidden.** Each generated table member carries
the J3 `EXEMPT=` hatch with a reason the generator writes, and each generated partial type carries
`Origin=Derived; Spec=JSD-0031 s5` with a fingerprint and file header computed by passing the text
through `AssuranceGenerator.DesiredSource`, the function J5 itself uses, so a regenerated file is
already what J5 would write. No assurance rule's statement or register row changed. **One
assertion in J5's test did change:** it asserted, as a fact about the checkout, that the product
used the hatch nowhere and that the report said zero. This design is the first use, so the
assertion now states that every declared exemption in the product is a member of one of the three
generated files carrying the generator's reason, and that the report counts and names each (25
today). `CODE-ASSURANCE.md` and `HUMAN_REVIEW.md` were regenerated through J5.

**Where U3 and U4 attach.** All of it is internal; nothing public was added and no API baseline
moved.

- U3 (F08), in `Broiler.VM.Profile.JavaScript`: `JsUnicodeNormalization.CombiningClass(int)`;
  `TryGetDecomposition(int codePoint, bool compatibility, out JsUnicodeCodePoints mapping)`, full,
  recursive and not reordered, Hangul algorithmic, at most `MaxDecompositionLength` (18) code
  points; `TryCompose(int first, int second, out int composite)`, primary composites and Hangul,
  blocking being the caller's; and `QuickCheck(int codePoint, bool compatibility)`, returning
  `JsNormalizationQuickCheck` for NFC or NFKC. A lone surrogate has class 0, no decomposition and
  composes with nothing. U3's `NormalizationTest.txt` probes (see "Follow-up slices") are to be
  generated by adding them to `UnicodeTableGenerator`; U2 generated none.
- U4 (F09), in `Broiler.VM.Profile.JavaScript.Format`: `JsUnicodeProperties.TryResolveLone` and
  `TryResolve(property, value, out JsUnicodeSet)` by exact name; `JsUnicodeSet.Contains`,
  `RangeCount`, `First(i)` and `Last(i)`; `JsUnicodeCaseFolding.SimpleFold(int)` and
  `Orbit(int, Span<int>)` with `MaxOrbit` (4).

**Validation beyond the rules**, by a scratch harness that compiles the product files into a
console program; its source and output are retained in the evidence as text. A reference UAX #15 normalizer
over the U3 lookups passes all 20,034 lines of `NormalizationTest.txt` in all four forms, and every
code point outside its part 1 normalizes to itself. For 1,722 name forms the resolved sets equal
Node v24.17.0's own property-escape sets, except the eight `Katakana_Or_Hiragana` and `Hrkt` forms:
the specification admits them because `PropertyValueAliases.txt` lists them, and V8 refuses them.
Of 30 misspellings, 29 are refused by both; the thirtieth, `WSpace`, is refused here and accepted by
V8, and the specification's table does not admit it. 9,049 case-folding pairs agree with Node's
`/x/iu` and `/[x]/iu`. **The owner asked on 2026-09-22 whether `Broiler.Unicode`, which Broiler.JS
uses with Unicode 17 data, could serve F08 and F09.** Section 4(b)'s reasons still hold against
using it as the source: rules A2 and N1 forbid the reference, its names resolve loosely, it has no
decomposition, combining-class, composition or case-folding data, and its inputs are not pinned. So
it was used as an independent cross-check instead: of 950 set comparisons against its
`Broiler.Unicode.Properties` data, 946 agree, none differs, and the four it has no entry for are the
`Hrkt` and `Katakana_Or_Hiragana` forms.

**Still open:** the release owner's co-signature; how a composition image carries the notice; and
U3 and U4 themselves.

## 10. 2026-09-22: U3 (F08) as built

Local implementation validation only; the record stays proposed and nothing in it is signed.
Evidence: [`docs/evidence/jseal-f08/`](../../../../docs/evidence/jseal-f08/README.md).

**`normalize` answers every string in all four forms.** `JsRealm.NormalizeText` (in
`JsRealm.String.cs`) is UAX #15 over the section 9.3 lookups: full decomposition, canonical
ordering, and for NFC/NFKC canonical composition with blocking. It keeps the method's
specification order (receiver `ToString`, then the form, `"NFC"` by default, a `RangeError` for any
other form before the text is looked at). A lone surrogate is read as a code point of class 0 with
no mapping and passes through. No platform normalizer is called; rule N23 still holds.

**Cost.** A quick check (the canonical-order test plus `NFC_QC`/`NFKC_QC`, or the
decomposition test for NFD/NFKD) scans once and returns the input itself when it is already
in the form; ASCII is answered a unit at a time, so that is the ASCII fast path. Otherwise a first
pass measures the full decomposition without allocating it, refuses one past the realm's
`StringLengthCeiling` (2^24 UTF-16 units, the ceiling `repeat` uses) with `RangeError: Invalid string
length`, and charges `StringCharge` for the storage and the three passes over it before the buffer
exists. Canonical ordering is a stable counting sort per run of non-starters, so a guest-chosen run
of marks is linear work. `'\uFDFA'.repeat(10000)` normalizes in NFKC to 180,000 units;
`repeat(900000)` in NFKD exhausts the CLI host's default fuel before any buffer is allocated, and
`repeat(1000000)` is the ceiling's `RangeError`.

**The vectors travel as probes the generator writes.** `UnicodeNormalizationProbes`, called from
`UnicodeTableGenerator.Generate` and therefore held by N22 byte for byte, writes 18 files
`src/tests/differential/the-unicode-normalization-*.js` from the verified archive: nine vector
probes (parts 0, 2, 3, 4 and 5 whole, part 1 in four equal slices, at most 4,500 vectors each; all
20,034 vectors, 20 checks each) and nine invariant probes that check part 1's closing claim - every
code point not in its `c1` column is unchanged by all four forms - over spans of 0x20000 code
points, surrogate code points included as lone units. Parts 4 and 5 are beyond the 0-3 the U3
acceptance names; they pass too. Each probe prints a count as case 1 and a case per failure, in
ASCII only. All 18 agree with their retained answers and with Node v24.17.0; the slowest
(`invariants-0C0000`) took 1.33 s through the CLI host with its default limits in the full
differential run, against the driver's 30-second timeout.

## 11. 2026-09-22: U4 (F09) performed

*Added 2026-09-22 with JSeal slice F09. Local implementation validation, not accepted milestone
evidence; validation record [`docs/evidence/jseal-f09/`](../../../../docs/evidence/jseal-f09/README.md).
No new decision number was taken, and nothing in this record is signed or co-signed.*

**What U4 changed.** `\p{...}` and `\P{...}` are accepted under `u`, inside and outside classes,
and resolve through `JsUnicodeProperties.TryResolveLone` / `TryResolve` by exact name only; any
other spelling, an unknown or unsupported property, a property of strings, a missing brace or an
empty name is a `SyntaxError` ("Invalid property name"), and the front end reports it early for a
literal because it reads the same parser. Without `u`, `\p` stays the Annex B identity escape. A
property joins a class as a reference to the generated table, not as a copy of its ranges, so
building a class costs one entry per escape whatever the property's width; a class holding
property escapes charges the matcher's meter one extra step per referenced property per character
tested (times five under `i`), and the prefilter scan now hands the meter each skip as it goes.
**The whole `u`-mode Canonicalize moved onto `CaseFolding.txt`, as section 7 decided:**
`JsRegExpCase.Canonicalize(c, unicode: true)` is `JsUnicodeCaseFolding.SimpleFold` and the `u`
closure is `Orbit`, for literals, classes, back-references, `\w`/`\b` under `iu` and property
escapes alike; the `lower(upper(c))` approximation, its U+0130/U+0131/U+017F hand exceptions and
the "astral pairs are reconstructed" branch are gone. The non-`u` path (`Upper`) is unchanged and
still reads the platform's Unicode 16 data, so `/\uA7CE/i` against U+A7CF stays `false`, as
section 7 said it would.

**One change the tests forced, outside the escape itself.** A greedy quantifier over a single
class now lowers to one `Run` instruction (an internal matcher opcode, not a `JsOpcode`) that
consumes the characters and leaves one backtrack point giving them back a character at a time -
by code point under `u` - instead of a loop that pushed a frame and wrote two cells per character.
The generated tests match `^\p{X}+$` over strings of up to the whole code space (1,114,112 code
points for `Any`, surrogates included), which the loop form could not do inside `MaximumFrames` (2^20). Backtracking
order and results are unchanged; every character read is still a metered step.

**Acceptance, as section 9's U4 line states it:** the 441 generated and 144 grammar
`property-escapes` files pass under the harness's 100,000,000-fuel and 5,000 ms limits (1,170 of
1,170 variants; before, 284); misspellings and aliases such as `\p{letter}`,
`\p{Script_Extensions=greek}`, `\p{script_extensions=Greek}` and `\p{Scx=Greek}` are `SyntaxError`s;
`/\p{Lu}/iu.test('a')` is `true`; for each of the 28 Unicode 17.0 pairs of section 1, `/\p{Lu}/iu`,
`/X/iu` and `/[X]/iu` match the lower-case partner, and a probe over all 1,414 `\p{Lu}` code points
with a single-code-point lower case reports no miss under `iu` in any of the three forms (on the
base build the literal and class forms missed exactly those 28); and `JsRegExpMatcher`'s and
`JsRegExpCase`'s own documentation of what they lack is updated. The seven properties of strings
and the `v` flag stay refused by name: the 7 positive `generated/strings` files (14 variants) still
fail, and the 21 negative ones pass because a property of strings is a `SyntaxError` under `u`.

**`Broiler.Unicode` again.** The owner's suggestion that the `Broiler.Unicode` component Broiler.JS
uses could serve F09 was answered in section 9.3 and still holds: the matcher reads only the
pinned tables, which U2 cross-checked against that component's data.

**Still open:** the release owner's co-signature; how a composition image carries the notice; U3;
and the later slices listed under "Follow-up slices" (the non-`u` Canonicalize from
`UnicodeData.txt`, `v`-mode string properties, `ID_Start`/`ID_Continue` for group names).

## 12. 2026-09-22: the "later, separately" items, performed

*Added 2026-09-22 with JSeal slice JSD-0031-later. Local implementation validation, not accepted
milestone evidence; validation record
[`docs/evidence/jseal-unicode-later/`](../../../../docs/evidence/jseal-unicode-later/README.md). No
new decision number, rule, opcode or diagnostic code was taken, and nothing in this record is
signed or co-signed.*

Five of the six items "Follow-up slices" lists under "Later, separately" are done, each on the
section 9 tables through the same generator and rule N22. Full and special case mapping for
`toUpperCase` / `toLowerCase` and `v`-mode string properties are not: `SpecialCasing.txt` is not
archived, and `v` stays refused.

**`ID_Start` / `ID_Continue` in the tokenizer, literal and escaped.** `SliceTokenizer` asks every
identifier question of a code point: a surrogate pair is read whole, so `var` U+1D400 `= 1` is a
declaration, and a lone surrogate is not an identifier character. An escaped code point is now
classified exactly as a literal one - `ID_Start`, `$` or `_` first, `ID_Continue`, `$`, ZWNJ or ZWJ
after, a private name's first character being the one after `#` - so `var \u0021 = 1`,
`var \u0030x = 1`, `var \u{1F600} = 1`, `var x\u{1F600} = 1`, `var \uD835\uDC00 = 1` (each
`\uXXXX` is one code point) and `#\u0031` are early SyntaxErrors (`UnexpectedCharacter`, naming
the code point). Whether an escaped name spells a reserved word stays the parser's question, as it
already was (`RefuseEscapedReservedWord`); the probe confirms `v\u0061r`, `\u0069f`,
`l\u0065t` in strict code and `\u0074his` are refused while `({ \u0069f: 1 }).if` is not.

**`WhiteSpace` as the language's set.** TAB, VT, FF, ZWNBSP and `Space_Separator`; U+0085 and
U+001C to U+001F, which `char.IsWhiteSpace` admitted, are no longer separators, and the line
terminators stay LF, CR, LS and PS (they are tested first, unchanged). Every candidate code point
the probe tries separates two tokens exactly when Node's does.

**Group names** read the same two sets, so U+A7CE, U+16EA0 and U+30FB (`Other_ID_Continue` since
15.1) are group-name characters, as they are identifier characters.

**The non-`u` Canonicalize** is `UnicodeData.txt`'s simple upper case of one code unit, with the
specification's two rules applied by the generator (a mapping that is not one code unit, or that
maps a non-ASCII code unit to ASCII, is dropped), so `/\uA7CE/i` matches U+A7CF and nothing reads
`char.ToUpperInvariant` any more; the lazily built reverse dictionary is gone, replaced by a
generated reverse index. **One stated difference remains:** the specification upper-cases with the
full mapping, under which 27 Greek letters with a ypogegrammeni (U+1F80 to U+1F87, U+1F90 to
U+1F97, U+1FA0 to U+1FA7, U+1FB3, U+1FC3, U+1FF3) become two code points and therefore
canonicalize to themselves; their simple upper case is their title-case partner, so
`/\u1F80/i.test("\u1F88")` is `true` here and `false` in Node. It was `true` before too (the
platform's mapping is also simple). `SpecialCasing.txt` would settle it and is not archived. A
probe over every BMP code unit the ES2026 non-`u` Canonicalize moves (1,169, generated with Node
from `toUpperCase` and the two rules) finds no miss; on the base the literal and class forms
missed exactly the three Unicode 17.0 BMP pairs U+A7CF, U+A7D3 and U+A7D5.

**`lastIndex` inside a surrogate pair under `u`.** RegExpBuiltinExec's matcher input is the list
of code points and "the character that was obtained from element lastIndex of S" is the pair's, so
the match now starts at the pair: `/\udf06/gu` with `lastIndex` 1 over U+1D306 answers `null`,
`/./gu` matches the whole pair. **One reading is chosen, and stated:** the pinned text then writes
the mid-pair `lastIndex` into `index` and the match record's start, which for an empty match would
put the start after the end that GetMatchString asserts it does not exceed; the realm reports the
pair's first unit instead (`index` 0, `lastIndex` 0 for `/(?:)/gu`), which is Node's answer.

**The `v` flag and a literal's flags are early errors.** `JsCompiler.CompileRegExpLiteral` checks
IsValidRegularExpressionLiteral's flag clauses before the matcher sees the pattern: a letter
outside `dgimsuyv`, a repeat, or `u` with `v` is a SyntaxError, and a well-formed `v` is an early
SyntaxError whose message names the flag as unsupported
(``Invalid regular expression flags: v (the flag `v`, unicodeSets, is not supported by
this profile's matcher)``), so `if (false) { /a/v }` no longer compiles. **The manifest refusal
(`ConstructOutsideManifest`) was built first and withdrawn**: it is the more exact code, but it
made the 356 `v` variants of the RegExp subtrees `unsupported`, and the whole-suite floor
`src/tests/conformance/floors/test262-wide.floor` holds `atMost unsupported 0`; the floor was not
lowered. The cost of the SyntaxError is that 58 negative `v`-flag variants (the `unicodeSets` and
`uv-flags` files) now pass because of the refusal, which is not evidence of `v` support.

**Tables and API.** The generator adds three set-id constants to `JsUnicodeProperties.g.cs`
(`IdStartSet`, `IdContinueSet`, `SpaceSeparatorSet`) and two tables to `JsUnicodeCaseFolding.g.cs`
(`UpperData`, 4,784 bytes, and `UpperOrbitIndex`, 2,392 bytes); table data grows from 236,721 to
243,897 bytes under the 307,200 cap. It also refuses input on two new checks: `ID_Start` and
`ID_Continue` from `DerivedCoreProperties.txt` must equal their UAX #31 derivation over
`UnicodeData.txt` and `PropList.txt`, and every simple upper case must belong to a
`Changes_When_Uppercased` code point. The tokenizer is in the compiler assembly and the tables are
internal to the Format assembly, so the Format assembly gains one public type,
`JsUnicodeLexical` (`IsIdentifierStart`, `IsIdentifierPart`, `IsWhiteSpace`, each over an `int`
code point), recorded in the family's API baseline (rule N10, decision JSD-0012); the matcher's
group names read it too. It is a reading of the lexical grammar, not host surface, and JSD-0024 is
unchanged. The product-file count moves from 199 to 200.

**Acceptance evidence.** Pinned Test262 over `language/identifiers`, `language/white-space`,
`language/line-terminators`, `language/literals/regexp`, `built-ins/RegExp` and
`annexB/built-ins/RegExp` (5,067 variants): the pass count moves from 4,274 to 4,489, failures
from 740 to 526, unsupported stays 0, and no variant that passed before fails after. Of the 214
that moved, 156 are the slice's own (all 128 failing `language/identifiers` variants, the 10
`white-space` and 8 `line-terminators` escaped-whitespace files, 4 `literals/regexp` flag early
errors, 6 `named-groups` name files) and 58 are the `v` refusal above. The Unicode 17.0 tokenizer
now passes every `start-unicode-*` and `part-unicode-*` file from 5.2.0 to 17.0.0.

**Still open:** the release owner's co-signature; how a composition image carries the notice; full
and special case mapping (and with it the 27 letters above); `v` mode and properties of strings.
