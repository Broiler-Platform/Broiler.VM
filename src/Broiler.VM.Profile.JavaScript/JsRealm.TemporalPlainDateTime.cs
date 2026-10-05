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

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.PlainDateTime</c> (Temporal s5; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=557619
    // Broiler-Human:        PENDING
    private void SetupTemporalPlainDateTime(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalPlainDateTimePrototype = prototype;

        var constructor = TemporalConstructor(temporal, "PlainDateTime", 3, prototype, static (engine, newTarget, arguments) =>
        {
            var values = new double[9];

            for (var i = 0; i < 9; i++)
            {
                var argument = TemporalArgument(arguments, i);
                values[i] = i >= 3 && argument.Type == JsType.Undefined ? 0 : JsTemporal.IntegerWithTruncation(engine, argument);
            }

            var calendar = TemporalCalendarArgument(engine, TemporalArgument(arguments, 9));

            if (!JsTemporalCore.IsValidIsoDate(values[0], values[1], values[2]))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime: the date is not valid");
            }

            if (!JsTemporalCore.IsValidTime(values[3], values[4], values[5], values[6], values[7], values[8]))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime: the time is not valid");
            }

            var dateTime = new JsIsoDateTime(
                new JsIsoDate((long)values[0], (int)values[1], (int)values[2]),
                new JsTimeRecord(0, (int)values[3], (int)values[4], (int)values[5], (int)values[6], (int)values[7], (int)values[8]));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, dateTime, calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(JsTemporalCore.Compare(one.DateTime, two.DateTime));
        });

        TemporalCalendarGetters(prototype, TemporalDateGetterNames[..7], static (engine, thisValue, member) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", member);
            return (dateTime.DateTime.Date, dateTime.Calendar);
        });

        TemporalTimeGetters(prototype, static (engine, thisValue, member) =>
            TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", member).DateTime.Time);

        TemporalCalendarGetters(prototype, TemporalDateGetterNames[7..], static (engine, thisValue, member) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", member);
            return (dateTime.DateTime.Date, dateTime.Calendar);
        });

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "with");
            var partial = PartialTemporalObject(engine, TemporalArgument(arguments, 0), "PlainDateTime");
            var calendar = dateTime.Calendar;
            var fields = JsTemporal.DateToFields(calendar, dateTime.DateTime.Date, JsTemporal.FieldsType.Date);
            SetTimeFields(fields, dateTime.DateTime.Time);
            var partialDateTime = JsTemporal.PrepareFields(
                engine,
                calendar,
                partial,
                [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day],
                [CalendarField.Hour, CalendarField.Minute, CalendarField.Second, CalendarField.Millisecond, CalendarField.Microsecond, CalendarField.Nanosecond],
                null,
                partial: true);
            fields = JsTemporal.MergeFields(calendar, fields, partialDateTime);
            var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, JsTemporal.InterpretDateTimeFields(engine, calendar, fields, reject), calendar));
        });

        Method(prototype, "withPlainTime", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "withPlainTime");
            var time = JsTemporal.ToTimeOrMidnight(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, new JsIsoDateTime(dateTime.DateTime.Date, time), dateTime.Calendar));
        });

        Method(prototype, "withCalendar", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "withCalendar");
            var calendar = JsTemporal.ToCalendarIdentifier(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, dateTime.DateTime, calendar));
        });

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToDateTime(engine, TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "add"), arguments, subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToDateTime(engine, TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "subtract"), arguments, subtract: true)));

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainDateTime(engine, TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainDateTime(engine, TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "since"), arguments, since: true)));

        Method(prototype, "round", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "round");
            var roundTo = TemporalRoundTo(engine, TemporalArgument(arguments, 0), "PlainDateTime");
            var increment = JsTemporal.RoundingIncrement(engine, roundTo);
            var mode = JsTemporal.RoundingMode(engine, roundTo, TemporalRounding.HalfExpand);
            var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: true);

            if (smallest != TemporalUnit.Day)
            {
                JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);
            }

            if (smallest == TemporalUnit.Day)
            {
                JsTemporal.ValidateIncrement(engine, increment, 1, inclusive: true);
            }
            else
            {
                JsTemporal.ValidateIncrement(engine, increment, JsTemporalCore.MaximumIncrement(smallest), inclusive: false);
            }

            if (smallest == TemporalUnit.Nanosecond && increment == 1)
            {
                return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, dateTime.DateTime, dateTime.Calendar));
            }

            var result = JsTemporalCore.RoundDateTime(dateTime.DateTime, increment, smallest, mode);
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, result, dateTime.Calendar));
        });

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "equals");
            var other = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(JsTemporalCore.Compare(dateTime.DateTime, other.DateTime) == 0 && dateTime.Calendar == other.Calendar);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var showCalendar = JsTemporal.ShowCalendar(engine, options);
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var mode = JsTemporal.RoundingMode(engine, options, TemporalRounding.Trunc);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);

            if (smallest == TemporalUnit.Hour)
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime.prototype.toString: smallestUnit cannot be hour");
            }

            var precision = JsTemporal.Precision(smallest, digits);
            var result = JsTemporalCore.RoundDateTime(dateTime.DateTime, precision.Increment, precision.Unit, mode);

            if (!JsTemporalCore.DateTimeWithinLimits(result))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime.prototype.toString: the rounded date-time is outside the representable range");
            }

            return JsValue.String(JsTemporal.DateTimeString(result, dateTime.Calendar, precision.Precision, showCalendar));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toLocaleString");
            return JsValue.String(JsTemporal.DateTimeString(dateTime.DateTime, dateTime.Calendar, JsTemporalCore.PrecisionAuto, "auto"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toJSON");
            return JsValue.String(JsTemporal.DateTimeString(dateTime.DateTime, dateTime.Calendar, JsTemporalCore.PrecisionAuto, "auto"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "PlainDateTime"));

        Method(prototype, "toZonedDateTime", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toZonedDateTime");
            var timeZone = JsTemporal.ToTimeZoneIdentifier(engine, TemporalArgument(arguments, 0));
            var disambiguation = JsTemporal.Disambiguation(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            var epochNs = JsTemporal.EpochNsFor(engine, timeZone, dateTime.DateTime, disambiguation);
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, epochNs, timeZone, dateTime.Calendar));
        });

        Method(prototype, "toPlainDate", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toPlainDate");
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, dateTime.DateTime.Date, dateTime.Calendar));
        });

        Method(prototype, "toPlainTime", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toPlainTime");
            return JsValue.Object(JsTemporal.CreatePlainTime(engine, dateTime.DateTime.Time));
        });
    }

    /// <summary>Sets a fields record's six time fields from a time record.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7C7880
    // Broiler-Human:        PENDING
    private static void SetTimeFields(JsCalendarFields fields, JsTimeRecord time)
    {
        fields.Hour = time.Hour;
        fields.Minute = time.Minute;
        fields.Second = time.Second;
        fields.Millisecond = time.Millisecond;
        fields.Microsecond = time.Microsecond;
        fields.Nanosecond = time.Nanosecond;
    }

    /// <summary>The proposal's AddDurationToDateTime (s5.5.16).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=61A643
    // Broiler-Human:        PENDING
    private static JsPlainDateTimeObject AddDurationToDateTime(JsEngine engine, JsPlainDateTimeObject dateTime, JsValue[] arguments, bool subtract)
    {
        var fields = SignedDuration(engine, TemporalArgument(arguments, 0), subtract);
        var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
        var internalDuration = JsTemporalCore.ToInternalWith24HourDays(fields);
        var timeResult = JsTemporalCore.AddTime(dateTime.DateTime.Time, internalDuration.Time);
        var dateDuration = JsTemporal.AdjustDate(engine, internalDuration.Date, timeResult.Days);
        var added = JsTemporal.DateAdd(engine, dateTime.DateTime.Date, dateDuration, reject);
        return JsTemporal.CreatePlainDateTime(engine, new JsIsoDateTime(added, timeResult), dateTime.Calendar);
    }

    /// <summary>The proposal's DifferenceTemporalPlainDateTime (s5.5.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B33FDE
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalPlainDateTime(JsEngine engine, JsPlainDateTimeObject dateTime, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
        RequireSameCalendar(engine, dateTime.Calendar, other.Calendar);
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var settings = JsTemporal.DifferenceSettings(engine, since, options, JsTemporal.UnitGroup.DateTime, [], TemporalUnit.Nanosecond, TemporalUnit.Day);

        if (JsTemporalCore.Compare(dateTime.DateTime, other.DateTime) == 0)
        {
            return JsTemporal.CreateDuration(engine, new double[10]);
        }

        var internalDuration = JsTemporal.DifferencePlainDateTimeWithRounding(
            engine, dateTime.DateTime, other.DateTime, dateTime.Calendar, settings.LargestUnit, settings.RoundingIncrement, settings.SmallestUnit, settings.RoundingMode);
        var result = JsTemporalCore.FromInternal(internalDuration, settings.LargestUnit);
        return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
    }
}
