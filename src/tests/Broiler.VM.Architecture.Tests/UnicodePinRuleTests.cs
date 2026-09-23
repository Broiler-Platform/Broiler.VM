using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N22: the Unicode tables are what the pinned archive generates, and the archive is the one
/// the pin describes.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a rule and not a review.</b> Decision JSD-0031 moves the profile's Unicode answers -
/// <c>normalize</c>, <c>\p{...}</c> and <c>u</c>-mode case folding - from the runtime's data onto
/// tables this repository generates. A table is a few hundred kilobytes of numbers nobody reads, so
/// the only statement about it worth anything is a mechanical one: these bytes are what this
/// generator writes from those files, and those files are the ones a person retrieved twice and
/// hashed. Every link in that sentence is a clause here, and each clause has its own witness.
/// </para>
/// <para>
/// <b>The four failures the decision names.</b> A changed input byte (the hash clause), a file of
/// another release (the header clause), a hand edit to a generated file (the currency clause) and a
/// disagreement between the specification's property tables and <c>PropertyAliases.txt</c> (the
/// cross-check). The first and third are witnessed by altering the real inputs in memory, because
/// a witness file of two megabytes would be a second archive; the second and fourth by small
/// witness files under <c>witnesses/register</c>.
/// </para>
/// </remarks>
public sealed class UnicodePinRuleTests
{
    /// <summary>
    /// The gate, and in write mode the generator: the checked-in tables are byte-identical to what
    /// the pinned archive generates.
    /// </summary>
    [Fact]
    public void N22_The_Generated_Tables_Are_What_The_Pinned_Archive_Generates()
    {
        var generation = UnicodeTableGenerator.Current;

        // Non-vacuous: three files, every one of them non-trivial, from a pin naming every input the
        // decision lists - thirteen UCD files, the licence and three specification tables.
        Assert.Equal(UnicodeTableGenerator.OutputPaths, generation.Artefacts.Select(static artefact => artefact.RelativePath));
        Assert.Equal(17, UnicodePin.Load().Entries.Count);
        Assert.All(generation.Artefacts, static artefact => Assert.True(artefact.Desired.Length > 10_000));

        // And the probes slice U3 runs through the end-user host: every part of
        // NormalizationTest.txt and every span of the code space, each probe non-trivial.
        Assert.All(generation.Probes, static probe =>
        {
            Assert.StartsWith($"{UnicodeNormalizationProbes.Directory}/{UnicodeNormalizationProbes.Prefix}", probe.RelativePath, StringComparison.Ordinal);
            Assert.Contains("normalize(", probe.Desired, StringComparison.Ordinal);
        });
        Assert.Equal(20_034, generation.Probes.Sum(static probe => probe.Desired.Split('\n').Count(static line => line.StartsWith("v(", StringComparison.Ordinal))));
        Assert.Equal(9, generation.Probes.Count(static probe => probe.RelativePath.Contains("-invariants-", StringComparison.Ordinal)));

        if (UnicodeTableGenerator.WriteRequested)
        {
            UnicodeTableGenerator.Apply(generation);
            return;
        }

        Assert.Empty(UnicodeTableGenerator.Stale(generation.Artefacts.Concat(generation.Probes)));
    }

    /// <summary>Generation twice yields byte-identical output.</summary>
    [Fact]
    public void N22_Generating_Twice_Yields_The_Same_Bytes()
    {
        var pin = UnicodePin.Load();
        var archive = pin.ReadArchive();
        var first = UnicodeTableGenerator.Generate(pin, archive, static _ => string.Empty);
        var second = UnicodeTableGenerator.Generate(pin, archive, static _ => string.Empty);

        Assert.Equal(
            first.Artefacts.Concat(first.Probes).Select(static artefact => artefact.Desired),
            second.Artefacts.Concat(second.Probes).Select(static artefact => artefact.Desired));

        // And nothing in the output names a machine or a moment.
        Assert.All(first.Artefacts.Concat(first.Probes), artefact =>
        {
            Assert.DoesNotContain(ComponentGraph.Root, artefact.Desired, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\r", artefact.Desired, StringComparison.Ordinal);
            Assert.DoesNotContain(DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), artefact.Desired, StringComparison.Ordinal);
        });
    }

