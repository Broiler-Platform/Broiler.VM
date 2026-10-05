// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           9
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.DurationFormat</c> (ECMA-402 s13; JSD-0051): a duration's units written by unit number
/// formats, or as a digital clock, and joined by a unit list format.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every number is the profile's own <c>Intl.NumberFormat</c>, and the list its own
/// <c>Intl.ListFormat</c></b>, each built by its constructor with the options the draft names, so a
/// duration writes each unit exactly as those formats write it.
/// </para>
/// <para>
/// <b>A fractional value is exact.</b> The draft adds milliseconds, microseconds and nanoseconds to a
/// larger unit as a mathematical value, and notes that floating point cannot. The sum is made here as
/// a BigInteger of the smallest unit and written as a decimal string, which the number format reads
/// exactly.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.DurationFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FB4C4B
    // Broiler-Human:        PENDING
    internal JsObject? DurationFormatPrototype { get; private set; }

    /// <summary><c>%Intl.DurationFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1EF22
    // Broiler-Human:        PENDING
    internal JsNativeFunction? DurationFormatConstructor { get; private set; }

    /// <summary>Table 20 and table 24: each unit, its singular, its styles and its digital default.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE21A7
    // Broiler-Human:        PENDING
    private static readonly (string Unit, string Singular, string[] Styles, string Digital)[] DurationUnits =
    [
        ("years", "year", ["long", "short", "narrow"], "short"),
        ("months", "month", ["long", "short", "narrow"], "short"),
        ("weeks", "week", ["long", "short", "narrow"], "short"),
        ("days", "day", ["long", "short", "narrow"], "short"),
        ("hours", "hour", ["long", "short", "narrow", "numeric", "2-digit"], "numeric"),
        ("minutes", "minute", ["long", "short", "narrow", "numeric", "2-digit"], "numeric"),
        ("seconds", "second", ["long", "short", "narrow", "numeric", "2-digit"], "numeric"),
        ("milliseconds", "millisecond", ["long", "short", "narrow", "numeric"], "numeric"),
        ("microseconds", "microsecond", ["long", "short", "narrow", "numeric"], "numeric"),
        ("nanoseconds", "nanosecond", ["long", "short", "narrow", "numeric"], "numeric"),
    ];

    /// <summary>The fields ToDurationRecord reads, in the order it reads them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1A36FF
    // Broiler-Human:        PENDING
    private static readonly string[] DurationFieldsRead =
        ["days", "hours", "microseconds", "milliseconds", "minutes", "months", "nanoseconds", "seconds", "weeks", "years"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=01E229
    // Broiler-Human:        PENDING
    private void SetupDurationFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        DurationFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "DurationFormat",
            0,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.DurationFormat requires 'new'"),
            static (engine, newTarget, arguments) => NewDurationFormat(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        DurationFormatConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        Method(prototype, "format", 1, static (engine, thisValue, arguments) =>
        {
            var format = DurationFormatOfThis(engine, thisValue, "format");
            var record = DurationFormatInput(engine, Argument(arguments, 0));
            var text = new System.Text.StringBuilder();

            foreach (var part in PartitionDurationFormatPattern(engine, format, record))
            {
                text.Append(part.Value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = DurationFormatOfThis(engine, thisValue, "formatToParts");
            var record = DurationFormatInput(engine, Argument(arguments, 0));
            var values = new System.Collections.Generic.List<JsValue>();

            foreach (var (type, value, unit) in PartitionDurationFormatPattern(engine, format, record))
            {
                engine.Charge(1);
                var item = new JsObject(engine.Realm.ObjectPrototype);
                item.DefineOrdinary("type", JsValue.String(type));
                item.DefineOrdinary("value", JsValue.String(value));

                if (unit is not null)
                {
                    item.DefineOrdinary("unit", JsValue.String(unit));
                }

                values.Add(JsValue.Object(item));
            }

            return JsValue.Object(engine.Realm.NewArray(values));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = DurationFormatOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 21, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(format.Locale));
            options.DefineOrdinary("numberingSystem", JsValue.String(format.NumberingSystem));
            options.DefineOrdinary("style", JsValue.String(format.Style));

            for (var i = 0; i < DurationUnits.Length; i++)
            {
                var (style, display) = format.Units[i];
                options.DefineOrdinary(DurationUnits[i].Unit, JsValue.String(style == "fractional" ? "numeric" : style));
                options.DefineOrdinary(DurationUnits[i].Unit + "Display", JsValue.String(display));
            }

            if (format.FractionalDigits is { } digits)
            {
                options.DefineOrdinary("fractionalDigits", JsValue.Number(digits));
            }

            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.DurationFormat"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "DurationFormat",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The duration format <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D53C06
    // Broiler-Human:        PENDING
    private static JsDurationFormatObject DurationFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDurationFormatObject ??
        throw engine.Error("TypeError", "Intl.DurationFormat.prototype." + member + " requires that 'this' be an Intl.DurationFormat");

    /// <summary>ECMA-402's <c>Intl.DurationFormat</c> constructor (s13.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A30F45
    // Broiler-Human:        PENDING
    internal static JsValue NewDurationFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.DurationFormatPrototype!);

        // RESOLVEOPTIONS: the locales, the options through GetOptionsObject, the matcher, and nu.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = JsValue.Object(Base64Options(engine, Argument(arguments, 1)));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            return engine.ThrowRangeError("Intl.DurationFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        var numbers = engine.Intl!.Numbers;
        var systems = new System.Collections.Generic.List<string?> { "latn" };

        foreach (var name in numbers.NumberingSystems.Keys)
        {
            if (name != "latn")
            {
                systems.Add(name);
            }
        }

        systems.Sort(1, systems.Count - 1, System.StringComparer.Ordinal);
        var nuValues = systems.ToArray();

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal) { ["nu"] = numberingSystem },
            ["nu"],
            (locale, key) => key == "nu" ? nuValues : [null]);

        // [[DIGITALFORMAT]]: CLDR's time separator, and whether its hours-minutes-seconds pattern
        // writes the hours with two digits.
        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";
        var separator = numbers.Value(language, "symbols.timeSeparator") ?? ":";
        var twoDigitHours = (numbers.Value(language, "duration.hms") ?? "h:mm:ss").StartsWith("hh", System.StringComparison.Ordinal);

        var style = GetStringOption(engine, options, "style", ["long", "short", "narrow", "digital"], "short")!;
        var units = new (string Style, string Display)[DurationUnits.Length];
        var previous = string.Empty;

        for (var i = 0; i < DurationUnits.Length; i++)
        {
            var (unit, _, styles, digital) = DurationUnits[i];
            units[i] = GetDurationUnitOptions(engine, unit, options, style, styles, digital, previous, twoDigitHours);

            if (unit is "hours" or "minutes" or "seconds" or "milliseconds" or "microseconds")
            {
                previous = units[i].Style;
            }
        }

        var fractionalDigits = GetNumberOption(engine, options, "fractionalDigits", 0, 9, null);

        return JsValue.Object(new JsDurationFormatObject(prototype)
        {
            Locale = resolved.Locale,
            NumberingSystem = resolved.Keys["nu"] ?? "latn",
            Style = style,
            Units = units,
            FractionalDigits = fractionalDigits,
            HourMinuteSeparator = separator,
            MinuteSecondSeparator = separator,
        });
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F87032
    // Broiler-Human:        PENDING
    private static bool IsFractionalSecondUnitName(string unit) => unit is "milliseconds" or "microseconds" or "nanoseconds";

    /// <summary>ECMA-402's GetDurationUnitOptions (s13.5.6) with ValidateDurationUnitStyle (s13.5.6.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=09E581
    // Broiler-Human:        PENDING
    private static (string Style, string Display) GetDurationUnitOptions(
        JsEngine engine, string unit, JsValue options, string baseStyle, string[] styles, string digitalBase, string previous, bool twoDigitHours)
    {
        var style = GetStringOption(engine, options, unit, styles, null);
        var displayDefault = "always";

        if (style is null)
        {
            if (baseStyle == "digital")
            {
                style = digitalBase;

                if (unit is not ("hours" or "minutes" or "seconds"))
                {
                    displayDefault = "auto";
                }
            }
            else if (previous is "fractional" or "numeric" or "2-digit")
            {
                style = "numeric";

                if (unit is not ("minutes" or "seconds"))
                {
                    displayDefault = "auto";
                }
            }
            else
            {
                style = baseStyle;
                displayDefault = "auto";
            }
        }

        if (style == "numeric" && IsFractionalSecondUnitName(unit))
        {
            style = "fractional";
            displayDefault = "auto";
        }

        var display = GetStringOption(engine, options, unit + "Display", ["auto", "always"], displayDefault)!;

        if (display == "always" && style == "fractional")
        {
            throw engine.Error("RangeError", "Intl.DurationFormat: " + unit + " cannot always be displayed as a fraction");
        }

        if (previous == "fractional" && style != "fractional")
        {
            throw engine.Error("RangeError", "Intl.DurationFormat: " + unit + " must be fractional after a fractional unit");
        }

        if (previous is "numeric" or "2-digit" && style is not ("fractional" or "numeric" or "2-digit"))
        {
            throw engine.Error("RangeError", "Intl.DurationFormat: " + unit + " must be numeric after a numeric unit");
        }

        if (unit == "hours" && twoDigitHours)
        {
            style = "2-digit";
        }

        if (unit is "minutes" or "seconds" && previous is "numeric" or "2-digit")
        {
            style = "2-digit";
        }

        return (style, display);
    }

    /// <summary>
    /// The duration format and formatToParts read: with Temporal admitted, ToTemporalDuration (the
    /// proposal's amendment of s15.10.1, which takes a string too); without it, ToDurationRecord.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D80BE2
    // Broiler-Human:        PENDING
    private static double[] DurationFormatInput(JsEngine engine, JsValue input) =>
        engine.Realm.TemporalDurationPrototype is not null ? JsTemporal.ToDuration(engine, input) : ToDurationRecord(engine, input);

    /// <summary>
    /// ECMA-402's ToDurationRecord (s13.5.3): the ten fields read in alphabetical order, each an
    /// integral Number, and the whole a valid duration.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A71719
    // Broiler-Human:        PENDING
    private static double[] ToDurationRecord(JsEngine engine, JsValue input)
    {
        if (!input.IsObject)
        {
            throw engine.Error(input.IsString ? "RangeError" : "TypeError", "Intl.DurationFormat: a duration must be an object");
        }

        // THE RECORD IN TABLE 24'S ORDER, READ IN ALPHABETICAL ORDER.
        var record = new double[DurationUnits.Length];
        var any = false;

        foreach (var field in DurationFieldsRead)
        {
            var value = engine.GetProperty(input, field);

            if (value.Type == JsType.Undefined)
            {
                continue;
            }

            var number = engine.ToNumber(value);

            if (!double.IsFinite(number) || System.Math.Truncate(number) != number)
            {
                throw engine.Error("RangeError", "Intl.DurationFormat: " + field + " must be an integer");
            }

            record[System.Array.FindIndex(DurationUnits, unit => unit.Unit == field)] = number + 0.0;
            any = true;
        }

        if (!any)
        {
            throw engine.Error("TypeError", "Intl.DurationFormat: a duration needs at least one field");
        }

        if (!IsValidDuration(record))
        {
            throw engine.Error("RangeError", "Intl.DurationFormat: the duration is not valid");
        }

        return record;
    }

    /// <summary>ECMA-402's IsValidDuration (s13.5.5), its sum of seconds made exactly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E5285F
    // Broiler-Human:        PENDING
    private static bool IsValidDuration(double[] record)
    {
        var sign = 0;

        foreach (var value in record)
        {
            if (value < 0)
            {
                if (sign > 0)
                {
                    return false;
                }

                sign = -1;
            }
            else if (value > 0)
            {
                if (sign < 0)
                {
                    return false;
                }

                sign = 1;
            }
        }

        var limit = System.Numerics.BigInteger.Pow(2, 32);

        for (var i = 0; i < 3; i++)
        {
            if (System.Numerics.BigInteger.Abs(new System.Numerics.BigInteger(record[i])) >= limit)
            {
                return false;
            }
        }

        // DAYS TO SECONDS AS NANOSECONDS: (d * 86,400 + h * 3,600 + m * 60 + s) * 10^9 + ms * 10^6 + us * 10^3 + ns.
        var nanoseconds =
            (((new System.Numerics.BigInteger(record[3]) * 86_400) + (new System.Numerics.BigInteger(record[4]) * 3_600) +
              (new System.Numerics.BigInteger(record[5]) * 60) + new System.Numerics.BigInteger(record[6])) * 1_000_000_000) +
            (new System.Numerics.BigInteger(record[7]) * 1_000_000) + (new System.Numerics.BigInteger(record[8]) * 1_000) +
            new System.Numerics.BigInteger(record[9]);

        return System.Numerics.BigInteger.Abs(nanoseconds) < System.Numerics.BigInteger.Pow(2, 53) * 1_000_000_000;
    }

    /// <summary>ECMA-402's DurationSign (s13.5.4).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7B1E85
    // Broiler-Human:        PENDING
    private static int DurationSign(double[] record)
    {
        foreach (var value in record)
        {
            if (value < 0)
            {
                return -1;
            }

            if (value > 0)
            {
                return 1;
            }
        }

        return 0;
    }

    /// <summary>
    /// The value of unit <paramref name="index"/> with ComputeFractionalDigits (s13.5.7) added: the
    /// fractional units that follow it, each a thousandth of the one before, as an exact decimal.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=36E10D
    // Broiler-Human:        PENDING
    private static JsDecimal WithFractions(JsEngine engine, JsDurationFormatObject format, double[] record, int index)
    {
        var total = new System.Numerics.BigInteger(record[index]);
        var digits = 0;

        for (var i = index + 1; i < DurationUnits.Length && format.Units[i].Style == "fractional"; i++)
        {
            total = (total * 1_000) + new System.Numerics.BigInteger(record[i]);
            digits += 3;
        }

        if (digits == 0)
        {
            return JsDecimal.FromNumber(record[index]);
        }

        var negative = total.Sign < 0;
        var text = System.Numerics.BigInteger.Abs(total).ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(digits + 1, '0');
        var decimalText = (negative ? "-" : string.Empty) + text[..^digits] + "." + text[^digits..];
        return JsDecimal.FromString(decimalText, engine.ChargeFuel);
    }

    /// <summary>A number format of the duration format's locale, built by the constructor from <paramref name="settings"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F02052
    // Broiler-Human:        PENDING
    private static JsNumberFormatter DurationNumberFormat(
        JsEngine engine, JsDurationFormatObject format, System.Collections.Generic.List<(string Key, JsValue Value)> settings)
    {
        var options = new JsObject(null);
        options.DefineOrdinary("numberingSystem", JsValue.String(format.NumberingSystem));

        foreach (var (key, value) in settings)
        {
            options.DefineOrdinary(key, value);
        }

        return ((JsNumberFormatObject)NewNumberFormat(
            engine,
            JsValue.Object(engine.Realm.NumberFormatConstructor!),
            [JsValue.String(format.Locale), JsValue.Object(options)],
            JsValue.Undefined,
            called: false).AsObject()).Formatter;
    }

    /// <summary>The digits settings of a value with fractions: up to nine digits, or exactly fractionalDigits, truncated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=86C383
    // Broiler-Human:        PENDING
    private static void FractionSettings(JsDurationFormatObject format, System.Collections.Generic.List<(string Key, JsValue Value)> settings)
    {
        var digits = format.FractionalDigits;
        settings.Add(("maximumFractionDigits", JsValue.Number(digits ?? 9)));
        settings.Add(("minimumFractionDigits", JsValue.Number(digits ?? 0)));
        settings.Add(("roundingMode", JsValue.String("trunc")));
    }

    /// <summary>
    /// ECMA-402's FormatNumericHours, FormatNumericMinutes and FormatNumericSeconds (s13.5.9 to
    /// s13.5.11): one clock field, after its separator where a larger field precedes it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6ECA63
    // Broiler-Human:        PENDING
    private static void FormatNumericField(
        JsEngine engine,
        JsDurationFormatObject format,
        int index,
        JsDecimal value,
        string? separator,
        bool signDisplayed,
        System.Collections.Generic.List<(string Type, string Value, string? Unit)> result)
    {
        if (separator is not null)
        {
            result.Add(("literal", separator, null));
        }

        var settings = new System.Collections.Generic.List<(string Key, JsValue Value)>();

        if (format.Units[index].Style == "2-digit")
        {
            settings.Add(("minimumIntegerDigits", JsValue.Number(2)));
        }

        if (!signDisplayed)
        {
            settings.Add(("signDisplay", JsValue.String("never")));
        }

        settings.Add(("useGrouping", JsValue.Boolean(false)));

        if (index == 6)
        {
            FractionSettings(format, settings);
        }

        foreach (var part in DurationNumberFormat(engine, format, settings).Parts(engine, value))
        {
            result.Add((part.Type, part.Value, DurationUnits[index].Singular));
        }
    }

    /// <summary>ECMA-402's FormatNumericUnits (s13.5.12): the clock, from its first numeric field.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1ECE55
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(string Type, string Value, string? Unit)> FormatNumericUnits(
        JsEngine engine, JsDurationFormatObject format, double[] record, int first, bool signDisplayed)
    {
        var result = new System.Collections.Generic.List<(string Type, string Value, string? Unit)>();
        var hours = record[4];
        var minutes = record[5];
        var seconds = record[7] != 0 || record[8] != 0 || record[9] != 0
            ? WithFractions(engine, format, record, 6)
            : JsDecimal.FromNumber(record[6]);

        var hoursFormatted = first == 4 && (hours != 0 || format.Units[4].Display == "always");
        var secondsFormatted = !seconds.IsZero || format.Units[6].Display == "always";
        var minutesFormatted = first is 4 or 5 &&
            ((hoursFormatted && secondsFormatted) || minutes != 0 || format.Units[5].Display == "always");

        if (hoursFormatted)
        {
            var value = signDisplayed && hours == 0 && DurationSign(record) == -1 ? -0.0 : hours;
            FormatNumericField(engine, format, 4, JsDecimal.FromNumber(value), null, signDisplayed, result);
            signDisplayed = false;
        }

        if (minutesFormatted)
        {
            var value = signDisplayed && minutes == 0 && DurationSign(record) == -1 ? -0.0 : minutes;
            FormatNumericField(engine, format, 5, JsDecimal.FromNumber(value), hoursFormatted ? format.HourMinuteSeparator : null, signDisplayed, result);
            signDisplayed = false;
        }

        if (secondsFormatted)
        {
            FormatNumericField(engine, format, 6, seconds, minutesFormatted ? format.MinuteSecondSeparator : null, signDisplayed, result);
        }

        return result;
    }

    /// <summary>
    /// ECMA-402's PartitionDurationFormatPattern (s13.5.15): each unit before the clock by a unit
    /// number format, the clock, and the whole joined by ListFormatParts (s13.5.14).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0ED68B
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(string Type, string Value, string? Unit)> PartitionDurationFormatPattern(
        JsEngine engine, JsDurationFormatObject format, double[] record)
    {
        var lists = new System.Collections.Generic.List<System.Collections.Generic.List<(string Type, string Value, string? Unit)>>();
        var signDisplayed = true;

        for (var i = 0; i < DurationUnits.Length; i++)
        {
            engine.Charge(1);
            var (style, display) = format.Units[i];

            if (style is "numeric" or "2-digit")
            {
                var numeric = FormatNumericUnits(engine, format, record, i, signDisplayed);

                if (numeric.Count != 0)
                {
                    lists.Add(numeric);
                }

                break;
            }

            var settings = new System.Collections.Generic.List<(string Key, JsValue Value)>();
            var nextFractional = i + 1 < DurationUnits.Length && i >= 6 && format.Units[i + 1].Style == "fractional";
            var value = nextFractional ? WithFractions(engine, format, record, i) : JsDecimal.FromNumber(record[i]);

            if (nextFractional)
            {
                FractionSettings(format, settings);
            }

            if (display == "always" || !value.IsZero)
            {
                if (signDisplayed)
                {
                    signDisplayed = false;

                    if (value.IsZero && DurationSign(record) == -1)
                    {
                        value = JsDecimal.FromNumber(-0.0);
                    }
                }
                else
                {
                    settings.Add(("signDisplay", JsValue.String("never")));
                }

                settings.Add(("style", JsValue.String("unit")));
                settings.Add(("unit", JsValue.String(DurationUnits[i].Singular)));
                settings.Add(("unitDisplay", JsValue.String(style)));

                var list = new System.Collections.Generic.List<(string Type, string Value, string? Unit)>();

                foreach (var part in DurationNumberFormat(engine, format, settings).Parts(engine, value))
                {
                    list.Add((part.Type, part.Value, DurationUnits[i].Singular));
                }

                lists.Add(list);
            }

            if (nextFractional)
            {
                break;
            }
        }

        // LISTFORMATPARTS: a unit list format of the style (short for digital), and each element's parts in its place.
        var listOptions = new JsObject(null);
        listOptions.DefineOrdinary("type", JsValue.String("unit"));
        listOptions.DefineOrdinary("style", JsValue.String(format.Style == "digital" ? "short" : format.Style));
        var listFormat = (JsListFormatObject)NewListFormat(
            engine,
            JsValue.Object(engine.Realm.ListFormatConstructor!),
            [JsValue.String(format.Locale), JsValue.Object(listOptions)]).AsObject();

        var strings = new System.Collections.Generic.List<string>();

        foreach (var parts in lists)
        {
            var text = new System.Text.StringBuilder();

            foreach (var part in parts)
            {
                text.Append(part.Value);
            }

            strings.Add(text.ToString());
        }

        var result = new System.Collections.Generic.List<(string Type, string Value, string? Unit)>();
        var element = 0;

        foreach (var (type, value) in CreatePartsFromList(engine, listFormat, strings))
        {
            if (type == "element")
            {
                result.AddRange(lists[element]);
                element++;
            }
            else
            {
                result.Add(("literal", value, null));
            }
        }

        return result;
    }
}

/// <summary>An <c>Intl.DurationFormat</c>: its resolved options, each unit's style and display.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=24169B
// Broiler-Human:        PENDING
internal sealed class JsDurationFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=111DCF
    // Broiler-Human:        PENDING
    internal JsDurationFormatObject(JsObject prototype)
        : base(prototype)
    {
    }

    /// <summary>The resolved locale.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDBB2D
    // Broiler-Human:        PENDING
    internal string Locale { get; init; } = string.Empty;

    /// <summary>The resolved numbering system.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=57F126
    // Broiler-Human:        PENDING
    internal string NumberingSystem { get; init; } = "latn";

    /// <summary><c>long</c>, <c>short</c>, <c>narrow</c> or <c>digital</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=72F5BE
    // Broiler-Human:        PENDING
    internal string Style { get; init; } = "short";

    /// <summary>Each unit's style and display, in table 24's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FC3282
    // Broiler-Human:        PENDING
    internal (string Style, string Display)[] Units { get; init; } = [];

    /// <summary>[[FractionalDigits]], or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EF1DA2
    // Broiler-Human:        PENDING
    internal int? FractionalDigits { get; init; }

    /// <summary>[[HourMinuteSeparator]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E511CB
    // Broiler-Human:        PENDING
    internal string HourMinuteSeparator { get; init; } = ":";

    /// <summary>[[MinuteSecondSeparator]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=978B6E
    // Broiler-Human:        PENDING
    internal string MinuteSecondSeparator { get; init; } = ":";
}
