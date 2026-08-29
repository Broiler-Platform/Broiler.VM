namespace Broiler.VM.Fixtures;

/// <summary>One row of the retained corpus manifest.</summary>
/// <remarks>
/// The row carries three separable things and keeps them separable: what the bytes are (file, byte
/// length, hash), how they are presented (descriptor format version, artifact-bytes request), and
/// what they must answer. The last is split again, between an expectation a person wrote and an
/// observation the last regeneration recorded, because collapsing those two is how a baseline stops
/// being able to fail.
/// </remarks>
public sealed class FixtureCorpusRecord
{
    /// <summary>Creates a row.</summary>
    public FixtureCorpusRecord(
        string id,
        string family,
        FixtureCorpusProvenance provenance,
        FixtureCorpusPinning pinning,
        string file,
        int byteLength,
        string sha256,
        uint descriptorFormatVersion,
        ulong artifactBytesRequest,
        VmOutcome expectedOutcome,
        VmReason expectedReason,
        int expectedProfileDiagnosticCode,
        VmBudgetDimension expectedDimension,
        VmBudgetScope expectedScope,
        bool namesDimension,
        VmOutcome recordedOutcome,
        VmReason recordedReason,
        int recordedProfileDiagnosticCode,
        VmBudgetDimension recordedDimension,
        VmBudgetScope recordedScope,
        string note)
    {
        Id = id;
        Family = family;
        Provenance = provenance;
        Pinning = pinning;
        File = file;
        ByteLength = byteLength;
        Sha256 = sha256;
        DescriptorFormatVersion = descriptorFormatVersion;
        ArtifactBytesRequest = artifactBytesRequest;
        ExpectedOutcome = expectedOutcome;
        ExpectedReason = expectedReason;
        ExpectedProfileDiagnosticCode = expectedProfileDiagnosticCode;
        ExpectedDimension = expectedDimension;
        ExpectedScope = expectedScope;
        NamesDimension = namesDimension;
        RecordedOutcome = recordedOutcome;
        RecordedReason = recordedReason;
        RecordedProfileDiagnosticCode = recordedProfileDiagnosticCode;
        RecordedDimension = recordedDimension;
        RecordedScope = recordedScope;
        Note = note;
    }

    /// <summary>The stable identifier, which is also the file name stem.</summary>
    public string Id { get; }

    /// <summary>The group of related cases this belongs to.</summary>
    public string Family { get; }

    /// <summary>Whether the bytes are seeded from a declaration or retained from a fuzz run.</summary>
    public FixtureCorpusProvenance Provenance { get; }

    /// <summary>How firmly the answer is pinned.</summary>
    public FixtureCorpusPinning Pinning { get; }

    /// <summary>The file name, relative to the corpus directory.</summary>
    public string File { get; }

    /// <summary>How many bytes it holds.</summary>
    public int ByteLength { get; }

    /// <summary>The lowercase hexadecimal SHA-256 of those bytes.</summary>
    public string Sha256 { get; }

    /// <summary>The profile-format version the presenting descriptor declares.</summary>
    public uint DescriptorFormatVersion { get; }

    /// <summary>The artifact-requested artifact-bytes tightening, or zero for no request.</summary>
    public ulong ArtifactBytesRequest { get; }

    /// <summary>The expected outcome category, or <see cref="VmOutcome.None"/> for a recorded row.</summary>
    public VmOutcome ExpectedOutcome { get; }

    /// <summary>The expected reason, for an exactly pinned row.</summary>
    public VmReason ExpectedReason { get; }

    /// <summary>The expected profile diagnostic code, for an exactly pinned row.</summary>
    public int ExpectedProfileDiagnosticCode { get; }

    /// <summary>The expected exhausted dimension, where the row names one.</summary>
    public VmBudgetDimension ExpectedDimension { get; }

    /// <summary>The expected exhausted scope, where the row names one.</summary>
    public VmBudgetScope ExpectedScope { get; }

    /// <summary>Whether the dimension and scope are part of the expectation.</summary>
    public bool NamesDimension { get; }

    /// <summary>The outcome the last regeneration observed.</summary>
    public VmOutcome RecordedOutcome { get; }

    /// <summary>The reason the last regeneration observed.</summary>
    public VmReason RecordedReason { get; }

    /// <summary>The profile diagnostic code the last regeneration observed.</summary>
    public int RecordedProfileDiagnosticCode { get; }

    /// <summary>The dimension the last regeneration observed named, meaningful for a resource answer.</summary>
    public VmBudgetDimension RecordedDimension { get; }

