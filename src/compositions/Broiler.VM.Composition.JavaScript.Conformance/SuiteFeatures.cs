namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The suite's own declaration of which feature names are proposals and which are the language.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because "is this construct in the language" is not a question this component can
/// answer for itself, and it was answering it by accident.</b> A test annotated with a feature flag
/// asks about a construct; if that construct is a proposal rather than a published edition, the
/// test asks about a language nothing here claims to implement, and scoring it is wrong in both
/// directions - a failure that is not a gap, and a pass that is not a credit. Roadmap
/// <see href="../../../Broiler.VM.Profile.JavaScript/docs/roadmap.md">section 3</see> records the
/// language edition as an unpinned external dependency, so this component has no edition to check
/// a construct against.
/// </para>
/// <para>
/// <b>The authority is the suite's, which is what makes this a reading rather than a claim.</b>
/// The ingested dialect's suite ships a <c>features.txt</c> that splits its own flags into a
/// proposed section and a standard one, and says in its own prose that the proposed flags exist
/// "so that consumers may more easily omit them as necessary". Omitting them is exactly what this
/// is for. A hand-written list in this repository would have been a list this component chose, and
/// a list this component chose is one it can quietly grow whenever something fails.
/// </para>
/// <para>
/// <b>The <c>##</c> prefix is NOT what marks a section.</b> The file uses it for ordinary comments
/// inside a section as well - the Source Phase Imports entry has two - so a reader keying on the
/// prefix silently splits the proposed section in half and stops excluding everything after it.
/// The headings are matched by their whole text, and a file that does not carry all three is
/// refused rather than read: a reader that found no proposed section would exclude nothing, which
/// is precisely the state this file exists to end.
/// </para>
/// </remarks>
internal sealed record SuiteFeatures(
    IReadOnlySet<string> Proposed,
    IReadOnlySet<string> Standard,
    IReadOnlySet<string> TestHarness)
{
    /// <summary>What the file is called at the root of an ingested suite.</summary>
    internal const string FileName = "features.txt";

    /// <summary>The heading over the flags naming proposals rather than the language.</summary>
    internal const string ProposedHeading = "## Proposed language features";

    /// <summary>The heading over the flags naming constructs of a published edition.</summary>
    internal const string StandardHeading = "## Standard language features";

    /// <summary>
    /// The heading over the flags naming host capabilities the suite needs and the language has
    /// not got.
    /// </summary>
    /// <remarks>
    /// <b>Read and not excluded, and the difference is measured rather than assumed.</b> These
    /// name functions on the suite's <c>$262</c> object, so a test claiming one needs a call - a
    /// construct <c>broiler.javascript.slice</c> does not admit - and every such test is already
    /// counted as unselectable one stage later. Excluding them here would move those cases into a
    /// figure about the language edition, which is not what they are about. The census over the
    /// pinned checkout is what says no scored case claims one; if that ever stops being true, the
    /// case shows up in a total and this comment is what it contradicts.
    /// </remarks>
    internal const string TestHarnessHeading = "## Test-Harness Features";

    /// <summary>Every name the file declares, whichever section declared it.</summary>
    internal IReadOnlySet<string> All =>
        new HashSet<string>(Proposed.Concat(Standard).Concat(TestHarness), StringComparer.Ordinal);

    /// <summary>An empty list, which declares nothing and therefore excludes nothing.</summary>
    internal static SuiteFeatures None { get; } = new(
        new HashSet<string>(StringComparer.Ordinal),
        new HashSet<string>(StringComparer.Ordinal),
        new HashSet<string>(StringComparer.Ordinal));

    /// <summary>Reads a suite's feature list, or says why the file is not one.</summary>
    internal static SuiteFeatures Read(string path, out IReadOnlyList<string> complaints)
    {
        if (!File.Exists(path))
        {
            complaints =
            [
                $"{path} does not exist, so this run has no way to tell a proposal from the " +
                "language and would score tests about constructs no edition contains",
            ];

            return None;
        }

        return Parse(File.ReadAllText(path), path, out complaints);
    }

    /// <summary>Reads the text of a feature list, or says why the text is not one.</summary>
    internal static SuiteFeatures Parse(string text, string origin, out IReadOnlyList<string> complaints)
    {
        var found = new List<string>();
        var sections = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            [ProposedHeading] = new(StringComparer.Ordinal),
            [StandardHeading] = new(StringComparer.Ordinal),
            [TestHarnessHeading] = new(StringComparer.Ordinal),
        };

        var seenHeadings = new List<string>();
        var declaredIn = new Dictionary<string, string>(StringComparer.Ordinal);
        string? section = null;
        var number = 0;

        foreach (var raw in text
                     .Replace("\r\n", "\n", StringComparison.Ordinal)
                     .Replace('\r', '\n')
                     .Split('\n'))
        {
            number++;
            var line = raw.Trim();

            if (line.Length == 0)
            {
                continue;
            }

            if (sections.ContainsKey(line))
            {
                if (seenHeadings.Contains(line, StringComparer.Ordinal))
                {
                    found.Add($"{origin}:{number}: `{line}` heads a second section of the same name");
                }

                seenHeadings.Add(line);
                section = line;
                continue;
            }

            // Every other `#` line is a comment, `##` included. That is not a tolerance: the file
            // uses `##` for the URL of a proposal and for a note about a specifier, both inside a
            // section, and a reader that took either for a heading would end the proposed section
            // early and quietly stop excluding the entries below it.
            if (line[0] == '#')
            {
                continue;
            }

            // A name may carry a trailing comment. The name is what precedes it; no feature name
            // contains a `#`.
            var comment = line.IndexOf('#', StringComparison.Ordinal);
            var name = (comment < 0 ? line : line[..comment]).Trim();

            if (name.Length == 0)
            {
                continue;
            }

            if (section is null)
            {
                found.Add($"{origin}:{number}: `{name}` is declared under no section heading");
                continue;
            }

            if (declaredIn.TryGetValue(name, out var already))
            {
                found.Add(
                    $"{origin}:{number}: `{name}` is declared under `{already}` and under " +
                    $"`{section}`, so its standing is whichever the reader saw last");

                continue;
            }

            declaredIn[name] = section;
            sections[section].Add(name);
        }

        foreach (var heading in sections.Keys)
        {
            if (!seenHeadings.Contains(heading, StringComparer.Ordinal))
            {
                found.Add($"{origin} carries no `{heading}` section");
            }
            else if (sections[heading].Count == 0)
            {
                found.Add($"{origin}: the `{heading}` section declares no feature");
            }
        }

        complaints = found;

        return found.Count != 0
            ? None
            : new SuiteFeatures(
                sections[ProposedHeading],
                sections[StandardHeading],
                sections[TestHarnessHeading]);
    }
}
