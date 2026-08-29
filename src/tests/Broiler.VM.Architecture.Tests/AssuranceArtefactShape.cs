using System.Globalization;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The shape every generated artefact is held to: a hand-maintained copy of the fixed text, and an
/// independent derivation of every value.
/// </summary>
/// <remarks>
/// <para>
/// <b>The defect this exists to close.</b> Nothing held the generator's own output TEXT to
/// anything. Two lines appended to <see cref="AssuranceManifest.Header"/> made
/// <c>assurance.manifest.json</c> assert <c>reviewState=VERIFIED</c>, <c>humanReviewed=true</c>,
/// <c>reviewer=&lt;name&gt;</c> for all of its entries and state that the component was eligible for
/// release - with both modes of the gate green, because rule J5 compares the file on disk against
/// what the generator would write and the generator would write exactly that. A gate that asks only
/// "is this what the generator produces" cannot ask "and should the generator produce it".
/// </para>
/// <para>
/// So this file is the second copy, and it is hand-maintained on purpose, exactly as
/// <see cref="AssuranceRegisterRows"/> is. Every fixed sentence in a generated artefact appears
/// here verbatim, so an added, deleted or reworded sentence fails until somebody edits both places
/// having read both. It is deliberately NOT generated from the generator: a copy the generator
/// produced would agree with the generator whatever the generator said, which is the tautology this
/// whole register was written to remove.
/// </para>
/// <para>
/// <b>Where content is derived, the derivation is checked rather than copied.</b> A count, a
/// portion, a distribution row, the worst assessed risk and the list of high-security units are
/// computed here from the units, by expressions written differently from the generator's - the risk
/// scan below walks its vocabulary backwards where <see cref="AssuranceSummary"/> ranks and takes a
/// maximum. That is what pins <see cref="AssuranceSummary"/>: the IP and Security rows of every file
/// header are the two lines a developer actually reads, and nothing compared them against the
/// annotations they claim to summarize.
/// </para>
/// <para>
/// <b>What it cannot do.</b> A change made in both places passes, and that is the accepted cost of
/// the mechanism rather than an oversight - it is EX-70. What it buys is that no single edit to the
/// generator can put a sentence in front of a reader, and that rule J9 gets a second, independent
/// hold on the same text: J9 forbids the review vocabulary outright, whatever this shape says.
/// </para>
/// </remarks>
internal static class AssuranceArtefactShape
{
    // =============================================================================================
    // assurance.manifest.json - the $comment block
    // =============================================================================================

    /// <summary>
    /// The manifest's <c>$comment</c> lines, hand-copied. This is the array the attack appended to.
    /// </summary>
    internal static readonly string[] ManifestHeader =
    [
        "GENERATED - DO NOT EDIT MANUALLY. Regenerate with",
        "`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`.",
        "",
        "This manifest is a change-detection record, not a review. Every code unit in the three product",
        "assemblies is listed here, exempt and relevant alike, with the fingerprint of its",
        "declaration. An entry records what that declaration's tokens hashed to when the generator",
        "last ran. It is not an assessment, it is not an approval, and it is not evidence that",
        "anyone has read the unit. Exempt units still need no annotation and carry none, and no",
        "human line in this component has moved off PENDING.",
        "",
        "The 'files' array beside the units is what makes this record COMPLETE. A unit entry exists",
        "only for a declaration kind the scanner enumerates, and that enumeration is a whitelist: an",
        "assembly-level attribute is a member of nothing and can be in no unit at all. Each file",
        "entry is a fingerprint over the complete token stream of that file's compilation unit.",
        "Nothing in a covered file can change without something moving here, whatever kind of declaration it is.",
        "",
        "What the manifest adds is that a unit the exemption predicate treats as trivial is no",
        "longer invisible: a semantic change to one moves a fingerprint in a generated file the",
        "gate compares byte for byte, so the change appears in a diff and fails an unregenerated",
        "suite. Rule J7 holds this file to the tree - every unit and every covered file present, no",
        "extras, every fingerprint current.",
    ];

    /// <summary>Every line of a manifest header that is not the line this shape declares.</summary>
    /// <summary>
    /// The property names the manifest may use, per array. Held here as a second copy so that a
    /// property added in <c>AssuranceManifest.Render</c> is reported rather than published: the
    /// attack that made every entry carry <c>reviewState</c>, <c>humanReviewed</c> and
    /// <c>reviewer</c> passed because nothing below the $comment array was compared to anything.
    /// </summary>
    internal static readonly string[] ManifestFileProperties = ["file", "fingerprint"];

    internal static readonly string[] ManifestUnitProperties =
        ["name", "file", "exempt", "exemption", "fingerprint"];

    internal static readonly string[] ManifestArrays = ["$comment", "files", "units"];

