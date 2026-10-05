// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   85
// Annotated:        85/85
// Exempt:           27
// Human-reviewed:   0/85
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       85
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>Temporal's units, in the order of the proposal's Table 21: largest first.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D7DB1A
// Broiler-Human:        PENDING
internal enum TemporalUnit
{
    Year,
    Month,
    Week,
    Day,
    Hour,
    Minute,
    Second,
    Millisecond,
    Microsecond,
    Nanosecond,

    /// <summary>The specification's <c>unset</c>: no unit was given.</summary>
    Unset,

    /// <summary>The specification's <c>auto</c>.</summary>
    Auto,
}

/// <summary>The rounding modes of the proposal's Table 28.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=331C77
// Broiler-Human:        PENDING
internal enum TemporalRounding
{
    Ceil,
    Floor,
    Expand,
    Trunc,
    HalfCeil,
    HalfFloor,
    HalfExpand,
    HalfTrunc,
    HalfEven,
}

/// <summary>An ISO Date Record (s3.5.1): a valid ISO 8601 date, the year not necessarily within Temporal's range.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A8BAB2
// Broiler-Human:        PENDING
internal readonly record struct JsIsoDate(long Year, int Month, int Day);

/// <summary>A Time Record (s4.5.1): a valid clock time and a number of overflow days.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=652EBE
// Broiler-Human:        PENDING
internal readonly record struct JsTimeRecord(long Days, int Hour, int Minute, int Second, int Millisecond, int Microsecond, int Nanosecond)
{
    /// <summary>The specification's MidnightTimeRecord.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DE5BAA
    // Broiler-Human:        PENDING
    internal static JsTimeRecord Midnight => default;

    /// <summary>The specification's NoonTimeRecord.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C5A337
    // Broiler-Human:        PENDING
    internal static JsTimeRecord Noon => new(0, 12, 0, 0, 0, 0, 0);
}

/// <summary>An ISO Date-Time Record (s5.5.1).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0F293E
// Broiler-Human:        PENDING
internal readonly record struct JsIsoDateTime(JsIsoDate Date, JsTimeRecord Time);

/// <summary>A Date Duration Record (s7.5.1): float64-representable integers.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=89B0DB
// Broiler-Human:        PENDING
internal readonly record struct JsDateDuration(double Years, double Months, double Weeks, double Days);

/// <summary>An Internal Duration Record (s7.5.3): a date duration and a time duration in nanoseconds.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C70FB8
// Broiler-Human:        PENDING
internal readonly record struct JsInternalDuration(JsDateDuration Date, BigInteger Time);

/// <summary>
/// The ISO 8601 arithmetic Temporal is built on (phase F8, JSD-0054): dates and times as records,
/// epoch days and nanoseconds, the representable limits, exact rounding to an increment, and the
/// duration operations of the proposal's sections 3, 4, 5, 7 and 13.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every quantity the proposal calls a mathematical value is exact here.</b> A time duration and an
/// epoch-nanoseconds value are <see cref="BigInteger"/>s, a fraction is a numerator and a
/// denominator, and a Number is made from either only where the proposal applies 𝔽, correctly
/// rounded to nearest, ties to even. A duration's ten fields are doubles, because the proposal
/// stores them as float64-representable integers.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=36F2D8
// Broiler-Human:        PENDING
internal static class JsTemporalCore
{
    /// <summary>Nanoseconds in a 24-hour day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EFD3C8
    // Broiler-Human:        PENDING
    internal static readonly BigInteger NsPerDay = new(86_400_000_000_000L);

    /// <summary>The proposal's nsMaxInstant: 10^8 days.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=98D70A
    // Broiler-Human:        PENDING
    internal static readonly BigInteger NsMaxInstant = NsPerDay * 100_000_000;

    /// <summary>The proposal's nsMinInstant.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=70E742
    // Broiler-Human:        PENDING
    internal static readonly BigInteger NsMinInstant = -NsMaxInstant;

    /// <summary>The proposal's maxTimeDuration: 2^53 × 10^9 - 1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A5419
    // Broiler-Human:        PENDING
    internal static readonly BigInteger MaxTimeDuration = (BigInteger.One << 53) * 1_000_000_000 - 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=35261A
    // Broiler-Human:        PENDING
    private static readonly BigInteger[] UnitLengths =
    [
        BigInteger.Zero,
        BigInteger.Zero,
        BigInteger.Zero,
        new(86_400_000_000_000L),
        new(3_600_000_000_000L),
        new(60_000_000_000L),
        new(1_000_000_000L),
        new(1_000_000L),
        new(1_000L),
        BigInteger.One,
    ];

    /// <summary>The "Length in Nanoseconds" of a time unit or day (Table 21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=53008E
    // Broiler-Human:        PENDING
    internal static BigInteger UnitLength(TemporalUnit unit) => UnitLengths[(int)unit];

    /// <summary>The singular property names of Table 21, in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=27D7DF
    // Broiler-Human:        PENDING
    internal static readonly string[] UnitNames =
        ["year", "month", "week", "day", "hour", "minute", "second", "millisecond", "microsecond", "nanosecond"];

