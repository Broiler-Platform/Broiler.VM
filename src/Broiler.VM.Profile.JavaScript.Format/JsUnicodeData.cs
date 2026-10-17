// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           1
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Unicode.Algorithms;

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The Unicode 17.0.0 tables this assembly embeds, for the JavaScript family's other assemblies: the
/// property, case folding and string property tables the matcher, the compiler and the runtime read
/// through the Unicode area's owner (JSD-0074).
/// </summary>
/// <remarks>
/// <para>
/// <b>Public because the readers are in other assemblies</b>, as <c>JsUnicodeLexical</c> was before
/// SJL-7.4: the compiler's tokenizer and the runtime's lexical checks read the same tables the
/// matcher reads, and this family opens its internals to nobody (rule A10). It names no type of the
/// owner's package, so the family's public surface carries none (rule I6): a table is asked for by
/// the number the owner's <c>UnicodeTable</c> gives it, and answered as constant bytes.
/// </para>
/// <para>
/// <b>The bytes are generated</b>: <c>JsUnicodeTables.g.cs</c>, which the owner's generator writes from
/// the files <c>src/tests/unicode/pins/unicode.pin</c> names and rule N22 holds byte for byte
/// (decision SJL-D-6 (c)). A table this assembly does not embed is answered empty.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=475712
// Broiler-Human:        PENDING
public static class JsUnicodeData
{
    /// <summary>The Unicode version of the tables, which rule I4 holds to the pin and the owner's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD1BD0
    // Broiler-Human:        PENDING
    public const string UnicodeVersion = "17.0.0";

    /// <summary>
    /// The bytes of the table the owner's <c>UnicodeTable</c> numbers <paramref name="table"/>, or an
    /// empty span when this assembly does not embed it.
    /// </summary>
    /// <param name="table">The table's number in the owner's enumeration, which is never renumbered.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D7C373
    // Broiler-Human:        PENDING
    public static ReadOnlySpan<byte> Table(int table) => JsUnicodeTableData.Get((UnicodeTable)table);
}

/// <summary>The provider the owner's algorithms read this assembly's tables through.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B758F6
// Broiler-Human:        PENDING
internal sealed class JsFormatUnicodeTables : IUnicodeTables
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

/// <summary>
/// The owner's algorithms over this assembly's tables, made once: the property lookups and the
/// lexical classes the matcher's property escapes and group names read, case folding for its
/// Canonicalize, and the properties of strings for the <c>v</c> flag.
/// </summary>
/// <remarks>
/// Each instance is immutable and safe to share between threads (the owner's adapter guide), so one
/// serves every engine of the process. None takes a meter: a lookup is a binary search bounded by the
/// tables, and the matcher charges for what it does with the answer exactly as it did when the lookup
/// was its own (JSD-0074, decision 4: the lookups and folding charge nothing).
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=695E20
// Broiler-Human:        PENDING
internal static class JsFormatUnicode
{
    /// <summary>The provider every instance reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A4A67D
    // Broiler-Human:        PENDING
    private static readonly JsFormatUnicodeTables Tables = new();

    /// <summary>The exact property lookups of <c>\p{...}</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=96B680
    // Broiler-Human:        PENDING
    internal static readonly UnicodeProperties Properties = new(Tables);

    /// <summary>IdentifierStartChar, IdentifierPartChar and WhiteSpace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D9919C
    // Broiler-Human:        PENDING
    internal static readonly EcmaScriptLexical Lexical = new(Properties);

    /// <summary>Simple case folding and the non-<c>u</c> canonicalisation, with their orbits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9683E
    // Broiler-Human:        PENDING
    internal static readonly UnicodeCaseFolding Folding = new(Tables);

    /// <summary>The properties of strings the <c>v</c> flag admits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=486F5C
    // Broiler-Human:        PENDING
    internal static readonly UnicodeStringProperties StringProperties = new(Tables);
}
