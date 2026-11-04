// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           3
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.PluralRules</c> (ECMA-402 s17; JSD-0047): a number's cardinal or ordinal plural category,
/// and a range's, by CLDR's rules for the format's language.
/// </summary>
/// <remarks>
/// <para>
/// <b>A plural rules object is a decimal number format that selects instead of writing.</b> Its digit
/// options are SetNumberFormatDigitOptions's and its rounding is FormatNumericToString's, so
/// <c>select</c> reads the operands of the string a decimal <c>Intl.NumberFormat</c> with the same
/// options would write. Since SJL-9.4 (JSD-0082, sub-area (b)) the record, the rounding, the plural
/// rules and the plural ranges are the shared Intl library's <c>PluralRulesRecord</c> and
/// <c>PluralEngine</c>; the options are read here, in the constructor's order, and
/// <c>ResolvePlural</c> is charged on the engine's meter through FormatNumericToString (decision D18).
/// In compact notation the library passes the compact exponent as the operands <c>c</c> and <c>e</c>
/// (decision D20), where this family passed 0; no rule of its data reads either (row P4).
/// </para>
/// <para>
/// <i>Revised 2026-11-03, at SJL-9.4: the first paragraph read "both
/// <c>JsNumberFormatter</c>'s"; that type is deleted.</i>
/// </para>
/// <para>
/// <b>A range's category is CLDR's plural ranges for the language</b>, for both types. CLDR states
/// ranges for cardinals only, and ICU resolves an ordinal range by the same data.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.PluralRules.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75DBB9
    // Broiler-Human:        PENDING
    internal JsObject? PluralRulesPrototype { get; private set; }

    /// <summary><c>%Intl.PluralRules%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6F2654
    // Broiler-Human:        PENDING
    internal JsNativeFunction? PluralRulesConstructor { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=871E3F
    // Broiler-Human:        PENDING
    private void SetupPluralRules(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        PluralRulesPrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "PluralRules", 0,
            static (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Intl.PluralRules requires 'new'"),
            static (engine, newTarget, arguments) => NewPluralRules(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        PluralRulesConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "resolvedOptions").Rules;
            var digits = rules.DigitOptions;
            var options = new JsObject(engine.Realm.ObjectPrototype);

            void PutNumber(string name, int? value)
            {
                if (value is not null)
                {
                    options.DefineOrdinary(name, JsValue.Number(value.Value));
                }
            }

            var categories = new List<JsValue>();

            foreach (var category in rules.PluralCategories)
            {
                categories.Add(JsValue.String(category));
            }

            // TABLE 32, IN ITS ORDER.
            options.DefineOrdinary("locale", JsValue.String(rules.Locale));
            options.DefineOrdinary("type", JsValue.String(rules.Type == PluralRuleType.Ordinal ? "ordinal" : "cardinal"));
            options.DefineOrdinary("notation", JsValue.String(NotationNames[(int)rules.Notation]));

            if (rules.Notation == NumberNotation.Compact)
            {
                options.DefineOrdinary("compactDisplay", JsValue.String(CompactDisplayNames[(int)rules.CompactDisplay]));
            }

            PutNumber("minimumIntegerDigits", digits.MinimumIntegerDigits);
            PutNumber("minimumFractionDigits", digits.MinimumFractionDigits);
            PutNumber("maximumFractionDigits", digits.MaximumFractionDigits);
            PutNumber("minimumSignificantDigits", digits.MinimumSignificantDigits);
            PutNumber("maximumSignificantDigits", digits.MaximumSignificantDigits);
            options.DefineOrdinary("pluralCategories", JsValue.Object(engine.Realm.NewArray(categories)));
            PutDigitTail(options, digits);
            return JsValue.Object(options);
        });

        Method(prototype, "select", 1, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "select").Rules;
            var n = ToIntlMathematicalValue(engine, Argument(arguments, 0));
            return JsValue.String(rules.ResolvePlural(n, engine.WorkCharge).PluralCategory);
        });

        Method(prototype, "selectRange", 2, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "selectRange").Rules;
            var start = Argument(arguments, 0);
            var end = Argument(arguments, 1);

            if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
            {
                throw engine.Error("TypeError", "Intl.PluralRules: a range needs a start and an end");
            }

            var x = ToIntlMathematicalValue(engine, start);
            var y = ToIntlMathematicalValue(engine, end);

            // RESOLVEPLURALRANGE: a range that starts or ends at NaN is the RangeError of its step 1.
            return rules.ResolvePluralRange(x, y, out var category, engine.WorkCharge) == IntlStatus.Completed
                ? JsValue.String(category)
                : throw engine.Error("RangeError", "Intl.PluralRules: a range cannot start or end at NaN");
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.PluralRules"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("PluralRules", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The plural rules <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=192815
    // Broiler-Human:        PENDING
    private static JsPluralRulesObject PluralRulesOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsPluralRulesObject ??
        throw engine.Error("TypeError", "Intl.PluralRules.prototype." + member + " requires that 'this' be an Intl.PluralRules");

    /// <summary>ECMA-402's <c>Intl.PluralRules</c> constructor (s17.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44F706
    // Broiler-Human:        PENDING
    internal static JsValue NewPluralRules(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.PluralRulesPrototype!);

        // RESOLVEOPTIONS: the locales, the options coerced, the matcher, and no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = CoerceOptionsToObject(engine, Argument(arguments, 1));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(StringComparer.Ordinal), [], static (locale, key) => [null]);

        var type = GetStringOption(engine, options, "type", ["cardinal", "ordinal"], "cardinal")!;
        var notation = (NumberNotation)Array.IndexOf(NotationNames, GetStringOption(engine, options, "notation", NotationNames, "standard"));
        var compactDisplay = (CompactDisplay)Array.IndexOf(CompactDisplayNames, GetStringOption(engine, options, "compactDisplay", CompactDisplayNames, "short"));
        var digitOptions = SetNumberFormatDigitOptions(engine, options, new FractionDigitDefaults(0, 3), notation);

        // THE RECORD THE LIBRARY TAKES: the rules of the data locale's language (the library's design,
        // section 1, sub-area (b)), read through the composite as every formatter reads.
        var rules = PluralRulesRecord.Create(engine.Intl!.FormatData, resolved.Locale, resolved.DataLocale,
            type == "ordinal" ? PluralRuleType.Ordinal : PluralRuleType.Cardinal, notation, compactDisplay, digitOptions);
        return JsValue.Object(new JsPluralRulesObject(prototype, rules));
    }
}

/// <summary>An <c>Intl.PluralRules</c>: the shared library's record, which holds its locale, type, notation and digit options.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=66F408
// Broiler-Human:        PENDING
internal sealed class JsPluralRulesObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B3A9C
    // Broiler-Human:        PENDING
    internal JsPluralRulesObject(JsObject prototype, PluralRulesRecord rules) : base(prototype) => Rules = rules;

    /// <summary>The record ResolvePlural and ResolvePluralRange read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=161EDE
    // Broiler-Human:        PENDING
    internal PluralRulesRecord Rules { get; }
}
