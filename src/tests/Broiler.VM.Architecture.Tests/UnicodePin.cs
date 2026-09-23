using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One file the Unicode pin names: where it is, how long it is and what it hashes to.</summary>
/// <param name="Kind"><c>file</c> for a UCD file or the licence text, <c>spec-table</c> for an
/// ECMAScript property table.</param>
/// <param name="Name">The path as the pin writes it.</param>
/// <param name="RepositoryPath">The same path relative to the repository root.</param>
internal sealed record UnicodePinEntry(string Kind, string Name, string RepositoryPath, long Bytes, string Sha256);

/// <summary>
/// The Unicode data pin, <c>src/tests/unicode/pins/unicode.pin</c>, parsed the way the generator
/// reads it, and the checks that stand between an archived byte and a generated table.
/// </summary>
/// <remarks>
/// <para>
/// <b>The generator reads nothing this type has not verified.</b> Decision JSD-0031 section 5 asks
/// that the generator "reads only the pinned files, and refuses a file whose hash differs or whose
/// header line does not name version 17.0.0". So the inputs are handed out as a dictionary built
/// here from the pin's own list, after every entry's length, SHA-256 and header have been compared
/// with what the pin records; a file the pin does not name is never opened, and a verification
/// failure is an exception rather than a warning the generator could run past.
/// </para>
/// <para>
/// <b>Each check is a pure function over bytes</b> so that rule N22's witnesses can hand it a
/// changed byte or a header naming another version without editing the archive.
/// </para>
/// </remarks>
internal sealed class UnicodePin
{
    /// <summary>The pin, relative to the repository root.</summary>
    internal const string PinPath = "src/tests/unicode/pins/unicode.pin";

    /// <summary>The directory the pin's <c>file</c> paths are relative to.</summary>
    internal const string PinDirectory = "src/tests/unicode/pins";

    /// <summary>The one version this repository's tables are generated from.</summary>
    internal const string PinnedVersion = "17.0.0";

    private UnicodePin(IReadOnlyDictionary<string, string> values, IReadOnlyList<UnicodePinEntry> entries)
    {
        Values = values;
        Entries = entries;
    }

    /// <summary>The single-valued keys: <c>version</c>, <c>source</c>, <c>archived</c> and the rest.</summary>
    internal IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>Every file the pin names, in the order it names them.</summary>
    internal IReadOnlyList<UnicodePinEntry> Entries { get; }

    /// <summary>The version line, or the empty string when there is none.</summary>
    internal string Version => Values.TryGetValue("version", out var version) ? version : string.Empty;

