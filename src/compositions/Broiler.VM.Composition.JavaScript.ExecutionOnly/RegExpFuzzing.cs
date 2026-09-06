// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The fourth untrusted-input surface: the regular-expression matcher, over pattern and subject.
/// </summary>
/// <remarks>
/// <para>
/// <b>Roadmap section 7 names four surfaces and JS-9 asks for all four to be fuzzed.</b> The
/// verifier and the executor are the sibling session's; the source tokenizer and parser are the
/// slice compiler's; this is the one that was reached by nothing. It was excused as ABSENT until
/// the workload programme replaced the translation onto the platform's engine with a matcher of
/// this component's own — and after that the excuse was wrong and the gap was real.
/// </para>
/// <para>
/// <b>Both halves of the input are guest-controlled and both are mutated.</b> A pattern is
/// compiled from text a program wrote and a subject is text a program passed, so a session that
/// mutated only the pattern would leave half the surface unexplored — and the half it left is the
/// one where a backtracker spends its time.
/// </para>
/// <para>
/// <b>The declared refusals are answers and everything else is a defect.</b>
/// <see cref="JsRegExpSyntaxError"/> is the matcher saying a pattern is not one, and
/// <see cref="JsRegExpOverflowError"/> is it reaching a ceiling it declares for itself. Any other
/// exception escaping is a counterexample, because the caller that turns these into guest-visible
/// answers knows about exactly two.
/// </para>
/// <para>
/// <b>The guidance is by published answer, as it is everywhere else in this component</b>, and
/// <see cref="Broiler.VM.Profile.JavaScript"/>'s JSD-0013 records the refusal to instrument for
/// anything finer. A mutant whose answer no seed produced is kept as a further seed; two paths to
/// one answer are one signal.
/// </para>
/// </remarks>
internal static class RegExpFuzzing
{
    /// <summary>How much matching work one iteration may spend before it is abandoned.</summary>
    /// <remarks>
    /// A pattern that backtracks catastrophically is a pattern this matcher charges for, and the
    /// charge is what a real caller's allowance stops. A session has no allowance of its own, so it
    /// states one here: an iteration that spends more than this is not a finding, it is a pattern
    /// doing what the ceiling exists for, and the session moves on.
    /// </remarks>
    private const ulong WorkCeiling = 2_000_000;

    /// <summary>How many mutants the seed pool may hold.</summary>
    private const int PoolCeiling = 512;

    /// <summary>The patterns a session opens with. Every one of them is this repository's own.</summary>
    private static readonly string[] SeedPatterns =
    [
        "a", "a+", "a*b", "(a|b)+", "^abc$", "a{2,4}", "[a-z]+", "[^a-z]", "\\d+", "\\w\\s\\W",
        "(?:ab)+", "(?=a)b", "(?!a)b", "(?<=a)b", "(?<!a)b", "(a)(b)(c)", "\\1", "(a)\\1",
        "(?<name>a)\\k<name>", "a|", "[]", "[^]", ".", "\\.", "\\u0041", "\\x41", "\\cA",
        "[\\b]", "\\B", "\\b", "a??", "a+?", "a*?", "(a+)+$", "((a)|(b))*", "[a-", "(", ")",
        "*", "+", "?", "{", "}", "\\", "(?", "(?<", "\\u{41}", "[\\u0041-\\u005A]", "\\p{L}",
    ];

    /// <summary>The subjects a session opens with.</summary>
    private static readonly string[] SeedSubjects =
    [
        "", "a", "ab", "abc", "aaaa", "aaaaaaaaaaaaaaaaaaaaaaaaX", "AbC", "\n", "a\nb",
        "é", "😀", "\ud800", "\udc00", "0123456789", "  spaced  ", "z",
    ];

