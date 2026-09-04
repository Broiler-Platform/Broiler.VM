// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   26
// Annotated:        26/26
// Exempt:           4
// Human-reviewed:   0/26
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       26
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The binary surface: <c>ArrayBuffer</c>, <c>DataView</c>, and the nine typed arrays under their
/// shared <c>%TypedArray%</c> superclass.
/// </summary>
/// <remarks>
/// <para>
/// <b>THERE IS NO <c>SharedArrayBuffer</c> AND NO <c>Atomics</c>, AND THAT IS THE POINT OF THIS
/// PARAGRAPH.</b> They are not missing work; they are the multi-agent surface, and they need an
/// agent model this profile does not have - a way to say which instances share a heap, what a
/// composition admits when it admits sharing, and what an allowance means once two agents spend
/// against one buffer. Folding them in beside the ordinary buffer would make the two
/// indistinguishable at the point where it matters most: a composition asking for a byte buffer
/// would admit cross-agent shared memory by accident, and the one line in a manifest that was
/// supposed to be about "can this program hold bytes" would silently also mean "can this program
/// share mutable state with another agent". They stay out until the agent model exists to say what
/// admitting them means.
/// </para>
/// <para>
/// <b><c>BigInt64Array</c> and <c>BigUint64Array</c> are absent for a smaller reason: this realm
/// has no BigInt.</b> Their elements are specified to read as BigInt values, and a realm without
/// that type could only answer a Number, which loses exactly the bits a 64-bit integer array
/// exists to keep. An absent global is a <c>ReferenceError</c> a program can detect; a lossy one
/// is a wrong answer it cannot.
/// </para>
/// <para>
/// <b>Every method that touches a buffer re-checks detachment.</b> Detaching is reachable in the
/// language through <c>ArrayBuffer.prototype.transfer</c>, and a <c>valueOf</c> called during a
/// built-in's own argument coercion can do it between two lines of that built-in. So nothing here
/// caches "the bytes"; the views ask their buffer on every access and the built-ins ask again
/// after every step that could have run guest code.
/// </para>
/// <para>
/// <b>Fuel is charged per element, the way <c>JsRealm.Array.cs</c> charges it.</b> A copy, a fill,
/// a sort or a join over a hundred-megabyte array is a hundred megabytes of work whether the guest
/// wrote the loop or a built-in did, and an allowance that only counted bytecode would be spent by
/// the wrong program. Allocation is charged too, before the bytes exist: a buffer the meter will
/// not pay for is never allocated.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>ArrayBuffer.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=501BF4
    // Broiler-Human:        PENDING
    internal JsObject ArrayBufferPrototype { get; private set; } = null!;

    /// <summary><c>DataView.prototype</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E6BEED
    // Broiler-Human:        PENDING
    internal JsObject DataViewPrototype { get; private set; } = null!;

    /// <summary>
    /// <c>%TypedArray%.prototype</c>: the object the nine element prototypes inherit from.
    /// </summary>
    /// <remarks>
    /// It carries every shared method exactly once, which is what makes
    /// <c>Object.getPrototypeOf(Int8Array.prototype) === Object.getPrototypeOf(Float64Array.prototype)</c>
    /// true and what lets a program hang a helper on all nine at once - a shape the specification
    /// exposes deliberately, even though <c>%TypedArray%</c> itself has no global name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1A07EA
    // Broiler-Human:        PENDING
    internal JsObject TypedArrayPrototype { get; private set; } = null!;

    /// <summary>The nine per-kind prototypes, by the kind whose constructor owns each.</summary>
    /// <remarks>
    /// <c>map</c>, <c>filter</c>, <c>slice</c> and <c>subarray</c> all answer a typed array of the
    /// receiver's OWN kind, so each of them needs to reach the prototype for a kind it only learns
    /// at run time. A dictionary keyed by the kind is what makes those four one line each instead
    /// of a nine-armed switch repeated four times.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B1F978
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<JsElementKind, JsObject> TypedArrayPrototypes { get; } =
        new();

    /// <summary>Builds the whole binary surface, in dependency order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C774E4
    // Broiler-Human:        PENDING
    private void SetupBinary()
    {
        ArrayBufferPrototype = new JsObject(ObjectPrototype, "ArrayBuffer");
        DataViewPrototype = new JsObject(ObjectPrototype, "DataView");
        TypedArrayPrototype = new JsObject(ObjectPrototype, "TypedArray");

        SetupArrayBuffer();
        SetupDataView();
        SetupTypedArrayAccessors();
        SetupTypedArrayMutators();
        SetupTypedArrayReaders();
        SetupTypedArrayIteration();
        SetupTypedArrayConstructors();
    }

    /// <summary>Builds <c>ArrayBuffer</c>, its one static and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=59EE3E
    // Broiler-Human:        PENDING
    private void SetupArrayBuffer()
    {
        var constructor = Constructor(
            "ArrayBuffer",
            1,
            ArrayBufferPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor ArrayBuffer requires 'new'"),
            (engine, thisValue, arguments) => JsValue.Object(
                BinaryNewBuffer(engine, BinaryToIndex(engine, ArgOfBinary(arguments, 0), "length"))));

        // `isView` ANSWERS FOR BOTH VIEW KINDS AND FOR NOTHING ELSE. It is not "is this backed by a
        // buffer" - an ArrayBuffer itself answers false, which is the question callers actually
        // need answered before they reach for `byteOffset`.
        Method(constructor, "isView", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(ArgOfBinary(arguments, 0).AsObjectOrNull() is JsTypedArray or JsDataView));

        BinaryGetter(ArrayBufferPrototype, "byteLength", static (engine, thisValue, arguments) =>
            JsValue.Number(BinaryThisBuffer(engine, thisValue, "byteLength").ByteLength));

        Method(ArrayBufferPrototype, "slice", 2, (engine, thisValue, arguments) =>
        {
            var buffer = BinaryThisBuffer(engine, thisValue, "slice");

            if (buffer.IsDetached)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice called on a detached ArrayBuffer");
            }

            var length = buffer.ByteLength;
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[1], length);

            var count = stop > start ? (int)(stop - start) : 0;
            engine.Charge((ulong)count);
            var made = BinaryNewBuffer(engine, count);

            // THE RECEIVER IS RE-READ AFTER THE COERCIONS. `ArrayRelative` calls `ToInteger`, which
            // can run a `valueOf` that transfers the receiver away, and the bytes this copies must
            // be the ones that are there now rather than the ones that were.
            if (!made.TryCopyFrom(buffer.Data, (int)start, count))
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice called on a detached ArrayBuffer");
            }

            return JsValue.Object(made);
        });

        // THE ONLY WAY TO DETACH A BUFFER FROM INSIDE THE LANGUAGE. Without it the detached state
        // would be unreachable and untestable, and every "is it detached" branch in this file and
        // in JsBinary.cs would be dead code that nothing could exercise.
        Method(ArrayBufferPrototype, "transfer", 0, (engine, thisValue, arguments) =>
        {
            var buffer = BinaryThisBuffer(engine, thisValue, "transfer");

            if (buffer.IsDetached)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.transfer called on a detached ArrayBuffer");
            }

            var requested = ArgOfBinary(arguments, 0).Type == JsType.Undefined
                ? buffer.ByteLength
                : BinaryToIndex(engine, arguments[0], "length");

            // ALLOCATE FIRST, DETACH SECOND. A refused allocation must leave the receiver alive:
            // detaching and then failing would destroy the bytes on the way to reporting that
            // there was nowhere to put them.
            var made = BinaryNewBuffer(engine, requested);
            var released = buffer.Detach();
            var carried = released is null
                ? 0
                : System.Math.Min(made.ByteLength, released.Length);

            engine.Charge((ulong)carried);
            _ = made.TryCopyFrom(released, 0, carried);
            return JsValue.Object(made);
        });
    }

    /// <summary>Builds <c>DataView</c>, its prototype and the sixteen accessors.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=6B2F42
    // Broiler-Human:        PENDING
    private void SetupDataView()
    {
        _ = Constructor(
            "DataView",
            1,
            DataViewPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor DataView requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                if (ArgOfBinary(arguments, 0).AsObjectOrNull() is not JsArrayBuffer buffer)
                {
                    return engine.ThrowTypeError(
                        "First argument to DataView constructor must be an ArrayBuffer");
                }

                var offset = BinaryToIndex(engine, ArgOfBinary(arguments, 1), "byteOffset");

                if (buffer.IsDetached)
                {
                    return engine.ThrowTypeError(
                        "Cannot construct a DataView over a detached ArrayBuffer");
                }

                if (offset > buffer.ByteLength)
                {
                    return engine.ThrowRangeError(
                        "Start offset " + JsNumberFormat.ToJsString(offset) +
                        " is outside the bounds of the buffer");
                }

                double length;

                if (ArgOfBinary(arguments, 2).Type == JsType.Undefined)
                {
                    length = buffer.ByteLength - offset;
                }
                else
                {
                    length = BinaryToIndex(engine, arguments[2], "byteLength");

                    if (offset + length > buffer.ByteLength)
                    {
                        return engine.ThrowRangeError(
                            "Invalid DataView length " + JsNumberFormat.ToJsString(length));
                    }
                }

                return JsValue.Object(
                    new JsDataView(DataViewPrototype, buffer, (int)offset, (int)length));
            });

        BinaryGetter(DataViewPrototype, "buffer", static (engine, thisValue, arguments) =>
            JsValue.Object(BinaryThisView(engine, thisValue, "buffer").Buffer));

        BinaryGetter(DataViewPrototype, "byteLength", static (engine, thisValue, arguments) =>
        {
            var view = BinaryThisView(engine, thisValue, "byteLength");

            return view.IsDetached
                ? engine.ThrowTypeError("Cannot read byteLength of a detached DataView")
                : JsValue.Number(view.ByteLength);
        });

        BinaryGetter(DataViewPrototype, "byteOffset", static (engine, thisValue, arguments) =>
        {
            var view = BinaryThisView(engine, thisValue, "byteOffset");

            return view.IsDetached
                ? engine.ThrowTypeError("Cannot read byteOffset of a detached DataView")
                : JsValue.Number(view.ByteOffset);
        });

        foreach (var kind in JsElements.All)
        {
            // A CLAMPED BYTE IS NOT A WIRE FORMAT. `Uint8ClampedArray` exists to hold image samples
            // and has no `getUint8Clamped` counterpart here, exactly as it has none in the
            // specification.
            if (kind == JsElementKind.Uint8Clamped)
            {
                continue;
            }

            var name = JsElements.ConstructorNameOf(kind);
            var suffix = name[..(name.Length - "Array".Length)];
            var reader = "get" + suffix;
            var writer = "set" + suffix;

            Method(DataViewPrototype, reader, 1, (engine, thisValue, arguments) =>
                BinaryViewRead(engine, thisValue, arguments, kind, reader));

            Method(DataViewPrototype, writer, 2, (engine, thisValue, arguments) =>
                BinaryViewWrite(engine, thisValue, arguments, kind, writer));
        }
    }

    /// <summary>The four getters every typed array inherits.</summary>
    /// <remarks>
    /// <b>Three of them answer zero for a detached buffer rather than throwing.</b> A program that
    /// has just had a buffer transferred out from under it should be able to ASK - <c>if
    /// (view.length === 0)</c> - without wrapping the question in a <c>try</c>. <c>buffer</c> is
    /// the exception and still answers the buffer, because the detached buffer is precisely what a
    /// caller in that state needs to name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=98D013
    // Broiler-Human:        PENDING
    private void SetupTypedArrayAccessors()
    {
        BinaryGetter(TypedArrayPrototype, "buffer", static (engine, thisValue, arguments) =>
            JsValue.Object(BinaryThisTypedArray(engine, thisValue, "buffer").Buffer));

        BinaryGetter(TypedArrayPrototype, "byteLength", static (engine, thisValue, arguments) =>
            JsValue.Number(BinaryThisTypedArray(engine, thisValue, "byteLength").ByteLength));

        BinaryGetter(TypedArrayPrototype, "byteOffset", static (engine, thisValue, arguments) =>
        {
            var array = BinaryThisTypedArray(engine, thisValue, "byteOffset");
            return JsValue.Number(array.IsDetached ? 0 : array.ByteOffset);
        });

        BinaryGetter(TypedArrayPrototype, "length", static (engine, thisValue, arguments) =>
        {
            var array = BinaryThisTypedArray(engine, thisValue, "length");
            return JsValue.Number(array.IsDetached ? 0 : array.Length);
        });
    }

    /// <summary><c>set</c>, <c>subarray</c>, <c>slice</c>, <c>fill</c>, <c>copyWithin</c>, <c>reverse</c>, <c>sort</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CB4842
    // Broiler-Human:        PENDING
    private void SetupTypedArrayMutators()
    {
        Method(TypedArrayPrototype, "set", 1, (engine, thisValue, arguments) =>
        {
            var target = BinaryLiveTypedArray(engine, thisValue, "set");
            var offset = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (offset < 0)
            {
                return engine.ThrowRangeError("offset is out of bounds");
            }

            var source = ArgOfBinary(arguments, 0);

            if (source.AsObjectOrNull() is JsTypedArray typed)
            {
                return BinarySetFromTypedArray(engine, target, typed, offset);
            }

            var host = source.IsNullish
                ? engine.ThrowTypeError("%TypedArray%.prototype.set requires an array-like source")
                : source.IsObject ? source : JsValue.Object(engine.ToObject(source));

            var length = engine.ToUint32(engine.GetProperty(host, "length"));

            if (length + offset > target.Length)
            {
                return engine.ThrowRangeError("offset is out of bounds");
            }

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = engine.ToNumber(engine.GetIndexed(host, JsValue.Number(at)));
                _ = target.TryWriteAt((int)(at + offset), element);
            }

            return JsValue.Undefined;
        });

        // `subarray` SHARES THE BYTES AND `slice` COPIES THEM. Both answer the receiver's own kind;
        // the difference is that writing through a subarray is visible through the array it came
        // from, which is the whole reason both exist.
        Method(TypedArrayPrototype, "subarray", 2, (engine, thisValue, arguments) =>
        {
            var array = BinaryThisTypedArray(engine, thisValue, "subarray");
            var length = array.IsDetached ? 0 : array.Length;
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[1], length);

            var count = stop > start ? (int)(stop - start) : 0;

            return JsValue.Object(new JsTypedArray(
                TypedArrayPrototypes[array.Kind],
                array.Buffer,
                array.ByteOffset + ((int)start * array.BytesPerElement),
                count,
                array.Kind));
        });

        Method(TypedArrayPrototype, "slice", 2, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "slice");
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), array.Length);
            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? array.Length
                : ArrayRelative(engine, arguments[1], array.Length);

            var count = stop > start ? (int)(stop - start) : 0;
            var made = BinaryNewTypedArray(engine, array.Kind, count);

            for (var at = 0; at < count; at++)
            {
                engine.Charge(1);
                _ = array.TryReadAt((int)start + at, out var element);
                _ = made.TryWriteAt(at, element);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "fill", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "fill");
            var value = engine.ToNumber(ArgOfBinary(arguments, 0));
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 1), array.Length);
            var stop = ArgOfBinary(arguments, 2).Type == JsType.Undefined
                ? array.Length
                : ArrayRelative(engine, arguments[2], array.Length);

            for (var at = (int)start; at < stop; at++)
            {
                engine.Charge(1);
                _ = array.TryWriteAt(at, value);
            }

            return thisValue;
        });

        Method(TypedArrayPrototype, "copyWithin", 2, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "copyWithin");
            var length = array.Length;
            var to = (int)ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var from = (int)ArrayRelative(engine, ArgOfBinary(arguments, 1), length);
            var stop = ArgOfBinary(arguments, 2).Type == JsType.Undefined
                ? length
                : (int)ArrayRelative(engine, arguments[2], length);

            var count = System.Math.Min(stop - from, length - to);

            if (count > 0)
            {
                engine.Charge((ulong)count);
                _ = array.TryCopyWithin(to, from, count);
            }

            return thisValue;
        });

        Method(TypedArrayPrototype, "reverse", 0, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "reverse");
            var middle = array.Length / 2;

            for (var lower = 0; lower < middle; lower++)
            {
                engine.Charge(1);
                var upper = (array.Length - lower) - 1;
                _ = array.TryReadAt(lower, out var first);
                _ = array.TryReadAt(upper, out var second);
                _ = array.TryWriteAt(lower, second);
                _ = array.TryWriteAt(upper, first);
            }

            return thisValue;
        });

        // THE DEFAULT ORDER IS NUMERIC, WHICH IS THE OPPOSITE OF `Array.prototype.sort`. An Array
        // sorts by ToString and answers [1, 10, 2]; a typed array holds only numbers, so the
        // specification orders them as numbers and a typed array answers [1, 2, 10]. Two methods
        // with one name and two orders is a real trap, and reproducing it is the job.
        Method(TypedArrayPrototype, "sort", 1, (engine, thisValue, arguments) =>
        {
            var comparator = ArgOfBinary(arguments, 0);

            if (comparator.Type != JsType.Undefined &&
                (!comparator.IsObject || !comparator.AsObject().IsCallable))
            {
                return engine.ThrowTypeError(
                    "the comparison function must be either a function or undefined");
            }

            var array = BinaryLiveTypedArray(engine, thisValue, "sort");
            var items = new System.Collections.Generic.List<double>(array.Length);

            // MATERIALISE FIRST, for the reason Array's sort materialises: a comparator is guest
            // code, it can detach the buffer, and a sort that read the elements as it went would
            // compare bytes that stopped existing halfway through.
            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                _ = array.TryReadAt(at, out var element);
                items.Add(element);
            }

            if (items.Count > 1)
            {
                var buffer = new System.Collections.Generic.List<double>(items);
                BinaryMergeSort(engine, comparator, items, buffer, 0, items.Count);
            }

            for (var at = 0; at < items.Count; at++)
            {
                engine.Charge(1);
                _ = array.TryWriteAt(at, items[at]);
            }

            return thisValue;
        });
    }

    /// <summary>The searches, <c>join</c>, <c>at</c> and <c>toString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F95984
    // Broiler-Human:        PENDING
    private void SetupTypedArrayReaders()
    {
        Method(TypedArrayPrototype, "indexOf", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "indexOf");
            var wanted = ArgOfBinary(arguments, 0);
            var from = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (array.Length == 0 || from >= array.Length)
            {
                return JsValue.Number(-1);
            }

            var start = from >= 0 ? from : System.Math.Max(array.Length + from, 0);

            for (var at = (int)start; at < array.Length; at++)
            {
                engine.Charge(1);

                if (array.ElementAt(at).StrictlyEquals(wanted))
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(TypedArrayPrototype, "lastIndexOf", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "lastIndexOf");
            var wanted = ArgOfBinary(arguments, 0);
            var from = arguments.Length > 1
                ? engine.ToInteger(arguments[1])
                : array.Length - 1;

            var start = from >= 0 ? System.Math.Min(from, array.Length - 1) : array.Length + from;

            if (array.Length == 0 || start < 0)
            {
                return JsValue.Number(-1);
            }

            for (var at = (int)start; at >= 0; at--)
            {
                engine.Charge(1);

                if (array.ElementAt(at).StrictlyEquals(wanted))
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        // SameValueZero, NOT ===, which is the one difference from `indexOf`: a Float64Array
        // holding a NaN reports `includes(NaN)` true and `indexOf(NaN)` -1.
        Method(TypedArrayPrototype, "includes", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "includes");
            var wanted = ArgOfBinary(arguments, 0);
            var from = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (array.Length == 0 || from >= array.Length)
            {
                return JsValue.False;
            }

            var start = from >= 0 ? from : System.Math.Max(array.Length + from, 0);

            for (var at = (int)start; at < array.Length; at++)
            {
                engine.Charge(1);

                if (array.ElementAt(at).SameValueZero(wanted))
                {
                    return JsValue.True;
                }
            }

            return JsValue.False;
        });

        // A TYPED ARRAY IS ITERABLE, AND IT WAS NOT UNTIL 2026-09-04.
        //
        // `%TypedArray%.prototype[Symbol.iterator]` IS `values` - the same function object under
        // both keys, exactly as `Array.prototype` has it - so `[...new Uint8Array([1, 2])]`,
        // `for (const b of bytes)` and a `yield*` over one all work. Without it a typed array was
        // indexable and not iterable, which is a distinction no program expects and which the
        // seam between the binary surface and the iteration protocol made visible the moment both
        // existed.
        //
        // The iterator is the ordinary indexed one: it reads `length` and the indices through the
        // property path, so the integer-indexed exotic rules apply to it as they do to every other
        // reader, and a detached buffer answers `undefined` rather than a stale byte.
        var typedValues = Native("values", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            _ = BinaryLiveTypedArray(engine, thisValue, "values");

            return JsValue.Object(
                engine.Realm.CreateIndexedIterator(thisValue, IndexedIteratorKind.Value));
        });

        TypedArrayPrototype.SetOwnProperty(
            "values", JsProperty.Data(JsValue.Object(typedValues), JsPropertyAttributes.BuiltIn));

        TypedArrayPrototype.SetOwnSymbol(
            IteratorSymbol,
            JsProperty.Data(JsValue.Object(typedValues), JsPropertyAttributes.BuiltIn));

        Method(TypedArrayPrototype, "keys", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            _ = BinaryLiveTypedArray(engine, thisValue, "keys");

            return JsValue.Object(
                engine.Realm.CreateIndexedIterator(thisValue, IndexedIteratorKind.Key));
        });

        Method(TypedArrayPrototype, "entries", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            _ = BinaryLiveTypedArray(engine, thisValue, "entries");

            return JsValue.Object(
                engine.Realm.CreateIndexedIterator(thisValue, IndexedIteratorKind.Entry));
        });

        Method(TypedArrayPrototype, "join", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "join");
            var separatorValue = ArgOfBinary(arguments, 0);
            var separator = separatorValue.Type == JsType.Undefined
                ? ","
                : engine.ToStringValue(separatorValue);

            var text = new System.Text.StringBuilder();

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);

                if (at > 0)
                {
                    text.Append(separator);
                }

                // AN ELEMENT IS ALWAYS A NUMBER UNLESS THE BUFFER WENT AWAY. There are no holes in
                // a typed array, so the only way this reads `undefined` is a detach that happened
                // during the separator's own coercion, and `undefined` renders as nothing.
                var element = array.ElementAt(at);

                if (!element.IsNullish)
                {
                    text.Append(engine.ToStringValue(element));
                }
            }

            return JsValue.String(text.ToString());
        });

        Method(TypedArrayPrototype, "at", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "at");
            var relative = engine.ToInteger(ArgOfBinary(arguments, 0));
            var at = relative >= 0 ? relative : array.Length + relative;

            return at < 0 || at >= array.Length ? JsValue.Undefined : array.ElementAt((int)at);
        });

        // `toString` DELEGATES TO WHATEVER `join` THE RECEIVER HAS, exactly as Array's does, so a
        // program that replaces `join` sees `String(view)` follow it. There is deliberately NO
        // `valueOf`: a typed array has no primitive it could sensibly become, and inheriting
        // Object.prototype's - which answers the object itself - is what makes `view + ""` fall
        // back to `toString` rather than to a number nobody meant.
        Method(TypedArrayPrototype, "toString", 0, (engine, thisValue, arguments) =>
        {
            var join = engine.GetProperty(thisValue, "join");

            if (join.IsObject && join.AsObject().IsCallable)
            {
                return engine.Call(join, thisValue, System.Array.Empty<JsValue>());
            }

            var fallback = engine.GetProperty(JsValue.Object(ObjectPrototype), "toString");
            return engine.Call(fallback, thisValue, System.Array.Empty<JsValue>());
        });
    }

    /// <summary>The callback-taking methods.</summary>
    /// <remarks>
    /// None of them skips an index. A typed array has no holes - every slot in range is a number
    /// the bytes decode to - so the hole distinction that runs through
    /// <c>JsRealm.Array.cs</c> simply does not arise here, and the loops are the simpler for it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=25C787
    // Broiler-Human:        PENDING
    private void SetupTypedArrayIteration()
    {
        Method(TypedArrayPrototype, "forEach", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "forEach");
            var callback = BinaryCallbackOf(engine, arguments, "forEach");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                _ = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);
            }

            return JsValue.Undefined;
        });

        Method(TypedArrayPrototype, "map", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "map");
            var callback = BinaryCallbackOf(engine, arguments, "map");
            var thisArg = ArgOfBinary(arguments, 1);
            var made = BinaryNewTypedArray(engine, array.Kind, array.Length);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var mapped = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);

                _ = made.TryWriteAt(at, engine.ToNumber(mapped));
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "filter", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "filter");
            var callback = BinaryCallbackOf(engine, arguments, "filter");
            var thisArg = ArgOfBinary(arguments, 1);
            var kept = new System.Collections.Generic.List<double>();

            // TWO PASSES, because the result's length is not known until the predicate has answered
            // for every element and a typed array cannot grow.
            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var element = array.ElementAt(at);
                var verdict = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), thisValue]);

                if (verdict.ToBooleanValue())
                {
                    kept.Add(element.IsNumber ? element.AsNumber() : double.NaN);
                }
            }

            var made = BinaryNewTypedArray(engine, array.Kind, kept.Count);

            for (var at = 0; at < kept.Count; at++)
            {
                engine.Charge(1);
                _ = made.TryWriteAt(at, kept[at]);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "every", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "every");
            var callback = BinaryCallbackOf(engine, arguments, "every");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var verdict = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);

                if (!verdict.ToBooleanValue())
                {
                    return JsValue.False;
                }
            }

            return JsValue.True;
        });

        Method(TypedArrayPrototype, "some", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "some");
            var callback = BinaryCallbackOf(engine, arguments, "some");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var verdict = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);

                if (verdict.ToBooleanValue())
                {
                    return JsValue.True;
                }
            }

            return JsValue.False;
        });

        Method(TypedArrayPrototype, "find", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "find");
            var callback = BinaryCallbackOf(engine, arguments, "find");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var element = array.ElementAt(at);
                var found = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), thisValue]);

                if (found.ToBooleanValue())
                {
                    return element;
                }
            }

            return JsValue.Undefined;
        });

        Method(TypedArrayPrototype, "findIndex", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "findIndex");
            var callback = BinaryCallbackOf(engine, arguments, "findIndex");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                var found = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);

                if (found.ToBooleanValue())
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(TypedArrayPrototype, "reduce", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "reduce");
            var callback = BinaryCallbackOf(engine, arguments, "reduce");
            var at = 0;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else if (array.Length == 0)
            {
                return engine.ThrowTypeError("Reduce of empty array with no initial value");
            }
            else
            {
                accumulated = array.ElementAt(0);
                at = 1;
            }

            for (; at < array.Length; at++)
            {
                engine.Charge(1);
                accumulated = engine.Call(
                    callback,
                    JsValue.Undefined,
                    [accumulated, array.ElementAt(at), JsValue.Number(at), thisValue]);
            }

            return accumulated;
        });

        Method(TypedArrayPrototype, "reduceRight", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "reduceRight");
            var callback = BinaryCallbackOf(engine, arguments, "reduceRight");
            var at = array.Length - 1;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else if (array.Length == 0)
            {
                return engine.ThrowTypeError("Reduce of empty array with no initial value");
            }
            else
            {
                accumulated = array.ElementAt(at);
                at--;
            }

            for (; at >= 0; at--)
            {
                engine.Charge(1);
                accumulated = engine.Call(
                    callback,
                    JsValue.Undefined,
                    [accumulated, array.ElementAt(at), JsValue.Number(at), thisValue]);
            }

            return accumulated;
        });
    }

    /// <summary>Builds <c>%TypedArray%</c> and the nine constructors that inherit from it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=50F4CD
    // Broiler-Human:        PENDING
    private void SetupTypedArrayConstructors()
    {
        // %TypedArray% IS NOT A GLOBAL. It is built by hand rather than through `Constructor`
        // precisely because `Constructor` binds a global name, and this one has none: the language
        // reaches it only as `Object.getPrototypeOf(Int8Array)`. Calling or constructing it is a
        // TypeError, which is what "abstract" means when there is no such thing as an abstract
        // function object.
        var superclass = new JsNativeFunction(
            FunctionPrototype,
            "TypedArray",
            0,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Abstract class TypedArray not directly callable"),
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Abstract class TypedArray not directly constructable"));

        superclass.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(TypedArrayPrototype), JsPropertyAttributes.None));

        TypedArrayPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                JsValue.Object(superclass),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        foreach (var kind in JsElements.All)
        {
            var name = JsElements.ConstructorNameOf(kind);
            var width = JsElements.WidthOf(kind);
            var prototype = new JsObject(TypedArrayPrototype, name);
            TypedArrayPrototypes[kind] = prototype;

            var constructor = Constructor(
                name,
                3,
                prototype,
                (engine, thisValue, arguments) =>
                    engine.ThrowTypeError("Constructor " + name + " requires 'new'"),
                (engine, thisValue, arguments) => BinaryConstructTypedArray(engine, kind, arguments));

            // THE CONSTRUCTOR'S OWN PROTOTYPE IS %TypedArray%, not Function.prototype, which is
            // what makes `Int8Array.from` reachable through the superclass in a real engine and
            // what a program checks when it asks whether something is one of the nine.
            constructor.Prototype = superclass;

            // ON BOTH THE CONSTRUCTOR AND THE PROTOTYPE, and frozen on each: the specification
            // defines it in both places, and code that computes an offset reads it off whichever
            // one it happens to hold.
            constructor.DefineFrozen("BYTES_PER_ELEMENT", JsValue.Number(width));
            prototype.DefineFrozen("BYTES_PER_ELEMENT", JsValue.Number(width));

            Method(constructor, "of", 0, (engine, thisValue, arguments) =>
            {
                var made = BinaryNewTypedArray(engine, kind, arguments.Length);

                for (var at = 0; at < arguments.Length; at++)
                {
                    engine.Charge(1);
                    _ = made.TryWriteAt(at, engine.ToNumber(arguments[at]));
                }

                return JsValue.Object(made);
            });

            // ARRAY-LIKES ONLY, for the reason `Array.from` takes array-likes only: iterables are
            // out of this profile's scope, so a Set or a generator arrives as an object with no
            // `length` and produces an empty array rather than a wrong one.
            Method(constructor, "from", 1, (engine, thisValue, arguments) =>
            {
                var items = ArgOfBinary(arguments, 0);

                if (items.IsNullish)
                {
                    return engine.ThrowTypeError(name + ".from requires an array-like object");
                }

                var mapper = ArgOfBinary(arguments, 1);

                if (mapper.Type != JsType.Undefined &&
                    (!mapper.IsObject || !mapper.AsObject().IsCallable))
                {
                    return engine.ThrowTypeError(
                        name + ".from: the mapping function is not a function");
                }

                var thisArg = ArgOfBinary(arguments, 2);
                var source = items.IsObject ? items : JsValue.Object(engine.ToObject(items));
                var length = engine.ToUint32(engine.GetProperty(source, "length"));
                var made = BinaryNewTypedArray(engine, kind, length);

                for (double at = 0; at < length; at++)
                {
                    engine.Charge(1);
                    var element = engine.GetIndexed(source, JsValue.Number(at));

                    if (mapper.IsObject)
                    {
                        element = engine.Call(mapper, thisArg, [element, JsValue.Number(at)]);
                    }

                    _ = made.TryWriteAt((int)at, engine.ToNumber(element));
                }

                return JsValue.Object(made);
            });
        }
    }

    /// <summary>Reads argument <paramref name="at"/>, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7EDB8D
    // Broiler-Human:        PENDING
    private static JsValue ArgOfBinary(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Defines a getter-only accessor on <paramref name="host"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=60F708
    // Broiler-Human:        PENDING
    private void BinaryGetter(JsObject host, string name, JsNativeBody body) =>
        host.SetOwnProperty(
            name,
            JsProperty.Accessor(
                Native("get " + name, 0, body), null, JsPropertyAttributes.Configurable));

    /// <summary>The specification's <c>ToIndex</c>: a non-negative integer a length may be.</summary>
    /// <remarks>
    /// <c>undefined</c> is zero, a fraction truncates, and a negative or an out-of-range value is a
    /// <c>RangeError</c> naming what it was - because a length that silently became zero would
    /// turn <c>new Uint8Array(-1)</c> into an empty array that the calling code then walks
    /// believing it asked for something.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C8445E
    // Broiler-Human:        PENDING
    private static double BinaryToIndex(JsEngine engine, JsValue value, string what)
    {
        if (value.Type == JsType.Undefined)
        {
            return 0;
        }

        var number = engine.ToInteger(value);

        if (number < 0 || number > 9007199254740991.0)
        {
            throw engine.Error(
                "RangeError",
                "Invalid " + what + ": " + JsNumberFormat.ToJsString(number));
        }

        return number;
    }

    /// <summary>Allocates a buffer, charging for the bytes before they exist.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=725C07
    // Broiler-Human:        PENDING
    private JsArrayBuffer BinaryNewBuffer(JsEngine engine, double byteLength)
    {
        if (byteLength < 0 || byteLength > int.MaxValue ||
            byteLength != System.Math.Floor(byteLength))
        {
            throw engine.Error(
                "RangeError",
                "Invalid array buffer length: " + JsNumberFormat.ToJsString(byteLength));
        }

        var size = (int)byteLength;

        // FUEL FIRST, ALLOCATION SECOND, RETENTION REPORTED THIRD. A guest that asks for a gigabyte
        // is asking for a gigabyte of work, and charging before the `new` is what stops the request
        // from being served and then regretted; reporting the retention afterwards is what keeps
        // the live-bytes ceiling a ceiling rather than an estimate.
        engine.Charge((ulong)size);
        var buffer = new JsArrayBuffer(ArrayBufferPrototype, size);
        engine.Retain((ulong)size);
        return buffer;
    }

    /// <summary>Allocates a typed array of <paramref name="length"/> elements over fresh bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=85EAE4
    // Broiler-Human:        PENDING
    private JsTypedArray BinaryNewTypedArray(JsEngine engine, JsElementKind kind, double length)
    {
        var width = JsElements.WidthOf(kind);

        if (length < 0 || length > int.MaxValue / width ||
            length != System.Math.Floor(length))
        {
            throw engine.Error(
                "RangeError",
                "Invalid typed array length: " + JsNumberFormat.ToJsString(length));
        }

        var count = (int)length;
        var buffer = BinaryNewBuffer(engine, (double)count * width);
        return new JsTypedArray(TypedArrayPrototypes[kind], buffer, 0, count, kind);
    }

    /// <summary>The four construction forms of a typed array constructor.</summary>
    /// <remarks>
    /// <c>new X(length)</c> and <c>new X(arrayLike)</c> allocate; <c>new X(typedArray)</c> allocates
    /// and CONVERTS element by element, so <c>new Uint8Array(new Float64Array([1.7]))</c> is
    /// <c>[1]</c>; and <c>new X(buffer, byteOffset, length)</c> allocates nothing and shares the
    /// bytes, which is the form that makes two typed arrays aliases of one another and the only one
    /// whose arguments can be misaligned.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=26554F
    // Broiler-Human:        PENDING
    private JsValue BinaryConstructTypedArray(
        JsEngine engine, JsElementKind kind, JsValue[] arguments)
    {
        var name = JsElements.ConstructorNameOf(kind);
        var width = JsElements.WidthOf(kind);
        var first = ArgOfBinary(arguments, 0);

        if (first.AsObjectOrNull() is JsArrayBuffer buffer)
        {
            var offset = BinaryToIndex(engine, ArgOfBinary(arguments, 1), "byteOffset");

            if (offset % width != 0)
            {
                return engine.ThrowRangeError(
                    "start offset of " + name + " should be a multiple of " + width);
            }

            if (buffer.IsDetached)
            {
                return engine.ThrowTypeError(
                    "Cannot construct " + name + " over a detached ArrayBuffer");
            }

            if (offset > buffer.ByteLength)
            {
                return engine.ThrowRangeError(
                    "start offset " + JsNumberFormat.ToJsString(offset) +
                    " is outside the bounds of the buffer");
            }

            double count;

            if (ArgOfBinary(arguments, 2).Type == JsType.Undefined)
            {
                if (buffer.ByteLength % width != 0)
                {
                    return engine.ThrowRangeError(
                        "byte length of " + name + " should be a multiple of " + width);
                }

                count = (buffer.ByteLength - offset) / width;
            }
            else
            {
                count = BinaryToIndex(engine, arguments[2], "length");

                if (offset + (count * width) > buffer.ByteLength)
                {
                    return engine.ThrowRangeError(
                        "Invalid typed array length: " + JsNumberFormat.ToJsString(count));
                }
            }

            return JsValue.Object(new JsTypedArray(
                TypedArrayPrototypes[kind], buffer, (int)offset, (int)count, kind));
        }

        if (first.AsObjectOrNull() is JsTypedArray source)
        {
            if (source.IsDetached)
            {
                return engine.ThrowTypeError("Cannot construct " + name + " from a detached buffer");
            }

            var copied = BinaryNewTypedArray(engine, kind, source.Length);

            for (var at = 0; at < source.Length; at++)
            {
                engine.Charge(1);
                _ = source.TryReadAt(at, out var element);
                _ = copied.TryWriteAt(at, element);
            }

            return JsValue.Object(copied);
        }

        if (first.IsObject)
        {
            var length = engine.ToUint32(engine.GetProperty(first, "length"));
            var made = BinaryNewTypedArray(engine, kind, length);

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = engine.GetIndexed(first, JsValue.Number(at));
                _ = made.TryWriteAt((int)at, engine.ToNumber(element));
            }

            return JsValue.Object(made);
        }

        // A PRIMITIVE IS A LENGTH, INCLUDING A STRING ONE: `new Int8Array("4")` is four zeroes,
        // because the specification runs ToIndex over anything that is not an object.
        return JsValue.Object(BinaryNewTypedArray(engine, kind, BinaryToIndex(engine, first, "length")));
    }

    /// <summary>Copies one typed array into another, converting to the target's kind.</summary>
    /// <remarks>
    /// <b>An overlapping copy is snapshotted first.</b> The two arrays may be views onto the same
    /// buffer at different offsets, and a straight forward loop would then read elements it had
    /// already overwritten. Only the same-buffer case pays for the snapshot; the ordinary case
    /// streams straight through.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A02218
    // Broiler-Human:        PENDING
    private static JsValue BinarySetFromTypedArray(
        JsEngine engine, JsTypedArray target, JsTypedArray source, double offset)
    {
        if (source.IsDetached)
        {
            return engine.ThrowTypeError("%TypedArray%.prototype.set called on a detached buffer");
        }

        if (source.Length + offset > target.Length)
        {
            return engine.ThrowRangeError("offset is out of bounds");
        }

        var at = (int)offset;

        if (!ReferenceEquals(source.Buffer, target.Buffer))
        {
            for (var index = 0; index < source.Length; index++)
            {
                engine.Charge(1);
                _ = source.TryReadAt(index, out var element);
                _ = target.TryWriteAt(at + index, element);
            }

            return JsValue.Undefined;
        }

        var snapshot = new double[source.Length];

        for (var index = 0; index < snapshot.Length; index++)
        {
            engine.Charge(1);
            _ = source.TryReadAt(index, out snapshot[index]);
        }

        for (var index = 0; index < snapshot.Length; index++)
        {
            engine.Charge(1);
            _ = target.TryWriteAt(at + index, snapshot[index]);
        }

        return JsValue.Undefined;
    }

    /// <summary>The receiver a <c>%TypedArray%.prototype</c> member operates on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3A5A6D
    // Broiler-Human:        PENDING
    private static JsTypedArray BinaryThisTypedArray(JsEngine engine, JsValue value, string member)
    {
        if (value.AsObjectOrNull() is JsTypedArray array)
        {
            return array;
        }

        throw engine.Error(
            "TypeError",
            "%TypedArray%.prototype." + member + " requires that 'this' be a typed array");
    }

    /// <summary>
    /// The specification's <c>ValidateTypedArray</c>: a typed array whose buffer is still there.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=04D58E
    // Broiler-Human:        PENDING
    private static JsTypedArray BinaryLiveTypedArray(JsEngine engine, JsValue value, string method)
    {
        var array = BinaryThisTypedArray(engine, value, method);

        if (array.IsDetached)
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method + " called on a detached ArrayBuffer");
        }

        return array;
    }

    /// <summary>The receiver an <c>ArrayBuffer.prototype</c> member operates on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5B3BA9
    // Broiler-Human:        PENDING
    private static JsArrayBuffer BinaryThisBuffer(JsEngine engine, JsValue value, string member)
    {
        if (value.AsObjectOrNull() is JsArrayBuffer buffer)
        {
            return buffer;
        }

        throw engine.Error(
            "TypeError",
            "ArrayBuffer.prototype." + member + " requires that 'this' be an ArrayBuffer");
    }

    /// <summary>The receiver a <c>DataView.prototype</c> member operates on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=0CD39C
    // Broiler-Human:        PENDING
    private static JsDataView BinaryThisView(JsEngine engine, JsValue value, string member)
    {
        if (value.AsObjectOrNull() is JsDataView view)
        {
            return view;
        }

        throw engine.Error(
            "TypeError",
            "DataView.prototype." + member + " requires that 'this' be a DataView");
    }

    /// <summary>One <c>DataView</c> read: <c>(byteOffset[, littleEndian])</c>.</summary>
    /// <remarks>
    /// <b>The endianness argument defaults to FALSE, meaning big-endian.</b> That is the opposite
    /// of the typed arrays in this same file, which are little-endian on every host, and it is the
    /// specification's own choice rather than an accident of this implementation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E313DF
    // Broiler-Human:        PENDING
    private static JsValue BinaryViewRead(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, JsElementKind kind, string method)
    {
        var view = BinaryThisView(engine, thisValue, method);
        var at = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "byteOffset");
        var littleEndian = ArgOfBinary(arguments, 1).ToBooleanValue();

        if (view.IsDetached)
        {
            return engine.ThrowTypeError(
                "DataView.prototype." + method + " called on a detached ArrayBuffer");
        }

        if (at > int.MaxValue || !view.TryRead(kind, (int)at, littleEndian, out var value))
        {
            return engine.ThrowRangeError("Offset is outside the bounds of the DataView");
        }

        return JsValue.Number(value);
    }

    /// <summary>One <c>DataView</c> write: <c>(byteOffset, value[, littleEndian])</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A64D62
    // Broiler-Human:        PENDING
    private static JsValue BinaryViewWrite(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, JsElementKind kind, string method)
    {
        var view = BinaryThisView(engine, thisValue, method);
        var at = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "byteOffset");

        // THE VALUE IS COERCED BEFORE THE BOUNDS ARE CHECKED, which the specification requires and
        // which is observable: a `valueOf` that detaches the buffer runs, and the write that
        // follows then fails rather than writing into bytes nobody owns any more.
        var value = engine.ToNumber(ArgOfBinary(arguments, 1));
        var littleEndian = ArgOfBinary(arguments, 2).ToBooleanValue();

        if (view.IsDetached)
        {
            return engine.ThrowTypeError(
                "DataView.prototype." + method + " called on a detached ArrayBuffer");
        }

        if (at > int.MaxValue || !view.TryWrite(kind, (int)at, value, littleEndian))
        {
            return engine.ThrowRangeError("Offset is outside the bounds of the DataView");
        }

        return JsValue.Undefined;
    }

    /// <summary>The callback an iteration method was given, which has to be callable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=C81269
    // Broiler-Human:        PENDING
    private static JsValue BinaryCallbackOf(JsEngine engine, JsValue[] arguments, string method)
    {
        var callback = ArgOfBinary(arguments, 0);

        if (!callback.IsObject || !callback.AsObject().IsCallable)
        {
            return engine.ThrowTypeError(
                "%TypedArray%.prototype." + method + " requires a callback function");
        }

        return callback;
    }

    /// <summary>One comparison during a sort.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3802B0
    // Broiler-Human:        PENDING
    private static int BinaryCompareElements(
        JsEngine engine, JsValue comparator, double left, double right)
    {
        engine.Charge(1);

        if (comparator.IsObject)
        {
            var ordering = engine.ToNumber(engine.Call(
                comparator, JsValue.Undefined, [JsValue.Number(left), JsValue.Number(right)]));

            if (double.IsNaN(ordering) || ordering == 0)
            {
                return 0;
            }

            return ordering < 0 ? -1 : 1;
        }

        return BinaryCompareNumbers(left, right);
    }

    /// <summary>The default numeric order: ascending, NaN last, negative zero before positive.</summary>
    /// <remarks>
    /// The two tails are the whole difference between this and a bare <c>&lt;</c>. NaN compares
    /// false against everything, so a comparison that only asked <c>&lt;</c> would leave NaNs
    /// wherever the merge happened to put them; and negative zero equals positive zero, so their
    /// order has to be decided by the sign bit or a sort would be free to reorder them and
    /// <c>Object.is</c> would see the difference afterwards.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=27ECF3
    // Broiler-Human:        PENDING
    private static int BinaryCompareNumbers(double left, double right)
    {
        if (double.IsNaN(left))
        {
            return double.IsNaN(right) ? 0 : 1;
        }

        if (double.IsNaN(right))
        {
            return -1;
        }

        if (left < right)
        {
            return -1;
        }

        if (left > right)
        {
            return 1;
        }

        if (left == 0 && right == 0 && double.IsNegative(left) != double.IsNegative(right))
        {
            return double.IsNegative(left) ? -1 : 1;
        }

        return 0;
    }

    /// <summary>A stable merge sort of <paramref name="items"/> over the half-open range.</summary>
    /// <remarks>
    /// Stable for the reason <see cref="ArrayMergeSort"/> is stable - the merge takes from the left
    /// run on a tie - and stability is observable here too: a comparator that answers zero for two
    /// distinct bit patterns, such as a negative and a positive zero, must leave them where they
    /// were.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=E5752E
    // Broiler-Human:        PENDING
    private static void BinaryMergeSort(
        JsEngine engine,
        JsValue comparator,
        System.Collections.Generic.List<double> items,
        System.Collections.Generic.List<double> buffer,
        int from,
        int to)
    {
        if (to - from < 2)
        {
            return;
        }

        var middle = from + ((to - from) / 2);
        BinaryMergeSort(engine, comparator, items, buffer, from, middle);
        BinaryMergeSort(engine, comparator, items, buffer, middle, to);

        var left = from;
        var right = middle;
        var written = from;

        while (left < middle && right < to)
        {
            buffer[written++] = BinaryCompareElements(engine, comparator, items[left], items[right]) <= 0
                ? items[left++]
                : items[right++];
        }

        while (left < middle)
        {
            buffer[written++] = items[left++];
        }

        while (right < to)
        {
            buffer[written++] = items[right++];
        }

        for (var at = from; at < to; at++)
        {
            items[at] = buffer[at];
        }
    }
}
