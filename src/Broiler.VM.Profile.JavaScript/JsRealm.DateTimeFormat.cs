// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   25
// Annotated:        25/25
// Exempt:           10
// Human-reviewed:   0/25
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       25
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.DateTimeFormat</c>, the three <c>Date.prototype.toLocale*</c> methods through it, and
/// <c>Intl.supportedValuesOf</c> (ECMA-402 s11, s8.3.2; JSD-0045).
/// </summary>
/// <remarks>
/// <para>
/// <b>The constructor reads its options in ECMA-402's order</b> and keeps what it resolved. A format
/// built from components takes its pattern from <see cref="JsDatePatternGenerator"/> and one built
/// from a style takes CLDR's style patterns; both are written with the resolved hour cycle's letter.
/// </para>
/// <para>
/// <b>The time zones are the IANA Time Zone Database's</b> (JSD-0053), from tzdb 2026e. An offset
/// resolves to its <c>+HH:MM</c> or <c>-HH:MM</c> form, and a name, in any ASCII case, to its primary
/// identifier: a name resolving to UTC to <c>UTC</c>, IANA's <c>Etc/GMT+N</c> and <c>Etc/GMT-N</c>
/// zones to themselves as fixed offsets, and any other to its zone, whose offset is read at each
/// instant formatted. Any other name is a <c>RangeError</c>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.DateTimeFormat.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ADABE5
    // Broiler-Human:        PENDING
    internal JsObject? DateTimeFormatPrototype { get; private set; }

    /// <summary><c>%Intl.DateTimeFormat%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60BC36
    // Broiler-Human:        PENDING
    internal JsNativeFunction? DateTimeFormatConstructor { get; private set; }

    /// <summary>ECMA-402's table of date-time components, in table order, with the values each admits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55DD20
    // Broiler-Human:        PENDING
    private static readonly (string Name, string[]? Values)[] DateTimeComponents =
    [
        ("weekday", ["narrow", "short", "long"]),
        ("era", ["narrow", "short", "long"]),
        ("year", ["2-digit", "numeric"]),
        ("month", ["2-digit", "numeric", "narrow", "short", "long"]),
        ("day", ["2-digit", "numeric"]),
        ("dayPeriod", ["narrow", "short", "long"]),
        ("hour", ["2-digit", "numeric"]),
        ("minute", ["2-digit", "numeric"]),
        ("second", ["2-digit", "numeric"]),
        ("fractionalSecondDigits", null),
        ("timeZoneName", ["short", "long", "shortOffset", "longOffset", "shortGeneric", "longGeneric"]),
    ];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F0D032
    // Broiler-Human:        PENDING
    private void SetupDateTimeFormat(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        DateTimeFormatPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "DateTimeFormat",
            0,
            static (engine, thisValue, arguments) =>
                NewDateTimeFormat(engine, JsValue.Object(engine.Realm.DateTimeFormatConstructor!), arguments, thisValue, called: true),
            static (engine, newTarget, arguments) => NewDateTimeFormat(engine, newTarget, arguments, JsValue.Undefined, called: false));

        constructor.BuildsFromNewTarget = true;
        DateTimeFormatConstructor = constructor;

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
                    var format = UnwrapDateTimeFormat(engine, thisValue, "format");

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
                                var date = Argument(values, 0);
                                var x = date.Type == JsType.Undefined ? DateCurrentTime() : inner.ToNumber(date);
                                return JsValue.String(Joined(format.Formatter.Parts(inner, ClippedTime(inner, x), null)));
                            });
                    }

                    return JsValue.Object(format.BoundFormat);
                }),
                null,
                JsPropertyAttributes.Configurable));

        Method(prototype, "formatToParts", 1, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatToParts");
            var date = Argument(arguments, 0);
            var x = date.Type == JsType.Undefined ? DateCurrentTime() : engine.ToNumber(date);
            return DatePartsArray(engine, format.Formatter.Parts(engine, ClippedTime(engine, x), null));
        });

        Method(prototype, "formatRange", 2, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatRange");
            var (x, y) = DateRangeValues(engine, arguments);
            return JsValue.String(Joined(format.Formatter.RangeParts(engine, x, y)));
        });

        Method(prototype, "formatRangeToParts", 2, static (engine, thisValue, arguments) =>
        {
            var format = DateTimeFormatOfThis(engine, thisValue, "formatRangeToParts");
            var (x, y) = DateRangeValues(engine, arguments);
            return DatePartsArray(engine, format.Formatter.RangeParts(engine, x, y));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var format = UnwrapDateTimeFormat(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);

            void Put(string name, string? value)
            {
                if (value is not null)
                {
                    options.DefineOrdinary(name, JsValue.String(value));
                }
            }

            Put("locale", format.Locale);
            Put("calendar", format.Calendar);
            Put("numberingSystem", format.NumberingSystem);
            Put("timeZone", format.Formatter.TimeZone);

            if (format.HourCycle is { } hourCycle)
            {
                Put("hourCycle", hourCycle);
                options.DefineOrdinary("hour12", JsValue.Boolean(hourCycle is "h11" or "h12"));
            }

            if (format.DateStyle is null && format.TimeStyle is null)
            {
                var components = ComponentsOf(format.Formatter.Pattern);

                foreach (var (name, _) in DateTimeComponents)
                {
                    if (!components.TryGetValue(name, out var value))
                    {
                        continue;
                    }

                    if (name == "fractionalSecondDigits")
                    {
                        options.DefineOrdinary(name, JsValue.Number(value.Length));
                    }
                    else
                    {
                        Put(name, value);
                    }
                }
            }

            Put("dateStyle", format.DateStyle);
            Put("timeStyle", format.TimeStyle);
            return JsValue.Object(options);
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.DateTimeFormat"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "DateTimeFormat",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>ECMA-402's <c>Intl.supportedValuesOf</c>: the values this data supports for a key, sorted.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s8.3.2; IP=Low; Security=Low; Resources=2; Fingerprint=64EB63
    // Broiler-Human:        PENDING
    private static JsValue SupportedValuesOf(JsEngine engine, JsValue key)
    {
        var name = engine.ToStringValue(key);
        var tables = engine.Intl!;
        var values = new System.Collections.Generic.List<string>();

        switch (name)
        {
            case "calendar":
                values.Add("gregory");
                break;
            case "collation":
                values.Add("phonebk");
                break;
            case "currency":
                foreach (var entry in tables.Numbers.Currencies.Keys)
                {
                    var code = entry[(entry.IndexOf('|') + 1)..];

                    if (code.Length == 3 && !values.Contains(code))
                    {
                        values.Add(code);
                    }
                }

                break;
            case "numberingSystem":
                values.AddRange(tables.Numbers.NumberingSystems.Keys);
                break;
            case "timeZone":
                // ECMA-402 6.5.3: every primary identifier the time zone data names (JSD-0053).
                values.AddRange(tables.TimeZones.PrimaryIdentifiers);
                break;
            case "unit":
                values.AddRange(SanctionedUnits);
                break;
            default:
                throw engine.Error("RangeError", "Intl.supportedValuesOf: invalid key: " + name);
        }

        values.Sort(System.StringComparer.Ordinal);
        return JsValue.Object(engine.Realm.NewArray(values.ConvertAll(static value => JsValue.String(value))));
    }

    /// <summary>The text of a list of parts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E219F3
    // Broiler-Human:        PENDING
    private static string Joined(System.Collections.Generic.List<JsDatePart> parts)
    {
        var text = new System.Text.StringBuilder();

        foreach (var part in parts)
        {
            text.Append(part.Value);
        }

        return text.ToString();
    }

    /// <summary>A time value TimeClip keeps, or the <c>RangeError</c> for one it does not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6118D5
    // Broiler-Human:        PENDING
    private static double ClippedTime(JsEngine engine, double x)
    {
        var clipped = DateTimeClip(x);

        if (double.IsNaN(clipped))
        {
            throw engine.Error("RangeError", "Invalid time value");
        }

        return clipped;
    }

    /// <summary>The two ends of a range: both present, both numbers, both times.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F70F1D
    // Broiler-Human:        PENDING
    private static (double X, double Y) DateRangeValues(JsEngine engine, JsValue[] arguments)
    {
        var start = Argument(arguments, 0);
        var end = Argument(arguments, 1);

        if (start.Type == JsType.Undefined || end.Type == JsType.Undefined)
        {
            throw engine.Error("TypeError", "Intl.DateTimeFormat: a range needs a start and an end");
        }

        var x = engine.ToNumber(start);
        var y = engine.ToNumber(end);
        return (ClippedTime(engine, x), ClippedTime(engine, y));
    }

    /// <summary>An array of <c>{ type, value }</c> objects, with <c>source</c> where a part has one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9445FE
    // Broiler-Human:        PENDING
    private static JsValue DatePartsArray(JsEngine engine, System.Collections.Generic.List<JsDatePart> parts)
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

    /// <summary>The date-time format <paramref name="thisValue"/> is, with no unwrapping, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=840F34
    // Broiler-Human:        PENDING
    private static JsDateTimeFormatObject DateTimeFormatOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsDateTimeFormatObject ??
        throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an Intl.DateTimeFormat");

    /// <summary>
    /// ECMA-402's UnwrapDateTimeFormat: the format itself, or the one a legacy construction stored on
    /// an object inheriting from <c>Intl.DateTimeFormat.prototype</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=377996
    // Broiler-Human:        PENDING
    private static JsDateTimeFormatObject UnwrapDateTimeFormat(JsEngine engine, JsValue thisValue, string member)
    {
        if (!thisValue.IsObject)
        {
            throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an object");
        }

        if (thisValue.AsObject() is JsDateTimeFormatObject direct)
        {
            return direct;
        }

        var realm = engine.Realm;

        if (engine.OrdinaryHasInstance(JsValue.Object(realm.DateTimeFormatConstructor!), thisValue) &&
            engine.GetSymbolWithReceiver(thisValue.AsObject(), realm.IntlFallbackSymbol!, thisValue).AsObjectOrNull() is JsDateTimeFormatObject wrapped)
        {
            return wrapped;
        }

        throw engine.Error("TypeError", "Intl.DateTimeFormat.prototype." + member + " requires that 'this' be an Intl.DateTimeFormat");
    }

    /// <summary>
    /// ECMA-402's <c>Intl.DateTimeFormat</c> constructor: the prototype from the new target, then
    /// CreateDateTimeFormat, then the legacy path when called as a function.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D564EF
    // Broiler-Human:        PENDING
    internal static JsValue NewDateTimeFormat(JsEngine engine, JsValue newTarget, JsValue[] arguments, JsValue thisValue, bool called)
    {
        var realm = engine.Realm;
        var prototype = engine.PrototypeFromConstructor(newTarget, realm.DateTimeFormatPrototype!);
        var made = CreateDateTimeFormat(engine, prototype, Argument(arguments, 0), Argument(arguments, 1), "any", "date");

        // CHAINDATETIMEFORMAT, ECMA-402'S NORMATIVE OPTIONAL LEGACY PATH.
        if (called && thisValue.IsObject &&
            engine.OrdinaryHasInstance(JsValue.Object(realm.DateTimeFormatConstructor!), thisValue))
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
    /// The text a <c>Date.prototype.toLocale*</c> method answers through <c>Intl.DateTimeFormat</c>:
    /// a format made for the call with <paramref name="required"/> and <paramref name="defaults"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BDC6D2
    // Broiler-Human:        PENDING
    internal static string ToLocaleDateString(JsEngine engine, double time, JsValue[] arguments, string required, string defaults)
    {
        var format = CreateDateTimeFormat(
            engine, engine.Realm.DateTimeFormatPrototype!, Argument(arguments, 0), Argument(arguments, 1), required, defaults);
        return Joined(format.Formatter.Parts(engine, time, null));
    }

    /// <summary>ECMA-402's CreateDateTimeFormat (s11.1.2).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=85683F
    // Broiler-Human:        PENDING
    internal static JsDateTimeFormatObject CreateDateTimeFormat(
        JsEngine engine,
        JsObject prototype,
        JsValue locales,
        JsValue optionsValue,
        string required,
        string defaults)
    {
        var requested = CanonicalizeLocaleList(engine, locales);
        var options = CoerceOptionsToObject(engine, optionsValue);

        // RESOLVEOPTIONS: the locale matcher, then each resolution option in the constructor's order.
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");
        var calendar = GetStringOption(engine, options, "calendar", null, null);

        if (calendar is not null && !IsTypeSequence(calendar))
        {
            throw engine.Error("RangeError", "Intl.DateTimeFormat: the calendar option is not a well-formed type: " + calendar);
        }

        var numberingSystem = GetStringOption(engine, options, "numberingSystem", null, null);

        if (numberingSystem is not null && !IsTypeSequence(numberingSystem))
        {
            throw engine.Error("RangeError", "Intl.DateTimeFormat: the numberingSystem option is not a well-formed type: " + numberingSystem);
        }

        var hour12 = GetBooleanOption(engine, options, "hour12", null);
        var hourCycle = GetStringOption(engine, options, "hourCycle", ["h11", "h12", "h23", "h24"], null);

        var tables = engine.Intl!;
        var numbers = tables.Numbers;
        var dates = tables.Dates;
        var systems = new System.Collections.Generic.List<string?> { "latn" };

        foreach (var system in numbers.NumberingSystems.Keys)
        {
            if (system != "latn")
            {
                systems.Add(system);
            }
        }

        systems.Sort(1, systems.Count - 1, System.StringComparer.Ordinal);
        var nuValues = systems.ToArray();

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal)
            {
                ["ca"] = calendar,
                ["hc"] = hour12 is null ? hourCycle : NullOption,
                ["nu"] = numberingSystem,
            },
            ["ca", "hc", "nu"],
            (locale, key) => key switch
            {
                "ca" => ["gregory"],
                "hc" => [null, "h11", "h12", "h23", "h24"],
                "nu" => nuValues,
                _ => [null],
            });

        var tag = JsLocaleTag.Parse(resolved.DataLocale);
        var language = tag?.Language ?? "en";
        var region = tag?.Region is { Length: > 0 } explicitRegion
            ? explicitRegion
            : tables.Likely(language) is { } likely ? likely[(likely.LastIndexOf('-') + 1)..] : "001";
        var preferred = dates.PreferredHourChar(region);
        var defaultCycle = HourCycleOfChar(preferred);

        var hc = hour12 switch
        {
            true => defaultCycle is "h11" or "h12" ? defaultCycle : "h12",
            false => defaultCycle is "h23" or "h24" ? defaultCycle : "h23",
            null => resolved.Keys["hc"] ?? defaultCycle,
        };

        var (timeZone, offset, kind, zone) = ResolveTimeZone(engine, engine.GetProperty(options, "timeZone"));

        var components = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
        var hasExplicitComponents = false;

        foreach (var (name, values) in DateTimeComponents)
        {
            string? value;

            if (name == "fractionalSecondDigits")
            {
                value = GetNumberOption(engine, options, name, 1, 3, null) is { } digits
                    ? new string('S', digits)
                    : null;
            }
            else
            {
                value = GetStringOption(engine, options, name, values, null);
            }

            if (value is not null)
            {
                components[name] = value;
                hasExplicitComponents = true;
            }
        }

        _ = GetStringOption(engine, options, "formatMatcher", ["basic", "best fit"], "best fit");
        var dateStyle = GetStringOption(engine, options, "dateStyle", ["full", "long", "medium", "short"], null);
        var timeStyle = GetStringOption(engine, options, "timeStyle", ["full", "long", "medium", "short"], null);

        var hcChar = hc switch { "h11" => 'K', "h12" => 'h', "h24" => 'k', _ => 'H' };
        var decimalSymbol = numbers.Value(language, "symbols.decimal") ?? ".";
        var generator = dates.Generator(language, resolved.Keys["hc"] is { } keyword ? CharOfHourCycle(keyword) : preferred, decimalSymbol);
        string pattern;

        if (dateStyle is not null || timeStyle is not null)
        {
            if (hasExplicitComponents)
            {
                throw engine.Error("TypeError", "Intl.DateTimeFormat: dateStyle and timeStyle cannot be combined with date-time components");
            }

            if (required == "date" && timeStyle is not null)
            {
                throw engine.Error("TypeError", "Intl.DateTimeFormat: a date format cannot have a timeStyle");
            }

            if (required == "time" && dateStyle is not null)
            {
                throw engine.Error("TypeError", "Intl.DateTimeFormat: a time format cannot have a dateStyle");
            }

            pattern = StylePattern(dates, language, dateStyle, timeStyle, hc, hcChar, generator);
        }
        else
        {
            var needDefaults = true;

            if (required is "date" or "any")
            {
                needDefaults &= !(components.ContainsKey("weekday") || components.ContainsKey("year") ||
                    components.ContainsKey("month") || components.ContainsKey("day"));
            }

            if (required is "time" or "any")
            {
                needDefaults &= !(components.ContainsKey("dayPeriod") || components.ContainsKey("hour") ||
                    components.ContainsKey("minute") || components.ContainsKey("second") || components.ContainsKey("fractionalSecondDigits"));
            }

            if (needDefaults && defaults is "date" or "all")
            {
                components["year"] = "numeric";
                components["month"] = "numeric";
                components["day"] = "numeric";
            }

            if (needDefaults && defaults is "time" or "all")
            {
                components["hour"] = "numeric";
                components["minute"] = "numeric";
                components["second"] = "numeric";
            }

            pattern = ReplaceHourCycle(
                generator.BestPattern(SkeletonOfComponents(components, hcChar), JsDatePatternGenerator.MatchHourLength), hcChar);
        }

        var digitsOfSystem = numbers.NumberingSystems.TryGetValue(resolved.Keys["nu"] ?? "latn", out var digitList) ? digitList : numbers.NumberingSystems["latn"];
        var patternCycle = HourCycleOf(pattern);
        var intervalChar = patternCycle is null ? preferred : hcChar;

        var formatter = new JsDateTimeFormatter(
            dates,
            language,
            digitsOfSystem,
            timeZone,
            offset,
            kind,
            pattern,
            () => new JsDateIntervalFormat(dates, language, dates.Generator(language, intervalChar, decimalSymbol), JsDatePatternGenerator.SkeletonOf(pattern)),
            zone);

        return new JsDateTimeFormatObject(prototype, formatter)
        {
            Locale = resolved.Locale,
            Calendar = resolved.Keys["ca"] ?? "gregory",
            NumberingSystem = resolved.Keys["nu"] ?? "latn",
            HourCycle = patternCycle is null ? null : hc,
            DateStyle = dateStyle,
            TimeStyle = timeStyle,
        };
    }

    /// <summary>The hour cycle a CLDR hour letter writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=94175B
    // Broiler-Human:        PENDING
    private static string HourCycleOfChar(char c) => c switch { 'h' => "h12", 'K' => "h11", 'k' => "h24", _ => "h23" };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B7F3F8
    // Broiler-Human:        PENDING
    private static char CharOfHourCycle(string hc) => hc switch { "h11" => 'K', "h12" => 'h', "h24" => 'k', _ => 'H' };

    /// <summary>The hour cycle of a pattern's first hour field, or nothing for a pattern without an hour.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=630771
    // Broiler-Human:        PENDING
    private static string? HourCycleOf(string pattern)
    {
        var quoted = false;

        foreach (var c in pattern)
        {
            if (c == '\'')
            {
                quoted = !quoted;
            }
            else if (!quoted && c is 'h' or 'H' or 'k' or 'K')
            {
                return HourCycleOfChar(c);
            }
        }

        return null;
    }

    /// <summary>A pattern with every hour field written in <paramref name="hourChar"/>, as V8 writes the resolved cycle into ICU's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8024E7
    // Broiler-Human:        PENDING
    private static string ReplaceHourCycle(string pattern, char hourChar)
    {
        var result = new System.Text.StringBuilder(pattern.Length + 1);
        var replace = true;
        var last = '\0';

        foreach (var c in pattern)
        {
            if (c == '\'')
            {
                replace = !replace;
                result.Append(c);
            }
            else if (c is 'h' or 'H' or 'k' or 'K')
            {
                if (replace && last == 'd')
                {
                    result.Append(' ');
                }

                result.Append(replace ? hourChar : c);
            }
            else
            {
                result.Append(c);
            }

            last = c;
        }

        return result.ToString();
    }

    /// <summary>The skeleton of the requested components, the hour in the resolved cycle's letter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C685D7
    // Broiler-Human:        PENDING
    private static string SkeletonOfComponents(System.Collections.Generic.Dictionary<string, string> components, char hourChar)
    {
        var skeleton = new System.Text.StringBuilder();

        string? Value(string name) => components.TryGetValue(name, out var value) ? value : null;

        skeleton.Append(Value("weekday") switch { "narrow" => "EEEEE", "short" => "EEE", "long" => "EEEE", _ => string.Empty });
        skeleton.Append(Value("era") switch { "narrow" => "GGGGG", "short" => "GGG", "long" => "GGGG", _ => string.Empty });
        skeleton.Append(Value("year") switch { "2-digit" => "yy", "numeric" => "y", _ => string.Empty });
        skeleton.Append(Value("month") switch
        {
            "narrow" => "MMMMM",
            "long" => "MMMM",
            "short" => "MMM",
            "2-digit" => "MM",
            "numeric" => "M",
            _ => string.Empty,
        });
        skeleton.Append(Value("day") switch { "2-digit" => "dd", "numeric" => "d", _ => string.Empty });
        skeleton.Append(Value("dayPeriod") switch { "narrow" => "BBBBB", "long" => "BBBB", "short" => "B", _ => string.Empty });
        skeleton.Append(Value("hour") switch { "2-digit" => new string(hourChar, 2), "numeric" => hourChar.ToString(), _ => string.Empty });
        skeleton.Append(Value("minute") switch { "2-digit" => "mm", "numeric" => "m", _ => string.Empty });
        skeleton.Append(Value("second") switch { "2-digit" => "ss", "numeric" => "s", _ => string.Empty });
        skeleton.Append(Value("fractionalSecondDigits") ?? string.Empty);
        skeleton.Append(Value("timeZoneName") switch
        {
            "short" => "z",
            "long" => "zzzz",
            "shortOffset" => "O",
            "longOffset" => "OOOO",
            "shortGeneric" => "v",
            "longGeneric" => "vvvv",
            _ => string.Empty,
        });

        return skeleton.ToString();
    }

    /// <summary>
    /// The pattern of a date style, a time style or both, joined by CLDR's date-time pattern for the
    /// date style; a time whose hour cycle is not the resolved one is regenerated from its skeleton.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2A1CD8
    // Broiler-Human:        PENDING
    private static string StylePattern(
        JsDateData dates,
        string language,
        string? dateStyle,
        string? timeStyle,
        string hc,
        char hcChar,
        JsDatePatternGenerator generator)
    {
        var date = dateStyle is null ? null : dates.Value(language, "dateFormats." + dateStyle);
        var time = timeStyle is null ? null : dates.Value(language, "timeFormats." + timeStyle);
        var pattern = date is not null && time is not null
            ? JsDatePatternGenerator.Substitute(dates.Value(language, "atTime." + dateStyle) ?? "{1} {0}", time, date, string.Empty)
            : date ?? time ?? string.Empty;

        if (time is null || HourCycleOf(pattern) == hc)
        {
            return pattern;
        }

        var skeleton = new System.Text.StringBuilder();

        foreach (var c in JsDatePatternGenerator.SkeletonOf(pattern))
        {
            if (c is 'h' or 'H' or 'k' or 'K')
            {
                skeleton.Append(hcChar);
            }
            else if (c is not ('a' or 'b' or 'B'))
            {
                skeleton.Append(c);
            }
        }

        return ReplaceHourCycle(generator.BestPattern(skeleton.ToString(), JsDatePatternGenerator.MatchHourLength), hcChar);
    }

    /// <summary>The components a pattern writes, as resolvedOptions reports them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CB3A2B
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.Dictionary<string, string> ComponentsOf(string pattern)
    {
        var components = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
        var tokens = JsDatePatternGenerator.Tokens(pattern);

        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];

            if (token[0] == '\'')
            {
                index = JsDatePatternGenerator.QuoteEnd(tokens, index, null);
                continue;
            }

            var count = token.Length;
            string Text() => count <= 3 ? "short" : count == 4 ? "long" : "narrow";
            string Digits() => count == 2 ? "2-digit" : "numeric";

            switch (token[0])
            {
                case 'G':
                    components["era"] = Text();
                    break;
                case 'y':
                    components["year"] = Digits();
                    break;
                case 'M' or 'L':
                    components["month"] = count <= 2 ? Digits() : Text();
                    break;
                case 'E' or 'c':
                    components["weekday"] = count == 6 ? "short" : Text();
                    break;
                case 'd':
                    components["day"] = Digits();
                    break;
                case 'B' or 'b':
                    components["dayPeriod"] = Text();
                    break;
                case 'h' or 'H' or 'k' or 'K':
                    components["hour"] = Digits();
                    break;
                case 'm':
                    components["minute"] = Digits();
                    break;
                case 's':
                    components["second"] = Digits();
                    break;
                case 'S':
                    components["fractionalSecondDigits"] = token;
                    break;
                case 'z':
                    components["timeZoneName"] = count < 4 ? "short" : "long";
                    break;
                case 'O':
                    components["timeZoneName"] = count < 4 ? "shortOffset" : "longOffset";
                    break;
                case 'v':
                    components["timeZoneName"] = count < 4 ? "shortGeneric" : "longGeneric";
                    break;
            }
        }

        return components;
    }

    /// <summary>
    /// The time zone an option names: UTC by default, an offset string by its <c>+HH:MM</c> or <c>-HH:MM</c> form,
    /// and otherwise an IANA Zone or Link name, matched without regard to ASCII case and resolved to its
    /// primary identifier (ECMA-402 11.1.2 steps 29 to 36, JSD-0053): a name resolving to UTC as
    /// <c>UTC</c>, an <c>Etc/GMT</c> zone with an hour as its fixed offset, and any other as the zone.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65FECD
    // Broiler-Human:        PENDING
    private static (string Identifier, int Offset, JsZoneKind Kind, JsZone? Zone) ResolveTimeZone(JsEngine engine, JsValue value)
    {
        if (value.Type == JsType.Undefined)
        {
            return ("UTC", 0, JsZoneKind.Utc, null);
        }

        var text = engine.ToStringValue(value);

        if (ParseOffset(text, out var minutes, out var hasSeconds))
        {
            if (hasSeconds)
            {
                throw engine.Error("RangeError", "Intl.DateTimeFormat: a time zone offset cannot have seconds: " + text);
            }

            var identifier = (minutes < 0 ? "-" : "+") +
                (System.Math.Abs(minutes) / 60).ToString("00", System.Globalization.CultureInfo.InvariantCulture) + ":" +
                (System.Math.Abs(minutes) % 60).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
            return (identifier, minutes, minutes == 0 ? JsZoneKind.Gmt : JsZoneKind.Offset, null);
        }

        if (engine.Intl!.TimeZones.Find(text) is not { } record)
        {
            throw engine.Error("RangeError", "Intl.DateTimeFormat: " + text + " is not an available time zone");
        }

        if (record.Primary == "UTC")
        {
            return ("UTC", 0, JsZoneKind.Utc, null);
        }

        // IANA'S ETC/GMT ZONES WITH AN HOUR ARE FIXED OFFSETS by definition, with POSIX's inverted sign.
        foreach (var zone in EtcGmtZones())
        {
            if (string.Equals(zone.Name, record.Primary, System.StringComparison.Ordinal))
            {
                return (zone.Name, zone.Offset, JsZoneKind.Offset, null);
            }
        }

        return (record.Primary, 0, JsZoneKind.Named, engine.Intl.TimeZones.Zone(record));
    }

    /// <summary>IANA's <c>Etc/GMT+1</c> to <c>Etc/GMT+12</c> and <c>Etc/GMT-1</c> to <c>Etc/GMT-14</c>, with each one's offset in minutes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E7BE2C
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.IEnumerable<(string Name, int Offset)> EtcGmtZones()
    {
        for (var hours = 1; hours <= 14; hours++)
        {
            var text = hours.ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (hours <= 12)
            {
                yield return ("Etc/GMT+" + text, -hours * 60);
            }

            yield return ("Etc/GMT-" + text, hours * 60);
        }
    }

    /// <summary>ECMA-262's UTCOffset grammar: a sign, two-digit hours, and optionally minutes and seconds, all extended or all basic.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5BAFE6
    // Broiler-Human:        PENDING
    private static bool ParseOffset(string text, out int minutes, out bool hasSeconds)
    {
        minutes = 0;
        hasSeconds = false;

        if (text.Length < 3 || text[0] is not ('+' or '-'))
        {
            return false;
        }

        static bool TwoDigits(string text, int at, int maximum, out int value)
        {
            value = 0;

            if (at + 2 > text.Length || !char.IsAsciiDigit(text[at]) || !char.IsAsciiDigit(text[at + 1]))
            {
                return false;
            }

            value = ((text[at] - '0') * 10) + (text[at + 1] - '0');
            return value <= maximum;
        }

        if (!TwoDigits(text, 1, 23, out var hours))
        {
            return false;
        }

        var at = 3;
        var minute = 0;

        if (at < text.Length)
        {
            var extended = text[at] == ':';

            if (extended)
            {
                at++;
            }

            if (!TwoDigits(text, at, 59, out minute))
            {
                return false;
            }

            at += 2;

            if (at < text.Length)
            {
                if ((text[at] == ':') != extended)
                {
                    return false;
                }

                if (extended)
                {
                    at++;
                }

                if (!TwoDigits(text, at, 59, out _))
                {
                    return false;
                }

                at += 2;
                hasSeconds = true;

                if (at < text.Length)
                {
                    if (text[at] is not ('.' or ',') || at + 1 == text.Length || text.Length - at - 1 > 9)
                    {
                        return false;
                    }

                    for (var digit = at + 1; digit < text.Length; digit++)
                    {
                        if (!char.IsAsciiDigit(text[digit]))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        minutes = ((hours * 60) + minute) * (text[0] == '-' ? -1 : 1);
        return true;
    }
}

/// <summary>An <c>Intl.DateTimeFormat</c>: its resolved options and its formatter.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A917A0
// Broiler-Human:        PENDING
internal sealed class JsDateTimeFormatObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ECDA96
    // Broiler-Human:        PENDING
    internal JsDateTimeFormatObject(JsObject prototype, JsDateTimeFormatter formatter)
        : base(prototype)
    {
        Formatter = formatter;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C7DBBD
    // Broiler-Human:        PENDING
    internal JsDateTimeFormatter Formatter { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EDBB2D
    // Broiler-Human:        PENDING
    internal string Locale { get; init; } = string.Empty;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=91A976
    // Broiler-Human:        PENDING
    internal string Calendar { get; init; } = "gregory";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=57F126
    // Broiler-Human:        PENDING
    internal string NumberingSystem { get; init; } = "latn";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CC5C11
    // Broiler-Human:        PENDING
    internal string? HourCycle { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C216CD
    // Broiler-Human:        PENDING
    internal string? DateStyle { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=99CA7B
    // Broiler-Human:        PENDING
    internal string? TimeStyle { get; init; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0782CD
    // Broiler-Human:        PENDING
    internal JsNativeFunction? BoundFormat { get; set; }
}
