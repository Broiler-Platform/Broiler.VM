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

namespace Broiler.VM.Profile.JavaScript;

/// <summary><c>Temporal.Duration</c> (Temporal s7; JSD-0054).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34360F
    // Broiler-Human:        PENDING
    private static readonly string[] DurationFieldNames =
        ["years", "months", "weeks", "days", "hours", "minutes", "seconds", "milliseconds", "microseconds", "nanoseconds"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CA8BA6
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

            return JsValue.Object(JsTemporal.CreateDuration(engine, fields, newTarget));
        });

        Method(constructor, "from", 1, static (engine, thisValue, arguments) =>
            JsValue.Object(JsTemporal.CreateDuration(engine, JsTemporal.ToDuration(engine, TemporalArgument(arguments, 0)))));

        Method(constructor, "compare", 2, static (engine, thisValue, arguments) =>
            JsValue.Number(DurationCompare(engine, arguments)));

        for (var i = 0; i < DurationFieldNames.Length; i++)
        {
            var index = i;
            TemporalGetter(prototype, DurationFieldNames[i], (engine, thisValue, arguments) =>
                JsValue.Number(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", DurationFieldNames[index]).Fields[index]));
        }

        TemporalGetter(prototype, "sign", static (engine, thisValue, arguments) =>
            JsValue.Number(JsTemporalCore.DurationSign(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "sign").Fields)));

        TemporalGetter(prototype, "blank", static (engine, thisValue, arguments) =>
            JsValue.Boolean(JsTemporalCore.DurationSign(TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "blank").Fields) == 0));

        Method(prototype, "with", 1, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "with");
            var partial = JsTemporal.ToPartialDuration(engine, TemporalArgument(arguments, 0));
            var fields = new double[10];

            for (var i = 0; i < 10; i++)
            {
                fields[i] = double.IsNaN(partial[i]) ? duration.Fields[i] : partial[i];
            }

            return JsValue.Object(JsTemporal.CreateDuration(engine, fields));
        });

        Method(prototype, "negated", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "negated");
            return JsValue.Object(JsTemporal.CreateDuration(engine, JsTemporal.Negated(duration.Fields)));
        });

        Method(prototype, "abs", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "abs");
            var fields = new double[10];

            for (var i = 0; i < 10; i++)
            {
                fields[i] = System.Math.Abs(duration.Fields[i]);
            }

            return JsValue.Object(JsTemporal.CreateDuration(engine, fields));
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
            var mode = JsTemporal.RoundingMode(engine, options, TemporalRounding.Trunc);
            var smallest = JsTemporal.UnitOption(engine, options, "smallestUnit", required: false);
            JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.Time);

            if (smallest is TemporalUnit.Hour or TemporalUnit.Minute)
            {
                throw engine.Error("RangeError", "Temporal.Duration.prototype.toString: smallestUnit must be a second or smaller");
            }

            var precision = JsTemporal.Precision(smallest, digits);

            if (precision.Unit == TemporalUnit.Nanosecond && precision.Increment == 1)
            {
                return JsValue.String(JsTemporalCore.DurationToString(duration.Fields, precision.Precision));
            }

            var largest = JsTemporalCore.DefaultLargestUnit(duration.Fields);
            var internalDuration = JsTemporalCore.ToInternal(duration.Fields);
            var time = JsTemporal.RoundTimeDuration(engine, internalDuration.Time, precision.Increment, precision.Unit, mode);
            var rounded = JsTemporal.FromInternal(engine, internalDuration with { Time = time }, JsTemporalCore.Larger(largest, TemporalUnit.Second));
            return JsValue.String(JsTemporalCore.DurationToString(rounded.Fields, precision.Precision));
        });

        Method(prototype, "toJSON", 0, static (engine, thisValue, arguments) =>
            JsValue.String(JsTemporalCore.DurationToString(
                TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "toJSON").Fields,
                JsTemporalCore.PrecisionAuto)));

        // ECMA-402'S DEFINITION (s15.11.1.1): the duration written by an Intl.DurationFormat.
        Method(prototype, "toLocaleString", 0, static (engine, thisValue, arguments) =>
        {
            var duration = TemporalThis<JsDurationObject>(engine, thisValue, "Duration", "toLocaleString");
            var format = (JsDurationFormatObject)NewDurationFormat(
                engine,
                JsValue.Object(engine.Realm.DurationFormatConstructor!),
                [TemporalArgument(arguments, 0), TemporalArgument(arguments, 1)]).AsObject();
            var text = new System.Text.StringBuilder();

            foreach (var part in PartitionDurationFormatPattern(engine, format, (double[])duration.Fields.Clone()))
            {
                text.Append(part.Value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "valueOf", 0, static (engine, thisValue, arguments) => TemporalValueOf(engine, "Duration"));
    }

    /// <summary>Temporal.Duration.compare (s7.2.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AEEC14
    // Broiler-Human:        PENDING
    private static double DurationCompare(JsEngine engine, JsValue[] arguments)
    {
        var one = JsTemporal.ToDuration(engine, TemporalArgument(arguments, 0));
        var two = JsTemporal.ToDuration(engine, TemporalArgument(arguments, 1));
        var options = JsTemporal.OptionsObject(engine, TemporalArgument(arguments, 2));
        var relativeTo = JsTemporal.RelativeToOption(engine, options);

        if (System.MemoryExtensions.SequenceEqual<double>(one, two))
        {
            return 0;
        }

        var largest1 = JsTemporalCore.DefaultLargestUnit(one);
        var largest2 = JsTemporalCore.DefaultLargestUnit(two);
        var duration1 = JsTemporalCore.ToInternal(one);
        var duration2 = JsTemporalCore.ToInternal(two);

        if (relativeTo.IsZoned && (JsTemporalCore.IsDateUnit(largest1) || JsTemporalCore.IsDateUnit(largest2)))
        {
            var after1 = JsTemporal.AddZonedDateTime(engine, relativeTo.ZonedNs!.Value, relativeTo.TimeZone!, relativeTo.Calendar, duration1, reject: false);
            var after2 = JsTemporal.AddZonedDateTime(engine, relativeTo.ZonedNs!.Value, relativeTo.TimeZone!, relativeTo.Calendar, duration2, reject: false);
            return after1.CompareTo(after2) switch { > 0 => 1, < 0 => -1, _ => 0 };
        }

        double days1;
        double days2;

        if (JsTemporalCore.IsCalendarUnit(largest1) || JsTemporalCore.IsCalendarUnit(largest2))
        {
            if (!relativeTo.IsPlain)
            {
                throw engine.Error("RangeError", "Temporal.Duration.compare: comparing years, months or weeks needs a relativeTo date");
            }

            days1 = JsTemporal.DateDurationDays(engine, duration1.Date, relativeTo.PlainDate!.Value);
            days2 = JsTemporal.DateDurationDays(engine, duration2.Date, relativeTo.PlainDate!.Value);
        }
        else
        {
            days1 = one[3];
            days2 = two[3];
        }

        var time1 = JsTemporal.Add24HourDays(engine, duration1.Time, days1);
        var time2 = JsTemporal.Add24HourDays(engine, duration2.Time, days2);
        return time1.CompareTo(time2) switch { > 0 => 1, < 0 => -1, _ => 0 };
    }

    /// <summary>The proposal's AddDurations (s7.5.41).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3F386
    // Broiler-Human:        PENDING
    private static JsDurationObject AddDurations(JsEngine engine, JsDurationObject duration, JsValue other, bool subtract)
    {
        var otherFields = JsTemporal.ToDuration(engine, other);

        if (subtract)
        {
            otherFields = JsTemporal.Negated(otherFields);
        }

        var largest = JsTemporalCore.Larger(JsTemporalCore.DefaultLargestUnit(duration.Fields), JsTemporalCore.DefaultLargestUnit(otherFields));

        if (JsTemporalCore.IsCalendarUnit(largest))
        {
            throw engine.Error("RangeError", "Temporal.Duration: adding years, months or weeks needs a relative date; use add on a date instead");
        }

        var d1 = JsTemporalCore.ToInternalWith24HourDays(duration.Fields);
        var d2 = JsTemporalCore.ToInternalWith24HourDays(otherFields);
        var time = JsTemporal.AddTimeDuration(engine, d1.Time, d2.Time);
        return JsTemporal.FromInternal(engine, new JsInternalDuration(default, time), largest);
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

    /// <summary>Temporal.Duration.prototype.round (s7.3.20).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F4E6FA
    // Broiler-Human:        PENDING
    private static JsDurationObject DurationRound(JsEngine engine, JsDurationObject duration, JsValue argument)
    {
        var roundTo = StringOrOptions(engine, argument, "smallestUnit", "round");
        var smallestPresent = true;
        var largestPresent = true;
        var largest = JsTemporal.UnitOption(engine, roundTo, "largestUnit", required: false);
        var relativeTo = JsTemporal.RelativeToOption(engine, roundTo);
        var increment = JsTemporal.RoundingIncrement(engine, roundTo);
        var mode = JsTemporal.RoundingMode(engine, roundTo, TemporalRounding.HalfExpand);
        var smallest = JsTemporal.UnitOption(engine, roundTo, "smallestUnit", required: false);
        JsTemporal.ValidateUnit(engine, smallest, JsTemporal.UnitGroup.DateTime);

        if (smallest == TemporalUnit.Unset)
        {
            smallestPresent = false;
            smallest = TemporalUnit.Nanosecond;
        }

        var existingLargest = JsTemporalCore.DefaultLargestUnit(duration.Fields);
        var defaultLargest = JsTemporalCore.Larger(existingLargest, smallest);

        if (largest == TemporalUnit.Unset)
        {
            largestPresent = false;
            largest = defaultLargest;
        }
        else if (largest == TemporalUnit.Auto)
        {
            largest = defaultLargest;
        }

        if (!smallestPresent && !largestPresent)
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: smallestUnit or largestUnit is required");
        }

        if (JsTemporalCore.Larger(largest, smallest) != largest)
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: largestUnit must be at least as large as smallestUnit");
        }

        var maximum = JsTemporalCore.MaximumIncrement(smallest);

        if (maximum != 0)
        {
            JsTemporal.ValidateIncrement(engine, increment, maximum, inclusive: false);
        }

        if (increment > 1 && largest != smallest && JsTemporalCore.IsDateUnit(smallest))
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: a date unit rounds by an increment only when it is also the largest unit");
        }

        if (relativeTo.IsZoned)
        {
            var internalDuration = JsTemporalCore.ToInternal(duration.Fields);
            var origin = relativeTo.ZonedNs!.Value;
            var target = JsTemporal.AddZonedDateTime(engine, origin, relativeTo.TimeZone!, relativeTo.Calendar, internalDuration, reject: false);
            internalDuration = JsTemporal.DifferenceZonedDateTimeWithRounding(
                engine, origin, target, relativeTo.TimeZone!, relativeTo.Calendar, largest, increment, smallest, mode);

            if (JsTemporalCore.IsDateUnit(largest))
            {
                largest = TemporalUnit.Hour;
            }

            return JsTemporal.FromInternal(engine, internalDuration, largest);
        }

        if (relativeTo.IsPlain)
        {
            var internalDuration = JsTemporalCore.ToInternalWith24HourDays(duration.Fields);
            var targetTime = JsTemporalCore.AddTime(JsTimeRecord.Midnight, internalDuration.Time);
            var dateDuration = JsTemporal.AdjustDate(engine, internalDuration.Date, targetTime.Days);
            var plain = relativeTo.PlainDate!.Value;
            var targetDate = JsTemporal.DateAdd(engine, plain, dateDuration, reject: false);
            internalDuration = JsTemporal.DifferencePlainDateTimeWithRounding(
                engine,
                new JsIsoDateTime(plain, JsTimeRecord.Midnight),
                new JsIsoDateTime(targetDate, targetTime with { Days = 0 }),
                relativeTo.Calendar,
                largest,
                increment,
                smallest,
                mode);
            return JsTemporal.FromInternal(engine, internalDuration, largest);
        }

        if (JsTemporalCore.IsCalendarUnit(existingLargest) || JsTemporalCore.IsCalendarUnit(largest))
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.round: rounding years, months or weeks needs a relativeTo date");
        }

        var balanced = JsTemporalCore.ToInternalWith24HourDays(duration.Fields);
        JsInternalDuration result;

        if (smallest == TemporalUnit.Day)
        {
            var days = JsTemporalCore.RoundToIncrement(balanced.Time, JsTemporalCore.NsPerDay, increment, mode);
            var dateDuration = JsTemporal.DateDurationRecord(engine, 0, 0, 0, JsTemporalCore.ToDouble(days));
            result = new JsInternalDuration(dateDuration, 0);
        }
        else
        {
            result = new JsInternalDuration(default, JsTemporal.RoundTimeDuration(engine, balanced.Time, increment, smallest, mode));
        }

        return JsTemporal.FromInternal(engine, result, largest);
    }

    /// <summary>Temporal.Duration.prototype.total (s7.3.21).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E4863
    // Broiler-Human:        PENDING
    private static double DurationTotal(JsEngine engine, JsDurationObject duration, JsValue argument)
    {
        var totalOf = StringOrOptions(engine, argument, "unit", "total");
        var relativeTo = JsTemporal.RelativeToOption(engine, totalOf);
        var unit = JsTemporal.UnitOption(engine, totalOf, "unit", required: true);
        JsTemporal.ValidateUnit(engine, unit, JsTemporal.UnitGroup.DateTime);

        if (relativeTo.IsZoned)
        {
            var internalDuration = JsTemporalCore.ToInternal(duration.Fields);
            var origin = relativeTo.ZonedNs!.Value;
            var target = JsTemporal.AddZonedDateTime(engine, origin, relativeTo.TimeZone!, relativeTo.Calendar, internalDuration, reject: false);
            return JsTemporal.DifferenceZonedDateTimeWithTotal(engine, origin, target, relativeTo.TimeZone!, relativeTo.Calendar, unit).ToDouble();
        }

        if (relativeTo.IsPlain)
        {
            var internalDuration = JsTemporalCore.ToInternalWith24HourDays(duration.Fields);
            var targetTime = JsTemporalCore.AddTime(JsTimeRecord.Midnight, internalDuration.Time);
            var dateDuration = JsTemporal.AdjustDate(engine, internalDuration.Date, targetTime.Days);
            var plain = relativeTo.PlainDate!.Value;
            var targetDate = JsTemporal.DateAdd(engine, plain, dateDuration, reject: false);
            return JsTemporal.DifferencePlainDateTimeWithTotal(
                engine,
                new JsIsoDateTime(plain, JsTimeRecord.Midnight),
                new JsIsoDateTime(targetDate, targetTime with { Days = 0 }),
                relativeTo.Calendar,
                unit).ToDouble();
        }

        var largest = JsTemporalCore.DefaultLargestUnit(duration.Fields);

        if (JsTemporalCore.IsCalendarUnit(largest) || JsTemporalCore.IsCalendarUnit(unit))
        {
            throw engine.Error("RangeError", "Temporal.Duration.prototype.total: a total in years, months or weeks needs a relativeTo date");
        }

        return JsTemporal.TotalTimeDuration(JsTemporalCore.ToInternalWith24HourDays(duration.Fields).Time, unit).ToDouble();
    }
}
