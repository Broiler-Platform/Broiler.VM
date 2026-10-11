# Milestone SJL-3 — Number conversion, the pilot

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/numbers.md)

- **Objective.** Both engines convert numbers to strings and strings to numbers through one shared
  implementation, `Broiler.ECMAScript.Algorithms`, and **neither engine keeps one of its own**. The
  conversions in scope are:
  - `Number::toString` at every radix;
  - `toFixed`, `toExponential` and `toPrecision`;
  - `StringToNumber`, `parseInt` and `parseFloat`;
  - the values of numeric literals, BigInt literal values included;
  - array-index key parsing;
  - the ECMAScript string-whitespace set.

  Whether the programme continues is no longer this milestone's to decide. Decision
  [SJL-D-23](../decisions.md#sjl-d-23) was taken before the pilot, on 2026-10-11: continue. Its
  criteria are this milestone's exit clauses (SJL-3.7).
- **Owner roles.**
  - **Broiler.JS:** its maintainer, for the library, the home evidence and Broiler.JS's cut-over.
  - **Broiler.VM:** the JavaScript profile owner, for Broiler.VM's cut-over; the architecture owner,
    for the composition register and the rules.
  - All one person.
- **Repositories.** Broiler.JS (home and consumer), Broiler.VM.
- **Waits on.** SJL-1 and SJL-2. It applies three decisions taken on 2026-10-11, and its records
  carry them: [SJL-D-10](../decisions.md#sjl-d-10) (the `WorkCharge` delegate),
  [SJL-D-16](../decisions.md#sjl-d-16) (corrections are fixes) and
  [SJL-D-25](../decisions.md#sjl-d-25) (forwarders by default).

**Why number conversion is the pilot.**

- Broiler.VM's `JsNumberFormat` already takes and returns plain CLR values and references no engine
  type.
- Broiler.JS's equivalents are pure functions too.
- The area has a precise specification, an exhaustive vector population, and no data.
- It also removes **duplication inside each engine**:
  - Broiler.VM's compiler carries its own copy of number rendering (`JsParser.NumberText`) and of
    BigInt literal parsing (`JsParser.TryBigIntValue`), because it may not reference the runtime
    profile.
  - Broiler.JS converts numbers in its parser (`NumberCoercion`), in its runtime (`NumberParser`)
    and in its built-ins (`JSNumber.ToECMAString`). The last one is reached through a delegate the
    built-ins initializer installs, because the runtime may not reference the built-ins.

---

## Work packages

### SJL-3.1 — The area verdict

The programme's extraction gate (roadmap section 5, conditions X1 to X7) is applied to the area,
using [areas/numbers.md](../areas/numbers.md)'s correspondence table re-derived at named revisions.
The verdict is filed as a JSD in the JavaScript profile's series and as a record in Broiler.JS's
shared-libraries plan. The JSD also records decisions `SJL-D-10` and `SJL-D-16`. On refusal, this
area ends like any other, with its pointers; the programme continues under
[SJL-D-23](../decisions.md#sjl-d-23). *(Revised 2026-10-11: the last sentence read "On refusal, the
programme stops at SJL-D-23's branch (c).")*

*Done when* both records exist.

### SJL-3.2 — The comparison rules are in force

The decision-rule files committed at SJL-0 are confirmed unchanged. A revision would be a new dated
file quoting the old one, written before any run it judges. **No code of SJL-3.3 to SJL-3.5 exists
before them.**

*Done when* the bundles can show the ancestry.

### SJL-3.3 — The implementation in the home

`Broiler.ECMAScript.Algorithms` gains the operations below, under the conventions. The API sketch
and the algorithm choices are in [areas/numbers.md](../areas/numbers.md).

| Operation | Algorithm requirement |
|---|---|
| Shortest round-trip digits; `Number::toString` at radix 10 | Correct for every double, **including the cases where the .NET `"R"` format is known not to round-trip** (Broiler.JS's own test of `String(2**-25)` records one). Either the library implements a shortest-digits algorithm itself, or it verifies the platform's digits and falls back to an exact search. A `"G17"` fallback is not shortest and is not acceptable. |
| `Number::toString` at radix 2 to 36 | Broiler.VM's algorithm: digits stop when the remaining fraction is below half the gap to the next double, rounded half-to-even with carry. The integer part is exact for values beyond `long`. |
| `toFixed`, `toExponential`, `toPrecision` | Exact digit generation with the specification's tie rule. Inputs are already-validated finite doubles and digit counts. |
| `StringToNumber` | The specification's grammar. **Correctly rounded for every literal form, radix strings beyond 2^53 included.** Both engines round such strings twice today. |
| `parseInt`, `parseFloat` | Correctly rounded; whitespace per the specification's set |
| Numeric literal values | Decimal, legacy octal, radix and separator-free forms. BigInt literal values take an explicit maximum width, from the caller's policy. |
| Array-index and canonical numeric keys | The specification's definitions |
| `StrWhiteSpaceChar` | The specification's set: the listed code points plus the `Zs` category, from a table generated from the pinned Unicode data. **Never** from the runtime's own Unicode data, which `JsNumberFormat.IsWhiteSpace` consults today. |

Every input-proportional operation takes the `WorkCharge` meter of
[SJL-D-10](../decisions.md#sjl-d-10), charged in documented blocks.

The library-level runner passes every number vector. A **reference-arm** comparison runs over the
predeclared input population. Its arms are test-only copies of both engines' pre-cut-over
implementations, kept in the home's test project. Every difference is classified, and each one is
expected to be a correction of one engine.

*Done when* the vectors pass, every reference-arm difference is classified, the `SL` rules and the
Native AOT probe pass, the API baseline is regenerated, and a preview package is on Broiler.VM's
committed feed ([SJL-D-4](../decisions.md#sjl-d-4) (c)).

### SJL-3.4 — Broiler.VM's cut-over

In one change:

1. **References.** `Broiler.VM.Profile.JavaScript` and `Broiler.VM.Profile.JavaScript.Compiler`
   reference the admitted package, and the allow-list names both projects. Lock files are updated.
2. **Adapters.** These call the library and keep coercion order, `RangeError`s, charging policy and
   string creation:
   - `JsRealm.Number`;
   - `JsRealm.Global` (`parseInt`, `parseFloat`);
   - `JsEngine.Conversions` (`ToNumber` from text);
   - `JsRealm.Json` (JSON number values);
   - `JsRealm.String` (`trim` uses the shared whitespace predicate);
   - the compiler's literal-value paths in `SliceTokenizer` and `JsParser`.
3. **Deleted.** Recorded in rule `I5`'s register:
   - from `JsNumberFormat`: `ToJsString`, `Decompose`, `Place`, `ToRadixString`, `RoundUpRadix`,
     `ToNumber`, `ParseInt`, `ParseFloat`, `IsWhiteSpace`, `Trim`, `IsDecimalLiteral`,
     `ParseRadix`, and their helpers;
   - the `JsRealm.Number` statics from `NumberFixedText` to `NumberIncrementFixed`, with their
     constants, and the overwritten placeholder `parseInt`/`parseFloat` functions;
   - `JsParser.NumberText`;
   - `JsParser.TryBigIntValue`;
   - the value loops of `SliceTokenizer.ReadNumericLiteral`. Its scanning and diagnostics stay.

   **The class `JsNumberFormat` is deleted if nothing remains in it.** Otherwise the record says what
   stays and why.
4. **The composition register.** `docs/compositions.md` gains its shared-library column (governance
   item V17). Every JavaScript root's row declares the algorithms assembly, and rule `I3` holds.
5. **Notices.** `THIRD_PARTY_NOTICES.md` gains the package's row.
6. **Profile records.** A JSD and JSC entries for:
   - `support.md` §2 ("implemented here rather than delegated to the platform" becomes "implemented
     in an admitted shared library, pinned by the shared vectors");
   - `release-gate.md` gates 8 and 12;
   - `packages.md`.
7. **Assurance.** The bundle's exclusions table names the code that left Broiler.VM's assurance
   coverage, numbered at filing time (EX-125 on this date).

*Done when* the change lands with every item above.

### SJL-3.5 — Broiler.JS's cut-over

In one change:

1. **References.** `Broiler.JavaScript.Parser`, `.Runtime` and `.BuiltIns` reference the library.
2. **The delegate seam goes.** The `JSValue.NumberToECMAString` delegate, and its installation in
   `BuiltInsAssemblyInitializer`, are deleted: `Runtime` references the library directly.
3. **Deleted:**
   - `Parser`'s `NumberCoercion` class;
   - `NumberParser`'s private algorithm members: `ParseCore`, `RefineEstimate`, `ParseHex`,
     `ParseOctal`, `ParseBinary`, `IsWhiteSpaceOrLineTerminator` and the rest named in the area's
     deletion list;
   - `JSNumber.ToECMAString` and `RoundTrippable`;
   - `JSNumberPrototype`'s kernels: `RoundScaled`, `ToExactFraction`, `ExactSignificantDigits`,
     `Compare10Pow`, `ShortestSignificantDigits`, `FormatExponential`;
   - the BigInt literal copies: `FastCompiler.CreateClass.BigIntLiteralToValue` and
     `ParseBigIntRadix`, and `JSBigInt.TryParseBigIntLiteral`;
   - one of the two duplicate `parseInt`/`parseFloat` wrapper pairs;
   - `JSString.trimCharacters`.
4. **Public types**, per [SJL-D-25](../decisions.md#sjl-d-25):
   - `NumberParser.CoerceToNumber`, `TryCoerceToUInt32` and `TryGetArrayIndex` become forwarders;
   - `JSNumberPrototype.DecimalToBase` and `DecimalToArbitrarySystem` become obsolete forwarders;
   - every disposition is recorded in `docs/public-api.md`.
5. **Corrections** are listed in Broiler.JS's changelog, each with its vector or test262 path. The
   list includes:
   - radix `0`, `null` and `NaN` handling;
   - `toPrecision`'s argument order and truncation;
   - `parseInt(null, radix)`;
   - the whitespace set (U+0085 and U+FEFF);
   - `01n`;
   - correctly rounded long radix strings;
   - the radix-fraction digit cap.
6. **Non-standard extensions stay in Broiler.JS** ([SJL-D-17](../decisions.md#sjl-d-17)): the `m`
   (`Decimal`) literal suffix, `Number.prototype.clz`, and the .NET format-string extension of
   `toLocaleString`. Where they need a literal value, a parameter of the shared lexer-value
   operation provides it.

*Done when* the change lands with every item above.

### SJL-3.6 — Evidence in both repositories

| Repository | Runs |
|---|---|
| Home (Broiler.JS) | Library-level vectors; the reference-arm comparison; `SL` rules; the Native AOT probe; package validation |
| Broiler.JS | Engine-level vectors; whole-suite test262 at the pin, base (SJL-0) against after, per path, under the predeclared rule; the pristine consumer; Broiler.JS's own measurement under its rule |
| Broiler.VM | Engine-level vectors. Whole-suite test262 in bytecode and native forms, base against after, per variant, under the predeclared rule, with **corrections as an admitted class, each listed**. The fuel-parity twins: one verdict at every ceiling tried. Exhaustion verdicts unchanged, or admitted under the "charge model changed by decision" class, each listed. The differential harness: no new undeclared finding against base; corrected declarations updated with reasons. Every JavaScript root published and run under JIT, trimming and Native AOT, with closures declaring the algorithms assembly. Nuspecs declaring it, under rule N37. The pristine JavaScript consumer restoring with the upstream unreachable from a local feed holding the shared package. Rules `I1` to `I7` on the real closure. Broiler.VM's own measurement under its rule. |

*Done when* every run is retained in its bundle.

### SJL-3.7 — The simplification check

The bundles retain the deletion check: rule `I5` in Broiler.VM, and the equivalent search in
Broiler.JS against the area's deletion list. Each adapter is reviewed against the area's adapter
definition (coercion, errors, charging, object creation, and no algorithm), and the review is
recorded. A search of each engine for a surviving intra-engine duplicate of the area is retained
too.

**These checks no longer decide [SJL-D-23](../decisions.md#sjl-d-23).** That decision was taken
before the pilot: continue. Its six criteria are exit clauses here instead: clauses 4 and 8 (the
deletion lists), 5 and 9 (conformance), 7 and 13 (each engine's measurement), 8 (no forbidden
assembly), and 14 (adapters and intra-engine duplicates). A failed criterion keeps SJL-3 `In
progress`. Reopening the decision on this evidence takes a new dated choice in decisions.md.

*Done when* the checks and the review are retained in both bundles.

*(Revised 2026-10-11, when SJL-D-23 was taken before the pilot. The work package was titled "The
simplification check, and decision SJL-D-23", and ended: "SJL-D-23 is then recorded: continue,
continue with conditions, or stop. *Done when* the decision is in the ledger.")*

---

## Deliverables

The algorithms library's first area; Broiler.VM's and Broiler.JS's cut-overs; the deletion records;
the composition-register column and rows; notices; the area-verdict JSD and its JSC entries;
Broiler.JS's public-API dispositions and changelog; both bundles, with the simplification check.

---

## Exit gate

**Home (Broiler.JS repository) clauses**

1. The library implements the area under the conventions, and SL1 to SL10 pass. The API baseline
   and package validation pass, and the Native AOT probe runs the vectors.
2. Every number vector passes at library level. The reference-arm comparison over the predeclared
   population shows every difference classified, and none unclassified.
3. A preview package is on Broiler.VM's committed feed, the source decision `SJL-D-4` (c) uses for
   an area in development.

**Broiler.JS clauses**

4. Broiler.JS references the library from `Parser`, `Runtime` and `BuiltIns`. Every member on its
   deletion list is absent, and every public disposition is applied as recorded.
5. Test262 at the pin, base against after, per path: **no path passing at base fails after**. Every
   improvement is listed.
6. Engine-level vectors pass, and every correction is listed in the changelog.
7. The pristine consumer and package checks pass. Broiler.JS's measurement meets its predeclared
   rule.

**Broiler.VM clauses**

8. The family references the library exactly as the allow-list says. Rules `I1` to `I7` pass on the
   real closures, and `I5` holds the deletion list.
9. Whole-suite test262 in both forms meets the predeclared rule. The fuel-parity twins give one
   verdict at every ceiling tried.
10. Every JavaScript root is published and run in three modes. Closures, register rows and nuspecs
    declare the algorithms assembly. The pristine JavaScript consumer restores from the local feed
    with the upstream unreachable.
11. Engine-level vectors pass, and the differential harness shows no new undeclared finding.
12. The composition register, notices, the JSD and its JSC entries, and the exclusion for code
    leaving assurance coverage are filed.
13. Broiler.VM's measurement meets its predeclared rule.

**Programme clause**

14. The simplification check of SJL-3.7 is retained in both bundles: each adapter's review finds no
    algorithm in it, and no intra-engine duplicate of the area survives in either engine.

*(Revised 2026-10-11, when SJL-D-23 was taken before the pilot. Clause 14 read "SJL-D-23 is
recorded." The decision's record now belongs to SJL-0, and its other criteria are clauses 4, 5, 7,
8, 9 and 13.)*

---

## Evidence bundles

- **Broiler.JS `docs/evidence/sjl-3-001/`:**
  - home: vectors, reference arms, `SL` rules, Native AOT, package validation;
  - engine: vectors, test262 comparison, pristine consumer, measurement;
  - the deletion check;
  - the decision-rule files.
- **Broiler.VM `docs/evidence/sjl-3-001/`:**
  - test262 in both forms with the comparison;
  - fuel-parity twins;
  - differential report;
  - closures in three modes;
  - nuspecs and the consumer restore;
  - rule runs;
  - measurement;
  - the deletion check;
  - `I7` identity in `manifest.json`;
  - the decision-rule files.

---

## Does not do

- **It moves no other area.** Integer conversions, Float16, Math and BigInt are SJL-4, whitespace
  `trim` beyond the shared predicate is SJL-5, and JSON number serialisation stays in each engine's
  JSON driver.
- **It changes no parser structure.** Only literal *values* come from the library; scanning,
  diagnostics and strict-mode checks stay in each front end until SJL-13 and SJL-14.
- **It does not make either engine's numbers faster or slower by design.** Its measurement exists to
  catch a regression, under a rule written first.
