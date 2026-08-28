// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           37
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>What a host capability answers with.</summary>
/// <remarks>
/// Registering a <see cref="Value"/> capability never implies an <see cref="ArtifactProvider"/>.
/// The provider is a distinct kind rather than an ordinary import precisely because it answers a
/// load with a descriptor and bytes instead of a value, and because a composition must be able to
/// register value capabilities without acquiring a path to executable code.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2FEF0B
// Broiler-Human:        PENDING
public enum VmCapabilityKind
{
    /// <summary>An ordinary import that returns a value.</summary>
    Value = 0,

    /// <summary>An artifact provider that answers a guest-initiated load.</summary>
    ArtifactProvider = 1,
}

/// <summary>Whether a host capability may call back into the runtime that invoked it.</summary>
/// <remarks>
/// Enforced, not merely declared: the core holds a per-runtime in-capability flag for the duration
/// of the call and refuses a re-entrant public call where the capability declared
/// <see cref="NonReentrant"/>. A declaration nothing enforces is documentation, and documentation
/// is not a boundary.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CFB013
// Broiler-Human:        PENDING
public enum VmCapabilityReentrancy
{
    /// <summary>The capability may not call back into the invoking runtime.</summary>
    NonReentrant = 0,

    /// <summary>The capability may call back into the invoking runtime.</summary>
    ReentrantIntoInvokingRuntime = 1,
}

/// <summary>The thread a host capability executes on.</summary>
/// <remarks>
/// One legal value in core contract version 1, and a deliberate placeholder: it is written now, with
/// one member, so an amendment can add affinity kinds without changing the descriptor's shape or the
/// identity that records it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=21CE96
// Broiler-Human:        PENDING
public enum VmCapabilityThreadAffinity
{
    /// <summary>
    /// The capability executes synchronously on the calling thread. Marshalling elsewhere is the
    /// host implementation's own business; its blocking time is charged to the operation's
    /// wall-clock allowance and it still counts as one host call.
    /// </summary>
    CallerThread = 0,
}

/// <summary>What the core does with an exception a host capability throws.</summary>
/// <remarks>
/// No default. The descriptor declares one, and a value capability and an artifact-provider
/// capability may declare different modes: a host may reasonably want a failing value import to be
/// observable to the guest while a failing provider terminates the operation.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF8502
// Broiler-Human:        PENDING
public enum VmExceptionTranslation
{
    /// <summary>The operation ends with a host failure.</summary>
    TerminateOperation = 0,

    /// <summary>The failure is reported to the profile as an observable host-call outcome.</summary>
    ObservableFault = 1,
}

/// <summary>Whether a profile requires a capability or can run without it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=871A2F
// Broiler-Human:        PENDING
public enum VmCapabilityImportKind
{
    /// <summary>The runtime is not created unless the capability is registered.</summary>
    Required = 0,

    /// <summary>The runtime is created either way, and the profile can ask whether it was bound.</summary>
    Optional = 1,
}

/// <summary>What a host capability delegate answers.</summary>
/// <remarks>
/// A policy refusal is a returned value rather than a thrown exception, so refusing costs no throw
/// on a path a guest can drive as often as it likes. This ships alongside the exception-translation
/// boundary, not instead of it: a host that throws is still translated.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2D76EA
// Broiler-Human:        PENDING
public enum VmHostCallOutcome
{
    /// <summary>The capability ran and produced its result.</summary>
    Completed = 0,

    /// <summary>The host declined this call as a matter of policy.</summary>
    Refused = 1,

    /// <summary>The capability is bound but cannot serve the call right now.</summary>
    Unavailable = 2,
}

