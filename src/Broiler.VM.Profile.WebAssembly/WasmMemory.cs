// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           3
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Critical
// Criteria:         6/6
// Resource impact:  4/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One linear memory: a byte array in pages of 65,536, with the bounds check on every access and
/// the growth rule this profile publishes a deviation for.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE REPRESENTATION IS A PINNED MANAGED ARRAY, AND THE BOUNDS CHECK IS NOT OPTIONAL AND NOT
/// DEFERRED.</b> Decision WAD-0001 pins the array (<see cref="Allocate"/>) so its base is stable
/// between growths. Every load and store goes through the universal bytecode's region primitive over
/// <see cref="Bytes"/>, which computes the effective address in 64-bit arithmetic - so a 32-bit
/// address plus a 32-bit static offset cannot wrap - and compares the whole accessed range against
/// the memory's current size before touching a byte; a data segment goes through
/// <see cref="TryInitialise"/>, which checks its whole range the same way. <i>The alternative not taken</i> is a reserved virtual
/// range with guard pages, which moves the check into the memory management unit and makes every
/// claimed runtime identifier a separate piece of evidence; it is the representation this profile
/// will have to cost when there is a measurement to cost it against, and it is not this one.
/// </para>
/// <para>
/// <b>THE PUBLISHED DEVIATION, AND THE REASON IT EXISTS.</b> The specification says a refused
/// <c>memory.grow</c> answers minus one: the operation completes normally and the module decides
/// what to do. The core's metering surface cannot spell that for a core budget: a refused
/// <c>TryCharge</c> at any scope - the allocation's, or the retention's, which a growth charges before
/// it allocates - latches exhaustion on the meter, after which the core rewrites the step as a resource
/// exhaustion whatever this profile did with the <see langword="false"/> it was handed. <b>So growth is
/// gated first on this profile's OWN declared page maximum</b> - see
/// <see cref="ProfileMaximumPages"/> - which is not a core budget and refuses nothing on the meter.
/// That gate is where the specification's minus-one comes from: the guest observes it, the
/// operation continues, and no allowance was spent because none was asked for.
/// </para>
/// <para>
/// <b>A refusal caused by a CORE budget stays non-guest-observable, and that is the deviation this
/// component's support surface publishes.</b> If a charge of <see cref="Grow"/> is refused, it still
/// answers minus one and says a budget refused it, and the answer does not reach the guest: the
/// family's handler ends the step there, the meter latched, and the core reports the operation as a
/// resource exhaustion naming the dimension and the scope. The module never runs the instruction after
/// the growth. That is a deviation from what the specification says <c>memory.grow</c> answers, it is
/// taken deliberately rather than discovered, and closing it needs a way for a core budget to refuse a
/// charge without latching that nobody has minted.
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

    /// <summary>
    /// Allocates the pinned, zeroed array a memory of <paramref name="byteCount"/> bytes is held in,
    /// which the caller has already charged.
    /// </summary>
    /// <remarks>
    /// <b>DECISION WAD-0001: A LINEAR MEMORY IS A PINNED MANAGED ARRAY.</b> It is allocated on the
    /// pinned object heap, so the collector never moves it while it lives and its base is stable
    /// between growths; a growth allocates a new one and republishes the base. The profile's own page
    /// ceiling keeps the count far below what one array can hold.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=35BEEF
    // Broiler-Falsified-If: a memory's array is allocated anywhere but the pinned object heap, or allocated before its charge
    // Broiler-Human:        PENDING
    internal static byte[] Allocate(ulong byteCount) => System.GC.AllocateArray<byte>((int)byteCount, pinned: true);

    /// <summary>
    /// The memory's current bytes, for a region access that reads the base at the access.
    /// </summary>
    /// <remarks>
    /// The span is a view, and a successful growth invalidates it: a caller takes it for
    /// one access and never holds it across an instruction that can grow the memory.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=0; Fingerprint=CB7CA1
    // Broiler-Falsified-If: a span taken here is held across a growth, or covers bytes outside the current array
    // Broiler-Human:        PENDING
    internal System.Span<byte> Bytes => bytes;

    /// <summary>How many pages the memory currently holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F46F93
    // Broiler-Human:        PENDING
    internal uint PageCount => (uint)(bytes.Length / PageBytes);

    /// <summary>
    /// The effective page ceiling: the tighter of what the module declared and what this profile
    /// declares.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=014764
    // Broiler-Human:        PENDING
    internal uint EffectiveMaximumPages =>
        declaredMaximumPages < ProfileMaximumPages ? declaredMaximumPages : ProfileMaximumPages;

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
    /// Grows the memory by whole pages, answering the old page count or minus one, and says whether a
    /// core budget refused the growth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The order is load-bearing and is the order of the two refusals this profile distinguishes.
    /// First the profile's own page ceiling, which spends nothing and hands the guest a minus one it
    /// can act on. Then the core's charges - the growth's fuel, its allocation and its retention - any
    /// of which if refused latches exhaustion and makes the minus one unreachable by the guest, the
    /// deviation this type's remarks publish. Then, and only then, the allocation.
    /// </para>
    /// <para>
    /// <b>The retention is charged before the allocation, not reported after it.</b> A report returns
    /// nothing, so a growth that reported its retention after allocating would go on past a refusal
    /// nobody heard; charged first - the meter's own statement of how a caller that must observe a
    /// ceiling refusal does it - a refused retention allocates nothing, holds nothing unreported, and is
    /// answered as the core-budget refusal it is. The bare-module executor this profile carried until
    /// milestone UBC-4 reported it after the allocation; the bytes and the dimension retained are the
    /// same.
    /// </para>
    /// <para>
    /// <paramref name="refusedByBudget"/> is true exactly when a charge or a retention the meter refused
    /// is why the answer is minus one, and false for a refusal against the page ceiling, which charged
    /// nothing. A caller that must not run the guest past a latched refusal reads it.
    /// </para>
    /// <para>
    /// A growth of zero pages is not a no-op the caller may skip: the specification defines it as the
    /// way a module reads its own size through this instruction, and it answers the current page
    /// count without allocating.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=C5B8BA
    // Broiler-Falsified-If: an array is allocated before the allocation and retention charges return true, a refusal against the profile ceiling reaches the meter or is answered as refused by a budget, a refused charge is answered as a refusal against the ceiling, or an allocation that throws leaves the added retention reported
    // Broiler-Human:        PENDING
    internal long Grow(uint deltaPages, IVmMeter meter, out bool refusedByBudget)
    {
        refusedByBudget = false;
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
        // Every refusal from here on latched on the meter.
        if (!meter.TryCharge(VmBudgetDimension.Fuel, deltaPages) ||
            !meter.TryCharge(VmBudgetDimension.AllocatedBytes, addedBytes) ||
            !meter.TryCharge(VmBudgetDimension.LiveBytes, addedBytes))
        {
            refusedByBudget = true;
            return GrowthRefused;
        }

        // A new pinned array, the contents copied, and the base republished: decision WAD-0001's growth.
        // Every view of the old array is stale from here, which is why nothing holds one across an
        // instruction.
        byte[] grown;

        try
        {
            grown = Allocate((ulong)current * PageBytes + addedBytes);
        }
        catch (System.Exception)
        {
            // The memory keeps its old array, and its release gives back only that array's length, so
            // the retention charged for the added pages is given back here.
            meter.ReportReleased(VmBudgetDimension.LiveBytes, addedBytes);
            throw;
        }

        System.Array.Copy(bytes, grown, bytes.Length);
        bytes = grown;

        // The live bytes charged above are retained rather than consumed: they live for as long as the
        // instance does, which is what makes live bytes a ceiling and allocated bytes an allowance.
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
