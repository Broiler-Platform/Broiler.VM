<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0082 - Locale data and the Intl algorithms move to the shared libraries: SJL-9's area verdict, sub-area by sub-area, decisions SJL-D-22 and SJL-D-24 recorded, and the owner's decisions of 2026-10-26

**Status:** Decided 2026-10-26.
- **All five sub-areas are accepted**, in the order the work takes them:
  - **(a)** locale identifiers, likely subtags, negotiation, Locale info and `supportedValuesOf` (rows L);
  - **(b)** the Intl mathematical value, digit options and FormatNumericToString, then plural rules and plural ranges
    (rows M and P);
  - **(c)** number formatting, then list, relative-time and display-name formatting (rows N and S);
  - **(d)** date-time formatting: the pattern generator, the formatter, intervals, day periods, hour cycles, zone names
    with metazone history, and Intl over Temporal (rows T, with row Z8);
  - **(e)** duration formatting (rows U).
- **Each moves to a new library, `Broiler.ECMAScript.Intl`** (decision D9), over locale data that this family's
  generator writes from its pins in the Broiler.JS repository, the data home, and that each engine embeds (decisions
  D5 and D10 to D12). The engines keep their Intl objects, the reads of their options in specification order,
  coercion, their errors, and their default-locale and available-locale policies, as providers (decision D7).
- **Two parts are adoptions, not extractions** (X1): plural ranges (rows P5b and N9d) and display-name naming (row S8).
  Broiler.JS implements neither, and takes this family's.
- **Decisions SJL-D-22 and SJL-D-24 are recorded**, and JSD-0027 §4's verdict on `UnicodeCldr.LocaleData`,
  "Candidate input, not accepted", stands.
- **The owner's decisions D1 to D35 of 2026-10-26 are recorded**, under the owner's direction of 2026-10-18. D1 to D31
  are SJL-9's plan's recommendations. Where the correspondence showed a recommendation's text to be wrong or
  incomplete, it was decided again the same day and is recorded as revised, with the text it replaces quoted: D7, D14,
  D15, D17, D18, D19, D21, D24, D25 and D26. D32 to D35 answer findings the plan had no decision for.
- **This family's corrections are named before any run**: rows Z8, L18 and L9, made at SJL-9.4 (decisions D15 and
  D32). **Broiler.JS's are listed by row** (decision D25).
- Nothing in this profile changes with this record. The cut-overs are SJL-9.4's and SJL-9.5's own changes, and this
  record is what they apply.

**Date:** 2026-10-26.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the owner of the library and of the data home, and of the other engine;
- the Broiler.Unicode maintainer, as a holder of decision SJL-D-22, and for the route by which the generator reads the
  Unicode Character Database (decision D12);
- the core architecture owner, for rule I1's allow-list and the composition register.

**All four roles are held by one person** (EX-30), and this record does not claim that anyone independent has
read it.

**Milestone:** the programme's SJL-9, work package SJL-9.1.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section
  "Area verdicts", "Locale data and the Intl algorithms (SJL-9.1)", with this repository's half of the decisions that
  bind Broiler.JS, by these numbers. It is filed in Broiler.JS's own change, observed on its branch until it merges;
  the ledger names its merge commit when it has.
- **SJL-9 started before SJL-7 and SJL-8, which it waits on, and SJL-3 met their gates**, at the owner's words of
  2026-10-19, which the programme ledger's SJL-9.0 entry records. SJL-9 also takes SJL-4's unsigned rounding. It may
  not meet its gate before SJL-7 and SJL-8 meet theirs, and so not before SJL-3 meets SJL-3's. This record moves no
  gate.

