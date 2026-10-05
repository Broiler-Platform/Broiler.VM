// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           0
// Human-reviewed:   0/28
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       28
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // ---- Creating the objects ---------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=64BC95
    // Broiler-Human:        PENDING
    private static JsObject PrototypeFor(JsEngine engine, JsValue newTarget, JsObject fallback) =>
        newTarget.IsObject ? engine.PrototypeFromConstructor(newTarget, fallback) : fallback;

    /// <summary>The proposal's CreateTemporalInstant (s8.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ABC489
    // Broiler-Human:        PENDING
    internal static JsInstantObject CreateInstant(JsEngine engine, BigInteger epochNs, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalInstantPrototype!), epochNs);

    /// <summary>The proposal's CreateTemporalDate (s3.5.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4C581C
    // Broiler-Human:        PENDING
    internal static JsPlainDateObject CreatePlainDate(JsEngine engine, JsIsoDate date, string calendar, JsValue newTarget = default)
    {
        if (!JsTemporalCore.DateWithinLimits(date))
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        return new JsPlainDateObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainDatePrototype!), date, calendar);
    }

    /// <summary>The proposal's CreateTemporalTime (s4.5.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E6D36
    // Broiler-Human:        PENDING
    internal static JsPlainTimeObject CreatePlainTime(JsEngine engine, JsTimeRecord time, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainTimePrototype!), time);

    /// <summary>The proposal's CreateTemporalDateTime (s5.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=294B00
    // Broiler-Human:        PENDING
    internal static JsPlainDateTimeObject CreatePlainDateTime(JsEngine engine, JsIsoDateTime dateTime, string calendar, JsValue newTarget = default)
    {
        if (!JsTemporalCore.DateTimeWithinLimits(dateTime))
        {
            throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
        }

        return new JsPlainDateTimeObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainDateTimePrototype!), dateTime, calendar);
    }

    /// <summary>The proposal's CreateTemporalZonedDateTime (s6.5.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0C010A
    // Broiler-Human:        PENDING
    internal static JsZonedDateTimeObject CreateZonedDateTime(JsEngine engine, BigInteger epochNs, string timeZone, string calendar, JsValue newTarget = default) =>
        new(PrototypeFor(engine, newTarget, engine.Realm.TemporalZonedDateTimePrototype!), epochNs, timeZone, calendar);

    /// <summary>The proposal's CreateTemporalYearMonth (s9.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=46B4C5
    // Broiler-Human:        PENDING
    internal static JsPlainYearMonthObject CreateYearMonth(JsEngine engine, JsIsoDate date, string calendar, JsValue newTarget = default)
    {
        if (!JsTemporalCore.YearMonthWithinLimits(date))
        {
            throw engine.Error("RangeError", "Temporal: the year-month is outside the representable range");
        }

        return new JsPlainYearMonthObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainYearMonthPrototype!), date, calendar);
    }

    /// <summary>The proposal's CreateTemporalMonthDay (s10.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=250243
    // Broiler-Human:        PENDING
    internal static JsPlainMonthDayObject CreateMonthDay(JsEngine engine, JsIsoDate date, string calendar, JsValue newTarget = default)
    {
        if (!JsTemporalCore.DateWithinLimits(date))
        {
            throw engine.Error("RangeError", "Temporal: the month-day is outside the representable range");
        }

        return new JsPlainMonthDayObject(PrototypeFor(engine, newTarget, engine.Realm.TemporalPlainMonthDayPrototype!), date, calendar);
    }

    /// <summary>A time value in milliseconds as epoch nanoseconds: NumberToBigInt, times 10^6.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2509C4
    // Broiler-Human:        PENDING
    internal static BigInteger EpochNsFromMilliseconds(JsEngine engine, double milliseconds) =>
        engine.NumberToBigInt(milliseconds).Value * 1_000_000;

    /// <summary>A RangeError unless the epoch nanoseconds are an instant's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=902C4E
    // Broiler-Human:        PENDING
    internal static BigInteger ValidEpochNs(JsEngine engine, BigInteger epochNs) =>
        JsTemporalCore.IsValidEpochNs(epochNs)
            ? epochNs
            : throw engine.Error("RangeError", "Temporal: the epoch nanoseconds are outside the representable range of instants");

    // ---- Converting values ------------------------------------------------------------------

    /// <summary>The proposal's ToTemporalInstant (s8.5.3), as epoch nanoseconds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A1227D
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

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.Instant);
        var offsetNs = parsed.Z ? 0 : JsTemporalParser.OffsetNanoseconds(parsed.Offset!);
        var time = parsed.Time!.Value;
        var balanced = JsTemporalCore.BalanceDateTime(
            new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day),
            JsTemporalCore.TimeNs(time) - offsetNs);

        if (!JsTemporalCore.DaysWithinRange(balanced.Date))
        {
            throw engine.Error("RangeError", "Temporal: the instant is outside the representable range");
        }

        return ValidEpochNs(engine, JsTemporalCore.UtcEpochNs(balanced));
    }

    /// <summary>Reads the overflow option of an options argument, for the conversions that only validate it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=625430
    // Broiler-Human:        PENDING
    private static void ValidateOverflow(JsEngine engine, JsValue options) =>
        _ = Reject(engine, OptionsObject(engine, options));

    /// <summary>The proposal's ToTemporalDate (s3.5.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=09BEAD
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
            var reject = Reject(engine, OptionsObject(engine, options));
            return CreatePlainDate(engine, DateFromFields(engine, calendar, fields, reject), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a date must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.DateTime);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        return CreatePlainDate(engine, new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day), parsedCalendar);
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

    /// <summary>The proposal's ToTemporalTime (s4.5.6), as the time record.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BF5189
    // Broiler-Human:        PENDING
    internal static JsTimeRecord ToPlainTime(JsEngine engine, JsValue item, JsValue options = default)
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
            var reject = Reject(engine, OptionsObject(engine, options));
            return RegulateTime(engine, record[0]!.Value, record[1]!.Value, record[2]!.Value, record[3]!.Value, record[4]!.Value, record[5]!.Value, reject);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6DBE23
    // Broiler-Human:        PENDING
    internal static JsTimeRecord ToTimeOrMidnight(JsEngine engine, JsValue item) =>
        item.Type == JsType.Undefined ? JsTimeRecord.Midnight : ToPlainTime(engine, item);

    /// <summary>The proposal's ToTemporalDateTime (s5.5.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B5C39B
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
                    return CreatePlainDateTime(engine, new JsIsoDateTime(date.Date, JsTimeRecord.Midnight), date.Calendar);
            }

            var calendar = CalendarWithIsoDefault(engine, source);
            var fields = PrepareFields(
                engine,
                calendar,
                source,
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond],
                []);
            var reject = Reject(engine, OptionsObject(engine, options));
            return CreatePlainDateTime(engine, InterpretDateTimeFields(engine, calendar, fields, reject), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a date-time must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.DateTime);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        var isoDateTime = new JsIsoDateTime(new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day), parsed.Time ?? JsTimeRecord.Midnight);
        return CreatePlainDateTime(engine, isoDateTime, parsedCalendar);
    }

    /// <summary>The proposal's ToTemporalZonedDateTime (s6.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CB656F
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
        JsIsoDate date;
        JsTimeRecord? time;

        if (item.IsObject)
        {
            var source = item.AsObject();

            if (source is JsZonedDateTimeObject zoned)
            {
                var resolved = OptionsObject(engine, options);
                _ = Disambiguation(engine, resolved);
                _ = OffsetOption(engine, resolved, "reject");
                _ = Reject(engine, resolved);
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
            var reject = Reject(engine, resolvedOptions);
            var result = InterpretDateTimeFields(engine, calendar, fields, reject);
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
            matchMinutes = !(offsetString is not null && OffsetHasSeconds(offsetString));
            var resolvedOptions = OptionsObject(engine, options);
            disambiguation = Disambiguation(engine, resolvedOptions);
            offsetOption = OffsetOption(engine, resolvedOptions, "reject");
            _ = Reject(engine, resolvedOptions);
            date = new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day);
            time = parsed.Time;
        }

        var offsetBehaviour = hasUtcDesignator ? "exact" : offsetString is null ? "wall" : "option";
        var offsetNs = offsetBehaviour == "option" ? JsTemporalParser.OffsetNanoseconds(offsetString!) : 0;
        var epochNs = InterpretDateTimeOffset(engine, date, time, offsetBehaviour, offsetNs, timeZone, disambiguation, offsetOption, matchMinutes);
        return CreateZonedDateTime(engine, epochNs, timeZone, calendar);
    }

    /// <summary>The proposal's ToTemporalYearMonth (s9.5.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7EA162
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
            var reject = Reject(engine, OptionsObject(engine, options));
            return CreateYearMonth(engine, YearMonthFromFields(engine, calendar, fields, reject), calendar);
        }

        if (!item.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a year-month must be a string, an object or a Temporal object");
        }

        var parsed = JsTemporalParser.Parse(engine, item.AsString(), TemporalGoal.YearMonth);
        var parsedCalendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
        ValidateOverflow(engine, options);
        var isoDate = new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day);

        if (!JsTemporalCore.YearMonthWithinLimits(isoDate))
        {
            throw engine.Error("RangeError", "Temporal: the year-month is outside the representable range");
        }

        var canonical = YearMonthFromFields(engine, parsedCalendar, DateToFields(parsedCalendar, isoDate, FieldsType.YearMonth), reject: false);
        return CreateYearMonth(engine, canonical, parsedCalendar);
    }

    /// <summary>The proposal's ToTemporalMonthDay (s10.5.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CEE284
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
            var reject = Reject(engine, OptionsObject(engine, options));
            return CreateMonthDay(engine, MonthDayFromFields(engine, calendar, fields, reject), calendar);
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
            return CreateMonthDay(engine, new JsIsoDate(1972, parsed.Month, parsed.Day), parsedCalendar);
        }

        var isoDate = new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day);

        if (!JsTemporalCore.DateWithinLimits(isoDate))
        {
            throw engine.Error("RangeError", "Temporal: the month-day is outside the representable range");
        }

        var canonical = MonthDayFromFields(engine, parsedCalendar, DateToFields(parsedCalendar, isoDate, FieldsType.MonthDay), reject: false);
        return CreateMonthDay(engine, canonical, parsedCalendar);
    }

    // ---- Strings ----------------------------------------------------------------------------

    /// <summary>The proposal's TemporalDateToString (s3.5.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0A811A
    // Broiler-Human:        PENDING
    internal static string DateString(JsIsoDate date, string calendar, string showCalendar) =>
        JsTemporalCore.DateToString(date) + CalendarAnnotation(calendar, showCalendar);

    /// <summary>The proposal's ISODateTimeToString (s5.5.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=150BA6
    // Broiler-Human:        PENDING
    internal static string DateTimeString(JsIsoDateTime dateTime, string calendar, int precision, string showCalendar) =>
        JsTemporalCore.DateToString(dateTime.Date) + "T" + JsTemporalCore.TimeToString(dateTime.Time, precision) +
        CalendarAnnotation(calendar, showCalendar);

    /// <summary>The proposal's TemporalYearMonthToString (s9.5.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=76970B
    // Broiler-Human:        PENDING
    internal static string YearMonthString(JsIsoDate date, string calendar, string showCalendar)
    {
        var result = JsTemporalCore.PadYear(date.Year) + "-" + JsTemporalCore.Padded(date.Month, 2);

        if (showCalendar is "always" or "critical" || calendar != "iso8601")
        {
            result += "-" + JsTemporalCore.Padded(date.Day, 2);
        }

        return result + CalendarAnnotation(calendar, showCalendar);
    }

    /// <summary>The proposal's TemporalMonthDayToString (s10.5.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E996EA
    // Broiler-Human:        PENDING
    internal static string MonthDayString(JsIsoDate date, string calendar, string showCalendar)
    {
        var result = JsTemporalCore.Padded(date.Month, 2) + "-" + JsTemporalCore.Padded(date.Day, 2);

        if (showCalendar is "always" or "critical" || calendar != "iso8601")
        {
            result = JsTemporalCore.PadYear(date.Year) + "-" + result;
        }

        return result + CalendarAnnotation(calendar, showCalendar);
    }

    /// <summary>The proposal's TemporalInstantToString (s8.5.8): a null time zone writes UTC as Z.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A15A1C
    // Broiler-Human:        PENDING
    internal static string InstantString(JsEngine engine, BigInteger epochNs, string? timeZone, int precision)
    {
        var outputTimeZone = timeZone ?? "UTC";
        var offsetNs = OffsetNanosecondsFor(engine, outputTimeZone, epochNs);
        var dateTime = IsoDateTimeFor(engine, outputTimeZone, epochNs);
        var text = DateTimeString(dateTime, "iso8601", precision, "never");
        return text + (timeZone is null ? "Z" : JsTemporalCore.FormatOffsetRounded(offsetNs));
    }

    /// <summary>The proposal's TemporalZonedDateTimeToString (s6.5.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8C2DC2
    // Broiler-Human:        PENDING
    internal static string ZonedDateTimeString(
        JsEngine engine,
        JsZonedDateTimeObject zoned,
        int precision,
        string showCalendar,
        string showTimeZone,
        string showOffset,
        long increment = 1,
        TemporalUnit unit = TemporalUnit.Nanosecond,
        TemporalRounding mode = TemporalRounding.Trunc)
    {
        var epochNs = RoundInstant(zoned.EpochNanoseconds, increment, unit, mode);
        var offsetNs = OffsetNanosecondsFor(engine, zoned.TimeZone, epochNs);
        var dateTime = IsoDateTimeFor(engine, zoned.TimeZone, epochNs);
        var text = DateTimeString(dateTime, "iso8601", precision, "never");

        if (showOffset != "never")
        {
            text += JsTemporalCore.FormatOffsetRounded(offsetNs);
        }

        if (showTimeZone != "never")
        {
            text += "[" + (showTimeZone == "critical" ? "!" : string.Empty) + zoned.TimeZone + "]";
        }

        return text + CalendarAnnotation(zoned.Calendar, showCalendar);
    }

    /// <summary>The proposal's RoundTemporalInstant (s8.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E08078
    // Broiler-Human:        PENDING
    internal static BigInteger RoundInstant(BigInteger epochNs, long increment, TemporalUnit unit, TemporalRounding mode) =>
        JsTemporalCore.RoundToIncrementAsIfPositive(epochNs, JsTemporalCore.UnitLength(unit) * increment, mode);
}
