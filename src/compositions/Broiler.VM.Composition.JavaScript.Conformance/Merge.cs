namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// Combining a run's shard reports into one, and refusing to combine reports of two runs.
/// </summary>
/// <remarks>
/// <para>
/// <b>The merge's job is not addition.</b> Adding six numbers is the easy half and it is the half
/// that hides everything: a run missing a shard adds to a smaller total that looks like a smaller
/// suite, and two shards configured differently add to a total that describes neither. What this
/// does first is prove that the reports it was handed are the shards of one run and that they
/// cover the whole selection; the addition happens afterwards, and only then.
/// </para>
/// <para>
/// <b>The pre-sharding selected count is what makes coverage decidable.</b> Every shard records
/// how many tests the pipeline selected before sharding, which is a figure no shard can influence.
/// If the merged case count is not that figure, tests went missing between the selection and the
/// report, whatever the per-shard numbers add up to.
/// </para>
/// </remarks>
internal static class Merge
{
    /// <summary>Merges shard reports, or names why they are not one run's.</summary>
    internal static Report Combine(IReadOnlyList<Report> shards)
    {
        var findings = new List<ConfigurationFinding>();

        if (shards.Count == 0)
        {
            return new Report(
                new SuiteRevision("unnamed", string.Empty),
                Sharding.AllShards,
                0,
                IncludeNegative: false,
                new SelectionCounts(0, 0, 0, 0, 0, 0, 0, 0, 0),
                [],
                [
                    new ConfigurationFinding(
                        ConfigurationFailure.IncompleteShardCoverage,
                        "no shard report was given to merge"),
                ]);
        }

        var first = shards[0];

        foreach (var (field, values) in Fields(shards))
        {
            if (values.Count > 1)
            {
                findings.Add(new ConfigurationFinding(
                    ConfigurationFailure.InconsistentShardConfiguration,
                    $"shard reports disagree about {field}: {string.Join(", ", values)}"));
            }
        }

        // A MERGED REPORT IS NOT A SHARD, and saying so is cheaper than the alternative. A merge
        // reads every report it is handed, so a merged one among them would be counted as a shard
        // whose case list duplicates every other's - reported as a duplicate-scoring failure that
        // blames the tests rather than the arrangement. The harness also refuses to WRITE a merged
        // report into the directory it merged, which is the same defence one step earlier.
        foreach (var whole in shards.Where(static shard => shard.ShardIndex == Sharding.AllShards))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.InconsistentShardConfiguration,
                $"a report covering all {whole.ShardCount} shards was handed to a merge; a merged " +
                "report is not a shard"));
        }

        var expected = Enumerable.Range(0, first.ShardCount).ToArray();
        var present = shards
            .Select(static shard => shard.ShardIndex)
            .OrderBy(static index => index)
            .ToArray();

        foreach (var missing in expected.Except(present))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.IncompleteShardCoverage,
                $"shard {missing} of {first.ShardCount} reported nothing"));
        }

        foreach (var duplicate in present
                     .GroupBy(static index => index)
                     .Where(static group => group.Count() > 1))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.InconsistentShardConfiguration,
                $"shard {duplicate.Key} reported {duplicate.Count()} times"));
        }

        var results = shards.SelectMany(static shard => shard.Results).ToArray();

        foreach (var duplicate in results
                     .GroupBy(static result => result.Path, StringComparer.Ordinal)
                     .Where(static group => group.Count() > 1))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.InconsistentShardConfiguration,
                $"`{duplicate.Key}` was scored by {duplicate.Count()} shards"));
        }

        // The coverage question, and the reason every shard records a figure it cannot influence.
        if (results.Length != first.Selection.Selected)
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.IncompleteShardCoverage,
                $"the pipeline selected {first.Selection.Selected} tests and the shards between " +
                $"them scored {results.Length}"));
        }

        var merged = first.Selection with { Sharded = results.Length };

        findings.AddRange(Report.Validate(first.Suite, merged, results));

        return new Report(
            first.Suite,
            Sharding.AllShards,
            first.ShardCount,
            first.IncludeNegative,
            merged,
            results,
            Distinct(findings));
    }

    /// <summary>
    /// The fields every shard of one run must state identically, with what they actually stated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The selection figures are in here and that is the point of the list. Two shards that
    /// discovered different numbers of candidates are two runs, however similar their totals look,
    /// and a merge that added them would publish a figure describing a suite nobody ran.
    /// </para>
    /// <para>
    /// <b>Every PRE-SHARDING figure, and only those.</b> All eight are decided before sharding, so
    /// no shard can influence any of them and two shards of one run state them identically;
    /// <c>sharded</c> is the ninth and is deliberately absent, because differing is what it is for.
    /// The list held four of the eight, and the four it omitted - the scope, both feature stages
    /// and the negatives - are exactly the ones a differently configured shard moves. Two shards
    /// whose filters removed the same NUMBER of tests would agree on <c>selected</c> while having
    /// scored different tests, and nothing here would have said so.
    /// </para>
    /// </remarks>
    private static IEnumerable<(string Field, IReadOnlyCollection<string> Values)> Fields(
        IReadOnlyList<Report> shards)
    {
        yield return ("suite", Distinct(shards, static shard => shard.Suite.Name));
        yield return ("suiteRevision", Distinct(shards, static shard => shard.Suite.ToString()));
        yield return ("shardCount", Distinct(shards, static shard => shard.ShardCount.ToString()));
        yield return ("includeNegative", Distinct(shards, static shard => shard.IncludeNegative.ToString()));
        yield return ("candidates", Distinct(shards, static shard => shard.Selection.Candidates.ToString()));
        yield return ("selected", Distinct(shards, static shard => shard.Selection.Selected.ToString()));
        yield return ("knownIncorrect", Distinct(shards, static shard => shard.Selection.KnownIncorrect.ToString()));
        yield return ("outOfScope", Distinct(shards, static shard => shard.Selection.OutOfScope.ToString()));
        yield return ("featureExcluded", Distinct(shards, static shard => shard.Selection.FeatureExcluded.ToString()));
        yield return ("featureFiltered", Distinct(shards, static shard => shard.Selection.FeatureFiltered.ToString()));
        yield return ("negativeWithheld", Distinct(shards, static shard => shard.Selection.NegativeWithheld.ToString()));
        yield return ("unselectable", Distinct(shards, static shard => shard.Selection.Unselectable.ToString()));
    }

    private static IReadOnlyCollection<string> Distinct(
        IReadOnlyList<Report> shards,
        Func<Report, string> field) =>
        shards.Select(field).Distinct(StringComparer.Ordinal).OrderBy(static value => value, StringComparer.Ordinal).ToArray();

    /// <summary>One finding per failure and detail, so a repeated cause is reported once.</summary>
    private static IReadOnlyList<ConfigurationFinding> Distinct(
        IEnumerable<ConfigurationFinding> findings) =>
        findings
            .GroupBy(static finding => finding.Failure.ToString() + '|' + finding.Detail, StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToArray();
}