    /// <summary>The hash clause: one changed byte of one input, and nothing is generated.</summary>
    [Fact]
    public void N22_Rejects_A_Changed_Input_Byte()
    {
        var pin = UnicodePin.Load();

        Assert.Empty(pin.Violations(pin.ReadArchive()));

        var archive = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        var changed = (byte[])archive["ucd-17.0.0/CaseFolding.txt"].Clone();

        // The last byte of the last data line's target, which is a hex digit: a mapping moved by one.
        var at = Array.LastIndexOf(changed, (byte)';');
        changed[at - 1] = changed[at - 1] == (byte)'0' ? (byte)'1' : (byte)'0';
        archive["ucd-17.0.0/CaseFolding.txt"] = changed;

        var violations = pin.Violations(archive).ToArray();

        Assert.Single(violations);
        Assert.Contains("CaseFolding.txt hashes to", violations[0], StringComparison.Ordinal);

        var refused = Assert.Throws<InvalidDataException>(() =>
            UnicodeTableGenerator.Generate(pin, archive, static _ => string.Empty));

        Assert.Contains("nothing is generated from it", refused.Message, StringComparison.Ordinal);

        // A file the pin names and the archive lacks is the same refusal, not a smaller table.
        archive.Remove("ucd-17.0.0/Scripts.txt");
        Assert.Contains(pin.Violations(archive), static violation => violation.Contains("Scripts.txt is pinned and no file is there", StringComparison.Ordinal));
    }

    /// <summary>
    /// The header clause: a file of another release is refused even when a pin records exactly its
    /// bytes, because the hash says the file is unchanged and only the header says which release.
    /// </summary>
    [Fact]
    public void N22_Rejects_A_Header_Naming_Another_Version()
    {
        var witness = File.ReadAllBytes(Witness("N22-a-header-naming-another-version.txt.witness"));
        var digest = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(witness));
        var pin = UnicodePin.Parse(
            "version 17.0.0\narchived yes\n" +
            $"file ucd-17.0.0/CaseFolding.txt {witness.Length} {digest}\n");

