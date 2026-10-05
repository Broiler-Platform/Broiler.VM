// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           5
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The region and script data <c>Intl.Locale</c>'s information methods read (JSD-0046): each
/// region's week as CLDR states it, and each script's line direction.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FDFFA3
// Broiler-Human:        PENDING
internal sealed class JsLocaleInfo
{
    /// <summary>Each region's first day, weekend start and end and minimal days, a field empty where the world's applies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=905AB8
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, (string FirstDay, string WeekendStart, string WeekendEnd, string MinDays)> Weeks { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Whether each script CLDR states a direction for is written right to left.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BF2E87
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, bool> RightToLeft { get; } = new(System.StringComparer.Ordinal);

    /// <summary>Each language's list patterns, by <c>language|type</c> with CLDR's type names (JSD-0048).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F35DA3
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, (string Start, string Middle, string End, string Pair)> ListPatterns { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Each language's relative time literals and patterns, by <c>language|field|key</c> (JSD-0049).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=19AB56
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, string> RelativeTimes { get; } = new(System.StringComparer.Ordinal);

    /// <summary>Each language's display names, by <c>language|kind|code</c> (JSD-0052).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7AB7F4
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, string> DisplayNames { get; } = new(System.StringComparer.Ordinal);
}
