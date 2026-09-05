// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// This harness's module resolution: a specifier is a path relative to the file that wrote it.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the harness's own and not the profile's, and it is deliberately a second
/// implementation.</b> The command-line host resolves specifiers too, and sharing one implementation
/// between the two roots would make a resolver a thing this repository has rather than a thing each
/// composition decides. The suite's own convention is what this one follows: a module test's
/// dependencies are <c>_FIXTURE.js</c> files beside it, named by a relative specifier.
/// </para>
/// <para>
/// <b>A specifier that resolves to nothing is not an error of this harness.</b> A whole family of
/// the suite's module tests expects exactly that, under a <c>resolution</c>-phase negative
/// expectation, so the failure is reported to the caller and scored rather than thrown.
/// </para>
/// </remarks>
internal static class Test262Modules
{
    /// <summary>What loading a module graph produced.</summary>
    /// <param name="Modules">Every module reached, the root first.</param>
    /// <param name="Failure">Why it could not be loaded, empty when it was.</param>
    internal sealed record Graph(IReadOnlyList<JsModuleUnit> Modules, string Failure);

    /// <summary>The most modules one test's graph may reach.</summary>
    private const int MaximumModules = 256;

    /// <summary>Loads the graph rooted at one test file.</summary>
    internal static Graph Load(string rootPath, string rootText)
    {
        var root = Key(rootPath);
        var modules = new List<JsModuleUnit>();
        var texts = new Dictionary<string, string>(StringComparer.Ordinal) { [root] = rootText };
        var seen = new HashSet<string>(StringComparer.Ordinal) { root };
        var pending = new Queue<string>();
        pending.Enqueue(root);

        while (pending.Count != 0)
        {
            var key = pending.Dequeue();

            if (modules.Count == MaximumModules)
            {
                return new Graph([], "the module graph reaches more than 256 files");
            }

            if (!texts.TryGetValue(key, out var text))
            {
                if (!File.Exists(key))
                {
                    return new Graph([], "no module at " + key);
                }

                text = ReadUtf8(key);
                texts[key] = text;
            }

            var requests = JsCompiler.Requests(text, SliceParseOptions.Module);

            if (!requests.Succeeded)
            {
                modules.Add(new JsModuleUnit(key, text, SliceParseOptions.Module));
                continue;
            }

            var resolutions = new List<JsResolvedRequest>(requests.Specifiers.Count);

            foreach (var specifier in requests.Specifiers)
            {
                var resolved = Resolve(key, specifier);

                // A SPECIFIER THAT NAMES NOTHING IS CARRIED INTO THE ARTIFACT AND REFUSED THERE.
                // Failing here would put resolution before parsing, and a module with both an early
                // error and an unresolvable specifier would then be reported for the second - which
                // is not the order the language settles them in, and is exactly what a family of
                // the suite's tests asks about.
                if (resolved.Length == 0 || !File.Exists(resolved))
                {
                    resolutions.Add(new JsResolvedRequest(specifier, specifier));
                    continue;
                }

                resolutions.Add(new JsResolvedRequest(specifier, resolved));

                if (seen.Add(resolved))
                {
                    pending.Enqueue(resolved);
                }
            }

            modules.Add(new JsModuleUnit(key, text, SliceParseOptions.Module, resolutions));
        }

        return new Graph(modules, string.Empty);
    }

    /// <summary>Loads the graph rooted at what one specifier names from one referrer.</summary>
    /// <remarks>
    /// <b>This is what a dynamic <c>import()</c> asks this harness for.</b> The suite reaches it in
    /// a thousand variants, in scripts as well as in modules, and the specifier is normally a value
    /// the test computed rather than a literal any compiler saw - so this is resolution at run time,
    /// through exactly the rule the static walk above uses, and not a second reading of what a
    /// specifier is.
    /// </remarks>
    internal static Graph LoadFor(string referrer, string specifier)
    {
        var resolved = Resolve(referrer, specifier);

        if (resolved.Length == 0 || !File.Exists(resolved))
        {
            return new Graph([], "no module at " + specifier);
        }

        return Load(resolved, ReadUtf8(resolved));
    }

    /// <summary>Rules on one resolution request the profile put to this harness.</summary>
    internal static bool Confirms(ReadOnlySpan<byte> request)
    {
        var parts = JsFormat.DecodeText(request).Split('\0');

        return parts.Length == 3 &&
            string.Equals(Resolve(parts[0], parts[1]), parts[2], StringComparison.Ordinal);
    }

    private static string Resolve(string referrer, string specifier)
    {
        if (!specifier.StartsWith("./", StringComparison.Ordinal) &&
            !specifier.StartsWith("../", StringComparison.Ordinal) &&
            !specifier.StartsWith("/", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        var directory = Path.GetDirectoryName(referrer);

        return Key(
            specifier.StartsWith("/", StringComparison.Ordinal)
                ? specifier
                : Path.Combine(directory is null or "" ? "." : directory, specifier));
    }

    private static string Key(string path) => Path.GetFullPath(path).Replace('\\', '/');

    private static string ReadUtf8(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
        return System.Text.Encoding.UTF8.GetString(bytes, start, bytes.Length - start);
    }
}
