using System.Globalization;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One alias that appears on a human line somewhere in the product tree.</summary>
/// <remarks>
/// The row exists because an alias was READ out of the tree, never because anybody registered it.
/// There is no list of permitted reviewers and no place to add one: an alias is in this table
/// exactly when some declaration's <c>// Broiler-Human:</c> line carries it, which is what lets the
/// record carry several of them without anybody maintaining a roster.
/// </remarks>
internal sealed record AssuranceReviewerRow(string Alias, int Units, int Files, int Current, int Outrun);

/// <summary>
/// <c>HUMAN_REVIEW.md</c>, derived from the annotations and the generated file headers.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this file is generated.</b> The record used to be written by hand, and beside it stood a
/// worksheet of numbered items that cited source files by line number. Both rotted, and one of them
/// rotted invisibly: <c>RC-01</c> cited a checked multiplication at <c>VmBoundedAllocator.cs:57</c>,
/// which the annotations had turned into a parameter declaration, and rule H3 checked identifiers,
/// areas and counts and never whether a citation meant anything. A hand-written record also has one
/// signature line, which is a record with room for one person.
/// </para>
/// <para>
/// So the whole of what a human writes is now the <c>// Broiler-Human:</c> line on the declaration
/// they read, and this file is computed from those lines and from nothing else. Two things follow.
/// A decision travels with its declaration, so there is no citation to rot. And the record has as
/// many aliases as the tree does: <see cref="Reviewers"/> reads them out rather than being told
/// them, so a second and a third person need no edit anywhere to appear here.
/// </para>
/// <para>
/// <b>No commit is recorded.</b> The old record carried a "commit under review" field, which said
/// that a tree had been read and left every reader to work out which declarations in it had since
/// moved. Each decision now names the fingerprint of the declaration it was made against, and the
/// state machine compares that value with the declaration as it stands: a commit says the tree
/// moved, and a fingerprint says whether THIS unit did. The field is not omitted for brevity - it
/// would be a weaker binding than the one every line already carries.
/// </para>
/// <para>
/// <b>What this may say.</b> Everything below is derived from the units the generator is about to
/// leave behind, and rule J9 reads every line of it: a review term stands here only where the
/// annotations give the count stated after it, or behind a negation. Rule J8 holds the fixed
/// sentences to a hand-maintained second copy in <see cref="AssuranceArtefactShape"/>. Neither is a
/// courtesy - this file is the one a reader trusts, and it is written by a machine.
/// </para>
/// </remarks>
internal static class AssuranceHumanReview
{
    internal const string RelativePath = "HUMAN_REVIEW.md";

    /// <summary>
    /// The status word the states give: the aggregate of the per-unit decisions, and never a
    /// judgement of its own.
    /// </summary>
    internal static string Status(IReadOnlyList<AssuranceUnit> units)
    {
        var relevant = units.Count(static unit => unit.IsRelevant);
        var current = units.Count(static unit =>
            unit.IsRelevant && unit.State == AssuranceReviewState.Verified);

        return current == 0 ? "PENDING" : current < relevant ? "PARTIAL" : "COMPLETE";
    }

    /// <summary>
    /// Every alias any human line names, live or preserved by a <c>STALE</c> line, in ordinal order.
    /// </summary>
    /// <remarks>
    /// A <c>STALE</c> line's <c>Previous=</c> alias is counted, in its own column and never in the
    /// current one. Dropping it would lose the one fact that distinguishes a unit nobody has read
    /// from a unit somebody read before it changed, which is the distinction the whole state machine
    /// exists to keep.
    /// </remarks>
    internal static IReadOnlyList<AssuranceReviewerRow> Reviewers(IReadOnlyList<AssuranceUnit> units)
    {
        var claimed = units
            .Where(static unit => unit.Annotation is not null)
            .Select(static unit => (unit, Alias: AliasOn(unit)))
            .Where(static named => named.Alias is not null)
            .Select(static named => (named.unit, Alias: named.Alias!))
            .ToArray();

        return claimed
            .GroupBy(static named => named.Alias, StringComparer.Ordinal)
            .OrderBy(static group => group.Key, StringComparer.Ordinal)
            .Select(static group => new AssuranceReviewerRow(
                Alias: group.Key,
                Units: group.Count(),
                Files: group
                    .Select(static named => named.unit.File.RelativePath)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
                Current: group.Count(static named =>
                    named.unit.State == AssuranceReviewState.Verified),
                Outrun: group.Count(static named =>
                    named.unit.State == AssuranceReviewState.Stale)))
            .ToArray();
    }

