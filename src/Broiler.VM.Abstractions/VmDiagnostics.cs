// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   36
// Annotated:        36/36
// Exempt:           84
// Human-reviewed:   0/36
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  3/10 max
// Unverified:       36
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>An opaque correlation token a host resolves against its own log.</summary>
/// <remarks>
/// The core stores and echoes it and never parses, formats or interprets it. It is how a host
/// correlates a core result with its own request without the core learning anything about the
/// host's identifiers.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CFBEE1
// Broiler-Human: PENDING
public readonly struct VmHostCorrelationToken : System.IEquatable<VmHostCorrelationToken>
{
    private readonly ulong high;
    private readonly ulong low;

    /// <summary>Creates a token from two 64-bit halves.</summary>
    public VmHostCorrelationToken(ulong high, ulong low)
    {
        this.high = high;
        this.low = low;
    }

    /// <summary>True when this is <see langword="default"/>.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=055DED
    // Broiler-Human: PENDING
    public bool IsEmpty => high == 0 && low == 0;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E1336B
    // Broiler-Human: PENDING
    public bool Equals(VmHostCorrelationToken other) => high == other.high && low == other.low;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmHostCorrelationToken other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(high, low);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmHostCorrelationToken left, VmHostCorrelationToken right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F864F6
    // Broiler-Human: PENDING
    public static bool operator !=(VmHostCorrelationToken left, VmHostCorrelationToken right) => !left.Equals(right);
}

/// <summary>The kind of lifecycle object a diagnostics record refers to.</summary>
// Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B8FE7C
// Broiler-Human: PENDING
public enum VmObjectKind
{
    /// <summary>No object.</summary>
    None = 0,

    /// <summary>A catalog builder.</summary>
    CatalogBuilder = 1,

    /// <summary>A built, immutable catalog.</summary>
    Catalog = 2,

    /// <summary>A runtime.</summary>
    Runtime = 3,

    /// <summary>A verified artifact handle.</summary>
    VerifiedArtifact = 4,

    /// <summary>An instance.</summary>
    Instance = 5,

    /// <summary>An operation.</summary>
    Operation = 6,

    /// <summary>A suspension object.</summary>
    Suspension = 7,

    /// <summary>An aggregate budget.</summary>
    AggregateBudget = 8,

    /// <summary>An operation control handle.</summary>
    ControlHandle = 9,
}

