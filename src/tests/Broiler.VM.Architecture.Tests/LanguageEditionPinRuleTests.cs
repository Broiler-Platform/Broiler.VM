using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N14: the pinned language edition says the same thing in three independently written
/// places, and the code's account of whether the document is archived binds the ledger's.
/// </summary>
/// <remarks>
/// <para>
/// The rule lives in a file of its own because its subject is neither the register nor the
/// diagnostic registry: it is one pin, and the three documents that would have to agree for it to
/// mean anything. The ledger has carried "the language-specification edition is not pinned" as an
/// open dependency since JS-0, with the consequence stated in its own words - no manifest may be
/// accepted against an unpinned edition - so the value of the pin is exactly the value of its
/// agreeing with itself.
/// </para>
/// <para>
/// <b>Read as source rather than as loaded constants</b>, because rule A11 keeps the profile
/// assembly out of this project's reference set. That is the same reason rules N5 through N9 parse
/// the profile's sources, and it has the same benefit: what the rule sees is what a reader of the
/// file sees, not what a compiler folded.
/// </para>
/// </remarks>
public sealed class LanguageEditionPinRuleTests
{
    /// <summary>Where the pin is declared.</summary>
    private const string DeclarationPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptLanguageEdition.cs";

    /// <summary>The record that argues for it.</summary>
    private const string DecisionPath =
        "src/Broiler.VM.Profile.JavaScript/docs/decisions/" +
        "0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md";

    /// <summary>Where a reader learns the pin's state.</summary>
    private const string LedgerPath =
        "src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md";

    [Fact]
    public void N14_The_Pin_Says_The_Same_Thing_In_The_Code_The_Record_And_The_Ledger()
    {
        Assert.Empty(ArchitectureRules.N14(
            Declared(), Document(DecisionPath), Document(LedgerPath), ArchivedDigest()));

        // Non-vacuous in the way this rule can most easily be empty for the wrong reason: it
        // compares constants against text, so a declaration that stopped declaring them would make
        // every clause free. The count is asserted rather than the names alone, because a rule
        // reading five of eleven constants is a rule the other six can be edited around. The
        // digest clause is non-vacuous in a second way that is worth asserting separately: it
        // compares against a file, so a run in which that file had vanished would otherwise report
        // one violation where it should report the absence.
        Assert.Equal(11, Declared().Count);
        Assert.Equal("ES2026", Declared()["Year"]);
        Assert.NotNull(ArchivedDigest());

        Assert.Contains(
            ArchitectureRules.N14(
                new Dictionary<string, string>(StringComparer.Ordinal) { ["Year"] = "ES2026" },
                Document(DecisionPath),
                Document(LedgerPath),
                ArchivedDigest()),
            violation => violation.Contains("quantifying over nothing", StringComparison.Ordinal));
    }

