// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using System.Text.Json;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The whole-run machinery's own regression checks: the partition, the merge, the family table and
/// the coverage field.
/// </summary>
/// <remarks>
/// <para>
/// <b>Not one of these composes a profile or lowers a line of JavaScript.</b> They are properties of
/// the instrument that takes a whole-suite run - that the shards cover the selection exactly once,
/// that merging them says the same thing as not sharding at all, that a family a run met is named
/// and one it did not meet is absent, and that a partial run cannot render as a whole one. A check
/// here failing means the transcript cannot be read, whatever the engine did.
/// </para>
/// <para>
/// <b>They are in a file of their own for the reason the ingestion checks are.</b> The harness's
/// arithmetic and its report format are one subject; how a third-party suite is cut into processes
/// and put back together is another, and a single eight-hundred-line list is a list nobody reads
/// before adding to it.
/// </para>
/// </remarks>
internal static class Test262Checks
{
    /// <summary>Runs every check.</summary>
    internal static IReadOnlyList<(string Name, bool Passed, string Detail)> Run() =>
    [
        ThePartitionIsExhaustiveAndDisjoint(),
        AShardArgumentIsReadOrRefused(),
        AMergeOfEveryShardEqualsAnUnshardedRun(),
        AMissingShardIsIncompleteCoverageAndNotASmallerTotal(),
        ShardsOfTwoManifestsAreNotOneRun(),
        TheFamilyTableNamesAFamilyTheRunMet(),
        AnExhaustionIsItsOwnVerdictAndNamesItsDimension(),
        APartialRunCannotRenderAsAWholeOne(),
        ATest262ReportRoundTripsThroughItsOwnFormat(),
        AReportOfAnEarlierFormatIsRefusedAndNotHalfRead(),
        TheFailureClassificationAccountsForEveryFailure(),
        AFeatureTaggingIsNotAPartitionAndSaysSo(),
        TheJsonDocumentParsesAndCarriesEveryDeclaredMember(),
        AWholeRunFloorHoldsAndRegressesInBothDirections(),
        AWholeRunFloorIsNeverComparedAcrossARevisionOrAManifest(),
        ARunTakenWithoutAPinMayNotBeRetainedOrSetAFloor(),
    ];

