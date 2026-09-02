namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group A's fifteenth rule: the topology record's budget still describes the graph.
/// </summary>
public sealed class TopologyBudgetTests
{
    private static string Record { get; } = File.ReadAllText(Path.Combine(
        ComponentGraph.Root,
        TopologyBudgetRules.Record.Replace('/', Path.DirectorySeparatorChar)));

    private static int Projects => ComponentGraph.Projects.Count;

    private static int Edges =>
        ComponentGraph.Projects.Sum(static project => project.ProjectReferences.Count);

    [Fact]
    public void A15_The_Topology_Records_Last_Budget_Describes_The_Graph()
    {
        // Non-vacuous: the record really does state budgets, and more than one, so the rule reads
        // a last one rather than the only one. A record that had stopped stating them would make
        // the clean direction true by having nothing to compare.
        Assert.True(
            TopologyBudgetRules.Budgets(Record).Count >= 5,
            "the topology record states fewer budgets than its revision history has "
            + "project-adding revisions, so this rule is reading the wrong sentences");

        Assert.Empty(TopologyBudgetRules.A15(Record, Projects, Edges));
    }

    [Fact]
    public void A15_Rejects_A_Record_Whose_Budget_Stopped_At_The_Previous_Revision()
    {
        // The real defect. The revision that added the Android composition root carried no budget
        // paragraph, so the record's last stated size was 19 projects and 55 edges while the graph
        // held 20 and 59, and nothing in the suite could see it.
        var reported = TopologyBudgetRules.A15(
            Witness("A15-a-budget-that-stopped-at-the-previous-revision.md.witness"),
            projects: 20,
            edges: 59).ToArray();

        Assert.Contains(reported, message =>
            message.Contains("states 19 projects and the graph holds 20", StringComparison.Ordinal));
        Assert.Contains(reported, message =>
            message.Contains("states 55 edges and the graph holds 59", StringComparison.Ordinal));
    }

    [Fact]
    public void A15_Rejects_A_Record_That_States_No_Budget_At_All()
    {
        // Not the same defect and not covered by the one above: the failure that produced this
        // rule was an ABSENT paragraph, and a rule reading "no sentence, nothing to disagree
        // with" would have been green on exactly the tree it was minted for.
        var reported = Assert.Single(TopologyBudgetRules.A15(
            "A record with prose and no budget sentence anywhere in it.",
            projects: 20,
            edges: 59));

        Assert.Contains("states no project budget at all", reported, StringComparison.Ordinal);
    }

    [Fact]
    public void A15_Reads_The_Last_Budget_And_Not_The_First()
    {
        // Every earlier sentence is history - "goes from 8 projects to 12" was true at VM-3 and
        // must stay written - so a rule checking all of them would demand the record forget what
        // it recorded.
        var budgets = TopologyBudgetRules.Budgets(Record);

        Assert.True(budgets.Count > 1, "the record states one budget, so 'last' means nothing");
        Assert.NotEqual(budgets[0].Projects, budgets[^1].Projects);
        Assert.Equal(Projects, budgets[^1].Projects);
    }

    [Fact]
    public void A15_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            static rule => string.Equals(rule.Id, "A15", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("last", row.Statement, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Writes what A15 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_A15_Are_Written_When_Asked_For()
    {
        RuleReport.Write("A15", [("A15", () => TopologyBudgetRules.A15(Record, Projects, Edges))]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "A15.txt")),
                "a report for A15 was asked for and none was written");
        }
    }

    private static string Witness(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "adr", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return File.ReadAllText(path);
    }
}
