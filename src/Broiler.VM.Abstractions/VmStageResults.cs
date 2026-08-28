// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   58
// Annotated:        58/58
// Exempt:           41
// Human-reviewed:   0/58
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  0/10 max
// Unverified:       58
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The shape every envelope-bearing stage result carries.
/// </summary>
/// <remarks>
/// It exists <strong>only</strong> as a generic constraint - <c>where T : struct,
/// IVmOperationResult</c> - and is never a storage type, so no stage result is ever boxed by the
/// core. A result held behind this interface would allocate on every call, including the failure
/// paths a hostile input drives hardest.
/// </remarks>
public interface IVmOperationResult
{
    /// <summary>The outcome category.</summary>
    VmOutcome Outcome { get; }

    /// <summary>The reason code.</summary>
    VmReason Reason { get; }

    /// <summary>The diagnostics record.</summary>
    VmDiagnostics Diagnostics { get; }

    /// <summary>True only when <see cref="Outcome"/> is <see cref="VmOutcome.Normal"/>.</summary>
    bool IsSuccess { get; }

    /// <summary>True only when <see cref="Outcome"/> is <see cref="VmOutcome.Suspension"/>.</summary>
    bool IsSuspended { get; }
}

/// <summary>
/// S2: the persisted-envelope preprocessing result.
/// </summary>
/// <remarks>
/// <strong>Declared and unreachable.</strong> Core contract version 1 admits the stage - its
/// ownership split, outer-header field list, failure mapping and re-verification rule are all
/// frozen - and core release 1 exposes no member by which it can be entered. Its invariant 8
/// discharge is absence from the public API baseline, not a returned failure, which is why this
/// type has no factory that any runtime member could call. A type that existed and threw would be
/// the shape-only stub invariant 8 rejects.
/// </remarks>
public readonly struct VmEnvelopeReadResult : IVmOperationResult
{
    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9A0727
    // Broiler-Human: PENDING
    public VmOutcome Outcome => VmOutcome.None;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=651B37
    // Broiler-Human: PENDING
    public VmReason Reason => VmReason.None;

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics => default;

    /// <inheritdoc/>
    public bool IsSuccess => false;

    /// <inheritdoc/>
    public bool IsSuspended => false;
}

/// <summary>S3: the caller-driven load and verification result.</summary>
/// <remarks>
/// There is no <c>ProfileFault</c>, <c>Suspension</c> or <c>HostFailure</c> factory, because those
/// cells are illegal at this stage: no profile instance exists to own a fault, a resumable
/// verification would let a half-verified artifact outlive its requesting operation, and no host
/// capability is invoked on the caller-driven path.
/// </remarks>
public readonly struct VmVerificationResult : IVmOperationResult
{
    private readonly VmVerifiedArtifact? artifact;

    private VmVerificationResult(VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics, VmVerifiedArtifact? artifact)
    {
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.artifact = artifact;
    }

    /// <summary>The artifact verified.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FDD515
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult Normal(VmVerifiedArtifact artifact, VmDiagnostics diagnostics) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, artifact);

    /// <summary>The composition cannot host the requested profile.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EB2DD6
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult UnsupportedProfile(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.UnsupportedProfile, reason, diagnostics, null);

    /// <summary>The bytes are not a well-formed artifact.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DE0D5B
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult InvalidArtifact(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidArtifact, reason, diagnostics, null);

    /// <summary>The call is not legal against the runtime in its current state.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E942EE
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult InvalidState(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidState, reason, diagnostics, null);

    /// <summary>Cancellation was observed at a polling point.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1403A5
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult Cancellation(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.Cancellation, reason, diagnostics, null);

    /// <summary>A named dimension in a named scope had no remaining allowance.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=97CC66
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerificationResult ResourceExhaustion(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.ResourceExhaustion, reason, diagnostics, null);

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B03FEA
    // Broiler-Human: PENDING
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    public bool IsSuspended => false;

    /// <summary>The verified handle, available only on success.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Medium; Resources=0; Fingerprint=56BC99
    // Broiler-Human: PENDING
    public bool TryGetArtifact(out VmVerifiedArtifact verifiedArtifact)
    {
        verifiedArtifact = artifact!;
        return Outcome is VmOutcome.Normal && artifact is not null;
    }
}

