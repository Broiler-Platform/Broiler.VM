// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           0
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    High
// Resource impact:  8/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The allocation guard. Every allocation whose size derives from untrusted data goes through it,
/// and it refuses <em>before</em> allocating rather than after.
/// </summary>
/// <remarks>
/// <para>
/// Roadmap section 7 states the rule as "no allocation based on an untrusted declared count before
/// the count passes its configured bound". The order here is the whole point: check the count
/// against its bound, compute the byte size with checked arithmetic, reserve it against the meter,
/// and only then allocate. A guard that allocated first and asked afterwards would already have
/// spent the memory an attacker was aiming at.
/// </para>
/// <para>
/// <see cref="VmReadBounds"/> and <see cref="IVmBoundedAllocationMeter"/> are required parameters
/// for the same reason they are required on <see cref="VmBoundedReader"/>: there must be no
/// overload that allocates without a policy.
/// </para>
/// <para>
/// The <c>unmanaged</c> constraint keeps the size computation exact and keeps the generic
/// instantiation set closed over value types, which is what lets a Native AOT closure root it
/// without a rooting descriptor. The size itself comes from
/// <c>System.Runtime.CompilerServices.Unsafe.SizeOf</c> rather than from an <c>unsafe</c> block,
/// so this assembly needs no <c>AllowUnsafeBlocks</c>.
/// </para>
/// </remarks>
public static class VmBoundedAllocator
{
    /// <summary>
    /// Allocates an array of <paramref name="declaredCount"/> elements, refusing if the count
    /// exceeds its bound or the meter will not reserve the bytes.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=630EF7
    // Broiler-Human: PENDING
    public static bool TryAllocate<T>(
        in VmReadBounds bounds,
        IVmBoundedAllocationMeter meter,
        uint declaredCount,
        out T[] buffer)
        where T : unmanaged
    {
        buffer = System.Array.Empty<T>();

        if (meter is null)
        {
            throw new System.ArgumentNullException(nameof(meter));
        }

        if (declaredCount > bounds.MaxDeclaredCount)
        {
            return false;
        }

        ulong byteCount;

        try
        {
            byteCount = checked(declaredCount * (ulong)System.Runtime.CompilerServices.Unsafe.SizeOf<T>());
        }
        catch (System.OverflowException)
        {
            // A count that clears its own bound can still overflow once multiplied by an element
            // size. The multiplication is therefore checked, and its overflow is a refusal rather
            // than an exception the caller has to be told about separately.
            return false;
        }

        return TryAllocateExact(in bounds, meter, byteCount, out buffer);
    }

    /// <summary>
    /// Allocates a buffer of exactly <paramref name="byteCount"/> bytes, reserving them against
    /// the meter first.
    /// </summary>
    /// <remarks>
    /// The byte count is checked against the artifact bound as well as against the meter. An
    /// artifact can never require more resident bytes than it is itself allowed to be, and
    /// enforcing that here means a profile cannot reach the meter with a number the read bounds
    /// already exclude.
    /// </remarks>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=8; Fingerprint=5185F6
    // Broiler-Human: PENDING
    public static bool TryAllocateExact<T>(
        in VmReadBounds bounds,
        IVmBoundedAllocationMeter meter,
        ulong byteCount,
        out T[] buffer)
        where T : unmanaged
    {
        buffer = System.Array.Empty<T>();

        if (meter is null)
        {
            throw new System.ArgumentNullException(nameof(meter));
        }

        if (byteCount == 0)
        {
            return true;
        }

        if (byteCount > bounds.MaxArtifactBytes)
        {
            return false;
        }

        var elementSize = (ulong)System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

        if (byteCount % elementSize != 0)
        {
            return false;
        }

        var elements = byteCount / elementSize;

        if (elements > int.MaxValue)
        {
            return false;
        }

        if (!meter.TryReserve(byteCount))
        {
            return false;
        }

        try
        {
            buffer = new T[(int)elements];
        }
        catch (System.OutOfMemoryException)
        {
            // The reservation succeeded and the allocation did not, so the reservation is given
            // back. Leaking it would make a host that survived one hostile artifact refuse the
            // next legitimate one.
            meter.Release(byteCount);
            throw;
        }

        return true;
    }
}
