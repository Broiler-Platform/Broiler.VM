using System.Reflection;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The complete public surface of the JavaScript profile family's three assemblies, as text.
/// </summary>
/// <remarks>
/// <para>
/// <b>The same describer, a different loader.</b> Every line here is produced by
/// <see cref="ApiSurface"/>'s own type and member describers, so the two baselines are one format
/// and a reader who can read one can read the other. What differs is where the assemblies come
/// from, and that difference is the whole reason this file exists.
/// </para>
/// <para>
/// <b>Why not <c>Assembly.Load</c>.</b> The packable three are in this project's output because it
/// references them. The profile's three are not, and may not be: rule A11 forbids a test project
/// to reference a profile assembly, and that prohibition is one of the properties this component
/// exists to demonstrate. Bundle JS-1-001 recorded the resulting gap as an open gate clause and
/// named two routes out; this is the first of them.
/// </para>
/// <para>
/// <b>Why not <c>Assembly.LoadFrom</c> over the built file.</b> Loading runs module initializers.
/// Invariant 2 forbids them and rule B5b exists to detect them, so a describer built on loading
/// would execute the code it is describing and would pass or fail partly on what that execution
/// did. <see cref="MetadataLoadContext"/> reflects without running anything, which is the same
/// reason <see cref="AssemblyFacts"/> reads metadata tables directly for the group B rules.
/// </para>
/// <para>
/// <b>The stated limit: this reads a build output, not a package.</b> The packable baseline
/// describes assemblies the test host has loaded; this one describes files on disk under the
/// profile projects' <c>bin</c> directories, so it describes the last build in this
/// configuration. A run that has not built the profile describes nothing, and the rule fails on
/// the empty surface rather than passing over it. Decision JSD-0012 records the limit.
/// </para>
/// </remarks>
internal static class ProfileApiSurface
{
    /// <summary>The JavaScript profile family's three assemblies, in dependency order.</summary>
    /// <remarks>
    /// Named rather than discovered, for the reason <see cref="ApiSurface.PackableAssemblies"/>
    /// gives: a set enumerated from whatever is on disk silently starts and stops covering things.
    /// A second profile family adds its own list and its own baseline rather than widening this
    /// one, because two families' surfaces are two published artefacts.
    /// </remarks>
    internal static readonly string[] FamilyAssemblies =
    [
        "Broiler.VM.Profile.JavaScript.Format",
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
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

        // The resolver needs every assembly the loaded ones name, which is the framework plus the
        // component's own. The framework comes from this test host's own runtime directory, which
        // is the same shared framework the profile was built against - the component targets one
        // framework version and rule B1 keeps the set that small.
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

    /// <summary>
    /// Loads one family type through the same context the describer uses, for a rule that needs to
    /// show the context executes nothing.
    /// </summary>
    /// <remarks>
    /// The context is deliberately NOT disposed and the type outlives it, because the property
    /// under test - that a metadata-only type has no runtime handle - is a property of the type
    /// and asking for it is what a caller would do. Nothing here invokes anything.
    /// </remarks>
    internal static Type LoadForInspection(string assemblyName, string typeName)
    {
        var resolverPaths = new List<string>(
            FamilyAssemblies.Select(AssemblyPath).Where(File.Exists));

        resolverPaths.AddRange(Directory.EnumerateFiles(
            Path.GetDirectoryName(typeof(object).Assembly.Location)!, "*.dll"));
        resolverPaths.AddRange(Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"));

        var context = new MetadataLoadContext(
            new PathAssemblyResolver(resolverPaths.Distinct(StringComparer.OrdinalIgnoreCase)));

        return context.LoadFromAssemblyPath(AssemblyPath(assemblyName)).GetType(typeName, throwOnError: true)!;
    }

    /// <summary>Which of the family's assemblies were found on disk, in the order named.</summary>
    internal static IReadOnlyList<string> Found() => FamilyAssemblies
        .Where(static name => File.Exists(AssemblyPath(name)))
        .ToArray();

    /// <summary>
    /// Where a family assembly's build output sits.
    /// </summary>
    /// <remarks>
    /// The configuration is taken from this test assembly's own output path rather than assumed to
    /// be Release. A run under Debug would otherwise describe a Release build that might be from
    /// last week, and the rule would compare a baseline against a file nobody had just produced.
    /// </remarks>
    private static string AssemblyPath(string assemblyName) => Path.Combine(
        ComponentGraph.Root, "src", assemblyName, "bin", Configuration, TargetFramework,
        assemblyName + ".dll");

    /// <summary>The configuration this test run was built in.</summary>
    internal static string Configuration { get; } = Segment(1);

    /// <summary>The target framework this test run was built for.</summary>
    internal static string TargetFramework { get; } = Segment(0);

    /// <summary>
    /// A trailing segment of this assembly's output directory, counted back from the end.
    /// </summary>
    /// <remarks>
    /// <c>…/bin/Release/net10.0/</c>: segment 0 is the framework and segment 1 the configuration.
    /// Reading them off the path is what makes this rule describe the build that just happened.
    /// </remarks>
    private static string Segment(int fromEnd)
    {
        var segments = AppContext.BaseDirectory
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Where(static segment => segment.Length > 0)
            .ToArray();

        return fromEnd < segments.Length ? segments[^(fromEnd + 1)] : string.Empty;
    }
}
