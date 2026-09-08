// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           6
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Critical
// Criteria:         6/6
// Resource impact:  4/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// Why one variable-length integer read stopped, in the terms the conformance suite distinguishes.
/// </summary>
/// <remarks>
/// <para>
/// <b>Three of these members exist because the specification's own test suite separates them.</b>
/// An integer whose encoding runs past the byte budget its width allows, an unsigned integer whose
/// terminal byte sets bits above that width, and a signed integer whose terminal byte is not a
/// correct sign extension are three different malformations with three different fixtures, and a
/// decoder reporting one code for all three would satisfy a hand-written test while recording the
/// wrong answer in every retained corpus entry that touched one.
/// </para>
/// <para>
/// <c>ReaderStopped</c> carries no information of its own on purpose: the bounded reader has
/// already latched exactly why it stopped, and the caller maps that status rather than a second,
/// possibly disagreeing, copy of it kept here.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=8E3646
// Broiler-Human:        PENDING
internal enum WasmVarIntStatus : byte
{
    /// <summary>The integer was read.</summary>
    Ok = 0,

    /// <summary>The bounded reader refused a byte; its own latched status says why.</summary>
    ReaderStopped = 1,

    /// <summary>The encoding uses more bytes than the integer's width admits.</summary>
    ByteBudgetExceeded = 2,

    /// <summary>An unsigned integer's terminal byte sets bits above the integer's width.</summary>
    UnusedBitsSet = 3,

    /// <summary>A signed integer's terminal byte is not a correct sign extension.</summary>
    SignExtensionInvalid = 4,

    /// <summary>A declared count did not clear the declared-count ceiling, or its charge was refused.</summary>
    DeclaredCountExhausted = 5,
}

/// <summary>
/// This profile's own LEB128 readers, built over the core's byte-level primitive and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE CORE'S VARIABLE-LENGTH INTEGER READERS ARE CANONICAL-ONLY AND THIS FORMAT IS NOT.</b>
/// <c>TryReadVarUInt32</c>, <c>TryReadVarUInt64</c> and <c>TryReadDeclaredCount</c> reject an
/// encoding carrying a redundant zero continuation, because two encodings of one value would make
/// a byte-identical artifact check meaningless for the format they were written for. The
/// WebAssembly binary format says the opposite: an encoding is well formed if it fits the byte
/// budget its width allows and its terminal byte sets no bit that width does not have, so padding
/// within the budget is legal and production toolchains emit it routinely - a single-pass emitter
/// writes a fixed-width placeholder and patches the value into it. A verifier built on the core's
/// readers would refuse modules a real compiler produces, which is why this class exists and why
/// no call to those three members may appear anywhere in this assembly.
/// </para>
/// <para>
/// <b>Everything else is still the core's.</b> Every byte arrives through
/// <see cref="VmBoundedReader.TryReadByte"/>, so the artifact-bytes bound, the verifier-work
/// charge, the cancellation poll and the latched failure status are the core's mechanism rather
/// than a second implementation of it. What this class re-derives is the acceptance rule, and only
/// the acceptance rule.
/// </para>
/// <para>
/// <b>The signed readers were always this profile's work.</b> The core has no signed reader at all,
/// and this format needs three widths of one: 32 bits for <c>i32.const</c> and the memory
/// immediates, 64 bits for <c>i64.const</c>, and the 33-bit form a block type uses so that a
/// single value type and a type index share one encoding.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=4; Fingerprint=839507
// Broiler-Falsified-If: any member here calls one of the core's canonical variable-length readers, or a padded encoding inside its byte budget is refused
// Broiler-Human:        PENDING
internal static class WasmLeb128
{
    /// <summary>The byte budget a 32-bit or 33-bit integer's encoding may occupy.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=FAEF96
    // Broiler-Human:        PENDING
    internal const int NarrowByteBudget = 5;

    /// <summary>The byte budget a 64-bit integer's encoding may occupy.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=DCEF74
    // Broiler-Human:        PENDING
    internal const int WideByteBudget = 10;

