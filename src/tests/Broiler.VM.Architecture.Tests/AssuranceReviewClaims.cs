using System.Globalization;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One piece of generated text, with a name a violation can carry.</summary>
internal sealed record AssuranceGeneratedText(string Where, IReadOnlyList<string> Lines);

/// <summary>
/// The policy's hardest rule, applied to the generated artefacts rather than to the source lines:
/// no generated text may say a unit is reviewed, approved or releasable while the annotations say
/// no such thing.
/// </summary>
/// <remarks>
/// <para>
/// <b>What this answers.</b> Rule J4 holds every human line in the tree, and every human line in an
/// artefact, to <c>PENDING</c>. It says nothing about the artefacts' PROSE - and prose is what a
/// reader trusts. Two lines appended to the manifest's header made
/// <c>assurance.manifest.json</c> assert <c>reviewState=VERIFIED</c>, <c>humanReviewed=true</c>,
/// <c>reviewer=&lt;name&gt;</c> for every entry and state that the component was eligible for
/// release, with both modes of the gate green and every source human line still reading
/// <c>PENDING</c>. Nothing in the suite read that sentence at all.
/// </para>
/// <para>
/// <b>How it is decided, and why it is not a whitelist of sentences.</b> Every occurrence of a
/// review term in generated text has to be one the ANNOTATIONS SUPPORT, and there are exactly two
/// ways to be that. The first is a COUNT the annotations give: the number the line states after the
/// term must equal the number the annotations produce for it, which is what makes
/// <c>| VERIFIED | 0 |</c> and <c>// Human-reviewed:   0/689</c> honest lines and
/// <c>reviewState=VERIFIED ... for all 1,592 units</c> a violation. The second is a statement of
/// ABSENCE: a negation standing before the term on the same line, which is what makes
/// <c>Nothing in this component has been reviewed by a human.</c> and <c>it is not an approval</c>
/// honest lines. A sentence that asserts a review and states neither a count nor a negation -
/// <c>eligible for release</c> is the whole of that attack - is reported with its text.
/// </para>
/// <para>
/// <b>The corpus is the GENERATED text and not the whole file.</b> For the report and the manifest
/// that is everything; for a product source file it is the generated header block alone. The rest
/// of a source file is the component's own code and comments, and <c>VmVerifiedArtifact</c> is a
/// public type of this component whose name appears in 112 lines below those headers. A rule that
/// read those would be reporting the component for having a domain.
/// </para>
/// <para>
/// <b>The limit.</b> The terms are a list, and a claim worded outside it is not seen - a generated
/// sentence saying a unit was "signed" or "cleared" passes. That is EX-71, and what covers it is
/// that rule J8 holds every generated line to a declared shape, so a new sentence of any wording
/// fails there. The two rules are deliberately independent: J8 can be defeated by editing the shape
/// as well as the generator, and this one cannot be defeated by editing anything but the terms.
/// </para>
/// </remarks>
internal static class AssuranceReviewClaims
{
    /// <summary>
    /// The vocabulary of a review claim, matched case-insensitively anywhere in a line.
    /// </summary>
    /// <remarks>
    /// <c>reviewed</c> on its own is deliberately absent and <c>reviewed by</c> is here instead:
    /// "the value answers whether a unit changed since it was reviewed" is a sentence about the
    /// fingerprint's purpose and not a claim that anything was reviewed, and a rule that could not
    /// tell the two apart would be answered by rewording rather than by telling the truth.
    /// </remarks>
    internal static readonly string[] Terms =
    [
        "verified",
        "approved",
        "approval",
        "reviewer",
        "reviewed by",
        "reviewed-by",
        "human reviewed",
        "human-reviewed",
        "humanreviewed",
        "eligible for release",
        "signed off",
        "sign-off",
        "certified",
        "attested",
    ];

    /// <summary>The words that turn a line carrying a review term into a statement of absence.</summary>
    /// <remarks>
    /// A negation counts only where it stands BEFORE the term on the same line, which is how
    /// English works and is what stops "verified, and nothing is pending" from reading as a denial.
    /// </remarks>
    private static readonly string[] Negations =
        ["no", "not", "nothing", "never", "none", "neither", "nobody", "absence", "unverified"];

    /// <summary>
    /// The generated text of every artefact in a plan: the whole of the report and the manifest,
    /// and the generated header block of every source file.
    /// </summary>
    internal static IReadOnlyList<AssuranceGeneratedText> GeneratedText(
        IEnumerable<AssuranceArtefact> artefacts) =>
        artefacts
            .Select(static artefact => new AssuranceGeneratedText(
                artefact.RelativePath,
                artefact.RelativePath.EndsWith(".cs", StringComparison.Ordinal)
                    ? AssuranceGenerator.GeneratedHeaderLines(artefact.Desired)
                    : new AssuranceTextLines(artefact.Desired)))
            .ToArray();

    /// <summary>
    /// Every line of generated text that states a review the annotations do not hold.
    /// </summary>
    internal static List<string> Violations(
        IEnumerable<AssuranceGeneratedText> texts,
        IReadOnlyList<AssuranceUnit> units)
    {
        var violations = new List<string>();

        foreach (var text in texts)
        {
            for (var line = 0; line < text.Lines.Count; line++)
            {
                var content = text.Lines[line];
                var lowered = content.ToLowerInvariant();

                foreach (var term in Terms)
                {
                    var at = WholeWord(lowered, term);

                    if (at < 0 || IsSupported(lowered, at, term, units))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{text.Where}({line + 1}) says '{content.Trim()}', and the annotations hold " +
                        $"no such state: the term '{term}' is stated with neither the count the " +
                        $"annotations give ({Supported(term, units)?.ToString(CultureInfo.InvariantCulture) ?? "none is defined for it"}) " +
                        "nor a negation before it");

                    break;
                }
            }
        }

