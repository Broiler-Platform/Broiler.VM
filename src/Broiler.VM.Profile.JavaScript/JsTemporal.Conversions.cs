// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   26
// Annotated:        26/26
// Exempt:           0
// Human-reviewed:   0/26
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       26
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // ---- Creating the objects ---------------------------------------------------------------
    //
    // Each limit is the shared library's TemporalLimits (JSD-0078's X3), which takes any year and
    // answers a status: the constructors' years past 2^53 are a RangeError there, where the epoch-day
    // arithmetic's precondition ended the invocation before SJL-10.4 (row R6, decision D9).

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=64BC95
    // Broiler-Human:        PENDING
    private static JsObject PrototypeFor(JsEngine engine, JsValue newTarget, JsObject fallback) =>
        newTarget.IsObject ? engine.PrototypeFromConstructor(newTarget, fallback) : fallback;

    /// <summary>The proposal's CreateTemporalInstant (s8.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ABC489
    // Broiler-Human:        PENDING
    internal static JsInstantObject CreateInstant(JsEngine engine, BigInteger epochNs, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalInstantPrototype!), epochNs);

    /// <summary>The proposal's CreateTemporalDate (s3.5.3): ISODateWithinLimits, the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44BE45
    // Broiler-Human:        PENDING
    internal static JsPlainDateObject CreatePlainDate(JsEngine engine, IsoDate date, string calendar, JsValue newTarget = default)
    {
        Check(engine, TemporalLimits.CheckIsoDate(date), "the date");
        return new JsPlainDateObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainDatePrototype!), date, calendar);
    }

    /// <summary>The proposal's CreateTemporalTime (s4.5.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=24C747
    // Broiler-Human:        PENDING
    internal static JsPlainTimeObject CreatePlainTime(JsEngine engine, IsoTime time, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainTimePrototype!), time);

    /// <summary>The proposal's CreateTemporalDateTime (s5.5.8): ISODateTimeWithinLimits, the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=509587
    // Broiler-Human:        PENDING
    internal static JsPlainDateTimeObject CreatePlainDateTime(JsEngine engine, IsoDateTime dateTime, string calendar, JsValue newTarget = default)
    {
        Check(engine, TemporalLimits.CheckIsoDateTime(dateTime), "the date-time");
        return new JsPlainDateTimeObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainDateTimePrototype!), dateTime, calendar);
    }

    /// <summary>The proposal's CreateTemporalZonedDateTime (s6.5.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0C010A
    // Broiler-Human:        PENDING
    internal static JsZonedDateTimeObject CreateZonedDateTime(JsEngine engine, BigInteger epochNs, string timeZone, string calendar, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalZonedDateTimePrototype!), epochNs, timeZone, calendar);

    /// <summary>The proposal's CreateTemporalYearMonth (s9.5.5): ISOYearMonthWithinLimits, the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8464DA
    // Broiler-Human:        PENDING
    internal static JsPlainYearMonthObject CreateYearMonth(JsEngine engine, IsoDate date, string calendar, JsValue newTarget = default)
    {
        Check(engine, TemporalLimits.CheckIsoYearMonth(date), "the year-month");
        return new JsPlainYearMonthObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainYearMonthPrototype!), date, calendar);
    }

    /// <summary>The proposal's CreateTemporalMonthDay (s10.5.2): ISODateWithinLimits, the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5E1E4C
    // Broiler-Human:        PENDING
    internal static JsPlainMonthDayObject CreateMonthDay(JsEngine engine, IsoDate date, string calendar, JsValue newTarget = default)
    {
        Check(engine, TemporalLimits.CheckIsoDate(date), "the month-day");
        return new JsPlainMonthDayObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainMonthDayPrototype!), date, calendar);
    }

    /// <summary>A time value in milliseconds as epoch nanoseconds: NumberToBigInt, times 10^6.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2509C4
    // Broiler-Human:        PENDING
    internal static BigInteger EpochNsFromMilliseconds(JsEngine engine, double milliseconds) =>
        engine.NumberToBigInt(milliseconds).Value * 1_000_000;

    /// <summary>A RangeError unless the epoch nanoseconds are an instant's (IsValidEpochNanoseconds, the shared library's).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0CBD02
    // Broiler-Human:        PENDING
    internal static BigInteger ValidEpochNs(JsEngine engine, BigInteger epochNs) =>
        TemporalLimits.CheckEpochNanoseconds(epochNs) == TemporalStatus.Completed
            ? epochNs
            : throw engine.Error("RangeError", "Temporal: the epoch nanoseconds are outside the representable range of instants");

    // ---- Converting values ------------------------------------------------------------------

    /// <summary>The proposal's ToTemporalInstant (s8.5.3), as epoch nanoseconds: its string the shared grammar's ParseTemporalInstant.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9C404E
    // Broiler-Human:        PENDING
    internal static BigInteger ToInstant(JsEngine engine, JsValue item)
    {
        if (item.IsObject)
        {
            switch (item.AsObject())
            {
                case JsInstantObject instant:
                    return instant.EpochNanoseconds;
                case JsZonedDateTimeObject zoned:
                    return zoned.EpochNanoseconds;
            }

            item = engine.ToPrimitive(item, "string");
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: an instant must be a string or a Temporal.Instant");
        }

        return JsTemporalParser.Instant(engine, item.AsString());
    }

    /// <summary>Reads the overflow option of an options argument, for the conversions that only validate it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9EC72E
    // Broiler-Human:        PENDING
    private static void ValidateOverflow(JsEngine engine, JsValue options) =>
        _ = Overflow(engine, OptionsObject(engine, options));

    /// <summary>The ISO date of a parse record that has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=080D39
    // Broiler-Human:        PENDING
    private static IsoDate DateOf(IsoParseResult parsed) => new(parsed.Year!.Value, parsed.Month, parsed.Day);

    /// <summary>The proposal's ToTemporalDate (s3.5.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=595EE6
    // Broiler-Human:        PENDING
    internal static JsPlainDateObject ToPlainDate(JsEngine engine, JsValue item, JsValue options = default)
    {
        if (item.IsObject)
        {
            var source = item.AsObject();

            switch (source)
            {
                case JsPlainDateObject date:
                    ValidateOverflow(engine, options);
                    return CreatePlainDate(engine, date.Date, date.Calendar);
                case JsZonedDateTimeObject zoned:
                    var dateTime = IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds);
                    ValidateOverflow(engine, options);
                    return CreatePlainDate(engine, dateTime.Date, zoned.Calendar);
                case JsPlainDateTimeObject plainDateTime:
                    ValidateOverflow(engine, options);
                    return CreatePlainDate(engine, plainDateTime.DateTime.Date, plainDateTime.Calendar);
            }

            var calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(engine, calendar, source, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day], [], []);
            var overflow = Overflow(engine, OptionsObject(engine, options));
            return CreatePlainDate(engine, DateFromFields(engine, calendar, fields, overflow), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a date must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.DateTime);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        return CreatePlainDate(engine, DateOf(parsed), parsedCalendar);
    }

    /// <summary>The proposal's ToTemporalTimeRecord (s4.5.12): each field, or null where partial and absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=210CF2
    // Broiler-Human:        PENDING
    internal static double?[] ToTimeRecord(JsEngine engine, JsValue value, bool partial)
    {
        // READ IN ALPHABETICAL ORDER, STORED IN TABLE 5'S.
        string[] names = ["hour", "microsecond", "millisecond", "minute", "nanosecond", "second"];
        int[] slots = [0, 4, 3, 1, 5, 2];
        var result = new double?[6];

        if (!partial)
        {
            System.Array.Fill(result, 0);
        }

        var any = false;

        for (var i = 0; i < names.Length; i++)
        {
            var property = engine.GetProperty(value, names[i]);

            if (property.Type != JsType.Undefined)
            {
                result[slots[i]] = IntegerWithTruncation(engine, property);
                any = true;
            }
        }

        if (!any)
        {
            throw engine.Error("TypeError", "Temporal: a time-like object needs at least one time property");
        }

        return result;
    }

    /// <summary>The proposal's RegulateTime (s4.5.8), the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7268D5
    // Broiler-Human:        PENDING
    internal static IsoTime RegulateTime(JsEngine engine, double hour, double minute, double second, double millisecond, double microsecond, double nanosecond, TemporalOverflow overflow)
    {
        Check(engine, IsoArithmetic.RegulateTime(hour, minute, second, millisecond, microsecond, nanosecond, overflow, out var time), "the time");
        return time;
    }

    /// <summary>The proposal's ToTemporalTime (s4.5.6), as the time record.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6B0C1D
    // Broiler-Human:        PENDING
    internal static IsoTime ToPlainTime(JsEngine engine, JsValue item, JsValue options = default)
    {
        if (item.IsObject)
        {
            switch (item.AsObject())
            {
                case JsPlainTimeObject time:
                    ValidateOverflow(engine, options);
                    return time.Time;
                case JsPlainDateTimeObject plainDateTime:
                    ValidateOverflow(engine, options);
                    return plainDateTime.DateTime.Time;
                case JsZonedDateTimeObject zoned:
                    var dateTime = IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds);
                    ValidateOverflow(engine, options);
                    return dateTime.Time;
            }

            var record = ToTimeRecord(engine, item, partial: false);
            var overflow = Overflow(engine, OptionsObject(engine, options));
            return RegulateTime(engine, record[0]!.Value, record[1]!.Value, record[2]!.Value, record[3]!.Value, record[4]!.Value, record[5]!.Value, overflow);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a time must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.Time);
        ValidateOverflow(engine, options);
        return parsed.Time!.Value;
    }

    /// <summary>The proposal's ToTimeRecordOrMidnight (s4.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=17119E
    // Broiler-Human:        PENDING
    internal static IsoTime ToTimeOrMidnight(JsEngine engine, JsValue item) =>
        item.Type == JsType.Undefined ? default : ToPlainTime(engine, item);

    /// <summary>The proposal's InterpretTemporalDateTimeFields (s5.5.5): the date the shared calendars give, then the time regulated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=110952
    // Broiler-Human:        PENDING
    internal static IsoDateTime InterpretDateTimeFields(JsEngine engine, string calendar, JsCalendarFields fields, TemporalOverflow overflow)
    {
        var date = DateFromFields(engine, calendar, fields, overflow);
        var time = RegulateTime(engine, fields.Hour ?? 0, fields.Minute ?? 0, fields.Second ?? 0, fields.Millisecond ?? 0, fields.Microsecond ?? 0, fields.Nanosecond ?? 0, overflow);
        return new IsoDateTime(date, time);
    }

    /// <summary>The proposal's ToTemporalDateTime (s5.5.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AD2AB8
    // Broiler-Human:        PENDING
    internal static JsPlainDateTimeObject ToPlainDateTime(JsEngine engine, JsValue item, JsValue options = default)
    {
        if (item.IsObject)
        {
            var source = item.AsObject();

            switch (source)
            {
                case JsPlainDateTimeObject plainDateTime:
                    ValidateOverflow(engine, options);
                    return CreatePlainDateTime(engine, plainDateTime.DateTime, plainDateTime.Calendar);
                case JsZonedDateTimeObject zoned:
                    var dateTime = IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds);
                    ValidateOverflow(engine, options);
                    return CreatePlainDateTime(engine, dateTime, zoned.Calendar);
                case JsPlainDateObject date:
                    ValidateOverflow(engine, options);
                    return CreatePlainDateTime(engine, new IsoDateTime(date.Date, default), date.Calendar);
            }

            var calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(
                engine,
                calendar,
                source,
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond],
                []);
            var overflow = Overflow(engine, OptionsObject(engine, options));
            return CreatePlainDateTime(engine, InterpretDateTimeFields(engine, calendar, fields, overflow), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a date-time must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.DateTime);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        return CreatePlainDateTime(engine, new IsoDateTime(DateOf(parsed), parsed.Time ?? default), parsedCalendar);
    }

    /// <summary>The proposal's ToTemporalZonedDateTime (s6.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=985E9C
    // Broiler-Human:        PENDING
    internal static JsZonedDateTimeObject ToZonedDateTime(JsEngine engine, JsValue item, JsValue options = default)
    {
        var hasUtcDesignator = false;
        var matchMinutes = false;
        string timeZone;
        string? offsetString;
        string calendar;
        string disambiguation;
        string offsetOption;
        IsoDate date;
        IsoTime? time;

        if (item.IsObject)
        {
            var source = item.AsObject();

            if (source is JsZonedDateTimeObject zoned)
            {
                var resolved = OptionsObject(engine, options);
                _ = Disambiguation(engine, resolved);
                _ = OffsetOption(engine, resolved, "reject");
                _ = Overflow(engine, resolved);
                return CreateZonedDateTime(engine, zoned.EpochNanoseconds, zoned.TimeZone, zoned.Calendar);
            }

            calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(
                engine,
                calendar,
                source,
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond, CalendarField.Offset, CalendarField.TimeZone],
                [CalendarField.TimeZone]);
            timeZone = fields.TimeZone!;
            offsetString = fields.Offset;
            var resolvedOptions = OptionsObject(engine, options);
            disambiguation = Disambiguation(engine, resolvedOptions);
            offsetOption = OffsetOption(engine, resolvedOptions, "reject");
            var overflow = Overflow(engine, resolvedOptions);
            var result = InterpretDateTimeFields(engine, calendar, fields, overflow);
            date = result.Date;
            time = result.Time;
        }
        else
        {
            if (!item.IsString)
            {
                throw engine.Error("TypeError", "Temporal: a zoned date-time must be a string, an object or a Temporal object");
            }

            var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.ZonedDateTime);
            timeZone = ToTimeZoneIdentifier(engine, JsValue.String(parsed.TimeZone!));
            offsetString = parsed.Offset;
            hasUtcDesignator = parsed.Z;
            calendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
            matchMinutes = !(offsetString is not null && parsed.OffsetHasSeconds);
            var resolvedOptions = OptionsObject(engine, options);
            disambiguation = Disambiguation(engine, resolvedOptions);
            offsetOption = OffsetOption(engine, resolvedOptions, "reject");
            _ = Overflow(engine, resolvedOptions);
            date = DateOf(parsed);
            time = parsed.Time;
        }

        var offsetBehaviour = hasUtcDesignator ? "exact" : offsetString is null ? "wall" : "option";
        var offsetNs = offsetBehaviour == "option" ? OffsetNanoseconds(offsetString!) : 0;
        var epochNs = InterpretDateTimeOffset(engine, date, time, offsetBehaviour, offsetNs, timeZone, disambiguation, offsetOption, matchMinutes);
        return CreateZonedDateTime(engine, epochNs, timeZone, calendar);
    }

    /// <summary>The proposal's ToTemporalYearMonth (s9.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4DAB84
    // Broiler-Human:        PENDING
    internal static JsPlainYearMonthObject ToYearMonth(JsEngine engine, JsValue item, JsValue options = default)
    {
        if (item.IsObject)
        {
            var source = item.AsObject();

            if (source is JsPlainYearMonthObject yearMonth)
            {
                ValidateOverflow(engine, options);
                return CreateYearMonth(engine, yearMonth.Date, yearMonth.Calendar);
            }

            var calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(engine, calendar, source, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode], [], []);
            var overflow = Overflow(engine, OptionsObject(engine, options));
            return CreateYearMonth(engine, YearMonthFromFields(engine, calendar, fields, overflow), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a year-month must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.YearMonth);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        var isoDate = DateOf(parsed);
        Check(engine, TemporalLimits.CheckIsoYearMonth(isoDate), "the year-month");
        var canonical = YearMonthFromFields(engine, parsedCalendar, DateToFields(engine, parsedCalendar, isoDate, CalendarFieldsType.YearMonth), TemporalOverflow.Constrain);
        return CreateYearMonth(engine, canonical, parsedCalendar);
    }

    /// <summary>The proposal's ToTemporalMonthDay (s10.5.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2A9C75
    // Broiler-Human:        PENDING
    internal static JsPlainMonthDayObject ToMonthDay(JsEngine engine, JsValue item, JsValue options = default)
    {
        if (item.IsObject)
        {
            var source = item.AsObject();

            if (source is JsPlainMonthDayObject monthDay)
            {
                ValidateOverflow(engine, options);
                return CreateMonthDay(engine, monthDay.Date, monthDay.Calendar);
            }

            var calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(engine, calendar, source, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day], [], []);
            var overflow = Overflow(engine, OptionsObject(engine, options));
            return CreateMonthDay(engine, MonthDayFromFields(engine, calendar, fields, overflow), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a month-day must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.MonthDay);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);

        if (parsedCalendar == "iso8601")
        {
            return CreateMonthDay(engine, new IsoDate(1972, parsed.Month, parsed.Day), parsedCalendar);
        }

        var isoDate = DateOf(parsed);
        Check(engine, TemporalLimits.CheckIsoDate(isoDate), "the month-day");
        var canonical = MonthDayFromFields(engine, parsedCalendar, DateToFields(engine, parsedCalendar, isoDate, CalendarFieldsType.MonthDay), TemporalOverflow.Constrain);
        return CreateMonthDay(engine, canonical, parsedCalendar);
    }

    // ---- Strings ----------------------------------------------------------------------------
    //
    // The string forms are the shared library's TemporalFormat (JSD-0078's serialization X6): what
    // stays here is the zone's offset, which the adapter looks up, and the instant's rounding before it.

    /// <summary>
    /// The proposal's TemporalInstantToString (s8.5.8): a null time zone writes UTC as Z; another's
    /// offset at the instant is looked up here, and the string is the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8BAD1F
    // Broiler-Human:        PENDING
    internal static string InstantString(JsEngine engine, BigInteger epochNs, string? timeZone, int precision) =>
        TemporalFormat.InstantToString(epochNs, timeZone is null ? null : OffsetNanosecondsFor(engine, timeZone, epochNs), precision);

    /// <summary>
    /// The proposal's TemporalZonedDateTimeToString (s6.5.4): the instant rounded, the zone's offset at
    /// the rounded instant looked up here, and the string the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DDB7AC
    // Broiler-Human:        PENDING
    internal static string ZonedDateTimeString(
        JsEngine engine,
        JsZonedDateTimeObject zoned,
        int precision,
        ShowCalendarName showCalendar,
        ShowTimeZoneName showTimeZone,
        ShowOffset showOffset,
        long increment = 1,
        TemporalUnit unit = TemporalUnit.Nanosecond,
        RoundingMode mode = RoundingMode.Trunc)
    {
        var epochNs = IsoArithmetic.RoundTemporalInstant(zoned.EpochNanoseconds, increment, unit, mode);
        var offsetNs = OffsetNanosecondsFor(engine, zoned.TimeZone, epochNs);
        return TemporalFormat.ZonedDateTimeToString(epochNs, offsetNs, zoned.TimeZone, zoned.Calendar, precision, showCalendar, showTimeZone, showOffset);
    }
}