    /// <summary>The checked-in pin.</summary>
    internal static UnicodePin Load() =>
        Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, PinPath)));

    /// <summary>Parses pin text: <c>key value</c> lines, <c>#</c> comments, repeated file lines.</summary>
    internal static UnicodePin Parse(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var entries = new List<UnicodePinEntry>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] is "file" or "spec-table")
            {
                if (parts.Length != 4 || !long.TryParse(parts[2], out var bytes))
                {
                    throw new InvalidDataException($"unicode.pin: `{line}` is not `{parts[0]} <path> <bytes> <sha256>`");
                }

                var repositoryPath = parts[0] == "file" ? $"{PinDirectory}/{parts[1]}" : parts[1];

                entries.Add(new UnicodePinEntry(parts[0], parts[1], repositoryPath, bytes, parts[3].ToLowerInvariant()));
                continue;
            }

            if (parts.Length < 2 || !values.TryAdd(parts[0], string.Join(' ', parts.Skip(1))))
            {
                throw new InvalidDataException($"unicode.pin: `{line}` is empty or repeats a key");
            }
        }

        return new UnicodePin(values, entries);
    }

    /// <summary>The entry for a UCD-relative or pin-relative name, e.g. <c>ucd-17.0.0/Scripts.txt</c>.</summary>
    internal UnicodePinEntry Entry(string name) =>
        Entries.SingleOrDefault(entry => string.Equals(entry.Name, name, StringComparison.Ordinal))
        ?? throw new InvalidDataException($"unicode.pin names no file `{name}`");

    /// <summary>Reads every pinned file off disk, by the pin's own list and nothing else.</summary>
    internal IReadOnlyDictionary<string, byte[]> ReadArchive() =>
        Entries.ToDictionary(
            static entry => entry.Name,
            static entry => File.ReadAllBytes(Path.Combine(
                ComponentGraph.Root, entry.RepositoryPath.Replace('/', Path.DirectorySeparatorChar))),
            StringComparer.Ordinal);

    /// <summary>
    /// Every way the given bytes differ from what the pin records: a missing file, a length, a
    /// hash, or a header that does not name the pinned version. Empty when the archive is the one
    /// the pin describes.
    /// </summary>
    internal IEnumerable<string> Violations(IReadOnlyDictionary<string, byte[]> archive)
    {
        if (!string.Equals(Version, PinnedVersion, StringComparison.Ordinal))
        {
            yield return $"unicode.pin names version `{Version}`, and the tables are generated from {PinnedVersion} only";
        }

        if (!Values.TryGetValue("archived", out var archived) || archived != "yes")
        {
            yield return "unicode.pin does not say `archived yes`, and the generator reads only archived bytes";
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

            if (HeaderViolation(entry.Name, bytes, PinnedVersion) is { } header)
            {
                yield return $"{entry.RepositoryPath}: {header}";
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
                "The Unicode archive is not the one unicode.pin describes, so nothing is generated from it:\n  " +
                string.Join("\n  ", violations));
        }

        return archive;
    }

    /// <summary>
    /// Whether a UCD file's header names the version, or null when it does. A file with no version
    /// header - <c>UnicodeData.txt</c>, the licence text and the ECMAScript tables - is held by its
    /// hash alone and answers null.
    /// </summary>
    /// <remarks>
    /// Every UCD file but one opens <c># Name-17.0.0.txt</c>. <c>emoji-data.txt</c> opens with its
    /// bare name and states <c># Version: 17.0</c> a few lines later, so that is what is read there.
    /// </remarks>
    internal static string? HeaderViolation(string name, byte[] bytes, string version)
    {
        var fileName = name[(name.LastIndexOf('/') + 1)..];

        if (!name.StartsWith("ucd-", StringComparison.Ordinal) ||
            string.Equals(fileName, "UnicodeData.txt", StringComparison.Ordinal))
        {
            return null;
        }

        var lines = Decode(bytes).Split('\n', 12);

        if (string.Equals(fileName, "emoji-data.txt", StringComparison.Ordinal))
        {
            var shortVersion = version[..version.LastIndexOf('.')];

            return lines.Take(11).Any(line => string.Equals(line, $"# Version: {shortVersion}", StringComparison.Ordinal))
                ? null
                : $"the header does not state `# Version: {shortVersion}`";
        }

        var expected = $"# {Path.GetFileNameWithoutExtension(fileName)}-{version}.txt";

        return string.Equals(lines[0], expected, StringComparison.Ordinal)
            ? null
            : $"the first line is `{lines[0]}`, and a {version} file opens `{expected}`";
    }

    /// <summary>Strict UTF-8: a byte sequence that is not UTF-8 is a refusal, not a replacement character.</summary>
    internal static string Decode(byte[] bytes) =>
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(bytes);
}

/// <summary>One row of an ECMAScript property table: a name the language admits and its canonical name.</summary>
internal sealed record UnicodeSpecName(string Name, string Canonical);

/// <summary>
/// The three ECMAScript property tables, read out of the archived HTML, and the cross-check
/// against <c>PropertyAliases.txt</c> that JSD-0031 section 5 asks for.
/// </summary>
internal static class UnicodeSpecTables
{
    internal const string NonBinary = "src/Broiler.VM.Profile.JavaScript/docs/specification/table-nonbinary-unicode-properties.html";
    internal const string Binary = "src/Broiler.VM.Profile.JavaScript/docs/specification/table-binary-unicode-properties.html";
    internal const string Strings = "src/Broiler.VM.Profile.JavaScript/docs/specification/table-binary-unicode-properties-of-strings.html";

    /// <summary>
    /// The three binary names the language takes from UTS #18 rather than from the UCD. The table
    /// links each of them to UTS #18's General_Category section and <c>PropertyAliases.txt</c>
    /// lists none of them, so they are the one place the cross-check has nothing to compare.
    /// </summary>
    internal static readonly string[] Uts18Names = ["ASCII", "Any", "Assigned"];

