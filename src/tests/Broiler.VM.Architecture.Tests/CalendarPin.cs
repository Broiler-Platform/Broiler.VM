using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One archived crate the pin names: its file name, length, SHA-256 and SHA-512.</summary>
internal sealed record CalendarPinArchive(string Name, long Bytes, string Sha256, string Sha512);

/// <summary>One file inside an archived crate: the crate, its path, its length and SHA-256.</summary>
internal sealed record CalendarPinMember(string Archive, string Name, long Bytes, string Sha256);

/// <summary>
/// The calendar data pin, <c>src/tests/calendars/pins/calendars.pin</c>, parsed the way the generator
/// reads it, and the checks that stand between an archived byte and anything made from it (rule N31).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is <see cref="TzdbPin"/>'s shape, over two archives.</b> crates.io publishes each crate
/// version as a gzipped tarball whose SHA-256 its index states, so each tarball is archived as
/// published and the pin records its digests; the generator then reads only the members the pin
/// names, each checked against its own length and SHA-256 after extraction.
/// </para>
/// <para>
/// <b>The versions are read where each crate states them</b>: its <c>Cargo.toml</c>'s
/// <c>[package]</c> name and version.
/// </para>
/// </remarks>
internal sealed class CalendarPin
{
    /// <summary>The pin, relative to the repository root.</summary>
    internal const string PinPath = "src/tests/calendars/pins/calendars.pin";

    /// <summary>The directory the pin's archives are in.</summary>
    internal const string PinDirectory = "src/tests/calendars/pins";

