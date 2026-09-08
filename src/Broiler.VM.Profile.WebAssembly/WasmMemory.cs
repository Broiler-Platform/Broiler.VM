// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           2
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Critical
// Criteria:         6/6
// Resource impact:  4/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One linear memory: a byte array in pages of 65,536, with the bounds check on every access and
/// the growth rule this profile publishes a deviation for.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE REPRESENTATION IS A MANAGED ARRAY, AND THE BOUNDS CHECK IS NOT OPTIONAL AND NOT
/// DEFERRED.</b> Every load and store goes through <see cref="TryLoad"/> or <see cref="TryStore"/>,
/// each of which computes the effective address in 64-bit arithmetic - so a 32-bit address plus a
/// 32-bit static offset cannot wrap - and compares the whole accessed range against the memory's
/// current size before touching a byte. <i>The alternative not taken</i> is a reserved virtual
/// range with guard pages, which moves the check into the memory management unit and makes every
/// claimed runtime identifier a separate piece of evidence; it is the representation this profile
/// will have to cost when there is a measurement to cost it against, and it is not this one.
/// </para>
/// <para>
/// <b>THE PUBLISHED DEVIATION, AND THE REASON IT EXISTS.</b> The specification says a refused
/// <c>memory.grow</c> answers minus one: the operation completes normally and the module decides
/// what to do. The core's metering surface cannot spell that. A retention report returns nothing,
/// so a ceiling refusal cannot be handed back at the point of retention; and a refused
/// <c>TryCharge</c> at any scope latches exhaustion on the meter, after which the core rewrites the
/// completed step as a resource exhaustion whatever this profile did with the <see langword="false"/>
/// it was handed. <b>So growth is gated first on this profile's OWN declared page maximum</b> - see
/// <see cref="ProfileMaximumPages"/> - which is not a core budget and refuses nothing on the meter.
/// That gate is where the specification's minus-one comes from: the guest observes it, the
/// operation continues, and no allowance was spent because none was asked for.
/// </para>
/// <para>
/// <b>A refusal caused by a CORE budget stays non-guest-observable, and that is the deviation this
/// component's support surface publishes.</b> If the allocation charge below is refused, this
/// method still answers minus one, and the answer does not reach the guest: the meter latched, and
/// the core reports the operation as a resource exhaustion naming the dimension and the scope. The
/// module never runs the instruction after the growth. That is a deviation from what the
/// specification says <c>memory.grow</c> answers, it is taken deliberately rather than discovered,
/// and closing it needs a refusable retention member on the core's metering surface that nobody has
/// minted.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=985077
// Broiler-Falsified-If: an access reaches a byte outside the current size, or a growth refused by the profile maximum spends a core allowance, or a growth allocates before its charge is taken
// Broiler-Human:        PENDING
internal sealed class WasmMemoryInstance
{
    /// <summary>How many bytes one page is, which the format fixes and nothing configures.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=330AAB
    // Broiler-Human:        PENDING
    internal const int PageBytes = 65_536;

    /// <summary>
    /// The largest number of pages this profile will let one memory reach, whatever the module
    /// declared and whatever the host granted.
    /// </summary>
    /// <remarks>
    /// <b>IT IS THIS PROFILE'S OWN NUMBER AND NOT A CORE BUDGET, WHICH IS THE WHOLE POINT OF IT.</b>
    /// Refusing here spends nothing on the meter and latches nothing, so the specification's
    /// minus-one answer is producible and the operation continues. Sixty-four mebibytes is the same
    /// number the descriptor's live-bytes default carries, so a memory that reaches this bound has
    /// reached the retained-bytes default too and a host that raised one without the other gains
    /// nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F9C3B1
    // Broiler-Falsified-If: this bound is derived from a limit vector rather than declared here, or refusing against it charges anything
    // Broiler-Human:        PENDING
    internal const uint ProfileMaximumPages = 1_024;

    /// <summary>What a refused growth answers, as the specification spells it.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=0602DC
    // Broiler-Human:        PENDING
    internal const long GrowthRefused = -1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6CDF62
    // Broiler-Human:        PENDING
    private readonly uint declaredMaximumPages;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=07A782
    // Broiler-Human:        PENDING
    private byte[] bytes;

    /// <summary>Allocates a memory at its declared minimum size, zeroed.</summary>
    /// <remarks>
    /// The array is allocated by the caller and handed in, because the caller is the one holding the
    /// meter and the allocation must be charged before it exists. This constructor takes what it is
    /// given and computes nothing about the budget.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3F4248
    // Broiler-Human:        PENDING
    internal WasmMemoryInstance(byte[] initial, uint maximumPages)
    {
        bytes = initial;
        declaredMaximumPages = maximumPages;
    }

    /// <summary>How many pages the memory currently holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F46F93
    // Broiler-Human:        PENDING
    internal uint PageCount => (uint)(bytes.Length / PageBytes);

    /// <summary>How many bytes the memory currently holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5B4E82
    // Broiler-Human:        PENDING
    internal int ByteCount => bytes.Length;

    /// <summary>
    /// The effective page ceiling: the tighter of what the module declared and what this profile
    /// declares.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=014764
    // Broiler-Human:        PENDING
    internal uint EffectiveMaximumPages =>
        declaredMaximumPages < ProfileMaximumPages ? declaredMaximumPages : ProfileMaximumPages;

