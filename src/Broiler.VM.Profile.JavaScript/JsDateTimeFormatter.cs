// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           22
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>A part of a formatted date: its ECMA-402 type, its text, and for a range its source.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F9641C
// Broiler-Human:        PENDING
internal sealed record JsDatePart(string Type, string Value, string? Source);

/// <summary>A formatted field: its pattern letter and where its text is.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=137283
// Broiler-Human:        PENDING
internal readonly record struct JsDateField(char Letter, int Start, int End);

/// <summary>
/// What a time zone a format admits is: UTC, the zero offset ICU names GMT, another offset, or an IANA
/// zone whose offset changes (JSD-0053).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8005BA
// Broiler-Human:        PENDING
internal enum JsZoneKind
{
    Utc,
    Gmt,
    Offset,
    Named,
}

/// <summary>
/// A time value broken into proleptic Gregorian fields in a zone's local time, with the era year
/// ICU's <c>y</c> writes.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CA4F34
// Broiler-Human:        PENDING
internal readonly record struct JsLocalTime(long Year, int Month, int Day, int Weekday, int Hour, int Minute, int Second, int Millisecond, int DayOfYear, long Utc)
{
    /// <summary>The CLDR era index of a calendar's era; null for the Gregorian calendar's, which the year decides.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DEC1A7
    // Broiler-Human:        PENDING
    internal string? CalendarEra { get; init; }

    /// <summary>A calendar's era year; null for the Gregorian calendar's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9F4E20
    // Broiler-Human:        PENDING
    internal long? CalendarEraYear { get; init; }

    /// <summary>The CLDR key of a calendar's month name, where it is not the month's number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D5F619
    // Broiler-Human:        PENDING
    internal string? MonthKey { get; init; }

    /// <summary>The number a calendar writes for its month, where it is not the ordinal: a Chinese month's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D67A1B
    // Broiler-Human:        PENDING
    internal int? MonthNumber { get; init; }

    /// <summary>Whether the month is a leap month that CLDR's month patterns mark (the Chinese calendars').</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A47384
    // Broiler-Human:        PENDING
    internal bool LeapMonth { get; init; }

    /// <summary>The year of the sixty-year cycle (ICU's <c>y</c> in the Chinese calendars); null elsewhere.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BCC859
    // Broiler-Human:        PENDING
    internal int? CyclicYear { get; init; }

    /// <summary>The CLDR era index: the calendar's, or the Gregorian one the year decides.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=745584
    // Broiler-Human:        PENDING
    internal string Era => CalendarEra ?? (Year <= 0 ? "0" : "1");

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1BA541
    // Broiler-Human:        PENDING
    internal long EraYear => CalendarEraYear ?? (Year <= 0 ? 1 - Year : Year);

    /// <summary>
    /// The same time's fields in a calendar other than the Gregorian one (JSD-0057): its arithmetic
    /// year, ordinal month, day and day of the year, its era as CLDR numbers it, and how its month is
    /// named and numbered.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1596FF
    // Broiler-Human:        PENDING
    internal JsLocalTime InCalendar(JsCalendarSystem system)
    {
        var days = JsTemporalCore.EpochDays(Year, Month, Day);
        var (year, month, day) = system.FromEpochDays(days);
        var (era, eraYear) = system.EraOf(year.Year, days);
        var code = JsCalendarSystem.MonthCode(year, month);
        var number = ((code[1] - '0') * 10) + (code[2] - '0');
        var lunisolar = system.Id is "chinese" or "dangi";

        string? monthKey = system.Id switch
        {
            "hebrew" => code == "M05L" ? "6" : code == "M06" && year.LeapMonth != 0 ? "7-yeartype-leap" : (number < 6 ? number : number + 1).ToString(System.Globalization.CultureInfo.InvariantCulture),
            "chinese" or "dangi" => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => null,
        };

        return this with
        {
            Year = year.Year,
            Month = month,
            Day = day,
            DayOfYear = (int)(days - year.Start) + 1,
            CalendarEra = lunisolar ? string.Empty : EraIndex(system.Id, era!),
            CalendarEraYear = lunisolar ? year.Year : eraYear,
            MonthKey = monthKey,
            MonthNumber = lunisolar ? number : null,
            LeapMonth = lunisolar && code.Length == 4,
            CyclicYear = lunisolar ? (int)((((year.Year - 4) % 60) + 60) % 60) + 1 : null,
        };
    }

    /// <summary>The index CLDR gives a Temporal era of a calendar (the Intl era and month code proposal's Table 2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=43DD64
    // Broiler-Human:        PENDING
    private static string EraIndex(string calendar, string era) => (calendar, era) switch
    {
        ("japanese", "meiji") => "232",
        ("japanese", "taisho") => "233",
        ("japanese", "showa") => "234",
        ("japanese", "heisei") => "235",
        ("japanese", "reiwa") => "236",
        (_, "bce" or "broc") => "0",
        (_, "ce" or "roc") => "1",
        (_, "bh") => "1",
        ("coptic" or "ethiopic", "am") => "1",
        _ => "0",
    };

    /// <summary>The fields of <paramref name="time"/>, an integral time value, <paramref name="offsetMinutes"/> from UTC.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=227A46
    // Broiler-Human:        PENDING
    internal static JsLocalTime From(double time, int offsetSeconds)
    {
        var local = (long)time + (offsetSeconds * 1_000L);
        var days = local >= 0 ? local / 86_400_000L : -((-local + 86_399_999L) / 86_400_000L);
        var within = local - (days * 86_400_000L);

        // HOWARD HINNANT'S civil_from_days, exact over the whole time-value range.
        var z = days + 719_468;
        var era = (z >= 0 ? z : z - 146_096) / 146_097;
        var dayOfEra = z - (era * 146_097);
        var yearOfEra = (dayOfEra - (dayOfEra / 1_460) + (dayOfEra / 36_524) - (dayOfEra / 146_096)) / 365;
        var dayOfYearMarch = dayOfEra - ((365 * yearOfEra) + (yearOfEra / 4) - (yearOfEra / 100));
        var monthMarch = ((5 * dayOfYearMarch) + 2) / 153;
        var day = (int)(dayOfYearMarch - (((153 * monthMarch) + 2) / 5) + 1);
        var month = (int)(monthMarch < 10 ? monthMarch + 3 : monthMarch - 9);
        var year = yearOfEra + (era * 400) + (month <= 2 ? 1 : 0);
        var weekday = (int)(((days % 7) + 11) % 7);

        var leap = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        int[] starts = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
        var dayOfYear = starts[month - 1] + day + (leap && month > 2 ? 1 : 0);

        return new JsLocalTime(
            year,
            month,
            day,
            weekday,
            (int)(within / 3_600_000),
            (int)(within / 60_000 % 60),
            (int)(within / 1_000 % 60),
            (int)(within % 1_000),
            dayOfYear,
            (long)time);
    }
}

