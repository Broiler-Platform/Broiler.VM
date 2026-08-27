namespace Broiler.VM;

/// <summary>
/// The four non-aggregate ceilings a bounded read is performed under: artifact bytes, section
/// count, declared count, and structural depth.
/// </summary>
/// <remarks>
/// <para>
/// ADR 0007's precedence algorithm requires the frozen effective policy to be a <em>required
/// parameter</em> of every bounded reader and bounded-allocation primitive, so that "allocate
/// before materializing" fails to compile rather than being asserted at run time. That policy is
/// a vector over the contract's fifteen budget dimensions, and ADR 0001 forbids this assembly
/// from naming contract vocabulary: bounded reading is mechanism and must not acquire it.
/// </para>
/// <para>
/// The two rules are reconciled by projection rather than by reference. This type carries the
/// four numbers a reader actually needs, as plain integers with no dimension identity, and the
/// party that holds both vocabularies performs the projection: the runtime for its own gates, the
/// profile for its verifier. ADR 0007's stated property is preserved exactly - the policy is
/// still a required constructor parameter - while ADR 0001's graph rule stays mechanically true.
/// </para>
/// </remarks>
public readonly struct VmReadBounds : System.IEquatable<VmReadBounds>
{
    /// <summary>
    /// Creates a bound set. Every value is a ceiling, and zero means nothing may pass: there is
    /// no member spelling "unbounded", because an omitted bound is what this type exists to make
    /// unrepresentable.
    /// </summary>
    public VmReadBounds(
        ulong maxArtifactBytes,
        ulong maxSectionCount,
        ulong maxDeclaredCount,
        ulong maxStructuralDepth)
    {
        MaxArtifactBytes = maxArtifactBytes;
        MaxSectionCount = maxSectionCount;
        MaxDeclaredCount = maxDeclaredCount;
        MaxStructuralDepth = maxStructuralDepth;
    }

    /// <summary>The largest artifact, in bytes, a reader may traverse.</summary>
    public ulong MaxArtifactBytes { get; }

    /// <summary>The largest number of sections a reader may enter across one artifact.</summary>
    public ulong MaxSectionCount { get; }

    /// <summary>The largest untrusted declared count that may be read and then acted upon.</summary>
    public ulong MaxDeclaredCount { get; }

    /// <summary>The deepest section nesting a reader may reach.</summary>
    public ulong MaxStructuralDepth { get; }

    /// <inheritdoc/>
    public bool Equals(VmReadBounds other) =>
        MaxArtifactBytes == other.MaxArtifactBytes &&
        MaxSectionCount == other.MaxSectionCount &&
        MaxDeclaredCount == other.MaxDeclaredCount &&
        MaxStructuralDepth == other.MaxStructuralDepth;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmReadBounds other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(MaxArtifactBytes, MaxSectionCount, MaxDeclaredCount, MaxStructuralDepth);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmReadBounds left, VmReadBounds right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmReadBounds left, VmReadBounds right) => !left.Equals(right);
}
