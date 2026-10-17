<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0075 - The profile on Broiler.Unicode: SJL-7.4's cut-over

**Status:** Decided 2026-10-16. It applies
[JSD-0074](0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md), the area's verdict.
**The code landed in the three changes before this record**, so that the evidence this record cites could
be taken at commits of the code it judges; this record is the change after them.

**Date:** 2026-10-16.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list, rule I4 and rule I5's register, and the release owner, for the notices rows. **All three roles
are held by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-7, work package SJL-7.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-7-unicode.md)). SJL-7 started
before SJL-3 was met, and SJL-7 may not meet its gate before SJL-3 meets SJL-3's; the ledger records both.

---

## Context

JSD-0074 accepted the Unicode area's six sub-areas - properties and lexical classes, case folding, case
mapping, normalization, properties of strings and segmentation - for Broiler.Unicode, under the four owner
conditions Broiler.Unicode adopted, and filed decision SJL-D-6 (c): the owner holds the pin and the
generators, and each engine embeds the tables the owner's generator writes from that engine's own pin. It
changed nothing in this profile. This record is the change itself: the family references the owner's
packages, its tables are the owner's generator's, every Unicode answer the family gave from code of its
own is the owner's, and the members that computed them are deleted. The evidence that judges it is bundle
[`sjl-7-001`](../../../../docs/evidence/sjl-7-001/README.md), under the rule SJL-7.1 committed before any
of this code existed.

**The changes**, each building, each with the tests it touches green:

| Change | What it does |
|---|---|
| `093feb8` | Admits `Broiler.Unicode.Generation` test-only for the architecture tests, with rule I1's dated revision for a test-only entry, and mints rule I4 |
| `0271008` | References `Broiler.Unicode.Algorithms` and `.Segmentation`; moves the property lookups, the lexical classes, case folding, the properties of strings and segmentation; carries the closures and the package baseline taken at its tree |
| `b0ac6e2` | Moves normalization and case mapping, and Soft_Dotted out of the Intl data |
| `8aa88db` | Re-points rules N18 and N23 |

## Decision

### 1. References

**`Broiler.Unicode.Algorithms`, `Broiler.Unicode.Segmentation` and `Broiler.Unicode.Generation`
`0.1.0-preview.4`**, the owner's stream's next preview, packed locally from Broiler.Unicode
`6e14f93ad753094c99f0013d22269b47550ae552` and restored from the committed feed `eng/packages/` with lock
files. They are not on nuget.org at this record; each `.pin` beside its package on the feed says so, and a
release of this family waits for their publication, which `eng/verify-feed.ps1` enforces.

| Package | Use | Projects rule I1 admits it for |
|---|---|---|
| `Broiler.Unicode.Algorithms` | product | `Broiler.VM.Profile.JavaScript`, `.Format`, `.Compiler`, `.Intl` |
| `Broiler.Unicode.Segmentation` | product | `Broiler.VM.Profile.JavaScript.Intl` alone |
| `Broiler.Unicode.Generation` | test-only | `Broiler.VM.Architecture.Tests` alone |

**`Broiler.Unicode.Tables` is not admitted**: under SJL-D-6 (c) neither engine references a package that
carries tables. Each allow-list entry landed in the change that adds its first reference, as the
allow-list's policy has it: Generation's in `093feb8`, the other two in `0271008`.

**Format references a package.** JSD-0074's X5 named the edge: Format holds the lookups the compiler and
the runtime both read, so the owner's code package enters it. Rule N3 limits Format's project references
and still holds. The project-graph manifest records each new package edge (rule A7), and the composition
register's six JavaScript rows declare the code package, and the segmentation package in the two rows
whose closure carries the Intl assembly, `.SliceCompiler` and `.Conformance` (rule K4).

