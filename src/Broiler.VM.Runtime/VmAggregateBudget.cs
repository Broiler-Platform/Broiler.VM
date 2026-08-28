// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           19
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  3/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>The identity of one aggregate budget.</summary>
public readonly struct VmAggregateBudgetId : System.IEquatable<VmAggregateBudgetId>
{
    private readonly VmObjectId value;

    internal VmAggregateBudgetId(VmObjectId value) => this.value = value;

    /// <summary>True when no aggregate budget is identified.</summary>
    public bool IsEmpty => value.IsEmpty;

    /// <summary>The underlying object identity, so a handle can record its parent.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DE3486
    // Broiler-Human: PENDING
    public VmObjectId ObjectId => value;

    /// <inheritdoc/>
    public bool Equals(VmAggregateBudgetId other) => value.Equals(other.value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmAggregateBudgetId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => value.GetHashCode();

    /// <summary>Identity equality.</summary>
    public static bool operator ==(VmAggregateBudgetId left, VmAggregateBudgetId right) => left.Equals(right);

    /// <summary>Identity inequality.</summary>
    // Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C9FF54
    // Broiler-Human: PENDING
    public static bool operator !=(VmAggregateBudgetId left, VmAggregateBudgetId right) => !left.Equals(right);
}

/// <summary>
/// A host-facing reading of one budget scope: what has been consumed, what remains, and what the
/// effective ceiling is.
/// </summary>
/// <remarks>
/// This is the <em>only</em> place remaining and effective values are readable. The profile-facing
/// metering surface has no non-consuming remaining reader, deliberately: a profile that could read
/// its remaining allowance could spend exactly up to it on every operation while staying formally
/// compliant.
/// </remarks>
public readonly struct VmBudgetSnapshot
{
    private readonly ulong[] consumed;
    private readonly ulong[] ceilings;

    internal VmBudgetSnapshot(VmBudgetScope scope, ulong[] consumed, ulong[] ceilings)
    {
        Scope = scope;
        this.consumed = consumed;
        this.ceilings = ceilings;
    }

    /// <summary>Which scope this snapshot reads.</summary>
    public VmBudgetScope Scope { get; }

    /// <summary>How much of <paramref name="dimension"/> has been consumed.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=6D7677
    // Broiler-Human: PENDING
    public ulong Consumed(VmBudgetDimension dimension) =>
        consumed is null ? 0 : consumed[(int)dimension];

    /// <summary>The effective ceiling for <paramref name="dimension"/>.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=26CE4B
    // Broiler-Human: PENDING
    public ulong EffectiveCeiling(VmBudgetDimension dimension) =>
        ceilings is null ? 0 : ceilings[(int)dimension];

    /// <summary>How much of <paramref name="dimension"/> remains.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=C6108A
    // Broiler-Human: PENDING
    public ulong Remaining(VmBudgetDimension dimension)
    {
        var ceiling = EffectiveCeiling(dimension);
        var used = Consumed(dimension);

        return used >= ceiling ? 0 : ceiling - used;
    }
}

/// <summary>
/// A shared parent budget several runtimes are metered against as well as against themselves.
/// </summary>
/// <remarks>
/// <para>
/// A first-class, host-created, host-owned, explicitly disposed core object rather than a host
/// responsibility. Making it the host's job would mean every host that runs more than one runtime
/// re-implements the same summing, and the one that gets it wrong discovers it as a resource
/// exhaustion that never fires.
/// </para>
/// <para>
/// It declares exactly the eleven aggregate dimensions and no others; the four artifact-shaped
/// ceilings do not sum across concurrent runtimes and carry no aggregate scope. Budgets do not
/// nest, so the meter chain is at most operation, runtime, parent.
/// </para>
/// <para>
/// <strong>Pay-as-you-go, not reservation.</strong> Creating a runtime reserves nothing. There is no
/// queue, no fairness policy, no admission ordering, no priority, no preemption, no thread, no timer
/// and no waiting: a budget that could make a caller wait would be a scheduler, and a scheduler in
/// the core is a policy decision the host did not get to make. Allowances never refund, including on
/// dispose; the live-runtime count is a ceiling on a live measure and does decrement.
/// </para>
/// </remarks>
public sealed class VmAggregateBudget : System.IDisposable
{
    private readonly object gate = new();
    private readonly ulong[] ceilings;
    private readonly ulong[] consumed = new ulong[VmBudgetDimensions.Count];
    private int liveRuntimes;
    private bool sealedOff;
    private bool disposed;

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=8CF01A
    // Broiler-Human: PENDING
    private VmAggregateBudget(VmObjectId objectId, ulong[] ceilings)
    {
        Id = new VmAggregateBudgetId(objectId);
        this.ceilings = ceilings;
    }

    /// <summary>
    /// Creates a parent budget from ceiling specifications. Only the eleven aggregate dimensions may
    /// be specified; anything else is refused rather than quietly ignored.
    /// </summary>
    /// <exception cref="System.ArgumentException">A non-aggregate dimension was specified, or one was omitted.</exception>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=3; Fingerprint=D42CF2
    // Broiler-Human: PENDING
    public static VmAggregateBudget Create(
        System.Collections.Immutable.ImmutableArray<VmCeilingSpec> aggregateCeilings)
    {
        var resolved = new ulong[VmBudgetDimensions.Count];
        var seen = new bool[VmBudgetDimensions.Count];

        if (!aggregateCeilings.IsDefault)
        {
            foreach (var spec in aggregateCeilings)
            {
                if (!VmBudgetDimensions.CarriesAggregateScope(spec.Dimension))
                {
                    throw new System.ArgumentException(
                        "Only the eleven dimensions that carry aggregate scope may be declared on an " +
                        "aggregate budget; " + spec.Dimension + " does not.",
                        nameof(aggregateCeilings));
                }

                if (spec.Source is not VmCeilingSource.Explicit)
                {
                    throw new System.ArgumentException(
                        "An aggregate ceiling must be explicit: there is no parent to adopt from and " +
                        "no profile default that applies to a budget shared by several profiles.",
                        nameof(aggregateCeilings));
                }

                resolved[(int)spec.Dimension] = spec.ExplicitValue;
                seen[(int)spec.Dimension] = true;
            }
        }

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (VmBudgetDimensions.CarriesAggregateScope(dimension) && !seen[(int)dimension])
            {
                throw new System.ArgumentException(
                    "Every aggregate dimension must carry an explicit ceiling; " + dimension +
                    " was omitted, and omission never means unbounded.",
                    nameof(aggregateCeilings));
            }
        }

        return new VmAggregateBudget(VmObjectId.Mint(), resolved);
    }

    /// <summary>This budget's identity, recorded in every child runtime's identity.</summary>
    public VmAggregateBudgetId Id { get; }

    /// <summary>
    /// How many runtimes are currently live under this budget.
    /// </summary>
    /// <remarks>
    /// Spelled with the <c>Value</c> suffix because <c>LiveRuntimeCount</c> is a struck name from an
    /// earlier spelling of the dimension set, and a struck name must appear nowhere.
    /// </remarks>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=031FA5
    // Broiler-Human: PENDING
    public int LiveRuntimeCountValue
    {
        get
        {
            lock (gate)
            {
                return liveRuntimes;
            }
        }
    }

    /// <summary>Whether the budget has been sealed against new runtimes.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=E12312
    // Broiler-Human: PENDING
    public bool IsSealed
    {
        get
        {
            lock (gate)
            {
                return sealedOff;
            }
        }
    }

    /// <summary>Reads consumption and remaining allowance across every aggregate dimension.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=A21258
    // Broiler-Human: PENDING
    public VmBudgetSnapshot GetSnapshot()
    {
        lock (gate)
        {
            return new VmBudgetSnapshot(
                VmBudgetScope.Aggregate,
                (ulong[])consumed.Clone(),
                (ulong[])ceilings.Clone());
        }
    }

    /// <summary>
    /// Refuses further runtime creation without waiting for anything. Explicit and non-blocking: a
    /// seal that waited would be the queue this type does not have.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=6EC9F5
    // Broiler-Human: PENDING
    public VmControlResult Seal()
    {
        lock (gate)
        {
            if (disposed)
            {
                return VmControlResult.InvalidState(VmReason.AggregateBudgetDisposed);
            }

            if (sealedOff)
            {
                return VmControlResult.NoOp;
            }

            sealedOff = true;
            return VmControlResult.Accepted;
        }
    }

    /// <summary>
    /// Disposes the budget. Refused while child runtimes are live: disposing a parent out from
    /// under its children would leave them metering against a budget that no longer answers.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=F09672
    // Broiler-Human: PENDING
    public VmControlResult Dispose()
    {
        lock (gate)
        {
            if (disposed)
            {
                return VmControlResult.NoOp;
            }

            if (liveRuntimes > 0)
            {
                return VmControlResult.InvalidState(VmReason.AggregateBudgetHasLiveRuntimes);
            }

            disposed = true;
            return VmControlResult.Accepted;
        }
    }

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Dispose();

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=1F59C1
    // Broiler-Human: PENDING
    internal ulong CeilingFor(VmBudgetDimension dimension)
    {
        lock (gate)
        {
            return ceilings[(int)dimension];
        }
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=A65A69
    // Broiler-Human: PENDING
    internal ulong RemainingFor(VmBudgetDimension dimension)
    {
        lock (gate)
        {
            var ceiling = ceilings[(int)dimension];
            var used = consumed[(int)dimension];
            return used >= ceiling ? 0 : ceiling - used;
        }
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=88A3FA
    // Broiler-Human: PENDING
    internal bool TryAdmitRuntime(out VmReason reason)
    {
        lock (gate)
        {
            if (disposed)
            {
                reason = VmReason.AggregateBudgetDisposed;
                return false;
            }

            if (sealedOff)
            {
                reason = VmReason.ParentExhausted;
                return false;
            }

            if ((ulong)liveRuntimes >= ceilings[(int)VmBudgetDimension.LiveRuntimes])
            {
                reason = VmReason.LiveRuntimeCeilingReached;
                return false;
            }

            // A parent with no remaining allowance admits nothing. Without this the only guard is
            // the per-dimension ceiling comparison at resolution, which passes trivially whenever
            // the host asks for zero or adopts the remainder - so a spent parent would keep handing
            // out runtimes that can do no work.
            if (IsSpent())
            {
                reason = VmReason.ParentExhausted;
                return false;
            }

            liveRuntimes++;
            reason = VmReason.None;
            return true;
        }
    }

    /// <summary>
    /// Whether any allowance-class aggregate dimension has been fully consumed.
    /// </summary>
    /// <remarks>
    /// Only allowances count. A ceiling-class dimension at its limit means the parent is currently
    /// full, not that it is finished, and a live measure can fall again as instances are disposed.
    /// </remarks>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=4C4F14
    // Broiler-Human: PENDING
    internal bool IsSpent()
    {
        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!VmBudgetDimensions.CarriesAggregateScope(dimension) ||
                VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Allowance)
            {
                continue;
            }

            if (consumed[(int)dimension] >= ceilings[(int)dimension])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether the parent still admits work, for the resume admission check.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=A68ABC
    // Broiler-Human: PENDING
    internal bool AdmitsResumption(out VmReason reason)
    {
        lock (gate)
        {
            if (disposed)
            {
                reason = VmReason.AggregateBudgetDisposed;
                return false;
            }

            if (IsSpent())
            {
                reason = VmReason.ParentExhausted;
                return false;
            }

            reason = VmReason.None;
            return true;
        }
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=020E98
    // Broiler-Human: PENDING
    internal void ReleaseRuntime()
    {
        lock (gate)
        {
            if (liveRuntimes > 0)
            {
                liveRuntimes--;
            }
        }
    }

    /// <summary>
    /// Charges the parent, or reports which dimension refused. Allowances accumulate and never
    /// refund; ceilings track a live measure and are released through <see cref="Release"/>.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=8D0EF6
    // Broiler-Human: PENDING
    internal bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.CarriesAggregateScope(dimension))
        {
            return true;
        }

        lock (gate)
        {
            if (disposed)
            {
                return false;
            }

            var ceiling = ceilings[(int)dimension];
            var used = consumed[(int)dimension];

            if (amount > ceiling - used)
            {
                return false;
            }

            consumed[(int)dimension] = used + amount;
            return true;
        }
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=30F5CF
    // Broiler-Human: PENDING
    internal void Release(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.CarriesAggregateScope(dimension) ||
            VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Ceiling)
        {
            // An allowance never refunds. Releasing one would let an operation that finished give
            // back what it spent, which is the monotonicity the budget contract rests on.
            return;
        }

        lock (gate)
        {
            var used = consumed[(int)dimension];
            consumed[(int)dimension] = amount >= used ? 0 : used - amount;
        }
    }
}
