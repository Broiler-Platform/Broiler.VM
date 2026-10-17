using Broiler.ECMAScript.Generation.Time;
using HomeCldrPin = Broiler.ECMAScript.Generation.Time.CldrPin;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// One time data file this repository embeds: where it is, the rule that holds it byte for byte, the
/// environment variable that turns that rule's gate into its write, the per-unit exemption every table
/// member states, and the source writer's options that make it this repository's file.
/// </summary>
/// <param name="RelativePath">The file, relative to the repository root.</param>
/// <param name="Rule">The rule that holds it byte for byte, N30 or N32.</param>
/// <param name="WriteVariable">The environment variable that turns that rule's gate into its write.</param>
/// <param name="ExemptReason">The per-unit exemption every table member of the file states.</param>
/// <param name="Options">What the home's source writer makes of the file around the tables.</param>
internal sealed record TimeEmbeddedFile(string RelativePath, string Rule, string WriteVariable, string ExemptReason, TableSourceOptions Options)
{
    /// <summary>Whether the rule's write was asked for.</summary>
    internal bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);
}

/// <summary>
/// The JavaScript family's embedded time zone and calendar tables, as the time data home's generator
/// writes them from this repository's pins (JSD-0076, decision SJL-D-6 (c); milestone SJL-8.5).
/// </summary>
/// <remarks>
/// <para>
/// <b>The generator is the home's, the pins and the files are this repository's.</b> Until SJL-8.5 the
/// architecture tests held their own tz compiler, table generators and pin readers (JSD-0053, JSD-0056).
/// They moved to the Broiler.JS repository, the data home, and reach this project as the test-only
/// package <c>Broiler.ECMAScript.Generation</c>: its <c>TzdbPin</c>, <c>CldrPin</c> and
/// <c>CalendarPin</c> verify every archive and member against this repository's own pins under
/// <c>src/tests/tzdb/pins/</c>, <c>src/tests/cldr/pins/</c> and <c>src/tests/calendars/pins/</c>, its
/// <c>TzdbTableGenerator</c> and <c>CalendarTableGenerator</c> write the tables, and its
/// <c>TzdbTableSource</c> and <c>CalendarTableSource</c> wrap them into C# with this repository's
/// namespace, class, comment lines and annotation lines. The package carries no pin file, so what it
/// reads is what this class hands it, and rule I4 holds those pins to the home's.
/// </para>
/// <para>
/// <b>The files are the ones this family embedded, byte for byte</b> (JSD-0076's falsification clause):
/// the options below are the header, summaries and annotation lines the moved generators wrote as
/// constants, and the writer's text is passed through <see cref="AssuranceGenerator.DesiredSource"/>,
/// which writes the assurance header and fills each fingerprint, as every generated file of this
/// repository is. Rules N30 and N32 compare the result with the checked-in file.
/// </para>
/// </remarks>
internal static class TimeTableEmbedding
{
    /// <summary>The tzdb pin, relative to the repository root.</summary>
    internal const string TzdbPinPath = "src/tests/tzdb/pins/tzdb.pin";

    /// <summary>The directory the tzdb pin's archive is in.</summary>
    internal const string TzdbPinDirectory = "src/tests/tzdb/pins";

    /// <summary>The calendar pin, relative to the repository root.</summary>
    internal const string CalendarPinPath = "src/tests/calendars/pins/calendars.pin";

    /// <summary>The directory the calendar pin's crates are in.</summary>
    internal const string CalendarPinDirectory = "src/tests/calendars/pins";

    /// <summary>The time zone tables of the Intl data assembly, held by rule N30.</summary>
    internal static readonly TimeEmbeddedFile Tzdb = new(
        "src/Broiler.VM.Profile.JavaScript.Intl/JsTzdbTables.g.cs",
        "N30",
        "BROILER_TZDB_WRITE",
        "tzdb 2026e table data written by TzdbTableGenerator from the members tzdb.pin names, compared byte for byte by rule N30",
        new TableSourceOptions("Broiler.VM.Profile.JavaScript.Intl", "JsTzdbTables")
        {
            Header =
            [
                "Written by TzdbTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the IANA Time",
                "Zone Database 2026e members that src/tests/tzdb/pins/tzdb.pin names and CLDR 48.2.0's time zone",
                "keys, under decision JSD-0053. Rule N30 regenerates this file and compares it byte for byte, so",
                "a change belongs in the generator or a pinned archive and never here: run the architecture",
                "tests with BROILER_TZDB_WRITE=1.",
                "",
                "The offsets are derived from the IANA Time Zone Database, which is in the public domain, and",
                "the primary identifiers from CLDR data under the Unicode License v3 (SPDX Unicode-3.0), whose",
                "text THIRD_PARTY_NOTICES.md carries.",
            ],
            ClassAnnotation =
            [
                "Broiler-AI:           Origin=Derived; Spec=JSD-0053; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
                "Broiler-Human:        PENDING",
            ],
            MemberAnnotation =
            [
                "Broiler-AI:           EXEMPT=tzdb 2026e table data written by TzdbTableGenerator from the members tzdb.pin names, compared byte for byte by rule N30",
                "Broiler-Human:        PENDING",
            ],
        });