**The closures and the package baseline moved with the references.** Rule K4 holds a row's declared
shared assemblies to the closure its evidence cell names, and rule N37 holds the package baseline to the
checkout's references, so both were taken again in `0271008`: the closures of every JavaScript root under
JIT, trimming and Native AOT in bundle sjl-7-001's
[`cutover/`](../../../../docs/evidence/sjl-7-001/cutover/README.md), and the pack, the pristine consumer,
the rollback and the negative control in bundle [JS-10-007](../evidence/js-10-007/README.md), which
`packages.md` now names ([JSC-308](../roadmap.corrections.md#jsc-308)). A change cannot carry evidence
taken at itself, so both were collected at `a9be9f1`, `0271008`'s tree before the evidence and its records
were added. Every path the two differ in is a documentation path, and the product-sources digest
`cutover/identity.txt` records is the same at both. The Android row keeps its retained closure, which
predates the shared assemblies, under EX-126, as it did at SJL-3.4.

### 2. Seams

| Seam | What it is |
|---|---|
| `JsUnicodeData` (Format, public) | The Unicode version, `"17.0.0"`, and `Table(int)`: the bytes of the table the owner's `UnicodeTable` numbers, or an empty span. Public because the compiler and the runtime read Format's tables and the family opens its internals to nobody (rule A10); it names no type of the owner's, so the family's public surface carries none (rule I6) |
| `JsFormatUnicode` (Format) | The owner's `UnicodeProperties`, `EcmaScriptLexical`, `UnicodeCaseFolding` and `UnicodeStringProperties` over Format's tables, made once, immutable and shared by every engine |
| `JsCompilerUnicode` (Compiler) | `UnicodeProperties` and `EcmaScriptLexical` over Format's tables through `JsUnicodeData`, and `IsLetter`, General_Category Lu, Ll, Lt, Lm or Lo, with an ASCII fast path. Read-only statics only (rule N12) |
| `JsUnicode` (profile) | One provider answering the normalization and case mapping tables from the profile's own `JsUnicodeTables.g.cs` and every other table from `JsUnicodeData`; `Lexical`, `Normalization` and `Casing` over it |
| `IJsTextSegmentation` and `JsSegmentGranularity` (Format, public) | UAX #29's default boundaries and ECMA-402's word-likeness, with the caller's meter as an `Action<ulong>`. The Intl data's `JsCldrData` implements it over the owner's `UnicodeSegmenter`, and `JsIntlTables` reads the composition's Intl data as an `IJsTextSegmentation`, so the profile references neither the Intl assembly nor the segmentation package, and a composition that declines `Intl` carries none of it (JSD-0074's X5; rule I3) |
| `JsEngine.UnicodeCharge` and `SegmentationCharge` | The engine's meter as the owner's `UnicodeWorkCharge` and as an `Action<ulong>`, each made once per engine, so no call allocates a delegate |

**An embedder that supplies Intl data of its own** implements `IJsTextSegmentation` beside `IJsIntlData`
for `Intl.Segmenter`. Without it a Segments object's first read throws a `TypeError` naming what is
missing; every other Intl surface is unchanged. The family's own `JsCldrData` implements both.

### 3. Tables, the generator and the budgets

**The owner's generator writes every table**, run test-only in the architecture tests over the archive
`src/tests/unicode/pins/unicode.pin` names, and `UnicodeTableEmbedding` writes its 37 tables into three
files, split by assembly as they were before: Format's `JsUnicodeTables.g.cs` (the property, folding and
string-property tables, 1 to 21), the profile's (normalization and case mapping, 22 to 36, Soft_Dotted's
ranges among them) and the Intl assembly's (the break table, 37). Each is one internal class whose `Get`
answers a `UnicodeTable` with a span over constant data, annotated per unit with the `EXEMPT=` line rule J3
accepts and passed through the assurance generator. The owner's `UnicodeTableSource.Write` is not used,
because it writes no assurance annotation.

**The tables are the ones this family embedded.** Checked at this change by decoding both: each of the
thirty tables `UnicodeTableGenerator` wrote, and the break table `CldrTableGenerator` wrote, is
byte-identical to the owner's table of the same layout; and the Soft_Dotted ranges, which
`CldrTableGenerator` wrote as 371 bytes of text, are the same 34 ranges in the owner's binary range list
of 204 bytes. What were C# constants beside the tables - the property layout, the folding orbits, the
string-property limits, the longest decomposition, the casing layout - are the owner's five layout tables.
`UnicodeTableGenerator` writes no table: it is rule N22's gate and the writer of the normalization probes,
which are this repository's test assets. `CldrTableGenerator` no longer writes the break properties, their
value names or Soft_Dotted.

| Rule | Before | After | Cap |
|---|---|---|---|
| N22, Format's and the profile's tables | 291,630 bytes, in five files | 291,891 bytes, in two: Format 182,686, the profile 109,205 | 300 x 1024 = 307,200 |
| N28, the Intl assembly's tables with the time zones and calendars | 841,145 bytes | 840,365 bytes, the break table's file included | 832 x 1024 = 851,968 |

