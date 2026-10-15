using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule I5: no member a cut-over retired because a shared library implements it is declared again
/// in the JavaScript family's product sources.
/// </summary>
/// <remarks>
/// <para>
/// <b>A duplicate comes back by being written again, under its old name or a new one.</b> The rule
/// can only see the first, which is the common case: a fix applied to the engine's own copy, or a
/// revert, re-declares the member the cut-over deleted. The second is what the simplification check
/// of each area's milestone searches for by hand (governance.md section 3.1; SJL-3.7).
/// </para>
/// <para>
/// <b>It reads declarations, not text.</b> The register's names are written in comments and
/// documentation that explain what replaced them, so a text search would be defeated by its own
/// records. A member is reported when a type of that fully qualified name declares a method,
/// property, field, constant, event or nested type of that name, whatever its signature; a partial
/// type is the same type in every file. A type declared directly in a namespace is reported by its
/// own fully qualified name, since SJL-4.4 retired three (JSD-0069).
/// </para>
/// </remarks>
public sealed class I5RetiredDuplicatesRuleTests
{
    /// <summary>The family's product assemblies the register's members lived in.</summary>
    private static readonly string[] FamilyAssemblies =
    [
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
        "Broiler.VM.Profile.JavaScript.Format",
        "Broiler.VM.Profile.JavaScript.Intl",
    ];

    [Fact]
    public void I5_No_Retired_Member_Is_Declared_Again()
    {
        var retired = Register();
        Assert.NotEmpty(retired);

        var covered = AssuranceSources.Files
            .Where(static file => FamilyAssemblies.Contains(file.Assembly, StringComparer.Ordinal))
            .ToArray();

        // Non-vacuous: every assembly is read, and the files the retired members lived in are among
        // them, so a register whose types no longer exist would not pass by finding nothing.
        Assert.Equal(FamilyAssemblies.Order(StringComparer.Ordinal), covered.Select(static f => f.Assembly).Distinct().Order(StringComparer.Ordinal));
        foreach (var file in (string[])["src/Broiler.VM.Profile.JavaScript/JsNumberFormat.cs", "src/Broiler.VM.Profile.JavaScript/JsRealm.Number.cs",
                                         "src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs", "src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs",
                                         "src/Broiler.VM.Profile.JavaScript/JsValue.cs", "src/Broiler.VM.Profile.JavaScript/JsBinary.cs",
                                         "src/Broiler.VM.Profile.JavaScript/JsRealm.Math.cs", "src/Broiler.VM.Profile.JavaScript/JsTemporalCore.cs",
                                         "src/Broiler.VM.Profile.JavaScript/JsDecimal.cs", "src/Broiler.VM.Profile.JavaScript/JsBigInt.cs",
                                         "src/Broiler.VM.Profile.JavaScript/JsRealm.Date.cs", "src/Broiler.VM.Profile.JavaScript/JsTimeZones.cs",
                                         "src/Broiler.VM.Profile.JavaScript/JsTemporalParser.cs"])
        {
            Assert.Contains(covered, file2 => string.Equals(file2.RelativePath, file, StringComparison.Ordinal));
        }

        var declared = covered.SelectMany(static file => Declarations(file.RelativePath, file.Tree)).ToArray();
        Assert.Contains(declared, static d => d.Name == "Broiler.VM.Profile.JavaScript.JsNumberFormat.NumberToString");
        Assert.Contains(declared, static d => d.Name == "Broiler.VM.Profile.JavaScript.JsBigInt");
        Assert.Empty(Violations(retired, declared));
    }

    [Fact]
    public void I5_A_Retired_Member_Declared_Again_Is_Reported()
    {
        var source = File.ReadAllText(Path.Combine(ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/witnesses/I5-re-adds-a-retired-member.cs.witness"));
        var violations = Violations(Register(), Declarations("witness.cs", AssuranceSources.Parse(source, "witness.cs"))).ToArray();

        // A retired member and a retired top-level type, each reported once.
        Assert.Equal(2, violations.Length);
        Assert.Contains(violations, static v => v.Contains("Broiler.VM.Profile.JavaScript.JsNumberFormat.ToJsString", StringComparison.Ordinal) && v.Contains("SJL-3.4", StringComparison.Ordinal));
        Assert.Contains(violations, static v => v.Contains("declares Broiler.VM.Profile.JavaScript.JsFloat16,", StringComparison.Ordinal) && v.Contains("SJL-4.4", StringComparison.Ordinal));
    }

    /// <summary>The register: member name to the milestone that retired it and what replaced it.</summary>
    internal static IReadOnlyDictionary<string, (string Milestone, string ReplacedBy)> Register()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/retired-members.json")));
        return document.RootElement.GetProperty("retired").EnumerateArray().ToDictionary(
            static e => e.GetProperty("member").GetString()!,
            static e => (e.GetProperty("milestone").GetString()!, e.GetProperty("replacedBy").GetString()!),
            StringComparer.Ordinal);
    }

    private static IEnumerable<string> Violations(IReadOnlyDictionary<string, (string Milestone, string ReplacedBy)> retired, IEnumerable<(string Name, string Where)> declared) =>
        from d in declared
        where retired.ContainsKey(d.Name)
        let entry = retired[d.Name]
        select $"{d.Where} declares {d.Name}, which {entry.Milestone} retired: {entry.ReplacedBy} implements it (rule I5)";

    /// <summary>
    /// Every member declaration of every type, and every type declared directly in a namespace, by
    /// fully qualified name, with its line.
    /// </summary>
    internal static IEnumerable<(string Name, string Where)> Declarations(string path, SyntaxTree tree)
    {
        foreach (var type in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
        {
            var typeName = FullName(type);
            if (type.Parent is BaseNamespaceDeclarationSyntax or CompilationUnitSyntax)
            {
                yield return (typeName, $"{path}:{type.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");
            }

            if (type is not TypeDeclarationSyntax members)
            {
                continue;
            }

            foreach (var member in members.Members)
            {
                IEnumerable<SyntaxToken> names = member switch
                {
                    MethodDeclarationSyntax m => [m.Identifier],
                    PropertyDeclarationSyntax p => [p.Identifier],
                    EventDeclarationSyntax e => [e.Identifier],
                    BaseFieldDeclarationSyntax f => f.Declaration.Variables.Select(static v => v.Identifier),
                    BaseTypeDeclarationSyntax t => [t.Identifier],
                    DelegateDeclarationSyntax d => [d.Identifier],
                    _ => [],
                };

                foreach (var name in names)
                {
                    yield return ($"{typeName}.{name.ValueText}", $"{path}:{name.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");
                }
            }
        }
    }

    private static string FullName(BaseTypeDeclarationSyntax type)
    {
        var parts = new List<string> { type.Identifier.ValueText };
        for (var parent = type.Parent; parent is not null; parent = parent.Parent)
        {
            switch (parent)
            {
                case BaseTypeDeclarationSyntax outer:
                    parts.Add(outer.Identifier.ValueText);
                    break;
                case BaseNamespaceDeclarationSyntax ns:
                    parts.Add(ns.Name.ToString());
                    break;
            }
        }

        parts.Reverse();
        return string.Join('.', parts);
    }
}
