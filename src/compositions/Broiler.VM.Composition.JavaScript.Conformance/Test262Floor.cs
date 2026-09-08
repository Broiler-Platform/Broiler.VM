// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>One figure a later whole run may not cross, and which way it may not cross it.</summary>
/// <param name="Field">The total this row is about, from <see cref="Test262Floor.Fields"/>.</param>
/// <param name="AtLeast">
/// Whether the run must be at or above <paramref name="Value"/>; otherwise it must be at or below.
/// </param>
/// <param name="Value">The figure the admitted run reported.</param>
internal sealed record Test262FloorRow(string Field, bool AtLeast, int Value);

/// <summary>
/// The ratchet for a WHOLE-SUITE <c>--test262</c> run: the totals a later run of the same revision
/// under the same manifest may not cross.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this exists, stated as the gap it closes.</b> <see cref="Floor"/> ratchets the
/// <c>--run</c> mode's report, and the floor this repository retains under it covers the
/// ingested-dialect path - about a thousand cases of the fifty-six thousand files. The whole-suite
/// figure the workload roadmap actually cites, the one produced by <c>--test262</c> and a merge, was
/// held to nothing at all: a change that turned seventy thousand passes into sixty thousand would
/// have produced a green run, a smaller number, and no complaint from any tool. This is the floor
/// for that report, and it is a different record from its sibling because the two modes count
/// different things - five verdicts over variants here, four statuses per host mode there.
/// </para>
/// <para>
/// <b>Two directions, because a ratchet in one direction is a ratchet a regression walks around.</b>
/// Passing variants may not FALL, which is the obvious half. Failing, unsupported and skipped
/// variants may not RISE, which is the half that catches the movements a pass count cannot see: a
/// change that stops the front end admitting a construct moves variants from passed to unsupported,
/// and a change that makes a file unreadable moves them to skipped. Files and variants may not fall
/// either, because a selection that quietly stopped reaching part of the tree is the failure the
/// coverage field exists to prevent and this is the second reading of it.
/// </para>
/// <para>
/// <b>Exhaustions are deliberately not ratcheted, and that is a decision rather than an
/// omission.</b> A variant that spent its allowance says what the ceiling was on the machine that
/// took the run - the retained whole run met 21 wall-clock exhaustions and 20 fuel ones out of
/// ninety-four thousand variants - and a busier machine meets more of them without anything about
/// the engine having changed. A floor over that column would fail runs for being taken on a loaded
/// laptop, which teaches a reader to pass <c>--admit</c> until the floor means nothing. The column
/// is in the report, named variant by variant, and a reader acts on it there.
/// </para>
/// <para>
/// <b>A floor records the revision AND the manifest it was set under, and is never compared across
/// either.</b> That is the same discipline <see cref="Floor"/> states for the suite revision, with
/// the manifest added because this mode's totals are a property of the composition: the wide
/// manifest's run and the slice manifest's run over one checkout differ by tens of thousands of
/// variants, and a floor that compared them would read a deliberate configuration as a catastrophe.
/// A change to either re-bases with <c>--admit</c>, and the old floor stays in the file with the
/// reason.
/// </para>
/// <para>
/// <b>A run that may not be retained may not set a floor.</b> A floor is the most durable thing this
/// harness writes, so <see cref="Test262Report.IsRetainable"/> is the gate: no unpinned suite, no
/// single shard, no partial coverage, no arithmetic that does not add up. A floor set from a run
/// nobody can identify is a number with a file around it.
/// </para>
/// </remarks>
internal sealed record Test262Floor(
    SuiteRevision Suite,
    string ManifestId,
    IReadOnlyList<Test262FloorRow> Rows,
    IReadOnlyList<string> Retired)
{
    /// <summary>The header a whole-run floor file carries.</summary>
    internal const string Header = "# broiler-js-conformance test262 floor 1";

    /// <summary>Every field a floor holds, and which way each of them may not move.</summary>
    /// <remarks>
    /// A closed list, so that a floor file naming a field this build does not know is refused rather
    /// than ignored - an ignored row is a ratchet somebody can delete a line from.
    /// </remarks>
    internal static IReadOnlyList<(string Field, bool AtLeast)> Fields { get; } =
    [
        ("files", true),
        ("variants", true),
        ("passed", true),
        ("failed", false),
        ("unsupported", false),
        ("skipped", false),
    ];

    /// <summary>What one report says for one field.</summary>
    internal static int Value(Test262Report report, string field)
    {
        var totals = report.Totals;

        return field switch
        {
            "files" => totals.Files,
            "variants" => totals.Variants,
            "passed" => totals.Passed,
            "failed" => totals.Failed,
            "unsupported" => totals.Unsupported,
            "skipped" => totals.Skipped,
            _ => throw new InvalidOperationException($"`{field}` is not a field a test262 floor holds"),
        };
    }

    /// <summary>Whether this run may be admitted as a floor at all.</summary>
    internal static bool Admissible(Test262Report report, out string why)
    {
        var reasons = report.Unretainable;

        why = reasons.Count == 0 ? string.Empty : string.Join("; ", reasons);
        return reasons.Count == 0;
    }

    /// <summary>The floor a run amounts to.</summary>
    internal static Test262Floor From(Test262Report report) => new(
        report.Suite,
        report.ManifestId,
        Fields.Select(field => new Test262FloorRow(field.Field, field.AtLeast, Value(report, field.Field))).ToArray(),
        []);

    /// <summary>Compares a run against this floor.</summary>
    internal Floor.Verdict Compare(Test262Report report, out IReadOnlyList<string> complaints)
    {
        if (!string.Equals(Suite.Name, report.Suite.Name, StringComparison.Ordinal) ||
            !string.Equals(Suite.Revision, report.Suite.Revision, StringComparison.Ordinal))
        {
            complaints =
            [
                $"the floor was set under {Suite} and this run read {report.Suite}: re-basing, " +
                "because a floor compared across revisions reads an added test as a regression",
            ];

            return Floor.Verdict.Rebased;
        }

        if (!string.Equals(ManifestId, report.ManifestId, StringComparison.Ordinal))
        {
            complaints =
            [
                $"the floor was set under manifest `{ManifestId}` and this run was taken under " +
                $"`{report.ManifestId}`: re-basing, because two manifests' totals are two runs",
            ];

            return Floor.Verdict.Rebased;
        }

        var found = new List<string>();

        foreach (var row in Rows)
        {
            var observed = Value(report, row.Field);

            if (row.AtLeast ? observed < row.Value : observed > row.Value)
            {
                found.Add(
                    $"`{row.Field}` is {Number(observed)} and the floor is " +
                    (row.AtLeast ? "at least " : "at most ") + Number(row.Value));
            }
        }

        complaints = found;
        return found.Count == 0 ? Floor.Verdict.Held : Floor.Verdict.Regressed;
    }

    /// <summary>Renders a floor.</summary>
    internal string Render()
    {
        var text = new StringBuilder();
        text.Append(Header).Append('\n');
        text.Append("# The whole-suite --test262 ratchet. A later run of THIS revision under THIS\n");
        text.Append("# manifest may not cross a row below. `atLeast` may not fall; `atMost` may not\n");
        text.Append("# rise. Exhaustions are deliberately not held: a spent allowance says what the\n");
        text.Append("# machine was doing, not what the engine does. A revision or manifest change\n");
        text.Append("# re-bases with --admit and retires the old rows here with their reason.\n");
        text.Append("suite ").Append(Suite.Name).Append('\n');
        text.Append("revision ").Append(Suite.IsPinned ? Suite.Revision : "unpinned").Append('\n');
        text.Append("manifest ").Append(ManifestId).Append('\n');

        foreach (var field in Fields)
        {
            var row = Rows.FirstOrDefault(candidate =>
                string.Equals(candidate.Field, field.Field, StringComparison.Ordinal));

            if (row is null)
            {
                continue;
            }

            text.Append(row.AtLeast ? "atLeast " : "atMost ")
                .Append(row.Field)
                .Append(' ')
                .Append(Number(row.Value))
                .Append('\n');
        }

        foreach (var retired in Retired)
        {
            text.Append("retired ").Append(retired.Replace('\n', ' ')).Append('\n');
        }

        return text.ToString();
    }

    /// <summary>Reads a floor back, or says the file is not one.</summary>
    internal static Test262Floor Read(string path)
    {
        var name = "unnamed";
        var revision = string.Empty;
        var manifest = Test262Manifest.Default;
        var rows = new List<Test262FloorRow>();
        var retired = new List<string>();
        var seenHeader = false;

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            switch (parts[0])
            {
                case "suite" when parts.Length == 2:
                    name = parts[1];
                    break;

                case "revision" when parts.Length == 2:
                    revision = string.Equals(parts[1], "unpinned", StringComparison.Ordinal)
                        ? string.Empty
                        : parts[1];

                    break;

                case "manifest" when parts.Length == 2:
                    manifest = parts[1];
                    break;

                case "atLeast" when parts.Length == 3:
                case "atMost" when parts.Length == 3:
                    rows.Add(Row(path, parts[0], parts[1], parts[2]));
                    break;

                case "retired":
                    retired.Add(line["retired ".Length..]);
                    break;

                default:
                    throw new InvalidOperationException($"{path}: `{line}` is not a test262 floor line");
            }
        }

        if (!seenHeader)
        {
            throw new InvalidOperationException($"{path} does not open with `{Header}`");
        }

        return new Test262Floor(new SuiteRevision(name, revision), manifest, rows, retired);
    }

    /// <summary>The floor a re-base produces: the new run's rows, with the old floor retained.</summary>
    internal Test262Floor Rebase(Test262Report report, string reason) => From(report) with
    {
        Retired =
        [
            .. Retired,
            $"{Suite} {ManifestId} " +
                string.Join(
                    "; ",
                    Rows.Select(static row =>
                        (row.AtLeast ? "atLeast " : "atMost ") + row.Field + " " + Number(row.Value))) +
                " :: " + reason,
        ],
    };

    /// <summary>
    /// One row, with the direction checked against the closed list rather than taken from the file.
    /// </summary>
    /// <remarks>
    /// <b>Which way a field ratchets is this build's decision and not the file's.</b> A floor whose
    /// <c>failed</c> row said <c>atLeast</c> would demand that a later run fail at least as often,
    /// which is a ratchet pointing at the floor - and it would be a one-word edit. The field's name
    /// is read from the file; its direction is looked up here and a disagreement is refused.
    /// </remarks>
    private static Test262FloorRow Row(string path, string direction, string field, string value)
    {
        var declared = Fields.FirstOrDefault(candidate =>
            string.Equals(candidate.Field, field, StringComparison.Ordinal));

        if (declared.Field is null)
        {
            throw new InvalidOperationException(
                $"{path}: `{field}` is not a field a test262 floor holds");
        }

        var atLeast = string.Equals(direction, "atLeast", StringComparison.Ordinal);

        if (atLeast != declared.AtLeast)
        {
            throw new InvalidOperationException(
                $"{path}: `{field}` is written `{direction}` and this build ratchets it " +
                (declared.AtLeast ? "`atLeast`" : "`atMost`"));
        }

        return new Test262FloorRow(field, atLeast, int.Parse(value, CultureInfo.InvariantCulture));
    }

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>Compares a merged whole run against a floor, or admits it as one.</summary>
    /// <remarks>
    /// <b>The same four outcomes the sibling ratchet has, and the same rule about <c>--admit</c>.</b>
    /// A floor that did not exist is set only where a caller asked for it; a revision or manifest
    /// that moved re-bases only where a caller asked for it; a run below the floor fails and no flag
    /// changes that. The one addition is the admissibility gate, which refuses to write a floor from
    /// a run whose identity or coverage was never established - the case the sibling could not have,
    /// because its report cannot be taken without a suite.
    /// </remarks>
    internal static int Ratchet(string floorPath, string reportPath, bool admit)
    {
        var run = Test262Report.Read(reportPath);

        if (!Admissible(run, out var why))
        {
            Console.WriteLine(
                "broiler-js-conformance: this run may not be compared to a floor: " + why);

            return ExitCodes.Failed;
        }

        if (!File.Exists(floorPath))
        {
            if (!admit)
            {
                Console.WriteLine(
                    $"broiler-js-conformance: {floorPath} holds no floor; pass --admit to set one " +
                    "from this run");

                return ExitCodes.Failed;
            }

            File.WriteAllText(floorPath, From(run).Render());

            Console.WriteLine(
                $"broiler-js-conformance: test262 floor set from {run.Suite} under {run.ManifestId}: " +
                run.Totals.Describe()[2..]);

            return ExitCodes.Ok;
        }

        var floor = Read(floorPath);
        var verdict = floor.Compare(run, out var complaints);

        foreach (var complaint in complaints)
        {
            Console.WriteLine((verdict == Floor.Verdict.Regressed ? "FAIL " : "note ") + complaint);
        }

        switch (verdict)
        {
            case Floor.Verdict.Held:
                Console.WriteLine(
                    $"broiler-js-conformance: the test262 floor holds at {floor.Suite} under " +
                    $"{floor.ManifestId}: " + run.Totals.Describe()[2..]);

                return ExitCodes.Ok;

            case Floor.Verdict.Regressed:
                Console.WriteLine(
                    $"broiler-js-conformance: {complaints.Count} total(s) crossed the test262 floor");

                return ExitCodes.Failed;

            default:
                if (!admit)
                {
                    Console.WriteLine(
                        "broiler-js-conformance: the suite revision or the manifest moved; pass " +
                        "--admit to re-base the floor onto this run");

                    return ExitCodes.Failed;
                }

                File.WriteAllText(
                    floorPath,
                    floor.Rebase(run, "the suite revision or the manifest moved").Render());

                Console.WriteLine(
                    $"broiler-js-conformance: test262 floor re-based onto {run.Suite} under " +
                    run.ManifestId);

                return ExitCodes.Ok;
        }
    }
}
