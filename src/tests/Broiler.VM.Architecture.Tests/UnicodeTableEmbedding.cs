using System.Globalization;
using System.Text;
using Broiler.Unicode.Algorithms;
using OwnerGenerator = Broiler.Unicode.Generation.UnicodeTableGenerator;
using OwnerPin = Broiler.Unicode.Generation.UnicodePin;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One file the embedding writes: where it is, its namespace, and the tables it holds.</summary>
/// <param name="RelativePath">The file, relative to the repository root.</param>
/// <param name="Namespace">The namespace of the class it declares.</param>
/// <param name="First">The first table it holds, in <see cref="UnicodeTable"/> order.</param>
/// <param name="Last">The last table it holds.</param>
/// <param name="Summary">The class's summary.</param>
/// <param name="Rule">The rule that holds it byte for byte, N22 or N28.</param>
/// <param name="WriteVariable">The environment variable that turns that rule's gate into its write.</param>
internal sealed record UnicodeEmbeddedFile(string RelativePath, string Namespace, UnicodeTable First, UnicodeTable Last, string Summary, string Rule, string WriteVariable)
{
    /// <summary>Whether <paramref name="table"/> is one this file holds.</summary>
    internal bool Holds(UnicodeTable table) => table >= First && table <= Last;
}

/// <summary>
/// The JavaScript family's embedded Unicode tables, as the owner's generator writes them from this
/// repository's pin (JSD-0074, decision SJL-D-6 (c)).
/// </summary>
/// <remarks>
/// <para>
/// <b>The bytes are the owner's, the files are this repository's.</b>
/// <c>Broiler.Unicode.Generation</c>'s <c>UnicodeTableGenerator.Generate</c> is run over the archive
/// <c>src/tests/unicode/pins/unicode.pin</c> names - the owner's own <c>UnicodePin</c> verifying every
/// file's length, hash and version header first - and its 37 tables are split by assembly as they
/// were before the owner held the generator: the property, folding and string-property tables in
/// Format, which the compiler and the runtime both read; normalization and casing in the profile; the
/// break table in the optional Intl assembly. Each file is one internal class whose <c>Get</c> answers
/// a <see cref="UnicodeTable"/> with a span over constant data, which the family's
/// <see cref="IUnicodeTables"/> providers hand to the owner's algorithms.
/// </para>
/// <para>
/// <b>Why not the owner's <c>UnicodeTableSource.Write</c>.</b> It writes no assurance annotation, and
/// a product file here must carry one per unit (rule J3) with the header the assurance generator
/// computes (J5). So the text is written here, in the owner's layout - one
/// <c>ReadOnlySpan&lt;byte&gt;</c> per table, decimal bytes each followed by a comma, at most 100
/// columns - with the per-unit <c>EXEMPT=</c> line on every table, and passed through
/// <see cref="AssuranceGenerator.DesiredSource"/> as every generated file of this repository is. The
/// comma after every byte is what rule N28's count reads.
/// </para>
/// </remarks>
internal static class UnicodeTableEmbedding
{
    internal const string FormatPath = "src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeTables.g.cs";

    internal const string ProfilePath = "src/Broiler.VM.Profile.JavaScript/JsUnicodeTables.g.cs";

    internal const string IntlPath = "src/Broiler.VM.Profile.JavaScript.Intl/JsUnicodeTables.g.cs";

    /// <summary>The per-unit exemption reason every embedded table states.</summary>
    internal const string ExemptReason =
        "Unicode 17.0.0 table data written by Broiler.Unicode.Generation from the files unicode.pin names, compared byte for byte by rules N22 and N28";

    /// <summary>The Format assembly's tables: properties, case folding and the properties of strings.</summary>
    internal static readonly UnicodeEmbeddedFile Format = new(
        FormatPath,
        "Broiler.VM.Profile.JavaScript.Format",
        UnicodeTable.PropertyLayout,
        UnicodeTable.StringPropertyNameIndex,
        "The Unicode 17.0.0 property, case folding and string property tables the matcher and the compiler read through the owner's algorithms.",
        "N22",
        "BROILER_UNICODE_WRITE");

    /// <summary>The profile's tables: normalization and case mapping, Soft_Dotted among them.</summary>
    internal static readonly UnicodeEmbeddedFile Profile = new(
        ProfilePath,
        "Broiler.VM.Profile.JavaScript",
        UnicodeTable.NormalizationLayout,
        UnicodeTable.SoftDottedRanges,
        "The Unicode 17.0.0 normalization and case mapping tables normalize and the case conversions read through the owner's algorithms.",
        "N22",
        "BROILER_UNICODE_WRITE");

    /// <summary>The Intl assembly's table: the break properties segmentation reads.</summary>
    internal static readonly UnicodeEmbeddedFile Intl = new(
        IntlPath,
        "Broiler.VM.Profile.JavaScript.Intl",
        UnicodeTable.SegmentBreaks,
        UnicodeTable.SegmentBreaks,
        "The Unicode 17.0.0 break table Intl.Segmenter reads through the owner's segmenter.",
        "N28",
        "BROILER_CLDR_WRITE");

    /// <summary>The owner's generation over this checkout's pinned archive.</summary>
    internal static IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>> Current => current.Value;

    private static readonly Lazy<IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>>> current = new(static () =>
    {
        var pin = UnicodePin.Load();
        return Generate(pin, pin.ReadArchive());
    });