    /// <summary>
    /// Every row of one table: the first cell is the name, and a row that opens a group also names
    /// the canonical name, which the rows below it inherit through <c>rowspan</c>.
    /// </summary>
    internal static IReadOnlyList<UnicodeSpecName> Rows(string html)
    {
        var rows = new List<UnicodeSpecName>();
        string? canonical = null;

        foreach (Match row in Regex.Matches(html, "<tr>(.*?)</tr>", RegexOptions.Singleline))
        {
            var cells = Regex.Matches(row.Groups[1].Value, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline)
                .Select(static cell => Cell(cell.Groups[1].Value))
                .ToArray();

            if (cells.Length == 0)
            {
                continue;
            }

            if (cells.Length > 1)
            {
                canonical = cells[1];
            }

            rows.Add(new UnicodeSpecName(
                cells[0],
                canonical ?? cells[0]));
        }

        return rows;

        static string Cell(string html)
        {
            var text = Regex.Replace(html, "<[^>]+>", string.Empty).Trim();

            if (text.Length < 2 || text[0] != '`' || text[^1] != '`')
            {
                throw new InvalidDataException($"a property table cell reads `{text}`, not a `name`");
            }

            return text[1..^1];
        }
    }

    /// <summary>
    /// Every disagreement between the names the ECMAScript tables admit and the names
    /// <c>PropertyAliases.txt</c> gives the same property. Empty when they agree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The direction checked is the one that can be wrong in a way that matters: every name the
    /// language admits must be a UCD name or alias OF THE PROPERTY THE TABLE SAYS IT IS, and the
    /// table's canonical column must be the UCD's long name. The other direction is not a
    /// disagreement - the UCD lists aliases the language does not admit (<c>WSpace</c> beside
    /// <c>White_Space</c> and <c>space</c>, for one), and the language's answer to those is a
    /// SyntaxError.
    /// </para>
    /// <para>
    /// The string-property table is read for one fact only: none of its names is admitted as a
    /// code point property, because the <c>v</c> flag that admits them is not supported.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> Disagreements(
        IReadOnlyList<UnicodeSpecName> nonBinary,
        IReadOnlyList<UnicodeSpecName> binary,
        IReadOnlyList<UnicodeSpecName> strings,
        string propertyAliases)
    {
        var aliases = PropertyAliases(propertyAliases);

        foreach (var row in nonBinary.Concat(binary))
        {
            if (Uts18Names.Contains(row.Canonical, StringComparer.Ordinal))
            {
                if (!string.Equals(row.Name, row.Canonical, StringComparison.Ordinal))
                {
                    yield return $"the table gives `{row.Canonical}` an alias `{row.Name}`, and UTS #18 names it alone";
                }

                continue;
            }

            if (!aliases.TryGetValue(row.Canonical, out var names))
            {
                yield return $"the table's canonical name `{row.Canonical}` is not a long name in PropertyAliases.txt";
                continue;
            }

            if (!names.Contains(row.Name))
            {
                yield return $"the table admits `{row.Name}` for `{row.Canonical}`, and PropertyAliases.txt gives that property " +
                    $"only {string.Join(", ", names.Order(StringComparer.Ordinal).Select(static name => $"`{name}`"))}";
            }
        }

        var admitted = nonBinary.Concat(binary).Select(static row => row.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var row in strings.Where(row => admitted.Contains(row.Name)))
        {
            yield return $"`{row.Name}` is a property of strings, and the code point tables also admit it";
        }

        foreach (var duplicate in nonBinary.Concat(binary).GroupBy(static row => row.Name, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            yield return $"the tables admit `{duplicate.Key}` more than once";
        }
    }

    /// <summary><c>PropertyAliases.txt</c>: each long name, with every name the file gives that property.</summary>
    internal static IReadOnlyDictionary<string, HashSet<string>> PropertyAliases(string text)
    {
        var byLongName = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var fields in UnicodeDataFile.Records(text))
        {
            if (fields.Length < 2)
            {
                continue;
            }

            var names = byLongName.TryGetValue(fields[1], out var found)
                ? found
                : byLongName[fields[1]] = new HashSet<string>(StringComparer.Ordinal);

            foreach (var name in fields)
            {
                names.Add(name);
            }
        }

        return byLongName;
    }
}
