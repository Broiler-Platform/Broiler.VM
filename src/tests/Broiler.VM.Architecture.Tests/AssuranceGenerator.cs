using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One generated artefact: what is on disk, and what the generator says should be.</summary>
internal sealed record AssuranceArtefact(string RelativePath, string FullPath, string Current, string Desired)
{
    internal bool IsCurrent => string.Equals(Current, Desired, StringComparison.Ordinal);
}

/// <summary>
/// What the generator would leave behind: every artefact, and the units as they read afterwards.
/// </summary>
/// <remarks>
/// The unit set is the post-generation one deliberately. Every property the assurance rules assert
/// - fingerprint currency, the absence of a reviewer, which units are annotated - is a property of
/// the tree the generator produces, and the gate separately asserts that the tree on disk IS that
/// tree. Asserting the properties against a snapshot taken before the generator ran would make
/// them fail during a write run for no reason a reader could act on.
/// </remarks>
internal sealed record AssurancePlan(
    IReadOnlyList<AssuranceArtefact> Artefacts,
    IReadOnlyList<AssuranceSourceFile> Files,
    IReadOnlyList<AssuranceUnit> Units);

/// <summary>
/// The generator and the gate, as one function.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why one function and not two.</b> The policy gives fingerprint maintenance and summary
/// generation to CI. This component has no CI lane - exclusion EX-45 records one RID, one machine
/// and no lane - so the same code runs in both roles. <see cref="Plan"/> computes what every
/// generated artefact should contain and is pure; <see cref="Apply"/> writes it. With
/// <c>BROILER_ASSURANCE_WRITE=1</c> the test applies the plan; without it, the test asserts every
/// artefact already equals it. A gate that verified a different computation from the one that
/// generated would be a gate over nothing, which is why there is only one.
/// </para>
/// <para>
/// <b>What the generator may write.</b> The <c>Fingerprint</c> field, the <c>STALE; Previous=</c>
/// rewrite of a human line that already names a reviewer, the generated summaries, and
/// <c>assurance.manifest.json</c> - which is in the plan rather than written separately, so the one
/// gate covers it exactly as it covers the report and the file headers. Nothing else. It may not
/// turn <c>PENDING</c> into a reviewer, may not invent a reviewer, and may not
/// turn <c>STALE</c> into <c>VERIFIED</c>. That is not left to intent:
/// <see cref="RefuseInventedApproval"/> compares the human lines before and after and throws if a
/// name appears that the source did not already carry.
/// </para>
/// <para>
/// <b>And what the generator may SAY.</b> Nothing in this file used to be held to anything: two
/// lines appended to <see cref="AssuranceManifest.Header"/> made the manifest assert that every
/// unit was verified, human-reviewed and eligible for release, and both modes of the gate stayed
/// green because the gate asks whether the file on disk is what this code would write and it was.
/// Two rules now read the output text itself. Rule J8 holds every generated artefact line for line
/// to <see cref="AssuranceArtefactShape"/>, a hand-maintained second copy of the fixed text with
/// every derived value derived again; rule J9 refuses review vocabulary the annotations do not
/// support anywhere in the generated text. Every sentence written here has to be written there too.
/// </para>
/// <para>
/// <b>Scope of the file header.</b> EVERY covered product file receives the header, including the
/// three that declare no code unit at all and therefore carry no annotation. Two things force
/// that. The policy asks for SPDX copyright and licence metadata on the source, and the header's
/// first two lines are exactly that, so a file the generator skipped would be a file with no
/// licence declaration - byte-identical SPDX across the tree is only achievable if one writer owns
/// all of it. And a file with no relevant unit is a claim worth publishing rather than a silence:
/// <c>Relevant units: 0</c> says the scanner looked and found nothing to review, which a missing
/// header does not say. An earlier revision adopted per file, beginning with the first annotation,
/// because the annotations had not landed yet; they have, and the scope is now the whole set.
/// </para>
/// </remarks>
internal static class AssuranceGenerator
{
    /// <summary>
    /// The fields a human line may carry beside the reviewer. Fingerprint is machine-maintained;
    /// the other three are the reviewer's own assessment and the generator never writes them.
    /// </summary>
    private static readonly string[] HumanFieldMarkers =
        ["Fingerprint=", "IP=", "Security=", "Resources="];

    /// <summary>Set to <c>1</c> to make the run write rather than assert.</summary>
    internal const string WriteVariable = "BROILER_ASSURANCE_WRITE";

    internal const string ReportPath = "CODE-ASSURANCE.md";

    internal const string GeneratedMarker = "// GENERATED - DO NOT EDIT MANUALLY";
    internal const string SpdxCopyright = "// SPDX-FileCopyrightText: 2026 Broiler Platform contributors";
    internal const string SpdxLicense = "// SPDX-License-Identifier: Apache-2.0";
    internal const string BannerRule = "// ----------------------";

    /// <summary>The banner line that opens the generated summary. Exactly one per covered file.</summary>
    internal const string Banner = "// Broiler Code Assurance";

    /// <summary>
    /// The labels the generated summary's rows carry, in the order it writes them, so a forged
    /// block can be recognised by its shape and not only by its banner - and so that rule J8 can
    /// hold the header to a row sequence rather than to whatever the generator felt like emitting.
    /// </summary>
    internal static readonly string[] HeaderRowLabels =
    [
        "Relevant units:", "Annotated:", "Exempt:", "Human-reviewed:",
        "IP risk:", "Security risk:", "Criteria:", "Resource impact:", "Unverified:",
    ];

