using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule J12: the register cites the assurance figures rather than restating them.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this rule exists, stated as the defect it was minted for.</b> Rule J10's register row
/// opened with <c>THIS RULE IS RED AT THIS MILESTONE, WHICH IS THE CLAUSE WORKING. Forty-four
/// units are assessed High or Critical; three of them carry a criterion, and the rule names the
/// other 41 one by one.</c> The generated report in the same repository said
/// <c>| Required and missing | 0 |</c>, and had said so for as long as anyone could find. The work
/// the row described as outstanding had been done and the row had not been touched.
/// </para>
/// <para>
/// <b>Nothing could see it.</b> <c>AssertTheRegisterRowIsWhatTheRulesImplement</c> compares the row
/// in <c>rules.register.json</c> against a hardcoded copy of the same prose in
/// <see cref="AssuranceRegisterRows"/>. Two copies of a claim agreeing with each other is not the
/// claim being true, and neither copy was ever compared to the tree.
/// </para>
/// <para>
/// <b>The fix is not a better comparison, it is removing the second copy of the number.</b> This
/// component already forbids a hand-maintained figure outright - rule J5's row says so about
/// <c>Human-reviewed: 47/47</c> - and a figure in a register row is the same object: a number a
/// human typed, which is current until the tree moves and silent when it does. A row that needs a
/// figure CITES one, as <c>{criteria:required}</c>, and this rule resolves the citation against
/// the generated report. There is then nothing to go stale, which is a stronger property than any
/// check that two numbers still match.
/// </para>
/// </remarks>
internal static class RegisterFigureRules
{
    /// <summary>A citation of an assurance figure, as a row is required to write one.</summary>
    internal const string Citation = @"\{(?<metric>[a-z]+:[a-z-]+)\}";

    /// <summary>
    /// The shapes in which a row states a COUNT OF UNITS against the criteria requirement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Adjacency, not co-occurrence, and the first version got that wrong.</b> This began as a
    /// sentence-level test - a sentence naming falsification criteria and carrying any figure -
    /// and it reported sixteen things on a clean register, every one of them innocent: rule J2's
    /// row counts the COMMENT LINES an annotation parses as, rule J11's counts the five CLAUSES a
    /// publish needs, and both mention criteria while doing it. A rule that fires on a row for
    /// discussing the subject is a rule about English.
    /// </para>
    /// <para>
    /// So the figure has to be adjacent to the thing it counts. Each pattern below is a way of
    /// writing "this many units, against the criterion requirement" and nothing else, which is
    /// narrow by design: <b>a claim worded outside them is not seen</b>, and that limit is in the
    /// register row rather than left for a reader to infer.
    /// </para>
    /// </remarks>
    internal static readonly string[] CountPatterns =
    [
        @"(?<figure>FIGURE)\s+units?\s+(are\s+|is\s+)?assessed",
        @"(?<figure>FIGURE)\s+units?\s+(carry|carries|carrying|owe|owes)",
        @"(?<figure>FIGURE)\s+of\s+(them|those|these)\s+(carry|carries|owe|owes)",
        @"\|\s*Units carrying a criterion\s*\|\s*(?<figure>\d+)",
        @"\|\s*Units required to carry one\s*\|\s*(?<figure>\d+)",
        @"\|\s*Required and missing\s*\|\s*(?<figure>\d+)",
    ];

    /// <summary>
    /// The phrases by which a row claims criteria are still outstanding.
    /// </summary>
    /// <remarks>
    /// Deliberately PRESENT TENSE. A row saying what the rule once did - "it named the other 41" -
    /// is history and stays legible; a row saying what it does now is a claim about this tree and
    /// is checked. The distinction is the whole reason the corrected J10 row can keep its own
    /// history without this rule firing on it.
    /// </remarks>
    internal static readonly string[] OutstandingVocabulary =
    [
        "is red at this milestone",
        "this rule is red",
        "rule is currently red",
        "currently red",
        "names the other",
        "the list of work it names",
    ];

