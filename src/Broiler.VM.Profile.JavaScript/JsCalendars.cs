// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   90
// Annotated:        90/90
// Exempt:           32
// Human-reviewed:   0/90
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       90
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One year of a calendar other than ISO 8601: its arithmetic year, its first day in days since
/// 1970-01-01, the days before each of its months, and its leap month's ordinal (0 for none).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=53E05B
// Broiler-Human:        PENDING
internal sealed class JsCalendarYear
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1CC13E
    // Broiler-Human:        PENDING
    internal JsCalendarYear(long year, long start, int[] offsets, int leapMonth = 0)
    {
        Year = year;
        Start = start;
        Offsets = offsets;
        LeapMonth = leapMonth;
    }

    /// <summary>The arithmetic year (the Calendar Date Record's [[Year]]).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9F6E91
    // Broiler-Human:        PENDING
    internal long Year { get; }

    /// <summary>The year's first day, in days since 1970-01-01.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C28BFF
    // Broiler-Human:        PENDING
    internal long Start { get; }

    /// <summary><c>Offsets[m - 1]</c> is the days before month <c>m</c>; the last element is the year's length.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C6A241
    // Broiler-Human:        PENDING
    internal int[] Offsets { get; }

    /// <summary>The ordinal of the year's leap month, or 0.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8122D2
    // Broiler-Human:        PENDING
    internal int LeapMonth { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D78382
    // Broiler-Human:        PENDING
    internal int Months => Offsets.Length - 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1AE161
    // Broiler-Human:        PENDING
    internal int Days => Offsets[^1];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=663BB0
    // Broiler-Human:        PENDING
    internal int DaysInMonth(int month) => Offsets[month] - Offsets[month - 1];

    /// <summary>The month and day of a day of the year, 1-based.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=876A3C
    // Broiler-Human:        PENDING
    internal (int Month, int Day) MonthDay(int dayOfYear)
    {
        var month = 1;

        while (month < Months && Offsets[month] < dayOfYear)
        {
            month++;
        }

        return (month, dayOfYear - Offsets[month - 1]);
    }

    /// <summary>A year the shared library's calendar year sources answer (JSD-0076), as the arithmetic reads it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=179284
    // Broiler-Human:        PENDING
    internal static JsCalendarYear Of(CalendarYear year)
    {
        Span<int> lengths = stackalloc int[year.MonthsInYear];

        for (var month = 1; month <= year.MonthsInYear; month++)
        {
            lengths[month - 1] = year.DaysInMonth(month);
        }

        return new JsCalendarYear(year.Year, year.NewYearEpochDays, FromLengths(lengths), year.LeapMonth);
    }

    /// <summary>Offsets from month lengths.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=306813
    // Broiler-Human:        PENDING
    internal static int[] FromLengths(ReadOnlySpan<int> lengths)
    {
        var offsets = new int[lengths.Length + 1];

        for (var i = 0; i < lengths.Length; i++)
        {
            offsets[i + 1] = offsets[i] + lengths[i];
        }

        return offsets;
    }
}

/// <summary>
/// The arithmetic of one calendar the Intl era and month code proposal requires (its Table 1),
/// other than ISO 8601: how its years divide into months, which month codes and eras it has, and
/// how its dates map to days (JSD-0056).
/// </summary>
/// <remarks>
/// <para>
/// <b>Everything is in days since 1970-01-01</b>, as ISO dates are elsewhere in the profile; the
/// algorithms that are stated in Calendrical Calculations' fixed days (R.D.) convert at
/// <see cref="RataDieOfEpoch"/>.
/// </para>
/// <para>
/// <b>A year is computed when it is asked for</b> and nothing is cached: the arithmetic calendars
/// take a few multiplications, and the lunisolar ones a table lookup or a few dozen additions.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3493EB
// Broiler-Human:        PENDING
internal abstract class JsCalendarSystem(string id)
{
    /// <summary>R.D. 719163 is 1970-01-01.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A9315E
    // Broiler-Human:        PENDING
    internal const long RataDieOfEpoch = 719163;

    /// <summary>The canonical calendar type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CC7042
    // Broiler-Human:        PENDING
    internal string Id { get; } = id;

    /// <summary>The year whose arithmetic year is <paramref name="year"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=311D20
    // Broiler-Human:        PENDING
    internal abstract JsCalendarYear Year(long year);

