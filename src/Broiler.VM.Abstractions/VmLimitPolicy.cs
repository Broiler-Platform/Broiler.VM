// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           16
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// One host-stated value for one budget dimension at one scope, supplied when an instance or an
/// operation is created.
/// </summary>
/// <remarks>
/// <para>
/// An override may only tighten. It is stated by trusted code, so a value looser than the one it
/// would replace is <em>refused</em> rather than clamped: silently clamping would discard an
/// instruction from inside the trust boundary and leave a runtime that dies under load with the
/// blame attached to the artifact. That is the asymmetry with an artifact-requested limit, which is
/// clamped precisely because it is stated from outside the boundary.
/// </para>
/// <para>
/// There is no "unset" value and no sentinel meaning "inherit". Inheriting is expressed by not
/// stating an override at all, which is why the whole set is optional and an omitted dimension
/// contributes no constraint at this layer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=9266BA
// Broiler-Human:        PENDING
public readonly struct VmLimitOverride : System.IEquatable<VmLimitOverride>
{
    /// <summary>Creates an override of one dimension.</summary>
    public VmLimitOverride(VmBudgetDimension dimension, ulong value)
    {
        Dimension = dimension;
        Value = value;
    }

    /// <summary>The dimension this override states.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>The value it states. It may be no looser than the value it would replace.</summary>
    public ulong Value { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=107BE1
    // Broiler-Human:        PENDING
    public bool Equals(VmLimitOverride other) =>
        Dimension == other.Dimension && Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmLimitOverride other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=017DC5
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine((int)Dimension, Value);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmLimitOverride left, VmLimitOverride right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D52720
    // Broiler-Human:        PENDING
    public static bool operator !=(VmLimitOverride left, VmLimitOverride right) => !left.Equals(right);
}

/// <summary>
/// The set of overrides one instantiation or one invocation states. An empty set inherits
/// everything.
/// </summary>
/// <remarks>
/// <para>
/// This is deliberately not a <see cref="VmLimitVector"/>. A vector has a value in every one of the
/// fifteen slots and no way to spell "say nothing about this dimension", so a host that wanted to
/// tighten one dimension would have to restate the other fourteen - and restating a value it read
/// back is exactly how an override meant to tighten one thing quietly loosens another.
/// </para>
/// <para>
/// The set is validated as a whole before any of it is applied: a dimension outside the closed
/// fifteen, a dimension stated twice, a dimension the scope table does not admit at this scope, and
/// a value that would raise a bound are each refused, and a refused set applies nothing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=974BBD
// Broiler-Human:        PENDING
public readonly struct VmLimitOverrides : System.IEquatable<VmLimitOverrides>
{
    private readonly System.Collections.Immutable.ImmutableArray<VmLimitOverride> entries;

    private VmLimitOverrides(System.Collections.Immutable.ImmutableArray<VmLimitOverride> entries) =>
        this.entries = entries;

    /// <summary>The empty set: every dimension inherits.</summary>
    public static VmLimitOverrides None => default;

    /// <summary>Creates a set from the stated overrides, in the order given.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=2; Fingerprint=767D9F
    // Broiler-Human:        PENDING
    public static VmLimitOverrides Create(System.ReadOnlySpan<VmLimitOverride> overrides)
    {
        if (overrides.IsEmpty)
        {
            return default;
        }

        var builder = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmLimitOverride>(overrides.Length);

        foreach (var entry in overrides)
        {
            builder.Add(entry);
        }

        return new VmLimitOverrides(builder.MoveToImmutable());
    }

    /// <summary>Creates a set stating exactly one dimension.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=94CDA7
    // Broiler-Human:        PENDING
    public static VmLimitOverrides Of(VmBudgetDimension dimension, ulong value) =>
        new(System.Collections.Immutable.ImmutableArray.Create(new VmLimitOverride(dimension, value)));

    /// <summary>True when nothing is stated and every dimension inherits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F72A4C
    // Broiler-Human:        PENDING
    public bool IsEmpty => entries.IsDefaultOrEmpty;

    /// <summary>How many overrides are stated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1AFA64
    // Broiler-Human:        PENDING
    public int Count => entries.IsDefault ? 0 : entries.Length;

    /// <summary>The override at <paramref name="index"/>, in the order it was stated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1A75A8
    // Broiler-Human:        PENDING
    public VmLimitOverride this[int index] => entries[index];

    /// <summary>Whether <paramref name="dimension"/> is stated, and what it states.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=4CE894
    // Broiler-Human:        PENDING
    public bool TryGetValue(VmBudgetDimension dimension, out ulong value)
    {
        if (!entries.IsDefault)
        {
            foreach (var entry in entries)
            {
                if (entry.Dimension == dimension)
                {
                    value = entry.Value;
                    return true;
                }
            }
        }

        value = 0;
        return false;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=853481
    // Broiler-Human:        PENDING
    public bool Equals(VmLimitOverrides other)
    {
        if (Count != other.Count)
        {
            return false;
        }

        for (var index = 0; index < Count; index++)
        {
            if (!entries[index].Equals(other.entries[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmLimitOverrides other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9F90E3
    // Broiler-Human:        PENDING
    public override int GetHashCode()
    {
        var hash = new System.HashCode();

        for (var index = 0; index < Count; index++)
        {
            hash.Add(entries[index]);
        }

        return hash.ToHashCode();
    }

    /// <summary>Value equality, order-sensitive.</summary>
    public static bool operator ==(VmLimitOverrides left, VmLimitOverrides right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=375F51
    // Broiler-Human:        PENDING
    public static bool operator !=(VmLimitOverrides left, VmLimitOverrides right) => !left.Equals(right);
}

/// <summary>
/// One artifact-requested limit that the host and profile intersection tightened, recorded on the
/// handle the verification produced.
/// </summary>
/// <remarks>
/// <para>
/// An artifact speaks from outside the trust boundary, so a request above the intersection is
/// clamped and the artifact is <strong>not</strong> rejected: rejecting it would turn a request into
/// a requirement, and the same safe bytes would then fail on a tighter host even though nothing in
/// them needs the larger limit.
/// </para>
/// <para>
/// The record is carried on the verified handle rather than in <see cref="VmDiagnostics"/>, whose
/// field set is frozen, and it is never crossed to the profile, which learns a limit only by
/// reaching it. The quantities here are the caller's own request and the ceiling the composition
/// already chose, so naming them to the caller discloses nothing the caller did not supply or
/// configure.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=8B2425
// Broiler-Human:        PENDING
public readonly struct VmLimitClamp : System.IEquatable<VmLimitClamp>
{
    /// <summary>Creates a clamp record.</summary>
    public VmLimitClamp(VmBudgetDimension dimension, ulong requested, ulong effective)
    {
        Dimension = dimension;
        Requested = requested;
        Effective = effective;
    }

    /// <summary>The dimension whose request was tightened.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>What the artifact descriptor asked for.</summary>
    public ulong Requested { get; }

    /// <summary>What it was given: the host and profile intersection.</summary>
    public ulong Effective { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FE35CA
    // Broiler-Human:        PENDING
    public bool Equals(VmLimitClamp other) =>
        Dimension == other.Dimension && Requested == other.Requested && Effective == other.Effective;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmLimitClamp other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E0AFCB
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine((int)Dimension, Requested, Effective);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmLimitClamp left, VmLimitClamp right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7FC179
    // Broiler-Human:        PENDING
    public static bool operator !=(VmLimitClamp left, VmLimitClamp right) => !left.Equals(right);
}
