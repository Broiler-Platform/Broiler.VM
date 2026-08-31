namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The JavaScript profile's published diagnostic registry, held to the four things it claims to
/// be bound to.
/// </summary>
/// <remarks>
/// <para>
/// Every test here asserts both directions: the checkout is clean, and the rule rejects an input
/// that violates it. A rule that has never rejected anything is a shape, and the registry is
/// exactly the kind of artefact whose rules can look strong while quantifying over nothing.
/// </para>
/// <para>
/// The rejecting direction is a witness input on disk wherever a reader would want to open one,
/// and the real artefact with one thing altered wherever the point is that the REAL registry would
/// fail. The two are not interchangeable: a small witness shows the rule's shape, and a doctored
/// copy of the real file shows the rule reaches the file that ships.
/// </para>
/// </remarks>
public sealed class DiagnosticRegistryRuleTests
{
    private static readonly IReadOnlyList<DiagnosticRegistryRow> Registry = DiagnosticRegistry.Rows;

    private static readonly IReadOnlyList<(string Name, int Value)> Vocabulary =
        DiagnosticRegistry.Vocabulary(
            AssuranceSources.File(DiagnosticRegistry.VocabularyPath).Tree);

    private static readonly IReadOnlyList<DiagnosticRegistry.EmissionSite> Sites =
        DiagnosticRegistry.EmissionSites(AssuranceSources.Files
            .Where(static file => file.Assembly == "Broiler.VM.Profile.JavaScript"));

