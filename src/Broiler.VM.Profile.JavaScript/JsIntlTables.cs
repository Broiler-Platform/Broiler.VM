// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           15
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The internationalization tables a composition handed over, decoded once and shared by every
/// runtime its descriptor makes (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Each table is decoded the first time a guest needs it</b>, under a thread-safe lazy: a
/// descriptor's runtimes may run on several threads, and the tables never change, so the first
/// decode is the only one and every later read is a dictionary lookup.
/// </para>
/// <para>
/// <b>Nothing here is guest-visible state.</b> The decoded tables are the composition's data in a
/// shape the profile can search; no realm, engine or guest value is held, so sharing them across
/// runtimes shares nothing a guest could write.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BAE255
// Broiler-Human:        PENDING
internal sealed class JsIntlTables
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0582E3
    // Broiler-Human:        PENDING
    private readonly Format.IJsIntlData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=678323
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, string>> likely;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E9ED15
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>> aliases;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F8026D
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, string>> extensions;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=38DA77
    // Broiler-Human:        PENDING
    private readonly System.Lazy<string[]> locales;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1EBC74
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsCollationData> collation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B4D744
    // Broiler-Human:        PENDING
    private readonly System.Lazy<(int First, int Last)[]> softDotted;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=228073
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsNumberData> numbers;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=10659A
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsDateData> dates;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E52983
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsLocaleInfo> localeInfo;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=215679
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsBreakData> breaks;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9311F4
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsTimeZones> timeZones;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3AA84F
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsCalendars> calendars;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F30CF4
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsZoneNames> zoneNames;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EAA1ED
    // Broiler-Human:        PENDING
    internal JsIntlTables(Format.IJsIntlData data)
    {
        this.data = data;
        likely = new(() => Pairs(Format.JsIntlTable.LikelySubtags));
        aliases = new(ReadAliases);
        extensions = new(ReadExtensions);
        locales = new(() => [.. Lines(Format.JsIntlTable.Locales)]);
        collation = new(() => new JsCollationData(
            data.Table(Format.JsIntlTable.CollationRoot), data.Table(Format.JsIntlTable.CollationTailorings)));
        softDotted = new(ReadRanges);
        numbers = new(ReadNumbers);
        dates = new(ReadDates);
        localeInfo = new(ReadLocaleInfo);
        breaks = new(() => new JsBreakData(Lines(Format.JsIntlTable.SegmentBreakValues), data.Table(Format.JsIntlTable.SegmentBreaks)));
        timeZones = new(() => new JsTimeZones(
            Lines(Format.JsIntlTable.TimeZoneIds), Lines(Format.JsIntlTable.TimeZoneRegions), data.Table(Format.JsIntlTable.TimeZones)));
        calendars = new(() => new JsCalendars(data.Table(Format.JsIntlTable.Calendars)));
        zoneNames = new(() => new JsZoneNames(Lines(Format.JsIntlTable.MetaZones), TimeZones));
    }

    /// <summary>The CLDR release the tables come from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B6F048
    // Broiler-Human:        PENDING
    internal string CldrVersion => data.CldrVersion;

    /// <summary>The locales the data supports, in canonical form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F9D74C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> Locales => locales.Value;

    /// <summary>The IANA Time Zone Database's identifiers and offsets, decoded the first time a zone is named.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5A12A5
    // Broiler-Human:        PENDING
    internal JsTimeZones TimeZones => timeZones.Value;

    /// <summary>The calendars other than ISO 8601, built the first time Temporal names one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F73E47
    // Broiler-Human:        PENDING
    internal JsCalendars Calendars => calendars.Value;

    /// <summary>CLDR's metazones and the algorithms that name a zone by them, built the first time a zone is named (JSD-0058).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0A74FA
    // Broiler-Human:        PENDING
    internal JsZoneNames ZoneNames => zoneNames.Value;

    /// <summary>The collation data.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B35C2E
    // Broiler-Human:        PENDING
    internal JsCollationData Collation => collation.Value;

    /// <summary>The likely subtags of a source in CLDR's form - <c>en</c>, <c>und-Latn</c>, <c>und-US</c> - or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E6580E
    // Broiler-Human:        PENDING
    internal string? Likely(string source) => likely.Value.TryGetValue(source, out var target) ? target : null;

    /// <summary>The alias of <paramref name="from"/> of one kind - language, script, region, variant, subdivision - or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B36EB0
    // Broiler-Human:        PENDING
    internal string? Alias(string kind, string from) =>
        aliases.Value.TryGetValue(kind, out var table) && table.TryGetValue(from, out var replacement) ? replacement : null;

    /// <summary>Every language alias, for the rule matcher that reads them all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=00F4A3
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyDictionary<string, string> LanguageAliases => aliases.Value["language"];

    /// <summary>Whether <paramref name="key"/> is a key of extension <paramref name="singleton"/> the BCP 47 data defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8B3C4F
    // Broiler-Human:        PENDING
    internal bool IsKey(char singleton, string key) => extensions.Value.ContainsKey(singleton + "|" + key + "|");

    /// <summary>
    /// Whether <paramref name="type"/> is a type of <paramref name="key"/>, and its preferred form:
    /// itself, or the type a deprecated one was replaced by.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A7A19D
    // Broiler-Human:        PENDING
    internal bool TryType(char singleton, string key, string type, out string preferred)
    {
        if (extensions.Value.TryGetValue(singleton + "|" + key + "|" + type, out var stated))
        {
            preferred = stated.Length == 0 ? type : stated;
            return true;
        }

        preferred = type;
        return false;
    }

    /// <summary>Whether a code point is Soft_Dotted: an <c>i</c> or <c>j</c> whose dot an accent above replaces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65A410
    // Broiler-Human:        PENDING
    internal bool IsSoftDotted(int codePoint)
    {
        var ranges = softDotted.Value;
        var low = 0;
        var high = ranges.Length - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < ranges[middle].First)
            {
                high = middle - 1;
            }
            else if (codePoint > ranges[middle].Last)
            {
                low = middle + 1;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The number, currency, unit and plural data, decoded the first time a number is formatted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C30BD0
    // Broiler-Human:        PENDING
    internal JsNumberData Numbers => numbers.Value;

    /// <summary>The calendar, hour cycle and day period data, decoded the first time a date is formatted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=813274
    // Broiler-Human:        PENDING
    internal JsDateData Dates => dates.Value;

    /// <summary>The week data and the scripts' line directions, decoded the first time a locale is asked for them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F90F89
    // Broiler-Human:        PENDING
    internal JsLocaleInfo LocaleInfo => localeInfo.Value;

    /// <summary>The break properties <c>Intl.Segmenter</c> reads, decoded the first time a string is segmented.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=82FD5A
    // Broiler-Human:        PENDING
    internal JsBreakData Breaks => breaks.Value;

    /// <summary>A table value with its <c>\uXXXX</c> escapes resolved.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=771818
    // Broiler-Human:        PENDING
    internal static string Unescape(string value)
    {
        if (value.IndexOf('\\') < 0)
        {
            return value;
        }

        var text = new System.Text.StringBuilder(value.Length);

        for (var at = 0; at < value.Length; at++)
        {
            if (value[at] == '\\' && at + 5 < value.Length && value[at + 1] == 'u')
            {
                text.Append((char)int.Parse(value.Substring(at + 2, 4), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture));
                at += 5;
                continue;
            }

            text.Append(value[at]);
        }

        return text.ToString();
    }

    /// <summary>Decodes the number tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=C4085C
    // Broiler-Human:        PENDING
    private JsNumberData ReadNumbers()
    {
        var data = new JsNumberData();

        foreach (var line in Lines(Format.JsIntlTable.NumberLocales))
        {
            var fields = line.Split('|');

            if (!data.Locales.TryGetValue(fields[0], out var locale))
            {
                locale = new(System.StringComparer.Ordinal);
                data.Locales[fields[0]] = locale;
            }

            locale[fields[1]] = Unescape(fields[2]);
        }

        foreach (var line in Lines(Format.JsIntlTable.Currencies))
        {
            var fields = line.Split('|');
            data.Currencies[fields[0] + "|" + fields[1]] = new JsCurrencyNames(
                Unescape(fields[2]), Unescape(fields[3]), Unescape(fields[4]), Unescape(fields[5]), Unescape(fields[6]), fields[7], fields[8]);
        }

        foreach (var line in Lines(Format.JsIntlTable.CurrencyDigits))
        {
            var bar = line.IndexOf('|');
            data.CurrencyDigits[line[..bar]] = int.Parse(line[(bar + 1)..], System.Globalization.CultureInfo.InvariantCulture);
        }

        foreach (var line in Lines(Format.JsIntlTable.NumberingSystems))
        {
            var bar = line.IndexOf('|');
            var digits = Unescape(line[(bar + 1)..]);
            var list = new string[10];
            var index = 0;

            for (var at = 0; at < digits.Length && index < 10; index++)
            {
                var width = char.IsHighSurrogate(digits[at]) && at + 1 < digits.Length ? 2 : 1;
                list[index] = digits.Substring(at, width);
                at += width;
            }

            data.NumberingSystems[line[..bar]] = list;
        }

        foreach (var (table, plurals) in new[] { (Format.JsIntlTable.Plurals, data.Plurals), (Format.JsIntlTable.Ordinals, data.Ordinals) })
        {
            foreach (var line in Lines(table))
            {
                var fields = line.Split('|');

                if (!plurals.TryGetValue(fields[0], out var rules))
                {
                    rules = [];
                    plurals[fields[0]] = rules;
                }

                rules.Add((fields[1], JsPluralRule.Parse(Unescape(fields[2]))));
            }
        }

        foreach (var line in Lines(Format.JsIntlTable.PluralRanges))
        {
            var fields = line.Split('|');
            data.PluralRanges[fields[0] + "|" + fields[1] + "|" + fields[2]] = fields[3];
        }

        foreach (var line in Lines(Format.JsIntlTable.Units))
        {
            var fields = line.Split('|');
            data.Units[fields[0] + "|" + fields[1] + "|" + fields[2] + "|" + fields[3]] = Unescape(fields[4]);
        }

        return data;
    }

    /// <summary>Decodes the date tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD2261
    // Broiler-Human:        PENDING
    /// <summary>A layer of date data over the data below it: its values set, and its <c>-</c> keys removed.</summary>
    private static System.Collections.Generic.Dictionary<string, string> Layered(
        System.Collections.Generic.Dictionary<string, string> below,
        System.Collections.Generic.Dictionary<string, string> layer)
    {
        var result = new System.Collections.Generic.Dictionary<string, string>(below, System.StringComparer.Ordinal);

        foreach (var (key, value) in layer)
        {
            if (key.StartsWith('-'))
            {
                result.Remove(key[1..]);
            }
            else
            {
                result[key] = value;
            }
        }

        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E8D3E
    // Broiler-Human:        PENDING
    private JsDateData ReadDates()
    {
        var data = new JsDateData();

        foreach (var line in Lines(Format.JsIntlTable.DateLocales))
        {
            var fields = line.Split('|');

            if (!data.Locales.TryGetValue(fields[0], out var locale))
            {
                locale = new(System.StringComparer.Ordinal);
                data.Locales[fields[0]] = locale;
            }

            locale[fields[1]] = Unescape(fields[2]);
        }

        // EACH LANGUAGE'S ISO 8601 CALENDAR (JSD-0055) is its Gregorian names under root's ISO 8601
        // patterns, as CLDR's aliases make it: `<language>@iso8601`. The date-time glue stays the
        // language's Gregorian one, as ICU resolves it, where root's would be "{1} {0}".
        if (data.Locales.TryGetValue(JsDateData.Iso8601, out var iso8601))
        {
            foreach (var language in new System.Collections.Generic.List<string>(data.Locales.Keys))
            {
                if (language.Contains('@', System.StringComparison.Ordinal))
                {
                    continue;
                }

                var calendar = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

                foreach (var (key, value) in data.Locales[language])
                {
                    if (!JsDateData.IsPatternKey(key) || key.StartsWith("dateTime.", System.StringComparison.Ordinal) || key.StartsWith("atTime.", System.StringComparison.Ordinal))
                    {
                        calendar[key] = value;
                    }
                }

                foreach (var (key, value) in iso8601)
                {
                    if (!key.StartsWith("dateTime.", System.StringComparison.Ordinal) && !key.StartsWith("atTime.", System.StringComparison.Ordinal))
                    {
                        calendar[key] = value;
                    }
                }

                data.Locales[language + JsDateData.Iso8601] = calendar;
            }
        }

        // EACH LANGUAGE'S OTHER CALENDARS (JSD-0057) are layers: CLDR's generic patterns over the
        // language's Gregorian data, and each calendar over the generic layer or its parent's. A layer's
        // key beginning with `-` removes the key below it.
        foreach (var language in new System.Collections.Generic.List<string>(data.Locales.Keys))
        {
            if (language.Contains('@', System.StringComparison.Ordinal) || !data.Locales.TryGetValue(language + "@generic", out var genericDiff))
            {
                continue;
            }

            var generic = Layered(data.Locales[language], genericDiff);
            data.Locales.Remove(language + "@generic");

            // A LAYER THE TABLE HOLDS NO LINE OF is its parent's data unchanged.
            foreach (var (calendar, parent) in JsDateData.CalendarLayers)
            {
                var diff = data.Locales.TryGetValue(language + "@" + calendar, out var lines) ? lines : [];
                data.Locales[language + "@" + calendar] = Layered(parent is null ? generic : data.Locales[language + "@" + parent], diff);
            }
        }

        foreach (var line in Lines(Format.JsIntlTable.TimeData))
        {
            var fields = line.Split('|');
            data.TimeData[fields[0]] = (fields[1], fields[2]);
        }

        static int Minutes(string time) =>
            time.Length == 0 ? -1 : (int.Parse(time[..2], System.Globalization.CultureInfo.InvariantCulture) * 60) + int.Parse(time[3..], System.Globalization.CultureInfo.InvariantCulture);

        foreach (var line in Lines(Format.JsIntlTable.DayPeriods))
        {
            var fields = line.Split('|');

            if (!data.DayPeriods.TryGetValue(fields[0], out var rules))
            {
                rules = [];
                data.DayPeriods[fields[0]] = rules;
            }

            rules.Add(fields[3].Length == 0
                ? new JsDayPeriodRule(fields[1], Minutes(fields[2]), -1, At: true)
                : new JsDayPeriodRule(fields[1], Minutes(fields[2]), Minutes(fields[3]), At: false));
        }

        return data;
    }

    /// <summary>Decodes the week and script tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=330A35
    // Broiler-Human:        PENDING
    private JsLocaleInfo ReadLocaleInfo()
    {
        var info = new JsLocaleInfo();

        foreach (var line in Lines(Format.JsIntlTable.WeekData))
        {
            var fields = line.Split('|');
            info.Weeks[fields[0]] = (fields[1], fields[2], fields[3], fields[4]);
        }

        foreach (var line in Lines(Format.JsIntlTable.Scripts))
        {
            var bar = line.IndexOf('|');
            info.RightToLeft[line[..bar]] = line[(bar + 1)..] == "YES";
        }

        foreach (var line in Lines(Format.JsIntlTable.ListPatterns))
        {
            var fields = line.Split('|');
            info.ListPatterns[fields[0] + "|" + fields[1]] =
                (Unescape(fields[2]), Unescape(fields[3]), Unescape(fields[4]), Unescape(fields[5]));
        }

        foreach (var line in Lines(Format.JsIntlTable.RelativeTimes))
        {
            var fields = line.Split('|');
            info.RelativeTimes[fields[0] + "|" + fields[1] + "|" + fields[2]] = Unescape(fields[3]);
        }

        foreach (var line in Lines(Format.JsIntlTable.DisplayNames))
        {
            var fields = line.Split('|');
            info.DisplayNames[fields[0] + "|" + fields[1] + "|" + fields[2]] = Unescape(fields[3]);
        }

        return info;
    }

    /// <summary>The lines of a text table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=D7F15D
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> Lines(Format.JsIntlTable table)
    {
        var text = System.Text.Encoding.ASCII.GetString(data.Table(table));
        var lines = new System.Collections.Generic.List<string>();

        foreach (var line in text.Split('\n'))
        {
            if (line.Length != 0)
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    /// <summary>A two-field table as a dictionary.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=041F11
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, string> Pairs(Format.JsIntlTable table)
    {
        var pairs = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var line in Lines(table))
        {
            var bar = line.IndexOf('|');
            pairs[line[..bar]] = line[(bar + 1)..];
        }

        return pairs;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=C4C031
    // Broiler-Human:        PENDING
    private (int First, int Last)[] ReadRanges()
    {
        var lines = Lines(Format.JsIntlTable.SoftDotted);
        var ranges = new (int First, int Last)[lines.Count];

        for (var at = 0; at < lines.Count; at++)
        {
            var bar = lines[at].IndexOf('|');
            ranges[at] = (
                int.Parse(lines[at][..bar], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(lines[at][(bar + 1)..], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture));
        }

        return ranges;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=50C7CB
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> ReadAliases()
    {
        var kinds = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>(System.StringComparer.Ordinal);

        foreach (var kind in new[] { "language", "script", "region", "variant", "subdivision" })
        {
            kinds[kind] = new(System.StringComparer.Ordinal);
        }

        foreach (var line in Lines(Format.JsIntlTable.Aliases))
        {
            var fields = line.Split('|');
            kinds[fields[0]][fields[1]] = fields[2];
        }

        return kinds;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=F3E6EB
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, string> ReadExtensions()
    {
        var table = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var line in Lines(Format.JsIntlTable.Extensions))
        {
            var fields = line.Split('|');
            table[fields[0] + "|" + fields[1] + "|" + fields[2]] = fields[3];
        }

        return table;
    }
}
