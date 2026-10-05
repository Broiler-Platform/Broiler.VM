// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           5
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The Gregorian calendar, hour cycle and day period data <c>Intl.DateTimeFormat</c> reads, decoded
/// from the composition's tables (JSD-0045).
/// </summary>
/// <remarks>
/// <para>
/// <b>The pattern generators are made once per language and hour character</b> and shared: a
/// generator holds the language's patterns in ICU's matching structures and never changes after it
/// is made, so every format of the same language reads the same one. They are made under a lock,
/// because the tables are shared by the runtimes of one descriptor.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B000EB
// Broiler-Human:        PENDING
internal sealed class JsDateData
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=328C44
    // Broiler-Human:        PENDING
    private readonly object gate = new();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2D1036
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsDatePatternGenerator> generators =
        new(System.StringComparer.Ordinal);

    /// <summary>Each language's data, by dotted key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1CB71C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> Locales { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>The hour cycles a region allows and prefers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=892C48
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, (string Allowed, string Preferred)> TimeData { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Each language's day period rules.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D4C1DF
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<JsDayPeriodRule>> DayPeriods { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>
    /// The pseudo-locale of the ISO 8601 calendar's patterns, and the suffix of each language's data
    /// in that calendar (JSD-0055).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B2F82B
    // Broiler-Human:        PENDING
    internal const string Iso8601 = "@iso8601";

    /// <summary>Whether a key is one of a calendar's patterns rather than one of its names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D3D1C3
    // Broiler-Human:        PENDING
    internal static bool IsPatternKey(string key) =>
        key.StartsWith("dateFormats.", System.StringComparison.Ordinal) ||
        key.StartsWith("timeFormats.", System.StringComparison.Ordinal) ||
        key.StartsWith("dateTime.", System.StringComparison.Ordinal) ||
        key.StartsWith("atTime.", System.StringComparison.Ordinal) ||
        key.StartsWith("available.", System.StringComparison.Ordinal) ||
        key.StartsWith("append.", System.StringComparison.Ordinal) ||
        key.StartsWith("interval.", System.StringComparison.Ordinal);

    /// <summary>A language's value for a key, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E99C1
    // Broiler-Human:        PENDING
    internal string? Value(string language, string key) =>
        Locales.TryGetValue(language, out var values) && values.TryGetValue(key, out var value) ? value : null;

    /// <summary>The pattern generator of a language whose default hour character is <paramref name="hourChar"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1BB26
    // Broiler-Human:        PENDING
    internal JsDatePatternGenerator Generator(string language, char hourChar, string decimalSymbol)
    {
        var key = language + "|" + hourChar + "|" + decimalSymbol;

        lock (gate)
        {
            if (!generators.TryGetValue(key, out var generator))
            {
                generator = new JsDatePatternGenerator(this, language, hourChar, decimalSymbol);
                generators[key] = generator;
            }

            return generator;
        }
    }

    /// <summary>
    /// The hour character a region prefers - <c>h</c>, <c>H</c>, <c>K</c> or <c>k</c> - from CLDR's
    /// time data, the world's where the region has none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8EE981
    // Broiler-Human:        PENDING
    internal char PreferredHourChar(string region)
    {
        if (!TimeData.TryGetValue(region, out var data) && !TimeData.TryGetValue("001", out data))
        {
            return 'H';
        }

        return data.Preferred.Length != 0 ? data.Preferred[0] : 'H';
    }
}

/// <summary>A day period rule: the period, and the minute of the day it is at or the minutes it is from and before.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E94DDA
// Broiler-Human:        PENDING
internal readonly record struct JsDayPeriodRule(string Period, int From, int Before, bool At);
