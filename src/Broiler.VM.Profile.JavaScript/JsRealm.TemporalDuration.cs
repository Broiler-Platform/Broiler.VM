// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Temporal.Duration</c> (Temporal s7; JSD-0054): the reads here, the arithmetic the shared
/// library's <see cref="DurationMath"/> since SJL-10.4 (JSD-0078).
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34360F
    // Broiler-Human:        PENDING
    private static readonly string[] DurationFieldNames =
        ["years", "months", "weeks", "days", "hours", "minutes", "seconds", "milliseconds", "microseconds", "nanoseconds"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55B601
    // Broiler-Human:        PENDING
    private void SetupTemporalDuration(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalDurationPrototype = prototype;

        var constructor = TemporalConstructor(temporal, "Duration", 0, prototype, static (engine, newTarget, arguments) =>
        {
            var fields = new double[10];

            for (var i = 0; i < 10; i++)
            {
                var argument = TemporalArgument(arguments, i);
                fields[i] = argument.Type == JsType.Undefined ? 0 : JsTemporal.IntegerIfIntegral(engine, argument);
            }

            return JsValue.Object(JsTemporal.CreateDuration(engine, JsTemporal.FieldsOf(fields), newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreateDuration(engine, JsTemporal.ToDuration(engine, TemporalArgument(arguments, 0)))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
            JsValue.Number(DurationCompare(engine, arguments)));

        for (var i = 0; i < DurationFieldNames.Length; i++)
        {
            var index = i;
            TemporalGetter(prototype, DurationFieldNames[i], (engine, thisValue, arguments) =>
                JsValue.Number(JsTemporal.FieldsArray(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", DurationFieldNames[index]).Fields)[index]));
        }

        TemporalGetter(prototype, "sign", static (engine, thisValue, arguments) =>
            JsValue.Number(DurationMath.Sign(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "sign").Fields)));

        TemporalGetter(prototype, "blank", static (engine, thisValue, arguments) =>
            JsValue.Boolean(DurationMath.Sign(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "blank").Fields) == 0));

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "with");
            var partial = JsTemporal.ToPartialDuration(engine, TemporalArgument(arguments, 0));
            var existing = JsTemporal.FieldsArray(duration.Fields);
            var fields = new double[10];

            for (var i = 0; i < 10; i++)
            {
                fields[i] = double.IsNaN(partial[i]) ? existing[i] : partial[i];
            }

            return JsValue.Object(JsTemporal.CreateDuration(engine, JsTemporal.FieldsOf(fields)));
        });

        Method(prototype, "negated", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "negated");
            return JsValue.Object(JsTemporal.CreateDuration(engine, DurationMath.Negate(duration.Fields)));
        });

        Method(prototype, "abs", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "abs");
            return JsValue.Object(JsTemporal.CreateDuration(engine, DurationMath.Abs(duration.Fields)));
        });

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurations(engine, TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "add"), TemporalArgument(arguments, 0), subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurations(engine, TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "subtract"), TemporalArgument(arguments, 0), subtract: true)));

        Method(prototype, "round", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DurationRound(engine, TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "round"), TemporalArgument(arguments, 0))));

        Method(prototype, "total", 1, static (engine, thisValue, arguments) =>
            JsValue.Number(DurationTotal(engine, TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "total"), TemporalArgument(arguments, 0))));

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var mode = JsTemporal.RoundingModeOption(engine, options, TemporalOptionRules.DefaultToStringRoundingMode);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);

            if (!TemporalOptionRules.IsValidToStringSmallestUnit(smallest, duration: true))
            {
                throw engine.Error("RangeError", "Temporal.Duration.prototype.toString: smallestUnit must be a second or smaller");
            }

            var precision = TemporalOptionRules.ToSecondsStringPrecisionRecord(smallest, digits);
            JsTemporal.Check(engine, TemporalFormat.TryDurationToString(duration.Fields, precision, mode, out var text), "the rounded duration");
            return JsValue.String(text!);
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
            JsValue.String(TemporalFormat.DurationToString(
                TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "toJSON").Fields,
                TemporalFormat.AutoPrecision)));

        // ECMA-402'S DEFINITION (s15.11.1.1): the duration written by an Intl.DurationFormat.
        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "toLocaleString");
            var format = (JsDurationFormatObject)NewDurationFormat(
                engine,
                JsValue.Object(engine.Realm.DurationFormatConstructor!),
                [TemporalArgument(arguments, 0), TemporalArgument(arguments, 1)]).AsObject();
            var text = new System.Text.StringBuilder();

            foreach (var part in PartitionDurationFormatPattern(engine, format, JsTemporal.FieldsArray(duration.Fields)))
            {
                text.Append(part.Value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "Duration"));
    }

    /// <summary>Temporal.Duration.compare (s7.2.3): the reads here, the comparison the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0C38B3
    // Broiler-Human:        PENDING
    private static double DurationCompare(JsEngine engine, JsValue[] arguments)
    {
        var one = JsTemporal.ToDuration(engine, TemporalArgument(arguments, 0));
        var two = JsTemporal.ToDuration(engine, TemporalArgument(arguments, 1));
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 2));
        var relativeTo = JsTemporal.RelativeToOption(engine, options);
        var status = DurationMath.Compare(one, two, relativeTo, out var result);

        if (status == TemporalStatus.RelativeToRequired)
        {
            throw engine.Error("RangeError", "Temporal.Duration.compare: comparing years, months or weeks needs a relativeTo date");
        }

        JsTemporal.Check(engine, status, "the result", "the time duration is too large");
        return result;
    }

    /// <summary>The proposal's AddDurations (s7.5.41): ToTemporalDuration here, the addition the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=54F54A
    // Broiler-Human:        PENDING
    private static JsDurationObject AddDurations(JsEngine engine, JsDurationObject duration, JsValue other, bool subtract)
    {
        var otherFields = JsTemporal.ToDuration(engine, other);

        if (subtract)
        {
            otherFields = DurationMath.Negate(otherFields);
        }

        var status = DurationMath.AddDurations(duration.Fields, otherFields, out var result);

        if (status == TemporalStatus.RelativeToRequired)
        {
            throw engine.Error("RangeError", "Temporal.Duration: adding years, months or weeks needs a relative date; use add on a date instead");
        }

        JsTemporal.Check(engine, status, "the duration", "the time duration is too large");
        return JsTemporal.CreateDuration(engine, result);
    }

    /// <summary>A roundTo or totalOf argument: a string names the one option, an object is the options.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=26F7F7
    // Broiler-Human:        PENDING
    private static JsObject StringOrOptions(JsEngine engine, JsValue value, string key, string member)
    {
        if (value.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Temporal.Duration.prototype." + member + ": an argument is required");
        }

        if (value.IsString)
        {
            var options = new JsObject(null);
            options.DefineOrdinary(key, value);
            return options;
        }

        return JsTemporal.OptionsObject(engine, value);
    }

    /// <summary>
    /// Temporal.Duration.prototype.round (s7.3.20): the five reads in their order here, the units'
    /// rules and the rounding the shared library's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=91D3E1
    // Broiler-Human:        PENDING
    private static JsDurationObject DurationRound(JsEngine engine, JsDurationObject duration, JsValue argument)
    {
        var roundTo = StringOrOptions(engine, argument, "smallestUnit", "round");
        var largest = JsTemporal.UnitOption(engine, roundTo, "largestUnit", required: false);
        var relativeTo = JsTemporal.RelativeToOption(engine, roundTo);
        var increment = JsTemporal.RoundingIncrement(engine, roundTo);
        var mode = JsTemporal.RoundingModeOption(engine, roundTo, TemporalOptionRules.DefaultRoundRoundingMode);
        var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: false);

        if (!TemporalOptionRules.TryGetDurationRoundUnits(duration.Fields, largest, increment, smallest, out var resolvedLargest, out var resolvedSmallest))
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: smallestUnit, largestUnit and roundingIncrement are not valid together");
        }

        var status = DurationMath.Round(duration.Fields, resolvedLargest, increment, resolvedSmallest, mode, relativeTo, out var result);

        if (status == TemporalStatus.RelativeToRequired)
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: rounding years, months or weeks needs a relativeTo date");
        }

        JsTemporal.Check(engine, status, "the date-time", "the rounded time duration is too large");
        return JsTemporal.CreateDuration(engine, result);
    }

    /// <summary>Temporal.Duration.prototype.total (s7.3.21): the reads here, the total the shared library's, an exact ratio rounded once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=630B03
    // Broiler-Human:        PENDING
    private static double DurationTotal(JsEngine engine, JsDurationObject duration, JsValue argument)
    {
        var totalOf = StringOrOptions(engine, argument, "unit", "total");
        var relativeTo = JsTemporal.RelativeToOption(engine, totalOf);
        var unit = JsTemporal.UnitOption(engine, totalOf, "unit", required: true);

        if (!TemporalOptionRules.IsValidTotalUnit(unit))
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.total: auto is not a valid unit here");
        }

        var status = DurationMath.Total(duration.Fields, unit, relativeTo, out var total);

        if (status == TemporalStatus.RelativeToRequired)
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.total: a total in years, months or weeks needs a relativeTo date");
        }

        JsTemporal.Check(engine, status, "the date-time");
        return total;
    }
}
