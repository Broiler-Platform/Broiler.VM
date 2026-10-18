# Area: Temporal core

**Delivered by:** [SJL-10](../milestones/sjl-10-temporal.md). **Library:**
`Broiler.ECMAScript.Algorithms`. **Status:** design. Observations marked *(reading)* are to be
confirmed by vectors or by the reference dataset.

---

## 1. Scope

| Sub-area | Shared | Stays in the engines |
|---|---|---|
| Records and limits | ISO date, time and date-time records; duration records with an exact time part; every limit check | The Temporal objects and their slots |
| Balancing | `BalanceISODate`, `BalanceTime`, `RegulateISODate`, `RegulateTime`, `AddISODate`, `AddTime` | The `overflow` option's reading |
| Durations | Validity, sign, default largest unit, internal conversion, addition, comparison, rounding (with nudging and bubbling), totals with exact fractions | `relativeTo` resolution: it reads properties and coerces |
| Differences | `until`/`since` for every type, through calendar and time-zone provider interfaces | `ToTemporal*` conversions, equality checks |
| Grammar | ISO 8601 and RFC 9557 parsing into records and error codes; duration strings | String coercion, error mapping |
| Options | The **rules**: allowed values, defaults, unit validation, difference settings | The **reading**, with `Get` and coercion in specification order |
| Calendars | Formula calendars (Gregorian and its era variants, Coptic, Ethiopic, Indian, tabular Islamic, Hebrew); table-backed calendars over SJL-8's data; month codes; eras; reference years | Calendar identifiers as engine-visible strings, through the shared canonicalisation |
| Serialization | Every Temporal string form | (none) |
| `Temporal.Now` | (none) | The clock and system zone (decision [SJL-D-19](../decisions.md#sjl-d-19)) |

---

## 2. Current state

### Broiler.VM: largely engine-free already

| Part | Where | Coupling |
|---|---|---|
| Records, limits, calendar math, balancing, rounding, duration helpers, formatting | `JsTemporalCore` | **None**: no engine parameter, no throw, no charge |
| Parser | `JsTemporalParser.Parse(JsEngine engine, string text, params TemporalGoal[] goals)` | The engine is used **only** to construct `RangeError`s, at a handful of sites. Input is a CLR `string`; output a neutral record. Its other entry points take no engine. The move is mechanical: a `TryParse` with an error code, a few call sites, and `ParseCalendarString`, which catches `JsThrow` today. |
| Calendars | `JsTemporal.Calendar`, `JsTemporal.CalendarNonIso`, and `JsCalendars` (which has no engine coupling) | Errors and property reads in the realm-facing parts |
| Durations | `JsTemporal.Durations` | Errors, calendar callbacks and zone callbacks; only `ToPartialDuration`, `ToDuration` and `RelativeToOption` read properties |
| Options | `JsTemporal.Options` | Reads properties, in specification order |
| Metering | none in any Temporal file; only the generic per-call charge applies | (none) |

**Pins and reference data:**

- Both engines follow `tc39/proposal-temporal@e8cc03fc`: Broiler.VM since JSD-0053, and Broiler.JS by
  [JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md)'s
  decision D1, which also has the area's rules run test262 `ccaac100` *(Revised 2026-10-18, at SJL-10.1, by JSD-0078;
  read "Broiler.VM pins `tc39/proposal-temporal@e8cc03fc` (JSD-0053).")*.
- It retains a host-function-free dataset produced by the reference polyfill at that revision, with
  no declared divergence: `src/tests/temporal/`.

### Broiler.JS: spread across the object classes

**State lives in fields of the `JSTemporal*` `JSObject` subclasses.** The helpers are copied per
type:

- **leap year** in every Temporal type, in `TemporalIsoString` and in the Date code;
- **civil-day conversion** in each type and in two calendar classes;
- **ISO validity** in each type;
- **unsigned rounding** in several places;
- **`ReadOverflow`** in several types;
- **`ToIntegerWithTruncation`**, narrowed to `int`, in many places.

**The real string parsers are private per type:**

- `ParseTemporalInstant`;
- `ParseTemporalDateString`;
- `ParseTemporalDateTimeString`;
- `ParseTemporalTimeString`;
- `ParseTemporalYearMonthString`;
- `ParseTemporalMonthDayString`;
- `ParseZonedDateTimeString`, which **reads options part-way through the parse**;
- `ParseTemporalDurationString`.

They are built on regular expressions, return engine objects and throw. `TemporalIsoString` holds
the following:

- regular-expression helpers;
- validators that throw;
- members that perform an observable `ToPrimitive` and throw `TypeError`;
- a dead member.

Errors come from static delegates that find the realm ambiently. No proposal revision was pinned until SJL-10.1,
whose decision D1 has Broiler.JS follow `e8cc03fc`, as Broiler.VM does; its behaviour was that revision's on every
marker probed but the era names and `Duration.prototype.add`'s `relativeTo` *(Revised 2026-10-18, at SJL-10.1, by
JSD-0078; read "No proposal revision is pinned.")*.

