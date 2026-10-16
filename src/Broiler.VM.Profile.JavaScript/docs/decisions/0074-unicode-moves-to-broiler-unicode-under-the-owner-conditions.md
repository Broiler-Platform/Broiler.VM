<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0074 - Unicode character data and algorithms move to Broiler.Unicode under its owner conditions: SJL-7's area verdict, sub-area by sub-area, and decision SJL-D-6

**Status:** Decided 2026-10-16.
- **All six sub-areas are accepted** and placed with Broiler.Unicode: properties, lexical classes, case
  folding, case mapping, normalization, properties of strings and segmentation. Properties and lexical
  classes are one sub-area, as the milestone counts them.
- **Broiler.Unicode adopts the four owner conditions of SJL-7.1** by a dated record of its own.
- **Decision SJL-D-6 is recorded** for the first data area.
- The programme's extraction gate (its roadmap section 5, conditions X1 to X7) holds for each sub-area on its
  own. Normalization and two parts of segmentation are adoptions for Broiler.JS, whose implementation there is
  the host's.
- **JSD-0031 §4(b)'s refusal of `Broiler.Unicode.Properties` stands** until SJL-7.6's evidence answers each of
  its five reasons. This record states how each is answered.
- Nothing in this profile changes with this record. The cut-over is SJL-7.4's own change, and this record is
  what it applies.

**Date:** 2026-10-16.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.Unicode maintainer, as the area's owner and SJL-D-6's data home;
- the Broiler.JS maintainer, as the other engine's owner and SJL-D-6's other holder;
- the core architecture owner, for rule I1's allow-list and the composition register.

**All four roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the programme's SJL-7, work package SJL-7.1.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`,
  section "Area verdicts".
- Broiler.Unicode's `docs/roadmap.md` records the owner conditions it adopts.
- **SJL-7 started before SJL-3 was met**, by the owner's choice of 2026-10-16. The programme ledger records
  that SJL-7 may not meet its gate before SJL-3 meets SJL-3's. This record moves no gate.

---

## Context

The area's design is the programme's
[`areas/unicode.md`](../../../../docs/javascript-shared-libraries/areas/unicode.md). It has no vectors yet:
SJL-7.3 adds them after this record.

**The correspondence is from code.** It was re-derived by reading:
- this repository at `16a35b2`, whose product sources are `24632b1`'s;
- Broiler.JS at `bdb1a3ca`, whose engine sources are `4d70821e`'s;
- Broiler.Unicode at `0faa1f9`, whose code is that of its `0.1.0-preview.3` packages;
- Broiler.Regex at `0799342`, whose code is that of its `0.1.0-preview.4` package.

Every member is named by file and line in
[`correspondence.md`](../../../../docs/evidence/sjl-7-001/correspondence.md), which this record's change
retains in bundle `sjl-7-001` as the gate's input, with each difference numbered (P, I, W, F, C, N, S, G).
**Every behavioural cell the rows below rest on was run** in both engines. In this family it was run through
the command-line root, whose realm has no Intl, and through the conformance root, whose realm has. In
Broiler.JS it was run through its shell, on the host's ICU 74.2, which carries Unicode 15.1, as the hosted
runners' does. SJL-7.3's vectors confirm or refute each one, and **a difference they find that this record
does not classify falsifies it**.

**What the re-derivation found:**
- **The owner's data already agrees with this family's pin.** Broiler.Unicode's four property generators and
  Broiler.Regex's case-folding generator, run against this family's pinned UCD 17.0.0 files, reproduce their
  committed tables byte for byte, and Broiler.Unicode's committed emoji inputs carry the hashes this family's
  pin records. **What the owner lacks is everything around the bytes**:
  - exact names, apart from one missing script value (P2);
  - immutable results;
  - pins and an in-repository generator;
  - normalization, folding of its own and segmentation;
  - meters and limits;
  - an assurance gate;
  - the AOT declarations.
- **Every behavioural difference but three corrects Broiler.JS.** The three are this family's own:
  - its casing ceiling is checked only after a mapping is appended (C12);
  - its normalization ceiling is checked on the decomposition rather than the result (N6);
  - its segmenter leaves several scans uncharged (G5).
