// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The <c>--test262</c> mode: run a real test262 checkout under a named manifest and report what
/// every variant did.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a separate mode from <c>--run</c> and reads a different tree.</b> <c>--run</c> scores
/// this component's own fixture tree, which is the only scoring target that existed before there
/// was an engine to score. This one takes the path of an unpacked third-party suite the repository
/// does not hold, and reports five verdicts rather than two - because "this manifest does not admit
/// that construct" is not a failure and must never be counted as a pass, and because "the allowance
/// ran out" is neither.
/// </para>
/// <para>
/// <b>What it now takes is a whole suite, and that is what JSW-10 asks for.</b> A run with no
/// <c>--test</c> and no <c>--dir</c> takes every <c>.js</c> file under the checkout's
/// <c>test/</c> tree; a run naming a subtree says so in its report; and a run cut into shards says
/// which shard it was and how the partition was computed. Tens of thousands of files will not
/// finish in one process in a useful time, so a whole run is <c>n</c> processes and a merge - and
/// the report carries what a merge needs to prove those <c>n</c> processes covered the selection
/// rather than a subset of it.
/// </para>
/// <para>
/// <b>It publishes no aggregate this repository may cite as a conformance figure on its own.</b> A
/// run over a list somebody chose is a measurement of that list, which is why the coverage field
/// exists and why a report that is not whole says so where a rule can see it. What the mode is for
/// is the thing the workload roadmap asks for and a ratchet cannot give: a manifest's own run over
/// the pinned suite, with every family it declines named.
/// </para>
/// </remarks>
internal static class Test262Command
{
    /// <summary>The instruction allowance one test variant gets unless a caller states one.</summary>
    private const ulong DefaultFuel = 2_000_000_000;

    /// <summary>The wall-clock allowance one test variant gets, in milliseconds.</summary>
    /// <remarks>
    /// <b>Generous, because a caller pointing this mode at ONE test is debugging and wants the
    /// answer rather than the ceiling.</b> A whole-suite run cannot afford it - a minute a variant
    /// over fifty thousand files is not a run anybody takes - so the driver that takes a whole run
    /// states a much smaller figure of its own and prints it. The default is deliberately not that
    /// figure: a default sized for the whole suite would silently turn a single-test investigation
    /// into a five-second one.
    /// </remarks>
    private const ulong DefaultWallClock = 60_000;

    /// <summary>Where a test262 checkout keeps the files that are scored.</summary>
    private const string TestDirectory = "test";

    /// <summary>Runs the mode.</summary>
    internal static int Run(string suiteRoot, string[] args, bool verbose)
    {
        if (!Directory.Exists(suiteRoot))
        {
            Console.WriteLine("broiler-js-conformance: no directory at " + suiteRoot);
            return ExitCodes.Usage;
        }

        if (!Directory.Exists(Path.Combine(suiteRoot, "harness")))
        {
            Console.WriteLine(
                "broiler-js-conformance: " + suiteRoot + " has no harness/ directory, so it is not " +
                "the root of a test262 checkout");

            return ExitCodes.Usage;
        }

        if (!Test262Manifest.TryParse(
                Argument(args, "--manifest"), Repeated(args, "--decline"), out var manifest, out var why))
        {
            Console.WriteLine("broiler-js-conformance: " + why);
            return ExitCodes.Usage;
        }

        if (!Test262Partition.TryRead(
                Argument(args, "--shard"), out var shardIndex, out var shardCount, out var badShard))
        {
            Console.WriteLine("broiler-js-conformance: " + badShard);
            return ExitCodes.Usage;
        }

        var narrowings = new List<string>();
        var candidates = Collect(suiteRoot, args, narrowings);

        if (candidates is null)
        {
            return ExitCodes.Usage;
        }

        var selected = Limit(candidates, args, narrowings);
        var mine = Test262Partition.Take(selected, shardIndex, shardCount);

        var suite = new SuiteRevision("test262", string.Empty);
        var expect = Argument(args, "--expect");

        if (expect is not null)
        {
            // A PIN THIS REPOSITORY HOLDS, FOR A SUITE IT DOES NOT, checked by every shard rather
            // than once by whoever launched them. A shard that reported a revision it had not read
            // would be certifying its own input, which is the exact shape `SuitePins` exists to
            // refuse; re-reading the checkout costs seconds against a run that costs hours.
            if (!Verify(suiteRoot, expect, out suite))
            {
                return ExitCodes.HarnessDefect;
            }
        }

        var fuel = Number(args, "--fuel", DefaultFuel);
        var wallClock = Number(args, "--wall", DefaultWallClock);

        Console.WriteLine("edition " + JavaScriptLanguageEdition.Describe());
        Console.WriteLine("suite " + suite);
        Console.WriteLine(manifest.Describe());
        Console.WriteLine("partition " + Test262Partition.Describe(shardIndex, shardCount));

        Console.WriteLine(
            "selection candidates=" + Count(candidates.Count) + " selected=" + Count(selected.Count) +
            " thisShard=" + Count(mine.Count));

        Console.WriteLine(
            "allowance fuel=" + fuel.ToString(CultureInfo.InvariantCulture) +
            " wallClockMs=" + wallClock.ToString(CultureInfo.InvariantCulture) + " per variant");

        var results = new List<Test262Outcome>();

        foreach (var relative in mine)
        {
            foreach (var outcome in Test262Run.RunOne(suiteRoot, relative, manifest, fuel, wallClock))
            {
                results.Add(outcome);

                Console.WriteLine(
                    Mark(outcome.Verdict) + " " + outcome.Path + " [" + outcome.Variant + "]" +
                    (verbose || outcome.Verdict != Test262Verdict.Passed
                        ? Because(outcome)
                        : string.Empty));
            }
        }

        var report = new Test262Report(
            suite,
            manifest.Id.ToString(),
            manifest.FormatVersion,
            manifest.LoadsHarness,
            manifest.Admitted,
            manifest.Declined,
            shardIndex,
            shardCount,

            // THE RULE AND NOT THIS SHARD'S PLACE IN IT. Every shard of one run states the same
            // partition, so a merge can refuse two runs cut two different ways; which shard this
            // one was is the run line's `shardIndex` two fields above, and writing it here as well
            // would make every shard disagree with every other about a field they share.
            Test262Partition.Rule,
            narrowings,
            candidates.Count,
            selected.Count,
            fuel,
            wallClock,
            results,
            Test262Report.Validate(suite, selected.Count, results));

        Summarize(report);

        var output = Argument(args, "--report");

        if (output is not null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
            File.WriteAllText(output, report.Render());
        }

        return report.Failed ? ExitCodes.Failed : ExitCodes.Ok;
    }

