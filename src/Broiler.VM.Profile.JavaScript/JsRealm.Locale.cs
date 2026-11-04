// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           4
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Low
// Criteria:         1/0
// Resource impact:  1/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using Broiler.ECMAScript.Intl;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.Locale</c> (ECMA-402 s15; JSD-0046): a canonicalized locale identifier with its parts,
/// its keywords as properties, Add and Remove Likely Subtags, and the locale's information.
/// </summary>
/// <remarks>
/// <para>
/// <b>A Locale is any well-formed tag, not only a supported one.</b> Its parts and keywords come from
/// the tag and the options; Add and Remove Likely Subtags and the aliases read CLDR's data for every
/// language, as canonicalization already does.
/// </para>
/// <para>
/// <b>The information methods answer from the data the profile holds</b>: the calendars and numbering
/// systems it formats, the collations <c>Intl.Collator</c> offers, CLDR's hour cycles and weeks for
/// every region and its line direction for every script. Since JSD-0053 the time zones in use in a
/// region are the IANA Time Zone Database's <c>zone.tab</c> entries for it.
/// </para>
/// <para>
/// <b>The constructor's steps and the information are the shared Intl library's since SJL-9.4</b>
/// (JSD-0082, sub-area (a)): <c>LocaleRecord</c> makes the record, <c>LocaleId</c> answers its parts,
/// Add and Remove Likely Subtags, and <c>LocaleInfo</c> the information, over the composition's
/// tables (<see cref="JsIntlTables.LocaleData"/>). The constructor still reads the tag and every
/// option here, in ECMA-402's order, checking each as it reads it, so that a refused option stops
/// the reads where the specification does. Two answers move, as decision D32 names them: row L18,
/// <c>getCollations</c> of a language the Collator lacks answers DefaultLocale's collations, and row
/// L9, a subdivision alias whose replacement is a region is written as UTS #35 Annex C writes it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.Locale.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E43789
    // Broiler-Human:        PENDING
    internal JsObject? LocalePrototype { get; private set; }

    /// <summary><c>%Intl.Locale%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=36E1AC
    // Broiler-Human:        PENDING
    internal JsNativeFunction? LocaleConstructor { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C2803A
    // Broiler-Human:        PENDING
    private void SetupLocale(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        LocalePrototype = prototype;

        var constructor = new JsNativeFunction(this, FunctionPrototype, "Locale", 1,
            static (engine, thisValue, arguments) => throw engine.Error("TypeError", "Constructor Intl.Locale requires 'new'"),
            static (engine, newTarget, arguments) => NewLocale(engine, newTarget, arguments))
        {
            BuildsFromNewTarget = true
        };
        LocaleConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty("constructor", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        void Getter(string name, System.Func<JsEngine, JsLocaleObject, JsValue> read) =>
            prototype.SetOwnProperty(name, JsProperty.Accessor(Native("get " + name, 0, (engine, thisValue, arguments) =>
            read(engine, LocaleOfThis(engine, thisValue, name))), null, JsPropertyAttributes.Configurable));

        static JsValue Optional(string? value) => value is null ? JsValue.Undefined : JsValue.String(value);

        Getter("baseName", static (engine, locale) => JsValue.String(Tag(locale.Locale).BaseName));
        Getter("calendar", static (engine, locale) => Optional(locale.Record.Calendar));
        Getter("caseFirst", static (engine, locale) => Optional(locale.Record.CaseFirst));
        Getter("collation", static (engine, locale) => Optional(locale.Record.Collation));
        Getter("firstDayOfWeek", static (engine, locale) => Optional(locale.Record.FirstDayOfWeek));
        Getter("hourCycle", static (engine, locale) => Optional(locale.Record.HourCycle));
        Getter("language", static (engine, locale) => JsValue.String(Tag(locale.Locale).Language));
        Getter("numberingSystem", static (engine, locale) => Optional(locale.Record.NumberingSystem));
        Getter("numeric", static (engine, locale) => JsValue.Boolean(locale.Record.Numeric));
        Getter("region", static (engine, locale) => Optional(Tag(locale.Locale).Region));
        Getter("script", static (engine, locale) => Optional(Tag(locale.Locale).Script));
        Getter("variants", static (engine, locale) =>
            Tag(locale.Locale).Variants is { Count: > 0 } variants ? JsValue.String(string.Join('-', variants)) : JsValue.Undefined);

        Method(prototype, "maximize", 0, static (engine, thisValue, arguments) =>
        {
            var locale = LocaleOfThis(engine, thisValue, "maximize");
            var maximal = Tag(locale.Locale).Maximize(engine.Intl!.LocaleData, out _);
            return JsValue.Object(NewLocaleObject(engine, engine.Realm.LocalePrototype!, maximal.ToString(), JsValue.Undefined));
        });

        Method(prototype, "minimize", 0, static (engine, thisValue, arguments) =>
        {
            var locale = LocaleOfThis(engine, thisValue, "minimize");
            var minimal = Tag(locale.Locale).Minimize(engine.Intl!.LocaleData, out _);
            return JsValue.Object(NewLocaleObject(engine, engine.Realm.LocalePrototype!, minimal.ToString(), JsValue.Undefined));
        });

        Method(prototype, "toString", 0, static (engine, thisValue, arguments) =>
            JsValue.String(LocaleOfThis(engine, thisValue, "toString").Locale));

        Method(prototype, "getCalendars", 0, static (engine, thisValue, arguments) =>
            StringArray(engine, LocaleInfo.Calendars(LocaleOfThis(engine, thisValue, "getCalendars").Record)));

        // ROW L18 (JSD-0082, decision D32): a language the Collator does not support answers
        // DefaultLocale's collations, and `emoji` and `eor` only `und`, as CollationsOfLocale has it.
        Method(prototype, "getCollations", 0, static (engine, thisValue, arguments) =>
            StringArray(engine, LocaleInfo.Collations(LocaleOfThis(engine, thisValue, "getCollations").Record, CollatorData(engine), DefaultLocale)));

        Method(prototype, "getHourCycles", 0, static (engine, thisValue, arguments) =>
            StringArray(engine, LocaleInfo.HourCycles(LocaleOfThis(engine, thisValue, "getHourCycles").Record, engine.Intl!.LocaleData)));

        Method(prototype, "getNumberingSystems", 0, static (engine, thisValue, arguments) =>
            StringArray(engine, LocaleInfo.NumberingSystems(LocaleOfThis(engine, thisValue, "getNumberingSystems").Record, AvailableLocales(engine), engine.Intl!.LocaleData)));

        Method(prototype, "getTimeZones", 0, static (engine, thisValue, arguments) =>
        {
            var locale = LocaleOfThis(engine, thisValue, "getTimeZones");

            // THE ZONES IN USE IN A REGION are zone.tab's for it, each a primary identifier
            // (ECMA-402 6.5 and 15.5.13, JSD-0053), in ordinal order: the shared database's answer
            // since SJL-8.5 (JSD-0076), through the shared Intl library's TimeZonesOfLocale since SJL-9.4.
            return LocaleInfo.TimeZones(locale.Record, engine.Intl!.TimeZones) is { } zones
                ? StringArray(engine, zones)
                : JsValue.Undefined;
        });

        Method(prototype, "getTextInfo", 0, static (engine, thisValue, arguments) =>
        {
            var locale = LocaleOfThis(engine, thisValue, "getTextInfo");
            var info = new JsObject(engine.Realm.ObjectPrototype);
            info.DefineOrdinary("direction", Optional(LocaleInfo.TextDirection(locale.Record, engine.Intl!.LocaleData)));
            return JsValue.Object(info);
        });

        Method(prototype, "getWeekInfo", 0, static (engine, thisValue, arguments) =>
        {
            var locale = LocaleOfThis(engine, thisValue, "getWeekInfo");
            var week = LocaleInfo.WeekInfo(locale.Record, engine.Intl!.LocaleData);
            var weekend = new List<JsValue>(week.Weekend.Count);

            foreach (var day in week.Weekend)
            {
                weekend.Add(JsValue.Number(day));
            }

            var info = new JsObject(engine.Realm.ObjectPrototype);
            info.DefineOrdinary("firstDay", JsValue.Number(week.FirstDay));
            info.DefineOrdinary("weekend", JsValue.Object(engine.Realm.NewArray(weekend)));
            return JsValue.Object(info);
        });

        prototype.SetOwnSymbol(ToStringTagSymbol, JsProperty.Data(JsValue.String("Intl.Locale"), JsPropertyAttributes.Configurable));
        intl.SetOwnProperty("Locale", JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The parsed form of a canonicalized locale identifier a Locale holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65E806
    // Broiler-Human:        PENDING
    private static LocaleId Tag(string locale) => ParseLocale(locale)!;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B41AE4
    // Broiler-Human:        PENDING
    private static JsValue StringArray(JsEngine engine, IReadOnlyList<string> values)
    {
        var array = new List<JsValue>(values.Count);

        foreach (var value in values)
        {
            array.Add(JsValue.String(value));
        }

        return JsValue.Object(engine.Realm.NewArray(array));
    }

    /// <summary>The Locale <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=14917F
    // Broiler-Human:        PENDING
    private static JsLocaleObject LocaleOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsLocaleObject ??
        throw engine.Error("TypeError", "Intl.Locale.prototype." + member + " requires that 'this' be an Intl.Locale");

    /// <summary>ECMA-402's <c>Intl.Locale</c> constructor (s15.1.1).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D826B8
    // Broiler-Human:        PENDING
    internal static JsValue NewLocale(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.LocalePrototype!);
        var tag = Argument(arguments, 0);

        if (!tag.IsString && !tag.IsObject)
        {
            throw engine.Error("TypeError", "Intl.Locale: the tag is neither a String nor an Object");
        }

        var text = tag.AsObjectOrNull() is JsLocaleObject existing ? existing.Locale : engine.ToStringValue(tag);
        return JsValue.Object(NewLocaleObject(engine, prototype, text, Argument(arguments, 1)));
    }

    /// <summary>
    /// The steps of the constructor after the tag is a String: the options read in order, each checked
    /// as it is read, and the record made by the shared library (JSD-0082, rows L13 and L14).
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s15.1.1; IP=Low; Security=Low; Resources=1; Fingerprint=25C3BB
    // Broiler-Falsified-If: an option is read in another order than ECMA-402's, a read follows an option the specification refuses before it, or the record differs from the one the shared library makes for the same tag and options
    // Broiler-Human:        PENDING
    private static JsLocaleObject NewLocaleObject(JsEngine engine, JsObject prototype, string text, JsValue optionsValue)
    {
        var options = CoerceOptionsToObject(engine, optionsValue);

        if (LocaleId.TryParse(text, out _) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Incorrect locale information provided: " + text);
        }

        // UPDATELANGUAGEID'S READS: each option checked as it is read, its absence the tag's own subtag.
        var language = GetStringOption(engine, options, "language", null, null);

        if (language is not null && LocaleRecord.CheckLanguage(language) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.Locale: invalid language subtag: " + language);
        }

        var script = GetStringOption(engine, options, "script", null, null);

        if (script is not null && LocaleRecord.CheckScript(script) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.Locale: invalid script subtag: " + script);
        }

        var region = GetStringOption(engine, options, "region", null, null);

        if (region is not null && LocaleRecord.CheckRegion(region) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.Locale: invalid region subtag: " + region);
        }

        var variants = GetStringOption(engine, options, "variants", null, null);

        if (variants is not null && LocaleRecord.CheckVariants(variants) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", variants.Length == 0
                ? "Intl.Locale: the variants option is empty"
                : "Intl.Locale: invalid variants: " + variants);
        }

        string? TypeOption(string property)
        {
            var value = GetStringOption(engine, options, property, null, null);

            if (value is not null && LocaleRecord.CheckType(value) != IntlStatus.Completed)
            {
                throw engine.Error("RangeError", "Intl.Locale: the " + property + " option is not a well-formed type: " + value);
            }

            return value;
        }

        var calendar = TypeOption("calendar");
        var collation = TypeOption("collation");
        var firstDay = GetStringOption(engine, options, "firstDayOfWeek", null, null);

        if (firstDay is not null && LocaleRecord.CheckFirstDayOfWeek(firstDay) != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Intl.Locale: the firstDayOfWeek option is not a well-formed type: " + LocaleRecord.WeekdayToUValue(firstDay));
        }

        var hourCycle = GetStringOption(engine, options, "hourCycle", ["h11", "h12", "h23", "h24"], null);
        var caseFirst = GetStringOption(engine, options, "caseFirst", ["upper", "lower", "false"], null);
        var numeric = GetBooleanOption(engine, options, "numeric", null);
        var numberingSystem = TypeOption("numberingSystem");

        // MAKELOCALERECORD AND THE CANONICAL TAG, the shared library's, every option already checked.
        var status = LocaleRecord.TryCreate(text, new LocaleOptions
        {
            Language = language,
            Script = script,
            Region = region,
            Variants = variants,
            Calendar = calendar,
            Collation = collation,
            FirstDayOfWeek = firstDay,
            HourCycle = hourCycle,
            CaseFirst = caseFirst,
            Numeric = numeric,
            NumberingSystem = numberingSystem,
        }, engine.Intl!.LocaleData, out var record);

        if (status != IntlStatus.Completed)
        {
            throw engine.Error("RangeError", "Incorrect locale information provided: " + text);
        }

        return new JsLocaleObject(prototype, record!);
    }
}

/// <summary>An <c>Intl.Locale</c>: its record, the shared library's (ECMA-402's internal slots).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A1974E
// Broiler-Human:        PENDING
internal sealed class JsLocaleObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3037C5
    // Broiler-Human:        PENDING
    internal JsLocaleObject(JsObject prototype, LocaleRecord record) : base(prototype) => Record = record;

    /// <summary>[[Locale]] and the seven keyword slots.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FBDE00
    // Broiler-Human:        PENDING
    internal LocaleRecord Record { get; }

    /// <summary>[[Locale]], the canonical locale identifier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=459B7C
    // Broiler-Human:        PENDING
    internal string Locale => Record.Locale;
}
