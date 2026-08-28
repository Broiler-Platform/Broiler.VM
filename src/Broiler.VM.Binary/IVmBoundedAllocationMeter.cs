// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

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
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A95709
// Broiler-Falsified-If: a member that can refuse has no way to say so in its return value, so refusal must be thrown
// Broiler-Human:        PENDING
public interface IVmBoundedAllocationMeter
{
    /// <summary>
    /// Reserves <paramref name="byteCount"/> bytes against the allocation allowance. Returns
    /// <see langword="false"/> when the allowance cannot cover it, in which case the caller must
    /// not allocate.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=612753
    // Broiler-Falsified-If: a false return has already charged the allowance, or a true one reserves nothing
    // Broiler-Human:        PENDING
    bool TryReserve(ulong byteCount);

    /// <summary>
    /// Returns a previously reserved allocation. It cannot fail and it cannot raise an allowance:
    /// only what <see cref="TryReserve"/> took may come back.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F50C5
    // Broiler-Human:        PENDING
    void Release(ulong byteCount);

    /// <summary>
    /// Charges <paramref name="workUnits"/> against the verifier-work allowance. Returns
    /// <see langword="false"/> when the allowance is spent.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E14B16
    // Broiler-Falsified-If: false is returned here for a cancellation, which the caller latches as a spent work allowance
    // Broiler-Human:        PENDING
    bool TryChargeWork(ulong workUnits);

    /// <summary>
    /// One combined budget and cancellation check. <see langword="false"/> means stop; it does not
    /// say which of the two applies, because a reader has the same obligation either way.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=84FB1F
    // Broiler-Human:        PENDING
    bool Poll();
}
