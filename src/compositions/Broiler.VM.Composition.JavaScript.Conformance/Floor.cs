using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>One host mode's floor: the numbers a later run of the same suite may not fall below.</summary>
internal sealed record FloorRow(HostMode Mode, int Executed, int Passed);

/// <summary>
/// The ratchet: the first per-host-mode totals admitted for a suite revision, and what they forbid.
/// </summary>
/// <remarks>
/// <para>
/// <b>Admitted is not the ledger's Accepted, and the difference is deliberate.</b> Accepting a
/// milestone needs a reviewer decision nothing in this component has, so a floor that could only be
/// set by an accepted milestone would be a floor nothing could ever set. A floor is a measurement
/// discipline; a status is a claim about review.
/// </para>
/// <para>
/// <b>A floor records the revision it was set under, and is never compared across revisions.</b> A
/// suite that added tests would otherwise read as a regression and a suite that removed them would
/// silently lower the bar - so a revision change re-bases the floor from the first run admitted on
/// the new revision, and the old floor and the reason stay in the file rather than being
/// overwritten. This is the discipline the diagnostic registry and the retained corpus already
/// apply to their own pinned revisions.
/// </para>
/// <para>
/// <b>A run carrying any configuration failure may not set a floor.</b> The whole value of a floor
/// is that it was measured over a run that covered what it claimed to cover, and a run missing a
/// shard covered less than it claimed.
/// </para>
/// </remarks>
internal sealed record Floor(SuiteRevision Suite, IReadOnlyList<FloorRow> Rows, IReadOnlyList<string> Retired)
{
    /// <summary>The header a floor file carries.</summary>
    internal const string Header = "# broiler-js-conformance floor 1";

    /// <summary>Whether this run may be admitted as a floor at all.</summary>
    internal static bool Admissible(Report report, out string why)
    {
        if (report.Findings.Count != 0)
        {
            why = "the run carries " + report.Findings.Count + " configuration failure(s)";
            return false;
        }

        if (report.ShardIndex != Sharding.AllShards)
        {
            why = "the run is one shard rather than a merged run";
            return false;
        }

        if (report.Executed == 0)
        {
            why = "the run executed nothing";
            return false;
        }

        why = string.Empty;
        return true;
    }

    /// <summary>The floor a run amounts to.</summary>
    internal static Floor From(Report report) => new(
        report.Suite,
        report.Modes
            .Where(static totals => totals.Selected != 0)
            .Select(static totals => new FloorRow(totals.Mode, totals.Executed, totals.Passed))
            .ToArray(),
        []);

    /// <summary>
    /// What a run does to a floor: hold it, regress against it, or re-base it onto a new revision.
    /// </summary>
    internal enum Verdict
    {
        /// <summary>The run meets or exceeds every row. Nothing is written.</summary>
        Held,

        /// <summary>The run falls below a row. A failure of the run.</summary>
        Regressed,

        /// <summary>The suite revision changed, so the floor is re-based rather than compared.</summary>
        Rebased,

        /// <summary>There was no floor. This run becomes one.</summary>
        Set,
    }

    /// <summary>Compares a run against this floor.</summary>
    internal Verdict Compare(Report report, out IReadOnlyList<string> complaints)
    {
        var found = new List<string>();

        if (!string.Equals(Suite.Name, report.Suite.Name, StringComparison.Ordinal) ||
            !string.Equals(Suite.Revision, report.Suite.Revision, StringComparison.Ordinal))
        {
            complaints =
            [
                $"the floor was set under {Suite} and this run read {report.Suite}: re-basing, " +
                "because a floor compared across revisions reads an added test as a regression",
            ];

            return Verdict.Rebased;
        }

        foreach (var row in Rows)
        {
            var totals = report.Modes.FirstOrDefault(candidate => candidate.Mode == row.Mode);

            if (totals is null || totals.Selected == 0)
            {
                found.Add($"host mode `{row.Mode}` had a floor of {row.Passed} passed and did not run");
                continue;
            }

            if (totals.Executed < row.Executed)
            {
                found.Add(
                    $"host mode `{row.Mode}` executed {totals.Executed} and the floor is {row.Executed}");
            }

            if (totals.Passed < row.Passed)
            {
                found.Add(
                    $"host mode `{row.Mode}` passed {totals.Passed} and the floor is {row.Passed}");
            }
        }

        complaints = found;
        return found.Count == 0 ? Verdict.Held : Verdict.Regressed;
    }

    /// <summary>Renders a floor.</summary>
    internal string Render()
    {
        var text = new StringBuilder();
        text.Append(Header).Append('\n');
        text.Append("# suite <name>; revision <revision>; mode <name> <executed> <passed>\n");
        text.Append("# a retired line is a floor a revision change re-based, kept with its reason\n");
        text.Append("suite ").Append(Suite.Name).Append('\n');
        text.Append("revision ").Append(Suite.IsPinned ? Suite.Revision : "unpinned").Append('\n');

        foreach (var row in Rows.OrderBy(static row => row.Mode))
        {
            text.Append("mode ")
                .Append(row.Mode)
                .Append(' ')
                .Append(row.Executed.ToString(CultureInfo.InvariantCulture))
                .Append(' ')
                .Append(row.Passed.ToString(CultureInfo.InvariantCulture))
                .Append('\n');
        }

        foreach (var retired in Retired)
        {
            text.Append("retired ").Append(retired.Replace('\n', ' ')).Append('\n');
        }

        return text.ToString();
    }

    /// <summary>Reads a floor back, or says the file is not one.</summary>
    internal static Floor Read(string path)
    {
        var name = "unnamed";
        var revision = string.Empty;
        var rows = new List<FloorRow>();
        var retired = new List<string>();
        var seenHeader = false;

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            switch (parts[0])
            {
                case "suite" when parts.Length == 2:
                    name = parts[1];
                    break;
                case "revision" when parts.Length == 2:
                    revision = string.Equals(parts[1], "unpinned", StringComparison.Ordinal)
                        ? string.Empty
                        : parts[1];
                    break;
                case "mode" when parts.Length == 4:
                    rows.Add(new FloorRow(
                        Enum.Parse<HostMode>(parts[1]),
                        int.Parse(parts[2], CultureInfo.InvariantCulture),
                        int.Parse(parts[3], CultureInfo.InvariantCulture)));
                    break;
                case "retired":
                    retired.Add(line["retired ".Length..]);
                    break;
                default:
                    throw new InvalidOperationException($"{path}: `{line}` is not a floor line");
            }
        }

        if (!seenHeader)
        {
            throw new InvalidOperationException($"{path} does not open with `{Header}`");
        }

        return new Floor(new SuiteRevision(name, revision), rows, retired);
    }

    /// <summary>The floor a re-base produces: the new run's rows, with the old floor retained.</summary>
    internal Floor Rebase(Report report, string reason) => From(report) with
    {
        Retired = [.. Retired, $"{Suite} {string.Join("; ", Rows.Select(static row => $"{row.Mode} {row.Executed} {row.Passed}"))} :: {reason}"],
    };
}