    /// <summary>
    /// Every property name the rendered manifest actually uses, against the three lists above.
    /// A manifest is data, so the check is on its SHAPE: an entry may not carry a field this
    /// system does not define, and a review state is exactly such a field.
    /// </summary>
    internal static List<string> ManifestShapeViolations(string rendered)
    {
        var violations = new List<string>();
        using var document = System.Text.Json.JsonDocument.Parse(rendered);
        var root = document.RootElement;

        foreach (var property in root.EnumerateObject())
        {
            if (!ManifestArrays.Contains(property.Name, StringComparer.Ordinal))
            {
                violations.Add($"the manifest carries a top-level property this system does not define: {property.Name}");
            }
        }

        foreach (var expected in ManifestArrays)
        {
            if (!root.TryGetProperty(expected, out _))
            {
                violations.Add($"the manifest is missing its {expected} array");
            }
        }

        Check("files", ManifestFileProperties);
        Check("units", ManifestUnitProperties);

        return violations;

        void Check(string array, string[] permitted)
        {
            if (!root.TryGetProperty(array, out var element))
            {
                return;
            }

            foreach (var entry in element.EnumerateArray())
            {
                foreach (var property in entry.EnumerateObject())
                {
                    if (!permitted.Contains(property.Name, StringComparer.Ordinal))
                    {
                        violations.Add(
                            $"a {array} entry carries a property this system does not define: {property.Name}");
                    }
                }

                foreach (var required in permitted)
                {
                    if (!entry.TryGetProperty(required, out _))
                    {
                        violations.Add($"a {array} entry is missing {required}");
                    }
                }
            }
        }
    }

    internal static List<string> ManifestHeaderViolations(IReadOnlyList<string> header) =>
        Compare(AssuranceManifest.RelativePath, ManifestHeader, header);

    // =============================================================================================
    // The generated file header
    // =============================================================================================

    /// <summary>
    /// The header the shape declares for a file carrying these units: five fixed lines, the eight
    /// rows in their fixed order with independently derived values, and the closing marker.
    /// </summary>
    internal static IReadOnlyList<string> ExpectedFileHeader(IReadOnlyList<AssuranceUnit> units)
    {
        var relevant = units.Where(static unit => unit.IsRelevant).ToArray();
        var assessed = relevant
            .Where(static unit => unit.Annotation is { ExemptReason: null })
            .Select(static unit => unit.Annotation!)
            .ToArray();

        var scores = assessed
            .Select(static annotation =>
                int.TryParse(annotation.Field("Resources"), NumberStyles.None, CultureInfo.InvariantCulture, out var score)
                    ? score
                    : (int?)null)
            .Where(static score => score is not null)
            .Select(static score => score!.Value)
            .ToArray();

        var verified = relevant.Count(static unit => unit.State == AssuranceReviewState.Verified);

        return
        [
            AssuranceGenerator.SpdxCopyright,
            AssuranceGenerator.SpdxLicense,
            "//",
            AssuranceGenerator.Banner,
            AssuranceGenerator.BannerRule,
            AssuranceGenerator.Row("Relevant units:", relevant.Length.ToString(CultureInfo.InvariantCulture)),
            AssuranceGenerator.Row("Annotated:", $"{assessed.Length}/{relevant.Length}"),
            AssuranceGenerator.Row("Exempt:", units.Count(static unit => unit.IsExempt).ToString(CultureInfo.InvariantCulture)),
            AssuranceGenerator.Row("Human-reviewed:", $"{verified}/{relevant.Length}"),
            AssuranceGenerator.Row("IP risk:", Weakest(assessed, "IP", AssuranceAnnotation.IpRiskValues) ?? "not assessed"),
            AssuranceGenerator.Row("Security risk:", Weakest(assessed, "Security", AssuranceAnnotation.SecurityRiskValues) ?? "not assessed"),
            AssuranceGenerator.Row("Criteria:", $"{Criteria(units)}/{RequiringACriterion(units)}"),
            AssuranceGenerator.Row("Resource impact:", scores.Length == 0 ? "not assessed" : $"{scores.Max()}/10 max"),
            AssuranceGenerator.Row(
                "Unverified:",
                relevant.Count(static unit => AssuranceStateMachine.BlocksRelease(unit.State))
                    .ToString(CultureInfo.InvariantCulture)),
            "//",
            AssuranceGenerator.GeneratedMarker,
        ];
    }

    /// <summary>Every line of a file's generated header that is not the line this shape declares.</summary>
    internal static List<string> FileHeaderViolations(
        string where,
        IReadOnlyList<string> header,
        IReadOnlyList<AssuranceUnit> units) =>
        Compare(where, ExpectedFileHeader(units), header);

    /// <summary>
    /// How many units carry a falsification criterion, counted from the LINE the block records
    /// rather than through the scanner's predicate: two expressions of one rule, so a defect in
    /// either is a disagreement rather than a shared answer.
    /// </summary>
    private static int Criteria(IEnumerable<AssuranceUnit> units) =>
        units.Count(static unit => unit.Annotation?.FalsifiedIfLine is not null);

    /// <summary>
    /// How many units owe one: the assessed units whose Security row is the top of its vocabulary,
    /// written out here rather than read from the scanner's list.
    /// </summary>
    private static int RequiringACriterion(IEnumerable<AssuranceUnit> units) => units.Count(static unit =>
        unit.Annotation is { ExemptReason: null } annotation &&
        (string.Equals(annotation.Field("Security"), "High", StringComparison.Ordinal) ||
         string.Equals(annotation.Field("Security"), "Critical", StringComparison.Ordinal)));

