// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           15
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Resource impact:  1/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// A core-owned, bounded, read-only view over bytes crossing the host boundary.
/// </summary>
/// <remarks>
/// A <c>ref struct</c>, so the call-scoped, non-retainable lifetime is enforced by the type system
/// rather than documented. A profile cannot store one in a field, put one in a collection, or carry
/// one across an await - the last of which is also why an asynchronous host capability is not
/// representable.
/// </remarks>
public readonly ref struct VmBytes
{
    private readonly System.ReadOnlySpan<byte> span;

    /// <summary>Wraps a span for the duration of one call.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C880D9
    // Broiler-Human: PENDING
    public VmBytes(System.ReadOnlySpan<byte> bytes) => span = bytes;

    /// <summary>The bytes.</summary>
    public System.ReadOnlySpan<byte> Span => span;

    /// <summary>How many bytes there are.</summary>
    public int Length => span.Length;

    /// <summary>True when there are no bytes.</summary>
    public bool IsEmpty => span.IsEmpty;
}

/// <summary>
/// A core-owned, bounded, read-only view over UTF-8 text crossing the host boundary.
/// </summary>
/// <remarks>
/// Text crosses the boundary as UTF-8 bytes rather than as a <see cref="string"/> so that no
/// encoding, normalization or culture decision is made by the core on behalf of a profile whose
/// language may have its own rules for all three.
/// </remarks>
public readonly ref struct VmUtf8Text
{
    private readonly System.ReadOnlySpan<byte> utf8;

    /// <summary>Wraps UTF-8 bytes for the duration of one call.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3F6F22
    // Broiler-Human: PENDING
    public VmUtf8Text(System.ReadOnlySpan<byte> bytes) => utf8 = bytes;

    /// <summary>The UTF-8 bytes.</summary>
    public System.ReadOnlySpan<byte> Utf8 => utf8;

    /// <summary>How many bytes there are.</summary>
    public int Length => utf8.Length;

    /// <summary>True when there is no text.</summary>
    public bool IsEmpty => utf8.IsEmpty;
}

/// <summary>
/// The identity a VM profile stamps on one of its typed payloads.
/// </summary>
/// <remarks>
/// The core inspects exactly this, for exactly three purposes: rejecting a payload whose profile ID
/// or kind ID does not belong to the profile that produced the result, recording it in diagnostics,
/// and enforcing the empty-slot rules. It never calls <c>ToString</c>, <c>GetHashCode</c> or
/// <c>Equals</c> on the payload itself, never pattern-matches on its concrete type, and never
/// stores, clones, pools or serialises it.
/// </remarks>
public readonly struct VmPayloadIdentity : System.IEquatable<VmPayloadIdentity>
{
    /// <summary>Creates a payload identity.</summary>
    public VmPayloadIdentity(VmProfileId profileId, int payloadKindId, int payloadSchemaVersion)
    {
        ProfileId = profileId;
        PayloadKindId = payloadKindId;
        PayloadSchemaVersion = payloadSchemaVersion;
    }

    /// <summary>The profile that minted the payload.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Which of the profile's payload kinds this is; the core attaches no meaning to it.</summary>
    public int PayloadKindId { get; }

    /// <summary>The profile's own schema version for that kind.</summary>
    public int PayloadSchemaVersion { get; }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1D965E
    // Broiler-Human: PENDING
    public bool Equals(VmPayloadIdentity other) =>
        ProfileId.Equals(other.ProfileId) &&
        PayloadKindId == other.PayloadKindId &&
        PayloadSchemaVersion == other.PayloadSchemaVersion;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmPayloadIdentity other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(ProfileId, PayloadKindId, PayloadSchemaVersion);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmPayloadIdentity left, VmPayloadIdentity right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmPayloadIdentity left, VmPayloadIdentity right) => !left.Equals(right);
}

/// <summary>
/// A typed, profile-owned result or fault payload carried behind a profile-neutral envelope.
/// </summary>
/// <remarks>
/// One member, and it is identity. This is how a language outcome reaches a caller without the core
/// acquiring a case for it: the core routes the payload and never interprets it, and the profile
/// ships a static accessor its own consumers use to get the concrete type back out.
/// </remarks>
public interface IVmProfilePayload
{
    /// <summary>Which profile minted this payload, of what kind, at what schema version.</summary>
    VmPayloadIdentity Identity { get; }
}
