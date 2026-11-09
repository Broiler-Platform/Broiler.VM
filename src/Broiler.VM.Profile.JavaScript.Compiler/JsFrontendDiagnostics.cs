// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  2/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The shared library's diagnostics as this family's: each shared code mapped onto the registry code
/// the mapping gives it, and rendered with the text this family rendered for its rule (JSD-0083,
/// decision D11; milestone SJL-13.3, step 1).
/// </summary>
/// <remarks>
/// <para>
/// <b>The mapping is the file, not a copy of it.</b> The compiler embeds
/// <c>src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv</c>, which the mapping
/// checker holds to the registry, the catalogue and the stage table (bundle <c>sjl-13-001</c>'s part
/// E), and reads its <c>code</c> rows: one shared code, one registry code. A shared code the file does
/// not map is a defect of the build, never a refusal with a guessed code.
/// </para>
/// <para>
/// <b>The text is this family's</b>: for each shared code one sentence that states the code's own
/// condition, with the library's arguments where it names a word; the regular-expression validator's
/// text travels as its argument (decision D7). A manifest refusal's text ends with the 2104 suffix
/// " is not admitted by the declared feature manifest". <i>(Revised 2026-11-09, later that day, with
/// JSD-0083's note of 2026-11-09, later that day, on decision D11, "One true text per code": it read "the wide parser's, the binder's or the
/// tokenizer's sentence for the rule, with the library's arguments where it named a word; [...] A code whose
/// meaning stays keeps the text it had, the 2104 suffix " is not admitted by the declared feature manifest"
/// included.")</i>
/// </para>
/// <para>
/// <b>One true text per shared code, so the base's per-site texts retire.</b> The library names its
/// condition and not the base's site, so a site's own sentence cannot be chosen, and one site's sentence
/// can be false of another's source under the same code. Each code's text in
/// <c>JsFrontendDiagnostics.Texts.cs</c> is written to be true of every source the library refuses under
/// it, a general wording where the code covers several of the base's sites. <i>(Revised 2026-11-09, later
/// that day, with the same note: it read "<b>One text per shared code, so not every base text survives.</b>
/// Where the base rendered several sentences under one registry code at sites the library reports under one
/// shared code (2101's unexpected-token sites, for one), the table keeps one of them, and the others' inputs
/// now read it [...]. That departs from the sentence above as decision D11 states it (added 2026-11-09)."; the
/// owner's decision of that day, later, makes the departure the decision.)</i>
/// </para>
/// <para>
/// <b>Nothing here is mutable</b> (rule N12): the table is read once into a frozen dictionary.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=88B6FF
// Broiler-Falsified-If: a library diagnostic reaches a caller under a registry code other than the one frontend-mapping.tsv gives its shared code, or at another line or column than the library reported
// Broiler-Human:        PENDING
internal static partial class JsFrontendDiagnostics
{
    /// <summary>The embedded mapping's resource name.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=38F4AA
    // Broiler-Human:        PENDING
    internal const string MappingResource = "Broiler.VM.Profile.JavaScript.Compiler.frontend-mapping.tsv";

    /// <summary>Each shared code's registry code, from the mapping's <c>code</c> rows.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8E5552
    // Broiler-Human:        PENDING
    private static readonly System.Collections.Frozen.FrozenDictionary<string, SliceSourceDiagnosticCode> Codes = ReadMapping();

    /// <summary>One library diagnostic, as this family reports it, its text read with the unit's source.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EFDE9C
    // Broiler-Falsified-If: the registry code, line or column of the answer differs from the mapping's code for the diagnostic's shared code and the library's span
    // Broiler-Human:        PENDING
    internal static SliceSourceDiagnostic Map(SyntaxDiagnostic diagnostic, string source)
    {
        if (!Codes.TryGetValue(diagnostic.Code, out var code))
        {
            throw new System.InvalidOperationException(
                "the shared code " + diagnostic.Code + " (" + diagnostic.MessageId + ") has no row in frontend-mapping.tsv");
        }

        return new SliceSourceDiagnostic(code, Render(diagnostic, source), diagnostic.Span.Line, diagnostic.Span.Column);
    }

    /// <summary>The mapping's <c>code</c> rows, read from the embedded file.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D259ED
    // Broiler-Human:        PENDING
    private static System.Collections.Frozen.FrozenDictionary<string, SliceSourceDiagnosticCode> ReadMapping()
    {
        using var stream = typeof(JsFrontendDiagnostics).Assembly.GetManifestResourceStream(MappingResource)
            ?? throw new System.InvalidOperationException("the compiler does not embed " + MappingResource);
        using var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);

        var codes = new System.Collections.Generic.Dictionary<string, SliceSourceDiagnosticCode>(System.StringComparer.Ordinal);

        while (reader.ReadLine() is { } line)
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var cells = line.Split('\t');

            // kind, shared_code, row, conditions, wide_codes, slice_codes, registry_code, ...
            if (cells.Length < 7 || cells[0] != "code")
            {
                continue;
            }

            codes[cells[1]] = (SliceSourceDiagnosticCode)int.Parse(
                cells[6], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture);
        }

        return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(codes, System.StringComparer.Ordinal);
    }
}
