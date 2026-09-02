namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group M: the rule that freezes the public API by enumerating it.
/// </summary>
/// <remarks>
/// <para>
/// The VM-6 gate asks that the public API be frozen and that it match a baseline. Group V does
/// not do that and was never meant to: every V rule is a claim about a named property of the
/// surface, so a member added tomorrow that breaks none of them is an addition nothing notices,
/// and a member deleted tomorrow is a breaking change nothing notices either.
/// </para>
/// <para>
/// M1 is the enumeration. It compares the surface the built assemblies actually export against
/// <c>docs/api/public-api.txt</c> in both directions, so an addition and a removal are each a
/// failure, and a signature change - which is one of each - is both.
/// </para>
/// <para>
/// <b>The baseline is regenerated deliberately.</b>
/// <c>BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release</c> rewrites the file; without it
/// the test asserts. That is the same shape as the Code Assurance generator and it is chosen for
/// the same reason: a baseline that regenerated itself on every run would agree with every change
/// and prove nothing, and one that could only be edited by hand would be edited by hand wrongly.
/// The switch makes the update an act rather than a side effect, and the diff it produces is what
/// a reviewer reads.
/// </para>
/// <para>
/// The letter M is used because A, B, C, D and E are taken by ADR 0001 and ADR 0003, V by the API
/// rules VM-1 minted, H by the review-record rules, J by Code Assurance, K by the composition
/// register and L by the baseline register. G, P, S, F, T and R all collide with clause labels
/// inside ADRs 0007, 0011 and 0012.
/// </para>
/// </remarks>
public sealed class ApiSurfaceTests
{
    private const string BaselineName = "docs/api/public-api.txt";

    private const string WriteSwitch = "BROILER_API_WRITE";

    /// <summary>
    /// M1's clean direction over this checkout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>M1 never needed rewriting, and four bundles said it did.</b> They recorded it among the
    /// rules that "assert equalities between a value and a constant", which is not what it does:
    /// <see cref="Violations"/> has returned a list of "exported but not declared" and "declared
    /// but not exported" messages since M1 was written, and the test has always compared its count
    /// to zero. The exclusion was a claim about a body nobody had read. It is worth recording as
    /// exactly that, because a survey that classifies by shape will mis-sort whatever it does not
    /// open.
    /// </para>
    /// <para>
    /// The empty-surface clause is the only thing added here. The test asserts it separately
    /// because it needs the surface anyway; the report needs it because a rule that described
    /// nothing would compare nothing, find no disagreement, and report the silence of a satisfied
    /// rule.
    /// </para>
    /// </remarks>
    private static IReadOnlyList<string> M1Violations() => M1Violations(ApiSurface.Describe());

    /// <inheritdoc cref="M1Violations()"/>
    private static IReadOnlyList<string> M1Violations(IReadOnlyList<string> surface) =>
        surface.Count == 0
            ? ["the describer produced no public surface, so this rule compared nothing"]
            : Violations(surface, Read(Path()));

    /// <summary>Writes what M1 said about this checkout, when asked to.</summary>
    /// <remarks>
    /// Silent under the write switch, for the reason rules J3, J5 and J7 are: a run that is
    /// REGENERATING the baseline has not compared anything against it, and reporting the
    /// pre-regeneration disagreements would be reporting a comparison the rule declined to make.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_M1_Are_Written_When_Asked_For()
    {
        RuleReport.Write("M1",
        [
            ("M1", () => Writing ? [] : M1Violations()),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(System.IO.Path.Combine(destination, "M1.txt")),
                "a report for M1 was asked for and none was written");
        }
    }

    /// <summary>Whether this run regenerates the baseline rather than asserting against it.</summary>
    private static bool Writing =>
        string.Equals(Environment.GetEnvironmentVariable(WriteSwitch), "1", StringComparison.Ordinal);

    [Fact]
    public void M1_The_Public_Surface_Is_Exactly_What_The_Baseline_Declares()
    {
        var surface = ApiSurface.Describe();

        Assert.NotEmpty(surface);

        if (Writing)
        {
            Write(surface);
            return;
        }

        var violations = M1Violations(surface);

        Assert.True(
            violations.Count == 0,
            $"The public surface and {BaselineName} disagree in {violations.Count} places. " +
            $"Review each, then regenerate with `{WriteSwitch}=1 dotnet test Broiler.VM.slnx " +
            $"-c Release`:{Environment.NewLine}" +
            string.Join(Environment.NewLine, violations.Take(40)));
    }

