// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>One member of the machine-readable report: its name, its type, and what it says.</summary>
/// <param name="Name">The JSON member name. The renderer and the schema both come from this list.</param>
/// <param name="Type">The JSON type, in the words a JSON Schema uses.</param>
/// <param name="What">One sentence a reader of the schema alone can act on.</param>
internal sealed record Test262JsonMember(string Name, string Type, string What);

/// <summary>
/// The whole-run report as one JSON document, beside the line-oriented one and saying the same
/// things.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a second document rather than a second reader.</b> The text report is the document this
/// harness's own merge reads, and it is line-oriented on purpose: a versioned header, one row per
/// fact, a hand-written reader that refuses a line it does not recognise. What it is not is a shape
/// anything OUTSIDE this repository can consume without writing that reader again - and the run it
/// records is the one figure the workload roadmap asks other tools to read. Every consumer that has
/// wanted the totals so far has cut them out of a transcript with a regular expression, which is a
/// reader nobody wrote down and nobody versioned.
/// </para>
/// <para>
/// <b>The schema is a document this harness holds, and both it and the report are built from one
/// list.</b> <see cref="Members"/> is that list: the renderer emits its members in its order and
/// refuses a member it has no section for, and <see cref="Schema"/> is generated from the same
/// entries. A schema written beside a renderer is a schema that drifts from it by the second commit;
/// one generated from the renderer's own table cannot, and <c>--json-schema</c> prints it so a
/// consumer can read the contract without reading this file.
/// </para>
/// <para>
/// <b>Nothing here is timed, dated or measured.</b> The document carries no timestamp, no duration
/// and no rate: two renderings of one run are byte-identical, which is what makes a diff of two runs
/// a diff of the engine rather than of the clock, and roadmap section 14 forbids an aggregate
/// conformance percentage - so every figure in it is a COUNT the report already derived from its
/// rows. This file computes nothing of its own.
/// </para>
/// <para>
/// <b>It says whether the run may be retained, in a field rather than in prose.</b> A run taken
/// without <c>--expect</c>, a run that is one shard, and a run whose narrowings make it partial all
/// produce a transcript somebody can copy into a bundle; <c>retention.retainable</c> is the field
/// that says they must not, and it names its reasons.
/// </para>
/// </remarks>
internal static class Test262Json
{
    /// <summary>What this document calls itself, in every copy of it.</summary>
    internal const string SchemaId = "broiler-js-conformance/test262-report";

    /// <summary>
    /// The version of THIS document's shape, which moves when a member is added or removed.
    /// </summary>
    /// <remarks>
    /// Separate from the text report's version, because the two documents are versioned by different
    /// things: the text one moves when a ROW changes, and a consumer of this one does not care. Both
    /// numbers are in the document, so a consumer can tell which pair of formats produced it.
    /// </remarks>
    internal const int SchemaVersion = 1;

    /// <summary>Every member of the document, in the order it is written.</summary>
    internal static IReadOnlyList<Test262JsonMember> Members { get; } =
    [
        new("schema", "string", "The constant `" + SchemaId + "`, so a reader can tell this document from another."),
        new("schemaVersion", "integer", "The version of this document's own shape; it moves when a member is added or removed."),
        new("reportFormat", "string", "The header line of the line-oriented report format this document was rendered from, which carries that format's own version."),
        new("edition", "object", "The language edition the manifests were defined against: standard, year, source, revision, document, digest and whether that document is archived here."),
        new("suite", "object", "The pin: the suite's name, whether it was pinned at all, the upstream project and commit the retained pin names, and the content digest the run computed and matched."),
        new("manifest", "object", "The feature manifest the run was taken under: its id, the artifact format version, whether the suite's harness prelude was loaded, and the admitted and declined surfaces."),
        new("allowance", "object", "The two ceilings one variant was given: fuel per variant and wall clock in milliseconds per variant."),
        new("partition", "object", "How the selection was cut into processes: the partition rule, this report's shard index (-1 for a whole run) and the shard count."),
        new("selection", "object", "What the run chose to score: candidate files, selected files, the files and variants actually reported, and every recorded narrowing."),
        new("coverage", "object", "Whether the run covers its whole selection, and every reason it does not."),
        new("retention", "object", "Whether this run's figures may be written into a record that outlives it, and every reason they may not."),
        new("totals", "object", "The five verdicts over the variants, with the file and variant counts and whether the five account for every variant."),
        new("unsupportedFamilies", "array", "Every construct family the manifest declined, with a count and one example variant."),
        new("exhaustedDimensions", "array", "Every budget dimension an allowance ran out on, with a count and one example variant."),
        new("exhausted", "array", "Every variant that spent an allowance, named individually with the dimension it spent."),
        new("failureClassification", "object", "The failing variants grouped three ways - by the kind of failure, by the area of the suite, and by the features their files declare."),
        new("failures", "array", "One record per failing variant: its path, variant, failure kind, area, declared features and the detail line the harness wrote."),
        new("findings", "array", "Every configuration failure this report carries, by name and with its detail."),
    ];

