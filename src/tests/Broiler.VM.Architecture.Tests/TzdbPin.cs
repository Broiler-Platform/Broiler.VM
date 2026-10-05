using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One file inside the archived tzdb tarball the pin names: its name, length and SHA-256.</summary>
internal sealed record TzdbPinMember(string Name, long Bytes, string Sha256);

/// <summary>
/// The IANA Time Zone Database pin, <c>src/tests/tzdb/pins/tzdb.pin</c>, parsed the way the generator
/// reads it, and the checks that stand between an archived byte and anything made from it (rule N29).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is <see cref="CldrPin"/>'s shape, over one file.</b> IANA publishes each release as a data
/// tarball, so the tarball is archived as published and the pin records its digest, as the retained
/// test262 pin records its archive's. The generator then reads only the members the pin names, each
/// checked against its own length and SHA-256 after extraction, so a reader sees which files the
/// tables come from without unpacking anything.
/// </para>
/// <para>
/// <b>The version is read where the release states it</b>: the <c>version</c> member, one line.
/// </para>
/// </remarks>
internal sealed class TzdbPin
{
    /// <summary>The pin, relative to the repository root.</summary>
    internal const string PinPath = "src/tests/tzdb/pins/tzdb.pin";

    /// <summary>The directory the pin's archive is in.</summary>
    internal const string PinDirectory = "src/tests/tzdb/pins";

    /// <summary>The one tzdb release this repository's time zone data is taken from.</summary>
    internal const string PinnedVersion = "2026e";

    private TzdbPin(IReadOnlyDictionary<string, string> values, IReadOnlyList<TzdbPinMember> members)
    {
        Values = values;
        Members = members;
    }

    /// <summary>The single-valued keys: <c>version</c>, <c>archive</c> and the rest.</summary>
    internal IReadOnlyDictionary<string, string> Values { get; }

    /// <summary>Every member the pin names, in the order it names them.</summary>
    internal IReadOnlyList<TzdbPinMember> Members { get; }

    /// <summary>The version line, or the empty string when there is none.</summary>
    internal string Version => Values.TryGetValue("version", out var version) ? version : string.Empty;

    /// <summary>The archive's file name, beside the pin.</summary>
    internal string ArchiveName => Values.TryGetValue("archive", out var archive) ? archive.Split(' ')[0] : string.Empty;

