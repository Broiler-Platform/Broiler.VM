// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           9
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
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
/// <para>
/// <i>Noted 2026-11-03, at SJL-9.4's review of sub-areas (d) and (e): the two paragraphs above that
/// say the date, display-name and zone-name decoders "stay until their sub-areas move" and "stay until
/// sub-area (d) moves" are kept as written; sub-area (d) removed those decoders, as the next paragraph
/// says.</i>
/// </para>
/// <para>
/// <b>The date formats read the same decoder since sub-area (d)</b> (JSD-0082, decision D24): the
/// date, time-data and day-period decoder with its <c>JsDateData</c>, the display-name decoder with its
/// <c>JsLocaleInfo</c>, and the zone-name decoder with its <c>JsZoneNames</c> left this type, and
/// <see cref="DateTimeData"/> hands the shared library's date-time formats <see cref="FormatData"/>, the
/// time zones and the calendars. Every table group is now decoded once, by the library's decoder,
/// but the collation tables, which <c>JsCollationData</c> decodes until SJL-17.4, and the time zones and
/// calendars, which the shared time library's own decoders read.
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

    // Broiler-AI:           Origin=AI; Spec=JSD-0082 D8; IP=Low; Security=Low; Resources=1; Fingerprint=E1D5B0
    // Broiler-Human:        PENDING
    private readonly Lazy<DateTimeData> dateTimes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=83AC78
    // Broiler-Human:        PENDING
    internal JsIntlTables(IJsIntlData data)
    {
        this.data = data;
        // THE SHARED DECODER (JSD-0082, decision D8): built here, decoding nothing until a question needs
        // a table, each of its table groups once per process under its own thread-safe lazy, as this
        // type's are. Since sub-area (d) it is the only decoder of the number, date, time-data, day-period,
        // display-name, list, relative-time and metazone tables: every question the Intl services ask of
        // them reads LocaleData, the formatters' through FormatData's parent walk, and no group is decoded
        // twice. (Revised 2026-11-03, at SJL-9.4's sub-area (d), when this type's dates, localeInfo and
        // zoneNames left it. Sub-areas (b) and (c) wrote: "Its number group is decoded there alone since
        // sub-areas (b) and (c): every number, currency, unit and plural question, the formatters' among
        // them, reads LocaleData, through FormatData's parent walk. Its date and locale-info groups are
        // still decoded a second time, apart from this type's own dates and localeInfo below: Intl.Locale's
        // information methods, supportedValuesOf and the list, relative-time and display-name formats read
        // them through LocaleData, while the date format still reads this type's. A script that uses both
        // holds two decoded copies of those groups, which costs memory and a first call's time and changes
        // no answer; it lasts until sub-area (d) deletes this type's decoders (decision D24).")
        LocaleData = new TableLocaleData(new JsIntlDataSet(data));
        FormatData = new CompositeLocaleData(LocaleData);
        availableLocales = new(ReadAvailableLocales);
        collation = new(() => new JsCollationData(data.Table(JsIntlTable.CollationRoot), data.Table(JsIntlTable.CollationTailorings)));
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
        // THE DATE-TIME FORMATS' DATA (JSD-0082, decision D8): the library's DateTimeData over FormatData,
        // the time zones and the calendars, made the first time a date-time format is created, so that its
        // pattern generators and its zone names are made once and shared, as this type's were.
        dateTimes = new(() => new DateTimeData(FormatData, TimeZones, Calendars));
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
    /// <para>
    /// <i>Noted 2026-11-03, at SJL-9.4's review of sub-areas (d) and (e):</i> the note above names "the
    /// number, list, relative-time and display-name formats"; since sub-areas (d) and (e) the date-time
    /// and duration formats read through this the same chain too: the date-time format's decimal symbol
    /// and its zone naming's region names, and the duration format's <c>symbols.timeSeparator</c> and
    /// <c>duration.hms</c> (row U13), each at the full data locale where the realm's own code read the
    /// data locale's language. Two more differences come with the shared library's date-time and
    /// duration formats. The 12- and 24-hour cycles, [[hourCycle12]] and [[hourCycle24]], are the
    /// region's preferred cycle where it is of the kind asked for and otherwise the first of that kind
    /// CLDR's time data allows for the region (row T10), where the realm's code answered <c>h12</c> and
    /// <c>h23</c>; and the <c>nu</c> values put the locale's <c>defaultNumberingSystem</c> first (rows
    /// T37 and U16, decisions D13 and D21), where the realm's code put <c>latn</c> first. The shipped
    /// time data (<c>DE</c> <c>H hB</c>, <c>US</c> <c>h hb H hB</c>, <c>001</c> <c>H h</c>) and the
    /// shipped locales' <c>latn</c> defaults give every shipped answer unchanged; data an embedder hands
    /// in through <see cref="IJsIntlData"/> can differ, for example <c>ja</c>'s <c>hour12: true</c>
    /// resolving <c>h11</c> (Japan allows <c>H K h</c>) where the realm's code resolved <c>h12</c>, or a
    /// locale whose default system is <c>arab</c> resolving <c>nu</c> to <c>arab</c> where it resolved
    /// <c>latn</c>. SJL-9.4's cut-over record lists these under rows T10, T37 and U16.
    /// </para>
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

    /// <summary>
    /// What the shared Intl library's date-time formats read besides their options (JSD-0082, decision
    /// D8): <see cref="FormatData"/>, the time zones and the calendars, with the pattern generators and
    /// CLDR's metazones the library makes from them, built the first time a date-time format is created.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=JSD-0082 D8; IP=Low; Security=Low; Resources=1; Fingerprint=F52075
    // Broiler-Human:        PENDING
    internal DateTimeData DateTimeData => dateTimes.Value;

    /// <summary>The collation data.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B35C2E
    // Broiler-Human:        PENDING
    internal JsCollationData Collation => collation.Value;

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