    [Fact]
    public void M1_Rejects_A_Baseline_That_Omits_An_Exported_Member()
    {
        // An addition. The direction that matters for a component promising a frozen surface: a
        // member that reaches a package without anyone deciding it should.
        var violations = Violations(
            ApiSurface.Describe(),
            Read(Witness("M1-baseline-omits-an-exported-member.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("exported but not declared:", StringComparison.Ordinal) &&
            violation.Contains("VmCoreContract.Version", StringComparison.Ordinal));
    }

    [Fact]
    public void M1_Rejects_A_Baseline_Declaring_A_Member_That_Is_Gone()
    {
        // A removal, which is a breaking change for every consumer of the package.
        var violations = Violations(
            ApiSurface.Describe(),
            Read(Witness("M1-baseline-declares-a-member-that-is-gone.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("VmCoreContract.RetiredMember", StringComparison.Ordinal));
    }

    [Fact]
    public void M1_Rejects_A_Changed_Signature_As_Both_A_Removal_And_An_Addition()
    {
        // A signature change is not a third kind of violation, and pretending otherwise would mean
        // deciding which member a changed line USED to be - a guess the rule has no basis for. It
        // is reported as the removal of the old signature and the addition of the new one, and the
        // reader is the one who sees they are the same member.
        var violations = Violations(
            ApiSurface.Describe(),
            Read(Witness("M1-baseline-changes-a-signature.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("MinimumSupportedVersion", StringComparison.Ordinal));

        Assert.Contains(violations, violation =>
            violation.StartsWith("exported but not declared:", StringComparison.Ordinal) &&
            violation.Contains("MinimumSupportedVersion", StringComparison.Ordinal));
    }

    [Fact]
    public void M1_Rejects_A_Baseline_Naming_An_Assembly_Outside_The_Packable_Three()
    {
        // The fixtures assembly has public types and is not packable. A baseline that admitted one
        // would be freezing a surface no package carries, which is worse than freezing nothing: it
        // reads as coverage.
        var violations = Violations(
            ApiSurface.Describe(),
            Read(Witness("M1-baseline-names-a-test-assembly.txt.witness")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("Broiler.VM.Fixtures", StringComparison.Ordinal));
    }

    [Fact]
    public void M1_Covers_Every_Packable_Assembly_And_Nothing_Else()
    {
        var assemblies = ApiSurface.Describe()
            .Where(static line => line.StartsWith("type ", StringComparison.Ordinal))
            .Select(static line => line.Split(' ')[1])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ApiSurface.PackableAssemblies.OrderBy(static name => name, StringComparer.Ordinal), assemblies);
    }

    [Fact]
    public void M1_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "M1", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(BaselineName, row.Evidence, StringComparison.Ordinal);
    }

    /// <summary>
    /// The two ways the surface and its baseline can disagree, in both directions.
    /// </summary>
    /// <remarks>
    /// Set difference rather than a line-by-line diff, because the file is sorted and a positional
    /// comparison would report every line after an insertion as changed. Comments and blank lines
    /// are not part of the surface and are dropped from the baseline before comparing, so the file
    /// can carry a header explaining what it is.
    /// </remarks>
    private static IReadOnlyList<string> Violations(
        IReadOnlyList<string> surface,
        IReadOnlyList<string> baseline)
    {
        var declared = baseline.ToHashSet(StringComparer.Ordinal);
        var exported = surface.ToHashSet(StringComparer.Ordinal);

        var violations = new List<string>();

        violations.AddRange(surface
            .Where(line => !declared.Contains(line))
            .Select(static line => "exported but not declared: " + line.Trim()));

        violations.AddRange(baseline
            .Where(line => !exported.Contains(line))
            .Select(static line => "declared but not exported: " + line.Trim()));

        return violations;
    }

    private static IReadOnlyList<string> Read(string path)
    {
        Assert.True(File.Exists(path), $"Missing API baseline {path}.");

        return File.ReadAllLines(path)
            .Where(static line => line.Length > 0 && !line.StartsWith('#'))
            .ToArray();
    }

    private static void Write(IReadOnlyList<string> surface)
    {
        var path = Path();

        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);

        var header = new[]
        {
            "# The frozen public surface of the three packable Broiler.VM assemblies.",
            "#",
            "# GENERATED - regenerate with:",
            "#   BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release",
            "#",
            "# Rule M1 compares this file against the built assemblies in BOTH directions, so a",
            "# member added without a decision fails and a member removed without one fails too.",
            "# Regenerating is how a deliberate change is recorded; the diff is what a reviewer",
            "# reads, and a diff nobody can explain is the finding.",
            "#",
            "# Constants carry their values, because a literal's value is part of the contract:",
            "# VmCoreContract.Version moving from 1 to 2 is the amendment ADR 0003 governs.",
            string.Empty,
        };

        // LF, explicitly. WriteAllLines uses Environment.NewLine, so every regeneration on Windows
        // rewrote all twelve hundred lines with CRLF while .gitattributes stores the file as LF -
        // a whole-file diff on a run that changed nothing, which is the diff a reviewer is
        // supposed to be reading. Rule N10's writer does the same.
        File.WriteAllText(
            path,
            string.Concat(header.Concat(surface).Select(static line => line + "\n")),
            AssuranceSources.Utf8NoBom);
    }

    private static string Path() =>
        System.IO.Path.Combine(ComponentGraph.Root, "docs", "api", "public-api.txt");

    private static string Witness(string fileName) =>
        System.IO.Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "api", fileName);
}