    /// <summary>The calendar table of the Intl data assembly, held by rule N32.</summary>
    /// <remarks>
    /// The member's summary is the one the moved generator wrote, which names the reader the table had
    /// then, <c>JsCalendarData</c>; the file is held byte for byte, and its reader is now the shared
    /// library's <c>CalendarTables</c>, over the same layout.
    /// </remarks>
    internal static readonly TimeEmbeddedFile Calendars = new(
        "src/Broiler.VM.Profile.JavaScript.Intl/JsCalendarTables.g.cs",
        "N32",
        "BROILER_CALENDARS_WRITE",
        "calendar year data written by CalendarTableGenerator from the crate members calendars.pin names, compared byte for byte by rule N32",
        new TableSourceOptions("Broiler.VM.Profile.JavaScript.Intl", "JsCalendarTables")
        {
            Header =
            [
                "Written by CalendarTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the members of",
                "the icu_calendar 2.3.0 and calendrical_calculations 0.2.4 crates that",
                "src/tests/calendars/pins/calendars.pin names, under decision JSD-0056. Rule N32 regenerates this",
                "file and compares it byte for byte, so a change belongs in the generator or a pinned archive and",
                "never here: run the architecture tests with BROILER_CALENDARS_WRITE=1.",
                "",
                "The years are ICU4X's, under the Unicode License v3 (SPDX Unicode-3.0), and the Persian",
                "corrections calendrical_calculations', under the Apache License 2.0; THIRD_PARTY_NOTICES.md",
                "carries both.",
            ],
            ClassAnnotation =
            [
                "Broiler-AI:           Origin=Derived; Spec=JSD-0056; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
                "Broiler-Human:        PENDING",
            ],
            MemberAnnotation =
            [
                "Broiler-AI:           EXEMPT=calendar year data written by CalendarTableGenerator from the crate members calendars.pin names, compared byte for byte by rule N32",
                "Broiler-Human:        PENDING",
            ],
            MemberSummaries = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Calendars"] = "The Chinese, Korean and Qing years, the Umm al-Qura years, and the Persian calendar's non-leap corrections, in the layout JsCalendarData reads.",
            },
        });

    /// <summary>The time zone tables' file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact TzdbCurrent => tzdbCurrent.Value;

    /// <summary>The calendar table's file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact CalendarsCurrent => calendarsCurrent.Value;

    private static readonly Lazy<UnicodeArtefact> tzdbCurrent = new(static () =>
        Artefact(Tzdb, TzdbSource(GenerateTzdb())));

    private static readonly Lazy<UnicodeArtefact> calendarsCurrent = new(static () =>
        Artefact(Calendars, CalendarSource(GenerateCalendars())));

    /// <summary>This repository's tzdb pin, as the home's reader parses it.</summary>
    internal static TzdbPin LoadTzdbPin() => TzdbPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, TzdbPinPath)));

    /// <summary>The archived tarball the tzdb pin names, off disk.</summary>
    internal static byte[] ReadTzdbTarball(TzdbPin pin) =>
        File.ReadAllBytes(Path.Combine(ComponentGraph.Root, TzdbPinDirectory, pin.ArchiveName));

    /// <summary>The pinned members of the tzdb tarball, verified by the home's reader, or its refusal.</summary>
    internal static IReadOnlyDictionary<string, byte[]> ReadTzdbArchive(TzdbPin pin) => pin.Extract(ReadTzdbTarball(pin));

    /// <summary>This repository's CLDR pin, as the home's reader parses it: the generator reads one of its files.</summary>
    internal static HomeCldrPin LoadCldrPin() => HomeCldrPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, CldrPin.PinPath)));

    /// <summary>This repository's calendar pin, as the home's reader parses it.</summary>
    internal static CalendarPin LoadCalendarPin() => CalendarPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, CalendarPinPath)));

    /// <summary>The archived crates the calendar pin names, off disk, by file name.</summary>
    internal static IReadOnlyDictionary<string, byte[]> ReadCalendarCrates(CalendarPin pin) =>
        pin.Archives.ToDictionary(
            static archive => archive.Name,
            static archive => File.ReadAllBytes(Path.Combine(ComponentGraph.Root, CalendarPinDirectory, archive.Name)),
            StringComparer.Ordinal);

    /// <summary>The pinned members of the calendar crates, verified by the home's reader, or its refusal.</summary>
    internal static IReadOnlyDictionary<string, byte[]> ReadCalendarArchive(CalendarPin pin) => pin.Extract(ReadCalendarCrates(pin));

    /// <summary>The time zone tables, from this checkout's tzdb and CLDR pins, by the home's generator.</summary>
    internal static TzdbTables GenerateTzdb()
    {
        var tzdb = LoadTzdbPin();
        var cldr = LoadCldrPin();
        return TzdbTableGenerator.Generate(tzdb, ReadTzdbArchive(tzdb), cldr, CldrPin.Load().ReadArchive());
    }

    /// <summary>The calendar table, from this checkout's calendar pin, by the home's generator.</summary>
    internal static CalendarTables GenerateCalendars()
    {
        var pin = LoadCalendarPin();
        return CalendarTableGenerator.Generate(pin, ReadCalendarArchive(pin));
    }

    /// <summary>The time zone tables' file text, as the home's writer writes it with this repository's options, before the assurance pass.</summary>
    internal static string TzdbSource(TzdbTables tables) => TzdbTableSource.Write(tables, Tzdb.Options);

    /// <summary>The calendar table's file text, as the home's writer writes it with this repository's options, before the assurance pass.</summary>
    internal static string CalendarSource(CalendarTables tables) => CalendarTableSource.Write(tables, Calendars.Options);

    /// <summary>Generated text passed through the function the assurance gate uses, beside the file on disk.</summary>
    internal static UnicodeArtefact Artefact(TimeEmbeddedFile file, string raw)
    {
        var full = Path.Combine(ComponentGraph.Root, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        var assembly = file.RelativePath.Split('/')[1];
        var source = new AssuranceSourceFile(full, file.RelativePath, assembly, raw, "\n", AssuranceSources.Parse(raw, full));

        return new UnicodeArtefact(
            file.RelativePath,
            File.Exists(full) ? File.ReadAllText(full) : string.Empty,
            AssuranceGenerator.DesiredSource(source, AssuranceScanner.Scan(source)));
    }
}