---

## 3. Differences already visible

| Case | Broiler.VM | Broiler.JS *(reading)* |
|---|---|---|
| `PlainDate.from('2020-01-01[]')`, `PlainYearMonth.from('2020-02-30')`, `PlainTime.from('2021-13-45T12:00')` | `RangeError` (the grammar refuses them) | accepted |
| Bracketed annotations in the wrong order or malformed | refused | partly checked |
| Offset strings in `with()` | the grammar | accepts `"Z"`; `+25:00` is refused since SJL-8.7 put the shared `IsTimeZoneOffsetString` in front (G19) *(Revised 2026-10-18, at SJL-10.1, by JSD-0078's correspondence; read "accepts `"Z"` and `+25:00`")* |
| `PlainDateTime…add({years: 4294967295})` | `RangeError` | wraps through an unchecked `int` cast |
| `Duration.prototype.add` with calendar units | ignores `relativeTo` per the current proposal, `RangeError` | reads `relativeTo`; throws a plain `Error`, "not yet implemented" |
| Non-ISO `relativeTo` | supported | a plain `Error` |
| Rounding options in non-ISO `until`/`since` | applied | ignored |
| `ZonedDateTime` day rounding across DST | real day lengths | 24-hour days (its own comment says DST is not modelled) |
| Invalid month codes (`M13` in a 12-month calendar) | `RangeError` | constrained silently |
| Month-day reference years | the proposal's table | a search without it |
| Duration strings | exact (`BigInteger`) | `double.Parse` |

*(Added 2026-10-18, at SJL-10.1, by
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
decision D6.)* The correspondence, read from code and run in both engines (bundle
[`sjl-10-001`](../../evidence/sjl-10-001/correspondence.md)), finds these grammar cases beyond the table above.
SJL-10.2's grammar vectors cover them as accept and refuse cases, beside the table's. Each is named by its row, and
Broiler.VM's answer is the grammar's.

| Row | Case | Broiler.VM | Broiler.JS *(run at SJL-10.1)* |
|---|---|---|---|
| G3 | A year-month string whose day is not a date: `2020-02-30`, `2020-01-00`, `2020-01-32`, `2021-02-29`, `2020-04-31`, `1900-02-29`, `2020-02-30[u-ca=gregory]` | `RangeError` (`DateSpec`'s early error) | accepted as the year-month |
| G4 | A time from a date-time whose date is not a date: `2021-13-45T12:00`, `2020-13-01T12:00`, `2020-02-30T12:00`, `2021-02-29T12:00`, `2020-00-01T12:00`, `2020-01-32T12:00` | `RangeError` | `12:00:00` |
| G6 | Mixed separators in an instant: `Instant.from("2020-01-01T12:3045Z")` and `"…T1230:45Z"` | `RangeError`: one form per time | accepted as `…T12:30:45Z` |
| G8 | Offsets out of range after a time: `+24:00`, `+23:60`, `+23:59:60` | `RangeError` in every goal | accepted by `PlainDate`, `PlainDateTime`, `PlainYearMonth` and `PlainMonthDay` |
| G9 | Decimal digits other than ASCII (Arabic-Indic, fullwidth) in a year, a day, a minute, a fraction, an offset or a duration part, in each parser and in `relativeTo` | `RangeError` | an `Error` carrying .NET's `FormatException`; in the four plain types' offset, accepted and ignored |
| G10 | U+2212 MINUS SIGN as the sign of a year, an offset, an offset annotation and a duration | `RangeError` | `RangeError` |
| G11 | An empty or flag-only annotation, `[]` and `[!]`, after a date-time in each goal | `RangeError` | accepted, but in `ZonedDateTime` and `relativeTo` |
| G12 | A time zone annotation the grammar refuses: `[Mars Olympus]`, `[ UTC]`, `[UTC ]`, `[/a]`, `[a/]`, `[a//b]`, `[9a]`, `[+24:00]` | `RangeError` | accepted |
| G13 | A time zone annotation after another annotation: `[u-ca=iso8601][UTC]`, `[foo=bar][UTC]`, in every goal and in `relativeTo` | `RangeError` | accepted, the zone taken wherever it stands |
| G16 | A time on a year-month: `2020-01T12:00`, `202001T1200`, `2020-01 12:00`, `2020-01T12:00[u-ca=iso8601]` | `RangeError` | accepted as `2020-01` |
| G17 | Calendar strings that are not a calendar string: `T10:00Z`, `10:00Z`, `2020-01[u-ca=gregory]`, `01-01[u-ca=gregory]`, `2020-01-01[]`, `…[!foo=bar]`, `1214[u-ca=gregory]`; and two `u-ca` annotations | `RangeError`; of two `u-ca`, the first | accepted; of two `u-ca`, the last |
| G18 | Time zone strings that are not a time zone string: `T10:00Z`, `10:00Z`, `1214Z`, `…[u-ca=gregory][Europe/Berlin]`, `…Z[Europe/Berlin][UTC]` | `RangeError` | accepted, as `UTC`, `Europe/Berlin` or the last annotation |
| G19 | `Z` and `z` as an offset string, in `with({offset})` and a bag's `offset` | `RangeError` | accepted |
| G20 | `relativeTo` strings with annotations out of order, `+25:00`, or digits other than ASCII | `RangeError` | accepted, accepted, and an `Error` |
| G22 | A time designator with no time part: `P1YT`, `P1DT`, `P1Dt` | `RangeError` | accepted as `P1Y`, `P1D` |

*(Added 2026-10-18, at SJL-10.1.)* Beyond the grammar, the same correspondence finds what the table above omits or
refines:
- `ParseZonedDateTimeString` reads its options after the string's checks, with no observable consequence (G24).
  Duration strings and month-day reference years differ in the code only, with equal answers (G23, C10).
- A non-ISO `PlainDateTime` applies rounding options, but against ISO months (F4). The 24-hour days reach
  `ZonedDateTime` `until` and `since` only: `round`, `hoursInDay` and rounding relative to a zoned date use real day
  lengths (F7, D9).
- In Broiler.JS: a year-month subtraction at the lower limit (B5), `compare`'s range check (D4), rounding to months
  through a clamped difference (D6, D7), conversions in other calendars that lose a field (C13, C15 to C17), month
  codes with digits other than ASCII (C6), the aliases `gregorian` and `minguo` (C2), and era names of earlier drafts
  (C9).
- In Broiler.VM: three constructors that end the invocation on a year above 2^53 (R6).
- Where the pinned text and its polyfill disagree, on rounding to months after a shifted window, the text rules (D8;
  JSD-0078's decision D7).

---

## 4. The target

```csharp
namespace Broiler.ECMAScript.Temporal;

// The records are SJL-8's, published in Broiler.ECMAScript.Time, and are extended, not redeclared:
//   IsoDate(long Year, int Month, int Day);
//   IsoTime(int Hour, int Minute, int Second, int Millisecond, int Microsecond, int Nanosecond), with no overflow days;
//   IsoDateTime(IsoDate Date, IsoTime Time).
// A balancing result that carries overflow days returns them beside the record, not in it.
public readonly record struct DateDuration(double Years, double Months, double Weeks, double Days);
public readonly record struct InternalDuration(DateDuration Date, BigInteger TimeNanoseconds);

public static class TemporalGrammar
{
    public static bool TryParse(ReadOnlySpan<char> text, ReadOnlySpan<TemporalGoal> goals,
                                out IsoParseResult result, out TemporalParseError error);
    public static bool TryParseDuration(ReadOnlySpan<char> text, out DurationFields fields, out TemporalParseError error);
    public static bool IsTimeZoneIdentifier(ReadOnlySpan<char> s);
    public static bool IsUtcOffset(ReadOnlySpan<char> s, bool allowSubMinute);
}

public interface ICalendarArithmetic
{
    CalendarStatus DateAdd(CalendarId calendar, IsoDate date, DateDuration duration, Overflow overflow, out IsoDate result);
    CalendarStatus DateUntil(CalendarId calendar, IsoDate one, IsoDate two, TemporalUnit largestUnit, out DateDuration result);
}
public interface ITimeZoneArithmetic   // implemented over SJL-8's data
{
    long OffsetNanoseconds(TimeZoneId zone, BigInteger epochNanoseconds);
    int PossibleEpochNanoseconds(TimeZoneId zone, IsoDateTime local, Span<BigInteger> destination);
    bool TryStartOfDay(TimeZoneId zone, IsoDate date, out BigInteger epochNanoseconds);
}

public static class DurationMath { /* IsValid, Sign, DefaultLargestUnit, ToInternal, FromInternal, Add, Compare,
                                     Round, Total, RoundRelative (nudge and bubble) — over the interfaces */ }
public static class TemporalOptionRules { /* the allowed values, defaults, unit groups and difference settings */ }
public static class TemporalFormat { /* every string form, offsets, calendar annotations */ }
public static class Calendars { /* formula calendars, month codes, eras, reference years; tables via SJL-8 */ }
```

*(Revised 2026-10-18, at SJL-10.1, by
[JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
decision D8: one set of records serves the zones, the core and both engines, and SJL-8 already published it. The block
declared three records of its own after the namespace: "`public readonly record struct IsoDate(long Year, int Month,
int Day);`", "`public readonly record struct TimeOfDay(int Hour, int Minute, int Second, int Millisecond, int
Microsecond, int Nanosecond);`" and "`public readonly record struct IsoDateTime(IsoDate Date, TimeOfDay Time);`".)*

**Errors** are `TemporalParseError`, `CalendarStatus` and similar codes. Each engine maps them to its
own `RangeError` and `TypeError`, at the specification's moments.

**Exactness.** Time durations are exact integers of nanoseconds. Totals are exact rationals, rounded
once to a double (SJL-4's `ExactRounding`).

**What adapters may contain:**

- receiver checks and object creation;
- property-bag reads in specification order, through each engine's own `Get` and conversions;
- `relativeTo` resolution;
- `ToTemporal*` classification;
- calendar and zone identifier strings as engine values;
- error mapping;
- the clock and system zone for `Temporal.Now`.

---

## 5. Deletion lists

**Broiler.VM.** Everything below moves to the library:

- `JsTemporalCore` in full: records, limits, balancing, duration helpers and formatting;
- `JsTemporal.Conversions`' string formatting;
- `JsTemporal.Calendar`'s `RegulateDate`, its ISO `DateAdd` body and `CalendarAnnotation`;
- `JsTemporal.Durations`' `RegulateTime` and its nudging, bubbling and total cores;
- `JsTemporal.CalendarNonIso`'s month-code, reference-date and non-ISO add and until cores;
- `JsTemporalParser`, behind `TryParse`;
- the rule half of `JsTemporal.Options`.

**Broiler.JS:**

- the per-type `IsValidISODate`, `IsValidTime`, `IsValidISODateTime`, `IsWithinIsoDateLimits`, the
  `Regulate*`, `AddISODate` and `BalanceISOYearMonth` copies, and the formatters;
- every regular-expression parser listed in section 2;
- `TemporalIsoString`. Its coercing helpers become adapter code, and `RejectIncompatibleStyle` is
  removed as dead code;
- the separate relative-rounding implementations;
- `JSTemporalRoundingOptions`' arithmetic;
- `TemporalNonIso`'s calendar parts.

**Kept:** the public `JSTemporal*` classes, as shells.

---

## 6. Acceptance

**Both engines run:**

- Broiler.VM's retained polyfill dataset;
- test262's `built-ins/Temporal` and `intl402/Temporal` at the pin.

Broiler.JS's area evidence pins the proposal revision it follows, by recorded decision: `e8cc03fc`, JSD-0078's
decision D1 *(the revision added 2026-10-18, at SJL-10.1)*. **Broiler.VM's
verdicts on these suites are expected to be unchanged**, because its own code becomes the shared
code; the rule says so.