    /// <summary>
    /// The vocabulary that marks a comment line as belonging to a generated assurance summary,
    /// matched case-insensitively and by containment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why not a literal-string whitelist.</b> The version before this one compared each line
    /// for exact equality against the banner and the marker, and asked whether it STARTED WITH
    /// <c>"// " + label</c> - ordinal, both of them. A summary block worded differently was
    /// therefore not a summary block: <c>// BROILER CODE ASSURANCE</c>, <c>//  Human-reviewed:
    /// 47/47</c> with two spaces, or a block that says <c>// Reviewer: EB</c> instead of using a
    /// row label at all, all sat below the generated header, survived regeneration byte for byte
    /// and read to a human exactly as the real block does.
    /// </para>
    /// <para>
    /// So the test is containment, case-insensitive, over the vocabulary a summary is made of: the
    /// banner, the marker, the eight row labels, and the review claims a forged summary exists in
    /// order to make. Every entry is checked against the product tree - below the generated header,
    /// the three product assemblies contain none of these phrases in any comment - so the rule
    /// costs nothing to adopt and is not a licence to reword an honest comment into a violation.
    /// The word <c>verified</c> is deliberately NOT here: <c>VmVerifiedArtifact</c> is a public type
    /// of this component and its name appears in 112 lines below the generated headers, 45 of them
    /// comments. Rule J9 is where the review vocabulary including that word is forbidden, over the
    /// GENERATED text alone.
    /// </para>
    /// </remarks>
    /// <remarks>
    /// <c>criteria:</c> is the ninth row label and is here for the same reason the other eight are.
    /// It does NOT reach a <c>// Broiler-Falsified-If:</c> line, which is a comment below the
    /// generated header in every annotated file: the criterion line carries its own label and the
    /// word appears nowhere on it. The list is asserted against the product tree, so a criterion
    /// whose prose happened to contain a summary row label would be reported here rather than
    /// silently accepted - which is a false positive on honest prose, and the answer to it is to
    /// write the criterion without the phrase rather than to stop looking below the header.
    /// </remarks>
    internal static readonly string[] SummaryVocabulary =
    [
        "broiler code assurance",
        "generated - do not edit manually",
        "relevant units:", "annotated:", "exempt:", "human-reviewed",
        "ip risk", "security risk", "criteria:", "resource impact", "unverified:",
        "human reviewed", "reviewer", "reviewstate", "humanreviewed", "approved",
        "eligible for release",
    ];

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The plan for this checkout, computed once. Pure: it reads the tree and writes nothing.</summary>
    internal static AssurancePlan Current { get; } = Plan();

