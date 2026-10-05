namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N29: the archived tzdb release is the one <c>tzdb.pin</c> describes. Rule N30: the time zone
/// tables are what <see cref="TzdbTableGenerator"/> writes from it.
/// </summary>
/// <remarks>
/// <para>
/// <b>N27 and N28's statements over a third archive</b> (decision JSD-0053). The archive is one
/// tarball, so the hash clause is over its bytes and then over each member the generator reads; the
/// version clause is the release's own <c>version</c> member. The witnesses are made in memory against
/// the real archive, as N27's are.
/// </para>
/// <para>
/// <b>The compiler is checked beside its tables.</b> A table nobody reads is only as right as the
/// compiler that wrote it, so N30 also holds a handful of offsets whose history is well known, read
/// back from the compiled zones, to the values the release's own commentary states.
/// </para>
/// </remarks>
public sealed class TzdbRuleTests
{
    /// <summary>The gate: the archive is the one the pin describes, and nothing else is beside it.</summary>
    [Fact]
    public void N29_The_Archive_Is_The_One_The_Pin_Describes()
    {
        var pin = TzdbPin.Load();
        var archive = pin.ReadArchive();

        Assert.Empty(pin.TarballViolations(pin.ReadTarball()));
        Assert.Empty(pin.Violations(archive));
        Assert.Equal(TzdbPin.PinnedVersion, pin.Version);
        Assert.Equal("2 byte-identical", pin.Values["retrievals"]);
        Assert.Equal("public-domain", pin.Values["licence"]);
        Assert.Equal(12, pin.Members.Count);
        Assert.Equal(pin.Members.Select(static member => member.Name).Order(StringComparer.Ordinal), archive.Keys.Order(StringComparer.Ordinal));

        foreach (var file in TzdbCompiler.SourceFiles)
        {
            Assert.Contains(pin.Members, member => member.Name == file);
        }

        Assert.StartsWith("Unless specified below, all files in the tz code and data", TzdbPin.Decode(archive["LICENSE"]), StringComparison.Ordinal);

        // NOTHING IS BESIDE THE PIN but the archive and the README.
        var directory = Path.Combine(ComponentGraph.Root, TzdbPin.PinDirectory);
        Assert.Equal(
            new[] { "README.md", pin.ArchiveName, "tzdb.pin" }.Order(StringComparer.Ordinal),
            Directory.EnumerateFiles(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal));
    }

    /// <summary>The hash clauses: one changed byte of the tarball, or of a member, is refused.</summary>
    [Fact]
    public void N29_Rejects_A_Changed_Byte()
    {
        var pin = TzdbPin.Load();
        var tarball = pin.ReadTarball();
        tarball[tarball.Length / 2] ^= 1;

        var refused = Assert.Throws<InvalidDataException>(() => pin.Extract(tarball));
        Assert.Contains("nothing is generated from it", refused.Message, StringComparison.Ordinal);
        Assert.Contains("tzdata2026e.tar.gz hashes to", refused.Message, StringComparison.Ordinal);

        var archive = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        var europe = (byte[])archive["europe"].Clone();
        var at = Array.IndexOf(europe, (byte)'R', europe.Length / 2);
        europe[at + 1] = europe[at + 1] == (byte)'u' ? (byte)'v' : (byte)'u';
        archive["europe"] = europe;

        var violations = pin.Violations(archive).ToArray();
        Assert.Single(violations);
        Assert.Contains("`europe` hashes to", violations[0], StringComparison.Ordinal);

        archive.Remove("europe");
        Assert.Contains(pin.Violations(archive), static violation => violation.Contains("no member `europe`", StringComparison.Ordinal));
    }

    /// <summary>The version clause: a release stating another version is refused, whatever its hashes.</summary>
    [Fact]
    public void N29_Rejects_Another_Release()
    {
        var pin = TzdbPin.Load();
        var archive = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal)
        {
            ["version"] = "2026d\n"u8.ToArray(),
        };