        return violations;
    }

    /// <summary>
    /// Where a term first stands as a WHOLE WORD that is not the tail of a dotted name, or -1.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both halves are needed, and both were found by running the rule over the real artefacts.
    /// <c>VmVerifiedArtifact</c> is a public type of this component, and the manifest carries its
    /// name on 71 of its lines; a containment test reported every one of them, which would have made the
    /// rule a report on the component for having a domain. And the report's high-security list names
    /// <c>Broiler.VM.IVmVerifiedState</c>, which is a NAME the generator echoed out of the tree
    /// rather than a claim it is making.
    /// </para>
    /// <para>
    /// A term at the tail of a dotted path - <c>Broiler.VM.VmInstanceState.Verified</c> - is the
    /// same thing: a member the component declares. A claim is written as prose or as a field, and
    /// neither is preceded by a dot.
    /// </para>
    /// </remarks>
    private static int WholeWord(string lowered, string term)
    {
        for (var index = lowered.IndexOf(term, StringComparison.Ordinal);
             index >= 0;
             index = index + 1 <= lowered.Length
                 ? lowered.IndexOf(term, index + 1, StringComparison.Ordinal)
                 : -1)
        {
            var before = index == 0 || !char.IsAsciiLetter(lowered[index - 1]);
            var after = index + term.Length >= lowered.Length ||
                !char.IsAsciiLetter(lowered[index + term.Length]);

            if (before && after && (index == 0 || lowered[index - 1] != '.'))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>True when the annotations support this occurrence: by count, or by negation.</summary>
    private static bool IsSupported(string lowered, int at, string term, IReadOnlyList<AssuranceUnit> units)
    {
        if (Negations.Any(negation => IsWordBefore(lowered, negation, at)))
        {
            return true;
        }

        if (Supported(term, units) is not { } expected)
        {
            return false;
        }

        var stated = FirstNumberAfter(lowered, at + term.Length);

        return stated == expected;
    }

    /// <summary>
    /// True when a negation stands as a WHOLE WORD somewhere before <paramref name="at"/>.
    /// </summary>
    /// <remarks>
    /// Whole word and not containment: <c>cannot</c> ends in <c>not</c>, and a rule that read that
    /// as a denial would accept "the component cannot be built without a verified artifact" as a
    /// statement that nothing is verified.
    /// </remarks>
    private static bool IsWordBefore(string lowered, string word, int at)
    {
        // Only within the SAME clause. A negation searched across the whole line let one leading
        // clause launder every claim after it - "No unit is outside this record: every unit below
        // is verified, human-reviewed and approved" read as a denial and passed. A denial has to
        // stand in the clause it denies.
        var clause = ClauseStart(lowered, at);

        for (var index = lowered.IndexOf(word, clause, StringComparison.Ordinal);
             index >= 0 && index < at;
             index = lowered.IndexOf(word, index + 1, StringComparison.Ordinal))
        {
            var before = index == 0 || !char.IsAsciiLetter(lowered[index - 1]);
            var after = index + word.Length >= lowered.Length ||
                !char.IsAsciiLetter(lowered[index + word.Length]);

            if (before && after)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Where the clause containing <paramref name="at"/> begins: after the last clause separator
    /// standing before it, or at the start of the line.
    /// </summary>
    private static int ClauseStart(string lowered, int at)
    {
        var start = 0;

        for (var index = 0; index < at && index < lowered.Length; index++)
        {
            if (ClauseSeparators.Contains(lowered[index]))
            {
                start = index + 1;
            }
        }

        return start;
    }

    private static readonly char[] ClauseSeparators = [':', ';', '.'];

    /// <summary>
    /// The count the annotations give for a term, or null where the term names no countable state.
    /// </summary>
    /// <remarks>
    /// Every one of these is derived from the units rather than read out of the artefact, which is
    /// the point: the line is permitted because it states what the annotations state, and if the
    /// annotations ever hold an approval the same comparison keeps the artefacts honest about it
    /// instead of freezing a zero into the rule.
    /// </remarks>
    private static int? Supported(string term, IReadOnlyList<AssuranceUnit> units) => term switch
    {
        "verified" or "human reviewed" or "human-reviewed" or "humanreviewed" =>
            units.Count(static unit => unit.IsRelevant && unit.State == AssuranceReviewState.Verified),
        "approved" =>
            units.Count(static unit =>
                unit.State == AssuranceReviewState.HumanApprovedPendingFingerprint),
        _ => null,
    };

    /// <summary>The first run of digits at or after an index, or null when the line states none.</summary>
    private static int? FirstNumberAfter(string line, int index)
    {
        while (index < line.Length && !char.IsAsciiDigit(line[index]))
        {
            index++;
        }

        if (index >= line.Length)
        {
            return null;
        }

        var digits = new string(line[index..].TakeWhile(char.IsAsciiDigit).ToArray());

        return int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }
}
