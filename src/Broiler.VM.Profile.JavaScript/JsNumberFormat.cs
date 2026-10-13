// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Globalization;
using Broiler.ECMAScript.Numbers;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// This family's adapter of the shared number-to-string conversion: the strings the engine keeps for
/// the commonest Numbers, in front of <c>Broiler.ECMAScript.Algorithms</c>, which computes every
/// conversion (JSD-0065).
/// </summary>
/// <remarks>
/// <para>
/// <b>What stays here, and why</b> (milestone SJL-3.4, item 3). The conversions this class held -
/// <c>ToJsString</c>, <c>ToRadixString</c>, <c>ToNumber</c>, <c>ParseInt</c>, <c>ParseFloat</c>,
/// <c>IsWhiteSpace</c>, <c>Trim</c> and their helpers - are deleted and recorded in rule I5's
/// register; the library's <c>NumberText</c>, <c>NumberParsing</c> and <c>NumericKeys</c> replace
/// them. What stays is string creation, which the area's adapter definition leaves to the engine:
/// the shared strings for the integers 0 to 255 and for <c>NaN</c> and the infinities, so that the
/// commonest conversions allocate nothing, and the white-space trim's choice to return the string it
/// was given when nothing is trimmed. None of it computes a digit or reads a character's class.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=10C05B
// Broiler-Human:        PENDING
internal static class JsNumberFormat
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6F3573
    // Broiler-Human:        PENDING
    private static readonly string[] SmallIntegers = BuildSmallIntegers();

    /// <summary>
    /// <c>Number::toString(x, 10)</c>: the library's <see cref="NumberText.ToString(double)"/>, with the
    /// engine's shared string for an integer from 0 to 255, <c>NaN</c> and the infinities.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=18F7BF
    // Broiler-Human:        PENDING
    internal static string NumberToString(double value)
    {
        // -0 is 0 here, which is what Number::toString writes for it.
        if (value >= 0 && value < SmallIntegers.Length && value == Math.Floor(value))
        {
            return SmallIntegers[(int)value];
        }

        if (double.IsNaN(value))
        {
            return "NaN";
        }

        if (double.IsInfinity(value))
        {
            return value > 0 ? "Infinity" : "-Infinity";
        }

        return NumberText.ToString(value);
    }

    /// <summary>Renders an unsigned integer, which is what an array index always is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F911BF
    // Broiler-Human:        PENDING
    internal static string ToUintString(uint value) =>
        value < SmallIntegers.Length
            ? SmallIntegers[(int)value]
            : value.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// The text without the specification's <c>StrWhiteSpace</c> at either end, the library's
    /// <see cref="NumberParsing.TrimStrWhiteSpace"/>; the string itself when there is none to trim.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0AE18A
    // Broiler-Human:        PENDING
    internal static string TrimWhiteSpace(string text)
    {
        var trimmed = NumberParsing.TrimStrWhiteSpace(text, TrimEnds.Both);
        return trimmed.Length == text.Length ? text : trimmed.ToString();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=786AF1
    // Broiler-Human:        PENDING
    private static string[] BuildSmallIntegers()
    {
        var table = new string[256];

        for (var at = 0; at < table.Length; at++)
        {
            table[at] = at.ToString(CultureInfo.InvariantCulture);
        }

        return table;
    }
}
