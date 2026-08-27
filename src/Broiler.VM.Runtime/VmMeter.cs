namespace Broiler.VM;

/// <summary>
/// The meter one operation charges through: the invocation, instance, runtime and aggregate levels
/// seen as one surface.
/// </summary>
/// <remarks>
/// <para>
/// It implements both the profile-facing metering interface and the bounded-reading assembly's
/// allocation meter. The second is what makes the runtime genuinely consume a type from
/// <c>Broiler.VM.Binary</c>: a runtime that only ever touched the contracts assembly would satisfy
/// the old subset form of the graph rule and fail the equality form VM-1 tightens it to.
/// </para>
/// <para>
/// <strong>Tie-break.</strong> When several levels would refuse one charge, the outermost is
/// reported: aggregate, then runtime, then artifact, then instance, then invocation. A host reading
/// a result therefore learns which ceiling actually stopped it, not which level happened to notice
/// first. That is why the levels are checked before any of them is committed.
/// </para>
/// </remarks>
internal sealed class VmMeter : IVmMeter, IVmBoundedAllocationMeter
{
    private readonly object gate;
    private readonly VmBudgetLevel invocation;
    private readonly VmBudgetLevel? instance;
    private readonly VmBudgetLevel runtime;
    private readonly VmAggregateBudget? parent;
    private readonly System.Diagnostics.Stopwatch clock;
    private readonly ulong pollBound;

    private System.Threading.CancellationToken cancellation;
    private ulong sinceLastPoll;
    private ulong pausedTicks;
    private long pauseStartedAt = -1;

    internal VmMeter(
        object gate,
        VmBudgetLevel invocation,
        VmBudgetLevel? instance,
        VmBudgetLevel runtime,
        VmAggregateBudget? parent,
        ulong pollBound,
        System.Threading.CancellationToken cancellation)
    {
        this.gate = gate;
        this.invocation = invocation;
        this.instance = instance;
        this.runtime = runtime;
        this.parent = parent;
        this.pollBound = pollBound;
        this.cancellation = cancellation;
        clock = System.Diagnostics.Stopwatch.StartNew();
    }

    /// <summary>The dimension that refused, meaningful once a charge or poll has failed.</summary>
    internal VmBudgetDimension FailedDimension { get; private set; }

    /// <summary>The scope that refused.</summary>
    internal VmBudgetScope FailedScope { get; private set; }

    /// <summary>Whether the last refusal was a cancellation rather than an exhaustion.</summary>
    internal bool CancellationObserved { get; private set; }

    /// <summary>Whether any charge or poll has been refused for want of allowance.</summary>
    internal bool ExhaustionObserved { get; private set; }

    /// <summary>
    /// Records a refusal detected outside the meter - a nested-load bound the mediator enforces
    /// itself - so the requesting operation reports it even if the profile ignores the result.
    /// </summary>
    internal void LatchNestedRefusal(VmBudgetDimension dimension, VmBudgetScope scope) =>
        Refuse(dimension, scope);

    /// <summary>
    /// Whether the profile exceeded its declared uncharged-work bound between two polls. That is a
    /// profile contract violation, not a resource exhaustion: the profile promised a cancellation
    /// latency and did not keep it.
    /// </summary>
    internal bool PollBoundExceeded { get; private set; }

    /// <summary>
    /// Whether work has accumulated past the bound since the last poll - including the case of a
    /// profile that never polled at all, which is the limiting case of breaking the bound rather
    /// than an exemption from it.
    /// </summary>
    internal bool UnpolledWorkExceedsBound
    {
        get
        {
            lock (gate)
            {
                return pollBound > 0 && sinceLastPoll > pollBound;
            }
        }
    }

    /// <summary>The invocation level's remaining allowance, for a nested load's request snapshot.</summary>
    internal VmLimitVector RemainingSnapshot
    {
        get
        {
            lock (gate)
            {
                return invocation.AsRemainingVector();
            }
        }
    }

