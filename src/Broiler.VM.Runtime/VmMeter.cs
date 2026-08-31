// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           18
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Medium
// Criteria:         10/0
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2575F4
// Broiler-Falsified-If: no level in a chain carries the Artifact scope, so an artifact-scoped dimension is reported at another
// Broiler-Human:        PENDING
internal sealed class VmMeter : IVmMeter, IVmBoundedAllocationMeter
{
    /// <summary>
    /// Which reason names a breach of <paramref name="dimension"/>: an allowance is spent, a
    /// ceiling is reached, and which of the two a dimension is, is fixed by its class rather than
    /// chosen at the call site.
    /// </summary>
    /// <remarks>
    /// Every site that reports exhaustion naming a dimension goes through here, because the
    /// alternative was tried and failed in both directions at once: the verification path
    /// hardcoded a ceiling reason and was right only for the one dimension it named, the
    /// invocation, resume and instantiation paths hardcoded an allowance reason and were wrong
    /// whenever the meter failed on a ceiling, and the guest-load mediator hardcoded a ceiling
    /// reason while two of its three dimensions are allowances.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0005; IP=Low; Security=Low; Resources=0; Fingerprint=43869B
    // Broiler-Falsified-If: a dimension's class and its reported reason disagree at any exhaustion site
    // Broiler-Human:        PENDING
    internal static VmReason ReasonFor(VmBudgetDimension dimension) =>
        VmBudgetDimensions.ClassOf(dimension) is VmBudgetClass.Ceiling
            ? VmReason.CeilingReached
            : VmReason.AllowanceExhausted;

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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A6A2A6
    // Broiler-Human:        PENDING
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

    /// <summary>
    /// Whether <paramref name="exception"/> is this operation's own cancellation rather than some
    /// other token's, which is what ADR 0011's X1 asks and what separates a host propagating our
    /// cancellation from a host throwing a cancellation that has nothing to do with us.
    /// </summary>
    /// <remarks>
    /// Both halves are load-bearing. A foreign token is a fault, because nothing about this
    /// operation was cancelled. And an exception carrying our token while the token is not actually
    /// cancellation-requested is also a fault, because reporting it as cancellation would name an
    /// event that did not happen - and would be dropped anyway, since the stage's own cancellation
    /// test reads the token rather than this flag.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=77EE01
    // Broiler-Falsified-If: a cancellation carrying a foreign token is reported as this operation's cancellation
    // Broiler-Human:        PENDING
    internal bool IsOperationCancellation(System.OperationCanceledException exception) =>
        cancellation.IsCancellationRequested && exception.CancellationToken == cancellation;

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
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=9422B7
    // Broiler-Human:        PENDING
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

    /// <summary>
    /// Whether a charge against <paramref name="dimension"/> counts toward the uncharged-work bound.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two dimensions do, and they are the two denominated in work units: the profile's own
    /// <c>Fuel</c> and the verifier's <c>VerifierWork</c>. The bound is on <em>work performed between
    /// two polls</em>, and the other thirteen dimensions count bytes, milliseconds, calls, depths and
    /// live objects. Adding a byte count to a work counter says one allocated byte is one unit of
    /// work, which is not a conversion anything in the contract defines.
    /// </para>
    /// <para>
    /// It is not a cosmetic distinction. Summing every dimension made one correctly metered,
    /// in-bounds allocation of half a megabyte breach a poll bound of a thousand instantly - and the
    /// poll-bound path reports a profile fault and poisons the runtime, so a core unit conflation
    /// was billed to the profile as a broken metering contract. The corpus entry that reads a
    /// constant pool at exactly the declared-count ceiling is what found it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=2336EA
    // Broiler-Falsified-If: a dimension counting bytes, time or objects reaches the uncharged-work counter
    // Broiler-Human:        PENDING
    private static bool IsWork(VmBudgetDimension dimension) =>
        dimension is VmBudgetDimension.Fuel or VmBudgetDimension.VerifierWork;

    /// <summary>The invocation level's remaining allowance, for a nested load's request snapshot.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=E3117F
    // Broiler-Falsified-If: a ceiling-class dimension is handed on as ceiling minus consumed, not as its effective ceiling
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=7A8087
    // Broiler-Falsified-If: one level commits while another refuses, or a refusal names Invocation where an outer level would
    // Broiler-Human:        PENDING
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

            if (IsWork(dimension))
            {
                sinceLastPoll += amount;
            }

