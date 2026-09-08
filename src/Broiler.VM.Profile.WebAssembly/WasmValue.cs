// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           4
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One runtime value slot, and the place the nine-row value, store and frame decision is written
/// down.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE DECISION IS RECORDED HERE BECAUSE IT WAS TAKEN BEFORE THE INTERPRETER'S FIRST LINE, AND A
/// DECISION NOBODY WROTE DOWN IS AN INVISIBLE BRANCH.</b> The roadmap's section 9 makes this a gate
/// on entry to the interpreter rather than the interpreter's first task, and what follows is each
/// row with the answer taken and the alternative that was not. None of the nine is a review
/// decision and none of them advances a milestone; they are routes, and each may be reversed - the
/// cost of a reversal is that whatever was built on it is unbuilt.
/// </para>
/// <para>
/// <b>Row 1, numeric representation: a single untyped 64-bit payload in a sixteen-byte slot.</b>
/// Validation already proved every operand's type, so nothing at run time needs to ask what a slot
/// holds; the instruction that reads it knows. So the slot carries bits and no tag.
/// <i>The alternative not taken</i> is an eight-byte tagged form that carries the type beside the
/// bits and checks it on every pop. It would catch an interpreter defect at the cost of a branch
/// per operand, and the check it performs is the one validation already performed once for the life
/// of the module.
/// </para>
/// <para>
/// <b>Row 2, vector representation: the slot is sixteen bytes now, and the high half is unused.</b>
/// This is the row whose late answer invalidates the other eight, which is why it is answered here
/// and not when the vector manifest opens. A 128-bit value fits the slot the day that manifest is
/// minted, and no operand stack, no local array, no global array and no interpreter signature
/// changes to admit it. <i>The alternative not taken</i> is an eight-byte slot with a separate
/// side-stack for 128-bit values, which halves the memory every stack and every locals array costs
/// today and pays for it with a second stack, a second height and a second truncation rule at every
/// branch. It is defensible, it is what a profile that never grew vectors should have chosen, and
/// it is recorded here rather than dismissed.
/// </para>
/// <para>
/// <b>Row 3, reference representation: there is none, because no reference type is admitted.</b>
/// The accepted manifest has no <c>funcref</c> or <c>externref</c> value, so no slot here holds a
/// reference and no parallel object array exists. A table's entries are function indices held as
/// integers in the store, which is not a reference value - it never reaches an operand stack.
/// <i>The alternative not taken</i> is a parallel <c>object?[]</c> beside the numeric stack, which
/// is what this profile will need the moment reference types open, and which would cost an
/// allocation and a write barrier per frame today for nothing.
/// </para>
/// <para>
/// <b>Row 4, rooting and lifetime: nothing needs rooting, for the same reason.</b> Every value in
/// this build is bits. The store's arrays are ordinary managed arrays reachable from the instance,
/// and a host holds no view onto any of them.
/// </para>
/// <para>
/// <b>Row 5, call convention: arguments are popped from the caller's operand stack directly into
/// the callee's locals array, in declaration order, and results are pushed back onto the caller's
/// stack.</b> A frame's locals array is its own; the operand stack is shared and each frame records
/// the height it began at. <i>The alternative not taken</i> is to leave arguments in place on one
/// stack and address them through a frame pointer, which saves the copy and makes the locals of a
/// captured frame a window into a stack that has since moved. There is no tail call in this build,
/// so the frame-reuse obligation is not expressible yet and is not pretended to be.
/// </para>
/// <para>
/// <b>Row 6, frames and labels: frames are heap-allocated objects and labels are a per-frame array
/// of three numbers and a flag.</b> This is the row section 14 forces: a frame model living on the
/// CLR stack cannot later be moved to the heap without rewriting the interpreter, so it is on the
/// heap from the first line even though nothing captures a frame today. The consequence is worth
/// stating in the direction that costs: guest call depth does not grow the CLR stack at all, so a
/// deep guest cannot overflow it, and the only thing bounding call depth is the meter.
/// <i>The alternative not taken</i> is a recursive interpreter whose CLR frame is the guest frame,
/// which is shorter, faster, uncapturable, and terminates the process on a deep enough guest.
/// </para>
/// <para>
/// <b>Row 7, trap propagation: a trap is a return code threaded through the dispatch loop and never
/// a CLR exception.</b> <see cref="WasmTrapKind"/> is the closed list and the interpreter returns
/// it beside a run status; every arithmetic and memory helper answers with it rather than throwing.
/// <i>The alternative not taken</i> is a CLR exception caught at the executor boundary, which is
/// less code, unwinds the heap frame stack for free, and makes the cost of a trap a property of the
/// runtime's exception machinery rather than of this interpreter - and which would put a CLR type
/// on the path between a guest instruction and a payload.
/// </para>
/// <para>
/// <b>Row 8, metering: one unit of <c>Fuel</c> per instruction dispatched, a proportional charge
/// beside it for the two operations whose cost the guest chooses, one unit of <c>CallDepth</c> per
/// activation released on return, <c>AllocatedBytes</c> and a <c>LiveBytes</c> retention report for
/// every memory and table byte the store holds, and one <c>Poll</c> per the declared uncharged-work
/// bound.</b> The poll is placed before the charge that would cross the bound rather than after a
/// fixed instruction count, so the bound cannot be crossed by a single proportional charge.
/// </para>
/// <para>
/// <b>Row 9, what a <c>LiveBytes</c> breach does: nothing this profile decides.</b> Retention is
/// reported and the report returns nothing, so a ceiling reached by a retention is observed at the
/// operation's next charge or poll and the core rewrites the step. This build therefore does not
/// terminate an operation on a retention breach of its own accord, and a guest observes an aborted
/// operation rather than a value it can act on. That is the same gap
/// <see cref="WasmMemoryInstance"/> records for memory growth, and it is not separable from it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EFC50D
// Broiler-Falsified-If: a slot here carries a type tag, or an interpreter path reads the payload as a type validation did not prove it to be
// Broiler-Human:        PENDING
internal readonly struct WasmValue
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=76EF03
    // Broiler-Human:        PENDING
    private readonly ulong low;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E9DF77
    // Broiler-Human:        PENDING
    private readonly ulong high;

    /// <summary>Builds a slot from its two halves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E93888
    // Broiler-Human:        PENDING
    private WasmValue(ulong lowBits, ulong highBits)
    {
        low = lowBits;
        high = highBits;
    }

    /// <summary>The low sixty-four bits, which is where every value this build admits sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=682D69
    // Broiler-Human:        PENDING
    internal ulong Low => low;

    /// <summary>
    /// The high sixty-four bits, which no instruction in this build reads or writes.
    /// </summary>
    /// <remarks>
    /// It is zero in every slot this build produces, and it exists so that the width of a slot does
    /// not change on the day the vector manifest opens. A reader who wants to know whether this
    /// profile implements vectors should read the descriptor's accepted manifests and not this
    /// member.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=54560D
    // Broiler-Human:        PENDING
    internal ulong High => high;

    /// <summary>The slot a 32-bit integer occupies, zero-extended into the payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=59215E
    // Broiler-Human:        PENDING
    internal static WasmValue FromI32(int value) => new((ulong)(uint)value, 0);

    /// <summary>The slot a 64-bit integer occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9E8191
    // Broiler-Human:        PENDING
    internal static WasmValue FromI64(long value) => new((ulong)value, 0);

    /// <summary>The slot a single-precision float occupies, as its exact bit pattern.</summary>
    /// <remarks>
    /// It is held as bits rather than as a <see cref="float"/> field so that a NaN's payload
    /// survives being stored and read back. A float moved through a floating-point register can be
    /// quieted on some architectures, and a slot that lost a NaN payload would lose it silently.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=44A97E
    // Broiler-Falsified-If: a stored NaN reads back with a different payload than it was written with
    // Broiler-Human:        PENDING
    internal static WasmValue FromF32(float value) =>
        new(System.BitConverter.SingleToUInt32Bits(value), 0);

    /// <summary>The slot a double-precision float occupies, as its exact bit pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5FFF79
    // Broiler-Falsified-If: a stored NaN reads back with a different payload than it was written with
    // Broiler-Human:        PENDING
    internal static WasmValue FromF64(double value) =>
        new(System.BitConverter.DoubleToUInt64Bits(value), 0);

    /// <summary>The slot built from raw bits, used by the loads that widen a narrow field.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9EBCCF
    // Broiler-Human:        PENDING
    internal static WasmValue FromBits(ulong bits) => new(bits, 0);

    /// <summary>Reads the payload as a 32-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BC18D1
    // Broiler-Human:        PENDING
    internal int I32 => (int)(uint)low;

    /// <summary>Reads the payload as a 64-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2D3BF5
    // Broiler-Human:        PENDING
    internal long I64 => (long)low;

    /// <summary>Reads the payload as a single-precision float.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BE6955
    // Broiler-Human:        PENDING
    internal float F32 => System.BitConverter.UInt32BitsToSingle((uint)low);

    /// <summary>Reads the payload as a double-precision float.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2E8763
    // Broiler-Human:        PENDING
    internal double F64 => System.BitConverter.UInt64BitsToDouble(low);

    /// <summary>The zero slot, which is what every local and every table slot starts as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D6381D
    // Broiler-Human:        PENDING
    internal static WasmValue Zero => new(0, 0);
}
