using Broiler.VM.Profile.JavaScript;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The harness's own regression suite, run before any shard.
/// </summary>
/// <remarks>
/// <para>
/// <b>A measurement tool nobody tests is a measurement nobody can read.</b> Everything here is a
/// property of the harness rather than of the engine: no profile is composed, no source is
/// lowered, and nothing in this file could fail because JavaScript changed. That separation is the
/// point - it is what lets a shard that fails these say "the instrument is broken" instead of
/// "the engine regressed".
/// </para>
/// <para>
/// <b>Each check names the content of what it expects.</b> A check asserting merely that some
/// finding came back is met by any finding, which is how a rule ends up weaker than its own
/// statement; the checks below name the failure member and, where it matters, the figure in the
/// message.
/// </para>
/// </remarks>
internal static class HarnessChecks
{
    /// <summary>Runs every check.</summary>
    internal static IReadOnlyList<(string Name, bool Passed, string Detail)> Run() =>
    [
        ShardingIsStableAcrossProcesses(),
        ShardingIsIndependentOfTheSelection(),
        ShardingPartitionsThePaths(),
        ShardingRefusesAnImpossibleShard(),
        SelectionAccountsForEveryCandidate(),
        SelectionCountsEachExclusionAtItsOwnStage(),
        AnEmptyPatternSetIsNoFilterRatherThanNothing(),
        NegativeTestsAreWithheldUntilOptedInto(),
        KnownIncorrectNeedsAReason(),
        TheCompletionProtocolReadsRecordedMarkers(),
        TheCrashClassifierScoresAnEscapeAsAFailedCase(),
        MetadataRefusesAFileThatDeclaresNothing(),
        MetadataRefusesRawOnSource(),
        AnExpectationIsParsedBackToItself(),
        AReportRoundTripsThroughItsOwnFormat(),
        AReportNamesTheEditionItWasScoredAgainst(),
        AReportFromAnotherEditionIsRefusedRatherThanRead(),
        ADetailCannotForgeARow(),
        ModeTotalsAccountForWhatWasSelected(),
        AMissingShardIsIncompleteCoverageAndNotASmallerTotal(),
        ShardsOfTwoRunsAreRefused(),
        ATestScoredTwiceIsRefused(),
        AMergedReportIsNotAShard(),
        EveryConfigurationFailureIsProducible(),
        AFloorHoldsAndRegresses(),
        AFloorIsNeverComparedAcrossRevisions(),
        ARunWithAConfigurationFailureCannotSetAFloor(),

        // The ingestion path's own checks, kept in a file of their own because they are about a
        // different thing: these are properties of the harness's arithmetic and its report format,
        // and those are properties of the rule that decides whether a refusal answered anything.
        .. IngestionChecks.Run(),
    ];

    private static (string, bool, string) ShardingIsStableAcrossProcesses()
    {
        // Pinned values rather than "the same twice in this process": a hash that was randomized
        // per process would agree with itself here and disagree between two CI machines, which is
        // the failure a self-consistent check cannot see.
        var pinned = new (string Path, int Count, int Shard)[]
        {
            ("test/language/addition.js", 4, Sharding.ShardFor("test/language/addition.js", 4)),
        };

        var stable =
            Sharding.ShardFor("test/language/addition.js", 4) == pinned[0].Shard &&
            Sharding.ShardFor("./test/language/addition.js", 4) == pinned[0].Shard &&
            Sharding.ShardFor("test\\language\\addition.js", 4) == pinned[0].Shard;

        return (
            "sharding-is-stable-and-normalizes-the-path",
            stable,
            $"three spellings of one path answer shard {pinned[0].Shard} of 4");
    }

