namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The policy's review block, parsed: the AI line, an optional falsification criterion, and the
/// human line.
/// </summary>
/// <remarks>
/// <para>
/// The block is two or three adjacent single-line comments immediately above the declaration, at
/// the declaration's own indentation:
/// </para>
/// <code>
/// // Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=TBF
/// // Broiler-Falsified-If: any path reaches new T[] before meter.TryReserve has returned true
/// // Broiler-Human:        PENDING
/// </code>
/// <para>
/// The three labels are padded to one width, so the values start in one column, which is what the
/// policy's examples show for the two lines it defines.
/// </para>
/// <para>
/// The AI line is a semicolon-separated field list. The human line is not: it is one of four
/// literal shapes, because a human line is a decision rather than a record, and the shapes are
/// what the state machine reads. <c>PENDING</c> is the only shape anything in this component
/// carries, and the only one the generator may leave in place unchanged.
/// </para>
/// <para>
/// <b>The middle line is neither.</b> A falsification criterion is prose: one sentence naming the
/// observation that would make this unit wrong. <c>Security=High</c> says that a unit is risky and
/// a set of 44 units is not a test; <em>any path sizes a buffer before <c>TryReserve</c> returns
/// true</em> is a test. It is REQUIRED where <c>Security</c> is <c>High</c> or <c>Critical</c> -
/// rule J10 - and permitted anywhere else. It carries no field, because a field is data this
/// system would then have to define, check and summarize, and the value of the line is that it is
/// the sentence a reviewer reads.
/// </para>
/// <para>
/// <b>It is a comment, so it is outside every fingerprint by construction.</b> Changing a criterion
/// invalidates no review, which is correct: the criterion is an instruction to whoever reviews the
/// unit, not part of what they certify. Rule J10 asserts that in both directions.
/// </para>
/// </remarks>
internal sealed class AssuranceAnnotation
{
    internal const string AiMarker = "// Broiler-AI:";
    internal const string FalsifiedIfMarker = "// Broiler-Falsified-If:";
    internal const string HumanMarker = "// Broiler-Human:";

    /// <summary>
    /// The column every value starts in: the longest of the three labels, plus one space.
    /// </summary>
    /// <remarks>
    /// Derived from the marker rather than written as a number, so a label that changes length
    /// re-aligns the whole tree at the next generation instead of leaving 689 blocks half aligned.
    /// </remarks>
    internal static readonly int LabelWidth = FalsifiedIfMarker.Length;

    /// <summary>The human line every unit in this component carries. Nothing here is reviewed.</summary>
    internal const string Pending = "PENDING";

    /// <summary>The marker the generator writes when a reviewed unit has since changed.</summary>
    internal const string Stale = "STALE";

    private AssuranceAnnotation(
        int aiLine,
        int? falsifiedIfLine,
        int humanLine,
        IReadOnlyList<AssuranceField> fields,
        string? falsifiedIf,
        string humanBody)
    {
        AiLine = aiLine;
        FalsifiedIfLine = falsifiedIfLine;
        HumanLine = humanLine;
        Fields = fields;
        FalsifiedIf = falsifiedIf;
        HumanBody = humanBody;
    }

    /// <summary>Zero-based index of the AI line in its file.</summary>
    internal int AiLine { get; }

    /// <summary>
    /// Zero-based index of the falsification criterion, or null where the block carries none.
    /// </summary>
    internal int? FalsifiedIfLine { get; }

    /// <summary>
    /// Zero-based index of the human line: <see cref="AiLine"/> + 1, or + 2 where a criterion
    /// stands between them.
    /// </summary>
    internal int HumanLine { get; }

    /// <summary>The AI line's fields, in source order.</summary>
    internal IReadOnlyList<AssuranceField> Fields { get; }

    /// <summary>
    /// Everything after <c>// Broiler-Falsified-If:</c>, trimmed, or null where the block carries
    /// no criterion at all. An empty string is a criterion line that says nothing, which J2 reports.
    /// </summary>
    internal string? FalsifiedIf { get; }

