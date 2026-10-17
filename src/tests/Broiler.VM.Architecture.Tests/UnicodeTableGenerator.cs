namespace Broiler.VM.Architecture.Tests;

/// <summary>One generated table file: what is on disk, and what the generator says should be.</summary>
internal sealed record UnicodeArtefact(string RelativePath, string Current, string Desired)
{
    internal bool IsCurrent => string.Equals(Current, Desired, StringComparison.Ordinal);
}

/// <summary>One table member and the bytes of data it holds.</summary>
internal sealed record UnicodeTableSize(string File, string Member, int Bytes);

/// <summary>
/// What one generation produced: the table files, the size of every table in them, and the
/// JavaScript probes that carry <c>NormalizationTest.txt</c> to the end-user host.
/// </summary>
internal sealed record UnicodeGeneration(
    IReadOnlyList<UnicodeArtefact> Artefacts,
    IReadOnlyList<UnicodeTableSize> Tables,
    IReadOnlyList<UnicodeArtefact> Probes)
{
    internal int TotalBytes => Tables.Sum(static table => table.Bytes);
}

/// <summary>
/// The generator and the gate for the JavaScript profile's Unicode tables, as one function.
/// </summary>
/// <remarks>
/// <para>
/// <b>Since SJL-7.4 the owner's generator writes every table</b> (JSD-0074, decision SJL-D-6 (c)):
/// <see cref="Generate"/> takes them from <see cref="UnicodeTableEmbedding"/>, which runs
/// <c>Broiler.Unicode.Generation</c> over the same verified archive, and the emitters that were here -
/// properties, case folding, properties of strings, normalization and case mapping - are gone; their
/// tables are the owner's byte for byte, in the same layouts. What stays here is the gate, the write,
/// and the normalization probes, which are this repository's test assets.
/// </para>
/// <para>
/// <b>The shape is rule J5's.</b> Decision JSD-0031 section 5 puts the generator here, in the
/// architecture test project, following <see cref="AssuranceGenerator"/>: <see cref="Generate"/>
/// computes every table file from the verified archive and is pure; with
/// <c>BROILER_UNICODE_WRITE=1</c> the test writes what it computed, and without it the test asserts
/// the checked-in files are byte-identical to it. C# rather than a script, so the check runs inside
/// <c>dotnet test</c> with no interpreter and no network.
/// </para>
/// <para>
/// <b>Its inputs are the pin's files and nothing else</b>, handed over by
/// <see cref="UnicodePin.Verified"/> after their lengths, hashes and version headers have been
/// compared with the pin. <b>Its output is deterministic</b>: every table is sorted by code point
/// or by ordinal name, numbers are written with the invariant culture, lines end in LF, and nothing
/// records a time, a machine or a path outside the repository.
/// </para>
/// <para>
/// <b>It annotates its output.</b> Each table member carries the per-unit <c>EXEMPT=</c> hatch rule J3
/// accepts, the class and its one method carry an ordinary assessment, and the text is then passed
/// through <see cref="AssuranceGenerator.DesiredSource"/> - the function the assurance gate itself
/// uses - so the file header and the fingerprints are exactly what J5 would write
/// (<see cref="UnicodeTableEmbedding.Artefact"/>). A regenerated file therefore needs no hand edit and
/// changes no assurance rule.
/// </para>
/// </remarks>
internal static class UnicodeTableGenerator
{
    /// <summary>Set to <c>1</c> to make the run write rather than assert.</summary>
    internal const string WriteVariable = "BROILER_UNICODE_WRITE";

    /// <summary>
    /// The owner's hard cap on table data: 640 KB, taken as 640 x 1024 bytes, since 2026-10-17.
    /// *(Revised 2026-10-17, when Broiler.Unicode.Algorithms 0.1.0-preview.5 moved normalization, casing
    /// and the lexical classes onto constant-time tables, by the owner's decision that their speed comes
    /// before their bytes: the 33 embedded tables hold 642,028 bytes. The summary read "The owner's hard
    /// cap on table data, 2026-09-22: 300 KB, taken as 300 x 1024 bytes.", and the cap was 300 * 1024.)*
    /// </summary>
    internal const int BudgetBytes = 640 * 1024;

