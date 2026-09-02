namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// N10: the JavaScript profile family's public surface is exactly what its baseline declares.
/// </summary>
/// <remarks>
/// <para>
/// <b>This closes a clause that had been open since JS-0.</b> Bundle JS-0-001 recorded that the
/// public API baseline's subject is the packable set, so it does not cover the profile's
/// assemblies, and carried the clause to JS-1 on the grounds that JS-1 would land a public
/// surface. JS-1 landed one and could not close it: <see cref="ApiSurface"/> describes a surface
/// by loading an assembly, loading needs a project reference, and rule A11 forbids a test project
/// to have one on a profile. Bundle JS-1-001 named two routes out and carried the clause to
/// JS-3b - which is blocked on JS-2, which is blocked on the core's acceptance gate. Decision
/// JSD-0012 re-homes it here, for the same reason the JS-3 split exists: a clause behind a blocker
/// it does not need is a clause nobody can schedule.
/// </para>
/// <para>
/// The route taken is the first one JS-1-001 named - describe from metadata - and it needs neither
/// the reference A11 forbids nor the execution invariant 2 forbids. See
/// <see cref="ProfileApiSurface"/> for why loading is not an option and what this reads instead.
/// </para>
/// <para>
/// The rule is in group N because its subject is the JavaScript profile family, the same subject
/// N1 through N9 have. It is deliberately not a widening of M1: M1's own non-vacuity clause is
/// that it covers the packable assemblies AND NOTHING ELSE, and these three are not packable -
/// rule N4 keeps every one of them unpackable until JS-10 takes the packaging decision. Two
/// subjects, two baselines, one describer.
/// </para>
/// </remarks>
public sealed class ProfileApiSurfaceTests
{
    private const string BaselineName =
        "src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt";

    private const string WriteSwitch = "BROILER_API_WRITE";

    /// <summary>Whether this run regenerates the baseline rather than asserting against it.</summary>
    private static bool Writing =>
        string.Equals(Environment.GetEnvironmentVariable(WriteSwitch), "1", StringComparison.Ordinal);

    /// <summary>
    /// N10's clean direction over this checkout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Like M1, N10 was excluded from the rule-message report on a reading of its shape rather
    /// than of its body</b>, and like M1 its comparison has always been a message list. What is
    /// genuinely new here is the two non-vacuity clauses, which were assertions the report could
    /// not see and are messages now.
    /// </para>
    /// <para>
    /// They are the clauses that most needed saying. A run that has not built the profile
    /// describes nothing, an empty surface compared against an empty baseline agrees, and the
    /// report would have written "N10: 0 message(s)" - the rule's way of saying it is satisfied -
    /// for a run in which it had read no assembly at all.
    /// </para>
    /// </remarks>
    private static List<string> N10Violations() => N10Violations(ProfileApiSurface.Describe());

    /// <inheritdoc cref="N10Violations()"/>
    private static List<string> N10Violations(IReadOnlyList<string> surface)
    {
        var found = ProfileApiSurface.Found().Count;

        if (found != ProfileApiSurface.FamilyAssemblies.Length)
        {
            return
            [
                $"the describer found {found} of the family's " +
                $"{ProfileApiSurface.FamilyAssemblies.Length} assemblies on disk, so this rule " +
                "compared a partial surface",
            ];
        }

        return surface.Count == 0
            ? ["the describer produced no surface, so this rule compared nothing"]
            : Violations(surface, Read(Path()));
    }

    /// <summary>Writes what N10 said about this checkout, when asked to.</summary>
    /// <remarks>
    /// Its own file rather than group N's, because <c>DiagnosticRegistryRuleTests</c> writes that
    /// one and xunit runs test classes in parallel: two classes appending to a single name is an
    /// interleaved file, which is the reason the report is per-group in the first place.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_N10_Are_Written_When_Asked_For()
    {
        RuleReport.Write("N10",
        [
            ("N10", () => Writing ? [] : N10Violations()),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(System.IO.Path.Combine(destination, "N10.txt")),
                "a report for N10 was asked for and none was written");
        }
    }

