using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// How many tests each stage of the selection pipeline let through.
/// </summary>
/// <param name="Candidates">Everything discovery found, before any filter.</param>
/// <param name="KnownIncorrect">Excluded by name, because the test itself is wrong at this revision.</param>
/// <param name="OutOfScope">Excluded by the scope patterns this run was given.</param>
/// <param name="FeatureExcluded">
/// Excluded because the test claims a feature the suite itself declares a proposal.
/// </param>
/// <param name="FeatureFiltered">Excluded by the feature patterns this run was given.</param>
/// <param name="NegativeWithheld">Negative-metadata tests this run did not opt into.</param>
/// <param name="Unselectable">Tests this profile has no way to present at all.</param>
/// <param name="Selected">What survived every filter, before sharding.</param>
/// <param name="Sharded">What this shard took of that.</param>
/// <remarks>
/// <para>
/// <b>Emitted stage by stage rather than as one number.</b> Roadmap section 14 asks the candidate
/// count and the pre-sharding selected count to be separate from a shard's executed count, because
/// that is what lets a merge prove the shards covered the whole selection instead of a subset. A
/// single "selected" figure cannot: a filter that quietly widened and a discovery that quietly
/// narrowed produce the same number.
/// </para>
/// <para>
/// <b>The two feature stages are two figures for that same reason and not for tidiness.</b>
/// <paramref name="FeatureExcluded"/> is a statement about the language - the suite says the
/// construct is a proposal, so no run may score the test - and <paramref name="FeatureFiltered"/>
/// is a statement about this run's interest. Added together, an inclusion set that widened by a
/// hundred and an exclusion that grew by a hundred leave the figure and the selected count both
/// unmoved while a different hundred tests ran, which is exactly the cancellation the paragraph
/// above is written against.
/// </para>
/// </remarks>
internal sealed record SelectionCounts(
    int Candidates,
    int KnownIncorrect,
    int OutOfScope,
    int FeatureExcluded,
    int FeatureFiltered,
    int NegativeWithheld,
    int Unselectable,
    int Selected,
    int Sharded)
{
    /// <summary>Whether the stages account for every candidate.</summary>
    /// <remarks>
    /// The pipeline is a partition, so the excluded counts and the selected count must add back up
    /// to the candidates. A stage that dropped a test without counting it would otherwise be
    /// invisible: the totals would simply be smaller, which is the failure mode this whole section
    /// is written against.
    /// </remarks>
    internal bool Accounts =>
        KnownIncorrect + OutOfScope + FeatureExcluded + FeatureFiltered + NegativeWithheld +
        Unselectable + Selected == Candidates;
}

/// <summary>
/// Content-independent sharding: a test's shard is a hash of its path and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <b>Of the path, so shard membership does not move when the selection changes.</b> Sharding by
/// position in a list - every second test, or a slice of the array - moves every test to a
/// different shard the moment one file is added, which makes a shard's history incomparable with
/// its own past. Roadmap section 14 asks for the stable answer and this is it.
/// </para>
/// <para>
/// FNV-1a over the normalized path's UTF-8 bytes. The algorithm is chosen for being small enough
/// to state in full here and identical on every platform: no framework hash is used, because
/// <see cref="string.GetHashCode()"/> is randomized per process and would put one test in a
/// different shard on every run.
/// </para>
/// </remarks>
internal static class Sharding
{
    /// <summary>The shard index that means "every shard".</summary>
    internal const int AllShards = -1;

    private const uint OffsetBasis = 2166136261;

    private const uint Prime = 16777619;

    /// <summary>Which shard a path belongs to.</summary>
    internal static int ShardFor(string path, int shardCount)
    {
        if (shardCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shardCount), shardCount, "a shard count is at least one");
        }

        var hash = OffsetBasis;

        foreach (var value in Encoding.UTF8.GetBytes(Suite.Normalize(path)))
        {
            hash ^= value;
            hash *= Prime;
        }

        return (int)(hash % (uint)shardCount);
    }

    /// <summary>Whether a shard index is one this count admits.</summary>
    internal static bool Admits(int shardIndex, int shardCount) =>
        shardCount > 0 && (shardIndex == AllShards || (shardIndex >= 0 && shardIndex < shardCount));
}

