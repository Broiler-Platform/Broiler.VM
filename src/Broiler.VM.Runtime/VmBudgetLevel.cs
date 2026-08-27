namespace Broiler.VM;

/// <summary>
/// One level of the meter chain: the ceilings declared at one scope and what has been consumed
/// against them.
/// </summary>
/// <remarks>
/// It holds no lock of its own. Charging has to be atomic across several levels at once - the
/// tie-break rule requires knowing whether an outer level would also have refused - so the lock
/// lives on the chain that owns the levels, and the levels themselves are plain state.
/// </remarks>
internal sealed class VmBudgetLevel
{
    private readonly ulong[] ceilings;
    private readonly ulong[] consumed = new ulong[VmBudgetDimensions.Count];

    internal VmBudgetLevel(VmBudgetScope scope, ulong[] ceilings)
    {
        Scope = scope;
        this.ceilings = ceilings;
    }

    internal VmBudgetScope Scope { get; }

    internal ulong Ceiling(VmBudgetDimension dimension) => ceilings[(int)dimension];

    internal ulong Consumed(VmBudgetDimension dimension) => consumed[(int)dimension];

    internal ulong Remaining(VmBudgetDimension dimension)
    {
        var ceiling = ceilings[(int)dimension];
        var used = consumed[(int)dimension];
        return used >= ceiling ? 0 : ceiling - used;
    }

    /// <summary>Whether <paramref name="amount"/> would fit. It does not commit anything.</summary>
    internal bool Admits(VmBudgetDimension dimension, ulong amount) =>
        amount <= Remaining(dimension);

    /// <summary>Commits a charge that <see cref="Admits"/> has already cleared.</summary>
    internal void Commit(VmBudgetDimension dimension, ulong amount) =>
        consumed[(int)dimension] += amount;

    /// <summary>
    /// Gives back part of a live measure. Only a ceiling-class dimension releases: an allowance
    /// never refunds, and a released allowance would be a refund wearing another name.
    /// </summary>
    internal void Release(VmBudgetDimension dimension, ulong amount)
    {
        if (VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Ceiling)
        {
            return;
        }

        var used = consumed[(int)dimension];
        consumed[(int)dimension] = amount >= used ? 0 : used - amount;
    }

    internal VmBudgetSnapshot Snapshot() =>
        new(Scope, (ulong[])consumed.Clone(), (ulong[])ceilings.Clone());

    internal ulong[] CeilingsCopy() => (ulong[])ceilings.Clone();

    internal VmLimitVector AsRemainingVector()
    {
        var remaining = new ulong[VmBudgetDimensions.Count];

        for (var index = 0; index < remaining.Length; index++)
        {
            remaining[index] = Remaining((VmBudgetDimension)index);
        }

        return VmLimitVector.TryCreate(remaining, out var vector) ? vector : default;
    }

    internal VmLimitVector AsCeilingVector() =>
        VmLimitVector.TryCreate(ceilings, out var vector) ? vector : default;
}
