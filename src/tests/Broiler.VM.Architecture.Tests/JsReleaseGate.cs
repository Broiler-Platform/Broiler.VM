using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The JavaScript profile's release gate: roadmap section 22's gates 1 to 13, read from the checkout,
/// answering a verdict and every blocker by the declaration that states it (phase F9 slice R4,
/// decision JSD-0062).
/// </summary>
/// <remarks>
/// <para>
/// <b>What it reads, and what it never reads.</b> A human decision is a <c>// Broiler-Human:</c>
/// line on a unit's declaration, and the gate counts those and nothing else: no headline, no
/// document, no table and no commit message can supply a reviewer it does not find on a unit. A
/// headline that says more than the units do is itself a blocker. So the gate cannot be argued into
/// passing by any file a person or a model writes, short of the per-unit decisions the policy defines.
/// </para>
/// <para>
/// <b>Three kinds of answer per gate.</b> A gate is <i>held</i> by the rules it names, each of which
/// must be <c>Active</c> in the register; it is <i>blocked</i> by an act only the owner or a named
/// human can take, named with the declaration that states it is missing; or a clause is
/// <i>unread</i>, because this check reads no mechanism for it. An unread clause blocks as surely as
/// a missing act: a gate that passed what it did not read would be the false record the whole
/// release discipline exists to prevent.
/// </para>
/// </remarks>
internal static class JsReleaseGate
{
    internal sealed record Blocker(string Id, int Gate, string Declaration, string What);

    internal sealed record GateState(int Gate, string Name, IReadOnlyList<string> HeldBy, IReadOnlyList<Blocker> Blockers);

    /// <summary>The texts a witness may replace; everything else is read from the checkout.</summary>
    internal sealed record Inputs(string SupportTable, string ReviewHeadline, string Notices, string Compositions)
    {
        internal static Inputs FromCheckout() => new(
            Read("src", "Broiler.VM.Profile.JavaScript", "docs", "support.md"),
            Read("HUMAN_REVIEW.md"),
            Read("THIRD_PARTY_NOTICES.md"),
            Read("docs", "compositions.md"));
    }

    internal const string Refused = "REFUSED";

    internal const string Bundle = "js-10-003";

    private static readonly string[] Runs = ["wide-bytecode", "wide-native", "slice-bytecode", "numeric-native"];

