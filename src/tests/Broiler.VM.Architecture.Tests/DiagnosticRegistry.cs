using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One published row of the JavaScript profile's diagnostic-code registry.</summary>
/// <remarks>
/// The registry is a published artefact of the profile, at
/// <c>src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt</c>, and decision JSD-0009
/// owns it. It is read here rather than restated: the group N rules bind it to the enum that
/// declares the codes, to the sites that emit them, to the corpus that reaches them, and to the
/// composition that restates them, and a rule that carried its own copy of the registry would be
/// binding four things to a fifth nobody publishes.
/// </remarks>
internal sealed record DiagnosticRegistryRow(
    int Code,
    string Name,
    string Half,
    string Reason,
    string Stage,
    string Reachability,
    string Case,
    int Since);

/// <summary>
/// The published registry, the enum that declares its codes, the sites that emit them, and the
/// composition that restates them - each read off disk, none derived from another.
/// </summary>
internal static class DiagnosticRegistry
{
    /// <summary>The published registry file, relative to the component root.</summary>
    internal const string RegistryPath =
        "src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt";

    /// <summary>The file declaring the code vocabulary.</summary>
    internal const string VocabularyPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs";

    /// <summary>The file the position encoding is decided in, and the only one that builds one.</summary>
    internal const string PositionPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptPosition.cs";

    /// <summary>The composition that restates the codes its corpus pins.</summary>
    internal const string MirrorPath =
        "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/CorpusBuilder.cs";

    /// <summary>The retained corpus manifest whose rows record a diagnostic code.</summary>
    internal const string CorpusPath = "src/tests/corpus/js-1/corpus.manifest";

    /// <summary>The type whose members are the code vocabulary.</summary>
    internal const string CodeType = "JavaScriptDiagnosticCode";

    /// <summary>The type the mirrored constants live in.</summary>
    internal const string MirrorType = "JavaScriptDiagnosticCodes";

    /// <summary>The core position record this profile publishes an encoding for.</summary>
    internal const string PositionType = "VmSourcePosition";

    /// <summary>The stage vocabulary a registry row may name. Closed.</summary>
    internal static readonly string[] Stages =
    [
        "header", "manifest", "framing", "limits", "constants", "code", "entries", "positions",
        "reserved", "reader",
    ];

    /// <summary>The two halves of the registry, per roadmap section 9's boundary question.</summary>
    internal static readonly string[] Halves = ["core-result", "embedder-seam"];

    /// <summary>The two reachability kinds a row may claim.</summary>
    internal static readonly string[] Reachabilities = ["corpus", "defensive"];

    /// <summary>The published registry, parsed.</summary>
    internal static IReadOnlyList<DiagnosticRegistryRow> Rows { get; } = Read(Text);

    /// <summary>The revision the registry states for itself.</summary>
    internal static int Revision { get; } = ReadRevision(Text);

    /// <summary>The registry file's text.</summary>
    internal static string Text => File.ReadAllText(Path.Combine(ComponentGraph.Root, RegistryPath));

    /// <summary>Parses a registry, so a witness input can be parsed by the same code.</summary>
    /// <remarks>
    /// Hand-parsed and deliberately strict: a row with the wrong number of columns is a parse
    /// failure rather than a row with a missing field, because the alternative is a rule that
    /// reads a truncated row as a row making fewer claims.
    /// </remarks>
    internal static IReadOnlyList<DiagnosticRegistryRow> Read(string text)
    {
        var rows = new List<DiagnosticRegistryRow>();

        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');

            if (trimmed.Length == 0 || trimmed[0] == '#')
            {
                continue;
            }

            var parts = trimmed.Split('|');

            if (parts.Length != 8)
            {
                throw new InvalidOperationException(
                    $"registry row has {parts.Length} columns rather than 8: {trimmed}");
            }

            rows.Add(new DiagnosticRegistryRow(
                int.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                parts[1],
                parts[2],
                parts[3],
                parts[4],
                parts[5],
                parts[6],
                int.Parse(parts[7], System.Globalization.CultureInfo.InvariantCulture)));
        }

