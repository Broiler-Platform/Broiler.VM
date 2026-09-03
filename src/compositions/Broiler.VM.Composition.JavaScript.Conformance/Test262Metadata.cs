namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>A suite's declared negative expectation: when it must fail, and as what.</summary>
/// <param name="Phase">
/// <c>parse</c>, <c>resolution</c> or <c>runtime</c> - which of the host's stages must produce it.
/// </param>
/// <param name="Type">The JavaScript error type, by name: <c>SyntaxError</c> and its neighbours.</param>
internal sealed record Test262Negative(string Phase, string Type);

/// <summary>The metadata block a suite file carries, in the suite's own dialect.</summary>
/// <param name="Description">What the test is about.</param>
/// <param name="Esid">The specification clause the test cites, or empty.</param>
/// <param name="Flags">How the file must be run.</param>
/// <param name="Features">The named language features it uses.</param>
/// <param name="Includes">The harness files it needs loaded first.</param>
/// <param name="Negative">Its declared failure, or null where it declares none.</param>
internal sealed record Test262Frontmatter(
    string Description,
    string Esid,
    IReadOnlyList<string> Flags,
    IReadOnlyList<string> Features,
    IReadOnlyList<string> Includes,
    Test262Negative? Negative);

/// <summary>
/// Reads the metadata dialect a real conformance suite writes, which is not the one this
/// component's own fixtures write.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because the claim that the two dialects were the same was false, and was
/// measured false.</b> <see cref="TestMetadata"/> reads a flat <c>key: value</c> block and requires
/// an <c>expected</c> key that this component invented; a suite file has no such key, writes
/// <c>negative</c> as a nested mapping, and folds its description over several lines. Five
/// suite-shaped files put through the harness on 2026-09-03 were refused five times with "declares
/// no readable expectation", and a suite is refused whole - so the harness would have scored
/// nothing at all, not scored it badly.
/// </para>
/// <para>
/// <b>This is a reader for a format and holds no suite content.</b> The dialect is a public
/// interchange format; what it carries is separately licensed material that this repository does
/// not hold and this reader never fetches. Nothing here embeds a test, a path, an expectation or
/// a revision from any suite.
/// </para>
/// <para>
/// <b>It is not a YAML parser and does not pretend to be.</b> It reads the shapes the dialect
/// actually uses - scalars, folded and literal block scalars, inline and block sequences, and one
/// level of nested mapping - and <b>refuses what it does not recognise rather than skipping it</b>.
/// A metadata reader that ignored a key it could not parse would run a file under the wrong rules
/// while reporting a clean total, and the key it silently dropped would most likely be the one
/// that said the file must fail.
/// </para>
/// </remarks>
internal static class Test262Metadata
{
    /// <summary>Where the metadata block opens.</summary>
    private const string Open = "/*---";

    /// <summary>Where it closes.</summary>
    private const string Close = "---*/";

    /// <summary>
    /// The flags the dialect defines, all of which change how a file is run.
    /// </summary>
    /// <remarks>
    /// <b>An unknown flag is a refusal and not a shrug.</b> Every flag here changes the rules a
    /// file runs under - its goal symbol, its strictness, whether a harness prelude is prepended,
    /// how completion is signalled - so a flag this reader does not know is a file it does not
    /// know how to run. Running it anyway under the default rules is how a suite grows a
    /// silently-mis-run corner.
    /// </remarks>
    internal static IReadOnlySet<string> KnownFlags { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "onlyStrict",
        "noStrict",
        "module",
        "raw",
        "async",
        "generated",
        "CanBlockIsFalse",
        "CanBlockIsTrue",
        "non-deterministic",
    };

    /// <summary>The phases a negative expectation may name.</summary>
    internal static IReadOnlySet<string> KnownPhases { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "parse",
        "resolution",
        "runtime",
    };

