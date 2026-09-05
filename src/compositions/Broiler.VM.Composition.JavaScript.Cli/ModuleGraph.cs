// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// This composition's module resolution: a specifier is a path relative to the file that wrote it.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every line of this file is a decision this composition takes and the profile does not.</b>
/// That a specifier is a filesystem path, that it is resolved against the importing file's
/// directory, that a bare name is refused, that a module is read as UTF-8 with a byte-order mark
/// stripped - none of it is in the profile, and a sibling root embedding the same profile in a
/// browser or over a bundle would answer all four differently. The profile asks a source what it
/// requests and asks this root whether a resolution is its own; it opens nothing.
/// </para>
/// <para>
/// <b>A bare specifier is refused rather than searched for.</b> Resolving <c>"lodash"</c> means a
/// package layout, a search path and a manifest format, and this host has none of the three; a
/// search that quietly found nothing would report a missing file, which names the symptom rather
/// than the decision.
/// </para>
/// </remarks>
internal static class ModuleGraph
{
    /// <summary>What loading a graph produced.</summary>
    /// <param name="Modules">Every module reached, the root first.</param>
    /// <param name="Failure">Why the graph could not be loaded, empty when it was.</param>
    internal sealed record Loaded(IReadOnlyList<JsModuleUnit> Modules, string Failure);

    /// <summary>The most modules this host will follow before it declines to follow more.</summary>
    /// <remarks>
    /// A graph is a person's own tree of files and not an adversary's, but a symbolic-link loop is
    /// not a hostile artifact either - it is a mistake, and one a host should name rather than walk.
    /// The cycle a module graph legitimately has is handled by the visited set below; this is only
    /// the bound on how large a correct graph may be.
    /// </remarks>
    private const int MaximumModules = 1024;

    /// <summary>Loads the module rooted at one file, following what each module requests.</summary>
    internal static Loaded Load(string rootPath)
    {
        var root = Key(rootPath);
        var modules = new List<JsModuleUnit>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Queue<string>();

        pending.Enqueue(root);
        seen.Add(root);

        while (pending.Count != 0)
        {
            var key = pending.Dequeue();

            if (modules.Count == MaximumModules)
            {
                return new Loaded(
                    [],
                    $"the module graph rooted at `{rootPath}` reaches more than {MaximumModules} " +
                    "files, which this host declines to follow");
            }

            var file = SourceFiles.Read(key);

            if (file.Unreadable.Length != 0)
            {
                return new Loaded([], $"`{key}`: {file.Unreadable}");
            }

            var requests = JsCompiler.Requests(file.Text, SliceParseOptions.Module);

            if (!requests.Succeeded)
            {
                // THE REFUSAL IS THE COMPILER'S AND IS REPORTED BY THE COMPILER. Answering here
                // would report a graph failure for a source error, and the caller would then have
                // two ways to hear about one refusal that read differently.
                modules.Add(new JsModuleUnit(key, file.Text, SliceParseOptions.Module));
                continue;
            }

            var resolutions = new List<JsResolvedRequest>(requests.Specifiers.Count);

            foreach (var specifier in requests.Specifiers)
            {
                if (!TryResolve(key, specifier, out var resolved))
                {
                    return new Loaded(
                        [],
                        $"`{key}` requests `{specifier}`, and this host resolves only a specifier " +
                        "that begins with `./`, `../` or `/`");
                }

                if (!File.Exists(resolved))
                {
                    return new Loaded([], $"`{key}` requests `{specifier}`, and there is no file at `{resolved}`");
                }

                resolutions.Add(new JsResolvedRequest(specifier, resolved));

                if (seen.Add(resolved))
                {
                    pending.Enqueue(resolved);
                }
            }

            modules.Add(new JsModuleUnit(key, file.Text, SliceParseOptions.Module, resolutions));
        }

        return new Loaded(modules, string.Empty);
    }

    /// <summary>
    /// Answers one resolution request the profile put to this composition.
    /// </summary>
    /// <remarks>
    /// The request is the referring module's key, the specifier as the source wrote it, and the key
    /// the artifact says it resolves to, separated by NULs. This host answers yes only when its own
    /// resolution of the first two is the third, so an artifact bundled under another host's rules
    /// is refused here rather than run.
    /// </remarks>
    internal static bool Confirms(ReadOnlySpan<byte> request)
    {
        var text = System.Text.Encoding.UTF8.GetString(request);
        var parts = text.Split('\0');

        if (parts.Length != 3)
        {
            return false;
        }

        return TryResolve(parts[0], parts[1], out var resolved) &&
            string.Equals(resolved, parts[2], StringComparison.Ordinal);
    }

    /// <summary>This host's rule: relative to the importing file, and nothing else.</summary>
    private static bool TryResolve(string referrer, string specifier, out string resolved)
    {
        resolved = string.Empty;

        if (!specifier.StartsWith("./", StringComparison.Ordinal) &&
            !specifier.StartsWith("../", StringComparison.Ordinal) &&
            !specifier.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        var directory = Path.GetDirectoryName(referrer);

        resolved = Key(
            specifier.StartsWith("/", StringComparison.Ordinal)
                ? specifier
                : Path.Combine(directory is null or "" ? "." : directory, specifier));

        return true;
    }

    /// <summary>
    /// The canonical key of one file: its full path, with one separator whatever the platform uses.
    /// </summary>
    /// <remarks>
    /// <b>Two specifiers naming one file must produce one key or the file becomes two modules</b> -
    /// with two environments, two evaluations and two copies of every binding it exports, which is
    /// the defect a module map exists to prevent. Full-path normalisation is what makes
    /// <c>./a.mjs</c> from one directory and <c>../x/a.mjs</c> from another the same module.
    /// </remarks>
    private static string Key(string path) => Path.GetFullPath(path).Replace('\\', '/');
}