/// <summary>
/// A resolved <c>Intl.DateTimeFormat</c>'s formatting: its pattern written for a time value, as
/// ICU's SimpleDateFormat writes it, and its ranges as ICU's DateIntervalFormat writes them (JSD-0045).
/// </summary>
/// <remarks>
/// <para>
/// <b>Every field is written from CLDR's data for the language</b>: names in the context the
/// letter asks for (<c>M</c> and <c>E</c> the format forms, <c>L</c> and <c>c</c> the stand-alone),
/// numbers in the format's numbering system, a flexible day period by the language's rules, and a
/// zone by its CLDR name or CLDR's GMT format.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5E70ED
// Broiler-Human:        PENDING
internal sealed class JsDateTimeFormatter
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=685530
    // Broiler-Human:        PENDING
    private static readonly string[] DayKeys = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=883FED
    // Broiler-Human:        PENDING
    private readonly JsDateData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=401D99
    // Broiler-Human:        PENDING
    private readonly string language;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DEB8BB
    // Broiler-Human:        PENDING
    private readonly string[] digits;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6CFEE5
    // Broiler-Human:        PENDING
    private readonly System.Func<JsDateIntervalFormat> intervals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F65F5B
    // Broiler-Human:        PENDING
    private JsDateIntervalFormat? interval;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C7BA00
    // Broiler-Human:        PENDING
    internal JsDateTimeFormatter(
        JsDateData data,
        string language,
        string[] digits,
        string timeZone,
        int offsetMinutes,
        JsZoneKind zoneKind,
        string pattern,
        System.Func<JsDateIntervalFormat> intervals,
        JsZone? zone = null,
        JsCalendarSystem? calendar = null)
    {
        this.zone = zone;
        this.calendar = calendar;
        this.data = data;
        this.language = language;
        this.digits = digits;
        this.intervals = intervals;
        TimeZone = timeZone;
        OffsetMinutes = offsetMinutes;
        ZoneKind = zoneKind;
        Pattern = pattern;
    }

    /// <summary>The IANA zone whose offsets apply, or nothing for UTC and a fixed offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B6CCEC
    // Broiler-Human:        PENDING
    private readonly JsZone? zone;

    /// <summary>The calendar the fields are written in, or nothing for the Gregorian and ISO 8601 ones (JSD-0057).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2174A5
    // Broiler-Human:        PENDING
    private readonly JsCalendarSystem? calendar;

    /// <summary>A time value's local fields, in the format's calendar.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A8FC8F
    // Broiler-Human:        PENDING
    internal JsLocalTime LocalTime(double time)
    {
        var local = JsLocalTime.From(time, OffsetSecondsAt(time));
        return calendar is null ? local : local.InCalendar(calendar);
    }

    /// <summary>The time zone's identifier: <c>UTC</c>, an offset such as <c>+05:30</c>, or an IANA primary identifier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9A6046
    // Broiler-Human:        PENDING
    internal string TimeZone { get; }

    /// <summary>The zone's offset from UTC, in minutes, where it is fixed; zero for an IANA zone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=02B42A
    // Broiler-Human:        PENDING
    internal int OffsetMinutes { get; }

    /// <summary>The offset, in seconds, in force at a time value: the fixed one, or the IANA zone's then.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=519DF9
    // Broiler-Human:        PENDING
    internal int OffsetSecondsAt(double time) =>
        zone is null ? OffsetMinutes * 60 : zone.OffsetAt((long)System.Math.Floor(time / 1000));

    /// <summary>Which kind of zone it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=39D641
    // Broiler-Human:        PENDING
    internal JsZoneKind ZoneKind { get; }

    /// <summary>The pattern a single time value is written in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E4B9F8
    // Broiler-Human:        PENDING
    internal string Pattern { get; }

    /// <summary>The parts of <paramref name="time"/>, an integral time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=81FCB4
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsDatePart> Parts(JsEngine engine, double time, string? source)
    {
        var text = new System.Text.StringBuilder();
        var fields = new System.Collections.Generic.List<JsDateField>();
        FormatPattern(Pattern, LocalTime(time), text, fields);
        engine.Charge((ulong)text.Length);
        return ToParts(text.ToString(), fields, source is null ? null : (_, _) => source);
    }

    /// <summary>
    /// The parts of the range from <paramref name="x"/> to <paramref name="y"/>: the range ICU writes,
    /// its parts' sources from the spans of the fields it repeats, or <paramref name="x"/> alone,
    /// shared, where it writes one date.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9B1FF7
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsDatePart> RangeParts(JsEngine engine, double x, double y)
    {
        interval ??= intervals();

        var text = new System.Text.StringBuilder();
        var fields = new System.Collections.Generic.List<JsDateField>();
        var first = interval.Format(LocalTime(x), LocalTime(y), this, text, fields);
        engine.Charge((ulong)text.Length);

        // THE SPANS ARE ICU'S: the first and second occurrences of each repeated field.
        int s1a = int.MaxValue, s1b = 0, s2a = int.MaxValue, s2b = 0;

        for (var i = 0; first >= 0 && i < fields.Count; i++)
        {
            for (var j = i + 1; j < fields.Count; j++)
            {
                if (fields[i].Letter != fields[j].Letter)
                {
                    continue;
                }

                s1a = System.Math.Min(s1a, fields[i].Start);
                s1b = System.Math.Max(s1b, fields[i].End);
                s2a = System.Math.Min(s2a, fields[j].Start);
                s2b = System.Math.Max(s2b, fields[j].End);
                break;
            }
        }

        if (first < 0 || s1a == int.MaxValue)
        {
            return Parts(engine, x, "shared");
        }

        var (startFrom, startTo, endFrom, endTo) = first == 0 ? (s1a, s1b, s2a, s2b) : (s2a, s2b, s1a, s1b);

        return ToParts(text.ToString(), fields, (start, end) =>
            startFrom <= start && end <= startTo ? "startRange" :
            endFrom <= start && end <= endTo ? "endRange" :
            "shared");
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E3EF37
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsDatePart> ToParts(
        string text,
        System.Collections.Generic.List<JsDateField> fields,
        System.Func<int, int, string>? source)
    {
        var parts = new System.Collections.Generic.List<JsDatePart>(fields.Count * 2);
        var previous = 0;

        foreach (var field in fields)
        {
            if (field.Start > previous)
            {
                parts.Add(new JsDatePart("literal", text[previous..field.Start], source?.Invoke(previous, field.Start)));
            }

            parts.Add(new JsDatePart(TypeOf(field.Letter), text[field.Start..field.End], source?.Invoke(field.Start, field.End)));
            previous = field.End;
        }

        if (text.Length > previous)
        {
            parts.Add(new JsDatePart("literal", text[previous..], source?.Invoke(previous, text.Length)));
        }

        return parts;
    }

    /// <summary>The ECMA-402 part type of a pattern letter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B6CC9
    // Broiler-Human:        PENDING
    internal static string TypeOf(char letter) => letter switch
    {
        'G' => "era",
        'y' or 'Y' or 'u' => "year",
        'U' => "yearName",
        'r' => "relatedYear",
        'M' or 'L' => "month",
        'd' => "day",
        'E' or 'c' or 'e' => "weekday",
        'a' or 'b' or 'B' => "dayPeriod",
        'h' or 'H' or 'k' or 'K' => "hour",
        'm' => "minute",
        's' => "second",
        'S' => "fractionalSecond",
        'z' or 'Z' or 'O' or 'v' or 'V' or 'X' or 'x' => "timeZoneName",
        _ => "unknown",
    };

    /// <summary>Writes <paramref name="pattern"/> for <paramref name="time"/>, recording each field it writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1B08A7
    // Broiler-Human:        PENDING
    internal void FormatPattern(string pattern, JsLocalTime time, System.Text.StringBuilder text, System.Collections.Generic.List<JsDateField> fields)
    {
        var hasMinute = false;
        var hasSecond = false;
        var quoted = false;

        foreach (var c in pattern)
        {
            if (c == '\'')
            {
                quoted = !quoted;
            }
            else if (!quoted)
            {
                hasMinute |= c == 'm';
                hasSecond |= c == 's';
            }
        }

        var at = 0;

        while (at < pattern.Length)
        {
            var c = pattern[at];

            if (c == '\'')
            {
                if (at + 1 < pattern.Length && pattern[at + 1] == '\'')
                {
                    text.Append('\'');
                    at += 2;
                    continue;
                }

                at++;

                while (at < pattern.Length)
                {
                    if (pattern[at] == '\'')
                    {
                        if (at + 1 < pattern.Length && pattern[at + 1] == '\'')
                        {
                            text.Append('\'');
                            at += 2;
                            continue;
                        }

                        at++;
                        break;
                    }

                    text.Append(pattern[at]);
                    at++;
                }

                continue;
            }

            if (!JsDatePatternGenerator.IsLetter(c))
            {
                text.Append(c);
                at++;
                continue;
            }

            var count = 1;

            while (at + count < pattern.Length && pattern[at + count] == c)
            {
                count++;
            }

            var start = text.Length;

            if (Field(c, count, time, hasMinute, hasSecond, text))
            {
                fields.Add(new JsDateField(c, start, text.Length));
            }

            at += count;
        }
    }

    /// <summary>Writes one field; whether the letter is one this formatter writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AC523A
    // Broiler-Human:        PENDING
    private bool Field(char c, int count, JsLocalTime time, bool hasMinute, bool hasSecond, System.Text.StringBuilder text)
    {
        switch (c)
        {
            case 'G':
                text.Append(Name("eras." + (count <= 3 ? "eraAbbr" : count == 4 ? "eraNames" : "eraNarrow") + "." + time.Era));
                return true;
            case 'y':
                var year = time.CyclicYear ?? time.EraYear;
                Number(text, count == 2 ? year % 100 : year, count);
                return true;
            case 'Y' or 'u' or 'r':
                Number(text, c == 'Y' ? time.EraYear : time.Year, count);
                return true;
            case 'U':
                text.Append(time.CyclicYear is { } cyclic
                    ? Name("cyclic.years." + (count <= 3 ? "abbreviated" : count == 4 ? "wide" : "narrow") + "." + cyclic.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    : time.Year.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return true;
            case 'M' or 'L':
                var context = c == 'M' ? "format" : "stand-alone";
                var monthStart = text.Length;

                if (count <= 2)
                {
                    Number(text, time.MonthNumber ?? time.Month, count);
                    Leap(text, monthStart, "monthPatterns.numeric.all.leap", time);
                }
                else
                {
                    var key = time.MonthKey ?? time.Month.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    text.Append(Name("months." + context + "." + Width(count) + "." + key));
                    Leap(text, monthStart, "monthPatterns." + context + "." + Width(count) + ".leap", time);
                }

                return true;
            case 'd':
                Number(text, time.Day, count);
                return true;
            case 'D':
                Number(text, time.DayOfYear, count);
                return true;
            case 'F':
                Number(text, ((time.Day - 1) / 7) + 1, count);
                return true;
            case 'E' or 'c' or 'e':
                if (c != 'E' && count <= 2)
                {
                    Number(text, time.Weekday + 1, count);
                }
                else
                {
                    var width = count switch { <= 3 => "abbreviated", 4 => "wide", 5 => "narrow", _ => "short" };
                    text.Append(Name("days." + (c == 'c' ? "stand-alone" : "format") + "." + width + "." + DayKeys[time.Weekday]));
                }

                return true;
            case 'a':
                text.Append(Name("dayPeriods.format." + Width(count) + "." + (time.Hour < 12 ? "am" : "pm")));
                return true;
            case 'b' or 'B':
                text.Append(FlexibleDayPeriod(c, count, time, hasMinute, hasSecond));
                return true;
            case 'h':
                Number(text, time.Hour % 12 == 0 ? 12 : time.Hour % 12, count);
                return true;
            case 'H':
                Number(text, time.Hour, count);
                return true;
            case 'K':
                Number(text, time.Hour % 12, count);
                return true;
            case 'k':
                Number(text, time.Hour == 0 ? 24 : time.Hour, count);
                return true;
            case 'm':
                Number(text, time.Minute, count);
                return true;
            case 's':
                Number(text, time.Second, count);
                return true;
            case 'S':
                Number(text, count switch { 1 => time.Millisecond / 100, 2 => time.Millisecond / 10, _ => time.Millisecond }, System.Math.Min(count, 3));

                for (var extra = 3; extra < count; extra++)
                {
                    text.Append(digits[0]);
                }

                return true;
            case 'Q' or 'q':
                Number(text, ((time.Month - 1) / 3) + 1, count);
                return true;
            case 'z' or 'Z' or 'O' or 'v' or 'V' or 'X' or 'x':
                text.Append(ZoneName(c, count, time.Utc));
                return true;
            default:
                return false;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0249F3
    // Broiler-Human:        PENDING
    private static string Width(int count) => count switch { <= 3 => "abbreviated", 4 => "wide", _ => "narrow" };

    /// <summary>A leap month's text written from <paramref name="start"/> on, put in CLDR's month pattern for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=759487
    // Broiler-Human:        PENDING
    private void Leap(System.Text.StringBuilder text, int start, string patternKey, JsLocalTime time)
    {
        if (!time.LeapMonth || data.Value(language, patternKey) is not { } pattern)
        {
            return;
        }

        var month = text.ToString(start, text.Length - start);
        text.Length = start;
        text.Append(pattern.Replace("{0}", month, System.StringComparison.Ordinal));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F3F6FF
    // Broiler-Human:        PENDING
    private string Name(string key) => data.Value(language, key) ?? string.Empty;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2F0550
    // Broiler-Human:        PENDING
    private void Number(System.Text.StringBuilder text, long value, int minimum)
    {
        var plain = System.Math.Abs(value).ToString(System.Globalization.CultureInfo.InvariantCulture);

        if (value < 0)
        {
            text.Append('-');
        }

        for (var pad = plain.Length; pad < minimum; pad++)
        {
            text.Append(digits[0]);
        }

        foreach (var c in plain)
        {
            text.Append(digits[c - '0']);
        }
    }

    /// <summary>
    /// ICU's flexible day period (<c>B</c>) and its noon form (<c>b</c>): noon and midnight at their
    /// exact time where the language names them, midnight written as the period it falls in, and the
    /// language's period for the hour; AM or PM where none is named.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8F2349
    // Broiler-Human:        PENDING
    private string FlexibleDayPeriod(char c, int count, JsLocalTime time, bool hasMinute, bool hasSecond)
    {
        var minute = hasMinute ? time.Minute : 0;
        var second = hasSecond ? time.Second : 0;
        var width = Width(count);
        var amPm = Name("dayPeriods.format." + width + "." + (time.Hour < 12 ? "am" : "pm"));

        // A CALENDAR'S DATA IS ITS LANGUAGE'S FOR DAY PERIODS, which belong to no calendar (JSD-0055).
        var rulesLanguage = JsDateData.LanguageOf(language);

        if (!data.DayPeriods.TryGetValue(rulesLanguage, out var rules))
        {
            return amPm;
        }

        bool Has(string period) => rules.Exists(rule => rule.Period == period);

        if (c == 'b')
        {
            return time.Hour == 12 && minute == 0 && second == 0 && Has("noon") && data.Value(language, "dayPeriods.format." + width + ".noon") is { } noon
                ? noon
                : amPm;
        }

        var period = time.Hour == 12 && minute == 0 && second == 0 && Has("noon") ? "noon" : null;

        if (period is null)
        {
            var at = time.Hour * 60;

            foreach (var rule in rules)
            {
                if (!rule.At && rule.From <= at && at < rule.Before)
                {
                    period = rule.Period;
                    break;
                }
            }
        }

        if (period is null or "am" or "pm")
        {
            return amPm;
        }

        return data.Value(language, "dayPeriods.format." + width + "." + period) ?? amPm;
    }

    /// <summary>
    /// A zone field: CLDR's name for UTC or GMT where the field asks for one, otherwise CLDR's GMT
    /// format. The GMT zone's name is its metazone's, which ICU maps only from 1970-01-01T00:00Z to
    /// before 9999-12-31T23:59Z; outside those bounds the zone has no name and the GMT format serves.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E164EB
    // Broiler-Human:        PENDING
    private string ZoneName(char c, int count, long utc)
    {
        var metazone = ZoneKind == JsZoneKind.Gmt && utc >= 0 && utc < 253_402_300_740_000L;

        string? Named(string width) => ZoneKind switch
        {
            JsZoneKind.Utc => data.Value(language, "zone.utc." + width),
            JsZoneKind.Gmt when metazone => data.Value(language, "zone.gmt." + width),
            _ => null,
        };

        string? Generic(string width) => metazone ? data.Value(language, "zone.gmt." + width) : null;

        var offset = OffsetSecondsAt(utc);

        return c switch
        {
            'z' => (count < 4 ? Named("short") ?? Gmt(offset, longForm: false) : Named("long") ?? Gmt(offset, longForm: true)),
            'v' => (count < 4 ? Generic("short") ?? Gmt(offset, longForm: false) : Generic("long") ?? Gmt(offset, longForm: true)),
            'O' => Gmt(offset, longForm: count >= 4),
            'Z' => count == 4 ? Gmt(offset, longForm: true) : count == 5 ? Iso(offset, extended: true, zulu: true) : Iso(offset, extended: false, zulu: false),
            'X' => Iso(offset, extended: count >= 3, zulu: true),
            'x' => Iso(offset, extended: count >= 3, zulu: false),
            _ => count == 2 ? TimeZone : Gmt(offset, longForm: true),
        };
    }

    /// <summary>
    /// CLDR's localized GMT format of the offset, long (<c>GMT+05:30</c>) or short (<c>GMT+5:30</c>),
    /// with its seconds after the minutes where it has any, as ICU writes a local mean time.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A6765B
    // Broiler-Human:        PENDING
    private string Gmt(int offsetSeconds, bool longForm)
    {
        if (offsetSeconds == 0)
        {
            return Name("zone.gmtZeroFormat");
        }

        var formats = Name("zone.hourFormat").Split(';');
        var format = offsetSeconds > 0 || formats.Length < 2 ? formats[0] : formats[1];
        var hours = System.Math.Abs(offsetSeconds) / 3600;
        var minutes = System.Math.Abs(offsetSeconds) / 60 % 60;
        var seconds = System.Math.Abs(offsetSeconds) % 60;
        var offset = new System.Text.StringBuilder();
        var at = 0;

        while (at < format.Length)
        {
            var c = format[at];
            var count = 1;

            while (at + count < format.Length && format[at + count] == c)
            {
                count++;
            }

            if (c == 'H')
            {
                Number(offset, hours, longForm ? count : 1);
            }
            else if (c == 'm')
            {
                if (longForm || minutes != 0 || seconds != 0)
                {
                    Number(offset, minutes, count);

                    if (seconds != 0)
                    {
                        offset.Append(':');
                        Number(offset, seconds, 2);
                    }
                }
            }
            else if (!longForm && minutes == 0 && seconds == 0 && at + count < format.Length && format[at + count] == 'm')
            {
                // THE SEPARATOR BEFORE THE MINUTES GOES WITH THEM in the short form.
            }
            else
            {
                offset.Append(c, count);
            }

            at += count;
        }

        return JsDatePatternGenerator.Substitute(Name("zone.gmtFormat"), offset.ToString(), string.Empty, string.Empty);
    }

    /// <summary>An ISO 8601 offset: <c>+0530</c>, or <c>+05:30</c> extended, or <c>Z</c> for zero where asked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=492D63
    // Broiler-Human:        PENDING
    private string Iso(int offsetSeconds, bool extended, bool zulu)
    {
        var offsetMinutes = offsetSeconds / 60;

        if (offsetMinutes == 0 && zulu)
        {
            return "Z";
        }

        var hours = System.Math.Abs(offsetMinutes) / 60;
        var minutes = System.Math.Abs(offsetMinutes) % 60;
        return (offsetMinutes < 0 ? "-" : "+") +
            hours.ToString("00", System.Globalization.CultureInfo.InvariantCulture) +
            (extended ? ":" : string.Empty) +
            minutes.ToString("00", System.Globalization.CultureInfo.InvariantCulture);
    }
}
