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
            // INCONCLUSIVE, and it has to be visible. An earlier version asserted a constant
            // true here, which meant a standalone checkout printed the same "33 passed" line as
            // a checkout where the rule actually scanned something - the outcome the register
            // and three documents describe as inconclusive was indistinguishable from a pass.
            // The branch taken is now written to the evidence bundle, so a reader can tell which
            // of the two runs produced their logs.
            RecordOutcome(
                "INCONCLUSIVE - no aggregate checkout above the component root, so D1 scanned " +
                "nothing. This is not a pass: see Exclusion EX-01.");
            return;
        }

        var violations = Directory
            .EnumerateFiles(aggregate.FullName, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase))
            .Where(static path => !IsUnderBuildOutput(path))
            .Where(path => ReferencesTheComponent(path, ComponentGraph.Root))
            .Select(path => Path.GetRelativePath(aggregate.FullName, path))
            .ToArray();

        RecordOutcome(
            $"SCANNED - aggregate checkout at {aggregate.FullName}; " +
            $"{violations.Length} project file(s) outside the component reference into it.");

        Assert.Empty(violations);
    }

    [Fact]
    public void D1_Rejects_A_Reference_Into_The_Component()
    {
        // The witness is materialised OUTSIDE the component root and run through the whole D1
        // pipeline, including the "not under the component" filter that is the entire substance
        // of an inbound edge. Evaluating the stored witness in place would exercise only the
        // reference predicate and would leave that filter - the part that decides whether a
        // project counts as outside at all - untested.
        var stored = Path.Combine(
            ComponentGraph.Root,
            "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "D1-inbound-project-reference.csproj.witness");

        Assert.True(File.Exists(stored), $"Missing witness input {stored}.");

        var staging = Directory.CreateTempSubdirectory("broiler-vm-d1-");

        try
        {
            // The stored witness points three levels up, which is where src/ sits relative to
            // the witnesses directory. From the staging directory the same shape has to be
            // rewritten to reach the real component, which is what an inbound edge from another
            // repository in the aggregate checkout would look like.
            var project = Path.Combine(staging.FullName, "Inbound.csproj");
            var target = Path.Combine(ComponentGraph.Root, "src", "Broiler.VM.Runtime", "Broiler.VM.Runtime.csproj");

            File.WriteAllText(project, $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <ItemGroup>
                    <ProjectReference Include="{target}" />
                  </ItemGroup>
                </Project>
                """);

            Assert.False(
                project.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase),
                "The witness must live outside the component for this to test anything.");

            Assert.True(ReferencesTheComponent(project, ComponentGraph.Root));
        }
        finally
        {
            staging.Delete(recursive: true);
        }
    }

    /// <summary>
    /// Writes which branch rule D1 took to the evidence bundle, so that "inconclusive" and
    /// "scanned and clean" are distinguishable after the fact.
    /// </summary>
    /// <remarks>
    /// A test run reports only pass or fail, and D1 has a third outcome that must not be
    /// collapsed into the first. The file is part of the retained bundle rather than console
    /// output because the bundle is what a reviewer reads.
    /// </remarks>
    /// <remarks>
    /// It goes into the bundle of the milestone the rule register declares, not into a literal
    /// <c>vm-0</c>. The literal meant every later run rewrote a retained VM-0 line with the path
    /// of whichever machine ran last - so the bundle a reviewer reads for VM-0 recorded a machine
    /// that collected nothing for it.
    /// </remarks>
    private static void RecordOutcome(string outcome)
    {
        var directory = Path.Combine(
            ComponentGraph.Root, "docs", "evidence", ComponentGraph.CurrentEvidenceDirectory);

        if (!Directory.Exists(directory))
        {
            return;
        }

        File.WriteAllText(
            Path.Combine(directory, "d1-outcome.txt"),
            "Rule D1, inbound half of the legacy boundary." + Environment.NewLine +
            outcome + Environment.NewLine);
    }

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
