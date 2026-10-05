// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.ZonedDateTime</c> (Temporal s6; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8E8851
    // Broiler-Human:        PENDING
    private void SetupTemporalZonedDateTime(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalZonedDateTimePrototype = prototype;

        var constructor = TemporalConstructor(temporal, "ZonedDateTime", 2, prototype, static (engine, newTarget, arguments) =>
        {
            var epochNs = JsTemporal.ValidEpochNs(engine, engine.ToBigInt(TemporalArgument(arguments, 0)).Value);
            var timeZoneValue = TemporalArgument(arguments, 1);

            if (!timeZoneValue.IsString)
            {
                throw engine.Error("TypeError", "Temporal.ZonedDateTime: the time zone must be a string");
            }

            var timeZone = JsTemporal.TimeZoneFromIdentifier(engine, timeZoneValue.AsString());
            var calendar = TemporalCalendarArgument(engine, TemporalArgument(arguments, 2));
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, epochNs, timeZone, calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToZonedDateTime(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToZonedDateTime(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToZonedDateTime(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(one.EpochNanoseconds.CompareTo(two.EpochNanoseconds) switch { > 0 => 1, < 0 => -1, _ => 0 });
        });

        static (JsIsoDate, string) DateOf(JsEngine engine, JsValue thisValue, string member)
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", member);
            return (JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Date, zoned.Calendar);
        }

        TemporalCalendarGetters(prototype, ["calendarId"], DateOf);

        TemporalGetter(prototype, "timeZoneId", static (engine, thisValue, arguments) =>
            JsValue.String(TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "timeZoneId").TimeZone));

        TemporalCalendarGetters(prototype, TemporalDateGetterNames[1..7], DateOf);

        TemporalTimeGetters(prototype, static (engine, thisValue, member) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", member);
            return JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Time;
        });

        TemporalGetter(prototype, "epochMilliseconds", static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "epochMilliseconds");
            return JsValue.Number((double)FloorDivide(zoned.EpochNanoseconds, 1_000_000));
        });

        TemporalGetter(prototype, "epochNanoseconds", static (engine, thisValue, arguments) =>
            JsValue.BigInt(new JsBigInt(TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "epochNanoseconds").EpochNanoseconds)));

        TemporalCalendarGetters(prototype, ["dayOfWeek", "dayOfYear", "weekOfYear", "yearOfWeek"], DateOf);

        TemporalGetter(prototype, "hoursInDay", static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "hoursInDay");
            var today = JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Date;
            var tomorrow = JsTemporalCore.AddDays(today, 1);
            var todayNs = JsTemporal.StartOfDay(engine, zoned.TimeZone, today);
            var tomorrowNs = JsTemporal.StartOfDay(engine, zoned.TimeZone, tomorrow);
            return JsValue.Number(JsTemporal.TotalTimeDuration(tomorrowNs - todayNs, TemporalUnit.Hour).ToDouble());
        });

        TemporalCalendarGetters(prototype, ["daysInWeek", "daysInMonth", "daysInYear", "monthsInYear", "inLeapYear"], DateOf);

        TemporalGetter(prototype, "offsetNanoseconds", static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "offsetNanoseconds");
            return JsValue.Number(JsTemporal.OffsetNanosecondsFor(engine, zoned.TimeZone, zoned.EpochNanoseconds));
        });

        TemporalGetter(prototype, "offset", static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "offset");
            return JsValue.String(JsTemporalCore.FormatOffsetNs(JsTemporal.OffsetNanosecondsFor(engine, zoned.TimeZone, zoned.EpochNanoseconds)));
        });

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "with");
            var partial = PartialTemporalObject(engine, TemporalArgument(arguments, 0), "ZonedDateTime");
            var timeZone = zoned.TimeZone;
            var calendar = zoned.Calendar;
            var offsetNs = JsTemporal.OffsetNanosecondsFor(engine, timeZone, zoned.EpochNanoseconds);
            var dateTime = JsTemporal.IsoDateTimeFor(engine, timeZone, zoned.EpochNanoseconds);
            var fields = JsTemporal.DateToFields(calendar, dateTime.Date, JsTemporal.FieldsType.Date);
            SetTimeFields(fields, dateTime.Time);
            fields.Offset = JsTemporalCore.FormatOffsetNs(offsetNs);
            var partialFields = JsTemporal.PrepareFields(
                engine,
                calendar,
                partial,
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond, CalendarField.Offset],
                null,
                partial: true);
            fields = JsTemporal.MergeFields(calendar, fields, partialFields);
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
            var disambiguation = JsTemporal.Disambiguation(engine, options);
            var offset = JsTemporal.OffsetOption(engine, options, "prefer");
            var reject = JsTemporal.Reject(engine, options);
            var result = JsTemporal.InterpretDateTimeFields(engine, calendar, fields, reject);
            var newOffsetNs = JsTemporalParser.OffsetNanoseconds(fields.Offset!);
            var epochNs = JsTemporal.InterpretDateTimeOffset(engine, result.Date, result.Time, "option", newOffsetNs, timeZone, disambiguation, offset, matchMinutes: false);
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, epochNs, timeZone, calendar));
        });

        Method(prototype, "withPlainTime", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "withPlainTime");
            var dateTime = JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds);
            var plainTimeLike = TemporalArgument(arguments, 0);
            BigInteger epochNs;

            if (plainTimeLike.Type == JsType.Undefined)
            {
                epochNs = JsTemporal.StartOfDay(engine, zoned.TimeZone, dateTime.Date);
            }
            else
            {
                var time = JsTemporal.ToPlainTime(engine, plainTimeLike);
                epochNs = JsTemporal.EpochNsFor(engine, zoned.TimeZone, new JsIsoDateTime(dateTime.Date, time), "compatible");
            }

            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, epochNs, zoned.TimeZone, zoned.Calendar));
        });

        Method(prototype, "withTimeZone", 1, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "withTimeZone");
            var timeZone = JsTemporal.ToTimeZoneIdentifier(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, zoned.EpochNanoseconds, timeZone, zoned.Calendar));
        });

        Method(prototype, "withCalendar", 1, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "withCalendar");
            var calendar = JsTemporal.ToCalendarIdentifier(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, zoned.EpochNanoseconds, zoned.TimeZone, calendar));
        });

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToZonedDateTime(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "add"), arguments, subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToZonedDateTime(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "subtract"), arguments, subtract: true)));

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalZonedDateTime(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalZonedDateTime(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "since"), arguments, since: true)));

        Method(prototype, "round", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(ZonedDateTimeRound(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "round"), TemporalArgument(arguments, 0))));

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "equals");
            var other = JsTemporal.ToZonedDateTime(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(
                zoned.EpochNanoseconds == other.EpochNanoseconds &&
                JsTemporal.TimeZoneEquals(engine, zoned.TimeZone, other.TimeZone) &&
                zoned.Calendar == other.Calendar);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var showCalendar = JsTemporal.ShowCalendar(engine, options);
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var showOffset = JsTemporal.ShowOffset(engine, options);
            var mode = JsTemporal.RoundingMode(engine, options, TemporalRounding.Trunc);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);
            var showTimeZone = JsTemporal.ShowTimeZone(engine, options);
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);

            if (smallest == TemporalUnit.Hour)
            {
                throw engine.Error("RangeError", "Temporal.ZonedDateTime.prototype.toString: smallestUnit cannot be hour");
            }

            var precision = JsTemporal.Precision(smallest, digits);
            return JsValue.String(JsTemporal.ZonedDateTimeString(
                engine, zoned, precision.Precision, showCalendar, showTimeZone, showOffset, precision.Increment, precision.Unit, mode));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toLocaleString");
            return JsValue.String(JsTemporal.ZonedDateTimeString(engine, zoned, JsTemporalCore.PrecisionAuto, "auto", "auto", "auto"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toJSON");
            return JsValue.String(JsTemporal.ZonedDateTimeString(engine, zoned, JsTemporalCore.PrecisionAuto, "auto", "auto", "auto"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "ZonedDateTime"));

        Method(prototype, "startOfDay", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "startOfDay");
            var date = JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Date;
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, JsTemporal.StartOfDay(engine, zoned.TimeZone, date), zoned.TimeZone, zoned.Calendar));
        });

        Method(prototype, "getTimeZoneTransition", 1, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "getTimeZoneTransition");
            var directionParam = TemporalArgument(arguments, 0);

            if (directionParam.Type == JsType.Undefined)
            {
                throw engine.Error("TypeError", "Temporal.ZonedDateTime.prototype.getTimeZoneTransition: a direction is required");
            }

            JsObject options;

            if (directionParam.IsString)
            {
                options = new JsObject(null);
                options.DefineOrdinary("direction", directionParam);
            }
            else
            {
                options = JsTemporal.OptionsObject(engine, directionParam);
            }

            var direction = JsTemporal.StringOption(engine, options, "direction", ["next", "previous"], null, required: true);

            if (JsTemporal.ParseZone(zoned.TimeZone).Name is null)
            {
                return JsValue.Null;
            }

            var transition = direction == "next"
                ? JsTemporal.NextTransition(engine, zoned.TimeZone, zoned.EpochNanoseconds)
                : JsTemporal.PreviousTransition(engine, zoned.TimeZone, zoned.EpochNanoseconds);

            return transition is { } ns
                ? JsValue.Object(JsTemporal.CreateZonedDateTime(engine, ns, zoned.TimeZone, zoned.Calendar))
                : JsValue.Null;
        });

        Method(prototype, "toInstant", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreateInstant(engine, TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toInstant").EpochNanoseconds)));

        Method(prototype, "toPlainDate", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toPlainDate");
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Date, zoned.Calendar));
        });

        Method(prototype, "toPlainTime", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toPlainTime");
            return JsValue.Object(JsTemporal.CreatePlainTime(engine, JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds).Time));
        });

        Method(prototype, "toPlainDateTime", 0, static (engine, thisValue, arguments) =>
        {
            var zoned = TemporalThis<JsZonedDateTimeObject>(engine, thisValue, "ZonedDateTime", "toPlainDateTime");
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, JsTemporal.IsoDateTimeFor(engine, zoned.TimeZone, zoned.EpochNanoseconds), zoned.Calendar));
        });
    }

    /// <summary>Temporal.ZonedDateTime.prototype.round (s6.3.39).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=63439A
    // Broiler-Human:        PENDING
    private static JsZonedDateTimeObject ZonedDateTimeRound(JsEngine engine, JsZonedDateTimeObject zoned, JsValue argument)
    {
        var roundTo = TemporalRoundTo(engine, argument, "ZonedDateTime");
        var increment = JsTemporal.RoundingIncrement(engine, roundTo);
        var mode = JsTemporal.RoundingMode(engine, roundTo, TemporalRounding.HalfExpand);
        var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: true);

        if (smallest == TemporalUnit.Day)
        {
            JsTemporal.ValidateIncrement(engine, increment, 1, inclusive: true);
        }
        else
        {
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);
            JsTemporal.ValidateIncrement(engine, increment, JsTemporalCore.MaximumIncrement(smallest), inclusive: false);
        }

        if (smallest == TemporalUnit.Nanosecond && increment == 1)
        {
            return JsTemporal.CreateZonedDateTime(engine, zoned.EpochNanoseconds, zoned.TimeZone, zoned.Calendar);
        }

        var thisNs = zoned.EpochNanoseconds;
        var timeZone = zoned.TimeZone;
        var dateTime = JsTemporal.IsoDateTimeFor(engine, timeZone, thisNs);
        BigInteger epochNs;

        if (smallest == TemporalUnit.Day)
        {
            var start = JsTemporal.StartOfDay(engine, timeZone, dateTime.Date);
            var end = JsTemporal.StartOfDay(engine, timeZone, JsTemporalCore.AddDays(dateTime.Date, 1));
            var rounded = JsTemporal.RoundTimeDurationToIncrement(engine, thisNs - start, end - start, mode);
            epochNs = start + rounded;
        }
        else
        {
            var roundResult = JsTemporalCore.RoundDateTime(dateTime, increment, smallest, mode);
            var offsetNs = JsTemporal.OffsetNanosecondsFor(engine, timeZone, thisNs);
            epochNs = JsTemporal.InterpretDateTimeOffset(engine, roundResult.Date, roundResult.Time, "option", offsetNs, timeZone, "compatible", "prefer", matchMinutes: false);
        }

        return JsTemporal.CreateZonedDateTime(engine, epochNs, timeZone, zoned.Calendar);
    }

    /// <summary>The proposal's AddDurationToZonedDateTime (s6.5.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B9A60F
    // Broiler-Human:        PENDING
    private static JsZonedDateTimeObject AddDurationToZonedDateTime(JsEngine engine, JsZonedDateTimeObject zoned, JsValue[] arguments, bool subtract)
    {
        var fields = SignedDuration(engine, TemporalArgument(arguments, 0), subtract);
        var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
        var internalDuration = JsTemporalCore.ToInternal(fields);
        var epochNs = JsTemporal.AddZonedDateTime(engine, zoned.EpochNanoseconds, zoned.TimeZone, zoned.Calendar, internalDuration, reject);
        return JsTemporal.CreateZonedDateTime(engine, epochNs, zoned.TimeZone, zoned.Calendar);
    }

    /// <summary>The proposal's DifferenceTemporalZonedDateTime (s6.5.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CB1315
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalZonedDateTime(JsEngine engine, JsZonedDateTimeObject zoned, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToZonedDateTime(engine, TemporalArgument(arguments, 0));
        RequireSameCalendar(engine, zoned.Calendar, other.Calendar);
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var settings = JsTemporal.DifferenceSettings(engine, since, options, JsTemporal.UnitGroup.DateTime, [], TemporalUnit.Nanosecond, TemporalUnit.Hour);
        double[] result;

        if (!JsTemporalCore.IsDateUnit(settings.LargestUnit))
        {
            var instantDifference = JsTemporal.DifferenceInstant(
                engine, zoned.EpochNanoseconds, other.EpochNanoseconds, settings.RoundingIncrement, settings.SmallestUnit, settings.RoundingMode);
            result = JsTemporalCore.FromInternal(instantDifference, settings.LargestUnit);
            return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
        }

        if (!JsTemporal.TimeZoneEquals(engine, zoned.TimeZone, other.TimeZone))
        {
            throw engine.Error("RangeError", "Temporal.ZonedDateTime: a difference in days or larger units needs both values in the same time zone");
        }

        if (zoned.EpochNanoseconds == other.EpochNanoseconds)
        {
            return JsTemporal.CreateDuration(engine, new double[10]);
        }

        var internalDuration = JsTemporal.DifferenceZonedDateTimeWithRounding(
            engine,
            zoned.EpochNanoseconds,
            other.EpochNanoseconds,
            zoned.TimeZone,
            zoned.Calendar,
            settings.LargestUnit,
            settings.RoundingIncrement,
            settings.SmallestUnit,
            settings.RoundingMode);
        result = JsTemporalCore.FromInternal(internalDuration, TemporalUnit.Hour);
        return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
    }
}
