using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The conformance harness: the composition that scores this profile against a pinned suite.
/// </summary>
/// <remarks>
/// <para>
/// <b>An engine that grades itself is not evidence, so the first thing this program does is prove
/// it can report a failure.</b> Every run - every shard, every time - starts with the harness's own
/// regression suite and then with the self-check: deliberately broken fixtures whose declared
/// verdicts the harness has to reach, and at least one control that must pass. A mismatch stops the
/// run on an exit code of its own, before a single suite test is scored, because a shard that
/// cannot report failure has measured nothing and its totals must not be merged.
/// </para>
/// <para>
/// <b>Why this is a composition root and not a test project.</b> Scoring a test means lowering it,
/// verifying the artifact and running it, so this drives the profile's own lowering, verifier and
/// executor - and rule A11 forbids a test project to reference a profile assembly. Roadmap
/// section 5 states the consequence rather than leaving it to be found here: the harness lives in a
/// composition root that is never advertised, publishes a closure of its own for its own evidence,
/// and is cited as evidence for no other composition. Rule N13 asserts that rather than assuming
/// it.
/// </para>
/// <para>
/// <b>No third-party suite is in this repository and this program fetches none.</b> It reads a
/// directory that already exists. Retrieving, hashing and archiving a conformance suite is a human
/// action the ledger records as open, and a run pointed at a directory with no verified revision
/// reports <see cref="ConfigurationFailure.MissingSuiteRevision"/> - a failure of that run, not a
/// smaller total.
/// </para>
/// </remarks>
internal static class Program
{
    /// <summary>Where a suite keeps the tests that are scored.</summary>
    private const string TestDirectory = "test";

    /// <summary>Where a suite keeps the fixtures the self-check is made of.</summary>
    /// <remarks>
    /// Beside the scored tests and not among them. A self-check fixture is a test about the
    /// HARNESS, and scoring it in the same total would put the harness's own failures into a figure
    /// about the engine.
    /// </remarks>
    private const string SelfCheckDirectory = "selfcheck";

    /// <summary>Where a suite lists the tests it believes are themselves wrong.</summary>
    private const string KnownIncorrectFileName = "known-incorrect.txt";

    /// <summary>The name a suite carries when its pin file does not give it one.</summary>
    private const string DefaultSuiteName = "unnamed-suite";

    private static int Main(string[] args)
    {
        try
        {
            var verbose = args.Contains("--verbose", StringComparer.Ordinal);

            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            if (args.Contains("--harness-checks", StringComparer.Ordinal))
            {
                return RunHarnessChecks(verbose);
            }

            var merge = Argument(args, "--merge");

            if (merge is not null)
            {
                return MergeShards(merge, Argument(args, "--output"), verbose);
            }

            var floor = Argument(args, "--floor");

            if (floor is not null)
            {
                return Ratchet(floor, Argument(args, "--report"), args.Contains("--admit", StringComparer.Ordinal));
            }

            var suite = Argument(args, "--suite");

            if (suite is null)
            {
                Console.WriteLine(
                    "broiler-js-conformance: no --suite <directory> was given. Modes: --closure, " +
                    "--harness-checks, --self-check, --run, --write-artifacts, --pin, " +
                    "--merge <dir>, --floor <file> --report <file>. A run adds " +
                    "--dialect native|ingested, --selfcheck <dir> and --expect <retained pin>.");

                return ExitCodes.Usage;
            }

            if (args.Contains("--write-artifacts", StringComparer.Ordinal))
            {
                var written = RawFixtures.Write(Path.Combine(suite, TestDirectory, "raw"));
                Console.WriteLine($"broiler-js-conformance: wrote {written.Count} raw fixtures");
                return ExitCodes.Ok;
            }

            if (args.Contains("--pin", StringComparer.Ordinal))
            {
                return WritePin(suite);
            }

            if (!Dialect(args, out var dialect))
            {
                Console.WriteLine(
                    "broiler-js-conformance: --dialect takes `native` or `ingested`");

                return ExitCodes.Usage;
            }

            if (args.Contains("--self-check", StringComparer.Ordinal))
            {
                using var engine = Compose(out var why);

                return engine is null
                    ? Refuse(why)
                    : RunSelfCheck(SelfCheckRoot(suite, args), dialect, engine, verbose);
            }

            if (args.Contains("--run", StringComparer.Ordinal))
            {
                return RunSuite(suite, dialect, args, verbose);
            }

            Console.WriteLine("broiler-js-conformance: no mode was given beside --suite");
            return ExitCodes.Usage;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-js-conformance: unhandled {failure.GetType().Name}: {failure.Message}");

            return ExitCodes.HarnessDefect;
        }
    }

