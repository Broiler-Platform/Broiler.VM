using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N28: the internationalization tables are what <see cref="CldrTableGenerator"/> writes from
/// the pinned CLDR and UCD archives.
/// </summary>
/// <remarks>
/// <para>
/// <b>N22's statement over a second archive.</b> A table is numbers nobody reads, so the statement
/// worth having is the mechanical one: these bytes are what this generator writes from those files,
/// which rules N27 and N22 hold to their pins. The witnesses are made in memory against the real
/// output, as N22's hand-edit witness is.
/// </para>
/// <para>
/// <b>The size is budgeted.</b> JSD-0027's owner decision (c) asked for a size budget from a measured
/// prototype, and JSD-0043's provisional 512 KiB bound held the data until the owner set one: 768 KiB,
/// on 2026-10-05, when Intl.DisplayNames needed more than the bound left. Growth past it is a
/// decision rather than a drift.
/// </para>
/// </remarks>
public sealed class CldrTablesRuleTests
{
    /// <summary>
    /// The budget on the generated data, in bytes: 768 KiB, which the repository owner set on
    /// 2026-10-05 as JSD-0027 decision (c), replacing JSD-0043's provisional 512 KiB bound.
    /// </summary>
    internal const int Budget = 768 * 1024;

    /// <summary>The gate, and in write mode the generator.</summary>
    [Fact]
    public void N28_The_Generated_Tables_Are_What_The_Pinned_Archives_Generate()
    {
        var artefact = CldrTableGenerator.Current;

        Assert.True(artefact.Desired.Length > 100_000, "the generated tables are not the size of the archive's data");

        foreach (var table in new[] { "LikelySubtags", "Aliases", "Extensions", "Locales", "SoftDotted", "NumberLocales", "Currencies", "CurrencyDigits", "NumberingSystems", "Plurals", "PluralRanges", "Ordinals", "ListPatterns", "RelativeTimes", "DisplayNames", "SegmentBreakValues", "SegmentBreaks", "Units", "DateLocales", "TimeData", "DayPeriods", "WeekData", "Scripts", "CollationRoot", "CollationTailorings" })
        {
            Assert.Contains($"internal static ReadOnlySpan<byte> {table} =>", artefact.Desired, StringComparison.Ordinal);
        }

        if (CldrTableGenerator.WriteRequested)
        {
            File.WriteAllText(
                Path.Combine(ComponentGraph.Root, CldrTableGenerator.OutputPath.Replace('/', Path.DirectorySeparatorChar)),
                artefact.Desired,
                AssuranceSources.Utf8NoBom);
            return;
        }

        Assert.True(
            artefact.IsCurrent,
            $"{CldrTableGenerator.OutputPath} is not what the CLDR table generator writes; change the generator or a " +
            $"pinned archive and run with {CldrTableGenerator.WriteVariable}=1, never edit the file");
    }

    /// <summary>Generation twice yields byte-identical text, so nothing in it reads a clock or an order the archive does not fix.</summary>
    [Fact]
    public void N28_Generating_Twice_Yields_The_Same_Bytes()
    {
        var pin = CldrPin.Load();
        var unicode = UnicodePin.Load();

        var first = CldrTableGenerator.Generate(pin, pin.ReadArchive(), unicode, unicode.ReadArchive());
        var second = CldrTableGenerator.Generate(pin, pin.ReadArchive(), unicode, unicode.ReadArchive());

        Assert.Equal(first, second);
    }

    /// <summary>A hand edit to the generated file is reported; and a changed archive byte generates nothing.</summary>
    [Fact]
    public void N28_Rejects_A_Hand_Edit_And_A_Changed_Archive()
    {
        var artefact = CldrTableGenerator.Current;
        var edited = artefact with { Current = artefact.Desired.Replace("de-DE", "de-AT", StringComparison.Ordinal) };

        Assert.False(edited.IsCurrent);

        var pin = CldrPin.Load();
        var unicode = UnicodePin.Load();
        var archive = new Dictionary<string, byte[]>(pin.ReadArchive(), StringComparer.Ordinal);
        const string Name = "cldr-48.2.0/json/cldr-core/supplemental/likelySubtags.json";
        var changed = (byte[])archive[Name].Clone();

        // A LETTER INSIDE A VALUE, so the file is still JSON and only the digest tells.
        changed[Array.IndexOf(changed, (byte)'L', Array.IndexOf(changed, (byte)'"', changed.Length / 2))] = (byte)'M';
        archive[Name] = changed;

        var refused = Assert.Throws<InvalidDataException>(() =>
            CldrTableGenerator.Generate(pin, archive, unicode, unicode.ReadArchive()));

        Assert.Contains("nothing is generated from it", refused.Message, StringComparison.Ordinal);
    }

    /// <summary>The notices carry the archived CLDR licence text, since the tables derived under it ship.</summary>
    [Fact]
    public void N28_The_Notices_Carry_The_Archived_Cldr_Licence()
    {
        var pin = CldrPin.Load();
        var notices = File.ReadAllText(Path.Combine(ComponentGraph.Root, "THIRD_PARTY_NOTICES.md"));
        var licence = CldrPin.Decode(pin.ReadArchive()["cldr-LICENSE.txt"]).TrimEnd('\n');

        Assert.StartsWith("UNICODE LICENSE V3", licence, StringComparison.Ordinal);
        Assert.Contains("Copyright \u00a9 2004-2026 Unicode, Inc.", licence, StringComparison.Ordinal);
        Assert.Contains(licence, notices, StringComparison.Ordinal);
        Assert.Contains(CldrTableGenerator.OutputPath, notices, StringComparison.Ordinal);
    }

    /// <summary>The table data, counted from the generated literals, stays under the owner's budget.</summary>
    [Fact]
    public void N28_The_Table_Data_Stays_Under_The_Budget()
    {
        var text = CldrTableGenerator.Current.Desired;

        var bytes = Regex.Matches(text, @"new byte\[\]\s*\{(?<body>[^}]*)\}")
            .Sum(static match => match.Groups["body"].Value.Count(static c => c == ','));

        // A RAW LITERAL'S INDENTATION IS NOT IN ITS BYTES: the compiler strips the closing
        // delimiter's whitespace from every line, so a line counts its content and its newline.
        var textBytes = Regex.Matches(text, "\"\"\"\\n(?<body>.*?)\\n\\s*\"\"\"u8", RegexOptions.Singleline)
            .Sum(static match => match.Groups["body"].Value.Split('\n').Sum(static line => line.TrimStart(' ').Length + 1) - 1);

        Assert.True(bytes > 100_000, $"the binary tables hold {bytes} bytes, fewer than the root collation needs");
        Assert.True(bytes + textBytes < Budget, $"the tables hold {bytes + textBytes} bytes, past the budget of {Budget}");
    }
}
