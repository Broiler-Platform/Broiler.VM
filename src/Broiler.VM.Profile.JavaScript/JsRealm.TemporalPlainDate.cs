// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.PlainDate</c> (Temporal s3; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F46ED9
    // Broiler-Human:        PENDING
    private void SetupTemporalPlainDate(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalPlainDatePrototype = prototype;

        var constructor = TemporalConstructor(temporal, "PlainDate", 3, prototype, static (engine, newTarget, arguments) =>
        {
            var year = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 0));
            var month = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 1));
            var day = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 2));
            var calendar = TemporalCalendarArgument(engine, TemporalArgument(arguments, 3));

            if (!JsTemporalCore.IsValidIsoDate(year, month, day))
            {
                throw engine.Error("RangeError", "Temporal.PlainDate: the date is not valid");
            }

            return JsValue.Object(JsTemporal.CreatePlainDate(engine, new JsIsoDate((long)year, (int)month, (int)day), calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToPlainDate(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToPlainDate(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToPlainDate(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(JsTemporalCore.Compare(one.Date, two.Date));
        });

        TemporalCalendarGetters(prototype, TemporalDateGetterNames, static (engine, thisValue, member) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", member);
            return (date.Date, date.Calendar);
        });

        Method(prototype, "toPlainYearMonth", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toPlainYearMonth");
            var fields = JsTemporal.DateToFields(date.Calendar, date.Date, JsTemporal.FieldsType.Date);
            return JsValue.Object(JsTemporal.CreateYearMonth(engine, JsTemporal.YearMonthFromFields(engine, date.Calendar, fields, reject: false), date.Calendar));
        });

        Method(prototype, "toPlainMonthDay", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toPlainMonthDay");
            var fields = JsTemporal.DateToFields(date.Calendar, date.Date, JsTemporal.FieldsType.Date);
            return JsValue.Object(JsTemporal.CreateMonthDay(engine, JsTemporal.MonthDayFromFields(engine, date.Calendar, fields, reject: false), date.Calendar));
        });

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToDate(engine, TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "add"), arguments, subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToDate(engine, TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "subtract"), arguments, subtract: true)));

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "with");
            var partial = PartialTemporalObject(engine, TemporalArgument(arguments, 0), "PlainDate");
            var fields = JsTemporal.DateToFields(date.Calendar, date.Date, JsTemporal.FieldsType.Date);
            var partialDate = JsTemporal.PrepareFields(
                engine, date.Calendar, partial, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day], [], null, partial: true);
            fields = JsTemporal.MergeFields(date.Calendar, fields, partialDate);
            var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, JsTemporal.DateFromFields(engine, date.Calendar, fields, reject), date.Calendar));
        });

        Method(prototype, "withCalendar", 1, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "withCalendar");
            var calendar = JsTemporal.ToCalendarIdentifier(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, date.Date, calendar));
        });

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainDate(engine, TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainDate(engine, TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "since"), arguments, since: true)));

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "equals");
            var other = JsTemporal.ToPlainDate(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(JsTemporalCore.Compare(date.Date, other.Date) == 0 && date.Calendar == other.Calendar);
        });

        Method(prototype, "toPlainDateTime", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toPlainDateTime");
            var time = JsTemporal.ToTimeOrMidnight(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreatePlainDateTime(engine, new JsIsoDateTime(date.Date, time), date.Calendar));
        });

        Method(prototype, "toZonedDateTime", 1, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toZonedDateTime");
            var item = TemporalArgument(arguments, 0);
            string timeZone;
            var temporalTime = JsValue.Undefined;

            if (item.IsObject)
            {
                var timeZoneLike = engine.GetProperty(item, "timeZone");

                if (timeZoneLike.Type == JsType.Undefined)
                {
                    timeZone = JsTemporal.ToTimeZoneIdentifier(engine, item);
                }
                else
                {
                    timeZone = JsTemporal.ToTimeZoneIdentifier(engine, timeZoneLike);
                    temporalTime = engine.GetProperty(item, "plainTime");
                }
            }
            else
            {
                timeZone = JsTemporal.ToTimeZoneIdentifier(engine, item);
            }

            System.Numerics.BigInteger epochNs;

            if (temporalTime.Type == JsType.Undefined)
            {
                epochNs = JsTemporal.StartOfDay(engine, timeZone, date.Date);
            }
            else
            {
                var time = JsTemporal.ToPlainTime(engine, temporalTime);
                var dateTime = new JsIsoDateTime(date.Date, time);

                if (!JsTemporalCore.DateTimeWithinLimits(dateTime))
                {
                    throw engine.Error("RangeError", "Temporal.PlainDate.prototype.toZonedDateTime: the date-time is outside the representable range");
                }

                epochNs = JsTemporal.EpochNsFor(engine, timeZone, dateTime, "compatible");
            }

            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, epochNs, timeZone, date.Calendar));
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toString");
            var showCalendar = JsTemporal.ShowCalendar(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0)));
            return JsValue.String(JsTemporal.DateString(date.Date, date.Calendar, showCalendar));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toLocaleString");
            return JsValue.String(JsTemporal.DateString(date.Date, date.Calendar, "auto"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var date = TemporalThis<JsPlainDateObject>(engine, thisValue, "PlainDate", "toJSON");
            return JsValue.String(JsTemporal.DateString(date.Date, date.Calendar, "auto"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "PlainDate"));
    }

    /// <summary>A constructor's calendar argument: undefined is ISO 8601, a string is canonicalized, else a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EB77E0
    // Broiler-Human:        PENDING
    private static string TemporalCalendarArgument(JsEngine engine, JsValue value)
    {
        if (value.Type == JsType.Undefined)
        {
            return "iso8601";
        }

        if (!value.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a calendar argument must be a string");
        }

        return JsTemporal.CanonicalizeCalendar(engine, value.AsString());
    }

    /// <summary>The proposal's ToDateDurationRecordWithoutTime (s7.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B7D5AD
    // Broiler-Human:        PENDING
    private static JsDateDuration DateDurationWithoutTime(JsEngine engine, double[] fields)
    {
        var internalDuration = JsTemporalCore.ToInternalWith24HourDays(fields);
        var days = System.Numerics.BigInteger.Divide(internalDuration.Time, JsTemporalCore.NsPerDay);
        return JsTemporal.DateDurationRecord(
            engine, internalDuration.Date.Years, internalDuration.Date.Months, internalDuration.Date.Weeks, JsTemporalCore.ToDouble(days));
    }

    /// <summary>The proposal's AddDurationToDate (s3.5.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7485F7
    // Broiler-Human:        PENDING
    private static JsPlainDateObject AddDurationToDate(JsEngine engine, JsPlainDateObject date, JsValue[] arguments, bool subtract)
    {
        var fields = SignedDuration(engine, TemporalArgument(arguments, 0), subtract);
        var dateDuration = DateDurationWithoutTime(engine, fields);
        var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
        return JsTemporal.CreatePlainDate(engine, JsTemporal.DateAdd(engine, date.Date, dateDuration, reject), date.Calendar);
    }

    /// <summary>The proposal's DifferenceTemporalPlainDate (s3.5.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=69BA1A
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalPlainDate(JsEngine engine, JsPlainDateObject date, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToPlainDate(engine, TemporalArgument(arguments, 0));
        RequireSameCalendar(engine, date.Calendar, other.Calendar);
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var settings = JsTemporal.DifferenceSettings(engine, since, options, JsTemporal.UnitGroup.Date, [], TemporalUnit.Day, TemporalUnit.Day);

        if (JsTemporalCore.Compare(date.Date, other.Date) == 0)
        {
            return JsTemporal.CreateDuration(engine, new double[10]);
        }

        var duration = new JsInternalDuration(JsTemporal.DateUntil(date.Date, other.Date, settings.LargestUnit), 0);

        if (settings.SmallestUnit != TemporalUnit.Day || settings.RoundingIncrement != 1)
        {
            var origin = new JsIsoDateTime(date.Date, JsTimeRecord.Midnight);
            var destination = new JsIsoDateTime(other.Date, JsTimeRecord.Midnight);
            duration = JsTemporal.RoundRelativeDuration(
                engine,
                duration,
                JsTemporalCore.UtcEpochNs(origin),
                JsTemporalCore.UtcEpochNs(destination),
                origin,
                null,
                date.Calendar,
                settings.LargestUnit,
                settings.RoundingIncrement,
                settings.SmallestUnit,
                settings.RoundingMode);
        }

        var result = JsTemporalCore.FromInternal(duration, TemporalUnit.Day);
        return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
    }
}
