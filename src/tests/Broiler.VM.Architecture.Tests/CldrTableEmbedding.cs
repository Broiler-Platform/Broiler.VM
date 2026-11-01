using Broiler.ECMAScript.Generation.Intl;
using Broiler.ECMAScript.Generation.Time;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The JavaScript family's Intl tables, <c>JsCldrTables.g.cs</c>, as the Intl data home's generator writes
/// them from this repository's CLDR and Unicode pins (JSD-0082, decisions D10 to D12; milestone SJL-9.2c).
/// </summary>
/// <remarks>
/// <para>
/// <b>The generator is the home's, the pins and the file are this repository's.</b> Until SJL-9.2c the
/// architecture tests held their own CLDR table generator, the UCD reader it used and the CLDR pin reader
/// (JSD-0043, rules N27 and N28). They moved to the Broiler.JS repository, the data home, at SJL-9.2b and
/// reach this project in the test-only package <c>Broiler.ECMAScript.Generation</c>: its
/// <c>IntlCldrPin</c> verifies every file this repository's <c>src/tests/cldr/pins/cldr.pin</c> names, its
/// <c>CldrUcd</c> reads the two UCD files the collation tables need from this repository's own pinned UCD,
/// verified first by <see cref="UnicodePin"/>, its <c>CldrTableGenerator</c> writes the tables for this
/// family's locale list, and its <c>IntlTableSource</c> wraps them into C# with this repository's
/// namespace, class, header and annotation lines. The package carries no pin and no table, so what it
/// reads is what this class hands it, and rule I4 holds this repository's CLDR pin to the home's, entry by
/// entry.
/// </para>
/// <para>
/// <b>The file is the one this family embedded, byte for byte</b> (JSD-0082's falsifier "the home's
/// generator, from this family's pins, does not write <c>JsCldrTables.g.cs</c> byte for byte"): the options
/// below are the locale list and the header, summary and annotation lines the moved generator wrote as
/// constants, and the writer's text is passed through <see cref="AssuranceGenerator.DesiredSource"/>, which
/// writes the assurance header and fills each fingerprint, as every generated file of this repository is.
/// Rule N28 compares the result with the checked-in file. The header still names the generator where it
/// ran when the file was last written, as the time tables' headers do since SJL-8.5: the file is held byte
/// for byte, and a product change would be needed to reword it.
/// </para>
/// </remarks>
internal static class CldrTableEmbedding
{
    /// <summary>The CLDR pin, relative to the repository root.</summary>
    internal const string PinPath = "src/tests/cldr/pins/cldr.pin";

    /// <summary>The directory the CLDR pin's archive is in.</summary>
    internal const string PinDirectory = "src/tests/cldr/pins";

    /// <summary>The file the generator writes, relative to the repository root, held by rule N28.</summary>
    internal const string OutputPath = "src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs";

    /// <summary>The environment variable that turns rule N28's gate into the generator's write.</summary>
    internal const string WriteVariable = "BROILER_CLDR_WRITE";

    /// <summary>The per-member exemption every generated table states, as the moved generator wrote it.</summary>
    internal const string ExemptReason =
        "CLDR 48.2.0 table data written by CldrTableGenerator from the files cldr.pin names, compared byte for byte by rule N28";

    /// <summary>
    /// This family's locale list (JSD-0082, decision D5), in the home's list grammar: the moved generator's
    /// <c>SupportedLocales</c>, <c>de</c>, <c>de-DE</c>, <c>en</c>, <c>en-US</c> and <c>und</c>, with main data
    /// for its <c>NumberLanguages</c>, <c>de</c> and <c>en</c>, and the supplemental rules of the same two
    /// languages. It does not grow: rule N28's budget binds it, and an addition is a dated data decision.
    /// </summary>
    internal const string LocaleList =
        """
        locale de main
        locale de-DE
        locale en main
        locale en-US
        locale und

        supplemental de
        supplemental en
        """;

    /// <summary>The generator's parameters: this family's list, and every other parameter at its default.</summary>
    internal static IntlGeneratorOptions Options => new(IntlLocaleList.Parse(LocaleList));