    /// <summary>The document, rendered.</summary>
    internal static string Render(Test262Report report)
    {
        var text = new StringBuilder();
        text.Append("{\n");

        for (var index = 0; index < Members.Count; index++)
        {
            text.Append("  ").Append(Quote(Members[index].Name)).Append(": ");
            text.Append(Section(Members[index].Name, report));
            text.Append(index == Members.Count - 1 ? "\n" : ",\n");
        }

        text.Append("}\n");
        return text.ToString();
    }

    /// <summary>The value one member carries.</summary>
    /// <remarks>
    /// <b>A member this switch has no arm for throws rather than rendering nothing.</b> The list and
    /// the renderer are meant to be one thing; a member added to the list and forgotten here would
    /// otherwise produce a document the schema promises a field the document does not have, which is
    /// the exact drift generating the schema from the list was meant to make impossible.
    /// </remarks>
    private static string Section(string member, Test262Report report)
    {
        var totals = report.Totals;

        switch (member)
        {
            case "schema":
                return Quote(SchemaId);

            case "schemaVersion":
                return Number(SchemaVersion);

            case "reportFormat":
                return Quote(Test262Report.Header);

            case "edition":
                return Object(
                    ("standard", Quote(JavaScriptLanguageEdition.Standard)),
                    ("year", Quote(JavaScriptLanguageEdition.Year)),
                    ("source", Quote(JavaScriptLanguageEdition.Source)),
                    ("revision", Quote(JavaScriptLanguageEdition.Revision)),
                    ("document", Quote(JavaScriptLanguageEdition.Document)),
                    ("documentDigest", Quote(JavaScriptLanguageEdition.DocumentDigest)),
                    ("archived", Bool(JavaScriptLanguageEdition.Archived)));

            case "suite":
                return Object(
                    ("name", Quote(report.Suite.Name)),
                    ("pinned", Bool(report.Suite.IsPinned)),
                    ("upstream", Quote(report.Upstream)),
                    ("upstreamRevision", Quote(report.UpstreamRevision)),
                    ("contentDigest", Quote(report.Suite.Revision)));

            case "manifest":
                return Object(
                    ("id", Quote(report.ManifestId)),
                    ("formatVersion", Number((int)report.FormatVersion)),
                    ("loadsHarness", Bool(report.LoadsHarness)),
                    ("admitted", Strings(report.Admitted)),
                    ("declined", Strings(report.Declined)));

            case "allowance":
                return Object(
                    ("fuelPerVariant", Quote(report.Fuel.ToString(CultureInfo.InvariantCulture))),
                    ("wallClockMsPerVariant", Quote(report.WallClock.ToString(CultureInfo.InvariantCulture))));

            case "partition":
                return Object(
                    ("rule", Quote(report.Partition)),
                    ("shardIndex", Number(report.ShardIndex)),
                    ("shardCount", Number(report.ShardCount)));

            case "selection":
                return Object(
                    ("candidateFiles", Number(report.Candidates)),
                    ("selectedFiles", Number(report.Selected)),
                    ("reportedFiles", Number(report.Files)),
                    ("reportedVariants", Number(report.Results.Count)),
                    ("narrowings", Strings(report.Narrowings)));

            case "coverage":
                return Object(
                    ("whole", Bool(report.IsWhole)),
                    ("field", Quote(report.Coverage)),
                    ("reasons", Strings(report.Incompleteness)));

            case "retention":
                return Object(
                    ("retainable", Bool(report.IsRetainable)),
                    ("reasons", Strings(report.Unretainable)));

            case "totals":
                return Object(
                    ("files", Number(totals.Files)),
                    ("variants", Number(totals.Variants)),
                    ("passed", Number(totals.Passed)),
                    ("failed", Number(totals.Failed)),
                    ("unsupported", Number(totals.Unsupported)),
                    ("exhausted", Number(totals.Exhausted)),
                    ("skipped", Number(totals.Skipped)),
                    ("accountsForEveryVariant", Bool(totals.Accounts)));

            case "unsupportedFamilies":
                return Tally(report.Families);

            case "exhaustedDimensions":
                return Tally(report.Exhaustions);

            case "exhausted":
                return Array(report.Exhausted.Select(static spent => Object(
                    ("path", Quote(spent.Path)),
                    ("variant", Quote(spent.Variant)),
                    ("dimension", Quote(spent.Dimension)),
                    ("detail", Quote(spent.Detail)))));

            case "failureClassification":
                return Object(
                    ("failingVariants", Number(totals.Failed)),
                    ("variantsDeclaringAFeature", Number(Test262Failures.WithAFeature(report.Results))),
                    ("areaSegments", Number(Test262Failures.AreaSegments)),
                    ("byKind", Tally(report.FailureKinds)),
                    ("byArea", Tally(report.FailureAreas)),
                    ("byFeature", Tally(report.FailureFeatures)));

            case "failures":
                return Array(report.Failures.Select(static failure => Object(
                    ("path", Quote(failure.Path)),
                    ("variant", Quote(failure.Variant)),
                    ("kind", Quote(failure.Kind.Length == 0 ? Test262Failures.Unclassified : failure.Kind)),
                    ("area", Quote(Test262Failures.Area(failure.Path))),
                    ("features", Strings(Test262Failures.Features(failure.Features))),
                    ("detail", Quote(failure.Detail)))));

            case "findings":
                return Array(report.Findings.Select(static finding => Object(
                    ("failure", Quote(finding.Failure.ToString())),
                    ("detail", Quote(finding.Detail)))));

            default:
                throw new InvalidOperationException(
                    $"`{member}` is declared in the JSON report's member list and this renderer has " +
                    "no section for it");
        }
    }