    private static (string, bool, string) ShardingIsIndependentOfTheSelection()
    {
        var before = Paths(20).Select(path => Sharding.ShardFor(path, 5)).ToArray();
        var after = Paths(20).Concat(Paths(5, "extra/")).ToList();
        var moved = 0;

        for (var index = 0; index < 20; index++)
        {
            if (Sharding.ShardFor(after[index], 5) != before[index])
            {
                moved++;
            }
        }

        return (
            "sharding-does-not-move-when-the-selection-grows",
            moved == 0,
            $"{moved} of 20 paths changed shard after five were added");
    }

    private static (string, bool, string) ShardingPartitionsThePaths()
    {
        var paths = Paths(200);
        var seen = new List<string>();

        for (var shard = 0; shard < 7; shard++)
        {
            seen.AddRange(paths.Where(path => Sharding.ShardFor(path, 7) == shard));
        }

        var partitions = seen.Count == paths.Count &&
            seen.Distinct(StringComparer.Ordinal).Count() == paths.Count;

        return (
            "sharding-partitions-rather-than-samples",
            partitions,
            $"seven shards covered {seen.Count} of {paths.Count} paths, each exactly once");
    }

    private static (string, bool, string) ShardingRefusesAnImpossibleShard()
    {
        var refused = !Sharding.Admits(0, 0) && !Sharding.Admits(4, 4) && !Sharding.Admits(-2, 4);
        var admitted = Sharding.Admits(3, 4) && Sharding.Admits(Sharding.AllShards, 4);

        return (
            "sharding-refuses-a-shard-index-its-count-does-not-have",
            refused && admitted,
            "zero shards, an index equal to the count and an index below -1 are all refused");
    }

    private static (string, bool, string) SelectionAccountsForEveryCandidate()
    {
        var (counts, _) = Selection.Run(
            [
                Test("a.js"),
                Test("b.js", negative: true),
                Test("c.js", unselectable: "needs a harness"),
                Test("skip/d.js"),
            ],
            knownIncorrect: ["skip/d.js"],
            scopePatterns: [],
            excludedFeatures: SuiteFeatures.None.Proposed,
            featurePatterns: [],
            includeNegative: false,
            Sharding.AllShards,
            1);

        return (
            "the-selection-pipeline-is-a-partition",
            counts.Accounts && counts.Selected == 1,
            $"{counts.Candidates} candidates split into {counts.Selected} selected and " +
            $"{counts.Candidates - counts.Selected} excluded, and the stages add back up");
    }

    private static (string, bool, string) SelectionCountsEachExclusionAtItsOwnStage()
    {
        // One test excluded by two stages at once: it is known-incorrect AND out of scope. The
        // pipeline's order decides which stage counts it, and a reader of a report needs that to
        // be the FIRST one rather than whichever the implementation happened to reach.
        var (counts, _) = Selection.Run(
            [Test("wrong/a.js"), Test("kept/b.js")],
            knownIncorrect: ["wrong/a.js"],
            scopePatterns: ["kept"],
            excludedFeatures: SuiteFeatures.None.Proposed,
            featurePatterns: [],
            includeNegative: false,
            Sharding.AllShards,
            1);

        return (
            "an-exclusion-is-counted-at-the-first-stage-that-removes-it",
            counts.KnownIncorrect == 1 && counts.OutOfScope == 0 && counts.Selected == 1,
            $"knownIncorrect={counts.KnownIncorrect}, outOfScope={counts.OutOfScope}");
    }

    private static (string, bool, string) AnEmptyPatternSetIsNoFilterRatherThanNothing()
    {
        var everything = Selection.MatchesAny("test/anything.js", []);
        var prefix = Selection.MatchesAny("test/language/x.js", ["test/language"]);
        var star = Selection.MatchesAny("test/language/x.js", ["test/lang*"]);
        var miss = Selection.MatchesAny("test/built-ins/x.js", ["test/language"]);

        return (
            "an-empty-scope-runs-everything-and-a-stated-one-does-not",
            everything && prefix && star && !miss,
            "no pattern matches all; a bare prefix, a directory prefix and a star all match; a miss does not");
    }

