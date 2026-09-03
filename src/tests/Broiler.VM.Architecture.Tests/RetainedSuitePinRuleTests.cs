namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N15: the retained conformance-suite pin says the same thing in the pin file, its decision
/// record and the ledger.
/// </summary>
/// <remarks>
/// <b>The suite is not in this tree and that is what makes the rule matter more here than for the
/// language edition.</b> The edition's document is archived, so its digest is checked against
/// bytes and a drifted constant is caught by the file itself. A conformance suite of 56,560 files
/// is retained as a pin and nothing else, so the pin IS the artifact: three documents naming three
/// revisions would be three claims with nothing behind any of them.
/// </remarks>
public sealed class RetainedSuitePinRuleTests
{
    private const string PinPath = "src/tests/conformance/pins/test262.pin";

    private const string DecisionPath =
        "src/Broiler.VM.Profile.JavaScript/docs/decisions/" +
        "0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md";

    private const string LedgerPath =
        "src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md";

    [Fact]
    public void N15_The_Retained_Pin_Says_The_Same_Thing_In_Three_Places()
    {
        Assert.Empty(ArchitectureRules.N15(Pin(), Document(DecisionPath), Document(LedgerPath)));

        // Non-vacuous: the rule compares parsed values against text, so a pin file that stopped
        // declaring them would make every clause free.
        Assert.Equal(8, Pin().Count);
        Assert.Equal("test262", Pin()["suite"]);
        Assert.Equal("no", Pin()["archived"]);

        Assert.Contains(
            ArchitectureRules.N15(
                new Dictionary<string, string>(StringComparer.Ordinal) { ["suite"] = "test262" },
                Document(DecisionPath),
                Document(LedgerPath)),
            violation => violation.Contains("quantifying over nothing", StringComparison.Ordinal));
    }

    [Fact]
    public void N15_Rejects_A_Revision_Or_Digest_Only_The_Pin_File_Names()
    {
        foreach (var key in new[] { "revision", "content-sha256" })
        {
            var moved = new Dictionary<string, string>(Pin(), StringComparer.Ordinal)
            {
                [key] = new string('0', Pin()[key].Length),
            };

            var violations = ArchitectureRules.N15(
                moved, Document(DecisionPath), Document(LedgerPath)).ToArray();

            Assert.Contains(violations, violation => violation.Contains(
                $"the decision record does not name the pinned {key}", StringComparison.Ordinal));

            Assert.Contains(violations, violation => violation.Contains(
                $"the ledger does not name the pinned {key}", StringComparison.Ordinal));
        }
    }

    [Fact]
    public void N15_Rejects_An_Archive_Claim_Only_One_Document_Makes()
    {
        // The suite is retrieved and hashed and not archived, so the ledger has to say which of
        // the three actions is outstanding. Both directions, because the field can move.
        var claimsArchived = new Dictionary<string, string>(Pin(), StringComparer.Ordinal)
        {
            ["archived"] = "yes",
        };

        Assert.Contains(
            ArchitectureRules.N15(claimsArchived, Document(DecisionPath), Document(LedgerPath)),
            violation => violation.Contains(
                "still says it is not", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N15(
                Pin(),
                Document(DecisionPath),
                "a ledger naming " + Pin()["revision"] + " and " + Pin()["content-sha256"] +
                " and saying nothing about what is outstanding"),
            violation => violation.Contains(
                "says which of the three actions is outstanding", StringComparison.Ordinal));
    }

    /// <summary>
    /// The retained pin, parsed the way the harness parses it rather than searched for.
    /// </summary>
    /// <remarks>
    /// A rule grepping the file for a hash would find it in a comment as readily as in the line
    /// the harness enforces, and which value is enforced is this rule's whole subject.
    /// </remarks>
    private static IReadOnlyDictionary<string, string> Pin()
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var raw in File.ReadAllLines(Path.Combine(ComponentGraph.Root, PinPath)))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var space = line.IndexOf(' ', StringComparison.Ordinal);

            if (space > 0)
            {
                values[line[..space]] = line[(space + 1)..].Trim();
            }
        }

        return values;
    }

    private static string Document(string relativePath) =>
        File.ReadAllText(Path.Combine(ComponentGraph.Root, relativePath));
}
