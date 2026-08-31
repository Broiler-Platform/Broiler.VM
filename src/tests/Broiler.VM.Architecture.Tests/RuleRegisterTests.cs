using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The register is itself under test. It is the artefact the ADRs, the evidence bundle and the
/// status ledger all quote when they say how much VM-0 proves, so nothing may drift between it,
/// the tests that run, and the witness inputs on disk.
/// </summary>
public sealed class RuleRegisterTests
{
    /// <summary>
    /// The register, loaded once. Internal because a rule whose subject is a document of its own -
    /// group L's baseline register, for one - has to hold its own row to what it proves, and
    /// reloading the file in each such test would give two readers of one file that could disagree.
    /// </summary>
    internal static readonly Register Loaded = Load();

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
        //
        // The current milestone is read from the register rather than written into this test.
        // Hardcoding it made the predicate silently weaker the moment the register advanced: a
        // row naming the NEW current milestone would have passed a check still looking for the
        // old one.
        var current = Loaded.Milestone;

        var violations = Loaded.Rules
            .Where(static rule => rule.Status is "Deferred" or "Vacuous")
            .Where(static rule => !rule.IsPermanent)
            .Where(rule => rule.ActivationMilestone is null ||
                string.CompareOrdinal(rule.ActivationMilestone, current) <= 0)
            .Select(rule => $"{rule.Id} is {rule.Status} but names activation milestone {rule.ActivationMilestone ?? "none"}, which is not later than {current}")
            .ToArray();

        Assert.Empty(violations);

        // The escape hatch, and the thing that stops it being one. A row may say it will NEVER
        // activate - VM-6 is the last milestone, so "name a later one" is not always answerable -
        // but only by carrying a reason in a field of its own. Prose in the statement would not do:
        // the check has to be able to see it, and a row that could satisfy this by being worded
        // carefully is a row that satisfies nothing.
        var unexplained = Loaded.Rules
            .Where(static rule => rule.IsPermanent)
            .Where(static rule => string.IsNullOrWhiteSpace(rule.PermanenceReason))
            .Select(static rule => $"{rule.Id} says it never activates and gives no reason")
            .ToArray();

        Assert.Empty(unexplained);

        // And the hatch is closed to Active rows. A rule that is asserted has activated, so
        // claiming permanence would be claiming both.
        var contradictory = Loaded.Rules
            .Where(static rule => rule.Status == "Active" && rule.IsPermanent)
            .Select(static rule => $"{rule.Id} is Active and also says it never activates")
            .ToArray();

        Assert.Empty(contradictory);

        var activeWithMilestone = Loaded.Rules
            .Where(static rule => rule.Status == "Active" && rule.ActivationMilestone is not null)
            .Where(static rule => !rule.IsPermanent)
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
        // Every *.witness file under witnesses/, recursively. Globbing only *.csproj.witness in
        // the top directory left witnesses/adr/ and witnesses/review/ entirely unchecked, so an
        // orphaned documentation witness - one whose rule was deleted, renamed or never written -
        // sat on disk looking like evidence for a rule that no longer read it.
        //
        // The comparison is by whole path and not by containment. Asking whether some rule's
        // witness field CONTAINED the file name made every orphan whose name is a suffix of a
        // named witness invisible - unknown-mark.md.witness is a suffix of
        // H1-table-cell-unknown-mark.md.witness - and the realistic orphan is exactly that: a
        // witness renamed to carry its rule prefix, the register updated, and the old file left
        // behind. The same containment also let a rule name a path that resolves to nothing.
        var named = NamedWitnesses()
            .Select(static named => named.Witness)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var orphans = ComponentGraph.WitnessInputs
            .Select(WitnessPath)
            .Where(file => !named.Contains(file))
            .Select(static file => $"{file} is on disk and no rule names it")
            .ToArray();

