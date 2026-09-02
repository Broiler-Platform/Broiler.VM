using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group E: the rules that bind the decision records to the code and to each other.
/// </summary>
/// <remarks>
/// The core contract version is the number every support table, catalog entry and evidence
/// bundle has to name, so it must not be able to differ between the record that assigns it and
/// the assembly that implements it. These tests are the reason ADR 0003 can be quoted as
/// authoritative: the documentation and the constant fail together or not at all.
/// </remarks>
public sealed class CoreContractVersionTests
{
    private static readonly string AdrDirectory = Path.Combine(ComponentGraph.Root, "docs", "adr");

    private static IReadOnlyList<AdrFile> Adrs { get; } = LoadAdrs();

    /// <summary>
    /// The closed contract-bearing set, published by ADR 0003. ADR 0001 governs component shape
    /// and ADR 0012 governs ownership and support; neither carries contract surface.
    /// </summary>
    private static readonly string[] ContractBearing =
        ["0002", "0003", "0004", "0005", "0006", "0007", "0008", "0009", "0010", "0011"];

    /// <summary>
    /// E1's clean direction: ADR 0003's header fields and the constants agree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Rewritten from two equalities into messages on 2026-09-02</b>, so a report can say which
    /// of the two numbers disagreed and in which direction. A rule whose failure is
    /// "Assert.Equal() Failure: 1 != 2" tells a reader which two values differed and nothing about
    /// which field of which record produced them.
    /// </para>
    /// <para>
    /// A MISSING field is a message rather than a separate assertion, which is a small widening and
    /// a deliberate one: the old <c>HeaderInteger</c> helper asserted the field's existence before
    /// comparing, so a record that had dropped the field failed with a different message from one
    /// that had the field wrong. Both are the same defect - ADR 0003 no longer states the version
    /// the build implements - and they read as one rule now.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> E1Violations()
    {
        var adr = Adrs.Single(static file => file.Number == "0003");

        return E1FieldViolations(adr, "Core contract version", VmCoreContract.Version)
            .Concat(E1FieldViolations(
                adr, "Minimum supported version", VmCoreContract.MinimumSupportedVersion));
    }

    /// <summary>One header field against the constant that implements it.</summary>
    private static IEnumerable<string> E1FieldViolations(AdrFile adr, string field, int constant)
    {
        var declared = TryHeaderInteger(adr, field);

        if (declared is null)
        {
            yield return
                $"{adr.FileName} has no **{field}:** header field, and the build implements " +
                $"{constant}";
        }
        else if (declared.Value != constant)
        {
            yield return
                $"{adr.FileName} declares {field} {declared.Value}, and the build implements " +
                $"{constant}";
        }
    }

    [Fact]
    public void E1_The_Version_Constants_Match_The_Core_Contract_Adr()
    {
        Assert.Empty(E1Violations());

        // The witness: a record whose header fields disagree with the constants.
        var witness = Witness("E1-wrong-contract-version-fields.md.witness");

        Assert.NotEqual(VmCoreContract.Version, TryHeaderInteger(witness, "Core contract version"));
        Assert.NotEqual(
            VmCoreContract.MinimumSupportedVersion,
            TryHeaderInteger(witness, "Minimum supported version"));
    }

    /// <summary>
    /// E2's clean direction: every ADR declares whether it bears the contract.
    /// </summary>
    /// <remarks>
    /// Extracted from the test so a report can call it, and the test CALLS IT rather than keeping
    /// a copy - an extraction that left the assertion computing its own collection would be two
    /// implementations of one rule, which is the drift a report exists to prevent.
    /// </remarks>
    private static IEnumerable<string> E2Violations() => Adrs
        .Where(static adr => adr.CoreContractField is null)
        .Select(static adr => $"{adr.FileName} has no **Core contract:** header field");

    /// <summary>E3's clean direction: every contract-bearing ADR declares the current version.</summary>
    private static IEnumerable<string> E3Violations() => Adrs
        .Where(static adr => adr.IsContractBearing)
        .Where(static adr => ExtractVersion(adr.CoreContractField!) != VmCoreContract.Version)
        .Select(static adr => $"{adr.FileName} declares {adr.CoreContractField}");

