namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N13: the conformance harness's ingestion path is never advertised.
/// </summary>
/// <remarks>
/// <para>
/// The rule and its controls live in a file of their own because its subject is neither the
/// register nor the diagnostic registry: it is one composition root and every route by which that
/// root, or a suite it reads, could reach an image somebody runs.
/// </para>
/// <para>
/// <b>Every clause has a witness of its own.</b> A rule asserted with one non-empty check pins
/// only whichever clause fires first, and four independent clauses can then be deleted in one
/// patch with nothing red - which is the shape of defeat this component's register records finding
/// four times in its own group H.
/// </para>
/// </remarks>
public sealed class IngestionPathRuleTests
{
    /// <summary>The harness root this rule is about.</summary>
    private const string Harness = "Broiler.VM.Composition.JavaScript.Conformance";

    /// <summary>
    /// The suite directories no project file may name.
    /// </summary>
    /// <remarks>
    /// A path fragment rather than a repository-relative path, so that a project file reaching the
    /// directory by any number of <c>..</c> steps is still caught. The harness is handed its suite
    /// as a command-line argument at run time, which is the whole reason no project file needs to
    /// name one.
    /// <para>
    /// <b>The second entry is the retained Octane workload, and it is here because the notice file
    /// says it is.</b> That archive is separately licensed third-party CODE rather than a document,
    /// and <c>THIRD_PARTY_NOTICES.md</c> states the mechanism keeping it out of a shipped image in
    /// the same words it uses for the conformance suite - so the mechanism has to exist. The
    /// end-user host takes the extracted files as paths on a command line exactly as the harness
    /// takes a suite, so no project file needs to name this one either.
    /// </para>
    /// </remarks>
    private static readonly string[] SuiteDirectories = ["tests/conformance", "tests/octane"];

    [Fact]
    public void N13_The_Harness_And_Its_Suite_Reach_No_Package_And_No_Advertised_Closure()
    {
        Assert.Empty(ArchitectureRules.N13(
            Harness,
            ComponentGraph.Projects,
            CompositionRegisterTests.RegisterRows,
            CompositionRegisterTests.Closures(),
            SuiteDirectories));

        // Non-vacuous in both of the ways this rule can be empty for the wrong reason. It passes by
        // finding nothing, so a renamed harness would be its cleanest-looking result, and a closure
        // list it never reads would make its most expensive clause free.
        Assert.Single(ComponentGraph.Projects.Where(project =>
            string.Equals(project.AssemblyName, Harness, StringComparison.Ordinal)));

        Assert.NotEmpty(CompositionRegisterTests.Closures());

        Assert.Contains(
            ArchitectureRules.N13("Broiler.VM.Composition.JavaScript.Renamed", ComponentGraph.Projects, [], [], []),
            violation => violation.Contains("quantifying over nothing", StringComparison.Ordinal));
    }

    [Fact]
    public void N13_Rejects_A_Reference_From_The_Execution_Only_Root()
    {
        // The direction that would actually ship: somebody wants the harness's fixtures, its report
        // format or "just the self-check", and adds one line to the root whose whole claim is that
        // it compiles nothing.
        var violations = ArchitectureRules.N13(
            Harness,
            [
                .. ComponentGraph.Projects,
                ComponentGraph.Witness("N13-execution-only-references-the-harness.csproj.witness"),
            ],
            CompositionRegisterTests.RegisterRows,
            CompositionRegisterTests.Closures(),
            SuiteDirectories);

        Assert.Contains(
            violations,
            violation => violation.Contains(
                "Broiler.VM.Composition.JavaScript.ExecutionOnly references " + Harness,
                StringComparison.Ordinal));
    }

    [Fact]
    public void N13_Rejects_A_Project_That_Carries_Suite_Files()
    {
        // No reference is added and no assembly changes: the suite is copied into a build output,
        // which is redistribution by a mechanism every reference-set rule in this component is
        // blind to.
        var violations = ArchitectureRules.N13(
            Harness,
            [
                .. ComponentGraph.Projects,
                ComponentGraph.Witness("N13-a-project-that-carries-suite-files.csproj.witness"),
            ],
            CompositionRegisterTests.RegisterRows,
            CompositionRegisterTests.Closures(),
            SuiteDirectories);

        Assert.Contains(
            violations,
            violation => violation.Contains(
                "names the suite directory tests/conformance", StringComparison.Ordinal));
    }