    /// <summary>
    /// Every generated artefact, with its current and desired content, and the units as they will
    /// read once the artefacts are written.
    /// </summary>
    internal static AssurancePlan Plan()
    {
        var byFile = AssuranceScanner.Units
            .GroupBy(static unit => unit.File.RelativePath, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        var artefacts = new List<AssuranceArtefact>();

        // The component report describes the tree the generator is about to leave behind, not the
        // one it found. Each rewritten file is rescanned from its own generated text, so one run
        // converges rather than publishing last run's states.
        var after = new List<AssuranceUnit>();
        var rewritten = new List<AssuranceSourceFile>();

        foreach (var file in AssuranceSources.Files)
        {
            var units = byFile.TryGetValue(file.RelativePath, out var found) ? found : [];
            var desired = DesiredSource(file, units);
            var generated = AssuranceSources.WithText(file, desired);

            artefacts.Add(new AssuranceArtefact(file.RelativePath, file.FullPath, file.Text, desired));
            rewritten.Add(generated);
            after.AddRange(AssuranceScanner.Scan(generated));
        }

        var report = Path.Combine(ComponentGraph.Root, ReportPath);

        artefacts.Add(new AssuranceArtefact(
            ReportPath,
            report,
            File.Exists(report) ? File.ReadAllText(report) : string.Empty,
            ComponentReport(after)));

        // The manifest covers EVERY unit, exempt and relevant alike. It is in the plan rather than
        // written separately so that the one gate - what is on disk against what the generator
        // would write - covers it exactly as it covers the report and the file headers.
        var manifest = Path.Combine(ComponentGraph.Root, AssuranceManifest.RelativePath);

        artefacts.Add(new AssuranceArtefact(
            AssuranceManifest.RelativePath,
            manifest,
            File.Exists(manifest) ? File.ReadAllText(manifest) : string.Empty,
            AssuranceManifest.Render(rewritten, after)));

        // The human-review record is in the plan for the same reason, and it is the reason the plan
        // is worth having: the record a release is decided from is now derived from the annotations
        // rather than written beside them, so J5 holds it to the tree, J8 holds its sentences to a
        // declared shape and J9 refuses it a review claim the human lines do not carry. A record
        // maintained by hand had none of those three.
        var humanReview = Path.Combine(ComponentGraph.Root, AssuranceHumanReview.RelativePath);

        artefacts.Add(new AssuranceArtefact(
            AssuranceHumanReview.RelativePath,
            humanReview,
            File.Exists(humanReview) ? File.ReadAllText(humanReview) : string.Empty,
            AssuranceHumanReview.Render(rewritten, after)));

        return new AssurancePlan(artefacts, rewritten, after);
    }

    /// <summary>Writes every artefact that is not already current, and names the ones it changed.</summary>
    internal static IReadOnlyList<string> Apply(IEnumerable<AssuranceArtefact> plan)
    {
        var written = new List<string>();

        foreach (var artefact in plan.Where(static artefact => !artefact.IsCurrent))
        {
            File.WriteAllText(artefact.FullPath, artefact.Desired, AssuranceSources.Utf8NoBom);
            written.Add(artefact.RelativePath);
        }

        return written;
    }

    /// <summary>
    /// Every artefact in a plan whose content on disk is not what the generator would write, with
    /// the first line at which the two part company. This is the gate, as one function.
    /// </summary>
    /// <remarks>
    /// It is a function rather than an expression written out at each call site because the
    /// currency comparison is the clause rule J5 and the generator harness both assert, and two
    /// hand-written copies of one property is how a property ends up asserted nowhere: the copies
    /// drift, and each looks like the other's coverage. One function, one witness - J5 drives a
    /// deliberately stale artefact through it and reads the message it produces.
    /// </remarks>
    internal static IReadOnlyList<string> StaleArtefacts(IEnumerable<AssuranceArtefact> plan) =>
        plan.Where(static artefact => !artefact.IsCurrent).Select(Describe).ToList();

    /// <summary>The first line where an artefact and its regeneration part company.</summary>
    internal static string Describe(AssuranceArtefact artefact)
    {
        var current = new AssuranceText(artefact.Current);
        var desired = new AssuranceText(artefact.Desired);

        for (var line = 0; line < Math.Max(current.Count, desired.Count); line++)
        {
            var left = line < current.Count ? current[line] : "<end of file>";
            var right = line < desired.Count ? desired[line] : "<end of file>";

            if (!string.Equals(left, right, StringComparison.Ordinal))
            {
                return $"{artefact.RelativePath}({line + 1}) is not what the generator would write." +
                    $"\n  on disk:   {left}" +
                    $"\n  generated: {right}" +
                    $"\n  Run: {WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release";
            }
        }

        return $"{artefact.RelativePath} differs in length only";
    }

    /// <summary>
    /// Every covered file carrying more than one generated assurance block, as a fact in its own
    /// right rather than as a byte difference.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A second block below the first used to survive regeneration verbatim, which made the
    /// forgery a FIXED POINT: the generator reproduced it byte for byte, the currency comparison
    /// was satisfied, and a reader of the file saw two summaries, the second one saying the file
    /// was fully human-reviewed and carried no security risk. <see cref="RemoveExistingHeader"/>
    /// now strips every block from the leading comment run, so the byte comparison catches that
    /// shape as well - and this is said separately because "the file disagrees with the generator
    /// by one line" is not the fact a reader needs. The fact is that the file carries two summaries
    /// and one of them is a forgery.
    /// </para>
    /// <para>
    /// <b>The banner is looked for ANYWHERE in the file, and both ends of the line are trimmed.</b>
    /// The version before this one did neither, and the two omissions were one defeat. A forged
    /// block INDENTED four spaces inside a class body is not part of the leading comment run, so
    /// the header stripper never saw it and reproduced it verbatim; and the count compared
    /// <c>TrimEnd()</c> against the banner, so four spaces of leading whitespace hid it from the
    /// count as well. The file then carried a second summary claiming a full human review, was a
    /// fixed point under regeneration, and passed both halves of the defence.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> DuplicateAssuranceBlocks(IEnumerable<AssuranceSourceFile> files) =>
        files
            .Select(static file => (file.RelativePath, Count: BannerCount(file.Text)))
            .Where(static counted => counted.Count > 1)
            .Select(static counted =>
                $"{counted.RelativePath} carries {counted.Count} '{Banner[3..]}' banners; " +
                "exactly one block is generated and every other one is a forgery")
            .ToList();

    /// <summary>
    /// How many generated assurance banners a text carries, at any indentation and anywhere in the
    /// file.
    /// </summary>
    /// <remarks>
    /// <c>Trim()</c> and not <c>TrimEnd()</c>. Leading whitespace is not a difference in what the
    /// line SAYS, and a forged summary indented into a class body reads to a human exactly as the
    /// header does - which is the whole of its value to a forger. Case is not a difference either,
    /// for the same reason: <c>// BROILER CODE ASSURANCE</c> is the same banner to a reader.
    /// </remarks>
    internal static int BannerCount(string text) => new AssuranceTextLines(text)
        .Count(static line => string.Equals(line.Trim(), Banner, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Every covered file carrying a line of a generated assurance summary BELOW the generated
    /// header, wherever it sits and at whatever indentation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The banner count answers "is there more than one summary in this file". This answers the
    /// question a forger asks next: is there a summary line ANYWHERE the header stripper does not
    /// reach. A block indented inside a class body is not part of the leading comment run, so the
    /// generator neither strips it nor rewrites it - it is reproduced byte for byte, which makes
    /// the forgery a fixed point that the currency comparison accepts.
    /// </para>
    /// <para>
    /// A summary line is recognised by any of the three things a generated block carries: the
    /// banner, the <c>GENERATED</c> marker, or one of the header's row labels. All three, and not
    /// the banner alone, because a forgery that drops the banner and keeps
    /// <c>// Human-reviewed:   47/47</c> is still a claim about how much of the file a human has
    /// read - and dropping one line is the cheapest possible way past a rule that looked for one
    /// line.
    /// </para>
    /// <para>
    /// The header ends at the first <c>GENERATED</c> marker, and everything below that line is
    /// examined. One violation per file, naming the first offending line and counting the rest, so
    /// a pasted ten-line block is one fact rather than ten.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> ForgedAssuranceBlocks(IEnumerable<AssuranceSourceFile> files)
    {
        var violations = new List<string>();

        foreach (var file in files)
        {
            var lines = new AssuranceTextLines(file.Text);
            var header = lines.FindIndex(static line =>
                string.Equals(line.Trim(), GeneratedMarker, StringComparison.Ordinal));

            var forged = new List<int>();

            for (var line = header + 1; line < lines.Count; line++)
            {
                if (IsAssuranceSummaryLine(lines[line]))
                {
                    forged.Add(line);
                }
            }

            if (forged.Count == 0)
            {
                continue;
            }

            violations.Add(
                $"{file.RelativePath}({forged[0] + 1}) carries the assurance summary line " +
                $"'{lines[forged[0]].Trim()}' below the generated header, and {forged.Count} " +
                "such line(s) sit there; the generated block is the only one a file may carry");
        }

        return violations;
    }

    /// <summary>
    /// True for a comment line that carries the vocabulary of a generated assurance summary, at any
    /// indent, in any case and at any position in the line.
    /// </summary>
    /// <remarks>
    /// The line must be a comment, because the subject is a forged SUMMARY and a summary is written
    /// in comments; a string literal that happens to contain a row label is not one. Everything else
    /// about the test is deliberately loose - see <see cref="SummaryVocabulary"/> for what an exact
    /// whitelist cost.
    /// </remarks>
    internal static bool IsAssuranceSummaryLine(string line)
    {
        var content = line.Trim();

        return content.StartsWith("//", StringComparison.Ordinal) &&
            SummaryVocabulary.Any(term => content.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    // =============================================================================================
    // One source file
    // =============================================================================================

    internal static string DesiredSource(AssuranceSourceFile file, IReadOnlyList<AssuranceUnit> units)
    {
        var refreshed = RefreshedAnnotations(file, units);

        // The header is derived from the annotations, so it is computed from the file as it will
        // BE, not as it was: a state that this same pass is about to change must not be published
        // as though it were the answer. Rescanning the refreshed text is what makes one generation
        // a fixed point.
        var text = new AssuranceText(refreshed);

        RemoveExistingHeader(text);
        text.Insert(0, Header(AssuranceScanner.Scan(AssuranceSources.WithText(file, refreshed)))
            .Append(string.Empty));

        return text.Render();
    }

    /// <summary>
    /// The file with every annotation's machine-maintained fields brought up to date and every
    /// block aligned to the three-label width, and nothing else touched. Each rewrite is one line
    /// for one line, so the line numbers the scanner recorded stay valid for the whole pass.
    /// </summary>
    /// <remarks>
    /// <b>The alignment is written here and nowhere else.</b> Three labels of three lengths line up
    /// only if one writer owns the padding: 689 hand-aligned blocks would be 689 places for it to
    /// be wrong, and the diff of the day the third line was introduced would have been unreadable.
    /// A block with no criterion is re-rendered too, which is the point - the AI and human lines of
    /// every block in the tree move to the width the criterion line needs whether or not that block
    /// carries one. What the generator does NOT do is author a criterion: the prose is carried
    /// through exactly as the source wrote it, because a criterion is a human's observation and
    /// this is not the thing that observes.
    /// </remarks>
    private static string RefreshedAnnotations(AssuranceSourceFile file, IReadOnlyList<AssuranceUnit> units)
    {
        var text = new AssuranceText(file.Text);

        foreach (var unit in units.Where(static unit => unit.Annotation is not null))
        {
            var annotation = unit.Annotation!;
            var indent = AssuranceText.IndentOf(text[annotation.AiLine]);

            text[annotation.AiLine] = AssuranceAnnotation.RenderAiLine(indent, RefreshedFields(unit));

            if (annotation.FalsifiedIfLine is { } criterion)
            {
                text[criterion] = AssuranceAnnotation.RenderFalsifiedIfLine(indent, annotation.FalsifiedIf!);
            }

            var body = RefreshedHumanBody(annotation, unit.Fingerprint);

            RefuseInventedApproval(unit, annotation.HumanBody, body);

            text[annotation.HumanLine] = AssuranceAnnotation.RenderHumanLine(indent, body);
        }

        return text.Render();
    }

    /// <summary>
    /// The AI line's fields with the <c>Fingerprint</c> field filled from the current code. Every
    /// other field is carried through untouched, in source order: they are assessments, and this
    /// is not the thing that assesses.
    /// </summary>
    private static IEnumerable<AssuranceField> RefreshedFields(AssuranceUnit unit) =>
        unit.Annotation!.Fields.Select(field =>
            string.Equals(field.Key, "Fingerprint", StringComparison.Ordinal)
                ? field with { Value = unit.Fingerprint }
                : field);

    /// <summary>
    /// The human line's new body. Four inputs, four answers, and no fifth: nothing here can produce
    /// a reviewer identifier that was not already on the line it is reading.
    /// </summary>
    private static string RefreshedHumanBody(AssuranceAnnotation annotation, string currentFingerprint)
    {
        // PENDING stays PENDING. This is the policy's hardest rule and its shortest branch.
        if (annotation.HumanIsPending || annotation.Reviewer is null)
        {
            return annotation.HumanIsStale ? annotation.HumanBody : AssuranceAnnotation.Pending;
        }

        var reviewer = annotation.Reviewer;
        var approved = annotation.HumanFingerprint;

        // Whatever the reviewer wrote beside their name, carried through untouched. Rebuilding
        // the line as "<reviewer>; Fingerprint=<x>" DELETED a reviewer's own IP, Security and
        // Resources assessment, which is precisely what the policy's CI mutation rules forbid:
        // the machine may fill the machine field and may never edit a human decision.
        var assessment = string.Concat(
            annotation.HumanBody
                .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Where(static part => !part.StartsWith("Fingerprint=", StringComparison.Ordinal))
                .Select(static part => "; " + part));

        // A human approved and left the machine field for the machine: fill it. This is the only
        // transition the generator performs into VERIFIED, and the approval was already there.
        if (approved is null ||
            string.Equals(approved, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal))
        {
            return $"{reviewer}{assessment}; Fingerprint={currentFingerprint}";
        }

        // The reviewed version is still the version here.
        if (string.Equals(approved, currentFingerprint, StringComparison.Ordinal))
        {
            return $"{reviewer}{assessment}; Fingerprint={approved}";
        }

        // The code has moved since the review. The reviewer and what they approved are preserved
        // rather than deleted, because "this was reviewed, and the current code is not that" is
        // more useful than a blank line.
        return $"{AssuranceAnnotation.Stale}; Previous={reviewer}@{approved}";
    }

    /// <summary>
    /// Refuses to rewrite a human line the policy does not define, and refuses to write one that
    /// names anyone the source did not already name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The policy forbids CI turning <c>Broiler-Human: PENDING</c> into
    /// <c>Broiler-Human: EB</c>. This is that rule as an exception rather than as a comment: the
    /// generator throws rather than writing, so a bug that reached for it fails loudly instead of
    /// quietly manufacturing an approval that no human gave.
    /// </para>
    /// <para>
    /// <b>Two clauses, because one was not enough.</b> An earlier revision had only the name
    /// comparison, and its <c>Names</c> helper skipped any part reading <c>PENDING</c> or
    /// <c>STALE</c> wherever it appeared. A human line reading
    /// <c>PENDING; Fingerprint=TBF</c> therefore named nobody on either side of the comparison and
    /// the refusal never fired - while <see cref="AssuranceAnnotation.HumanIsPending"/>, which
    /// wants exact equality, answered false and <see cref="AssuranceAnnotation.Reviewer"/> handed
    /// back the head token <c>PENDING</c> as a reviewer identifier. The generator filled the
    /// fingerprint, the state machine resolved VERIFIED, and the component report published a
    /// human review of a line that named no human. So: a body that is not one of the four defined
    /// shapes is refused before anything is rewritten, and <c>PENDING</c> names nobody only when
    /// it is the WHOLE body.
    /// </para>
    /// </remarks>
    internal static void RefuseInventedApproval(AssuranceUnit unit, string before, string after)
    {
        if (!IsDefinedHumanBody(before))
        {
            throw new InvalidOperationException(
                $"The assurance generator will not rewrite the human line on {unit.Where}, which " +
                $"reads '{before}'. A human line is one of '{AssuranceAnnotation.Pending}', a " +
                "reviewer, a reviewer with a Fingerprint, or " +
                $"'{AssuranceAnnotation.Stale}; Previous=<reviewer>@<fingerprint>'. " +
                "Only a human may create an approval.");
        }

        var permitted = Names(before);

        foreach (var name in Names(after).Where(name => !permitted.Contains(name)))
        {
            throw new InvalidOperationException(
                $"The assurance generator tried to write reviewer '{name}' onto {unit.Where}, " +
                $"whose human line reads '{before}'. Only a human may create an approval.");
        }
    }

    /// <summary>
    /// True for the four human-line shapes the policy defines, and for nothing else.
    /// </summary>
    /// <remarks>
    /// Anything outside this set is a line a human wrote by hand and the machine cannot read, which
    /// is exactly when it must not guess. The check is deliberately narrow: the cost of refusing a
    /// shape a reviewer meant is one error message naming the line, and the cost of accepting one
    /// is a manufactured approval.
    /// </remarks>
    internal static bool IsDefinedHumanBody(string body)
    {
        if (string.Equals(body, AssuranceAnnotation.Pending, StringComparison.Ordinal))
        {
            return true;
        }

        var parts = body.Split(';', StringSplitOptions.TrimEntries);

        if (parts.Length == 0 || parts[0].Length == 0)
        {
            return false;
        }

        // STALE; Previous=<reviewer>@<fingerprint>, and nothing else beside it.
        if (string.Equals(parts[0], AssuranceAnnotation.Stale, StringComparison.Ordinal))
        {
            const string Marker = "Previous=";

            return parts.Length == 2 &&
                parts[1].StartsWith(Marker, StringComparison.Ordinal) &&
                parts[1].IndexOf('@', StringComparison.Ordinal) > Marker.Length;
        }

        // A reviewer identifier is a bare token: it is neither of the two reserved words and it
        // carries no '=', which is what a field looks like.
        if (string.Equals(parts[0], AssuranceAnnotation.Pending, StringComparison.Ordinal) ||
            parts[0].Contains('=', StringComparison.Ordinal))
        {
            return false;
        }

        // A reviewer may state their OWN assessment beside their name. The policy's CI mutation
        // rules name "human IP assessment, human security assessment, human resource assessment"
        // among the things CI must never create, which only means anything if a human may write
        // them. It is also the one mechanical answer to EX-76 and EX-65: an AI assessment is a
        // comment that moves no fingerprint, so a reviewer who disagrees with Security=High has
        // nowhere to say so unless the human line can carry it.
        for (var index = 1; index < parts.Length; index++)
        {
            if (!HumanFieldMarkers.Any(marker =>
                    parts[index].StartsWith(marker, StringComparison.Ordinal)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// The reviewer identifiers a human line carries. Only the exact body <c>PENDING</c> carries
    /// none.
    /// </summary>
    internal static HashSet<string> ReviewerNames(string body) => Names(body);

    private static HashSet<string> Names(string body)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        if (string.Equals(body, AssuranceAnnotation.Pending, StringComparison.Ordinal))
        {
            return names;
        }

        var parts = body.Split(';', StringSplitOptions.TrimEntries);

        for (var index = 0; index < parts.Length; index++)
        {
            var part = parts[index];

            if (part.Length == 0 || part.StartsWith("Fingerprint=", StringComparison.Ordinal))
            {
                continue;
            }

            // STALE is a reserved word only where the policy puts it: at the head of the line.
            // Skipping it wherever it appeared is what made a reserved word into a hiding place.
            if (index == 0 && string.Equals(part, AssuranceAnnotation.Stale, StringComparison.Ordinal))
            {
                continue;
            }

            var value = part.StartsWith("Previous=", StringComparison.Ordinal)
                ? part["Previous=".Length..]
                : part;

            var at = value.LastIndexOf('@');

            names.Add(at < 0 ? value : value[..at]);
        }

        return names;
    }

    // =============================================================================================
    // The generated file header
    // =============================================================================================

    /// <summary>
    /// Removes the header block a previous generation left, so the next one replaces it rather
    /// than stacking on top of it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The block is the leading run of <c>//</c> lines, and it is only touched when the file opens
    /// with an SPDX copyright line - the one shape this generator writes. A file that opens with
    /// any other comment is left alone, so an ordinary banner comment is not mistaken for a header.
    /// </para>
    /// <para>
    /// The <c>GENERATED</c> marker is searched for INSIDE that run and nowhere else. Scanning the
    /// whole file for it, as an earlier revision did, meant a file whose body happened to contain
    /// the marker text in a comment would have had everything above it deleted. And a leading run
    /// with no marker in it - a hand-written SPDX pair, which is what a file adopting the system
    /// carries before its first generation - is removed whole: leaving it in place appended the
    /// generated header above it and published the licence declaration twice.
    /// </para>
    /// <para>
    /// <b>Every block, not the first one.</b> An earlier revision stopped at the first
    /// <c>GENERATED</c> marker and carried everything below it through verbatim, so a second
    /// summary block pasted under the real one survived regeneration byte for byte. That made the
    /// forgery a fixed point: the generator reproduced it, the byte comparison was satisfied, and
    /// the file published two summaries - the second one claiming a human review that no
    /// annotation supported. The loop below removes every further leading comment run that looks
    /// like a generated block, recognised by its banner, its <c>GENERATED</c> marker or any of its
    /// row labels, so a forgery is deleted rather than republished. Rule J5 also reports a file
    /// carrying more than one banner as a violation in its own right, because the byte difference
    /// this creates does not say what is wrong.
    /// </para>
    /// </remarks>
    private static void RemoveExistingHeader(AssuranceText text)
    {
        if (text.Count == 0 || !text[0].StartsWith("// SPDX-FileCopyrightText:", StringComparison.Ordinal))
        {
            return;
        }

        RemoveLeadingCommentRun(text);

        while (LeadingRunIsAnAssuranceBlock(text))
        {
            RemoveLeadingCommentRun(text);
        }
    }

    /// <summary>Removes the leading run of <c>//</c> lines, through its marker and one blank line.</summary>
    private static void RemoveLeadingCommentRun(AssuranceText text)
    {
        var run = LeadingCommentRun(text);

        if (run == 0)
        {
            return;
        }

        var marker = -1;

        for (var line = 0; line < run; line++)
        {
            if (string.Equals(text[line], GeneratedMarker, StringComparison.Ordinal))
            {
                marker = line;
                break;
            }
        }

        var through = marker >= 0 ? marker + 1 : run;

        if (through < text.Count && text[through].Trim().Length == 0)
        {
            through++;
        }

        text.RemoveRange(0, through);
    }

    private static int LeadingCommentRun(AssuranceText text)
    {
        var run = 0;

        while (run < text.Count && text[run].StartsWith("//", StringComparison.Ordinal))
        {
            run++;
        }

        return run;
    }

    /// <summary>
    /// True when the leading comment run is a generated summary rather than an ordinary comment.
    /// </summary>
    /// <remarks>
    /// Recognised by three things and not one: the banner, the <c>GENERATED</c> marker, and the
    /// row labels. A forgery that dropped the banner and kept the rows would still be a claim
    /// about how much of this file a human has read.
    /// </remarks>
    private static bool LeadingRunIsAnAssuranceBlock(AssuranceText text)
    {
        var run = LeadingCommentRun(text);

        for (var line = 0; line < run; line++)
        {
            if (IsAssuranceSummaryLine(text[line]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The generated header block of a file, or nothing when it carries none: the lines from the
    /// SPDX copyright through the <c>GENERATED</c> marker.
    /// </summary>
    /// <remarks>
    /// This is the GENERATED text of a source artefact, and it is what rules J8 and J9 read. The
    /// rest of the file is the component's own code and comments, which the generator does not
    /// write and must not be held to the shape or the vocabulary of something it did not produce.
    /// </remarks>
    internal static IReadOnlyList<string> GeneratedHeaderLines(string text)
    {
        var lines = new AssuranceTextLines(text);

        if (lines.Count == 0 ||
            !lines[0].StartsWith("// SPDX-FileCopyrightText:", StringComparison.Ordinal))
        {
            return [];
        }

        for (var line = 0; line < lines.Count && lines[line].StartsWith("//", StringComparison.Ordinal); line++)
        {
            if (string.Equals(lines[line], GeneratedMarker, StringComparison.Ordinal))
            {
                return lines.Take(line + 1).ToArray();
            }
        }

        return [];
    }

    internal static IEnumerable<string> Header(IReadOnlyList<AssuranceUnit> units)
    {
        var summary = AssuranceSummary.Of(units);

        yield return SpdxCopyright;
        yield return SpdxLicense;
        yield return "//";
        yield return Banner;
        yield return BannerRule;
        yield return Row("Relevant units:", summary.Relevant.ToString());
        yield return Row("Annotated:", $"{summary.Annotated}/{summary.Relevant}");
        yield return Row("Exempt:", summary.Exempt.ToString());
        yield return Row("Human-reviewed:", $"{summary.Verified}/{summary.Relevant}");
        yield return Row("IP risk:", summary.MaxIpRisk ?? "not assessed");
        yield return Row("Security risk:", summary.MaxSecurityRisk ?? "not assessed");

        // How much of this file's high-risk surface carries a falsification criterion. It sits
        // under the security row because the denominator IS that row: the units assessed High or
        // Critical are the ones a criterion is required on.
        yield return Row("Criteria:", $"{summary.Criteria}/{summary.CriteriaRequired}");
        yield return Row("Resource impact:", summary.MaxResources is { } score
            ? $"{score}/10 max"
            : "not assessed");
        yield return Row("Unverified:", summary.Unverified.ToString());
        yield return "//";
        yield return GeneratedMarker;
    }

    /// <summary>
    /// One row of the generated file header. Internal because rule J8 holds every header to this
    /// shape and would otherwise have to spell the padding out a second time, which is how two
    /// copies of one format drift apart.
    /// </summary>
    internal static string Row(string label, string value) => $"// {label.PadRight(18)}{value}";

    // =============================================================================================
    // CODE-ASSURANCE.md
    // =============================================================================================

    internal static string ComponentReport(IReadOnlyList<AssuranceUnit> units)
    {
        var summary = AssuranceSummary.Of(units);
        var files = units.Select(static unit => unit.File.RelativePath).Distinct(StringComparer.Ordinal).Count();
        var report = new StringBuilder();

        report.Append("# Broiler.VM Code Assurance\n\n");
        report.Append("GENERATED - DO NOT EDIT MANUALLY. Regenerate with\n");
        report.Append("`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,\n");
        report.Append($"`{AssuranceHumanReview.RelativePath}`, `{AssuranceManifest.RelativePath}` and every generated source header from the\n");
        report.Append("product tree.\n\n");
        // The opening sentence is DERIVED, and that is not a stylistic choice. Written as a fixed
        // statement of absence it becomes false on the day somebody records a decision, and nothing
        // would report it: rule J9 accepts a review term standing behind a negation, so a report
        // still saying nothing had been read while units were VERIFIED would pass every rule in the
        // suite. A record that cannot describe its own success is a record nobody may use.
        report.Append(summary.Verified == 0
            ? "**Nothing in this component has been reviewed by a human.** This report records that\n" +
                "absence precisely. It is not a claim that the code is reviewed, assured or safe, and the\n" +
                "figures below are the measurement of how far from that claim the component is.\n\n"
            : $"**Human-reviewed: {summary.Verified} of {summary.Relevant} relevant units.** This report records what the\n" +
                "annotations state and no more. A decision recorded here is one person's, bound to one\n" +
                "version of one declaration, and it is not a claim that the code is assured or safe.\n\n");

        report.Append("## Summary\n\n");
        report.Append("| Metric | Value |\n|---|---:|\n");
        report.Append($"| Files scanned | {AssuranceSources.Files.Count} |\n");
        report.Append($"| Files carrying an annotation | {AnnotatedFiles(units)} |\n");
        report.Append($"| Code units | {units.Count} |\n");
        report.Append($"| Relevant | {summary.Relevant} |\n");
        report.Append($"| Exempt by predicate | {summary.Exempt} |\n");
        report.Append($"| Annotated | {Portion(summary.Annotated, summary.Relevant)} |\n");
        report.Append($"| Human reviewed | {Portion(summary.Verified, summary.Relevant)} |\n");
        report.Append($"| Unverified | {summary.Unverified} |\n\n");

        report.Append("## Review states\n\n");
        report.Append("| State | Count |\n|---|---:|\n");

        foreach (var state in Enum.GetValues<AssuranceReviewState>())
        {
            report.Append($"| {AssuranceStateMachine.Name(state)} | {units.Count(unit => unit.State == state)} |\n");
        }

        report.Append('\n');
        report.Append(Distribution("IP risk", AssuranceAnnotation.IpRiskValues, "IP", units, summary));
        report.Append(Distribution("Security risk", AssuranceAnnotation.SecurityRiskValues, "Security", units, summary));

        report.Append("## Resource impact\n\n");
        report.Append("| Metric | Value |\n|---|---:|\n");
        report.Append($"| Maximum | {(summary.MaxResources is { } max ? $"{max} / 10" : "n/a")} |\n");
        report.Append($"| Average over annotated units | {(summary.MeanResources is { } mean ? mean.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " / 10" : "n/a")} |\n");
        report.Append($"| Units scored | {summary.Annotated} |\n\n");

        report.Append("## High-security review areas\n\n");

        // The file is part of the entry because the NAME is not unique on its own: a partial type
        // is two declarations, and `VmRuntime` appeared twice here with nothing to tell a reader
        // which of the two they were looking at. A manifest entry is addressed by file and name for
        // the same reason, and this is the section a reader turns to first.
        // The human line as the source states it, and not the state machine's name for what it made
        // of that line. Two reasons, and the second is a defect this list carried until a unit was
        // reviewed: the line is what a reader can go and check, and `state VERIFIED` is the bare
        // review term standing in generated prose with no count beside it, which rule J9 reports the
        // moment the first High unit is read - so the honest rendering is also the only one that
        // survives the system being used.
        var high = units
            .Where(static unit => unit.Annotation?.Field("Security") is "High" or "Critical")
            .Select(static unit =>
                $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
                $"Security={unit.Annotation!.Field("Security")}, " +
                $"human line {AssuranceHumanReview.HumanLine(unit)}")
            .ToArray();

        report.Append(high.Length == 0
            ? "No annotated unit is assessed High or Critical. This says nothing about the units nothing\nhas assessed.\n\n"
            : string.Join("\n", high) + "\n\n");

        // How much of that surface carries a falsification criterion. The section is next to the
        // high-security list because it is the same population: the units listed above are the
        // units a criterion is required on.
        report.Append("## Falsification criteria\n\n");
        report.Append("| Metric | Value |\n|---|---:|\n");
        report.Append($"| Units carrying a criterion | {summary.Criteria} |\n");
        report.Append($"| Units required to carry one | {summary.CriteriaRequired} |\n");
        report.Append($"| Required and missing | {AssuranceScanner.MissingFalsificationCriteria(units).Count} |\n\n");
        report.Append("A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make\n");
        report.Append("the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the\n");
        report.Append("criterion is the test. It is required where `Security` is `High` or `Critical`, permitted\n");
        report.Append("elsewhere, and rule J10 names every unit that owes one and carries none.\n\n");
        report.Append("The line is a comment, so it is outside every fingerprint by construction: rewording a\n");
        report.Append("criterion moves no recorded value here, in a file header or in\n");
        report.Append($"`{AssuranceManifest.RelativePath}`, and invalidates nothing. That is the intended reading - a\n");
        report.Append("criterion is an instruction to whoever reads the unit, not part of what a review is bound to.\n\n");
        report.Append("This third line is a local extension. The owner's policy defines two lines and not three,\n");
        report.Append("and it is added here because the two cannot carry a falsification criterion at all, and\n");
        report.Append("because the line numbers a separate worksheet cited rotted the moment the annotations moved\n");
        report.Append("the code: an annotation travels with its declaration and a citation does not. Exclusion\n");
        report.Append("EX-74 records that this is an extension to the policy rather than an implementation of it,\n");
        report.Append("and that the owner may reject it.\n\n");

        report.Append("## Exemption\n\n");
        report.Append("Exemption is decided by one predicate in `AssuranceScanner.ExemptionFor`, not per unit, so\n");
        report.Append("that the rule is reviewable in one place rather than in several hundred.\n\n");
        report.Append("| Case | Units |\n|---|---:|\n");

        foreach (var exemption in Enum.GetValues<AssuranceExemption>().Where(static value => value != AssuranceExemption.None))
        {
            report.Append($"| {exemption} | {units.Count(unit => unit.Exemption == exemption)} |\n");
        }

        // The per-unit escape hatch, counted and named. It is a sentence a human wrote and nothing
        // mechanical can check it, so the one thing that can be done with it is make every use
        // visible in the component's own report rather than silent in one source file.
        var declared = AssuranceScanner.DeclaredExemptions(units);

        report.Append("\n## Per-unit exemptions\n\n");
        report.Append($"| Metric | Value |\n|---|---:|\n| Per-unit exemptions | {declared.Count} |\n\n");
        report.Append("A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the\n");
        report.Append("predicate cannot see. Nothing mechanical checks that the reason is true, that it describes\n");
        report.Append("the unit it sits on, or that it says anything at all, so every use is counted and named\n");
        report.Append($"here. `{string.Join("`, `", AssuranceScanner.AssembliesClosedToTheEscapeHatch)}` is closed to it entirely: that assembly reads untrusted\n");
        report.Append("input, and a unit there is assessed or it is not shipped. Rule J1 asserts both halves.\n\n");

        report.Append(declared.Count == 0
            ? "No unit in this component states a per-unit exemption.\n\n"
            : string.Join(
                "\n",
                declared.Select(static unit =>
                    $"- `{unit.Name}` in `{unit.File.RelativePath}` - {unit.Annotation!.ExemptReason}")) + "\n\n");

        report.Append("## Change detection\n\n");
        report.Append($"`{AssuranceManifest.RelativePath}` lists **every** code unit in the three product assemblies -\n");
        report.Append($"{units.Count} of them, exempt and relevant alike - with the fingerprint of its declaration.\n");
        report.Append($"{AssuranceManifest.ChangeDetectionStatement} A unit listed there is watched, not reviewed:\n");
        report.Append("the entry records what the declaration's tokens hashed to when the generator last ran, and\n");
        report.Append("nothing else. Exempt units still need no annotation and carry none, and no human line in\n");
        report.Append("this component has moved off `PENDING`. What the manifest adds is that a unit the exemption\n");
        report.Append("predicate treats as trivial is no longer invisible: a semantic change to one moves a value\n");
        report.Append("in a generated file the gate compares byte for byte. Rule J7 holds the manifest to the tree.\n\n");
        report.Append($"Beside the units it lists **every covered file** - {AssuranceSources.Files.Count} of them - with a\n");
        report.Append("fingerprint over the complete token stream of its compilation unit. A unit entry exists only\n");
        report.Append("for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an\n");
        report.Append("`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.\n");
        report.Append($"{AssuranceManifest.CompletenessStatement} Comments are outside the stream, because a token's\n");
        report.Append("text is its own characters, so the generated header above and the annotation lines below move\n");
        report.Append("no file fingerprint - which is what lets one generation be a fixed point.\n\n");

        report.Append("## Verification\n\n");
        report.Append("The generator and the gate are the same code, run as a test in the architecture suite. Two\n");
        report.Append("lanes under `.github/workflows/` compel it rather than leaving it to whoever remembers: the\n");
        report.Append("review lane regenerates every artefact on a pull request and commits what moved, and the\n");
        report.Append("publish lane runs the release mode below and refuses to pack while anything is unresolved.\n");
        report.Append("Exclusion EX-45 still records one RID and one machine for the Native AOT evidence, which no\n");
        report.Append("lane reproduces.\n\n");
        report.Append("| Mode | Command | Effect |\n|---|---|---|\n");
        report.Append($"| Generate | `{WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `{AssuranceHumanReview.RelativePath}`, `{AssuranceManifest.RelativePath}` and this file. |\n");
        report.Append("| Gate | `dotnet test Broiler.VM.slnx -c Release` | Asserts every generated artefact is byte-identical to what the generator would produce. |\n");
        report.Append($"| Release | `{AssuranceRelease.GateVariable}=1 dotnet test Broiler.VM.slnx -c Release` | The gate, and additionally: no relevant unit left in a state that blocks a release, no annotation this system cannot read, no fingerprint out of date, no unit at the top of the security vocabulary without a criterion. |\n\n");
        report.Append("The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token\n");
        report.Append("texts, joined by single spaces. Trivia is excluded because a token's text is its own\n");
        report.Append("characters and never the comments or whitespace around it, so `dotnet format` moves no\n");
        report.Append("fingerprint and an annotation is never part of what it describes. The value answers whether a\n");
        report.Append("unit changed since it was reviewed. It is not a collision-free identifier across units and it\n");
        report.Append("is not a cryptographic commitment.\n");

        return report.ToString();
    }

    private static int AnnotatedFiles(IEnumerable<AssuranceUnit> units) => units
        .Where(static unit => unit.Annotation is not null)
        .Select(static unit => unit.File.RelativePath)
        .Distinct(StringComparer.Ordinal)
        .Count();

    private static string Distribution(
        string heading,
        IReadOnlyList<string> vocabulary,
        string field,
        IReadOnlyList<AssuranceUnit> units,
        AssuranceSummary summary)
    {
        var section = new StringBuilder($"## {heading}\n\n| Value | Units |\n|---|---:|\n");

        foreach (var value in vocabulary)
        {
            section.Append($"| {value} | {units.Count(unit => string.Equals(unit.Annotation?.Field(field), value, StringComparison.Ordinal))} |\n");
        }

        section.Append($"| *not annotated* | {summary.Relevant - summary.Annotated} |\n\n");

        return section.ToString();
    }

    private static string Portion(int part, int whole) => whole == 0
        ? $"{part}"
        : $"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)";
}

/// <summary>The aggregate the file header and the component report are both derived from.</summary>
internal sealed record AssuranceSummary(
    int Relevant,
    int Exempt,
    int Annotated,
    int Verified,
    int Unverified,
    int Criteria,
    int CriteriaRequired,
    string? MaxIpRisk,
    string? MaxSecurityRisk,
    int? MaxResources,
    double? MeanResources)
{
    internal static AssuranceSummary Of(IReadOnlyList<AssuranceUnit> units)
    {
        var relevant = units.Where(static unit => unit.IsRelevant).ToArray();

        var assessed = relevant
            .Where(static unit => unit.Annotation is { ExemptReason: null })
            .Select(static unit => unit.Annotation!)
            .ToArray();

        var scores = assessed
            .Select(static annotation => int.TryParse(annotation.Field("Resources"), out var score) ? score : (int?)null)
            .Where(static score => score is not null)
            .Select(static score => score!.Value)
            .ToArray();

        return new AssuranceSummary(
            Relevant: relevant.Length,
            Exempt: units.Count(static unit => unit.IsExempt),
            Annotated: assessed.Length,
            Verified: relevant.Count(static unit => unit.State == AssuranceReviewState.Verified),
            Unverified: relevant.Count(static unit => AssuranceStateMachine.BlocksRelease(unit.State)),
            Criteria: AssuranceScanner.FalsificationCriteria(units),
            CriteriaRequired: AssuranceScanner.UnitsRequiringACriterion(units),
            MaxIpRisk: Worst(assessed, "IP", AssuranceAnnotation.IpRiskValues),
            MaxSecurityRisk: Worst(assessed, "Security", AssuranceAnnotation.SecurityRiskValues),
            MaxResources: scores.Length == 0 ? null : scores.Max(),
            MeanResources: scores.Length == 0 ? null : scores.Average());
    }

    /// <summary>
    /// The weakest claim among the assessed units. The vocabularies are ordered weakest-claim-last,
    /// so <c>Unknown</c> outranks <c>High</c> for IP: a provenance nobody established is not a
    /// better answer than one that was.
    /// </summary>
    private static string? Worst(
        IReadOnlyList<AssuranceAnnotation> assessed,
        string field,
        IReadOnlyList<string> vocabulary)
    {
        var worst = -1;

        foreach (var annotation in assessed)
        {
            var rank = vocabulary
                .Select(static (value, index) => (value, index))
                .Where(entry => string.Equals(entry.value, annotation.Field(field), StringComparison.Ordinal))
                .Select(static entry => entry.index)
                .DefaultIfEmpty(-1)
                .Max();

            worst = Math.Max(worst, rank);
        }

        return worst < 0 ? null : vocabulary[worst];
    }
}
