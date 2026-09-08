namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group W: the WebAssembly profile family's reference set and its public-API baseline.
/// </summary>
/// <remarks>
/// Each rule is asserted twice, as every group here is: the checkout is clean, and the rule rejects
/// a violating input. The violating inputs are witness files rather than edits, so a reader can see
/// what a violation looks like without one existing.
/// </remarks>
public sealed class WebAssemblyFamilyRuleTests
{
    private const string BaselineName =
        "src/Broiler.VM.Profile.WebAssembly/docs/api/public-api.txt";

    private const string WriteSwitch = "BROILER_API_WRITE";

    /// <summary>Whether this run regenerates the baseline rather than asserting against it.</summary>
    private static bool Writing =>
        string.Equals(Environment.GetEnvironmentVariable(WriteSwitch), "1", StringComparison.Ordinal);

    [Fact]
    public void W1_The_WebAssembly_Profile_References_Exactly_Abstractions_And_Binary()
    {
        // Non-vacuous in both directions: the real project exists and has the set, and the rule
        // rejects each of the four ways it could stop having it. Without the first clause this
        // would pass over a checkout that contained no WebAssembly profile at all.
        Assert.Contains(
            ComponentGraph.Projects,
            project => string.Equals(
                project.AssemblyName,
                WebAssemblyFamilyRules.ProfileAssembly,
                StringComparison.Ordinal));

        Assert.Empty(ComponentGraph.Projects.SelectMany(WebAssemblyFamilyRules.W1));

        Assert.Contains(
            WebAssemblyFamilyRules.W1(
                ComponentGraph.Witness("W1-profile-references-runtime.csproj.witness")),
            message => message.Contains("Broiler.VM.Runtime", StringComparison.Ordinal));

        Assert.Contains(
            WebAssemblyFamilyRules.W1(
                ComponentGraph.Witness("W1-profile-references-a-family-sibling.csproj.witness")),
            message => message.Contains("sibling it does not have", StringComparison.Ordinal));

        Assert.Contains(
            WebAssemblyFamilyRules.W1(
                ComponentGraph.Witness("W1-profile-package-reference.csproj.witness")),
            message => message.Contains("PackageReference", StringComparison.Ordinal));

        Assert.Contains(
            WebAssemblyFamilyRules.W1(
                ComponentGraph.Witness("W1-profile-internals-visible-to.csproj.witness")),
            message => message.Contains("opens internals", StringComparison.Ordinal));
    }

