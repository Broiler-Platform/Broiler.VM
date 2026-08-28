// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           24
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  1/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// One immutable value per budget dimension: the shape of a limit default, a profile hard maximum,
/// an artifact request, and an effective ceiling.
/// </summary>
/// <remarks>
/// <para>
/// A vector is a fixed-length value over the fifteen frozen dimensions. There is no "unset"
/// member and no nullable slot, because omission is the thing the contract exists to make
/// impossible: a host that forgets a dimension must fail runtime creation, not inherit whatever
/// the core felt like.
/// </para>
/// <para>
/// <see cref="Unconstrained"/> is the only way to spell TOP. A profile's hard maxima may use it -
/// a profile that genuinely imposes no maximum of its own says so - and a profile's limit defaults
/// may not, which is checked at catalog construction rather than left as a convention.
/// </para>
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=256734
// Broiler-Human: PENDING
public readonly struct VmLimitVector : System.IEquatable<VmLimitVector>
{
    private readonly ulong[]? values;

    private VmLimitVector(ulong[] values) => this.values = values;

    /// <summary>
    /// The vector with every dimension at TOP. Only a profile hard maximum and the
    /// <c>LiveRuntimes</c> slot of an unparented runtime may legitimately be this.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A9FFF9
    // Broiler-Human: PENDING
    public static VmLimitVector Unconstrained
    {
        get
        {
            var top = new ulong[VmBudgetDimensions.Count];
            System.Array.Fill(top, ulong.MaxValue);
            return new VmLimitVector(top);
        }
    }

    /// <summary>True when this is <see langword="default"/>, which is not a usable vector.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D83984
    // Broiler-Human: PENDING
    public bool IsEmpty => values is null;

    /// <summary>The value declared for <paramref name="dimension"/>.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CBB5AC
    // Broiler-Human: PENDING
    public ulong this[VmBudgetDimension dimension] =>
        values is null
            ? 0
            : values[(int)dimension];

    /// <summary>Whether <paramref name="dimension"/> is at TOP.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F1F79B
    // Broiler-Human: PENDING
    public bool IsUnconstrained(VmBudgetDimension dimension) =>
        this[dimension] == ulong.MaxValue;

    /// <summary>Whether any dimension is at TOP.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=04A197
    // Broiler-Human: PENDING
    public bool HasAnyUnconstrained()
    {
        if (values is null)
        {
            return false;
        }

        foreach (var value in values)
        {
            if (value == ulong.MaxValue)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Creates a vector from exactly fifteen values, in the frozen dimension order. A span of any
    /// other length is refused rather than padded.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=02727D
    // Broiler-Human: PENDING
    public static bool TryCreate(System.ReadOnlySpan<ulong> perDimension, out VmLimitVector vector)
    {
        vector = default;

        if (perDimension.Length != VmBudgetDimensions.Count)
        {
            return false;
        }

        vector = new VmLimitVector(perDimension.ToArray());
        return true;
    }

    /// <summary>
    /// The element-wise minimum of two vectors. This is the intersection operation the precedence
    /// algorithm uses: a limit can only ever be tightened by combining, never loosened.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3B66E9
    // Broiler-Human: PENDING
    public static VmLimitVector Intersect(VmLimitVector left, VmLimitVector right)
    {
        var result = new ulong[VmBudgetDimensions.Count];

        for (var index = 0; index < result.Length; index++)
        {
            var a = left.values is null ? ulong.MaxValue : left.values[index];
            var b = right.values is null ? ulong.MaxValue : right.values[index];
            result[index] = a < b ? a : b;
        }

        return new VmLimitVector(result);
    }

    /// <summary>
    /// Whether every dimension of <paramref name="candidate"/> is no looser than the corresponding
    /// dimension of <paramref name="bound"/>.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BADFC1
    // Broiler-Human: PENDING
    public static bool IsNoLooserThan(VmLimitVector candidate, VmLimitVector bound)
    {
        for (var index = 0; index < VmBudgetDimensions.Count; index++)
        {
            var a = candidate.values is null ? ulong.MaxValue : candidate.values[index];
            var b = bound.values is null ? ulong.MaxValue : bound.values[index];

            if (a > b)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Copies the vector into <paramref name="destination"/>, which must hold fifteen values.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3B39D8
    // Broiler-Human: PENDING
    public void CopyTo(System.Span<ulong> destination)
    {
        if (destination.Length != VmBudgetDimensions.Count)
        {
            throw new System.ArgumentException(
                "A limit vector has exactly fifteen values, one per budget dimension.",
                nameof(destination));
        }

        for (var index = 0; index < VmBudgetDimensions.Count; index++)
        {
            destination[index] = values is null ? 0 : values[index];
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C759BA
    // Broiler-Human: PENDING
    public bool Equals(VmLimitVector other)
    {
        if (values is null || other.values is null)
        {
            return values is null && other.values is null;
        }

        for (var index = 0; index < values.Length; index++)
        {
            if (values[index] != other.values[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmLimitVector other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=66B94B
    // Broiler-Human: PENDING
    public override int GetHashCode()
    {
        if (values is null)
        {
            return 0;
        }

        var hash = new System.HashCode();

        foreach (var value in values)
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmLimitVector left, VmLimitVector right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C8D278
    // Broiler-Human: PENDING
    public static bool operator !=(VmLimitVector left, VmLimitVector right) => !left.Equals(right);
}

/// <summary>
/// A VM profile's fixed-length declaration of which budget dimensions it charges.
/// </summary>
/// <remarks>
/// Three rows are constrained by other records rather than by the profile's own preference.
/// <c>VerifierWork = NotApplicable</c> is illegal outright, because verification always does work.
/// <c>HostCalls = NotApplicable</c> means no host capability may be bound to that profile.
/// <c>NestedLoadDepth = NotApplicable</c> means no artifact-provider capability may be bound. All
/// three are checked at catalog construction, where the composition root is on the stack.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AC252A
// Broiler-Human: PENDING
public readonly struct VmBudgetDeclarationMatrix : System.IEquatable<VmBudgetDeclarationMatrix>
{
    private readonly VmBudgetApplicability[]? rows;

    private VmBudgetDeclarationMatrix(VmBudgetApplicability[] rows) => this.rows = rows;

    /// <summary>True when every one of the fifteen rows is present.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=65E2CC
    // Broiler-Human: PENDING
    public bool IsComplete => rows is not null;

    /// <summary>The declaration for <paramref name="dimension"/>.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F4760A
    // Broiler-Human: PENDING
    public VmBudgetApplicability this[VmBudgetDimension dimension] =>
        rows is null ? VmBudgetApplicability.NotApplicable : rows[(int)dimension];

    /// <summary>
    /// Creates a matrix from exactly fifteen rows in the frozen dimension order. A span of any
    /// other length is refused rather than padded with a default, because a defaulted row is
    /// silence in code form.
    /// </summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=965C46
    // Broiler-Human: PENDING
    public static bool TryCreate(
        System.ReadOnlySpan<VmBudgetApplicability> perDimension,
        out VmBudgetDeclarationMatrix matrix)
    {
        matrix = default;

        if (perDimension.Length != VmBudgetDimensions.Count)
        {
            return false;
        }

        foreach (var row in perDimension)
        {
            if (row is not (VmBudgetApplicability.Charged or VmBudgetApplicability.NotApplicable))
            {
                return false;
            }
        }

        matrix = new VmBudgetDeclarationMatrix(perDimension.ToArray());
        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CD356A
    // Broiler-Human: PENDING
    public bool Equals(VmBudgetDeclarationMatrix other)
    {
        if (rows is null || other.rows is null)
        {
            return rows is null && other.rows is null;
        }

        for (var index = 0; index < rows.Length; index++)
        {
            if (rows[index] != other.rows[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmBudgetDeclarationMatrix other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D5E5EF
    // Broiler-Human: PENDING
    public override int GetHashCode()
    {
        if (rows is null)
        {
            return 0;
        }

        var hash = new System.HashCode();

        foreach (var row in rows)
        {
            hash.Add((int)row);
        }

        return hash.ToHashCode();
    }

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmBudgetDeclarationMatrix left, VmBudgetDeclarationMatrix right) =>
        left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=052283
    // Broiler-Human: PENDING
    public static bool operator !=(VmBudgetDeclarationMatrix left, VmBudgetDeclarationMatrix right) =>
        !left.Equals(right);
}

/// <summary>
/// The materialized intersection of host ceilings, profile hard maxima and artifact requests,
/// frozen into a verified handle before any untrusted byte is read.
/// </summary>
/// <remarks>
/// Compared by <em>exact</em> equality in the cross-runtime sharing predicate, never by
/// subsumption. Relaxing that to element-wise subsumption - accepting a handle no looser than the
/// receiving runtime - is ADR 0003's candidate amendment 1 and is breaking, because a refusal
/// would become a success.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E97F91
// Broiler-Human: PENDING
public readonly struct VmEffectiveCeilings : System.IEquatable<VmEffectiveCeilings>
{
    /// <summary>Creates a ceiling pair.</summary>
    public VmEffectiveCeilings(VmLimitVector verificationCeilings, VmLimitVector instantiationCeilings)
    {
        VerificationCeilings = verificationCeilings;
        InstantiationCeilings = instantiationCeilings;
    }

    /// <summary>The ceilings verification ran under.</summary>
    public VmLimitVector VerificationCeilings { get; }

    /// <summary>The ceilings instantiation from this handle must run under.</summary>
    public VmLimitVector InstantiationCeilings { get; }

    /// <inheritdoc/>
    public bool Equals(VmEffectiveCeilings other) =>
        VerificationCeilings.Equals(other.VerificationCeilings) &&
        InstantiationCeilings.Equals(other.InstantiationCeilings);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmEffectiveCeilings other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(VerificationCeilings, InstantiationCeilings);

    /// <summary>Exact value equality.</summary>
    public static bool operator ==(VmEffectiveCeilings left, VmEffectiveCeilings right) => left.Equals(right);

    /// <summary>Exact value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BF42B3
    // Broiler-Human: PENDING
    public static bool operator !=(VmEffectiveCeilings left, VmEffectiveCeilings right) => !left.Equals(right);
}

/// <summary>
/// The four bounds a guest-initiated load is subject to: nesting depth, fan-out per operation,
/// cumulative nested bytes, and cumulative nested verifier work.
/// </summary>
/// <remarks>
/// Each is one of the fifteen dimensions rather than a private counter, so a nested load is
/// charged through the same meter as everything else and cannot acquire an allowance of its own.
/// The struck names <c>MaxDepth</c>, <c>MaxFanOutPerOperation</c>,
/// <c>MaxCumulativeNestedBytes</c> and <c>MaxCumulativeNestedVerifierWork</c> appear nowhere.
/// </remarks>
// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=327E45
// Broiler-Human: PENDING
public readonly struct VmGuestLoadBounds : System.IEquatable<VmGuestLoadBounds>
{
    /// <summary>Creates a bound group.</summary>
    public VmGuestLoadBounds(
        ulong nestedLoadDepth,
        ulong nestedLoadFanOut,
        ulong nestedLoadBytes,
        ulong verifierWork)
    {
        NestedLoadDepth = nestedLoadDepth;
        NestedLoadFanOut = nestedLoadFanOut;
        NestedLoadBytes = nestedLoadBytes;
        VerifierWork = verifierWork;
    }

    /// <summary>
    /// The bounds a profile that does not declare guest-initiated loads carries: all four zero.
    /// Its four matrix rows are <see cref="VmBudgetApplicability.NotApplicable"/>.
    /// </summary>
    public static VmGuestLoadBounds None => default;

    /// <summary>Deepest provider-mediated nesting.</summary>
    public ulong NestedLoadDepth { get; }

    /// <summary>Most provider requests admitted for one operation.</summary>
    public ulong NestedLoadFanOut { get; }

    /// <summary>Most provider-returned bytes for one operation.</summary>
    public ulong NestedLoadBytes { get; }

    /// <summary>Most verifier work units spent on nested verification for one operation.</summary>
    public ulong VerifierWork { get; }

    /// <summary>True when every bound is finite; TOP in any slot is refused at catalog construction.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B06362
    // Broiler-Human: PENDING
    public bool IsFinite =>
        NestedLoadDepth != ulong.MaxValue &&
        NestedLoadFanOut != ulong.MaxValue &&
        NestedLoadBytes != ulong.MaxValue &&
        VerifierWork != ulong.MaxValue;

    /// <summary>True when every bound is greater than zero.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=75C968
    // Broiler-Human: PENDING
    public bool IsPositive =>
        NestedLoadDepth > 0 && NestedLoadFanOut > 0 && NestedLoadBytes > 0 && VerifierWork > 0;

    /// <inheritdoc/>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=92B243
    // Broiler-Human: PENDING
    public bool Equals(VmGuestLoadBounds other) =>
        NestedLoadDepth == other.NestedLoadDepth &&
        NestedLoadFanOut == other.NestedLoadFanOut &&
        NestedLoadBytes == other.NestedLoadBytes &&
        VerifierWork == other.VerifierWork;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmGuestLoadBounds other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine(NestedLoadDepth, NestedLoadFanOut, NestedLoadBytes, VerifierWork);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmGuestLoadBounds left, VmGuestLoadBounds right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B1F72E
    // Broiler-Human: PENDING
    public static bool operator !=(VmGuestLoadBounds left, VmGuestLoadBounds right) => !left.Equals(right);
}
