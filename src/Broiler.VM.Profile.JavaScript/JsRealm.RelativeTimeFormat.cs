// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           3
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Intl;

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
/// <para>
/// <b>Since SJL-9.4 the core is the shared Intl library's <c>RelativeTimeFormatter</c></b> (JSD-0082,
/// sub-area (c)), which holds both and reads the patterns at the data locale; the options, the
/// value's and the unit's conversions and the parts array stay here. Its number format and plural
/// rules are made as the draft makes them, so the adapter charges the one unit per element their
/// CanonicalizeLocaleList of the resolved locale charged here (decision D18).
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

    /// <summary>The <c>style</c> option's values of the relative-time and list formats, in <c>FormatStyle</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=082A57
    // Broiler-Human:        PENDING
    internal static readonly string[] FormatStyleNames = ["long", "short", "narrow"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=520C16
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
            RelativeTimeCompleted(engine, format.Formatter.Format(value, unit, out var text, engine.WorkCharge), unit);
            return JsValue.String(text);
        });

        Method(prototype, "formatToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = RelativeTimeFormatOfThis(engine, thisValue, "formatToParts");
            var value = engine.ToNumber(Argument(arguments, 0));
            var unit = engine.ToStringValue(Argument(arguments, 1));
            var values = new System.Collections.Generic.List<JsValue>();
            RelativeTimeCompleted(engine, format.Formatter.FormatToParts(value, unit, out var parts, engine.WorkCharge), unit);

            foreach (var (type, text, partUnit) in parts)
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
            var formatter = format.Formatter;
            options.DefineOrdinary("locale", JsValue.String(formatter.Locale));
            options.DefineOrdinary("style", JsValue.String(FormatStyleNames[(int)formatter.Style]));
            options.DefineOrdinary("numeric", JsValue.String(formatter.Numeric == RelativeTimeNumeric.Auto ? "auto" : "always"));
            options.DefineOrdinary("numberingSystem", JsValue.String(formatter.NumberingSystem));
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C33D1E
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

        var data = engine.Intl!.FormatData;
        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal) { ["nu"] = numberingSystem },
            ["nu"],
            (locale, key) => key == "nu" ? [.. NumberFormatter.NumberingSystemKeyValues(data, locale)] : [null]);

        var style = GetStringOption(engine, options, "style", FormatStyleNames, "long")!;
        var numeric = GetStringOption(engine, options, "numeric", ["always", "auto"], "always")!;

        // [[NUMBERFORMAT]] AND [[PLURALRULES]], CONSTRUCTED AS THE DRAFT CONSTRUCTS THEM, by the library:
        // each from a list of the one resolved locale, whose CanonicalizeLocaleList charged one unit
        // here when this family made them through its own constructors.
        engine.Charge(2);
        var formatter = RelativeTimeFormatter.Create(data, resolved.Locale, resolved.DataLocale,
            (FormatStyle)System.Array.IndexOf(FormatStyleNames, style),
            numeric == "auto" ? RelativeTimeNumeric.Auto : RelativeTimeNumeric.Always,
            resolved.Keys["nu"] ?? "latn");
        return JsValue.Object(new JsRelativeTimeFormatObject(prototype, formatter));
    }

    /// <summary>
    /// PartitionRelativeTimePattern's refusals as the errors they are: a value that is not finite
    /// (step 1) and a unit SingularRelativeTimeUnit refuses (step 3), each a <c>RangeError</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s18.5.2; IP=Low; Security=Low; Resources=0; Fingerprint=F3CB30
    // Broiler-Human:        PENDING
    private static void RelativeTimeCompleted(JsEngine engine, IntlStatus status, string unit)
    {
        switch (status)
        {
            case IntlStatus.Completed:
                return;
            case IntlStatus.RelativeTimeValueNotFinite:
                throw engine.Error("RangeError", "Intl.RelativeTimeFormat: the value must be finite");
            default:
                throw engine.Error("RangeError", "Intl.RelativeTimeFormat: invalid unit: " + unit);
        }
    }
}

/// <summary>An <c>Intl.RelativeTimeFormat</c>: the shared library's formatter, which holds its resolved options and the number format and plural rules it writes with.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44505C
// Broiler-Human:        PENDING
internal sealed class JsRelativeTimeFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=53B721
    // Broiler-Human:        PENDING
    internal JsRelativeTimeFormatObject(JsObject prototype, RelativeTimeFormatter formatter) : base(prototype) => Formatter = formatter;

    /// <summary>The formatter PartitionRelativeTimePattern runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=452DE7
    // Broiler-Human:        PENDING
    internal RelativeTimeFormatter Formatter { get; }
}
