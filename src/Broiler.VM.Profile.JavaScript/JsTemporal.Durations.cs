// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   45
// Annotated:        45/45
// Exempt:           0
// Human-reviewed:   0/45
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       45
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>An exact fraction, its denominator positive: the proposal's mathematical values that are not integers.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=89B610
// Broiler-Human:        PENDING
internal readonly record struct JsFraction(BigInteger Numerator, BigInteger Denominator)
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBB6CC
    // Broiler-Human:        PENDING
    internal double ToDouble() => JsTemporalCore.ToDouble(Numerator, Denominator);
}

/// <summary>The record GetTemporalRelativeToOption returns (s13.19): a plain date, a zoned instant, or neither.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F47A9A
// Broiler-Human:        PENDING
internal readonly record struct JsRelativeTo(JsIsoDate? PlainDate, BigInteger? ZonedNs, string? TimeZone, string Calendar)
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=22A776
    // Broiler-Human:        PENDING
    internal bool IsZoned => ZonedNs is not null;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37EDCF
    // Broiler-Human:        PENDING
    internal bool IsPlain => PlainDate is not null;
}

/// <summary>A Duration Nudge Result Record (s7.5.32).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=046FBD
// Broiler-Human:        PENDING
internal readonly record struct JsNudge(JsInternalDuration Duration, BigInteger NudgedEpochNs, bool DidExpandCalendarUnit);

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // ---- Creating and reading durations ---------------------------------------------------------

    /// <summary>The proposal's CreateTemporalDuration (s7.5.19).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=28AE52
    // Broiler-Human:        PENDING
    internal static JsDurationObject CreateDuration(JsEngine engine, double[] fields, JsValue newTarget = default)
    {
        if (!JsTemporalCore.IsValidDuration(fields))
        {
            throw engine.Error("RangeError", "Temporal: the duration is not valid: mixed signs, or too large");
        }

        var prototype = newTarget.IsObject
            ? engine.PrototypeFromConstructor(newTarget, engine.Realm.TemporalDurationPrototype!)
            : engine.Realm.TemporalDurationPrototype!;

        for (var i = 0; i < fields.Length; i++)
        {
            fields[i] += 0.0;
        }

        return new JsDurationObject(prototype, fields);
    }

    /// <summary>The proposal's CreateNegatedTemporalDuration (s7.5.20).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A9341D
    // Broiler-Human:        PENDING
    internal static double[] Negated(double[] fields)
    {
        var result = new double[10];

        for (var i = 0; i < 10; i++)
        {
            result[i] = fields[i] == 0 ? 0 : -fields[i];
        }

        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9C811
    // Broiler-Human:        PENDING
    private static readonly string[] DurationProperties =
        ["days", "hours", "microseconds", "milliseconds", "minutes", "months", "nanoseconds", "seconds", "weeks", "years"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A6DDCC
    // Broiler-Human:        PENDING
    private static readonly int[] DurationPropertyIndex = [3, 4, 8, 7, 5, 1, 9, 6, 2, 0];

    /// <summary>The proposal's ToTemporalPartialDurationRecord (s7.5.18): a field per unit, NaN where absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=369B2D
    // Broiler-Human:        PENDING
    internal static double[] ToPartialDuration(JsEngine engine, JsValue value)
    {
        if (!value.IsObject)
        {
            throw engine.Error("TypeError", "Temporal: a duration-like value must be an object");
        }

        var result = new double[10];
        System.Array.Fill(result, double.NaN);
        var any = false;

        for (var i = 0; i < DurationProperties.Length; i++)
        {
            var property = engine.GetProperty(value, DurationProperties[i]);

            if (property.Type != JsType.Undefined)
            {
                result[DurationPropertyIndex[i]] = IntegerIfIntegral(engine, property);
                any = true;
            }
        }

        if (!any)
        {
            throw engine.Error("TypeError", "Temporal: a duration-like object needs at least one duration property");
        }

        return result;
    }

    /// <summary>The proposal's ToTemporalDuration (s7.5.12), as the ten fields.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=61A532
    // Broiler-Human:        PENDING
    internal static double[] ToDuration(JsEngine engine, JsValue item)
    {
        if (item.AsObjectOrNull() is JsDurationObject duration)
        {
            return (double[])duration.Fields.Clone();
        }

        if (!item.IsObject)
        {
            if (!item.IsString)
            {
                throw engine.Error("TypeError", "Temporal: a duration must be a string, an object or a Temporal.Duration");
            }

            var parsed = JsTemporalParser.Duration(item.AsString()) ??
                throw engine.Error("RangeError", "Temporal: " + item.AsString() + " is not a duration");

            return CreateDuration(engine, parsed).Fields;
        }

        var partial = ToPartialDuration(engine, item);

        for (var i = 0; i < 10; i++)
        {
            if (double.IsNaN(partial[i]))
            {
                partial[i] = 0;
            }
        }

        return CreateDuration(engine, partial).Fields;
    }

    /// <summary>The proposal's TemporalDurationFromInternal (s7.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=89C732
    // Broiler-Human:        PENDING
    internal static JsDurationObject FromInternal(JsEngine engine, JsInternalDuration duration, TemporalUnit largestUnit) =>
        CreateDuration(engine, JsTemporalCore.FromInternal(duration, largestUnit));

    /// <summary>The proposal's CreateDateDurationRecord (s7.5.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=47CC1A
    // Broiler-Human:        PENDING
    internal static JsDateDuration DateDurationRecord(JsEngine engine, double years, double months, double weeks, double days)
    {
        if (!JsTemporalCore.IsValidDuration([years, months, weeks, days, 0, 0, 0, 0, 0, 0]))
        {
            throw engine.Error("RangeError", "Temporal: the duration is not valid");
        }

        return new JsDateDuration(years + 0.0, months + 0.0, weeks + 0.0, days + 0.0);
    }

    /// <summary>The proposal's AdjustDateDurationRecord (s7.5.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=575234
    // Broiler-Human:        PENDING
    internal static JsDateDuration AdjustDate(JsEngine engine, JsDateDuration date, double days, double? weeks = null, double? months = null) =>
        DateDurationRecord(engine, date.Years, months ?? date.Months, weeks ?? date.Weeks, days);

    /// <summary>The proposal's AddTimeDuration (s7.5.22).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1A09AB
    // Broiler-Human:        PENDING
    internal static BigInteger AddTimeDuration(JsEngine engine, BigInteger one, BigInteger two)
    {
        var result = one + two;

        if (BigInteger.Abs(result) > JsTemporalCore.MaxTimeDuration)
        {
            throw engine.Error("RangeError", "Temporal: the time duration is too large");
        }

        return result;
    }

    /// <summary>The proposal's Add24HourDaysToTimeDuration (s7.5.23).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD9241
    // Broiler-Human:        PENDING
    internal static BigInteger Add24HourDays(JsEngine engine, BigInteger time, double days) =>
        AddTimeDuration(engine, time, JsTemporalCore.Exact(days) * JsTemporalCore.NsPerDay);

    /// <summary>The proposal's RoundTimeDurationToIncrement (s7.5.27).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=282D4D
    // Broiler-Human:        PENDING
    internal static BigInteger RoundTimeDurationToIncrement(JsEngine engine, BigInteger duration, BigInteger increment, TemporalRounding mode)
    {
        var rounded = JsTemporalCore.RoundToIncrement(duration, increment, mode);

        if (BigInteger.Abs(rounded) > JsTemporalCore.MaxTimeDuration)
        {
            throw engine.Error("RangeError", "Temporal: the rounded time duration is too large");
        }

        return rounded;
    }

    /// <summary>The proposal's RoundTimeDuration (s7.5.30).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=28BC96
    // Broiler-Human:        PENDING
    internal static BigInteger RoundTimeDuration(JsEngine engine, BigInteger duration, long increment, TemporalUnit unit, TemporalRounding mode) =>
        RoundTimeDurationToIncrement(engine, duration, JsTemporalCore.UnitLength(unit) * increment, mode);

    /// <summary>The proposal's TotalTimeDuration (s7.5.31), exact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B485FA
    // Broiler-Human:        PENDING
    internal static JsFraction TotalTimeDuration(BigInteger duration, TemporalUnit unit) =>
        new(duration, JsTemporalCore.UnitLength(unit));

    /// <summary>The proposal's DifferenceInstant (s8.5.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E66BFC
    // Broiler-Human:        PENDING
    internal static JsInternalDuration DifferenceInstant(JsEngine engine, BigInteger ns1, BigInteger ns2, long increment, TemporalUnit smallestUnit, TemporalRounding mode) =>
        new(default, RoundTimeDuration(engine, ns2 - ns1, increment, smallestUnit, mode));

    // ---- relativeTo ---------------------------------------------------------------------------

    /// <summary>The proposal's GetTemporalRelativeToOption (s13.19).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=919FCA
    // Broiler-Human:        PENDING
    internal static JsRelativeTo RelativeToOption(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "relativeTo");

        if (value.Type == JsType.Undefined)
        {
            return default;
        }

        var offsetBehaviour = "option";
        var matchMinutes = false;
        JsIsoDate date;
        JsTimeRecord? time;
        string? timeZone;
        string? offsetString;
        string calendar;

        if (value.IsObject)
        {
            switch (value.AsObject())
            {
                case JsZonedDateTimeObject zoned:
                    return new JsRelativeTo(null, zoned.EpochNanoseconds, zoned.TimeZone, zoned.Calendar);
                case JsPlainDateObject plainDate:
                    return new JsRelativeTo(plainDate.Date, null, null, plainDate.Calendar);
                case JsPlainDateTimeObject plainDateTime:
                    return new JsRelativeTo(plainDateTime.DateTime.Date, null, null, plainDateTime.Calendar);
            }

            calendar = CalendarWithIsoDefault(engine, value.AsObject());
            var fields = PrepareFields(
                engine,
                calendar,
                value.AsObject(),
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond, CalendarField.Offset, CalendarField.TimeZone],
                []);

            var result = InterpretDateTimeFields(engine, calendar, fields, reject: false);
            timeZone = fields.TimeZone;
            offsetString = fields.Offset;

            if (offsetString is null)
            {
                offsetBehaviour = "wall";
            }

            date = result.Date;
            time = result.Time;
        }
        else
        {
            if (!value.IsString)
            {
                throw engine.Error("TypeError", "Temporal: relativeTo must be a string, an object or a Temporal object");
            }

            var parsed = JsTemporalParser.Parse(engine, value.AsString(), TemporalGoal.ZonedDateTime, TemporalGoal.DateTime);
            offsetString = parsed.Offset;
            timeZone = parsed.TimeZone is null ? null : ToTimeZoneIdentifier(engine, JsValue.String(parsed.TimeZone));

            if (parsed.Z)
            {
                offsetBehaviour = "exact";
            }
            else if (offsetString is null)
            {
                offsetBehaviour = "wall";
            }

            matchMinutes = !(offsetString is not null && OffsetHasSeconds(offsetString));

            calendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
            date = new JsIsoDate(parsed.Year!.Value, parsed.Month, parsed.Day);
            time = parsed.Time;
        }

        if (timeZone is null)
        {
            if (!JsTemporalCore.DateWithinLimits(date))
            {
                throw engine.Error("RangeError", "Temporal: relativeTo is outside the representable range");
            }

            return new JsRelativeTo(date, null, null, calendar);
        }

        var offsetNs = offsetBehaviour == "option" ? JsTemporalParser.OffsetNanoseconds(offsetString!) : 0;
        var epochNs = InterpretDateTimeOffset(engine, date, time, offsetBehaviour, offsetNs, timeZone, "compatible", "reject", matchMinutes);
        return new JsRelativeTo(null, epochNs, timeZone, calendar);
    }

    /// <summary>Whether an offset string has a seconds part: more than one MinuteSecond.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E934E7
    // Broiler-Human:        PENDING
    private static bool OffsetHasSeconds(string offset)
    {
        var digits = 0;

        foreach (var c in offset)
        {
            if (c is '.' or ',')
            {
                break;
            }

            if (char.IsAsciiDigit(c))
            {
                digits++;
            }
        }

        return digits > 4;
    }

    /// <summary>The proposal's InterpretTemporalDateTimeFields (s5.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BB8C47
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime InterpretDateTimeFields(JsEngine engine, string calendar, JsCalendarFields fields, bool reject)
    {
        var date = DateFromFields(engine, calendar, fields, reject);
        var time = RegulateTime(engine, fields.Hour ?? 0, fields.Minute ?? 0, fields.Second ?? 0, fields.Millisecond ?? 0, fields.Microsecond ?? 0, fields.Nanosecond ?? 0, reject);
        return new JsIsoDateTime(date, time);
    }

    /// <summary>The proposal's RegulateTime (s4.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D1EF7A
    // Broiler-Human:        PENDING
    internal static JsTimeRecord RegulateTime(JsEngine engine, double hour, double minute, double second, double millisecond, double microsecond, double nanosecond, bool reject)
    {
        if (reject)
        {
            if (!JsTemporalCore.IsValidTime(hour, minute, second, millisecond, microsecond, nanosecond))
            {
                throw engine.Error("RangeError", "Temporal: the time is not valid");
            }
        }
        else
        {
            hour = System.Math.Clamp(hour, 0, 23);
            minute = System.Math.Clamp(minute, 0, 59);
            second = System.Math.Clamp(second, 0, 59);
            millisecond = System.Math.Clamp(millisecond, 0, 999);
            microsecond = System.Math.Clamp(microsecond, 0, 999);
            nanosecond = System.Math.Clamp(nanosecond, 0, 999);
        }

        return new JsTimeRecord(0, (int)hour, (int)minute, (int)second, (int)millisecond, (int)microsecond, (int)nanosecond);
    }

    /// <summary>The proposal's InterpretISODateTimeOffset (s6.5.1); a null time is start-of-day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDC759
    // Broiler-Human:        PENDING
    internal static BigInteger InterpretDateTimeOffset(
        JsEngine engine,
        JsIsoDate date,
        JsTimeRecord? time,
        string offsetBehaviour,
        long offsetNanoseconds,
        string timeZone,
        string disambiguation,
        string offsetOption,
        bool matchMinutes)
    {
        if (time is not { } clock)
        {
            return StartOfDay(engine, timeZone, date);
        }

        var dateTime = new JsIsoDateTime(date, clock);

        if (offsetBehaviour == "wall" || (offsetBehaviour == "option" && offsetOption == "ignore"))
        {
            return EpochNsFor(engine, timeZone, dateTime, disambiguation);
        }

        if (offsetBehaviour == "exact" || (offsetBehaviour == "option" && offsetOption == "use"))
        {
            var balanced = JsTemporalCore.BalanceDateTime(date, JsTemporalCore.TimeNs(clock) - offsetNanoseconds);

            if (!JsTemporalCore.DaysWithinRange(balanced.Date))
            {
                throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
            }

            var epochNs = JsTemporalCore.UtcEpochNs(balanced);

            if (!JsTemporalCore.IsValidEpochNs(epochNs))
            {
                throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
            }

            return epochNs;
        }

        if (!JsTemporalCore.DaysWithinRange(date))
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        var utc = JsTemporalCore.UtcEpochNs(dateTime);
        var possible = PossibleEpochNs(engine, timeZone, dateTime);

        foreach (var candidate in possible)
        {
            var candidateOffset = utc - candidate;

            if (candidateOffset == offsetNanoseconds)
            {
                return candidate;
            }

            if (matchMinutes &&
                JsTemporalCore.RoundToIncrement(candidateOffset, new BigInteger(60_000_000_000L), TemporalRounding.HalfExpand) == offsetNanoseconds)
            {
                return candidate;
            }
        }

        if (offsetOption == "reject")
        {
            throw engine.Error("RangeError", "Temporal: the offset does not agree with the time zone");
        }

        return Disambiguate(engine, possible, timeZone, dateTime, disambiguation);
    }

    // ---- Adding and differencing relative to a date ---------------------------------------------

    /// <summary>The proposal's AddInstant (s8.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7823D4
    // Broiler-Human:        PENDING
    internal static BigInteger AddInstant(JsEngine engine, BigInteger epochNs, BigInteger duration)
    {
        var result = epochNs + duration;

        if (!JsTemporalCore.IsValidEpochNs(result))
        {
            throw engine.Error("RangeError", "Temporal: the result is outside the representable range of instants");
        }

        return result;
    }

    /// <summary>The proposal's AddZonedDateTime (s6.5.5).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4AF8FC
    // Broiler-Human:        PENDING
    internal static BigInteger AddZonedDateTime(JsEngine engine, BigInteger epochNs, string timeZone, string calendar, JsInternalDuration duration, bool reject)
    {
        if (JsTemporalCore.Sign(duration.Date) == 0)
        {
            return AddInstant(engine, epochNs, duration.Time);
        }

        var dateTime = IsoDateTimeFor(engine, timeZone, epochNs);
        var added = DateAdd(engine, dateTime.Date, duration.Date, reject);
        var intermediate = new JsIsoDateTime(added, dateTime.Time);

        if (!JsTemporalCore.DateTimeWithinLimits(intermediate))
        {
            throw engine.Error("RangeError", "Temporal: the result is outside the representable range");
        }

        var intermediateNs = EpochNsFor(engine, timeZone, intermediate, "compatible");
        return AddInstant(engine, intermediateNs, duration.Time);
    }

    /// <summary>The proposal's DifferenceISODateTime (s5.5.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8941B5
    // Broiler-Human:        PENDING
    internal static JsInternalDuration DifferenceIsoDateTime(JsEngine engine, JsIsoDateTime one, JsIsoDateTime two, TemporalUnit largestUnit)
    {
        var time = JsTemporalCore.DifferenceTime(one.Time, two.Time);
        var timeSign = time.Sign;
        var dateSign = JsTemporalCore.Compare(one.Date, two.Date);
        var adjusted = two.Date;

        if (timeSign == dateSign)
        {
            adjusted = JsTemporalCore.AddDays(adjusted, timeSign);
            time -= timeSign * JsTemporalCore.NsPerDay;
        }

        var dateLargest = JsTemporalCore.Larger(TemporalUnit.Day, largestUnit);
        var date = DateUntil(one.Date, adjusted, dateLargest);

        if (largestUnit != dateLargest)
        {
            time = Add24HourDays(engine, time, date.Days);
            date = date with { Days = 0 };
        }

        return new JsInternalDuration(date, time);
    }

    /// <summary>The proposal's DifferencePlainDateTimeWithRounding (s5.5.13).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=122324
    // Broiler-Human:        PENDING
    internal static JsInternalDuration DifferencePlainDateTimeWithRounding(
        JsEngine engine, JsIsoDateTime one, JsIsoDateTime two, string calendar, TemporalUnit largestUnit, long increment, TemporalUnit smallestUnit, TemporalRounding mode)
    {
        if (JsTemporalCore.Compare(one, two) == 0)
        {
            return default;
        }

        if (!JsTemporalCore.DateTimeWithinLimits(one) || !JsTemporalCore.DateTimeWithinLimits(two))
        {
            throw engine.Error("RangeError", "Temporal: a date-time is outside the representable range");
        }

        var difference = DifferenceIsoDateTime(engine, one, two, largestUnit);

        if (smallestUnit == TemporalUnit.Nanosecond && increment == 1)
        {
            return difference;
        }

        return RoundRelativeDuration(
            engine, difference, JsTemporalCore.UtcEpochNs(one), JsTemporalCore.UtcEpochNs(two), one, null, calendar, largestUnit, increment, smallestUnit, mode);
    }

    /// <summary>The proposal's DifferencePlainDateTimeWithTotal (s5.5.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2E43BB
    // Broiler-Human:        PENDING
    internal static JsFraction DifferencePlainDateTimeWithTotal(JsEngine engine, JsIsoDateTime one, JsIsoDateTime two, string calendar, TemporalUnit unit)
    {
        if (JsTemporalCore.Compare(one, two) == 0)
        {
            return new JsFraction(0, 1);
        }

        if (!JsTemporalCore.DateTimeWithinLimits(one) || !JsTemporalCore.DateTimeWithinLimits(two))
        {
            throw engine.Error("RangeError", "Temporal: a date-time is outside the representable range");
        }

        var difference = DifferenceIsoDateTime(engine, one, two, unit);

        if (unit == TemporalUnit.Nanosecond)
        {
            return new JsFraction(difference.Time, 1);
        }

        return TotalRelativeDuration(engine, difference, JsTemporalCore.UtcEpochNs(one), JsTemporalCore.UtcEpochNs(two), one, null, calendar, unit);
    }

    /// <summary>The proposal's DifferenceZonedDateTime (s6.5.6).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE5737
    // Broiler-Human:        PENDING
    internal static JsInternalDuration DifferenceZonedDateTime(JsEngine engine, BigInteger ns1, BigInteger ns2, string timeZone, string calendar, TemporalUnit largestUnit)
    {
        if (ns1 == ns2)
        {
            return default;
        }

        var start = IsoDateTimeFor(engine, timeZone, ns1);
        var end = IsoDateTimeFor(engine, timeZone, ns2);

        if (JsTemporalCore.Compare(start.Date, end.Date) == 0)
        {
            return new JsInternalDuration(default, ns2 - ns1);
        }

        var sign = ns2 - ns1 < 0 ? 1 : -1;
        var maxDayCorrection = sign == -1 ? 2 : 1;
        var dayCorrection = 0;
        var time = JsTemporalCore.DifferenceTime(start.Time, end.Time);

        if (time.Sign == sign)
        {
            dayCorrection++;
        }

        var success = false;
        JsIsoDateTime intermediate = default;

        while (dayCorrection <= maxDayCorrection && !success)
        {
            var intermediateDate = JsTemporalCore.AddDays(end.Date, dayCorrection * sign);
            intermediate = new JsIsoDateTime(intermediateDate, start.Time);
            var intermediateNs = EpochNsFor(engine, timeZone, intermediate, "compatible");
            time = ns2 - intermediateNs;

            if (time.Sign != sign)
            {
                success = true;
            }

            dayCorrection++;
        }

        var dateLargest = JsTemporalCore.Larger(largestUnit, TemporalUnit.Day);
        var date = DateUntil(start.Date, intermediate.Date, dateLargest);
        return new JsInternalDuration(date, time);
    }

    /// <summary>The proposal's DifferenceZonedDateTimeWithRounding (s6.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1353A3
    // Broiler-Human:        PENDING
    internal static JsInternalDuration DifferenceZonedDateTimeWithRounding(
        JsEngine engine, BigInteger ns1, BigInteger ns2, string timeZone, string calendar, TemporalUnit largestUnit, long increment, TemporalUnit smallestUnit, TemporalRounding mode)
    {
        if (!JsTemporalCore.IsDateUnit(largestUnit))
        {
            return DifferenceInstant(engine, ns1, ns2, increment, smallestUnit, mode);
        }

        var difference = DifferenceZonedDateTime(engine, ns1, ns2, timeZone, calendar, largestUnit);

        if (smallestUnit == TemporalUnit.Nanosecond && increment == 1)
        {
            return difference;
        }

        var dateTime = IsoDateTimeFor(engine, timeZone, ns1);
        return RoundRelativeDuration(engine, difference, ns1, ns2, dateTime, timeZone, calendar, largestUnit, increment, smallestUnit, mode);
    }

    /// <summary>The proposal's DifferenceZonedDateTimeWithTotal (s6.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=374914
    // Broiler-Human:        PENDING
    internal static JsFraction DifferenceZonedDateTimeWithTotal(JsEngine engine, BigInteger ns1, BigInteger ns2, string timeZone, string calendar, TemporalUnit unit)
    {
        if (!JsTemporalCore.IsDateUnit(unit))
        {
            return TotalTimeDuration(ns2 - ns1, unit);
        }

        var difference = DifferenceZonedDateTime(engine, ns1, ns2, timeZone, calendar, unit);
        var dateTime = IsoDateTimeFor(engine, timeZone, ns1);
        return TotalRelativeDuration(engine, difference, ns1, ns2, dateTime, timeZone, calendar, unit);
    }

    /// <summary>The proposal's DateDurationDays (s7.5.29).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B1153
    // Broiler-Human:        PENDING
    internal static double DateDurationDays(JsEngine engine, JsDateDuration date, JsIsoDate relativeTo)
    {
        var yearsMonthsWeeks = date with { Days = 0 };

        if (JsTemporalCore.Sign(yearsMonthsWeeks) == 0)
        {
            return date.Days;
        }

        var later = DateAdd(engine, relativeTo, yearsMonthsWeeks, reject: false);
        return date.Days + (JsTemporalCore.EpochDays(later) - JsTemporalCore.EpochDays(relativeTo));
    }

    // ---- Rounding relative to a date (s7.5.33 to s7.5.39) ---------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6133E9
    // Broiler-Human:        PENDING
    private readonly record struct NudgeWindow(
        long R1, long R2, BigInteger StartEpochNs, BigInteger EndEpochNs, JsInternalDuration StartDuration, JsInternalDuration EndDuration);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=76BA30
    // Broiler-Human:        PENDING
    private static BigInteger EpochFor(JsEngine engine, string? timeZone, JsIsoDateTime dateTime) =>
        timeZone is null ? JsTemporalCore.UtcEpochNs(dateTime) : EpochNsFor(engine, timeZone, dateTime, "compatible");

    /// <summary>The proposal's ComputeNudgeWindow (s7.5.33).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A38517
    // Broiler-Human:        PENDING
    private static NudgeWindow ComputeNudgeWindow(
        JsEngine engine, int sign, JsInternalDuration duration, BigInteger originEpochNs, JsIsoDateTime dateTime,
        string? timeZone, string calendar, long increment, TemporalUnit unit, bool additionalShift)
    {
        long r1, r2;
        JsDateDuration startDate, endDate;

        long Truncated(double value) => (long)JsTemporalCore.RoundToIncrement(JsTemporalCore.Exact(value), increment, TemporalRounding.Trunc);

        switch (unit)
        {
            case TemporalUnit.Year:
                var years = Truncated(duration.Date.Years);
                r1 = additionalShift ? years + (increment * sign) : years;
                r2 = r1 + (increment * sign);
                startDate = DateDurationRecord(engine, r1, 0, 0, 0);
                endDate = DateDurationRecord(engine, r2, 0, 0, 0);
                break;

            case TemporalUnit.Month:
                var months = Truncated(duration.Date.Months);
                r1 = additionalShift ? months + (increment * sign) : months;
                r2 = r1 + (increment * sign);
                startDate = AdjustDate(engine, duration.Date, 0, 0, r1);
                endDate = AdjustDate(engine, duration.Date, 0, 0, r2);
                break;

            case TemporalUnit.Week:
                var yearsMonths = duration.Date with { Weeks = 0, Days = 0 };
                var weeksStart = DateAdd(engine, dateTime.Date, yearsMonths, reject: false);
                var weeksEnd = JsTemporalCore.AddDays(weeksStart, (long)duration.Date.Days);
                var until = DateUntil(weeksStart, weeksEnd, TemporalUnit.Week);
                var weeks = (long)JsTemporalCore.RoundToIncrement(JsTemporalCore.Exact(duration.Date.Weeks + until.Weeks), increment, TemporalRounding.Trunc);
                r1 = weeks;
                r2 = weeks + (increment * sign);
                startDate = AdjustDate(engine, duration.Date, 0, r1);
                endDate = AdjustDate(engine, duration.Date, 0, r2);
                break;

            default:
                var days = Truncated(duration.Date.Days);
                r1 = days;
                r2 = days + (increment * sign);
                startDate = AdjustDate(engine, duration.Date, r1);
                endDate = AdjustDate(engine, duration.Date, r2);
                break;
        }

        BigInteger startEpochNs;

        // THE START IS THE ORIGIN WHEN THE START DURATION IS ZERO, as the reference polyfill tests it.
        // The draft's text tests r1 = 0, which a month window with whole years also meets, and would
        // then measure the window from the origin rather than from those years (JSD-0054).
        if (JsTemporalCore.Sign(startDate) == 0)
        {
            startEpochNs = originEpochNs;
        }
        else
        {
            var start = DateAdd(engine, dateTime.Date, startDate, reject: false);
            startEpochNs = EpochFor(engine, timeZone, new JsIsoDateTime(start, dateTime.Time));
        }

        var end = DateAdd(engine, dateTime.Date, endDate, reject: false);
        var endEpochNs = EpochFor(engine, timeZone, new JsIsoDateTime(end, dateTime.Time));

        return new NudgeWindow(r1, r2, startEpochNs, endEpochNs, new JsInternalDuration(startDate, 0), new JsInternalDuration(endDate, 0));
    }

    /// <summary>The proposal's NudgeToCalendarUnit (s7.5.34): the nudge and the exact total.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CFE1C9
    // Broiler-Human:        PENDING
    private static (JsNudge Nudge, JsFraction Total) NudgeToCalendarUnit(
        JsEngine engine, int sign, JsInternalDuration duration, BigInteger originEpochNs, BigInteger destEpochNs, JsIsoDateTime dateTime,
        string? timeZone, string calendar, long increment, TemporalUnit unit, TemporalRounding mode)
    {
        var didExpand = false;
        var window = ComputeNudgeWindow(engine, sign, duration, originEpochNs, dateTime, timeZone, calendar, increment, unit, additionalShift: false);

        bool Inside(NudgeWindow w) => sign == 1
            ? w.StartEpochNs <= destEpochNs && destEpochNs <= w.EndEpochNs
            : w.EndEpochNs <= destEpochNs && destEpochNs <= w.StartEpochNs;

        if (!Inside(window))
        {
            window = ComputeNudgeWindow(engine, sign, duration, originEpochNs, dateTime, timeZone, calendar, increment, unit, additionalShift: true);
            didExpand = true;
        }

        // TOTAL = r1 + progress × increment × sign, progress = (dest - start) / (end - start), exact.
        var span = window.EndEpochNs - window.StartEpochNs;
        var progressed = destEpochNs - window.StartEpochNs;
        var numerator = (window.R1 * span) + (progressed * increment * sign);
        var denominator = span;

        if (denominator.Sign < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        var total = new JsFraction(numerator, denominator);
        bool roundUp;

        if (progressed == span)
        {
            roundUp = true;
        }
        else
        {
            roundUp = JsTemporalCore.RoundsToSecond(
                BigInteger.Abs(numerator), denominator, System.Math.Abs(window.R1), System.Math.Abs(window.R2), mode, sign < 0);
        }

        JsNudge nudge;

        if (roundUp)
        {
            nudge = new JsNudge(window.EndDuration, window.EndEpochNs, true);
        }
        else
        {
            nudge = new JsNudge(window.StartDuration, window.StartEpochNs, didExpand);
        }

        return (nudge, total);
    }

    /// <summary>The proposal's NudgeToZonedTime (s7.5.35).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BD237F
    // Broiler-Human:        PENDING
    private static JsNudge NudgeToZonedTime(
        JsEngine engine, int sign, JsInternalDuration duration, JsIsoDateTime dateTime, string timeZone, string calendar,
        long increment, TemporalUnit unit, TemporalRounding mode)
    {
        var start = DateAdd(engine, dateTime.Date, duration.Date, reject: false);
        var startDateTime = new JsIsoDateTime(start, dateTime.Time);
        var endDate = JsTemporalCore.AddDays(start, sign);
        var endDateTime = new JsIsoDateTime(endDate, dateTime.Time);
        var startEpochNs = EpochNsFor(engine, timeZone, startDateTime, "compatible");
        var endEpochNs = EpochNsFor(engine, timeZone, endDateTime, "compatible");
        var daySpan = endEpochNs - startEpochNs;
        var unitLength = JsTemporalCore.UnitLength(unit);
        var rounded = RoundTimeDurationToIncrement(engine, duration.Time, increment * unitLength, mode);
        var beyondDaySpan = rounded - daySpan;
        bool didRoundBeyondDay;
        int dayDelta;
        BigInteger nudgedEpochNs;

        if (beyondDaySpan.Sign != -sign)
        {
            didRoundBeyondDay = true;
            dayDelta = sign;
            rounded = RoundTimeDurationToIncrement(engine, beyondDaySpan, increment * unitLength, mode);
            nudgedEpochNs = endEpochNs + rounded;
        }
        else
        {
            didRoundBeyondDay = false;
            dayDelta = 0;
            nudgedEpochNs = startEpochNs + rounded;
        }

        var date = AdjustDate(engine, duration.Date, duration.Date.Days + dayDelta);
        return new JsNudge(new JsInternalDuration(date, rounded), nudgedEpochNs, didRoundBeyondDay);
    }

    /// <summary>The proposal's NudgeToDayOrTime (s7.5.36).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5988A4
    // Broiler-Human:        PENDING
    private static JsNudge NudgeToDayOrTime(
        JsEngine engine, JsInternalDuration duration, BigInteger destEpochNs, TemporalUnit largestUnit, long increment, TemporalUnit smallestUnit, TemporalRounding mode)
    {
        var time = Add24HourDays(engine, duration.Time, duration.Date.Days);
        var unitLength = JsTemporalCore.UnitLength(smallestUnit);
        var rounded = RoundTimeDurationToIncrement(engine, time, unitLength * increment, mode);
        var diff = rounded - time;
        var wholeDays = BigInteger.Divide(time, JsTemporalCore.NsPerDay);
        var roundedWholeDays = BigInteger.Divide(rounded, JsTemporalCore.NsPerDay);
        var dayDelta = roundedWholeDays - wholeDays;
        var didExpandDays = dayDelta.Sign == time.Sign;
        var nudgedEpochNs = destEpochNs + diff;
        BigInteger days = 0;
        var remainder = rounded;

        if (JsTemporalCore.IsDateUnit(largestUnit))
        {
            days = roundedWholeDays;
            remainder = rounded - (roundedWholeDays * JsTemporalCore.NsPerDay);
        }

        var date = AdjustDate(engine, duration.Date, JsTemporalCore.ToDouble(days));
        return new JsNudge(new JsInternalDuration(date, remainder), nudgedEpochNs, didExpandDays);
    }

    /// <summary>The proposal's BubbleRelativeDuration (s7.5.37).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E9E229
    // Broiler-Human:        PENDING
    private static JsInternalDuration BubbleRelativeDuration(
        JsEngine engine, int sign, JsInternalDuration duration, BigInteger nudgedEpochNs, JsIsoDateTime dateTime,
        string? timeZone, string calendar, TemporalUnit largestUnit, TemporalUnit smallestUnit)
    {
        if (smallestUnit == largestUnit)
        {
            return duration;
        }

        for (var unit = smallestUnit - 1; unit >= largestUnit; unit--)
        {
            if (unit == TemporalUnit.Week && largestUnit != TemporalUnit.Week)
            {
                continue;
            }

            JsDateDuration endDuration;

            if (unit == TemporalUnit.Year)
            {
                endDuration = DateDurationRecord(engine, duration.Date.Years + sign, 0, 0, 0);
            }
            else if (unit == TemporalUnit.Month)
            {
                endDuration = AdjustDate(engine, duration.Date, 0, 0, duration.Date.Months + sign);
            }
            else
            {
                endDuration = AdjustDate(engine, duration.Date, 0, duration.Date.Weeks + sign);
            }

            var end = DateAdd(engine, dateTime.Date, endDuration, reject: false);
            var endEpochNs = EpochFor(engine, timeZone, new JsIsoDateTime(end, dateTime.Time));
            var beyondEnd = nudgedEpochNs - endEpochNs;

            if (beyondEnd.Sign != -sign)
            {
                duration = new JsInternalDuration(endDuration, 0);
            }
            else
            {
                break;
            }
        }

        return duration;
    }

    /// <summary>The proposal's RoundRelativeDuration (s7.5.38); a null time zone is unset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DDB951
    // Broiler-Human:        PENDING
    internal static JsInternalDuration RoundRelativeDuration(
        JsEngine engine, JsInternalDuration duration, BigInteger originEpochNs, BigInteger destEpochNs, JsIsoDateTime dateTime,
        string? timeZone, string calendar, TemporalUnit largestUnit, long increment, TemporalUnit smallestUnit, TemporalRounding mode)
    {
        var irregular = JsTemporalCore.IsCalendarUnit(smallestUnit) || (timeZone is not null && smallestUnit == TemporalUnit.Day);
        var sign = JsTemporalCore.Sign(duration) < 0 ? -1 : 1;
        JsNudge nudge;

        if (irregular)
        {
            nudge = NudgeToCalendarUnit(engine, sign, duration, originEpochNs, destEpochNs, dateTime, timeZone, calendar, increment, smallestUnit, mode).Nudge;
        }
        else if (timeZone is not null)
        {
            nudge = NudgeToZonedTime(engine, sign, duration, dateTime, timeZone, calendar, increment, smallestUnit, mode);
        }
        else
        {
            nudge = NudgeToDayOrTime(engine, duration, destEpochNs, largestUnit, increment, smallestUnit, mode);
        }

        duration = nudge.Duration;

        if (nudge.DidExpandCalendarUnit && smallestUnit != TemporalUnit.Week)
        {
            var startUnit = JsTemporalCore.Larger(smallestUnit, TemporalUnit.Day);
            duration = BubbleRelativeDuration(engine, sign, duration, nudge.NudgedEpochNs, dateTime, timeZone, calendar, largestUnit, startUnit);
        }

        return duration;
    }

    /// <summary>The proposal's TotalRelativeDuration (s7.5.39), exact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75D5E6
    // Broiler-Human:        PENDING
    internal static JsFraction TotalRelativeDuration(
        JsEngine engine, JsInternalDuration duration, BigInteger originEpochNs, BigInteger destEpochNs, JsIsoDateTime dateTime,
        string? timeZone, string calendar, TemporalUnit unit)
    {
        if (JsTemporalCore.IsCalendarUnit(unit) || (timeZone is not null && unit == TemporalUnit.Day))
        {
            var sign = JsTemporalCore.Sign(duration) < 0 ? -1 : 1;
            return NudgeToCalendarUnit(engine, sign, duration, originEpochNs, destEpochNs, dateTime, timeZone, calendar, 1, unit, TemporalRounding.Trunc).Total;
        }

        var time = Add24HourDays(engine, duration.Time, duration.Date.Days);
        return TotalTimeDuration(time, unit);
    }
}