    /// <summary>The arithmetic year containing a day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F451E6
    // Broiler-Human:        PENDING
    internal abstract long YearOf(long epochDays);

    /// <summary>Whether a year is a leap year (s4.1.14, note 7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E8326B
    // Broiler-Human:        PENDING
    internal abstract bool InLeapYear(JsCalendarYear year);

    /// <summary>The month codes beyond M01 to M12 (Table 3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A05149
    // Broiler-Human:        PENDING
    internal virtual string[] AdditionalMonthCodes => [];

    /// <summary>Whether a missing leap month becomes the common month of its number (Table 3's skip-backward) rather than the next.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=84819E
    // Broiler-Human:        PENDING
    internal virtual bool SkipBackward => true;

    /// <summary>The months between a fixed origin and the first month of a year, for balancing months into years.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2E17F4
    // Broiler-Human:        PENDING
    internal virtual long MonthsBefore(long year) => 12 * year;

    /// <summary>
    /// Whether <see cref="MonthsBefore"/> reads the calendar's years, which the shared library's sources
    /// answer only within <see cref="CalendarYears.MaxYearMagnitude"/>: the Chinese and Korean calendars.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=62A054
    // Broiler-Human:        PENDING
    internal virtual bool MonthsBeforeReadsYears => false;

    /// <summary>The mean months in a year, for estimating which year a month count falls in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=29859C
    // Broiler-Human:        PENDING
    internal virtual double MeanMonths => 12;

    /// <summary>Whether the calendar has eras (CalendarSupportsEra, s4.1.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2C9F31
    // Broiler-Human:        PENDING
    internal virtual bool HasEras => false;

    /// <summary>Whether an era begins within a year (CalendarHasMidYearEras, s4.1.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2C7A03
    // Broiler-Human:        PENDING
    internal virtual bool MidYearEras => false;

    /// <summary>The era and era year of a date (s4.1.9, s4.1.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=05A6DD
    // Broiler-Human:        PENDING
    internal virtual (string? Era, long? EraYear) EraOf(long year, long epochDays) => (null, null);

    /// <summary>
    /// CalendarDateArithmeticYearForEraYear (s4.1.12) after CanonicalizeEraInCalendar (s4.1.2):
    /// the arithmetic year, or null when the era is not one of the calendar's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B57613
    // Broiler-Human:        PENDING
    internal virtual long? YearForEra(string era, long eraYear) => null;

    /// <summary>The ISO reference years Table 6 states, for the lunisolar calendars it covers; null elsewhere.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F442C8
    // Broiler-Human:        PENDING
    internal virtual int? TableReferenceYear(string monthCode, int day) => null;

    /// <summary>The year, ordinal month and day of a day since the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C58FFE
    // Broiler-Human:        PENDING
    internal (JsCalendarYear Year, int Month, int Day) FromEpochDays(long epochDays)
    {
        var year = Year(YearOf(epochDays));
        var (month, day) = year.MonthDay((int)(epochDays - year.Start) + 1);
        return (year, month, day);
    }

    /// <summary>The month code of an ordinal month (s4.1.14, [[MonthCode]]).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1A1E23
    // Broiler-Human:        PENDING
    internal static string MonthCode(JsCalendarYear year, int month)
    {
        var leap = year.LeapMonth;

        if (leap == 0 || month < leap)
        {
            return Code(month, false);
        }

        return month == leap ? Code(month - 1, true) : Code(month - 1, false);
    }

    /// <summary>MonthCodeToOrdinal (s4.1.7), or 0 where the year does not contain the code (s4.1.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A375C5
    // Broiler-Human:        PENDING
    internal static int Ordinal(JsCalendarYear year, string monthCode)
    {
        var number = ((monthCode[1] - '0') * 10) + (monthCode[2] - '0');
        var isLeap = monthCode.Length == 4;
        var leap = year.LeapMonth;

        if (isLeap)
        {
            return leap != 0 && leap - 1 == number ? leap : 0;
        }

        var ordinal = leap != 0 && number >= leap ? number + 1 : number;
        return ordinal <= year.Months ? ordinal : 0;
    }

