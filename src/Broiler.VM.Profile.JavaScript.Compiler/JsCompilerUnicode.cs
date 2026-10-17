// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           1
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Unicode.Algorithms;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The Unicode classes the compiler reads - the lexical grammar's identifier and white-space classes,
/// and the general category the parser's routing checks ask about - from the Unicode area's owner over
/// the tables Format embeds (JSD-0074; milestone SJL-7.4).
/// </summary>
/// <remarks>
/// <para>
/// <b>The tables are Format's</b>, read through its public <see cref="JsUnicodeData"/>, so the tokenizer,
/// the matcher's group names and the runtime classify a character from one copy of the pinned data. Until
/// SJL-7.4 the tokenizer read Format's <c>JsUnicodeLexical</c>, and the parser asked the platform's
/// <c>char.IsLetter</c>, <c>char.IsWhiteSpace</c> and <c>char.IsLetterOrDigit</c> (correspondence row W6).
/// </para>
/// <para>
/// <b>Nothing here is mutable</b> (rule N12): the instances are read-only references to immutable objects,
/// built once and shared by every compilation of the process, as the owner's adapter guide allows.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DA4EEC
// Broiler-Human:        PENDING
internal static class JsCompilerUnicode
{
    /// <summary>The owner's property lookups over Format's tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B66754
    // Broiler-Human:        PENDING
    private static readonly UnicodeProperties Properties = new(new FormatTables());

    /// <summary>IdentifierStartChar, IdentifierPartChar and WhiteSpace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D9919C
    // Broiler-Human:        PENDING
    internal static readonly EcmaScriptLexical Lexical = new(Properties);

    /// <summary>
    /// Whether <paramref name="c"/> is a letter: General_Category Lu, Ll, Lt, Lm or Lo, the categories
    /// <c>char.IsLetter</c> named, from the pinned Unicode 17.0.0 data rather than the platform's.
    /// </summary>
    /// <remarks>
    /// The parser's two routing checks that asked <c>char.IsLetter</c> keep its meaning exactly, so what
    /// they route and the diagnostics they give move only where Unicode 17.0 and the platform's data
    /// disagree about a letter. A surrogate code unit is category Cs, and not a letter, as before.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E729E9
    // Broiler-Human:        PENDING
    internal static bool IsLetter(char c) =>
        c < 0x80
            ? (uint)((c | 0x20) - 'a') <= 'z' - 'a'
            : Properties.GeneralCategoryOf(c) is GeneralCategory.UppercaseLetter or GeneralCategory.LowercaseLetter
                or GeneralCategory.TitlecaseLetter or GeneralCategory.ModifierLetter or GeneralCategory.OtherLetter;

    /// <summary>The provider of Format's tables, which the owner's algorithms read.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=96D3B0
    // Broiler-Human:        PENDING
    private sealed class FormatTables : IUnicodeTables
    {
        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D326D2
        // Broiler-Human:        PENDING
        public string UnicodeVersion => JsUnicodeData.UnicodeVersion;

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5BC117
        // Broiler-Human:        PENDING
        public ReadOnlySpan<byte> Table(UnicodeTable table) => JsUnicodeData.Table((int)table);
    }
}
