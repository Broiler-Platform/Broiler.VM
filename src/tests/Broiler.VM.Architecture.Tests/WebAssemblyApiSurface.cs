using System.Reflection;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The complete public surface of the WebAssembly profile family's one assembly, as text.
/// </summary>
/// <remarks>
/// <para>
/// <b>A second family, a second list, a second baseline.</b> <see cref="ProfileApiSurface"/>'s own
/// remark says a second profile family adds its own list rather than widening that one, because
/// two families' surfaces are two published artefacts and a rule holding both to one file would be
/// freezing a surface whose two halves can move independently. This class is that second list, and
/// it is deliberately not a generalisation of the first: a set enumerated from whatever is on disk
/// silently starts and stops covering things.
/// </para>
/// <para>
/// <b>One assembly, because this family has one.</b> The WebAssembly profile has no format sibling
/// of its own: the bytecode it is translated into is the universal bytecode, whose assembly the
/// profile references and which is frozen by its own baseline. The list is therefore short, and its
/// being short is a fact about the family rather than about how much of it has been written.
/// </para>
/// <para>
/// The describer and the loader are the ones <see cref="ProfileApiSurface"/> uses and for the same
/// reasons: <see cref="ApiSurface"/> writes every line, so the three baselines are one format; and
/// a <see cref="MetadataLoadContext"/> reflects without running anything, which is what makes the
/// rule legal when rule A11 forbids the project reference that would let this be
/// <c>Assembly.Load</c> and loading would run the module initializers invariant 2 forbids.
/// </para>
/// </remarks>
internal static class WebAssemblyApiSurface
{
    /// <summary>The WebAssembly profile family's assemblies.</summary>
    internal static readonly string[] FamilyAssemblies =
    [
        "Broiler.VM.Profile.WebAssembly",
    ];

    /// <summary>
    /// The assemblies the family references that this test project does not, resolved from the
    /// profile's own build output: only the universal bytecode.
    /// </summary>
    internal static readonly string[] ProfileOnlyReferences =
    [
        "Broiler.VM.Ubc",
    ];

    /// <summary>Describes the public surface of the family's assemblies, sorted.</summary>
    internal static IReadOnlyList<string> Describe()
    {
        var files = FamilyAssemblies
            .Select(AssemblyPath)
            .Where(File.Exists)
            .ToArray();

        if (files.Length == 0)
        {
            return [];
        }

        var resolverPaths = new List<string>(files);
        resolverPaths.AddRange(Directory.EnumerateFiles(
            Path.GetDirectoryName(typeof(object).Assembly.Location)!, "*.dll"));
        resolverPaths.AddRange(Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"));

        // The profile's own build output holds the assemblies it references that this test does not:
        // Broiler.VM.Ubc, which its family's public types derive from and implement, and which the
        // describer reads through their base types and interfaces. Named one by one rather than the
        // whole directory, so a copy of an assembly this test already resolves is not offered twice.
        foreach (var file in files)
        {
            foreach (var reference in ProfileOnlyReferences)
            {
                var path = Path.Combine(Path.GetDirectoryName(file)!, reference + ".dll");

                if (File.Exists(path))
                {
                    resolverPaths.Add(path);
                }
            }
        }

        using var context = new MetadataLoadContext(
            new PathAssemblyResolver(resolverPaths.Distinct(StringComparer.OrdinalIgnoreCase)));

        var lines = new List<string>();

        foreach (var file in files)
        {
            var assembly = context.LoadFromAssemblyPath(file);
            lines.AddRange(ApiSurface.Describe(assembly));
        }

        lines.Sort(StringComparer.Ordinal);
        return lines;
    }

    /// <summary>Which of the family's assemblies were found on disk, in the order named.</summary>
    internal static IReadOnlyList<string> Found() => FamilyAssemblies
        .Where(static name => File.Exists(AssemblyPath(name)))
        .ToArray();

    /// <summary>
    /// Where a family assembly's build output sits.
    /// </summary>
    /// <remarks>
    /// The configuration and framework are read off this test assembly's own output path, exactly
    /// as <see cref="ProfileApiSurface"/> reads them, so the rule describes the build that just
    /// happened rather than a Release build that might be from last week.
    /// </remarks>
    private static string AssemblyPath(string assemblyName) => Path.Combine(
        ComponentGraph.Root, "src", assemblyName, "bin",
        ProfileApiSurface.Configuration, ProfileApiSurface.TargetFramework,
        assemblyName + ".dll");
}