    /// <summary>CreateMonthCode (s12.2.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2CC5EB
    // Broiler-Human:        PENDING
    internal static string Code(int number, bool leap) => "M" + (number < 10 ? "0" : string.Empty) + number.ToString(CultureInfo.InvariantCulture) + (leap ? "L" : string.Empty);

    /// <summary>Floor division.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0DDA64
    // Broiler-Human:        PENDING
    internal static long FloorDiv(long dividend, long divisor)
    {
        var quotient = dividend / divisor;
        return (dividend % divisor != 0) && ((dividend < 0) != (divisor < 0)) ? quotient - 1 : quotient;
    }

    /// <summary>
    /// A table-backed calendar's year from the shared library's sources (JSD-0076): the table the Intl
    /// data hands in inside its range, and ICU4X's approximation, the civil tabular year or the 33-year
    /// rule outside it. Every caller keeps the year within the sources' bound, which is
    /// <c>JsTemporal.YearBound</c>, beyond every year that holds a date within the ISO limits.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CA9B70
    // Broiler-Human:        PENDING
    protected static JsCalendarYear SharedYear(ICalendarTables tables, TableCalendar calendar, long year) =>
        CalendarYears.GetYear(tables, calendar, year, out var result) == TimeStatus.Completed
            ? JsCalendarYear.Of(result)
            : throw new InvalidOperationException("The year " + year + " is beyond the calendar year sources' bound, which every caller keeps.");

    /// <summary>The year an estimate names, moved until it contains the day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1FB334
    // Broiler-Human:        PENDING
    protected long AdjustYear(long estimate, long epochDays)
    {
        var year = estimate;

        while (Year(year).Start > epochDays)
        {
            year--;
        }

        while (Year(year + 1).Start <= epochDays)
        {
            year++;
        }

        return year;
    }
}