    /// <summary>
    /// Reads <paramref name="width"/> bytes little-endian at an address, or refuses.
    /// </summary>
    /// <remarks>
    /// The address arrives already summed in 64 bits from a 32-bit dynamic address and a 32-bit
    /// static offset, so the sum cannot have wrapped before it got here; the comparison below is
    /// therefore the only thing standing between a guest and the rest of the heap.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=8389FD
    // Broiler-Falsified-If: it reads a byte at or past the current size, or the range check is performed in 32-bit arithmetic
    // Broiler-Human:        PENDING
    internal bool TryLoad(ulong address, int width, out ulong bits)
    {
        bits = 0;

        if (address + (ulong)width > (ulong)bytes.Length)
        {
            return false;
        }

        var at = (int)address;

        for (var index = 0; index < width; index++)
        {
            bits |= (ulong)bytes[at + index] << (index * 8);
        }

        return true;
    }

    /// <summary>
    /// Writes the low <paramref name="width"/> bytes of a value little-endian at an address, or
    /// refuses.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=A3FB78
    // Broiler-Falsified-If: it writes a byte at or past the current size, or the range check is performed in 32-bit arithmetic
    // Broiler-Human:        PENDING
    internal bool TryStore(ulong address, int width, ulong bits)
    {
        if (address + (ulong)width > (ulong)bytes.Length)
        {
            return false;
        }

        var at = (int)address;

        for (var index = 0; index < width; index++)
        {
            bytes[at + index] = (byte)(bits >> (index * 8));
        }

        return true;
    }

    /// <summary>
    /// Copies a data segment's bytes in, after checking the whole segment fits.
    /// </summary>
    /// <remarks>
    /// <b>THE CHECK COVERS THE WHOLE SEGMENT BEFORE ONE BYTE IS WRITTEN, WHICH IS WHAT SEGMENT
    /// ATOMICITY MEANS.</b> A segment that does not fit writes nothing at all - not a prefix, not a
    /// page. What this member deliberately does not do is undo an earlier segment: the specification
    /// makes initialisation atomic per segment and explicitly not across segments, and a whole-module
    /// rollback would pass every hand-written test and contradict the suite.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=FA5507
    // Broiler-Falsified-If: a prefix of a segment that does not fit is written, or a segment that does fit is refused
    // Broiler-Human:        PENDING
    internal bool TryInitialise(ulong address, System.ReadOnlySpan<byte> contents)
    {
        if (address + (ulong)contents.Length > (ulong)bytes.Length)
        {
            return false;
        }

        contents.CopyTo(System.MemoryExtensions.AsSpan(bytes, (int)address));
        return true;
    }

    /// <summary>
    /// Grows the memory by whole pages, answering the old page count or minus one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The order is load-bearing and is the order of the two refusals this profile distinguishes.
    /// First the profile's own page ceiling, which spends nothing and hands the guest a minus one it
    /// can act on. Then the core's allocation charge, which if refused latches exhaustion and makes
    /// the minus one unreachable by the guest - the deviation this type's remarks publish. Then, and
    /// only then, the allocation.
    /// </para>
    /// <para>
    /// A growth of zero pages is not a no-op the caller may skip: the specification defines it as the
    /// way a module reads its own size through this instruction, and it answers the current page
    /// count without allocating.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=859A66
    // Broiler-Falsified-If: an array is allocated before the allocation charge returns true, or a refusal against the profile ceiling reaches the meter
    // Broiler-Human:        PENDING
    internal long Grow(uint deltaPages, IVmMeter meter)
    {
        var current = PageCount;

        if (deltaPages == 0)
        {
            return current;
        }

        var wanted = (ulong)current + deltaPages;

        // THIS REFUSAL IS THE GUEST-OBSERVABLE ONE. Nothing has been charged, nothing has been
        // retained, and nothing on the meter has latched, so the operation continues and the module
        // reads the minus one the specification promises it.
        if (wanted > EffectiveMaximumPages)
        {
            return GrowthRefused;
        }

        var addedBytes = (ulong)deltaPages * PageBytes;

        // Proportional rather than flat: the cost of a growth is the cost of zeroing what it added,
        // and a flat charge would let a guest buy sixty-four mebibytes for the price of one page.
        if (!meter.TryCharge(VmBudgetDimension.Fuel, deltaPages))
        {
            return GrowthRefused;
        }

        if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, addedBytes))
        {
            return GrowthRefused;
        }

        var grown = new byte[(int)((ulong)current * PageBytes + addedBytes)];
        System.Array.Copy(bytes, grown, bytes.Length);
        bytes = grown;

        // Retained rather than consumed: the bytes live for as long as the store does, which is what
        // makes live bytes a ceiling and allocated bytes an allowance.
        meter.ReportRetained(VmBudgetDimension.LiveBytes, addedBytes);
        return current;
    }

    /// <summary>Reports the memory's bytes released, and drops them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4F2E86
    // Broiler-Human:        PENDING
    internal void Release(IVmMeter meter)
    {
        var held = (ulong)bytes.Length;
        bytes = [];
        meter.ReportReleased(VmBudgetDimension.LiveBytes, held);
    }
}