    /// <summary>Runs the harness's own regression suite.</summary>
    private static int RunHarnessChecks(bool verbose)
    {
        var checks = HarnessChecks.Run();
        var failed = Print(checks, verbose);

        Console.WriteLine(
            failed == 0
                ? $"broiler-js-conformance: {checks.Count} harness checks passed"
                : $"broiler-js-conformance: {failed} of {checks.Count} harness checks FAILED");

        return failed == 0 ? ExitCodes.Ok : ExitCodes.HarnessDefect;
    }

    /// <summary>Which dialect the suite's files are written in.</summary>
    /// <remarks>
    /// <b>The default is this component's own</b>, so a run that forgets the switch scores the
    /// fixtures it was always scoring rather than quietly translating them under rules they were
    /// not written for.
    /// </remarks>
    private static bool Dialect(string[] args, out SuiteDialect dialect)
    {
        dialect = SuiteDialect.Native;
        var declared = Argument(args, "--dialect");

        return declared switch
        {
            null or "native" => true,
            "ingested" => Ingested(out dialect),
            _ => false,
        };

        static bool Ingested(out SuiteDialect dialect)
        {
            dialect = SuiteDialect.Ingested;
            return true;
        }
    }

    /// <summary>Where the self-check fixtures are, which is not always inside the suite.</summary>
    /// <remarks>
    /// <b>A third-party checkout does not contain this component's fixtures and must not be asked
    /// to.</b> Section 14 requires the self-check to run before every shard; against an ingested
    /// suite the fixtures therefore have to come from this repository while the tests come from
    /// the checkout, which is what this switch is for. It defaults to the suite's own directory,
    /// so the fixtures this repository does hold are found without it.
    /// </remarks>
    private static string SelfCheckRoot(string suite, string[] args) =>
        Argument(args, "--selfcheck") ?? Path.Combine(suite, SelfCheckDirectory);

    /// <summary>Runs the self-check: does a failing test come back as a failure?</summary>
    private static int RunSelfCheck(string root, SuiteDialect dialect, Execution engine, bool verbose)
    {
        var cases = SelfCheck.Run(root, dialect, engine, out var complaints);

        foreach (var one in cases)
        {
            if (verbose || !one.Matched)
            {
                Console.WriteLine(
                    $"{(one.Matched ? "ok  " : "FAIL")} {one.Path}: expected " +
                    $"{one.ExpectedStatus}/{one.ExpectedCompletion}, got " +
                    $"{one.ActualStatus}/{one.ActualCompletion}" +
                    (one.Detail.Length == 0 ? string.Empty : " - " + one.Detail));
            }
        }

        foreach (var complaint in complaints)
        {
            Console.WriteLine("FAIL " + complaint);
        }

        var mismatched = cases.Count(static one => !one.Matched);

        if (cases.Count == 0)
        {
            Console.WriteLine("FAIL the self-check ran no fixture at all");
        }

        var broken = mismatched != 0 || complaints.Count != 0 || cases.Count == 0;

        Console.WriteLine(
            broken
                ? $"broiler-js-conformance: the self-check FAILED - {mismatched} of {cases.Count} " +
                    $"fixtures reported the wrong verdict and {complaints.Count} complaint(s) stand"
                : $"broiler-js-conformance: {cases.Count} self-check fixtures reported the verdict " +
                    "they declared");

        return broken ? ExitCodes.HarnessDefect : ExitCodes.Ok;
    }

