// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   36
// Annotated:        36/36
// Exempt:           17
// Human-reviewed:   0/36
// IP risk:          Low
// Security risk:    High
// Criteria:         22/22
// Resource impact:  4/10 max
// Unverified:       36
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Structured clone, internally: a serializer that turns a guest value into a
/// <see cref="JsCloneCarrier"/> and a deserializer that rebuilds one in this realm.
/// </summary>
/// <remarks>
/// <para>
/// <b>The algorithm is HTML's <c>StructuredSerializeInternal</c> and
/// <c>StructuredDeserialize</c>, over the brands decision JSD-0032's matrix lists.</b> A value is
/// visited once: the memory map answers an object met before with the record it already has, which
/// is what makes a cycle terminate and a shared reference stay shared. An ordinary object or an
/// Array contributes its own enumerable string-keyed properties, read with <c>[[Get]]</c> - so a
/// getter runs, in key order, depth first, and a throw from it propagates unchanged. Symbol keys
/// are skipped, as HTML skips them. Everything outside the matrix is refused.
/// </para>
/// <para>
/// <b>It lives on the realm because two of its brands do.</b> A Date's time value and a RegExp's
/// original source are held by types private to this class, and reconstruction needs this realm's
/// intrinsics - its prototypes, its Error constructors, its element kinds - and nothing of the
/// realm that produced the carrier.
/// </para>
/// <para>
/// <b>Neither walk recurses on the CLR stack.</b> Serialization keeps its own stack of frames, so a
/// linked list a million nodes deep costs a million frames of heap rather than a stack overflow;
/// deserialization builds every object first and fills them second, so it needs no stack at all.
/// Both are charged: fuel per record, per value and per byte copied, with the destination's
/// buffers reported as retained, and the carrier's size is capped by
/// <see cref="JsCloneCarrier.MaxEntries"/> and <see cref="JsCloneCarrier.MaxBytes"/>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=60DD8D
// Broiler-Falsified-If: a clone graph carries a reference into the realm that produced it
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The Error names HTML keeps; every other name becomes <c>Error</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3982A7
    // Broiler-Human:        PENDING
    private static readonly string[] CloneErrorNames =
    [
        "Error", "EvalError", "RangeError", "ReferenceError", "SyntaxError", "TypeError", "URIError",
    ];

    /// <summary>Serializes <paramref name="value"/> into a carrier that holds no object of this realm.</summary>
    /// <remarks>
    /// May run guest code (getters, <c>name</c> accessors, a message's <c>toString</c>), and a guest
    /// throw from any of it propagates as itself. A value outside the matrix raises
    /// <see cref="JsCloneRefusedException"/>; so does a graph past either bound, which the caller
    /// may narrow below the carrier's own maxima and never widen past them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=33240C
    // Broiler-Falsified-If: a serialization completes for a function, a symbol, a Proxy or an unlisted brand, or its work is not charged
    // Broiler-Human:        PENDING
    internal JsCloneCarrier CloneSerialize(JsValue value, long maxEntries, long maxBytes)
    {
        var writer = new CloneWriter(
            this,
            System.Math.Clamp(maxEntries, 0, JsCloneCarrier.MaxEntries),
            System.Math.Clamp(maxBytes, 0, JsCloneCarrier.MaxBytes));

        return writer.Run(value, []);
    }

    /// <summary>
    /// Serializes <paramref name="value"/> and moves the bytes of every buffer in
    /// <paramref name="transfer"/> into the carrier: HTML's <c>StructuredSerializeWithTransfer</c>,
    /// with every check made before the first detachment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The order is HTML's, except that nothing is detached until everything has passed.</b>
    /// The list is checked first (an ArrayBuffer, not already listed), before any getter runs; the
    /// graph is serialized with each listed buffer already in the memory map, so every reference to
    /// it and every view over it names the one record; then each listed buffer is asked again
    /// whether it is detached (a getter may have detached it) and its bytes are counted against the
    /// bound. HTML detaches each buffer as it passes that last check, so a later refusal leaves the
    /// earlier ones detached; here the checks all run first and the detachments afterwards, in a
    /// loop that charges nothing and cannot throw, so a refusal, a guest throw or an abort at any
    /// point leaves every listed buffer as it was.
    /// </para>
    /// <para>
    /// <b>A resizable buffer is refused</b> with <see cref="JsCloneRefusal.NotTransferable"/> until the
    /// record carries its maximum length (decision JSD-0032 section 5a). The clone check
    /// <c>clone/i16/resizable-buffers-are-refused-until-f04-f06-integrate</c> watches it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=8C1269
    // Broiler-Falsified-If: a refused transfer leaves any listed buffer detached, or a completed one leaves any listed buffer attached
    // Broiler-Human:        PENDING
    internal JsCloneCarrier CloneSerializeWithTransfer(
        JsValue value,
        JsValue[] transfer,
        long maxEntries,
        long maxBytes)
    {
        var writer = new CloneWriter(
            this,
            System.Math.Clamp(maxEntries, 0, JsCloneCarrier.MaxEntries),
            System.Math.Clamp(maxBytes, 0, JsCloneCarrier.MaxBytes));

        return writer.Run(value, transfer);
    }

    /// <summary>Rebuilds a carrier's graph out of this realm's intrinsics.</summary>
    /// <remarks>
    /// Runs no guest code: every object is fresh, every property is defined rather than assigned,
    /// and every Map or Set entry is appended to the table directly, as HTML's deserializer does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=185FAE
    // Broiler-Falsified-If: a rebuilt object has a prototype that is not this realm's, or two adoptions share a buffer
    // Broiler-Human:        PENDING
    internal JsValue CloneDeserialize(JsCloneCarrier carrier)
    {
        var records = carrier.Records;
        var built = new JsObject?[records.Length];

        // A REALM THAT DECLINED THE BIGINT SURFACE HOLDS NO BIGINT (card B06), and it says so before
        // it claims a single-use carrier: a refusal here must not spend moved bytes.
        if (carrier.HoldsBigInt && BigIntPrototype is null)
        {
            throw new JsCloneRefusedException(
                JsCloneRefusal.Unrepresentable,
                "the carrier holds a BigInt and this realm's composition declined the BigInt surface");
        }

        // MOVED BYTES ARRIVE ONCE. The claim comes before anything is built or charged, so a
        // second adoption is refused whole rather than halfway.
        if (carrier.Moved > 0 && !carrier.TryClaimMoved())
        {
            throw new JsCloneRefusedException(
                JsCloneRefusal.Consumed, "the carrier's transferred buffers were already adopted");
        }

        engine.Charge((ulong)records.Length + 1);

        // FIRST EVERY OBJECT THAT OWNS ITS OWN STATE, buffers included, so that a view built next
        // can name the one buffer every view of it shares.
        for (var at = 0; at < records.Length; at++)
        {
            engine.Charge(4);
            built[at] = CloneShell(records[at]);
        }

        for (var at = 0; at < records.Length; at++)
        {
            var record = records[at];

            if (record.Kind is JsCloneKind.TypedArray or JsCloneKind.DataView)
            {
                engine.Charge(4);
                built[at] = CloneView(record, built);
            }
        }

        // THEN THE CONTENTS, which may name any record including the one being filled: a cycle is
        // an index, and the object it names already exists.
        for (var at = 0; at < records.Length; at++)
        {
            CloneFill(records[at], built[at]!, built);
        }

        return CloneValueOf(carrier.Root, built);
    }

    /// <summary>Builds the object a record names, or nothing for a view, which is built second.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D3477C
    // Broiler-Human:        PENDING
    private JsObject? CloneShell(JsCloneRecord record)
    {
        switch (record.Kind)
        {
            case JsCloneKind.Object:
                return new JsObject(ObjectPrototype);

            case JsCloneKind.Array:
            {
                // THE LENGTH FIRST, AS HTML'S ArrayCreate(length) DOES. Setting it allocates
                // nothing, so a length of a billion with one element is one element.
                var array = new JsArray(ArrayPrototype);
                array.SetLength(JsValue.ToUint32(record.Number));
                return array;
            }

            case JsCloneKind.Boolean:
                return new JsPrimitiveWrapper(BooleanPrototype, "Boolean", JsValue.Boolean(record.Number != 0));

            case JsCloneKind.Number:
                return new JsPrimitiveWrapper(NumberPrototype, "Number", JsValue.Number(record.Number));

            case JsCloneKind.String:
                return new JsPrimitiveWrapper(StringPrototype, "String", JsValue.String(record.Text ?? string.Empty));

            // THE CLASS IS "Object", as ToObject makes it: a BigInt object's tag is its
            // prototype's @@toStringTag, not a class of its own.
            case JsCloneKind.BigInt:
                return new JsPrimitiveWrapper(
                    BigIntPrototype!, "Object", CloneBigInt(record.Values.Length == 1 ? record.Values[0] : default));

            case JsCloneKind.Date:
                return new DateObject(DatePrototype, record.Number);

            case JsCloneKind.RegExp:
                return CloneRegExp(record);

            case JsCloneKind.Map:
                return new JsMapObject(MapPrototype);

            case JsCloneKind.Set:
                return new JsSetObject(SetPrototype);

            case JsCloneKind.Error:
                return CloneError(record);

            case JsCloneKind.ArrayBuffer:
                return CloneBuffer(record);

            case JsCloneKind.TypedArray:
            case JsCloneKind.DataView:
                return null;

            default:
                throw new JsCloneRefusedException(
                    JsCloneRefusal.Unrepresentable, "the carrier names a record kind this realm does not know");
        }
    }

    /// <summary>Recompiles a RegExp from its original source and flags; <c>lastIndex</c> starts at zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=AC35AC
    // Broiler-Human:        PENDING
    private RegExpObject CloneRegExp(JsCloneRecord record)
    {
        try
        {
            return RegExpBuild(engine, record.Text ?? string.Empty, record.Detail ?? string.Empty);
        }
        catch (JsThrow)
        {
            // A pattern the source realm compiled and this one cannot is a difference between two
            // builds, not a guest error, and a SyntaxError here would name the wrong party.
            throw new JsCloneRefusedException(
                JsCloneRefusal.Unrepresentable, "this realm cannot compile the RegExp the carrier holds");
        }
    }

    /// <summary>Builds an Error on this realm's prototype for the carried name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DFD2C1
    // Broiler-Human:        PENDING
    private JsObject CloneError(JsCloneRecord record)
    {
        var prototype = ErrorPrototype;

        if (record.Text is { } name &&
            ErrorConstructors.TryGetValue(name, out var constructor) &&
            constructor.TryGetOwnProperty("prototype", out var held) &&
            !held.IsAccessor &&
            held.Value.IsObject)
        {
            prototype = held.Value.AsObject();
        }

        var error = new JsObject(prototype, "Error");

        // AN ABSENT MESSAGE STAYS ABSENT, so the clone of `new Error()` has no own `message` and
        // reads the empty string off its prototype, exactly as the original did.
        if (record.Detail is { } message)
        {
            error.SetOwnProperty(
                "message", JsProperty.Data(JsValue.String(message), JsPropertyAttributes.BuiltIn));
        }

        return error;
    }

    /// <summary>Builds a fresh buffer holding a copy of the carried bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A67638
    // Broiler-Falsified-If: the destination buffer shares its byte array with the carrier or another adoption
    // Broiler-Human:        PENDING
    private JsArrayBuffer CloneBuffer(JsCloneRecord record)
    {
        // A TRANSFERRED RECORD WITHOUT ITS BYTES has been adopted already. The claim in
        // CloneDeserialize refuses that first; this keeps the record from ever being rebuilt as an
        // empty buffer standing in for the one that moved.
        if (record.Transferred && record.Bytes is null)
        {
            throw new JsCloneRefusedException(
                JsCloneRefusal.Consumed, "the carrier's transferred buffers were already adopted");
        }

        var bytes = record.Bytes ?? [];

        // FUEL FIRST, ALLOCATION SECOND, RETENTION REPORTED THIRD, in the order BinaryNewBuffer
        // gives and for its reason. The copy is what keeps a second adoption of the same carrier
        // from writing into the first one's bytes.
        engine.Charge((ulong)bytes.Length);
        var buffer = new JsArrayBuffer(ArrayBufferPrototype, bytes.Length);
        engine.Retain((ulong)bytes.Length);
        _ = buffer.TryCopyFrom(bytes, 0, bytes.Length);

        // MOVED BYTES LEAVE THE CARRIER WITH THE ADOPTION THAT CLAIMED THEM. They are copied once
        // rather than handed over, because JsArrayBuffer has no constructor that adopts an array
        // and cards F04-F06 are reshaping that type; the copy is charged like any other. Dropping
        // the carrier's reference lets the source's array go.
        if (record.Transferred)
        {
            record.Bytes = null;
        }

        return buffer;
    }

    /// <summary>Builds a typed array or DataView over the buffer its record names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=36BE86
    // Broiler-Falsified-If: a view is built over bytes outside its buffer, or two views of one source buffer see two buffers
    // Broiler-Human:        PENDING
    private JsObject CloneView(JsCloneRecord record, JsObject?[] built)
    {
        if (record.Buffer < 0 || record.Buffer >= built.Length || built[record.Buffer] is not JsArrayBuffer buffer)
        {
            throw new JsCloneRefusedException(
                JsCloneRefusal.Unrepresentable, "the carrier names a view without a buffer");
        }

        if (record.Kind == JsCloneKind.DataView)
        {
            if (record.Offset < 0 || record.Count < 0 || (long)record.Offset + record.Count > buffer.ByteLength)
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.Unrepresentable, "the carrier names a DataView outside its buffer");
            }

            return new JsDataView(DataViewPrototype, buffer, record.Offset, record.Count);
        }

        foreach (var kind in JsElements.All)
        {
            // A KIND THIS REALM DID NOT BUILD IS A NAME IT HAS NO CONSTRUCTOR FOR: a BigInt64Array
            // carried into a realm whose composition declines BigInt is refused below, exactly as an
            // unknown name is, rather than rebuilt over a prototype that does not exist (JSeal B07).
            if (!string.Equals(JsElements.ConstructorNameOf(kind), record.Text, System.StringComparison.Ordinal) ||
                !TypedArrayPrototypes.ContainsKey(kind))
            {
                continue;
            }

            var width = JsElements.WidthOf(kind);

            if (record.Offset < 0 || record.Count < 0 || record.Offset % width != 0 ||
                (long)record.Offset + ((long)record.Count * width) > buffer.ByteLength)
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.Unrepresentable, "the carrier names a typed array outside its buffer");
            }

            return new JsTypedArray(TypedArrayPrototypes[kind], buffer, record.Offset, record.Count, kind);
        }

        throw new JsCloneRefusedException(
            JsCloneRefusal.Unrepresentable,
            "this realm has no typed array constructor named " + (record.Text ?? "(none)"));
    }

    /// <summary>Fills one rebuilt object with its properties, entries or cause.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5162CA
    // Broiler-Human:        PENDING
    private void CloneFill(JsCloneRecord record, JsObject target, JsObject?[] built)
    {
        switch (record.Kind)
        {
            case JsCloneKind.Object:
            case JsCloneKind.Array:
            {
                var keys = record.Keys;
                var values = record.Values;

                for (var at = 0; at < keys.Length && at < values.Length; at++)
                {
                    engine.Charge(1);

                    // DEFINED, NOT ASSIGNED: HTML's CreateDataProperty. The target is fresh, so no
                    // setter on the way can run, and an index key on an Array lands as an element.
                    target.SetOwnProperty(
                        keys[at],
                        JsProperty.Data(CloneValueOf(values[at], built), JsPropertyAttributes.Default));
                }

                return;
            }

            case JsCloneKind.Map:
            {
                var table = ((JsMapObject)target).Table;
                var values = record.Values;

                for (var at = 0; at + 1 < values.Length; at += 2)
                {
                    engine.Charge(1);
                    table.Set(CloneValueOf(values[at], built), CloneValueOf(values[at + 1], built));
                }

                return;
            }

            case JsCloneKind.Set:
            {
                var table = ((JsSetObject)target).Table;

                foreach (var member in record.Values)
                {
                    engine.Charge(1);
                    var value = CloneValueOf(member, built);
                    table.Set(value, value);
                }

                return;
            }

            case JsCloneKind.Error when record.HasCause && record.Values.Length == 1:
                engine.Charge(1);
                target.SetOwnProperty(
                    "cause",
                    JsProperty.Data(CloneValueOf(record.Values[0], built), JsPropertyAttributes.BuiltIn));

                return;

            default:
                return;
        }
    }

    /// <summary>The value one carrier slot names in this realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0AB677
    // Broiler-Human:        PENDING
    private JsValue CloneValueOf(JsCloneSlot slot, JsObject?[] built) => slot.Kind switch
    {
        JsCloneSlotKind.Undefined => JsValue.Undefined,
        JsCloneSlotKind.Null => JsValue.Null,
        JsCloneSlotKind.Boolean => JsValue.Boolean(slot.Number != 0),
        JsCloneSlotKind.Number => JsValue.Number(slot.Number),
        JsCloneSlotKind.String => JsValue.String(slot.Text ?? string.Empty),
        JsCloneSlotKind.BigInt => CloneBigInt(slot),
        _ when slot.Record >= 0 && slot.Record < built.Length && built[slot.Record] is { } target =>
            JsValue.Object(target),
        _ => throw new JsCloneRefusedException(
            JsCloneRefusal.Unrepresentable, "the carrier names a record it does not hold"),
    };

    /// <summary>A carrier's BigInt in this realm, charged per word as the sender's was (card B06).</summary>
    /// <remarks>
    /// The integer is shared, not copied: it is immutable and names no realm. The carrier's
    /// <see cref="JsCloneCarrier.HoldsBigInt"/> was checked against this realm's surfaces before the
    /// rebuild began, so a declining realm never reaches here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4CF6A0
    // Broiler-Falsified-If: a BigInt slot is rebuilt uncharged, or a slot of another kind is read as a BigInt
    // Broiler-Human:        PENDING
    private JsValue CloneBigInt(JsCloneSlot slot)
    {
        var integer = slot.Kind == JsCloneSlotKind.BigInt && slot.BigInt is { } held
            ? held
            : throw new JsCloneRefusedException(
                JsCloneRefusal.Unrepresentable, "the carrier's BigInt record holds no BigInt");

        engine.Charge(JsBigInt.LinearCost(integer.Words));
        return JsValue.BigInt(integer);
    }

    /// <summary>
    /// Whether a plain object's tag says it carries internal state the matrix does not list.
    /// </summary>
    /// <remarks>
    /// <b>These are plain objects in this implementation and slot-carrying objects in the
    /// specification.</b> An arguments object has <c>[[ParameterMap]]</c> and a built-in iterator
    /// has its own slots, so HTML refuses both; here each is an ordinary object whose state lives in
    /// a closure, and cloning it as an ordinary object would answer <c>{}</c> where the language
    /// owes a refusal.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=66D826
    // Broiler-Human:        PENDING
    private static bool CloneIsSlottedTag(string tag) =>
        string.Equals(tag, "Arguments", System.StringComparison.Ordinal) ||
        string.Equals(tag, "Iterator Helper", System.StringComparison.Ordinal) ||
        tag.EndsWith(" Iterator", System.StringComparison.Ordinal);

    /// <summary>One serialization: the memory map, the records, the frames and the two meters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=499643
    // Broiler-Falsified-If: one source object produces two records, or a bound is passed without a refusal
    // Broiler-Human:        PENDING
    private sealed class CloneWriter
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3283F6
        // Broiler-Human:        PENDING
        private readonly JsEngine engine;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EA45E0
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.List<JsCloneRecord> records = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7A7021
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.Dictionary<JsObject, int> memory =
            new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9E8D89
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.HashSet<object> counted =
            new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1CF320
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.Stack<CloneFrame> frames = new();

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=88E752
        // Broiler-Falsified-If: this bound is larger than JsCloneCarrier.MaxEntries
        // Broiler-Human:        PENDING
        private readonly long maxEntries;

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5AF476
        // Broiler-Falsified-If: this bound is larger than JsCloneCarrier.MaxBytes
        // Broiler-Human:        PENDING
        private readonly long maxBytes;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=467DF6
        // Broiler-Human:        PENDING
        private long entries;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6FCE12
        // Broiler-Human:        PENDING
        private long bytes;

        /// <summary>Whether a BigInt slot was written (card B06).</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F85E91
        // Broiler-Human:        PENDING
        private bool holdsBigInt;

        /// <summary>The part of <see cref="bytes"/> that transferred buffers move rather than copy.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=01489A
        // Broiler-Human:        PENDING
        private long moved;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A139B7
        // Broiler-Human:        PENDING
        internal CloneWriter(JsRealm owner, long entryBound, long byteBound)
        {
            engine = owner.engine;
            maxEntries = entryBound;
            maxBytes = byteBound;
        }

        /// <summary>Serializes the root, then drains the frames depth first, in HTML's order.</summary>
        /// <remarks>
        /// <b>The top frame is always the object most recently met</b>, so a value's whole subgraph
        /// is serialized before its parent reads its next key - which is the order in which HTML's
        /// recursive algorithm runs getters, and the order a program observing them sees.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=549634
        // Broiler-Falsified-If: getters run in an order other than HTML's recursive depth-first order
        // Broiler-Human:        PENDING
        internal JsCloneCarrier Run(JsValue value, JsValue[] transfer)
        {
            var listed = CheckTransferList(transfer);
            var root = Slot(value);

            while (frames.Count > 0)
            {
                var frame = frames.Peek();

                if (frame.Keys is { } keys)
                {
                    if (frame.At >= keys.Count)
                    {
                        frame.Seal();
                        _ = frames.Pop();
                        continue;
                    }

                    var key = keys[frame.At++];
                    engine.Charge(1);

                    // HTML'S HasOwnProperty CHECK: a getter that ran earlier may have deleted a
                    // later key, and a deleted key is skipped rather than read as undefined.
                    if (!frame.Source.TryGetOwnProperty(key, out _))
                    {
                        continue;
                    }

                    var read = engine.GetProperty(JsValue.Object(frame.Source), key);
                    CountText(key);
                    var slot = Slot(read);
                    frame.OutKeys.Add(key);
                    frame.OutValues.Add(slot);
                    continue;
                }

                var pending = frame.Pending!;

                if (frame.At >= pending.Count)
                {
                    frame.Seal();
                    _ = frames.Pop();
                    continue;
                }

                engine.Charge(1);
                frame.OutValues.Add(Slot(pending[frame.At++]));
            }

            Validate(listed);

            // THE SENDER PAYS FOR THE CARRIER (card I17, decision JSD-0032 section 4), and it pays
            // before the commit: a live-bytes ceiling that refuses the carrier ends the operation
            // while every listed buffer is still attached. Moved bytes are not charged again; the
            // sender retained them when it made the buffers and is not credited when they leave.
            var charged = JsCloneCarrier.ChargeFor(entries, bytes - moved);
            engine.RetainOrAbort((ulong)charged);

            // THE COMMIT: every check has passed, and from here nothing charges, counts or throws.
            // Each listed buffer lets go of its array and the carrier's record takes it, so every
            // view over the source observes a detached buffer from this instant on.
            foreach (var (buffer, record) in listed)
            {
                record.Bytes = buffer.Detach();
            }

            return new JsCloneCarrier([.. records], root, entries, bytes, listed.Count, charged, holdsBigInt);
        }

        /// <summary>
        /// HTML's first transfer loop: each entry an ArrayBuffer, none twice, and each entered in
        /// the memory map before the graph is walked.
        /// </summary>
        /// <remarks>
        /// Nothing about a listed buffer's bytes is read here. HTML does not ask about detachment
        /// until the walk is over, and a getter can still detach a buffer after this loop.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D25FD2
        // Broiler-Falsified-If: a transfer list naming a non-ArrayBuffer or one buffer twice is admitted
        // Broiler-Human:        PENDING
        private System.Collections.Generic.List<(JsArrayBuffer Buffer, JsCloneRecord Record)> CheckTransferList(
            JsValue[] transfer)
        {
            var listed = new System.Collections.Generic.List<(JsArrayBuffer, JsCloneRecord)>(transfer.Length);

            foreach (var entry in transfer)
            {
                engine.Charge(4);
                Count(1);

                // THIS REALM HAS NO SharedArrayBuffer (JSD-0028) and no transferable platform
                // object, so an entry is an ArrayBuffer or it is refused.
                if (entry.AsObjectOrNull() is not JsArrayBuffer buffer)
                {
                    throw new JsCloneRefusedException(
                        JsCloneRefusal.NotTransferable, "only an ArrayBuffer can be transferred");
                }

                if (memory.ContainsKey(buffer))
                {
                    throw new JsCloneRefusedException(
                        JsCloneRefusal.DuplicateTransfer, "the transfer list names an ArrayBuffer twice");
                }

                // A RESIZABLE BUFFER IS NOT TRANSFERABLE YET, and is refused before any getter runs:
                // moving its bytes into a fixed record would hand the destination a buffer that has
                // stopped being resizable (JSD-0032 section 5a).
                if (buffer.IsResizable)
                {
                    throw new JsCloneRefusedException(
                        JsCloneRefusal.NotTransferable, "a resizable ArrayBuffer cannot be transferred");
                }

                _ = New(buffer, JsCloneKind.ArrayBuffer, out var record);
                record.Transferred = true;
                listed.Add((buffer, record));
            }

            return listed;
        }

        /// <summary>
        /// HTML's second transfer loop, less its detachments: each listed buffer still attached,
        /// and its bytes within the bound.
        /// </summary>
        /// <remarks>
        /// <b>Moved bytes count against <see cref="JsCloneCarrier.MaxBytes"/> as copied ones do</b>,
        /// because the carrier holds them either way; the bound is a statement about the carrier.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E91B98
        // Broiler-Falsified-If: a detached buffer or one past the byte bound reaches the commit
        // Broiler-Human:        PENDING
        private void Validate(System.Collections.Generic.List<(JsArrayBuffer Buffer, JsCloneRecord Record)> listed)
        {
            foreach (var (buffer, _) in listed)
            {
                if (buffer.IsDetached)
                {
                    throw new JsCloneRefusedException(
                        JsCloneRefusal.DetachedBuffer, "a detached ArrayBuffer could not be transferred");
                }

                bytes += buffer.ByteLength;
                moved += buffer.ByteLength;

                if (bytes > maxBytes)
                {
                    throw TooLarge();
                }
            }
        }

        /// <summary>Serializes one value; an object gets a record, and a frame if it has contents.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DF1346
        // Broiler-Falsified-If: a Symbol value serializes rather than being refused
        // Broiler-Human:        PENDING
        private JsCloneSlot Slot(JsValue value)
        {
            Count(1);

            switch (value.Type)
            {
                case JsType.Empty:
                case JsType.Undefined:
                    return JsCloneSlot.Undefined;

                case JsType.Null:
                    return JsCloneSlot.Null;

                case JsType.Boolean:
                    return JsCloneSlot.Boolean(value.AsBoolean());

                case JsType.Number:
                    return JsCloneSlot.NumberOf(value.AsNumber());

                case JsType.String:
                {
                    var text = value.AsString();
                    CountText(text);
                    return JsCloneSlot.StringOf(text);
                }

                case JsType.Symbol:
                    throw new JsCloneRefusedException(JsCloneRefusal.Symbol, "a Symbol could not be cloned");

                // HTML SERIALISES A BIGINT PRIMITIVE AS ITS VALUE (card B06, JSD-0032's matrix).
                case JsType.BigInt:
                    return JsCloneSlot.BigIntOf(CountBigInt(value.AsBigInt()));

                default:
                    return JsCloneSlot.Reference(Record(value.AsObject()));
            }
        }

        /// <summary>The record for one object: the one it already has, or a new one for its brand.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=1A9F4B
        // Broiler-Falsified-If: an object whose brand is not in the matrix is recorded as any kind at all
        // Broiler-Human:        PENDING
        private int Record(JsObject source)
        {
            if (memory.TryGetValue(source, out var known))
            {
                return known;
            }

            engine.Charge(4);
            Count(1);

            switch (source)
            {
                // A PROXY IS NAMED BEFORE A FUNCTION, because a callable proxy is both and the
                // refusal should say which rule it broke first: HTML refuses an exotic object
                // whatever its target is.
                case JsProxy:
                    throw new JsCloneRefusedException(JsCloneRefusal.Proxy, "a Proxy could not be cloned");

                case { IsCallable: true }:
                    throw new JsCloneRefusedException(JsCloneRefusal.Function, "a function could not be cloned");

                case JsPrimitiveWrapper wrapper:
                    return Wrapper(source, wrapper.Primitive);

                case DateObject date:
                {
                    var index = New(source, JsCloneKind.Date, out var record);
                    record.Number = date.TimeValue;
                    return index;
                }

                case RegExpObject regExp:
                {
                    // [[OriginalSource]] and [[OriginalFlags]]; lastIndex and every own property
                    // stay behind, as HTML says.
                    CountText(regExp.Source);
                    var index = New(source, JsCloneKind.RegExp, out var record);
                    record.Text = regExp.Source;
                    record.Detail = regExp.Flags;
                    return index;
                }

                case JsArrayBuffer buffer:
                    return Buffer(source, buffer);

                case JsTypedArray view:
                {
                    RefuseDetachedView(view.IsDetached);
                    RefuseResizable(view.Buffer);
                    var index = New(source, JsCloneKind.TypedArray, out var record);
                    record.Text = JsElements.ConstructorNameOf(view.Kind);
                    record.Offset = view.ByteOffset;
                    record.Count = view.Length;
                    record.Buffer = Record(view.Buffer);
                    return index;
                }

                case JsDataView view:
                {
                    RefuseDetachedView(view.IsDetached);
                    RefuseResizable(view.Buffer);
                    var index = New(source, JsCloneKind.DataView, out var record);
                    record.Offset = view.ByteOffset;
                    record.Count = view.ByteLength;
                    record.Buffer = Record(view.Buffer);
                    return index;
                }

                case JsMapObject map:
                    return Keyed(source, JsCloneKind.Map, map.Table, pairs: true);

                case JsSetObject set:
                    return Keyed(source, JsCloneKind.Set, set.Table, pairs: false);

                case JsArray array:
                {
                    // THE LENGTH IS READ WHEN THE RECORD IS MADE, AS HTML READS IT, so a getter
                    // that later pushes onto the Array does not move the clone's length.
                    var index = New(source, JsCloneKind.Array, out var record);
                    record.Number = array.Length;
                    Push(source, record, EnumerableKeys(source), null);
                    return index;
                }

                default:
                    break;
            }

            if (source.GetType() != typeof(JsObject))
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.UnsupportedBrand, "a " + source.ClassName + " object could not be cloned");
            }

            // THE CLASS NAME IS THE [[ErrorData]] TEST. The Error and NativeError prototypes are
            // ordinary objects with class name `Object` (JSeal VM-FIX-I), so they clone as ordinary
            // objects without a separate identity check; that check was removed as redundant
            // (JSeal VM-FIX-J).
            if (string.Equals(source.ClassName, "Error", System.StringComparison.Ordinal))
            {
                return Error(source);
            }

            if (CloneIsSlottedTag(source.ClassName))
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.UnsupportedBrand, "a " + source.ClassName + " object could not be cloned");
            }

            var ordinary = New(source, JsCloneKind.Object, out var plain);
            Push(source, plain, EnumerableKeys(source), null);
            return ordinary;
        }

        /// <summary>
        /// Refuses a view whose buffer is detached. Asked of the view itself, not left to
        /// <see cref="Buffer"/>: a buffer the memory map already holds is not visited again, so a
        /// getter that detached it after it was recorded would otherwise leave the view cloned over
        /// the bytes copied before (HTML: IsArrayBufferViewOutOfBounds throws DataCloneError).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=551994
        // Broiler-Falsified-If: a typed array or DataView over a detached buffer serializes, whether or not its buffer was recorded first
        // Broiler-Human:        PENDING
        private static void RefuseDetachedView(bool detached)
        {
            if (detached)
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.DetachedBuffer, "a view over a detached ArrayBuffer could not be cloned");
            }
        }

        /// <summary>
        /// Refuses a resizable buffer, and so every view over one: JSD-0032's matrix entry for
        /// resizable buffers, taken as written.
        /// </summary>
        /// <remarks>
        /// A record here carries a byte count and a view's fixed length; it has no place for a
        /// maximum or for "tracks the buffer", so cloning a resizable buffer as a fixed one would
        /// hand the destination a buffer that answers <c>resizable === false</c> and views that no
        /// longer follow it - a wrong answer, where the decision asks for a refusal until the
        /// carrier can say both. The view is refused before its offset or length is read, so an
        /// out-of-bounds view is never measured.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=22B2B9
        // Broiler-Falsified-If: a resizable ArrayBuffer, or a typed array or DataView over one, serializes
        // Broiler-Human:        PENDING
        private static void RefuseResizable(JsArrayBuffer buffer)
        {
            if (buffer.IsResizable)
            {
                throw new JsCloneRefusedException(
                    JsCloneRefusal.UnsupportedBrand, "a resizable ArrayBuffer could not be cloned");
            }
        }

        /// <summary>A Boolean, Number or String wrapper; a Symbol wrapper is refused.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C9C787
        // Broiler-Human:        PENDING
        private int Wrapper(JsObject source, JsValue primitive)
        {
            switch (primitive.Type)
            {
                case JsType.Boolean:
                {
                    var index = New(source, JsCloneKind.Boolean, out var record);
                    record.Number = primitive.AsBoolean() ? 1 : 0;
                    return index;
                }

                case JsType.Number:
                {
                    var index = New(source, JsCloneKind.Number, out var record);
                    record.Number = primitive.AsNumber();
                    return index;
                }

                case JsType.String:
                {
                    var text = primitive.AsString();
                    CountText(text);
                    var index = New(source, JsCloneKind.String, out var record);
                    record.Text = text;
                    return index;
                }

                // A BIGINT OBJECT CLONES AS A BIGINT OBJECT (card B06): HTML's [[BigIntData]] row,
                // its integer and nothing else, as for the other three wrappers.
                case JsType.BigInt:
                {
                    Count(1);
                    var integer = CountBigInt(primitive.AsBigInt());
                    var index = New(source, JsCloneKind.BigInt, out var record);
                    record.Values = [JsCloneSlot.BigIntOf(integer)];
                    return index;
                }

                default:
                    throw new JsCloneRefusedException(
                        JsCloneRefusal.Symbol, "a Symbol object could not be cloned");
            }
        }

        /// <summary>A fixed-length buffer: its bytes, copied now, charged per byte.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=BF8169
        // Broiler-Falsified-If: the carrier holds the source buffer's own byte array, or a detached buffer serializes
        // Broiler-Human:        PENDING
        private int Buffer(JsObject source, JsArrayBuffer buffer)
        {
            var data = buffer.Data ?? throw new JsCloneRefusedException(
                JsCloneRefusal.DetachedBuffer, "a detached ArrayBuffer could not be cloned");

            RefuseResizable(buffer);

            bytes += data.Length;

            if (bytes > maxBytes)
            {
                throw TooLarge();
            }

            // CHARGED BEFORE THE COPY, and the copy is the point: a later write to the source
            // buffer must not reach the carrier, so the carrier cannot hold the source's array.
            engine.Charge((ulong)data.Length);
            var index = New(source, JsCloneKind.ArrayBuffer, out var record);
            record.Bytes = (byte[])data.Clone();
            return index;
        }

        /// <summary>A Map or Set: its live entries, snapshotted now, serialized by a frame.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=CEAC6C
        // Broiler-Human:        PENDING
        private int Keyed(JsObject source, JsCloneKind kind, JsKeyedTable table, bool pairs)
        {
            var index = New(source, kind, out var record);

            // THE SNAPSHOT IS HTML'S copiedList: a getter reached while the entries are serialized
            // may add to or delete from the table, and neither changes what this clone holds.
            var pending = new System.Collections.Generic.List<JsValue>(pairs ? table.Count * 2 : table.Count);

            for (var slot = 0; slot < table.SlotCount; slot++)
            {
                engine.Charge(1);

                if (!table.TryAt(slot, out var key, out var value))
                {
                    continue;
                }

                pending.Add(key);

                if (pairs)
                {
                    pending.Add(value);
                }
            }

            Push(source, record, null, pending);
            return index;
        }

        /// <summary>An Error: HTML's name rule, the own data <c>message</c>, and an own data <c>cause</c>.</summary>
        /// <remarks>
        /// <para>
        /// <b>The name is read with <c>[[Get]]</c> and may run a getter</b>; a name outside the fixed
        /// list, including <c>AggregateError</c> and any non-string, becomes <c>Error</c>. The message
        /// is read as an OWN DATA property only - an accessor or an inherited message clones as no
        /// message - and converted with ToString, which may run guest code.
        /// </para>
        /// <para>
        /// <b>The cause is the one field HTML does not name.</b> HTML lets a user agent attach
        /// "interesting accompanying data", and the engines this is compared against attach an own
        /// data <c>cause</c>, serialized like any other value. Own enumerable properties, the stack
        /// and an AggregateError's <c>errors</c> are not carried.
        /// </para>
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D72E0A
        // Broiler-Human:        PENDING
        private int Error(JsObject source)
        {
            var index = New(source, JsCloneKind.Error, out var record);
            var named = engine.GetProperty(JsValue.Object(source), "name");

            record.Text = named.IsString && System.Array.IndexOf(CloneErrorNames, named.AsString()) >= 0
                ? named.AsString()
                : "Error";

            if (source.TryGetOwnProperty("message", out var message) && !message.IsAccessor)
            {
                var text = engine.ToStringValue(message.Value);
                CountText(text);
                record.Detail = text;
            }

            if (source.TryGetOwnProperty("cause", out var cause) && !cause.IsAccessor)
            {
                record.HasCause = true;
                Push(source, record, null, [cause.Value]);
            }

            return index;
        }

        /// <summary>The own enumerable string keys, in the specification's order, as of now.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=EC0AC7
        // Broiler-Human:        PENDING
        private System.Collections.Generic.List<string> EnumerableKeys(JsObject source)
        {
            var names = source.OwnPropertyNames();
            engine.Charge((ulong)names.Count + 1);

            var keys = new System.Collections.Generic.List<string>(names.Count);

            foreach (var name in names)
            {
                if (source.TryGetOwnProperty(name, out var property) && property.Enumerable)
                {
                    keys.Add(name);
                }
            }

            return keys;
        }

        /// <summary>Adds a record and puts its object in the memory map before anything inside it is met.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E54E31
        // Broiler-Falsified-If: an object is entered in the memory map after its contents are serialized
        // Broiler-Human:        PENDING
        private int New(JsObject source, JsCloneKind kind, out JsCloneRecord record)
        {
            record = new JsCloneRecord(kind);
            var index = records.Count;
            records.Add(record);
            memory[source] = index;
            return index;
        }

        /// <summary>Queues an object's contents to be serialized next.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2CFA59
        // Broiler-Human:        PENDING
        private void Push(
            JsObject source,
            JsCloneRecord record,
            System.Collections.Generic.List<string>? keys,
            System.Collections.Generic.List<JsValue>? pending) =>
            frames.Push(new CloneFrame(source, record, keys, pending));

        /// <summary>Counts entries against <see cref="JsCloneCarrier.MaxEntries"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CDB11E
        // Broiler-Falsified-If: an entry is added past the bound without a refusal
        // Broiler-Human:        PENDING
        private void Count(long added)
        {
            entries += added;

            if (entries > maxEntries)
            {
                throw TooLarge();
            }
        }

        /// <summary>Counts a string's bytes once, however many times the graph names it.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=2BB13D
        // Broiler-Falsified-If: distinct string text is carried past the byte bound without a refusal
        // Broiler-Human:        PENDING
        private void CountText(string text)
        {
            if (!counted.Add(text))
            {
                return;
            }

            bytes += 2L * text.Length;

            if (bytes > maxBytes)
            {
                throw TooLarge();
            }
        }

        /// <summary>
        /// Counts a BigInt's words against <see cref="JsCloneCarrier.MaxBytes"/> as eight bytes each,
        /// and charges them, before the carrier holds it (card B06).
        /// </summary>
        /// <remarks>
        /// The integer is immutable, so the carrier shares it rather than copying it; what is bounded
        /// and charged is what a destination will hold, since every adoption builds a value of that
        /// width. Each occurrence counts: two slots naming one integer are two values, as two
        /// Numbers are.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=0651DE
        // Broiler-Falsified-If: a BigInt is carried past the byte bound without a refusal, or carried uncharged
        // Broiler-Human:        PENDING
        private JsBigInt CountBigInt(JsBigInt value)
        {
            bytes += 8L * value.Words;

            if (bytes > maxBytes)
            {
                throw TooLarge();
            }

            engine.Charge(JsBigInt.LinearCost(value.Words));
            holdsBigInt = true;
            return value;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8D02BC
        // Broiler-Human:        PENDING
        private static JsCloneRefusedException TooLarge() =>
            new(JsCloneRefusal.TooLarge, "the value is larger than one clone may carry");
    }

    /// <summary>One object whose contents are still being serialized.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B7C5F2
    // Broiler-Human:        PENDING
    private sealed class CloneFrame(
        JsObject source,
        JsCloneRecord record,
        System.Collections.Generic.List<string>? keys,
        System.Collections.Generic.List<JsValue>? pending)
    {
        /// <summary>The object being walked.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6225DE
        // Broiler-Human:        PENDING
        internal JsObject Source { get; } = source;

        /// <summary>The keys still to read, for an ordinary object or an Array.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=29CEDB
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string>? Keys { get; } = keys;

        /// <summary>The values still to serialize, for a Map, a Set or an Error's cause.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5C55E6
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsValue>? Pending { get; } = pending;

        /// <summary>How far the walk has got.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=002A46
        // Broiler-Human:        PENDING
        internal int At { get; set; }

        /// <summary>The keys serialized so far.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CF5195
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> OutKeys { get; } = [];

        /// <summary>The values serialized so far.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=804C7F
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsCloneSlot> OutValues { get; } = [];

        /// <summary>Writes what was serialized into the record, once the walk is done.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=38F290
        // Broiler-Human:        PENDING
        internal void Seal()
        {
            record.Keys = [.. OutKeys];
            record.Values = [.. OutValues];
        }
    }
}
