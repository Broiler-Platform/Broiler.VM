# Milestone SJL-9 — Locale data and Intl algorithms

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/locale-data-and-intl.md)

- **Objective.** Both engines run the ECMA-402 algorithms from `Broiler.ECMAScript.Intl`, over
  locale data with a public CLDR version and pinned, hashed inputs. Each engine's own
  implementations are gone. Each engine keeps its Intl objects, option reading, coercion order,
  errors and host policies. The algorithms in scope:
  - locale identifiers and likely subtags;
  - locale negotiation;
  - the Intl mathematical value and number formatting;
  - plural rules and plural ranges;
  - date-time patterns, formatting and intervals;
  - time-zone display names;
  - list, relative-time, display-name and duration formatting.

  Collation is shared after this milestone, by [SJL-17](sjl-17-collation.md), under decision
  [SJL-D-24](../decisions.md#sjl-d-24) (c).
- **Owner roles.** The Broiler.JS maintainer and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS (the locale-data home, and an engine) and Broiler.VM.
- **Waits on.** SJL-7 and SJL-8. The unsigned rounding of SJL-4. It applies three decisions taken on
  2026-10-11:
  - [SJL-D-22](../decisions.md#sjl-d-22) (a) with (c): Broiler.VM's CLDR generator, moved with its
    pins to the Broiler.JS repository, generates the shared locale data, and Broiler.JS keeps a
    host-culture provider for the locales it lacks;
  - [SJL-D-24](../decisions.md#sjl-d-24) (c): collation later, as SJL-17;
  - [SJL-D-19](../decisions.md#sjl-d-19): the default locale and the available locales as
    providers.

**The engines differ most in this area.**

- **Broiler.VM:**
  - complete CLDR 48.2.0 data for `de`, `de-DE`, `en` and `en-US`;
  - ports of ICU's pattern generator, interval formatter and zone naming;
  - UCA collation;
  - metering;
  - a deterministic `en-US` default.
- **Broiler.JS:** host cultures for availability and the default locale, the CLDR package for some
  data, hard-coded English date-format data, and several stubs and approximations. Among them:
  - `DisplayNames.of` returns its input;
  - `PluralRules.selectRange` always answers `"other"`;
  - number formatting rounds the binary value of a double rather than its `Number::toString`
    digits, so `1.15` with one fraction digit gives `"1.1"` instead of `"1.2"`.

The area moves in **sub-areas, in the order given in the [area design](../areas/locale-data-and-intl.md)**.
Each sub-area can be refused on its own.

---

## Work packages

### SJL-9.1 — Area verdicts, decisions and comparison rules

Decisions [SJL-D-22](../decisions.md#sjl-d-22) and [SJL-D-24](../decisions.md#sjl-d-24), taken on
2026-10-11, are recorded. The extraction gate is applied to each sub-area. Broiler.VM's JSD-0027 §4
recorded `UnicodeCldr.LocaleData` as "Candidate input, not accepted". The verdict on locale data
cites it: under SJL-D-22 (a) that position stands, because the shared data is Broiler.VM's own
generator's output, under the conditions JSD-0027 implies:

- a published CLDR version;
- pinned generation;
- review.

The comparison rules commit Broiler.VM's retained Intl reference datasets
(`src/tests/cldr/<area>/`, every area except `orderings`, which is SJL-17's), with their declared
divergences, as acceptance data for **both** engines.

*(Added 2026-10-26, at SJL-9.1, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md).)*
The area verdict is JSD-0082, decided on 2026-10-26: all five sub-areas accepted, plural ranges and display-name
naming as adoptions, decisions SJL-D-22 and SJL-D-24 recorded, and JSD-0027 §4's position standing. It records the
owner's decisions D1 to D35, taken under the owner's direction of 2026-10-18, which changes no rule. Its input is the
correspondence of bundle [`sjl-9-001`](../../evidence/sjl-9-001/correspondence.md), read from code in both engines,
with every behavioural cell run. The population above is read by its decision D16:
- the copies take every area under `src/tests/cldr/` but `orderings`, SJL-17's, and `timezones`, which Broiler.JS
  has held since SJL-8.4 and its rule reads there; and the three Temporal Intl programs under
  `src/tests/temporal/` (`intl`, `calendars-intl`, `zone-names`);
- every group of declared divergences is classed `spec`, `coverage` or `zone` in Broiler.JS's
  `divergence-groups.tsv`, committed with its rule, and Broiler.JS's rule admits classes V, R, Z8, O and H only;
- `segments` is copied, as written, and its lines are SJL-7's (class O).

### SJL-9.2 — The locale data

Decision [SJL-D-22](../decisions.md#sjl-d-22) (a): Broiler.VM's CLDR generator, with its pins
(`cldr.pin`, the npm package integrity values, the per-file hashes), moves to the Broiler.JS
repository. It generates the provider format the Intl library reads, for the agreed locale set.
`Broiler.Unicode.Cldr.LocaleData` is not extended; that was option (b), not taken.

Under [SJL-D-6](../decisions.md#sjl-d-6) (c), **each engine embeds the tables the generator
produces**: Broiler.VM in its optional `.Intl` assembly, as today, and Broiler.JS in its own
assemblies. Also:

- the data is optional;
- the size budget that Broiler.VM's rule N28 tests is restated in the bundle **by citation**;
- every locale added to the set is a data change under [SJL-D-6](../decisions.md#sjl-d-6).

*(Read and revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D5 and D3.)*
- **"The agreed locale set" is read per embedding**, from one generator (decision D5). This family's stays `de`,
  `de-DE`, `en`, `en-US` and `und`, so its table is unchanged under rule N28. Broiler.JS's main data is the union of
  the languages its CLDR package serves today, every locale the `intl402` files passing at its base and the reference
  datasets name, `en-US` and `und`; its plural, ordinal, plural-range and day-period tables cover every CLDR language.
  "The size budget that Broiler.VM's rule N28 tests" binds this family's list; Broiler.JS's sizes are stated in its
  bundle.
- **The work package is split** into SJL-9.2a (the dataset copies, the harness and the Intl vectors), SJL-9.2b (the
  data home: its superset CLDR pin, the generator moved whole, and Broiler.JS's tables) and SJL-9.2c (this family's
  rules N27, N28 and I4 on the home's generator, its table byte for byte). The section stood as one work package,
  SJL-9.2.

### SJL-9.3 — The Intl library

The library is built in this order. Each sub-area is gated by its own vectors and reference
datasets before the next one starts:

1. **Locale identifiers.** Parsing, canonicalisation and likely subtags, with the full CLDR alias
   data, and negotiation by lookup.
2. **Small formatters.** Plural rules (operands taken from the formatted decimal text, plus plural
   ranges), list, relative time and display names.
3. **Number formatting.** The Intl mathematical value built from `Number::toString` digits, digit
   options, patterns, compact, currency, units, ranges, and the parts.
4. **Date-time formatting.** The pattern generator, the formatter, intervals, day periods, hour
   cycles and zone names, with metazone history.
5. **Duration formatting.**

Collation is not built here. Decision [SJL-D-24](../decisions.md#sjl-d-24) (c) moved it to
[SJL-17](sjl-17-collation.md).

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D3 and D22: a work-package revision under the roadmap's section 13.)* **The order is revised**, because this
family's plural rules, relative time and duration call its number formatter, and duration also needs lists:
1. **(a)** locale identifiers, likely subtags, negotiation, Locale info and `supportedValuesOf`;
2. **(b)** the Intl mathematical value, digit options and FormatNumericToString, then plural rules and plural ranges;
3. **(c)** number formatting (patterns, compact, currency, units, ranges, parts), then list, relative time and display
   names;
4. **(d)** date-time: the pattern generator, the formatter, intervals, day periods, hour cycles, and zone names with
   metazone history, with Intl over Temporal;
5. **(e)** duration formatting.

Each is one merge of the library, SJL-9.3a to SJL-9.3e, and SJL-9.3f integrates them. **The gate of each stage is
read** as its vectors at library level, and a differential check against this family's code at its rule commit, beside
the library, test-only and JIT only, over every Intl vector, every dataset input as a library call, and seeded random
tags, numbers, dates and durations (decision D22). The datasets are programs that need an engine, so both engines'
dataset runs after the cut-overs complete the reading. The list read: "1. **Locale identifiers.** [...] 2. **Small
formatters.** Plural rules (operands taken from the formatted decimal text, plus plural ranges), list, relative time
and display names. 3. **Number formatting.** The Intl mathematical value built from `Number::toString` digits, digit
options, patterns, compact, currency, units, ranges, and the parts. 4. **Date-time formatting.** [...] 5. **Duration
formatting.**", and the gate read "Each sub-area is gated by its own vectors and reference datasets before the next one
starts."

Option reading stays in the engines. The library takes **resolved option records**, and each engine
reads options in the specification's order.

### SJL-9.4 — Broiler.VM's cut-over

**Deleted:** the algorithm parts listed in the area design, including:

- `JsLocaleTag`;
- the decoders of `JsIntlTables`;
- `JsDecimal` and `JsRawDecimal`;
- `JsNumberData` and `JsNumberFormatter`;
- `JsPluralRule`;
- `JsDatePatternGenerator`, `JsDateTimeFormatter`, `JsDateIntervalFormat` and `JsZoneNames`;
- `JsDateData` and `JsLocaleInfo`;
- the cores of the `JsRealm.*` Intl files.

**The data boundary.** The `.Intl` assembly keeps its generated CLDR table, which the generator in
the Broiler.JS repository now produces from Broiler.VM's pins ([SJL-D-6](../decisions.md#sjl-d-6)
(c)). The generator moves there, and the home starts from a copy of the pins; rule N28 still holds
the table byte for byte here, and rule `I4` holds Broiler.VM's pins equal to the home's. Composition roots hand the `.Intl` assembly's data to
the profile as today. The Format assembly's `IJsIntlData` is replaced by the shared provider
interface, or adapted to it.

**Unchanged:** optionality, the `en-US` default, and the deterministic resolution of unknown
locales.

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D8, D15, D24 and D32.)*
- **The deletion list is widened**, and every deleted type and member is registered in rule I5: the `JsRealm.Locale`
  cores; every `JsIntlTables` decoder and its lazies (numbers, dates, locale information, zone names);
  `SupportedValuesOf`'s core; the helper types `JsDecimalKind`, `JsRawFormat`, `JsNumberPart`, `JsCurrencyNames`,
  `JsAffixKind` and `JsAffixToken`, `JsNumberPattern`, `JsPluralOperands`, `JsDatePart`, `JsDateField`, `JsZoneKind`,
  `JsLocalTime`, `JsZoneNaming` and `JsDayPeriodRule`; and, on the test side, `CldrTableGenerator.cs`,
  `UnicodeDatabase.cs` and `CldrPin.cs`. `NewCollator` and `CollatorKeyData` stay, SJL-17's, and
  `JsRealm.Segmenter.cs` is SJL-7's. The list ended at "the cores of the `JsRealm.*` Intl files."
- **The reading of DurationFormat's argument stays in the adapter**, ECMA-402's own ToDurationRecord included, and the
  adapter keeps reading its options through `Base64Options`.
- **`IJsIntlData` is adapted, not replaced**: it and `JsIntlTable` stay public in the Format assembly, which the
  sentence above leaves open ("replaced by the shared provider interface, or adapted to it").
- **This family's corrections are rows Z8, L18 and L9**, each named in bundle `sjl-9-001`'s rule before any code:
  - **Z8**: the formatter reports a time-zone identifier as given, case-normalised, and still names the zone by its
    primary; `canonical-tz` is admitted to the scored suite in the same change;
  - **L18**: `Intl.Locale.prototype.getCollations`, for a language the Collator lacks, answers DefaultLocale's
    collations, and `emoji` and `eor` only for `und`. It moves the `collations` lines of
    `src/tests/cldr/locales/divergences.txt` that the rule's class L18 names;
  - **L9**: a subdivision alias whose replacement is a region is written as UTS #35 writes it, the region in lower case
    followed by `zzzz`.

  "Unchanged" above holds otherwise.

### SJL-9.5 — Broiler.JS's cut-over

**Deleted:**

- `JSIntl`'s locale-tag code and its hand-written alias tables;
- negotiation;
- the number-formatting engine, including `ExactRational` and the hand-written compact and digit
  tables;
- the plural, list, relative-time and display-name internals;
- the English-only date-time formatting engine, and the interval and zone-name helpers;
- duration-format internals;
- its uses of `Broiler.Unicode.Cldr.LocaleData` for Intl data, and the package reference itself
  once SJL-8 has removed the calendar uses.

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D13 and D24.)* **The list is extended:**
- `JSIntlDefaultNumberingSystems.g.cs` and its script, `scripts/intl/generate_default_numbering_systems.py`, whose
  table the shared data absorbs (decision D13);
- the members of `JSIntlDateTimeFormat` that no caller reaches (row T42): the constructor from a `CultureInfo`,
  `Format(DateTimeOffset, JSObject)`, `FormatEnglishHour(DateTimeOffset)`, `FormatDayPeriod(DateTimeOffset, string)`
  and the `locale` field, as row T42 names them. The overload `FormatEnglishHour(int)`, which the `dayPeriod` paths
  of `format` and `formatToParts` call, is not among them.

Broiler.JS's public Intl types stay as the engine's Intl objects, with every public member, `JSIntlDateTimeFormat`'s
public static `Get(CultureInfo)` and `ApplyAllDefaults` among them (SJL-D-25, disposition (a)). The list read as above,
ending at "its uses of `Broiler.Unicode.Cldr.LocaleData` for Intl data, and the package reference itself once SJL-8 has
removed the calendar uses."

**Embedded instead:** the CLDR tables the shared generator produces, in Broiler.JS's own assemblies
([SJL-D-6](../decisions.md#sjl-d-6) (c)).

**What stays.** Host cultures remain only as an **optional provider** for locales the shared data
lacks ([SJL-D-22](../decisions.md#sjl-d-22) (c)), and for the default locale
([SJL-D-19](../decisions.md#sjl-d-19)).

**Corrections, listed:**

- `selectRange`;
- `DisplayNames.of`;
- binary-value rounding;
- `trailingZeroDisplay`;
- `currencyDisplay: "name"`;
- German month names;
- option-reading order and repeated reads;
- the `timeZone` option ignored in one parts path;
- `numberingSystem` dropped in relative time;
- the bound `format` identity.

*(Revised 2026-10-26, at SJL-9.1, before any evidence of SJL-9 existed, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decision D25.)* **The list is the correspondence's column "Correction of Broiler.JS", row by row**, and the changelog
lists each by its row:
- **(a):** L2, L10, L12, L14, L15, L17, L20, L21, L25, L29, L30, L37, L44, L45;
- **(b):** M1a, M1b, M2b, M3, M4, M5, M7; P1, P2, P5a, P6, P7;
- **(c):** N4, N6a, N6b, N6c, N6e, N6f, N9b, N10, N11; S5, S9;
- **(d):** T2, T4, T7, T14, T19, T27, T29, T34, T35, T39;
- **(e):** U1, U2, U3, U7, U9.

The changelog also lists each data change (C5) by its row and decision, each implementation-defined change Broiler.JS
takes (C3) by its row, and the default-locale change of row L35 (decision D7). Read against the list above:
- `selectRange` is two rows, P5a, a correction, and P5b, an implementation-defined adoption; `DisplayNames.of` is two,
  S9, a correction, and S8, a data adoption;
- "German month names" is widened to non-English month, day and era names and to other calendars' names (T14, T35);
- "option-reading order and repeated reads" holds for Date's `toLocale*` (T2), NumberFormat after construction (N10),
  PluralRules (P7, M4) and DurationFormat's argument (U7), and not for any constructor's option order, which both
  engines read alike;
- "the `timeZone` option ignored in one parts path" is dropped: it was fixed on 2026-10-16 (T28).

The list read as above, from "`selectRange`" to "the bound `format` identity".

### SJL-9.6 — Evidence

As in SJL-3.6, plus three items:

- the Intl reference datasets in both engines;
- test262's `intl402` directories in both engines;
- the size of the data, restated by citation.

*(Read 2026-10-26, at SJL-9.1, by
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
decisions D22 and D23, which revise nothing.)* **"As in SJL-3.6"** is read for layers L2 and L15: the reference arm is
the differential check against this family's code, and Broiler.JS has no J arm, since its Intl depends on the host's
globalization (decision D22); there is no measurement part, as at SJL-8.7 and SJL-10.6, since the exit gate has no
measurement clause and SJL-3's clause 7 is unmet, and the tables' decoding cost is held by tests that decode each
table once per process, lazily and thread-safely (decision D23).

---

## Exit gate

**Data-home clauses.**

1. The data is generated from pinned, hashed inputs, and regeneration is byte-identical.
2. The public CLDR version is exposed.
3. The agreed locale set is complete.

   *(Read 2026-10-26, at SJL-9.1, by
   [JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
   decision D5, which revises nothing.)* The clause is met per embedding: each engine's list, generated by the one
   generator, carries every table its group carries, which a completeness test committed with Broiler.JS's tables
   holds.

**Home (Intl library) clauses.**

4. Each sub-area that is not refused is implemented under the conventions.
5. Its vectors and reference datasets pass.
6. The `SL` rules pass, and the Native AOT probe runs.

**Broiler.JS clauses.**

7. The deletion list is absent.
8. Host globalization remains only behind the optional providers.
9. test262 `intl402` per path meets the area rule.
10. Every correction is listed.

**Broiler.VM clauses.**

11. Rules `I1` to `I7` hold, and rule `I4` agrees with the CLDR pin.
12. N27 and N28 regenerate the `.Intl` table with the home's generator from Broiler.VM's pins, and
    hold it byte for byte under its budget.
13. Whole-suite test262 in both forms, and the Intl reference datasets, meet the area rule.
14. Every root is published and run in three modes.
15. Optionality is unchanged, as rule `I3` shows.

*(Revised 2026-10-11, when SJL-D-6 and SJL-D-22 were taken. Clause 12 read "N27 and N28 are
re-homed." The records listed "Broiler.Unicode's CLDR records (conditional)", which no longer
apply.)*

**Records.**

- The area-verdict JSD, which revisits JSD-0027 §4.
- Broiler.JS's record.
- Decisions `SJL-D-22` and `SJL-D-24`.

---

## Evidence bundles

`docs/evidence/sjl-9-001/` in each repository involved.

---

## Does not do

- **It does not make Intl code optional** in either engine. That is Broiler.JS's satellite item
  A-2, and a later Broiler.VM decision.
- **It does not change either engine's default-locale or available-locale policy.**
- **It does not grow Broiler.VM's supported locales as a goal.** A locale is added only by a data
  decision, with its budget.
- **It does not share collation.** That is [SJL-17](sjl-17-collation.md).
