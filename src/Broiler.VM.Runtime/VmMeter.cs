// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           20
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         20/0
// Resource impact:  1/10 max
// Unverified:       31
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

    // Fuel this meter may still admit without taking the gate. Raised only under the gate and only
    // from zero; lowered lock-free by the fast path; taken to zero by a settle. Never negative.
    private long preAdmittedFuel;

    // The block as it was pre-admitted. Read and written only under the gate. Zero exactly when
    // this meter is not in its runtime's pre-admission table.
    private ulong preAdmissionSize;

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
    /// <remarks>
    /// The counter is read here, so this meter's block is settled first: work admitted from a block
    /// reaches the counter at the settle, and a breach decided before that would be decided on a
    /// count of work the profile did rather than a count of work it was charged for. This settle
    /// alone also covers a thread the step left running, which can charge after the step's own
    /// settle and before this read.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=346ABD
    // Broiler-Falsified-If: work charged since the last poll is missing from the count a breach is decided on
    // Broiler-Human:        PENDING
    internal bool UnpolledWorkExceedsBound
    {
        get
        {
            lock (gate)
            {
                runtime.FuelPreAdmissions!.Settle(this);
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
    /// <remarks>
    /// This reads a remaining value, so it settles first. An invocation level belongs to exactly one
    /// meter - a fresh one is built for every invocation, instantiation and verification, and the
    /// nested path reuses the requesting meter rather than its level - so settling this meter alone
    /// covers every holder whose fuel this read could otherwise miss.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=57709C
    // Broiler-Falsified-If: a ceiling-class dimension is handed on as ceiling minus consumed, not as its effective ceiling, or a remainder is handed to a nested verification while this meter's own block is uncommitted
    // Broiler-Human:        PENDING
    internal VmLimitVector RemainingSnapshot
    {
        get
        {
            lock (gate)
            {
                runtime.FuelPreAdmissions!.Settle(this);
                return invocation.AsRemainingVector();
            }
        }
    }

    // ---- pre-admitted fuel ---------------------------------------------------------------------

    /// <summary>The block this meter holds, as it was pre-admitted. Read under the gate.</summary>
    /// <remarks>
    /// Zero exactly when this meter is not in its runtime's table, which is what lets the table be
    /// asked whether a meter holds a block without searching it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=7DB20C
    // Broiler-Falsified-If: this reads non-zero for a meter the table does not hold, or zero for one it does
    // Broiler-Human:        PENDING
    internal ulong PreAdmissionSizeLocked => preAdmissionSize;

    /// <summary>
    /// Whether this meter may hold a block at all.
    /// </summary>
    /// <remarks>
    /// Never under an aggregate parent. A parent is shared with sibling runtimes, and its own
    /// admission, its snapshot and whether it is spent all read a sum this runtime would otherwise
    /// be holding fuel back from. Under a parent every charge takes the exact locked path, which is
    /// what it did before any of this existed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=032026
    // Broiler-Falsified-If: a meter whose runtime has an aggregate parent is allowed to hold a block
    // Broiler-Human:        PENDING
    internal bool MayPreAdmit => parent is null;

    /// <summary>The runtime level this meter chains, which owns the table.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=EEBF50
    // Broiler-Human:        PENDING
    internal VmBudgetLevel RuntimeLevel => runtime;

    /// <summary>The instance level this meter chains, if it has one.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=754D53
    // Broiler-Human:        PENDING
    internal VmBudgetLevel? InstanceLevel => instance;

    /// <summary>The invocation level this meter chains.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=E4AA44
    // Broiler-Human:        PENDING
    internal VmBudgetLevel InvocationLevel => invocation;

    /// <summary>The largest block this meter may hold.</summary>
    /// <remarks>
    /// Twice the declared poll bound, because a compliant profile charges at most its bound between
    /// two polls and a poll re-admits, so anything larger buys nothing - and the factor two is what
    /// lets a block that began at a charge rather than at a poll still reach the next poll. A
    /// profile that declares no bound gets the flat maximum. Written to avoid overflowing the
    /// doubling.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=35AA33
    // Broiler-Falsified-If: a declared poll bound at the top of its range doubles past the maximum instead of saturating at it
    // Broiler-Human:        PENDING
    internal ulong BlockCap =>
        pollBound == 0 || pollBound >= VmFuelPreAdmissions.MaxBlock / 2
            ? VmFuelPreAdmissions.MaxBlock
            : 2 * pollBound;

    /// <summary>Whether <paramref name="level"/> is one of the three this meter charges.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=10791D
    // Broiler-Falsified-If: a level this meter charges is not recognised, so its block is left out of that level's sum
    // Broiler-Human:        PENDING
    internal bool Chains(VmBudgetLevel level) =>
        ReferenceEquals(level, runtime) ||
        ReferenceEquals(level, instance) ||
        ReferenceEquals(level, invocation);

    /// <summary>Takes a block of <paramref name="size"/> units. Called under the gate only.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=A548C5
    // Broiler-Falsified-If: a block is begun over one this meter still holds, so what was spent from the old one is lost
    // Broiler-Human:        PENDING
    internal void BeginPreAdmissionLocked(ulong size)
    {
        preAdmissionSize = size;
        System.Threading.Volatile.Write(ref preAdmittedFuel, (long)size);
    }

    /// <summary>
    /// Commits what this meter spent from its block to every level it chains, and to the
    /// uncharged-work counter. Called under the gate only.
    /// </summary>
    /// <remarks>
    /// The block is zeroed FIRST. A fast-path charge racing this either landed in what is still
    /// there, in which case it is part of what is committed here, or fails its compare-exchange and
    /// takes the locked path, where it waits for this lock section to end - so no charge is lost
    /// and none is counted twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=3F7F04
    // Broiler-Falsified-If: fuel spent from a block reaches a level twice, or reaches none of them, or bypasses the uncharged-work counter
    // Broiler-Human:        PENDING
    internal void CommitPreAdmittedFuelLocked()
    {
        var left = (ulong)System.Threading.Interlocked.Exchange(ref preAdmittedFuel, 0);
        var used = preAdmissionSize - left;

        preAdmissionSize = 0;

        if (used == 0)
        {
            return;
        }

        runtime.Commit(VmBudgetDimension.Fuel, used);
        instance?.Commit(VmBudgetDimension.Fuel, used);
        invocation.Commit(VmBudgetDimension.Fuel, used);

        // Fuel is work, so it counts toward the bound exactly as a per-charge commit would have.
        sinceLastPoll += used;
    }

    /// <summary>
    /// Settles this meter's block, taking the gate to do it. Step-end hygiene.
    /// </summary>
    /// <remarks>
    /// A step that has ended holds a block nobody will spend, and its fuel is uncommitted until
    /// something settles it. It states no falsifiable claim of its own, and that is deliberate: the
    /// operation's completion reads the uncharged-work counter, and that reader settles this same
    /// meter itself, so on every path that reaches the read either settle alone makes it exact and
    /// removing this one changes nothing an observation point can see. The reader's settle is also
    /// the only one that covers a thread the step left running, which can charge between here and
    /// the read. What this settle does on its own is free the table slot at once and drop the
    /// reference to a finished operation's meter, rather than leaving both until some other meter's
    /// charge evicts it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=6575FD
    // Broiler-Human:        PENDING
    internal void SettlePreAdmittedFuel()
    {
        lock (gate)
        {
            runtime.FuelPreAdmissions!.Settle(this);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The head is the whole of the fast path. A fuel charge that fits inside the block this meter
    /// has already been admitted is taken by one compare-exchange and no lock at all, because every
    /// level cleared that block's whole size when it was pre-admitted. Everything else - every
    /// other dimension, an undefined one, and every fuel charge that does not fit - falls through
    /// to the exact body, which is where a refusal is decided and the only place one can be.
    /// </para>
    /// <para>
    /// The dimension test is load-bearing rather than an optimisation. Without it a charge on
    /// another dimension would be admitted against fuel and committed nowhere, so a host-call or
    /// call-depth ceiling would simply stop being enforced.
    /// </para>
    /// <para>
    /// The amount test is not load-bearing, and is there for what it costs rather than for what it
    /// answers. A charge of no units is admitted either way - the exact body answers it before it
    /// takes the gate - but without the test it satisfies the block test on a meter that holds no
    /// block at all, because zero fits in zero, and every such charge then issues a lock-prefixed
    /// write to a field other threads are reading. Two register compares instead, on the member
    /// every charge in the component passes through.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=E1B1AC
    // Broiler-Falsified-If: a charge on a dimension other than Fuel is answered from a fuel block, or a fuel charge larger than the block is answered without the exact body
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        if (dimension == VmBudgetDimension.Fuel && amount != 0)
        {
            var left = System.Threading.Volatile.Read(ref preAdmittedFuel);

            while (amount <= (ulong)left)
            {
                var seen = System.Threading.Interlocked.CompareExchange(
                    ref preAdmittedFuel, left - (long)amount, left);

                if (seen == left)
                {
                    return true;
                }

                left = seen;
            }
        }

        return TryChargeLocked(dimension, amount);
    }

    /// <summary>
    /// The exact charge: today's body, with a settle before any decision it could affect and a
    /// pre-admission after an admitted fuel charge.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A fuel charge settles its own block first, so what it spent is committed before anything is
    /// decided. It then asks whether the OTHER holders' blocks still leave room for it: if they do,
    /// every level admits it whatever those holders go on to spend, and the body runs against state
    /// that is exact for the purposes of this decision. If they do not, every holder is settled and
    /// the body runs against state that is exact outright - so a refusal, and which scope it names,
    /// is never decided against fuel somebody has already spent.
    /// </para>
    /// <para>
    /// Not inlined, deliberately. It is the slow path, and inlining it into the head would put a
    /// lock section and the whole level chain into every caller of a charge that is meant to cost a
    /// compare-exchange.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=DC8233
    // Broiler-Falsified-If: one level commits while another refuses, or a refusal names Invocation where an outer level would, or a wall-clock ceiling reached during a run of charges is never reported at the next poll, or a refusal is decided while another holder's spending is uncommitted
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private bool TryChargeLocked(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.IsDefined(dimension))
        {
            return false;
        }

        if (amount == 0)
        {
            return true;
        }

        // Wall clock is deliberately NOT accrued here. It accrues in Poll, and the contract already
        // bounds how long a profile may go between polls, so a wall-clock ceiling reached in the
        // middle of a run of charges is still reported - at the next poll, within the latency the
        // profile declared. Accruing on every charge bought nothing that guarantee did not already
        // give, and cost a second lock acquisition and a clock read on the hottest path in the
        // component: a verifier reading one byte at a time charges once per byte.
        lock (gate)
        {
            // Only a fuel charge reads the state a block leaves uncommitted. Every other dimension
            // is committed per charge at its own levels and never touches a block.
            var preAdmissions = dimension is VmBudgetDimension.Fuel ? runtime.FuelPreAdmissions : null;
            var contenders = 0;

            if (preAdmissions is not null && !preAdmissions.IsEmpty)
            {
                preAdmissions.Settle(this);

                if (!preAdmissions.IsEmpty && !preAdmissions.LeavesRoomFor(this, amount))
                {
                    contenders = preAdmissions.SettleAll();
                }
            }

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

            // The charge is admitted and committed, so this meter is charging fuel and is worth a
            // block. The size is decided here and nowhere else, against the state this section is
            // about to leave behind.
            preAdmissions?.PreAdmit(this, contenders);

            return true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// A poll settles this meter's block before it reads the uncharged-work counter, and takes a
    /// fresh one on the way out if it held one when it arrived. That pairing is what makes a block
    /// worth having for a profile that polls on a window: the block is spent between two polls and
    /// renewed at each, so the bound is decided on every unit charged and the charges themselves
    /// stay off the lock.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=8621A4
    // Broiler-Falsified-If: a poll decides the bound before the fuel admitted from a block has reached the counter
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
            var held = runtime.FuelPreAdmissions!.Settle(this);

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

            // Only a meter that arrived holding a block takes another. A poll is not by itself
            // evidence that anything is charging fuel: a profile polls on paths that charge
            // nothing, and a block taken there would sit unspent against a level its holder is not
            // using while some other operation needs it.
            if (held)
            {
                runtime.FuelPreAdmissions.PreAdmit(this, 0);
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
    // Broiler-AI:           Origin=AI; Spec=ADR-0003 s12 row 9, ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=E6F911
    // Broiler-Falsified-If: a level commits a retention on a path where the parent refused that same retention, or a retention is admitted or committed while any holder's fuel is uncommitted
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
            // Every holder, not just this meter: a retention is admitted against the runtime and
            // instance levels, which every operation of this runtime shares, so fuel any of them
            // has spent and not yet committed is fuel this admission must already see.
            if (dimension is VmBudgetDimension.Fuel)
            {
                runtime.FuelPreAdmissions!.SettleAll();
            }

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
            // The two sections are not one, here or before any of this existed: a fuel charge can
            // land between them. Settling again is what keeps the blocks outstanding against a
            // level within what that level has left across the gap this commit opens.
            if (dimension is VmBudgetDimension.Fuel)
            {
                runtime.FuelPreAdmissions!.SettleAll();
            }

            runtime.Commit(dimension, amount);
            instance?.Commit(dimension, amount);
            invocation.Commit(dimension, amount);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The parent is credited exactly what it was debited, which the ordering in
    /// <see cref="ReportRetained"/> guarantees: a retention the parent refused was never committed
    /// locally, so it can never be released from the parent either.
    /// </para>
    /// <para>
    /// This is the one fuel read in this type that deliberately does NOT settle first, and it is
    /// safe for two reasons that have to hold together. The consumption read only chooses between
    /// an early return and a release, and a release is a no-op at every level for any dimension
    /// that is not ceiling-class, so fuel a block leaves uncommitted cannot change what this member
    /// does. And a meter that holds a block has no aggregate parent, so the credit handed on below
    /// is never computed from a short count either. Make fuel releasable at the invocation level,
    /// or let a meter under a parent hold a block, and this member needs the settle the others
    /// take.
    /// </para>
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

    /// <remarks>
    /// The invocation level belongs to this meter alone, so settling this meter is the whole of
    /// what a consumption read here needs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=2CA9E5
    // Broiler-Falsified-If: the snapshot reports consumption that stops short of what this operation has been admitted
    // Broiler-Human:        PENDING
    internal VmBudgetSnapshot Snapshot()
    {
        lock (gate)
        {
            runtime.FuelPreAdmissions!.Settle(this);
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
