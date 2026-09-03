using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>Which metadata dialect a suite's files are written in.</summary>
/// <remarks>
/// <b>A suite is in one dialect and the run is told which, rather than sniffing it per file.</b>
/// The two dialects are distinguishable in practice - one has an <c>expected</c> key the other
/// never writes - but a reader that guessed would silently change how a file is scored when
/// somebody added a key, and the guess would be least reliable on exactly the malformed files
/// where the answer matters most.
/// </remarks>
internal enum SuiteDialect
{
    /// <summary>This component's own fixtures, which declare a verdict in this build's vocabulary.</summary>
    Native,

    /// <summary>An ingested third-party suite, which declares only what must fail and as what.</summary>
    Ingested,
}

/// <summary>
/// Translates a third-party suite's metadata into the cases this harness can honestly score.
/// </summary>
/// <remarks>
/// <para>
/// <b>Most of what this does is decline.</b> <c>broiler.javascript.slice</c> admits no function,
/// which means it cannot load the assertion library every positive test in such a suite is written
/// against - so the honest translation of the overwhelming majority of an ingested suite is "this
/// harness cannot run it", recorded per file with the reason. A translator that produced a case
/// for everything would produce a large total made of cases that could only have one outcome.
/// </para>
/// <para>
/// <b>What survives is the parse-and-early-error slice</b>, which is the thing roadmap section 19's
/// JS-3b gate asks to see scored. A negative test whose declared phase is <c>parse</c> never
/// executes, so the assertion library it would have needed is never reached, and the question it
/// asks - is this source a syntax error - is one this front end genuinely answers. That is not a
/// convenient coincidence: it is why the milestone's gate names that slice and not another.
/// </para>
/// <para>
/// <b>Nothing in this file is suite content.</b> It handles a metadata dialect and a set of flag
/// names, which are a format; it embeds no test, no path, no expectation and no revision, fetches
/// nothing, and this repository holds no suite file. Retrieving, hashing and archiving the suite
/// remains the human action the evidence ledger's section 3 records as open.
/// </para>
/// </remarks>
internal static class Test262Adapter
{
    /// <summary>What a strict reading of a file is called.</summary>
    internal const string StrictSuffix = "#strict";

    /// <summary>What a sloppy reading of a file is called.</summary>
    internal const string SloppySuffix = "#sloppy";

    /// <summary>The prologue that makes a script strict, which is how the suite specifies it.</summary>
    /// <remarks>
    /// <b>This is the one place the harness alters what it runs, and it is the suite's own rule
    /// rather than this harness's idea.</b> A file that declares neither strictness is defined to
    /// be run twice, once with this prepended - so the alteration is part of what the file means,
    /// not a convenience. The sloppy reading is still the bytes on disk, the strict one is named
    /// <see cref="StrictSuffix"/> so no report can confuse the two, and there is no third form.
    /// </remarks>
    internal const string StrictPrologue = "\"use strict\";\n";

    /// <summary>The flags that need an agent, a shared buffer, or a clock this profile has none of.</summary>
    private static readonly string[] AgentFlags =
        ["CanBlockIsFalse", "CanBlockIsTrue", "non-deterministic"];

    /// <summary>
    /// Turns one suite file into the cases it stands for - none, one, or the two strictness
    /// readings.
    /// </summary>
    /// <remarks>
    /// <b>A file this harness cannot run comes back as a case that is counted and named, never as
    /// nothing.</b> The selection pipeline's candidate count is what the merge proves its coverage
    /// against, so a translator that returned an empty list for the files it declined would shrink
    /// every total without anything saying why.
    /// </remarks>
    internal static bool TryTranslate(
        string path,
        string text,
        out IReadOnlyList<ConformanceTest> tests,
        out string failure)
    {
        tests = [];
        failure = string.Empty;

        // A .js FILE WITH NO METADATA BLOCK IS A HARNESS FILE, NOT A BROKEN TEST. An ingested
        // suite ships its assertion library and its module fixtures beside its tests, as source
        // files with no block. This is where the two dialects are treated differently on purpose:
        // a fixture WE wrote with no block is a defect and is refused, because we own it; a file we
        // were handed is declined and counted, because we do not. Refusing it would make a run
        // against a real checkout fail whole on the presence of exactly the files that must be
        // there.
        if (!text.Contains("/*---", StringComparison.Ordinal))
        {
            tests =
            [
                Declined(
                    path,
                    "a source file carrying no metadata block, which is a harness or fixture file " +
                    "rather than a test"),
            ];

            return true;
        }

        if (!Test262Metadata.TryRead(path, text, out var front, out failure))
        {
            return false;
        }

        var flags = front.Flags;
        var raw = flags.Contains("raw", StringComparer.Ordinal);
        var module = flags.Contains("module", StringComparer.Ordinal);

        var reason = WhyItCannotRun(front, raw);

        if (reason.Length != 0)
        {
            tests = [Declined(path, reason, front, module ? HostMode.Module : HostMode.Script)];
            return true;
        }

        var expectation = Expectation(front);

        // THE `raw` FLAG MEANS TWO DIFFERENT THINGS IN THE TWO DIALECTS AND THIS IS WHERE THAT IS
        // HANDLED RATHER THAN INHERITED. In the ingested suite it means "prepend no harness file
        // and take no strictness variant" - the file is still SOURCE. In this harness it means the
        // test carries ARTIFACT BYTES that no front end lowers. Carrying the flag straight across
        // would route source into the raw host mode, where it would be handed to the verifier as
        // if it were bytecode, and every such test would fail for a reason nobody wrote down.
        var mode = module ? HostMode.Module : HostMode.Script;

        // Module code is strict by its goal symbol and a raw test declares that it takes no
        // variant, so both are one case under the name they were given. Everything else is read
        // once for each strictness the file admits.
        if (raw || module)
        {
            tests = [Case(path, front, mode, expectation, text)];
            return true;
        }

        var cases = new List<ConformanceTest>();

        if (!flags.Contains("noStrict", StringComparer.Ordinal))
        {
            cases.Add(Case(
                path + StrictSuffix, front, mode, expectation, StrictPrologue + text));
        }

        if (!flags.Contains("onlyStrict", StringComparer.Ordinal))
        {
            cases.Add(Case(path + SloppySuffix, front, mode, expectation, text));
        }

        tests = cases;
        return true;
    }

