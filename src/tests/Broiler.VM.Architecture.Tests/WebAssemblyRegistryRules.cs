using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W3: the WebAssembly profile's published diagnostic registry, held to the enumeration that
/// declares its codes, to the source that emits them, to the corpus and the execution checks that
/// reach them, and to the corpus manifest its revision dates.
/// </summary>
/// <remarks>
/// <para>
/// <b>The shape is rule U8's</b>, and it is written again here rather than reused, for the reason U8
/// gives for not reusing group N's: the subjects differ in ways that would each be a parameter. This
/// registry has three carriers where U8's has one, so a row states the passes that emit its code and
/// the carrier those passes use, and the rule holds both to the source. Its corpus is the harness
/// root's pipe-separated manifest rather than a JSON one. Its traps travel in a payload and are
/// reached by the execution checks rather than by the corpus. And its source emits in two shapes U8's
/// does not: the family hook's reason TABLE, a switch from a code to its reason that a helper passes
/// to the core, and the family's PAYLOAD MAPPING, a switch from a trap kind to its code.
/// </para>
/// <para>
/// <b>Every input is read off disk and none is derived from another</b>: the registry file, the
/// enumeration's declaration, the profile assembly's source parsed with Roslyn, the corpus manifest,
/// the execution checks and the corpus writer. The two lists the rule does carry - the rows allowed
/// to say no named case reaches them, and the uses of the code type that emit nothing - are carried
/// because carrying them is the point: a row that excuses itself, or a use that escapes the reading,
/// is an edit to this file.
/// </para>
/// </remarks>
internal static class WebAssemblyRegistryRules
{
    /// <summary>The published registry.</summary>
    internal const string RegistryFile = "src/Broiler.VM.Profile.WebAssembly/docs/diagnostics/registry.txt";

    /// <summary>The file that declares the code vocabulary.</summary>
    internal const string DiagnosticsFile = "src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs";

    /// <summary>The retained corpus manifest a corpus row names an entry of.</summary>
    internal const string CorpusManifestFile = "src/tests/wasm/corpus/corpus.manifest";

    /// <summary>The harness root's execution checks, which name the trap kinds an execution row claims.</summary>
    internal const string ExecutionChecksFile =
        "src/compositions/Broiler.VM.Composition.WebAssembly.Harness/ExecutionChecks.cs";

    /// <summary>The corpus writer, which dates every manifest it writes with a registry revision.</summary>
    internal const string CorpusWriterFile =
        "src/compositions/Broiler.VM.Composition.WebAssembly.Harness/CorpusStore.cs";

    /// <summary>The enumeration whose members are the codes.</summary>
    internal const string CodeType = "WebAssemblyDiagnosticCode";

    /// <summary>The core enumeration a code's reason is a member of.</summary>
    internal const string ReasonType = "VmReason";

    /// <summary>The enumeration a payload mapping switches over.</summary>
    internal const string TrapKindType = "WasmTrapKind";

    /// <summary>What a site whose reason the rule cannot read is recorded with.</summary>
    internal const string NoReason = "(none)";

    /// <summary>The reason the core gives every profile fault, and so every payload code.</summary>
    internal const string PayloadReason = "ProfileFaultUnspecified";

    /// <summary>The writer's constant that dates a manifest.</summary>
    internal const string RevisionConstant = "RegistryRevision";

    /// <summary>The registry's columns, in order.</summary>
    internal static readonly string[] Columns =
        ["code", "name", "pass", "carrier", "reason", "reachability", "case", "since"];

    /// <summary>The passes a row may name, in the order a row names them.</summary>
    internal static readonly string[] Passes =
        ["decode", "validate", "hook", "translate", "execute", "bare-module"];

    /// <summary>The carriers a row may name, in the order a row names them.</summary>
    internal static readonly string[] Carriers = ["translation", "outcome", "payload"];

    /// <summary>The four reachability kinds a row may claim.</summary>
    internal static readonly string[] Reachabilities = ["corpus", "execution", "unreached", "retired"];

