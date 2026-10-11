// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           0
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The specification's own well-formedness rule for a name, written out rather than delegated.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE PLATFORM DECODER IS NOT THIS RULE AND MAY NOT BE SUBSTITUTED FOR IT.</b>
/// <c>System.Text.Encoding.UTF8</c> is a decoder: handed a malformed sequence it replaces or throws
/// depending on how it was constructed, and its replacement behaviour has to answer "what should
/// the text be" rather than "are these bytes well formed".
/// </para>
/// <para>
/// <b><see cref="System.Text.Unicode.Utf8.IsValid(System.ReadOnlySpan{byte})"/> is a validator.</b>
/// It creates no decoded string and does not repair invalid input. It adheres strictly to Unicode scalar
/// rules, rejecting overlong encodings, UTF-16 surrogates, and values above U+10FFFF.
/// </para>
/// <para>
/// <b>It answers about bytes and produces no string.</b> A name is compared and stored as the bytes
/// the artifact carried, so nothing here allocates and nothing here has to be reversed later.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=88A0F6
// Broiler-Falsified-If: an overlong form, a surrogate code point, or a value above U+10FFFF is reported well formed
// Broiler-Human:        PENDING
internal static class WasmName
{
    /// <summary>Whether <paramref name="bytes"/> is a well-formed name under the format's rule.</summary>
    /// <remarks>
    /// The empty sequence is well formed: an export may be named by the empty string, and the
    /// suite has a case for it.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=FB583B
    // Broiler-Falsified-If: a sequence this method accepts decodes to a different scalar value than the specification assigns it
    // Broiler-Human:        PENDING
    internal static bool IsWellFormed(System.ReadOnlySpan<byte> bytes) =>
        System.Text.Unicode.Utf8.IsValid(bytes);
}
