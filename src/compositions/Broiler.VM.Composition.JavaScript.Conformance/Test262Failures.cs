// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The vocabulary a failing variant is filed under, and the three axes a run's failures are grouped
/// along.
/// </summary>
/// <remarks>
/// <para>
/// <b>Fourteen thousand failures in one column is a number and not a work list.</b> The whole-suite
/// run has reported its failed total since the day it first ran whole, and that total has never once
/// told a reader which repair would move it: a reader who wants to know what to fix next has to open
/// a hundred-megabyte transcript and read sentences. What turns the column into something a person
/// can act on is a grouping, and this file is the three groupings the suite's own shape offers - the
/// KIND of failure the harness observed, the AREA of the suite the file sits in, and the FEATURE the
/// file declares it is about.
/// </para>
/// <para>
/// <b>The kind is a field on the outcome and not a sentence parsed back out of the detail.</b> That
/// is the same rule <see cref="Test262Outcome.Family"/> and <see cref="Test262Outcome.Dimension"/>
/// were given and for the same reason, stated there: a table recovered by cutting a message at a
/// colon is a table that is a property of how a message happens to be punctuated, and it moves the
/// day somebody rewords a diagnostic. Every place that decides a variant Failed names the kind it is
/// deciding, from the closed set below, and a kind this file does not build is
/// <see cref="Unclassified"/> - which is a row a reader must see rather than a number to round down.
/// </para>
/// <para>
/// <b>The three axes do not sum to each other and the report says so.</b> Kind and area each account
/// for every failing variant exactly once. Feature does not: a test262 file declares zero, one or
/// several <c>features</c>, so a variant appears in as many feature rows as it declared flags and in
/// none at all when it declared no flag. A reader who added the feature column would get a figure
/// larger than the failures, which is why the rendered table carries its own count of the variants it
/// covers rather than inviting the sum.
/// </para>
/// </remarks>
internal static class Test262Failures
{
    /// <summary>What a failing variant is filed under when this file built no kind for it.</summary>
    /// <remarks>
    /// <b>Its own row rather than a silent omission.</b> A verdict of Failed reached from a place
    /// that names no kind is a gap in this vocabulary, and a classification that quietly dropped it
    /// would report a failure table whose rows do not add up to the failed total - which is the one
    /// property that makes the table checkable at all.
    /// </remarks>
    internal const string Unclassified = "(no kind was named)";

    /// <summary>How many leading path segments an area row is cut to.</summary>
    /// <remarks>
    /// <b>Four, uniformly, and it is a choice rather than a discovery.</b> Three would put every
    /// statement in the language under one row - <c>test/language/statements</c> holds classes,
    /// generators and <c>for</c> together - and five would cut <c>test/built-ins/Array/prototype</c>
    /// into one row per method and produce a table longer than the failures are interesting. Four
    /// gives <c>test/built-ins/Array/prototype</c> and <c>test/language/statements/class</c>, which
    /// are the two granularities a repair is actually planned at. It is stated here so that two
    /// transcripts of two runs are cut the same way.
    /// </remarks>
    internal const int AreaSegments = 4;

    /// <summary>The kind an uncaught exception at the top of the test amounts to.</summary>
    internal static string Uncaught(string errorName) => "an uncaught " + ErrorName(errorName);

    /// <summary>The kind a harness prelude file throwing amounts to.</summary>
    /// <remarks>
    /// <b>Kept apart from the test throwing, because the two are different defects.</b> A test that
    /// throws is a test this engine fails; <c>assert.js</c> or <c>propertyHelper.js</c> throwing is
    /// the prelude the whole suite is scored through failing to install, which fails every test that
    /// includes it and is one repair rather than hundreds.
    /// </remarks>
    internal static string HarnessThrew(string errorName) =>
        "a harness file threw " + ErrorName(errorName);

    /// <summary>The kind a job throwing during the drain amounts to.</summary>
    internal static string JobThrew(string errorName) => "a job threw " + ErrorName(errorName);

