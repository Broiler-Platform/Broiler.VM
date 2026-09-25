// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           3
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The charge-and-poll pacing an instantiation's segment charges go through.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE POLL GOES BEFORE THE CHARGE THAT WOULD CROSS THE BOUND, NOT AFTER A FIXED COUNT.</b> The
/// core measures uncharged work as fuel charged since the last poll and reports a profile fault when
/// it exceeds the declared bound, so a caller that polled every so many charges and then made a
/// proportional one would breach the bound with one charge. <see cref="TryCharge"/> buys the headroom
/// for a charge by polling first whenever the charge would cross the bound.
/// </para>
/// <para>
/// <b>Its one caller is the family's instance state.</b> It was the bare-module interpreter's pacing
/// until milestone UBC-4 retired that interpreter; under the universal bytecode an emitter's loop
/// paces the guest's own fuel, and what is left to pace here is what the making of an instance charges
/// before any guest code runs - the element and data segments, each charged in pieces no larger than
/// the bound.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A58F73
// Broiler-Falsified-If: fuel charged between two polls can exceed the declared uncharged-work bound
// Broiler-Human:        PENDING
internal sealed class WasmPacing
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ACABAB
    // Broiler-Human:        PENDING
    private readonly ulong bound;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2AB034
    // Broiler-Human:        PENDING
    private ulong sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B68EFE
    // Broiler-Human:        PENDING
    internal WasmPacing(IVmMeter contractMeter, ulong unchargedWorkBound)
    {
        meter = contractMeter;
        bound = unchargedWorkBound;
    }

    /// <summary>Charges fuel, polling first if this charge would cross the bound.</summary>
    /// <remarks>
    /// A refused poll is a cancellation or a wall-clock ceiling and a refused charge an exhaustion; the
    /// meter latched whichever it was, and the core's own precedence decides which the caller of the
    /// operation is told about, so a refusal answers false and says no more.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4F6409
    // Broiler-Falsified-If: a charge is committed without the poll that its size demanded
    // Broiler-Human:        PENDING
    internal bool TryCharge(ulong amount)
    {
        if (sinceLastPoll + amount > bound)
        {
            if (!meter.Poll())
            {
                return false;
            }

            sinceLastPoll = 0;
        }

        if (!meter.TryCharge(VmBudgetDimension.Fuel, amount))
        {
            return false;
        }

        sinceLastPoll += amount;
        return true;
    }

    /// <summary>Records fuel another party charged, so the poll bound stays honest.</summary>
    /// <remarks>
    /// The executor charges an instantiation's one fuel unit before it asks the family for the
    /// instance, and that charge counts toward the bound as much as any made here. This is how it gets
    /// into the pacing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6ABDC7
    // Broiler-Falsified-If: a charge made elsewhere never reaches this counter
    // Broiler-Human:        PENDING
    internal void Observe(ulong amount) => sinceLastPoll += amount;
}
