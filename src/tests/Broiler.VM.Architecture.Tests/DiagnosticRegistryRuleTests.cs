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

    [Fact]
    public void N11_Every_Dimension_The_Profile_Can_Exhaust_Is_Pinned_By_A_Corpus_Entry()
    {
        var profile = AssuranceSources.Files
            .Where(static file => file.Assembly == "Broiler.VM.Profile.JavaScript")
            .ToArray();

        var answers = DiagnosticRegistry.ExhaustionAnswers(profile);
        var corpus = DiagnosticRegistry.CorpusOutcomes(DiagnosticRegistry.CorpusText);

        Assert.Empty(ArchitectureRules.N11(answers, corpus, Dimensions, Scopes));

        // Non-vacuous, and the figure the clause is about: the profile answers on seven dimensions
        // - the bounded reader's four ceilings, the allocator's bytes, the link stage's work charge
        // and the poll's wall clock - from more sites than that, and seven corpus entries pin them
        // one for one.
        var answered = answers
            .Select(static answer => answer.Dimension)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(7, answered.Count);
        Assert.True(answers.Count > answered.Count);
        Assert.Equal(
            7,
            corpus.Count(static row => row.Outcome == "ResourceExhaustion"));

        // The scopes are held to the core's vocabulary and NOT to the site, because they honestly
        // differ: a reader ceiling is compared inside the verification and answers at Artifact,
        // and an allowance is charged through the meter, which reports the level that refused.
        // Both are in the corpus and a rule that demanded the site's scope would fail on the four
        // that are right.
        Assert.Equal(
            ["Artifact", "Runtime"],
            corpus.Where(static row => row.Outcome == "ResourceExhaustion")
                .Select(static row => row.Scope)
                .Distinct(StringComparer.Ordinal)
                .Order(StringComparer.Ordinal)
                .ToArray());

        var witness = WitnessFile("N11-an-exhaustion-answer-nothing-pins.cs.witness");

        var violations = ArchitectureRules
            .N11(DiagnosticRegistry.ExhaustionAnswers([witness]), corpus, Dimensions, Scopes)
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "answers a resource exhaustion on HostCalls", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "answers with ReadaheadWindow, which is not a member of VmBudgetDimension",
            StringComparison.Ordinal));

        // The other direction, over the real manifest with one thing altered: an entry recording a
        // dimension this profile never answers, and one recording a scope the core does not have.
        // A stale entry is as much a broken binding as a missing one - it records an answer
        // nothing in this component can give.
        var doctored = corpus
            .Select(static row => row.Dimension == "WallClock" ? row with { Dimension = "NestedLoadBytes" } : row)
            .Select(static row => row.Dimension == "SectionCount" ? row with { Scope = "Universe" } : row)
            .ToArray();

        var stale = ArchitectureRules.N11(answers, doctored, Dimensions, Scopes).ToArray();

        Assert.Contains(stale, violation => violation.Contains(
            "records the dimension NestedLoadBytes, and no site in the profile answers on it",
            StringComparison.Ordinal));
        Assert.Contains(stale, violation => violation.Contains(
            "records the scope Universe, which is not a member of VmBudgetScope",
            StringComparison.Ordinal));
        Assert.Contains(stale, violation => violation.Contains(
            "answers a resource exhaustion on WallClock", StringComparison.Ordinal));

        // And the vacuity clause in both halves: a profile that answers nothing and a corpus that
        // pins nothing each report themselves rather than passing.
        Assert.Contains(
            ArchitectureRules.N11([], corpus, Dimensions, Scopes),
            violation => violation.Contains("answers no resource exhaustion at all", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N11(answers, [], Dimensions, Scopes),
            violation => violation.Contains("pins no exhaustion at all", StringComparison.Ordinal));
    }

    /// <summary>
    /// The two core vocabularies an exhaustion answer names one member of each of, read from the
    /// enumerations themselves.
    /// </summary>
    /// <remarks>
    /// Held to the core's own types rather than to a list written here, for the reason rule N6
    /// gives about reason names: this project does reference the contract assembly, so a list would
    /// be a second copy that could disagree with it.
    /// </remarks>
    private static readonly string[] Dimensions = Enum.GetNames<VmBudgetDimension>();

    private static readonly string[] Scopes = Enum.GetNames<VmBudgetScope>();

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