/// <summary>
/// The seven-field description of one host capability.
/// </summary>
/// <remarks>
/// <para>
/// Exactly seven fields; adding a field, or a member to any closed set it names, is a numbered
/// amendment. There is deliberately no <c>Permissions</c> field: <strong>registration is the
/// permission.</strong> A capability the composition did not register into a runtime is not
/// reachable from it at all, so permission denial is a binding-time refusal rather than a per-call
/// check the core would have to evaluate on the hot path.
/// </para>
/// <para>
/// Cancellation observability at a capability boundary is likewise not a field. It is a
/// runtime-identity input, owned by the lifecycle and artifact records, not a property of the
/// capability.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=910B5D
// Broiler-Human:        PENDING
public readonly struct VmHostCapabilityDescriptor : System.IEquatable<VmHostCapabilityDescriptor>
{
    /// <summary>Creates a capability description.</summary>
    public VmHostCapabilityDescriptor(
        VmCapabilityId capabilityId,
        int version,
        VmCapabilitySignatureId signatureId,
        VmCapabilityKind kind,
        VmCapabilityReentrancy reentrancy,
        VmCapabilityThreadAffinity threadAffinity,
        VmExceptionTranslation exceptionTranslation)
    {
        CapabilityId = capabilityId;
        Version = version;
        SignatureId = signatureId;
        Kind = kind;
        Reentrancy = reentrancy;
        ThreadAffinity = threadAffinity;
        ExceptionTranslation = exceptionTranslation;
    }

    /// <summary>F1: the stable, non-localized, namespaced identity.</summary>
    public VmCapabilityId CapabilityId { get; }

    /// <summary>
    /// F2: one exact version. No range, no "or later", no negotiation - a host supporting two
    /// versions registers two capabilities, so what a profile bound to is never ambiguous.
    /// </summary>
    public int Version { get; }

    /// <summary>F3: the identity of the parameter and return shape, compared at binding.</summary>
    public VmCapabilitySignatureId SignatureId { get; }

    /// <summary>F4: value or artifact provider.</summary>
    public VmCapabilityKind Kind { get; }

    /// <summary>F5: whether the capability may re-enter the invoking runtime.</summary>
    public VmCapabilityReentrancy Reentrancy { get; }

    /// <summary>F6: which thread it runs on.</summary>
    public VmCapabilityThreadAffinity ThreadAffinity { get; }

    /// <summary>F7: how a thrown exception is translated.</summary>
    public VmExceptionTranslation ExceptionTranslation { get; }

    /// <summary>Whether every field is present and internally consistent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5CE517
    // Broiler-Human:        PENDING
    public bool IsWellFormed =>
        !CapabilityId.IsEmpty &&
        Version >= 1 &&
        !SignatureId.IsEmpty &&
        ThreadAffinity is VmCapabilityThreadAffinity.CallerThread &&
        // An artifact provider that could re-enter the runtime it is answering for would let a
        // guest-initiated load reach the very operation that requested it. The two declarations
        // are refused together rather than caught later on the nesting path.
        !(Kind is VmCapabilityKind.ArtifactProvider &&
          Reentrancy is VmCapabilityReentrancy.ReentrantIntoInvokingRuntime);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=7AE32A
    // Broiler-Human:        PENDING
    public bool Equals(VmHostCapabilityDescriptor other) =>
        CapabilityId.Equals(other.CapabilityId) &&
        Version == other.Version &&
        SignatureId.Equals(other.SignatureId) &&
        Kind == other.Kind &&
        Reentrancy == other.Reentrancy &&
        ThreadAffinity == other.ThreadAffinity &&
        ExceptionTranslation == other.ExceptionTranslation;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmHostCapabilityDescriptor other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=850367
    // Broiler-Human:        PENDING
    public override int GetHashCode() =>
        System.HashCode.Combine(
            CapabilityId, Version, SignatureId, (int)Kind, (int)Reentrancy,
            (int)ThreadAffinity, (int)ExceptionTranslation);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmHostCapabilityDescriptor left, VmHostCapabilityDescriptor right) =>
        left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2E7664
    // Broiler-Human:        PENDING
    public static bool operator !=(VmHostCapabilityDescriptor left, VmHostCapabilityDescriptor right) =>
        !left.Equals(right);
}

