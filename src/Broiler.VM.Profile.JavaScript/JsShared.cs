// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           9
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// A shared data block: the bytes a <c>SharedArrayBuffer</c> holds, and the waiter list its
/// <c>Atomics.wait</c> and <c>Atomics.notify</c> meet in (JSD-0041).
/// </summary>
/// <remarks>
/// <para>
/// <b>The block, not the buffer object, is what is shared.</b> A buffer object belongs to one realm;
/// the block is what every view over it reads and what a second agent will hold, so the waiter list
/// and the lock that guards it live here, keyed by byte index as the specification keys its
/// <c>WaiterList</c>s.
/// </para>
/// <para>
/// <b>A growable block replaces its array when it grows</b>, as a resizable <c>ArrayBuffer</c> does,
/// under <see cref="Gate"/>. A growth that races another agent's plain write may lose that write;
/// with one agent - all this profile runs until agents land - nothing races it (JSD-0041 section 3).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5987D4
// Broiler-Human:        PENDING
internal sealed class JsSharedBlock
{
    /// <summary>Creates a zero-filled block of <paramref name="byteLength"/> bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=72A8EA
    // Broiler-Human:        PENDING
    internal JsSharedBlock(int byteLength, int? maxByteLength)
    {
        Bytes = new byte[byteLength];
        MaxByteLength = maxByteLength;
    }

    /// <summary>The block's bytes; an array of exactly its byte length.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8335A1
    // Broiler-Human:        PENDING
    internal byte[] Bytes { get; set; }

    /// <summary>The most a growable block may hold, or nothing for a fixed-length one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B169FC
    // Broiler-Human:        PENDING
    internal int? MaxByteLength { get; }

    /// <summary>
    /// The specification's critical section for this block: it guards <see cref="Waiters"/>, a
    /// growth, and the atomic accesses that are not lock-free (the one- and two-byte widths).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=66BB92
    // Broiler-Human:        PENDING
    internal object Gate { get; } = new();

    /// <summary>Every waiter on this block, in the order it began waiting.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=108FE8
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsWaiter> Waiters { get; } = [];
}

/// <summary>One agent waiting on one byte index of a shared block: <c>Atomics.wait</c> or <c>waitAsync</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E3F343
// Broiler-Human:        PENDING
internal sealed class JsWaiter
{
    /// <summary>Creates a waiter on <paramref name="byteIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=958DF2
    // Broiler-Human:        PENDING
    internal JsWaiter(int byteIndex, long deadline, JsPromiseObject? promise)
    {
        ByteIndex = byteIndex;
        Deadline = deadline;
        Promise = promise;
    }

    /// <summary>The byte index waited on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0FE60C
    // Broiler-Human:        PENDING
    internal int ByteIndex { get; }

    /// <summary>When the wait times out, in <see cref="System.Environment.TickCount64"/> milliseconds; <see cref="long.MaxValue"/> for never.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CCBBF0
    // Broiler-Human:        PENDING
    internal long Deadline { get; }

    /// <summary>The promise an asynchronous wait settles, or nothing for a blocking one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=75AEA5
    // Broiler-Human:        PENDING
    internal JsPromiseObject? Promise { get; }

    /// <summary>Whether a notification removed this waiter from its list; set under the block's gate.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A3094D
    // Broiler-Human:        PENDING
    internal bool Notified { get; set; }
}

/// <summary>
/// The raw, sequentially consistent element accesses <c>Atomics</c> performs on a buffer's bytes.
/// </summary>
/// <remarks>
/// <para>
/// <b>Four- and eight-byte accesses are lock-free</b>: a compare-and-swap loop over the element with
/// <see cref="System.Threading.Interlocked"/>, which is atomic on every platform this profile claims.
/// <b>One- and two-byte accesses take the block's gate</b>, so <c>Atomics.isLockFree</c> answers
/// <see langword="false"/> for them, truthfully; the specification makes only the four-byte answer
/// normative. A view's elements are aligned to their width within the buffer, and a <c>byte[]</c>'s
/// data is eight-byte aligned, so every access the two lock-free widths make is aligned.
/// </para>
/// <para>
/// <b>Values are raw bit patterns</b>, zero-extended to 64 bits; the caller converts a JavaScript value
/// to the element's bits before and the old bits to a value after. Elements are little-endian, the
/// order every typed array here uses; a big-endian process takes the gate for every width.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=501B83
// Broiler-Falsified-If: an atomic access is not sequentially consistent with another atomic access to the same element, or a four-byte one takes a lock
// Broiler-Human:        PENDING
internal static class JsAtomicAccess
{
    /// <summary>Whether an access of <paramref name="width"/> bytes is lock-free here.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1530B5
    // Broiler-Human:        PENDING
    internal static bool IsLockFree(int width) =>
        System.BitConverter.IsLittleEndian && width is 4 or 8;

    /// <summary>Reads an element's bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BEEA95
    // Broiler-Human:        PENDING
    internal static long Load(byte[] bytes, int at, int width, object gate) =>
        ReadModifyWrite(bytes, at, width, gate, static (old, _) => old, 0);

    /// <summary>
    /// Replaces an element's bits with <paramref name="operate"/>'s answer, atomically, and answers the
    /// bits it held before.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=91A202
    // Broiler-Falsified-If: an atomic access is not sequentially consistent with another atomic access to the same element, or a four-byte one takes a lock
    // Broiler-Human:        PENDING
    internal static long ReadModifyWrite(
        byte[] bytes, int at, int width, object gate, System.Func<long, long, long> operate, long operand)
    {
        var mask = width == 8 ? -1L : (1L << (width * 8)) - 1;

        if (IsLockFree(width) && width == 4)
        {
            ref var cell = ref System.Runtime.CompilerServices.Unsafe.As<byte, int>(ref bytes[at]);

            while (true)
            {
                var old = System.Threading.Volatile.Read(ref cell);
                var next = unchecked((int)(operate((uint)old, operand) & mask));

                if (System.Threading.Interlocked.CompareExchange(ref cell, next, old) == old)
                {
                    return (uint)old;
                }
            }
        }

        if (IsLockFree(width) && width == 8)
        {
            ref var cell = ref System.Runtime.CompilerServices.Unsafe.As<byte, long>(ref bytes[at]);

            while (true)
            {
                var old = System.Threading.Interlocked.Read(ref cell);
                var next = operate(old, operand);

                if (System.Threading.Interlocked.CompareExchange(ref cell, next, old) == old)
                {
                    return old;
                }
            }
        }

        lock (gate)
        {
            long old = 0;

            for (var b = 0; b < width; b++)
            {
                old |= (long)bytes[at + b] << (b * 8);
            }

            var next = operate(old, operand) & mask;

            for (var b = 0; b < width; b++)
            {
                bytes[at + b] = unchecked((byte)(next >> (b * 8)));
            }

            return old;
        }
    }

    /// <summary>
    /// Writes <paramref name="replacement"/> where the element holds <paramref name="expected"/>, and
    /// answers the bits it held.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=85C53B
    // Broiler-Human:        PENDING
    internal static long CompareExchange(byte[] bytes, int at, int width, object gate, long expected, long replacement)
    {
        // BOTH OPERANDS ARE THE ELEMENT'S RAW BYTES, as NumericToRawBytes makes them: an Int8Array's
        // expected 0x1FF compares as 0xFF.
        var mask = width == 8 ? -1L : (1L << (width * 8)) - 1;
        var wanted = expected & mask;
        var written = replacement & mask;

        return ReadModifyWrite(bytes, at, width, gate, (old, _) => old == wanted ? written : old, 0);
    }
}
