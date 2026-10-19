# Milestone SJL-10 — Temporal core

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/temporal.md)

- **Objective.** Both engines take the Temporal core from `Broiler.ECMAScript.Algorithms`, and each
  engine's own copies are gone. The core comprises:
  - ISO date, time and date-time records and their limits;
  - balancing, regulation and addition;
  - duration arithmetic, comparison, rounding and totals, with exact fractions;
  - differences (`until` and `since`) over calendars and time zones through providers;
  - the RFC 9557 and ISO 8601 grammar, returning neutral records and error codes;
  - the formula calendars, plus the table calendars over SJL-8's data;
  - non-ISO month codes, eras and reference years;
  - serialization.

  Each engine keeps its Temporal objects, option reading in specification order, coercion,
  `relativeTo` resolution and errors.
- **Owner roles.** The Broiler.JS maintainer and the JavaScript profile owner. One person.
- **Repositories.** Broiler.JS, Broiler.VM.
- **Waits on.** SJL-6 (civil-day helpers) and SJL-8 (zones and calendar tables). The rounding of
  SJL-4.

**Why this area matters.**

- **Broiler.VM.** Most of its Temporal core already references no engine type. `JsTemporalCore` is
  engine-free, and `JsTemporalParser` uses its engine argument only to construct `RangeError`s.
- **Broiler.JS.** Its Temporal types each carry their own copies of the same helpers, and each type
  has its own regular-expression string parser that returns engine objects and throws.
- **The reference.** Broiler.VM retains a host-function-free dataset produced by the Temporal
  reference polyfill at the pinned proposal revision, with no declared divergence. It is the
  acceptance test for both engines.

---

## Work packages

### SJL-10.1 — Area verdict and comparison rules

As in SJL-4.1, one verdict for each of these sub-areas:

- records and limits;
- balancing;
- durations and rounding;
- differences;
- the grammar;
- calendars;
- serialization.

**The pinned revision.** The Temporal proposal revision Broiler.VM follows is
`tc39/proposal-temporal@e8cc03fc` (JSD-0053). Broiler.JS pins none today. The area pins one for both
engines, by recorded decision.

### SJL-10.2 — Vectors and reference datasets

The retained polyfill dataset runs in both engines (`src/tests/temporal/temporal.js` and its
companions). The corpus gains library-level vectors for:

- the grammar, covering every accept and refuse case in the area design's table;
- duration rounding in every mode and increment;
- balancing at the limits;
- the calendars at their table edges.

### SJL-10.3 — The implementation

- **Records and parsing.**
  - The records, the limits and the balancing come from `JsTemporalCore` as it stands.
  - The parser is Broiler.VM's linear grammar. Its `TryParse` returns records and a
    `TemporalParseError` code and needs no engine.
- **Duration mathematics.** Duration arithmetic, nudging, bubbling, totals and the relative rounding
  run over calendar and time-zone provider interfaces, with exact rationals.
- **Options.** The library holds the option **rules**: valid units, defaults and the difference
  settings. Each engine's adapter does the reading.
- **Calendars.** The formula calendars cover Gregorian and its era variants, Coptic and Ethiopic,
  Indian, the tabular Islamic calendars and Hebrew. The table-backed calendars run over SJL-8's
  provider. Month codes, eras and the proposal's reference-year table are included.
- **Serialization.**

### SJL-10.4 — Broiler.VM's cut-over

**Moved to the library:**

- `JsTemporalCore`, the parts SJL-6 and SJL-4 did not already take;
- the string formatting of `JsTemporal.Conversions`;
- the cores of `JsTemporal.Calendar`, `JsTemporal.CalendarNonIso` and `JsTemporal.Durations`;
- `JsTemporalParser`, behind the `Try` interface. Only a thin wrapper that raises errors stays;
- the rule half of `JsTemporal.Options`.

**Unchanged:** the realm files keep the objects, the spec-order reads and the errors.