    /// <summary>Reads an unsigned 32-bit integer, accepting padding within five bytes.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=D83660
    // Broiler-Falsified-If: a five-byte encoding whose terminal byte is zero is refused, or a terminal byte above 0x0F is accepted
    // Broiler-Human:        PENDING
    internal static bool TryReadVarU32(
        ref VmBoundedReader reader, out uint value, out WasmVarIntStatus status)
    {
        var read = TryReadUnsigned(ref reader, bits: 32, NarrowByteBudget, out var wide, out status);
        value = (uint)wide;
        return read;
    }

    /// <summary>Reads an unsigned 64-bit integer, accepting padding within ten bytes.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=16A233
    // Broiler-Human:        PENDING
    internal static bool TryReadVarU64(
        ref VmBoundedReader reader, out ulong value, out WasmVarIntStatus status) =>
        TryReadUnsigned(ref reader, bits: 64, WideByteBudget, out value, out status);

    /// <summary>Reads a signed 32-bit integer, accepting sign padding within five bytes.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=8D4328
    // Broiler-Falsified-If: a terminal byte that is not a sign extension of the value's top bit is accepted
    // Broiler-Human:        PENDING
    internal static bool TryReadVarS32(
        ref VmBoundedReader reader, out int value, out WasmVarIntStatus status)
    {
        var read = TryReadSigned(ref reader, bits: 32, NarrowByteBudget, out var wide, out status);
        value = (int)wide;
        return read;
    }

    /// <summary>
    /// Reads the signed 33-bit form a block type is encoded in.
    /// </summary>
    /// <remarks>
    /// The width is 33 rather than 32 because the encoding holds every negative one-byte value type
    /// and every non-negative type index in one number, and a 32-bit signed form could not hold the
    /// whole index space above them. The byte budget stays five: the ceiling of 33 over 7.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=BA03C3
    // Broiler-Human:        PENDING
    internal static bool TryReadVarS33(
        ref VmBoundedReader reader, out long value, out WasmVarIntStatus status) =>
        TryReadSigned(ref reader, bits: 33, NarrowByteBudget, out value, out status);

    /// <summary>Reads a signed 64-bit integer, accepting sign padding within ten bytes.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=063B02
    // Broiler-Human:        PENDING
    internal static bool TryReadVarS64(
        ref VmBoundedReader reader, out long value, out WasmVarIntStatus status) =>
        TryReadSigned(ref reader, bits: 64, WideByteBudget, out value, out status);

    /// <summary>
    /// Reads a vector length: the untrusted number this format uses to say how many of something
    /// follow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE COUNT IS COMPARED AND CHARGED BEFORE IT IS RETURNED, AND THAT ORDER IS THE WHOLE
    /// POINT OF THIS MEMBER.</b> A caller handed a count has a number it may loop on or size a
    /// buffer from; a caller handed <see langword="false"/> has nothing. This is the property
    /// <c>TryReadDeclaredCount</c> provides and which this profile had to re-derive, because that
    /// member reads a canonical integer first and this format's counts are not canonical.
    /// </para>
    /// <para>
    /// <b>The comparison and the charge are two different things.</b> The comparison holds one count
    /// to the effective declared-count ceiling. The charge accumulates every count in the module
    /// against that same ceiling, so a module made of ten thousand small vectors is bounded as well
    /// as a module made of one enormous one. Both refusals reach a caller as the same answer - a
    /// resource exhaustion naming <c>DeclaredCount</c> - and both are refusals rather than
    /// truncations.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=Critical; Resources=3; Fingerprint=A89DDB
    // Broiler-Falsified-If: a count is returned before both the ceiling comparison and the charge have succeeded
    // Broiler-Human:        PENDING
    internal static bool TryReadBoundedCount(
        ref VmBoundedReader reader,
        WasmReadAdapter meter,
        out uint count,
        out WasmVarIntStatus status)
    {
        count = 0;

        if (!TryReadVarU32(ref reader, out var declared, out status))
        {
            return false;
        }

        if (declared > reader.Bounds.MaxDeclaredCount)
        {
            status = WasmVarIntStatus.DeclaredCountExhausted;
            return false;
        }

        if (!meter.TryChargeDeclaredCount(declared))
        {
            status = WasmVarIntStatus.DeclaredCountExhausted;
            return false;
        }

        count = declared;
        return true;
    }

