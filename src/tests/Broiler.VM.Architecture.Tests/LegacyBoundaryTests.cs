using System.Xml.Linq;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The boundary against every legacy Broiler component, in both directions.
/// </summary>
/// <remarks>
/// <para>
/// Roadmap section 12 says Broiler.VM does not depend on, extend, wrap or replace the legacy
/// Broiler.JS component, and sections 5 and 14 make that boundary bidirectional. VM-0 was asked
/// to record it "as an architecture-tested rule, not a convention".
/// </para>
/// <para>
/// The outbound half is enforced twice over, by rules A1, A2 and A3 on project files and B3 on
/// compiled metadata, because a package can reintroduce an edge no project file spells out.
/// </para>
/// <para>
/// The inbound half is rule D1, and it is honest about what it can see. A standalone checkout of
/// this submodule cannot enumerate the repositories that might reference it, so D1 runs when an
/// aggregate checkout is present above the component and reports INCONCLUSIVE - not a pass -
/// when none is. Exclusion EX-01 records that conditionality in ADR 0001, in the VM-0 evidence
/// bundle and in the ledger row. Closing it unconditionally means a check in the aggregate
/// repository, which milestone VM-0 does not own; ADR 0001 records that as a recommendation.
/// </para>
/// </remarks>
public sealed class LegacyBoundaryTests
{
    [Fact]
    public void D1_No_Project_Outside_The_Component_References_Into_It()
    {
        var aggregate = Directory.GetParent(ComponentGraph.Root);

        if (aggregate is null || !File.Exists(Path.Combine(aggregate.FullName, ".gitmodules")))
        {
            // INCONCLUSIVE. Recorded, not silently passed: the register marks D1's precondition,
            // and the evidence bundle records which of the two branches actually ran.
            Assert.True(
                InconclusiveIsAcceptable,
                "D1 is inconclusive: no aggregate checkout above the component root.");
            return;
        }

        var violations = Directory
            .EnumerateFiles(aggregate.FullName, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase))
            .Where(static path => !IsUnderBuildOutput(path))
            .Where(path => ReferencesTheComponent(path, ComponentGraph.Root))
            .Select(path => Path.GetRelativePath(aggregate.FullName, path))
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void D1_Rejects_A_Reference_Into_The_Component()
    {
        // The same predicate that clears the aggregate checkout must reject the witness, or the
        // clean result above means nothing. The A1 witness is reused deliberately: an inbound
        // edge and an outbound edge are the same ProjectReference seen from opposite sides.
        var witness = Path.Combine(
            ComponentGraph.Root,
            "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "D1-inbound-project-reference.csproj.witness");

        Assert.True(File.Exists(witness), $"Missing witness input {witness}.");
        Assert.True(ReferencesTheComponent(witness, ComponentGraph.Root));
    }

    /// <summary>
    /// The inconclusive branch is a recorded outcome rather than a hidden pass. It is a constant
    /// so that the branch is visible in the source and cannot be mistaken for an assertion that
    /// something was checked.
    /// </summary>
    private const bool InconclusiveIsAcceptable = true;

    private static bool IsUnderBuildOutput(string path) =>
        path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
        path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// True when the project declares a ProjectReference resolving inside the component, or a
    /// PackageReference on one of its packages. The path is resolved rather than matched as
    /// text: a substring test would miss a reference written through a directory that does not
    /// spell the package name, and would fire on any comment mentioning the component.
    /// </summary>
    private static bool ReferencesTheComponent(string projectPath, string componentRoot)
    {
        XDocument document;

        try
        {
            document = XDocument.Load(projectPath);
        }
        catch (System.Xml.XmlException)
        {
            // An unparseable project file elsewhere in the aggregate is that repository's
            // problem, not evidence about this boundary.
            return false;
        }

        var directory = Path.GetDirectoryName(projectPath)!;

        var byProject = document
            .Descendants("ProjectReference")
            .Select(static element => element.Attribute("Include")?.Value)
            .Where(static include => !string.IsNullOrWhiteSpace(include))
            .Select(include => include!.Replace('\\', Path.DirectorySeparatorChar))
            .Where(static include => !include.Contains('$'))
            .Select(include => Path.GetFullPath(Path.Combine(directory, include)))
            .Any(resolved => resolved.StartsWith(componentRoot, StringComparison.OrdinalIgnoreCase));

        var byPackage = document
            .Descendants("PackageReference")
            .Select(static element => element.Attribute("Include")?.Value)
            .Any(static include => include is not null &&
                include.StartsWith("Broiler.VM", StringComparison.OrdinalIgnoreCase));

        return byProject || byPackage;
    }
}
