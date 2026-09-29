namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W6: the WebAssembly profile names no execution step, and no core outcome category outside the
/// translator's four verification answers.
/// </summary>
public sealed class WebAssemblyVocabularyRuleTests
{
    [Fact]
    public void W6_No_Profile_Code_Names_A_Core_Outcome_Category_Where_It_Executes()
    {
        var files = WebAssemblyRegistryRules.ProfileSourceFiles();

        Assert.Empty(WebAssemblyVocabularyRules.W6(files, out var admitted));

        // Non-vacuous: the sources were read, and the translator's four answers were found where the
        // rule admits them, so a reader that followed no name would fail here rather than pass.
        Assert.True(files.Count > 10);
        Assert.Equal(["Cancellation", "InvalidArtifact", "Normal", "ResourceExhaustion"], admitted);

        // The witness, read as a file of the assembly beside the real ones: its import of the outcome
        // type's members, each spelling of a category and of the step - a return type included - and
        // nothing for the result it passes on.
        var witness = AssuranceSources.ReadFile(
            Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
                "W6-a-family-handler-naming-the-cores-outcome-and-step.cs.witness"),
            WebAssemblyFamilyRules.ProfileAssembly);

        var reported = WebAssemblyVocabularyRules.W6([.. files, witness], out _);

        Assert.Equal(6, reported.Count);
        Assert.Contains(reported, static message => message.Contains("imports the members of Broiler.VM.VmOutcome", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith("names VmOutcome, a core outcome category, outside the translator", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith("names VmOutcome.ProfileFault, a core outcome category, outside the translator", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.EndsWith("names VmOutcome.Normal, a core outcome category, outside the translator", StringComparison.Ordinal));
        Assert.Equal(2, reported.Count(static message => message.EndsWith(
            "names VmExecutionStep, and the emitter, not the profile, answers the execution step", StringComparison.Ordinal)));

        // The translator answering a category no verification answers is the translator answering for
        // execution.
        var translator = files.Single(static file => file.RelativePath == WebAssemblyVocabularyRules.TranslatorFile);
        var answering = AssuranceSources.WithText(translator, translator.Text.Replace(
            "public static class WasmTranslator\n{", "public static class WasmTranslator\n{\n    private const VmOutcome Answering = VmOutcome.ProfileFault;",
            StringComparison.Ordinal));

        Assert.NotEqual(translator.Text, answering.Text);
        var answered = Assert.Single(WebAssemblyVocabularyRules.W6([.. files.Where(file => file != translator), answering], out _));
        Assert.EndsWith(
            "names VmOutcome.ProfileFault, a category no verification of this profile answers, so the translator would be answering for execution",
            answered, StringComparison.Ordinal);

        // The listed file gone is reported rather than read as a file that names nothing, and an empty
        // scan is a failure.
        Assert.Contains(WebAssemblyVocabularyRules.W6([.. files.Where(file => file != translator)], out _),
            static message => message.StartsWith("the rule lists", StringComparison.Ordinal));
        Assert.NotEmpty(WebAssemblyVocabularyRules.W6([], out _));
    }

    [Fact]
    public void W6_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(static rule => string.Equals(rule.Id, "W6", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("VmExecutionStep", row.Statement, StringComparison.Ordinal);
        Assert.Contains("WasmTranslator", row.Statement, StringComparison.Ordinal);
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);
    }
}