    /// <summary>Why this harness cannot run a file, or empty where it can.</summary>
    /// <remarks>
    /// The order is by specificity, and the first match is the reason reported. A file can be
    /// undeliverable for several reasons at once - an async test with includes and an agent flag is
    /// three - and reporting the most specific one is what makes a triage of the declined total
    /// readable.
    /// </remarks>
    private static string WhyItCannotRun(Test262Frontmatter front, bool raw)
    {
        foreach (var flag in AgentFlags)
        {
            if (front.Flags.Contains(flag, StringComparer.Ordinal))
            {
                return $"declares `{flag}`, which needs an agent and a shared memory this profile has none of";
            }
        }

        if (front.Flags.Contains("async", StringComparer.Ordinal))
        {
            return "signals completion by calling a harness function, and this manifest admits no call";
        }

        if (front.Negative is { } negative)
        {
            switch (negative.Phase)
            {
                case "resolution":
                    return "fails at module resolution, which needs a linker this manifest admits no import to reach";

                case "parse":
                    // A PARSE-PHASE NEGATIVE IS SELECTABLE EVEN WITH INCLUDES DECLARED, and that is
                    // the whole reason this slice is scorable at all. The file must fail before it
                    // runs, so no harness file it names is ever reached, so needing one costs
                    // nothing. This is the arm that lets a manifest with no functions score a
                    // suite written entirely against an assertion library.
                    return string.Equals(negative.Type, LanguageErrors.SyntaxError, StringComparison.Ordinal)
                        ? string.Empty
                        : $"declares a parse-phase `{negative.Type}`, and this front end reports every " +
                            "early error as a SyntaxError";

                default:
                    if (front.Includes.Count != 0)
                    {
                        return "needs harness files this manifest cannot provide: " +
                            string.Join(", ", front.Includes);
                    }

                    return Enum.TryParse<JavaScriptErrorKind>(negative.Type, out var kind) &&
                        Enum.IsDefined(kind)
                            ? string.Empty
                            : $"declares a runtime `{negative.Type}`, which is not one of this " +
                                "profile's fault kinds";
            }
        }

        if (front.Includes.Count != 0)
        {
            return "needs harness files this manifest cannot provide: " + string.Join(", ", front.Includes);
        }

        // THE IMPLICIT PRELUDE IS THE REASON ALMOST EVERY POSITIVE TEST LANDS HERE, and it is
        // implicit rather than declared. The suite prepends its assertion library to every file
        // that does not carry the raw flag, whatever its `includes` line says, so a positive test
        // without that flag needs a function call before its first statement runs.
        return raw
            ? string.Empty
            : "needs the suite's implicit assertion prelude, which this manifest admits no call to load";
    }

    /// <summary>What the file declares must happen, in this harness's vocabulary.</summary>
    /// <remarks>
    /// A file with no negative expectation declares only that nothing was thrown, whatever else it
    /// asserts through the library this harness cannot load. That is weaker than the value a
    /// native fixture declares, and it is everything that survives the translation.
    /// </remarks>
    private static ConformanceExpectation Expectation(Test262Frontmatter front)
    {
        if (front.Negative is { } negative)
        {
            return negative.Phase == "parse"
                ? new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, negative.Type)
                : new ConformanceExpectation(ExpectationKind.Fault, negative.Type);
        }

        return new ConformanceExpectation(ExpectationKind.CompletesWithoutFault, string.Empty);
    }

    /// <summary>One runnable case.</summary>
    private static ConformanceTest Case(
        string path,
        Test262Frontmatter front,
        HostMode mode,
        ConformanceExpectation expectation,
        string source) =>
        new(
            Suite.Normalize(path),
            front.Description,
            mode,
            expectation,
            source,
            null,
            front.Features,
            string.Empty,
            string.Empty,
            Ingested: true);

    /// <summary>One case that is counted as a candidate and never selected.</summary>
    /// <remarks>
    /// It carries the goal the file declared rather than a default. A declined case is in no mode
    /// total - the selection drops it before sharding - but it is in the report, and a row saying
    /// Script for a file whose metadata says module is a row that misleads a reader triaging what
    /// the harness could not run.
    /// </remarks>
    private static ConformanceTest Declined(
        string path,
        string reason,
        Test262Frontmatter? front = null,
        HostMode mode = HostMode.Script) =>
        new(
            Suite.Normalize(path),
            front?.Description is { Length: > 0 } description ? description : "(no description)",
            mode,
            new ConformanceExpectation(ExpectationKind.CompletesWithoutFault, string.Empty),
            string.Empty,
            null,
            front?.Features ?? [],
            reason,
            string.Empty,
            Ingested: true);
}
