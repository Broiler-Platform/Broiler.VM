// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           0
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.Instant</c> (Temporal s8) and <c>Temporal.Now</c> (s2; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=86A7F7
    // Broiler-Human:        PENDING
    private void SetupTemporalInstant(JsObject temporal)
    {
        var prototype = new JsObject(ObjectPrototype);
        TemporalInstantPrototype = prototype;

        var constructor = TemporalConstructor(temporal, "Instant", 1, prototype, static (engine, newTarget, arguments) =>
        {
            var epochNs = JsTemporal.ValidEpochNs(engine, engine.ToBigInt(TemporalArgument(arguments, 0)).Value);
            return JsValue.Object(JsTemporal.CreateInstant(engine, epochNs, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreateInstant(engine, JsTemporal.ToInstant(engine, TemporalArgument(arguments, 0)))));

        Method(constructor, "fromEpochMilliseconds", 1, static (engine, thisValue, arguments) =>
        {
            var milliseconds = engine.ToNumber(TemporalArgument(arguments, 0));
            var epochNs = JsTemporal.ValidEpochNs(engine, JsTemporal.EpochNsFromMilliseconds(engine, milliseconds));
            return JsValue.Object(JsTemporal.CreateInstant(engine, epochNs));
        });

        Method(constructor, "fromEpochNanoseconds", 1, static (engine, thisValue, arguments) =>
        {
            var epochNs = JsTemporal.ValidEpochNs(engine, engine.ToBigInt(TemporalArgument(arguments, 0)).Value);
            return JsValue.Object(JsTemporal.CreateInstant(engine, epochNs));
        });

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
        {
            var one = JsTemporal.ToInstant(engine, TemporalArgument(arguments, 0));
            var two = JsTemporal.ToInstant(engine, TemporalArgument(arguments, 1));
            return JsValue.Number(one.CompareTo(two) switch { > 0 => 1, < 0 => -1, _ => 0 });
        });

        TemporalGetter(prototype, "epochMilliseconds", static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "epochMilliseconds");
            return JsValue.Number((double)FloorDivide(instant.EpochNanoseconds, 1_000_000));
        });

        TemporalGetter(prototype, "epochNanoseconds", static (engine, thisValue, arguments) =>
            JsValue.BigInt(new JsBigInt(TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "epochNanoseconds").EpochNanoseconds)));

        Method(prototype, "add", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToInstant(engine, TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "add"), TemporalArgument(arguments, 0), subtract: false)));

        Method(prototype, "subtract", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(AddDurationToInstant(engine, TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "subtract"), TemporalArgument(arguments, 0), subtract: true)));

        Method(prototype, "until", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalInstant(engine, TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "until"), arguments, since: false)));

        Method(prototype, "since", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(DifferenceTemporalInstant(engine, TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "since"), arguments, since: true)));

        Method(prototype, "round", 1, static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "round");
            var roundTo = TemporalRoundTo(engine, TemporalArgument(arguments, 0), "Instant");
            var increment = JsTemporal.RoundingIncrement(engine, roundTo);
            var mode = JsTemporal.RoundingModeOption(engine, roundTo, RoundingMode.HalfExpand);
            var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: true);

            if (!TemporalOptionRules.IsValidRoundOptions(TemporalType.Instant, smallest, increment))
            {
                throw engine.Error("RangeError", "Temporal.Instant.prototype.round: smallestUnit and roundingIncrement are not valid together");
            }

            return JsValue.Object(JsTemporal.CreateInstant(engine, IsoArithmetic.RoundTemporalInstant(instant.EpochNanoseconds, increment, smallest, mode)));
        });

        Method(prototype, "equals", 1, static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "equals");
            return JsValue.Boolean(instant.EpochNanoseconds == JsTemporal.ToInstant(engine, TemporalArgument(arguments, 0)));
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "toString");
            var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 0));
            var digits = JsTemporal.FractionalSecondDigits(engine, options);
            var mode = JsTemporal.RoundingModeOption(engine, options, TemporalOptionRules.DefaultToStringRoundingMode);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);
            var timeZoneValue = engine.GetProperty(JsValue.Object(options), "timeZone");

            if (!TemporalOptionRules.IsValidToStringSmallestUnit(smallest, duration: false))
            {
                throw engine.Error("RangeError", "Temporal.Instant.prototype.toString: smallestUnit must be a minute or smaller");
            }

            var timeZone = timeZoneValue.Type == JsType.Undefined ? null : JsTemporal.ToTimeZoneIdentifier(engine, timeZoneValue);
            var precision = TemporalOptionRules.ToSecondsStringPrecisionRecord(smallest, digits);
            var rounded = IsoArithmetic.RoundTemporalInstant(instant.EpochNanoseconds, precision.Increment, precision.Unit, mode);
            return JsValue.String(JsTemporal.InstantString(engine, rounded, timeZone, precision.Precision));
        });

        // ECMA-402'S DEFINITION (s15.11.2.1): the instant written by an Intl.DateTimeFormat made for
        // the call.
        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            _ = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "toLocaleString");
            return JsValue.String(TemporalToLocaleString(engine, thisValue, arguments, "any", "all"));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "toJSON");
            return JsValue.String(JsTemporal.InstantString(engine, instant.EpochNanoseconds, null, TemporalFormat.AutoPrecision));
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "Instant"));

        Method(prototype, "toZonedDateTimeISO", 1, static (engine, thisValue, arguments) =>
        {
            var instant = TemporalThis<JsInstantObject>(engine, thisValue, "Instant", "toZonedDateTimeISO");
            var timeZone = JsTemporal.ToTimeZoneIdentifier(engine, TemporalArgument(arguments, 0));
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, instant.EpochNanoseconds, timeZone, "iso8601"));
        });
    }

    /// <summary>floor(a / b) for a positive divisor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=90872C
    // Broiler-Human:        PENDING
    private static BigInteger FloorDivide(BigInteger value, BigInteger divisor)
    {
        var quotient = BigInteger.DivRem(value, divisor, out var remainder);
        return remainder.Sign < 0 ? quotient - 1 : quotient;
    }

    /// <summary>A round method's argument: a string is the smallest unit, an object the options.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2EFB10
    // Broiler-Human:        PENDING
    private static JsObject TemporalRoundTo(JsEngine engine, JsValue value, string type)
    {
        if (value.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Temporal." + type + ".prototype.round: an argument is required");
        }

        if (value.IsString)
        {
            var options = new JsObject(null);
            options.DefineOrdinary("smallestUnit", value);
            return options;
        }

        return JsTemporal.OptionsObject(engine, value);
    }

    /// <summary>The proposal's AddDurationToInstant (s8.5.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=952CDD
    // Broiler-Human:        PENDING
    private static JsInstantObject AddDurationToInstant(JsEngine engine, JsInstantObject instant, JsValue durationLike, bool subtract)
    {
        var fields = JsTemporal.ToDuration(engine, durationLike);

        if (subtract)
        {
            fields = DurationMath.Negate(fields);
        }

        if (TemporalOptionRules.IsDateUnit(DurationMath.DefaultLargestUnit(fields)))
        {
            throw engine.Error("RangeError", "Temporal.Instant: a duration with days or larger units cannot be added to an instant");
        }

        var internalDuration = DurationMath.ToInternalWith24HourDays(fields);
        return JsTemporal.CreateInstant(engine, JsTemporal.AddInstant(engine, instant.EpochNanoseconds, internalDuration.TimeNanoseconds));
    }

    /// <summary>The proposal's DifferenceTemporalInstant (s8.5.9): the reads here, the difference the shared library's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2ABE08
    // Broiler-Human:        PENDING
    private static JsDurationObject DifferenceTemporalInstant(JsEngine engine, JsInstantObject instant, JsValue[] arguments, bool since)
    {
        var other = JsTemporal.ToInstant(engine, TemporalArgument(arguments, 0));
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 1));
        var operation = since ? DifferenceOperation.Since : DifferenceOperation.Until;
        var settings = JsTemporal.DifferenceSettingsOf(engine, operation, options, TemporalType.Instant);
        JsTemporal.Check(
            engine, TemporalDifferences.DifferenceTemporalInstant(operation, instant.EpochNanoseconds, other, settings, out var result), "the difference");
        return JsTemporal.CreateDuration(engine, result);
    }

    // ---- Temporal.Now ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B608F4
    // Broiler-Human:        PENDING
    private void SetupTemporalNow(JsObject temporal)
    {
        var now = new JsObject(ObjectPrototype);

        now.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Temporal.Now"), JsPropertyAttributes.Configurable));

        temporal.SetOwnProperty(
            "Now",
            JsProperty.Data(JsValue.Object(now), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(now, "timeZoneId", 0, static (engine, thisValue, arguments) =>
            JsValue.String(SystemTimeZone(engine)));

        Method(now, "instant", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreateInstant(engine, SystemEpochNs())));

        Method(now, "plainDateTimeISO", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreatePlainDateTime(engine, SystemDateTime(engine, TemporalArgument(arguments, 0)), "iso8601")));

        Method(now, "zonedDateTimeISO", 0, static (engine, thisValue, arguments) =>
        {
            var timeZoneLike = TemporalArgument(arguments, 0);
            var timeZone = timeZoneLike.Type == JsType.Undefined ? SystemTimeZone(engine) : JsTemporal.ToTimeZoneIdentifier(engine, timeZoneLike);
            return JsValue.Object(JsTemporal.CreateZonedDateTime(engine, SystemEpochNs(), timeZone, "iso8601"));
        });

        Method(now, "plainDateISO", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreatePlainDate(engine, SystemDateTime(engine, TemporalArgument(arguments, 0)).Date, "iso8601")));

        Method(now, "plainTimeISO", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreatePlainTime(engine, SystemDateTime(engine, TemporalArgument(arguments, 0)).Time)));
    }

    /// <summary>
    /// The proposal's SystemTimeZoneIdentifier (s14.6.4): the profile's host time zone is UTC, as
    /// its Date's local time is (JSD-0045), so every program sees the same answer.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=915EFE
    // Broiler-Human:        PENDING
    private static string SystemTimeZone(JsEngine engine)
    {
        _ = engine;
        return "UTC";
    }

    /// <summary>The proposal's SystemUTCEpochNanoseconds (s2.3.3): the clock Date reads, in nanoseconds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=833B38
    // Broiler-Human:        PENDING
    private static BigInteger SystemEpochNs() =>
        new BigInteger(DateCurrentTime()) * 1_000_000;

    /// <summary>The proposal's SystemDateTime (s2.3.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4101F9
    // Broiler-Human:        PENDING
    private static IsoDateTime SystemDateTime(JsEngine engine, JsValue timeZoneLike)
    {
        var timeZone = timeZoneLike.Type == JsType.Undefined ? SystemTimeZone(engine) : JsTemporal.ToTimeZoneIdentifier(engine, timeZoneLike);
        return JsTemporal.IsoDateTimeFor(engine, timeZone, SystemEpochNs());
    }
}
