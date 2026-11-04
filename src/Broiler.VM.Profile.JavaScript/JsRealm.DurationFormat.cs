// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           3
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using Broiler.ECMAScript.Intl;
using Broiler.ECMAScript.Temporal;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.DurationFormat</c> (ECMA-402 s13; JSD-0051): a duration's units written by unit number
/// formats, or as a digital clock, and joined by a unit list format.
/// </summary>
/// <remarks>
/// <para>
/// <b>The formatting is the shared Intl library's <c>DurationFormatCore</c></b> since SJL-9.4's sub-area
/// (e) (JSD-0082, decision D24): GetDurationUnitOptions' defaults and checks, [[DigitalFormat]], and
/// PartitionDurationFormatPattern over the library's number and list formatters, made as this family's
/// constructors made them, with a fractional value summed exactly. This file keeps the reads: the
/// options through <c>Base64Options</c>, the profile's GetOptionsObject, each option in the
/// specification's order and checked as it is read; the argument through ToTemporalDuration, or
/// ECMA-402's own ToDurationRecord where Temporal is not composed (row U22); the parts arrays with their
/// per-part charge, <c>resolvedOptions</c>' object, and each <c>RangeError</c> with its message.
/// </para>
/// <para>
/// <i>Revised 2026-11-03, at SJL-9.4's sub-area (e).</i> The remarks read: "<b>Every number is the
/// profile's own <c>Intl.NumberFormat</c>, and the list its own <c>Intl.ListFormat</c></b>, each built
/// by its constructor with the options the draft names, so a duration writes each unit exactly as those
/// formats write it." and "<b>A fractional value is exact.</b> The draft adds milliseconds, microseconds
/// and nanoseconds to a larger unit as a mathematical value, and notes that floating point cannot. The
/// sum is made here as a BigInteger of the smallest unit and written as a decimal string, which the
/// number format reads exactly." The library does both, as this file did.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.DurationFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FB4C4B
    // Broiler-Human:        PENDING
    internal JsObject? DurationFormatPrototype { get; private set; }

    /// <summary><c>%Intl.DurationFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1EF22
    // Broiler-Human:        PENDING
    internal JsNativeFunction? DurationFormatConstructor { get; private set; }

    /// <summary>The <c>style</c> option's values, by <see cref="DurationFormatStyle"/>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 sec-Intl.DurationFormat; IP=Low; Security=Low; Resources=1; Fingerprint=1FD1B8
    // Broiler-Human:        PENDING
    private static readonly string[] DurationStyleNames = ["long", "short", "narrow", "digital"];

    /// <summary>A unit option's values, by <see cref="DurationUnitStyle"/>; <c>fractional</c> is no option's value.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 sec-Intl.DurationFormat Table 20; IP=Low; Security=Low; Resources=1; Fingerprint=1DFBA0
    // Broiler-Human:        PENDING
    private static readonly string[] DurationUnitStyleNames = ["long", "short", "narrow", "numeric", "2-digit", "fractional"];

    /// <summary>The ten fields of a duration record in Table 20's order, as ToDurationRecord stores them.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 sec-todurationrecord; IP=Low; Security=Low; Resources=1; Fingerprint=6979E1
    // Broiler-Human:        PENDING
    private static readonly string[] DurationFieldsInTableOrder =
        ["years", "months", "weeks", "days", "hours", "minutes", "seconds", "milliseconds", "microseconds", "nanoseconds"];

    /// <summary>The fields ToDurationRecord reads, in the order it reads them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1A36FF
    // Broiler-Human:        PENDING
    private static readonly string[] DurationFieldsRead =
        ["days", "hours", "microseconds", "milliseconds", "minutes", "months", "nanoseconds", "seconds", "weeks", "years"];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7F1F09
    // Broiler-Human:        PENDING
    private void SetupDurationFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        DurationFormatPrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "DurationFormat", 0,
            static (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Intl.DurationFormat requires 'new'"),
            static (engine, newTarget, arguments) => NewDurationFormat(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        DurationFormatConstructor = constructor;

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
            var format = DurationFormatOfThis(engine, thisValue, "format");
            return JsValue.String(format.Core.Format(DurationFormatInput(engine, Argument(arguments, 0)), engine.WorkCharge));
        });

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = DurationFormatOfThis(engine, thisValue, "formatToParts");
            var duration = DurationFormatInput(engine, Argument(arguments, 0));
            var values = new List<JsValue>();

            foreach (var (type, value, unit) in format.Core.FormatToParts(duration, engine.WorkCharge))
            {
                engine.Charge(1);
                var item = new JsObject(engine.Realm.ObjectPrototype);
                item.DefineOrdinary("type", JsValue.String(type));
                item.DefineOrdinary("value", JsValue.String(value));

                if (unit is not null)
                {
                    item.DefineOrdinary("unit", JsValue.String(unit));
                }

                values.Add(JsValue.Object(item));
            }

            return JsValue.Object(engine.Realm.NewArray(values));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = DurationFormatOfThis(engine, thisValue, "resolvedOptions").Core;
            var options = new JsObject(engine.Realm.ObjectPrototype);

            // TABLE 21, IN ITS ORDER, as the shared core resolved it: fractional written as numeric.
            options.DefineOrdinary("locale", JsValue.String(format.Locale));
            options.DefineOrdinary("numberingSystem", JsValue.String(format.NumberingSystem));
            options.DefineOrdinary("style", JsValue.String(DurationStyleNames[(int)format.Style]));

            for (var i = 0; i < format.Units.Count; i++)
            {
                var (style, display) = format.Units[i];
                var name = DurationFormatCore.UnitName((DurationUnit)i);
                options.DefineOrdinary(name, JsValue.String(style == DurationUnitStyle.Fractional ? "numeric" : DurationUnitStyleNames[(int)style]));
                options.DefineOrdinary(name + "Display", JsValue.String(display == DurationUnitDisplay.Always ? "always" : "auto"));
            }

            if (format.FractionalDigits is { } digits)
            {
                options.DefineOrdinary("fractionalDigits", JsValue.Number(digits));
            }

            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.DurationFormat"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("DurationFormat", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The duration format <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D53C06
    // Broiler-Human:        PENDING
    private static JsDurationFormatObject DurationFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDurationFormatObject ??
        throw engine.Error("TypeError", "Intl.DurationFormat.prototype." + member + " requires that 'this' be an Intl.DurationFormat");

    /// <summary>ECMA-402's <c>Intl.DurationFormat</c> constructor (s13.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=77E314
    // Broiler-Human:        PENDING
    internal static JsValue NewDurationFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.DurationFormatPrototype!);

        // RESOLVEOPTIONS: the locales, the options through GetOptionsObject, the matcher, and nu.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = JsValue.Object(Base64Options(engine, Argument(arguments, 1)));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            return engine.ThrowRangeError("Intl.DurationFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        // [[LOCALEDATA]]'S nu VALUES are the library's, the locale's default numbering system first:
        // latn, then the data's others in code-unit order, for this family's tables.
        var data = engine.Intl!.FormatData;
        var resolved = ResolveLocale(engine, requested, new Dictionary<string, string?>(System.StringComparer.Ordinal) { ["nu"] = numberingSystem }, ["nu"],
            (locale, key) => key == "nu" ? [.. NumberFormatter.NumberingSystemKeyValues(data, locale)] : [null]);

        // [[DIGITALFORMAT]]: CLDR's time separator, and whether its hours-minutes-seconds pattern
        // writes the hours with two digits, read at the data locale.
        var twoDigitHours = DurationFormatCore.DigitalFormat(data, resolved.DataLocale).TwoDigitHours;

        var style = (DurationFormatStyle)System.Array.IndexOf(DurationStyleNames, GetStringOption(engine, options, "style", DurationStyleNames, "short")!);
        var units = new DurationUnitOptions[DurationFieldsInTableOrder.Length];
        DurationUnitStyle? previous = null;

        for (var i = 0; i < units.Length; i++)
        {
            var unit = (DurationUnit)i;
            units[i] = GetDurationUnitOptions(engine, unit, options, style, previous, twoDigitHours);

            if (unit is DurationUnit.Hours or DurationUnit.Minutes or DurationUnit.Seconds or DurationUnit.Milliseconds or DurationUnit.Microseconds)
            {
                previous = units[i].Style;
            }
        }

        var fractionalDigits = GetNumberOption(engine, options, "fractionalDigits", 0, 9, null);

        return JsValue.Object(new JsDurationFormatObject(
            prototype,
            DurationFormatCore.Create(data, resolved.Locale, resolved.DataLocale, resolved.Keys["nu"] ?? "latn", style, units, fractionalDigits)));
    }

    /// <summary>
    /// ECMA-402's GetDurationUnitOptions (s13.5.6): the unit's two options read here, in order, each
    /// checked as it is read; the defaults and ValidateDurationUnitStyle (s13.5.6.2) are the shared
    /// core's, whose refusals are this family's <c>RangeError</c>s with their messages as before.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=062056
    // Broiler-Human:        PENDING
    private static DurationUnitOptions GetDurationUnitOptions(JsEngine engine, DurationUnit unit, JsValue options, DurationFormatStyle baseStyle, DurationUnitStyle? previous, bool twoDigitHours)
    {
        var name = DurationFormatCore.UnitName(unit);
        var admitted = new List<string>();

        foreach (var admittedStyle in DurationFormatCore.UnitStyles(unit))
        {
            admitted.Add(DurationUnitStyleNames[(int)admittedStyle]);
        }

        var style = GetStringOption(engine, options, name, [.. admitted], null);
        var display = GetStringOption(engine, options, name + "Display", ["auto", "always"], null);

        var status = DurationFormatCore.GetDurationUnitOptions(
            unit,
            style is null ? null : (DurationUnitStyle)System.Array.IndexOf(DurationUnitStyleNames, style),
            display is null ? null : display == "always" ? DurationUnitDisplay.Always : DurationUnitDisplay.Auto,
            baseStyle,
            previous,
            twoDigitHours,
            out var resolved);

        return status switch
        {
            IntlStatus.Completed => resolved,
            IntlStatus.FractionalDurationUnitAlwaysDisplayed => throw engine.Error("RangeError", "Intl.DurationFormat: " + name + " cannot always be displayed as a fraction"),
            IntlStatus.DurationUnitAfterFractional => throw engine.Error("RangeError", "Intl.DurationFormat: " + name + " must be fractional after a fractional unit"),
            _ => throw engine.Error("RangeError", "Intl.DurationFormat: " + name + " must be numeric after a numeric unit"),
        };
    }

    /// <summary>
    /// The duration format and formatToParts read: with Temporal admitted, ToTemporalDuration (the
    /// proposal's amendment of s15.10.1, which takes a string too); without it, ToDurationRecord.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=093A8F
    // Broiler-Human:        PENDING
    private static DurationFields DurationFormatInput(JsEngine engine, JsValue input) => engine.Realm.TemporalDurationPrototype is not null ? JsTemporal.ToDuration(engine, input) : ToDurationRecord(engine, input);

    /// <summary>
    /// ECMA-402's ToDurationRecord (s13.5.3): the ten fields read in alphabetical order, each an
    /// integral Number, and the whole a valid duration by the shared IsValidDuration (JSD-0082,
    /// decision D24, row U22). (Revised 2026-11-03, at SJL-9.4's sub-area (e); its validity read "the
    /// whole a valid duration", by this file's own IsValidDuration, which is gone.)
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6B348C
    // Broiler-Human:        PENDING
    private static DurationFields ToDurationRecord(JsEngine engine, JsValue input)
    {
        if (!input.IsObject)
        {
            throw engine.Error(input.IsString ? "RangeError" : "TypeError", "Intl.DurationFormat: a duration must be an object");
        }

        // THE RECORD IN TABLE 24'S ORDER, READ IN ALPHABETICAL ORDER.
        var record = new double[DurationFieldsInTableOrder.Length];
        var any = false;

        foreach (var field in DurationFieldsRead)
        {
            var value = engine.GetProperty(input, field);

            if (value.Type == JsType.Undefined)
            {
                continue;
            }

            var number = engine.ToNumber(value);

            if (!double.IsFinite(number) || System.Math.Truncate(number) != number)
            {
                throw engine.Error("RangeError", "Intl.DurationFormat: " + field + " must be an integer");
            }

            record[System.Array.IndexOf(DurationFieldsInTableOrder, field)] = number + 0.0;
            any = true;
        }

        if (!any)
        {
            throw engine.Error("TypeError", "Intl.DurationFormat: a duration needs at least one field");
        }

        var duration = new DurationFields(record[0], record[1], record[2], record[3], record[4], record[5], record[6], record[7], record[8], record[9]);

        if (TemporalLimits.CheckDuration(duration) != TemporalStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.DurationFormat: the duration is not valid");
        }

        return duration;
    }

}

/// <summary>
/// An <c>Intl.DurationFormat</c>: the shared library's core, which holds what the constructor resolved.
/// (Revised 2026-11-03, at SJL-9.4's sub-area (e). It read "An <c>Intl.DurationFormat</c>: its resolved
/// options, each unit's style and display.", and held the locale, the numbering system, the style, each
/// unit's style and display, the fractional digits and the two separators, which the core now holds.)
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=24169B
// Broiler-Human:        PENDING
internal sealed class JsDurationFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37F319
    // Broiler-Human:        PENDING
    internal JsDurationFormatObject(JsObject prototype, DurationFormatCore core)
        : base(prototype)
    {
        Core = core;
    }

    /// <summary>The shared library's duration format: the resolved options, and PartitionDurationFormatPattern over them.</summary>
    // Broiler-AI:           Origin=AI; Spec=JSD-0082 sub-area (e); IP=Low; Security=Low; Resources=1; Fingerprint=9B9313
    // Broiler-Human:        PENDING
    internal DurationFormatCore Core { get; }
}