**Rule I4**, minted in `093feb8` under ADR 0014, holds this repository's pin equal to the owner's, which the
generator package carries, by the owner's own comparison, and every admitted assembly's `UnicodeVersion`
or `PinnedVersion` constant to the pin's version, 17.0.0, read from the package images through metadata.
Its witnesses are a pin differing from the owner's by one hash and a library declaring another version.

### 4. Adapters

Each calls the owner and keeps what the language and this family decide - argument conversion and its
order, the errors and their messages, the charge where the owner does not make it, the storage - and
computes no Unicode answer itself:

| Adapter | What it keeps | What it calls |
|---|---|---|
| The matcher's property escapes, group names and character classes (`JsRegExpMatcher`) | Its parse, its errors, its sets' ranges and every charge it made for them | `UnicodeProperties.TryResolve` and `CodePointSet`; `EcmaScriptLexical.IsIdentifierStart` and `IsIdentifierPart` |
| The matcher's `Canonicalize` and its variants (`JsRegExpCase`) | Its two charge factors, `MaxUnicodeVariants` and `MaxVariants`, now the owner's `MaxOrbit` and `MaxUpperOrbit`, 4 and 4, which rule N22 checks over the generated tables | `UnicodeCaseFolding.SimpleFold`, `SimpleUpper`, `Orbit` and `UpperOrbit` |
| The `v` flag's properties of strings (`CollectStringProperty`) | The insertion order of its set, which the matcher's steps follow: the union's members in id order, then each property's ranges, then its sequences | `UnicodeStringProperties.TryResolve`, `StringProperty`, its ranges and `StringPropertySequences.TryRead` |
| The tokenizer (`SliceTokenizer`) and the parser's routing checks | The tokens, the routing and the diagnostics | `EcmaScriptLexical`; `JsCompilerUnicode.IsLetter` for the two checks that asked `char.IsLetter`; an ASCII letter test where only ASCII reaches it; the language's white space and line terminators; identifier parts for a literal's flags |
| `FunctionIsNativeName`, `RegExpEscapeCodePoint` | Their callers' answers | `EcmaScriptLexical.IsIdentifierStart`, `IsIdentifierPart`, `IsWhiteSpace` and `IsLineTerminator` |
| `normalize` and the collator (`NormalizeText`) | The form's conversion and its `RangeError` before any text is read; the `RangeError` `Invalid string length` | `UnicodeNormalization.Transform` with the form, the ceiling 2^24 and `UnicodeCharge`; the collation's `CombiningClass` |
| `toUpperCase`, `toLowerCase` and their locale forms (`StringChangeCase`, `TransformCase`, `ChangeCase`) | The locale's resolution, ECMA-402's or, in a realm without Intl, none (row C10); `tr` and `az` as `CaseLanguage.Turkic`, `lt` as `Lithuanian`; the language pass's charge; the `RangeError` | `UnicodeCaseMapping.ToUpperCase` and `ToLowerCase` with the language, the ceiling and `UnicodeCharge` |
| `Intl.Segmenter`'s Segments objects and iterators (`JsRealm.Segmenter`) | The objects, the iterators, the binary search over the boundaries, the one unit each segment's data object costs | `IJsTextSegmentation.Boundaries` and `IsWordLike`, which the Intl data implements over `UnicodeSegmenter.Boundaries` and `GetWordStatus` |

### 5. Charging and limits

**Normalization and case mapping keep this family's units** (JSD-0074, decision 4). The owner charges
exactly what correspondence rows N7 and C13 state, through the engine's meter: for normalization the
text's length, and then, unless the text is in the form, its length plus three units per code point of its
full decomposition; for case mapping's default pass the text's length, each mapping's length in code
points and the units each Final_Sigma test reads. **The language pass's extra input length is charged by
the adapter**, before the call and only for a language, the split JSD-0065's decision 2 made. **The lookups
and folding charge nothing**, as before; the matcher charges for what it does with their answers exactly as
it did.

**Segmentation's units change**, by JSD-0074's decision 4: the owner's segmenter charges one unit per code
point each rule classifies or reads, the look-behinds and the word-likeness scan included, through
`SegmentationCharge`. Bundle sjl-7-001's rule admits a moved exhaustion verdict on the `intl402/Segmenter`
paths alone, as class C2, and no probe of the command-line root reaches segmentation.