    /// <summary>
    /// The alias a unit's human line names: the live one, or the one a <c>STALE</c> line preserves.
    /// </summary>
    internal static string? AliasOn(AssuranceUnit unit) =>
        unit.Annotation?.Reviewer ?? unit.Annotation?.Previous?.Reviewer;

    /// <summary>
    /// Every unit whose human line carries a decision: the live ones and the outrun ones, in the
    /// order the tree declares them.
    /// </summary>
    internal static IReadOnlyList<AssuranceUnit> Decided(IReadOnlyList<AssuranceUnit> units) => units
        .Where(static unit => unit.State is
            AssuranceReviewState.Verified or AssuranceReviewState.HumanApprovedPendingFingerprint)
        .ToArray();

    /// <summary>Every unit carrying a decision the code has since outrun.</summary>
    internal static IReadOnlyList<AssuranceUnit> Outrun(IReadOnlyList<AssuranceUnit> units) => units
        .Where(static unit => unit.State == AssuranceReviewState.Stale)
        .ToArray();

    /// <summary>
    /// Every unit a decision is required on before anything is published: the ones the assessment
    /// puts at the top of the security vocabulary.
    /// </summary>
    /// <remarks>
    /// This is the risk ordering the old record wrote out by hand as eight areas with suggested
    /// times. It is the same claim with one difference that matters: the set is READ from the
    /// assessments, so a unit that becomes <c>High</c> joins it at the next generation and a
    /// hand-written route would not have noticed.
    /// </remarks>
    internal static IReadOnlyList<AssuranceUnit> RequiredFirst(IReadOnlyList<AssuranceUnit> units) => units
        .Where(static unit => unit.Annotation?.Field("Security") is "High" or "Critical")
        .ToArray();

    /// <summary>The human line as the source states it, which is what a reader has to be shown.</summary>
    /// <remarks>
    /// The body and not <see cref="AssuranceStateMachine.Name"/>. A state name is this system's
    /// word for what it made of the line; the body is the line. Rendering the name would also put
    /// the bare word <c>VERIFIED</c> into a list entry that states no count, which rule J9 reports -
    /// correctly, because a term standing alone in generated prose is a claim.
    /// </remarks>
    internal static string HumanLine(AssuranceUnit unit) => unit.Annotation switch
    {
        null => "no annotation",
        { HumanBody.Length: 0 } => "an empty line",
        var annotation => annotation.HumanBody,
    };

    // =============================================================================================
    // The document
    // =============================================================================================

