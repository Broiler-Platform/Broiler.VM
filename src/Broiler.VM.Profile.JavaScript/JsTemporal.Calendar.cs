// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           28
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

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

/// <summary>
/// A Calendar Fields Record (s12.3.2) as PrepareCalendarFields reads it: each field a value or unset
/// (null). Its calendar half is the shared library's <see cref="CalendarFields"/>; the time, offset and
/// time zone fields are the engine's, which no calendar operation reads.
/// </summary>
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A1B71
    // Broiler-Human:        PENDING
    internal MonthCode? MonthCode { get; set; }

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

    /// <summary>The calendar half, as the shared calendars take it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E95EB
    // Broiler-Human:        PENDING
    internal CalendarFields Calendar => new()
    {
        Era = Era,
        EraYear = EraYear,
        Year = Year,
        Month = Month,
        MonthCode = MonthCode,
        Day = Day,
    };

    /// <summary>A record whose calendar half is <paramref name="fields"/> and whose other fields are unset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=680D06
    // Broiler-Human:        PENDING
    internal static JsCalendarFields Of(CalendarFields fields) => new()
    {
        Era = fields.Era,
        EraYear = fields.EraYear,
        Year = fields.Year,
        Month = fields.Month,
        MonthCode = fields.MonthCode,
        Day = fields.Day,
    };
}

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED29FE
    // Broiler-Human:        PENDING
    private static readonly string[] FieldProperties =
        ["era", "eraYear", "year", "month", "monthCode", "day", "hour", "minute", "second", "millisecond", "microsecond", "nanosecond", "offset", "timeZone"];

    /// <summary>
    /// The proposal's CanonicalizeCalendar (s12.1.1), the shared library's over the calendars of the
    /// Intl era and month code proposal's Table 1 with its two aliases (JSD-0056, JSD-0078 decision D5).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5D8D8A
    // Broiler-Human:        PENDING
    internal static string CanonicalizeCalendar(JsEngine engine, string id) =>
        TemporalCalendars.Canonicalize(id, out var calendar) is var status && status == CalendarStatus.Completed
            ? TemporalCalendars.Identifier(calendar)
            : throw CalendarError(engine, status, id);

    /// <summary>The shared library's identity of a canonical calendar type a Temporal object holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A00E79
    // Broiler-Human:        PENDING
    internal static CalendarId CalendarIdOf(string calendar) =>
        TemporalCalendars.Canonicalize(calendar, out var id) == CalendarStatus.Completed
            ? id
            : throw new System.InvalidOperationException("A Temporal object holds a calendar that is not canonical: " + calendar);

    /// <summary>
    /// The calendars of the realm's Intl data, the shared library's <see cref="TemporalCalendars"/>
    /// over its calendar table (JSD-0078): Temporal is built only with Intl (JSD-0054), so the data is
    /// there.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=53096A
    // Broiler-Human:        PENDING
    internal static TemporalCalendars Calendars(JsEngine engine) => engine.Intl!.Calendars;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9A112C
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

        return CanonicalizeCalendar(engine, JsTemporalParser.CalendarString(engine, value.AsString()));
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

    /// <summary>The proposal's CalendarExtraFields (s12.3.27), whether a calendar has eras the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0C3405
    // Broiler-Human:        PENDING
    private static CalendarField[] ExtraFields(string calendar, CalendarField[] fields) =>
        TemporalCalendars.SupportsEra(CalendarIdOf(calendar)) && System.Array.IndexOf(fields, CalendarField.Year) >= 0
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

    /// <summary>
    /// The month code a field's value is: ToPrimitive and its TypeError here, then the proposal's
    /// ParseMonthCode (s12.2.1), the shared library's <see cref="MonthCode.Parse"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=98A90E
    // Broiler-Human:        PENDING
    private static MonthCode MonthCodeOf(JsEngine engine, JsValue value)
    {
        var primitive = engine.ToPrimitive(value, "string");

        if (!primitive.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a month code must be a string");
        }

        var code = primitive.AsString();
        return MonthCode.Parse(code, out var monthCode) == CalendarStatus.Completed
            ? monthCode
            : throw engine.Error("RangeError", "Temporal: " + code + " is not a month code");
    }

    /// <summary>
    /// The proposal's CalendarMergeFields (s12.3.5): the calendar fields the shared library merges,
    /// with its CalendarFieldKeysToIgnore; each time, offset and time zone field <paramref name="additional"/>
    /// holds overrides, which is what those keys ignore.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CE60A4
    // Broiler-Human:        PENDING
    internal static JsCalendarFields MergeFields(string calendar, JsCalendarFields fields, JsCalendarFields additional)
    {
        var merged = JsCalendarFields.Of(TemporalCalendars.MergeFields(CalendarIdOf(calendar), fields.Calendar, additional.Calendar));
        merged.Hour = additional.Hour ?? fields.Hour;
        merged.Minute = additional.Minute ?? fields.Minute;
        merged.Second = additional.Second ?? fields.Second;
        merged.Millisecond = additional.Millisecond ?? fields.Millisecond;
        merged.Microsecond = additional.Microsecond ?? fields.Microsecond;
        merged.Nanosecond = additional.Nanosecond ?? fields.Nanosecond;
        merged.Offset = additional.Offset ?? fields.Offset;
        merged.TimeZone = additional.TimeZone ?? fields.TimeZone;
        return merged;
    }

    /// <summary>A Temporal overflow as the shared calendars take it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=88F0D3
    // Broiler-Human:        PENDING
    private static CalendarOverflow CalendarOverflowOf(TemporalOverflow overflow) =>
        overflow == TemporalOverflow.Reject ? CalendarOverflow.Reject : CalendarOverflow.Constrain;

    /// <summary>The proposal's CalendarDateFromFields (s12.3.12), the shared calendars'.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=990BFC
    // Broiler-Human:        PENDING
    internal static IsoDate DateFromFields(JsEngine engine, string calendar, JsCalendarFields fields, TemporalOverflow overflow)
    {
        Check(engine, Calendars(engine).DateFromFields(CalendarIdOf(calendar), fields.Calendar, CalendarOverflowOf(overflow), out var result), calendar);
        return result;
    }

    /// <summary>The proposal's CalendarYearMonthFromFields (s12.3.13), the shared calendars'.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=00FD99
    // Broiler-Human:        PENDING
    internal static IsoDate YearMonthFromFields(JsEngine engine, string calendar, JsCalendarFields fields, TemporalOverflow overflow)
    {
        Check(engine, Calendars(engine).YearMonthFromFields(CalendarIdOf(calendar), fields.Calendar, CalendarOverflowOf(overflow), out var result), calendar, "the year-month");
        return result;
    }

    /// <summary>The proposal's CalendarMonthDayFromFields (s12.3.14) with its reference date, the shared calendars'.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=11C726
    // Broiler-Human:        PENDING
    internal static IsoDate MonthDayFromFields(JsEngine engine, string calendar, JsCalendarFields fields, TemporalOverflow overflow)
    {
        Check(engine, Calendars(engine).MonthDayFromFields(CalendarIdOf(calendar), fields.Calendar, CalendarOverflowOf(overflow), out var result), calendar, "the month-day");
        return result;
    }

    /// <summary>
    /// The proposal's CalendarDateAdd (s12.3.7), the shared calendars': a valid duration's date part,
    /// each field an exact integer of magnitude below 2^53 and so held exactly.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FD6336
    // Broiler-Human:        PENDING
    internal static IsoDate DateAdd(JsEngine engine, string calendar, IsoDate date, DateDuration duration, TemporalOverflow overflow)
    {
        var exact = new CalendarDuration((long)duration.Years, (long)duration.Months, (long)duration.Weeks, (long)duration.Days);
        Check(engine, Calendars(engine).DateAdd(CalendarIdOf(calendar), date, exact, CalendarOverflowOf(overflow), out var result), calendar, "the result");
        return result;
    }

    /// <summary>The proposal's CalendarISOToDate (s12.3.26), the shared calendars'.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE7275
    // Broiler-Human:        PENDING
    internal static CalendarDate IsoToDate(JsEngine engine, string calendar, IsoDate date)
    {
        Check(engine, Calendars(engine).IsoToDate(CalendarIdOf(calendar), date, out var result), calendar);
        return result;
    }

    /// <summary>The proposal's ISODateToFields (s13.42), the shared calendars'.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0C93CA
    // Broiler-Human:        PENDING
    internal static JsCalendarFields DateToFields(JsEngine engine, string calendar, IsoDate date, CalendarFieldsType type)
    {
        Check(engine, Calendars(engine).DateToFields(CalendarIdOf(calendar), date, type, out var result), calendar);
        return JsCalendarFields.Of(result);
    }
}