        return rows;
    }

    /// <summary>The revision a registry states for itself, or <c>-1</c> when it states none.</summary>
    internal static int ReadRevision(string text)
    {
        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');

            if (!trimmed.StartsWith("# registry-revision:", StringComparison.Ordinal))
            {
                continue;
            }

            return int.TryParse(
                trimmed["# registry-revision:".Length..].Trim(),
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out var revision)
                ? revision
                : -1;
        }

        return -1;
    }

    /// <summary>The declared vocabulary: every member of the code enum, with its number.</summary>
    internal static IReadOnlyList<(string Name, int Value)> Vocabulary(SyntaxTree tree) => tree
        .GetRoot()
        .DescendantNodes()
        .OfType<EnumDeclarationSyntax>()
        .Where(static declaration => declaration.Identifier.ValueText == CodeType)
        .SelectMany(static declaration => declaration.Members)
        .Select(static member => (
            Name: member.Identifier.ValueText,
            Value: int.Parse(
                member.EqualsValue!.Value.ToString().Replace("_", string.Empty, StringComparison.Ordinal),
                System.Globalization.CultureInfo.InvariantCulture)))
        .ToArray();

    /// <summary>One place a diagnostic code is handed to a refusal, with the reason beside it.</summary>
    internal sealed record EmissionSite(string File, int Line, string Code, string Reason);

    /// <summary>
    /// Every site in the profile assembly that hands a code to a refusal, and the core reason each
    /// carries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two shapes exist and both are resolved. A DIRECT site names the reason and the code in one
    /// call. A FORWARDING site hands a code to a helper that supplies the reason from its own body
    /// - <c>ReadReserved</c> is the one that exists - and the reason is resolved by reading that
    /// helper rather than by listing the helper here, so a second one needs no change.
    /// </para>
    /// <para>
    /// This is syntax and not semantics: there is no compilation and no symbol resolution, because
    /// rule A11 keeps the profile assembly out of this project's reference set. What that costs is
    /// that a helper in another file would not be resolved, and the rule states it: every code the
    /// vocabulary declares must be reached by a site this reader finds, so a code whose only site
    /// this reader cannot see fails rather than passing quietly.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<EmissionSite> EmissionSites(IEnumerable<AssuranceSourceFile> files)
    {
        var sites = new List<EmissionSite>();

        foreach (var file in files)
        {
            var root = file.Tree.GetRoot();
            var forwarding = ForwardingHelpers(root);

            foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var codes = invocation.ArgumentList.Arguments
                    .SelectMany(static argument => argument.DescendantNodesAndSelf())
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(static access => access.Expression.ToString() == CodeType)
                    .Select(static access => access.Name.Identifier.ValueText)
                    .ToArray();

                if (codes.Length == 0)
                {
                    continue;
                }

                var reason = invocation.ArgumentList.Arguments
                    .SelectMany(static argument => argument.DescendantNodesAndSelf())
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(static access => access.Expression.ToString() == "VmReason")
                    .Select(static access => access.Name.Identifier.ValueText)
                    .FirstOrDefault();

                if (reason is null &&
                    invocation.Expression is IdentifierNameSyntax callee &&
                    forwarding.TryGetValue(callee.Identifier.ValueText, out var forwarded))
                {
                    reason = forwarded;
                }

                var line = file.Tree.GetLineSpan(invocation.Span).StartLinePosition.Line + 1;

                foreach (var code in codes)
                {
                    sites.Add(new EmissionSite(file.RelativePath, line, code, reason ?? "(none)"));
                }
            }
        }

        return sites;
    }

    /// <summary>
    /// Methods that take a code as a parameter and supply the reason themselves, by name and
    /// reason.
    /// </summary>
    private static Dictionary<string, string> ForwardingHelpers(SyntaxNode root)
    {
        var helpers = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            var parameter = method.ParameterList.Parameters
                .FirstOrDefault(static candidate => candidate.Type?.ToString() == CodeType);

            if (parameter is null)
            {
                continue;
            }

            var supplied = method
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation.ArgumentList.Arguments
                    .Any(argument => argument.Expression is IdentifierNameSyntax name &&
                        name.Identifier.ValueText == parameter.Identifier.ValueText))
                .SelectMany(static invocation => invocation.ArgumentList.Arguments)
                .Select(static argument => argument.Expression)
                .OfType<MemberAccessExpressionSyntax>()
                .Where(static access => access.Expression.ToString() == "VmReason")
                .Select(static access => access.Name.Identifier.ValueText)
                .FirstOrDefault();

            if (supplied is not null)
            {
                helpers[method.Identifier.ValueText] = supplied;
            }
        }

        return helpers;
    }

    /// <summary>The constants a composition restates, by name and value.</summary>
    internal static IReadOnlyList<(string Name, int Value)> Mirror(SyntaxTree tree) => tree
        .GetRoot()
        .DescendantNodes()
        .OfType<ClassDeclarationSyntax>()
        .Where(static declaration => declaration.Identifier.ValueText == MirrorType)
        .SelectMany(static declaration => declaration.Members.OfType<FieldDeclarationSyntax>())
        .SelectMany(static field => field.Declaration.Variables)
        .Select(static variable => (
            Name: variable.Identifier.ValueText,
            Value: int.Parse(
                variable.Initializer!.Value.ToString().Replace("_", string.Empty, StringComparison.Ordinal),
                System.Globalization.CultureInfo.InvariantCulture)))
        .ToArray();

    /// <summary>Every code a retained corpus manifest records, by the entry that records it.</summary>
    internal static ILookup<int, string> CorpusCases(string text) => text
        .Split('\n')
        .Select(static line => line.TrimEnd('\r'))
        .Where(static line => line.Length > 0 && line[0] != '#')
        .Select(static line => line.Split('|'))
        .Where(static parts => parts.Length >= 6)
        .ToLookup(
            static parts => int.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture),
            static parts => parts[0]);

    /// <summary>The retained corpus manifest's text.</summary>
    internal static string CorpusText =>
        File.ReadAllText(Path.Combine(ComponentGraph.Root, CorpusPath));

    /// <summary>One member that answers with a position, and whether it builds one itself.</summary>
    internal sealed record PositionProducer(string File, string Member, bool Constructs);

    /// <summary>
    /// Every member of the profile assembly whose answer is a core position, and whether it
    /// constructs one rather than delegating.
    /// </summary>
    /// <remarks>
    /// Both creation shapes count: <c>new VmSourcePosition(...)</c> and the target-typed
    /// <c>new(...)</c> the factories are written with. A rule that looked only for the named form
    /// would be satisfied by exactly the form this component actually writes.
    /// </remarks>
    internal static IReadOnlyList<PositionProducer> PositionProducers(
        IEnumerable<AssuranceSourceFile> files)
    {
        var producers = new List<PositionProducer>();

        foreach (var file in files)
        {
            foreach (var method in file.Tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                if (method.ReturnType.ToString() != PositionType)
                {
                    continue;
                }

                var constructs = method.DescendantNodes().Any(static node =>
                    node is ImplicitObjectCreationExpressionSyntax ||
                    (node is ObjectCreationExpressionSyntax created &&
                        created.Type.ToString() == PositionType));

                producers.Add(new PositionProducer(
                    file.RelativePath, method.Identifier.ValueText, constructs));
            }
        }

        return producers;
    }

    /// <summary>Every named construction of a core position, wherever it sits.</summary>
    internal static IReadOnlyList<string> NamedPositionConstructions(
        IEnumerable<AssuranceSourceFile> files) => files
        .SelectMany(file => file.Tree.GetRoot()
            .DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .Where(static created => created.Type.ToString() == PositionType)
            .Select(created =>
                $"{file.RelativePath}" +
                $"({file.Tree.GetLineSpan(created.Span).StartLinePosition.Line + 1})"))
        .ToArray();
}
