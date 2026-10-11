# Area: Date arithmetic, the Date string grammar and Date formatting

**Delivered by:** [SJL-6](../milestones/sjl-6-date.md). **Library:**
`Broiler.ECMAScript.Algorithms`. **Status:** design. Observations marked *(reading)* are to be
confirmed by vectors.

---

## 1. Scope

| Sub-area | Specification | Shared | Stays in the engines |
|---|---|---|---|
| Time arithmetic | `Day`, `TimeWithinDay`, `DaysInYear`, `InLeapYear`, `YearFromTime`, `MonthFromTime`, `DateFromTime`, `WeekDay`, `HourFromTime` and its siblings, `MakeTime`, `MakeDay`, `MakeDate`, `TimeClip`, `MakeFullYear` | All of it, over `double` | Argument conversion order, storage of the time value |
| Civil-day helpers | (used by Date, Temporal, time zones, formatting) | Leap year, month length, epoch days in both directions, weekday, day of year, ISO week, over `long` | (none) |
| Local time | `LocalTime`, `UTC`, offset lookup, disambiguation of skipped and repeated times | A **provider interface** and the specification's disambiguation | **The provider**, which is engine policy (decision [SJL-D-19](../decisions.md#sjl-d-19)) |
| Parsing | The Date Time String Format (normative); implementation-defined fallbacks | The normative grammar; Broiler.VM's fallback grammar and a strict dialect, as a **dialect parameter** | `ToString`/`ToPrimitive`, charging policy, the choice of dialect, and Broiler.JS's culture-dependent fallback (decision [SJL-D-17](../decisions.md#sjl-d-17)) |
| Formatting | `toString`, `toDateString`, `toTimeString`, `toUTCString`, `toISOString` | The renderings | The `RangeError` for invalid dates; `toJSON`'s object protocol; routing `toLocale*` to Intl |

---

## 2. Current state

### Broiler.VM

- **Arithmetic.** Private static `double` functions in `JsRealm.Date`: `DateFloorMod`, `DateDay`,
  `DateTimeWithinDay`, `DateDaysInYear`, `DateDayFromYear`, `DateTimeFromYear`, `DateYearFromTime`,
  `DateInLeapYear`, `DateDayWithinYear`, `DateMonthStartDay`, `DateMonthFromTime`,
  `DateDateFromTime`, `DateWeekDay`, `DateHourFromTime` and its siblings, `DateMakeTime`,
  `DateMakeDay`, `DateMakeDate`, `DateTimeClip`. **No engine type, no error, no charge.**
- **Local time is UTC by decision** (JSD-0053 §7). The local and UTC variants of every getter and
  setter share one body.
- **Parsing.** `DateParseText` charges the text length, reads `±YYYYYY` (refusing `-000000`), and
  falls back to `DateParseTextual`. The fallback accepts the engine's own `toString` and
  `toUTCString` shapes. A doc comment in that region is stale: it says expanded years and legacy
  forms are refused.
- **Formatting.** `DateToFullText`, `DateToCalendarText`, `DateToClockText`, `DateToUtcText`,
  `DateToIsoText`. The zone text is fixed.
- **Other Gregorian copies.** `JsTemporalCore`, over longs; `JsTimeZones`' `DaysFromCivil`,
  `MonthLength` and `Weekday`; and `JsLocalTime.From` in the date-time formatter.

### Broiler.JS

- **Arithmetic.** `BuiltIns/Date/JSDateMath` (internal static) holds the specification formulas.
  The getters and setters use `DateTimeOffset` for years 0001 to 9999 and `JSDateMath` only on raw
  time-value paths.
- **Storage.** A `DateTimeOffset` value, a raw `double`, and a `MinValue` sentinel. Setters catch
  `ArgumentOutOfRangeException`.
- **Local time.** `JSDate.Local` reads `TimeZoneInfo.Local.BaseUtcOffset`, which ignores
  daylight-saving time. `JSDateMath.UTC`, `LocalTime` and `GetLocalOffsetMs` read DST-aware offsets.
  Outside the .NET range, the offset at the current instant is used.
- **Parsing.** `Date.parse` calls the public `Runtime/DateParser.Parse`, which tries .NET format
  lists, then a parse in the **current culture**, then the invariant culture. It falls back to
  `JSDateStatic.TryParseExtendedIso`, a regular expression. Its formats include SpiderMonkey-style
  `N/N/N` heuristics.
- **Formatting.** `JSDatePrototype.Formatters`. The zone suffix uses the operating system's
  localised `TimeZoneInfo` names. `toLocale*` goes through Intl, or through a non-standard .NET
  format string when its second argument is a string.

---

## 3. Differences already visible

| Case | Specification | Broiler.VM | Broiler.JS |
|---|---|---|---|
| Date-only ISO strings (`"2021-03-04"`) | **UTC** | UTC | **local time**: the format list uses `AssumeLocal` |
| `TimeClip(-0)` | `+0` | `+0` | `-0` is kept |
| `setUTCHours(1.5)` | `MakeTime` truncates its arguments | truncated | adds 90 minutes *(reading)* |
| Setters with fractional arguments | truncated | truncated | kept *(reading)* |
| Large integer setter arguments | converted per `ToIntegerOrInfinity` | correct | wrap at 2^31 *(reading)* |
| A valid date outside 0001 to 9999 after a setter | that date | that date | `NaN` *(reading)* |
| Fraction digits in the ISO format | the format has exactly three | exactly three | one to three, or one to nine, accepted |
| `"2021-02-30"` | implementation-defined (outside the format) | 2021-03-02 | `NaN` |
| Space- or slash-separated and culture formats | implementation-defined | refused unless in its own textual forms | accepted, culture-dependently |
| Zone suffix in `toString` | implementation-defined text | fixed | the host's localised names |
| Local time across DST | the specification's rules | not applicable (UTC) | getters and `toString` can disagree, because one uses the base offset and the other the DST-aware offset |

---

## 4. The target

```csharp
namespace Broiler.ECMAScript.Time;

public static class EcmaTime
{
    public const double MsPerDay = 8.64e7;
    public static double Day(double t);            public static double TimeWithinDay(double t);
    public static int DaysInYear(double y);        public static double DayFromYear(double y);
    public static double YearFromTime(double t);   public static bool InLeapYear(double t);
    public static int MonthFromTime(double t);     public static int DateFromTime(double t);
    public static int WeekDay(double t);           // …HourFromTime, MinFromTime, SecFromTime, MsFromTime
    public static double MakeTime(double h, double m, double s, double ms);   // ToIntegerOrInfinity inside
    public static double MakeDay(double year, double month, double date);
    public static double MakeDate(double day, double time);
    public static double TimeClip(double t);       // folds -0 to +0
    public static double MakeFullYear(double year);
}

public static class IsoCalendarMath
{
    public static bool IsLeapYear(long year);      public static int DaysInMonth(long year, int month);  // month valid by contract
    public static long EpochDays(long year, int month, int day);
    public static (long Year, int Month, int Day) FromEpochDays(long days);
    public static int DayOfWeek(long epochDays);   public static int DayOfYear(long y, int m, int d);
    public static (int Week, long Year) WeekOfYear(long y, int m, int d);
}

public interface ILocalTimeProvider       // engine policy: Broiler.VM = UTC; Broiler.JS = host or IANA
{
    double OffsetAtUtc(double utcTime);                        // ms
    double OffsetForLocal(double localTime, LocalDisambiguation d);
    string? DisplayName(double utcTime);                       // for toString's suffix; may be null
}

public static class EcmaDateParser
{
    public static EcmaParsedDate Parse(ReadOnlySpan<char> s, DateParseDialect dialect, WorkCharge? charge);
}
public readonly record struct EcmaParsedDate(EcmaParsedKind Kind /* Invalid | Utc | Local */, double Value);

public static class EcmaDateFormatter
{
    public static string DateString(double localTime);  public static string TimeString(double localTime);
    public static string TimeZoneString(double offsetMs, string? name);
    public static string ToUtcString(double t);          public static bool TryToIsoString(double t, out string s);
}
```

**Dialects.** `DateParseDialect` names the implementation-defined fallback grammar:

- **Broiler.VM's dialect**, which accepts its own `toString` and `toUTCString` outputs.
- **A strict dialect**, the normative format only.
- **Broiler.JS's culture-dependent legacy parsing is not a shared dialect.** It stays in
  Broiler.JS's adapter, behind `DateParser`'s public shell, under decision
  [SJL-D-17](../decisions.md#sjl-d-17). Broiler.JS calls the shared strict and textual grammars
  first, and its own fallback only for what they refuse.

**What adapters may contain:**

- argument conversion in specification order (the setters read the time value first, then convert
  only the arguments present);
- storage;
- the `RangeError` of `toISOString`;
- the `toJSON` protocol;
- `toLocale*` routing;
- the provider instance;
- the dialect choice;
- charging.

---

## 5. Broiler.JS's storage change

The shared arithmetic works on time values: doubles in milliseconds across ±8.64e15. Broiler.JS
stores a `DateTimeOffset`, which cannot represent most of that range, and its getters and setters
are written against it. **The cut-over therefore makes the `double` time value Broiler.JS's single
source of truth.** Any `DateTimeOffset` becomes a derived view, kept only for its CLR interop.

---

## 6. Deletion lists

**Broiler.VM:**

- the `Date*` arithmetic functions and their constants;
- `DateParseTextual`, `DateMonthByName`, `DateWholeNumber`, `DateReadDigits` (and the parsing body
  of `DateParseText`);
- the rendering helpers;
- `JsTemporalCore`'s `IsLeapYear`, `DaysInMonth`, `DaysInYear`, `EpochDays`, `FromEpochDays`,
  `AddDays`, `BalanceYearMonth`, `DayOfWeek`, `DayOfYear` and `WeekOfYear`;
- `JsTimeZones.DaysFromCivil`, `MonthLength` and `Weekday`;
- `JsLocalTime.From`.

**Broiler.JS:**

- `JSDateMath`'s arithmetic. `UTC`, `LocalTime` and `GetLocalOffsetMs` move into Broiler.JS's
  provider;
- `JSDateStatic.DaysInMonth` and `TryParseExtendedIso`;
- the copies of `IsLeapYear`, `DaysFromCivil` and `CivilFromDays` and the week helpers in
  `JSTemporalInstant`, `PlainDate`, `PlainDateTime`, `PlainMonthDay`, `PlainYearMonth`,
  `ZonedDateTime`, `TemporalIsoString`, `TemporalArithmeticCalendar` and `TemporalNonIso`;
- `Setters.CoercedIntValue`.

**`DateParser` stays as a public shell** under decision [SJL-D-25](../decisions.md#sjl-d-25).

---

## 7. Risks

| Risk | Mitigation |
|---|---|
| Broiler.JS's tests pin `TZ=UTC` and the invariant culture, which hides host-dependent behaviour | The area's Broiler.JS evidence also runs under a non-UTC zone with daylight-saving time, as a predeclared configuration |
| Broiler.JS consumers rely on local-time date-only parsing | A listed correction under decision `SJL-D-16`. The behaviour is the specification's. |
| Broiler.JS's storage change touches every getter and setter | Done in this cut-over with the full `Date` test262 directory per path, and the engine-level vectors |
