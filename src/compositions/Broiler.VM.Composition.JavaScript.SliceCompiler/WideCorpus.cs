// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained malformed entries of format version 2, one per structural refusal that version
/// adds.
/// </summary>
/// <remarks>
/// <para>
/// <b>Five entries, five codes, and the count is the point.</b> Version 2 adds a function table,
/// an environment model and exception regions; each of those is a place where an artifact can be
/// structurally wrong in a way version 1 has no vocabulary for, and the published registry binds
/// each new code to a named entry here. A verifier that refused every version-2 artifact would
/// satisfy none of the five, because they are distinguished by the code and not by the refusal.
/// </para>
/// <para>
/// <b>Every entry is bytes, not a host.</b> They carry the replay mode <c>wide</c>, and the only
/// thing that mode changes is the descriptor the caller presents - the format version and the
/// manifest - because a version-2 payload announced as version 1 is refused for the mismatch
/// before the version-2 pass reads a section, which would test the wrong thing.
/// </para>
/// </remarks>
internal static class WideCorpus
{
    /// <summary>The replay mode a version-2 entry is presented under.</summary>
    internal const string Mode = "wide";

    /// <summary>Every version-2 entry, in the order the registry publishes their codes.</summary>
    internal static CorpusEntry[] Build() =>
    [
        Entry(
            "wide-a-function-row-the-format-cannot-represent",
            Artifact(parameterCount: 2, scopeSlots: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedFunctionRow),
        Entry(
            "wide-an-entry-point-naming-no-code-unit",
            Artifact(entryUnit: 3),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.FunctionIndexOutOfRange),
        Entry(
            "wide-code-units-that-do-not-tile-the-code-section",
            Artifact(codeOffset: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.CodeUnitRangeInvalid),
        Entry(
            "wide-a-scope-popped-past-the-frame",
            Artifact(code: [(byte)JsOpcode.PopScope, (byte)JsOpcode.ReturnUndefined]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ScopeDepthOutOfRange),
        Entry(
            "wide-an-exception-handler-outside-its-code-unit",
            Artifact(regionHandlerOutsideUnit: true),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedExceptionRegion),

        // ---- two rows that were unreachable while one version was registered ------------------
        //
        // Both are the CALLER mislabelling the bytes, and neither could happen while the profile
        // registered one format version and accepted one manifest: the core screens the descriptor
        // before the profile is called, so anything that got through named the only answer there
        // was. Registering a second of each is what makes them observable, and an entry is what
        // stops the registry carrying two rows whose justification has quietly expired.
        Entry(
            "wide-a-version-1-artifact-announced-as-version-2",
            Broiler.VM.Profile.JavaScript.Compiler.SliceLowering.Constant(1),
            "DescriptorMismatch",
            JavaScriptDiagnosticCodes.DescriptorFormatVersionMismatch),
        Entry(
            "wide-a-slice-manifest-announced-as-the-wide-one",
            Artifact(manifest: "broiler.javascript.slice"),
            "DescriptorMismatch",
            JavaScriptDiagnosticCodes.DescriptorManifestMismatch),

        // ---- and one that verifies, instantiates and runs --------------------------------------
        //
        // Roadmap section 7's corpus discipline makes a corpus in which nothing verifies a release
        // blocker, and it is right: five malformed entries are satisfied by a verifier that refuses
        // everything of this format version. This one is a whole version-2 program - a closure
        // called through a property of an object - and its completion value is what a verifier that
        // refused the format could never produce.
        Ok(
            "wide-a-method-called-through-a-property",
            WideProgram(),
            "7"),
    ];

    /// <summary>A version-2 program that returns a Number, compiled from source.</summary>
    /// <remarks>
    /// Compiled rather than hand-assembled, because the point of the entry is that the whole path -
    /// tokenizer, parser, lowering, verifier, executor - answers with the value the language says.
    /// A hand-assembled artifact would prove the last two and say nothing about the first three.
    /// </remarks>
    private static byte[] WideProgram()
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            "function Point(x) { this.x = x; }\n" +
            "Point.prototype.twice = function () { return this.x * 2; };\n" +
            "new Point(3).twice() + 1;\n",
            Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained version-2 control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>A well-formed version-2 entry with a recorded completion value.</summary>
    private static CorpusEntry Ok(string name, byte[] bytes, string completion) =>
        new(name, Mode, "Normal", "NormalCompleted", 0, completion, "-", "-", "-", bytes);

    /// <summary>One malformed version-2 entry.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes, string reason, int code) =>
        new(name, Mode, "InvalidArtifact", reason, code, "-", "-", "-", "-", bytes);

    /// <summary>
    /// A version-2 artifact that is well formed except where a parameter says otherwise.
    /// </summary>
    /// <remarks>
    /// One builder with one deviation per call, so that what an entry is about is the argument it
    /// passes and everything else is the same artifact. A separate hand-written byte string per
    /// entry would leave a reader comparing five things to find the one that differs.
    /// </remarks>
    private static byte[] Artifact(
        uint parameterCount = 0,
        uint scopeSlots = 1,
        uint entryUnit = 0,
        uint codeOffset = 0,
        byte[]? code = null,
        bool regionHandlerOutsideUnit = false,
        string? manifest = null)
    {
        var body = code ?? [(byte)JsOpcode.LoadConstant, 0x00, 0x00, (byte)JsOpcode.Return];

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants([JsArtifactWriter.NumberConstant(1)])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, body),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", entryUnit)])),
        };

        if (regionHandlerOutsideUnit)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ExceptionRegions,
                JsArtifactWriter.ExceptionRegions(
                [
                    new JsExceptionRegionRow(
                        0, 0, (uint)body.Length, (uint)body.Length + 8, 0, 0,
                        JsFormat.HandlerKind.Catch),
                ])));
        }

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
            JsArtifactWriter.Positions([(0, 1, 1)])));

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
            JsArtifactWriter.Functions(
            [
                new JsFunctionRow(
                    0,
                    parameterCount,
                    scopeSlots,
                    16,
                    codeOffset,
                    (uint)body.Length,
                    (uint)JsFormat.FunctionFlags.ProgramBody),
            ])));

        return JsArtifactWriter.Write(manifest ?? JsFormat.ManifestId, sections.ToArray());
    }
}
