// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           20
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The nine element types this realm's typed arrays store.</summary>
/// <remarks>
/// <para>
/// <b>Nine and not eleven.</b> <c>BigInt64Array</c> and <c>BigUint64Array</c> are absent because
/// the realm has no BigInt: their elements read as a BigInt and there is no such value here, so an
/// implementation would have to answer a Number and lose the top bits it exists to carry. An
/// absent constructor is a <c>ReferenceError</c> a guest can see and route around; a lossy one is
/// a wrong answer it cannot.
/// </para>
/// <para>
/// <b><c>Uint8Clamped</c> is a kind and not a flag on <c>Uint8</c>.</b> The two read identically -
/// one byte, zero to 255 - and differ only in how a write converts, which is exactly the shape a
/// separate kind expresses without a second field that every read would have to ignore.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8FE814
// Broiler-Human:        PENDING
internal enum JsElementKind : byte
{
    /// <summary>A signed byte, written by the language's modular <c>ToInt32</c> wrap.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=85F9FD
    // Broiler-Human:        PENDING
    Int8 = 0,

    /// <summary>An unsigned byte, written by the same modular wrap.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FFE55F
    // Broiler-Human:        PENDING
    Uint8 = 1,

    /// <summary>An unsigned byte whose write CLAMPS to 0..255 rather than wrapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=282A28
    // Broiler-Human:        PENDING
    Uint8Clamped = 2,

    /// <summary>A signed 16-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A88DE4
    // Broiler-Human:        PENDING
    Int16 = 3,

    /// <summary>An unsigned 16-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=22740D
    // Broiler-Human:        PENDING
    Uint16 = 4,

    /// <summary>A signed 32-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F3BAE6
    // Broiler-Human:        PENDING
    Int32 = 5,

    /// <summary>An unsigned 32-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=41AC39
    // Broiler-Human:        PENDING
    Uint32 = 6,

    /// <summary>An IEEE 754 binary32, which a write narrows to and a read widens from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C7717F
    // Broiler-Human:        PENDING
    Float32 = 7,

    /// <summary>An IEEE 754 binary64: the language's own Number, stored exactly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CAC440
    // Broiler-Human:        PENDING
    Float64 = 8,
}

/// <summary>
/// What each <see cref="JsElementKind"/> is worth in bytes, what it is called, and the one pair of
/// routines that moves a Number in and out of those bytes.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every byte goes through <c>BinaryPrimitives</c>, never through <c>BitConverter</c>.</b> The
/// language says a typed array's bytes are the platform's order and a <c>DataView</c>'s are
/// whichever the caller asked for, and this profile answers "little-endian" for the platform on
/// every machine. <c>BitConverter</c> would answer the host's order instead, which means the same
/// program produces different bytes on a big-endian host and a conformance run that passes on the
/// developer's laptop says nothing about the one that does not. Declaring one order and writing it
/// explicitly costs nothing on the hosts anybody runs and makes the answer a property of the
/// profile rather than of the machine.
/// </para>
/// <para>
/// <b>The write conversions are the language's, not the CLR's.</b> A C# cast from <c>double</c> to
/// <c>int</c> is saturating and undefined for NaN; the language's <c>ToInt32</c> is modular and
/// answers zero for NaN, so <c>new Int8Array(1)[0] = 1e30</c> is 0 and not 127. Every integer kind
/// therefore goes through <see cref="JsValue.ToUint32(double)"/> and keeps the low bits, and only
/// <see cref="JsElementKind.Uint8Clamped"/> - which the specification defines as a clamp with
/// round-half-to-even - departs from that.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8BF214
// Broiler-Human:        PENDING
internal static class JsElements
{
    /// <summary>The nine kinds, in the order the constructors are built in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9CC2BA
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.IReadOnlyList<JsElementKind> All { get; } =
    [
        JsElementKind.Int8,
        JsElementKind.Uint8,
        JsElementKind.Uint8Clamped,
        JsElementKind.Int16,
        JsElementKind.Uint16,
        JsElementKind.Int32,
        JsElementKind.Uint32,
        JsElementKind.Float32,
        JsElementKind.Float64,
    ];

