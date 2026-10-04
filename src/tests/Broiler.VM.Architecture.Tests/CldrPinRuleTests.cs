namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N27: the archived CLDR files are the ones <c>cldr.pin</c> describes, and the pin names every
/// file archived beside it.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a rule before anything reads the archive.</b> Decision JSD-0027 section 5 asks for CLDR to
/// be pinned by release and digest, as JSD-0031 pinned the UCD, before any Intl table is generated
/// from it; a generator written later is only as trustworthy as the bytes it is handed, and the
/// archive is several megabytes nobody reads. So the mechanical statement comes first: these bytes
/// are what the pin records, of the release it names.
/// </para>
/// <para>
/// <b>Two failures, each with a witness.</b> A changed byte (the hash clause), made in memory against
/// the real archive because a witness of two megabytes would be a second archive; and a file of
/// another release (the version clause), refused from a small witness file even under a pin that
/// records exactly its bytes, because the hash says a file is unchanged and only its stated version
/// says which release it is.
/// </para>
/// </remarks>
public sealed class CldrPinRuleTests
{
    /// <summary>The gate: the archive is the one the pin describes, and the pin describes all of it.</summary>
    [Fact]
    public void N27_The_Archive_Is_The_One_The_Pin_Describes()
    {
        var pin = CldrPin.Load();

        Assert.Empty(pin.Violations(pin.ReadArchive()));
        Assert.True(pin.Entries.Count > 20, $"the pin names {pin.Entries.Count} files, and the archive holds more than twenty");
        Assert.Equal("48", pin.Values["cldr-major"]);
        Assert.Equal(CldrPin.PinnedUcaVersion, pin.Values["uca-version"]);
        Assert.Equal("2 byte-identical", pin.Values["retrievals"]);
        Assert.Contains(pin.Integrities, static line => line.StartsWith("cldr-core sha512-", StringComparison.Ordinal));
        Assert.Contains(pin.Integrities, static line => line.StartsWith("cldr-bcp47 sha512-", StringComparison.Ordinal));

        // THE PIN NAMES EVERY FILE BESIDE IT: a file archived and not pinned is a file nothing
        // verified, and it would be read by the first generator that globbed the directory.
        var directory = Path.Combine(ComponentGraph.Root, CldrPin.PinDirectory);
        var archived = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(directory, file).Replace(Path.DirectorySeparatorChar, '/'))
            .Where(static name => name is not "cldr.pin" and not "README.md")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            pin.Entries.Select(static entry => entry.Name).Order(StringComparer.Ordinal).ToArray(),
            archived);

        // EVERY VERSION-BEARING KIND IS PRESENT, so the version clause is exercised on real files.
        Assert.Contains(pin.Entries, static entry => entry.Name.EndsWith("/package.json", StringComparison.Ordinal));
        Assert.Contains(pin.Entries, static entry => entry.Name.Contains("/supplemental/", StringComparison.Ordinal));
        Assert.Contains(pin.Entries, static entry => entry.Name.EndsWith("/common/uca/allkeys_CLDR.txt", StringComparison.Ordinal));
    }

    /// <summary>The hash clause: one changed byte of one input is reported, for that file alone.</summary>
    [Fact]
    public void N27_Rejects_A_Changed_Input_Byte()
    {
        var pin = CldrPin.Load();
        var archive = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        const string Name = "cldr-48.2.0/common/uca/allkeys_CLDR.txt";
        var changed = (byte[])archive[Name].Clone();

        // The last weight digit of the last data line: one collation element moved by one.
        var at = Array.LastIndexOf(changed, (byte)']');
        changed[at - 1] = changed[at - 1] == (byte)'0' ? (byte)'1' : (byte)'0';
        archive[Name] = changed;

        var violations = pin.Violations(archive).ToArray();

        Assert.Single(violations);
        Assert.Contains("allkeys_CLDR.txt hashes to", violations[0], StringComparison.Ordinal);

        var refused = Assert.Throws<InvalidDataException>(() => pin.Verified(archive));
        Assert.Contains("nothing is generated from it", refused.Message, StringComparison.Ordinal);

        // A file the pin names and the archive lacks is the same refusal, not a smaller archive.
        archive.Remove("cldr-48.2.0/json/cldr-core/supplemental/likelySubtags.json");
        Assert.Contains(pin.Violations(archive), static violation => violation.Contains("likelySubtags.json is pinned and no file is there", StringComparison.Ordinal));
    }

    /// <summary>
    /// The version clause: a supplemental file of CLDR 45 and a UCA file of 16.0.0 are refused even
    /// under a pin that records exactly their bytes.
    /// </summary>
    [Fact]
    public void N27_Rejects_A_File_Stating_Another_Release()
    {
        var supplemental = File.ReadAllBytes(Witness("N27-a-supplemental-file-of-another-release.json.witness"));
        var uca = File.ReadAllBytes(Witness("N27-a-uca-file-of-another-version.txt.witness"));

        var forged = CldrPin.Parse(
            "version 48.2.0\narchived yes\n" +
            $"file cldr-48.2.0/json/cldr-core/supplemental/likelySubtags.json {supplemental.Length} {Hash(supplemental)}\n" +
            $"file cldr-48.2.0/common/uca/allkeys_CLDR.txt {uca.Length} {Hash(uca)}\n");

        var violations = forged.Violations(new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            ["cldr-48.2.0/json/cldr-core/supplemental/likelySubtags.json"] = supplemental,
            ["cldr-48.2.0/common/uca/allkeys_CLDR.txt"] = uca,
        }).ToArray();

        Assert.Equal(2, violations.Length);
        Assert.Contains(violations, static violation => violation.Contains("states CLDR version `45`", StringComparison.Ordinal));
        Assert.Contains(violations, static violation => violation.Contains("does not state `# UCA Version: 17.0.0`", StringComparison.Ordinal));

        // A package of another release is refused the same way.
        Assert.NotNull(CldrPin.VersionViolation("cldr-48.2.0/json/cldr-core/package.json", "{\"version\":\"47.0.0\"}"u8.ToArray()));
        Assert.Null(CldrPin.VersionViolation("cldr-48.2.0/json/cldr-core/package.json", "{\"version\":\"48.2.0\"}"u8.ToArray()));
    }

    /// <summary>
    /// The archive is declared binary, so no checkout filter rewrites a line ending under a recorded
    /// hash, and its README and licence say what the pin says.
    /// </summary>
    [Fact]
    public void N27_The_Archive_Is_Declared_Binary_And_Recorded()
    {
        var attributes = File.ReadAllText(Path.Combine(ComponentGraph.Root, ".gitattributes"));

        Assert.Contains("src/tests/cldr/pins/cldr-48.2.0/** binary", attributes, StringComparison.Ordinal);
        Assert.Contains("src/tests/cldr/pins/cldr-LICENSE.txt binary", attributes, StringComparison.Ordinal);

        var readme = File.ReadAllText(Path.Combine(ComponentGraph.Root, CldrPin.PinDirectory, "README.md"));

        Assert.Contains("CLDR 48.2.0", readme, StringComparison.Ordinal);
        Assert.Contains("JSD-0027", readme, StringComparison.Ordinal);

        var pin = CldrPin.Load();
        var licence = CldrPin.Decode(pin.ReadArchive()["cldr-LICENSE.txt"]);

        Assert.StartsWith("UNICODE LICENSE V3", licence, StringComparison.Ordinal);
    }

    private static string Hash(byte[] bytes) =>
        Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));

    private static string Witness(string name) =>
        Path.Combine(ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/witnesses/register", name);
}
