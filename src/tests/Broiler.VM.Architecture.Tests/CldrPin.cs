using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One file the CLDR pin names: where it is, how long it is and what it hashes to.</summary>
/// <param name="Name">The path as the pin writes it, relative to the pin's directory.</param>
/// <param name="RepositoryPath">The same path relative to the repository root.</param>
internal sealed record CldrPinEntry(string Name, string RepositoryPath, long Bytes, string Sha256);

/// <summary>
/// The CLDR data pin, <c>src/tests/cldr/pins/cldr.pin</c>, parsed the way a generator will read it,
/// and the checks that stand between an archived byte and anything made from it (rule N27).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is <see cref="UnicodePin"/>'s shape on purpose</b>: decision JSD-0027 section 5 asks for CLDR
/// to be "pinned by release and digest", in the same form JSD-0031 gave the UCD, so the archive is
/// read only through the pin's own list, after every entry's length, SHA-256 and version have been
/// compared with what the pin records, and a failure is an exception rather than a warning.
/// </para>
/// <para>
/// <b>The version is read where each file states it</b>: a supplemental JSON file in its
/// <c>version</c> object's <c>_cldrVersion</c>, a <c>package.json</c> in its <c>version</c>, and the
/// UCA files in a <c># UCA Version:</c> line. The <c>bcp47</c> files, the collation XML and the
/// licence state none and are held by their hashes alone.
/// </para>
/// </remarks>
internal sealed class CldrPin
{
    /// <summary>The pin, relative to the repository root.</summary>
    internal const string PinPath = "src/tests/cldr/pins/cldr.pin";

    /// <summary>The directory the pin's <c>file</c> paths are relative to.</summary>
    internal const string PinDirectory = "src/tests/cldr/pins";

    /// <summary>The one CLDR release this repository's Intl data is taken from.</summary>
    internal const string PinnedVersion = "48.2.0";

    /// <summary>The CLDR major version the JSON files state.</summary>
    internal const string PinnedMajor = "48";

    /// <summary>The UCA version the collation files state, which is the UCD version JSD-0031 pins.</summary>
    internal const string PinnedUcaVersion = UnicodePin.PinnedVersion;

    private CldrPin(IReadOnlyDictionary<string, string> values, IReadOnlyList<CldrPinEntry> entries, IReadOnlyList<string> integrities)
    {
        Values = values;
        Entries = entries;
        Integrities = integrities;
    }

    /// <summary>The single-valued keys: <c>version</c>, <c>archived</c> and the rest.</summary>
    internal IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>Every file the pin names, in the order it names them.</summary>
    internal IReadOnlyList<CldrPinEntry> Entries { get; }

    /// <summary>The <c>json-integrity</c> lines, one per npm package the JSON files came from.</summary>
    internal IReadOnlyList<string> Integrities { get; }

    /// <summary>The version line, or the empty string when there is none.</summary>
    internal string Version => Values.TryGetValue("version", out var version) ? version : string.Empty;