    /// <summary>
    /// The pass each file of the profile assembly that emits a code belongs to. A file that emits and
    /// is not here is reported rather than guessed at.
    /// </summary>
    /// <remarks>
    /// <c>WebAssemblyDiagnostics.cs</c> is two passes because its <c>WasmRefusal</c> mapping is the one
    /// the decoder and the validator share, and says so: a code only it emits is emitted by both.
    /// </remarks>
    internal static readonly IReadOnlyDictionary<string, string[]> PassesOfFile =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs"] = ["decode"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs"] = ["validate"],
            ["src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs"] = ["decode", "validate"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmFamilyVerifier.cs"] = ["hook"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmFamilyData.cs"] = ["hook"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmLowering.cs"] = ["translate"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmTranslator.cs"] = ["translate"],
            ["src/Broiler.VM.Profile.WebAssembly/WasmFamily.cs"] = ["execute"],
        };

    /// <summary>
    /// The rows allowed to say no named case reaches them, each with the reason, in the words the
    /// registry row must use.
    /// </summary>
    /// <remarks>
    /// <b>The list is here and not in the registry</b>, as rule U8's defensive list is: a registry that
    /// could admit its own unreached rows could excuse any row from the corpus by being edited. They are
    /// the part of WA-3's gate clause "every code in it is reachable from a named case" that is not met,
    /// and the ledger names them as that.
    /// </remarks>
    internal static readonly UnreachedCode[] Unreached =
    [
        new(2107, "ReaderMalformedEncoding",
            "The profile reads through none of the bounded reader's operations that latch a malformation but the section exit, " +
            "and the decoder answers that one as SectionLengthMismatch."),
        .. HookOnly(
            (2305, "LimitsMinimumAboveMaximum"),
            (2306, "MemoryPagesAboveFormatMaximum"),
            (2851, "ModuleDefinitionsMissing"),
            (2852, "ModuleDefinitionsTruncated"),
            (2853, "ModuleDefinitionsTrailingBytes"),
            (2854, "ModuleDefinitionsVersionUnsupported"),
            (2855, "ModuleDefinitionsMalformedPresence"),
            (2856, "GlobalInitialValueOutOfRange"),
            (2857, "ValueSlotNotAdmitted"),
            (2858, "ExportedFunctionNotAnEntry"),
            (2859, "PositionsNotOrdered"),
            (2860, "GlobalRowTypeMismatch")),
        new(2871, "TranslationLocalsAboveMaximum", PastABound),
        new(2872, "TranslationOperandHeightAboveMaximum", PastABound),
        new(2873, "TranslationJumpTablesAboveMaximum", PastABound),
        new(2874, "TranslationOperandOutOfRange",
            "No module the validator admits reaches it, as the enumeration's remark on it says."),
        new(2901, "VerifierDefect",
            "It is the answer a defect in this assembly gives, and a retained entry recording it would be a bug report."),
        new(2902, "ReaderStopped",
            "The reader's mapping has an arm for every status the core's bounded reader defines, " +
            "so its default arm answers a status no failed read latches."),
        new(3007, "TrapUndefinedElement",
            "No trap this build raises has that kind: an indirect call past the table's end is reported under the current " +
            "revision's name, 3006."),
    ];

    /// <summary>
    /// The uses of the code type that emit nothing and are not a parameter, each named by its file, the
    /// member it is in and its kind.
    /// </summary>
    /// <remarks>
    /// Any other use the reading cannot follow to an emission is reported. These two carry a code a
    /// pass already chose onto the translation's answer, and neither chooses one: a new cast in another
    /// member is a new place a number could be minted, and it fails the rule until it is listed here.
    /// </remarks>
    internal static readonly AdmittedUse[] AdmittedUses =
    [
        new("src/Broiler.VM.Profile.WebAssembly/WasmTranslator.cs", "Refused", "a cast",
            "re-carries the code a decoder's or validator's outcome already chose onto the translation's answer"),
        new("src/Broiler.VM.Profile.WebAssembly/WasmTranslator.cs", "Code", "a property type",
            "the translation's answer exposes the code it carries"),
    ];

    private const string PastABound =
        "No retained entry is a valid module past a bound one universal bytecode unit or artifact holds.";

    private static IEnumerable<UnreachedCode> HookOnly(params (int Code, string Name)[] codes) =>
        codes.Select(static code => new UnreachedCode(
            code.Code,
            code.Name,
            "Only the family hook emits it, over module definitions an artifact carries, and no retained entry is an " +
            "artifact written other than by the translator."));

    /// <summary>The carrier a pass's codes travel on.</summary>
    internal static string CarrierOf(string pass) => pass switch
    {
        "decode" or "validate" or "translate" => "translation",
        "hook" or "bare-module" => "outcome",
        "execute" => "payload",
        _ => "?",
    };

    /// <summary>One row of the registry, with the line it is on.</summary>
    internal sealed record RegistryRow(
        int Line, int Code, string Name, string Pass, string Carrier, string Reason, string Reachability, string Case, int Since);

    /// <summary>A registry, parsed: its rows, the revision it states, and every line it could not read.</summary>
    internal sealed record Registry(IReadOnlyList<RegistryRow> Rows, int Revision, IReadOnlyList<string> Problems);

    /// <summary>One place a code is emitted, with the reason it carries.</summary>
    internal sealed record EmissionSite(string File, int Line, string Code, string Reason);

    /// <summary>One arm of a reason table: the code it names and the reason it gives.</summary>
    internal sealed record TableEntry(string File, int Line, string Table, string Code, string Reason);

    /// <summary>One arm of a payload mapping: the trap kind it names, or null for the discard arm, and its code.</summary>
    internal sealed record PayloadArm(string File, int Line, string? Kind, string Code);

    /// <summary>
    /// What the source says: every emission site, every arm of a reason table and of a payload
    /// mapping, every member of the code type named anywhere, the admitted uses found, and every use
    /// the rule could not follow.
    /// </summary>
    internal sealed record Emissions(
        IReadOnlyList<EmissionSite> Sites,
        IReadOnlyList<TableEntry> Tables,
        IReadOnlyList<PayloadArm> Payload,
        IReadOnlyList<(string File, int Line, string Code)> Named,
        IReadOnlyList<AdmittedUse> Admitted,
        IReadOnlyList<string> Unreadable);

    /// <summary>One corpus manifest entry, reduced to what a registry row is compared with.</summary>
    internal sealed record CorpusEntry(string Name, string Outcome, string Reason, int Code, string Provenance);

    /// <summary>A code no named case reaches, and why.</summary>
    internal sealed record UnreachedCode(int Code, string Name, string Why);

    /// <summary>A use of the code type that emits nothing.</summary>
    internal sealed record AdmittedUse(string File, string Member, string Kind, string Why);

    // =============================================================================================
    // Reading
    // =============================================================================================

    /// <summary>Parses a registry, so a witness is parsed by the code that parses the real file.</summary>
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

                revision = StatedRevision(line);
                continue;
            }

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != Columns.Length)
            {
                problems.Add($"{source}({number}): a row has {parts.Length} columns rather than {Columns.Length}: {line}");
                continue;
            }

            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var code) ||
                !int.TryParse(parts[7], NumberStyles.None, CultureInfo.InvariantCulture, out var since))
            {
                problems.Add($"{source}({number}): a row's code or since is not a number: {line}");
                continue;
            }

            rows.Add(new RegistryRow(number, code, parts[1], parts[2], parts[3], parts[4], parts[5], parts[6], since));
        }

        return new Registry(rows, revision, problems);
    }

    /// <summary>The revision a text states on its first registry-revision line: -1 for none, 0 for one the rule cannot read.</summary>
    internal static int ManifestRevision(string text)
    {
        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            if (line.StartsWith("# registry-revision:", StringComparison.Ordinal))
            {
                return StatedRevision(line);
            }
        }

        return -1;
    }

    private static int StatedRevision(string line) =>
        int.TryParse(line["# registry-revision:".Length..].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var stated) &&
        stated > 0
            ? stated
            : 0;

    /// <summary>The members of the code enumeration with their numbers, in number order.</summary>
    internal static IReadOnlyList<(string Name, int Value)> Vocabulary(SyntaxTree tree, List<string> problems) =>
        UbcRules.EnumValues(tree.GetRoot(), CodeType, problems)
            .Select(static member => (member.Key, member.Value))
            .OrderBy(static member => member.Value)
            .ToArray();

    /// <summary>Every covered source file of the WebAssembly profile assembly.</summary>
    internal static IReadOnlyList<AssuranceSourceFile> ProfileSourceFiles() =>
        AssuranceSources.Files
            .Where(static file => string.Equals(file.Assembly, WebAssemblyFamilyRules.ProfileAssembly, StringComparison.Ordinal))
            .ToArray();

    /// <summary>Reads the corpus manifest's entries; a line the rule cannot split is a problem, not a shorter entry.</summary>
    internal static IReadOnlyList<CorpusEntry> ReadCorpus(string text, List<string> problems)
    {
        var entries = new List<CorpusEntry>();

        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != 10 ||
                !int.TryParse(parts[5], NumberStyles.None, CultureInfo.InvariantCulture, out var code))
            {
                problems.Add($"{CorpusManifestFile}: a line the rule cannot read as an entry: {line}");
                continue;
            }

            entries.Add(new CorpusEntry(parts[0], parts[3], parts[4], code, parts[8]));
        }

        return entries;
    }

    /// <summary>Every member of <c>WasmTrapKind</c> a source names.</summary>
    internal static IReadOnlySet<string> NamedTrapKinds(SyntaxTree tree) => tree.GetRoot()
        .DescendantNodes()
        .OfType<MemberAccessExpressionSyntax>()
        .Where(static access => string.Equals(Rightmost(access.Expression), TrapKindType, StringComparison.Ordinal))
        .Select(static access => access.Name.Identifier.ValueText)
        .ToHashSet(StringComparer.Ordinal);

    /// <summary>The literal value of the writer's revision constant, or -1 when there is none the rule can read.</summary>
    internal static int WriterRevision(SyntaxTree tree) => tree.GetRoot()
        .DescendantNodes()
        .OfType<FieldDeclarationSyntax>()
        .Where(static field => field.Modifiers.Any(SyntaxKind.ConstKeyword))
        .SelectMany(static field => field.Declaration.Variables)
        .Where(static variable => variable.Identifier.ValueText == RevisionConstant)
        .Select(static variable => variable.Initializer?.Value is LiteralExpressionSyntax { Token.Value: int value } ? value : -1)
        .DefaultIfEmpty(-1)
        .First();

    /// <summary>
    /// Every site in <paramref name="files"/> that emits a member of the code type, with the core
    /// reason it carries, and every use of the type the rule cannot follow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Four shapes are read, and anything else is reported.</b> A DIRECT site is the nearest call
    /// whose argument list a member is written in, and its reason is every <c>VmReason</c> member in
    /// that argument list. A FORWARDING site is such a call naming no reason, to a helper that takes a
    /// code and supplies the reason from its body - the validator's <c>Fail</c> and
    /// <c>PopOperand</c>, the lowering's <c>Refuse</c> - read through any chain of helpers. A TABLE
    /// site calls a helper whose body hands the core its code with the answer of a reason table - the
    /// hook's <c>Refuse</c>, which passes <c>ReasonOf(code)</c> - and its reason is the table's arm
    /// for that code, or the table's discard arm. A PAYLOAD site is an arm of a switch from a trap
    /// kind to a code, and its reason is the one the core gives every profile fault.
    /// </para>
    /// <para>
    /// <b>A helper is resolved by the type it is called on, its name and its argument count</b>, not
    /// by its simple name as rule U8's is: this assembly has two <c>Refuse</c> helpers that supply
    /// different reasons, one in the hook and one in the lowering, and reading them as one would give
    /// every hook refusal the lowering's reason. A call through an instance, whose type syntax cannot
    /// name, is resolved by name and argument count across the assembly, and a disagreement between
    /// two candidates becomes two reasons and is reported.
    /// </para>
    /// </remarks>
    internal static Emissions ReadEmissions(IEnumerable<AssuranceSourceFile> files)
    {
        var parsed = files.Select(static file => (File: file, Root: file.Tree.GetRoot())).ToArray();
        var unreadable = new List<string>();
        var tables = new Dictionary<string, ReasonTable>(StringComparer.Ordinal);
        var tableEntries = new List<TableEntry>();
        var tablePatterns = new HashSet<SyntaxNode>();
        var payload = new List<PayloadArm>();
        var payloadArms = new HashSet<SyntaxNode>();
        var payloadMethods = new HashSet<SyntaxNode>();

        foreach (var (file, root) in parsed)
        {
            ReadTables(file, root, tables, tableEntries, tablePatterns, unreadable);
            ReadPayloadMappings(file, root, payload, payloadArms, payloadMethods, unreadable);
        }

        var forwarding = ForwardingHelpers(parsed.Select(static unit => unit.Root), tables);
        var sites = new List<EmissionSite>();
        var named = new List<(string File, int Line, string Code)>();
        var admitted = new List<AdmittedUse>();

        foreach (var (file, root) in parsed)
        {
            foreach (var name in root.DescendantNodes().OfType<IdentifierNameSyntax>()
                         .Where(static name => name.Identifier.ValueText == CodeType))
            {
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
                    named.Add((file.RelativePath, line, member));

                    if (tablePatterns.Contains(access))
                    {
                        continue;
                    }

                    if (payloadArms.Contains(access))
                    {
                        sites.Add(new EmissionSite(file.RelativePath, line, member, PayloadReason));
                        continue;
                    }

                    if (EmittingCall(access) is not { } call)
                    {
                        unreadable.Add(
                            $"{file.RelativePath}({line}) uses {CodeType}.{member} outside the argument list of a call, " +
                            "so the rule cannot read the reason it is emitted with");
                        continue;
                    }

                    var reasons = Reasons(call, member, forwarding, tables);

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
                    // A helper's parameter: its callers are the sites.
                }
                else if (type.Parent is MethodDeclarationSyntax method && method.ReturnType == type && payloadMethods.Contains(method))
                {
                    // A payload mapping's return type: its arms are the sites.
                }
                else if (Admitted(file.RelativePath, type) is { } use)
                {
                    admitted.Add(use);
                }
                else
                {
                    unreadable.Add(
                        $"{file.RelativePath}({line}) names {CodeType} in {Describe(type)}, which the rule cannot follow to an emission");
                }
            }
        }

        return new Emissions(sites, tableEntries, payload, named, admitted, unreadable);
    }

    private sealed record ReasonTable(IReadOnlyDictionary<string, string> Arms, string? Default);

    private sealed record Forward(IReadOnlySet<string> Reasons, IReadOnlySet<string> Tables);

    /// <summary>
    /// Every method that answers a <c>VmReason</c> from one code parameter with a switch over it, keyed
    /// by its type and its name: its arms, and the pattern nodes that name codes in them.
    /// </summary>
    private static void ReadTables(
        AssuranceSourceFile file,
        SyntaxNode root,
        Dictionary<string, ReasonTable> tables,
        List<TableEntry> entries,
        HashSet<SyntaxNode> patterns,
        List<string> unreadable)
    {
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (!string.Equals(Rightmost(method.ReturnType), ReasonType, StringComparison.Ordinal) ||
                method.ParameterList.Parameters.Count != 1 ||
                !string.Equals(Rightmost(method.ParameterList.Parameters[0].Type), CodeType, StringComparison.Ordinal))
            {
                continue;
            }

            var parameter = method.ParameterList.Parameters[0].Identifier.ValueText;
            var line = file.Tree.GetLineSpan(method.Span).StartLinePosition.Line + 1;
            var key = $"{EnclosingType(method)}.{method.Identifier.ValueText}";

            if (method.ExpressionBody?.Expression is not SwitchExpressionSyntax body ||
                body.GoverningExpression is not IdentifierNameSyntax governing ||
                governing.Identifier.ValueText != parameter)
            {
                unreadable.Add(
                    $"{file.RelativePath}({line}) answers a {ReasonType} from a {CodeType} in {method.Identifier.ValueText}, " +
                    "which is not a switch over the code the rule can read");
                continue;
            }

            var arms = new Dictionary<string, string>(StringComparer.Ordinal);
            string? fallback = null;

            foreach (var arm in body.Arms)
            {
                var armLine = file.Tree.GetLineSpan(arm.Span).StartLinePosition.Line + 1;
                var reason = arm.Expression is MemberAccessExpressionSyntax given &&
                    string.Equals(Rightmost(given.Expression), ReasonType, StringComparison.Ordinal)
                        ? given.Name.Identifier.ValueText
                        : null;

                if (reason is null || arm.WhenClause is not null)
                {
                    unreadable.Add($"{file.RelativePath}({armLine}) is an arm of {method.Identifier.ValueText} the rule cannot read");
                    continue;
                }

                foreach (var pattern in Alternatives(arm.Pattern))
                {
                    if (pattern is DiscardPatternSyntax)
                    {
                        fallback = reason;
                    }
                    else if (pattern is ConstantPatternSyntax { Expression: MemberAccessExpressionSyntax code } &&
                        string.Equals(Rightmost(code.Expression), CodeType, StringComparison.Ordinal))
                    {
                        var member = code.Name.Identifier.ValueText;
                        patterns.Add(code);

                        if (!arms.TryAdd(member, reason))
                        {
                            unreadable.Add($"{file.RelativePath}({armLine}) names {member} a second time in {method.Identifier.ValueText}");
                        }

                        entries.Add(new TableEntry(file.RelativePath, armLine, key, member, reason));
                    }
                    else
                    {
                        unreadable.Add($"{file.RelativePath}({armLine}) is a pattern of {method.Identifier.ValueText} the rule cannot read");
                    }
                }
            }

            tables[key] = new ReasonTable(arms, fallback);
        }
    }

    /// <summary>Every method that answers a code from a switch over a trap kind: its arms.</summary>
    private static void ReadPayloadMappings(
        AssuranceSourceFile file,
        SyntaxNode root,
        List<PayloadArm> payload,
        HashSet<SyntaxNode> arms,
        HashSet<SyntaxNode> methods,
        List<string> unreadable)
    {
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (!string.Equals(Rightmost(method.ReturnType), CodeType, StringComparison.Ordinal))
            {
                continue;
            }

            var line = file.Tree.GetLineSpan(method.Span).StartLinePosition.Line + 1;

            if (method.ExpressionBody?.Expression is not SwitchExpressionSyntax body)
            {
                unreadable.Add(
                    $"{file.RelativePath}({line}) answers a {CodeType} from {method.Identifier.ValueText}, " +
                    "which is not a switch the rule can read");
                continue;
            }

            methods.Add(method);

            foreach (var arm in body.Arms)
            {
                var armLine = file.Tree.GetLineSpan(arm.Span).StartLinePosition.Line + 1;

                if (arm.Expression is not MemberAccessExpressionSyntax code ||
                    !string.Equals(Rightmost(code.Expression), CodeType, StringComparison.Ordinal) ||
                    arm.WhenClause is not null)
                {
                    unreadable.Add($"{file.RelativePath}({armLine}) is an arm of {method.Identifier.ValueText} the rule cannot read");
                    continue;
                }

                string? kind;

                if (arm.Pattern is DiscardPatternSyntax)
                {
                    kind = null;
                }
                else if (arm.Pattern is ConstantPatternSyntax { Expression: MemberAccessExpressionSyntax trap } &&
                    string.Equals(Rightmost(trap.Expression), TrapKindType, StringComparison.Ordinal))
                {
                    kind = trap.Name.Identifier.ValueText;
                }
                else
                {
                    unreadable.Add($"{file.RelativePath}({armLine}) is a pattern of {method.Identifier.ValueText} the rule cannot read");
                    continue;
                }

                arms.Add(code);
                payload.Add(new PayloadArm(file.RelativePath, armLine, kind, code.Name.Identifier.ValueText));
            }
        }
    }

    /// <summary>The alternatives of an <c>or</c> pattern, or the pattern itself.</summary>
    private static IEnumerable<PatternSyntax> Alternatives(PatternSyntax pattern) =>
        pattern is BinaryPatternSyntax { OperatorToken.RawKind: (int)SyntaxKind.OrKeyword } either
            ? Alternatives(either.Left).Concat(Alternatives(either.Right))
            : [pattern];

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

    /// <summary>The reasons a site's call names, or supplies through a helper, for <paramref name="code"/>.</summary>
    private static IReadOnlyList<string> Reasons(
        InvocationExpressionSyntax call,
        string code,
        IReadOnlyDictionary<string, Forward> forwarding,
        IReadOnlyDictionary<string, ReasonTable> tables)
    {
        var named = NamedReasons(call);

        if (named.Count > 0)
        {
            return named;
        }

        var reasons = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var forward in Resolve(call, forwarding))
        {
            reasons.UnionWith(forward.Reasons);

            foreach (var table in forward.Tables)
            {
                var arms = tables[table];
                reasons.Add(arms.Arms.TryGetValue(code, out var reason) ? reason : arms.Default ?? NoReason);
            }
        }

        reasons.Remove(NoReason);
        return [.. reasons];
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
    /// The helpers a call may reach: by the type it names and its name and argument count, or, for a
    /// call through an instance, every helper of that name and argument count.
    /// </summary>
    private static IEnumerable<Forward> Resolve(InvocationExpressionSyntax call, IReadOnlyDictionary<string, Forward> forwarding)
    {
        var arity = call.ArgumentList.Arguments.Count;

        switch (call.Expression)
        {
            case IdentifierNameSyntax simple:
                return Lookup(EnclosingType(call), simple.Identifier.ValueText);
            case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax owner } qualified
                when char.IsUpper(owner.Identifier.ValueText[0]):
                return Lookup(owner.Identifier.ValueText, qualified.Name.Identifier.ValueText);
            case MemberAccessExpressionSyntax instance:
                var suffix = $".{instance.Name.Identifier.ValueText}/{arity}";
                return forwarding.Where(helper => helper.Key.EndsWith(suffix, StringComparison.Ordinal)).Select(static helper => helper.Value);
            default:
                return [];
        }

        IEnumerable<Forward> Lookup(string? type, string name) =>
            type is not null && forwarding.TryGetValue($"{type}.{name}/{arity}", out var forward) ? [forward] : [];
    }

    private static string? EnclosingType(SyntaxNode node) =>
        node.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;

    /// <summary>
    /// Every method or local function that takes a code and supplies the reason itself - a fixed one,
    /// or a reason table's answer for the code - keyed by its type, its name and its parameter count,
    /// followed through chains of such helpers to a fixed point.
    /// </summary>
    private static IReadOnlyDictionary<string, Forward> ForwardingHelpers(
        IEnumerable<SyntaxNode> roots, IReadOnlyDictionary<string, ReasonTable> tables)
    {
        var helpers = new List<(string Key, string Parameter, SyntaxNode Body)>();

        foreach (var parameter in roots.SelectMany(static root => root.DescendantNodes().OfType<ParameterSyntax>()))
        {
            if (!string.Equals(Rightmost(parameter.Type), CodeType, StringComparison.Ordinal))
            {
                continue;
            }

            switch (parameter.Parent?.Parent)
            {
                case MethodDeclarationSyntax method when !string.Equals(Rightmost(method.ReturnType), ReasonType, StringComparison.Ordinal):
                    helpers.Add((
                        $"{EnclosingType(method)}.{method.Identifier.ValueText}/{method.ParameterList.Parameters.Count}",
                        parameter.Identifier.ValueText,
                        method));
                    break;
                case LocalFunctionStatementSyntax function:
                    helpers.Add((
                        $"{EnclosingType(function)}.{function.Identifier.ValueText}/{function.ParameterList.Parameters.Count}",
                        parameter.Identifier.ValueText,
                        function));
                    break;
            }
        }

        var forwarding = new Dictionary<string, Forward>(StringComparer.Ordinal);

        for (var changed = true; changed;)
        {
            changed = false;

            foreach (var (key, parameter, body) in helpers)
            {
                var reasons = new SortedSet<string>(StringComparer.Ordinal);
                var answered = new SortedSet<string>(StringComparer.Ordinal);

                foreach (var call in body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                             .Where(call => call.ArgumentList.Arguments.Any(argument => IsParameter(argument.Expression, parameter))))
                {
                    var named = NamedReasons(call);

                    if (named.Count > 0)
                    {
                        reasons.UnionWith(named);
                    }
                    else
                    {
                        foreach (var forward in Resolve(call, forwarding))
                        {
                            reasons.UnionWith(forward.Reasons);
                            answered.UnionWith(forward.Tables);
                        }
                    }

                    // A reason table's answer for this code, handed to the core beside it.
                    foreach (var argument in call.ArgumentList.Arguments)
                    {
                        if (argument.Expression is InvocationExpressionSyntax { Expression: IdentifierNameSyntax table } asked &&
                            tables.ContainsKey($"{EnclosingType(asked)}.{table.Identifier.ValueText}") &&
                            asked.ArgumentList.Arguments.Count == 1 &&
                            IsParameter(asked.ArgumentList.Arguments[0].Expression, parameter))
                        {
                            answered.Add($"{EnclosingType(asked)}.{table.Identifier.ValueText}");
                        }
                    }
                }

                if (reasons.Count == 0 && answered.Count == 0)
                {
                    continue;
                }

                if (forwarding.TryGetValue(key, out var known))
                {
                    reasons.UnionWith(known.Reasons);
                    answered.UnionWith(known.Tables);

                    if (reasons.Count == known.Reasons.Count && answered.Count == known.Tables.Count)
                    {
                        continue;
                    }
                }

                forwarding[key] = new Forward(reasons, answered);
                changed = true;
            }
        }

        return forwarding;
    }

    /// <summary>Whether an argument is the parameter itself, cast or parenthesised or not.</summary>
    private static bool IsParameter(ExpressionSyntax expression, string parameter)
    {
        while (expression is CastExpressionSyntax or ParenthesizedExpressionSyntax)
        {
            expression = expression is CastExpressionSyntax cast ? cast.Expression : ((ParenthesizedExpressionSyntax)expression).Expression;
        }

        return expression is IdentifierNameSyntax passed && passed.Identifier.ValueText == parameter;
    }

    /// <summary>The admitted use a node is, or null.</summary>
    private static AdmittedUse? Admitted(string file, SyntaxNode type)
    {
        var (kind, member) = type.Parent switch
        {
            CastExpressionSyntax cast when cast.Type == type =>
                ("a cast", type.Ancestors().OfType<MemberDeclarationSyntax>().FirstOrDefault() switch
                {
                    MethodDeclarationSyntax method => method.Identifier.ValueText,
                    PropertyDeclarationSyntax property => property.Identifier.ValueText,
                    _ => null,
                }),
            PropertyDeclarationSyntax property when property.Type == type => ("a property type", property.Identifier.ValueText),
            _ => (null, null),
        };

        return AdmittedUses.FirstOrDefault(use =>
            string.Equals(use.File, file, StringComparison.Ordinal) &&
            string.Equals(use.Kind, kind, StringComparison.Ordinal) &&
            string.Equals(use.Member, member, StringComparison.Ordinal));
    }

    private static string Describe(SyntaxNode type) => type.Parent switch
    {
        CastExpressionSyntax => "a cast",
        PropertyDeclarationSyntax => "a property type",
        UsingDirectiveSyntax { Alias: not null } => "an alias directive",
        UsingDirectiveSyntax => "a using static directive",
        null => "nothing",
        var parent => $"a node of kind {parent.Kind()}",
    };

    private static string? Rightmost(SyntaxNode? node) => node switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        GenericNameSyntax generic => generic.Identifier.ValueText,
        QualifiedNameSyntax qualified => Rightmost(qualified.Right),
        AliasQualifiedNameSyntax alias => Rightmost(alias.Name),
        MemberAccessExpressionSyntax access => Rightmost(access.Name),
        _ => null,
    };

    // =============================================================================================
    // The clauses
    // =============================================================================================

    /// <summary>
    /// W3's first clause: the registry states its revision, and it and the vocabulary are the same
    /// set - every member has exactly one row, every row names a member of its number, and no row
    /// dates from a revision the registry does not have.
    /// </summary>
    internal static IEnumerable<string> W3Vocabulary(Registry registry, IReadOnlyList<(string Name, int Value)> vocabulary)
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
            yield return $"no member of {CodeType} was read, so the registry was compared with nothing";
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
                yield return $"{CodeType} declares {name} = {value} and the registry has no row for it";
            }
        }

        foreach (var row in registry.Rows)
        {
            if (!byNumber[row.Code].Any(member => string.Equals(member.Name, row.Name, StringComparison.Ordinal)))
            {
                yield return byNumber[row.Code].FirstOrDefault() is { Name: { } actual }
                    ? $"the row for {row.Code} names {row.Name}, which is not a member of that number: {CodeType}.{actual} is"
                    : $"the row for {row.Code} names {row.Name}, and {CodeType} has no member of that number";
            }

            if (registry.Revision > 0 && (row.Since < 1 || row.Since > registry.Revision))
            {
                yield return $"the row for {row.Code} dates from revision {row.Since}, and the registry is at revision {registry.Revision}";
            }
        }
    }

    /// <summary>
    /// W3's second clause: every row states its passes, its carrier, its reason, its reachability and
    /// its case in the registry's own vocabulary - the passes in order, the carrier the one its passes
    /// travel on, the reason a member of <c>VmReason</c> that is an invalid-artifact reason on every
    /// carrier but the payload and the core's profile-fault reason on the payload.
    /// </summary>
    internal static IEnumerable<string> W3Columns(Registry registry, IReadOnlyDictionary<string, int> coreReasons)
    {
        foreach (var row in registry.Rows)
        {
            var name = $"the row for {row.Code} {row.Name}";
            var passes = row.Pass.Split('+');

            foreach (var pass in passes.Where(pass => !Passes.Contains(pass, StringComparer.Ordinal)))
            {
                yield return $"{name} names the pass {pass}, which is not one";
            }

            var ordered = Passes.Where(pass => passes.Contains(pass, StringComparer.Ordinal)).ToArray();

            if (!ordered.SequenceEqual(passes, StringComparer.Ordinal) && passes.All(pass => Passes.Contains(pass, StringComparer.Ordinal)))
            {
                yield return $"{name} names its passes as {row.Pass} rather than {string.Join('+', ordered)}";
            }

            if (passes.Contains("bare-module", StringComparer.Ordinal) && row.Reachability != "retired")
            {
                yield return $"{name} names the retired bare-module verifier and is not retired";
            }

            var carrier = string.Join('+', Carriers.Where(candidate => ordered.Any(pass => CarrierOf(pass) == candidate)));

            if (!string.Equals(row.Carrier, carrier, StringComparison.Ordinal))
            {
                yield return $"{name} names the carrier {row.Carrier}, and its passes travel on {carrier}";
            }

            if (!coreReasons.TryGetValue(row.Reason, out var value))
            {
                yield return $"{name} names the reason {row.Reason}, which is not a member of {ReasonType}";
            }
            else if (row.Carrier.Contains("payload", StringComparison.Ordinal))
            {
                if (!string.Equals(row.Reason, PayloadReason, StringComparison.Ordinal))
                {
                    yield return $"{name} travels in the payload and names {row.Reason}, where the core gives every profile fault {PayloadReason}";
                }
            }
            else if (value is <= (int)VmReason.InvalidArtifactUnspecified or >= (int)VmReason.InvalidStateUnspecified)
            {
                yield return $"{name} names {row.Reason}, which is not an invalid-artifact reason";
            }

            if (!Reachabilities.Contains(row.Reachability, StringComparer.Ordinal))
            {
                yield return $"{name} claims the reachability {row.Reachability}, which is not one";
            }
            else if ((row.Reachability == "retired") != (row.Case == "-"))
            {
                yield return row.Reachability == "retired"
                    ? $"{name} is retired and names a case, {row.Case}"
                    : $"{name} claims {row.Reachability} and names no case";
            }

            if (row.Reachability == "execution" && !row.Carrier.Equals("payload", StringComparison.Ordinal))
            {
                yield return $"{name} claims an execution case and does not travel in the payload";
            }

            if (row.Reachability == "corpus" && row.Carrier.Contains("payload", StringComparison.Ordinal))
            {
                yield return $"{name} claims a corpus entry and travels in the payload, which no corpus entry records";
            }
        }
    }

    /// <summary>
    /// W3's third clause: every code maps onto exactly one core reason, the one its row names, which
    /// every emission site and every reason-table arm naming it carries; every pass the row names emits
    /// it and no other pass does; a retired code is named nowhere; and nothing naming the code type is
    /// left unfollowed.
    /// </summary>
    internal static IEnumerable<string> W3Reasons(
        Registry registry, IReadOnlyList<(string Name, int Value)> vocabulary, Emissions emissions)
    {
        var members = vocabulary.Select(static member => member.Name).ToHashSet(StringComparer.Ordinal);
        var rows = registry.Rows
            .GroupBy(static row => row.Name, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        foreach (var message in emissions.Unreadable)
        {
            yield return message;
        }

        if (emissions.Sites.Count == 0)
        {
            yield return $"no emission site was read from {WebAssemblyFamilyRules.ProfileAssembly}, so no reason was compared";
        }

        foreach (var site in emissions.Sites)
        {
            if (!members.Contains(site.Code))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code}, which is not a member of {CodeType}";
            }
            else if (string.Equals(site.Reason, NoReason, StringComparison.Ordinal))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code} with no reason the rule can read";
            }
            else if (rows.TryGetValue(site.Code, out var row) && !string.Equals(site.Reason, row.Reason, StringComparison.Ordinal))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code} with {site.Reason}, and the registry says {row.Reason}";
            }

            if (!PassesOfFile.ContainsKey(site.File))
            {
                yield return $"{site.File}({site.Line}) emits {site.Code} from a file whose pass the rule does not know";
            }
        }

        foreach (var entry in emissions.Tables)
        {
            if (rows.TryGetValue(entry.Code, out var row) && !string.Equals(entry.Reason, row.Reason, StringComparison.Ordinal))
            {
                yield return
                    $"{entry.File}({entry.Line}): the reason table {entry.Table} gives {entry.Code} the reason {entry.Reason}, " +
                    $"and the registry says {row.Reason}";
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

        foreach (var use in AdmittedUses.Where(use => !emissions.Admitted.Contains(use)))
        {
            yield return $"the rule admits {use.Kind} of {CodeType} in {use.File} {use.Member}, and the source has none there";
        }

        foreach (var row in registry.Rows)
        {
            var sites = emissions.Sites.Where(site => string.Equals(site.Code, row.Name, StringComparison.Ordinal)).ToArray();

            if (row.Reachability == "retired")
            {
                foreach (var (file, line, _) in emissions.Named.Where(named => string.Equals(named.Code, row.Name, StringComparison.Ordinal)))
                {
                    yield return $"the row for {row.Code} {row.Name} is retired, and {file}({line}) names it";
                }

                continue;
            }

            if (sites.Length == 0)
            {
                yield return $"{row.Name} is declared and nothing in {WebAssemblyFamilyRules.ProfileAssembly} emits it";
                continue;
            }

            var emitted = Passes
                .Where(pass => sites.Any(site => PassesOfFile.TryGetValue(site.File, out var passes) && passes.Contains(pass, StringComparer.Ordinal)))
                .ToArray();

            var named = row.Pass.Split('+');

            if (!emitted.ToHashSet(StringComparer.Ordinal).SetEquals(named))
            {
                yield return
                    $"the row for {row.Code} {row.Name} names the passes {row.Pass}, and the source emits it in " +
                    string.Join('+', emitted);
            }
        }
    }

    /// <summary>
    /// W3's fourth clause: every row is reachable from a named case or is one the rule lists - a corpus
    /// row names a derived entry recording exactly its code and reason; every entry recording a code
    /// records its row's reason and belongs to a corpus row; an execution row names a trap kind the
    /// execution checks name and the payload mapping maps onto its code; and the unreached rows are
    /// the ones <paramref name="admitted"/> lists, in its words.
    /// </summary>
    internal static IEnumerable<string> W3Reachability(
        Registry registry,
        IReadOnlyList<CorpusEntry> corpus,
        IReadOnlySet<string> checkedKinds,
        IReadOnlyList<PayloadArm> payload,
        IReadOnlyList<UnreachedCode> admitted)
    {
        var entries = corpus
            .GroupBy(static entry => entry.Name, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);
        var byCode = registry.Rows.GroupBy(static row => row.Code).ToDictionary(static group => group.Key, static group => group.First());
        var excused = admitted.ToDictionary(static code => code.Code);
        var discard = payload.FirstOrDefault(static arm => arm.Kind is null)?.Code;

        if (entries.Count == 0)
        {
            yield return "the corpus manifest has no entry, so no row's case was compared";
        }

        if (checkedKinds.Count == 0)
        {
            yield return $"the execution checks name no {TrapKindType}, so no execution row was compared";
        }

        foreach (var row in registry.Rows)
        {
            var name = $"the row for {row.Code} {row.Name}";

            if (row.Reachability != "unreached" && excused.ContainsKey(row.Code))
            {
                yield return $"{name} is listed by the rule as unreached, and claims {row.Reachability}";
            }

            switch (row.Reachability)
            {
                case "corpus":
                {
                    if (!entries.TryGetValue(row.Case, out var entry))
                    {
                        yield return $"{name} names the entry {row.Case}, and the corpus manifest has no entry of that name";
                        break;
                    }

                    if (!string.Equals(entry.Provenance, "derived", StringComparison.Ordinal))
                    {
                        yield return
                            $"{name} names the entry {row.Case}, whose answer is {entry.Provenance} rather than derived, " +
                            "so nobody wrote it down before the profile was asked";
                    }

                    if (!string.Equals(entry.Outcome, "InvalidArtifact", StringComparison.Ordinal) || entry.Code != row.Code)
                    {
                        yield return
                            $"{name} names the entry {row.Case}, which records {entry.Outcome} with code {entry.Code} " +
                            $"rather than InvalidArtifact with code {row.Code}";
                    }

                    break;
                }

                case "execution":
                {
                    const string Prefix = TrapKindType + ".";

                    if (!row.Case.StartsWith(Prefix, StringComparison.Ordinal))
                    {
                        yield return $"{name} names {row.Case}, which is not a member of {TrapKindType}";
                        break;
                    }

                    var kind = row.Case[Prefix.Length..];

                    if (!checkedKinds.Contains(kind))
                    {
                        yield return $"{name} names {row.Case}, and no execution check expects a trap of that kind";
                    }

                    var mapped = payload.FirstOrDefault(arm => string.Equals(arm.Kind, kind, StringComparison.Ordinal))?.Code ?? discard;

                    if (!string.Equals(mapped, row.Name, StringComparison.Ordinal))
                    {
                        yield return $"{name} names {row.Case}, which the payload mapping maps onto {mapped ?? "nothing"}";
                    }

                    break;
                }

                case "unreached":
                    if (!excused.TryGetValue(row.Code, out var listed) || !string.Equals(listed.Name, row.Name, StringComparison.Ordinal))
                    {
                        yield return $"{name} claims no named case reaches it, and is not one of the rows this rule lists";
                    }
                    else if (!string.Equals(listed.Why, row.Case, StringComparison.Ordinal))
                    {
                        yield return $"{name} says why no named case reaches it in words the rule's list does not: the rule says \"{listed.Why}\"";
                    }

                    break;
            }
        }

        foreach (var entry in corpus.Where(static entry => entry.Code != 0))
        {
            if (!byCode.TryGetValue(entry.Code, out var row))
            {
                yield return $"the corpus entry {entry.Name} records code {entry.Code}, which the registry has no row for";
                continue;
            }

            if (row.Reachability != "corpus")
            {
                yield return $"the corpus entry {entry.Name} records {entry.Code} {row.Name}, and the registry says it is {row.Reachability}";
            }

            if (!string.Equals(entry.Outcome, "InvalidArtifact", StringComparison.Ordinal) ||
                !string.Equals(entry.Reason, row.Reason, StringComparison.Ordinal))
            {
                yield return
                    $"the corpus entry {entry.Name} records {entry.Code} {row.Name} as {entry.Outcome} with {entry.Reason}, " +
                    $"and the registry says InvalidArtifact with {row.Reason}";
            }
        }

        foreach (var code in admitted.Where(code => !registry.Rows.Any(row => row.Code == code.Code && row.Reachability == "unreached")))
        {
            yield return $"the rule lists {code.Code} {code.Name} as unreached, and the registry has no unreached row for it";
        }
    }

    /// <summary>
    /// W3's fifth clause: the corpus manifest states the registry revision its codes are read against,
    /// that revision is one the registry has had, and the writer dates the next manifest with the
    /// registry's current one.
    /// </summary>
    internal static IEnumerable<string> W3Revision(int registryRevision, int manifestRevision, int writerRevision)
    {
        if (manifestRevision == -1)
        {
            yield return "the corpus manifest states no registry revision, so no entry in it is dated";
        }
        else if (manifestRevision == 0)
        {
            yield return "the corpus manifest states a registry revision that is not a positive integer";
        }
        else if (registryRevision > 0 && manifestRevision > registryRevision)
        {
            yield return $"the corpus manifest is dated by registry revision {manifestRevision}, and the registry is at revision {registryRevision}";
        }

        if (writerRevision != registryRevision)
        {
            yield return
                $"the corpus writer dates a manifest with registry revision {writerRevision}, and the registry is at revision {registryRevision}";
        }
    }

    /// <summary>The core's reasons, by name, with their numbers.</summary>
    internal static IReadOnlyDictionary<string, int> CoreReasons() =>
        Enum.GetValues<VmReason>().ToDictionary(static reason => reason.ToString(), static reason => (int)reason, StringComparer.Ordinal);

    /// <summary>W3 over the checkout: every input read off disk.</summary>
    internal static IReadOnlyList<string> W3Violations()
    {
        var problems = new List<string>();
        var registry = ReadRegistry(File.ReadAllText(UbcRules.RootPath(RegistryFile)), RegistryFile);
        var vocabulary = Vocabulary(AssuranceSources.File(DiagnosticsFile).Tree, problems);
        var emissions = ReadEmissions(ProfileSourceFiles());
        var manifest = File.ReadAllText(UbcRules.RootPath(CorpusManifestFile));
        var corpus = ReadCorpus(manifest, problems);
        var checkedKinds = NamedTrapKinds(Parse(ExecutionChecksFile));
        var writer = WriterRevision(Parse(CorpusWriterFile));

        return
        [
            .. problems,
            .. W3Vocabulary(registry, vocabulary),
            .. W3Columns(registry, CoreReasons()),
            .. W3Reasons(registry, vocabulary, emissions),
            .. W3Reachability(registry, corpus, checkedKinds, emissions.Payload, Unreached),
            .. W3Revision(registry.Revision, ManifestRevision(manifest), writer),
        ];
    }

    /// <summary>Parses a file of the checkout that is not a covered source of a product assembly.</summary>
    internal static SyntaxTree Parse(string relative) =>
        CSharpSyntaxTree.ParseText(File.ReadAllText(UbcRules.RootPath(relative)), path: relative);
}