    /// <summary>The schema, generated from the same list the renderer walks.</summary>
    internal static string Schema()
    {
        var text = new StringBuilder();
        text.Append("{\n");
        text.Append("  \"$schema\": \"https://json-schema.org/draft/2020-12/schema\",\n");
        text.Append("  \"$id\": ").Append(Quote(SchemaId)).Append(",\n");
        text.Append("  \"title\": \"A broiler-js-conformance whole-run test262 report\",\n");

        text.Append("  \"description\": ").Append(Quote(
                "One --test262 run of a pinned checkout under one named feature manifest. Every " +
                "figure is a count the harness derived from its own per-variant rows; there is no " +
                "rate, no duration and no timestamp here, so two renderings of one run are " +
                "byte-identical. A document whose `retention.retainable` is false records a run " +
                "whose identity or coverage was never established and must not be cited."))
            .Append(",\n");

        text.Append("  \"type\": \"object\",\n");
        text.Append("  \"additionalProperties\": false,\n");
        text.Append("  \"required\": ")
            .Append(Strings(Members.Select(static member => member.Name).ToArray()))
            .Append(",\n");

        text.Append("  \"properties\": {\n");

        for (var index = 0; index < Members.Count; index++)
        {
            var member = Members[index];
            text.Append("    ").Append(Quote(member.Name)).Append(": ");
            text.Append(Object(("type", Quote(member.Type)), ("description", Quote(member.What))));
            text.Append(index == Members.Count - 1 ? "\n" : ",\n");
        }

        text.Append("  }\n");
        text.Append("}\n");
        return text.ToString();
    }

    /// <summary>A tally as an array of named counts with one example each.</summary>
    private static string Tally(IReadOnlyList<Test262TallyRow> rows) => Array(rows.Select(
        static row => Object(
            ("name", Quote(row.Name)),
            ("count", Number(row.Count)),
            ("example", Quote(row.Example)))));

    private static string Object(params (string Name, string Value)[] members) =>
        "{" + string.Join(", ", members.Select(static member => Quote(member.Name) + ": " + member.Value)) + "}";

    private static string Array(IEnumerable<string> values)
    {
        var rendered = values.ToArray();

        return rendered.Length == 0 ? "[]" : "[\n    " + string.Join(",\n    ", rendered) + "\n  ]";
    }

    private static string Strings(IReadOnlyList<string> values) =>
        "[" + string.Join(", ", values.Select(Quote)) + "]";

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Bool(bool value) => value ? "true" : "false";

    /// <summary>A JSON string, escaped the way the grammar requires and not the way it usually is.</summary>
    /// <remarks>
    /// <b>Every control character below U+0020 is escaped, and the lone surrogates a JavaScript
    /// String may legally contain are replaced.</b> A test262 file may name an export with an
    /// unpaired surrogate and the front end quotes that name back in its refusal, which is exactly
    /// the case that once discarded a whole shard's totals when a throwing UTF-8 encoder met it. A
    /// document that cannot be written has recorded nothing, so this substitutes where the
    /// transcript encoder substitutes and for the same reason.
    /// </remarks>
    private static string Quote(string value)
    {
        var text = new StringBuilder(value.Length + 2);
        text.Append('"');

        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];

            switch (character)
            {
                case '"':
                    text.Append("\\\"");
                    break;
                case '\\':
                    text.Append("\\\\");
                    break;
                case '\n':
                    text.Append("\\n");
                    break;
                case '\r':
                    text.Append("\\r");
                    break;
                case '\t':
                    text.Append("\\t");
                    break;
                default:
                    if (character < ' ')
                    {
                        text.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else if (char.IsHighSurrogate(character) &&
                        index + 1 < value.Length &&
                        char.IsLowSurrogate(value[index + 1]))
                    {
                        // A WELL-FORMED PAIR IS TWO CODE UNITS AND ONE CHARACTER, copied through
                        // together. Testing each unit on its own would call the trailing half of
                        // every astral character a lone surrogate and replace it.
                        text.Append(character).Append(value[index + 1]);
                        index++;
                    }
                    else if (char.IsSurrogate(character))
                    {
                        text.Append('\uFFFD');
                    }
                    else
                    {
                        text.Append(character);
                    }

                    break;
            }
        }

        text.Append('"');
        return text.ToString();
    }
}