    /// <summary>How many bytes one element of <paramref name="kind"/> occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C58A47
    // Broiler-Human:        PENDING
    internal static int WidthOf(JsElementKind kind) => kind switch
    {
        JsElementKind.Int8 or JsElementKind.Uint8 or JsElementKind.Uint8Clamped => 1,
        JsElementKind.Int16 or JsElementKind.Uint16 => 2,
        JsElementKind.Float64 => 8,
        _ => 4,
    };

    /// <summary>The global name the constructor of <paramref name="kind"/> is bound to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9C9067
    // Broiler-Human:        PENDING
    internal static string ConstructorNameOf(JsElementKind kind) => kind switch
    {
        JsElementKind.Int8 => "Int8Array",
        JsElementKind.Uint8 => "Uint8Array",
        JsElementKind.Uint8Clamped => "Uint8ClampedArray",
        JsElementKind.Int16 => "Int16Array",
        JsElementKind.Uint16 => "Uint16Array",
        JsElementKind.Int32 => "Int32Array",
        JsElementKind.Uint32 => "Uint32Array",
        JsElementKind.Float32 => "Float32Array",
        _ => "Float64Array",
    };

    /// <summary>Reads one element out of <paramref name="bytes"/> at a byte offset.</summary>
    /// <remarks>
    /// A NaN read out of a <see cref="JsElementKind.Float32"/> or
    /// <see cref="JsElementKind.Float64"/> slot stays a NaN, and a negative zero stays negative:
    /// the bytes are widened, never normalised, so <c>Object.is(-0, new Float64Array([-0])[0])</c>
    /// is true.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=15E380
    // Broiler-Human:        PENDING
    internal static double Read(byte[] bytes, int at, JsElementKind kind, bool littleEndian)
    {
        var span = new System.ReadOnlySpan<byte>(bytes, at, WidthOf(kind));

        return kind switch
        {
            JsElementKind.Int8 => (double)unchecked((sbyte)span[0]),
            JsElementKind.Uint8 or JsElementKind.Uint8Clamped => (double)span[0],
            JsElementKind.Int16 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadInt16LittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadInt16BigEndian(span)),
            JsElementKind.Uint16 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(span)),
            JsElementKind.Int32 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(span)),
            JsElementKind.Uint32 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(span)),
            JsElementKind.Float32 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadSingleLittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadSingleBigEndian(span)),
            _ => littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadDoubleLittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadDoubleBigEndian(span),
        };
    }

    /// <summary>Writes one element into <paramref name="bytes"/> at a byte offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0ADC20
    // Broiler-Human:        PENDING
    internal static void Write(
        byte[] bytes, int at, JsElementKind kind, double value, bool littleEndian)
    {
        var span = new System.Span<byte>(bytes, at, WidthOf(kind));

        switch (kind)
        {
            case JsElementKind.Int8:
            case JsElementKind.Uint8:
                span[0] = unchecked((byte)JsValue.ToUint32(value));
                return;

            case JsElementKind.Uint8Clamped:
                span[0] = Clamp(value);
                return;

            case JsElementKind.Int16:
            case JsElementKind.Uint16:
            {
                var narrowed = unchecked((ushort)JsValue.ToUint32(value));

                if (littleEndian)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(span, narrowed);
                }
                else
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(span, narrowed);
                }

                return;
            }

            case JsElementKind.Int32:
            case JsElementKind.Uint32:
            {
                // ONE WRITE SERVES BOTH SIGNS. Int32 and Uint32 differ in how a READ interprets the
                // four bytes; the bits a write produces are the same modular wrap either way.
                var wrapped = JsValue.ToUint32(value);

                if (littleEndian)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(span, wrapped);
                }
                else
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(span, wrapped);
                }

                return;
            }

            case JsElementKind.Float32:
            {
                var single = (float)value;

                if (littleEndian)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(span, single);
                }
                else
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteSingleBigEndian(span, single);
                }

                return;
            }

            default:
            {
                if (littleEndian)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteDoubleLittleEndian(span, value);
                }
                else
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteDoubleBigEndian(span, value);
                }

                return;
            }
        }
    }

    /// <summary>
    /// The number an element write stores when the object model has no engine to convert with.
    /// </summary>
    /// <remarks>
    /// <b>An object stored into an element becomes NaN rather than running its <c>valueOf</c>.</b>
    /// An indexed write arrives through <see cref="JsObject.SetOwnProperty"/>, which is the
    /// specification's ordinary internal method over own properties and has no frame, no fuel and
    /// no place to put an exception - so it cannot call guest code, and <c>ToNumber</c> of an
    /// object is guest code. Every primitive converts exactly; this is a declared deviation for
    /// the one case that would need to run a program, and it is the same line
    /// <see cref="JsArray"/> draws when it coerces a written <c>length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DEF874
    // Broiler-Human:        PENDING
    internal static double NumberOf(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.String => JsNumberFormat.ToNumber(value.AsString()),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Null => 0,
        _ => double.NaN,
    };

    /// <summary>The clamp <see cref="JsElementKind.Uint8Clamped"/> writes through.</summary>
    /// <remarks>
    /// Round-half-to-EVEN, which is the one place the language rounds that way: 0.5 stores 0 and
    /// 1.5 stores 2. Every other rounding in the language is half-up, so a reader who assumes
    /// <c>Math.round</c>'s rule here gets 1 and 2 and a picture one grey level off.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8FE417
    // Broiler-Human:        PENDING
    private static byte Clamp(double value)
    {
        if (double.IsNaN(value) || value <= 0)
        {
            return 0;
        }

        return value >= 255 ? (byte)255 : (byte)System.Math.Round(value, System.MidpointRounding.ToEven);
    }
}