- **Two rows are SJL-11's**, being Broiler.JS's .NET translator's (F2, W5).
- **Several of the design's readings no longer hold** (section "The design's readings"). The largest is that
  the segmentation tables and Soft_Dotted are written by this family's CLDR generator and held by rule N28,
  not by the Unicode generator and N22.

## The owner conditions (SJL-7.1)

SJL-D-1 (b) places the area with Broiler.Unicode **if it adopts four things**. Against each, the
correspondence's section 8 states where it stands today, and **Broiler.Unicode adopts all four** by the dated
record of this date in its `docs/roadmap.md`. It meets them in SJL-7.2, and exit clause 1 judges them at
SJL-7.6:

1. **The programme's conventions** are those of the programme's `architecture.md` section 4, as Broiler.JS's
   `Broiler.ECMAScript/docs/conventions.md` states them. Broiler.Unicode files its own copy, with the same
   numbering, and enforces what a test can decide with rules of the `SL` shape in an architecture-test project
   of its own, each with a witness. **Its contract types are its own.** The layering
   (`architecture.md` section 1) lets the shared libraries depend on Broiler.Unicode and never the reverse, so
   its meter is a delegate of the same shape as `WorkCharge`, defined in Broiler.Unicode, and each engine's
   adapter passes the same meter to both. The packages the engines consume target `net10.0`, enable nullable,
   disable implicit usings, and declare `IsAotCompatible` and `IsTrimmable`, with a Native AOT probe in its CI.
2. **A roadmap: met.** It was filed at SJL-0 and merged into the default branch. Its items U1 to U10 are
   SJL-7.2's work.
3. **An assurance gate:**
   - every relevant unit of the packages the engines consume carries the Broiler Code Assurance annotation;
   - the artefacts are generated with Broiler.Code's command-line tool;
   - its CI gates on them;
   - `HUMAN_REVIEW.md` names the revision it reviews, so this family's release gate can read it through rule
     I7 (SJL-D-5 (a)).
