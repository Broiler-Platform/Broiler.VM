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

### SJL-9.6 — Evidence

As in SJL-3.6, plus three items:

- the Intl reference datasets in both engines;
- test262's `intl402` directories in both engines;
- the size of the data, restated by citation.

---

## Exit gate

**Data-home clauses.**

1. The data is generated from pinned, hashed inputs, and regeneration is byte-identical.
2. The public CLDR version is exposed.
3. The agreed locale set is complete.

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
