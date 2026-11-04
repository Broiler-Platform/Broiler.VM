// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// A refused source of the retained source corpus that one text under one goal cannot carry: it is
/// refused only when it is compiled under a request, or as more than one unit.
/// </summary>
/// <remarks>
/// <para>
/// <b>Written 2026-11-04, at SJL-13's second tools commit, on the owner's decision of that day on
/// registry revision 19</b> (JSD-0083's dated note on decision D12). Revision 19 gives a new code to
/// each meaning a split row of the front end's mapping leaves, and check E13 of
/// <c>eng/check-frontend-mapping.py</c> asks each new code to be reached by a refused entry of this
/// corpus. Three of those meanings are reached only by something a single text cannot carry: two
/// module units under one key (the catalogue's EX0020), a compile request the engine cannot serve
/// (EX0030), and eval code under its caller's flags (EX0013). Every other refused source is one text
/// under one goal, in <see cref="SliceSourcePrograms"/>; this is the form for the ones that are not.
/// </para>
/// <para>
/// <b>Here, in the composition, and not beside the other lists in the lowering assembly</b>, because
/// the tools commit that adds it changes no file the compiler is built from. The type is the corpus
/// format and nothing else: <see cref="Compile"/> calls the public entry point every host calls,
/// with the units and the request it holds.
/// </para>
/// <para>
/// <b>Retained as files, as every other refused source is.</b> Each unit is a file beside the manifest,
/// named for the entry and its place, and a <c>.request</c> file names the request and every unit with
/// its options, its file and that file's SHA-256. The manifest's row names the request file's SHA-256
/// and marks the entry <c>request</c>, so a reader who opens the manifest finds everything the compiler
/// was given. A unit fact the request file cannot record - a referrer, a source name, a module's
/// resolved requests - is refused when the corpus is written rather than dropped.
/// </para>
/// </remarks>
internal sealed record RefusedRequest(string Name, SliceSourceDiagnosticCode Code)
{
    /// <summary>The scripts, compiled first, in order.</summary>
    internal IReadOnlyList<JsScriptUnit> Scripts { get; init; } = [];

    /// <summary>The modules, compiled after the scripts, in order; the first is the root.</summary>
    internal IReadOnlyList<JsModuleUnit> Modules { get; init; } = [];

    /// <summary>The request, the wide manifest and the bytecode form unless stated.</summary>
    internal JsCompileRequest Request { get; init; } = new();

    /// <summary>Compiles the entry as a host would: every unit, under the request.</summary>
    internal JsCompilation Compile() => JsCompiler.Compile(Scripts, Modules, Request);

    /// <summary>
    /// Compiles the entry's first unit alone under the default request: what a single text under one
    /// goal would be, which the row holds must not reach the entry's code.
    /// </summary>
    internal JsCompilation CompileFirstUnitAlone() => Scripts.Count != 0
        ? JsCompiler.Compile([Scripts[0]])
        : JsCompiler.Compile([], [Modules[0]]);

    /// <summary>
    /// Compiles the entry's first unit alone under the entry's own manifest, in the default form and
    /// with no backend: which the row holds must not reach the entry's code either, so that an entry
    /// whose request differs in manifest and form is a case of the form it asks for and not of its
    /// manifest alone (added 2026-11-04, in a later commit of the change that added this type).
    /// </summary>
    internal JsCompilation CompileFirstUnitUnderItsManifest()
    {
        var request = new JsCompileRequest(Request.Manifest);

        return Scripts.Count != 0
            ? JsCompiler.Compile([Scripts[0]], [], request)
            : JsCompiler.Compile([], [Modules[0]], request);
    }

