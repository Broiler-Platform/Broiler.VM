namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule B5c and group X: what may map memory executable, and where the calls that do it live.
/// </summary>
/// <remarks>
/// <para>
/// <b>Group X is minted here rather than folded into an existing group, and the letter is the
/// point.</b> Write-exclusive-or-execute is one property, it is asserted over source text, and it
/// belongs to no profile family - group N is the JavaScript family's and group W the WebAssembly
/// family's, while the arming path is a mechanism the component would govern the same way whoever
/// wrote it. B5c stays in group B because it reads compiled metadata, which is what group B is.
/// </para>
/// <para>
/// Each rule is asserted twice, as every group here is: the checkout is clean, and the rule
/// rejects a violating input.
/// </para>
/// </remarks>
public sealed class NativeMappingRuleTests
{
    /// <summary>The shipping source tree, read once.</summary>
    private static readonly IReadOnlyList<NativeMappingRules.SourceUnit> Tree =
        NativeMappingRules.Tree();

    [Fact]
    public void B5c_No_Assembly_Outside_The_Arming_Path_Declares_A_Mapping_Platform_Invoke()
    {
        // Non-vacuous, and in the way this rule can most easily be empty for the wrong reason: it
        // passes by finding nothing, so an unbuilt tree would be its cleanest-looking result. The
        // arming assembly has to be among the assemblies read, and it has to be carrying the
        // imports the rule exists to allow, before an empty answer about the others means anything.
        var arming = AssemblyFacts.Shipping.Single(assembly => string.Equals(
            assembly.Name, NativeMappingRules.ArmingAssembly, StringComparison.Ordinal));

        Assert.Contains(
            arming.PlatformInvokes,
            invoke => invoke.EndsWith("!VirtualProtect", StringComparison.Ordinal));

        Assert.Contains(
            arming.PlatformInvokes,
            invoke => invoke.EndsWith("!mprotect", StringComparison.Ordinal));

        foreach (var assembly in AssemblyFacts.Shipping)
        {
            Assert.Empty(NativeMappingRules.B5c(assembly));
        }

        // The witness: the test assembly declares a platform invoke to mprotect, and the same
        // scanner that clears every shipping assembly flags it.
        Assert.Contains(
            NativeMappingRules.B5c(AssemblyFacts.TestAssembly),
            message => message.Contains("libc!mprotect", StringComparison.Ordinal));
    }

