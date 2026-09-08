// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// A per-file digest another process in this same run already computed, offered to a shard so the
/// run reads the checkout once instead of once per shard.
/// </summary>
/// <remarks>
/// <para>
/// <b>What this is for, in one figure.</b> A whole-suite run verifies the checkout against the
/// retained pin before it scores anything, and every shard verifies it again for the reason
/// <c>Test262Command.Verify</c> records: a shard that reported a revision it had not itself read
/// would be certifying its own input. The pinned checkout is 56,560 files and 232 megabytes, so a
/// twelve-shard run reads and hashes that tree thirteen times, and twelve of the thirteen readings
/// answer a question the first one already answered - on the same machine, in the same run, minutes
/// apart.
/// </para>
/// <para>
/// <b>WHY THE VERIFICATION IS NOT SKIPPED, AND WHY IT NEVER WILL BE.</b> This cache is keyed on a
/// file's SIZE and its MODIFICATION TIME, and neither is evidence about content: both are writable
/// by anyone who can write the file, an edit that preserves them takes one line of script, and a
/// filesystem that reports seconds cannot see a change made inside the same second. So the cache is
/// not a cheaper verification and this file does not offer one. It is a way to spend ONE full
/// reading per RUN instead of one per SHARD - the driver reads every byte of every file, computes
/// the digest, compares it against the retained pin, and only then writes what it read here. A cache
/// this harness was handed without such a reading having happened is a cache whose declared content
/// digest does not match the pin, and that is refused below rather than believed.
/// </para>
/// <para>
/// <b>Every disagreement re-reads the file rather than failing the run.</b> A file whose size or
/// modification time differs from the cache is hashed from its bytes, an entry the cache does not
/// hold is hashed from its bytes, and a cache whose header or digest is wrong is ignored whole. That
/// is the only safe direction: a cache is an optimisation, and an optimisation that can make a run
/// say something a full reading would not have said is a defect wearing a performance argument.
/// </para>
/// </remarks>
internal static class Test262DigestCache
{
    /// <summary>The header a cache file carries, naming its format.</summary>
    internal const string Header = "# broiler-js-conformance suite digest cache 1";

    /// <summary>The key a cache states the digest its writer verified under.</summary>
    internal const string DigestKey = "# content-sha256 ";

    /// <summary>How a run says it read the checkout rather than a cache.</summary>
    internal const string ReadInFull = "the checkout was read in full";

    /// <summary>
    /// Every file the suite holds with its hash, taking a hash from the cache where the cache's
    /// size and modification time still describe the file on disk.
    /// </summary>
    /// <param name="root">The checkout.</param>
    /// <param name="cachePath">The cache, or <c>null</c> to read every byte.</param>
    /// <param name="expected">
    /// The content digest the retained pin names. A cache written under any other digest is a cache
    /// from another revision or another run and is ignored whole.
    /// </param>
    /// <param name="note">One line saying what was reused and what was read, for the transcript.</param>
    internal static IReadOnlyList<SuiteFile> Files(
        string root, string? cachePath, string expected, out string note)
    {
        if (cachePath is null)
        {
            note = ReadInFull;
            return Suite.Files(root);
        }

        var entries = Read(cachePath, expected, out var why);

        if (entries is null)
        {
            note = ReadInFull + "; the cache at " + cachePath + " was not used because " + why;
            return Suite.Files(root);
        }

        var reused = 0;
        var read = 0;

        var files = Suite.Files(root, (full, relative) =>
        {
            if (entries.TryGetValue(relative, out var entry) && Describes(full, entry))
            {
                reused++;
                return entry.Sha256;
            }

            read++;
            return Suite.Sha256(File.ReadAllBytes(full));
        });

        note =
            reused.ToString(CultureInfo.InvariantCulture) + " file(s) taken from " + cachePath +
            " on size and modification time, " + read.ToString(CultureInfo.InvariantCulture) +
            " read in full; the digest below is still computed over all " +
            files.Count.ToString(CultureInfo.InvariantCulture) + " and compared to the pin";

        return files;
    }

    /// <summary>One cached file: what it was, and what its bytes hashed to when it was read.</summary>
    private sealed record Entry(long Length, long ModifiedSeconds, string Sha256);

    /// <summary>Whether the file on disk is still the one this entry was taken from.</summary>
    /// <remarks>
    /// Whole seconds, because the writer is a Python driver and this is C#: two runtimes' idea of a
    /// sub-second timestamp differ by rounding on filesystems that have one and by everything on
    /// filesystems that do not, and a key that disagreed with itself across languages would send
    /// every file back to a full read and cost the reading it was meant to save. The coarseness is
    /// affordable precisely because the key is not the verification.
    /// </remarks>
    private static bool Describes(string path, Entry entry)
    {
        var info = new FileInfo(path);

        return info.Length == entry.Length &&
            new DateTimeOffset(File.GetLastWriteTimeUtc(path), TimeSpan.Zero).ToUnixTimeSeconds() ==
                entry.ModifiedSeconds;
    }

    /// <summary>Reads a cache, or says why it is not one this run may take anything from.</summary>
    private static Dictionary<string, Entry>? Read(string path, string expected, out string why)
    {
        if (!File.Exists(path))
        {
            why = "there is no file there";
            return null;
        }

        var entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
        var seenHeader = false;
        var declared = string.Empty;

        foreach (var line in File.ReadLines(path))
        {
            if (line.Length == 0)
            {
                continue;
            }

            if (line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);

                if (line.StartsWith(DigestKey, StringComparison.Ordinal))
                {
                    declared = line[DigestKey.Length..].Trim();
                }

                continue;
            }

            // `<length> <modifiedSeconds> <sha256> <path>`, the path last and unsplit, because a
            // suite path may hold a space and a reader that split on every space would drop the
            // file rather than the space.
            var first = line.IndexOf(' ', StringComparison.Ordinal);
            var second = first < 0 ? -1 : line.IndexOf(' ', first + 1);
            var third = second < 0 ? -1 : line.IndexOf(' ', second + 1);

            if (third < 0 ||
                !long.TryParse(line[..first], NumberStyles.None, CultureInfo.InvariantCulture, out var length) ||
                !long.TryParse(
                    line[(first + 1)..second], NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out var modified))
            {
                why = "`" + line + "` is not `<length> <modifiedSeconds> <sha256> <path>`";
                return null;
            }

            entries[Suite.Normalize(line[(third + 1)..])] =
                new Entry(length, modified, line[(second + 1)..third]);
        }

        if (!seenHeader)
        {
            why = "it does not open with `" + Header + "`";
            return null;
        }

        // THE ONE CHECK THAT MAKES THE CACHE ANSWERABLE. A cache states the content digest its
        // writer verified against the pin; a cache naming any other digest was written for another
        // revision, or by a run whose verification did not agree with this run's pin, and taking a
        // hash out of it would let a shard report a revision on the strength of a stale file.
        if (!string.Equals(declared, expected, StringComparison.Ordinal))
        {
            why =
                "it was written under content digest `" +
                (declared.Length == 0 ? "(none)" : declared) + "` and this run's pin names `" +
                expected + "`";

            return null;
        }

        why = string.Empty;
        return entries;
    }
}
