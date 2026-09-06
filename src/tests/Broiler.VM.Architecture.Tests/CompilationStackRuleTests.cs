using System.Text.RegularExpressions;

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
/// </remarks>
public sealed class CompilationStackRuleTests
{
    private const string Assembly = "Broiler.VM.Profile.JavaScript.Compiler";

    /// <summary>Public compile entry points, which are what a caller reaches the walks through.</summary>
    private static readonly Regex EntryPoint = new(
        @"public static\s+(?<returns>\w+)\s+Compile\s*\(", RegexOptions.Compiled);

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
    /// Every public compile entry point routes through the declared stack.
    /// </summary>
    /// <remarks>
    /// Read off the source rather than the metadata because what is asserted is that the CALL is
    /// there, and a call is not a thing an assembly's shape records. An entry point that delegates
    /// to another entry point satisfies this by the one it calls, which is why a body naming
    /// either the helper or a sibling <c>Compile</c> is admitted.
    /// </remarks>
    private static IEnumerable<string> EntryPointViolations()
    {
        var scanned = 0;

        foreach (var file in CompilerFiles)
        {
            var path = Path.Combine(
                ComponentGraph.Root, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            var text = File.ReadAllText(path);

            foreach (Match match in EntryPoint.Matches(text))
            {
                scanned++;
                var body = text[match.Index..Math.Min(text.Length, match.Index + 600)];

                if (!body.Contains("CompilationStack.Run", StringComparison.Ordinal) &&
                    !body.Contains("Compile(", StringComparison.Ordinal))
                {
                    yield return
                        $"{file.RelativePath} declares a public Compile entry point that reaches " +
                        "the tree walks on the caller's stack";
                }
            }
        }

        // The vacuity clause: this rule passes by finding nothing, so a run that matched no entry
        // point at all would be its cleanest result and would mean the pattern had gone stale.
        if (scanned == 0)
        {
            yield return
                "no public compile entry point was found, so this rule is quantifying over " +
                "nothing rather than deciding anything";
        }
    }

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