/// <summary>S4: the guest-initiated load result, observed by the profile that requested it.</summary>
/// <remarks>
/// Profile-facing only: it is never returned to the caller of an invocation. Folding it into the
/// caller-driven verification row was rejected because it would make a caller-driven verification
/// appear able to fail for a host-capability reason that cannot occur on that path, weakening the
/// matrix as a test oracle and hiding the nesting depth and requesting-operation identity that the
/// charging rule must keep auditable.
/// </remarks>
public readonly struct VmGuestLoadResult : IVmOperationResult
{
    private readonly VmVerifiedArtifact? artifact;

    private VmGuestLoadResult(VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics, VmVerifiedArtifact? artifact)
    {
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.artifact = artifact;
    }

    /// <summary>The nested artifact was verified.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F3121E
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult Normal(VmVerifiedArtifact artifact, VmDiagnostics diagnostics) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, artifact);

    /// <summary>The provider named a profile the composition cannot host.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EB62F1
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult UnsupportedProfile(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.UnsupportedProfile, reason, diagnostics, null);

    /// <summary>The provider's bytes are not a well-formed artifact.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=366EA3
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult InvalidArtifact(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidArtifact, reason, diagnostics, null);

    /// <summary>The request is not legal - an out-of-scope mediator, a disposed runtime.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1B572C
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult InvalidState(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidState, reason, diagnostics, null);

    /// <summary>Cancellation was observed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=308D92
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult Cancellation(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.Cancellation, reason, diagnostics, null);

    /// <summary>A bound or an allowance the nested load draws on was spent.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5E7120
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult ResourceExhaustion(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.ResourceExhaustion, reason, diagnostics, null);

    /// <summary>
    /// The provider could not be reached, refused, or faulted - including the deterministic
    /// refusal of a composition that registered no provider at all.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A09655
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmGuestLoadResult HostFailure(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.HostFailure, reason, diagnostics, null);

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B03FEA
    // Broiler-Human: PENDING
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    public bool IsSuspended => false;

    /// <summary>The nested verified handle, available only on success.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Medium; Resources=0; Fingerprint=56BC99
    // Broiler-Human: PENDING
    public bool TryGetArtifact(out VmVerifiedArtifact verifiedArtifact)
    {
        verifiedArtifact = artifact!;
        return Outcome is VmOutcome.Normal && artifact is not null;
    }
}