    /// <summary>The kind a negative test that threw nothing at all amounts to.</summary>
    internal static string NegativeThrewNothing(string phase, string type) =>
        "a declared " + Word(phase) + "-phase " + ErrorName(type) + " was not raised";

    /// <summary>The kind a negative test that threw the wrong error amounts to.</summary>
    internal static string NegativeWrongError(string declared, string thrown) =>
        "a declared " + ErrorName(declared) + " was raised as " + ErrorName(thrown);

    /// <summary>The kind a negative test that threw in the wrong phase amounts to.</summary>
    internal static string NegativeWrongPhase(string phase) =>
        "a declared " + Word(phase) + "-phase error was raised at run time instead";

    /// <summary>The kind an asynchronous test that printed no completion amounts to.</summary>
    internal const string AsyncSilent = "an asynchronous test printed no completion";

    /// <summary>The kind an asynchronous test that printed a failure amounts to.</summary>
    internal const string AsyncReported = "an asynchronous test reported its own failure";

    /// <summary>The kind the front end refusing a source this manifest admits amounts to.</summary>
    /// <remarks>
    /// <b>Not the same thing as <see cref="Test262Verdict.Unsupported"/> and never folded into
    /// it.</b> A refusal carrying <c>ConstructOutsideManifest</c> is the composition declining a
    /// construct by name and is its own verdict; a refusal carrying anything else is this front end
    /// rejecting JavaScript it claims to admit, which is a failure and the most actionable one in
    /// the table.
    /// </remarks>
    internal const string FrontEndRefused = "the front end refused a source this manifest admits";

    /// <summary>The kind the verifier refusing this runner's own artifact amounts to.</summary>
    internal const string VerifierRefused = "the verifier refused the artifact this runner produced";

    /// <summary>The kind an artifact that would not instantiate amounts to.</summary>
    internal const string WouldNotInstantiate = "the artifact would not instantiate";

    /// <summary>The kind an invocation that answered with no payload amounts to.</summary>
    internal const string NoPayload = "the invocation carried no completion payload";

    /// <summary>The kind a module graph this runner could not assemble amounts to.</summary>
    internal const string ModuleGraphUnreadable = "the module graph could not be assembled";

    /// <summary>The area a suite-relative path is filed under.</summary>
    /// <remarks>
    /// The DIRECTORY and never the file, cut to <see cref="AreaSegments"/>. A path shorter than the
    /// cut is its own directory, and a file at the suite root - which the checkout does not have and
    /// a synthetic one might - is filed under <c>(root)</c> rather than under the empty string.
    /// </remarks>
    internal static string Area(string path)
    {
        var segments = Suite.Normalize(path).Split('/');

        if (segments.Length <= 1)
        {
            return "(root)";
        }

        var taken = Math.Min(AreaSegments, segments.Length - 1);
        return string.Join('/', segments.Take(taken));
    }

    /// <summary>The features one outcome declares, from the comma-separated field it carries.</summary>
    internal static IReadOnlyList<string> Features(string features) =>
        features.Length == 0
            ? []
            : features.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    /// <summary>Every failing variant of a run, in the order a report renders them.</summary>
    internal static IReadOnlyList<Test262Outcome> Of(IEnumerable<Test262Outcome> results) => results
        .Where(static result => result.Verdict == Test262Verdict.Failed)
        .OrderBy(static result => result.Path, StringComparer.Ordinal)
        .ThenBy(static result => result.Variant, StringComparer.Ordinal)
        .ToArray();

    /// <summary>The failures grouped by the kind the harness named, most-met first.</summary>
    internal static IReadOnlyList<Test262TallyRow> ByKind(IEnumerable<Test262Outcome> results) =>
        Test262Families.Tally(
            Of(results).Select(static failure => (
                failure.Kind.Length == 0 ? Unclassified : failure.Kind,
                failure.Path + " [" + failure.Variant + "]")));