/// <summary>
/// The calendars whose months and days are ISO 8601's: <c>gregory</c>, <c>buddhist</c>, <c>roc</c>
/// and <c>japanese</c>, each with its own year numbering and eras (Table 2).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C5F397
// Broiler-Human:        PENDING
internal sealed class JsGregorianCalendar : JsCalendarSystem
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=234F8C
    // Broiler-Human:        PENDING
    private readonly long isoOffset;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6B16E3
    // Broiler-Human:        PENDING
    internal JsGregorianCalendar(string id, long isoOffset) : base(id)
    {
        this.isoOffset = isoOffset;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0CE51F
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year)
    {
        var iso = year + isoOffset;
        var lengths = new int[12];

        for (var month = 1; month <= 12; month++)
        {
            lengths[month - 1] = IsoCalendarMath.DaysInMonth(iso, month);
        }

        return new JsCalendarYear(year, IsoCalendarMath.EpochDays(iso, 1, 1), JsCalendarYear.FromLengths(lengths));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=136932
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) => IsoCalendarMath.FromEpochDays(epochDays).Year - isoOffset;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=93750F
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Days == 366;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AEE81B
    // Broiler-Human:        PENDING
    internal override bool MidYearEras => Id == "japanese";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C272C9
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays)
    {
        switch (Id)
        {
            case "buddhist":
                return ("be", year);
            case "roc":
                return year >= 1 ? ("roc", year) : ("broc", 1 - year);
            case "japanese":
                // THE MODERN ERAS by the date each began (Table 2's offsets), then Gregorian years.
                if (epochDays >= IsoCalendarMath.EpochDays(2019, 5, 1))
                {
                    return ("reiwa", year - 2018);
                }

                if (epochDays >= IsoCalendarMath.EpochDays(1989, 1, 8))
                {
                    return ("heisei", year - 1988);
                }

                if (epochDays >= IsoCalendarMath.EpochDays(1926, 12, 25))
                {
                    return ("showa", year - 1925);
                }

                if (epochDays >= IsoCalendarMath.EpochDays(1912, 7, 30))
                {
                    return ("taisho", year - 1911);
                }

                if (epochDays >= IsoCalendarMath.EpochDays(1873, 1, 1))
                {
                    return ("meiji", year - 1867);
                }

                goto default;
            default:
                return year >= 1 ? ("ce", year) : ("bce", 1 - year);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C8702F
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => (Id, era) switch
    {
        ("buddhist", "be") => eraYear,
        ("roc", "roc") => eraYear,
        ("roc", "broc") => 1 - eraYear,
        ("japanese", "reiwa") => 2019 + eraYear - 1,
        ("japanese", "heisei") => 1989 + eraYear - 1,
        ("japanese", "showa") => 1926 + eraYear - 1,
        ("japanese", "taisho") => 1912 + eraYear - 1,
        ("japanese", "meiji") => 1868 + eraYear - 1,
        ("gregory" or "japanese", "ce" or "ad") => eraYear,
        ("gregory" or "japanese", "bce" or "bc") => 1 - eraYear,
        _ => null,
    };
}

/// <summary>
/// The Coptic and Ethiopian calendars: twelve months of 30 days and a thirteenth of five, or six
/// in the year before a year divisible by four (Calendrical Calculations, ch. 4).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0F193F
// Broiler-Human:        PENDING
internal sealed class JsCopticCalendar : JsCalendarSystem
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75FB94
    // Broiler-Human:        PENDING
    private readonly long epoch;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6B8E73
    // Broiler-Human:        PENDING
    private readonly long yearShift;

    /// <param name="id">The calendar type.</param>
    /// <param name="epochRataDie">The R.D. of the first day of year 1 of the reckoning.</param>
    /// <param name="yearShift">What the calendar adds to the reckoning's year: 5500 for <c>ethioaa</c>.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E07681
    // Broiler-Human:        PENDING
    internal JsCopticCalendar(string id, long epochRataDie, long yearShift)
        : base(id)
    {
        epoch = epochRataDie - RataDieOfEpoch;
        this.yearShift = yearShift;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F0C6AE
    // Broiler-Human:        PENDING
    internal override string[] AdditionalMonthCodes => ["M13"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9E57C4
    // Broiler-Human:        PENDING
    internal override long MonthsBefore(long year) => 13 * year;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9ACC94
    // Broiler-Human:        PENDING
    internal override double MeanMonths => 13;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D2091
    // Broiler-Human:        PENDING
    private long NewYear(long year)
    {
        var y = year - yearShift;
        return epoch + (365 * (y - 1)) + FloorDiv(y, 4);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3D1E27
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year)
    {
        var days = (int)(NewYear(year + 1) - NewYear(year));
        var offsets = new int[14];

        for (var month = 1; month <= 13; month++)
        {
            offsets[month] = month == 13 ? days : 30 * month;
        }

        return new JsCalendarYear(year, NewYear(year), offsets);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AA49D2
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) =>
        FloorDiv((4 * (epochDays - epoch)) + 1463, 1461) + yearShift;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=93750F
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Days == 366;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A3E71F
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays) => Id switch
    {
        "coptic" => ("am", year),
        "ethioaa" => ("aa", year),
        _ => year >= 1 ? ("am", year) : ("aa", year + 5500),
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C99EEA
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => (Id, era) switch
    {
        ("coptic", "am") => eraYear,
        ("ethioaa", "aa") => eraYear,
        ("ethiopic", "am") => eraYear,
        ("ethiopic", "aa") => -5499 + eraYear - 1,
        _ => null,
    };
}

/// <summary>
/// The Indian national calendar: Saka years beginning on 22 March, or 21 March in a Gregorian leap
/// year, of the ISO year 78 later; Chaitra has 30 days, or 31 in a leap year.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F4144C
// Broiler-Human:        PENDING
internal sealed class JsIndianCalendar : JsCalendarSystem
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=764D9A
    // Broiler-Human:        PENDING
    internal JsIndianCalendar() : base("indian") { }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B1C06D
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year)
    {
        var iso = year + 78;
        var leap = IsoCalendarMath.IsLeapYear(iso);
        ReadOnlySpan<int> lengths = [leap ? 31 : 30, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 30];
        return new JsCalendarYear(year, IsoCalendarMath.EpochDays(iso, 3, leap ? 21 : 22), JsCalendarYear.FromLengths(lengths));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6628AC
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) => AdjustYear(IsoCalendarMath.FromEpochDays(epochDays).Year - 78, epochDays);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=93750F
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Days == 366;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=11003C
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays) => ("shaka", year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EAFE18
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => era == "shaka" ? eraYear : null;
}