    /// <summary>True when the block carries a falsification criterion line, empty or not.</summary>
    internal bool HasFalsificationCriterion => FalsifiedIf is not null;

    /// <summary>Everything after <c>// Broiler-Human:</c>, trimmed.</summary>
    internal string HumanBody { get; }

    internal string? Field(string key) => Fields
        .FirstOrDefault(field => string.Equals(field.Key, key, StringComparison.Ordinal))?.Value;

    /// <summary>The reason on an explicit per-unit exemption, or null when this is a full assessment.</summary>
    internal string? ExemptReason => Field("EXEMPT");

    internal string? RecordedFingerprint => Field("Fingerprint");

    // ---- The human line's four shapes --------------------------------------------------------

    /// <summary>No human has looked at this unit.</summary>
    internal bool HumanIsPending =>
        string.Equals(HumanBody, Pending, StringComparison.Ordinal);

    /// <summary>The generator has recorded that a review no longer describes the current code.</summary>
    internal bool HumanIsStale => HumanBody.StartsWith(Stale, StringComparison.Ordinal);

    /// <summary>
    /// The reviewer identifier a human wrote, or null when the line carries none. A <c>STALE</c>
    /// line's <c>Previous=</c> reviewer is history, not a live approval, and is not returned here.
    /// </summary>
    internal string? Reviewer
    {
        get
        {
            if (HumanIsPending || HumanIsStale || HumanBody.Length == 0)
            {
                return null;
            }

            var head = HumanBody.Split(';')[0].Trim();

            return head.Length == 0 ? null : head;
        }
    }

    /// <summary>The fingerprint the human line names, if any.</summary>
    internal string? HumanFingerprint
    {
        get
        {
            foreach (var part in HumanBody.Split(';', StringSplitOptions.TrimEntries))
            {
                if (part.StartsWith("Fingerprint=", StringComparison.Ordinal))
                {
                    return part["Fingerprint=".Length..];
                }
            }

            return null;
        }
    }

    /// <summary>The <c>Previous=reviewer@fingerprint</c> a stale line preserves.</summary>
    internal (string Reviewer, string Fingerprint)? Previous
    {
        get
        {
            foreach (var part in HumanBody.Split(';', StringSplitOptions.TrimEntries))
            {
                if (!part.StartsWith("Previous=", StringComparison.Ordinal))
                {
                    continue;
                }

                var value = part["Previous=".Length..];
                var at = value.LastIndexOf('@');

                return at < 0 ? (value, string.Empty) : (value[..at], value[(at + 1)..]);
            }

            return null;
        }
    }

    // ---- Parsing -----------------------------------------------------------------------------

    /// <summary>
    /// Reads the block whose AI line is at <paramref name="aiLine"/>. Returns null, with a reason,
    /// when the lines are not the shape the policy fixes.
    /// </summary>
    /// <remarks>
    /// The criterion line is optional and stands in exactly one place: between the AI line and the
    /// human line. A second one is refused here rather than accepted, because a criterion that does
    /// not fit on a line is too vague to be one - the line names an observation, and an observation
    /// that needs a paragraph is a concern.
    /// </remarks>
    internal static AssuranceAnnotation? TryParse(
        AssuranceText text,
        int aiLine,
        out string? problem)
    {
        problem = null;

        var ai = text[aiLine].Trim();

        if (!ai.StartsWith(AiMarker, StringComparison.Ordinal))
        {
            problem = $"line {aiLine + 1} does not open with '{AiMarker}'";
            return null;
        }

        var next = aiLine + 1;
        string? falsifiedIf = null;
        int? falsifiedIfLine = null;

        if (next < text.Count && text[next].Trim().StartsWith(FalsifiedIfMarker, StringComparison.Ordinal))
        {
            falsifiedIf = text[next].Trim()[FalsifiedIfMarker.Length..].Trim();
            falsifiedIfLine = next;
            next++;

            if (next < text.Count && text[next].Trim().StartsWith(FalsifiedIfMarker, StringComparison.Ordinal))
            {
                problem = $"line {next + 1} carries a second '{FalsifiedIfMarker}' line, and a " +
                    "falsification criterion is one line";
                return null;
            }
        }

        if (next >= text.Count)
        {
            problem = $"line {aiLine + 1} has no '{HumanMarker}' line under it";
            return null;
        }

        var human = text[next].Trim();

        if (!human.StartsWith(HumanMarker, StringComparison.Ordinal))
        {
            problem = $"line {aiLine + 1} is not immediately followed by a '{HumanMarker}' line";
            return null;
        }

        var fields = new List<AssuranceField>();

        foreach (var part in ai[AiMarker.Length..].Split(';'))
        {
            if (part.Trim().Length == 0)
            {
                continue;
            }

            var separator = part.IndexOf('=', StringComparison.Ordinal);

            if (separator < 0)
            {
                problem = $"line {aiLine + 1} has a field with no '=': '{part.Trim()}'";
                return null;
            }

            fields.Add(new AssuranceField(
                part[..separator].Trim(),
                part[(separator + 1)..].Trim()));
        }

        if (fields.Count == 0)
        {
            problem = $"line {aiLine + 1} carries no fields";
            return null;
        }

        return new AssuranceAnnotation(
            aiLine,
            falsifiedIfLine,
            next,
            fields,
            falsifiedIf,
            human[HumanMarker.Length..].Trim());
    }

