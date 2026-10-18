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

using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.PlainDateTime</c> (Temporal s5; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D54C46
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

            if (!IsoCalendarMath.IsValidIsoDate(JsTemporal.Year(values[0]), (int)values[1], (int)values[2]))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime: the date is not valid");
            }

            var time = new IsoTime((int)values[3], (int)values[4], (int)values[5], (int)values[6], (int)values[7], (int)values[8]);

            if (!TemporalLimits.IsValidTime(time))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime: the time is not valid");
            }

            // ROW R6 (decision D9): the limits are checked through the shared library, which takes any
            // year, so a year past 2^53 is the RangeError the specification asks for.
            var dateTime = new IsoDateTime(new IsoDate(JsTemporal.Year(values[0]), (int)values[1], (int)values[2]), time);
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, dateTime, calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(IsoArithmetic.CompareIsoDateTime(one.DateTime, two.DateTime));
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
            var fields = JsTemporal.DateToFields(engine, calendar, dateTime.DateTime.Date, CalendarFieldsType.Date);
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
            var overflow = JsTemporal.Overflow(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, JsTemporal.InterpretDateTimeFields(engine, calendar, fields, overflow), calendar));
        });

        Method(prototype, "withPlainTime", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "withPlainTime");
            var time = JsTemporal.ToTimeOrMidnight(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, new IsoDateTime(dateTime.DateTime.Date, time), dateTime.Calendar));
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
            var mode = JsTemporal.RoundingModeOption(engine, roundTo, RoundingMode.HalfExpand);
            var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: true);

            if (!TemporalOptionRules.IsValidRoundOptions(TemporalType.PlainDateTime, smallest, increment))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime.prototype.round: smallestUnit and roundingIncrement are not valid together");
            }

            if (smallest == TemporalUnit.Nanosecond && increment == 1)
            {
                return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, dateTime.DateTime, dateTime.Calendar));
            }

            var result = IsoArithmetic.RoundIsoDateTime(dateTime.DateTime, increment, smallest, mode);
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, result, dateTime.Calendar));
        });

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "equals");
            var other = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(IsoArithmetic.CompareIsoDateTime(dateTime.DateTime, other.DateTime) == 0 && dateTime.Calendar == other.Calendar);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var showCalendar = JsTemporal.ShowCalendarOption(engine, options);
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var mode = JsTemporal.RoundingModeOption(engine, options, TemporalOptionRules.DefaultToStringRoundingMode);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);

            if (!TemporalOptionRules.IsValidToStringSmallestUnit(smallest, duration: false))
            {
                throw engine.Error("RangeError", "Temporal.PlainDateTime.prototype.toString: smallestUnit must be a minute or smaller");
            }

            var status = TemporalFormat.TryPlainDateTimeToString(
                dateTime.DateTime, dateTime.Calendar, TemporalOptionRules.ToSecondsStringPrecisionRecord(smallest, digits), mode, showCalendar, out var text);
            JsTemporal.Check(engine, status, "the rounded date-time");
            return JsValue.String(text!);
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toLocaleString");
            return JsValue.String(TemporalToLocaleString(engine, thisValue, arguments, "any", "all"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var dateTime = TemporalThis<JsPlainDateTimeObject>(engine, thisValue, "PlainDateTime", "toJSON");
            return JsValue.String(TemporalFormat.DateTimeToString(dateTime.DateTime, dateTime.Calendar, TemporalFormat.AutoPrecision, ShowCalendarName.Auto));
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BBCC2A
    // Broiler-Human:        PENDING
    private static void SetTimeFields(JsCalendarFields fields, IsoTime time)
    {
        fields.Hour = time.Hour;
        fields.Minute = time.Minute;
        fields.Second = time.Second;
        fields.Millisecond = time.Millisecond;
        fields.Microsecond = time.Microsecond;
        fields.Nanosecond = time.Nanosecond;
    }

    /// <summary>
    /// The proposal's AddDurationToDateTime (s5.5.16): AddTime with its carried days, then
    /// CalendarDateAdd, each the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=927971
    // Broiler-Human:        PENDING
    private static JsPlainDateTimeObject AddDurationToDateTime(JsEngine engine, JsPlainDateTimeObject dateTime, JsValue[] arguments, bool subtract)
    {
        var fields = SignedDuration(engine, TemporalArgument(arguments, 0), subtract);
        var overflow = JsTemporal.Overflow(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
        var internalDuration = DurationMath.ToInternalWith24HourDays(fields);
        var timeResult = IsoArithmetic.AddTime(dateTime.DateTime.Time, internalDuration.TimeNanoseconds, out var days);
        JsTemporal.Check(
            engine,
            DurationMath.CreateDateDurationRecord(internalDuration.Date.Years, internalDuration.Date.Months, internalDuration.Date.Weeks, days, out var dateDuration),
            "the duration");
        var added = JsTemporal.DateAdd(engine, dateTime.Calendar, dateTime.DateTime.Date, dateDuration, overflow);
        return JsTemporal.CreatePlainDateTime(engine, new IsoDateTime(added, timeResult), dateTime.Calendar);
    }

    /// <summary>The proposal's DifferenceTemporalPlainDateTime (s5.5.15): the reads here, the difference the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=59A8A5
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalPlainDateTime(JsEngine engine, JsPlainDateTimeObject dateTime, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToPlainDateTime(engine, TemporalArgument(arguments, 0));
        RequireSameCalendar(engine, dateTime.Calendar, other.Calendar);
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var operation = since ? DifferenceOperation.Since : DifferenceOperation.Until;
        var settings = JsTemporal.DifferenceSettingsOf(engine, operation, options, TemporalType.PlainDateTime);
        JsTemporal.Check(
            engine,
            TemporalDifferences.DifferenceTemporalPlainDateTime(
                operation, JsTemporal.Calendars(engine), JsTemporal.CalendarIdOf(dateTime.Calendar), dateTime.DateTime, other.DateTime, settings, out var result),
            "the date-time");
        return JsTemporal.CreateDuration(engine, result);
    }
}
