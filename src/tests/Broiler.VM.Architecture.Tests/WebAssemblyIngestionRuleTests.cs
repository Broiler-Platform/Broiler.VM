namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W7: the WebAssembly harness root's ingestion path - its script reader, corpus store and
/// encoders, and the specification's scripts it reads - is never advertised.
/// </summary>
public sealed class WebAssemblyIngestionRuleTests
{
    [Fact]
    public void W7_The_Harness_Its_Sources_And_Its_Suite_Reach_No_Package_And_No_Advertised_Closure()
    {
        Assert.Empty(WebAssemblyIngestionRules.W7(
            ComponentGraph.Projects, CompositionRegisterTests.RegisterRows, CompositionRegisterTests.Closures()));

        // Non-vacuous: exactly one project builds the harness, the closure reports were read, and a
        // renamed harness is reported rather than read as a clean checkout.
        Assert.Single(ComponentGraph.Projects.Where(static project =>
            string.Equals(project.AssemblyName, WebAssemblyIngestionRules.Harness, StringComparison.Ordinal)));

        Assert.NotEmpty(CompositionRegisterTests.Closures());

        Assert.Contains(
            WebAssemblyIngestionRules.W7(
                [.. ComponentGraph.Projects.Where(static project =>
                    !string.Equals(project.AssemblyName, WebAssemblyIngestionRules.Harness, StringComparison.Ordinal))],
                [], []),
            static violation => violation.Contains("quantifying over nothing", StringComparison.Ordinal));
    }

    [Fact]
    public void W7_Rejects_A_Reference_From_The_Execution_Root()
    {
        // The control WA-4's gate names: the script reader reached from the execution root.
        var violations = WebAssemblyIngestionRules.W7(
            [.. ComponentGraph.Projects, ComponentGraph.Witness("W7-execution-root-references-the-harness.csproj.witness")],
            CompositionRegisterTests.RegisterRows,
            CompositionRegisterTests.Closures()).ToArray();

        Assert.Contains(violations, static violation => violation.Contains(
            "Broiler.VM.Composition.WebAssembly.Execution references " + WebAssemblyIngestionRules.Harness, StringComparison.Ordinal));
    }

    [Fact]
    public void W7_Rejects_A_Project_Compiling_A_File_Of_The_Harness_Root()
    {
        // No reference changes, so N13's clauses see nothing, and the link stays inside the component,
        // so A3 sees nothing: only this rule's own clause does.
        var witness = ComponentGraph.Witness("W7-a-project-compiling-the-script-reader.csproj.witness");
        var violations = WebAssemblyIngestionRules.W7(
            [.. ComponentGraph.Projects, witness],
            CompositionRegisterTests.RegisterRows,
            CompositionRegisterTests.Closures()).ToArray();

        var carried = Assert.Single(violations);
        Assert.EndsWith(
            "carries src/compositions/Broiler.VM.Composition.WebAssembly.Harness/ScriptText.cs, a file of " +
            WebAssemblyIngestionRules.Harness + ", into its own build",
            carried, StringComparison.Ordinal);

        Assert.Empty(ArchitectureRules.A3(witness));
    }

    [Fact]
    public void W7_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(static rule => string.Equals(rule.Id, "W7", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains(WebAssemblyIngestionRules.Harness, row.Statement, StringComparison.Ordinal);
        Assert.Contains("tests/wasm/spec", row.Statement, StringComparison.Ordinal);
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);
    }
}