/// <summary>
/// The Persian (Solar Hijri) calendar: six months of 31 days, five of 30 and Esfand of 29, or 30 in
/// a leap year, which the 33-year rule decides, corrected after 1501 AP by the years the Intl data's
/// calendar table lists (Calendrical Calculations, ch. 15). Its years are the shared library's since
/// SJL-8.5 (JSD-0076); the arithmetic over them is this family's.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE63C7
// Broiler-Human:        PENDING
internal sealed class JsPersianCalendar : JsCalendarSystem
{
    /// <summary>R.D. 226896, Julian 622-03-19, the first day of 1 AP.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=970A38
    // Broiler-Human:        PENDING
    private const long Epoch = 226896 - RataDieOfEpoch;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4DD1E4
    // Broiler-Human:        PENDING
    private readonly ICalendarTables tables;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A5FDDA
    // Broiler-Human:        PENDING
    internal JsPersianCalendar(ICalendarTables tables) : base("persian") => this.tables = tables;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A93DD2
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year) => SharedYear(tables, TableCalendar.Persian, year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E6F78E
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) =>
        AdjustYear(1 + FloorDiv((33 * (epochDays - Epoch + 1)) + 3, 12053), epochDays);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=93750F
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Days == 366;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8C64D5
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays) => ("ap", year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=56AC9C
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => era == "ap" ? eraYear : null;
}

/// <summary>
/// The Hijri calendars: the tabular one with leap years 2, 5, 7, 10, 13, 16, 18, 21, 24, 26 and 29
/// of each 30, from the civil (Friday) or the astronomical (Thursday) epoch; and Umm al-Qura, whose
/// years 1300 to 1600 AH are KACST's, from the Intl data's calendar table, and whose other years are
/// the civil tabular ones, both the shared library's since SJL-8.5 (JSD-0076).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FB8D4F
// Broiler-Human:        PENDING
internal sealed class JsHijriCalendar : JsCalendarSystem
{
    /// <summary>R.D. 227015, Julian 622-07-16, a Friday: the civil epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A3EBF7
    // Broiler-Human:        PENDING
    internal const long CivilEpoch = 227015 - RataDieOfEpoch;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75FB94
    // Broiler-Human:        PENDING
    private readonly long epoch;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6C064F
    // Broiler-Human:        PENDING
    private readonly ICalendarTables? ummAlQura;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=23FD7B
    // Broiler-Human:        PENDING
    internal JsHijriCalendar(string id, bool thursday, ICalendarTables? ummAlQura = null)
        : base(id)
    {
        epoch = CivilEpoch - (thursday ? 1 : 0);
        this.ummAlQura = ummAlQura;
    }

    /// <summary>The tabular new year of a year from an epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=18843C
    // Broiler-Human:        PENDING
    internal static long TabularNewYear(long epoch, long year) =>
        epoch + ((year - 1) * 354) + FloorDiv(3 + (11 * year), 30);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0B7D3B
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year)
    {
        if (ummAlQura is not null)
        {
            return SharedYear(ummAlQura, TableCalendar.IslamicUmmAlQura, year);
        }

        var lengths = new int[12];

        for (var month = 1; month <= 12; month++)
        {
            lengths[month - 1] = month % 2 == 1 ? 30 : 29;
        }

        if ((((14 + (11 * year)) % 30) + 30) % 30 < 11)
        {
            lengths[11] = 30;
        }

        return new JsCalendarYear(year, TabularNewYear(epoch, year), JsCalendarYear.FromLengths(lengths));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FB2370
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) =>
        AdjustYear(FloorDiv((30 * (epochDays - epoch)) + 10646, 10631), epochDays);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ACB0AA
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Days == 355;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1EE6E0
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays) =>
        year >= 1 ? ("ah", year) : ("bh", 1 - year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBE1BB
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => era switch
    {
        "ah" => eraYear,
        "bh" => 1 - eraYear,
        _ => null,
    };
}

