using Broiler.ECMAScript.Generation.Time;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N31: the archived calendar crates are the ones <c>calendars.pin</c> describes. Rule N32: the
/// calendar table is what the time data home's <c>CalendarTableGenerator</c> writes from them.
/// </summary>
/// <remarks>
/// <para>
/// <b>N29 and N30's statements over a fourth archive</b> (decision JSD-0056). The archive is two
/// crates, so the hash clause is over each crate's bytes and then over each member the generator
/// reads; the version clause is each crate's own <c>Cargo.toml</c>.
/// </para>
/// <para>
/// <b>The generator is the home's since SJL-8.5</b> (JSD-0076, decision SJL-D-6 (c)), as rule N30's is:
/// the pin reader and the calendar table generator moved to the Broiler.JS repository and come back as
/// the test-only package <c>Broiler.ECMAScript.Generation</c>, and the pin, the crates and the generated
/// file stay this repository's (<see cref="TimeTableEmbedding"/>).
/// </para>
/// <para>
/// <b>The table is checked beside its regeneration.</b> The years it covers - China's and Korea's 1912
/// to 2102, the Qing years 1900 to 1911, Umm al-Qura 1300 to 1600 AH and the Persian corrections from
/// 1502 AP - are read from its section headers. Until SJL-8.5 this test also read a handful of known
/// dates from the generator's parse, whose readers are internal to the home's package now; the home's
/// own tests hold those dates (its <c>CalendarTableTests</c>), and rule N32 holds the bytes they are in.
/// </para>
/// </remarks>
public sealed class CalendarRuleTests
{
    /// <summary>The gate: the archives are the ones the pin describes, and nothing else is beside them.</summary>
    [Fact]
    public void N31_The_Archives_Are_The_Ones_The_Pin_Describes()
    {
        var pin = TimeTableEmbedding.LoadCalendarPin();
        var members = TimeTableEmbedding.ReadCalendarArchive(pin);

        Assert.Empty(pin.ArchiveViolations(TimeTableEmbedding.ReadCalendarCrates(pin)));
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
        var directory = Path.Combine(ComponentGraph.Root, TimeTableEmbedding.CalendarPinDirectory);
        Assert.Equal(
            new[] { "README.md", "calendars.pin" }.Concat(pin.Archives.Select(static archive => archive.Name)).Order(StringComparer.Ordinal),
            Directory.EnumerateFiles(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal));
    }