    /// <summary>What the home's source writer makes of the file around the tables: the moved generator's header constant.</summary>
    internal static readonly TableSourceOptions SourceOptions = new("Broiler.VM.Profile.JavaScript.Intl", "JsCldrTables")
    {
        Header =
        [
            "Written by CldrTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the CLDR 48.2.0",
            "files that src/tests/cldr/pins/cldr.pin names and the Unicode Character Database 17.0.0 files",
            "that src/tests/unicode/pins/unicode.pin names, under decision JSD-0043. Rule N28 regenerates",
            "this file and compares it byte for byte, so a change belongs in the generator or a pinned",
            "archive and never here: run the architecture tests with BROILER_CLDR_WRITE=1.",
            "",
            "The table data is derived from Unicode data files and is subject to the Unicode License v3",
            "(SPDX Unicode-3.0), whose text THIRD_PARTY_NOTICES.md carries. The SPDX lines above are written",
            "on every product file by the code assurance generator and describe this file's code.",
        ],
        ClassAnnotation =
        [
            "Broiler-AI:           Origin=Derived; Spec=JSD-0043; IP=Low; Security=Low; Resources=1; Fingerprint=TBF",
            "Broiler-Human:        PENDING",
        ],
        MemberAnnotation =
        [
            "Broiler-AI:           EXEMPT=" + ExemptReason,
            "Broiler-Human:        PENDING",
        ],
    };

    /// <summary>Whether rule N28's write was asked for.</summary>
    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>This repository's CLDR pin, as the home's reader parses it.</summary>
    internal static IntlCldrPin LoadPin() => IntlCldrPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, PinPath)));

    /// <summary>Every file the pin names, off disk, by the path the pin writes it under; not yet verified.</summary>
    internal static IReadOnlyDictionary<string, byte[]> ReadArchive(IntlCldrPin pin) =>
        pin.Entries.ToDictionary(
            static entry => entry.Name,
            static entry => File.ReadAllBytes(Path.Combine(
                ComponentGraph.Root, PinDirectory, entry.Name.Replace('/', Path.DirectorySeparatorChar))),
            StringComparer.Ordinal);

    /// <summary>
    /// The tables, from this repository's CLDR archive and its pinned UCD, by the home's generator: the UCD
    /// files are verified against <c>unicode.pin</c> before the home's reader reads the two it needs, and the
    /// CLDR files against <c>cldr.pin</c> by the generator before any table is made.
    /// </summary>
    internal static IntlTables Generate(
        IntlCldrPin pin,
        IReadOnlyDictionary<string, byte[]> cldr,
        UnicodePin unicodePin,
        IReadOnlyDictionary<string, byte[]> ucd) =>
        CldrTableGenerator.Generate(pin, cldr, CldrUcd.Read(unicodePin.Verified(ucd)), Options);

    /// <summary>The file's text, as the home's writer writes it with this repository's options, before the assurance pass.</summary>
    internal static string Source(IntlTables tables) => IntlTableSource.Write(tables, SourceOptions);

    /// <summary>The file's text from the given pins and archives, before the assurance pass.</summary>
    internal static string Write(
        IntlCldrPin pin,
        IReadOnlyDictionary<string, byte[]> cldr,
        UnicodePin unicodePin,
        IReadOnlyDictionary<string, byte[]> ucd) => Source(Generate(pin, cldr, unicodePin, ucd));

    /// <summary>The generated file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact Current => current.Value;

    private static readonly Lazy<UnicodeArtefact> current = new(static () =>
    {
        var pin = LoadPin();
        var unicode = UnicodePin.Load();
        return Artefact(Write(pin, ReadArchive(pin), unicode, unicode.ReadArchive()));
    });

    /// <summary>Generated text passed through the function the assurance gate uses, beside the file on disk.</summary>
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

    /// <summary>An archived text file's bytes as text, as the pins' readers decode them.</summary>
    internal static string Decode(byte[] bytes) => UnicodePin.Decode(bytes);
}
