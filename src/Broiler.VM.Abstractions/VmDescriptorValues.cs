// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           24
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Low
// Resource impact:  1/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// How a VM profile represents a verified artifact: as an immutable byte snapshot, or as a fully
/// decoded immutable form.
/// </summary>
/// <remarks>
/// The choice is per profile, never per artifact and never per call, and the core never branches on
/// it semantically. It exists so a support table and a closure report can state which shape a
/// profile uses, not so the core can behave differently for one of them.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9C9CF5
// Broiler-Human: PENDING
public enum VmArtifactRepresentationKind
{
    /// <summary>An immutable copy of the caller's bytes.</summary>
    Snapshot = 0,

    /// <summary>A fully decoded immutable form; the caller's bytes are not retained.</summary>
    Decoded = 1,
}

/// <summary>Whether a verified artifact owns explicitly disposable resources.</summary>
/// <remarks>
/// The handle implements <see cref="System.IDisposable"/> either way, so caller code is identical
/// for both kinds. A profile that declares <see cref="Managed"/> is promising that disposal has
/// nothing to release, not that disposal is unavailable.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=454F6D
// Broiler-Human: PENDING
public enum VmArtifactLifetimeKind
{
    /// <summary>Ordinary managed immutable data.</summary>
    Managed = 0,

    /// <summary>The representation owns resources that must be released explicitly.</summary>
    Disposable = 1,
}

/// <summary>Whether one verified artifact may be used by more than one runtime.</summary>
/// <remarks>
/// The restrictive value is ordinal zero so that it is also <c>default</c>: a profile that declares
/// nothing gets the answer that cannot leak state between runtimes. A verifier may narrow an
/// artifact to <see cref="RuntimeScoped"/> and may never widen one to <see cref="Shareable"/>.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EF64D0
// Broiler-Human: PENDING
public enum VmArtifactSharing
{
    /// <summary>Usable only by the runtime that verified it.</summary>
    RuntimeScoped = 0,

    /// <summary>Usable by any runtime whose identity matches exactly.</summary>
    Shareable = 1,
}

/// <summary>What a profile fault does to the instance that produced it.</summary>
/// <remarks>
/// Mandatory with no default: it is the only way a profile changes the outcome-to-instance-state
/// mapping, and a defaulted answer would make that mapping depend on which value happened to be
/// zero.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B96EAC
// Broiler-Human: PENDING
public enum VmFaultRecovery
{
    /// <summary>The instance remains live and may be invoked again.</summary>
    InstanceRecoverable = 0,

    /// <summary>The instance moves to its faulted state and accepts only disposal.</summary>
    InstanceFatal = 1,
}

/// <summary>The thread affinity a VM profile declares for its own execution.</summary>
/// <remarks>
/// Distinct from a host capability's affinity, which is a different closed set on a different
/// object. One name for two closed sets would be exactly the ambiguity the qualifier rule exists to
/// prevent, so the capability one is spelled separately.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8E9D85
// Broiler-Human: PENDING
public enum VmThreadAffinity
{
    /// <summary>Any thread may enter, one at a time.</summary>
    Agile = 0,

    /// <summary>Every call on one operation must arrive on the thread that started it.</summary>
    OperationThreadPinned = 1,
}

/// <summary>A two-valued declaration: the profile either declares a capability or it does not.</summary>
/// <remarks>
/// <see cref="NotDeclared"/> being ordinal zero does not weaken the "no default" rule: the
/// descriptor's constructor parameters for these fields are required, so silence is not
/// expressible at the only place a value is supplied.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D8F051
// Broiler-Human: PENDING
public enum VmDeclaration
{
    /// <summary>The profile does not declare the capability.</summary>
    NotDeclared = 0,

    /// <summary>The profile declares the capability.</summary>
    Declared = 1,
}

/// <summary>
/// A profile's guest-initiated-load declaration: either not declared, or declared with its three
/// mandatory parts.
/// </summary>
/// <remarks>
/// A declared declaration missing any part, or carrying an unbounded maximum, is rejected at
/// catalog construction by a thrown exception rather than by an invalid-state outcome later. The
/// composition root is on the stack at that point, which is where a wiring defect belongs.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=536012
// Broiler-Human: PENDING
public sealed class VmGuestLoadDeclaration
{
    private VmGuestLoadDeclaration(
        VmDeclaration kind,
        int minimumProviderCapabilityVersion,
        VmGuestLoadBounds profileHardMaxima,
        uint verifierWorkToFuelRate)
    {
        Kind = kind;
        MinimumProviderCapabilityVersion = minimumProviderCapabilityVersion;
        ProfileHardMaxima = profileHardMaxima;
        VerifierWorkToFuelRate = verifierWorkToFuelRate;
    }

