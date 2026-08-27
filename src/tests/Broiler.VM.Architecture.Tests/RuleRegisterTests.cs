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
    public void Every_Active_Rule_Names_A_Witness_That_Resolves()
    {
        // Naming a witness is not enough - an earlier revision let a rule pass this check with a
        // prose prediction in the field. Every witness an Active rule names must resolve to a
        // file on disk or to a type in the test assembly.
        var types = typeof(RuleRegisterTests).Assembly
            .GetTypes()
            .Select(static type => type.Name)
            .ToHashSet(StringComparer.Ordinal);

        var unresolved = Loaded.Rules
            .Where(static rule => rule.Status == "Active")
            .SelectMany(rule => (rule.Witness ?? string.Empty)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(part => (rule.Id, Witness: part)))
            .Where(named => !Resolves(named.Witness, types))
            .Select(static named => $"{named.Id} names an unresolvable witness: {named.Witness}")
            .ToArray();

        var absent = Loaded.Rules
            .Where(static rule => rule.Status == "Active")
            .Where(static rule => string.IsNullOrWhiteSpace(rule.Witness))
            .Select(static rule => $"{rule.Id} is Active with no witness")
            .ToArray();

        Assert.Empty(unresolved.Concat(absent));

        static bool Resolves(string witness, IReadOnlySet<string> types)
        {
            var path = Path.Combine(
                ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
                witness.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(path))
            {
                return true;
            }

            // Otherwise it names a witness type compiled into the test assembly, written as
            // "TypeName in Broiler.VM.Architecture.Tests.dll" or "Outer.Nested in ...".
            var head = witness.Split(" in ")[0].Split('.')[^1].Trim();

            if (types.Contains(head))
            {
                return true;
            }

            // Or it names a whole assembly, which is the honest form for a rule whose witness is
            // what an assembly REFERENCES rather than a type it declares - B1's witness is that
            // the test assembly references xunit at all.
            var assembly = witness.Split(' ')[0].Trim();

            return assembly.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                File.Exists(Path.Combine(AppContext.BaseDirectory, assembly));
        }
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
