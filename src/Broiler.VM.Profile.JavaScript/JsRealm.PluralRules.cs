// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           5
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
/// <c>Intl.PluralRules</c> (ECMA-402 s17; JSD-0047): a number's cardinal or ordinal plural category,
/// and a range's, by CLDR's rules for the format's language.
/// </summary>
/// <remarks>
/// <para>
/// <b>A plural rules object is a decimal number format that selects instead of writing.</b> Its digit
/// options are SetNumberFormatDigitOptions's and its rounding is FormatNumericToString's, both
/// <see cref="JsNumberFormatter"/>'s, so <c>select</c> reads the operands of the string a decimal
/// <c>Intl.NumberFormat</c> with the same options would write.
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

    /// <summary>The plural categories in the order <c>resolvedOptions</c> lists them (s17.3.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C182CD
    // Broiler-Human:        PENDING
    private static readonly string[] PluralCategoryOrder = ["zero", "one", "two", "few", "many", "other"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=068CFE
    // Broiler-Human:        PENDING
    private void SetupPluralRules(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        PluralRulesPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "PluralRules",
            0,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.PluralRules requires 'new'"),
            static (engine, newTarget, arguments) => NewPluralRules(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        PluralRulesConstructor = constructor;

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

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "resolvedOptions");
            var formatter = rules.Formatter;
            var options = new JsObject(engine.Realm.ObjectPrototype);

            void Put(string name, string? value)
            {
                if (value is not null)
                {
                    options.DefineOrdinary(name, JsValue.String(value));
                }
            }

            void PutNumber(string name, int? value)
            {
                if (value is not null)
                {
                    options.DefineOrdinary(name, JsValue.Number(value.Value));
                }
            }

            var categories = new System.Collections.Generic.List<JsValue>();

            foreach (var category in PluralCategoriesOf(engine, rules))
            {
                categories.Add(JsValue.String(category));
            }

            // TABLE 32, IN ITS ORDER.
            Put("locale", formatter.Locale);
            Put("type", rules.Type);
            Put("notation", formatter.Notation);
            Put("compactDisplay", formatter.CompactDisplay);
            PutNumber("minimumIntegerDigits", formatter.MinimumIntegerDigits);
            PutNumber("minimumFractionDigits", formatter.MinimumFractionDigits);
            PutNumber("maximumFractionDigits", formatter.MaximumFractionDigits);
            PutNumber("minimumSignificantDigits", formatter.MinimumSignificantDigits);
            PutNumber("maximumSignificantDigits", formatter.MaximumSignificantDigits);
            options.DefineOrdinary("pluralCategories", JsValue.Object(engine.Realm.NewArray(categories)));
            PutNumber("roundingIncrement", formatter.RoundingIncrement);
            Put("roundingMode", formatter.RoundingMode);
            Put("roundingPriority", formatter.ComputedRoundingPriority);
            Put("trailingZeroDisplay", formatter.TrailingZeroDisplay);
            return JsValue.Object(options);
        });

        Method(prototype, "select", 1, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "select");
            var n = ToIntlMathematicalValue(engine, Argument(arguments, 0));
            return JsValue.String(ResolvePlural(engine, rules, n).Category);
        });

        Method(prototype, "selectRange", 2, static (engine, thisValue, arguments) =>
        {
            var rules = PluralRulesOfThis(engine, thisValue, "selectRange");
            var start = Argument(arguments, 0);
            var end = Argument(arguments, 1);

            if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
            {
                throw engine.Error("TypeError", "Intl.PluralRules: a range needs a start and an end");
            }

            var x = ToIntlMathematicalValue(engine, start);
            var y = ToIntlMathematicalValue(engine, end);
            return JsValue.String(ResolvePluralRange(engine, rules, x, y));
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.PluralRules"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "PluralRules",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The plural rules <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=192815
    // Broiler-Human:        PENDING
    private static JsPluralRulesObject PluralRulesOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsPluralRulesObject ??
        throw engine.Error("TypeError", "Intl.PluralRules.prototype." + member + " requires that 'this' be an Intl.PluralRules");

    /// <summary>ECMA-402's <c>Intl.PluralRules</c> constructor (s17.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2AB2C1
    // Broiler-Human:        PENDING
    internal static JsValue NewPluralRules(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.PluralRulesPrototype!);

        // RESOLVEOPTIONS: the locales, the options coerced, the matcher, and no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = CoerceOptionsToObject(engine, Argument(arguments, 1));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal),
            [],
            static (locale, key) => [null]);

        var type = GetStringOption(engine, options, "type", ["cardinal", "ordinal"], "cardinal")!;
        var notation = GetStringOption(engine, options, "notation", ["standard", "scientific", "engineering", "compact"], "standard")!;
        var compactDisplay = GetStringOption(engine, options, "compactDisplay", ["short", "long"], "short")!;
        var digitOptions = SetNumberFormatDigitOptions(engine, options, 0, 3, notation);
        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";

        var formatter = new JsNumberFormatter(engine.Intl!.Numbers, language, "latn")
        {
            Locale = resolved.Locale,
            MinimumIntegerDigits = digitOptions.MinimumIntegerDigits,
            MinimumFractionDigits = digitOptions.MinimumFractionDigits,
            MaximumFractionDigits = digitOptions.MaximumFractionDigits,
            MinimumSignificantDigits = digitOptions.MinimumSignificantDigits,
            MaximumSignificantDigits = digitOptions.MaximumSignificantDigits,
            RoundingType = digitOptions.RoundingType,
            ComputedRoundingPriority = digitOptions.ComputedRoundingPriority,
            RoundingIncrement = digitOptions.RoundingIncrement,
            RoundingMode = digitOptions.RoundingMode,
            TrailingZeroDisplay = digitOptions.TrailingZeroDisplay,
            Notation = notation,
            CompactDisplay = notation == "compact" ? compactDisplay : null,
        };

        return JsValue.Object(new JsPluralRulesObject(prototype, formatter, type, language));
    }

    /// <summary>
    /// ECMA-402's ResolvePlural (s17.5.2): <c>other</c> for not-a-number and the infinities, and
    /// otherwise PluralRuleSelect over FormatNumericToString's string.
    /// </summary>
    /// <remarks>
    /// PluralRuleSelect reads the operands of that string. The compact and scientific exponents, the
    /// operands <c>c</c> and <c>e</c>, are 0: the string is the whole value's, and of the supported
    /// languages' rules none reads them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EAEA74
    // Broiler-Human:        PENDING
    private static (string Category, string Formatted) ResolvePlural(JsEngine engine, JsPluralRulesObject rules, JsDecimal n)
    {
        if (!n.IsFinite)
        {
            return ("other", n.Kind == JsDecimalKind.NaN ? "NaN" : n.Kind == JsDecimalKind.PositiveInfinity ? "Infinity" : "-Infinity");
        }

        var (_, formatted) = rules.Formatter.FormatNumericToString(engine, n);
        var operands = JsPluralOperands.Of(formatted, 0);
        return (engine.Intl!.Numbers.Plural(rules.Language, operands, rules.Type == "ordinal"), formatted);
    }

    /// <summary>
    /// ECMA-402's ResolvePluralRange (s17.5.4): a <c>RangeError</c> at not-a-number, the start's
    /// category where both ends write the same string, and otherwise CLDR's range rule.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E969F1
    // Broiler-Human:        PENDING
    private static string ResolvePluralRange(JsEngine engine, JsPluralRulesObject rules, JsDecimal x, JsDecimal y)
    {
        if (x.Kind == JsDecimalKind.NaN || y.Kind == JsDecimalKind.NaN)
        {
            throw engine.Error("RangeError", "Intl.PluralRules: a range cannot start or end at NaN");
        }

        var start = ResolvePlural(engine, rules, x);
        var end = ResolvePlural(engine, rules, y);

        if (start.Formatted == end.Formatted)
        {
            return start.Category;
        }

        return engine.Intl!.Numbers.PluralRange(rules.Language, start.Category, end.Category);
    }

    /// <summary>Every category PluralRuleSelect can answer for the language and type, in s17.3.2's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=76A751
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> PluralCategoriesOf(JsEngine engine, JsPluralRulesObject rules)
    {
        var numbers = engine.Intl!.Numbers;
        var categories = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal) { "other" };

        if ((rules.Type == "ordinal" ? numbers.Ordinals : numbers.Plurals).TryGetValue(rules.Language, out var list))
        {
            foreach (var (category, _) in list)
            {
                categories.Add(category);
            }
        }

        var ordered = new System.Collections.Generic.List<string>();

        foreach (var category in PluralCategoryOrder)
        {
            if (categories.Contains(category))
            {
                ordered.Add(category);
            }
        }

        return ordered;
    }
}

/// <summary>An <c>Intl.PluralRules</c>: its type, its language, and the decimal format whose strings it reads.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=66F408
// Broiler-Human:        PENDING
internal sealed class JsPluralRulesObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=44D402
    // Broiler-Human:        PENDING
    internal JsPluralRulesObject(JsObject prototype, JsNumberFormatter formatter, string type, string language)
        : base(prototype)
    {
        Formatter = formatter;
        Type = type;
        Language = language;
    }

    /// <summary>The decimal format holding the locale, the notation and the digit options.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60C8BD
    // Broiler-Human:        PENDING
    internal JsNumberFormatter Formatter { get; }

    /// <summary><c>cardinal</c> or <c>ordinal</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=35ED61
    // Broiler-Human:        PENDING
    internal string Type { get; }

    /// <summary>The language whose rules select.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DE92F3
    // Broiler-Human:        PENDING
    internal string Language { get; }
}
