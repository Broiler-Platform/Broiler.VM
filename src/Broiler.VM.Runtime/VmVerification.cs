// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           4
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  8/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>What a verification is a total function of, and nothing else.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=300BB3
// Broiler-Human:        PENDING
internal sealed class VmVerificationContext : IVmVerificationContext
{
    private readonly System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor> shapes;

    internal VmVerificationContext(
        VmEffectiveCeilings ceilings,
        IVmMeter meter,
        System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor> shapes)
    {
        Ceilings = ceilings;
        Meter = meter;
        this.shapes = shapes;
    }

    /// <inheritdoc/>
    public VmEffectiveCeilings Ceilings { get; }

    /// <inheritdoc/>
    public IVmMeter Meter { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9D33CA
    // Broiler-Human:        PENDING
    public System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities => shapes;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=9DE8C1
    // Broiler-Human:        PENDING
    public bool TryGetCapabilityDescriptor(
        VmCapabilityId capabilityId,
        int version,
        out VmHostCapabilityDescriptor descriptor)
    {
        foreach (var shape in shapes)
        {
            if (shape.CapabilityId.Equals(capabilityId) && shape.Version == version)
            {
                descriptor = shape;
                return true;
            }
        }

        descriptor = default;
        return false;
    }
}

/// <summary>
/// The verification stage, shared by the caller-driven and the guest-initiated paths.
/// </summary>
/// <remarks>
/// A guest-initiated load is an ordinary load requested from an unusual place, not a second
/// execution path. It runs the same steps in the same order under a different budget origin, which
/// is what makes "nesting relaxes no bound and skips no descriptor match" true by construction
/// rather than by review.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=910045
// Broiler-Falsified-If: a guest-initiated load is admitted while a profile verifier frame is on the stack
// Broiler-Human:        PENDING
public sealed partial class VmRuntime
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=4166AF
    // Broiler-Falsified-If: cancellation is decided after an input is examined, or an unknown profile answers InvalidArtifact
    // Broiler-Human:        PENDING
    internal VmVerificationResult VerifyCore(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        System.Threading.CancellationToken cancellationToken,
        VmDiagnostics baseline,
        VmArtifactOrigin origin,
        VmMeter? requestingMeter)
    {
        // Cancellation ranks second in the frozen precedence, above unsupported profile and
        // invalid artifact, and the latch is observed before any input is examined - which is what
        // makes a cancelled request deterministic rather than a function of thread timing.
        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerificationResult.Cancellation(
                VmReason.Cancelled,
                baseline.WithOutcome(VmStage.Verification, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
        }

        if (!descriptor.IsWellFormed)
        {
            return VmVerificationResult.InvalidArtifact(
                VmReason.MalformedArtifactDescriptor,
                baseline.WithOutcome(VmStage.Verification, VmOutcome.InvalidArtifact, VmReason.MalformedArtifactDescriptor, VmInitiator.Caller));
        }

        if (!TryGetDescriptor(descriptor.ProfileId, out var profile))
        {
            // Not an invalid artifact: the bytes were never looked at. Reporting a composition
            // mistake as a corrupt file is the single most likely diagnostic error for a
            // single-profile product, so it gets its own category.
            return VmVerificationResult.UnsupportedProfile(
                VmReason.ProfileNotInCatalog,
                baseline
                    .WithOutcome(VmStage.Verification, VmOutcome.UnsupportedProfile, VmReason.ProfileNotInCatalog, VmInitiator.Caller)
                    .WithProfile(descriptor.ProfileId, descriptor.FormatVersion, descriptor.FeatureManifestId, 0));
        }

        var identified = baseline.WithProfile(
            profile.ProfileId,
            descriptor.FormatVersion,
            descriptor.FeatureManifestId,
            profile.Verifier.VerifierSemanticVersion);

        if (!profile.SupportedFormatVersions.Contains(descriptor.FormatVersion))
        {
            return VmVerificationResult.InvalidArtifact(
                VmReason.UnsupportedProfileFormatVersion,
                identified.WithOutcome(VmStage.Verification, VmOutcome.InvalidArtifact, VmReason.UnsupportedProfileFormatVersion, VmInitiator.Caller));
        }

        if (!Accepts(profile, descriptor.FeatureManifestId))
        {
            return VmVerificationResult.InvalidArtifact(
                VmReason.UnsupportedFeatureManifest,
                identified.WithOutcome(VmStage.Verification, VmOutcome.InvalidArtifact, VmReason.UnsupportedFeatureManifest, VmInitiator.Caller));
        }

        // The slot is taken whatever the profile declares. A profile that declares it supports
        // concurrent verification is still bounded by the runtime's own configured maximum, and one
        // that does not is held to a single verification at a time - the declaration narrows the
        // bound, it does not remove it.
        var slots = profile.SupportsConcurrentVerification ? Options.MaxConcurrentVerifications : 1;

        if (!TryEnterVerification(slots))
        {
            return VmVerificationResult.InvalidState(
                VmReason.WrongState,
                Invalid(identified, VmStage.Verification, VmReason.WrongState, VmObjectKind.Runtime, VmAttemptedCall.Verify));
        }

        try
        {
            return RunVerifier(profile, in descriptor, payload, cancellationToken, identified, origin, requestingMeter);
        }
        finally
        {
            ExitVerification();
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=D433B9
    // Broiler-Falsified-If: an escaping verifier exception is answered as a category, or both effective ceilings are one vector, or a cancelled or poll-bound-violating verification is answered as resource exhaustion
    // Broiler-Human:        PENDING
    private VmVerificationResult RunVerifier(
        VmProfileDescriptor profile,
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        System.Threading.CancellationToken cancellationToken,
        VmDiagnostics identified,
        VmArtifactOrigin origin,
        VmMeter? requestingMeter)
    {
        var profileState = GetProfileState(profile);

        // The effective ceilings are materialized BEFORE the first payload byte is read and before
        // any allocation sized by payload data. A nested verification starts from the requesting
        // operation's remaining allowance instead of the runtime ceiling, so it can exhaust an
        // invocation and can never enlarge one.
        var hostCeilings = requestingMeter is null
            ? RuntimeLevel.AsCeilingVector()
            : requestingMeter.RemainingSnapshot;

        // P2 in two steps rather than one, so the clamp is observable. The bound is what the host
        // and the profile agreed before the artifact spoke; the intersection with the request can
        // only tighten it, and every dimension where the request asked for more is recorded on the
        // handle rather than being silently discarded.
        var bound = VmLimitVector.Intersect(hostCeilings, profile.ProfileHardMaxima);

        var effective = VmLimitVector.Intersect(
            bound,
            descriptor.RequestedLimits.IsEmpty ? VmLimitVector.Unconstrained : descriptor.RequestedLimits);

        var clamps = VmLimitPrecedence.Clamps(bound, descriptor.RequestedLimits);

        var ceilings = new VmEffectiveCeilings(effective, effective);

        var invocationLevel = new VmBudgetLevel(VmBudgetScope.Invocation, ToArray(effective));
        var meter = requestingMeter ?? new VmMeter(
            Gate, invocationLevel, null, RuntimeLevel, Parent, profile.MaxUnchargedWork, cancellationToken);

        if ((ulong)payload.Length > effective[VmBudgetDimension.ArtifactBytes])
        {
            return VmVerificationResult.ResourceExhaustion(
                VmReason.CeilingReached,
                identified
                    .WithOutcome(VmStage.Verification, VmOutcome.ResourceExhaustion, VmReason.CeilingReached, VmInitiator.Caller)
                    .WithExhaustion(VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact));
        }

        var context = new VmVerificationContext(ceilings, meter, profileState.BoundShapes);

        // An escaping verifier exception is NOT translated into a category, and is deliberately not
        // caught here. Translating it would let a verifier bug masquerade as a malicious artifact
        // and hide from a malformed corpus: the same result would be reported for a verifier that
        // dereferenced null and for bytes that were genuinely invalid, and a corpus labelled by
        // category and reason could not tell them apart. The budget already charged stays charged -
        // the work was genuinely done, so there is no refund by throwing - and the runtime stays
        // usable.
        var outcome = profile.Verifier.Verify(in descriptor, payload, context, cancellationToken);

        switch (outcome.Category)
        {
            case VmOutcome.UnsupportedProfile:
                return VmVerificationResult.UnsupportedProfile(
                    VmReason.ProfileNotInCatalog,
                    identified.WithOutcome(VmStage.Verification, VmOutcome.UnsupportedProfile, VmReason.ProfileNotInCatalog, VmInitiator.Guest));

            case VmOutcome.InvalidArtifact:
                return VmVerificationResult.InvalidArtifact(
                    outcome.Reason,
                    identified
                        .WithOutcome(VmStage.Verification, VmOutcome.InvalidArtifact, outcome.Reason, VmInitiator.Guest)
                        .WithPosition(outcome.Position, outcome.ProfileDiagnosticCode));

            case VmOutcome.ResourceExhaustion:
            {
                // A verifier built on the bounded reader cannot tell these three apart on its own:
                // TryChargeWork and Poll are both folded into WorkBudgetExhausted, so a cancelled
                // verification, a poll-bound violation and a genuinely exhausted budget all reach
                // the profile as one status and come back here as ResourceExhaustion. Reporting
                // the verifier's own attribution would tell a caller its artifact was too
                // expensive when the host had in fact cancelled, and a corpus labelled by category
                // and reason could not tell the two apart. VmInstantiation already consults the
                // meter's latches for exactly this reason; verification did not, and the meter is
                // the only party that knows which of the three actually happened.
                if (meter.CancellationObserved)
                {
                    return VmVerificationResult.Cancellation(
                        VmReason.Cancelled,
                        identified.WithOutcome(VmStage.Verification, VmOutcome.Cancellation, VmReason.Cancelled, VmInitiator.Host));
                }

                // A poll-bound violation is deliberately NOT translated here. It is a profile
                // contract breach, and the verification taxonomy has no ProfileFault category to
                // put one in; reporting it as InvalidArtifact would blame the artifact for the
                // verifier's defect, which is the same error the escaping-exception rule above
                // exists to prevent. Choosing a category for it is a contract question, and it is
                // recorded as one rather than answered here.

                // Where the meter latched, its dimension and scope name the level that actually
                // refused; the verifier can only report the level it can attribute unaided.
                var exhaustedDimension = meter.ExhaustionObserved ? meter.FailedDimension : outcome.ExhaustedDimension;
                var exhaustedScope = meter.ExhaustionObserved ? meter.FailedScope : outcome.ExhaustedScope;

                return VmVerificationResult.ResourceExhaustion(
                    outcome.Reason,
                    identified
                        .WithOutcome(VmStage.Verification, VmOutcome.ResourceExhaustion, outcome.Reason, VmInitiator.Guest)
                        .WithExhaustion(exhaustedDimension, exhaustedScope));
            }

            case VmOutcome.Cancellation:
                return VmVerificationResult.Cancellation(
                    outcome.Reason,
                    identified.WithOutcome(VmStage.Verification, VmOutcome.Cancellation, outcome.Reason, VmInitiator.Host));
        }

        if (outcome.State is null)
        {
            // The core itself detected a verifier contract breach: an answer of Normal with no
            // verified state. That is not something an artifact can cause, so it cannot be reported
            // as one - it is thrown, and the runtime is poisoned so no later call computes anything
            // from a state the core no longer trusts.
            Poison();

            throw new VmCoreDefectException(
                "The verifier for " + profile.ProfileId + " answered Normal without producing " +
                "verified state, which the profile contract does not permit.",
                ObjectId);
        }

        // A verifier may narrow sharing and may never widen it. Taking the tighter of the two is
        // the enforcement, not a convention the profile is trusted to observe.
        var sharing = profile.ArtifactSharing is VmArtifactSharing.Shareable &&
            outcome.NarrowedSharing is VmArtifactSharing.Shareable
            ? VmArtifactSharing.Shareable
            : VmArtifactSharing.RuntimeScoped;

        // A guest-initiated handle is never shareable: its ceilings came from one operation's
        // remainder, so it means nothing anywhere else.
        if (origin is VmArtifactOrigin.GuestInitiated)
        {
            sharing = VmArtifactSharing.RuntimeScoped;
        }

        var identity = new VmVerifiedArtifactIdentity(
            profile.ProfileId,
            profile.DescriptorRevision,
            descriptor.FormatVersion,
            descriptor.FeatureManifestId,
            profile.ConformanceManifestVersion,
            profile.Verifier.VerifierSemanticVersion,
            VmCoreContract.Version,
            ceilings,
            profileState.Assumptions);

        var artifactId = VmObjectId.Mint();

        var artifact = VmVerifiedArtifact.Create(
            artifactId,
            identity,
            profile.ArtifactRepresentationKind,
            profile.ArtifactLifetimeKind,
            sharing,
            origin,
            ObjectId,
            Parent?.Id.ObjectId ?? default,
            (ulong)payload.Length,
            outcome.State,
            clamps,
            identified.WithArtifact(artifactId, (ulong)payload.Length, descriptor.CallerIdentity));

        return VmVerificationResult.Normal(
            artifact,
            identified
                .WithOutcome(VmStage.Verification, VmOutcome.Normal, VmReason.NormalCompleted, VmInitiator.Caller)
                .WithArtifact(artifactId, (ulong)payload.Length, descriptor.CallerIdentity));
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=830489
    // Broiler-Human:        PENDING
    private static bool Accepts(VmProfileDescriptor profile, VmFeatureManifestId manifest)
    {
        if (profile.AcceptedFeatureManifests.IsDefault)
        {
            return false;
        }

        foreach (var accepted in profile.AcceptedFeatureManifests)
        {
            if (accepted.Equals(manifest))
            {
                return true;
            }
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=63A61D
    // Broiler-Human:        PENDING
    internal static ulong[] ToArray(VmLimitVector vector)
    {
        var values = new ulong[VmBudgetDimensions.Count];
        vector.CopyTo(values);
        return values;
    }
}
