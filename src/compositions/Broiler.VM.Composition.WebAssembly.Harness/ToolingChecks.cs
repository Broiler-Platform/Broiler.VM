using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The harness's own regression suite, run before any shard starts and before any merge: the sharding,
/// the scope reader, the merge, its classifier of a shard that did not finish, and the failure queue,
/// each held to recorded inputs with a declared answer.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why the tooling is tested as the engine is.</b> Roadmap section 15: "The harness has its own
/// regression suite, run before any shard starts, with the crash classifier tested against recorded output.
/// A measurement tool nobody tests is a measurement nobody can read." A merge that summed a partial set, a
/// queue that let a passing entry stay, or a hash that moved scripts between shards would each report a
/// total that looked right, so each is given an input it must refuse.
/// </para>
/// <para>
/// <b>The crash classifier.</b> A shard that dies leaves a report that stops early. The recorded outputs
/// here are reports cut off before their closing line, and the merge must name the shard as one that did
/// not finish, never sum what it wrote. The shard assignments pinned here were computed apart from this
/// code, from the SHA-256 of each name, so a change to the hash or to the byte order fails them.
/// </para>
/// </remarks>
internal static class ToolingChecks
{
    /// <summary>Runs every check and prints each; answers whether all held.</summary>
    internal static bool Run()
    {
        List<(string Name, bool Passed, string Detail)> checks =
        [
            ShardHash(),
            ScopeRefusesAnAbsentFile(),
            ScopeRefusesADuplicate(),
            MergeSumsACompleteSet(),
            MergeReportsAMissingShard(),
            MergeNamesAnInconsistentField(),
            MergeClassifiesAShardThatDidNotFinish(),
            MergeRefusesAScriptTheShardDoesNotOwn(),
            .. Queue(),
        ];

        foreach (var (name, passed, detail) in checks)
        {
            Console.WriteLine($"# tooling {(passed ? "ok  " : "FAIL")} {name}: {detail}");
        }

        var failed = checks.Count(static check => !check.Passed);
        Console.WriteLine($"# tooling: {Text(checks.Count)} checks, {(failed == 0 ? "every one as declared" : $"{Text(failed)} NOT as declared")}");
        return failed == 0;
    }

    /// <summary>Script names whose shards were computed apart from this code.</summary>
    private static (string, bool, string) ShardHash()
    {
        (string Name, int Of4, int Of7)[] pinned =
        [
            ("address.wast", 0, 2), ("br_table.wast", 2, 2), ("memory.wast", 3, 4), ("i32.wast", 2, 6), ("imports.wast", 2, 4),
        ];

        var wrong = pinned.Where(static pin => SpecSelection.Bucket(pin.Name, 4) != pin.Of4 || SpecSelection.Bucket(pin.Name, 7) != pin.Of7).ToList();

        return ("sharding-assigns-each-script-the-shard-its-recorded-hash-gives", wrong.Count == 0,
            wrong.Count == 0
                ? $"{Text(pinned.Length)} names, each in its recorded shard of four and of seven"
                : string.Join(", ", wrong.Select(static pin => $"{pin.Name} in {Text(SpecSelection.Bucket(pin.Name, 4))} and {Text(SpecSelection.Bucket(pin.Name, 7))}")));
    }

    private static (string, bool, string) ScopeRefusesAnAbsentFile() =>
        Scope("scope-refuses-a-manifest-naming-a-file-the-suite-does-not-contain",
            "manifest broiler.webassembly.slice\nfile a.wast\nfile absent.wast\n", "naming a file the suite does not contain");

    private static (string, bool, string) ScopeRefusesADuplicate() =>
        Scope("scope-refuses-a-manifest-naming-a-file-twice",
            "manifest broiler.webassembly.slice\nfile a.wast\nfile a.wast\n", "twice");

    private static (string, bool, string) Scope(string name, string text, string expected)
    {
        var path = Path.GetTempFileName();

        try
        {
            File.WriteAllText(path, text);
            var read = SpecSelection.TryReadNames(path, "scope manifest", new HashSet<string>(["a.wast", "b.wast"], StringComparer.Ordinal), out _, out _, out var failure);
            // The fixture's temporary name is replaced, so the line is the same on every run.
            failure = failure.Replace(Path.GetFileName(path), "<fixture>", StringComparison.Ordinal);
            return (name, !read && failure.Contains(expected, StringComparison.Ordinal), read ? "read it" : failure);
        }
        finally
        {
            File.Delete(path);
        }
    }

    // =============================================================================================
    // The merge, over recorded reports of a two-shard run
    // =============================================================================================

    /// <summary>A script name in each of two shards, found by the hash itself.</summary>
    private static readonly string[] OneInEach =
    [
        Enumerable.Range(0, 64).Select(static index => $"fixture-{Text(index)}.wast").First(static name => SpecSelection.Bucket(name, 2) == 0),
        Enumerable.Range(0, 64).Select(static index => $"fixture-{Text(index)}.wast").First(static name => SpecSelection.Bucket(name, 2) == 1),
    ];

    private static string Recorded(int shard, string? limits = null, int passed = 5, int failed = 1, bool finished = true)
    {
        var configuration = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["report"] = "1", ["revision"] = "fixture", ["manifest"] = "broiler.webassembly.slice", ["limits"] = limits ?? "Fuel=1",
            ["scope"] = "no scope manifest: every candidate is in scope", ["candidates"] = "2", ["known-incorrect"] = "0",
            ["out-of-scope"] = "0", ["unselectable"] = "0", ["selected"] = "2", ["shards"] = "2",
        };

