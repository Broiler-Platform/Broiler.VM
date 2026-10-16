// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           9
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  2/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Date</c> constructor, its statics and <c>Date.prototype</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>The calendar is the specification's own arithmetic and not <c>System.DateTime</c>.</b> The
/// language's time value ranges over ±8.64e15 milliseconds, which is roughly the years -271821 to
/// 275760, and <c>System.DateTime</c> covers 0001 to 9999. Every operation outside that window -
/// <c>new Date(-8.64e15)</c>, <c>new Date(275760, 8, 13)</c>, and every conformance test built on
/// them - would be an <c>ArgumentOutOfRangeException</c> in a conversion where the language answers
/// a number. Since milestone SJL-6.4 (JSD-0073) the arithmetic, the Date string grammar and the
/// renderings are the shared library's, <c>Broiler.ECMAScript.Time</c>: <c>EcmaTime</c> over doubles,
/// <c>EcmaDateParser</c> in this profile's dialect and <c>EcmaDateFormatter</c>. What stays here is
/// the adapters: argument conversion and its order, the time value's storage, the charge,
/// <c>toISOString</c>'s <c>RangeError</c>, <c>toJSON</c>'s object protocol and <c>toLocale*</c>'s
/// routing. The only clock call in the file is the one that reads the current instant.
/// </para>
/// <para>
/// <b>This profile fixes the local time zone to UTC</b> (JSD-0053 section 7; decision SJL-D-19 makes
/// it the provider this profile supplies). The specification's <c>LocalTime</c> and <c>UTC</c> are
/// therefore the identity, which is why <c>getHours</c> and <c>getUTCHours</c> are literally the same
/// function body, why <c>getTimezoneOffset</c> answers zero, and why a date-time string with no offset
/// is read as UTC. The provider, <see cref="DateZone"/>, goes where the shared library takes one: the
/// renderings, and a parse's local result. A host time zone would make a benchmark's numbers depend on
/// where it ran, and this profile is built to be measured.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>What every string-producing method answers for a NaN time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4B9F73
    // Broiler-Human:        PENDING
    private const string DateInvalidText = EcmaDateFormatter.InvalidDate;

    /// <summary>
    /// This profile's local time zone: UTC, an offset of zero named <c>Coordinated Universal Time</c>,
    /// as the renderings write it (decision SJL-D-19; JSD-0053 section 7).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9D3FD5
    // Broiler-Human:        PENDING
    private static readonly FixedOffsetLocalTimeProvider DateZone = new(0, "Coordinated Universal Time");

    /// <summary>Builds <c>Date</c>, its statics and <c>Date.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3CFF7B
    // Broiler-Human:        PENDING
    private void SetupDate()
    {
        var constructor = Constructor(
            "Date",
            7,
            DatePrototype,

            // CALLED AS A FUNCTION, `Date` IGNORES ITS ARGUMENTS AND RETURNS A STRING.
            // `Date(2020, 0, 1)` is not a shorthand for `new Date(2020, 0, 1)`; it is the current
            // instant rendered, and code that drops the `new` gets a String rather than an object.
            static (engine, thisValue, arguments) => JsValue.String(EcmaDateFormatter.ToDateString(DateCurrentTime(), DateZone)),

            static (engine, thisValue, arguments) =>
            {
                double time;

                if (arguments.Length == 0)
                {
                    time = DateCurrentTime();
                }
                else if (arguments.Length == 1)
                {
                    time = DateTimeValueOf(engine, arguments[0]);
                }
                else
                {
                    time = DateFromFields(engine, arguments);
                }

                return JsValue.Object(new DateObject(engine.Realm.DatePrototype, time));
            });

        // `Date.now` HAS TO SURVIVE BEING TORN OFF ITS CONSTRUCTOR. A harness that writes
        // `var now = Date.now; ... now()` calls it with an undefined receiver, so the body reads
        // nothing from `thisValue` at all.
        Method(constructor, "now", 0, static (engine, thisValue, arguments) => JsValue.Number(DateCurrentTime()));
        Method(constructor, "parse", 1, static (engine, thisValue, arguments) => JsValue.Number(DateParseText(engine, engine.ToStringValue(DateArg(arguments, 0)))));
        Method(constructor, "UTC", 7, static (engine, thisValue, arguments) => JsValue.Number(DateFromFields(engine, arguments)));

        // THE HOTTEST BUILT-IN IN THE OCTANE HARNESS. Tens of thousands of calls reach this body,
        // so it is a type test and a field read with nothing allocated on either side of them -
        // no argument array walk, no receiver coercion, no boxing. `JsValue.Number` is a struct.
        Method(DatePrototype, "valueOf", 0, static (engine, thisValue, arguments) =>
        {
            if (thisValue.IsObject && thisValue.AsObject() is DateObject date)
            {
                return JsValue.Number(date.TimeValue);
            }

            throw engine.Error("TypeError", "Date.prototype.valueOf is not generic");
        });

        Method(DatePrototype, "getTime", 0, static (engine, thisValue, arguments) =>
        {
            if (thisValue.IsObject && thisValue.AsObject() is DateObject date)
            {
                return JsValue.Number(date.TimeValue);
            }

            throw engine.Error("TypeError", "Date.prototype.getTime is not generic");
        });

        Method(DatePrototype, "setTime", 1, static (engine, thisValue, arguments) =>
        {
            var date = DateReceiver(engine, thisValue);
            var time = EcmaTime.TimeClip(engine.ToNumber(DateArg(arguments, 0)));
            date.TimeValue = time;
            return JsValue.Number(time);
        });

        // THE LOCAL AND UTC GETTERS ARE ONE BODY REGISTERED TWICE, because this profile's
        // LocalTime is the identity. Two definitions of the same arithmetic could drift; one
        // cannot.
        DatePair("getFullYear", "getUTCFullYear", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Year));
        DatePair("getMonth", "getUTCMonth", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Month));
        DatePair("getDate", "getUTCDate", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.DayOfMonth));
        DatePair("getDay", "getUTCDay", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.DayOfWeek));
        DatePair("getHours", "getUTCHours", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Hour));
        DatePair("getMinutes", "getUTCMinutes", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Minute));
        DatePair("getSeconds", "getUTCSeconds", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Second));
        DatePair("getMilliseconds", "getUTCMilliseconds", static (engine, thisValue, arguments) => DateFieldOf(engine, thisValue, DateFieldKind.Millisecond));

        // ZERO, BECAUSE THE PROFILE'S ZONE IS UTC - but NaN for an Invalid Date, which is what the
        // specification says and what a test that reads `new Date(NaN).getTimezoneOffset()` checks.
        Method(DatePrototype, "getTimezoneOffset", 0, static (engine, thisValue, arguments) =>
        {
            var date = DateReceiver(engine, thisValue);
            return JsValue.Number(double.IsNaN(date.TimeValue) ? double.NaN : 0);
        });

        DateSetterPair("setFullYear", "setUTCFullYear", 3, static (engine, thisValue, arguments) => DateSetDateFields(engine, thisValue, arguments, 0));
        DateSetterPair("setMonth", "setUTCMonth", 2, static (engine, thisValue, arguments) => DateSetDateFields(engine, thisValue, arguments, 1));
        DateSetterPair("setDate", "setUTCDate", 1, static (engine, thisValue, arguments) => DateSetDateFields(engine, thisValue, arguments, 2));
        DateSetterPair("setHours", "setUTCHours", 4, static (engine, thisValue, arguments) => DateSetTimeFields(engine, thisValue, arguments, 0));
        DateSetterPair("setMinutes", "setUTCMinutes", 3, static (engine, thisValue, arguments) => DateSetTimeFields(engine, thisValue, arguments, 1));
        DateSetterPair("setSeconds", "setUTCSeconds", 2, static (engine, thisValue, arguments) => DateSetTimeFields(engine, thisValue, arguments, 2));
        DateSetterPair("setMilliseconds", "setUTCMilliseconds", 1, static (engine, thisValue, arguments) => DateSetTimeFields(engine, thisValue, arguments, 3));

        Method(DatePrototype, "toString", 0, static (engine, thisValue, arguments) => JsValue.String(EcmaDateFormatter.ToDateString(DateReceiver(engine, thisValue).TimeValue, DateZone)));
        Method(DatePrototype, "toDateString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;
            return JsValue.String(double.IsNaN(time) ? DateInvalidText : EcmaDateFormatter.DateString(EcmaLocalTime.LocalTime(time, DateZone)));
        });

        Method(DatePrototype, "toTimeString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;
            return JsValue.String(double.IsNaN(time) ? DateInvalidText : DateTimeText(time));
        });

        Method(DatePrototype, "toUTCString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;
            return JsValue.String(EcmaDateFormatter.ToUtcString(time));
        });

        // ANNEX B'S `toGMTString` IS `toUTCString` ITSELF, the same function object (JSP-7, JSC-239).
        _ = DatePrototype.TryGetOwnProperty("toUTCString", out var utcText);

        DatePrototype.SetOwnProperty("toGMTString", JsProperty.Data(utcText.Value, JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        // ANNEX B'S TWO-DIGIT YEAR PAIR (JSP-7, JSC-239). `getYear` is the local year less 1900;
        // `setYear` reads 0 to 99 as 1900 to 1999 and any other year as itself, starting from +0
        // when the date is invalid. The local time is the time value, because this profile's zone
        // is UTC.
        Method(DatePrototype, "getYear", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;
            return JsValue.Number(double.IsNaN(time) ? double.NaN : EcmaTime.YearFromTime(time) - 1900);
        });

        Method(DatePrototype, "setYear", 1, static (engine, thisValue, arguments) =>
        {
            var date = DateReceiver(engine, thisValue);
            var time = double.IsNaN(date.TimeValue) ? 0 : date.TimeValue;
            var year = engine.ToNumber(DateArg(arguments, 0));

            if (double.IsNaN(year))
            {
                date.TimeValue = double.NaN;
                return JsValue.Number(double.NaN);
            }

            var day = EcmaTime.MakeDay(EcmaTime.MakeFullYear(year), EcmaTime.MonthFromTime(time), EcmaTime.DateFromTime(time));
            var clipped = EcmaTime.TimeClip(EcmaTime.MakeDate(day, EcmaTime.TimeWithinDay(time)));
            date.TimeValue = clipped;
            return JsValue.Number(clipped);
        });

        Method(DatePrototype, "toISOString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            // AN INVALID DATE THROWS HERE AND NOWHERE ELSE. Every other renderer answers
            // "Invalid Date"; this one has no such string in its grammar, so it has to throw.
            return EcmaDateFormatter.TryToIsoString(time, out var text)
                ? JsValue.String(text)
                : engine.ThrowRangeError("Invalid time value");
        });

        // GENERIC BY DEFINITION, NOT A DATE METHOD (ES2026 Date.prototype.toJSON; since
        // 2026-09-22, JSeal VM-FIX-J): any receiver goes through ToObject and ToPrimitive with hint
        // number, and whatever `toISOString` that object has is invoked - it read a Date's time
        // value and refused every other receiver. A non-finite Number answers `null`, which is how
        // JSON.stringify serialises an invalid Date rather than throwing through toISOString.
        Method(DatePrototype, "toJSON", 1, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var target = JsValue.Object(engine.ToObject(thisValue));
            var primitive = engine.ToPrimitive(target, "number");

            if (primitive.IsNumber && !double.IsFinite(primitive.AsNumber()))
            {
                return JsValue.Null;
            }

            return engine.Call(engine.GetProperty(target, "toISOString"), target, Array.Empty<JsValue>());
        });

        // WITHOUT INTL THE LOCALE FORMS ARE THE PLAIN FORMS: the specification allows an
        // implementation-defined result, and answering the same text is honest about carrying no
        // locale data. With Intl they are ECMA-402's (s20.4): an invalid date is "Invalid Date"
        // before the arguments are read, and any other is formatted by a DateTimeFormat made for
        // the call (JSD-0045).
        Method(DatePrototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            if (engine.Realm.DateTimeFormatPrototype is null)
            {
                return JsValue.String(EcmaDateFormatter.ToDateString(time, DateZone));
            }

            return JsValue.String(double.IsNaN(time) ? DateInvalidText : ToLocaleDateString(engine, time, arguments, "any", "all"));
        });

        Method(DatePrototype, "toLocaleDateString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            if (double.IsNaN(time))
            {
                return JsValue.String(DateInvalidText);
            }

            return JsValue.String(engine.Realm.DateTimeFormatPrototype is null
                ? EcmaDateFormatter.DateString(EcmaLocalTime.LocalTime(time, DateZone))
                : ToLocaleDateString(engine, time, arguments, "date", "date"));
        });

        Method(DatePrototype, "toLocaleTimeString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            if (double.IsNaN(time))
            {
                return JsValue.String(DateInvalidText);
            }

            return JsValue.String(engine.Realm.DateTimeFormatPrototype is null
                ? DateTimeText(time)
                : ToLocaleDateString(engine, time, arguments, "time", "time"));
        });

        void DatePair(string localName, string utcName, JsNativeBody body)
        {
            Method(DatePrototype, localName, 0, body);
            Method(DatePrototype, utcName, 0, body);
        }

        void DateSetterPair(string localName, string utcName, int arity, JsNativeBody body)
        {
            Method(DatePrototype, localName, arity, body);
            Method(DatePrototype, utcName, arity, body);
        }
    }

    /// <summary>Which broken-down field a getter reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A9411C
    // Broiler-Human:        PENDING
    private enum DateFieldKind
    {
        /// <summary>The full year, unabbreviated and possibly negative.</summary>
        Year = 0,

        /// <summary>The month, zero-based.</summary>
        Month = 1,

        /// <summary>The day of the month, one-based.</summary>
        DayOfMonth = 2,

        /// <summary>The day of the week, Sunday zero.</summary>
        DayOfWeek = 3,

        /// <summary>The hour.</summary>
        Hour = 4,

        /// <summary>The minute.</summary>
        Minute = 5,

        /// <summary>The second.</summary>
        Second = 6,

        /// <summary>The millisecond.</summary>
        Millisecond = 7,
    }

    /// <summary>Reads one argument, answering <c>undefined</c> past the end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=730D5A
    // Broiler-Human:        PENDING
    private static JsValue DateArg(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>The receiver as a Date, or a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E4D494
    // Broiler-Human:        PENDING
    private static DateObject DateReceiver(JsEngine engine, JsValue value)
    {
        if (value.IsObject && value.AsObject() is DateObject date)
        {
            return date;
        }

        throw engine.Error("TypeError", "this is not a Date object");
    }

    /// <summary>The current instant as a time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AA47FD
    // Broiler-Human:        PENDING
    private static double DateCurrentTime() =>
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>One broken-down field of a receiver's time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A06590
    // Broiler-Human:        PENDING
    private static JsValue DateFieldOf(JsEngine engine, JsValue thisValue, DateFieldKind field)
    {
        var time = DateReceiver(engine, thisValue).TimeValue;

        if (double.IsNaN(time))
        {
            // AN INVALID DATE ANSWERS NaN FROM EVERY GETTER rather than throwing or answering a
            // plausible-looking zero, which is what makes `isNaN(d.getTime())` the idiomatic test.
            return JsValue.Number(double.NaN);
        }

        return JsValue.Number(field switch
        {
            DateFieldKind.Year => EcmaTime.YearFromTime(time),
            DateFieldKind.Month => EcmaTime.MonthFromTime(time),
            DateFieldKind.DayOfMonth => EcmaTime.DateFromTime(time),
            DateFieldKind.DayOfWeek => EcmaTime.WeekDay(time),
            DateFieldKind.Hour => EcmaTime.HourFromTime(time),
            DateFieldKind.Minute => EcmaTime.MinFromTime(time),
            DateFieldKind.Second => EcmaTime.SecFromTime(time),
            _ => EcmaTime.MsFromTime(time),
        });
    }

    /// <summary>
    /// What <c>toTimeString</c> answers for a valid date: the specification's <c>TimeString</c> of the
    /// local time and <c>TimeZoneString</c> (21.4.4.42), <c>"00:00:00 GMT+0000 (Coordinated Universal Time)"</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4FEFA9
    // Broiler-Human:        PENDING
    private static string DateTimeText(double time) =>
        EcmaDateFormatter.TimeString(EcmaLocalTime.LocalTime(time, DateZone)) + EcmaDateFormatter.TimeZoneString(time, DateZone);

    // ---- the one-argument constructor ------------------------------------------------------------

    /// <summary>The time value <c>new Date(value)</c> produces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B82B8F
    // Broiler-Human:        PENDING
    private static double DateTimeValueOf(JsEngine engine, JsValue value)
    {
        if (value.IsObject && value.AsObject() is DateObject other)
        {
            // `new Date(existing)` COPIES THE TIME VALUE DIRECTLY and does not go through the
            // prototype, so a program that replaced `Date.prototype.valueOf` still gets a copy.
            return other.TimeValue;
        }

        var primitive = engine.ToPrimitive(value, "default");

        return primitive.IsString
            ? DateParseText(engine, primitive.AsString())
            : EcmaTime.TimeClip(engine.ToNumber(primitive));
    }

    /// <summary>
    /// The time value the seven-field form produces, shared by <c>new Date(y, m, …)</c> and
    /// <c>Date.UTC</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E389C9
    // Broiler-Human:        PENDING
    private static double DateFromFields(JsEngine engine, JsValue[] arguments)
    {
        // EVERY SUPPLIED FIELD IS COERCED, IN ORDER, BEFORE ANY OF THEM IS USED. A `valueOf` on
        // the third argument that throws must throw after the first two have been converted.
        var year = engine.ToNumber(DateArg(arguments, 0));
        var month = arguments.Length > 1 ? engine.ToNumber(arguments[1]) : 0;
        var day = arguments.Length > 2 ? engine.ToNumber(arguments[2]) : 1;
        var hour = arguments.Length > 3 ? engine.ToNumber(arguments[3]) : 0;
        var minute = arguments.Length > 4 ? engine.ToNumber(arguments[4]) : 0;
        var second = arguments.Length > 5 ? engine.ToNumber(arguments[5]) : 0;
        var milli = arguments.Length > 6 ? engine.ToNumber(arguments[6]) : 0;

        // THE TWO-DIGIT YEAR WINDOW: 0..99 MEANS 1900..1999 (MakeFullYear). It is a compatibility wart the
        // language kept, and `new Date(99, 0, 1)` being 1999 rather than year 99 is observable.
        return EcmaTime.TimeClip(EcmaTime.MakeDate(
            EcmaTime.MakeDay(EcmaTime.MakeFullYear(year), month, day), EcmaTime.MakeTime(hour, minute, second, milli)));
    }

    // ---- the setters -----------------------------------------------------------------------------

    /// <summary>
    /// Whether the field numbered <paramref name="field"/> is one this call supplies.
    /// </summary>
    /// <remarks>
    /// The first field a setter names is always supplied, even when the caller passed nothing:
    /// <c>setMilliseconds()</c> converts <c>undefined</c> and lands on NaN, where
    /// <c>setHours(1)</c> leaves the minutes alone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F957F7
    // Broiler-Human:        PENDING
    private static bool DateFieldSupplied(JsValue[] arguments, int firstField, int field)
    {
        var at = field - firstField;
        return at == 0 || (at > 0 && at < arguments.Length);
    }

    /// <summary>The coerced argument for one field, or NaN when this call does not supply it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6D239B
    // Broiler-Human:        PENDING
    private static double DateFieldArgument(JsEngine engine, JsValue[] arguments, int firstField, int field)
    {
        var at = field - firstField;

        if (at < 0)
        {
            return double.NaN;
        }

        return at == 0 || at < arguments.Length
            ? engine.ToNumber(DateArg(arguments, at))
            : double.NaN;
    }

    /// <summary>The shared body of <c>setHours</c>, <c>setMinutes</c>, <c>setSeconds</c> and
    /// <c>setMilliseconds</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F73FB
    // Broiler-Human:        PENDING
    private static JsValue DateSetTimeFields(JsEngine engine, JsValue thisValue, JsValue[] arguments, int firstField)
    {
        var date = DateReceiver(engine, thisValue);
        var time = date.TimeValue;

        // COERCION RUNS BEFORE THE NaN TEST. The specification orders it that way, so a setter on
        // an Invalid Date still calls the arguments' `valueOf` - and still throws when one throws.
        var hour = DateFieldArgument(engine, arguments, firstField, 0);
        var minute = DateFieldArgument(engine, arguments, firstField, 1);
        var second = DateFieldArgument(engine, arguments, firstField, 2);
        var milli = DateFieldArgument(engine, arguments, firstField, 3);

        if (double.IsNaN(time))
        {
            return JsValue.Number(double.NaN);
        }

        if (!DateFieldSupplied(arguments, firstField, 0))
        {
            hour = EcmaTime.HourFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 1))
        {
            minute = EcmaTime.MinFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 2))
        {
            second = EcmaTime.SecFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 3))
        {
            milli = EcmaTime.MsFromTime(time);
        }

        var result = EcmaTime.TimeClip(
            EcmaTime.MakeDate(EcmaTime.Day(time), EcmaTime.MakeTime(hour, minute, second, milli)));

        date.TimeValue = result;
        return JsValue.Number(result);
    }

    /// <summary>The shared body of <c>setFullYear</c>, <c>setMonth</c> and <c>setDate</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=22DA3D
    // Broiler-Human:        PENDING
    private static JsValue DateSetDateFields(JsEngine engine, JsValue thisValue, JsValue[] arguments, int firstField)
    {
        var date = DateReceiver(engine, thisValue);
        var time = date.TimeValue;
        var year = DateFieldArgument(engine, arguments, firstField, 0);
        var month = DateFieldArgument(engine, arguments, firstField, 1);
        var day = DateFieldArgument(engine, arguments, firstField, 2);

        if (double.IsNaN(time))
        {
            if (firstField != 0)
            {
                return JsValue.Number(double.NaN);
            }

            // `setFullYear` ALONE REVIVES AN INVALID DATE. It substitutes the epoch for the
            // unusable time value and carries on, because it supplies every field the calendar
            // needs; `setMonth` on an Invalid Date has no year to build on and stays NaN.
            time = 0;
        }

        if (!DateFieldSupplied(arguments, firstField, 0))
        {
            year = EcmaTime.YearFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 1))
        {
            month = EcmaTime.MonthFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 2))
        {
            day = EcmaTime.DateFromTime(time);
        }

        var result = EcmaTime.TimeClip(
            EcmaTime.MakeDate(EcmaTime.MakeDay(year, month, day), EcmaTime.TimeWithinDay(time)));

        date.TimeValue = result;
        return JsValue.Number(result);
    }

    // ---- parsing ---------------------------------------------------------------------------------

    /// <summary>
    /// <c>Date.parse</c> and the string constructor: the shared parser, in this profile's dialect.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The charge is this profile's, <c>length + 1</c> at entry</b> (JSD-0072, decision 4). The shared
    /// parser charges the text's length before it reads a character, when the text is not empty; this
    /// adapter charges the one unit beyond it first, so every total, and every exhaustion's point, is what
    /// it was before SJL-6.4 (the split JSD-0065, decision 2, made for number parsing).
    /// </para>
    /// <para>
    /// <b>The dialect is this profile's</b>, <c>DateParseDialect.BroilerVM</c>: the Date Time String Format
    /// with a day past its month's length carried into the next month, and for any other text the textual
    /// reading of the renderings, which reads a sign and digits as a year, so that the renderings of
    /// negative years read back (correction P9). A local result - a date-time form with no offset, or a
    /// textual form with no zone - is read in this profile's zone, UTC.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=046E47
    // Broiler-Falsified-If: Date.parse charges a total, or exhausts at a point, other than length + 1 at entry
    // Broiler-Human:        PENDING
    private static double DateParseText(JsEngine engine, string text)
    {
        engine.Charge(1UL);
        var (kind, value) = EcmaDateParser.Parse(text, DateParseDialect.BroilerVM, engine.WorkCharge);

        return kind switch
        {
            EcmaParsedKind.Utc => value,
            EcmaParsedKind.Local => EcmaTime.TimeClip(EcmaLocalTime.Utc(value, DateZone)),
            _ => double.NaN,
        };
    }

    /// <summary>A Date: an ordinary object carrying one number.</summary>
    /// <remarks>
    /// The time value is a field rather than a property in the object's own map because
    /// <c>valueOf</c> reads it on the hottest path the harness has, and because a property would be
    /// reachable from guest code - a Date whose internal slot could be assigned is not a Date.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=35390D
    // Broiler-Human:        PENDING
    private sealed class DateObject : JsObject
    {
        /// <summary>Creates a Date holding <paramref name="timeValue"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8E6887
        // Broiler-Human:        PENDING
        internal DateObject(JsObject? prototype, double timeValue)
            : base(prototype, "Date") => TimeValue = timeValue;

        /// <summary>The specification's <c>[[DateValue]]</c>: milliseconds since the epoch, or NaN.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=96FB65
        // Broiler-Human:        PENDING
        internal double TimeValue { get; set; }
    }
}
