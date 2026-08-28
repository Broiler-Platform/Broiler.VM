// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   21
// Annotated:        21/21
// Exempt:           6
// Human-reviewed:   0/21
// IP risk:          Low
// Security risk:    High
// Resource impact:  2/10 max
// Unverified:       21
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// A validated VM profile identity: an ASCII, dot-separated, case-preserved token.
/// </summary>
/// <remarks>
/// <para>
/// The grammar is frozen by ADR 0002 at core contract version 1 as
/// <c>id := label ('.' label){1,7}</c>, <c>label := alnum ('-' alnum)*</c>, with the first
/// character of the first label an ASCII letter. Two to eight labels, one to sixty-four
/// characters per label, three to one hundred and twenty-eight characters per ID.
/// </para>
/// <para>
/// The bounds are the decision rather than decoration. An ID therefore contains no whitespace,
/// no path separator, no <c>:</c>, <c>*</c>, <c>?</c>, quote or angle bracket, no leading,
/// trailing or doubled hyphen, no empty label and no <c>..</c> sequence, so a host may use one
/// verbatim as a file-name component, a cache-key segment, a log field or an evidence-bundle key
/// without escaping.
/// </para>
/// <para>
/// <strong>Two comparison rules, deliberately different.</strong> Matching - descriptor to catalog
/// entry, handle-sharing identity, capability lookup - is ordinal and case-sensitive, because the
/// ID recorded in a handle and in an evidence bundle must be the ID the caller supplied.
/// Uniqueness - collision detection at registration - folds ASCII case, so a confusable pair is
/// caught at composition time instead of shadowing each other at run time. The ASCII-only grammar
/// makes that fold a pure byte operation: no ICU, no <c>CultureInfo</c>, no dependence on
/// <c>InvariantGlobalization</c>, and identical behaviour in JIT, trimmed and Native AOT hosts.
/// </para>
/// <para>
/// The core never lower-cases, upper-cases, trims or otherwise rewrites an ID it stores or echoes.
/// The reserved namespace is spelled <c>Broiler.*</c>, and a canonicalizing core would make its
/// own diagnostics disagree with the composition root that wrote them.
/// </para>
/// </remarks>
public readonly struct VmProfileId : System.IEquatable<VmProfileId>, System.IComparable<VmProfileId>
{
    /// <summary>The fewest labels a well-formed ID may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=None; Security=Medium; Resources=0; Fingerprint=A29732
    // Broiler-Human: PENDING
    public const int MinimumLabelCount = 2;

    /// <summary>The most labels a well-formed ID may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=None; Security=Medium; Resources=0; Fingerprint=C2441C
    // Broiler-Human: PENDING
    public const int MaximumLabelCount = 8;

    /// <summary>The most characters one label may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=None; Security=Medium; Resources=0; Fingerprint=30B94C
    // Broiler-Human: PENDING
    public const int MaximumLabelLength = 64;

    /// <summary>The fewest characters a well-formed ID may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=None; Security=Medium; Resources=0; Fingerprint=A1971C
    // Broiler-Human: PENDING
    public const int MinimumLength = 3;

    /// <summary>The most characters a well-formed ID may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=None; Security=Medium; Resources=0; Fingerprint=C4D2A4
    // Broiler-Human: PENDING
    public const int MaximumLength = 128;

    private readonly string? text;
    private readonly byte labelCount;

    private VmProfileId(string text, byte labelCount)
    {
        this.text = text;
        this.labelCount = labelCount;
    }

    /// <summary>
    /// True when this is <see langword="default"/>. Every core API rejects an empty ID as
    /// malformed, which is what stops an unvalidated value reaching a catalog entry or a lookup.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=Low; Resources=0; Fingerprint=173B23
    // Broiler-Human: PENDING
    public bool IsEmpty => text is null;

    /// <summary>The number of characters in the ID, or zero when empty.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0CF2CC
    // Broiler-Human: PENDING
    public int Length => text?.Length ?? 0;

    /// <summary>The number of dot-separated labels, or zero when empty.</summary>
    public int LabelCount => labelCount;

    /// <summary>
    /// True when the first label folds to <c>broiler</c>. The namespace is reserved for
    /// Broiler-owned profiles; an application-local profile uses a documented reverse-domain
    /// namespace of its own.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s2; IP=Low; Security=Low; Resources=1; Fingerprint=BF6977
    // Broiler-Human: PENDING
    public bool IsReservedNamespace
    {
        get
        {
            if (text is null)
            {
                return false;
            }

            var first = FirstLabel(text);

            return first.Length == 7 &&
                FoldAscii(first[0]) == 'b' && FoldAscii(first[1]) == 'r' && FoldAscii(first[2]) == 'o' &&
                FoldAscii(first[3]) == 'i' && FoldAscii(first[4]) == 'l' && FoldAscii(first[5]) == 'e' &&
                FoldAscii(first[6]) == 'r';
        }
    }

    /// <summary>The ID as a span, without allocating.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=864836
    // Broiler-Human: PENDING
    public System.ReadOnlySpan<char> AsSpan() => System.MemoryExtensions.AsSpan(text);

    /// <summary>
    /// Parses <paramref name="candidate"/>, returning <see langword="false"/> rather than throwing
    /// when it does not satisfy the frozen grammar.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=2; Fingerprint=745304
    // Broiler-Human: PENDING
    public static bool TryParse(System.ReadOnlySpan<char> candidate, out VmProfileId id)
    {
        id = default;

        if (!TryValidate(candidate, out var labels))
        {
            return false;
        }

        id = new VmProfileId(candidate.ToString(), labels);
        return true;
    }

    /// <summary>
    /// Parses <paramref name="candidate"/> or throws. Intended for composition-root literals,
    /// where a malformed ID is a programming error rather than untrusted input.
    /// </summary>
    /// <exception cref="System.ArgumentException">The candidate does not satisfy the grammar.</exception>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=2; Fingerprint=AA7040
    // Broiler-Human: PENDING
    public static VmProfileId Parse(System.ReadOnlySpan<char> candidate)
    {
        if (!TryParse(candidate, out var id))
        {
            throw new System.ArgumentException(
                "The value is not a well-formed VM profile ID: it must be two to eight " +
                "dot-separated ASCII labels, each one to sixty-four alphanumeric characters " +
                "with interior hyphens only, the first character an ASCII letter, three to one " +
                "hundred and twenty-eight characters in total.",
                nameof(candidate));
        }

        return id;
    }

    /// <summary>
    /// Ordinal, case-sensitive equality: the MATCHING rule. This is what a descriptor-to-catalog
    /// lookup, a handle-sharing identity check and an envelope dispatch use.
    /// </summary>
    public bool Equals(VmProfileId other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <summary>
    /// ASCII-folded equality: the UNIQUENESS rule, used only for collision detection at
    /// registration. It is deliberately a separate, named operation rather than an overload, so
    /// that no matching path can reach it by accident.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=Medium; Resources=1; Fingerprint=5DED8A
    // Broiler-Human: PENDING
    public static bool EqualsUnderAsciiFold(VmProfileId left, VmProfileId right)
    {
        if (left.text is null || right.text is null)
        {
            return left.text is null && right.text is null;
        }

        if (left.text.Length != right.text.Length)
        {
            return false;
        }

        for (var index = 0; index < left.text.Length; index++)
        {
            if (FoldAscii(left.text[index]) != FoldAscii(right.text[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Ordinal ordering, used to normalize catalog entries into a canonical order so that
    /// declaration order has no observable effect anywhere.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EA157F
    // Broiler-Human: PENDING
    public int CompareTo(VmProfileId other) =>
        string.CompareOrdinal(text ?? string.Empty, other.text ?? string.Empty);

    /// <summary>The ID verbatim. It is never trimmed, re-cased or otherwise rewritten.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=Low; Resources=0; Fingerprint=57BC75
    // Broiler-Human: PENDING
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmProfileId other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=Low; Resources=1; Fingerprint=6C9B84
    // Broiler-Human: PENDING
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal, case-sensitive equality.</summary>
    public static bool operator ==(VmProfileId left, VmProfileId right) => left.Equals(right);

    /// <summary>Ordinal, case-sensitive inequality.</summary>
    public static bool operator !=(VmProfileId left, VmProfileId right) => !left.Equals(right);

    /// <summary>
    /// The ASCII lower-case fold: <c>c | 0x20</c> applied only to <c>A</c> through <c>Z</c>.
    /// </summary>
    /// <remarks>
    /// A pure byte operation with no culture, no ICU and no globalization mode behind it, which is
    /// why it behaves identically in JIT, trimmed and Native AOT hosts. It is public because the
    /// uniqueness rule it implements has more than one legitimate caller: the catalog folds IDs to
    /// detect confusable pairs, and the reserved-namespace check folds a package ID the same way. A
    /// second, private copy of a one-line rule is how two callers come to disagree about it.
    /// </remarks>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=Medium; Resources=0; Fingerprint=89F121
    // Broiler-Human: PENDING
    public static char FoldAscii(char value) =>
        value is >= 'A' and <= 'Z' ? (char)(value | 0x20) : value;

    /// <summary>
    /// Validates the frozen grammar. Shared with <see cref="VmFeatureManifestId"/> and
    /// <see cref="VmCapabilityId"/>, whose own records say their policy mirrors this one exactly.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=2; Fingerprint=A4118A
    // Broiler-Human: PENDING
    internal static bool TryValidateGrammar(
        System.ReadOnlySpan<char> candidate,
        int minimumLabels,
        int maximumLabels,
        int minimumLength,
        int maximumLength,
        out byte labelCount)
    {
        labelCount = 0;

        if (candidate.Length < minimumLength || candidate.Length > maximumLength)
        {
            return false;
        }

        if (!IsAsciiLetter(candidate[0]))
        {
            return false;
        }

        var labels = 1;
        var labelLength = 0;
        var previousWasHyphen = false;

        for (var index = 0; index < candidate.Length; index++)
        {
            var current = candidate[index];

            if (current == '.')
            {
                // An empty label, a label ending in a hyphen, and a doubled dot are all this one
                // condition: the label just closed must be non-empty and must not end on a
                // separator.
                if (labelLength == 0 || previousWasHyphen)
                {
                    return false;
                }

                labels++;

                if (labels > maximumLabels)
                {
                    return false;
                }

                labelLength = 0;
                previousWasHyphen = false;
                continue;
            }

            if (current == '-')
            {
                // Interior only: never leading, never doubled. A trailing hyphen is caught by the
                // dot branch above and by the end-of-input check below.
                if (labelLength == 0 || previousWasHyphen)
                {
                    return false;
                }

                previousWasHyphen = true;
                labelLength++;

                if (labelLength > MaximumLabelLength)
                {
                    return false;
                }

                continue;
            }

            if (!IsAsciiAlphanumeric(current))
            {
                return false;
            }

            previousWasHyphen = false;
            labelLength++;

            if (labelLength > MaximumLabelLength)
            {
                return false;
            }
        }

        if (labelLength == 0 || previousWasHyphen || labels < minimumLabels)
        {
            return false;
        }

        labelCount = (byte)labels;
        return true;
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=2; Fingerprint=7D6A16
    // Broiler-Human: PENDING
    private static bool TryValidate(System.ReadOnlySpan<char> candidate, out byte labelCount) =>
        TryValidateGrammar(
            candidate,
            MinimumLabelCount,
            MaximumLabelCount,
            MinimumLength,
            MaximumLength,
            out labelCount);

    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=0; Fingerprint=C1992B
    // Broiler-Human: PENDING
    private static bool IsAsciiLetter(char value) =>
        value is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z');

    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s1; IP=Low; Security=High; Resources=0; Fingerprint=204317
    // Broiler-Human: PENDING
    private static bool IsAsciiAlphanumeric(char value) =>
        IsAsciiLetter(value) || value is >= '0' and <= '9';

    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s2; IP=Low; Security=Low; Resources=1; Fingerprint=129175
    // Broiler-Human: PENDING
    private static System.ReadOnlySpan<char> FirstLabel(string value)
    {
        var dot = value.IndexOf('.');

        return dot < 0
            ? System.MemoryExtensions.AsSpan(value)
            : System.MemoryExtensions.AsSpan(value, 0, dot);
    }
}