    /// <summary>The proposal's IsCalendarUnit (s13.21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1D8B30
    // Broiler-Human:        PENDING
    internal static bool IsCalendarUnit(TemporalUnit unit) => unit is TemporalUnit.Year or TemporalUnit.Month or TemporalUnit.Week;

    /// <summary>Whether a unit's category is date (Table 21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AF1A71
    // Broiler-Human:        PENDING
    internal static bool IsDateUnit(TemporalUnit unit) => unit <= TemporalUnit.Day;

    /// <summary>The proposal's LargerOfTwoTemporalUnits (s13.20).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CC1E45
    // Broiler-Human:        PENDING
    internal static TemporalUnit Larger(TemporalUnit one, TemporalUnit two) => one <= two ? one : two;

    /// <summary>The proposal's MaximumTemporalDurationRoundingIncrement (s13.23): 0 for unset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8FD53F
    // Broiler-Human:        PENDING
    internal static long MaximumIncrement(TemporalUnit unit) => unit switch
    {
        TemporalUnit.Hour => 24,
        TemporalUnit.Minute or TemporalUnit.Second => 60,
        TemporalUnit.Millisecond or TemporalUnit.Microsecond or TemporalUnit.Nanosecond => 1000,
        _ => 0,
    };

    // ---- ISO dates --------------------------------------------------------------------------