    internal static string Render(
        IReadOnlyList<AssuranceSourceFile> files,
        IReadOnlyList<AssuranceUnit> units)
    {
        var summary = AssuranceSummary.Of(units);
        var record = new StringBuilder();

        record.Append("# Human Review: Broiler.VM\n\n");
        record.Append("GENERATED - DO NOT EDIT MANUALLY. Regenerate with\n");
        record.Append($"`{AssuranceGenerator.WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,\n");
        record.Append($"`{AssuranceGenerator.ReportPath}`, `{AssuranceManifest.RelativePath}` and every generated source header from the\n");
        record.Append("product tree.\n\n");

        record.Append($"> **Status: {Status(units)}.** Human-reviewed: {summary.Verified} of {summary.Relevant} relevant units. No package\n");
        record.Append("> may be published from this component, no RID claimed and no milestone accepted until every\n");
        record.Append("> relevant unit carries a decision, which is update rule 8 in the status ledger.\n\n");

        record.Append(HowToUseThisFile());
        record.Append(HowAReviewIsRecorded());

        record.Append("## 3. Summary\n\n");
        record.Append("| Metric | Value |\n|---|---:|\n");
        record.Append($"| Files scanned | {files.Count} |\n");
        record.Append($"| Code units | {units.Count} |\n");
        record.Append($"| Relevant | {summary.Relevant} |\n");
        record.Append($"| Exempt | {summary.Exempt} |\n");
        record.Append($"| Assessed | {Portion(summary.Annotated, summary.Relevant)} |\n");
        record.Append($"| Human reviewed | {Portion(summary.Verified, summary.Relevant)} |\n");
        record.Append($"| Unverified | {summary.Unverified} |\n");
        record.Append($"| Aliases naming a decision | {Reviewers(units).Count} |\n\n");

        record.Append("## 4. Review States\n\n");
        record.Append("One row per state of the machine that reads the two lines. The states are computed from the\n");
        record.Append("annotations and the current fingerprints; nothing stores them.\n\n");
        record.Append("| State | Units |\n|---|---:|\n");

        foreach (var state in Enum.GetValues<AssuranceReviewState>())
        {
            record.Append($"| {AssuranceStateMachine.Name(state)} | {units.Count(unit => unit.State == state)} |\n");
        }

        record.Append('\n');
        record.Append(ReviewerSection(units));
        record.Append(CoverageSection(files, units));
        record.Append(DecisionSection(units));
        record.Append(OutrunSection(units));
        record.Append(RequiredFirstSection(units));
        record.Append(WhatThisRecordDoesNotSay());

        return record.ToString();
    }

    // ---- The two sections that are prose ---------------------------------------------------------

    /// <summary>
    /// Section 1: the component's mark legend. Every other review document links here rather than
    /// carrying a second copy, and rule H1 closes the corpus against exactly these rows.
    /// </summary>
    private static string HowToUseThisFile() =>
        "## 1. How To Use This File\n" +
        "\n" +
        "This section is the canonical mark legend for the component. The evidence bundles and the\n" +
        "status ledger link here rather than repeating the tables. There are two vocabularies, they are\n" +
        "different kinds of thing, and they must never be mixed. Both are closed sets, and rule H1\n" +
        "refuses a mark in any review document that this section does not publish.\n" +
        "\n" +
        "### Evidence verdicts - stated about a piece of evidence\n" +
        "\n" +
        "| Mark | Meaning |\n" +
        "|---|---|\n" +
        "| `[MET]` | Demonstrated. An execution, artefact or log in a retained bundle shows it. |\n" +
        "| `[PART]` | Partly demonstrated. What is not shown is named on the same row. |\n" +
        "| `[UNMET]` | Not discharged. The condition is stated and not satisfied. |\n" +
        "| `[N/A]` | Not claimed at this milestone. The milestone that owns it is named. |\n" +
        "\n" +
        "### Review verdicts - stated in an evidence bundle about a gate clause\n" +
        "\n" +
        "| Mark | Meaning |\n" +
        "|---|---|\n" +
        "| `[ ]` | Not yet read. |\n" +
        "| `[A]` | Accepted as stated. |\n" +
        "| `[C]` | Accepted with a condition. The condition is recorded beside it. |\n" +
        "| `[R]` | Rejected. The defect is recorded. |\n" +
        "| `[?]` | Cannot be judged from what is here. What is missing is named. |\n" +
        "\n" +
        "**No verdict in this file is a mark.** A decision about a code unit is the\n" +
        "`// Broiler-Human:` line on that unit's declaration, and every table below is read out of\n" +
        "those lines. There is nothing here to fill in and nothing here to leave blank.\n" +
        "\n";

