// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The engine-level vectors run's own checks (<see cref="VectorsCommand"/>): which vectors it evaluates, and what
/// its summary says of those it does not.
/// </summary>
/// <remarks>
/// <para>
/// <b>No profile is composed here.</b> The run is given a stand-in evaluator that answers each script with the
/// outcome its vector expects and records every script it is asked, so a check fails for one reason only: the run
/// chose differently which vectors to evaluate, or said differently what it skipped.
/// </para>
/// <para>
/// <b>Why a status vector is skipped.</b> The corpus's SCHEMA.md section 4 has an engine-level runner skip a vector
/// whose <c>out</c> is a library-level status and give it no line; corpus revision 31's regular-expression budget
/// vectors are the first such, <c>status: exhausted</c>, which no engine's evaluation can render and no
/// <c>divergence</c> field can classify. <i>(Added 2026-11-01, at SJL-11.3, before this root's pin moves past corpus
/// revision 30.)</i>
/// </para>
/// </remarks>
internal static class VectorsChecks
{
    /// <summary>Runs every check over the vectors run.</summary>
    internal static IReadOnlyList<(string Name, bool Passed, string Detail)> Run() =>
    [
        AStatusVectorIsSkippedCountedAndNamed(),
        AnOutcomeIsAStatusOnlyWhenAnAlternativeIsOne(),
    ];

    private const string Corpus =
        "# a check's corpus\n" +
        "schema: 1\n" +
        "\n" +
        "id: regexp/check/0001\nclass: normative\nout: value: bool:true\nscript: first\n" +
        "\n" +
        "id: regexp/check/0002\nclass: normative\nout: status: exhausted\nscript: second\n" +
        "\n" +
        "id: regexp/check/0003\nclass: normative\nout: value: bool:false\nscript: third\n" +
        "\n" +
        "id: regexp/check/0004\nclass: normative\nout: status: exhausted\nscript: fourth\n" +
        "\n";

    private static (string, bool, string) AStatusVectorIsSkippedCountedAndNamed()
    {
        const string name = "a vector whose out is a status is skipped, gets no report line, and the summary counts and names it";
        var vectors = new List<VectorsCommand.Vector>();

        if (!VectorsCommand.Read(Corpus, "check.vectors", vectors, out var failure))
        {
            return (name, false, "the check's corpus does not read: " + failure);
        }

        // Non-vacuous: the corpus read holds both kinds of vector, so a run that evaluated everything or nothing
        // would be told apart from one that skips the status vectors only.
        if (vectors.Count != 4)
        {
            return (name, false, $"the check's corpus read as {vectors.Count} vectors, not 4");
        }

        var answers = vectors.ToDictionary(static v => v.Script, static v => v.Out, StringComparer.Ordinal);
        var asked = new List<string>();
        var run = VectorsCommand.RunVectors(vectors, script => { asked.Add(script); return answers[script]; }, verbose: false);
        var summary = VectorsCommand.Summary(run);
        string[] skipped = ["regexp/check/0002", "regexp/check/0004"];

        var passed =
            asked.SequenceEqual(["first", "third"], StringComparer.Ordinal) &&
            run.Skipped.SequenceEqual(skipped, StringComparer.Ordinal) &&
            run.Evaluated == 2 &&
            run.Lines.Count == 2 &&
            !run.Lines.Any(line => skipped.Any(id => line.StartsWith(id + "\t", StringComparison.Ordinal))) &&
            run.Counts.GetValueOrDefault("match") == 2 &&
            run.Counts.GetValueOrDefault("difference") == 0 &&
            summary.Contains("2 vectors whose out is a status skipped", StringComparison.Ordinal) &&
            skipped.All(id => summary.Contains(id, StringComparison.Ordinal));

        return (name, passed,
            $"asked [{string.Join(", ", asked)}]; skipped [{string.Join(", ", run.Skipped)}]; " +
            $"{run.Evaluated} evaluated, {run.Lines.Count} lines; summary: {summary}");
    }

    private static (string, bool, string) AnOutcomeIsAStatusOnlyWhenAnAlternativeIsOne()
    {
        const string name = "an outcome is a status when one of its alternatives is, and a string holding the word is not";
        (string Outcome, bool Status)[] cases =
        [
            ("status: exhausted", true),
            ("status: not-implemented", true),
            ("value: bool:true | status: exhausted", true),
            ("value: str:\"status: exhausted\"", false),
            ("error: SyntaxError", false),
            ("value: undefined", false),
        ];

        var wrong = cases.Where(static c => VectorsCommand.IsStatus(c.Outcome) != c.Status).ToArray();

        return (name, wrong.Length == 0,
            wrong.Length == 0 ? $"{cases.Length} outcomes read" : "misread: " + string.Join("; ", wrong.Select(static c => c.Outcome)));
    }
}