        Assert.Contains(pin.Violations(archive), static violation => violation.Contains("states version `2026d`", StringComparison.Ordinal));
        Assert.Contains(
            TzdbPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, TzdbPin.PinPath)).Replace("version 2026e", "version 2026d", StringComparison.Ordinal))
                .TarballViolations(pin.ReadTarball()),
            static violation => violation.Contains("names version `2026d`", StringComparison.Ordinal));
    }

    /// <summary>The gate, and in write mode the generator.</summary>
    [Fact]
    public void N30_The_Generated_Tables_Are_What_The_Pinned_Archives_Generate()
    {
        var artefact = TzdbTableGenerator.Current;

        foreach (var table in new[] { "TimeZones", "TimeZoneIds", "TimeZoneRegions" })
        {
            Assert.Contains($"internal static ReadOnlySpan<byte> {table} =>", artefact.Desired, StringComparison.Ordinal);
        }

        if (TzdbTableGenerator.WriteRequested)
        {
            File.WriteAllText(
                Path.Combine(ComponentGraph.Root, TzdbTableGenerator.OutputPath.Replace('/', Path.DirectorySeparatorChar)),
                artefact.Desired,
                AssuranceSources.Utf8NoBom);
            return;
        }

        Assert.True(
            artefact.IsCurrent,
            $"{TzdbTableGenerator.OutputPath} is not what the tzdb table generator writes; change the generator or a " +
            $"pinned archive and run with {TzdbTableGenerator.WriteVariable}=1, never edit the file");
    }

    /// <summary>Generation twice yields byte-identical text, and a hand edit is reported.</summary>
    [Fact]
    public void N30_Generates_The_Same_Bytes_And_Rejects_A_Hand_Edit()
    {
        var tzdb = TzdbPin.Load();
        var cldr = CldrPin.Load();

        var first = TzdbTableGenerator.Generate(tzdb, tzdb.ReadArchive(), cldr, cldr.ReadArchive());
        var second = TzdbTableGenerator.Generate(tzdb, tzdb.ReadArchive(), cldr, cldr.ReadArchive());
        Assert.Equal(first, second);

        var artefact = TzdbTableGenerator.Current;
        Assert.False((artefact with { Current = artefact.Desired.Replace("Europe/Vienna", "Europe/Wien", StringComparison.Ordinal) }).IsCurrent);
    }

    /// <summary>
    /// The compiled offsets of a few zones whose history is well known: New York's daylight saving
    /// time in 2026 and its rule after the listed years, Kolkata's half hour, Lord Howe's half-hour
    /// daylight saving, Kiritimati's skipped day, and the local mean time before standard time.
    /// </summary>
    [Fact]
    public void N30_The_Compiler_Reads_Known_Histories()
    {
        var source = TzdbCompiler.Parse(TzdbPin.Load().ReadArchive());
        var zones = TzdbTableGenerator.Zones(source).ToDictionary(static zone => zone.Name, static zone => zone.Zone, StringComparer.Ordinal);

        static long Utc(int year, int month, int day, int hour, int minute = 0) =>
            (TzdbCompiler.DaysFromCivil(year, month, day) * 86400) + (hour * 3600) + (minute * 60);

        var newYork = zones["America/New_York"];
        Assert.Equal(-5 * 3600, TzdbCompiler.OffsetAt(newYork, Utc(2026, 3, 8, 6, 59)));
        Assert.Equal(-4 * 3600, TzdbCompiler.OffsetAt(newYork, Utc(2026, 3, 8, 7)));
        Assert.Equal(-4 * 3600, TzdbCompiler.OffsetAt(newYork, Utc(2026, 11, 1, 5, 59)));
        Assert.Equal(-5 * 3600, TzdbCompiler.OffsetAt(newYork, Utc(2026, 11, 1, 6)));
        Assert.Equal(-4 * 3600, TzdbCompiler.OffsetAt(newYork, Utc(2400, 7, 1, 0)));
        Assert.Equal(-((4 * 3600) + (56 * 60) + 2), newYork.InitialOffset);
        Assert.NotNull(newYork.Final);

        Assert.Equal((5 * 3600) + 1800, TzdbCompiler.OffsetAt(zones["Asia/Kolkata"], Utc(2026, 1, 1, 0)));
        Assert.Equal(11 * 3600, TzdbCompiler.OffsetAt(zones["Australia/Lord_Howe"], Utc(2026, 1, 1, 0)));
        Assert.Equal((10 * 3600) + 1800, TzdbCompiler.OffsetAt(zones["Australia/Lord_Howe"], Utc(2026, 7, 1, 0)));
        Assert.Equal(-10 * 3600, TzdbCompiler.OffsetAt(zones["Pacific/Kiritimati"], Utc(1994, 12, 30, 0)));
        Assert.Equal(14 * 3600, TzdbCompiler.OffsetAt(zones["Pacific/Kiritimati"], Utc(1995, 1, 2, 0)));
        Assert.Equal(0, TzdbCompiler.OffsetAt(zones["Etc/UTC"], Utc(2026, 1, 1, 0)));
        Assert.Null(zones["Asia/Kolkata"].Final);
    }
}
