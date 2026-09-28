namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W4: the WebAssembly profile calls none of the core's canonical variable-length readers, and
/// compares a declared count with its ceiling only where the rule lists.
/// </summary>
public sealed class WebAssemblyReaderRuleTests
{
    [Fact]
    public void W4_The_Profile_Reads_Its_Own_Integers_And_Bounds_A_Count_Once()
    {
        var files = WebAssemblyRegistryRules.ProfileSourceFiles();

        Assert.Empty(WebAssemblyReaderRules.W4(files));

        // Non-vacuous: the profile's sources were read, and the clean result includes both listed
        // comparisons being found where the rule says they are - a listed member that stopped comparing
        // would be reported, as the last assertion shows.
        Assert.True(files.Count > 10);
        Assert.Contains(files, static file => file.RelativePath == "src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs");

        // The witness, read as a file of the assembly beside the real ones: exactly its two defects.
        var witness = AssuranceSources.ReadFile(
            Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
                "W4-a-profile-source-reading-canonical-integers.cs.witness"),
            WebAssemblyFamilyRules.ProfileAssembly);

        var reported = WebAssemblyReaderRules.W4([.. files, witness]).ToArray();

        Assert.Equal(2, reported.Length);
        Assert.Contains(reported, static message => message.Contains(
            "names the core's canonical reader TryReadVarUInt32", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.Contains(
            "compares with MaxDeclaredCount in TryReadTableCount, a second implementation of the count-bound comparison",
            StringComparison.Ordinal));

        // A listed member that no longer compares is reported rather than read as a list that happens to
        // be longer than the source: the sources without the decoder lose TryReadLocals.
        Assert.Equal(
            ["the rule lists TryReadLocals in src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs as comparing a count with MaxDeclaredCount, and it does not"],
            WebAssemblyReaderRules.W4(files.Where(static file => !file.RelativePath.EndsWith("/WasmDecoder.cs", StringComparison.Ordinal)).ToArray()));

        // And an empty scan is a failure, not a pass.
        Assert.NotEmpty(WebAssemblyReaderRules.W4([]));
    }

    [Fact]
    public void W4_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(static rule => string.Equals(rule.Id, "W4", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("TryReadVarUInt32", row.Statement, StringComparison.Ordinal);
        Assert.Contains("TryReadBoundedCount", row.Statement, StringComparison.Ordinal);
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);
    }
}