        var families = new Dictionary<string, SpecSuite.FamilyTotals>(StringComparer.Ordinal)
        {
            ["malformed"] = new(passed + failed, passed + failed, passed, failed, 0),
        };

        var text = SpecReport.Write(configuration, shard, [OneInEach[shard]], families,
            new Dictionary<string, int>(StringComparer.Ordinal) { [OneInEach[shard]] = failed });

        return finished ? text : text[..text.IndexOf("family ", StringComparison.Ordinal)];
    }

    private static (string, bool, string) MergeSumsACompleteSet()
    {
        var failures = new List<string>();
        var (families, failing, scripts, _) = SpecReport.Merge([("0.report", Recorded(0)), ("1.report", Recorded(1, passed: 7, failed: 0))], failures);
        var malformed = families.GetValueOrDefault("malformed");
        var passed = failures.Count == 0 && scripts.Count == 2 && malformed.Passed == 12 && malformed.Failed == 1 && failing.Count(static pair => pair.Value > 0) == 1;

        return ("merge-sums-a-complete-set-and-proves-it-covers-the-selection", passed,
            failures.Count > 0 ? string.Join("; ", failures) : $"{Text(scripts.Count)} scripts, malformed passed {Text(malformed.Passed)} and failed {Text(malformed.Failed)}");
    }

    private static (string, bool, string) MergeReportsAMissingShard() =>
        MergeRefuses("merge-reports-a-missing-shard-as-incomplete-coverage", [("0.report", Recorded(0))], "incomplete coverage: shard 1 of 2 has no report");

    private static (string, bool, string) MergeNamesAnInconsistentField() =>
        MergeRefuses("merge-names-a-field-that-differs-between-shards",
            [("0.report", Recorded(0)), ("1.report", Recorded(1, limits: "Fuel=2"))], "inconsistent shard configuration: limits");

    private static (string, bool, string) MergeClassifiesAShardThatDidNotFinish() =>
        MergeRefuses("merge-classifies-a-report-cut-short-as-a-shard-that-did-not-finish",
            [("0.report", Recorded(0)), ("1.report", Recorded(1, finished: false))], "1.report ends before its closing line: the shard did not finish");

    private static (string, bool, string) MergeRefusesAScriptTheShardDoesNotOwn() =>
        MergeRefuses("merge-refuses-a-script-reported-by-a-shard-that-does-not-own-it",
            [("0.report", Recorded(0)), ("1.report", Recorded(1).Replace("shard 1\n", "shard 0\n", StringComparison.Ordinal))], "is reported by 0.report and 1.report");

    private static (string, bool, string) MergeRefuses(string name, (string, string)[] reports, string expected)
    {
        var failures = new List<string>();
        SpecReport.Merge(reports, failures);
        var passed = failures.Any(failure => failure.Contains(expected, StringComparison.Ordinal));
        return (name, passed, failures.Count == 0 ? "merged it" : string.Join("; ", failures));
    }

    // =============================================================================================
    // The failure queue
    // =============================================================================================

    private static IEnumerable<(string, bool, string)> Queue()
    {
        const string Header = "revision fixture\nmanifest broiler.webassembly.slice\n";
        var selected = new HashSet<string>(["a.wast", "b.wast", "c.wast"], StringComparer.Ordinal);
        var failing = new Dictionary<string, int>(StringComparer.Ordinal) { ["a.wast"] = 2, ["b.wast"] = 0, ["c.wast"] = 1 };

        yield return Judge("queue-confirms-a-listed-script-that-still-fails",
            Header + "path a.wast failing 2\npath c.wast failing 1\n", selected, selected, failing, null);

        yield return Judge("queue-reports-a-listed-script-that-passes-now",
            Header + "path a.wast failing 2\npath b.wast failing 1\npath c.wast failing 1\n", selected, selected, failing, "b.wast, and every command of it passes now");

        yield return Judge("queue-does-not-keep-a-hand-written-entry-the-run-cannot-confirm",
            Header + "path a.wast failing 2\npath c.wast failing 1\npath written-by-hand.wast failing 1\n", selected, selected, failing, "written-by-hand.wast, which this run's selection does not hold");

        yield return Judge("queue-reports-a-failure-it-does-not-list",
            Header + "path a.wast failing 2\n", selected, selected, failing, "c.wast fails 1 commands, and the queue does not list it");

        yield return Judge("queue-leaves-another-shards-script-to-the-merge",
            Header + "path a.wast failing 2\npath c.wast failing 1\n", selected, new HashSet<string>(["a.wast"], StringComparer.Ordinal),
            new Dictionary<string, int>(StringComparer.Ordinal) { ["a.wast"] = 2 }, null);
    }

    private static (string, bool, string) Judge(
        string name, string text, IReadOnlySet<string> selected, IReadOnlySet<string> ran, IReadOnlyDictionary<string, int> failing, string? expected)
    {
        if (!FailureQueue.TryParse(text, out var queue, out var failure))
        {
            return (name, false, failure);
        }

        var violations = FailureQueue.Check(queue, "fixture", selected, ran, failing);
        var passed = expected is null ? violations.Count == 0 : violations.Any(violation => violation.Contains(expected, StringComparison.Ordinal));
        return (name, passed, violations.Count == 0 ? "no disagreement" : string.Join("; ", violations));
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
