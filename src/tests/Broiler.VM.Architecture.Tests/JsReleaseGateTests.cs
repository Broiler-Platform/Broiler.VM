using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N38: the JavaScript profile's release gate refuses, and the blockers it names are exactly
/// the ones its register records, in both directions; a review headline or a support table that
/// claims more than the units record cannot make it pass (phase F9 slice R4, decision JSD-0062).
/// </summary>
/// <remarks>
/// <para>
/// <b>The rule is about the gate's honesty, not its verdict.</b> The day every relevant unit
/// carries a human decision and the owner has taken the acts the register names, the gate's
/// blockers shrink, the register shrinks with them, and the verdict this rule expects changes in the
/// same change. What it forbids is the gate and its register disagreeing - a blocker cleared on
/// paper that the checkout still shows, or one the checkout no longer shows that the paper keeps.
/// </para>
/// <para>
/// <b>The negative controls are the point.</b> A headline that claims every unit reviewed, and a
/// support table that claims to be issued and reviewed, are each handed to the gate in place of the
/// checkout's; the gate still refuses, and names the contradiction. Neither can invent a reviewer.
/// </para>
/// </remarks>
public sealed class JsReleaseGateTests
{
    private const string RegisterName = "src/Broiler.VM.Profile.JavaScript/docs/release-gate.md";

    [Fact]
    public void N38_The_Release_Gate_Refuses_Naming_Exactly_The_Blockers_Its_Register_Records()
    {
        var gates = JsReleaseGate.Evaluate(JsReleaseGate.Inputs.FromCheckout());
        var named = gates.SelectMany(static gate => gate.Blockers).Select(static blocker => blocker.Id).ToList();
        var recorded = RegisterIds();

        Assert.Equal(13, gates.Count);
        Assert.Equal(JsReleaseGate.Refused, JsReleaseGate.Verdict(gates));
        Assert.Equal(named.Count, named.Distinct(StringComparer.Ordinal).Count());
        Assert.Empty(named.Except(recorded, StringComparer.Ordinal).Select(id => $"the gate names {id} and {RegisterName} does not record it"));
        Assert.Empty(recorded.Except(named, StringComparer.Ordinal).Select(id => $"{RegisterName} records {id} and the gate no longer names it"));
        Assert.Contains($"**Verdict: {JsReleaseGate.Refused}.**", Register(), StringComparison.Ordinal);

        foreach (var blocker in gates.SelectMany(static gate => gate.Blockers))
        {
            Assert.False(string.IsNullOrWhiteSpace(blocker.Declaration), $"{blocker.Id} names no declaration");
        }
    }

    [Fact]
    public void N38_A_Headline_Claiming_Every_Unit_Reviewed_Is_Not_A_Reviewer()
    {
        var inputs = JsReleaseGate.Inputs.FromCheckout() with { ReviewHeadline = Witness("N38-review-headline-claims-every-unit.md.witness") };
        var gates = JsReleaseGate.Evaluate(inputs);
        var ids = gates.SelectMany(static gate => gate.Blockers).Select(static blocker => blocker.Id).ToList();

        Assert.Equal(JsReleaseGate.Refused, JsReleaseGate.Verdict(gates));
        Assert.Contains("G11-units-undecided", ids);
        Assert.Contains("G11-headline-contradicts-units", ids);
    }

    [Fact]
    public void N38_A_Support_Table_Claiming_To_Be_Issued_And_Reviewed_Is_Not_A_Reviewer()
    {
        var inputs = JsReleaseGate.Inputs.FromCheckout() with { SupportTable = Witness("N38-support-table-reads-as-issued.md.witness") };
        var gates = JsReleaseGate.Evaluate(inputs);
        var ids = gates.SelectMany(static gate => gate.Blockers).Select(static blocker => blocker.Id).ToList();

        Assert.Equal(JsReleaseGate.Refused, JsReleaseGate.Verdict(gates));
        Assert.Contains("G1-issued-without-review", ids);
        Assert.Contains("G11-units-undecided", ids);
        Assert.DoesNotContain("G1-not-issued", ids);
    }

    [Fact]
    public void N38_The_Gate_Report_Is_Written_When_Asked_For()
    {
        var report = JsReleaseGate.Report(JsReleaseGate.Evaluate(JsReleaseGate.Inputs.FromCheckout()));

        Assert.StartsWith("# The JavaScript profile's release gate\nverdict " + JsReleaseGate.Refused + "\n", report, StringComparison.Ordinal);

        if (Environment.GetEnvironmentVariable("BROILER_RELEASE_GATE_REPORT") is { Length: > 0 } path)
        {
            File.WriteAllText(path, report);
        }
    }

    [Fact]
    public void N38_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, "N38", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(RegisterName, row.Evidence, StringComparison.Ordinal);
    }

    private static string Register() => File.ReadAllText(Path.Combine(ComponentGraph.Root, RegisterName));

    /// <summary>The register's blocker rows: a first cell that is one backticked identifier beginning with G and a gate number.</summary>
    private static List<string> RegisterIds() =>
        Register().Split('\n')
            .Select(static line => Regex.Match(line, @"^\|\s*`(?<id>G[0-9]{1,2}-[A-Za-z0-9.+-]+)`\s*\|"))
            .Where(static match => match.Success)
            .Select(static match => match.Groups["id"].Value)
            .ToList();

    private static string Witness(string fileName)
    {
        var path = Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "js-release-gate", fileName);
        Assert.True(File.Exists(path), $"Missing witness input {path}.");
        return File.ReadAllText(path);
    }
}
