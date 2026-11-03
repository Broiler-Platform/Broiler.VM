// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           11
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Broiler.ECMAScript.Intl;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;
using Broiler.VM.Profile.JavaScript.Format;

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
/// <para>
/// <b>The locale core reads the shared Intl library's decoder since SJL-9.4</b> (JSD-0082, decisions
/// D8 and D24): <see cref="LocaleData"/> is <c>Broiler.ECMAScript.Intl</c>'s <c>TableLocaleData</c>
/// over the same tables, handed over through <see cref="JsIntlDataSet"/>, so that this family's
/// <see cref="IJsIntlData"/> and <see cref="JsIntlTable"/> stay its public contract. The decoders of
/// the likely subtags, the aliases, the extension types, the locale list, the week data and the
/// scripts' directions left this type with it; the number, date, list, relative-time, display-name
/// and zone-name decoders stay until their sub-areas move.
/// </para>
/// <para>
/// <b>The number formats read the same decoder since sub-areas (b) and (c)</b> (JSD-0082, decision
/// D24): <see cref="FormatData"/> orders <see cref="LocaleData"/> as the library's formatters read it,
/// and this type's own number decoder, with its <c>JsNumberData</c>, left it. The date, list,
/// relative-time, display-name and zone-name decoders stay until sub-area (d) moves; the list,
/// relative-time and display-name formats no longer read them, and the date format's zone names still
/// read the display names.
/// </para>
/// <para>
/// <i>Noted 2026-11-03, at SJL-9.4's review of sub-areas (b) and (c): the list and relative-time
/// decoders the paragraph above names as staying have gone after all, with <c>JsLocaleInfo</c>'s
/// <c>ListPatterns</c> and <c>RelativeTimes</c>, whose only readers were the ListFormat and
/// RelativeTimeFormat cores; the locale-information decoder now decodes the display names alone, which
/// the date format's zone names read, and the paragraph is kept as written.</i>
/// </para>
/// <para>
/// <i>Noted 2026-11-03, at SJL-9.4's sub-areas (b) and (c): the paragraph above, as stage (a) wrote
/// it, still lists "the number" decoder among those that stay; since then it has gone, and the
/// paragraph is kept as written.</i>
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BAE255
// Broiler-Human:        PENDING
internal sealed class JsIntlTables
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=503B2D
    // Broiler-Human:        PENDING
    private readonly IJsIntlData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBA9D2
    // Broiler-Human:        PENDING
    private readonly Lazy<string[]> availableLocales;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B4AA12
    // Broiler-Human:        PENDING
    private readonly Lazy<JsCollationData> collation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B2C9F
    // Broiler-Human:        PENDING
    private readonly Lazy<JsDateData> dates;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=07F030
    // Broiler-Human:        PENDING
    private readonly Lazy<JsLocaleInfo> localeInfo;

    /// <summary>The release the database states for data that does not carry <see cref="JsIntlTable.TimeZoneRelease"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4E10B0
    // Broiler-Human:        PENDING
    private const string UnstatedRelease = "unstated";

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=708F96
    // Broiler-Human:        PENDING
    private readonly Lazy<TimeZoneDatabase> timeZones;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=568AE0
    // Broiler-Human:        PENDING
    private readonly Lazy<TemporalCalendars> calendars;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F83F08
    // Broiler-Human:        PENDING
    private readonly Lazy<JsZoneNames> zoneNames;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2A4F3B
    // Broiler-Human:        PENDING
    internal JsIntlTables(IJsIntlData data)
    {
        this.data = data;
        // THE SHARED DECODER (JSD-0082, decision D8): built here, decoding nothing until a question needs
        // a table, each of its table groups once per process under its own thread-safe lazy, as this
        // type's are. Its number group is decoded there alone since sub-areas (b) and (c): every number,
        // currency, unit and plural question, the formatters' among them, reads LocaleData, through
        // FormatData's parent walk. Its date and locale-info groups are still decoded a second time,
        // apart from this type's own dates and localeInfo below: Intl.Locale's information methods,
        // supportedValuesOf and the list, relative-time and display-name formats read them through
        // LocaleData, while the date format still reads this type's. A script that uses both holds two
        // decoded copies of those groups, which costs memory and a first call's time and changes no
        // answer; it lasts until sub-area (d) deletes this type's decoders (decision D24).
        // (Revised 2026-11-03, at SJL-9.4's sub-areas (b) and (c). Stage (a) wrote: "Its number, date and
        // locale-info groups are decoded a second time, apart from this type's own numbers, dates and
        // localeInfo below: Intl.Locale's information methods and supportedValuesOf read them through
        // LocaleData, while the formatters still read this type's. A script that uses both holds two
        // decoded copies of those groups, which costs memory and a first call's time and changes no
        // answer; it lasts until sub-areas (b) to (e) delete this type's decoders (decision D24).")
        LocaleData = new TableLocaleData(new JsIntlDataSet(data));
        FormatData = new CompositeLocaleData(LocaleData);
        availableLocales = new(ReadAvailableLocales);
        collation = new(() => new JsCollationData(data.Table(JsIntlTable.CollationRoot), data.Table(JsIntlTable.CollationTailorings)));
        dates = new(ReadDates);
        localeInfo = new(ReadLocaleInfo);
        // DATA THAT DOES NOT CARRY THE RELEASE, as an embedder's written before SJL-8.5 does not, still
        // reads: the database states the release and nothing in the profile reads it (JSD-0077).
        timeZones = new(() => new TimeZoneDatabase(
            data.Table(JsIntlTable.TimeZoneRelease).IsEmpty ? UnstatedRelease : Encoding.ASCII.GetString(data.Table(JsIntlTable.TimeZoneRelease)),
            data.Table(JsIntlTable.TimeZones),
            data.Table(JsIntlTable.TimeZoneIds),
            data.Table(JsIntlTable.TimeZoneRegions)));
        // THE CALENDARS ARE THE SHARED LIBRARY'S since SJL-10.4 (JSD-0078): one TemporalCalendars over the
        // shared CalendarTables of the table the Intl data hands in, as the time zones are one database.
        calendars = new(() => new TemporalCalendars(new CalendarTables(data.Table(JsIntlTable.Calendars))));
        zoneNames = new(() => new JsZoneNames(Lines(JsIntlTable.MetaZones), TimeZones));
    }

    /// <summary>The CLDR release the tables come from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B6F048
    // Broiler-Human:        PENDING
    internal string CldrVersion => data.CldrVersion;

    /// <summary>
    /// The shared Intl library's answers over this composition's tables (JSD-0082, decision D8): the
    /// likely subtags, aliases and extension types canonicalisation reads, the locale list, the hour
    /// cycles, the week data and the scripts' directions.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=35CA41
    // Broiler-Human:        PENDING
    internal ILocaleData LocaleData { get; }

    /// <summary>
    /// <see cref="LocaleData"/> as the shared Intl library's formatters read it (JSD-0082, decision D8):
    /// the library's <c>CompositeLocaleData</c> over it, with no secondary provider (decision D6), so
    /// that a question at a locale the tables do not hold, such as <c>en-US</c>, reads its parent's
    /// lines, <c>en</c>'s, which are the lines this family's formatters read at the data locale's
    /// language.
    /// </summary>
    /// <remarks>
    /// <i>Noted 2026-11-03, at SJL-9.4's review of sub-areas (b) and (c):</i> the number, list,
    /// relative-time and display-name formats read through this the data locale's whole chain (the
    /// locale, its truncations, then <c>und</c>, stopping at the first locale the data holds), where
    /// the realm's own decoders read the data locale's language alone and never read a region locale's
    /// lines. The shipped tables carry those groups for <c>de</c> and <c>en</c> alone, so every shipped
    /// answer is the one the language-only read gave; the two can differ only for data an embedder
    /// hands in through <see cref="IJsIntlData"/> that holds lines for a region locale, such as
    /// <c>en-GB</c>, which are read now and were ignored before. The plural rules still read by
    /// language and are unaffected.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=JSD-0082 D8; IP=Low; Security=Low; Resources=1; Fingerprint=92318E
    // Broiler-Human:        PENDING
    internal ILocaleData FormatData { get; }

    /// <summary>
    /// The locales every service offers (the default-locale and available-locale providers, JSD-0082
    /// decision D7): the data's list without <c>und</c>, in the data's order, <c>en-US</c>, the
    /// default, among them.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3263E8
    // Broiler-Human:        PENDING
    internal IReadOnlyList<string> AvailableLocales => availableLocales.Value;

    /// <summary>
    /// The IANA Time Zone Database's identifiers, regions and offsets: the shared library's decoder over
    /// the three tables and the release the data hands in (JSD-0076, JSD-0077), made the first time a zone
    /// is named and decoding each zone the first time its rules are read.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0B9661
    // Broiler-Human:        PENDING
    internal ITimeZoneDatabase TimeZones => timeZones.Value;

    /// <summary>
    /// The calendars, the shared library's <see cref="TemporalCalendars"/> over the Intl data's calendar
    /// table, built the first time Temporal or a format names a calendar.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CF2654
    // Broiler-Human:        PENDING
    internal TemporalCalendars Calendars => calendars.Value;

    /// <summary>CLDR's metazones and the algorithms that name a zone by them, built the first time a zone is named (JSD-0058).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0A74FA
    // Broiler-Human:        PENDING
    internal JsZoneNames ZoneNames => zoneNames.Value;

    /// <summary>The collation data.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B35C2E
    // Broiler-Human:        PENDING
    internal JsCollationData Collation => collation.Value;

    /// <summary>The calendar, hour cycle and day period data, decoded the first time a date is formatted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=813274
    // Broiler-Human:        PENDING
    internal JsDateData Dates => dates.Value;

    /// <summary>
    /// The display names the date format's zone names read, decoded the first time a zone is named.
    /// (Revised 2026-11-03, at SJL-9.4's review of sub-areas (b) and (c), when the list patterns and
    /// relative times left it. It read: "The list patterns, relative times and display names, decoded
    /// the first time a locale is asked for them.")
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F90F89
    // Broiler-Human:        PENDING
    internal JsLocaleInfo LocaleInfo => localeInfo.Value;

    /// <summary>
    /// The text segmentation <c>Intl.Segmenter</c> reads, which the composition's data offers beside its
    /// tables, or null when it offers none (JSD-0074; milestone SJL-7.4).
    /// </summary>
    /// <remarks>
    /// Until SJL-7.4 this was the break table, decoded here and segmented by the profile's own
    /// <c>JsSegmentation</c>. Segmentation is now the Unicode area's owner's optional package, which the
    /// Intl data assembly alone references, so the profile reaches it through the data, as it reaches
    /// the tables (<see cref="IJsTextSegmentation"/>).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=31E0CD
    // Broiler-Human:        PENDING
    internal IJsTextSegmentation? Segmentation => data as IJsTextSegmentation;

    /// <summary>A table value with its <c>\uXXXX</c> escapes resolved.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E0016
    // Broiler-Human:        PENDING
    internal static string Unescape(string value)
    {
        if (value.IndexOf('\\') < 0)
        {
            return value;
        }

        var text = new StringBuilder(value.Length);

        for (var at = 0; at < value.Length; at++)
        {
            if (value[at] == '\\' && at + 5 < value.Length && value[at + 1] == 'u')
            {
                text.Append((char)int.Parse(value.AsSpan(at + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                at += 5;
                continue;
            }

            text.Append(value[at]);
        }

        return text.ToString();
    }

    /// <summary>Decodes the date tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D65CD
    // Broiler-Human:        PENDING
    /// <summary>A layer of date data over the data below it: its values set, and its <c>-</c> keys removed.</summary>
    private static Dictionary<string, string> Layered(Dictionary<string, string> below, Dictionary<string, string> layer)
    {
        var result = new Dictionary<string, string>(below, StringComparer.Ordinal);

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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E9F5B
    // Broiler-Human:        PENDING
    private JsDateData ReadDates()
    {
        var data = new JsDateData();

        foreach (var line in Lines(JsIntlTable.DateLocales))
        {
            var fields = line.Split('|');

            if (!data.Locales.TryGetValue(fields[0], out var locale))
            {
                locale = new(StringComparer.Ordinal);
                data.Locales[fields[0]] = locale;
            }

            locale[fields[1]] = Unescape(fields[2]);
        }

        // EACH LANGUAGE'S ISO 8601 CALENDAR (JSD-0055) is its Gregorian names under root's ISO 8601
        // patterns, as CLDR's aliases make it: `<language>@iso8601`. The date-time glue stays the
        // language's Gregorian one, as ICU resolves it, where root's would be "{1} {0}".
        if (data.Locales.TryGetValue(JsDateData.Iso8601, out var iso8601))
        {
            foreach (var language in new List<string>(data.Locales.Keys))
            {
                if (language.Contains('@', StringComparison.Ordinal))
                {
                    continue;
                }

                var calendar = new Dictionary<string, string>(StringComparer.Ordinal);

                foreach (var (key, value) in data.Locales[language])
                {
                    if (!JsDateData.IsPatternKey(key) || key.StartsWith("dateTime.", StringComparison.Ordinal) || key.StartsWith("atTime.", StringComparison.Ordinal))
                    {
                        calendar[key] = value;
                    }
                }

                foreach (var (key, value) in iso8601)
                {
                    if (!key.StartsWith("dateTime.", StringComparison.Ordinal) && !key.StartsWith("atTime.", StringComparison.Ordinal))
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
        foreach (var language in new List<string>(data.Locales.Keys))
        {
            if (language.Contains('@', StringComparison.Ordinal) || !data.Locales.TryGetValue(language + "@generic", out var genericDiff))
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

        foreach (var line in Lines(JsIntlTable.TimeData))
        {
            var fields = line.Split('|');
            data.TimeData[fields[0]] = (fields[1], fields[2]);
        }

        static int Minutes(string time) => time.Length == 0 ? -1 : (int.Parse(time[..2], CultureInfo.InvariantCulture) * 60) + int.Parse(time[3..], CultureInfo.InvariantCulture);

        foreach (var line in Lines(JsIntlTable.DayPeriods))
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

    /// <summary>
    /// Decodes the display-name table. (Revised 2026-11-03, at SJL-9.4's review of sub-areas (b) and
    /// (c): it read "Decodes the list, relative-time and display-name tables.", and its loops over
    /// <c>JsIntlTable.ListPatterns</c> and <c>JsIntlTable.RelativeTimes</c>, whose only readers were the
    /// ListFormat and RelativeTimeFormat cores that sub-area (c) deleted, are gone; the library's
    /// decoder reads those tables.)
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=70C07A
    // Broiler-Human:        PENDING
    private JsLocaleInfo ReadLocaleInfo()
    {
        var info = new JsLocaleInfo();

        foreach (var line in Lines(JsIntlTable.DisplayNames))
        {
            var fields = line.Split('|');
            info.DisplayNames[fields[0] + "|" + fields[1] + "|" + fields[2]] = Unescape(fields[3]);
        }

        return info;
    }

    /// <summary>The lines of a text table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=7154A5
    // Broiler-Human:        PENDING
    private List<string> Lines(JsIntlTable table)
    {
        var text = Encoding.ASCII.GetString(data.Table(table));
        var lines = new List<string>();

        foreach (var line in text.Split('\n'))
        {
            if (line.Length != 0)
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    /// <summary>The data's locales without <c>und</c>, which no service offers, in the data's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=15B295
    // Broiler-Human:        PENDING
    private string[] ReadAvailableLocales()
    {
        var available = new List<string>();

        foreach (var locale in LocaleData.Locales)
        {
            if (locale != "und")
            {
                available.Add(locale);
            }
        }

        return [.. available];
    }
}