    /// <summary>
    /// Writes what the reportable group E rules said about this checkout, when asked to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>E1 joined them on 2026-09-02</b>, rewritten from two equalities into a message list.
    /// </para>
    /// <para>
    /// <b>E5 is not here, and the reason recorded until 2026-09-02 was the wrong one.</b> Four
    /// bundles said E5 "produces no collection at all", which read as a rule written in an awkward
    /// shape. It is not: E5 is <b>Deferred</b>, superseded at VM-1 by V1 and V2, and its register
    /// row names <c>never</c> as its activation milestone. <b>No test asserts it</b>, because
    /// <c>RuleRegisterTests.Deferred_Rules_Are_Not_Asserted_And_Name_A_Later_Milestone</c> requires
    /// that none does. Reporting what E5 said about this checkout would mean writing the rule the
    /// register says is not asserted, which is a stronger objection than the one on record: the
    /// other exclusions were about how a rule is written, and this one is about whether the rule
    /// exists.
    /// </para>
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_E_Are_Written_When_Asked_For()
    {
        RuleReport.Write("E",
        [
            ("E1", E1Violations),
            ("E2", E2Violations),
            ("E3", E3Violations),
            ("E4", E4Violations),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "E.txt")),
                "a report for group E was asked for and none was written");
        }
    }

    [Fact]
    public void E2_Every_Adr_Declares_Whether_It_Is_Contract_Bearing()
    {
        Assert.Empty(E2Violations());

        var declared = Adrs
            .Where(static adr => adr.IsContractBearing)
            .Select(static adr => adr.Number)
            .OrderBy(static number => number, StringComparer.Ordinal);

        Assert.Equal(ContractBearing, declared);

        // The witness: a record carrying no Core contract field at all.
        Assert.Null(Witness("E2-missing-core-contract-header.md.witness").CoreContractField);
    }

    [Fact]
    public void E3_Every_Contract_Bearing_Adr_Declares_The_Current_Version()
    {
        Assert.Empty(E3Violations());

        // The witness: contract-bearing, but declaring a version the build does not implement.
        var witness = Witness("E3-wrong-declared-version.md.witness");

        Assert.True(witness.IsContractBearing);
        Assert.NotEqual(VmCoreContract.Version, ExtractVersion(witness.CoreContractField!));
    }

    /// <summary>E4's first clause: every ADR appears in the index.</summary>
    private static IEnumerable<string> E4UnlistedViolations(string index) => Adrs
        .Where(adr => !index.Contains(adr.FileName, StringComparison.Ordinal))
        .Select(static adr => $"{adr.FileName} is not listed in the index");

    /// <summary>E4's second clause: the index links nothing that is gone.</summary>
    private static IEnumerable<string> E4StaleViolations(IEnumerable<string> linked) => linked
        .Where(static file => !Adrs.Any(adr =>
            string.Equals(adr.FileName, file, StringComparison.Ordinal)))
        .Select(static file => $"the index links {file}, which does not exist");

    /// <summary>E4's clean direction over this checkout, both clauses.</summary>
    /// <remarks>
    /// The third clause - every rule an ADR names is registered - stays in the test: it reads the
    /// register through a helper the test owns, and moving it would move more than a rule.
    /// </remarks>
    private static IEnumerable<string> E4Violations()
    {
        var index = File.ReadAllText(Path.Combine(AdrDirectory, "README.md"));

        var linked = System.Text.RegularExpressions.Regex
            .Matches(index, @"\((?<file>\d{4}-[a-z0-9-]+\.md)\)")
            .Select(static match => match.Groups["file"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return E4UnlistedViolations(index).Concat(E4StaleViolations(linked));
    }

    [Fact]
    public void E4_The_Index_Lists_Every_Adr_And_Every_Named_Rule_Is_Registered()
    {
        var index = File.ReadAllText(Path.Combine(AdrDirectory, "README.md"));

        Assert.Empty(E4UnlistedViolations(index));

        var linked = Regex
            .Matches(index, @"\((?<file>\d{4}-[a-z0-9-]+\.md)\)")
            .Select(match => match.Groups["file"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(E4StaleViolations(linked));

        // Every architecture rule an ADR names as "Rule <ID>" must have a register row, or the
        // register has stopped being the authority the evidence bundle quotes.
        //
        // The character class is the set of group letters the register uses, and it is widened
        // whenever a group is added - J for the code-assurance rules here. No record names a J
        // rule today, so the class does not need J to keep the suite green; it needs J so that
        // the first record that DOES name one is checked rather than silently skipped, which is
        // the failure mode a character class has.
        var registered = RegisteredRuleIds();

        var unregistered = Adrs
            .SelectMany(static adr => Regex
                .Matches(adr.Text, @"\bRule (?<id>[A-EHJV]\d{1,2}b?)\b")
                .Select(match => (adr.FileName, Id: match.Groups["id"].Value)))
            .Where(named => !registered.Contains(named.Id))
            .Select(named => $"{named.FileName} names unregistered {named.Id}")
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(unregistered);

        // The witness: an index linking a record that does not exist, and naming a rule with no
        // register row. Both halves of E4 must fire on it.
        var witnessIndex = Witness("E4-index-links-a-missing-record.md.witness").Text;

        Assert.NotEmpty(Regex
            .Matches(witnessIndex, @"\((?<file>\d{4}-[a-z0-9-]+\.md)\)")
            .Select(match => match.Groups["file"].Value)
            .Where(file => !Adrs.Any(adr => string.Equals(adr.FileName, file, StringComparison.Ordinal)))
            .ToArray());

        Assert.NotEmpty(Regex
            .Matches(witnessIndex, @"\bRule (?<id>[A-Z]\d{1,2}b?)\b")
            .Select(match => match.Groups["id"].Value)
            .Where(id => !registered.Contains(id))
            .ToArray());
    }

    private static HashSet<string> RegisteredRuleIds()
    {
        var register = File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json"));

        return Regex
            .Matches(register, @"""id"":\s*""(?<id>[^""]+)""")
            .Select(match => match.Groups["id"].Value)
            .ToHashSet(StringComparer.Ordinal);
    }

    // HeaderInteger, which asserted a field's existence and then returned it, was deleted on
    // 2026-09-02 when E1 became a message list: E1FieldViolations reports a missing field as a
    // message, so the assertion had no caller left. Keeping it would have left a helper that
    // enforced a clause nothing enforced any more.
    private static int? TryHeaderInteger(AdrFile adr, string field)
    {
        var match = Regex.Match(adr.Text, $@"\*\*{Regex.Escape(field)}:\*\*\s*(?<value>\d+)");

        return match.Success ? int.Parse(match.Groups["value"].Value) : null;
    }

    private static int ExtractVersion(string field)
    {
        var match = Regex.Match(field, @"version\s+(?<value>\d+)");

        return match.Success ? int.Parse(match.Groups["value"].Value) : -1;
    }

    /// <summary>
    /// The witness records under witnesses/adr/, each a fixture a group E rule must reject.
    /// </summary>
    private static AdrFile Witness(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "adr", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return Parse(path);
    }

    private static AdrFile Parse(string path)
    {
        var text = File.ReadAllText(path);
        var field = Regex.Match(text, @"\*\*Core contract:\*\*\s*(?<value>[^
]+)");

        return new AdrFile(
            FileName: Path.GetFileName(path),
            Number: Path.GetFileName(path)[..4],
            Text: text,
            CoreContractField: field.Success ? field.Groups["value"].Value.Trim() : null);
    }

    private static IReadOnlyList<AdrFile> LoadAdrs()
    {
        if (!Directory.Exists(AdrDirectory))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(AdrDirectory, "*.md", SearchOption.TopDirectoryOnly)
            .Where(static path => Regex.IsMatch(Path.GetFileName(path), @"^\d{4}-"))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(static path =>
            {
                var text = File.ReadAllText(path);
                var field = Regex.Match(text, @"\*\*Core contract:\*\*\s*(?<value>[^\r\n]+)");

                return new AdrFile(
                    FileName: Path.GetFileName(path),
                    Number: Path.GetFileName(path)[..4],
                    Text: text,
                    CoreContractField: field.Success ? field.Groups["value"].Value.Trim() : null);
            })
            .ToArray();
    }

    private sealed record AdrFile(string FileName, string Number, string Text, string? CoreContractField)
    {
        internal bool IsContractBearing =>
            CoreContractField?.StartsWith("version", StringComparison.OrdinalIgnoreCase) == true;
    }
}