4. **Pinned and hashed generator inputs:**
   - a pin file hashes the UCD 17.0.0 inputs, the auxiliary break data and its tests, the emoji sequence files
     and the specification's three property tables;
   - it starts as a copy of this family's `unicode.pin`, the same files by the same hashes;
   - the generators live in the repository: ports of this family's `UnicodeDatabase`, `UnicodeTableGenerator`
     and the Unicode parts of `CldrTableGenerator`;
   - a test regenerates every table byte for byte (the programme's rule SL7).

**If Broiler.Unicode does not meet a condition by the end of SJL-7.2**, the sub-areas that condition reaches
move to the Broiler.JS repository, under `Broiler.ECMAScript`, by a dated record naming the condition. This
record is revised beside it. This is SJL-D-1 (b)'s rule, and nothing here weakens it.

## The gate, sub-area by sub-area

### Properties and lexical classes

**X1 - met.**
- **This family** resolves `\p{…}` by exact name over its generated sets (`JsUnicodeProperties`), and
  classifies identifiers and white space with `JsUnicodeLexical` (correspondence sections 1 and 2).
- **Broiler.JS** resolves through Broiler.Regex and Broiler.Unicode's loose lookup, with its .NET translator
  behind them. It classifies identifiers in `CharExtensions` over the runtime's categories and hand lists, and
  white space in several lists.

**X2 - met, with seven corrections of Broiler.JS.**
- **P1, P2, P3 and P4**, the property lookup:
  - it matches names loosely;
  - it refuses `Katakana_Or_Hiragana`, whose script has no code points;
  - an unknown name can fall through to .NET's `\p`;
  - its set-operation path does not take General_Category.
- **I1**: identifiers do not subtract Pattern_Syntax, so U+2E2F is an identifier there.
- **I3**: group names copy the identifier tests without the Unicode 17 tables or the subtraction.
- **W1**: U+0085 is white space to the scanner, to `RegExp.escape`, and to `Intl.Segmenter`'s word
  categories.
- **I2** is a data-policy difference (class C5 under SJL-D-6): Broiler.JS's classes agree at Unicode 17.0 only
  through patches over the runtime's data.
- **W2 to W5 agree.** The white space of `trim` and `Number` is already SJL-3's shared set.
- **I4, W4 and W6 are platform classification without an observable difference**, and they go.

**X6 - extended.**
- **Broiler.VM:**
  - the lookup code of `JsUnicodeProperties`;
  - `JsUnicodeLexical`;
  - `FunctionIsNativeName`'s `char.IsLetter` and `GetUnicodeCategory`;
  - `RegExpEscapeCodePoint`'s `CharUnicodeInfo`;
  - **the parser's five routing checks over `char.IsLetter`, `char.IsWhiteSpace` and `char.IsLetterOrDigit`**,
    which the design did not name.
  - `JsNumberFormat.IsWhiteSpace`, which the milestone names, no longer exists (SJL-3.4).
- **Broiler.JS:**
  - `CharExtensions.IsIdentifierStart` and `IsIdentifierPart` with their Unicode 17 tables and hand lists;
  - the scanner's white-space test;
  - `RegExp.escape`'s;
  - `StringSpan.Trim`'s, which has no caller;
  - `JSRegExp`'s group-name classes `IsRegExpIdentifierStart` and `IsRegExpIdentifierPart`;
  - the translator's loose property lookup with `NormalizeKey`, which becomes a call to the exact lookup (the
    translator itself is SJL-11's);
  - **`IsValidNativeFunctionToStringName`'s `char.IsLetter`**, which the design did not name.
  - `JSString.trimCharacters` and `NumberParser`'s list, which the milestone names, are gone since SJL-3.5.

### Case folding

**X1 - met.** This family implements simple folding, its orbits and the canonicalisation without `u` in
`JsUnicodeCaseFolding`. Broiler.JS's folding under `u` and `v` is Broiler.Regex's `CaseFolding`, which it
consumes.

**X2 - met, with nothing to correct in SJL-7.**
- **F1**: the two engines agree on every input run, from the same derivation. Broiler.Regex's table,
  regenerated from this family's pin, is byte-identical to its committed one.
- **F2**: `/[a-z]/i` matching U+212A is Broiler.JS's .NET translator. It is corrected when SJL-11 retires
  that translator.
- **F3**: the orbit is a contract condition (an immutable result), not a behaviour.

**X6.**
- **Broiler.VM:** `JsUnicodeCaseFolding`'s lookup code; the matcher calls the owner.
- **The owner:** Broiler.Regex's table becomes Broiler.Unicode's, one generated table. Broiler.Regex reads it
  through the owner's provider at SJL-11 (its roadmap's R11).
- **Broiler.JS:** nothing of its own outside the translator, which is SJL-11's.

### Case mapping

**X1 - met.** This family implements the default and language-sensitive mappings in `JsUnicodeCasing` and
`JsRealm.String`. Broiler.JS implements them through Broiler.Unicode's `JSStringSpecialCasing` over the
runtime's data and the host culture.

**X2 - met, with six corrections of Broiler.JS and one of this family.**
- **C1**: U+00AA and U+00BA are Cased, so `'ªΣ'.toLowerCase()` ends in ς.
- **C4**: the Turkic lower-case path maps through the runtime alone and misses the Unicode 16 and 17 pairs.
- **C5**: its Soft_Dotted set misses four code points.
- **C6**: More_Above treats a class-0 mark as transparent.
- **C7**: a list of locales reaches `CultureInfo` and throws.
- **C8**: `null` and no argument read the host's culture.
- **C12** corrects this family: its ceiling is checked only after a mapping is appended and not at all in the
  language pass. The owner's maximum result length is checked on the whole result.
- **C3** is class C5 under SJL-D-6.
- **C10**, this family's realm without Intl ignoring the locale, is its policy and stays the adapter's.

**X6 - extended.**
- **Broiler.VM:**
  - `JsUnicodeCasing`'s code;
  - in `JsRealm.String`: `LanguageCasing`, `AfterCapitalI`, `AfterSoftDotted`, `BeforeDot`, `MoreAbove` and
    `AppendCodePoint`, and the bodies of `StringChangeCase` and `TransformCase` but for the locale's resolution;
  - **Soft_Dotted in the Intl data**: `JsIntlTables.IsSoftDotted`, the table's enumeration entry and its part
    of `CldrTableGenerator`. The owner's casing tables carry Soft_Dotted.
