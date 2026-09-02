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
    /// <summary>The metrics a row may cite, and the report line each is read from.</summary>
    /// <remarks>
    /// Read from the GENERATED REPORT rather than recomputed from the units. Recomputing would
    /// make this rule a second implementation of the report's own counters, which is the drift the
    /// citation is meant to remove; and the report is held to the units by rule J8 and to the
    /// generator by rule J5, so reading it here is reading a figure two rules already keep honest.
    /// </remarks>
    internal static readonly IReadOnlyDictionary<string, string> Metrics =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["carrying"] = "Units carrying a criterion",
            ["required"] = "Units required to carry one",
            ["missing"] = "Required and missing",
        };

    /// <summary>A citation of an assurance figure, as a row is required to write one.</summary>
    internal const string Citation = @"\{criteria:(?<metric>[a-z]+)\}";

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

    /// <summary>The criteria figures the generated report carries.</summary>
    internal static IReadOnlyDictionary<string, int> Figures(string report)
    {
        var figures = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var (metric, label) in Metrics)
        {
            var match = Regex.Match(
                report,
                @"^\|\s*" + Regex.Escape(label) + @"\s*\|\s*(?<value>\d+)\s*\|\s*$",
                RegexOptions.Multiline);

            if (match.Success)
            {
                figures[metric] = int.Parse(match.Groups["value"].Value);
            }
        }

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
            $"the register row for {cited.Id} cites the assurance figure {{criteria:{cited.Metric}}}, " +
            "and the generated report defines no such metric")
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

    /// <summary>The text with its citations removed, so a citation is not read as a figure.</summary>
    private static string Uncited(string sentence) => Regex.Replace(sentence, Citation, " ");

    private static string Trim(string sentence) =>
        sentence.Length <= 120 ? sentence : sentence[..117] + "...";
}
