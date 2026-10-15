# Milestone SJL-6 — Date arithmetic, the Date string grammar and Date formatting

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/date.md)

- **Objective.** Both engines take the following from `Broiler.ECMAScript.Algorithms`, and each
  engine's own copies are gone:
  - the ECMAScript time-value arithmetic: `MakeTime`, `MakeDay`, `MakeDate`, `TimeClip` and the
    field extractors;
  - the shared proleptic Gregorian civil-day helpers, which Temporal and the time-zone code consume
    later;
  - the specification's Date string format, and Broiler.VM's implementation-defined fallback
    grammar beside a strict dialect, chosen by a dialect parameter. Broiler.JS's culture-dependent
    fallback stays in Broiler.JS ([SJL-D-17](../decisions.md#sjl-d-17));
  - the Date string renderings.

  Local time comes from a **provider each engine supplies**: Broiler.VM keeps UTC, and Broiler.JS
  keeps its host time zone.
- **Owner roles.** The Broiler.JS maintainer and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS, Broiler.VM.
- **Waits on.** SJL-3. It applies decision [SJL-D-19](../decisions.md#sjl-d-19), taken on
  2026-10-11: host policy as providers, each engine supplying its own.

---

## Work packages

### SJL-6.0 — Broiler.VM's hosted test262 evidence workflow

*(Added 2026-10-15, at the owner's direction, during SJL-5.6 and before any work of SJL-6. It comes
before SJL-6.1, so that SJL-6's rule can name it, and it changes no product code, so it may start
before SJL-3 is met.)*

Broiler.JS takes its whole-suite test262 runs for a bundle on GitHub's hosted runners, through its
`test262-evidence.yml`. Broiler.VM takes both forms on one local machine, and an area's after arm
holds that machine for the length of its runs. Broiler.VM gains a workflow that takes the runs its
bundles' part A asks for in the same way:

- **a dispatch input naming the commit to test**, and both forms, bytecode and native, in one dispatch;
- **the procedure the bundles' rules name**: `eng/run-test262.py` at the pin, with the allowances and
  floors `eng/collect-js-release-candidate.py` fixes, run per shard and merged by the harness; or, if the
  existing `test262-javascript-profile.yml` sharding is kept, a shown equivalence between its merged
  report and that procedure's;
- **every file the bundle collectors retain from a run**: the report, the run, merge and floor logs,
  the JSON report and the failure manifest;
- **the conformance binary's digest on every shard**, so that a run which mixed builds is visible;
- **the runner configuration recorded with the run**: the image, the processor count and the native
  backend;
- **a read-only token**: it writes nothing, opens nothing and commits nothing. A bundle imports its
  artifact unchanged, as Broiler.JS's bundles import theirs.

It does not take class C4's re-runs, (c') and (e): each runs one row alone at both commits on one
machine, and the bundle's rule says where. The wall clock differs between machines, so a hosted run is
compared with another hosted run, never with a local one, except in the check below, whose differences
are classified rather than judged.

*Done when* the workflow has run once in both forms at a commit already run locally, the two runs are
compared per variant through `eng/compare-test262-forms.py --same-form` with every difference
classified, and the comparison is retained in bundle `sjl-6-001`; and SJL-6.1's decision rule names the
workflow for both its base and its after runs.

*(2026-10-15: the workflow and its driver, `eng/test262-evidence.py`, landed in `9c3d809`
(Broiler-Platform/Broiler.VM#128), with bundle [`sjl-6-001`](../../evidence/sjl-6-001/README.md)'s rule. Its
first run, at bundle `sjl-5-001`'s after commit with both forms, meets that rule: each hosted run is whole
and retainable from one build, and every difference from the same commit's local runs is a wall-clock
race between the machines, which the rule admits. What remains of this done-when is SJL-6.1's rule naming
the workflow.)*

### SJL-6.1 — Area verdict and comparison rules

Applied as in SJL-4.1, with one verdict for each of the four sub-areas: time arithmetic, civil-day
helpers, the parse grammar, and the renderings.

### SJL-6.2 — Vectors

The corpus gains:

- **Time arithmetic:** `MakeTime` with fractional and non-finite components, `MakeDay` across the
  representable range, and `TimeClip` at ±8.64e15. `TimeClip` must turn −0 into +0.
- **Field extraction:** before 1970, around leap days, and at the range ends.
- **The ISO format:**
  - every allowed omission;
  - expanded years, with `-000000` refused;
  - exactly three fraction digits;
  - `24:00`;
  - offsets;
  - **date-only forms read as UTC.**
- **Each engine's dialect:**
  - Broiler.VM's textual forms, which are its own `toString` and `toUTCString` outputs;
  - Broiler.JS's legacy forms, as **implementation-defined** vectors with an expectation per engine.
- **Renderings:** `toString`, `toDateString`, `toTimeString`, `toUTCString` and `toISOString`, at the
  range ends and for negative years.

### SJL-6.3 — The implementation

The algorithms library gains:

- `EcmaTime`, the arithmetic over doubles;
- `IsoCalendarMath`, the civil-day helpers over longs: leap years, month lengths, epoch days in both
  directions, weekday, day of year and ISO week;
- a local-time provider interface, with the specification's disambiguation of skipped and repeated
  local times;
- `EcmaDateParser`, taking the dialect, an optional meter, and a result that says whether the value
  is UTC, local, or invalid;
- `EcmaDateFormatter`, whose zone text comes from the provider.

### SJL-6.4 — Broiler.VM's cut-over

**Deleted:**

- from `JsRealm.Date`, the `Date*` arithmetic members, from `DateFloorMod` to `DateTimeClip`, and
  their constants;
- `DateParseTextual` and its helpers. `DateParseText` keeps the charge and becomes a call;
- the rendering helpers;
- the civil-day copies: `JsTemporalCore`'s calendar members, `JsTimeZones`' `DaysFromCivil`,
  `MonthLength` and `Weekday`, and `JsLocalTime.From` in the date-time formatter.

**Unchanged:** the provider Broiler.VM passes is UTC, under JSD-0053 §7.

### SJL-6.5 — Broiler.JS's cut-over

**Deleted:**

- `JSDateMath`'s arithmetic;
- `JSDateStatic`'s `DaysInMonth` and `TryParseExtendedIso`;
- the leap-year, civil-day and week-helper copies in every Temporal type.

**`DateParser` (public)** keeps its type. Its standard grammars forward to the library
([SJL-D-25](../decisions.md#sjl-d-25) (a)), and its .NET-culture fallback stays in it as Broiler.JS's
dialect ([SJL-D-17](../decisions.md#sjl-d-17)).

**Date storage.** Broiler.JS stores a `DateTimeOffset` plus a raw double today. Moving to a single
time value as the source of truth is part of this cut-over, because otherwise the getters and
setters cannot use the shared arithmetic outside 0001 to 9999.

**The local-time provider** wraps the host `TimeZoneInfo`. Alternatively, once SJL-8 lands, it wraps
the shared IANA data. Either way, Broiler.JS's `Date` and its `Temporal` stop reading different
time-zone data.

**Corrections, listed:**

- date-only ISO strings were read as local time;
- `TimeClip` kept −0;
- `MakeTime` did not truncate its arguments;
- setters kept fractional arguments;
- setters returned `NaN` for valid dates outside 0001 to 9999;
- the offset base mixed DST-aware and DST-unaware values;
- out-of-range dates took the current season's offset.

### SJL-6.6 — Evidence

As SJL-3.6, for this area. Broiler.JS's test suite pins `TZ=UTC` and the invariant culture today.
**The area's Broiler.JS evidence also runs under a non-UTC host zone with DST**, so that the
host-zone provider path is exercised, and its rule says so.

---

## Exit gate

**Home clauses.** Every sub-area that was not refused is implemented under the conventions. Its
vectors and reference arms pass under the area rule. The `SL` rules, the API baseline and package
validation pass, and the Native AOT probe runs.

**Broiler.JS clauses.**

1. The deletion list is absent.
2. Date storage is migrated.
3. test262 for `Date` and its `Temporal` interop meets the area rule, per path.
4. The non-UTC host run meets its rule.
5. Every correction is listed.

**Broiler.VM clauses.**

6. Rules `I1` to `I7` hold, and `I5` holds the deletion list.
7. test262 in both forms meets the area rule.
8. Every root is published and run in three modes.
9. Engine-level vectors pass.

*(Corrected 2026-10-11: these clauses were numbered 1 to 4, repeating the Broiler.JS clauses'
numbers. Their text is unchanged.)*

**Records.** The area-verdict JSD and Broiler.JS record, and decision `SJL-D-19`.

---

## Evidence bundles

`docs/evidence/sjl-6-001/` in each repository.

---

## Does not do

- **It does not choose a time-zone dataset.** That is SJL-8.
- **It does not change Broiler.VM's UTC local time** or Broiler.JS's host-zone policy.
- **It does not share `toLocale*String`.** Those go through Intl, which is SJL-9.