    /// <summary>How many of those carry none. The number rule J10 reports unit by unit.</summary>
    private static int MissingCriteria(IEnumerable<AssuranceUnit> units) => units.Count(static unit =>
        unit.Annotation is { ExemptReason: null, FalsifiedIfLine: null } annotation &&
        (string.Equals(annotation.Field("Security"), "High", StringComparison.Ordinal) ||
         string.Equals(annotation.Field("Security"), "Critical", StringComparison.Ordinal)));

    /// <summary>
    /// The weakest claim any assessed annotation makes for a field, or null when none makes one.
    /// </summary>
    /// <remarks>
    /// This is <see cref="AssuranceSummary"/>'s <c>Worst</c>, derived the other way round: it walks
    /// the vocabulary from its weakest-claim end and stops at the first value some annotation
    /// carries, where the generator ranks every annotation and takes a maximum. Two expressions of
    /// one rule, so a defect in either is a disagreement rather than a shared answer - which is the
    /// whole of what "check the derivation" can mean for a value both sides read from the same
    /// annotations.
    /// </remarks>
    private static string? Weakest(
        IReadOnlyList<AssuranceAnnotation> assessed,
        string field,
        IReadOnlyList<string> vocabulary)
    {
        for (var index = vocabulary.Count - 1; index >= 0; index--)
        {
            if (assessed.Any(annotation =>
                    string.Equals(annotation.Field(field), vocabulary[index], StringComparison.Ordinal)))
            {
                return vocabulary[index];
            }
        }

        return null;
    }

    // =============================================================================================
    // CODE-ASSURANCE.md
    // =============================================================================================

