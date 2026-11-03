// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           3
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The list, relative-time and display-name data the small formatters read (JSD-0048, JSD-0049,
/// JSD-0052).
/// </summary>
/// <remarks>
/// Until SJL-9.4 it also held the region and script data <c>Intl.Locale</c>'s information methods
/// read (JSD-0046), each region's week and each script's line direction; since then the shared Intl
/// library answers them from the same tables (JSD-0082, decision D24).
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FDFFA3
// Broiler-Human:        PENDING
internal sealed class JsLocaleInfo
{
    /// <summary>Each language's list patterns, by <c>language|type</c> with CLDR's type names (JSD-0048).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=020F8B
    // Broiler-Human:        PENDING
    internal Dictionary<string, (string Start, string Middle, string End, string Pair)> ListPatterns { get; } = new(StringComparer.Ordinal);

    /// <summary>Each language's relative time literals and patterns, by <c>language|field|key</c> (JSD-0049).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1AA84
    // Broiler-Human:        PENDING
    internal Dictionary<string, string> RelativeTimes { get; } = new(StringComparer.Ordinal);

    /// <summary>Each language's display names, by <c>language|kind|code</c> (JSD-0052).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A3186
    // Broiler-Human:        PENDING
    internal Dictionary<string, string> DisplayNames { get; } = new(StringComparer.Ordinal);
}