/// <summary>A block of bytes a typed array or a <c>DataView</c> reads and writes through.</summary>
/// <remarks>
/// <para>
/// <b>Detachment is <see cref="Data"/> becoming <see langword="null"/>, and nothing else.</b> The
/// language lets a buffer be emptied while views over it are still reachable, and every one of
/// those views has to answer "absent" for every index from that instant on. One nullable field
/// gives every view a single test to make and makes the state impossible to half-observe: there is
/// no length that says zero while an array is still there to read.
/// </para>
/// <para>
/// <b>The bytes are a plain <c>byte[]</c> and are never shared between agents.</b> A
/// <c>SharedArrayBuffer</c> is a different type for a reason - it admits concurrent readers - and
/// this profile does not build one. A composition that wants a byte buffer gets exactly a byte
/// buffer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=37B22E
// Broiler-Human:        PENDING
internal sealed class JsArrayBuffer : JsObject
{
    /// <summary>Creates a zero-filled buffer of <paramref name="byteLength"/> bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=39FD4A
    // Broiler-Human:        PENDING
    internal JsArrayBuffer(JsObject? prototype, int byteLength)
        : base(prototype, "ArrayBuffer") => Data = new byte[byteLength];

    /// <summary>The bytes, or <see langword="null"/> once the buffer has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=18F504
    // Broiler-Human:        PENDING
    internal byte[]? Data { get; private set; }

    /// <summary>How many bytes the buffer holds; zero once it is detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=41DA18
    // Broiler-Human:        PENDING
    internal int ByteLength => Data?.Length ?? 0;

    /// <summary>Whether the bytes are gone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=26D0E0
    // Broiler-Human:        PENDING
    internal bool IsDetached => Data is null;

    /// <summary>Detaches the buffer and hands the bytes to the caller.</summary>
    /// <remarks>
    /// The bytes are RETURNED rather than dropped because the only caller is
    /// <c>ArrayBuffer.prototype.transfer</c>, whose whole purpose is to move them into a new
    /// buffer without copying them twice. Returning them also makes the transfer atomic from the
    /// guest's view: there is no instant at which both buffers can be read.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7F5B7A
    // Broiler-Human:        PENDING
    internal byte[]? Detach()
    {
        var released = Data;
        Data = null;
        return released;
    }

