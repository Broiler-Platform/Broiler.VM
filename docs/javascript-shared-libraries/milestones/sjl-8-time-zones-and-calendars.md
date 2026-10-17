# Milestone SJL-8 — Time-zone and calendar data

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/time-zones-and-calendars.md)

- **Objective.** Both engines read one pinned time-zone dataset and one set of calendar tables, both
  generated reproducibly by one generator in their home, the Broiler.JS repository. Each engine
  embeds the generated tables itself. The time-zone algorithms over the dataset, and the decoder of
  its table format, come from `Broiler.ECMAScript.Algorithms`. Every **optional** data stays
  optional where each engine makes it optional today. The time-zone algorithms are
  possible-epoch-nanoseconds, disambiguation, start of day and transitions.
- **Owner roles.**
  - The Broiler.JS maintainer for the data, whose home is the Broiler.JS repository
    ([SJL-D-1](../decisions.md#sjl-d-1) (b-iii)).
  - The JavaScript profile owner and the Broiler.JS maintainer for the engines.
  - All one person.
- **Repositories.** Broiler.JS (the data home, and an engine) and Broiler.VM.
- **Waits on.** SJL-3. It applies three decisions taken on 2026-10-11:
  - [SJL-D-6](../decisions.md#sjl-d-6) (c): a shared generator and pins, with tables embedded by
    each engine;
  - [SJL-D-20](../decisions.md#sjl-d-20): Broiler.VM's dataset policy, at the newest release both
    engines can adopt;
  - [SJL-D-21](../decisions.md#sjl-d-21): ICU4X's calendar sources, and no .NET calendar class.

**The two datasets differ today:**

| | Broiler.VM | Broiler.JS |
|---|---|---|
| tz database release | 2026e | 2025b |
| How the data is built | Compiled in the repository from a pinned, hashed archive by its own compiler | Copied from a build host's compiled zoneinfo, with no pin |
| Backzone | Excluded | Mostly included |
| After the last listed transition | Recurring rules are extrapolated | The last offset is frozen. This is wrong after 2037: for example, America/New_York in July 2040 is −05:00 in Broiler.JS and −04:00 in Broiler.VM. |
| Primary identifiers | From CLDR | tz database link targets |
| `Factory` zone | Absent | Present |

The calendars differ too. Broiler.VM uses ICU4X's published tables and `calendrical_calculations`
corrections. Broiler.JS uses .NET's lunisolar and Persian calendars and an astronomical computation.
**Equivalence is reached by sharing one dataset and one policy, never by declaring the current
datasets equal.**

---

## Work packages

### SJL-8.1 — Area verdict, decisions and comparison rules

1. Decisions [SJL-D-20](../decisions.md#sjl-d-20) and [SJL-D-21](../decisions.md#sjl-d-21), taken on
   2026-10-11, are recorded, and the release both engines adopt is pinned by a dated record.
2. The extraction gate is applied to the time-zone data, the time-zone algorithms and the calendar
   tables.
3. The comparison rules are committed. They name, as an admitted class, **every difference the
   chosen policy makes** for Broiler.JS: the release change, backzone, extrapolation and primary
   identifiers. Each is listed.

### SJL-8.2 — The data home

The data home holds:

- **The tz database.** The pinned tz archive, with archive and member hashes in the shape of
  `src/tests/tzdb/pins/tzdb.pin`. Broiler.VM's tz compiler and table generator, moved or ported. The
  compiler is checked against `zic` output across its validated range.
- **The table format.** Broiler.VM's compact format: varint transitions, an offset palette per zone,
  recurring rules, and the identifier table with primaries and regions.
- **The calendar tables.** The pinned ICU4X crates, `icu_calendar` and `calendrical_calculations`,
  and Broiler.VM's calendar table generator.
- **Generation rules.** Byte-for-byte regeneration (SL7) and public version constants.

**No package carries the tables** ([SJL-D-6](../decisions.md#sjl-d-6) (c)). Each engine embeds what
the generator produces. The decoder of the table format, which implements the algorithms library's
provider interfaces over the bytes an engine hands it, is shared code in
`Broiler.ECMAScript.Algorithms`.

### SJL-8.3 — The algorithms

The algorithms library gains the time-zone database and rules interfaces. Over them, it gains:

- possible epoch nanoseconds, by the specification's sampling;
- disambiguation for every mode;
- start of day, **which fails with a status when no instant exists**. It never falls back to local
  midnight, as Broiler.JS does today;
- next and previous transitions, **without a 2037 cliff**;
- identifier lookup under the policy's case rule, and primary-identifier resolution.

The calendar arithmetic over the tables (Chinese, Korean, Umm al-Qura, Persian) joins the formula
calendars that SJL-10 owns.

### SJL-8.4 — Vectors and reference datasets

The corpus gains transition and offset vectors for the zones whose history the policy fixes, at
dates on both sides of every listed policy difference. Broiler.VM's retained acceptance programs use
no host functions, so they run in **both** engines as reference datasets:
`src/tests/cldr/timezones/timezones.js` and `src/tests/temporal/temporal.js`, with their declared
divergences.

### SJL-8.5 — Broiler.VM's cut-over

**Moved to the data home:** the tz compiler and table generator, and the calendar table generator,
with the pin readers they use. The home starts from copies of Broiler.VM's tz and calendar pins.

**Kept and regenerated** ([SJL-D-6](../decisions.md#sjl-d-6) (c)): the `JsTzdbTables` and
`JsCalendarTables` tables, which the home's generator now produces from Broiler.VM's pins, embedded
in the `.Intl` assembly as today; the pins; and the rule tests that hold the tables byte for byte.

**Deleted:**

- `JsTimeZones`, and `JsCalendars`' table-backed parts, replaced by the shared decoder and
  algorithms;
- the time-zone algorithm bodies of `JsTemporal.Zones`;
- the core of `ResolveTimeZone` and its offset parsing in `JsRealm.DateTimeFormat`.

Then:

- The `.Intl` assembly hands its tz and calendar tables to the shared code through the provider.
  Optionality is unchanged: only roots that hand in Intl data carry them.
- Rules N29 to N32 stay in Broiler.VM, now regenerating with the home's generator, and rule `I4`
  holds Broiler.VM's pins equal to the home's. **They gain the register rows they lack today.**

### SJL-8.6 — Broiler.JS's cut-over

**Deleted:**

- `Temporal/Tz/IanaTimeZone`, `IanaTimeZoneDatabase` and `TzifReader`;
- the embedded `tzdata.bin` and its `EmbeddedResource` item;
- `scripts/tzdata/generate_tzdata.py`;
- `JSTemporalZonedDateTime`'s offset, candidate, disambiguation, start-of-day and transition
  helpers;
- the calendar classes that use .NET calendars or computations: `TemporalLunisolarCalendar`,
  `TemporalSolarHijriCalendar`, `TemporalArithmeticCalendar` and `TemporalCalendarMath`;
- the uses of the CLDR package's `CldrChineseCalendar` and `CldrUmmAlQura`.

**Embedded instead:** the tables the home's generator produces, in `BuiltIns`, until Broiler.JS's
satellite plan (A-2) makes them optional ([SJL-D-6](../decisions.md#sjl-d-6) (c)).

**The `Date` provider.** Broiler.JS's `Date` local-time provider may use the shared data for its
host zone ([SJL-D-19](../decisions.md#sjl-d-19)).

**Corrections, listed:**

- the 2037 cliff;
- the release change;
- backzone;
- primaries;
- `Factory`;
- start-of-day fallback;
- unknown zones silently reading as offset zero;
- the calendar years that change source.

*(Added 2026-10-17, at SJL-8.1, by JSD-0076.)* Two of these delete code that no input reaches: every entry point
validates a name before reading it, and no gap lacks a following transition (the correspondence's A6 and A11).
Five more corrections are listed with them, as the verdict classifies them: lookup by ASCII case only (Z10), no
ISO string as the formatter's zone (Z11), the region lists (Z12), non-ASCII offset digits (A13) and an unvalidated
host zone (A16). The calendar years that change source are the policy changes K4 to K6 and K11, beside three
corrections: `chinese` 2057 (K2), the far lunisolar dates (K7) and the Persian seams at .NET's bounds (K12).

### SJL-8.7 — Evidence

As in SJL-3.6, plus the reference datasets in both engines. **Broiler.VM's Intl data budget is
restated, by citation, in the bundle.**

---

## Exit gate

**Data-home clauses.**

1. The pinned archive and crates are present.
2. Generation is byte-identical.
3. The `zic` comparison passes across the validated range.
4. Public versions are exposed.
5. The provider implementation is in place.
6. The `SL` rules pass.

**Broiler.JS clauses.**

7. The deletion list is absent.
8. No host zone or .NET calendar is used by `Temporal`. `Date` follows the provider decision.
9. test262 for Temporal and Intl time zones meets the area rule.
10. The reference datasets meet their rule.
11. Every correction is listed.

**Broiler.VM clauses.**

12. Rules `I1` to `I7` hold, with `I4` for the tz and calendar pins.
13. Rules N29 to N32 regenerate the tables with the home's generator from Broiler.VM's pins, rule
    `I4` holds those pins equal to the home's, and N29 to N32 are registered.
14. test262 in both forms meets the area rule.
15. The reference datasets are unchanged.
16. Every root is published and run in three modes.
17. Optionality is unchanged, as rule `I3` shows.

*(Revised 2026-10-11, when SJL-D-6 was taken as (c). Clause 13 read "Rules N29 to N32 are re-homed
and registered.")*

**Records.** The area-verdict JSD, amending JSD-0053 and JSD-0056 where they move and recording
decisions `SJL-D-20` and `SJL-D-21`, and Broiler.JS's record of the `tzdata.bin` retirement.

---

## Evidence bundles

`docs/evidence/sjl-8-001/` in Broiler.JS, for the data home and the engine, and in Broiler.VM.

---

## Does not do

- **It does not change either engine's `Date` local-time policy.**
- **It does not share zone *display names*.** Those are CLDR data, and belong to SJL-9.
- **It does not implement Temporal's arithmetic over calendars.** That is SJL-10.