    /// <summary>Runs one session and reports what it did.</summary>
    internal static int Run(ulong seed, int iterations)
    {
        Console.WriteLine(
            $"broiler-js-regexp-fuzz: seed {seed}, {iterations} iterations, " +
            $"{SeedPatterns.Length} seed patterns and {SeedSubjects.Length} seed subjects, " +
            "surface: the regular-expression matcher over pattern and subject");

        var answers = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var patterns = new List<string>(SeedPatterns);
        var subjects = new List<string>(SeedSubjects);
        var baseline = new HashSet<string>(StringComparer.Ordinal);
        var offered = 0;
        var kept = 0;
        var state = seed == 0 ? 1UL : seed;

        // THE BASELINE IS WHAT THE SEEDS ALONE REACH, taken before a single mutant runs. A session
        // that reported growth against a pool it had already grown would be reporting its own
        // warm-up.
        foreach (var pattern in SeedPatterns)
        {
            foreach (var subject in SeedSubjects)
            {
                baseline.Add(Answer(pattern, subject, out _));
            }
        }

        // TAKEN BEFORE THE LOOP, because it stops being derivable once the pool fills: a mutant
        // whose answer is new still grows this set after the pool has stopped accepting seeds, so
        // subtracting what was kept from the final count would report a number that drifts by
        // exactly the mutants the ceiling refused.
        var reachedBySeeds = baseline.Count;
        var saturated = false;
        string? counterexample = null;

        for (var index = 0; index < iterations && counterexample is null; index++)
        {
            var pattern = Mutate(patterns[(int)(Next(ref state) % (ulong)patterns.Count)], ref state);
            var subject = Mutate(subjects[(int)(Next(ref state) % (ulong)subjects.Count)], ref state);
            var answer = Answer(pattern, subject, out var escaped);

            answers[answer] = answers.TryGetValue(answer, out var seen) ? seen + 1 : 1;
            offered++;

            if (escaped is not null)
            {
                counterexample =
                    $"pattern {Render(pattern)} over subject {Render(subject)} escaped " +
                    $"{escaped.GetType().FullName}: {escaped.Message}";

                continue;
            }

            if (!baseline.Add(answer))
            {
                continue;
            }

            if (patterns.Count >= PoolCeiling)
            {
                saturated = true;
                continue;
            }

            kept++;
            patterns.Add(pattern);
            subjects.Add(subject);
        }

        foreach (var (answer, count) in answers)
        {
            Console.WriteLine($"  {count,8}  {answer}");
        }

        Console.WriteLine(
            $"broiler-js-regexp-fuzz: guidance - {reachedBySeeds} answers reached by the seeds " +
            $"alone, {baseline.Count} by the end of the session, {kept} mutants kept as further " +
            $"seeds, {offered} offered" +
            (saturated
                ? $", and the pool reached its ceiling of {PoolCeiling}, after which a mutant with " +
                  "a new answer grew the answer set and was not kept"
                : string.Empty) +
            ". Guidance is by PUBLISHED ANSWER and is not edge coverage.");

        if (counterexample is not null)
        {
            Console.WriteLine("broiler-js-regexp-fuzz: COUNTEREXAMPLE - " + counterexample);
            return 1;
        }

        // A SESSION THAT REACHED NEITHER A MATCH NOR A REFUSAL REACHED NOTHING. It is the same
        // integrity clause the sibling sessions carry: a clean run over a surface nobody touched is
        // worth nothing, and saying so is cheaper than discovering it later.
        var matched = answers.Keys.Any(static key => key.StartsWith("match:", StringComparison.Ordinal));
        var refused = answers.Keys.Any(static key => key.StartsWith("refused:", StringComparison.Ordinal));

        if (!matched || !refused)
        {
            Console.WriteLine(
                "broiler-js-regexp-fuzz: the session did not reach both a match and a refusal, so " +
                "it may not be read as covering the surface");

            return 5;
        }

        Console.WriteLine($"broiler-js-regexp-fuzz: no counterexample in {iterations} iterations.");
        return 0;
    }

    /// <summary>What the matcher published for this pair, and what escaped if anything did.</summary>
    private static string Answer(string pattern, string subject, out Exception? escaped)
    {
        escaped = null;
        ulong spent = 0;

        try
        {
            var matcher = JsRegExpMatcher.Compile(
                pattern, ignoreCase: false, multiline: false, dotAll: false, unicode: false);

            var match = matcher.Match(
                subject,
                0,
                anchored: false,
                units =>
                {
                    spent += units;

                    if (spent > WorkCeiling)
                    {
                        throw new JsRegExpOverflowError("the session's own work ceiling");
                    }
                });

            if (match is null)
            {
                return "no-match";
            }

            var participating = 0;

            for (var group = 1; group <= match.CaptureCount; group++)
            {
                participating += match.Participated(group) ? 1 : 0;
            }

            return $"match:{match.Length}:{match.CaptureCount}:{participating}";
        }
        catch (JsRegExpSyntaxError failure)
        {
            return "refused:" + First(failure.Message);
        }
        catch (JsRegExpOverflowError)
        {
            return "overflow";
        }
        catch (Exception failure)
        {
            escaped = failure;
            return "escaped:" + failure.GetType().Name;
        }
    }

    /// <summary>One edit to a string: an insertion, a deletion, a replacement or a doubling.</summary>
    private static string Mutate(string text, ref ulong state)
    {
        var alphabet = "ab()[]{}|*+?^$.\\-,0123456789<>=!:é😀";
        var choice = Next(ref state) % 5;

        if (text.Length == 0 || choice == 0)
        {
            var at = text.Length == 0 ? 0 : (int)(Next(ref state) % (ulong)(text.Length + 1));
            var character = alphabet[(int)(Next(ref state) % (ulong)alphabet.Length)];
            return text[..at] + character + text[at..];
        }

        if (choice == 1)
        {
            var at = (int)(Next(ref state) % (ulong)text.Length);
            return text.Remove(at, 1);
        }

        if (choice == 2)
        {
            var at = (int)(Next(ref state) % (ulong)text.Length);
            var character = alphabet[(int)(Next(ref state) % (ulong)alphabet.Length)];
            return text[..at] + character + text[(at + 1)..];
        }

        if (choice == 3 && text.Length < 64)
        {
            return text + text;
        }

        var take = (int)(Next(ref state) % (ulong)text.Length) + 1;
        return text[..take];
    }

    /// <summary>A session is a total function of its seed, so the generator is this one.</summary>
    private static ulong Next(ref ulong state)
    {
        state ^= state << 13;
        state ^= state >> 7;
        state ^= state << 17;
        return state;
    }

    /// <summary>The first word of a refusal, which is the class rather than the sentence.</summary>
    private static string First(string message)
    {
        var space = message.IndexOf(' ', StringComparison.Ordinal);
        return space < 0 ? message : message[..space];
    }

    /// <summary>A counterexample printed so it can be pasted back in.</summary>
    private static string Render(string text)
    {
        var rendered = new System.Text.StringBuilder("\"");

        foreach (var character in text)
        {
            rendered.Append(
                character is >= ' ' and <= '~'
                    ? character.ToString()
                    : "\\u" + ((int)character).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
        }

        return rendered.Append('"').ToString();
    }
}
