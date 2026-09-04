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
/// <b>One entry per code, and the count is the point.</b> Version 2 adds a function table, an
/// environment model, exception regions, the optional surfaces and the one unit kind that may
/// suspend; each of those is a place where an artifact can be structurally wrong in a way version 1
/// has no vocabulary for, and the published registry binds each new code to a named entry here. A
/// verifier that refused every version-2 artifact would satisfy none of them, because they are
/// distinguished by the code and not by the refusal.
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

    /// <summary>
    /// The replay mode for a composition that admits no optional surface.
    /// </summary>
    /// <remarks>
    /// <b>It is a different HOST, not different bytes.</b> The entry that carries it is a
    /// well-formed artifact declaring the binary surface, and the only reason it is refused is that
    /// the composition replaying it registered a descriptor admitting none. That is the property
    /// roadmap section 6 describes and which no entry recorded until now: a composition declining a
    /// manifest refuses the artifact at verification, with an invalid-artifact reason, rather than
    /// letting it run and answering a run-time error the guest could catch.
    /// </remarks>
    internal const string DecliningMode = "wide-declining";

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

        // ---- three rows about the optional surfaces an artifact may declare --------------------
        //
        // A surface made of globals cannot be refused by refusing a construct, because reading
        // `Uint8Array` is byte for byte reading a name. The artifact declares what it reaches, and
        // these are the three ways that declaration can be wrong: said twice, naming something
        // nobody wrote, and naming something this composition declined. The last is the only one
        // whose answer depends on the host, and it is the one the manifest boundary is FOR.
        Entry(
            "wide-a-surface-declared-twice",
            Artifact(surfaces: [JsSurfaces.Binary, JsSurfaces.Binary]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.DuplicateSurface),
        Entry(
            "wide-a-surface-this-build-does-not-implement",
            Artifact(surfaces: ["broiler.javascript.telepathy"]),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownSurface),
        new CorpusEntry(
            "wide-a-surface-the-composition-declined",
            DecliningMode,
            "InvalidArtifact",
            "UnsupportedFeatureManifest",
            JavaScriptDiagnosticCodes.SurfaceOutsideComposition,
            "-",
            "-",
            "-",
            "-",
            Artifact(surfaces: [JsSurfaces.Binary])),

        // ---- two rows about the one unit kind that may suspend ---------------------------------
        //
        // A generator's frame is put on the heap by the EXECUTOR, from the unit's own flag, before
        // any of its code runs. Both of these are ways an artifact can ask for a suspension the
        // executor has not allocated a frame for, and both are refused by the verifier rather than
        // met by a null frame in the middle of the dispatch loop: one puts the suspension in a unit
        // that is not a generator, the other declares a unit that is a generator AND one of the
        // three things a generator cannot also be.
        Entry(
            "wide-a-suspension-outside-a-generator",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.Yield,
                (byte)JsOpcode.Return,
            ]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.YieldOutsideGenerator),
        Entry(
            "wide-a-generator-that-is-also-the-program-body",
            Artifact(
                flags: JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Generator),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.GeneratorFlagsInconsistent),

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
        string? manifest = null,
        string[]? surfaces = null,
        JsFormat.FunctionFlags flags = JsFormat.FunctionFlags.ProgramBody)
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
                    (uint)flags),
            ])));

        if (surfaces is not null)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces(surfaces)));
        }

        return JsArtifactWriter.Write(manifest ?? JsFormat.ManifestId, sections.ToArray());
    }
}
