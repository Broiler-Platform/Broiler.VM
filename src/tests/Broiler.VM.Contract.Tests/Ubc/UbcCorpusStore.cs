using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Broiler.VM;
using Broiler.VM.Fixtures;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>One seeded corpus entry: the bytes, how they are presented, and what they must answer.</summary>
internal sealed class UbcCorpusEntry
{
    internal UbcCorpusEntry(
        string id,
        string family,
        byte[] bytes,
        FixtureCorpusPinning pinning,
        UbcCorpusAnswer expected,
        bool namesDimension,
        UbcCorpusConfiguration configuration,
        string note)
    {
        Id = id;
        Family = family;
        Bytes = bytes;
        Pinning = pinning;
        Expected = expected;
        NamesDimension = namesDimension;
        Configuration = configuration;
        Note = note;
    }

    internal string Id { get; }

    internal string Family { get; }

    internal byte[] Bytes { get; }

    internal FixtureCorpusPinning Pinning { get; }

    internal UbcCorpusAnswer Expected { get; }

    internal bool NamesDimension { get; }

    internal UbcCorpusConfiguration Configuration { get; }

    internal string Note { get; }
}

/// <summary>One manifest row, read back from disk or about to be written.</summary>
internal sealed class UbcCorpusRecord
{
    internal UbcCorpusRecord(
        string id,
        string family,
        FixtureCorpusProvenance provenance,
        FixtureCorpusPinning pinning,
        string file,
        int byteLength,
        string sha256,
        UbcCorpusConfiguration configuration,
        UbcCorpusAnswer expected,
        bool namesDimension,
        UbcCorpusAnswer recorded,
        string note)
    {
        Id = id;
        Family = family;
        Provenance = provenance;
        Pinning = pinning;
        File = file;
        ByteLength = byteLength;
        Sha256 = sha256;
        Configuration = configuration;
        Expected = expected;
        NamesDimension = namesDimension;
        Recorded = recorded;
        Note = note;
    }

    internal string Id { get; }

    internal string Family { get; }

    internal FixtureCorpusProvenance Provenance { get; }

    internal FixtureCorpusPinning Pinning { get; }

    internal string File { get; }

    internal int ByteLength { get; }

    internal string Sha256 { get; }

    internal UbcCorpusConfiguration Configuration { get; }

    internal UbcCorpusAnswer Expected { get; }

    internal bool NamesDimension { get; }

    internal UbcCorpusAnswer Recorded { get; }

    internal string Note { get; }

    internal UbcCorpusRecord WithRecorded(UbcCorpusAnswer recorded) =>
        new(Id, Family, Provenance, Pinning, File, ByteLength, Sha256, Configuration, Expected, NamesDimension, recorded, Note);
}

/// <summary>A code no artifact can reach through the verifier, and why.</summary>
internal sealed record UbcDefensiveCode(UbcDiagnosticCode Code, string Why);

/// <summary>
/// Reads and writes the universal bytecode's retained corpus: <c>src/tests/corpus/ubc-1/</c>, one
/// <c>.bin</c> file per entry and a manifest in ADR 0011's published schema, the way
/// <see cref="FixtureCorpusStore"/> keeps VM-2's.
/// </summary>
/// <remarks>
/// The manifest is rendered by hand, LF-terminated, in the generator's order, so two regenerations
/// are byte-identical and a gate can compare the file with what the generator would write.
/// </remarks>
internal static class UbcCorpusStore
{
    /// <summary>Set to 1 to rewrite the corpus and its manifest instead of asserting them.</summary>
    internal const string WriteVariable = "BROILER_UBC_CORPUS_WRITE";

    internal const string RelativeDirectory = "src/tests/corpus/ubc-1";

    internal const string ManifestFileName = "manifest.json";

    internal const string ArtifactExtension = ".bin";

    /// <summary>
    /// The codes no artifact reaches through the verifier. Every other member of
    /// <see cref="UbcDiagnosticCode"/> is the expected code of a named exactly pinned entry.
    /// </summary>
    internal static ImmutableArray<UbcDefensiveCode> DefensiveCodes { get; } =
    [
        new(UbcDiagnosticCode.ReaderStopped,
            "The reader's mapping of a latched bounded-read status (ADR 0011, C2) has an arm for every status a failed read can latch; " +
            "the default arm answers a status no read produces, Ok included, which a failed read never leaves latched."),
    ];

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    internal static string Directory(string componentRoot) =>
        Path.Combine(componentRoot, RelativeDirectory.Replace('/', Path.DirectorySeparatorChar));