**The checks at this change.** Every fuel-parity twin and Unicode probe of the bundle's part B - the ten
wide twins and `unicode/casing`, `unicode/normalize` and `unicode/properties` - completes at its base figure
with the base's answer, and is refused on fuel at one less, in both forms, at `0271008`'s and `b0ac6e2`'s
trees; part B's after run is SJL-7.6's. The figures are in section 7.

### 6. Corrections

This family's three, C12, N6 and G5, under decision SJL-D-16 (a), and one consequence of the first two
that JSD-0074 did not state:

| Correction | What this family did | What it does |
|---|---|---|
| C12 | Casing checked its ceiling only after a mapping was appended, and not at all in the language pass | The owner checks the caller's maximum on the whole result, in both passes |
| N6 | Normalization checked its ceiling on the decomposition, so an NFC or NFKC result that would fit could be refused | The owner checks it on the result |
| G5 | Segmentation charged one unit per sixteen code points, and left the regional-indicator back-walk, the Sp and Close back-walks of SB8 to SB11, the word-likeness scan and the segments' copies uncharged | One unit per code point classified or read (section 5) |
| C12 and N6, a text longer than the ceiling that the operation leaves unchanged | `normalize` returned a text in the form as it came, and `toLowerCase` a text with nothing to lower, whatever their length | Refused with the `RangeError`, since the owner's maximum is on the whole result. A string that long is reachable only by concatenation, which has no ceiling of its own |

Checked once at this change through the command-line root, with the text `'a'.repeat(2 ** 24) + 'a'`
(2^24 + 1 code units): at the base, `normalize()` and `toLowerCase()` returned it and `toUpperCase()`
threw; after `b0ac6e2` all three throw. And `'a'.repeat(2 ** 24 - 2) + '\u1E9B\u0323'`, whose NFC is its own
length and whose decomposition is one unit longer: at the base `normalize('NFC')` threw, and after `b0ac6e2`
it answers the 2^24 units.

### 7. Evidence at this change

**The shared vectors**, at this family's pin of the corpus (Broiler.JS `c976cbe`, archived under
`src/tests/vectors/pins/`): 88,465 vectors, 88,465 match, 0 classified divergences and 0 unclassified
differences, at `0271008`'s product sources and at `b0ac6e2`'s.

**Part B's twins and probes**, at `b0ac6e2`'s product sources, each at the base figure bundle sjl-7-001
retains and at one less, in both forms, against the base's answer; `0271008`'s gave the same:

| Twin or probe | Figure | Answer | Bytecode | Native |
|---|---|---|---|---|
| `unicode/casing.small.js` | 328,460 | 179280 | completes; refused at one less | completes; refused at one less |
| `unicode/normalize.small.js` | 607,821 | 420519 | completes; refused at one less | completes; refused at one less |
| `unicode/properties.small.js` | 14,348,623 | 35050 | completes; refused at one less | completes; refused at one less |
| `wide/array-loop.small.js` | 73,082 | 999000 | completes; refused at one less | completes; refused at one less |
| `wide/call-loop.small.js` | 28,026 | 1000 | completes; refused at one less | completes; refused at one less |
| `wide/closure-loop.small.js` | 31,044 | 1000 | completes; refused at one less | completes; refused at one less |
| `wide/fib.small.js` | 7,682 | 144 | completes; refused at one less | completes; refused at one less |
| `wide/generator-loop.small.js` | 40,075 | 499500 | completes; refused at one less | completes; refused at one less |
| `wide/numeric-loop.small.js` | 23,022 | 498497 | completes; refused at one less | completes; refused at one less |
| `wide/property-loop.small.js` | 29,029 | 183125 | completes; refused at one less | completes; refused at one less |
| `wide/string-loop.small.js` | 26,083 | 25 | completes; refused at one less | completes; refused at one less |
| `wide/throw-loop.small.js` | 34,026 | 1000 | completes; refused at one less | completes; refused at one less |
| `wide/try-loop.small.js` | 34,022 | 499500 | completes; refused at one less | completes; refused at one less |

**The rules.** The architecture tests pass at each change, and at `0271008` with the five Native AOT
image records placed in the tree, so that rule I3 read the real images: the execution-only root's and
the command-line roots' carry no segmentation package. Bundle sjl-7-001's `cutover/closures/` shows the
same in every mode.

### 8. Deleted

