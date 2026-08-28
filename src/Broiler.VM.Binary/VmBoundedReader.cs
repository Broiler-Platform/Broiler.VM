// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   21
// Annotated:        21/21
// Exempt:           12
// Human-reviewed:   0/21
// IP risk:          Low
// Security risk:    High
// Criteria:         19/18
// Resource impact:  2/10 max
// Unverified:       21
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// A checked-arithmetic reader over untrusted bytes. Every length, count, offset and index is
/// computed with overflow checking and compared against a bound before it is used.
/// </summary>
/// <remarks>
/// <para>
/// Roadmap section 7's load-time requirements are the specification: checked arithmetic
/// everywhere, no allocation from an untrusted declared count before that count clears its bound,
/// and bounds on artifact bytes, sections and nesting. The reader is a <c>ref struct</c> so it
/// cannot outlive the span it reads or be captured into a field, which is what makes "the bytes
/// were still there" a property of the type rather than a review comment.
/// </para>
/// <para>
/// <see cref="VmReadBounds"/> and <see cref="IVmBoundedAllocationMeter"/> are required constructor
/// parameters. There is deliberately no unbounded overload and no bounds-free constructor: ADR
/// 0007 asks that allocating before a policy exists fail to compile, and the only way to get that
/// is to make the policy impossible to omit.
/// </para>
/// <para>
/// No member throws. A malformed artifact is the expected input on this path, so every member
/// returns <see langword="false"/> and latches <see cref="Status"/>. Once the status is not
/// <see cref="VmBoundedReadStatus.Ok"/> the reader is spent: every later call returns
/// <see langword="false"/> without re-examining the bytes, so one failure cannot be stepped past
/// by a caller that ignored a return value.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CCF177
// Broiler-Falsified-If: a public member examines bytes or advances position while Status is not Ok
// Broiler-Human:        PENDING
public ref struct VmBoundedReader
{
    private readonly System.ReadOnlySpan<byte> bytes;
    private readonly VmReadBounds bounds;
    private readonly IVmBoundedAllocationMeter meter;
    private ulong position;
    private ulong sectionsEntered;
    private uint structuralDepth;
    private VmBoundedReadStatus status;

    /// <summary>
    /// Creates a reader over <paramref name="source"/>, bounded by <paramref name="readBounds"/>
    /// and charging <paramref name="allocationMeter"/>.
    /// </summary>
    /// <remarks>
    /// A source longer than the artifact bound does not throw and does not truncate silently: the
    /// reader is constructed already failed with
    /// <see cref="VmBoundedReadStatus.ArtifactBytesExceeded"/>, so the caller learns it on the
    /// first read exactly as it learns every other bound.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=97DF17
    // Broiler-Falsified-If: a source longer than MaxArtifactBytes leaves Status Ok, so the excess truncates silently
    // Broiler-Human:        PENDING
    public VmBoundedReader(
        System.ReadOnlySpan<byte> source,
        in VmReadBounds readBounds,
        IVmBoundedAllocationMeter allocationMeter)
    {
        bytes = source;
        bounds = readBounds;
        meter = allocationMeter ?? throw new System.ArgumentNullException(nameof(allocationMeter));
        position = 0;
        sectionsEntered = 0;
        structuralDepth = 0;
        status = (ulong)source.Length > readBounds.MaxArtifactBytes
            ? VmBoundedReadStatus.ArtifactBytesExceeded
            : VmBoundedReadStatus.Ok;
    }

    /// <summary>How many bytes have been consumed.</summary>
    public readonly ulong Position => position;

    /// <summary>How many bytes remain unconsumed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D3559E
    // Broiler-Falsified-If: position can exceed bytes.Length, so the subtraction wraps to a remainder larger than the span
    // Broiler-Human:        PENDING
    public readonly ulong Remaining => (ulong)bytes.Length - position;

    /// <summary>The current section nesting depth.</summary>
    public readonly uint StructuralDepth => structuralDepth;

    /// <summary>How many sections have been entered so far.</summary>
    public readonly ulong SectionsEntered => sectionsEntered;

    /// <summary>Why the reader stopped, or <see cref="VmBoundedReadStatus.Ok"/> if it has not.</summary>
    public readonly VmBoundedReadStatus Status => status;

    /// <summary>True while no bound has been reached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A98E73
    // Broiler-Human:        PENDING
    public readonly bool IsOk => status == VmBoundedReadStatus.Ok;

    /// <summary>The bounds this reader was constructed with.</summary>
    public readonly VmReadBounds Bounds => bounds;

    /// <summary>Reads one byte.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EADE4A
    // Broiler-Falsified-If: bytes is indexed on a path where TryConsume(1) returned false, or the (int) index leaves the span
    // Broiler-Human:        PENDING
    public bool TryReadByte(out byte value)
    {
        value = 0;

        if (!TryConsume(1))
        {
            return false;
        }

        value = bytes[(int)(position - 1)];
        return true;
    }

    /// <summary>Reads a little-endian 32-bit unsigned integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E8E77F
    // Broiler-Falsified-If: a window shorter than four bytes reaches the shifts, or the assembly is not little-endian
    // Broiler-Human:        PENDING
    public bool TryReadUInt32LittleEndian(out uint value)
    {
        value = 0;

        if (!TryTake(4, out var window))
        {
            return false;
        }

        value = (uint)window[0]
            | ((uint)window[1] << 8)
            | ((uint)window[2] << 16)
            | ((uint)window[3] << 24);

        return true;
    }

    /// <summary>Reads a little-endian 64-bit unsigned integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BF1BC9
    // Broiler-Falsified-If: a window shorter than eight bytes reaches the loop, or the descending loop is not little-endian
    // Broiler-Human:        PENDING
    public bool TryReadUInt64LittleEndian(out ulong value)
    {
        value = 0;

        if (!TryTake(8, out var window))
        {
            return false;
        }

        ulong accumulator = 0;

        for (var index = 7; index >= 0; index--)
        {
            accumulator = (accumulator << 8) | window[index];
        }

        value = accumulator;
        return true;
    }

    /// <summary>
    /// Reads an LEB128 variable-length unsigned 32-bit integer.
    /// </summary>
    /// <remarks>
    /// Over-long encodings are rejected rather than accepted and truncated. Two encodings of one
    /// value would make a byte-identical artifact check meaningless and would let a payload carry
    /// a value past a length check that read it differently, so the canonical form is the only
    /// accepted form.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A13073
    // Broiler-Falsified-If: two distinct byte sequences both return true with the same value, or the (uint) cast drops bits
    // Broiler-Human:        PENDING
    public bool TryReadVarUInt32(out uint value)
    {
        value = 0;

        if (!TryReadVarUInt64Core(maxBits: 32, out var wide))
        {
            return false;
        }

        value = (uint)wide;
        return true;
    }

    /// <summary>Reads an LEB128 variable-length unsigned 64-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=69F550
    // Broiler-Falsified-If: two distinct byte sequences both return true with one value; shift 63 is the case to try
    // Broiler-Human:        PENDING
    public bool TryReadVarUInt64(out ulong value) => TryReadVarUInt64Core(maxBits: 64, out value);

    /// <summary>
    /// Reads an untrusted declared count and refuses it if it exceeds the configured bound.
    /// </summary>
    /// <remarks>
    /// This is the member a verifier calls anywhere a payload says how many of something follow.
    /// It refuses before the count is returned, so a caller cannot loop, size a buffer, or reserve
    /// capacity from a number that never passed its bound.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D8A056
    // Broiler-Falsified-If: a count is returned before its comparison with MaxDeclaredCount, or no path here calls TryReserve
    // Broiler-Human:        PENDING
    public bool TryReadDeclaredCount(out uint count)
    {
        count = 0;

        if (!TryReadVarUInt32(out var declared))
        {
            return false;
        }

        if (declared > bounds.MaxDeclaredCount)
        {
            return Fail(VmBoundedReadStatus.DeclaredCountExceeded);
        }

        count = declared;
        return true;
    }

    /// <summary>Takes a bounded window of <paramref name="length"/> bytes without copying it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=58DE6E
    // Broiler-Falsified-If: window is set on a path where TryTake returned false, or its length is not the length asked for
    // Broiler-Human:        PENDING
    public bool TryReadBytes(ulong length, out System.ReadOnlySpan<byte> window)
    {
        window = default;

        if (!TryTake(length, out var taken))
        {
            return false;
        }

        window = taken;
        return true;
    }

    /// <summary>
    /// Enters a section of <paramref name="declaredLength"/> bytes, charging the section count and
    /// the structural-depth bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2E1BF5
    // Broiler-Falsified-If: a frame is minted before the length, section-count and depth bounds have all been compared
    // Broiler-Human:        PENDING
    public bool TryEnterSection(ulong declaredLength, out VmSectionFrame frame)
    {
        frame = default;

        if (!Check())
        {
            return false;
        }

        if (declaredLength > Remaining)
        {
            return Fail(VmBoundedReadStatus.Truncated);
        }

        if (sectionsEntered == bounds.MaxSectionCount)
        {
            return Fail(VmBoundedReadStatus.SectionCountExceeded);
        }

        if (structuralDepth >= bounds.MaxStructuralDepth)
        {
            return Fail(VmBoundedReadStatus.StructuralDepthExceeded);
        }

        sectionsEntered++;
        structuralDepth++;
        frame = new VmSectionFrame(position, declaredLength, structuralDepth);
        return true;
    }

    /// <summary>
    /// Leaves a section, checking that exactly its declared length was consumed.
    /// </summary>
    /// <remarks>
    /// Consuming less than declared is as much a structural error as consuming more: it means the
    /// artifact and the verifier disagree about where the next section starts, which is precisely
    /// the confusion framing exists to prevent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D4F2B9
    // Broiler-Falsified-If: a frame this reader never minted reaches here and Start + DeclaredLength wraps
    // Broiler-Human:        PENDING
    public bool TryExitSection(in VmSectionFrame frame)
    {
        if (!Check())
        {
            return false;
        }

        if (frame.Depth != structuralDepth)
        {
            return Fail(VmBoundedReadStatus.MalformedEncoding);
        }

        var end = frame.Start + frame.DeclaredLength;

        if (position != end)
        {
            return Fail(VmBoundedReadStatus.MalformedEncoding);
        }

        structuralDepth--;
        return true;
    }

    /// <summary>Skips to the end of a section without reading its body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E174C9
    // Broiler-Falsified-If: the end sum wraps past the bytes.Length test, or position advances on a refused ChargeWork
    // Broiler-Human:        PENDING
    public bool TrySkipSectionBody(in VmSectionFrame frame)
    {
        if (!Check())
        {
            return false;
        }

        var end = frame.Start + frame.DeclaredLength;

        if (end < position || end > (ulong)bytes.Length)
        {
            return Fail(VmBoundedReadStatus.Truncated);
        }

        if (!ChargeWork(end - position))
        {
            return false;
        }

        position = end;
        return true;
    }

    /// <summary>
    /// Charges the verifier-work allowance and polls for cancellation. A verifier calls this at
    /// the granularity its descriptor declares.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D3A8E1
    // Broiler-Falsified-If: the meter is charged while Status is not Ok, so a spent reader keeps spending the allowance
    // Broiler-Human:        PENDING
    public bool TryChargeWork(ulong workUnits)
    {
        if (!Check())
        {
            return false;
        }

        return ChargeWork(workUnits);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6D9975
    // Broiler-Falsified-If: the (int) casts narrow an index or length TryConsume allowed, so the slice leaves the span
    // Broiler-Human:        PENDING
    private bool TryTake(ulong length, out System.ReadOnlySpan<byte> window)
    {
        window = default;

        if (!TryConsume(length))
        {
            return false;
        }

        window = bytes.Slice((int)(position - length), (int)length);
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7DE9F2
    // Broiler-Falsified-If: position advances past a failed bound test or a refused ChargeWork, or the addition is unchecked
    // Broiler-Human:        PENDING
    private bool TryConsume(ulong length)
    {
        if (!Check())
        {
            return false;
        }

        if (length > Remaining)
        {
            return Fail(VmBoundedReadStatus.Truncated);
        }

        if (!ChargeWork(length))
        {
            return false;
        }

        // Checked because a caller-supplied length is untrusted even when it is in range: the
        // addition is the one place a bound check could be stepped past by wrapping.
        checked
        {
            position += length;
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=DE9CB5
    // Broiler-Falsified-If: an over-long encoding is accepted: a group past maxBits, an overflowing tail, a zero continuation
    // Broiler-Human:        PENDING
    private bool TryReadVarUInt64Core(int maxBits, out ulong value)
    {
        value = 0;

        if (!Check())
        {
            return false;
        }

        var shift = 0;
        ulong accumulator = 0;

        while (true)
        {
            if (!TryConsume(1))
            {
                return false;
            }

            var current = bytes[(int)(position - 1)];
            var payload = (ulong)(current & 0x7F);

            if (shift >= maxBits)
            {
                return Fail(VmBoundedReadStatus.MalformedEncoding);
            }

            // The final group may not carry bits that would not fit, and it may not be a
            // redundant zero continuation: both are non-canonical encodings of a value that
            // already had one.
            if (shift + 7 > maxBits && payload >= 1UL << (maxBits - shift))
            {
                return Fail(VmBoundedReadStatus.MalformedEncoding);
            }

            accumulator |= payload << shift;

            if ((current & 0x80) == 0)
            {
                if (shift > 0 && current == 0)
                {
                    return Fail(VmBoundedReadStatus.MalformedEncoding);
                }

                value = accumulator;
                return true;
            }

            shift += 7;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=183D6C
    // Broiler-Falsified-If: WorkBudgetExhausted is latched for a Poll that returned false under cancellation, not exhaustion
    // Broiler-Human:        PENDING
    private bool ChargeWork(ulong workUnits)
    {
        if (!meter.TryChargeWork(workUnits) || !meter.Poll())
        {
            return Fail(VmBoundedReadStatus.WorkBudgetExhausted);
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=452009
    // Broiler-Human:        PENDING
    private readonly bool Check() => status == VmBoundedReadStatus.Ok;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=057954
    // Broiler-Falsified-If: a later Fail overwrites an earlier non-Ok status, so the echo replaces the first cause
    // Broiler-Human:        PENDING
    private bool Fail(VmBoundedReadStatus reason)
    {
        // The first failure is the one retained. A later call that also fails describes a reader
        // that was already spent, and overwriting would replace the real cause with its echo.
        if (status == VmBoundedReadStatus.Ok)
        {
            status = reason;
        }

        return false;
    }
}
