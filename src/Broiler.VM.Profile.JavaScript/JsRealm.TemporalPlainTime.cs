// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.PlainTime</c> (Temporal s4; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9797EC
    // Broiler-Human:        PENDING
    private static readonly string[] TimeFieldNames = ["hour", "minute", "second", "millisecond", "microsecond", "nanosecond"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=650CAB
    // Broiler-Human:        PENDING
    private void SetupTemporalPlainTime(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalPlainTimePrototype = prototype;

        var constructor = TemporalConstructor(temporal, "PlainTime", 0, prototype, static (engine, newTarget, arguments) =>
        {
            var values = new double[6];

            for (var i = 0; i < 6; i++)
            {
                var argument = TemporalArgument(arguments, i);
                values[i] = argument.Type == JsType.Undefined ? 0 : JsTemporal.IntegerWithTruncation(engine, argument);
            }

            if (!JsTemporalCore.IsValidTime(values[0], values[1], values[2], values[3], values[4], values[5]))
            {
                throw engine.Error("RangeError", "Temporal.PlainTime: the time is not valid");
            }

            var time = new JsTimeRecord(0, (int)values[0], (int)values[1], (int)values[2], (int)values[3], (int)values[4], (int)values[5]);
            return JsValue.Object(JsTemporal.CreatePlainTime(engine, time, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreatePlainTime(engine, JsTemporal.ToPlainTime(engine, TemporalArgument(arguments, 0), TemporalArgument(arguments, 1)))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToPlainTime(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToPlainTime(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(JsTemporalCore.Compare(one, two));
        });

        TemporalTimeGetters(prototype, static (engine, thisValue, member) =>
            TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", member).Time);

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToTime(engine, TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "add"), TemporalArgument(arguments, 0), subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToTime(engine, TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "subtract"), TemporalArgument(arguments, 0), subtract: true)));

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var plainTime = TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "with");
            var partial = JsTemporal.ToTimeRecord(engine, JsValue.Object(PartialTemporalObject(engine, TemporalArgument(arguments, 0), "PlainTime")), partial: true);
            var time = plainTime.Time;
            var reject = JsTemporal.Reject(engine, JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1)));
            var result = JsTemporal.RegulateTime(
                engine,
                partial[0] ?? time.Hour,
                partial[1] ?? time.Minute,
                partial[2] ?? time.Second,
                partial[3] ?? time.Millisecond,
                partial[4] ?? time.Microsecond,
                partial[5] ?? time.Nanosecond,
                reject);
            return JsValue.Object(JsTemporal.CreatePlainTime(engine, result));
        });

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainTime(engine, TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalPlainTime(engine, TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "since"), arguments, since: true)));

        Method(prototype, "round", 1, static (engine, thisValue, arguments) =>
        {
            var plainTime = TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "round");
            var roundTo = TemporalRoundTo(engine, TemporalArgument(arguments, 0), "PlainTime");
            var increment = JsTemporal.RoundingIncrement(engine, roundTo);
            var mode = JsTemporal.RoundingMode(engine, roundTo, TemporalRounding.HalfExpand);
            var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: true);
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);
            JsTemporal.ValidateIncrement(engine, increment, JsTemporalCore.MaximumIncrement(smallest), inclusive: false);
            return JsValue.Object(JsTemporal.CreatePlainTime(engine, JsTemporalCore.RoundTime(plainTime.Time, increment, smallest, mode)));
        });

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var plainTime = TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "equals");
            return JsValue.Boolean(JsTemporalCore.Compare(plainTime.Time, JsTemporal.ToPlainTime(engine, TemporalArgument(arguments, 0))) == 0);
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var plainTime = TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var mode = JsTemporal.RoundingMode(engine, options, TemporalRounding.Trunc);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);

            if (smallest == TemporalUnit.Hour)
            {
                throw engine.Error("RangeError", "Temporal.PlainTime.prototype.toString: smallestUnit cannot be hour");
            }

            var precision = JsTemporal.Precision(smallest, digits);
            var rounded = JsTemporalCore.RoundTime(plainTime.Time, precision.Increment, precision.Unit, mode);
            return JsValue.String(JsTemporalCore.TimeToString(rounded, precision.Precision));
        });

        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "toLocaleString");
            return JsValue.String(TemporalToLocaleString(engine, thisValue, arguments, "time", "time"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
            JsValue.String(JsTemporalCore.TimeToString(
                TemporalThis<JsPlainTimeObject>(engine, thisValue, "PlainTime", "toJSON").Time, JsTemporalCore.PrecisionAuto)));

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "PlainTime"));
    }

    /// <summary>Reads a Temporal receiver's wall-clock time, or throws its TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=020B6B
    // Broiler-Human:        PENDING
    private delegate JsTimeRecord TemporalTimeReceiver(JsEngine engine, JsValue thisValue, string member);

    /// <summary>Defines the six time getters the time-bearing types share.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=658363
    // Broiler-Human:        PENDING
    private void TemporalTimeGetters(JsObject prototype, TemporalTimeReceiver receiver)
    {
        foreach (var name in TimeFieldNames)
        {
            var member = name;
            TemporalGetter(prototype, member, (engine, thisValue, arguments) =>
            {
                var time = receiver(engine, thisValue, member);
                return JsValue.Number(member switch
                {
                    "hour" => time.Hour,
                    "minute" => time.Minute,
                    "second" => time.Second,
                    "millisecond" => time.Millisecond,
                    "microsecond" => time.Microsecond,
                    _ => time.Nanosecond,
                });
            });
        }
    }

    /// <summary>The proposal's AddDurationToTime (s4.5.18).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C0E506
    // Broiler-Human:        PENDING
    private static JsPlainTimeObject AddDurationToTime(JsEngine engine, JsPlainTimeObject plainTime, JsValue durationLike, bool subtract)
    {
        var fields = SignedDuration(engine, durationLike, subtract);
        var internalDuration = JsTemporalCore.ToInternal(fields);
        return JsTemporal.CreatePlainTime(engine, JsTemporalCore.AddTime(plainTime.Time, internalDuration.Time));
    }

    /// <summary>The proposal's DifferenceTemporalPlainTime (s4.5.17).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F31BE1
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalPlainTime(JsEngine engine, JsPlainTimeObject plainTime, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToPlainTime(engine, TemporalArgument(arguments, 0));
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var settings = JsTemporal.DifferenceSettings(engine, since, options, JsTemporal.UnitGroup.Time, [], TemporalUnit.Nanosecond, TemporalUnit.Hour);
        var time = JsTemporalCore.DifferenceTime(plainTime.Time, other);
        time = JsTemporal.RoundTimeDuration(engine, time, settings.RoundingIncrement, settings.SmallestUnit, settings.RoundingMode);
        var result = JsTemporalCore.FromInternal(new JsInternalDuration(default, time), settings.LargestUnit);
        return JsTemporal.CreateDuration(engine, since ? JsTemporal.Negated(result) : result);
    }
}
