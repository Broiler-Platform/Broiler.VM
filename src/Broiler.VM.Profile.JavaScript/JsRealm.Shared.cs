// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           2
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  3/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>SharedArrayBuffer</c> and <c>Atomics</c>, where the composition admitted
/// <c>broiler.javascript.shared</c> beside <c>broiler.javascript.binary</c> (JSD-0041).
/// </summary>
/// <remarks>
/// <para>
/// <b>A shared buffer is the binary surface's buffer type over a shared block.</b> The typed arrays and
/// <c>DataView</c> read and write it as they read any buffer, it is never detached, and every
/// <c>ArrayBuffer.prototype</c> member refuses it as every member here refuses an unshared one.
/// </para>
/// <para>
/// <b><c>Atomics</c> works on every integer view</b>, shared or not, as the edition says; <c>wait</c>,
/// <c>waitAsync</c> and a <c>notify</c> that wakes anything need a shared <c>Int32Array</c> or
/// <c>BigInt64Array</c>. Every access is sequentially consistent (<see cref="JsAtomicAccess"/>).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=60DD8D
// Broiler-Falsified-If: a shared buffer is detached, transferred or reachable as an ArrayBuffer, or an Atomics access is not sequentially consistent
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>SharedArrayBuffer.prototype</c>, where the surface was admitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F8406B
    // Broiler-Human:        PENDING
    internal JsObject? SharedArrayBufferPrototype { get; private set; }

    /// <summary>The realm's <c>%SharedArrayBuffer%</c>, the default species of <c>slice</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=27D4DD
    // Broiler-Human:        PENDING
    private JsNativeFunction? sharedArrayBufferConstructor;

    /// <summary>Builds <c>SharedArrayBuffer</c> and <c>Atomics</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6DB918
    // Broiler-Falsified-If: a realm builds SharedArrayBuffer or Atomics without the binary and shared surfaces, or a shared buffer is reachable as an ArrayBuffer
    // Broiler-Human:        PENDING
    private void SetupShared()
    {
        var prototype = new JsObject(ObjectPrototype);
        SharedArrayBufferPrototype = prototype;

        var constructor = Constructor(
            "SharedArrayBuffer",
            1,
            prototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor SharedArrayBuffer requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                // THE LENGTH, THEN THE OPTIONS BAG, THEN THE OBJECT, THEN THE BLOCK: AllocateSharedArrayBuffer's
                // order, the same as ArrayBuffer's.
                var byteLength = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "length");
                var maxByteLength = BinaryMaxByteLengthOption(engine, ArgOfBinary(arguments, 1));

                if (maxByteLength is { } bound && byteLength > bound)
                {
                    throw engine.Error(
                        "RangeError",
                        "Invalid shared array buffer length: " + JsNumberFormat.ToJsString(byteLength) +
                        " exceeds the maximum byte length " + JsNumberFormat.ToJsString(bound));
                }

                var made = engine.PrototypeFromConstructor(thisValue, SharedArrayBufferPrototype!);
                var buffer = SharedNewBuffer(engine, byteLength, maxByteLength);
                buffer.Prototype = made;
                return JsValue.Object(buffer);
            });

        constructor.BuildsFromNewTarget = true;
        sharedArrayBufferConstructor = constructor;
        SpeciesGetter(constructor);

        BinaryGetter(prototype, "byteLength", static (engine, thisValue, arguments) =>
            JsValue.Number(SharedThisBuffer(engine, thisValue, "byteLength").ByteLength));

        BinaryGetter(prototype, "growable", static (engine, thisValue, arguments) =>
            JsValue.Boolean(SharedThisBuffer(engine, thisValue, "growable").IsResizable));

        BinaryGetter(prototype, "maxByteLength", static (engine, thisValue, arguments) =>
        {
            var buffer = SharedThisBuffer(engine, thisValue, "maxByteLength");
            return JsValue.Number(buffer.MaxByteLength ?? buffer.ByteLength);
        });

        // `grow` ONLY GROWS: a length below the current one is a RangeError, as is one past the
        // maximum, and a fixed-length receiver is refused before the length is converted.
        Method(prototype, "grow", 1, (engine, thisValue, arguments) =>
        {
            if (thisValue.AsObjectOrNull() is not JsArrayBuffer { IsShared: true, IsResizable: true } buffer)
            {
                return engine.ThrowTypeError(
                    "SharedArrayBuffer.prototype.grow requires that 'this' be a growable SharedArrayBuffer");
            }

            var requested = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "length");

            if (requested > buffer.MaxByteLength)
            {
                return engine.ThrowRangeError(
                    "SharedArrayBuffer.prototype.grow: " + JsNumberFormat.ToJsString(requested) +
                    " exceeds the maximum byte length");
            }

            lock (buffer.Block!.Gate)
            {
                if (requested < buffer.ByteLength)
                {
                    return engine.ThrowRangeError(
                        "SharedArrayBuffer.prototype.grow: a SharedArrayBuffer cannot shrink");
                }

                if (requested > buffer.ByteLength)
                {
                    BinaryResizeBuffer(engine, buffer, (int)requested);
                }
            }

            return JsValue.Undefined;
        });

        Method(prototype, "slice", 2, (engine, thisValue, arguments) =>
        {
            var buffer = SharedThisBuffer(engine, thisValue, "slice");
            var length = buffer.ByteLength;
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[1], length);

            var count = stop > start ? (int)(stop - start) : 0;
            var species = BinarySpeciesConstructor(engine, thisValue, sharedArrayBufferConstructor!);
            var constructed = engine.Construct(species, [JsValue.Number(count)]);

            if (constructed.AsObjectOrNull() is not JsArrayBuffer { IsShared: true } made)
            {
                return engine.ThrowTypeError(
                    "SharedArrayBuffer.prototype.slice: the species constructor did not return a SharedArrayBuffer");
            }

            if (ReferenceEquals(made, buffer) || ReferenceEquals(made.Block, buffer.Block))
            {
                return engine.ThrowTypeError(
                    "SharedArrayBuffer.prototype.slice: the species constructor returned the receiver itself");
            }

            if (made.ByteLength < count)
            {
                return engine.ThrowTypeError(
                    "SharedArrayBuffer.prototype.slice: the species constructor returned a buffer smaller " +
                    "than the slice");
            }

            if (count != 0)
            {
                engine.Charge((ulong)count);
                System.Array.Copy(buffer.Data!, (int)start, made.Data!, 0, count);
            }

            return constructed;
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("SharedArrayBuffer"), JsPropertyAttributes.Configurable));

        SetupAtomics();
    }

    /// <summary>Allocates a shared buffer, charging for its bytes as an unshared one is charged.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D059D2
    // Broiler-Human:        PENDING
    private JsArrayBuffer SharedNewBuffer(JsEngine engine, double byteLength, double? maxByteLength)
    {
        if (byteLength > System.Array.MaxLength ||
            maxByteLength is { } bound && bound > System.Array.MaxLength)
        {
            throw engine.Error(
                "RangeError",
                "Invalid shared array buffer length: " + JsNumberFormat.ToJsString(maxByteLength ?? byteLength));
        }

        var size = (int)byteLength;
        engine.Charge((ulong)size);
        engine.RetainOrAbort((ulong)size);

        var block = new JsSharedBlock(size, maxByteLength is { } max ? (int)max : null);
        return new JsArrayBuffer(SharedArrayBufferPrototype, block);
    }

    /// <summary>The receiver a <c>SharedArrayBuffer.prototype</c> member operates on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9862E8
    // Broiler-Human:        PENDING
    private static JsArrayBuffer SharedThisBuffer(JsEngine engine, JsValue value, string member) =>
        value.AsObjectOrNull() as JsArrayBuffer is { IsShared: true } buffer
            ? buffer
            : throw engine.Error(
                "TypeError",
                "SharedArrayBuffer.prototype." + member + " requires that 'this' be a SharedArrayBuffer");

    /// <summary>Builds the <c>Atomics</c> namespace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=59E354
    // Broiler-Human:        PENDING
    private void SetupAtomics()
    {
        var atomics = new JsObject(ObjectPrototype);

        AtomicsReadModifyWrite(atomics, "add", static (old, operand) => old + operand);
        AtomicsReadModifyWrite(atomics, "and", static (old, operand) => old & operand);
        AtomicsReadModifyWrite(atomics, "exchange", static (old, operand) => operand);
        AtomicsReadModifyWrite(atomics, "or", static (old, operand) => old | operand);
        AtomicsReadModifyWrite(atomics, "sub", static (old, operand) => old - operand);
        AtomicsReadModifyWrite(atomics, "xor", static (old, operand) => old ^ operand);

        Method(atomics, "compareExchange", 4, static (engine, thisValue, arguments) =>
        {
            var (array, byteIndex) = AtomicsAccess(engine, arguments, waitable: false);
            var expected = AtomicsOperand(engine, array, ArgOfBinary(arguments, 2), out _);
            var replacement = AtomicsOperand(engine, array, ArgOfBinary(arguments, 3), out _);
            var bytes = AtomicsRevalidate(engine, array, byteIndex);

            var old = JsAtomicAccess.CompareExchange(
                bytes, byteIndex, array.BytesPerElement, AtomicsGate(array), expected, replacement);

            return AtomicsValueOf(array.Kind, old);
        });

        Method(atomics, "isLockFree", 1, static (engine, thisValue, arguments) =>
        {
            var size = engine.ToInteger(ArgOfBinary(arguments, 0));
            return JsValue.Boolean(size is 1 or 2 or 4 or 8 && JsAtomicAccess.IsLockFree((int)size));
        });

        Method(atomics, "load", 2, static (engine, thisValue, arguments) =>
        {
            var (array, byteIndex) = AtomicsAccess(engine, arguments, waitable: false);
            var bytes = AtomicsRevalidate(engine, array, byteIndex);

            return AtomicsValueOf(
                array.Kind,
                JsAtomicAccess.Load(bytes, byteIndex, array.BytesPerElement, AtomicsGate(array)));
        });

        Method(atomics, "store", 3, static (engine, thisValue, arguments) =>
        {
            var (array, byteIndex) = AtomicsAccess(engine, arguments, waitable: false);
            var raw = AtomicsOperand(engine, array, ArgOfBinary(arguments, 2), out var stored);
            var bytes = AtomicsRevalidate(engine, array, byteIndex);

            _ = JsAtomicAccess.ReadModifyWrite(
                bytes, byteIndex, array.BytesPerElement, AtomicsGate(array), static (_, operand) => operand, raw);

            return stored;
        });

        Method(atomics, "wait", 4, static (engine, thisValue, arguments) =>
            AtomicsDoWait(engine, arguments, synchronous: true));

        Method(atomics, "waitAsync", 4, static (engine, thisValue, arguments) =>
            AtomicsDoWait(engine, arguments, synchronous: false));

        Method(atomics, "notify", 3, static (engine, thisValue, arguments) =>
        {
            var (array, byteIndex) = AtomicsAccess(engine, arguments, waitable: true);
            var count = ArgOfBinary(arguments, 2).Type == JsType.Undefined
                ? double.PositiveInfinity
                : System.Math.Max(0, engine.ToInteger(arguments[2]));

            // AN UNSHARED BUFFER HAS NO WAITERS, so a notify on one wakes nothing and answers zero.
            return array.Buffer.Block is { } block
                ? JsValue.Number(JsEngine.Notify(block, byteIndex, count))
                : JsValue.Number(0);
        });

        // `pause` IS A HINT, AND ITS ONE OBLIGATION IS THE ARGUMENT CHECK: a Number that is not an
        // integral Number is a TypeError, and anything else but `undefined` too.
        Method(atomics, "pause", 0, static (engine, thisValue, arguments) =>
        {
            var hint = ArgOfBinary(arguments, 0);

            if (hint.Type != JsType.Undefined &&
                (!hint.IsNumber || !double.IsInteger(hint.AsNumber())))
            {
                return engine.ThrowTypeError("Atomics.pause: the iteration hint is not an integral Number");
            }

            System.Threading.Thread.SpinWait(1);
            return JsValue.Undefined;
        });

        atomics.SetOwnSymbol(
            ToStringTagSymbol, JsProperty.Data(JsValue.String("Atomics"), JsPropertyAttributes.Configurable));

        GlobalObject.SetOwnProperty(
            "Atomics",
            JsProperty.Data(
                JsValue.Object(atomics), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>Defines one read-modify-write member: <c>add</c>, <c>and</c>, <c>exchange</c>, <c>or</c>, <c>sub</c>, <c>xor</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=41C19D
    // Broiler-Human:        PENDING
    private void AtomicsReadModifyWrite(JsObject atomics, string name, System.Func<long, long, long> operate) =>
        Method(atomics, name, 3, (engine, thisValue, arguments) =>
        {
            var (array, byteIndex) = AtomicsAccess(engine, arguments, waitable: false);
            var operand = AtomicsOperand(engine, array, ArgOfBinary(arguments, 2), out _);
            var bytes = AtomicsRevalidate(engine, array, byteIndex);

            var old = JsAtomicAccess.ReadModifyWrite(
                bytes, byteIndex, array.BytesPerElement, AtomicsGate(array), operate, operand);

            return AtomicsValueOf(array.Kind, old);
        });

    /// <summary>
    /// <c>ValidateIntegerTypedArray</c> and <c>ValidateAtomicAccess</c>: the view the first argument
    /// names and the byte index the second asks for.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=156D19
    // Broiler-Human:        PENDING
    private static (JsTypedArray Array, int ByteIndex) AtomicsAccess(
        JsEngine engine, JsValue[] arguments, bool waitable, string? requireShared = null)
    {
        if (ArgOfBinary(arguments, 0).AsObjectOrNull() is not JsTypedArray array || array.IsOutOfBounds)
        {
            throw engine.Error("TypeError", "Atomics: the first argument is not a valid typed array");
        }

        var admitted = waitable
            ? array.Kind is JsElementKind.Int32 or JsElementKind.BigInt64
            : array.Kind is JsElementKind.Int8 or JsElementKind.Uint8 or JsElementKind.Int16 or
                JsElementKind.Uint16 or JsElementKind.Int32 or JsElementKind.Uint32 or
                JsElementKind.BigInt64 or JsElementKind.BigUint64;

        if (!admitted)
        {
            throw engine.Error(
                "TypeError",
                waitable
                    ? "Atomics: the typed array is not an Int32Array or a BigInt64Array"
                    : "Atomics: the typed array is not an integer typed array");
        }

        // `wait` AND `waitAsync` REFUSE AN UNSHARED BUFFER BEFORE THE INDEX IS CONVERTED, which is
        // DoWait's order; `notify` converts the index first and answers zero for one.
        if (requireShared is not null && !array.Buffer.IsShared)
        {
            throw engine.Error("TypeError", requireShared + " requires a typed array over a SharedArrayBuffer");
        }

        var length = array.Length;
        var index = BinaryToIndex(engine, ArgOfBinary(arguments, 1), "index");

        if (index >= length)
        {
            throw engine.Error("RangeError", "Atomics: the index is out of range");
        }

        return (array, (int)index * array.BytesPerElement + array.ByteOffset);
    }

    /// <summary>
    /// Converts an operand as the element's content type asks - <c>ToBigInt</c> or
    /// <c>ToIntegerOrInfinity</c> - and answers its bits, with the converted value in <paramref name="converted"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0682EB
    // Broiler-Human:        PENDING
    private static long AtomicsOperand(JsEngine engine, JsTypedArray array, JsValue value, out JsValue converted)
    {
        if (array.HoldsBigInts)
        {
            var big = engine.ToBigInt(value);
            converted = JsValue.BigInt(big);
            return unchecked((long)(ulong)(big.Value & ulong.MaxValue));
        }

        var integer = engine.ToInteger(value);

        // `ToIntegerOrInfinity` ANSWERS +0 FOR -0, which is the value `store` hands back.
        converted = JsValue.Number(integer == 0 ? 0 : integer);
        return JsValue.ToUint32(integer);
    }

    /// <summary>
    /// <c>RevalidateAtomicAccess</c>: the view is still in bounds after the conversions, which may have
    /// run guest code, and still covers the byte index; answers the bytes to work on now.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=49D71C
    // Broiler-Human:        PENDING
    private static byte[] AtomicsRevalidate(JsEngine engine, JsTypedArray array, int byteIndex)
    {
        if (array.IsOutOfBounds || array.Buffer.Data is not { } bytes)
        {
            throw engine.Error("TypeError", "Atomics: the typed array is detached or out of bounds");
        }

        if (byteIndex + array.BytesPerElement > bytes.Length)
        {
            throw engine.Error("RangeError", "Atomics: the index is out of range");
        }

        return bytes;
    }

    /// <summary>The lock a narrow access takes: the shared block's gate, or the unshared buffer itself.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0BD824
    // Broiler-Human:        PENDING
    private static object AtomicsGate(JsTypedArray array) =>
        array.Buffer.Block?.Gate ?? array.Buffer;

    /// <summary>An element's bits as the value its kind reads them as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BCB739
    // Broiler-Human:        PENDING
    private static JsValue AtomicsValueOf(JsElementKind kind, long raw) => kind switch
    {
        JsElementKind.Int8 => JsValue.Number(unchecked((sbyte)raw)),
        JsElementKind.Uint8 => JsValue.Number(unchecked((byte)raw)),
        JsElementKind.Int16 => JsValue.Number(unchecked((short)raw)),
        JsElementKind.Uint16 => JsValue.Number(unchecked((ushort)raw)),
        JsElementKind.Int32 => JsValue.Number(unchecked((int)raw)),
        JsElementKind.Uint32 => JsValue.Number(unchecked((uint)raw)),
        JsElementKind.BigInt64 => JsValue.BigInt(new JsBigInt(new System.Numerics.BigInteger(raw))),
        _ => JsValue.BigInt(new JsBigInt(new System.Numerics.BigInteger(unchecked((ulong)raw)))),
    };

    /// <summary><c>DoWait</c>: <c>Atomics.wait</c> when <paramref name="synchronous"/>, else <c>waitAsync</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F20377
    // Broiler-Falsified-If: Atomics.wait blocks an agent whose host did not say it may block
    // Broiler-Human:        PENDING
    private static JsValue AtomicsDoWait(JsEngine engine, JsValue[] arguments, bool synchronous)
    {
        var (array, byteIndex) = AtomicsAccess(
            engine, arguments, waitable: true, synchronous ? "Atomics.wait" : "Atomics.waitAsync");
        var block = array.Buffer.Block!;

        var expected = array.Kind == JsElementKind.BigInt64
            ? unchecked((long)(ulong)(engine.ToBigInt(ArgOfBinary(arguments, 2)).Value & ulong.MaxValue))
            : (long)(uint)JsValue.ToInt32(engine.ToNumber(ArgOfBinary(arguments, 2)));

        var requested = engine.ToNumber(ArgOfBinary(arguments, 3));
        var timeout = double.IsNaN(requested) || double.IsPositiveInfinity(requested)
            ? double.PositiveInfinity
            : System.Math.Max(requested, 0);

        if (!synchronous)
        {
            return engine.WaitAsync(block, byteIndex, array.BytesPerElement, expected, timeout);
        }

        // AN AGENT THAT MAY NOT BLOCK IS REFUSED AFTER THE CONVERSIONS, as AgentCanSuspend is asked.
        if (!engine.CanBlock)
        {
            return engine.ThrowTypeError("Atomics.wait: this agent may not block");
        }

        return JsValue.String(
            engine.WaitBlocking(block, byteIndex, array.BytesPerElement, expected, timeout));
    }
}
