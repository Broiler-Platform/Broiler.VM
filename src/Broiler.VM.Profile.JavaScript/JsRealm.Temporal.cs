// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           8
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Temporal</c> namespace (Temporal s1; JSD-0054): its eight types, <c>Temporal.Now</c>, and
/// <c>Date.prototype.toTemporalInstant</c>.
/// </summary>
/// <remarks>
/// The namespace is built only where the composition admits the Temporal surface, which it does only
/// together with <c>Intl</c>: the time zones it reads are the composition's tzdb tables (JSD-0053).
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Temporal.Duration.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5016BD
    // Broiler-Human:        PENDING
    internal JsObject? TemporalDurationPrototype { get; private set; }

    /// <summary><c>%Temporal.Instant.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C8B650
    // Broiler-Human:        PENDING
    internal JsObject? TemporalInstantPrototype { get; private set; }

    /// <summary><c>%Temporal.PlainDate.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=36CD14
    // Broiler-Human:        PENDING
    internal JsObject? TemporalPlainDatePrototype { get; private set; }

    /// <summary><c>%Temporal.PlainTime.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=06FF76
    // Broiler-Human:        PENDING
    internal JsObject? TemporalPlainTimePrototype { get; private set; }

    /// <summary><c>%Temporal.PlainDateTime.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D8D021
    // Broiler-Human:        PENDING
    internal JsObject? TemporalPlainDateTimePrototype { get; private set; }

    /// <summary><c>%Temporal.ZonedDateTime.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0D1455
    // Broiler-Human:        PENDING
    internal JsObject? TemporalZonedDateTimePrototype { get; private set; }

    /// <summary><c>%Temporal.PlainYearMonth.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=209E5B
    // Broiler-Human:        PENDING
    internal JsObject? TemporalPlainYearMonthPrototype { get; private set; }

    /// <summary><c>%Temporal.PlainMonthDay.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=246581
    // Broiler-Human:        PENDING
    internal JsObject? TemporalPlainMonthDayPrototype { get; private set; }

    /// <summary>Builds the <c>Temporal</c> namespace and every type in it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DBD0B0
    // Broiler-Human:        PENDING
    private void SetupTemporal()
    {
        var temporal = new JsObject(ObjectPrototype);

        temporal.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Temporal"), JsPropertyAttributes.Configurable));

        GlobalObject.SetOwnProperty(
            "Temporal",
            JsProperty.Data(JsValue.Object(temporal), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        SetupTemporalDuration(temporal);
        SetupTemporalInstant(temporal);
        SetupTemporalPlainDate(temporal);
        SetupTemporalPlainTime(temporal);
        SetupTemporalPlainDateTime(temporal);
        SetupTemporalZonedDateTime(temporal);
        SetupTemporalPlainYearMonth(temporal);
        SetupTemporalPlainMonthDay(temporal);
        SetupTemporalNow(temporal);

        // DATE.PROTOTYPE.TOTEMPORALINSTANT (s14.9.1): the Date's time value as an instant.
        Method(DatePrototype, "toTemporalInstant", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;
            return JsValue.Object(JsTemporal.CreateInstant(engine, JsTemporal.EpochNsFromMilliseconds(engine, time)));
        });
    }

    /// <summary>
    /// Builds one Temporal type's constructor: called it throws, constructed it reads the prototype
    /// off the new target; it is linked to its prototype and published on the namespace.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BA0FF6
    // Broiler-Human:        PENDING
    private JsNativeFunction TemporalConstructor(JsObject temporal, string name, int arity, JsObject prototype, JsNativeBody construct)
    {
        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            name,
            arity,
            (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Temporal." + name + " requires 'new'"),
            construct);

        constructor.BuildsFromNewTarget = true;
        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Temporal." + name), JsPropertyAttributes.Configurable));
        temporal.SetOwnProperty(
            name,
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
        return constructor;
    }

    /// <summary>Defines a getter with no setter, the shape of every Temporal accessor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=110C79
    // Broiler-Human:        PENDING
    private void TemporalGetter(JsObject host, string name, JsNativeBody body) =>
        host.SetOwnProperty(
            name,
            JsProperty.Accessor(Native("get " + name, 0, body), null, JsPropertyAttributes.Configurable));

    /// <summary>RequireInternalSlot for a Temporal type: the receiver, or a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8488E3
    // Broiler-Human:        PENDING
    private static T TemporalThis<T>(JsEngine engine, JsValue thisValue, string type, string member)
        where T : JsObject =>
        thisValue.AsObjectOrNull() as T ??
        throw engine.Error("TypeError", "Temporal." + type + ".prototype." + member + " requires that 'this' be a Temporal." + type);

    /// <summary>The argument at an index, or undefined.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=68F3C6
    // Broiler-Human:        PENDING
    private static JsValue TemporalArgument(JsValue[] arguments, int index) =>
        index < arguments.Length ? arguments[index] : JsValue.Undefined;

    /// <summary>The valueOf every Temporal type but Instant shares (it throws, steering to compare).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A83914
    // Broiler-Human:        PENDING
    private static JsValue TemporalValueOf(JsEngine engine, string type) =>
        throw engine.Error("TypeError", "Temporal." + type + ".prototype.valueOf: use Temporal." + type + ".compare() or equals() to compare Temporal values");

    /// <summary>Reads a Temporal receiver's ISO date and calendar, or throws its TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2CA518
    // Broiler-Human:        PENDING
    private delegate (JsIsoDate Date, string Calendar) TemporalDateReceiver(JsEngine engine, JsValue thisValue, string member);

    /// <summary>
    /// Defines the calendar getters the date-bearing types share (s3.3.3 to s3.3.18 and their
    /// counterparts), each reading CalendarISOToDate of the receiver.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DED038
    // Broiler-Human:        PENDING
    private void TemporalCalendarGetters(JsObject prototype, string[] names, TemporalDateReceiver receiver)
    {
        foreach (var name in names)
        {
            var member = name;
            TemporalGetter(prototype, member, (engine, thisValue, arguments) =>
            {
                var (date, calendar) = receiver(engine, thisValue, member);

                if (member == "calendarId")
                {
                    return JsValue.String(calendar);
                }

                var parts = JsTemporal.IsoToDate(calendar, date);
                return member switch
                {
                    "era" => parts.Era is null ? JsValue.Undefined : JsValue.String(parts.Era),
                    "eraYear" => parts.EraYear is null ? JsValue.Undefined : JsValue.Number(parts.EraYear.Value),
                    "year" => JsValue.Number(parts.Year),
                    "month" => JsValue.Number(parts.Month),
                    "monthCode" => JsValue.String(parts.MonthCode),
                    "day" => JsValue.Number(parts.Day),
                    "dayOfWeek" => JsValue.Number(parts.DayOfWeek),
                    "dayOfYear" => JsValue.Number(parts.DayOfYear),
                    "weekOfYear" => parts.WeekOfYear is null ? JsValue.Undefined : JsValue.Number(parts.WeekOfYear.Value),
                    "yearOfWeek" => parts.YearOfWeek is null ? JsValue.Undefined : JsValue.Number(parts.YearOfWeek.Value),
                    "daysInWeek" => JsValue.Number(7),
                    "daysInMonth" => JsValue.Number(parts.DaysInMonth),
                    "daysInYear" => JsValue.Number(parts.DaysInYear),
                    "monthsInYear" => JsValue.Number(12),
                    _ => JsValue.Boolean(parts.InLeapYear),
                };
            });
        }
    }

    /// <summary>The calendar getters of a full date, in the order the proposal defines them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5F8D7F
    // Broiler-Human:        PENDING
    private static readonly string[] TemporalDateGetterNames =
    [
        "calendarId", "era", "eraYear", "year", "month", "monthCode", "day", "dayOfWeek", "dayOfYear",
        "weekOfYear", "yearOfWeek", "daysInWeek", "daysInMonth", "daysInYear", "monthsInYear", "inLeapYear",
    ];

    /// <summary>The proposal's IsPartialTemporalObject (s13.24), throwing the TypeError its callers throw.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=61D4C5
    // Broiler-Human:        PENDING
    private static JsObject PartialTemporalObject(JsEngine engine, JsValue value, string type)
    {
        if (!value.IsObject ||
            value.AsObject() is JsPlainDateObject or JsPlainDateTimeObject or JsPlainMonthDayObject or JsPlainTimeObject or JsPlainYearMonthObject or JsZonedDateTimeObject ||
            engine.GetProperty(value, "calendar").Type != JsType.Undefined ||
            engine.GetProperty(value, "timeZone").Type != JsType.Undefined)
        {
            throw engine.Error("TypeError", "Temporal." + type + ".prototype.with: the argument must be a plain object with no calendar or timeZone");
        }

        return value.AsObject();
    }

    /// <summary>A duration's sign-adjusted fields for add or subtract.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E97417
    // Broiler-Human:        PENDING
    private static double[] SignedDuration(JsEngine engine, JsValue durationLike, bool subtract)
    {
        var fields = JsTemporal.ToDuration(engine, durationLike);
        return subtract ? JsTemporal.Negated(fields) : fields;
    }

    /// <summary>The proposal's CalendarEquals (s12.3.16), or the RangeError a difference throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7967EC
    // Broiler-Human:        PENDING
    private static void RequireSameCalendar(JsEngine engine, string one, string two)
    {
        if (one != two)
        {
            throw engine.Error("RangeError", "Temporal: the calendars " + one + " and " + two + " differ");
        }
    }
}