    /// <summary>Renders one line of a block: the label padded to the shared value column.</summary>
    /// <remarks>
    /// A value that says nothing renders as the bare label rather than as the label and a run of
    /// spaces, so that a line the source left empty does not acquire trailing whitespace on its way
    /// through the generator.
    /// </remarks>
    private static string Render(string indent, string marker, string value) =>
        value.Length == 0 ? indent + marker : indent + marker.PadRight(LabelWidth) + " " + value;

    /// <summary>Renders the AI line, at the given indentation, with the given fields.</summary>
    internal static string RenderAiLine(string indent, IEnumerable<AssuranceField> fields) =>
        Render(indent, AiMarker, string.Join(
            "; ",
            fields.Select(static field => $"{field.Key}={field.Value}")));

    /// <summary>
    /// Renders the falsification criterion. The prose is carried through exactly as the source
    /// wrote it: the generator aligns this line and never authors one.
    /// </summary>
    internal static string RenderFalsifiedIfLine(string indent, string criterion) =>
        Render(indent, FalsifiedIfMarker, criterion);

    /// <summary>Renders the human line. The body is never invented; it is only ever carried over.</summary>
    internal static string RenderHumanLine(string indent, string body) =>
        Render(indent, HumanMarker, body);

    // ---- Closed vocabularies -----------------------------------------------------------------

    internal static readonly string[] OriginValues =
        ["Original", "AI", "Specification", "Derived", "Ported", "ThirdParty"];

    /// <summary>IP risk, weakest claim last: an unestablished provenance is not a good result.</summary>
    internal static readonly string[] IpRiskValues =
        ["None", "Low", "Medium", "High", "Unknown"];

    internal static readonly string[] SecurityRiskValues =
        ["None", "Low", "Medium", "High", "Critical"];

    /// <summary>The fields a full assessment must carry. <c>Spec</c> is optional.</summary>
    internal static readonly string[] RequiredFields =
        ["Origin", "IP", "Security", "Resources", "Fingerprint"];