- **Broiler.JS:** every use of `JSStringSpecialCasing` and the `CultureInfo` resolution. A locale maps to the
  owner's language enumeration through Broiler.JS's own locale canonicalisation.

### Normalization

**X1 - met for this family, and an adoption for Broiler.JS.** This family implements the four forms in
`JsUnicodeNormalization` and `JsRealm.String`. **Broiler.JS implements none**: its `normalize` and its
collator's decomposition are the host's `string.Normalize`. Moving it onto the owner's normalizer is an
adoption of pinned data in place of host data, recorded as such.

**X2 - met, with three corrections of Broiler.JS and one of this family.**
- **N1**: a lone surrogate throws, in `normalize` and through the collator in `localeCompare`.
- **N2**: Unicode 16's decompositions are missing on the host's ICU, which is class C5 under SJL-D-6.
- **N3**: under invariant globalization, normalization is nothing.
- **N6** corrects this family: its ceiling is checked on the decomposition, so a composed result that would fit
  can be refused.
- **N4 and N5 agree.**

**X6.**
- **Broiler.VM:**
  - `JsUnicodeNormalization`'s code;
  - in `JsRealm.String`: `IsNormalizedAlready`, `OrderCanonically`, `ComposeCanonically`, `WriteCodePoints`
    and `NormalizeText`'s body.
  - `ReadCodePoint`, which the design names, was retired at SJL-5.4.
  - The collator and `localeCompare` keep calling `NormalizeText`, which becomes the owner's normalizer with
    this family's charge.
- **Broiler.JS:** `string.Normalize` and `IsNormalized` in `normalize` and in the collator. Collation itself is
  SJL-17's.

### Properties of strings

**X1 - met.** This family implements them in `JsUnicodeStringProperties`. Broiler.JS reads Broiler.Unicode's
emoji package through two loose copies of the name map.

**X2 - met, with one correction of Broiler.JS.**
- **S1**: both engines agree, from the same sequence files by hash.
- **S2**: the loose map accepts `\p{rgiemoji}`, which is P1's correction.
- **S3**: the duplicated map is the area's own duplicate.

**X6.**
- **Broiler.VM:** `JsUnicodeStringProperties`'s lookup code.
- **Broiler.JS:** `EmojiStringPropertyNames` and the direct calls to `GetSequences`. The translator's
  alternation is SJL-11's.
- **The owner:** the map joins the emoji data, and `GetSequences` stops allocating (its roadmap's U9).
  Broiler.Regex's map is retired at SJL-11.

### Segmentation

**X1 - met for words, and an adoption for Broiler.JS's graphemes and sentences.** This family implements UAX
#29's three granularities in `JsSegmentation`. Broiler.JS implements words by a heuristic of its own, takes
graphemes from the runtime's `StringInfo`, and has no sentence segmentation (the whole string is one
segment).

**X2 - met, with two corrections of Broiler.JS and one of this family.**
- **G1**: sentence segmentation follows UAX #29.
- **G2**: word segmentation follows UAX #29: `can't`, `a_b`, `c.d` and a Katakana run are one word each, and
  each ideograph is its own, with no dictionary in either engine.
- **G5** corrects this family's charging (decision 4).
- **G3** is class C5 under SJL-D-6.
- **G4**, the word status, becomes the owner's.
- **G6**, the locale, agrees: neither engine tailors.
- **G7**, the Segments object's shape, is outside the area.

**X6.**
- **Broiler.VM:**
  - the algorithm in `JsSegmenter.cs`: `JsSegmentation`'s boundaries, its three rule sets with their
    look-behind helpers, its code-point reader and its word-likeness;
  - `JsBreakData`'s decoding, which becomes the owner's, reading the same tables through the provider.
  - The segmentation tables stay in the optional Intl assembly, regenerated by the owner's generator, and the
    objects in `JsRealm.Segmenter` stay the adapter.
- **Broiler.JS:** `ComputeGraphemes`, `ComputeWords`, `ComputeSentences` and `IsWordLike`, with their category,
  join and numeric-infix helpers.

### X3 - neutral contract, all six sub-areas

Every owner member takes code points, spans, enumerations, a meter and a limit, and answers sets, code
points, strings, boundaries and statuses. It takes no engine type, does no coercion and reads no property. A
failure is data:
- an unknown property name is a `false` from the lookup;
- an over-long result is its own status;
- an invalid form or granularity is refused by the adapter before the call, as both engines do today.

