// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained entries of the module goal, one per refusal the module manifest adds.
/// </summary>
/// <remarks>
/// <para>
/// <b>Eight refusals and one control, and the count is what the published registry binds.</b> A
/// module artifact can be wrong in ways a script artifact has no vocabulary for - a request that
/// names no module, an import of a name nothing exports, two star re-exports supplying one name
/// from two bindings, a resolution that walks a cycle - and each of those is a code with a row in
/// the registry and an entry here. A verifier that refused every module artifact would satisfy
/// none of the eight, because they are distinguished by the code rather than by the refusal.
/// </para>
/// <para>
/// <b>Two of the nine are about the COMPOSITION rather than the bytes.</b> A composition that
/// registers no module resolver has declined the surface, and the artifact it declines is a
/// perfectly well-formed one - so what those rows vary is the host, which is why they carry a
/// replay mode of their own. That is the same shape the exhaustion rows use and for the same
/// reason: what a row is about has to be the thing the row varies.
/// </para>
/// </remarks>
internal static class ModuleCorpus
{
    /// <summary>The replay mode a module entry is presented under, with a resolver registered.</summary>
    internal const string Mode = "modules";

    /// <summary>
    /// The replay mode of a composition that registered no resolver and so declined the surface.
    /// </summary>
    internal const string DeclinedMode = "modules-declined";

    /// <summary>Every module entry, in the order the registry publishes their codes.</summary>
    internal static CorpusEntry[] Build() =>
    [
        // ITS MODE IS THE WIDE ONE AND THAT IS WHAT THE ROW IS ABOUT. The payload names the wide
        // manifest and carries module records, so the descriptor has to name the wide manifest
        // too - presenting it under the module one would be refused for the descriptor mismatch
        // first, and the row would then be about a caller's mistake rather than about a section a
        // manifest does not admit.
        new CorpusEntry(
            "modules-a-module-section-the-manifest-excludes",
            WideCorpus.Mode,
            "InvalidArtifact",
            "UnknownFeature",
            JavaScriptDiagnosticCodes.ModuleSectionOutsideManifest,
            "-",
            "-",
            "-",
            "-",
            Artifact(manifest: JsFormat.ManifestId)),
        Entry(
            "modules-a-module-row-the-format-cannot-represent",
            Artifact(bodyUnit: 7),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedModuleRow),
        Entry(
            "modules-a-request-naming-no-module",
            Artifact(requestKey: "./nowhere.mjs"),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ModuleRequestUnresolved),
        Entry(
            "modules-an-import-of-a-name-nothing-exports",
            Compiled(
                new Module(
                    "main",
                    "import { absent } from './other.mjs';\nabsent;\n",
                    ("./other.mjs", "other")),
                new Module("other", "export const present = 1;\n")),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ModuleExportNotFound),
        Entry(
            "modules-a-name-two-star-re-exports-supply",
            Compiled(
                new Module(
                    "main",
                    "import { shared } from './mid.mjs';\nshared;\n",
                    ("./mid.mjs", "mid")),
                new Module(
                    "mid",
                    "export * from './one.mjs';\nexport * from './two.mjs';\n",
                    ("./one.mjs", "one"),
                    ("./two.mjs", "two")),
                new Module("one", "export const shared = 1;\n"),
                new Module("two", "export const shared = 2;\n")),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ModuleExportAmbiguous),
        Entry(
            "modules-a-cyclic-export-resolution",
            Compiled(
                new Module(
                    "main", "export { spin } from './other.mjs';\n", ("./other.mjs", "other")),
                new Module(
                    "other", "export { spin } from './main.mjs';\n", ("./main.mjs", "main"))),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ModuleExportCircular),
        new CorpusEntry(
            "modules-a-module-artifact-a-composition-declined",
            DeclinedMode,
            "InvalidArtifact",
            "UnsupportedFeatureManifest",
            JavaScriptDiagnosticCodes.ModuleResolverAbsent,
            "-",
            "-",
            "-",
            "-",
            Compiled(new Module("main", "export const answer = 42;\nanswer;\n"))),
        Entry(
            "modules-the-module-manifest-with-no-module-records",
            Artifact(omitModuleSection: true),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ModuleSectionMissing),

        // ---- and one that verifies, links, evaluates and runs ---------------------------------
        //
        // Eight malformed entries are satisfied by a verifier that refuses every module artifact,
        // which is the outcome the roadmap's corpus discipline calls a release blocker. This one is
        // a two-module graph whose completion value is what a live binding produces: the importing
        // module reads the counter AFTER calling into the exporting module to change it, so a
        // verifier that refused the format could not produce the value, and an implementation that
        // copied the binding instead of reading through it would produce a different one.
        new CorpusEntry(
            "modules-a-live-binding-read-after-a-write",
            Mode,
            "Normal",
            "NormalCompleted",
            0,
            "3",
            "-",
            "-",
            "-",
            Compiled(
                new Module(
                    "main",
                    "import { counter, bump } from './lib.mjs';\nbump();\nbump();\ncounter + 1;\n",
                    ("./lib.mjs", "lib")),
                new Module(
                    "lib",
                    "export let counter = 0;\nexport function bump() { counter = counter + 1; }\n"))),
    ];

