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

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;

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
/// <para>
/// <b>Since SJL-9.4 the core is the shared Intl library's <c>ListFormatter</c></b> (JSD-0082,
/// sub-area (c)): [[Templates]] read at the data locale, and CreatePartsFromList with its one unit per
/// template step charged on the engine's meter (decision D18). The options, StringListFromIterable with
/// its one unit per element, and the parts array with its one unit per part stay here.
/// </para>
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

    /// <summary>The <c>type</c> option's values, in <c>ListFormatType</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FC69B9
    // Broiler-Human:        PENDING
    private static readonly string[] ListTypeNames = ["conjunction", "disjunction", "unit"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=49AD4C
    // Broiler-Human:        PENDING
    private void SetupListFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        ListFormatPrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "ListFormat", 0,
            static (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Intl.ListFormat requires 'new'"),
            static (engine, newTarget, arguments) => NewListFormat(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        ListFormatConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

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
            return JsValue.String(format.Formatter.Format(list, engine.WorkCharge));
        });

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = ListFormatOfThis(engine, thisValue, "formatToParts");
            var list = StringListFromIterable(engine, Argument(arguments, 0));
            var values = new List<JsValue>();

            foreach (var (type, value) in format.Formatter.FormatToParts(list, engine.WorkCharge))
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
            var formatter = format.Formatter;
            options.DefineOrdinary("locale", JsValue.String(formatter.Locale));
            options.DefineOrdinary("type", JsValue.String(ListTypeNames[(int)formatter.Type]));
            options.DefineOrdinary("style", JsValue.String(FormatStyleNames[(int)formatter.Style]));
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.ListFormat"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("ListFormat", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The list format <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F2B0DE
    // Broiler-Human:        PENDING
    private static JsListFormatObject ListFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsListFormatObject ??
        throw engine.Error("TypeError", "Intl.ListFormat.prototype." + member + " requires that 'this' be an Intl.ListFormat");

    /// <summary>ECMA-402's <c>Intl.ListFormat</c> constructor (s14.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B71E35
    // Broiler-Human:        PENDING
    internal static JsValue NewListFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.ListFormatPrototype!);

        // RESOLVEOPTIONS: the locales, the options as an object (undefined allowed, no coercion of
        // primitives, GetOptionsObject), the matcher, and no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = JsValue.Object(Base64Options(engine, Argument(arguments, 1)));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");

        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(StringComparer.Ordinal), [], static (locale, key) => [null]);

        var type = GetStringOption(engine, options, "type", ListTypeNames, "conjunction")!;
        var style = GetStringOption(engine, options, "style", FormatStyleNames, "long")!;

        // [[TEMPLATES]]: CLDR's patterns for the type and style at the data locale, the library's.
        var formatter = ListFormatter.Create(engine.Intl!.FormatData, resolved.Locale, resolved.DataLocale,
            (ListFormatType)Array.IndexOf(ListTypeNames, type), (FormatStyle)Array.IndexOf(FormatStyleNames, style));
        return JsValue.Object(new JsListFormatObject(prototype, formatter));
    }

    /// <summary>ECMA-402's StringListFromIterable (s14.5.5): every value a String, or the iterator closed and a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7BEC6A
    // Broiler-Human:        PENDING
    private static List<string> StringListFromIterable(JsEngine engine, JsValue iterable)
    {
        var list = new List<string>();

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
}

/// <summary>An <c>Intl.ListFormat</c>: the shared library's formatter, which holds its locale, type, style and the patterns they select.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBDF77
// Broiler-Human:        PENDING
internal sealed class JsListFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=62583B
    // Broiler-Human:        PENDING
    internal JsListFormatObject(JsObject prototype, ListFormatter formatter) : base(prototype) => Formatter = formatter;

    /// <summary>The formatter CreatePartsFromList runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9C61D7
    // Broiler-Human:        PENDING
    internal ListFormatter Formatter { get; }
}