    private static readonly string[] FlooredRuns = ["wide-bytecode", "wide-native"];

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine([ComponentGraph.Root, .. parts]));

    internal static string Verdict(IReadOnlyList<GateState> gates) =>
        gates.Any(static gate => gate.Blockers.Count != 0) ? Refused : "PASSED";

    internal static List<GateState> Evaluate(Inputs inputs)
    {
        var (relevant, decided) = Decisions();
        var gates = new List<GateState>();

        // 1. SUPPORT TRUTH: the table's cells are held by N34 to N36; issuing it is the owner's act.
        var g1 = new List<Blocker>();

        if (inputs.SupportTable.Contains("DRAFTED, NOT ISSUED", StringComparison.Ordinal))
        {
            g1.Add(new("G1-not-issued", 1, "support.md, its banner: DRAFTED, NOT ISSUED", "the support table is drafted and not issued; issuing it is the owner's act, after gate 11"));
        }
        else if (decided < relevant)
        {
            g1.Add(new("G1-issued-without-review", 1, "support.md, which no longer says it is not issued", "the support table reads as issued while relevant units carry no human decision"));
        }

        gates.Add(Gate(1, "Support truth", ["N34", "N35", "N36"], g1));

        // 2. GRAPH AND REGISTRATION: every clause has a rule.
        gates.Add(Gate(2, "Graph and registration", ["A7", "N1", "N2", "B5", "B5b"], []));

        // 3 TO 6: shown by retained bundles, read by no mechanism here.
        gates.Add(Gate(3, "Correctness and safety", ["V9", "N5", "N6", "N7", "N11"], [Unread(3, "the malformed corpus's replay on all three publish modes, the closed fuzz counterexamples, and the absence of a check after verification are shown, where they are shown at all, by retained bundles")]));
        gates.Add(Gate(4, "Lifecycle and results", ["V8"], [Unread(4, "the step-kind mapping, the escaping exception, the suspension holding no thread and the call-stack overflow on every claimed RID under Native AOT are shown, where they are shown at all, by retained bundles")]));
        gates.Add(Gate(5, "Guest loads and policy", [], [Unread(5, "the deterministic refusal without a provider, the conversion table in both directions and uncatchable exhaustion are shown, where they are shown at all, by retained bundles")]));
        gates.Add(Gate(6, "Host boundary", [], [Unread(6, "exact binding at creation, no partially bound runtime, every optional import's unbound branch and each capability's translation mode are shown, where they are shown at all, by retained bundles")]));

        // 7. NATIVE AOT, over advertised compositions on claimed RIDs.
        var g7 = new List<Blocker>();

        if (inputs.Compositions.Contains("## 1. The advertised set is empty", StringComparison.Ordinal))
        {
            g7.Add(new("G7-no-composition-advertised", 7, "docs/compositions.md section 1: The advertised set is empty", "no composition is advertised; advertising one is the owner's act"));
        }

        if (inputs.SupportTable.Contains("**No runtime identifier is claimed.**", StringComparison.Ordinal))
        {
            g7.Add(new("G7-no-rid-claimed", 7, "support.md section 8: No runtime identifier is claimed", "no RID is claimed; claiming one is a release act gate 11 forbids before review"));
        }

        gates.Add(Gate(7, "Native AOT", ["K1", "K2", "K3", "K4", "K5"], g7));

        // 8. PACKAGES AND CONSUMERS: held by the package baseline's rule and the consumer's.
        gates.Add(Gate(8, "Packages and consumers", ["N37", "A14"], []));

        // 9. CONFORMANCE: the release-candidate bundle, read file by file.
        gates.Add(Gate(9, "Conformance", ["N15"], ConformanceBlockers(inputs.SupportTable)));

        // 10. MEASUREMENT HONESTY: held by the profile's baseline register's rule.
        gates.Add(Gate(10, "Measurement honesty", ["N33"], []));

        // 11. HUMAN REVIEW: the units, and nothing else.
        var g11 = new List<Blocker>();

        if (decided < relevant)
        {
            g11.Add(new("G11-units-undecided", 11, "the // Broiler-Human: line of every relevant unit of the JavaScript family", $"{decided} of {relevant} relevant units of the JavaScript family carry a human decision"));
        }

        var headline = Regex.Match(inputs.ReviewHeadline, @"Human-reviewed: (?<count>[0-9]+) of (?<of>[0-9]+)");

        int? claimed = headline.Success ? int.Parse(headline.Groups["count"].Value, System.Globalization.CultureInfo.InvariantCulture) : null;
        var pendingHeadline = inputs.ReviewHeadline.Contains("**Status: PENDING.**", StringComparison.Ordinal);

        // A HEADLINE IS NOT A DECISION. It is checked against the units, never counted as one.
        if (claimed is null || (decided < relevant && !pendingHeadline) || (decided == 0 && claimed > 0))
        {
            g11.Add(new("G11-headline-contradicts-units", 11, "HUMAN_REVIEW.md, its status line", "the review record's headline claims more than the units' own human lines record"));
        }

        if (Regex.IsMatch(Section(inputs.SupportTable, "## 9."), @"published to nuget\.org at"))
        {
            g11.Add(new("G11-published-before-review", 11, "support.md section 9, and ADR 0001's revision of 2026-09-28", "package versions were published to nuget.org before any review, and answering that is the owner's decision"));
        }

        gates.Add(Gate(11, "Human review", ["J3", "J4", "J11"], g11));

        // 12. LICENCE AND ATTRIBUTION: the notices' own confirmation column.
        gates.Add(Gate(12, "Licence and attribution", ["N22", "N27"], NoticeBlockers(inputs.Notices)));

        // 13. OPERATIONS: the holders section of the table.
        var g13 = new List<Blocker>();

        foreach (var line in Section(inputs.SupportTable, "## 10.").Split('\n'))
        {
            var cells = line.Trim().Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (line.StartsWith('|') && cells.Length == 3 && cells[1].Contains("**Vacant**", StringComparison.Ordinal))
            {
                g13.Add(new("G13-vacant-" + Slug(cells[0]), 13, $"support.md section 10, the row {cells[0]}", $"no holder is named for {cells[0].ToLowerInvariant()}; naming one is the owner's appointment"));
            }

            if (line.StartsWith('|') && cells.Length == 3 && cells[1].Contains("no security contact and no intake channel exists", StringComparison.Ordinal))
            {
                g13.Add(new("G13-no-security-intake", 13, $"support.md section 10, the row {cells[0]}", "no security contact and no intake channel exists for a report about the parser or the interpreter"));
            }
        }

        gates.Add(Gate(13, "Operations", [], g13));
        return gates;
    }

    /// <summary>Relevant units of the family, from the assurance manifest; decided ones, from their own human lines.</summary>
    private static (int Relevant, int Decided) Decisions()
    {
        using var manifest = System.Text.Json.JsonDocument.Parse(Read("assurance.manifest.json"));
        var files = new HashSet<string>(StringComparer.Ordinal);
        var relevant = 0;

        foreach (var unit in manifest.RootElement.GetProperty("units").EnumerateArray())
        {
            var file = unit.GetProperty("file").GetString()!;

            if (!file.StartsWith("src/Broiler.VM.Profile.JavaScript", StringComparison.Ordinal))
            {
                continue;
            }

            files.Add(file);
            relevant += unit.GetProperty("exempt").GetBoolean() ? 0 : 1;
        }

        var decided = 0;

        foreach (var file in files)
        {
            foreach (var line in File.ReadLines(Path.Combine(ComponentGraph.Root, file)))
            {
                var trimmed = line.Trim();

                if (!trimmed.StartsWith(AssuranceAnnotation.HumanMarker, StringComparison.Ordinal))
                {
                    continue;
                }

                var body = trimmed[AssuranceAnnotation.HumanMarker.Length..].Trim();

                if (body.Length != 0 &&
                    !string.Equals(body, AssuranceAnnotation.Pending, StringComparison.Ordinal) &&
                    !body.StartsWith(AssuranceAnnotation.Stale, StringComparison.Ordinal))
                {
                    decided++;
                }
            }
        }

        return (relevant, decided);
    }

    private static List<Blocker> ConformanceBlockers(string supportTable)
    {
        var blockers = new List<Blocker>();
        var directory = Path.Combine(ComponentGraph.Root, "src", "Broiler.VM.Profile.JavaScript", "docs", "evidence", Bundle);
        var bundle = $"evidence/{Bundle}";

        string? Text(string name) => File.Exists(Path.Combine(directory, name)) ? File.ReadAllText(Path.Combine(directory, name)) : null;

        var manifest = Text("manifest.txt");

        if (manifest is null || !Regex.IsMatch(manifest, @"(?m)^commit [0-9a-f]{40}$") || !manifest.Contains("\ntree clean\n", StringComparison.Ordinal))
        {
            blockers.Add(new("G9-no-exact-commit", 9, $"{bundle}/manifest.txt", "no release-candidate run is retained from an exact commit and a clean tree"));
        }

        foreach (var run in Runs)
        {
            var limits = Text($"limits-{run}.log");

            if (limits is null || Regex.Matches(limits, @"(?m)^limit\|").Count == 0 || !limits.Contains("exit code: 0", StringComparison.Ordinal))
            {
                blockers.Add(new($"G9-{run}-no-limit-vector", 9, $"{bundle}/limits-{run}.log", $"run {run} publishes no effective limit vector"));
            }

            if (Text($"{run}.failures.txt") is null || !File.Exists(Path.Combine(directory, $"{run}.report.gz")))
            {
                blockers.Add(new($"G9-{run}-not-retained", 9, $"{bundle}/{run}.report.gz", $"run {run} has no retained report or failure manifest"));
            }

            var log = Text($"{run}.run.log");
            var exit = log is null ? null : Regex.Match(log, @"exit code: (?<code>[0-9]+)\s*$");

            // RETAINABLE IS THE RUNNER'S OWN WORD, and a crossed or uncomparable floor is not a reason
            // to discard the run: its exit code 4 is read by the ratchet check below.
            if (log is null || exit is null || !exit.Success || exit.Groups["code"].Value is "2" or "3" ||
                !log.Contains("# this run may be retained: pinned, whole, and its verdicts account for it", StringComparison.Ordinal))
            {
                blockers.Add(new($"G9-{run}-not-retainable", 9, $"{bundle}/{run}.run.log", $"run {run} did not end as a retainable whole run"));
            }

            var floor = Text($"{run}.floor.log");

            if (FlooredRuns.Contains(run) && (floor is null || !floor.Contains("the test262 floor holds", StringComparison.Ordinal)))
            {
                blockers.Add(new($"G9-{run}-ratchet", 9, $"{bundle}/{run}.floor.log", $"run {run}'s floor does not hold, or could not be compared with it"));
            }
        }

        if (Section(supportTable, "## 3.").Contains('%'))
        {
            blockers.Add(new("G9-aggregate-percentage", 9, "support.md section 3", "the support table publishes a percentage"));
        }

        return blockers;
    }

    private static List<Blocker> NoticeBlockers(string notices)
    {
        var blockers = new List<Blocker>();
        var start = notices.IndexOf("| Component | What it ingests | Confirmed |", StringComparison.Ordinal);
        var named = new List<string>();

        foreach (var line in notices[Math.Max(0, start)..].Split('\n').Skip(2))
        {
            if (!line.StartsWith('|'))
            {
                break;
            }

            var cells = line.Trim().Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (!cells[0].Contains("JavaScript", StringComparison.Ordinal))
            {
                continue;
            }

            named.AddRange(Regex.Matches(cells[0], "`([^`]+)`").Select(static m => m.Groups[1].Value));

            if (cells[^1].Contains("not yet confirmed", StringComparison.Ordinal))
            {
                blockers.Add(new("G12-unconfirmed-" + Slug(cells[0]), 12, $"THIRD_PARTY_NOTICES.md, the ingesting-components row {cells[0]}", "its scoping is not confirmed; confirming it is a release-facing statement the release owner co-signs"));
            }
        }

        foreach (var family in new[] { "Broiler.VM.Profile.JavaScript.Intl" })
        {
            if (!named.Contains(family, StringComparer.Ordinal))
            {
                blockers.Add(new("G12-no-row-" + Slug(family), 12, "THIRD_PARTY_NOTICES.md, the ingesting-components table", $"no row names {family}, which ships tables derived from CLDR and tzdb"));
            }
        }

        return blockers;
    }

    private static GateState Gate(int number, string name, string[] heldBy, List<Blocker> blockers)
    {
        foreach (var id in heldBy)
        {
            var row = RuleRegisterTests.Loaded.Rules.SingleOrDefault(rule => string.Equals(rule.Id, id, StringComparison.Ordinal));

            if (row is null || !string.Equals(row.Status, "Active", StringComparison.Ordinal))
            {
                blockers.Add(new($"G{number}-rule-{id}-not-active", number, "rules.register.json, the row " + id, $"rule {id}, which this gate is held by, is not Active"));
            }
        }

        return new GateState(number, name, heldBy, blockers);
    }

    private static Blocker Unread(int gate, string what) =>
        new($"G{gate}-unread", gate, $"roadmap.gates.md section 22, gate {gate}", what + ", which this check does not read");

    private static string Section(string document, string heading)
    {
        var start = document.IndexOf("\n" + heading, StringComparison.Ordinal);

        if (start < 0)
        {
            return string.Empty;
        }

        var end = document.IndexOf("\n## ", start + 1, StringComparison.Ordinal);
        return end < 0 ? document[start..] : document[start..end];
    }

    private static string Slug(string text) =>
        Regex.Replace(Regex.Replace(text.ToLowerInvariant(), "[`*]", string.Empty), "[^a-z0-9.]+", "-").Trim('-');

    /// <summary>The gate's report: the verdict, then each gate and each blocker.</summary>
    internal static string Report(IReadOnlyList<GateState> gates)
    {
        var text = new System.Text.StringBuilder();
        text.Append("# The JavaScript profile's release gate\n");
        text.Append("verdict ").Append(Verdict(gates)).Append('\n');

        foreach (var gate in gates)
        {
            text.Append($"gate {gate.Gate} {gate.Name}: {(gate.Blockers.Count == 0 ? "held" : "blocked")}");
            text.Append(gate.HeldBy.Count == 0 ? string.Empty : " - held by " + string.Join(", ", gate.HeldBy)).Append('\n');

            foreach (var blocker in gate.Blockers)
            {
                text.Append($"  blocker {blocker.Id}: {blocker.What} ({blocker.Declaration})\n");
            }
        }

        return text.ToString();
    }
}
