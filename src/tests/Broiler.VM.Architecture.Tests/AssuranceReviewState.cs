namespace Broiler.VM.Architecture.Tests;

/// <summary>The policy's review states. Computed from the two lines, never stored.</summary>
internal enum AssuranceReviewState
{
    /// <summary>A relevant unit carrying no <c>Broiler-AI</c> line at all. A rule failure.</summary>
    New,

    /// <summary>Assessed, fingerprint still <c>TBF</c>: the generator has not run over it.</summary>
    AiAssessed,

    /// <summary>
    /// Assessed with a real fingerprint and no human decision. Every unit in this component that
    /// carries an annotation is here, and the whole point of the system at this milestone is that
    /// it says so.
    /// </summary>
    HumanPending,

    /// <summary>A human named themselves and left the fingerprint for the generator to fill.</summary>
    HumanApprovedPendingFingerprint,

    /// <summary>A human approved exactly the version that is here now.</summary>
    Verified,

    /// <summary>A human approved a version that is no longer the one here.</summary>
    Stale,

    /// <summary>Exempt by the scanner's predicate, or by an explicit reason in the source.</summary>
    Exempt,
}

/// <summary>
/// The state machine, as one function of the two lines and the current fingerprint.
/// </summary>
/// <remarks>
/// <para>
/// The transitions the policy draws are all here, and so is the one it forbids. Only a human may
/// create an approval: nothing in this resolver, and nothing in the generator that consumes it,
/// can produce <see cref="AssuranceReviewState.Verified"/> or a reviewer identifier from a source
/// that does not already carry one. The generator's whole write budget is the
/// <c>Fingerprint</c> field, the <c>STALE; Previous=</c> rewrite of a line that already names a
/// reviewer, and the generated summaries.
/// </para>
/// <para>
/// A <c>STALE</c> line resolves to <see cref="AssuranceReviewState.Stale"/> and stays there. The
/// policy's forbidden edge - <c>STALE</c> straight to <c>VERIFIED</c> - is unreachable here
/// because the only thing that clears staleness is a human replacing the line with their own
/// identifier and <c>Fingerprint=TBF</c>, which is a source edit no automated step performs.
/// </para>
/// </remarks>
internal static class AssuranceStateMachine
{
    internal static AssuranceReviewState Resolve(
        AssuranceAnnotation? annotation,
        bool isExemptByPredicate,
        string currentFingerprint)
    {
        if (isExemptByPredicate || annotation?.ExemptReason is not null)
        {
            return AssuranceReviewState.Exempt;
        }

        if (annotation is null)
        {
            return AssuranceReviewState.New;
        }

        if (annotation.HumanIsStale)
        {
            return AssuranceReviewState.Stale;
        }

        var recorded = annotation.RecordedFingerprint;

        if (recorded is null ||
            string.Equals(recorded, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal))
        {
            return AssuranceReviewState.AiAssessed;
        }

        if (annotation.HumanIsPending || annotation.Reviewer is null)
        {
            return AssuranceReviewState.HumanPending;
        }

        var approved = annotation.HumanFingerprint;

        if (approved is null ||
            string.Equals(approved, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal))
        {
            return AssuranceReviewState.HumanApprovedPendingFingerprint;
        }

        return string.Equals(approved, currentFingerprint, StringComparison.Ordinal)
            ? AssuranceReviewState.Verified
            : AssuranceReviewState.Stale;
    }

    /// <summary>The release-blocking states. Only <c>VERIFIED</c> and <c>EXEMPT</c> are not.</summary>
    internal static bool BlocksRelease(AssuranceReviewState state) =>
        state is not (AssuranceReviewState.Verified or AssuranceReviewState.Exempt);

    /// <summary>The name the policy writes, so a report and the policy read the same.</summary>
    internal static string Name(AssuranceReviewState state) => state switch
    {
        AssuranceReviewState.New => "NEW",
        AssuranceReviewState.AiAssessed => "AI_ASSESSED",
        AssuranceReviewState.HumanPending => "HUMAN_PENDING",
        AssuranceReviewState.HumanApprovedPendingFingerprint => "HUMAN_APPROVED_PENDING_FINGERPRINT",
        AssuranceReviewState.Verified => "VERIFIED",
        AssuranceReviewState.Stale => "STALE",
        AssuranceReviewState.Exempt => "EXEMPT",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
    };
}