    /// <summary>The declaration of a profile that never requests code while executing.</summary>
    public static VmGuestLoadDeclaration NotDeclared { get; } =
        new(VmDeclaration.NotDeclared, 0, VmGuestLoadBounds.None, 0);

    /// <summary>The declaration of a profile that may request code while executing.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E6369
    // Broiler-Human: PENDING
    public static VmGuestLoadDeclaration Declared(
        int minimumProviderCapabilityVersion,
        VmGuestLoadBounds profileHardMaxima,
        uint verifierWorkToFuelRate) =>
        new(VmDeclaration.Declared, minimumProviderCapabilityVersion, profileHardMaxima, verifierWorkToFuelRate);

    /// <summary>Whether guest-initiated loads are declared.</summary>
    public VmDeclaration Kind { get; }

    /// <summary>The lowest artifact-provider capability version this profile can work with.</summary>
    public int MinimumProviderCapabilityVersion { get; }

    /// <summary>The profile's own hard maxima on nested loads. A composition may tighten, never loosen.</summary>
    public VmGuestLoadBounds ProfileHardMaxima { get; }

    /// <summary>
    /// How many fuel units one unit of nested verifier work costs the requesting operation. It is
    /// how nested verification is charged to the operation that asked for it rather than to a
    /// separate allowance.
    /// </summary>
    public uint VerifierWorkToFuelRate { get; }

    /// <summary>Whether a declared declaration carries every part it must.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AADA84
    // Broiler-Human: PENDING
    public bool IsWellFormed =>
        Kind is VmDeclaration.NotDeclared
            ? MinimumProviderCapabilityVersion == 0 &&
              ProfileHardMaxima.Equals(VmGuestLoadBounds.None) &&
              VerifierWorkToFuelRate == 0
            : MinimumProviderCapabilityVersion >= 1 &&
              ProfileHardMaxima.IsFinite &&
              ProfileHardMaxima.IsPositive &&
              VerifierWorkToFuelRate >= 1;
}

/// <summary>
/// The closed range of payload kind identifiers a VM profile may stamp on its typed payloads.
/// </summary>
/// <remarks>
/// The core validates membership and attaches no meaning to any value. Range membership is how a
/// payload minted by one profile is recognised as foreign when it appears on another profile's
/// result, without the core learning what any kind means.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C7A5E0
// Broiler-Human: PENDING
public readonly struct VmPayloadKindIdRange : System.IEquatable<VmPayloadKindIdRange>
{
    /// <summary>Creates an inclusive range.</summary>
    public VmPayloadKindIdRange(int minInclusive, int maxInclusive)
    {
        MinInclusive = minInclusive;
        MaxInclusive = maxInclusive;
    }

    /// <summary>The lowest kind ID the profile may use.</summary>
    public int MinInclusive { get; }

    /// <summary>The highest kind ID the profile may use.</summary>
    public int MaxInclusive { get; }

    /// <summary>True when the range is non-empty and does not start below zero.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4DED19
    // Broiler-Human: PENDING
    public bool IsWellFormed => MinInclusive >= 0 && MinInclusive <= MaxInclusive;

    /// <summary>Whether <paramref name="payloadKindId"/> lies in the range.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FF4A8F
    // Broiler-Human: PENDING
    public bool Contains(int payloadKindId) =>
        payloadKindId >= MinInclusive && payloadKindId <= MaxInclusive;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3F9751
    // Broiler-Human: PENDING
    public bool Equals(VmPayloadKindIdRange other) =>
        MinInclusive == other.MinInclusive && MaxInclusive == other.MaxInclusive;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmPayloadKindIdRange other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(MinInclusive, MaxInclusive);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmPayloadKindIdRange left, VmPayloadKindIdRange right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=66FED0
    // Broiler-Human: PENDING
    public static bool operator !=(VmPayloadKindIdRange left, VmPayloadKindIdRange right) => !left.Equals(right);
}
