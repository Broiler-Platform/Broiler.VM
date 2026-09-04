// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The <c>--test262</c> mode: run named tests of a real test262 checkout and report per test.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a separate mode from <c>--run</c> and reads a different tree.</b> <c>--run</c> scores
/// this component's own fixture tree, which is the only scoring target that existed before there
/// was an engine to score. This one takes the path of an unpacked third-party suite the repository
/// does not hold, runs what it is pointed at, and reports four verdicts rather than two - because
/// "this manifest does not admit that construct" is not a failure and must never be counted as a
/// pass.
/// </para>
/// <para>
/// <b>It scores nothing on its own.</b> There is no total here that a ledger may cite as a
/// conformance figure: a run over a list somebody chose is a measurement of that list. What it is
/// for is the thing a bring-up needs and a ratchet cannot give - pointing the engine at one test
/// and reading what happened.
/// </para>
/// </remarks>
internal static class Test262Command
{
    /// <summary>The instruction allowance one test variant gets unless a caller states one.</summary>
    private const ulong DefaultFuel = 2_000_000_000;

    /// <summary>The wall-clock allowance one test variant gets, in milliseconds.</summary>
    private const ulong DefaultWallClock = 60_000;

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

        var wanted = Collect(suiteRoot, args);

        if (wanted.Count == 0)
        {
            Console.WriteLine(
                "broiler-js-conformance: --test262 <root> needs --test <path> (repeatable) or " +
                "--dir <path>, and takes --limit <n>, --fuel <n> and --wall <ms>.");

            return ExitCodes.Usage;
        }

        var fuel = Number(args, "--fuel", DefaultFuel);
        var wallClock = Number(args, "--wall", DefaultWallClock);

        var passed = 0;
        var failed = 0;
        var unsupported = 0;
        var skipped = 0;

        foreach (var relative in wanted)
        {
            foreach (var outcome in Test262Run.RunOne(suiteRoot, relative, fuel, wallClock))
            {
                switch (outcome.Verdict)
                {
                    case Test262Verdict.Passed:
                        passed++;
                        break;

                    case Test262Verdict.Failed:
                        failed++;
                        break;

                    case Test262Verdict.Unsupported:
                        unsupported++;
                        break;

                    default:
                        skipped++;
                        break;
                }

                if (verbose || outcome.Verdict != Test262Verdict.Passed)
                {
                    Console.WriteLine(
                        Mark(outcome.Verdict) + " " + outcome.Path + " [" + outcome.Variant + "]" +
                        (outcome.Detail.Length == 0 ? string.Empty : "  " + outcome.Detail));
                }
                else
                {
                    Console.WriteLine(Mark(outcome.Verdict) + " " + outcome.Path + " [" + outcome.Variant + "]");
                }
            }
        }

        Console.WriteLine(
            "# test262 " + Count(wanted.Count) + " files: pass " + Count(passed) +
            ", fail " + Count(failed) + ", unsupported " + Count(unsupported) +
            ", skipped " + Count(skipped));

        return failed == 0 ? ExitCodes.Ok : ExitCodes.Failed;
    }

    private static List<string> Collect(string suiteRoot, string[] args)
    {
        var wanted = new List<string>();

        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], "--test", StringComparison.Ordinal))
            {
                wanted.Add(args[index + 1].Replace('\\', '/'));
            }
        }

        for (var index = 0; index < args.Length - 1; index++)
        {
            if (!string.Equals(args[index], "--dir", StringComparison.Ordinal))
            {
                continue;
            }

            var relative = args[index + 1].Replace('\\', '/');
            var directory = Path.Combine(suiteRoot, relative.Replace('/', Path.DirectorySeparatorChar));

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("broiler-js-conformance: no directory at " + directory);
                continue;
            }

            var found = Directory.GetFiles(directory, "*.js", SearchOption.AllDirectories);
            Array.Sort(found, StringComparer.Ordinal);

            foreach (var file in found)
            {
                wanted.Add(
                    Path.GetRelativePath(suiteRoot, file).Replace('\\', '/'));
            }
        }

        var limit = (int)Number(args, "--limit", (ulong)int.MaxValue);

        if (wanted.Count > limit)
        {
            // A CAP IS ANNOUNCED, never silent. A run that quietly stopped at a hundred files and
            // printed a total would read as a run over everything it was pointed at.
            Console.WriteLine(
                "# --limit " + Count(limit) + " drops " + Count(wanted.Count - limit) +
                " of the " + Count(wanted.Count) + " files named");

            wanted.RemoveRange(limit, wanted.Count - limit);
        }

        return wanted;
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
        _ => "skipped    ",
    };

    private static string Count(int value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
