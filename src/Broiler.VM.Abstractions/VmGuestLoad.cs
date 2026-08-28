// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           15
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  7/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>Where a verified artifact came from.</summary>
/// <remarks>
/// <see cref="GuestInitiated"/> is exactly the set of remainder-derived handles: one verified under
/// the requesting operation's remaining allowance rather than under a runtime ceiling. There is no
/// separate flag for that, because origin already is it.
/// </remarks>
public enum VmArtifactOrigin
{
    /// <summary>The caller presented the bytes.</summary>
    Caller = 0,

    /// <summary>Executing guest code asked for them through a provider.</summary>
    GuestInitiated = 1,
}

/// <summary>What a profile asks for when it needs code it does not have.</summary>
/// <remarks>
/// The request payload is opaque: the core carries the bytes and never interprets them, because
/// specifier syntax and module resolution are language concepts. What the core does own is
/// everything else on this type - the depth, the remaining allowance, and the identity of the
/// operation being charged.
/// </remarks>
public readonly ref struct VmArtifactRequest
{
    /// <summary>Creates a request.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmArtifactRequest(
        VmProfileId requestingProfileId,
        VmObjectId requestingRuntimeId,
        VmObjectId requestingOperationId,
        int nestingDepth,
        VmLimitVector remainingAllowanceSnapshot,
        System.Threading.CancellationToken cancellationToken,
        VmBytes requestPayload)
    {
        RequestingProfileId = requestingProfileId;
        RequestingRuntimeId = requestingRuntimeId;
        RequestingOperationId = requestingOperationId;
        NestingDepth = nestingDepth;
        RemainingAllowanceSnapshot = remainingAllowanceSnapshot;
        CancellationToken = cancellationToken;
        RequestPayload = requestPayload;
    }

    /// <summary>The profile asking. A provider may only answer with an artifact of this profile.</summary>
    public VmProfileId RequestingProfileId { get; }

    /// <summary>The runtime the request came from.</summary>
    public VmObjectId RequestingRuntimeId { get; }

    /// <summary>The operation the nested work is charged to.</summary>
    public VmObjectId RequestingOperationId { get; }

    /// <summary>How deep this request is. Zero would be a caller-driven load, so this is at least one.</summary>
    public int NestingDepth { get; }

    /// <summary>What the requesting operation has left. A provider may use it to decide what to answer.</summary>
    public VmLimitVector RemainingAllowanceSnapshot { get; }

    /// <summary>The requesting operation's cancellation token.</summary>
    public System.Threading.CancellationToken CancellationToken { get; }

    /// <summary>The profile's own opaque request bytes - a specifier, a name, whatever it means.</summary>
    public VmBytes RequestPayload { get; }
}

/// <summary>Which of the three answers an artifact provider gave.</summary>
/// <remarks>
/// A closed set of exactly three. Adding a fourth is a core contract amendment, because a fourth
/// answer is a fourth thing the charging and refusal rules would have to cover.
/// </remarks>
public enum VmArtifactProviderAnswerKind
{
    /// <summary>The provider supplied a descriptor and bytes.</summary>
    Provided = 0,

    /// <summary>The provider declined as a matter of policy.</summary>
    Refused = 1,

    /// <summary>The provider has no artifact matching the request.</summary>
    NotFound = 2,
}

/// <summary>What an artifact provider answers a guest-initiated load with.</summary>
/// <remarks>
/// <para>
/// A refusal is a typed answer, never an exception. A thrown exception is a host fault translated by
/// the capability's declared translation mode and is <em>not</em> a refusal: the difference matters,
/// because a policy that refuses is working and a provider that throws is broken.
/// </para>
/// <para>
/// Synchronous and byte-returning by construction. The answer is a <c>ref struct</c>, so it cannot
/// cross an await, which is what makes an asynchronous provider unrepresentable rather than
/// merely discouraged.
/// </para>
/// </remarks>
public readonly ref struct VmArtifactProviderAnswer
{
    private VmArtifactProviderAnswer(
        VmArtifactProviderAnswerKind kind,
        VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        VmReason reason)
    {
        Kind = kind;
        Descriptor = descriptor;
        Payload = payload;
        Reason = reason;
    }

    /// <summary>The provider supplied an artifact, exactly as a caller would.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1E33DD
    // Broiler-Human: PENDING
    public static VmArtifactProviderAnswer Provided(
        scoped in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload) =>
        new(VmArtifactProviderAnswerKind.Provided, descriptor, payload, VmReason.None);

    /// <summary>The provider declined.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DC6F85
    // Broiler-Human: PENDING
    public static VmArtifactProviderAnswer Refused(VmReason reason) =>
        new(VmArtifactProviderAnswerKind.Refused, default, default, reason);

    /// <summary>The provider has nothing matching.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1977AB
    // Broiler-Human: PENDING
    public static VmArtifactProviderAnswer NotFound(VmReason reason) =>
        new(VmArtifactProviderAnswerKind.NotFound, default, default, reason);

    /// <summary>Which of the three answers this is.</summary>
    public VmArtifactProviderAnswerKind Kind { get; }

    /// <summary>The descriptor, present only on the provided answer.</summary>
    public VmArtifactDescriptor Descriptor { get; }

    /// <summary>The bytes, present only on the provided answer.</summary>
    public System.ReadOnlySpan<byte> Payload { get; }

    /// <summary>The reason, present on the two negative answers.</summary>
    public VmReason Reason { get; }
}

/// <summary>
/// The typed, allowlisted host capability that answers a guest-initiated load with a descriptor and
/// bytes.
/// </summary>
/// <remarks>
/// <para>
/// A distinct capability kind rather than an ordinary import. Registering value capabilities never
/// implies one, and a composition that registers none refuses every guest-initiated load
/// deterministically - which is how a content policy forbidding dynamic evaluation is expressed as
/// a contract outcome rather than as an ad-hoc check inside an engine.
/// </para>
/// <para>
/// A composition that includes a compiler supplies it behind this capability. That keeps the
/// compiler inside the declared Native AOT closure and keeps the profile from reaching a filesystem,
/// a socket or a compiler on its own.
/// </para>
/// </remarks>
public interface IVmArtifactProvider
{
    /// <summary>The capability identity this provider is registered under.</summary>
    VmCapabilityId CapabilityId { get; }

    /// <summary>Its exact version.</summary>
    int Version { get; }

    /// <summary>Answers one request.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=731666
    // Broiler-Human: PENDING
    VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request);
}

/// <summary>
/// The only route by which an executing profile can obtain further executable bytes.
/// </summary>
/// <remarks>
/// It is handed to a declaring profile's executor and is valid only for the dynamic extent of the
/// invocation that supplied it; retaining one and using it later is an invalid state naming the
/// mediator as out of scope. After a suspend and resume cycle the profile must request through the
/// freshly supplied mediator, because the old one belonged to an operation step that has ended.
/// </remarks>
public interface IVmArtifactLoadMediator
{
    /// <summary>
    /// Requests an artifact. The result is an ordinary verification result: the bytes become their
    /// own immutable verified handle before anything in them runs, nesting relaxes no bound, and
    /// the work is charged to the operation that asked.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=572045
    // Broiler-Human: PENDING
    VmGuestLoadResult RequestLoad(scoped in VmArtifactRequest request);
}
