// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  3/10 max
// Unverified:       3
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
/// the text be" rather than "are these bytes well formed". Those are different questions, and the
/// cases where the two answers separate - an overlong encoding of a value that has a shorter form,
/// a surrogate code point encoded in three bytes, a four-byte sequence naming a value above
/// U+10FFFF - are exactly the cases the specification's test suite has files for. A validator that
/// asked the platform decoder would pass a module the specification calls malformed, and would do
/// it silently.
/// </para>
/// <para>
/// So the rule is written here as a range table over the leading byte, which is the form the
/// specification itself uses: each leading byte fixes both the sequence length and the range its
/// first continuation byte may take, and the narrowed continuation ranges for <c>0xE0</c>,
/// <c>0xED</c>, <c>0xF0</c> and <c>0xF4</c> are precisely where overlong forms, surrogates and
/// out-of-range values are excluded.
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
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=683B7D
    // Broiler-Falsified-If: a sequence this method accepts decodes to a different scalar value than the specification assigns it
    // Broiler-Human:        PENDING
    internal static bool IsWellFormed(System.ReadOnlySpan<byte> bytes)
    {
        var index = 0;

        while (index < bytes.Length)
        {
            var lead = bytes[index];

            if (lead <= 0x7F)
            {
                index++;
                continue;
            }

            // 0x80..0xC1 can never lead: the first eight are continuation bytes with no lead in
            // front of them, and 0xC0 and 0xC1 could only ever begin an overlong two-byte form of a
            // value the one-byte form already holds.
            if (lead <= 0xC1)
            {
                return false;
            }

            if (lead <= 0xDF)
            {
                if (!Follows(bytes, index + 1, 1, 0x80, 0xBF))
                {
                    return false;
                }

                index += 2;
                continue;
            }

            if (lead <= 0xEF)
            {
                // The first continuation byte is narrowed twice: 0xA0 as a floor under 0xE0 refuses
                // the overlong three-byte forms, and 0x9F as a ceiling under 0xED refuses the
                // surrogate range U+D800..U+DFFF, which is not a scalar value and which the
                // specification therefore does not admit in a name.
                var floor = lead == 0xE0 ? (byte)0xA0 : (byte)0x80;
                var ceiling = lead == 0xED ? (byte)0x9F : (byte)0xBF;

                if (!Follows(bytes, index + 1, 1, floor, ceiling) ||
                    !Follows(bytes, index + 2, 1, 0x80, 0xBF))
                {
                    return false;
                }

                index += 3;
                continue;
            }

            if (lead > 0xF4)
            {
                // 0xF5 and above could only begin a sequence naming a value above U+10FFFF, and
                // 0xF8 and above are not a lead byte in any form of the encoding at all.
                return false;
            }

            // 0x90 as a floor under 0xF0 refuses the overlong four-byte forms; 0x8F as a ceiling
            // under 0xF4 refuses everything above U+10FFFF.
            var wideFloor = lead == 0xF0 ? (byte)0x90 : (byte)0x80;
            var wideCeiling = lead == 0xF4 ? (byte)0x8F : (byte)0xBF;

            if (!Follows(bytes, index + 1, 1, wideFloor, wideCeiling) ||
                !Follows(bytes, index + 2, 2, 0x80, 0xBF))
            {
                return false;
            }

            index += 4;
        }

        return true;
    }

    /// <summary>
    /// Whether <paramref name="count"/> bytes from <paramref name="start"/> exist and all lie
    /// inside the inclusive range given.
    /// </summary>
    /// <remarks>
    /// The length test is inside this helper rather than outside it, so a truncated multi-byte
    /// sequence at the end of a name is refused by the same code that refuses a wrong continuation
    /// byte instead of by an index check somebody could forget to write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=240E15
    // Broiler-Falsified-If: it reads past the end of the span, or reports true for a sequence that runs off the end of the name
    // Broiler-Human:        PENDING
    private static bool Follows(
        System.ReadOnlySpan<byte> bytes, int start, int count, byte floor, byte ceiling)
    {
        if (start < 0 || start + count > bytes.Length)
        {
            return false;
        }

        for (var offset = 0; offset < count; offset++)
        {
            var current = bytes[start + offset];

            if (current < floor || current > ceiling)
            {
                return false;
            }
        }

        return true;
    }
}