    /// <summary>The checked-in pin.</summary>
    internal static TzdbPin Load() =>
        Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, PinPath)));

    /// <summary>Parses pin text: <c>key value</c> lines, <c>#</c> comments, repeated <c>member</c> lines.</summary>
    internal static TzdbPin Parse(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var members = new List<TzdbPinMember>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] == "member")
            {
                if (parts.Length != 4 || !long.TryParse(parts[2], out var bytes))
                {
                    throw new InvalidDataException($"tzdb.pin: `{line}` is not `member <name> <bytes> <sha256>`");
                }

                members.Add(new TzdbPinMember(parts[1], bytes, parts[3].ToLowerInvariant()));
                continue;
            }

            if (parts.Length < 2 || !values.TryAdd(parts[0], string.Join(' ', parts.Skip(1))))
            {
                throw new InvalidDataException($"tzdb.pin: `{line}` is empty or repeats a key");
            }
        }

        return new TzdbPin(values, members);
    }

    /// <summary>The archived tarball's bytes, off disk.</summary>
    internal byte[] ReadTarball() =>
        File.ReadAllBytes(Path.Combine(ComponentGraph.Root, PinDirectory, ArchiveName));

    /// <summary>
    /// The pinned members of the archived tarball, by the pin's own list and nothing else, or an
    /// exception naming every way the tarball or a member is not the pinned one.
    /// </summary>
    internal IReadOnlyDictionary<string, byte[]> ReadArchive() => Extract(ReadTarball());

    /// <summary>The members of a tarball the pin names, after the tarball's own digest is checked.</summary>
    internal IReadOnlyDictionary<string, byte[]> Extract(byte[] tarball)
    {
        var refusals = TarballViolations(tarball).ToArray();

        if (refusals.Length > 0)
        {
            throw Refusal(refusals);
        }

        var wanted = Members.Select(static member => member.Name).ToHashSet(StringComparer.Ordinal);
        var archive = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        using var gzip = new GZipStream(new MemoryStream(tarball, writable: false), CompressionMode.Decompress);
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

            if (!archive.TryAdd(entry.Name, copy.ToArray()))
            {
                throw Refusal([$"{ArchiveName} holds `{entry.Name}` twice"]);
            }
        }

        return Verified(archive);
    }

    /// <summary>Every way the tarball's bytes differ from what the pin records.</summary>
    internal IEnumerable<string> TarballViolations(byte[] tarball)
    {
        if (!string.Equals(Version, PinnedVersion, StringComparison.Ordinal))
        {
            yield return $"tzdb.pin names version `{Version}`, and the time zone data is taken from {PinnedVersion} only";
        }

        if (!Values.TryGetValue("archived", out var archived) || archived != "yes")
        {
            yield return "tzdb.pin does not say `archived yes`, and nothing reads unarchived bytes";
        }

        var recorded = Values.TryGetValue("archive", out var archive) ? archive.Split(' ') : [];

        if (recorded.Length != 2 || !long.TryParse(recorded[1], out var length))
        {
            yield return "tzdb.pin does not record `archive <file> <bytes>`";
            yield break;
        }

        if (tarball.LongLength != length)
        {
            yield return $"{ArchiveName} is {tarball.LongLength} bytes, and the pin records {length}";
        }

        var digest = Convert.ToHexStringLower(SHA256.HashData(tarball));

        if (!Values.TryGetValue("archive-sha256", out var sha256) || !string.Equals(digest, sha256, StringComparison.Ordinal))
        {
            yield return $"{ArchiveName} hashes to {digest}, and the pin records {sha256}";
        }

        var sha512 = Convert.ToBase64String(SHA512.HashData(tarball));

        if (!Values.TryGetValue("archive-sha512", out var pinned512) || !string.Equals(sha512, pinned512, StringComparison.Ordinal))
        {
            yield return $"{ArchiveName}'s SHA-512 is {sha512}, and the pin records {pinned512}";
        }
    }

    /// <summary>
    /// Every way extracted members differ from what the pin records: a missing member, a length, a
    /// hash, or a stated version that is not the pinned one. Empty when they are the pinned ones.
    /// </summary>
    internal IEnumerable<string> Violations(IReadOnlyDictionary<string, byte[]> archive)
    {
        foreach (var member in Members)
        {
            if (!archive.TryGetValue(member.Name, out var bytes))
            {
                yield return $"{ArchiveName} has no member `{member.Name}`, which the pin names";
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

        if (archive.TryGetValue("version", out var version) &&
            !string.Equals(Decode(version).Trim(), PinnedVersion, StringComparison.Ordinal))
        {
            yield return $"the release states version `{Decode(version).Trim()}`, and the pin is {PinnedVersion}";
        }
    }

    /// <summary>The verified members, or an exception naming every way they are not the pinned ones.</summary>
    internal IReadOnlyDictionary<string, byte[]> Verified(IReadOnlyDictionary<string, byte[]> archive)
    {
        var violations = Violations(archive).ToArray();

        if (violations.Length > 0)
        {
            throw Refusal(violations);
        }

        return archive;
    }

    private static InvalidDataException Refusal(IEnumerable<string> violations) =>
        new("The tzdb archive is not the one tzdb.pin describes, so nothing is generated from it:\n  " +
            string.Join("\n  ", violations));

    /// <summary>Strict UTF-8: a byte sequence that is not UTF-8 is a refusal, not a replacement character.</summary>
    internal static string Decode(byte[] bytes) => UnicodePin.Decode(bytes);
}
