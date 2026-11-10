// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           1
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The regular-expression validator this family supplies to the shared syntax library's request: the
/// checks the binder's <c>CheckRegExp</c> made, which leave the binder for the library's provider
/// (JSD-0083, decision D7; milestone SJL-13.3, step 1).
/// </summary>
/// <remarks>
/// <para>
/// <b>It answers what the binder answered, in its order and with its texts</b>: a flag the language
/// does not define, or one given twice; <c>u</c> with <c>v</c>; and then the pattern, which the matcher
/// this family runs compiles under the literal's flags, a pattern it refuses being refused with its own
/// message. The checks moved here from the binder unchanged; the binder's had moved there from the
/// lowering's <c>RefuseRegExpLiteralFlags</c> and <c>CompileRegExpLiteral</c> with SJL-12.6 item 2.
/// </para>
/// <para>
/// <b>The library reports the refusal at the literal's span</b>, the span the binder reported it at,
/// under the row of the identifier this answers, EE0019's (IsValidRegularExpressionLiteral), whose
/// registry code is 2215. The text travels as the verdict's argument, so this family renders the text
/// the binder rendered (decision D11).
/// </para>
/// <para>
/// <b>Nothing here is mutable</b> (rule N12): the one instance holds no state, and the matcher it calls
/// builds its program per call.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=CBE35E
// Broiler-Falsified-If: a literal the matcher compiles is refused, or one it refuses, or whose flags the language refuses, is admitted
// Broiler-Human:        PENDING
internal sealed class JsRegularExpressionValidator : IRegularExpressionValidator
{
    /// <summary>The message identifier of IsValidRegularExpressionLiteral's row, EE0019.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=009E70
    // Broiler-Human:        PENDING
    internal const string InvalidLiteral = "regexp.literal.invalid";

    /// <summary>The validator every request of this family names.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0E4EE2
    // Broiler-Human:        PENDING
    internal static readonly JsRegularExpressionValidator Instance = new();

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F365CC
    // Broiler-Human:        PENDING
    private JsRegularExpressionValidator()
    {
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=248115
    // Broiler-Falsified-If: the answer for a literal differs from the binder's CheckRegExp at step 1's base, in outcome or in text
    // Broiler-Human:        PENDING
    public RegularExpressionVerdict Validate(string pattern, string flags)
    {
        for (var at = 0; at < flags.Length; at++)
        {
            if ("dgimsuyv".IndexOf(flags[at], System.StringComparison.Ordinal) < 0 ||
                flags.IndexOf(flags[at], at + 1) >= 0)
            {
                return Refused("Invalid regular expression flags: " + flags, inFlags: true);
            }
        }

        if (flags.Contains('v', System.StringComparison.Ordinal) &&
            flags.Contains('u', System.StringComparison.Ordinal))
        {
            return Refused("Invalid regular expression flags: " + flags + " (u and v may not be combined)", inFlags: true);
        }

        try
        {
            _ = Format.JsRegExpMatcher.Compile(
                pattern,
                flags.Contains('i', System.StringComparison.Ordinal),
                flags.Contains('m', System.StringComparison.Ordinal),
                flags.Contains('s', System.StringComparison.Ordinal),
                flags.Contains('u', System.StringComparison.Ordinal),
                flags.Contains('v', System.StringComparison.Ordinal));
        }
        catch (Format.JsRegExpSyntaxError failure)
        {
            return Refused(failure.Message, inFlags: false);
        }

        return RegularExpressionVerdict.Valid;
    }

    /// <summary>A refusal under EE0019's row, carrying this family's text as its argument.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5EAB3A
    // Broiler-Human:        PENDING
    private static RegularExpressionVerdict Refused(string text, bool inFlags) =>
        RegularExpressionVerdict.Invalid(InvalidLiteral, text, 2215, 0, inFlags);
}
