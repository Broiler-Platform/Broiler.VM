using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>The four verdicts a conformance test may declare for itself.</summary>
/// <remarks>
/// They are four because this profile can answer in four places - the front end, the verifier, the
/// executor's fault channel, and a completion value - and a test that could only declare "passes"
/// would be satisfied by an engine that refused it for the wrong reason.
/// </remarks>
internal enum ExpectationKind
{
    /// <summary>The program compiles, verifies, runs, and its completion value renders as the declared text.</summary>
    Completion,

    /// <summary>The front end refuses the source, with the declared embedder-seam diagnostic.</summary>
    RefusedBySource,

    /// <summary>The artifact is refused at verification, with the declared core-result diagnostic.</summary>
    RefusedByVerifier,

    /// <summary>Execution faults, with the declared JavaScript error kind.</summary>
    Fault,

    /// <summary>
    /// The front end refuses the source <b>as the language would</b>, with the declared JavaScript
    /// error type name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is not <see cref="RefusedBySource"/> with a different spelling, and fusing them
    /// would be the scoring bug this harness exists to refuse.</b> That kind names one of THIS
    /// build's diagnostic codes and asks "did this front end refuse for the reason I wrote down" -
    /// a question about the implementation. This one names a JavaScript error type and asks "is
    /// this source a syntax error in the language" - a question about the language, which most of
    /// this build's refusals do not answer, because
    /// <c>broiler.javascript.slice</c> refuses valid JavaScript on almost every line.
    /// </para>
    /// <para>
    /// Roadmap section 14 asks for exactly this unit: a negative test's uncaught error reported
    /// "by its JavaScript type name so a parse-phase syntax error is matched on what it is".
    /// <see cref="LanguageErrors"/> holds which refusals qualify.
    /// </para>
    /// </remarks>
    RefusedAsEarlyError,

    /// <summary>The program compiles, verifies, runs, and does not fault. Its value is not read.</summary>
    /// <remarks>
    /// <b>An ingested suite's positive tests can declare nothing stronger.</b> They assert through
    /// a harness library - a call this manifest has no way to make - so what survives translation
    /// is the part that needs no library: that nothing was thrown. It is deliberately weaker than
    /// <see cref="Completion"/> and is never used by this component's own fixtures, which can
    /// declare the value and therefore must.
    /// </remarks>
    CompletesWithoutFault,
}

/// <summary>What a test says must happen to it.</summary>
/// <param name="Kind">Which of this profile's four answering places is expected to answer.</param>
/// <param name="Value">
/// The answer, as text: a rendered completion value, or the NAME of a diagnostic code or error
/// kind.
/// </param>
/// <remarks>
/// <b>The refusal is declared by name and never by number.</b> Roadmap section 14 asks a negative
/// test's uncaught error to be reported by its JavaScript type name "so a parse-phase syntax error
/// is matched on what it is"; this profile's equivalent of that type name is the diagnostic code's
/// member name, which is what the published registry keys on. A test declaring `1401` would be a
/// test nobody could read and one that a renumbering would silently invalidate.
/// </remarks>
internal sealed record ConformanceExpectation(ExpectationKind Kind, string Value)
{
    /// <summary>
    /// Whether this is a negative-metadata test: one whose declared verdict is a refusal.
    /// </summary>
    /// <remarks>
    /// Negative tests are opt-in, per roadmap section 14, because a harness that ran them by
    /// default would be scoring the front end's refusals inside a total about the executor. The
    /// flag is read from the expectation rather than from a separate metadata field: a test that
    /// declares a refusal IS the negative case, and two spellings of one fact could disagree.
    /// </remarks>
    internal bool IsNegative =>
        Kind is ExpectationKind.RefusedBySource
            or ExpectationKind.RefusedByVerifier
            or ExpectationKind.RefusedAsEarlyError;

    /// <summary>The declaration, in the spelling a test file writes it in.</summary>
    public override string ToString() => Kind switch
    {
        ExpectationKind.Completion => "completion " + Value,
        ExpectationKind.RefusedBySource => "refused-by-source " + Value,
        ExpectationKind.RefusedByVerifier => "refused-by-verifier " + Value,
        ExpectationKind.RefusedAsEarlyError => "refused-as-early-error " + Value,
        ExpectationKind.CompletesWithoutFault => "completes",
        _ => "fault " + Value,
    };