/// <summary>S5: the instantiation result.</summary>
public readonly struct VmInstantiationResult : IVmOperationResult
{
    private readonly VmInstance? instance;
    private readonly IVmProfilePayload? payload;
    private readonly VmSuspension? suspension;

    private VmInstantiationResult(
        VmOutcome outcome,
        VmReason reason,
        VmDiagnostics diagnostics,
        VmInstance? instance,
        IVmProfilePayload? payload,
        VmSuspension? suspension)
    {
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.instance = instance;
        this.payload = payload;
        this.suspension = suspension;
    }

    /// <summary>The instance was created.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DE5953
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult Normal(VmInstance instance, IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, instance, payload, null);

    /// <summary>A handle from another runtime names a profile this composition cannot host.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A237BB
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult UnsupportedProfile(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.UnsupportedProfile, reason, diagnostics, null, null, null);

    /// <summary>The call is not legal against the runtime or the handle in its current state.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FA7193
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult InvalidState(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidState, reason, diagnostics, null, null, null);

    /// <summary>The profile faulted, carrying its own typed payload.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=24C378
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult ProfileFault(VmReason reason, IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(VmOutcome.ProfileFault, reason, diagnostics, null, payload, null);

    /// <summary>Instantiation parked, which requires a declaring descriptor.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8F0775
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult Suspension(VmSuspension suspension, IVmProfilePayload? projection, VmDiagnostics diagnostics) =>
        new(VmOutcome.Suspension, VmReason.InstantiationSuspended, diagnostics, null, projection, suspension);

    /// <summary>Cancellation was observed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1CCF11
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult Cancellation(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.Cancellation, reason, diagnostics, null, null, null);

    /// <summary>A named dimension in a named scope had no remaining allowance.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FF2D54
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult ResourceExhaustion(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.ResourceExhaustion, reason, diagnostics, null, null, null);

    /// <summary>A host capability could not be reached, refused, or faulted.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F9CAFB
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInstantiationResult HostFailure(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.HostFailure, reason, diagnostics, null, null, null);

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B03FEA
    // Broiler-Human: PENDING
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4A00D0
    // Broiler-Human: PENDING
    public bool IsSuspended => Outcome is VmOutcome.Suspension;

    /// <summary>The payload's identity, or the empty identity where no payload was produced.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C296A0
    // Broiler-Human: PENDING
    public VmPayloadIdentity PayloadIdentity => payload?.Identity ?? default;

    /// <summary>The instance, available only on success.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=115C81
    // Broiler-Human: PENDING
    public bool TryGetInstance(out VmInstance createdInstance)
    {
        createdInstance = instance!;
        return Outcome is VmOutcome.Normal && instance is not null;
    }

    /// <summary>The typed profile payload, where one was produced.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=039889
    // Broiler-Human: PENDING
    public bool TryGetPayload<TPayload>(out TPayload typedPayload)
        where TPayload : class, IVmProfilePayload
    {
        typedPayload = (payload as TPayload)!;
        return typedPayload is not null;
    }

    /// <summary>The resumption object, available only on a suspension.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B759ED
    // Broiler-Human: PENDING
    public bool TryGetSuspension(out VmSuspension pending)
    {
        pending = suspension!;
        return Outcome is VmOutcome.Suspension && suspension is not null;
    }
}

/// <summary>S6: the invocation result.</summary>
/// <remarks>
/// There is deliberately no <c>InvalidArtifact</c> and no <c>UnsupportedProfile</c> factory. The
/// first would create a second, later verification point; the second is already answered, because
/// the profile is bound once an instance exists.
/// </remarks>
public readonly struct VmInvocationResult : IVmOperationResult
{
    private readonly IVmProfilePayload? payload;
    private readonly VmSuspension? suspension;

    private VmInvocationResult(
        VmOutcome outcome,
        VmReason reason,
        VmDiagnostics diagnostics,
        IVmProfilePayload? payload,
        VmSuspension? suspension)
    {
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.payload = payload;
        this.suspension = suspension;
    }

    /// <summary>The invocation completed, carrying the profile's typed result payload.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B07DBE
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult Normal(IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, payload, null);

    /// <summary>The call is not legal against the instance in its current state.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=665DDD
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult InvalidState(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidState, reason, diagnostics, null, null);

    /// <summary>A language-defined fault, carried as a typed payload the core never interprets.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=758B13
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult ProfileFault(VmReason reason, IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(VmOutcome.ProfileFault, reason, diagnostics, payload, null);

    /// <summary>The invocation parked and is resumable.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=07791C
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult Suspension(VmSuspension? suspension, IVmProfilePayload? projection, VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.Suspension, reason, diagnostics, projection, suspension);

    /// <summary>Cancellation was observed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C8B440
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult Cancellation(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.Cancellation, reason, diagnostics, null, null);

    /// <summary>A named dimension in a named scope had no remaining allowance.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4A018F
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult ResourceExhaustion(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.ResourceExhaustion, reason, diagnostics, null, null);

    /// <summary>A host capability could not be reached, refused, or faulted.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0FF766
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmInvocationResult HostFailure(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.HostFailure, reason, diagnostics, null, null);

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B03FEA
    // Broiler-Human: PENDING
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4A00D0
    // Broiler-Human: PENDING
    public bool IsSuspended => Outcome is VmOutcome.Suspension;

    /// <summary>The payload's identity, or the empty identity where no payload was produced.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C296A0
    // Broiler-Human: PENDING
    public VmPayloadIdentity PayloadIdentity => payload?.Identity ?? default;

    /// <summary>
    /// The typed profile payload.
    /// </summary>
    /// <remarks>
    /// The <c>class</c> constraint is load-bearing rather than tidy: a generic method instantiated
    /// only over reference types compiles to one canonical shared body, which is what keeps the
    /// core free of per-profile generic instantiations a Native AOT closure could not root.
    /// </remarks>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=039889
    // Broiler-Human: PENDING
    public bool TryGetPayload<TPayload>(out TPayload typedPayload)
        where TPayload : class, IVmProfilePayload
    {
        typedPayload = (payload as TPayload)!;
        return typedPayload is not null;
    }

    /// <summary>
    /// The resumption object, available only on a guest-origin suspension. An external suspension
    /// carries the origin and the operation identity and no object: that one is delivered once,
    /// through the control handle.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B759ED
    // Broiler-Human: PENDING
    public bool TryGetSuspension(out VmSuspension pending)
    {
        pending = suspension!;
        return Outcome is VmOutcome.Suspension && suspension is not null;
    }
}

/// <summary>S7: the resume result.</summary>
/// <remarks>
/// Its legal set is the row of the stage that suspended, plus invalid state, minus unsupported
/// profile. The subtraction is deliberate: unsupported profile at instantiation is an entry check
/// on a handle shared from another runtime, and that check has already passed by the time an
/// instantiation suspends, so resume cannot reach it.
/// </remarks>
public readonly struct VmResumeResult : IVmOperationResult
{
    private readonly VmInstance? instance;
    private readonly IVmProfilePayload? payload;
    private readonly VmSuspension? suspension;

    private VmResumeResult(
        VmStage suspendedStage,
        VmOutcome outcome,
        VmReason reason,
        VmDiagnostics diagnostics,
        VmInstance? instance,
        IVmProfilePayload? payload,
        VmSuspension? suspension)
    {
        SuspendedStage = suspendedStage;
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.instance = instance;
        this.payload = payload;
        this.suspension = suspension;
    }

    /// <summary>The resumed operation completed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=37D812
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult Normal(VmStage suspendedStage, VmInstance? instance, IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, instance, payload, null);

    /// <summary>The resumption object or the target is not in a resumable state.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B4283E
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult InvalidState(VmStage suspendedStage, VmReason reason, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.InvalidState, reason, diagnostics, null, null, null);

    /// <summary>A language-defined fault.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2ED7B3
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult ProfileFault(VmStage suspendedStage, VmReason reason, IVmProfilePayload? payload, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.ProfileFault, reason, diagnostics, null, payload, null);

    /// <summary>The operation parked again.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0F7F7A
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult Suspension(VmStage suspendedStage, VmSuspension? suspension, IVmProfilePayload? projection, VmReason reason, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.Suspension, reason, diagnostics, null, projection, suspension);

    /// <summary>Cancellation was observed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A8E29E
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult Cancellation(VmStage suspendedStage, VmReason reason, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.Cancellation, reason, diagnostics, null, null, null);

    /// <summary>A named dimension in a named scope had no remaining allowance.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=846D23
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult ResourceExhaustion(VmStage suspendedStage, VmReason reason, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.ResourceExhaustion, reason, diagnostics, null, null, null);

    /// <summary>A host capability could not be reached, refused, or faulted.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0BE395
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmResumeResult HostFailure(VmStage suspendedStage, VmReason reason, VmDiagnostics diagnostics) =>
        new(suspendedStage, VmOutcome.HostFailure, reason, diagnostics, null, null, null);

    /// <summary>Which stage had suspended.</summary>
    public VmStage SuspendedStage { get; }

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B03FEA
    // Broiler-Human: PENDING
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4A00D0
    // Broiler-Human: PENDING
    public bool IsSuspended => Outcome is VmOutcome.Suspension;

    /// <summary>The payload's identity, or the empty identity where no payload was produced.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C296A0
    // Broiler-Human: PENDING
    public VmPayloadIdentity PayloadIdentity => payload?.Identity ?? default;

    /// <summary>The instance, where a suspended instantiation completed.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=115C81
    // Broiler-Human: PENDING
    public bool TryGetInstance(out VmInstance createdInstance)
    {
        createdInstance = instance!;
        return Outcome is VmOutcome.Normal && instance is not null;
    }

    /// <summary>The typed profile payload, where one was produced.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=039889
    // Broiler-Human: PENDING
    public bool TryGetPayload<TPayload>(out TPayload typedPayload)
        where TPayload : class, IVmProfilePayload
    {
        typedPayload = (payload as TPayload)!;
        return typedPayload is not null;
    }

    /// <summary>The resumption object, where the operation parked again.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B759ED
    // Broiler-Human: PENDING
    public bool TryGetSuspension(out VmSuspension pending)
    {
        pending = suspension!;
        return Outcome is VmOutcome.Suspension && suspension is not null;
    }
}