            return true;
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=86CFE4
    // Broiler-Human:        PENDING
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
    /// The member returns <c>void</c>, and the reason is the frozen shape of the surface rather than
    /// anything about remaining values - <c>TryCharge</c> hands a refusal back and reads no
    /// remaining value either, so that cannot be what distinguishes them. ADR 0003's
    /// candidate-amendment register states the fact directly, in the row that would change it: the
    /// retention report returns nothing and the refusal is latched for the next charge or poll. ADR
    /// 0007 supplies the observation point - live operations fail at their next charge or poll - and
    /// freezes the surface at four members with <c>TryCharge</c> the only one given a return.
    /// </para>
    /// <para>
    /// The consequence a profile needs, and the reason this is worth stating where it is enforced:
    /// <strong>a ceiling-class dimension cannot carry a guest-observable refusal.</strong> A language
    /// construct that must observe a refusal and continue - a guest asking to grow a region and
    /// deciding what to do when told no - gates on <c>TryCharge</c>. Reporting the retention and
    /// hoping to hear about it is a refusal the guest observes one operation too late, by which time
    /// it has already seen the growth succeed.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0003 s12 row 9, ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=68B388
    // Broiler-Falsified-If: a level commits a retention on a path where the parent refused that same retention
    // Broiler-Human:        PENDING
    public void ReportRetained(VmBudgetDimension dimension, ulong amount)
    {
        if (amount == 0)
        {
            return;
        }

        // Admission is tested at every level before anything is committed at any of them. The test
        // is separate from the commit because this member cannot refuse its caller - it returns
        // nothing - so the answer is latched and observed at the next charge or poll. Committing a
        // breach anyway is what made a runtime, instance or invocation ceiling report nothing at
        // all: only the parent was ever asked, Poll looks at wall clock alone, and a profile that
        // retained past its ceiling and then completed was never told.
        VmBudgetScope? refusedLocally = null;

        lock (gate)
        {
            if (!runtime.Admits(dimension, amount))
            {
                refusedLocally = VmBudgetScope.Runtime;
            }
            else if (instance is not null && !instance.Admits(dimension, amount))
            {
                refusedLocally = VmBudgetScope.Instance;
            }
            else if (!invocation.Admits(dimension, amount))
            {
                refusedLocally = VmBudgetScope.Invocation;
            }
        }

        // Outermost scope first, as everywhere else, so the parent is asked even when a local level
        // has already refused - and what it accepts is handed straight back when one has, because a
        // parent debited for a retention no level committed could never be released: the release
        // path credits the parent only what the invocation level actually holds.
        if (parent is not null && VmBudgetDimensions.CarriesAggregateScope(dimension))
        {
            if (!parent.TryCharge(dimension, amount))
            {
                Refuse(dimension, VmBudgetScope.Aggregate);
                return;
            }

            if (refusedLocally is not null)
            {
                parent.Release(dimension, amount);
            }
        }

        if (refusedLocally is not null)
        {
            Refuse(dimension, refusedLocally.Value);
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=8030AD
    // Broiler-Falsified-If: the parent is credited more than it accepted, or an allowance-class dimension refunds at any level
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=5C88D9
    // Broiler-Human:        PENDING
    bool IVmBoundedAllocationMeter.TryReserve(ulong byteCount) =>
        TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=939F9E
    // Broiler-Human:        PENDING
    void IVmBoundedAllocationMeter.Release(ulong byteCount) =>
        ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=31B54B
    // Broiler-Human:        PENDING
    bool IVmBoundedAllocationMeter.TryChargeWork(ulong workUnits) =>
        TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    bool IVmBoundedAllocationMeter.Poll() => Poll();

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=536CA8
    // Broiler-Human:        PENDING
    internal VmBudgetSnapshot Snapshot()
    {
        lock (gate)
        {
            return invocation.Snapshot();
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7C7769
    // Broiler-Human:        PENDING
    internal void ReplaceCancellation(System.Threading.CancellationToken token) => cancellation = token;

    /// <summary>
    /// Stops the wall clock for the duration of a suspension.
    /// </summary>
    /// <remarks>
    /// The clock pauses under every suspension origin, with no host override. A parked sibling that
    /// kept accruing would drain a shared parent while doing no work at all, which is the opposite
    /// of what a wall-clock allowance is for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=076A86
    // Broiler-Falsified-If: the clock accrues across a parked interval, so time nobody spent is billed to the guest
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=602915
    // Broiler-Human:        PENDING
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=956ADA
    // Broiler-Falsified-If: a delta the parent refuses is dropped rather than re-offered, or one delta is attributed twice
    // Broiler-Human:        PENDING
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=335330
    // Broiler-Human:        PENDING
    private bool Refuse(VmBudgetDimension dimension, VmBudgetScope scope)
    {
        FailedDimension = dimension;
        FailedScope = scope;
        ExhaustionObserved = true;
        return false;
    }
}
