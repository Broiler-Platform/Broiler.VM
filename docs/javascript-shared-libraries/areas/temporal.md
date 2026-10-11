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

- Broiler.VM pins `tc39/proposal-temporal@e8cc03fc` (JSD-0053).
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

Errors come from static delegates that find the realm ambiently. No proposal revision is pinned.

---

## 3. Differences already visible

| Case | Broiler.VM | Broiler.JS *(reading)* |
|---|---|---|
| `PlainDate.from('2020-01-01[]')`, `PlainYearMonth.from('2020-02-30')`, `PlainTime.from('2021-13-45T12:00')` | `RangeError` (the grammar refuses them) | accepted |
| Bracketed annotations in the wrong order or malformed | refused | partly checked |
| Offset strings in `with()` | the grammar | accepts `"Z"` and `+25:00` |
| `PlainDateTime…add({years: 4294967295})` | `RangeError` | wraps through an unchecked `int` cast |
| `Duration.prototype.add` with calendar units | ignores `relativeTo` per the current proposal, `RangeError` | reads `relativeTo`; throws a plain `Error`, "not yet implemented" |
| Non-ISO `relativeTo` | supported | a plain `Error` |
| Rounding options in non-ISO `until`/`since` | applied | ignored |
| `ZonedDateTime` day rounding across DST | real day lengths | 24-hour days (its own comment says DST is not modelled) |
| Invalid month codes (`M13` in a 12-month calendar) | `RangeError` | constrained silently |
| Month-day reference years | the proposal's table | a search without it |
| Duration strings | exact (`BigInteger`) | `double.Parse` |

---

## 4. The target

```csharp
namespace Broiler.ECMAScript.Temporal;

public readonly record struct IsoDate(long Year, int Month, int Day);
public readonly record struct TimeOfDay(int Hour, int Minute, int Second, int Millisecond, int Microsecond, int Nanosecond);
public readonly record struct IsoDateTime(IsoDate Date, TimeOfDay Time);
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

Broiler.JS's area evidence pins the proposal revision it follows, by recorded decision. **Broiler.VM's
verdicts on these suites are expected to be unchanged**, because its own code becomes the shared
code; the rule says so.
