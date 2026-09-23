// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   38
// Annotated:        38/38
// Exempt:           7
// Human-reviewed:   0/38
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  4/10 max
// Unverified:       38
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The binary surface: <c>ArrayBuffer</c>, <c>DataView</c>, and the twelve typed arrays under their
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
/// <b><c>BigInt64Array</c>, <c>BigUint64Array</c> and the four <c>DataView</c> BigInt accessors
/// exist where BigInt does, and nowhere else.</b> (Since 2026-09-22, JSeal B07-B08.) Their elements
/// are BigInt values, so they are built only when the composition admits the BigInt surface as well
/// as this one; a composition that declines BigInt still gets the ten Number kinds and a
/// <c>DataView</c> without the four, and no BigInt value can arise in its realms. Every method on
/// <c>%TypedArray%.prototype</c> converts by the receiver's content type - <c>ToBigInt</c> for the
/// two BigInt kinds, <c>ToNumber</c> for the ten - and copying between the two content types is a
/// <c>TypeError</c>, never a conversion. <i>(This paragraph read "absent for a smaller reason: this
/// realm has no BigInt" until card B05 admitted BigInt; it is quoted rather than deleted.)</i>
/// </para>
/// <para>
/// <b>Every method that touches a buffer re-checks detachment.</b> Detaching is reachable in the
/// language through <c>ArrayBuffer.prototype.transfer</c>, and a <c>valueOf</c> called during a
/// built-in's own argument coercion can do it between two lines of that built-in. So nothing here
/// caches "the bytes"; the views ask their buffer on every access and the built-ins ask again
/// after every step that could have run guest code.
/// </para>
/// <para>
/// <b>A resizable buffer makes "is it still there" into "is it still long enough".</b> (Since
/// 2026-09-21, JSeal F04-F06.) <c>new ArrayBuffer(n, { maxByteLength })</c>, <c>resize</c>,
/// <c>resizable</c>, <c>maxByteLength</c>, <c>detached</c> and <c>transferToFixedLength</c> are
/// published together with the view rules they need: a view built without a length over such a
/// buffer tracks it, a view built with one is out of bounds while the buffer is shorter than its
/// end, and every re-check a built-in made for a detach now asks <c>IsOutOfBounds</c>, which covers
/// both. Where the specification measures a receiver once and then runs guest code - a callback,
/// a coercion, a species constructor - the method measures once too and reads each index through
/// the view, so a shrink yields <c>undefined</c> for the indices it took away and a growth adds no
/// visits. There is still no growable <c>SharedArrayBuffer</c>, for the reason above.
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
    /// <c>%TypedArray%.prototype</c>: the object the element prototypes inherit from.
    /// </summary>
    /// <remarks>
    /// It carries every shared method exactly once, which is what makes
    /// <c>Object.getPrototypeOf(Int8Array.prototype) === Object.getPrototypeOf(Float64Array.prototype)</c>
    /// true and what lets a program hang a helper on every kind at once - a shape the specification
    /// exposes deliberately, even though <c>%TypedArray%</c> itself has no global name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1A07EA
    // Broiler-Human:        PENDING
    internal JsObject TypedArrayPrototype { get; private set; } = null!;

    /// <summary>The per-kind prototypes, by the kind whose constructor owns each.</summary>
    /// <remarks>
    /// Every allocation of a view of a kind learned only at run time - a constructor, <c>from</c>,
    /// <c>of</c>, <c>toSorted</c> - needs to reach the prototype for that kind. A dictionary keyed
    /// by the kind is what makes each of those one line instead of a ten-armed switch. (<c>map</c>,
    /// <c>filter</c>, <c>slice</c> and <c>subarray</c> no longer allocate here: they answer
    /// through the receiver's species, whose default is the intrinsic constructor of its kind.)
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B1F978
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<JsElementKind, JsObject> TypedArrayPrototypes { get; } =
        new();

    /// <summary>The intrinsic typed array constructors, by the kind each builds.</summary>
    /// <remarks>
    /// The species algorithms fall back to the constructor of the RECEIVER'S kind when the
    /// receiver names no species, and that fallback is the intrinsic object - not whatever a
    /// program has since assigned to the global of the same name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=087805
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsElementKind, JsObject> typedArrayConstructors =
        new();

    /// <summary>The intrinsic <c>%ArrayBuffer%</c>: <c>ArrayBuffer.prototype.slice</c>'s species default.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=86A657
    // Broiler-Human:        PENDING
    private JsObject arrayBufferConstructor = null!;

    /// <summary>
    /// Whether this realm builds the two BigInt element kinds and the <c>DataView</c> BigInt
    /// accessors: its composition admits the BigInt surface as well as the binary one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E59042
    // Broiler-Human:        PENDING
    private bool binaryHoldsBigInts;

    /// <summary>Builds the whole binary surface, in dependency order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FB7AEB
    // Broiler-Human:        PENDING
    private void SetupBinary()
    {
        // THE BIGINT KINDS NEED BOTH SURFACES. An element read makes a BigInt value, which a realm
        // whose composition declined BigInt may never hold (JSD-0033); the artifact that names
        // either constructor declares both surfaces, so a declining composition refuses it at
        // verification and this realm simply does not build them.
        binaryHoldsBigInts = engine.Admits(Format.JsSurfaces.BigInt);

        ArrayBufferPrototype = new JsObject(ObjectPrototype, "ArrayBuffer");
        DataViewPrototype = new JsObject(ObjectPrototype, "DataView");
        TypedArrayPrototype = new JsObject(ObjectPrototype, "TypedArray");

        SetupArrayBuffer();
        SetupDataView();
        SetupTypedArrayAccessors();
        SetupTypedArrayMutators();
        SetupTypedArrayReaders();
        SetupTypedArrayIteration();
        SetupTypedArrayLaterAdditions();
        SetupTypedArrayConstructors();
        SetupBinaryTags();
    }

    /// <summary>
    /// The binary brands' <c>Symbol.toStringTag</c> (JSeal B06): a data property on
    /// <c>ArrayBuffer.prototype</c> and <c>DataView.prototype</c>. The <c>%TypedArray%.prototype</c>
    /// getter that answers the receiver's <c>[[TypedArrayName]]</c> is installed with the other
    /// typed-array accessors.
    /// </summary>
    /// <remarks>
    /// <b>The realm had left these to the class name</b>, which <c>Object.prototype.toString</c> no
    /// longer consults for any kind the specification does not list as a builtin tag. (B06 and B07
    /// each added the typed-array getter; the merged tree keeps the one in
    /// <see cref="SetupTypedArrayAccessors"/>.)
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=84793A
    // Broiler-Human:        PENDING
    private void SetupBinaryTags()
    {
        ArrayBufferPrototype.SetOwnSymbol(
            ToStringTagSymbol, JsProperty.Data(JsValue.String("ArrayBuffer"), JsPropertyAttributes.Configurable));

        DataViewPrototype.SetOwnSymbol(
            ToStringTagSymbol, JsProperty.Data(JsValue.String("DataView"), JsPropertyAttributes.Configurable));
    }

    /// <summary>Builds <c>ArrayBuffer</c>, its one static and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=F74A76
    // Broiler-Human:        PENDING
    private void SetupArrayBuffer()
    {
        var constructor = Constructor(
            "ArrayBuffer",
            1,
            ArrayBufferPrototype,
            static (engine, thisValue, arguments) =>
                engine.ThrowTypeError("Constructor ArrayBuffer requires 'new'"),
            (engine, thisValue, arguments) =>
            {
                // THE LENGTH, THEN THE OPTIONS BAG, THEN THE ALLOCATION: the specification's order,
                // and observable, because both conversions can run guest code. A bag without a
                // `maxByteLength`, or no bag at all, is a fixed-length buffer exactly as before.
                var byteLength = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "length");
                var maxByteLength = BinaryMaxByteLengthOption(engine, ArgOfBinary(arguments, 1));

                // THE OBJECT BEFORE THE DATA BLOCK (AllocateArrayBuffer, ES2026 25.1.3.1): a length
                // past the maximum is refused first, then `new.target`'s `prototype` is read - a
                // getter there runs, and its throw wins over the RangeError an impossible
                // allocation answers - and only then is the block allocated. `thisValue` is
                // `new.target` on this path, so the constructor builds from it itself.
                if (maxByteLength is { } bound && byteLength > bound)
                {
                    throw engine.Error(
                        "RangeError",
                        "Invalid array buffer length: " + JsNumberFormat.ToJsString(byteLength) +
                        " exceeds the maximum byte length " + JsNumberFormat.ToJsString(bound));
                }

                var prototype = BinaryPrototypeFrom(engine, thisValue, ArrayBufferPrototype);

                var made = maxByteLength is { } max
                    ? BinaryNewResizableBuffer(engine, byteLength, max)
                    : BinaryNewBuffer(engine, byteLength);

                made.Prototype = prototype;
                return JsValue.Object(made);
            });

        constructor.BuildsFromNewTarget = true;

        // `isView` ANSWERS FOR BOTH VIEW KINDS AND FOR NOTHING ELSE. It is not "is this backed by a
        // buffer" - an ArrayBuffer itself answers false, which is the question callers actually
        // need answered before they reach for `byteOffset`.
        SpeciesGetter(constructor);
        arrayBufferConstructor = constructor;

        Method(constructor, "isView", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(ArgOfBinary(arguments, 0).AsObjectOrNull() is JsTypedArray or JsDataView));

        BinaryGetter(ArrayBufferPrototype, "byteLength", static (engine, thisValue, arguments) =>
            JsValue.Number(BinaryThisBuffer(engine, thisValue, "byteLength").ByteLength));

        // THE THREE GETTERS A RESIZABLE BUFFER NEEDS, AND `detached`. A fixed-length buffer answers
        // its byte length for `maxByteLength`, a detached one zero; `resizable` is a property of
        // how the buffer was built and survives the detach; `detached` is the one question a
        // program could otherwise only answer by catching the TypeError of some other member.
        BinaryGetter(ArrayBufferPrototype, "maxByteLength", static (engine, thisValue, arguments) =>
        {
            var buffer = BinaryThisBuffer(engine, thisValue, "maxByteLength");

            return JsValue.Number(buffer.IsDetached ? 0 : buffer.MaxByteLength ?? buffer.ByteLength);
        });

        BinaryGetter(ArrayBufferPrototype, "resizable", static (engine, thisValue, arguments) =>
            JsValue.Boolean(BinaryThisBuffer(engine, thisValue, "resizable").IsResizable));

        BinaryGetter(ArrayBufferPrototype, "detached", static (engine, thisValue, arguments) =>
            JsValue.Boolean(BinaryThisBuffer(engine, thisValue, "detached").IsDetached));

        // `resize` BELONGS TO A RESIZABLE BUFFER AND TO NOTHING ELSE. A fixed-length receiver is
        // refused as a wrong receiver - the specification's RequireInternalSlot on the maximum -
        // before the length is even converted; a detach during that conversion is a TypeError and a
        // length past the maximum a RangeError, and none of the three changes the buffer.
        Method(ArrayBufferPrototype, "resize", 1, (engine, thisValue, arguments) =>
        {
            if (thisValue.AsObjectOrNull() is not JsArrayBuffer { IsResizable: true } buffer)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.resize requires that 'this' be a resizable ArrayBuffer");
            }

            var requested = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "length");

            if (buffer.IsDetached)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.resize called on a detached ArrayBuffer");
            }

            if (requested > buffer.MaxByteLength)
            {
                return engine.ThrowRangeError(
                    "ArrayBuffer.prototype.resize: " + JsNumberFormat.ToJsString(requested) +
                    " exceeds the maximum byte length");
            }

            BinaryResizeBuffer(engine, buffer, (int)requested);
            return JsValue.Undefined;
        });

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

            // THE RESULT COMES FROM THE SPECIES AND IS VALIDATED BEFORE A BYTE MOVES. The species
            // constructor is guest code: it can answer something that is not a buffer, the
            // receiver itself, a buffer too small for the slice, a detached buffer, or it can
            // detach the receiver while it runs. Each of those is a TypeError, and every one is
            // checked before the copy, so a refused result is never written into.
            var constructor = BinarySpeciesConstructor(engine, thisValue, arrayBufferConstructor);
            var constructed = engine.Construct(constructor, [JsValue.Number(count)]);

            if (constructed.AsObjectOrNull() is not JsArrayBuffer made)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice: the species constructor did not return an ArrayBuffer");
            }

            if (made.IsDetached)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice: the species constructor returned a detached ArrayBuffer");
            }

            if (ReferenceEquals(made, buffer))
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice: the species constructor returned the receiver itself");
            }

            if (made.ByteLength < count)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice: the species constructor returned a buffer smaller " +
                    "than the slice");
            }

            // THE RECEIVER IS RE-READ AFTER THE CONSTRUCTION AND THE COERCIONS. `ArrayRelative`
            // calls `ToInteger`, which can run a `valueOf`, and the species constructor is guest
            // code; either can transfer the receiver away, and the bytes this copies must be the
            // ones that are there now rather than the ones that were.
            if (buffer.IsDetached)
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice called on a detached ArrayBuffer");
            }

            // A RECEIVER SHRUNK PAST THE START COPIES NOTHING AND IS NOT AN ERROR. The new buffer
            // was sized from the length before the species ran; the copy is the longest prefix
            // the receiver still holds, which may be none of it (the specification's `first <
            // currentLen` test).
            var available = buffer.ByteLength - start;
            var copied = (int)System.Math.Max(0, System.Math.Min(count, available));

            if (copied == 0)
            {
                return constructed;
            }

            engine.Charge((ulong)copied);

            if (!made.TryCopyFrom(buffer.Data, (int)start, copied))
            {
                return engine.ThrowTypeError(
                    "ArrayBuffer.prototype.slice called on a detached ArrayBuffer");
            }

            return constructed;
        });

        // THE ONLY WAY TO DETACH A BUFFER FROM INSIDE THE LANGUAGE. Without it the detached state
        // would be unreachable and untestable, and every "is it detached" branch in this file and
        // in JsBinary.cs would be dead code that nothing could exercise.
        //
        // `transfer` KEEPS THE RECEIVER'S RESIZABILITY AND `transferToFixedLength` DROPS IT; the two
        // are otherwise one algorithm, the specification's ArrayBufferCopyAndDetach.
        Method(ArrayBufferPrototype, "transfer", 0, (engine, thisValue, arguments) =>
            BinaryCopyAndDetach(engine, thisValue, arguments, preserveResizability: true, "transfer"));

        Method(ArrayBufferPrototype, "transferToFixedLength", 0, (engine, thisValue, arguments) =>
            BinaryCopyAndDetach(
                engine, thisValue, arguments, preserveResizability: false, "transferToFixedLength"));
    }

    /// <summary>
    /// The specification's <c>ArrayBufferCopyAndDetach</c>: <c>transfer</c> and
    /// <c>transferToFixedLength</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The length is converted before the receiver is asked whether it is detached</b>, which is
    /// the specification's order and was not this method's until 2026-09-21: a <c>valueOf</c> that
    /// detaches the receiver meets the detached-buffer <c>TypeError</c>, and a receiver that was
    /// already detached still has its argument converted first.
    /// </para>
    /// <para>
    /// <b>ALLOCATE FIRST, DETACH SECOND.</b> A refused allocation - a length past a preserved
    /// maximum, a length this runtime cannot hold, a refused charge - must leave the receiver alive:
    /// detaching and then failing would destroy the bytes on the way to reporting that there was
    /// nowhere to put them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FF3A3C
    // Broiler-Human:        PENDING
    private JsValue BinaryCopyAndDetach(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, bool preserveResizability, string method)
    {
        var buffer = BinaryThisBuffer(engine, thisValue, method);
        var requested = ArgOfBinary(arguments, 0).Type == JsType.Undefined
            ? buffer.ByteLength
            : BinaryToIndex(engine, arguments[0], "length");

        if (buffer.IsDetached)
        {
            return engine.ThrowTypeError(
                "ArrayBuffer.prototype." + method + " called on a detached ArrayBuffer");
        }

        var made = preserveResizability && buffer.MaxByteLength is { } max
            ? BinaryNewResizableBuffer(engine, requested, max)
            : BinaryNewBuffer(engine, requested);

        var released = buffer.Detach();
        var carried = released is null
            ? 0
            : System.Math.Min(made.ByteLength, released.Length);

        engine.Charge((ulong)carried);
        _ = made.TryCopyFrom(released, 0, carried);
        return JsValue.Object(made);
    }

    /// <summary>
    /// Builds <c>DataView</c>, its prototype and the twenty-two accessors (eighteen where BigInt is
    /// declined).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=CE3326
    // Broiler-Human:        PENDING
    private void SetupDataView()
    {
        var dataView = Constructor(
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

                // THE LENGTH IS MEASURED ONCE, BEFORE `byteLength` IS CONVERTED, and the first range
                // check is against that measurement (the constructor's steps 5 and 9.b). A
                // `valueOf` on `byteLength` can grow a resizable buffer, which must not admit a
                // view the buffer was too short for when it was measured, or detach the buffer,
                // which must reach the TypeError after the prototype read rather than a RangeError
                // against a length of zero.
                var bufferByteLength = buffer.ByteLength;

                if (offset > bufferByteLength)
                {
                    return engine.ThrowRangeError(
                        "Start offset " + JsNumberFormat.ToJsString(offset) +
                        " is outside the bounds of the buffer");
                }

                // AN OMITTED LENGTH OVER A RESIZABLE BUFFER IS `auto`, NOT THE LENGTH NOW: the view
                // tracks the buffer from here on. Over a fixed-length buffer it is the rest of the
                // buffer, which can never change except to nothing.
                var lengthGiven = ArgOfBinary(arguments, 2).Type != JsType.Undefined;
                double length;

                if (!lengthGiven)
                {
                    length = bufferByteLength - offset;
                }
                else
                {
                    length = BinaryToIndex(engine, arguments[2], "byteLength");

                    if (offset + length > bufferByteLength)
                    {
                        return engine.ThrowRangeError(
                            "Invalid DataView length " + JsNumberFormat.ToJsString(length));
                    }
                }

                // THE PROTOTYPE IS READ HERE, AFTER THE ARGUMENTS AND BEFORE THE SECOND ROUND OF
                // CHECKS (OrdinaryCreateFromConstructor, then IsDetachedBuffer and both ranges
                // again): a `prototype` getter on `new.target` can detach the buffer, which must
                // meet the TypeError, or shrink a resizable one, which must meet the RangeError
                // measured against the length the buffer has now.
                var prototype = BinaryPrototypeFrom(engine, thisValue, DataViewPrototype);

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

                if (lengthGiven && offset + length > buffer.ByteLength)
                {
                    return engine.ThrowRangeError(
                        "Invalid DataView length " + JsNumberFormat.ToJsString(length));
                }

                int? viewLength = lengthGiven || !buffer.IsResizable
                    ? (int)length
                    : null;

                return JsValue.Object(
                    new JsDataView(prototype, buffer, (int)offset, viewLength));
            });

        dataView.BuildsFromNewTarget = true;

        BinaryGetter(DataViewPrototype, "buffer", static (engine, thisValue, arguments) =>
            JsValue.Object(BinaryThisView(engine, thisValue, "buffer").Buffer));

        BinaryGetter(DataViewPrototype, "byteLength", static (engine, thisValue, arguments) =>
        {
            var view = BinaryThisView(engine, thisValue, "byteLength");

            // DETACHED OR OUT OF BOUNDS, THE ANSWER IS A TypeError and not a zero: unlike a typed
            // array's getters, a DataView's are specified to throw for both.
            return view.IsOutOfBounds
                ? engine.ThrowTypeError("Cannot read byteLength of a detached or out-of-bounds DataView")
                : JsValue.Number(view.ByteLength);
        });

        BinaryGetter(DataViewPrototype, "byteOffset", static (engine, thisValue, arguments) =>
        {
            var view = BinaryThisView(engine, thisValue, "byteOffset");

            return view.IsOutOfBounds
                ? engine.ThrowTypeError("Cannot read byteOffset of a detached or out-of-bounds DataView")
                : JsValue.Number(view.ByteOffset);
        });

        foreach (var kind in JsElements.All)
        {
            // A CLAMPED BYTE IS NOT A WIRE FORMAT. `Uint8ClampedArray` exists to hold image samples
            // and has no `getUint8Clamped` counterpart here, exactly as it has none in the
            // specification.
            //
            // THE BIGINT ACCESSORS ARE THE SAME PATH AS THE OTHER SIXTEEN (JSeal B08): ToIndex of the
            // offset, then the value converted - ToBigInt for them - then the endianness, then the
            // detached and bounds checks; only the conversion and the element's type differ.
            if (kind == JsElementKind.Uint8Clamped ||
                (JsElements.HoldsBigInts(kind) && !binaryHoldsBigInts))
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

    /// <summary>The four getters every typed array inherits, and its <c>[Symbol.toStringTag]</c>.</summary>
    /// <remarks>
    /// <b>Three of them answer zero for a detached buffer, and for a view a resize has left out of
    /// bounds, rather than throwing.</b> A program that
    /// has just had a buffer transferred out from under it should be able to ASK - <c>if
    /// (view.length === 0)</c> - without wrapping the question in a <c>try</c>. <c>buffer</c> is
    /// the exception and still answers the buffer, because the detached buffer is precisely what a
    /// caller in that state needs to name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EFDA8A
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
            return JsValue.Number(array.IsOutOfBounds ? 0 : array.ByteOffset);
        });

        BinaryGetter(TypedArrayPrototype, "length", static (engine, thisValue, arguments) =>
        {
            return JsValue.Number(BinaryThisTypedArray(engine, thisValue, "length").Length);
        });

        // `[Symbol.toStringTag]` IS A GETTER THAT NEVER THROWS: the view's constructor name, and
        // `undefined` for anything that is not a typed array - the receiver test itself is the
        // answer, which is what lets a program ask it of an arbitrary value. It reads the element
        // kind, so a detached or out-of-bounds view keeps its name and a kind a later card adds is
        // named by the table its constructor is. (Added 2026-09-22 by JSeal B07, and by B06 in
        // SetupBinaryTags; the merge keeps this one. The prototype had none, and
        // `view[Symbol.toStringTag]` answered `undefined` for every kind, while
        // `Object.prototype.toString` read the class name instead.)
        TypedArrayPrototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Accessor(
                Native("get [Symbol.toStringTag]", 0, static (engine, thisValue, arguments) =>
                {
                    _ = engine;
                    _ = arguments;
                    return thisValue.AsObjectOrNull() is JsTypedArray view
                        ? JsValue.String(JsElements.ConstructorNameOf(view.Kind))
                        : JsValue.Undefined;
                }),
                null,
                JsPropertyAttributes.Configurable));
    }

    /// <summary><c>set</c>, <c>subarray</c>, <c>slice</c>, <c>fill</c>, <c>copyWithin</c>, <c>reverse</c>, <c>sort</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=91A2C5
    // Broiler-Human:        PENDING
    private void SetupTypedArrayMutators()
    {
        Method(TypedArrayPrototype, "set", 1, (engine, thisValue, arguments) =>
        {
            // THE OFFSET IS CONVERTED BEFORE THE TARGET IS VALIDATED (SetTypedArrayFromTypedArray
            // and SetTypedArrayFromArrayLike both begin by measuring the target), so a `valueOf`
            // that detaches or shrinks the target meets the TypeError rather than running after it.
            var target = BinaryThisTypedArray(engine, thisValue, "set");
            var offset = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (offset < 0)
            {
                return engine.ThrowRangeError("offset is out of bounds");
            }

            if (target.IsDetached)
            {
                return engine.ThrowTypeError("%TypedArray%.prototype.set called on a detached ArrayBuffer");
            }

            var source = ArgOfBinary(arguments, 0);

            if (source.AsObjectOrNull() is JsTypedArray typed)
            {
                return BinarySetFromTypedArray(engine, target, typed, offset);
            }

            // THE TARGET'S LENGTH IS TAKEN HERE, BEFORE THE SOURCE'S `length` IS READ: a getter
            // there that resizes the target does not move the bound it is measured against, and
            // the writes below are each discarded if their index has stopped being valid.
            var targetLength = BinaryValidTypedArray(engine, target, "set").Length;

            var host = source.IsNullish
                ? engine.ThrowTypeError("%TypedArray%.prototype.set requires an array-like source")
                : source.IsObject ? source : JsValue.Object(engine.ToObject(source));

            // `LengthOfArrayLike`, which is `ToLength`: a negative length is an empty source and
            // not the 4294967295 elements `ToUint32` made of it.
            var length = ArrayLengthOf(engine, host);

            if (length + offset > targetLength)
            {
                return engine.ThrowRangeError("offset is out of bounds");
            }

            // EACH ELEMENT IS CONVERTED BY THE TARGET'S CONTENT TYPE, so a Number bound for a
            // BigInt64Array is the TypeError ToBigInt owes it, raised at that element (JSeal B07).
            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = engine.ToElementValue(
                    target.Kind, engine.GetIndexed(host, JsValue.Number(at)));
                _ = target.TryWriteAt((int)(at + offset), element);
            }

            return JsValue.Undefined;
        });

        // `subarray` SHARES THE BYTES AND `slice` COPIES THEM. Both answer through the receiver's
        // species; the difference is that writing through a subarray is visible through the array
        // it came from, which is the whole reason both exist. So `subarray` hands its species the
        // receiver's own BUFFER with a byte offset and a length, and `slice` hands it a COUNT and
        // then copies into whatever came back. Neither is the other with a different name.
        Method(TypedArrayPrototype, "subarray", 2, (engine, thisValue, arguments) =>
        {
            // NO LIVENESS CHECK HERE, which is the specification's choice: a detached or
            // out-of-bounds receiver has a length of zero, and it is the construction over its
            // buffer that refuses or accepts.
            var array = BinaryThisTypedArray(engine, thisValue, "subarray");
            var buffer = array.Buffer;
            var length = array.Length;
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var beginByteOffset = array.ByteOffset + (start * array.BytesPerElement);

            // A LENGTH-TRACKING RECEIVER WITH NO END MAKES A LENGTH-TRACKING RESULT: the species is
            // handed only the buffer and the offset, so the subarray keeps following the buffer
            // exactly as the array it came from does.
            if (array.TracksLength && ArgOfBinary(arguments, 1).Type == JsType.Undefined)
            {
                return JsValue.Object(BinaryTypedArraySpeciesCreate(
                    engine,
                    thisValue,
                    array,
                    [JsValue.Object(buffer), JsValue.Number(beginByteOffset)],
                    "subarray"));
            }

            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[1], length);

            var count = stop > start ? stop - start : 0;

            return JsValue.Object(BinaryTypedArraySpeciesCreate(
                engine,
                thisValue,
                array,
                [JsValue.Object(buffer), JsValue.Number(beginByteOffset), JsValue.Number(count)],
                "subarray"));
        });

        Method(TypedArrayPrototype, "slice", 2, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "slice");
            var length = array.Length;
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 0), length);
            var stop = ArgOfBinary(arguments, 1).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[1], length);

            var count = stop > start ? (int)(stop - start) : 0;
            var made = BinaryTypedArraySpeciesCreate(
                engine, thisValue, array, [JsValue.Number(count)], "slice");

            if (count == 0)
            {
                return JsValue.Object(made);
            }

            // THE RECEIVER IS RE-CHECKED AFTER THE SPECIES RAN. The constructor is guest code and
            // may have transferred the receiver's buffer away or shrunk it below the view; a
            // slice with nothing to copy does not care, and one with something to copy is a
            // TypeError rather than a result full of zeroes that looks like a copy. A receiver
            // that is merely shorter now copies the prefix it still has.
            if (array.IsOutOfBounds)
            {
                return engine.ThrowTypeError(
                    "%TypedArray%.prototype.slice called on a detached or out-of-bounds typed array");
            }

            var end = System.Math.Min((int)stop, array.Length);
            count = System.Math.Max(end - (int)start, 0);
            engine.Charge((ulong)count);

            // THE SAME ELEMENT TYPE COPIES BYTES; ANY OTHER CONVERTS ELEMENT BY ELEMENT. "Same" is
            // the kind, never the width: an Int8Array and a Uint8Array are one byte each and still
            // two element types, and a Float32Array and an Int32Array share a width and nothing
            // else. The byte copy runs FORWARD one byte at a time because the species may have
            // answered a view over the receiver's own buffer at an overlapping offset, and the
            // specification defines the result of that overlap as exactly this loop's result.
            if (made.Kind == array.Kind)
            {
                var source = array.Buffer.Data;
                var target = made.Buffer.Data;

                if (source is null || target is null)
                {
                    return engine.ThrowTypeError(
                        "%TypedArray%.prototype.slice called on a detached ArrayBuffer");
                }

                var width = array.BytesPerElement;
                var from = array.ByteOffset + ((int)start * width);
                var to = made.ByteOffset;
                var limit = to + (count * width);

                while (to < limit)
                {
                    target[to++] = source[from++];
                }

                return JsValue.Object(made);
            }

            for (var at = 0; at < count; at++)
            {
                _ = array.TryReadAt((int)start + at, out var element);
                _ = made.TryWriteAt(at, element);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "fill", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "fill");
            var length = array.Length;

            // THE VALUE FIRST, BY THE CONTENT TYPE: ToBigInt for a BigInt kind, before either index
            // is converted, which is the specification's order and observable through `valueOf`.
            var value = engine.ToElementValue(array.Kind, ArgOfBinary(arguments, 0));
            var start = ArrayRelative(engine, ArgOfBinary(arguments, 1), length);
            var stop = ArgOfBinary(arguments, 2).Type == JsType.Undefined
                ? length
                : ArrayRelative(engine, arguments[2], length);

            // THE THREE COERCIONS CAN RUN GUEST CODE, AND GUEST CODE CAN DETACH THE BUFFER OR
            // RESIZE IT. The specification revalidates the receiver after the last of them and
            // throws when it is gone or out of bounds, rather than letting the writes below fall
            // silently on bytes that are gone; a receiver that is merely shorter now is filled up
            // to its new end, and one that grew is filled only as far as the old one reached.
            if (array.IsOutOfBounds)
            {
                return engine.ThrowTypeError(
                    "%TypedArray%.prototype.fill: the ArrayBuffer was detached or resized out of bounds while its arguments were converted");
            }

            stop = System.Math.Min(stop, array.Length);

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
                // The same revalidation `fill` makes, and only where there is something to move,
                // which is where the specification places it. A receiver the coercions shrank
                // copies "the longest still-applicable prefix": the count is clamped to what both
                // the source and the target range still hold, and may reach nothing.
                if (array.IsOutOfBounds)
                {
                    return engine.ThrowTypeError(
                        "%TypedArray%.prototype.copyWithin: the ArrayBuffer was detached or resized out of bounds while its arguments were converted");
                }

                length = array.Length;
                count = System.Math.Min(count, System.Math.Min(length - from, length - to));

                if (count > 0)
                {
                    engine.Charge((ulong)count);
                    _ = array.TryCopyWithin(to, from, count);
                }
            }

            return thisValue;
        });

        Method(TypedArrayPrototype, "reverse", 0, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "reverse");
            var length = array.Length;
            var middle = length / 2;

            for (var lower = 0; lower < middle; lower++)
            {
                engine.Charge(1);
                var upper = (length - lower) - 1;
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
            var items = new System.Collections.Generic.List<JsValue>(array.Length);

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
                var buffer = new System.Collections.Generic.List<JsValue>(items);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1BACC8
    // Broiler-Human:        PENDING
    private void SetupTypedArrayReaders()
    {
        // THE THREE SEARCHES MEASURE THE RECEIVER ONCE, BEFORE `fromIndex` IS CONVERTED, and
        // search that many indices whatever the conversion did to the buffer. `indexOf` and
        // `lastIndexOf` skip an index that is no longer valid (their HasProperty step), so a
        // shrink can never make them report an index whose element is gone; `includes` reads
        // every index, and an index a shrink took away reads `undefined` - which is what makes
        // `includes(undefined)` true after such a shrink, as the specification says it is.
        Method(TypedArrayPrototype, "indexOf", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "indexOf");
            var length = array.Length;

            if (length == 0)
            {
                return JsValue.Number(-1);
            }

            var wanted = ArgOfBinary(arguments, 0);
            var from = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (from >= length)
            {
                return JsValue.Number(-1);
            }

            var start = from >= 0 ? from : System.Math.Max(length + from, 0);

            for (var at = (int)start; at < length; at++)
            {
                engine.Charge(1);

                if (array.TryReadAt(at, out var element) && element.StrictlyEquals(wanted))
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(TypedArrayPrototype, "lastIndexOf", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "lastIndexOf");
            var length = array.Length;

            if (length == 0)
            {
                return JsValue.Number(-1);
            }

            var wanted = ArgOfBinary(arguments, 0);
            var from = arguments.Length > 1
                ? engine.ToInteger(arguments[1])
                : length - 1;

            var start = from >= 0 ? System.Math.Min(from, length - 1) : length + from;

            if (start < 0)
            {
                return JsValue.Number(-1);
            }

            for (var at = (int)start; at >= 0; at--)
            {
                engine.Charge(1);

                if (array.TryReadAt(at, out var element) && element.StrictlyEquals(wanted))
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
            var length = array.Length;

            if (length == 0)
            {
                return JsValue.False;
            }

            var wanted = ArgOfBinary(arguments, 0);
            var from = engine.ToInteger(ArgOfBinary(arguments, 1));

            if (from >= length)
            {
                return JsValue.False;
            }

            var start = from >= 0 ? from : System.Math.Max(length + from, 0);

            for (var at = (int)start; at < length; at++)
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

            // MEASURED BEFORE THE SEPARATOR IS CONVERTED, and that many elements are joined: a
            // `toString` on the separator that shrinks the buffer leaves empty places, one that
            // grows it adds none.
            var length = array.Length;
            var separatorValue = ArgOfBinary(arguments, 0);
            var separator = separatorValue.Type == JsType.Undefined
                ? ","
                : engine.ToStringValue(separatorValue);

            var text = new System.Text.StringBuilder();

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (at > 0)
                {
                    text.Append(separator);
                }

                // AN ELEMENT IS ALWAYS A NUMBER UNLESS THE BUFFER WENT AWAY OR SHRANK. There are no
                // holes in a typed array, so the only way this reads `undefined` is a detach or a
                // resize that happened during the separator's own coercion, and `undefined` renders
                // as nothing.
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
            // THE LENGTH IS THE ONE BEFORE THE INDEX IS CONVERTED; the element is read after, and
            // answers `undefined` if the conversion took it away.
            var array = BinaryLiveTypedArray(engine, thisValue, "at");
            var length = array.Length;
            var relative = engine.ToInteger(ArgOfBinary(arguments, 0));
            var at = relative >= 0 ? relative : length + relative;

            return at < 0 || at >= length ? JsValue.Undefined : array.ElementAt((int)at);
        });

        // `toString` IS ARRAY'S OWN FUNCTION OBJECT, as the specification makes it
        // (%TypedArray%.prototype.toString is %Array.prototype.toString%): it delegates to whatever
        // `join` the receiver has, so a program that replaces `join` sees `String(view)` follow
        // it, and falls back to the intrinsic %Object.prototype.toString%. There is deliberately
        // NO `valueOf`: a typed array has no primitive it could sensibly become, and inheriting
        // Object.prototype's - which answers the object itself - is what makes `view + ""` fall
        // back to `toString` rather than to a number nobody meant.
        TypedArrayPrototype.SetOwnProperty(
            "toString",
            JsProperty.Data(IntrinsicArrayToString, JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The members added to this prototype after the views themselves were.</summary>
    /// <remarks>
    /// <para>
    /// <b>A view's surface is not a subset of an Array's and never was.</b> <c>%TypedArray%</c> got
    /// <c>findLast</c> and <c>findLastIndex</c> alongside <c>Array.prototype</c>, and the
    /// change-by-copy trio the same year — but only three of the four: <c>toSpliced</c> is Array's
    /// alone, because splicing changes a length and a view over a buffer does not have one to
    /// change. The three that are here return a NEW view of the same kind rather than an Array,
    /// which is the difference that makes them worth having on a view at all.
    /// </para>
    /// <para>
    /// <b><c>toLocaleString</c> is here and runs the <c>Array</c> algorithm over the view's
    /// length</b>, which is what the specification defines it as: <c>Invoke</c> of each element's
    /// own <c>toLocaleString</c>, joined with <c>,</c>, and a <c>TypeError</c> when that property
    /// is not callable. This remark used to say it did what the <c>Array</c> one does when
    /// <c>Array.prototype</c> had no own <c>toLocaleString</c> at all, and the body used to fall
    /// back to <c>ToString</c> of the element instead of throwing (decision JSD-0027, follow-up
    /// N1). What <c>Number.prototype.toLocaleString</c> answers is still exactly <c>toString</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=FC377E
    // Broiler-Human:        PENDING
    private void SetupTypedArrayLaterAdditions()
    {
        Method(TypedArrayPrototype, "findLast", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "findLast");
            var callback = BinaryCallbackOf(engine, arguments, "findLast");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = array.Length - 1; at >= 0; at--)
            {
                engine.Charge(1);
                var element = array.ElementAt(at);

                if (engine.Call(callback, thisArg, [element, JsValue.Number(at), thisValue])
                    .ToBooleanValue())
                {
                    return element;
                }
            }

            return JsValue.Undefined;
        });

        Method(TypedArrayPrototype, "findLastIndex", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "findLastIndex");
            var callback = BinaryCallbackOf(engine, arguments, "findLastIndex");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = array.Length - 1; at >= 0; at--)
            {
                engine.Charge(1);

                if (engine.Call(
                        callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue])
                    .ToBooleanValue())
                {
                    return JsValue.Number(at);
                }
            }

            return JsValue.Number(-1);
        });

        Method(TypedArrayPrototype, "toReversed", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var array = BinaryLiveTypedArray(engine, thisValue, "toReversed");
            var length = array.Length;
            var made = BinaryNewTypedArray(engine, array.Kind, length);

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);
                _ = array.TryReadAt((length - at) - 1, out var element);
                _ = made.TryWriteAt(at, element);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "toSorted", 1, (engine, thisValue, arguments) =>
        {
            var comparator = ArgOfBinary(arguments, 0);

            if (comparator.Type != JsType.Undefined &&
                (!comparator.IsObject || !comparator.AsObject().IsCallable))
            {
                return engine.ThrowTypeError(
                    "the comparison function must be either a function or undefined");
            }

            var array = BinaryLiveTypedArray(engine, thisValue, "toSorted");
            var items = new System.Collections.Generic.List<JsValue>(array.Length);

            for (var at = 0; at < array.Length; at++)
            {
                engine.Charge(1);
                _ = array.TryReadAt(at, out var element);
                items.Add(element);
            }

            if (items.Count > 1)
            {
                var buffer = new System.Collections.Generic.List<JsValue>(items);
                BinaryMergeSort(engine, comparator, items, buffer, 0, items.Count);
            }

            // THE COPY IS MADE AFTER THE COMPARATOR HAS RUN, which is what makes this method safe
            // where the in-place one needs care: a comparator that detaches the buffer has nothing
            // to corrupt here, because the answer was never in that buffer.
            var made = BinaryNewTypedArray(engine, array.Kind, items.Count);

            for (var at = 0; at < items.Count; at++)
            {
                engine.Charge(1);
                _ = made.TryWriteAt(at, items[at]);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "with", 2, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "with");
            var length = array.Length;
            var wanted = JsValue.ToInteger(engine.ToNumber(ArgOfBinary(arguments, 0)));
            var index = wanted < 0 ? length + wanted : wanted;

            // THE CONVERSION HAPPENS BEFORE THE RANGE TEST, and the order is observable: a value
            // whose `valueOf` throws throws even for an index nobody could write to. It is the
            // content type's conversion - ToBigInt for a BigInt kind (JSeal B07).
            var element = engine.ToElementValue(array.Kind, ArgOfBinary(arguments, 1));

            // THE RANGE TEST IS IsValidIntegerIndex AGAINST THE BUFFER AS THE CONVERSIONS LEFT IT,
            // while the copy is as long as the receiver was before them: an index a shrink took
            // away is a RangeError, one a growth brought into range is accepted, and an element
            // the copy can no longer read is copied as `undefined` - NaN, or zero in an integer
            // kind - rather than as a stale byte. A BigInt kind copies such an element as 0n: the
            // specification's `! Set` of `undefined` there asserts a conversion ToBigInt would
            // refuse, and zero is the value the Number integer kinds already store for it.
            if (index < 0 || index >= array.Length)
            {
                return engine.ThrowRangeError("Invalid index : " + JsNumberFormat.ToJsString(wanted));
            }

            var made = BinaryNewTypedArray(engine, array.Kind, length);

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);
                var held = array.TryReadAt(at, out var read)
                    ? read
                    : array.HoldsBigInts ? JsValue.BigInt(JsBigInt.Zero) : JsValue.Number(double.NaN);
                _ = made.TryWriteAt(at, at == (int)index ? element : held);
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "toLocaleString", 0, (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var array = BinaryLiveTypedArray(engine, thisValue, "toLocaleString");
            var text = new System.Text.StringBuilder();

            // THE LENGTH IS READ ONCE, as the Array algorithm reads it: an element's method that
            // shrinks the buffer leaves the remaining separators in place and those elements empty,
            // because a read past the new end answers `undefined` and a nullish element renders as
            // nothing.
            var length = array.Length;

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);

                if (at > 0)
                {
                    text.Append(',');
                }

                var element = array.ElementAt(at);

                if (element.IsNullish)
                {
                    continue;
                }

                var method = engine.GetProperty(element, "toLocaleString");

                text.Append(engine.ToStringValue(
                    engine.Call(method, element, System.Array.Empty<JsValue>())));
            }

            return JsValue.String(text.ToString());
        });
    }

    /// <summary>The callback-taking methods.</summary>
    /// <remarks>
    /// <para>
    /// None of them skips an index. A typed array has no holes - every slot in range is a number
    /// the bytes decode to - so the hole distinction that runs through
    /// <c>JsRealm.Array.cs</c> simply does not arise here, and the loops are the simpler for it.
    /// </para>
    /// <para>
    /// <b>Each measures the receiver ONCE, before the first callback, and visits that many
    /// indices.</b> (Since 2026-09-21, JSeal F06; the loops used to re-read the length on every
    /// step, which a detach made harmless and a resize does not.) A callback that shrinks the
    /// buffer makes the indices past the new end read <c>undefined</c> and still be visited; one
    /// that grows it adds no visits. That is the specification's <c>len</c>, taken from
    /// <c>ValidateTypedArray</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=2F86D4
    // Broiler-Human:        PENDING
    private void SetupTypedArrayIteration()
    {
        Method(TypedArrayPrototype, "forEach", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "forEach");
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "forEach");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);
                _ = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);
            }

            return JsValue.Undefined;
        });

        // `map` CONSTRUCTS FIRST AND `filter` CONSTRUCTS LAST. That is the specification's order
        // and it is observable: `map` knows its result's length before the first callback and
        // reads the species before calling it; `filter` cannot know it until every predicate has
        // answered, so every callback runs before its species is read. Both write through the
        // result's own conversion, so an Int8Array mapped into a Float64Array keeps its fractions
        // and one mapped into a Uint8ClampedArray clamps.
        Method(TypedArrayPrototype, "map", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "map");
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "map");
            var thisArg = ArgOfBinary(arguments, 1);
            var made = BinaryTypedArraySpeciesCreate(
                engine, thisValue, array, [JsValue.Number(length)], "map");

            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);
                var mapped = engine.Call(
                    callback, thisArg, [array.ElementAt(at), JsValue.Number(at), thisValue]);

                _ = made.TryWriteAt(at, engine.ToElementValue(made.Kind, mapped));
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "filter", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "filter");
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "filter");
            var thisArg = ArgOfBinary(arguments, 1);
            var kept = new System.Collections.Generic.List<JsValue>();

            // TWO PASSES, because the result's length is not known until the predicate has answered
            // for every element and a typed array cannot grow. What is kept is the VALUE read, which
            // is `undefined` for an element read after a callback detached the receiver.
            for (var at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = array.ElementAt(at);
                var verdict = engine.Call(
                    callback, thisArg, [element, JsValue.Number(at), thisValue]);

                if (verdict.ToBooleanValue())
                {
                    kept.Add(element);
                }
            }

            var made = BinaryTypedArraySpeciesCreate(
                engine, thisValue, array, [JsValue.Number(kept.Count)], "filter");

            for (var at = 0; at < kept.Count; at++)
            {
                engine.Charge(1);
                _ = made.TryWriteAt(at, engine.ToElementValue(made.Kind, kept[at]));
            }

            return JsValue.Object(made);
        });

        Method(TypedArrayPrototype, "every", 1, (engine, thisValue, arguments) =>
        {
            var array = BinaryLiveTypedArray(engine, thisValue, "every");
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "every");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < length; at++)
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
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "some");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < length; at++)
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
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "find");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < length; at++)
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
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "findIndex");
            var thisArg = ArgOfBinary(arguments, 1);

            for (var at = 0; at < length; at++)
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
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "reduce");
            var at = 0;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else if (length == 0)
            {
                return engine.ThrowTypeError("Reduce of empty array with no initial value");
            }
            else
            {
                accumulated = array.ElementAt(0);
                at = 1;
            }

            for (; at < length; at++)
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
            var length = array.Length;
            var callback = BinaryCallbackOf(engine, arguments, "reduceRight");
            var at = length - 1;
            JsValue accumulated;

            if (arguments.Length > 1)
            {
                accumulated = arguments[1];
            }
            else if (length == 0)
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

    /// <summary>
    /// Builds <c>%TypedArray%</c> and the twelve constructors that inherit from it (ten where BigInt
    /// is declined).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=611D97
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

        SpeciesGetter(superclass);

        superclass.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(TypedArrayPrototype), JsPropertyAttributes.None));

        TypedArrayPrototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                JsValue.Object(superclass),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        // `of` CONSTRUCTS THROUGH ITS RECEIVER, as `from` does: any constructor, a subclass of one
        // of the twelve included, validated by TypedArrayCreateFromConstructor before anything is
        // written, and each element stored by a strict `Set`, so the result's own content type
        // converts it - ToBigInt for a BigInt kind. (Since 2026-09-22, JSeal B07. It used to look
        // its receiver up among the intrinsic constructors and refuse a subclass by name; the
        // language answers a subclass, and so does this.)
        Method(superclass, "of", 0, (engine, thisValue, arguments) =>
        {
            if (!thisValue.IsObject || !thisValue.AsObject().IsConstructor)
            {
                return engine.ThrowTypeError("%TypedArray%.of: the receiver is not a constructor");
            }

            var made = JsValue.Object(BinaryTypedArrayCreateFromConstructor(
                engine, thisValue, [JsValue.Number(arguments.Length)], "of"));

            for (var at = 0; at < arguments.Length; at++)
            {
                engine.Charge(1);
                engine.SetIndexed(made, JsValue.Number(at), arguments[at], true);
            }

            return made;
        });

        // AN ITERABLE FIRST AND AN ARRAY-LIKE SECOND, which is the order the language reads its
        // argument in and the opposite of what this profile could do when the views were written:
        // the iteration protocol was not admitted then, so a Set or a generator arrived as an
        // object with no `length` and produced an EMPTY view rather than a wrong one. It is
        // admitted now, and a source that answers `Symbol.iterator` is drained through it.
        //
        // THE RECEIVER BUILDS THE RESULT, as `TypedArrayCreateFromConstructor` - any constructor,
        // a subclass of one of the twelve included, validated before anything is written - and each
        // element goes in through a strict `Set`. `Symbol.iterator` is read once. An iterable is
        // drained before the first mapping call, as the language's `IteratorToList` does; an
        // array-like is read with `ToLength` and each element is read, mapped and written in turn,
        // so a source that is neither - a number, a plain object without a `length` - is empty.
        Method(superclass, "from", 1, (engine, thisValue, arguments) =>
        {
            if (!thisValue.IsObject || !thisValue.AsObject().IsConstructor)
            {
                return engine.ThrowTypeError("%TypedArray%.from: the receiver is not a constructor");
            }

            var items = ArgOfBinary(arguments, 0);
            var mapper = ArgOfBinary(arguments, 1);

            if (mapper.Type != JsType.Undefined &&
                (!mapper.IsObject || !mapper.AsObject().IsCallable))
            {
                return engine.ThrowTypeError("%TypedArray%.from: the mapping function is not a function");
            }

            var thisArg = ArgOfBinary(arguments, 2);

            if (items.IsNullish)
            {
                return engine.ThrowTypeError("%TypedArray%.from requires an array-like object");
            }

            if (engine.TryGetSymbolMethod(items, IteratorSymbol, out var method))
            {
                var iterator = engine.GetIteratorFromMethod(items, method);
                var collected = new System.Collections.Generic.List<JsValue>();

                while (engine.TryIterateNext(iterator, out var yielded))
                {
                    engine.Charge(1);
                    collected.Add(yielded);
                }

                var filled = JsValue.Object(BinaryTypedArrayCreateFromConstructor(
                    engine, thisValue, [JsValue.Number(collected.Count)], "from"));

                for (var at = 0; at < collected.Count; at++)
                {
                    engine.Charge(1);
                    var element = collected[at];

                    if (mapper.IsObject)
                    {
                        element = engine.Call(mapper, thisArg, [element, JsValue.Number(at)]);
                    }

                    engine.SetIndexed(filled, JsValue.Number(at), element, true);
                }

                return filled;
            }

            var source = JsValue.Object(engine.ToObject(items));
            var length = ArrayLengthOf(engine, source);
            var made = JsValue.Object(BinaryTypedArrayCreateFromConstructor(
                engine, thisValue, [JsValue.Number(length)], "from"));

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = engine.GetIndexed(source, JsValue.Number(at));

                if (mapper.IsObject)
                {
                    element = engine.Call(mapper, thisArg, [element, JsValue.Number(at)]);
                }

                engine.SetIndexed(made, JsValue.Number(at), element, true);
            }

            return made;
        });

        foreach (var kind in JsElements.All)
        {
            // The two BigInt kinds only where BigInt is admitted; see `binaryHoldsBigInts`.
            if (JsElements.HoldsBigInts(kind) && !binaryHoldsBigInts)
            {
                continue;
            }

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
                (engine, thisValue, arguments) =>
                    BinaryConstructTypedArray(engine, kind, arguments, thisValue));

            constructor.BuildsFromNewTarget = true;

            // THE CONSTRUCTOR'S OWN PROTOTYPE IS %TypedArray%, not Function.prototype, which is
            // what makes `Int8Array.from` reachable through the superclass in a real engine and
            // what a program checks when it asks whether something is one of the twelve.
            constructor.Prototype = superclass;
            typedArrayConstructors[kind] = constructor;

            // ON BOTH THE CONSTRUCTOR AND THE PROTOTYPE, and frozen on each: the specification
            // defines it in both places, and code that computes an offset reads it off whichever
            // one it happens to hold.
            constructor.DefineFrozen("BYTES_PER_ELEMENT", JsValue.Number(width));
            prototype.DefineFrozen("BYTES_PER_ELEMENT", JsValue.Number(width));

        }
    }

    /// <summary>
    /// The specification's <c>SpeciesConstructor(O, defaultConstructor)</c> for the binary surface.
    /// </summary>
    /// <remarks>
    /// <b>Two reads and two defaults, in that order.</b> <c>constructor</c> is read first and
    /// <c>undefined</c> answers the default; anything else that is not an object is a
    /// <c>TypeError</c>. Its <c>Symbol.species</c> is read second and <c>undefined</c> or
    /// <c>null</c> answers the default; anything else must be a constructor, because the next thing
    /// that happens to it is a <c>new</c>. Both reads are ordinary property reads, so a throwing
    /// getter propagates as itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EBC340
    // Broiler-Human:        PENDING
    private JsValue BinarySpeciesConstructor(
        JsEngine engine, JsValue receiver, JsObject defaultConstructor)
    {
        var constructor = engine.GetProperty(receiver, "constructor");

        if (constructor.Type == JsType.Undefined)
        {
            return JsValue.Object(defaultConstructor);
        }

        if (!constructor.IsObject)
        {
            throw engine.Error("TypeError", "the receiver's `constructor` is not an object");
        }

        var species = engine.GetSymbol(constructor, SpeciesSymbol);

        if (species.IsNullish)
        {
            return JsValue.Object(defaultConstructor);
        }

        if (!species.IsObject || !species.AsObject().IsConstructor)
        {
            throw engine.Error("TypeError", "the species is not a constructor");
        }

        return species;
    }

    /// <summary>
    /// The specification's <c>TypedArraySpeciesCreate(exemplar, argumentList)</c>: a new typed
    /// array from the exemplar's species, defaulting to the constructor of the exemplar's own kind.
    /// </summary>
    /// <remarks>
    /// <b>The content type is compared explicitly.</b> A species may answer a typed array of any
    /// kind - an <c>Int8Array</c>'s <c>map</c> may land in a <c>Float64Array</c> - but not one
    /// whose elements are a different TYPE of value: a <c>BigInt64Array</c> whose species answers a
    /// <c>Float64Array</c> is a <c>TypeError</c> here, before anything is written (reachable since
    /// JSeal B07; until then every kind held Numbers and the check could not fail).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3D528B
    // Broiler-Human:        PENDING
    private JsTypedArray BinaryTypedArraySpeciesCreate(
        JsEngine engine, JsValue exemplarValue, JsTypedArray exemplar, JsValue[] arguments, string method)
    {
        var constructor = BinarySpeciesConstructor(
            engine, exemplarValue, typedArrayConstructors[exemplar.Kind]);
        var made = BinaryTypedArrayCreateFromConstructor(engine, constructor, arguments, method);

        if (JsElements.HoldsBigInts(made.Kind) != JsElements.HoldsBigInts(exemplar.Kind))
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method +
                ": the species constructor returned a typed array of another content type");
        }

        return made;
    }

    /// <summary>
    /// The specification's <c>TypedArrayCreateFromConstructor(constructor, argumentList)</c>: a
    /// construction whose result is validated before anything is written into it.
    /// </summary>
    /// <remarks>
    /// <b>The result must be a live typed array, and, when it was asked for a length, a long enough
    /// one.</b> A species constructor is guest code and may answer anything - a plain object, a
    /// proxy around a typed array (which has no typed-array slots of its own), a view over a buffer
    /// it has just detached, or a view shorter than the count it was handed. Each of those is a
    /// <c>TypeError</c> here, before a caller writes the first element. The length check applies
    /// only to the single-Number form; <c>subarray</c>'s <c>(buffer, byteOffset, length)</c> form
    /// is not held to it, which is what the specification says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=43E8BF
    // Broiler-Human:        PENDING
    private static JsTypedArray BinaryTypedArrayCreateFromConstructor(
        JsEngine engine, JsValue constructor, JsValue[] arguments, string method)
    {
        var constructed = engine.Construct(constructor, arguments);

        if (constructed.AsObjectOrNull() is not JsTypedArray made)
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method +
                ": the species constructor did not return a typed array");
        }

        if (made.IsOutOfBounds)
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method +
                ": the species constructor returned a typed array that is detached or out of bounds");
        }

        if (arguments.Length == 1 && arguments[0].IsNumber && made.Length < arguments[0].AsNumber())
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method +
                ": the species constructor returned a typed array shorter than requested");
        }

        return made;
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

    /// <summary>
    /// The specification's <c>GetArrayBufferMaxByteLengthOption</c>: the maximum an options bag
    /// asks for, or <see langword="null"/> for a fixed-length buffer.
    /// </summary>
    /// <remarks>
    /// Only an object is read, and only its <c>maxByteLength</c>; a primitive in the options place
    /// is ignored rather than refused, and <c>undefined</c> there means "fixed length". The read is
    /// an ordinary property read and the conversion is <c>ToIndex</c>, so a getter or a
    /// <c>valueOf</c> runs and its exception propagates as itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1E8332
    // Broiler-Human:        PENDING
    private static double? BinaryMaxByteLengthOption(JsEngine engine, JsValue options)
    {
        if (!options.IsObject)
        {
            return null;
        }

        var wanted = engine.GetProperty(options, "maxByteLength");

        return wanted.Type == JsType.Undefined
            ? null
            : BinaryToIndex(engine, wanted, "maxByteLength");
    }

    /// <summary>
    /// Allocates a resizable buffer of <paramref name="byteLength"/> bytes that may grow to
    /// <paramref name="maxByteLength"/>, charging for the bytes it holds now.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The maximum is a promise the buffer may be held to, not memory it takes.</b> No storage
    /// is reserved up front: only the current length is allocated, charged and retained, and each
    /// growth pays for itself when it happens (<see cref="BinaryResizeBuffer"/>). A maximum this
    /// runtime could never allocate - past <c>Array.MaxLength</c> - is a <c>RangeError</c> now,
    /// which the specification allows ("if it is not possible to create a Data Block consisting of
    /// maxByteLength bytes"), rather than a promise every later resize would break.
    /// </para>
    /// <para>
    /// A length above the maximum is refused first, as the specification orders it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=7FFA8D
    // Broiler-Human:        PENDING
    private JsArrayBuffer BinaryNewResizableBuffer(JsEngine engine, double byteLength, double maxByteLength)
    {
        if (byteLength > maxByteLength)
        {
            throw engine.Error(
                "RangeError",
                "Invalid array buffer length: " + JsNumberFormat.ToJsString(byteLength) +
                " exceeds the maximum byte length " + JsNumberFormat.ToJsString(maxByteLength));
        }

        if (maxByteLength > System.Array.MaxLength)
        {
            throw engine.Error(
                "RangeError",
                "Invalid array buffer max length: " + JsNumberFormat.ToJsString(maxByteLength));
        }

        var size = (int)byteLength;

        engine.Charge((ulong)size);
        var buffer = new JsArrayBuffer(ArrayBufferPrototype, size, (int)maxByteLength);
        engine.Retain((ulong)size);
        return buffer;
    }

    /// <summary>
    /// Resizes a resizable buffer the caller has already validated: the specification's default
    /// <c>HostResizeArrayBuffer</c>, a new block holding the common prefix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every step that can fail comes before the buffer changes.</b> The fuel is charged - one
    /// unit per byte of the new block, which is the allocation and the copy - then the live-bytes
    /// ceiling is asked to admit any growth past the buffer's high-water mark, then the new array
    /// is allocated. A refused charge ends the run; an allocation the runtime cannot make is a
    /// <c>RangeError</c>, which the specification's <c>CreateByteDataBlock</c> names for exactly
    /// that case. In each of them the buffer still holds its old bytes at its old length, and every
    /// view over it still reads them.
    /// </para>
    /// <para>
    /// <b>The ceiling is ASKED, not told</b>, as the host surface asks it: a growth it refuses must
    /// not have happened, and a retention reported afterwards would only be refused at the next
    /// charge, once the guest could already see the grown buffer. A shrink retains nothing and
    /// releases nothing; see <see cref="JsArrayBuffer.RetainedByteLength"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A87CC7
    // Broiler-Falsified-If: a resize the fuel meter, the live-bytes ceiling or the allocator refuses leaves the buffer at a different length or with different bytes
    // Broiler-Human:        PENDING
    private static void BinaryResizeBuffer(JsEngine engine, JsArrayBuffer buffer, int newByteLength)
    {
        engine.Charge((ulong)newByteLength);

        // THE CEILING IS ASKED BEFORE THE ALLOCATION, which is `RetainOrAbort`'s own contract: a
        // growth it refuses must not first cost the host the block it refused. An allocation that
        // then fails leaves those bytes counted and unused, which the no-release rule above already
        // accepts for every allocation in this realm.
        var growth = newByteLength - buffer.RetainedByteLength;

        if (growth > 0)
        {
            engine.RetainOrAbort((ulong)growth);
        }

        byte[] storage;

        try
        {
            storage = new byte[newByteLength];
        }
        catch (System.OutOfMemoryException)
        {
            throw engine.Error(
                "RangeError",
                "ArrayBuffer.prototype.resize: " + JsNumberFormat.ToJsString(newByteLength) +
                " bytes could not be allocated");
        }

        if (!buffer.TryReplaceStorage(storage))
        {
            throw engine.Error(
                "TypeError", "ArrayBuffer.prototype.resize called on a detached or fixed-length ArrayBuffer");
        }
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

    /// <summary>The five construction forms of a typed array constructor.</summary>
    /// <remarks>
    /// <para>
    /// <c>new X(length)</c>, <c>new X(iterable)</c> and <c>new X(arrayLike)</c> allocate;
    /// <c>new X(typedArray)</c> allocates and CONVERTS element by element, so
    /// <c>new Uint8Array(new Float64Array([1.7]))</c> is <c>[1]</c>; and
    /// <c>new X(buffer, byteOffset, length)</c> allocates nothing and shares the bytes, which is the
    /// form that makes two typed arrays aliases of one another and the only one whose arguments can
    /// be misaligned.
    /// </para>
    /// <para>
    /// <b>An object argument is asked for <c>Symbol.iterator</c> BEFORE it is asked for a
    /// <c>length</c>, and the answer until 2026-09-21 was that it was never asked.</b> Every
    /// object that was not a buffer or a view was read as an array-like, so
    /// <c>new Uint8Array(new Set([1, 2]))</c> and <c>new Float64Array(generator())</c> - objects
    /// with no <c>length</c> - built EMPTY views, and an object carrying both an iterator and a
    /// <c>length</c> was read through the wrong one. The specification's <c>TypedArray</c> reads
    /// the method once with <c>GetMethod</c>, drains the iterator it returns into a list and only
    /// then allocates, and that is the order here: <c>undefined</c> or <c>null</c> falls through to
    /// the array-like reading, anything else that is not callable is a <c>TypeError</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1C05F7
    // Broiler-Human:        PENDING
    private JsValue BinaryConstructTypedArray(
        JsEngine engine, JsElementKind kind, JsValue[] arguments, JsValue newTarget)
    {
        var name = JsElements.ConstructorNameOf(kind);
        var width = JsElements.WidthOf(kind);
        var first = ArgOfBinary(arguments, 0);

        // THE PROTOTYPE IS READ BEFORE AN OBJECT ARGUMENT IS LOOKED AT (AllocateTypedArray comes
        // first in every object form), so a `prototype` getter on `new.target` that detaches the
        // buffer or the source is seen by the checks below rather than after them. A primitive
        // argument is converted to a length first, and the prototype is read afterwards.
        var prototype = first.IsObject
            ? BinaryPrototypeFrom(engine, newTarget, TypedArrayPrototypes[kind])
            : TypedArrayPrototypes[kind];

        if (first.AsObjectOrNull() is JsArrayBuffer buffer)
        {
            var offset = BinaryToIndex(engine, ArgOfBinary(arguments, 1), "byteOffset");

            if (offset % width != 0)
            {
                return engine.ThrowRangeError(
                    "start offset of " + name + " should be a multiple of " + width);
            }

            // THE LENGTH IS CONVERTED BEFORE THE BUFFER IS ASKED WHETHER IT IS STILL THERE, which
            // is the specification's order: the conversion can run a `valueOf` that detaches it,
            // and what that must meet is the detached-buffer TypeError rather than a RangeError
            // measured against a length that is no longer the buffer's.
            var lengthGiven = ArgOfBinary(arguments, 2).Type != JsType.Undefined;
            var requested = lengthGiven ? BinaryToIndex(engine, arguments[2], "length") : 0;

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

            // NO LENGTH OVER A RESIZABLE BUFFER IS `auto`: the view tracks the buffer, so neither
            // the buffer's length nor what lies past the offset need be a multiple of the element
            // width - a partial trailing element is simply not part of the view.
            if (!lengthGiven && buffer.IsResizable)
            {
                return JsValue.Object(new JsTypedArray(prototype, buffer, (int)offset, null, kind));
            }

            double count;

            if (!lengthGiven)
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
                count = requested;

                if (offset + (count * width) > buffer.ByteLength)
                {
                    return engine.ThrowRangeError(
                        "Invalid typed array length: " + JsNumberFormat.ToJsString(count));
                }
            }

            return JsValue.Object(new JsTypedArray(
                prototype, buffer, (int)offset, (int)count, kind));
        }

        if (first.AsObjectOrNull() is JsTypedArray source)
        {
            if (source.IsOutOfBounds)
            {
                return engine.ThrowTypeError(
                    "Cannot construct " + name + " from a typed array that is detached or out of bounds");
            }

            // The copy is of the source's length NOW, and nothing between here and the last
            // element runs guest code, so it cannot change under the loop.
            var sourceLength = source.Length;
            var copied = BinaryNewTypedArray(engine, kind, sourceLength);
            copied.Prototype = prototype;

            // TWO CONTENT TYPES DO NOT CONVERT INTO ONE ANOTHER: `new BigInt64Array(float64s)` and
            // `new Float64Array(bigInt64s)` are TypeErrors, raised after the allocation as
            // InitializeTypedArrayFromTypedArray orders them (JSeal B07).
            if (source.HoldsBigInts != copied.HoldsBigInts)
            {
                return engine.ThrowTypeError(
                    "Cannot construct " + name + " from a " + JsElements.ConstructorNameOf(source.Kind) +
                    ": the two hold different content types");
            }

            for (var at = 0; at < sourceLength; at++)
            {
                engine.Charge(1);
                _ = source.TryReadAt(at, out var element);
                _ = copied.TryWriteAt(at, element);
            }

            return JsValue.Object(copied);
        }

        if (first.IsObject && engine.TryGetSymbolMethod(first, IteratorSymbol, out var iterate))
        {
            // DRAINED FIRST, ALLOCATED SECOND, CONVERTED THIRD - the specification's
            // `InitializeTypedArrayFromList`. The iterator is guest code and the count is not known
            // until it is done, so the values are held as values and each is converted as it is
            // stored, which is where a `valueOf` on an element runs. Every step of the drain is
            // charged, by the iteration protocol itself or by the Array drain that stands in for it.
            var values = new System.Collections.Generic.List<JsValue>();

            // An element the Array drain below has charged for is not charged again when it is
            // stored, which keeps `new Float64Array(array)` at the fuel the array-like reading cost.
            var drainCharged = false;

            if (ArrayIterationIsIntrinsic(first, iterate))
            {
                BinaryDrainArrayValues(engine, (JsArray)first.AsObject(), values);
                drainCharged = true;
            }
            else
            {
                var record = engine.GetIteratorFromMethod(first, iterate);

                while (engine.TryIterateNext(record, out var value))
                {
                    values.Add(value);
                }
            }

            var made = BinaryNewTypedArray(engine, kind, values.Count);
            made.Prototype = prototype;

            for (var at = 0; at < values.Count; at++)
            {
                if (!drainCharged)
                {
                    engine.Charge(1);
                }

                _ = made.TryWriteAt(at, engine.ToElementValue(kind, values[at]));
            }

            return JsValue.Object(made);
        }

        if (first.IsObject)
        {
            // `LengthOfArrayLike`, which is `ToLength` and not `ToUint32`: a `length` of 2**53 is
            // a length no buffer can hold and a RangeError, where the modular wrap made it zero and
            // answered an empty view.
            var declared = engine.ToInteger(engine.GetProperty(first, "length"));
            var length = declared <= 0 ? 0 : System.Math.Min(declared, 9007199254740991.0);
            var made = BinaryNewTypedArray(engine, kind, length);
            made.Prototype = prototype;

            for (double at = 0; at < length; at++)
            {
                engine.Charge(1);
                var element = engine.GetIndexed(first, JsValue.Number(at));
                _ = made.TryWriteAt((int)at, engine.ToElementValue(kind, element));
            }

            return JsValue.Object(made);
        }

        // A PRIMITIVE IS A LENGTH, INCLUDING A STRING ONE: `new Int8Array("4")` is four zeroes,
        // because the specification runs ToIndex over anything that is not an object.
        var elements = BinaryToIndex(engine, first, "length");
        prototype = BinaryPrototypeFrom(engine, newTarget, TypedArrayPrototypes[kind]);
        var allocated = BinaryNewTypedArray(engine, kind, elements);
        allocated.Prototype = prototype;
        return JsValue.Object(allocated);
    }

    /// <summary>
    /// The specification's <c>GetPrototypeFromConstructor(newTarget, default)</c> for the binary
    /// constructors that build from <c>new.target</c> themselves.
    /// </summary>
    /// <remarks>
    /// A <c>new.target</c> that is not an object - a direct internal construction - or whose
    /// <c>prototype</c> is not an object answers the default, which is the same answer the engine
    /// gives when it re-points a built-in's instance.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=555F6F
    // Broiler-Human:        PENDING
    private static JsObject BinaryPrototypeFrom(JsEngine engine, JsValue newTarget, JsObject fallback)
    {
        if (!newTarget.IsObject)
        {
            return fallback;
        }

        var wanted = engine.GetProperty(newTarget, "prototype");
        return wanted.IsObject ? wanted.AsObject() : fallback;
    }

    /// <summary>
    /// Drains an Array through the intrinsic <c>Array.prototype.values</c> without building its
    /// iterator, for the typed array constructor's iterable path.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Taken only when nothing could tell the difference.</b> The caller has already read
    /// <c>Symbol.iterator</c> once, as the language does, and found this realm's own
    /// <c>Array.prototype.values</c>, and has checked that <c>%ArrayIteratorPrototype%.next</c> is
    /// still the intrinsic one (<see cref="ArrayIterationIsIntrinsic"/>). With both in place what
    /// the iterator would do is not observable: the intrinsic <c>next</c> the protocol would read
    /// off the shared prototype runs the step <c>CreateIndexedIterator</c> gives it, and the result
    /// objects it answers hold plain data properties that nothing but this loop reads.
    /// The steps that ARE observable are kept one for one: <c>length</c> is re-read before every
    /// element, so an array a getter grows or shortens is followed exactly as the iterator would
    /// follow it, and each element is read through <see cref="JsEngine.GetIndexed"/>, so a hole
    /// still reaches the prototype chain and an accessor still runs, in index order, before any
    /// element is converted.
    /// </para>
    /// <para>
    /// <b>It exists because the plain Array is the commonest argument there is.</b> Driving the
    /// protocol for it costs a native call, a result object and two property reads per element,
    /// which made <c>new Float64Array(array)</c> roughly ten times slower and more than half as
    /// expensive again in fuel than the array-like reading it replaced. The array iterator has no
    /// <c>next</c> of its own: it reads it from the shared <c>%ArrayIteratorPrototype%</c> (JSeal
    /// F06), which a program may replace - which is why the check above covers <c>next</c> as well
    /// as <c>values</c> (JSeal VM-FIX-I).
    /// </para>
    /// <para>
    /// Every element is charged here, once, so a getter that keeps growing the array still runs
    /// out of fuel; the caller does not charge the same element again when it stores it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B9DFA7
    // Broiler-Human:        PENDING
    private static void BinaryDrainArrayValues(
        JsEngine engine, JsArray array, System.Collections.Generic.List<JsValue> values)
    {
        for (var at = 0u; at < array.Length; at++)
        {
            engine.Charge(1);
            values.Add(engine.GetIndexed(JsValue.Object(array), JsValue.Number(at)));
        }
    }

    /// <summary>Copies one typed array into another, converting to the target's kind.</summary>
    /// <remarks>
    /// <b>An overlapping copy is snapshotted first.</b> The two arrays may be views onto the same
    /// buffer at different offsets, and a straight forward loop would then read elements it had
    /// already overwritten. Only the same-buffer case pays for the snapshot; the ordinary case
    /// streams straight through.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=EC1AA3
    // Broiler-Human:        PENDING
    private static JsValue BinarySetFromTypedArray(
        JsEngine engine, JsTypedArray target, JsTypedArray source, double offset)
    {
        // BOTH VIEWS ARE MEASURED NOW AND NOT AGAIN: nothing from here to the last write runs guest
        // code, so neither length can change under the copy.
        var targetLength = BinaryValidTypedArray(engine, target, "set").Length;

        if (source.IsOutOfBounds)
        {
            return engine.ThrowTypeError(
                "%TypedArray%.prototype.set: the source typed array is detached or out of bounds");
        }

        var sourceLength = source.Length;

        if (sourceLength + offset > targetLength)
        {
            return engine.ThrowRangeError("offset is out of bounds");
        }

        // A COPY BETWEEN THE TWO CONTENT TYPES IS A TypeError, after the range check as
        // SetTypedArrayFromTypedArray orders them: a BigInt64Array into a Float64Array would
        // otherwise need a conversion the language refuses in both directions (JSeal B07).
        if (source.HoldsBigInts != target.HoldsBigInts)
        {
            return engine.ThrowTypeError(
                "%TypedArray%.prototype.set: the source and the target hold different content types");
        }

        var at = (int)offset;

        if (!ReferenceEquals(source.Buffer, target.Buffer))
        {
            for (var index = 0; index < sourceLength; index++)
            {
                engine.Charge(1);
                _ = source.TryReadAt(index, out var element);
                _ = target.TryWriteAt(at + index, element);
            }

            return JsValue.Undefined;
        }

        var snapshot = new JsValue[sourceLength];

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
    /// The specification's <c>ValidateTypedArray</c>: a typed array whose buffer is still there
    /// and still covers the view.
    /// </summary>
    /// <remarks>
    /// Out of bounds is refused exactly as detached is (since 2026-09-21, JSeal F05): a view a
    /// resize has left past the end of its buffer has no elements a method could work on, and the
    /// specification throws for it rather than letting the method run over a length of zero.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A94B34
    // Broiler-Human:        PENDING
    private static JsTypedArray BinaryLiveTypedArray(JsEngine engine, JsValue value, string method) =>
        BinaryValidTypedArray(engine, BinaryThisTypedArray(engine, value, method), method);

    /// <summary>
    /// The second half of <c>ValidateTypedArray</c>, for a view already known to be one: refuses it
    /// when it is detached or out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=24D22D
    // Broiler-Human:        PENDING
    private static JsTypedArray BinaryValidTypedArray(JsEngine engine, JsTypedArray array, string method)
    {
        if (array.IsOutOfBounds)
        {
            throw engine.Error(
                "TypeError",
                "%TypedArray%.prototype." + method +
                " called on a typed array that is detached or out of bounds");
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=DD683D
    // Broiler-Human:        PENDING
    private static JsValue BinaryViewRead(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, JsElementKind kind, string method)
    {
        var view = BinaryThisView(engine, thisValue, method);
        var at = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "byteOffset");
        var littleEndian = ArgOfBinary(arguments, 1).ToBooleanValue();

        // DETACHED AND OUT OF BOUNDS ARE ONE TypeError, asked after every coercion: a `valueOf`
        // may have detached the buffer or shrunk it below the view, and only an index past a view
        // that is still in bounds is the RangeError below.
        if (view.IsOutOfBounds)
        {
            return engine.ThrowTypeError(
                "DataView.prototype." + method + " called on a detached or out-of-bounds view");
        }

        if (at > int.MaxValue || !view.TryRead(kind, (int)at, littleEndian, out var value))
        {
            return engine.ThrowRangeError("Offset is outside the bounds of the DataView");
        }

        return value;
    }

    /// <summary>One <c>DataView</c> write: <c>(byteOffset, value[, littleEndian])</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=BAE818
    // Broiler-Human:        PENDING
    private static JsValue BinaryViewWrite(
        JsEngine engine, JsValue thisValue, JsValue[] arguments, JsElementKind kind, string method)
    {
        var view = BinaryThisView(engine, thisValue, method);
        var at = BinaryToIndex(engine, ArgOfBinary(arguments, 0), "byteOffset");

        // THE VALUE IS COERCED BEFORE THE BOUNDS ARE CHECKED, which the specification requires and
        // which is observable: a `valueOf` that detaches the buffer runs, and the write that
        // follows then fails rather than writing into bytes nobody owns any more. The coercion is
        // ToBigInt for `setBigInt64` and `setBigUint64` (JSeal B08), so a Number there is the
        // TypeError it owes, raised before the endianness is read.
        var value = engine.ToElementValue(kind, ArgOfBinary(arguments, 1));
        var littleEndian = ArgOfBinary(arguments, 2).ToBooleanValue();

        // DETACHED AND OUT OF BOUNDS ARE ONE TypeError, asked after every coercion: a `valueOf`
        // may have detached the buffer or shrunk it below the view, and only an index past a view
        // that is still in bounds is the RangeError below.
        if (view.IsOutOfBounds)
        {
            return engine.ThrowTypeError(
                "DataView.prototype." + method + " called on a detached or out-of-bounds view");
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
    /// <remarks>
    /// The elements are both Numbers or both BigInts - one view's content type - and the default
    /// order compares them as what they are: two BigInts by their integers, exactly, however far
    /// past 2**53 they lie (JSeal B07).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=A63AA5
    // Broiler-Human:        PENDING
    private static int BinaryCompareElements(
        JsEngine engine, JsValue comparator, JsValue left, JsValue right)
    {
        engine.Charge(1);

        if (comparator.IsObject)
        {
            var ordering = engine.ToNumber(engine.Call(
                comparator, JsValue.Undefined, [left, right]));

            if (double.IsNaN(ordering) || ordering == 0)
            {
                return 0;
            }

            return ordering < 0 ? -1 : 1;
        }

        if (left.IsBigInt && right.IsBigInt)
        {
            return left.AsBigInt().Value.CompareTo(right.AsBigInt().Value);
        }

        return BinaryCompareNumbers(left.AsNumber(), right.AsNumber());
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B24B69
    // Broiler-Human:        PENDING
    private static void BinaryMergeSort(
        JsEngine engine,
        JsValue comparator,
        System.Collections.Generic.List<JsValue> items,
        System.Collections.Generic.List<JsValue> buffer,
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