    /// <summary>Scores one shard of a suite.</summary>
    private static int RunSuite(string suite, SuiteDialect dialect, string[] args, bool verbose)
    {
        // The instrument before the measurement, in that order, on every shard. A harness whose own
        // checks fail has nothing to say about an engine, and a harness that cannot report a
        // failure would report its best number ever.
        var checks = HarnessChecks.Run();

        if (Print(checks, verbose) != 0)
        {
            Console.WriteLine("broiler-js-conformance: the harness's own checks failed; nothing was scored");
            return ExitCodes.HarnessDefect;
        }

        using var engine = Compose(out var why);

        if (engine is null)
        {
            return Refuse(why);
        }

        var selfCheck = RunSelfCheck(SelfCheckRoot(suite, args), dialect, engine, verbose);

        if (selfCheck != ExitCodes.Ok)
        {
            Console.WriteLine("broiler-js-conformance: the self-check failed; nothing was scored");
            return selfCheck;
        }

        var tests = Suite.Read(Path.Combine(suite, TestDirectory), dialect, out var unreadable);
        var knownIncorrect = Selection.ReadKnownIncorrect(
            Path.Combine(suite, KnownIncorrectFileName), out var listComplaints);

        foreach (var complaint in unreadable.Concat(listComplaints))
        {
            Console.WriteLine("FAIL " + complaint);
        }

        if (unreadable.Count != 0 || listComplaints.Count != 0)
        {
            Console.WriteLine("broiler-js-conformance: the suite is not readable; nothing was scored");
            return ExitCodes.HarnessDefect;
        }

        var files = Suite.Files(suite);
        var revision = Suite.Resolve(suite, DefaultSuiteName, files, out var pinFailure);

        if (pinFailure.Length != 0)
        {
            Console.WriteLine("FAIL " + pinFailure);
        }

        // A PIN THIS REPOSITORY HOLDS, FOR A SUITE IT DOES NOT. The mode above verifies a suite
        // against a pin file inside that same suite, which is right for one this repository holds
        // and circular for one it does not: whoever can edit a third-party checkout can edit the
        // pin somebody generated inside it, in the same gesture. `--expect` names a pin retained
        // here instead, so the digest the run computed has to match a figure the suite cannot
        // reach. It is optional because a suite that carries its own pin is already answerable;
        // when it is given, a disagreement stops the run rather than shrinking a total.
        var expect = Argument(args, "--expect");

        if (expect is not null)
        {
            var retained = RetainedSuitePin.Read(expect, out var pinComplaints);

            foreach (var complaint in pinComplaints)
            {
                Console.WriteLine("FAIL " + complaint);
            }

            if (retained is null)
            {
                Console.WriteLine(
                    "broiler-js-conformance: the retained pin is not readable; nothing was scored");

                return ExitCodes.HarnessDefect;
            }

            // Over the digest this run computed from the files it read, so a pristine checkout
            // carrying no pin of its own is verifiable - which is the normal case for third-party
            // material and was the case this check refused until the archived suite was extracted
            // and pointed at it.
            var disagreements = retained.Disagrees(Suite.Digest(files), files.Count);

            foreach (var complaint in disagreements)
            {
                Console.WriteLine("FAIL " + complaint);
            }

            if (disagreements.Count != 0)
            {
                Console.WriteLine(
                    "broiler-js-conformance: the suite is not the one this pin names; nothing was " +
                    "scored");

                return ExitCodes.HarnessDefect;
            }

            Console.WriteLine("retained pin " + retained.Describe());

            // The retained pin becomes the revision this run reports. A checkout verified against
            // a pin this repository holds is pinned, whatever it says about itself, and a report
            // that called it unpinned would name the one suite here whose identity is best
            // established.
            revision = retained.AsRevision();
        }

        // WHICH CONSTRUCTS ARE IN THE LANGUAGE IS THE SUITE'S ANSWER AND NOT THIS COMPONENT'S, and
        // reading it is required rather than offered. An ingested suite declares its own feature
        // flags in two sections, one of them proposals; a run that did not read them would score
        // tests about constructs no published edition contains, which is a failure that is not a
        // gap and a pass that is not a credit. Roadmap section 3 records the language edition as an
        // unpinned dependency - this is the nearest thing to an edition that is actually pinned,
        // because the suite's revision covers it.
        var features = SuiteFeatures.None;

        if (dialect == SuiteDialect.Ingested)
        {
            features = SuiteFeatures.Read(
                Path.Combine(suite, SuiteFeatures.FileName), out var featureComplaints);

            foreach (var complaint in featureComplaints)
            {
                Console.WriteLine("FAIL " + complaint);
            }

            if (featureComplaints.Count != 0)
            {
                Console.WriteLine(
                    "broiler-js-conformance: the suite's feature list is not readable; nothing " +
                    "was scored");

                return ExitCodes.HarnessDefect;
            }

            Console.WriteLine(
                $"broiler-js-conformance: {SuiteFeatures.FileName} declares " +
                $"{features.Proposed.Count} proposed, {features.Standard.Count} standard and " +
                $"{features.TestHarness.Count} test-harness features; a test claiming a proposed " +
                "one is not scored");
        }

        var shardCount = Number(Argument(args, "--shard-count"), 1);
        var shardIndex = Number(Argument(args, "--shard-index"), Sharding.AllShards);

        if (!Sharding.Admits(shardIndex, shardCount))
        {
            Console.WriteLine(
                $"broiler-js-conformance: shard {shardIndex} of {shardCount} is not a shard this " +
                "count has");

            return ExitCodes.Usage;
        }

        var (counts, selected) = Selection.Run(
            tests,
            knownIncorrect,
            Patterns(Argument(args, "--scope")),
            features.Proposed,
            Patterns(Argument(args, "--features")),
            args.Contains("--include-negative", StringComparer.Ordinal),
            shardIndex,
            shardCount);

        // THE READER'S OWN CONTROL, and it guards the one mistake the reader can make silently. If
        // a section heading were mis-recognised - the file writes `##` for comments inside a
        // section as well as for its headings - the names below the break would land in no section
        // and this run would go back to scoring proposals without saying anything. A feature that
        // a selected test claims and the feature list does not declare is that mis-parse, or a
        // suite whose tests and whose list disagree; either way it is not something to score
        // through.
        if (dialect == SuiteDialect.Ingested)
        {
            var declared = features.All;
            var undeclared = selected
                .SelectMany(static test => test.Features)
                .Where(feature => !declared.Contains(feature))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static feature => feature, StringComparer.Ordinal)
                .ToArray();

            if (undeclared.Length != 0)
            {
                Console.WriteLine(
                    $"FAIL {undeclared.Length} feature(s) a selected test claims are declared in " +
                    $"no section of {SuiteFeatures.FileName}: {string.Join(", ", undeclared)}");

                Console.WriteLine(
                    "broiler-js-conformance: the suite and its feature list disagree; nothing " +
                    "was scored");

                return ExitCodes.HarnessDefect;
            }
        }

