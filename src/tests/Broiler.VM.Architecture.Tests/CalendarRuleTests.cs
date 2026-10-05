namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N31: the archived calendar crates are the ones <c>calendars.pin</c> describes. Rule N32: the
/// calendar table is what <see cref="CalendarTableGenerator"/> writes from them.
/// </summary>
/// <remarks>
/// <para>
/// <b>N29 and N30's statements over a fourth archive</b> (decision JSD-0056). The archive is two
/// crates, so the hash clause is over each crate's bytes and then over each member the generator
/// reads; the version clause is each crate's own <c>Cargo.toml</c>.
/// </para>
/// <para>
/// <b>The parse is checked beside its table.</b> A handful of years whose dates are well known -
/// Chinese and Korean new years, the year 2012 whose leap month the two calendars place differently,
/// an Umm al-Qura new year and the first Persian correction - are read back from the parsed tables.
/// </para>
/// </remarks>
public sealed class CalendarRuleTests
{
    /// <summary>The gate: the archives are the ones the pin describes, and nothing else is beside them.</summary>
    [Fact]
    public void N31_The_Archives_Are_The_Ones_The_Pin_Describes()
    {
        var pin = CalendarPin.Load();
        var members = pin.ReadArchive();

        Assert.Empty(pin.ArchiveViolations(pin.Archives.ToDictionary(static archive => archive.Name, archive => pin.ReadArchiveBytes(archive.Name), StringComparer.Ordinal)));
        Assert.Empty(pin.Violations(members));
        Assert.Equal("2 byte-identical", pin.Values["retrievals"]);
        Assert.Equal("Unicode-3.0", pin.Values["licence icu_calendar-2.3.0.crate"]);
        Assert.Equal("Apache-2.0", pin.Values["licence calendrical_calculations-0.2.4.crate"]);
        Assert.Equal(9, pin.Members.Count);
        Assert.Equal(pin.Members.Select(static member => member.Name).Order(StringComparer.Ordinal), members.Keys.Order(StringComparer.Ordinal));

        foreach (var file in new[] { CalendarTableGenerator.China, CalendarTableGenerator.Korea, CalendarTableGenerator.Qing, CalendarTableGenerator.UmmAlQura, CalendarTableGenerator.Persian })
        {
            Assert.Contains(pin.Members, member => member.Name == file);
        }

        Assert.StartsWith("UNICODE LICENSE V3", CalendarPin.Decode(members["icu_calendar-2.3.0/LICENSE"]), StringComparison.Ordinal);
        Assert.Contains("Apache License", CalendarPin.Decode(members["calendrical_calculations-0.2.4/LICENSE"]), StringComparison.Ordinal);

        // NOTHING IS BESIDE THE PIN but the two crates and the README.
        var directory = Path.Combine(ComponentGraph.Root, CalendarPin.PinDirectory);
        Assert.Equal(
            new[] { "README.md", "calendars.pin" }.Concat(pin.Archives.Select(static archive => archive.Name)).Order(StringComparer.Ordinal),
            Directory.EnumerateFiles(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal));
    }

    /// <summary>The hash clauses: one changed byte of a crate, or of a member, is refused.</summary>
    [Fact]
    public void N31_Rejects_A_Changed_Byte()
    {
        var pin = CalendarPin.Load();
        var crates = pin.Archives.ToDictionary(static archive => archive.Name, archive => pin.ReadArchiveBytes(archive.Name), StringComparer.Ordinal);
        var crate = crates["icu_calendar-2.3.0.crate"];
        crate[crate.Length / 2] ^= 1;

        var refused = Assert.Throws<InvalidDataException>(() => pin.Extract(crates));
        Assert.Contains("nothing is generated from them", refused.Message, StringComparison.Ordinal);
        Assert.Contains("icu_calendar-2.3.0.crate hashes to", refused.Message, StringComparison.Ordinal);

        var members = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        var china = (byte[])members[CalendarTableGenerator.China].Clone();
        var at = Array.IndexOf(china, (byte)'l', china.Length / 2);
        china[at] = (byte)'s';
        members[CalendarTableGenerator.China] = china;

        var violations = pin.Violations(members).ToArray();
        Assert.Single(violations);
        Assert.Contains("china_data.rs` hashes to", violations[0], StringComparison.Ordinal);

        members.Remove(CalendarTableGenerator.China);
        Assert.Contains(pin.Violations(members), static violation => violation.Contains("no member `icu_calendar-2.3.0/src/cal/east_asian_traditional/china_data.rs`", StringComparison.Ordinal));
    }

