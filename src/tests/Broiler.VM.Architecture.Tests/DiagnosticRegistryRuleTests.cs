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

    /// <summary>
    /// The embedder-seam vocabulary, declared in the lowering assembly since JS-3b.
    /// </summary>
    /// <remarks>
    /// A second fixture rather than a merged one, because the rule's membership check is per half
    /// and a merged list would let a row in the wrong half pass by finding its name in the other
    /// vocabulary.
    /// </remarks>
    private static readonly IReadOnlyList<(string Name, int Value)> SeamVocabulary =
        DiagnosticRegistry.Vocabulary(
            AssuranceSources.File(DiagnosticRegistry.SeamVocabularyPath).Tree,
            DiagnosticRegistry.SeamCodeType);

    private static readonly IReadOnlyList<DiagnosticRegistry.EmissionSite> Sites =
        DiagnosticRegistry.EmissionSites(AssuranceSources.Files
            .Where(static file => file.Assembly == "Broiler.VM.Profile.JavaScript"));

    [Fact]
    public void N5_The_Registry_And_The_Code_Vocabulary_Are_The_Same_Set()
    {
        Assert.Empty(ArchitectureRules.N5(Registry, Vocabulary, SeamVocabulary, DiagnosticRegistry.Revision));

        // Non-vacuous: forty core-result codes and twenty-four embedder-seam ones, sixty-four rows,
        // so a clean result is a comparison over two real sets rather than over an empty one. The
        // seam half was declared and empty at revision 1 and is the half at revision 2.
        Assert.Equal(40, Vocabulary.Count);
        Assert.Equal(24, SeamVocabulary.Count);
        Assert.Equal(Vocabulary.Count + SeamVocabulary.Count, Registry.Count);
        Assert.Equal(2, DiagnosticRegistry.Revision);

        // The two vocabularies live in two assemblies that cannot see each other, so the one thing
        // no compiler could catch is a number used in both. Nothing else in the build reads both
        // of these lists, which is why this assertion is here and not left to the rule's own sweep.
        Assert.Empty(Vocabulary.Select(static member => member.Value)
            .Intersect(SeamVocabulary.Select(static member => member.Value)));

        var witness = Witness("N5-registry-omits-a-declared-code.txt.witness");

        var violations = ArchitectureRules
            .N5(DiagnosticRegistry.Read(witness), Vocabulary, SeamVocabulary,
                DiagnosticRegistry.ReadRevision(witness))
            .ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "declares UnsupportedFormatVersion = 1002 and the registry has no core-result row for it",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "names UnsupportedFormatVersionExtended, which is not a member of that number",
            StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "dates from revision 3, and the registry is at revision 2", StringComparison.Ordinal));

        // And the real registry with its revision line removed reports the omission rather than
        // treating an unversioned registry as a versioned one that happens to say nothing.
        Assert.Contains(
            ArchitectureRules.N5(Registry, Vocabulary, SeamVocabulary, revision: -1),
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
        var sourceCorpus = DiagnosticRegistry.SourceCases(DiagnosticRegistry.SourceCorpusText);

        Assert.Empty(ArchitectureRules.N7(Registry, corpus, sourceCorpus));

        // Non-vacuous, and the figures that matter: thirty-seven of the forty core-result rows are
        // reached by a retained corpus entry and three are not, which is the count rule N7 fixes
        // rather than the registry; and every one of the twenty-two embedder-seam rows JS-3b
        // published is reached by a retained source entry, with none defensive. The seam half has
        // no defensive row on purpose - all three of its format-ceiling codes ARE reachable by a
        // program, and recording them as unreachable would have been recording something untrue to
        // avoid generating three sources.
        Assert.Equal(4, ArchitectureRules.DefensiveCodes.Length);
        Assert.Equal(
            37,
            Registry.Count(static row => row.Reachability == "corpus"));
        Assert.Equal(
            23,
            Registry.Count(static row => row.Reachability == "source"));
        // One seam row is defensive, and which one is the finding: the operand-stack ceiling
        // cannot be reached through this front end, because the parse depth bound refuses at about
        // 170 levels of source and the ceiling needs more than a thousand. The parse bound
        // dominates the stack ceiling, so the code is declared, reachable in principle by another
        // producer, and reached by no source this front end will accept.
        Assert.Equal(
            [2303],
            Registry.Where(static row =>
                    row.Half == "embedder-seam" && row.Reachability == "defensive")
                .Select(static row => row.Code));

        // The seam half's own rejecting direction: a row reached by a source that claims to travel
        // in a core result. Both halves of that pair are wrong together - a rejection of source
        // reaches no envelope - so the rule names the contradiction rather than either half.
        Assert.Contains(
            ArchitectureRules.N7(
                Registry
                    .Select(static row =>
                        row.Code == 2001 ? row with { Half = "core-result" } : row)
                    .ToArray(),
                corpus,
                sourceCorpus),
            violation => violation.Contains(
                "is reached by a source and is not an embedder-seam row", StringComparison.Ordinal));

        // ...and a seam row naming a source entry the retained manifest does not have.
        Assert.Contains(
            ArchitectureRules.N7(
                Registry
                    .Select(static row =>
                        row.Code == 2001 ? row with { Case = "refuse-something-nobody-wrote" } : row)
                    .ToArray(),
                corpus,
                sourceCorpus),
            violation => violation.Contains(
                "names the case refuse-something-nobody-wrote, and no retained source entry of " +
                "that name is refused with UnexpectedCharacter",
                StringComparison.Ordinal));

        var witness = Witness("N7-registry-names-a-case-the-corpus-does-not-have.txt.witness");

        var violations = ArchitectureRules
            .N7(DiagnosticRegistry.Read(witness), corpus, sourceCorpus)
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
            ArchitectureRules.N7(promoted, corpus, sourceCorpus),
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

    /// <summary>
    /// Writes what each registry rule said about this checkout, when asked to.
    /// </summary>
    /// <remarks>
    /// The mechanism lives on <see cref="RuleReport"/>. What is here is these rules' inputs, which
    /// are this class's own statics - the same ones the tests above compare, so the report answers
    /// the question those tests ask rather than a neighbouring one. N1 through N4 are swept over
    /// the project graph and are reported with group A, where that sweep lives; N10 is asserted in
    /// another class by another shape, and the report's scope note names it.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_The_Registry_Rules_Are_Written_When_Asked_For()
    {
        var profile = AssuranceSources.Files
            .Where(static file => file.Assembly == "Broiler.VM.Profile.JavaScript")
            .ToArray();

        RuleReport.Write("N",
        [
            ("N5", () => ArchitectureRules.N5(Registry, Vocabulary, SeamVocabulary, DiagnosticRegistry.Revision)),
            ("N6", () => ArchitectureRules.N6(Registry, Sites, Vocabulary)),
            ("N7", () => ArchitectureRules.N7(
                Registry,
                DiagnosticRegistry.CorpusCases(DiagnosticRegistry.CorpusText),
                DiagnosticRegistry.SourceCases(DiagnosticRegistry.SourceCorpusText))),
            ("N8", () => ArchitectureRules.N8(
                Registry, DiagnosticRegistry.Mirror(Parse(DiagnosticRegistry.MirrorPath)))),
            ("N9", () => ArchitectureRules.N9(
                DiagnosticRegistry.PositionProducers(profile),
                DiagnosticRegistry.NamedPositionConstructions(profile))),
            ("N11", () => ArchitectureRules.N11(
                DiagnosticRegistry.ExhaustionAnswers(profile),
                DiagnosticRegistry.CorpusOutcomes(DiagnosticRegistry.CorpusText),
                Dimensions,
                Scopes)),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "N.txt")),
                "a report for the registry rules was asked for and none was written");
        }
    }

    [Fact]
    public void N12_The_Front_End_Holds_No_State_That_Outlives_A_Call()
    {
        var lowering = AssuranceSources.Files
            .Where(static file => file.Assembly == FrontEndAmbientState.Assembly)
            .ToArray();

        Assert.Empty(ArchitectureRules.N12(FrontEndAmbientState.Sites(lowering), lowering.Length));

        // Non-vacuous, and this rule needs the clause more than most: it passes by finding
        // nothing, so a run over no files would be the cleanest-looking result in the register and
        // would mean the assembly had been renamed out from under it. The scan finds real static
        // declarations in these files - the punctuator table and the reserved-name list among
        // them - which it correctly does not report.
        //
        // FOURTEEN FILES. The prose here said "eleven - JS-1's three and JS-3b's eight" while the
        // figure beside it read 13, so the breakdown was already two revisions stale and is not
        // replaced with a third one that will go the same way: the count is what this clause
        // asserts, and which milestone contributed which file is what the graph manifest is for.
        // The fourteenth is SliceControlFlow, added when the lowering stopped emitting a loop
        // continuation nothing could reach.
        Assert.Equal(14, lowering.Length);
        Assert.Contains(
            ArchitectureRules.N12([], filesScanned: 0),
            violation => violation.Contains(
                "no file of the lowering assembly was scanned", StringComparison.Ordinal));

        // The rejecting direction: four ways a parse's state could outlive its call, in one file.
        var witness = FrontEndAmbientState.Sites(
            [WitnessFile("N12-a-parse-switch-that-outlives-the-call.cs.witness")]);

        var violations = ArchitectureRules.N12(witness, filesScanned: 1).ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "declares Goal, which is a mutable static field", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "declares ScopedGoal, which is a [ThreadStatic] field", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "declares AmbientGoal, which is a field of ambient-context type", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains(
            "declares CurrentGoal, which is a settable static property", StringComparison.Ordinal));

        // ...and the accepting direction inside the same witness, which is the half that stops
        // this being a rule about the `static` keyword: the file's `static readonly` array and its
        // `const` are not reported, because nothing can write either.
        Assert.DoesNotContain(witness, static site => site.Member == "Punctuators");
        Assert.DoesNotContain(witness, static site => site.Member == "DefaultDepth");
        Assert.Equal(4, witness.Count);
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
