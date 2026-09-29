namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W5: nothing reachable from the WebAssembly profile's verified state is mutable.
/// </summary>
public sealed class WebAssemblyStateRuleTests
{
    private const string FamilyDataFile = "src/Broiler.VM.Profile.WebAssembly/WasmFamilyData.cs";

    private static readonly string[] Roots = [.. WebAssemblyStateRules.Roots.Select(static root => root.Type)];

    [Fact]
    public void W5_Nothing_Reachable_From_The_Verified_State_Is_Mutable()
    {
        var files = WebAssemblyRegistryRules.ProfileSourceFiles();

        Assert.Empty(WebAssemblyStateRules.W5(files, Roots, out var walked));

        // Non-vacuous: the walk reached every type the definitions keep, through the immutable arrays
        // and the position index, and not only the root.
        Assert.Equal(
            ["WasmDataDefinition", "WasmDefinitions", "WasmElementDefinition", "WasmExportDefinition", "WasmGlobalDefinition", "WasmLimits", "WasmPositionIndex"],
            walked.Order(StringComparer.Ordinal));

        // The witness, read as a file of the assembly beside the real ones and walked from its own root:
        // exactly its six defects, the last reached through a part the scan walked to, and nothing for
        // the static field, the computed property or the enumeration beside them.
        var witness = AssuranceSources.ReadFile(
            Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
                "W5-a-verified-state-that-keeps-something-mutable.cs.witness"),
            WebAssemblyFamilyRules.ProfileAssembly);

        var reported = WebAssemblyStateRules.W5([.. files, witness], ["WasmStateWitness"], out var witnessWalked).ToArray();

        Assert.Equal(6, reported.Length);
        Assert.Contains(reported, static message => message.EndsWith("WasmStateWitness.reads is a field that is not readonly", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith("WasmStateWitness.keys keeps an array, which any holder of it can write", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith("WasmStateWitness.Start is a property with a setter", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith(
            "WasmStateWitness.Functions keeps a List, a generic type the rule does not admit as immutable", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith(
            "WasmStateWitness.Name keeps a StringBuilder, a type the scan cannot see and the rule does not admit", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith(
            "WasmStatePartWitness.Contents keeps a Memory, a generic type the rule does not admit as immutable", StringComparison.Ordinal));
        Assert.Equal(["WasmStatePartWitness", "WasmStateWitness"], witnessWalked.Order(StringComparer.Ordinal));

        // The real source with the defect the rule was minted beside put back: the position index keeping
        // the array its keys were sorted into.
        var familyData = files.Single(static file => file.RelativePath == FamilyDataFile);
        var altered = AssuranceSources.WithText(familyData, familyData.Text.Replace(
            "private readonly ImmutableArray<ulong> keys;", "private readonly ulong[] keys;", StringComparison.Ordinal));

        Assert.NotEqual(familyData.Text, altered.Text);

        var restored = Assert.Single(WebAssemblyStateRules.W5([.. files.Where(file => file != familyData), altered], Roots, out _));

        Assert.StartsWith($"{FamilyDataFile}(", restored, StringComparison.Ordinal);
        Assert.EndsWith(") WasmPositionIndex.keys keeps an array, which any holder of it can write", restored, StringComparison.Ordinal);

        // A root the sources do not declare is reported rather than walked as nothing, so an empty scan
        // is a failure, not a pass.
        Assert.Equal(
            [$"the rule starts at WasmDefinitions, and no source of {WebAssemblyFamilyRules.ProfileAssembly} declares it"],
            WebAssemblyStateRules.W5([], Roots, out _));
    }

    [Fact]
    public void W5_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(static rule => string.Equals(rule.Id, "W5", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("WasmDefinitions", row.Statement, StringComparison.Ordinal);
        Assert.Contains("ImmutableArray", row.Statement, StringComparison.Ordinal);
        Assert.Contains("ReadOnlyMemory", row.Statement, StringComparison.Ordinal);
        Assert.Contains("UbcPosition", row.Statement, StringComparison.Ordinal);
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);
    }
}
