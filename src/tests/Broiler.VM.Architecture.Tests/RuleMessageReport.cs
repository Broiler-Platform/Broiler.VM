using System.Reflection;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Writes what a group's rules actually said about this checkout, when asked to.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS REPORTS AND DOES NOT JUDGE.</b> The tests assert that the rules are silent; this writes
/// down what they say. Nothing here can make a defect pass, because nothing here is what a defect
/// has to get past - a negative control's verdict is still the suite's exit code.
/// </para>
/// <para>
/// <b>Why it exists.</b> A control judged by an exit code cannot distinguish one clause of a
/// many-clause rule from another: the suite reports that the rule went red, and xunit prints an
/// empty-collection assertion without printing the collection. Four retained bundles carried that
/// as a stated limit before the group K report retired it, and this is the same mechanism for
/// every other group whose rules are expressible as a function.
/// </para>
/// <para>
/// <b>It runs only when asked</b>, through <c>BROILER_RULE_MESSAGES</c> naming a DIRECTORY, so an
/// ordinary suite run neither writes anything nor pays for it. One file per group, because xunit
/// runs test classes in parallel and one file would interleave.
/// </para>
/// </remarks>
internal static class RuleReport
{
    /// <summary>The directory a report was asked for, or null when none was.</summary>
    internal static string? Destination =>
        Environment.GetEnvironmentVariable("BROILER_RULE_MESSAGES") is { Length: > 0 } directory
            ? directory
            : null;

    /// <summary>
    /// Runs each rule and writes what it said, or that it threw.
    /// </summary>
    /// <remarks>
    /// A rule that THROWS is recorded as throwing rather than crashing the report: two group K
    /// rules threw on a register row naming a composition the checkout does not have, and that is
    /// a fact about the input worth writing down rather than an error in the reporter.
    /// </remarks>
    internal static void Write(
        string group, IEnumerable<(string Id, Func<IEnumerable<string>> Run)> rules)
    {
        var destination = Destination;

        if (destination is null)
        {
            return;
        }

        Directory.CreateDirectory(destination);

        var lines = new List<string>
        {
            $"# what group {group}'s rules said about this checkout",
            "#",
            "# Reported, not judged. A rule with no lines said nothing, which is what a clean",
            "# checkout looks like; a rule that threw is recorded as throwing.",
            string.Empty,
        };

        foreach (var (id, run) in rules)
        {
            try
            {
                var messages = run().ToArray();

                lines.Add($"[{id}] {messages.Length} message(s)");
                lines.AddRange(messages.Select(static message =>
                    "    " + message.Replace("\n", " ", StringComparison.Ordinal)));
            }
            catch (Exception failure)
            {
                lines.Add($"[{id}] THREW {failure.GetType().Name}: {failure.Message}");
            }

            lines.Add(string.Empty);
        }

        File.WriteAllLines(Path.Combine(destination, group + ".txt"), lines);
    }

    /// <summary>Every project in the graph, which is what a group A rule is swept over.</summary>
    internal static IEnumerable<string> Sweep(
        Func<ComponentGraph.ProjectFile, IEnumerable<string>> rule) =>
        ComponentGraph.Projects.SelectMany(rule);

    /// <summary>Every product assembly, which is what most group B rules are swept over.</summary>
    internal static IEnumerable<string> Sweep(Func<AssemblyFacts, IEnumerable<string>> rule) =>
        AssemblyFacts.Product.SelectMany(rule);
}