The 29 members and types JSD-0074's deletion lists name are deleted and registered in rule I5's
`retired-members.json`, each with what replaced it:

- **Format**: `JsUnicodeProperties` with `JsUnicodeSet`, `JsUnicodeCaseFolding`, `JsUnicodeStringProperties`
  and `JsUnicodeLexical`, and their generated files;
- **the profile's normalization and case mapping**: `JsUnicodeNormalization`, `JsNormalizationQuickCheck`,
  `JsUnicodeCodePoints` and `JsUnicodeCasing`, and their generated files; in `JsRealm`,
  `IsNormalizedAlready`, `OrderCanonically`, `ComposeCanonically`, `WriteCodePoints`, `LanguageCasing`,
  `AfterCapitalI`, `AfterSoftDotted`, `BeforeDot`, `MoreAbove` and `AppendCodePoint`;
- **Soft_Dotted in the Intl data**: `JsIntlTables.IsSoftDotted` and `ReadRanges`, and the table's entry;
- **segmentation**: `JsSegmenter.cs` whole - `JsSegmentation`, `JsBreakData`, `JsBreakProperties`,
  `JsGraphemeBreak`, `JsWordBreak`, `JsSentenceBreak` and `JsIndicConjunctBreak` - and
  `JsIntlTables.Breaks`, with the break table's two entries.

**What stays, and holds no Unicode algorithm:** the adapters of section 4, and the platform-free
`JsRegExpMatcher.cs:2739` test of an ASCII marker, which is the matcher's (SJL-11), for SJL-7.6's
simplification check.

### 9. Rules

- **I1**, revised in `093feb8` with a dated note: a test project may reference an admitted package only by a
  test-only entry naming it, and a test-only entry may name test projects and never a shipping one. Its
  witnesses are a product entry naming a test project and a test-only entry naming a shipping one; a
  further witness, the segmentation package in Format, is refused because the entry names the Intl
  assembly alone.
- **I3** treats `Broiler.Unicode.Segmentation` as optional data, carried only where the Intl assembly is;
  its witness is an execution-only closure carrying it.
- **I4**, new (section 3).
- **I5** registers the 29 (section 8); its statement gains a dated revision.
- **N18 and N23** read the Intl data assembly with the other three, which JSD-0074 found N23 did not, and
  N23's guard requires `JsUnicode.cs` beside `JsRealm.String.cs`. Each remark and register row says, dated,
  that the code that moved is held by rule I2, which reads every admitted assembly's image for the
  platform's normalization, case mapping, culture and regular expressions, and by the owner's own `SL`
  rules and convention 14.
- **N22 and N28**, revised with dated notes: N22 holds Format's and the profile's files to the owner's
  generator, and checks the matcher's two charge factors over the generated tables; N28 holds the Intl
  assembly's break table file too, inside its budget.
- **The rule register** counts 125 active rules of 126.

### 10. Public API

The family's preview API changes, and `docs/api/public-api.txt` records it:

- **removed**: Format's `JsUnicodeLexical` (JSD-0074's decision 7) and `JsIntlTable.SoftDotted` (7),
  `SegmentBreakValues` (23) and `SegmentBreaks` (24), whose numbers are not reused;
- **added**: Format's `JsUnicodeData`, `IJsTextSegmentation` and `JsSegmentGranularity`, and the Intl
  data's `JsCldrData.Boundaries` and `IsWordLike`, which implement the interface.

An embedder's `IJsIntlData` that answered the three removed tables loses nothing: the profile no longer
asks for them. One that wants `Intl.Segmenter` implements `IJsTextSegmentation` (section 2).

### 11. Records