*(Added 2026-10-18, at SJL-10.1, by
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
decision D9.)* **Broiler.VM's correction R6 is made here**, its only correction in the area. `new PlainDate`, `new
PlainDateTime` and `new PlainMonthDay`'s reference year end the invocation on a year above 2^53, because the shared
`IsoCalendarMath.EpochDays` precondition throws before the limit check. The cut-over checks the limits first, through
the library, whose limit checks take any year and answer a status, so each constructor throws the `RangeError` the
specification asks for.

### SJL-10.5 — Broiler.JS's cut-over

**Deleted:**

- the per-type validity, regulation, addition and balancing copies;
- the per-type formatters;
- `TemporalIsoString`'s helpers. Its coercing members become adapter code, and its dead
  `RejectIncompatibleStyle` is removed;
- the per-type regular-expression parsers;
- the separate relative-rounding implementations;
- `JSTemporalRoundingOptions`' arithmetic;
- `TemporalNonIso`'s calendar parts;
- the other calendar copies: the formula calendars, `TemporalArithmeticCalendar`; the arithmetic of
  `TemporalTableCalendar` over SJL-8's years; their dispatch, `TemporalCalendarMath`; the Gregorian family's
  identifiers, years and era tables in `JSTemporalCalendar.cs`; and the five per-type month-code copies.

*(Revised 2026-10-18, at SJL-10.1, before any evidence of SJL-10 existed, by
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
decision D3: SJL-10 takes the formula calendars into the library, so the list reaches Broiler.JS's other calendar
copies, two of which JSD-0076's X6 kept "for the formula calendars". The list's last item read "`TemporalNonIso`'s
calendar parts.", and no item followed it.)*

**Kept:** the public `JSTemporal*` types stay as shells
([SJL-D-25](../decisions.md#sjl-d-25)).

**Corrections, listed:**

- lenient grammar acceptances;
- integer narrowing that wraps instead of throwing a `RangeError`;
- `Duration.prototype.add` with calendar units throwing "not yet implemented";
- rounding options ignored in non-ISO differences;
- day rounding that ignores daylight-saving changes;
- invalid month codes silently constrained;
- reference years computed without the proposal's table.

*(Added 2026-10-18, at SJL-10.1, by JSD-0078.)* The correspondence confirms each of these, two in part: day rounding
that ignores daylight saving holds for `until` and `since` only (F7), and the reference years answer as the
proposal's table does, so the table replaces the search with no answer changed (C10). The lenient acceptances are
wider than the area design named (G3, G4, G6, G8, G9, G11 to G13, G16 to G20, G22), and the narrowing also saturates
an ISO month-day's year past 2^31 (C12). Further corrections are listed with them, as the verdict classifies them:
- a year-month subtraction at the lower limit (B5);
- `compare`'s range check (D4);
- rounding to months through a clamped difference (D6, D7);
- the nudge after a shifted window, where the pinned text rules over its polyfill (D8, decision D7);
- a `relativeTo` in another calendar (D11);
- the calendar aliases `gregorian` and `minguo` (C2, decision D5);
- month codes with digits other than ASCII (C6);
- era names of earlier drafts (C9, decision D4);
- the calendar a `PlainMonthDay` lends (C13);
- the three conversions that lose a calendar's day, calendar or year (C15 to C17).

Code no input reaches is deleted (G10, C3, C10, S8, and row D12's shift of years and months and fourth day
correction). Row D12's day window and its zoned branch are reached, and decisions D11 and D12 answer a `RangeError`
there.

*(Revised 2026-10-19, at SJL-10.5, by
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
section "SJL-10.5's cut-over, read against this record". Row D12 is reached: a zone's skipped day reaches its day
window, which the owner's decision D11 answers, and its zoned branch, the "inconsistent Instants" test of
NudgeToZonedTime's day span, which the owner's decision D12 answers. And Broiler.JS's records say that much of the code
no input reaches lived in files that still exist, and was deleted from them, not with its file. The sentence read
"Code no input reaches is deleted with its file (D12, G10, C3, C10, S8).")*

### SJL-10.6 — Evidence

The same as SJL-3.6, plus two items:

- the polyfill dataset in both engines;
- test262 `built-ins/Temporal` and `intl402/Temporal` in both engines.

---

## Exit gate

**Home clauses.** Every sub-area not refused is implemented. Its vectors pass, and the polyfill
dataset passes at library level wherever it can run there.

**Broiler.JS clauses.**

- The deletion list is absent.
- test262 Temporal per path meets the area rule.
- The polyfill dataset meets its rule.
- Every correction is listed.

**Broiler.VM clauses.**

- Rules `I1` to `I7` hold.
- test262 in both forms and the polyfill dataset meet the area rule. For Broiler.VM that is
  unchanged.
- Every root is published and run in three modes.

**Records.**

- The area-verdict JSD.
- Broiler.JS's record, including the proposal pin it now follows.

---

## Evidence bundles

`docs/evidence/sjl-10-001/` in each repository.

---

## Does not do

- **It does not implement Temporal in either engine where it is absent.** Both engines have it.
- **It does not change `Temporal.Now`'s clock policy.** Clock resolution and the system zone stay
  each engine's provider ([SJL-D-19](../decisions.md#sjl-d-19)).
- **It does not share Intl formatting of Temporal objects.** That belongs to SJL-9.