/// <summary>
/// The reports for the groups whose rules are functions over inputs this class can build.
/// </summary>
/// <remarks>
/// <para>
/// <b>This class holds groups A, B and V. The other reports live beside the tests whose inputs
/// they mirror</b>, in <c>CompositionRegisterTests</c>, <c>DiagnosticRegistryRuleTests</c>,
/// <c>ReviewRecordRuleTests</c>, <c>AssuranceRuleTests</c>, <c>BaselineRegisterTests</c>,
/// <c>CoreContractVersionTests</c>, <c>PackageRuleTests</c>, <c>ProjectFileRuleTests</c>,
/// <c>LegacyBoundaryTests</c>, <c>ApiSurfaceTests</c> and <c>ProfileApiSurfaceTests</c>.
/// </para>
/// <para>
/// <b>Seventy-three of the register's seventy-six rules are reported, and the three that are not
/// are named rather than left as an absence a reader has to notice.</b>
/// </para>
/// <list type="bullet">
/// <item><b>E5</b> is <b>Deferred</b> and superseded at VM-1 by V1 and V2. No test asserts it,
/// because <c>RuleRegisterTests</c> requires that none does. A report on it would be writing the
/// rule the register says is not asserted.</item>
/// <item><b>J10</b> and <b>J11</b> assert their clean direction over WITNESS inputs rather than
/// over the checkout, so there is no "what this rule said here" to write down.</item>
/// </list>
/// <para>
/// <b>The count moved twice, and both moves were corrections rather than additions.</b> It was 46
/// while thirty rules were called unreportable for asserting inline; fifteen of those already had
/// a named helper returning a collection and needed no extraction at all, and seven were extracted
/// as a MOVE - the test calling the extracted function, because two implementations of one rule is
/// the drift a report exists to prevent. It was 68 while six were called unreportable for
/// asserting equalities or an absence; two of those six had returned message lists all along and
/// were mis-sorted by a survey that read their shape without opening their bodies, three were
/// genuinely restated as message lists on 2026-09-02 under Bundle JS-ANDROID-012, and the sixth is
/// E5, which turned out not to be a rule this suite asserts at all.
/// </para>
/// <para>
/// <b>Each report mirrors its tests' inputs exactly.</b> Where a test sweeps every project, so
/// does the report; where it names three assemblies, so does the report. A report over different
/// inputs would answer a question nobody asked, and would be worse than none because it would look
/// like an answer to this one.
/// </para>
/// </remarks>
public sealed class RuleMessageReportTests
{
    [Fact]
    public void RuleMessages_For_Group_A_Are_Written_When_Asked_For()
    {
        RuleReport.Write("A",
        [
            ("A1", () => RuleReport.Sweep(ArchitectureRules.A1)),
            ("A2", () => RuleReport.Sweep(ArchitectureRules.A2)),
            ("A3", () => RuleReport.Sweep(ArchitectureRules.A3)),
            ("A4", () => RuleReport.Sweep(ArchitectureRules.A4)),
            ("A5", () => RuleReport.Sweep(ArchitectureRules.A5)),
            ("A6", () => RuleReport.Sweep(ArchitectureRules.A6)),
            ("A8", () => RuleReport.Sweep(ArchitectureRules.A8)),
            ("A9", () => RuleReport.Sweep(ArchitectureRules.A9)),
            ("A10", () => RuleReport.Sweep(ArchitectureRules.A10)),
            ("A11", () => RuleReport.Sweep(ArchitectureRules.A11)),
            ("A12", () => RuleReport.Sweep(ArchitectureRules.A12)),
            ("A13", () => RuleReport.Sweep(ArchitectureRules.A13)),
            ("N1", () => RuleReport.Sweep(ArchitectureRules.N1)),
            ("N2", () => RuleReport.Sweep(ArchitectureRules.N2)),
            ("N3", () => RuleReport.Sweep(ArchitectureRules.N3)),
            ("N4", () => RuleReport.Sweep(ArchitectureRules.N4)),
        ]);

        Wrote("A");
    }

    [Fact]
    public void RuleMessages_For_Group_B_Are_Written_When_Asked_For()
    {
        RuleReport.Write("B",
        [
            ("B1", () => new[] { AssemblyFacts.Abstractions, AssemblyFacts.Binary }
                .SelectMany(ArchitectureRules.B1)),
            ("B2", () => ArchitectureRules.B2(AssemblyFacts.Runtime)),
            ("B3", () => AssemblyFacts.Product.Append(AssemblyFacts.Fixtures)
                .SelectMany(ArchitectureRules.B3)),
            ("B4", () => new[] { typeof(VmCoreContract).Assembly, typeof(VmBoundedReader).Assembly,
                typeof(VmRuntime).Assembly }.SelectMany(ArchitectureRules.B4)),
            ("B5", () => RuleReport.Sweep(ArchitectureRules.B5)),
            ("B5b", () => RuleReport.Sweep(ArchitectureRules.B5b)),
            ("B6", () => RuleReport.Sweep(ArchitectureRules.B6)),
            ("B7", () => RuleReport.Sweep(ArchitectureRules.B7)),
        ]);

        Wrote("B");
    }

    [Fact]
    public void RuleMessages_For_Group_V_Are_Written_When_Asked_For()
    {
        RuleReport.Write("V",
        [
            ("V1", () => ApiBaselineRules.V1(AssemblyFacts.Product)),
            ("V2", () => ApiBaselineRules.V2(typeof(VmCoreContract))),
            ("V3", () => ApiBaselineRules.V3(ApiBaselineRules.ProductTypes)),
            ("V4", () => ApiBaselineRules.V4(typeof(VmProfileDescriptor))),
            ("V5", () => ApiBaselineRules.V5(VmReasonRegistry.All())),
            ("V6", () => ApiBaselineRules.V6(typeof(IVmMeter))),
            ("V7", () => ApiBaselineRules.V7(ApiBaselineRules.ProductTypes)),
            ("V8", () => ApiBaselineRules.V8(ApiBaselineRules.ProductTypes)
                .Concat(ApiBaselineRules.V8Timers(AssemblyFacts.Product))),
            ("V9", () => ApiBaselineRules.V9(ApiBaselineRules.ProductTypes, AssemblyFacts.Product)),
            ("V10", () => ApiBaselineRules.V10(ApiBaselineRules.ProductTypes)),
            ("V11", () => ApiBaselineRules.V11(typeof(VmDiagnostics))),
            ("V12", () => ApiBaselineRules.V12(ApiBaselineRules.ProfileFacingContracts)),
        ]);

        Wrote("V");
    }

    /// <summary>
    /// A report that was asked for and wrote nothing is worse than absent.
    /// </summary>
    /// <remarks>
    /// A control reading an empty directory would report no messages and read that as a silent
    /// rule rather than as a broken harness, which is exactly the misreading this whole mechanism
    /// exists to prevent.
    /// </remarks>
    private static void Wrote(string group)
    {
        if (RuleReport.Destination is not { } destination)
        {
            return;
        }

        Assert.True(
            File.Exists(Path.Combine(destination, group + ".txt")),
            $"a report for group {group} was asked for and none was written");
    }
}