    /// <summary>
    /// The component report this shape declares for a tree of this size and these units.
    /// </summary>
    /// <remarks>
    /// Every fixed line is hand-copied; every value, table row and list entry is derived here. The
    /// comparison against the generated report is therefore a comparison against a second author,
    /// and a sentence the generator invents appears in one and not the other.
    /// </remarks>
    internal static string ExpectedReport(int filesScanned, IReadOnlyList<AssuranceUnit> units)
    {
        var relevant = units.Where(static unit => unit.IsRelevant).ToArray();
        var assessed = relevant
            .Where(static unit => unit.Annotation is { ExemptReason: null })
            .Select(static unit => unit.Annotation!)
            .ToArray();

        var scores = assessed
            .Select(static annotation =>
                int.TryParse(annotation.Field("Resources"), NumberStyles.None, CultureInfo.InvariantCulture, out var score)
                    ? score
                    : (int?)null)
            .Where(static score => score is not null)
            .Select(static score => score!.Value)
            .ToArray();

        var verified = relevant.Count(static unit => unit.State == AssuranceReviewState.Verified);
        var unverified = relevant.Count(static unit => AssuranceStateMachine.BlocksRelease(unit.State));
        var declared = AssuranceScanner.DeclaredExemptions(units);
        var report = new StringBuilder();

        Fixed(report,
            "# Broiler.VM Code Assurance",
            "",
            "GENERATED - DO NOT EDIT MANUALLY. Regenerate with",
            "`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,",
            "`HUMAN_REVIEW.md`, `assurance.manifest.json` and every generated source header from the",
            "product tree.",
            "");

        if (verified == 0)
        {
            Fixed(report,
                "**Nothing in this component has been reviewed by a human.** This report records that",
                "absence precisely. It is not a claim that the code is reviewed, assured or safe, and the",
                "figures below are the measurement of how far from that claim the component is.");
        }
        else
        {
            Line(
                report,
                $"**Human-reviewed: {verified} of {relevant.Length} relevant units.** This report records what the");

            Fixed(report,
                "annotations state and no more. A decision recorded here is one person's, bound to one",
                "version of one declaration, and it is not a claim that the code is assured or safe.");
        }

        Fixed(report,
            "",
            "## Summary",
            "",
            "| Metric | Value |",
            "|---|---:|");

        Line(report, $"| Files scanned | {filesScanned} |");
        Line(report, $"| Files carrying an annotation | {AnnotatedFiles(units)} |");
        Line(report, $"| Code units | {units.Count} |");
        Line(report, $"| Relevant | {relevant.Length} |");
        Line(report, $"| Exempt by predicate | {units.Count(static unit => unit.IsExempt)} |");
        Line(report, $"| Annotated | {Portion(assessed.Length, relevant.Length)} |");
        Line(report, $"| Human reviewed | {Portion(verified, relevant.Length)} |");
        Line(report, $"| Unverified | {unverified} |");

        Fixed(report,
            "",
            "## Review states",
            "",
            "| State | Count |",
            "|---|---:|");

        foreach (var state in Enum.GetValues<AssuranceReviewState>())
        {
            Line(report, $"| {AssuranceStateMachine.Name(state)} | {units.Count(unit => unit.State == state)} |");
        }

        Line(report, string.Empty);
        Distribution(report, "IP risk", "IP", AssuranceAnnotation.IpRiskValues, units, relevant.Length - assessed.Length);
        Distribution(report, "Security risk", "Security", AssuranceAnnotation.SecurityRiskValues, units, relevant.Length - assessed.Length);

        Fixed(report,
            "## Resource impact",
            "",
            "| Metric | Value |",
            "|---|---:|");

        Line(report, $"| Maximum | {(scores.Length == 0 ? "n/a" : $"{scores.Max()} / 10")} |");
        Line(
            report,
            "| Average over annotated units | " +
            (scores.Length == 0
                ? "n/a"
                : scores.Average().ToString("0.0", CultureInfo.InvariantCulture) + " / 10") +
            " |");
        Line(report, $"| Units scored | {assessed.Length} |");

        Fixed(report,
            "",
            "## High-security review areas",
            "");

        var high = units
            .Where(static unit => unit.Annotation?.Field("Security") is "High" or "Critical")
            .ToArray();

        if (high.Length == 0)
        {
            Fixed(report,
                "No annotated unit is assessed High or Critical. This says nothing about the units nothing",
                "has assessed.",
                "");
        }
        else
        {
            foreach (var unit in high)
            {
                Line(
                    report,
                    $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
                    $"Security={unit.Annotation!.Field("Security")}, " +
                    $"human line {AssuranceHumanReview.HumanLine(unit)}");
            }

            Line(report, string.Empty);
        }

        Fixed(report,
            "## Falsification criteria",
            "",
            "| Metric | Value |",
            "|---|---:|");

        Line(report, $"| Units carrying a criterion | {Criteria(units)} |");
        Line(report, $"| Units required to carry one | {RequiringACriterion(units)} |");
        Line(report, $"| Required and missing | {MissingCriteria(units)} |");

        Fixed(report,
            "",
            "A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make",
            "the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the",
            "criterion is the test. It is required where `Security` is `High` or `Critical`, permitted",
            "elsewhere, and rule J10 names every unit that owes one and carries none.",
            "",
            "The line is a comment, so it is outside every fingerprint by construction: rewording a",
            "criterion moves no recorded value here, in a file header or in");

        Line(
            report,
            $"`{AssuranceManifest.RelativePath}`, and invalidates nothing. That is the intended reading - a");

        Fixed(report,
            "criterion is an instruction to whoever reads the unit, not part of what a review is bound to.",
            "",
            "This third line is a local extension. The owner's policy defines two lines and not three,",
            "and it is added here because the two cannot carry a falsification criterion at all, and",
            "because the line numbers a separate worksheet cited rotted the moment the annotations moved",
            "the code: an annotation travels with its declaration and a citation does not. Exclusion",
            "EX-74 records that this is an extension to the policy rather than an implementation of it,",
            "and that the owner may reject it.",
            "");

        Fixed(report,
            "## Exemption",
            "",
            "Exemption is decided by one predicate in `AssuranceScanner.ExemptionFor`, not per unit, so",
            "that the rule is reviewable in one place rather than in several hundred.",
            "",
            "| Case | Units |",
            "|---|---:|");

        foreach (var exemption in Enum.GetValues<AssuranceExemption>()
                     .Where(static value => value != AssuranceExemption.None))
        {
            Line(report, $"| {exemption} | {units.Count(unit => unit.Exemption == exemption)} |");
        }

        Fixed(report,
            "",
            "## Per-unit exemptions",
            "",
            "| Metric | Value |",
            "|---|---:|");

        Line(report, $"| Per-unit exemptions | {declared.Count} |");

        Fixed(report,
            "",
            "A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the",
            "predicate cannot see. Nothing mechanical checks that the reason is true, that it describes",
            "the unit it sits on, or that it says anything at all, so every use is counted and named");

        Line(
            report,
            $"here. `{string.Join("`, `", AssuranceScanner.AssembliesClosedToTheEscapeHatch)}` is closed to it entirely: that assembly reads untrusted");

        Fixed(report,
            "input, and a unit there is assessed or it is not shipped. Rule J1 asserts both halves.",
            "");

        if (declared.Count == 0)
        {
            Fixed(report, "No unit in this component states a per-unit exemption.", "");
        }
        else
        {
            foreach (var unit in declared)
            {
                Line(report, $"- `{unit.Name}` in `{unit.File.RelativePath}` - {unit.Annotation!.ExemptReason}");
            }

            Line(report, string.Empty);
        }

        Fixed(report,
            "## Change detection",
            "");

        Line(
            report,
            $"`{AssuranceManifest.RelativePath}` lists **every** code unit in the three product assemblies -");
        Line(report, $"{units.Count} of them, exempt and relevant alike - with the fingerprint of its declaration.");
        Line(report, $"{AssuranceManifest.ChangeDetectionStatement} A unit listed there is watched, not reviewed:");

        Fixed(report,
            "the entry records what the declaration's tokens hashed to when the generator last ran, and",
            "nothing else. Exempt units still need no annotation and carry none, and no human line in",
            "this component has moved off `PENDING`. What the manifest adds is that a unit the exemption",
            "predicate treats as trivial is no longer invisible: a semantic change to one moves a value",
            "in a generated file the gate compares byte for byte. Rule J7 holds the manifest to the tree.",
            "");

        Line(report, $"Beside the units it lists **every covered file** - {filesScanned} of them - with a");

        Fixed(report,
            "fingerprint over the complete token stream of its compilation unit. A unit entry exists only",
            "for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an",
            "`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.");

        Line(report, $"{AssuranceManifest.CompletenessStatement} Comments are outside the stream, because a token's");

        Fixed(report,
            "text is its own characters, so the generated header above and the annotation lines below move",
            "no file fingerprint - which is what lets one generation be a fixed point.",
            "",
            "## Verification",
            "",
            "The generator and the gate are the same code, run as a test in the architecture suite. Two",
            "lanes under `.github/workflows/` compel it rather than leaving it to whoever remembers: the",
            "review lane regenerates every artefact on a pull request and commits what moved, and the",
            "publish lane runs the release mode below and refuses to pack while anything is unresolved.",
            "Exclusion EX-45 still records one RID and one machine for the Native AOT evidence, which no",
            "lane reproduces.",
            "",
            "| Mode | Command | Effect |",
            "|---|---|---|");

        Line(
            report,
            $"| Generate | `{AssuranceGenerator.WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `{AssuranceHumanReview.RelativePath}`, `{AssuranceManifest.RelativePath}` and this file. |");

        Fixed(report,
            "| Gate | `dotnet test Broiler.VM.slnx -c Release` | Asserts every generated artefact is byte-identical to what the generator would produce. |");

        Line(
            report,
            $"| Release | `{AssuranceRelease.GateVariable}=1 dotnet test Broiler.VM.slnx -c Release` | The gate, and additionally: no relevant unit left in a state that blocks a release, no annotation this system cannot read, no fingerprint out of date, no unit at the top of the security vocabulary without a criterion. |");

        Fixed(report,
            "",
            "The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token",
            "texts, joined by single spaces. Trivia is excluded because a token's text is its own",
            "characters and never the comments or whitespace around it, so `dotnet format` moves no",
            "fingerprint and an annotation is never part of what it describes. The value answers whether a",
            "unit changed since it was reviewed. It is not a collision-free identifier across units and it",
            "is not a cryptographic commitment.");

        return report.ToString();
    }

