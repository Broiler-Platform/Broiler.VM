// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// How a whole-suite run is cut into the processes that take it.
/// </summary>
/// <remarks>
/// <para>
/// <b>A whole run of the pinned checkout is tens of thousands of files and one process will not
/// finish it in a useful time</b>, so a whole run is <c>n</c> processes and a merge. That makes the
/// partition part of the evidence rather than an implementation detail: it has to be exhaustive, it
/// has to be disjoint, and the transcript has to say which rule produced it, or a reader cannot tell
/// a merged whole run from a merge of whatever happened to be lying in the directory.
/// </para>
/// <para>
/// <b>It is the harness's existing content-independent hash and not a new one.</b> Roadmap
/// section 14 asks for a test's shard to be a stable hash of its normalized path so that shard
/// membership does not move when the selection changes; <see cref="Sharding"/> already is that hash,
/// already refuses a shard index its count does not have, and is already pinned by the harness's own
/// checks. Slicing the ordinally sorted list by index would have been the obvious alternative and is
/// the wrong one: adding one file to the suite would move nearly every test to a different shard, so
/// no shard's transcript would be comparable with its own past.
/// </para>
/// <para>
/// <b>The partition is over FILES and never over variants.</b> A file is run in the variants its own
/// flags call for, and two shards that each took one variant of one file would report a path twice -
/// which a merge is right to refuse as a test scored by two shards. Keeping a file whole is what
/// makes the disjointness a property of the partition rather than of the runner.
/// </para>
/// </remarks>
internal static class Test262Partition
{
    /// <summary>How the partition is computed, in the words a transcript prints.</summary>
    internal const string Rule =
        "FNV-1a over the normalized suite-relative path, modulo the shard count, one whole file per shard";

    /// <summary>Reads <c>--shard k/n</c>, or says why it is not a shard.</summary>
    /// <remarks>
    /// <b>Absent means the whole selection and not shard zero.</b> A run with no
    /// <c>--shard</c> takes everything it selected, and its report says so where a merge and a
    /// reader can both see it; defaulting to shard zero would turn a plain run into a silent
    /// sixty-fourth of one.
    /// </remarks>
    internal static bool TryRead(string? text, out int index, out int count, out string failure)
    {
        index = Sharding.AllShards;
        count = 1;
        failure = string.Empty;

        if (text is null)
        {
            return true;
        }

        var parts = text.Split('/');

        if (parts.Length != 2 ||
            !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out index) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out count))
        {
            failure = $"`{text}` is not a shard: --shard takes `<k>/<n>`, with k below n";
            index = Sharding.AllShards;
            count = 1;
            return false;
        }

        if (!Sharding.Admits(index, count) || index == Sharding.AllShards)
        {
            failure = $"shard {text} is not a shard that count has";
            index = Sharding.AllShards;
            count = 1;
            return false;
        }

        return true;
    }

    /// <summary>The files one shard takes of a list, in the order the list gave them.</summary>
    internal static IReadOnlyList<string> Take(
        IReadOnlyList<string> files, int shardIndex, int shardCount)
    {
        if (shardIndex == Sharding.AllShards)
        {
            return files;
        }

        var mine = new List<string>();

        foreach (var file in files)
        {
            if (Sharding.ShardFor(file, shardCount) == shardIndex)
            {
                mine.Add(file);
            }
        }

        return mine;
    }

    /// <summary>The partition line a transcript and a report both carry.</summary>
    internal static string Describe(int shardIndex, int shardCount) =>
        shardIndex == Sharding.AllShards
            ? "every selected file, unsharded"
            : "shard " + shardIndex.ToString(CultureInfo.InvariantCulture) + " of " +
                shardCount.ToString(CultureInfo.InvariantCulture) + " by " + Rule;
}
