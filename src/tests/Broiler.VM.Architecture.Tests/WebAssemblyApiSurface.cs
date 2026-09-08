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
/// and no lowering - its payload is a bare WebAssembly module produced by an external toolchain, so
/// there is no bytecode two projects have to agree on without depending on each other. The list is
/// therefore short, and its being short is a fact about the family rather than about how much of it
/// has been written.
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