    /// <summary>The scope the last regeneration observed named.</summary>
    public VmBudgetScope RecordedScope { get; }

    /// <summary>What this case is for, in one line.</summary>
    public string Note { get; }

    /// <summary>The same row with a different observation.</summary>
    public FixtureCorpusRecord WithObservation(
        VmOutcome outcome,
        VmReason reason,
        int profileDiagnosticCode,
        VmBudgetDimension dimension,
        VmBudgetScope scope) =>
        new(Id, Family, Provenance, Pinning, File, ByteLength, Sha256, DescriptorFormatVersion,
            ArtifactBytesRequest, ExpectedOutcome, ExpectedReason, ExpectedProfileDiagnosticCode,
            ExpectedDimension, ExpectedScope, NamesDimension, outcome, reason, profileDiagnosticCode,
            dimension, scope, Note);
}

/// <summary>
/// Reads and writes the retained corpus: the <c>.bin</c> files and the manifest that binds each one
/// to an identity and an answer.
/// </summary>
/// <remarks>
/// <para>
/// The manifest is rendered by hand rather than serialized, for the same reason the assurance
/// artefacts are: a gate that compares a file with what a generator would write needs the writing to
/// be a total function of the model, and a serializer's option defaults are neither stable across
/// versions nor visible in the diff when they change.
/// </para>
/// <para>
/// Nothing here executes an artifact or knows what a runtime is. The corpus is data plus an
/// expectation; running it belongs to a composition root, and keeping the two apart is what lets the
/// behavioural suite, the fuzz host and the publish-mode host all read one corpus.
/// </para>
/// </remarks>
public static class FixtureCorpusStore
{
    /// <summary>Set to 1 to rewrite the corpus and its manifest instead of asserting them.</summary>
    public const string WriteVariable = "BROILER_CORPUS_WRITE";

    /// <summary>The corpus directory, relative to the component root.</summary>
    public const string RelativeDirectory = "src/tests/corpus/vm-2";

    /// <summary>The manifest file name.</summary>
    public const string ManifestFileName = "manifest.json";

    /// <summary>The extension every corpus file carries.</summary>
    public const string ArtifactExtension = ".bin";

    /// <summary>Whether the write variable is set.</summary>
    public static bool WriteRequested =>
        string.Equals(
            System.Environment.GetEnvironmentVariable(WriteVariable), "1", System.StringComparison.Ordinal);

    /// <summary>The corpus directory under <paramref name="componentRoot"/>.</summary>
    public static string Directory(string componentRoot) =>
        System.IO.Path.Combine(
            componentRoot, RelativeDirectory.Replace('/', System.IO.Path.DirectorySeparatorChar));

    /// <summary>The manifest path under <paramref name="componentRoot"/>.</summary>
    public static string ManifestPath(string componentRoot) =>
        System.IO.Path.Combine(Directory(componentRoot), ManifestFileName);

