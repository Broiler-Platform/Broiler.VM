// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           6
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.ListFormat</c> (ECMA-402 s14; JSD-0048): a list of strings joined by CLDR's list patterns
/// for the format's language, type and style.
/// </summary>
/// <remarks>
/// <b>One template per type and style.</b> ECMA-402 lets an implementation choose among several
/// templates by the elements, as Spanish chooses <c>y</c> or <c>e</c> by the next word. German and
/// English have no such choice, so the index into [[Templates]] is always the one template CLDR
/// gives.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.ListFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=14E2DA
    // Broiler-Human:        PENDING
    internal JsObject? ListFormatPrototype { get; private set; }

    /// <summary><c>%Intl.ListFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0A0F04
    // Broiler-Human:        PENDING
    internal JsNativeFunction? ListFormatConstructor { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=29B5A1
    // Broiler-Human:        PENDING
    private void SetupListFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        ListFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "ListFormat",
            0,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.ListFormat requires 'new'"),
            static (engine, newTarget, arguments) => NewListFormat(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        ListFormatConstructor = constructor;

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
            var format = ListFormatOfThis(engine, thisValue, "format");
            var list = StringListFromIterable(engine, Argument(arguments, 0));
            var text = new System.Text.StringBuilder();

            foreach (var (_, value) in CreatePartsFromList(engine, format, list))
            {
                text.Append(value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = ListFormatOfThis(engine, thisValue, "formatToParts");
            var list = StringListFromIterable(engine, Argument(arguments, 0));
            var values = new System.Collections.Generic.List<JsValue>();

            foreach (var (type, value) in CreatePartsFromList(engine, format, list))
            {
                engine.Charge(1);
                var item = new JsObject(engine.Realm.ObjectPrototype);
                item.DefineOrdinary("type", JsValue.String(type));
                item.DefineOrdinary("value", JsValue.String(value));
                values.Add(JsValue.Object(item));
            }

            return JsValue.Object(engine.Realm.NewArray(values));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = ListFormatOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 25, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(format.Locale));
            options.DefineOrdinary("type", JsValue.String(format.Type));
            options.DefineOrdinary("style", JsValue.String(format.Style));
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.ListFormat"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "ListFormat",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The list format <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F2B0DE
    // Broiler-Human:        PENDING
    private static JsListFormatObject ListFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsListFormatObject ??
        throw engine.Error("TypeError", "Intl.ListFormat.prototype." + member + " requires that 'this' be an Intl.ListFormat");

    /// <summary>ECMA-402's <c>Intl.ListFormat</c> constructor (s14.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C90F98
    // Broiler-Human:        PENDING
    internal static JsValue NewListFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.ListFormatPrototype!);

        // RESOLVEOPTIONS: the locales, the options as an object (undefined allowed, no coercion of
        // primitives, GetOptionsObject), the matcher, and no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = JsValue.Object(Base64Options(engine, Argument(arguments, 1)));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal),
            [],
            static (locale, key) => [null]);

        var type = GetStringOption(engine, options, "type", ["conjunction", "disjunction", "unit"], "conjunction")!;
        var style = GetStringOption(engine, options, "style", ["long", "short", "narrow"], "long")!;
        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";

        // [[TEMPLATES]]: CLDR's type for the type and style.
        var key = language + "|" + type switch { "disjunction" => "or", "unit" => "unit", _ => "standard" } +
            style switch { "short" => "-short", "narrow" => "-narrow", _ => string.Empty };

        var templates = engine.Intl!.LocaleInfo.ListPatterns.TryGetValue(key, out var found)
            ? found
            : ("{0}, {1}", "{0}, {1}", "{0}, {1}", "{0}, {1}");

        return JsValue.Object(new JsListFormatObject(prototype, resolved.Locale, type, style, templates));
    }

    /// <summary>ECMA-402's StringListFromIterable (s14.5.5): every value a String, or the iterator closed and a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C51195
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> StringListFromIterable(JsEngine engine, JsValue iterable)
    {
        var list = new System.Collections.Generic.List<string>();

        if (iterable.Type == JsType.Undefined)
        {
            return list;
        }

        var record = engine.GetIterator(iterable);

        while (engine.TryIterateNext(record, out var next))
        {
            if (!next.IsString)
            {
                engine.CloseIteratorQuietly(record);
                throw engine.Error("TypeError", "Intl.ListFormat: every element of the list must be a String");
            }

            engine.Charge(1);
            list.Add(next.AsString());
        }

        return list;
    }

    /// <summary>
    /// ECMA-402's CreatePartsFromList (s14.5.2): nothing, the element, the pair pattern, or the end
    /// pattern folded leftward through the middle patterns to the start pattern.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2B0E36
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(string Type, string Value)> CreatePartsFromList(
        JsEngine engine, JsListFormatObject format, System.Collections.Generic.List<string> list)
    {
        var parts = new System.Collections.Generic.List<(string Type, string Value)>();
        var size = list.Count;

        if (size == 0)
        {
            return parts;
        }

        if (size == 1)
        {
            parts.Add(("element", list[0]));
            return parts;
        }

        if (size == 2)
        {
            return DeconstructPattern(format.Templates.Pair, [("element", list[0])], [("element", list[1])]);
        }

        parts.Add(("element", list[size - 1]));

        for (var i = size - 2; i >= 0; i--)
        {
            engine.Charge(1);
            var pattern = i == 0 ? format.Templates.Start : i < size - 2 ? format.Templates.Middle : format.Templates.End;
            parts = DeconstructPattern(pattern, [("element", list[i])], parts);
        }

        return parts;
    }

    /// <summary>
    /// ECMA-402's DeconstructPattern (s14.5.1) over a list pattern: each non-empty literal between the
    /// placeables, and the parts of <c>{0}</c> and <c>{1}</c> in their places.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D512C9
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(string Type, string Value)> DeconstructPattern(
        string pattern,
        System.Collections.Generic.List<(string Type, string Value)> zero,
        System.Collections.Generic.List<(string Type, string Value)> one)
    {
        var result = new System.Collections.Generic.List<(string Type, string Value)>();
        var at = 0;

        while (at < pattern.Length)
        {
            var open = pattern.IndexOf('{', at);

            if (open < 0)
            {
                result.Add(("literal", pattern[at..]));
                break;
            }

            var close = pattern.IndexOf('}', open);

            if (open > at)
            {
                result.Add(("literal", pattern[at..open]));
            }

            result.AddRange(pattern[(open + 1)..close] == "0" ? zero : one);
            at = close + 1;
        }

        return result;
    }
}

/// <summary>An <c>Intl.ListFormat</c>: its locale, type, style, and the patterns they select.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBDF77
// Broiler-Human:        PENDING
internal sealed class JsListFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=536BEA
    // Broiler-Human:        PENDING
    internal JsListFormatObject(
        JsObject prototype, string locale, string type, string style, (string Start, string Middle, string End, string Pair) templates)
        : base(prototype)
    {
        Locale = locale;
        Type = type;
        Style = style;
        Templates = templates;
    }

    /// <summary>The resolved locale.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=04656B
    // Broiler-Human:        PENDING
    internal string Locale { get; }

    /// <summary><c>conjunction</c>, <c>disjunction</c> or <c>unit</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=35ED61
    // Broiler-Human:        PENDING
    internal string Type { get; }

    /// <summary><c>long</c>, <c>short</c> or <c>narrow</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=528E3C
    // Broiler-Human:        PENDING
    internal string Style { get; }

    /// <summary>CLDR's start, middle, end and two-element patterns.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3F580A
    // Broiler-Human:        PENDING
    internal (string Start, string Middle, string End, string Pair) Templates { get; }
}
