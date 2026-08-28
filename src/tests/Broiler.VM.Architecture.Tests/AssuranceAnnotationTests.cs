namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The two-line block, the state machine it resolves to, and the one transition the generator is
/// forbidden to perform.
/// </summary>
/// <remarks>
/// The states are asserted on synthesized source rather than on the checkout, because the
/// checkout is entirely in one state - <c>HUMAN_PENDING</c> - and a table tested only at the row
/// it happens to occupy is a table nobody has tested.
/// </remarks>
public sealed class AssuranceAnnotationTests
{
    /// <summary>A probe method whose fingerprint every case below is stated against.</summary>
    private static string Probe(string ai, string human) => $$"""
        namespace Probe;

        public sealed class Subject
        {
            // {{ai}}
            // {{human}}
            public int Measure(int declared)
            {
                var scaled = declared * 3;

                return scaled > 100 ? 100 : scaled;
            }
        }
        """;

    private const string Member = "Measure";

    private static AssuranceUnit Unit(string ai, string human) =>
        AssuranceProbe.Named(Probe(ai, human), Member);

    private static string Current => Unit(
        "Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
        "Broiler-Human:        PENDING").Fingerprint;

    // ---- Parsing --------------------------------------------------------------------------------

    [Fact]
    public void The_Block_Parses_Into_Fields_And_A_Human_Body()
    {
        var annotation = Unit(
            "Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=TBF",
            "Broiler-Human:        PENDING").Annotation;

        Assert.NotNull(annotation);
        Assert.Equal("AI", annotation!.Field("Origin"));
        Assert.Equal("ADR-0007 s6", annotation.Field("Spec"));
        Assert.Equal("Low", annotation.Field("IP"));
        Assert.Equal("High", annotation.Field("Security"));
        Assert.Equal("7", annotation.Field("Resources"));
        Assert.Equal(AssuranceFingerprint.ToBeFilled, annotation.RecordedFingerprint);
        Assert.True(annotation.HumanIsPending);
        Assert.Null(annotation.Reviewer);
        Assert.Empty(annotation.VocabularyProblems());

        // The two lines are adjacent, in that order. Nothing else is a block.
        Assert.Equal(annotation.AiLine + 1, annotation.HumanLine);
    }

    [Fact]
    public void Every_Field_Value_Is_Held_To_Its_Closed_Vocabulary()
    {
        foreach (var (line, expected) in new[]
                 {
                     ("Origin=Invented; IP=Low; Security=Low; Resources=1; Fingerprint=TBF", "Origin=Invented"),
                     ("Origin=AI; IP=Negligible; Security=Low; Resources=1; Fingerprint=TBF", "IP=Negligible"),
                     ("Origin=AI; IP=Low; Security=Severe; Resources=1; Fingerprint=TBF", "Security=Severe"),
                     ("Origin=AI; IP=Low; Security=Low; Resources=11; Fingerprint=TBF", "Resources=11"),
                     ("Origin=AI; IP=Low; Security=Low; Resources=x; Fingerprint=TBF", "Resources=x"),
                     ("Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=zz", "Fingerprint=zz"),
                     ("Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF; Mood=Calm", "Mood=Calm"),
                 })
        {
            var problems = Unit("Broiler-AI:           " + line, "Broiler-Human:        PENDING")
                .Annotation!
                .VocabularyProblems()
                .ToArray();

            Assert.Contains(problems, problem => problem.StartsWith(expected, StringComparison.Ordinal));
        }

        // And a missing required field is a problem in its own right, so a line cannot pass by
        // saying less.
        Assert.Contains(
            "no Security field",
            Unit("Broiler-AI:           Origin=AI; IP=Low; Resources=1; Fingerprint=TBF", "Broiler-Human:        PENDING")
                .Annotation!
                .VocabularyProblems());
    }

    [Fact]
    public void A_Block_That_Is_Not_Two_Adjacent_Lines_Is_Not_A_Block()
    {
        const string Separated = """
            namespace Probe;

            public sealed class Subject
            {
                // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF

                // Broiler-Human:        PENDING
                public int Measure(int declared) { return declared + 1; }
            }
            """;

        var file = AssuranceProbe.Source(Separated);
        var units = AssuranceScanner.Scan(file);

        Assert.Null(units.Single(static unit => unit.Name.EndsWith("Measure(int)", StringComparison.Ordinal)).Annotation);

        // And the stranded lines are reported rather than ignored: an annotation attached to
        // nothing is worse than none, because it looks like coverage.
        Assert.NotEmpty(AssuranceScanner.OrphanAnnotations(file, units));
    }

    // ---- The state machine ----------------------------------------------------------------------

