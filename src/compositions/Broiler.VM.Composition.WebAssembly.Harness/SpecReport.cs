using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// One shard's report, and the merge that proves a set of them covered the whole selection.
/// </summary>
/// <remarks>
/// <para>
/// <b>A report states the configuration it ran under, and the merge holds every report to the others.</b>
/// The suite revision, the feature manifest, the effective limits, the scope, every count of the
/// selection pipeline and the shard count must be the same in every report; the first field that
/// differs is named as an inconsistent shard configuration, one of the roadmap's named failures. Each
/// shard index from zero up must report exactly once, and every script a report names must be one its
/// shard owns.
/// </para>
/// <para>
/// <b>Removing one shard's report produces incomplete coverage, not a smaller total.</b> A missing index is
/// named, and so is a set of reports whose scripts do not add up to the selected count the pipeline
/// emitted before sharding. A report that stops before its closing line is classified as a shard that
/// did not finish, not read as one that ran fewer scripts: a report that ends early says less than it
/// would have, and summing it would be the smaller total the roadmap forbids.
/// </para>
/// </remarks>
internal static class SpecReport
{
    /// <summary>The fields every report of one run must agree on, in the order the merge compares them.</summary>
    internal static readonly string[] Configuration =
        ["report", "revision", "manifest", "limits", "scope", "candidates", "known-incorrect", "out-of-scope", "unselectable", "selected", "shards"];

    /// <summary>A report as read: its configuration, its shard, its scripts, its families and its failing scripts.</summary>
    internal sealed record Report(
        string Source,
        IReadOnlyDictionary<string, string> Fields,
        int Shard,
        IReadOnlyList<string> Scripts,
        IReadOnlyDictionary<string, SpecSuite.FamilyTotals> Families,
        IReadOnlyDictionary<string, int> Failing);

    /// <summary>The report of one run's shard, as text.</summary>
    internal static string Write(
        IReadOnlyDictionary<string, string> configuration,
        int shard,
        IEnumerable<string> scripts,
        IReadOnlyDictionary<string, SpecSuite.FamilyTotals> families,
        IReadOnlyDictionary<string, int> failing)
    {
        var text = new StringBuilder("# broiler-wasm-harness spec shard report: see SpecReport.cs\n");

        foreach (var field in Configuration)
        {
            text.Append(field).Append(' ').Append(configuration[field]).Append('\n');
        }

        text.Append("shard ").Append(Text(shard)).Append('\n');

        foreach (var script in scripts)
        {
            text.Append("script ").Append(script).Append('\n');
        }

        foreach (var (family, totals) in families)
        {
            text.Append("family ").Append(family)
                .Append(" selected ").Append(Text(totals.Selected))
                .Append(" executed ").Append(Text(totals.Executed))
                .Append(" passed ").Append(Text(totals.Passed))
                .Append(" failed ").Append(Text(totals.Failed))
                .Append(" skipped ").Append(Text(totals.Skipped))
                .Append(" timed-out 0\n");
        }

        foreach (var (script, count) in failing.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            text.Append("failing ").Append(script).Append(' ').Append(Text(count)).Append('\n');
        }

        return text.Append("end\n").ToString();
    }

    /// <summary>
    /// Reads a report, classifying one that stopped before its closing line as a shard that did not
    /// finish and one it cannot read as malformed.
    /// </summary>
    internal static bool TryRead(string source, string text, out Report report, out string failure)
    {
        report = null!;
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')
            .Where(static line => line.Length > 0 && line[0] != '#').ToList();

        if (lines.Count == 0 || lines[^1] != "end")
        {
            failure = $"shard report {source} ends before its closing line: the shard did not finish, and what it reported is not a total";
            return false;
        }

        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        var scripts = new List<string>();
        var families = new Dictionary<string, SpecSuite.FamilyTotals>(StringComparer.Ordinal);
        var failing = new Dictionary<string, int>(StringComparer.Ordinal);
        int? shard = null;

        foreach (var line in lines.Take(lines.Count - 1))
        {
            var space = line.IndexOf(' ', StringComparison.Ordinal);
            var (key, value) = space < 0 ? (line, string.Empty) : (line[..space], line[(space + 1)..]);

            switch (key)
            {
                case "shard" when int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var index):
                    shard = index;
                    break;
                case "script":
                    scripts.Add(value);
                    break;
                case "family" when TryFamily(value, out var name, out var totals):
                    families[name] = totals;
                    break;
                case "failing" when value.Split(' ') is [var script, var counted] &&
                                    int.TryParse(counted, NumberStyles.None, CultureInfo.InvariantCulture, out var count):
                    failing[script] = count;
                    break;
                default:
                    if (Configuration.Contains(key, StringComparer.Ordinal))
                    {
                        fields[key] = value;
                        break;
                    }

                    failure = $"shard report {source} holds the line \"{line}\", which no report holds";
                    return false;
            }
        }

