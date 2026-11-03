// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           3
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;

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
/// <para>
/// <b>Since SJL-9.4 the core is the shared Intl library's <c>DisplayNamesCore</c></b> (JSD-0082,
/// sub-area (c)): CanonicalCodeForDisplayNames, the names read at the data locale and their
/// composition. The options and the code's ToString stay here; display names charge nothing in this
/// family, so the adapter hands the library no meter (row L43, decision D18).
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

    /// <summary>The <c>type</c> option's values, in <c>DisplayNamesType</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3533EF
    // Broiler-Human:        PENDING
    private static readonly string[] DisplayNamesTypeNames = ["language", "region", "script", "currency", "calendar", "dateTimeField"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=42ABB1
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
            static (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Intl.DisplayNames requires 'new'"),
            static (engine, newTarget, arguments) => NewDisplayNames(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        DisplayNamesConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        Method(prototype, "of", 1, static (engine, thisValue, arguments) =>
        {
            var names = DisplayNamesOfThis(engine, thisValue, "of").Core;
            var code = engine.ToStringValue(Argument(arguments, 0));

            // THE CODE'S CHECK AND CANONICAL FORM, THE NAME AND THE FALLBACK are the library's (JSD-0082,
            // sub-area (c)); display names charge nothing here, so neither does the code's parse (row L43).
            if (names.Of(code, out var name) != IntlStatus.Completed)
            {
                throw engine.Error("RangeError", "Intl.DisplayNames: invalid " + DisplayNamesTypeNames[(int)names.Type] + " code: " + code);
            }

            return name is not null ? JsValue.String(name) : JsValue.Undefined;
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var names = DisplayNamesOfThis(engine, thisValue, "resolvedOptions").Core;
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 18, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(names.Locale));
            options.DefineOrdinary("style", JsValue.String(FormatStyleNames[(int)names.Style]));
            options.DefineOrdinary("type", JsValue.String(DisplayNamesTypeNames[(int)names.Type]));
            options.DefineOrdinary("fallback", JsValue.String(names.Fallback == DisplayNamesFallback.Code ? "code" : "none"));

            if (names.LanguageDisplay is { } display)
            {
                options.DefineOrdinary("languageDisplay", JsValue.String(display == LanguageDisplay.Dialect ? "dialect" : "standard"));
            }

            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.DisplayNames"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("DisplayNames", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The display names <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=87FC3D
    // Broiler-Human:        PENDING
    private static JsDisplayNamesObject DisplayNamesOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDisplayNamesObject ??
        throw engine.Error("TypeError", "Intl.DisplayNames.prototype." + member + " requires that 'this' be an Intl.DisplayNames");

    /// <summary>ECMA-402's <c>Intl.DisplayNames</c> constructor (s12.1.1): the options required, and read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9B0C84
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

        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(StringComparer.Ordinal), [], static (locale, key) => [null]);

        var style = GetStringOption(engine, options, "style", ["narrow", "short", "long"], "long")!;
        var type = GetStringOption(engine, options, "type", DisplayNamesTypeNames, null) ??
            throw engine.Error("TypeError", "Intl.DisplayNames: the type option is required");
        var fallback = GetStringOption(engine, options, "fallback", ["code", "none"], "code")!;
        var languageDisplay = GetStringOption(engine, options, "languageDisplay", ["dialect", "standard"], "dialect")!;

        // THE RECORD THE LIBRARY TAKES: the names are read at the data locale through the composite.
        var core = DisplayNamesCore.Create(engine.Intl!.FormatData, resolved.Locale, resolved.DataLocale,
            (FormatStyle)Array.IndexOf(FormatStyleNames, style),
            (DisplayNamesType)Array.IndexOf(DisplayNamesTypeNames, type),
            fallback == "none" ? DisplayNamesFallback.None : DisplayNamesFallback.Code,
            languageDisplay == "standard" ? LanguageDisplay.Standard : LanguageDisplay.Dialect);
        return JsValue.Object(new JsDisplayNamesObject(prototype, core));
    }
}

/// <summary>An <c>Intl.DisplayNames</c>: the shared library's core, which holds its resolved options and reads its names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6F2395
// Broiler-Human:        PENDING
internal sealed class JsDisplayNamesObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BDC8AB
    // Broiler-Human:        PENDING
    internal JsDisplayNamesObject(JsObject prototype, DisplayNamesCore core) : base(prototype) => Core = core;

    /// <summary>The core <c>of</c> and <c>resolvedOptions</c> read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ACA1A3
    // Broiler-Human:        PENDING
    internal DisplayNamesCore Core { get; }
}