    /// <summary>
    /// Section 2: the whole of what a human writes, and the reason this file records no commit.
    /// </summary>
    private static string HowAReviewIsRecorded() =>
        "## 2. How A Review Is Recorded\n" +
        "\n" +
        "In one place: the `// Broiler-Human:` line of the assurance annotation that sits on the\n" +
        "declaration being read. Nothing in this file is edited by hand, no second document carries a\n" +
        "per-item checklist, and no list of permitted aliases exists to be added to.\n" +
        "\n" +
        "```csharp\n" +
        "// Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=630EF7\n" +
        "// Broiler-Falsified-If: new T[] is reached before TryReserve returns true\n" +
        "// Broiler-Human:        PENDING\n" +
        "```\n" +
        "\n" +
        "The last line has four shapes. A human writes three of them; the generator writes the fourth\n" +
        "and may never invent an alias, which rule J4 asserts in both directions.\n" +
        "\n" +
        "| Line | Meaning |\n" +
        "|---|---|\n" +
        "| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |\n" +
        "| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |\n" +
        "| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |\n" +
        "| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |\n" +
        "\n" +
        "A human may state their own `IP=`, `Security=` and `Resources=` assessment beside their alias,\n" +
        "which is how a reader disagrees with the machine assessment on the line above: an assessment is\n" +
        "a comment and moves no fingerprint, so there is nowhere else to say it.\n" +
        "\n" +
        "**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of\n" +
        "the declaration it was made against, and the state machine compares that value with the\n" +
        "declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit\n" +
        "did, which is the narrower and the more useful of the two.\n" +
        "\n" +
        "This file is produced on every pull request by the review lane in `.github/workflows/`, and the\n" +
        "publish lane refuses to run while any relevant unit is unresolved, any fingerprint is out of\n" +
        "date, any annotation is malformed or any generated artefact is stale.\n" +
        "\n";

    /// <summary>Section 10: the limits, which no generation can remove.</summary>
    private static string WhatThisRecordDoesNotSay() =>
        "## 10. What This Record Does Not Say\n" +
        "\n" +
        "It is not an approval of the component, and a full table above would not be one either. It\n" +
        "records which declarations somebody stated a decision about, and against which version of\n" +
        "each. It does not record what they read, how long they spent, or whether they were right.\n" +
        "\n" +
        "Broiler.VM has one person in every role: architecture owner, core-contract owner, security\n" +
        "owner and reader are the same individual, so **no second pair of eyes has seen this work.**\n" +
        "That is a property of the project's size rather than a defect in this component, and it is why\n" +
        "the tables above have room for as many aliases as the tree names rather than one signature\n" +
        "line.\n" +
        "\n" +
        "A fingerprint is six hex characters of SHA-256 over a declaration's token texts. It answers\n" +
        "whether a unit changed since a decision was recorded against it. It is not a collision-free\n" +
        "identifier across units and it is not a cryptographic commitment, so it detects a change and\n" +
        "does not resist a forger with commit access.\n" +
        "\n" +
        "The assessments the decisions are recorded beside are machine-written and unread: an\n" +
        "assessment is a comment, so downgrading one moves no fingerprint anywhere, which exclusions\n" +
        "EX-65 and EX-76 record.\n";

    // ---- The derived sections --------------------------------------------------------------------

    private static string ReviewerSection(IReadOnlyList<AssuranceUnit> units)
    {
        var rows = Reviewers(units);
        var section = new StringBuilder("## 5. Aliases In The Tree\n\n");

        if (rows.Count == 0)
        {
            section.Append("No alias appears on a human line anywhere in the product tree. Nobody has recorded a\n");
            section.Append("decision about any unit of this component.\n\n");

            return section.ToString();
        }

        section.Append("Read out of the human lines, never registered. `Current` counts the units whose decision\n");
        section.Append("names the fingerprint the declaration carries now; `Outrun` counts the units whose\n");
        section.Append("declaration has changed since.\n\n");
        section.Append("| Alias | Units | Files | Current | Outrun |\n|---|---:|---:|---:|---:|\n");

        foreach (var row in rows)
        {
            section.Append($"| {row.Alias} | {row.Units} | {row.Files} | {row.Current} | {row.Outrun} |\n");
        }

        section.Append('\n');

        return section.ToString();
    }

