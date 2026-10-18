// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           0
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

// THE DURATIONS' ARITHMETIC IS THE SHARED LIBRARY'S since SJL-10.4 (JSD-0078): DurationMath holds a
// duration's validity, sign and balance, addition, compare, round and total, and the relative
// machinery (the nudge windows, the nudges and the bubbling), and TemporalDifferences every type's
// until and since, over the calendars and a zone handed in as providers. What stays here is the
// engine's: the Duration objects, ToTemporalDuration's reads, GetTemporalRelativeToOption's reads,
// classification and resolution with InterpretISODateTimeOffset, and each status mapped to its error.

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // ---- Creating and reading durations ---------------------------------------------------------

    /// <summary>The proposal's CreateTemporalDuration (s7.5.19): IsValidDuration the shared library's; every zero made positive.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=17B7CC
    // Broiler-Human:        PENDING
    internal static JsDurationObject CreateDuration(JsEngine engine, DurationFields fields, JsValue newTarget = default)
    {
        Check(engine, TemporalLimits.CheckDuration(fields), "the duration");

        var prototype = newTarget.IsObject
            ? engine.PrototypeFromConstructor(newTarget, engine.Realm.TemporalDurationPrototype!)
            : engine.Realm.TemporalDurationPrototype!;

        return new JsDurationObject(
            prototype,
            new DurationFields(
                fields.Years + 0.0, fields.Months + 0.0, fields.Weeks + 0.0, fields.Days + 0.0, fields.Hours + 0.0,
                fields.Minutes + 0.0, fields.Seconds + 0.0, fields.Milliseconds + 0.0, fields.Microseconds + 0.0, fields.Nanoseconds + 0.0));
    }

    /// <summary>A duration's ten fields in Table 21's order, as Intl.DurationFormat reads them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6CCCE0
    // Broiler-Human:        PENDING
    internal static double[] FieldsArray(DurationFields fields) =>
    [
        fields.Years, fields.Months, fields.Weeks, fields.Days, fields.Hours,
        fields.Minutes, fields.Seconds, fields.Milliseconds, fields.Microseconds, fields.Nanoseconds,
    ];

    /// <summary>The ten fields of an array in Table 21's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=59DAF9
    // Broiler-Human:        PENDING
    internal static DurationFields FieldsOf(double[] fields) =>
        new(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6], fields[7], fields[8], fields[9]);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9C811
    // Broiler-Human:        PENDING
    private static readonly string[] DurationProperties =
        ["days", "hours", "microseconds", "milliseconds", "minutes", "months", "nanoseconds", "seconds", "weeks", "years"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A6DDCC
    // Broiler-Human:        PENDING
    private static readonly int[] DurationPropertyIndex = [3, 4, 8, 7, 5, 1, 9, 6, 2, 0];

    /// <summary>The proposal's ToTemporalPartialDurationRecord (s7.5.18): a field per unit in Table 21's order, NaN where absent.</summary>
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

    /// <summary>The proposal's ToTemporalDuration (s7.5.12), as the ten fields: a string the shared grammar's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=74BA11
    // Broiler-Human:        PENDING
    internal static DurationFields ToDuration(JsEngine engine, JsValue item)
    {
        if (item.AsObjectOrNull() is JsDurationObject duration)
        {
            return duration.Fields;
        }

        if (!item.IsObject)
        {
            if (!item.IsString)
            {
                throw engine.Error("TypeError", "Temporal: a duration must be a string, an object or a Temporal.Duration");
            }

            return CreateDuration(engine, JsTemporalParser.Duration(engine, item.AsString())).Fields;
        }

        var partial = ToPartialDuration(engine, item);

        for (var i = 0; i < 10; i++)
        {
            if (double.IsNaN(partial[i]))
            {
                partial[i] = 0;
            }
        }

        return CreateDuration(engine, FieldsOf(partial)).Fields;
    }

    /// <summary>The proposal's TemporalDurationFromInternal (s7.5.8), the shared library's, then CreateTemporalDuration.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E9522E
    // Broiler-Human:        PENDING
    internal static JsDurationObject FromInternal(JsEngine engine, InternalDuration duration, TemporalUnit largestUnit)
    {
        Check(engine, DurationMath.FromInternal(duration, largestUnit, out var fields), "the duration");
        return CreateDuration(engine, fields);
    }

    // ---- relativeTo ---------------------------------------------------------------------------

    /// <summary>
    /// The proposal's GetTemporalRelativeToOption (s13.19): the reads, the classification and the
    /// resolution here, and the record the shared library's <see cref="RelativeTo"/>, with the
    /// calendars and the zone it computes through.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3FCB7
    // Broiler-Human:        PENDING
    internal static RelativeTo RelativeToOption(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "relativeTo");

        if (value.Type == JsType.Undefined)
        {
            return RelativeTo.None;
        }

        var offsetBehaviour = "option";
        var matchMinutes = false;
        IsoDate date;
        IsoTime? time;
        string? timeZone;
        string? offsetString;
        string calendar;

        if (value.IsObject)
        {
            switch (value.AsObject())
            {
                case JsZonedDateTimeObject zoned:
                    return RelativeTo.FromZoned(zoned.EpochNanoseconds, TimeZoneOf(engine, zoned.TimeZone), Calendars(engine), CalendarIdOf(zoned.Calendar));
                case JsPlainDateObject plainDate:
                    return RelativeTo.FromPlainDate(plainDate.Date, Calendars(engine), CalendarIdOf(plainDate.Calendar));
                case JsPlainDateTimeObject plainDateTime:
                    return RelativeTo.FromPlainDate(plainDateTime.DateTime.Date, Calendars(engine), CalendarIdOf(plainDateTime.Calendar));
            }

            calendar = CalendarWithIsoDefault(engine, value.AsObject());
            var fields = PrepareFields(
                engine,
                calendar,
                value.AsObject(),
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond, CalendarField.Offset, CalendarField.TimeZone],
                []);

            var result = InterpretDateTimeFields(engine, calendar, fields, TemporalOverflow.Constrain);
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

            matchMinutes = !(offsetString is not null && parsed.OffsetHasSeconds);

            calendar = CanonicalizeCalendar(engine, parsed.Calendar ?? "iso8601");
            date = DateOf(parsed);
            time = parsed.Time;
        }

        if (timeZone is null)
        {
            if (TemporalLimits.CheckIsoDate(date) != TemporalStatus.Completed)
            {
                throw engine.Error("RangeError", "Temporal: relativeTo is outside the representable range");
            }

            return RelativeTo.FromPlainDate(date, Calendars(engine), CalendarIdOf(calendar));
        }

        var offsetNs = offsetBehaviour == "option" ? OffsetNanoseconds(offsetString!) : 0;
        var epochNs = InterpretDateTimeOffset(engine, date, time, offsetBehaviour, offsetNs, timeZone, "compatible", "reject", matchMinutes);
        return RelativeTo.FromZoned(epochNs, TimeZoneOf(engine, timeZone), Calendars(engine), CalendarIdOf(calendar));
    }

    /// <summary>
    /// The proposal's InterpretISODateTimeOffset (s6.5.1), the adapter's: the offset behaviour and
    /// option decide which instant a wall time is, the zone's candidates and the arithmetic the shared
    /// libraries'. A null time is start-of-day.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=614670
    // Broiler-Human:        PENDING
    internal static BigInteger InterpretDateTimeOffset(
        JsEngine engine,
        IsoDate date,
        IsoTime? time,
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

        var dateTime = new IsoDateTime(date, clock);

        if (offsetBehaviour == "wall" || (offsetBehaviour == "option" && offsetOption == "ignore"))
        {
            return EpochNsFor(engine, timeZone, dateTime, disambiguation);
        }

        if (offsetBehaviour == "exact" || (offsetBehaviour == "option" && offsetOption == "use"))
        {
            var balanced = IsoArithmetic.BalanceIsoDateTime(date, IsoArithmetic.TimeToNanoseconds(clock) - offsetNanoseconds);

            if (TemporalLimits.CheckIsoDaysRange(balanced.Date) != TemporalStatus.Completed)
            {
                throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
            }

            var epochNs = IsoArithmetic.GetUtcEpochNanoseconds(balanced);

            if (TemporalLimits.CheckEpochNanoseconds(epochNs) != TemporalStatus.Completed)
            {
                throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
            }

            return epochNs;
        }

        if (TemporalLimits.CheckIsoDaysRange(date) != TemporalStatus.Completed)
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }

        var utc = IsoArithmetic.GetUtcEpochNanoseconds(dateTime);
        var possible = PossibleEpochNs(engine, timeZone, dateTime);

        foreach (var candidate in possible)
        {
            var candidateOffset = utc - candidate;

            if (candidateOffset == offsetNanoseconds)
            {
                return candidate;
            }

            if (matchMinutes &&
                ExactRounding.RoundNumberToIncrement(candidateOffset, BigInteger.One, new BigInteger(60_000_000_000L), RoundingMode.HalfExpand) == offsetNanoseconds)
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

    // ---- Adding --------------------------------------------------------------------------------

    /// <summary>The proposal's AddInstant (s8.5.5), the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=771CA5
    // Broiler-Human:        PENDING
    internal static BigInteger AddInstant(JsEngine engine, BigInteger epochNs, BigInteger duration)
    {
        CheckInstantResult(engine, IsoArithmetic.AddInstant(epochNs, duration, out var result));
        return result;
    }

    /// <summary>
    /// The proposal's AddZonedDateTime (s6.5.5), the shared library's over this realm's calendars and
    /// the zone: an addition under <c>reject</c> to a day or a leap month the target lacks is a
    /// RangeError, as every other refusal.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1D1641
    // Broiler-Human:        PENDING
    internal static BigInteger AddZonedDateTime(JsEngine engine, BigInteger epochNs, string timeZone, string calendar, InternalDuration duration, TemporalOverflow overflow)
    {
        CheckInstantResult(
            engine,
            DurationMath.AddZonedDateTime(epochNs, TimeZoneOf(engine, timeZone), Calendars(engine), CalendarIdOf(calendar), duration, overflow, out var result));
        return result;
    }

    /// <summary>An addition's status where its result is an instant: one outside the limits with this family's message for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D0F3CF
    // Broiler-Human:        PENDING
    private static void CheckInstantResult(JsEngine engine, TemporalStatus status)
    {
        if (status == TemporalStatus.OutOfRange)
        {
            throw engine.Error("RangeError", "Temporal: the result is outside the representable range of instants");
        }

        Check(engine, status, "the result");
    }
}