    /// <summary>Reads an expectation, or says why the text is not one.</summary>
    internal static bool TryParse(string text, out ConformanceExpectation expectation, out string failure)
    {
        expectation = new ConformanceExpectation(ExpectationKind.Completion, string.Empty);

        // `completes` is the one declaration that carries no value, because the thing it asserts is
        // that nothing was thrown and there is nothing further to say. It is read before the
        // `<kind> <value>` split rather than inside it, so that the split can keep requiring both
        // halves for every kind that does have a value.
        if (string.Equals(text.Trim(), "completes", StringComparison.Ordinal))
        {
            expectation = new ConformanceExpectation(ExpectationKind.CompletesWithoutFault, string.Empty);
            failure = string.Empty;
            return true;
        }

        var space = text.IndexOf(' ', StringComparison.Ordinal);

        if (space <= 0 || space == text.Length - 1)
        {
            failure = $"`{text}` is not `<kind> <value>`";
            return false;
        }

        var kind = text[..space];
        var value = text[(space + 1)..].Trim();

        var parsed = kind switch
        {
            "completion" => ExpectationKind.Completion,
            "refused-by-source" => ExpectationKind.RefusedBySource,
            "refused-by-verifier" => ExpectationKind.RefusedByVerifier,
            "refused-as-early-error" => ExpectationKind.RefusedAsEarlyError,
            "fault" => ExpectationKind.Fault,
            _ => (ExpectationKind?)null,
        };

        if (parsed is null)
        {
            failure =
                $"`{kind}` is not one of completion, completes, refused-by-source, " +
                "refused-by-verifier, refused-as-early-error, fault";

            return false;
        }

        expectation = new ConformanceExpectation(parsed.Value, value);
        failure = string.Empty;
        return true;
    }
}

/// <summary>One conformance test: what to run, how to run it, and what must happen.</summary>
/// <param name="Path">The suite-relative path, normalized. This is the test's identity everywhere.</param>
/// <param name="Description">What the test is about, from its own metadata.</param>
/// <param name="Mode">Which of the three ways this profile takes a program the test is presented through.</param>
/// <param name="Expectation">The verdict the test declares for itself.</param>
/// <param name="Source">The source text, whole and including its metadata comment. Empty for a raw test.</param>
/// <param name="Bytes">The artifact bytes of a raw test, and null for a source one.</param>
/// <param name="Features">The feature names the test's metadata claims, for the feature filter.</param>
/// <param name="Unselectable">Why the test cannot be selected, or empty where it can.</param>
/// <param name="RequiredHost">
/// A host this harness does not build, or empty where the default host runs the test.
/// </param>
/// <param name="Ingested">
/// Whether this test asks a question about <b>the language</b> rather than about this front end.
/// </param>
/// <remarks>
/// <para>
/// <b><paramref name="Unselectable"/> and <paramref name="RequiredHost"/> are two different
/// facts and are deliberately two fields.</b> An unselectable test never enters the selection and
/// is not in any total; a test needing a host is selected, counted, and reported skipped with the
/// host named. Fusing them would let a selection shrink for a reason that reads as an execution
/// gap, or the reverse.
/// </para>
/// <para>
/// <b><paramref name="Ingested"/> decides whether a refusal has to have been earned.</b> This
/// component's own fixtures are written against this front end and declare its diagnostic codes by
/// name, so a refusal is the answer they asked for and is scored as one. A test translated out of
/// a third-party suite asked whether some source is valid <i>JavaScript</i>, and this profile
/// refuses valid JavaScript constantly - so its refusal answers that question only when
/// <see cref="LanguageErrors"/> says the refusal was a language answer, and the case is otherwise
/// reported unscorable rather than passed. Without the flag the rule would have to be inferred
/// from the expectation kind at each site, which is how one of the two suites eventually acquires
/// the other one's rule.
/// </para>
/// </remarks>
internal sealed record ConformanceTest(
    string Path,
    string Description,
    HostMode Mode,
    ConformanceExpectation Expectation,
    string Source,
    byte[]? Bytes,
    IReadOnlyList<string> Features,
    string Unselectable,
    string RequiredHost = "",
    bool Ingested = false);

/// <summary>
/// The metadata block a source test carries, and the reader for it.
/// </summary>
/// <remarks>
/// <para>
/// <b>The shape is deliberately the one the conformance suite uses</b> - a leading
/// <c>/*--- … ---*/</c> comment carrying <c>description</c>, <c>flags</c>, <c>features</c> and
/// <c>includes</c> - so that the day a pinned suite is retrieved, this reader is pointed at it
/// rather than replaced. Copying a FORMAT is not copying a suite: nothing here reads a suite file,
/// and no suite file is in this repository.
/// </para>
/// <para>
/// <b>The block is a comment, so the file compiles as written.</b> That is the property the shape
/// exists for: the harness hands the lowering the whole file, metadata included, and never a
/// stripped copy - so what is scored is the bytes on disk rather than something the harness built.
/// </para>
/// <para>
/// <b>One key is this component's own and is not the suite's: `expected`.</b> The suite declares a
/// negative expectation and leaves a positive one implicit in the assertions a test makes, which
/// needs a harness library this manifest cannot express - it has no functions to call. Declaring
/// the verdict in metadata is what lets a test whose whole body is <c>1 + 2</c> be scored at all.
/// </para>
/// </remarks>
internal static class TestMetadata
{
    /// <summary>Where the metadata block opens.</summary>
    private const string Open = "/*---";

