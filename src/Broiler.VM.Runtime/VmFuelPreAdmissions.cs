// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           2
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Medium
// Criteria:         10/0
// Resource impact:  1/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The fuel blocks outstanding against one runtime: which meters hold one, in the order their
/// blocks began, and how large each was when it began.
/// </summary>
/// <remarks>
/// <para>
/// A block is fuel a meter has been admitted in advance, under the runtime gate, at a size no level
/// of its chain would have refused. The meter may then spend it without taking the gate at all, and
/// what it spent is committed to the levels at the next settle. That is the whole mechanism: it
/// moves when a charge is committed, never whether it is admitted.
/// </para>
/// <para>
/// This type is plain state under the runtime gate, exactly like the levels it describes. It takes
/// no lock of its own, and it calls nothing outside <see cref="VmMeter"/> and
/// <see cref="VmBudgetLevel"/>, so a settle can never reach back into a caller that is mid-charge.
/// </para>
/// <para>
/// <b>The size rule is the invariant.</b> Every block is at most what its level has left after
/// every other outstanding block against that level is subtracted whole - not what is left of those
/// blocks, because their unused part can still be spent without the gate. So the sum of the blocks
/// outstanding against a level never exceeds what that level has left, and every charge a block
/// admits is one every level would have admitted.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=7B7CC9
// Broiler-Falsified-If: the blocks outstanding against one level ever sum to more than that level has left
// Broiler-Human:        PENDING
internal sealed class VmFuelPreAdmissions
{
    /// <summary>
    /// How many meters of one runtime may hold a block at once before the oldest is settled.
    /// </summary>
    /// <remarks>
    /// One holder per runtime would be exact and slow: two operations of one runtime may legally
    /// run on two threads, and with one slot each would settle the other on almost every charge,
    /// which costs a settle, the exact body and a pre-admission under a contended lock where today
    /// costs one lock section. Four covers the shapes the core can actually produce at once - two
    /// instances, a caller-driven verification beside them, and a thread a step left running - and
    /// a fifth concurrent meter costs one holder one extra locked charge rather than a table-wide
    /// stampede.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=BE9422
    // Broiler-Falsified-If: a fifth concurrent holder makes every pre-admission settle the whole table rather than one victim
    // Broiler-Human:        PENDING
    internal const int Capacity = 4;

    /// <summary>The largest block, and the one used when a meter declares no poll bound.</summary>
    /// <remarks>
    /// A meter that declares a poll bound is capped at twice that bound instead (see the meter's
    /// cap). This value bounds the other case, and it is what keeps a block small enough that the
    /// fuel a reader has to settle stays a bounded quantity.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=77165A
    // Broiler-Falsified-If: a block larger than this is ever pre-admitted to a meter that declares no poll bound
    // Broiler-Human:        PENDING
    internal const ulong MaxBlock = 1UL << 20;

    private readonly VmMeter?[] holders = new VmMeter?[Capacity];

    private int count;

    /// <summary>Whether no meter of this runtime holds a block.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=8B263A
    // Broiler-Human:        PENDING
    internal bool IsEmpty => count == 0;

    /// <summary>
    /// Settles <paramref name="meter"/>'s block if it holds one, and answers whether it did.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=95AE37
    // Broiler-Falsified-If: a settled meter is left in the table, so its block is counted twice against its levels
    // Broiler-Human:        PENDING
    internal bool Settle(VmMeter meter)
    {
        for (var index = 0; index < count; index++)
        {
            if (ReferenceEquals(holders[index], meter))
            {
                meter.CommitPreAdmittedFuelLocked();
                RemoveAt(index);
                return true;
            }
        }

        return false;
    }

    /// <summary>Settles every holder, and answers how many there were.</summary>
    /// <remarks>
    /// The count is what the charge that caused it shares its next block with. Without it the table
    /// is empty afterwards, the charging meter takes the whole remainder, and the meter it just
    /// settled must settle it again on its next charge - a ping-pong that would repeat for every
    /// charge over the last block's worth of any shared ceiling.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=7B05F7
    // Broiler-Falsified-If: a holder is dropped from the table without what it spent being committed to its levels
    // Broiler-Human:        PENDING
    internal int SettleAll()
    {
        var settled = count;

        for (var index = 0; index < count; index++)
        {
            holders[index]!.CommitPreAdmittedFuelLocked();
            holders[index] = null;
        }

        count = 0;
        return settled;
    }

    /// <summary>
    /// Whether <paramref name="amount"/> fits at every level of <paramref name="meter"/>'s chain
    /// after every other holder's whole block. The meter must hold no block of its own.
    /// </summary>
    /// <remarks>
    /// When it does, every level admits the charge whatever the other holders go on to spend, so
    /// the decision needs no settle. When it does not, the decision is taken on exact state
    /// instead - which scope refuses included - and that is what the caller settles for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=84C7B9
    // Broiler-Falsified-If: a charge is admitted on this answer that a level would refuse once every outstanding block is spent
    // Broiler-Human:        PENDING
    internal bool LeavesRoomFor(VmMeter meter, ulong amount) =>
        amount <= Unreserved(meter.RuntimeLevel) &&
        (meter.InstanceLevel is null || amount <= Unreserved(meter.InstanceLevel)) &&
        amount <= Unreserved(meter.InvocationLevel);

