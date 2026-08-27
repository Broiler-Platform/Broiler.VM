namespace Broiler.VM;

/// <summary>
/// The profile-neutral outcome category every envelope-bearing stage returns. Closed at ten.
/// </summary>
/// <remarks>
/// <para>
/// Values 10 and above are reserved for numbered amendments. Renumbering, renaming, removing or
/// re-scoping any of the ten is forbidden outright: persisted envelopes, evidence bundles, support
/// tables and malformed-corpus expectations all record numeric outcomes, and every one of them
/// would become silently wrong.
/// </para>
/// <para>
/// The set is closed because an open one is section 16's "one case per language" risk wearing a
/// different shape. A profile carries its language outcomes as a typed payload behind one of these
/// categories; it never adds a category. Reason codes, not categories, are the pressure valve.
/// </para>
/// </remarks>
public enum VmOutcome
{
    /// <summary>
    /// Reserved and never returned by any stage. It exists so that <c>default</c> of any result
    /// struct cannot be read as success.
    /// </summary>
    None = 0,

    /// <summary>The stage completed and produced its stage value.</summary>
    Normal = 1,

    /// <summary>
    /// A well-formed profile identity was resolved against a composition that cannot host it.
    /// This is deliberately not <see cref="InvalidArtifact"/>: conflating them misreports a
    /// composition mistake as a corrupt file, which is the most likely diagnostic error for
    /// single-profile products.
    /// </summary>
    UnsupportedProfile = 2,

    /// <summary>
    /// Offered bytes are not a well-formed artifact of the identified profile and format version,
    /// or a persisted envelope's outer schema, bounds, checksum, atomic-replacement state or
    /// migration is not acceptable.
    /// </summary>
    InvalidArtifact = 3,

    /// <summary>The operation is not legal against the target object in its current state.</summary>
    InvalidState = 4,

    /// <summary>
    /// The profile completed the operation by producing a language-defined fault, or violated the
    /// profile contract.
    /// </summary>
    ProfileFault = 5,

    /// <summary>
    /// The operation paused at a declared transition and is resumable through a core-owned
    /// single-use suspension object.
    /// </summary>
    Suspension = 6,

    /// <summary>A host-requested cancellation was observed at a declared polling point.</summary>
    Cancellation = 7,

    /// <summary>One named budget dimension in one named scope had no remaining allowance.</summary>
    ResourceExhaustion = 8,

    /// <summary>A host capability could not be reached, refused, or faulted.</summary>
    HostFailure = 9,
}

/// <summary>
/// The seven envelope-bearing stages.
/// </summary>
/// <remarks>
/// Catalog construction is deliberately not a stage. A catalog is authored by a composition root
/// from trusted compile-time data, so a defect there is a wiring bug that must be loud and
/// unrecoverable: the builder throws. That is why the matrix names seven stages and not eight, and
/// why <see cref="VmOutcome.UnsupportedProfile"/> keeps one meaning instead of two.
/// </remarks>
public enum VmStage
{
    /// <summary>Not a stage; the value a diagnostics record carries before a stage is entered.</summary>
    None = 0,

    /// <summary>S1: runtime creation.</summary>
    RuntimeCreation = 1,

    /// <summary>
    /// S2: persisted-envelope preprocessing. Admitted by core contract version 1 and
    /// <strong>not implemented</strong> in release 1: no public member can enter it. Its
    /// invariant 8 discharge is absence from the API baseline, not a returned failure.
    /// </summary>
    EnvelopePreprocessing = 2,

    /// <summary>S3: caller-driven load and verification.</summary>
    Verification = 3,

    /// <summary>S4: guest-initiated load. Profile-facing only; never returned to an invocation caller.</summary>
    GuestInitiatedLoad = 4,

    /// <summary>S5: instantiation.</summary>
    Instantiation = 5,

    /// <summary>S6: invocation.</summary>
    Invocation = 6,

    /// <summary>S7: resume.</summary>
    Resume = 7,
}

