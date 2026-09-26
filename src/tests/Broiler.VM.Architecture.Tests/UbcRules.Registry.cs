using System.Globalization;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule U8: the universal bytecode's published diagnostic registry, held to the vocabulary that
/// declares its codes, to the source that emits them and to the corpus that reaches them.
/// </summary>
/// <remarks>
/// <para>
/// <b>The shape is the one rules N5 to N7 hold a profile's registry to</b>, and it is written again
/// here rather than reused, because the subjects differ in ways that would each be a parameter: one
/// half rather than two, a corpus manifest in ADR 0011's JSON schema C3 rather than a pipe-separated
/// file, and a code vocabulary in an assembly group N does not read. What is shared is read through
/// the shared helpers - the enum reader of rule U4 and the covered-source parse of group J.
/// </para>
/// <para>
/// <b>Every input is read off disk and none is derived from another</b>: the registry file, the
/// enumeration's declaration, the source of <c>Broiler.VM.Ubc</c> parsed with Roslyn, and the corpus
/// manifest. A rule that carried its own copy of any of the four would be binding three things to a
/// fourth nobody publishes. The one list the rule does carry is the defensive rows', and carrying it
/// is the point: a row that excuses itself from the corpus is an edit to this file.
/// </para>
/// </remarks>
internal static partial class UbcRules
{
    // =============================================================================================
    // U8 - the diagnostic registry
    // =============================================================================================

    /// <summary>The published registry.</summary>
    internal const string RegistryFile = "docs/ubc/diagnostics/registry.txt";

    /// <summary>The file that declares the code vocabulary.</summary>
    internal const string DiagnosticsFile = "src/Broiler.VM.Ubc/UbcDiagnostics.cs";

    /// <summary>The retained corpus manifest the registry's corpus rows name entries of.</summary>
    internal const string CorpusManifestFile = "src/tests/corpus/ubc-1/manifest.json";

    /// <summary>The enumeration whose members are the codes.</summary>
    internal const string DiagnosticCodeType = "UbcDiagnosticCode";

    /// <summary>The core enumeration a code's reason is a member of.</summary>
    internal const string ReasonType = "VmReason";

    /// <summary>What a site whose reason the rule cannot read is recorded with.</summary>
    internal const string NoReason = "(none)";

    /// <summary>The registry's columns, in order.</summary>
    internal static readonly string[] RegistryColumns = ["code", "name", "reason", "reachability", "case", "since"];

    /// <summary>The two reachability kinds a row may claim.</summary>
    internal static readonly string[] RegistryReachabilities = ["corpus", "defensive"];

    /// <summary>
    /// The rows allowed to say no artifact reaches them, each with the reason, in the words the
    /// registry row and the corpus manifest must both use.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The list is here and not in the registry</b>, which is what makes it a list: a registry that
    /// could admit its own defensive rows could excuse any row from the corpus by being edited. A new
    /// defensive row is an edit to this array, in the test assembly, beside the rule that trusts it.
    /// </para>
    /// <para>
    /// <c>ReaderStopped</c> is the default arm of the reader's mapping of a latched bounded-read status.
    /// Every status a failed read can latch has an arm of its own, so the default answers only a status
    /// no read leaves latched. <c>VerifierDefect</c>, which the enumeration also calls defensive, is NOT
    /// here: the corpus reaches it through hooks that break the hook contract on purpose, and a row that
    /// the corpus reaches is held to the corpus.
    /// </para>
    /// </remarks>
    internal static readonly DefensiveCode[] DefensiveCodes =
    [
        new(3903, "ReaderStopped",
            "The reader's mapping of a latched bounded-read status (ADR 0011, C2) has an arm for every status a failed read can latch; " +
            "the default arm answers a status no read produces, Ok included, which a failed read never leaves latched."),
    ];

    /// <summary>One row of the registry, with the line it is on.</summary>
    internal sealed record RegistryRow(int Line, int Code, string Name, string Reason, string Reachability, string Case, int Since);

