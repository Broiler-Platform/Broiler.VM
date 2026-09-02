namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group J's twelfth rule: the register cites the assurance figures rather than restating them.
/// </summary>
/// <remarks>
/// <para>
/// <b>Minted 2026-09-02, for a defect that had already happened.</b> Rule J10's register row said
/// the rule was red and named 41 units of outstanding work while the generated report in the same
/// repository said <c>| Required and missing | 0 |</c>. Bundle JS-ANDROID-013 corrected the row and
/// recorded, as an exclusion, that the correction was prose and nothing compared it to the tree on
/// any later day. This is the rule that exclusion named.
/// </para>
/// <para>
/// <b>It is in group J because its subject is the assurance record</b>, which ADR 0012 owns: the
/// figures it reads are the ones the generator writes, and the reason a register row may not carry
/// its own copy is the reason rule J5 forbids a hand-maintained <c>Human-reviewed: 47/47</c>. It
/// is not in the register's own unnumbered tests, because those hold the register's SHAPE - a row
/// has a witness, a Deferred row names a later milestone - and this holds its CONTENT against
/// something outside it.
/// </para>
/// </remarks>
public sealed class RegisterFigureTests
{
    /// <summary>The generated component report, which is where the figures live.</summary>
    private static string Report { get; } =
        File.ReadAllText(Path.Combine(ComponentGraph.Root, "CODE-ASSURANCE.md"));

    /// <summary>Every register row as one block of prose, which is what this rule reads.</summary>
    private static IReadOnlyList<(string Id, string Text)> Rows { get; } =
        RuleRegisterTests.Loaded.Rules
            .Select(static rule => (
                rule.Id,
                // Newline-joined, not space-joined: the fields are separate prose and a claim
                // must not be able to form across the seam between two of them. Rule V12's
                // evidence ends "the five profile-facing contracts" and its next field opens
                // "All five contracts exist", which read as one sentence when joined by a space.
                Text: string.Join(
                    "\n", rule.Statement, rule.Evidence, rule.NonVacuousWhen,
                    rule.PermanenceReason ?? string.Empty)))
            .ToArray();

    [Fact]
    public void J12_The_Register_Cites_The_Assurance_Figures_Rather_Than_Restating_Them()
    {
        var figures = RegisterFigureRules.Figures(Report);

        // Non-vacuous: the report really does define the three metrics, so a clean result is a
        // comparison rather than a quantifier over an empty set. A report that had stopped
        // carrying the table would make every clause below silently true.
        Assert.True(
            figures.Count >= RegisterFigureRules.ReportMetrics.Count + 7,
            $"the figure catalog resolved only {figures.Count} figures, so a citation could go " +
            "unresolved for want of a catalog entry rather than for want of a metric");

        // ...and the register really does cite them, so clause one has something to resolve.
        Assert.NotEmpty(Rows.Where(static row =>
            row.Text.Contains("{criteria:", StringComparison.Ordinal)));

        Assert.Empty(RegisterFigureRules.UnresolvableCitations(Rows, figures));
        Assert.Empty(RegisterFigureRules.RestatedFigures(Rows));
        Assert.Empty(RegisterFigureRules.OutstandingClaims(Rows, figures["criteria:missing"]));
        Assert.Empty(RegisterFigureRules.UncitedExistenceClaims(Rows));
        Assert.Empty(RegisterFigureRules.UncitedSubjectCounts(Rows));
    }

    [Fact]
    public void J12_Rejects_A_Citation_The_Report_Cannot_Resolve()
    {
        var reported = Assert.Single(RegisterFigureRules.UnresolvableCitations(
            Witness("J12-a-row-cites-a-metric-that-is-gone"),
            RegisterFigureRules.Figures(Report)));

        Assert.Contains("{criteria:unitsthatwerenevercounted}", reported, StringComparison.Ordinal);
    }

