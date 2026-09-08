// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>What one <c>--test262</c> run counted, in the five words a total is built from.</summary>
/// <param name="Files">How many suite files this report covers.</param>
/// <param name="Variants">How many variants those files were run in.</param>
/// <param name="Passed">Variants that did what the file declared.</param>
/// <param name="Failed">Variants that did not.</param>
/// <param name="Unsupported">Variants naming a construct the manifest does not admit.</param>
/// <param name="Exhausted">Variants that spent an allowance without answering.</param>
/// <param name="Skipped">Variants the runner declined by name.</param>
/// <remarks>
/// <b>Five verdicts and not four, because "we did not wait long enough" is not a failure.</b> An
/// exhaustion is the harness's allowance running out, which is a statement about the ceiling this
/// run chose rather than about the engine; folding it into <paramref name="Failed"/> would put a
/// number nobody can act on into the one column a repair queue is read from. It is not a pass
/// either, and it is not <paramref name="Skipped"/>: a skipped variant was never started.
/// </remarks>
internal sealed record Test262Totals(
    int Files,
    int Variants,
    int Passed,
    int Failed,
    int Unsupported,
    int Exhausted,
    int Skipped)
{
    /// <summary>Whether the five verdicts account for every variant.</summary>
    internal bool Accounts => Passed + Failed + Unsupported + Exhausted + Skipped == Variants;

    /// <summary>The one line a transcript ends with.</summary>
    internal string Describe() =>
        "# test262 " + Number(Files) + " files, " + Number(Variants) + " variants: pass " +
        Number(Passed) + ", fail " + Number(Failed) + ", unsupported " + Number(Unsupported) +
        ", exhausted " + Number(Exhausted) + ", skipped " + Number(Skipped);

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// One <c>--test262</c> run's conclusive report: what it was taken under, how much of the suite it
/// covers, what each variant did, and which families and dimensions it met.
/// </summary>
/// <remarks>
/// <para>
/// <b>The same idiom as the <c>--run</c> mode's report and deliberately not the same document.</b>
/// It is line-oriented text in the shape <see cref="Report"/> already uses - a versioned header, one
/// row per fact, a hand-written reader that refuses a line it does not recognise, and derived rows
/// written for a reader rather than read back - because that shape is AOT-compatible, diffable by
/// eye, and already the idiom of the retained corpus manifest and the diagnostic registry. What it
/// is not is the same RECORD: the two modes count different things. <see cref="Report"/> counts
/// four statuses per host mode over this component's own fixtures; this one counts five verdicts per
/// variant over a third-party checkout, under a named manifest, with the families a manifest
/// declines named. Bending one record over both would have produced a document whose columns mean
/// different things depending on which mode wrote it.
/// </para>
/// <para>
/// <b>The coverage field is the guard against a partial run reading as a whole one.</b> A transcript
/// of half the suite that looks like a whole-suite run is exactly the failure this repository's
/// records exist to prevent, so a run says in one field - which a reader can see and a rule can
/// grep - whether it covered the whole selection, and names every reason it did not: it was one
/// shard, a limit dropped files, or a subtree was named rather than the suite root. The field is
/// DERIVED from the facts that make it true and checked against them on read, so a hand-edited
/// report that claims to be whole is refused rather than believed.
/// </para>
/// </remarks>
internal sealed record Test262Report(
    SuiteRevision Suite,
    string Upstream,
    string UpstreamRevision,
    string ManifestId,
    uint FormatVersion,
    bool LoadsHarness,
    IReadOnlyList<string> Admitted,
    IReadOnlyList<string> Declined,
    int ShardIndex,
    int ShardCount,
    string Partition,
    IReadOnlyList<string> Narrowings,
    int Candidates,
    int Selected,
    ulong Fuel,
    ulong WallClock,
    IReadOnlyList<Test262Outcome> Results,
    IReadOnlyList<ConfigurationFinding> Findings)
{
    /// <summary>The header every report of this kind carries, naming the format's version.</summary>
    /// <remarks>
    /// <b>Version 2 since 2026-09-07, and version 1 is refused rather than half-read.</b> Two things
    /// moved at once: a <c>result</c> row gained the failure KIND and the file's declared FEATURES,
    /// so that a run's failures can be classified from fields rather than from prose, and the
    /// <c>run</c> row gained the upstream project and commit the retained pin names, so that a
    /// machine-readable report states which revision of which suite it is about without being handed
    /// the pin as well. A reader that accepted both arities would let one version number describe two
    /// documents, which is the shape this repository refuses everywhere else.
    /// </remarks>
    internal const string Header = "# broiler-js-conformance test262 report 2";

    /// <summary>What every version of this header begins with.</summary>
    /// <remarks>
    /// A merge decides which mode wrote a report from its first line, so a report of an EARLIER
    /// version has to be recognised as this kind in order to be refused with a message about its
    /// version, rather than handed to the other mode's reader and refused with a message about a
    /// line.
    /// </remarks>
    internal const string HeaderPrefix = "# broiler-js-conformance test262 report ";

    /// <summary>What a whole run's coverage field says.</summary>
    internal const string WholeCoverage = "whole";

    /// <summary>What a run covering less than its selection says instead.</summary>
    internal const string Partial = "partial";

    /// <summary>How many suite files this report covers, counted from the rows themselves.</summary>
    /// <remarks>
    /// Derived rather than stored, so it cannot disagree with the cases it is a count OF. A file
    /// always produces at least one row - the runner reports a skip by name rather than nothing -
    /// so a file that ran is a file that is here.
    /// </remarks>
    internal int Files =>
        Results.Select(static result => result.Path).Distinct(StringComparer.Ordinal).Count();

    /// <summary>The five verdicts, derived from the rows.</summary>
    internal Test262Totals Totals => new(
        Files,
        Results.Count,
        Results.Count(static result => result.Verdict == Test262Verdict.Passed),
        Results.Count(static result => result.Verdict == Test262Verdict.Failed),
        Results.Count(static result => result.Verdict == Test262Verdict.Unsupported),
        Results.Count(static result => result.Verdict == Test262Verdict.Exhausted),
        Results.Count(static result => result.Verdict == Test262Verdict.Skipped));

    /// <summary>The <c>unsupported</c> families this run met, most-met first.</summary>
    internal IReadOnlyList<Test262TallyRow> Families => Test262Families.Tally(
        Results
            .Where(static result => result.Verdict == Test262Verdict.Unsupported)
            .Select(static result => (result.Family, result.Path + " [" + result.Variant + "]")));

    /// <summary>The budget dimensions this run's allowances ran out on, most-met first.</summary>
    internal IReadOnlyList<Test262TallyRow> Exhaustions => Test262Families.Tally(
        Results
            .Where(static result => result.Verdict == Test262Verdict.Exhausted)
            .Select(static result => (result.Dimension, result.Path + " [" + result.Variant + "]")));

    /// <summary>Every variant that spent an allowance, named with the dimension it spent.</summary>
    /// <remarks>
    /// <b>Named individually and not only tallied</b>, because the two questions a reader has are
    /// different: the tally answers "was the ceiling too low", and this list answers "which tests
    /// does this engine not finish". A count alone cannot be followed up.
    /// </remarks>
    internal IReadOnlyList<Test262Outcome> Exhausted => Results
        .Where(static result => result.Verdict == Test262Verdict.Exhausted)
        .OrderBy(static result => result.Path, StringComparer.Ordinal)
        .ThenBy(static result => result.Variant, StringComparer.Ordinal)
        .ToArray();

    /// <summary>The failing variants grouped by the kind the harness named, most-met first.</summary>
    /// <remarks>
    /// <b>Derived from the rows exactly as the family table is, and written for a reader.</b> Three
    /// axes rather than one, because the three questions a repair queue asks are different: WHAT went
    /// wrong (kind), WHERE in the suite (area), and WHICH language feature the file declares itself
    /// to be about (feature). None of the three is read back off a report - deriving them again from
    /// the rows is what stops a merged table from disagreeing with the cases it is a breakdown of.
    /// </remarks>
    internal IReadOnlyList<Test262TallyRow> FailureKinds => Test262Failures.ByKind(Results);

    /// <summary>The failing variants grouped by the area of the suite they sit in.</summary>
    internal IReadOnlyList<Test262TallyRow> FailureAreas => Test262Failures.ByArea(Results);

    /// <summary>The failing variants grouped by the features their files declare.</summary>
    internal IReadOnlyList<Test262TallyRow> FailureFeatures => Test262Failures.ByFeature(Results);

    /// <summary>Every variant that failed, named individually and in path order.</summary>
    internal IReadOnlyList<Test262Outcome> Failures => Test262Failures.Of(Results);

    /// <summary>
    /// Why nothing about this run may be written into a record that outlives it, or nothing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A run taken without <c>--expect</c> is a run about a directory.</b> The report already
    /// carries <see cref="ConfigurationFailure.MissingSuiteRevision"/> in that case, which fails the
    /// run - but a failed run still writes a transcript, and a transcript is exactly what somebody
    /// copies into an evidence bundle six weeks later. Nothing in this harness said, in one field a
    /// tool can read, that such a transcript may not be retained; this is that field, and the floor
    /// and the JSON document both refuse a run it names a reason for.
    /// </para>
    /// <para>
    /// It is deliberately STRICTER than <see cref="Failed"/> and deliberately not the same question.
    /// A whole run of this suite fails cases today and those totals are exactly what is worth
    /// retaining; what may not be retained is a run whose identity, coverage or arithmetic was never
    /// established.
    /// </para>
    /// </remarks>
    internal IReadOnlyList<string> Unretainable
    {
        get
        {
            var reasons = new List<string>();

            if (!Suite.IsPinned)
            {
                reasons.Add(
                    "the run read no pinned suite: it was taken without `--expect <retained pin>`, " +
                    "so its totals are about a directory rather than about a revision");
            }

            if (ShardIndex != Sharding.AllShards)
            {
                reasons.Add("the run is one shard and not a merged run");
            }

            if (!IsWhole)
            {
                reasons.Add("the run covers less than its selection: " + Coverage);
            }

            if (!Totals.Accounts)
            {
                reasons.Add(
                    "the five verdicts account for " +
                    Number(Totals.Passed + Totals.Failed + Totals.Unsupported + Totals.Exhausted +
                        Totals.Skipped) +
                    " of " + Number(Totals.Variants) + " variants");
            }

            if (Totals.Variants == 0)
            {
                reasons.Add("the run scored no variant at all");
            }

            return reasons;
        }
    }

    /// <summary>Whether this run's figures may be written into a record that outlives it.</summary>
    internal bool IsRetainable => Unretainable.Count == 0;

    /// <summary>The composition this report was taken under, in the words the run printed.</summary>
    internal string DescribeManifest() =>
        Test262Manifest.Describe(ManifestId, FormatVersion, LoadsHarness, Admitted, Declined);

    /// <summary>Whether anything failed: a case, or the configuration.</summary>
    /// <remarks>
    /// <b>An exhaustion is not here and that is the point of it being its own verdict.</b> A run
    /// that met a ceiling has not found a defect, and an exit code that said it had would send a
    /// reader looking for one. What DOES fail a run in which nothing answered is the
    /// <see cref="ConfigurationFailure.NoExecutedTests"/> finding, which the command raises where a
    /// run reached a verdict about the engine in no variant at all.
    /// </remarks>
    internal bool Failed =>
        Findings.Count != 0 || Results.Any(static result => result.Verdict == Test262Verdict.Failed);

    /// <summary>Why this report covers less than a whole run, in the order a reader wants them.</summary>
    /// <remarks>
    /// The shard reason is derived and the rest are recorded, because only the first one changes
    /// when shards are merged: a merge of every shard of one run is whole, and a merge that dropped
    /// a limit or widened a subtree is not a thing that can happen.
    /// </remarks>
    internal IReadOnlyList<string> Incompleteness
    {
        get
        {
            var reasons = new List<string>();

            if (ShardIndex != Sharding.AllShards)
            {
                reasons.Add(
                    "one shard of " + ShardCount.ToString(CultureInfo.InvariantCulture));
            }

            reasons.AddRange(Narrowings);

            // The failure's NAME and once each, because this field is a summary and the `config`
            // rows below carry every finding with its detail. Two shards missing is two findings
            // and one reason a reader has to act on.
            foreach (var failure in Findings
                         .Select(static finding => finding.Failure)
                         .Distinct()
                         .OrderBy(static failure => failure.ToString(), StringComparer.Ordinal))
            {
                reasons.Add("configuration failure " + failure);
            }

            return reasons;
        }
    }

    /// <summary>Whether this report covers a whole run of what it selected.</summary>
    internal bool IsWhole => Incompleteness.Count == 0;

    /// <summary>The coverage field, in the one word a rule reads and the reasons a person does.</summary>
    internal string Coverage
    {
        get
        {
            var reasons = Incompleteness;

            return reasons.Count == 0 ? WholeCoverage : Partial + "|" + string.Join("; ", reasons);
        }
    }

    /// <summary>
    /// The configuration failures one report can carry on its own, from the closed named set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Three of the six are decidable from one report and the other three need the whole set and
    /// are the merge's - the same split the <c>--run</c> mode's report makes, stated here rather
    /// than left to whoever calls this, because a check that ran in only one of the two places is a
    /// check a run can be configured around.
    /// </para>
    /// <para>
    /// <b>A run of an unpinned checkout is a failure of that run and not a smaller total.</b> That
    /// is the same rule the sibling mode applies, and it is why a whole-suite run is taken with
    /// <c>--expect</c>: totals over a directory whose identity nobody established are totals about
    /// a directory.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<ConfigurationFinding> Validate(
        SuiteRevision suite, int selected, IReadOnlyList<Test262Outcome> results)
    {
        var findings = new List<ConfigurationFinding>();

        if (!suite.IsPinned)
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.MissingSuiteRevision,
                $"suite `{suite.Name}` was read at no pinned revision; a branch name is not a pin " +
                "and neither is nothing"));
        }

        if (selected == 0)
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.EmptySelection,
                "the selection named no file at all"));
        }

        // A VARIANT THAT ANSWERED IS ONE THAT PASSED, FAILED OR MET THE MANIFEST. A shard whose
        // every variant was skipped or spent an allowance has measured nothing about the engine,
        // and an exit code of zero on such a run is the "green run with zero executed tests" the
        // evidence matrix names as release-blocking.
        if (results.Count != 0 &&
            !results.Any(static result =>
                result.Verdict is Test262Verdict.Passed or Test262Verdict.Failed or
                    Test262Verdict.Unsupported))
        {
            findings.Add(new ConfigurationFinding(
                ConfigurationFailure.NoExecutedTests,
                $"{results.Count} variants ran and none of them reached a verdict about the engine"));
        }

        return findings;
    }

    /// <summary>Renders the report.</summary>
    internal string Render()
    {
        var text = new StringBuilder();
        text.Append(Header).Append('\n');
        text.Append("# run|suite|revision|upstream|upstreamRevision|shardIndex|shardCount|partition\n");
        text.Append("# edition|standard|year|source|revision|document|digest|archived\n");
        text.Append("# manifest|id|formatVersion|harness|admitted|declined\n");
        text.Append("# allowance|fuel|wallClockMs\n");
        text.Append("# narrowing|reason\n");
        text.Append("# selection|candidates|selected|sharded|files\n");
        text.Append("# coverage|whole, or partial and why\n");
        text.Append("# total|files|variants|passed|failed|unsupported|exhausted|skipped\n");
        text.Append("# family|name|count|example\n");
        text.Append("# exhausted|dimension|count|example\n");
        text.Append("# failure|kind|count|example\n");
        text.Append("# area|directory|count|example\n");
        text.Append("# feature|name|count|example\n");
        text.Append("# result|path|variant|verdict|family|dimension|kind|features|detail\n");
        text.Append("# config|failure|detail\n");

        // THE OTHER PINNED INPUT, WRITTEN BESIDE THE SUITE'S, exactly as the `--run` mode's report
        // writes it and for the same reason: a total is about two pinned inputs, and one that does
        // not say which document its manifests were defined against is a figure with half its
        // question missing.
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
                Upstream.Length == 0 ? "-" : Report.Cell(Upstream),
                UpstreamRevision.Length == 0 ? "-" : Report.Cell(UpstreamRevision),
                Number(ShardIndex),
                Number(ShardCount),
                Report.Cell(Partition)))
            .Append('\n');

        text.Append(string.Join(
                '|',
                "manifest",
                ManifestId,
                FormatVersion.ToString(CultureInfo.InvariantCulture),
                LoadsHarness ? "loaded" : "not-loaded",
                Admitted.Count == 0 ? "-" : string.Join(",", Admitted),
                Declined.Count == 0 ? "-" : string.Join(",", Declined)))
            .Append('\n');

        text.Append(string.Join(
                '|',
                "allowance",
                Fuel.ToString(CultureInfo.InvariantCulture),
                WallClock.ToString(CultureInfo.InvariantCulture)))
            .Append('\n');

        foreach (var narrowing in Narrowings)
        {
            text.Append("narrowing|").Append(Report.Cell(narrowing)).Append('\n');
        }

        text.Append(string.Join(
                '|',
                "selection",
                Number(Candidates),
                Number(Selected),
                Number(Results.Count),
                Number(Files)))
            .Append('\n');

        // Derived and written anyway, because the field a rule reads must be in the document a
        // rule reads. It is checked against the facts on the way back in, so a hand-edited claim to
        // be whole is refused rather than carried.
        text.Append("coverage|").Append(Coverage).Append('\n');

        var totals = Totals;

        text.Append(string.Join(
                '|',
                "total",
                Number(totals.Files),
                Number(totals.Variants),
                Number(totals.Passed),
                Number(totals.Failed),
                Number(totals.Unsupported),
                Number(totals.Exhausted),
                Number(totals.Skipped)))
            .Append('\n');

        // The tallies are derived from the rows and are written for a reader, so they are not read
        // back: parsing them would give a merge a second, independent set of figures that could
        // disagree with the cases they are a breakdown OF. That is the rule the `--run` mode's
        // report applies to its mode lines, and it is the same rule.
        foreach (var row in Families)
        {
            text.Append(string.Join(
                    '|', "family", Report.Cell(row.Name), Number(row.Count), Report.Cell(row.Example)))
                .Append('\n');
        }

        foreach (var row in Exhaustions)
        {
            text.Append(string.Join(
                    '|', "exhausted", Report.Cell(row.Name), Number(row.Count), Report.Cell(row.Example)))
                .Append('\n');
        }

        // THE THREE FAILURE AXES, derived from the rows and written beside the two tallies that
        // were already here. They are not read back, for the reason the family table is not: a merge
        // that parsed them would hold a second set of figures able to disagree with the cases they
        // are a breakdown of.
        foreach (var row in FailureKinds)
        {
            text.Append(string.Join(
                    '|', "failure", Report.Cell(row.Name), Number(row.Count), Report.Cell(row.Example)))
                .Append('\n');
        }

        foreach (var row in FailureAreas)
        {
            text.Append(string.Join(
                    '|', "area", Report.Cell(row.Name), Number(row.Count), Report.Cell(row.Example)))
                .Append('\n');
        }

        foreach (var row in FailureFeatures)
        {
            text.Append(string.Join(
                    '|', "feature", Report.Cell(row.Name), Number(row.Count), Report.Cell(row.Example)))
                .Append('\n');
        }

        foreach (var result in Results
                     .OrderBy(static result => result.Path, StringComparer.Ordinal)
                     .ThenBy(static result => result.Variant, StringComparer.Ordinal))
        {
            text.Append(string.Join(
                    '|',
                    "result",
                    Report.Cell(result.Path),
                    Report.Cell(result.Variant),
                    result.Verdict.ToString(),
                    Report.Cell(result.Family),
                    Report.Cell(result.Dimension),
                    Report.Cell(result.Kind),
                    Report.Cell(result.Features),
                    Report.Cell(result.Detail)))
                .Append('\n');
        }

        foreach (var finding in Findings)
        {
            text.Append(string.Join('|', "config", finding.Failure.ToString(), Report.Cell(finding.Detail)))
                .Append('\n');
        }

        return text.ToString();
    }

    /// <summary>Whether a file opens with this report's header.</summary>
    /// <remarks>
    /// <b>A merge is handed a directory and not a mode</b>, so it reads each report's first line to
    /// learn which kind it is rather than being told on a command line. A directory holding both
    /// kinds is two runs and is refused as one, which is the same argument that refuses two suite
    /// revisions.
    /// </remarks>
    internal static bool Recognises(string path)
    {
        foreach (var line in File.ReadLines(path))
        {
            if (line.Length == 0)
            {
                continue;
            }

            return line.StartsWith(HeaderPrefix, StringComparison.Ordinal);
        }

        return false;
    }

    /// <summary>Reads a report back, hand-parsed and refusing anything it does not recognise.</summary>
    internal static Test262Report Read(string path)
    {
        var suite = new SuiteRevision("unnamed", string.Empty);
        var upstream = string.Empty;
        var upstreamRevision = string.Empty;
        var manifestId = JavaScriptProfile.WideManifest.ToString();
        var formatVersion = 0u;
        var loadsHarness = false;
        IReadOnlyList<string> admitted = [];
        IReadOnlyList<string> declined = [];
        var shardIndex = Sharding.AllShards;
        var shardCount = 1;
        var partition = string.Empty;
        var narrowings = new List<string>();
        var candidates = 0;
        var selected = 0;
        var fuel = 0ul;
        var wallClock = 0ul;
        var coverage = string.Empty;
        var results = new List<Test262Outcome>();
        var findings = new List<ConfigurationFinding>();
        var seenHeader = false;

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                // AN EARLIER FORMAT IS REFUSED HERE AND NAMED, rather than falling through to the
                // line reader and being refused for the first row whose arity moved. A report
                // written before 2026-09-07 has seven-field results and a six-field run row, and
                // every one of them would have been reported as an unrecognised line.
                if (line.StartsWith(HeaderPrefix, StringComparison.Ordinal) &&
                    !string.Equals(line, Header, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"{path} was written by `{line}` and this build reads `{Header}`; a report " +
                        "of an earlier version is refused rather than read under the current one");
                }

                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var parts = line.Split('|');

            switch (parts[0])
            {
                case "run" when parts.Length == 8:
                    suite = new SuiteRevision(
                        parts[1],
                        string.Equals(parts[2], "unpinned", StringComparison.Ordinal) ? string.Empty : parts[2]);
                    upstream = Stated(parts[3]);
                    upstreamRevision = Stated(parts[4]);
                    shardIndex = Value(parts[5]);
                    shardCount = Value(parts[6]);
                    partition = parts[7];
                    break;

                case "manifest" when parts.Length == 6:
                    manifestId = parts[1];
                    formatVersion = uint.Parse(parts[2], CultureInfo.InvariantCulture);
                    loadsHarness = string.Equals(parts[3], "loaded", StringComparison.Ordinal);
                    admitted = Set(parts[4]);
                    declined = Set(parts[5]);
                    break;

                case "allowance" when parts.Length == 3:
                    fuel = ulong.Parse(parts[1], CultureInfo.InvariantCulture);
                    wallClock = ulong.Parse(parts[2], CultureInfo.InvariantCulture);
                    break;

                case "narrowing" when parts.Length == 2:
                    narrowings.Add(parts[1]);
                    break;

                case "selection" when parts.Length == 5:
                    candidates = Value(parts[1]);
                    selected = Value(parts[2]);
                    break;

                case "coverage":
                    coverage = line["coverage|".Length..];
                    break;

                // The edition line is checked rather than read, and a disagreement is refused
                // rather than averaged. Two shards built against two editions are two runs whatever
                // their totals look like.
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

                // Derived from the rows and written for a reader, so not read back.
                case "total":
                case "family":
                case "exhausted":
                case "failure":
                case "area":
                case "feature":
                    break;

                case "result" when parts.Length == 9:
                    results.Add(new Test262Outcome(
                        parts[1],
                        parts[2],
                        Enum.Parse<Test262Verdict>(parts[3]),
                        parts[8],
                        parts[4],
                        parts[5],
                        parts[6],
                        parts[7]));
                    break;

                case "config" when parts.Length == 3:
                    findings.Add(new ConfigurationFinding(
                        Enum.Parse<ConfigurationFailure>(parts[1]), parts[2]));
                    break;

                default:
                    throw new InvalidOperationException($"{path}: `{line}` is not a test262 report line");
            }
        }

        if (!seenHeader)
        {
            throw new InvalidOperationException($"{path} does not open with `{Header}`");
        }

        var report = new Test262Report(
            suite, upstream, upstreamRevision, manifestId, formatVersion, loadsHarness, admitted,
            declined, shardIndex, shardCount, partition, narrowings, candidates, selected, fuel,
            wallClock, results, findings);

        // THE COVERAGE CLAIM IS CHECKED AGAINST THE FACTS AND NOT TRUSTED. It is the field a rule
        // reads to decide whether a transcript is a whole-suite run, which makes it the one field
        // worth editing by hand; deriving it again from the shard index, the recorded narrowings
        // and the findings, and refusing a disagreement, is what stops a half run from claiming to
        // be a whole one on the strength of one word.
        if (!string.Equals(coverage, report.Coverage, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{path} claims coverage `{coverage}` and the run it records is `{report.Coverage}`");
        }

        return report;
    }

    /// <summary>A comma-separated set, where a lone dash is the empty one.</summary>
    private static IReadOnlyList<string> Set(string text) =>
        string.Equals(text, "-", StringComparison.Ordinal)
            ? []
            : text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    /// <summary>A cell whose lone dash means the field was not stated.</summary>
    private static string Stated(string text) =>
        string.Equals(text, "-", StringComparison.Ordinal) ? string.Empty : text;

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static int Value(string text) => int.Parse(text, CultureInfo.InvariantCulture);
}
