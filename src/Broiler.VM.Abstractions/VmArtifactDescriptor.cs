namespace Broiler.VM;

/// <summary>
/// The caller-supplied description of an artifact: which profile it belongs to, which format
/// version and feature manifest it claims, what limits it requests, and what the caller calls it.
/// </summary>
/// <remarks>
/// <para>
/// It carries <strong>no core contract version</strong> and may never gain one. The contract version
/// is a property of the core and of the profile that was built against it, not of a payload: an
/// artifact that declared one would be asserting something about the host it is presented to.
/// </para>
/// <para>
/// <see cref="RequestedLimits"/> may only tighten. A limit an artifact omits adds no restriction
/// and, critically, does not remove the materialized ceiling either - it is read from the descriptor
/// alone and never from the payload, because a limit read out of the payload would require reading
/// untrusted bytes before a policy exists to read them under.
/// </para>
/// <para>
/// The descriptor is copied by value at the verification entry point, so mutating the caller's copy
/// afterwards cannot change what a verified handle is bound to.
/// </para>
/// </remarks>
public readonly struct VmArtifactDescriptor : System.IEquatable<VmArtifactDescriptor>
{
    /// <summary>Creates an artifact descriptor.</summary>
    public VmArtifactDescriptor(
        VmProfileId profileId,
        uint formatVersion,
        VmFeatureManifestId featureManifestId,
        VmLimitVector requestedLimits,
        VmCallerIdentity callerIdentity)
    {
        ProfileId = profileId;
        FormatVersion = formatVersion;
        FeatureManifestId = featureManifestId;
        RequestedLimits = requestedLimits;
        CallerIdentity = callerIdentity;
    }

    /// <summary>The profile whose verifier owns these bytes. There is no probing of alternatives.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Exactly one profile-format version. Not a range.</summary>
    public uint FormatVersion { get; }

    /// <summary>Exactly one feature manifest. Not a set.</summary>
    public VmFeatureManifestId FeatureManifestId { get; }

    /// <summary>
    /// Limits the artifact requests. They may only tighten the host and profile ceilings; a vector
    /// left empty requests nothing and removes nothing.
    /// </summary>
    public VmLimitVector RequestedLimits { get; }

    /// <summary>The caller's own identity for these bytes, echoed into diagnostics and never parsed.</summary>
    public VmCallerIdentity CallerIdentity { get; }

    /// <summary>
    /// Whether the descriptor is structurally usable: a present identity and manifest, a non-zero
    /// format version, and a manifest under the named profile's namespace.
    /// </summary>
    public bool IsWellFormed =>
        !ProfileId.IsEmpty &&
        FormatVersion >= 1 &&
        !FeatureManifestId.IsEmpty &&
        FeatureManifestId.StartsWithProfileNamespace(ProfileId);

    /// <inheritdoc/>
    public bool Equals(VmArtifactDescriptor other) =>
        ProfileId.Equals(other.ProfileId) &&
        FormatVersion == other.FormatVersion &&
        FeatureManifestId.Equals(other.FeatureManifestId) &&
        RequestedLimits.Equals(other.RequestedLimits) &&
        CallerIdentity.Equals(other.CallerIdentity);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmArtifactDescriptor other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(ProfileId, FormatVersion, FeatureManifestId, RequestedLimits, CallerIdentity);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmArtifactDescriptor left, VmArtifactDescriptor right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmArtifactDescriptor left, VmArtifactDescriptor right) => !left.Equals(right);
}