    /// <summary>
    /// Fills this buffer's front with <paramref name="count"/> bytes taken from
    /// <paramref name="source"/> at <paramref name="at"/>, answering whether it could.
    /// </summary>
    /// <remarks>
    /// The source is nullable because both callers may be handed the bytes of a buffer that is
    /// already detached - <c>slice</c>, whose argument coercion can run a <c>valueOf</c> that
    /// detaches the receiver, and <c>transfer</c>, which detaches it deliberately. Answering
    /// <see langword="false"/> rather than throwing keeps the choice of error with the built-in
    /// that knows which one the specification names.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D0D4C4
    // Broiler-Human:        PENDING
    internal bool TryCopyFrom(byte[]? source, int at, int count)
    {
        var bytes = Data;

        if (bytes is null || source is null || at < 0 || count < 0 ||
            at + count > source.Length || count > bytes.Length)
        {
            return false;
        }

        System.Array.Copy(source, at, bytes, 0, count);
        return true;
    }
}

/// <summary>A view that reads and writes one element at a time at a caller-chosen byte offset.</summary>
/// <remarks>
/// <para>
/// <b>A <c>DataView</c> is unaligned and big-endian by default, which is the opposite of a typed
/// array on both counts.</b> A typed array's element <c>n</c> lives at <c>n * width</c> and is
/// read in the platform's order, which this profile fixes to little-endian; a <c>DataView</c> reads
/// whatever byte the caller names and, unless the call passes <c>true</c> for
/// <c>littleEndian</c>, reads it in NETWORK order. The two defaults disagreeing is the
/// specification's own decision - a <c>DataView</c> exists to read file and wire formats, and those
/// are overwhelmingly big-endian - and it is the single most common source of "the same bytes read
/// back different" reports, so it is written here rather than left to be discovered.
/// </para>
/// <para>
/// <b>Every access re-checks detachment and range.</b> The buffer can be detached between two
/// calls on the same view, and the offsets this view was built with then describe bytes that no
/// longer exist. Nothing is cached and nothing is trusted from construction time.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AAD290
// Broiler-Human:        PENDING
internal sealed class JsDataView : JsObject
{
    /// <summary>Creates a view over part of <paramref name="buffer"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8ADE4D
    // Broiler-Human:        PENDING
    internal JsDataView(JsObject? prototype, JsArrayBuffer buffer, int byteOffset, int byteLength)
        : base(prototype, "DataView")
    {
        Buffer = buffer;
        ByteOffset = byteOffset;
        ByteLength = byteLength;
    }

    /// <summary>The buffer the view reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=239486
    // Broiler-Human:        PENDING
    internal JsArrayBuffer Buffer { get; }

    /// <summary>Where in the buffer the view starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=04F8CE
    // Broiler-Human:        PENDING
    internal int ByteOffset { get; }

    /// <summary>How many bytes the view spans.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B53D7A
    // Broiler-Human:        PENDING
    internal int ByteLength { get; }

    /// <summary>Whether the buffer under the view has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D18ED
    // Broiler-Human:        PENDING
    internal bool IsDetached => Buffer.IsDetached;

    /// <summary>Reads one element, answering <see langword="false"/> when it is not there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A89907
    // Broiler-Human:        PENDING
    internal bool TryRead(JsElementKind kind, int at, bool littleEndian, out double value)
    {
        var bytes = Buffer.Data;
        var width = JsElements.WidthOf(kind);

        if (bytes is null || at < 0 || at > ByteLength - width ||
            ByteOffset + at + width > bytes.Length)
        {
            value = 0;
            return false;
        }

        value = JsElements.Read(bytes, ByteOffset + at, kind, littleEndian);
        return true;
    }

    /// <summary>Writes one element, answering <see langword="false"/> when it does not fit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E92D06
    // Broiler-Human:        PENDING
    internal bool TryWrite(JsElementKind kind, int at, double value, bool littleEndian)
    {
        var bytes = Buffer.Data;
        var width = JsElements.WidthOf(kind);

        if (bytes is null || at < 0 || at > ByteLength - width ||
            ByteOffset + at + width > bytes.Length)
        {
            return false;
        }

        JsElements.Write(bytes, ByteOffset + at, kind, value, littleEndian);
        return true;
    }
}

