// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           34
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>The three states of a verified artifact handle.</summary>
/// <remarks>
/// <see cref="Disposed"/> is terminal and a handle's instance identity is never reused. There is no
/// force-dispose and no lease revocation: a handle with live leases drains, it is not seized, so one
/// runtime can never invalidate another's input.
/// </remarks>
public enum VmVerifiedArtifactState
{
    /// <summary>Usable: leases may be taken and instantiation may proceed.</summary>
    Ready = 0,

    /// <summary>Disposal has been requested and existing leases are being waited out.</summary>
    Draining = 1,

    /// <summary>Terminal.</summary>
    Disposed = 2,
}

/// <summary>
/// The seven identity components of a verified artifact, compared by value.
/// </summary>
/// <remarks>
/// All seven are compared in the cross-runtime sharing predicate. Adding or removing a component
/// requires minting core contract version 2, because the predicate is what a host relies on when it
/// shares one compiled artifact between realms. No delegate, registration object, host object or
/// capability instance is reachable from here: component 7 records the <em>shape</em> the verifier
/// assumed, never a binding.
/// </remarks>
public readonly struct VmVerifiedArtifactIdentity : System.IEquatable<VmVerifiedArtifactIdentity>
{
    /// <summary>Creates an identity.</summary>
    public VmVerifiedArtifactIdentity(
        VmProfileId profileId,
        int descriptorRevision,
        uint acceptedProfileFormatVersion,
        VmFeatureManifestId manifestId,
        int manifestVersion,
        int verifierSemanticVersion,
        int coreContractVersion,
        VmEffectiveCeilings effectiveCeilings,
        System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption> hostSignatureAssumptions)
    {
        ProfileId = profileId;
        DescriptorRevision = descriptorRevision;
        AcceptedProfileFormatVersion = acceptedProfileFormatVersion;
        ManifestId = manifestId;
        ManifestVersion = manifestVersion;
        VerifierSemanticVersion = verifierSemanticVersion;
        CoreContractVersion = coreContractVersion;
        EffectiveCeilings = effectiveCeilings;
        HostSignatureAssumptions = hostSignatureAssumptions;
    }

    /// <summary>Component 1a: the profile.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Component 1b: the descriptor revision verification ran against.</summary>
    public int DescriptorRevision { get; }

    /// <summary>Component 2: the exact profile-format version accepted.</summary>
    public uint AcceptedProfileFormatVersion { get; }

    /// <summary>Component 3a: the feature manifest accepted.</summary>
    public VmFeatureManifestId ManifestId { get; }

    /// <summary>Component 3b: its version.</summary>
    public int ManifestVersion { get; }

    /// <summary>Component 4: the verifier semantic version that produced this handle.</summary>
    public int VerifierSemanticVersion { get; }

    /// <summary>Component 5: the core contract version in force.</summary>
    public int CoreContractVersion { get; }

    /// <summary>Component 6: the materialized ceilings, compared by exact equality.</summary>
    public VmEffectiveCeilings EffectiveCeilings { get; }

    /// <summary>Component 7: the host signature assumptions, in canonical order.</summary>
    public System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption> HostSignatureAssumptions { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s4; IP=Low; Security=Medium; Resources=3; Fingerprint=A41232
    // Broiler-Human: PENDING
    public bool Equals(VmVerifiedArtifactIdentity other)
    {
        if (!ProfileId.Equals(other.ProfileId) ||
            DescriptorRevision != other.DescriptorRevision ||
            AcceptedProfileFormatVersion != other.AcceptedProfileFormatVersion ||
            !ManifestId.Equals(other.ManifestId) ||
            ManifestVersion != other.ManifestVersion ||
            VerifierSemanticVersion != other.VerifierSemanticVersion ||
            CoreContractVersion != other.CoreContractVersion ||
            !EffectiveCeilings.Equals(other.EffectiveCeilings))
        {
            return false;
        }

        var mine = HostSignatureAssumptions.IsDefault
            ? System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption>.Empty
            : HostSignatureAssumptions;

        var theirs = other.HostSignatureAssumptions.IsDefault
            ? System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption>.Empty
            : other.HostSignatureAssumptions;

        if (mine.Length != theirs.Length)
        {
            return false;
        }

        for (var index = 0; index < mine.Length; index++)
        {
            if (!mine[index].Equals(theirs[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Which of the identity clauses <paramref name="other"/> fails, as the reason a sharing
    /// refusal names. Clause order is the frozen order, so a handle failing two clauses always
    /// reports the same one.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s4; IP=Low; Security=Low; Resources=3; Fingerprint=29234F
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmReason FirstMismatch(VmVerifiedArtifactIdentity other)
    {
        if (CoreContractVersion != other.CoreContractVersion)
        {
            return VmReason.SharedHandleCoreContractVersionMismatch;
        }

        if (DescriptorRevision != other.DescriptorRevision)
        {
            return VmReason.SharedHandleDescriptorRevisionMismatch;
        }

        if (AcceptedProfileFormatVersion != other.AcceptedProfileFormatVersion)
        {
            return VmReason.SharedHandleFormatVersionMismatch;
        }

        if (!ManifestId.Equals(other.ManifestId) || ManifestVersion != other.ManifestVersion)
        {
            return VmReason.SharedHandleFeatureManifestMismatch;
        }

        if (VerifierSemanticVersion != other.VerifierSemanticVersion)
        {
            return VmReason.SharedHandleVerifierVersionMismatch;
        }

        if (!EffectiveCeilings.Equals(other.EffectiveCeilings))
        {
            return VmReason.SharedHandleCeilingMismatch;
        }

        if (!Equals(other))
        {
            return VmReason.SharedHandleCapabilityAssumptionMismatch;
        }

        return VmReason.None;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmVerifiedArtifactIdentity other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(
            ProfileId, DescriptorRevision, AcceptedProfileFormatVersion, ManifestId,
            ManifestVersion, VerifierSemanticVersion, CoreContractVersion, EffectiveCeilings);

    /// <summary>Value equality over all seven components.</summary>
    public static bool operator ==(VmVerifiedArtifactIdentity left, VmVerifiedArtifactIdentity right) =>
        left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D15F6D
    // Broiler-Human: PENDING
    public static bool operator !=(VmVerifiedArtifactIdentity left, VmVerifiedArtifactIdentity right) =>
        !left.Equals(right);
}

/// <summary>
/// An explicit pin a host takes on a verified artifact so that a concurrent disposal drains rather
/// than invalidating it.
/// </summary>
/// <remarks>
/// Acquire and release are control operations: they charge nothing, are never refused for
/// exhaustion, and appear in no stage row. Releasing twice is a no-op rather than an error, because
/// the alternative is a host that has to track whether its own <c>using</c> already ran.
/// </remarks>
public sealed class VmArtifactLease : System.IDisposable
{
    private readonly VmVerifiedArtifact artifact;
    private int released;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal VmArtifactLease(VmVerifiedArtifact artifact) => this.artifact = artifact;

    /// <summary>The artifact this lease pins.</summary>
    public VmVerifiedArtifact Artifact => artifact;

    /// <summary>Whether the lease has been released.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Low; Resources=0; Fingerprint=D5129D
    // Broiler-Human: PENDING
    public bool IsReleased => System.Threading.Volatile.Read(ref released) != 0;

    /// <summary>Releases the lease. Idempotent.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=1; Fingerprint=67CDDE
    // Broiler-Human: PENDING
    public VmControlResult Release()
    {
        if (System.Threading.Interlocked.Exchange(ref released, 1) != 0)
        {
            return VmControlResult.NoOp;
        }

        artifact.ReleaseLease();
        return VmControlResult.Accepted;
    }

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Release();
}

/// <summary>
/// The opaque, immutable, profile-bound output of a successful verification: the only thing that
/// may be instantiated or executed.
/// </summary>
/// <remarks>
/// <para>
/// The handle owns a byte snapshot or a fully decoded immutable representation and never aliases
/// mutable caller storage, so later mutation, disposal or concurrent reuse of the caller's buffer
/// cannot affect verified instructions. Nothing reachable from it references a runtime, an instance,
/// a capability delegate, a host object, a cancellation token or a diagnostics sink - which is what
/// lets one handle serve two runtimes without carrying either one's state into the other.
/// </para>
/// <para>
/// It always implements <see cref="System.IDisposable"/> and never
/// <c>IAsyncDisposable</c>, so caller code is identical for a managed and a disposable profile.
/// Identity stays readable for the whole life of the handle, <em>including after disposal</em>:
/// reading what a handle was is not a use of it, and a diagnostic that could not name a disposed
/// handle would be useless exactly when it is needed.
/// </para>
/// </remarks>
public sealed class VmVerifiedArtifact : System.IDisposable
{
    private readonly IVmVerifiedState state;
    private readonly object gate = new();
    private int leaseCount;
    private VmVerifiedArtifactState currentState;

    /// <summary>
    /// The single construction site for a verified handle.
    /// </summary>
    /// <remarks>
    /// ADR 0006 asks that a handle have one construction site and no public constructor, and ADR
    /// 0001 rule A10 forbids <c>InternalsVisibleTo</c> in a product project - so the handle, which
    /// a profile package must be able to name, cannot be constructed across the assembly boundary
    /// by an internal member. This factory is the compromise: it is hidden from IntelliSense, and
    /// an architecture rule asserts over IL that it has exactly one call site in the product graph,
    /// inside the verification stage in <c>Broiler.VM.Runtime</c>. What is lost is that a host
    /// could in principle mint one; what is kept is that the one-construction-site property stays
    /// mechanically testable.
    /// </remarks>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s2; IP=Low; Security=Medium; Resources=1; Fingerprint=69AA1B
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmVerifiedArtifact Create(
        VmObjectId objectId,
        VmVerifiedArtifactIdentity identity,
        VmArtifactRepresentationKind representationKind,
        VmArtifactLifetimeKind lifetimeKind,
        VmArtifactSharing sharing,
        VmArtifactOrigin origin,
        VmObjectId owningRuntimeId,
        VmObjectId aggregateBudgetId,
        ulong byteLength,
        IVmVerifiedState state,
        VmDiagnostics diagnosticsBase) =>
        new(objectId, identity, representationKind, lifetimeKind, sharing, origin,
            owningRuntimeId, aggregateBudgetId, byteLength, state, diagnosticsBase);

    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s2; IP=Low; Security=Low; Resources=1; Fingerprint=AE38A6
    // Broiler-Human: PENDING
    private VmVerifiedArtifact(
        VmObjectId objectId,
        VmVerifiedArtifactIdentity identity,
        VmArtifactRepresentationKind representationKind,
        VmArtifactLifetimeKind lifetimeKind,
        VmArtifactSharing sharing,
        VmArtifactOrigin origin,
        VmObjectId owningRuntimeId,
        VmObjectId aggregateBudgetId,
        ulong byteLength,
        IVmVerifiedState state,
        VmDiagnostics diagnosticsBase)
    {
        ObjectId = objectId;
        VerifiedArtifactInstanceId = objectId;
        Identity = identity;
        RepresentationKind = representationKind;
        LifetimeKind = lifetimeKind;
        Sharing = sharing;
        Origin = origin;
        OwningRuntimeId = owningRuntimeId;
        AggregateBudgetId = aggregateBudgetId;
        ByteLength = byteLength;
        this.state = state;
        DiagnosticsBase = diagnosticsBase;
        currentState = VmVerifiedArtifactState.Ready;
    }

    /// <summary>This handle's process-local identity.</summary>
    public VmObjectId ObjectId { get; }

    /// <summary>
    /// The instance identity, never reused once disposed. It is distinct from the seven-component
    /// <see cref="Identity"/>: two handles verified from identical bytes share an identity and do
    /// not share this.
    /// </summary>
    public VmObjectId VerifiedArtifactInstanceId { get; }

    /// <summary>The seven identity components the sharing predicate compares.</summary>
    public VmVerifiedArtifactIdentity Identity { get; }

    /// <summary>Non-compared: whether the representation is a snapshot or a decoded form.</summary>
    public VmArtifactRepresentationKind RepresentationKind { get; }

    /// <summary>Non-compared: whether disposal has anything to release.</summary>
    public VmArtifactLifetimeKind LifetimeKind { get; }

    /// <summary>Non-compared: whether the profile declared this representation shareable.</summary>
    public VmArtifactSharing Sharing { get; }

    /// <summary>Non-compared: whether the caller or executing guest code asked for these bytes.</summary>
    public VmArtifactOrigin Origin { get; }

    /// <summary>Non-compared: the runtime that verified it.</summary>
    public VmObjectId OwningRuntimeId { get; }

    /// <summary>Non-compared: the aggregate budget that runtime sat under, or the empty identity.</summary>
    public VmObjectId AggregateBudgetId { get; }

    /// <summary>How many bytes were verified.</summary>
    public ulong ByteLength { get; }

    /// <summary>The identity groups every result about this handle starts from.</summary>
    public VmDiagnostics DiagnosticsBase { get; }

    /// <summary>The handle's current state.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Low; Resources=1; Fingerprint=7ADEBB
    // Broiler-Human: PENDING
    public VmVerifiedArtifactState State
    {
        get
        {
            lock (gate)
            {
                return currentState;
            }
        }
    }

    /// <summary>How many leases are currently held.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Low; Resources=1; Fingerprint=8E9141
    // Broiler-Human: PENDING
    public int LeaseCount
    {
        get
        {
            lock (gate)
            {
                return leaseCount;
            }
        }
    }

    /// <summary>
    /// The profile-owned verified state. Returns <see langword="false"/> once the handle is
    /// draining or disposed, which is how use-after-dispose becomes a refusal rather than a
    /// half-valid read.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=1; Fingerprint=DC43FB
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public bool TryGetState(out IVmVerifiedState verifiedState)
    {
        lock (gate)
        {
            if (currentState is not VmVerifiedArtifactState.Ready)
            {
                verifiedState = null!;
                return false;
            }

            verifiedState = state;
            return true;
        }
    }

    /// <summary>Takes an explicit lease, pinning the handle against disposal.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=1; Fingerprint=B4CB03
    // Broiler-Human: PENDING
    public VmControlResult TryAcquireLease(out VmArtifactLease lease)
    {
        lock (gate)
        {
            switch (currentState)
            {
                case VmVerifiedArtifactState.Draining:
                    lease = null!;
                    return VmControlResult.InvalidState(VmReason.HandleDraining);

                case VmVerifiedArtifactState.Disposed:
                    lease = null!;
                    return VmControlResult.InvalidState(VmReason.HandleDisposed);

                default:
                    leaseCount++;
                    lease = new VmArtifactLease(this);
                    return VmControlResult.Accepted;
            }
        }
    }

    /// <summary>
    /// Requests disposal. Idempotent: a second call answers no-op rather than failing. A handle
    /// with live leases enters <see cref="VmVerifiedArtifactState.Draining"/> and completes when
    /// the last lease is released - it is never seized.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=1; Fingerprint=9D7C26
    // Broiler-Human: PENDING
    public VmControlResult Dispose()
    {
        lock (gate)
        {
            switch (currentState)
            {
                case VmVerifiedArtifactState.Disposed:
                    return VmControlResult.NoOp;

                case VmVerifiedArtifactState.Draining:
                    return VmControlResult.NoOp;

                default:
                    if (leaseCount > 0)
                    {
                        currentState = VmVerifiedArtifactState.Draining;
                        return VmControlResult.Accepted;
                    }

                    currentState = VmVerifiedArtifactState.Disposed;
                    return VmControlResult.Accepted;
            }
        }
    }

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Dispose();

    // Broiler-AI:    Origin=AI; Spec=ADR-0006 s3; IP=Low; Security=Medium; Resources=1; Fingerprint=BEB421
    // Broiler-Human: PENDING
    internal void ReleaseLease()
    {
        lock (gate)
        {
            if (leaseCount > 0)
            {
                leaseCount--;
            }

            if (leaseCount == 0 && currentState is VmVerifiedArtifactState.Draining)
            {
                currentState = VmVerifiedArtifactState.Disposed;
            }
        }
    }
}