        var missing = Configuration.FirstOrDefault(field => !fields.ContainsKey(field));

        if (missing is not null || shard is null)
        {
            failure = $"shard report {source} does not state its {missing ?? "shard"}";
            return false;
        }

        report = new Report(source, fields, shard.Value, scripts, families, failing);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Merges <paramref name="reports"/>: answers the merged totals and failing scripts, and adds to
    /// <paramref name="failures"/> every configuration failure the set shows.
    /// </summary>
    internal static (Dictionary<string, SpecSuite.FamilyTotals> Families, Dictionary<string, int> Failing, HashSet<string> Scripts, IReadOnlyDictionary<string, string> Configuration)
        Merge(IReadOnlyList<(string Source, string Text)> reports, List<string> failures)
    {
        var families = new Dictionary<string, SpecSuite.FamilyTotals>(StringComparer.Ordinal);
        var failing = new Dictionary<string, int>(StringComparer.Ordinal);
        var scripts = new HashSet<string>(StringComparer.Ordinal);
        var read = new List<Report>();

        foreach (var (source, text) in reports)
        {
            if (TryRead(source, text, out var report, out var failure))
            {
                read.Add(report);
            }
            else
            {
                failures.Add(failure);
            }
        }

        if (read.Count == 0)
        {
            failures.Add("no shard report was read, so nothing was merged");
            return (families, failing, scripts, new Dictionary<string, string>());
        }

        var first = read[0];

        // Every report states the configuration of one run, or the set is not one run.
        foreach (var report in read.Skip(1))
        {
            var differing = Configuration.FirstOrDefault(field => !string.Equals(report.Fields[field], first.Fields[field], StringComparison.Ordinal));

            if (differing is not null)
            {
                failures.Add(
                    $"inconsistent shard configuration: {differing} is \"{first.Fields[differing]}\" in {first.Source} and \"{report.Fields[differing]}\" in {report.Source}");
            }
        }

        if (!int.TryParse(first.Fields["shards"], NumberStyles.None, CultureInfo.InvariantCulture, out var shards) || shards < 1 ||
            !int.TryParse(first.Fields["selected"], NumberStyles.None, CultureInfo.InvariantCulture, out var selected))
        {
            failures.Add($"shard report {first.Source} states a shard count or a selected count that is not a number");
            return (families, failing, scripts, first.Fields);
        }

        // Each index once; a missing one is incomplete coverage, never a smaller total.
        foreach (var group in read.GroupBy(static report => report.Shard).Where(static group => group.Count() > 1))
        {
            failures.Add($"inconsistent shard configuration: shard {Text(group.Key)} is reported by {string.Join(" and ", group.Select(static report => report.Source))}");
        }

        foreach (var index in Enumerable.Range(0, shards).Where(index => read.All(report => report.Shard != index)))
        {
            failures.Add($"incomplete coverage: shard {Text(index)} of {Text(shards)} has no report");
        }

        foreach (var report in read)
        {
            foreach (var script in report.Scripts)
            {
                if (SpecSelection.Bucket(script, shards) != report.Shard)
                {
                    failures.Add($"inconsistent shard configuration: {report.Source} ran {script}, which shard {Text(SpecSelection.Bucket(script, shards))} owns and not shard {Text(report.Shard)}");
                }

                if (!scripts.Add(script))
                {
                    failures.Add($"inconsistent shard configuration: {script} is reported by more than one shard");
                }
            }

            foreach (var (family, totals) in report.Families)
            {
                var sum = families.GetValueOrDefault(family);
                families[family] = new SpecSuite.FamilyTotals(
                    sum.Selected + totals.Selected, sum.Executed + totals.Executed, sum.Passed + totals.Passed,
                    sum.Failed + totals.Failed, sum.Skipped + totals.Skipped);
            }

            foreach (var (script, count) in report.Failing)
            {
                failing[script] = failing.GetValueOrDefault(script) + count;
            }
        }

        if (scripts.Count != selected)
        {
            failures.Add(
                $"incomplete coverage: the shards ran {Text(scripts.Count)} scripts of the {Text(selected)} the pipeline selected before sharding");
        }

        return (families, failing, scripts, first.Fields);
    }

    private static bool TryFamily(string value, out string name, out SpecSuite.FamilyTotals totals)
    {
        name = string.Empty;
        totals = default;
        var parts = value.Split(' ');

        if (parts.Length != 13 || parts[1] != "selected" || parts[3] != "executed" || parts[5] != "passed" ||
            parts[7] != "failed" || parts[9] != "skipped" || parts[11] != "timed-out")
        {
            return false;
        }

        var numbers = new int[5];

        for (var index = 0; index < 5; index++)
        {
            if (!int.TryParse(parts[2 + (index * 2)], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[index]))
            {
                return false;
            }
        }

        name = parts[0];
        totals = new SpecSuite.FamilyTotals(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4]);
        return true;
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