    /// <summary>
    /// The shapes a falsification criterion may not have: a line that parsed, that says something,
    /// and that says it as prose rather than as data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The fourth clause of the line's well-formedness - that it is ONE line - is answered in
    /// <see cref="TryParse"/> rather than here, because a criterion wrapped onto a second line is
    /// not a block this can be asked about: the block does not parse at all, and J2 reports it
    /// through <c>AssuranceScanner.OrphanAnnotations</c> like any other unreadable block.
    /// </para>
    /// <para>
    /// <b>Why a field is refused.</b> A <c>Key=Value</c> pair on this line would be a claim the
    /// system does not define, does not check and does not summarize, sitting in the one place a
    /// reader will read it as though it were checked - which is the shape of every defect this
    /// register exists to prevent. The AI line is where data goes, and its field set is closed. The
    /// test is narrow on purpose: it is an identifier immediately followed by <c>=</c> and a value,
    /// so prose comparing values with <c>==</c>, <c>!=</c>, <c>&lt;=</c> or <c>&gt;=</c> is prose.
    /// </para>
    /// </remarks>
    internal IEnumerable<string> CriterionProblems()
    {
        if (FalsifiedIf is not { } criterion)
        {
            yield break;
        }

        if (criterion.Length == 0)
        {
            yield return $"{FalsifiedIfMarker} carries no criterion";
            yield break;
        }

        var field = FieldOnACriterion.Match(criterion);

        if (field.Success)
        {
            yield return
                $"{FalsifiedIfMarker} states the field {field.Value}, and a falsification " +
                "criterion is prose, not data";
        }

        // A criterion says what would make a unit WRONG. It can never say that somebody has
        // looked, and an attack showed one reading "this code was signed off by a person on
        // 2026-08-28 and needs no further reading" passing every rule: J9's corpus for a source
        // file is its generated header, so authored prose in the block was outside it. A criterion
        // is the one piece of authored text a reader meets at the declaration, so a review claim
        // written there is the cheapest false record in the system.
        var lowered = criterion.ToLowerInvariant();

        foreach (var term in AssuranceReviewClaims.Terms)
        {
            if (lowered.Contains(term, StringComparison.Ordinal))
            {
                yield return
                    $"{FalsifiedIfMarker} claims a review by saying '{term}', and a falsification " +
                    "criterion states what would make the unit wrong, never that anyone read it";
            }
        }
    }

    /// <summary>
    /// A <c>Key=Value</c> field: an identifier, an <c>=</c> that is not part of a comparison
    /// operator, and a value.
    /// </summary>
    private static readonly System.Text.RegularExpressions.Regex FieldOnACriterion =
        new(@"(?<![=!<>])\b[A-Za-z_][A-Za-z0-9_.-]*\s*=\s*(?![=])\S+", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// Every problem with this annotation's field values. Empty means well formed.
    /// </summary>
    internal IEnumerable<string> VocabularyProblems()
    {
        // The criterion is checked before anything else, because it is checked on every block:
        // an EXEMPT line may carry one, and a criterion that says nothing says nothing there too.
        foreach (var problem in CriterionProblems())
        {
            yield return problem;
        }

        if (ExemptReason is not null)
        {
            if (ExemptReason.Length == 0)
            {
                yield return "EXEMPT carries no reason";
            }

            if (Fields.Count > 1)
            {
                yield return "EXEMPT is stated beside other fields; an exemption is not an assessment";
            }

            yield break;
        }

        foreach (var required in RequiredFields)
        {
            if (Field(required) is null)
            {
                yield return $"no {required} field";
            }
        }

        foreach (var (key, value) in Fields.Select(static field => (field.Key, field.Value)))
        {
            var problem = key switch
            {
                "Origin" => Closed(value, OriginValues),
                "IP" => Closed(value, IpRiskValues),
                "Security" => Closed(value, SecurityRiskValues),
                "Resources" => int.TryParse(value, out var score) && score is >= 0 and <= 10
                    ? null
                    : "is not an integer 0 to 10",
                "Fingerprint" => string.Equals(value, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal) ||
                    AssuranceFingerprint.IsWellFormed(value)
                    ? null
                    : $"is neither {AssuranceFingerprint.ToBeFilled} nor six uppercase hex characters",
                "Spec" => value.Length == 0 ? "is empty" : null,
                _ => "is not a field this system defines",
            };

            if (problem is not null)
            {
                yield return $"{key}={value} {problem}";
            }
        }

        static string? Closed(string value, string[] allowed) =>
            allowed.Contains(value, StringComparer.Ordinal)
                ? null
                : $"is outside its vocabulary ({string.Join(", ", allowed)})";
    }
}

/// <summary>One <c>Key=Value</c> pair on the AI line.</summary>
internal sealed record AssuranceField(string Key, string Value);