**Casing, normalization and segmentation take the caller's meter and a maximum result length**, because their
work and output grow with their input. The lookups and folding take none. Their work is bounded by the longest
name the tables hold: a longer input is refused before it is read, a constant ceiling the owner documents.

### X4 - no edge between the engines, none into Broiler.VM

The owner references neither engine, nothing in this repository, and none of the shared libraries, which may
depend on it and not the reverse (`architecture.md` section 1). Neither engine references the other.

### X5 - boundaries hold

The owner reads no culture, ICU, `CharUnicodeInfo` or `string.Normalize`: its answers come from the tables the
engine hands it. This is the conventions condition, met at SJL-7.2.
- **This family's compiler reads the lexical classes and the property sets**, as it does today. **The owner's
  code package enters Format**, which holds the lookups the compiler and the runtime both read. That edge is
  one the layering diagram did not draw and rule I1 refuses today; SJL-7.4 widens I1's allow-list for it.
- **Segmentation is an optional package**, carried only by the roots whose closure carries the Intl assembly,
  as today. Rule I3 holds the boundary.

### X7 - owners accept

The JavaScript profile owner, the Broiler.JS maintainer and the Broiler.Unicode maintainer accept all six
sub-areas and file SJL-D-6's application. One person holds the three roles (EX-30).

## Decision SJL-D-6, as this area applies it

The programme took SJL-D-6 (c) on 2026-10-11, and this record, the first data area's, files it.
- **The owner holds the pin and the generators.** Its pin starts as a copy of this family's, so rule I4,
  minted at SJL-7.4, holds the two equal by hash from its first run.
- **Each engine embeds the tables the owner's generator writes from that engine's own pin**, in assemblies of
  its own.
  - This family's stay where they are: properties, lexical classes, folding and string properties in Format;
    normalization and casing in the profile; segmentation in the optional Intl assembly.
  - Rule N22 holds the first two groups byte for byte under its budget, and rule N28 the third under its.
  - Broiler.JS's go in an assembly its cut-over names, held by a byte-for-byte test of rule SL7's shape in its
    repository.
- **The owner's algorithms read the tables through a provider** in the shape of this family's `IJsIntlData`:
  a Unicode version, and one byte span per table. Decoding stays in the owner's code. Neither engine
  references a package that carries tables. The owner's existing packages may keep shipping theirs for their
  other consumers.
- **The generator reaches each engine's tests as a pinned, test-only package**, and never a product closure.
- **The tables' format is the owner's, chosen to fit N22's and N28's budgets**, which sit close to their
  limits today. A budget that must move moves by a dated decision with its measurement, in the change that
  moves it.
- **The data version stays 17.0.0** in both engines. SJL-7 moves no pin.
- **For Broiler.JS**, each behaviour that changes because pinned data replaces the host's is a correction
  under SJL-D-16, class C5 in its rule: I2, C3, N2, N3 and G3.

## JSD-0031 §4(b): how each reason is answered

JSD-0031 refused `Broiler.Unicode.Properties` for five independent reasons. **None is answered by this record
alone**: each is answered by evidence SJL-7 collects, and **if any reason stands after SJL-7.6, the area is
refused** and JSD-0031 stands with it.

