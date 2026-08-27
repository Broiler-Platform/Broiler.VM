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

    [Fact]
    public void E1_The_Version_Constants_Match_The_Core_Contract_Adr()
    {
        var adr = Adrs.Single(static file => file.Number == "0003");

        Assert.Equal(VmCoreContract.Version, HeaderInteger(adr, "Core contract version"));
        Assert.Equal(VmCoreContract.MinimumSupportedVersion, HeaderInteger(adr, "Minimum supported version"));

        // The witness: a record whose header fields disagree with the constants.
        var witness = Witness("E1-wrong-contract-version-fields.md.witness");

        Assert.NotEqual(VmCoreContract.Version, TryHeaderInteger(witness, "Core contract version"));
        Assert.NotEqual(
            VmCoreContract.MinimumSupportedVersion,
            TryHeaderInteger(witness, "Minimum supported version"));
    }

    [Fact]
    public void E2_Every_Adr_Declares_Whether_It_Is_Contract_Bearing()
    {
        var missing = Adrs
            .Where(static adr => adr.CoreContractField is null)
            .Select(static adr => $"{adr.FileName} has no **Core contract:** header field")
            .ToArray();

        Assert.Empty(missing);

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
        var wrong = Adrs
            .Where(static adr => adr.IsContractBearing)
            .Where(static adr => ExtractVersion(adr.CoreContractField!) != VmCoreContract.Version)
            .Select(static adr => $"{adr.FileName} declares {adr.CoreContractField}")
            .ToArray();

        Assert.Empty(wrong);

        // The witness: contract-bearing, but declaring a version the build does not implement.
        var witness = Witness("E3-wrong-declared-version.md.witness");

        Assert.True(witness.IsContractBearing);
        Assert.NotEqual(VmCoreContract.Version, ExtractVersion(witness.CoreContractField!));
    }

    [Fact]
    public void E4_The_Index_Lists_Every_Adr_And_Every_Named_Rule_Is_Registered()
    {
        var index = File.ReadAllText(Path.Combine(AdrDirectory, "README.md"));

        var unlisted = Adrs
            .Where(adr => !index.Contains(adr.FileName, StringComparison.Ordinal))
            .Select(static adr => $"{adr.FileName} is not listed in the index")
            .ToArray();

        Assert.Empty(unlisted);

        var linked = Regex
            .Matches(index, @"\((?<file>\d{4}-[a-z0-9-]+\.md)\)")
            .Select(match => match.Groups["file"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var stale = linked
            .Where(file => !Adrs.Any(adr => string.Equals(adr.FileName, file, StringComparison.Ordinal)))
            .Select(file => $"the index links {file}, which does not exist")
            .ToArray();

        Assert.Empty(stale);

        // Every architecture rule an ADR names as "Rule <ID>" must have a register row, or the
        // register has stopped being the authority the evidence bundle quotes.
        var registered = RegisteredRuleIds();

        var unregistered = Adrs
            .SelectMany(static adr => Regex
                .Matches(adr.Text, @"\bRule (?<id>[A-EV]\d{1,2}b?)\b")
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

    private static int HeaderInteger(AdrFile adr, string field)
    {
        var value = TryHeaderInteger(adr, field);

        Assert.True(value is not null, $"{adr.FileName} has no **{field}:** header field.");

        return value!.Value;
    }

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