/// <summary>
/// An integer-indexed exotic object: a window of one element kind onto a buffer's bytes.
/// </summary>
/// <remarks>
/// <para>
/// <b>The indices are not properties and the ordinary map never sees them.</b> A canonical numeric
/// key in range IS an element - reading it decodes bytes, writing it encodes them - and a
/// canonical numeric key out of range is ABSENT rather than merely "not stored here": the index
/// branch of <see cref="TryGetOwnProperty"/> answers and returns instead of falling through, so
/// the ordinary map can never supply an element the buffer has no bytes for, and the out-of-range
/// branch of <see cref="SetOwnProperty"/> discards the write instead of creating a property. The
/// silence is the specification's: <c>a[99] = 1</c> on a three-element array is not an error in
/// either mode, it simply does not happen.
/// </para>
/// <para>
/// <b>What that cannot reach from here is the prototype chain, and it is a declared deviation.</b>
/// The specification's <c>[[Get]]</c> for an integer-indexed object answers <c>undefined</c> for an
/// out-of-range index WITHOUT consulting the prototype; the engine owns <c>[[Get]]</c> and walks
/// the chain whenever an own property is absent, and an own-property method has no way to stop it.
/// So in a realm where somebody wrote <c>Object.prototype[9] = 42</c>, <c>new Int32Array(3)[9]</c>
/// reads 42 here and <c>undefined</c> in a conforming engine. Closing it means a hook the object
/// model does not have; the write direction, which is the one where a wrong answer would corrupt
/// something, is closed already, because <see cref="SetOwnProperty"/> discards an out-of-range
/// index whatever the chain says.
/// </para>
/// <para>
/// <b>A detached buffer makes the object answer empty for every key, not only for the indices.</b>
/// The specification only empties the indices and leaves an ordinary property that was written
/// before the detach visible; this profile empties the whole own-property view, which keeps
/// "detached" a single test at the top of every method here instead of a condition threaded
/// through five overrides. What it costs is one observable case - a stray expando on a typed array
/// whose buffer was then detached - and what it buys is that no reader has to ask which half of a
/// detached object still answers.
/// </para>
/// <para>
/// <b>The bytes are little-endian on every host.</b> The platform order is a declared property of
/// this profile rather than of the machine it runs on; see <see cref="JsElements"/>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=70D36A
// Broiler-Human:        PENDING
internal sealed class JsTypedArray : JsObject
{
    /// <summary>Creates a view of <paramref name="length"/> elements over a buffer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B37D21
    // Broiler-Human:        PENDING
    internal JsTypedArray(
        JsObject? prototype, JsArrayBuffer buffer, int byteOffset, int length, JsElementKind kind)
        : base(prototype, JsElements.ConstructorNameOf(kind))
    {
        Buffer = buffer;
        ByteOffset = byteOffset;
        Length = length;
        Kind = kind;
    }

    /// <summary>The buffer the elements live in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=239486
    // Broiler-Human:        PENDING
    internal JsArrayBuffer Buffer { get; }

    /// <summary>Where in the buffer element zero starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=04F8CE
    // Broiler-Human:        PENDING
    internal int ByteOffset { get; }

    /// <summary>How many elements the view spans.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EA42CE
    // Broiler-Human:        PENDING
    internal int Length { get; }

    /// <summary>What kind of element the view reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A3DBEE
    // Broiler-Human:        PENDING
    internal JsElementKind Kind { get; }

    /// <summary>How wide one element is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=144D95
    // Broiler-Human:        PENDING
    internal int BytesPerElement => JsElements.WidthOf(Kind);

    /// <summary>How many bytes the view spans; zero once the buffer is detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C523D9
    // Broiler-Human:        PENDING
    internal int ByteLength => Buffer.IsDetached ? 0 : Length * BytesPerElement;

    /// <summary>Whether the buffer under the view has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D18ED
    // Broiler-Human:        PENDING
    internal bool IsDetached => Buffer.IsDetached;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7F1CF4
    // Broiler-Human:        PENDING
    internal override int OwnPropertyCount =>
        Buffer.IsDetached ? 0 : Length + base.OwnPropertyCount;