    [Fact]
    public void N13_Rejects_A_Package_Identity_And_A_Missing_Non_Packable_Declaration()
    {
        var packable = ArchitectureRules.N13(
            Harness,
            [Shell(Harness, packageId: "Broiler.VM.Conformance", rawText: "<Project />")],
            [],
            [],
            []);

        Assert.Contains(
            packable,
            violation => violation.Contains(
                "declares the package identity Broiler.VM.Conformance", StringComparison.Ordinal));

        Assert.Contains(
            packable,
            violation => violation.Contains(
                "does not carry the literal element IsPackable false", StringComparison.Ordinal));
    }

    [Fact]
    public void N13_Rejects_An_Advertised_Row_And_An_Advertised_Closure()
    {
        var advertised = ArchitectureRules.N13(
            Harness,
            [Shell(Harness, packageId: null, rawText: NonPackable)],
            [
                new CompositionRules.Row(Harness, "advertised", ["broiler.javascript"], ["Broiler.VM.Profile.JavaScript"]),
                new CompositionRules.Row(
                    "Broiler.VM.Composition.JavaScript.ExecutionOnly",
                    "advertised",
                    ["broiler.javascript"],
                    ["Broiler.VM.Profile.JavaScript"],
                    [Harness]),
            ],
            [],
            []);

        Assert.Contains(
            advertised,
            violation => violation.Contains(
                Harness + " is registered as an advertised composition", StringComparison.Ordinal));

        Assert.Contains(
            advertised,
            violation => violation.Contains(
                "the advertised composition Broiler.VM.Composition.JavaScript.ExecutionOnly declares " +
                Harness,
                StringComparison.Ordinal));
    }

    [Fact]
    public void N13_Rejects_A_Published_Closure_That_Ships_The_Harness()
    {
        // A closure report is the strongest of the five: it says a published image ALREADY contains
        // the ingestion path, whatever the project files currently say.
        var shipped = ArchitectureRules.N13(
            Harness,
            [Shell(Harness, packageId: null, rawText: NonPackable)],
            [],
            [
                ("Broiler.VM.Composition.JavaScript.ExecutionOnly",
                    new CompositionRules.ClosureMode("trimmed", ["Broiler.VM.Runtime", Harness])),
            ],
            []);

        Assert.Contains(
            shipped,
            violation => violation.Contains(
                "Broiler.VM.Composition.JavaScript.ExecutionOnly [trimmed] ships " + Harness,
                StringComparison.Ordinal));

        // ...and the harness's OWN closure is not a violation, which is the whole point of the
        // phrasing: a root publishes a closure of its own, so a rule saying "no published closure"
        // would be falsified by the evidence this milestone retains.
        Assert.DoesNotContain(
            ArchitectureRules.N13(
                Harness,
                [Shell(Harness, packageId: null, rawText: NonPackable)],
                [],
                [(Harness, new CompositionRules.ClosureMode("trimmed", ["Broiler.VM.Runtime", Harness]))],
                []),
            violation => violation.Contains("ships " + Harness, StringComparison.Ordinal));
    }

    /// <summary>A minimal project file standing in for one clause's subject.</summary>
    /// <remarks>
    /// Constructed rather than stored, because these three clauses read one property each and a
    /// whole second copy of a real project file would make the witness harder to read than the
    /// rule. The two clauses whose subject is a FILE - a reference and a content item - have
    /// stored witnesses instead, because a file is what they are about.
    /// </remarks>
    private static ComponentGraph.ProjectFile Shell(string assembly, string? packageId, string rawText) =>
        new(
            Path: assembly + ".csproj",
            RelativePath: "src/compositions/" + assembly + "/" + assembly + ".csproj",
            IsWitness: false,
            AssemblyName: assembly,
            RootNamespace: assembly,
            PackageId: packageId,
            OutputType: "Exe",
            RawText: rawText,
            ProjectReferences: [],
            PackageReferences: [],
            InternalsVisibleTo: [],
            SourceItemPaths: []);

    /// <summary>A project file that satisfies the packability clause and nothing else.</summary>
    /// <remarks>
    /// The three clauses below are about a row, a closure and a reference, so their subject has to
    /// be clean on the two clauses they are not testing. A shell that violated everything would let
    /// each of them pass on somebody else's violation.
    /// </remarks>
    private const string NonPackable = "<Project><IsPackable>false</IsPackable></Project>";
}