    /// <summary>The failures grouped by the area of the suite they sit in, most-met first.</summary>
    internal static IReadOnlyList<Test262TallyRow> ByArea(IEnumerable<Test262Outcome> results) =>
        Test262Families.Tally(
            Of(results).Select(static failure => (
                Area(failure.Path), failure.Path + " [" + failure.Variant + "]")));

    /// <summary>The failures grouped by the features the file declares, most-declared first.</summary>
    /// <remarks>
    /// <b>A variant with two features is in two rows and a variant with none is in no row.</b> That
    /// is what a test262 <c>features</c> list is - a set of tags rather than a partition - and the
    /// only alternative that sums to the failures is a row called "several", which answers nothing.
    /// The rendered table names how many variants it covers so a reader is not invited to add it.
    /// </remarks>
    internal static IReadOnlyList<Test262TallyRow> ByFeature(IEnumerable<Test262Outcome> results) =>
        Test262Families.Tally(
            Of(results).SelectMany(static failure => Features(failure.Features)
                .Select(feature => (feature, failure.Path + " [" + failure.Variant + "]"))));

    /// <summary>How many failing variants declared at least one feature.</summary>
    internal static int WithAFeature(IEnumerable<Test262Outcome> results) =>
        Of(results).Count(static failure => failure.Features.Length != 0);

    /// <summary>The ranked summary a person reads off the end of a transcript.</summary>
    /// <remarks>
    /// <b>Ranked and truncated, with the truncation stated.</b> A whole run meets more areas than a
    /// terminal holds, and a table that printed all of them would be a table nobody scrolls to the
    /// end of; the report and the JSON document carry every row, and this prints the head of each
    /// axis with a line saying how many rows it did not print. What it never prints is a rate:
    /// roadmap section 14 forbids an aggregate conformance percentage, and a count is what every
    /// line here is.
    /// </remarks>
    internal static IReadOnlyList<string> Describe(Test262Report report, int head)
    {
        var failures = Of(report.Results);

        if (failures.Count == 0)
        {
            return ["# no variant failed, so there is nothing to classify"];
        }

        var lines = new List<string>
        {
            "# failure classification over " + Count(failures.Count) + " failing variant(s)",
        };

        Axis(lines, "by kind", report.FailureKinds, failures.Count, head, sums: true);
        Axis(lines, "by area", report.FailureAreas, failures.Count, head, sums: true);

        var tagged = WithAFeature(report.Results);

        Axis(
            lines,
            "by declared feature (" + Count(tagged) + " of " + Count(failures.Count) +
                " failing variants declare one; a variant with two is in two rows)",
            report.FailureFeatures,
            tagged,
            head,
            sums: false);

        return lines;
    }

    /// <summary>One axis of the ranked summary.</summary>
    private static void Axis(
        List<string> lines,
        string heading,
        IReadOnlyList<Test262TallyRow> rows,
        int covered,
        int head,
        bool sums)
    {
        var total = rows.Sum(static row => row.Count);

        lines.Add(
            "#   " + heading + ": " + Count(rows.Count) + " distinct, " + Count(total) +
            (sums
                ? " variants" + (total == covered ? string.Empty : " of " + Count(covered) + " - THESE DISAGREE")
                : " taggings over " + Count(covered) + " variants"));

        foreach (var row in rows.Take(head))
        {
            lines.Add(
                "#     " + Count(row.Count).PadLeft(7) + "  " + row.Name + "  e.g. " + row.Example);
        }

        if (rows.Count > head)
        {
            lines.Add(
                "#     " + Count(rows.Count - head).PadLeft(7) +
                "  further row(s) not printed here; every one of them is in the report");
        }
    }

    /// <summary>An error name, or the words for a thrown value that has none.</summary>
    private static string ErrorName(string name) =>
        name.Length == 0 ? "value that is not an Error" : name;

    /// <summary>A phase name, or the words for a negative record that declared none.</summary>
    private static string Word(string phase) => phase.Length == 0 ? "(unnamed)" : phase;

    private static string Count(int value) => value.ToString(CultureInfo.InvariantCulture);
}