    /// <summary>Prints one report's tallies, its totals and its coverage, in that order.</summary>
    /// <remarks>
    /// <b>Coverage is printed last as well as written into the report.</b> A reader looking at the
    /// tail of a transcript is looking at the totals, and the one thing that decides what those
    /// totals are about is whether the run covered the whole suite.
    /// </remarks>
    internal static void Summarize(Test262Report report)
    {
        foreach (var line in Test262Families.Describe(
                     report.Families,
                     "unsupported families",
                     "this run met no construct outside the manifest"))
        {
            Console.WriteLine(line);
        }

        foreach (var line in Test262Families.Describe(
                     report.Exhaustions,
                     "exhausted dimensions",
                     "no variant spent an allowance"))
        {
            Console.WriteLine(line);
        }

        foreach (var spent in report.Exhausted)
        {
            Console.WriteLine(
                "# exhausted " + spent.Path + " [" + spent.Variant + "] on " + spent.Dimension);
        }

        Console.WriteLine(report.Totals.Describe());
        Console.WriteLine("coverage " + report.Coverage.Replace('|', ' '));

        foreach (var finding in report.Findings)
        {
            Console.WriteLine($"CONFIGURATION {finding.Failure}: {finding.Detail}");
        }
    }

    /// <summary>
    /// Every file this run selected, before any limit and before the partition.
    /// </summary>
    /// <remarks>
    /// <b>Naming nothing means the whole <c>test/</c> tree, and naming something is recorded as a
    /// narrowing.</b> The mode used to require a <c>--test</c> or a <c>--dir</c>, which made every
    /// run a run over a list somebody chose - and a transcript of such a run is indistinguishable
    /// from a whole-suite one once the command line has scrolled away. A run over the whole tree is
    /// now what the mode does by default, and a subtree is the thing that has to be declared.
    /// </remarks>
    private static List<string>? Collect(string suiteRoot, string[] args, List<string> narrowings)
    {
        var named = new List<string>();
        var wanted = new List<string>();

        foreach (var relative in Repeated(args, "--test"))
        {
            named.Add(relative);
            wanted.Add(Suite.Normalize(relative));
        }

        foreach (var relative in Repeated(args, "--dir"))
        {
            named.Add(relative);
            var normalized = Suite.Normalize(relative);
            var directory = Path.Combine(suiteRoot, normalized.Replace('/', Path.DirectorySeparatorChar));

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("broiler-js-conformance: no directory at " + directory);
                return null;
            }

            wanted.AddRange(Under(suiteRoot, directory));
        }

