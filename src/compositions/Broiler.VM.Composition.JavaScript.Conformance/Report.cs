using Broiler.VM.Profile.JavaScript;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>One scored case.</summary>
/// <param name="Path">The test's identity: its normalized suite-relative path.</param>
/// <param name="Mode">The host mode it was presented through.</param>
/// <param name="Status">What it is counted as.</param>
/// <param name="Completion">How the program settled, on every result and not only an async one.</param>
/// <param name="Answer">What actually happened, in the expectation vocabulary.</param>
/// <param name="Detail">Why, where a reader needs it.</param>
internal sealed record CaseResult(
    string Path,
    HostMode Mode,
    ConformanceStatus Status,
    CompletionKind Completion,
    string Answer,
    string Detail);

/// <summary>One host mode's own totals.</summary>
/// <remarks>
/// <b>Selected and executed are separate numbers.</b> A mode that selected forty files and ran
/// none of them is a coverage gap, and a combined total would show it as forty passes elsewhere
/// plus a smaller number. Roadmap section 14 makes that a named configuration failure, which needs
/// both figures to be visible before it can be one.
/// </remarks>
internal sealed record ModeTotals(
    HostMode Mode,
    int Selected,
    int Executed,
    int Passed,
    int Failed,
    int Skipped,
    int TimedOut)
{
    /// <summary>Whether the four verdicts account for everything selected.</summary>
    internal bool Accounts =>
        Passed + Failed + Skipped + TimedOut == Selected && Executed == Selected - Skipped;

    /// <summary>Totals for one mode's results, derived rather than counted twice.</summary>
    internal static ModeTotals From(HostMode mode, IReadOnlyList<CaseResult> results)
    {
        var mine = results.Where(result => result.Mode == mode).ToArray();

        return new ModeTotals(
            mode,
            mine.Length,
            mine.Count(static result => result.Status != ConformanceStatus.Skipped),
            mine.Count(static result => result.Status == ConformanceStatus.Passed),
            mine.Count(static result => result.Status == ConformanceStatus.Failed),
            mine.Count(static result => result.Status == ConformanceStatus.Skipped),
            mine.Count(static result => result.Status == ConformanceStatus.TimedOut));
    }
}