    /// <summary>
    /// A shipping assembly this run could not read is covered by the source rule instead.
    /// </summary>
    /// <remarks>
    /// The mobile head needs a workload the main solution deliberately does not require, so a plain
    /// <c>dotnet test</c> produces no build output for it and rule B5c cannot read its metadata.
    /// That is a real limit and it is asserted rather than described: every assembly B5c could not
    /// read is one the main solution does not list, and its source is in the tree rule X1 sweeps.
    /// An assembly that went unread for any other reason fails here.
    /// </remarks>
    [Fact]
    public void B5c_Names_Every_Shipping_Assembly_It_Could_Not_Read()
    {
        var main = File.ReadAllText(Path.Combine(ComponentGraph.Root, "Broiler.VM.slnx"));

        foreach (var name in AssemblyFacts.Unread)
        {
            var project = ComponentGraph.Projects.Single(candidate =>
                string.Equals(candidate.AssemblyName, name, StringComparison.Ordinal));

            Assert.DoesNotContain(project.RelativePath, main, StringComparison.Ordinal);

            Assert.Contains(
                Tree,
                file => string.Equals(file.Assembly, name, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void B5c_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "B5c", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        // The row must say which table it reads, because reading the other one is precisely what
        // rule B5 does and precisely why B5 could not have made this claim.
        Assert.Contains("ImplMap", row.Statement, StringComparison.Ordinal);
    }

    [Fact]
    public void X1_The_Arming_Path_Is_The_One_Place_That_Maps_Memory()
    {
        // Non-vacuous three ways before the clean answer is read. All three files of the arming
        // path have to be in the tree by name, a composition root's source has to be in it - the
        // roots are published and run, and a rule that swept only the product projects would clear
        // an arming path in a composition - and the sweep has to be over a real number of files
        // rather than over whatever a broken path expression returned.
        foreach (var path in NativeMappingRules.ArmingPath)
        {
            Assert.Contains(
                Tree,
                file => string.Equals(file.RelativePath, path, StringComparison.Ordinal));
        }

        Assert.Contains(
            Tree,
            file => file.Assembly.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal));

        Assert.True(Tree.Count > 100);

        Assert.Empty(NativeMappingRules.X1(Tree));
    }

    /// <summary>
    /// A second place that maps memory is reported, whichever platform's spelling it copied.
    /// </summary>
    [Fact]
    public void X1_A_Second_Place_That_Maps_Memory_Is_Reported()
    {
        var violations = NativeMappingRules.X1(
            [.. Tree, Witness(
                "X1-a-second-place-that-maps-memory.cs.witness",
                "src/Broiler.VM.Profile.JavaScript.Compiler/JsBackendPage.cs",
                "Broiler.VM.Profile.JavaScript.Compiler")])
            .ToArray();

        Assert.Contains(violations, message =>
            message.Contains("JsBackendPage.cs names VirtualProtect", StringComparison.Ordinal));

        Assert.Contains(violations, message =>
            message.Contains("JsBackendPage.cs names mprotect", StringComparison.Ordinal));
    }

    /// <summary>
    /// A page armed readable, writable and executable is reported: the negative control the
    /// roadmap's exit gate names by name.
    /// </summary>
    /// <remarks>
    /// The witness stands in for the arming path's Windows half at that half's own path, which is
    /// how a rule that quantifies over one named place is shown rejecting something. It carries the
    /// defect in both of the shapes the rule distinguishes: a named constant of the arming path
    /// whose value admits a write and an execute, and a bare literal passed into the protection
    /// argument without being named at all.
    /// </remarks>
    [Fact]
    public void X1_A_Page_Armed_Writable_And_Executable_Is_Reported()
    {
        var armed = Witness(
            "X1-a-page-armed-read-write-and-execute.cs.witness",
            "src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs",
            "Broiler.VM.Profile.JavaScript");

        var violations = NativeMappingRules.X1([armed]).ToArray();

        Assert.Contains(violations, message => message.Contains(
            "declares PageExecuteReadWrite as a protection that admits a write and an execute",
            StringComparison.Ordinal));

        Assert.Contains(violations, message => message.Contains(
            "a protection is passed as a named constant of the arming path or not at all",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// The rule reports its own vacuity rather than passing over an input it never found.
    /// </summary>
    /// <remarks>
    /// Two ways it can be empty for the wrong reason: an input with no arming file in it at all,
    /// which is what a renamed or moved file produces, and an arming path that has stopped naming
    /// a mapping API, which is what a mechanism moved elsewhere produces. Neither is a clean
    /// result and the rule says so.
    /// </remarks>
    [Fact]
    public void X1_Reports_An_Input_In_Which_It_Found_Nothing_To_Quantify_Over()
    {
        Assert.Contains(
            NativeMappingRules.X1([]),
            message => message.Contains(
                "no file of the arming path is in this input", StringComparison.Ordinal));

        Assert.Contains(
            NativeMappingRules.X1([new NativeMappingRules.SourceUnit(
                NativeMappingRules.ArmingPath[0],
                NativeMappingRules.ArmingAssembly,
                "namespace Broiler.VM.Profile.JavaScript; internal sealed class JsNativePage { }")]),
            message => message.Contains(
                "the arming path names no mapping or protection API", StringComparison.Ordinal));
    }

    [Fact]
    public void X1_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "X1", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        // The row must state the scope of its fourth clause. A reader who took "the value appears
        // nowhere" literally would be reading a claim about a tree that contains three instruction
        // encoders, and the row has to say what makes the narrower scope complete.
        Assert.Contains("arming path", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("opcode", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Writes what each group X rule said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_Group_X_Are_Written_When_Asked_For()
    {
        RuleReport.Write("X", [("X1", () => NativeMappingRules.X1(Tree))]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "X.txt")),
                "a report for group X was asked for and none was written");
        }
    }

    /// <summary>A stored witness input, read as though it were the file it stands in for.</summary>
    private static NativeMappingRules.SourceUnit Witness(
        string fileName, string relativePath, string assembly) =>
        new(
            relativePath,
            assembly,
            File.ReadAllText(Path.Combine(
                ComponentGraph.Root,
                "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
                fileName)));
}
