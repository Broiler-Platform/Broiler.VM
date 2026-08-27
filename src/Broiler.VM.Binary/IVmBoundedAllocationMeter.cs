namespace Broiler.VM;

/// <summary>
/// The callback through which a bounded read charges the allocation and verifier-work allowances
/// it consumes.
/// </summary>
/// <remarks>
/// <para>
/// This assembly deliberately does not know which budget dimension a reservation charges.
/// <see cref="TryReserve"/> is the contract's allocated-bytes dimension and
/// <see cref="TryChargeWork"/> is its verifier-work dimension, but the mapping lives on the
/// implementing side, in the assembly that owns the vocabulary. Naming the dimensions here would
/// be exactly the contract vocabulary ADR 0001 keeps out of this assembly.
/// </para>
/// <para>
/// Every method returns rather than throws. A refusal is an ordinary, expected answer on the
/// malformed-input path, and raising an exception there would make the cost of rejecting hostile
/// input depend on how hostile it was.
/// </para>
/// </remarks>
public interface IVmBoundedAllocationMeter
{
    /// <summary>
    /// Reserves <paramref name="byteCount"/> bytes against the allocation allowance. Returns
    /// <see langword="false"/> when the allowance cannot cover it, in which case the caller must
    /// not allocate.
    /// </summary>
    bool TryReserve(ulong byteCount);

    /// <summary>
    /// Returns a previously reserved allocation. It cannot fail and it cannot raise an allowance:
    /// only what <see cref="TryReserve"/> took may come back.
    /// </summary>
    void Release(ulong byteCount);

    /// <summary>
    /// Charges <paramref name="workUnits"/> against the verifier-work allowance. Returns
    /// <see langword="false"/> when the allowance is spent.
    /// </summary>
    bool TryChargeWork(ulong workUnits);

    /// <summary>
    /// One combined budget and cancellation check. <see langword="false"/> means stop; it does not
    /// say which of the two applies, because a reader has the same obligation either way.
    /// </summary>
    bool Poll();
}