    /// <summary>
    /// A registry, parsed: its rows, the revision it states (-1 when it states none, 0 when it states
    /// one the rule cannot read), and every line the rule could not read as a row.
    /// </summary>
    internal sealed record Registry(IReadOnlyList<RegistryRow> Rows, int Revision, IReadOnlyList<string> Problems);

    /// <summary>One place a code is handed to a refusal, with the reason beside it.</summary>
    internal sealed record EmissionSite(string File, int Line, string Code, string Reason);

    /// <summary>
    /// What the source says: every emission site, and every use of the code type the rule could not
    /// follow to one.
    /// </summary>
    internal sealed record Emissions(IReadOnlyList<EmissionSite> Sites, IReadOnlyList<string> Unreadable);

    /// <summary>One corpus entry, reduced to what a registry row is compared with.</summary>
    internal sealed record CorpusAnswer(
        string Id,
        string Pinning,
        string ExpectedOutcome,
        int ExpectedCode,
        string ExpectedReason,
        string RecordedOutcome,
        int RecordedCode,
        string RecordedReason);

    /// <summary>A code no artifact reaches, and why.</summary>
    internal sealed record DefensiveCode(int Code, string Name, string Why);

    /// <summary>The corpus manifest, reduced to its entries and its own list of defensive codes.</summary>
    internal sealed record CorpusManifest(IReadOnlyList<CorpusAnswer> Entries, IReadOnlyList<DefensiveCode> Defensive);