    /// <inheritdoc/>
    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.IsDefined(dimension))
        {
            return false;
        }

        if (amount == 0)
        {
            return true;
        }

        AccrueWallClock();

        lock (gate)
        {
            // Outermost first, and deliberately without committing: a level that would refuse must
            // be discoverable before a nearer level has already taken the charge.
            if (parent is not null &&
                VmBudgetDimensions.CarriesAggregateScope(dimension) &&
                amount > parent.RemainingFor(dimension))
            {
                return Refuse(dimension, VmBudgetScope.Aggregate);
            }

            if (!runtime.Admits(dimension, amount))
            {
                return Refuse(dimension, VmBudgetScope.Runtime);
            }

            if (instance is not null && !instance.Admits(dimension, amount))
            {
                return Refuse(dimension, VmBudgetScope.Instance);
            }

            if (!invocation.Admits(dimension, amount))
            {
                return Refuse(dimension, VmBudgetScope.Invocation);
            }

            if (parent is not null &&
                VmBudgetDimensions.CarriesAggregateScope(dimension) &&
                !parent.TryCharge(dimension, amount))
            {
                // Another runtime under the same parent took the remainder between the read above
                // and this commit. Reporting the aggregate is still the truthful answer.
                return Refuse(dimension, VmBudgetScope.Aggregate);
            }

            runtime.Commit(dimension, amount);
            instance?.Commit(dimension, amount);
            invocation.Commit(dimension, amount);

            sinceLastPoll += amount;
            return true;
        }
    }

    /// <inheritdoc/>
    public bool Poll()
    {
        AccrueWallClock();

        if (cancellation.IsCancellationRequested)
        {
            CancellationObserved = true;
            return false;
        }

        lock (gate)
        {
            // The bound is on work performed between two polls. Exceeding it is how a profile
            // silently makes cancellation latency unbounded, so it is detected rather than trusted.
            if (pollBound > 0 && sinceLastPoll > pollBound)
            {
                PollBoundExceeded = true;
                return false;
            }

            sinceLastPoll = 0;

            // Wall clock accrues on its own rather than being charged by the profile, so its
            // exhaustion has to be looked for here: charging zero would never find it. Outermost
            // scope first, as everywhere else.
            if (parent is not null && parent.RemainingFor(VmBudgetDimension.WallClock) == 0)
            {
                return Refuse(VmBudgetDimension.WallClock, VmBudgetScope.Aggregate);
            }

            if (runtime.Remaining(VmBudgetDimension.WallClock) == 0)
            {
                return Refuse(VmBudgetDimension.WallClock, VmBudgetScope.Runtime);
            }

            if (instance is not null && instance.Remaining(VmBudgetDimension.WallClock) == 0)
            {
                return Refuse(VmBudgetDimension.WallClock, VmBudgetScope.Instance);
            }

            if (invocation.Remaining(VmBudgetDimension.WallClock) == 0)
            {
                return Refuse(VmBudgetDimension.WallClock, VmBudgetScope.Invocation);
            }
        }

        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The parent is charged <strong>first</strong>, and the local levels commit only what the
    /// parent accepted. Committing locally and then offering the parent a charge it may refuse
    /// makes the pair asymmetric: the retention would later be released from the parent in full,
    /// driving the parent's live sum below the true sum across its children and eventually to
    /// zero, at which point it would admit a retention it should refuse.
    /// </para>
    /// <para>
    /// The member returns <c>void</c> because the frozen metering surface has exactly four members
    /// and none of them reports a remaining value, so a refusal cannot be handed back here. It is
    /// latched instead, and the operation's next charge or poll reports the exhaustion at aggregate
    /// scope.
    /// </para>
    /// </remarks>
    public void ReportRetained(VmBudgetDimension dimension, ulong amount)
    {
        if (amount == 0)
        {
            return;
        }

        if (parent is not null &&
            VmBudgetDimensions.CarriesAggregateScope(dimension) &&
            !parent.TryCharge(dimension, amount))
        {
            Refuse(dimension, VmBudgetScope.Aggregate);
            return;
        }

        lock (gate)
        {
            runtime.Commit(dimension, amount);
            instance?.Commit(dimension, amount);
            invocation.Commit(dimension, amount);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The parent is credited exactly what it was debited, which the ordering in
    /// <see cref="ReportRetained"/> guarantees: a retention the parent refused was never committed
    /// locally, so it can never be released from the parent either.
    /// </remarks>
    public void ReportReleased(VmBudgetDimension dimension, ulong amount)
    {
        if (amount == 0)
        {
            return;
        }

        ulong releasable;

        lock (gate)
        {
            // Never release more than this level actually holds. A profile that over-reports a
            // release would otherwise credit the parent for bytes it was never debited.
            releasable = System.Math.Min(amount, invocation.Consumed(dimension));

            if (releasable == 0)
            {
                return;
            }

            runtime.Release(dimension, releasable);
            instance?.Release(dimension, releasable);
            invocation.Release(dimension, releasable);
        }

        parent?.Release(dimension, releasable);
    }

    /// <inheritdoc/>
    bool IVmBoundedAllocationMeter.TryReserve(ulong byteCount) =>
        TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    void IVmBoundedAllocationMeter.Release(ulong byteCount) =>
        ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    bool IVmBoundedAllocationMeter.TryChargeWork(ulong workUnits) =>
        TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    bool IVmBoundedAllocationMeter.Poll() => Poll();

    internal VmBudgetSnapshot Snapshot()
    {
        lock (gate)
        {
            return invocation.Snapshot();
        }
    }

    internal void ReplaceCancellation(System.Threading.CancellationToken token) => cancellation = token;

    /// <summary>
    /// Stops the wall clock for the duration of a suspension.
    /// </summary>
    /// <remarks>
    /// The clock pauses under every suspension origin, with no host override. A parked sibling that
    /// kept accruing would drain a shared parent while doing no work at all, which is the opposite
    /// of what a wall-clock allowance is for.
    /// </remarks>
    internal void PauseWallClock()
    {
        lock (gate)
        {
            if (pauseStartedAt < 0)
            {
                pauseStartedAt = clock.ElapsedMilliseconds;
            }
        }
    }

    internal void ResumeWallClock()
    {
        lock (gate)
        {
            if (pauseStartedAt >= 0)
            {
                pausedTicks += (ulong)(clock.ElapsedMilliseconds - pauseStartedAt);
                pauseStartedAt = -1;
            }
        }
    }

    /// <summary>
    /// Attributes elapsed time since the last accrual to every level.
    /// </summary>
    /// <remarks>
    /// The parent is charged before the local levels commit, for the same reason retention is: a
    /// delta the parent refuses must not be recorded locally, or the parent permanently under-sums
    /// attributed time across its children and its wall-clock ceiling stops meaning anything. A
    /// refusal is latched so the next poll reports it at aggregate scope rather than the parent
    /// silently stalling below its own ceiling.
    /// </remarks>
    private void AccrueWallClock()
    {
        ulong elapsed;

        lock (gate)
        {
            if (pauseStartedAt >= 0)
            {
                return;
            }

            var total = (ulong)clock.ElapsedMilliseconds;
            var attributed = total > pausedTicks ? total - pausedTicks : 0;
            var already = invocation.Consumed(VmBudgetDimension.WallClock);

            if (attributed <= already)
            {
                return;
            }

            elapsed = attributed - already;
        }

        if (parent is not null && !parent.TryCharge(VmBudgetDimension.WallClock, elapsed))
        {
            // Not committed locally either, so the delta is re-offered on the next accrual rather
            // than lost. The latch is what turns the refusal into a reported outcome.
            Refuse(VmBudgetDimension.WallClock, VmBudgetScope.Aggregate);
            return;
        }

        lock (gate)
        {
            runtime.Commit(VmBudgetDimension.WallClock, elapsed);
            instance?.Commit(VmBudgetDimension.WallClock, elapsed);
            invocation.Commit(VmBudgetDimension.WallClock, elapsed);
        }
    }

    private bool Refuse(VmBudgetDimension dimension, VmBudgetScope scope)
    {
        FailedDimension = dimension;
        FailedScope = scope;
        ExhaustionObserved = true;
        return false;
    }
}
