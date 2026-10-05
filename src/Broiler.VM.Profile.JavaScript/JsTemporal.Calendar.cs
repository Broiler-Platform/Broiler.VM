// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   34
// Annotated:        34/34
// Exempt:           31
// Human-reviewed:   0/34
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       34
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The enumeration keys of Table 19, in its order.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=46F3FA
// Broiler-Human:        PENDING
internal enum CalendarField
{
    Era,
    EraYear,
    Year,
    Month,
    MonthCode,
    Day,
    Hour,
    Minute,
    Second,
    Millisecond,
    Microsecond,
    Nanosecond,
    Offset,
    TimeZone,
}

/// <summary>A Calendar Fields Record (s12.3.2): each field a value or unset (null).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=000B82
// Broiler-Human:        PENDING
internal sealed class JsCalendarFields
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C1C340
    // Broiler-Human:        PENDING
    internal string? Era { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E34B03
    // Broiler-Human:        PENDING
    internal double? EraYear { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=583229
    // Broiler-Human:        PENDING
    internal double? Year { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5E6541
    // Broiler-Human:        PENDING
    internal double? Month { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=00519F
    // Broiler-Human:        PENDING
    internal string? MonthCode { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3AA990
    // Broiler-Human:        PENDING
    internal double? Day { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FC5840
    // Broiler-Human:        PENDING
    internal double? Hour { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=72CB75
    // Broiler-Human:        PENDING
    internal double? Minute { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A8AE0
    // Broiler-Human:        PENDING
    internal double? Second { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7E60DF
    // Broiler-Human:        PENDING
    internal double? Millisecond { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3EF8A9
    // Broiler-Human:        PENDING
    internal double? Microsecond { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4C76B8
    // Broiler-Human:        PENDING
    internal double? Nanosecond { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37AE33
    // Broiler-Human:        PENDING
    internal string? Offset { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34BD29
    // Broiler-Human:        PENDING
    internal string? TimeZone { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9834FA
    // Broiler-Human:        PENDING
    internal bool Has(CalendarField key) => key switch
    {
        CalendarField.Era => Era is not null,
        CalendarField.EraYear => EraYear is not null,
        CalendarField.Year => Year is not null,
        CalendarField.Month => Month is not null,
        CalendarField.MonthCode => MonthCode is not null,
        CalendarField.Day => Day is not null,
        CalendarField.Hour => Hour is not null,
        CalendarField.Minute => Minute is not null,
        CalendarField.Second => Second is not null,
        CalendarField.Millisecond => Millisecond is not null,
        CalendarField.Microsecond => Microsecond is not null,
        CalendarField.Nanosecond => Nanosecond is not null,
        CalendarField.Offset => Offset is not null,
        _ => TimeZone is not null,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B23880
    // Broiler-Human:        PENDING
    internal void CopyFrom(JsCalendarFields source, CalendarField key)
    {
        switch (key)
        {
            case CalendarField.Era: Era = source.Era; break;
            case CalendarField.EraYear: EraYear = source.EraYear; break;
            case CalendarField.Year: Year = source.Year; break;
            case CalendarField.Month: Month = source.Month; break;
            case CalendarField.MonthCode: MonthCode = source.MonthCode; break;
            case CalendarField.Day: Day = source.Day; break;
            case CalendarField.Hour: Hour = source.Hour; break;
            case CalendarField.Minute: Minute = source.Minute; break;
            case CalendarField.Second: Second = source.Second; break;
            case CalendarField.Millisecond: Millisecond = source.Millisecond; break;
            case CalendarField.Microsecond: Microsecond = source.Microsecond; break;
            case CalendarField.Nanosecond: Nanosecond = source.Nanosecond; break;
            case CalendarField.Offset: Offset = source.Offset; break;
            default: TimeZone = source.TimeZone; break;
        }
    }
}

/// <summary>A Calendar Date Record (s12.3.1).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8B89E5
// Broiler-Human:        PENDING
internal readonly record struct JsCalendarDate(
    string? Era,
    long? EraYear,
    long Year,
    int Month,
    string MonthCode,
    int Day,
    int DayOfWeek,
    int DayOfYear,
    int? WeekOfYear,
    long? YearOfWeek,
    int DaysInMonth,
    int DaysInYear,
    bool InLeapYear);

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED29FE
    // Broiler-Human:        PENDING
    private static readonly string[] FieldProperties =
        ["era", "eraYear", "year", "month", "monthCode", "day", "hour", "minute", "second", "millisecond", "microsecond", "nanosecond", "offset", "timeZone"];

    /// <summary>
    /// The calendars Temporal and Intl.DateTimeFormat support (s12.1.2): the ISO 8601 calendar, and
    /// the Gregorian calendar with its two eras.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DA754A
    // Broiler-Human:        PENDING
    internal static readonly string[] AvailableCalendars = ["gregory", "iso8601"];

    /// <summary>The proposal's CanonicalizeCalendar (s12.1.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBDD16
    // Broiler-Human:        PENDING
    internal static string CanonicalizeCalendar(JsEngine engine, string id)
    {
        var lower = AsciiLower(id);

        if (System.Array.IndexOf(AvailableCalendars, lower) < 0)
        {
            throw engine.Error("RangeError", "Temporal: the calendar " + id + " is not supported");
        }

        return lower;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EF1A13
    // Broiler-Human:        PENDING
    private static string AsciiLower(string text)
    {
        var chars = text.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] is >= 'A' and <= 'Z')
            {
                chars[i] = (char)(chars[i] + 32);
            }
        }

        return new string(chars);
    }

    /// <summary>The calendar of a Temporal object that has one, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0627AC
    // Broiler-Human:        PENDING
    internal static string? CalendarOf(JsObject? item) => item switch
    {
        JsPlainDateObject date => date.Calendar,
        JsPlainDateTimeObject dateTime => dateTime.Calendar,
        JsPlainMonthDayObject monthDay => monthDay.Calendar,
        JsPlainYearMonthObject yearMonth => yearMonth.Calendar,
        JsZonedDateTimeObject zoned => zoned.Calendar,
        _ => null,
    };

    /// <summary>The proposal's ToTemporalCalendarIdentifier (s12.3.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=829524
    // Broiler-Human:        PENDING
    internal static string ToCalendarIdentifier(JsEngine engine, JsValue value)
    {
        if (CalendarOf(value.AsObjectOrNull()) is { } calendar)
        {
            return calendar;
        }

        if (!value.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a calendar must be a string or a Temporal object with a calendar");
        }

        return CanonicalizeCalendar(engine, ParseCalendarString(engine, value.AsString()));
    }

    /// <summary>The proposal's ParseTemporalCalendarString (s13.36).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5E39B6
    // Broiler-Human:        PENDING
    private static string ParseCalendarString(JsEngine engine, string text)
    {
        try
        {
            var parsed = JsTemporalParser.Parse(
                engine,
                text,
                TemporalGoal.ZonedDateTime,
                TemporalGoal.DateTime,
                TemporalGoal.Instant,
                TemporalGoal.Time,
                TemporalGoal.MonthDay,
                TemporalGoal.YearMonth);
            return parsed.Calendar ?? "iso8601";
        }
        catch (JsThrow)
        {
            if (!JsTemporalParser.IsAnnotationValue(text))
            {
                throw engine.Error("RangeError", "Temporal: " + text + " is not a calendar");
            }

            return text;
        }
    }

    /// <summary>The proposal's GetTemporalCalendarIdentifierWithISODefault (s12.3.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DAC467
    // Broiler-Human:        PENDING
    internal static string CalendarWithIsoDefault(JsEngine engine, JsObject item)
    {
        if (CalendarOf(item) is { } calendar)
        {
            return calendar;
        }

        var calendarLike = engine.GetProperty(JsValue.Object(item), "calendar");
        return calendarLike.Type == JsType.Undefined ? "iso8601" : ToCalendarIdentifier(engine, calendarLike);
    }

    /// <summary>The proposal's CalendarExtraFields (s12.3.27).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9DD221
    // Broiler-Human:        PENDING
    private static CalendarField[] ExtraFields(string calendar, CalendarField[] fields) =>
        calendar == "gregory" && System.Array.IndexOf(fields, CalendarField.Year) >= 0
            ? [CalendarField.Era, CalendarField.EraYear]
            : [];

    /// <summary>
    /// The proposal's PrepareCalendarFields (s12.3.3): <paramref name="required"/> null with
    /// <paramref name="partial"/> for partial input.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AEAF8B
    // Broiler-Human:        PENDING
    internal static JsCalendarFields PrepareFields(
        JsEngine engine,
        string calendar,
        JsObject fields,
        CalendarField[] calendarFields,
        CalendarField[] nonCalendarFields,
        CalendarField[]? required,
        bool partial = false)
    {
        var names = new System.Collections.Generic.List<CalendarField>(calendarFields);
        names.AddRange(nonCalendarFields);
        names.AddRange(ExtraFields(calendar, calendarFields));
        names.Sort((one, two) => string.CompareOrdinal(FieldProperties[(int)one], FieldProperties[(int)two]));

        var result = new JsCalendarFields();
        var any = false;

        foreach (var key in names)
        {
            var value = engine.GetProperty(JsValue.Object(fields), FieldProperties[(int)key]);

            if (value.Type != JsType.Undefined)
            {
                any = true;

                switch (key)
                {
                    case CalendarField.Month:
                    case CalendarField.Day:
                        Set(result, key, PositiveIntegerWithTruncation(engine, value));
                        break;
                    case CalendarField.Era:
                        result.Era = engine.ToStringValue(value);
                        break;
                    case CalendarField.MonthCode:
                        result.MonthCode = MonthCodeOf(engine, value);
                        break;
                    case CalendarField.TimeZone:
                        result.TimeZone = ToTimeZoneIdentifier(engine, value);
                        break;
                    case CalendarField.Offset:
                        result.Offset = OffsetString(engine, value);
                        break;
                    default:
                        Set(result, key, IntegerWithTruncation(engine, value));
                        break;
                }
            }
            else if (!partial)
            {
                if (required is not null && System.Array.IndexOf(required, key) >= 0)
                {
                    throw engine.Error("TypeError", "Temporal: the " + FieldProperties[(int)key] + " property is required");
                }

                // THE DEFAULTS OF TABLE 19: zero for each time field, unset for the rest.
                if (key is >= CalendarField.Hour and <= CalendarField.Nanosecond)
                {
                    Set(result, key, 0);
                }
            }
        }

        if (partial && !any)
        {
            throw engine.Error("TypeError", "Temporal: the object has none of the properties a partial value needs");
        }

        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=10F24B
    // Broiler-Human:        PENDING
    private static void Set(JsCalendarFields fields, CalendarField key, double value)
    {
        switch (key)
        {
            case CalendarField.EraYear: fields.EraYear = value; break;
            case CalendarField.Year: fields.Year = value; break;
            case CalendarField.Month: fields.Month = value; break;
            case CalendarField.Day: fields.Day = value; break;
            case CalendarField.Hour: fields.Hour = value; break;
            case CalendarField.Minute: fields.Minute = value; break;
            case CalendarField.Second: fields.Second = value; break;
            case CalendarField.Millisecond: fields.Millisecond = value; break;
            case CalendarField.Microsecond: fields.Microsecond = value; break;
            default: fields.Nanosecond = value; break;
        }
    }

    /// <summary>The proposal's ParseMonthCode (s12.2.1) then CreateMonthCode (s12.2.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4BFB8D
    // Broiler-Human:        PENDING
    private static string MonthCodeOf(JsEngine engine, JsValue value)
    {
        var primitive = engine.ToPrimitive(value, "string");

        if (!primitive.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a month code must be a string");
        }

        var code = primitive.AsString();
        var valid = code.Length is 3 or 4 && code[0] == 'M' && char.IsAsciiDigit(code[1]) && char.IsAsciiDigit(code[2]) &&
            (code.Length == 3 || code[3] == 'L') && !(code.Length == 3 && code[1] == '0' && code[2] == '0');

        if (!valid)
        {
            throw engine.Error("RangeError", "Temporal: " + code + " is not a month code");
        }

        return code;
    }

    /// <summary>The proposal's CalendarFieldKeysToIgnore (s12.3.29).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=77D295
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.HashSet<CalendarField> KeysToIgnore(string calendar, System.Collections.Generic.IEnumerable<CalendarField> keys)
    {
        var ignored = new System.Collections.Generic.HashSet<CalendarField>();

        foreach (var key in keys)
        {
            ignored.Add(key);

            if (key == CalendarField.Month)
            {
                ignored.Add(CalendarField.MonthCode);
            }
            else if (key == CalendarField.MonthCode)
            {
                ignored.Add(CalendarField.Month);
            }

            if (calendar == "gregory" && key is CalendarField.Era or CalendarField.EraYear or CalendarField.Year)
            {
                ignored.Add(CalendarField.Era);
                ignored.Add(CalendarField.EraYear);
                ignored.Add(CalendarField.Year);
            }
        }

        return ignored;
    }

    /// <summary>The proposal's CalendarMergeFields (s12.3.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A97A0B
    // Broiler-Human:        PENDING
    internal static JsCalendarFields MergeFields(string calendar, JsCalendarFields fields, JsCalendarFields additional)
    {
        var additionalKeys = new System.Collections.Generic.List<CalendarField>();

        foreach (CalendarField key in System.Enum.GetValues<CalendarField>())
        {
            if (additional.Has(key))
            {
                additionalKeys.Add(key);
            }
        }

        var overridden = KeysToIgnore(calendar, additionalKeys);
        var merged = new JsCalendarFields();

        foreach (CalendarField key in System.Enum.GetValues<CalendarField>())
        {
            if (fields.Has(key) && !overridden.Contains(key))
            {
                merged.CopyFrom(fields, key);
            }

            if (additional.Has(key))
            {
                merged.CopyFrom(additional, key);
            }
        }

        return merged;
    }

    /// <summary>The kinds of input CalendarResolveFields validates.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=151CE4
    // Broiler-Human:        PENDING
    internal enum FieldsType
    {
        Date,
        YearMonth,
        MonthDay,
    }

    /// <summary>The proposal's CalendarResolveFields (s12.3.31), with the Gregorian calendar's eras.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A8B61E
    // Broiler-Human:        PENDING
    internal static void ResolveFields(JsEngine engine, string calendar, JsCalendarFields fields, FieldsType type)
    {
        if (calendar == "gregory")
        {
            if ((fields.Era is null) != (fields.EraYear is null))
            {
                throw engine.Error("TypeError", "Temporal: era and eraYear must be given together");
            }

            if (fields.Era is not null)
            {
                var eraYear = fields.EraYear!.Value;
                var year = AsciiLower(fields.Era) switch
                {
                    "ce" or "ad" => eraYear,
                    "bce" or "bc" => 1 - eraYear,
                    _ => throw engine.Error("RangeError", "Temporal: " + fields.Era + " is not an era of the gregory calendar"),
                };

                if (fields.Year is { } given && given != year)
                {
                    throw engine.Error("RangeError", "Temporal: the year and the era year disagree");
                }

                fields.Year = year;
                fields.Era = null;
                fields.EraYear = null;
            }
        }

        var needsYear = type is FieldsType.Date or FieldsType.YearMonth;
        var needsDay = type is FieldsType.Date or FieldsType.MonthDay;

        if (needsYear && fields.Year is null)
        {
            throw engine.Error("TypeError", "Temporal: the year property is required");
        }

        if (needsDay && fields.Day is null)
        {
            throw engine.Error("TypeError", "Temporal: the day property is required");
        }

        if (fields.Month is null && fields.MonthCode is null)
        {
            throw engine.Error("TypeError", "Temporal: the month or monthCode property is required");
        }

        if (calendar == "gregory" && type == FieldsType.MonthDay && fields.MonthCode is null && fields.Year is null)
        {
            throw engine.Error("TypeError", "Temporal: a month-day in the gregory calendar needs a monthCode or a year");
        }

        if (fields.MonthCode is { } code)
        {
            if (code.Length == 4)
            {
                throw engine.Error("RangeError", "Temporal: the ISO 8601 calendar has no leap month " + code);
            }

            var month = int.Parse(code[1..], System.Globalization.CultureInfo.InvariantCulture);

            if (month > 12)
            {
                throw engine.Error("RangeError", "Temporal: " + code + " is not a month of the year");
            }

            if (fields.Month is { } given && given != month)
            {
                throw engine.Error("RangeError", "Temporal: month and monthCode disagree");
            }

            fields.Month = month;
        }
    }

    /// <summary>The proposal's RegulateISODate (s3.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=82579A
    // Broiler-Human:        PENDING
    internal static JsIsoDate RegulateDate(JsEngine engine, double year, double month, double day, bool reject)
    {
        if (reject)
        {
            if (!JsTemporalCore.IsValidIsoDate(year, month, day))
            {
                throw engine.Error("RangeError", "Temporal: " + year + "-" + month + "-" + day + " is not a date");
            }

            return new JsIsoDate((long)year, (int)month, (int)day);
        }

        // OUT-OF-RANGE YEARS STAY AS THEY ARE: the caller's limits refuse them afterwards.
        var clampedMonth = (int)System.Math.Clamp(month, 1, 12);
        var y = (long)System.Math.Clamp(year, -1e15, 1e15);
        var clampedDay = (int)System.Math.Clamp(day, 1, JsTemporalCore.DaysInMonth(y, clampedMonth));
        return new JsIsoDate(y, clampedMonth, clampedDay);
    }

    /// <summary>The proposal's CalendarDateToISO (s12.3.22).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C8E58E
    // Broiler-Human:        PENDING
    private static JsIsoDate DateToIso(JsEngine engine, JsCalendarFields fields, bool reject)
    {
        if (System.Math.Abs(fields.Year!.Value) > 1e9)
        {
            throw engine.Error("RangeError", "Temporal: the year is outside the representable range");
        }

        return RegulateDate(engine, fields.Year!.Value, fields.Month!.Value, fields.Day!.Value, reject);
    }

    /// <summary>The proposal's CalendarDateFromFields (s12.3.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1F7125
    // Broiler-Human:        PENDING
    internal static JsIsoDate DateFromFields(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        ResolveFields(engine, calendar, fields, FieldsType.Date);
        var result = DateToIso(engine, fields, reject);

        if (!JsTemporalCore.DateWithinLimits(result))
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        return result;
    }

    /// <summary>The proposal's CalendarYearMonthFromFields (s12.3.13).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3AE88
    // Broiler-Human:        PENDING
    internal static JsIsoDate YearMonthFromFields(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        fields.Day = 1;
        ResolveFields(engine, calendar, fields, FieldsType.YearMonth);
        var result = DateToIso(engine, fields, reject);

        if (!JsTemporalCore.YearMonthWithinLimits(result))
        {
            throw engine.Error("RangeError", "Temporal: the year-month is outside the representable range");
        }

        return result;
    }

    /// <summary>The proposal's CalendarMonthDayFromFields (s12.3.14) and CalendarMonthDayToISOReferenceDate (s12.3.24).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E112EB
    // Broiler-Human:        PENDING
    internal static JsIsoDate MonthDayFromFields(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        ResolveFields(engine, calendar, fields, FieldsType.MonthDay);

        if (calendar != "iso8601" && fields.Year is { } y && (y < -271821 || y > 275760))
        {
            throw engine.Error("RangeError", "Temporal: the year is outside the representable range");
        }

        var year = fields.Year ?? 1972;

        if (System.Math.Abs(year) > 1e9)
        {
            year = 1972 + (year % 400);
        }

        var regulated = RegulateDate(engine, year, fields.Month!.Value, fields.Day!.Value, reject);
        return new JsIsoDate(1972, regulated.Month, regulated.Day);
    }

    /// <summary>The proposal's CalendarDateAdd (s12.3.7); the Gregorian calendar adds as the ISO one does.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5D69E2
    // Broiler-Human:        PENDING
    internal static JsIsoDate DateAdd(JsEngine engine, JsIsoDate date, JsDateDuration duration, bool reject)
    {
        if (System.Math.Abs(duration.Years) > 1e9 || System.Math.Abs(duration.Months) > 1e10 ||
            System.Math.Abs(duration.Weeks) > 1e10 || System.Math.Abs(duration.Days) > 1e11)
        {
            throw engine.Error("RangeError", "Temporal: the result is outside the representable range");
        }

        var (year, month) = JsTemporalCore.BalanceYearMonth(date.Year + (long)duration.Years, date.Month + (long)duration.Months);
        var intermediate = RegulateDate(engine, year, month, date.Day, reject);
        var result = JsTemporalCore.AddDays(intermediate, (long)duration.Days + (7 * (long)duration.Weeks));

        if (!JsTemporalCore.DateWithinLimits(result))
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        return result;
    }

    /// <summary>The proposal's CompareSurpasses (s3.5.5) for the ISO calendar's numeric months.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E7B7E7
    // Broiler-Human:        PENDING
    private static bool Surpasses(int sign, long year, long month, long day, JsIsoDate target)
    {
        if (year != target.Year)
        {
            return sign * (year - target.Year) > 0;
        }

        if (month != target.Month)
        {
            return sign * (month - target.Month) > 0;
        }

        if (day != target.Day)
        {
            return sign * (day - target.Day) > 0;
        }

        return false;
    }

    /// <summary>
    /// The proposal's ISODateSurpasses (s3.5.6) for years and months: whether the base date moved by
    /// them, its day unregulated, surpasses the target. Weeks and days are not passed: the published
    /// text returns false whenever months is zero, which would never end its own weeks loop, so the
    /// weeks and days are reckoned in whole days afterwards, as the proposal's polyfill reckons them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9E627D
    // Broiler-Human:        PENDING
    private static bool DateSurpasses(int sign, JsIsoDate baseDate, JsIsoDate target, long years, long months)
    {
        var y0 = baseDate.Year + years;

        if (Surpasses(sign, y0, baseDate.Month, baseDate.Day, target))
        {
            return true;
        }

        if (months == 0)
        {
            return false;
        }

        var (year, month) = JsTemporalCore.BalanceYearMonth(y0, baseDate.Month + months);
        return Surpasses(sign, year, month, baseDate.Day, target);
    }

    /// <summary>
    /// The proposal's CalendarDateUntil (s12.3.9) for the ISO and Gregorian calendars: the years and
    /// months the largest counts that do not surpass the second date, found from an estimate rather
    /// than by counting up one at a time; then the weeks and days between the constrained
    /// intermediate date and the second date.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AFDFD4
    // Broiler-Human:        PENDING
    internal static JsDateDuration DateUntil(JsIsoDate one, JsIsoDate two, TemporalUnit largestUnit)
    {
        var sign = -JsTemporalCore.Compare(one, two);

        if (sign == 0)
        {
            return default;
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

        long years = 0;
        long months = 0;

        if (largestUnit is TemporalUnit.Year or TemporalUnit.Month)
        {
            if (largestUnit == TemporalUnit.Year)
            {
                years = Largest(two.Year - one.Year, y => DateSurpasses(sign, one, two, y, 0));
            }

            var estimate = ((two.Year - one.Year - years) * 12) + (two.Month - one.Month);
            months = Largest(estimate, m => DateSurpasses(sign, one, two, years, m));
        }

        var (yearAfter, monthAfter) = JsTemporalCore.BalanceYearMonth(one.Year + years, one.Month + months);
        var constrained = new JsIsoDate(yearAfter, monthAfter, System.Math.Min(one.Day, JsTemporalCore.DaysInMonth(yearAfter, monthAfter)));
        var days = JsTemporalCore.EpochDays(two) - JsTemporalCore.EpochDays(constrained);
        long weeks = 0;

        if (largestUnit == TemporalUnit.Week)
        {
            weeks = days / 7;
            days %= 7;
        }

        return new JsDateDuration(years, months, weeks, days);
    }

    /// <summary>The proposal's CalendarISOToDate (s12.3.26).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E7A98E
    // Broiler-Human:        PENDING
    internal static JsCalendarDate IsoToDate(string calendar, JsIsoDate date)
    {
        var gregory = calendar == "gregory";
        var (week, weekYear) = JsTemporalCore.WeekOfYear(date);

        return new JsCalendarDate(
            gregory ? (date.Year > 0 ? "ce" : "bce") : null,
            gregory ? (date.Year > 0 ? date.Year : 1 - date.Year) : null,
            date.Year,
            date.Month,
            "M" + JsTemporalCore.Padded(date.Month, 2),
            date.Day,
            JsTemporalCore.DayOfWeek(date),
            JsTemporalCore.DayOfYear(date),
            gregory ? null : week,
            gregory ? null : weekYear,
            JsTemporalCore.DaysInMonth(date.Year, date.Month),
            JsTemporalCore.DaysInYear(date.Year),
            JsTemporalCore.IsLeapYear(date.Year));
    }

    /// <summary>The proposal's ISODateToFields (s13.42).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EEE2C8
    // Broiler-Human:        PENDING
    internal static JsCalendarFields DateToFields(string calendar, JsIsoDate date, FieldsType type)
    {
        var parts = IsoToDate(calendar, date);
        var fields = new JsCalendarFields { MonthCode = parts.MonthCode };

        if (type is FieldsType.MonthDay or FieldsType.Date)
        {
            fields.Day = parts.Day;
        }

        if (type is FieldsType.YearMonth or FieldsType.Date)
        {
            fields.Year = parts.Year;
        }

        return fields;
    }

    /// <summary>The proposal's FormatCalendarAnnotation (s12.3.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=02AD1E
    // Broiler-Human:        PENDING
    internal static string CalendarAnnotation(string calendar, string showCalendar) => showCalendar switch
    {
        "never" => string.Empty,
        "auto" when calendar == "iso8601" => string.Empty,
        "critical" => "[!u-ca=" + calendar + "]",
        _ => "[u-ca=" + calendar + "]",
    };
}