    /// <summary>Every number word a figure could be spelled as, except "one".</summary>
    /// <remarks>
    /// "One" is excluded because this prose uses it as a pronoun in almost every row - "every one
    /// of them", "the one Deferred row" - and a rule that fired on those would be a rule about
    /// English rather than about figures. THE COST IS STATED IN THE ROW: a figure of one, spelled
    /// as a word, is not seen. It is the only figure that can hide, and a register row stating
    /// "one unit carries a criterion" is wrong in a way a reader can check by reading the report.
    /// </remarks>
    private const string NumberWords =
        "two|three|four|five|six|seven|eight|nine|ten|eleven|twelve|thirteen|fourteen|fifteen|" +
        "sixteen|seventeen|eighteen|nineteen|twenty|thirty|forty|fifty|sixty|seventy|eighty|" +
        "ninety|hundred";

    /// <summary>A figure, in digits or spelled out.</summary>
    private const string FigureSource =
        @"(?<![\w-])(?:\d+|(?:" + NumberWords +
        @")(?:-(?:one|two|three|four|five|six|seven|eight|nine))?)(?![\w-])";

    /// <summary>Each count pattern with its figure placeholder filled in.</summary>
    private static readonly Regex[] Counts = CountPatterns
        .Select(static pattern => new Regex(
            pattern.Replace("FIGURE", FigureSource, StringComparison.Ordinal),
            RegexOptions.IgnoreCase | RegexOptions.Compiled))
        .ToArray();

