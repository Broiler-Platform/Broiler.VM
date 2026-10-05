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

/// <summary><c>Temporal.PlainYearMonth</c> (Temporal s9) and <c>Temporal.PlainMonthDay</c> (s10; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E2433
    // Broiler-Human:        PENDING
    private void SetupTemporalPlainYearMonth(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalPlainYearMonthPrototype = prototype;

        var constructor = TemporalConstructor(temporal, "PlainYearMonth", 2, prototype, static (engine, newTarget, arguments) =>
        {
            var referenceDay = TemporalArgument(arguments, 3);
            var year = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 0));
            var month = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 1));
            var calendar = TemporalCalendarArgument(engine, TemporalArgument(arguments, 2));
            var day = referenceDay.Type == JsType.Undefined ? 1 : JsTemporal.IntegerWithTruncation(engine, referenceDay);

            if (!JsTemporalCore.IsValidIsoDate(year, month, day))
            {
                throw engine.Error("RangeError", "Temporal.PlainYearMonth: the date is not valid");
            }

            return JsValue.Object(JsTemporal.CreateYearMonth(engine, new JsIsoDate((long)year, (int)month, (int)day), calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToYearMonth(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToYearMonth(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToYearMonth(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(JsTemporalCore.Compare(one.Date, two.Date));
        });

        TemporalCalendarGetters(
            prototype,
            ["calendarId", "era", "eraYear", "year", "month", "monthCode", "daysInYear", "daysInMonth", "monthsInYear", "inLeapYear"],
            static (engine, thisValue, member) =>
            {
                var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", member);
                return (yearMonth.Date, yearMonth.Calendar);
            });

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "with");
            var partial = PartialTemporalObject(engine, TemporalArgument(arguments, 0), "PlainYearMonth");
            var calendar = yearMonth.Calendar;
            var fields = JsTemporal.DateToFields(calendar, yearMonth.Date, JsTemporal.FieldsType.YearMonth);
            var partialFields = JsTemporal.PrepareFields(
                engine, calendar, partial, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode], [], null, partial: true);
            fields = JsTemporal.MergeFields(calendar, fields, partialFields);
            var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            return JsValue.Object(JsTemporal.CreateYearMonth(engine, JsTemporal.YearMonthFromFields(engine, calendar, fields, reject), calendar));
        });

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToYearMonth(engine, TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "add"), arguments, subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToYearMonth(engine, TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "subtract"), arguments, subtract: true)));

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainYearMonth(engine, TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainYearMonth(engine, TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "since"), arguments, since: true)));

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "equals");
            var other = JsTemporal.ToYearMonth(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(JsTemporalCore.Compare(yearMonth.Date, other.Date) == 0 && yearMonth.Calendar == other.Calendar);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "toString");
            var showCalendar = JsTemporal.ShowCalendar(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0)));
            return JsValue.String(JsTemporal.YearMonthString(yearMonth.Date, yearMonth.Calendar, showCalendar));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "toLocaleString");
            return JsValue.String(TemporalToLocaleString(engine, thisValue, arguments, "date", "date"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "toJSON");
            return JsValue.String(JsTemporal.YearMonthString(yearMonth.Date, yearMonth.Calendar, "auto"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "PlainYearMonth"));

        Method(prototype, "toPlainDate", 1, static (engine, thisValue, arguments) =>
        {
            var yearMonth = TemporalThis<JsPlainYearMonthObject>(engine, thisValue, "PlainYearMonth", "toPlainDate");
            var item = TemporalArgument(arguments, 0);

            if (!item.IsObject)
            {
                throw engine.Error("TypeError", "Temporal.PlainYearMonth.prototype.toPlainDate: the argument must be an object with a day");
            }

            var calendar = yearMonth.Calendar;
            var fields = JsTemporal.DateToFields(calendar, yearMonth.Date, JsTemporal.FieldsType.YearMonth);
            var inputFields = JsTemporal.PrepareFields(engine, calendar, item.AsObject(), [CalendarField.Day], [], []);
            var merged = JsTemporal.MergeFields(calendar, fields, inputFields);
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, JsTemporal.DateFromFields(engine, calendar, merged, reject: false), calendar));
        });
    }

    /// <summary>The proposal's AddDurationToYearMonth (s9.5.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FF5592
    // Broiler-Human:        PENDING
    private static JsPlainYearMonthObject AddDurationToYearMonth(JsEngine engine, JsPlainYearMonthObject yearMonth, JsValue[] arguments, bool subtract)
    {
        var fields = SignedDuration(engine, TemporalArgument(arguments, 0), subtract);
        var internalDuration = JsTemporalCore.ToInternal(fields);
        var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
        var durationToAdd = internalDuration.Date;

        if (durationToAdd.Weeks != 0 || durationToAdd.Days != 0 || !internalDuration.Time.IsZero)
        {
            throw engine.Error("RangeError", "Temporal.PlainYearMonth: only years and months can be added to a year-month");
        }

        var calendar = yearMonth.Calendar;
        var startFields = JsTemporal.DateToFields(calendar, yearMonth.Date, JsTemporal.FieldsType.YearMonth);
        startFields.Day = 1;
        var date = JsTemporal.DateFromFields(engine, calendar, startFields, reject: false);
        var added = JsTemporal.DateAdd(engine, date, durationToAdd, reject);
        var addedFields = JsTemporal.DateToFields(calendar, added, JsTemporal.FieldsType.YearMonth);
        return JsTemporal.CreateYearMonth(engine, JsTemporal.YearMonthFromFields(engine, calendar, addedFields, reject), calendar);
    }

    /// <summary>The proposal's DifferenceTemporalPlainYearMonth (s9.5.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4070B2
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalPlainYearMonth(JsEngine engine, JsPlainYearMonthObject yearMonth, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToYearMonth(engine, TemporalArgument(arguments, 0));
        var calendar = yearMonth.Calendar;
        RequireSameCalendar(engine, calendar, other.Calendar);
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var settings = JsTemporal.DifferenceSettings(
            engine, since, options, JsTemporal.UnitGroup.Date, [TemporalUnit.Week, TemporalUnit.Day], TemporalUnit.Month, TemporalUnit.Year);

        if (JsTemporalCore.Compare(yearMonth.Date, other.Date) == 0)
        {
            return JsTemporal.CreateDuration(engine, new double[10]);
        }

        var thisFields = JsTemporal.DateToFields(calendar, yearMonth.Date, JsTemporal.FieldsType.YearMonth);
        thisFields.Day = 1;
        var thisDate = JsTemporal.DateFromFields(engine, calendar, thisFields, reject: false);
        var otherFields = JsTemporal.DateToFields(calendar, other.Date, JsTemporal.FieldsType.YearMonth);
        otherFields.Day = 1;
        var otherDate = JsTemporal.DateFromFields(engine, calendar, otherFields, reject: false);
        var dateDifference = JsTemporal.DateUntil(thisDate, otherDate, settings.LargestUnit);
        var duration = new JsInternalDuration(JsTemporal.AdjustDate(engine, dateDifference, 0, 0), 0);

        if (settings.SmallestUnit != TemporalUnit.Month || settings.RoundingIncrement != 1)
        {
            var origin = new JsIsoDateTime(thisDate, JsTimeRecord.Midnight);
            var destination = new JsIsoDateTime(otherDate, JsTimeRecord.Midnight);
            duration = JsTemporal.RoundRelativeDuration(
                engine,
                duration,
                JsTemporalCore.UtcEpochNs(origin),
                JsTemporalCore.UtcEpochNs(destination),
                origin,
                null,
                calendar,
                settings.LargestUnit,
                settings.RoundingIncrement,
                settings.SmallestUnit,
                settings.RoundingMode);
        }

        var result = JsTemporalCore.FromInternal(duration, TemporalUnit.Day);
        return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
    }

    // ---- Temporal.PlainMonthDay -----------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=57CB25
    // Broiler-Human:        PENDING
    private void SetupTemporalPlainMonthDay(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalPlainMonthDayPrototype = prototype;

        var constructor = TemporalConstructor(temporal, "PlainMonthDay", 2, prototype, static (engine, newTarget, arguments) =>
        {
            var referenceYear = TemporalArgument(arguments, 3);
            var month = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 0));
            var day = JsTemporal.IntegerWithTruncation(engine, TemporalArgument(arguments, 1));
            var calendar = TemporalCalendarArgument(engine, TemporalArgument(arguments, 2));
            var year = referenceYear.Type == JsType.Undefined ? 1972 : JsTemporal.IntegerWithTruncation(engine, referenceYear);

            if (!JsTemporalCore.IsValidIsoDate(year, month, day))
            {
                throw engine.Error("RangeError", "Temporal.PlainMonthDay: the date is not valid");
            }

            return JsValue.Object(JsTemporal.CreateMonthDay(engine, new JsIsoDate((long)year, (int)month, (int)day), calendar, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.ToMonthDay(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1))));

        TemporalCalendarGetters(prototype, ["calendarId", "monthCode", "day"], static (engine, thisValue, member) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", member);
            return (monthDay.Date, monthDay.Calendar);
        });

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "with");
            var partial = PartialTemporalObject(engine, TemporalArgument(arguments, 0), "PlainMonthDay");
            var calendar = monthDay.Calendar;
            var fields = JsTemporal.DateToFields(calendar, monthDay.Date, JsTemporal.FieldsType.MonthDay);
            var partialFields = JsTemporal.PrepareFields(
                engine, calendar, partial, [CalendarField.Year, CalendarField.Month, CalendarField.MonthCode, CalendarField.Day], [], null, partial: true);
            fields = JsTemporal.MergeFields(calendar, fields, partialFields);
            var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            return JsValue.Object(JsTemporal.CreateMonthDay(engine, JsTemporal.MonthDayFromFields(engine, calendar, fields, reject), calendar));
        });

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "equals");
            var other = JsTemporal.ToMonthDay(engine, TemporalArgument(arguments, 0));
            return JsValue.Boolean(JsTemporalCore.Compare(monthDay.Date, other.Date) == 0 && monthDay.Calendar == other.Calendar);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "toString");
            var showCalendar = JsTemporal.ShowCalendar(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0)));
            return JsValue.String(JsTemporal.MonthDayString(monthDay.Date, monthDay.Calendar, showCalendar));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "toLocaleString");
            return JsValue.String(TemporalToLocaleString(engine, thisValue, arguments, "date", "date"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "toJSON");
            return JsValue.String(JsTemporal.MonthDayString(monthDay.Date, monthDay.Calendar, "auto"));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "PlainMonthDay"));

        Method(prototype, "toPlainDate", 1, static (engine, thisValue, arguments) =>
        {
            var monthDay = TemporalThis<JsPlainMonthDayObject>(engine, thisValue, "PlainMonthDay", "toPlainDate");
            var item = TemporalArgument(arguments, 0);

            if (!item.IsObject)
            {
                throw engine.Error("TypeError", "Temporal.PlainMonthDay.prototype.toPlainDate: the argument must be an object with a year");
            }

            var calendar = monthDay.Calendar;
            var fields = JsTemporal.DateToFields(calendar, monthDay.Date, JsTemporal.FieldsType.MonthDay);
            var inputFields = JsTemporal.PrepareFields(engine, calendar, item.AsObject(), [CalendarField.Year], [], []);
            var merged = JsTemporal.MergeFields(calendar, fields, inputFields);
            return JsValue.Object(JsTemporal.CreatePlainDate(engine, JsTemporal.DateFromFields(engine, calendar, merged, reject: false), calendar));
        });
    }
}
