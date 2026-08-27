namespace Broiler.VM;

/// <summary>
/// A section or segment framing token: the unit <c>TryEnterSection</c> and <c>TryExitSection</c>
/// balance, and the unit section count and structural depth are counted over.
/// </summary>
/// <remarks>
/// The frame records where a section body began and how long it declared itself to be, so that
/// exiting can check the section consumed exactly what it claimed. A profile that under-reads or
/// over-reads a section is a structural error the frame catches, rather than one the profile has
/// to remember to look for.
/// </remarks>
public readonly struct VmSectionFrame : System.IEquatable<VmSectionFrame>
{
    internal VmSectionFrame(ulong start, ulong declaredLength, uint depth)
    {
        Start = start;
        DeclaredLength = declaredLength;
        Depth = depth;
    }

    /// <summary>The reader position at which the section body began.</summary>
    public ulong Start { get; }

    /// <summary>The length the section declared for itself, in bytes.</summary>
    public ulong DeclaredLength { get; }

    /// <summary>The nesting depth of this section, counting from one.</summary>
    public uint Depth { get; }

    /// <inheritdoc/>
    public bool Equals(VmSectionFrame other) =>
        Start == other.Start && DeclaredLength == other.DeclaredLength && Depth == other.Depth;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmSectionFrame other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Start, DeclaredLength, Depth);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmSectionFrame left, VmSectionFrame right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmSectionFrame left, VmSectionFrame right) => !left.Equals(right);
}
