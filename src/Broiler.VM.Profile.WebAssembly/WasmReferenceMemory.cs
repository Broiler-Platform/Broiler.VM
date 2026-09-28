// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Critical
// Criteria:         5/5
// Resource impact:  2/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly profile's own load, store and <c>memory.size</c> arms: the reference handler of the
/// family's twenty-four region rows.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE ARMS WERE MOVED HERE FROM THE RETIRED INTERPRETER, AND NOT ONE CHANGED BUT FOR THE SLOT.</b>
/// The dispatch is the bare-module interpreter's <c>MemoryAccess</c>, the range checks and the byte
/// order are its memory instance's <c>TryLoad</c> and <c>TryStore</c>, and <c>memory.size</c> is its
/// arm answering the page count; they are recovered from the commit before milestone UBC-4 retired
/// that path, as the numeric arms were kept in <see cref="WasmReferenceNumerics"/>. The retired slot's
/// conversions are written out where they were called: a thirty-two-bit result zero-extended into
/// the word, a sixty-four-bit one as its bits, a float as its bits.
/// </para>
/// <para>
/// <b>Why a reference exists for rows the family executes through the primitive table.</b> The
/// family's own handler for a region row calls the table's region primitive, so comparing it with
/// the table would compare the table with itself. Obligation E2 compares the profile's own arms with
/// the table, as it does for the numeric rows, and these are those arms. Nothing here is on the
/// execution path.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=494A15
// Broiler-Falsified-If: an arm here answers differently from the retired interpreter arm it was moved from, or the effective address is summed in thirty-two-bit arithmetic
// Broiler-Human:        PENDING
internal static class WasmReferenceMemory
{
    /// <summary>
    /// Runs the arm of the region row <paramref name="opcode"/> over <paramref name="memory"/>, the
    /// dynamic <paramref name="address"/>, the static <paramref name="offset"/> and, for a store,
    /// <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// Answers true with a load's or <c>memory.size</c>'s result bits, a thirty-two-bit result's in the
    /// low half, and zero bits for a store; or true with the trap the arm raised in
    /// <paramref name="trap"/> and zero bits. <paramref name="trap"/> is zero, which names no trap, when
    /// none was raised. A store that does not trap has written <paramref name="memory"/>. Answers false
    /// for a byte that is not a region row of the family's table.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=ABA14E
    // Broiler-Falsified-If: an answer is given for a byte that is no region row, or a trapping store writes any byte
    // Broiler-Human:        PENDING
    internal static bool TryEvaluate(
        byte opcode,
        System.Span<byte> memory,
        uint address,
        uint offset,
        ulong value,
        out ulong bits,
        out WasmTrapKind trap)
    {
        bits = 0;
        trap = 0;

        if (opcode == WasmFamilyTable.MemorySize)
        {
            // The retired arm pushed WasmValue.FromI32((int)PageCount), and the page count is the
            // memory's length in whole pages.
            bits = FromI32((int)(uint)(memory.Length / WasmMemoryInstance.PageBytes));
            return true;
        }

        if (opcode is < WasmFamilyTable.FirstAccess or > WasmFamilyTable.LastAccess)
        {
            return false;
        }

        if (MemoryAccess(opcode, memory, address, offset, value, out var loaded) is { } raised)
        {
            trap = raised;
            return true;
        }

        bits = loaded;
        return true;
    }

    /// <summary>Executes one load or store, answering the trap it raised or nothing.</summary>
    /// <remarks>
    /// The retired arm read the alignment immediate and discarded it, and read the static offset from
    /// the code; here the offset arrives as a parameter, which is the only difference.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=FFD238
    // Broiler-Falsified-If: an effective address is computed in 32-bit arithmetic, or a width here disagrees with the one validation typed
    // Broiler-Human:        PENDING
    private static WasmTrapKind? MemoryAccess(
        byte opcode,
        System.Span<byte> memory,
        uint address,
        uint offset,
        ulong value,
        out ulong bits)
    {
        bits = 0;

        if (opcode >= 0x36)
        {
            var storeAddress = (ulong)address + offset;

            var width = opcode switch
            {
                0x3A or 0x3C => 1,
                0x3B or 0x3D => 2,
                0x36 or 0x38 or 0x3E => 4,
                _ => 8,
            };

            return TryStore(memory, storeAddress, width, value)
                ? null
                : WasmTrapKind.OutOfBoundsMemoryAccess;
        }

        var loadAddress = (ulong)address + offset;

        var loadWidth = opcode switch
        {
            0x2C or 0x2D or 0x30 or 0x31 => 1,
            0x2E or 0x2F or 0x32 or 0x33 => 2,
            0x28 or 0x2A or 0x34 or 0x35 => 4,
            _ => 8,
        };

        if (!TryLoad(memory, loadAddress, loadWidth, out var raw))
        {
            return WasmTrapKind.OutOfBoundsMemoryAccess;
        }

        bits = opcode switch
        {
            0x2C => FromI32((sbyte)raw),
            0x2D => FromI32((byte)raw),
            0x2E => FromI32((short)raw),
            0x2F => FromI32((ushort)raw),
            0x30 => FromI64((sbyte)raw),
            0x31 => FromI64((byte)raw),
            0x32 => FromI64((short)raw),
            0x33 => FromI64((ushort)raw),
            0x34 => FromI64((int)(uint)raw),
            0x35 => FromI64((long)(uint)raw),
            _ => raw,
        };

        return null;
    }

    /// <summary>Reads <paramref name="width"/> bytes little-endian at an address, or refuses.</summary>
    /// <remarks>
    /// The address arrives already summed in 64 bits from a 32-bit dynamic address and a 32-bit
    /// static offset, so the sum cannot have wrapped before it got here; the comparison below is
    /// therefore the only thing standing between a guest and the rest of the heap.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=7E104D
    // Broiler-Falsified-If: it reads a byte at or past the current size, or the range check is performed in 32-bit arithmetic
    // Broiler-Human:        PENDING
    private static bool TryLoad(System.Span<byte> bytes, ulong address, int width, out ulong bits)
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=47D921
    // Broiler-Falsified-If: it writes a byte at or past the current size, or the range check is performed in 32-bit arithmetic
    // Broiler-Human:        PENDING
    private static bool TryStore(System.Span<byte> bytes, ulong address, int width, ulong bits)
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

    /// <summary>The retired slot's <c>FromI32</c>: a 32-bit integer zero-extended into the bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=76587D
    // Broiler-Human:        PENDING
    private static ulong FromI32(int value) => (ulong)(uint)value;

    /// <summary>The retired slot's <c>FromI64</c>: a 64-bit integer's bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6F1F64
    // Broiler-Human:        PENDING
    private static ulong FromI64(long value) => (ulong)value;
}
