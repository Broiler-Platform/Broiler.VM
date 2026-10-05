// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           8
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.DisplayNames</c> (ECMA-402 s12; JSD-0052): the names CLDR gives languages, regions,
/// scripts, currencies, calendars and date-time fields in the format's language.
/// </summary>
/// <remarks>
/// <para>
/// <b>[[Fields]] holds a name for every code whose every part CLDR names.</b> A language's name is
/// composed as ICU composes it: a dialect name for the whole code where the data has one (<c>en-GB</c>
/// is "British English"), and otherwise the language's name with its script, region and variants in
/// CLDR's locale pattern. A code with a part CLDR does not name has no field, so
/// <c>Intl.DisplayNames.prototype.of</c> answers the code itself or <c>undefined</c>, as the draft
/// states.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.DisplayNames.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F246F8
    // Broiler-Human:        PENDING
    internal JsObject? DisplayNamesPrototype { get; private set; }

    /// <summary><c>%Intl.DisplayNames%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=70AE34
    // Broiler-Human:        PENDING
    internal JsNativeFunction? DisplayNamesConstructor { get; private set; }

    /// <summary>The date-time field codes of table 19.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CE52F9
    // Broiler-Human:        PENDING
    private static readonly string[] DateTimeFieldCodes =
        ["era", "year", "quarter", "month", "weekOfYear", "weekday", "day", "dayPeriod", "hour", "minute", "second", "timeZoneName"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0F05BE
    // Broiler-Human:        PENDING
    private void SetupDisplayNames(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        DisplayNamesPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "DisplayNames",
            2,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.DisplayNames requires 'new'"),
            static (engine, newTarget, arguments) => NewDisplayNames(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        DisplayNamesConstructor = constructor;

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

        Method(prototype, "of", 1, static (engine, thisValue, arguments) =>
        {
            var names = DisplayNamesOfThis(engine, thisValue, "of");
            var code = CanonicalCodeForDisplayNames(engine, names.Type, engine.ToStringValue(Argument(arguments, 0)));
            var name = DisplayName(engine, names, code);

            return name is not null ? JsValue.String(name) : names.Fallback == "code" ? JsValue.String(code) : JsValue.Undefined;
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var names = DisplayNamesOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 18, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(names.Locale));
            options.DefineOrdinary("style", JsValue.String(names.Style));
            options.DefineOrdinary("type", JsValue.String(names.Type));
            options.DefineOrdinary("fallback", JsValue.String(names.Fallback));

            if (names.LanguageDisplay is { } display)
            {
                options.DefineOrdinary("languageDisplay", JsValue.String(display));
            }

            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.DisplayNames"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "DisplayNames",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The display names <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=87FC3D
    // Broiler-Human:        PENDING
    private static JsDisplayNamesObject DisplayNamesOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDisplayNamesObject ??
        throw engine.Error("TypeError", "Intl.DisplayNames.prototype." + member + " requires that 'this' be an Intl.DisplayNames");

    /// <summary>ECMA-402's <c>Intl.DisplayNames</c> constructor (s12.1.1): the options required, and read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F72D91
    // Broiler-Human:        PENDING
    internal static JsValue NewDisplayNames(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.DisplayNamesPrototype!);

        // RESOLVEOPTIONS WITH REQUIRE-OPTIONS: the locales, then options that must not be undefined,
        // through GetOptionsObject, the matcher, and no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var optionsArgument = Argument(arguments, 1);

        if (optionsArgument.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Intl.DisplayNames: the options are required");
        }

        var options = JsValue.Object(Base64Options(engine, optionsArgument));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal),
            [],
            static (locale, key) => [null]);

        var style = GetStringOption(engine, options, "style", ["narrow", "short", "long"], "long")!;
        var type = GetStringOption(engine, options, "type", ["language", "region", "script", "currency", "calendar", "dateTimeField"], null) ??
            throw engine.Error("TypeError", "Intl.DisplayNames: the type option is required");
        var fallback = GetStringOption(engine, options, "fallback", ["code", "none"], "code")!;
        var languageDisplay = GetStringOption(engine, options, "languageDisplay", ["dialect", "standard"], "dialect")!;

        return JsValue.Object(new JsDisplayNamesObject(prototype)
        {
            Locale = resolved.Locale,
            Language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en",
            Style = style,
            Type = type,
            Fallback = fallback,
            LanguageDisplay = type == "language" ? languageDisplay : null,
        });
    }

    /// <summary>ECMA-402's CanonicalCodeForDisplayNames (s12.5.1): a well-formed code of the type, case-regularized, or a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AF56E6
    // Broiler-Human:        PENDING
    private static string CanonicalCodeForDisplayNames(JsEngine engine, string type, string code)
    {
        static bool Letters(string text) => text.Length > 0 && JsLocaleTag.IsAlpha(text);

        static bool Digits(string text)
        {
            foreach (var c in text)
            {
                if (!char.IsAsciiDigit(c))
                {
                    return false;
                }
            }

            return text.Length > 0;
        }

        switch (type)
        {
            case "language":
                {
                    // UNICODE_LANGUAGE_ID IN ITS BCP 47 FORM: no extension, no private use, no underscore.
                    if (code.Contains('_', System.StringComparison.Ordinal) ||
                        JsLocaleTag.Parse(code) is not { } tag ||
                        tag.LanguageId().Length != code.Length)
                    {
                        throw engine.Error("RangeError", "Intl.DisplayNames: invalid language code: " + code);
                    }

                    return tag.Canonicalize(engine.Intl!).ToString();
                }

            case "region":
                if (!(code.Length == 2 && Letters(code)) && !(code.Length == 3 && Digits(code)))
                {
                    throw engine.Error("RangeError", "Intl.DisplayNames: invalid region code: " + code);
                }

                return code.ToUpperInvariant();

            case "script":
                if (code.Length != 4 || !Letters(code))
                {
                    throw engine.Error("RangeError", "Intl.DisplayNames: invalid script code: " + code);
                }

                return char.ToUpperInvariant(code[0]) + code[1..].ToLowerInvariant();

            case "calendar":
                if (!IsTypeSequence(code) || code.Contains('_', System.StringComparison.Ordinal))
                {
                    throw engine.Error("RangeError", "Intl.DisplayNames: invalid calendar code: " + code);
                }

                return code.ToLowerInvariant();

            case "dateTimeField":
                if (System.Array.IndexOf(DateTimeFieldCodes, code) < 0)
                {
                    throw engine.Error("RangeError", "Intl.DisplayNames: invalid date-time field code: " + code);
                }

                return code;

            default:
                if (!IsWellFormedCurrencyCode(code))
                {
                    throw engine.Error("RangeError", "Intl.DisplayNames: invalid currency code: " + code);
                }

                return code.ToUpperInvariant();
        }
    }

    /// <summary>The name [[Fields]] holds for a canonical code, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=997FE9
    // Broiler-Human:        PENDING
    private static string? DisplayName(JsEngine engine, JsDisplayNamesObject names, string code)
    {
        var data = engine.Intl!.LocaleInfo.DisplayNames;
        var prefix = names.Language + "|";
        var shortened = names.Style != "long";

        string? Named(string kind, string key) =>
            (shortened && data.TryGetValue(prefix + kind + "|" + key + "-alt-short", out var alternate)) ? alternate :
            data.TryGetValue(prefix + kind + "|" + key, out var name) ? name : null;

        switch (names.Type)
        {
            case "region":
                return Named("region", code);

            case "script":
                return data.TryGetValue(prefix + "script|" + code, out var script) ? script : null;

            case "calendar":
                return data.TryGetValue(prefix + "calendar|" + code, out var calendar) ? calendar : null;

            case "dateTimeField":
                return data.TryGetValue(prefix + "field|" + code + "-" + names.Style, out var field) ? field : null;

            case "currency":
                return engine.Intl!.Numbers.Currencies.TryGetValue(names.Language + "|" + code, out var currency) && currency.Name.Length != 0
                    ? currency.Name
                    : null;

            default:
                return LanguageName(engine, names, code, Named);
        }
    }

    /// <summary>
    /// A language code's name, composed as ICU's LocaleDisplayNames composes it: under the dialect
    /// display the whole code's own name where CLDR has one, then the language's name, and the script,
    /// region and variants it did not cover in CLDR's locale pattern. An unnamed part is its code under
    /// the code fallback, as ICU writes "xyz (Germany)", and makes the whole name nothing otherwise.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=867072
    // Broiler-Human:        PENDING
    private static string? LanguageName(
        JsEngine engine, JsDisplayNamesObject names, string code, System.Func<string, string, string?> named)
    {
        var tag = JsLocaleTag.Parse(code)!;
        var data = engine.Intl!.LocaleInfo.DisplayNames;
        var language = tag.Language;
        var script = tag.Script;
        var region = tag.Region;
        string? name = null;

        if (names.LanguageDisplay == "dialect")
        {
            // ONE DIALECT KEY, THE WHOLE CODE'S: a script and a region together are never split.
            if (script is not null && region is not null)
            {
                if (named("language", language + "-" + script + "-" + region) is { } full)
                {
                    (name, script, region) = (full, null, null);
                }
            }
            else if (script is not null)
            {
                if (named("language", language + "-" + script) is { } withScript)
                {
                    (name, script) = (withScript, null);
                }
            }
            else if (region is not null && named("language", language + "-" + region) is { } withRegion)
            {
                (name, region) = (withRegion, null);
            }
        }

        var substitute = names.Fallback == "code";
        name ??= named("language", language) ?? (substitute ? language : null);

        if (name is null)
        {
            return null;
        }

        var details = new System.Collections.Generic.List<string>();

        if (script is not null)
        {
            if (!data.TryGetValue(names.Language + "|script|" + script, out var scriptName))
            {
                if (!substitute)
                {
                    return null;
                }

                scriptName = script;
            }

            details.Add(scriptName);
        }

        if (region is not null)
        {
            if (named("region", region) is not { } regionName)
            {
                if (!substitute)
                {
                    return null;
                }

                regionName = region;
            }

            details.Add(regionName);
        }

        foreach (var variant in tag.Variants)
        {
            if (!data.TryGetValue(names.Language + "|variant|" + variant.ToUpperInvariant(), out var variantName))
            {
                if (!substitute)
                {
                    return null;
                }

                variantName = variant.ToUpperInvariant();
            }

            details.Add(variantName);
        }

        if (details.Count == 0)
        {
            return name;
        }

        var separator = data.TryGetValue(names.Language + "|pattern|localeSeparator", out var s) ? s : "{0}, {1}";
        var pattern = data.TryGetValue(names.Language + "|pattern|localePattern", out var p) ? p : "{0} ({1})";
        var joined = details[0];

        for (var i = 1; i < details.Count; i++)
        {
            joined = separator.Replace("{0}", joined, System.StringComparison.Ordinal).Replace("{1}", details[i], System.StringComparison.Ordinal);
        }

        return pattern.Replace("{0}", name, System.StringComparison.Ordinal).Replace("{1}", joined, System.StringComparison.Ordinal);
    }
}