    internal static string ManifestPath(string componentRoot) => Path.Combine(Directory(componentRoot), ManifestFileName);

    internal static string Hash(byte[] bytes) =>
        Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));

    /// <summary>The row a seeded entry produces, recording <paramref name="observed"/>.</summary>
    internal static UbcCorpusRecord RowFor(UbcCorpusEntry entry, UbcCorpusAnswer observed) =>
        new(
            entry.Id,
            entry.Family,
            FixtureCorpusProvenance.Seeded,
            entry.Pinning,
            entry.Id + ArtifactExtension,
            entry.Bytes.Length,
            Hash(entry.Bytes),
            entry.Configuration,
            entry.Expected,
            entry.NamesDimension,
            observed,
            entry.Note);

    /// <summary>Reads the manifest rows in the order the file lists them.</summary>
    internal static IReadOnlyList<UbcCorpusRecord> Read(string componentRoot)
    {
        var path = ManifestPath(componentRoot);

        if (!File.Exists(path))
        {
            return Array.Empty<UbcCorpusRecord>();
        }

        return Parse(File.ReadAllText(path));
    }

    /// <summary>Parses manifest text.</summary>
    internal static IReadOnlyList<UbcCorpusRecord> Parse(string text)
    {
        using var document = JsonDocument.Parse(text);
        var rows = new List<UbcCorpusRecord>();

        foreach (var element in document.RootElement.GetProperty("entries").EnumerateArray())
        {
            var ceilings = ImmutableSortedDictionary.CreateBuilder<VmBudgetDimension, ulong>();

            foreach (var ceiling in element.GetProperty("ceilings").EnumerateObject())
            {
                ceilings.Add(Enum.Parse<VmBudgetDimension>(ceiling.Name), ceiling.Value.GetUInt64());
            }

            var configuration = new UbcCorpusConfiguration(
                element.GetProperty("descriptorFormatVersion").GetUInt32(),
                element.GetProperty("artifactBytesRequest").GetUInt64(),
                element.GetProperty("descriptorManifest").GetString()!,
                Enum.Parse<UbcCorpusHookMode>(element.GetProperty("hook").GetString()!),
                element.GetProperty("cancelled").GetBoolean(),
                ceilings.ToImmutable());

            var expected = element.GetProperty("expected");

            rows.Add(new UbcCorpusRecord(
                element.GetProperty("id").GetString()!,
                element.GetProperty("family").GetString()!,
                Enum.Parse<FixtureCorpusProvenance>(element.GetProperty("provenance").GetString()!),
                Enum.Parse<FixtureCorpusPinning>(element.GetProperty("pinning").GetString()!),
                element.GetProperty("file").GetString()!,
                element.GetProperty("bytes").GetInt32(),
                element.GetProperty("sha256").GetString()!,
                configuration,
                ReadAnswer(expected),
                expected.GetProperty("namesDimension").GetBoolean(),
                ReadAnswer(element.GetProperty("recorded")),
                element.GetProperty("note").GetString()!));
        }

        return rows;
    }

    /// <summary>Renders the manifest exactly as it must appear on disk.</summary>
    internal static string Render(IReadOnlyList<UbcCorpusRecord> rows)
    {
        var text = new StringBuilder(256 * 1024);

        text.Append("{\n");
        text.Append("  \"$comment\": [\n");

        var comment = Comment();

        for (var index = 0; index < comment.Count; index++)
        {
            text.Append("    ").Append(Quote(comment[index])).Append(index == comment.Count - 1 ? "\n" : ",\n");
        }

        text.Append("  ],\n");
        text.Append("  \"milestone\": \"UBC-1\",\n");
        text.Append("  \"coreContractVersion\": ").Append(VmCoreContract.Version).Append(",\n");
        text.Append("  \"ubcContractVersion\": ").Append(UbcContract.Version).Append(",\n");
        text.Append("  \"ubcFormatVersion\": ").Append(UbcFormat.FormatVersion).Append(",\n");
        text.Append("  \"walkVersion\": ").Append(UbcVerifier.WalkVersion).Append(",\n");
        text.Append("  \"profile\": ").Append(Quote(UbcCorpusFamily.Identity)).Append(",\n");
        text.Append("  \"featureManifests\": [")
            .Append(Quote(UbcCorpusFamily.BaseManifestText)).Append(", ")
            .Append(Quote(UbcCorpusFamily.WideManifestText)).Append("],\n");
        text.Append("  \"defensiveCodes\": [\n");

        for (var index = 0; index < DefensiveCodes.Length; index++)
        {
            var code = DefensiveCodes[index];
            text.Append("    { \"name\": ").Append(Quote(code.Code.ToString()))
                .Append(", \"code\": ").Append((int)code.Code)
                .Append(", \"why\": ").Append(Quote(code.Why))
                .Append(index == DefensiveCodes.Length - 1 ? " }\n" : " },\n");
        }

        text.Append("  ],\n");
        text.Append("  \"entries\": [\n");

        for (var index = 0; index < rows.Count; index++)
        {
            AppendRow(text, rows[index], index == rows.Count - 1);
        }

        text.Append("  ]\n");
        text.Append("}\n");

        return text.ToString();
    }

    /// <summary>Writes every seeded file and the manifest; never deletes anything.</summary>
    internal static void Write(string componentRoot, IReadOnlyList<UbcCorpusEntry> entries, IReadOnlyList<UbcCorpusRecord> rows)
    {
        var directory = Directory(componentRoot);
        System.IO.Directory.CreateDirectory(directory);

        foreach (var entry in entries)
        {
            File.WriteAllBytes(Path.Combine(directory, entry.Id + ArtifactExtension), entry.Bytes);
        }

        File.WriteAllText(ManifestPath(componentRoot), Render(rows), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static UbcCorpusAnswer ReadAnswer(JsonElement element) =>
        new(
            Enum.Parse<VmOutcome>(element.GetProperty("outcome").GetString()!),
            Enum.Parse<VmReason>(element.GetProperty("reason").GetString()!),
            element.GetProperty("profileDiagnosticCode").GetInt32(),
            Enum.Parse<VmBudgetDimension>(element.GetProperty("dimension").GetString()!),
            Enum.Parse<VmBudgetScope>(element.GetProperty("scope").GetString()!),
            element.GetProperty("position").GetString()!);

    private static void AppendRow(StringBuilder text, UbcCorpusRecord row, bool last)
    {
        var configuration = row.Configuration;

        text.Append("    {\n");
        text.Append("      \"id\": ").Append(Quote(row.Id)).Append(",\n");
        text.Append("      \"family\": ").Append(Quote(row.Family)).Append(",\n");
        text.Append("      \"provenance\": ").Append(Quote(row.Provenance.ToString())).Append(",\n");
        text.Append("      \"pinning\": ").Append(Quote(row.Pinning.ToString())).Append(",\n");
        text.Append("      \"file\": ").Append(Quote(row.File)).Append(",\n");
        text.Append("      \"bytes\": ").Append(row.ByteLength).Append(",\n");
        text.Append("      \"sha256\": ").Append(Quote(row.Sha256)).Append(",\n");
        text.Append("      \"descriptorFormatVersion\": ").Append(configuration.DescriptorFormatVersion).Append(",\n");
        text.Append("      \"artifactBytesRequest\": ").Append(configuration.ArtifactBytesRequest).Append(",\n");
        text.Append("      \"descriptorManifest\": ").Append(Quote(configuration.DescriptorManifest)).Append(",\n");
        text.Append("      \"hook\": ").Append(Quote(configuration.Hook.ToString())).Append(",\n");
        text.Append("      \"cancelled\": ").Append(configuration.Cancelled ? "true" : "false").Append(",\n");
        text.Append("      \"ceilings\": {");

        var first = true;

        foreach (var (dimension, value) in configuration.Ceilings)
        {
            text.Append(first ? " " : ", ").Append(Quote(dimension.ToString())).Append(": ").Append(value);
            first = false;
        }

        text.Append(first ? "},\n" : " },\n");
        text.Append("      \"expected\": {\n");
        AppendAnswer(text, row.Expected);
        text.Append(",\n        \"namesDimension\": ").Append(row.NamesDimension ? "true" : "false").Append('\n');
        text.Append("      },\n");
        text.Append("      \"recorded\": {\n");
        AppendAnswer(text, row.Recorded);
        text.Append('\n');
        text.Append("      },\n");
        text.Append("      \"note\": ").Append(Quote(row.Note)).Append('\n');
        text.Append(last ? "    }\n" : "    },\n");
    }

    private static void AppendAnswer(StringBuilder text, UbcCorpusAnswer answer)
    {
        text.Append("        \"outcome\": ").Append(Quote(answer.Outcome.ToString())).Append(",\n");
        text.Append("        \"reason\": ").Append(Quote(answer.Reason.ToString())).Append(",\n");
        text.Append("        \"profileDiagnosticCode\": ").Append(answer.ProfileDiagnosticCode).Append(",\n");
        text.Append("        \"dimension\": ").Append(Quote(answer.Dimension.ToString())).Append(",\n");
        text.Append("        \"scope\": ").Append(Quote(answer.Scope.ToString())).Append(",\n");
        text.Append("        \"position\": ").Append(Quote(answer.Position));
    }

    private static string Quote(string value)
    {
        var text = new StringBuilder(value.Length + 2);
        text.Append('"');

        foreach (var character in value)
        {
            switch (character)
            {
                case '"':
                    text.Append("\\\"");
                    break;

                case '\\':
                    text.Append("\\\\");
                    break;

                default:
                    if (character < ' ' || character > '~')
                    {
                        text.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                        break;
                    }

                    text.Append(character);
                    break;
            }
        }

        text.Append('"');
        return text.ToString();
    }

    private static List<string> Comment()
    {
        var lines = new List<string>
        {
            "The retained UBC-1 malformed-input corpus of the universal bytecode: one file of artifact bytes per",
            "entry, and the answer the universal bytecode's verifier must give for it, in ADR 0011's published",
            "entry schema C3. The artifacts are written against a test-only family, com.example.ubccorpus, whose",
            "descriptor is built by UbcDescriptors.Build and never registered in a catalog: the runner",
            "(src/tests/Broiler.VM.Contract.Tests/Ubc) calls its verifier directly with a verification context and",
            "a meter of its own.",
            "",
            "EXPECTED AND RECORDED ARE NOT THE SAME FIELD. `expected` is written by a person, beside the bytes, in",
            "UbcCorpus.cs. `recorded` is what the last regeneration observed. A row pinned Exact is held to",
            "`expected`; a row pinned Recorded is held to `recorded`, an observation under version control rather",
            "than an expectation. Recorded rows are the two systematic sweeps over the canonical control: truncate",
            "it at every offset, invert every one of its bytes.",
            "",
            "FIELDS BEYOND C3, each part of the configuration an answer was pinned under or of the answer itself:",
            "- descriptorManifest: the feature manifest the presenting artifact descriptor names.",
            "- hook: which verifier hook the descriptor is built with. Standard is the family's own; UniversalCode,",
            "  NoInvalidReason and Exhausting break the hook contract on purpose at the first family instruction.",
            "- cancelled: whether the cancellation token is cancelled before verification begins.",
            "- ceilings: verification ceilings that replace the family's declared default for their dimension; the",
            "  artifact-bytes request then tightens, never loosens, what results.",
            "- expected.position and recorded.position: the refusal's position as",
            "  sectionIndex:byteOffset:coordinate0:coordinate1, or - where an Exact row pins none. A section index",
            "  is the kind byte of the section, or -1 for the header; a walk refusal's offset is the instruction's",
            "  absolute Code offset and its first coordinate the unit; a decoded row's refusal carries offset zero",
            "  and the row index as its second coordinate.",
            "",
            "DEFENSIVE CODES. Every member of UbcDiagnosticCode is the expected code of at least one named Exact",
            "entry, except the ones `defensiveCodes` lists, each with the reason no artifact reaches it:",
        };

        foreach (var code in DefensiveCodes)
        {
            lines.Add($"- {code.Code} ({(int)code.Code}): {code.Why}");
        }

        lines.Add("");
        lines.Add("Regenerate with BROILER_UBC_CORPUS_WRITE=1 dotnet test src/tests/Broiler.VM.Contract.Tests -c Release");
        lines.Add("--no-build. Without the variable the same code asserts every file, every hash and every answer.");

        return lines;
    }
}
