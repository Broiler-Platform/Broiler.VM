namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The policy's two-line review block, parsed.
/// </summary>
/// <remarks>
/// <para>
/// The block is exactly two adjacent single-line comments immediately above the declaration, at
/// the declaration's own indentation:
/// </para>
/// <code>
/// // Broiler-AI:    Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=TBF
/// // Broiler-Human: PENDING
/// </code>
/// <para>
/// Four spaces after <c>Broiler-AI:</c> and one after <c>Broiler-Human:</c>, so the two field
/// columns line up, which is what the policy's examples show.
/// </para>
/// <para>
/// The AI line is a semicolon-separated field list. The human line is not: it is one of four
/// literal shapes, because a human line is a decision rather than a record, and the shapes are
/// what the state machine reads. <c>PENDING</c> is the only shape anything in this component
/// carries, and the only one the generator may leave in place unchanged.
/// </para>
/// </remarks>
internal sealed class AssuranceAnnotation
{
    internal const string AiMarker = "// Broiler-AI:";
    internal const string HumanMarker = "// Broiler-Human:";

    /// <summary>The human line every unit in this component carries. Nothing here is reviewed.</summary>
    internal const string Pending = "PENDING";

    /// <summary>The marker the generator writes when a reviewed unit has since changed.</summary>
    internal const string Stale = "STALE";

    private AssuranceAnnotation(
        int aiLine,
        int humanLine,
        IReadOnlyList<AssuranceField> fields,
        string humanBody)
    {
        AiLine = aiLine;
        HumanLine = humanLine;
        Fields = fields;
        HumanBody = humanBody;
    }

    /// <summary>Zero-based index of the AI line in its file.</summary>
    internal int AiLine { get; }

    /// <summary>Zero-based index of the human line, always <see cref="AiLine"/> + 1.</summary>
    internal int HumanLine { get; }

    /// <summary>The AI line's fields, in source order.</summary>
    internal IReadOnlyList<AssuranceField> Fields { get; }

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
    /// when the two lines are not the shape the policy fixes.
    /// </summary>
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

        if (aiLine + 1 >= text.Count)
        {
            problem = $"line {aiLine + 1} has no '{HumanMarker}' line under it";
            return null;
        }

        var human = text[aiLine + 1].Trim();

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
            aiLine + 1,
            fields,
            human[HumanMarker.Length..].Trim());
    }

    /// <summary>Renders the AI line, at the given indentation, with the given fields.</summary>
    internal static string RenderAiLine(string indent, IEnumerable<AssuranceField> fields) =>
        indent + AiMarker + "    " + string.Join(
            "; ",
            fields.Select(static field => $"{field.Key}={field.Value}"));

    /// <summary>Renders the human line. The body is never invented; it is only ever carried over.</summary>
    internal static string RenderHumanLine(string indent, string body) =>
        indent + HumanMarker + " " + body;

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
    /// Every problem with this annotation's field values. Empty means well formed.
    /// </summary>
    internal IEnumerable<string> VocabularyProblems()
    {
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
