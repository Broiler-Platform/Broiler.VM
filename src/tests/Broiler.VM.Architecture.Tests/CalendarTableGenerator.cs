using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Writes the calendar table of <c>Broiler.VM.Profile.JavaScript.Intl</c> from the crate members
/// <c>calendars.pin</c> names (decision JSD-0056).
/// </summary>
/// <remarks>
/// <para>
/// <b>One function from verified bytes to one file's text</b>, as <see cref="TzdbTableGenerator"/>
/// is: rule N32 calls it to compare the checked-in file and, in write mode, to write it.
/// </para>
/// <para>
/// <b>It reads Rust source as data, and nothing else of it.</b> ICU4X states each published year as
/// one constructor call - <c>PackedEastAsianTraditionalYearData::new(year, [month lengths], leap
/// month, gregorian(new year))</c> and <c>PackedHijriYearData::try_new(year, [month lengths],
/// gregorian(new year))</c> - and the Persian corrections as one array literal. Each table's years
/// must run on from its <c>STARTING_YEAR</c> without a gap, and each new year must fit the layout
/// <c>JsCalendarData</c> in the profile reads, or nothing is written.
/// </para>
/// </remarks>
internal static partial class CalendarTableGenerator
{
    /// <summary>The file this writes, relative to the repository root.</summary>
    internal const string OutputPath = "src/Broiler.VM.Profile.JavaScript.Intl/JsCalendarTables.g.cs";

    /// <summary>The environment variable that turns rule N32's gate into the generator's write.</summary>
    internal const string WriteVariable = "BROILER_CALENDARS_WRITE";

    /// <summary>The per-member exemption every generated table states.</summary>
    internal const string ExemptReason =
        "calendar year data written by CalendarTableGenerator from the crate members calendars.pin names, compared byte for byte by rule N32";

    internal const string China = "icu_calendar-2.3.0/src/cal/east_asian_traditional/china_data.rs";
    internal const string Korea = "icu_calendar-2.3.0/src/cal/east_asian_traditional/korea_data.rs";
    internal const string Qing = "icu_calendar-2.3.0/src/cal/east_asian_traditional/qing_data.rs";
    internal const string UmmAlQura = "icu_calendar-2.3.0/src/cal/hijri/ummalqura_data.rs";
    internal const string Persian = "calendrical_calculations-0.2.4/src/persian.rs";

    /// <summary>R.D. 227015, the civil Hijri epoch, in days since 1970-01-01.</summary>
    private const long CivilHijriEpoch = 227015 - 719163;

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The generated file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact Current => current.Value;

    private static readonly Lazy<UnicodeArtefact> current = new(static () =>
    {
        var pin = CalendarPin.Load();
        return Artefact(Generate(pin, pin.ReadArchive()));
    });

    /// <summary>One published East Asian year: its related ISO year, month lengths, leap month and new year.</summary>
    internal sealed record EastAsianYear(int Year, bool[] Long, int LeapMonth, long NewYear);

    /// <summary>One published Umm al-Qura year: its year, month lengths and new year.</summary>
    internal sealed record HijriYear(int Year, bool[] Long, long NewYear);

    /// <summary>The generated file's text, from the verified members.</summary>
    internal static string Generate(CalendarPin pin, IReadOnlyDictionary<string, byte[]> members)
    {
        pin.Verified(members);

        var bytes = new List<byte>();
        EastAsianSection(bytes, EastAsianYears(CalendarPin.Decode(members[China]), China));
        EastAsianSection(bytes, EastAsianYears(CalendarPin.Decode(members[Korea]), Korea));
        EastAsianSection(bytes, EastAsianYears(CalendarPin.Decode(members[Qing]), Qing));
        UmmAlQuraSection(bytes, HijriYears(CalendarPin.Decode(members[UmmAlQura])));

        var corrections = PersianCorrections(CalendarPin.Decode(members[Persian]));
        U16(bytes, corrections.Count);

        foreach (var year in corrections)
        {
            U16(bytes, year);
        }

        var text = new StringBuilder();
        text.Append(Header).Append('\n');
        ByteTable(text, "Calendars", "The Chinese, Korean and Qing years, the Umm al-Qura years, and the Persian calendar's non-leap corrections, in the layout JsCalendarData reads.", [.. bytes]);

        while (text.Length > 2 && text[^1] == '\n' && text[^2] == '\n')
        {
            text.Length--;
        }

        text.Append("}\n");
        return text.ToString();
    }

