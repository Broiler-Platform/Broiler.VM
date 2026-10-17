// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           3
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Unicode.Algorithms;
using Broiler.Unicode.Segmentation;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Intl;

/// <summary>
/// The internationalization data generated from CLDR 48.2.0, for a composition to hand to the
/// JavaScript profile when it admits <see cref="JsSurfaces.Intl"/> (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>It holds nothing but the generated tables</b>, which live in this assembly's data section and
/// are never copied: each <see cref="Table"/> answers a span over them. The tables are what rule N28
/// holds byte for byte to the generator and the pinned archive.
/// </para>
/// <para>
/// <b>One instance serves every runtime a process creates</b>, because the data never changes.
/// </para>
/// <para>
/// <b>It segments text too, since SJL-7.4</b> (JSD-0074): <see cref="IJsTextSegmentation"/>, over the
/// Unicode area's owner's optional segmentation package and the break table this assembly embeds,
/// <c>JsUnicodeTables.g.cs</c>, which the owner's generator writes from this repository's Unicode pin and
/// rule N28 holds byte for byte. The package is referenced here and nowhere else in the family, so only a
/// root that carries this assembly carries it (rule I3).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=52FE6A
// Broiler-Human:        PENDING
public sealed class JsCldrData : IJsIntlData, IJsTextSegmentation
{
    /// <summary>The owner's segmenter over the break table this assembly embeds, decoded once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=87093F
    // Broiler-Human:        PENDING
    private static readonly UnicodeSegmenter Segmenter = new(new BreakTables());

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1DD3CB
    // Broiler-Human:        PENDING
    private JsCldrData()
    {
    }

    /// <summary>The one instance.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F80706
    // Broiler-Human:        PENDING
    public static JsCldrData Instance { get; } = new();

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5FEC04
    // Broiler-Human:        PENDING
    public string CldrVersion => JsCldrTables.Version;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6648FA
    // Broiler-Human:        PENDING
    public System.ReadOnlySpan<byte> Table(JsIntlTable table) => table switch
    {
        JsIntlTable.LikelySubtags => JsCldrTables.LikelySubtags,
        JsIntlTable.Aliases => JsCldrTables.Aliases,
        JsIntlTable.Extensions => JsCldrTables.Extensions,
        JsIntlTable.Locales => JsCldrTables.Locales,
        JsIntlTable.CollationRoot => JsCldrTables.CollationRoot,
        JsIntlTable.CollationTailorings => JsCldrTables.CollationTailorings,
        JsIntlTable.NumberLocales => JsCldrTables.NumberLocales,
        JsIntlTable.Currencies => JsCldrTables.Currencies,
        JsIntlTable.CurrencyDigits => JsCldrTables.CurrencyDigits,
        JsIntlTable.NumberingSystems => JsCldrTables.NumberingSystems,
        JsIntlTable.Plurals => JsCldrTables.Plurals,
        JsIntlTable.PluralRanges => JsCldrTables.PluralRanges,
        JsIntlTable.Units => JsCldrTables.Units,
        JsIntlTable.DateLocales => JsCldrTables.DateLocales,
        JsIntlTable.TimeData => JsCldrTables.TimeData,
        JsIntlTable.DayPeriods => JsCldrTables.DayPeriods,
        JsIntlTable.WeekData => JsCldrTables.WeekData,
        JsIntlTable.Scripts => JsCldrTables.Scripts,
        JsIntlTable.Ordinals => JsCldrTables.Ordinals,
        JsIntlTable.ListPatterns => JsCldrTables.ListPatterns,
        JsIntlTable.RelativeTimes => JsCldrTables.RelativeTimes,
        JsIntlTable.DisplayNames => JsCldrTables.DisplayNames,
        JsIntlTable.TimeZones => JsTzdbTables.TimeZones,
        JsIntlTable.TimeZoneIds => JsTzdbTables.TimeZoneIds,
        JsIntlTable.TimeZoneRegions => JsTzdbTables.TimeZoneRegions,
        JsIntlTable.Calendars => JsCalendarTables.Calendars,
        JsIntlTable.MetaZones => JsCldrTables.MetaZones,
        _ => default,
    };

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Medium; Resources=2; Fingerprint=FCA283
    // Broiler-Human:        PENDING
    public int[] Boundaries(ReadOnlySpan<char> text, JsSegmentGranularity granularity, Action<ulong>? charge) =>
        Segmenter.Boundaries(
            text,
            granularity switch
            {
                JsSegmentGranularity.Word => SegmentGranularity.Word,
                JsSegmentGranularity.Sentence => SegmentGranularity.Sentence,
                JsSegmentGranularity.Grapheme => SegmentGranularity.Grapheme,
                _ => throw new ArgumentOutOfRangeException(nameof(granularity)),
            },
            Meter(charge));

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Medium; Resources=2; Fingerprint=36364F
    // Broiler-Human:        PENDING
    public bool IsWordLike(ReadOnlySpan<char> segment, Action<ulong>? charge) =>
        Segmenter.GetWordStatus(segment, Meter(charge)) != WordStatus.None;

    /// <summary>The caller's meter as the owner's delegate, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EE52E7
    // Broiler-Human:        PENDING
    private static UnicodeWorkCharge? Meter(Action<ulong>? charge) => charge is null ? null : new UnicodeWorkCharge(charge);

    /// <summary>The provider of the break table, the one Unicode table this assembly embeds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=78EFF4
    // Broiler-Human:        PENDING
    private sealed class BreakTables : IUnicodeTables
    {
        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D326D2
        // Broiler-Human:        PENDING
        public string UnicodeVersion => JsUnicodeData.UnicodeVersion;

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FFF816
        // Broiler-Human:        PENDING
        public ReadOnlySpan<byte> Table(UnicodeTable table) => JsUnicodeTableData.Get(table);
    }
}
