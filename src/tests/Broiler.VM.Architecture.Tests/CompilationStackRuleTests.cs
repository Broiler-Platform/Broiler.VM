using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N19: compilation runs on a stack this component declared, and the bound derived against
/// that stack is not a host's to raise.
/// </summary>
/// <remarks>
/// <para>
/// <b>Both halves or neither, which is why one rule carries both.</b> The tree-depth bound is what
/// makes a deep source a refusal at every size; the declared stack is what lets that bound sit high
/// enough to cost no real program. A checkout with the bound and not the stack refuses programs a
/// minifier emits; one with the stack and not the bound has moved its cliff rather than removed it,
/// and the failure at a cliff is the one failure the CLR cannot turn into an exception. Two rules
/// could each pass while the pair was broken.
/// </para>
/// <para>
/// <b>The host-settable clause is the one a later change would break by kindness.</b> Making the
/// tree bound a field of the parse options reads like an improvement - every other limit here is a
/// host's to state - and it would hand a host a limit on a resource it does not own and cannot
/// measure: the stack belongs to this component, and a host raising the bound past it would be
/// asking for the process termination the bound exists to prevent, in a call that looks like
/// configuration.
/// </para>
/// <para>
/// <b>The entry-point clause was widened on 2026-10-27, at SJL-13's tools commit</b> (JSD-0080
/// decision D16, JSD-0083 decision D27). It read every public static member NAMED <c>Compile</c>,
/// and <c>JsCompiler.Requests</c>, which parses a module to answer its specifiers, parsed on its
/// caller's stack until SJL-12.6 without the clause seeing it. It now reads every public static
/// member of the assembly that reaches a front-end stage, whatever its name: a member whose body, or
/// the body of a member of the same file it calls, names the wide compiler, its parser, the slice
/// parser or the tokenizer. Such a member runs that work through <c>CompilationStack.Run</c>, or
/// reaches it only through a sibling public member that does. The clause read: "Every public compile
/// entry point routes through the declared stack", over the pattern
/// <c>public static\s+(?&lt;returns&gt;\w+)\s+Compile\s*\(</c> and a 600-character window after it.
/// </para>
/// </remarks>
public sealed class CompilationStackRuleTests
{
    private const string Assembly = "Broiler.VM.Profile.JavaScript.Compiler";

    /// <summary>
    /// The pattern the entry-point clause read until 2026-10-27, kept to show what the widening adds:
    /// it matches no member of the witness, which parses off the stack under another name.
    /// </summary>
    private static readonly Regex NamedCompileOnly = new(
        @"public static\s+(?<returns>\w+)\s+Compile\s*\(", RegexOptions.Compiled);

    /// <summary>
    /// The names whose appearance in a member's body says it reaches a front-end stage: the wide
    /// compiler, which parses in its own run, the wide parser, the slice parser and the tokenizer both
    /// share.
    /// </summary>
    private static readonly string[] FrontEndStages = ["JsCompiler", "JsParser", "SliceParser", "SliceTokenizer"];

    /// <summary>
    /// Public static members known to reach a front-end stage on their caller's stack, each with the
    /// reason it is named rather than excused: an entry here is a gap the rule shows, not a member it
    /// admits, and an entry whose member no longer does so is itself reported, so that it cannot outlive
    /// the gap.
    /// </summary>
    /// <remarks>
    /// <b>One entry, found when the clause was widened on 2026-10-27.</b> The construct census parses and
    /// walks every source it is given on its caller's stack; it is no compilation, so the clause as it
    /// read before did not reach it, and its routing through <c>CompilationStack</c> is a change to this
    /// product assembly, which SJL-13's tools commit may not make (bundle <c>sjl-13-001</c>'s rule, "The
    /// commits, named before they exist"). It is handed on to a product change of its own.
    /// </remarks>
    private static readonly (string File, string Member, string Why)[] KnownOffTheStack =
    [
        (
            "src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs",
            "Take",
            "the construct census tokenizes, parses and walks each source on its caller's stack; routing it " +
            "through CompilationStack is a product change SJL-13's tools commit may not make"),
    ];

    private static IEnumerable<AssuranceSourceFile> CompilerFiles =>
        AssuranceSources.Files.Where(static file => file.Assembly == Assembly);

