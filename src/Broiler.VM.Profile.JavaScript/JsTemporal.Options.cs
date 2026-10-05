// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           3
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The settings GetDifferenceSettings reads (s13.43).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E78ED7
// Broiler-Human:        PENDING
internal readonly record struct JsDifferenceSettings(TemporalUnit SmallestUnit, TemporalUnit LargestUnit, TemporalRounding RoundingMode, long RoundingIncrement);

/// <summary>A ToSecondsStringPrecisionRecord (s13.16): the precision, the unit and the increment.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E9A13A
// Broiler-Human:        PENDING
internal readonly record struct JsPrecision(int Precision, TemporalUnit Unit, long Increment);

/// <summary>
/// Temporal's operations over guest values (phase F8, JSD-0054): reading options, converting
/// arguments, time zones, the ISO 8601 calendar, durations relative to a date, and the objects.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5931F8
    // Broiler-Human:        PENDING
    private static readonly string[] RoundingModeNames =
        ["ceil", "floor", "expand", "trunc", "halfCeil", "halfFloor", "halfExpand", "halfTrunc", "halfEven"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9C56E8
    // Broiler-Human:        PENDING
    private static readonly string[] UnitOptionValues =
    [
        "year", "years", "month", "months", "week", "weeks", "day", "days", "hour", "hours", "minute", "minutes",
        "second", "seconds", "millisecond", "milliseconds", "microsecond", "microseconds", "nanosecond", "nanoseconds", "auto",
    ];

    /// <summary>ECMA-402's GetOptionsObject: undefined as a fresh null-prototype object, an object as itself, anything else a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=876780
    // Broiler-Human:        PENDING
    internal static JsObject OptionsObject(JsEngine engine, JsValue options)
    {
        // AN EMPTY VALUE IS AN ARGUMENT NOT PASSED: the conversions' optional options default to it.
        if (options.Type is JsType.Undefined or JsType.Empty)
        {
            return new JsObject(null);
        }

        if (options.IsObject)
        {
            return options.AsObject();
        }

        throw engine.Error("TypeError", "Temporal: the options argument must be an object or undefined");
    }

    /// <summary>The proposal's GetOption (s14.5.2.1) of type string: the value, the default, or a RangeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CCE4E9
    // Broiler-Human:        PENDING
    internal static string? StringOption(JsEngine engine, JsObject options, string property, string[] values, string? fallback, bool required = false)
    {
        var value = engine.GetProperty(JsValue.Object(options), property);

        if (value.Type == JsType.Undefined)
        {
            if (required)
            {
                throw engine.Error("RangeError", "Temporal: the " + property + " option is required");
            }

            return fallback;
        }

        var text = engine.ToStringValue(value);

        if (System.Array.IndexOf(values, text) < 0)
        {
            throw engine.Error("RangeError", "Temporal: " + text + " is not a valid value for " + property);
        }

        return text;
    }

    /// <summary>The proposal's GetTemporalOverflowOption (s13.6): true for reject.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=737D42
    // Broiler-Human:        PENDING
    internal static bool Reject(JsEngine engine, JsObject options) =>
        StringOption(engine, options, "overflow", ["constrain", "reject"], "constrain") == "reject";

    /// <summary>The proposal's GetTemporalDisambiguationOption (s13.7).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1B6C06
    // Broiler-Human:        PENDING
    internal static string Disambiguation(JsEngine engine, JsObject options) =>
        StringOption(engine, options, "disambiguation", ["compatible", "earlier", "later", "reject"], "compatible")!;

    /// <summary>The proposal's GetTemporalOffsetOption (s13.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2C8018
    // Broiler-Human:        PENDING
    internal static string OffsetOption(JsEngine engine, JsObject options, string fallback) =>
        StringOption(engine, options, "offset", ["prefer", "use", "ignore", "reject"], fallback)!;

    /// <summary>The proposal's GetTemporalShowCalendarNameOption (s13.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=91F4D1
    // Broiler-Human:        PENDING
    internal static string ShowCalendar(JsEngine engine, JsObject options) =>
        StringOption(engine, options, "calendarName", ["auto", "always", "never", "critical"], "auto")!;

    /// <summary>The proposal's GetTemporalShowTimeZoneNameOption (s13.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=50BF33
    // Broiler-Human:        PENDING
    internal static string ShowTimeZone(JsEngine engine, JsObject options) =>
        StringOption(engine, options, "timeZoneName", ["auto", "never", "critical"], "auto")!;

    /// <summary>The proposal's GetTemporalShowOffsetOption (s13.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8CE35F
    // Broiler-Human:        PENDING
    internal static string ShowOffset(JsEngine engine, JsObject options) =>
        StringOption(engine, options, "offset", ["auto", "never"], "auto")!;

    /// <summary>The proposal's GetRoundingModeOption (s14.5.2.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=601A29
    // Broiler-Human:        PENDING
    internal static TemporalRounding RoundingMode(JsEngine engine, JsObject options, TemporalRounding fallback)
    {
        var text = StringOption(engine, options, "roundingMode", RoundingModeNames, RoundingModeNames[(int)fallback])!;
        return (TemporalRounding)System.Array.IndexOf(RoundingModeNames, text);
    }

    /// <summary>The proposal's GetRoundingIncrementOption (s14.5.2.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F85D21
    // Broiler-Human:        PENDING
    internal static long RoundingIncrement(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "roundingIncrement");

        if (value.Type == JsType.Undefined)
        {
            return 1;
        }

        var integer = IntegerWithTruncation(engine, value);

        if (integer < 1 || integer > 1_000_000_000)
        {
            throw engine.Error("RangeError", "Temporal: roundingIncrement must be from 1 to 10^9");
        }

        return (long)integer;
    }

    /// <summary>The proposal's ValidateTemporalRoundingIncrement (s13.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=71B924
    // Broiler-Human:        PENDING
    internal static void ValidateIncrement(JsEngine engine, long increment, long dividend, bool inclusive)
    {
        var maximum = inclusive ? dividend : dividend - 1;

        if (increment > maximum || dividend % increment != 0)
        {
            throw engine.Error("RangeError", "Temporal: roundingIncrement " + increment + " does not divide " + dividend + " evenly");
        }
    }

    /// <summary>The proposal's GetTemporalFractionalSecondDigitsOption (s13.15): -1 for auto.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CD2DE5
    // Broiler-Human:        PENDING
    internal static int FractionalSecondDigits(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "fractionalSecondDigits");

        if (value.Type == JsType.Undefined)
        {
            return JsTemporalCore.PrecisionAuto;
        }

        if (!value.IsNumber)
        {
            if (engine.ToStringValue(value) != "auto")
            {
                throw engine.Error("RangeError", "Temporal: fractionalSecondDigits must be auto or 0 to 9");
            }

            return JsTemporalCore.PrecisionAuto;
        }

        var number = value.AsNumber();

        if (!double.IsFinite(number))
        {
            throw engine.Error("RangeError", "Temporal: fractionalSecondDigits must be auto or 0 to 9");
        }

        var digits = System.Math.Floor(number);

        if (digits < 0 || digits > 9)
        {
            throw engine.Error("RangeError", "Temporal: fractionalSecondDigits must be auto or 0 to 9");
        }

        return (int)digits;
    }

    /// <summary>The proposal's ToSecondsStringPrecisionRecord (s13.16).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7BC57F
    // Broiler-Human:        PENDING
    internal static JsPrecision Precision(TemporalUnit smallestUnit, int digits) => smallestUnit switch
    {
        TemporalUnit.Minute => new JsPrecision(JsTemporalCore.PrecisionMinute, TemporalUnit.Minute, 1),
        TemporalUnit.Second => new JsPrecision(0, TemporalUnit.Second, 1),
        TemporalUnit.Millisecond => new JsPrecision(3, TemporalUnit.Millisecond, 1),
        TemporalUnit.Microsecond => new JsPrecision(6, TemporalUnit.Microsecond, 1),
        TemporalUnit.Nanosecond => new JsPrecision(9, TemporalUnit.Nanosecond, 1),
        _ => digits switch
        {
            JsTemporalCore.PrecisionAuto => new JsPrecision(JsTemporalCore.PrecisionAuto, TemporalUnit.Nanosecond, 1),
            0 => new JsPrecision(0, TemporalUnit.Second, 1),
            <= 3 => new JsPrecision(digits, TemporalUnit.Millisecond, Pow10(3 - digits)),
            <= 6 => new JsPrecision(digits, TemporalUnit.Microsecond, Pow10(6 - digits)),
            _ => new JsPrecision(digits, TemporalUnit.Nanosecond, Pow10(9 - digits)),
        },
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=86E3C0
    // Broiler-Human:        PENDING
    private static long Pow10(int exponent)
    {
        var result = 1L;

        for (var i = 0; i < exponent; i++)
        {
            result *= 10;
        }

        return result;
    }

    /// <summary>The proposal's GetTemporalUnitValuedOption (s13.17): a unit, unset, or auto.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=877EC3
    // Broiler-Human:        PENDING
    internal static TemporalUnit UnitOption(JsEngine engine, JsObject options, string key, bool required)
    {
        var text = StringOption(engine, options, key, UnitOptionValues, null, required);

        if (text is null)
        {
            return TemporalUnit.Unset;
        }

        if (text == "auto")
        {
            return TemporalUnit.Auto;
        }

        var singular = text.EndsWith('s') ? text[..^1] : text;
        return (TemporalUnit)System.Array.IndexOf(JsTemporalCore.UnitNames, singular);
    }

    /// <summary>The unit groups of ValidateTemporalUnitValue (s13.18).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CE76F4
    // Broiler-Human:        PENDING
    internal enum UnitGroup
    {
        Date,
        Time,
        DateTime,
    }

    /// <summary>The proposal's ValidateTemporalUnitValue (s13.18).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=466924
    // Broiler-Human:        PENDING
    internal static void ValidateUnit(JsEngine engine, TemporalUnit unit, UnitGroup group, bool allowAuto = false)
    {
        if (unit == TemporalUnit.Unset || (allowAuto && unit == TemporalUnit.Auto))
        {
            return;
        }

        if (unit == TemporalUnit.Auto)
        {
            throw engine.Error("RangeError", "Temporal: auto is not a valid unit here");
        }

        var isDate = JsTemporalCore.IsDateUnit(unit);

        if ((isDate && group != UnitGroup.Time) || (!isDate && group != UnitGroup.Date))
        {
            return;
        }

        throw engine.Error("RangeError", "Temporal: " + JsTemporalCore.UnitNames[(int)unit] + " is not a valid unit here");
    }

    /// <summary>The proposal's GetDifferenceSettings (s13.43).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=43B6B4
    // Broiler-Human:        PENDING
    internal static JsDifferenceSettings DifferenceSettings(
        JsEngine engine,
        bool since,
        JsObject options,
        UnitGroup group,
        TemporalUnit[] disallowed,
        TemporalUnit fallbackSmallest,
        TemporalUnit smallestLargestDefault)
    {
        var largest = UnitOption(engine, options, "largestUnit", required: false);
        var increment = RoundingIncrement(engine, options);
        var mode = RoundingMode(engine, options, TemporalRounding.Trunc);
        var smallest = UnitOption(engine, options, "smallestUnit", required: false);

        ValidateUnit(engine, largest, group, allowAuto: true);

        if (largest == TemporalUnit.Unset)
        {
            largest = TemporalUnit.Auto;
        }

        if (System.Array.IndexOf(disallowed, largest) >= 0)
        {
            throw engine.Error("RangeError", "Temporal: largestUnit " + JsTemporalCore.UnitNames[(int)largest] + " is not allowed here");
        }

        ValidateUnit(engine, smallest, group);

        if (smallest == TemporalUnit.Unset)
        {
            smallest = fallbackSmallest;
        }

        if (System.Array.IndexOf(disallowed, smallest) >= 0)
        {
            throw engine.Error("RangeError", "Temporal: smallestUnit " + JsTemporalCore.UnitNames[(int)smallest] + " is not allowed here");
        }

        var defaultLargest = JsTemporalCore.Larger(smallestLargestDefault, smallest);

        if (largest == TemporalUnit.Auto)
        {
            largest = defaultLargest;
        }

        if (JsTemporalCore.Larger(largest, smallest) != largest)
        {
            throw engine.Error("RangeError", "Temporal: largestUnit must be at least as large as smallestUnit");
        }

        var maximum = JsTemporalCore.MaximumIncrement(smallest);

        if (maximum != 0)
        {
            ValidateIncrement(engine, increment, maximum, inclusive: false);
        }

        return new JsDifferenceSettings(smallest, largest, since ? JsTemporalCore.Negate(mode) : mode, increment);
    }

    // ---- Number conversions -----------------------------------------------------------------

    /// <summary>The proposal's ToIntegerWithTruncation (s13.40).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D386A
    // Broiler-Human:        PENDING
    internal static double IntegerWithTruncation(JsEngine engine, JsValue argument)
    {
        var number = engine.ToNumber(argument);

        if (!double.IsFinite(number))
        {
            throw engine.Error("RangeError", "Temporal: " + number + " is not a finite number");
        }

        return System.Math.Truncate(number) + 0.0;
    }

    /// <summary>The proposal's ToPositiveIntegerWithTruncation (s13.39).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=92BDED
    // Broiler-Human:        PENDING
    internal static double PositiveIntegerWithTruncation(JsEngine engine, JsValue argument)
    {
        var integer = IntegerWithTruncation(engine, argument);

        if (integer <= 0)
        {
            throw engine.Error("RangeError", "Temporal: " + integer + " is not a positive integer");
        }

        return integer;
    }

    /// <summary>The proposal's ToIntegerIfIntegral (s14.5.1.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55AD9C
    // Broiler-Human:        PENDING
    internal static double IntegerIfIntegral(JsEngine engine, JsValue argument)
    {
        var number = engine.ToNumber(argument);

        if (!double.IsFinite(number) || System.Math.Truncate(number) != number)
        {
            throw engine.Error("RangeError", "Temporal: " + number + " is not an integer");
        }

        return number + 0.0;
    }

    /// <summary>The proposal's ToOffsetString (s13.41).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1A8593
    // Broiler-Human:        PENDING
    internal static string OffsetString(JsEngine engine, JsValue argument)
    {
        var primitive = engine.ToPrimitive(argument, "string");

        if (!primitive.IsString)
        {
            throw engine.Error("TypeError", "Temporal: an offset must be a string");
        }

        var text = primitive.AsString();

        if (!JsTemporalParser.IsUtcOffset(text, subMinute: true))
        {
            throw engine.Error("RangeError", "Temporal: " + text + " is not a UTC offset");
        }

        return text;
    }
}