    /// <summary>
    /// Every table, from the archive <paramref name="pin"/> describes, by the owner's generator. Throws
    /// <see cref="InvalidDataException"/> when the archive is not the pinned one; nothing is generated
    /// from it.
    /// </summary>
    internal static IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>> Generate(UnicodePin pin, IReadOnlyDictionary<string, byte[]> archive) =>
        OwnerGenerator.Generate(OwnerPin.Parse(pin.Text), archive);

    /// <summary>The file's text as the assurance generator would annotate it, beside the file on disk.</summary>
    internal static UnicodeArtefact Artefact(
        UnicodeEmbeddedFile file,
        IReadOnlyList<KeyValuePair<UnicodeTable, byte[]>> tables,
        Func<string, string> currentText,
        List<UnicodeTableSize>? sizes = null)
    {
        var held = tables.Where(table => file.Holds(table.Key)).ToArray();

        if (held.Length != (int)file.Last - (int)file.First + 1)
        {
            throw new InvalidDataException($"the owner's generator wrote {held.Length} of the tables {file.First} to {file.Last}");
        }

        var text = new StringBuilder();

        text.Append("using System;\nusing Broiler.Unicode.Algorithms;\n\n");
        text.Append("namespace ").Append(file.Namespace).Append(";\n\n");
        text.Append("// Written by UnicodeTableEmbedding (src/tests/Broiler.VM.Architecture.Tests) with the owner's generator,\n");
        text.Append("// Broiler.Unicode.Generation, from the Unicode Character Database 17.0.0 files that\n");
        text.Append("// src/tests/unicode/pins/unicode.pin names, under decisions JSD-0031 and JSD-0074 (SJL-D-6 (c)). Rule\n");
        text.Append("// ").Append(file.Rule).Append(" regenerates this file and compares it byte for byte, so a change belongs in the pinned\n");
        text.Append("// archive or the admitted generator and never here: run the architecture tests with\n");
        text.Append("// ").Append(file.WriteVariable).Append("=1.\n");
        text.Append("//\n");
        text.Append("// The table data is derived from Unicode data files and is subject to the Unicode License v3\n");
        text.Append("// (SPDX Unicode-3.0), whose text THIRD_PARTY_NOTICES.md carries. The SPDX lines above are written\n");
        text.Append("// on every product file by the code assurance generator and describe this file's code.\n\n");
        text.Append("/// <summary>").Append(file.Summary).Append("</summary>\n");
        text.Append("// Broiler-AI:           Origin=Derived; Spec=JSD-0074; IP=Low; Security=Low; Resources=1; Fingerprint=TBF\n");
        text.Append("// Broiler-Human:        PENDING\n");
        text.Append("internal static class JsUnicodeTableData\n");
        text.Append("{\n");
        text.Append("    /// <summary>The bytes of <paramref name=\"table\"/>, or an empty span for a table this assembly does not embed.</summary>\n");
        text.Append("    // Broiler-AI:           Origin=Derived; Spec=JSD-0074; IP=Low; Security=Low; Resources=1; Fingerprint=TBF\n");
        text.Append("    // Broiler-Human:        PENDING\n");
        text.Append("    internal static ReadOnlySpan<byte> Get(UnicodeTable table) => table switch\n");
        text.Append("    {\n");

        foreach (var (table, _) in held)
        {
            text.Append("        UnicodeTable.").Append(table).Append(" => ").Append(table).Append(",\n");
        }

        text.Append("        _ => default,\n");
        text.Append("    };\n");

        foreach (var (table, bytes) in held)
        {
            sizes?.Add(new UnicodeTableSize(file.RelativePath, $"JsUnicodeTableData.{table}", bytes.Length));

            text.Append('\n');
            text.Append("    /// <summary>").Append(table).Append(", ")
                .Append(bytes.Length.ToString("N0", CultureInfo.InvariantCulture))
                .Append(" bytes, in the layout the owner's UnicodeTable states for it.</summary>\n");
            text.Append("    // Broiler-AI:           EXEMPT=").Append(ExemptReason).Append('\n');
            text.Append("    // Broiler-Human:        PENDING\n");
            text.Append("    private static ReadOnlySpan<byte> ").Append(table).Append(" => new byte[]\n");
            text.Append("    {\n");

            var line = new StringBuilder("       ");

            foreach (var value in bytes)
            {
                var item = value.ToString(CultureInfo.InvariantCulture);

                if (line.Length + item.Length + 2 > 100)
                {
                    text.Append(line).Append('\n');
                    line.Clear().Append("       ");
                }

                line.Append(' ').Append(item).Append(',');
            }

            if (bytes.Length > 0)
            {
                text.Append(line).Append('\n');
            }

            text.Append("    };\n");
        }

        text.Append("}\n");

        var full = Path.Combine(ComponentGraph.Root, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        var assembly = file.RelativePath.Split('/')[1];
        var raw = text.ToString();
        var source = new AssuranceSourceFile(full, file.RelativePath, assembly, raw, "\n", AssuranceSources.Parse(raw, full));

        return new UnicodeArtefact(
            file.RelativePath,
            currentText(file.RelativePath),
            AssuranceGenerator.DesiredSource(source, AssuranceScanner.Scan(source)));
    }

    /// <summary>The text of a file in this checkout, or the empty string when there is none.</summary>
    internal static string OnDisk(string relativePath)
    {
        var full = Path.Combine(ComponentGraph.Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(full) ? File.ReadAllText(full) : string.Empty;
    }
}
