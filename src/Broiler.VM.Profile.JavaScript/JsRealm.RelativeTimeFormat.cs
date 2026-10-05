// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           9
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.RelativeTimeFormat</c> (ECMA-402 s18; JSD-0049): a value of a unit written as CLDR's
/// relative time pattern for its tense and plural category, or as CLDR's literal for it.
/// </summary>
/// <remarks>
/// <para>
/// <b>The number is the format's own <c>Intl.NumberFormat</c> and the category its own
/// <c>Intl.PluralRules</c></b>, each made as the constructor makes them, so a relative time writes
/// its number as a decimal format of the locale and numbering system writes it.
/// </para>
/// <para>
/// <b>The number written is the value's magnitude.</b> The tense carries the sign: CLDR's past
/// patterns say "ago", and every engine and test262 write <c>-1</c> days as "1 day ago". The
/// draft's PartitionRelativeTimePattern passes the signed value to PartitionNumberPattern, which would
/// write the minus sign as well; JSD-0049 section 3 records the reading.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.RelativeTimeFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=575C0B
    // Broiler-Human:        PENDING
    internal JsObject? RelativeTimeFormatPrototype { get; private set; }

    /// <summary><c>%Intl.RelativeTimeFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ECF2C9
    // Broiler-Human:        PENDING
    internal JsNativeFunction? RelativeTimeFormatConstructor { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6E8772
    // Broiler-Human:        PENDING
    private void SetupRelativeTimeFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        RelativeTimeFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "RelativeTimeFormat",
            0,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.RelativeTimeFormat requires 'new'"),
            static (engine, newTarget, arguments) => NewRelativeTimeFormat(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        RelativeTimeFormatConstructor = constructor;

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

        Method(prototype, "format", 2, static (engine, thisValue, arguments) =>
        {
            var format = RelativeTimeFormatOfThis(engine, thisValue, "format");
            var value = engine.ToNumber(Argument(arguments, 0));
            var unit = engine.ToStringValue(Argument(arguments, 1));
            var text = new System.Text.StringBuilder();

            foreach (var part in PartitionRelativeTimePattern(engine, format, value, unit))
            {
                text.Append(part.Value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "formatToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = RelativeTimeFormatOfThis(engine, thisValue, "formatToParts");
            var value = engine.ToNumber(Argument(arguments, 0));
            var unit = engine.ToStringValue(Argument(arguments, 1));
            var values = new System.Collections.Generic.List<JsValue>();

            foreach (var (type, text, partUnit) in PartitionRelativeTimePattern(engine, format, value, unit))
            {
                engine.Charge(1);
                var item = new JsObject(engine.Realm.ObjectPrototype);
                item.DefineOrdinary("type", JsValue.String(type));
                item.DefineOrdinary("value", JsValue.String(text));

                if (partUnit is not null)
                {
                    item.DefineOrdinary("unit", JsValue.String(partUnit));
                }

                values.Add(JsValue.Object(item));
            }

            return JsValue.Object(engine.Realm.NewArray(values));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = RelativeTimeFormatOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 33, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(format.Locale));
            options.DefineOrdinary("style", JsValue.String(format.Style));
            options.DefineOrdinary("numeric", JsValue.String(format.Numeric));
            options.DefineOrdinary("numberingSystem", JsValue.String(format.NumberingSystem));
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.RelativeTimeFormat"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "RelativeTimeFormat",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The relative time format <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=525DBA
    // Broiler-Human:        PENDING
    private static JsRelativeTimeFormatObject RelativeTimeFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsRelativeTimeFormatObject ??
        throw engine.Error("TypeError", "Intl.RelativeTimeFormat.prototype." + member + " requires that 'this' be an Intl.RelativeTimeFormat");

    /// <summary>ECMA-402's <c>Intl.RelativeTimeFormat</c> constructor (s18.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F91077
    // Broiler-Human:        PENDING
    internal static JsValue NewRelativeTimeFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var realm = engine.Realm;
        var prototype = engine.PrototypeFromConstructor(newTarget, realm.RelativeTimeFormatPrototype!);

        // RESOLVEOPTIONS: the locales, the options coerced, the matcher, and the one relevant key,
        // nu, read from numberingSystem.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = CoerceOptionsToObject(engine, Argument(arguments, 1));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            return engine.ThrowRangeError("Intl.RelativeTimeFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        var systems = new System.Collections.Generic.List<string?> { "latn" };

        foreach (var name in engine.Intl!.Numbers.NumberingSystems.Keys)
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

        var style = GetStringOption(engine, options, "style", ["long", "short", "narrow"], "long")!;
        var numeric = GetStringOption(engine, options, "numeric", ["always", "auto"], "always")!;
        var nu = resolved.Keys["nu"] ?? "latn";

        // [[NUMBERFORMAT]] AND [[PLURALRULES]], CONSTRUCTED AS THE DRAFT CONSTRUCTS THEM.
        var nfOptions = new JsObject(null);
        nfOptions.DefineOrdinary("numberingSystem", JsValue.String(nu));
        var numberFormat = (JsNumberFormatObject)NewNumberFormat(
            engine,
            JsValue.Object(realm.NumberFormatConstructor!),
            [JsValue.String(resolved.Locale), JsValue.Object(nfOptions)],
            JsValue.Undefined,
            called: false).AsObject();
        var pluralRules = (JsPluralRulesObject)NewPluralRules(
            engine, JsValue.Object(realm.PluralRulesConstructor!), [JsValue.String(resolved.Locale)]).AsObject();

        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";
        return JsValue.Object(new JsRelativeTimeFormatObject(
            prototype, resolved.Locale, language, style, numeric, nu, numberFormat.Formatter, pluralRules));
    }

    /// <summary>ECMA-402's SingularRelativeTimeUnit (s18.5.1): a unit, or its plural, or a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E1A47
    // Broiler-Human:        PENDING
    private static string SingularRelativeTimeUnit(JsEngine engine, string unit) => unit switch
    {
        "second" or "seconds" => "second",
        "minute" or "minutes" => "minute",
        "hour" or "hours" => "hour",
        "day" or "days" => "day",
        "week" or "weeks" => "week",
        "month" or "months" => "month",
        "quarter" or "quarters" => "quarter",
        "year" or "years" => "year",
        _ => throw engine.Error("RangeError", "Intl.RelativeTimeFormat: invalid unit: " + unit),
    };

    /// <summary>
    /// ECMA-402's PartitionRelativeTimePattern (s18.5.2): the literal for the value where the numeric
    /// option is <c>auto</c> and CLDR names one, and otherwise the tense's pattern for the plural
    /// category, its placeable the formatted number's parts, each with the unit.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EB82D9
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(string Type, string Value, string? Unit)> PartitionRelativeTimePattern(
        JsEngine engine, JsRelativeTimeFormatObject format, double value, string unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw engine.Error("RangeError", "Intl.RelativeTimeFormat: the value must be finite");
        }

        unit = SingularRelativeTimeUnit(engine, unit);
        var patterns = engine.Intl!.LocaleInfo.RelativeTimes;
        var field = unit;

        if (format.Style != "long" && patterns.ContainsKey(format.Language + "|" + unit + "-" + format.Style + "|future.other"))
        {
            field = unit + "-" + format.Style;
        }

        var prefix = format.Language + "|" + field + "|";
        var parts = new System.Collections.Generic.List<(string Type, string Value, string? Unit)>();

        if (format.Numeric == "auto" &&
            patterns.TryGetValue(prefix + engine.ToStringValue(JsValue.Number(value)), out var literal))
        {
            parts.Add(("literal", literal, null));
            return parts;
        }

        var tense = value < 0 || (value == 0 && double.IsNegative(value)) ? "past" : "future";
        var magnitude = JsDecimal.FromNumber(System.Math.Abs(value));
        var category = ResolvePlural(engine, format.PluralRules, magnitude).Category;

        if (!patterns.TryGetValue(prefix + tense + "." + category, out var pattern))
        {
            pattern = patterns[prefix + tense + ".other"];
        }

        // MAKEPARTSLIST (s18.5.3): the literals of the pattern, and the number's parts in its place.
        var at = 0;

        while (at < pattern.Length)
        {
            var open = pattern.IndexOf("{0}", at, System.StringComparison.Ordinal);

            if (open < 0)
            {
                parts.Add(("literal", pattern[at..], null));
                break;
            }

            if (open > at)
            {
                parts.Add(("literal", pattern[at..open], null));
            }

            foreach (var part in format.NumberFormatter.Parts(engine, magnitude))
            {
                parts.Add((part.Type, part.Value, unit));
            }

            at = open + 3;
        }

        return parts;
    }
}

/// <summary>An <c>Intl.RelativeTimeFormat</c>: its resolved options, and the number format and plural rules it writes with.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44505C
// Broiler-Human:        PENDING
internal sealed class JsRelativeTimeFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B91AC9
    // Broiler-Human:        PENDING
    internal JsRelativeTimeFormatObject(
        JsObject prototype,
        string locale,
        string language,
        string style,
        string numeric,
        string numberingSystem,
        JsNumberFormatter numberFormatter,
        JsPluralRulesObject pluralRules)
        : base(prototype)
    {
        Locale = locale;
        Language = language;
        Style = style;
        Numeric = numeric;
        NumberingSystem = numberingSystem;
        NumberFormatter = numberFormatter;
        PluralRules = pluralRules;
    }

    /// <summary>The resolved locale.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=04656B
    // Broiler-Human:        PENDING
    internal string Locale { get; }

    /// <summary>The language whose patterns are read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DE92F3
    // Broiler-Human:        PENDING
    internal string Language { get; }

    /// <summary><c>long</c>, <c>short</c> or <c>narrow</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=528E3C
    // Broiler-Human:        PENDING
    internal string Style { get; }

    /// <summary><c>always</c> or <c>auto</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A42596
    // Broiler-Human:        PENDING
    internal string Numeric { get; }

    /// <summary>The resolved numbering system.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E2F6E7
    // Broiler-Human:        PENDING
    internal string NumberingSystem { get; }

    /// <summary>[[NumberFormat]]'s formatter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=966252
    // Broiler-Human:        PENDING
    internal JsNumberFormatter NumberFormatter { get; }

    /// <summary>[[PluralRules]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B4F169
    // Broiler-Human:        PENDING
    internal JsPluralRulesObject PluralRules { get; }
}