**The owner's words of 2026-10-19.** The owner wrote: "Also continue with the other remaining SJL milestones". The
programme reads it as the choice to start SJL-9, SJL-11 and SJL-17 before the milestones they wait on meet their gates
(JSD-0081). **Each milestone files its own start entry** (JSD-0081's decision D2): SJL-9's is the ledger's SJL-9.0
entry, filed with this record. SJL-17 is covered by the same words and starts with its own record.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." This record reads it in two parts, as JSD-0078, JSD-0080 and JSD-0081 did:
- each decision SJL-9's plan leaves open is taken as the plan recommends it, and recorded below as the owner's
  decision, dated 2026-10-26, the day it is filed. The correspondence's findings F1 to F21 were each answered the same
  day, as recommended: by a revision of the decision a finding bears on, by a new decision, or as a record only;
- the programme proceeds through SJL-9's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-9 is accepted by it.
- SJL-9 still may not meet its gate before SJL-7 and SJL-8 meet theirs, and so not before SJL-3 meets SJL-3's.
- Rules still come before runs. Bundle `sjl-9-001`'s rules are committed in each repository before any code they
  judge.
- It covers decisions, not publication: the stream version that carries the library is published only with the
  owner's approval, asked for when it is ready (decision D29).

**Names.**
- **Decisions.** "Decision Dn" is always this record's. Another record's decision is named with its record, as in
  "JSD-0078's decision D1".
- **Rows.** The correspondence's rows are named by their letters: L for sub-area (a), M and P for (b), N and S for
  (c), T for (d), with row Z8, which keeps its SJL-8 name, and U for (e). A row that splits is lettered from `a`
  (`M1a`, `P5b`).
- **Findings.** F1 to F21 are the correspondence's findings against the plan and the records, in its section "What
  the rows find against the plan and the records".
- **Steps.** A step of SJL-9 is named by its work package, SJL-9.0 to SJL-9.6, with SJL-9.2 lettered a to c and
  SJL-9.3 a to f, and by the file or member it changes. SJL-9's plan is held in no repository, so its task numbers
  appear here only where a decision quotes its text.
- **Classes.** C1 to C5 are the classes of the programme's comparison templates
  ([`validation.md`](../../../../docs/javascript-shared-libraries/validation.md) section 4): a correction, a moved
  charge, an implementation-defined choice, the harness or the environment, and data policy by decision.
- **Paths.** `VM/` is `src/Broiler.VM.Profile.JavaScript/`, as in the correspondence, and a line number is the line at
  `cd1928d`.

---

## Context

The area's design is the programme's
[`areas/locale-data-and-intl.md`](../../../../docs/javascript-shared-libraries/areas/locale-data-and-intl.md). It has
no vectors yet. SJL-9.2a adds them after this record, and classifies both engines' first runs in the corpus revision
after the one that adds them.

**The correspondence is from code, and every behavioural cell was run.** It reads:
- this family at **`cd1928d`**, the default branch after SJL-11.1, whose product code is SJL-12's after commit
  `1de3af6`'s;
- Broiler.JS at **`f918b040`**, the default branch after SJL-11.1, whose product code is SJL-12's after commit
  `c29d653f`'s, with the packages it restores, `Broiler.Unicode.Cldr.LocaleData` `0.1.0-preview.3` among them;
- test262 **`ccaac100`**, and the texts decision D14 pins, each file by its SHA-256.

Each column was run so:
- **this family** through the conformance root, each probe as a raw test262 test, once in the bytecode form and once
  in the native form. No row differs by form. The command-line root composes no Intl at `cd1928d` and answers no row;
- **this family's answers on the test262 files of the three proposals it skips**, `canonical-tz`, `Intl.Locale-info`
  and `Intl.NumberFormat-v3`, in a scratch copy that admits them. Its patch is retained and was never committed, and
  it changes which files the root scores and nothing the engine answers;
- **Broiler.JS** through its shell, and every row that depends on the host again under a host culture, a host zone
  and invariant globalization;
- **Node 22 and the `cldr-json` packages** as witnesses, never the arbiter of a class.

[`correspondence.md`](../../../../docs/evidence/sjl-9-001/correspondence.md), which this record's change retains in
bundle `sjl-9-001` with every probe, each engine's answers and the map of the `intl402` files, names every member by
file and line and every row with its class. Every reading of the pinned text it rests on stands until SJL-9.2's
vectors check it, and a reading they refute is a finding against this record.

**The default branches moved after the pinned revisions**, and no move reaches a cell. The correspondence's own table
reads them at this family's `76ace86` and Broiler.JS's `a6c68dd4`, documents only in each. At filing:
- this family's is `f2ce6e9`, after SJL-12's after evidence (Broiler-Platform/Broiler.VM#170): documents and evidence
  only;
- Broiler.JS's is `58928ac9`, after its SJL-12 records and two merges (Broiler-Platform/Broiler.JS#1071 and #1072)
  that change `Broiler.JS/Broiler.JavaScript/`'s assembly code cache and project file, a compiler test and a
  performance script. None is an Intl, built-ins or library file.

**Two gaps are accepted as recorded:**
- the test262 harness files copied into the runs' `vm262/` directory are not retained. The pinned suite holds them, as
  at SJL-8 and SJL-10;
- **the `intl402` population is the report bundle `sjl-12-001` retains** at Broiler.JS's `4d813f2a` (`base/part-a`),
  the newest valid whole-suite report in UTC with negative tests, not a report at SJL-9's base, which is later.
  Broiler.JS at `f918b040` gives the same verdict on every file the map holds. Decision D5's lists are run again on the
  report of SJL-9's base before SJL-9.2 generates (finding F17).

**The test262 population.** The map holds every `intl402` file of the report: each file that exercises a row, with its
rows, the locale data it needs and each binary's verdict, and every other file with the milestone that owns it. So it
holds every file Broiler.JS passes at its base.
- Every file of the population this family fails, it fails on its locale data (decisions D5 and D21) or on row Z8.
  The copy that admits the proposals adds L17's file, which asks for more than the pinned text gives (decision D33).
- No file fails in this family because of a row classed a correction of Broiler.JS.
- No correction of sections M, P or U moves a Broiler.JS verdict in the population, so the datasets and the vectors
  are the acceptance data that see them (finding F19, decision D17).
- The plan's risk R5, a passing test262 case that asks for what the pinned text corrects, does not occur in those
  sections. It occurs at rows S7 and T8, where both engines pass a test the text contradicts (decision D33).

**What the correspondence found:**
- **Every row is classified**, in seven classes: same in both; a correction of Broiler.JS; a correction of this family;
  implementation-defined; data policy (C5); engine policy that becomes a parameter; and code no input reaches. No
  row's class is unknown.
- **This family has three corrections**: Z8, the formatter's resolved time zone; L18, `getCollations` for a language
  the Collator lacks; and L9, the form of a subdivision alias whose replacement is a region. The plan named Z8 alone
  (finding F1).
- **Broiler.JS's corrections reach every sub-area**, far past the milestone's list, and two of the plan's additions are
  not corrections (finding F6). Among them:
  - tags, keys and option values compared and lowered by culture casing, so that a KELVIN SIGN, a final line feed and
    non-ASCII digits pass (L2, L14, L45);
  - a default locale that is not among its available locales under invariant globalization (L44, finding F2).
- **Under a host culture, Broiler.JS's casing and its Intl services read two different defaults** (L35, finding F3).
- **test262 files both engines pass contradict the pinned text**, at S7 and T8, and one asks for more than the text
  gives, at L17 (finding F5).
- **This family's charges sit inside code that moves**, so the library takes the caller's meter at the points this
  family charges, in its units (finding F8).
- **Broiler.JS's numbering-system symbols, its DurationFormat's clock and its percent unit** each needed a data or
  implementation decision the plan had not taken (findings F10 to F12).
- **Every stub of the surveys and their critiques was reproduced or refuted by a run**, never carried over, and each
  leftover item of earlier bundles has its row (decision D26).

**What the design reads as current and is not:** section "The design's readings".

## The gate, sub-area by sub-area

### (a) Locale identifiers, likely subtags, negotiation, Locale info and `supportedValuesOf`

**X1 - met.**
- **This family**: `JsLocaleTag` parses a tag, ASCII only, and canonicalises it over CLDR 48.2.0's alias and BCP 47
  data, with Add and Remove Likely Subtags, engine-free; `JsRealm.Intl` holds CanonicalizeLocaleList, ResolveLocale,
  the lookup matcher and FilterLocales; `JsRealm.Locale` holds the Locale constructor, UpdateLanguageId,
  RegionPreference and the information methods over CLDR's time, week and script data; `SupportedValuesOf`'s lists.
- **Broiler.JS**: `JSIntl`'s tag validation by a case-insensitive .NET regular expression, its hand-written alias
  tables, its canonicalisation and its negotiation over the host's cultures; `JSIntlLocale`, with hand tables for hour
  cycles, text direction and the first day of the week; likely subtags through `Broiler.Unicode.Cldr.LocaleData`.

**X2 - met.**
- **Same:** L1, L3, L4, L11, L13, L16, L23, L27, L31, L34, L36, L38, L39, L41.
- **Corrections of Broiler.JS:** L2, L10, L12, L14, L15, L17, L20, L21, L25, L29, L30, L37 (the missing
  `Intl.DisplayNames.supportedLocalesOf`), L44 (the default unavailable under invariant globalization) and L45.
- **Corrections of this family:** L18 and L9 (decision D32).
- **Data policy (C5):** L5 to L8, the generated alias tables replacing Broiler.JS's hand-written ones; L19, L22, L24
  and L26, CLDR's time, numbering-system, script and week data; L32, each engine's collation data, SJL-17's.
- **Engine policy, a parameter:** L28, the available locales; L33 and L35, the default locale (decision D7); L40, each
  Collator's collations, SJL-17's; L43, the charges (decision D18).
- **Code no input reaches:** L42.

**X6 - extended** (decision D24).
- **This family:** `JsLocaleTag`; `JsIntlTables`' decoders of likely subtags, aliases and types; the `JsRealm.Intl` and
  `JsRealm.Locale` cores; `SupportedValuesOf`'s core. `NewCollator` and `CollatorKeyData` stay, SJL-17's.
- **Broiler.JS:** the tag validation and canonicalisation, the hand-written alias tables, negotiation, and
  `JSIntlLocale`'s parse helpers and hand tables.

### (b) The Intl mathematical value, digit options and FormatNumericToString; plural rules and plural ranges

**X1 - met.**
- **This family**: `JsDecimal` and `JsRawDecimal`, the mathematical value as `Number::toString`'s digits, a BigInt's
  digits or a String's exact value; `SetNumberFormatDigitOptions`; `JsNumberFormatter`'s FormatNumericToString;
  `JsPluralRule` over the formatted decimal text, with plural ranges, for its data's languages.
- **Broiler.JS**: `ExactRational`, the exact binary value of a double; its digit options and FormatNumericToString in
  `JSIntlNumberFormat`; plural rules over a double through the CLDR package, for every CLDR language; and no plural
  ranges.

**X2 - met.**
- **Same:** M2a, M6, M8, M10, P5c, P8.
- **Corrections of Broiler.JS:** M1a (binary-value rounding), M1b, M2b, M3, M4, M5, M7 (`trailingZeroDisplay`), P1,
  P2, P5a, P6, P7.
- **Implementation-defined:** P5b, every range whose ends format differently. **An adoption**: Broiler.JS has no
  plural-range algorithm, and the library takes this family's ResolvePluralRange over CLDR's plural ranges.
- **Data policy (C5):** P3, the languages whose rules each engine holds.
- **Engine policy, a parameter:** M9, the charges (decision D18).
- **Code no input reaches:** P4, this family's compact exponent 0 in PluralRules, which no rule of its data reads
  (decision D20).

**X6 - extended** (decision D24).
- **This family:** `JsDecimal`, `JsRawDecimal`, `JsPluralRule` with `JsPluralOperands`, `JsDecimalKind`, and the
  PluralRules core.
- **Broiler.JS:** `ExactRational` and the number engine's digit code; the plural internals.

### (c) Number formatting; list, relative-time and display-name formatting

**X1 - met.**
- **This family**: one formatter, `JsNumberData` and `JsNumberFormatter` (patterns, compact, currency spacing, units,
  ranges, parts), over CLDR tables for `de`, `de-DE`, `en` and `en-US`; ListFormat, RelativeTimeFormat and
  DisplayNames over CLDR data for the same.
- **Broiler.JS**: `JSIntlNumberFormat`, over host-culture symbols and hand-written compact tables; list,
  relative-time and unit patterns from the CLDR package; and a DisplayNames whose `of` answers its validated code.

**X2 - met.**
- **Same:** N1a, N2a, N3a, N5a, N7a, N8a, N9f, N13, N17a, S1a, S2, S3, S4a, S4c, S6, S7, S10. S7 is read by
  decision D33.
- **Corrections of Broiler.JS:** N4 (`currencyDisplay: "name"`), N6a, N6b, N6c, N6e, N6f, N9b, N10, N11 (the bound
  `format`'s identity), S5 (`numberingSystem` in relative time), S9 (DisplayNames' canonical code).
- **Implementation-defined:** N2c (decision D35), N5b, N5c, N9a, N9d, N12, N17c. N9d writes a range's names by plural
  ranges, part of P5b's adoption. N12, the normative-optional legacy constructor, stays in each engine.
- **Data policy (C5):** N1b, N2b, N3b, N3c, N5d, N6d, N7b, N8b, N9c, N9e, N14, S1b, S4b, and S8, **an adoption**:
  Broiler.JS names nothing, and the library carries this family's `DisplayName` and `LanguageName` with CLDR's names
  for every locale Broiler.JS embeds.
- **Engine policy, a parameter:** N16, the charges (decision D18).
- **Code no input reaches:** N15, N17b.

**X6 - extended** (decision D24).
- **This family:** `JsNumberData`, `JsNumberFormatter` and their helper types (`JsRawFormat`, `JsNumberPart`,
  `JsCurrencyNames`, `JsAffixKind` and `JsAffixToken`, `JsNumberPattern`); `JsIntlTables`' number decoders; the
  NumberFormat, ListFormat, RelativeTimeFormat and DisplayNames cores.
- **Broiler.JS:** the number-format engine with its compact and digit tables; the list, relative-time and
  display-name internals; `JSIntlDefaultNumberingSystems.g.cs` and its script (decision D13).

### (d) Date-time formatting, intervals, day periods, hour cycles, zone names and Intl over Temporal

**X1 - met.**
- **This family**: a port of ICU's pattern generator (`JsDatePatternGenerator`), formatter (`JsDateTimeFormatter`),
  interval formatter (`JsDateIntervalFormat`) and zone naming with metazone history (`JsZoneNames`), over CLDR's
  calendar data (`JsDateData`) for its locales and every calendar `supportedValuesOf` lists, with Intl over Temporal
  as `e8cc03fc` has it, `377fb80e` included.
- **Broiler.JS**: `JSIntlDateTimeFormatEngine`, whose own header names `en` and `en-US`: hand-written English names,
  the numeric order from .NET's `ShortDatePattern`, a few interval patterns, and zone names as hand tables or offsets;
  with Intl over Temporal through the same engine.

**X2 - met.**
- **Same:** T1, T6, T8, T9, T21, T25, T28, T32, T38. T8 is read by decision D33.
- **Corrections of Broiler.JS:** T2 (Date's `toLocale*` reads), T4 (the bound `format`'s identity), T7, T14 and T35
  (names outside English and outside the Gregorian calendar), T19, T27, T29 (a range in fields the format does not
  display), T34, T39.
- **Correction of this family:** Z8 (decision D15).
- **Implementation-defined:** T5, the normative-optional legacy constructor, which stays in each engine; T13, T18,
  T20, T23, T30 and T33, which the library takes from this family; T26, the names of `UTC`, `Etc/GMT` and `+00:00`,
  by the primary (decision D15).
- **Data policy (C5):** T10, T12 (U+202F and the `atTime` joins), T15, T16, T17, T22, T24, T31, T36, T37.
- **Engine policy, a parameter:** T3, Broiler.JS's string options, its extension; T11, the host zone (SJL-D-19); T40,
  the charges (decision D18).
- **Code no input reaches:** T41; T42, Broiler.JS's members with no caller.

**X6 - extended** (decision D24).
- **This family:** `JsDatePatternGenerator`, `JsDateTimeFormatter`, `JsDateIntervalFormat`, `JsZoneNames`,
  `JsDateData`, `JsLocaleInfo`, their helper types (`JsDatePart`, `JsDateField`, `JsZoneKind`, `JsLocalTime`,
  `JsZoneNaming`, `JsDayPeriodRule`), `JsIntlTables`' date, locale-information and zone-name decoders, and the
  DateTimeFormat cores.
- **Broiler.JS:** `JSIntlDateTimeFormatEngine.cs` whole; the interval and zone-name helpers; the members of
  `JSIntlDateTimeFormat` no caller reaches (T42).

### (e) Duration formatting

**X1 - met.**
- **This family**: `JsRealm.DurationFormat`, over its own NumberFormat and ListFormat, with the `fractional` style and
  [[DigitalFormat]] from the number data.
- **Broiler.JS**: `JSIntlDurationFormat`, over its NumberFormat and ListFormat, with no `fractional` style and the
  clock's separator written as `":"`.

**X2 - met.**
- **Same:** U4, U5, U6, U8, U10, U11, U12, U17, U18, U20.
- **Corrections of Broiler.JS:** U1, U2, U3, U7, U9.
- **Data policy (C5):** U13 and U14 (decision D34), U15, U16 (decision D21).
- **Engine policy, a parameter:** U19, the charges (decision D18).
- **Code no input reaches:** U21; U22, this family's ECMA-402 ToDurationRecord, which its adapter keeps (decision
  D24).

**X6 - extended** (decision D24). **This family:** the DurationFormat cores; the reading of the argument stays the
adapter's. **Broiler.JS:** the duration-format internals.

### X3 - neutral contract, all five sub-areas

- **What the shared members take and answer.** They take resolved option records, never an options object; a
  mathematical value as digits; dates and times as SJL-8's and SJL-10's records, zones through SJL-8's providers, and
  durations as records; and locale data through two layers, `IIntlDataSet` and the question-level `ILocaleData`
  (decision D8). They answer strings, parts records and statuses: no engine type, no coercion, no property access.
- **A failure is data.** An ill-formed tag, an unknown calendar or numbering system, a value out of range: each a
  status, which each engine maps to its own `RangeError` or `TypeError` at the specification's moment.
- **Limits are explicit**: the library's own, and the caller's meter.
- **Metering wherever work grows with input** (decision D18). The library takes the caller's `WorkCharge` at the
  points this family charges today, in its units. This family's adapters keep the per-element and per-part charges
  they make in the engine. Broiler.JS passes a charge that charges nothing. **There is no class C2.**
- **No culture.** No `CultureInfo`, invariant casing or culture-sensitive overload enters the library: ASCII casing or
  `NumberFormatInfo.InvariantInfo` (decision D19).

### X4 - no edge between the engines, none into Broiler.VM

`Broiler.ECMAScript.Intl` references `Broiler.ECMAScript.Algorithms` and `Broiler.Unicode.Algorithms` (decision D9),
neither engine, and nothing in this repository. Neither engine references the other.

### X5 - boundaries hold

- **No platform globalization API enters the library** (decision D19; rule I2 in this family, SL4 in the home).
  Broiler.JS's host provider reads `CultureInfo` in Broiler.JS, behind `ILocaleData`, and answers only what .NET exposes
  exactly (decision D6).
- **Optional data stays optional.** This family's locale data stays in its optional `.Intl` assembly, which the
  composition roots hand to the profile as today, and rule I3 holds that. The library's code is carried by every root
  that carries the profile (decision D9; rules K2 and K4). Making Intl code optional is a later decision (the
  milestone's "Does not do").
- **No frontend in a compiler-free root.** The area is runtime code, and the compiler does not call it.
- **No host data is needed.** Under invariant globalization, nothing throws for want of a culture (the area design's
  section 3.3; Broiler.JS's rule, part D).

### X7 - owners accept

The JavaScript profile owner, the Broiler.JS maintainer and the Broiler.Unicode maintainer accept all five
sub-areas, and record decisions D1 to D35 under the owner's direction of 2026-10-18. One person holds the roles
(EX-30).

## Decisions SJL-D-22 and SJL-D-24, recorded

**SJL-D-22, taken 2026-10-11: (a) with (c).** This record files it as the area applies it.
- **(a): this family's generator, moved with its pins to the Broiler.JS repository, writes the shared locale data.**
  The data home is `Broiler.ECMAScript/data/intl/` with `Broiler.ECMAScript.Generation/Intl/`. Each embedding has its
  own list (decision D5): this family's does not grow, and its table stays byte for byte under rule N28; Broiler.JS's
  covers the locales its passing tests and the datasets name. `Broiler.Unicode.Cldr.LocaleData` is not extended,
  which was option (b), and Broiler.JS's use of it for Intl data ends at SJL-9.5.
- **(c): Broiler.JS keeps a host-culture provider** for the locales the shared data lacks. It answers only what .NET
  exposes exactly, and never throws. This family registers none (decision D6).
- **"A size budget in Broiler.VM's evidence"** binds this family's list, which rule N28 holds. Broiler.JS's sizes are
  measured by the generator and stated in its bundle, which has no budget rule. Each later addition is a dated data
  decision with its measured size (decision D5).

**JSD-0027 §4 revisited.** Its verdict on `UnicodeCldr.LocaleData`, "Candidate input, not accepted", **stands**
(decision D27). The package is not adopted. The shared data is this family's own generator's output, under the three
conditions the milestone reads JSD-0027 to imply:
- a published CLDR version, 48.2.0, which the library exposes;
- pinned generation: this family's pin under rules N27 and N28, held entry by entry to the home's by rule I4's CLDR
  clause (decision D10);
- review, under SJL-D-5 (a).

**SJL-D-24, taken 2026-10-11: (c).** This record files it. **Collation leaves SJL-9 and is shared by SJL-17, after
SJL-9.** Until SJL-17's cut-overs, each engine keeps its own: this family's `JsCollator`, `JsCollationData`,
`NewCollator`'s core and `CollatorKeyData`; Broiler.JS's `CompareInfo` path. SJL-9's generator moves whole, collation
tables included, so SJL-17 starts from the shared generator (decision D11). Rows L32 and L40 are each engine's
collation data and collations, SJL-17's, and row L17's condition passes to SJL-17's plan (decision D33). JSD-0043 is
not amended here; SJL-17's record amends it where its sections move (decision D27).

## The default-locale and available-locale providers

SJL-D-19 made host policy a provider, and JSD-0072 defined the local-time provider and left the locale providers to
SJL-9. This record defines them, by decision D7 as revised:
- **this family's**: the default locale `en-US`; the available locales, its data's locales; an unknown locale
  resolving to the default;
- **Broiler.JS's**: the default locale, the host's `CurrentCulture`, `en-US` when that is empty or invariant; the
  available locales, the host's cultures;
- **in both, the default is available**: the available locales always include the default locale and the less narrow
  tags the same clause requires for it. Under invariant globalization Broiler.JS's are `en-US` and `en` (row L44, a
  correction of Broiler.JS), which Broiler.JS's rule holds in its part D;
- **in both, one default**: every reader of DefaultLocale reads the same provider, the Intl services, the `toLocale*`
  methods and TransformCase with no locale (row L35).

Each engine's policy stands, as the milestone's "Does not do" asks. The two readings are what the pinned text requires
of any policy, and they move Broiler.JS's answers in two places only: under invariant globalization, and with no
locale under an `az`, `lt` or `tr` host culture.

## The owner's decisions of 2026-10-26

Each decision below was taken on 2026-10-26 by the owner, under the direction quoted above.
- **D1 to D31** are SJL-9's plan's recommendations, each with its reason. Each takes the plan's text, with its task
  numbers given as the steps they name and its figures left out (validation principle 5), which the decision says
  where it does so.
- **D7, D14, D15, D17, D18, D19, D21, D24, D25 and D26** were decided again the same day, on the findings named in
  each, and are recorded as revised. Each quotes the plan's text it replaces.
- **D32 to D35** are new, on findings the plan had no decision for.

The decisions that bind Broiler.JS, in its engine, its data home or its rule, are D3 to D18, D20 to D26, D28 to D30 and
D32 to D35. Broiler.JS's area entry records its half of them, by these numbers.

### Start, numbers and staging

**Decision D1 - the start record.** One dated ledger entry, SJL-9.0, quoting the owner's words of 2026-10-19 and the
standing direction. It says that SJL-9 waits on SJL-7, SJL-8 and SJL-4's rounding, and through them on SJL-3, and that
the choice moves no gate. SJL-9's row moves to `In progress`. The reason: the precedent of SJL-4.0 to SJL-8.0 and
SJL-10.0; update rule 10 holds the row to the milestones it waits on.

**Decision D2 - numbers, names and people.** This record is the verdict, at the next free number, JSD-0082; a second
record, the cut-over's, at SJL-9.4; bundles `sjl-9-001` in both repositories, this family's rule
`decision-rule-area.md` and Broiler.JS's `decision-rule.md`. No JSC and no exclusion at SJL-9.1: SJL-9.2c takes the
next free JSC and SJL-9.6 the next free EX, at filing. The owner, the co-signers and the EX-30 sentence are as the
header states. The plan's text read "Verdict JSD at the next free number (expected JSD-0081)"; JSD-0081 is SJL-11's,
which filed first. The reason: JSD-0078's shape; SJL-D-22's holders include the Broiler.Unicode maintainer.

**Decision D3 - staging and order.** **One ledger row, with lettered work packages.** SJL-9.2 is split a to c; SJL-9.3a
to f is one merge per library stage; SJL-9.4 and SJL-9.5 are each **one cut-over merge**, prepared as one commit per
sub-area. The sub-areas are taken in the order (a) to (e) of this record's status: locale identifiers; the mathematical
value and plural rules; number formatting and the small formatters; date-time; duration. That order is a dated
work-package revision of SJL-9.3's and of the area design's section 1, each quoting the old text. Each sub-area can
still be refused at SJL-9.1, and its commit then leaves the branch. The reason: this family's plural rules, relative
time and duration call its number formatter, and duration also needs lists; one merge keeps each window to a single
merge (decision D4) and needs one published package; the precedent of SJL-4.3a to g and of SJL-10.4's and SJL-10.5's
single cut-overs.

**Decision D4 - the base and after commits.** **The after commit is the cut-over merge, and the base is its first
parent.** The base descends from the rule commit and from bundle `sjl-12-001`'s after commit in that repository, and
no other merge on the first-parent line between them changes a product path. The reason: rules come before code
(validation principle 3), and the base still precedes every change it judges; it keeps SJL-12's, SJL-11's and
SJL-17's merges out of SJL-9's comparison without holding them for weeks.

### The data

**Decision D5 - the agreed locale set (exit clause 3).** **Per-embedding lists from one generator.**
- **This family's** stays `de`, `de-DE`, `en`, `en-US` and `und`, so its table is unchanged under rule N28.
- **Broiler.JS's main-data list** is the union of four sources: the languages its CLDR package serves today (`ar`,
  `de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`, `pt`, `ru`, `sv`, `tr`, `zh`); every locale the `intl402`
  files passing at its base and the reference datasets name, which Broiler.JS's `scripts/intl/enumerate_locales.py`
  writes into its bundle `sjl-9-001`'s `locales/`; `en-US`; and `und`. Its plural, ordinal, plural-range and
  day-period tables cover **every CLDR language**, and its global tables are as this family's.
- Sizes are measured by the generator at SJL-9.2 and stated in Broiler.JS's bundle, which has no budget rule.

**Readings:** exit clause 3 is met per embedding. SJL-D-22's "size budget in Broiler.VM's evidence" binds this
family's list, which does not grow. Each later addition is a dated data decision with its measured size. The reason:
template T2 forbids a regression on any passing `intl402` path, and a host provider cannot answer plural, compact,
unit, list or relative-time questions; rule N28's budget leaves this family's table no room to grow; SJL-D-22 (a) as
taken reads "the locales Broiler.JS needs". The plan's reason gave this family's headroom under rule N28 as a size,
left out here (validation principle 5); rule N28's register row states the budget.

**Decision D6 - Broiler.JS's host-culture provider (SJL-D-22 (c)).** It answers **only what .NET exposes exactly**
(number symbols and native digits, month, day and era names, AM and PM, the first day of the week), and only for
locales the shared data lacks. Every other question answers "no data", so the algorithm uses the parent or root. It
reports a locale unavailable when the host has no culture, and never throws. This family registers none. The reason:
the provider is "never a second implementation" (the area design's section 3.3), and .NET has no plural, compact,
unit, list, relative-time, display-name, interval or day-period data.

**Decision D7 - the default-locale and available-locale providers (SJL-D-19)** *(revised 2026-10-26, findings F2 and
F3)*. Policies are unchanged, with two readings the correspondence's runs require.
- **This family:** `en-US`, its data's locales, and unknown locales resolving to the default. **Broiler.JS:** the
  host's `CurrentCulture` (`en-US` when it is empty or invariant), and the host's cultures.
- **The default is available.** In both engines the available locales always include the default locale and the less
  narrow tags the same clause requires for it. The pinned text requires it ("[[AvailableLocales]] ... must include the
  value returned by DefaultLocale", `sec-internal-slots`), and test262's `intl402/default-locale-is-supported.js`
  asserts it. Under invariant globalization Broiler.JS's available locales are therefore `en-US` and `en`. Today they
  are none (row L44, a correction of Broiler.JS). Broiler.JS's rule holds this in its part D.
- **One default.** Every reader of DefaultLocale in an engine reads the same default-locale provider: the Intl
  services, the `toLocale*` methods, and TransformCase with no locale (row L35). In Broiler.JS, under a host culture,
  `toLocaleLowerCase()` and `toLocaleUpperCase()` with no locale case by that culture's language, as its Intl services
  already resolve it. SJL-7.5's C8 is read as "casing reads the host's culture only through the default-locale
  provider". Its `null` refusal stands.

Neither change moves a verdict of either rule, since no run takes a host culture. Broiler.JS's changelog lists both by
row. These are the providers JSD-0072 left to SJL-9. **Rejected:** availability from the embedded data under invariant
globalization, a policy change the milestone leaves out; and a constant `en-US` for Intl, which this decision and
SJL-D-19 do not take.

The plan's text read: "**Policies unchanged.** Broiler.VM: `en-US`, its data's locales, unknown locales resolving to
the default. Broiler.JS: the host's `CurrentCulture` (`en-US` when empty or invariant) and the host's cultures, so
`en-US` is in its list (D5). Recorded as the providers JSD-0072 left to SJL-9." Its reason: "The milestone's "Does not
do". Availability stays the host's, so under invariant globalization Broiler.JS resolves as it does today." Row L44
shows that under invariant globalization `en-US` is not in Broiler.JS's list (F2), and row L35 that its casing reads
another default than its Intl services (F3).

**Decision D8 - the data contract.** **Two layers.** First, `IIntlDataSet`, a version and one span per `IntlTable`,
whose enum keeps `JsIntlTable`'s numbers, with 7, 23 and 24 retired and not reused, and appends values only for tables
this family's data never carries; each engine answers an empty span for a table it lacks. Second, the library decodes it
into a question-level `ILocaleData`, which Broiler.JS's host provider also implements, and which `CompositeLocaleData`
orders: the shared data first, then an optional secondary, then the parent and root, as the plan's step for the
library's skeleton orders it. **This family keeps `IJsIntlData` and `JsIntlTable` public** in Format and adapts them
internally. The reason: SJL-D-6 (c), decoding stays in shared code; `IJsIntlData`, `JsIntlTable` and
`JsComposition.IntlData` are in `public-api.txt`, and replacing them breaches rule I6 and the baseline, as SJL-8.5
avoided.

**Decision D9 - the package.** **A new `Broiler.ECMAScript.Intl`**, referencing `Broiler.ECMAScript.Algorithms` and
`Broiler.Unicode.Algorithms`. This family's profile project references it as a product entry, and every root that
carries the profile carries it (rules K2 and K4). Broiler.JS's BuiltIns uses a project reference. The reason: SJL-D-2
(a) names it; Algorithms is data-free; the programme's `architecture.md` fixes the Intl library's dependencies as
"Algorithms, Unicode; data through a provider interface". Making Intl code optional is a later decision (the
milestone's "Does not do"); rule I3 concerns data, not code.

**Decision D10 - the CLDR pin in the home, and rule I4.** `Broiler.ECMAScript/data/intl/cldr/` holds a **superset
pin**: every entry of this family's pin byte-identical in path, length and SHA-256, with its `version`, `uca-version`
and `json-integrity` lines, plus Broiler.JS's extra files from the same packages. `data/time/cldr/` is unchanged, and a
home test holds the two `timezone.json` entries and copies equal. **I4's CLDR clause is entry-wise**: each of this
family's entries and header lines equals the home's, read at a recorded Broiler.JS commit, and the generator declares
48.2.0. The reason: it follows SJL-8.5's `timezone.json` clause, keeps this family's pin unchanged and stays
mechanical; a whole-file identity would break on the superset; it is the least churn for SL7 and the time home. The
plan's text counted this family's entries; the count is not a record's (validation principle 5).

**Decision D11 - the generator.** **It moves whole**, collation included, with parameters for the main-data locales,
the supplemental locales, numbering systems, calendars and tailorings. This family's options reproduce
`JsCldrTables.g.cs` byte for byte through `TableSourceOptions` (rules N27 and N28), as SJL-8.5 did. Broiler.JS's
tables are one file per table under `data/intl/generated/`, embedded as resources in BuiltIns as the time tables are,
and held by SL7. The reason: one output file under rule N28; SJL-17's library and cut-overs assume the shared
generator writes collation; the hard-coded locale list (`CldrTableGenerator.cs:33`) must become a parameter.

**Decision D12 - how the generator reads the Unicode Character Database.** **Port the subset of this family's
`UnicodeDatabase`** the generator uses (General_Category, decompositions, canonical combining classes,
Unified_Ideograph) into Generation. The database stays an explicit input: this family hands its own pinned files,
which I4's Unicode clause already holds to Broiler.Unicode's; the home hands copies under `data/intl/ucd/`, held equal
to `UnicodePin.Owner()`'s entries. **Fallback:** Broiler.Unicode makes its Generation reader public. This family's
`UnicodeDatabase.cs` is then deleted. The reason: SJL-D-6 (c) keeps rule N28's inputs this family's pins, and the
runtime package would make them a package's embedded tables; the fallback needs another repository's release.

**Decision D13 - Broiler.JS's CLDR 47.0.0 numbering-system table.** **Absorbed** at SJL-9.5 by the shared data:
`JSIntlDefaultNumberingSystems.g.cs` and `scripts/intl/generate_default_numbering_systems.py` are deleted. The move
from 47.0.0 to 48.2.0 is C5, listed. A dated note is filed on the data decision of 2026-10-15 in Broiler.JS's plan.
The reason: that plan already says "SJL-9 moves it into the shared locale data".

### The texts, the zones, the datasets and the vectors

**Decision D14 - the ECMA-402 text both engines follow** *(revised 2026-10-26, finding F4)*. `tc39/ecma402` at the
last `main` commit on or before 2026-05-08, the commit date of test262 `ccaac100`:
`3d4c558180f643d7dea75f011a6355fb285f377e` (2026-04-16). It is recorded by its forty-digit identifier and the rendered
specification's SHA-256 in Broiler.JS's `conformance/vectors/sources/ecma402.pin`.

The pin already holds the Intl Locale Info proposal, merged as `70b0ecc` (2026-02-19). The proposal's last commit by
then, `4926520c`, reads the same for every operation the correspondence cites. The pin names IsWellFormedLanguageTag
(formerly IsStructurallyValidLanguageTag), and records cite operations by the pinned names.

Added to the pin:
- the Intl era and month code proposal, at its last commit on or before the same date, recorded in the same file. Both
  engines implement it (this family admits `Intl.Era-monthcode`, JSD-0056), and files of the suite claim it. The
  correspondence read `5833eae`; SJL-9.2a confirms that it is the last commit on or before the date, or records the one
  that is;
- Intl over Temporal, by `tc39/proposal-temporal@e8cc03fc`'s `spec/intl.html` (JSD-0078's decision D1), which carries
  `377fb80e`.

This family's admitted proposals are otherwise unchanged (decision D15). test262 stays at `ccaac100`. A conflict
between a passing test and the text is a falsifier ("Falsified if"), except at rows S7 and T8, which decision D33
reads. The reason: without a pin, a correction cannot be told apart from drift; that date keeps the text and the suite
contemporary; the era proposal is pinned for the reason the Locale Info proposal was: both engines implement it, and
rows T33 and T35 rest on it.

The plan's text read: "**`tc39/ecma402` at the last `main` commit on or before 2026-05-08**, the commit date of
test262 `ccaac100`, recorded by its forty-digit id and the rendered spec's SHA-256 in
`conformance/vectors/sources/ecma402.pin`. Added to it: the Intl Locale Info proposal at its commit of the same date,
which both engines implement; and Intl over Temporal by `tc39/proposal-temporal@e8cc03fc`'s `spec/intl.html`
(JSD-0078 D1), which carries 377fb80e. Broiler.VM's admitted proposals are otherwise unchanged (D15). test262 stays at
`ccaac100`, and a conflict between a passing test and the text is a falsifier (section 8)." The finding also said that
JSD-0046 cites IsStructurallyValidLanguageTag by the old name. No record of this repository does: only the doc
comments at `VM/JsLocaleTag.cs:25` and `:86`, which SJL-9.4 deletes, name it.

**Decision D15 - Z8: the formatter's resolved time zone** *(revised 2026-10-26, finding F9)*. **Settled by the pinned
text: the identifier as given, case-normalised.** `e8cc03fc`'s `intl.html:434` replaces CanonicalizeTimeZoneName with
`[[Identifier]]`, and both engines follow it (JSD-0078's decision D1); SJL-D-20 ties `canonical-tz` to that edition.

It is this family's correction, made at SJL-9.4, which also admits `canonical-tz` to its scored suite. It touches
only the resolved-name lines of its divergences files and their headers: rewritten, removed where Node's retained line
already resolves the identifier to itself, or added. Broiler.JS does not change its resolved identifier.

*New:* **The zone keeps its names.** The shared resolution answers both the identifier and its primary. The engine
adapter reports the identifier as [[TimeZone]], and the library names the zone (its metazone, and its generic,
specific and location names) by the primary, as this family does at the base. Naming by the identifier would move
`Etc/GMT`'s names from "Coordinated Universal Time" to "Greenwich Mean Time" in this family's dates and zone-names
datasets (row T26, implementation-defined, which keeps this family's answer). Broiler.JS's names for such an
identifier move to the primary's, a C3 change listed under T26.

The vectors' per-engine Z8 expectations become one at the next revision. The reason: template T2 forbids moving
Broiler.JS, which passes the `canonical-tz` tests; `ecma402`'s `main` still reads [[PrimaryIdentifier]], but the
Temporal revision both engines pin overrides it; JSD-0076 left the edition question to SJL-9; and T26's names are the
implementation's, which this family's rule keeps.

The plan's text read: "**Settled by the pinned text: the identifier as given, case-normalised.** `e8cc03fc`'s
`intl.html:434` replaces CanonicalizeTimeZoneName with `[[Identifier]]`, and both engines follow it (JSD-0078 D1);
SJL-D-20 ties canonical-tz to that edition. **It is Broiler.VM's correction, made at SJL-9.4**, which also admits
`canonical-tz` to its scored suite, and touches only the resolved-name lines of its divergences files (rewritten,
removed where Node's retained line already resolves the identifier to itself, or added) and their headers. Broiler.JS
does not change. The shared resolution answers both names, and the engine adapter takes the identifier. The vectors'
per-engine Z8 expectations become one at the next revision." Its reason: "T2 forbids moving Broiler.JS, which passes
the canonical-tz tests. `ecma402` `main` still reads `[[PrimaryIdentifier]]`, but the Temporal revision both pin
overrides it. JSD-0076 left the edition question to SJL-9."

Finding F9 showed that "only the resolved-name lines" held only if the zone's names kept coming from the primary. The
revision changes two of the plan's sentences:
- "Broiler.JS does not change." now reads "Broiler.JS does not change its resolved identifier.", because its names for
  an identifier that is not its own primary move under row T26;
- "The shared resolution answers both names, and the engine adapter takes the identifier." gives way to the paragraph
  marked *New*, which also has the library name the zone by the primary.

The paragraph marked *New* and the reason's last clause, on T26's names, are added.

**Decision D16 - the reference datasets as acceptance data.** Copy every `src/tests/cldr/` area except `orderings`
(SJL-17's) and `timezones` (already copied, SJL-8.4), plus the three Temporal Intl programs, into Broiler.JS's
`conformance/datasets/intl/` with a new pin. Extend the harness (`--bundle sjl-9-001`,
`--invariant-globalization`). **Classify every divergence group** as `spec`, `coverage` or `zone` in Broiler.JS's
`divergence-groups.tsv`, committed with its rule. Broiler.JS's part C admits classes V, R, Z8, O and H only.
`segments` is copied, as the milestone says, and its line is class O. The reason: the milestone's "acceptance data
for both engines"; it follows SJL-8.4's dataset discipline; this family's divergence files mix the draft's
corrections with its coverage policy.

**Decision D17 - vector provenance and Broiler.JS's extra locales** *(revised 2026-10-26, findings F13 and F19)*.
Expected values come from ICU, through Node on CLDR 48 (the release Broiler.JS's bundle's `reference.txt` names).
Each divergence is argued against an ECMA-402 clause, and `impl-defined` is used only where the text leaves the
answer.

Three readings are fixed before any vector:
- **(a) DurationFormat.** Where the reference answers as Node 22 does on rows U1 and U3 (the display default of
  minutes and seconds after a numeric unit; `fractional` with `always`), the vector takes the pinned text's answer,
  argued against `sec-getdurationunitoptions` and `sec-validatedurationunitstyle`, as the durations dataset's first
  divergence group does.
- **(b) Date-time strings.** The reference's `format` writes U+0020 where ICU's pattern has U+202F, V8's own
  substitution. So a date-time vector's expected string is the concatenation of the reference's `formatToParts`
  values, which keep U+202F. Node 22 is no witness for row T12.
- **(c) Rows the datasets do not reach.** The vectors carry rows U7 and U9, which no dataset line reaches. No
  correction of sections M, P or U moves a test262 verdict, so the datasets and the vectors are their only acceptance
  data.

No new reference datasets: Broiler.JS's extra locales rest on test262, the vectors (one per table family for each
extra locale), and its part C's class R against the same reference. The reason: ICU is independent of both engines,
and CLDR 48 removes the release gap the datasets had to absorb; datasets for every extra locale would be a second
corpus to maintain; the reference carries the same DurationFormat lineage and the same substitution as Node 22, so the
readings are fixed before any vector exists.

The plan's text read: "Expected values come from **ICU through Node on CLDR 48.x** (the release named in
`reference.txt`), each divergence argued against an ECMA-402 clause; `impl-defined` only where the text leaves it.
**No new reference datasets**: Broiler.JS's extra locales rest on test262, the vectors (one per table family for each
extra locale), and part C's class R against the same reference."

### The library

**Decision D18 - metering (X3)** *(revised 2026-10-26, finding F8)*. Every library path whose work grows with its
input takes the caller's `WorkCharge`: tag parsing and canonicalisation, plural evaluation over digits, the pattern
search over skeletons, list and display-name composition, and number, date and duration formatting.

**The library charges where this family charges today, in its units:**
- the mathematical value of a String, `length / 8 + 1` (`VM/JsDecimal.cs:215`);
- the mathematical value of a BigInt, through its decimal digits (`:198-204`);
- FormatNumericToString, `digits / 16 + 1` (`VM/JsNumberFormatter.cs:821`);
- CreatePartsFromList, one per template step (`VM/JsRealm.ListFormat.cs:217`);
- the date formatter, its output's length (`VM/JsDateTimeFormatter.cs:298`, `:316`);
- PartitionDurationFormatPattern, one per unit visited (`VM/JsRealm.DurationFormat.cs:597`), plus the charges of the
  number and list formats it builds, CanonicalizeLocaleList's per-element charge included.

**This family's adapters keep the engine-side charges:**
- one per element where they walk a script value: CanonicalizeLocaleList and FilterLocales (`VM/JsRealm.Intl.cs:282`,
  `:550`), and StringListFromIterable (`VM/JsRealm.ListFormat.cs:179`);
- one per part where they build a `formatToParts` array: `VM/JsRealm.ListFormat.cs:94`,
  `VM/JsRealm.NumberFormat.cs:216`, `VM/JsRealm.RelativeTimeFormat.cs:106`, `VM/JsRealm.DateTimeFormat.cs:460` and
  `VM/JsRealm.DurationFormat.cs:124`.

Tag parsing, canonicalisation and likely subtags charge nothing today, so this family's `WorkCharge` charges nothing
there (row L43). Broiler.JS passes a `WorkCharge` that charges nothing. **No C2.**

The fuel references are the correspondence's bisections of rows M9, N16, T40 and U19, in both forms. N16's list
program costs more over its control than its named charges explain, so SJL-9.4 bisects N16's two probes again after
the cut-over and compares the results, rather than reasoning from the formulas. The reason: X3's "metering wherever
work grows with input" is met by the contract, and the charge applied is engine policy, as JSD-0078 read it for the
Temporal grammar; this family's rule admits no C2 and judges part A's fuel exhaustions, so a charge that moves is a
regression.

The plan's text read: "**Every library path whose work grows with its input takes the caller's `WorkCharge`**: tag
parsing and canonicalisation, plural evaluation over digits, the pattern search over skeletons, list and display-name
composition, number and date formatting. Broiler.VM's adapters pass exactly today's charges (its [n] sites, at their
units); Broiler.JS passes none. **No C2.**" Its count of the sites is left out here (validation principle 5), and it
named no site and no unit.

**Decision D19 - X5 in the moved code** *(revised 2026-10-26, finding F20)*. Every `ToUpperInvariant`,
`ToLowerInvariant` and `CultureInfo` use and every culture-sensitive overload in the moving code becomes ASCII casing
or a call with `NumberFormatInfo.InvariantInfo`. That covers the critique's list, and `VM/JsNumberFormatter.cs:1013`'s
`int.TryParse(string, out int)`, the current-culture overload (row N15).

No input reaches a culture difference at any of them at `cd1928d` (rows L42, M10, N15, T41, U20), so the port moves no
answer of this family.

Broiler.JS's culture-invariant ignore-case matching does move answers (rows L2, L14, L45). Those are corrections of
Broiler.JS, not X5 items. The reason: rule I2 and SL4 forbid the overload in the home whether or not an input reaches
it. The plan's text read: "Every `ToUpperInvariant`, `ToLowerInvariant` and `CultureInfo` use in the moving code
(about [n] lines, listed in the critique) becomes **ASCII casing** or **`NumberFormatInfo.InvariantInfo`**." Its
count is left out here (validation principle 5).

**Decision D20 - plural operands `c` and `e`.** The shared plural engine carries both, and `PluralRules.select` with
compact notation passes the exponent. For this family's `de` and `en` no category moves, so it is recorded as a
latent correction with no answer changed. The reason: Broiler.JS supplies `e` today, and its other locales (`fr`,
`es`, `it`, `pt`) need `c` and `e`. **Confirmed by row P4** (finding F21): this family's `de` and `en` rules read
neither `c` nor `e`, so PluralRules' exponent 0 moves no answer.

**Decision D21 - non-`latn` numbering systems** *(revised 2026-10-26, finding F10)*. This family's limitation stays
(`latn` symbols for every numbering system), recorded under rule N28's budget.

Broiler.JS's embedding carries each numbering system's own CLDR 48.2.0 symbols, for every locale of its list, through
the locale's parent chain to root, and the locale's `latn` symbols where CLDR has none. Root holds `arab`'s and
`arabext`'s in `common/main/root.xml`, which this family's CLDR pin already archives. The `cldr-json` package's
`und/numbers.json` carries only the `latn` symbols, so the generator reads root's from that file. Every formatter
writes numbers from that one table. So `en-u-nu-arab` writes U+066B and U+066C, and U+061C before the minus sign.

**The reason.** Broiler.JS passes test262's `DateTimeFormat/prototype/format/numbering-system.js` at its base. That file
asserts U+066B for `en-US-u-nu-arab` (row T37), and template T2 forbids losing it. The other option, own symbols only
for a locale's default or native system, would write `.` there once one table serves every formatter.

**What moves.** Broiler.JS's NumberFormat and DurationFormat write `latn` symbols with `arab` digits today (rows N8a,
U16), so their answers move at SJL-9.5, a data change (C5) listed by row. N8a's class at the base stays "same", with
that note. Broiler.JS's part C admits a durations line that equals this family's line with the numbering system's own
symbols.

The plan's text read: "**Broiler.VM's limitation stays** (latn symbols for every system), recorded under N28's budget.
Broiler.JS's embedding carries native digits and symbols for its locales' default and native systems, as appended
keys or tables Broiler.VM's data does not carry." Its reason: "Broiler.VM cannot grow. Broiler.JS passes non-latn
tests today." That holds for digits only (F10).

**Decision D22 - the L2 form, and the gate of each library stage.** **A differential check against this family's
code** (its code at its rule commit, beside the library, test-only, JIT only) over a declared population: every Intl
vector, every dataset input as a library call, and seeded random tags, numbers, dates and durations. It runs as
SJL-8.3 and SJL-10.3 ran theirs. **No J-arm copy** of `JSIntl`, which depends on host globalization. Broiler.JS's side
is the correspondence's runs, the vectors, its part C and test262. **Home clause 5 is read** as the vectors at library
level, plus the differential check over the dataset inputs, plus both engines' dataset runs after the cut-overs.
Dated notes on SJL-9.3 and SJL-9.6. The reason: the validation document's section 3 allows JIT-only arms, but a
host-dependent J arm's answers vary by host; the datasets are programs that need an engine.

**Decision D23 - L15.** **No measurement part**, as SJL-8.7 and SJL-10.6 had none. The exit gate has no measurement
clause, and SJL-3's clause 7 (the controlled lane) is unmet. A dated note on SJL-9.6's "as in SJL-3.6". The decode
cost is held by L3 tests: each table is decoded once per process, lazily and thread-safely. The reason: a figure could
not be quoted anyway, and a measurement without a lane would decide nothing.

### Deletion, corrections and the records

**Decision D24 - the deletion lists (X6) and SJL-D-25** *(revised 2026-10-26, finding F14)*.

**This family:** the area design's list, **plus**:
- the `JsRealm.Locale` cores (`:333-592`);
- every `JsIntlTables` decoder and its lazies (numbers, dates, locale info, zone names);
- `SupportedValuesOf`'s core;
- the helper types: `JsDecimalKind`, `JsRawFormat`, `JsNumberPart`, `JsCurrencyNames`, `JsAffixKind` and
  `JsAffixToken`, `JsNumberPattern`, `JsPluralOperands`, `JsDatePart`, `JsDateField`, `JsZoneKind`, `JsLocalTime`,
  `JsZoneNaming`, `JsDayPeriodRule`;
- on the test side, `CldrTableGenerator.cs`, `UnicodeDatabase.cs` and `CldrPin.cs`.

**Minus:** `NewCollator` and `CollatorKeyData` (SJL-17's), and `JsRealm.Segmenter.cs` (SJL-7's).

**Broiler.JS:** the area design's list, plus `JSIntlDefaultNumberingSystems.g.cs` and its script. Its public Intl types
(`JSIntlNumberFormat` and the others) **stay as the engine's Intl objects, with every public member** (SJL-D-25
disposition (a)). Nothing public is removed.

*New:*
- **U22.** The reading of DurationFormat's argument stays in this family's adapter. That includes ECMA-402's own
  ToDurationRecord, the reading where Temporal is not composed (row U22, reached by no root built at `cd1928d`), beside
  ToTemporalDuration's reads. Its validation goes through the shared IsValidDuration. The library's duration-format
  core takes a duration record and reads no argument.
- **`Base64Options`.** The adapter keeps reading DurationFormat's options object through `Base64Options`
  (`VM/JsRealm.Base64.cs:186`), the GetOptionsObject the profile shares, or an equal call. SJL-9.4's adapter review
  names it.
- **T42.** Broiler.JS's deletion list gains the members of `JSIntlDateTimeFormat` that no caller reaches: the
  constructor from a `CultureInfo`, `Format(DateTimeOffset, JSObject)`, `FormatEnglishHour`, `FormatDayPeriod`, and
  the `locale` field. The public static `Get(CultureInfo)` and `ApplyAllDefaults` stay, over the library, and
  Broiler.JS's `docs/public-api.md` records them.

The reason: an incomplete list misses rule I5's rows; the public members are the prototype entry points the engine
keeps; ToDurationRecord's work is reading a script value's properties, which is engine work, as ToTemporalDuration's
reads already are, and deleting it would drop a reading ECMA-402 alone still states, for a composition, Intl without
Temporal, that the profile's optionality may yet build.

The plan's text read: "**Broiler.VM:** the area design's list, **plus**: the `JsRealm.Locale` cores (`:333-592`);
every `JsIntlTables` decoder and its lazies (numbers, dates, locale info, zone names); `SupportedValuesOf`'s core; the
helper types (`JsDecimalKind`, `JsRawFormat`, `JsNumberPart`, `JsCurrencyNames`, `JsAffixKind` and `JsAffixToken`,
`JsNumberPattern`, `JsPluralOperands`, `JsDatePart`, `JsDateField`, `JsZoneKind`, `JsLocalTime`, `JsZoneNaming`,
`JsDayPeriodRule`); and, on the test side, `CldrTableGenerator.cs`, `UnicodeDatabase.cs` and `CldrPin.cs`.
**Minus:** `NewCollator` and `CollatorKeyData` (SJL-17), and `JsRealm.Segmenter.cs` (SJL-7). **Broiler.JS:** the area
design's list, plus `JSIntlDefaultNumberingSystems.g.cs` and its script. Its public Intl types (`JSIntlNumberFormat`
and the others) **stay as the engine's Intl objects with every public member**, disposition (a). Nothing is
removed." Its reason: "An incomplete list misses I5 rows. The public members are the prototype entry points the engine
keeps."

Finding F14 narrows the plan's last sentence, "Nothing is removed.", to "Nothing public is removed.": the members of
T42 that are not public join Broiler.JS's deletion list, and its public ones stay. The paragraph marked *New* and the
reason's sentence on ToDurationRecord are added.

**Decision D25 - Broiler.JS's correction list** *(revised 2026-10-26, findings F6 and F7)*. SJL-9.5's correction list
is the correspondence's column "Correction of Broiler.JS", row by row:
- **L:** L2, L10, L12, L14, L15, L17, L20, L21, L25, L29, L30, L37, L44, L45;
- **M and P:** M1a, M1b, M2b, M3, M4, M5, M7; P1, P2, P5a, P6, P7;
- **N and S:** N4, N6a, N6b, N6c, N6e, N6f, N9b, N10, N11; S5, S9;
- **T:** T2, T4, T7, T14, T19, T27, T29, T34, T35, T39;
- **U:** U1, U2, U3, U7, U9.

SJL-9.5's changelog lists:
- each of those by its row;
- each data change (C5) by its row and its decision (D5, D13, D21, D34);
- each implementation-defined change Broiler.JS takes (C3) by its row: P5b, N2c (decision D35), N5b, N5c, N9a, N9d,
  N17c, T13, T18, T20, T23, T26 (decision D15), T30, T33;
- the default-locale change of row L35 (decision D7).

**The plan's additions, as the runs class them:**
- `Intl.DisplayNames.supportedLocalesOf` (L37), PluralRules' digit options (P1), `select` and `selectRange` of a
  BigInt (P2), percent's default fraction digits (M5), and the bound `format` identity of NumberFormat and
  DateTimeFormat (N11, T4) are corrections.
- The weekday-only range is a correction, widened to any field the format does not display (T29).
- Currency code spacing is data (N3b, CLDR's `currencySpacing`, C5).
- Range spacing and the collapse of shared affixes are implementation-defined (N9a, N17c, C3). The collapse is a
  correction only where it makes two different ranges print alike (N9b).

**Reclassed items of the milestone's list:**
- "German month names" is widened to non-English month, day and era names and to other calendars' names (T14, T35).
- `selectRange` is two rows: both ends formatting alike (P5a, C1), and every other range (P5b, C3, an X1 adoption).
- `DisplayNames.of` is two rows: the code's case and validation (S9, C1), and the names (S8, C5, an X1 adoption).

**Unchanged from the plan:** the drops stand (T28, fixed on 2026-10-16); the `atTime` joins and U+202F are C5 data
(T12); the Collator's compare identity is SJL-17's.

**Option reads.** The milestone's "option-reading order and repeated reads" holds for Date's `toLocale*` (T2),
NumberFormat after construction (N10), PluralRules (P7, M4) and DurationFormat's argument (U7). It does not hold for
any constructor's option order, which both engines read alike (L4, L13, L36, L38, N13, T1, U18).

The reason: the rows are the evidence, each run in both engines and classed against the pinned text, where the plan's
list came from probes at `main` and mixed three classes. The plan's text read: "Revise SJL-9.5's list. **Add:**
`Intl.DisplayNames.supportedLocalesOf`; PluralRules' digit options and `select` of a BigInt; percent's default fraction
digits; currency code spacing and range spacing; the weekday-only range; the bound `format` identity of NumberFormat
**and** DateTimeFormat. **Widen** "German month names" to non-English month and day names. **Drop**, as fixed on
2026-10-16 (`5e0538df`, `ee6c6dda`): "the `timeZone` option ignored in one parts path" and `formatToParts(8.64e15)`.
The `atTime` joins and U+202F are **C5 data**, not corrections. The Collator's compare identity is SJL-17's."

**Decision D26 - the leftover items of earlier bundles** *(revised 2026-10-26, finding F15)*. Each has a row of the
correspondence, whose section "The leftover items (D26)" gives each verdict.

**From this repository's bundles:**
- `sjl-3-001/README.md:269` (the plan cited `:254`);
- `sjl-4-001/correspondence.md:120-124` (rows 1, 4 and 5);
- `sjl-5-001/README.md:247, 252`;
- `sjl-5-001/correspondence.md:183`, whose members are now `JSStringPrototype.Transform.cs:141`, `:149`. It is refuted
  as stated: since SJL-7.5's C8, casing with no locale reads `en-US` (row L35), and what remains is decision D7's one
  default;
- `sjl-6-001/correspondence.md:186-189`;
- `sjl-8-001/README.md:544` (`JsZoneNames.City` for `Etc/`);
- `sjl-10-001/correspondence.md:198-202` (`377fb80e`);
- the Intl calendar identifiers, `sjl-10-001/README.md:809` (the plan cited `sjl-10-001/correspondence.md:236-241`);
- Z8.

**From Broiler.JS's bundles:**
- `sjl-3-001/README.md:207` (the plan cited `:194`);
- `sjl-4-001/README.md:267-268, 278`;
- `sjl-6-001/README.md:356, 368`;
- `sjl-7-001/README.md:172, 548`.

The reason: they are recorded as SJL-9's in earlier bundles of both repositories and would otherwise be missed. The
plan's citations had drifted, and one item read as an open defect what SJL-7.5 had already changed.

The plan's text read: "Each becomes a row of the correspondence. From Broiler.VM's bundles: `sjl-3-001/README.md:254`;
`sjl-4-001/correspondence.md:120-124` (rows 1, 4 and 5); `sjl-5-001/README.md:247, 252` and `correspondence.md:183`;
`sjl-6-001/correspondence.md:186-189`; `sjl-8-001/README.md:544` (`JsZoneNames.City` for `Etc/`);
`sjl-10-001/correspondence.md:198-202, 236-241` (377fb80e, the [n] Intl calendar identifiers); Z8. From Broiler.JS's
bundles: `sjl-3-001/README.md:194`; `sjl-4-001/README.md:267-268, 278`; `sjl-6-001/README.md:356, 368`;
`sjl-7-001/README.md:172, 548`." Its reason: "They are recorded as SJL-9's in [n] bundles and would otherwise be
missed." Its counts are left out here (validation principle 5).

**Decision D27 - earlier records.** JSD-0027 §4 **stands** (the package was not accepted, and the shared data is this
family's generator's output, pinned and reviewed). JSD-0046 §5 is **amended** for `getTimeZones`, by a dated note.
JSD-0043 is **not amended** here; it is SJL-17's. The reason: the roadmap's section 11 row for SJL-9, and update rules
1 and 5.

The same dated note also carries finding F16's readings of the section's `getCalendars` (row L16) and `getCollations`
(row L18, decision D32), which are a record only (section "The design's readings"). They do not revise this decision.

**Decision D28 - cut-over order.** **This family first**, which validates the port against an unchanged answer set;
then Broiler.JS. The two merges may be days apart, and each repository is judged alone. The reason: SJL-10.4 then
SJL-10.5; a library defect shows first where no answer should move.

**Decision D29 - publication.** One stream version carrying the complete library and the generator, **published with
the owner's approval**, asked for when both cut-over branches pass their local pre-checks on the packed library, and
before this family's cut-over merges, so that the cut-over needs no re-pin inside its merge. A later fix is a new
version and its own re-pin. The reason: SJL-D-4; the owner approved `0.1.0-preview.5` to `0.1.0-preview.7` one by
one; the standing direction covers decisions, not publication.

**Decision D30 - the rules' parts.** This family: A (test262 in both forms), B (the datasets and the checks), C (the
twins). Broiler.JS: A (UTC, with negative tests), B (New York), C (the datasets), D (invariant globalization). They are
bundle `sjl-9-001`'s `decision-rule-area.md` here and `decision-rule.md` in Broiler.JS. The reason: part D carries
the area design's "never throws".

**Decision D31 - the run-time coupling.** `JsIntlTables` keeps its name and `JsEngine` its constructor signature, so
that `BigIntChecks`' `UnsafeAccessor` still binds. If either must change, the same change updates
`BigIntChecks.cs:1250-1277` and runs the slice compiler's `--checks`. The reason: the coupling breaks at run time, not
at compile time.

### The decisions the correspondence required

**Decision D32 - this family's corrections are rows Z8, L18 and L9** (finding F1). SJL-9.4 makes all three, and bundle
`sjl-9-001`'s `decision-rule-area.md` names each before any code.

**L18.** For a locale whose language `Intl.Collator` does not support in this family's data,
`Intl.Locale.prototype.getCollations` answers DefaultLocale's collations: `[]` for this family's `en-US`. It answers
`["emoji", "eor"]` only for `und`, as CollationsOfLocale has it. The change rewrites the `[emoji,eor]` divergence
lines of `src/tests/cldr/locales/divergences.txt` for such languages and adds one for each such line whose retained
answer is `[emoji,eor]`. It also rewrites their header, whose "emoji and eor for one it does not" is the misreading.
Part B's class L18 admits exactly those lines. The Collator's own data (JSD-0043) does not change and stays SJL-17's.

**L9.** A subdivision alias whose replacement is a region is written as UTS #35 Annex C writes it: the region in lower
case followed by `zzzz` (`en-u-sd-fi01` becomes `en-u-sd-axzzzz`). No test262 file and no dataset line reaches it.

**Rejected.** Leaving L18 to SJL-17 as a listed divergence. It would keep a reading the pinned text contradicts,
either in the shared library or in a carve-out of one method from decision D24's deletion list, and the engines would
answer the operation's fallback differently through one shared operation from SJL-9.5 on.

The reason: the rules are not yet filed, so naming L18 now keeps rules before code (validation principle 3); the defect
is in this family's code (`VM/JsRealm.Locale.cs:151` answers `emoji, eor` whenever the Collator lacks the language),
not in its data; the lines it moves are mechanical and listable, as Z8's are. The correspondence named only the
group's `[emoji,eor]` divergence lines. The correction also adds a divergence line for every `collations` line of a
language other than `und`, `de` or `en` whose retained answer is `[emoji,eor]`, which this family matches today, and
part B's class admits both. In Broiler.JS, L9's inputs move at SJL-9.5 with the generated alias table, a data change
listed under row L8, to the corrected form.

**Decision D33 - where a passing test262 file and the pinned text part** (finding F5). For a row where a test262 file
of `ccaac100` that both engines pass asks for an answer the decision D14 text does not give, the file is the arbiter.
The library keeps both engines' answer.

**(a) S7.** In PartitionRelativeTimePattern the number written into a past pattern is the magnitude of ℝ(value):
`format(-1, "second")` is `"1 second ago"`. That is what `RelativeTimeFormat/prototype/format/en-us-numeric-always.js`
asserts, and what both engines and Node write. The pinned step
"PartitionNumberPattern(_relativeTimeFormat_.[[NumberFormat]], ℝ(_value_))", whose NumberFormat's `signDisplay` is
`"auto"`, is read as formatting the absolute value, because the past pattern carries the sign. `tc39/ecma402` `main`
at `3691634` (2026-10-23) reads as the pin.

**(b) T8.** CreateDateTimeFormat, as `e8cc03fc` amends it, sets [[HourCycle]] always. `resolvedOptions` reports
`hourCycle` and `hour12` only when [[DateTimeFormat]] has an [[hour]] field, as `3d4c558`'s text and
`DateTimeFormat/prototype/resolvedOptions/hourCycle-dateStyle.js` and `hourCycle.js` have it. The amendment serves the
Temporal formats' hour.

Both rows stay "same" in both engines, and no answer moves. The vectors carry them with this decision as their reason
(decision D17).

**(c) L17.** `Locale/prototype/getCollations/output-array-values.js` asks `en` for at least one collation, which the
pinned text does not require. The pinned text is the arbiter of the library's CollationsOfLocale, which reads each
engine's `Intl.Collator` data. Broiler.JS's Collator keeps its host path until SJL-17 and lists `emoji` and `eor` for
`en`, so the file keeps passing if Broiler.JS's adapter hands the operation that list. SJL-9.5's local pre-check
holds it. SJL-17's plan inherits the condition: Broiler.JS's Collator data for `en` keeps a collation, or SJL-17's
rule names the file.

**How the rules read them.** In both rules the files keep their base verdicts, and no class admits a move. This
family skips L17's file at both commits (`Intl.Locale-info` is not admitted). The falsifier reads "a passing test262
case at `ccaac100` asks for an answer the pinned text corrects, other than rows S7 and T8, which decision D33 reads".

The reason: the suite is the contemporaneous evidence decision D14 dates the pin by; every implementation, Node and
both engines among them, answers as the suite asserts; and in each case the text's literal answer is a gap, not a
decision: in S7 the past pattern already carries the sign, and in T8 the amendment's purpose is the Temporal formats'
hour, as its own tests of a format with an [[hour]] field show. Taking the text as the arbiter would make both rules
NOT MET by construction, and no later text settles S7. For L17 the text does not conflict with the test: it allows
none, and the test asks for one.

**Decision D34 - DurationFormat's digital data** (finding F11). [[DigitalFormat]] is CLDR 48.2.0 data, which the
shared generator writes into each embedding's number table. For each locale it writes `symbols.timeSeparator` and the
`durationUnit-type-hm`, `-hms` and `-ms` patterns. [[TwoDigitHours]] is true where the `hms` pattern begins `hh`, as
this family reads it (JSD-0051 §2).

This is a dated data decision (C5). For Broiler.JS's list it moves `digital` answers where CLDR says so, each listed in
its changelog: two-digit hours for `af` and `lt` (row U14), and `.` as the clock's separator for `da`, `en-DK`, `id`
and `si` (row U13). This family's `de` and `en` do not move (rule N28).

**The vectors** carry a `digital` line for each locale of Broiler.JS's list whose separator is not `:` or whose `hms`
pattern begins `hh`. Where the reference (decision D17) writes one-digit hours, the vector keeps the library's answer
as a data divergence, argued from the CLDR 48.2.0 field, not `impl-defined`.

**The completeness test of exit clause 3**, which SJL-9.2b commits, holds those keys for every locale of Broiler.JS's
list, so that none falls back to `:` and one digit for want of a key.

**Rejected:** ICU's reading of the pattern, which departs from CLDR's data to follow one implementation's omission;
and `impl-defined` vectors, which would misname what the text makes locale data. The reason: `hh:mm:ss` is CLDR's own
statement of two-digit hours for those locales, which JSD-0051 §2 already takes; no test262 file and no dataset line
covers either, so the vectors and the completeness test are the only checks.

**Decision D35 - the percent unit's long form (row N2c)** (finding F12). `style: "unit"` with `unit: "percent"` keeps
this family's answer: the percent pattern in every `unitDisplay`. Every implementation-defined row does so under this
family's rule, which admits no C3.

Broiler.JS's long form moves from CLDR's unit pattern (`"50 Prozent"`) to the percent pattern. That is a C3 change,
listed in its changelog under N2c with the note that it departs from CLDR's `percent` unit pattern.

**Rejected:** listing it as a correction of this family. The text leaves the unit's affixes to the implementation.

The normative-optional legacy constructors (N12, T5) stay in each engine. The reason: moving this family's numbers
dataset would need a part B class for a choice the text does not require; under this decision, Broiler.JS's part C line
equals this family's expected answer after its cut-over. A later dated data decision may adopt CLDR's pattern in both
engines.

## Every row, classified

Each row is in one class, as the correspondence's section "The rows by class" puts it. The columns are its classes,
"Same in both" and "Engine policy (a parameter)" shortened.

| Sub-area | Same | Correction of Broiler.JS | Correction of Broiler.VM | Implementation-defined | Data policy (C5) | Engine policy | Code no input reaches |
|---|---|---|---|---|---|---|---|
| (a) L | L1, L3, L4, L11, L13, L16, L23, L27, L31, L34, L36, L38, L39, L41 | L2, L10, L12, L14, L15, L17, L20, L21, L25, L29, L30, L37, L44, L45 | L9, L18 (decision D32) | | L5, L6, L7, L8, L19, L22, L24, L26, L32 | L28, L33, L35, L40, L43 | L42 |
| (b) M and P | M2a, M6, M8, M10, P5c, P8 | M1a, M1b, M2b, M3, M4, M5, M7, P1, P2, P5a, P6, P7 | | P5b (an adoption) | P3 | M9 | P4 |
| (c) N and S | N1a, N2a, N3a, N5a, N7a, N8a, N9f, N13, N17a, S1a, S2, S3, S4a, S4c, S6, S7, S10 | N4, N6a, N6b, N6c, N6e, N6f, N9b, N10, N11, S5, S9 | | N2c, N5b, N5c, N9a, N9d, N12, N17c | N1b, N2b, N3b, N3c, N5d, N6d, N7b, N8b, N9c, N9e, N14, S1b, S4b, S8 (an adoption) | N16 | N15, N17b |
| (d) T | T1, T6, T8, T9, T21, T25, T28, T32, T38 | T2, T4, T7, T14, T19, T27, T29, T34, T35, T39 | Z8 (decision D15) | T5, T13, T18, T20, T23, T26, T30, T33 | T10, T12, T15, T16, T17, T22, T24, T31, T36, T37 | T3, T11, T40 | T41, T42 |
| (e) U | U4, U5, U6, U8, U10, U11, U12, U17, U18, U20 | U1, U2, U3, U7, U9 | | | U13, U14, U15, U16 | U19 | U21, U22 |

**What the decisions of 2026-10-26 add to the classes:**
- **S7 and T8** are "same" in both engines, and both pass test262 files the pinned text contradicts; decision D33
  makes the files the arbiter, and no answer moves. **L17**'s test asks for more than the text gives; decision D33
  (c) states the condition Broiler.JS's Collator data meets.
- **N8a** is "same" at the base, and **U16** data policy: Broiler.JS's `en-u-nu-arab` and `de-u-nu-arab` symbols move
  at SJL-9.5, a data change (decision D21).
- **N2c** keeps this family's answer, and Broiler.JS's long form moves, a C3 change (decision D35).
- **T26** keeps this family's names, by the primary, after Z8 moves the resolved identifier; Broiler.JS's names for
  such an identifier move, a C3 change (decision D15).
- **U13 and U14** move Broiler.JS's `digital` answers where CLDR says so (decision D34).
- **L35** stays engine policy, and Broiler.JS's no-argument casing under an `az`, `lt` or `tr` host culture moves to
  the one default (decision D7). **L44** is a correction of Broiler.JS under invariant globalization (decision D7).
- **U22** stays code no input reaches, in this family's adapter (decision D24).

**Broiler.JS's corrections**, each a fix under decision SJL-D-16 (a), are listed in its changelog at SJL-9.5 with
their rows, with its data and implementation-defined changes (decision D25). **This family's corrections** are Z8,
L18 and L9, made at SJL-9.4 (decisions D15 and D32); its rule names each before any code.

## The correspondence's findings, and what answers each

| Finding | What it found | Answered by |
|---|---|---|
| F1 | L18 and L9 are corrections of this family besides Z8, and L18 moves dataset lines part B did not admit | Decision D32; part B's class L18; the falsifier on the datasets |
| F2 | Under invariant globalization Broiler.JS's default `en-US` is not an available locale (L44) | Decision D7, revised; Broiler.JS's rule, part D |
| F3 | Casing and Intl read two defaults in Broiler.JS under a host culture (L35) | Decision D7, revised |
| F4 | The pin holds Locale Info; the era and month code proposal is missing; the operation's rename | Decision D14, revised. No record of this repository uses the old name |
| F5 | S7 and T8: passing files the pinned text contradicts; L17's test asks for more | Decision D33; the falsifier excepts S7 and T8 |
| F6 | D25's list misses most corrections of Broiler.JS, and two of its additions are not corrections | Decision D25, revised |
| F7 | "Option-reading order and repeated reads" holds for T2, N10, P7, M4 and U7 only | Decision D25, revised |
| F8 | The charges' points and units | Decision D18, revised; N16 bisected again after the cut-over |
| F9 | Z8 moves names too unless the zone is named by its primary (T26) | Decision D15, revised |
| F10 | `arab` symbols; the durations coverage claim | Decision D21, revised; Broiler.JS's bundle README notes `divergence-groups.tsv`'s row 43 |
| F11 | DurationFormat's two-digit hours and clock separators | Decision D34 |
| F12 | N2c's percent long form | Decision D35 |
| F13 | Node as a witness for DurationFormat and U+202F | Decision D17, revised |
| F14 | U22, `Base64Options` and T42 | Decision D24, revised. Two of T42's members are public and stay |
| F15 | D26's citations have drifted; one item is refuted | Decision D26, revised |
| F16 | Stale readings: the divergence groups' rows 49 and 50, JSD-0046 §5 | Record only: section "The design's readings"; JSD-0046's dated note; Broiler.JS's bundle README |
| F17 | The base report the D5 lists read | Record only: section "Context"; Broiler.JS's area entry and bundle README |
| F18 | The brief's path for Broiler.JS's Intl code | Record only: the correspondence's "Paths" paragraph |
| F19 | No correction of sections M, P or U moves a test262 verdict; the vectors must carry U7 and U9 | Record only, with decision D17 (c) |
| F20 | `JsNumberFormatter.cs:1013`; no answer of this family moves | Decision D19, revised |
| F21 | D20 confirmed by P4 | Record only: decision D20 |

## The design's readings

Each is corrected beside the reading, which is quoted there, in this record's change.
- **The area design**, `areas/locale-data-and-intl.md`:
  - section 1's order is revised to (a) to (e) (decision D3);
  - section 3.1's single interface is read as decision D8's two layers;
  - section 3.3's host provider is read by decisions D6 and D7;
  - section 3.4's "Today" cell for Broiler.JS, "platform normalization first", is stale since SJL-7.5: the collator
    decomposes through Broiler.Unicode before the host's collation;
  - section 4's deletion lists are extended (decision D24);
  - section 5's corrections are reconciled with SJL-9.5's list (decision D25).
- **The milestone**, `sjl-9-locale-data-and-intl.md`: SJL-9.1's population (decision D16); SJL-9.2's "the agreed
  locale set", per embedding (decision D5); SJL-9.3's order and gate (decisions D3 and D22); SJL-9.4's deletion list
  and corrections (decisions D24 and D32); SJL-9.5's deletion list (decision D24) and correction list (decision D25);
  SJL-9.6's "as in SJL-3.6" (decisions D22 and D23); and exit clause 3, per embedding (decision D5).
- **SJL-17's milestone**: SJL-17.5's deletion item "the collator's platform normalization" was done at SJL-7.5.
- **JSD-0046 §5** (decision D27, finding F16):
  - **`getCalendars`** answers `gregory` because the profile's data holds no CLDR calendar preference data (row L16),
    not because the profile formats the Gregorian calendar alone, which it has not done since SJL-10.4;
  - **`getCollations`**, for a language the Collator does not support, answers DefaultLocale's collations (`[]`), with
    `emoji` and `eor` only for `und`: the correction of SJL-9.4 (decision D32, row L18);
  - **`getTimeZones`** answers the region's zones from tzdb's `zone.tab` through SJL-8's shared database, in code-unit
    order, `[]` for a region with none, and `undefined` without a region (row L23).
- **This family's `src/tests/cldr/locales/divergences.txt`**: the calendar group's header (its lines 37 to 39) gives the
  same stale reason as JSD-0046 §5. It stays as committed through SJL-9's window, because part B of this family's rule
  admits no change of a comment outside classes Z8 and L18; this record and JSD-0046's note record it as stale, and a
  change after SJL-9.6's after runs may rewrite it. The collations group's header is rewritten at SJL-9.4 under class
  L18 (decision D32).
- **Broiler.JS's `divergence-groups.tsv`**, committed with its rule, keeps its bytes; its bundle's README carries a
  dated note on its rows 49, 50 and 43 (finding F16): the calendar group is class `spec`, where it read `coverage`;
  the collations group stays `coverage`, its header the misreading row L18 corrects; and the durations `arab` group's
  "Broiler.JS's carries" holds only after SJL-9.5 (decision D21).

## Decision

1. **All five sub-areas move to `Broiler.ECMAScript.Intl`**, under X3's constraints: resolved option records, data
   through `IIntlDataSet` and `ILocaleData` (decision D8), statuses, and the caller's meter (decision D18). SJL-9.3
   settles the names.
2. **The data** is this family's generator's output, from its pins, in the Broiler.JS repository (SJL-D-22 (a);
   decisions D10 to D12), for each embedding's list (decision D5). This family's table stays byte for byte.
   Broiler.JS keeps a host provider for the rest (decision D6).
3. **The texts.** Both engines follow ECMA-402 at `3d4c558`, with Intl over Temporal by `e8cc03fc` and the Intl era and
   month code proposal (decision D14). Where a test262 file both engines pass and the text part, at S7 and T8, the file
   is the arbiter (decision D33).
4. **The engines keep:** their Intl objects and prototypes; the reads of options and arguments in specification order,
   and coercion; their errors; bound functions and their caching; the parts arrays; the legacy constructors; their
   default-locale, available-locale and zone providers (decision D7); and their charges, passed through the meter.
5. **Corrections.**
   - **This family's** are Z8, L18 and L9, made at SJL-9.4 (decisions D15 and D32).
   - **Broiler.JS's** are the rows decision D25 lists, fixes under SJL-D-16 (a), each listed in its changelog at
     SJL-9.5 with its data and implementation-defined changes.
6. **The deletion lists** are X6's (decision D24). The milestone's SJL-9.4 and SJL-9.5 deletion lists are revised
   under update rule 5.
7. **The reference datasets** this family retains under `src/tests/cldr/` and `src/tests/temporal/` are the acceptance
   data of both engines (decision D16). Here, part B holds them unchanged but for classes Z8 and L18.
8. **The bases** are each repository's bundle `sjl-9-001`'s, at the first parent of its cut-over merge (decision D4).
   The rules are committed with this record.
9. **JSD-0027 §4 stands; JSD-0046 §5 is amended by a dated note; JSD-0043 is not amended** (decision D27).

## What this rejects, and why

- **Adopting `UnicodeCldr.LocaleData`**: JSD-0027 §4 stands; the shared data is this family's generator's output
  (SJL-D-22 (a), decision D27).
- **One locale list for both engines**: this family's table cannot grow, and Broiler.JS may not lose a passing test
  (decision D5).
- **A J-arm copy of `JSIntl`**: its answers vary with the host (decision D22).
- **Leaving L18 to SJL-17 as a listed divergence**: the shared library would keep a reading the text contradicts
  (decision D32).
- **The pinned text as the arbiter at S7 and T8**: both rules would be NOT MET by construction (decision D33).
- **Naming a zone by the identifier as given**: it moves names the text leaves to the implementation (decision D15).
- **Own symbols only for a locale's default numbering system**: Broiler.JS would lose a passing test (decision D21).
- **ICU's reading of DurationFormat's patterns**: it departs from CLDR's data (decision D34).
- **Listing N2c as a correction of this family**: the text leaves the unit's affixes to the implementation (decision
  D35).