    /// <summary>The hash clauses: one changed byte of a crate, or of a member, is refused.</summary>
    [Fact]
    public void N31_Rejects_A_Changed_Byte()
    {
        var pin = TimeTableEmbedding.LoadCalendarPin();
        var crates = new Dictionary<string, byte[]>(TimeTableEmbedding.ReadCalendarCrates(pin), StringComparer.Ordinal);
        var crate = crates["icu_calendar-2.3.0.crate"];
        crate[crate.Length / 2] ^= 1;

        var refused = Assert.Throws<InvalidDataException>(() => pin.Extract(crates));
        Assert.Contains("nothing is generated from them", refused.Message, StringComparison.Ordinal);
        Assert.Contains("icu_calendar-2.3.0.crate hashes to", refused.Message, StringComparison.Ordinal);

        var members = new Dictionary<string, byte[]>(TimeTableEmbedding.ReadCalendarArchive(pin), StringComparer.Ordinal);
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
        var pin = TimeTableEmbedding.LoadCalendarPin();
        var members = new Dictionary<string, byte[]>(TimeTableEmbedding.ReadCalendarArchive(pin), StringComparer.Ordinal);
        var manifest = CalendarPin.Decode(members["icu_calendar-2.3.0/Cargo.toml"]);
        members["icu_calendar-2.3.0/Cargo.toml"] = System.Text.Encoding.UTF8.GetBytes(manifest.Replace("version = \"2.3.0\"", "version = \"2.2.1\"", StringComparison.Ordinal));

        Assert.Contains(pin.Violations(members), static violation => violation.Contains("states the package `icu_calendar` 2.2.1", StringComparison.Ordinal));
        Assert.Contains(
            CalendarPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, TimeTableEmbedding.CalendarPinPath)).Replace("archive icu_calendar-2.3.0.crate", "archive icu_calendar-2.2.1.crate", StringComparison.Ordinal))
                .ArchiveViolations(new Dictionary<string, byte[]>(StringComparer.Ordinal)),
            static violation => violation.Contains("`icu_calendar-2.2.1.crate`, which is not a pinned crate version", StringComparison.Ordinal));
    }

    /// <summary>The gate, and in write mode the generator.</summary>
    [Fact]
    public void N32_The_Generated_Table_Is_What_The_Pinned_Archives_Generate()
    {
        var file = TimeTableEmbedding.Calendars;
        var artefact = TimeTableEmbedding.CalendarsCurrent;
        Assert.Contains("internal static ReadOnlySpan<byte> Calendars =>", artefact.Desired, StringComparison.Ordinal);

        if (file.WriteRequested)
        {
            File.WriteAllText(
                Path.Combine(ComponentGraph.Root, file.RelativePath.Replace('/', Path.DirectorySeparatorChar)),
                artefact.Desired,
                AssuranceSources.Utf8NoBom);
            return;
        }

        Assert.True(
            artefact.IsCurrent,
            $"{file.RelativePath} is not what the time data home's calendar table generator writes from this repository's pin; " +
            $"change the generator in its home, or a pinned archive, and run with {file.WriteVariable}=1, never edit the file");
    }

    /// <summary>Generation twice yields byte-identical text, and a hand edit or a changed byte is reported.</summary>
    [Fact]
    public void N32_Generates_The_Same_Bytes_And_Rejects_A_Hand_Edit()
    {
        var first = TimeTableEmbedding.CalendarSource(TimeTableEmbedding.GenerateCalendars());
        var second = TimeTableEmbedding.CalendarSource(TimeTableEmbedding.GenerateCalendars());
        Assert.Equal(first, second);

        var artefact = TimeTableEmbedding.CalendarsCurrent;
        Assert.False((artefact with { Current = artefact.Desired.Replace("        ", "       ", StringComparison.Ordinal) }).IsCurrent);

        var at = artefact.Desired.IndexOf("new byte[]\n    {\n        ", StringComparison.Ordinal) + "new byte[]\n    {\n        ".Length;
        var digit = artefact.Desired[at];
        var changed = artefact.Desired[..at] + (digit == '9' ? '8' : (char)(digit + 1)) + artefact.Desired[(at + 1)..];
        Assert.False((artefact with { Current = changed }).IsCurrent);
    }

    /// <summary>
    /// The years the table covers, read from its section headers in the layout the shared library's
    /// <c>CalendarTables</c> reads: four sections of a first year and a count, little-endian 16-bit, and a
    /// record of three bytes (the East Asian years) or two (Umm al-Qura) per year, then a count of the
    /// Persian corrections and each as a 16-bit year.
    /// </summary>
    [Fact]
    public void N32_The_Table_Holds_The_Published_Years()
    {
        var table = TimeTableEmbedding.GenerateCalendars().Calendars.Span;
        var at = 0;

        int U16(ReadOnlySpan<byte> bytes)
        {
            var value = bytes[at] | (bytes[at + 1] << 8);
            at += 2;
            return value;
        }

        (int First, int Last) Section(ReadOnlySpan<byte> bytes, int size)
        {
            var first = U16(bytes);
            var count = U16(bytes);
            at += count * size;
            return (first, first + count - 1);
        }

        Assert.Equal((1912, 2102), Section(table, 3));
        Assert.Equal((1912, 2102), Section(table, 3));
        Assert.Equal((1900, 1911), Section(table, 3));
        Assert.Equal((1300, 1600), Section(table, 2));

        var corrections = U16(table);
        Assert.Equal(78, corrections);
        Assert.Equal(1502, U16(table));
        Assert.Equal(table.Length, at + ((corrections - 1) * 2));
    }
}
