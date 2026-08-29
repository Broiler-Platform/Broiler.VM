namespace Broiler.VM.Fixtures;

/// <summary>
/// Records, for one verification, the order in which the effective policy was handed over, the
/// first payload byte was consumed, and the first allocation was reserved.
/// </summary>
/// <remarks>
/// <para>
/// The precedence algorithm requires materialization to complete before the first read of any
/// payload byte and before any allocation sized by payload data, and calls that ordering
/// mechanically assertable. This is the mechanism: a monotonic sequence, stamped at three points
/// that the profile cannot reorder.
/// </para>
/// <para>
/// The stamps are taken where the events actually happen rather than where they are convenient.
/// The policy stamp is taken on entry to the verifier, because the verifier's parameter list is the
/// first moment the frozen policy is observable at all. The read stamp is taken in
/// <see cref="TryChargeWork"/>, which the bounded reader calls from <c>TryConsume</c> and therefore
/// on every byte it consumes and on no other path. The allocation stamp is taken in
/// <see cref="TryReserve"/>, which only the bounded allocator calls.
/// </para>
/// <para>
/// A recorder belongs to one descriptor and therefore to one composition. It carries no static
/// state, so two suites running side by side record their own verifications and not each other's.
/// </para>
/// </remarks>
public sealed class FixtureReadOrderRecorder
{
    private readonly object gate = new();
    private long sequence;
    private long policyStamp;
    private long firstReadStamp;
    private long firstReserveStamp;
    private ulong reservedBytes;
    private VmLimitVector policy;

    /// <summary>Forgets everything recorded, so one recorder can serve a sweep of artifacts.</summary>
    public void Reset()
    {
        lock (gate)
        {
            sequence = 0;
            policyStamp = 0;
            firstReadStamp = 0;
            firstReserveStamp = 0;
            reservedBytes = 0;
            policy = default;
        }
    }

    /// <summary>Whether the verifier was entered at all.</summary>
    public bool PolicyObserved
    {
        get
        {
            lock (gate)
            {
                return policyStamp != 0;
            }
        }
    }

    /// <summary>The verification ceilings the verifier was handed, as it was handed them.</summary>
    public VmLimitVector ObservedPolicy
    {
        get
        {
            lock (gate)
            {
                return policy;
            }
        }
    }

    /// <summary>Whether any payload byte was consumed.</summary>
    public bool AnyRead
    {
        get
        {
            lock (gate)
            {
                return firstReadStamp != 0;
            }
        }
    }

    /// <summary>Whether any allocation was reserved.</summary>
    public bool AnyReservation
    {
        get
        {
            lock (gate)
            {
                return firstReserveStamp != 0;
            }
        }
    }

    /// <summary>How many bytes were reserved in total.</summary>
    public ulong ReservedBytes
    {
        get
        {
            lock (gate)
            {
                return reservedBytes;
            }
        }
    }

    /// <summary>
    /// True when the policy was stamped, and was stamped strictly before both the first byte read
    /// and the first reservation. It is vacuously true of neither having happened, which is the
    /// correct answer for an artifact refused before the verifier was entered.
    /// </summary>
    public bool PolicyPrecededEveryRead
    {
        get
        {
            lock (gate)
            {
                if (policyStamp == 0)
                {
                    return firstReadStamp == 0 && firstReserveStamp == 0;
                }

                return (firstReadStamp == 0 || policyStamp < firstReadStamp) &&
                    (firstReserveStamp == 0 || policyStamp < firstReserveStamp);
            }
        }
    }

    /// <summary>
    /// True when no allocation was reserved before the first payload byte was consumed.
    /// </summary>
    /// <remarks>
    /// This is the second half of the ordering claim and the one an implementation can actually get
    /// wrong: a verifier that sized a buffer from a declared count it had not yet read would reserve
    /// first, and a verifier that sized one from a count it read but had not bounded would reserve
    /// an amount the artifact chose. The first is caught here; the second is caught by the reserved
    /// total staying under the allocation ceiling.
    /// </remarks>
    public bool NoReservationPrecededTheFirstRead
    {
        get
        {
            lock (gate)
            {
                return firstReserveStamp == 0 || (firstReadStamp != 0 && firstReadStamp < firstReserveStamp);
            }
        }
    }

    internal void StampPolicy(VmLimitVector verificationCeilings)
    {
        lock (gate)
        {
            if (policyStamp != 0)
            {
                return;
            }

            policy = verificationCeilings;
            policyStamp = ++sequence;
        }
    }

    internal void StampRead()
    {
        lock (gate)
        {
            if (firstReadStamp == 0)
            {
                firstReadStamp = ++sequence;
            }
        }
    }

    internal void StampReserve(ulong byteCount)
    {
        lock (gate)
        {
            reservedBytes += byteCount;

            if (firstReserveStamp == 0)
            {
                firstReserveStamp = ++sequence;
            }
        }
    }
}

/// <summary>
/// The bounded-reading meter the fixture verifier uses when a recorder is attached: the ordinary
/// adapter with a stamp on each of the two methods whose order is the claim.
/// </summary>
/// <remarks>
/// It wraps rather than replaces, so what is recorded is the behaviour of the real path. A recorder
/// that answered the bounds itself would be measuring the recorder.
/// </remarks>
public sealed class FixtureRecordingReadAdapter : IVmBoundedAllocationMeter
{
    private readonly FixtureBoundedReadAdapter inner;
    private readonly FixtureReadOrderRecorder recorder;

    /// <summary>Wraps <paramref name="meter"/> and stamps into <paramref name="orderRecorder"/>.</summary>
    public FixtureRecordingReadAdapter(IVmMeter meter, FixtureReadOrderRecorder orderRecorder)
    {
        inner = new FixtureBoundedReadAdapter(meter);
        recorder = orderRecorder;
    }

    /// <inheritdoc/>
    public bool TryReserve(ulong byteCount)
    {
        // Stamped before the inner call, so a reservation the meter refuses is still recorded as
        // having been attempted. A guard that refused after allocating would otherwise look
        // identical to one that refused before.
        recorder.StampReserve(byteCount);
        return inner.TryReserve(byteCount);
    }

    /// <inheritdoc/>
    public void Release(ulong byteCount) => inner.Release(byteCount);

    /// <inheritdoc/>
    public bool TryChargeWork(ulong workUnits)
    {
        recorder.StampRead();
        return inner.TryChargeWork(workUnits);
    }

    /// <inheritdoc/>
    public bool Poll() => inner.Poll();
}
