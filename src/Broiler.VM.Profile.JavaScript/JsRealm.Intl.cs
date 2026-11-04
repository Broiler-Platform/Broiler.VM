// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           12
// Human-reviewed:   0/28
// IP risk:          Low
// Security risk:    Medium
// Criteria:         4/0
// Resource impact:  3/10 max
// Unverified:       28
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>Intl</c> namespace and its first constructor, <c>Intl.Collator</c>, over the tables the
/// composition handed over (ECMA-402; JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Built only where the surface is admitted and the data is present</b>, which a descriptor
/// guarantees together: a door handed no data does not admit the surface. A realm without it has no
/// <c>Intl</c> and its locale-sensitive methods keep the fixed answers JSD-0027 section 1 states.
/// </para>
/// <para>
/// <b>The locales are the data's</b>: <c>de</c>, <c>de-DE</c>, <c>en</c> and <c>en-US</c>, with
/// <c>en-US</c> the default, so the answer does not depend on the host's culture. Every locale
/// collates by the CLDR root, German's <c>phonebk</c> and <c>search</c> tailorings aside, and the root's
/// <c>search</c> tailoring serves every other search.
/// </para>
/// <para>
/// <b>The locale core is the shared Intl library's since SJL-9.4</b> (JSD-0082, sub-area (a)):
/// canonicalisation through <c>LocaleId</c>, and the matchers, FilterLocales and ResolveLocale
/// through <c>LocaleNegotiation</c>, over the composition's tables the library decodes
/// (<see cref="JsIntlTables.LocaleData"/>). What reads a script value stays here: the walk over the
/// locales argument and each element's checks, the options and their order, and the charges this
/// family makes there (decision D18). The Collator's own steps and data are SJL-17's and stay.
/// </para>
/// <para>
/// <b>The comparison is the shared Intl library's since SJL-17.4</b> (JSD-0084, JSD-0086):
/// <see cref="UcaCollator"/> compares, over <see cref="UcaCollationData"/>, the composition's
/// collation tables the library decodes (<see cref="JsIntlTables.CollationData"/>), which also
/// answers which tailoring a collator compares by and the values of its relevant keys. What reads a
/// script value stays here: the constructor's option reads in the specification's order, each
/// checked as it is read, the resolved options, the bound <c>compare</c>, <c>localeCompare</c>'s
/// coercion and its realm's <c>%Intl.Collator%</c>, the errors, and the meter the library charges
/// in this family's units (decision E7). <i>(Noted 2026-11-04, at SJL-17.4: the paragraph above
/// says "The Collator's own steps and data are SJL-17's and stay"; the steps stay, and the
/// comparison, the tailoring's choice and the key values are the library's, as this paragraph
/// says. The paragraph above is kept as written.)</i>
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ECMA-402; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The locale every service falls back to (JSD-0027 section 5 item 1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=160344
    // Broiler-Human:        PENDING
    internal const string DefaultLocale = "en-US";

    /// <summary><c>%Intl.Collator.prototype%</c>, or nothing where the surface is not built.</summary>
    internal JsObject? CollatorPrototype { get; private set; }

    /// <summary><c>%Intl.Collator%</c>, or nothing where the surface is not built.</summary>
    internal JsNativeFunction? CollatorConstructor { get; private set; }

    /// <summary>Builds <c>Intl</c> and <c>Intl.Collator</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s8, s10; IP=Low; Security=Medium; Resources=3; Fingerprint=7CCD05
    // Broiler-Human:        PENDING
    private void SetupIntl()
    {
        var intl = new JsObject(ObjectPrototype);

        intl.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl"), JsPropertyAttributes.Configurable));

        Method(intl, "getCanonicalLocales", 1, static (engine, thisValue, arguments) =>
        {
            var locales = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            return JsValue.Object(engine.Realm.NewArray(locales.ConvertAll(static locale => JsValue.String(locale))));
        });

        GlobalObject.SetOwnProperty("Intl", JsProperty.Data(JsValue.Object(intl), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(intl, "supportedValuesOf", 1, static (engine, thisValue, arguments) => SupportedValuesOf(engine, Argument(arguments, 0)));

        SetupCollator(intl);
        SetupNumberFormat(intl);
        SetupDateTimeFormat(intl);
        SetupLocale(intl);
        SetupPluralRules(intl);
        SetupListFormat(intl);
        SetupRelativeTimeFormat(intl);
        SetupSegmenter(intl);
        SetupDurationFormat(intl);
        SetupDisplayNames(intl);
    }

    // ---- Intl.Collator ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10; IP=Low; Security=Medium; Resources=3; Fingerprint=BFF221
    // Broiler-Human:        PENDING
    private void SetupCollator(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        CollatorPrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "Collator", 0,
            // CALLED WITHOUT `new`, THE ACTIVE FUNCTION IS THE NEW TARGET (ECMA-402 s10.1.1 step 1).
            static (engine, thisValue, arguments) => NewCollator(engine, JsValue.Object(engine.Realm.CollatorConstructor!), arguments),
            static (engine, newTarget, arguments) => NewCollator(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        CollatorConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        prototype.SetOwnProperty("compare", JsProperty.Accessor(Native("get compare", 0, static (engine, thisValue, arguments) =>
        {
            var collator = CollatorOfThis(engine, thisValue, "compare");

            // [[BoundCompare]]: MADE ONCE, an anonymous function of length 2 that remembers its
            // collator, so `c.compare === c.compare`.
            if (collator.BoundCompare is null)
            {
                var bound = new JsNativeFunction(engine.Realm, engine.Realm.FunctionPrototype, string.Empty, 2, (inner, receiver, values) =>
                {
                    var x = inner.ToStringValue(Argument(values, 0));
                    var y = inner.ToStringValue(Argument(values, 1));
                    return JsValue.Number(CompareStrings(inner, collator, x, y));
                });

                collator.BoundCompare = bound;
            }

            return JsValue.Object(collator.BoundCompare);
        }), null, JsPropertyAttributes.Configurable));

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var collator = CollatorOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            options.DefineOrdinary("locale", JsValue.String(collator.Locale));
            options.DefineOrdinary("usage", JsValue.String(collator.Usage));
            options.DefineOrdinary("sensitivity", JsValue.String(collator.Sensitivity));
            options.DefineOrdinary("ignorePunctuation", JsValue.Boolean(collator.IgnorePunctuation));
            options.DefineOrdinary("collation", JsValue.String(collator.Collation));
            options.DefineOrdinary("numeric", JsValue.Boolean(collator.Numeric));
            options.DefineOrdinary("caseFirst", JsValue.String(collator.CaseFirst));
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.Collator"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("Collator", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The collator <paramref name="thisValue"/> is, or the <c>TypeError</c> a method throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=40A86B
    // Broiler-Human:        PENDING
    private static JsCollatorObject CollatorOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsCollatorObject ??
        throw engine.Error("TypeError", "Intl.Collator.prototype." + member + " requires that 'this' be an Intl.Collator");

    /// <summary>
    /// ECMA-402's <c>CompareStrings</c> for <paramref name="collator"/>: -1, 0 or 1, from the shared
    /// library's <see cref="UcaCollator"/>, which charges this engine's meter in its units, NFD as
    /// <c>normalize</c> charges it and then each collation element of both strings (JSD-0084,
    /// decision E7), and refuses a string whose NFD form passes this engine's string-length ceiling
    /// with the <c>RangeError</c> <c>normalize</c> throws.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.3.3.1; IP=Low; Security=Medium; Resources=3; Fingerprint=04FC3E
    // Broiler-Falsified-If: an order, a charge or an error differs from what JsCollator gave for the same strings and options before SJL-17.4
    // Broiler-Human:        PENDING
    internal static int CompareStrings(JsEngine engine, JsCollatorObject collator, string x, string y) =>
        collator.Collator.Compare(x, y, StringLengthCeiling, engine.WorkCharge, out var order) == IntlStatus.Completed
            ? order
            : throw engine.Error("RangeError", "Invalid string length");

    /// <summary>
    /// ECMA-402's <c>Intl.Collator</c> constructor steps: the prototype from the new target, then the
    /// options read in the specification's order, then the locale resolved.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.1.1; IP=Low; Security=Medium; Resources=3; Fingerprint=5A6B74
    // Broiler-Falsified-If: an option is read in another order than ECMA-402's, or a resolved option differs from what the locale and options select
    // Broiler-Human:        PENDING
    internal static JsValue NewCollator(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.CollatorPrototype!);
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = CoerceOptionsToObject(engine, Argument(arguments, 1));

        var usage = GetStringOption(engine, options, "usage", ["sort", "search"], "sort")!;
        var matcher = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit")!;
        _ = matcher;

        var collation = GetStringOption(engine, options, "collation", null, null);

        if (collation is not null && !IsTypeSequence(collation))
        {
            return engine.ThrowRangeError("Intl.Collator: the collation option is not a well-formed type: " + collation);
        }

        var numericOption = GetBooleanOption(engine, options, "numeric", null);
        var caseFirstOption = GetStringOption(engine, options, "caseFirst", ["upper", "lower", "false"], null);

        // THE KEYS' VALUES, THE TAILORING AND THE COMPARISON ARE THE SHARED LIBRARY'S (SJL-17.4, JSD-0084
        // decision E11): its UcaCollationData answers them from the tailorings this family's tables carry,
        // as CollatorKeyData and the choice of a tailoring here did.
        var data = engine.Intl!.CollationData;
        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["co"] = collation,
            ["kn"] = numericOption is null ? null : numericOption.Value ? "true" : "false",
            ["kf"] = caseFirstOption,
        }, ["co", "kf", "kn"], (locale, key) => [.. data.KeyValues(LanguageOf(locale) ?? string.Empty, key, usage)]);

        var collationValue = resolved.Keys["co"] ?? "default";
        var numeric = resolved.Keys["kn"] == "true";
        var caseFirst = resolved.Keys["kf"] ?? "false";
        var tailoring = data.TailoringFor(LanguageOf(resolved.DataLocale) ?? "en", usage, collationValue);

        var sensitivity = GetStringOption(engine, options, "sensitivity", ["base", "accent", "case", "variant"], "variant")!;
        var ignorePunctuation = GetBooleanOption(engine, options, "ignorePunctuation", data.ShiftedByDefault(tailoring)) ?? false;

        var collator = new UcaCollator(
            data,
            UcaCollatorOptions.FromResolved(sensitivity, ignorePunctuation, caseFirst, numeric, tailoring),
            JsUnicode.Normalization);

        return JsValue.Object(new JsCollatorObject(prototype, collator)
        {
            Locale = resolved.Locale,
            Usage = usage,
            Sensitivity = sensitivity,
            IgnorePunctuation = ignorePunctuation,
            Collation = collationValue,
            Numeric = numeric,
            CaseFirst = caseFirst,
        });
    }

    /// <summary>The locales the data supports, as every service offers them: the data's list without <c>und</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9DBD54
    // Broiler-Human:        PENDING
    private static IReadOnlyList<string> AvailableLocales(JsEngine engine) => engine.Intl!.AvailableLocales;

    /// <summary>
    /// The language subtag of a well-formed tag, or nothing for one that is not: the shared library's
    /// parse, ASCII alone (JSD-0082, decision D19).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=724332
    // Broiler-Human:        PENDING
    internal static string? LanguageOf(string locale) => ParseLocale(locale)?.Language;

    /// <summary>A well-formed tag parsed by the shared library, in the case and order written, or nothing for one that is not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0123C2
    // Broiler-Human:        PENDING
    internal static LocaleId? ParseLocale(string locale) =>
        LocaleId.TryParse(locale, out var parsed) == IntlStatus.Completed ? parsed : null;

    /// <summary>
    /// <c>Intl.Collator</c>'s data as the shared library's Locale info and <c>supportedValuesOf</c> read
    /// it (JSD-0082, rows L17, L18, L32 and L40): the services' available locales, each one's sort
    /// collations from <see cref="UcaCollationData.KeyValues"/>, and the one collation this family's data adds,
    /// <c>phonebk</c>. The Collator itself stays this family's until SJL-17 (decision SJL-D-24 (c)).
    /// <i>(Noted 2026-11-04, at SJL-17.4: each locale's sort collations read "from
    /// <c>CollatorKeyData</c>", which SJL-17.4 deleted; they are the shared library's
    /// <see cref="UcaCollationData.KeyValues"/>, over this family's tables, which answers the same; and
    /// the Collator's comparison is the library's since SJL-17.4. The sentence on SJL-17 is kept as
    /// written.)</i>
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.2.3; IP=Low; Security=Low; Resources=1; Fingerprint=2E44A4
    // Broiler-Human:        PENDING
    private sealed class CollatorLocaleData(IReadOnlyList<string> availableLocales, UcaCollationData data) : ICollatorLocaleData
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AFF13A
        // Broiler-Human:        PENDING
        public IReadOnlyList<string> AvailableLocales => availableLocales;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1079F9
        // Broiler-Human:        PENDING
        public IReadOnlyList<string> Collations => ["phonebk"];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B33ECD
        // Broiler-Human:        PENDING
        public IReadOnlyList<string?> SortLocaleData(string locale) => data.KeyValues(LanguageOf(locale) ?? string.Empty, "co", "sort");
    }

    /// <summary>The Collator's data the shared library reads, over the services' available locales.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4846C1
    // Broiler-Human:        PENDING
    private static ICollatorLocaleData CollatorData(JsEngine engine) => new CollatorLocaleData(AvailableLocales(engine), engine.Intl!.CollationData);

    /// <summary>
    /// ECMA-402's <c>Intl.supportedValuesOf</c>: the key converted here, and the values this data supports
    /// for it, sorted, from the shared library (JSD-0082, rows L39 to L41).
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s8.3.2; IP=Low; Security=Low; Resources=1; Fingerprint=05C1EE
    // Broiler-Human:        PENDING
    private static JsValue SupportedValuesOf(JsEngine engine, JsValue key)
    {
        var name = engine.ToStringValue(key);
        var tables = engine.Intl!;

        if (SupportedValues.Of(name, tables.LocaleData, tables.TimeZones, CollatorData(engine), out var values) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.supportedValuesOf: invalid key: " + name);
        }

        var array = new List<JsValue>(values.Count);

        foreach (var value in values)
        {
            array.Add(JsValue.String(value));
        }

        return JsValue.Object(engine.Realm.NewArray(array));
    }

    // ---- the abstract operations ----------------------------------------------------------------

    /// <summary>ECMA-402's <c>CanonicalizeLocaleList</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.1; IP=Low; Security=Medium; Resources=3; Fingerprint=CEC9F7
    // Broiler-Falsified-If: an element that is not a String or an Object is admitted, or a structurally invalid tag is not a RangeError
    // Broiler-Human:        PENDING
    internal static List<string> CanonicalizeLocaleList(JsEngine engine, JsValue locales)
    {
        var seen = new List<string>();

        if (locales.Type == JsType.Undefined)
        {
            return seen;
        }

        var list = locales.IsString || locales.AsObjectOrNull() is JsLocaleObject ? engine.Realm.NewArray([locales]) : engine.ToObject(locales);
        var length = ArrayLengthOf(engine, JsValue.Object(list));

        for (var index = 0.0; index < length; index++)
        {
            engine.Charge(1);
            var key = ArrayKeyOf(index);

            if (!JsEngine.HasProperty(list, key))
            {
                continue;
            }

            var value = engine.GetProperty(JsValue.Object(list), key);

            if (!value.IsString && !value.IsObject)
            {
                return ThrowList(engine, "TypeError", "Intl: a locale in the list is neither a String nor an Object");
            }

            var tag = value.AsObjectOrNull() is JsLocaleObject locale ? locale.Locale : engine.ToStringValue(value);

            // THE STRING'S PART IS THE SHARED LIBRARY'S (JSD-0082, row L4): IsWellFormedLanguageTag and
            // CanonicalizeUnicodeLocaleId. Tag parsing and canonicalisation charge nothing here, as
            // before (row L43); the charge above is this adapter's, one per element.
            if (LocaleId.TryCanonicalize(tag, engine.Intl!.LocaleData, out var canonical) != IntlStatus.Completed)
            {
                return ThrowList(engine, "RangeError", "Incorrect locale information provided: " + tag);
            }

            if (!seen.Contains(canonical))
            {
                seen.Add(canonical);
            }
        }

        return seen;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9EE630
    // Broiler-Human:        PENDING
    private static List<string> ThrowList(JsEngine engine, string kind, string message) =>
        throw engine.Error(kind, message);

    /// <summary>ECMA-402's <c>CoerceOptionsToObject</c>: <c>undefined</c> is an empty object with no prototype.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.10; IP=Low; Security=Low; Resources=1; Fingerprint=002FE8
    // Broiler-Human:        PENDING
    internal static JsValue CoerceOptionsToObject(JsEngine engine, JsValue options) =>
        options.Type == JsType.Undefined ? JsValue.Object(new JsObject(null)) : JsValue.Object(engine.ToObject(options));

    /// <summary>ECMA-402's <c>GetOption</c> for a String: read, convert, and check against the values when there are any.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.11; IP=Low; Security=Low; Resources=1; Fingerprint=D3DDBD
    // Broiler-Human:        PENDING
    internal static string? GetStringOption(JsEngine engine, JsValue options, string property, string[]? values, string? fallback)
    {
        var value = engine.GetProperty(options, property);

        if (value.Type == JsType.Undefined)
        {
            return fallback;
        }

        var text = engine.ToStringValue(value);

        if (values is not null && Array.IndexOf(values, text) < 0)
        {
            throw engine.Error("RangeError", "Value " + text + " out of range for option " + property);
        }

        return text;
    }

    /// <summary>ECMA-402's <c>GetOption</c> for a Boolean.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.11; IP=Low; Security=Low; Resources=1; Fingerprint=0C4684
    // Broiler-Human:        PENDING
    internal static bool? GetBooleanOption(JsEngine engine, JsValue options, string property, bool? fallback)
    {
        var value = engine.GetProperty(options, property);
        return value.Type == JsType.Undefined ? fallback : value.ToBooleanValue();
    }

    /// <summary>Whether a String is a BCP 47 type: subtags of three to eight letters and digits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=05DF5D
    // Broiler-Human:        PENDING
    private static bool IsTypeSequence(string text) => LocaleId.IsUnicodeType(text);

    /// <summary>What <see cref="ResolveLocale"/> answers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C02A03
    // Broiler-Human:        PENDING
    internal sealed record JsResolvedLocale(string Locale, string DataLocale, Dictionary<string, string?> Keys);

    /// <summary>
    /// An option ResolveLocale reads as <c>null</c> rather than as absent: the value a keyword had is
    /// dropped and the key resolves to the data's <c>null</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C70C6E
    // Broiler-Human:        PENDING
    internal const string NullOption = "\u0000null";

    /// <summary>
    /// ECMA-402's <c>ResolveLocale</c> with the lookup matcher, which also serves <c>best fit</c>: the
    /// first requested locale a prefix of which is available, its supported <c>-u-</c> keywords kept,
    /// and an option overriding a keyword it names.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.7; IP=Low; Security=Medium; Resources=3; Fingerprint=82CDC3
    // Broiler-Falsified-If: a resolved locale names a keyword the locale data does not support, or an option the data supports is not honoured
    // Broiler-Human:        PENDING
    internal static JsResolvedLocale ResolveLocale(JsEngine engine, List<string> requested, Dictionary<string, string?> options, string[] relevantKeys, Func<string, string, string?[]> keyData)
    {
        var tables = engine.Intl!;

        // THE SHARED NEGOTIATION (JSD-0082, rows L28 to L38): the match first, over this family's
        // providers (decision D7), then each relevant key with the service's data for the matched
        // locale and the option this service read for it, NullOption standing for ECMA-402's null.
        var match = LocaleNegotiation.MatchLocale(tables.AvailableLocales, requested, DefaultLocale);
        var keys = new List<RelevantKey>(relevantKeys.Length);

        foreach (var key in relevantKeys)
        {
            var option = options.TryGetValue(key, out var value) ? value : null;
            keys.Add(new RelevantKey(key, keyData(match.Locale, key))
            {
                Option = option == NullOption ? null : option,
                OptionIsNull = option == NullOption,
            });
        }

        var resolved = LocaleNegotiation.ResolveLocale(match, keys, tables.LocaleData);
        return new JsResolvedLocale(resolved.Locale, resolved.DataLocale, new Dictionary<string, string?>(resolved.KeyValues, StringComparer.Ordinal));
    }

    /// <summary>ECMA-402's <c>FilterLocales</c>: the requested locales a prefix of which is available, in the order requested.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.9; IP=Low; Security=Low; Resources=2; Fingerprint=23E9BA
    // Broiler-Human:        PENDING
    internal static List<string> FilterLocales(JsEngine engine, IReadOnlyList<string> available, List<string> requested, JsValue options)
    {
        var coerced = CoerceOptionsToObject(engine, options);
        _ = GetStringOption(engine, coerced, "localeMatcher", ["lookup", "best fit"], "best fit");

        // THE CHARGE STAYS THIS ADAPTER'S (JSD-0082, decision D18): one unit per requested locale, as
        // before; the shared filter, handed no meter, charges nothing.
        foreach (var _ in requested)
        {
            engine.Charge(1);
        }

        return [.. LocaleNegotiation.FilterLocales(available, requested)];
    }

    /// <summary>The argument at <paramref name="index"/>, or <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=31D128
    // Broiler-Human:        PENDING
    private static JsValue Argument(JsValue[] arguments, int index) =>
        index < arguments.Length ? arguments[index] : JsValue.Undefined;
}

/// <summary>An <c>Intl.Collator</c>: its resolved options and its comparison.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8B595B
// Broiler-Human:        PENDING
internal sealed class JsCollatorObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FADDEA
    // Broiler-Human:        PENDING
    internal JsCollatorObject(JsObject prototype, UcaCollator collator) : base(prototype) => Collator = collator;

    /// <summary>The shared library's collator, under the resolved options (SJL-17.4; it was this family's <c>JsCollator</c>).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D988D9
    // Broiler-Human:        PENDING
    internal UcaCollator Collator { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDBB2D
    // Broiler-Human:        PENDING
    internal string Locale { get; init; } = string.Empty;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=598A3E
    // Broiler-Human:        PENDING
    internal string Usage { get; init; } = "sort";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BF5CE4
    // Broiler-Human:        PENDING
    internal string Sensitivity { get; init; } = "variant";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4F0780
    // Broiler-Human:        PENDING
    internal bool IgnorePunctuation { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4661DE
    // Broiler-Human:        PENDING
    internal string Collation { get; init; } = "default";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BDA850
    // Broiler-Human:        PENDING
    internal bool Numeric { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=322AE9
    // Broiler-Human:        PENDING
    internal string CaseFirst { get; init; } = "false";

    /// <summary>The <c>compare</c> getter's function, made the first time it is read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AC946A
    // Broiler-Human:        PENDING
    internal JsNativeFunction? BoundCompare { get; set; }
}