    /// <summary>
    /// Pre-admits a block to <paramref name="meter"/>, which must hold none.
    /// <paramref name="contenders"/> is how many holders this same lock section has just settled
    /// for want of room, and zero on every other call.
    /// </summary>
    /// <remarks>
    /// When the table is full the oldest block is settled to make room, and that happens before the
    /// size is known - so a pre-admission that ends up taking nothing has still sent one holder
    /// back to the gate for its next charge. Exactness does not turn on it either way, because an
    /// early settle is always safe; the cost does, and in the place it is hardest to see, which is
    /// five or more meters charging against a level near its ceiling. It is left in this order
    /// because the size an eviction makes room for cannot be known before the eviction: evicting
    /// raises what the level has unreserved and lowers the divisor, so a size computed with the
    /// victim still holding would refuse blocks this order grants. What it costs is unmeasured.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=26DE61
    // Broiler-Falsified-If: a block is pre-admitted under an aggregate parent, or larger than a level of the meter's chain has left
    // Broiler-Human:        PENDING
    internal void PreAdmit(VmMeter meter, int contenders)
    {
        if (!meter.MayPreAdmit || meter.PreAdmissionSizeLocked != 0)
        {
            return;
        }

        if (count == Capacity)
        {
            // One victim, the oldest block, rather than every holder: settling all four sends all
            // four back to the gate, and with a fifth concurrent meter that would happen on nearly
            // every pre-admission. The oldest is also the likeliest to belong to a finished
            // operation, whose block is doing nothing for anybody. Spent before the size below is
            // known, and so possibly for nothing: see the remark on this member.
            holders[0]!.CommitPreAdmittedFuelLocked();
            RemoveAt(0);
        }

        var size = System.Math.Min(meter.BlockCap, Share(meter.RuntimeLevel, contenders));

        if (meter.InstanceLevel is not null)
        {
            size = System.Math.Min(size, Share(meter.InstanceLevel, contenders));
        }

        size = System.Math.Min(size, Share(meter.InvocationLevel, contenders));

        if (size == 0)
        {
            return;
        }

        holders[count++] = meter;
        meter.BeginPreAdmissionLocked(size);
    }

    /// <summary>
    /// What one more holder of <paramref name="level"/> may take: the unreserved remainder split
    /// evenly between it, the holders already chaining that level, and the contenders just settled.
    /// </summary>
    /// <remarks>
    /// The divisor decides only how a contended level is shared, never whether a charge is
    /// admitted: the answer is at most the unreserved remainder whatever it is, and with one holder
    /// the divisor is one and nothing changes. Without it the meter that reaches the gate last
    /// takes the whole remainder, and two operations under a shared ceiling settle each other on
    /// almost every charge.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=E54B13
    // Broiler-Falsified-If: the share exceeds what the level has left once every outstanding block is subtracted
    // Broiler-Human:        PENDING
    private ulong Share(VmBudgetLevel level, int contenders)
    {
        var sharers = 1UL + (ulong)contenders;

        for (var index = 0; index < count; index++)
        {
            if (holders[index]!.Chains(level))
            {
                sharers++;
            }
        }

        return Unreserved(level) / sharers;
    }

    /// <summary>
    /// Removes one holder, keeping the rest in the order their blocks began.
    /// </summary>
    /// <remarks>
    /// The order is what makes eviction pick the oldest block rather than an arbitrary one, so it
    /// is maintained rather than the last entry being swapped into the hole. At most three moves.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=E5DB19
    // Broiler-Falsified-If: a removal reorders the table, so eviction stops naming the oldest block
    // Broiler-Human:        PENDING
    private void RemoveAt(int index)
    {
        for (var next = index + 1; next < count; next++)
        {
            holders[next - 1] = holders[next];
        }

        holders[--count] = null;
    }

    /// <summary>
    /// What <paramref name="level"/> has left once every outstanding block against it is subtracted
    /// whole, saturating at zero.
    /// </summary>
    /// <remarks>
    /// The whole block, not what is left of it. The part already spent is not yet committed to the
    /// level, and the part not yet spent can still be spent without the gate, so only the block as
    /// it was pre-admitted bounds both at once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=F8B003
    // Broiler-Falsified-If: only the unspent part of a block is subtracted, so fuel already spent is offered twice
    // Broiler-Human:        PENDING
    private ulong Unreserved(VmBudgetLevel level)
    {
        var unreserved = level.Remaining(VmBudgetDimension.Fuel);

        for (var index = 0; index < count; index++)
        {
            if (holders[index]!.Chains(level))
            {
                var block = holders[index]!.PreAdmissionSizeLocked;
                unreserved = block >= unreserved ? 0 : unreserved - block;
            }
        }

        return unreserved;
    }
}
