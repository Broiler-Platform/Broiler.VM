// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           0
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       29
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Temporal's operations over guest values (phase F8, JSD-0054): reading options, converting
/// arguments, time zones, calendars, durations relative to a date, and the objects.
/// </summary>
/// <remarks>
/// <para>
/// <b>The arithmetic is the shared library's since SJL-10.4</b> (JSD-0078, JSD-0079):
/// <c>Broiler.ECMAScript.Algorithms</c>' <c>Broiler.ECMAScript.Temporal</c> holds the records and
/// limits, balancing, the grammar, serialization, the calendars, durations and differences, and the
/// rules every option is held to. What stays here is the engine's: the objects and their slots, each
/// option read with <c>Get</c> and converted in the specification's order, the conversion of
/// arguments, <c>relativeTo</c> and the zones resolved, and each status the library answers mapped to
/// the error the specification throws, with this family's message (JSD-0054 section 5).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
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

    /// <summary>
    /// The proposal's GetOption (s14.5.2.1) of type string over a list of values: the value, the
    /// default, or a RangeError. The options the shared library has no rule for read through it:
    /// <c>disambiguation</c> and <c>offset</c>'s zone policy, which are the zones' (SJL-8), and
    /// <c>direction</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5789FA
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
            throw OptionError(engine, property, text);
        }

        return text;
    }

    /// <summary>
    /// GetOption's read of a string option (s14.5.2.1 steps 1 to 3): the value converted with
    /// ToString, or null where it is undefined. The rule it is then held to is the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75998C
    // Broiler-Human:        PENDING
    private static string? ReadStringOption(JsEngine engine, JsObject options, string property)
    {
        var value = engine.GetProperty(JsValue.Object(options), property);
        return value.Type == JsType.Undefined ? null : engine.ToStringValue(value);
    }

    /// <summary>The RangeError of an option value outside its rule's values.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7FA8E4
    // Broiler-Human:        PENDING
    private static JsThrow OptionError(JsEngine engine, string property, string text) =>
        engine.Error("RangeError", "Temporal: " + text + " is not a valid value for " + property);

    /// <summary>The proposal's GetTemporalOverflowOption (s13.6), under the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6559D4
    // Broiler-Human:        PENDING
    internal static TemporalOverflow Overflow(JsEngine engine, JsObject options)
    {
        var text = ReadStringOption(engine, options, "overflow");

        if (text is null)
        {
            return TemporalOptionRules.DefaultOverflow;
        }

        return TemporalOptionRules.TryParseOverflow(text, out var overflow) ? overflow : throw OptionError(engine, "overflow", text);
    }

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

    /// <summary>The proposal's GetTemporalShowCalendarNameOption (s13.10), under the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1F5D2
    // Broiler-Human:        PENDING
    internal static ShowCalendarName ShowCalendarOption(JsEngine engine, JsObject options)
    {
        var text = ReadStringOption(engine, options, "calendarName");

        if (text is null)
        {
            return ShowCalendarName.Auto;
        }

        return TemporalOptionRules.TryParseShowCalendarName(text, out var show) ? show : throw OptionError(engine, "calendarName", text);
    }

    /// <summary>The proposal's GetTemporalShowTimeZoneNameOption (s13.11), under the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B4EE59
    // Broiler-Human:        PENDING
    internal static ShowTimeZoneName ShowTimeZoneOption(JsEngine engine, JsObject options)
    {
        var text = ReadStringOption(engine, options, "timeZoneName");

        if (text is null)
        {
            return ShowTimeZoneName.Auto;
        }

        return TemporalOptionRules.TryParseShowTimeZoneName(text, out var show) ? show : throw OptionError(engine, "timeZoneName", text);
    }

    /// <summary>The proposal's GetTemporalShowOffsetOption (s13.12), under the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=939122
    // Broiler-Human:        PENDING
    internal static ShowOffset ShowOffsetOption(JsEngine engine, JsObject options)
    {
        var text = ReadStringOption(engine, options, "offset");

        if (text is null)
        {
            return ShowOffset.Auto;
        }

        return TemporalOptionRules.TryParseShowOffset(text, out var show) ? show : throw OptionError(engine, "offset", text);
    }

    /// <summary>The proposal's GetRoundingModeOption (s14.5.2.2): the names are the shared library's <see cref="ExactRounding.TryParseRoundingMode"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=502902
    // Broiler-Human:        PENDING
    internal static RoundingMode RoundingModeOption(JsEngine engine, JsObject options, RoundingMode fallback)
    {
        var text = ReadStringOption(engine, options, "roundingMode");

        if (text is null)
        {
            return fallback;
        }

        return ExactRounding.TryParseRoundingMode(text, out var mode) ? mode : throw OptionError(engine, "roundingMode", text);
    }

    /// <summary>The proposal's GetRoundingIncrementOption (s14.5.2.3): ToNumber here, the rule the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3EB5E
    // Broiler-Human:        PENDING
    internal static long RoundingIncrement(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "roundingIncrement");

        if (value.Type == JsType.Undefined)
        {
            return TemporalOptionRules.DefaultRoundingIncrement;
        }

        return TemporalOptionRules.TryParseRoundingIncrement(engine.ToNumber(value), out var increment)
            ? increment
            : throw engine.Error("RangeError", "Temporal: roundingIncrement must be from 1 to 10^9");
    }

    /// <summary>The proposal's ValidateTemporalRoundingIncrement (s13.14), the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CEC549
    // Broiler-Human:        PENDING
    internal static void ValidateIncrement(JsEngine engine, long increment, long dividend, bool inclusive)
    {
        if (!TemporalOptionRules.IsValidRoundingIncrement(increment, dividend, inclusive))
        {
            throw engine.Error("RangeError", "Temporal: roundingIncrement " + increment + " does not divide " + dividend + " evenly");
        }
    }

    /// <summary>The proposal's GetTemporalFractionalSecondDigitsOption (s13.15): <see cref="TemporalFormat.AutoPrecision"/> for auto.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED1D4E
    // Broiler-Human:        PENDING
    internal static int FractionalSecondDigits(JsEngine engine, JsObject options)
    {
        var value = engine.GetProperty(JsValue.Object(options), "fractionalSecondDigits");

        if (value.Type == JsType.Undefined)
        {
            return TemporalFormat.AutoPrecision;
        }

        if (!value.IsNumber)
        {
            return TemporalOptionRules.IsFractionalSecondDigitsAuto(engine.ToStringValue(value))
                ? TemporalFormat.AutoPrecision
                : throw engine.Error("RangeError", "Temporal: fractionalSecondDigits must be auto or 0 to 9");
        }

        return TemporalOptionRules.TryParseFractionalSecondDigits(value.AsNumber(), out var digits)
            ? digits
            : throw engine.Error("RangeError", "Temporal: fractionalSecondDigits must be auto or 0 to 9");
    }

    /// <summary>
    /// The proposal's GetTemporalUnitValuedOption (s13.17): a unit, unset, or auto, the names the
    /// shared library's rule.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B2853B
    // Broiler-Human:        PENDING
    internal static TemporalUnit UnitOption(JsEngine engine, JsObject options, string key, bool required)
    {
        var text = ReadStringOption(engine, options, key);

        if (text is null)
        {
            return required ? throw engine.Error("RangeError", "Temporal: the " + key + " option is required") : TemporalUnit.Unset;
        }

        return TemporalOptionRules.TryParseUnit(text, out var unit) ? unit : throw OptionError(engine, key, text);
    }

    /// <summary>The proposal's ValidateTemporalUnitValue (s13.18), the shared library's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0556E3
    // Broiler-Human:        PENDING
    internal static void ValidateUnit(JsEngine engine, TemporalUnit unit, TemporalUnitGroup group, bool allowAuto = false)
    {
        if (!TemporalOptionRules.IsValidUnit(unit, group, allowAuto))
        {
            throw engine.Error("RangeError", "Temporal: " + UnitName(unit) + " is not a valid unit here");
        }
    }

    /// <summary>A unit's singular name (Table 21), or <c>auto</c>, for a message.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=109832
    // Broiler-Human:        PENDING
    private static string UnitName(TemporalUnit unit) =>
        unit == TemporalUnit.Auto ? "auto" : System.Enum.GetName(unit)!.ToLowerInvariant();

    /// <summary>
    /// The proposal's GetDifferenceSettings (s13.43): the four reads in its order - largestUnit,
    /// roundingIncrement, roundingMode, smallestUnit - each held to its rule as it is read, then the
    /// type's rules of the units and the increment, which are the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D8A57F
    // Broiler-Human:        PENDING
    internal static DifferenceSettings DifferenceSettingsOf(JsEngine engine, DifferenceOperation operation, JsObject options, TemporalType type)
    {
        var largest = UnitOption(engine, options, "largestUnit", required: false);
        var increment = RoundingIncrement(engine, options);
        var mode = RoundingModeOption(engine, options, TemporalOptionRules.DefaultDifferenceRoundingMode);
        var smallest = UnitOption(engine, options, "smallestUnit", required: false);

        return TemporalOptionRules.TryGetDifferenceSettings(type, operation, largest, increment, mode, smallest, out var settings)
            ? settings
            : throw engine.Error("RangeError", "Temporal: largestUnit, smallestUnit and roundingIncrement are not valid together for this difference");
    }

    // ---- Statuses as errors ----------------------------------------------------------------------

    /// <summary>
    /// The error a status of the shared library's Temporal core is: every one is the specification's
    /// RangeError (JSD-0078's X3), with this family's message for what it refused.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=436EB2
    // Broiler-Human:        PENDING
    internal static JsThrow StatusError(JsEngine engine, TemporalStatus status, string what, string? invalidDuration = null) => status switch
    {
        TemporalStatus.InvalidDate => engine.Error("RangeError", "Temporal: " + what + " is not a valid date"),
        TemporalStatus.InvalidTime => engine.Error("RangeError", "Temporal: " + what + " is not valid"),
        TemporalStatus.InvalidDuration => engine.Error("RangeError", "Temporal: " + (invalidDuration ?? "the duration is not valid: mixed signs, or too large")),
        TemporalStatus.RelativeToRequired => engine.Error("RangeError", "Temporal: years, months or weeks need a relativeTo date"),
        _ => engine.Error("RangeError", "Temporal: " + what + " is outside the representable range"),
    };

    /// <summary>
    /// Throws the status's error unless it is completion; <paramref name="invalidDuration"/>, where
    /// given, is the message of an invalid duration, which this family named by where it arose.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=683C8E
    // Broiler-Human:        PENDING
    internal static void Check(JsEngine engine, TemporalStatus status, string what, string? invalidDuration = null)
    {
        if (status != TemporalStatus.Completed)
        {
            throw StatusError(engine, status, what, invalidDuration);
        }
    }

    /// <summary>
    /// The error a status of the shared calendars is: a missing field the specification's TypeError,
    /// every other its RangeError, with this family's message (the adapter guide's table).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C0EEDC
    // Broiler-Human:        PENDING
    internal static JsThrow CalendarError(JsEngine engine, CalendarStatus status, string calendar, string outOfRange = "the date") => status switch
    {
        CalendarStatus.UnknownCalendar => engine.Error("RangeError", "Temporal: the calendar " + calendar + " is not supported"),
        CalendarStatus.InvalidMonthCode => engine.Error("RangeError", "Temporal: the " + calendar + " calendar has no such month code"),
        CalendarStatus.MissingYear => engine.Error(
            "TypeError", TemporalCalendars.Canonicalize(calendar, out var id) == CalendarStatus.Completed && TemporalCalendars.SupportsEra(id)
                ? "Temporal: the year property, or era and eraYear, is required"
                : "Temporal: the year property is required"),
        CalendarStatus.MissingMonth => engine.Error("TypeError", "Temporal: the month or monthCode property is required"),
        CalendarStatus.MissingDay => engine.Error("TypeError", "Temporal: the day property is required"),
        CalendarStatus.IncompleteEra => engine.Error("TypeError", "Temporal: era and eraYear must be given together"),
        CalendarStatus.UnknownEra => engine.Error("RangeError", "Temporal: the era is not an era of the " + calendar + " calendar"),
        CalendarStatus.EraYearMismatch => engine.Error("RangeError", "Temporal: the year and the era year disagree"),
        CalendarStatus.MonthCodeMismatch => engine.Error("RangeError", "Temporal: month and monthCode disagree"),
        CalendarStatus.MonthNotInYear => engine.Error("RangeError", "Temporal: the year of the " + calendar + " calendar has no such month"),
        CalendarStatus.FieldOutOfRange => engine.Error("RangeError", "Temporal: the month or day is out of range in the " + calendar + " calendar"),
        CalendarStatus.NoReferenceDate => engine.Error("RangeError", "Temporal: no year of the " + calendar + " calendar has that month and day"),
        _ => engine.Error("RangeError", "Temporal: " + outOfRange + " is outside the representable range"),
    };

    /// <summary>
    /// Throws the calendar status's error unless it is completion; <paramref name="outOfRange"/> names
    /// what a result outside the limits is, as this family named it at each operation.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=88B7CE
    // Broiler-Human:        PENDING
    internal static void Check(JsEngine engine, CalendarStatus status, string calendar, string outOfRange = "the date")
    {
        if (status != CalendarStatus.Completed)
        {
            throw CalendarError(engine, status, calendar, outOfRange);
        }
    }

    // ---- Number conversions -----------------------------------------------------------------

    /// <summary>The proposal's ToIntegerWithTruncation (s13.40).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C2D905
    // Broiler-Human:        PENDING
    internal static double IntegerWithTruncation(JsEngine engine, JsValue argument)
    {
        var number = engine.ToNumber(argument);

        if (!double.IsFinite(number))
        {
            throw engine.Error("RangeError", "Temporal: " + number + " is not a finite number");
        }

        // The truncation of a finite Number is the shared library's ToIntegerOrInfinity (SJL-4.6), which
        // also answers +0 for a negative zero.
        return IntegerConversions.ToIntegerOrInfinity(number);
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

    /// <summary>
    /// A year a conversion gave, narrowed to the shared records' <see cref="long"/>: a Number past
    /// <see cref="long"/>'s range saturates, and is outside every limit either way (the adapter
    /// guide's records and limits, step 1).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AF8114
    // Broiler-Human:        PENDING
    internal static long Year(double year) => (long)year;

    /// <summary>
    /// The proposal's ToOffsetString (s13.41): ToPrimitive and the TypeError here, the test the shared
    /// grammar's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8D86EB
    // Broiler-Human:        PENDING
    internal static string OffsetString(JsEngine engine, JsValue argument)
    {
        var primitive = engine.ToPrimitive(argument, "string");

        if (!primitive.IsString)
        {
            throw engine.Error("TypeError", "Temporal: an offset must be a string");
        }

        var text = primitive.AsString();

        if (!TemporalGrammar.TryParseOffsetString(text, out _))
        {
            throw engine.Error("RangeError", "Temporal: " + text + " is not a UTC offset");
        }

        return text;
    }

    /// <summary>The nanoseconds of an offset string <see cref="OffsetString"/> accepted: ParseDateTimeUTCOffset (s14.6.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5FED03
    // Broiler-Human:        PENDING
    internal static long OffsetNanoseconds(string offset) =>
        TemporalGrammar.TryParseOffsetString(offset, out var nanoseconds)
            ? nanoseconds
            : throw new System.InvalidOperationException("The offset string was accepted before it was read.");
}