    /// <summary>Every line of a component report that is not the line this shape declares.</summary>
    internal static List<string> ReportViolations(
        string report,
        int filesScanned,
        IReadOnlyList<AssuranceUnit> units) =>
        Compare(
            AssuranceGenerator.ReportPath,
            new AssuranceTextLines(ExpectedReport(filesScanned, units)),
            new AssuranceTextLines(report));

    private static void Fixed(StringBuilder report, params string[] lines)
    {
        foreach (var line in lines)
        {
            Line(report, line);
        }
    }

    private static void Line(StringBuilder report, string line) => report.Append(line).Append('\n');

    private static void Distribution(
        StringBuilder report,
        string heading,
        string field,
        IReadOnlyList<string> vocabulary,
        IReadOnlyList<AssuranceUnit> units,
        int notAnnotated)
    {
        Fixed(report, $"## {heading}", "", "| Value | Units |", "|---|---:|");

        foreach (var value in vocabulary)
        {
            Line(
                report,
                $"| {value} | {units.Count(unit => string.Equals(unit.Annotation?.Field(field), value, StringComparison.Ordinal))} |");
        }

        Line(report, $"| *not annotated* | {notAnnotated} |");
        Line(report, string.Empty);
    }

    private static int AnnotatedFiles(IEnumerable<AssuranceUnit> units) => units
        .Where(static unit => unit.Annotation is not null)
        .Select(static unit => unit.File.RelativePath)
        .Distinct(StringComparer.Ordinal)
        .Count();

    private static string Portion(int part, int whole) => whole == 0
        ? $"{part}"
        : $"{part} of {whole} ({(int)Math.Round(100.0 * part / whole)}%)";

    // =============================================================================================
    // HUMAN_REVIEW.md
    // =============================================================================================