    /// <summary>Parses a registry, so a witness is parsed by the code that parses the real file.</summary>
    /// <remarks>
    /// Strict, as rule N5's reader is: a row with the wrong number of columns, or a code or a
    /// <c>since</c> that is not a number, is reported rather than read as a row making fewer claims.
    /// </remarks>
    internal static Registry ReadRegistry(string text, string source)
    {
        var rows = new List<RegistryRow>();
        var problems = new List<string>();
        var revision = -1;
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var number = index + 1;

            if (line.StartsWith("# registry-revision:", StringComparison.Ordinal))
            {
                if (revision != -1)
                {
                    problems.Add($"{source}({number}): the registry states its revision a second time");
                }

                revision = int.TryParse(
                    line["# registry-revision:".Length..].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var stated) && stated > 0
                    ? stated
                    : 0;

                continue;
            }

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != RegistryColumns.Length)
            {
                problems.Add($"{source}({number}): a row has {parts.Length} columns rather than {RegistryColumns.Length}: {line}");
                continue;
            }

            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var code) ||
                !int.TryParse(parts[5], NumberStyles.None, CultureInfo.InvariantCulture, out var since))
            {
                problems.Add($"{source}({number}): a row's code or since is not a number: {line}");
                continue;
            }

            rows.Add(new RegistryRow(number, code, parts[1], parts[2], parts[3], parts[4], since));
        }

        return new Registry(rows, revision, problems);
    }

    /// <summary>The members of <c>UbcDiagnosticCode</c> with their numbers, in declaration order.</summary>
    internal static IReadOnlyList<(string Name, int Value)> DiagnosticVocabulary(SyntaxTree tree, List<string> problems) =>
        EnumValues(tree.GetRoot(), DiagnosticCodeType, problems)
            .Select(static member => (member.Key, member.Value))
            .OrderBy(static member => member.Value)
            .ToArray();

    /// <summary>Every covered source file of the universal bytecode.</summary>
    internal static IReadOnlyList<AssuranceSourceFile> UbcSourceFiles() =>
        AssuranceSources.Files
            .Where(static file => string.Equals(file.Assembly, UbcAssembly, StringComparison.Ordinal))
            .ToArray();

    /// <summary>
    /// Every site in <paramref name="files"/> that hands a <c>UbcDiagnosticCode</c> member to a
    /// refusal, with the core reason it carries, and every use of the type the rule cannot follow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A site is the call whose argument list a member of the code type is written in</b>, the
    /// nearest one, and its reason is every <c>VmReason</c> member written in the same argument list.
    /// That reads every shape the universal bytecode emits in: <c>UbcRefusal.Invalid(code, reason,
    /// ...)</c> in the reader and in each arm of its <c>FromReader</c> mapping, and the walk's
    /// <c>Invalid(code, reason, ...)</c> and <c>Fail(code, reason, ...)</c>. A call that names no
    /// reason is a FORWARDING site when the method it calls takes a code and supplies the reason
    /// from its own body - the walk's <c>Or</c>, which always passes <c>InconsistentStructure</c> - and
    /// the reason is read from that body rather than listed here, through any chain of such helpers,
    /// so a second one needs no change to the rule. A call that names two reasons is two sites, which
    /// is how a conditional reason becomes a code emitted with two.
    /// </para>
    /// <para>
    /// <b>What cannot be followed is reported, not skipped.</b> A member written outside the argument
    /// list of any call - held in a local, returned from a lambda - reaches a refusal the rule cannot
    /// see, and so does the type named anywhere but in a member access or a parameter's type: a cast
    /// of an integer to it, an alias or a <c>using static</c> of it. Each is a message, so the reading
    /// cannot be escaped by writing the same emission a second way. Syntax rather than semantics,
    /// because this project does not reference the assembly (rule U1's reference set is the
    /// assembly's; the architecture tests read it as source and as metadata), and what that costs is
    /// stated: a helper is resolved by its simple name, across the files read together.
    /// </para>
    /// </remarks>
    internal static Emissions ReadEmissions(IEnumerable<AssuranceSourceFile> files)
    {
        var parsed = files.Select(static file => (File: file, Root: file.Tree.GetRoot())).ToArray();
        var forwarding = ForwardingHelpers(parsed.Select(static unit => unit.Root));
        var sites = new List<EmissionSite>();
        var unreadable = new List<string>();

        foreach (var (file, root) in parsed)
        {
            foreach (var name in root.DescendantNodes().OfType<IdentifierNameSyntax>()
                         .Where(static name => name.Identifier.ValueText == DiagnosticCodeType))
            {
                // The type as written: the name itself, or the qualified access or name ending in it.
                SyntaxNode type = name.Parent switch
                {
                    MemberAccessExpressionSyntax qualified when qualified.Name == name => qualified,
                    QualifiedNameSyntax qualified when qualified.Right == name => qualified,
                    AliasQualifiedNameSyntax qualified when qualified.Name == name => qualified,
                    _ => name,
                };

                var line = file.Tree.GetLineSpan(type.Span).StartLinePosition.Line + 1;

                if (type.Parent is MemberAccessExpressionSyntax access && access.Expression == type)
                {
                    var member = access.Name.Identifier.ValueText;

                    if (EmittingCall(access) is not { } call)
                    {
                        unreadable.Add(
                            $"{file.RelativePath}({line}) uses {DiagnosticCodeType}.{member} outside the argument list " +
                            "of a call, so the rule cannot read the reason it is emitted with");
                        continue;
                    }

                    var reasons = Reasons(call, forwarding);

                    if (reasons.Count == 0)
                    {
                        sites.Add(new EmissionSite(file.RelativePath, line, member, NoReason));
                    }

                    foreach (var reason in reasons)
                    {
                        sites.Add(new EmissionSite(file.RelativePath, line, member, reason));
                    }
                }
                else if (type.Parent is ParameterSyntax parameter && parameter.Type == type)
                {
                    // A helper that takes a code: its callers are the sites, and a forwarding one's
                    // reason was read from its body above.
                }
                else
                {
                    unreadable.Add(
                        $"{file.RelativePath}({line}) names {DiagnosticCodeType} in {Describe(type.Parent)}, which the " +
                        "rule cannot follow to an emission");
                }
            }
        }

        return new Emissions(sites, unreadable);

        static string Describe(SyntaxNode? node) => node switch
        {
            CastExpressionSyntax => "a cast",
            UsingDirectiveSyntax { Alias: not null } => "an alias directive",
            UsingDirectiveSyntax => "a using static directive",
            null => "nothing",
            _ => $"a node of kind {node.Kind()}",
        };
    }

    /// <summary>The nearest call whose argument list <paramref name="access"/> is written in, or null.</summary>
    private static InvocationExpressionSyntax? EmittingCall(MemberAccessExpressionSyntax access)
    {
        for (SyntaxNode? node = access; node is not null; node = node.Parent)
        {
            switch (node)
            {
                case ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax call } }:
                    return call;
                case StatementSyntax or MemberDeclarationSyntax or AnonymousFunctionExpressionSyntax:
                    return null;
            }
        }

        return null;
    }

    /// <summary>The reasons a call names in its argument list, or supplies through a forwarding helper.</summary>
    private static IReadOnlyList<string> Reasons(InvocationExpressionSyntax call, IReadOnlyDictionary<string, IReadOnlySet<string>> forwarding)
    {
        var named = NamedReasons(call);

        if (named.Count > 0)
        {
            return named;
        }

        return Rightmost(call.Expression) is { } callee && forwarding.TryGetValue(callee, out var forwarded)
            ? forwarded.Order(StringComparer.Ordinal).ToArray()
            : [];
    }

    /// <summary>Every <c>VmReason</c> member written in a call's argument list, once each.</summary>
    private static IReadOnlyList<string> NamedReasons(InvocationExpressionSyntax call) =>
        call.ArgumentList.Arguments
            .SelectMany(static argument => argument.DescendantNodesAndSelf())
            .OfType<MemberAccessExpressionSyntax>()
            .Where(static access => string.Equals(Rightmost(access.Expression), ReasonType, StringComparison.Ordinal))
            .Select(static access => access.Name.Identifier.ValueText)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// Every method or local function that takes a code and supplies the reason itself, by simple
    /// name, with the reasons it supplies - followed through chains of such helpers to a fixed point.
    /// </summary>
    private static IReadOnlyDictionary<string, IReadOnlySet<string>> ForwardingHelpers(IEnumerable<SyntaxNode> roots)
    {
        var helpers = new List<(string Name, string Parameter, SyntaxNode Body)>();

        foreach (var parameter in roots.SelectMany(static root => root.DescendantNodes().OfType<ParameterSyntax>()))
        {
            if (!string.Equals(Rightmost(parameter.Type), DiagnosticCodeType, StringComparison.Ordinal))
            {
                continue;
            }

            switch (parameter.Parent?.Parent)
            {
                case MethodDeclarationSyntax method:
                    helpers.Add((method.Identifier.ValueText, parameter.Identifier.ValueText, method));
                    break;
                case LocalFunctionStatementSyntax function:
                    helpers.Add((function.Identifier.ValueText, parameter.Identifier.ValueText, function));
                    break;
            }
        }

        var forwarding = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal);

        for (var changed = true; changed;)
        {
            changed = false;

            foreach (var (name, parameter, body) in helpers)
            {
                var supplied = new SortedSet<string>(StringComparer.Ordinal);

                foreach (var call in body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                             .Where(call => call.ArgumentList.Arguments.Any(argument =>
                                 argument.Expression is IdentifierNameSyntax passed &&
                                 passed.Identifier.ValueText == parameter)))
                {
                    var named = NamedReasons(call);

                    if (named.Count > 0)
                    {
                        supplied.UnionWith(named);
                    }
                    else if (Rightmost(call.Expression) is { } callee && forwarding.TryGetValue(callee, out var forwarded))
                    {
                        supplied.UnionWith(forwarded);
                    }
                }

                if (supplied.Count == 0)
                {
                    continue;
                }

                if (forwarding.TryGetValue(name, out var known))
                {
                    supplied.UnionWith(known);

                    if (supplied.Count == known.Count)
                    {
                        continue;
                    }
                }

                forwarding[name] = supplied;
                changed = true;
            }
        }

        return forwarding;
    }

    /// <summary>Reads the corpus manifest's entries and its <c>defensiveCodes</c> list.</summary>
    internal static CorpusManifest ReadCorpusManifest(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var entries = root.GetProperty("entries").EnumerateArray()
            .Select(static entry =>
            {
                var expected = entry.GetProperty("expected");
                var recorded = entry.GetProperty("recorded");

                return new CorpusAnswer(
                    entry.GetProperty("id").GetString()!,
                    entry.GetProperty("pinning").GetString()!,
                    expected.GetProperty("outcome").GetString()!,
                    expected.GetProperty("profileDiagnosticCode").GetInt32(),
                    expected.GetProperty("reason").GetString()!,
                    recorded.GetProperty("outcome").GetString()!,
                    recorded.GetProperty("profileDiagnosticCode").GetInt32(),
                    recorded.GetProperty("reason").GetString()!);
            })
            .ToArray();

        var defensive = root.TryGetProperty("defensiveCodes", out var listed)
            ? listed.EnumerateArray()
                .Select(static code => new DefensiveCode(
                    code.GetProperty("code").GetInt32(),
                    code.GetProperty("name").GetString()!,
                    code.GetProperty("why").GetString()!))
                .ToArray()
            : [];

        return new CorpusManifest(entries, defensive);
    }

    /// <summary>
    /// U8's first clause: the registry states its revision, and it and the vocabulary are the same
    /// set - every member has exactly one row, every row names a member of its number, and no row
    /// dates from a revision the registry does not have.
    /// </summary>
    internal static IEnumerable<string> U8Vocabulary(Registry registry, IReadOnlyList<(string Name, int Value)> vocabulary)
    {
        foreach (var problem in registry.Problems)
        {
            yield return problem;
        }

        if (registry.Revision == -1)
        {
            yield return "the registry states no revision of its own";
        }
        else if (registry.Revision == 0)
        {
            yield return "the registry states a revision that is not a positive integer";
        }

        if (vocabulary.Count == 0)
        {
            yield return $"no member of {DiagnosticCodeType} was read, so the registry was compared with nothing";
        }

        if (registry.Rows.Count == 0)
        {
            yield return "the registry has no row, so the vocabulary was compared with nothing";
        }

        foreach (var duplicate in registry.Rows.GroupBy(static row => row.Code).Where(static group => group.Count() > 1))
        {
            yield return $"the registry has {duplicate.Count()} rows for code {duplicate.Key}";
        }

        foreach (var duplicate in registry.Rows.GroupBy(static row => row.Name, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            yield return $"the registry has {duplicate.Count()} rows named {duplicate.Key}";
        }

        var byNumber = vocabulary.ToLookup(static member => member.Value);

        foreach (var (name, value) in vocabulary)
        {
            if (!registry.Rows.Any(row => row.Code == value && string.Equals(row.Name, name, StringComparison.Ordinal)))
            {
                yield return $"{DiagnosticCodeType} declares {name} = {value} and the registry has no row for it";
            }
        }

        foreach (var row in registry.Rows)
        {
            if (!byNumber[row.Code].Any(member => string.Equals(member.Name, row.Name, StringComparison.Ordinal)))
            {
                yield return byNumber[row.Code].FirstOrDefault() is { Name: { } actual }
                    ? $"the row for {row.Code} names {row.Name}, which is not a member of that number: {DiagnosticCodeType}.{actual} is"
                    : $"the row for {row.Code} names {row.Name}, and {DiagnosticCodeType} has no member of that number";
            }

            if (registry.Revision > 0 && (row.Since < 1 || row.Since > registry.Revision))
            {
                yield return $"the row for {row.Code} dates from revision {row.Since}, and the registry is at revision {registry.Revision}";
            }
        }
    }

    /// <summary>
    /// U8's second clause: every code maps onto exactly one core reason, that reason is a member of
    /// <c>VmReason</c>, it is the reason every emission site in the source of <c>Broiler.VM.Ubc</c>
    /// carries, and no declared code is emitted by nothing.
    /// </summary>
    internal static IEnumerable<string> U8Reasons(
        Registry registry,
        IReadOnlyList<(string Name, int Value)> vocabulary,
        Emissions emissions,
        IReadOnlyCollection<string> coreReasons)
    {
        var members = vocabulary.Select(static member => member.Name).ToHashSet(StringComparer.Ordinal);
        var rows = registry.Rows
            .GroupBy(static row => row.Name, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        foreach (var row in registry.Rows.Where(row => !coreReasons.Contains(row.Reason, StringComparer.Ordinal)))
        {
            yield return $"the row for {row.Code} {row.Name} names the reason {row.Reason}, which is not a member of {ReasonType}";
        }

        foreach (var message in emissions.Unreadable)
        {
            yield return message;
        }

        if (emissions.Sites.Count == 0)
        {
            yield return $"no emission site was read from {UbcAssembly}, so no reason was compared";
        }

        foreach (var site in emissions.Sites)
        {
            if (!members.Contains(site.Code))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code}, which is not a member of {DiagnosticCodeType}";
            }
            else if (string.Equals(site.Reason, NoReason, StringComparison.Ordinal))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code} with no reason the rule can read";
            }
            else if (rows.TryGetValue(site.Code, out var row) && !string.Equals(site.Reason, row.Reason, StringComparison.Ordinal))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code} with {site.Reason}, and the registry says {row.Reason}";
            }
        }

        foreach (var code in emissions.Sites
                     .Where(static site => !string.Equals(site.Reason, NoReason, StringComparison.Ordinal))
                     .GroupBy(static site => site.Code, StringComparer.Ordinal)
                     .OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            var reasons = code
                .GroupBy(static site => site.Reason, StringComparer.Ordinal)
                .OrderBy(static group => group.Key, StringComparer.Ordinal)
                .ToArray();

            if (reasons.Length > 1)
            {
                yield return
                    $"{code.Key} is emitted with {reasons.Length} reasons: " +
                    string.Join("; ", reasons.Select(static reason =>
                        $"{reason.Key} at {string.Join(", ", reason.Select(static site => $"{site.File}({site.Line})"))}"));
            }
        }

        var emitted = emissions.Sites.Select(static site => site.Code).ToHashSet(StringComparer.Ordinal);

        foreach (var (name, _) in vocabulary.Where(member => !emitted.Contains(member.Name)))
        {
            yield return $"{name} is declared and no site in {UbcAssembly} emits it";
        }
    }

    /// <summary>
    /// U8's third clause: every row is reachable from a named case - a corpus row names an entry of the
    /// retained corpus pinned Exact whose expected and recorded answers are an invalid artifact with
    /// exactly that code and that reason, and a defensive row is one <paramref name="admitted"/> lists,
    /// in the words it lists it, and one the corpus manifest lists the same way.
    /// </summary>
    internal static IEnumerable<string> U8Reachability(
        Registry registry, CorpusManifest corpus, IReadOnlyList<DefensiveCode> admitted)
    {
        var entries = corpus.Entries
            .GroupBy(static entry => entry.Id, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        if (entries.Count == 0)
        {
            yield return "the corpus manifest has no entry, so no row's case was compared";
        }

        var excused = admitted.ToDictionary(static code => code.Code);

        foreach (var row in registry.Rows)
        {
            var name = $"the row for {row.Code} {row.Name}";

            switch (row.Reachability)
            {
                case "corpus":
                {
                    if (excused.ContainsKey(row.Code))
                    {
                        yield return $"{name} is admitted as unreachable and claims to be reachable";
                    }

                    if (!entries.TryGetValue(row.Case, out var entry))
                    {
                        yield return $"{name} names the entry {row.Case}, and the corpus manifest has no entry of that id";
                        break;
                    }

                    if (!string.Equals(entry.Pinning, "Exact", StringComparison.Ordinal))
                    {
                        yield return
                            $"{name} names the entry {row.Case}, which is pinned {entry.Pinning} rather than Exact, " +
                            "so its answer is an observation nobody wrote down as expected";
                    }

                    if (!string.Equals(entry.ExpectedOutcome, "InvalidArtifact", StringComparison.Ordinal) || entry.ExpectedCode != row.Code)
                    {
                        yield return
                            $"{name} names the entry {row.Case}, which expects {entry.ExpectedOutcome} with code " +
                            $"{entry.ExpectedCode} rather than InvalidArtifact with code {row.Code}";
                    }
                    else if (!string.Equals(entry.ExpectedReason, row.Reason, StringComparison.Ordinal))
                    {
                        yield return $"{name} names the entry {row.Case}, which expects the reason {entry.ExpectedReason} rather than {row.Reason}";
                    }

                    if (!string.Equals(entry.RecordedOutcome, entry.ExpectedOutcome, StringComparison.Ordinal) ||
                        entry.RecordedCode != entry.ExpectedCode ||
                        !string.Equals(entry.RecordedReason, entry.ExpectedReason, StringComparison.Ordinal))
                    {
                        yield return
                            $"{name} names the entry {row.Case}, whose recorded answer ({entry.RecordedOutcome}, " +
                            $"{entry.RecordedCode}, {entry.RecordedReason}) is not its expected one";
                    }

                    break;
                }

                case "defensive":
                    if (!excused.TryGetValue(row.Code, out var listed) || !string.Equals(listed.Name, row.Name, StringComparison.Ordinal))
                    {
                        yield return $"{name} claims to be unreachable and is not one of the rows this rule admits";
                    }
                    else if (!string.Equals(listed.Why, row.Case, StringComparison.Ordinal))
                    {
                        yield return
                            $"{name} says why it is unreachable in words the rule's list does not: the rule says " +
                            $"\"{listed.Why}\"";
                    }

                    break;

                default:
                    yield return $"{name} claims the reachability {row.Reachability}, which is not one";
                    break;
            }
        }

        foreach (var code in admitted)
        {
            if (!registry.Rows.Any(row => row.Code == code.Code))
            {
                yield return $"the rule admits {code.Code} {code.Name} as unreachable and the registry has no row for it";
            }

            foreach (var entry in corpus.Entries.Where(entry =>
                         string.Equals(entry.Pinning, "Exact", StringComparison.Ordinal) &&
                         string.Equals(entry.ExpectedOutcome, "InvalidArtifact", StringComparison.Ordinal) &&
                         entry.ExpectedCode == code.Code))
            {
                yield return $"the rule admits {code.Code} {code.Name} as unreachable, and the corpus entry {entry.Id} reaches it";
            }

            if (!corpus.Defensive.Contains(code))
            {
                yield return
                    $"the rule admits {code.Code} {code.Name} as unreachable, and the corpus manifest's defensiveCodes " +
                    "does not list it in the same words";
            }
        }

        foreach (var code in corpus.Defensive.Where(code => !admitted.Contains(code)))
        {
            yield return $"the corpus manifest lists {code.Code} {code.Name} as defensive, and this rule does not admit it in those words";
        }
    }

    /// <summary>U8 over the checkout: the registry, the vocabulary, the source and the corpus, read off disk.</summary>
    internal static IReadOnlyList<string> U8Violations()
    {
        var problems = new List<string>();
        var registry = ReadRegistry(File.ReadAllText(RootPath(RegistryFile)), RegistryFile);
        var vocabulary = DiagnosticVocabulary(AssuranceSources.File(DiagnosticsFile).Tree, problems);
        var emissions = ReadEmissions(UbcSourceFiles());
        var corpus = ReadCorpusManifest(File.ReadAllText(RootPath(CorpusManifestFile)));

        return
        [
            .. problems,
            .. U8Vocabulary(registry, vocabulary),
            .. U8Reasons(registry, vocabulary, emissions, Enum.GetNames<VmReason>()),
            .. U8Reachability(registry, corpus, DefensiveCodes),
        ];
    }
}