    [Fact]
    public void J12_Rejects_A_Row_That_Types_The_Figure_Back_In()
    {
        // The direction that matters. A row carrying no citation at all is the state J10's row was
        // in when it went stale, so a rule that only checked citations would have been green over
        // the defect it was minted for.
        var reported = RegisterFigureRules.RestatedFigures(
            Witness("J12-a-row-restates-a-figure")).ToArray();

        // All three shapes, named separately, because a bare non-empty check would pin whichever
        // fired first and two of the three would then be free to stop working.
        Assert.Contains(reported, message =>
            message.Contains("counts Forty-four units", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("counts 3 units", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("counts 41 units", StringComparison.Ordinal));

        // ...and exactly three. The witness carries four further sentences that count something
        // else while naming criteria - artefacts, comment lines, the clauses of a publish gate -
        // and the first version of this rule reported all of them, because it tested for a figure
        // and a criteria word in one sentence rather than for a figure counting units.
        Assert.Equal(3, reported.Length);
    }

    [Fact]
    public void J12_Rejects_A_Row_That_Counts_The_Tree_By_Hand()
    {
        var reported = RegisterFigureRules.UncitedExistenceClaims(
            Witness("J12-a-row-counts-the-tree-by-hand")).ToArray();

        // The three real shapes, each named. "Five test-only projects" and "Eight edges" were
        // both wrong when this rule was minted - there are nine and fifty-nine - and 689/903/1592
        // were three wrong figures in one sentence.
        Assert.Contains(reported, message =>
            message.Contains("that Five of something exist", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("that Eight of something exist", StringComparison.Ordinal));
        // The third names 903 rather than 689, and that is the rule working as written: the
        // figure it reports is the one ADJACENT to "exist". A sentence carrying several figures
        // is flagged once, on the nearest, and a reader converting the row converts all of them.
        // The limit is real and it is in the register row: this clause reports a sentence, not
        // every number in it.
        Assert.Contains(reported, message =>
            message.Contains("that 903 of something exist", StringComparison.Ordinal));

        // ...and nothing else. An ADR number, a revision number and a bare "the rule exists" are
        // in the witness precisely because a rule reading them as counts would ask the register to
        // cite a document's name.
        Assert.Equal(3, reported.Length);
    }

    [Fact]
    public void J12_Rejects_A_Row_That_Binds_A_Figure_To_A_Countable_Subject()
    {
        var reported = RegisterFigureRules.UncitedSubjectCounts(
            Witness("J12-a-row-binds-a-figure-to-a-noun")).ToArray();

        // The three sentences the register actually carried, each stale by hundreds.
        Assert.Contains(reported, message =>
            message.Contains("\"45 covered product source files\"", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("\"Forty-eight artefacts\"", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("\"689 annotations\"", StringComparison.Ordinal));

        // ...and nothing else. The witness carries a chosen witness set, a count of a rule's own
        // clauses and a record number, none of which measures the tree. A vocabulary that read
        // those would be asking the register to stop explaining itself.
        Assert.Equal(3, reported.Length);
    }

    /// <summary>
    /// Every countable subject the fifth clause names is one the catalog can answer for.
    /// </summary>
    /// <remarks>
    /// The discipline that keeps the clause repairable: it reports a figure bound to one of these
    /// subjects, and the fix is a citation that already exists. A subject with no metric would
    /// make the rule report a sentence nobody could repair, which is a rule that has to be
    /// suppressed rather than obeyed.
    /// </remarks>
    [Fact]
    public void J12_Every_Countable_Subject_Has_A_Figure_Behind_It()
    {
        var figures = RegisterFigureRules.Figures(Report);

        Assert.NotEmpty(RegisterFigureRules.CountableSubjects);
        Assert.All(
            RegisterFigureRules.CountableSubjects,
            subject => Assert.True(
                figures.Count > 0,
                $"no figure in the catalog can answer for '{subject}'"));

        // The catalog answers for the subjects this register actually counts: files, units,
        // artefacts, assemblies, edges, roots, ADRs and review documents.
        foreach (var metric in new[]
        {
            "assurance:files", "assurance:units", "assurance:relevant", "assurance:exempt",
            "assurance:artefacts", "graph:packable", "graph:javascript-family", "graph:edges",
            "graph:composition-roots", "docs:adrs", "review:documents",
        })
        {
            Assert.True(figures.ContainsKey(metric), $"the catalog defines no {metric}");
        }
    }

    /// <summary>
    /// A quotation is not a claim, and the exemption that says so is narrow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the clause that was reconsidered rather than worked around.</b> Rule J12's row
    /// fired on itself three times and rule A15's once, always for quoting the defect the rule was
    /// minted for, and the repair each time was to paraphrase the quotation until its shape was
    /// gone. Four repairs is a design, not a coincidence: a rule that forbids a sentence shape has
    /// to give authors a way to SHOW that shape, or every row explaining why a rule exists pays
    /// for it in evidence.
    /// </para>
    /// <para>
    /// <b>The exemption is the one place here where a miss is unsafe</b>, so it needs both halves:
    /// an attributing verb from a short list AND a code span. The witness carries three
    /// near-misses with one half each, and all three must still be reported.
    /// </para>
    /// </remarks>
    [Fact]
    public void J12_Does_Not_Report_An_Attributed_Quotation_And_Still_Reports_Every_Near_Miss()
    {
        var reported = RegisterFigureRules.UncitedSubjectCounts(
            Witness("J12-a-quotation-and-three-things-that-are-not-one")).ToArray();

        // Three near-misses: a code span with no verb, a verb with no code span, and a verb too
        // far from its span. The attributed quotation is the fourth occurrence and is not here.
        Assert.Equal(3, reported.Length);

        foreach (var figure in new[] { "12", "78", "93" })
        {
            Assert.Contains(reported, message => message.Contains(
                figure + " covered source files", StringComparison.Ordinal));
        }

        // ...and the quoted one is absent, which is the whole point.
        Assert.DoesNotContain(reported, message =>
            message.Contains("45 covered source files", StringComparison.Ordinal));
    }

    [Fact]
    public void J12_Rejects_A_Row_Claiming_Work_The_Report_Says_Is_Done()
    {
        var reported = Assert.Single(RegisterFigureRules.OutstandingClaims(
            Witness("J12-a-row-claims-work-that-is-done"), missing: 0));

        Assert.Contains("says falsification criteria are outstanding", reported, StringComparison.Ordinal);
        Assert.Contains("Required and missing | 0", reported, StringComparison.Ordinal);
    }

    [Fact]
    public void J12_Rejects_A_Register_Silent_About_Work_The_Report_Names()
    {
        // The other direction, which is the same failure pointing the other way: the tree owes
        // criteria and the register does not mention it. Driven over the REAL rows, so it is the
        // register as it stands that must answer.
        var reported = Assert.Single(RegisterFigureRules.OutstandingClaims(Rows, missing: 7));

        Assert.Contains(
            "states 7 unit(s) owe a falsification criterion", reported, StringComparison.Ordinal);
    }

    /// <summary>
    /// The vocabularies do not fire on prose that means something else.
    /// </summary>
    /// <remarks>
    /// Rule J9's recogniser was defeated four times by rewording, and the lesson recorded there is
    /// that a vocabulary is a liability until someone checks what it already matches. Both of
    /// these are checked against the register itself: the criteria vocabulary must match some rows
    /// and not all of them, and the outstanding vocabulary must match none, because the tree owes
    /// nothing. A vocabulary matching everything would make clause two fire on every row, and one
    /// matching nothing would make it fire on none.
    /// </remarks>
    [Fact]
    public void J12_The_Vocabularies_Are_Checked_Against_The_Register_They_Read()
    {
        // Every row that DISCUSSES criteria without counting units against them - and there are
        // several, because group J is where the requirement is defined - must be silent. This is
        // the assertion the first version of the rule failed: it reported sixteen figures across
        // five rows, every one of them innocent.
        var discussing = Rows
            .Where(static row =>
                row.Text.Contains("falsification criteri", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.True(discussing.Length >= 4, "too few rows discuss criteria for this to mean much");
        Assert.Empty(RegisterFigureRules.RestatedFigures(discussing));

        // And the outstanding vocabulary claims nothing, because nothing is outstanding. Driven
        // through the rule rather than over the raw text: J12's own row QUOTES the redness
        // sentence it was minted for, and a check reading the raw text would report the quotation.
        Assert.Empty(RegisterFigureRules.OutstandingClaims(Rows, missing: 0));
    }

    [Fact]
    public void J12_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            static rule => string.Equals(rule.Id, "J12", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Equal("0012", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        // The row must state the limit rather than claim it closed the whole class. This rule
        // reads three figures, and a register row can be wrong about anything else with nothing
        // to stop it.
        Assert.Contains("three", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Writes what J12 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_J12_Are_Written_When_Asked_For()
    {
        var figures = RegisterFigureRules.Figures(Report);

        RuleReport.Write("J12",
        [
            ("J12", () => RegisterFigureRules.UnresolvableCitations(Rows, figures)
                .Concat(RegisterFigureRules.RestatedFigures(Rows))
                .Concat(RegisterFigureRules.OutstandingClaims(
                    Rows, figures.TryGetValue("criteria:missing", out var missing) ? missing : 0))
                .Concat(RegisterFigureRules.UncitedExistenceClaims(Rows))
                .Concat(RegisterFigureRules.UncitedSubjectCounts(Rows))),
            // Its own line, because these are NOT violations. A reader auditing the escape hatch
            // needs the list; J12's count needs to stay a count of things wrong.
            ("J12-exempt", () => RegisterFigureRules.ExemptedQuotations(Rows)),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "J12.txt")),
                "a report for J12 was asked for and none was written");
        }
    }

    /// <summary>
    /// A witness register, read as rows rather than parsed as a register.
    /// </summary>
    /// <remarks>
    /// One row per file, because these witnesses exist to be read by a human deciding whether the
    /// rule is right, and a JSON register with one interesting field is a worse read than the
    /// sentence itself.
    /// </remarks>
    private static IReadOnlyList<(string Id, string Text)> Witness(string name)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "register", name + ".md.witness");

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return [(name[..3], File.ReadAllText(path))];
    }
}