    [Fact]
    public void N5_The_Registry_And_The_Code_Vocabulary_Are_The_Same_Set()
    {
        Assert.Empty(ArchitectureRules.N5(Registry, Vocabulary, DiagnosticRegistry.Revision));

        // Non-vacuous: forty declared codes and forty published rows, so a clean result is a
        // comparison over a real set rather than over an empty one.
        Assert.Equal(40, Vocabulary.Count);
        Assert.Equal(Vocabulary.Count, Registry.Count);
        Assert.Equal(1, DiagnosticRegistry.Revision);

        var witness = Witness("N5-registry-omits-a-declared-code.txt.witness");

        var violations = ArchitectureRules
            .N5(DiagnosticRegistry.Read(witness), Vocabulary, DiagnosticRegistry.ReadRevision(witness))
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "declares UnsupportedFormatVersion = 1002 and the registry has no row for it",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "names UnsupportedFormatVersionExtended, which is not a member of that number",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "dates from revision 3, and the registry is at revision 2", StringComparison.Ordinal));

        // And the real registry with its revision line removed reports the omission rather than
        // treating an unversioned registry as a versioned one that happens to say nothing.
        Assert.Contains(
            ArchitectureRules.N5(Registry, Vocabulary, revision: -1),
            violation => violation.Contains("states no revision of its own", StringComparison.Ordinal));
    }

    [Fact]
    public void N6_Every_Code_Maps_Onto_Exactly_One_Core_Reason()
    {
        Assert.Empty(ArchitectureRules.N6(Registry, Sites, Vocabulary));

        // Non-vacuous: the sites are read out of the profile's own sources, and there are more of
        // them than there are codes, so the one-reason clause has something to compare.
        Assert.True(Sites.Count > Vocabulary.Count);
        Assert.DoesNotContain(Sites, static site => site.Reason == "(none)");

        var witness = Witness("N6-registry-names-the-wrong-reason.txt.witness");

        var violations = ArchitectureRules
            .N6(DiagnosticRegistry.Read(witness), Sites, [])
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "names the reason MalformedBytes, which the core does not have", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "emits UnknownOpcode with UnknownFeature, and the registry says SemanticValidationFailed",
            StringComparison.Ordinal));

        // A code emitted with two reasons is the defect that would make a corpus triple stop
        // identifying what happened, and no witness file can carry it: it is a property of the
        // SITES, so it is injected into the site list rather than into a registry.
        var doubled = Sites
            .Append(new DiagnosticRegistry.EmissionSite(
                DiagnosticRegistry.VocabularyPath, 1, "WrongMagic", "InconsistentStructure"))
            .ToArray();

        Assert.Contains(
            ArchitectureRules.N6(Registry, doubled, Vocabulary),
            violation => violation.Contains(
                "WrongMagic is emitted with 2 reasons", StringComparison.Ordinal));

        // And a declared code nothing emits is reported, because a vocabulary may grow a member
        // before anything can produce it.
        Assert.Contains(
            ArchitectureRules.N6(Registry, Sites, Vocabulary.Append(("NeverEmitted", 1999)).ToArray()),
            violation => violation.Contains(
                "NeverEmitted is declared and no site emits it", StringComparison.Ordinal));
    }

    [Fact]
    public void N7_Every_Registry_Row_Is_Reachable_From_A_Named_Case()
    {
        var corpus = DiagnosticRegistry.CorpusCases(DiagnosticRegistry.CorpusText);

        Assert.Empty(ArchitectureRules.N7(Registry, corpus));

        // Non-vacuous, and the figure that matters: thirty-seven of the forty rows are reached by
        // a retained corpus entry and three are not, which is the count rule N7 fixes rather than
        // the registry.
        Assert.Equal(3, ArchitectureRules.DefensiveCodes.Length);
        Assert.Equal(
            37,
            Registry.Count(static row => row.Reachability == "corpus"));

        var witness = Witness("N7-registry-names-a-case-the-corpus-does-not-have.txt.witness");

        var violations = ArchitectureRules
            .N7(DiagnosticRegistry.Read(witness), corpus)
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "names the case a-corpus-entry-nobody-wrote, and no corpus entry of that name records code 1001",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "names the case wrong-magic, and no corpus entry of that name records code 1401",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "claims to be unreachable and is not one of the rows this rule admits",
            StringComparison.Ordinal));

        // The other direction of the same clause: a row this rule expects to be unreachable that
        // claims a case instead. A row that quietly became reachable is good news and still has to
        // be recorded, because the rule's own list is what a reader trusts.
        var promoted = Registry
            .Select(static row => row.Code == 1903 ? row with { Reachability = "corpus" } : row)
            .ToArray();

        Assert.Contains(
            ArchitectureRules.N7(promoted, corpus),
            violation => violation.Contains(
                "is admitted as unreachable and claims to be reachable", StringComparison.Ordinal));
    }

    [Fact]
    public void N8_The_Restated_Codes_Agree_With_The_Registry()
    {
        var mirror = DiagnosticRegistry.Mirror(Parse(DiagnosticRegistry.MirrorPath));

        Assert.Empty(ArchitectureRules.N8(Registry, mirror));

        // Non-vacuous: the producer restates the codes its corpus pins, and there are enough of
        // them that agreement is a comparison.
        Assert.True(mirror.Count >= 37);

        var violations = ArchitectureRules
            .N8(Registry, DiagnosticRegistry.Mirror(ParseWitness("N8-the-mirror-disagrees-with-the-registry.cs.witness")))
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "restates UnknownOpcode = 1499, and the registry publishes it as 1401",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "restates SomethingNobodyPublished = 1601, and the registry has no row of that name",
            StringComparison.Ordinal));
    }

    [Fact]
    public void N9_A_Position_Is_Built_In_One_Place()
    {
        var profile = AssuranceSources.Files
            .Where(static file => file.Assembly == "Broiler.VM.Profile.JavaScript")
            .ToArray();

        var producers = DiagnosticRegistry.PositionProducers(profile);

        Assert.Empty(ArchitectureRules.N9(
            producers, DiagnosticRegistry.NamedPositionConstructions(profile)));

        // Non-vacuous: the assembly does answer with positions, from more than one member, and
        // exactly one file builds them.
        Assert.True(producers.Count > 1);
        Assert.Equal(
            [DiagnosticRegistry.PositionPath],
            producers.Where(static producer => producer.Constructs)
                .Select(static producer => producer.File)
                .Distinct(StringComparer.Ordinal)
                .ToArray());

        var witness = WitnessFile("N9-a-position-built-outside-the-factory.cs.witness");

        var violations = ArchitectureRules
            .N9(
                DiagnosticRegistry.PositionProducers([witness]),
                DiagnosticRegistry.NamedPositionConstructions([witness]))
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "builds a VmSourcePosition in At", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "names VmSourcePosition in a construction outside", StringComparison.Ordinal));

        // And the vacuity clause: a run in which nothing builds a position at all reports itself
        // rather than passing, because "no violations" over an empty set is not a clean result.
        Assert.Contains(
            ArchitectureRules.N9([], []),
            violation => violation.Contains("builds no position at all", StringComparison.Ordinal));
    }

    private static string Witness(string fileName) => File.ReadAllText(WitnessPath(fileName));

    private static AssuranceSourceFile WitnessFile(string fileName) =>
        AssuranceSources.ReadFile(WitnessPath(fileName), "Broiler.VM.Profile.JavaScript");

    private static Microsoft.CodeAnalysis.SyntaxTree ParseWitness(string fileName) =>
        Parse(Path.GetRelativePath(ComponentGraph.Root, WitnessPath(fileName)).Replace('\\', '/'));

    private static Microsoft.CodeAnalysis.SyntaxTree Parse(string relativePath)
    {
        var path = Path.Combine(
            ComponentGraph.Root, relativePath.Replace('/', Path.DirectorySeparatorChar));

        return AssuranceSources.Parse(File.ReadAllText(path), path);
    }

    private static string WitnessPath(string fileName) => Path.Combine(
        ComponentGraph.Root,
        "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "diagnostics", fileName);
}
