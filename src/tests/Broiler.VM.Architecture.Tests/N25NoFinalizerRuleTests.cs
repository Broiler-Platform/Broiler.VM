using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N25: no type in the JavaScript profile's product assemblies declares a finalizer.
/// </summary>
/// <remarks>
/// <para>
/// <b>A finalizer runs guest-reachable work at a moment no guest and no host chose.</b> It runs on
/// the collector's thread, outside the step that owns the realm, outside every budget the meter
/// charges and after the realm may be gone. Decision JSD-0011's Row 2 keeps the profile's references
/// rooted by the realm and nothing else, and decision JSD-0029 makes "no guest code ever runs from a
/// CLR finalizer" the rule its optional cleanup model rests on: a `FinalizationRegistry` callback
/// runs only as a job a host-requested drain queued. The one line that would break both is a
/// destructor, and every other test in this repository would stay green after it.
/// </para>
/// <para>
/// <b>Taken on its own, ahead of the model.</b> JSD-0029's recommendation of 2026-10-03 asks for this
/// rule now, because it costs nothing and puts the record's main safety claim under test while the
/// model itself stays unscheduled.
/// </para>
/// <para>
/// <b>It reads declarations, not text.</b> A destructor (<c>~T()</c>) and a method named
/// <c>Finalize</c> with no parameters are both reported; the profile's documentation names both
/// in prose, and a text search would be defeated by it.
/// </para>
/// </remarks>
public sealed class N25NoFinalizerRuleTests
{
    /// <summary>The profile's three product assemblies, as in rules N18 and N23.</summary>
    private static readonly string[] ProfileAssemblies =
    [
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
        "Broiler.VM.Profile.JavaScript.Format",
    ];

    [Fact]
    public void N25_No_Type_Of_This_Profile_Declares_A_Finalizer()
    {
        var covered = AssuranceSources.Files
            .Where(static file => ProfileAssemblies.Contains(file.Assembly, StringComparer.Ordinal))
            .ToArray();

        // Non-vacuous: all three assemblies represented, and the file holding the
        // FinalizationRegistry - the type the regression would arrive in - among them by name.
        Assert.Equal(
            ProfileAssemblies.Order(StringComparer.Ordinal),
            covered.Select(static file => file.Assembly).Distinct().Order(StringComparer.Ordinal));

        Assert.Contains(covered, static file => string.Equals(
            file.RelativePath, "src/Broiler.VM.Profile.JavaScript/JsCollections.cs", StringComparison.Ordinal));

        Assert.Empty(covered.SelectMany(static file => Violations(file.RelativePath, file.Tree)));
    }

    [Fact]
    public void N25_A_Type_That_Declares_A_Finalizer_Is_Reported()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            "src/tests/Broiler.VM.Architecture.Tests/witnesses/register",
            "N25-a-profile-type-that-declares-a-finalizer.cs.witness"));

        var fence = witness.IndexOf("```csharp\n", StringComparison.Ordinal) + "```csharp\n".Length;
        var source = witness[fence..witness.IndexOf("```", fence, StringComparison.Ordinal)];
        var violations = Violations("witness.cs", CSharpSyntaxTree.ParseText(source)).ToArray();

        // Exactly the two declarations: the destructor and the Finalize method. The comment and the
        // string literal name both again and are not reported.
        Assert.Equal(2, violations.Length);
        Assert.Contains(violations, static violation => violation.Contains("a destructor", StringComparison.Ordinal));
        Assert.Contains(violations, static violation => violation.Contains("a Finalize method", StringComparison.Ordinal));
        Assert.Contains("Finalize()", source, StringComparison.Ordinal);
    }

    /// <summary>Each finalizer declared in the tree, with its line.</summary>
    private static IEnumerable<string> Violations(string path, SyntaxTree tree)
    {
        const string Reason =
            ": a finalizer runs on the collector's thread at a moment no guest or host chose, outside every " +
            "budget, and JSD-0029 forbids guest-reachable work from one (JSD-0011 Row 2)";

        foreach (var node in tree.GetRoot().DescendantNodes())
        {
            switch (node)
            {
                case DestructorDeclarationSyntax destructor:
                    yield return $"{path}:{Line(destructor)} declares a destructor for `{destructor.Identifier.ValueText}`" + Reason;
                    break;

                case MethodDeclarationSyntax method
                    when method.Identifier.ValueText == "Finalize" && method.ParameterList.Parameters.Count == 0:
                    yield return $"{path}:{Line(method)} declares a Finalize method" + Reason;
                    break;
            }
        }
    }

    private static int Line(SyntaxNode node) => node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
}