    /// <summary>
    /// Section 6: the generated file headers, gathered into one table.
    /// </summary>
    /// <remarks>
    /// Every column here is a row of the header the generator writes at the top of that file, so a
    /// reader comparing the two is comparing one derivation with itself. That is deliberate: the
    /// header is what a developer opening the file sees, and this is the same figure in the record
    /// that decides a release.
    /// </remarks>
    private static string CoverageSection(
        IReadOnlyList<AssuranceSourceFile> files,
        IReadOnlyList<AssuranceUnit> units)
    {
        var section = new StringBuilder("## 6. Coverage By File\n\n");

        section.Append("One row per covered file, carrying that file's generated header. `Unverified` counts the\n");
        section.Append("relevant units in a state that blocks a release.\n\n");
        section.Append("| File | Units | Relevant | Exempt | Unverified | IP risk | Security risk | Criteria |\n");
        section.Append("|---|---:|---:|---:|---:|---|---|---:|\n");

        foreach (var file in files)
        {
            var owned = units
                .Where(unit => string.Equals(unit.File.RelativePath, file.RelativePath, StringComparison.Ordinal))
                .ToArray();

            var summary = AssuranceSummary.Of(owned);

            section.Append(
                $"| `{file.RelativePath}` | {owned.Length} | {summary.Relevant} | {summary.Exempt} | " +
                $"{summary.Unverified} | {summary.MaxIpRisk ?? "not assessed"} | " +
                $"{summary.MaxSecurityRisk ?? "not assessed"} | {summary.Criteria}/{summary.CriteriaRequired} |\n");
        }

        section.Append('\n');

        return section.ToString();
    }

    private static string DecisionSection(IReadOnlyList<AssuranceUnit> units)
    {
        var decided = Decided(units);
        var section = new StringBuilder("## 7. Decisions Recorded\n\n");

        if (decided.Count == 0)
        {
            section.Append("No unit in this component carries a decision on its human line. Every one of them reads\n");
            section.Append($"`{AssuranceAnnotation.Pending}`.\n\n");

            return section.ToString();
        }

        section.Append("One entry per unit whose human line names an alias, with the line exactly as the source\n");
        section.Append("states it.\n\n");

        foreach (var unit in decided)
        {
            section.Append($"- `{unit.Name}` in `{unit.File.RelativePath}` - {HumanLine(unit)}\n");
        }

        section.Append('\n');

        return section.ToString();
    }

    private static string OutrunSection(IReadOnlyList<AssuranceUnit> units)
    {
        var outrun = Outrun(units);
        var section = new StringBuilder("## 8. Decisions The Code Has Outrun\n\n");

        if (outrun.Count == 0)
        {
            section.Append("No unit carries a decision that the code has since moved past.\n\n");

            return section.ToString();
        }

        section.Append("The declaration changed after the decision was recorded. The alias and the version it was\n");
        section.Append("recorded against are preserved rather than deleted, because that is more useful than a\n");
        section.Append("blank line. Only a human clears one.\n\n");

        foreach (var unit in outrun)
        {
            section.Append(
                $"- `{unit.Name}` in `{unit.File.RelativePath}` - {HumanLine(unit)}, now `{unit.Fingerprint}`\n");
        }

        section.Append('\n');

        return section.ToString();
    }

    private static string RequiredFirstSection(IReadOnlyList<AssuranceUnit> units)
    {
        var required = RequiredFirst(units);
        var section = new StringBuilder("## 9. Where A Decision Is Required First\n\n");

        if (required.Count == 0)
        {
            section.Append("No unit is assessed `High` or `Critical`. This says nothing about the units nothing has\n");
            section.Append("assessed.\n\n");

            return section.ToString();
        }

        section.Append("The units at the top of the security vocabulary, with the observation that would show each\n");
        section.Append("one wrong and the human line it carries. The set is read from the assessments rather than\n");
        section.Append("written out, so a unit that becomes `High` joins it at the next generation.\n\n");

        foreach (var unit in required)
        {
            section.Append(
                $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
                $"Security={unit.Annotation!.Field("Security")}, `{unit.Fingerprint}`, {HumanLine(unit)}\n");

            section.Append($"  - Falsified if: {unit.Annotation!.FalsifiedIf ?? "no criterion is stated"}\n");
        }

        section.Append('\n');

        return section.ToString();
    }

    private static string Portion(int part, int whole) => whole == 0
        ? part.ToString(CultureInfo.InvariantCulture)
        : $"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)";
}
