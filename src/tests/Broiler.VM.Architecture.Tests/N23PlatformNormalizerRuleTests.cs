using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N23: no product source of the JavaScript profile reaches the host platform's Unicode
/// normalizer.
/// </summary>
/// <remarks>
/// <para>
/// <b>The platform's answer is chosen by the host, and the language's is not.</b> Decision
/// JSD-0031 section 2 measured <c>string.Normalize</c> both ways: with invariant globalization -
/// which nine compositions here set - it returned <c>e U+0301</c> unchanged from NFC and reported
/// it normalized; without it - the Android composition, and any embedder that chooses so - it
/// normalized, and threw for the lone surrogate the language requires to pass through. So one
/// profile build would give different answers on different hosts, and a wrong answer on every
/// one of them for ill-formed input. <c>normalize</c> refused non-ASCII input for exactly that
/// reason (JSC-91) until slice U3 replaced the refusal with the generated tables.
/// </para>
/// <para>
/// <b>Why a rule.</b> The one line that would replace the table-driven normalizer is a call on
/// the string, and most tests in this repository would stay green after it; this is the assertion N18 makes for
/// the platform's regular expressions, over the same three assemblies.
/// </para>
/// <para>
/// <b>It reads identifier tokens, not text.</b> The profile explains this prohibition in comments
/// and <c>normalize</c> is a JavaScript method name in string literals, so a text search would be
/// defeated by its own documentation. An identifier token <c>Normalize</c>, <c>IsNormalized</c> or
/// <c>NormalizationForm</c> is reported wherever it stands - a call, a method group, a type name,
/// a <c>using static</c> - and that includes a method of the profile's own called
/// <c>Normalize</c>: a syntax tree cannot tell the two receivers apart, so slice U3 named its
/// normalizer <c>NormalizeText</c> rather than this rule guessing.
/// </para>
/// </remarks>
public sealed class N23PlatformNormalizerRuleTests
{
    /// <summary>The profile's three product assemblies, as in rule N18.</summary>
    private static readonly string[] ProfileAssemblies =
    [
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
        "Broiler.VM.Profile.JavaScript.Format",
    ];

    /// <summary>The identifiers that reach the platform normalizer.</summary>
    internal static readonly string[] Identifiers = ["Normalize", "IsNormalized", "NormalizationForm"];

    [Fact]
    public void N23_No_Product_Source_Of_This_Profile_Reaches_The_Platform_Normalizer()
    {
        var covered = AssuranceSources.Files
            .Where(static file => ProfileAssemblies.Contains(file.Assembly, StringComparer.Ordinal))
            .ToArray();

        // Non-vacuous as N18 is: all three assemblies represented, and the file holding the
        // `normalize` method - the call site the regression would arrive in - among them by name.
        Assert.Equal(
            ProfileAssemblies.Order(StringComparer.Ordinal),
            covered.Select(static file => file.Assembly).Distinct().Order(StringComparer.Ordinal));

        Assert.Contains(covered, static file => string.Equals(
            file.RelativePath, "src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs", StringComparison.Ordinal));

        Assert.Empty(covered.SelectMany(static file => Violations(file.RelativePath, file.Tree)));
    }

    [Fact]
    public void N23_A_Source_That_Reaches_The_Platform_Normalizer_Is_Reported()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            "src/tests/Broiler.VM.Architecture.Tests/witnesses/register",
            "N23-a-product-source-that-calls-the-platform-normalizer.cs.witness"));

        var fence = witness.IndexOf("```csharp\n", StringComparison.Ordinal) + "```csharp\n".Length;
        var source = witness[fence..witness.IndexOf("```", fence, StringComparison.Ordinal)];
        var violations = Violations("witness.cs", CSharpSyntaxTree.ParseText(source)).ToArray();

        // Exactly the three code occurrences: the query, the call and the form. The comment and
        // the string literal name all three again and are not reported.
        Assert.Equal(3, violations.Length);

        foreach (var identifier in Identifiers)
        {
            Assert.Contains(violations, violation => violation.Contains($"`{identifier}`", StringComparison.Ordinal));
        }

        Assert.Contains("string.Normalize()", source, StringComparison.Ordinal);
    }

    /// <summary>Each identifier token that names the platform normalizer, with its line.</summary>
    private static IEnumerable<string> Violations(string path, SyntaxTree tree) =>
        from token in tree.GetRoot().DescendantTokens()
        where token.IsKind(SyntaxKind.IdentifierToken) && Identifiers.Contains(token.ValueText, StringComparer.Ordinal)
        select $"{path}:{token.GetLocation().GetLineSpan().StartLinePosition.Line + 1} names `{token.ValueText}`: " +
            "the platform's normalization depends on the host's globalization mode and throws on a lone " +
            "surrogate, so the profile normalizes from its own generated tables (JSD-0031)";
}
