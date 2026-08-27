namespace Broiler.VM;

/// <summary>
/// The inclusive range of profile-format versions one VM profile accepts.
/// </summary>
/// <remarks>
/// Version <c>0</c> is reserved as "unset" and is rejected, so a descriptor that forgot the field
/// fails at catalog construction rather than silently declaring an empty or universal range.
/// <see langword="default"/> is therefore <c>(0,0)</c>, which is not well formed - no separate
/// sentinel is added, because a second way to spell "unset" is a second thing to check.
/// </remarks>
public readonly struct VmFormatVersionRange : System.IEquatable<VmFormatVersionRange>
{
    /// <summary>Creates an inclusive range.</summary>
    public VmFormatVersionRange(uint min, uint max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>The lowest accepted profile-format version.</summary>
    public uint Min { get; }

    /// <summary>The highest accepted profile-format version.</summary>
    public uint Max { get; }

    /// <summary>True when <c>1 &lt;= Min &lt;= Max</c>.</summary>
    public bool IsWellFormed => Min >= 1 && Min <= Max;

    /// <summary>Whether <paramref name="formatVersion"/> falls inside the inclusive range.</summary>
    public bool Contains(uint formatVersion) => formatVersion >= Min && formatVersion <= Max;

    /// <inheritdoc/>
    public bool Equals(VmFormatVersionRange other) => Min == other.Min && Max == other.Max;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmFormatVersionRange other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Min, Max);

    /// <inheritdoc/>
    public override string ToString() => Min + ".." + Max;

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmFormatVersionRange left, VmFormatVersionRange right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmFormatVersionRange left, VmFormatVersionRange right) => !left.Equals(right);
}

