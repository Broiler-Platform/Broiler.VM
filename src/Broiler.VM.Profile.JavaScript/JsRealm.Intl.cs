// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           11
// Human-reviewed:   0/22
// IP risk:          Low
// Security risk:    Medium
// Criteria:         3/0
// Resource impact:  3/10 max
// Unverified:       22
//
// GENERATED - DO NOT EDIT MANUALLY

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
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s8, s10; IP=Low; Security=Medium; Resources=3; Fingerprint=131B5A
    // Broiler-Human:        PENDING
    private void SetupIntl()
    {
        var intl = new JsObject(ObjectPrototype);

        intl.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl"), JsPropertyAttributes.Configurable));

        Method(intl, "getCanonicalLocales", 1, static (engine, thisValue, arguments) =>
        {
            var locales = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            return JsValue.Object(engine.Realm.NewArray(locales.ConvertAll(static locale => JsValue.String(locale))));
        });

        GlobalObject.SetOwnProperty(
            "Intl",
            JsProperty.Data(JsValue.Object(intl), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(intl, "supportedValuesOf", 1, static (engine, thisValue, arguments) =>
            SupportedValuesOf(engine, Argument(arguments, 0)));

        SetupCollator(intl);
        SetupNumberFormat(intl);
        SetupDateTimeFormat(intl);
        SetupLocale(intl);
    }

    // ---- Intl.Collator ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10; IP=Low; Security=Medium; Resources=3; Fingerprint=103307
    // Broiler-Human:        PENDING
    private void SetupCollator(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        CollatorPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "Collator",
            0,
            // CALLED WITHOUT `new`, THE ACTIVE FUNCTION IS THE NEW TARGET (ECMA-402 s10.1.1 step 1).
            static (engine, thisValue, arguments) =>
                NewCollator(engine, JsValue.Object(engine.Realm.CollatorConstructor!), arguments),
            static (engine, newTarget, arguments) => NewCollator(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        CollatorConstructor = constructor;

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

        prototype.SetOwnProperty(
            "compare",
            JsProperty.Accessor(
                Native("get compare", 0, static (engine, thisValue, arguments) =>
                {
                    var collator = CollatorOfThis(engine, thisValue, "compare");

                    // [[BoundCompare]]: MADE ONCE, an anonymous function of length 2 that remembers its
                    // collator, so `c.compare === c.compare`.
                    if (collator.BoundCompare is null)
                    {
                        var bound = new JsNativeFunction(
                            engine.Realm,
                            engine.Realm.FunctionPrototype,
                            string.Empty,
                            2,
                            (inner, receiver, values) =>
                            {
                                var x = inner.ToStringValue(Argument(values, 0));
                                var y = inner.ToStringValue(Argument(values, 1));
                                return JsValue.Number(collator.Collator.Compare(inner, x, y));
                            });

                        collator.BoundCompare = bound;
                    }

                    return JsValue.Object(collator.BoundCompare);
                }),
                null,
                JsPropertyAttributes.Configurable));

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

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.Collator"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "Collator",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The collator <paramref name="thisValue"/> is, or the <c>TypeError</c> a method throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=40A86B
    // Broiler-Human:        PENDING
    private static JsCollatorObject CollatorOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsCollatorObject ??
        throw engine.Error("TypeError", "Intl.Collator.prototype." + member + " requires that 'this' be an Intl.Collator");

    /// <summary>
    /// ECMA-402's <c>Intl.Collator</c> constructor steps: the prototype from the new target, then the
    /// options read in the specification's order, then the locale resolved.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.1.1; IP=Low; Security=Medium; Resources=3; Fingerprint=7DC5D8
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

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal)
            {
                ["co"] = collation,
                ["kn"] = numericOption is null ? null : numericOption.Value ? "true" : "false",
                ["kf"] = caseFirstOption,
            },
            ["co", "kf", "kn"],
            (locale, key) => CollatorKeyData(locale, key, usage));

        var collationValue = resolved.Keys["co"] ?? "default";
        var numeric = resolved.Keys["kn"] == "true";
        var caseFirst = resolved.Keys["kf"] ?? "false";

        var sensitivity = GetStringOption(engine, options, "sensitivity", ["base", "accent", "case", "variant"], "variant")!;
        var ignorePunctuation = GetBooleanOption(engine, options, "ignorePunctuation", false) ?? false;

        var data = engine.Intl!.Collation;
        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";
        var tailoringName = usage == "search"
            ? (language == "de" ? "de-search" : "und-search")
            : collationValue == "phonebk" && language == "de" ? "de-phonebk" : null;

        var collator = new JsCollator(
            data,
            tailoringName is null ? null : data.TailoringNamed(tailoringName),
            strength: sensitivity switch { "base" or "case" => 1, "accent" => 2, _ => 3 },
            caseLevel: sensitivity == "case",
            caseFirst: caseFirst switch { "upper" => JsCaseFirst.Upper, "lower" => JsCaseFirst.Lower, _ => JsCaseFirst.Off },
            shifted: ignorePunctuation,
            numeric: numeric);

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

    /// <summary>
    /// The values a collator supports for one relevant extension key in one locale, the default
    /// first: ECMA-402's <c>[[SortLocaleData]]</c> and <c>[[SearchLocaleData]]</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.2.3; IP=Low; Security=Low; Resources=1; Fingerprint=99E57A
    // Broiler-Human:        PENDING
    private static string?[] CollatorKeyData(string locale, string key, string usage) => key switch
    {
        // THE FIRST VALUE OF `co` IS NULL, and neither `standard` nor `search` is ever one (s10.2.3).
        "co" => usage == "sort" && JsLocaleTag.Parse(locale)?.Language == "de" ? [null, "phonebk"] : [null],
        "kn" => ["false", "true"],
        "kf" => ["false", "lower", "upper"],
        _ => [null],
    };

    /// <summary>The locales the data supports, as every service offers them: the data's list without <c>und</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F2FA35
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> AvailableLocales(JsEngine engine)
    {
        var available = new System.Collections.Generic.List<string>();

        foreach (var locale in engine.Intl!.Locales)
        {
            if (locale != "und")
            {
                available.Add(locale);
            }
        }

        return available;
    }

    // ---- the abstract operations ----------------------------------------------------------------

    /// <summary>ECMA-402's <c>CanonicalizeLocaleList</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.1; IP=Low; Security=Medium; Resources=3; Fingerprint=19C4FC
    // Broiler-Falsified-If: an element that is not a String or an Object is admitted, or a structurally invalid tag is not a RangeError
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<string> CanonicalizeLocaleList(JsEngine engine, JsValue locales)
    {
        var seen = new System.Collections.Generic.List<string>();

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

            if (!engine.HasProperty(list, key))
            {
                continue;
            }

            var value = engine.GetProperty(JsValue.Object(list), key);

            if (!value.IsString && !value.IsObject)
            {
                return ThrowList(engine, "TypeError", "Intl: a locale in the list is neither a String nor an Object");
            }

            var tag = value.AsObjectOrNull() is JsLocaleObject locale ? locale.Locale : engine.ToStringValue(value);

            if (JsLocaleTag.Parse(tag) is not { } parsed)
            {
                return ThrowList(engine, "RangeError", "Incorrect locale information provided: " + tag);
            }

            var canonical = parsed.Canonicalize(engine.Intl!).ToString();

            if (!seen.Contains(canonical))
            {
                seen.Add(canonical);
            }
        }

        return seen;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=06FD53
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> ThrowList(JsEngine engine, string kind, string message) =>
        throw engine.Error(kind, message);

    /// <summary>ECMA-402's <c>CoerceOptionsToObject</c>: <c>undefined</c> is an empty object with no prototype.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.10; IP=Low; Security=Low; Resources=1; Fingerprint=002FE8
    // Broiler-Human:        PENDING
    internal static JsValue CoerceOptionsToObject(JsEngine engine, JsValue options) =>
        options.Type == JsType.Undefined ? JsValue.Object(new JsObject(null)) : JsValue.Object(engine.ToObject(options));

    /// <summary>ECMA-402's <c>GetOption</c> for a String: read, convert, and check against the values when there are any.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.11; IP=Low; Security=Low; Resources=1; Fingerprint=EA4D7B
    // Broiler-Human:        PENDING
    internal static string? GetStringOption(JsEngine engine, JsValue options, string property, string[]? values, string? fallback)
    {
        var value = engine.GetProperty(options, property);

        if (value.Type == JsType.Undefined)
        {
            return fallback;
        }

        var text = engine.ToStringValue(value);

        if (values is not null && System.Array.IndexOf(values, text) < 0)
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=793C6E
    // Broiler-Human:        PENDING
    private static bool IsTypeSequence(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        foreach (var part in text.Split('-'))
        {
            if (part.Length is < 3 or > 8 || !JsLocaleTag.IsAlphanumeric(part))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>What <see cref="ResolveLocale"/> answers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6A54FD
    // Broiler-Human:        PENDING
    internal sealed record JsResolvedLocale(string Locale, string DataLocale, System.Collections.Generic.Dictionary<string, string?> Keys);

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
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.7; IP=Low; Security=Medium; Resources=3; Fingerprint=D5E229
    // Broiler-Falsified-If: a resolved locale names a keyword the locale data does not support, or an option the data supports is not honoured
    // Broiler-Human:        PENDING
    internal static JsResolvedLocale ResolveLocale(
        JsEngine engine,
        System.Collections.Generic.List<string> requested,
        System.Collections.Generic.Dictionary<string, string?> options,
        string[] relevantKeys,
        System.Func<string, string, string?[]> keyData)
    {
        var available = AvailableLocales(engine);
        var (found, extension) = LookupMatchingLocale(available, requested) ?? (DefaultLocale, null);

        var keywords = new System.Collections.Generic.List<(string Key, string Type)>();

        if (extension is not null)
        {
            keywords.AddRange(extension.Keywords);
        }

        var supported = new System.Collections.Generic.List<(string Key, string Type)>();
        var result = new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal);

        foreach (var key in relevantKeys)
        {
            var values = keyData(found, key);
            var value = values[0];
            (string Key, string Type)? keyword = null;

            var entry = keywords.FindIndex(pair => pair.Key == key);

            if (entry >= 0)
            {
                var requestedValue = keywords[entry].Type;

                if (requestedValue.Length != 0)
                {
                    if (System.Array.IndexOf(values, requestedValue) >= 0)
                    {
                        value = requestedValue;
                        keyword = (key, value);
                    }
                }
                else if (System.Array.IndexOf(values, "true") >= 0)
                {
                    value = "true";
                    keyword = (key, string.Empty);
                }
            }

            if (options.TryGetValue(key, out var optionValue) && optionValue == NullOption)
            {
                if (value is not null && System.Array.IndexOf(values, null) >= 0)
                {
                    value = null;
                    keyword = null;
                }
            }
            else if (optionValue is not null)
            {
                var canonical = optionValue.ToLowerInvariant();

                if (engine.Intl!.TryType('u', key, canonical, out var preferred))
                {
                    canonical = preferred;
                }

                if (canonical.Length == 0)
                {
                    canonical = "true";
                }

                if (!string.Equals(canonical, value, System.StringComparison.Ordinal) && System.Array.IndexOf(values, canonical) >= 0)
                {
                    value = canonical;
                    keyword = null;
                }
            }

            if (keyword is { } kept)
            {
                supported.Add(kept);
            }

            result[key] = value;
        }

        var locale = found;

        if (supported.Count != 0)
        {
            var tag = JsLocaleTag.Parse(found)!;
            tag.HasUnicodeExtension = true;
            tag.Keywords.AddRange(supported);
            locale = tag.Canonicalize(engine.Intl!).ToString();
        }

        return new JsResolvedLocale(locale, found, result);
    }

    /// <summary>
    /// ECMA-402's <c>LookupMatchingLocaleByPrefix</c>: for each requested locale, its <c>-u-</c>
    /// extension set aside, the longest available prefix, truncated a subtag (and a singleton before
    /// it) at a time.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.3; IP=Low; Security=Low; Resources=2; Fingerprint=22AD93
    // Broiler-Human:        PENDING
    internal static (string Locale, JsLocaleTag? Extension)? LookupMatchingLocale(
        System.Collections.Generic.List<string> available, System.Collections.Generic.List<string> requested)
    {
        foreach (var locale in requested)
        {
            var tag = JsLocaleTag.Parse(locale);

            if (tag is null)
            {
                continue;
            }

            var extension = tag.HasUnicodeExtension ? tag : null;
            var prefix = tag.WithoutUnicodeExtension();

            while (prefix.Length != 0)
            {
                if (available.Contains(prefix))
                {
                    return (prefix, extension);
                }

                var dash = prefix.LastIndexOf('-');

                if (dash < 0)
                {
                    break;
                }

                prefix = prefix[..dash];

                if (prefix.Length >= 2 && prefix[^2] == '-')
                {
                    prefix = prefix[..^2];
                }
            }
        }

        return null;
    }

    /// <summary>ECMA-402's <c>FilterLocales</c>: the requested locales a prefix of which is available, in the order requested.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.9; IP=Low; Security=Low; Resources=2; Fingerprint=928316
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<string> FilterLocales(
        JsEngine engine,
        System.Collections.Generic.List<string> available,
        System.Collections.Generic.List<string> requested,
        JsValue options)
    {
        var coerced = CoerceOptionsToObject(engine, options);
        _ = GetStringOption(engine, coerced, "localeMatcher", ["lookup", "best fit"], "best fit");

        var subset = new System.Collections.Generic.List<string>();

        foreach (var locale in requested)
        {
            engine.Charge(1);

            if (LookupMatchingLocale(available, [locale]) is not null)
            {
                subset.Add(locale);
            }
        }

        return subset;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=58315C
    // Broiler-Human:        PENDING
    internal JsCollatorObject(JsObject prototype, JsCollator collator)
        : base(prototype) => Collator = collator;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=24AFF4
    // Broiler-Human:        PENDING
    internal JsCollator Collator { get; }

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
