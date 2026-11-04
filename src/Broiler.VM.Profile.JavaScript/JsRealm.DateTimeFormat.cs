// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           5
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.DateTimeFormat</c>, the three <c>Date.prototype.toLocale*</c> methods through it, and
/// <c>Intl.supportedValuesOf</c> (ECMA-402 s11, s8.3.2; JSD-0045).
/// </summary>
/// <remarks>
/// <para>
/// <b>The constructor reads its options in ECMA-402's order</b>, each checked as it is read, and the
/// shared Intl library's <c>DateTimeFormat</c> takes the decisions after them (JSD-0082, sub-area (d),
/// decision D24): the hour cycle, the calendar's data, the pattern of the styles or of the components,
/// the numbering system's digits, the zone's names and the formats of Temporal's types. This file keeps
/// the reads, the time zone's resolution as an option, ToDateTimeFormattable, the bound <c>format</c>,
/// the legacy constructor, the parts arrays with their per-part charge, and each <c>RangeError</c> and
/// <c>TypeError</c> with its message, made from the library's status.
/// </para>
/// <para>
/// <b>The time zones are the IANA Time Zone Database's</b> (JSD-0053), from tzdb 2026e, resolved by the
/// shared <c>ResolvedTimeZone</c>: an offset to its <c>+HH:MM</c> or <c>-HH:MM</c> form, and a name, in
/// any ASCII case, to its zone, whose offset is read at each instant formatted, and which is named by its
/// primary identifier. Any other name is a <c>RangeError</c>. <c>resolvedOptions().timeZone</c> is the
/// identifier as given, in the database's case: <c>Asia/Calcutta</c> stays <c>Asia/Calcutta</c>, as
/// <c>tc39/proposal-temporal@e8cc03fc</c>'s <c>intl.html:434</c> has it (row Z8, JSD-0082 decision D15),
/// where until SJL-9.4 it was the primary, <c>Asia/Kolkata</c>.
/// </para>
/// <para>
/// <i>Revised 2026-11-03, at SJL-9.4's sub-area (d).</i> The first paragraph read: "<b>The constructor
/// reads its options in ECMA-402's order</b> and keeps what it resolved. A format built from components
/// takes its pattern from <c>JsDatePatternGenerator</c> and one built from a style takes CLDR's style
/// patterns; both are written with the resolved hour cycle's letter." The second ended: "and a name, in
/// any ASCII case, to its primary identifier: a name resolving to UTC to <c>UTC</c>, IANA's
/// <c>Etc/GMT+N</c> and <c>Etc/GMT-N</c> zones to themselves as fixed offsets, and any other to its zone,
/// whose offset is read at each instant formatted. Any other name is a <c>RangeError</c>."
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.DateTimeFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ADABE5
    // Broiler-Human:        PENDING
    internal JsObject? DateTimeFormatPrototype { get; private set; }

    /// <summary><c>%Intl.DateTimeFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60BC36
    // Broiler-Human:        PENDING
    internal JsNativeFunction? DateTimeFormatConstructor { get; private set; }

    /// <summary>
    /// ECMA-402's table of date-time components (Table 16), in table order, with the values each option
    /// admits, which the constructor reads in that order. (Renamed 2026-11-03, at SJL-9.4's sub-area (d),
    /// from <c>DateTimeComponents</c>, the shared library's record of the values read.)
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 Table 16; IP=Low; Security=Low; Resources=1; Fingerprint=478D1B
    // Broiler-Human:        PENDING
    private static readonly (string Name, string[]? Values)[] DateTimeComponentOptions =
    [
        ("weekday", ["narrow", "short", "long"]),
        ("era", ["narrow", "short", "long"]),
        ("year", ["2-digit", "numeric"]),
        ("month", ["2-digit", "numeric", "narrow", "short", "long"]),
        ("day", ["2-digit", "numeric"]),
        ("dayPeriod", ["narrow", "short", "long"]),
        ("hour", ["2-digit", "numeric"]),
        ("minute", ["2-digit", "numeric"]),
        ("second", ["2-digit", "numeric"]),
        ("fractionalSecondDigits", null),
        ("timeZoneName", ["short", "long", "shortOffset", "longOffset", "shortGeneric", "longGeneric"]),
    ];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5CAFA1
    // Broiler-Human:        PENDING
    private void SetupDateTimeFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        DateTimeFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "DateTimeFormat",
            0,
            static (engine, thisValue, arguments) => NewDateTimeFormat(engine, JsValue.Object(engine.Realm.DateTimeFormatConstructor!), arguments, thisValue, called: true),
            static (engine, newTarget, arguments) => NewDateTimeFormat(engine, newTarget, arguments, JsValue.Undefined, called: false))
        {
            BuildsFromNewTarget = true
        };
        DateTimeFormatConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        prototype.SetOwnProperty(
            "format",
            JsProperty.Accessor(
                Native("get format", 0, static (engine, thisValue, arguments) =>
                {
                    var format = UnwrapDateTimeFormat(engine, thisValue, "format");

                    // [[BoundFormat]]: MADE ONCE, an anonymous function of length 1.
                    if (format.BoundFormat is null)
                    {
                        format.BoundFormat = new JsNativeFunction(
                            engine.Realm,
                            engine.Realm.FunctionPrototype,
                            string.Empty,
                            1,
                            (inner, receiver, values) =>
                            {
                                var (formatter, x) = FormatValue(inner, format, Argument(values, 0));
                                return JsValue.String(formatter.Format(x, inner.WorkCharge));
                            });
                    }

                    return JsValue.Object(format.BoundFormat);
                }),
                null,
                JsPropertyAttributes.Configurable));

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatToParts");
            var (formatter, x) = FormatValue(engine, format, Argument(arguments, 0));
            return DatePartsArray(engine, formatter.FormatToParts(x, engine.WorkCharge));
        });

        Method(prototype, "formatRange", 2, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatRange");
            var (formatter, x, y) = DateRangeValues(engine, format, arguments);
            return JsValue.String(formatter.FormatRange(x, y, engine.WorkCharge));
        });

        Method(prototype, "formatRangeToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatRangeToParts");
            var (formatter, x, y) = DateRangeValues(engine, format, arguments);
            return DatePartsArray(engine, formatter.FormatRangeToParts(x, y, engine.WorkCharge));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = UnwrapDateTimeFormat(engine, thisValue, "resolvedOptions").Format;
            var options = new JsObject(engine.Realm.ObjectPrototype);

            void Put(string name, string? value)
            {
                if (value is not null)
                {
                    options.DefineOrdinary(name, JsValue.String(value));
                }
            }

            // TABLE 16'S ORDER, as the shared format resolved it: the hour cycle where the pattern has an
            // hour, the components of a format made from components, fractionalSecondDigits a Number.
            Put("locale", format.Locale);
            Put("calendar", format.Calendar);
            Put("numberingSystem", format.NumberingSystem);
            Put("timeZone", format.TimeZone);

            if (format.HourCycle is { } hourCycle)
            {
                Put("hourCycle", hourCycle);
                options.DefineOrdinary("hour12", JsValue.Boolean(format.Hour12 == true));
            }

            foreach (var (name, value) in format.ResolvedComponents)
            {
                if (name == "fractionalSecondDigits")
                {
                    options.DefineOrdinary(name, JsValue.Number(value[0] - '0'));
                }
                else
                {
                    Put(name, value);
                }
            }

            Put("dateStyle", format.DateStyle);
            Put("timeStyle", format.TimeStyle);
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.DateTimeFormat"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("DateTimeFormat", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>
    /// The start and end a range method formats: each read by ToDateTimeFormattable, then the shared
    /// format's HandleDateTimeRange, two Temporal objects of one type or two times, and the format they
    /// are written in.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 sec-intl.datetimeformat.prototype.formatRange as proposal-temporal e8cc03fc amends it, JSD-0082 sub-area (d); IP=Low; Security=Low; Resources=1; Fingerprint=6F64E2
    // Broiler-Human:        PENDING
    private static (DateTimeFormatter Formatter, double X, double Y) DateRangeValues(JsEngine engine, JsDateTimeFormatObject format, JsValue[] arguments)
    {
        var start = Argument(arguments, 0);
        var end = Argument(arguments, 1);

        if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Intl.DateTimeFormat: a range needs a start and an end");
        }

        var x = DateTimeFormattable(engine, format, start);
        var y = DateTimeFormattable(engine, format, end);
        var status = format.Format.HandleDateTimeRange(FormattableValue(x), FormattableValue(y), out var formatter, out var xTime, out var yTime);

        if (status != IntlStatus.Completed)
        {
            // THE VALUE THE MESSAGE NAMES: the start where the start alone is refused, else the end.
            var refused = status == IntlStatus.RangeOfDifferentTypes ||
                format.Format.HandleDateTimeValue(FormattableValue(x), out _, out _) != IntlStatus.Completed ? x : y;
            throw DateTimeValueError(engine, format, refused, status);
        }

        return (formatter!, xTime, yTime);
    }

    /// <summary>
    /// The value format and the time of one argument of format or formatToParts: now when it is
    /// undefined. (Renamed 2026-11-03, at SJL-9.4's sub-area (d), from <c>DateTimeValue</c>, the shared
    /// library's record of a value to format.)
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 sec-datetime-format-functions, JSD-0082 sub-area (d); IP=Low; Security=Low; Resources=1; Fingerprint=D598F8
    // Broiler-Human:        PENDING
    private static (DateTimeFormatter Formatter, double Time) FormatValue(JsEngine engine, JsDateTimeFormatObject format, JsValue date) =>
        date.Type == JsType.Undefined
            ? (format.Format.Formatter, DateCurrentTime())
            : HandleDateTimeValue(engine, format, DateTimeFormattable(engine, format, date));

    /// <summary>
    /// The proposal's ToDateTimeFormattable (amended ECMA-402 s15.6.11): a Temporal object as itself
    /// where the realm has Temporal, anything else as a Number.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5A4866
    // Broiler-Human:        PENDING
    private static JsValue DateTimeFormattable(JsEngine engine, JsDateTimeFormatObject format, JsValue value) =>
        format.FormatsTemporal && IsFormattableTemporal(value) ? value : JsValue.Number(engine.ToNumber(value));

    /// <summary>The proposal's IsTemporalObject (amended ECMA-402 s15.6.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=040218
    // Broiler-Human:        PENDING
    private static bool IsFormattableTemporal(JsValue value) =>
        value.AsObjectOrNull() is JsPlainDateObject or JsPlainTimeObject or JsPlainDateTimeObject or JsZonedDateTimeObject
            or JsPlainYearMonthObject or JsPlainMonthDayObject or JsInstantObject;

    /// <summary>
    /// What ToDateTimeFormattable gave, as the shared library's value to format: a Temporal object's
    /// slots, or a Number's time value.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=proposal-temporal e8cc03fc sec-temporal-handledatetimevalue, JSD-0082 sub-area (d); IP=Low; Security=Low; Resources=1; Fingerprint=511961
    // Broiler-Human:        PENDING
    private static DateTimeValue FormattableValue(JsValue x) => x.AsObjectOrNull() switch
    {
        JsPlainDateObject date => DateTimeValue.PlainDate(date.Date, date.Calendar),
        JsPlainYearMonthObject yearMonth => DateTimeValue.PlainYearMonth(yearMonth.Date, yearMonth.Calendar),
        JsPlainMonthDayObject monthDay => DateTimeValue.PlainMonthDay(monthDay.Date, monthDay.Calendar),
        JsPlainTimeObject time => DateTimeValue.PlainTime(time.Time),
        JsPlainDateTimeObject dateTime => DateTimeValue.PlainDateTime(dateTime.DateTime, dateTime.Calendar),
        JsInstantObject instant => DateTimeValue.Instant(instant.EpochNanoseconds),
        JsZonedDateTimeObject zoned => DateTimeValue.ZonedDateTime(zoned.EpochNanoseconds, zoned.Calendar),
        _ => DateTimeValue.Number(x.AsNumber()),
    };

    /// <summary>
    /// The proposal's HandleDateTimeValue (amended ECMA-402 s15.6.22), the shared format's: the format a
    /// value is written in and its time in milliseconds; a plain value's format is written at UTC, so no
    /// zone moves it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BB3A1C
    // Broiler-Human:        PENDING
    private static (DateTimeFormatter Formatter, double Time) HandleDateTimeValue(JsEngine engine, JsDateTimeFormatObject format, JsValue x)
    {
        var status = format.Format.HandleDateTimeValue(FormattableValue(x), out var formatter, out var time);
        return status == IntlStatus.Completed ? (formatter!, time) : throw DateTimeValueError(engine, format, x, status);
    }

    /// <summary>
    /// The error of a value the shared format refuses, with the message this family wrote before the
    /// library took the steps: a <c>RangeError</c> for a time TimeClip refuses or a calendar the format
    /// does not write, a <c>TypeError</c> for a type the options give no field of, a ZonedDateTime, or a
    /// range of two types.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=proposal-temporal e8cc03fc sec-temporal-handledatetimevalue, JSD-0082 sub-area (d); IP=Low; Security=Low; Resources=1; Fingerprint=58D6B7
    // Broiler-Human:        PENDING
    private static Exception DateTimeValueError(JsEngine engine, JsDateTimeFormatObject format, JsValue x, IntlStatus status)
    {
        var value = x.AsObjectOrNull();
        var type = value switch
        {
            JsPlainDateObject => "PlainDate",
            JsPlainYearMonthObject => "PlainYearMonth",
            JsPlainMonthDayObject => "PlainMonthDay",
            JsPlainTimeObject => "PlainTime",
            JsPlainDateTimeObject => "PlainDateTime",
            _ => "Instant",
        };
        var calendar = value switch
        {
            JsPlainDateObject date => date.Calendar,
            JsPlainYearMonthObject yearMonth => yearMonth.Calendar,
            JsPlainMonthDayObject monthDay => monthDay.Calendar,
            JsPlainDateTimeObject dateTime => dateTime.Calendar,
            JsZonedDateTimeObject zoned => zoned.Calendar,
            _ => "iso8601",
        };

        return status switch
        {
            IntlStatus.TimeValueOutOfRange => engine.Error("RangeError", "Invalid time value"),
            IntlStatus.TemporalCalendarMismatch when value is JsZonedDateTimeObject =>
                engine.Error("RangeError", "Temporal.ZonedDateTime.prototype.toLocaleString: a value in the " + calendar + " calendar cannot be formatted in the " + format.Format.Calendar + " calendar"),
            IntlStatus.TemporalCalendarMismatch =>
                engine.Error("RangeError", "Intl.DateTimeFormat: a Temporal." + type + " in the " + calendar + " calendar cannot be formatted in the " + format.Format.Calendar + " calendar"),
            IntlStatus.TemporalTypeNotFormatted => engine.Error("TypeError", "Intl.DateTimeFormat: the options give no field of a Temporal." + type),
            IntlStatus.ZonedDateTimeNotFormatted =>
                engine.Error("TypeError", "Intl.DateTimeFormat: a Temporal.ZonedDateTime cannot be formatted; use its toLocaleString, or format its toInstant()"),
            IntlStatus.RangeOfDifferentTypes =>
                engine.Error("TypeError", "Intl.DateTimeFormat: a range's start and end must be two Temporal objects of one type, or two times"),
            _ => new InvalidOperationException("The shared date-time format answered " + status + ", which no step of this adapter expects."),
        };
    }

    /// <summary>
    /// ECMA-402's toLocaleString of a Temporal object (as the proposal amends s15.11): a date-time
    /// format made for the call, which writes the object; a ZonedDateTime's in its own zone, through its
    /// instant, after its calendar is checked.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34564F
    // Broiler-Human:        PENDING
    internal static string TemporalToLocaleString(JsEngine engine, JsValue value, JsValue[] arguments, DateTimeRequired required, DateTimeDefaults defaults, string? timeZone = null)
    {
        var format = CreateDateTimeFormat(
            engine, engine.Realm.DateTimeFormatPrototype!, Argument(arguments, 0), Argument(arguments, 1), required, defaults, timeZone);
        var (formatter, time) = HandleDateTimeValue(engine, format, value);
        return formatter.Format(time, engine.WorkCharge);
    }

    /// <summary>An array of <c>{ type, value }</c> objects, with <c>source</c> where a part has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3850E4
    // Broiler-Human:        PENDING
    private static JsValue DatePartsArray(JsEngine engine, IReadOnlyList<DateTimePart> parts)
    {
        var values = new List<JsValue>(parts.Count);

        foreach (var part in parts)
        {
            engine.Charge(1);
            var item = new JsObject(engine.Realm.ObjectPrototype);
            item.DefineOrdinary("type", JsValue.String(part.Type));
            item.DefineOrdinary("value", JsValue.String(part.Value));

            if (part.Source is not null)
            {
                item.DefineOrdinary("source", JsValue.String(part.Source));
            }

            values.Add(JsValue.Object(item));
        }

        return JsValue.Object(engine.Realm.NewArray(values));
    }

    /// <summary>The date-time format <paramref name="thisValue"/> is, with no unwrapping, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=840F34
    // Broiler-Human:        PENDING
    private static JsDateTimeFormatObject DateTimeFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDateTimeFormatObject ??
        throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an Intl.DateTimeFormat");

    /// <summary>
    /// ECMA-402's UnwrapDateTimeFormat: the format itself, or the one a legacy construction stored on
    /// an object inheriting from <c>Intl.DateTimeFormat.prototype</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=377996
    // Broiler-Human:        PENDING
    private static JsDateTimeFormatObject UnwrapDateTimeFormat(JsEngine engine, JsValue thisValue, string member)
    {
        if (!thisValue.IsObject)
        {
            throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an object");
        }

        if (thisValue.AsObject() is JsDateTimeFormatObject direct)
        {
            return direct;
        }

        var realm = engine.Realm;

        if (engine.OrdinaryHasInstance(JsValue.Object(realm.DateTimeFormatConstructor!), thisValue) &&
            engine.GetSymbolWithReceiver(thisValue.AsObject(), realm.IntlFallbackSymbol!, thisValue).AsObjectOrNull() is JsDateTimeFormatObject wrapped)
        {
            return wrapped;
        }

        throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an Intl.DateTimeFormat");
    }

    /// <summary>
    /// ECMA-402's <c>Intl.DateTimeFormat</c> constructor: the prototype from the new target, then
    /// CreateDateTimeFormat, then the legacy path when called as a function.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BC10DE
    // Broiler-Human:        PENDING
    internal static JsValue NewDateTimeFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments, JsValue thisValue, bool called)
    {
        var realm = engine.Realm;
        var prototype = engine.PrototypeFromConstructor(newTarget, realm.DateTimeFormatPrototype!);
        var made = CreateDateTimeFormat(engine, prototype, Argument(arguments, 0), Argument(arguments, 1), DateTimeRequired.Any, DateTimeDefaults.Date);

        // CHAINDATETIMEFORMAT, ECMA-402'S NORMATIVE OPTIONAL LEGACY PATH.
        if (called && thisValue.IsObject &&
            engine.OrdinaryHasInstance(JsValue.Object(realm.DateTimeFormatConstructor!), thisValue))
        {
            var fields = new ObjectDescriptorFields
            {
                HasValue = true,
                Value = JsValue.Object(made),
                HasWritable = true,
                Writable = false,
                HasEnumerable = true,
                Enumerable = false,
                HasConfigurable = true,
                Configurable = false,
            };

            ObjectApplyDescriptorAt(engine, thisValue.AsObject(), JsValue.Symbol(realm.IntlFallbackSymbol!), fields);
            return thisValue;
        }

        return JsValue.Object(made);
    }

    /// <summary>
    /// The text a <c>Date.prototype.toLocale*</c> method answers through <c>Intl.DateTimeFormat</c>:
    /// a format made for the call with <paramref name="required"/> and <paramref name="defaults"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4175F8
    // Broiler-Human:        PENDING
    internal static string ToLocaleDateString(JsEngine engine, double time, JsValue[] arguments, DateTimeRequired required, DateTimeDefaults defaults)
    {
        var format = CreateDateTimeFormat(engine, engine.Realm.DateTimeFormatPrototype!, Argument(arguments, 0), Argument(arguments, 1), required, defaults);
        return format.Format.Formatter.Format(time, engine.WorkCharge);
    }

    /// <summary>
    /// ECMA-402's CreateDateTimeFormat (s11.1.2, as <c>tc39/proposal-temporal@e8cc03fc</c> amends it):
    /// every read in the specification's order, each checked as it is read, then the shared library's
    /// <c>DateTimeFormat.Create</c> over the record read, whose refusals are this family's
    /// <c>TypeError</c>s (JSD-0082, sub-area (d)).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=415122
    // Broiler-Human:        PENDING
    internal static JsDateTimeFormatObject CreateDateTimeFormat(JsEngine engine, JsObject prototype, JsValue locales, JsValue optionsValue, DateTimeRequired required, DateTimeDefaults defaults, string? toLocaleStringTimeZone = null)
    {
        var requested = CanonicalizeLocaleList(engine, locales);
        var options = CoerceOptionsToObject(engine, optionsValue);

        // RESOLVEOPTIONS: the locale matcher, then each resolution option in the constructor's order.
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var calendar = GetStringOption(engine, options, "calendar", null, null);

        if (calendar is not null && !IsTypeSequence(calendar))
        {
            throw engine.Error("RangeError", "Intl.DateTimeFormat: the calendar option is not a well-formed type: " + calendar);
        }

        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            throw engine.Error("RangeError", "Intl.DateTimeFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        var hour12 = GetBooleanOption(engine, options, "hour12", null);
        var hourCycle = GetStringOption(engine, options, "hourCycle", ["h11", "h12", "h23", "h24"], null);

        // [[LOCALEDATA]]'S ca, hc AND nu VALUES are the shared format's: the nu values the locale's
        // default numbering system first, latn, then the data's others, for this family's tables.
        var tables = engine.Intl!;
        var resolved = ResolveLocale(
            engine,
            requested,
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["ca"] = calendar,
                ["hc"] = hour12 is null ? hourCycle : NullOption,
                ["nu"] = numberingSystem,
            },
            ["ca", "hc", "nu"],
            (locale, key) => key switch
            {
                "ca" => [.. DateTimeFormat.CalendarKeyValues],
                "hc" => [.. DateTimeFormat.HourCycleKeyValues],
                "nu" => [.. DateTimeFormat.NumberingSystemKeyValues(tables.FormatData, locale)],
                _ => [null],
            });

        // TEMPORAL'S ZonedDateTime.prototype.toLocaleString NAMES ITS OWN ZONE (the proposal's
        // amendment of CreateDateTimeFormat), and refuses one the options name as well.
        var timeZoneOption = engine.GetProperty(options, "timeZone");

        if (toLocaleStringTimeZone is not null)
        {
            if (timeZoneOption.Type != JsType.Undefined)
            {
                throw engine.Error("TypeError", "Temporal.ZonedDateTime.prototype.toLocaleString: the timeZone option cannot be given; the value's own zone is used");
            }

            timeZoneOption = JsValue.String(toLocaleStringTimeZone);
        }

        var timeZone = ResolveTimeZone(engine, timeZoneOption, tables.TimeZones);

        var components = new Dictionary<string, string>(StringComparer.Ordinal);
        int? fractionalSecondDigits = null;

        foreach (var (name, values) in DateTimeComponentOptions)
        {
            if (name == "fractionalSecondDigits")
            {
                fractionalSecondDigits = GetNumberOption(engine, options, name, 1, 3, null);
            }
            else if (GetStringOption(engine, options, name, values, null) is { } value)
            {
                components[name] = value;
            }
        }

        _ = GetStringOption(engine, options, "formatMatcher", ["basic", "best fit"], "best fit");
        var dateStyle = GetStringOption(engine, options, "dateStyle", ["full", "long", "medium", "short"], null);
        var timeStyle = GetStringOption(engine, options, "timeStyle", ["full", "long", "medium", "short"], null);

        string? Component(string name) => components.TryGetValue(name, out var value) ? value : null;

        var status = DateTimeFormat.Create(
            tables.DateTimeData,
            new ResolvedLocale(resolved.Locale, resolved.DataLocale, resolved.Keys),
            new DateTimeFormatOptions
            {
                Hour12 = hour12,
                TimeZone = timeZone,
                Components = new DateTimeComponents
                {
                    Weekday = Component("weekday"),
                    Era = Component("era"),
                    Year = Component("year"),
                    Month = Component("month"),
                    Day = Component("day"),
                    DayPeriod = Component("dayPeriod"),
                    Hour = Component("hour"),
                    Minute = Component("minute"),
                    Second = Component("second"),
                    FractionalSecondDigits = fractionalSecondDigits,
                    TimeZoneName = Component("timeZoneName"),
                },
                DateStyle = dateStyle,
                TimeStyle = timeStyle,
                Required = required,
                Defaults = defaults,
                ForZonedDateTime = toLocaleStringTimeZone is not null,
            },
            out var format);

        return status switch
        {
            IntlStatus.Completed => new JsDateTimeFormatObject(prototype, format!)
            {
                // THE FORMATS OF TEMPORAL'S OBJECTS are written only where the realm has Temporal.
                FormatsTemporal = engine.Realm.TemporalDurationPrototype is not null,
            },
            IntlStatus.StyleWithComponents => throw engine.Error("TypeError", "Intl.DateTimeFormat: dateStyle and timeStyle cannot be combined with date-time components"),
            IntlStatus.DateFormatWithTimeStyle => throw engine.Error("TypeError", "Intl.DateTimeFormat: a date format cannot have a timeStyle"),
            IntlStatus.TimeFormatWithDateStyle => throw engine.Error("TypeError", "Intl.DateTimeFormat: a time format cannot have a dateStyle"),
            _ => throw new InvalidOperationException("The shared date-time format answered " + status + ", which CreateDateTimeFormat does not expect."),
        };
    }

    /// <summary>
    /// The time zone an option names (ECMA-402 11.1.2 steps 29 to 36, as
    /// <c>tc39/proposal-temporal@e8cc03fc</c>'s <c>intl.html:434</c> amends them; JSD-0053): UTC by
    /// default, and otherwise the option's String resolved by the shared <c>ResolvedTimeZone</c>, an
    /// offset string by its <c>+HH:MM</c> or <c>-HH:MM</c> form and an IANA Zone or Link name matched
    /// without regard to ASCII case. [[TimeZone]] is the identifier as given, in the database's case,
    /// and the zone is named by its primary identifier (row Z8, JSD-0082 decision D15). This keeps the
    /// option's conversion and the two <c>RangeError</c>s; the resolution charges nothing, as before
    /// (JSD-0082, decision D18, row L43).
    /// </summary>
    /// <remarks>
    /// <i>Revised 2026-11-03, at SJL-9.4's sub-area (d).</i> The summary read: "The time zone an option
    /// names: UTC by default, an offset string by its <c>+HH:MM</c> or <c>-HH:MM</c> form, and otherwise
    /// an IANA Zone or Link name, matched without regard to ASCII case and resolved to its primary
    /// identifier (ECMA-402 11.1.2 steps 29 to 36, JSD-0053): a name resolving to UTC as <c>UTC</c>, an
    /// <c>Etc/GMT</c> zone with an hour as its fixed offset, and any other as the zone." The remarks
    /// read: "The offset strings, the <c>Etc/GMT</c> zones and the lookup are the shared library's since
    /// SJL-8.5 (JSD-0076, JSD-0077): <c>OffsetTimeZones</c> reads and writes the offsets and names the
    /// <c>Etc/GMT</c> zones' fixed offsets, and the Intl data's <c>TimeZoneDatabase</c> finds a name, its
    /// primary identifier and its rules. This keeps the option's conversion, the two <c>RangeError</c>s and
    /// the formatter's choice to report the primary identifier." Until row Z8's correction, made by the
    /// commit after sub-area (d)'s, the zone was resolved again from its primary identifier, so that the
    /// format reported the primary.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=433463
    // Broiler-Human:        PENDING
    private static ResolvedTimeZone ResolveTimeZone(JsEngine engine, JsValue value, ITimeZoneDatabase zones)
    {
        if (value.Type == JsType.Undefined)
        {
            return ResolvedTimeZone.Utc;
        }

        var text = engine.ToStringValue(value);

        return ResolvedTimeZone.Resolve(text, zones, out var zone) switch
        {
            IntlStatus.Completed => zone!,
            IntlStatus.TimeZoneOffsetHasSeconds => throw engine.Error("RangeError", "Intl.DateTimeFormat: a time zone offset cannot have seconds: " + text),
            _ => throw engine.Error("RangeError", "Intl.DateTimeFormat: " + text + " is not an available time zone"),
        };
    }
}