- **Notices.** `THIRD_PARTY_NOTICES.md` has a build-and-test row for `Broiler.Unicode.Generation` and
  `Broiler.Unicode.Algorithms` as its dependency, and an ingesting row naming the four assemblies that
  reference the code and segmentation packages, not confirmed. Its Unicode Character Database row names
  the Intl assembly, whose break table the owner's generator writes from the same pin. Its Unicode data
  entry says the owner's generator writes the three files, and that the Soft_Dotted ranges now ship in
  every image that carries the realm; its CLDR entry no longer lists the break properties or Soft_Dotted,
  with a dated note. Release gate 12 names the new row's blocker and the renamed one
  ([JSC-309](../roadmap.corrections.md#jsc-309)), and its refusal of an Intl data assembly with no notices
  row now asks for a row naming it whose ingested cell names CLDR, since the two Unicode rows name the
  assembly too.
- **Packages.** `packages.md`'s baseline is bundle JS-10-007's, with the Unicode area's dependencies
  ([JSC-308](../roadmap.corrections.md#jsc-308)); `support.md` names the bundle.
- **Compositions.** The register's six JavaScript rows (section 1).
- **Assurance.** The record is regenerated. The code that left this family's assurance coverage is covered
  by the owner's own assurance component in the Broiler.Unicode repository, as EX-125 and EX-127 say of
  the earlier areas; bundle `sjl-7-001` names it with SJL-7.6's evidence.

## Departures from the plan

Each is this record's, and none edits JSD-0074:

- **The owner offers no enumeration of the fold entries** (the plan's P2). The matcher enumerated them in
  two places, the sources of a class's folds and the fold of a range. With no owner change, it builds the
  pairs once per process, lazily, by asking `SimpleFold` of every code point from U+0000 to U+10FFFF and
  keeping those it moves, and both places read the pairs. It is compile-time work, uncharged as the
  enumeration was, and part B's figures do not move.
- **Rule I1 admits a test-only package for a test project** (P3), by the dated revision of section 9: the
  generator must reach the tests, and I1 refused any project outside the family.
- **Rule I4 checks the pin and the Unicode versions** (P6). The programme's `areas/numbers.md` says I4
  checks the shared algorithms package's `StrWhiteSpace` table against the owner's Zs; the architecture
  tests cannot run that package's code. I4 holds every admitted assembly's version constant to the pin
  instead, and the set stays covered by the W2 vectors and Broiler.JS's own regeneration test.
- **The whole-result maximum refuses an unchanged text past the ceiling** (P5), listed with C12 and N6 in
  section 6.
- **The changes are four before this record, where the plan had seven.** Its first two are `093feb8`:
  admitting the three packages before any project referenced them would have put allow-list entries in the
  tree that no project used, so each entry lands with its first reference. Its wiring change, its lookups
  change and its segmentation change are `0271008`: trimming drops a referenced package no code calls, so a
  change of references alone would have retained closures without the packages its rows declare, and a
  baseline declaring dependencies no code needed. Its rules change is `8aa88db`, for N18 and N23; the
  statement revisions of I1, I3, I5, N22 and N28 land in the changes that made them true.
- **Two of the matcher's comments are corrected**, dated: they said the canonicalisation without `u`
  maps the 27 Greek letters with a ypogegrammeni to their title-case partners because `SpecialCasing.txt`
  is not archived. It has been archived since 2026-10-03, and the generated table, the family's and now
  the owner's, leaves them as the language says.
- **`collect-area.py` gains a `cutover-closures` step**, whose output its `area-manifest.json` does not
  list; the bundle's manifest is SJL-7.6's to regenerate.
- **The compiler's letter test has an ASCII fast path**, which answers what the owner's lookup answers for
  ASCII, and the parser's white-space check (`JsParser.cs:7644`) reads the language's white space and line
  terminators, where `char.IsWhiteSpace` also skipped U+0085 and U+001C to U+001F and did not skip U+FEFF;
  each routes a construct the tokenizer then reads or refuses (row W6).

## Correction entries

[JSC-308](../roadmap.corrections.md#jsc-308) (`packages.md` and `support.md`, the package baseline and the
Unicode area's dependencies) and [JSC-309](../roadmap.corrections.md#jsc-309) (`release-gate.md`, gate 12's
blockers).

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-7 is met.** Its evidence is bundle `sjl-7-001`, read under its rules at SJL-7.6, and the
  ledger records the result.
- **Broiler.JS's cut-over**, SJL-7.5, which its own changelog records.
- **Publishing the owner's packages** to nuget.org, which is the owner's to choose; this family cannot be
  released before it.
- **JSD-0031 §4(b)**, whose five reasons JSD-0074 answers with SJL-7.6's evidence.
- **A Unicode version.** The pin stays 17.0.0.

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 4 computes a property, a fold, a mapping, a decomposition or a boundary itself;
- a vector that matched before this change does not after it, a fuel-parity twin or Unicode probe completes
  at a different allowance or with a different answer, an exhaustion verdict moves outside the
  `intl402/Segmenter` paths, or a test262 verdict moves other than by C12, N6 or G5, without a dated
  decision admitting it;
- a composition that declines `Intl` carries `Broiler.Unicode.Segmentation`.