- **Deleting ECMA-402's ToDurationRecord**: Intl without Temporal is a composition the profile may yet build
  (decision D24).
- **Availability from the embedded data under invariant globalization, and a constant `en-US` for Intl**: policy
  changes the milestone and SJL-D-19 do not take (decision D7).

## Settling condition

This record is settled when SJL-9.4's and SJL-9.5's cut-overs have landed with both deletion lists absent, and every
part of both repositories' `sjl-9-001` rules has been judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record. SJL-9.2c takes the next free JSC, and SJL-9.4's cut-over its
own. These carry dated notes beside the text they correct, each quoting it, in this record's change: the milestone
file, `areas/locale-data-and-intl.md`, SJL-17's milestone file, decisions SJL-D-19, SJL-D-22 and SJL-D-24 in
`decisions.md`, and JSD-0046 §5.

## What this record does not decide

- **The cut-overs.** SJL-9.4 and SJL-9.5 apply this record, and SJL-9.4 files the second record.
- **The vectors.** SJL-9.2a adds them, and they may falsify this record.
- **Collation**, which is SJL-17's (SJL-D-24 (c)).
- **Making Intl code optional** in either engine: a later decision (the milestone's "Does not do").
- **Publication**, which waits on the owner's approval (decision D29).
- **A speed claim of any kind.** SJL-9 measures nothing (decision D23).

## Falsified if

- A vector or a dataset line finds a difference the correspondence does not classify.
- A reading of the pinned text in the correspondence that the vectors refute, until the row is reclassified here.
- The home's generator, from this family's pins, does not write `JsCldrTables.g.cs` byte for byte.
- A passing test262 case at `ccaac100` asks for an answer the pinned text corrects, other than rows S7 and T8, which
  decision D33 reads.
- This family's datasets move at its cut-over outside rows Z8 and L18.