        var violations = pin.Violations(new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            ["ucd-17.0.0/CaseFolding.txt"] = witness,
        }).ToArray();

        Assert.Single(violations);
        Assert.Contains(
            "the first line is `# CaseFolding-16.0.0.txt`, and a 17.0.0 file opens `# CaseFolding-17.0.0.txt`",
            violations[0],
            StringComparison.Ordinal);

        // The pin's own version line is the same clause one level up.
        Assert.Contains(
            UnicodePin.Parse("version 16.0.0\narchived yes\n").Violations(new Dictionary<string, byte[]>()),
            static violation => violation.Contains("names version `16.0.0`", StringComparison.Ordinal));

        // emoji-data.txt states its version on a later line, and UnicodeData.txt states none.
        Assert.NotNull(UnicodePin.HeaderViolation("ucd-17.0.0/emoji/emoji-data.txt", Encoding.UTF8.GetBytes("# emoji-data.txt\n# Version: 16.0\n"), "17.0.0"));
        Assert.Null(UnicodePin.HeaderViolation("ucd-17.0.0/UnicodeData.txt", Encoding.UTF8.GetBytes("0000;<control>;Cc;0;BN;;;;;N;NULL;;;;\n"), "17.0.0"));
    }

    /// <summary>The currency clause: a hand edit to a generated file is reported, with what to do instead.</summary>
    [Fact]
    public void N22_Rejects_A_Hand_Edit_To_A_Generated_File()
    {
        var artefact = UnicodeTableGenerator.Current.Artefacts[0];
        var text = artefact.Desired;

        // One digit of the first table's first data line: a property that now contains one code
        // point more or less.
        var table = text.IndexOf("new byte[]", StringComparison.Ordinal);
        var digit = text.IndexOf("\n        ", table, StringComparison.Ordinal) + 9;
        var edited = text[..digit] + (text[digit] == '1' ? '2' : '1') + text[(digit + 1)..];

        var stale = UnicodeTableGenerator.Stale([artefact with { Current = edited }]);

        Assert.Single(stale);
        Assert.Contains($"{artefact.RelativePath} is not what the Unicode table generator writes", stale[0], StringComparison.Ordinal);
        Assert.Contains("never edit the file", stale[0], StringComparison.Ordinal);

        Assert.Empty(UnicodeTableGenerator.Stale([artefact with { Current = text }]));
    }

    /// <summary>
    /// The cross-check: a name the specification's tables admit and <c>PropertyAliases.txt</c> does
    /// not give that property is a disagreement, and the agreeing rows beside it are not.
    /// </summary>
    [Fact]
    public void N22_Rejects_A_Property_Table_That_Disagrees_With_PropertyAliases()
    {
        var aliases = UnicodePin.Decode(UnicodePin.Load().ReadArchive()["ucd-17.0.0/PropertyAliases.txt"]);
        var archived = UnicodeSpecTables.Rows(File.ReadAllText(Path.Combine(ComponentGraph.Root, UnicodeSpecTables.Binary)));
        var nonBinary = UnicodeSpecTables.Rows(File.ReadAllText(Path.Combine(ComponentGraph.Root, UnicodeSpecTables.NonBinary)));
        var strings = UnicodeSpecTables.Rows(File.ReadAllText(Path.Combine(ComponentGraph.Root, UnicodeSpecTables.Strings)));

        // The archived tables agree, over every row: 53 binary properties, their aliases, and the
        // six non-binary names.
        Assert.Empty(UnicodeSpecTables.Disagreements(nonBinary, archived, strings, aliases));
        Assert.Equal(53, archived.Select(static row => row.Canonical).Distinct().Count());
        Assert.Equal(6, nonBinary.Count);
        Assert.Equal(7, strings.Count);

        var witness = UnicodeSpecTables.Rows(File.ReadAllText(Witness("N22-a-property-table-admitting-an-alias-the-ucd-lacks.html.witness")));
        var disagreements = UnicodeSpecTables.Disagreements([], witness, [], aliases).ToArray();

        Assert.Equal(4, witness.Count);
        Assert.Single(disagreements);
        Assert.StartsWith("the table admits `Whitespace` for `White_Space`", disagreements[0], StringComparison.Ordinal);

        // A property of strings admitted as a code point property is a disagreement too.
        Assert.Contains(
            UnicodeSpecTables.Disagreements([], [new UnicodeSpecName("RGI_Emoji", "Emoji")], strings, aliases),
            static violation => violation.Contains("`RGI_Emoji` is a property of strings", StringComparison.Ordinal));
    }

    /// <summary>
    /// The owner's size decision of 2026-09-22: total table data under a hard cap of 300 KB,
    /// measured twice - by the generator, and by counting the byte literals in the checked-in files.
    /// </summary>
    [Fact]
    public void N22_The_Table_Data_Stays_Under_The_Budget()
    {
        var generation = UnicodeTableGenerator.Current;
        var counted = UnicodeTableGenerator.OutputPaths.Sum(static path =>
            CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(ComponentGraph.Root, path)))
                .GetRoot()
                .DescendantNodes()
                .OfType<ArrayCreationExpressionSyntax>()
                .Sum(static array => array.Initializer?.Expressions.Count ?? 0));

        Assert.Equal(generation.TotalBytes, counted);
        Assert.True(
            generation.TotalBytes <= UnicodeTableGenerator.BudgetBytes,
            $"the Unicode tables hold {generation.TotalBytes} bytes, over the {UnicodeTableGenerator.BudgetBytes}-byte cap");

        // Non-vacuous: the data is real, and every table the decision names is present.
        Assert.True(generation.TotalBytes > 100_000);
        // 17 from slice U2, and the non-u Canonicalize mapping and its reverse from JSeal slice
        // JSD-0031-later.
        Assert.Equal(19, generation.Tables.Count);
    }

    /// <summary>
    /// The records that carry the archive's obligations say what the pin says: the specification
    /// directory's README records each property table's length and hash, and the notices file
    /// carries the Unicode licence text as archived, which is how it travels in every package.
    /// </summary>
    [Fact]
    public void N22_The_Readme_And_The_Notices_Carry_What_The_Pin_Records()
    {
        var pin = UnicodePin.Load();
        var readme = File.ReadAllText(Path.Combine(ComponentGraph.Root, "src/Broiler.VM.Profile.JavaScript/docs/specification/README.md"));

        foreach (var entry in pin.Entries.Where(static entry => entry.Kind == "spec-table"))
        {
            Assert.Contains(entry.Sha256, readme, StringComparison.Ordinal);
            Assert.Contains(entry.Bytes.ToString("N0", CultureInfo.InvariantCulture), readme, StringComparison.Ordinal);
        }

        Assert.Equal(3, pin.Entries.Count(static entry => entry.Kind == "spec-table"));

        var notices = File.ReadAllText(Path.Combine(ComponentGraph.Root, "THIRD_PARTY_NOTICES.md"));
        var licence = UnicodePin.Decode(pin.ReadArchive()["unicode-LICENSE.txt"]).TrimEnd('\n');

        Assert.Contains(licence, notices, StringComparison.Ordinal);
        Assert.StartsWith("UNICODE LICENSE V3", licence, StringComparison.Ordinal);
    }

    private static string Witness(string name) =>
        Path.Combine(ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/witnesses/register", name);
}