    /// <summary>
    /// The files the entry is retained as: each unit's text under its file name, and the request file
    /// last, whose text names every other file and its SHA-256.
    /// </summary>
    internal IReadOnlyList<(string File, string Text)> Files(Func<string, string> normalise)
    {
        var files = new List<(string File, string Text)>();
        var request = new StringBuilder();

        request.Append("# A refused source of the retained source corpus that one text under one goal cannot carry\n");
        request.Append("# (src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/RefusedRequests.cs).\n");
        request.Append("# source.manifest names this file's SHA-256; each unit names its file, beside this one, and that file's SHA-256.\n");
        request.Append("# Units compile in the order listed, scripts before modules, the first module the root.\n");
        request.Append("answer|").Append(Code).Append('\n');
        request.Append("request|manifest=").Append(Request.Manifest)
            .Append("|form=").Append(Request.Form)
            .Append("|backend=").Append(Request.Backend.Length == 0 ? "-" : Request.Backend)
            .Append("|source-text=").Append(Request.KeepsSourceText ? "kept" : "dropped")
            .Append('\n');

        var index = 0;

        foreach (var script in Scripts)
        {
            if (script.Referrer.Length != 0 || script.SourceName.Length != 0)
            {
                throw new InvalidOperationException(
                    $"{Name}: a script's referrer or source name is a fact the retained form does not record");
            }

            var file = $"{Name}.{++index}.js";
            var text = normalise(script.Text);
            files.Add((file, text));
            request.Append("unit|script|").Append(script.Name).Append('|')
                .Append(Options(script.Options)).Append(script.ForceStrict ? ",strict" : string.Empty)
                .Append('|').Append(file).Append('|').Append(Sha256(text)).Append('\n');
        }

        foreach (var module in Modules)
        {
            if (module.Requests is not null)
            {
                throw new InvalidOperationException(
                    $"{Name}: a module's resolved requests are a fact the retained form does not record");
            }

            var file = $"{Name}.{++index}." + (module.Type.Length != 0 ? module.Type : "mjs");
            var text = normalise(module.Text);
            files.Add((file, text));
            request.Append("unit|module|").Append(module.Key).Append('|')
                .Append(Options(module.Options))
                .Append(module.Type.Length != 0 ? ",type=" + module.Type : string.Empty)
                .Append('|').Append(file).Append('|').Append(Sha256(text)).Append('\n');
        }

        files.Add((Name + ".request", request.ToString()));
        return files;
    }

    /// <summary>
    /// The manifest's row for the entry, as the corpus producer writes it: its name, its request
    /// file's SHA-256, the code it is answered with and the kind <c>request</c>.
    /// </summary>
    internal static string ManifestRow(string name, string requestText, SliceSourceDiagnosticCode code) =>
        $"refused|{name}|{Sha256(requestText)}|{code}|request";

    /// <summary>The SHA-256 of a retained text, as the manifest records it.</summary>
    internal static string Sha256(string text) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    private static string Options(SliceParseOptions options) =>
        "goal=" + (options.Goal == SliceGoal.Module ? "module" : "script") +
        ",top-level-await=" + (options.AllowTopLevelAwait ? "yes" : "no") +
        ",nesting=" + options.MaximumNestingDepth.ToString(CultureInfo.InvariantCulture) +
        (options.IsEval ? ",eval=" + options.EvalFlags.ToString().Replace(", ", "+", StringComparison.Ordinal) : string.Empty);
}

/// <summary>
/// The retained source corpus's refused requests: sources refused only under a request or as more than
/// one unit, each with the code it must be refused with.
/// </summary>
/// <remarks>
/// <b>Each is answered with the code the base gives it, and step 1's registry commit renumbers it.</b>
/// At registry revision 18 two modules under one key are refused with 2201 and the value form under
/// the numeric manifest with 2104, codes the front end's mapping marks as split for those meanings
/// (EX0020 and EX0030). Revision 19 mints their codes, and the registry commit changes the code each
/// entry here is answered with, which is all it needs to give each new code a case. An entry for EX0013
/// is not here: every eval unit is parsed within the default nesting bound, and no source found so far
/// crosses the scope-depth ceiling inside it (JSD-0083's note of 2026-11-04 on decision D12).
/// </remarks>
internal static class RefusedRequests
{
    internal static RefusedRequest[] All =>
    [
        // TWO MODULES UNDER ONE KEY. Each text is an ordinary module; what is refused is the
        // composition, which presents a second module under a key an earlier unit holds.
        new("refuse-two-modules-under-one-key", SliceSourceDiagnosticCode.DuplicateLexicalDeclaration)
        {
            Modules =
            [
                new JsModuleUnit("/main.mjs", "export const first = 1;", SliceParseOptions.Module),
                new JsModuleUnit("/main.mjs", "export const second = 2;", SliceParseOptions.Module),
            ],
        },

        // THE VALUE FORM UNDER THE NUMERIC MANIFEST. The source is the numeric manifest's own; what is
        // refused is the request, which asks for an output form only the wide manifest defines.
        new("refuse-the-value-form-under-the-numeric-manifest", SliceSourceDiagnosticCode.ConstructOutsideManifest)
        {
            Scripts = [new JsScriptUnit("main", "1 + 2", SliceParseOptions.Script)],
            Request = new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Value),
        },
    ];
}
