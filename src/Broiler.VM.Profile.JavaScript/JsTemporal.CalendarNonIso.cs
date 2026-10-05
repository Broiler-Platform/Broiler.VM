// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           0
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The calendar operations of the Intl era and month code proposal (its section 4.1) over
/// <see cref="JsCalendarSystem"/>: every calendar but ISO 8601, the Gregorian one included
/// (JSD-0056).
/// </summary>
/// <remarks>
/// <para>
/// <b>The proposal's difference loops are counted from estimates.</b> NonISODateUntil (s4.1.19) adds
/// a year, a month, a week and a day at a time until NonISODateSurpasses says the next would pass
/// the second date. That is a loop over hundreds of thousands of iterations for dates far apart, so
/// the years and months start from the difference of the two dates' years and month counts and move
/// until the same condition holds; the weeks and days, which the proposal reckons in days from the
/// last regulated date, are a subtraction.
/// </para>
/// <para>
/// <b>A year far outside the representable range is refused before it is computed.</b> No date in a
/// year more than <see cref="YearBound"/> from the epoch is within the limits of
/// ISODateWithinLimits, so every operation that would compute one throws the RangeError its limits
/// would; the order of its earlier TypeErrors is the proposal's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    /// <summary>A year bound beyond every calendar's years within the ISO limits (the Hijri ones reach 283,583).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=228960
    // Broiler-Human:        PENDING
    private const long YearBound = 300_000;

    /// <summary>-271821-04-19, the first day ISODateWithinLimits admits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D7FDA9
    // Broiler-Human:        PENDING
    private static readonly long MinDay = JsTemporalCore.EpochDays(-271821, 4, 19);

    /// <summary>275760-09-13, the last day ISODateWithinLimits admits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C17397
    // Broiler-Human:        PENDING
    private static readonly long MaxDay = JsTemporalCore.EpochDays(275760, 9, 13);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=271368
    // Broiler-Human:        PENDING
    private static JsCalendarYear CalendarYear(JsEngine engine, JsCalendarSystem system, double year)
    {
        if (System.Math.Abs(year) > YearBound)
        {
            throw engine.Error("RangeError", "Temporal: the year is outside the representable range");
        }

        return system.Year((long)year);
    }

    /// <summary>IsValidMonthCodeForCalendar (s4.1.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D07C04
    // Broiler-Human:        PENDING
    private static bool IsValidMonthCode(JsCalendarSystem system, string monthCode) =>
        (monthCode.Length == 3 && monthCode is not "M00" and not "M13" && string.CompareOrdinal(monthCode, "M12") <= 0) ||
        System.Array.IndexOf(system.AdditionalMonthCodes, monthCode) >= 0;

    /// <summary>ConstrainMonthCode (s4.1.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D41F7
    // Broiler-Human:        PENDING
    private static string ConstrainMonthCode(JsEngine engine, JsCalendarSystem system, JsCalendarYear year, string monthCode, bool reject)
    {
        if (JsCalendarSystem.Ordinal(year, monthCode) > 0)
        {
            return monthCode;
        }

        if (reject)
        {
            throw engine.Error("RangeError", "Temporal: the year " + year.Year + " of the " + system.Id + " calendar has no month " + monthCode);
        }

        return system.SkipBackward ? monthCode[..3] : "M06";
    }

    /// <summary>NonISOResolveFields (s4.1.24).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=363FDC
    // Broiler-Human:        PENDING
    private static void NonIsoResolveFields(JsEngine engine, string calendar, JsCalendarFields fields, FieldsType type)
    {
        var system = SystemOf(engine, calendar);
        var needsYear = type != FieldsType.MonthDay || fields.MonthCode is null || fields.Month is not null;
        var needsDay = type != FieldsType.YearMonth;

        if (needsYear && fields.Year is null && (!system.HasEras || fields.Era is null || fields.EraYear is null))
        {
            throw engine.Error("TypeError", "Temporal: the year property, or era and eraYear, is required");
        }

        if (system.HasEras && (fields.Era is null) != (fields.EraYear is null))
        {
            throw engine.Error("TypeError", "Temporal: era and eraYear must be given together");
        }

        if (needsDay && fields.Day is null)
        {
            throw engine.Error("TypeError", "Temporal: the day property is required");
        }

        if (fields.Month is null && fields.MonthCode is null)
        {
            throw engine.Error("TypeError", "Temporal: the month or monthCode property is required");
        }

        if (system.HasEras && fields.EraYear is { } eraYear)
        {
            if (System.Math.Abs(eraYear) > 2 * YearBound)
            {
                throw engine.Error("RangeError", "Temporal: the year is outside the representable range");
            }

            var year = system.YearForEra(fields.Era!, (long)eraYear) ??
                throw engine.Error("RangeError", "Temporal: " + fields.Era + " is not an era of the " + calendar + " calendar");

            if (fields.Year is { } given && given != year)
            {
                throw engine.Error("RangeError", "Temporal: the year and the era year disagree");
            }

            fields.Year = year;
            fields.Era = null;
            fields.EraYear = null;
        }

        if (fields.MonthCode is { } code)
        {
            if (!IsValidMonthCode(system, code))
            {
                throw engine.Error("RangeError", "Temporal: the " + calendar + " calendar has no month code " + code);
            }

            if (fields.Year is { } y)
            {
                var year = CalendarYear(engine, system, y);
                var month = JsCalendarSystem.Ordinal(year, ConstrainMonthCode(engine, system, year, code, reject: false));

                if (fields.Month is { } given && given != month)
                {
                    throw engine.Error("RangeError", "Temporal: month and monthCode disagree");
                }

                fields.Month = month;
            }
        }
    }

    /// <summary>NonISOCalendarDateToISO (s4.1.20).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0085A8
    // Broiler-Human:        PENDING
    private static JsIsoDate NonIsoDateToIso(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        var system = SystemOf(engine, calendar);
        var year = CalendarYear(engine, system, fields.Year!.Value);

        if (fields.MonthCode is { } code)
        {
            ConstrainMonthCode(engine, system, year, code, reject);
        }

        var month = Regulated(engine, fields.Month!.Value, year.Months, reject, "month");
        var day = Regulated(engine, fields.Day!.Value, year.DaysInMonth(month), reject, "day");
        return JsTemporalCore.FromEpochDays(year.Start + year.Offsets[month - 1] + day - 1);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=03A586
    // Broiler-Human:        PENDING
    private static int Regulated(JsEngine engine, double value, int maximum, bool reject, string what)
    {
        if (value <= maximum)
        {
            return (int)value;
        }

        if (reject)
        {
            throw engine.Error("RangeError", "Temporal: the " + what + " " + value + " is out of range");
        }

        return maximum;
    }

    /// <summary>NonISOMonthDayToISOReferenceDate (s4.1.21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7D9B4C
    // Broiler-Human:        PENDING
    private static JsIsoDate NonIsoMonthDayToReferenceDate(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        var system = SystemOf(engine, calendar);
        string monthCode;
        int daysInMonth;

        if (fields.Year is { } y)
        {
            var year = CalendarYear(engine, system, y);

            if (year.Start + year.Days <= MinDay || year.Start > MaxDay)
            {
                throw engine.Error("RangeError", "Temporal: the year is outside the representable range");
            }

            var month = Regulated(engine, fields.Month!.Value, year.Months, reject, "month");
            monthCode = fields.MonthCode is { } code
                ? ConstrainMonthCode(engine, system, year, code, reject)
                : JsCalendarSystem.MonthCode(year, month);
            daysInMonth = year.DaysInMonth(month);
        }
        else
        {
            monthCode = fields.MonthCode!;
            daysInMonth = system is JsEastAsianCalendar ? 30 : MaxDaysInMonth(system, monthCode);
        }

        var day = Regulated(engine, fields.Day!.Value, daysInMonth, reject, "day");

        if (system is JsEastAsianCalendar)
        {
            if (system.TableReferenceYear(monthCode, day) is not > 0)
            {
                if (reject)
                {
                    throw engine.Error("RangeError", "Temporal: no year of the " + calendar + " calendar is known to have " + monthCode + " day " + day);
                }

                monthCode = monthCode[..3];
            }

            var isoYear = system.TableReferenceYear(monthCode, day)!.Value;
            return ReferenceDate(system, monthCode, day, JsTemporalCore.EpochDays(isoYear, 1, 1), JsTemporalCore.EpochDays(isoYear, 12, 31), latest: true) ??
                throw engine.Error("RangeError", "Temporal: the reference year " + isoYear + " has no " + monthCode + " day " + day);
        }

        return ReferenceDate(system, monthCode, day, JsTemporalCore.EpochDays(1900, 1, 1), JsTemporalCore.EpochDays(1972, 12, 31), latest: true) ??
            ReferenceDate(system, monthCode, day, JsTemporalCore.EpochDays(1973, 1, 1), JsTemporalCore.EpochDays(2035, 12, 31), latest: false) ??
            throw engine.Error("RangeError", "Temporal: no year of the " + calendar + " calendar has " + monthCode + " day " + day);
    }

    /// <summary>The latest or earliest ISO date between two days whose month code and day are the given ones.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8EDD12
    // Broiler-Human:        PENDING
    private static JsIsoDate? ReferenceDate(JsCalendarSystem system, string monthCode, int day, long from, long to, bool latest)
    {
        var step = latest ? -1 : 1;

        for (var y = system.YearOf(latest ? to : from); ; y += step)
        {
            var year = system.Year(y);

            if (latest ? year.Start + year.Days <= from : year.Start > to)
            {
                return null;
            }

            var ordinal = JsCalendarSystem.Ordinal(year, monthCode);

            if (ordinal > 0 && day <= year.DaysInMonth(ordinal))
            {
                var days = year.Start + year.Offsets[ordinal - 1] + day - 1;

                if (days >= from && days <= to)
                {
                    return JsTemporalCore.FromEpochDays(days);
                }
            }
        }
    }

    /// <summary>The most days a month code has in any year (s4.1.21), from the years 1900 to 2035.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4FADE8
    // Broiler-Human:        PENDING
    private static int MaxDaysInMonth(JsCalendarSystem system, string monthCode)
    {
        var most = 0;
        var last = system.YearOf(JsTemporalCore.EpochDays(2035, 12, 31));

        for (var y = system.YearOf(JsTemporalCore.EpochDays(1900, 1, 1)); y <= last; y++)
        {
            var year = system.Year(y);
            var ordinal = JsCalendarSystem.Ordinal(year, monthCode);

            if (ordinal > 0)
            {
                most = System.Math.Max(most, year.DaysInMonth(ordinal));
            }
        }

        return most;
    }

    /// <summary>
    /// BalanceNonISODate's months (s4.1.16): the year and ordinal month a possibly out-of-range
    /// month of a year names, or null when that year is beyond <see cref="YearBound"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E237C
    // Broiler-Human:        PENDING
    private static (long Year, int Month)? BalanceMonths(JsCalendarSystem system, long year, long month)
    {
        var estimate = year + (long)System.Math.Floor((month - 1) / system.MeanMonths);

        if (System.Math.Abs(estimate) > YearBound)
        {
            return null;
        }

        var total = system.MonthsBefore(year) + month - 1;
        var y = estimate;

        while (system.MonthsBefore(y) > total)
        {
            y--;
        }

        while (system.MonthsBefore(y + 1) <= total)
        {
            y++;
        }

        return (y, (int)(total - system.MonthsBefore(y)) + 1);
    }

    /// <summary>NonISODateAdd (s4.1.18).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=614369
    // Broiler-Human:        PENDING
    private static JsIsoDate NonIsoDateAdd(JsEngine engine, string calendar, JsIsoDate date, JsDateDuration duration, bool reject)
    {
        var system = SystemOf(engine, calendar);
        var (year, month, day) = system.FromEpochDays(JsTemporalCore.EpochDays(date));
        var y0 = CalendarYear(engine, system, year.Year + duration.Years);
        var m0 = JsCalendarSystem.Ordinal(y0, ConstrainMonthCode(engine, system, y0, JsCalendarSystem.MonthCode(year, month), reject));

        var (endYear, endMonth) = BalanceMonths(system, y0.Year, m0 + (long)duration.Months) ??
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        var target = system.Year(endYear);
        var regulated = Regulated(engine, day, target.DaysInMonth(endMonth), reject, "day");
        var days = target.Start + target.Offsets[endMonth - 1] + regulated - 1 + (7 * (long)duration.Weeks) + (long)duration.Days;

        if (days < MinDay || days > MaxDay)
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        return JsTemporalCore.FromEpochDays(days);
    }

    /// <summary>A Calendar Date Record's year, month code, ordinal month and day, for CompareSurpasses.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B70481
    // Broiler-Human:        PENDING
    private readonly record struct CalendarPoint(long Year, string MonthCode, int Month, int Day);

    /// <summary>The proposal's CompareSurpasses (s3.5.5) with a month code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E32853
    // Broiler-Human:        PENDING
    private static bool SurpassesByCode(int sign, long year, string monthCode, int day, CalendarPoint target)
    {
        if (year != target.Year)
        {
            return sign * (year - target.Year) > 0;
        }

        if (monthCode != target.MonthCode)
        {
            return sign > 0
                ? string.CompareOrdinal(monthCode, target.MonthCode) > 0
                : string.CompareOrdinal(target.MonthCode, monthCode) > 0;
        }

        return day != target.Day && sign * (day - target.Day) > 0;
    }

    /// <summary>The proposal's CompareSurpasses (s3.5.5) with an ordinal month.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B34CD
    // Broiler-Human:        PENDING
    private static bool SurpassesByMonth(int sign, long year, int month, int day, CalendarPoint target)
    {
        if (year != target.Year)
        {
            return sign * (year - target.Year) > 0;
        }

        if (month != target.Month)
        {
            return sign * (month - target.Month) > 0;
        }

        return day != target.Day && sign * (day - target.Day) > 0;
    }

    /// <summary>NonISODateUntil (s4.1.19), its loops counted from estimates.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BA4155
    // Broiler-Human:        PENDING
    private static JsDateDuration NonIsoDateUntil(JsEngine engine, string calendar, JsIsoDate one, JsIsoDate two, TemporalUnit largestUnit, int sign)
    {
        var system = SystemOf(engine, calendar);
        var oneDays = JsTemporalCore.EpochDays(one);
        var twoDays = JsTemporalCore.EpochDays(two);
        long years = 0;
        long months = 0;
        var intermediate = oneDays;

        if (largestUnit is TemporalUnit.Year or TemporalUnit.Month)
        {
            var (year1, month1, day1) = system.FromEpochDays(oneDays);
            var (year2, month2, day2) = system.FromEpochDays(twoDays);
            var code1 = JsCalendarSystem.MonthCode(year1, month1);
            var target = new CalendarPoint(year2.Year, JsCalendarSystem.MonthCode(year2, month2), month2, day2);

            // NonISODateSurpasses (s4.1.17) for years and months; the start of the months it adds.
            (long Year, int Month) Start(long y)
            {
                var year = system.Year(year1.Year + y);
                return (year.Year, JsCalendarSystem.Ordinal(year, ConstrainMonthCode(engine, system, year, code1, reject: false)));
            }

            bool Surpasses(long y, long m)
            {
                if (SurpassesByCode(sign, year1.Year + y, code1, day1, target))
                {
                    return true;
                }

                var (startYear, startMonth) = Start(y);
                var (added, addedMonth) = BalanceMonths(system, startYear, startMonth + m)!.Value;
                return SurpassesByMonth(sign, added, addedMonth, day1, target);
            }

            long Largest(long estimate, System.Func<long, bool> surpasses)
            {
                var count = estimate;

                while (count != 0 && surpasses(count))
                {
                    count -= sign;
                }

                while (!surpasses(count + sign))
                {
                    count += sign;
                }

                return count;
            }

            if (largestUnit == TemporalUnit.Year)
            {
                years = Largest(year2.Year - year1.Year, y => Surpasses(y, 0));
            }

            var (fromYear, fromMonth) = Start(years);
            var estimate = system.MonthsBefore(year2.Year) + month2 - (system.MonthsBefore(fromYear) + fromMonth);
            months = Largest(estimate, m => Surpasses(years, m));

            var (endYear, endMonth) = BalanceMonths(system, fromYear, fromMonth + months)!.Value;
            var end = system.Year(endYear);
            intermediate = end.Start + end.Offsets[endMonth - 1] + System.Math.Min(day1, end.DaysInMonth(endMonth)) - 1;
        }

        var days = twoDays - intermediate;
        long weeks = 0;

        if (largestUnit == TemporalUnit.Week)
        {
            weeks = days / 7;
            days %= 7;
        }

        return new JsDateDuration(years, months, weeks, days);
    }

    /// <summary>NonISOCalendarISOToDate (Temporal s12.3.25, the Intl era and month code proposal's s4.1.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DDEA47
    // Broiler-Human:        PENDING
    private static JsCalendarDate NonIsoToDate(JsEngine engine, string calendar, JsIsoDate date)
    {
        var system = SystemOf(engine, calendar);
        var days = JsTemporalCore.EpochDays(date);
        var (year, month, day) = system.FromEpochDays(days);
        var (era, eraYear) = system.EraOf(year.Year, days);

        return new JsCalendarDate(
            era,
            eraYear,
            year.Year,
            month,
            JsCalendarSystem.MonthCode(year, month),
            day,
            JsTemporalCore.DayOfWeek(date),
            (int)(days - year.Start) + 1,
            null,
            null,
            year.DaysInMonth(month),
            year.Days,
            year.Months,
            system.InLeapYear(year));
    }
}
