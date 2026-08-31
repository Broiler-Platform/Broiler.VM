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

        // The second witness is the one that matters. An earlier version of this rule skipped
        // any path containing an MSBuild property, so a shared source link written through
        // $(MSBuildThisFileDirectory) cleared A3 without being examined - and A7 does not inspect
        // source items, so nothing else would have caught it.
        Assert.NotEmpty(ArchitectureRules.A3(
            ComponentGraph.Witness("A3-property-shared-source-link.csproj.witness")));
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

        // A MULTISET comparison, not a set difference. LINQ's Except deduplicates both operands,
        // so a project that declares the same ProjectReference twice would compare equal to one
        // that declares it once - and a duplicate edge is exactly the drift this rule exists to
        // catch. Both directions are reported: an edge the component grew without amending
        // ADR 0001, and an edge the manifest promises that the checkout no longer has.
        var difference = Counted(actual, +1)
            .Concat(Counted(ArchitectureRules.DeclaredEdges, -1))
            .GroupBy(static entry => entry.Edge, StringComparer.Ordinal)
            .Select(static group => (Edge: group.Key, Delta: group.Sum(static entry => entry.Sign)))
            .Where(static entry => entry.Delta != 0)
            .Select(static entry => entry.Delta > 0
                ? $"checkout has {entry.Delta} more: {entry.Edge}"
                : $"manifest has {-entry.Delta} more: {entry.Edge}")
            .OrderBy(static message => message, StringComparer.Ordinal);

        Assert.Empty(difference);

        static IEnumerable<(string Edge, int Sign)> Counted(
            IEnumerable<GraphManifest.Edge> edges, int sign) =>
            edges.Select(edge => ($"{edge.From} -> {edge.To}", sign));

        // The witness: an edge that is not in the manifest is seen as added.
        var witness = ComponentGraph.Witness("A8-profile-references-runtime.csproj.witness");
        var withWitness = actual
            .Concat(witness.ReferencedAssemblyNames
                .Select(target => new GraphManifest.Edge(witness.AssemblyName, target)));

        Assert.NotEmpty(withWitness.Except(ArchitectureRules.DeclaredEdges));
    }

    [Fact]
    public void A7_The_Declared_Graph_Is_Acyclic()
    {
        // The register says A7 subsumes acyclicity. It does so only because the manifest it
        // compares against is itself acyclic, and nothing was checking that - so the claim is
        // made good here rather than assumed. A cycle is also the one graph defect the build
        // catches on its own, by refusing to restore, which makes it easy to leave untested.
        var outgoing = ArchitectureRules.DeclaredEdges
            .GroupBy(static edge => edge.From, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Select(static edge => edge.To).ToArray(),
                StringComparer.Ordinal);

        var settled = new HashSet<string>(StringComparer.Ordinal);
        var onStack = new HashSet<string>(StringComparer.Ordinal);
        var cycles = new List<string>();

        foreach (var node in outgoing.Keys)
        {
            Walk(node, []);
        }

        Assert.Empty(cycles);

        void Walk(string node, IReadOnlyList<string> path)
        {
            if (onStack.Contains(node))
            {
                cycles.Add(string.Join(" -> ", [.. path, node]));
                return;
            }

            if (!settled.Add(node))
            {
                return;
            }

            onStack.Add(node);

            foreach (var next in outgoing.TryGetValue(node, out var targets) ? targets : [])
            {
                Walk(next, [.. path, node]);
            }

            onStack.Remove(node);
        }
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
    public void A12_A_Composition_Root_References_Only_Core_Packages_And_Profiles()
    {
        Assert.Empty(Sweep(ArchitectureRules.A12));

        // Three witnesses, because A12 makes three independent claims and a single non-empty
        // assertion would pin only whichever fires first. A root that drags the fixture profile
        // into a shipped image is the failure the exit gate names by name; a root that composes
        // nothing is a composition in name only; and a package reference is the way a closure
        // grows without any project reference changing.
        Assert.Contains(
            ArchitectureRules.A12(ComponentGraph.Witness("A12-composition-references-fixtures.csproj.witness")),
            message => message.Contains("Broiler.VM.Fixtures", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.A12(ComponentGraph.Witness("A12-composition-composes-no-profile.csproj.witness")),
            message => message.Contains("composes no profile", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.A12(ComponentGraph.Witness("A12-composition-package-reference.csproj.witness")),
            message => message.Contains("PackageReference", StringComparison.Ordinal));
    }

    [Fact]
    public void A13_A_Consumer_Profile_References_Exactly_Abstractions_And_Binary()
    {
        Assert.Empty(Sweep(ArchitectureRules.A13));

        // The reference set, the package surface and the internals grant are three separate
        // promises, so each has its own violating input naming its own content.
        Assert.Contains(
            ArchitectureRules.A13(ComponentGraph.Witness("A13-profile-references-runtime.csproj.witness")),
            message => message.Contains("Broiler.VM.Runtime", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.A13(ComponentGraph.Witness("A13-profile-package-reference.csproj.witness")),
            message => message.Contains("PackageReference", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.A13(ComponentGraph.Witness("A13-profile-internals-visible-to.csproj.witness")),
            message => message.Contains("opens internals", StringComparison.Ordinal));
    }

    [Fact]
    public void A11_Admits_A_Profile_Family_Sibling_And_Refuses_Another_Family()
    {
        // The exemption is one line in A11 and it is the line that decides whether this component
        // can hold a profile at all, so it is asserted in both directions rather than left to be
        // read out of the clean sweep above. The real profile assembly references its own format
        // sibling and must be clean; the same shape pointed at another language must not be.
        Assert.Empty(ArchitectureRules.A11(
            ComponentGraph.Projects.Single(project => string.Equals(
                project.AssemblyName, "Broiler.VM.Profile.JavaScript", StringComparison.Ordinal))));

        Assert.NotEmpty(ArchitectureRules.A11(
            ComponentGraph.Witness("N2-family-references-another-profile.csproj.witness")));

        // And the exemption is not a prefix exemption. Naming the predicate directly is what
        // stops a later widening from passing both assertions above by accident.
        Assert.True(ArchitectureRules.IsSameProfileFamily(
            "Broiler.VM.Profile.JavaScript", "Broiler.VM.Profile.JavaScript.Format"));

        Assert.False(ArchitectureRules.IsSameProfileFamily(
            "Broiler.VM.Profile.JavaScript", "Broiler.VM.Profile.WebAssembly"));

        Assert.False(ArchitectureRules.IsSameProfileFamily(
            "Broiler.VM.Runtime", "Broiler.VM.Profile.JavaScript"));
    }

    [Fact]
    public void N1_The_JavaScript_Profile_References_Abstractions_Binary_And_Its_Own_Format()
    {
        Assert.Empty(Sweep(ArchitectureRules.N1));

        // Four independent claims, four violating inputs, each asserted on the CONTENT of the
        // message it should produce. A bare non-empty check would pin only whichever clause fires
        // first and would let the other three be deleted in one patch with nothing red.
        Assert.Contains(
            ArchitectureRules.N1(ComponentGraph.Witness("N1-profile-references-runtime.csproj.witness")),
            message => message.Contains("Broiler.VM.Runtime", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N1(ComponentGraph.Witness("N1-profile-references-the-lowering.csproj.witness")),
            message => message.Contains("would carry a lowering", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N1(ComponentGraph.Witness("N1-profile-package-reference.csproj.witness")),
            message => message.Contains("PackageReference", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N1(ComponentGraph.Witness("N1-profile-internals-visible-to.csproj.witness")),
            message => message.Contains("opens internals", StringComparison.Ordinal));
    }

    [Fact]
    public void N2_No_Profile_Family_Reaches_Another_In_Either_Direction()
    {
        Assert.Empty(Sweep(ArchitectureRules.N2));

        Assert.Contains(
            ArchitectureRules.N2(ComponentGraph.Witness("N2-family-references-another-profile.csproj.witness")),
            message => message.Contains("Broiler.VM.Profile.WebAssembly", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N2(ComponentGraph.Witness("N2-non-family-project-references-the-profile.csproj.witness")),
            message => message.Contains("outside every profile family", StringComparison.Ordinal));
    }

    [Fact]
    public void N3_The_JavaScript_Format_Assembly_Is_A_Sink()
    {
        Assert.Empty(Sweep(ArchitectureRules.N3));

        // Non-vacuous in both directions: the real format project exists and has no edge, and the
        // rule rejects one that does. Without the first half this would pass over a checkout that
        // contained no format assembly at all.
        Assert.Contains(
            ComponentGraph.Projects,
            project => string.Equals(
                project.AssemblyName, "Broiler.VM.Profile.JavaScript.Format", StringComparison.Ordinal));

        Assert.NotEmpty(ArchitectureRules.N3(
            ComponentGraph.Witness("N3-format-references-the-profile.csproj.witness")));
    }

    [Fact]
    public void N4_No_JavaScript_Profile_Project_Is_Packable()
    {
        Assert.Empty(Sweep(ArchitectureRules.N4));

        // The rule has real subjects: three family projects in the graph, none packable. A rule
        // whose subject set is empty is witnessed and proves nothing about the checkout.
        Assert.Equal(
            3,
            ComponentGraph.Projects.Count(project =>
                ArchitectureRules.ProfileFamily(project.AssemblyName) is not null));

        Assert.Contains(
            ArchitectureRules.N4(ComponentGraph.Witness("N4-family-project-declares-a-package-id.csproj.witness")),
            message => message.Contains("declares PackageId", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N4(ComponentGraph.Witness("N4-family-project-omits-ispackable.csproj.witness")),
            message => message.Contains("IsPackable", StringComparison.Ordinal));
    }

    [Fact]
    public void The_Graph_Manifest_Describes_The_Checkout()
    {
        // A7 compares only the edge multiset, so the manifest's other columns were description
        // rather than assertion: ADR 0001 prints them as authority while nothing checked them.
        var problems = new List<string>();

        foreach (var declared in GraphManifest.Projects)
        {
            var actual = ComponentGraph.Projects
                .SingleOrDefault(project => string.Equals(
                    project.RelativePath, declared.Path, StringComparison.OrdinalIgnoreCase));

            if (actual is null)
            {
                problems.Add($"{declared.Path} is in the manifest but not in the checkout");
                continue;
            }

            if (!string.Equals(actual.AssemblyName, declared.AssemblyName, StringComparison.Ordinal))
            {
                problems.Add($"{declared.Path}: AssemblyName {actual.AssemblyName} != {declared.AssemblyName}");
            }

            if (!string.Equals(actual.RootNamespace, declared.RootNamespace, StringComparison.Ordinal))
            {
                problems.Add($"{declared.Path}: RootNamespace {actual.RootNamespace} != {declared.RootNamespace}");
            }

            if (!string.Equals(actual.PackageId, declared.PackageId, StringComparison.Ordinal))
            {
                problems.Add($"{declared.Path}: PackageId {actual.PackageId ?? "none"} != {declared.PackageId ?? "none"}");
            }

            if (actual.IsTestOnly != declared.IsTestProject && declared.IsPackable)
            {
                problems.Add($"{declared.Path}: packable but under src/tests/");
            }
        }

        var unlisted = ComponentGraph.Projects
            .Where(project => !GraphManifest.Projects.Any(declared => string.Equals(
                declared.Path, project.RelativePath, StringComparison.OrdinalIgnoreCase)))
            .Select(static project => $"{project.RelativePath} is in the checkout but not in the manifest");

        Assert.Empty(problems.Concat(unlisted));
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

                // The third shape, reserved by ADR 0001 at VM-0 and occupied at VM-3. It is
                // written out rather than folded into the product shape because the partition is
                // what makes A4, A5, A10 and A11 decidable: a composition root is permitted a
                // reference a product project is not, and a rule cannot tell them apart from a
                // path expression that treats both as "src/<name>/".
                var shapeIsComposition = segments is ["src", "compositions", _, _];

                if (!shapeIsProduct && !shapeIsTest && !shapeIsComposition)
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