        if (named.Count == 0)
        {
            var whole = Path.Combine(suiteRoot, TestDirectory);

            if (!Directory.Exists(whole))
            {
                Console.WriteLine("broiler-js-conformance: no directory at " + whole);
                return null;
            }

            wanted.AddRange(Under(suiteRoot, whole));
        }
        else
        {
            narrowings.Add("a named selection rather than the whole test tree: " + string.Join(" ", named));
        }

        // Ordinally sorted and de-duplicated, because the partition is a function of the list and
        // two callers naming overlapping directories must not put one file in the run twice - which
        // a merge would report as a test scored by two shards and blame on the shards.
        wanted.Sort(StringComparer.Ordinal);

        var distinct = new List<string>(wanted.Count);

        foreach (var file in wanted)
        {
            if (distinct.Count == 0 ||
                !string.Equals(distinct[^1], file, StringComparison.Ordinal))
            {
                distinct.Add(file);
            }
        }

        return distinct;
    }

    /// <summary>Every <c>.js</c> file under a directory, as suite-relative paths.</summary>
    private static IEnumerable<string> Under(string suiteRoot, string directory) =>
        Directory
            .EnumerateFiles(directory, "*.js", SearchOption.AllDirectories)
            .Select(file => Suite.Normalize(Path.GetRelativePath(suiteRoot, file)));

    /// <summary>Applies <c>--limit</c>, recording it as a narrowing rather than announcing it once.</summary>
    /// <remarks>
    /// <b>A cap is a property of the report and not only a line in a log.</b> The mode already
    /// announced a limit on the console; a transcript that scrolled past it and a report that did
    /// not carry it left a partial run looking exactly like a whole one, which is the failure this
    /// repository's records exist to prevent.
    /// </remarks>
    private static IReadOnlyList<string> Limit(
        List<string> candidates, string[] args, List<string> narrowings)
    {
        var limit = (int)Number(args, "--limit", (ulong)int.MaxValue);

        if (candidates.Count <= limit)
        {
            return candidates;
        }

        narrowings.Add(
            "--limit kept " + Count(limit) + " of " + Count(candidates.Count) + " files");

        return candidates.GetRange(0, limit);
    }

    /// <summary>Checks the checkout against a pin this repository holds, or says why it is not it.</summary>
    private static bool Verify(string suiteRoot, string expect, out SuiteRevision suite)
    {
        suite = new SuiteRevision("test262", string.Empty);
        var retained = RetainedSuitePin.Read(expect, out var complaints);

        foreach (var complaint in complaints)
        {
            Console.WriteLine("FAIL " + complaint);
        }

        if (retained is null)
        {
            Console.WriteLine(
                "broiler-js-conformance: the retained pin is not readable; nothing was scored");

            return false;
        }

        var files = Suite.Files(suiteRoot);
        var disagreements = retained.Disagrees(Suite.Digest(files), files.Count);

        foreach (var complaint in disagreements)
        {
            Console.WriteLine("FAIL " + complaint);
        }

        if (disagreements.Count != 0)
        {
            Console.WriteLine(
                "broiler-js-conformance: the checkout is not the one this pin names; nothing was scored");

            return false;
        }

        Console.WriteLine("retained pin " + retained.Describe());
        suite = retained.AsRevision();
        return true;
    }

    /// <summary>The reason a variant's line carries, where it has one.</summary>
    private static string Because(Test262Outcome outcome)
    {
        var reason = outcome.Verdict switch
        {
            Test262Verdict.Unsupported when outcome.Family.Length != 0 => outcome.Family,
            Test262Verdict.Exhausted when outcome.Dimension.Length != 0 =>
                outcome.Dimension + ": " + outcome.Detail,
            _ => outcome.Detail,
        };

        return reason.Length == 0 ? string.Empty : "  " + reason;
    }

    /// <summary>Every value a repeated option was given, in the order it was given.</summary>
    private static IReadOnlyList<string> Repeated(string[] args, string option)
    {
        var values = new List<string>();

        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], option, StringComparison.Ordinal))
            {
                values.Add(args[index + 1]);
            }
        }

        return values;
    }

    private static string? Argument(string[] args, string option)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], option, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private static ulong Number(string[] args, string option, ulong fallback)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], option, StringComparison.Ordinal) &&
                ulong.TryParse(args[index + 1], out var stated) &&
                stated != 0)
            {
                return stated;
            }
        }

        return fallback;
    }

    private static string Mark(Test262Verdict verdict) => verdict switch
    {
        Test262Verdict.Passed => "pass       ",
        Test262Verdict.Failed => "FAIL       ",
        Test262Verdict.Unsupported => "unsupported",
        Test262Verdict.Exhausted => "exhausted  ",
        _ => "skipped    ",
    };

    private static string Count(int value) => value.ToString(CultureInfo.InvariantCulture);
}