/// <summary>An <c>Intl.DisplayNames</c>: its resolved options and the language whose names it reads.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6F2395
// Broiler-Human:        PENDING
internal sealed class JsDisplayNamesObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AD6CCF
    // Broiler-Human:        PENDING
    internal JsDisplayNamesObject(JsObject prototype)
        : base(prototype)
    {
    }

    /// <summary>The resolved locale.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDBB2D
    // Broiler-Human:        PENDING
    internal string Locale { get; init; } = string.Empty;

    /// <summary>The language whose names are read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D9CAFA
    // Broiler-Human:        PENDING
    internal string Language { get; init; } = "en";

    /// <summary><c>narrow</c>, <c>short</c> or <c>long</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D3FBA0
    // Broiler-Human:        PENDING
    internal string Style { get; init; } = "long";

    /// <summary>The display name type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6B9B9B
    // Broiler-Human:        PENDING
    internal string Type { get; init; } = "language";

    /// <summary><c>code</c> or <c>none</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D1331D
    // Broiler-Human:        PENDING
    internal string Fallback { get; init; } = "code";

    /// <summary><c>dialect</c> or <c>standard</c> for the language type, and nothing otherwise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=10E9F7
    // Broiler-Human:        PENDING
    internal string? LanguageDisplay { get; init; }
}