    /// <summary>Reads one file's metadata block, or says why it is not one.</summary>
    internal static bool TryRead(
        string path,
        string text,
        out Test262Frontmatter frontmatter,
        out string failure)
    {
        frontmatter = null!;
        var open = text.IndexOf(Open, StringComparison.Ordinal);

        if (open < 0)
        {
            failure = $"{path} carries no {Open} … {Close} metadata block";
            return false;
        }

        var close = text.IndexOf(Close, open + Open.Length, StringComparison.Ordinal);

        if (close < 0)
        {
            failure = $"{path} opens a metadata block and never closes it";
            return false;
        }

        // EVERY LINE TERMINATOR THE LANGUAGE HAS, and a real suite uses more than one. A file
        // written with CARRIAGE RETURNS ALONE - which one test262 file is, deliberately, because
        // its subject is line-terminator normalisation - became a single line here, so no key was
        // found and the reader reported that the file declared no description. The order matters:
        // the pair first, then a lone carriage return, or a CRLF file gains a blank line between
        // every pair.
        var lines = Dedent(
            text[(open + Open.Length)..close]
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n'));

        var description = string.Empty;
        var esid = string.Empty;
        var flags = new List<string>();
        var features = new List<string>();
        var includes = new List<string>();
        Test262Negative? negative = null;

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];

            if (line.Trim().Length == 0)
            {
                continue;
            }

            // A line that is indented at this point is a continuation of a key this loop has
            // already consumed - every reader below advances `index` past what it took - so
            // reaching one here means the block indented something under nothing.
            if (Indent(line) > 0)
            {
                failure = $"{path} indents `{line.Trim()}` under no key";
                return false;
            }

            var colon = line.IndexOf(':', StringComparison.Ordinal);

            if (colon <= 0)
            {
                failure = $"{path} carries `{line.Trim()}` in its metadata block, which is not `key: value`";
                return false;
            }

            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();

            switch (key)
            {
                case "description":
                    description = Scalar(lines, ref index, value);
                    break;

                case "esid":
                    esid = Scalar(lines, ref index, value);
                    break;

                case "flags":
                    if (!Sequence(path, lines, ref index, value, out flags, out failure))
                    {
                        return false;
                    }

                    break;

                case "features":
                    if (!Sequence(path, lines, ref index, value, out features, out failure))
                    {
                        return false;
                    }

                    break;

                case "includes":
                    if (!Sequence(path, lines, ref index, value, out includes, out failure))
                    {
                        return false;
                    }

                    break;

                case "negative":
                    if (!Negative(path, lines, ref index, value, out negative, out failure))
                    {
                        return false;
                    }

                    break;

                default:
                    // An unrecognised KEY is skipped and an unrecognised FLAG is refused, and the
                    // asymmetry is the point. The dialect's key set is open - `author`, `es5id`,
                    // `info`, `timeout`, `locale` and more appear, and new ones are added without
                    // changing how any file runs. A flag is the opposite: every one of them
                    // changes the rules, so one this reader does not know is a file it cannot run.
                    _ = Scalar(lines, ref index, value);
                    break;
            }
        }

        if (description.Length == 0)
        {
            failure = $"{path} declares no description";
            return false;
        }

        foreach (var flag in flags)
        {
            if (!KnownFlags.Contains(flag))
            {
                failure = $"{path} declares the flag `{flag}`, which this harness does not know how to honour";
                return false;
            }
        }

        if (flags.Contains("onlyStrict", StringComparer.Ordinal) &&
            flags.Contains("noStrict", StringComparer.Ordinal))
        {
            failure = $"{path} declares both onlyStrict and noStrict, which leaves no way to run it";
            return false;
        }

        frontmatter = new Test262Frontmatter(description, esid, flags, features, includes, negative);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Removes the indentation the whole block shares, so that a block indented as a unit reads
    /// the same as one at the margin.
    /// </summary>
    /// <remarks>
    /// <b>Relative indentation is what the dialect means, and this reader used to take the
    /// absolute kind.</b> A real suite file whose entire metadata block is indented by one space -
    /// which is legal and which one test262 file does - had every one of its keys reported as
    /// "indented under no key", because the check for a top-level key compared against column
    /// zero. Dedenting once here is what lets every reader below keep comparing against zero.
    /// Blank lines are ignored when measuring, because a blank line has no indentation to speak of
    /// and counting it as zero would make every block flush.
    /// </remarks>
    private static string[] Dedent(string[] lines)
    {
        var shared = int.MaxValue;

        foreach (var line in lines)
        {
            if (line.Trim().Length != 0)
            {
                shared = Math.Min(shared, Indent(line));
            }
        }

        if (shared is 0 or int.MaxValue)
        {
            return lines;
        }

        for (var index = 0; index < lines.Length; index++)
        {
            lines[index] = lines[index].Length <= shared
                ? string.Empty
                : lines[index][shared..];
        }

        return lines;
    }