    /// <summary>The version clause: a crate stating another version is refused, whatever its hashes.</summary>
    [Fact]
    public void N31_Rejects_Another_Release()
    {
        var pin = CalendarPin.Load();
        var members = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        var manifest = CalendarPin.Decode(members["icu_calendar-2.3.0/Cargo.toml"]);
        members["icu_calendar-2.3.0/Cargo.toml"] = System.Text.Encoding.UTF8.GetBytes(manifest.Replace("version = \"2.3.0\"", "version = \"2.2.1\"", StringComparison.Ordinal));

        Assert.Contains(pin.Violations(members), static violation => violation.Contains("states the package `icu_calendar` 2.2.1", StringComparison.Ordinal));
        Assert.Contains(
            CalendarPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, CalendarPin.PinPath)).Replace("archive icu_calendar-2.3.0.crate", "archive icu_calendar-2.2.1.crate", StringComparison.Ordinal))
                .ArchiveViolations(new Dictionary<string, byte[]>(StringComparer.Ordinal)),
            static violation => violation.Contains("`icu_calendar-2.2.1.crate`, which is not a pinned crate version", StringComparison.Ordinal));
    }

    /// <summary>The gate, and in write mode the generator.</summary>
    [Fact]
    public void N32_The_Generated_Table_Is_What_The_Pinned_Archives_Generate()
    {
        var artefact = CalendarTableGenerator.Current;
        Assert.Contains("internal static ReadOnlySpan<byte> Calendars =>", artefact.Desired, StringComparison.Ordinal);

        if (CalendarTableGenerator.WriteRequested)
        {
            File.WriteAllText(
                Path.Combine(ComponentGraph.Root, CalendarTableGenerator.OutputPath.Replace('/', Path.DirectorySeparatorChar)),
                artefact.Desired,
                AssuranceSources.Utf8NoBom);
            return;
        }

        Assert.True(
            artefact.IsCurrent,
            $"{CalendarTableGenerator.OutputPath} is not what the calendar table generator writes; change the generator or a " +
            $"pinned archive and run with {CalendarTableGenerator.WriteVariable}=1, never edit the file");
    }

    /// <summary>Generation twice yields byte-identical text, and a hand edit is reported.</summary>
    [Fact]
    public void N32_Generates_The_Same_Bytes_And_Rejects_A_Hand_Edit()
    {
        var pin = CalendarPin.Load();

        var first = CalendarTableGenerator.Generate(pin, pin.ReadArchive());
        var second = CalendarTableGenerator.Generate(pin, pin.ReadArchive());
        Assert.Equal(first, second);

        var artefact = CalendarTableGenerator.Current;
        Assert.False((artefact with { Current = artefact.Desired.Replace("        ", "       ", StringComparison.Ordinal) }).IsCurrent);
    }

    /// <summary>
    /// Years whose dates are well known, read back from the parsed tables: Chinese New Year 2024 on
    /// 10 February, the year 2012 whose leap month China places after the fourth month and Korea
    /// after the third, the Qing year 1900's leap eighth month, 1 Muharram 1445 on 19 July 2023, and
    /// the first year the Persian 33-year rule is corrected.
    /// </summary>
    [Fact]
    public void N32_The_Parse_Reads_Known_Years()
    {
        var members = CalendarPin.Load().ReadArchive();
        var china = CalendarTableGenerator.EastAsianYears(CalendarPin.Decode(members[CalendarTableGenerator.China]), "china");
        var korea = CalendarTableGenerator.EastAsianYears(CalendarPin.Decode(members[CalendarTableGenerator.Korea]), "korea");
        var qing = CalendarTableGenerator.EastAsianYears(CalendarPin.Decode(members[CalendarTableGenerator.Qing]), "qing");
        var ummAlQura = CalendarTableGenerator.HijriYears(CalendarPin.Decode(members[CalendarTableGenerator.UmmAlQura]));

        Assert.Equal((1912, 2102), (china[0].Year, china[^1].Year));
        Assert.Equal((1912, 2102), (korea[0].Year, korea[^1].Year));
        Assert.Equal((1900, 1911), (qing[0].Year, qing[^1].Year));
        Assert.Equal((1300, 1600), (ummAlQura[0].Year, ummAlQura[^1].Year));

        Assert.Equal(TzdbCompiler.DaysFromCivil(2024, 2, 10), china.Single(static year => year.Year == 2024).NewYear);
        Assert.Equal(5, china.Single(static year => year.Year == 2012).LeapMonth);
        Assert.Equal(4, korea.Single(static year => year.Year == 2012).LeapMonth);
        Assert.Equal(9, qing[0].LeapMonth);
        Assert.Equal(TzdbCompiler.DaysFromCivil(2023, 7, 19), ummAlQura.Single(static year => year.Year == 1445).NewYear);
        Assert.Equal(1502, CalendarTableGenerator.PersianCorrections(CalendarPin.Decode(members[CalendarTableGenerator.Persian]))[0]);
    }
}