    /// <summary>Where it closes.</summary>
    private const string Close = "---*/";

    /// <summary>
    /// Reads a test out of one file's text, or says why the file is not a test.
    /// </summary>
    /// <remarks>
    /// A file with no metadata block is <b>refused rather than defaulted</b>. A reader that
    /// silently treated a missing block as "script, expect it to pass" would turn every
    /// mis-authored fixture into a case that could only pass, which is the shape of failure this
    /// whole harness exists to remove.
    /// </remarks>
    internal static bool TryRead(
        string path,
        string text,
        byte[]? bytes,
        out ConformanceTest test,
        out string failure)
    {
        test = null!;
        var open = text.IndexOf(Open, StringComparison.Ordinal);
        var close = text.IndexOf(Close, StringComparison.Ordinal);

        if (open < 0 || close < open)
        {
            failure = $"{path} carries no {Open} … {Close} metadata block";
            return false;
        }

        var description = string.Empty;
        var expectation = string.Empty;
        var flags = new List<string>();
        var features = new List<string>();
        var includes = new List<string>();

        foreach (var raw in text[(open + Open.Length)..close]
                     .Replace("\r\n", "\n", StringComparison.Ordinal)
                     .Split('\n'))
        {
            var line = raw.Trim();
            var colon = line.IndexOf(':', StringComparison.Ordinal);

            if (line.Length == 0 || colon <= 0)
            {
                continue;
            }

            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();

            switch (key)
            {
                case "description":
                    description = value;
                    break;
                case "expected":
                    expectation = value;
                    break;
                case "flags":
                    flags.AddRange(List(value));
                    break;
                case "features":
                    features.AddRange(List(value));
                    break;
                case "includes":
                    includes.AddRange(List(value));
                    break;
                default:
                    break;
            }
        }

        if (description.Length == 0)
        {
            failure = $"{path} declares no description";
            return false;
        }

        if (!ConformanceExpectation.TryParse(expectation, out var expected, out var why))
        {
            failure = $"{path} declares no readable expectation: {why}";
            return false;
        }

        var mode = HostMode.Script;

        if (flags.Contains("module", StringComparer.Ordinal))
        {
            mode = HostMode.Module;
        }

        // THE RAW FLAG IS CHECKED IN BOTH DIRECTIONS, because each way round is a different
        // mistake. On a source file it is a contradiction - raw means bytes with no lowering, and a
        // .js file has none - and a fixture that carried it would land in a host mode whose totals
        // it cannot contribute to. On an artifact its ABSENCE is the mistake: a sidecar that forgot
        // it would put artifact bytes into the script mode's totals, where the lowering would be
        // handed a file it cannot read and every such test would fail for a reason nobody wrote.
        var declaresRaw = flags.Contains("raw", StringComparer.Ordinal);

        if (declaresRaw && bytes is null)
        {
            failure = $"{path} declares the raw flag, which names artifact bytes and not source";
            return false;
        }

        if (!declaresRaw && bytes is not null)
        {
            failure = $"{path} carries artifact bytes and does not declare the raw flag";
            return false;
        }

        if (declaresRaw)
        {
            mode = HostMode.Raw;
        }

        // What the suite calls an include is a harness file this profile has no way to provide:
        // there are no functions, so there is nothing for `assert.js` to define. The test is a
        // test - it is read, counted as a candidate, and reported unselectable by name.
        var unselectable = includes.Count == 0
            ? string.Empty
            : "needs harness files this manifest cannot provide: " + string.Join(", ", includes);

        test = new ConformanceTest(
            Suite.Normalize(path),
            description,
            mode,
            expected,
            bytes is null ? text : string.Empty,
            bytes,
            features,
            unselectable);

        failure = string.Empty;
        return true;
    }

    /// <summary>Reads a metadata list, in either the bracketed or the bare spelling.</summary>
    private static IEnumerable<string> List(string value)
    {
        var trimmed = value.Trim();

        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            trimmed = trimmed[1..^1];
        }

        return trimmed
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
    }

    /// <summary>Renders an integer the one way this harness renders integers.</summary>
    internal static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);
}