    /// <summary>The crate versions this repository's calendar tables are taken from.</summary>
    internal static readonly IReadOnlyDictionary<string, string> PinnedVersions = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["icu_calendar"] = "2.3.0",
        ["calendrical_calculations"] = "0.2.4",
    };

    private CalendarPin(IReadOnlyDictionary<string, string> values, IReadOnlyList<CalendarPinArchive> archives, IReadOnlyList<CalendarPinMember> members)
    {
        Values = values;
        Archives = archives;
        Members = members;
    }

    /// <summary>The single-valued keys.</summary>
    internal IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>Every archive the pin names, in its order.</summary>
    internal IReadOnlyList<CalendarPinArchive> Archives { get; }

    /// <summary>Every member the pin names, in its order.</summary>
    internal IReadOnlyList<CalendarPinMember> Members { get; }

    /// <summary>The checked-in pin.</summary>
    internal static CalendarPin Load() =>
        Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, PinPath)));

    /// <summary>
    /// Parses pin text: <c>key value</c> lines, <c>#</c> comments, repeated <c>archive</c>,
    /// <c>upstream</c>, <c>licence</c> and <c>member</c> lines.
    /// </summary>
    internal static CalendarPin Parse(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var archives = new List<CalendarPinArchive>();
        var members = new List<CalendarPinMember>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            switch (parts[0])
            {
                case "archive":
                    if (parts.Length != 5 || !long.TryParse(parts[2], out var length))
                    {
                        throw new InvalidDataException($"calendars.pin: `{line}` is not `archive <file> <bytes> <sha256> <sha512>`");
                    }

                    archives.Add(new CalendarPinArchive(parts[1], length, parts[3].ToLowerInvariant(), parts[4]));
                    continue;
                case "member":
                    if (parts.Length != 5 || !long.TryParse(parts[3], out var bytes))
                    {
                        throw new InvalidDataException($"calendars.pin: `{line}` is not `member <archive> <name> <bytes> <sha256>`");
                    }

                    members.Add(new CalendarPinMember(parts[1], parts[2], bytes, parts[4].ToLowerInvariant()));
                    continue;
                case "upstream":
                case "licence":
                    if (parts.Length != 3 || !values.TryAdd(parts[0] + " " + parts[1], parts[2]))
                    {
                        throw new InvalidDataException($"calendars.pin: `{line}` is not `{parts[0]} <archive> <value>` or repeats one");
                    }

                    continue;
            }

            if (parts.Length < 2 || !values.TryAdd(parts[0], string.Join(' ', parts.Skip(1))))
            {
                throw new InvalidDataException($"calendars.pin: `{line}` is empty or repeats a key");
            }
        }

        return new CalendarPin(values, archives, members);
    }

    /// <summary>An archived crate's bytes, off disk.</summary>
    internal byte[] ReadArchiveBytes(string name) =>
        File.ReadAllBytes(Path.Combine(ComponentGraph.Root, PinDirectory, name));

    /// <summary>
    /// The pinned members of the archived crates, by the pin's own list and nothing else, or an
    /// exception naming every way an archive or a member is not the pinned one.
    /// </summary>
    internal IReadOnlyDictionary<string, byte[]> ReadArchive() =>
        Extract(Archives.ToDictionary(static archive => archive.Name, archive => ReadArchiveBytes(archive.Name), StringComparer.Ordinal));

    /// <summary>The members of the crates the pin names, after each crate's own digests are checked.</summary>
    internal IReadOnlyDictionary<string, byte[]> Extract(IReadOnlyDictionary<string, byte[]> crates)
    {
        var refusals = ArchiveViolations(crates).ToArray();

        if (refusals.Length > 0)
        {
            throw Refusal(refusals);
        }

        var extracted = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        foreach (var archive in Archives)
        {
            var wanted = Members.Where(member => member.Archive == archive.Name).Select(static member => member.Name).ToHashSet(StringComparer.Ordinal);

            using var gzip = new GZipStream(new MemoryStream(crates[archive.Name], writable: false), CompressionMode.Decompress);
            using var reader = new TarReader(gzip);

            while (reader.GetNextEntry() is { } entry)
            {
                if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile) ||
                    !wanted.Contains(entry.Name) || entry.DataStream is null)
                {
                    continue;
                }

                using var copy = new MemoryStream();
                entry.DataStream.CopyTo(copy);

                if (!extracted.TryAdd(entry.Name, copy.ToArray()))
                {
                    throw Refusal([$"{archive.Name} holds `{entry.Name}` twice"]);
                }
            }
        }

        return Verified(extracted);
    }

    /// <summary>Every way the crates' bytes differ from what the pin records.</summary>
    internal IEnumerable<string> ArchiveViolations(IReadOnlyDictionary<string, byte[]> crates)
    {
        if (!Values.TryGetValue("archived", out var archived) || archived != "yes")
        {
            yield return "calendars.pin does not say `archived yes`, and nothing reads unarchived bytes";
        }

        if (Archives.Count != PinnedVersions.Count)
        {
            yield return $"calendars.pin names {Archives.Count} archives, and the calendar data is taken from {PinnedVersions.Count}";
        }

        foreach (var archive in Archives)
        {
            if (!PinnedVersions.Any(pair => archive.Name == $"{pair.Key}-{pair.Value}.crate"))
            {
                yield return $"calendars.pin names `{archive.Name}`, which is not a pinned crate version";
            }

            if (!crates.TryGetValue(archive.Name, out var bytes))
            {
                yield return $"`{archive.Name}` is missing";
                continue;
            }

            if (bytes.LongLength != archive.Bytes)
            {
                yield return $"{archive.Name} is {bytes.LongLength} bytes, and the pin records {archive.Bytes}";
            }

            var sha256 = Convert.ToHexStringLower(SHA256.HashData(bytes));

            if (!string.Equals(sha256, archive.Sha256, StringComparison.Ordinal))
            {
                yield return $"{archive.Name} hashes to {sha256}, and the pin records {archive.Sha256}";
            }

            var sha512 = Convert.ToBase64String(SHA512.HashData(bytes));

            if (!string.Equals(sha512, archive.Sha512, StringComparison.Ordinal))
            {
                yield return $"{archive.Name}'s SHA-512 is {sha512}, and the pin records {archive.Sha512}";
            }
        }
    }

    /// <summary>
    /// Every way extracted members differ from what the pin records: a missing member, a length, a
    /// hash, or a crate stating another name or version. Empty when they are the pinned ones.
    /// </summary>
    internal IEnumerable<string> Violations(IReadOnlyDictionary<string, byte[]> members)
    {
        foreach (var member in Members)
        {
            if (!members.TryGetValue(member.Name, out var bytes))
            {
                yield return $"{member.Archive} has no member `{member.Name}`, which the pin names";
                continue;
            }

            if (bytes.LongLength != member.Bytes)
            {
                yield return $"`{member.Name}` is {bytes.LongLength} bytes, and the pin records {member.Bytes}";
            }

            var digest = Convert.ToHexStringLower(SHA256.HashData(bytes));

            if (!string.Equals(digest, member.Sha256, StringComparison.Ordinal))
            {
                yield return $"`{member.Name}` hashes to {digest}, and the pin records {member.Sha256}";
            }
        }

        foreach (var (crate, version) in PinnedVersions)
        {
            var manifest = $"{crate}-{version}/Cargo.toml";

            if (!members.TryGetValue(manifest, out var bytes))
            {
                continue;
            }

            var (name, stated) = PackageOf(Decode(bytes));

            if (name != crate || stated != version)
            {
                yield return $"{manifest} states the package `{name}` {stated}, and the pin is {crate} {version}";
            }
        }
    }

    /// <summary>The verified members, or an exception naming every way they are not the pinned ones.</summary>
    internal IReadOnlyDictionary<string, byte[]> Verified(IReadOnlyDictionary<string, byte[]> members)
    {
        var violations = Violations(members).ToArray();

        if (violations.Length > 0)
        {
            throw Refusal(violations);
        }

        return members;
    }

    /// <summary>The name and version a <c>Cargo.toml</c>'s <c>[package]</c> table states.</summary>
    internal static (string Name, string Version) PackageOf(string manifest)
    {
        var name = string.Empty;
        var version = string.Empty;
        var inPackage = false;

        foreach (var raw in manifest.Split('\n'))
        {
            var line = raw.Trim();

            if (line.StartsWith('['))
            {
                inPackage = line == "[package]";
                continue;
            }

            if (!inPackage)
            {
                continue;
            }

            if (line.StartsWith("name = \"", StringComparison.Ordinal))
            {
                name = line[8..^1];
            }
            else if (line.StartsWith("version = \"", StringComparison.Ordinal))
            {
                version = line[11..^1];
            }
        }

        return (name, version);
    }

    private static InvalidDataException Refusal(IEnumerable<string> violations) =>
        new("The calendar archives are not the ones calendars.pin describes, so nothing is generated from them:\n  " +
            string.Join("\n  ", violations));

    /// <summary>Strict UTF-8: a byte sequence that is not UTF-8 is a refusal, not a replacement character.</summary>
    internal static string Decode(byte[] bytes) => UnicodePin.Decode(bytes);
}