/// <summary>
/// The selection pipeline, recorded stage by stage.
/// </summary>
/// <remarks>
/// <para>
/// The stages are in the order roadmap section 14 states them: discovery, known-incorrect
/// exclusion, scope filtering, feature-metadata filtering, per-file selectability, and then
/// sharding. The order is load-bearing for the counts rather than for the result - a test excluded
/// by two stages is counted at the first one - so a reader of a report can see which decision
/// removed it.
/// </para>
/// <para>
/// <b>Within the feature stage, the exclusion is asked before the inclusion, and that order is a
/// decision.</b> A run asking for a scope may not thereby score a test about a construct no
/// edition contains: whether the test is answerable is prior to whether this run is interested in
/// it. Put the other way round, <c>--features</c> would become a way to opt back into exactly the
/// tests the suite says nobody should be scoring.
/// </para>
/// </remarks>
internal static class Selection
{
    /// <summary>Runs the pipeline over everything discovery found.</summary>
    internal static (SelectionCounts Counts, IReadOnlyList<ConformanceTest> Tests) Run(
        IReadOnlyList<ConformanceTest> candidates,
        IReadOnlyCollection<string> knownIncorrect,
        IReadOnlyCollection<string> scopePatterns,
        IReadOnlySet<string> excludedFeatures,
        IReadOnlyCollection<string> featurePatterns,
        bool includeNegative,
        int shardIndex,
        int shardCount)
    {
        var knownIncorrectCount = 0;
        var outOfScope = 0;
        var featureExcluded = 0;
        var featureFiltered = 0;
        var negativeWithheld = 0;
        var unselectable = 0;
        var selected = new List<ConformanceTest>();

        foreach (var test in candidates)
        {
            if (knownIncorrect.Contains(test.Path, StringComparer.Ordinal))
            {
                knownIncorrectCount++;
                continue;
            }

            if (!MatchesAny(test.Path, scopePatterns))
            {
                outOfScope++;
                continue;
            }

            if (ExcludedBy(test.Features, excludedFeatures).Length != 0)
            {
                featureExcluded++;
                continue;
            }

            if (featurePatterns.Count != 0 &&
                !test.Features.Any(feature => MatchesAny(feature, featurePatterns)))
            {
                featureFiltered++;
                continue;
            }

            if (test.Expectation.IsNegative && !includeNegative)
            {
                negativeWithheld++;
                continue;
            }

            if (test.Unselectable.Length != 0)
            {
                unselectable++;
                continue;
            }

            selected.Add(test);
        }

        var sharded = shardIndex == Sharding.AllShards
            ? selected
            : selected.Where(test => Sharding.ShardFor(test.Path, shardCount) == shardIndex).ToList();

        return (
            new SelectionCounts(
                candidates.Count,
                knownIncorrectCount,
                outOfScope,
                featureExcluded,
                featureFiltered,
                negativeWithheld,
                unselectable,
                selected.Count,
                sharded.Count),
            sharded);
    }

    /// <summary>
    /// The first feature a test claims that the excluded set names, or empty where it claims none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The empty set here excludes NOTHING, which is the opposite of what the empty set means
    /// one method down, and the two must never share an implementation.</b>
    /// <see cref="MatchesAny"/> reads an empty set as "no filter" because a run given no scope runs
    /// the whole suite; an exclusion read the same way would remove every test, and the run would
    /// report <see cref="ConfigurationFailure.EmptySelection"/> rather than anything about an
    /// engine. Written as its own method, with its own name, so that the asymmetry is a thing a
    /// reader sees rather than a case a reader has to notice.
    /// </para>
    /// <para>
    /// <b>Names are matched whole and never as patterns.</b> These come from the suite's own
    /// feature list rather than from a command line, so there is nothing to glob - and a prefix
    /// match would put <c>class</c> and <c>class-fields-private</c> in one bucket, which is two
    /// different constructs of two different editions.
    /// </para>
    /// </remarks>
    internal static string ExcludedBy(
        IReadOnlyList<string> claimed,
        IReadOnlySet<string> excluded)
    {
        if (excluded.Count == 0)
        {
            return string.Empty;
        }

        foreach (var feature in claimed)
        {
            if (excluded.Contains(feature))
            {
                return feature;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Whether a value matches any of a pattern set, where an empty set matches everything.
    /// </summary>
    /// <remarks>
    /// <b>The empty set means "no filter" and not "nothing".</b> That is the one asymmetry in this
    /// file and it is the conventional one: a run given no scope runs the whole suite. It is
    /// written here once so that no caller has to decide it twice, and the harness's own regression
    /// suite pins it - a filter that emptied a selection instead of leaving it alone would produce
    /// <see cref="ConfigurationFailure.EmptySelection"/>, which is a failure and not a quiet zero.
    /// </remarks>
    internal static bool MatchesAny(string value, IReadOnlyCollection<string> patterns)
    {
        if (patterns.Count == 0)
        {
            return true;
        }

        foreach (var pattern in patterns)
        {
            if (Matches(value, pattern))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>One pattern: an exact name, a prefix ending in <c>*</c>, or a bare prefix path.</summary>
    private static bool Matches(string value, string pattern)
    {
        if (pattern.EndsWith('*'))
        {
            return value.StartsWith(pattern[..^1], StringComparison.Ordinal);
        }

        return string.Equals(value, pattern, StringComparison.Ordinal) ||
            value.StartsWith(pattern + "/", StringComparison.Ordinal);
    }

    /// <summary>
    /// Reads the known-incorrect list: a path and the reason it is excluded, one per line.
    /// </summary>
    /// <remarks>
    /// <b>A reason is required and an entry without one is refused.</b> The list excludes tests
    /// this component believes are wrong, which is the most self-serving claim a conformance run
    /// can make; an entry nobody had to justify is how such a list turns into an allow-list for
    /// whatever fails today.
    /// </remarks>
    internal static IReadOnlyList<string> ReadKnownIncorrect(string path, out IReadOnlyList<string> failures)
    {
        var paths = new List<string>();
        var complaints = new List<string>();

        if (!File.Exists(path))
        {
            failures = complaints;
            return paths;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|', 2);

            if (parts.Length != 2 || parts[1].Trim().Length == 0)
            {
                complaints.Add($"{path}: `{line}` names no reason");
                continue;
            }

            paths.Add(Suite.Normalize(parts[0].Trim()));
        }

        failures = complaints;
        return paths;
    }
}