    private static (string, bool, string) AReportOfAnEarlierFormatIsRefusedAndNotHalfRead()
    {
        // A FORMAT CHANGE IS A FORMAT CHANGE, and the failure mode this refuses is the quiet one: a
        // version-1 report has seven-field results and a six-field run row, so a reader that only
        // checked the header PREFIX would refuse it one line at a time and blame a row.
        var path = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-test262-earlier.txt");

        try
        {
            File.WriteAllText(
                path,
                "# broiler-js-conformance test262 report 1\nrun|test262|abcd|-1|1|rule\n");

            var recognised = Test262Report.Recognises(path);
            var refused = string.Empty;

            try
            {
                Test262Report.Read(path);
            }
            catch (InvalidOperationException failure)
            {
                refused = failure.Message;
            }

            return (
                "a-test262-report-of-an-earlier-format-is-recognised-as-one-and-then-refused",
                recognised && refused.Contains("earlier version", StringComparison.Ordinal),
                recognised
                    ? "a version-1 report is recognised as this kind and refused by version: " + refused
                    : "a version-1 report was not recognised as a test262 report at all");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (string, bool, string) TheFailureClassificationAccountsForEveryFailure()
    {
        // THE PROPERTY THAT MAKES THE TABLE CHECKABLE. Kind and area each file every failing variant
        // exactly once, so both columns sum to the failed total; a classification that dropped a row
        // - because a verdict was reached somewhere that names no kind, say - would produce a work
        // list quietly smaller than the number it is a breakdown of.
        var results = new List<Test262Outcome>
        {
            new("test/built-ins/Array/prototype/map/a.js", "strict", Test262Verdict.Failed,
                "uncaught TypeError: x", Kind: Test262Failures.Uncaught("TypeError")),
            new("test/built-ins/Array/prototype/map/b.js", "sloppy", Test262Verdict.Failed,
                "uncaught TypeError: y", Kind: Test262Failures.Uncaught("TypeError")),
            new("test/language/statements/class/c.js", "strict", Test262Verdict.Failed,
                "a harness file threw: z", Kind: Test262Failures.HarnessThrew("Test262Error")),

            // The one with no kind, which must become a row rather than vanish.
            new("test/language/statements/class/d.js", "strict", Test262Verdict.Failed, "something"),
            new("test/built-ins/Array/prototype/map/e.js", "strict", Test262Verdict.Passed, string.Empty),
        };

        var report = ReportOver(["x"], Sharding.AllShards, 1, ["x"], results);
        var kinds = report.FailureKinds;
        var areas = report.FailureAreas;
        var unclassified = kinds.FirstOrDefault(static row =>
            string.Equals(row.Name, Test262Failures.Unclassified, StringComparison.Ordinal));

        var kindTotal = kinds.Sum(static row => row.Count);
        var areaTotal = areas.Sum(static row => row.Count);

        return (
            "the-failure-classification-files-every-failing-variant-exactly-once",
            kindTotal == 4 && areaTotal == 4 && report.Totals.Failed == 4 &&
                unclassified is { Count: 1 } && areas.Count == 2 &&
                areas.Any(static row => string.Equals(
                    row.Name, "test/built-ins/Array/prototype", StringComparison.Ordinal)) &&
                areas.Any(static row => string.Equals(
                    row.Name, "test/language/statements/class", StringComparison.Ordinal)),
            $"{kindTotal} kind and {areaTotal} area taggings over {report.Totals.Failed} failures, " +
            $"{areas.Count} areas, and a verdict naming no kind is filed under " +
            $"`{Test262Failures.Unclassified}` rather than dropped");
    }

    private static (string, bool, string) AFeatureTaggingIsNotAPartitionAndSaysSo()
    {
        // A test262 file declares zero, one or several `features`, so the feature axis is a set of
        // tags rather than a partition and its column deliberately does NOT sum to the failures. The
        // check is that both facts hold: two features put one variant in two rows, and a variant
        // with none is in no row at all.
        var results = new List<Test262Outcome>
        {
            new("test/a.js", "strict", Test262Verdict.Failed, "uncaught", Kind: "k",
                Features: "Proxy,Reflect"),
            new("test/b.js", "strict", Test262Verdict.Failed, "uncaught", Kind: "k",
                Features: "Proxy"),
            new("test/c.js", "strict", Test262Verdict.Failed, "uncaught", Kind: "k"),
        };

        var report = ReportOver(["x"], Sharding.AllShards, 1, ["x"], results);
        var features = report.FailureFeatures;
        var proxy = features.FirstOrDefault(static row =>
            string.Equals(row.Name, "Proxy", StringComparison.Ordinal));

        return (
            "a-declared-feature-is-a-tag-and-not-a-partition-of-the-failures",
            features.Count == 2 && proxy is { Count: 2 } &&
                features.Sum(static row => row.Count) == 3 && report.Totals.Failed == 3 &&
                Test262Failures.WithAFeature(report.Results) == 2,
            "three failures declare three taggings over two of them, so the feature column does not " +
            "sum to the failed total and the report states how many variants it covers");
    }

    private static (string, bool, string) TheJsonDocumentParsesAndCarriesEveryDeclaredMember()
    {
        // ONE LIST, TWO DOCUMENTS. The renderer walks the member list and the schema is generated
        // from it, so the check that matters is that what came out is well-formed JSON whose
        // top-level members are exactly that list, in that order - which is the promise the schema
        // makes to a consumer that never reads this repository.
        var files = Paths(20);
        var report = ReportOver(files, Sharding.AllShards, 1, files);

        try
        {
            using var document = JsonDocument.Parse(Test262Json.Render(report));
            using var schema = JsonDocument.Parse(Test262Json.Schema());

            var emitted = document.RootElement
                .EnumerateObject()
                .Select(static member => member.Name)
                .ToArray();

            var declared = Test262Json.Members.Select(static member => member.Name).ToArray();

            var promised = schema.RootElement
                .GetProperty("properties")
                .EnumerateObject()
                .Select(static member => member.Name)
                .ToArray();

            // And the totals in the document are the report's own, not a second count of the rows.
            var totals = document.RootElement.GetProperty("totals");
            var failures = document.RootElement.GetProperty("failures").GetArrayLength();

            var agrees = totals.GetProperty("failed").GetInt32() == failures &&
                totals.GetProperty("variants").GetInt32() == report.Results.Count &&
                document.RootElement.GetProperty("retention").GetProperty("retainable").GetBoolean();

            return (
                "the-json-report-parses-and-its-members-are-the-schema-s-in-order",
                emitted.SequenceEqual(declared, StringComparer.Ordinal) &&
                    promised.SequenceEqual(declared, StringComparer.Ordinal) && agrees,
                $"{emitted.Length} members rendered, {promised.Length} promised by the schema, and " +
                $"{failures} failure record(s) against a failed total of " +
                totals.GetProperty("failed").GetInt32().ToString(CultureInfo.InvariantCulture));
        }
        catch (JsonException failure)
        {
            return (
                "the-json-report-parses-and-its-members-are-the-schema-s-in-order",
                false,
                "the rendered document is not JSON: " + failure.Message);
        }
    }

    private static (string, bool, string) AWholeRunFloorHoldsAndRegressesInBothDirections()
    {
        // THE RATCHET THE WHOLE-SUITE FIGURE HAD NONE OF. A run equal to the floor holds; a run that
        // passes fewer variants regresses; and - the half a one-directional ratchet walks around - a
        // run that passes as many while FAILING more regresses too, because a construct that moved
        // from passed to unsupported or a file that stopped being readable is a regression a pass
        // count cannot see.
        var files = Paths(60);
        var run = ReportOver(files, Sharding.AllShards, 1, files);
        var floor = Test262Floor.From(run);

        var held = floor.Compare(run, out _) == Floor.Verdict.Held;

        var fewer = ReportOver(
            files,
            Sharding.AllShards,
            1,
            files,
            [.. run.Results.Select(static result => result.Verdict == Test262Verdict.Passed
                ? result with { Verdict = Test262Verdict.Failed, Kind = "a kind" }
                : result)]);

        var passesFell = floor.Compare(fewer, out var fell) == Floor.Verdict.Regressed &&
            fell.Any(static complaint => complaint.Contains("`passed`", StringComparison.Ordinal));

        // Same passes, one skip turned into a failure: the `atMost failed` row is what catches it.
        var worse = ReportOver(
            files,
            Sharding.AllShards,
            1,
            files,
            [.. run.Results.Select(static result => result.Verdict == Test262Verdict.Skipped
                ? result with { Verdict = Test262Verdict.Failed, Kind = "a kind" }
                : result)]);

        var failuresRose = floor.Compare(worse, out var rose) == Floor.Verdict.Regressed &&
            rose.Any(static complaint => complaint.Contains("`failed`", StringComparison.Ordinal));

        return (
            "a-whole-run-floor-holds-and-catches-a-fall-in-passes-and-a-rise-in-failures",
            held && passesFell && failuresRose,
            held
                ? "the run that set the floor holds it; fewer passes and more failures each regress"
                : "the run that set the floor did not hold it");
    }

    private static (string, bool, string) AWholeRunFloorIsNeverComparedAcrossARevisionOrAManifest()
    {
        // A floor compared across revisions reads an added test as a regression and a removed one as
        // a lower bar; a floor compared across manifests reads a deliberate composition as a
        // catastrophe. Both re-base rather than compare, and a re-base retains the old rows.
        var files = Paths(30);
        var run = ReportOver(files, Sharding.AllShards, 1, files);
        var floor = Test262Floor.From(run);

        var moved = run with { Suite = new SuiteRevision("test262", "efgh") };
        var other = run with { ManifestId = "broiler.javascript.slice" };

        var rebasedRevision = floor.Compare(moved, out _) == Floor.Verdict.Rebased;
        var rebasedManifest = floor.Compare(other, out _) == Floor.Verdict.Rebased;
        var rebased = floor.Rebase(moved, "the suite revision moved");

        var round = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-test262-floor.txt");
        var trip = false;

        try
        {
            File.WriteAllText(round, rebased.Render());
            trip = string.Equals(
                Test262Floor.Read(round).Render(), rebased.Render(), StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(round);
        }

        return (
            "a-whole-run-floor-re-bases-across-a-revision-or-a-manifest-and-retains-the-old-rows",
            rebasedRevision && rebasedManifest && rebased.Retired.Count == 1 && trip &&
                rebased.Retired[0].Contains("test262@abcd", StringComparison.Ordinal),
            "a moved revision and a changed manifest both re-base rather than compare, the retired " +
            "rows stay in the file, and the file round-trips");
    }

    private static (string, bool, string) ARunTakenWithoutAPinMayNotBeRetainedOrSetAFloor()
    {
        // THE CLAUSE NOTHING CHECKED. A run without `--expect` already carries
        // MissingSuiteRevision - but a failed run still writes a transcript, and a transcript is
        // what somebody copies into an evidence bundle. Three places now refuse it by the same
        // field: the floor, the JSON document's `retention`, and the driver that takes the run.
        var files = Paths(20);
        var pinned = ReportOver(files, Sharding.AllShards, 1, files);

        var unpinned = pinned with
        {
            Suite = new SuiteRevision("test262", string.Empty),
            Findings =
            [
                new ConfigurationFinding(
                    ConfigurationFailure.MissingSuiteRevision, "no pin was given"),
            ],
        };

        var shard = ReportOver(Test262Partition.Take(files, 1, 4), 1, 4, files);

        var refusedUnpinned = !Test262Floor.Admissible(unpinned, out var whyUnpinned);
        var refusedShard = !Test262Floor.Admissible(shard, out var whyShard);
        var admitted = Test262Floor.Admissible(pinned, out _);

        var said = Test262Json.Render(unpinned).Contains(
            "\"retainable\": false", StringComparison.Ordinal);

        return (
            "a-run-taken-without-a-retained-pin-may-not-be-retained-and-may-not-set-a-floor",
            refusedUnpinned && refusedShard && admitted && said && !unpinned.IsRetainable &&
                whyUnpinned.Contains("--expect", StringComparison.Ordinal) &&
                whyShard.Contains("one shard", StringComparison.Ordinal),
            refusedUnpinned
                ? "an unpinned run and a single shard are both refused a floor, and the JSON " +
                    "document says `retainable: false` with the reason"
                : "an unpinned run was admitted as a floor");
    }

    private static (string, bool, string) ThePartitionIsExhaustiveAndDisjoint()
    {
        // THE PROPERTY A WHOLE RUN IS ASSEMBLED FROM. Every selected file lands in exactly one
        // shard, so `n` processes between them run the selection and nothing twice. Checked over
        // several counts rather than one, because a partition can be exhaustive at four and lose a
        // file at seven only if the arithmetic is wrong in a way one count hides.
        var files = Paths(500);
        var wrong = new List<string>();

        foreach (var count in (int[])[1, 2, 3, 4, 7, 16, 64])
        {
            var seen = new List<string>();

            for (var shard = 0; shard < count; shard++)
            {
                seen.AddRange(Test262Partition.Take(files, shard, count));
            }

            if (seen.Count != files.Count ||
                seen.Distinct(StringComparer.Ordinal).Count() != files.Count)
            {
                wrong.Add(
                    count.ToString(CultureInfo.InvariantCulture) + " covered " +
                    seen.Count.ToString(CultureInfo.InvariantCulture));
            }
        }

        var unsharded = Test262Partition.Take(files, Sharding.AllShards, 4);

        return (
            "the-test262-partition-is-exhaustive-and-disjoint",
            wrong.Count == 0 && unsharded.Count == files.Count,
            wrong.Count == 0
                ? $"{files.Count} files, each in exactly one shard at 1, 2, 3, 4, 7, 16 and 64 shards"
                : "not a partition at " + string.Join(", ", wrong));
    }

    private static (string, bool, string) AShardArgumentIsReadOrRefused()
    {
        var read = Test262Partition.TryRead("3/8", out var index, out var count, out _) &&
            index == 3 && count == 8;

        var absent = Test262Partition.TryRead(null, out var whole, out var one, out _) &&
            whole == Sharding.AllShards && one == 1;

        // The refusals, and the third is the one that would go unnoticed: `4/4` names a shard the
        // count does not have, and a run that took it would score nothing and report a clean zero.
        var refused =
            !Test262Partition.TryRead("3", out _, out _, out _) &&
            !Test262Partition.TryRead("a/b", out _, out _, out _) &&
            !Test262Partition.TryRead("4/4", out _, out _, out _) &&
            !Test262Partition.TryRead("-1/4", out _, out _, out _);

        return (
            "a-shard-argument-is-read-or-refused-and-never-defaulted",
            read && absent && refused,
            "`3/8` reads; no --shard is the whole selection; `3`, `a/b`, `4/4` and `-1/4` are refused");
    }

    private static (string, bool, string) AMergeOfEveryShardEqualsAnUnshardedRun()
    {
        // THE CLAIM A WHOLE RUN RESTS ON, checked rather than argued. The same file list produces
        // the same outcomes; one report over all of it and a merge of four shards of it must agree
        // on every verdict, every family row and every exhausted dimension - not merely on the
        // total, which two different runs can share.
        var files = Paths(120);
        var whole = ReportOver(files, Sharding.AllShards, 1, files);

        var shards = new List<Test262Report>();

        for (var shard = 0; shard < 4; shard++)
        {
            shards.Add(ReportOver(Test262Partition.Take(files, shard, 4), shard, 4, files));
        }

        var merged = Merge.Combine(shards);

        var totals = merged.Totals == whole.Totals;
        var families = Same(merged.Families, whole.Families);
        var exhaustions = Same(merged.Exhaustions, whole.Exhaustions);
        var cases = merged.Results.Count == whole.Results.Count;

        return (
            "a-merge-of-every-shard-equals-an-unsharded-run-over-the-same-list",
            totals && families && exhaustions && cases && merged.Findings.Count == 0 && merged.IsWhole,
            totals && families && exhaustions
                ? $"four shards of {files.Count} files merge to {merged.Totals.Describe()[2..]}, " +
                    $"with {merged.Families.Count} families and {merged.Exhaustions.Count} dimensions " +
                    "identical to the unsharded run"
                : $"merged {merged.Totals.Describe()} against unsharded {whole.Totals.Describe()}");
    }

    private static (string, bool, string) AMissingShardIsIncompleteCoverageAndNotASmallerTotal()
    {
        var files = Paths(40);
        var shards = new List<Test262Report>();

        for (var shard = 0; shard < 4; shard++)
        {
            shards.Add(ReportOver(Test262Partition.Take(files, shard, 4), shard, 4, files));
        }

        var complete = Merge.Combine(shards);
        var missing = Merge.Combine(shards.Take(3).ToArray());

        var named = missing.Findings.Any(static finding =>
            finding.Failure == ConfigurationFailure.IncompleteShardCoverage);

        return (
            "removing-a-test262-shard-report-produces-incomplete-coverage",
            complete.Findings.Count == 0 && named && !missing.IsWhole &&
                missing.Coverage.Contains("IncompleteShardCoverage", StringComparison.Ordinal),
            "three of four shards report incomplete coverage and a coverage field that says partial, " +
            "not a whole run of a smaller suite");
    }

    private static (string, bool, string) ShardsOfTwoManifestsAreNotOneRun()
    {
        // JSW-10 asks for a run PER MANIFEST. Two shards taken under two manifests have two
        // `unsupported` columns about two different questions, and a merge that added them would
        // publish a family table describing neither composition.
        var files = Paths(20);
        var first = ReportOver(Test262Partition.Take(files, 0, 2), 0, 2, files);
        var second = ReportOver(Test262Partition.Take(files, 1, 2), 1, 2, files) with
        {
            ManifestId = "broiler.javascript.slice",
        };

        var declined = ReportOver(Test262Partition.Take(files, 1, 2), 1, 2, files) with
        {
            Declined = ["broiler.javascript.binary"],
        };

        var manifests = Merge.Combine([first, second]);
        var surfaces = Merge.Combine([first, declined]);

        return (
            "shards-taken-under-two-manifests-or-two-admitted-sets-are-not-one-run",
            manifests.Findings.Any(static finding =>
                finding.Failure == ConfigurationFailure.InconsistentShardConfiguration &&
                finding.Detail.Contains("manifest", StringComparison.Ordinal)) &&
            surfaces.Findings.Any(static finding =>
                finding.Failure == ConfigurationFailure.InconsistentShardConfiguration &&
                finding.Detail.Contains("declined", StringComparison.Ordinal)),
            "a merge of two manifests and a merge of two declined sets are both refused rather than added");
    }

    private static (string, bool, string) TheFamilyTableNamesAFamilyTheRunMet()
    {
        // The messages are the front end's own shape - `JsParser` composes every named refusal as
        // the construct followed by the manifest suffix - so this check is about the reader rather
        // than about a sentence invented here.
        var results = new List<Test262Outcome>
        {
            Unsupported("test/a.js", "strict", "a class declaration is not admitted by the declared feature manifest"),
            Unsupported("test/b.js", "sloppy", "a class declaration is not admitted by the declared feature manifest"),
            Unsupported("test/a.js", "sloppy", "a generator function is not admitted by the declared feature manifest"),
            Unsupported("test/c.js", "strict", "a call with more than 255 arguments is not admitted"),
            new("test/d.js", "strict", Test262Verdict.Passed, string.Empty),
        };

        var table = Test262Families.Tally(
            results
                .Where(static result => result.Verdict == Test262Verdict.Unsupported)
                .Select(static result => (result.Family, result.Path + " [" + result.Variant + "]")));

        var classes = table.FirstOrDefault(static row =>
            string.Equals(row.Name, "a class declaration", StringComparison.Ordinal));

        // AND A FAMILY THE RUN DID NOT MEET IS ABSENT, which is what makes the table read forward:
        // there is no static list to fall out of date, so a family the front end has stopped
        // refusing stops appearing the day it stops being refused.
        var absent = table.All(static row =>
            !string.Equals(row.Name, "a spread element", StringComparison.Ordinal));

        return (
            "the-family-table-names-a-family-the-run-met-and-omits-one-it-did-not",
            table.Count == 3 && classes is { Count: 2 } &&
                string.Equals(classes.Example, "test/a.js [strict]", StringComparison.Ordinal) &&
                absent &&
                table[0].Count >= table[^1].Count,
            table.Count == 0
                ? "the table named nothing"
                : $"{table.Count} families, most-met first: " +
                    string.Join(", ", table.Select(static row => row.Name)));
    }

    private static (string, bool, string) AnExhaustionIsItsOwnVerdictAndNamesItsDimension()
    {
        // AN ABSENCE, A REFUSAL, A FAILURE AND A CEILING ARE FOUR DIFFERENT ANSWERS. A run whose
        // failed column silently carried "we did not wait long enough" is a run whose failures
        // nobody can act on, so the totals keep them apart and the dimension is named.
        var report = ReportOver(
            ["test/a.js"],
            Sharding.AllShards,
            1,
            ["test/a.js"],
            [
                new Test262Outcome("test/a.js", "strict", Test262Verdict.Failed, "uncaught TypeError"),
                new Test262Outcome(
                    "test/a.js", "sloppy", Test262Verdict.Exhausted, "the allowance was spent",
                    Family: string.Empty, Dimension: "WallClock"),
            ]);

        var totals = report.Totals;
        var dimension = report.Exhaustions.SingleOrDefault();

        return (
            "an-exhaustion-is-counted-apart-from-a-failure-and-names-its-dimension",
            totals.Failed == 1 && totals.Exhausted == 1 && totals.Accounts &&
                dimension is { Count: 1 } &&
                string.Equals(dimension.Name, "WallClock", StringComparison.Ordinal) &&
                report.Exhausted.Count == 1,
            $"one failure and one exhaustion, and the exhausted variant is named with `{dimension?.Name}`");
    }

    private static (string, bool, string) APartialRunCannotRenderAsAWholeOne()
    {
        var files = Paths(16);
        var whole = ReportOver(files, Sharding.AllShards, 1, files);
        var shard = ReportOver(Test262Partition.Take(files, 1, 4), 1, 4, files);

        var limited = whole with
        {
            Narrowings = ["--limit kept 8 of 16 files"],
        };

        var named = whole with
        {
            Narrowings = ["a named selection rather than the whole test tree: test/built-ins"],
        };

        var claimed = 0;

        foreach (var partial in (Test262Report[])[shard, limited, named])
        {
            if (!partial.IsWhole &&
                partial.Coverage.StartsWith(Test262Report.Partial, StringComparison.Ordinal) &&
                partial.Render().Contains("\ncoverage|partial|", StringComparison.Ordinal))
            {
                claimed++;
            }
        }

        return (
            "a-sharded-limited-or-narrowed-run-says-so-where-a-rule-can-see-it",
            claimed == 3 && whole.IsWhole &&
                whole.Render().Contains("\ncoverage|whole\n", StringComparison.Ordinal),
            $"{claimed} of 3 partial runs render `coverage|partial`, and the whole one renders " +
            "`coverage|whole`");
    }

    private static (string, bool, string) ATest262ReportRoundTripsThroughItsOwnFormat()
    {
        var files = Paths(12);
        var report = ReportOver(Test262Partition.Take(files, 0, 3), 0, 3, files);
        var path = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-test262-report.txt");

        try
        {
            File.WriteAllText(path, report.Render());
            var read = Test262Report.Read(path);
            var same = string.Equals(read.Render(), report.Render(), StringComparison.Ordinal);

            // AND A HAND-EDITED COVERAGE CLAIM IS REFUSED, because that field is the one worth
            // editing: it is what a rule reads to decide whether a transcript is a whole-suite run.
            File.WriteAllText(
                path,
                report.Render().Replace("\ncoverage|partial|", "\ncoverage|whole\n#", StringComparison.Ordinal));

            var forged = false;

            try
            {
                Test262Report.Read(path);
            }
            catch (InvalidOperationException failure)
            {
                forged = failure.Message.Contains("claims coverage", StringComparison.Ordinal);
            }

            return (
                "a-test262-report-round-trips-and-a-forged-coverage-claim-is-refused",
                same && forged,
                $"{report.Results.Count} results survived the trip; a report edited to claim it was " +
                "whole is refused rather than believed");
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>Whether two tallies name the same rows with the same counts and examples.</summary>
    private static bool Same(
        IReadOnlyList<Test262TallyRow> left, IReadOnlyList<Test262TallyRow> right) =>
        left.Count == right.Count && left.Zip(right).All(static pair => pair.First == pair.Second);

    private static Test262Outcome Unsupported(string path, string variant, string message) =>
        new(path, variant, Test262Verdict.Unsupported, message, Test262Families.Of(message));

    /// <summary>
    /// The outcomes a synthetic file produces, chosen from its own name so that two runs over one
    /// list produce identical rows.
    /// </summary>
    /// <remarks>
    /// <b>Deterministic in the path and not in a counter</b>, because a shard runs a subset in a
    /// different order and a fixture keyed on position would make the merge and the unsharded run
    /// differ for a reason that has nothing to do with either.
    /// </remarks>
    private static IEnumerable<Test262Outcome> Outcomes(string path)
    {
        var seed = Sharding.ShardFor(path, 10);

        yield return seed switch
        {
            0 or 1 or 2 => new Test262Outcome(path, "strict", Test262Verdict.Passed, string.Empty),
            3 => new Test262Outcome(path, "strict", Test262Verdict.Failed, "uncaught TypeError"),
            4 => new Test262Outcome(
                path, "strict", Test262Verdict.Exhausted, "the allowance was spent",
                Family: string.Empty, Dimension: "Fuel"),
            5 => new Test262Outcome(
                path, "strict", Test262Verdict.Exhausted, "the allowance was spent",
                Family: string.Empty, Dimension: "WallClock"),
            6 => new Test262Outcome(path, "strict", Test262Verdict.Skipped, "this manifest admits no module goal"),
            7 => Unsupported(path, "strict", "a class declaration is not admitted by the declared feature manifest"),
            8 => Unsupported(path, "strict", "a generator function is not admitted by the declared feature manifest"),
            _ => Unsupported(path, "strict", "a spread element is not admitted by the declared feature manifest"),
        };

        yield return new Test262Outcome(path, "sloppy", Test262Verdict.Passed, string.Empty);
    }

    /// <summary>One report over a list of files, otherwise identical to its siblings.</summary>
    private static Test262Report ReportOver(
        IReadOnlyList<string> mine,
        int shardIndex,
        int shardCount,
        IReadOnlyList<string> selection,
        IReadOnlyList<Test262Outcome>? results = null) =>
        new(
            new SuiteRevision("test262", "abcd"),
            "tc39/test262",
            "ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e",
            Test262Manifest.Default,
            2,
            LoadsHarness: true,
            ["broiler.javascript.binary", "broiler.javascript.dynamic"],
            [],
            shardIndex,
            shardCount,
            Test262Partition.Rule,
            [],
            selection.Count,
            selection.Count,
            2_000_000_000,
            5_000,
            results ?? mine.SelectMany(Outcomes).ToArray(),
            []);

    private static IReadOnlyList<string> Paths(int count) =>
        Enumerable
            .Range(0, count)
            .Select(static index => "test/built-ins/case" + index.ToString(CultureInfo.InvariantCulture) + ".js")
            .ToArray();
}
