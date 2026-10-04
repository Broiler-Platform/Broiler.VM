using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N26: no code of the JavaScript profile holds a realm it did not get from the running one,
/// outside the places that set or restore the running realm.
/// </summary>
/// <remarks>
/// <para>
/// <b>An engine holds several realms, and the right one is the one running.</b> Decision JSD-0030's
/// slice SR-2 gives every function a <c>[[Realm]]</c> and the engine a running realm that each frame
/// and built-in sets; <c>engine.Realm</c> is then the specification's "current Realm Record". The
/// regression this rule exists for is a realm captured once and read later - a field caching the
/// realm a type was made in, or a read of the engine's first realm from code a guest reaches - which
/// would hand a guest of one realm another realm's intrinsics and pass every single-realm test.
/// </para>
/// <para>
/// <b>Two clauses, read as declarations.</b> A field or auto-property typed <c>JsRealm</c> is
/// reported unless it is one of the members that ARE the model: the running realm, the first realm,
/// a function's <c>[[Realm]]</c>, a frame's saved realm, an awaiting built-in's own realm, an
/// embedder view's realm and a proxy's maker. And <c>FirstRealm</c> is reported wherever it is read but the engine's constructor and the
/// embedder's first view, the two places no frame is running.
/// </para>
/// </remarks>
public sealed class N26FixedRealmRuleTests
{
    /// <summary>The profile's three product assemblies, as in rules N18, N23 and N25.</summary>
    private static readonly string[] ProfileAssemblies =
    [
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
        "Broiler.VM.Profile.JavaScript.Format",
    ];

    /// <summary>The stored realms that are the model, as <c>Type.Member</c>.</summary>
    private static readonly string[] Holders =
    [
        "JsEngine.Realm",
        "JsEngine.FirstRealm",
        "JsFunction.Realm",
        "JsStackSite.OuterRealm",
        "JsHostRealm.realm",
        "JsProxy.realm",

        // `disposeAsync()` is a built-in that awaits: the run resumes in promise reactions and is
        // still the built-in running, so it keeps the built-in's realm as a frame keeps its function's.
        "JsDisposalRun.realm",

        // A ShadowRealm instance's [[ShadowRealm]] slot: the realm its `evaluate` runs source in.
        "JsShadowRealmObject.Inner",
    ];

    /// <summary>The members that may read the first realm: where no frame is running.</summary>
    private static readonly string[] FirstRealmReaders =
    [
        "JsEngine..ctor",
        "JsEngine.HostRealm",
    ];

    [Fact]
    public void N26_No_Profile_Code_Holds_Or_Reads_A_Fixed_Realm()
    {
        var covered = AssuranceSources.Files
            .Where(static file => ProfileAssemblies.Contains(file.Assembly, StringComparer.Ordinal))
            .ToArray();

        // Non-vacuous: the files that declare the running realm and a function's realm are among the
        // sources, and every holder the allowlist names is found, so a renamed member fails here
        // rather than leaving its replacement unchecked.
        Assert.Contains(covered, static file => string.Equals(
            file.RelativePath, "src/Broiler.VM.Profile.JavaScript/JsEngine.Realms.cs", StringComparison.Ordinal));

        var found = covered.SelectMany(static file => Holding(file.Tree)).ToHashSet(StringComparer.Ordinal);
        Assert.Equal(Holders.Order(StringComparer.Ordinal), found.Order(StringComparer.Ordinal));

        Assert.Empty(covered.SelectMany(static file => Violations(file.RelativePath, file.Tree)));
    }

    [Fact]
    public void N26_A_Cached_Realm_And_A_Read_Of_The_First_Realm_Are_Reported()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            "src/tests/Broiler.VM.Architecture.Tests/witnesses/register",
            "N26-a-profile-type-that-caches-a-realm.cs.witness"));

        var fence = witness.IndexOf("```csharp\n", StringComparison.Ordinal) + "```csharp\n".Length;
        var source = witness[fence..witness.IndexOf("```", fence, StringComparison.Ordinal)];
        var violations = Violations("witness.cs", CSharpSyntaxTree.ParseText(source)).ToArray();

        // Exactly two: the cached field and the read of the first realm from a built-in's body. The
        // expression-bodied property that forwards the running realm, and the comment and string that
        // name FirstRealm, are not reported.
        Assert.Equal(2, violations.Length);
        Assert.Contains(violations, static violation => violation.Contains("JsIntrinsicCache.made", StringComparison.Ordinal));
        Assert.Contains(violations, static violation => violation.Contains("reads FirstRealm", StringComparison.Ordinal));
    }

    /// <summary>Every stored realm the tree declares, as <c>Type.Member</c>.</summary>
    private static IEnumerable<string> Holding(SyntaxTree tree)
    {
        foreach (var node in tree.GetRoot().DescendantNodes())
        {
            switch (node)
            {
                case FieldDeclarationSyntax field when IsRealm(field.Declaration.Type):
                    foreach (var variable in field.Declaration.Variables)
                    {
                        yield return Owner(field) + "." + variable.Identifier.ValueText;
                    }

                    break;

                case PropertyDeclarationSyntax property
                    when IsRealm(property.Type) && property.AccessorList is { } accessors &&
                        accessors.Accessors.All(static accessor => accessor.Body is null && accessor.ExpressionBody is null):
                    yield return Owner(property) + "." + property.Identifier.ValueText;
                    break;
            }
        }
    }

    /// <summary>Each stored realm outside the model, and each read of the first realm outside its readers.</summary>
    private static IEnumerable<string> Violations(string path, SyntaxTree tree)
    {
        const string Holds =
            " stores a realm: an engine holds several, and anything a guest reaches has to read the running " +
            "one (engine.Realm) or a function's own, never one it captured (JSD-0030 SR-2)";

        const string Reads =
            " reads FirstRealm where a frame may be running, so a guest of another realm would be handed the " +
            "first realm's intrinsics (JSD-0030 SR-2)";

        foreach (var held in Holding(tree))
        {
            if (!Holders.Contains(held, StringComparer.Ordinal))
            {
                yield return $"{path}: {held}" + Holds;
            }
        }

        foreach (var name in tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (name.Identifier.ValueText != "FirstRealm" || name.Parent is PropertyDeclarationSyntax)
            {
                continue;
            }

            var member = name.Ancestors().FirstOrDefault(static ancestor =>
                ancestor is BaseMethodDeclarationSyntax or PropertyDeclarationSyntax);

            var reader = member switch
            {
                ConstructorDeclarationSyntax constructor => Owner(constructor) + "..ctor",
                MethodDeclarationSyntax method => Owner(method) + "." + method.Identifier.ValueText,
                PropertyDeclarationSyntax property => Owner(property) + "." + property.Identifier.ValueText,
                _ => "(no member)",
            };

            if (!FirstRealmReaders.Contains(reader, StringComparer.Ordinal))
            {
                yield return $"{path}:{Line(name)} {reader}" + Reads;
            }
        }
    }

    private static bool IsRealm(TypeSyntax type) =>
        (type is NullableTypeSyntax nullable ? nullable.ElementType : type) is IdentifierNameSyntax
        {
            Identifier.ValueText: "JsRealm",
        };

    private static string Owner(SyntaxNode node) =>
        node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText ?? "(none)";

    private static int Line(SyntaxNode node) => node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
}
