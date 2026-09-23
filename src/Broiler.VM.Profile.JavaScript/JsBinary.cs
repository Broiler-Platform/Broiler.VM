// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   53
// Annotated:        53/53
// Exempt:           27
// Human-reviewed:   0/53
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  4/10 max
// Unverified:       53
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The twelve element types this realm's typed arrays store.</summary>
/// <remarks>
/// <para>
/// <b>Twelve since 2026-09-22 (JSeal B07); it was ten.</b> <c>BigInt64Array</c> and
/// <c>BigUint64Array</c> were absent while the realm had no BigInt: their elements read as a
/// BigInt, and a realm without one could only have answered a Number and lost the top bits the
/// kinds exist to carry. BigInt was admitted by card B05 (decision JSD-0033 section 7), so the two
/// kinds exist now - and exist only in a realm whose composition admits BIGINT as well as the
/// binary surface, because reading an element makes a BigInt value. Their elements move as a
/// <see cref="JsValue"/> holding a BigInt and never as a <c>double</c>: the Number path refuses
/// them by name (<see cref="JsElements.Read"/>) rather than answering a rounded value.
/// <i>(The superseded paragraph read "Ten and not twelve ... this realm has no BigInt"; it stopped
/// being true with B05 and is quoted rather than deleted.)</i>
/// </para>
/// <para>
/// <b><c>Uint8Clamped</c> is a kind and not a flag on <c>Uint8</c>.</b> The two read identically -
/// one byte, zero to 255 - and differ only in how a write converts, which is exactly the shape a
/// separate kind expresses without a second field that every read would have to ignore.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D0977E
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

    /// <summary>
    /// An IEEE 754 binary16, which a write rounds to through <see cref="JsFloat16"/> and a read
    /// widens from exactly.
    /// </summary>
    /// <remarks>
    /// It is numbered after <see cref="Float64"/> rather than beside <see cref="Float32"/> so that
    /// the value of every kind that existed before it is unchanged; nothing orders the kinds by
    /// their value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AA615C
    // Broiler-Human:        PENDING
    Float16 = 9,

    /// <summary>
    /// A signed 64-bit integer whose element is a BigInt, written by the language's
    /// <c>ToBigInt64</c> (the value modulo 2**64, read back as two's complement).
    /// </summary>
    /// <remarks>
    /// Numbered after every Number kind for the reason <see cref="Float16"/> is: no existing kind's
    /// value moves.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=425E85
    // Broiler-Human:        PENDING
    BigInt64 = 10,

    /// <summary>
    /// An unsigned 64-bit integer whose element is a BigInt, written by <c>ToBigUint64</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=191110
    // Broiler-Human:        PENDING
    BigUint64 = 11,
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
    /// <summary>
    /// The twelve kinds, in the order the constructors are built in: the specification's table of
    /// element types.
    /// </summary>
    /// <remarks>
    /// A realm whose composition declines BigInt builds the ten Number kinds only; see
    /// <see cref="HoldsBigInts"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DE0044
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
        JsElementKind.BigInt64,
        JsElementKind.BigUint64,
        JsElementKind.Float16,
        JsElementKind.Float32,
        JsElementKind.Float64,
    ];

    /// <summary>How many bytes one element of <paramref name="kind"/> occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=85806A
    // Broiler-Human:        PENDING
    internal static int WidthOf(JsElementKind kind) => kind switch
    {
        JsElementKind.Int8 or JsElementKind.Uint8 or JsElementKind.Uint8Clamped => 1,
        JsElementKind.Int16 or JsElementKind.Uint16 or JsElementKind.Float16 => 2,
        JsElementKind.Float64 or JsElementKind.BigInt64 or JsElementKind.BigUint64 => 8,
        _ => 4,
    };

    /// <summary>
    /// Whether <paramref name="kind"/>'s content type is BigInt rather than Number: the
    /// specification's <c>[[ContentType]]</c>, which species results must match.
    /// </summary>
    /// <remarks>
    /// <b>The two 64-bit integer kinds answer <see langword="true"/> and the ten Number kinds
    /// <see langword="false"/>, and each is named.</b> (Since 2026-09-22, JSeal B07; until then every
    /// kind answered false and the question was asked so that the species checks were already
    /// written against the content type.) Every conversion into an element, every species result and
    /// every copy between two views is decided by this answer: a BigInt kind converts with
    /// <c>ToBigInt</c>, a Number kind with <c>ToNumber</c>, and a copy between the two is a
    /// <c>TypeError</c>. A kind this switch does not name is refused rather than guessed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3E79B8
    // Broiler-Human:        PENDING
    internal static bool HoldsBigInts(JsElementKind kind) => kind switch
    {
        JsElementKind.Int8 or JsElementKind.Uint8 or JsElementKind.Uint8Clamped or
        JsElementKind.Int16 or JsElementKind.Uint16 or JsElementKind.Int32 or
        JsElementKind.Uint32 or JsElementKind.Float16 or JsElementKind.Float32 or
        JsElementKind.Float64 => false,
        JsElementKind.BigInt64 or JsElementKind.BigUint64 => true,
        _ => throw new System.ArgumentOutOfRangeException(nameof(kind)),
    };

    /// <summary>The global name the constructor of <paramref name="kind"/> is bound to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B80BCE
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
        JsElementKind.Float16 => "Float16Array",
        JsElementKind.Float32 => "Float32Array",
        JsElementKind.BigInt64 => "BigInt64Array",
        JsElementKind.BigUint64 => "BigUint64Array",
        _ => "Float64Array",
    };

    /// <summary>Reads one Number element out of <paramref name="bytes"/> at a byte offset.</summary>
    /// <remarks>
    /// <para>
    /// A NaN read out of a <see cref="JsElementKind.Float32"/> or
    /// <see cref="JsElementKind.Float64"/> slot stays a NaN, and a negative zero stays negative:
    /// the bytes are widened, never normalised, so <c>Object.is(-0, new Float64Array([-0])[0])</c>
    /// is true.
    /// </para>
    /// <para>
    /// <b>A BigInt kind is refused here by name</b> (since JSeal B07): its element is a BigInt, and
    /// answering it as a <c>double</c> would round away the bits the kind exists to keep. Those
    /// kinds are read through <see cref="ReadValue"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B71892
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
            JsElementKind.Float16 => JsFloat16.Decode(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(span)),
            JsElementKind.Float32 => (double)(littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadSingleLittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadSingleBigEndian(span)),
            JsElementKind.BigInt64 or JsElementKind.BigUint64 => throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a BigInt typed-array element was read through the Number path"),
            _ => littleEndian
                ? System.Buffers.Binary.BinaryPrimitives.ReadDoubleLittleEndian(span)
                : System.Buffers.Binary.BinaryPrimitives.ReadDoubleBigEndian(span),
        };
    }

    /// <summary>Writes one Number element into <paramref name="bytes"/> at a byte offset.</summary>
    /// <remarks>
    /// A BigInt kind is refused by name, as <see cref="Read"/> refuses it; see
    /// <see cref="WriteValue"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=72BBCA
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

            case JsElementKind.Float16:
            {
                var encoded = JsFloat16.Encode(value);

                if (littleEndian)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(span, encoded);
                }
                else
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(span, encoded);
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

            case JsElementKind.BigInt64:
            case JsElementKind.BigUint64:
                throw new JsAbort(
                    JsAbortKind.InternalDefect,
                    "a Number was written into a BigInt typed-array element");

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
    /// Reads one element of any kind: a Number for the ten Number kinds and a BigInt for the two
    /// 64-bit integer kinds - the specification's <c>GetValueFromBuffer</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BAB170
    // Broiler-Falsified-If: a BigInt64 element reads as anything but the two's-complement integer of its eight bytes, a BigUint64 element as anything but their unsigned integer, or either as a Number
    // Broiler-Human:        PENDING
    internal static JsValue ReadValue(byte[] bytes, int at, JsElementKind kind, bool littleEndian)
    {
        if (!HoldsBigInts(kind))
        {
            return JsValue.Number(Read(bytes, at, kind, littleEndian));
        }

        var span = new System.ReadOnlySpan<byte>(bytes, at, 8);
        var raw = littleEndian
            ? System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(span)
            : System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(span);

        // THE SIGN IS THE READ'S, NOT THE BYTES': the same eight bytes are -1n through a
        // BigInt64Array and 2n ** 64n - 1n through a BigUint64Array.
        var integer = kind == JsElementKind.BigInt64
            ? new System.Numerics.BigInteger(unchecked((long)raw))
            : new System.Numerics.BigInteger(raw);

        return JsValue.BigInt(new JsBigInt(integer));
    }

    /// <summary>
    /// Writes one element of any kind from a value already converted to the kind's content type:
    /// the specification's <c>SetValueInBuffer</c>.
    /// </summary>
    /// <remarks>
    /// <b>The conversion is the caller's and has already happened</b> - <c>ToNumber</c> or
    /// <c>ToBigInt</c>, each of which can run guest code and throw - so a value of the wrong type
    /// here is a defect in a built-in and not a guest error: it ends the invocation by name rather
    /// than storing a value converted a second, silent way.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D893D2
    // Broiler-Falsified-If: a BigInt element stores anything but its value modulo 2**64, or a value of the other content type is stored rather than refused
    // Broiler-Human:        PENDING
    internal static void WriteValue(
        byte[] bytes, int at, JsElementKind kind, JsValue value, bool littleEndian)
    {
        if (!HoldsBigInts(kind))
        {
            if (!value.IsNumber)
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect,
                    "an unconverted value reached a Number typed-array element write");
            }

            Write(bytes, at, kind, value.AsNumber(), littleEndian);
            return;
        }

        if (!value.IsBigInt)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a value that is not a BigInt reached a BigInt typed-array element write");
        }

        // ONE WRITE SERVES BOTH SIGNS, as it does for Int32 and Uint32: ToBigInt64 and ToBigUint64
        // differ only in how the result is READ, and the bits either stores are the value modulo
        // 2**64.
        var span = new System.Span<byte>(bytes, at, 8);
        var low = Low64(value.AsBigInt().Value);

        if (littleEndian)
        {
            System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(span, low);
        }
        else
        {
            System.Buffers.Binary.BinaryPrimitives.WriteUInt64BigEndian(span, low);
        }
    }

    /// <summary>
    /// The low 64 bits of <paramref name="value"/> in two's complement: <paramref name="value"/>
    /// modulo 2**64.
    /// </summary>
    /// <remarks>
    /// A value that fits a <c>long</c> or a <c>ulong</c> converts directly; anything wider is masked,
    /// which reads every word of it. The engine's element conversion narrows a wide value ONCE,
    /// charged, with <see cref="Narrow"/>, so a write repeated over many elements - <c>fill</c> -
    /// never masks a wide value more than once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=33AFD0
    // Broiler-Falsified-If: the answer differs from the value modulo 2**64 for any integer, negative ones included
    // Broiler-Human:        PENDING
    internal static ulong Low64(System.Numerics.BigInteger value)
    {
        if (value >= long.MinValue && value <= long.MaxValue)
        {
            return unchecked((ulong)(long)value);
        }

        if (value.Sign > 0 && value <= ulong.MaxValue)
        {
            return (ulong)value;
        }

        // BigInteger's AND IS TWO'S COMPLEMENT WITH AN INFINITE SIGN, so a negative value masks to
        // exactly its residue modulo 2**64.
        return (ulong)(value & ulong.MaxValue);
    }

    /// <summary>
    /// <paramref name="value"/> reduced to what an element of <paramref name="kind"/> would read
    /// back: <c>BigInt.asIntN(64, v)</c> or <c>BigInt.asUintN(64, v)</c>.
    /// </summary>
    /// <remarks>
    /// Storing the narrowed value writes exactly the bytes storing the original would, so a caller
    /// that converts once and writes many times narrows once. A value already in range is answered
    /// as itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=25C1DD
    // Broiler-Human:        PENDING
    internal static JsBigInt Narrow(JsElementKind kind, JsBigInt value)
    {
        var low = Low64(value.Value);
        var narrowed = kind == JsElementKind.BigInt64
            ? new System.Numerics.BigInteger(unchecked((long)low))
            : new System.Numerics.BigInteger(low);

        return narrowed == value.Value ? value : new JsBigInt(narrowed);
    }

    /// <summary>
    /// The value an element write of <paramref name="kind"/> stores when the object model has no
    /// engine to convert with: <see cref="NumberOf"/> for a Number kind, the BigInt itself for a
    /// BigInt kind.
    /// </summary>
    /// <remarks>
    /// <b>A BigInt kind converts a BigInt and a Boolean exactly and refuses everything else by
    /// name.</b> <c>ToBigInt</c> of a Number, <c>undefined</c>, <c>null</c> or a Symbol is a
    /// <c>TypeError</c>, of a String a parse whose cost is the String's, and of an object a call;
    /// none of those can happen on this path, which has no frame, no fuel and nowhere to put an
    /// exception. Every guest-reachable element write is meant to convert through the engine first
    /// (<c>JsEngine.ToElementValue</c>), so reaching this with one of them is a defect in an
    /// internal caller, and the invocation is ended rather than a value invented. That is a
    /// property of the callers, not of this method: <c>JSON.parse</c>'s reviver stored through
    /// <see cref="JsObject.SetOwnProperty"/> and reached here until the B07 review (2026-09-22),
    /// and the audit then made covered the <c>SetOwnProperty</c> call sites whose target a guest
    /// can choose, not every conceivable future one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=50F640
    // Broiler-Falsified-If: a BigInt kind stores a value converted from a Number, a String or an object on this engine-less path
    // Broiler-Human:        PENDING
    internal static JsValue ElementOf(JsElementKind kind, JsValue value)
    {
        if (!HoldsBigInts(kind))
        {
            return JsValue.Number(NumberOf(value));
        }

        return value.Type switch
        {
            JsType.BigInt => value,
            JsType.Boolean => JsValue.BigInt(value.AsBoolean() ? JsBigInt.One : JsBigInt.Zero),
            _ => throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a value that is not a BigInt reached a BigInt typed-array element write with no engine to convert it"),
        };
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CC6ABD
    // Broiler-Human:        PENDING
    internal static double NumberOf(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.String => JsNumberFormat.ToNumber(value.AsString()),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Null => 0,

        // A BIGINT IS NOT A NUMBER AND THIS PATH CANNOT THROW THE TypeError `ToNumber` OWES IT. The
        // guest's element writes all convert through the engine first and refuse there; reaching
        // this with one would be an internal path writing a BigInt, and NaN would be the silent
        // Number conversion decision JSD-0033 forbids - so the invocation is ended by name instead.
        JsType.BigInt => throw new JsAbort(
            JsAbortKind.InternalDefect,
            "a BigInt reached a typed-array element write that has no engine to refuse it"),
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

/// <summary>
/// The one conversion between a Number and an IEEE 754 binary16, which <c>Math.f16round</c>,
/// <c>DataView.prototype.getFloat16</c>/<c>setFloat16</c> and <c>Float16Array</c> all go through.
/// </summary>
/// <remarks>
/// <para>
/// <b>The narrowing is the platform's <c>(System.Half)double</c>, and that is a verified claim
/// rather than an assumption.</b> The language rounds a Number to binary16 ONCE, directly, ties to
/// even; a conversion that went through <c>float</c> first would round twice and answer wrongly
/// for a double just above a binary16 midpoint - <c>1 + 2**-11 + 2**-40</c> becomes the tie
/// <c>1 + 2**-11</c> in binary32 and then 1, where the correct answer is <c>1 + 2**-10</c>. The
/// runtime this profile targets converts from the double directly; it was checked on 2026-09-21
/// against an integer-arithmetic reference over every binary16 value, every midpoint between two
/// neighbours, the doubles either side of each midpoint and random points between neighbours, with
/// no disagreement, and the probe <c>src/tests/differential/the-float16-surface.js</c> repeats the
/// boundary sweep in the guest on every run. Should a runtime change that, the probe is what fails.
/// </para>
/// <para>
/// <b>What the rounding answers at the edges, all of which is IEEE 754's and the language's.</b>
/// A magnitude at or above 65520 - the midpoint between the largest finite value 65504 and the
/// next power of two - becomes an infinity of the same sign; a magnitude at or below 2**-25 - half
/// the smallest subnormal - becomes a zero of the same sign, so a negative number that underflows
/// is <c>-0</c>; subnormals are kept rather than flushed; infinities and signed zeros are exact.
/// </para>
/// <para>
/// <b>Every NaN is written as the one quiet NaN <c>0x7E00</c>.</b> The specification lets an
/// implementation choose which NaN encoding a write stores, and choosing one makes the bytes a
/// property of the profile rather than of which NaN the guest happened to compute. A read is
/// exact in the other direction: every binary16 value, NaN included, widens to a double without
/// rounding.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=99D5EC
// Broiler-Human:        PENDING
internal static class JsFloat16
{
    /// <summary>The quiet NaN every NaN is stored as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0C7512
    // Broiler-Human:        PENDING
    private const ushort QuietNaN = 0x7E00;

    /// <summary>The binary16 bits <paramref name="value"/> rounds to, ties to even.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CE0A76
    // Broiler-Human:        PENDING
    internal static ushort Encode(double value) =>
        double.IsNaN(value)
            ? QuietNaN
            : System.BitConverter.HalfToUInt16Bits((System.Half)value);

    /// <summary>The Number the binary16 <paramref name="bits"/> denote, which is exact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=8CD7DB
    // Broiler-Human:        PENDING
    internal static double Decode(ushort bits) =>
        (double)System.BitConverter.UInt16BitsToHalf(bits);

    /// <summary>
    /// <paramref name="value"/> rounded to the nearest binary16 and widened back: <c>Math.f16round</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3640E7
    // Broiler-Human:        PENDING
    internal static double Round(double value) => Decode(Encode(value));
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
/// <para>
/// <b>A resizable buffer REPLACES its array on every resize, and <c>Data.Length</c> is always the
/// byte length.</b> (Added 2026-09-21, JSeal F04.) The specification describes a resize as a new
/// data block the old bytes are copied into, and this is that literally: no spare capacity is
/// reserved up to <see cref="MaxByteLength"/>, so the invariant every reader of <see cref="Data"/>
/// relied on before resizing existed - the array's length IS the buffer's length - still holds,
/// and a fixed-length buffer pays nothing for the feature. What it costs is that an array read out
/// of <see cref="Data"/> is only the buffer's storage until the next resize, exactly as it was only
/// the storage until the next detach; so no code may hold one across anything that can run guest
/// code. Every built-in re-reads it after each such step, and the host surface copies out of it
/// within a single crossing in which no guest code runs.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=37B22E
// Broiler-Human:        PENDING
internal sealed class JsArrayBuffer : JsObject
{
    /// <summary>Creates a zero-filled buffer of <paramref name="byteLength"/> bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E88D9A
    // Broiler-Human:        PENDING
    internal JsArrayBuffer(JsObject? prototype, int byteLength)
        : base(prototype, "ArrayBuffer")
    {
        Data = new byte[byteLength];
        RetainedByteLength = byteLength;
    }

    /// <summary>
    /// Creates a zero-filled RESIZABLE buffer of <paramref name="byteLength"/> bytes that may grow
    /// to <paramref name="maxByteLength"/>.
    /// </summary>
    /// <remarks>
    /// The caller has already refused a length above the maximum and a maximum this runtime could
    /// never allocate; the constructor does not repeat either check, because the error each one
    /// raises is the built-in's to choose.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5A8F54
    // Broiler-Human:        PENDING
    internal JsArrayBuffer(JsObject? prototype, int byteLength, int maxByteLength)
        : this(prototype, byteLength) => MaxByteLength = maxByteLength;

    /// <summary>The bytes, or <see langword="null"/> once the buffer has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=18F504
    // Broiler-Human:        PENDING
    internal byte[]? Data { get; private set; }

    /// <summary>
    /// The specification's <c>[[ArrayBufferMaxByteLength]]</c>, or <see langword="null"/> for a
    /// fixed-length buffer.
    /// </summary>
    /// <remarks>
    /// It survives a detach, as the specification's slot does: a detached resizable buffer is
    /// still resizable - <c>resizable</c> answers true - and only its bytes and length are gone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B169FC
    // Broiler-Human:        PENDING
    internal int? MaxByteLength { get; }

    /// <summary>Whether this buffer was built resizable: <c>IsFixedLengthArrayBuffer</c> negated.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=47D067
    // Broiler-Human:        PENDING
    internal bool IsResizable => MaxByteLength is not null;

    /// <summary>
    /// The most bytes this buffer has ever reported to the <c>LiveBytes</c> ceiling: its length at
    /// creation, raised by every resize past it.
    /// </summary>
    /// <remarks>
    /// <b>A shrink gives nothing back and a regrowth up to this mark charges nothing again.</b> No
    /// allocation in this realm is released to the ceiling when the collector reclaims it, because
    /// the meter cannot see the collector; a buffer that shrinks and grows is held to the same rule,
    /// and charging it only past its own high-water mark is what keeps a loop of
    /// <c>resize(max)</c> and <c>resize(0)</c> at a cost of <c>max</c> bytes rather than an
    /// unbounded sum.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E303CF
    // Broiler-Human:        PENDING
    internal int RetainedByteLength { get; private set; }

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
    /// Makes <paramref name="storage"/> this buffer's bytes: the internal half of
    /// <c>ArrayBuffer.prototype.resize</c>, after the caller has validated and paid for it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The new array is allocated by the caller, so a failure leaves nothing to undo.</b> A
    /// refused fuel or live-bytes charge, or an allocation the runtime cannot make, all happen
    /// before this is called, while <see cref="Data"/> is still the old array; this method itself
    /// only copies and assigns, and cannot fail once its preconditions hold.
    /// </para>
    /// <para>
    /// <b>The common prefix is copied and the rest of a larger array is already zero</b>, which is
    /// what the specification's fresh data block holds, so a grow exposes zeroes and a shrink
    /// followed by a grow does not bring the discarded bytes back. The old array is dropped here and
    /// never reached again by the buffer; a view reads through the buffer on every access and so
    /// never holds it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B340C3
    // Broiler-Falsified-If: a resize is committed on a fixed-length or detached buffer, past the maximum, or loses a byte of the common prefix or exposes a stale byte past it
    // Broiler-Human:        PENDING
    internal bool TryReplaceStorage(byte[] storage)
    {
        var bytes = Data;

        if (bytes is null || MaxByteLength is not { } max || storage.Length > max)
        {
            return false;
        }

        System.Array.Copy(bytes, storage, System.Math.Min(bytes.Length, storage.Length));
        Data = storage;

        if (storage.Length > RetainedByteLength)
        {
            RetainedByteLength = storage.Length;
        }

        return true;
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
/// <para>
/// <b>An omitted length over a resizable buffer TRACKS the buffer.</b> (Added 2026-09-21, JSeal
/// F05.) The specification's <c>[[ByteLength]]</c> is then <c>auto</c>, and the view's length is
/// whatever the buffer holds past its offset at the moment it is asked. A view with a length it
/// was given keeps that length and is <i>out of bounds</i> - every access a <c>TypeError</c>, its
/// <c>byteLength</c> and <c>byteOffset</c> getters too - while the buffer is shorter than its end,
/// and in bounds again, over the buffer's current bytes, once the buffer grows back.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AAD290
// Broiler-Human:        PENDING
internal sealed class JsDataView : JsObject
{
    /// <summary>The length the view was given, which <see cref="TracksLength"/> makes meaningless.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=58EDF9
    // Broiler-Human:        PENDING
    private readonly int fixedByteLength;

    /// <summary>
    /// Creates a view over part of <paramref name="buffer"/>; a <see langword="null"/>
    /// <paramref name="byteLength"/> is the specification's <c>auto</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6D9AFE
    // Broiler-Human:        PENDING
    internal JsDataView(JsObject? prototype, JsArrayBuffer buffer, int byteOffset, int? byteLength)
        : base(prototype, "DataView")
    {
        Buffer = buffer;
        ByteOffset = byteOffset;
        TracksLength = byteLength is null;
        fixedByteLength = byteLength ?? 0;
    }

    /// <summary>Whether the view's length is the buffer's: <c>[[ByteLength]]</c> is <c>auto</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BE926C
    // Broiler-Human:        PENDING
    internal bool TracksLength { get; }

    /// <summary>
    /// The specification's <c>IsViewOutOfBounds</c>, against the buffer as it is now: detached, or
    /// shorter than the view's offset or its fixed end.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=850EA8
    // Broiler-Human:        PENDING
    internal bool IsOutOfBounds
    {
        get
        {
            var bytes = Buffer.Data;

            if (bytes is null || ByteOffset > bytes.Length)
            {
                return true;
            }

            return !TracksLength && (long)ByteOffset + fixedByteLength > bytes.Length;
        }
    }

    /// <summary>The buffer the view reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=239486
    // Broiler-Human:        PENDING
    internal JsArrayBuffer Buffer { get; }

    /// <summary>Where in the buffer the view starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=04F8CE
    // Broiler-Human:        PENDING
    internal int ByteOffset { get; }

    /// <summary>
    /// How many bytes the view spans now: the specification's <c>GetViewByteLength</c>, and zero
    /// while the view is out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D37918
    // Broiler-Human:        PENDING
    internal int ByteLength
    {
        get
        {
            if (IsOutOfBounds)
            {
                return 0;
            }

            return TracksLength ? Buffer.ByteLength - ByteOffset : fixedByteLength;
        }
    }

    /// <summary>Whether the buffer under the view has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D18ED
    // Broiler-Human:        PENDING
    internal bool IsDetached => Buffer.IsDetached;

    /// <summary>
    /// Reads one element, answering <see langword="false"/> when it is not there; the value is a
    /// Number, or a BigInt for a BigInt kind.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=34664B
    // Broiler-Human:        PENDING
    internal bool TryRead(JsElementKind kind, int at, bool littleEndian, out JsValue value)
    {
        var bytes = Buffer.Data;
        var width = JsElements.WidthOf(kind);

        if (bytes is null || IsOutOfBounds || at < 0 || at > ByteLength - width ||
            ByteOffset + at + width > bytes.Length)
        {
            value = JsValue.Undefined;
            return false;
        }

        value = JsElements.ReadValue(bytes, ByteOffset + at, kind, littleEndian);
        return true;
    }

    /// <summary>
    /// Writes one element already converted to the kind's content type, answering
    /// <see langword="false"/> when it does not fit.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9F2728
    // Broiler-Human:        PENDING
    internal bool TryWrite(JsElementKind kind, int at, JsValue value, bool littleEndian)
    {
        var bytes = Buffer.Data;
        var width = JsElements.WidthOf(kind);

        if (bytes is null || IsOutOfBounds || at < 0 || at > ByteLength - width ||
            ByteOffset + at + width > bytes.Length)
        {
            return false;
        }

        JsElements.WriteValue(bytes, ByteOffset + at, kind, value, littleEndian);
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
/// <b>A detached buffer empties the indices and nothing else.</b> Every integer-indexed internal
/// method the specification writes branches on whether the key is numeric first, and only that
/// branch consults the buffer; every other string key, and every symbol, is an ordinary property
/// that is written, read, listed and deleted exactly as it was before the detach.
/// <i>(Corrected 2026-09-21, JSeal follow-up VM-FIX-A. This paragraph read "A detached buffer
/// makes the object answer empty for every key, not only for the indices", and called what that
/// cost "one observable case - a stray expando on a typed array whose buffer was then detached".
/// It was not one case: the write still reached the ordinary map while every read hid it, so
/// <c>a.foo = 1</c> and <c>a.constructor = X</c> on a detached view were silently lost, and a
/// species lookup that should have seen the assigned constructor read the prototype's instead.
/// The superseded reading is quoted rather than deleted because the trade it described was never
/// the one the code made.)</i>
/// </para>
/// <para>
/// <b>The bytes are little-endian on every host.</b> The platform order is a declared property of
/// this profile rather than of the machine it runs on; see <see cref="JsElements"/>.
/// </para>
/// <para>
/// <b><see cref="Length"/> is computed from the buffer on every read, never cached.</b> (Changed
/// 2026-09-21, JSeal F05; it was a field fixed at construction.) Over a resizable buffer a view
/// built without a length TRACKS the buffer - its length is however many whole elements lie past
/// its offset now - and a view built with one is <i>out of bounds</i> whenever the buffer is
/// shorter than its end, answering zero for its length and absent for every index, until the
/// buffer grows back and the same view reads the buffer's current bytes again. Detachment is the
/// same state reached a different way. Every member that asked "is it detached" of the view for
/// the answer "are there elements" now asks <see cref="IsOutOfBounds"/>, which says both.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=70D36A
// Broiler-Human:        PENDING
internal sealed class JsTypedArray : JsObject
{
    /// <summary>The element count the view was given, which <see cref="TracksLength"/> makes meaningless.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F54BE1
    // Broiler-Human:        PENDING
    private readonly int fixedLength;

    /// <summary>
    /// Creates a view of <paramref name="length"/> elements over a buffer; a
    /// <see langword="null"/> length is the specification's <c>auto</c>, which only a view over a
    /// resizable buffer is given.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=AC4836
    // Broiler-Human:        PENDING
    internal JsTypedArray(
        JsObject? prototype, JsArrayBuffer buffer, int byteOffset, int? length, JsElementKind kind)
        : base(prototype, JsElements.ConstructorNameOf(kind))
    {
        Buffer = buffer;
        ByteOffset = byteOffset;
        TracksLength = length is null;
        fixedLength = length ?? 0;
        Kind = kind;
    }

    /// <summary>Whether the view's length is the buffer's: <c>[[ArrayLength]]</c> is <c>auto</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BE926C
    // Broiler-Human:        PENDING
    internal bool TracksLength { get; }

    /// <summary>
    /// The specification's <c>IsTypedArrayFixedLength</c>: a view with a length of its own over a
    /// buffer that cannot be resized.
    /// </summary>
    /// <remarks>
    /// A fixed-length view over a RESIZABLE buffer is not fixed-length in this sense: its length is
    /// its own, but the buffer can shrink under it and take every element away, and grow again and
    /// bring them back.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C166A0
    // Broiler-Human:        PENDING
    internal bool IsFixedLength => !TracksLength && !Buffer.IsResizable;

    /// <summary>
    /// The typed array's own <c>[[PreventExtensions]]</c>: refused - <see langword="false"/> - for
    /// a view that is not <see cref="IsFixedLength"/>, and otherwise
    /// <c>OrdinaryPreventExtensions</c>.
    /// </summary>
    /// <remarks>
    /// <b>The refusal keeps an essential invariant, not a convenience.</b> A view over a resizable
    /// buffer gains an integer-indexed own property whenever the buffer grows, and a non-extensible
    /// object may never gain one; the language therefore makes such a view impossible to close, so
    /// <c>Object.preventExtensions</c>, <c>Object.seal</c> and <c>Object.freeze</c> throw on it and
    /// <c>Reflect.preventExtensions</c> answers false. (Added 2026-09-21, JSeal F05-F06 review.)
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FFD0A4
    // Broiler-Human:        PENDING
    internal bool PreventExtensions()
    {
        if (!IsFixedLength)
        {
            return false;
        }

        Extensible = false;
        return true;
    }

    /// <summary>
    /// The specification's <c>IsTypedArrayOutOfBounds</c>, against the buffer as it is now:
    /// detached, or shorter than the view's offset or its fixed end.
    /// </summary>
    /// <remarks>
    /// A view whose offset is exactly the buffer's length is IN bounds with no elements, which is
    /// the specification's own note; only an offset past the end is out.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FAE511
    // Broiler-Human:        PENDING
    internal bool IsOutOfBounds
    {
        get
        {
            var bytes = Buffer.Data;

            if (bytes is null || ByteOffset > bytes.Length)
            {
                return true;
            }

            return !TracksLength &&
                (long)ByteOffset + ((long)fixedLength * BytesPerElement) > bytes.Length;
        }
    }

    /// <summary>The buffer the elements live in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=239486
    // Broiler-Human:        PENDING
    internal JsArrayBuffer Buffer { get; }

    /// <summary>Where in the buffer element zero starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=04F8CE
    // Broiler-Human:        PENDING
    internal int ByteOffset { get; }

    /// <summary>
    /// How many elements the view spans now: the specification's <c>TypedArrayLength</c>, and zero
    /// while the view is out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=33198A
    // Broiler-Human:        PENDING
    internal int Length
    {
        get
        {
            if (IsOutOfBounds)
            {
                return 0;
            }

            return TracksLength ? (Buffer.ByteLength - ByteOffset) / BytesPerElement : fixedLength;
        }
    }

    /// <summary>What kind of element the view reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A3DBEE
    // Broiler-Human:        PENDING
    internal JsElementKind Kind { get; }

    /// <summary>How wide one element is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=144D95
    // Broiler-Human:        PENDING
    internal int BytesPerElement => JsElements.WidthOf(Kind);

    /// <summary>How many bytes the view spans; zero while it is out of bounds or detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6B8EA8
    // Broiler-Human:        PENDING
    internal int ByteLength => Length * BytesPerElement;

    /// <summary>Whether the buffer under the view has been detached.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D18ED
    // Broiler-Human:        PENDING
    internal bool IsDetached => Buffer.IsDetached;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0B2E51
    // Broiler-Human:        PENDING
    internal override int OwnPropertyCount => Length + base.OwnPropertyCount;

    /// <summary>
    /// Whether the view's elements are BigInts: its <c>[[ContentType]]</c> is BigInt.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=133E22
    // Broiler-Human:        PENDING
    internal bool HoldsBigInts => JsElements.HoldsBigInts(Kind);

    /// <summary>
    /// Reads element <paramref name="at"/> - a Number, or a BigInt for a BigInt kind - or answers
    /// that there is none.
    /// </summary>
    /// <remarks>
    /// <b>The element moves as a value and not as a <c>double</c></b> (since 2026-09-22, JSeal B07):
    /// a copy between two views of one content type carries a BigInt element exactly, where a
    /// <c>double</c> would have rounded it past 2**53.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=27C5B4
    // Broiler-Human:        PENDING
    internal bool TryReadAt(int at, out JsValue value)
    {
        var bytes = Buffer.Data;

        if (bytes is null || at < 0 || at >= Length)
        {
            value = JsValue.Undefined;
            return false;
        }

        value = JsElements.ReadValue(bytes, ByteOffset + (at * BytesPerElement), Kind, true);
        return true;
    }

    /// <summary>
    /// Writes element <paramref name="at"/> from a value already converted to the view's content
    /// type, or answers that there is nowhere to.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A81EEC
    // Broiler-Human:        PENDING
    internal bool TryWriteAt(int at, JsValue value)
    {
        var bytes = Buffer.Data;

        if (bytes is null || at < 0 || at >= Length)
        {
            return false;
        }

        JsElements.WriteValue(bytes, ByteOffset + (at * BytesPerElement), Kind, value, true);
        return true;
    }

    /// <summary>Element <paramref name="at"/> as a value, which is <c>undefined</c> when absent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=2B5244
    // Broiler-Human:        PENDING
    internal JsValue ElementAt(int at) =>
        TryReadAt(at, out var value) ? value : JsValue.Undefined;

    /// <summary>Moves <paramref name="count"/> elements inside this view.</summary>
    /// <remarks>
    /// It is one <c>System.Array.Copy</c> over the bytes rather than an element loop because the
    /// two ranges may overlap and the specification requires the SOURCE values, not the ones a
    /// forward loop would have already overwritten. <c>Array.Copy</c> promises exactly that.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5252FD
    // Broiler-Human:        PENDING
    internal bool TryCopyWithin(int to, int from, int count)
    {
        var bytes = Buffer.Data;

        if (bytes is null || count <= 0)
        {
            return bytes is not null;
        }

        var width = BytesPerElement;

        // THE RANGES ARE CHECKED AGAINST THE BUFFER AS IT IS NOW. The caller clamps the count to
        // the view's current length, as the specification does after its coercions; this is the
        // backstop that makes a caller's mistake a refusal rather than a copy through bytes a
        // shrink has already taken away.
        if (from < 0 || to < 0 ||
            (long)ByteOffset + (((long)System.Math.Max(from, to) + count) * width) > bytes.Length)
        {
            return false;
        }

        System.Array.Copy(
            bytes, ByteOffset + (from * width), bytes, ByteOffset + (to * width), count * width);

        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3A0090
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        if (IsArrayIndex(key, out var at))
        {
            if (at < (uint)Length && TryReadAt((int)at, out var element))
            {
                property = JsProperty.Data(element, JsPropertyAttributes.Default);
                return true;
            }

            // ABSENT, AND THE OWN-PROPERTY SEARCH STOPS HERE. Falling through would let the
            // ordinary map answer for a slot that is out of the view, which is the whole reason an
            // integer-indexed object is exotic rather than ordinary. What this cannot stop is the
            // engine's walk up the prototype chain; the type's remarks say so.
            property = default;
            return false;
        }

        // A NUMERIC KEY THAT IS NOT AN INDEX - "-0", "1.5", "-1", "Infinity" - names no element
        // either, and it is absent for the same reason: the ordinary map may not answer for it.
        if (IsNumericKey(key))
        {
            property = default;
            return false;
        }

        return base.TryGetOwnProperty(key, out property);
    }

    /// <summary>
    /// Whether <paramref name="key"/> is numeric in the specification's sense: its
    /// <c>CanonicalNumericIndexString</c> is not <c>undefined</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every such key belongs to the integer-indexed branch of every internal method, including
    /// the ones that name no element</b> - <c>"-0"</c>, <c>"1.5"</c>, <c>"-1"</c>, <c>"NaN"</c>,
    /// <c>"Infinity"</c>. They are never valid indices, so they are always absent and a write to one
    /// is always discarded; what they may never do is reach the ordinary map, where a detached or an
    /// attached view would otherwise answer for a key the specification says it cannot hold.
    /// <c>"01"</c> and <c>"1.0"</c> are NOT numeric - the round trip does not reproduce them - and
    /// stay ordinary property names.
    /// </para>
    /// <para>
    /// <b>The own-property overrides here are half of it; the engine is the other half.</b> The
    /// prototype walk of <c>[[Get]]</c>, <c>[[HasProperty]]</c> and <c>[[Set]]</c> is the engine's,
    /// so <c>JsEngine</c> asks this same question before walking, and
    /// <c>ObjectApplyDescriptor</c> asks it before defining, so that such a key is never read off
    /// a prototype, never reaches an inherited setter, and is refused by
    /// <c>[[DefineOwnProperty]]</c>.
    /// </para>
    /// <para>
    /// <b>The work is bounded by a constant, not by the key.</b> A canonical number string is at most
    /// two dozen characters, so a longer key is answered by its length, and a key whose first
    /// character no number string starts with is answered by that character. Only what is left is
    /// converted and printed back.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8E958A
    // Broiler-Human:        PENDING
    internal static bool IsNumericKey(string key)
    {
        if (IsArrayIndex(key, out _))
        {
            return true;
        }

        if (key.Length == 0 || key.Length > 32 || key[0] is not ((>= '0' and <= '9') or '-' or 'I' or 'N'))
        {
            return false;
        }

        return string.Equals(key, "-0", System.StringComparison.Ordinal) ||
            string.Equals(
                JsNumberFormat.ToJsString(JsNumberFormat.ToNumber(key)), key, System.StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=74713C
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
                _ = TryWriteAt((int)at, JsElements.ElementOf(Kind, property.Value));
            }

            return;
        }

        // A numeric key that is not an index is never a valid element, so the write is discarded
        // for the same reason an out-of-range index is.
        if (IsNumericKey(key))
        {
            return;
        }

        base.SetOwnProperty(key, property);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=97ECE9
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        if (IsArrayIndex(key, out var at))
        {
            // An element in the view refuses to be deleted - there is no hole a typed array could
            // become - and an index outside it answers true because deleting what is not there
            // succeeds.
            return at >= (uint)Length;
        }

        // Deleting what cannot be there succeeds, and it must not reach an ordinary property.
        if (IsNumericKey(key))
        {
            return true;
        }

        return base.DeleteOwnProperty(key);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=D5D7E4
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>();

        // A DETACHED OR OUT-OF-BOUNDS VIEW HAS NO INDICES TO LIST, and it still has every ordinary
        // key it was given. The length is read once: nothing in this loop can resize the buffer.
        var length = Length;

        for (var at = 0; at < length; at++)
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