/// <summary>
/// The Hebrew calendar, civil year from Tishrei: the molad of Tishrei and the four postponements
/// decide the new year, and Adar I is inserted after Shevat in years 3, 6, 8, 11, 14, 17 and 19 of
/// each 19 (Calendrical Calculations, ch. 8).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BBED31
// Broiler-Human:        PENDING
internal sealed class JsHebrewCalendar : JsCalendarSystem
{
    /// <summary>R.D. -1373427, Julian 3761 BCE 10-07: the epoch of the Hebrew calendar.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0E196E
    // Broiler-Human:        PENDING
    private const long Epoch = -1373427 - RataDieOfEpoch;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C21E68
    // Broiler-Human:        PENDING
    internal JsHebrewCalendar() : base("hebrew") { }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=464F55
    // Broiler-Human:        PENDING
    internal override string[] AdditionalMonthCodes => ["M05L"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9E05CF
    // Broiler-Human:        PENDING
    internal override bool SkipBackward => false;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B7BD58
    // Broiler-Human:        PENDING
    internal override long MonthsBefore(long year) => FloorDiv((235 * year) - 234, 19);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=961EA9
    // Broiler-Human:        PENDING
    internal override double MeanMonths => 235.0 / 19;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5727D4
    // Broiler-Human:        PENDING
    private static long ElapsedDays(long year)
    {
        var months = FloorDiv((235 * year) - 234, 19);
        var parts = 12084 + (13753 * months);
        var days = (29 * months) + FloorDiv(parts, 25920);
        return ((((3 * (days + 1)) % 7) + 7) % 7) < 3 ? days + 1 : days;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=98A807
    // Broiler-Human:        PENDING
    private static long NewYear(long year)
    {
        var previous = ElapsedDays(year - 1);
        var current = ElapsedDays(year);
        var next = ElapsedDays(year + 1);
        var correction = next - current == 356 ? 2 : current - previous == 382 ? 1 : 0;
        return Epoch + current + correction;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED0B33
    // Broiler-Human:        PENDING
    private static bool IsLeap(long year) => ((((7 * year) + 1) % 19) + 19) % 19 < 7;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E6B4A
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year)
    {
        var start = NewYear(year);
        var days = (int)(NewYear(year + 1) - start);
        var heshvan = days % 10 == 5 ? 30 : 29;
        var kislev = days % 10 == 3 ? 29 : 30;

        if (IsLeap(year))
        {
            ReadOnlySpan<int> leap = [30, heshvan, kislev, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29];
            return new JsCalendarYear(year, start, JsCalendarYear.FromLengths(leap), leapMonth: 6);
        }

        ReadOnlySpan<int> common = [30, heshvan, kislev, 29, 30, 29, 30, 29, 30, 29, 30, 29];
        return new JsCalendarYear(year, start, JsCalendarYear.FromLengths(common));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=186249
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays) =>
        AdjustYear(FloorDiv((epochDays - Epoch) * 98496, 35975351) + 1, epochDays);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=868169
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Months == 13;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D3A57
    // Broiler-Human:        PENDING
    internal override bool HasEras => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C2953
    // Broiler-Human:        PENDING
    internal override (string? Era, long? EraYear) EraOf(long year, long epochDays) => ("am", year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5C8D27
    // Broiler-Human:        PENDING
    internal override long? YearForEra(string era, long eraYear) => era == "am" ? eraYear : null;
}

/// <summary>
/// The Chinese and Korean calendars: lunar months beginning on the day of the new moon in Beijing
/// or Korean time, a leap month inserted in a year without a major solar term. Years 1900 to 2102
/// are the published ones the Intl data's calendar table holds; other years follow ICU4X's
/// mean-motion approximation, as Table 1 allows. Both are the shared library's since SJL-8.5
/// (JSD-0076); the arithmetic over the years is this family's.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=05A813
// Broiler-Human:        PENDING
internal sealed class JsEastAsianCalendar : JsCalendarSystem
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0DAD3C
    // Broiler-Human:        PENDING
    private readonly bool korean;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4DD1E4
    // Broiler-Human:        PENDING
    private readonly ICalendarTables tables;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5FAF4F
    // Broiler-Human:        PENDING
    private readonly long anchor;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=20A320
    // Broiler-Human:        PENDING
    internal JsEastAsianCalendar(string id, ICalendarTables tables) : base(id)
    {
        korean = id == "dangi";
        this.tables = tables;
        anchor = Year(2000).Start;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5692DB
    // Broiler-Human:        PENDING
    internal override string[] AdditionalMonthCodes =>
        ["M01L", "M02L", "M03L", "M04L", "M05L", "M06L", "M07L", "M08L", "M09L", "M10L", "M11L", "M12L"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7CC6F9
    // Broiler-Human:        PENDING
    internal override double MeanMonths => 365.2425 / 29.530588853;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED710E
    // Broiler-Human:        PENDING
    internal override long MonthsBefore(long year) =>
        (long)Math.Round((Year(year).Start - anchor) / 29.530588853, MidpointRounding.AwayFromZero);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AB95CD
    // Broiler-Human:        PENDING
    internal override bool MonthsBeforeReadsYears => true;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=21EEE7
    // Broiler-Human:        PENDING
    internal override JsCalendarYear Year(long year) =>
        SharedYear(tables, korean ? TableCalendar.Dangi : TableCalendar.Chinese, year);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E7249
    // Broiler-Human:        PENDING
    internal override long YearOf(long epochDays)
    {
        var related = IsoCalendarMath.FromEpochDays(epochDays).Year;
        return Year(related).Start > epochDays ? related - 1 : related;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=868169
    // Broiler-Human:        PENDING
    internal override bool InLeapYear(JsCalendarYear year) => year.Months == 13;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=36C7B4
    // Broiler-Human:        PENDING
    internal override int? TableReferenceYear(string monthCode, int day)
    {
        // TABLE 6: "-1" IS ITS DASH.
        var years = monthCode switch
        {
            "M01" => (1972, 1970),
            "M01L" => (-1, -1),
            "M02" => (1972, 1972),
            "M02L" => (1947, -1),
            "M03" => (1972, korean ? 1968 : 1966),
            "M03L" => (1966, 1955),
            "M04" => (1972, 1970),
            "M04L" => (1963, 1944),
            "M05" => (1972, 1972),
            "M05L" => (1971, 1952),
            "M06" => (1972, 1971),
            "M06L" => (1960, 1941),
            "M07" => (1972, 1972),
            "M07L" => (1968, 1938),
            "M08" => (1972, 1971),
            "M08L" => (1957, -1),
            "M09" => (1972, 1972),
            "M09L" => (2014, -1),
            "M10" => (1972, 1972),
            "M10L" => (1984, -1),
            "M11" => (1972, 1970),
            "M11L" => (day <= 10 ? 2033 : 2034, -1),
            "M12" => (1972, 1972),
            _ => (-1, -1),
        };

        return day >= 30 ? years.Item2 : years.Item1;
    }
}

/// <summary>The calendar systems of one Intl data set, by canonical calendar type.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE484D
// Broiler-Human:        PENDING
internal sealed class JsCalendars
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F47157
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsCalendarSystem> systems;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AFCECF
    // Broiler-Human:        PENDING
    internal JsCalendars(ReadOnlySpan<byte> table)
    {
        // THE TABLE IS THE SHARED DECODER'S (JSD-0076): it is read once, here, and every table-backed
        // calendar asks it for its years.
        var data = new CalendarTables(table);
        systems = new System.Collections.Generic.Dictionary<string, JsCalendarSystem>(StringComparer.Ordinal)
        {
            ["buddhist"] = new JsGregorianCalendar("buddhist", -543),
            ["chinese"] = new JsEastAsianCalendar("chinese", data),
            ["coptic"] = new JsCopticCalendar("coptic", 103605, 0),
            ["dangi"] = new JsEastAsianCalendar("dangi", data),
            ["ethioaa"] = new JsCopticCalendar("ethioaa", 2796, 5500),
            ["ethiopic"] = new JsCopticCalendar("ethiopic", 2796, 0),
            ["gregory"] = new JsGregorianCalendar("gregory", 0),
            ["hebrew"] = new JsHebrewCalendar(),
            ["indian"] = new JsIndianCalendar(),
            ["islamic-civil"] = new JsHijriCalendar("islamic-civil", thursday: false),
            ["islamic-tbla"] = new JsHijriCalendar("islamic-tbla", thursday: true),
            ["islamic-umalqura"] = new JsHijriCalendar("islamic-umalqura", thursday: false, data),
            ["japanese"] = new JsGregorianCalendar("japanese", 0),
            ["persian"] = new JsPersianCalendar(data),
            ["roc"] = new JsGregorianCalendar("roc", 1911),
        };
    }

    /// <summary>The system of a canonical calendar type other than <c>iso8601</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0F2D0A
    // Broiler-Human:        PENDING
    internal JsCalendarSystem For(string calendar) => systems[calendar];
}