    /// <summary>
    /// The unsigned acceptance rule: at most the width's byte budget, and no bit above the width in
    /// the terminal byte.
    /// </summary>
    /// <remarks>
    /// Padding is accepted by construction rather than by a special case. A continuation byte
    /// carrying a zero payload contributes nothing to the accumulator and costs one byte of the
    /// budget, so an encoding padded up to the budget decodes to the same value as the canonical
    /// one, and an encoding padded past it is refused for exceeding the budget.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=2; Fingerprint=650D29
    // Broiler-Falsified-If: two encodings inside the budget decode to different values, or the shift reaches 64 and wraps
    // Broiler-Human:        PENDING
    private static bool TryReadUnsigned(
        ref VmBoundedReader reader,
        int bits,
        int byteBudget,
        out ulong value,
        out WasmVarIntStatus status)
    {
        value = 0;
        status = WasmVarIntStatus.Ok;

        ulong accumulator = 0;
        var shift = 0;

        for (var index = 1; ; index++)
        {
            if (!reader.TryReadByte(out var current))
            {
                status = WasmVarIntStatus.ReaderStopped;
                return false;
            }

            if (index == byteBudget)
            {
                // The last byte the budget admits may not ask for another one, and the bits it
                // carries above the integer's width must be absent rather than dropped.
                if ((current & 0x80) != 0)
                {
                    status = WasmVarIntStatus.ByteBudgetExceeded;
                    return false;
                }

                if ((current >> (bits - shift)) != 0)
                {
                    status = WasmVarIntStatus.UnusedBitsSet;
                    return false;
                }

                value = accumulator | ((ulong)current << shift);
                return true;
            }

            accumulator |= (ulong)(current & 0x7F) << shift;

            if ((current & 0x80) == 0)
            {
                value = accumulator;
                return true;
            }

            shift += 7;
        }
    }

    /// <summary>
    /// The signed acceptance rule: at most the width's byte budget, and a terminal byte whose bits
    /// above the width are a correct extension of the value's sign bit.
    /// </summary>
    /// <remarks>
    /// A short encoding is sign-extended from the top bit of its own last payload group, which is
    /// what makes <c>0x7F</c> the whole of minus one. A full-budget encoding is checked instead:
    /// the bits the width does not reach must all equal the value's sign bit, so a positive number
    /// padded with ones and a negative number padded with zeros are both refused - and refused with
    /// their own status rather than with the one an over-long encoding gets.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=31C498
    // Broiler-Falsified-If: a sign extension that disagrees with the value's top bit is accepted, or a short encoding is not sign-extended
    // Broiler-Human:        PENDING
    private static bool TryReadSigned(
        ref VmBoundedReader reader,
        int bits,
        int byteBudget,
        out long value,
        out WasmVarIntStatus status)
    {
        value = 0;
        status = WasmVarIntStatus.Ok;

        ulong accumulator = 0;
        var shift = 0;

        for (var index = 1; ; index++)
        {
            if (!reader.TryReadByte(out var current))
            {
                status = WasmVarIntStatus.ReaderStopped;
                return false;
            }

            if (index == byteBudget)
            {
                if ((current & 0x80) != 0)
                {
                    status = WasmVarIntStatus.ByteBudgetExceeded;
                    return false;
                }

                var payloadBits = bits - shift;
                var sign = (current >> (payloadBits - 1)) & 1;
                var above = current >> payloadBits;
                var extension = sign == 1 ? (1 << (7 - payloadBits)) - 1 : 0;

                if (above != extension)
                {
                    status = WasmVarIntStatus.SignExtensionInvalid;
                    return false;
                }

                accumulator |= (ulong)(uint)(current & ((1 << payloadBits) - 1)) << shift;

                if (sign == 1 && bits < 64)
                {
                    accumulator |= ~0UL << bits;
                }

                value = (long)accumulator;
                return true;
            }

            accumulator |= (ulong)(current & 0x7F) << shift;
            shift += 7;

            if ((current & 0x80) == 0)
            {
                if ((current & 0x40) != 0 && shift < 64)
                {
                    accumulator |= ~0UL << shift;
                }

                value = (long)accumulator;
                return true;
            }
        }
    }
}