        // The pipeline is a partition and this is where that is asserted rather than assumed. A
        // stage that dropped a test without counting it would make every total quietly smaller.
        if (!counts.Accounts)
        {
            Console.WriteLine(
                $"broiler-js-conformance: the selection stages account for " +
                $"{counts.KnownIncorrect + counts.OutOfScope + counts.FeatureExcluded + counts.FeatureFiltered + counts.NegativeWithheld + counts.Unselectable + counts.Selected} " +
                $"of {counts.Candidates} candidates");

            return ExitCodes.HarnessDefect;
        }

        var results = new List<CaseResult>();

        foreach (var test in selected)
        {
            Observation observed;

            try
            {
                observed = engine.Run(test);
            }
            catch (Exception failure) when (failure is not OutOfMemoryException)
            {
                // A tool that stops at the first fault measures nothing. The case is a failure
                // carrying the exception type, and the run continues.
                observed = CompletionProtocol.Escaped(failure);
            }

            results.Add(new CaseResult(
                test.Path, test.Mode, observed.Status, observed.Completion, observed.Answer, observed.Detail));

            if (verbose || observed.Status != ConformanceStatus.Passed)
            {
                Console.WriteLine(
                    $"{(observed.Status == ConformanceStatus.Passed ? "ok  " : observed.Status.ToString().ToUpperInvariant())} " +
                    $"{test.Path}: {observed.Answer}" +
                    (observed.Detail.Length == 0 ? string.Empty : " - " + observed.Detail));
            }
        }

        var findings = new List<ConfigurationFinding>(Report.Validate(revision, counts, results));

