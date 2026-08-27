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
public readonly struct VmLimitVector : System.IEquatable<VmLimitVector>
{
    private readonly ulong[]? values;

    private VmLimitVector(ulong[] values) => this.values = values;

    /// <summary>
    /// The vector with every dimension at TOP. Only a profile hard maximum and the
    /// <c>LiveRuntimes</c> slot of an unparented runtime may legitimately be this.
    /// </summary>
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
    public bool IsEmpty => values is null;

    /// <summary>The value declared for <paramref name="dimension"/>.</summary>
    public ulong this[VmBudgetDimension dimension] =>
        values is null
            ? 0
            : values[(int)dimension];

    /// <summary>Whether <paramref name="dimension"/> is at TOP.</summary>
    public bool IsUnconstrained(VmBudgetDimension dimension) =>
        this[dimension] == ulong.MaxValue;

    /// <summary>Whether any dimension is at TOP.</summary>
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
public readonly struct VmBudgetDeclarationMatrix : System.IEquatable<VmBudgetDeclarationMatrix>
{
    private readonly VmBudgetApplicability[]? rows;

    private VmBudgetDeclarationMatrix(VmBudgetApplicability[] rows) => this.rows = rows;

    /// <summary>True when every one of the fifteen rows is present.</summary>
    public bool IsComplete => rows is not null;

    /// <summary>The declaration for <paramref name="dimension"/>.</summary>
    public VmBudgetApplicability this[VmBudgetDimension dimension] =>
        rows is null ? VmBudgetApplicability.NotApplicable : rows[(int)dimension];

    /// <summary>
    /// Creates a matrix from exactly fifteen rows in the frozen dimension order. A span of any
    /// other length is refused rather than padded with a default, because a defaulted row is
    /// silence in code form.
    /// </summary>
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
    public bool IsFinite =>
        NestedLoadDepth != ulong.MaxValue &&
        NestedLoadFanOut != ulong.MaxValue &&
        NestedLoadBytes != ulong.MaxValue &&
        VerifierWork != ulong.MaxValue;

    /// <summary>True when every bound is greater than zero.</summary>
    public bool IsPositive =>
        NestedLoadDepth > 0 && NestedLoadFanOut > 0 && NestedLoadBytes > 0 && VerifierWork > 0;

    /// <inheritdoc/>
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
    public static bool operator !=(VmGuestLoadBounds left, VmGuestLoadBounds right) => !left.Equals(right);
}