/// <summary>
/// The observed state of the object a call was rejected against, rendered from that object's own
/// state enum.
/// </summary>
/// <remarks>
/// It carries the kind alongside the numeric value because the numbers are only meaningful
/// together: state 2 of a runtime and state 2 of an instance are different facts, and a
/// diagnostics consumer that saw only the number would have to guess which table to read.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A6A8FD
// Broiler-Human: PENDING
public readonly struct VmObjectState : System.IEquatable<VmObjectState>
{
    /// <summary>Creates a state observation.</summary>
    public VmObjectState(VmObjectKind kind, int value)
    {
        Kind = kind;
        Value = value;
    }

    /// <summary>Which object's state enum <see cref="Value"/> is drawn from.</summary>
    public VmObjectKind Kind { get; }

    /// <summary>The numeric state, in that object's own enum.</summary>
    public int Value { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=693E4F
    // Broiler-Human: PENDING
    public bool Equals(VmObjectState other) => Kind == other.Kind && Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmObjectState other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4861A5
    // Broiler-Human: PENDING
    public override int GetHashCode() => System.HashCode.Combine((int)Kind, Value);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmObjectState left, VmObjectState right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=ACBD2D
    // Broiler-Human: PENDING
    public static bool operator !=(VmObjectState left, VmObjectState right) => !left.Equals(right);
}

/// <summary>The public call an invalid-state result was produced for.</summary>
// Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B50319
// Broiler-Human: PENDING
public enum VmAttemptedCall
{
    /// <summary>No call.</summary>
    None = 0,

    /// <summary>Verification.</summary>
    Verify = 1,

    /// <summary>Instantiation.</summary>
    Instantiate = 2,

    /// <summary>Invocation.</summary>
    Invoke = 3,

    /// <summary>Resumption.</summary>
    Resume = 4,

    /// <summary>Disposal.</summary>
    Dispose = 5,

    /// <summary>A cancellation request.</summary>
    RequestCancel = 6,

    /// <summary>An external-suspension request.</summary>
    RequestSuspend = 7,

    /// <summary>Taking a suspension object from a control handle.</summary>
    TryTakeSuspension = 8,

    /// <summary>Querying operation state.</summary>
    QueryState = 9,

    /// <summary>Polling residency and other deadlines.</summary>
    PollDeadlines = 10,

    /// <summary>Acquiring an artifact lease.</summary>
    LeaseAcquire = 11,

    /// <summary>Releasing an artifact lease.</summary>
    LeaseRelease = 12,

    /// <summary>Projecting a typed profile payload.</summary>
    ProjectPayload = 13,

    /// <summary>A guest-initiated load request.</summary>
    GuestLoad = 14,

    /// <summary>Adding a descriptor to a catalog builder.</summary>
    CatalogAdd = 15,

    /// <summary>Building a catalog.</summary>
    CatalogBuild = 16,
}

/// <summary>
/// Who initiated a transition. Closed at four.
/// </summary>
/// <remarks>
/// The tag is part of the frozen contract rather than a convenience, because diagnostics, audit and
/// tests all key on it: "the guest suspended" and "the host suspended" are different facts with
/// different legal successors, and a result that did not distinguish them would make the state
/// tables untestable.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=140C0D
// Broiler-Human: PENDING
public enum VmInitiator
{
    /// <summary>The caller that entered the public surface.</summary>
    Caller = 0,

    /// <summary>Executing guest code.</summary>
    Guest = 1,

    /// <summary>The host, through a control handle or a runtime-level request.</summary>
    Host = 2,

    /// <summary>The core itself - a deadline, a latch, a bound.</summary>
    Core = 3,
}

/// <summary>
/// An opaque, profile-owned position inside an artifact.
/// </summary>
/// <remarks>
/// The core stores and returns it and never parses, orders, formats or compares it. Two of the
/// four fields are the profile's own coordinates precisely so that a profile can carry a line and
/// column, a function and offset, or anything else, without the core acquiring a notion of what
/// any of them mean.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=69E733
// Broiler-Human: PENDING
public readonly struct VmSourcePosition : System.IEquatable<VmSourcePosition>
{
    /// <summary>Creates a position.</summary>
    public VmSourcePosition(int sectionIndex, ulong byteOffset, int profileCoordinate0, int profileCoordinate1)
    {
        SectionIndex = sectionIndex;
        ByteOffset = byteOffset;
        ProfileCoordinate0 = profileCoordinate0;
        ProfileCoordinate1 = profileCoordinate1;
    }

    /// <summary>The framed section, or -1 when the position is outside any section.</summary>
    public int SectionIndex { get; }

    /// <summary>The byte offset within the artifact.</summary>
    public ulong ByteOffset { get; }

    /// <summary>A profile-defined coordinate. The core attaches no meaning to it.</summary>
    public int ProfileCoordinate0 { get; }

    /// <summary>A second profile-defined coordinate.</summary>
    public int ProfileCoordinate1 { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=595AC0
    // Broiler-Human: PENDING
    public bool Equals(VmSourcePosition other) =>
        SectionIndex == other.SectionIndex &&
        ByteOffset == other.ByteOffset &&
        ProfileCoordinate0 == other.ProfileCoordinate0 &&
        ProfileCoordinate1 == other.ProfileCoordinate1;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmSourcePosition other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(SectionIndex, ByteOffset, ProfileCoordinate0, ProfileCoordinate1);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmSourcePosition left, VmSourcePosition right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7601B2
    // Broiler-Human: PENDING
    public static bool operator !=(VmSourcePosition left, VmSourcePosition right) => !left.Equals(right);
}

/// <summary>
/// The caller-supplied canonical source or module identity, echoed verbatim in diagnostics.
/// </summary>
/// <remarks>
/// The core never parses it, never logs it and never derives anything from it. It is flagged as
/// caller-supplied so a diagnostics sink can redact it without having to guess which fields might
/// carry a URL, a file path or a user identifier.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8C6D88
// Broiler-Human: PENDING
public readonly struct VmCallerIdentity : System.IEquatable<VmCallerIdentity>
{
    private readonly string? text;

    private VmCallerIdentity(string text) => this.text = text;

    /// <summary>The absent identity.</summary>
    public static VmCallerIdentity None => default;

    /// <summary>Creates an identity from a caller-supplied canonical string.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=A051B8
    // Broiler-Human: PENDING
    public static VmCallerIdentity FromCanonicalIdentity(System.ReadOnlySpan<char> identity) =>
        identity.IsEmpty ? default : new VmCallerIdentity(identity.ToString());

    /// <summary>True when no identity was supplied.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=173B23
    // Broiler-Human: PENDING
    public bool IsEmpty => text is null;

    /// <summary>
    /// True when this value came from the caller, which is every non-empty value. It is the
    /// redaction flag a diagnostics sink keys on.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C4403F
    // Broiler-Human: PENDING
    public bool IsCallerSupplied => text is not null;

    /// <inheritdoc/>
    public bool Equals(VmCallerIdentity other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <summary>The identity verbatim.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=57BC75
    // Broiler-Human: PENDING
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmCallerIdentity other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=6C9B84
    // Broiler-Human: PENDING
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmCallerIdentity left, VmCallerIdentity right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=33A459
    // Broiler-Human: PENDING
    public static bool operator !=(VmCallerIdentity left, VmCallerIdentity right) => !left.Equals(right);
}

/// <summary>
/// The diagnostics record every stage result carries.
/// </summary>
/// <remarks>
/// <para>
/// It is a readonly struct of value types and references the core already held, so carrying it
/// costs no allocation on any path, including the failure paths a hostile input drives hardest.
/// </para>
/// <para>
/// <strong>What it deliberately does not carry.</strong> The exhaustion group names a dimension and
/// a scope and never an absolute ceiling or an absolute consumption figure - those live on the
/// host-facing budget snapshot, because a guest-observable result that carried them would leak the
/// host's policy to the code the policy is applied to. No member is typed as a catalog listing or
/// any collection of catalog entries: the listing is reachable only from the catalog itself, so the
/// disclosure split is enforced by type rather than by a flag, and there is no verbose mode that
/// could turn it back on.
/// </para>
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=18A7D4
// Broiler-Human: PENDING
public readonly struct VmDiagnostics : System.IEquatable<VmDiagnostics>
{
    /// <summary>Creates a diagnostics record. Every field is supplied; there is no partial form.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics(
        VmStage stage,
        VmOutcome outcome,
        VmReason reason,
        VmObjectId runtimeId,
        VmObjectId operationId,
        VmObjectId requestingOperationId,
        int nestingDepth,
        VmProfileId profileId,
        uint profileFormatVersion,
        VmFeatureManifestId featureManifestId,
        int verifierSemanticVersion,
        VmObjectId artifactId,
        ulong artifactByteLength,
        VmCallerIdentity callerIdentity,
        VmSourcePosition sourcePosition,
        VmBudgetDimension exhaustedDimension,
        VmBudgetScope exhaustedScope,
        VmCapabilityId capabilityId,
        int capabilityVersion,
        VmHostCorrelationToken hostCorrelation,
        int profileDiagnosticCode,
        VmObjectKind objectKind,
        VmObjectState objectState,
        VmAttemptedCall attemptedCall,
        VmInitiator initiator)
    {
        Stage = stage;
        Outcome = outcome;
        Reason = reason;
        RuntimeId = runtimeId;
        OperationId = operationId;
        RequestingOperationId = requestingOperationId;
        NestingDepth = nestingDepth;
        ProfileId = profileId;
        ProfileFormatVersion = profileFormatVersion;
        FeatureManifestId = featureManifestId;
        VerifierSemanticVersion = verifierSemanticVersion;
        ArtifactId = artifactId;
        ArtifactByteLength = artifactByteLength;
        CallerIdentity = callerIdentity;
        SourcePosition = sourcePosition;
        ExhaustedDimension = exhaustedDimension;
        ExhaustedScope = exhaustedScope;
        CapabilityId = capabilityId;
        CapabilityVersion = capabilityVersion;
        HostCorrelation = hostCorrelation;
        ProfileDiagnosticCode = profileDiagnosticCode;
        ObjectKind = objectKind;
        ObjectState = objectState;
        AttemptedCall = attemptedCall;
        Initiator = initiator;
    }

    /// <summary>
    /// The minimal record: stage, outcome, reason and who asked. Every other group is absent
    /// rather than defaulted to something that reads as a claim.
    /// </summary>
    /// <remarks>
    /// The wither methods below fill the remaining groups. They exist because a twenty-five
    /// parameter constructor at every call site would be a place for two adjacent identities to
    /// be transposed without the compiler noticing.
    /// </remarks>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E37CC2
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmDiagnostics Create(
        VmStage stage,
        VmOutcome outcome,
        VmReason reason,
        VmObjectId runtimeId,
        VmInitiator initiator,
        VmAttemptedCall attemptedCall) =>
        new(stage, outcome, reason, runtimeId, default, default, 0,
            default, 0, default, 0, default, 0, VmCallerIdentity.None, default,
            VmBudgetDimension.Fuel, VmBudgetScope.Invocation, default, 0, default, 0,
            VmObjectKind.None, default, attemptedCall, initiator);

    /// <summary>Adds the operation and nesting group.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9F1DCA
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithOperation(
        VmObjectId operationId,
        VmObjectId requestingOperationId,
        int nestingDepth) =>
        new(Stage, Outcome, Reason, RuntimeId, operationId, requestingOperationId, nestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the profile identity group.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EBDDB1
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithProfile(
        VmProfileId profileId,
        uint profileFormatVersion,
        VmFeatureManifestId featureManifestId,
        int verifierSemanticVersion) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            profileId, profileFormatVersion, featureManifestId, verifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the artifact group.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=511B71
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithArtifact(
        VmObjectId artifactId,
        ulong artifactByteLength,
        VmCallerIdentity callerIdentity) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            artifactId, artifactByteLength, callerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the profile-owned position and diagnostic code.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=467AF2
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithPosition(VmSourcePosition position, int profileDiagnosticCode) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, position,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            profileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the exhausted dimension and scope.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6DE21D
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithExhaustion(VmBudgetDimension dimension, VmBudgetScope scope) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            dimension, scope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the capability group.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=89F987
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithCapability(
        VmCapabilityId capabilityId,
        int capabilityVersion,
        VmHostCorrelationToken correlation) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, capabilityId, capabilityVersion, correlation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, Initiator);

    /// <summary>Adds the three facts an invalid-state result must carry.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE2B1B
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithObject(VmObjectKind kind, int observedState, VmAttemptedCall attemptedCall) =>
        new(Stage, Outcome, Reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, kind, new VmObjectState(kind, observedState), attemptedCall, Initiator);

    /// <summary>Restates the outcome and reason, keeping every identity group.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E49B8
    // Broiler-Human: PENDING
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmDiagnostics WithOutcome(VmStage stage, VmOutcome outcome, VmReason reason, VmInitiator initiator) =>
        new(stage, outcome, reason, RuntimeId, OperationId, RequestingOperationId, NestingDepth,
            ProfileId, ProfileFormatVersion, FeatureManifestId, VerifierSemanticVersion,
            ArtifactId, ArtifactByteLength, CallerIdentity, SourcePosition,
            ExhaustedDimension, ExhaustedScope, CapabilityId, CapabilityVersion, HostCorrelation,
            ProfileDiagnosticCode, ObjectKind, ObjectState, AttemptedCall, initiator);

    /// <summary>Group 1: the core contract version this build implements.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7C60B3
    // Broiler-Human: PENDING
    public int CoreContractVersion => VmCoreContract.Version;

    /// <summary>Group 1: the reason-registry revision the reason is drawn from.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=345DB5
    // Broiler-Human: PENDING
    public int ReasonRegistryRevision => VmReasonRegistry.Revision;

    /// <summary>Group 2: which stage produced the result.</summary>
    public VmStage Stage { get; }

    /// <summary>Group 2: the outcome category.</summary>
    public VmOutcome Outcome { get; }

    /// <summary>Group 2: the reason code.</summary>
    public VmReason Reason { get; }

    /// <summary>Group 3: the runtime the operation ran in.</summary>
    public VmObjectId RuntimeId { get; }

    /// <summary>Group 3: the operation.</summary>
    public VmObjectId OperationId { get; }

    /// <summary>Group 3: for a nested load, the operation that requested it.</summary>
    public VmObjectId RequestingOperationId { get; }

    /// <summary>Group 3: provider-mediated nesting depth, zero for a caller-driven operation.</summary>
    public int NestingDepth { get; }

    /// <summary>Group 4: the profile identity.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Group 4: the profile-format version.</summary>
    public uint ProfileFormatVersion { get; }

    /// <summary>Group 4: the feature manifest.</summary>
    public VmFeatureManifestId FeatureManifestId { get; }

    /// <summary>Group 4: the verifier semantic version.</summary>
    public int VerifierSemanticVersion { get; }

    /// <summary>Group 5: the artifact handle.</summary>
    public VmObjectId ArtifactId { get; }

    /// <summary>Group 5: the artifact's byte length.</summary>
    public ulong ArtifactByteLength { get; }

    /// <summary>Group 5: the caller-supplied identity, echoed verbatim and flagged for redaction.</summary>
    public VmCallerIdentity CallerIdentity { get; }

    /// <summary>Group 6: the profile-owned position, opaque to the core.</summary>
    public VmSourcePosition SourcePosition { get; }

    /// <summary>Group 7: which dimension was exhausted. Never an absolute ceiling.</summary>
    public VmBudgetDimension ExhaustedDimension { get; }

    /// <summary>Group 7: at which scope. Never an absolute consumption figure.</summary>
    public VmBudgetScope ExhaustedScope { get; }

    /// <summary>Group 8: the capability involved in a host failure.</summary>
    public VmCapabilityId CapabilityId { get; }

    /// <summary>Group 8: its declared version.</summary>
    public int CapabilityVersion { get; }

    /// <summary>Group 8: the host's own correlation token, echoed.</summary>
    public VmHostCorrelationToken HostCorrelation { get; }

    /// <summary>Group 9: a stable 32-bit code the profile chose. The core attaches no meaning to it.</summary>
    public int ProfileDiagnosticCode { get; }

    /// <summary>The kind of object an invalid-state result was produced against.</summary>
    public VmObjectKind ObjectKind { get; }

    /// <summary>The state that object was observed in.</summary>
    public VmObjectState ObjectState { get; }

    /// <summary>The call that was attempted.</summary>
    public VmAttemptedCall AttemptedCall { get; }

    /// <summary>Who initiated the transition.</summary>
    public VmInitiator Initiator { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=F4B6C3
    // Broiler-Human: PENDING
    public bool Equals(VmDiagnostics other) =>
        Stage == other.Stage && Outcome == other.Outcome && Reason == other.Reason &&
        RuntimeId.Equals(other.RuntimeId) && OperationId.Equals(other.OperationId) &&
        RequestingOperationId.Equals(other.RequestingOperationId) &&
        NestingDepth == other.NestingDepth && ProfileId.Equals(other.ProfileId) &&
        ProfileFormatVersion == other.ProfileFormatVersion &&
        FeatureManifestId.Equals(other.FeatureManifestId) &&
        VerifierSemanticVersion == other.VerifierSemanticVersion &&
        ArtifactId.Equals(other.ArtifactId) && ArtifactByteLength == other.ArtifactByteLength &&
        CallerIdentity.Equals(other.CallerIdentity) && SourcePosition.Equals(other.SourcePosition) &&
        ExhaustedDimension == other.ExhaustedDimension && ExhaustedScope == other.ExhaustedScope &&
        CapabilityId.Equals(other.CapabilityId) && CapabilityVersion == other.CapabilityVersion &&
        HostCorrelation.Equals(other.HostCorrelation) &&
        ProfileDiagnosticCode == other.ProfileDiagnosticCode &&
        ObjectKind == other.ObjectKind && ObjectState.Equals(other.ObjectState) &&
        AttemptedCall == other.AttemptedCall && Initiator == other.Initiator;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmDiagnostics other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C423B7
    // Broiler-Human: PENDING
    public override int GetHashCode()
    {
        var hash = new System.HashCode();
        hash.Add((int)Stage);
        hash.Add((int)Outcome);
        hash.Add((int)Reason);
        hash.Add(RuntimeId);
        hash.Add(OperationId);
        hash.Add(ProfileId);
        hash.Add(ArtifactId);
        hash.Add((int)AttemptedCall);
        hash.Add((int)Initiator);
        return hash.ToHashCode();
    }

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmDiagnostics left, VmDiagnostics right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DCF31E
    // Broiler-Human: PENDING
    public static bool operator !=(VmDiagnostics left, VmDiagnostics right) => !left.Equals(right);
}
