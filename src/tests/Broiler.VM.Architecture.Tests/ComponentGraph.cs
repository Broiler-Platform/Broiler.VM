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

    /// <summary>
    /// The milestone the rule register declares itself to be at, lowercased into the directory
    /// name its evidence bundle uses.
    /// </summary>
    /// <remarks>
    /// A suite that writes an outcome into a bundle has to write it into the CURRENT one. Naming
    /// the directory literally meant a VM-2 run overwrote a line in VM-0's retained bundle with a
    /// path from whichever machine happened to run last, which is the opposite of what "retained"
    /// means: ledger update rule 1 keeps earlier evidence as dated history, and history a later
    /// run edits is not history.
    /// </remarks>
    internal static string CurrentEvidenceDirectory { get; } = ReadCurrentMilestone();

    /// <summary>Every real project file in the component.</summary>
    internal static IReadOnlyList<ProjectFile> Projects { get; } = LoadProjects();

    /// <summary>
    /// The deliberately violating project files under witnesses/. They are named *.csproj.witness
    /// so MSBuild never globs them into the build, and they exist so that every group A rule can
    /// be shown to REJECT something. A rule that has never rejected anything is not evidence.
    /// </summary>
    internal static IReadOnlyList<ProjectFile> Witnesses { get; } = LoadWitnesses();

    /// <summary>
    /// Every witness input on disk, of every shape, found by recursing over <c>*.witness</c>.
    /// </summary>
    /// <remarks>
    /// <see cref="Witnesses"/> parses project files and so can only glob <c>*.csproj.witness</c>;
    /// the group E witnesses under witnesses/adr/ and the group H witnesses under
    /// witnesses/review/ are markdown and would not survive an XML load. The register's
    /// orphan check reads THIS list, because a witness file no rule names is an orphan wherever
    /// it sits, and globbing only the top directory left both subdirectories unchecked.
    /// </remarks>
    internal static IReadOnlyList<string> WitnessInputs { get; } = LoadWitnessInputs();

    internal static ProjectFile Witness(string fileName) =>
        Witnesses.SingleOrDefault(witness =>
            string.Equals(Path.GetFileName(witness.Path), fileName, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"No witness input named {fileName}.");

    private static string ReadCurrentMilestone()
    {
        var register = Path.Combine(
            Root, "src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json");

        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(register));

        return document.RootElement.GetProperty("milestone").GetString()!.ToLowerInvariant();
    }

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

    private static IReadOnlyList<string> LoadWitnessInputs()
    {
        var directory = Path.Combine(
            Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses");

        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(directory, "*.witness", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
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
            // Both spellings. The <InternalsVisibleTo> item is the documented one; an
            // <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
            // emits the same attribute and reached the compiled assembly past this rule and past
            // the assurance record, because it is neither an item A10 read nor a line in any
            // covered source file.
            InternalsVisibleTo:
            [
                .. Includes(document, "InternalsVisibleTo"),
                .. Includes(document, "AssemblyAttribute")
                    .Where(static include => include.Contains("InternalsVisibleTo", StringComparison.Ordinal)),
            ],
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
        // A path carrying an MSBuild property cannot be resolved to a real location here, so it
        // is returned unchanged and the rule decides what to do with the literal. Rules A1 and
        // A3 treat an unresolvable path as a violation of its own kind rather than clearing it -
        // see ArchitectureRules.Escapes, which also judges whether the literal's
        // parent-directory hops leave the component.
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

    /// <summary>
    /// Reads an identity property the way MSBuild would: the last unconditional definition wins.
    /// </summary>
    /// <remarks>
    /// Taking the first would invert MSBuild's own evaluation order, so a second
    /// <c>&lt;OutputType&gt;Exe&lt;/OutputType&gt;</c> appended after a <c>Library</c> would build an
    /// executable while rule A9 read the harmless earlier value. A conditional definition is not
    /// resolvable here at all - the condition depends on properties this reader does not
    /// evaluate - so it is surfaced as a sentinel that fails the rule loudly instead of being
    /// silently ignored.
    /// </remarks>
    private static string? Property(XDocument document, string name)
    {
        var definitions = document.Descendants(name).ToArray();

        if (definitions.Any(static definition => definition.Attribute("Condition") is not null))
        {
            return ConditionalProperty;
        }

        return definitions.LastOrDefault()?.Value?.Trim();
    }

    /// <summary>
    /// Stands in for an identity property this reader cannot evaluate. It matches no legal value,
    /// so any rule comparing against it fails and says why.
    /// </summary>
    internal const string ConditionalProperty = "<conditional; not evaluated by the architecture tests>";

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

        /// <summary>
        /// A named composition root: the one project kind ADR 0001 permits to reference a profile
        /// assembly.
        /// </summary>
        /// <remarks>
        /// A third partition rather than a flag on either of the other two, because a composition
        /// root is neither. It is not a product project - nothing there packs, no rule about
        /// product packages applies to it, and A4 would otherwise forbid the reference the record
        /// exists to permit - and it is not test-only, because it is published and run rather than
        /// collected by a test runner. ADR 0001 revision 1 records the directory and what boundary
        /// it enforces.
        /// </remarks>
        internal bool IsComposition =>
            !IsWitness && RelativePath.StartsWith("src/compositions/", StringComparison.Ordinal);

        internal bool IsProduct => !IsTestOnly && !IsComposition && !IsWitness;

        internal IEnumerable<string> ReferencedAssemblyNames =>
            ProjectReferences.Select(static reference =>
                System.IO.Path.GetFileNameWithoutExtension(reference))!;
    }
}