    /// <summary>
    /// The human-review record this shape declares for these files and these units.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same mechanism as the report above, applied to the document that decides a release. It
    /// matters more here than anywhere else in this file: <c>CODE-ASSURANCE.md</c> is a measurement
    /// and this is the record a publish is refused on, so a sentence invented in the generator would
    /// be a sentence about who has read what, standing in the one place a reader goes to find out.
    /// </para>
    /// <para>
    /// Every count below is derived again rather than copied, and by a different expression: the
    /// generator asks <see cref="AssuranceSummary"/>, and this counts the units directly. The alias
    /// rows are rebuilt here from the human lines rather than read from
    /// <see cref="AssuranceHumanReview.Reviewers"/>, because those rows ARE the claim this document
    /// makes about people and they are the last thing that should have one author.
    /// </para>
    /// </remarks>
    internal static string ExpectedHumanReview(
        IReadOnlyList<AssuranceSourceFile> files,
        IReadOnlyList<AssuranceUnit> units)
    {
        var relevant = units.Count(static unit => !unit.IsExempt);
        var exempt = units.Count(static unit => unit.Exemption != AssuranceExemption.None);
        var assessed = units.Count(static unit =>
            !unit.IsExempt && unit.Annotation is { ExemptReason: null });
        var reviewed = units.Count(static unit =>
            !unit.IsExempt && unit.State == AssuranceReviewState.Verified);
        // AssuranceStateMachine.BlocksRelease written out rather than called, so the two
        // derivations stay two. The second half is redundant today and is kept deliberately: a
        // relevant unit cannot be in state EXEMPT, because the one thing that produces that state
        // for an annotated unit - a per-unit `EXEMPT=` reason - is also what makes
        // AssuranceScanner.ExemptionFor answer DeclaredInSource. Writing only the first half would
        // be relying on that coincidence between two files, which is the kind of agreement this
        // shape exists to stop depending on.
        var unverified = units.Count(static unit =>
            !unit.IsExempt && unit.State is not (AssuranceReviewState.Verified or AssuranceReviewState.Exempt));
        var aliases = Aliases(units);
        var record = new StringBuilder();

        Fixed(record,
            "# Human Review: Broiler.VM",
            "",
            "GENERATED - DO NOT EDIT MANUALLY. Regenerate with",
            "`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,",
            "`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the",
            "product tree.",
            "");

        Line(
            record,
            $"> **Status: {Status(reviewed, relevant)}.** Human-reviewed: {reviewed} of {relevant} relevant units. No package");

        Fixed(record,
            "> may be published from this component, no RID claimed and no milestone accepted until every",
            "> relevant unit carries a decision, which is update rule 8 in the status ledger.",
            "",
            "## 1. How To Use This File",
            "",
            "This section is the canonical mark legend for the component. The evidence bundles and the",
            "status ledger link here rather than repeating the tables. There are two vocabularies, they are",
            "different kinds of thing, and they must never be mixed. Both are closed sets, and rule H1",
            "refuses a mark in any review document that this section does not publish.",
            "",
            "### Evidence verdicts - stated about a piece of evidence",
            "",
            "| Mark | Meaning |",
            "|---|---|",
            "| `[MET]` | Demonstrated. An execution, artefact or log in a retained bundle shows it. |",
            "| `[PART]` | Partly demonstrated. What is not shown is named on the same row. |",
            "| `[UNMET]` | Not discharged. The condition is stated and not satisfied. |",
            "| `[N/A]` | Not claimed at this milestone. The milestone that owns it is named. |",
            "",
            "### Review verdicts - stated in an evidence bundle about a gate clause",
            "",
            "| Mark | Meaning |",
            "|---|---|",
            "| `[ ]` | Not yet read. |",
            "| `[A]` | Accepted as stated. |",
            "| `[C]` | Accepted with a condition. The condition is recorded beside it. |",
            "| `[R]` | Rejected. The defect is recorded. |",
            "| `[?]` | Cannot be judged from what is here. What is missing is named. |",
            "",
            "**No verdict in this file is a mark.** A decision about a code unit is the",
            "`// Broiler-Human:` line on that unit's declaration, and every table below is read out of",
            "those lines. There is nothing here to fill in and nothing here to leave blank.",
            "",
            "## 2. How A Review Is Recorded",
            "",
            "In one place: the `// Broiler-Human:` line of the assurance annotation that sits on the",
            "declaration being read. Nothing in this file is edited by hand, no second document carries a",
            "per-item checklist, and no list of permitted aliases exists to be added to.",
            "",
            "```csharp",
            "// Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=630EF7",
            "// Broiler-Falsified-If: new T[] is reached before TryReserve returns true",
            "// Broiler-Human:        PENDING",
            "```",
            "",
            "The last line has four shapes. A human writes three of them; the generator writes the fourth",
            "and may never invent an alias, which rule J4 asserts in both directions.",
            "",
            "| Line | Meaning |",
            "|---|---|",
            "| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |",
            "| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |",
            "| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |",
            "| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |",
            "",
            "A human may state their own `IP=`, `Security=` and `Resources=` assessment beside their alias,",
            "which is how a reader disagrees with the machine assessment on the line above: an assessment is",
            "a comment and moves no fingerprint, so there is nowhere else to say it.",
            "",
            "**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of",
            "the declaration it was made against, and the state machine compares that value with the",
            "declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit",
            "did, which is the narrower and the more useful of the two.",
            "",
            "This file is produced on every pull request by the review lane in `.github/workflows/`, and the",
            "publish lane refuses to run while any relevant unit is unresolved, any fingerprint is out of",
            "date, any annotation is malformed or any generated artefact is stale.",
            "",
            "## 3. Summary",
            "",
            "| Metric | Value |",
            "|---|---:|");

        Line(record, $"| Files scanned | {files.Count} |");
        Line(record, $"| Code units | {units.Count} |");
        Line(record, $"| Relevant | {relevant} |");
        Line(record, $"| Exempt | {exempt} |");
        Line(record, $"| Assessed | {Portion(assessed, relevant)} |");
        Line(record, $"| Human reviewed | {Portion(reviewed, relevant)} |");
        Line(record, $"| Unverified | {unverified} |");
        Line(record, $"| Aliases naming a decision | {aliases.Count} |");

        Fixed(record,
            "",
            "## 4. Review States",
            "",
            "One row per state of the machine that reads the two lines. The states are computed from the",
            "annotations and the current fingerprints; nothing stores them.",
            "",
            "| State | Units |",
            "|---|---:|");

        foreach (var state in Enum.GetValues<AssuranceReviewState>())
        {
            Line(record, $"| {AssuranceStateMachine.Name(state)} | {units.Count(unit => unit.State == state)} |");
        }

        Fixed(record, "", "## 5. Aliases In The Tree", "");

        if (aliases.Count == 0)
        {
            Fixed(record,
                "No alias appears on a human line anywhere in the product tree. Nobody has recorded a",
                "decision about any unit of this component.",
                "");
        }
        else
        {
            Fixed(record,
                "Read out of the human lines, never registered. `Current` counts the units whose decision",
                "names the fingerprint the declaration carries now; `Outrun` counts the units whose",
                "declaration has changed since.",
                "",
                "| Alias | Units | Files | Current | Outrun |",
                "|---|---:|---:|---:|---:|");

            foreach (var alias in aliases)
            {
                var owned = units.Where(unit => string.Equals(
                    AssuranceHumanReview.AliasOn(unit), alias, StringComparison.Ordinal)).ToArray();

                Line(
                    record,
                    $"| {alias} | {owned.Length} | " +
                    $"{owned.Select(static unit => unit.File.RelativePath).Distinct(StringComparer.Ordinal).Count()} | " +
                    $"{owned.Count(static unit => unit.State == AssuranceReviewState.Verified)} | " +
                    $"{owned.Count(static unit => unit.State == AssuranceReviewState.Stale)} |");
            }

            Line(record, string.Empty);
        }

        Fixed(record,
            "## 6. Coverage By File",
            "",
            "One row per covered file, carrying that file's generated header. `Unverified` counts the",
            "relevant units in a state that blocks a release.",
            "",
            "| File | Units | Relevant | Exempt | Unverified | IP risk | Security risk | Criteria |",
            "|---|---:|---:|---:|---:|---|---|---:|");

        foreach (var file in files)
        {
            var owned = units
                .Where(unit => string.Equals(unit.File.RelativePath, file.RelativePath, StringComparison.Ordinal))
                .ToArray();

            var scored = owned
                .Where(static unit => !unit.IsExempt && unit.Annotation is { ExemptReason: null })
                .Select(static unit => unit.Annotation!)
                .ToArray();

            Line(
                record,
                $"| `{file.RelativePath}` | {owned.Length} | " +
                $"{owned.Count(static unit => !unit.IsExempt)} | " +
                $"{owned.Count(static unit => unit.IsExempt)} | " +
                $"{owned.Count(static unit => !unit.IsExempt && unit.State is not (AssuranceReviewState.Verified or AssuranceReviewState.Exempt))} | " +
                $"{Weakest(scored, "IP", AssuranceAnnotation.IpRiskValues) ?? "not assessed"} | " +
                $"{Weakest(scored, "Security", AssuranceAnnotation.SecurityRiskValues) ?? "not assessed"} | " +
                $"{Criteria(owned)}/{RequiringACriterion(owned)} |");
        }

        Fixed(record, "", "## 7. Decisions Recorded", "");

        var decided = units
            .Where(static unit => unit.State is AssuranceReviewState.Verified
                or AssuranceReviewState.HumanApprovedPendingFingerprint)
            .ToArray();

        if (decided.Length == 0)
        {
            Fixed(record,
                "No unit in this component carries a decision on its human line. Every one of them reads",
                "`PENDING`.",
                "");
        }
        else
        {
            Fixed(record,
                "One entry per unit whose human line names an alias, with the line exactly as the source",
                "states it.",
                "");

            foreach (var unit in decided)
            {
                Line(
                    record,
                    $"- `{unit.Name}` in `{unit.File.RelativePath}` - {AssuranceHumanReview.HumanLine(unit)}");
            }

            Line(record, string.Empty);
        }

        Fixed(record, "## 8. Decisions The Code Has Outrun", "");

        var outrun = units.Where(static unit => unit.State == AssuranceReviewState.Stale).ToArray();

        if (outrun.Length == 0)
        {
            Fixed(record, "No unit carries a decision that the code has since moved past.", "");
        }
        else
        {
            Fixed(record,
                "The declaration changed after the decision was recorded. The alias and the version it was",
                "recorded against are preserved rather than deleted, because that is more useful than a",
                "blank line. Only a human clears one.",
                "");

            foreach (var unit in outrun)
            {
                Line(
                    record,
                    $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
                    $"{AssuranceHumanReview.HumanLine(unit)}, now `{unit.Fingerprint}`");
            }

            Line(record, string.Empty);
        }

        Fixed(record, "## 9. Where A Decision Is Required First", "");

        var required = units
            .Where(static unit => unit.Annotation?.Field("Security") is "High" or "Critical")
            .ToArray();

        if (required.Length == 0)
        {
            Fixed(record,
                "No unit is assessed `High` or `Critical`. This says nothing about the units nothing has",
                "assessed.",
                "");
        }
        else
        {
            Fixed(record,
                "The units at the top of the security vocabulary, with the observation that would show each",
                "one wrong and the human line it carries. The set is read from the assessments rather than",
                "written out, so a unit that becomes `High` joins it at the next generation.",
                "");

            foreach (var unit in required)
            {
                Line(
                    record,
                    $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
                    $"Security={unit.Annotation!.Field("Security")}, " +
                    $"Spec={unit.Annotation!.Field("Spec") ?? "none cited"}, " +
                    $"`{unit.Fingerprint}`, " +
                    $"{AssuranceHumanReview.HumanLine(unit)}");

                Line(
                    record,
                    $"  - Falsified if: {unit.Annotation!.FalsifiedIf ?? "no criterion is stated"}");
            }

            Line(record, string.Empty);
        }

        Fixed(record,
            "## 10. What This Record Does Not Say",
            "",
            "It is not an approval of the component, and a full table above would not be one either. It",
            "records which declarations somebody stated a decision about, and against which version of",
            "each. It does not record what they read, how long they spent, or whether they were right.",
            "",
            "Broiler.VM has one person in every role: architecture owner, core-contract owner, security",
            "owner and reader are the same individual, so **no second pair of eyes has seen this work.**",
            "That is a property of the project's size rather than a defect in this component, and it is why",
            "the tables above have room for as many aliases as the tree names rather than one signature",
            "line.",
            "",
            "A fingerprint is six hex characters of SHA-256 over a declaration's token texts. It answers",
            "whether a unit changed since a decision was recorded against it. It is not a collision-free",
            "identifier across units and it is not a cryptographic commitment, so it detects a change and",
            "does not resist a forger with commit access.",
            "",
            "The assessments the decisions are recorded beside are machine-written and unread: an",
            "assessment is a comment, so downgrading one moves no fingerprint anywhere, which exclusions",
            "EX-65 and EX-76 record.",
            "");

        // The provenance figure is derived here from the Origin field directly, where the generator
        // asks AssuranceHumanReview.Provenance. It is the caveat the old record carried as an
        // attention item a reader had to be pointed at, and a caveat nobody has to remember to keep
        // true is worth two derivations.
        Line(
            record,
            $"That is not a figure of speech. {units.Count(static unit => !unit.IsExempt && unit.Annotation is { ExemptReason: null } annotation && string.Equals(annotation.Field("Origin"), "AI", StringComparison.Ordinal))} of the {assessed} assessed units declare");

        Fixed(record,
            "`Origin=AI`, and the records this component implements were drafted the same way. An",
            "adversarial pass over the work confirmed findings and they were corrected, which is a check",
            "on it and not an independent judgement of it. Reading a declaration is the only thing that",
            "makes it read.");

        return record.ToString();
    }

