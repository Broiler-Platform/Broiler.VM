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

    /// <summary>The file declaring the core-result code vocabulary.</summary>
    internal const string VocabularyPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs";

    /// <summary>The file declaring the embedder-seam code vocabulary.</summary>
    /// <remarks>
    /// <b>A different assembly, and that is the reason the registry exists in one file.</b> The
    /// profile assembly and the lowering assembly do not reference each other - rule N1 holds that
    /// - so neither compiler can see the other's numbers, and a code used twice would be a defect
    /// nothing in the build could notice. The registry is the third artefact that sees both.
    /// </remarks>
    internal const string SeamVocabularyPath =
        "src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceDiagnostics.cs";

    /// <summary>The file the position encoding is decided in, and the only one that builds one.</summary>
    internal const string PositionPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptPosition.cs";

    /// <summary>The composition that restates the codes its corpus pins.</summary>
    internal const string MirrorPath =
        "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/CorpusBuilder.cs";

    /// <summary>The retained corpus manifest whose rows record a diagnostic code.</summary>
    internal const string CorpusPath = "src/tests/corpus/js-1/corpus.manifest";

    /// <summary>The type whose members are the core-result code vocabulary.</summary>
    internal const string CodeType = "JavaScriptDiagnosticCode";

    /// <summary>The type whose members are the embedder-seam code vocabulary.</summary>
    internal const string SeamCodeType = "SliceSourceDiagnosticCode";

    /// <summary>The retained source-corpus manifest, which the seam half is bound to.</summary>
    internal const string SourceCorpusPath = "src/tests/corpus/js-1/source/source.manifest";

    /// <summary>The type the mirrored constants live in.</summary>
    internal const string MirrorType = "JavaScriptDiagnosticCodes";

    /// <summary>The core position record this profile publishes an encoding for.</summary>
    internal const string PositionType = "VmSourcePosition";

    /// <summary>The stage vocabulary a core-result row may name. Closed.</summary>
    internal static readonly string[] Stages =
    [
        "header", "manifest", "framing", "limits", "constants", "code", "entries", "functions",
        "modules", "positions", "reserved", "reader",
    ];

    /// <summary>The stage vocabulary an embedder-seam row may name. Closed.</summary>
    /// <remarks>
    /// A separate list rather than four more members of the one above, because the two halves
    /// refuse different things and a row that named a verification stage while rejecting source -
    /// or the other way round - would be a row whose half and whose stage disagree. Keeping the
    /// lists apart is what makes that a rule violation rather than a plausible-looking row.
    /// </remarks>
    internal static readonly string[] SeamStages = ["tokenizer", "parser", "semantics", "lowering"];

    /// <summary>The two halves of the registry, per roadmap section 9's boundary question.</summary>
    internal static readonly string[] Halves = ["core-result", "embedder-seam"];

    /// <summary>The three reachability kinds a row may claim.</summary>
    internal static readonly string[] Reachabilities = ["corpus", "source", "defensive"];

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
    internal static IReadOnlyList<(string Name, int Value)> Vocabulary(SyntaxTree tree) =>
        Vocabulary(tree, CodeType);

    /// <summary>The members of <paramref name="type"/>, an enum declared in <paramref name="tree"/>.</summary>
    /// <remarks>
    /// The type is a parameter since JS-3b, because there are two vocabularies now and they are
    /// declared in two assemblies. Reading them with one function is what makes "the two halves
    /// are the same kind of thing" a fact about the code rather than a claim in the registry's
    /// header.
    /// </remarks>
    internal static IReadOnlyList<(string Name, int Value)> Vocabulary(SyntaxTree tree, string type) => tree
        .GetRoot()
        .DescendantNodes()
        .OfType<EnumDeclarationSyntax>()
        .Where(declaration => declaration.Identifier.ValueText == type)
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

    /// <summary>
    /// The refused entries of the retained source corpus, as code name to entry names.
    /// </summary>
    /// <remarks>
    /// <b>Keyed by the code's NAME and not by its number</b>, which is the one place these two
    /// manifests differ in shape. The artifact corpus records a number, because a core result
    /// carries a number in a fixed field and the replay reads it out of that field; a source
    /// refusal is reported to a caller who holds the enumeration, so the manifest records the name
    /// the caller would see. Recording the number instead would have made the manifest agree with
    /// the registry by carrying the same integer twice.
    /// </remarks>
    internal static ILookup<string, string> SourceCases(string text) => text
        .Split('\n')
        .Select(static line => line.TrimEnd('\r'))
        .Where(static line => line.Length > 0 && line[0] != '#')
        .Select(static line => line.Split('|'))
        .Where(static parts => parts.Length >= 4 &&
            string.Equals(parts[0], "refused", StringComparison.Ordinal))
        .ToLookup(static parts => parts[3], static parts => parts[1], StringComparer.Ordinal);

    /// <summary>The retained source-corpus manifest's text.</summary>
    internal static string SourceCorpusText =>
        File.ReadAllText(Path.Combine(ComponentGraph.Root, SourceCorpusPath));

    /// <summary>One place the profile answers a resource exhaustion, and the pair it names.</summary>
    internal sealed record ExhaustionAnswer(string File, int Line, string Dimension, string Scope);

    /// <summary>One corpus row, reduced to what an exhaustion binding reads.</summary>
    internal sealed record CorpusOutcome(string Name, string Outcome, string Dimension, string Scope);

    /// <summary>The contract factory that mints a resource-exhaustion answer.</summary>
    internal const string ExhaustionFactory = "ResourceExhaustion";

    /// <summary>The core enumerations an exhaustion answer names one member of each of.</summary>
    internal const string DimensionType = "VmBudgetDimension";

    /// <summary>The scope half of that pair.</summary>
    internal const string ScopeType = "VmBudgetScope";

    /// <summary>
    /// Every dimension the profile assembly can answer a resource exhaustion on, with the scope
    /// each site names and the line it is on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Syntax and not semantics, for the reason <see cref="EmissionSites"/> gives: rule A11 keeps
    /// the profile assembly out of this project's reference set, so there is no symbol resolution
    /// and a site is recognised by the shape it is written in - a call to the contract's
    /// <c>ResourceExhaustion</c> factory naming one member of each enumeration.
    /// </para>
    /// <para>
    /// <b>What that costs, stated rather than glossed.</b> A site that computed its dimension
    /// instead of naming it would not be found. The rule states the limit by asserting the count
    /// it expects to see, so a verifier that grew a computed arm fails this rule rather than
    /// passing it quietly - which is the same shape as the vocabulary clause in N6.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<ExhaustionAnswer> ExhaustionAnswers(
        IEnumerable<AssuranceSourceFile> files)
    {
        var answers = new List<ExhaustionAnswer>();

        foreach (var file in files)
        {
            foreach (var invocation in file.Tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>())
            {
                if (invocation.Expression is not MemberAccessExpressionSyntax callee ||
                    callee.Name.Identifier.ValueText != ExhaustionFactory)
                {
                    continue;
                }

                var named = invocation.ArgumentList.Arguments
                    .SelectMany(static argument => argument.DescendantNodesAndSelf())
                    .OfType<MemberAccessExpressionSyntax>()
                    .ToArray();

                var dimension = named
                    .Where(static access => access.Expression.ToString() == DimensionType)
                    .Select(static access => access.Name.Identifier.ValueText)
                    .FirstOrDefault();

                if (dimension is null)
                {
                    continue;
                }

                var scope = named
                    .Where(static access => access.Expression.ToString() == ScopeType)
                    .Select(static access => access.Name.Identifier.ValueText)
                    .FirstOrDefault();

                answers.Add(new ExhaustionAnswer(
                    file.RelativePath,
                    file.Tree.GetLineSpan(invocation.Span).StartLinePosition.Line + 1,
                    dimension,
                    scope ?? "(none)"));
            }
        }

        return answers;
    }

    /// <summary>Every row of a retained corpus manifest, with its outcome and its exhausted pair.</summary>
    /// <remarks>
    /// The two columns are read positionally, like the diagnostic code above, and a row too short
    /// to carry them is dropped rather than defaulted: a manifest written before the columns
    /// existed must fail this binding rather than satisfy it with a blank.
    /// </remarks>
    internal static IReadOnlyList<CorpusOutcome> CorpusOutcomes(string text) => text
        .Split('\n')
        .Select(static line => line.TrimEnd('\r'))
        .Where(static line => line.Length > 0 && line[0] != '#')
        .Select(static line => line.Split('|'))
        .Where(static parts => parts.Length >= 10)
        .Select(static parts => new CorpusOutcome(parts[0], parts[3], parts[8], parts[9]))
        .ToArray();

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