    /// <summary>The years one East Asian table states, checked to run on from its starting year.</summary>
    internal static IReadOnlyList<EastAsianYear> EastAsianYears(string source, string file)
    {
        var start = StartingYear(source, file);
        var years = new List<EastAsianYear>();

        foreach (Match match in EastAsianRow().Matches(source))
        {
            var year = Int(match.Groups[1].Value);
            var lengths = Lengths(match.Groups[2].Value, 13, file);
            var leap = match.Groups[4].Success ? Int(match.Groups[4].Value) : 0;
            var newYear = TzdbCompiler.DaysFromCivil(Int(match.Groups[5].Value), Int(match.Groups[6].Value), Int(match.Groups[7].Value));

            if (year != start + years.Count)
            {
                throw new InvalidDataException($"{file}: the year {year} does not follow {start + years.Count - 1}");
            }

            if (leap is 1 or > 13 || (leap == 0 && lengths[12]))
            {
                throw new InvalidDataException($"{file}: the year {year} has a leap month {leap} the layout cannot state");
            }

            years.Add(new EastAsianYear(year, lengths, leap, newYear));
        }

        if (years.Count == 0)
        {
            throw new InvalidDataException($"{file} states no years");
        }

        return years;
    }

    /// <summary>The Umm al-Qura years, checked to run on from the table's starting year.</summary>
    internal static IReadOnlyList<HijriYear> HijriYears(string source)
    {
        var start = StartingYear(source, UmmAlQura);
        var years = new List<HijriYear>();

        foreach (Match match in HijriRow().Matches(source))
        {
            var year = Int(match.Groups[1].Value);

            if (year != start + years.Count)
            {
                throw new InvalidDataException($"{UmmAlQura}: the year {year} does not follow {start + years.Count - 1}");
            }

            years.Add(new HijriYear(
                year,
                Lengths(match.Groups[2].Value, 12, UmmAlQura),
                TzdbCompiler.DaysFromCivil(Int(match.Groups[3].Value), Int(match.Groups[4].Value), Int(match.Groups[5].Value))));
        }

        if (years.Count == 0)
        {
            throw new InvalidDataException($"{UmmAlQura} states no years");
        }

        return years;
    }

    /// <summary>The Persian years the 33-year rule makes leap and are not, ascending.</summary>
    internal static IReadOnlyList<int> PersianCorrections(string source)
    {
        var match = PersianTable().Match(source);

        if (!match.Success)
        {
            throw new InvalidDataException($"{Persian} has no NON_LEAP_CORRECTION array");
        }

        var years = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Int).ToList();

        if (years.Count != Int(match.Groups[1].Value) || years.Zip(years.Skip(1)).Any(static pair => pair.First >= pair.Second))
        {
            throw new InvalidDataException($"{Persian}: NON_LEAP_CORRECTION is not {match.Groups[1].Value} ascending years");
        }