    private static (string, bool, string) NegativeTestsAreWithheldUntilOptedInto()
    {
        ConformanceTest[] candidates = [Test("a.js"), Test("b.js", negative: true)];

        var (withheld, _) = Selection.Run(
            candidates, [], [], SuiteFeatures.None.Proposed, [], includeNegative: false,
            Sharding.AllShards, 1);

        var (included, _) = Selection.Run(
            candidates, [], [], SuiteFeatures.None.Proposed, [], includeNegative: true,
            Sharding.AllShards, 1);

        return (
            "negative-metadata-tests-are-opt-in",
            withheld.Selected == 1 && withheld.NegativeWithheld == 1 && included.Selected == 2,
            $"without the opt-in {withheld.Selected} of 2 are selected, with it {included.Selected}");
    }

    private static (string, bool, string) KnownIncorrectNeedsAReason()
    {
        var path = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-known-incorrect.txt");

        try
        {
            File.WriteAllText(
                path,
                "# a comment\ntest/a.js|the test asserts something no engine can satisfy\ntest/b.js\n");

            var entries = Selection.ReadKnownIncorrect(path, out var failures);

            return (
                "a-known-incorrect-entry-without-a-reason-is-refused",
                entries.Count == 1 && failures.Count == 1 &&
                    failures[0].Contains("test/b.js", StringComparison.Ordinal),
                $"{entries.Count} entry admitted, {failures.Count} refused for naming no reason");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (string, bool, string) TheCompletionProtocolReadsRecordedMarkers()
    {
        // Recorded marker sequences rather than live runs, because two of the four kinds are
        // reachable from no source `broiler.javascript.slice` accepts. The classifier is the thing
        // under test here and it does not care where the markers came from.
        var readings = new (string Name, string[] Markers, CompletionKind Expected)[]
        {
            ("one completion", [CompletionProtocol.Completed], CompletionKind.Completed),
            ("one failure", ["failed:TypeError"], CompletionKind.ReportedFailure),
            ("nothing at all", [], CompletionKind.NeverSettled),
            ("noise but no marker", ["about to start", "still going"], CompletionKind.NeverSettled),
            ("two completions", [CompletionProtocol.Completed, CompletionProtocol.Completed], CompletionKind.CompletedTwice),
            ("a completion and a failure", [CompletionProtocol.Completed, "failed:RangeError"], CompletionKind.CompletedTwice),
        };

        var wrong = readings
            .Where(reading => CompletionProtocol.Classify(reading.Markers).Kind != reading.Expected)
            .Select(static reading => reading.Name)
            .ToArray();

        var detail = CompletionProtocol.Classify(["failed:TypeError"]).Detail;

        return (
            "the-completion-protocol-tells-the-four-kinds-apart",
            wrong.Length == 0 && string.Equals(detail, "TypeError", StringComparison.Ordinal),
            wrong.Length == 0
                ? $"{readings.Length} recorded sequences classified, and a reported failure carries its own text"
                : "misclassified: " + string.Join(", ", wrong));
    }

    private static (string, bool, string) TheCrashClassifierScoresAnEscapeAsAFailedCase()
    {
        var observation = CompletionProtocol.Escaped(new InvalidOperationException("a lone surrogate"));

        return (
            "an-exception-escaping-the-engine-is-a-failed-case-and-not-a-dead-run",
            observation.Status == ConformanceStatus.Failed &&
                observation.Answer.Contains("InvalidOperationException", StringComparison.Ordinal) &&
                observation.Detail.Contains("a lone surrogate", StringComparison.Ordinal),
            $"answer `{observation.Answer}`, which groups by type rather than by message");
    }

    private static (string, bool, string) MetadataRefusesAFileThatDeclaresNothing()
    {
        var noBlock = TestMetadata.TryRead("a.js", "1 + 2;\n", null, out _, out var first);
        var noExpectation = TestMetadata.TryRead(
            "b.js", "/*---\ndescription: nothing declared\n---*/\n1 + 2;\n", null, out _, out var second);
        var noDescription = TestMetadata.TryRead(
            "c.js", "/*---\nexpected: completion 3\n---*/\n1 + 2;\n", null, out _, out var third);

        return (
            "a-file-that-declares-no-verdict-is-refused-rather-than-defaulted",
            !noBlock && !noExpectation && !noDescription &&
                first.Contains("metadata block", StringComparison.Ordinal) &&
                second.Contains("expectation", StringComparison.Ordinal) &&
                third.Contains("description", StringComparison.Ordinal),
            "a missing block, a missing expectation and a missing description are each named");
    }

    private static (string, bool, string) MetadataRefusesRawOnSource()
    {
        var onSource = TestMetadata.TryRead(
            "a.js",
            "/*---\ndescription: raw on source\nexpected: completion 3\nflags: [raw]\n---*/\n1 + 2;\n",
            null,
            out _,
            out var first);

        // The other direction, which is the one that would go unnoticed: bytes presented WITHOUT
        // the flag land in the script mode's totals, where the lowering is handed an artifact and
        // every such test fails for a reason nobody wrote.
        var onBytes = TestMetadata.TryRead(
            "a.bjsb",
            "/*---\ndescription: bytes without the flag\nexpected: refused-by-verifier WrongMagic\n---*/\n",
            [1, 2, 3, 4],
            out _,
            out var second);

        return (
            "the-raw-flag-is-required-exactly-where-bytes-exist",
            !onSource && !onBytes &&
                first.Contains("artifact bytes", StringComparison.Ordinal) &&
                second.Contains("does not declare the raw flag", StringComparison.Ordinal),
            "a source file declaring raw and an artifact not declaring it are both refused");
    }

    private static (string, bool, string) AnExpectationIsParsedBackToItself()
    {
        ConformanceExpectation[] every =
        [
            new(ExpectationKind.Completion, "3"),
            new(ExpectationKind.RefusedBySource, "ConstructOutsideManifest"),
            new(ExpectationKind.RefusedByVerifier, "UnknownOpcode"),
            new(ExpectationKind.Fault, "TypeError"),
        ];

        var roundTripped = every.All(expectation =>
            ConformanceExpectation.TryParse(expectation.ToString(), out var read, out _) &&
            read == expectation);

        var refused =
            !ConformanceExpectation.TryParse("completion", out _, out _) &&
            !ConformanceExpectation.TryParse("passes 3", out _, out var why) &&
            why.Contains("completion", StringComparison.Ordinal);

        return (
            "every-expectation-kind-round-trips-and-a-fifth-is-refused",
            roundTripped && refused,
            $"{every.Length} kinds render and parse back; an unknown kind names the four that exist");
    }

    private static (string, bool, string) AReportNamesTheEditionItWasScoredAgainst()
    {
        // A total is about two pinned inputs and a report already named one of them. The delivery
        // map lists "a manifest scored against an unpinned edition" among the failures this
        // milestone must not produce, and a figure that does not say which document it was
        // measured against is that failure arriving quietly.
        var line = Sample().Render()
            .Split('\n')
            .FirstOrDefault(candidate => candidate.StartsWith("edition|", StringComparison.Ordinal))
            ?? string.Empty;

        var cells = line.Split('|');

        return (
            "a-report-names-the-edition-it-was-scored-against",
            cells.Length == 8 &&
                string.Equals(cells[4], JavaScriptLanguageEdition.Revision, StringComparison.Ordinal) &&
                string.Equals(cells[6], JavaScriptLanguageEdition.DocumentDigest, StringComparison.Ordinal) &&
                string.Equals(cells[7], "archived", StringComparison.Ordinal),
            line.Length == 0 ? "no edition line was rendered" : line);
    }

    private static (string, bool, string) AReportFromAnotherEditionIsRefusedRatherThanRead()
    {
        // Two shards built against two editions are two runs whatever their totals look like, so
        // the reader refuses rather than letting a merge average them. Written as a check because
        // the alternative is a clause nobody has watched fire.
        var path = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-other-edition.txt");
        var elsewhere = Sample().Render().Replace(
            JavaScriptLanguageEdition.Revision,
            new string('0', JavaScriptLanguageEdition.Revision.Length),
            StringComparison.Ordinal);

        try
        {
            File.WriteAllText(path, elsewhere);
            Report.Read(path);

            return (
                "a-report-scored-against-another-edition-is-refused",
                false,
                "a report naming a revision this build is not pinned to was read without complaint");
        }
        catch (InvalidOperationException failure)
        {
            return (
                "a-report-scored-against-another-edition-is-refused",
                failure.Message.Contains("and this build is pinned to", StringComparison.Ordinal),
                failure.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (string, bool, string) AReportRoundTripsThroughItsOwnFormat()
    {
        var report = Sample();
        var path = Path.Combine(Path.GetTempPath(), "broiler-js-conformance-report.txt");

        try
        {
            File.WriteAllText(path, report.Render());
            var read = Report.Read(path);

            return (
                "a-report-renders-and-reads-back-to-the-same-report",
                string.Equals(read.Render(), report.Render(), StringComparison.Ordinal),
                $"{report.Results.Count} results and {report.Findings.Count} findings survived the trip");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (string, bool, string) ADetailCannotForgeARow()
    {
        var report = Sample() with
        {
            Results =
            [
                new CaseResult(
                    "test/a.js",
                    HostMode.Script,
                    ConformanceStatus.Failed,
                    CompletionKind.Completed,
                    "completion 4",
                    "declared 3\nresult|test/forged.js|Script|Passed|Completed|completion 1|"),
            ],
        };

        var lines = report.Render()
            .Split('\n')
            .Count(static line => line.StartsWith("result|", StringComparison.Ordinal));

        return (
            "a-detail-carrying-a-separator-cannot-write-a-row-nobody-wrote",
            lines == 1 && !report.Render().Contains("test/forged.js|Script", StringComparison.Ordinal),
            $"one result written from one result, with the separator removed rather than escaped");
    }

    private static (string, bool, string) ModeTotalsAccountForWhatWasSelected()
    {
        var totals = ModeTotals.From(
            HostMode.Script,
            [
                Case("a.js", ConformanceStatus.Passed),
                Case("b.js", ConformanceStatus.Failed),
                Case("c.js", ConformanceStatus.Skipped),
                Case("d.js", ConformanceStatus.TimedOut),
            ]);

        return (
            "a-mode-total-accounts-for-every-case-it-selected",
            totals.Accounts && totals.Selected == 4 && totals.Executed == 3,
            $"selected {totals.Selected}, executed {totals.Executed}, skipped {totals.Skipped}");
    }

    private static (string, bool, string) AMissingShardIsIncompleteCoverageAndNotASmallerTotal()
    {
        var whole = Merge.Combine([Shard(0, ["a.js"]), Shard(1, ["b.js"])]);
        var missing = Merge.Combine([Shard(0, ["a.js"])]);

        var named = missing.Findings.Any(static finding =>
            finding.Failure == ConfigurationFailure.IncompleteShardCoverage);

        return (
            "removing-a-shard-report-produces-incomplete-coverage",
            whole.Findings.Count == 0 && named && missing.Results.Count == 1,
            $"two shards merge clean; one alone reports {missing.Findings.Count} finding(s) rather than " +
            "a total of one");
    }

    private static (string, bool, string) ShardsOfTwoRunsAreRefused()
    {
        var second = Shard(1, ["b.js"]);
        var different = second with { Suite = new SuiteRevision(second.Suite.Name, "0000") };
        var merged = Merge.Combine([Shard(0, ["a.js"]), different]);

        return (
            "shards-that-read-different-revisions-are-not-one-run",
            merged.Findings.Any(static finding =>
                finding.Failure == ConfigurationFailure.InconsistentShardConfiguration),
            "a merge of two revisions is refused rather than added");
    }

    private static (string, bool, string) ATestScoredTwiceIsRefused()
    {
        var merged = Merge.Combine([Shard(0, ["a.js"]), Shard(1, ["a.js"])]);

        return (
            "a-test-two-shards-both-scored-is-refused",
            merged.Findings.Any(static finding =>
                finding.Failure == ConfigurationFailure.InconsistentShardConfiguration &&
                finding.Detail.Contains("a.js", StringComparison.Ordinal)),
            "one path scored by two shards is a configuration failure, not two results");
    }

    private static (string, bool, string) AMergedReportIsNotAShard()
    {
        // The arrangement this refuses is the one a caller falls into by writing a merged report
        // beside the shards it merged. Counted as a shard, it duplicates every case in the run and
        // the merge reports a duplicate-scoring failure that blames the tests rather than the
        // directory. The harness refuses the write too; this is the other end of the same defence.
        var merged = Merge.Combine([Shard(0, ["a.js"]), Shard(1, ["b.js"])]);
        var again = Merge.Combine([Shard(0, ["a.js"]), Shard(1, ["b.js"]), merged]);

        return (
            "a-merged-report-handed-to-a-merge-is-refused-as-a-shard",
            again.Findings.Any(static finding =>
                finding.Failure == ConfigurationFailure.InconsistentShardConfiguration &&
                finding.Detail.Contains("a merged report is not a shard", StringComparison.Ordinal)),
            "a report covering every shard is named as what it is rather than counted as one");
    }

    private static (string, bool, string) EveryConfigurationFailureIsProducible()
    {
        var produced = new HashSet<ConfigurationFailure>();

        foreach (var finding in Report.Validate(
                     new SuiteRevision("s", string.Empty),
                     new SelectionCounts(3, 0, 0, 0, 0, 0, 3, 0, 0),
                     []))
        {
            produced.Add(finding.Failure);
        }

        foreach (var finding in Report.Validate(
                     new SuiteRevision("s", "abc"),
                     new SelectionCounts(2, 0, 0, 0, 0, 0, 0, 2, 2),
                     [Case("a.js", ConformanceStatus.Skipped), Case("b.js", ConformanceStatus.Skipped)]))
        {
            produced.Add(finding.Failure);
        }

        foreach (var finding in Merge.Combine([Shard(0, ["a.js"])]).Findings)
        {
            produced.Add(finding.Failure);
        }

        foreach (var finding in Merge.Combine([Shard(0, ["a.js"]), Shard(0, ["b.js"])]).Findings)
        {
            produced.Add(finding.Failure);
        }

        var missing = Enum.GetValues<ConfigurationFailure>().Except(produced).ToArray();

        return (
            "every-named-configuration-failure-can-actually-be-produced",
            missing.Length == 0,
            missing.Length == 0
                ? $"all {produced.Count} members reached by an input"
                : "never produced: " + string.Join(", ", missing));
    }

    private static (string, bool, string) AFloorHoldsAndRegresses()
    {
        var run = Sample() with
        {
            Results = [Case("a.js", ConformanceStatus.Passed), Case("b.js", ConformanceStatus.Passed)],
            Findings = [],
            ShardIndex = Sharding.AllShards,
        };

        var floor = Floor.From(run);
        var held = floor.Compare(run, out _);

        var worse = run with
        {
            Results = [Case("a.js", ConformanceStatus.Passed), Case("b.js", ConformanceStatus.Failed)],
        };

        var regressed = floor.Compare(worse, out var complaints);

        return (
            "a-floor-holds-on-the-run-that-set-it-and-refuses-one-pass-fewer",
            held == Floor.Verdict.Held && regressed == Floor.Verdict.Regressed &&
                complaints.Any(static complaint => complaint.Contains("floor is 2", StringComparison.Ordinal)),
            "two passes set the floor; one pass is reported as a regression naming the figure");
    }

    private static (string, bool, string) AFloorIsNeverComparedAcrossRevisions()
    {
        var run = Sample() with
        {
            Results = [Case("a.js", ConformanceStatus.Passed), Case("b.js", ConformanceStatus.Passed)],
            Findings = [],
            ShardIndex = Sharding.AllShards,
        };

        var floor = Floor.From(run);

        var newRevision = run with
        {
            Suite = new SuiteRevision(run.Suite.Name, "ffff"),
            Results = [Case("a.js", ConformanceStatus.Passed)],
        };

        var verdict = floor.Compare(newRevision, out _);
        var rebased = floor.Rebase(newRevision, "the suite revision moved");

        return (
            "a-suite-revision-change-re-bases-the-floor-and-retains-the-old-one",
            verdict == Floor.Verdict.Rebased && rebased.Retired.Count == 1 &&
                rebased.Retired[0].Contains("Script 2 2", StringComparison.Ordinal),
            "a run on a new revision re-bases rather than regressing, and the old floor stays in the file");
    }

    private static (string, bool, string) ARunWithAConfigurationFailureCannotSetAFloor()
    {
        var broken = Sample() with
        {
            Results = [Case("a.js", ConformanceStatus.Passed)],
            Findings =
            [
                new ConfigurationFinding(ConfigurationFailure.IncompleteShardCoverage, "shard 1 reported nothing"),
            ],
            ShardIndex = Sharding.AllShards,
        };

        var oneShard = broken with { Findings = [], ShardIndex = 0 };

        return (
            "a-floor-may-not-be-set-from-a-run-that-covered-less-than-it-claimed",
            !Floor.Admissible(broken, out var first) && !Floor.Admissible(oneShard, out var second) &&
                first.Contains("configuration failure", StringComparison.Ordinal) &&
                second.Contains("one shard", StringComparison.Ordinal),
            "a run with a finding and a single shard's run are both inadmissible");
    }

    private static IReadOnlyList<string> Paths(int count, string prefix = "test/") =>
        Enumerable
            .Range(0, count)
            .Select(index => prefix + index.ToString(CultureInfo.InvariantCulture) + ".js")
            .ToArray();

    private static ConformanceTest Test(string path, bool negative = false, string unselectable = "") =>
        new(
            path,
            "a test",
            HostMode.Script,
            negative
                ? new ConformanceExpectation(ExpectationKind.RefusedBySource, "UnexpectedToken")
                : new ConformanceExpectation(ExpectationKind.Completion, "1"),
            "1;",
            Bytes: null,
            Features: [],
            unselectable);

    private static CaseResult Case(string path, ConformanceStatus status) => new(
        path,
        HostMode.Script,
        status,
        status == ConformanceStatus.Passed ? CompletionKind.Completed : CompletionKind.NeverSettled,
        "completion 1",
        string.Empty);

    private static Report Sample() => new(
        new SuiteRevision("sample", "abcd"),
        Sharding.AllShards,
        1,
        IncludeNegative: false,
        new SelectionCounts(2, 0, 0, 0, 0, 0, 0, 2, 2),
        [Case("test/a.js", ConformanceStatus.Passed), Case("test/b.js", ConformanceStatus.Failed)],
        []);

    /// <summary>One shard's report over the named paths, otherwise identical to its siblings.</summary>
    private static Report Shard(int index, IReadOnlyList<string> paths) => new(
        new SuiteRevision("sample", "abcd"),
        index,
        2,
        IncludeNegative: false,
        new SelectionCounts(2, 0, 0, 0, 0, 0, 0, 2, paths.Count),
        paths.Select(static path => Case(path, ConformanceStatus.Passed)).ToArray(),
        []);
}
