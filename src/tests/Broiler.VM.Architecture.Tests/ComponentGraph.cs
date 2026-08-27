using System.Xml.Linq;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The component's real project graph, read off disk, so that the group A rules assert against
/// the checkout rather than against a description of it.
/// </summary>
/// <remarks>
/// <para>
/// Group A rules read project files and group B rules read compiled metadata, and the two are
/// not interchangeable. A rule about which edges the component DECLARES has to read project
/// files, because at milestone VM-0 the projects are shells: a shell that uses no type from a
/// referenced assembly emits no assembly reference into its metadata, so metadata cannot yet
/// witness a declared edge. A rule about which edges must NOT exist reads metadata as well,
/// because absence is provable whether or not the assembly has behaviour, and because a package
/// can reintroduce an edge that no project file spells out.
/// </para>
/// <para>ADR 0001 (0001-component-topology-and-dependency-graph.md) owns this distinction.</para>
/// </remarks>
internal static class ComponentGraph
{
    /// <summary>The component root: the directory holding Broiler.VM.slnx.</summary>
    internal static string Root { get; } = FindRoot();

    /// <summary>Every real project file in the component.</summary>
    internal static IReadOnlyList<ProjectFile> Projects { get; } = LoadProjects();

    /// <summary>
    /// The deliberately violating project files under witnesses/. They are named *.csproj.witness
    /// so MSBuild never globs them into the build, and they exist so that every group A rule can
    /// be shown to REJECT something. A rule that has never rejected anything is not evidence.
    /// </summary>
    internal static IReadOnlyList<ProjectFile> Witnesses { get; } = LoadWitnesses();

    internal static ProjectFile Witness(string fileName) =>
        Witnesses.SingleOrDefault(witness =>
            string.Equals(Path.GetFileName(witness.Path), fileName, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"No witness input named {fileName}.");

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.VM.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"No Broiler.VM.slnx above {AppContext.BaseDirectory}; the component root could not be located.");
    }

    private static IReadOnlyList<ProjectFile> LoadProjects() =>
        Directory
            .EnumerateFiles(Root, "*.csproj", SearchOption.AllDirectories)
            .Where(static path => !IsUnderBuildOutput(path))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(static path => Parse(path, isWitness: false))
            .ToArray();

    private static IReadOnlyList<ProjectFile> LoadWitnesses()
    {
        var directory = Path.Combine(
            Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses");

        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(directory, "*.csproj.witness", SearchOption.TopDirectoryOnly)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(static path => Parse(path, isWitness: true))
            .ToArray();
    }

    private static bool IsUnderBuildOutput(string path)
    {
        var segments = Path
            .GetRelativePath(Root, path)
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return segments.Any(static segment =>
            string.Equals(segment, "bin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(segment, "obj", StringComparison.OrdinalIgnoreCase));
    }

    private static ProjectFile Parse(string path, bool isWitness)
    {
        var document = XDocument.Load(path);
        var directory = Path.GetDirectoryName(path)!;

        // A witness is evaluated as though it sat where the project it imitates would sit, so
        // that "resolves outside the component root" means the same thing for both.
        var basis = isWitness
            ? Path.Combine(Root, "src", "Broiler.VM.Witness")
            : directory;

        return new ProjectFile(
            Path: path,
            RelativePath: Path.GetRelativePath(Root, path).Replace('\\', '/'),
            IsWitness: isWitness,
            AssemblyName: Property(document, "AssemblyName")
                ?? Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path)),
            RootNamespace: Property(document, "RootNamespace"),
            PackageId: Property(document, "PackageId"),
            OutputType: Property(document, "OutputType"),
            RawText: File.ReadAllText(path),
            ProjectReferences: Resolve(document, "ProjectReference", "Include", basis),
            PackageReferences: Includes(document, "PackageReference"),
            InternalsVisibleTo: Includes(document, "InternalsVisibleTo"),
            SourceItemPaths:
            [
                .. Resolve(document, "Compile", "Include", basis),
                .. Resolve(document, "None", "Include", basis),
                .. Resolve(document, "Content", "Include", basis),
                .. Resolve(document, "EmbeddedResource", "Include", basis),
                .. Resolve(document, "Import", "Project", basis),
            ]);
    }

    private static string[] Resolve(XDocument document, string element, string attribute, string basis) =>
        document
            .Descendants(element)
            .Select(node => node.Attribute(attribute)?.Value)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(value => SafeFullPath(basis, value!))
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static string SafeFullPath(string basis, string value)
    {
        // MSBuild properties and wildcards cannot be resolved to a real path here. They are
        // returned unchanged so a rule can still see the literal, which is what A3 needs: a
        // shared-source link is recognisable from its "..\..\" prefix alone.
        var normalized = value.Replace('\\', Path.DirectorySeparatorChar);

        try
        {
            return normalized.Contains('$')
                ? normalized
                : Path.GetFullPath(Path.Combine(basis, normalized));
        }
        catch (ArgumentException)
        {
            return normalized;
        }
    }

    private static string[] Includes(XDocument document, string element) =>
        document
            .Descendants(element)
            .Select(node => node.Attribute("Include")?.Value)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(static value => value!)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();

    private static string? Property(XDocument document, string name) =>
        document.Descendants(name).FirstOrDefault()?.Value?.Trim();

    internal sealed record ProjectFile(
        string Path,
        string RelativePath,
        bool IsWitness,
        string AssemblyName,
        string? RootNamespace,
        string? PackageId,
        string? OutputType,
        string RawText,
        IReadOnlyList<string> ProjectReferences,
        IReadOnlyList<string> PackageReferences,
        IReadOnlyList<string> InternalsVisibleTo,
        IReadOnlyList<string> SourceItemPaths)
    {
        /// <summary>
        /// True when the project lives under src/tests/. ADR 0001 makes the PATH, not the name,
        /// the authority for the product/test partition: the fixture profile is deliberately not
        /// named *.Tests, so a name-suffix rule would miss the one project the containment rule
        /// exists to hold.
        /// </summary>
        /// <remarks>
        /// A witness is never test-only regardless of where it is stored. Witness inputs sit
        /// under src/tests/ only because that is where the test project lives; each one imitates
        /// the project shape its rule guards, and classifying them by their storage location
        /// would let a rule skip its own witness and report a false clean.
        /// </remarks>
        internal bool IsTestOnly =>
            !IsWitness && RelativePath.StartsWith("src/tests/", StringComparison.Ordinal);

        internal bool IsProduct => !IsTestOnly && !IsWitness;

        internal IEnumerable<string> ReferencedAssemblyNames =>
            ProjectReferences.Select(static reference =>
                System.IO.Path.GetFileNameWithoutExtension(reference))!;
    }
}