/// <summary>What a VM profile declares it needs from its host.</summary>
/// <remarks>
/// The import carries the whole capability description, not just an ID and a version, because the
/// signature and the declared reentrancy and translation modes are what binding compares. A profile
/// that named only an ID would be asking for whatever the host happened to register under it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=78DEED
// Broiler-Human:        PENDING
public readonly struct VmCapabilityImport : System.IEquatable<VmCapabilityImport>
{
    /// <summary>Creates an import declaration.</summary>
    public VmCapabilityImport(VmHostCapabilityDescriptor descriptor, VmCapabilityImportKind importKind)
    {
        Descriptor = descriptor;
        ImportKind = importKind;
    }

    /// <summary>The shape the profile expects.</summary>
    public VmHostCapabilityDescriptor Descriptor { get; }

    /// <summary>Whether the import is required for the runtime to be created.</summary>
    public VmCapabilityImportKind ImportKind { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=05324A
    // Broiler-Human:        PENDING
    public bool Equals(VmCapabilityImport other) =>
        Descriptor.Equals(other.Descriptor) && ImportKind == other.ImportKind;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmCapabilityImport other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=3F2523
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine(Descriptor, (int)ImportKind);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmCapabilityImport left, VmCapabilityImport right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E8E5B5
    // Broiler-Human:        PENDING
    public static bool operator !=(VmCapabilityImport left, VmCapabilityImport right) => !left.Equals(right);
}

/// <summary>
/// The seven-field record of what a verifier assumed about one host capability, frozen into a
/// verified artifact's identity.
/// </summary>
/// <remarks>
/// It records the <em>shape</em> the verifier assumed, never a binding: no delegate, registration
/// object, host object or capability instance is reachable from it, which is what lets a handle be
/// shared between runtimes without carrying one runtime's host objects into another.
/// <see cref="VmHostCapabilityDescriptor.ThreadAffinity"/> is deliberately absent - it is a
/// runtime-identity input, not a cache-key input.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C7C53D
// Broiler-Human:        PENDING
public readonly struct VmHostSignatureAssumption : System.IEquatable<VmHostSignatureAssumption>
{
    /// <summary>Creates an assumption record.</summary>
    public VmHostSignatureAssumption(
        VmCapabilityId capabilityId,
        int version,
        VmCapabilitySignatureId signatureId,
        VmCapabilityKind kind,
        VmCapabilityReentrancy reentrancy,
        VmExceptionTranslation exceptionTranslation,
        bool optionalImportBound)
    {
        CapabilityId = capabilityId;
        Version = version;
        SignatureId = signatureId;
        Kind = kind;
        Reentrancy = reentrancy;
        ExceptionTranslation = exceptionTranslation;
        OptionalImportBound = optionalImportBound;
    }

    /// <summary>The capability identity assumed.</summary>
    public VmCapabilityId CapabilityId { get; }

    /// <summary>The exact version assumed.</summary>
    public int Version { get; }

    /// <summary>The signature identity assumed.</summary>
    public VmCapabilitySignatureId SignatureId { get; }

    /// <summary>The kind assumed.</summary>
    public VmCapabilityKind Kind { get; }

    /// <summary>The reentrancy declaration assumed.</summary>
    public VmCapabilityReentrancy Reentrancy { get; }

    /// <summary>The translation mode assumed.</summary>
    public VmExceptionTranslation ExceptionTranslation { get; }

    /// <summary>
    /// Whether an optional import was actually bound at verification time. It is part of the
    /// assumption because a verifier may specialise differently depending on the answer.
    /// </summary>
    public bool OptionalImportBound { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=141740
    // Broiler-Human:        PENDING
    public bool Equals(VmHostSignatureAssumption other) =>
        CapabilityId.Equals(other.CapabilityId) &&
        Version == other.Version &&
        SignatureId.Equals(other.SignatureId) &&
        Kind == other.Kind &&
        Reentrancy == other.Reentrancy &&
        ExceptionTranslation == other.ExceptionTranslation &&
        OptionalImportBound == other.OptionalImportBound;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmHostSignatureAssumption other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=A24466
    // Broiler-Human:        PENDING
    public override int GetHashCode() =>
        System.HashCode.Combine(
            CapabilityId, Version, SignatureId, (int)Kind, (int)Reentrancy,
            (int)ExceptionTranslation, OptionalImportBound);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmHostSignatureAssumption left, VmHostSignatureAssumption right) =>
        left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=14E94A
    // Broiler-Human:        PENDING
    public static bool operator !=(VmHostSignatureAssumption left, VmHostSignatureAssumption right) =>
        !left.Equals(right);
}