    /// <summary>
    /// The generated files rule N22 holds, in the order they are generated: Format's and the profile's,
    /// which the owner's generator writes since SJL-7.4 (<see cref="UnicodeTableEmbedding"/>). Until then
    /// they were five files of this generator's: <c>JsUnicodeProperties.g.cs</c>,
    /// <c>JsUnicodeCaseFolding.g.cs</c> and <c>JsUnicodeStringProperties.g.cs</c> in Format, and
    /// <c>JsUnicodeNormalization.g.cs</c> and <c>JsUnicodeCasing.g.cs</c> in the profile.
    /// </summary>
    internal static readonly string[] OutputPaths = [UnicodeTableEmbedding.FormatPath, UnicodeTableEmbedding.ProfilePath];

    /// <summary>The per-unit exemption reason of each generated file: the owner's generator's.</summary>
    internal static string ExemptReasonOf(string relativePath) => UnicodeTableEmbedding.ExemptReason;

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The generation for this checkout, from the archive on disk.</summary>
    internal static UnicodeGeneration Current => current.Value;

    private static readonly Lazy<UnicodeGeneration> current = new(static () =>
    {
        var pin = UnicodePin.Load();

        return Generate(pin, pin.ReadArchive(), path => File.Exists(Full(path)) ? File.ReadAllText(Full(path)) : string.Empty);
    });

    /// <summary>
    /// Every table file, from the pinned archive. Throws when the archive is not the pinned one or
    /// the data disagrees with itself; nothing is generated from input that failed a check.
    /// </summary>
    internal static UnicodeGeneration Generate(
        UnicodePin pin,
        IReadOnlyDictionary<string, byte[]> archive,
        Func<string, string> currentText)
    {
        var verified = pin.Verified(archive);
        var owner = UnicodeTableEmbedding.Generate(pin, verified);
        var tables = new List<UnicodeTableSize>();

        var artefacts = new[]
        {
            UnicodeTableEmbedding.Artefact(UnicodeTableEmbedding.Format, owner, currentText, tables),
            UnicodeTableEmbedding.Artefact(UnicodeTableEmbedding.Profile, owner, currentText, tables),
        };

        // Slice U3's conformance probes: test files, not product source, so they carry no
        // assurance annotation and are written exactly as computed.
        var probes = UnicodeNormalizationProbes.Write(verified)
            .Select(probe => new UnicodeArtefact(probe.Path, currentText(probe.Path), probe.Text))
            .ToArray();

        return new UnicodeGeneration(artefacts, tables, probes);
    }

    /// <summary>Writes every table file and probe that is not current, and names the ones it changed.</summary>
    internal static IReadOnlyList<string> Apply(UnicodeGeneration generation)
    {
        var written = new List<string>();

        foreach (var artefact in generation.Artefacts.Concat(generation.Probes).Where(static artefact => !artefact.IsCurrent))
        {
            File.WriteAllText(Full(artefact.RelativePath), artefact.Desired, AssuranceSources.Utf8NoBom);
            written.Add(artefact.RelativePath);
        }

        return written;
    }

    /// <summary>Every artefact whose checked-in text is not what the generator writes, with where they part.</summary>
    internal static IReadOnlyList<string> Stale(IEnumerable<UnicodeArtefact> artefacts) =>
        artefacts.Where(static artefact => !artefact.IsCurrent).Select(static artefact =>
        {
            var current = artefact.Current.Split('\n');
            var desired = artefact.Desired.Split('\n');
            var line = Enumerable.Range(0, Math.Min(current.Length, desired.Length))
                .FirstOrDefault(index => !string.Equals(current[index], desired[index], StringComparison.Ordinal), Math.Min(current.Length, desired.Length));

            return $"{artefact.RelativePath} is not what the Unicode table generator writes: first difference at line {line + 1}. " +
                $"Change the generator or the pinned archive and run with {WriteVariable}=1; never edit the file.";
        }).ToList();

    private static string Full(string relativePath) =>
        Path.Combine(ComponentGraph.Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
