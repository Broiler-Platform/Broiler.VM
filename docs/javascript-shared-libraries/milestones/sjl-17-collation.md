# Milestone SJL-17 — Collation

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/locale-data-and-intl.md#34-collation-sjl-17)

- **Objective.** Both engines collate through one shared implementation of the Unicode Collation
  Algorithm in `Broiler.ECMAScript.Intl`, over CLDR's root collation and the tailorings of the shared
  locale set, with `numeric` and `caseFirst` applied. Each engine's own collation is gone, except
  for Broiler.JS's optional host provider for the locales the shared data does not cover. Each
  engine keeps its `Intl.Collator` objects, option reading in specification order, the bound
  `compare` function, `localeCompare`'s coercion, and its errors.
- **Owner roles.** The Broiler.JS maintainer (the Intl library and the locale data, and Broiler.JS's
  cut-over) and the JavaScript profile owner (Broiler.VM's cut-over). One person.
- **Repositories.** Broiler.JS (home, and an engine) and Broiler.VM.
- **Waits on.** SJL-9: the Intl library, its locale identifiers and provider interface, and the
  locale data. Through SJL-9, SJL-7 for normalization.

**Why this milestone exists.** Decision [SJL-D-24](../decisions.md#sjl-d-24) was taken on 2026-10-11
as (c): collation is shared after SJL-9, not in it. It is the last Intl component, as both surveys
ranked it, and the one where the engines differ most:

- **Broiler.VM** implements UCA over CLDR's root collation and a few tailorings for its locales
  (`JsCollator`, `JsCollationData`; JSD-0043), with `numeric` and `caseFirst`. Its collation tables
  are generated into the optional `.Intl` assembly.
- **Broiler.JS** uses the host's `CompareInfo`. It supports every host locale, resolves `numeric`
  and `caseFirst` without applying them, and normalizes through the platform first (SJL-7 removes
  that step).

**Extraction or adoption.** Broiler.JS delegates collation to the platform rather than implementing
the algorithm, so the verdict of SJL-17.1 states, under condition X1 of the
[extraction gate](../roadmap.md#5-the-programmes-extraction-gate), whether the area is an
extraction or an adoption. Either may move; the record says which.

---

## Work packages

### SJL-17.1 — Area verdict and comparison rules

The extraction gate is applied to the area, and the verdict is filed as a JSD in the JavaScript
profile's series, amending JSD-0043 where it moves, and as a record in Broiler.JS's
shared-libraries plan. The comparison rules are committed first. They commit Broiler.VM's retained
`orderings` reference dataset (`src/tests/cldr/orderings/`), with its declared divergences, as
acceptance data for **both** engines.

*Done when* both records and the rule files exist.

### SJL-17.2 — Vectors

The corpus gains vectors for:

- root order across scripts, with canonically equivalent and ignorable sequences;
- every tailoring the shared data carries;
- the options `Intl.Collator` resolves: `sensitivity`, `ignorePunctuation`, `numeric` and
  `caseFirst`;
- lone surrogates.

The UCA collation test files Broiler.VM pins under `src/tests/cldr/pins/` are the library-level
oracle.

*Done when* the vectors exist with provenance, and both engines' current results are classified.

### SJL-17.3 — The implementation

`Broiler.ECMAScript.Intl` gains `UcaCollator`:

- comparison by the levels the resolved options select, over collation data read through the
  library's provider interface;
- **resolved options as a record**, never an options object;
- normalization through the Unicode owner (SJL-7), never the platform;
- a meter for the work that grows with the inputs.

**The data.** CLDR's root collation and the tailorings come from the CLDR generator in the
Broiler.JS repository. Each engine embeds the tables it produces from the pinned CLDR inputs,
Broiler.VM from its own pins, which rule `I4` holds equal to the home's
([SJL-D-6](../decisions.md#sjl-d-6) (c)). A tailoring added for a locale is a data change under that
decision, with Broiler.VM's budget restated.

*Done when* the vectors, the UCA test files and the reference dataset pass at library level.

### SJL-17.4 — Broiler.VM's cut-over

**Deleted:** `JsCollator` and `JsCollationData`, the algorithm and the decoder, and the collation
core of the `JsRealm.*` Intl files.

**Kept and regenerated:** the collation tables in the `.Intl` assembly, which the shared generator
produces, held byte for byte by rule N28.

**Unchanged:** optionality, and what `localeCompare` does in a composition that carries no Intl
data.

### SJL-17.5 — Broiler.JS's cut-over

**Deleted:** the `CompareInfo` path of `Intl.Collator` and `String.prototype.localeCompare` for the
locales the shared data covers, and the collator's platform normalization.

**Kept:** an optional host provider for the locales the shared data does not cover, behind the same
provider as decision [SJL-D-22](../decisions.md#sjl-d-22) (c).

**Corrections, listed:**

- `numeric` and `caseFirst` applied;
- ordering that no longer depends on the host's ICU for the covered locales.

### SJL-17.6 — Evidence

As in SJL-3.6, plus three items:

- the `orderings` reference dataset in both engines;
- test262's `intl402/Collator` and `String.prototype.localeCompare` directories in both engines;
- the size of the collation data, restated by citation.

---

## Deliverables

`UcaCollator` and its vectors; both cut-overs; the area-verdict JSD and Broiler.JS's record; both
bundles.

---

## Exit gate

**Home clauses.**

1. The collator is implemented under the conventions. Its vectors, the UCA test files and the
   reference dataset pass at library level.
2. The `SL` rules pass, and the Native AOT probe runs.

**Broiler.JS clauses.**

3. The deletion list is absent, and host collation remains only behind the optional provider.
4. test262 `intl402/Collator` and `localeCompare` per path meet the area rule.
5. Every correction is listed.

**Broiler.VM clauses.**

6. Rules `I1` to `I7` hold, and rule `I5` holds the deletion list.
7. Whole-suite test262 in both forms, and the `orderings` reference dataset, meet the area rule.
8. Every root is published and run in three modes.
9. Optionality is unchanged, as rule `I3` shows, and N28's budget holds for the collation tables.

**Records.** The area-verdict JSD, amending JSD-0043 where it moves, and Broiler.JS's record.

---

## Evidence bundles

`docs/evidence/sjl-17-001/` in Broiler.JS and in Broiler.VM.

---

## Does not do

- **It does not change either engine's default-locale or available-locale policy**
  ([SJL-D-19](../decisions.md#sjl-d-19)).
- **It does not grow Broiler.VM's supported locales.** A locale is added only by a data decision,
  with its budget.
- **It does not share `Intl.Segmenter`**, which is SJL-7's, or `localeCompare`'s coercion, which
  stays in each engine's adapter.