    /// <summary>
    /// The status word the states give, derived here by comparison rather than by the generator's
    /// conditional chain.
    /// </summary>
    private static string Status(int reviewed, int relevant)
    {
        if (reviewed == 0)
        {
            return "PENDING";
        }

        return reviewed == relevant ? "COMPLETE" : "PARTIAL";
    }

    /// <summary>Every alias any human line names, in ordinal order, counted from the lines.</summary>
    private static IReadOnlyList<string> Aliases(IEnumerable<AssuranceUnit> units) => units
        .Select(AssuranceHumanReview.AliasOn)
        .Where(static alias => alias is not null)
        .Select(static alias => alias!)
        .Distinct(StringComparer.Ordinal)
        .OrderBy(static alias => alias, StringComparer.Ordinal)
        .ToArray();

    /// <summary>Every line of the human-review record that is not the line this shape carries.</summary>
    internal static List<string> HumanReviewViolations(
        string record,
        IReadOnlyList<AssuranceSourceFile> files,
        IReadOnlyList<AssuranceUnit> units) =>
        Compare(
            AssuranceHumanReview.RelativePath,
            new AssuranceTextLines(ExpectedHumanReview(files, units)),
            new AssuranceTextLines(record));

    // =============================================================================================
    // The comparison
    // =============================================================================================

    /// <summary>
    /// Every line at which a generated artefact and the shape declared for it disagree, each named
    /// with both texts.
    /// </summary>
    /// <remarks>
    /// Every differing line is reported and not only the first, because the fact a reader needs is
    /// WHICH sentence the generator invented, and an artefact that gained one sentence disagrees
    /// from there to its end. The messages name both sides so that the reader can tell an invented
    /// sentence from a shape that has fallen behind an intended edit.
    /// </remarks>
    private static List<string> Compare(string where, IReadOnlyList<string> declared, IReadOnlyList<string> actual)
    {
        var violations = new List<string>();

        for (var line = 0; line < Math.Max(declared.Count, actual.Count); line++)
        {
            var expected = line < declared.Count ? declared[line] : "<end of artefact>";
            var found = line < actual.Count ? actual[line] : "<end of artefact>";

            if (!string.Equals(expected, found, StringComparison.Ordinal))
            {
                violations.Add(
                    $"{where}({line + 1}) is not the line the declared shape carries." +
                    $"\n  generated: {found}" +
                    $"\n  declared:  {expected}");
            }
        }

        return violations;
    }
}