/// <summary>
/// One run's conclusive report: what was configured, what was selected, what each mode did, and
/// every case.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a line-oriented text document rather than a serialized object graph</b>, in the same
/// idiom as the retained corpus manifest and the published diagnostic registry. Two reasons, and
/// the second is the load-bearing one: this root is AOT-compatible and a reflection-based
/// serializer is not, and a format a reader can diff by eye is a format a reviewer can check
/// without running anything.
/// </para>
/// <para>
/// <b>The writer and the reader are deliberately separate code.</b> A serializer that round-trips
/// itself agrees with itself whatever either half means, which is the same argument the corpus
/// manifest's hand-written parser is written under.
/// </para>
/// </remarks>
internal sealed record Report(
    SuiteRevision Suite,
    int ShardIndex,
    int ShardCount,
    bool IncludeNegative,
    SelectionCounts Selection,
    IReadOnlyList<CaseResult> Results,
    IReadOnlyList<ConfigurationFinding> Findings)
{
    /// <summary>The header every report carries, naming the format's version.</summary>
    /// <remarks>
    /// <b>Version 2 added a column to the selection line; version 3 adds the `edition` line.</b> A
    /// reader that met an older report would otherwise refuse it on a line it could not parse and
    /// blame the line rather than the format. Nothing in this repository holds a report - they are
    /// produced and merged inside one run - so a bump costs a message rather than a migration.
    /// </remarks>
    internal const string Header = "# broiler-js-conformance report 3";

    /// <summary>Per-mode totals, in the enumeration's order so two reports line up.</summary>
    internal IReadOnlyList<ModeTotals> Modes =>
        Enum.GetValues<HostMode>().Select(mode => ModeTotals.From(mode, Results)).ToArray();

    /// <summary>How many cases actually ran.</summary>
    internal int Executed => Results.Count(static result => result.Status != ConformanceStatus.Skipped);

    /// <summary>Whether anything at all failed: a case, or the configuration.</summary>
    internal bool Failed =>
        Findings.Count != 0 ||
        Results.Any(static result => result.Status is ConformanceStatus.Failed or ConformanceStatus.TimedOut);

    /// <summary>
    /// The configuration failures a single shard's report can carry on its own.
    /// </summary>
    /// <remarks>
    /// Three of the six are decidable from one report; the other three need the whole set and are
    /// the merge's. Which is which is stated here rather than left to whoever calls this, because a
    /// check that ran in only one of the two places is a check a run can be configured around.
    /// </remarks>
    internal static IReadOnlyList<ConfigurationFinding> Validate(
        SuiteRevision suite,
        SelectionCounts selection,
        IReadOnlyList<CaseResult> results)
    {
        var findings = new List<ConfigurationFinding>();

        if (!suite.IsPinned)
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.MissingSuiteRevision,
                $"suite `{suite.Name}` was read at no pinned revision; a branch name is not a pin " +
                "and neither is nothing"));
        }

        if (selection.Selected == 0)
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.EmptySelection,
                $"the pipeline admitted none of {selection.Candidates} candidates"));
        }

        if (selection.Sharded != 0 &&
            results.All(static result => result.Status == ConformanceStatus.Skipped))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.NoExecutedTests,
                $"{selection.Sharded} tests were selected for this shard and none of them ran"));
        }

        foreach (var totals in Enum.GetValues<HostMode>()
                     .Select(mode => ModeTotals.From(mode, results))
                     .Where(static totals => totals.Selected != 0 && totals.Executed == 0))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.IncompleteHostModeCoverage,
                $"host mode `{totals.Mode}` selected {totals.Selected} tests and executed none"));
        }

        return findings;
    }

    /// <summary>Renders the report.</summary>
    internal string Render()
    {
        var text = new StringBuilder();
        text.Append(Header).Append('\n');
        text.Append("# run|suite|revision|shardIndex|shardCount|includeNegative\n");
        text.Append("# edition|standard|year|source|revision|document|digest|archived\n");
        text.Append("# selection|candidates|knownIncorrect|outOfScope|featureExcluded|featureFiltered|negativeWithheld|unselectable|selected|sharded\n");
        text.Append("# mode|name|selected|executed|passed|failed|skipped|timedOut\n");
        text.Append("# result|path|mode|status|completion|answer|detail\n");
        text.Append("# config|failure|detail\n");

        // THE OTHER PINNED INPUT, WRITTEN BESIDE THE SUITE'S. A report already names the suite
        // revision it scored; the edition its manifests are defined against is the second half of
        // the same question, and roadmap section 14's delivery map lists "a manifest scored against
        // an unpinned edition" among the failures this milestone must not produce. Written from the
        // profile's own declaration rather than restated here, so the two cannot disagree, and
        // carrying `archived` because a pin that is provisional has to say so where the numbers
        // are rather than only in a ledger a reader might not open.
        text.Append(string.Join(
                '|',
                "edition",
                JavaScriptLanguageEdition.Standard,
                JavaScriptLanguageEdition.Year,
                JavaScriptLanguageEdition.Source,
                JavaScriptLanguageEdition.Revision,
                JavaScriptLanguageEdition.Document,
                JavaScriptLanguageEdition.DocumentDigest,
                JavaScriptLanguageEdition.Archived ? "archived" : "not-archived"))
            .Append('\n');

        text.Append(string.Join(
                '|',
                "run",
                Suite.Name,
                Suite.IsPinned ? Suite.Revision : "unpinned",
                Number(ShardIndex),
                Number(ShardCount),
                IncludeNegative ? "yes" : "no"))
            .Append('\n');

        text.Append(string.Join(
                '|',
                "selection",
                Number(Selection.Candidates),
                Number(Selection.KnownIncorrect),
                Number(Selection.OutOfScope),
                Number(Selection.FeatureExcluded),
                Number(Selection.FeatureFiltered),
                Number(Selection.NegativeWithheld),
                Number(Selection.Unselectable),
                Number(Selection.Selected),
                Number(Selection.Sharded)))
            .Append('\n');

        foreach (var totals in Modes)
        {
            text.Append(string.Join(
                    '|',
                    "mode",
                    totals.Mode.ToString(),
                    Number(totals.Selected),
                    Number(totals.Executed),
                    Number(totals.Passed),
                    Number(totals.Failed),
                    Number(totals.Skipped),
                    Number(totals.TimedOut)))
                .Append('\n');
        }

        foreach (var result in Results.OrderBy(static result => result.Path, StringComparer.Ordinal))
        {
            text.Append(string.Join(
                    '|',
                    "result",
                    result.Path,
                    result.Mode.ToString(),
                    result.Status.ToString(),
                    result.Completion.ToString(),
                    Cell(result.Answer),
                    Cell(result.Detail)))
                .Append('\n');
        }

        foreach (var finding in Findings)
        {
            text.Append(string.Join('|', "config", finding.Failure.ToString(), Cell(finding.Detail)))
                .Append('\n');
        }

        return text.ToString();
    }

    /// <summary>Reads a report back, hand-parsed and refusing anything it does not recognise.</summary>
    internal static Report Read(string path)
    {
        var suite = new SuiteRevision("unnamed", string.Empty);
        var shardIndex = Sharding.AllShards;
        var shardCount = 1;
        var includeNegative = false;
        var selection = new SelectionCounts(0, 0, 0, 0, 0, 0, 0, 0, 0);
        var results = new List<CaseResult>();
        var findings = new List<ConfigurationFinding>();
        var seenHeader = false;

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var parts = line.Split('|');

            switch (parts[0])
            {
                case "run" when parts.Length == 6:
                    suite = new SuiteRevision(
                        parts[1],
                        string.Equals(parts[2], "unpinned", StringComparison.Ordinal) ? string.Empty : parts[2]);
                    shardIndex = Value(parts[3]);
                    shardCount = Value(parts[4]);
                    includeNegative = string.Equals(parts[5], "yes", StringComparison.Ordinal);
                    break;

                case "selection" when parts.Length == 10:
                    selection = new SelectionCounts(
                        Value(parts[1]), Value(parts[2]), Value(parts[3]), Value(parts[4]),
                        Value(parts[5]), Value(parts[6]), Value(parts[7]), Value(parts[8]),
                        Value(parts[9]));
                    break;

                // The edition line is not read back into the report either - it is a property of
                // the build rather than of the run - but it IS checked, and a disagreement is
                // refused rather than averaged. Two shards built against two editions are two runs
                // whatever their totals look like, and the merge's whole job is to refuse that
                // shape before it adds anything.
                case "edition" when parts.Length == 8:
                    if (!string.Equals(parts[2], JavaScriptLanguageEdition.Year, StringComparison.Ordinal) ||
                        !string.Equals(parts[4], JavaScriptLanguageEdition.Revision, StringComparison.Ordinal) ||
                        !string.Equals(parts[6], JavaScriptLanguageEdition.DocumentDigest, StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            $"{path} was scored against {parts[1]} {parts[2]} at {parts[4]} and this " +
                            $"build is pinned to {JavaScriptLanguageEdition.Standard} " +
                            $"{JavaScriptLanguageEdition.Year} at {JavaScriptLanguageEdition.Revision}");
                    }

                    break;

                // The mode lines are derived from the results and are written for a reader, so they
                // are not read back: parsing them would give the merge a second, independent set of
                // totals that could disagree with the cases they are totals OF.
                case "mode":
                    break;

                case "result" when parts.Length == 7:
                    results.Add(new CaseResult(
                        parts[1],
                        Enum.Parse<HostMode>(parts[2]),
                        Enum.Parse<ConformanceStatus>(parts[3]),
                        Enum.Parse<CompletionKind>(parts[4]),
                        parts[5],
                        parts[6]));
                    break;

                case "config" when parts.Length == 3:
                    findings.Add(new ConfigurationFinding(
                        Enum.Parse<ConfigurationFailure>(parts[1]), parts[2]));
                    break;

                default:
                    throw new InvalidOperationException($"{path}: `{line}` is not a report line");
            }
        }

        if (!seenHeader)
        {
            throw new InvalidOperationException($"{path} does not open with `{Header}`");
        }

        return new Report(suite, shardIndex, shardCount, includeNegative, selection, results, findings);
    }

    /// <summary>
    /// A cell: one line, with the separator removed rather than escaped.
    /// </summary>
    /// <remarks>
    /// A detail carrying a newline or a pipe would silently produce a row nobody wrote, which is
    /// the defect the composition register's own reader was found to have. Removing is chosen over
    /// escaping because nothing reads a detail back into a decision - it is prose for a human - and
    /// an escape nobody unescapes is worse than a character nobody wrote.
    /// </remarks>
    private static string Cell(string value) => value
        .Replace('\r', ' ')
        .Replace('\n', ' ')
        .Replace('|', '/')
        .Trim();

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static int Value(string text) => int.Parse(text, CultureInfo.InvariantCulture);
}

/// <summary>One named configuration failure, with what a reader has to know to fix it.</summary>
internal sealed record ConfigurationFinding(ConfigurationFailure Failure, string Detail);