    [Fact]
    public void N14_Rejects_A_Revision_Only_The_Code_Names()
    {
        var moved = new Dictionary<string, string>(Declared(), StringComparer.Ordinal)
        {
            ["Revision"] = "0000000000000000000000000000000000000000",
        };

        var violations = ArchitectureRules.N14(
            moved, Document(DecisionPath), Document(LedgerPath), ArchivedDigest()).ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "the decision record does not name the declared Revision", StringComparison.Ordinal));

        Assert.Contains(violations, violation => violation.Contains(
            "the ledger does not name the declared revision", StringComparison.Ordinal));
    }

    [Fact]
    public void N14_Rejects_A_Digest_The_Record_Does_Not_Carry()
    {
        var rehashed = new Dictionary<string, string>(Declared(), StringComparer.Ordinal)
        {
            ["DocumentDigest"] = new string('f', 64),
        };

        var violations = ArchitectureRules.N14(
            rehashed, Document(DecisionPath), Document(LedgerPath), ArchivedDigest()).ToArray();

        Assert.Contains(violations, violation => violation.Contains(
            "the decision record does not name the declared DocumentDigest",
            StringComparison.Ordinal));

        // And the archived bytes disagree with it too, which is the clause that could not exist
        // while the document lived only at a URL.
        Assert.Contains(violations, violation => violation.Contains(
            "the archived document hashes to", StringComparison.Ordinal));
    }

    [Fact]
    public void N14_Rejects_An_Archive_Claim_Only_One_Document_Makes()
    {
        // BOTH DIRECTIONS, because the pin has been in both states and could go back. While the
        // document was retrieved and hashed and not archived, a ledger that had stopped calling
        // the pin provisional would have been claiming a fully taken one; now that it IS archived,
        // a ledger line still calling it provisional is describing a state that has passed.
        var unarchived = new Dictionary<string, string>(Declared(), StringComparer.Ordinal)
        {
            ["Archived"] = "false",
        };

        Assert.Contains(
            ArchitectureRules.N14(
                unarchived, Document(DecisionPath), Document(LedgerPath), ArchivedDigest()),
            violation => violation.Contains(
                "an unarchived pin carries a named exclusion", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N14(
                Declared(),
                Document(DecisionPath),
                Document(LedgerPath) + "\nthis pin at " + Declared()["Revision"] + " is provisional",
                ArchivedDigest()),
            violation => violation.Contains(
                "still calls the pin provisional", StringComparison.Ordinal));
    }

    [Fact]
    public void N14_Rejects_An_Archive_That_Is_Not_There()
    {
        // The clause reads a file, so its failure mode is a file that has gone. A declaration
        // saying a document is archived somewhere nothing is retained is the pin at its least
        // useful: every run keeps printing a digest nobody in this checkout can check.
        Assert.Contains(
            ArchitectureRules.N14(
                Declared(), Document(DecisionPath), Document(LedgerPath), archivedDigest: null),
            violation => violation.Contains("and no file is there", StringComparison.Ordinal));

        var unarchived = new Dictionary<string, string>(Declared(), StringComparer.Ordinal)
        {
            ["Archived"] = "false",
        };

        Assert.Contains(
            ArchitectureRules.N14(
                unarchived,
                Document(DecisionPath),
                Document(LedgerPath) + "\nthis pin at " + Declared()["Revision"] + " is provisional",
                ArchivedDigest()),
            violation => violation.Contains(
                "and the declaration says nothing is archived", StringComparison.Ordinal));
    }

    /// <summary>
    /// The SHA-256 of the archived document, or null where nothing is retained at the declared
    /// path.
    /// </summary>
    /// <remarks>
    /// Read as bytes and hashed here rather than trusted from anywhere: the point of the archive
    /// is that the published constant describes a file this repository holds, and a check that
    /// took the digest from a manifest beside the file would be comparing two copies of the same
    /// claim.
    /// </remarks>
    private static string? ArchivedDigest()
    {
        var path = Path.Combine(
            ComponentGraph.Root,
            Declared()["ArchivedAt"].Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(path))
        {
            return null;
        }

        return Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));
    }

    /// <summary>Every constant the declaration declares, by name, as written.</summary>
    /// <remarks>
    /// String literals arrive without their quotes and everything else as its token text, so a
    /// digest and a boolean are compared the same way. Nothing here evaluates an expression: a
    /// constant assembled from two concatenated literals would be reported as absent rather than
    /// quietly folded, which is the reading a reviewer standing at the file would also take.
    /// </remarks>
    private static IReadOnlyDictionary<string, string> Declared()
    {
        var declared = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var field in AssuranceSources.File(DeclarationPath).Tree
                     .GetRoot()
                     .DescendantNodes()
                     .OfType<FieldDeclarationSyntax>())
        {
            foreach (var variable in field.Declaration.Variables)
            {
                if (variable.Initializer?.Value is LiteralExpressionSyntax literal)
                {
                    declared[variable.Identifier.ValueText] = literal.Token.ValueText;
                }
            }
        }

        return declared;
    }

    private static string Document(string relativePath) =>
        File.ReadAllText(Path.Combine(ComponentGraph.Root, relativePath));
}