        return years;
    }

    private static void EastAsianSection(List<byte> bytes, IReadOnlyList<EastAsianYear> years)
    {
        U16(bytes, years[0].Year);
        U16(bytes, years.Count);

        foreach (var year in years)
        {
            var offset = year.NewYear - TzdbCompiler.DaysFromCivil(year.Year, 1, 19);

            if (offset is < 0 or > 63)
            {
                throw new InvalidDataException($"the new year of {year.Year} is {offset} days from 19 January, outside the layout");
            }

            var packed = 0;

            for (var month = 0; month < 13; month++)
            {
                packed |= year.Long[month] ? 1 << month : 0;
            }

            packed |= year.LeapMonth << 13;
            packed |= (int)offset << 17;
            bytes.Add((byte)packed);
            bytes.Add((byte)(packed >> 8));
            bytes.Add((byte)(packed >> 16));
        }
    }

    private static void UmmAlQuraSection(List<byte> bytes, IReadOnlyList<HijriYear> years)
    {
        U16(bytes, years[0].Year);
        U16(bytes, years.Count);

        foreach (var year in years)
        {
            var tabular = CivilHijriEpoch + ((year.Year - 1L) * 354) + Math.Floor((3 + (11.0 * year.Year)) / 30);
            var offset = year.NewYear - (long)tabular;

            if (offset is < -5 or > 5)
            {
                throw new InvalidDataException($"the Umm al-Qura new year of {year.Year} is {offset} days from the tabular one, outside the layout");
            }

            var packed = 0;

            for (var month = 0; month < 12; month++)
            {
                packed |= year.Long[month] ? 1 << month : 0;
            }

            packed |= (int)(offset + 8) << 12;
            U16(bytes, packed);
        }
    }

    private static int StartingYear(string source, string file)
    {
        var match = StartingYearLine().Match(source);
        return match.Success ? Int(match.Groups[1].Value) : throw new InvalidDataException($"{file} states no STARTING_YEAR");
    }

    private static bool[] Lengths(string list, int count, string file)
    {
        var lengths = list.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(length => length switch
            {
                "l" => true,
                "s" => false,
                _ => throw new InvalidDataException($"{file}: `{length}` is not a month length"),
            })
            .ToArray();

        return lengths.Length == count ? lengths : throw new InvalidDataException($"{file}: a year states {lengths.Length} month lengths, not {count}");
    }

    private static void U16(List<byte> bytes, int value)
    {
        if (value is < 0 or > 0xFFFF)
        {
            throw new InvalidDataException($"{value} does not fit the table's 16 bits");
        }

        bytes.Add((byte)value);
        bytes.Add((byte)(value >> 8));
    }

    private static int Int(string text) => int.Parse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);

    [GeneratedRegex(@"PackedEastAsianTraditionalYearData::new\((\d+), \[([ls, ]+)\], (None|Some\((\d+)\)), gregorian\((\d+), (\d+), (\d+)\)\)")]
    private static partial Regex EastAsianRow();

    [GeneratedRegex(@"PackedHijriYearData::try_new\((\d+), \[([ls, ]+)\], gregorian\((\d+), (\d+), (\d+)\)\)\.unwrap\(\)")]
    private static partial Regex HijriRow();

    [GeneratedRegex(@"const NON_LEAP_CORRECTION: \[i32; (\d+)\] = \[([0-9,\s]+)\];")]
    private static partial Regex PersianTable();

    [GeneratedRegex(@"pub const STARTING_YEAR: i32 = (\d+);")]
    private static partial Regex StartingYearLine();

    internal static UnicodeArtefact Artefact(string raw)
    {
        var full = Path.Combine(ComponentGraph.Root, OutputPath.Replace('/', Path.DirectorySeparatorChar));
        var assembly = OutputPath.Split('/')[1];
        var file = new AssuranceSourceFile(full, OutputPath, assembly, raw, "\n", AssuranceSources.Parse(raw, full));

        return new UnicodeArtefact(
            OutputPath,
            File.Exists(full) ? File.ReadAllText(full) : string.Empty,
            AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file)));
    }

    private const string Header =
        """
        using System;

        namespace Broiler.VM.Profile.JavaScript.Intl;

        // Written by CalendarTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the members of
        // the icu_calendar 2.3.0 and calendrical_calculations 0.2.4 crates that
        // src/tests/calendars/pins/calendars.pin names, under decision JSD-0056. Rule N32 regenerates this
        // file and compares it byte for byte, so a change belongs in the generator or a pinned archive and
        // never here: run the architecture tests with BROILER_CALENDARS_WRITE=1.
        //
        // The years are ICU4X's, under the Unicode License v3 (SPDX Unicode-3.0), and the Persian
        // corrections calendrical_calculations', under the Apache License 2.0; THIRD_PARTY_NOTICES.md
        // carries both.

        /// <summary>The calendar tables, as the generator wrote them.</summary>
        // Broiler-AI:           Origin=Derived; Spec=JSD-0056; IP=Low; Security=Low; Resources=1; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal static class JsCalendarTables
        {
        """;

    private static void ByteTable(StringBuilder text, string name, string summary, byte[] data)
    {
        text.Append("    /// <summary>").Append(summary).Append("</summary>\n");
        text.Append("    // Broiler-AI:           EXEMPT=").Append(ExemptReason).Append('\n');
        text.Append("    // Broiler-Human:        PENDING\n");
        text.Append("    internal static ReadOnlySpan<byte> ").Append(name).Append(" => new byte[]\n    {\n");

        for (var at = 0; at < data.Length; at += 32)
        {
            text.Append("        ");

            for (var i = at; i < Math.Min(at + 32, data.Length); i++)
            {
                text.Append(data[i].ToString(CultureInfo.InvariantCulture)).Append(',');
                text.Append(i + 1 < Math.Min(at + 32, data.Length) ? " " : string.Empty);
            }

            text.Append('\n');
        }

        text.Append("    };\n\n");
    }
}
