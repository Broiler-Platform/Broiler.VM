// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           5
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  3/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.NumberFormat</c>, and <c>Number.prototype.toLocaleString</c> and
/// <c>BigInt.prototype.toLocaleString</c> through it (ECMA-402 s16; JSD-0044).
/// </summary>
/// <remarks>
/// <para>
/// <b>The constructor reads its options in ECMA-402's order</b> and keeps what it resolved; the
/// formatting is <see cref="JsNumberFormatter"/>'s. Called as a function on an object that inherits
/// from <c>Intl.NumberFormat.prototype</c>, it takes ECMA-402's normative optional legacy path: the
/// new formatter is stored on that object under the realm's <c>IntlLegacyConstructedSymbol</c>, and
/// the methods that unwrap read it from there.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.NumberFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    internal JsObject? NumberFormatPrototype { get; private set; }

    /// <summary><c>%Intl.NumberFormat%</c>, or nothing where the surface is not built.</summary>
    internal JsNativeFunction? NumberFormatConstructor { get; private set; }

    /// <summary><c>%Intl%.[[FallbackSymbol]]</c>, the legacy constructor's key.</summary>
    internal JsSymbol? IntlFallbackSymbol { get; private set; }

    /// <summary>The sanctioned single units of ECMA-402 table 2.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2BE57D
    // Broiler-Human:        PENDING
    private static readonly string[] SanctionedUnits =
    [
        "acre", "bit", "byte", "celsius", "centimeter", "day", "degree", "fahrenheit", "fluid-ounce", "foot",
        "gallon", "gigabit", "gigabyte", "gram", "hectare", "hour", "inch", "kilobit", "kilobyte", "kilogram",
        "kilometer", "liter", "megabit", "megabyte", "meter", "microsecond", "mile", "mile-scandinavian",
        "milliliter", "millimeter", "millisecond", "minute", "month", "nanosecond", "ounce", "percent", "petabyte",
        "pound", "second", "stone", "terabit", "terabyte", "week", "yard", "year",
    ];

    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16; IP=Low; Security=Medium; Resources=3; Fingerprint=28FC0B
    // Broiler-Human:        PENDING
    private void SetupNumberFormat(JsObject intl)
    {
        IntlFallbackSymbol = new JsSymbol("IntlLegacyConstructedSymbol", described: true);

        var prototype = new JsObject(ObjectPrototype);
        NumberFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "NumberFormat",
            0,
            static (engine, thisValue, arguments) =>
                NewNumberFormat(engine, JsValue.Object(engine.Realm.NumberFormatConstructor!), arguments, thisValue, called: true),
            static (engine, newTarget, arguments) => NewNumberFormat(engine, newTarget, arguments, JsValue.Undefined, called: false));

        constructor.BuildsFromNewTarget = true;
        NumberFormatConstructor = constructor;

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
            "format",
            JsProperty.Accessor(
                Native("get format", 0, static (engine, thisValue, arguments) =>
                {
                    var format = UnwrapNumberFormat(engine, thisValue, "format");

                    // [[BoundFormat]]: MADE ONCE, an anonymous function of length 1.
                    if (format.BoundFormat is null)
                    {
                        format.BoundFormat = new JsNativeFunction(
                            engine.Realm,
                            engine.Realm.FunctionPrototype,
                            string.Empty,
                            1,
                            (inner, receiver, values) =>
                                JsValue.String(format.Formatter.Format(inner, ToIntlMathematicalValue(inner, Argument(values, 0)))));
                    }

                    return JsValue.Object(format.BoundFormat);
                }),
                null,
                JsPropertyAttributes.Configurable));

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatToParts");
            var x = ToIntlMathematicalValue(engine, Argument(arguments, 0));
            return PartsArray(engine, format.Formatter.Parts(engine, x));
        });

        Method(prototype, "formatRange", 2, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatRange");
            var (x, y) = RangeValues(engine, arguments);
            var text = new System.Text.StringBuilder();

            foreach (var part in format.Formatter.RangeParts(engine, x, y))
            {
                text.Append(part.Value);
            }

            return JsValue.String(text.ToString());
        });

        Method(prototype, "formatRangeToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatRangeToParts");
            var (x, y) = RangeValues(engine, arguments);
            return PartsArray(engine, format.Formatter.RangeParts(engine, x, y));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var formatter = UnwrapNumberFormat(engine, thisValue, "resolvedOptions").Formatter;
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

            Put("locale", formatter.Locale);
            Put("numberingSystem", formatter.NumberingSystem);
            Put("style", formatter.Style);
            Put("currency", formatter.Currency);
            Put("currencyDisplay", formatter.CurrencyDisplay);
            Put("currencySign", formatter.CurrencySign);
            Put("unit", formatter.Unit);
            Put("unitDisplay", formatter.UnitDisplay);
            PutNumber("minimumIntegerDigits", formatter.MinimumIntegerDigits);
            PutNumber("minimumFractionDigits", formatter.MinimumFractionDigits);
            PutNumber("maximumFractionDigits", formatter.MaximumFractionDigits);
            PutNumber("minimumSignificantDigits", formatter.MinimumSignificantDigits);
            PutNumber("maximumSignificantDigits", formatter.MaximumSignificantDigits);
            options.DefineOrdinary("useGrouping", formatter.UseGrouping is { } grouping ? JsValue.String(grouping) : JsValue.Boolean(false));
            Put("notation", formatter.Notation);
            Put("compactDisplay", formatter.CompactDisplay);
            Put("signDisplay", formatter.SignDisplay);
            PutNumber("roundingIncrement", formatter.RoundingIncrement);
            Put("roundingMode", formatter.RoundingMode);
            Put("roundingPriority", formatter.ComputedRoundingPriority);
            Put("trailingZeroDisplay", formatter.TrailingZeroDisplay);
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.NumberFormat"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "NumberFormat",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The two values of a range: both present, both numbers.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.3; IP=Low; Security=Low; Resources=1; Fingerprint=3BFAA0
    // Broiler-Human:        PENDING
    private static (JsDecimal X, JsDecimal Y) RangeValues(JsEngine engine, JsValue[] arguments)
    {
        var start = Argument(arguments, 0);
        var end = Argument(arguments, 1);

        if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Intl.NumberFormat: a range needs a start and an end");
        }

        var x = ToIntlMathematicalValue(engine, start);
        var y = ToIntlMathematicalValue(engine, end);

        if (x.Kind == JsDecimalKind.NaN || y.Kind == JsDecimalKind.NaN)
        {
            throw engine.Error("RangeError", "Intl.NumberFormat: a range cannot start or end at NaN");
        }

        return (x, y);
    }

    /// <summary>An array of <c>{ type, value }</c> objects, with <c>source</c> where a part has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=7363B8
    // Broiler-Human:        PENDING
    private static JsValue PartsArray(JsEngine engine, System.Collections.Generic.List<JsNumberPart> parts)
    {
        var values = new System.Collections.Generic.List<JsValue>(parts.Count);

        foreach (var part in parts)
        {
            engine.Charge(1);
            var item = new JsObject(engine.Realm.ObjectPrototype);
            item.DefineOrdinary("type", JsValue.String(part.Type));
            item.DefineOrdinary("value", JsValue.String(part.Value));

            if (part.Source is not null)
            {
                item.DefineOrdinary("source", JsValue.String(part.Source));
            }

            values.Add(JsValue.Object(item));
        }

        return JsValue.Object(engine.Realm.NewArray(values));
    }

    /// <summary>ECMA-402's ToIntlMathematicalValue.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16; IP=Low; Security=Medium; Resources=2; Fingerprint=776FBB
    // Broiler-Human:        PENDING
    internal static JsDecimal ToIntlMathematicalValue(JsEngine engine, JsValue value)
    {
        var primitive = engine.ToPrimitive(value, "number");

        if (primitive.IsBigInt)
        {
            return JsDecimal.FromBigInt(primitive.AsBigInt(), engine.ChargeFuel);
        }

        if (primitive.IsString)
        {
            return JsDecimal.FromString(primitive.AsString(), engine.ChargeFuel);
        }

        return JsDecimal.FromNumber(engine.ToNumber(primitive));
    }

    /// <summary>The number format <paramref name="thisValue"/> is, with no unwrapping, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D4F8D0
    // Broiler-Human:        PENDING
    private static JsNumberFormatObject NumberFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsNumberFormatObject ??
        throw engine.Error("TypeError", "Intl.NumberFormat.prototype." + member + " requires that 'this' be an Intl.NumberFormat");

    /// <summary>
    /// ECMA-402's UnwrapNumberFormat: the number format itself, or the one a legacy construction
    /// stored on an object inheriting from <c>Intl.NumberFormat.prototype</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.10; IP=Low; Security=Medium; Resources=1; Fingerprint=5B9E15
    // Broiler-Human:        PENDING
    private static JsNumberFormatObject UnwrapNumberFormat(JsEngine engine, JsValue thisValue, string member)
    {
        if (!thisValue.IsObject)
        {
            throw engine.Error("TypeError", "Intl.NumberFormat.prototype." + member + " requires that 'this' be an object");
        }

        if (thisValue.AsObject() is JsNumberFormatObject direct)
        {
            return direct;
        }

        var realm = engine.Realm;

        if (engine.OrdinaryHasInstance(JsValue.Object(realm.NumberFormatConstructor!), thisValue) &&
            engine.GetSymbolWithReceiver(thisValue.AsObject(), realm.IntlFallbackSymbol!, thisValue).AsObjectOrNull() is JsNumberFormatObject wrapped)
        {
            return wrapped;
        }

        throw engine.Error("TypeError", "Intl.NumberFormat.prototype." + member + " requires that 'this' be an Intl.NumberFormat");
    }

    /// <summary>
    /// ECMA-402's <c>Intl.NumberFormat</c> constructor: the prototype from the new target, then
    /// InitializeNumberFormat, then the legacy path when called as a function.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.1.1; IP=Low; Security=Medium; Resources=3; Fingerprint=9EAA6C
    // Broiler-Falsified-If: an option is read in another order than ECMA-402's, an invalid option is admitted, or a resolved option differs from what the locale and options select
    // Broiler-Human:        PENDING
    internal static JsValue NewNumberFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments, JsValue thisValue, bool called)
    {
        var realm = engine.Realm;
        var prototype = engine.PrototypeFromConstructor(newTarget, realm.NumberFormatPrototype!);
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = CoerceOptionsToObject(engine, Argument(arguments, 1));

        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            return engine.ThrowRangeError("Intl.NumberFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        var numbers = engine.Intl!.Numbers;
        var systems = new System.Collections.Generic.List<string?> { "latn" };

        foreach (var name in numbers.NumberingSystems.Keys)
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

        var language = JsLocaleTag.Parse(resolved.DataLocale)?.Language ?? "en";

        // SetNumberFormatUnitOptions.
        var style = GetStringOption(engine, options, "style", ["decimal", "percent", "currency", "unit"], "decimal")!;
        var currency = GetStringOption(engine, options, "currency", null, null);

        if (currency is null)
        {
            if (style == "currency")
            {
                throw engine.Error("TypeError", "Intl.NumberFormat: the currency style needs a currency");
            }
        }
        else if (!IsWellFormedCurrencyCode(currency))
        {
            return engine.ThrowRangeError("Intl.NumberFormat: invalid currency code: " + currency);
        }

        var currencyDisplay = GetStringOption(engine, options, "currencyDisplay", ["code", "symbol", "narrowSymbol", "name"], "symbol")!;
        var currencySign = GetStringOption(engine, options, "currencySign", ["standard", "accounting"], "standard")!;
        var unit = GetStringOption(engine, options, "unit", null, null);

        if (unit is null)
        {
            if (style == "unit")
            {
                throw engine.Error("TypeError", "Intl.NumberFormat: the unit style needs a unit");
            }
        }
        else if (!IsWellFormedUnitIdentifier(unit))
        {
            return engine.ThrowRangeError("Intl.NumberFormat: invalid unit: " + unit);
        }

        var unitDisplay = GetStringOption(engine, options, "unitDisplay", ["short", "narrow", "long"], "short")!;
        var notation = GetStringOption(engine, options, "notation", ["standard", "scientific", "engineering", "compact"], "standard")!;

        int mnfdDefault;
        int mxfdDefault;

        if (style == "currency" && notation == "standard")
        {
            var upper = currency!.ToUpperInvariant();
            mnfdDefault = numbers.CurrencyDigits.TryGetValue(upper, out var digits) ? digits : 2;
            mxfdDefault = mnfdDefault;
        }
        else
        {
            mnfdDefault = 0;
            mxfdDefault = style == "percent" ? 0 : 3;
        }

        var digitOptions = SetNumberFormatDigitOptions(engine, options, mnfdDefault, mxfdDefault, notation);

        var compactDisplay = GetStringOption(engine, options, "compactDisplay", ["short", "long"], "short")!;
        var defaultUseGrouping = notation == "compact" ? "min2" : "auto";
        var useGrouping = GetBooleanOrStringNumberFormatOption(engine, options, "useGrouping", ["min2", "auto", "always", "true", "false"], defaultUseGrouping);

        var signDisplay = GetStringOption(engine, options, "signDisplay", ["auto", "never", "always", "exceptZero", "negative"], "auto")!;

        var formatter = new JsNumberFormatter(numbers, language, resolved.Keys["nu"] ?? "latn")
        {
            Locale = resolved.Locale,
            Style = style,
            Currency = style == "currency" ? currency!.ToUpperInvariant() : null,
            CurrencyDisplay = style == "currency" ? currencyDisplay : null,
            CurrencySign = style == "currency" ? currencySign : null,
            Unit = style == "unit" ? unit : null,
            UnitDisplay = style == "unit" ? unitDisplay : null,
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
            UseGrouping = useGrouping,
            SignDisplay = signDisplay,
        };

        var made = new JsNumberFormatObject(prototype, formatter);

        // CHAINNUMBERFORMAT, ECMA-402'S NORMATIVE OPTIONAL LEGACY PATH: called as a function on an
        // object that is an Intl.NumberFormat by its prototype chain, the new format is stored on it.
        if (called && thisValue.IsObject &&
            engine.OrdinaryHasInstance(JsValue.Object(realm.NumberFormatConstructor!), thisValue))
        {
            var fields = new ObjectDescriptorFields
            {
                HasValue = true,
                Value = JsValue.Object(made),
                HasWritable = true,
                Writable = false,
                HasEnumerable = true,
                Enumerable = false,
                HasConfigurable = true,
                Configurable = false,
            };

            ObjectApplyDescriptorAt(engine, thisValue.AsObject(), JsValue.Symbol(realm.IntlFallbackSymbol!), fields);
            return thisValue;
        }

        return JsValue.Object(made);
    }

    /// <summary>What SetNumberFormatDigitOptions resolves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F0C7A5
    // Broiler-Human:        PENDING
    internal sealed record JsDigitOptions(
        int MinimumIntegerDigits,
        int? MinimumFractionDigits,
        int? MaximumFractionDigits,
        int? MinimumSignificantDigits,
        int? MaximumSignificantDigits,
        string RoundingType,
        string ComputedRoundingPriority,
        int RoundingIncrement,
        string RoundingMode,
        string TrailingZeroDisplay);

    /// <summary>ECMA-402's SetNumberFormatDigitOptions.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.1.2; IP=Low; Security=Medium; Resources=2; Fingerprint=ED6326
    // Broiler-Falsified-If: a digit option is read more than once or out of ECMA-402's order, or a combination ECMA-402 refuses is admitted
    // Broiler-Human:        PENDING
    internal static JsDigitOptions SetNumberFormatDigitOptions(JsEngine engine, JsValue options, int mnfdDefault, int mxfdDefault, string notation)
    {
        var mnid = GetNumberOption(engine, options, "minimumIntegerDigits", 1, 21, 1)!.Value;
        var mnfd = engine.GetProperty(options, "minimumFractionDigits");
        var mxfd = engine.GetProperty(options, "maximumFractionDigits");
        var mnsd = engine.GetProperty(options, "minimumSignificantDigits");
        var mxsd = engine.GetProperty(options, "maximumSignificantDigits");
        var roundingIncrement = GetNumberOption(engine, options, "roundingIncrement", 1, 5000, 1)!.Value;

        if (System.Array.IndexOf([1, 2, 5, 10, 20, 25, 50, 100, 200, 250, 500, 1000, 2000, 2500, 5000], roundingIncrement) < 0)
        {
            throw engine.Error("RangeError", "Intl.NumberFormat: roundingIncrement must be one of the admitted increments");
        }

        var roundingMode = GetStringOption(engine, options, "roundingMode",
            ["ceil", "floor", "expand", "trunc", "halfCeil", "halfFloor", "halfExpand", "halfTrunc", "halfEven"], "halfExpand")!;
        var roundingPriority = GetStringOption(engine, options, "roundingPriority", ["auto", "morePrecision", "lessPrecision"], "auto")!;
        var trailingZeroDisplay = GetStringOption(engine, options, "trailingZeroDisplay", ["auto", "stripIfInteger"], "auto")!;

        if (roundingIncrement != 1)
        {
            mxfdDefault = mnfdDefault;
        }

        var hasSd = mnsd.Type != JsType.Undefined || mxsd.Type != JsType.Undefined;
        var hasFd = mnfd.Type != JsType.Undefined || mxfd.Type != JsType.Undefined;
        var needSd = true;
        var needFd = true;

        if (roundingPriority == "auto")
        {
            needSd = hasSd;

            if (needSd || (!hasFd && notation == "compact"))
            {
                needFd = false;
            }
        }

        int? minSd = null, maxSd = null, minFd = null, maxFd = null;

        if (needSd)
        {
            if (hasSd)
            {
                minSd = DefaultNumberOption(engine, mnsd, 1, 21, 1, "minimumSignificantDigits");
                maxSd = DefaultNumberOption(engine, mxsd, minSd!.Value, 21, 21, "maximumSignificantDigits");
            }
            else
            {
                minSd = 1;
                maxSd = 21;
            }
        }

        if (needFd)
        {
            if (hasFd)
            {
                var min = DefaultNumberOption(engine, mnfd, 0, 100, null, "minimumFractionDigits");
                var max = DefaultNumberOption(engine, mxfd, 0, 100, null, "maximumFractionDigits");

                if (min is null)
                {
                    min = System.Math.Min(mnfdDefault, max!.Value);
                }
                else if (max is null)
                {
                    max = System.Math.Max(mxfdDefault, min.Value);
                }
                else if (min > max)
                {
                    throw engine.Error("RangeError", "Intl.NumberFormat: minimumFractionDigits is greater than maximumFractionDigits");
                }

                minFd = min;
                maxFd = max;
            }
            else
            {
                minFd = mnfdDefault;
                maxFd = mxfdDefault;
            }
        }

        string roundingType;
        string computed;

        if (!needSd && !needFd)
        {
            minFd = 0;
            maxFd = 0;
            minSd = 1;
            maxSd = 2;
            roundingType = "morePrecision";
            computed = "morePrecision";
        }
        else if (roundingPriority is "morePrecision" or "lessPrecision")
        {
            roundingType = roundingPriority;
            computed = roundingPriority;
        }
        else if (hasSd)
        {
            roundingType = "significantDigits";
            computed = "auto";
        }
        else
        {
            roundingType = "fractionDigits";
            computed = "auto";
        }

        if (roundingIncrement != 1)
        {
            if (roundingType != "fractionDigits")
            {
                throw engine.Error("TypeError", "Intl.NumberFormat: roundingIncrement needs fraction-digit rounding");
            }

            if (maxFd != minFd)
            {
                throw engine.Error("RangeError", "Intl.NumberFormat: roundingIncrement needs equal minimum and maximum fraction digits");
            }
        }

        return new JsDigitOptions(mnid, minFd, maxFd, minSd, maxSd, roundingType, computed, roundingIncrement, roundingMode, trailingZeroDisplay);
    }

    /// <summary>ECMA-402's GetNumberOption.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.14; IP=Low; Security=Low; Resources=1; Fingerprint=E620D8
    // Broiler-Human:        PENDING
    internal static int? GetNumberOption(JsEngine engine, JsValue options, string property, int minimum, int maximum, int? fallback) =>
        DefaultNumberOption(engine, engine.GetProperty(options, property), minimum, maximum, fallback, property);

    /// <summary>ECMA-402's DefaultNumberOption.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.13; IP=Low; Security=Low; Resources=1; Fingerprint=9EAB51
    // Broiler-Human:        PENDING
    internal static int? DefaultNumberOption(JsEngine engine, JsValue value, int minimum, int maximum, int? fallback, string property)
    {
        if (value.Type == JsType.Undefined)
        {
            return fallback;
        }

        var number = engine.ToNumber(value);

        if (double.IsNaN(number) || number < minimum || number > maximum)
        {
            throw engine.Error("RangeError", property + " value is out of range.");
        }

        return (int)System.Math.Floor(number);
    }

    /// <summary>
    /// ECMA-402's GetBooleanOrStringNumberFormatOption, with <see langword="true"/> already read as
    /// <c>always</c> and <see langword="false"/> answered as <see langword="null"/>; the strings
    /// <c>"true"</c> and <c>"false"</c> answer the fallback, as the step after it requires.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.12; IP=Low; Security=Low; Resources=1; Fingerprint=418222
    // Broiler-Human:        PENDING
    private static string? GetBooleanOrStringNumberFormatOption(JsEngine engine, JsValue options, string property, string[] values, string fallback)
    {
        var value = engine.GetProperty(options, property);

        if (value.Type == JsType.Undefined)
        {
            return fallback;
        }

        if (value.Type == JsType.Boolean && value.ToBooleanValue())
        {
            return "always";
        }

        if (!value.ToBooleanValue())
        {
            return null;
        }

        var text = engine.ToStringValue(value);

        if (text is "true" or "false")
        {
            return fallback;
        }

        if (System.Array.IndexOf(values, text) < 0)
        {
            throw engine.Error("RangeError", "Value " + text + " out of range for option " + property);
        }

        return text;
    }

    /// <summary>ECMA-402's IsWellFormedCurrencyCode: three ASCII letters.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s6.3.1; IP=Low; Security=Low; Resources=0; Fingerprint=F691DB
    // Broiler-Human:        PENDING
    private static bool IsWellFormedCurrencyCode(string code) =>
        code.Length == 3 && char.IsAsciiLetter(code[0]) && char.IsAsciiLetter(code[1]) && char.IsAsciiLetter(code[2]);

    /// <summary>ECMA-402's IsWellFormedUnitIdentifier: a sanctioned unit, or two joined by <c>-per-</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s6.6.1; IP=Low; Security=Low; Resources=1; Fingerprint=D5DE66
    // Broiler-Human:        PENDING
    private static bool IsWellFormedUnitIdentifier(string unit)
    {
        if (System.Array.IndexOf(SanctionedUnits, unit) >= 0)
        {
            return true;
        }

        var per = unit.IndexOf("-per-", System.StringComparison.Ordinal);
        return per > 0 &&
            System.Array.IndexOf(SanctionedUnits, unit[..per]) >= 0 &&
            System.Array.IndexOf(SanctionedUnits, unit[(per + 5)..]) >= 0;
    }

    /// <summary>A Number's or a BigInt's <c>toLocaleString</c> through the realm's own <c>%Intl.NumberFormat%</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s20.2.1, s20.3.1; IP=Low; Security=Low; Resources=2; Fingerprint=53E538
    // Broiler-Human:        PENDING
    internal static string ToLocaleNumberString(JsEngine engine, JsDecimal x, JsValue[] arguments)
    {
        var made = NewNumberFormat(
            engine,
            JsValue.Object(engine.Realm.NumberFormatConstructor!),
            [Argument(arguments, 0), Argument(arguments, 1)],
            JsValue.Undefined,
            called: false);

        return ((JsNumberFormatObject)made.AsObject()).Formatter.Format(engine, x);
    }
}

/// <summary>An <c>Intl.NumberFormat</c>: its formatter and its bound <c>format</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B1542
// Broiler-Human:        PENDING
internal sealed class JsNumberFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D23E25
    // Broiler-Human:        PENDING
    internal JsNumberFormatObject(JsObject prototype, JsNumberFormatter formatter)
        : base(prototype) => Formatter = formatter;

    internal JsNumberFormatter Formatter { get; }

    /// <summary>The <c>format</c> getter's function, made the first time it is read.</summary>
    internal JsNativeFunction? BoundFormat { get; set; }
}