/// <summary>
/// An <c>Intl.DateTimeFormat</c>: the shared library's resolved format, and the bound <c>format</c>.
/// (Revised 2026-11-03, at SJL-9.4's sub-area (d); it read "An <c>Intl.DateTimeFormat</c>: its resolved
/// options and its formatter.", and held the formatter, the resolved locale, calendar, numbering system,
/// hour cycle and styles, and the formats of Temporal's objects, which the shared format now holds.)
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A917A0
// Broiler-Human:        PENDING
internal sealed class JsDateTimeFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=67C10E
    // Broiler-Human:        PENDING
    internal JsDateTimeFormatObject(JsObject prototype, DateTimeFormat format) : base(prototype)
    {
        Format = format;
    }

    /// <summary>The shared library's format: what CreateDateTimeFormat resolved, its formatter and the formats of Temporal's types.</summary>
    // Broiler-AI:           Origin=AI; Spec=JSD-0082 sub-area (d); IP=Low; Security=Low; Resources=1; Fingerprint=311BCD
    // Broiler-Human:        PENDING
    internal DateTimeFormat Format { get; }

    /// <summary>Whether the realm had Temporal when the format was made, so that ToDateTimeFormattable takes a Temporal object as itself.</summary>
    // Broiler-AI:           Origin=AI; Spec=proposal-temporal e8cc03fc sec-temporal-todatetimeformattable; IP=Low; Security=Low; Resources=1; Fingerprint=C4D121
    // Broiler-Human:        PENDING
    internal bool FormatsTemporal { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0782CD
    // Broiler-Human:        PENDING
    internal JsNativeFunction? BoundFormat { get; set; }
}
