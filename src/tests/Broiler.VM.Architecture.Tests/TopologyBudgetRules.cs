using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule A15: ADR 0001's last budget sentence states the graph's current size.
/// </summary>
/// <remarks>
/// <para>
/// <b>Minted for an omission its author made.</b> ADR 0001's budget section is what authorises a
/// project set at all - it says the set "may not grow without a dated revision" - and every
/// project-adding revision in the record states the growth: <c>The graph goes from 17 projects and
/// 46 edges to 19 and 55</c>. The revision that added the Android composition root on 2026-09-02
/// states what the head composes, why it is an application and what it does not settle, and
/// <b>carries no budget paragraph at all</b>. The record's last stated size was 19 projects and 55
/// edges; the graph held 20 and 59.
/// </para>
/// <para>
/// <b>Why nothing caught it.</b> Rule A7 holds <c>graph.manifest.json</c> to the project files, and
/// both of those ARE the tree - a manifest and a graph agreeing tells you nothing about a document
/// that describes them. The budget sentence was prose no rule read, which is rule J10's register
/// row one document over, and it was found by a sweep rather than by the suite.
/// </para>
/// <para>
/// <b>The rule reads the LAST such sentence and nothing else.</b> Every earlier one is history -
/// "goes from 8 projects to 12" was true at VM-3 and must stay written - so a rule checking all of
/// them would demand that the record forget what it recorded. Only the final figure is a claim
/// about this checkout.
/// </para>
/// </remarks>
internal static class TopologyBudgetRules
{
    /// <summary>Where the topology record lives, relative to the component root.</summary>
    internal const string Record = "docs/adr/0001-component-topology-and-dependency-graph.md";

    /// <summary>
    /// A budget-growth sentence, in the two shapes this record uses.
    /// </summary>
    /// <remarks>
    /// The edge half is optional because the earliest revisions state projects alone - "goes from
    /// 8 projects to 12, and test-only projects from 5 to 7". Those are history and are never the
    /// last match, but a pattern that required edges would fail to parse them at all and the
    /// "last" it found would silently be the wrong sentence.
    /// </remarks>
    private static readonly Regex Growth = new(
        @"graph goes from (?<fromProjects>\d+) projects(?: and (?<fromEdges>\d+) edges)? to " +
        @"(?<toProjects>\d+)(?: and (?<toEdges>\d+))?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Every budget-growth sentence the record carries, in order.</summary>
    internal static IReadOnlyList<(int Projects, int? Edges)> Budgets(string record) => Growth
        .Matches(record)
        .Select(static match => (
            Projects: int.Parse(match.Groups["toProjects"].Value),
            Edges: match.Groups["toEdges"].Success
                ? int.Parse(match.Groups["toEdges"].Value)
                : (int?)null))
        .ToArray();

    /// <summary>
    /// A15's clean direction: the record's last budget is the graph's size now.
    /// </summary>
    /// <remarks>
    /// A record that states NO budget at all is reported rather than passing. That is the whole
    /// defect this rule exists for - the failure was an absent paragraph, not a wrong number, and
    /// a rule that read "no sentence, nothing to disagree with" would have been green on it.
    /// </remarks>
    internal static IEnumerable<string> A15(string record, int projects, int edges)
    {
        var budgets = Budgets(record);

        if (budgets.Count == 0)
        {
            yield return
                $"{Record} states no project budget at all, and the graph holds {projects} " +
                "projects: the record that authorises the project set has stopped describing it";

            yield break;
        }

        var (statedProjects, statedEdges) = budgets[^1];

        if (statedProjects != projects)
        {
            yield return
                $"{Record}'s last budget states {statedProjects} projects and the graph holds " +
                $"{projects}: a project was added without the dated revision that authorises it, " +
                "or the revision omitted its budget paragraph";
        }

        if (statedEdges is null)
        {
            yield return
                $"{Record}'s last budget states a project count and no edge count, and the graph " +
                $"holds {edges} edges";
        }
        else if (statedEdges.Value != edges)
        {
            yield return
                $"{Record}'s last budget states {statedEdges.Value} edges and the graph holds " +
                $"{edges}";
        }
    }
}