    [Fact]
    public void N10_The_Profile_Family_Surface_Is_Exactly_What_Its_Baseline_Declares()
    {
        var surface = ProfileApiSurface.Describe();

        // A run that has not built the profile describes nothing, and an empty surface compared
        // against an empty baseline would agree. The rule fails on it instead.
        Assert.Equal(ProfileApiSurface.FamilyAssemblies.Length, ProfileApiSurface.Found().Count);
        Assert.NotEmpty(surface);

        if (Writing)
        {
            Write(surface);
            return;
        }

        var violations = N10Violations(surface);

        Assert.True(
            violations.Count == 0,
            $"The profile family's public surface and {BaselineName} disagree in " +
            $"{violations.Count} places. Review each, then regenerate with `{WriteSwitch}=1 " +
            $"dotnet test Broiler.VM.slnx -c Release`:{Environment.NewLine}" +
            string.Join(Environment.NewLine, violations.Take(40)));
    }

    [Fact]
    public void N10_Rejects_A_Baseline_That_Omits_An_Exported_Member()
    {
        // An addition. For a profile this is the direction that matters most: the profile
        // assemblies are referenced by composition roots, and a member added here is a member a
        // composition can bind to without anyone deciding it should be bindable.
        var violations = Violations(
            ProfileApiSurface.Describe(),
            Read(Witness("N10-baseline-omits-an-exported-member.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("exported but not declared:", StringComparison.Ordinal) &&
            violation.Contains("JavaScriptFormat.FormatVersion", StringComparison.Ordinal));
    }

    [Fact]
    public void N10_Rejects_A_Baseline_Declaring_A_Member_That_Is_Gone()
    {
        var violations = Violations(
            ProfileApiSurface.Describe(),
            Read(Witness("N10-baseline-declares-a-member-that-is-gone.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("JavaScriptFormat.RetiredCeiling", StringComparison.Ordinal));
    }

    [Fact]
    public void N10_Covers_Every_Family_Assembly_And_Nothing_Else()
    {
        var assemblies = ProfileApiSurface.Describe()
            .Where(static line => line.StartsWith("type ", StringComparison.Ordinal))
            .Select(static line => line.Split(' ')[1])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            ProfileApiSurface.FamilyAssemblies.OrderBy(static name => name, StringComparer.Ordinal),
            assemblies);

        // And the two baselines are disjoint subjects. A packable assembly appearing in this one
        // would mean a surface frozen twice, which is a surface whose two records can disagree.
        Assert.Empty(ProfileApiSurface.FamilyAssemblies.Intersect(
            ApiSurface.PackableAssemblies, StringComparer.Ordinal));
    }

    /// <summary>
    /// The describer reflects without running anything, which is the property that makes this rule
    /// legal at all.
    /// </summary>
    /// <remarks>
    /// A <c>MetadataLoadContext</c> cannot execute code, and asserting that is not a matter of
    /// reading its documentation: a type obtained from one throws when a caller tries to invoke
    /// through it. The clause below is that, in one line, so a future edit that swapped the loader
    /// for <c>Assembly.LoadFrom</c> - which would work, and would run the module initializers rule
    /// B5b exists to detect - fails here rather than passing quietly.
    /// </remarks>
    [Fact]
    public void N10_Describes_Without_Executing_Anything()
    {
        var surface = ProfileApiSurface.Describe();

        Assert.NotEmpty(surface);
        Assert.Contains(surface, static line => line.Contains(
            "Broiler.VM.Profile.JavaScript.Format.JavaScriptFormat", StringComparison.Ordinal));

        // The type the describer saw is a metadata-only type: asking it for a runtime handle is
        // what a loader that had actually loaded the assembly would answer.
        var refused = Assert.Throws<InvalidOperationException>(static () =>
        {
            var format = ProfileApiSurface.LoadForInspection(
                "Broiler.VM.Profile.JavaScript.Format",
                "Broiler.VM.Profile.JavaScript.Format.JavaScriptFormat");

            return format.TypeHandle;
        });

        Assert.Contains("MetadataLoadContext", refused.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void N10_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "N10", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(BaselineName, row.Evidence, StringComparison.Ordinal);

        // The row must state the limit rather than claim a package surface. This baseline is over
        // a build output and nothing here packs.
        Assert.Contains("build output", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
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

        text.Append("# The frozen public surface of the Broiler.VM.Profile.JavaScript family's\n");
        text.Append("# three assemblies. NONE OF THEM PACKS - rule N4 keeps every one of them\n");
        text.Append("# unpackable until JS-10 takes the packaging decision - so this file freezes\n");
        text.Append("# what a composition root in this repository can bind to, not what a consumer\n");
        text.Append("# outside it can. The packable three are frozen separately in docs/api/.\n");
        text.Append("#\n");
        text.Append("# GENERATED - regenerate with:\n");
        text.Append("#   BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release\n");
        text.Append("# Rule N10 asserts it otherwise. Decision JSD-0012 owns it.\n");
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