    /// <summary>How many spaces a line opens with.</summary>
    private static int Indent(string line)
    {
        var count = 0;

        while (count < line.Length && line[count] == ' ')
        {
            count++;
        }

        return count;
    }

    /// <summary>
    /// Reads a scalar, following a folded (<c>&gt;</c>) or literal (<c>|</c>) block onto the lines
    /// under it.
    /// </summary>
    /// <remarks>
    /// Both block forms are joined with single spaces here. The distinction between them is where
    /// line breaks survive, and nothing this harness does with a description or an <c>esid</c>
    /// depends on one: they are reported, not executed.
    /// </remarks>
    private static string Scalar(string[] lines, ref int index, string value)
    {
        var block = value is ">" or "|" or ">-" or "|-";

        if (value.Length != 0 && !block)
        {
            return value;
        }

        var parts = new List<string>();

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            parts.Add(next.Trim());
            index++;
        }

        return string.Join(' ', parts);
    }

    /// <summary>Reads a list, in the inline or the block-sequence spelling.</summary>
    private static bool Sequence(
        string path,
        string[] lines,
        ref int index,
        string value,
        out List<string> items,
        out string failure)
    {
        items = [];
        failure = string.Empty;
        var trimmed = value.Trim();

        if (trimmed.StartsWith('['))
        {
            if (!trimmed.EndsWith(']'))
            {
                failure = $"{path} opens an inline list with `{trimmed}` and does not close it";
                return false;
            }

            items.AddRange(Split(trimmed[1..^1]));
            return true;
        }

        if (trimmed.Length != 0)
        {
            items.AddRange(Split(trimmed));
            return true;
        }

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            var item = next.Trim();

            if (!item.StartsWith("- ", StringComparison.Ordinal))
            {
                failure = $"{path} carries `{item}` under a list, which is not a `- item` entry";
                return false;
            }

            items.Add(item[2..].Trim());
            index++;
        }

        return true;
    }

    /// <summary>Reads the nested mapping a negative expectation is written as.</summary>
    /// <remarks>
    /// <b>Both members are required.</b> A phase with no type says a file must fail without saying
    /// as what, which a harness can only score by accepting any failure - and accepting any failure
    /// is the scoring bug this whole ingestion path is built to refuse.
    /// </remarks>
    private static bool Negative(
        string path,
        string[] lines,
        ref int index,
        string value,
        out Test262Negative? negative,
        out string failure)
    {
        negative = null;

        if (value.Trim().Length != 0)
        {
            failure = $"{path} writes `negative: {value.Trim()}`, and a negative expectation is a phase and a type";
            return false;
        }

        var phase = string.Empty;
        var type = string.Empty;

        while (index + 1 < lines.Length)
        {
            var next = lines[index + 1];

            if (next.Trim().Length == 0)
            {
                index++;
                continue;
            }

            if (Indent(next) == 0)
            {
                break;
            }

            var line = next.Trim();
            var colon = line.IndexOf(':', StringComparison.Ordinal);

            if (colon <= 0)
            {
                failure = $"{path} carries `{line}` under `negative`, which is not `key: value`";
                return false;
            }

            var key = line[..colon].Trim();
            var member = line[(colon + 1)..].Trim();

            switch (key)
            {
                case "phase":
                    phase = member;
                    break;
                case "type":
                    type = member;
                    break;
                default:
                    failure = $"{path} carries `{key}` under `negative`, which is neither phase nor type";
                    return false;
            }

            index++;
        }

        if (phase.Length == 0 || type.Length == 0)
        {
            failure = $"{path} declares a negative expectation with no {(phase.Length == 0 ? "phase" : "type")}";
            return false;
        }

        if (!KnownPhases.Contains(phase))
        {
            failure = $"{path} declares the negative phase `{phase}`, which this harness does not know";
            return false;
        }

        negative = new Test262Negative(phase, type);
        failure = string.Empty;
        return true;
    }

    /// <summary>Splits a comma-separated list, dropping the empties a trailing comma leaves.</summary>
    private static IEnumerable<string> Split(string value) =>
        value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}
