namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group A: the rules decidable from the project files. Each one is asserted twice - the
/// component is clean, and the rule rejects a deliberately violating input - because a rule that
/// has never rejected anything expresses nothing.
/// </summary>
public sealed class ProjectFileRuleTests
{
    private static IEnumerable<string> Sweep(
        Func<ComponentGraph.ProjectFile, IEnumerable<string>> rule) =>
        ComponentGraph.Projects.SelectMany(rule);

    [Fact]
    public void A1_No_Project_Reference_Resolves_Outside_The_Component()
    {
        Assert.Empty(Sweep(ArchitectureRules.A1));
        Assert.NotEmpty(ArchitectureRules.A1(
            ComponentGraph.Witness("A1-outbound-project-reference.csproj.witness")));
    }

    [Fact]
    public void A2_No_Project_References_A_Broiler_Package()
    {
        Assert.Empty(Sweep(ArchitectureRules.A2));
        Assert.NotEmpty(ArchitectureRules.A2(
            ComponentGraph.Witness("A2-broiler-package-reference.csproj.witness")));
    }

    [Fact]
    public void A3_No_Source_Item_Escapes_The_Component()
    {
        Assert.Empty(Sweep(ArchitectureRules.A3));
        Assert.NotEmpty(ArchitectureRules.A3(
            ComponentGraph.Witness("A3-shared-source-link.csproj.witness")));
    }

    [Fact]
    public void A4_No_Product_Project_References_A_Test_Only_Project()
    {
        Assert.Empty(Sweep(ArchitectureRules.A4));
        Assert.NotEmpty(ArchitectureRules.A4(
            ComponentGraph.Witness("A4-product-references-test.csproj.witness")));
    }

    [Fact]
    public void A5_Every_Test_Only_Project_Declares_IsPackable_False()
    {
        Assert.Empty(Sweep(ArchitectureRules.A5));
        Assert.NotEmpty(ArchitectureRules.A5(
            ComponentGraph.Witness("A5-test-project-omits-ispackable.csproj.witness")));
    }

    [Fact]
    public void A6_The_Package_Identity_Budget_Holds()
    {
        Assert.Empty(Sweep(ArchitectureRules.A6));

        var declared = ComponentGraph.Projects
            .Where(static project => project.PackageId is not null)
            .Select(static project => project.PackageId!)
            .OrderBy(static id => id, StringComparer.Ordinal);

        Assert.Equal(ArchitectureRules.DeclaredPackageIds, declared);

        Assert.NotEmpty(ArchitectureRules.A6(
            ComponentGraph.Witness("A6-fourth-package-id.csproj.witness")));
    }

    [Fact]
    public void A7_Declared_Edges_Match_The_Graph_Manifest_Exactly()
    {
        var actual = ComponentGraph.Projects
            .SelectMany(static project => project.ReferencedAssemblyNames
                .Select(target => new GraphManifest.Edge(project.AssemblyName, target)))
            .OrderBy(static edge => edge.From, StringComparer.Ordinal)
            .ThenBy(static edge => edge.To, StringComparer.Ordinal)
            .ToArray();

        // Both directions of the difference are reported: an edge the component grew without
        // amending ADR 0001, and an edge the manifest promises that the checkout no longer has.
        var added = actual.Except(ArchitectureRules.DeclaredEdges)
            .Select(static edge => $"+ {edge.From} -> {edge.To}");
        var removed = ArchitectureRules.DeclaredEdges.Except(actual)
            .Select(static edge => $"- {edge.From} -> {edge.To}");

        Assert.Empty(added.Concat(removed));

        // The witness: an edge that is not in the manifest is seen as added.
        var witness = ComponentGraph.Witness("A8-profile-references-runtime.csproj.witness");
        var withWitness = actual
            .Concat(witness.ReferencedAssemblyNames
                .Select(target => new GraphManifest.Edge(witness.AssemblyName, target)));

        Assert.NotEmpty(withWitness.Except(ArchitectureRules.DeclaredEdges));
    }

    [Fact]
    public void A8_No_Profile_Project_References_The_Runtime()
    {
        Assert.Empty(Sweep(ArchitectureRules.A8));
        Assert.NotEmpty(ArchitectureRules.A8(
            ComponentGraph.Witness("A8-profile-references-runtime.csproj.witness")));
    }

    [Fact]
    public void A9_The_Runtime_Is_A_Library()
    {
        Assert.Empty(Sweep(ArchitectureRules.A9));
        Assert.NotEmpty(ArchitectureRules.A9(
            ComponentGraph.Witness("A9-runtime-is-an-executable.csproj.witness")));
    }

    [Fact]
    public void A10_No_Product_Project_Opens_Its_Internals()
    {
        Assert.Empty(Sweep(ArchitectureRules.A10));
        Assert.NotEmpty(ArchitectureRules.A10(
            ComponentGraph.Witness("A10-product-internals-visible-to.csproj.witness")));
    }

    [Fact]
    public void A11_No_Project_Outside_A_Composition_Root_References_A_Profile()
    {
        Assert.Empty(Sweep(ArchitectureRules.A11));
        Assert.NotEmpty(ArchitectureRules.A11(
            ComponentGraph.Witness("A11-profile-reference-outside-composition-root.csproj.witness")));
    }

    [Fact]
    public void Every_Project_Lives_At_Its_Declared_Path()
    {
        // The path partition is what makes A4, A5 and A10 decidable, so it is asserted in its
        // own right rather than assumed by them.
        var violations = ComponentGraph.Projects
            .Where(static project =>
            {
                var segments = project.RelativePath.Split('/');
                var shapeIsProduct = segments is ["src", _, _];
                var shapeIsTest = segments is ["src", "tests", _, _];

                if (!shapeIsProduct && !shapeIsTest)
                {
                    return true;
                }

                return !string.Equals(
                    segments[^2],
                    Path.GetFileNameWithoutExtension(segments[^1]),
                    StringComparison.Ordinal);
            })
            .Select(static project => project.RelativePath);

        Assert.Empty(violations);
    }
}
