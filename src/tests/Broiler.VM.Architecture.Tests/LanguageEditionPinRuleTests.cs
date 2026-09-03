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
        Assert.Empty(ArchitectureRules.N14(Declared(), Document(DecisionPath), Document(LedgerPath)));

        // Non-vacuous in the way this rule can most easily be empty for the wrong reason: it
        // compares constants against text, so a declaration that stopped declaring them would make
        // every clause free. The count is asserted rather than the names alone, because a rule
        // reading four of ten constants is a rule the other six can be edited around.
        Assert.Equal(10, Declared().Count);
        Assert.Equal("ES2026", Declared()["Year"]);

        Assert.Contains(
            ArchitectureRules.N14(
                new Dictionary<string, string>(StringComparer.Ordinal) { ["Year"] = "ES2026" },
                Document(DecisionPath),
                Document(LedgerPath)),
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
            moved, Document(DecisionPath), Document(LedgerPath)).ToArray();

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

        Assert.Contains(
            ArchitectureRules.N14(rehashed, Document(DecisionPath), Document(LedgerPath)),
            violation => violation.Contains(
                "the decision record does not name the declared DocumentDigest",
                StringComparison.Ordinal));
    }

    [Fact]
    public void N14_Rejects_An_Archive_Claim_Only_One_Document_Makes()
    {
        // THE CLAUSE THAT GUARDS THE OVERCLAIM, in both directions. Flipping the boolean without
        // resolving the ledger's exclusion claims a fully taken pin the ledger does not support;
        // resolving the exclusion without flipping the boolean leaves every run printing "NOT
        // archived" over a document somebody archived.
        var claimsArchived = new Dictionary<string, string>(Declared(), StringComparer.Ordinal)
        {
            ["Archived"] = "true",
        };

        Assert.Contains(
            ArchitectureRules.N14(claimsArchived, Document(DecisionPath), Document(LedgerPath)),
            violation => violation.Contains(
                "still calls the pin provisional", StringComparison.Ordinal));

        Assert.Contains(
            ArchitectureRules.N14(
                Declared(),
                Document(DecisionPath),
                "a ledger that says nothing about how this pin was taken"),
            violation => violation.Contains(
                "an unarchived pin carries a named exclusion", StringComparison.Ordinal));
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
