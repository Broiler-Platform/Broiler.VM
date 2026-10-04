// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           2
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

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
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=540C9B
// Broiler-Human:        PENDING
public sealed class JsCldrData : IJsIntlData
{
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D9334A
    // Broiler-Human:        PENDING
    public System.ReadOnlySpan<byte> Table(JsIntlTable table) => table switch
    {
        JsIntlTable.LikelySubtags => JsCldrTables.LikelySubtags,
        JsIntlTable.Aliases => JsCldrTables.Aliases,
        JsIntlTable.Extensions => JsCldrTables.Extensions,
        JsIntlTable.Locales => JsCldrTables.Locales,
        JsIntlTable.CollationRoot => JsCldrTables.CollationRoot,
        JsIntlTable.CollationTailorings => JsCldrTables.CollationTailorings,
        JsIntlTable.SoftDotted => JsCldrTables.SoftDotted,
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
        _ => default,
    };
}