| Reason | The answer | Where it is shown |
|---|---|---|
| 1. Rules A2 and N1 forbid the dependency | The admission path of SJL-2: rule I1's allow-list admits the owner's code packages for the projects that call them, Format and the Intl assembly included, with their lock-file hashes | SJL-7.4; exit clause 9 |
| 2. Its loose name resolution does not conform | The exact-name lookup (the owner's item U1), with ECMA-262's property tables as its tests and this family's exact lookup as a reference arm | SJL-7.2 and the reference arms; exit clause 2 |
| 3. It has no decomposition or case-folding data | Normalization and folding in the owner (its item U8), passing `NormalizationTest` and `CaseFolding` | SJL-7.2; exit clause 3 |
| 4. Its provenance cannot be reproduced | The pin, the generators in its repository and the byte-for-byte test (its item U4), with rule I4 here | SJL-7.2 and SJL-7.4; exit clauses 4, 9 and 10 |
| 5. Its runtime cost | Spans over constant data in place of hex text parsed under a lock (convention 13), and each engine's measurement of `normalize`, casing and identifier scanning under the rules this record's change commits | SJL-7.6 |

**A measurement its own rule voids answers nothing.** If Broiler.JS's lane is not a controlled lane, the fifth
reason stays open for Broiler.JS, as SJL-3's clause 7 stays open, and the area is then neither met nor refused
on it.

## Decision

1. **All six sub-areas move to Broiler.Unicode** at milestone SJL-7, in two packages of the owner's:
   - **a code package** for properties, lexical classes, case folding, case mapping, normalization and
     properties of strings;
   - **an optional package** for segmentation.

   Both read their tables through the provider. They take the shape the design's section 4.1 gives: exact
   lookups, `CodePointSet`-like immutable results, a public `UnicodeVersion`, and operations with a meter and
   a maximum result length whose names avoid the tokens rule N23 rejects. Their identities and namespaces are
   settled in Broiler.Unicode's record at SJL-7.2, under its own prefix.
2. **Broiler.Unicode adopts the four owner conditions** by its dated record of this date, meets them at SJL-7.2,
   and exit clause 1 judges them. A condition not met moves the sub-areas it reaches to the Broiler.JS
   repository by a dated record.
3. **Decision SJL-D-6 is applied** as the section above states.
4. **Charging and limits.**
   - **Normalization and casing keep this family's units.** The owner's members take the caller's meter and
     charge exactly what correspondence rows N7 and C13 state. The language pass's extra input length is
     charged by this family's adapter, the split JSD-0065 (decision 2) made, so every fuel-parity probe and
     twin keeps its figure.
   - **The lookups and folding charge nothing**, as today.
   - **Segmentation's units change by this decision.** The owner's segmenter charges one unit per code point
     each rule scans, the look-behinds and the word-status scan included, which corrects G5.
     - This family's `Intl.Segmenter` work moves with it, so its rule admits a moved exhaustion verdict as
       class C2 on the `intl402/Segmenter` paths alone.
     - No probe of the command-line root reaches segmentation, whose manifest has no Intl.
   - **Limits.** Casing and normalization take the caller's maximum result length and check it on the whole
     result, which corrects C12 and N6.
     - This family passes its ceiling and keeps its `RangeError`.
     - Broiler.JS passes its own string ceiling.
     - Segmentation's output is bounded by its input.
5. **Corrections** are fixes under decision SJL-D-16 (a).
   - **Broiler.JS's** are listed in its changelog at SJL-7.5: P1 to P4, I1, I3, W1, C1, C4 to C8, N1 to N3, G1
     and G2. With them go the data-policy rows I2, C3 and G3, and the word status G4.
   - **This family's** are listed in its cut-over's record at SJL-7.4: C12, N6 and G5.
   - F2 and W5's translator row are SJL-11's.
6. **U+0085 in Broiler.JS's scanner and `RegExp.escape` is SJL-7's**, because their white-space lists are on
   SJL-7.5's deletion list and their answer becomes the owner's `IsWhiteSpace`. This revises Broiler.JS's
   record of SJL-5, which left them to SJL-14 and SJL-11. The lexer and `RegExp.escape`'s other escaping stay
   those milestones'.
7. **Public types.**
   - **This family's `JsUnicodeLexical`**, public in Format's API baseline, is deleted at SJL-7.4 as the
     milestone lists. Its two callers, the tokenizer and the matcher, call the owner, and the baseline records
     the removal in the same change.
   - **Broiler.JS's `StringSpan.Trim`, `TrimStart` and `TrimEnd`** are public and have no caller. Their
     disposition under SJL-D-25 is decided at SJL-7.5 and recorded in its `docs/public-api.md`.
   - `CharExtensions`' members are internal.
8. **The engines keep** the following:
   - receiver and argument conversion;
   - each one's locale canonicalisation and the mapping of a locale to the owner's language enumeration;
   - their ceilings and `RangeError`s;
   - the charging adapters;
   - `Intl.Segmenter`'s objects and iterators;
   - this family's realm-without-Intl policy for `toLocale*Case` (C10);
   - their regular-expression engines and Broiler.JS's translator, which are SJL-11's.
9. **The bases** are bundle `sjl-7-001`'s in each repository, taken at the commit that carries each
   repository's rule. The rules are committed with this record.
   - This family's test262 runs are the hosted workflow's.
   - Broiler.JS's are its `test262 evidence` workflow's, with negative tests included through a new input, so
     that the property-name and identifier corrections are inside the population.
   - The owner's rules are in its bundle of the same name.

## What this rejects, and why

- **Placing the area in the Broiler.JS repository now.** SJL-D-1 (b) places it with Broiler.Unicode if it
  adopts the conditions, and it does. Only a condition it fails at SJL-7.2 moves a sub-area.
- **Extending the loose lookup in place.** Its consumers keep it, with a documented warning, until they move;
  the engines take the exact lookup.
- **Sharing the bytes** (SJL-D-6 (a) or (b)). A later dated decision may move to either once both engines have
  consumed shared tables for one release.
- **Moving the regular-expression engines, collation or locale data.** These are SJL-11, SJL-17 and SJL-9.
- **Keeping a host Unicode version for Broiler.JS** for normalization or segmentation: X5 refuses it, and
  SJL-D-6 makes the change a correction.

## The design's readings

Each is corrected in `areas/unicode.md` beside the reading, which is quoted there, and in the milestone where
it repeats one.

- **`JsNumberFormat.IsWhiteSpace` and `ReadCodePoint` no longer exist** (SJL-3.4 and SJL-5.4).
  **Broiler.JS's `JSString.trimCharacters` and `NumberParser`'s list are gone** (SJL-3.5). `trim`, `Number`
  and `parseFloat` read the shared set in both engines.
- **The segmentation tables and Soft_Dotted** are written by this family's `CldrTableGenerator` and held by
  rule N28 in the Intl assembly, not by `UnicodeTableGenerator` and N22. **There is no type `JsSegmenter`**.
- **This family's remaining platform classification** includes the parser's routing checks.
- **Broiler.JS's U+0085** survives in the scanner, `RegExp.escape`, `StringSpan.Trim` and the segmenter, not in
  `trim` or `Number`. **Its group names** do not share its identifier tables. **Its `\p` lookup** is loose
  twice and can fall through to .NET's. **Its collator** throws on a lone surrogate as `normalize` does. **Its
  dependency** on Broiler.Unicode is transitive, through Broiler.Regex, while its code calls the packages
  directly.
- **Rule I1 refuses an admitted package in Format**, and SJL-7.4 needs one there. **Rule N23 does not read the
  Intl assembly.**
- **JSD-0027's statement that the Turkic and Lithuanian tailorings are not built** is stale: they are.

## Settling condition

This record is settled when:
- SJL-7.4's and SJL-7.5's cut-overs land with both deletion lists absent;
- every rule of the three repositories' bundles `sjl-7-001` is judged, whatever each decides;
- an addendum here then supersedes JSD-0031 §4(b) or confirms it, reason by reason.

## Correction entries

None in this profile's plan documents at this record: the corrections it names are applied in the cut-overs.
`areas/unicode.md`'s readings are corrected beside each, quoting it, and so are the milestone's where they
repeat one.

## What this record does not decide

- **Either cut-over.** SJL-7.4 and SJL-7.5 apply this record; each repository's rule judges its own.
- **The vectors.** SJL-7.3 adds them, and they may falsify this record (below).
- **The owner's package names, its provider's binding and its table format.** These are settled at SJL-7.2,
  within the constraints above.
- **A Unicode version.** The pin stays 17.0.0 unless a dated decision moves it.
- **A speed claim of any kind.** The measurements answer JSD-0031's fifth reason, under their rules, and
  nothing else.

## Falsified if

- SJL-7.3's vectors find a difference between the engines, or between an engine and the specification, that
  the correspondence does not classify.
- The owner's casing or normalization cannot charge this family's units without moving a fuel-parity probe or
  twin (part B of the rule).
- The owner's tables cannot fit N22's and N28's budgets in a format its algorithms decode through the
  provider, and no dated decision moves a budget.
- Broiler.Unicode cannot meet a condition at SJL-7.2. The sub-areas it reaches then move, as decision 2 says,
  and this record is revised.