    /// <summary>
    /// Figures the generated report carries, beyond the three about criteria.
    /// </summary>
    /// <remarks>
    /// Curated rather than swept, because the report's tables reuse labels - <c>None</c>,
    /// <c>Low</c> and <c>High</c> each head a row in two different tables - and a catalog built by
    /// slugging every label would collide silently and resolve a citation to whichever row it met
    /// first. Every entry here is a label that appears once.
    /// </remarks>
    internal static readonly IReadOnlyDictionary<string, string> ReportMetrics =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["criteria:carrying"] = "Units carrying a criterion",
            ["criteria:required"] = "Units required to carry one",
            ["criteria:missing"] = "Required and missing",
            ["assurance:files"] = "Files scanned",
            ["assurance:units"] = "Code units",
            ["assurance:relevant"] = "Relevant",
            ["assurance:exempt"] = "Exempt by predicate",
            ["assurance:unverified"] = "Unverified",
        };

    /// <summary>
    /// Every figure a register row may cite: the report's, the graph's and the records'.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The graph figures are computed the way the rules that read the graph compute them</b>,
    /// from <see cref="ComponentGraph.Projects"/>, rather than parsed back out of a document. A
    /// catalog that re-derived them would be a second opinion about the tree, and the register
    /// would then cite a number no rule uses.
    /// </para>
    /// <para>
    /// <b>Every figure here is one a register row was found stating by hand.</b> The catalog is
    /// not speculative: rows A4, A7, A11, A12, K1, J1 and J4 each carried a count the tree
    /// contradicted, and this is the set needed to say those things by citation instead.
    /// </para>
    /// </remarks>
    internal static IReadOnlyDictionary<string, int> Figures(string report)
    {
        var figures = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var (metric, label) in ReportMetrics)
        {
            var match = Regex.Match(
                report,
                @"^\|\s*" + Regex.Escape(label) + @"\s*\|\s*(?<value>[\d,]+)\s*\|\s*$",
                RegexOptions.Multiline);

            if (match.Success)
            {
                figures[metric] = int.Parse(
                    match.Groups["value"].Value.Replace(",", string.Empty, StringComparison.Ordinal));
            }
        }

        var projects = ComponentGraph.Projects;

        figures["graph:projects"] = projects.Count;
        figures["graph:edges"] = projects.Sum(static project => project.ProjectReferences.Count);
        figures["graph:packable"] = projects.Count(static project => project.PackageId is not null);
        figures["graph:test-only"] = projects.Count(static project => project.IsTestOnly);
        figures["graph:composition-roots"] = projects.Count(static project =>
            project.AssemblyName.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal));
        figures["graph:javascript-family"] = projects.Count(static project =>
            project.AssemblyName.StartsWith("Broiler.VM.Profile.JavaScript", StringComparison.Ordinal));

        var adrs = Directory
            .EnumerateFiles(Path.Combine(ComponentGraph.Root, "docs", "adr"), "*.md")
            .Where(static path => Regex.IsMatch(Path.GetFileName(path), @"^\d{4}-"))
            .ToArray();

        figures["docs:adrs"] = adrs.Length;

        // Contract-bearing is the record's own declaration, read the way rule E2 reads it: a
        // **Core contract:** field whose value opens with "version". Counting them any other way
        // would give the register a figure no rule agrees with.
        figures["docs:contract-bearing"] = adrs.Count(static path => Regex.IsMatch(
            File.ReadAllText(path),
            @"\*\*Core contract:\*\*\s*version",
            RegexOptions.IgnoreCase));

        // A consumer profile is what a composition root reaches for that is not the core: the
        // three packable assemblies are what EVERY root references, so subtracting them leaves
        // exactly the profiles being consumed. Defined this way rather than by name-matching
        // ".Profile.", because the fixture profiles are deliberately not named that - rule A11's
        // subject is what a root consumes, not what a project is called.
        var roots = projects
            .Where(static project => project.AssemblyName.StartsWith(
                "Broiler.VM.Composition.", StringComparison.Ordinal))
            .ToArray();

        var core = projects
            .Where(static project => project.PackageId is not null)
            .Select(static project => project.AssemblyName)
            .ToHashSet(StringComparer.Ordinal);

        figures["graph:consumer-profiles"] = roots
            .SelectMany(static root => root.ProjectReferences)
            .Where(reference => !core.Contains(reference))
            .Distinct(StringComparer.Ordinal)
            .Count();

        figures["review:documents"] = ReviewRecordRuleTests.CorpusCount;
        figures["composition:registered"] = CompositionRegisterTests.RegisteredCount;

        figures["composition:catalog-tables"] = Directory
            .EnumerateFiles(
                Path.Combine(
                    ComponentGraph.Root, "docs", "evidence", ComponentGraph.CurrentEvidenceDirectory),
                "catalog-*.txt")
            .Count();

        // Two figures that are NOT counts of the tree, and the row citing each says so. They are
        // the arity of a rule and the size of a declared contract set: numbers fixed by a decision
        // rather than by what the checkout grew into. Citing them still beats retyping them, since
        // a decision that changes updates one array and every row citing it follows.
        figures["composition:fact-sources"] = CompositionRules.FactSources.Length;
        figures["contracts:profile-facing"] = ApiBaselineRules.ProfileFacingContracts.Length;

        // Read from the plan rather than computed as "files plus three", so that a fourth kind of
        // generated artefact is counted the day it is added rather than the day someone
        // remembers. Rows J5, J8 and J9 each said forty-eight, which was files-plus-three when
        // there were forty-five files.
        figures["assurance:artefacts"] = AssuranceGenerator.Current.Artefacts.Count;

        return figures;
    }

    /// <summary>
    /// J12's first clause: every citation names a metric the generated report defines.
    /// </summary>
    /// <remarks>
    /// This is the coupling the register did not have. A metric renamed or dropped from the
    /// report's table breaks every row that cites it, so the two documents cannot drift silently
    /// apart the way a typed number and a generated one could.
    /// </remarks>
    internal static IEnumerable<string> UnresolvableCitations(
        IEnumerable<(string Id, string Text)> rows, IReadOnlyDictionary<string, int> figures) => rows
        .SelectMany(row => Regex.Matches(row.Text, Citation)
            .Select(match => (row.Id, Metric: match.Groups["metric"].Value)))
        .Where(cited => !figures.ContainsKey(cited.Metric))
        .Select(cited =>
            $"the register row for {cited.Id} cites the figure {{{cited.Metric}}}, and the " +
            "figure catalog defines no such metric")
        .Distinct(StringComparer.Ordinal);

    /// <summary>
    /// J12's second clause: no row states a criteria figure of its own.
    /// </summary>
    /// <remarks>
    /// The clause with teeth. Clause one only holds the citations honest, and a row that typed the
    /// number back in would carry no citation at all - which is exactly the state J10's row was in
    /// when it went stale, so a rule that checked only citations would have been green over the
    /// defect it was minted for.
    /// </remarks>
    internal static IEnumerable<string> RestatedFigures(IEnumerable<(string Id, string Text)> rows) =>
        rows.SelectMany(row => Counts
            .SelectMany(pattern => pattern.Matches(Uncited(row.Text)).Cast<Match>())
            .Select(match => (row.Id, Figure: match.Groups["figure"].Value, Where: match.Value))
            .Select(found =>
                $"the register row for {found.Id} counts {found.Figure} units against the " +
                "falsification-criterion requirement, and a figure a human typed goes stale " +
                $"where a citation cannot: \"{Trim(found.Where)}\""));

    /// <summary>
    /// J12's third clause: no row claims criteria are outstanding while none are.
    /// </summary>
    /// <remarks>
    /// The defect itself, in one sentence. Both directions: when the report says work is
    /// outstanding, J10's row is the one that must say so, because a rule that is red over the
    /// tree and a register that does not mention it is the same failure pointing the other way.
    /// </remarks>
    internal static IEnumerable<string> OutstandingClaims(
        IEnumerable<(string Id, string Text)> rows, int missing)
    {
        var claiming = rows
            .Where(static row => OutstandingVocabulary.Any(phrase =>
                row.Text.Contains(phrase, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        if (missing == 0)
        {
            foreach (var row in claiming)
            {
                yield return
                    $"the register row for {row.Id} says falsification criteria are outstanding, " +
                    "and the generated report states 'Required and missing | 0'";
            }

            yield break;
        }

        if (!claiming.Any(static row => string.Equals(row.Id, "J10", StringComparison.Ordinal)))
        {
            yield return
                $"the generated report states {missing} unit(s) owe a falsification criterion and " +
                "carry none, and the register row for J10 does not say so";
        }
    }

    /// <summary>A claim that some number of things EXIST, which is a claim about the tree.</summary>
    /// <remarks>
    /// The lookbehind keeps IDENTIFIERS out. `ADR 0003 and VmCoreContract both exist` is not a
    /// count of anything, and neither is a revision, a section, an invariant or a rule number; a
    /// rule that read them as figures would ask the register to cite a document's name.
    /// </remarks>
    private static readonly Regex Existence = new(
        @"(?<!\b(?:ADR|revision|section|invariant|clause|rule)\s)(?<figure>" + FigureSource +
        @")\s+(?<subject>[a-z][a-z\- ]{0,60}?)\s+exists?(?![\w-])",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// J12's fourth clause: a counted existence claim cites its figure rather than stating it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the clause that closes the class, and it was minted because the class turned out
    /// to be populated.</b> The first three clauses cover the three criteria figures. A sweep for
    /// this shape - a number, a noun phrase, and the word "exist" - found eight rows asserting
    /// counts the tree contradicts: A4 said five test-only projects where there are nine and two
    /// composition roots where there are five, A7 said eight edges where there are fifty-nine, and
    /// J1 said 689 relevant units, 903 exempt ones and 1,592 code units where the generated report
    /// says 905, 1082 and 1987. Every one of them was green, because the row-equality test
    /// compares a row to a copy of itself.
    /// </para>
    /// <para>
    /// <b>The principle is one sentence: if the tree can compute it, cite it; if the tree cannot,
    /// do not state it as a number.</b> A row whose subject has no figure in the catalog is not
    /// exempt - it is a row asserting a count nothing can check, which is the thing this rule
    /// exists to stop, and the fix is to say the sentence without the number.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> UncitedExistenceClaims(
        IEnumerable<(string Id, string Text)> rows) => rows
        .SelectMany(row => Existence.Matches(Uncited(row.Text)).Cast<Match>()
            .Select(match => (row.Id, Claim: match.Value, Figure: match.Groups["figure"].Value)))
        .Select(found =>
            $"the register row for {found.Id} states that {found.Figure} of something exist " +
            $"without citing a figure: \"{Trim(found.Claim)}\" - a count of the tree written by " +
            "hand is current until the tree moves and silent when it does");

    /// <summary>
    /// The countable subjects a figure may be bound to only by citing one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every phrase here names a subject the figure catalog computes.</b> That is the whole
    /// discipline: the clause below reports a figure bound to one of these, and the reader's fix
    /// is a citation that already exists rather than a new metric or a shrug. A subject with no
    /// metric does not belong on this list, because the rule would then report a sentence nobody
    /// could repair.
    /// </para>
    /// <para>
    /// <b>What is deliberately absent</b> is every noun this register counts while describing a
    /// RULE rather than the tree - clauses, rounds, witnesses, copies, halves, shapes, questions,
    /// derivations, edits. Those are prose about design, they do not move when the checkout moves,
    /// and a rule that demanded citations for them would be asking the register to stop explaining
    /// itself.
    /// </para>
    /// </remarks>
    internal static readonly string[] CountableSubjects =
    [
        "covered source files",
        "covered product source files",
        "source files",
        "files",
        "units",
        "annotations",
        "artefacts",
        "pieces of generated text",
        "product assemblies",
        "packable assemblies",
        "family assemblies",
        "edges",
        "composition roots",
        "ADR files",
        "review documents",
    ];

    private static readonly Regex[] Subjects = CountableSubjects
        .Select(static subject => new Regex(
            FigureSource + @"\s+(?<subject>" + Regex.Escape(subject) + @")(?![\w-])",
            RegexOptions.IgnoreCase | RegexOptions.Compiled))
        .ToArray();

    /// <summary>
    /// J12's fifth clause: a figure bound to a countable subject cites rather than states.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The clause the fourth one could not see.</b> Rule J12's fourth clause reads adjacency to
    /// the word "exist", and rule H1's row said "the two evidence-bundle READMEs" - a figure bound
    /// to a NOUN, invisible to it, and wrong. That was found by reading, which is not a method.
    /// </para>
    /// <para>
    /// <b>Sweeping for the shape found the whole assurance family stale.</b> Seven rows said 45
    /// covered source files where there are 61, three said 48 artefacts where there are 64, and
    /// the unit figures - 689 annotated, 903 exempt, 1,592 in the tree - were each several hundred
    /// short of what the generated report states. The tree grew when the JavaScript profile came
    /// under coverage; the register did not.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> UncitedSubjectCounts(
        IEnumerable<(string Id, string Text)> rows) => rows
        .SelectMany(row => Subjects
            .SelectMany(pattern => pattern.Matches(Uncited(row.Text)).Cast<Match>())
            .Select(match => (row.Id, Claim: match.Value)))
        .Select(found =>
            $"the register row for {found.Id} counts a subject the catalog computes without " +
            $"citing it: \"{Trim(found.Claim)}\" - a figure a human typed is current until the " +
            "tree moves and silent when it does")
        .Distinct(StringComparer.Ordinal);

    /// <summary>The text with its citations removed, so a citation is not read as a figure.</summary>
    private static string Uncited(string sentence) => Regex.Replace(sentence, Citation, " ");

    private static string Trim(string sentence) =>
        sentence.Length <= 120 ? sentence : sentence[..117] + "...";
}