    /// <summary>One malformed module entry.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes, string reason, int code) =>
        new(name, Mode, "InvalidArtifact", reason, code, "-", "-", "-", "-", bytes);

    /// <summary>
    /// Compiles a module graph the way a composition would, resolving specifiers by a table.
    /// </summary>
    /// <remarks>
    /// The resolution is a table rather than a filesystem walk, which is the point of the seam:
    /// this producer is a composition too, and its rule is "a key is the name I gave it". A
    /// producer that had to open a file to write a corpus entry would have made the corpus depend
    /// on a directory layout.
    /// </remarks>
    private static byte[] Compiled(params Module[] rows)
    {
        var modules = new System.Collections.Generic.List<JsModuleUnit>(rows.Length);

        foreach (var row in rows)
        {
            var requests = new System.Collections.Generic.List<JsResolvedRequest>(
                row.Resolutions.Length);

            foreach (var (specifier, target) in row.Resolutions)
            {
                requests.Add(new JsResolvedRequest(specifier, target));
            }

            modules.Add(new JsModuleUnit(row.Key, row.Source, SliceParseOptions.Module, requests));
        }

        var compiled = JsCompiler.Compile([], modules);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "a retained module entry did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>One module of a retained graph: its key, its source, and what it resolves to.</summary>
    private sealed record Module(
        string Key, string Source, params (string Specifier, string Target)[] Resolutions);

    /// <summary>
    /// A hand-assembled module artifact that is well formed except where a parameter says otherwise.
    /// </summary>
    /// <remarks>
    /// Hand-assembled rather than compiled, because the deviations these entries need are ones the
    /// lowering cannot produce: a code unit index past the function table, a request whose key
    /// names no module, a manifest that does not admit the section it carries. A compiler that
    /// could emit any of the three would be the defect the verifier exists to catch.
    /// </remarks>
    private static byte[] Artifact(
        string? manifest = null,
        uint bodyUnit = 1,
        string requestKey = "",
        bool omitModuleSection = false)
    {
        var body = new byte[]
        {
            (byte)JsOpcode.LoadConstant, 0x00, 0x00,
            (byte)JsOpcode.InitialiseScoped, 0x00, 0x00, 0x00,
            (byte)JsOpcode.LoadScoped, 0x00, 0x00, 0x00,
            (byte)JsOpcode.Return,
        };

        var initialiser = new byte[] { (byte)JsOpcode.ReturnUndefined };
        var code = new byte[initialiser.Length + body.Length];
        initialiser.CopyTo(code, 0);
        body.CopyTo(code, initialiser.Length);

        var constants = new System.Collections.Generic.List<byte[]>
        {
            JsArtifactWriter.NumberConstant(7),
            JsArtifactWriter.InternedNameConstant("main"),
            JsArtifactWriter.InternedNameConstant(requestKey.Length == 0 ? "unused" : requestKey),
        };

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 8)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants(constants.ToArray())),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, code),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([(JsCompiler.ModuleEntry, 1u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
                JsArtifactWriter.Positions([(0, 1, 1)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
                JsArtifactWriter.Functions(
                [
                    new JsFunctionRow(
                        0, 0, 1, 16, 0, (uint)initialiser.Length,
                        (uint)(JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict)),
                    new JsFunctionRow(
                        0, 0, 1, 16, (uint)initialiser.Length, (uint)body.Length,
                        (uint)(JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict)),
                ])),
        };

        if (!omitModuleSection)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Modules,
                JsArtifactWriter.Modules(
                [
                    new JsModuleRow(
                        1,
                        bodyUnit,
                        0,
                        requestKey.Length == 0 ? [] : [2],
                        requestKey.Length == 0 ? [] : [2],
                        [],
                        [],
                        [],
                        []),
                ])));
        }

        return JsArtifactWriter.Write(
            manifest ?? JsFormat.ModulesManifestId, sections.ToArray());
    }
}
