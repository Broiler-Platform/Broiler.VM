// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           1
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
/// The display-name data the date format's zone names read (JSD-0052). (Revised 2026-11-03, at
/// SJL-9.4's review of sub-areas (b) and (c). It read: "The list, relative-time and display-name data
/// the small formatters read (JSD-0048, JSD-0049, JSD-0052).")
/// </summary>
/// <remarks>
/// <para>
/// Until SJL-9.4 it also held the region and script data <c>Intl.Locale</c>'s information methods
/// read (JSD-0046), each region's week and each script's line direction; since then the shared Intl
/// library answers them from the same tables (JSD-0082, decision D24).
/// </para>
/// <para>
/// Until SJL-9.4's sub-area (c) it also held <c>ListPatterns</c>, each language's list patterns by
/// <c>language|type</c> (JSD-0048), and <c>RelativeTimes</c>, each language's relative time literals
/// and patterns by <c>language|field|key</c> (JSD-0049). Their only readers were the ListFormat and
/// RelativeTimeFormat cores, which that sub-area deleted; the shared Intl library's
/// <c>ListFormatter</c> and <c>RelativeTimeFormatter</c> read the same tables through its own decoder
/// (JSD-0082, decision D24). What is left, with the type, is sub-area (d)'s (JSD-0082, X6).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FDFFA3
// Broiler-Human:        PENDING
internal sealed class JsLocaleInfo
{
    /// <summary>Each language's display names, by <c>language|kind|code</c> (JSD-0052).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A3186
    // Broiler-Human:        PENDING
    internal Dictionary<string, string> DisplayNames { get; } = new(StringComparer.Ordinal);
}