    /// <summary>The checked-in pin.</summary>
    internal static CldrPin Load() =>
        Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, PinPath)));

    /// <summary>Parses pin text: <c>key value</c> lines, <c>#</c> comments, repeated file and integrity lines.</summary>
    internal static CldrPin Parse(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var entries = new List<CldrPinEntry>();
        var integrities = new List<string>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] == "file")
            {
                if (parts.Length != 4 || !long.TryParse(parts[2], out var bytes))
                {
                    throw new InvalidDataException($"cldr.pin: `{line}` is not `file <path> <bytes> <sha256>`");
                }

                entries.Add(new CldrPinEntry(parts[1], $"{PinDirectory}/{parts[1]}", bytes, parts[3].ToLowerInvariant()));
                continue;
            }

            if (parts[0] == "json-integrity")
            {
                integrities.Add(string.Join(' ', parts.Skip(1)));
                continue;
            }

            if (parts.Length < 2 || !values.TryAdd(parts[0], string.Join(' ', parts.Skip(1))))
            {
                throw new InvalidDataException($"cldr.pin: `{line}` is empty or repeats a key");
            }
        }

        return new CldrPin(values, entries, integrities);
    }

    /// <summary>The entry for a pin-relative name, e.g. <c>cldr-48.2.0/common/uca/allkeys_CLDR.txt</c>.</summary>
    internal CldrPinEntry Entry(string name) =>
        Entries.SingleOrDefault(entry => string.Equals(entry.Name, name, StringComparison.Ordinal))
        ?? throw new InvalidDataException($"cldr.pin names no file `{name}`");

    /// <summary>Reads every pinned file off disk, by the pin's own list and nothing else.</summary>
    internal IReadOnlyDictionary<string, byte[]> ReadArchive() =>
        Entries.ToDictionary(
            static entry => entry.Name,
            static entry => File.ReadAllBytes(Path.Combine(
                ComponentGraph.Root, entry.RepositoryPath.Replace('/', Path.DirectorySeparatorChar))),
            StringComparer.Ordinal);

    /// <summary>
    /// Every way the given bytes differ from what the pin records: a missing file, a length, a hash,
    /// or a stated version that is not the pinned one. Empty when the archive is the one the pin
    /// describes.
    /// </summary>
    internal IEnumerable<string> Violations(IReadOnlyDictionary<string, byte[]> archive)
    {
        if (!string.Equals(Version, PinnedVersion, StringComparison.Ordinal))
        {
            yield return $"cldr.pin names version `{Version}`, and the Intl data is taken from {PinnedVersion} only";
        }

        if (!Values.TryGetValue("archived", out var archived) || archived != "yes")
        {
            yield return "cldr.pin does not say `archived yes`, and nothing reads unarchived bytes";
        }

        foreach (var entry in Entries)
        {
            if (!archive.TryGetValue(entry.Name, out var bytes))
            {
                yield return $"{entry.RepositoryPath} is pinned and no file is there";
                continue;
            }

            if (bytes.LongLength != entry.Bytes)
            {
                yield return $"{entry.RepositoryPath} is {bytes.LongLength} bytes, and the pin records {entry.Bytes}";
            }

            var digest = Convert.ToHexStringLower(SHA256.HashData(bytes));

            if (!string.Equals(digest, entry.Sha256, StringComparison.Ordinal))
            {
                yield return $"{entry.RepositoryPath} hashes to {digest}, and the pin records {entry.Sha256}";
            }

            if (VersionViolation(entry.Name, bytes) is { } stated)
            {
                yield return $"{entry.RepositoryPath}: {stated}";
            }
        }
    }

    /// <summary>The verified archive, or an exception naming every way it is not the pinned one.</summary>
    internal IReadOnlyDictionary<string, byte[]> Verified(IReadOnlyDictionary<string, byte[]> archive)
    {
        var violations = Violations(archive).ToArray();

        if (violations.Length > 0)
        {
            throw new InvalidDataException(
                "The CLDR archive is not the one cldr.pin describes, so nothing is generated from it:\n  " +
                string.Join("\n  ", violations));
        }

        return archive;
    }

    /// <summary>
    /// Whether a file states a version other than the pinned one, or null when it states the pinned
    /// one or none at all.
    /// </summary>
    internal static string? VersionViolation(string name, byte[] bytes)
    {
        var fileName = name[(name.LastIndexOf('/') + 1)..];

        if (string.Equals(fileName, "package.json", StringComparison.Ordinal))
        {
            using var package = JsonDocument.Parse(bytes);

            var version = package.RootElement.TryGetProperty("version", out var stated) ? stated.GetString() : null;

            return string.Equals(version, PinnedVersion, StringComparison.Ordinal)
                ? null
                : $"the package states version `{version}`, and the pin is {PinnedVersion}";
        }

        if (name.Contains("/supplemental/", StringComparison.Ordinal))
        {
            using var document = JsonDocument.Parse(bytes);

            var major = document.RootElement.TryGetProperty("supplemental", out var supplemental) &&
                supplemental.TryGetProperty("version", out var version) &&
                version.TryGetProperty("_cldrVersion", out var cldr)
                    ? cldr.GetString()
                    : null;

            return string.Equals(major, PinnedMajor, StringComparison.Ordinal)
                ? null
                : $"the file states CLDR version `{major}`, and the pin is CLDR {PinnedMajor}";
        }

        if (name.Contains("/common/uca/", StringComparison.Ordinal))
        {
            var expected = $"# UCA Version: {PinnedUcaVersion}";
            var lines = Decode(bytes).Split('\n', 12);

            return lines.Take(11).Any(line => string.Equals(line.TrimEnd('\r'), expected, StringComparison.Ordinal))
                ? null
                : $"the header does not state `{expected}`";
        }

        return null;
    }

    /// <summary>Strict UTF-8: a byte sequence that is not UTF-8 is a refusal, not a replacement character.</summary>
    internal static string Decode(byte[] bytes) => UnicodePin.Decode(bytes);
}
