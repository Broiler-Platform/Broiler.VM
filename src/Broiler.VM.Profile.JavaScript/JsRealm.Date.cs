// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   53
// Annotated:        53/53
// Exempt:           9
// Human-reviewed:   0/53
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       53
//
// GENERATED - DO NOT EDIT MANUALLY

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
/// a number. So <c>Day</c>, <c>YearFromTime</c>, <c>MakeDay</c> and the rest are written here as
/// double arithmetic that has no representable range of its own, and the only clock call in the
/// file is the one that reads the current instant.
/// </para>
/// <para>
/// <b>This profile fixes the local time zone to UTC.</b> The specification's <c>LocalTime</c> and
/// <c>UTC</c> operations are therefore the identity, which is why <c>getHours</c> and
/// <c>getUTCHours</c> are literally the same function body, why <c>getTimezoneOffset</c> answers
/// zero, and why a date-time string with no offset is read as UTC. A host time zone would make a
/// benchmark's numbers depend on where it ran, and this profile is built to be measured.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Milliseconds in a second.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8F794D
    // Broiler-Human:        PENDING
    private const double DateMsPerSecond = 1000;

    /// <summary>Milliseconds in a minute.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5D8F9D
    // Broiler-Human:        PENDING
    private const double DateMsPerMinute = 60000;

    /// <summary>Milliseconds in an hour.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F7604
    // Broiler-Human:        PENDING
    private const double DateMsPerHour = 3600000;

    /// <summary>Milliseconds in a day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AEB3AA
    // Broiler-Human:        PENDING
    private const double DateMsPerDay = 86400000;

    /// <summary>The largest magnitude a time value may have before it clips to NaN.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F88230
    // Broiler-Human:        PENDING
    private const double DateMaxTimeValue = 8.64e15;

    /// <summary>What every string-producing method answers for a NaN time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=76C62D
    // Broiler-Human:        PENDING
    private const string DateInvalidText = "Invalid Date";

    /// <summary>The zone tail this profile's fixed UTC offset produces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E611F2
    // Broiler-Human:        PENDING
    private const string DateZoneText = " GMT+0000 (Coordinated Universal Time)";

    /// <summary>Builds <c>Date</c>, its statics and <c>Date.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4A1236
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
            static (engine, thisValue, arguments) =>
                JsValue.String(DateToFullText(DateCurrentTime())),

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
        Method(constructor, "now", 0, static (engine, thisValue, arguments) =>
            JsValue.Number(DateCurrentTime()));

        Method(constructor, "parse", 1, static (engine, thisValue, arguments) =>
            JsValue.Number(DateParseText(engine, engine.ToStringValue(DateArg(arguments, 0)))));

        Method(constructor, "UTC", 7, static (engine, thisValue, arguments) =>
            JsValue.Number(DateFromFields(engine, arguments)));

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
            var time = DateTimeClip(engine.ToNumber(DateArg(arguments, 0)));
            date.TimeValue = time;
            return JsValue.Number(time);
        });

        // THE LOCAL AND UTC GETTERS ARE ONE BODY REGISTERED TWICE, because this profile's
        // LocalTime is the identity. Two definitions of the same arithmetic could drift; one
        // cannot.
        DatePair("getFullYear", "getUTCFullYear", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.Year));

        DatePair("getMonth", "getUTCMonth", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.Month));

        DatePair("getDate", "getUTCDate", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.DayOfMonth));

        DatePair("getDay", "getUTCDay", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.DayOfWeek));

        DatePair("getHours", "getUTCHours", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.Hour));

        DatePair("getMinutes", "getUTCMinutes", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.Minute));

        DatePair("getSeconds", "getUTCSeconds", static (engine, thisValue, arguments) =>
            DateFieldOf(engine, thisValue, DateFieldKind.Second));

        DatePair(
            "getMilliseconds",
            "getUTCMilliseconds",
            static (engine, thisValue, arguments) =>
                DateFieldOf(engine, thisValue, DateFieldKind.Millisecond));

        // ZERO, BECAUSE THE PROFILE'S ZONE IS UTC - but NaN for an Invalid Date, which is what the
        // specification says and what a test that reads `new Date(NaN).getTimezoneOffset()` checks.
        Method(DatePrototype, "getTimezoneOffset", 0, static (engine, thisValue, arguments) =>
        {
            var date = DateReceiver(engine, thisValue);
            return JsValue.Number(double.IsNaN(date.TimeValue) ? double.NaN : 0);
        });

        DateSetterPair("setFullYear", "setUTCFullYear", 3, static (engine, thisValue, arguments) =>
            DateSetDateFields(engine, thisValue, arguments, 0));

        DateSetterPair("setMonth", "setUTCMonth", 2, static (engine, thisValue, arguments) =>
            DateSetDateFields(engine, thisValue, arguments, 1));

        DateSetterPair("setDate", "setUTCDate", 1, static (engine, thisValue, arguments) =>
            DateSetDateFields(engine, thisValue, arguments, 2));

        DateSetterPair("setHours", "setUTCHours", 4, static (engine, thisValue, arguments) =>
            DateSetTimeFields(engine, thisValue, arguments, 0));

        DateSetterPair("setMinutes", "setUTCMinutes", 3, static (engine, thisValue, arguments) =>
            DateSetTimeFields(engine, thisValue, arguments, 1));

        DateSetterPair("setSeconds", "setUTCSeconds", 2, static (engine, thisValue, arguments) =>
            DateSetTimeFields(engine, thisValue, arguments, 2));

        DateSetterPair(
            "setMilliseconds",
            "setUTCMilliseconds",
            1,
            static (engine, thisValue, arguments) =>
                DateSetTimeFields(engine, thisValue, arguments, 3));

        Method(DatePrototype, "toString", 0, static (engine, thisValue, arguments) =>
            JsValue.String(DateToFullText(DateReceiver(engine, thisValue).TimeValue)));

        Method(DatePrototype, "toDateString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            return JsValue.String(
                double.IsNaN(time) ? DateInvalidText : DateToCalendarText(time));
        });

        Method(DatePrototype, "toTimeString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            return JsValue.String(
                double.IsNaN(time) ? DateInvalidText : DateToClockText(time) + DateZoneText);
        });

        Method(DatePrototype, "toUTCString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            return JsValue.String(double.IsNaN(time) ? DateInvalidText : DateToUtcText(time));
        });

        Method(DatePrototype, "toISOString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            // AN INVALID DATE THROWS HERE AND NOWHERE ELSE. Every other renderer answers
            // "Invalid Date"; this one has no such string in its grammar, so it has to throw.
            return double.IsNaN(time)
                ? engine.ThrowRangeError("Invalid time value")
                : JsValue.String(DateToIsoText(time));
        });

        Method(DatePrototype, "toJSON", 1, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            // JSON.stringify SERIALISES AN INVALID DATE AS `null` RATHER THAN THROWING, which is
            // why this is not simply a call to toISOString.
            return double.IsNaN(time) ? JsValue.Null : JsValue.String(DateToIsoText(time));
        });

        // THE LOCALE FORMS ARE THE PLAIN FORMS. This profile carries no locale data, and the
        // specification allows an implementation-defined result; answering the same text is
        // honest about that, where inventing a format would only look like locale support.
        Method(DatePrototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
            JsValue.String(DateToFullText(DateReceiver(engine, thisValue).TimeValue)));

        Method(DatePrototype, "toLocaleDateString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            return JsValue.String(
                double.IsNaN(time) ? DateInvalidText : DateToCalendarText(time));
        });

        Method(DatePrototype, "toLocaleTimeString", 0, static (engine, thisValue, arguments) =>
        {
            var time = DateReceiver(engine, thisValue).TimeValue;

            return JsValue.String(
                double.IsNaN(time) ? DateInvalidText : DateToClockText(time) + DateZoneText);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CC0887
    // Broiler-Human:        PENDING
    private static double DateCurrentTime() =>
        System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>One broken-down field of a receiver's time value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=739F75
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
            DateFieldKind.Year => DateYearFromTime(time),
            DateFieldKind.Month => DateMonthFromTime(time),
            DateFieldKind.DayOfMonth => DateDateFromTime(time),
            DateFieldKind.DayOfWeek => DateWeekDay(time),
            DateFieldKind.Hour => DateHourFromTime(time),
            DateFieldKind.Minute => DateMinFromTime(time),
            DateFieldKind.Second => DateSecFromTime(time),
            _ => DateMsFromTime(time),
        });
    }

    // ---- the one-argument constructor ------------------------------------------------------------

    /// <summary>The time value <c>new Date(value)</c> produces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2105CA
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
            : DateTimeClip(engine.ToNumber(primitive));
    }

    /// <summary>
    /// The time value the seven-field form produces, shared by <c>new Date(y, m, …)</c> and
    /// <c>Date.UTC</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A7CC40
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

        if (!double.IsNaN(year))
        {
            var integral = JsValue.ToInteger(year);

            // THE TWO-DIGIT YEAR WINDOW: 0..99 MEANS 1900..1999. It is a compatibility wart the
            // language kept, and `new Date(99, 0, 1)` being 1999 rather than year 99 is observable.
            if (integral >= 0 && integral <= 99)
            {
                year = 1900 + integral;
            }
        }

        return DateTimeClip(
            DateMakeDate(DateMakeDay(year, month, day), DateMakeTime(hour, minute, second, milli)));
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
    private static double DateFieldArgument(
        JsEngine engine, JsValue[] arguments, int firstField, int field)
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7EAFC0
    // Broiler-Human:        PENDING
    private static JsValue DateSetTimeFields(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, int firstField)
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
            hour = DateHourFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 1))
        {
            minute = DateMinFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 2))
        {
            second = DateSecFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 3))
        {
            milli = DateMsFromTime(time);
        }

        var result = DateTimeClip(
            DateMakeDate(DateDay(time), DateMakeTime(hour, minute, second, milli)));

        date.TimeValue = result;
        return JsValue.Number(result);
    }

    /// <summary>The shared body of <c>setFullYear</c>, <c>setMonth</c> and <c>setDate</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8C97AF
    // Broiler-Human:        PENDING
    private static JsValue DateSetDateFields(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, int firstField)
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
            year = DateYearFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 1))
        {
            month = DateMonthFromTime(time);
        }

        if (!DateFieldSupplied(arguments, firstField, 2))
        {
            day = DateDateFromTime(time);
        }

        var result = DateTimeClip(
            DateMakeDate(DateMakeDay(year, month, day), DateTimeWithinDay(time)));

        date.TimeValue = result;
        return JsValue.Number(result);
    }

    // ---- the specification's own operations over a time value ------------------------------------

    /// <summary>
    /// The specification's <c>modulo</c>, which takes the sign of the divisor.
    /// </summary>
    /// <remarks>
    /// C#'s <c>%</c> takes the sign of the dividend, so <c>-1 % 24</c> is <c>-1</c> where the
    /// language needs <c>23</c>. Every clock field of a pre-epoch date depends on the difference.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=818A36
    // Broiler-Human:        PENDING
    private static double DateFloorMod(double left, double right) =>
        left - (right * System.Math.Floor(left / right));

    /// <summary>The specification's <c>Day</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8DE6AE
    // Broiler-Human:        PENDING
    private static double DateDay(double time) => System.Math.Floor(time / DateMsPerDay);

    /// <summary>The specification's <c>TimeWithinDay</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F1D0A3
    // Broiler-Human:        PENDING
    private static double DateTimeWithinDay(double time) => DateFloorMod(time, DateMsPerDay);

    /// <summary>The specification's <c>DaysInYear</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EB8CD3
    // Broiler-Human:        PENDING
    private static int DateDaysInYear(double year)
    {
        if (year % 4 != 0)
        {
            return 365;
        }

        if (year % 100 != 0)
        {
            return 366;
        }

        return year % 400 != 0 ? 365 : 366;
    }

    /// <summary>The specification's <c>DayFromYear</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=056ECE
    // Broiler-Human:        PENDING
    private static double DateDayFromYear(double year) =>
        (365 * (year - 1970)) +
        System.Math.Floor((year - 1969) / 4) -
        System.Math.Floor((year - 1901) / 100) +
        System.Math.Floor((year - 1601) / 400);

    /// <summary>The specification's <c>TimeFromYear</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0A3134
    // Broiler-Human:        PENDING
    private static double DateTimeFromYear(double year) => DateMsPerDay * DateDayFromYear(year);

    /// <summary>The specification's <c>YearFromTime</c>.</summary>
    /// <remarks>
    /// The mean Gregorian year puts the estimate within one of the answer for every representable
    /// time value, so the two corrections below run at most twice between them. A search would be
    /// correct too and would cost a hundred times as much on the hot path <c>getFullYear</c> is on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=116739
    // Broiler-Human:        PENDING
    private static double DateYearFromTime(double time)
    {
        var year = System.Math.Floor(time / (DateMsPerDay * 365.2425)) + 1970;

        while (DateTimeFromYear(year) > time)
        {
            year--;
        }

        while (DateTimeFromYear(year + 1) <= time)
        {
            year++;
        }

        return year;
    }

    /// <summary>The specification's <c>InLeapYear</c>, as the zero or one it adds to a month start.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C0673A
    // Broiler-Human:        PENDING
    private static int DateInLeapYear(double time) =>
        DateDaysInYear(DateYearFromTime(time)) == 366 ? 1 : 0;

    /// <summary>The specification's <c>DayWithinYear</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EC48D2
    // Broiler-Human:        PENDING
    private static double DateDayWithinYear(double time) =>
        DateDay(time) - DateDayFromYear(DateYearFromTime(time));

    /// <summary>The day of the year the given month starts on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E2F2FE
    // Broiler-Human:        PENDING
    private static double DateMonthStartDay(int month, int leap) => month switch
    {
        0 => 0,
        1 => 31,
        2 => 59 + leap,
        3 => 90 + leap,
        4 => 120 + leap,
        5 => 151 + leap,
        6 => 181 + leap,
        7 => 212 + leap,
        8 => 243 + leap,
        9 => 273 + leap,
        10 => 304 + leap,
        _ => 334 + leap,
    };

    /// <summary>The specification's <c>MonthFromTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CE1BCC
    // Broiler-Human:        PENDING
    private static double DateMonthFromTime(double time)
    {
        var within = DateDayWithinYear(time);
        var leap = DateInLeapYear(time);

        if (within < 31)
        {
            return 0;
        }

        if (within < 59 + leap)
        {
            return 1;
        }

        if (within < 90 + leap)
        {
            return 2;
        }

        if (within < 120 + leap)
        {
            return 3;
        }

        if (within < 151 + leap)
        {
            return 4;
        }

        if (within < 181 + leap)
        {
            return 5;
        }

        if (within < 212 + leap)
        {
            return 6;
        }

        if (within < 243 + leap)
        {
            return 7;
        }

        if (within < 273 + leap)
        {
            return 8;
        }

        if (within < 304 + leap)
        {
            return 9;
        }

        return within < 334 + leap ? 10 : 11;
    }

    /// <summary>The specification's <c>DateFromTime</c>: the one-based day of the month.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=33FEE9
    // Broiler-Human:        PENDING
    private static double DateDateFromTime(double time) =>
        DateDayWithinYear(time) -
        DateMonthStartDay((int)DateMonthFromTime(time), DateInLeapYear(time)) +
        1;

    /// <summary>The specification's <c>WeekDay</c>: Sunday is zero.</summary>
    /// <remarks>
    /// The <c>+ 4</c> is the epoch's own weekday: 1970-01-01 was a Thursday.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0B9042
    // Broiler-Human:        PENDING
    private static double DateWeekDay(double time) => DateFloorMod(DateDay(time) + 4, 7);

    /// <summary>The specification's <c>HourFromTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=20DB73
    // Broiler-Human:        PENDING
    private static double DateHourFromTime(double time) =>
        DateFloorMod(System.Math.Floor(time / DateMsPerHour), 24);

    /// <summary>The specification's <c>MinFromTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A484A2
    // Broiler-Human:        PENDING
    private static double DateMinFromTime(double time) =>
        DateFloorMod(System.Math.Floor(time / DateMsPerMinute), 60);

    /// <summary>The specification's <c>SecFromTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=608551
    // Broiler-Human:        PENDING
    private static double DateSecFromTime(double time) =>
        DateFloorMod(System.Math.Floor(time / DateMsPerSecond), 60);

    /// <summary>The specification's <c>msFromTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=40A5B0
    // Broiler-Human:        PENDING
    private static double DateMsFromTime(double time) => DateFloorMod(time, DateMsPerSecond);

    /// <summary>The specification's <c>MakeTime</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CC1F15
    // Broiler-Human:        PENDING
    private static double DateMakeTime(double hour, double minute, double second, double milli)
    {
        if (!double.IsFinite(hour) || !double.IsFinite(minute) ||
            !double.IsFinite(second) || !double.IsFinite(milli))
        {
            return double.NaN;
        }

        return (JsValue.ToInteger(hour) * DateMsPerHour) +
            (JsValue.ToInteger(minute) * DateMsPerMinute) +
            (JsValue.ToInteger(second) * DateMsPerSecond) +
            JsValue.ToInteger(milli);
    }

    /// <summary>The specification's <c>MakeDay</c>.</summary>
    /// <remarks>
    /// The fields roll over rather than being validated: month 12 is January of the next year and
    /// day 0 is the last day of the previous month. That is not leniency, it is the operation
    /// <c>setMonth</c> and <c>new Date(y, m, 0)</c> are both defined in terms of.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A9E3FF
    // Broiler-Human:        PENDING
    private static double DateMakeDay(double year, double month, double day)
    {
        if (!double.IsFinite(year) || !double.IsFinite(month) || !double.IsFinite(day))
        {
            return double.NaN;
        }

        var wholeYear = JsValue.ToInteger(year);
        var wholeMonth = JsValue.ToInteger(month);
        var wholeDay = JsValue.ToInteger(day);
        var carried = wholeYear + System.Math.Floor(wholeMonth / 12);

        if (!double.IsFinite(carried))
        {
            return double.NaN;
        }

        var within = DateFloorMod(wholeMonth, 12);
        var leap = DateDaysInYear(carried) == 366 ? 1 : 0;

        return DateDayFromYear(carried) + DateMonthStartDay((int)within, leap) + wholeDay - 1;
    }

    /// <summary>The specification's <c>MakeDate</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=203675
    // Broiler-Human:        PENDING
    private static double DateMakeDate(double day, double time)
    {
        if (!double.IsFinite(day) || !double.IsFinite(time))
        {
            return double.NaN;
        }

        return (day * DateMsPerDay) + time;
    }

    /// <summary>The specification's <c>TimeClip</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=81B8C1
    // Broiler-Human:        PENDING
    private static double DateTimeClip(double time)
    {
        if (!double.IsFinite(time) || System.Math.Abs(time) > DateMaxTimeValue)
        {
            return double.NaN;
        }

        var truncated = System.Math.Truncate(time);

        // The comparison folds a negative zero into a positive one, which is what
        // ToIntegerOrInfinity does and what keeps `new Date(-0.5).getTime()` from printing "-0".
        return truncated == 0 ? 0 : truncated;
    }

    // ---- parsing ---------------------------------------------------------------------------------

    /// <summary>
    /// Parses the Date Time String Format, answering NaN for anything else.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The accepted shapes are <c>YYYY</c>, <c>YYYY-MM</c> and <c>YYYY-MM-DD</c>, each optionally
    /// followed by <c>THH:mm</c>, <c>THH:mm:ss</c> or <c>THH:mm:ss.sss</c>, and that optionally
    /// followed by <c>Z</c> or <c>±HH:mm</c>. Nothing else parses - no leading or trailing space,
    /// no expanded <c>±YYYYYY</c> year, and none of the legacy shapes an engine accepts for
    /// compatibility with the web.
    /// </para>
    /// <para>
    /// A date-only form is UTC and a date-time form without an offset is local time; this profile
    /// fixes local time to UTC, so the two agree and one computation serves both.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BFC295
    // Broiler-Human:        PENDING
    private static double DateParseText(JsEngine engine, string text)
    {
        engine.Charge((ulong)text.Length + 1);

        var at = 0;

        if (!DateReadDigits(text, ref at, 4, out var year))
        {
            return double.NaN;
        }

        double month = 1;
        double day = 1;
        double hour = 0;
        double minute = 0;
        double second = 0;
        double milli = 0;
        var offsetMinutes = 0.0;

        if (at < text.Length && text[at] == '-')
        {
            at++;

            if (!DateReadDigits(text, ref at, 2, out month))
            {
                return double.NaN;
            }

            if (at < text.Length && text[at] == '-')
            {
                at++;

                if (!DateReadDigits(text, ref at, 2, out day))
                {
                    return double.NaN;
                }
            }
        }

        if (at < text.Length && text[at] == 'T')
        {
            at++;

            if (!DateReadDigits(text, ref at, 2, out hour) || at >= text.Length || text[at] != ':')
            {
                return double.NaN;
            }

            at++;

            if (!DateReadDigits(text, ref at, 2, out minute))
            {
                return double.NaN;
            }

            if (at < text.Length && text[at] == ':')
            {
                at++;

                if (!DateReadDigits(text, ref at, 2, out second))
                {
                    return double.NaN;
                }

                if (at < text.Length && text[at] == '.')
                {
                    at++;

                    if (!DateReadDigits(text, ref at, 3, out milli))
                    {
                        return double.NaN;
                    }
                }
            }

            if (at < text.Length && text[at] == 'Z')
            {
                at++;
            }
            else if (at < text.Length && (text[at] == '+' || text[at] == '-'))
            {
                var negative = text[at] == '-';
                at++;

                if (!DateReadDigits(text, ref at, 2, out var offsetHour) ||
                    at >= text.Length || text[at] != ':')
                {
                    return double.NaN;
                }

                at++;

                if (!DateReadDigits(text, ref at, 2, out var offsetMinute) ||
                    offsetHour > 23 || offsetMinute > 59)
                {
                    return double.NaN;
                }

                offsetMinutes = (offsetHour * 60) + offsetMinute;

                if (negative)
                {
                    offsetMinutes = -offsetMinutes;
                }
            }
        }

        if (at != text.Length ||
            month < 1 || month > 12 ||
            day < 1 || day > 31 ||
            minute > 59 || second > 59 ||
            hour > 24 || (hour == 24 && (minute != 0 || second != 0 || milli != 0)))
        {
            return double.NaN;
        }

        var parsed = DateMakeDate(
            DateMakeDay(year, month - 1, day), DateMakeTime(hour, minute, second, milli));

        return DateTimeClip(parsed - (offsetMinutes * DateMsPerMinute));
    }

    /// <summary>Reads exactly <paramref name="count"/> decimal digits, advancing the cursor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F9DF1
    // Broiler-Human:        PENDING
    private static bool DateReadDigits(string text, ref int at, int count, out double value)
    {
        value = 0;

        if (at + count > text.Length)
        {
            return false;
        }

        for (var step = 0; step < count; step++)
        {
            var character = text[at + step];

            if (character is < '0' or > '9')
            {
                return false;
            }

            value = (value * 10) + (character - '0');
        }

        at += count;
        return true;
    }

    // ---- rendering -------------------------------------------------------------------------------

    /// <summary>A non-negative field as a zero-padded decimal, with a sign when it is negative.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A45C3C
    // Broiler-Human:        PENDING
    private static string DatePadded(double value, int width)
    {
        var negative = value < 0;
        var digits = ((long)System.Math.Abs(value)).ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        if (digits.Length < width)
        {
            digits = new string('0', width - digits.Length) + digits;
        }

        return negative ? "-" + digits : digits;
    }

    /// <summary>The three-letter weekday name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A048CC
    // Broiler-Human:        PENDING
    private static string DateWeekDayName(double time)
    {
        var weekDay = (int)DateWeekDay(time);

        return weekDay switch
        {
            0 => "Sun",
            1 => "Mon",
            2 => "Tue",
            3 => "Wed",
            4 => "Thu",
            5 => "Fri",
            _ => "Sat",
        };
    }

    /// <summary>The three-letter month name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CEF1FB
    // Broiler-Human:        PENDING
    private static string DateMonthName(double time)
    {
        var month = (int)DateMonthFromTime(time);

        return month switch
        {
            0 => "Jan",
            1 => "Feb",
            2 => "Mar",
            3 => "Apr",
            4 => "May",
            5 => "Jun",
            6 => "Jul",
            7 => "Aug",
            8 => "Sep",
            9 => "Oct",
            10 => "Nov",
            _ => "Dec",
        };
    }

    /// <summary>The specification's <c>DateString</c>: <c>"Thu Jan 01 1970"</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=19D890
    // Broiler-Human:        PENDING
    private static string DateToCalendarText(double time) =>
        DateWeekDayName(time) + " " + DateMonthName(time) + " " +
        DatePadded(DateDateFromTime(time), 2) + " " + DatePadded(DateYearFromTime(time), 4);

    /// <summary>The clock half of the specification's <c>TimeString</c>: <c>"00:00:00"</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4CE5DE
    // Broiler-Human:        PENDING
    private static string DateToClockText(double time) =>
        DatePadded(DateHourFromTime(time), 2) + ":" +
        DatePadded(DateMinFromTime(time), 2) + ":" +
        DatePadded(DateSecFromTime(time), 2);

    /// <summary>What <c>Date.prototype.toString</c> answers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A1CDA6
    // Broiler-Human:        PENDING
    private static string DateToFullText(double time) =>
        double.IsNaN(time)
            ? DateInvalidText
            : DateToCalendarText(time) + " " + DateToClockText(time) + DateZoneText;

    /// <summary>What <c>Date.prototype.toUTCString</c> answers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B60E8B
    // Broiler-Human:        PENDING
    private static string DateToUtcText(double time) =>
        DateWeekDayName(time) + ", " + DatePadded(DateDateFromTime(time), 2) + " " +
        DateMonthName(time) + " " + DatePadded(DateYearFromTime(time), 4) + " " +
        DateToClockText(time) + " GMT";

    /// <summary>What <c>Date.prototype.toISOString</c> answers.</summary>
    /// <remarks>
    /// A year outside 0000..9999 takes the expanded six-digit form with a mandatory sign, so
    /// <c>new Date(8.64e15).toISOString()</c> is <c>"+275760-09-13T00:00:00.000Z"</c> rather than a
    /// truncation that would parse back as a different year.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D53585
    // Broiler-Human:        PENDING
    private static string DateToIsoText(double time)
    {
        var year = DateYearFromTime(time);

        var yearText = year >= 0 && year <= 9999
            ? DatePadded(year, 4)
            : (year < 0 ? "-" : "+") + DatePadded(System.Math.Abs(year), 6);

        return yearText + "-" +
            DatePadded(DateMonthFromTime(time) + 1, 2) + "-" +
            DatePadded(DateDateFromTime(time), 2) + "T" +
            DatePadded(DateHourFromTime(time), 2) + ":" +
            DatePadded(DateMinFromTime(time), 2) + ":" +
            DatePadded(DateSecFromTime(time), 2) + "." +
            DatePadded(DateMsFromTime(time), 3) + "Z";
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