/// <summary>
/// A stable, non-localized, namespaced host-capability identity.
/// </summary>
/// <remarks>
/// ADR 0011 field F1 states that the capability-ID policy mirrors the profile-ID policy exactly,
/// and that policy bans a raw <see cref="string"/> wherever an identity is stored or matched. So
/// this is a validated value type over the same grammar, with the same reserved
/// <c>Broiler.</c> first label under the same ASCII fold.
/// </remarks>
public readonly struct VmCapabilityId
    : System.IEquatable<VmCapabilityId>, System.IComparable<VmCapabilityId>
{
    private readonly string? text;

    private VmCapabilityId(string text) => this.text = text;

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => text is null;

    /// <summary>True when the first label folds to <c>broiler</c>.</summary>
    public bool IsReservedNamespace
    {
        get
        {
            if (text is null)
            {
                return false;
            }

            var dot = text.IndexOf('.');
            var first = dot < 0
                ? System.MemoryExtensions.AsSpan(text)
                : System.MemoryExtensions.AsSpan(text, 0, dot);

            if (first.Length != 7)
            {
                return false;
            }

            System.ReadOnlySpan<char> reserved = "broiler";

            for (var index = 0; index < 7; index++)
            {
                if (VmProfileId.FoldAscii(first[index]) != reserved[index])
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>The ID as a span, without allocating.</summary>
    public System.ReadOnlySpan<char> AsSpan() => System.MemoryExtensions.AsSpan(text);

    /// <summary>Parses <paramref name="candidate"/>, returning <see langword="false"/> when it does not satisfy the grammar.</summary>
    public static bool TryParse(System.ReadOnlySpan<char> candidate, out VmCapabilityId id)
    {
        id = default;

        if (!VmProfileId.TryValidateGrammar(
                candidate,
                VmProfileId.MinimumLabelCount,
                VmProfileId.MaximumLabelCount,
                VmProfileId.MinimumLength,
                VmProfileId.MaximumLength,
                out _))
        {
            return false;
        }

        id = new VmCapabilityId(candidate.ToString());
        return true;
    }

    /// <summary>Parses <paramref name="candidate"/> or throws.</summary>
    /// <exception cref="System.ArgumentException">The candidate does not satisfy the grammar.</exception>
    public static VmCapabilityId Parse(System.ReadOnlySpan<char> candidate)
    {
        if (!TryParse(candidate, out var id))
        {
            throw new System.ArgumentException(
                "The value is not a well-formed capability ID; the grammar is the profile-ID grammar.",
                nameof(candidate));
        }

        return id;
    }

    /// <inheritdoc/>
    public bool Equals(VmCapabilityId other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <inheritdoc/>
    public int CompareTo(VmCapabilityId other) =>
        string.CompareOrdinal(text ?? string.Empty, other.text ?? string.Empty);

    /// <inheritdoc/>
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmCapabilityId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmCapabilityId left, VmCapabilityId right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    public static bool operator !=(VmCapabilityId left, VmCapabilityId right) => !left.Equals(right);
}

/// <summary>
/// A stable identifier of a host capability's parameter and return shape.
/// </summary>
/// <remarks>
/// It is derived from a canonical description that both the host registration and the profile
/// import declare independently, so a mismatch is refused at binding rather than discovered at
/// first call. The core never parses the description: it compares the derived identifier.
/// </remarks>
public readonly struct VmCapabilitySignatureId : System.IEquatable<VmCapabilitySignatureId>
{
    private readonly string? text;

    private VmCapabilitySignatureId(string text) => this.text = text;

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => text is null;

    /// <summary>
    /// Derives a signature identity from a canonical description. The description is stored
    /// verbatim and compared ordinally; the core attaches no meaning to its internal structure.
    /// </summary>
    public static VmCapabilitySignatureId FromCanonicalDescription(System.ReadOnlySpan<char> canonical) =>
        canonical.IsEmpty ? default : new VmCapabilitySignatureId(canonical.ToString());

    /// <inheritdoc/>
    public bool Equals(VmCapabilitySignatureId other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmCapabilitySignatureId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmCapabilitySignatureId left, VmCapabilitySignatureId right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    public static bool operator !=(VmCapabilitySignatureId left, VmCapabilitySignatureId right) => !left.Equals(right);
}

/// <summary>
/// The opaque identity of a VM profile's conformance corpus.
/// </summary>
/// <remarks>
/// It is used for support tables and evidence only and never for matching. It is a distinct type
/// rather than a string precisely so that a drift test can prove it never reaches a matching path.
/// </remarks>
public readonly struct VmConformanceManifestId : System.IEquatable<VmConformanceManifestId>
{
    private readonly string? text;

    private VmConformanceManifestId(string text) => this.text = text;

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => text is null;

    /// <summary>Creates an identity from an opaque token.</summary>
    public static VmConformanceManifestId Create(System.ReadOnlySpan<char> token) =>
        token.IsEmpty ? default : new VmConformanceManifestId(token.ToString());

    /// <inheritdoc/>
    public bool Equals(VmConformanceManifestId other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmConformanceManifestId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmConformanceManifestId left, VmConformanceManifestId right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    public static bool operator !=(VmConformanceManifestId left, VmConformanceManifestId right) => !left.Equals(right);
}

/// <summary>
/// A VM profile's opaque diagnostics token, which must lie under that profile's own ID namespace.
/// </summary>
/// <remarks>
/// The namespace requirement is enforced at creation so that a malformed token can be reported at
/// registration, where the composition root that wrote it is on the stack.
/// </remarks>
public readonly struct VmDiagnosticsIdentity : System.IEquatable<VmDiagnosticsIdentity>
{
    private readonly string? text;

    private VmDiagnosticsIdentity(string text) => this.text = text;

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => text is null;

    /// <summary>
    /// Creates a diagnostics identity under <paramref name="owner"/>, returning
    /// <see langword="false"/> when the token does not lie in that profile's namespace.
    /// </summary>
    public static bool TryCreate(
        VmProfileId owner,
        System.ReadOnlySpan<char> token,
        out VmDiagnosticsIdentity identity)
    {
        identity = default;

        if (owner.IsEmpty || token.IsEmpty)
        {
            return false;
        }

        var ownerSpan = owner.AsSpan();

        if (token.Length <= ownerSpan.Length + 1 ||
            !System.MemoryExtensions.SequenceEqual(token[..ownerSpan.Length], ownerSpan) ||
            token[ownerSpan.Length] != '.')
        {
            return false;
        }

        identity = new VmDiagnosticsIdentity(token.ToString());
        return true;
    }

    /// <inheritdoc/>
    public bool Equals(VmDiagnosticsIdentity other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <inheritdoc/>
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmDiagnosticsIdentity other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmDiagnosticsIdentity left, VmDiagnosticsIdentity right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    public static bool operator !=(VmDiagnosticsIdentity left, VmDiagnosticsIdentity right) => !left.Equals(right);
}

/// <summary>
/// Release-engineering metadata about the package a VM profile ships in.
/// </summary>
/// <remarks>
/// <see cref="PackageId"/> participates only in the reserved-namespace self-consistency check at
/// catalog construction; <see cref="PackageVersion"/> participates in nothing and is excluded from
/// the canonical catalog encoding. A raw <see cref="string"/> is admissible here, unlike for an
/// identity, precisely because these are inert data the core neither stores as an identity nor
/// matches on.
/// </remarks>
public readonly struct VmPackageIdentity : System.IEquatable<VmPackageIdentity>
{
    /// <summary>Creates a package identity. No part may be empty.</summary>
    public VmPackageIdentity(string packageId, string packageVersion, string ownerTag)
    {
        PackageId = packageId;
        PackageVersion = packageVersion;
        OwnerTag = ownerTag;
    }

    /// <summary>The package ID the profile ships under.</summary>
    public string PackageId { get; }

    /// <summary>The package version. It participates in nothing mechanical.</summary>
    public string PackageVersion { get; }

    /// <summary>An owner tag used in support tables and evidence.</summary>
    public string OwnerTag { get; }

    /// <summary>True when every part is present and non-blank.</summary>
    public bool IsComplete =>
        !string.IsNullOrWhiteSpace(PackageId) &&
        !string.IsNullOrWhiteSpace(PackageVersion) &&
        !string.IsNullOrWhiteSpace(OwnerTag);

    /// <inheritdoc/>
    public bool Equals(VmPackageIdentity other) =>
        string.Equals(PackageId, other.PackageId, System.StringComparison.Ordinal) &&
        string.Equals(PackageVersion, other.PackageVersion, System.StringComparison.Ordinal) &&
        string.Equals(OwnerTag, other.OwnerTag, System.StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmPackageIdentity other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(PackageId, PackageVersion, OwnerTag);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmPackageIdentity left, VmPackageIdentity right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmPackageIdentity left, VmPackageIdentity right) => !left.Equals(right);
}

/// <summary>
/// The opaque, process-local identity carried by every lifecycle object.
/// </summary>
/// <remarks>
/// It is stable for the object's lifetime and never reused within the process, and it is never
/// derived from an address, a secret or host state - a diagnostics field that leaked an address
/// would be an information disclosure in a type whose whole purpose is to be safe to log.
/// </remarks>
public readonly struct VmObjectId : System.IEquatable<VmObjectId>
{
    private readonly ulong value;

    private VmObjectId(ulong value) => this.value = value;

    /// <summary>True when this is <see langword="default"/>, meaning no object is identified.</summary>
    public bool IsEmpty => value == 0;

    /// <summary>
    /// Mints the next identity in the process-wide sequence. Public because the fixture profile and
    /// the contract suite must be able to observe identity behaviour through the public surface
    /// alone: ADR 0001 rule A10 forbids <c>InternalsVisibleTo</c> in a product project.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmObjectId Mint() =>
        new(unchecked((ulong)System.Threading.Interlocked.Increment(ref counter)));

    private static long counter;

    /// <inheritdoc/>
    public bool Equals(VmObjectId other) => value == other.value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmObjectId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => value.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmObjectId left, VmObjectId right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmObjectId left, VmObjectId right) => !left.Equals(right);
}

/// <summary>
/// The per-runtime opaque transfer reference: a value a host capability may hand back to guest
/// code without the guest being able to dereference, forge or carry it elsewhere.
/// </summary>
/// <remarks>
/// <para>
/// It is runtime-scoped and generation-stamped. Presented to a capability of another runtime it is
/// a host failure naming a foreign reference; presented after its generation has been invalidated,
/// a host failure naming a stale one. Both are refusals rather than undefined behaviour.
/// </para>
/// <para>
/// The name is deliberately not <c>VmHandle</c>. Unqualified "handle" always means the
/// verified-artifact handle, and a second type called <c>VmHandle</c> beside it is exactly the
/// ambiguity ADR 0003's qualifier rule exists to prevent.
/// </para>
/// </remarks>
public readonly struct VmOpaqueRef : System.IEquatable<VmOpaqueRef>
{
    private readonly ulong runtime;
    private readonly ulong generation;
    private readonly ulong slot;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmOpaqueRef(ulong runtime, ulong generation, ulong slot)
    {
        this.runtime = runtime;
        this.generation = generation;
        this.slot = slot;
    }

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => runtime == 0 && generation == 0 && slot == 0;

    /// <summary>The identity of the runtime this reference belongs to.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public ulong OwningRuntime => runtime;

    /// <summary>The generation stamp that makes a stale reference detectable.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public ulong Generation => generation;

    /// <summary>The slot within the owning runtime. It is not an address and is not dereferenceable.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public ulong Slot => slot;

    /// <inheritdoc/>
    public bool Equals(VmOpaqueRef other) =>
        runtime == other.runtime && generation == other.generation && slot == other.slot;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmOpaqueRef other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(runtime, generation, slot);

    /// <summary>Identity equality.</summary>
    public static bool operator ==(VmOpaqueRef left, VmOpaqueRef right) => left.Equals(right);

    /// <summary>Identity inequality.</summary>
    public static bool operator !=(VmOpaqueRef left, VmOpaqueRef right) => !left.Equals(right);
}