        var report = new Report(revision, shardIndex, shardCount, args.Contains("--include-negative", StringComparer.Ordinal), counts, results, findings);
        var output = Argument(args, "--output");

        if (output is not null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
            File.WriteAllText(output, report.Render());
        }

        Summarize(report);
        return report.Failed ? ExitCodes.Failed : ExitCodes.Ok;
    }

    /// <summary>Merges every shard report in a directory.</summary>
    /// <remarks>
    /// <b>The merged report may not be written into the directory it merged.</b> A merge reads
    /// every report it finds, so a merged report left among the shards would be read as a shard by
    /// the next merge - and it would be read as a shard whose case list duplicates every other's,
    /// which reports as a configuration failure blaming the wrong thing. Refusing the arrangement
    /// is cheaper than teaching the reader to recognise its own output.
    /// </remarks>
    private static int MergeShards(string directory, string? output, bool verbose)
    {
        if (output is not null &&
            string.Equals(
                Path.GetFullPath(Path.GetDirectoryName(Path.GetFullPath(output))!),
                Path.GetFullPath(directory),
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                "broiler-js-conformance: --output may not write into the directory being merged, " +
                "because the next merge would read the merged report as a shard");

            return ExitCodes.Usage;
        }

        var reports = Directory
            .EnumerateFiles(directory, "*.report")
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(Report.Read)
            .ToArray();

        var merged = Merge.Combine(reports);

        if (output is not null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
            File.WriteAllText(output, merged.Render());
        }

        if (verbose)
        {
            Console.Write(merged.Render());
        }

        Console.WriteLine($"broiler-js-conformance: merged {reports.Length} shard report(s)");
        Summarize(merged);
        return merged.Failed ? ExitCodes.Failed : ExitCodes.Ok;
    }

    /// <summary>Compares a merged run against a floor, or admits it as one.</summary>
    private static int Ratchet(string floorPath, string? reportPath, bool admit)
    {
        if (reportPath is null)
        {
            Console.WriteLine("broiler-js-conformance: --floor needs a --report <file> to compare");
            return ExitCodes.Usage;
        }

        var run = Report.Read(reportPath);

        if (!Floor.Admissible(run, out var why))
        {
            Console.WriteLine($"broiler-js-conformance: this run may not be compared to a floor: {why}");
            return ExitCodes.Failed;
        }

        if (!File.Exists(floorPath))
        {
            if (!admit)
            {
                Console.WriteLine(
                    $"broiler-js-conformance: {floorPath} holds no floor; pass --admit to set one " +
                    "from this run");

                return ExitCodes.Failed;
            }

            File.WriteAllText(floorPath, Floor.From(run).Render());
            Console.WriteLine($"broiler-js-conformance: floor set from {run.Suite}");
            return ExitCodes.Ok;
        }

        var floor = Floor.Read(floorPath);
        var verdict = floor.Compare(run, out var complaints);

        foreach (var complaint in complaints)
        {
            Console.WriteLine((verdict == Floor.Verdict.Regressed ? "FAIL " : "note ") + complaint);
        }

        switch (verdict)
        {
            case Floor.Verdict.Held:
                Console.WriteLine($"broiler-js-conformance: the floor holds at {floor.Suite}");
                return ExitCodes.Ok;

            case Floor.Verdict.Regressed:
                Console.WriteLine(
                    $"broiler-js-conformance: {complaints.Count} host mode(s) fell below the floor");

                return ExitCodes.Failed;

            default:
                if (!admit)
                {
                    Console.WriteLine(
                        "broiler-js-conformance: the suite revision moved; pass --admit to re-base " +
                        "the floor onto it");

                    return ExitCodes.Failed;
                }

                File.WriteAllText(
                    floorPath,
                    floor.Rebase(run, "the suite revision moved").Render());

                Console.WriteLine($"broiler-js-conformance: floor re-based onto {run.Suite}");
                return ExitCodes.Ok;
        }
    }

    /// <summary>Writes a suite's pin from what the suite currently holds.</summary>
    /// <remarks>
    /// <b>Writing a pin is a separate act from running under one</b>, and it is deliberately a
    /// command somebody has to type. A run that silently re-pinned a suite it found had moved would
    /// turn the one check that catches an edited fixture into a no-op.
    /// </remarks>
    private static int WritePin(string suite)
    {
        var files = Suite.Files(suite);
        var name = File.Exists(Path.Combine(suite, Suite.PinFileName))
            ? Suite.Resolve(suite, DefaultSuiteName, files, out _).Name
            : DefaultSuiteName;

        var text =
            "# broiler-js-conformance suite pin\n" +
            "# revision is the digest over every path and content this suite holds, the pin excepted\n" +
            $"suite {name}\n" +
            $"revision {Suite.Digest(files)}\n" +
            $"files {files.Count.ToString(CultureInfo.InvariantCulture)}\n";

        File.WriteAllText(Path.Combine(suite, Suite.PinFileName), text);
        Console.WriteLine($"broiler-js-conformance: pinned {files.Count} files of {name}");
        return ExitCodes.Ok;
    }

    /// <summary>Prints one report's totals, one line per host mode.</summary>
    /// <remarks>
    /// <b>The edition is printed beside the suite, because both are pinned inputs and a total is
    /// about neither one alone.</b> A reader of a transcript can see which document the manifests
    /// were defined against without opening the report, and sees in the same line that the pin is
    /// provisional while nobody has archived it.
    /// </remarks>
    private static void Summarize(Report report)
    {
        Console.WriteLine("edition " + JavaScriptLanguageEdition.Describe());

        Console.WriteLine(
            $"suite {report.Suite}; shard {(report.ShardIndex == Sharding.AllShards ? "all" : report.ShardIndex.ToString(CultureInfo.InvariantCulture))} of " +
            $"{report.ShardCount.ToString(CultureInfo.InvariantCulture)}");

        Console.WriteLine(
            $"selection candidates={report.Selection.Candidates} knownIncorrect={report.Selection.KnownIncorrect} " +
            $"outOfScope={report.Selection.OutOfScope} featureExcluded={report.Selection.FeatureExcluded} " +
            $"featureFiltered={report.Selection.FeatureFiltered} " +
            $"negativeWithheld={report.Selection.NegativeWithheld} unselectable={report.Selection.Unselectable} " +
            $"selected={report.Selection.Selected} sharded={report.Selection.Sharded}");

        foreach (var totals in report.Modes)
        {
            Console.WriteLine(
                $"mode {totals.Mode}: selected={totals.Selected} executed={totals.Executed} " +
                $"passed={totals.Passed} failed={totals.Failed} skipped={totals.Skipped} " +
                $"timedOut={totals.TimedOut}");
        }

        foreach (var finding in report.Findings)
        {
            Console.WriteLine($"CONFIGURATION {finding.Failure}: {finding.Detail}");
        }
    }

    /// <summary>Prints a check list and answers how many failed.</summary>
    private static int Print(
        IReadOnlyList<(string Name, bool Passed, string Detail)> checks,
        bool verbose)
    {
        var failed = 0;

        foreach (var (name, passed, detail) in checks)
        {
            if (!passed)
            {
                failed++;
            }

            if (verbose || !passed)
            {
                Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
            }
        }

        return failed;
    }

    /// <summary>Composes the engine, printing why if it will not compose.</summary>
    private static Execution? Compose(out string failure) => Execution.Create(out failure);

    private static int Refuse(string failure)
    {
        Console.WriteLine($"broiler-js-conformance: the runtime refused creation: {failure}");
        return ExitCodes.HarnessDefect;
    }

    /// <summary>The catalog table this composition prints, in the format its siblings print.</summary>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.JavaScript.Conformance");
        Console.WriteLine("label narrow-runtime-compiler-shaped");
        Console.WriteLine("carries-lowering yes");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length));
        Console.WriteLine(string.Join(' ', "manifest", JavaScriptProfile.SliceManifest));
        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Min,
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Max));

        return ExitCodes.Ok;
    }

    private static string? Argument(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private static int Number(string? text, int fallback) =>
        text is null ? fallback : int.Parse(text, CultureInfo.InvariantCulture);

    private static IReadOnlyCollection<string> Patterns(string? text) =>
        text is null
            ? []
            : text.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}