    [Fact]
    public void W1_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "W1", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        // The row must state the sibling clause, because that is the half no rule in group A or N
        // covers: both of them exempt a same-family sibling by design.
        Assert.Contains("sibling", row.Statement, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void W2_The_WebAssembly_Family_Surface_Is_Exactly_What_Its_Baseline_Declares()
    {
        var surface = WebAssemblyApiSurface.Describe();

        // A run that has not built the profile describes nothing, and an empty surface compared
        // against an empty baseline would agree. The rule fails on it instead.
        Assert.Equal(WebAssemblyApiSurface.FamilyAssemblies.Length, WebAssemblyApiSurface.Found().Count);
        Assert.NotEmpty(surface);

        if (Writing)
        {
            Write(surface);
            return;
        }

        var violations = W2Violations(surface);

        Assert.True(
            violations.Count == 0,
            $"The WebAssembly family's public surface and {BaselineName} disagree in " +
            $"{violations.Count} places. Review each, then regenerate with `{WriteSwitch}=1 " +
            $"dotnet test Broiler.VM.slnx -c Release`:{Environment.NewLine}" +
            string.Join(Environment.NewLine, violations.Take(40)));
    }

    [Fact]
    public void W2_Rejects_A_Baseline_That_Omits_An_Exported_Member()
    {
        var violations = Violations(
            WebAssemblyApiSurface.Describe(),
            Read(Witness("W2-baseline-omits-an-exported-member.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("exported but not declared:", StringComparison.Ordinal) &&
            violation.Contains("WebAssemblyProfile.Id", StringComparison.Ordinal));
    }

    [Fact]
    public void W2_Rejects_A_Baseline_Declaring_A_Member_That_Is_Gone()
    {
        var violations = Violations(
            WebAssemblyApiSurface.Describe(),
            Read(Witness("W2-baseline-declares-a-member-that-is-gone.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("WebAssemblyProfile.RetiredManifest", StringComparison.Ordinal));
    }

    /// <summary>
    /// The two profile baselines are disjoint subjects, and so are this one and the packable one.
    /// </summary>
    /// <remarks>
    /// A surface frozen in two files is a surface whose two records can disagree, which is the
    /// reason each family names its own list rather than widening a neighbour's.
    /// </remarks>
    [Fact]
    public void W2_Covers_Every_Family_Assembly_And_Nothing_Else()
    {
        var assemblies = WebAssemblyApiSurface.Describe()
            .Where(static line => line.StartsWith("type ", StringComparison.Ordinal))
            .Select(static line => line.Split(' ')[1])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            WebAssemblyApiSurface.FamilyAssemblies.OrderBy(static name => name, StringComparer.Ordinal),
            assemblies);

        Assert.Empty(WebAssemblyApiSurface.FamilyAssemblies.Intersect(
            ApiSurface.PackableAssemblies, StringComparer.Ordinal));

        Assert.Empty(WebAssemblyApiSurface.FamilyAssemblies.Intersect(
            ProfileApiSurface.FamilyAssemblies, StringComparer.Ordinal));
    }

    [Fact]
    public void W2_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "W2", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(BaselineName, row.Evidence, StringComparison.Ordinal);

        // The row must state the limit rather than claim a package surface. This baseline is over a
        // build output and nothing in this family packs.
        Assert.Contains("build output", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Writes what each group W rule said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_Group_W_Are_Written_When_Asked_For()
    {
        RuleReport.Write("W",
        [
            ("W1", () => ComponentGraph.Projects.SelectMany(WebAssemblyFamilyRules.W1)),
            ("W2", () => Writing ? [] : W2Violations(WebAssemblyApiSurface.Describe())),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(System.IO.Path.Combine(destination, "W.txt")),
                "a report for group W was asked for and none was written");
        }
    }

    private static List<string> W2Violations(IReadOnlyList<string> surface)
    {
        var found = WebAssemblyApiSurface.Found().Count;

        if (found != WebAssemblyApiSurface.FamilyAssemblies.Length)
        {
            return
            [
                $"the describer found {found} of the family's " +
                $"{WebAssemblyApiSurface.FamilyAssemblies.Length} assemblies on disk, so this rule " +
                "compared a partial surface",
            ];
        }

        return surface.Count == 0
            ? ["the describer produced no surface, so this rule compared nothing"]
            : Violations(surface, Read(Path()));
    }

    private static List<string> Violations(IEnumerable<string> surface, IEnumerable<string> baseline)
    {
        var exported = surface.ToHashSet(StringComparer.Ordinal);
        var declared = baseline.ToHashSet(StringComparer.Ordinal);

        var violations = exported
            .Where(line => !declared.Contains(line))
            .Select(static line => "exported but not declared: " + line.Trim())
            .ToList();

        violations.AddRange(declared
            .Where(line => !exported.Contains(line))
            .Select(static line => "declared but not exported: " + line.Trim()));

        violations.Sort(StringComparer.Ordinal);
        return violations;
    }

    private static IEnumerable<string> Read(string path) => File
        .ReadAllLines(path)
        .Where(static line => line.Length > 0 && !line.StartsWith('#'));

    private static void Write(IEnumerable<string> surface)
    {
        var path = Path();
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);

        var text = new System.Text.StringBuilder();

        text.Append("# The frozen public surface of the Broiler.VM.Profile.WebAssembly family's\n");
        text.Append("# one assembly. IT DOES NOT PACK - rule N4 keeps every project of every\n");
        text.Append("# profile family unpackable until that family takes its packaging decision -\n");
        text.Append("# so this file freezes what a composition root in this repository can bind to,\n");
        text.Append("# not what a consumer outside it can. The packable three are frozen separately\n");
        text.Append("# in docs/api/, and the JavaScript family in its own docs/api/.\n");
        text.Append("#\n");
        text.Append("# GENERATED - regenerate with:\n");
        text.Append("#   BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release\n");
        text.Append("# Rule W2 asserts it otherwise.\n");
        text.Append("#\n");
        text.Append("# Described from the build output by MetadataLoadContext, which reflects\n");
        text.Append("# without running anything: rule A11 forbids the project reference that would\n");
        text.Append("# let this be Assembly.Load, and loading would run the module initializers\n");
        text.Append("# invariant 2 forbids.\n");
        text.Append("\n");

        foreach (var line in surface)
        {
            text.Append(line).Append('\n');
        }

        File.WriteAllText(path, text.ToString(), AssuranceSources.Utf8NoBom);
    }

    private static string Path() => System.IO.Path.Combine(
        ComponentGraph.Root, BaselineName.Replace('/', System.IO.Path.DirectorySeparatorChar));

    private static string Witness(string fileName) => System.IO.Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
        "witnesses", "api", fileName);
}