/// <summary>
/// The frozen stage/category matrix: which outcome categories are legal at which stage.
/// </summary>
/// <remarks>
/// <para>
/// The negative rules are the part an implementer must not reopen, so they are recorded here with
/// their reasons. <see cref="VmOutcome.InvalidArtifact"/> is illegal at instantiation, invocation
/// and resume because invariant 3 makes verification complete: a verified handle cannot later
/// become invalid, and admitting it would create a second, later verification point - section 16's
/// second-verifier stop condition in miniature. <see cref="VmOutcome.ProfileFault"/> is illegal
/// before instantiation because there is no profile instance to own a fault.
/// <see cref="VmOutcome.Suspension"/> is illegal before instantiation because a resumable nested
/// verification would let a half-verified artifact outlive its requesting operation.
/// </para>
/// <para>
/// This table is the oracle the result factories and the drift test both read. A category with no
/// factory on a result type is how an illegal cell stays a compile-time fact rather than a runtime
/// assertion.
/// </para>
/// </remarks>
public static class VmStageMatrix
{
    private static readonly bool[,] Legal = BuildMatrix();

    /// <summary>Whether <paramref name="outcome"/> is a legal category at <paramref name="stage"/>.</summary>
    public static bool IsLegal(VmStage stage, VmOutcome outcome)
    {
        if (stage is VmStage.None || outcome is VmOutcome.None)
        {
            return false;
        }

        return Legal[(int)stage, (int)outcome];
    }

    /// <summary>Every legal category at <paramref name="stage"/>, in ascending numeric order.</summary>
    public static VmOutcome[] LegalCategoriesAt(VmStage stage)
    {
        var legal = new System.Collections.Generic.List<VmOutcome>(9);

        for (var outcome = 1; outcome <= 9; outcome++)
        {
            if (IsLegal(stage, (VmOutcome)outcome))
            {
                legal.Add((VmOutcome)outcome);
            }
        }

        return legal.ToArray();
    }

    private static bool[,] BuildMatrix()
    {
        var matrix = new bool[8, 10];

        Set(VmStage.RuntimeCreation,
            VmOutcome.Normal, VmOutcome.InvalidState, VmOutcome.ResourceExhaustion, VmOutcome.HostFailure);

        Set(VmStage.EnvelopePreprocessing,
            VmOutcome.Normal, VmOutcome.UnsupportedProfile, VmOutcome.InvalidArtifact,
            VmOutcome.InvalidState, VmOutcome.Cancellation, VmOutcome.ResourceExhaustion);

        Set(VmStage.Verification,
            VmOutcome.Normal, VmOutcome.UnsupportedProfile, VmOutcome.InvalidArtifact,
            VmOutcome.InvalidState, VmOutcome.Cancellation, VmOutcome.ResourceExhaustion);

        Set(VmStage.GuestInitiatedLoad,
            VmOutcome.Normal, VmOutcome.UnsupportedProfile, VmOutcome.InvalidArtifact,
            VmOutcome.InvalidState, VmOutcome.Cancellation, VmOutcome.ResourceExhaustion,
            VmOutcome.HostFailure);

        Set(VmStage.Instantiation,
            VmOutcome.Normal, VmOutcome.UnsupportedProfile, VmOutcome.InvalidState,
            VmOutcome.ProfileFault, VmOutcome.Suspension, VmOutcome.Cancellation,
            VmOutcome.ResourceExhaustion, VmOutcome.HostFailure);

        Set(VmStage.Invocation,
            VmOutcome.Normal, VmOutcome.InvalidState, VmOutcome.ProfileFault, VmOutcome.Suspension,
            VmOutcome.Cancellation, VmOutcome.ResourceExhaustion, VmOutcome.HostFailure);

        // S7 is the row of the stage that suspended - S5 or S6 - plus InvalidState, minus
        // UnsupportedProfile. The subtraction is deliberate: UnsupportedProfile at instantiation
        // is an entry check on a handle shared from another runtime, and that check has already
        // passed by the time an instantiation suspends, so resume cannot reach it.
        Set(VmStage.Resume,
            VmOutcome.Normal, VmOutcome.InvalidState, VmOutcome.ProfileFault, VmOutcome.Suspension,
            VmOutcome.Cancellation, VmOutcome.ResourceExhaustion, VmOutcome.HostFailure);

        return matrix;

        void Set(VmStage stage, params VmOutcome[] outcomes)
        {
            foreach (var outcome in outcomes)
            {
                matrix[(int)stage, (int)outcome] = true;
            }
        }
    }
}