    /// <summary>The lowercase hexadecimal SHA-256 of <paramref name="bytes"/>.</summary>
    public static string Hash(byte[] bytes) =>
        System.Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));

    /// <summary>The row a seeded entry produces, with no observation yet recorded.</summary>
    public static FixtureCorpusRecord RowFor(FixtureCorpusEntry entry) =>
        new(
            entry.Id,
            entry.Family,
            FixtureCorpusProvenance.Seeded,
            entry.Pinning,
            entry.Id + ArtifactExtension,
            entry.Bytes.Length,
            Hash(entry.Bytes),
            entry.DescriptorFormatVersion,
            entry.ArtifactBytesRequest,
            entry.Outcome,
            entry.Reason,
            entry.ProfileDiagnosticCode,
            entry.Dimension,
            entry.Scope,
            entry.NamesDimension,
            VmOutcome.None,
            VmReason.None,
            0,
            VmBudgetDimension.Fuel,
            VmBudgetScope.Artifact,
            entry.Note);

    /// <summary>Reads the manifest rows, in the order the file lists them.</summary>
    public static System.Collections.Generic.IReadOnlyList<FixtureCorpusRecord> Read(string componentRoot)
    {
        var path = ManifestPath(componentRoot);

        if (!System.IO.File.Exists(path))
        {
            return System.Array.Empty<FixtureCorpusRecord>();
        }

        using var document = System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText(path));

        var rows = new System.Collections.Generic.List<FixtureCorpusRecord>();

        foreach (var element in document.RootElement.GetProperty("entries").EnumerateArray())
        {
            var expected = element.GetProperty("expected");
            var recorded = element.GetProperty("recorded");

            rows.Add(new FixtureCorpusRecord(
                element.GetProperty("id").GetString()!,
                element.GetProperty("family").GetString()!,
                System.Enum.Parse<FixtureCorpusProvenance>(element.GetProperty("provenance").GetString()!),
                System.Enum.Parse<FixtureCorpusPinning>(element.GetProperty("pinning").GetString()!),
                element.GetProperty("file").GetString()!,
                element.GetProperty("bytes").GetInt32(),
                element.GetProperty("sha256").GetString()!,
                element.GetProperty("descriptorFormatVersion").GetUInt32(),
                element.GetProperty("artifactBytesRequest").GetUInt64(),
                System.Enum.Parse<VmOutcome>(expected.GetProperty("outcome").GetString()!),
                System.Enum.Parse<VmReason>(expected.GetProperty("reason").GetString()!),
                expected.GetProperty("profileDiagnosticCode").GetInt32(),
                System.Enum.Parse<VmBudgetDimension>(expected.GetProperty("dimension").GetString()!),
                System.Enum.Parse<VmBudgetScope>(expected.GetProperty("scope").GetString()!),
                expected.GetProperty("namesDimension").GetBoolean(),
                System.Enum.Parse<VmOutcome>(recorded.GetProperty("outcome").GetString()!),
                System.Enum.Parse<VmReason>(recorded.GetProperty("reason").GetString()!),
                recorded.GetProperty("profileDiagnosticCode").GetInt32(),
                System.Enum.Parse<VmBudgetDimension>(recorded.GetProperty("dimension").GetString()!),
                System.Enum.Parse<VmBudgetScope>(recorded.GetProperty("scope").GetString()!),
                element.GetProperty("note").GetString()!));
        }

        return rows;
    }

    /// <summary>Renders the manifest exactly as it must appear on disk.</summary>
    public static string Render(
        System.Collections.Generic.IReadOnlyList<FixtureCorpusRecord> rows,
        string milestone,
        int coreContractVersion)
    {
        var text = new System.Text.StringBuilder(64 * 1024);

        text.Append("{\n");
        text.Append("  \"$comment\": [\n");

        foreach (var line in Comment)
        {
            text.Append("    ").Append(Quote(line)).Append(",\n");
        }

        text.Length -= 2;
        text.Append("\n  ],\n");
        text.Append("  \"milestone\": ").Append(Quote(milestone)).Append(",\n");
        text.Append("  \"coreContractVersion\": ").Append(coreContractVersion).Append(",\n");
        text.Append("  \"profile\": ").Append(Quote(FixtureVmProfile.Id.ToString())).Append(",\n");
        text.Append("  \"featureManifest\": ").Append(Quote(FixtureVmProfile.Manifest.ToString())).Append(",\n");
        text.Append("  \"entries\": [\n");

        for (var index = 0; index < rows.Count; index++)
        {
            AppendRow(text, rows[index], index == rows.Count - 1);
        }

        text.Append("  ]\n");
        text.Append("}\n");

        return text.ToString();
    }

    /// <summary>Writes every seeded file, every manifest row, and nothing else.</summary>
    /// <remarks>
    /// It never deletes and never rewrites a file whose row is
    /// <see cref="FixtureCorpusProvenance.Minimized"/>. A minimized regression is an input a fuzz
    /// run found; regenerating one would erase the only copy of the thing that failed.
    /// </remarks>
    public static void Write(
        string componentRoot,
        System.Collections.Generic.IReadOnlyList<FixtureCorpusEntry> seeded,
        System.Collections.Generic.IReadOnlyList<FixtureCorpusRecord> rows,
        string milestone,
        int coreContractVersion)
    {
        var directory = Directory(componentRoot);
        System.IO.Directory.CreateDirectory(directory);

        foreach (var entry in seeded)
        {
            System.IO.File.WriteAllBytes(
                System.IO.Path.Combine(directory, entry.Id + ArtifactExtension), entry.Bytes);
        }

        System.IO.File.WriteAllText(
            ManifestPath(componentRoot),
            Render(rows, milestone, coreContractVersion),
            new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static void AppendRow(System.Text.StringBuilder text, FixtureCorpusRecord row, bool last)
    {
        text.Append("    {\n");
        text.Append("      \"id\": ").Append(Quote(row.Id)).Append(",\n");
        text.Append("      \"family\": ").Append(Quote(row.Family)).Append(",\n");
        text.Append("      \"provenance\": ").Append(Quote(row.Provenance.ToString())).Append(",\n");
        text.Append("      \"pinning\": ").Append(Quote(row.Pinning.ToString())).Append(",\n");
        text.Append("      \"file\": ").Append(Quote(row.File)).Append(",\n");
        text.Append("      \"bytes\": ").Append(row.ByteLength).Append(",\n");
        text.Append("      \"sha256\": ").Append(Quote(row.Sha256)).Append(",\n");
        text.Append("      \"descriptorFormatVersion\": ").Append(row.DescriptorFormatVersion).Append(",\n");
        text.Append("      \"artifactBytesRequest\": ").Append(row.ArtifactBytesRequest).Append(",\n");
        text.Append("      \"expected\": {\n");
        text.Append("        \"outcome\": ").Append(Quote(row.ExpectedOutcome.ToString())).Append(",\n");
        text.Append("        \"reason\": ").Append(Quote(row.ExpectedReason.ToString())).Append(",\n");
        text.Append("        \"profileDiagnosticCode\": ").Append(row.ExpectedProfileDiagnosticCode).Append(",\n");
        text.Append("        \"dimension\": ").Append(Quote(row.ExpectedDimension.ToString())).Append(",\n");
        text.Append("        \"scope\": ").Append(Quote(row.ExpectedScope.ToString())).Append(",\n");
        text.Append("        \"namesDimension\": ").Append(row.NamesDimension ? "true" : "false").Append('\n');
        text.Append("      },\n");
        text.Append("      \"recorded\": {\n");
        text.Append("        \"outcome\": ").Append(Quote(row.RecordedOutcome.ToString())).Append(",\n");
        text.Append("        \"reason\": ").Append(Quote(row.RecordedReason.ToString())).Append(",\n");
        text.Append("        \"profileDiagnosticCode\": ").Append(row.RecordedProfileDiagnosticCode).Append(",\n");
        text.Append("        \"dimension\": ").Append(Quote(row.RecordedDimension.ToString())).Append(",\n");
        text.Append("        \"scope\": ").Append(Quote(row.RecordedScope.ToString())).Append('\n');
        text.Append("      },\n");
        text.Append("      \"note\": ").Append(Quote(row.Note)).Append('\n');
        text.Append(last ? "    }\n" : "    },\n");
    }

    private static string Quote(string value)
    {
        var text = new System.Text.StringBuilder(value.Length + 2);
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

                case '\n':
                    text.Append("\\n");
                    break;

                case '\r':
                    text.Append("\\r");
                    break;

                case '\t':
                    text.Append("\\t");
                    break;

                default:
                    if (character < ' ')
                    {
                        text.Append("\\u").Append(((int)character).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }

                    text.Append(character);
                    break;
            }
        }

        text.Append('"');
        return text.ToString();
    }

    private static readonly string[] Comment =
    [
        "The retained VM-2 malformed-input corpus. Every entry is a file of artifact bytes plus the",
        "answer verification must give for it.",
        "",
        "TWO FIELDS, AND THEY ARE NOT THE SAME FIELD. `expected` is written by a person, beside the",
        "bytes, in FixtureCorpus.cs. `recorded` is what the last regeneration observed. A row pinned",
        "Exact is held to `expected`; a row pinned Recorded is held to `recorded`, which is an",
        "observation under version control rather than an expectation - it cannot prove the answer is",
        "right, and it does prove the answer has not moved without somebody accepting the diff.",
        "",
        "Recorded rows exist for the systematic sweeps: truncate the canonical artifact at every",
        "offset, invert every one of its bytes. Hand-computing an answer for each would be a second",
        "implementation of the verifier written by the same hand, which is not an oracle. What every",
        "row is held to regardless of pinning is the closed set: the outcome is one the load stage may",
        "produce, a failure yields no handle, and a success yields one.",
        "",
        "Some inverted-byte entries verify successfully, and that is deliberate. A corpus in which",
        "everything fails cannot distinguish a verifier that classifies correctly from one that",
        "rejects whatever it is handed, and the three `control` entries plus those successes are what",
        "make the corpus able to fail in both directions.",
        "",
        "PROVENANCE. `Seeded` rows are rewritten from FixtureCorpus.cs whenever the corpus is",
        "regenerated. `Minimized` rows are regressions a fuzz run found and reduced; their bytes are",
        "the artefact, nothing regenerates them, and the writer never rewrites or removes one.",
        "",
        "Regenerate with BROILER_CORPUS_WRITE=1 dotnet test Broiler.VM.slnx -c Release. Without the",
        "variable the same code asserts every file, every hash and every answer instead of writing",
        "them.",
    ];
}