    [Fact]
    public void The_State_Machine_Resolves_Every_Row_Of_The_Table()
    {
        var current = Current;

        Assert.Equal(AssuranceReviewState.AiAssessed, State(
            "Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
            "Broiler-Human:        PENDING"));

        Assert.Equal(AssuranceReviewState.HumanPending, State(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={current}",
            "Broiler-Human:        PENDING"));

        Assert.Equal(AssuranceReviewState.HumanApprovedPendingFingerprint, State(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={current}",
            "Broiler-Human:        RV; Fingerprint=TBF"));

        Assert.Equal(AssuranceReviewState.Verified, State(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={current}",
            $"Broiler-Human:        RV; Fingerprint={current}"));

        Assert.Equal(AssuranceReviewState.Stale, State(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={current}",
            "Broiler-Human:        RV; Fingerprint=000000"));

        Assert.Equal(AssuranceReviewState.Stale, State(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={current}",
            "Broiler-Human:        STALE; Previous=RV@000000"));

        // NEW is the shape with no AI line at all, which no probe with a block can produce.
        Assert.Equal(
            AssuranceReviewState.New,
            AssuranceProbe
                .Named(
                    "namespace Probe;\n\npublic sealed class Subject\n{\n" +
                    "    public int Measure(int declared)\n    {\n        var scaled = declared * 3;\n\n" +
                    "        return scaled > 100 ? 100 : scaled;\n    }\n}\n",
                    Member)
                .State);

        static AssuranceReviewState State(string ai, string human) => Unit(ai, human).State;
    }

    [Fact]
    public void Only_Verified_And_Exempt_Clear_A_Release()
    {
        var blocking = Enum.GetValues<AssuranceReviewState>()
            .Where(AssuranceStateMachine.BlocksRelease)
            .Select(AssuranceStateMachine.Name)
            .OrderBy(static name => name, StringComparer.Ordinal);

        Assert.Equal(
            new[] { "AI_ASSESSED", "HUMAN_APPROVED_PENDING_FINGERPRINT", "HUMAN_PENDING", "NEW", "STALE" },
            blocking);
    }

    // ---- What the generator may and may not write ------------------------------------------------

    [Fact]
    public void The_Generator_Fills_TBF_With_The_Current_Fingerprint()
    {
        var source = Probe(
            "Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=TBF",
            "Broiler-Human:        PENDING");

        Assert.Equal(
            "// Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; " +
            $"Fingerprint={Current}",
            AssuranceProbe.AnnotationLine(source, ".Measure(int)", AssuranceAnnotation.AiMarker));
    }

    [Fact]
    public void The_Generator_Refreshes_A_Review_The_Code_Has_Outrun_Into_Stale_With_Its_History()
    {
        var source = Probe(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={Current}",
            "Broiler-Human:        RV; Fingerprint=7A91C2");

        // The reviewer and the version they approved are preserved rather than deleted: "this was
        // reviewed, and the current code is not that" is the useful sentence.
        Assert.Equal(
            "// Broiler-Human:        STALE; Previous=RV@7A91C2",
            AssuranceProbe.AnnotationLine(source, ".Measure(int)", AssuranceAnnotation.HumanMarker));

        // And a line the generator already made stale is left exactly as it is: STALE never
        // becomes VERIFIED by anything automated.
        var already = Probe(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={Current}",
            "Broiler-Human:        STALE; Previous=RV@7A91C2");

        Assert.Equal(
            "// Broiler-Human:        STALE; Previous=RV@7A91C2",
            AssuranceProbe.AnnotationLine(already, ".Measure(int)", AssuranceAnnotation.HumanMarker));
    }

    [Fact]
    public void The_Generator_Fills_A_Human_Approval_That_A_Human_Already_Made()
    {
        var source = Probe(
            $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={Current}",
            "Broiler-Human:        RV; Fingerprint=TBF");

        Assert.Equal(
            $"// Broiler-Human:        RV; Fingerprint={Current}",
            AssuranceProbe.AnnotationLine(source, ".Measure(int)", AssuranceAnnotation.HumanMarker));
    }

    [Fact]
    public void The_Generator_Never_Turns_Pending_Into_A_Reviewer()
    {
        // The policy's hardest rule, asserted at the one place that could break it. PENDING in,
        // PENDING out, whatever the fingerprint says.
        foreach (var ai in new[]
                 {
                     "Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
                     $"Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint={Current}",
                     "Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=000000",
                 })
        {
            Assert.Equal(
                "// Broiler-Human:        PENDING",
                AssuranceProbe.AnnotationLine(Probe(ai, "Broiler-Human:        PENDING"), ".Measure(int)", AssuranceAnnotation.HumanMarker));
        }
    }

    [Fact]
    public void The_Generator_Refuses_To_Write_A_Reviewer_The_Source_Does_Not_Carry()
    {
        // The guard is the enforcement, not the four branches above: a future edit that reached
        // for a name has to get past this, and it throws rather than writing.
        var unit = Unit(
            "Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
            "Broiler-Human:        PENDING");

        var refused = Assert.Throws<InvalidOperationException>(() =>
            AssuranceGenerator.RefuseInventedApproval(unit, "PENDING", "EB; Fingerprint=7A91C2"));

        Assert.Contains("Only a human may create an approval", refused.Message, StringComparison.Ordinal);

        // Substituting one reviewer for another is the same offence and is caught the same way.
        Assert.Throws<InvalidOperationException>(() =>
            AssuranceGenerator.RefuseInventedApproval(unit, "RV; Fingerprint=TBF", "EB; Fingerprint=7A91C2"));

        // What the generator IS allowed to do passes it: carrying a reviewer into the stale
        // record, and filling a fingerprint beside a name the source already had.
        AssuranceGenerator.RefuseInventedApproval(unit, "RV; Fingerprint=7A91C2", "STALE; Previous=RV@7A91C2");
        AssuranceGenerator.RefuseInventedApproval(unit, "RV; Fingerprint=TBF", "RV; Fingerprint=7A91C2");
        AssuranceGenerator.RefuseInventedApproval(unit, "PENDING", "PENDING");
    }
}