        Assert.Empty(orphans);
    }

    /// <summary>
    /// A witness file named for a rule is evidence for THAT rule. The three checks either side of
    /// this one are all satisfied by a permutation of the truth: exchanging two rows' witness
    /// fields wholesale leaves every path resolving, every file named by some rule, and every
    /// count unchanged, so the register could record the attestation witnesses as the figure
    /// rule's evidence and nothing would disagree. The file-name prefix is the one part of a
    /// witness that says which rule it belongs to, so it is held to the row it sits in.
    /// </summary>
    /// <remarks>
    /// Only the rule that OWNS the prefix has to name the file. A second rule may name it as well,
    /// which is how A7 and A8 share one project-file witness: the same file violates both, and
    /// saying so twice is honest rather than duplicated.
    /// </remarks>
    [Fact]
    public void Every_Witness_Named_For_A_Rule_Is_Named_By_That_Rule()
    {
        var byRule = NamedWitnesses()
            .GroupBy(static named => named.Id, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Select(static named => named.Witness)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                StringComparer.Ordinal);

        var ids = Loaded.Rules.Select(static rule => rule.Id).ToHashSet(StringComparer.Ordinal);

        var misfiled = ComponentGraph.WitnessInputs
            .Select(path => (Path: WitnessPath(path), Owner: RulePrefix(path)))
            .Where(witness => witness.Owner is not null)
            .Select(witness => (witness.Path, Owner: witness.Owner!))
            .Where(witness => !ids.Contains(witness.Owner) ||
                !byRule.TryGetValue(witness.Owner, out var named) ||
                !named.Contains(witness.Path))
            .Select(static witness =>
                $"{witness.Path} is named for rule {witness.Owner}, and rule {witness.Owner} does not name it")
            .ToArray();

        Assert.Empty(misfiled);
    }

    /// <summary>
    /// The rule a witness file is named for: the identifier its file name opens with, if it opens
    /// with one at all. Witness files that carry no rule prefix - the shared fixtures - are not
    /// bound to any row by this.
    /// </summary>
    private static string? RulePrefix(string path)
    {
        var match = WitnessRulePrefix.Match(Path.GetFileName(path));

        return match.Success ? match.Groups["id"].Value : null;
    }

    private static readonly Regex WitnessRulePrefix =
        new(@"^(?<id>[A-Z]{1,2}\d{1,2}[a-z]?)-", RegexOptions.Compiled);

    /// <summary>
    /// A row that says nothing about itself is not a register row. Nothing else in the suite reads
    /// <c>statement</c>, <c>evidence</c> or <c>nonVacuousWhen</c>, so a row could be emptied of the
    /// prose that carries its honest limits and stay green; the group H tests hold their own rows
    /// to the specific limits their rules depend on, and this holds every row to having them at
    /// all.
    /// </summary>
    [Fact]
    public void Every_Rule_Row_States_Itself()
    {
        var silent = Loaded.Rules
            .SelectMany(static rule => new[]
            {
                (rule.Id, Field: "statement", Value: rule.Statement),
                (rule.Id, Field: "evidence", Value: rule.Evidence),
                (rule.Id, Field: "nonVacuousWhen", Value: rule.NonVacuousWhen),
            })
            .Where(static field => string.IsNullOrWhiteSpace(field.Value))
            .Select(static field => $"{field.Id} has an empty {field.Field}")
            .ToArray();

        Assert.Empty(silent);
    }

    private static IEnumerable<(string Id, string Witness)> NamedWitnesses() =>
        Loaded.Rules
            .SelectMany(static rule => (rule.Witness ?? string.Empty)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(part => (rule.Id, Witness: part)));

    private static string WitnessPath(string path) =>
        Path.GetRelativePath(
            Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests"),
            path)
            .Replace('\\', '/');

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

        // JS-0 mints group N - four rules over the JavaScript profile family's project graph -
        // and changes neither of the other two counts: the one Vacuous row is B3, which nothing
        // in the graph can violate, and the one Deferred row still awaits its own milestone.
        Assert.Equal(67, byStatus["Active"]);
        Assert.Equal(1, byStatus["Vacuous"]);
        Assert.Equal(1, byStatus["Deferred"]);
        Assert.Equal(69, Loaded.Rules.Count);
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

        [JsonPropertyName("evidence")]
        public string Evidence { get; init; } = string.Empty;

        [JsonPropertyName("nonVacuousWhen")]
        public string NonVacuousWhen { get; init; } = string.Empty;

        [JsonPropertyName("activationMilestone")]
        public string? ActivationMilestone { get; init; }

        [JsonPropertyName("witness")]
        public string? Witness { get; init; }

        /// <summary>
        /// Why this rule can never activate, for the rows whose activation milestone is "never".
        /// </summary>
        /// <remarks>
        /// A separate field rather than a sentence in the statement, so the check that demands it
        /// can see it. VM-6 is the last milestone in the roadmap, so the register's own rule -
        /// a Deferred or Vacuous row must name a LATER milestone - has no answer for a rule whose
        /// subject will never exist. This is that answer, and requiring a reason beside it is what
        /// stops it becoming a way to retire an inconvenient row.
        /// </remarks>
        [JsonPropertyName("permanenceReason")]
        public string? PermanenceReason { get; init; }

        /// <summary>Whether this row claims it can never activate.</summary>
        internal bool IsPermanent =>
            string.Equals(ActivationMilestone, "never", StringComparison.Ordinal);
    }
}
