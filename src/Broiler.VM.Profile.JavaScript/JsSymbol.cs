// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           3
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One Symbol: a property key whose identity is the object itself.
/// </summary>
/// <remarks>
/// <para>
/// <b>A class rather than a struct, because identity is the whole of what a Symbol is.</b> Two
/// Symbols with the same description are different keys, and the only way to say "the same one" is
/// reference equality. <see cref="JsValue"/> holds it in the reference field it already has for
/// Strings and objects, so a Symbol costs the value representation nothing.
/// </para>
/// <para>
/// <b>It is deliberately NOT a <see cref="JsObject"/>.</b> A Symbol is a primitive, and making it
/// an object would put it on a prototype chain, give it properties, and make <c>typeof</c> answer
/// wrongly. The wrapper a method call on a Symbol needs is built on demand, the way a String's is.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FD45EA
// Broiler-Human:        PENDING
internal sealed class JsSymbol
{
    /// <summary>Creates a Symbol with an optional description.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E3D9E6
    // Broiler-Human:        PENDING
    internal JsSymbol(string description, bool described)
    {
        Description = description;
        Described = described;
    }

    /// <summary>The description, which is the empty string when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1AC63F
    // Broiler-Human:        PENDING
    internal string Description { get; }

    /// <summary>
    /// Whether a description was given at all, which <c>undefined</c> and <c>""</c> differ by.
    /// </summary>
    /// <remarks>
    /// <c>Symbol().description</c> is <c>undefined</c> and <c>Symbol("").description</c> is the
    /// empty string, and a single string field cannot tell them apart. The flag is what makes the
    /// two answers different rather than nearly the same.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=394878
    // Broiler-Human:        PENDING
    internal bool Described { get; }

    /// <summary>What <c>String(symbol)</c> answers, which is not what <c>ToString</c> does.</summary>
    /// <remarks>
    /// The implicit coercion of a Symbol to a String is a <c>TypeError</c> — that is the whole
    /// point of the type — and this is the explicit form the language nonetheless provides. Keeping
    /// them apart is why this is a named member rather than an override of <c>ToString</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B25F68
    // Broiler-Human:        PENDING
    internal string Rendered => "Symbol(" + Description + ")";
}
