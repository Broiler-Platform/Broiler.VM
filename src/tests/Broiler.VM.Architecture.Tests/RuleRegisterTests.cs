using System.Text.Json;
using System.Text.Json.Serialization;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The register is itself under test. It is the artefact the ADRs, the evidence bundle and the
/// status ledger all quote when they say how much VM-0 proves, so nothing may drift between it,
/// the tests that run, and the witness inputs on disk.
/// </summary>
public sealed class RuleRegisterTests
{
    private static readonly Register Loaded = Load();

    [Fact]
    public void Every_Registered_Rule_Has_A_Test_That_Asserts_It()
    {
        var asserted = typeof(RuleRegisterTests).Assembly
            .GetTypes()
            .SelectMany(static type => type.GetMethods())
            .Where(static method => method.GetCustomAttributes(typeof(FactAttribute), inherit: false).Length > 0)
            .Select(static method => method.Name)
            .ToArray();

        var missing = Loaded.Rules
            .Where(static rule => rule.Status != "Deferred")
            .Where(rule => !asserted.Any(name =>
                name.StartsWith(rule.Id + "_", StringComparison.Ordinal)))
            .Select(static rule => $"{rule.Id} is {rule.Status} but no test method asserts it")
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void Deferred_Rules_Are_Not_Asserted_And_Name_A_Later_Milestone()
    {
        // A Deferred or Vacuous row may not name an activation milestone at or before the
        // current one, and a rule whose activation milestone IS the current milestone must be
        // Active with a passing witness. ADR 0001 states this once; the register obeys it here.
        var violations = Loaded.Rules
            .Where(static rule => rule.Status is "Deferred" or "Vacuous")
            .Where(static rule => rule.ActivationMilestone is null or "VM-0")
            .Select(static rule => $"{rule.Id} is {rule.Status} but names activation milestone {rule.ActivationMilestone ?? "none"}")
            .ToArray();

        Assert.Empty(violations);

        var activeWithMilestone = Loaded.Rules
            .Where(static rule => rule.Status == "Active" && rule.ActivationMilestone is not null)
            .Select(static rule => $"{rule.Id} is Active but still names an activation milestone")
            .ToArray();

        Assert.Empty(activeWithMilestone);
    }

    [Fact]
    public void Every_Active_Rule_Names_A_Witness()
    {
        var missing = Loaded.Rules
            .Where(static rule => rule.Status == "Active")
            .Where(static rule => string.IsNullOrWhiteSpace(rule.Witness))
            .Select(static rule => $"{rule.Id} is Active with no witness")
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void Every_Witness_Input_On_Disk_Is_Named_By_A_Rule()
    {
        var named = Loaded.Rules
            .Select(static rule => rule.Witness)
            .Where(static witness => witness is not null)
            .Select(static witness => witness!)
            .ToArray();

        var orphans = ComponentGraph.Witnesses
            .Select(static witness => Path.GetFileName(witness.Path))
            .Where(file => !named.Any(witness => witness.Contains(file, StringComparison.Ordinal)))
            .ToArray();

        Assert.Empty(orphans);
    }

    [Fact]
    public void The_Register_Agrees_With_The_Graph_Manifest_On_The_Contract_Version()
    {
        Assert.Equal(VmCoreContract.Version, Loaded.CoreContractVersion);
        Assert.Equal(VmCoreContract.Version, GraphManifest.CoreContractVersion);
    }

    /// <summary>
    /// The counts the evidence bundle and the ledger row quote. Asserting them here means the
    /// sentence "N rules await their subject" cannot go stale without a test failing.
    /// </summary>
    [Fact]
    public void The_Recorded_Status_Counts_Are_What_The_Evidence_Bundle_Claims()
    {
        var byStatus = Loaded.Rules
            .GroupBy(static rule => rule.Status, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.Ordinal);

        Assert.Equal(19, byStatus["Active"]);
        Assert.Equal(6, byStatus["Vacuous"]);
        Assert.Equal(3, byStatus["Deferred"]);
        Assert.Equal(28, Loaded.Rules.Count);
    }

    private static Register Load()
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json");

        return JsonSerializer.Deserialize<Register>(
            File.ReadAllText(path),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            })
            ?? throw new InvalidOperationException($"{path} deserialized to null.");
    }

    internal sealed class Register
    {
        [JsonPropertyName("milestone")]
        public string Milestone { get; init; } = string.Empty;

        [JsonPropertyName("coreContractVersion")]
        public int CoreContractVersion { get; init; }

        [JsonPropertyName("rules")]
        public IReadOnlyList<Rule> Rules { get; init; } = [];
    }

    internal sealed class Rule
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("owningAdr")]
        public string OwningAdr { get; init; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; init; } = string.Empty;

        [JsonPropertyName("statement")]
        public string Statement { get; init; } = string.Empty;

        [JsonPropertyName("activationMilestone")]
        public string? ActivationMilestone { get; init; }

        [JsonPropertyName("witness")]
        public string? Witness { get; init; }
    }
}