    private static string TextOf(string name) =>
        File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            CompilerFiles.Single(file => file.RelativePath.EndsWith("/" + name, StringComparison.Ordinal))
                .RelativePath.Replace('/', Path.DirectorySeparatorChar)));

    /// <summary>N19's clauses over this checkout.</summary>
    private static IEnumerable<(string Id, Func<IEnumerable<string>> Run)> Clauses =>
    [
        ("N19-entry-points-declare-a-stack", EntryPointViolations),
        ("N19-the-stack-is-declared-here", StackDeclarationViolations),
        ("N19-the-tree-bound-is-not-host-settable", HostSettableViolations),
    ];

    /// <summary>
    /// Every public static member that reaches a front-end stage routes through the declared stack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Read off the source rather than the metadata because what is asserted is that the CALL is
    /// there, and a call is not a thing an assembly's shape records. An entry point that delegates
    /// to another entry point satisfies this by the one it calls.
    /// </para>
    /// <para>
    /// <b>Widened 2026-10-27</b> from the members named <c>Compile</c> to every public static member
    /// that parses, <c>JsCompiler.Requests</c> among them (JSD-0080 decision D16); the class's remarks
    /// quote what it read. Two further findings keep it from passing by finding nothing: no such member
    /// at all, and a <c>Requests</c> declared in <c>JsCompiler.cs</c> that the scan does not reach.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> EntryPointViolations()
    {
        var scanned = 0;
        var reachedRequests = false;
        var requestsDeclared = false;
        var known = new List<(string File, string Member)>();

        foreach (var file in CompilerFiles)
        {
            foreach (var entry in ParsingEntryPoints(file.Tree))
            {
                scanned++;

                if (file.RelativePath.EndsWith("/JsCompiler.cs", StringComparison.Ordinal) && entry.Name == "Requests")
                {
                    reachedRequests = true;
                }

                if (entry.Compliant)
                {
                    continue;
                }

                if (KnownOffTheStack.Any(gap => gap.File == file.RelativePath && gap.Member == entry.Name))
                {
                    known.Add((file.RelativePath, entry.Name));
                    continue;
                }

                yield return
                    $"{file.RelativePath}:{entry.Line} declares a public static member `{entry.Name}` that reaches " +
                    "a front-end stage on its caller's stack: it neither runs through CompilationStack.Run nor " +
                    "reaches the stage only through a sibling public member that does";
            }

            if (file.RelativePath.EndsWith("/JsCompiler.cs", StringComparison.Ordinal) &&
                file.Tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Any(static method => method.Identifier.ValueText == "Requests" && IsPublicStatic(method)))
            {
                requestsDeclared = true;
            }
        }

        foreach (var gap in KnownOffTheStack.Where(gap => !known.Contains((gap.File, gap.Member))))
        {
            yield return
                $"{gap.File} `{gap.Member}` is named among the members known to parse off the stack, and the " +
                "scan no longer finds it doing so: remove the entry, so the rule holds the member as it holds " +
                "every other";
        }

        // The vacuity clauses: this rule passes by finding nothing, so a run that matched no entry
        // point at all would be its cleanest result and would mean the pattern had gone stale.
        if (scanned == 0)
        {
            yield return
                "no public static member that reaches a front-end stage was found, so this rule is " +
                "quantifying over nothing rather than deciding anything";
        }

        if (requestsDeclared && !reachedRequests)
        {
            yield return
                "JsCompiler.cs declares a public static Requests, and the scan does not count it among the " +
                "members that parse, which is the widening this clause exists for (JSD-0080 decision D16)";
        }
    }

    /// <summary>One public static member that reaches a front-end stage, and whether it complies.</summary>
    private readonly record struct ParsingEntry(string Name, int Line, bool Compliant);

    /// <summary>
    /// The public static members of one file that reach a front-end stage, each with whether it runs on
    /// the declared stack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What "reaches" means here.</b> A method reaches a stage when its body names one of
    /// <see cref="FrontEndStages"/>, or calls by name a method declared in the same file that reaches
    /// one. Names and calls are read from the syntax tree, so a stage named in a comment or a string
    /// reaches nothing. The call graph stops at the file: the compiler's entry points construct the
    /// stage they reach, or call the private member of their own file that does.
    /// </para>
    /// <para>
    /// <b>What "complies" means.</b> The member's body invokes <c>CompilationStack.Run</c>; or it names
    /// no stage itself and every method of the same file it reaches a stage through is another public
    /// static member that complies.
    /// </para>
    /// </remarks>
    private static IEnumerable<ParsingEntry> ParsingEntryPoints(SyntaxTree tree)
    {
        var methods = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(static method => method.Body is not null || method.ExpressionBody is not null)
            .ToArray();
        var byName = methods.ToLookup(static method => method.Identifier.ValueText, StringComparer.Ordinal);
        var reaches = new Dictionary<MethodDeclarationSyntax, bool>();

        bool Reaches(MethodDeclarationSyntax method, HashSet<MethodDeclarationSyntax> visiting)
        {
            if (reaches.TryGetValue(method, out var known))
            {
                return known;
            }

            if (!visiting.Add(method))
            {
                return false;
            }

            var answer = NamesAStage(method) ||
                Callees(method).Any(name => byName[name].Any(callee => Reaches(callee, visiting)));

            visiting.Remove(method);
            reaches[method] = answer;
            return answer;
        }

        bool Complies(MethodDeclarationSyntax method, HashSet<MethodDeclarationSyntax> visiting)
        {
            if (RunsOnTheDeclaredStack(method))
            {
                return true;
            }

            if (NamesAStage(method) || !visiting.Add(method))
            {
                return false;
            }

            var answer = Callees(method)
                .SelectMany(name => byName[name])
                .Where(callee => callee != method && Reaches(callee, []))
                .All(callee => IsPublicStatic(callee) && Complies(callee, visiting));

            visiting.Remove(method);
            return answer;
        }

        foreach (var method in methods.Where(IsPublicStatic))
        {
            if (Reaches(method, []))
            {
                yield return new ParsingEntry(
                    method.Identifier.ValueText,
                    method.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    Complies(method, []));
            }
        }
    }

    private static bool IsPublicStatic(MethodDeclarationSyntax method) =>
        method.Modifiers.Any(SyntaxKind.PublicKeyword) && method.Modifiers.Any(SyntaxKind.StaticKeyword);

    /// <summary>Whether the method's own body names a front-end stage.</summary>
    private static bool NamesAStage(MethodDeclarationSyntax method) =>
        BodyOf(method).DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
            .Any(static name => FrontEndStages.Contains(name.Identifier.ValueText, StringComparer.Ordinal));

    /// <summary>Whether the method's own body invokes <c>CompilationStack.Run</c>.</summary>
    private static bool RunsOnTheDeclaredStack(MethodDeclarationSyntax method) =>
        BodyOf(method).DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
            .Any(static call => call.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "CompilationStack" },
                Name.Identifier.ValueText: "Run",
            });

    /// <summary>The names the method's body calls: a bare name, or a member of a receiver.</summary>
    private static IEnumerable<string> Callees(MethodDeclarationSyntax method) =>
        BodyOf(method).DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
            .Select(static call => call.Expression switch
            {
                IdentifierNameSyntax name => name.Identifier.ValueText,
                GenericNameSyntax generic => generic.Identifier.ValueText,
                MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                _ => string.Empty,
            })
            .Where(static name => name.Length != 0)
            .Distinct(StringComparer.Ordinal);

    private static SyntaxNode BodyOf(MethodDeclarationSyntax method) =>
        (SyntaxNode?)method.Body ?? method.ExpressionBody!;

    /// <summary>The declared stack exists, states a size, and runs the work on a thread.</summary>
    private static IEnumerable<string> StackDeclarationViolations()
    {
        var text = TextOf("CompilationStack.cs");

        if (!text.Contains("CompileStackBytes", StringComparison.Ordinal))
        {
            yield return "CompilationStack.cs declares no stack size";
        }

        if (!text.Contains("new System.Threading.Thread(", StringComparison.Ordinal))
        {
            yield return "CompilationStack.cs starts no thread, so it declares no stack at all";
        }

        if (!text.Contains("CompileStackBytes)", StringComparison.Ordinal))
        {
            yield return
                "CompilationStack.cs starts a thread without handing it the declared size, so the " +
                "thread takes the runtime's default and the size is decoration";
        }
    }

    /// <summary>The tree bound is a constant here and not a switch a host states.</summary>
    private static IEnumerable<string> HostSettableViolations()
    {
        var text = TextOf("SliceParseOptions.cs");

        if (!text.Contains("public const int MaximumTreeDepth", StringComparison.Ordinal))
        {
            yield return "SliceParseOptions declares no MaximumTreeDepth constant";
            yield break;
        }

        // A property or constructor parameter of that name would make it the host's.
        foreach (var shape in new[]
        {
            "int MaximumTreeDepth { get",
            "maximumTreeDepth",
            "MaximumTreeDepth =>",
        })
        {
            if (text.Contains(shape, StringComparison.Ordinal))
            {
                yield return
                    $"SliceParseOptions exposes the tree bound as `{shape}`, which makes a limit " +
                    "on this component's own stack something a host states";
            }
        }
    }

    /// <summary>Writes what N19 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_N19_Are_Written_When_Asked_For()
    {
        RuleReport.Write("N19", Clauses);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "N19.txt")),
                "a report for N19 was asked for and none was written");
        }
    }

    [Fact]
    public void N19_Compilation_Runs_On_A_Stack_This_Component_Declared()
    {
        foreach (var (id, run) in Clauses)
        {
            Assert.Empty(run().Select(message => $"{id}: {message}"));
        }
    }

    /// <summary>
    /// The rejecting direction, run over the witness rather than over a story about one.
    /// </summary>
    /// <remarks>
    /// The witness is a parse-options file that hands the tree bound to the host as a settable
    /// property - the shape a later contributor produces, because every other limit in that type
    /// really is the host's and this one reads like an oversight.
    /// </remarks>
    [Fact]
    public void N19_Rejects_A_Tree_Bound_A_Host_Could_Raise()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "N19-a-tree-bound-the-host-can-raise.cs.witness"));

        Assert.Contains("public const int MaximumTreeDepth", witness, StringComparison.Ordinal);
        Assert.Contains("maximumTreeDepth", witness, StringComparison.Ordinal);

        // The real file passes the same predicate the witness fails, which is what makes the
        // comparison a rule rather than a pair of strings.
        Assert.DoesNotContain("maximumTreeDepth", TextOf("SliceParseOptions.cs"), StringComparison.Ordinal);
    }

    /// <summary>
    /// The rejecting direction of the widened entry-point clause, run over its witness (2026-10-27).
    /// </summary>
    /// <remarks>
    /// The witness is <c>JsCompiler.Requests</c> as it stood before SJL-12.6 item 4: a public static
    /// member that is not named <c>Compile</c>, that parses a module to answer its specifiers, and that
    /// does so on its caller's stack. The pattern the clause read before the widening matches nothing in
    /// it, and the widened clause refuses it; the real <c>Requests</c> passes the predicate the witness
    /// fails.
    /// </remarks>
    [Fact]
    public void N19_Rejects_A_Parsing_Entry_Point_Off_The_Declared_Stack()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "N19-a-parsing-entry-point-off-the-declared-stack.cs.witness"));

        Assert.Empty(NamedCompileOnly.Matches(witness));

        var entries = ParsingEntryPoints(CSharpSyntaxTree.ParseText(witness)).ToArray();
        var refused = Assert.Single(entries);
        Assert.Equal("Requests", refused.Name);
        Assert.False(refused.Compliant);

        var real = CompilerFiles.Single(static file => file.RelativePath.EndsWith("/JsCompiler.cs", StringComparison.Ordinal));
        Assert.Contains(
            ParsingEntryPoints(real.Tree),
            static entry => entry.Name == "Requests" && entry.Compliant);
    }

    /// <summary>The bound and the stack cite one another, so neither can move alone.</summary>
    /// <remarks>
    /// A derivation that names no input is a number; this asserts the constant's remark names the
    /// stack it was derived against, which is what a reader re-deriving it needs and what a later
    /// change to the stack size has to come back to.
    /// </remarks>
    [Fact]
    public void N19_The_Bound_Cites_The_Stack_It_Was_Derived_Against()
    {
        var options = TextOf("SliceParseOptions.cs");
        var stack = TextOf("CompilationStack.cs");

        Assert.Contains("declares for compilation", options, StringComparison.Ordinal);
        Assert.Contains("MaximumTreeDepth", stack, StringComparison.Ordinal);
    }
}