    /// <summary>Reads element <paramref name="at"/>, or answers that there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FCCE91
    // Broiler-Human:        PENDING
    internal bool TryReadAt(int at, out double value)
    {
        var bytes = Buffer.Data;

        if (bytes is null || at < 0 || at >= Length)
        {
            value = 0;
            return false;
        }

        value = JsElements.Read(bytes, ByteOffset + (at * BytesPerElement), Kind, true);
        return true;
    }

    /// <summary>Writes element <paramref name="at"/>, or answers that there is nowhere to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=67433F
    // Broiler-Human:        PENDING
    internal bool TryWriteAt(int at, double value)
    {
        var bytes = Buffer.Data;

        if (bytes is null || at < 0 || at >= Length)
        {
            return false;
        }

        JsElements.Write(bytes, ByteOffset + (at * BytesPerElement), Kind, value, true);
        return true;
    }

    /// <summary>Element <paramref name="at"/> as a value, which is <c>undefined</c> when absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C05C21
    // Broiler-Human:        PENDING
    internal JsValue ElementAt(int at) =>
        TryReadAt(at, out var value) ? JsValue.Number(value) : JsValue.Undefined;

    /// <summary>Moves <paramref name="count"/> elements inside this view.</summary>
    /// <remarks>
    /// It is one <c>System.Array.Copy</c> over the bytes rather than an element loop because the
    /// two ranges may overlap and the specification requires the SOURCE values, not the ones a
    /// forward loop would have already overwritten. <c>Array.Copy</c> promises exactly that.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=973EFC
    // Broiler-Human:        PENDING
    internal bool TryCopyWithin(int to, int from, int count)
    {
        var bytes = Buffer.Data;

        if (bytes is null || count <= 0)
        {
            return bytes is not null;
        }

        var width = BytesPerElement;

        System.Array.Copy(
            bytes, ByteOffset + (from * width), bytes, ByteOffset + (to * width), count * width);

        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=8B2240
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        if (IsArrayIndex(key, out var at))
        {
            if (at < (uint)Length && TryReadAt((int)at, out var element))
            {
                property = JsProperty.Data(JsValue.Number(element), JsPropertyAttributes.Default);
                return true;
            }

            // ABSENT, AND THE OWN-PROPERTY SEARCH STOPS HERE. Falling through would let the
            // ordinary map answer for a slot that is out of the view, which is the whole reason an
            // integer-indexed object is exotic rather than ordinary. What this cannot stop is the
            // engine's walk up the prototype chain; the type's remarks say so.
            property = default;
            return false;
        }

        if (Buffer.IsDetached)
        {
            property = default;
            return false;
        }

        return base.TryGetOwnProperty(key, out property);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=07CA90
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property)
    {
        if (IsArrayIndex(key, out var at))
        {
            // An accessor cannot be installed over an element and an out-of-range write is not an
            // error: both are DISCARDED. Creating an ordinary property for either would make the
            // object claim an index its buffer has no bytes for.
            if (!property.IsAccessor && at < (uint)Length)
            {
                _ = TryWriteAt((int)at, JsElements.NumberOf(property.Value));
            }

            return;
        }

        base.SetOwnProperty(key, property);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9462D1
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        if (IsArrayIndex(key, out var at))
        {
            // An element in the view refuses to be deleted - there is no hole a typed array could
            // become - and an index outside it answers true because deleting what is not there
            // succeeds.
            return Buffer.IsDetached || at >= (uint)Length;
        }

        return base.DeleteOwnProperty(key);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=08F718
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>();

        if (Buffer.IsDetached)
        {
            return names;
        }

        for (var at = 0; at < Length; at++)
        {
            names.Add(JsNumberFormat.ToUintString((uint)at));
        }

        // The indices are already in ascending order and no index key can be in the ordinary map -
        // SetOwnProperty routes every canonical numeric key to an element - so the stray list this
        // collects into is always empty and nothing needs sorting afterwards.
        var stray = new System.Collections.Generic.List<string>();
        CollectOwnNames(stray, names);
        return names;
    }
}