    /// <summary>Whether a year is a leap year in the proleptic Gregorian calendar.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DC089B
    // Broiler-Human:        PENDING
    internal static bool IsLeapYear(long year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

    /// <summary>The proposal's ISODaysInMonth (s12.3.17).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F76245
    // Broiler-Human:        PENDING
    internal static int DaysInMonth(long year, int month) => month switch
    {
        2 => IsLeapYear(year) ? 29 : 28,
        4 or 6 or 9 or 11 => 30,
        _ => 31,
    };

    /// <summary>The proposal's MathematicalDaysInYear.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=53E22E
    // Broiler-Human:        PENDING
    internal static int DaysInYear(long year) => IsLeapYear(year) ? 366 : 365;

    /// <summary>Days since 1970-01-01 of a valid ISO date (ISODateToEpochDays with a 1-based month).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3DB4D5
    // Broiler-Human:        PENDING
    internal static long EpochDays(long year, int month, int day)
    {
        var y = year - (month <= 2 ? 1 : 0);
        var era = (y >= 0 ? y : y - 399) / 400;
        var yearOfEra = y - (era * 400);
        var dayOfYear = (((153 * (month + (month > 2 ? -3 : 9))) + 2) / 5) + day - 1;
        var dayOfEra = (yearOfEra * 365) + (yearOfEra / 4) - (yearOfEra / 100) + dayOfYear;
        return (era * 146097) + dayOfEra - 719468;
    }

    /// <summary>Days since the epoch of an ISO date.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=38B27E
    // Broiler-Human:        PENDING
    internal static long EpochDays(JsIsoDate date) => EpochDays(date.Year, date.Month, date.Day);

    /// <summary>The ISO date of a day since the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B7626C
    // Broiler-Human:        PENDING
    internal static JsIsoDate FromEpochDays(long days)
    {
        var z = days + 719468;
        var era = (z >= 0 ? z : z - 146096) / 146097;
        var dayOfEra = z - (era * 146097);
        var yearOfEra = (dayOfEra - (dayOfEra / 1460) + (dayOfEra / 36524) - (dayOfEra / 146096)) / 365;
        var dayOfYear = dayOfEra - ((365 * yearOfEra) + (yearOfEra / 4) - (yearOfEra / 100));
        var monthPart = ((5 * dayOfYear) + 2) / 153;
        var day = (int)(dayOfYear - (((153 * monthPart) + 2) / 5) + 1);
        var month = (int)(monthPart < 10 ? monthPart + 3 : monthPart - 9);
        return new JsIsoDate(yearOfEra + (era * 400) + (month <= 2 ? 1 : 0), month, day);
    }

    /// <summary>The proposal's IsValidISODate (s3.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E9762
    // Broiler-Human:        PENDING
    internal static bool IsValidIsoDate(double year, double month, double day) =>
        month is >= 1 and <= 12 && day >= 1 && day <= DaysInMonth((long)year, (int)month);

    /// <summary>The proposal's AddDaysToISODate (s3.5.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BB5D60
    // Broiler-Human:        PENDING
    internal static JsIsoDate AddDays(JsIsoDate date, long days) => FromEpochDays(EpochDays(date) + days);

    /// <summary>The proposal's BalanceISOYearMonth: a year and a month 1 to 12.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AE72EE
    // Broiler-Human:        PENDING
    internal static (long Year, int Month) BalanceYearMonth(long year, long month)
    {
        var zeroBased = month - 1;
        var carry = zeroBased >= 0 ? zeroBased / 12 : ((zeroBased + 1) / 12) - 1;
        return (year + carry, (int)(zeroBased - (carry * 12)) + 1);
    }

    /// <summary>The proposal's CompareISODate (s3.5.13).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1F9B18
    // Broiler-Human:        PENDING
    internal static int Compare(JsIsoDate one, JsIsoDate two) =>
        one.Year != two.Year ? (one.Year > two.Year ? 1 : -1)
        : one.Month != two.Month ? (one.Month > two.Month ? 1 : -1)
        : one.Day != two.Day ? (one.Day > two.Day ? 1 : -1)
        : 0;

    /// <summary>The proposal's ISODayOfWeek (s12.3.20): Monday 1 to Sunday 7.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3F7BE
    // Broiler-Human:        PENDING
    internal static int DayOfWeek(JsIsoDate date)
    {
        var weekday = (int)(((EpochDays(date) + 4) % 7 + 7) % 7);
        return weekday == 0 ? 7 : weekday;
    }

    /// <summary>The proposal's ISODayOfYear (s12.3.19).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F4F876
    // Broiler-Human:        PENDING
    internal static int DayOfYear(JsIsoDate date) => (int)(EpochDays(date) - EpochDays(date.Year, 1, 1)) + 1;

    /// <summary>The proposal's ISOWeekOfYear (s12.3.18): the week and the week-year.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=42B6F7
    // Broiler-Human:        PENDING
    internal static (int Week, long Year) WeekOfYear(JsIsoDate date)
    {
        var year = date.Year;
        var dayOfYear = DayOfYear(date);
        var dayOfWeek = DayOfWeek(date);
        var week = (dayOfYear + 7 - dayOfWeek + 3) / 7;

        if (week < 1)
        {
            var jan1 = DayOfWeek(new JsIsoDate(year, 1, 1));

            if (jan1 == 5 || (jan1 == 6 && IsLeapYear(year - 1)))
            {
                return (53, year - 1);
            }

            return (52, year - 1);
        }

        if (week == 53 && DaysInYear(year) - dayOfYear < 4 - dayOfWeek)
        {
            return (1, year + 1);
        }

        return (week, year);
    }

    /// <summary>The proposal's ISODateWithinLimits (s3.5.12): the date at noon is a representable date-time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DEC44D
    // Broiler-Human:        PENDING
    internal static bool DateWithinLimits(JsIsoDate date) => DateTimeWithinLimits(new JsIsoDateTime(date, JsTimeRecord.Noon));

    /// <summary>The proposal's ISODateTimeWithinLimits (s5.5.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ACBAF5
    // Broiler-Human:        PENDING
    internal static bool DateTimeWithinLimits(JsIsoDateTime dateTime)
    {
        if (System.Math.Abs(EpochDays(dateTime.Date)) > 100_000_001)
        {
            return false;
        }

        var ns = UtcEpochNs(dateTime);
        return ns > NsMinInstant - NsPerDay && ns < NsMaxInstant + NsPerDay;
    }

    /// <summary>The proposal's ISOYearMonthWithinLimits: April -271821 to September 275760.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=82B1B2
    // Broiler-Human:        PENDING
    internal static bool YearMonthWithinLimits(JsIsoDate date) =>
        date.Year >= -271821 && date.Year <= 275760 &&
        !(date.Year == -271821 && date.Month < 4) && !(date.Year == 275760 && date.Month > 9);

    /// <summary>The proposal's CheckISODaysRange (s13.4): whether the date is within 10^8 days of the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0E5416
    // Broiler-Human:        PENDING
    internal static bool DaysWithinRange(JsIsoDate date) => System.Math.Abs(EpochDays(date)) <= 100_000_000;

    // ---- Times ------------------------------------------------------------------------------

    /// <summary>The proposal's IsValidTime (s4.5.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2FA6E9
    // Broiler-Human:        PENDING
    internal static bool IsValidTime(double hour, double minute, double second, double millisecond, double microsecond, double nanosecond) =>
        hour is >= 0 and <= 23 && minute is >= 0 and <= 59 && second is >= 0 and <= 59 &&
        millisecond is >= 0 and <= 999 && microsecond is >= 0 and <= 999 && nanosecond is >= 0 and <= 999;

    /// <summary>A time's nanoseconds since midnight.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E2150
    // Broiler-Human:        PENDING
    internal static BigInteger TimeNs(JsTimeRecord time) =>
        (((((((BigInteger)time.Hour * 60) + time.Minute) * 60) + time.Second) * 1000 + time.Millisecond) * 1000 + time.Microsecond) * 1000 +
        time.Nanosecond;

    /// <summary>The proposal's BalanceTime (s4.5.10), over a whole number of nanoseconds from midnight.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44C80B
    // Broiler-Human:        PENDING
    internal static JsTimeRecord BalanceTime(BigInteger nanoseconds)
    {
        var days = BigInteger.DivRem(nanoseconds, NsPerDay, out var within);

        if (within.Sign < 0)
        {
            days -= 1;
            within += NsPerDay;
        }

        var ns = (long)within;
        return new JsTimeRecord(
            (long)days,
            (int)(ns / 3_600_000_000_000L),
            (int)(ns / 60_000_000_000L % 60),
            (int)(ns / 1_000_000_000L % 60),
            (int)(ns / 1_000_000L % 1000),
            (int)(ns / 1000 % 1000),
            (int)(ns % 1000));
    }

    /// <summary>The proposal's BalanceTime of six components.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FFF86E
    // Broiler-Human:        PENDING
    internal static JsTimeRecord BalanceTime(double hour, double minute, double second, double millisecond, double microsecond, double nanosecond) =>
        BalanceTime(
            (new BigInteger(hour) * 3_600_000_000_000L) + (new BigInteger(minute) * 60_000_000_000L) +
            (new BigInteger(second) * 1_000_000_000L) + (new BigInteger(millisecond) * 1_000_000L) +
            (new BigInteger(microsecond) * 1000) + new BigInteger(nanosecond));

    /// <summary>The proposal's CompareTimeRecord (s4.5.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=84A2F6
    // Broiler-Human:        PENDING
    internal static int Compare(JsTimeRecord one, JsTimeRecord two) => TimeNs(one).CompareTo(TimeNs(two)) switch
    {
        > 0 => 1,
        < 0 => -1,
        _ => 0,
    };

    /// <summary>The proposal's DifferenceTime (s4.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B69C33
    // Broiler-Human:        PENDING
    internal static BigInteger DifferenceTime(JsTimeRecord one, JsTimeRecord two) => TimeNs(two) - TimeNs(one);

    /// <summary>The proposal's AddTime (s4.5.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FEC13
    // Broiler-Human:        PENDING
    internal static JsTimeRecord AddTime(JsTimeRecord time, BigInteger duration) => BalanceTime(TimeNs(time) + duration);

    /// <summary>The proposal's RoundTime (s4.5.16).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3CC195
    // Broiler-Human:        PENDING
    internal static JsTimeRecord RoundTime(JsTimeRecord time, long increment, TemporalUnit unit, TemporalRounding mode)
    {
        var length = UnitLength(unit);
        var quantity = unit switch
        {
            TemporalUnit.Day or TemporalUnit.Hour => TimeNs(time),
            TemporalUnit.Minute => TimeNs(time with { Hour = 0 }),
            TemporalUnit.Second => TimeNs(time with { Hour = 0, Minute = 0 }),
            TemporalUnit.Millisecond => TimeNs(time with { Hour = 0, Minute = 0, Second = 0 }),
            TemporalUnit.Microsecond => (BigInteger)time.Microsecond * 1000 + time.Nanosecond,
            _ => time.Nanosecond,
        };

        var result = RoundToIncrement(quantity, increment * length, mode) / length;

        return unit switch
        {
            TemporalUnit.Day => new JsTimeRecord((long)result, 0, 0, 0, 0, 0, 0),
            TemporalUnit.Hour => BalanceTime(result * UnitLength(TemporalUnit.Hour)),
            TemporalUnit.Minute => BalanceTime(((BigInteger)time.Hour * 60 + result) * UnitLength(TemporalUnit.Minute)),
            TemporalUnit.Second => BalanceTime(TimeNs(time with { Second = 0, Millisecond = 0, Microsecond = 0, Nanosecond = 0 }) + result * UnitLength(TemporalUnit.Second)),
            TemporalUnit.Millisecond => BalanceTime(TimeNs(time with { Millisecond = 0, Microsecond = 0, Nanosecond = 0 }) + result * UnitLength(TemporalUnit.Millisecond)),
            TemporalUnit.Microsecond => BalanceTime(TimeNs(time with { Microsecond = 0, Nanosecond = 0 }) + result * 1000),
            _ => BalanceTime(TimeNs(time with { Nanosecond = 0 }) + result),
        };
    }

    // ---- Date-times and epoch nanoseconds ---------------------------------------------------

    /// <summary>The proposal's GetUTCEpochNanoseconds (s14.6.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=360DA9
    // Broiler-Human:        PENDING
    internal static BigInteger UtcEpochNs(JsIsoDateTime dateTime) =>
        ((BigInteger)EpochDays(dateTime.Date) * NsPerDay) + TimeNs(dateTime.Time);

    /// <summary>The proposal's IsValidEpochNanoseconds (s8.5.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FB51D
    // Broiler-Human:        PENDING
    internal static bool IsValidEpochNs(BigInteger ns) => ns >= NsMinInstant && ns <= NsMaxInstant;

    /// <summary>The proposal's GetISOPartsFromEpoch (s11.1.2), for any integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FD6639
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime FromEpochNs(BigInteger ns)
    {
        var time = BalanceTime(ns);
        return new JsIsoDateTime(FromEpochDays(time.Days), time with { Days = 0 });
    }

    /// <summary>The proposal's BalanceISODateTime (s5.5.7), the time part given in nanoseconds from the date's midnight.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F966B2
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime BalanceDateTime(JsIsoDate date, BigInteger nanoseconds)
    {
        var time = BalanceTime(nanoseconds);
        return new JsIsoDateTime(AddDays(date, time.Days), time with { Days = 0 });
    }

    /// <summary>The proposal's CompareISODateTime (s5.5.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=507FB2
    // Broiler-Human:        PENDING
    internal static int Compare(JsIsoDateTime one, JsIsoDateTime two)
    {
        var date = Compare(one.Date, two.Date);
        return date != 0 ? date : Compare(one.Time, two.Time);
    }

    /// <summary>The proposal's RoundISODateTime (s5.5.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6F915E
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime RoundDateTime(JsIsoDateTime dateTime, long increment, TemporalUnit unit, TemporalRounding mode)
    {
        var rounded = RoundTime(dateTime.Time, increment, unit, mode);
        return new JsIsoDateTime(AddDays(dateTime.Date, rounded.Days), rounded with { Days = 0 });
    }

    // ---- Rounding ---------------------------------------------------------------------------

    /// <summary>The proposal's NegateRoundingMode (s13.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C0FFAC
    // Broiler-Human:        PENDING
    internal static TemporalRounding Negate(TemporalRounding mode) => mode switch
    {
        TemporalRounding.Ceil => TemporalRounding.Floor,
        TemporalRounding.Floor => TemporalRounding.Ceil,
        TemporalRounding.HalfCeil => TemporalRounding.HalfFloor,
        TemporalRounding.HalfFloor => TemporalRounding.HalfCeil,
        _ => mode,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0A895E
    // Broiler-Human:        PENDING
    private enum Unsigned
    {
        Zero,
        Infinity,
        HalfZero,
        HalfInfinity,
        HalfEven,
    }

    /// <summary>The proposal's GetUnsignedRoundingMode (s13.27, Table 22).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=231421
    // Broiler-Human:        PENDING
    private static Unsigned UnsignedMode(TemporalRounding mode, bool negative) => mode switch
    {
        TemporalRounding.Ceil => negative ? Unsigned.Zero : Unsigned.Infinity,
        TemporalRounding.Floor => negative ? Unsigned.Infinity : Unsigned.Zero,
        TemporalRounding.Expand => Unsigned.Infinity,
        TemporalRounding.Trunc => Unsigned.Zero,
        TemporalRounding.HalfCeil => negative ? Unsigned.HalfZero : Unsigned.HalfInfinity,
        TemporalRounding.HalfFloor => negative ? Unsigned.HalfInfinity : Unsigned.HalfZero,
        TemporalRounding.HalfExpand => Unsigned.HalfInfinity,
        TemporalRounding.HalfTrunc => Unsigned.HalfZero,
        _ => Unsigned.HalfEven,
    };

    /// <summary>
    /// The proposal's ApplyUnsignedRoundingMode (s13.28) for a non-negative fraction
    /// <paramref name="numerator"/> / <paramref name="denominator"/> between the integers
    /// <paramref name="r1"/> and <paramref name="r1"/> + 1: the integer it rounds to.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9A90C
    // Broiler-Human:        PENDING
    private static BigInteger ApplyUnsigned(BigInteger numerator, BigInteger denominator, BigInteger r1, Unsigned mode)
    {
        var remainder = numerator - (r1 * denominator);

        if (remainder.IsZero)
        {
            return r1;
        }

        var r2 = r1 + 1;

        switch (mode)
        {
            case Unsigned.Zero:
                return r1;
            case Unsigned.Infinity:
                return r2;
        }

        var twice = remainder * 2;

        if (twice < denominator)
        {
            return r1;
        }

        if (twice > denominator)
        {
            return r2;
        }

        return mode switch
        {
            Unsigned.HalfZero => r1,
            Unsigned.HalfInfinity => r2,
            _ => r1.IsEven ? r1 : r2,
        };
    }

    /// <summary>
    /// The proposal's RoundNumberToIncrement (s13.29) of a fraction <paramref name="numerator"/> /
    /// <paramref name="denominator"/>, the denominator positive: a multiple of <paramref name="increment"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=434861
    // Broiler-Human:        PENDING
    internal static BigInteger RoundToIncrement(BigInteger numerator, BigInteger denominator, BigInteger increment, TemporalRounding mode)
    {
        var negative = numerator.Sign < 0;
        var magnitude = BigInteger.Abs(numerator);
        var scaled = denominator * increment;
        var r1 = BigInteger.Divide(magnitude, scaled);
        var rounded = ApplyUnsigned(magnitude, scaled, r1, UnsignedMode(mode, negative));
        return (negative ? -rounded : rounded) * increment;
    }

    /// <summary>The proposal's RoundNumberToIncrement (s13.29) of an integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3BD5A
    // Broiler-Human:        PENDING
    internal static BigInteger RoundToIncrement(BigInteger x, BigInteger increment, TemporalRounding mode) =>
        RoundToIncrement(x, BigInteger.One, increment, mode);

    /// <summary>The proposal's RoundNumberToIncrementAsIfPositive (s13.30) of an integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6D2DB7
    // Broiler-Human:        PENDING
    internal static BigInteger RoundToIncrementAsIfPositive(BigInteger x, BigInteger increment, TemporalRounding mode)
    {
        var r1 = BigInteger.Divide(x, increment);

        if (x.Sign < 0 && !(r1 * increment).Equals(x))
        {
            r1 -= 1;
        }

        // THE FLOOR ABOVE MAKES x - r1 × increment NON-NEGATIVE, so the positive table applies, and
        // halfEven's tie goes to the even one of r1 and r1 + 1 themselves.
        return ApplyUnsigned(x, increment, r1, UnsignedMode(mode, negative: false)) * increment;
    }

    /// <summary>
    /// The unsigned rounding mode's choice between <paramref name="r1"/> and <paramref name="r2"/> for
    /// a value whose absolute value is the fraction given, which lies between them (s13.28, as
    /// NudgeToCalendarUnit applies it).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6305B3
    // Broiler-Human:        PENDING
    internal static bool RoundsToSecond(
        BigInteger numerator, BigInteger denominator, BigInteger r1, BigInteger r2, TemporalRounding mode, bool negative)
    {
        // THE SPACING IS ONE INCREMENT, so the fraction (value - r1) / (r2 - r1) decides.
        var span = r2 - r1;
        var offset = numerator - (r1 * denominator);
        var unsignedMode = UnsignedMode(mode, negative);

        if (offset.IsZero)
        {
            return false;
        }

        switch (unsignedMode)
        {
            case Unsigned.Zero:
                return false;
            case Unsigned.Infinity:
                return true;
        }

        var twice = offset * 2;
        var whole = span * denominator;

        if (twice < whole)
        {
            return false;
        }

        if (twice > whole)
        {
            return true;
        }

        return unsignedMode switch
        {
            Unsigned.HalfZero => false,
            Unsigned.HalfInfinity => true,
            _ => !(BigInteger.Divide(r1, span).IsEven),
        };
    }

    // ---- Numbers ----------------------------------------------------------------------------

    /// <summary>𝔽 of a fraction: the nearest double, ties to even.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D53D7A
    // Broiler-Human:        PENDING
    internal static double ToDouble(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.Sign < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        if (numerator.IsZero)
        {
            return 0;
        }

        var negative = numerator.Sign < 0;
        var n = BigInteger.Abs(numerator);

        // SCALE SO THE QUOTIENT HAS 55 OR 56 BITS, then round to 53 with the remainder as sticky.
        var shift = 55 - (int)(n.GetBitLength() - denominator.GetBitLength());
        var quotient = shift >= 0
            ? BigInteger.DivRem(n << shift, denominator, out var remainder)
            : BigInteger.DivRem(n, denominator << -shift, out remainder);

        var extra = (int)quotient.GetBitLength() - 53;
        var mantissa = quotient >> extra;
        var dropped = quotient - (mantissa << extra);
        var half = BigInteger.One << (extra - 1);

        if (dropped > half || (dropped == half && (!remainder.IsZero || !mantissa.IsEven)))
        {
            mantissa += 1;
        }

        var result = System.Math.ScaleB((double)mantissa, extra - shift);
        return negative ? -result : result;
    }

    /// <summary>𝔽 of an integer: the nearest double, ties to even.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E717BC
    // Broiler-Human:        PENDING
    internal static double ToDouble(BigInteger value) => ToDouble(value, BigInteger.One);

    /// <summary>The exact integer of an integral double.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ADA9FA
    // Broiler-Human:        PENDING
    internal static BigInteger Exact(double value) => new(value);

    /// <summary>The decimal digits of an integral double, without an exponent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AD4A52
    // Broiler-Human:        PENDING
    internal static string IntegerText(double value) => new BigInteger(value).ToString(System.Globalization.CultureInfo.InvariantCulture);

    // ---- Durations ----------------------------------------------------------------------------

    /// <summary>The proposal's IsValidDuration (s7.5.16), over a duration's ten fields.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7550E2
    // Broiler-Human:        PENDING
    internal static bool IsValidDuration(System.ReadOnlySpan<double> fields)
    {
        var sign = 0;

        foreach (var value in fields)
        {
            if (!double.IsFinite(value))
            {
                return false;
            }

            if (value < 0)
            {
                if (sign > 0)
                {
                    return false;
                }

                sign = -1;
            }
            else if (value > 0)
            {
                if (sign < 0)
                {
                    return false;
                }

                sign = 1;
            }
        }

        var limit = 4294967296.0;

        if (System.Math.Abs(fields[0]) >= limit || System.Math.Abs(fields[1]) >= limit || System.Math.Abs(fields[2]) >= limit)
        {
            return false;
        }

        var normalized = (Exact(fields[3]) * NsPerDay) + TimeFromComponents(fields[4], fields[5], fields[6], fields[7], fields[8], fields[9]);
        return BigInteger.Abs(normalized) <= MaxTimeDuration;
    }

    /// <summary>The proposal's TimeDurationFromComponents (s7.5.21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=288EE6
    // Broiler-Human:        PENDING
    internal static BigInteger TimeFromComponents(double hours, double minutes, double seconds, double milliseconds, double microseconds, double nanoseconds) =>
        (Exact(hours) * 3_600_000_000_000L) + (Exact(minutes) * 60_000_000_000L) + (Exact(seconds) * 1_000_000_000L) +
        (Exact(milliseconds) * 1_000_000L) + (Exact(microseconds) * 1000) + Exact(nanoseconds);

    /// <summary>The proposal's DurationSign (s7.5.13).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7995F0
    // Broiler-Human:        PENDING
    internal static int DurationSign(System.ReadOnlySpan<double> fields)
    {
        foreach (var value in fields)
        {
            if (value < 0)
            {
                return -1;
            }

            if (value > 0)
            {
                return 1;
            }
        }

        return 0;
    }

    /// <summary>The proposal's DateDurationSign (s7.5.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=232E78
    // Broiler-Human:        PENDING
    internal static int Sign(JsDateDuration date) =>
        DurationSign([date.Years, date.Months, date.Weeks, date.Days]);

    /// <summary>The proposal's InternalDurationSign (s7.5.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F64CB3
    // Broiler-Human:        PENDING
    internal static int Sign(JsInternalDuration duration)
    {
        var date = Sign(duration.Date);
        return date != 0 ? date : duration.Time.Sign;
    }

    /// <summary>The proposal's DefaultTemporalLargestUnit (s7.5.17).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=08F834
    // Broiler-Human:        PENDING
    internal static TemporalUnit DefaultLargestUnit(System.ReadOnlySpan<double> fields)
    {
        for (var i = 0; i < 9; i++)
        {
            if (fields[i] != 0)
            {
                return (TemporalUnit)i;
            }
        }

        return TemporalUnit.Nanosecond;
    }

    /// <summary>The proposal's ToInternalDurationRecord (s7.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=374850
    // Broiler-Human:        PENDING
    internal static JsInternalDuration ToInternal(System.ReadOnlySpan<double> fields) =>
        new(new JsDateDuration(fields[0], fields[1], fields[2], fields[3]), TimeFromComponents(fields[4], fields[5], fields[6], fields[7], fields[8], fields[9]));

    /// <summary>The proposal's ToInternalDurationRecordWith24HourDays (s7.5.6); the days are within the time duration's range by IsValidDuration.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=559210
    // Broiler-Human:        PENDING
    internal static JsInternalDuration ToInternalWith24HourDays(System.ReadOnlySpan<double> fields) =>
        new(
            new JsDateDuration(fields[0], fields[1], fields[2], 0),
            TimeFromComponents(fields[4], fields[5], fields[6], fields[7], fields[8], fields[9]) + (Exact(fields[3]) * NsPerDay));

    /// <summary>
    /// The proposal's TemporalDurationFromInternal (s7.5.8) before CreateTemporalDuration: the ten
    /// fields, the time balanced up to <paramref name="largestUnit"/> and each made a Number.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=17F9CB
    // Broiler-Human:        PENDING
    internal static double[] FromInternal(JsInternalDuration duration, TemporalUnit largestUnit)
    {
        var sign = duration.Time.Sign;
        var nanoseconds = BigInteger.Abs(duration.Time);
        BigInteger days = 0, hours = 0, minutes = 0, seconds = 0, milliseconds = 0, microseconds = 0;

        if (largestUnit <= TemporalUnit.Microsecond)
        {
            microseconds = BigInteger.DivRem(nanoseconds, 1000, out nanoseconds);
        }

        if (largestUnit <= TemporalUnit.Millisecond)
        {
            milliseconds = BigInteger.DivRem(microseconds, 1000, out microseconds);
        }

        if (largestUnit <= TemporalUnit.Second)
        {
            seconds = BigInteger.DivRem(milliseconds, 1000, out milliseconds);
        }

        if (largestUnit <= TemporalUnit.Minute)
        {
            minutes = BigInteger.DivRem(seconds, 60, out seconds);
        }

        if (largestUnit <= TemporalUnit.Hour)
        {
            hours = BigInteger.DivRem(minutes, 60, out minutes);
        }

        if (largestUnit <= TemporalUnit.Day)
        {
            days = BigInteger.DivRem(hours, 24, out hours);
        }

        double Signed(BigInteger value) => ToDouble(value * sign);

        return
        [
            duration.Date.Years,
            duration.Date.Months,
            duration.Date.Weeks,
            ToDouble(Exact(duration.Date.Days) + (days * sign)),
            Signed(hours),
            Signed(minutes),
            Signed(seconds),
            Signed(milliseconds),
            Signed(microseconds),
            Signed(nanoseconds),
        ];
    }

    // ---- Strings ------------------------------------------------------------------------------

    /// <summary>A non-negative integer with at least <paramref name="width"/> digits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44F8C9
    // Broiler-Human:        PENDING
    internal static string Padded(long value, int width) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(width, '0');

    /// <summary>The proposal's PadISOYear (s3.5.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9BA9F6
    // Broiler-Human:        PENDING
    internal static string PadYear(long year) =>
        year is >= 0 and <= 9999 ? Padded(year, 4) : (year > 0 ? "+" : "-") + Padded(System.Math.Abs(year), 6);

    /// <summary>
    /// The proposal's FormatFractionalSeconds (s13.25): <paramref name="precision"/> -1 is auto, 0 to
    /// 9 a digit count.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=50B3F1
    // Broiler-Human:        PENDING
    internal static string FractionalSeconds(long subSecondNanoseconds, int precision)
    {
        if (precision < 0)
        {
            if (subSecondNanoseconds == 0)
            {
                return string.Empty;
            }

            return "." + Padded(subSecondNanoseconds, 9).TrimEnd('0');
        }

        return precision == 0 ? string.Empty : "." + Padded(subSecondNanoseconds, 9)[..precision];
    }

    /// <summary>A precision of FormatTimeString: -1 auto, -2 minute, 0 to 9 digits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A7E08F
    // Broiler-Human:        PENDING
    internal const int PrecisionAuto = -1;

    /// <summary>The precision that writes no seconds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1C61BF
    // Broiler-Human:        PENDING
    internal const int PrecisionMinute = -2;

    /// <summary>The proposal's FormatTimeString (s13.26).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A02424
    // Broiler-Human:        PENDING
    internal static string FormatTime(int hour, int minute, int second, long subSecondNanoseconds, int precision, bool separated = true)
    {
        var separator = separated ? ":" : string.Empty;
        var text = Padded(hour, 2) + separator + Padded(minute, 2);

        if (precision == PrecisionMinute)
        {
            return text;
        }

        return text + separator + Padded(second, 2) + FractionalSeconds(subSecondNanoseconds, precision);
    }

    /// <summary>The proposal's TimeRecordToString (s4.5.13).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=63AE1F
    // Broiler-Human:        PENDING
    internal static string TimeToString(JsTimeRecord time, int precision) =>
        FormatTime(time.Hour, time.Minute, time.Second, SubSecond(time), precision);

    /// <summary>A time's nanoseconds within its second.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5C4386
    // Broiler-Human:        PENDING
    internal static long SubSecond(JsTimeRecord time) => (time.Millisecond * 1_000_000L) + (time.Microsecond * 1000L) + time.Nanosecond;

    /// <summary>An ISO date as <c>YYYY-MM-DD</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D93345
    // Broiler-Human:        PENDING
    internal static string DateToString(JsIsoDate date) =>
        PadYear(date.Year) + "-" + Padded(date.Month, 2) + "-" + Padded(date.Day, 2);

    /// <summary>The proposal's FormatOffsetTimeZoneIdentifier (s11.1.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E45B76
    // Broiler-Human:        PENDING
    internal static string FormatOffsetMinutes(long offsetMinutes, bool separated = true)
    {
        var sign = offsetMinutes >= 0 ? "+" : "-";
        var absolute = System.Math.Abs(offsetMinutes);
        return sign + FormatTime((int)(absolute / 60), (int)(absolute % 60), 0, 0, PrecisionMinute, separated);
    }

    /// <summary>The proposal's FormatUTCOffsetNanoseconds (s11.1.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9028D0
    // Broiler-Human:        PENDING
    internal static string FormatOffsetNs(long offsetNanoseconds)
    {
        var sign = offsetNanoseconds >= 0 ? "+" : "-";
        var absolute = System.Math.Abs(offsetNanoseconds);
        var hour = (int)(absolute / 3_600_000_000_000L);
        var minute = (int)(absolute / 60_000_000_000L % 60);
        var second = (int)(absolute / 1_000_000_000L % 60);
        var sub = absolute % 1_000_000_000L;
        return sign + FormatTime(hour, minute, second, sub, second == 0 && sub == 0 ? PrecisionMinute : PrecisionAuto);
    }

    /// <summary>The proposal's FormatDateTimeUTCOffsetRounded (s11.1.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C46986
    // Broiler-Human:        PENDING
    internal static string FormatOffsetRounded(long offsetNanoseconds)
    {
        var rounded = RoundToIncrement(offsetNanoseconds, new BigInteger(60_000_000_000L), TemporalRounding.HalfExpand);
        return FormatOffsetMinutes((long)(rounded / 60_000_000_000L));
    }

    /// <summary>The proposal's TemporalDurationToString (s7.5.40).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3A18A4
    // Broiler-Human:        PENDING
    internal static string DurationToString(System.ReadOnlySpan<double> fields, int precision)
    {
        var sign = DurationSign(fields);
        var datePart = new System.Text.StringBuilder();
        string[] dateLetters = ["Y", "M", "W", "D"];

        for (var i = 0; i < 4; i++)
        {
            if (fields[i] != 0)
            {
                datePart.Append(IntegerText(System.Math.Abs(fields[i]))).Append(dateLetters[i]);
            }
        }

        var timePart = new System.Text.StringBuilder();

        if (fields[4] != 0)
        {
            timePart.Append(IntegerText(System.Math.Abs(fields[4]))).Append('H');
        }

        if (fields[5] != 0)
        {
            timePart.Append(IntegerText(System.Math.Abs(fields[5]))).Append('M');
        }

        var zeroMinutesAndHigher = DefaultLargestUnit(fields) >= TemporalUnit.Second;
        var secondsDuration = TimeFromComponents(0, 0, fields[6], fields[7], fields[8], fields[9]);

        if (!secondsDuration.IsZero || zeroMinutesAndHigher || precision != PrecisionAuto)
        {
            var whole = BigInteger.Abs(BigInteger.DivRem(secondsDuration, 1_000_000_000, out var fraction));
            timePart.Append(whole.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(FractionalSeconds((long)BigInteger.Abs(fraction), precision))
                .Append('S');
        }

        var result = (sign < 0 ? "-" : string.Empty) + "P" + datePart;
        return timePart.Length == 0 ? result : result + "T" + timePart;
    }
}
