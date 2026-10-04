// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           24
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The tables the internationalization surface is built from, which a composition admitting
/// <see cref="JsSurfaces.Intl"/> hands to the profile (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Data the composition supplies, so a composition that declines the surface carries none of
/// it</b> (JSD-0027 section 5 item 3). The tables are generated from the pinned CLDR release into an
/// assembly of their own; the profile reads them through this interface and references no such
/// assembly, as it reads emitted machine code through <see cref="IJsNativeEmitter"/>.
/// </para>
/// <para>
/// <b>Each table is bytes in a form its reader in the profile decodes</b>, named by
/// <see cref="JsIntlTable"/>. An implementation answers the same bytes for the life of the process,
/// and the profile may decode a table once and keep what it decoded.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D2D630
// Broiler-Human:        PENDING
public interface IJsIntlData
{
    /// <summary>The CLDR release the tables were generated from, e.g. <c>48.2.0</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=421618
    // Broiler-Human:        PENDING
    string CldrVersion { get; }

    /// <summary>One table's bytes; empty for a table this data does not carry.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E1A035
    // Broiler-Human:        PENDING
    System.ReadOnlySpan<byte> Table(JsIntlTable table);
}

/// <summary>The tables an <see cref="IJsIntlData"/> carries.</summary>
/// <remarks>
/// The text tables are ASCII, one record per line and fields separated by <c>|</c>, in the order the
/// generator writes them; a character a line cannot carry is written <c>\uXXXX</c>. The collation
/// tables are binary and their reader states their layout.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E7C352
// Broiler-Human:        PENDING
public enum JsIntlTable
{
    /// <summary>Nothing.</summary>
    None = 0,

    /// <summary>CLDR's likely subtags: <c>from</c>, <c>to</c>.</summary>
    LikelySubtags = 1,

    /// <summary>CLDR's aliases: <c>kind</c>, <c>from</c>, <c>replacement</c>, for the language, script, region, variant and subdivision kinds.</summary>
    Aliases = 2,

    /// <summary>The BCP 47 extension keys and types: <c>extension</c>, <c>key</c>, <c>type</c>, <c>preferred</c>.</summary>
    Extensions = 3,

    /// <summary>The locales this data supports, one per line, in canonical form.</summary>
    Locales = 4,

    /// <summary>The root collation, binary.</summary>
    CollationRoot = 5,

    /// <summary>The tailorings the supported locales use, binary, in weights scaled by 256.</summary>
    CollationTailorings = 6,

    /// <summary>The ranges of Soft_Dotted, which Lithuanian's casing reads: <c>first</c>, <c>last</c>, in hexadecimal.</summary>
    SoftDotted = 7,

    /// <summary>Each language's number data for <c>latn</c>: <c>language</c>, a dotted <c>key</c>, <c>value</c>.</summary>
    NumberLocales = 8,

    /// <summary>
    /// Each language's currencies: <c>language</c>, <c>code</c>, <c>symbol</c>, <c>narrow</c>, <c>one</c>,
    /// <c>other</c>, <c>name</c>, and for the symbol and the narrow symbol whether its first and last
    /// characters are symbols or separators (<c>S</c>) or not (<c>L</c>).
    /// </summary>
    Currencies = 9,

    /// <summary>The currencies whose fraction digits are not 2: <c>code</c>, <c>digits</c>.</summary>
    CurrencyDigits = 10,

    /// <summary>The numbering systems with a simple digit mapping: <c>name</c>, <c>digits</c>.</summary>
    NumberingSystems = 11,

    /// <summary>Each language's cardinal plural rules: <c>language</c>, <c>category</c>, <c>rule</c>.</summary>
    Plurals = 12,

    /// <summary>Each language's plural range rules: <c>language</c>, <c>start</c>, <c>end</c>, <c>result</c>.</summary>
    PluralRanges = 13,

    /// <summary>Each language's unit patterns: <c>language</c>, <c>width</c>, <c>unit</c>, <c>field</c>, <c>value</c>.</summary>
    Units = 14,

    /// <summary>
    /// Each language's Gregorian calendar, date field and zone name data: <c>language</c>, a dotted
    /// <c>key</c>, <c>value</c>.
    /// </summary>
    DateLocales = 15,

    /// <summary>The hour cycles of a region: <c>region</c>, <c>allowed</c>, <c>preferred</c>.</summary>
    TimeData = 16,

    /// <summary>Each language's day period rules: <c>language</c>, <c>period</c>, <c>at</c> or <c>from</c>, <c>before</c>.</summary>
    DayPeriods = 17,

    /// <summary>
    /// Each region's week: <c>region</c>, <c>firstDay</c>, <c>weekendStart</c>, <c>weekendEnd</c>,
    /// <c>minDays</c>, a field empty where the world's (<c>001</c>) applies.
    /// </summary>
    WeekData = 18,

    /// <summary>Each script's line direction: <c>script</c>, <c>YES</c> for right to left or <c>NO</c>.</summary>
    Scripts = 19,

    /// <summary>Each language's ordinal plural rules: <c>language</c>, <c>category</c>, <c>rule</c>.</summary>
    Ordinals = 20,

    /// <summary>
    /// Each language's list patterns: <c>language</c>, <c>type</c> as CLDR names it, and the
    /// <c>start</c>, <c>middle</c>, <c>end</c> and two-element patterns.
    /// </summary>
    ListPatterns = 21,

    /// <summary>
    /// Each language's relative time data: <c>language</c>, <c>field</c> (a unit, or a unit with
    /// <c>-short</c> or <c>-narrow</c>), <c>key</c> (a value's literal, or <c>future.</c> or
    /// <c>past.</c> and a plural category) and the pattern.
    /// </summary>
    RelativeTimes = 22,
}
