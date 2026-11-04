// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           5
// Human-reviewed:   0/28
// IP risk:          Low
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  3/10 max
// Unverified:       28
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.ECMAScript.Intl;
using Broiler.ECMAScript.Numbers;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.NumberFormat</c>, and <c>Number.prototype.toLocaleString</c> and
/// <c>BigInt.prototype.toLocaleString</c> through it (ECMA-402 s16; JSD-0044).
/// </summary>
/// <remarks>
/// <para>
/// <b>The constructor reads its options in ECMA-402's order</b> and keeps what it resolved; the
/// formatting is the shared Intl library's <c>NumberFormatter</c> since SJL-9.4 (JSD-0082, sub-area
/// (c)), and the digit options its <c>DigitOptionRules</c> (sub-area (b)). Called as a function on an
/// object that inherits from <c>Intl.NumberFormat.prototype</c>, it takes ECMA-402's normative
/// optional legacy path: the new formatter is stored on that object under the realm's
/// <c>IntlLegacyConstructedSymbol</c>, and the methods that unwrap read it from there.
/// </para>
/// <para>
/// <b>What stays here</b> (the library's design, section 6): every option read, in the order the
/// specification reads it, each checked as it is read; ToIntlMathematicalValue's ToPrimitive; a
/// range's <c>undefined</c> ends; the bound <c>format</c> and its caching; the legacy constructor; and
/// the parts arrays with their one-unit charge per part. The library takes the resolved record and the
/// engine's meter, and charges where this family charged (decision D18).
/// </para>
/// <para>
/// <i>Revised 2026-11-03, at SJL-9.4: the first paragraph read "the formatting is
/// <c>JsNumberFormatter</c>'s"; that type is deleted.</i>
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

    /// <summary>The <c>style</c> option's values, in <c>NumberStyle</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E9CF09
    // Broiler-Human:        PENDING
    private static readonly string[] StyleNames = ["decimal", "percent", "currency", "unit"];

    /// <summary>The <c>currencyDisplay</c> option's values, in <c>CurrencyDisplay</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E368C1
    // Broiler-Human:        PENDING
    private static readonly string[] CurrencyDisplayNames = ["symbol", "narrowSymbol", "code", "name"];

    /// <summary>The <c>currencySign</c> option's values, in <c>CurrencySign</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=06D2DE
    // Broiler-Human:        PENDING
    private static readonly string[] CurrencySignNames = ["standard", "accounting"];

    /// <summary>The <c>unitDisplay</c> option's values, in <c>UnitDisplay</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C97B6A
    // Broiler-Human:        PENDING
    private static readonly string[] UnitDisplayNames = ["short", "narrow", "long"];

    /// <summary>The <c>notation</c> option's values, in <c>NumberNotation</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=97048B
    // Broiler-Human:        PENDING
    internal static readonly string[] NotationNames = ["standard", "scientific", "engineering", "compact"];

    /// <summary>The <c>compactDisplay</c> option's values, in <c>CompactDisplay</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=098B9E
    // Broiler-Human:        PENDING
    internal static readonly string[] CompactDisplayNames = ["short", "long"];

    /// <summary>The <c>signDisplay</c> option's values, in <c>SignDisplay</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=438D24
    // Broiler-Human:        PENDING
    private static readonly string[] SignDisplayNames = ["auto", "never", "always", "exceptZero", "negative"];

    /// <summary>The <c>useGrouping</c> option's string values, and <see langword="null"/> for <see langword="false"/>, in <c>NumberGrouping</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8ADBB4
    // Broiler-Human:        PENDING
    private static readonly string?[] GroupingNames = ["auto", "always", "min2", null];

    /// <summary>The <c>roundingMode</c> option's values, in <c>RoundingMode</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5931F8
    // Broiler-Human:        PENDING
    private static readonly string[] RoundingModeNames = ["ceil", "floor", "expand", "trunc", "halfCeil", "halfFloor", "halfExpand", "halfTrunc", "halfEven"];

    /// <summary>The <c>roundingPriority</c> option's values, in <c>RoundingPriority</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=773850
    // Broiler-Human:        PENDING
    private static readonly string[] RoundingPriorityNames = ["auto", "morePrecision", "lessPrecision"];

    /// <summary>The <c>trailingZeroDisplay</c> option's values, in <c>TrailingZeroDisplay</c>'s order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1E3F3D
    // Broiler-Human:        PENDING
    private static readonly string[] TrailingZeroDisplayNames = ["auto", "stripIfInteger"];

    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16; IP=Low; Security=Medium; Resources=3; Fingerprint=27163B
    // Broiler-Human:        PENDING
    private void SetupNumberFormat(JsObject intl)
    {
        IntlFallbackSymbol = new JsSymbol("IntlLegacyConstructedSymbol", described: true);

        var prototype = new JsObject(ObjectPrototype);
        NumberFormatPrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "NumberFormat", 0,
            static (engine, thisValue, arguments) => NewNumberFormat(engine, JsValue.Object(engine.Realm.NumberFormatConstructor!), arguments, thisValue, called: true),
            static (engine, newTarget, arguments) => NewNumberFormat(engine, newTarget, arguments, JsValue.Undefined, called: false))
        {
            BuildsFromNewTarget = true
        };
        NumberFormatConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        prototype.SetOwnProperty("format", JsProperty.Accessor(Native("get format", 0, static (engine, thisValue, arguments) =>
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
                    {
                        var x = ToIntlMathematicalValue(inner, Argument(values, 0));
                        return JsValue.String(format.Formatter.Format(x, inner.WorkCharge));
                    });
            }

            return JsValue.Object(format.BoundFormat);
        }), null, JsPropertyAttributes.Configurable));

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatToParts");
            var x = ToIntlMathematicalValue(engine, Argument(arguments, 0));
            return PartsArray(engine, format.Formatter.FormatToParts(x, engine.WorkCharge));
        });

        Method(prototype, "formatRange", 2, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatRange");
            var (x, y) = RangeValues(engine, arguments);
            RangeCompleted(engine, format.Formatter.FormatRange(x, y, out var text, engine.WorkCharge));
            return JsValue.String(text);
        });

        Method(prototype, "formatRangeToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = NumberFormatOfThis(engine, thisValue, "formatRangeToParts");
            var (x, y) = RangeValues(engine, arguments);
            RangeCompleted(engine, format.Formatter.FormatRangeToParts(x, y, out var parts, engine.WorkCharge));
            return PartsArray(engine, parts);
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var formatter = UnwrapNumberFormat(engine, thisValue, "resolvedOptions").Formatter;
            var digits = formatter.DigitOptions;
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

            // TABLE 15, IN ITS ORDER: each slot as the constructor resolved it, read back from the
            // library's record.
            Put("locale", formatter.Locale);
            Put("numberingSystem", formatter.NumberingSystem);
            Put("style", StyleNames[(int)formatter.Style]);
            Put("currency", formatter.Currency);
            Put("currencyDisplay", formatter.CurrencyDisplay is { } currencyDisplay ? CurrencyDisplayNames[(int)currencyDisplay] : null);
            Put("currencySign", formatter.CurrencySign is { } currencySign ? CurrencySignNames[(int)currencySign] : null);
            Put("unit", formatter.Unit);
            Put("unitDisplay", formatter.UnitDisplay is { } unitDisplay ? UnitDisplayNames[(int)unitDisplay] : null);
            PutNumber("minimumIntegerDigits", digits.MinimumIntegerDigits);
            PutNumber("minimumFractionDigits", digits.MinimumFractionDigits);
            PutNumber("maximumFractionDigits", digits.MaximumFractionDigits);
            PutNumber("minimumSignificantDigits", digits.MinimumSignificantDigits);
            PutNumber("maximumSignificantDigits", digits.MaximumSignificantDigits);
            options.DefineOrdinary("useGrouping", GroupingNames[(int)formatter.UseGrouping] is { } grouping ? JsValue.String(grouping) : JsValue.Boolean(false));
            Put("notation", NotationNames[(int)formatter.Notation]);
            Put("compactDisplay", formatter.CompactDisplay is { } compactDisplay ? CompactDisplayNames[(int)compactDisplay] : null);
            Put("signDisplay", SignDisplayNames[(int)formatter.SignDisplay]);
            PutDigitTail(options, digits);
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.NumberFormat"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("NumberFormat", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>
    /// The last four slots <c>resolvedOptions</c> reports of a number format or plural rules, in Table
    /// 15's and Table 32's order: <c>roundingIncrement</c>, <c>roundingMode</c>, <c>roundingPriority</c>
    /// (the computed one) and <c>trailingZeroDisplay</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.3.5 s17.3.4; IP=Low; Security=Low; Resources=1; Fingerprint=8BCEC7
    // Broiler-Human:        PENDING
    internal static void PutDigitTail(JsObject options, DigitOptions digits)
    {
        options.DefineOrdinary("roundingIncrement", JsValue.Number(digits.RoundingIncrement));
        options.DefineOrdinary("roundingMode", JsValue.String(RoundingModeNames[(int)digits.RoundingMode]));
        options.DefineOrdinary("roundingPriority", JsValue.String(RoundingPriorityNames[(int)digits.ComputedRoundingPriority]));
        options.DefineOrdinary("trailingZeroDisplay", JsValue.String(TrailingZeroDisplayNames[(int)digits.TrailingZeroDisplay]));
    }

    /// <summary>The two values of a range: both present, each converted, the start first.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.3; IP=Low; Security=Low; Resources=1; Fingerprint=E4C84F
    // Broiler-Human:        PENDING
    private static (IntlDecimal X, IntlDecimal Y) RangeValues(JsEngine engine, JsValue[] arguments)
    {
        var start = Argument(arguments, 0);
        var end = Argument(arguments, 1);

        if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Intl.NumberFormat: a range needs a start and an end");
        }

        var x = ToIntlMathematicalValue(engine, start);
        var y = ToIntlMathematicalValue(engine, end);
        return (x, y);
    }

    /// <summary>PartitionNumberRangePattern's refusal, step 1 (an end is not-a-number), as the <c>RangeError</c> it is.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.21; IP=Low; Security=Low; Resources=0; Fingerprint=F45759
    // Broiler-Human:        PENDING
    private static void RangeCompleted(JsEngine engine, IntlStatus status)
    {
        if (status != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.NumberFormat: a range cannot start or end at NaN");
        }
    }

    /// <summary>An array of <c>{ type, value }</c> objects, with <c>source</c> where a part has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=D18A27
    // Broiler-Human:        PENDING
    private static JsValue PartsArray(JsEngine engine, IReadOnlyList<NumberPart> parts)
    {
        var values = new List<JsValue>(parts.Count);

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

    /// <summary>
    /// ECMA-402's ToIntlMathematicalValue: ToPrimitive here, the value the shared library's
    /// <c>IntlDecimal</c>, a String's read under this family's BigInt policy and charged on the engine's
    /// meter (JSD-0082, sub-area (b), decision D18).
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.16; IP=Low; Security=Medium; Resources=2; Fingerprint=48E68E
    // Broiler-Human:        PENDING
    internal static IntlDecimal ToIntlMathematicalValue(JsEngine engine, JsValue value)
    {
        var primitive = engine.ToPrimitive(value, "number");

        if (primitive.IsBigInt)
        {
            return IntlDecimal.FromBigInt(primitive.AsBigInt().Value, engine.WorkCharge);
        }

        if (primitive.IsString)
        {
            return IntlDecimal.FromString(primitive.AsString(), JsBigInt.Policy, engine.WorkCharge);
        }

        return IntlDecimal.FromNumber(engine.ToNumber(primitive));
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
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.1.1; IP=Low; Security=Medium; Resources=3; Fingerprint=BEF95B
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

        // [[LOCALEDATA]]'S nu VALUES are the library's, the locale's default numbering system first:
        // latn, then the data's others in code-unit order, for this family's tables.
        var data = engine.Intl!.FormatData;
        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(StringComparer.Ordinal) { ["nu"] = numberingSystem }, ["nu"],
            (locale, key) => key == "nu" ? [.. NumberFormatter.NumberingSystemKeyValues(data, locale)] : [null]);

        // SetNumberFormatUnitOptions.
        var style = GetStringOption(engine, options, "style", StyleNames, "decimal")!;
        var currency = GetStringOption(engine, options, "currency", null, null);

        if (currency is null)
        {
            if (style == "currency")
            {
                throw engine.Error("TypeError", "Intl.NumberFormat: the currency style needs a currency");
            }
        }
        else if (!NumberFormatter.IsWellFormedCurrencyCode(currency))
        {
            return engine.ThrowRangeError("Intl.NumberFormat: invalid currency code: " + currency);
        }

        var currencyDisplay = GetStringOption(engine, options, "currencyDisplay", ["code", "symbol", "narrowSymbol", "name"], "symbol")!;
        var currencySign = GetStringOption(engine, options, "currencySign", CurrencySignNames, "standard")!;
        var unit = GetStringOption(engine, options, "unit", null, null);

        if (unit is null)
        {
            if (style == "unit")
            {
                throw engine.Error("TypeError", "Intl.NumberFormat: the unit style needs a unit");
            }
        }
        else if (!NumberFormatter.IsWellFormedUnitIdentifier(unit))
        {
            return engine.ThrowRangeError("Intl.NumberFormat: invalid unit: " + unit);
        }

        var unitDisplay = GetStringOption(engine, options, "unitDisplay", UnitDisplayNames, "short")!;
        var notation = (NumberNotation)Array.IndexOf(NotationNames, GetStringOption(engine, options, "notation", NotationNames, "standard"));
        var numberStyle = (NumberStyle)Array.IndexOf(StyleNames, style);

        var digitOptions = SetNumberFormatDigitOptions(engine, options, DigitOptionRules.NumberFormatDefaults(numberStyle, currency, notation, data), notation);

        var compactDisplay = GetStringOption(engine, options, "compactDisplay", CompactDisplayNames, "short")!;
        var defaultUseGrouping = notation == NumberNotation.Compact ? "min2" : "auto";
        var useGrouping = GetBooleanOrStringNumberFormatOption(engine, options, "useGrouping", ["min2", "auto", "always", "true", "false"], defaultUseGrouping);

        var signDisplay = GetStringOption(engine, options, "signDisplay", SignDisplayNames, "auto")!;

        // THE RECORD THE LIBRARY TAKES (convention 2): what was read, each slot the style does not use
        // ignored by the formatter, as the constructor leaves it unset.
        var formatter = NumberFormatter.Create(data, resolved.Locale, resolved.DataLocale, resolved.Keys["nu"] ?? "latn", new NumberFormatOptions
        {
            Style = numberStyle,
            Currency = currency,
            CurrencyDisplay = (CurrencyDisplay)Array.IndexOf(CurrencyDisplayNames, currencyDisplay),
            CurrencySign = (CurrencySign)Array.IndexOf(CurrencySignNames, currencySign),
            Unit = unit,
            UnitDisplay = (UnitDisplay)Array.IndexOf(UnitDisplayNames, unitDisplay),
            DigitOptions = digitOptions,
            Notation = notation,
            CompactDisplay = (CompactDisplay)Array.IndexOf(CompactDisplayNames, compactDisplay),
            UseGrouping = (NumberGrouping)Array.IndexOf(GroupingNames, useGrouping),
            SignDisplay = (SignDisplay)Array.IndexOf(SignDisplayNames, signDisplay),
        });

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

    /// <summary>
    /// ECMA-402's SetNumberFormatDigitOptions, split at its conversions as the shared library splits it
    /// (JSD-0082, sub-area (b)): every read here, in the specification's order, each checked as it is
    /// read; <c>DigitOptionRules.Plan</c> says which digit pairs to convert, each converted here; the
    /// library takes the decisions after them, and a status it answers is the error the step throws.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.1.2; IP=Low; Security=Medium; Resources=2; Fingerprint=FD4A96
    // Broiler-Falsified-If: a digit option is read more than once or out of ECMA-402's order, or a combination ECMA-402 refuses is admitted
    // Broiler-Human:        PENDING
    internal static DigitOptions SetNumberFormatDigitOptions(JsEngine engine, JsValue options, FractionDigitDefaults defaults, NumberNotation notation)
    {
        var mnid = GetNumberOption(engine, options, "minimumIntegerDigits", 1, 21, 1)!.Value;
        var mnfd = engine.GetProperty(options, "minimumFractionDigits");
        var mxfd = engine.GetProperty(options, "maximumFractionDigits");
        var mnsd = engine.GetProperty(options, "minimumSignificantDigits");
        var mxsd = engine.GetProperty(options, "maximumSignificantDigits");
        var roundingIncrement = GetNumberOption(engine, options, "roundingIncrement", 1, 5000, 1)!.Value;

        if (!DigitOptionRules.IsAdmittedRoundingIncrement(roundingIncrement))
        {
            throw engine.Error("RangeError", "Intl.NumberFormat: roundingIncrement must be one of the admitted increments");
        }

        var roundingMode = GetStringOption(engine, options, "roundingMode", RoundingModeNames, "halfExpand")!;
        var roundingPriority = (RoundingPriority)Array.IndexOf(RoundingPriorityNames, GetStringOption(engine, options, "roundingPriority", RoundingPriorityNames, "auto"));
        var trailingZeroDisplay = GetStringOption(engine, options, "trailingZeroDisplay", TrailingZeroDisplayNames, "auto")!;

        var hasSd = mnsd.Type != JsType.Undefined || mxsd.Type != JsType.Undefined;
        var hasFd = mnfd.Type != JsType.Undefined || mxfd.Type != JsType.Undefined;
        var plan = DigitOptionRules.Plan(hasSd, hasFd, roundingPriority, notation);
        int? minSd = null, maxSd = null, minFd = null, maxFd = null;

        // THE CONVERSIONS THE PLAN SELECTS, the minimum before the maximum.
        if (plan.NeedSignificantDigits && hasSd)
        {
            minSd = DefaultNumberOption(engine, mnsd, 1, 21, 1, "minimumSignificantDigits");
            maxSd = DefaultNumberOption(engine, mxsd, minSd!.Value, 21, 21, "maximumSignificantDigits");
        }

        if (plan.NeedFractionDigits && hasFd)
        {
            minFd = DefaultNumberOption(engine, mnfd, 0, 100, null, "minimumFractionDigits");
            maxFd = DefaultNumberOption(engine, mxfd, 0, 100, null, "maximumFractionDigits");
        }

        var values = new DigitOptionValues(mnid, hasSd, minSd, maxSd, hasFd, minFd, maxFd, roundingIncrement,
            (RoundingMode)Array.IndexOf(RoundingModeNames, roundingMode), roundingPriority,
            (TrailingZeroDisplay)Array.IndexOf(TrailingZeroDisplayNames, trailingZeroDisplay));

        return DigitOptionRules.SetNumberFormatDigitOptions(values, defaults, notation, out var digits) switch
        {
            IntlStatus.Completed => digits!,
            IntlStatus.RoundingIncrementNeedsFractionDigits => throw engine.Error("TypeError", "Intl.NumberFormat: roundingIncrement needs fraction-digit rounding"),
            IntlStatus.RoundingIncrementNeedsEqualFractionDigits => throw engine.Error("RangeError", "Intl.NumberFormat: roundingIncrement needs equal minimum and maximum fraction digits"),
            IntlStatus.FractionDigitsOutOfOrder => throw engine.Error("RangeError", "Intl.NumberFormat: minimumFractionDigits is greater than maximumFractionDigits"),
            _ => throw engine.Error("RangeError", "Intl.NumberFormat: roundingIncrement must be one of the admitted increments"),
        };
    }

    /// <summary>ECMA-402's GetNumberOption.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.14; IP=Low; Security=Low; Resources=1; Fingerprint=E620D8
    // Broiler-Human:        PENDING
    internal static int? GetNumberOption(JsEngine engine, JsValue options, string property, int minimum, int maximum, int? fallback) =>
        DefaultNumberOption(engine, engine.GetProperty(options, property), minimum, maximum, fallback, property);

    /// <summary>
    /// ECMA-402's DefaultNumberOption: ToNumber here, the range check and the floor the shared
    /// library's <c>DigitOptionRules.DefaultNumberOption</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.13; IP=Low; Security=Low; Resources=1; Fingerprint=8459A3
    // Broiler-Human:        PENDING
    internal static int? DefaultNumberOption(JsEngine engine, JsValue value, int minimum, int maximum, int? fallback, string property)
    {
        if (value.Type == JsType.Undefined)
        {
            return fallback;
        }

        if (DigitOptionRules.DefaultNumberOption(engine.ToNumber(value), minimum, maximum, out var result) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", property + " value is out of range.");
        }

        return result;
    }

    /// <summary>
    /// ECMA-402's GetBooleanOrStringNumberFormatOption, with <see langword="true"/> already read as
    /// <c>always</c> and <see langword="false"/> answered as <see langword="null"/>; the strings
    /// <c>"true"</c> and <c>"false"</c> answer the fallback, as the step after it requires.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s9.2.12; IP=Low; Security=Low; Resources=1; Fingerprint=2DF0D7
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

        if (Array.IndexOf(values, text) < 0)
        {
            throw engine.Error("RangeError", "Value " + text + " out of range for option " + property);
        }

        return text;
    }

    /// <summary>A Number's or a BigInt's <c>toLocaleString</c> through the realm's own <c>%Intl.NumberFormat%</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s20.2.1, s20.3.1; IP=Low; Security=Low; Resources=2; Fingerprint=2AD885
    // Broiler-Human:        PENDING
    internal static string ToLocaleNumberString(JsEngine engine, IntlDecimal x, JsValue[] arguments)
    {
        var made = NewNumberFormat(engine, JsValue.Object(engine.Realm.NumberFormatConstructor!), [Argument(arguments, 0), Argument(arguments, 1)], JsValue.Undefined, called: false);
        return ((JsNumberFormatObject)made.AsObject()).Formatter.Format(x, engine.WorkCharge);
    }
}

/// <summary>An <c>Intl.NumberFormat</c>: the shared library's formatter, and its bound <c>format</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B1542
// Broiler-Human:        PENDING
internal sealed class JsNumberFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=29327F
    // Broiler-Human:        PENDING
    internal JsNumberFormatObject(JsObject prototype, NumberFormatter formatter) : base(prototype) => Formatter = formatter;

    internal NumberFormatter Formatter { get; }

    /// <summary>The <c>format</c> getter's function, made the first time it is read.</summary>
    internal JsNativeFunction? BoundFormat { get; set; }
}
