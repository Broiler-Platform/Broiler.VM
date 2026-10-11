# Area: time-zone and calendar data

**Delivered by:** [SJL-8](../milestones/sjl-8-time-zones-and-calendars.md). **Libraries:** the
time-data generator and pins in the Broiler.JS repository (decision
[SJL-D-1](../decisions.md#sjl-d-1) (b-iii)), whose tables each engine embeds (decision
[SJL-D-6](../decisions.md#sjl-d-6) (c)); and the zone algorithms and the table decoder in
`Broiler.ECMAScript.Algorithms`. The identity `Broiler.ECMAScript.TimeData` is reserved for a data
package that a later decision may publish. **Status:** design.

---

## 1. Scope

| Part | Shared | Stays in the engines |
|---|---|---|
| tz database | One pinned release, compiled reproducibly into one table format; identifiers, primaries, regions | Which roots carry it (optional data) |
| Zone algorithms | Offset at an instant; possible epoch nanoseconds for a local time; disambiguation; start of day; next and previous transitions; identifier lookup and primary resolution | Mapping statuses to errors |
| Calendar tables | Chinese, Korean and Qing year tables; Umm al-Qura; Persian corrections | (none) |
| Display names of zones | (none here: CLDR data, SJL-9) | (none) |

---

## 2. Current state: the datasets are not equivalent

The feasibility report said that equivalence of the two datasets "was not established". **It is now
established that they are not equivalent:**

| Dimension | Broiler.VM | Broiler.JS |
|---|---|---|
| Release | **2026e** | **2025b** |
| Pinned | `src/tests/tzdb/pins/tzdb.pin`: archive hashes, member hashes; rules N29 and N30, which have tests but no register row | Not pinned. Copied from the build host's `tzdata.zi` and compiled zoneinfo by `scripts/tzdata/generate_tzdata.py`; not reproducible across hosts; committed once |
| Built by | Its own compiler (`TzdbCompiler`), checked against `zic` output across its validated range | The host's `zic` output, read as TZif |
| Backzone | Excluded (the default build) | Mostly included, so pre-1970 histories differ widely (for example Amsterdam and Reykjavik) |
| Beyond the last listed transition | Recurring rules are evaluated for every later year | **The TZif footer is ignored, so the last offset holds forever.** On 2040-07-01, America/New_York reads −05:00 and Europe/Berlin +01:00, where Broiler.VM reads −04:00 and +02:00. |
| Identifiers | Set A | Set A plus `Factory` |
| Primary identifiers | From CLDR's `bcp47/timezone.json` (`Etc/UTC`, `Etc/GMT` and `GMT` resolve to `UTC`) | The tz database's link targets, plus hard-coded legacy aliases and a UTC-family rule; several names differ in which is primary (for example `Europe/Bratislava`) |
| `Intl.DateTimeFormat`'s `timeZone` | Resolved to the primary; the `canonical-tz` test262 cases are skipped | Keeps the link name |
| Lookup | ASCII case-insensitive; non-ASCII refused | Invariant upper-casing |
| Algorithms | `PossibleEpochNs` samples the offset at the instant and a day either side, keeping candidates that round-trip; `StartOfDay` fails with `RangeError` if no instant exists | Two samples; an unknown named zone silently reads offset zero; `StartOfDay` falls back to local midnight; "next transition" is `null` after 2037 |
| Format | Compact: varint transitions, an offset palette per zone, recurring rules; decoded lazily per zone | A DEFLATE-compressed blob of TZif files, inflated in full on first use |
| Optional | Yes: in the `.Intl` assembly, handed in by the composition roots that carry Intl | No: embedded in `BuiltIns`, always shipped |

**Calendars:**

| Calendar | Broiler.VM | Broiler.JS |
|---|---|---|
| Gregorian, Buddhist, ROC, Japanese; Coptic, Ethiopic; Indian; tabular Islamic; Hebrew | Formulas | The same formulas *(reading)* |
| Umm al-Qura | ICU4X tables | The CLDR package's table, **identical** over its range |
| Chinese, Korean | ICU4X's published years, with ICU4X's approximation outside them | .NET's lunisolar calendars in their range; outside it, the CLDR package's astronomical computation with an unbounded cache |
| Persian | A 33-year rule with corrections from `calendrical_calculations` 0.2.4 | .NET's `PersianCalendar`; equivalence not established |
| Pinned | `src/tests/calendars/pins/calendars.pin` (ICU4X `icu_calendar` 2.3.0, `calendrical_calculations` 0.2.4); rules N31 and N32 | Not pinned |

---

## 3. The target

### 3.1 The data home

- **Inputs:** the pinned tz archive and the pinned calendar crates, with their hashes, in the
  shapes of Broiler.VM's pin files.
- **Generators:** Broiler.VM's tz compiler, its table generators and its calendar table generator,
  moved to the Broiler.JS repository. Each engine regenerates its embedded tables with them, byte
  for byte (rule SL7 in the home; N30 and N32 in Broiler.VM), under
  [SJL-D-6](../decisions.md#sjl-d-6) (c).
- **Version constants:** `TzdbRelease` and the calendar sources.
- **The decoder** of the table format, in the algorithms library, implementing the interfaces in
  3.2 over the bytes an engine hands it. No package carries the tables.
- **The policy of [SJL-D-20](../decisions.md#sjl-d-20)**, taken on 2026-10-11 and applied by the
  generator:
  - the newest release both engines can adopt together, pinned by a dated record at SJL-8.1;
  - no backzone (the default build);
  - extrapolation by recurring rules;
  - CLDR primaries;
  - no `Factory`;
  - ASCII case-insensitive lookup.

### 3.2 The interfaces and algorithms

```csharp
namespace Broiler.ECMAScript.Time;

public interface ITimeZoneDatabase
{
    string Release { get; }
    bool TryFind(ReadOnlySpan<char> identifier, out TimeZoneId id);      // policy's case rule
    TimeZoneId Primary(TimeZoneId id);
    IReadOnlyList<string> PrimaryIdentifiers { get; }
    ITimeZoneRules Rules(TimeZoneId id);
}
public interface ITimeZoneRules
{
    int OffsetSecondsAt(long epochSeconds);
    bool IsDaylightAt(long epochSeconds);
    long? NextTransition(long afterEpochSeconds, long limit);
    long? PreviousTransition(long beforeEpochSeconds);
}
public static class ZonedTime
{
    public static int PossibleEpochNanoseconds(ITimeZoneRules rules, IsoDateTime local, Span<BigInteger> destination);
    public static DisambiguationStatus Disambiguate(ITimeZoneRules rules, IsoDateTime local, Disambiguation mode, out BigInteger epochNs);
    public static bool TryStartOfDay(ITimeZoneRules rules, IsoDate date, out BigInteger epochNs);
}
public interface ICalendarTables { /* the year tables for the table-backed calendars */ }
```

### 3.3 Optionality

Broiler.VM keeps its shape. The time data reaches the profile only through the `.Intl` assembly's
adapter, which only some composition roots hand in, and rule `I3` holds it.

Broiler.JS drops its always-embedded blob. It can make the data optional when its satellite plan
(A-2) lands. Until then it embeds the generated tables in `BuiltIns`, as it embeds the blob today.

---

## 4. Deletion lists

**Broiler.VM** moves its generators to the data home: `TzdbCompiler`, `TzdbTableGenerator` and
`CalendarTableGenerator`, with the pin readers they use. The home starts from copies of
`src/tests/tzdb/pins/` and `src/tests/calendars/pins/`.

**Broiler.VM keeps** ([SJL-D-6](../decisions.md#sjl-d-6) (c)) its pins, the embedded `JsTzdbTables`
and `JsCalendarTables`, and the rule tests that hold them, which now regenerate with the home's
generator.

It deletes the following, which are replaced by the shared decoder and algorithms:

- `JsTimeZones`;
- the algorithm bodies in `JsTemporal.Zones`;
- the core of `ResolveTimeZone` in `JsRealm.DateTimeFormat`, its `EtcGmtZones`, and its offset
  parser.

**Broiler.JS** deletes:

- `Temporal/Tz/IanaTimeZone`, `IanaTimeZoneDatabase`, `TzifReader` and `tzdata.bin`, with its
  `EmbeddedResource` item;
- `scripts/tzdata/generate_tzdata.py`;
- `JSTemporalZonedDateTime`'s zone helpers;
- `JSIntl`'s offset-zone normalisation;
- the date-time formatting engine's zone helpers;
- the .NET-calendar classes and the CLDR package's calendar uses.

---

## 5. Acceptance

The reference datasets Broiler.VM retains run in both engines: `src/tests/cldr/timezones/` and
`src/tests/temporal/`. Their programs use no host functions. Their zones were chosen where two
tz releases agree, and their declared divergences come with them.

Per-zone transition vectors cover the dates around every policy difference listed in section 2.
**Every difference Broiler.JS sees is listed as a correction, or as a policy change under
`SJL-D-20`**: the release, backzone, extrapolation, primaries, `Factory` and the lookup rule.
