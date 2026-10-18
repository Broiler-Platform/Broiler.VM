// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;
using Broiler.ECMAScript.Temporal;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The thin wrapper over the proposal's ISO 8601 / RFC 9557 grammar (s13.31), which is the shared
/// library's <see cref="TemporalGrammar"/> since SJL-10.4 (JSD-0078's X6: this family's linear
/// parser moved whole, behind its <c>Try</c> interface): each refusal, a
/// <see cref="TemporalParseError"/>, raised as the specification's RangeError with this family's
/// message. Nothing here reads a character of the text.
/// </summary>
/// <remarks>
/// The grammar takes the caller's meter (JSD-0078's X3), and this family passes none: it charged
/// nothing for a Temporal string before the cut-over, so no charge moves.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CB97FA
// Broiler-Human:        PENDING
internal static class JsTemporalParser
{
    /// <summary>
    /// The proposal's ParseISODateTime (s13.35): the first of <paramref name="goals"/> the string
    /// parses as, with its annotations checked, or the RangeError.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6CF410
    // Broiler-Human:        PENDING
    internal static IsoParseResult Parse(JsEngine engine, string text, params TemporalGoal[] goals) =>
        TemporalGrammar.TryParse(text, goals, out var result, out var error) ? result : throw ParseError(engine, text, error);

    /// <summary>The proposal's ParseTemporalInstant (s8.5.4): the instant's epoch nanoseconds, or the RangeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=841BD5
    // Broiler-Human:        PENDING
    internal static BigInteger Instant(JsEngine engine, string text) =>
        TemporalGrammar.TryParseInstant(text, out var epochNanoseconds, out var error)
            ? epochNanoseconds
            : throw (error == TemporalParseError.OutOfRange
                ? engine.Error("RangeError", "Temporal: the epoch nanoseconds are outside the representable range of instants")
                : ParseError(engine, text, error));

    /// <summary>The proposal's ParseTemporalDurationString (s13.37): the ten fields, exact, or the RangeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=64BF83
    // Broiler-Human:        PENDING
    internal static DurationFields Duration(JsEngine engine, string text) =>
        TemporalGrammar.TryParseDuration(text, out var duration, out _)
            ? duration
            : throw engine.Error("RangeError", "Temporal: " + text + " is not a duration");

    /// <summary>The proposal's ParseTemporalCalendarString (s13.36): the identifier as written, not yet canonicalized, or the RangeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BAFAF8
    // Broiler-Human:        PENDING
    internal static string CalendarString(JsEngine engine, string text) =>
        TemporalGrammar.TryParseCalendarString(text, out var calendar, out _)
            ? calendar
            : throw engine.Error("RangeError", "Temporal: " + text + " is not a calendar");

    /// <summary>
    /// The proposal's ParseTemporalTimeZoneString (s13.38): the zone's name, not yet looked up, or its
    /// offset in minutes, or the RangeError.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ACBD64
    // Broiler-Human:        PENDING
    internal static TimeZoneIdentifierRecord TimeZoneString(JsEngine engine, string text) =>
        TemporalGrammar.TryParseTimeZoneString(text, out var zone, out var error) ? zone : throw ParseError(engine, text, error);

    /// <summary>The RangeError of each refusal, with the message this family gave it before the grammar moved.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2A338B
    // Broiler-Human:        PENDING
    private static JsThrow ParseError(JsEngine engine, string text, TemporalParseError error) => error switch
    {
        TemporalParseError.CriticalCalendarConflict =>
            engine.Error("RangeError", "Temporal: a critical calendar annotation conflicts with another in " + text),
        TemporalParseError.UnknownCriticalAnnotation =>
            engine.Error("RangeError", "Temporal: a critical annotation is not recognized in " + text),
        TemporalParseError.YearMonthOutsideIsoCalendar =>
            engine.Error("RangeError", "Temporal: a year-month without a day must be in the ISO 8601 calendar: " + text),
        TemporalParseError.MonthDayOutsideIsoCalendar =>
            engine.Error("RangeError", "Temporal: a month-day without a year must be in the ISO 8601 calendar: " + text),
        TemporalParseError.OutOfRange =>
            engine.Error("RangeError", "Temporal: the instant is outside the representable range"),
        TemporalParseError.NoTimeZone =>
            engine.Error("RangeError", "Temporal: " + text + " names no time zone"),
        TemporalParseError.SubMinuteTimeZoneOffset =>
            engine.Error("RangeError", "Temporal: a time zone offset cannot have seconds: " + text),
        _ => engine.Error("RangeError", "Temporal: cannot parse " + Quoted(text)),
    };

    /// <summary>A string as a message quotes it: at most 64 characters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=12D3D8
    // Broiler-Human:        PENDING
    private static string Quoted(string text) => "'" + (text.Length > 64 ? text[..64] + "..." : text) + "'";
}
